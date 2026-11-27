// CCF Multi 2D — Phase 2A-2 Forest Maker UI, Multi-Lane View, C-FENCE & Save v3 real-browser acceptance driver (Edge CDP).
'use strict';
import { spawn } from 'node:child_process';
import { mkdirSync, writeFileSync, rmSync } from 'node:fs';
import { join } from 'node:path';
import { tmpdir } from 'node:os';

const args = process.argv.slice(2);
const opt = (name, fallback = null) => {
  const i = args.indexOf('--' + name);
  return i >= 0 && i + 1 < args.length ? args[i + 1] : fallback;
};
const mode = opt('mode', 'full'); // 'full' | 'restart-load' | 'overflow-503' | 'clamp-raise'
const tag = opt('tag', mode);
const width = Number(opt('width', '1920'));
const height = Number(opt('height', '1080'));
const url = opt('url');
const out = opt('out', '.');
const vpLabel = `[${width}x${height}]`;
const EDGE = process.env.EDGE || 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';

let failures = 0, passes = 0;
const log = (line) => console.log(line);
function check(ok, title) {
  if (ok) { passes++; log(`PASS ${vpLabel} ${tag}: ${title}`); }
  else { failures++; log(`FAIL ${vpLabel} ${tag}: ${title}`); }
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
  const profile = join(tmpdir(), 'ccf-p2a-edge-' + process.pid + '-' + Date.now());
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
  await evaluate(cdp, `(() => {
    const e = document.querySelector(${JSON.stringify(selector)});
    if (!e || !e.scrollIntoView) return;
    e.scrollIntoView({ block: 'center', inline: 'nearest' });
    const r = e.getBoundingClientRect();
    const topEl = document.elementFromPoint(r.x + r.width / 2, r.y + r.height / 2);
    if (topEl && topEl !== e && !e.contains(topEl)) {
      e.scrollIntoView({ block: 'start', inline: 'nearest' });
    }
  })()`);
  await sleep(80);
  const rect = await rectOf(cdp, selector);
  if (!rect || !rect.visible) throw new Error('cannot click ' + selector + ' (not visible)');
  const base = { x: rect.x, y: rect.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(160);
}

async function pressKey(cdp, key) {
  const vk = key === 'Escape' ? 27 : key === 'Enter' ? 13 : 0;
  const params = vk ? { key, code: key, windowsVirtualKeyCode: vk, nativeVirtualKeyCode: vk } : { key, code: key };
  await cdp.send('Input.dispatchKeyEvent', { type: 'rawKeyDown', ...params });
  await cdp.send('Input.dispatchKeyEvent', { type: 'keyUp', ...params });
  await sleep(160);
}

async function shot(cdp, name, w = width, h = height) {
  await setViewport(cdp, w, h);
  await sleep(160);
  const data = await cdp.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
  const file = join(out, `${tag}-${name}-${w}x${h}.png`);
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

async function checkLayout(cdp, label, expectMulti) {
  const geom = await evaluate(cdp, `(() => {
    const noHScroll = document.documentElement.scrollWidth <= window.innerWidth + 1;
    const actions = Array.from(document.querySelectorAll('.top-actions > button'));
    const rects = actions.map((b) => { const r = b.getBoundingClientRect(); return { id: b.id, top: Math.round(r.top), right: Math.round(r.right) }; });
    const maxTopDelta = rects.length ? Math.max(...rects.map((r) => r.top)) - Math.min(...rects.map((r) => r.top)) : 0;
    const allInWidth = rects.every((r) => r.right <= window.innerWidth + 1);
    const woodlandIdx = actions.findIndex((b) => b.id === 'woodland-open');
    const savesIdx = actions.findIndex((b) => b.id === 'saves-open');
    const badge = document.getElementById('experiment-badge');
    const badgeRect = badge ? badge.getBoundingClientRect() : null;
    const badgeUnclipped = Boolean(badge && badgeRect && badgeRect.width > 0 && badge.scrollWidth <= badge.clientWidth + 1 && badgeRect.right <= window.innerWidth + 1);
    const minimapHidden = document.getElementById('woodland-minimap').hidden;
    const laneToggleHidden = document.getElementById('lane-mode-toggle').hidden;
    const advSub = document.getElementById('advance-sub').textContent.trim();
    return { noHScroll, scrollW: document.documentElement.scrollWidth, innerW: window.innerWidth,
      maxTopDelta, allInWidth, besideSaves: Math.abs(woodlandIdx - savesIdx) === 1, badgeUnclipped,
      minimapHidden, laneToggleHidden, advSub };
  })()`);
  const multiOk = expectMulti
    ? (!geom.minimapHidden && !geom.laneToggleHidden && geom.advSub.includes('All lanes'))
    : (geom.minimapHidden && geom.laneToggleHidden && geom.advSub.includes('Both areas'));
  check(geom.noHScroll && geom.maxTopDelta <= 10 && geom.allInWidth && geom.besideSaves && geom.badgeUnclipped && multiOk,
    `layout (${label}): zero h-scroll (${geom.scrollW}<=${geom.innerW}), single-row topbar (delta=${geom.maxTopDelta}px), New woodland beside Saved games, multi=${expectMulti} (${geom.advSub})`);
}

async function runFull(cdp, collectEvents, auditErrors, netTracker) {
  await setViewport(cdp, width, height);
  await checkLayout(cdp, 'MA-1 initial', false);

  // 1. Accessible New Woodland modal: open, Esc closes, focus returns to #woodland-open
  await click(cdp, '#woodland-open');
  await waitFor(cdp, "document.getElementById('woodland-dialog').open && document.querySelectorAll('input[name=\"woodland-choice\"]').length >= 5",
    'New Woodland dialog open with all choices');
  await pressKey(cdp, 'Escape');
  await waitFor(cdp, "!document.getElementById('woodland-dialog').open", 'Esc closed New Woodland dialog');
  const focusAfterEsc = await evaluate(cdp, "document.activeElement && document.activeElement.id");
  check(focusAfterEsc === 'woodland-open', `Esc closes New Woodland dialog and returns focus to #woodland-open (got ${focusAfterEsc})`);

  // 2. Preview valid custom + Core refusal verbatim (> 3000 stems cap)
  const validPrev = await fetchJson(cdp, 'api/multi/woodland/preview', {
    recipe: { widthM: 80, lengthM: 80, laneCount: 5, startAgeYears: 20, stemsPerHa: 2100, seed: 6400101, speciesMix: 'SitkaPlusEdgeBroadleaves', oakPercent: 0, beechPercent: 0 },
  });
  check(validPrev.status === 200 && validPrev.payload?.valid === true && validPrev.payload?.preview?.totalStems === 1344 && validPrev.payload?.preview?.cellCount === 1024,
    `POST api/multi/woodland/preview returns Core-exact 1344 stems and 1024 cells`);

  const overCapPrev = await fetchJson(cdp, 'api/multi/woodland/preview', {
    recipe: { widthM: 120, lengthM: 120, laneCount: 6, startAgeYears: 20, stemsPerHa: 3000, seed: 101, speciesMix: 'PureSitka', oakPercent: 0, beechPercent: 0 },
  });
  check(overCapPrev.status === 409 && overCapPrev.payload?.valid === false && overCapPrev.payload?.refusal?.code === 'ForestMakerStemCapExceeded',
    `POST api/multi/woodland/preview refuses >3000 stems with Core 409 ForestMakerStemCapExceeded`);

  // 3. Unsaved progress warning + 409 WoodlandUnsavedProgress without confirmDiscardUnsaved + atomic swap + stale fence refusal
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'MA-1 advanced to Year 1');
  const dirtyView = (await fetchJson(cdp, 'api/multi/view')).payload.view;
  const dirtyFence = fenceOf(dirtyView);

  const unconfirmed = await fetchJson(cdp, 'api/multi/woodland/new', {
    fence: dirtyFence, choice: 'preset', presetId: 'fm-064-sitka-edge-broadleaves-v1', economyEnabled: false, confirmDiscardUnsaved: false,
  });
  check(unconfirmed.status === 409 && unconfirmed.payload?.refusal?.code === 'WoodlandUnsavedProgress' &&
    unconfirmed.payload?.view?.sessionId === dirtyView.sessionId && unconfirmed.payload?.view?.source?.ecologicalYear === 1,
    'Unconfirmed woodland replacement refuses 409 WoodlandUnsavedProgress and leaves MA-1 Year 1 untouched');

  // Open modal via UI, trigger unsaved confirmation banner, confirm discard -> starts 0.64 ha primary preset!
  await click(cdp, '#woodland-open');
  await waitFor(cdp, "document.getElementById('woodland-dialog').open && document.getElementById('woodland-preview').textContent.includes('1344')",
    'New Woodland modal live preview for 0.64 ha primary preset');
  await shot(cdp, 'woodland-modal');
  await click(cdp, '#woodland-create');
  await waitFor(cdp, "!document.getElementById('woodland-confirm-box').hidden && document.getElementById('woodland-confirm-text').textContent.includes('Discard unsaved progress')",
    'Unsaved-work confirmation banner shown');

  // Reset network tracker right before switching into >=3-area woodland so we audit ONLY player-page network traffic!
  collectEvents(false);
  netTracker.reset();

  await click(cdp, '#woodland-confirm-discard');
  await waitFor(cdp, "!document.getElementById('woodland-dialog').open && document.getElementById('year-value').textContent.trim() === '0' && document.querySelectorAll('[data-lane-border]').length === 2",
    'Primary 0.64 ha preset loaded at Year 0 with 2 visible lanes at outer Lane A');
  await sleep(250);

  // 4. Multi-lane view: 2 lanes at A (outer), 3 lanes at C (interior), 2 lanes at E (outer), open grassland, mini-map click + Enter, 1-lane toggle, neighbour click!
  await checkLayout(cdp, '0.64 ha Primary (Lane A)', true);
  await shot(cdp, 'primary-lane-a');

  const laneACheck = await evaluate(cdp, `({
    borders: Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')),
    hasGrassland: Boolean(document.querySelector('.grassland-exterior')),
    minimapLanes: document.querySelectorAll('[data-minimap-lane]').length,
    minimapStems: document.querySelectorAll('.minimap-stem').length,
    stripPills: document.querySelectorAll('[data-strip-lane]').length,
  })`);
  check(laneACheck.borders.join(',') === 'MA-A,MA-B' && laneACheck.hasGrassland && laneACheck.minimapLanes === 5 &&
    laneACheck.minimapStems === 1344 && laneACheck.stripPills === 5,
    `Lane A shows 2 visible lanes (MA-A,MA-B), open grassland, 5 mini-map lanes with 1344 stems, 5-lane strip (${JSON.stringify(laneACheck)})`);

  // Click Lane C on mini-map -> must show 3 lanes (MA-B, MA-C, MA-D)!
  await click(cdp, '[data-minimap-lane="MA-C"]');
  await waitFor(cdp, "document.querySelectorAll('[data-lane-border]').length === 3", 'Lane C shows 3 visible lanes');
  const laneCBorders = await evaluate(cdp, "Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')).join(',')");
  check(laneCBorders === 'MA-B,MA-C,MA-D', `Mini-map click to interior Lane C shows 3 lanes (${laneCBorders})`);
  await shot(cdp, 'primary-lane-c-3lanes');

  // Keyboard Enter on mini-map Lane E -> must show 2 lanes (MA-D, MA-E)!
  await evaluate(cdp, "document.querySelector('[data-minimap-lane=\"MA-E\"]').focus()");
  await pressKey(cdp, 'Enter');
  await waitFor(cdp, "Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')).join(',') === 'MA-D,MA-E'",
    'Keyboard Enter on mini-map switches to Lane E (2 outer lanes)');
  check(true, 'Keyboard Enter on mini-map Lane E switches to outer 2-lane view (MA-D,MA-E)');

  // 1-lane toggle -> shows only MA-E; toggle off -> back to MA-D,MA-E
  await click(cdp, '#lane-mode-toggle');
  await waitFor(cdp, "document.querySelectorAll('[data-lane-border]').length === 1", '1-lane toggle shows 1 lane');
  const oneLaneBorders = await evaluate(cdp, "Array.from(document.querySelectorAll('[data-lane-border]')).map((n) => n.getAttribute('data-lane-border')).join(',')");
  check(oneLaneBorders === 'MA-E', `1-lane toggle isolates active lane (${oneLaneBorders})`);
  await click(cdp, '#lane-mode-toggle');
  await waitFor(cdp, "document.querySelectorAll('[data-lane-border]').length === 2", '1-lane toggle off restores neighbours');

  // Click a tree in visible neighbour lane MA-D (while MA-E is active) -> selects tree in MA-D & shifts lane window to MA-C,MA-D,MA-E!
  await click(cdp, '[data-lane-crowns="MA-D"] [data-tree-id]');
  await waitFor(cdp, "document.getElementById('insp-id') && document.getElementById('insp-id').textContent.startsWith('MA-D-') && document.querySelectorAll('[data-lane-border]').length === 3",
    'Clicking a visible neighbour lane tree inspects it with its exact areaId and shifts window to MA-C,MA-D,MA-E');
  const pickedNeighbour = await evaluate(cdp, "document.getElementById('insp-id').textContent.trim()");
  check(pickedNeighbour.startsWith('MA-D-'), `Visible neighbour tree click routed with exact area ID (${pickedNeighbour})`);

  // Verify Work Plan / verb routes work against the active lane in the lane-filtered 5-lane view (Crop mark + unmark).
  await waitFor(cdp, "!document.documentElement.classList.contains('busy') && !document.getElementById('competition-content').textContent.includes('Reading competition')", 'idle before Crop mark');
  await click(cdp, '#crop-button');
  await waitFor(cdp, "!document.documentElement.classList.contains('busy') && document.getElementById('crop-button').textContent.toLowerCase().includes('remove') && !document.getElementById('competition-content').textContent.includes('Reading competition')", 'Crop mark committed on active lane MA-D');
  await click(cdp, '#crop-button');
  await waitFor(cdp, "!document.documentElement.classList.contains('busy') && !document.getElementById('crop-button').textContent.toLowerCase().includes('remove')", 'Crop unmark committed on active lane MA-D');
  check(true, `Work Plan / Crop verb succeeded against active lane MA-D (${pickedNeighbour}) under lane-filtered reads`);

  // Verify Re-read button on >=3-area stand uses lane-filtered reads, NOT /view!
  await click(cdp, '#reread');
  await sleep(250);

  // AUDIT PLAYER-PAGE NETWORK TRAFFIC during >=3-area session:
  // Must have requested /stand (overview) and /stand?lanes= for 2-lane (MA-A,MA-B & MA-D,MA-E), 3-lane (MA-B,MA-C,MA-D & MA-C,MA-D,MA-E),
  // and 1-lane (MA-E), with ZERO GET /api/multi/view requests and EVERY response <= 524,288 B!
  collectEvents(false);
  const playerSnap = netTracker.snapshot();
  const viewCalls = playerSnap.viewGets;
  const ovCalls = playerSnap.standOverviews;
  const laneCalls = playerSnap.standLanes;
  const laneQueries = laneCalls.map((r) => r.lanes);
  const hasLaneA2 = laneQueries.includes('MA-A,MA-B');
  const hasLaneC3 = laneQueries.includes('MA-B,MA-C,MA-D');
  const hasLaneE2 = laneQueries.includes('MA-D,MA-E');
  const hasLaneE1 = laneQueries.includes('MA-E');
  const hasLaneD3 = laneQueries.includes('MA-C,MA-D,MA-E');
  const maxOvBytes = ovCalls.length ? Math.max(...ovCalls.map((r) => r.bytes)) : 0;
  const max1LaneBytes = laneCalls.filter((r) => r.laneCount === 1).reduce((m, r) => Math.max(m, r.bytes), 0);
  const max2LaneBytes = laneCalls.filter((r) => r.laneCount === 2).reduce((m, r) => Math.max(m, r.bytes), 0);
  const max3LaneBytes = laneCalls.filter((r) => r.laneCount === 3).reduce((m, r) => Math.max(m, r.bytes), 0);
  const allUnderBudget = ovCalls.every((r) => r.status === 200 && r.bytes > 0 && r.bytes <= 524288) &&
    laneCalls.every((r) => r.status === 200 && r.bytes > 0 && r.bytes <= 524288);
  check(viewCalls.length === 0 && ovCalls.length >= 5 && laneCalls.length >= 5 &&
    hasLaneA2 && hasLaneC3 && hasLaneE2 && hasLaneE1 && hasLaneD3 && allUnderBudget,
    `Player >=3-area network audit: /view GETs=${viewCalls.length} (must be 0), /stand overviews=${ovCalls.length} (max=${maxOvBytes} B), /stand?lanes= calls=${laneCalls.length} (1-lane max=${max1LaneBytes} B, 2-lane max=${max2LaneBytes} B, 3-lane max=${max3LaneBytes} B; all <= 524288 B; lanes seen=[${[...new Set(laneQueries)].join(' | ')}])`);

  // Stale old MA-1 fence must now be refused
  const staleCheck = await fetchJson(cdp, 'api/multi/year/advance', { fence: dirtyFence, consentToCancelWork: false });
  check(staleCheck.status === 409 && staleCheck.payload?.refusal?.code === 'PlayerSessionMismatch',
    'Old pre-swap MA-1 fence is refused with 409 PlayerSessionMismatch');

  // 5. Direct API C-FENCE checks: POST form, stale-fence 409, unknown lane 409
  const standOv = await fetchJson(cdp, 'api/multi/stand');
  const stand3 = await fetchJson(cdp, 'api/multi/stand?lanes=MA-A,MA-B,MA-C');
  const standPost = await fetchJson(cdp, 'api/multi/stand', { fence: standOv.payload.fence, lanes: ['MA-B', 'MA-C', 'MA-D'] });
  const standStale = await fetchJson(cdp, 'api/multi/stand', { fence: dirtyFence, lanes: ['MA-A'] });
  const standUnknown = await fetchJson(cdp, 'api/multi/stand?lanes=MA-Z');
  const fenceEqual = JSON.stringify(standOv.payload?.fence) === JSON.stringify(standPost.payload?.fence);
  check(standOv.status === 200 && standOv.bytes < 524288 &&
    stand3.status === 200 && stand3.bytes < 524288 &&
    standPost.status === 200 && standPost.bytes < 524288 && fenceEqual &&
    standStale.status === 409 && standStale.payload?.refusal?.code === 'PlayerSessionMismatch' &&
    standUnknown.status === 409 && standUnknown.payload?.refusal?.code === 'MultiAreaUnknownArea',
    `C-FENCE stand reads: overview=${standOv.bytes} B, 3-lane GET=${stand3.bytes} B, 3-lane POST=${standPost.bytes} B (all < 524288 B), identical fence=${fenceEqual}, stale=409, unknown=409`);

  // 6. Advance primary 0.64 ha preset to Year 2 and save v3 slot 'fm-restart-test' for cross-process restart proof!
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'Primary preset Year 1');
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '2'", 'Primary preset Year 2');

  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open", 'Saves dialog opened for v3 save');
  await evaluate(cdp, "(() => { const i = document.getElementById('save-slot'); i.value = 'fm-restart-test'; i.dispatchEvent(new Event('input', { bubbles: true })); })()");
  await click(cdp, '#save-do');
  await waitFor(cdp, "document.getElementById('saves-status').textContent.includes('Saved \"fm-restart-test\"') && document.querySelector('#saves-rows .slot-ver') !== null", 'v3 slot fm-restart-test saved and listed');
  const v3RowTag = await evaluate(cdp, "document.querySelector('#saves-rows .slot-ver')?.textContent?.trim() || ''");
  check(v3RowTag === 'v3', `Saved 0.64 ha primary preset at Year 2 as Save v3 slot fm-restart-test (badge=${v3RowTag})`);
  await click(cdp, '#saves-close');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open", 'Saves dialog closed');

  // 7. Create secondary 0.64 ha pure Sitka preset with Money On at Year 0 -> verify economy strip + SaveEconomyRequiresV3
  const curV2 = (await fetchJson(cdp, 'api/multi/stand')).payload;
  const pureRes = await fetchJson(cdp, 'api/multi/woodland/new', {
    fence: curV2.fence, choice: 'preset', presetId: 'fm-064-pure-sitka-v1', economyEnabled: true, confirmDiscardUnsaved: false,
  });
  check(pureRes.status === 200 && pureRes.payload?.economyEnabled === true &&
    pureRes.payload?.view?.experiment?.experimentId === 'mad-fm-064-pure-sitka-v1',
    'Pure Sitka 0.64 ha preset created with Money On at Year 0 (and saved stand needed no discard confirm!)');
  const econSaveRef = await fetchJson(cdp, 'api/multi/save', {
    fence: fenceOf(pureRes.payload.view), slot: 'should-refuse', replaceExisting: false,
  });
  check(econSaveRef.status === 409 && econSaveRef.payload?.refusal?.code === 'SaveEconomyRequiresV3',
    'Economy-enabled Forest Maker stand refuses Save with 409 SaveEconomyRequiresV3');

  // 8. Switch back to MA-1 via New Woodland -> prove 2-area layout & /view path restored unchanged
  const backMa1 = await fetchJson(cdp, 'api/multi/woodland/new', {
    fence: fenceOf(pureRes.payload.view), choice: 'ma-1', economyEnabled: false, confirmDiscardUnsaved: true,
  });
  check(backMa1.status === 200 && backMa1.payload?.view?.areas?.length === 2, 'Switched cleanly back to 2-area MA-1');
  collectEvents(false);
  netTracker.reset();
  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('woodland-minimap').hidden && document.getElementById('advance-sub').textContent.includes('Both areas')",
    '2-area MA-1 layout restored');
  collectEvents(false);
  const ma1Snap = netTracker.snapshot();
  check(ma1Snap.viewGets.length >= 1, `2-area MA-1 re-read uses api/multi/view unchanged (viewGets=${ma1Snap.viewGets.length})`);
  await checkLayout(cdp, 'MA-1 restored', false);

  // Also test 1280x720 layout directly!
  await setViewport(cdp, 1280, 720);
  await checkLayout(cdp, 'MA-1 at 1280x720', false);
  await setViewport(cdp, width, height);

  auditErrors(false);
}

async function runRestartLoad(cdp, collectEvents, auditErrors, netTracker) {
  await setViewport(cdp, width, height);
  // Fresh process starts on MA-1 (2 areas, Year 0)
  const startExp = await evaluate(cdp, "document.getElementById('experiment-badge').textContent");
  check(startExp.includes('MA-1'), `Fresh restarted server started on MA-1 (${startExp})`);

  // Negative test (Review Condition 2 / Item 3): create unsaved progress (Year 1), then call POST api/multi/load
  // OMITTING confirmDiscardUnsaved -> MUST refuse 409 WoodlandUnsavedProgress and leave MA-1 Year 1 untouched!
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'MA-1 advanced to Year 1 for v3 load unsaved-progress guard test');
  const dirtyMa1 = (await fetchJson(cdp, 'api/multi/view')).payload.view;
  const dirtyFence = fenceOf(dirtyMa1);

  const loadOmitted = await fetchJson(cdp, 'api/multi/load', { fence: dirtyFence, slot: 'fm-restart-test' });
  check(loadOmitted.status === 409 && loadOmitted.payload?.refusal?.code === 'WoodlandUnsavedProgress' &&
    loadOmitted.payload?.view?.sessionId === dirtyMa1.sessionId && loadOmitted.payload?.view?.source?.ecologicalYear === 1,
    'POST api/multi/load omitting confirmDiscardUnsaved refuses 409 WoodlandUnsavedProgress when unsaved progress exists (session untouched)');

  const loadFalse = await fetchJson(cdp, 'api/multi/load', { fence: dirtyFence, slot: 'fm-restart-test', confirmDiscardUnsaved: false });
  check(loadFalse.status === 409 && loadFalse.payload?.refusal?.code === 'WoodlandUnsavedProgress' &&
    loadFalse.payload?.view?.sessionId === dirtyMa1.sessionId && loadFalse.payload?.view?.source?.ecologicalYear === 1,
    'POST api/multi/load with confirmDiscardUnsaved=false refuses 409 WoodlandUnsavedProgress (session untouched)');

  // Open Saved games -> v3 slot 'fm-restart-test' must be valid & canLoad=true even while on MA-1!
  // Shipped UI sends confirmDiscardUnsaved: true and renders restored 5-lane stand via C-FENCE lane reads!
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.querySelector('[data-load=\"fm-restart-test\"]') !== null", 'v3 slot fm-restart-test listed as loadable');
  await click(cdp, '[data-load="fm-restart-test"]');
  await waitFor(cdp, "!document.getElementById('load-confirm').hidden", 'Load confirmation shown');

  collectEvents(false);
  netTracker.reset();
  await click(cdp, '#load-do');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open && document.getElementById('year-value').textContent.trim() === '2' && document.querySelectorAll('[data-strip-lane]').length === 5",
    'Restored 0.64 ha 5-lane stand at Year 2 after genuine server restart');
  await sleep(220);

  collectEvents(false);
  const loadSnap = netTracker.snapshot();
  check(loadSnap.viewGets.length === 0 && loadSnap.standOverviews.length >= 1 && loadSnap.standLanes.length >= 1 &&
    loadSnap.standLanes.every((r) => r.status === 200 && r.bytes <= 524288),
    `Restored v3 5-lane page renders via /stand?lanes= (viewGets=${loadSnap.viewGets.length}, overviews=${loadSnap.standOverviews.length}, laneReads=${loadSnap.standLanes.length}, lanes=${loadSnap.standLanes.map((r) => r.lanes).join(',')})`);

  await checkLayout(cdp, 'Restored v3 at 1280x720', true);
  await shot(cdp, 'restarted-v3-loaded');

  // Continue advancing to Year 3 after v3 restore!
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '3'", 'Continued restored v3 stand to Year 3');
  check(true, 'Save v3 -> genuine process restart -> Load -> Continue to Year 3 succeeded cleanly');

  auditErrors(false);
}

async function runOverflow503(cdp, collectEvents, auditErrors) {
  await setViewport(cdp, width, height);
  // Process launched with CCF_TEST_MAX_VIEW_BYTES=400000 (tests LOWERING the active budget):
  // MA-1 (~325 KiB) loads fine as last good view!
  const beforeExp = await evaluate(cdp, "document.getElementById('experiment-badge').textContent.trim()");
  const beforeCrowns = await evaluate(cdp, "document.querySelectorAll('#crowns [data-tree-id]').length");
  check(beforeExp.includes('MA-1') && beforeCrowns > 0, `Last good view loaded under lowered 400000 B test budget (${beforeCrowns} crowns)`);

  // Direct API request to create 5-lane 0.64 ha preset (exceeds 400000 B) -> MUST return HTTP 503 MultiViewPayloadBudgetExceeded!
  const curV = (await fetchJson(cdp, 'api/multi/view')).payload.view;
  const staleMa1Fence = fenceOf(curV);
  const overRes = await fetchJson(cdp, 'api/multi/woodland/new', {
    fence: staleMa1Fence, choice: 'preset', presetId: 'fm-064-sitka-edge-broadleaves-v1', economyEnabled: false, confirmDiscardUnsaved: true,
  });
  check(overRes.status === 503 && overRes.payload?.code === 'MultiViewPayloadBudgetExceeded' &&
    typeof overRes.payload?.bytes === 'number' && overRes.payload?.budget === 400000 && overRes.payload.bytes > 400000,
    `Forced overflow (woodland/new) returned HTTP 503 MultiViewPayloadBudgetExceeded (bytes=${overRes.payload?.bytes}, budget=${overRes.payload?.budget})`);

  // Helper to get fresh 5-lane fence from overview (~78 KB < 400 KB)
  const freshFence = async () => (await fetchJson(cdp, 'api/multi/stand')).payload.fence;

  // Check EVERY view-returning route refuses with HTTP 503 (never an over-budget 200 or 409)!
  const overView = await fetchJson(cdp, 'api/multi/view');
  check(overView.status === 503 && overView.payload?.code === 'MultiViewPayloadBudgetExceeded' && overView.payload?.budget === 400000,
    `GET api/multi/view refuses over-budget with HTTP 503 (bytes=${overView.payload?.bytes}, budget=${overView.payload?.budget})`);

  const overAdvance = await fetchJson(cdp, 'api/multi/year/advance', { fence: await freshFence(), consentToCancelWork: false });
  check(overAdvance.status === 503 && overAdvance.payload?.code === 'MultiViewPayloadBudgetExceeded' && overAdvance.payload?.budget === 400000,
    `POST api/multi/year/advance refuses over-budget with HTTP 503 (bytes=${overAdvance.payload?.bytes}, budget=${overAdvance.payload?.budget})`);

  const overSkip = await fetchJson(cdp, 'api/multi/year/skip', { fence: await freshFence(), targetYear: 3 });
  check(overSkip.status === 503 && overSkip.payload?.code === 'MultiViewPayloadBudgetExceeded' && overSkip.payload?.budget === 400000,
    `POST api/multi/year/skip (valid fence) refuses over-budget with HTTP 503 (bytes=${overSkip.payload?.bytes}, budget=${overSkip.payload?.budget})`);

  const overSkipStale = await fetchJson(cdp, 'api/multi/year/skip', { fence: staleMa1Fence, targetYear: 3 });
  check(overSkipStale.status === 503 && overSkipStale.payload?.code === 'MultiViewPayloadBudgetExceeded' && overSkipStale.payload?.budget === 400000,
    `POST api/multi/year/skip (stale fence) refuses over-budget with HTTP 503 rather than over-budget 409`);

  const overHit = await fetchJson(cdp, 'api/multi/hit', { fence: await freshFence(), areaId: 'MA-A', u: 5, v: 5 });
  check(overHit.status === 503 && overHit.payload?.code === 'MultiViewPayloadBudgetExceeded' && overHit.payload?.budget === 400000,
    `POST api/multi/hit (valid fence) refuses over-budget with HTTP 503 (bytes=${overHit.payload?.bytes}, budget=${overHit.payload?.budget})`);

  const overHitStale = await fetchJson(cdp, 'api/multi/hit', { fence: staleMa1Fence, areaId: 'MA-A', u: 5, v: 5 });
  check(overHitStale.status === 503 && overHitStale.payload?.code === 'MultiViewPayloadBudgetExceeded' && overHitStale.payload?.budget === 400000,
    `POST api/multi/hit (stale fence) refuses over-budget with HTTP 503 rather than over-budget 409`);

  const overCell = await fetchJson(cdp, 'api/multi/cell', { fence: await freshFence(), areaId: 'MA-A', column: 1, row: 1 });
  check(overCell.status === 503 && overCell.payload?.code === 'MultiViewPayloadBudgetExceeded' && overCell.payload?.budget === 400000,
    `POST api/multi/cell refuses over-budget with HTTP 503 (bytes=${overCell.payload?.bytes}, budget=${overCell.payload?.budget})`);

  const overComp = await fetchJson(cdp, 'api/multi/competition', { fence: await freshFence(), areaId: 'MA-A', treeId: 'MA-A-0001' });
  check(overComp.status === 503 && overComp.payload?.code === 'MultiViewPayloadBudgetExceeded' && overComp.payload?.budget === 400000,
    `POST api/multi/competition refuses over-budget with HTTP 503 (bytes=${overComp.payload?.bytes}, budget=${overComp.payload?.budget})`);

  const overEcon = await fetchJson(cdp, 'api/multi/economy/session', { fence: await freshFence(), enabled: true, confirmReset: true });
  check(overEcon.status === 503 && overEcon.payload?.code === 'MultiViewPayloadBudgetExceeded' && overEcon.payload?.budget === 400000,
    `POST api/multi/economy/session (Money switch) refuses over-budget with HTTP 503 (bytes=${overEcon.payload?.bytes}, budget=${overEcon.payload?.budget})`);

  // Click #reread in UI -> shows clear alert banner AND keeps last good view intact!
  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('alert').classList.contains('shown') && document.getElementById('alert').textContent.includes('MultiViewPayloadBudgetExceeded')",
    'UI displays MultiViewPayloadBudgetExceeded alert banner');
  const afterCrowns = await evaluate(cdp, "document.querySelectorAll('#crowns [data-tree-id]').length");
  check(afterCrowns === beforeCrowns, `UI kept last good view intact after 503 overflow (${afterCrowns} === ${beforeCrowns} crowns)`);
  await shot(cdp, 'overflow-503-banner');

  auditErrors(true);
}

async function runClampRaise(cdp, collectEvents, auditErrors) {
  await setViewport(cdp, width, height);
  // Process launched with CCF_TEST_MAX_VIEW_BYTES=9999999 (attempting to RAISE budgets above frozen ceilings).
  // Switch to a near-cap 2,880-stem 5-lane custom woodland (120x120 m, 2,000 stems/ha, 5 lanes, PureSitka):
  //   - 3-lane filtered read (~1,728 stems) exceeds the frozen 524,288 B budget -> MUST still refuse 503 with budget=524288 (NEVER 9999999)!
  //   - If whole /view exceeds 2,621,440 B -> MUST still refuse 503 with budget=2621440 (NEVER 9999999)!
  const curV = (await fetchJson(cdp, 'api/multi/view')).payload.view;
  const nearCapRecipe = {
    widthM: 120, lengthM: 120, laneCount: 5, startAgeYears: 20, stemsPerHa: 2000, seed: 2880101,
    speciesMix: 'PureSitka', oakPercent: 0, beechPercent: 0,
  };
  const createRes = await fetchJson(cdp, 'api/multi/woodland/new', {
    fence: fenceOf(curV), choice: 'custom', presetId: null, recipe: nearCapRecipe, economyEnabled: false, confirmDiscardUnsaved: true,
  });
  const createOkOrClamped503 = (createRes.status === 200 && createRes.bytes <= 2621440) ||
    (createRes.status === 503 && createRes.payload?.code === 'MultiViewPayloadBudgetExceeded' && createRes.payload?.budget === 2621440);
  check(createOkOrClamped503,
    `CCF_TEST_MAX_VIEW_BYTES=9999999 cannot raise 2.5 MiB stand ceiling (woodland/new status=${createRes.status}, bytes=${createRes.payload?.bytes ?? createRes.bytes}, budget=${createRes.payload?.budget ?? 'within-2621440'})`);

  const stand3 = await fetchJson(cdp, 'api/multi/stand?lanes=MA-A,MA-B,MA-C');
  check(stand3.status === 503 && stand3.payload?.code === 'MultiViewPayloadBudgetExceeded' && stand3.payload?.budget === 524288,
    `CCF_TEST_MAX_VIEW_BYTES=9999999 cannot raise 512 KiB 3-lane budget (status=${stand3.status}, bytes=${stand3.payload?.bytes}, budget=${stand3.payload?.budget})`);

  const viewCheck = await fetchJson(cdp, 'api/multi/view');
  const viewClampedOk = (viewCheck.status === 200 && viewCheck.bytes <= 2621440) ||
    (viewCheck.status === 503 && viewCheck.payload?.code === 'MultiViewPayloadBudgetExceeded' && viewCheck.payload?.budget === 2621440);
  check(viewClampedOk,
    `CCF_TEST_MAX_VIEW_BYTES=9999999 cannot raise /view ceiling (status=${viewCheck.status}, bytes=${viewCheck.payload?.bytes ?? viewCheck.bytes}, budget=${viewCheck.payload?.budget ?? 'within-2621440'})`);

  auditErrors(true);
}

async function main() {
  const { cdp, child, profile } = await launch();
  const badNetwork = [];
  const consoleIssues = [];
  const cspViolations = [];
  const deliberateRefusal = (entry, allow503) => entry.source === 'network' &&
    (allow503 ? /status of (400|409|503)/.test(entry.text || '') : /status of (400|409)/.test(entry.text || ''));

  const netState = { viewGets: [], standOverviews: [], standLanes: [] };
  const netTracker = {
    reset() { netState.viewGets.length = 0; netState.standOverviews.length = 0; netState.standLanes.length = 0; },
    snapshot() { return { viewGets: netState.viewGets.slice(), standOverviews: netState.standOverviews.slice(), standLanes: netState.standLanes.slice() }; },
  };
  const reqMethods = new Map();

  const collectEvents = (allow503 = false) => {
    for (const e of cdp.events) {
      if (e.method === 'Network.requestWillBeSent') {
        reqMethods.set(e.params.requestId, e.params.request?.method || 'GET');
      }
      if (e.method === 'Runtime.exceptionThrown') consoleIssues.push(e.params.exceptionDetails?.text || 'exception');
      if (e.method === 'Runtime.consoleAPICalled' && e.params.type === 'error') consoleIssues.push(JSON.stringify(e.params.args));
      if (e.method === 'Log.entryAdded' && e.params.entry.level === 'error') {
        const txt = e.params.entry.text || '';
        if (/Content Security Policy/i.test(txt)) cspViolations.push(txt);
        else if (!deliberateRefusal(e.params.entry, allow503)) consoleIssues.push(txt);
      }
      if (e.method === 'Network.responseReceived') {
        const resp = e.params.response || {};
        const st = resp.status;
        const rUrl = resp.url || '';
        const method = reqMethods.get(e.params.requestId) || 'GET';
        const headers = resp.headers || {};
        const clRaw = headers['content-length'] ?? headers['Content-Length'] ?? '0';
        const bytes = Number(clRaw) || Math.round(resp.encodedDataLength || 0);
        if (method === 'GET') {
          try {
            const parsed = new URL(rUrl);
            if (parsed.pathname.endsWith('/api/multi/view')) {
              netState.viewGets.push({ status: st, bytes, url: parsed.pathname });
            } else if (parsed.pathname.endsWith('/api/multi/stand')) {
              const lanesParam = parsed.searchParams.get('lanes');
              if (lanesParam) {
                const decodedLanes = lanesParam.split(',').map((s) => s.trim()).filter(Boolean).sort().join(',');
                const laneCount = decodedLanes ? decodedLanes.split(',').length : 0;
                netState.standLanes.push({ status: st, bytes, lanes: decodedLanes, laneCount });
              } else {
                netState.standOverviews.push({ status: st, bytes });
              }
            }
          } catch { /* ignore non-URL */ }
        }
        if (st === 404 || (st >= 500 && !(allow503 && st === 503))) badNetwork.push(`${st} ${rUrl}`);
      }
      if (e.method === 'Network.loadingFailed' && !e.params.canceled) {
        badNetwork.push(`LOAD_FAIL ${e.params.errorText || 'unknown'}`);
      }
    }
    cdp.events.length = 0;
  };
  const auditErrors = (allow503) => {
    collectEvents(allow503);
    check(consoleIssues.length === 0 && cspViolations.length === 0 && badNetwork.length === 0,
      `Zero unexpected console/CSP/404/5xx errors (console=${consoleIssues.length}, csp=${cspViolations.length}, net=${badNetwork.length})`);
  };

  try {
    await cdp.send('Page.enable');
    await cdp.send('Runtime.enable');
    await cdp.send('Log.enable');
    await cdp.send('Network.enable');
    await setViewport(cdp, width, height);
    await cdp.send('Page.navigate', { url });
    await waitFor(cdp, "document.getElementById('year-value') && document.getElementById('year-value').textContent.trim() !== '–'",
      'the /multi page to render', 30000);
    await sleep(300);
    if (mode === 'full') await runFull(cdp, collectEvents, auditErrors, netTracker);
    else if (mode === 'restart-load') await runRestartLoad(cdp, collectEvents, auditErrors, netTracker);
    else if (mode === 'overflow-503') await runOverflow503(cdp, collectEvents, auditErrors);
    else if (mode === 'clamp-raise') await runClampRaise(cdp, collectEvents, auditErrors);
    else throw new Error('unknown mode: ' + mode);
  } finally {
    cdp.close();
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    try { rmSync(profile, { recursive: true, force: true }); } catch { /* ignore */ }
  }
  log(`SUMMARY ${vpLabel} ${tag}: ${passes} passed, ${failures} failed`);
  process.exit(failures === 0 ? 0 : 1);
}

main().catch((err) => {
  console.error('FATAL', err);
  process.exit(1);
});
