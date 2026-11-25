// Phase 1 Step 2 real-browser acceptance driver (Microsoft Edge over CDP).
// Exercises Time Pacing (Skip to Review), Decision -> Outcome History drawer, Woodland Structure (M_i & T_i),
// bounded multi-decade view payloads (Year 0, Year 5, Year 15+, and truncation proof), NB-2 Plant-drawer fold
// polish at BOTH 1280x720 and 1920x1080, 4-cell resolution x experiment matrix (1920x1080 & 1280x720 x MA-1 & MA-1-MIXED),
// and Save -> quit server -> restart -> Load continuity.
'use strict';
import { spawn } from 'node:child_process';
import { mkdirSync, readFileSync, writeFileSync, rmSync } from 'node:fs';
import { join } from 'node:path';
import { tmpdir } from 'node:os';

const args = process.argv.slice(2);
const command = args[0];
const opt = (name, fallback = null) => {
  const i = args.indexOf('--' + name);
  return i >= 0 && i + 1 < args.length ? args[i + 1] : fallback;
};
const url = opt('url');
const out = opt('out', '.');
const slot = opt('slot', 'step2-e2e');
const snapFile = opt('snap', null);
const EDGE = process.env.EDGE || 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';
const EXACT_NON_CAUSAL = 'Observed changes reflect ongoing natural growth, competition, seeding, and light alongside recorded management — not a single-cause effect.';
const EXACT_STRUCTURE_DISCLAIMER = 'Spatial structure descriptor (up to k=3 nearest living trees) — not measured biodiversity, habitat quality, or an Irish forecast.';

let failures = 0, passes = 0;
const log = (line) => console.log(line);
function check(ok, title) {
  if (ok) { passes++; log('PASS ' + title); } else { failures++; log('FAIL ' + title); }
}

class Cdp {
  constructor(ws) { this.ws = ws; this.id = 0; this.pending = new Map(); this.events = []; }
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
        if (message.error) reject(new Error(message.error.message)); else resolve(message.result);
      } else if (message.method) cdp.events.push(message);
    });
    return cdp;
  }
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
    server.listen(0, '127.0.0.1', () => { const { port } = server.address(); server.close(() => resolve(port)); });
  });
}

async function launch() {
  const port = await freePort();
  const profile = join(tmpdir(), 'ccf-step2-edge-' + process.pid + '-' + Date.now());
  mkdirSync(profile, { recursive: true });
  const child = spawn(EDGE, [
    '--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check', '--disable-extensions',
    '--hide-scrollbars', '--force-device-scale-factor=1', '--window-size=1280,720',
    '--remote-debugging-port=' + port, '--user-data-dir=' + profile, 'about:blank',
  ], { stdio: 'ignore', detached: false });
  let target = null;
  for (let attempt = 0; attempt < 200 && !target; attempt++) {
    await new Promise((r) => setTimeout(r, 100));
    try {
      const list = await (await fetch('http://127.0.0.1:' + port + '/json/list')).json();
      target = list.find((t) => t.type === 'page');
    } catch { /* wait */ }
  }
  if (!target) throw new Error('Edge did not expose a page target');
  const cdp = await Cdp.connect(target.webSocketDebuggerUrl);
  return { cdp, child, profile };
}

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

async function viewport(cdp, width, height) {
  await cdp.send('Emulation.setDeviceMetricsOverride', { width, height, deviceScaleFactor: 1, mobile: false });
  await sleep(160);
}

async function evaluate(cdp, expression) {
  const result = await cdp.send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
  if (result.exceptionDetails) throw new Error('page error: ' + (result.exceptionDetails.exception?.description || result.exceptionDetails.text));
  return result.result.value;
}

async function waitFor(cdp, expression, what, timeoutMs = 25000) {
  const deadline = Date.now() + timeoutMs;
  while (Date.now() < deadline) {
    if (await evaluate(cdp, 'Boolean(' + expression + ')').catch(() => false)) return true;
    await sleep(100);
  }
  throw new Error('timed out waiting for ' + what);
}

async function rectOf(cdp, selector) {
  return evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (!e) return null;
    const r = e.getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2,
    left: r.left, top: r.top, right: r.right, bottom: r.bottom, w: r.width, h: r.height,
    visible: r.width > 0 && r.height > 0 && getComputedStyle(e).visibility !== 'hidden' }; })()`);
}

async function click(cdp, selector) {
  const rect = await rectOf(cdp, selector);
  if (!rect || !rect.visible) throw new Error('cannot click ' + selector + ' (not visible)');
  const base = { x: rect.x, y: rect.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(120);
}

async function clickPoint(cdp, x, y) {
  const base = { x, y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(140);
}

async function press(cdp, key) {
  const map = { Enter: ['Enter', 13, '\r'], Space: ['Space', 32, ' '], Tab: ['Tab', 9, undefined], Escape: ['Escape', 27, undefined] };
  const [code, vk, text] = map[key] || [key, 0, undefined];
  for (const type of ['keyDown', 'keyUp']) {
    await cdp.send('Input.dispatchKeyEvent', {
      type, key: key === 'Space' ? ' ' : key, code, windowsVirtualKeyCode: vk, nativeVirtualKeyCode: vk,
      text: type === 'keyDown' ? text : undefined,
    });
  }
  await sleep(140);
}

async function typeInto(cdp, selector, text) {
  await click(cdp, selector);
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); e.select(); })()`);
  await cdp.send('Input.insertText', { text });
  await sleep(60);
}

async function shot(cdp, name, width, height) {
  await viewport(cdp, width, height);
  await sleep(220);
  const data = await cdp.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
  const file = join(out, name + '-' + width + 'x' + height + '.png');
  writeFileSync(file, Buffer.from(data.data, 'base64'));
  log('SHOT ' + file);
}

async function fetchJson(cdp, path, body) {
  const expression = `(async () => { const r = await fetch(${JSON.stringify(path)}, ${body === undefined
    ? `{ method: 'GET', cache: 'no-store', credentials: 'omit' }`
    : `{ method: 'POST', cache: 'no-store', credentials: 'omit', headers: { 'Content-Type': 'application/json' }, body: ${JSON.stringify(JSON.stringify(body))} }`});
    const raw = await r.text();
    let p = null; try { p = JSON.parse(raw); } catch {}
    const bytes = new TextEncoder().encode(raw).length;
    let ecoBytes = null;
    if (p && p.ecology !== undefined) ecoBytes = new TextEncoder().encode(JSON.stringify(p.ecology)).length;
    return { status: r.status, bytes, ecoBytes, payload: p }; })()`;
  return evaluate(cdp, expression);
}

async function measureViewBudget(cdp, label) {
  const res = await fetchJson(cdp, 'api/multi/view');
  const ok = res.status === 200 && res.bytes <= 524288 && res.ecoBytes !== null && res.ecoBytes <= 153600;
  log(`BUDGET ${label}: year=${res.payload.view.source.ecologicalYear} raw=${res.bytes} B (<=524288) ecology=${res.ecoBytes} B (<=153600) historyTruncated=${res.payload.historyTruncated} totalRecordedYears=${res.payload.totalRecordedYears}`);
  check(ok, `payload budget ${label}: view=${res.bytes} B <= 524288 and ecology=${res.ecoBytes} B <= 153600`);
  return res;
}

function fenceOf(view) { return { sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision }; }
async function currentView(cdp) { return (await fetchJson(cdp, 'api/multi/view')).payload.view; }

const deliberateRefusal = (entry) => entry.source === 'network' &&
  /status of (400|403|404|405|409|413|415)/.test(entry.text || '');

function watchConsole(cdp) {
  cdp.events.length = 0;
  return () => cdp.events.filter((e) => {
    if (e.method === 'Runtime.exceptionThrown') return true;
    if (e.method === 'Runtime.consoleAPICalled' && e.params.type === 'error') return true;
    if (e.method === 'Log.entryAdded' && e.params.entry.level === 'error') return !deliberateRefusal(e.params.entry);
    return false;
  });
}

async function withPage(fn) {
  const { cdp, child, profile } = await launch();
  try {
    await cdp.send('Page.enable');
    await cdp.send('Runtime.enable');
    await cdp.send('Log.enable');
    await viewport(cdp, 1280, 720);
    await cdp.send('Page.navigate', { url });
    await waitFor(cdp, "document.getElementById('year-value') && document.getElementById('year-value').textContent.trim() !== '–'",
      'the /multi page to render', 30000);
    await sleep(350);
    await fn(cdp);
  } finally {
    cdp.close();
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    try { rmSync(profile, { recursive: true, force: true }); } catch { /* ignore */ }
  }
}

// Checks zero horizontal scroll, single-row topbar (no wrapping), and R3 5-competitor above-the-fold gate.
async function checkLayoutAndR3Fold(cdp, tag) {
  for (const [w, h] of [[1920, 1080], [1280, 720]]) {
    await viewport(cdp, w, h);
    await sleep(200);
    const geom = await evaluate(cdp, `(() => {
      const noHScroll = document.documentElement.scrollWidth <= window.innerWidth + 1;
      const topbar = document.querySelector('.topbar');
      const actions = Array.from(document.querySelectorAll('.top-actions > button'));
      const rects = actions.map((b) => { const r = b.getBoundingClientRect(); return { id: b.id, top: Math.round(r.top), bottom: Math.round(r.bottom), right: Math.round(r.right), h: Math.round(r.height) }; });
      const maxTopDelta = rects.length ? Math.max(...rects.map((r) => r.top)) - Math.min(...rects.map((r) => r.top)) : 0;
      const allInWidth = rects.every((r) => r.right <= window.innerWidth + 1);
      return { noHScroll, scrollW: document.documentElement.scrollWidth, innerW: window.innerWidth, topbarH: Math.round(topbar.getBoundingClientRect().height), maxTopDelta, allInWidth, rects };
    })()`);
    check(geom.noHScroll && geom.maxTopDelta <= 10 && geom.allInWidth,
      `${tag} ${w}x${h}: zero horizontal scroll (${geom.scrollW}<=${geom.innerW}) and single-row topbar (h=${geom.topbarH}px, delta=${geom.maxTopDelta}px)`);
  }

  // Select a living MA-A tree with 5 competitors and prove R3 5-competitor above-the-fold at 1280x720.
  await viewport(cdp, 1280, 720);
  const compTree = await evaluate(cdp, `(() => {
    const rows = Array.from(document.querySelectorAll('#tree-rows [data-select]'));
    return rows.length > 60 ? rows[60].getAttribute('data-select') : (rows[0] ? rows[0].getAttribute('data-select') : null);
  })()`);
  await click(cdp, '[data-pane-button="trees"]');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'trees'", 'Trees pane');
  await evaluate(cdp, `document.querySelector('#tree-rows [data-select="${compTree}"]').scrollIntoView({ block: 'center' })`);
  await sleep(100);
  await click(cdp, `#tree-rows [data-select="${compTree}"]`);
  await waitFor(cdp, "document.querySelectorAll('#competition-content li').length === 5 && !document.getElementById('inspector-body').hidden",
    '5 competitors loaded in Tree inspector');
  await sleep(150);
  const fold = await evaluate(cdp, `(() => {
    const pane = document.getElementById('pane-tree');
    const pr = pane.getBoundingClientRect();
    const refCard = document.querySelector('#inspector-body .ref-card').getBoundingClientRect();
    const rows = Array.from(document.querySelectorAll('#competition-content li')).map((li) => li.getBoundingClientRect());
    const addBtn = document.getElementById('add-button').getBoundingClientRect();
    const maxBottom = Math.max(refCard.bottom, ...rows.map((r) => r.bottom), addBtn.bottom);
    return {
      scrollTop: Math.round(pane.scrollTop),
      rowCount: rows.length,
      refTop: Math.round(refCard.top),
      addBottom: Math.round(addBtn.bottom),
      paneBottom: Math.round(pr.bottom),
      vh: window.innerHeight,
      fits: pane.scrollTop === 0 && rows.length === 5 && refCard.top >= pr.top - 1 && maxBottom <= Math.min(pr.bottom, window.innerHeight) + 1
    };
  })()`);
  check(fold.fits, `${tag} 1280x720 R3 fold preserved (5 competitors + Add visible without scroll: ${JSON.stringify(fold)})`);
  await shot(cdp, `${tag}-r3-fold`, 1280, 720);
  await shot(cdp, `${tag}-desktop`, 1920, 1080);
  await click(cdp, '#inspect-close');
  await waitFor(cdp, "document.getElementById('inspector-body').hidden", 'inspector closed');
}

// Find a valid open planting point in the active area and return its screen coordinates on the #map SVG.
async function findOpenMapPoint(cdp, areaId) {
  const res = await fetchJson(cdp, 'api/multi/view');
  const area = res.payload.view.areas.find((a) => a.areaId === areaId);
  const s4 = res.payload.stage4.areas.find((a) => a.areaId === areaId);
  const ext = area.extent;
  const allLiving = res.payload.view.areas.flatMap((a) => a.trees).filter((t) => t.isLiving);
  for (const cell of s4.ground) {
    const x = ext.minXM + cell.centreU;
    const z = ext.minZM + ext.sideM - cell.centreV;
    if (allLiving.every((t) => Math.hypot(t.xm - x, t.zm - z) >= 1.35)) {
      const screen = await evaluate(cdp, `(() => {
        const svg = document.getElementById('map');
        const pt = new DOMPoint(${cell.centreU}, ${cell.centreV}).matrixTransform(svg.getScreenCTM());
        return { sx: pt.x, sy: pt.y };
      })()`);
      return { u: cell.centreU, v: cell.centreV, x, z, column: cell.column, row: cell.row, sx: screen.sx, sy: screen.sy };
    }
  }
  throw new Error('no valid open planting point found in ' + areaId);
}

// NB-2 proof: partial refusal on Plant -> Shelter -> SpotClear (pre-queued SpotClear causes step-3 409 Phase2DuplicateOrder)
// at BOTH 1280x720 and 1920x1080: refusal code AND "Already queued" list + working Remove buttons strictly visible without scroll!
async function checkNb2AtResolution(cdp, width, height) {
  await viewport(cdp, width, height);
  await sleep(180);
  const pt = await findOpenMapPoint(cdp, 'MA-A');

  // Pre-queue SpotClear at pt so step 3 of Plant+Shelter+SpotClear hits 409 Phase2DuplicateOrder.
  await click(cdp, '#mode-spot');
  await waitFor(cdp, "!document.getElementById('spot-panel').hidden", 'Spot Clear panel');
  await clickPoint(cdp, pt.sx, pt.sy);
  await waitFor(cdp, "!document.getElementById('spot-queue').disabled", 'Spot Clear Queue enabled');
  await click(cdp, '#spot-queue');
  await waitFor(cdp, "document.querySelectorAll('#spot-patches [data-spot-order]').length === 1", 'pre-queued SpotClear');
  const preSpotId = await evaluate(cdp, "document.querySelector('#spot-patches [data-spot-order]').getAttribute('data-spot-order')");

  // Switch to Plant mode, click same point, check Shelter + SpotClear, click Queue.
  await click(cdp, '#mode-plant');
  await waitFor(cdp, "!document.getElementById('plant-panel').hidden", 'Plant panel');
  await clickPoint(cdp, pt.sx, pt.sy);
  await evaluate(cdp, `(() => {
    const sh = document.getElementById('plant-opt-shelter');
    const sp = document.getElementById('plant-opt-spot');
    if (!sh.checked) sh.click();
    if (!sp.checked) sp.click();
  })()`);
  await sleep(80);
  await click(cdp, '#plant-queue');
  await waitFor(cdp, "!document.getElementById('plant-result').hidden && document.getElementById('plant-result').textContent.includes('Phase2DuplicateOrder')",
    'NB-2 partial refusal Phase2DuplicateOrder');
  await sleep(150);

  const nb2 = await evaluate(cdp, `(() => {
    const pane = document.getElementById('pane-tree');
    const pr = pane.getBoundingClientRect();
    const box = document.getElementById('plant-result');
    const br = box.getBoundingClientRect();
    const codeEl = box.querySelector('code');
    const cr = codeEl ? codeEl.getBoundingClientRect() : null;
    const btns = Array.from(box.querySelectorAll('[data-planting-remove]'));
    const btnRects = btns.map((b) => {
      const r = b.getBoundingClientRect();
      return { id: b.getAttribute('data-planting-remove'), top: Math.round(r.top), bottom: Math.round(r.bottom), right: Math.round(r.right),
        visibleInPane: r.top >= pr.top - 1 && r.bottom <= pr.bottom + 1,
        visibleInViewport: r.top >= 0 && r.bottom <= window.innerHeight + 1 && r.right <= window.innerWidth + 1 };
    });
    const text = box.textContent;
    return {
      scrollTop: Math.round(pane.scrollTop),
      hasCode: text.includes('Phase2DuplicateOrder'),
      hasAlreadyQueued: text.includes('Already queued'),
      btnCount: btns.length,
      boxTop: Math.round(br.top), boxBottom: Math.round(br.bottom),
      paneTop: Math.round(pr.top), paneBottom: Math.round(pr.bottom),
      vh: window.innerHeight,
      codeVisible: Boolean(cr) && cr.top >= Math.max(0, pr.top) - 1 && cr.bottom <= Math.min(window.innerHeight, pr.bottom) + 1,
      allBtnsVisible: btnRects.length === 2 && btnRects.every((b) => b.visibleInPane && b.visibleInViewport),
      btnRects
    };
  })()`);

  check(nb2.scrollTop === 0 && nb2.hasCode && nb2.hasAlreadyQueued && nb2.codeVisible && nb2.allBtnsVisible,
    `NB-2 ${width}x${height}: Phase2DuplicateOrder + Already queued + both Remove buttons visible without scrolling (${JSON.stringify(nb2)})`);
  await shot(cdp, 'nb2-plant-partial-refusal', width, height);

  // Click BOTH working Remove buttons directly from #plant-result without scrolling!
  await click(cdp, '#plant-result [data-planting-remove]');
  await waitFor(cdp, "document.querySelectorAll('#plant-result [data-planting-remove]').length === 1", 'first NB-2 Remove click');
  await click(cdp, '#plant-result [data-planting-remove]');
  await waitFor(cdp, "document.querySelectorAll('#plant-result [data-planting-remove]').length === 0", 'second NB-2 Remove click');

  // Remove the pre-existing SpotClear so the plan is completely clean for subsequent checks.
  const v = await currentView(cdp);
  const rm = await fetchJson(cdp, 'api/multi/planting/remove', { fence: fenceOf(v), orderId: preSpotId });
  check(rm.status === 200, `NB-2 ${width}x${height}: Remove buttons cleared partial orders and pre-SpotClear cleaned`);
  await click(cdp, '#mode-tree');
  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('pending-total').textContent === 'No pending orders'", 'clean plan after NB-2');
}

// ------------------------------------------------------------------------------------------------ command 1: ma1-full
async function runMa1Full(cdp) {
  const errors = watchConsole(cdp);

  await measureViewBudget(cdp, 'MA-1 Year 0');
  await checkLayoutAndR3Fold(cdp, 'ma1');

  // NB-2 proof at BOTH 1280x720 and 1920x1080!
  await checkNb2AtResolution(cdp, 1280, 720);
  await checkNb2AtResolution(cdp, 1920, 1080);

  // P1-UI-01: Year-0 observation skip label + GAMEPLAY CALIBRATION + WorkPlanOpen refusal without cancelling Pending or Approved work
  await viewport(cdp, 1280, 720);
  const skip0 = await evaluate(cdp, `({ label: document.getElementById('skip-review-label').textContent, sub: document.getElementById('skip-review-sub').textContent })`);
  check(skip0.label === 'Skip → Yr 4 (Obs.)' && skip0.sub.includes('Quiet observation') && skip0.sub.includes('GAMEPLAY CALIBRATION'),
    `MA-1 Year 0 Skip control shows Obs. + Quiet observation + GAMEPLAY CALIBRATION (${JSON.stringify(skip0)})`);

  // Queue a Pending Fell -> click Skip -> inline WorkPlanOpen refusal, Pending intact; Approve -> click Skip -> WorkPlanOpen, Approved intact
  const v0 = await currentView(cdp);
  const fellTree = v0.areas[0].trees.find((t) => t.isLiving).treeId;
  const q1 = await fetchJson(cdp, 'api/multi/pending/add', { fence: fenceOf(v0), areaId: 'MA-A', treeId: fellTree, reason: 'Step 2 open work test' });
  check(q1.status === 200, 'queued Pending Fell before Skip refusal test');
  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('skip-review-sub').textContent.includes('Work plan open')", 'Skip button blocked for Pending work');
  await click(cdp, '#skip-review');
  await waitFor(cdp, "document.getElementById('alert').textContent.includes('WorkPlanOpen')", 'Skip WorkPlanOpen inline refusal (Pending)');
  check((await currentView(cdp)).pending.length === 1, 'Pending Fell preserved intact after Skip WorkPlanOpen refusal');
  await shot(cdp, 'ma1-skip-workplan-open', 1280, 720);

  // Approve -> click Skip -> still refused WorkPlanOpen and Approved intact
  await click(cdp, '#view-plan');
  await waitFor(cdp, "!document.getElementById('approve').hidden", 'Approve button');
  await click(cdp, '#approve');
  await waitFor(cdp, "!document.getElementById('approved-box').hidden", 'Approved box');
  await click(cdp, '#skip-review');
  await waitFor(cdp, "document.getElementById('alert').textContent.includes('WorkPlanOpen')", 'Skip WorkPlanOpen inline refusal (Approved)');
  check((await currentView(cdp)).approved !== null, 'Approved plan preserved intact after Skip WorkPlanOpen refusal');

  // Cancel Approved plan -> perform real Year-0 observation Skip (Year 0 -> Year 4) via button click!
  const vApp = await currentView(cdp);
  const cancelled = await fetchJson(cdp, 'api/multi/plan/cancel', { fence: fenceOf(vApp) });
  check(cancelled.status === 200, 'cancelled test plan before Year-0 observation Skip');
  await click(cdp, '#reread');
  await waitFor(cdp, "!document.getElementById('skip-review').classList.contains('blocked')", 'Skip unblocked');
  await click(cdp, '#skip-review');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '4' && !document.getElementById('skip-summary').hidden",
    'Year-0 observation Skip to Year 4', 30000);
  const sum4 = await evaluate(cdp, "document.getElementById('skip-summary').textContent");
  check(sum4.includes('Year 0 → Year 4') && sum4.includes('ReachedReviewYear') && !sum4.includes('null') && sum4.includes('Quiet observation') && sum4.includes('GAMEPLAY CALIBRATION'),
    'Year-0 observation Skip summary displays stepped years, stopReason ReachedReviewYear, Quiet observation, and GAMEPLAY CALIBRATION');
  await shot(cdp, 'ma1-skip-yr4-obs', 1280, 720);

  // Execute a Fell at Year 4 -> post-Fell anchor Skip button updates to Skip -> Yr 8 (NOT Obs.) -> advance 1 yr -> Year 5 budget
  const v4 = await currentView(cdp);
  const f4Tree = v4.areas[0].trees.find((t) => t.isLiving).treeId;
  let r = await fetchJson(cdp, 'api/multi/pending/add', { fence: fenceOf(v4), areaId: 'MA-A', treeId: f4Tree, reason: 'Year 4 anchor fell' });
  r = await fetchJson(cdp, 'api/multi/plan/approve', { fence: fenceOf(r.payload.view) });
  r = await fetchJson(cdp, 'api/multi/plan/execute', { fence: fenceOf(r.payload.view), planId: r.payload.view.approved.planId, executionKey: r.payload.view.approved.executionKey });
  check(r.status === 200, 'executed Year-4 Fell to anchor review spacing');
  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('skip-review-label').textContent === 'Skip → Yr 8'", 'Skip label updated to anchored Yr 8');
  const skip8 = await evaluate(cdp, `({ label: document.getElementById('skip-review-label').textContent, sub: document.getElementById('skip-review-sub').textContent })`);
  check(skip8.label === 'Skip → Yr 8' && skip8.sub.includes('Fell Yr 4') && skip8.sub.includes('GAMEPLAY CALIBRATION'),
    `post-Fell anchor Skip shows Yr 8 (Fell Yr 4) + GAMEPLAY CALIBRATION (${JSON.stringify(skip8)})`);

  // Step to Year 5 and measure Year 5 budget
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '5'", 'Year 5 advance', 25000);
  await measureViewBudget(cdp, 'MA-1 Year 5');

  // Click Skip from Year 5 -> reaches anchored Year 8 (3 steps), then verify overdue fallback at Year 8 -> Year 12!
  await click(cdp, '#skip-review');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '8'", 'post-Fell Skip to Year 8', 30000);
  await waitFor(cdp, "document.getElementById('skip-review-label').textContent === 'Skip → Yr 12'", 'overdue fallback Yr 12');
  const skip12 = await evaluate(cdp, `({ label: document.getElementById('skip-review-label').textContent, sub: document.getElementById('skip-review-sub').textContent })`);
  check(skip12.label === 'Skip → Yr 12' && skip12.sub.includes('overdue') && skip12.sub.includes('GAMEPLAY CALIBRATION'),
    `overdue fallback at Year 8 (anchor 4) shows Skip -> Yr 12 (${JSON.stringify(skip12)})`);

  // Stale fence 409 check on preview-review and skip
  const stalePrev = await fetchJson(cdp, 'api/multi/year/preview-review', { fence: fenceOf(v0) });
  const staleSkip = await fetchJson(cdp, 'api/multi/year/skip', { fence: fenceOf(v0), targetYear: 12 });
  check(stalePrev.status === 409 && staleSkip.status === 409 && staleSkip.payload.view.source.ecologicalYear === 8,
    'stale fence returns 409 + fresh view on both preview-review and skip');

  // MA-1 Structure panel on-demand load
  await click(cdp, '[data-pane-button="details"]');
  await waitFor(cdp, "document.getElementById('structure-content').textContent.includes('Whole woodland')", 'MA-1 Structure panel loaded');
  const stText = await evaluate(cdp, `({ disc: document.getElementById('structure-disclaimer').textContent, body: document.getElementById('structure-content').textContent })`);
  const stApi = (await fetchJson(cdp, 'api/multi/structure')).payload;
  check(stApi.meanMingling === 0 && stApi.provenance === 'DERIVED' && stText.disc === EXACT_STRUCTURE_DISCLAIMER &&
    stText.body.includes('0.00') && stText.body.includes('DERIVED'),
    `MA-1 Structure shows M_i=0.00, T_i=${stApi.meanDiameterDifferentiation.toFixed(2)}, DERIVED, exact disclaimer`);
  await shot(cdp, 'ma1-structure', 1280, 720);

  // Advance MA-1 to Year 15 (measure Year 15 budget), then execute work & advance to Year 25 to actively prove tail truncation!
  let cur = await currentView(cdp);
  for (const yr of [12, 15]) {
    const sk = await fetchJson(cdp, 'api/multi/year/skip', { fence: fenceOf(cur), targetYear: yr });
    check(sk.status === 200, `MA-1 skip to Year ${yr}`);
    cur = sk.payload.view;
  }
  await measureViewBudget(cdp, 'MA-1 Year 15');

  // Execute 2 additional small Fells and advance to Year 26 so view payload exercises tail truncation (<= 524288 B, historyTruncated === true)
  for (const yr of [18, 22, 26]) {
    const tId = cur.areas[0].trees.find((t) => t.isLiving).treeId;
    let w = await fetchJson(cdp, 'api/multi/pending/add', { fence: fenceOf(cur), areaId: 'MA-A', treeId: tId, reason: 'decade thin yr ' + cur.source.ecologicalYear });
    w = await fetchJson(cdp, 'api/multi/plan/approve', { fence: fenceOf(w.payload.view) });
    w = await fetchJson(cdp, 'api/multi/plan/execute', { fence: fenceOf(w.payload.view), planId: w.payload.view.approved.planId, executionKey: w.payload.view.approved.executionKey });
    cur = w.payload.view;
    const sk = await fetchJson(cdp, 'api/multi/year/skip', { fence: fenceOf(cur), targetYear: yr });
    check(sk.status === 200, `MA-1 multi-decade skip to Year ${yr}`);
    cur = sk.payload.view;
  }
  let y26 = await measureViewBudget(cdp, 'MA-1 Year 26');
  // If Year 26 still fits without truncating, advance a few more years until historyTruncated flips true (proving live truncation!).
  while (!y26.payload.historyTruncated && cur.source.ecologicalYear < 45) {
    const sk = await fetchJson(cdp, 'api/multi/year/skip', { fence: fenceOf(cur), targetYear: cur.source.ecologicalYear + 5 });
    if (sk.status !== 200) break;
    cur = sk.payload.view;
    y26 = await measureViewBudget(cdp, 'MA-1 Year ' + cur.source.ecologicalYear);
  }
  const idxFull = (await fetchJson(cdp, 'api/multi/history/index?pageSize=50')).payload;
  const detOldest = await fetchJson(cdp, 'api/multi/history/year', { fence: fenceOf(cur), year: 4 });
  check(y26.payload.historyTruncated === true && y26.bytes <= 524288 && y26.ecoBytes <= 153600 &&
    idxFull.totalRecordedYears === cur.source.ecologicalYear + 1 && detOldest.status === 200 && detOldest.payload.workYear === 4,
    `MA-1 Year ${cur.source.ecologicalYear} tail truncation verified: historyTruncated=true, raw=${y26.bytes} B <= 524288, full history preserved (totalRecordedYears=${idxFull.totalRecordedYears}, Year 4 detail readable)`);

  const errs = errors();
  check(errs.length === 0, 'MA-1: zero console error, uncaught exception, CSP violation or 5xx' + (errs.length ? ': ' + JSON.stringify(errs[0]).slice(0, 220) : ''));
}

// ------------------------------------------------------------------------------------------------ command 2: mixed-play-save
async function runMixedPlaySave(cdp) {
  const errors = watchConsole(cdp);

  await measureViewBudget(cdp, 'MA-1-MIXED Year 0');
  await checkLayoutAndR3Fold(cdp, 'mixed');

  // Year 0 mixed work: Fell (MA-A), Prune (MA-B), ClearUndergrowth (MA-B), Plant (Oak) + Shelter + SpotClear (MA-B) -> Approve -> Execute
  await viewport(cdp, 1280, 720);
  let v = await currentView(cdp);
  const full0 = (await fetchJson(cdp, 'api/multi/view')).payload;
  const aFell = v.areas[0].trees.find((t) => t.isLiving).treeId;
  const bPrune = v.areas[1].trees.find((t) => t.isLiving).treeId;
  const bCell = full0.stage4.areas[1].ground[0];
  const allLiving = v.areas.flatMap((a) => a.trees).filter((t) => t.isLiving);
  const extB = v.areas[1].extent;
  let openB = null;
  for (const c of full0.stage4.areas[1].ground) {
    const x = extB.minXM + c.centreU;
    const z = extB.minZM + extB.sideM - c.centreV;
    if (allLiving.every((t) => Math.hypot(t.xm - x, t.zm - z) >= 1.35)) { openB = { x, z }; break; }
  }
  check(openB !== null, 'found valid open planting point in MA-B for mixed workflow');

  let r = await fetchJson(cdp, 'api/multi/pending/add', { fence: fenceOf(v), areaId: 'MA-A', treeId: aFell, reason: 'Step 2 mixed release fell' });
  r = await fetchJson(cdp, 'api/multi/prune/add', { fence: fenceOf(r.payload.view), areaId: 'MA-B', treeId: bPrune });
  r = await fetchJson(cdp, 'api/multi/clear/add', { fence: fenceOf(r.payload.view), areaId: 'MA-B', column: bCell.column, row: bCell.row });
  r = await fetchJson(cdp, 'api/multi/plant/add', { fence: fenceOf(r.payload.view), areaId: 'MA-B', speciesId: 'sessile-oak', xM: openB.x, zM: openB.z });
  r = await fetchJson(cdp, 'api/multi/shelter/add', { fence: fenceOf(r.payload.view), areaId: 'MA-B', xM: openB.x, zM: openB.z });
  r = await fetchJson(cdp, 'api/multi/spot-clear/add', { fence: fenceOf(r.payload.view), areaId: 'MA-B', xM: openB.x, zM: openB.z });
  check(r.status === 200, 'queued all 6 action families (Fell, Prune, Clear, Plant, Shelter, SpotClear) at Year 0');

  // Re-read, then Approve and Execute via real page buttons!
  await click(cdp, '#reread');
  await click(cdp, '#view-plan');
  await waitFor(cdp, "!document.getElementById('approve').hidden", 'Approve button');
  await click(cdp, '#approve');
  await waitFor(cdp, "!document.getElementById('execute').hidden", 'Execute button');
  await click(cdp, '#execute');
  await waitFor(cdp, "document.getElementById('execute-dialog').open", 'Execute confirmation');
  await click(cdp, '#execute-confirm');
  await waitFor(cdp, "!document.getElementById('execute-dialog').open && document.getElementById('receipt').textContent.includes('" + aFell + "')",
    'Execute commit', 30000);

  // Skip to Review via real page button (anchored Year 0 Fell -> Year 4), then Advance 1 year -> Year 5 budget
  await waitFor(cdp, "document.getElementById('skip-review-label').textContent === 'Skip → Yr 4'", 'Skip label Yr 4');
  await click(cdp, '#skip-review');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '4'", 'MA-1-MIXED Skip to Year 4', 30000);
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '5'", 'MA-1-MIXED Advance to Year 5', 30000);
  await measureViewBudget(cdp, 'MA-1-MIXED Year 5');

  // Advance to Year 15 via bounded skip calls (+5, +5), then measure Year 15 budget
  let cur = await currentView(cdp);
  for (const yr of [10, 15]) {
    const sk = await fetchJson(cdp, 'api/multi/year/skip', { fence: fenceOf(cur), targetYear: yr });
    check(sk.status === 200, `MA-1-MIXED skip to Year ${yr}`);
    cur = sk.payload.view;
  }
  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '15'", 'UI synced at Year 15');
  await measureViewBudget(cdp, 'MA-1-MIXED Year 15');

  // Tick 1 draft Fell choice on a LIVING MA-A tree BEFORE opening History drawer so we can prove Escape closes History without clearing it!
  await click(cdp, '[data-pane-button="trees"]');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'trees'", 'Trees pane');
  const liveTreeIds = cur.areas[0].trees.filter((t) => t.isLiving).map((t) => t.treeId);
  const draftRef = liveTreeIds[Math.min(20, liveTreeIds.length - 1)];
  await evaluate(cdp, `document.querySelector('#tree-rows [data-select="${draftRef}"]').scrollIntoView({ block: 'center' })`);
  await sleep(80);
  await click(cdp, `#tree-rows [data-select="${draftRef}"]`);
  await waitFor(cdp, "document.querySelectorAll('#competition-content [data-draft]').length > 0", 'competitor checkboxes');
  await click(cdp, '#competition-content [data-draft]');
  await waitFor(cdp, "document.getElementById('add-button').textContent === 'Add selected 1 to Fell plan'", '1 draft Fell tick armed');

  // P1-UI-02: Open History drawer -> verify newest-first paging (10 items of 16) -> Load older years -> click Year 0
  await click(cdp, '#history-open');
  await waitFor(cdp, "!document.getElementById('history-drawer').hidden && document.querySelectorAll('#history-year-list [data-history-year]').length === 10",
    'History drawer first page (10 years)');
  const firstPageYears = await evaluate(cdp, "Array.from(document.querySelectorAll('#history-year-list [data-history-year]')).map((b) => Number(b.getAttribute('data-history-year')))");
  check(JSON.stringify(firstPageYears) === JSON.stringify([15, 14, 13, 12, 11, 10, 9, 8, 7, 6]),
    'History drawer first page lists Years 15..6 newest-first (' + firstPageYears.join(',') + ')');

  await click(cdp, '#history-load-older');
  await waitFor(cdp, "document.querySelectorAll('#history-year-list [data-history-year]').length === 16 && document.getElementById('history-load-older').hidden",
    'Load older years to append Years 5..0');
  const allYears = await evaluate(cdp, "Array.from(document.querySelectorAll('#history-year-list [data-history-year]')).map((b) => Number(b.getAttribute('data-history-year')))");
  check(JSON.stringify(allYears) === JSON.stringify([15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0]),
    'Load older years appended Years 5..0 cleanly without gaps or duplicates');

  await evaluate(cdp, "document.querySelector('[data-history-year=\"0\"]').scrollIntoView({ block: 'center' })");
  await sleep(80);
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.getElementById('history-detail-pane').textContent.includes('Work Executed — Year 0') && document.getElementById('history-detail-pane').textContent.includes('Observed Outcome — Year 0 → Year 1')",
    'History Year 0 Decision -> Outcome detail rendered');

  const histAudit = await evaluate(cdp, `(() => {
    const pane = document.getElementById('history-detail-pane');
    const drawer = document.getElementById('history-drawer');
    const workSec = pane.querySelector('[data-history-section="work-executed"]');
    const outSec = pane.querySelector('[data-history-section="observed-outcome"]');
    const cmpSec = pane.querySelector('[data-history-section="later-comparison"]');
    const discSec = pane.querySelector('[data-history-section="not-recorded"]');
    const nonCausal = pane.querySelector('[data-non-causal]')?.textContent || '';
    const pills = Array.from(drawer.querySelectorAll('.prov-pill')).map((p) => p.textContent.trim());
    const text = pane.textContent;
    return {
      separated: Boolean(workSec && outSec && workSec !== outSec),
      hasWorkYr0: Boolean(workSec && workSec.textContent.includes('Work Executed — Year 0') && workSec.textContent.includes('Planted juvenile follow-up')),
      hasOutcomeYr1: Boolean(outSec && outSec.textContent.includes('Observed Outcome — Year 0 → Year 1')),
      hasComparison: Boolean(cmpSec && cmpSec.textContent.includes('Later Observed Comparison')),
      hasDisclosures: Boolean(discSec && discSec.textContent.includes('NOT RECORDED')),
      nonCausal,
      pills: [...new Set(pills)],
      hasWeather: /weather/i.test(text),
      hasEconomy: /€|\beuro\b|\brevenue\b|\bprice\b|\bcash\b|\bbudget\b/i.test(text),
    };
  })()`);
  const requiredPills = ['RECORDED', 'CURRENT', 'DERIVED', 'RECONSTRUCTED', 'SESSION-ONLY', 'NOT RECORDED'];
  check(histAudit.separated && histAudit.hasWorkYr0 && histAudit.hasOutcomeYr1 && histAudit.hasComparison && histAudit.hasDisclosures &&
    histAudit.nonCausal === EXACT_NON_CAUSAL && !histAudit.hasWeather && !histAudit.hasEconomy &&
    requiredPills.every((p) => histAudit.pills.includes(p)),
    'History Year 0 detail passes all P1-UI-02 honesty/provenance/timing/zero-economy checks (' + JSON.stringify({ pills: histAudit.pills, weather: histAudit.hasWeather, econ: histAudit.hasEconomy }) + ')');
  await shot(cdp, 'mixed-history-drawer-yr0', 1280, 720);
  await shot(cdp, 'mixed-history-drawer-yr0', 1920, 1080);

  // Escape closes History drawer WITHOUT clearing draft Fell tick!
  await viewport(cdp, 1280, 720);
  await press(cdp, 'Escape');
  await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'History drawer closed by Escape');
  const draftAfterEsc = await evaluate(cdp, `({ btn: document.getElementById('add-button').textContent, active: document.activeElement?.id })`);
  check(draftAfterEsc.btn === 'Add selected 1 to Fell plan' && draftAfterEsc.active === 'history-open',
    'Escape closed History drawer, focused #history-open, and preserved draft Fell tick (' + JSON.stringify(draftAfterEsc) + ')');

  // P1-UI-03: Open Details -> Structure panel on MA-1-MIXED (M_i > 0, DERIVED, exact disclaimer)
  await click(cdp, '[data-pane-button="details"]');
  await waitFor(cdp, "document.getElementById('structure-content').textContent.includes('Whole woodland')", 'MA-1-MIXED Structure loaded');
  const stMixed = (await fetchJson(cdp, 'api/multi/structure')).payload;
  const stUi = await evaluate(cdp, `(() => {
    const sec = document.getElementById('structure-section').getBoundingClientRect();
    const tbl = document.querySelector('#structure-content table');
    const tr = tbl ? tbl.getBoundingClientRect() : null;
    return {
      disc: document.getElementById('structure-disclaimer').textContent,
      body: document.getElementById('structure-content').textContent,
      tableFits: Boolean(tr) && tr.right <= sec.right + 1 && tr.right <= window.innerWidth + 1,
      tableW: tr ? Math.round(tr.width) : 0, secW: Math.round(sec.width)
    };
  })()`);
  const areaBMi = stMixed.areas.find((a) => a.areaId === 'MA-B').meanMingling;
  const areaAMi = stMixed.areas.find((a) => a.areaId === 'MA-A').meanMingling;
  check(stMixed.meanMingling > 0 && areaAMi >= 0 && areaBMi > areaAMi && areaBMi > 0 && stMixed.provenance === 'DERIVED' &&
    stUi.disc === EXACT_STRUCTURE_DISCLAIMER && stUi.body.includes(stMixed.meanMingling.toFixed(2)) &&
    stUi.body.includes(stMixed.meanDiameterDifferentiation.toFixed(2)) && stUi.tableFits,
    `MA-1-MIXED Structure verified: whole M_i=${stMixed.meanMingling.toFixed(4)} (>0), T_i=${stMixed.meanDiameterDifferentiation.toFixed(4)}, Area A=${areaAMi.toFixed(4)}, Area B=${areaBMi.toFixed(4)}, DERIVED, table fits (${stUi.tableW}<=${stUi.secW}px)`);
  await shot(cdp, 'mixed-structure', 1280, 720);

  // Capture pre-save snapshot of Preview, History index + Year 0 detail, and Structure
  const preView = await currentView(cdp);
  const prePreview = (await fetchJson(cdp, 'api/multi/year/preview-review', { fence: fenceOf(preView) })).payload;
  const preIdx = (await fetchJson(cdp, 'api/multi/history/index?pageSize=50')).payload;
  const preDet0 = (await fetchJson(cdp, 'api/multi/history/year', { fence: fenceOf(preView), year: 0 })).payload;
  if (snapFile) {
    writeFileSync(snapFile, JSON.stringify({
      year: preView.source.ecologicalYear,
      sessionId: preView.sessionId,
      preview: {
        currentYear: prePreview.currentYear, eligible: prePreview.eligible,
        suggestedTargetYear: prePreview.suggestedTargetYear, anchorFellYear: prePreview.anchorFellYear,
        isObservationPeriod: prePreview.isObservationPeriod, stopReason: prePreview.stopReason,
      },
      historyTotal: preIdx.totalRecordedYears,
      historyYears: preIdx.items.map((i) => i.year),
      det0WorkCounts: preDet0.totalWorkCounts,
      det0OutcomeStems: preDet0.annualOutcomeNextYear.areas.map((a) => [a.areaId, a.before.livingCount, a.after.livingCount]),
      structure: {
        meanMingling: stMixed.meanMingling,
        meanDiameterDifferentiation: stMixed.meanDiameterDifferentiation,
        focalLivingTreeCount: stMixed.focalLivingTreeCount,
        areas: stMixed.areas.map((a) => ({ areaId: a.areaId, m: a.meanMingling, t: a.meanDiameterDifferentiation, n: a.focalLivingTreeCount })),
      },
    }, null, 2));
  }

  // Save slot via real Saved games dialog
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open && document.querySelectorAll('#saves-rows tr').length > 0", 'Saved games dialog');
  await typeInto(cdp, '#save-slot', slot);
  await click(cdp, '#save-do');
  await waitFor(cdp, "document.getElementById('saves-status').textContent.includes('durably committed')", 'durable save commit', 30000);
  check(true, `saved MA-1-MIXED Year 15 session to slot '${slot}' durably`);
  await click(cdp, '#saves-close');

  // Also measure Year 20 on MA-1-MIXED (after Save) to confirm bounded view payload <= 512 KiB even with 350+ promoted broadleaf stems!
  const sk20 = await fetchJson(cdp, 'api/multi/year/skip', { fence: fenceOf(preView), targetYear: 20 });
  check(sk20.status === 200, 'MA-1-MIXED skip Year 15 -> Year 20');
  await measureViewBudget(cdp, 'MA-1-MIXED Year 20');

  const errs = errors();
  check(errs.length === 0, 'MA-1-MIXED play/save: zero console error, uncaught exception, CSP violation or 5xx' + (errs.length ? ': ' + JSON.stringify(errs[0]).slice(0, 220) : ''));
}

// ------------------------------------------------------------------------------------------------ command 3: mixed-restart-load
async function runMixedRestartLoad(cdp) {
  const errors = watchConsole(cdp);
  const pre = JSON.parse(readFileSync(snapFile, 'utf8'));
  const fresh = await currentView(cdp);
  check(fresh.source.ecologicalYear === 0 && fresh.sessionId !== pre.sessionId,
    'fresh restarted server starts at Year 0 with a new sessionId');

  // Load slot through Saved games dialog
  await viewport(cdp, 1280, 720);
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open && document.querySelector('[data-load=\"" + slot + "\"]')",
    'saved slot listed in restarted server');
  await click(cdp, `[data-load="${slot}"]`);
  await waitFor(cdp, "!document.getElementById('load-confirm').hidden", 'load confirmation');
  await click(cdp, '#load-do');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open && document.getElementById('year-value').textContent.trim() === '" + pre.year + "'",
    'restored Year 15 after Load', 30000);
  await sleep(450);

  const postView = await currentView(cdp);
  const postPreview = (await fetchJson(cdp, 'api/multi/year/preview-review', { fence: fenceOf(postView) })).payload;
  const postIdx = (await fetchJson(cdp, 'api/multi/history/index?pageSize=50')).payload;
  const postDet0 = (await fetchJson(cdp, 'api/multi/history/year', { fence: fenceOf(postView), year: 0 })).payload;
  const postSt = (await fetchJson(cdp, 'api/multi/structure')).payload;

  const previewMatches = postPreview.currentYear === pre.preview.currentYear &&
    postPreview.eligible === pre.preview.eligible &&
    postPreview.suggestedTargetYear === pre.preview.suggestedTargetYear &&
    postPreview.anchorFellYear === pre.preview.anchorFellYear &&
    postPreview.isObservationPeriod === pre.preview.isObservationPeriod &&
    postPreview.stopReason === pre.preview.stopReason;
  check(previewMatches, `Save/Restart/Load: restored Preview matches pre-save exactly (target Yr ${postPreview.suggestedTargetYear}, anchor ${postPreview.anchorFellYear})`);

  const historyMatches = postIdx.totalRecordedYears === pre.historyTotal &&
    JSON.stringify(postIdx.items.map((i) => i.year)) === JSON.stringify(pre.historyYears) &&
    JSON.stringify(postDet0.totalWorkCounts) === JSON.stringify(pre.det0WorkCounts) &&
    JSON.stringify(postDet0.annualOutcomeNextYear.areas.map((a) => [a.areaId, a.before.livingCount, a.after.livingCount])) === JSON.stringify(pre.det0OutcomeStems);
  check(historyMatches, `Save/Restart/Load: restored History index (${postIdx.totalRecordedYears} years) and Year 0 Work/Outcome match pre-save exactly`);

  const structureMatches = Math.abs(postSt.meanMingling - pre.structure.meanMingling) < 1e-12 &&
    Math.abs(postSt.meanDiameterDifferentiation - pre.structure.meanDiameterDifferentiation) < 1e-12 &&
    postSt.focalLivingTreeCount === pre.structure.focalLivingTreeCount &&
    JSON.stringify(postSt.areas.map((a) => ({ areaId: a.areaId, m: a.meanMingling, t: a.meanDiameterDifferentiation, n: a.focalLivingTreeCount }))) === JSON.stringify(pre.structure.areas);
  check(structureMatches, `Save/Restart/Load: restored Structure (M_i=${postSt.meanMingling.toFixed(4)}, T_i=${postSt.meanDiameterDifferentiation.toFixed(4)}, n=${postSt.focalLivingTreeCount}) matches pre-save exactly`);

  // Verify live UI panels render the restored History & Structure cleanly after Load
  await click(cdp, '[data-pane-button="details"]');
  await waitFor(cdp, "document.getElementById('structure-content').textContent.includes('" + postSt.meanMingling.toFixed(2) + "')", 'restored Structure UI');
  await shot(cdp, 'restored-structure', 1280, 720);
  await click(cdp, '#history-open');
  await waitFor(cdp, "!document.getElementById('history-drawer').hidden && document.querySelectorAll('#history-year-list [data-history-year]').length === 10",
    'restored History drawer UI');
  await click(cdp, '#history-load-older');
  await waitFor(cdp, "document.querySelector('[data-history-year=\"0\"]')", 'Year 0 button in restored History drawer');
  await evaluate(cdp, "document.querySelector('[data-history-year=\"0\"]').scrollIntoView({ block: 'center' })");
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.getElementById('history-detail-pane').textContent.includes('Work Executed — Year 0')", 'restored Year 0 detail UI');
  await shot(cdp, 'restored-history', 1280, 720);
  await shot(cdp, 'restored-history', 1920, 1080);

  const errs = errors();
  check(errs.length === 0, 'Save/Restart/Load: zero console error, uncaught exception, CSP violation or 5xx' + (errs.length ? ': ' + JSON.stringify(errs[0]).slice(0, 220) : ''));
}

mkdirSync(out, { recursive: true });
try {
  if (command === 'ma1-full') await withPage(runMa1Full);
  else if (command === 'mixed-play-save') await withPage(runMixedPlaySave);
  else if (command === 'mixed-restart-load') await withPage(runMixedRestartLoad);
  else throw new Error('unknown command: ' + command);
} catch (error) {
  failures++;
  log('FAIL driver error: ' + (error?.stack || error));
}
log('RESULT ' + (failures === 0 ? 'PASS' : 'FAIL') + ' (' + passes + ' passed, ' + failures + ' failed)');
process.exit(failures === 0 ? 0 : 1);
