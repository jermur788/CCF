// CCF Multi 2D — Map-First Menu & Lenses Upgrade real-browser acceptance driver (Microsoft Edge over CDP).
'use strict';
import { spawn } from 'node:child_process';
import { mkdirSync, readFileSync, writeFileSync, rmSync } from 'node:fs';
import { join } from 'node:path';
import { tmpdir } from 'node:os';

const args = process.argv.slice(2);
const opt = (name, fallback = null) => {
  const i = args.indexOf('--' + name);
  return i >= 0 && i + 1 < args.length ? args[i + 1] : fallback;
};
const tag = opt('tag', 'vp');
const mode = opt('mode', 'main'); // 'main' | 'load'
const width = Number(opt('width', '1280'));
const height = Number(opt('height', '720'));
const slotName = opt('slot', 'mlui-v3');
const url = opt('url');
const out = opt('out', '.');
const vpLabel = `[${width}x${height}]`;
const EDGE = process.env.EDGE || 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';
const expectedPath = join(out, `${tag}-expected.json`);

let failures = 0, passes = 0;
const log = (line) => console.log(line);
function check(ok, title) {
  if (ok) { passes++; log(`PASS ${vpLabel} ${tag}/${mode}: ${title}`); }
  else { failures++; log(`FAIL ${vpLabel} ${tag}/${mode}: ${title}`); }
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
  const profile = join(tmpdir(), 'ccf-mlui-edge-' + process.pid + '-' + Date.now());
  mkdirSync(profile, { recursive: true });
  const child = spawn(EDGE, [
    '--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check', '--disable-extensions',
    '--hide-scrollbars', '--force-device-scale-factor=1', `--window-size=${width},${height}`,
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
  if (!target) {
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    throw new Error('Edge did not expose a page target');
  }
  const cdp = await Cdp.connect(target.webSocketDebuggerUrl);
  return { cdp, child, profile };
}

const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

async function setViewport(cdp, w, h) {
  await cdp.send('Emulation.setDeviceMetricsOverride', { width: w, height: h, deviceScaleFactor: 1, mobile: false });
  await sleep(180);
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
  const diag = await evaluate(cdp, `(() => ({
    busy: document.documentElement.classList.contains('busy'),
    pane: document.getElementById('side')?.getAttribute('data-pane'),
    alert: document.getElementById('alert')?.textContent?.slice(0, 220) || '',
    verdict: document.getElementById('economy-verdict')?.textContent?.slice(0, 180) || '',
    approveDisabled: document.getElementById('approve')?.disabled,
    approveHidden: document.getElementById('approve')?.hidden,
    approvedHidden: document.getElementById('approved-box')?.hidden,
    quoteBadge: document.getElementById('economy-quote-badge')?.textContent || '',
    explainOpen: !document.getElementById('explain-popover')?.hidden,
  }))()`).catch(() => null);
  throw new Error('timed out waiting for ' + what + ' | diag=' + JSON.stringify(diag));
}

async function rectOf(cdp, selector) {
  return evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (!e) return null;
    const r = e.getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2,
    left: r.left, top: r.top, right: r.right, bottom: r.bottom, w: r.width, h: r.height,
    visible: r.width > 0 && r.height > 0 && getComputedStyle(e).visibility !== 'hidden' }; })()`);
}

async function moveMouse(cdp, x, y) {
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', x, y, button: 'none', pointerType: 'mouse' });
  await sleep(120);
}

async function click(cdp, selector) {
  await waitFor(cdp, "!document.documentElement.classList.contains('busy')", 'page idle before clicking ' + selector, 10000);
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (e && e.scrollIntoView) e.scrollIntoView({ block: 'center', inline: 'nearest' }); })()`);
  await sleep(80);
  const rect = await rectOf(cdp, selector);
  if (!rect || !rect.visible) throw new Error('cannot click ' + selector + ' (not visible)');
  const base = { x: rect.x, y: rect.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(150);
}

async function pressKey(cdp, key) {
  const vkMap = { Escape: 27, Enter: 13, '[': 219, ']': 221, '?': 191 };
  const vk = vkMap[key] || (key.length === 1 ? key.toUpperCase().charCodeAt(0) : 0);
  const modifiers = key === '?' ? 8 : 0; // Shift for '?'
  const params = {
    key,
    code: key === 'Enter' ? 'Enter' : (key === 'Escape' ? 'Escape' : undefined),
    text: key === 'Enter' ? '\r' : (key.length === 1 ? key : undefined),
    windowsVirtualKeyCode: vk,
    nativeVirtualKeyCode: vk,
    modifiers,
  };
  await cdp.send('Input.dispatchKeyEvent', { type: 'keyDown', ...params });
  await cdp.send('Input.dispatchKeyEvent', { type: 'keyUp', ...params });
  await sleep(160);
}

async function typeInto(cdp, selector, text) {
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); e.focus(); e.value = ${JSON.stringify(text)}; e.dispatchEvent(new Event('input', { bubbles: true })); })()`);
  await sleep(80);
}

async function shot(cdp, name) {
  await sleep(140);
  const data = await cdp.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
  const file = join(out, `${tag}-${mode}-${name}-${width}x${height}.png`);
  writeFileSync(file, Buffer.from(data.data, 'base64'));
  log('SHOT ' + file);
}

async function fetchJson(cdp, path, body) {
  const expression = `(async () => { const r = await fetch(${JSON.stringify(path)}, ${body === undefined
    ? `{ method: 'GET', cache: 'no-store', credentials: 'omit' }`
    : `{ method: 'POST', cache: 'no-store', credentials: 'omit', headers: { 'Content-Type': 'application/json' }, body: ${JSON.stringify(JSON.stringify(body))} }`});
    const raw = await r.text();
    let p = null; try { p = JSON.parse(raw); } catch {}
    return { status: r.status, bytes: new TextEncoder().encode(raw).length, payload: p }; })()`;
  return evaluate(cdp, expression);
}

function fenceOf(view) {
  return { sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision };
}

const deliberateRefusal = (entry) => entry.source === 'network' && /status of (400|409)/.test(entry.text || '');

function makeAuditor(cdp) {
  const badNetwork = [];
  const consoleIssues = [];
  const cspViolations = [];
  let initialAssetOkCount = 0;

  const collect = (isInitial = false) => {
    for (const e of cdp.events) {
      if (e.method === 'Runtime.exceptionThrown') consoleIssues.push(e.params.exceptionDetails?.text || 'exception');
      if (e.method === 'Runtime.consoleAPICalled' && e.params.type === 'error') consoleIssues.push(JSON.stringify(e.params.args));
      if (e.method === 'Log.entryAdded' && e.params.entry.level === 'error') {
        const txt = e.params.entry.text || '';
        if (/Content Security Policy/i.test(txt)) cspViolations.push(txt);
        else if (!deliberateRefusal(e.params.entry)) consoleIssues.push(txt);
      }
      if (e.method === 'Network.responseReceived') {
        const st = e.params.response?.status;
        if (st === 404 || st >= 500) badNetwork.push(`${st} ${e.params.response?.url}`);
        else if (isInitial && st === 200) initialAssetOkCount++;
      }
      if (e.method === 'Network.loadingFailed' && !e.params.canceled) {
        badNetwork.push(`LOAD_FAIL ${e.params.errorText || 'unknown'}`);
      }
    }
    cdp.events.length = 0;
  };
  return { collect, badNetwork, consoleIssues, cspViolations, getInitialOk: () => initialAssetOkCount };
}

async function runMainPhase(cdp) {
  const audit = makeAuditor(cdp);
  audit.collect(true);
  check(audit.getInitialOk() >= 18 && audit.badNetwork.length === 0 && audit.consoleIssues.length === 0 && audit.cspViolations.length === 0,
    `MLUI-12 Initial page load clean (${audit.getInitialOk()} HTTP 200 assets, console=${audit.consoleIssues.length}, csp=${audit.cspViolations.length}, net=${audit.badNetwork.length})`);

  await setViewport(cdp, width, height);

  // 1. MLUI-02 & MLUI-09: 2-column layout, reclaimed map width (~21-26%), compact legend + floating expand toggle, zero h-scroll
  const layout0 = await evaluate(cdp, `(() => {
    const ws = document.getElementById('workspace');
    const cols = getComputedStyle(ws).gridTemplateColumns.split(' ').filter(Boolean).length;
    const mapW = Math.round(document.getElementById('map-pane').getBoundingClientRect().width);
    const noHScroll = document.documentElement.scrollWidth <= window.innerWidth + 1;
    const legendHidden = document.getElementById('legend-panel').hidden;
    const compactChips = document.querySelectorAll('#legend-compact-items .legend-chip').length;
    const legendLabel = document.getElementById('legend-context-label').textContent.trim();
    const topActions = Array.from(document.querySelectorAll('.top-actions > button'));
    const tops = topActions.map((b) => Math.round(b.getBoundingClientRect().top));
    const topDelta = tops.length ? Math.max(...tops) - Math.min(...tops) : 0;
    return { cols, mapW, noHScroll, scrollW: document.documentElement.scrollWidth, innerW: window.innerWidth,
      legendHidden, compactChips, legendLabel, topDelta };
  })()`);
  const baselineMapW = width >= 1600 ? 1226 : 738;
  const reclaimedPct = ((layout0.mapW - baselineMapW) / baselineMapW) * 100;
  check(layout0.cols === 2 && layout0.noHScroll && layout0.legendHidden && layout0.compactChips >= 5 &&
    layout0.topDelta <= 10 && reclaimedPct >= 19 && reclaimedPct <= 30,
    `MLUI-02 Compact contextual legend + 2-column grid reclaims ${reclaimedPct.toFixed(1)}% map width (${baselineMapW}px -> ${layout0.mapW}px, cols=${layout0.cols}, chips=${layout0.compactChips}, noHScroll=${layout0.noHScroll})`);

  // Expand full legend -> floating popover (16 symbols) without changing column count or map width -> Esc collapses it
  await click(cdp, '#legend-toggle');
  await waitFor(cdp, "!document.getElementById('legend-panel').hidden", 'Full legend expanded');
  const legendExp = await evaluate(cdp, `({
    cols: getComputedStyle(document.getElementById('workspace')).gridTemplateColumns.split(' ').filter(Boolean).length,
    mapW: Math.round(document.getElementById('map-pane').getBoundingClientRect().width),
    items: document.querySelectorAll('#legend > li').length,
    ariaExp: document.getElementById('legend-toggle').getAttribute('aria-expanded'),
  })`);
  check(legendExp.cols === 2 && legendExp.mapW === layout0.mapW && legendExp.items === 16 && legendExp.ariaExp === 'true',
    `MLUI-02 Expand full legend opens 16-symbol floating popover without reducing map width (${legendExp.mapW}px, cols=${legendExp.cols})`);
  await pressKey(cdp, 'Escape');
  await waitFor(cdp, "document.getElementById('legend-panel').hidden", 'Esc collapsed full legend');
  check(true, 'MLUI-02 Esc collapses expanded full legend cleanly');

  // 2. MLUI-09: Select a living tree -> verify reference tree + competitor ranks 1-5 + Add remain above the fold at 1280x720
  const initView = (await fetchJson(cdp, 'api/multi/view')).payload.view;
  const firstLiving = initView.areas[0].trees.find((t) => t.isLiving);
  await typeInto(cdp, '#quick-id', firstLiving.treeId);
  await pressKey(cdp, 'Enter');
  await waitFor(cdp, `document.getElementById('insp-id')?.textContent.trim() === ${JSON.stringify(firstLiving.treeId)} && document.querySelectorAll('.comp-row').length === 5`,
    'Reference tree and 5 competitors loaded');
  await sleep(160);

  const foldCheck = await evaluate(cdp, `(() => {
    const pane = document.getElementById('pane-tree');
    const hRect = document.getElementById('inspector-heading').getBoundingClientRect();
    const rows = Array.from(document.querySelectorAll('.comp-row')).map((r) => Math.round(r.getBoundingClientRect().bottom));
    const addRect = document.getElementById('add-button').getBoundingClientRect();
    return {
      scrollTop: pane.scrollTop,
      headingTop: Math.round(hRect.top),
      maxRowBottom: rows.length ? Math.max(...rows) : 9999,
      addBottom: Math.round(addRect.bottom),
      vh: window.innerHeight,
      rowCount: rows.length,
    };
  })()`);
  check(foldCheck.scrollTop === 0 && foldCheck.rowCount === 5 && foldCheck.addBottom <= foldCheck.vh,
    `MLUI-09 Above-the-fold marking gate: ref + 5 competitors + Add fit without scrolling (addBottom=${foldCheck.addBottom}px <= ${foldCheck.vh}px)`);

  // Tick competitor #1 into Fell draft, mark Crop Tree, and zoom in once to verify preservation across lenses/tabs/History/scroll
  await click(cdp, '.comp-row input[type="checkbox"]');
  await waitFor(cdp, "document.querySelectorAll('.comp-row.chosen').length === 1", 'Draft competitor #1 ticked');
  await click(cdp, '#crop-button');
  await waitFor(cdp, "document.getElementById('crop-button').textContent.toLowerCase().includes('remove')", 'Crop Tree marked');
  await click(cdp, '#zoom-in');
  await waitFor(cdp, "document.getElementById('zoom-state').textContent.trim() !== 'Fit'", 'Map zoomed in');
  await shot(cdp, 'woodland-lens-selected-fold');

  // 3. MLUI-01 & MLUI-10: Switch across all 5 lenses -> verify presentation/overlay/tool separation, safe Select Tree default, zero mutation, state preserved, Part 2 slots
  const beforeLensState = await evaluate(cdp, `({
    selected: document.getElementById('insp-id').textContent.trim(),
    chosen: document.querySelectorAll('.comp-row.chosen').length,
    zoom: document.getElementById('zoom-state').textContent.trim(),
    viewBox: document.getElementById('map').getAttribute('viewBox'),
  })`);
  const fenceBeforeLenses = fenceOf((await fetchJson(cdp, 'api/multi/view')).payload.view);

  // Switch to Trees (2)
  await click(cdp, '[data-lens-btn="trees"]');
  const treesState = await evaluate(cdp, `({
    lens: document.getElementById('workspace').getAttribute('data-lens'),
    tool: document.querySelector('.mode-btn[data-map-mode][aria-pressed="true"]')?.getAttribute('data-map-mode'),
    overlay: document.getElementById('overlay-select').value,
    plantHidden: document.getElementById('mode-plant').hidden,
    slots: Array.from(document.querySelectorAll('#lens-slots-list [data-extension-slot]')).map((n) => n.getAttribute('data-extension-slot')),
  })`);
  check(treesState.lens === 'trees' && treesState.tool === 'tree' && treesState.overlay === 'off' && treesState.plantHidden &&
    treesState.slots.includes('scots-pine-profile') && treesState.slots.includes('birch-alder-profiles') && treesState.slots.includes('real-woodland-measurements'),
    `MLUI-01/10 Trees lens: safe Select Tree default, Plant hidden, 3 Part 2 slots (${treesState.slots.join(',')})`);

  // Switch to Regeneration (3) -> MUST apply light overlay, show Plant & Spot Clear, and NEVER auto-activate Plant!
  await click(cdp, '[data-lens-btn="regeneration"]');
  const regenState = await evaluate(cdp, `({
    lens: document.getElementById('workspace').getAttribute('data-lens'),
    tool: document.querySelector('.mode-btn[data-map-mode][aria-pressed="true"]')?.getAttribute('data-map-mode'),
    overlay: document.getElementById('overlay-select').value,
    plantHidden: document.getElementById('mode-plant').hidden,
    spotHidden: document.getElementById('mode-spot').hidden,
    legendCtx: document.getElementById('legend-context-label').textContent.trim(),
    slots: Array.from(document.querySelectorAll('#lens-slots-list [data-extension-slot]')).map((n) => n.getAttribute('data-extension-slot')),
  })`);
  check(regenState.lens === 'regeneration' && regenState.tool === 'tree' && regenState.overlay === 'light' &&
    !regenState.plantHidden && !regenState.spotHidden && regenState.legendCtx.startsWith('Regeneration') &&
    regenState.slots.includes('soil-moisture-overlay') && regenState.slots.includes('jay-acorn-dispersal') && regenState.slots.includes('birch-alder-planting'),
    `MLUI-01/10 Regeneration lens: defaults to safe Select Tree (never auto-Plant), Light Guide overlay, Plant/Spot available, 3 Part 2 slots`);
  await shot(cdp, 'regeneration-lens');

  // Explicitly activate Plant tool in Regeneration, then switch to Harvesting (4) -> must reset to safe Select Tree & preserve Fell draft + selection + zoom!
  await click(cdp, '#mode-plant');
  await waitFor(cdp, "document.getElementById('mode-plant').getAttribute('aria-pressed') === 'true'", 'Plant tool explicitly activated');
  await click(cdp, '[data-lens-btn="harvesting"]');
  const harvState = await evaluate(cdp, `({
    lens: document.getElementById('workspace').getAttribute('data-lens'),
    tool: document.querySelector('.mode-btn[data-map-mode][aria-pressed="true"]')?.getAttribute('data-map-mode'),
    overlay: document.getElementById('overlay-select').value,
    plantHidden: document.getElementById('mode-plant').hidden,
    slots: Array.from(document.querySelectorAll('#lens-slots-list [data-extension-slot]')).map((n) => n.getAttribute('data-extension-slot')),
  })`);
  check(harvState.lens === 'harvesting' && harvState.tool === 'tree' && harvState.overlay === 'off' && harvState.plantHidden &&
    harvState.slots.includes('scots-pine-timber-grader') && harvState.slots.includes('windstorm-salvage'),
    `MLUI-01/10 Harvesting lens: resets Plant tool to safe Select Tree, hides Plant, 2 Part 2 slots`);

  // Switch to Habitat (5) -> undergrowth overlay, safe Select Tree, 3 Part 2 slots
  await click(cdp, '[data-lens-btn="habitat"]');
  const habState = await evaluate(cdp, `({
    lens: document.getElementById('workspace').getAttribute('data-lens'),
    tool: document.querySelector('.mode-btn[data-map-mode][aria-pressed="true"]')?.getAttribute('data-map-mode'),
    overlay: document.getElementById('overlay-select').value,
    slots: Array.from(document.querySelectorAll('#lens-slots-list [data-extension-slot]')).map((n) => n.getAttribute('data-extension-slot')),
  })`);
  check(habState.lens === 'habitat' && habState.tool === 'tree' && habState.overlay === 'undergrowth' &&
    habState.slots.includes('real-woodland-microhabitats') && habState.slots.includes('windstorm-deadwood-resilience') && habState.slots.includes('sensitive-wet-ground'),
    `MLUI-01/10 Habitat lens: undergrowth overlay, safe Select Tree, 3 Part 2 slots`);

  // Return to Woodland (1) and prove ZERO server mutation + selection, Fell draft, zoom, and viewBox preserved 100%
  await click(cdp, '[data-lens-btn="woodland"]');
  const afterLensState = await evaluate(cdp, `({
    selected: document.getElementById('insp-id').textContent.trim(),
    chosen: document.querySelectorAll('.comp-row.chosen').length,
    zoom: document.getElementById('zoom-state').textContent.trim(),
    viewBox: document.getElementById('map').getAttribute('viewBox'),
  })`);
  const fenceAfterLenses = fenceOf((await fetchJson(cdp, 'api/multi/view')).payload.view);
  check(JSON.stringify(beforeLensState) === JSON.stringify(afterLensState) &&
    JSON.stringify(fenceBeforeLenses) === JSON.stringify(fenceAfterLenses),
    `MLUI-01 Lens round-trip mutated ZERO server state and preserved selection, Fell draft (${afterLensState.chosen}), zoom (${afterLensState.zoom}), and viewBox`);

  // 4. MLUI-06: Single-level accessible pinned explanations (WCAG 2.2 SC 1.4.13: hover, hover-onto-popover, focus, pin, Esc dismiss, zero nested triggers)
  const expTriggerRect = await rectOf(cdp, '[data-explain="lenses-tools"]');
  await moveMouse(cdp, expTriggerRect.x, expTriggerRect.y);
  await waitFor(cdp, "!document.getElementById('explain-popover').hidden", 'Explanation revealed on hover');
  const popRect = await rectOf(cdp, '#explain-popover');
  await moveMouse(cdp, popRect.x, popRect.y);
  await sleep(250);
  const hoverableOk = await evaluate(cdp, "!document.getElementById('explain-popover').hidden && document.getElementById('explain-popover').getAttribute('data-pinned') === 'false'");
  await moveMouse(cdp, 10, 10);
  await sleep(280);
  const unhoverClosed = await evaluate(cdp, "document.getElementById('explain-popover').hidden");

  // Keyboard focus reveal + click pin + zero nested links + Esc dismiss preserves selection & drafts
  await evaluate(cdp, "document.querySelector('[data-explain=\"hegyi-ci\"]').focus()");
  await waitFor(cdp, "!document.getElementById('explain-popover').hidden && document.getElementById('explain-popover').getAttribute('data-explain-key') === 'hegyi-ci'",
    'Explanation revealed on keyboard focus');
  await click(cdp, '[data-explain="hegyi-ci"]');
  await moveMouse(cdp, 10, 10);
  await sleep(260);
  const pinnedState = await evaluate(cdp, `(() => {
    const p = document.getElementById('explain-popover');
    return {
      open: !p.hidden,
      pinned: p.getAttribute('data-pinned'),
      nestedTriggers: p.querySelectorAll('[data-explain], a[href]').length,
      bodyLen: document.getElementById('explain-body').textContent.trim().length,
    };
  })()`);
  await pressKey(cdp, 'Escape');
  await waitFor(cdp, "document.getElementById('explain-popover').hidden", 'Esc dismissed pinned explanation');
  const selAfterExplainEsc = await evaluate(cdp, `({
    selected: document.getElementById('insp-id').textContent.trim(),
    chosen: document.querySelectorAll('.comp-row.chosen').length,
  })`);
  check(hoverableOk && unhoverClosed && pinnedState.open && pinnedState.pinned === 'true' && pinnedState.nestedTriggers === 0 && pinnedState.bodyLen > 20 &&
    selAfterExplainEsc.selected === firstLiving.treeId && selAfterExplainEsc.chosen === 1,
    `MLUI-06 WCAG 2.2 SC 1.4.13 explanation: hoverable=${hoverableOk}, focus+pin=${pinnedState.pinned}, nested=${pinnedState.nestedTriggers}, Esc dismiss preserved selection & draft`);

  // 5. MLUI-08: Scroll-ownership isolation (wheel scrolling over #side never zooms or pans #map)
  const mapBeforeWheel = await evaluate(cdp, `({ viewBox: document.getElementById('map').getAttribute('viewBox'), zoom: document.getElementById('zoom-state').textContent.trim() })`);
  const sideRect = await rectOf(cdp, '#pane-tree');
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseWheel', x: sideRect.x, y: sideRect.y, deltaX: 0, deltaY: 240 });
  await sleep(160);
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseWheel', x: sideRect.x, y: sideRect.y, deltaX: 0, deltaY: -240 });
  await sleep(160);
  const mapAfterWheel = await evaluate(cdp, `({ viewBox: document.getElementById('map').getAttribute('viewBox'), zoom: document.getElementById('zoom-state').textContent.trim() })`);
  check(mapBeforeWheel.viewBox === mapAfterWheel.viewBox && mapBeforeWheel.zoom === mapAfterWheel.zoom,
    `MLUI-08 Scroll-ownership isolation: wheel over #side left map viewBox & zoom (${mapAfterWheel.zoom}) untouched`);

  // 6. MLUI-07 & MLUI-09: Safe hotkeys strictly guarded inside text input; work cleanly outside; selection preserved across History & Details
  await evaluate(cdp, "document.getElementById('quick-id').focus()");
  for (const k of ['2', '3', 'h', 'w', '?']) await pressKey(cdp, k);
  const guardedState = await evaluate(cdp, `({
    lens: document.getElementById('workspace').getAttribute('data-lens'),
    histHidden: document.getElementById('history-drawer').hidden,
    pane: document.getElementById('side').getAttribute('data-pane'),
    shortcutsOpen: document.getElementById('shortcuts-dialog').open,
  })`);
  check(guardedState.lens === 'woodland' && guardedState.histHidden && guardedState.pane === 'tree' && !guardedState.shortcutsOpen,
    `MLUI-07 Text-input hotkey guard blocked 2/3/h/w/? while #quick-id had focus`);

  // Blur input -> test hotkeys outside text input: 3 (Regeneration), 1 (Woodland), h (History open + Esc close), w (Work Plan), ? (Shortcuts help + Esc close)
  await evaluate(cdp, "document.getElementById('quick-id').blur(); document.body.focus();");
  await pressKey(cdp, '3');
  const hkLens3 = await evaluate(cdp, "document.getElementById('workspace').getAttribute('data-lens')");
  await pressKey(cdp, '1');
  await pressKey(cdp, 'h');
  await waitFor(cdp, "!document.getElementById('history-drawer').hidden", 'H hotkey opened History drawer');
  await pressKey(cdp, 'Escape');
  await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'Esc closed History drawer');
  await click(cdp, '[data-pane-button="details"]');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'details'", 'Details tab opened');
  await pressKey(cdp, 'w');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'plan'", 'W hotkey opened Work Plan tab');
  await pressKey(cdp, '?');
  await waitFor(cdp, "document.getElementById('shortcuts-dialog').open", '? hotkey opened Shortcuts dialog');
  const shortcutsText = await evaluate(cdp, "document.getElementById('shortcuts-dialog').textContent");
  await pressKey(cdp, 'Escape');
  await waitFor(cdp, "!document.getElementById('shortcuts-dialog').open", 'Esc closed Shortcuts dialog');

  // Verify selection & draft preserved after History + Details + Work Plan + Shortcuts
  await click(cdp, '[data-pane-button="tree"]');
  const preservedAfterNav = await evaluate(cdp, `({
    selected: document.getElementById('insp-id').textContent.trim(),
    chosen: document.querySelectorAll('.comp-row.chosen').length,
  })`);
  check(hkLens3 === 'regeneration' && shortcutsText.includes('zero unguarded keyboard shortcuts') &&
    preservedAfterNav.selected === firstLiving.treeId && preservedAfterNav.chosen === 1,
    `MLUI-07/09 Hotkeys (3/1/H/W/?/Esc) work outside inputs; zero unguarded approve/execute/reset; selection & draft preserved across History & Details`);

  // 7. MLUI-03 & MLUI-04: Start 5-lane 0.64 ha Forest Maker woodland with Money On -> verify adjacent lanes (3 @ 1920, 2 @ 1280), 1-lane precision toggle (M), mini-map range sync, Woodland Navigator
  await click(cdp, '#woodland-open');
  await waitFor(cdp, "document.getElementById('woodland-dialog').open && document.querySelector('input[name=\"woodland-choice\"]:checked') !== null", 'New woodland dialog ready');
  await evaluate(cdp, `(() => {
    const cb = document.getElementById('woodland-economy');
    if (!cb.checked) { cb.checked = true; cb.dispatchEvent(new Event('change', { bubbles: true })); }
  })()`);
  await click(cdp, '#woodland-create');
  await waitFor(cdp, "!document.getElementById('woodland-confirm-box').hidden", 'Unsaved progress confirm shown');
  await click(cdp, '#woodland-confirm-discard');
  await waitFor(cdp, "!document.getElementById('woodland-dialog').open && document.getElementById('economy-toggle-label').textContent.trim() === 'Money: On' && document.querySelectorAll('[data-lane-border]').length === 2",
    '5-lane Forest Maker woodland loaded with Money On');
  await sleep(250);

  // Switch to interior Lane C via mini-map -> 3 lanes at 1920x1080 (MA-B,MA-C,MA-D), 2 lanes at 1280x720 (MA-C,MA-D)
  await click(cdp, '[data-minimap-lane="MA-C"]');
  const expectedInterior = width >= 1440 ? 'MA-B,MA-C,MA-D' : 'MA-C,MA-D';
  await waitFor(cdp, `Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')).join(',') === ${JSON.stringify(expectedInterior)}`,
    `Interior Lane C shows expected adjacent lanes (${expectedInterior})`);
  const mmSync = await evaluate(cdp, `({
    borders: Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')).join(','),
    rangeFrame: document.querySelector('.minimap-range-frame')?.getAttribute('data-visible-range') || '',
    modeText: document.querySelector('.minimap-mode')?.textContent.trim() || '',
  })`);
  check(mmSync.borders === expectedInterior && mmSync.rangeFrame === expectedInterior,
    `MLUI-03 Adjacent-lanes default & mini-map range sync at ${width}x${height}: borders=${mmSync.borders}, minimapRange=${mmSync.rangeFrame} (${mmSync.modeText})`);
  await shot(cdp, 'multilane-adjacent-minimap');

  // Toggle 1-lane precision mode via 'M' hotkey -> 1 lane (MA-C), mini-map highlights MA-C -> toggle back via button
  await evaluate(cdp, "document.activeElement && document.activeElement.blur()");
  await pressKey(cdp, 'm');
  await waitFor(cdp, "Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')).join(',') === 'MA-C'",
    'M hotkey enabled 1-lane precision view');
  const oneLaneSync = await evaluate(cdp, `({
    rangeFrame: document.querySelector('.minimap-range-frame')?.getAttribute('data-visible-range') || '',
    modeText: document.querySelector('.minimap-mode')?.textContent.trim() || '',
    pressed: document.getElementById('lane-mode-toggle').getAttribute('aria-pressed'),
  })`);
  check(oneLaneSync.rangeFrame === 'MA-C' && oneLaneSync.modeText.includes('1 lane') && oneLaneSync.pressed === 'true',
    `MLUI-03 1-lane precision toggle (M) isolates MA-C and syncs mini-map (${JSON.stringify(oneLaneSync)})`);
  await click(cdp, '#lane-mode-toggle');
  await waitFor(cdp, `Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')).join(',') === ${JSON.stringify(expectedInterior)}`,
    '1-lane toggle off restored adjacent lanes');

  // Mark a tree in MA-C as Crop Tree -> test Collapsible Woodland Navigator + Prev/Next cycling + count scope
  const laneCData = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-C')).payload;
  const cropTarget = laneCData.view.areas[0].trees.find((t) => t.isLiving);
  await fetchJson(cdp, 'api/multi/crop/mark', { fence: fenceOf(laneCData.view), areaId: 'MA-C', treeId: cropTarget.treeId });
  await click(cdp, '#reread');
  await sleep(220);
  await click(cdp, '[data-pane-button="navigator"]');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'navigator' && document.getElementById('nav-crops-count').textContent.trim() === '1'",
    'Woodland Navigator tab loaded with marked crop tree');
  const navCheck = await evaluate(cdp, `({
    lanesCount: document.getElementById('nav-lanes-count').textContent.trim(),
    cropsCount: document.getElementById('nav-crops-count').textContent.trim(),
    cropsScopeKind: document.getElementById('nav-crops-scope').getAttribute('data-scope-kind'),
    cropsScopeText: document.getElementById('nav-crops-scope').textContent.trim(),
    hasStormSlot: Boolean(document.querySelector('[data-extension-slot="nav-storm-felled"]')),
    hasInspSlot: Boolean(document.querySelector('[data-extension-slot="nav-inspection-blockers"]')),
  })`);
  check(navCheck.lanesCount === '5' && navCheck.cropsCount === '1' && navCheck.cropsScopeKind === 'visible' &&
    navCheck.hasStormSlot && navCheck.hasInspSlot,
    `MLUI-04 Woodland Navigator shows 5 whole-woodland lanes, 1 visible-lane crop (${navCheck.cropsScopeText}), and Part 2 storm/inspection slots`);
  await shot(cdp, 'woodland-navigator');

  // Cycle next crop via Navigator Next button + cycle next lane via ']' hotkey
  await click(cdp, '[data-nav-cycle="crops"][data-nav-dir="1"]');
  await waitFor(cdp, `document.getElementById('insp-id')?.textContent.trim() === ${JSON.stringify(cropTarget.treeId)}`, 'Navigator Next crop selected crop tree');
  await evaluate(cdp, "document.activeElement && document.activeElement.blur()");
  await pressKey(cdp, ']');
  await waitFor(cdp, "document.querySelector('.minimap-lane.active')?.getAttribute('data-minimap-lane') === 'MA-D'", '] hotkey cycled to next lane MA-D');
  await pressKey(cdp, '[');
  await waitFor(cdp, "document.querySelector('.minimap-lane.active')?.getAttribute('data-minimap-lane') === 'MA-C'", '[ hotkey cycled back to MA-C');
  check(true, 'MLUI-04/07 Navigator Next crop cycling and [ / ] prev/next lane cycling work cleanly');

  // 8. MLUI-05 & MLUI-11: Queue 3 Fell stems (Sell/Keep/Leave) + 1 Clear (Landowner) -> Priority-Ranked Grouped Alerts -> Work Plan -> Approve -> Execute -> Advance -> Save v3
  const laneA0 = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-A')).payload;
  const deadwoodBefore = laneA0.stage4.areas[0].canopy.fallenDeadwoodCount;
  const liveTrees = laneA0.view.areas[0].trees.filter((t) => t.isLiving);
  const tSell = liveTrees[0].treeId;
  const tKeep = liveTrees[1].treeId;
  const tLeave = liveTrees[2].treeId;
  const cell0 = laneA0.stage4.areas[0].groundCompact[0];
  let curV = laneA0.view;
  for (const [tid, why] of [[tSell, 'sell stem'], [tKeep, 'keep for own use'], [tLeave, 'leave as deadwood']]) {
    const r = await fetchJson(cdp, 'api/multi/pending/add', { fence: fenceOf(curV), areaId: 'MA-A', treeId: tid, reason: why });
    curV = r.payload.view;
  }
  curV = (await fetchJson(cdp, 'api/multi/clear/add', { fence: fenceOf(curV), areaId: 'MA-A', column: cell0[0], row: cell0[1] })).payload.view;
  curV = (await fetchJson(cdp, 'api/multi/economy/executor', {
    fence: fenceOf(curV), kind: 'ClearUndergrowth', areaId: 'MA-A', targetKey: `${cell0[0]},${cell0[1]}`, executor: 'Landowner',
  })).payload.view;

  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('alert').classList.contains('shown') && document.getElementById('alert').getAttribute('data-top-tier') === 'attention'",
    'Attention Required alert displayed for unfinished work');

  // Verify benign search miss is Information tier (ranks BELOW Attention Required), then trigger a REAL Blocking
  // refusal via Skip to Review with unfinished work (WorkPlanOpen from untouched panels/time-history.js).
  await click(cdp, '[data-pane-button="tree"]');
  await typeInto(cdp, '#quick-id', 'NO-SUCH-TREE');
  await pressKey(cdp, 'Enter');
  await waitFor(cdp, "document.getElementById('alert').getAttribute('data-top-tier') === 'attention' && document.getElementById('alerts-expand') !== null",
    'Search miss ranked below Attention Required as info tier');
  await click(cdp, '#skip-review');
  await waitFor(cdp, "document.getElementById('alert').getAttribute('data-top-tier') === 'blocking' && document.getElementById('alert').textContent.includes('WorkPlanOpen') && document.getElementById('alerts-expand') !== null",
    'Blocking WorkPlanOpen refusal ranks above Attention Required with grouped alert expand button');
  await sleep(80);
  // Keyboard Enter on #alerts-expand must expand AND keep focus on #alerts-expand (never drop to <body>).
  await evaluate(cdp, "document.getElementById('alerts-expand').focus()");
  await pressKey(cdp, 'Enter');
  await waitFor(cdp, "document.getElementById('alerts-expand')?.getAttribute('aria-expanded') === 'true'", 'Enter expanded grouped alerts');
  const alertRankCheck = await evaluate(cdp, `({
    topTier: document.getElementById('alert').getAttribute('data-top-tier'),
    tiersInOrder: Array.from(document.querySelectorAll('#alert [data-alert-tier]')).map((n) => n.getAttribute('data-alert-tier')),
    blockingRole: document.querySelector('#alert [data-alert-tier="blocking"]')?.getAttribute('role') || '',
    announced: document.getElementById('announce')?.textContent || '',
    expandFocusId: document.activeElement?.id || '',
    hasOpenModal: Boolean(document.querySelector('dialog[open]')),
    hasHistoryLink: Boolean(document.querySelector('#alert [data-alert-action="history"]')),
  })`);
  await shot(cdp, 'priority-ranked-alerts');

  // Verify caller-supplied button listener inside .alert-custom-parts works (M-1: live node, never cloned).
  await click(cdp, '#alert .alert-custom-parts button');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'plan' && document.querySelectorAll('[data-disposition-value]').length === 9",
    'Live caller Open Work Plan button in Skip refusal opened Work Plan');

  // Switch back to Tree tab, dismiss blocking alert -> Attention Required surfaces & focus stays sensible -> click its direct 'Open Work Plan' link.
  await click(cdp, '[data-pane-button="tree"]');
  await click(cdp, '#alert [data-alert-action="dismiss"]');
  await waitFor(cdp, "document.getElementById('alert').getAttribute('data-top-tier') === 'attention'", 'Attention Required surfaced after dismissing blocking notice');
  const postDismissFocusTag = await evaluate(cdp, "document.activeElement ? (document.activeElement.tagName + '#' + (document.activeElement.id || '')) : ''");
  await click(cdp, '#alert [data-alert-action="plan"]');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'plan' && document.querySelectorAll('[data-disposition-value]').length === 9",
    'Attention Required direct Open Work Plan link opened Work Plan with 3-way felling controls');
  check(alertRankCheck.topTier === 'blocking' && alertRankCheck.tiersInOrder[0] === 'blocking' &&
    alertRankCheck.tiersInOrder.includes('attention') && alertRankCheck.blockingRole === 'alert' &&
    alertRankCheck.announced.includes('WorkPlanOpen') && alertRankCheck.expandFocusId === 'alerts-expand' &&
    !alertRankCheck.hasOpenModal && alertRankCheck.hasHistoryLink && !postDismissFocusTag.startsWith('BODY'),
    `MLUI-05 Priority-ranked alerts + live caller button + role=alert + focus preserved: order=[${alertRankCheck.tiersInOrder.join(' -> ')}], expandFocus=${alertRankCheck.expandFocusId}, dismissFocus=${postDismissFocusTag}`);

  // Apply 3-way felling dispositions (KeepForUse + LeaveAsDeadwood) -> Approve -> Execute -> Advance 1 Year
  const keepSel = `[data-disposition-area="MA-A"][data-disposition-tree="${tKeep}"][data-disposition-value="KeepForUse"]`;
  const leaveSel = `[data-disposition-area="MA-A"][data-disposition-tree="${tLeave}"][data-disposition-value="LeaveAsDeadwood"]`;
  await click(cdp, keepSel);
  await waitFor(cdp, `document.querySelector('${keepSel}')?.getAttribute('aria-pressed') === 'true' && !document.documentElement.classList.contains('busy')`, 'KeepForUse applied');
  await click(cdp, leaveSel);
  await waitFor(cdp, `document.querySelector('${leaveSel}')?.getAttribute('aria-pressed') === 'true' && !document.documentElement.classList.contains('busy') && !document.getElementById('approve').disabled`, 'LeaveAsDeadwood applied and Approve ready');
  await sleep(120);

  await click(cdp, '#approve');
  await waitFor(cdp, "!document.getElementById('approved-box').hidden && document.getElementById('economy-quote-badge').textContent.includes('Frozen')",
    'Work Plan approved with frozen quote');
  await click(cdp, '#execute');
  await waitFor(cdp, "document.getElementById('execute-dialog').open", 'Execute confirmation dialog opened');
  await click(cdp, '#execute-confirm');
  await waitFor(cdp, "document.getElementById('approved-box').hidden && document.getElementById('receipt').textContent.includes('Retained timber added')",
    'Execute committed with retained timber and fallen deadwood');
  await sleep(220);

  const econAfterExec = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  const laneAAfterExec = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-A')).payload;
  const addedDw = laneAAfterExec.stage4.areas[0].canopy.fallenDeadwood.find((d) => d.treeId === tLeave);
  check(econAfterExec.retainedTimber.retainedTimberCm3 > 0 && Boolean(addedDw) && addedDw.cause === 'fell',
    `MLUI-11 3-way felling executed: retained=${econAfterExec.retainedTimber.volumeDisplay}, deadwood=${addedDw?.deadwoodId}`);

  // Advance 1 Year -> Year 0 -> 1
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'Advanced to Year 1');

  // Save v3 slot via UI
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open", 'Saved games dialog opened');
  await typeInto(cdp, '#save-slot', slotName);
  await click(cdp, '#save-do');
  await waitFor(cdp, `document.getElementById('saves-status').textContent.includes(${JSON.stringify('Saved "' + slotName + '"')})`,
    `Save v3 slot ${slotName} committed`);
  await click(cdp, '#saves-close');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open", 'Saved games dialog closed');

  writeFileSync(expectedPath, JSON.stringify({
    cashBalanceCents: econAfterExec.cashBalanceCents,
    cashBalanceDisplay: econAfterExec.cashBalanceDisplay,
    retainedTimberCm3: econAfterExec.retainedTimber.retainedTimberCm3,
    retainedTimberDisplay: econAfterExec.retainedTimber.volumeDisplay,
    deadwoodCount: laneAAfterExec.stage4.areas[0].canopy.fallenDeadwoodCount,
    deadwoodId: addedDw.deadwoodId,
    deadwoodTreeId: tLeave,
  }, null, 2));

  // Final layout check: zero horizontal scroll, 2 columns
  const endLayout = await evaluate(cdp, `({
    noHScroll: document.documentElement.scrollWidth <= window.innerWidth + 1,
    cols: getComputedStyle(document.getElementById('workspace')).gridTemplateColumns.split(' ').filter(Boolean).length,
  })`);
  check(endLayout.noHScroll && endLayout.cols === 2, `MLUI-09 Final layout clean (noHScroll=${endLayout.noHScroll}, cols=${endLayout.cols})`);

  audit.collect();
  check(audit.consoleIssues.length === 0 && audit.cspViolations.length === 0 && audit.badNetwork.length === 0,
    `MLUI-12 Main phase zero console/CSP/404/5xx (console=${audit.consoleIssues.length}, csp=${audit.cspViolations.length}, net=${audit.badNetwork.length})`);
}

async function runLoadPhase(cdp) {
  const expected = JSON.parse(readFileSync(expectedPath, 'utf8'));
  const audit = makeAuditor(cdp);
  audit.collect(true);
  await setViewport(cdp, width, height);

  await click(cdp, '#saves-open');
  await waitFor(cdp, `document.getElementById('saves-dialog').open && document.querySelector('[data-load="${slotName}"]') !== null`,
    `Fresh server lists Save v3 slot ${slotName}`);
  await click(cdp, `[data-load="${slotName}"]`);
  await waitFor(cdp, "!document.getElementById('load-confirm').hidden", 'Load confirmation shown');
  await click(cdp, '#load-do');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open && document.getElementById('year-value').textContent.trim() === '1' && document.getElementById('economy-toggle-label').textContent.trim() === 'Money: On'",
    'Save v3 restored after server restart');
  await sleep(250);

  const restoredEcon = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  const restoredLaneA = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-A')).payload;
  const restoredDw = restoredLaneA.stage4.areas[0].canopy.fallenDeadwood.find((d) => d.treeId === expected.deadwoodTreeId);
  const infoTier = await evaluate(cdp, "document.getElementById('alert').getAttribute('data-top-tier') || ''");
  check(restoredEcon.cashBalanceCents === expected.cashBalanceCents &&
    restoredEcon.retainedTimber.retainedTimberCm3 === expected.retainedTimberCm3 &&
    Boolean(restoredDw) && restoredDw.deadwoodId === expected.deadwoodId &&
    (infoTier === 'info' || infoTier === 'event'),
    `MLUI-11/05 Save v3 round-trip intact after restart (cash=${restoredEcon.cashBalanceDisplay}, retained=${restoredEcon.retainedTimber.volumeDisplay}, dw=${restoredDw?.deadwoodId}, topAlertTier=${infoTier})`);

  audit.collect();
  check(audit.consoleIssues.length === 0 && audit.cspViolations.length === 0 && audit.badNetwork.length === 0,
    `MLUI-12 Load phase zero console/CSP/404/5xx (console=${audit.consoleIssues.length}, csp=${audit.cspViolations.length}, net=${audit.badNetwork.length})`);
}

async function main() {
  const { cdp, child, profile } = await launch();
  try {
    await cdp.send('Page.enable');
    await cdp.send('Runtime.enable');
    await cdp.send('Log.enable');
    await cdp.send('Network.enable');
    await setViewport(cdp, width, height);
    await cdp.send('Page.navigate', { url });
    await waitFor(cdp, "document.getElementById('year-value') && document.getElementById('year-value').textContent.trim() !== '–'",
      'the /multi page to render', 30000);
    await sleep(280);
    if (mode === 'load') await runLoadPhase(cdp);
    else await runMainPhase(cdp);
  } finally {
    cdp.close();
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    try { rmSync(profile, { recursive: true, force: true }); } catch { /* ignore */ }
  }
  log(`SUMMARY ${vpLabel} ${tag}/${mode}: ${passes} passed, ${failures} failed`);
  process.exit(failures === 0 ? 0 : 1);
}

main().catch((err) => {
  console.error('FATAL', err);
  process.exit(1);
});
