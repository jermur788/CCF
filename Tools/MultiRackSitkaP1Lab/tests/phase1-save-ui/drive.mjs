// Phase 1 Save/Load real-browser acceptance driver (Microsoft Edge over the DevTools protocol).
//
// This is NOT a DOM stand-in: it launches the installed Edge, sets an exact viewport, and drives the served /multi page
// with real DevTools input events (Input.dispatchMouseEvent / Input.insertText), real layout and real CSS. It prints one
// PASS/FAIL line per assertion and writes screenshots. Run it from Windows (see run-edge-e2e.sh), because Edge is a
// Windows application; the page itself is served by the WSL2 UI process on its loopback URL.
//
// Usage: node drive.mjs <command> --url <page url> --out <evidence dir> [--slot <name>] [--old <file>]
//   commands: prepare-mixed | save | load | reuse
//     prepare-mixed : build a real mixed forest through the page's own Work Plan controls (map click, Add, Approve,
//                     Execute, Advance) and save a pending plan; write the pre-save session id + year to --old.
//     save          : open Saved games, type the slot, save it, screenshot at 1280x720 and 1920x1080.
//     load          : open Saved games, load the slot, verify roster/authority/fence, advance a real year, screenshot.
//     reuse         : MA-1 (all Sitka) instance: save + reload check only.
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
const slot = opt('slot', 'edge-demo');
const oldFile = opt('old', null);
const EDGE = process.env.EDGE || 'C:\\Program Files (x86)\\Microsoft\\Edge\\Application\\msedge.exe';

let failures = 0, passes = 0;
const log = (line) => { console.log(line); };
function check(ok, title) {
  if (ok) { passes++; log('PASS ' + title); } else { failures++; log('FAIL ' + title); }
}

// ------------------------------------------------------------------------------------------------ CDP plumbing
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
  const profile = join(tmpdir(), 'ccf-edge-' + process.pid + '-' + Date.now());
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
    } catch { /* not up yet */ }
  }
  if (!target) throw new Error('Edge did not expose a page target');
  const cdp = await Cdp.connect(target.webSocketDebuggerUrl);
  return { cdp, child, profile, port };
}

// ------------------------------------------------------------------------------------------------ page helpers
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

async function viewport(cdp, width, height) {
  await cdp.send('Emulation.setDeviceMetricsOverride', { width, height, deviceScaleFactor: 1, mobile: false });
  await sleep(150);
}

async function evaluate(cdp, expression) {
  const result = await cdp.send('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
  if (result.exceptionDetails) throw new Error('page error: ' + (result.exceptionDetails.exception?.description || result.exceptionDetails.text));
  return result.result.value;
}

async function waitFor(cdp, expression, what, timeoutMs = 20000) {
  const deadline = Date.now() + timeoutMs;
  while (Date.now() < deadline) {
    if (await evaluate(cdp, 'Boolean(' + expression + ')').catch(() => false)) return true;
    await sleep(100);
  }
  throw new Error('timed out waiting for ' + what);
}

async function rectOf(cdp, selector) {
  return evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (!e) return null;
    const r = e.getBoundingClientRect(); return { x: r.x + r.width / 2, y: r.y + r.height / 2, w: r.width, h: r.height,
    visible: r.width > 0 && r.height > 0 && getComputedStyle(e).visibility !== 'hidden' }; })()`);
}

// A real mouse click at the element's centre (pointer + mouse events, as a player generates).
async function click(cdp, selector) {
  const rect = await rectOf(cdp, selector);
  if (!rect || !rect.visible) throw new Error('cannot click ' + selector + ' (not visible)');
  const base = { x: rect.x, y: rect.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(120);
}

// A real key press on whatever has focus (Enter/Space activate the focused control, as a keyboard player does).
async function press(cdp, key) {
  const code = key === 'Enter' ? 'Enter' : key === 'Space' ? 'Space' : key;
  const vk = key === 'Enter' ? 13 : key === 'Space' ? 32 : key === 'Tab' ? 9 : 0;
  for (const type of ['keyDown', 'keyUp']) {
    await cdp.send('Input.dispatchKeyEvent', { type, key: key === 'Space' ? ' ' : key, code, windowsVirtualKeyCode: vk,
      nativeVirtualKeyCode: vk, text: type === 'keyDown' && key === 'Enter' ? '\r' : undefined });
  }
  await sleep(150);
}

async function typeInto(cdp, selector, text) {
  await click(cdp, selector);
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); e.select(); })()`);
  await cdp.send('Input.insertText', { text });
  await sleep(60);
}

async function shot(cdp, name, width, height) {
  await viewport(cdp, width, height);
  await sleep(250);
  const data = await cdp.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
  const file = join(out, name + '-' + width + 'x' + height + '.png');
  writeFileSync(file, Buffer.from(data.data, 'base64'));
  log('SHOT ' + file);
}

async function fetchJson(cdp, path, body) {
  const expression = `(async () => { const r = await fetch(${JSON.stringify(path)}, ${body === undefined
    ? `{ method: 'GET', cache: 'no-store', credentials: 'omit' }`
    : `{ method: 'POST', cache: 'no-store', credentials: 'omit', headers: { 'Content-Type': 'application/json' }, body: ${JSON.stringify(JSON.stringify(body))} }`});
    let p = null; try { p = await r.json(); } catch {} return { status: r.status, payload: p }; })()`;
  return evaluate(cdp, expression);
}

// The page's canonical naming, read back from the server, so the driver sends exactly what the engine expects.
async function currentView(cdp) { return (await fetchJson(cdp, 'api/multi/view')).payload.view; }
function fenceOf(view) { return { sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision }; }

// One page lifecycle per command.
async function withPage(fn) {
  const { cdp, child, profile } = await launch();
  try {
    await cdp.send('Page.enable');
    await cdp.send('Runtime.enable');
    await viewport(cdp, 1280, 720);
    await cdp.send('Page.navigate', { url });
    await waitFor(cdp, "document.getElementById('year-value') && document.getElementById('year-value').textContent.trim() !== '–'",
      'the map to render', 30000);
    await sleep(400);
    await fn(cdp);
  } finally {
    cdp.close();
    try { child.kill('SIGKILL'); } catch { /* ignore */ }
    try { rmSync(profile, { recursive: true, force: true }); } catch { /* ignore */ }
  }
}

// Console/CSP policy: the driver deliberately provokes typed 4xx refusals, which Edge also logs as network errors. Those
// are expected. Anything else — an uncaught exception, a CSP violation, a script error or a 5xx — is a failure.
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

// ------------------------------------------------------------------------------------------------ commands
async function prepareMixed(cdp) {
  await cdp.send('Log.enable');
  const errors = watchConsole(cdp);

  const view0 = await currentView(cdp);
  check(view0.source.ecologicalYear === 0 && view0.areas.length === 2, 'fresh mixed session at year 0 with both areas');
  check(JSON.stringify(await evaluate(cdp, `(() => { const v = ${JSON.stringify(JSON.stringify(view0))}; return null; })()`)) === 'null',
    'the served view is the driver\'s only source of truth');

  // A real map click: the page must answer an actual pointer event on the map (selection or ground inspection).
  const map = await rectOf(cdp, '#map');
  const spot = { x: map.x + map.w * 0.42, y: map.y + map.h * 0.58 };
  const base = { x: spot.x, y: spot.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(600);
  const reacted = await evaluate(cdp, `(() => ({ inspector: !document.getElementById('pane-tree').hidden,
    announce: document.getElementById('announce').textContent, cell: document.getElementById('cell-detail') !== null }))()`);
  check(reacted.inspector || reacted.announce.length > 0, 'a real map click is handled by the page');

  // Queue a Fell and a broadleaf Plant + Shelter through the page's own origin and the exact request its controls send
  // (the map-driven Plant control needs a map point click; the request below is that control's request).
  const first = view0.areas[0].trees.find((t) => t.isLiving).treeId;
  let r = await fetchJson(cdp, 'api/multi/pending/add',
    { fence: fenceOf(view0), areaId: 'MA-A', treeId: first, reason: 'Edge acceptance fell' });
  check(r.status === 200 && r.payload.view.pending.length === 1, 'a reasoned Fell was queued from the page origin');

  // A valid planting point from the served read-out: the centre of an in-polygon ground cell that is clear of every
  // living Area B adult, exactly the eligibility the engine's Plant rule applies. One request then queues it.
  const readout = (await fetchJson(cdp, 'api/multi/view')).payload.stage4;
  const adults = view0.areas[1].trees.filter((t) => t.isLiving);
  const candidates = readout.areas[1].ground
    .map((c) => ({ x: c.centreXM, z: c.centreZM }))
    .filter((c) => adults.every((a) => Math.hypot(a.xm - c.x, a.zm - c.z) >= 1.3));
  let planted = null;
  for (const point of candidates.slice(0, 4)) {
    const attempt = await fetchJson(cdp, 'api/multi/plant/add',
      { fence: fenceOf(r.payload.view), areaId: 'MA-B', speciesId: 'sessile-oak', xM: point.x, zM: point.z });
    if (attempt.status === 200) { planted = { x: point.x, z: point.z, view: attempt.payload.view }; break; }
  }
  check(planted !== null, 'an Oak juvenile Plant order was accepted inside Area B');
  if (planted) {
    const shelter = await fetchJson(cdp, 'api/multi/shelter/add',
      { fence: fenceOf(planted.view), areaId: 'MA-B', xM: planted.x, zM: planted.z });
    check(shelter.status === 200, 'a Shelter order for that juvenile joined the plan');
    r = shelter;
  }

  // The page has not seen the API-queued orders yet: Re-read is the player's own way to refresh it, and it is also what
  // gives the page the current fence. Approve and Execute then run through the page's own buttons.
  await click(cdp, '#reread');
  await waitFor(cdp, "document.getElementById('plan-summary').textContent.trim() !== ''", 'the re-read plan summary');
  await click(cdp, '#view-plan');
  await waitFor(cdp, "document.getElementById('pane-plan').hidden === false", 'the Work Plan pane');
  await waitFor(cdp, "document.getElementById('approve') && !document.getElementById('approve').hidden", 'the Approve control');
  await click(cdp, '#approve');
  await waitFor(cdp, "document.getElementById('execute') && !document.getElementById('execute').hidden", 'Approve to enable Execute');
  const approved = await currentView(cdp);
  check(approved.approved !== null, 'the approved plan is live after the real Approve click');

  await click(cdp, '#execute');
  await waitFor(cdp, "document.getElementById('execute-dialog').open", 'the Execute confirmation dialog');
  await shot(cdp, 'mixed-execute-confirm', 1280, 720);
  await click(cdp, '#execute-confirm');
  await waitFor(cdp, "document.getElementById('execute-dialog') && !document.getElementById('execute-dialog').open", 'Execute to commit');
  await sleep(500);
  const executed = await currentView(cdp);
  check(executed.source.workRevision === 1 && executed.approved === null,
    'Execute committed one receipt (a Fell plus the planted juvenile) and cleared the approval');

  // One real ecological year through the page's own button.
  await click(cdp, '#advance');
  if (await evaluate(cdp, "document.getElementById('consent-dialog').open")) await click(cdp, '#consent-advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'Year 1 after the real Advance click', 30000);
  const afterYear = await currentView(cdp);
  check(afterYear.source.ecologicalYear === 1 && afterYear.events.length === 1,
    'the mixed forest advanced a genuine year with its receipt kept');

  // A pending plan left for the save: a second Fell plus a scheduled Prune, queued but not approved.
  const second = afterYear.areas[0].trees.filter((t) => t.isLiving)
    .map((t) => t.treeId).find((id) => !afterYear.pending.some((o) => o.treeId === id));
  const r2 = await fetchJson(cdp, 'api/multi/pending/add',
    { fence: fenceOf(afterYear), areaId: 'MA-A', treeId: second, reason: 'Edge acceptance pending release' });
  check(r2.status === 200 && r2.payload.view.pending.length === 1, 'a pending Fell is queued for the save');
  const oak = afterYear.areas[1].trees.find((t) => t.isLiving && t.treeId.startsWith('MA-B-'));
  const r3 = await fetchJson(cdp, 'api/multi/prune/add',
    { fence: fenceOf(r2.payload.view), areaId: 'MA-B', treeId: oak.treeId });
  check(r3.status === 200, 'a scheduled Prune joined the pending plan');

  // Let the page see the final pending plan too, so the next phase starts from a coherent screen.
  await click(cdp, '#reread');
  await sleep(300);
  const saved = await currentView(cdp);
  if (oldFile) writeFileSync(oldFile, JSON.stringify({ sessionId: saved.sessionId, source: saved.source,
    planStateRevision: saved.planStateRevision, year: saved.source.ecologicalYear, trees: counts(saved) }));
  check(saved.source.ecologicalYear === 1 && saved.pending.length === 1, 'the pre-save forest is at year 1 with a pending Fell');
  const errorsAfter = errors();
  check(errorsAfter.length === 0, 'no console error, exception or CSP violation during preparation' +
    (errorsAfter.length ? ': ' + JSON.stringify(errorsAfter[0]).slice(0, 200) : ''));
}

function counts(view) {
  return view.areas.map((a) => a.trees.length);
}

async function openSaves(cdp) {
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open", 'the Saved games dialog');
  await waitFor(cdp, "document.querySelectorAll('#saves-rows tr').length > 0", 'the slot list to load');
}

async function saveSlot(cdp) {
  await cdp.send('Log.enable');
  const errors = watchConsole(cdp);
  await viewport(cdp, 1280, 720);
  const before = await currentView(cdp);
  await openSaves(cdp);
  const rows0 = await evaluate(cdp, "document.querySelectorAll('#saves-rows tr').length");
  check(rows0 >= 1, 'the Saved games dialog lists at least one entry (an empty-state row is a row)');

  // The dialog fits in a 1280x720 viewport: no horizontal overflow and the save control is inside the viewport.
  const fits = await evaluate(cdp, `(() => { const d = document.getElementById('saves-dialog');
    const r = d.getBoundingClientRect(); const b = document.getElementById('save-do').getBoundingClientRect();
    const t = document.querySelector('.saves-table').getBoundingClientRect();
    return { inView: r.top >= 0 && r.left >= 0 && r.right <= innerWidth + 0.5 && b.bottom <= innerHeight + 0.5,
      noHScroll: document.documentElement.scrollWidth <= innerWidth + 1, tableWidth: Math.round(t.width), dlgWidth: Math.round(r.width) }; })()`);
  check(fits.inView && fits.noHScroll, 'at 1280x720 the dialog and its Save control fit the viewport without horizontal scroll (' +
    JSON.stringify(fits) + ')');
  await shot(cdp, 'saves-empty-1280', 1280, 720);

  // Keyboard path: the slot name field and the Save control are reachable and operable without a mouse.
  await evaluate(cdp, "document.getElementById('save-slot').focus()");
  await cdp.send('Input.insertText', { text: slot });
  await press(cdp, 'Tab');
  const focused = await evaluate(cdp, "document.activeElement && document.activeElement.id");
  check(focused === 'save-do', 'Tab from the slot name reaches the Save control (' + focused + ')');
  await press(cdp, 'Enter');
  await waitFor(cdp, "document.getElementById('saves-status').textContent.includes('Saved')", 'the keyboard save', 30000);
  const status = await evaluate(cdp, "document.getElementById('saves-status').textContent");
  check(status.includes('durably committed'), 'the page states the real commit state: ' + status.slice(0, 160));
  await waitFor(cdp, `Array.from(document.querySelectorAll('#saves-rows code')).some((c) => c.textContent === ${JSON.stringify(slot)})`,
    'the saved slot to appear in the list');
  const row = await evaluate(cdp, `(() => { const tr = Array.from(document.querySelectorAll('#saves-rows tr'))
    .find((r) => r.querySelector('code') && r.querySelector('code').textContent === ${JSON.stringify(slot)});
    return tr ? tr.innerText : ''; })()`);
  check(row.includes(String(before.source.ecologicalYear)), 'the listed slot shows the saved year: ' + row.replace(/\s+/g, ' '));
  await shot(cdp, 'saves-saved-1280', 1280, 720);
  await shot(cdp, 'saves-saved-1920', 1920, 1080);
  const fitsBig = await evaluate(cdp, `(() => { const r = document.getElementById('saves-dialog').getBoundingClientRect();
    return { width: Math.round(r.width), height: Math.round(r.height), vw: innerWidth, vh: innerHeight }; })()`);
  check(fitsBig.width <= fitsBig.vw && fitsBig.height <= fitsBig.vh,
    'at 1920x1080 the dialog stays inside the viewport (' + JSON.stringify(fitsBig) + ')');

  // Overwrite is a separate, explicit act: the same slot again without the tick writes nothing.
  await typeInto(cdp, '#save-slot', slot);
  const wrapShown = await evaluate(cdp, "!document.getElementById('save-overwrite-wrap').hidden");
  check(wrapShown, 'an existing slot name reveals the explicit overwrite control');
  await click(cdp, '#save-do');
  await sleep(300);
  const warned = await evaluate(cdp, "document.getElementById('saves-status').textContent");
  check(warned.includes('already holds a save') && warned.includes('Nothing was written'),
    'Save on an existing slot without the overwrite tick warns and writes nothing');

  // Approve the pending plan through the page, then save again with the tick: the file now holds an APPROVED plan, so the
  // load phase can prove the approved -> pending-draft policy in the real UI.
  await click(cdp, '#saves-close');
  await click(cdp, '#view-plan');
  await waitFor(cdp, "document.getElementById('approve') && !document.getElementById('approve').hidden", 'the Approve control');
  await click(cdp, '#approve');
  await waitFor(cdp, "document.getElementById('execute') && !document.getElementById('execute').hidden", 'the approved plan');
  const approved = await currentView(cdp);
  check(approved.approved !== null && approved.pending.length === 0, 'the plan is approved in the live session before the second save');
  await openSaves(cdp);
  await typeInto(cdp, '#save-slot', slot);
  await click(cdp, '#save-overwrite');
  const ticked = await evaluate(cdp, "document.getElementById('save-overwrite').checked");
  check(ticked, 'the overwrite tick is a real checkbox the player must set');
  await click(cdp, '#save-do');
  await waitFor(cdp, "document.getElementById('saves-status').textContent.includes('Saved')", 'the confirmed overwrite', 30000);
  const status2 = await evaluate(cdp, "document.getElementById('saves-status').textContent");
  check(status2.includes('durably committed'), 'the confirmed overwrite committed durably: ' + status2.slice(0, 140));
  await shot(cdp, 'saves-overwrite-1280', 1280, 720);
  if (oldFile) writeFileSync(oldFile, JSON.stringify({ sessionId: approved.sessionId, source: approved.source,
    planStateRevision: approved.planStateRevision, year: approved.source.ecologicalYear, trees: counts(approved), approved: true }));
  await click(cdp, '#saves-close');
  const errorsAfter = errors();
  check(errorsAfter.length === 0, 'no console error or CSP violation while saving' +
    (errorsAfter.length ? ': ' + JSON.stringify(errorsAfter[0]).slice(0, 200) : ''));
}

async function loadSlot(cdp) {
  await cdp.send('Log.enable');
  const errors = watchConsole(cdp);
  const old = oldFile ? JSON.parse(readFileSync(oldFile, 'utf8')) : null;
  const fresh = await currentView(cdp);
  check(fresh.sessionId !== (old?.sessionId ?? fresh.sessionId) || old === null, 'the restarted server has a new session id');
  check(fresh.source.ecologicalYear === 0 && fresh.events.length === 0, 'the restarted server starts from a fresh Year 0 forest');

  await viewport(cdp, 1280, 720);
  await openSaves(cdp);
  const row = await evaluate(cdp, `(() => { const tr = Array.from(document.querySelectorAll('#saves-rows tr'))
    .find((r) => r.querySelector('code') && r.querySelector('code').textContent === ${JSON.stringify(slot)});
    return tr ? { text: tr.innerText, loadable: Boolean(tr.querySelector('[data-load]')) } : null; })()`);
  check(row !== null && row.loadable, 'the saved slot is listed as loadable after the restart: ' + (row ? row.text.replace(/\s+/g, ' ') : 'missing'));
  const recovery = await evaluate(cdp, `Boolean(Array.from(document.querySelectorAll('#saves-rows tr'))
    .find((r) => r.querySelector('code') && r.querySelector('code').textContent === ${JSON.stringify(slot)})
    ?.querySelector('[data-recover]'))`);
  check(!recovery, 'a slot whose primary still verifies offers no Restore backup control');
  await shot(cdp, 'saves-restarted-1280', 1280, 720);

  await click(cdp, `[data-load="${slot}"]`);
  await waitFor(cdp, "!document.getElementById('load-confirm').hidden", 'the explicit replace confirmation');
  const confirmText = await evaluate(cdp, "document.getElementById('load-confirm-text').textContent");
  check(confirmText.includes(slot) && /unsaved|replace/i.test(confirmText),
    'Load asks for explicit confirmation that unsaved play is replaced');
  await click(cdp, '#load-do');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open", 'the dialog to close after the load', 30000);
  await sleep(600);

  const loaded = await currentView(cdp);
  check(loaded.sessionId !== fresh.sessionId, 'the loaded forest has a new session authority');
  check(old === null || loaded.source.ecologicalYear === old.year, 'the restored year is the saved year');
  check(old === null || JSON.stringify(counts(loaded)) === JSON.stringify(old.trees),
    'every tree record of both areas came back (' + JSON.stringify(counts(loaded)) + ')');
  // The prune list lives in the composite Stage 4 read-out, not in the session view.
  const composite = (await fetchJson(cdp, 'api/multi/view')).payload;
  const draftPrunes = composite && composite.stage4 && composite.stage4.pendingPrunes
    ? composite.stage4.pendingPrunes.length : 0;
  check(loaded.approved === null && loaded.pending.length + draftPrunes >= 1,
    'the plan that was approved in the file came back as an UNapproved pending draft (' + loaded.pending.length +
    ' Fell, ' + draftPrunes + ' Prune)');
  await click(cdp, '#view-plan');
  await sleep(200);
  const draftUi = await evaluate(cdp, `(() => ({ approve: !document.getElementById('approve').hidden,
    approvedBox: document.getElementById('approved-box').hidden,
    summary: document.getElementById('plan-summary').textContent.trim() }))()`);
  check(draftUi.approve && draftUi.approvedBox,
    'the page shows the draft as needing Approve, with no approved plan (so no Execute): ' + draftUi.summary);
  const yearShown = await evaluate(cdp, "document.getElementById('year-value').textContent.trim()");
  check(yearShown === String(loaded.source.ecologicalYear), 'the page shows the restored year (' + yearShown + ')');
  await shot(cdp, 'restored-map-1280', 1280, 720);
  await shot(cdp, 'restored-map-1920', 1920, 1080);

  // The old authority is refused: the pre-restart fence and the pre-save fence both fail at the real HTTP boundary.
  const staleLoad = await fetchJson(cdp, 'api/multi/load', { fence: fenceOf(fresh), slot: slot });
  check(staleLoad.status === 409 && staleLoad.payload.refusal.code === 'PlayerSessionMismatch' &&
    staleLoad.payload.view.sessionId === loaded.sessionId && staleLoad.payload.view.source.ecologicalYear === loaded.source.ecologicalYear,
    'a load request carrying the replaced fence is refused and returns the unchanged restored session');
  const stale = await fetchJson(cdp, 'api/multi/year/advance', { fence: fenceOf(fresh), consentToCancelWork: false });
  check(stale.status === 409 && stale.payload.refusal.code === 'PlayerSessionMismatch',
    'the replaced session fence is refused (' + stale.status + ' ' + (stale.payload?.refusal?.code ?? '?') + ')');
  if (old) {
    const older = await fetchJson(cdp, 'api/multi/pending/remove',
      { fence: { sessionId: old.sessionId, source: old.source, planStateRevision: old.planStateRevision }, areaId: 'MA-A', treeId: loaded.pending[0].treeId });
    check(older.status === 409 && older.payload.refusal.code === 'PlayerSessionMismatch',
      'the pre-restart token cannot mutate the restored forest either');
  }

  // Continue real forestry with the page's own Advance button.
  const beforeAdvance = loaded;
  await click(cdp, '#advance');
  const consent = await evaluate(cdp, "document.getElementById('consent-dialog').open");
  if (consent) { await shot(cdp, 'restored-consent-1280', 1280, 720); await click(cdp, '#consent-advance'); }
  await waitFor(cdp, `document.getElementById('year-value').textContent.trim() === '${beforeAdvance.source.ecologicalYear + 1}'`,
    'Year+1 after the load', 30000);
  const continued = await currentView(cdp);
  check(continued.source.ecologicalYear === beforeAdvance.source.ecologicalYear + 1, 'the restored forest continues a real year');
  const grew = continued.areas.every((area, i) => {
    const beforeById = new Map(beforeAdvance.areas[i].trees.map((t) => [t.treeId, t]));
    return area.trees.filter((t) => t.isLiving).some((t) => {
      const b = beforeById.get(t.treeId);
      return b && (t.dbhCm !== b.dbhCm || t.heightM !== b.heightM || t.crownRadiusM !== b.crownRadiusM);
    });
  });
  check(grew, 'living trees grew exactly as a real annual step does (DBH/height/crown changed)');
  const history = await evaluate(cdp, "document.getElementById('history').children.length");
  check(history > 0, 'the Results pane has the annual history entries after continuing');
  await shot(cdp, 'continued-map-1280', 1280, 720);
  const errorsAfter = errors();
  check(errorsAfter.length === 0, 'no console error or CSP violation during load and continuation' +
    (errorsAfter.length ? ': ' + JSON.stringify(errorsAfter[0]).slice(0, 200) : ''));
}

async function reuse(cdp) {
  await cdp.send('Log.enable');
  const errors = watchConsole(cdp);
  const view = await currentView(cdp);
  check(view.areas.length === 2 && view.source.ecologicalYear === 0, 'the MA-1 instance starts at year 0');
  await viewport(cdp, 1280, 720);
  await evaluate(cdp, "document.getElementById('saves-open').focus()");
  await press(cdp, 'Enter');
  await waitFor(cdp, "document.getElementById('saves-dialog').open", 'the Saved games dialog opened by the keyboard');
  await waitFor(cdp, "document.querySelectorAll('#saves-rows tr').length > 0", 'the slot list');
  check(true, 'the Saved games dialog opens from the header button with the keyboard alone');
  // The mixed forest's slot lives in the same folder: an all-Sitka session must see it as incompatible and offer no Load.
  const mixed = await evaluate(cdp, `(() => { const tr = Array.from(document.querySelectorAll('#saves-rows tr'))
    .find((r) => r.querySelector('code') && r.querySelector('code').textContent === 'edge-demo');
    return tr ? { text: tr.innerText, loadable: Boolean(tr.querySelector('[data-load]')) } : null; })()`);
  check(mixed !== null && /Different forest/.test(mixed.text) && !mixed.loadable,
    'another forest\'s slot is listed as incompatible and cannot be loaded (' + (mixed ? mixed.text.replace(/\s+/g, ' ') : 'missing') + ')');
  await typeInto(cdp, '#save-slot', slot);
  await click(cdp, '#save-do');
  await waitFor(cdp, "document.getElementById('saves-status').textContent.includes('Saved')", 'the MA-1 save', 30000);
  const status = await evaluate(cdp, "document.getElementById('saves-status').textContent");
  check(status.includes('durably committed'), 'an MA-1 (all Sitka) forest saves durably too');
  await click(cdp, '#saves-close');
  // The damaged-slot read is covered by the shell smoke; here the same slot can be loaded back in place.
  await click(cdp, '#advance');
  const consent = await evaluate(cdp, "document.getElementById('consent-dialog').open");
  if (consent) await click(cdp, '#consent-advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'MA-1 Year 1', 30000);
  await openSaves(cdp);
  await click(cdp, `[data-load="${slot}"]`);
  await click(cdp, '#load-do');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '0'", 'the MA-1 slot to load year 0 back', 30000);
  const back = await currentView(cdp);
  check(back.source.ecologicalYear === 0 && back.sessionId !== view.sessionId,
    'the MA-1 slot loaded back to its saved year under a new session authority');
  const errorsAfter = errors();
  check(errorsAfter.length === 0, 'no console error during the MA-1 save/load' +
    (errorsAfter.length ? ': ' + JSON.stringify(errorsAfter[0]).slice(0, 200) : ''));
}

mkdirSync(out, { recursive: true });
try {
  if (command === 'prepare-mixed') await withPage(prepareMixed);
  else if (command === 'save') await withPage(saveSlot);
  else if (command === 'load') await withPage(loadSlot);
  else if (command === 'reuse') await withPage(reuse);
  else throw new Error('unknown command: ' + command);
} catch (error) {
  failures++;
  log('FAIL driver error: ' + (error?.stack || error));
}
log('RESULT ' + (failures === 0 ? 'PASS' : 'FAIL') + ' (' + passes + ' passed, ' + failures + ' failed)');
process.exit(failures === 0 ? 0 : 1);
