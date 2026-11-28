#!/usr/bin/env node
/**
 * Strict-CSP, offline, headless-Chromium (CDP) matrix B01-B20 for the MapLibre Display Adapter.
 *
 * - Loopback static server (server.mjs) sends the exact strict CSP + Referrer-Policy on every response.
 * - Chromium uses SwiftShader WebGL (no --disable-gpu), --host-resolver-rules that fail every
 *   hostname except 127.0.0.1, and a dead proxy so even IP-literal outbound requests fail loudly.
 * - Dedicated workers are auto-attached so worker console errors and worker URLs are audited too.
 *
 * Usage: node drive-maplibre-cdp.mjs --out <evidence-dir>
 * Exit 0 only if all 20 cases pass and the audit has zero console errors, CSP violations,
 * failed requests and non-loopback requests. Exit 2 when WebGL2 is unavailable (NOT TESTED).
 */

import { spawn } from 'node:child_process';
import { mkdirSync, mkdtempSync, rmSync, writeFileSync } from 'node:fs';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import net from 'node:net';
import { REFERRER_POLICY, STRICT_CSP_HEADER, startHarnessServer } from './server.mjs';
import { decodePngRgba, meanRgbInBox } from './synthetic-png.mjs';

const argv = process.argv.slice(2);
const outIndex = argv.indexOf('--out');
const evidenceDir = outIndex >= 0 ? argv[outIndex + 1] : join(tmpdir(), 'ccf-maplibre-evidence');
const CHROMIUM = process.env.CHROMIUM || '/usr/bin/chromium';
const W = 1280;
const H = 720;
const BG = [21, 37, 26]; // #15251a
const IMG = [196, 168, 92]; // synthetic imagery margin colour
const WORKER_PATH = '/map/maplibre/vendor/maplibre-gl-6.20.0/maplibre-gl-worker.mjs';
mkdirSync(evidenceDir, { recursive: true });

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));
const results = [];
function record(id, ok, detail) {
  results.push({ id, status: ok ? 'PASS' : 'FAIL', detail });
  console.log(`${ok ? 'PASS' : 'FAIL'} ${id}: ${detail}`);
}

// ---- CDP plumbing ---------------------------------------------------------------------------

class Cdp {
  constructor(ws) {
    this.ws = ws;
    this.nextId = 0;
    this.pending = new Map();
    this.listeners = [];
  }

  static async connect(url) {
    const ws = new WebSocket(url);
    await new Promise((ok, fail) => {
      ws.addEventListener('open', ok, { once: true });
      ws.addEventListener('error', () => fail(new Error('CDP websocket failed')), { once: true });
    });
    const cdp = new Cdp(ws);
    ws.addEventListener('message', (event) => {
      const msg = JSON.parse(event.data);
      if (msg.id && cdp.pending.has(msg.id)) {
        const { ok, fail } = cdp.pending.get(msg.id);
        cdp.pending.delete(msg.id);
        if (msg.error) fail(new Error(msg.error.message));
        else ok(msg.result);
      } else if (msg.method) {
        for (const fn of cdp.listeners) fn(msg);
      }
    });
    return cdp;
  }

  send(method, params = {}, sessionId = undefined) {
    const id = ++this.nextId;
    return new Promise((ok, fail) => {
      this.pending.set(id, { ok, fail });
      this.ws.send(JSON.stringify(sessionId ? { id, method, params, sessionId } : { id, method, params }));
    });
  }

  close() { try { this.ws.close(); } catch { /* closed */ } }
}

function freePort() {
  return new Promise((ok, fail) => {
    const s = net.createServer();
    s.once('error', fail);
    s.listen(0, '127.0.0.1', () => { const { port } = s.address(); s.close(() => ok(port)); });
  });
}

async function launchChromium() {
  const port = await freePort();
  const profile = mkdtempSync(join(tmpdir(), 'ccf-maplibre-cdp-'));
  const child = spawn(CHROMIUM, [
    '--headless=new',
    '--use-gl=angle',
    '--use-angle=swiftshader',
    '--enable-unsafe-swiftshader',
    '--no-first-run',
    '--no-default-browser-check',
    '--disable-extensions',
    '--disable-background-networking',
    '--disable-component-update',
    '--disable-sync',
    '--disable-default-apps',
    '--no-pings',
    '--hide-scrollbars',
    '--force-device-scale-factor=1',
    `--window-size=${W},${H}`,
    '--host-resolver-rules=MAP * ~NOTFOUND, EXCLUDE 127.0.0.1',
    '--proxy-server=http://127.0.0.1:9',
    `--remote-debugging-port=${port}`,
    `--user-data-dir=${profile}`,
    'about:blank',
  ], { stdio: 'ignore', detached: true });
  if (process.env.CCF_CHROMIUM_PIDFILE) writeFileSync(process.env.CCF_CHROMIUM_PIDFILE, String(child.pid));

  let target = null;
  for (let i = 0; i < 200 && !target; i++) {
    await sleep(100);
    try {
      const list = await (await fetch(`http://127.0.0.1:${port}/json/list`)).json();
      target = list.find((t) => t.type === 'page');
    } catch { /* not up yet */ }
  }
  const stop = async () => {
    try { process.kill(-child.pid, 'SIGTERM'); } catch { /* gone */ }
    for (let i = 0; i < 30 && child.exitCode === null && child.signalCode === null; i++) await sleep(100);
    try { process.kill(-child.pid, 'SIGKILL'); } catch { /* gone */ }
    rmSync(profile, { recursive: true, force: true });
  };
  if (!target) { await stop(); throw new Error('Chromium exposed no CDP page target'); }
  return { cdp: await Cdp.connect(target.webSocketDebuggerUrl), stop };
}

// ---- audit: everything the page and its workers report --------------------------------------

function createAudit(cdp) {
  const audit = {
    consoleErrors: [],
    cspViolations: [],
    failedRequests: [],
    nonLoopbackRequests: [],
    httpRequests: 0,
    responses: [],
    workers: [],
  };
  const isCsp = (t) => /Content[- ]Security[- ]Policy|Refused to/i.test(t);
  cdp.listeners.push((msg) => {
    const p = msg.params || {};
    switch (msg.method) {
      case 'Target.attachedToTarget':
        audit.workers.push({ url: p.targetInfo.url, type: p.targetInfo.type, at: Date.now() });
        for (const method of ['Runtime.enable', 'Log.enable', 'Network.enable']) {
          cdp.send(method, {}, p.sessionId).catch(() => {});
        }
        cdp.send('Runtime.runIfWaitingForDebugger', {}, p.sessionId).catch(() => {});
        break;
      case 'Runtime.exceptionThrown':
        audit.consoleErrors.push('exception: ' + (p.exceptionDetails?.exception?.description || p.exceptionDetails?.text));
        break;
      case 'Runtime.consoleAPICalled':
        if (p.type === 'error' || p.type === 'assert') {
          const text = (p.args || []).map((a) => a.value ?? a.description ?? '').join(' ');
          (isCsp(text) ? audit.cspViolations : audit.consoleErrors).push(text);
        }
        break;
      case 'Log.entryAdded':
        if (p.entry?.level === 'error') {
          const text = `${p.entry.source}: ${p.entry.text} ${p.entry.url || ''}`;
          (isCsp(text) ? audit.cspViolations : audit.consoleErrors).push(text);
        }
        break;
      case 'Network.requestWillBeSent': {
        const url = p.request?.url || '';
        if (/^https?:/.test(url)) {
          audit.httpRequests++;
          if (new URL(url).hostname !== '127.0.0.1') audit.nonLoopbackRequests.push(url);
        } else if (!/^(data|blob|about):/.test(url)) {
          audit.nonLoopbackRequests.push(url);
        }
        break;
      }
      case 'Network.responseReceived': {
        const r = p.response || {};
        const headers = Object.fromEntries(Object.entries(r.headers || {}).map(([k, v]) => [k.toLowerCase(), v]));
        if (/^https?:/.test(r.url || '')) audit.responses.push({ url: r.url, status: r.status, csp: headers['content-security-policy'], referrer: headers['referrer-policy'] });
        if (r.status >= 400) audit.failedRequests.push(`HTTP ${r.status} ${r.url}`);
        break;
      }
      case 'Network.loadingFailed':
        if (!p.canceled) audit.failedRequests.push(`LOAD_FAILED ${p.errorText} (${p.type || 'unknown'})`);
        break;
      default:
        break;
    }
  });
  return audit;
}

// ---- main -----------------------------------------------------------------------------------

async function main() {
  const srv = await startHarnessServer();
  let browser = null;
  const finish = async (code) => {
    if (browser) { browser.cdp.close(); await browser.stop(); browser = null; }
    await srv.close();
    process.exit(code);
  };
  process.on('SIGINT', () => finish(130));
  process.on('SIGTERM', () => finish(143));

  try {
    browser = await launchChromium();
    const { cdp } = browser;
    const audit = createAudit(cdp);
    await cdp.send('Target.setAutoAttach', { autoAttach: true, waitForDebuggerOnStart: false, flatten: true });
    for (const domain of ['Page', 'Runtime', 'Network', 'Log']) await cdp.send(`${domain}.enable`);
    const setViewport = (width, height) => cdp.send('Emulation.setDeviceMetricsOverride', { width, height, deviceScaleFactor: 1, mobile: false });
    await setViewport(W, H);

    const evaluate = async (expression) => {
      const res = await cdp.send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
      if (res.exceptionDetails) throw new Error('page exception: ' + (res.exceptionDetails.exception?.description || res.exceptionDetails.text));
      return res.result.value;
    };
    const waitFor = async (expr, label, timeoutMs = 20000) => {
      const end = Date.now() + timeoutMs;
      while (Date.now() < end) {
        if (await evaluate(`Boolean(${expr})`).catch(() => false)) return true;
        await sleep(80);
      }
      throw new Error('timed out waiting for ' + label);
    };
    const frames = () => evaluate('new Promise((r) => requestAnimationFrame(() => requestAnimationFrame(() => r(true))))');
    const idle = async () => { await evaluate('window.__CCF_HARNESS__.actions().whenIdle()'); await frames(); };
    const screenshot = async (name) => {
      await frames();
      const { data } = await cdp.send('Page.captureScreenshot', { format: 'png' });
      const buf = Buffer.from(data, 'base64');
      if (name) writeFileSync(join(evidenceDir, name), buf);
      return decodePngRgba(buf);
    };
    const near = (rgb, want, tol) => rgb.every((v, i) => Math.abs(v - want[i]) <= tol);
    const fmt = (rgb) => '(' + rgb.map((v) => v.toFixed(1)).join(',') + ')';
    const vendorRequests = (fromIndex = 0) => srv.requestLog.slice(fromIndex).filter((r) => r.pathname.includes('/maplibre/vendor/'));
    const freshPage = async () => {
      const mark = { log: srv.requestLog.length, workers: audit.workers.length };
      await cdp.send('Page.navigate', { url: `${srv.baseUrl}/harness.html` });
      await waitFor('window.__CCF_HARNESS__ && window.__CCF_HARNESS__.ready', 'harness ready');
      await frames();
      return mark;
    };
    const statusExpr = `(() => {
      const root = document.querySelector('.ccf-ml-root');
      const notice = document.querySelector('.ccf-ml-notice');
      const attr = document.querySelector('.ccf-ml-attribution');
      const h = window.__CCF_HARNESS__;
      const d = h.debug();
      return {
        style: root.getAttribute('data-map-style'),
        fallback: root.getAttribute('data-fallback'),
        notice: notice.textContent,
        noticeVisible: !notice.hidden,
        attrVisible: !attr.hidden,
        attrText: attr.textContent,
        hasMap: d.hasMap,
        mapLoaded: d.mapLoaded,
        selectionKept: Boolean(document.querySelector('[data-layer-index="8"] .ccf-selection[data-selection-id="MA-C-T02"]')),
        modals: h.modalCalls().length + h.openDialogs(),
        crowns: document.querySelectorAll('[data-layer-index="6"] .ccf-crown').length,
      };
    })()`;
    const status = () => evaluate(statusExpr);
    const SELECT = `window.__CCF_HARNESS__.adapter().setSelection({ kind: 'tree', areaId: 'MA-C', id: 'MA-C-T02' })`;
    const COPY = {
      noLocation: 'No map location set. Showing Clean Woodland.',
      imagery: 'Imagery unavailable. Showing woodland map.',
      display: 'Map display unavailable. Showing Clean Woodland.',
    };
    const marginPixel = async () => evaluate('window.__CCF_HARNESS__.actions().project(-12, 40)');
    const sampleAt = (img, p) => meanRgbInBox(img, { x: p.x - 2, y: p.y - 2, w: 5, h: 5 });

    // WebGL2 must work before the matrix means anything.
    const webgl = await evaluate(`(() => {
      const gl = document.createElement('canvas').getContext('webgl2');
      if (!gl) return { ok: false };
      const dbg = gl.getExtension('WEBGL_debug_renderer_info');
      return { ok: true, version: gl.getParameter(gl.VERSION), renderer: dbg ? gl.getParameter(dbg.UNMASKED_RENDERER_WEBGL) : gl.getParameter(gl.RENDERER) };
    })()`);
    if (!webgl.ok) {
      console.log('NOT TESTED: WebGL2 is unavailable in this headless Chromium; B01-B20 not run.');
      await finish(2);
      return;
    }
    console.log('WEBGL2_READY ' + JSON.stringify(webgl));
    const browserVersion = (await cdp.send('Browser.getVersion')).product;

    // ===== Fresh session ========================================================================
    const session1 = await freshPage();

    // B01 five-method contract
    const b01 = await evaluate(`(() => {
      const h = window.__CCF_HARNESS__;
      const a = h.adapter();
      const keys = Object.keys(a).sort();
      return {
        asserted: h.assertMapRenderer(a) === a,
        keys,
        names: Object.getOwnPropertyNames(a).sort(),
        expected: [...h.MAP_RENDERER_METHODS].sort(),
        frozen: Object.isFrozen(a),
        plainAdapterKeys: Object.getOwnPropertyNames(h.newAdapter()).concat(Object.getOwnPropertySymbols(h.newAdapter()).map(String)).sort(),
      };
    })()`);
    record('B01', b01.asserted && JSON.stringify(b01.keys) === JSON.stringify(b01.expected) && JSON.stringify(b01.names) === JSON.stringify(b01.expected)
      && JSON.stringify(b01.plainAdapterKeys) === JSON.stringify(b01.expected) && b01.frozen,
      `assertMapRenderer passes; own properties=[${b01.names}]; production adapter (no debug) properties=[${b01.plainAdapterKeys}]; frozen=${b01.frozen}`);

    // B02 frame immutability
    const b02 = await evaluate(`(() => {
      const h = window.__CCF_HARNESS__;
      const unmutated = JSON.stringify(h.firstFrame()) === h.firstFrameJson();
      const f = h.debug().frame;
      const deepFrozen = (o) => !o || typeof o !== 'object' || (Object.isFrozen(o) && Object.values(o).every(deepFrozen));
      const distinct = f !== h.firstFrame() && f.areas[0] !== h.firstFrame().areas[0];
      const frozenInput = h.createFixtureFrame();
      const freeze = (o) => { if (o && typeof o === 'object') { Object.values(o).forEach(freeze); Object.freeze(o); } return o; };
      freeze(frozenInput);
      let frozenInputOk = true;
      try { h.adapter().render(frozenInput); } catch { frozenInputOk = false; }
      const radiusCopied = h.debug().frame.areas[2].trees[2].radiusM === 3.8;
      return { unmutated, deepFrozen: deepFrozen(f), distinct, frozenInputOk, radiusCopied };
    })()`);
    record('B02', b02.unmutated && b02.deepFrozen && b02.distinct && b02.frozenInputOk && b02.radiusCopied,
      `callerFrameUnmutated=${b02.unmutated}, normalisedFrameDeepFrozen=${b02.deepFrozen}, copiedNotAliased=${b02.distinct}, frozenInputAccepted=${b02.frozenInputOk}, radiusCopiedVerbatim=${b02.radiusCopied}`);

    // B03 fresh session = Clean Woodland, zero MapLibre
    const s03 = await status();
    const img03 = await screenshot('b03-clean-woodland-default.png');
    const bg03 = sampleAt(img03, { x: 40, y: 360 });
    const layer1Bg = await evaluate(`getComputedStyle(document.querySelector('[data-layer-index="1"]')).backgroundColor`);
    const vendor03 = vendorRequests(session1.log).length;
    const workers03 = audit.workers.length - session1.workers;
    record('B03', s03.style === 'clean-woodland' && s03.fallback === 'none' && !s03.noticeVisible && !s03.hasMap && near(bg03, BG, 1)
      && layer1Bg === 'rgb(21, 37, 26)' && vendor03 === 0 && workers03 === 0 && s03.crowns === 20,
      `style=${s03.style}, notice hidden=${!s03.noticeVisible}, canvas pixel=${fmt(bg03)} (want #15251a), layer1=${layer1Bg}, vendorRequests=${vendor03}, workers=${workers03}, crowns=${s03.crowns}`);

    // B04 no validated location -> Clean Woodland, still zero MapLibre
    await evaluate(SELECT);
    const badAnchors = {
      'missing anchor': 'geoAnchor: null',
      'centroid origin': "geoAnchor: { ...base, originKind: 'centroid' }",
      'digest mismatch': "geoAnchor: { ...base, geometryDigest: 'sha256:other' }",
      'frame without source identity': 'sourceId: null',
      'latitude out of range': 'geoAnchor: { ...base, latDeg: 91 }',
      'non-finite bearing': 'geoAnchor: { ...base, bearingDeg: NaN }',
    };
    const b04 = [];
    for (const [label, override] of Object.entries(badAnchors)) {
      await evaluate(`(() => { const h = window.__CCF_HARNESS__; const base = h.createFixtureFrame().geoAnchor; h.render({ mapStyle: 'satellite', ${override} }); })()`);
      await frames();
      const s = await status();
      b04.push({ label, ok: s.style === 'clean-woodland' && s.fallback === 'no-location' && s.notice === COPY.noLocation && s.noticeVisible
        && !s.attrVisible && s.selectionKept && s.modals === 0 && !s.hasMap });
    }
    const vendor04 = vendorRequests(session1.log).length;
    record('B04', b04.every((c) => c.ok) && vendor04 === 0 && audit.workers.length === session1.workers,
      `${b04.filter((c) => c.ok).length}/${b04.length} invalid anchors -> "${COPY.noLocation}" with selection kept, 0 modals [${b04.map((c) => c.label + ':' + (c.ok ? 'ok' : 'FAIL')).join('; ')}]; vendorRequests=${vendor04}`);

    // B05 idempotent lifecycle
    const b05 = await evaluate(`(() => {
      const h = window.__CCF_HARNESS__;
      const host = document.getElementById('map-host');
      const raw = h.newAdapter();
      let preMountSafe = true;
      try { raw.render(h.createFixtureFrame()); raw.setSelection(null); raw.focusArea('MA-A'); raw.destroy(); } catch { preMountSafe = false; }
      const preMountTouchedDom = host.querySelectorAll('.ccf-ml-root').length;
      const a = h.adapter();
      a.mount(host, { onPointer: () => {} });
      const afterDoubleMount = host.querySelectorAll('.ccf-ml-root').length;
      a.destroy();
      const afterDestroy = host.querySelectorAll('.ccf-ml-root').length;
      let postDestroySafe = true;
      try { a.destroy(); a.render(h.createFixtureFrame()); a.setSelection(null); a.focusArea('MA-A'); } catch { postDestroySafe = false; }
      const afterPostDestroyCalls = host.querySelectorAll('.ccf-ml-root').length;
      h.create();
      h.render();
      return { preMountSafe, preMountTouchedDom, afterDoubleMount, afterDestroy, postDestroySafe, afterPostDestroyCalls, remounted: host.querySelectorAll('.ccf-ml-root').length };
    })()`);
    record('B05', b05.preMountSafe && b05.preMountTouchedDom === 1 && b05.afterDoubleMount === 1 && b05.afterDestroy === 0 && b05.postDestroySafe
      && b05.afterPostDestroyCalls === 0 && b05.remounted === 1,
      `callsBeforeMount safe=${b05.preMountSafe}; roots after double mount=${b05.afterDoubleMount}, after destroy=${b05.afterDestroy}; second destroy + calls after destroy safe=${b05.postDestroySafe} (roots=${b05.afterPostDestroyCalls}); remount roots=${b05.remounted}`);

    // B06 generation guard
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.create('delayed-loader'); h.render({ mapStyle: 'satellite' }); h.render({ mapStyle: 'clean-woodland' }); window.__CCF_RELEASE_LOADER__(); })()`);
    await sleep(1500);
    const b06a = await evaluate(`({ s: document.querySelector('.ccf-ml-root').getAttribute('data-map-style'), hasMap: window.__CCF_HARNESS__.debug().hasMap,
      constructed: window.__CCF_HARNESS__.diagnostics().filter((d) => d.kind === 'map-constructed').length })`);
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.create(); h.render({ mapStyle: 'satellite', frameKey: 'key-A' }); h.render({ mapStyle: 'satellite', frameKey: 'key-B' }); })()`);
    await waitFor('window.__CCF_HARNESS__.debug().mapReady', 'satellite map load (B06)');
    await idle();
    const b06b = await evaluate(`({ key: document.querySelector('.ccf-ml-root').getAttribute('data-frame-key'), frameKey: window.__CCF_HARNESS__.debug().frame.frameKey,
      constructed: window.__CCF_HARNESS__.diagnostics().filter((d) => d.kind === 'map-constructed').length })`);
    record('B06', b06a.s === 'clean-woodland' && !b06a.hasMap && b06a.constructed === 0 && b06b.key === 'key-B' && b06b.frameKey === 'key-B' && b06b.constructed === 1,
      `stale delayed module load ignored (style=${b06a.s}, map=${b06a.hasMap}, constructed=${b06a.constructed}); back-to-back satellite frames -> shown frameKey=${b06b.key}, maps constructed=${b06b.constructed}`);

    // B07 lazy module + same-origin module worker under strict CSP
    const expectedWorker = srv.baseUrl + WORKER_PATH;
    const ok200 = (suffix) => srv.requestLog.some((r) => r.pathname.endsWith(suffix) && r.status === 200);
    const workerUrls = audit.workers.map((w) => w.url);
    const b07 = await evaluate(`(() => { const h = window.__CCF_HARNESS__; const k = h.diagnostics().map((d) => d.kind);
      return { workerUrl: h.debug().workerUrl, setBeforeMap: k.indexOf('worker-url-set') >= 0 && k.indexOf('worker-url-set') < k.indexOf('map-constructed'), pageCsp: h.cspViolations().length }; })()`);
    record('B07', ok200('/maplibre-gl.mjs') && ok200('/maplibre-gl-worker.mjs') && ok200('/maplibre-gl.css') && workerUrls.length > 0
      && workerUrls.every((u) => u === expectedWorker) && b07.workerUrl === expectedWorker && b07.setBeforeMap && b07.pageCsp === 0
      && audit.cspViolations.length === 0 && audit.consoleErrors.length === 0,
      `module/worker/css 200 after switch; worker targets=${workerUrls.length} all at ${WORKER_PATH} (no blob/data); getWorkerUrl matches; setWorkerUrl before new Map=${b07.setBeforeMap}; CSP violations=${audit.cspViolations.length + b07.pageCsp}; console errors=${audit.consoleErrors.length}`);

    // B08 exact headers on every response, loopback only
    const direct404 = await fetch(srv.baseUrl + '/no-such-file');
    const direct404Ok = direct404.status === 404 && direct404.headers.get('content-security-policy') === STRICT_CSP_HEADER && direct404.headers.get('referrer-policy') === REFERRER_POLICY;
    await direct404.arrayBuffer();
    const badHeaders = audit.responses.filter((r) => r.csp !== STRICT_CSP_HEADER || r.referrer !== REFERRER_POLICY);
    record('B08', audit.responses.length > 0 && badHeaders.length === 0 && direct404Ok && audit.nonLoopbackRequests.length === 0 && audit.failedRequests.length === 0,
      `${audit.responses.length} browser responses all carry the exact CSP + Referrer-Policy (mismatches=${badHeaders.length}); server 404 also exact=${direct404Ok}; http requests=${audit.httpRequests}, non-loopback=${audit.nonLoopbackRequests.length}, failed=${audit.failedRequests.length}`);

    // B09 inline-only MapLibre style
    await waitFor('window.__CCF_HARNESS__.debug().imageryLoaded', 'imagery loaded (B09)');
    const b09 = await evaluate(`(() => {
      const st = window.__CCF_HARNESS__.debug().style;
      const sources = Object.entries(st.sources || {});
      return {
        glyphs: st.glyphs ?? null, sprite: st.sprite ?? null,
        sources: sources.map(([id, s]) => ({ id, type: s.type, url: s.url, tiles: s.tiles, inline: typeof s.data === 'object' })),
        layers: st.layers.map((l) => l.id + ':' + l.type),
        bg: st.layers[0].paint['background-color'],
        extentFill: st.layers[1].paint['fill-color'],
      };
    })()`);
    const imageSrc = b09.sources.find((x) => x.id === 'ccf-local-imagery');
    const extentSrc = b09.sources.find((x) => x.id === 'ccf-woodland-extent');
    const srcOk = b09.sources.length === 2 && b09.sources.every((x) => !x.tiles)
      && imageSrc && imageSrc.type === 'image' && imageSrc.url.startsWith(srv.baseUrl + '/')
      && extentSrc && extentSrc.type === 'geojson' && extentSrc.url === undefined && extentSrc.inline;
    record('B09', b09.glyphs === null && b09.sprite === null && srcOk && JSON.stringify(b09.layers) === JSON.stringify(['ccf-woodland-canvas:background', 'ccf-woodland-extent:fill', 'ccf-georeferenced-imagery:raster'])
      && String(b09.extentFill).toLowerCase() === '#15251a' && String(b09.bg).toLowerCase() === '#15251a',
      `glyphs=${b09.glyphs}, sprite=${b09.sprite}, sources=${JSON.stringify(b09.sources.map((s) => s.type + ' ' + (s.url ? s.url.replace(srv.baseUrl, '') : 'inline data')))}, layers=[${b09.layers}], background=${b09.bg}, extent fill=${b09.extentFill} (Layer 1, worker-tiled)`);

    // B10 layer order
    const b10 = await evaluate(`(() => {
      const h = window.__CCF_HARNESS__;
      const nodes = Array.from(document.querySelectorAll('.ccf-ml-root [data-layer-index]'));
      const ids = nodes.map((n) => n.getAttribute('data-layer-id'));
      const idx = nodes.map((n) => Number(n.getAttribute('data-layer-index')));
      const slots = Array.from(document.querySelectorAll('[data-layer-index="3"] [data-reserved-slot]'));
      const world = document.querySelector('.ccf-ml-world');
      const z = (sel) => Number(getComputedStyle(document.querySelector(sel)).zIndex);
      return {
        ids, order: h.verifyLayerStackOrder(ids).ok && JSON.stringify(idx) === '[1,2,3,4,5,6,7,8]',
        descriptors: h.LAYER_STACK.map((l) => l.index + ':' + l.id).join(','),
        slots: slots.map((s) => s.getAttribute('data-reserved-slot') + '=' + s.getAttribute('data-implemented') + '/' + s.childNodes.length),
        topIsMarks: world.lastElementChild.getAttribute('data-layer-index') === '8',
        overlayAbove: z('.ccf-ml-overlay') > z('.ccf-ml-canvas'),
      };
    })()`);
    const slotsOk = JSON.stringify(b10.slots) === JSON.stringify(['drone-lidar=false/0', 'hillshade-contours=false/0', 'streams-rivers=false/0', 'roads-bridges-paths=false/0']);
    record('B10', b10.order && slotsOk && b10.topIsMarks && b10.overlayAbove && JSON.stringify(b09.layers.map((l) => l.split(':')[1])) === '["background","fill","raster"]',
      `DOM bottom->top [${b10.ids.join(' > ')}] matches descriptors; reserved slots ${b10.slots.join(', ')}; MapLibre canvas order background<extent fill (Layer 1)<raster (Layer 2); overlay above canvas=${b10.overlayAbove}; marks topmost=${b10.topIsMarks}`);

    // B11 crowns are true radiusM (satellite, 1280x720)
    const m11 = await evaluate('window.__CCF_HARNESS__.measureSatelliteCrowns()');
    record('B11', m11.count === 20 && m11.rAttrMatches && m11.maxErrM <= 0.002 && m11.maxCentrePx < 0.5,
      `${m11.count} crowns, r attribute == radiusM metres: ${m11.rAttrMatches}; rendered rim -> MapLibre unproject -> local metres: max radius error ${(m11.maxErrM * 1000).toFixed(4)} mm (<= 2 mm); centre vs map.project max ${m11.maxCentrePx.toFixed(4)} px; MA-C-T03 (3.8 m) = ${m11.radiusPx['MA-C-T03'].toFixed(2)} px`);

    // B12 re-measure after a viewport resize, both styles
    await setViewport(800, 500);
    await waitFor(`document.querySelector('.ccf-ml-stage').clientWidth === 800`, 'stage resized');
    await evaluate(`window.__CCF_HARNESS__.adapter().focusArea(null)`);
    await idle();
    const m12 = await evaluate('window.__CCF_HARNESS__.measureSatelliteCrowns()');
    const ratio12 = m12.radiusPx['MA-C-T03'] / m11.radiusPx['MA-C-T03'];
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'clean-woodland' })`);
    await frames();
    const cwSmall = await evaluate('window.__CCF_HARNESS__.measureCleanWoodlandCrowns()');
    await setViewport(W, H);
    await waitFor(`document.querySelector('.ccf-ml-stage').clientWidth === ${W}`, 'stage restored');
    await frames();
    const cwLarge = await evaluate('window.__CCF_HARNESS__.measureCleanWoodlandCrowns()');
    const cwRatio = cwSmall.radiusPx['MA-C-T03'] / cwLarge.radiusPx['MA-C-T03'];
    record('B12', m12.count === 20 && m12.maxErrM <= 0.002 && m12.maxCentrePx < 0.5 && Math.abs(ratio12 - 1) > 0.2
      && cwSmall.isotropic && cwLarge.isotropic && cwSmall.rAttrMatches && cwSmall.maxErrM <= 0.002 && cwLarge.maxErrM <= 0.002 && Math.abs(cwRatio - 1) > 0.2,
      `satellite 800x500: max error ${(m12.maxErrM * 1000).toFixed(4)} mm, MA-C-T03 ${m11.radiusPx['MA-C-T03'].toFixed(2)} -> ${m12.radiusPx['MA-C-T03'].toFixed(2)} px (x${ratio12.toFixed(3)}); clean woodland uniform scale, max error ${(Math.max(cwSmall.maxErrM, cwLarge.maxErrM) * 1000).toFixed(4)} mm, ${cwLarge.radiusPx['MA-C-T03'].toFixed(2)} px @1280x720 -> ${cwSmall.radiusPx['MA-C-T03'].toFixed(2)} px @800x500 (x${cwRatio.toFixed(3)})`);

    // B13 overlay stays aligned through zoom + pan
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'satellite' })`);
    await waitFor('window.__CCF_HARNESS__.debug().mapReady', 'satellite reload (B13)');
    await evaluate(`(() => { const a = window.__CCF_HARNESS__.actions(); a.zoomBy(0.8); a.panBy(120, -60); })()`);
    await idle();
    const m13 = await evaluate('window.__CCF_HARNESS__.measureSatelliteCrowns()');
    record('B13', m13.count === 20 && m13.maxCentrePx < 0.5 && m13.maxErrM <= 0.002,
      `after zoom +0.8 and pan (120,-60) px: centre error max ${m13.maxCentrePx.toFixed(4)} px (< 0.5), radius error max ${(m13.maxErrM * 1000).toFixed(4)} mm`);

    // B14 imagery at the default 65%, with attribution
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.render({ mapStyle: 'satellite', imageryOpacity: undefined }); h.adapter().focusArea(null); })()`);
    await waitFor('window.__CCF_HARNESS__.debug().imageryLoaded', 'imagery loaded (B14)');
    await idle();
    const s14 = await status();
    const st14 = await evaluate(`({ paint: window.__CCF_HARNESS__.debug().rasterOpacity, frameOpacity: window.__CCF_HARNESS__.debug().frame.imageryOpacity,
      visible: document.querySelector('[data-layer-index="2"]').getAttribute('data-imagery-visible') })`);
    const px = await marginPixel();
    const img65 = sampleAt(await screenshot('b14-imagery-65.png'), px);
    const want65 = IMG.map((v, i) => 0.65 * v + 0.35 * BG[i]);
    record('B14', st14.paint === 0.65 && st14.frameOpacity === 0.65 && st14.visible === 'true' && s14.attrVisible
      && s14.attrText === 'Synthetic local test imagery (not a real photograph)' && near(img65, want65, 4),
      `default raster-opacity=${st14.paint}; imagery pixel ${fmt(img65)} vs 65% blend ${fmt(want65)} (+-4); attribution visible "${s14.attrText}"`);

    // B15 imagery at 0% and 100%; only Layer 2 changes
    const overlayOpacity = `[1,4,5,6,7,8].map((i) => getComputedStyle(document.querySelector('[data-layer-index="' + i + '"]')).opacity).concat(getComputedStyle(document.querySelector('.ccf-ml-overlay')).opacity)`;
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'satellite', imageryOpacity: 0 })`);
    await idle();
    const s0 = await status();
    const p0 = await evaluate(`window.__CCF_HARNESS__.debug().rasterOpacity`);
    const o0 = await evaluate(overlayOpacity);
    const img0 = sampleAt(await screenshot('b15-imagery-0.png'), px);
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'satellite', imageryOpacity: 1 })`);
    await idle();
    const s100 = await status();
    const p100 = await evaluate(`window.__CCF_HARNESS__.debug().rasterOpacity`);
    const o100 = await evaluate(overlayOpacity);
    const img100 = sampleAt(await screenshot('b15-imagery-100.png'), px);
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'satellite', imageryOpacity: 0.65 })`);
    await idle();
    const overlaysFull = [...o0, ...o100].every((o) => o === '1');
    record('B15', p0 === 0 && p100 === 1 && near(img0, BG, 3) && near(img100, IMG, 3) && !s0.attrVisible && s100.attrVisible && overlaysFull && s0.crowns === 20 && s100.crowns === 20,
      `0%: paint=${p0}, pixel ${fmt(img0)} (= canvas #15251a), attribution hidden=${!s0.attrVisible}; 100%: paint=${p100}, pixel ${fmt(img100)} (= imagery), attribution visible=${s100.attrVisible}; layers 1,4-8 + overlay opacity stay 1=${overlaysFull}`);

    // B16 valid location, imagery unavailable -> dark MapLibre canvas
    await evaluate(SELECT);
    const b16 = [];
    for (const [label, override] of [['no imagery configured', 'imagery: null'],
      ['imagery URL returns a non-image', "imagery: { url: '/fixtures/not-an-image.png', attribution: 'Synthetic local test imagery (not a real photograph)', boundsLocal: { minXM: -24, minZM: -24, maxXM: 104, maxZM: 104 } }"]]) {
      await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'satellite', ${override} })`);
      await waitFor(`document.querySelector('.ccf-ml-root').getAttribute('data-fallback') === 'imagery-unavailable'`, 'imagery fallback ' + label);
      await idle();
      const s = await status();
      const layers = await evaluate(`window.__CCF_HARNESS__.debug().style.layers.map((l) => l.id)`);
      const pix = sampleAt(await screenshot(`b16-imagery-unavailable-${b16.length + 1}.png`), await marginPixel());
      b16.push({ label, ok: s.style === 'satellite' && s.hasMap && s.notice === COPY.imagery && s.noticeVisible && !s.attrVisible && s.selectionKept
        && s.modals === 0 && s.crowns === 20 && near(pix, BG, 3) && JSON.stringify(layers) === '["ccf-woodland-canvas","ccf-woodland-extent"]', pix });
    }
    record('B16', b16.every((c) => c.ok),
      `${b16.filter((c) => c.ok).length}/2 -> "${COPY.imagery}" on the dark MapLibre canvas (pixels ${b16.map((c) => fmt(c.pix)).join(', ')}), no raster layer, attribution hidden, selection kept, 0 modals [${b16.map((c) => c.label + ':' + (c.ok ? 'ok' : 'FAIL')).join('; ')}]`);

    // B17 WebGL / module / worker / context failure -> Clean Woodland
    const b17 = [];
    const checkDisplay = async (label, extra = true) => {
      const s = await status();
      b17.push({ label, ok: extra && s.style === 'clean-woodland' && s.fallback === 'display-unavailable' && s.notice === COPY.display && s.noticeVisible
        && !s.attrVisible && s.selectionKept && s.modals === 0 && !s.hasMap });
    };
    let mark = await freshPage();
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.create('no-webgl'); h.render(); ${SELECT}; h.render({ mapStyle: 'satellite' }); })()`);
    await frames();
    await checkDisplay('WebGL unavailable (no vendor request)', vendorRequests(mark.log).length === 0);
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.create('module-fail'); h.render(); ${SELECT}; h.render({ mapStyle: 'satellite' }); })()`);
    await waitFor(`document.querySelector('.ccf-ml-root').getAttribute('data-fallback') === 'display-unavailable'`, 'module failure fallback');
    await checkDisplay('module import failure');
    mark = await freshPage();
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.create('worker-silent'); h.render(); ${SELECT}; h.render({ mapStyle: 'satellite' }); })()`);
    await waitFor(`document.querySelector('.ccf-ml-root').getAttribute('data-fallback') === 'display-unavailable'`, 'worker failure fallback', 10000);
    const silentWorkerSeen = audit.workers.slice(mark.workers).some((w) => w.url.endsWith('/harness/fixtures/silent-worker.mjs'));
    await checkDisplay('worker never responds (load watchdog)', silentWorkerSeen);
    await freshPage();
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.create(); h.render({ mapStyle: 'satellite' }); })()`);
    await waitFor('window.__CCF_HARNESS__.debug().mapReady', 'satellite load before context loss');
    await evaluate(SELECT);
    const lost = await evaluate('window.__CCF_HARNESS__.actions().loseContext()');
    await waitFor(`document.querySelector('.ccf-ml-root').getAttribute('data-fallback') === 'display-unavailable'`, 'context loss fallback');
    await checkDisplay('WebGL context lost on a live map', lost);
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'satellite' })`);
    await frames();
    await checkDisplay('later satellite frame stays on Clean Woodland (no retry loop)');
    record('B17', b17.every((c) => c.ok),
      `${b17.filter((c) => c.ok).length}/${b17.length} -> "${COPY.display}", selection kept, 0 modals [${b17.map((c) => c.label + ':' + (c.ok ? 'ok' : 'FAIL')).join('; ')}]`);

    // B18 pointer payload
    const centreOf = (selector) => evaluate(`(() => { const r = document.querySelector('${selector}').getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2 }; })()`);
    const click = async (p) => {
      const base = { x: p.x, y: p.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
      await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', x: p.x, y: p.y, button: 'none', pointerType: 'mouse' });
      await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
      await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
      await sleep(120);
    };
    const targets = ['circle.ccf-crown[data-tree-id="MA-C-T02"]', '[data-juvenile-id="MA-C-PJ01"] .ccf-juvenile-core', 'circle.ccf-crown[data-tree-id="MA-E-T01"]'];
    const clickTargets = async () => {
      await evaluate('window.__CCF_HARNESS__.clearPointerEvents()');
      for (const t of targets) await click(await centreOf(t));
      return evaluate('window.__CCF_HARNESS__.pointerEvents()');
    };
    const payloadOk = (events) => Array.isArray(events) && events.length === 3
      && events.every((e) => Object.keys(e).join(',') === 'areaId,u,v,juvenileId,frameKey' && e.frameKey === 'fixture-80x80-5lane-y0')
      && events[0].areaId === 'MA-C' && events[0].juvenileId === null && Math.hypot(events[0].u - 8.2, events[0].v - 36) < 0.25
      && events[1].areaId === 'MA-C' && events[1].juvenileId === 'MA-C-PJ01' && events[1].u === 5.5 && events[1].v === 44
      && events[2].areaId === 'MA-E' && events[2].juvenileId === null && Math.hypot(events[2].u - 4.5, events[2].v - 18) < 0.25;
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.create(); h.render({ mapStyle: 'satellite' }); })()`);
    await waitFor('window.__CCF_HARNESS__.debug().mapReady', 'satellite load (B18)');
    await idle();
    const satEvents = await clickTargets();
    await evaluate('window.__CCF_HARNESS__.clearPointerEvents()');
    const before = await evaluate('window.__CCF_HARNESS__.actions().unproject(640, 360)');
    await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', x: 700, y: 400, button: 'left', clickCount: 1 });
    for (let i = 1; i <= 8; i++) await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', x: 700 + i * 8, y: 400, button: 'left', buttons: 1 });
    await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', x: 764, y: 400, button: 'left', clickCount: 1 });
    await idle();
    const after = await evaluate('window.__CCF_HARNESS__.actions().unproject(640, 360)');
    const dragEvents = (await evaluate('window.__CCF_HARNESS__.pointerEvents()')).length;
    const panned = Math.hypot(after.xM - before.xM, after.zM - before.zM) > 1;
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'clean-woodland' })`);
    await frames();
    const cwEvents = await clickTargets();
    record('B18', payloadOk(satEvents) && payloadOk(cwEvents) && dragEvents === 0 && panned,
      `satellite ${JSON.stringify(satEvents.map((e) => [e.areaId, +e.u.toFixed(3), +e.v.toFixed(3), e.juvenileId]))}; clean woodland ${JSON.stringify(cwEvents.map((e) => [e.areaId, +e.u.toFixed(3), +e.v.toFixed(3), e.juvenileId]))}; keys exactly {areaId,u,v,juvenileId,frameKey}; 64 px drag panned the map=${panned} and emitted ${dragEvents} callbacks`);

    // B19 layer content, accessible marks, focusArea without data change
    await evaluate(`(() => { const h = window.__CCF_HARNESS__; h.render({ mapStyle: 'satellite', overlay: 'soil-moisture' }); ${SELECT}; })()`);
    await idle();
    const b19 = await evaluate(`(() => {
      const q = (s) => document.querySelectorAll(s);
      const L = (i, s) => q('[data-layer-index="' + i + '"] ' + s).length;
      const cell = document.querySelector('[data-layer-index="4"] .ccf-cell-moisture[data-area-id="MA-C"][data-cell="0:0"]');
      const pts = cell.getAttribute('points').split(' ').map((p) => p.split(',').map(Number));
      const span = (k) => Math.max(...pts.map((p) => p[k])) - Math.min(...pts.map((p) => p[k]));
      const l8 = document.querySelector('[data-layer-index="8"]');
      const sel = l8.lastElementChild;
      const dash = (s) => getComputedStyle(document.querySelector(s)).strokeDasharray;
      return {
        lanes: L(4, '.ccf-lane'), dry: L(4, '[data-moisture-class="DryFreeDraining"]'), moist: L(4, '[data-moisture-class="FreshMoistMineral"]'),
        cellSize: [span(0), span(1)], deadwood: L(5, '.ccf-deadwood'), crowns: L(6, '.ccf-crown'),
        statuses: [...new Set(Array.from(q('[data-layer-index="6"] .ccf-tree')).map((n) => n.getAttribute('data-status')))].sort(),
        juveniles: L(7, '.ccf-juvenile'), shelters: L(7, '.ccf-juvenile-shelter'),
        crop: L(8, '.ccf-mark-crop-ring') === 1 && L(8, '[data-mark-badge="C"] text') === 1,
        pendingDashed: dash('.ccf-mark-pending-ring') !== 'none', approvedSolid: dash('.ccf-mark-approved-ring') === 'none',
        ranks: Array.from(l8.querySelectorAll('.ccf-rank-badge text')).map((t) => t.textContent),
        selTop: sel.classList.contains('ccf-selection') && sel.getAttribute('data-selection-id') === 'MA-C-T02',
        selShape: Boolean(sel.querySelector('.ccf-selection-frame')) && sel.querySelector('.ccf-selection-tag-text').textContent === 'SEL' && sel.getAttribute('aria-label') === 'Selected tree MA-C-T02',
        rankAria: l8.querySelector('.ccf-rank-badge').getAttribute('aria-label'),
      };
    })()`);
    const frameBefore = await evaluate('JSON.stringify(window.__CCF_HARNESS__.debug().frame)');
    await evaluate(`window.__CCF_HARNESS__.adapter().focusArea('MA-A')`);
    await idle();
    const centreA = await evaluate('window.__CCF_HARNESS__.actions().unproject(640, 360)');
    await evaluate(`window.__CCF_HARNESS__.adapter().focusArea('MA-E')`);
    await idle();
    const centreE = await evaluate('window.__CCF_HARNESS__.actions().unproject(640, 360)');
    const focusSel = (await status()).selectionKept;
    const frameAfter = await evaluate('JSON.stringify(window.__CCF_HARNESS__.debug().frame)');
    await evaluate(`window.__CCF_HARNESS__.render({ mapStyle: 'clean-woodland', overlay: 'soil-moisture' })`);
    await evaluate(`window.__CCF_HARNESS__.adapter().focusArea('MA-A')`);
    const cwA = await evaluate('window.__CCF_HARNESS__.debug().matrix.e');
    await evaluate(`window.__CCF_HARNESS__.adapter().focusArea('MA-E')`);
    const cwE = await evaluate('window.__CCF_HARNESS__.debug().matrix.e');
    await evaluate(`window.__CCF_HARNESS__.adapter().focusArea(null)`);
    const b19ok = b19.lanes === 5 && b19.dry === 576 && b19.moist === 544 && Math.abs(b19.cellSize[0] - 2.5) < 1e-9 && Math.abs(b19.cellSize[1] - 2.5) < 1e-9
      && b19.deadwood === 5 && b19.crowns === 20 && JSON.stringify(b19.statuses) === '["ApprovedFell","Felled","Living","NaturalDead","PendingFell"]'
      && b19.juveniles === 5 && b19.shelters === 1 && b19.crop && b19.pendingDashed && b19.approvedSolid && JSON.stringify(b19.ranks) === '["1","2"]'
      && b19.selTop && b19.selShape && Math.abs(centreA.xM - 8) < 0.5 && Math.abs(centreE.xM - 72) < 0.5 && focusSel && frameBefore === frameAfter && cwE < cwA;
    record('B19', b19ok,
      `L4 lanes=${b19.lanes}, moisture cells dry=${b19.dry}/moist=${b19.moist}, cell ${b19.cellSize.map((v) => v.toFixed(2)).join(' x ')} m; L5 deadwood=${b19.deadwood}; L6 crowns=${b19.crowns} [${b19.statuses}]; L7 juveniles=${b19.juveniles} (shelter ${b19.shelters}); L8 crop ring+C=${b19.crop}, pending dashed=${b19.pendingDashed}/approved solid=${b19.approvedSolid}, ranks=${b19.ranks}, selection on top with frame+SEL+aria=${b19.selTop && b19.selShape}; focusArea centres x=${centreA.xM.toFixed(2)} (MA-A) -> ${centreE.xM.toFixed(2)} (MA-E), frame unchanged=${frameBefore === frameAfter}, selection kept=${focusSel}`);

    // B20 session-only style, Guardrail 7, reload, final audit
    const b20 = await evaluate(`(() => {
      const h = window.__CCF_HARNESS__;
      const text = document.body.innerText + ' ' + Array.from(document.querySelectorAll('[aria-label]')).map((n) => n.getAttribute('aria-label')).join(' ');
      return { writes: h.storageWrites().length, local: localStorage.length, session: sessionStorage.length, cookie: document.cookie,
        guardrail: !/\\[(?:C|E)\\]|calibration disclaimer|not empirically validated/i.test(text), pageCsp: h.cspViolations().length };
    })()`);
    const reloadMark = await freshPage();
    await sleep(400);
    const reloadStyle = await evaluate(`document.querySelector('.ccf-ml-root').getAttribute('data-map-style')`);
    const reloadVendor = vendorRequests(reloadMark.log).length;
    const reloadWorkers = audit.workers.length - reloadMark.workers;
    const auditClean = audit.consoleErrors.length === 0 && audit.cspViolations.length === 0 && audit.failedRequests.length === 0 && audit.nonLoopbackRequests.length === 0;
    record('B20', b20.writes === 0 && b20.local === 0 && b20.session === 0 && b20.cookie === '' && b20.guardrail && b20.pageCsp === 0
      && reloadStyle === 'clean-woodland' && reloadVendor === 0 && reloadWorkers === 0 && auditClean,
      `storage writes=${b20.writes}, local/session items=${b20.local}/${b20.session}, cookies="${b20.cookie}", no [C]/[E]/disclaimers=${b20.guardrail}; fresh load style=${reloadStyle}, vendor requests=${reloadVendor}, workers=${reloadWorkers}; whole run: console errors=${audit.consoleErrors.length}, CSP violations=${audit.cspViolations.length}, failed requests=${audit.failedRequests.length}, non-loopback=${audit.nonLoopbackRequests.length}`);

    const passed = results.filter((r) => r.status === 'PASS').length;
    const report = {
      timestampUtc: new Date().toISOString(),
      browser: browserVersion,
      webgl,
      cspHeader: STRICT_CSP_HEADER,
      referrerPolicy: REFERRER_POLICY,
      totals: { passed, failed: results.length - passed, total: results.length },
      audit: {
        consoleErrors: audit.consoleErrors,
        cspViolations: audit.cspViolations,
        failedRequests: audit.failedRequests,
        nonLoopbackRequests: audit.nonLoopbackRequests,
        httpRequests: audit.httpRequests,
        responsesChecked: audit.responses.length,
        workerTargets: audit.workers.map((w) => w.url.replace(srv.baseUrl, '')),
        serverRequests: srv.requestLog.length,
      },
      cases: results,
    };
    writeFileSync(join(evidenceDir, 'cdp-b01-b20-report.json'), JSON.stringify(report, null, 2) + '\n');
    writeFileSync(join(evidenceDir, 'server-request-log.json'), JSON.stringify(srv.requestLog, null, 1) + '\n');
    console.log(`CDP_MATRIX_SUMMARY: ${passed} passed, ${results.length - passed} failed, total=${results.length}, console_errors=${audit.consoleErrors.length}, csp_violations=${audit.cspViolations.length}, failed_requests=${audit.failedRequests.length}, non_loopback=${audit.nonLoopbackRequests.length}`);
    for (const key of ['consoleErrors', 'cspViolations', 'failedRequests', 'nonLoopbackRequests']) {
      if (audit[key].length) console.log(key.toUpperCase() + ': ' + JSON.stringify(audit[key], null, 1));
    }
    await finish(passed === results.length && results.length === 20 && auditClean ? 0 : 1);
  } catch (err) {
    console.log('HARNESS_FATAL: ' + (err && err.stack ? err.stack : err));
    writeFileSync(join(evidenceDir, 'cdp-b01-b20-report.json'), JSON.stringify({ fatal: String(err && err.stack ? err.stack : err), cases: results }, null, 2) + '\n');
    await finish(1);
  }
}

main();
