// S0c real-browser parity driver (Microsoft Edge over CDP, Node built-ins only).
// Captures 9 deterministic UI states for one (experiment, viewport) cell:
//   01-initial-load, 02-tree-selected-competition, 03-fell-draft-pending,
//   04-approved-plan, 05-after-execute, 06-after-year-plus-1,
//   07-saved-games-dialog, 08-history-drawer, 09-structure-panel.
// Records canonicalized document.body.outerHTML (exact per-process random sessionId replaced with <SESSION_ID>),
// DSF-1 PNG screenshots, console/CSP/HTTP diagnostics, and the full loopback-only network log.
'use strict';
import { spawn } from 'node:child_process';
import { createHash } from 'node:crypto';
import { mkdirSync, writeFileSync, rmSync } from 'node:fs';
import { join } from 'node:path';
import { tmpdir } from 'node:os';

const args = process.argv.slice(2);
const opt = (name, fallback = null) => {
  const i = args.indexOf('--' + name);
  return i >= 0 && i + 1 < args.length ? args[i + 1] : fallback;
};

const url = opt('url');
const out = opt('out', '.');
const cell = opt('cell', 'ma-1_1280x720');
const width = Number(opt('width', '1280'));
const height = Number(opt('height', '720'));
const EDGE = process.env.EDGE || 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';

if (!url) {
  console.error('Usage: node drive-parity.mjs --url <url> --out <dir> --cell <id> --width <w> --height <h>');
  process.exit(64);
}

const parsedStartUrl = new URL(url);
const expectedOrigin = parsedStartUrl.origin;

mkdirSync(out, { recursive: true });

let failures = 0;
let passes = 0;
const log = (line) => console.log(line);
function check(ok, title) {
  if (ok) { passes++; log('PASS ' + title); }
  else { failures++; log('FAIL ' + title); }
}

const sha256Hex = (buf) => createHash('sha256').update(buf).digest('hex');
const redactCap = (s) => String(s).replace(/\/s\/[0-9a-f]{64}\//g, '/s/<capability>/');
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

class Cdp {
  constructor(ws) {
    this.ws = ws;
    this.id = 0;
    this.pending = new Map();
    this.listeners = [];
  }
  static async connect(wsUrl) {
    const ws = new WebSocket(wsUrl);
    await new Promise((resolve, reject) => {
      ws.addEventListener('open', resolve, { once: true });
      ws.addEventListener('error', (e) => reject(new Error('websocket failed: ' + (e.message || 'error'))), { once: true });
    });
    const cdp = new Cdp(ws);
    ws.addEventListener('message', (event) => {
      const message = JSON.parse(event.data);
      if (message.id && cdp.pending.has(message.id)) {
        const { resolve, reject } = cdp.pending.get(message.id);
        cdp.pending.delete(message.id);
        if (message.error) reject(new Error(message.error.message));
        else resolve(message.result);
      } else if (message.method) {
        for (const fn of cdp.listeners) fn(message);
      }
    });
    return cdp;
  }
  onEvent(fn) { this.listeners.push(fn); }
  send(method, params = {}) {
    const id = ++this.id;
    return new Promise((resolve, reject) => {
      this.pending.set(id, { resolve, reject });
      this.ws.send(JSON.stringify({ id, method, params }));
    });
  }
  close() { try { this.ws.close(); } catch { /* ignore */ } }
}

async function freePort() {
  const net = await import('node:net');
  return new Promise((resolve) => {
    const server = net.createServer();
    server.listen(0, '127.0.0.1', () => {
      const { port } = server.address();
      server.close(() => resolve(port));
    });
  });
}

async function launch() {
  const port = await freePort();
  const profile = join(tmpdir(), 'ccf-s0c-parity-' + process.pid + '-' + Date.now());
  mkdirSync(profile, { recursive: true });
  const child = spawn(EDGE, [
    '--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check', '--disable-extensions',
    '--hide-scrollbars', '--force-device-scale-factor=1', `--window-size=${width},${height}`,
    '--remote-debugging-port=' + port, '--user-data-dir=' + profile, 'about:blank',
  ], { stdio: 'ignore', detached: false });
  let target = null;
  for (let attempt = 0; attempt < 200 && !target; attempt++) {
    await sleep(100);
    try {
      const list = await (await fetch('http://127.0.0.1:' + port + '/json/list')).json();
      target = list.find((t) => t.type === 'page');
    } catch { /* wait */ }
  }
  if (!target) throw new Error('Edge did not expose a page target');
  const cdp = await Cdp.connect(target.webSocketDebuggerUrl);
  return { cdp, child, profile };
}

async function viewport(cdp, w, h) {
  await cdp.send('Emulation.setDeviceMetricsOverride', { width: w, height: h, deviceScaleFactor: 1, mobile: false });
  await sleep(150);
}

async function evaluate(cdp, expression) {
  const result = await cdp.send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
  if (result.exceptionDetails) {
    throw new Error('page error: ' + (result.exceptionDetails.exception?.description || result.exceptionDetails.text));
  }
  return result.result.value;
}

async function waitFor(cdp, expression, what, timeoutMs = 25000) {
  const deadline = Date.now() + timeoutMs;
  while (Date.now() < deadline) {
    if (await evaluate(cdp, 'Boolean(' + expression + ')').catch(() => false)) return true;
    await sleep(80);
  }
  throw new Error('timed out waiting for ' + what);
}

async function rectOf(cdp, selector) {
  return evaluate(cdp, `(() => {
    const e = document.querySelector(${JSON.stringify(selector)});
    if (!e) return null;
    const r = e.getBoundingClientRect();
    return {
      x: r.x + r.width / 2, y: r.y + r.height / 2,
      w: r.width, h: r.height,
      visible: r.width > 0 && r.height > 0 && getComputedStyle(e).visibility !== 'hidden'
    };
  })()`);
}

async function click(cdp, selector) {
  const rect = await rectOf(cdp, selector);
  if (!rect || !rect.visible) throw new Error('cannot click ' + selector + ' (not visible)');
  const base = { x: rect.x, y: rect.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(100);
}

async function press(cdp, key) {
  const map = { Enter: ['Enter', 13, '\r'], Escape: ['Escape', 27, undefined] };
  const [code, vk, text] = map[key] || [key, 0, undefined];
  for (const type of ['keyDown', 'keyUp']) {
    await cdp.send('Input.dispatchKeyEvent', {
      type, key, code, windowsVirtualKeyCode: vk, nativeVirtualKeyCode: vk,
      text: type === 'keyDown' ? text : undefined,
    });
  }
  await sleep(120);
}

async function typeInto(cdp, selector, text) {
  await click(cdp, selector);
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); e.select(); })()`);
  await cdp.send('Input.insertText', { text });
  await sleep(60);
}

async function main() {
  const { cdp, child, profile } = await launch();
  const inFlight = new Set();
  const networkLog = [];
  const consoleErrors = [];
  const cspViolations = [];
  const httpErrors = [];
  const nonLoopback = [];
  const requestMap = new Map();

  cdp.onEvent((ev) => {
    if (ev.method === 'Network.requestWillBeSent') {
      const { requestId, request, type } = ev.params;
      if (request.url === 'about:blank') return;
      inFlight.add(requestId);
      const entry = {
        method: request.method,
        url: redactCap(request.url),
        rawOrigin: null,
        type: type || 'Other',
        status: null,
        failed: false,
      };
      try {
        const u = new URL(request.url);
        entry.rawOrigin = u.origin;
        if (u.origin !== expectedOrigin) {
          nonLoopback.push({ url: redactCap(request.url), origin: u.origin, expected: expectedOrigin });
        }
      } catch {
        nonLoopback.push({ url: redactCap(request.url), origin: '<invalid>', expected: expectedOrigin });
      }
      requestMap.set(requestId, entry);
      networkLog.push(entry);
    } else if (ev.method === 'Network.responseReceived') {
      const { requestId, response } = ev.params;
      const entry = requestMap.get(requestId);
      if (entry) {
        entry.status = response.status;
        if (response.status >= 400) {
          httpErrors.push({ method: entry.method, url: entry.url, status: response.status });
        }
      }
    } else if (ev.method === 'Network.loadingFinished') {
      inFlight.delete(ev.params.requestId);
    } else if (ev.method === 'Network.loadingFailed') {
      inFlight.delete(ev.params.requestId);
      const entry = requestMap.get(ev.params.requestId);
      if (entry) {
        entry.failed = true;
        entry.errorText = ev.params.errorText;
        httpErrors.push({ method: entry.method, url: entry.url, errorText: ev.params.errorText });
      }
    } else if (ev.method === 'Runtime.exceptionThrown') {
      consoleErrors.push({ kind: 'exception', text: ev.params.exceptionDetails?.exception?.description || ev.params.exceptionDetails?.text });
    } else if (ev.method === 'Runtime.consoleAPICalled' && ev.params.type === 'error') {
      consoleErrors.push({ kind: 'console.error', text: (ev.params.args || []).map((a) => a.value ?? a.description).join(' ') });
    } else if (ev.method === 'Log.entryAdded') {
      const e = ev.params.entry;
      const text = e.text || '';
      if (e.source === 'security' || /Content Security Policy|CSP/i.test(text)) {
        cspViolations.push({ source: e.source, level: e.level, text: redactCap(text) });
      }
      if (e.level === 'error') {
        consoleErrors.push({ kind: 'log.' + e.source, text: redactCap(text) });
      }
    }
  });

  async function waitNetworkQuiet(quietMs = 250, timeoutMs = 15000) {
    const deadline = Date.now() + timeoutMs;
    let quietSince = null;
    while (Date.now() < deadline) {
      const busy = await evaluate(cdp, "document.documentElement.classList.contains('busy')").catch(() => true);
      if (inFlight.size === 0 && !busy) {
        if (quietSince === null) quietSince = Date.now();
        if (Date.now() - quietSince >= quietMs) return;
      } else {
        quietSince = null;
      }
      await sleep(50);
    }
    throw new Error('timed out waiting for network quiet (inFlight=' + inFlight.size + ')');
  }

  let sessionId = null;
  const states = [];

  async function captureState(stateName) {
    await waitNetworkQuiet(280, 20000);
    // Blur active text input before screenshot so blinking text carets cannot cause non-semantic pixel diffs.
    await evaluate(cdp, `(() => {
      if (document.activeElement && (document.activeElement.tagName === 'INPUT' || document.activeElement.tagName === 'TEXTAREA')) {
        document.activeElement.blur();
      }
    })()`);
    await sleep(160);
    let html = await evaluate(cdp, 'document.body.outerHTML');
    if (sessionId) {
      const short12 = sessionId.slice(0, 12) + '…';
      html = html.split(sessionId).join('<SESSION_ID>').split(short12).join('<SESSION_ID_12>…');
    }
    const htmlBuf = Buffer.from(html, 'utf8');
    const htmlFile = `${cell}__${stateName}.html`;
    writeFileSync(join(out, htmlFile), htmlBuf);

    const shot = await cdp.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
    const pngBuf = Buffer.from(shot.data, 'base64');
    const pngFile = `${cell}__${stateName}.png`;
    writeFileSync(join(out, pngFile), pngBuf);

    const rec = {
      state: stateName,
      htmlFile,
      htmlBytes: htmlBuf.length,
      htmlSha256: sha256Hex(htmlBuf),
      pngFile,
      pngBytes: pngBuf.length,
      pngSha256: sha256Hex(pngBuf),
    };
    states.push(rec);
    log(`STATE ${cell} ${stateName} html=${rec.htmlSha256.slice(0, 12)} (${rec.htmlBytes} B) png=${rec.pngSha256.slice(0, 12)} (${rec.pngBytes} B)`);
  }

  try {
    await cdp.send('Page.enable');
    await cdp.send('Runtime.enable');
    await cdp.send('Log.enable');
    await cdp.send('Network.enable');
    await viewport(cdp, width, height);
    await cdp.send('Page.navigate', { url });

    await waitFor(cdp,
      "document.getElementById('year-value') && document.getElementById('year-value').textContent.trim() === '0' && " +
      "document.querySelectorAll('#tree-rows [data-select]').length > 0 && " +
      "document.getElementById('skip-review-label') && document.getElementById('skip-review-label').textContent.includes('Yr 4')",
      'initial /multi render + review preview', 30000);
    await waitNetworkQuiet(280, 15000);

    sessionId = await evaluate(cdp, `(() => {
      const dts = Array.from(document.querySelectorAll('#facts dt'));
      const dt = dts.find((d) => d.textContent.trim() === 'Session');
      const code = dt && dt.nextElementSibling ? dt.nextElementSibling.querySelector('code.full') : null;
      return code ? code.textContent.trim() : null;
    })()`);
    check(Boolean(sessionId && sessionId.length >= 16), `${cell}: captured per-session id for canonicalization`);

    // 1. initial load
    await captureState('01-initial-load');

    // 2. tree selected with competition
    const compTree = await evaluate(cdp, `(() => {
      const rows = Array.from(document.querySelectorAll('#tree-rows [data-select]'));
      const target = rows.length > 60 ? rows[60] : rows[0];
      return target ? target.getAttribute('data-select') : null;
    })()`);
    check(Boolean(compTree), `${cell}: deterministic living tree found (${compTree})`);
    await click(cdp, '[data-pane-button="trees"]');
    await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'trees'", 'Trees pane');
    await evaluate(cdp, `document.querySelector('#tree-rows [data-select="${compTree}"]').scrollIntoView({ block: 'center' })`);
    await sleep(100);
    await click(cdp, `#tree-rows [data-select="${compTree}"]`);
    await waitFor(cdp,
      "!document.getElementById('inspector-body').hidden && " +
      "document.querySelectorAll('#competition-content li').length === 5 && " +
      "document.querySelectorAll('#rank-layer [data-rank-toggle]').length === 5",
      'tree inspector + 5 competitors');
    await captureState('02-tree-selected-competition');

    // 3. Fell draft pending (tick top competitor draft, add to pending Work Plan, view plan)
    await click(cdp, '#competition-content [data-draft]');
    await waitFor(cdp, "!document.getElementById('add-button').disabled", 'Add to Fell plan enabled');
    await typeInto(cdp, '#reason', 'S0c parity fell reason');
    await click(cdp, '#add-button');
    await waitFor(cdp,
      "document.querySelectorAll('#pending-areas [data-remove]').length === 1 && " +
      "document.getElementById('skip-review-sub').textContent.includes('Work plan open')",
      'pending Fell queued');
    await click(cdp, '#view-plan');
    await waitFor(cdp,
      "document.getElementById('side').getAttribute('data-pane') === 'plan' && !document.getElementById('approve').hidden",
      'Work Plan pane showing pending Fell');
    await captureState('03-fell-draft-pending');

    // 4. approved plan
    await click(cdp, '#approve');
    await waitFor(cdp,
      "!document.getElementById('approved-box').hidden && !document.getElementById('execute').hidden",
      'approved plan box');
    await captureState('04-approved-plan');

    // 5. after Execute
    await click(cdp, '#execute');
    await waitFor(cdp, "document.getElementById('execute-dialog').open", 'execute confirmation dialog');
    await click(cdp, '#execute-confirm');
    await waitFor(cdp,
      "!document.getElementById('execute-dialog').open && " +
      "document.getElementById('receipt').textContent.includes('Completed') && " +
      "document.getElementById('skip-review-label').textContent === 'Skip → Yr 4'",
      'executed plan receipt + anchored review');
    await captureState('05-after-execute');

    // 6. after Year +1
    await click(cdp, '#advance');
    await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'Year +1 advance', 25000);
    await captureState('06-after-year-plus-1');

    // 7. Saved games dialog
    await click(cdp, '#saves-open');
    await waitFor(cdp,
      "document.getElementById('saves-dialog').open && Boolean(document.querySelector('#saves-rows tr'))",
      'Saved games dialog loaded');
    await captureState('07-saved-games-dialog');
    await press(cdp, 'Escape');
    await waitFor(cdp, "!document.getElementById('saves-dialog').open", 'Saved games dialog closed');

    // 8. history drawer (open + select Year 0 detail)
    await click(cdp, '#history-open');
    await waitFor(cdp,
      "!document.getElementById('history-drawer').hidden && Boolean(document.querySelector('#history-year-list [data-history-year=\"0\"]'))",
      'History drawer index');
    await click(cdp, '#history-year-list [data-history-year="0"]');
    await waitFor(cdp,
      "document.getElementById('history-detail-pane').textContent.includes('Work Executed — Year 0') && " +
      "document.getElementById('history-detail-pane').textContent.includes('Observed Outcome — Year 0 → Year 1')",
      'History Year 0 detail');
    await captureState('08-history-drawer');
    await press(cdp, 'Escape');
    await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'History drawer closed');

    // 9. structure panel
    await click(cdp, '[data-pane-button="details"]');
    await waitFor(cdp, "document.getElementById('structure-content').textContent.includes('Whole woodland')", 'Structure panel loaded');
    await captureState('09-structure-panel');

    // Network & error assertions
    check(nonLoopback.length === 0, `${cell}: zero non-loopback network requests (${networkLog.length} total, all ${expectedOrigin})`);
    check(httpErrors.length === 0, `${cell}: zero HTTP 4xx/5xx or failed requests` + (httpErrors.length ? ': ' + JSON.stringify(httpErrors) : ''));
    check(consoleErrors.length === 0, `${cell}: zero console errors or uncaught exceptions` + (consoleErrors.length ? ': ' + JSON.stringify(consoleErrors) : ''));
    check(cspViolations.length === 0, `${cell}: zero CSP violations` + (cspViolations.length ? ': ' + JSON.stringify(cspViolations) : ''));
    check(states.length === 9, `${cell}: captured all 9 required parity states`);

    writeFileSync(join(out, `${cell}__network.json`), JSON.stringify({
      cell,
      expectedOrigin,
      requestCount: networkLog.length,
      nonLoopback,
      httpErrors,
      requests: networkLog,
    }, null, 2) + '\n');

    writeFileSync(join(out, `${cell}__manifest.json`), JSON.stringify({
      cell,
      width,
      height,
      sessionIdCanonicalized: Boolean(sessionId),
      consoleErrorCount: consoleErrors.length,
      cspViolationCount: cspViolations.length,
      httpErrorCount: httpErrors.length,
      nonLoopbackCount: nonLoopback.length,
      requestCount: networkLog.length,
      states,
    }, null, 2) + '\n');
  } finally {
    cdp.close();
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    try { rmSync(profile, { recursive: true, force: true }); } catch { /* ignore */ }
  }

  log(`CELL ${cell}: ${passes} passed, ${failures} failed`);
  process.exit(failures === 0 ? 0 : 1);
}

main().catch((err) => {
  console.error('FATAL', err && err.stack ? err.stack : String(err));
  process.exit(1);
});
