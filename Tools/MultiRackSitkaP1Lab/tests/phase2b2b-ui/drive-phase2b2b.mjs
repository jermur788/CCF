// CCF Multi 2D — Phase 2B-2b Dispositions UI + Save v3 Economy real-browser acceptance driver (Microsoft Edge over CDP).
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
const mode = opt('mode', 'save'); // 'save' or 'load'
const width = Number(opt('width', '1280'));
const height = Number(opt('height', '720'));
const slotName = opt('slot', 'p2b2b-v3');
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
  const profile = join(tmpdir(), 'ccf-p2b2b-edge-' + process.pid + '-' + Date.now());
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
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); if (e && e.scrollIntoView) e.scrollIntoView({ block: 'center', inline: 'nearest' }); })()`);
  await sleep(60);
  const rect = await rectOf(cdp, selector);
  if (!rect || !rect.visible) throw new Error('cannot click ' + selector + ' (not visible)');
  const base = { x: rect.x, y: rect.y, button: 'left', clickCount: 1, pointerType: 'mouse' };
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseMoved', ...base, button: 'none' });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mousePressed', ...base });
  await cdp.send('Input.dispatchMouseEvent', { type: 'mouseReleased', ...base });
  await sleep(140);
}

async function typeInto(cdp, selector, text) {
  await evaluate(cdp, `(() => { const e = document.querySelector(${JSON.stringify(selector)}); e.focus(); e.value = ${JSON.stringify(text)}; e.dispatchEvent(new Event('input', { bubbles: true })); })()`);
  await sleep(80);
}

async function shot(cdp, name, w = width, h = height) {
  await setViewport(cdp, w, h);
  await sleep(160);
  const data = await cdp.send('Page.captureScreenshot', { format: 'png', captureBeyondViewport: false });
  const file = join(out, `${tag}-${mode}-${name}-${w}x${h}.png`);
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

async function checkLayout(cdp, label) {
  const geom = await evaluate(cdp, `(() => {
    const noHScroll = document.documentElement.scrollWidth <= window.innerWidth + 1;
    const actions = Array.from(document.querySelectorAll('.top-actions > button'));
    const rects = actions.map((b) => { const r = b.getBoundingClientRect(); return { id: b.id, top: Math.round(r.top), right: Math.round(r.right) }; });
    const maxTopDelta = rects.length ? Math.max(...rects.map((r) => r.top)) - Math.min(...rects.map((r) => r.top)) : 0;
    const allInWidth = rects.every((r) => r.right <= window.innerWidth + 1);
    const wsCols = getComputedStyle(document.getElementById('workspace')).gridTemplateColumns.split(' ').filter(Boolean).length;
    const histBtns = document.querySelectorAll('#history-open').length;
    const strip = document.getElementById('economy-strip');
    const stripRect = strip && !strip.hidden ? strip.getBoundingClientRect() : null;
    const stripFits = !stripRect || stripRect.right <= window.innerWidth + 1;
    const planPane = document.getElementById('pane-plan');
    const planNoHScroll = !planPane || planPane.scrollWidth <= planPane.clientWidth + 2;
    return { noHScroll, scrollW: document.documentElement.scrollWidth, innerW: window.innerWidth, maxTopDelta, allInWidth, wsCols, histBtns, stripFits, planNoHScroll };
  })()`);
  check(geom.noHScroll && geom.maxTopDelta <= 10 && geom.allInWidth && geom.wsCols <= 3 && geom.histBtns === 1 && geom.stripFits && geom.planNoHScroll,
    `layout (${label}): zero h-scroll (${geom.scrollW}<=${geom.innerW}), topbar delta=${geom.maxTopDelta}px, ${geom.wsCols} cols (<=3), 1 History btn, planNoHScroll=${geom.planNoHScroll}`);
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

async function runSavePhase(cdp) {
  const audit = makeAuditor(cdp);
  audit.collect(true);
  check(audit.getInitialOk() >= 16 && audit.badNetwork.length === 0 && audit.consoleIssues.length === 0 && audit.cspViolations.length === 0,
    `Initial page load clean (${audit.getInitialOk()} HTTP 200 assets, console=${audit.consoleIssues.length}, csp=${audit.cspViolations.length}, net=${audit.badNetwork.length})`);

  // 1. Money Off baseline: History drawer preserves FinancialLedger + NOT RECORDED + zero €
  await setViewport(cdp, width, height);
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'Advance to Year 1 while Off');
  await click(cdp, '#history-open');
  await waitFor(cdp, "document.querySelector('[data-history-year=\"0\"]') !== null", 'Year 0 in History (Off)');
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.getElementById('history-detail-pane').textContent.includes('Not Recorded in 2D History')", 'Year 0 detail (Off)');
  const offHist = await evaluate(cdp, `(() => {
    const p = document.getElementById('history-detail-pane');
    const txt = p.textContent;
    return {
      hasFinCard: Boolean(p.querySelector('[data-history-section="financial-ledger"]')),
      hasNotRec: txt.includes('FinancialLedger') && txt.includes('NOT RECORDED'),
      hasEuro: txt.includes('€'),
    };
  })()`);
  check(!offHist.hasFinCard && offHist.hasNotRec && !offHist.hasEuro,
    `Money Off History drawer keeps FinancialLedger NOT RECORDED and zero € (${JSON.stringify(offHist)})`);
  await click(cdp, '#history-close');
  await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'History drawer closed (Off)');

  // 2. Start Forest Maker woodland (fm-064-sitka-edge-broadleaves-v1) with Money On via New Woodland dialog
  await click(cdp, '#woodland-open');
  await waitFor(cdp, "document.getElementById('woodland-dialog').open && document.querySelector('input[name=\"woodland-choice\"]:checked') !== null", 'New woodland dialog ready');
  await evaluate(cdp, `(() => {
    const cb = document.getElementById('woodland-economy');
    if (!cb.checked) { cb.checked = true; cb.dispatchEvent(new Event('change', { bubbles: true })); }
  })()`);
  await click(cdp, '#woodland-create');
  await waitFor(cdp, "!document.getElementById('woodland-confirm-box').hidden", 'Unsaved progress confirm shown');
  await click(cdp, '#woodland-confirm-discard');
  await waitFor(cdp, "!document.getElementById('woodland-dialog').open && document.getElementById('economy-toggle-label').textContent.trim() === 'Money: On' && document.getElementById('year-value').textContent.trim() === '0'",
    'Forest Maker woodland loaded with Money On');
  await sleep(250);

  await checkLayout(cdp, 'Forest Maker Money On Year 0');
  const strip0 = await evaluate(cdp, `({
    cash: document.getElementById('economy-cash').textContent.trim(),
    hours: document.getElementById('economy-hours').textContent.trim(),
    retained: document.getElementById('economy-retained').textContent.trim(),
    stage: document.getElementById('economy-stage').textContent.trim(),
    note: document.getElementById('economy-strip-note').textContent.trim(),
  })`);
  check(strip0.cash === '€12,000.00' && strip0.hours === '40 / 40 hrs' && strip0.retained === '0.000 m³' && strip0.note.includes('Save v3 enabled'),
    `Top resource strip shows Cash, 40/40 hrs, Retained 0.000 m³, and Save v3 enabled (${JSON.stringify(strip0)})`);

  // 3. Pick 3 living stems in MA-A (lane 0) and 1 ClearUndergrowth cell in MA-A
  const laneA0 = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-A')).payload;
  const deadwoodBefore = laneA0.stage4.areas[0].canopy.fallenDeadwoodCount;
  const liveTrees = laneA0.view.areas[0].trees.filter((t) => t.isLiving);
  const tSell = liveTrees[0].treeId;
  const tKeep = liveTrees[1].treeId;
  const tLeave = liveTrees[2].treeId;
  const cell0 = laneA0.stage4.areas[0].groundCompact[0];
  const clearCol = cell0[0];
  const clearRow = cell0[1];

  let curV = laneA0.view;
  for (const [tid, why] of [[tSell, 'sell stem'], [tKeep, 'keep for own use'], [tLeave, 'leave as deadwood']]) {
    const r = await fetchJson(cdp, 'api/multi/pending/add', { fence: fenceOf(curV), areaId: 'MA-A', treeId: tid, reason: why });
    check(r.status === 200, `Queued pending Fell ${tid}`);
    curV = r.payload.view;
  }
  const addClear = await fetchJson(cdp, 'api/multi/clear/add', { fence: fenceOf(curV), areaId: 'MA-A', column: clearCol, row: clearRow });
  curV = addClear.payload.view;
  const setOwner = await fetchJson(cdp, 'api/multi/economy/executor', {
    fence: fenceOf(curV), kind: 'ClearUndergrowth', areaId: 'MA-A', targetKey: `${clearCol},${clearRow}`, executor: 'Landowner',
  });
  curV = setOwner.payload.view;

  // Verify contractor-only Fell refusal & unknown disposition refusal
  const fellOwnerRef = await fetchJson(cdp, 'api/multi/economy/executor', {
    fence: fenceOf(curV), kind: 'Fell', areaId: 'MA-A', targetKey: tSell, executor: 'Landowner',
  });
  check(fellOwnerRef.status === 409 && fellOwnerRef.payload?.refusal?.code === 'EconomyFellContractorOnly',
    'Fell stays contractor-only (409 EconomyFellContractorOnly)');
  const badDispRef = await fetchJson(cdp, 'api/multi/economy/disposition', {
    fence: fenceOf(curV), areaId: 'MA-A', treeId: tSell, disposition: 'ClearfellAll',
  });
  check(badDispRef.status === 409 && badDispRef.payload?.refusal?.code === 'EconomyDispositionUnknown',
    'Unknown disposition refused (409 EconomyDispositionUnknown)');

  // Re-read in UI and open Work Plan tab
  await click(cdp, '#reread');
  await sleep(200);
  await click(cdp, '[data-pane-button="plan"]');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'plan' && document.querySelectorAll('[data-disposition-value]').length === 9",
    'Work Plan tab renders 3-way disposition controls for all 3 Fell orders');

  // Capture all-sell quote before changing dispositions
  const quoteAllSell = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  const revBeforeKeep = quoteAllSell.activeQuote.planStateRevision;

  // 4. Click Keep for Own Use on tKeep via UI button
  const keepSel = `[data-disposition-area="MA-A"][data-disposition-tree="${tKeep}"][data-disposition-value="KeepForUse"]`;
  await click(cdp, keepSel);
  await waitFor(cdp, `document.querySelector('${keepSel}')?.getAttribute('aria-pressed') === 'true'`,
    `UI switched ${tKeep} to Keep for Own Use`);
  const quoteAfterKeep = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  check(quoteAfterKeep.activeQuote.planStateRevision > revBeforeKeep &&
    quoteAfterKeep.activeQuote.harvestJob.retainedTimber.retainedTimberCm3 > 0 &&
    quoteAfterKeep.activeQuote.timberSaleRevenueCents < quoteAllSell.activeQuote.timberSaleRevenueCents,
    `KeepForUse bumped planStateRevision (${revBeforeKeep}->${quoteAfterKeep.activeQuote.planStateRevision}), set server retained m³ (${quoteAfterKeep.activeQuote.harvestJob.retainedTimber.volumeDisplay}), and reduced cash revenue`);

  // Verify ZERO editable volume input box anywhere in pending orders or quote panel
  const keepUiCheck = await evaluate(cdp, `(() => {
    const inputs = document.querySelectorAll('#pending-areas input, #economy-quote-panel input').length;
    const readout = document.querySelector('[data-disposition-readout="${tKeep}"]')?.textContent || '';
    return { inputs, readout };
  })()`);
  check(keepUiCheck.inputs === 0 && keepUiCheck.readout.includes(quoteAfterKeep.activeQuote.harvestJob.retainedTimber.volumeDisplay),
    `Keep for Own Use has ZERO editable volume input and displays server-supplied retained m³ (${keepUiCheck.readout})`);

  // 5. Click Leave as Deadwood on tLeave via UI button
  const revBeforeLeave = quoteAfterKeep.activeQuote.planStateRevision;
  const leaveSel = `[data-disposition-area="MA-A"][data-disposition-tree="${tLeave}"][data-disposition-value="LeaveAsDeadwood"]`;
  await click(cdp, leaveSel);
  await waitFor(cdp, `document.querySelector('${leaveSel}')?.getAttribute('aria-pressed') === 'true'`,
    `UI switched ${tLeave} to Leave as Deadwood`);
  const quoteMixed = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  check(quoteMixed.activeQuote.planStateRevision > revBeforeLeave &&
    quoteMixed.activeQuote.harvestJob.deadwoodIntentCount === 1 &&
    quoteMixed.activeQuote.harvestJob.minimumJobFloorApplied === true,
    `LeaveAsDeadwood bumped planStateRevision (${revBeforeLeave}->${quoteMixed.activeQuote.planStateRevision}), set deadwoodIntentCount=1, kept €2,500 floor`);

  await checkLayout(cdp, 'Work Plan Pending Mixed Dispositions');
  await shot(cdp, 'work-plan-dispositions-pending');

  // 6. Approve -> verify disposition change while approved is refused -> Cancel -> verify stale fence refused -> re-queue & Execute
  await click(cdp, '#approve');
  await waitFor(cdp, "!document.getElementById('approved-box').hidden && document.getElementById('economy-quote-badge').textContent.includes('Frozen')",
    'Work Plan approved with frozen quote');

  const approvedOverview = (await fetchJson(cdp, 'api/multi/stand')).payload;
  const approvedFence = approvedOverview.fence;
  const editWhileApproved = await fetchJson(cdp, 'api/multi/economy/disposition', {
    fence: approvedFence, areaId: 'MA-A', treeId: tKeep, disposition: 'SellAndExtract',
  });
  check(editWhileApproved.status === 409 && editWhileApproved.payload?.refusal?.code === 'WorkPlanAlreadyApproved',
    'Changing disposition while approved refused with 409 WorkPlanAlreadyApproved');

  // Cancel approval, re-add the 3 stems + Clear (Landowner), re-apply dispositions via UI, verify stale fence refused, then re-Approve
  await click(cdp, '#cancel-approved');
  await waitFor(cdp, "document.getElementById('approved-box').hidden", 'Approved plan cancelled');
  const staleAfterCancel = await fetchJson(cdp, 'api/multi/economy/disposition', {
    fence: approvedFence, areaId: 'MA-A', treeId: tKeep, disposition: 'KeepForUse',
  });
  check(staleAfterCancel.status === 409 && staleAfterCancel.payload?.refusal?.code === 'PlayerPlanStateStale',
    'Stale pre-cancel fence refused on disposition endpoint (409 PlayerPlanStateStale)');

  let reV = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-A')).payload.view;
  for (const [tid, why] of [[tSell, 'sell stem'], [tKeep, 'keep for own use'], [tLeave, 'leave as deadwood']]) {
    reV = (await fetchJson(cdp, 'api/multi/pending/add', { fence: fenceOf(reV), areaId: 'MA-A', treeId: tid, reason: why })).payload.view;
  }
  reV = (await fetchJson(cdp, 'api/multi/clear/add', { fence: fenceOf(reV), areaId: 'MA-A', column: clearCol, row: clearRow })).payload.view;
  reV = (await fetchJson(cdp, 'api/multi/economy/executor', {
    fence: fenceOf(reV), kind: 'ClearUndergrowth', areaId: 'MA-A', targetKey: `${clearCol},${clearRow}`, executor: 'Landowner',
  })).payload.view;
  await click(cdp, '#reread');
  await waitFor(cdp, "document.querySelectorAll('[data-disposition-value]').length === 9", 'Re-queued 3 Fell orders after cancel');
  await click(cdp, keepSel);
  await waitFor(cdp, `document.querySelector('${keepSel}')?.getAttribute('aria-pressed') === 'true'`, 'Re-applied KeepForUse');
  await click(cdp, leaveSel);
  await waitFor(cdp, `document.querySelector('${leaveSel}')?.getAttribute('aria-pressed') === 'true'`, 'Re-applied LeaveAsDeadwood');
  await click(cdp, '#approve');
  await waitFor(cdp, "!document.getElementById('approved-box').hidden && document.getElementById('economy-quote-badge').textContent.includes('Frozen')",
    'Re-approved Work Plan with fresh server quote');

  // Execute via UI button -> confirm dialog
  await click(cdp, '#execute');
  await waitFor(cdp, "document.getElementById('execute-dialog').open && document.getElementById('execute-economy-summary').textContent.includes('Retained timber added')",
    'Execute confirmation dialog shows frozen retained timber & deadwood summary');
  await click(cdp, '#execute-confirm');
  await waitFor(cdp, "document.getElementById('approved-box').hidden && document.getElementById('receipt').textContent.includes('Retained timber added')",
    'Execute committed with retained timber & fallen deadwood');
  await sleep(220);

  // Verify top strip RetainedTimberM3, Cash, Hours, Stage4 fallen deadwood
  const econAfterExec = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  const laneAAfterExec = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-A')).payload;
  const deadwoodAfter = laneAAfterExec.stage4.areas[0].canopy.fallenDeadwoodCount;
  const addedDw = laneAAfterExec.stage4.areas[0].canopy.fallenDeadwood.find((d) => d.treeId === tLeave);
  const stripAfter = await evaluate(cdp, `({
    cash: document.getElementById('economy-cash').textContent.trim(),
    hours: document.getElementById('economy-hours').textContent.trim(),
    retained: document.getElementById('economy-retained').textContent.trim(),
    stage: document.getElementById('economy-stage').textContent.trim(),
  })`);
  check(stripAfter.retained === econAfterExec.retainedTimber.volumeDisplay &&
    econAfterExec.retainedTimber.retainedTimberCm3 > 0 &&
    stripAfter.cash === econAfterExec.cashBalanceDisplay &&
    stripAfter.hours === '39.8 / 40 hrs' &&
    stripAfter.stage.includes('Second thinning'),
    `Post-Execute strip updated: Cash=${stripAfter.cash}, Hours=${stripAfter.hours}, Retained=${stripAfter.retained}, Stage=${stripAfter.stage}`);
  check(deadwoodAfter === deadwoodBefore + 1 && Boolean(addedDw) && addedDw.cause === 'fell' && /^DW\d{4}$/.test(addedDw.deadwoodId),
    `LeaveAsDeadwood materialised Stage 4 deadwood (${deadwoodBefore}->${deadwoodAfter}, ${addedDw?.deadwoodId} cause=${addedDw?.cause})`);
  await shot(cdp, 'after-execute-retained-deadwood');

  // 7. Existing History drawer -> Year 0 -> verify Financial Ledger + Retained timber + Felling dispositions table
  await click(cdp, '#history-open');
  await waitFor(cdp, "document.querySelector('[data-history-year=\"0\"]') !== null", 'Year 0 in History drawer');
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.querySelector('[data-history-section=\"financial-ledger\"]')?.textContent.includes('Felling dispositions')",
    'History drawer Financial Ledger shows Felling dispositions');
  const histCheck = await evaluate(cdp, `(() => {
    const fin = document.querySelector('[data-history-section="financial-ledger"]');
    const txt = fin ? fin.textContent : '';
    return {
      hasRetained: txt.includes('Retained timber added') && txt.includes(${JSON.stringify(econAfterExec.retainedTimber.volumeDisplay)}),
      hasSell: txt.includes('Sell & Extract'),
      hasKeep: txt.includes('Keep for Own Use'),
      hasLeave: txt.includes('Leave as Deadwood'),
      histBtnCount: document.querySelectorAll('#history-open').length,
    };
  })()`);
  check(histCheck.hasRetained && histCheck.hasSell && histCheck.hasKeep && histCheck.hasLeave && histCheck.histBtnCount === 1,
    `Single History drawer shows Year 0 dispositions & engine-graded retained m³ (${JSON.stringify(histCheck)})`);
  await shot(cdp, 'history-dispositions');
  await click(cdp, '#history-close');
  await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'History drawer closed');

  // 8. Save to Save v3 slot via UI
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open", 'Saved games dialog opened');
  await typeInto(cdp, '#save-slot', slotName);
  const saveEnabled = await evaluate(cdp, "!document.getElementById('save-do').disabled");
  check(saveEnabled, `Save button enabled for Forest Maker Money-On session (Save v3)`);
  await click(cdp, '#save-do');
  await waitFor(cdp, `document.getElementById('saves-status').textContent.includes(${JSON.stringify('Saved "' + slotName + '"')}) && document.getElementById('saves-rows').textContent.includes(${JSON.stringify(slotName)})`,
    `Save v3 slot ${slotName} committed and listed in table`);
  const slotRowText = await evaluate(cdp, "document.getElementById('saves-rows').textContent");
  check(slotRowText.includes(slotName) && slotRowText.includes('v3') && slotRowText.includes('Money'),
    `Saved games table lists ${slotName} with v3 & Money tags`);
  await shot(cdp, 'saves-v3-committed');
  await click(cdp, '#saves-close');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open", 'Saved games dialog closed');

  // Persist expected values for the post-restart Load verification phase
  writeFileSync(expectedPath, JSON.stringify({
    cashBalanceCents: econAfterExec.cashBalanceCents,
    cashBalanceDisplay: econAfterExec.cashBalanceDisplay,
    remainingLandownerMinutes: econAfterExec.remainingLandownerMinutes,
    landownerHoursDisplay: econAfterExec.landownerHoursDisplay,
    retainedTimberCm3: econAfterExec.retainedTimber.retainedTimberCm3,
    retainedTimberDisplay: econAfterExec.retainedTimber.volumeDisplay,
    bySpecies: econAfterExec.retainedTimber.bySpecies,
    executedHarvestPlansCount: econAfterExec.executedHarvestPlansCount,
    nextHarvestStage: econAfterExec.nextHarvestStage,
    settlementsCount: econAfterExec.latestSettlement ? 1 : 0,
    deadwoodCount: deadwoodAfter,
    deadwoodId: addedDw.deadwoodId,
    deadwoodTreeId: tLeave,
  }, null, 2));

  await checkLayout(cdp, 'Post-Execute & Save v3');
  audit.collect();
  check(audit.consoleIssues.length === 0 && audit.cspViolations.length === 0 && audit.badNetwork.length === 0,
    `Save phase zero console/CSP/404/5xx (console=${audit.consoleIssues.length}, csp=${audit.cspViolations.length}, net=${audit.badNetwork.length})`);
}

async function runLoadPhase(cdp) {
  const expected = JSON.parse(readFileSync(expectedPath, 'utf8'));
  const audit = makeAuditor(cdp);
  audit.collect(true);
  check(audit.getInitialOk() >= 16 && audit.badNetwork.length === 0 && audit.consoleIssues.length === 0 && audit.cspViolations.length === 0,
    `Fresh restarted server loaded cleanly (${audit.getInitialOk()} HTTP 200 assets)`);

  await setViewport(cdp, width, height);
  // Open Saved games dialog on the fresh server (starts Money Off, built-in MA-1-MIXED)
  await click(cdp, '#saves-open');
  await waitFor(cdp, `document.getElementById('saves-dialog').open && document.querySelector('[data-load="${slotName}"]') !== null`,
    `Fresh server lists v3 Economy slot ${slotName} as loadable`);
  await click(cdp, `[data-load="${slotName}"]`);
  await waitFor(cdp, "!document.getElementById('load-confirm').hidden", 'Load confirmation shown');
  await click(cdp, '#load-do');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open && document.getElementById('economy-toggle-label').textContent.trim() === 'Money: On'",
    'Save v3 Economy woodland restored after server restart');
  await sleep(300);

  const restoredStrip = await evaluate(cdp, `({
    cash: document.getElementById('economy-cash').textContent.trim(),
    hours: document.getElementById('economy-hours').textContent.trim(),
    retained: document.getElementById('economy-retained').textContent.trim(),
    stage: document.getElementById('economy-stage').textContent.trim(),
  })`);
  const restoredEcon = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  const restoredLaneA = (await fetchJson(cdp, 'api/multi/stand?lanes=MA-A')).payload;
  const restoredDw = restoredLaneA.stage4.areas[0].canopy.fallenDeadwood.find((d) => d.treeId === expected.deadwoodTreeId);

  check(restoredStrip.cash === expected.cashBalanceDisplay &&
    restoredStrip.hours === expected.landownerHoursDisplay &&
    restoredStrip.retained === expected.retainedTimberDisplay &&
    restoredStrip.stage.includes('Second thinning'),
    `Post-restart UI strip restored identically (Cash=${restoredStrip.cash}, Hours=${restoredStrip.hours}, Retained=${restoredStrip.retained})`);
  check(restoredEcon.cashBalanceCents === expected.cashBalanceCents &&
    restoredEcon.remainingLandownerMinutes === expected.remainingLandownerMinutes &&
    restoredEcon.retainedTimber.retainedTimberCm3 === expected.retainedTimberCm3 &&
    JSON.stringify(restoredEcon.retainedTimber.bySpecies) === JSON.stringify(expected.bySpecies) &&
    restoredEcon.executedHarvestPlansCount === expected.executedHarvestPlansCount,
    `Post-restart server economy state matches bit-for-bit (cash=${restoredEcon.cashBalanceCents}, retainedCm3=${restoredEcon.retainedTimber.retainedTimberCm3}, stageCount=${restoredEcon.executedHarvestPlansCount})`);
  check(restoredLaneA.stage4.areas[0].canopy.fallenDeadwoodCount === expected.deadwoodCount &&
    Boolean(restoredDw) && restoredDw.deadwoodId === expected.deadwoodId && restoredDw.cause === 'fell',
    `Post-restart Stage 4 deadwood matches (${restoredDw?.deadwoodId}, count=${restoredLaneA.stage4.areas[0].canopy.fallenDeadwoodCount})`);

  // Verify History drawer after restart still shows Financial Ledger + dispositions + retained m³
  await click(cdp, '#history-open');
  await waitFor(cdp, "document.querySelector('[data-history-year=\"0\"]') !== null", 'Year 0 in History drawer after restart');
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.querySelector('[data-history-section=\"financial-ledger\"]')?.textContent.includes('Felling dispositions')",
    'History drawer Financial Ledger restored after restart');
  const histAfter = await evaluate(cdp, `(() => {
    const txt = document.querySelector('[data-history-section="financial-ledger"]').textContent;
    return txt.includes(${JSON.stringify(expected.retainedTimberDisplay)}) &&
      txt.includes('Sell & Extract') && txt.includes('Keep for Own Use') && txt.includes('Leave as Deadwood');
  })()`);
  check(histAfter, 'Post-restart History drawer retains Financial Ledger, dispositions & retained m³');
  await shot(cdp, 'restored-history-after-restart');
  await click(cdp, '#history-close');
  await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'History drawer closed after restart');

  await checkLayout(cdp, 'Restored After Server Restart');
  audit.collect();
  check(audit.consoleIssues.length === 0 && audit.cspViolations.length === 0 && audit.badNetwork.length === 0,
    `Load phase zero console/CSP/404/5xx (console=${audit.consoleIssues.length}, csp=${audit.cspViolations.length}, net=${audit.badNetwork.length})`);
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
    await sleep(300);
    if (mode === 'load') await runLoadPhase(cdp);
    else await runSavePhase(cdp);
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
