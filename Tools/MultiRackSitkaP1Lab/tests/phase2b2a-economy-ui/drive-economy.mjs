// CCF Multi 2D — Phase 2B-2a Economy UI real-browser acceptance driver (Microsoft Edge over CDP).
// Exercises the full Phase 2B-2a contract at the requested viewport (1920x1080 or 1280x720) with
// PID-scoped Edge lifecycle, scratch user-data-dir, and strict console/CSP/404/5xx auditing.
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
const tag = opt('tag', 'vp');
const width = Number(opt('width', '1280'));
const height = Number(opt('height', '720'));
const experiment = opt('experiment', 'ma-1-mixed');
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
  const profile = join(tmpdir(), 'ccf-econ-edge-' + process.pid + '-' + Date.now());
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

async function shot(cdp, name, w = width, h = height) {
  await setViewport(cdp, w, h);
  await sleep(180);
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

async function checkLayout(cdp, label) {
  const geom = await evaluate(cdp, `(() => {
    const noHScroll = document.documentElement.scrollWidth <= window.innerWidth + 1;
    const topbar = document.querySelector('.topbar');
    const actions = Array.from(document.querySelectorAll('.top-actions > button'));
    const rects = actions.map((b) => { const r = b.getBoundingClientRect(); return { id: b.id, top: Math.round(r.top), right: Math.round(r.right) }; });
    const maxTopDelta = rects.length ? Math.max(...rects.map((r) => r.top)) - Math.min(...rects.map((r) => r.top)) : 0;
    const allInWidth = rects.every((r) => r.right <= window.innerWidth + 1);
    const badge = document.getElementById('experiment-badge');
    const badgeRect = badge ? badge.getBoundingClientRect() : null;
    const badgeUnclipped = Boolean(badge && badgeRect && badgeRect.width > 0 && badge.scrollWidth <= badge.clientWidth + 1 && badgeRect.right <= window.innerWidth + 1);
    const badgeText = badge ? badge.textContent.trim() : '';
    const wsCols = getComputedStyle(document.getElementById('workspace')).gridTemplateColumns.split(' ').filter(Boolean).length;
    const histBtns = document.querySelectorAll('#history-open').length;
    const strip = document.getElementById('economy-strip');
    const stripRect = strip && !strip.hidden ? strip.getBoundingClientRect() : null;
    const stripItemsUnclipped = !stripRect || Array.from(strip.children).every((ch) => {
      const r = ch.getBoundingClientRect();
      return r.right <= window.innerWidth + 1 && ch.scrollWidth <= ch.clientWidth + 1;
    });
    const stripFits = !stripRect || (stripRect.right <= window.innerWidth + 1 && stripItemsUnclipped);
    return { noHScroll, scrollW: document.documentElement.scrollWidth, innerW: window.innerWidth, topbarH: Math.round(topbar.getBoundingClientRect().height), maxTopDelta, allInWidth, badgeUnclipped, badgeText, wsCols, histBtns, stripFits };
  })()`);
  const needsCaveat = experiment === 'ma-1-mixed';
  const caveatOk = !needsCaveat || geom.badgeText.includes('not a surveyed Irish forecast');
  check(geom.noHScroll && geom.maxTopDelta <= 10 && geom.allInWidth && geom.badgeUnclipped && caveatOk && geom.wsCols <= 3 && geom.histBtns === 1 && geom.stripFits,
    `layout (${label}): zero h-scroll (${geom.scrollW}<=${geom.innerW}), single-row topbar (delta=${geom.maxTopDelta}px), badge unclipped (${geom.badgeUnclipped}), ${geom.wsCols} cols (<=3), 1 History button`);
}

const deliberateRefusal = (entry) => entry.source === 'network' && /status of (400|409)/.test(entry.text || '');

async function runSuite(cdp) {
  const badNetwork = [];
  const consoleIssues = [];
  const cspViolations = [];
  let initialAssetOkCount = 0;

  const collectEvents = (isInitial = false) => {
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

  // Capture initial page-load network/console events recorded since BEFORE Page.navigate
  collectEvents(true);
  check(initialAssetOkCount >= 16 && badNetwork.length === 0 && consoleIssues.length === 0 && cspViolations.length === 0,
    `Initial page load captured before navigation (${initialAssetOkCount} HTTP 200 assets/modules, console=${consoleIssues.length}, csp=${cspViolations.length}, net=${badNetwork.length})`);

  // ---- 1. Money Off default: unchanged, economy omitted, History drawer FinancialLedger: NOT RECORDED ----
  await setViewport(cdp, width, height);
  await checkLayout(cdp, 'Money Off');

  const offState = await evaluate(cdp, `({
    toggleLabel: document.getElementById('economy-toggle-label').textContent.trim(),
    togglePressed: document.getElementById('economy-toggle').getAttribute('aria-pressed'),
    stripHidden: document.getElementById('economy-strip').hidden,
    quoteHidden: document.getElementById('economy-quote-panel').hidden,
  })`);
  check(offState.toggleLabel === 'Money: Off' && offState.togglePressed === 'false' && offState.stripHidden && offState.quoteHidden,
    `Money defaults to Off (strip & quote hidden: ${JSON.stringify(offState)})`);

  const offViewRes = await fetchJson(cdp, 'api/multi/view');
  check(offViewRes.status === 200 && offViewRes.payload?.economyEnabled === false && !('economy' in offViewRes.payload),
    'GET api/multi/view emits explicit economyEnabled=false and omits "economy" when Money is Off');

  const offEconViewRes = await fetchJson(cdp, 'api/multi/economy/view');
  check(offEconViewRes.status === 409 && offEconViewRes.payload?.refusal?.code === 'EconomyModelRequired',
    'GET api/multi/economy/view refuses with 409 EconomyModelRequired when Money is Off');

  // Advance 1 Year while Off so Year 0 is recorded, then verify History drawer shows FinancialLedger: NOT RECORDED + zero currency text
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1'", 'Advance to Year 1 while Off');
  await click(cdp, '#history-open');
  await waitFor(cdp, "document.querySelector('[data-history-year=\"0\"]') !== null", 'Year 0 in History index (Off)');
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.getElementById('history-detail-pane').textContent.includes('Not Recorded in 2D History')",
    'Year 0 detail loaded while Off');
  const offHistAudit = await evaluate(cdp, `(() => {
    const pane = document.getElementById('history-detail-pane');
    const txt = pane.textContent;
    const hasFinCard = Boolean(pane.querySelector('[data-history-section="financial-ledger"]'));
    const hasFinNotRecorded = txt.includes('FinancialLedger') && txt.includes('NOT RECORDED');
    const hasEuroGlyph = txt.includes('€');
    return { hasFinCard, hasFinNotRecorded, hasEuroGlyph };
  })()`);
  check(!offHistAudit.hasFinCard && offHistAudit.hasFinNotRecorded && !offHistAudit.hasEuroGlyph,
    `History drawer while Money Off keeps FinancialLedger NOT RECORDED with zero € glyph (${JSON.stringify(offHistAudit)})`);
  await click(cdp, '#history-close');
  await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'History drawer closed');

  // ---- 2. Launch-time Money On toggle with dirty-progress confirmation (Year 1 -> fresh Year 0) ----
  const dirtyView = (await fetchJson(cdp, 'api/multi/view')).payload.view;
  const dirtyFence = fenceOf(dirtyView);
  const unconfirmedToggle = await fetchJson(cdp, 'api/multi/economy/session', {
    fence: dirtyFence, enabled: true, confirmReset: false,
  });
  check(unconfirmedToggle.status === 409 && unconfirmedToggle.payload?.refusal?.code === 'EconomyResetConfirmationRequired',
    'Server refuses dirty Money toggle without confirmReset=true (409 EconomyResetConfirmationRequired)');

  // Now toggle via UI button -> dirty confirmation dialog -> confirm
  await click(cdp, '#economy-toggle');
  await waitFor(cdp, "document.getElementById('economy-toggle-dialog').open", 'Money toggle confirmation dialog');
  const dlgInfo = await evaluate(cdp, `({
    warnHidden: document.getElementById('economy-toggle-warning').hidden,
    warnText: document.getElementById('economy-toggle-warning').textContent,
    confirmText: document.getElementById('economy-toggle-confirm').textContent,
  })`);
  check(!dlgInfo.warnHidden && dlgInfo.warnText.includes('Year 1') && dlgInfo.warnText.includes('Unsaved progress'),
    `Dirty-progress confirmation warning shown before resetting (${dlgInfo.warnText})`);
  await shot(cdp, 'toggle-confirm-dialog');

  await click(cdp, '#economy-toggle-confirm');
  await waitFor(cdp, "document.getElementById('economy-toggle-label').textContent.trim() === 'Money: On' && document.getElementById('year-value').textContent.trim() === '0'",
    'Fresh Year-0 session with Money On');
  await sleep(200);

  // Stale fence from prior Off session must be refused
  const staleCheck = await fetchJson(cdp, 'api/multi/economy/quote', { fence: dirtyFence });
  check(staleCheck.status === 409 && staleCheck.payload?.refusal?.code === 'PlayerSessionMismatch',
    'Stale pre-toggle fence is refused with 409 PlayerSessionMismatch after Money On swap');

  await checkLayout(cdp, 'Money On Year 0');
  const onStrip = await evaluate(cdp, `({
    stripHidden: document.getElementById('economy-strip').hidden,
    cash: document.getElementById('economy-cash').textContent.trim(),
    hours: document.getElementById('economy-hours').textContent.trim(),
    stage: document.getElementById('economy-stage').textContent.trim(),
  })`);
  check(!onStrip.stripHidden && onStrip.cash === '€12,000.00' && onStrip.hours === '40 / 40 hrs' && /€21(\.00)?\/t/.test(onStrip.stage),
    `Top strip displays server Cash & Landowner hours (${JSON.stringify(onStrip)})`);

  // Saved games dialog surfaces Save v3 deferral cleanly (zero 500s)
  await click(cdp, '#saves-open');
  await waitFor(cdp, "document.getElementById('saves-dialog').open && document.getElementById('saves-status').textContent.includes('SaveEconomyRequiresV3')",
    'Saved games dialog shows SaveEconomyRequiresV3');
  const saveDlg = await evaluate(cdp, `({
    status: document.getElementById('saves-status').textContent,
    saveDisabled: document.getElementById('save-do').disabled,
  })`);
  check(saveDlg.status.includes('SaveEconomyRequiresV3') && saveDlg.status.includes('Save v3') && saveDlg.saveDisabled,
    `Saved games dialog surfaces Save v3 deferral cleanly (${saveDlg.status})`);
  await click(cdp, '#saves-close');
  await waitFor(cdp, "!document.getElementById('saves-dialog').open", 'Saved games dialog closed');

  // ---- 3. Queue Fell + non-harvest orders, verify quote, €2,500 floor, D1b honesty, Contractor-only Fell, Landowner toggle ----
  const v0Res = await fetchJson(cdp, 'api/multi/view');
  const v0 = v0Res.payload.view;
  check(v0Res.payload?.economyEnabled === true && v0.source.areaModelVersion.includes('+economy-v1'),
    `Server emits economyEnabled=true and AreaModelVersion carries +economy-v1 (${v0.source.areaModelVersion})`);

  // Queue 1 living tree in MA-A for Fell + 1 ClearUndergrowth cell (and on ma-1-mixed, a Plant + Shelter!)
  const livingA = v0.areas[0].trees.filter((t) => t.isLiving);
  let curV = (await fetchJson(cdp, 'api/multi/pending/add', {
    fence: fenceOf(v0), areaId: 'MA-A', treeId: livingA[0].treeId, reason: 'Phase 2B-2a economy UI test fell',
  })).payload.view;

  // Test server refusal for Fell -> Landowner (EconomyFellContractorOnly)
  const fellLandowner = await fetchJson(cdp, 'api/multi/economy/executor', {
    fence: fenceOf(curV), kind: 'Fell', areaId: 'MA-A', targetKey: livingA[0].treeId, executor: 'Landowner',
  });
  check(fellLandowner.status === 409 && fellLandowner.payload?.refusal?.code === 'EconomyFellContractorOnly',
    'Server refuses assigning Fell to Landowner with 409 EconomyFellContractorOnly');

  // Queue one ClearUndergrowth cell in MA-A (12 min = 0.2 hrs)
  const cell0 = v0Res.payload.stage4.areas[0].ground[0];
  curV = (await fetchJson(cdp, 'api/multi/clear/add', {
    fence: fenceOf(curV), areaId: 'MA-A', column: cell0.column, row: cell0.row,
  })).payload.view;

  // On ma-1-mixed, also queue an Oak Plant + Shelter to verify on-demand materials (€5.50 + €5.00 = €10.50)
  let plantedOrder = false;
  if (experiment === 'ma-1-mixed') {
    const extB = curV.areas[1].extent;
    const liveAll = curV.areas.flatMap((a) => a.trees.filter((t) => t.isLiving));
    for (const c of v0Res.payload.stage4.areas[1].ground) {
      const x = extB.minXM + c.centreU;
      const z = extB.minZM + extB.sideM - c.centreV;
      if (liveAll.every((t) => (t.xm - x) ** 2 + (t.zm - z) ** 2 >= 2.0)) {
        const pr = await fetchJson(cdp, 'api/multi/plant/add', {
          fence: fenceOf(curV), areaId: 'MA-B', speciesId: 'sessile-oak', xM: x, zM: z,
        });
        if (pr.status === 200) {
          curV = pr.payload.view;
          const sr = await fetchJson(cdp, 'api/multi/shelter/add', {
            fence: fenceOf(curV), areaId: 'MA-B', xM: x, zM: z,
          });
          if (sr.status === 200) curV = sr.payload.view;
          plantedOrder = true;
          break;
        }
      }
    }
  }

  // Re-read in the page and open the Work Plan tab
  await click(cdp, '#reread');
  await waitFor(cdp, "!document.getElementById('economy-quote-panel').hidden && document.getElementById('economy-quote-body').textContent.includes('Contractor harvest fee')",
    'Work Plan quote populated');
  await click(cdp, '[data-pane-button="plan"]');
  await waitFor(cdp, "document.getElementById('side').getAttribute('data-pane') === 'plan'", 'Work Plan tab active');

  // Verify quote UI, €2,500 floor highlight, Contractor-only Fell badge + tooltip, D1b no-disposition honesty, calm disclaimer
  const quoteAudit = await evaluate(cdp, `(() => {
    const panel = document.getElementById('economy-quote-panel');
    const txt = panel.textContent;
    const floorEl = panel.querySelector('[data-floor-applied="true"]');
    const fellLock = document.querySelector('#pending-areas .exec-locked');
    const disc = document.getElementById('economy-disclaimer').textContent;
    const hasKeepForUse = /KeepForUse|retained-timber|disposition selector/i.test(txt);
    const hasClearfellOrOverride = /clearfell|stage override/i.test(txt);
    const approveDisabled = document.getElementById('approve').disabled;
    return {
      hasFloorHighlight: Boolean(floorEl && floorEl.textContent.includes('€2,500')),
      hasFellLock: Boolean(fellLock && fellLock.textContent.includes('Contractor only') && (fellLock.getAttribute('title') || '').includes('contractor-only')),
      hasDisclaimer: disc.includes('Calibrated teaching estimates') && disc.includes('net a loss') && disc.includes('No automatic grants') && disc.includes('sell-and-extract'),
      hasMaterials: txt.includes('Materials & saplings'),
      hasTimberIncome: txt.includes('Expected timber income'),
      hasKeepForUse,
      hasClearfellOrOverride,
      approveDisabled,
    };
  })()`);
  check(quoteAudit.hasFloorHighlight && quoteAudit.hasFellLock && quoteAudit.hasDisclaimer &&
    quoteAudit.hasMaterials && quoteAudit.hasTimberIncome && !quoteAudit.hasKeepForUse &&
    !quoteAudit.hasClearfellOrOverride && !quoteAudit.approveDisabled,
    `Quote panel verified (€2,500 floor, contractor-only Fell tooltip, disclaimer, D1b no-dispositions, no clearfell: ${JSON.stringify(quoteAudit)})`);

  // Assign ClearUndergrowth to Landowner via real UI button click!
  const clearTarget = `${cell0.column},${cell0.row}`;
  const landownerBtnSel = `[data-executor-kind="ClearUndergrowth"][data-executor-target="${clearTarget}"][data-executor-value="Landowner"]`;
  await click(cdp, landownerBtnSel);
  await waitFor(cdp, `document.querySelector('${landownerBtnSel}')?.getAttribute('aria-pressed') === 'true' && document.getElementById('economy-quote-body').textContent.includes('12 min')`,
    'ClearUndergrowth assigned to Landowner (12 min)');
  await shot(cdp, 'work-plan-orders-executors');
  await evaluate(cdp, "document.getElementById('pane-plan').scrollTop = 0");
  await sleep(100);
  await shot(cdp, 'work-plan-quote-pending');

  // For ma1-check we stop after verifying layout at 1920x1080 too
  if (tag === 'ma1-check') {
    await setViewport(cdp, 1920, 1080);
    await checkLayout(cdp, 'MA-1 Money On 1920x1080');
    await setViewport(cdp, 1280, 720);
    await checkLayout(cdp, 'MA-1 Money On 1280x720');
    collectEvents();
    check(consoleIssues.length === 0 && cspViolations.length === 0 && badNetwork.length === 0,
      `Zero console/CSP/404/5xx issues (console=${consoleIssues.length}, csp=${cspViolations.length}, net=${badNetwork.length})`);
    return;
  }

  // ---- 4. Approve -> verify frozen quote + executor locked -> Execute -> verify atomic settlement + hours deduction + replay ----
  await click(cdp, '#approve');
  await waitFor(cdp, "!document.getElementById('approved-box').hidden && document.getElementById('economy-quote-badge').textContent.includes('Frozen')",
    'Work Plan approved with frozen quote');

  const approvedView = (await fetchJson(cdp, 'api/multi/view')).payload.view;
  const approvedFence = fenceOf(approvedView);
  const approvedPlanId = approvedView.approved.planId;
  const approvedExecKey = approvedView.approved.executionKey;

  // Editing executor while approved must be refused (WorkPlanAlreadyApproved)
  const editWhileApproved = await fetchJson(cdp, 'api/multi/economy/executor', {
    fence: approvedFence, kind: 'ClearUndergrowth', areaId: 'MA-A', targetKey: clearTarget, executor: 'Contractor',
  });
  check(editWhileApproved.status === 409 && editWhileApproved.payload?.refusal?.code === 'WorkPlanAlreadyApproved',
    'Executor edit while approved refused with 409 WorkPlanAlreadyApproved');

  // Execute via UI button -> Execute confirmation dialog -> Confirm
  await click(cdp, '#execute');
  await waitFor(cdp, "document.getElementById('execute-dialog').open && !document.getElementById('execute-economy-summary').hidden",
    'Execute confirmation dialog with frozen financial summary');
  await click(cdp, '#execute-confirm');
  await waitFor(cdp, "document.getElementById('approved-box').hidden && document.getElementById('receipt').textContent.includes('Financial settlement')",
    'Execute committed with atomic financial settlement');
  await sleep(180);

  const afterExec = await evaluate(cdp, `({
    cash: document.getElementById('economy-cash').textContent.trim(),
    hours: document.getElementById('economy-hours').textContent.trim(),
    stage: document.getElementById('economy-stage').textContent.trim(),
    receipt: document.getElementById('receipt').textContent,
  })`);
  check(afterExec.cash !== '€12,000.00' && afterExec.hours === '39.8 / 40 hrs' && afterExec.stage.includes('Second thinning'),
    `Atomic settlement updated Cash (${afterExec.cash}), deducted Landowner hours (${afterExec.hours}), and advanced thinning stage (${afterExec.stage})`);
  await shot(cdp, 'after-execute-settlement');

  // Replay exact Execute request -> must return stored receipt with ZERO double charge!
  const beforeReplay = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  const replayRes = await fetchJson(cdp, 'api/multi/plan/execute', {
    fence: approvedFence, planId: approvedPlanId, executionKey: approvedExecKey,
  });
  const afterReplay = (await fetchJson(cdp, 'api/multi/economy/view')).payload.economy;
  check(replayRes.status === 200 && replayRes.payload?.execution?.replayed === true &&
    afterReplay.cashBalanceCents === beforeReplay.cashBalanceCents &&
    afterReplay.remainingLandownerMinutes === beforeReplay.remainingLandownerMinutes,
    `Execute replay returned stored receipt with zero double charge (cash=${afterReplay.cashBalanceDisplay}, hours=${afterReplay.landownerHoursDisplay})`);

  // ---- 5. Advance 1 Year -> verify 40 / 40 hrs reset + History drawer Financial Ledger rows ----
  await click(cdp, '#advance');
  await waitFor(cdp, "document.getElementById('year-value').textContent.trim() === '1' && document.getElementById('economy-hours').textContent.trim() === '40 / 40 hrs'",
    'Advance 1 Year reset Landowner hours to 40 / 40 hrs');
  const afterAdv = await evaluate(cdp, `({
    year: document.getElementById('year-value').textContent.trim(),
    hours: document.getElementById('economy-hours').textContent.trim(),
  })`);
  const afterAdvEcon = (await fetchJson(cdp, 'api/multi/economy/view')).payload?.economy;
  check(afterAdv.year === '1' && afterAdv.hours === '40 / 40 hrs' &&
    afterAdvEcon?.remainingLandownerMinutes === afterAdvEcon?.annualLandownerAllowanceMinutes &&
    afterAdvEcon?.usedLandownerMinutesThisYear === 0,
    `Advance 1 Year reset Landowner allowance back to 40 / 40 hrs (ui=${afterAdv.hours}, server=${afterAdvEcon?.remainingLandownerMinutes}/${afterAdvEcon?.annualLandownerAllowanceMinutes} min)`);

  // Open single History drawer -> Year 0 -> verify Financial Ledger card + rows
  await click(cdp, '#history-open');
  await waitFor(cdp, "document.querySelector('[data-history-year=\"0\"]') !== null", 'Year 0 in History drawer (On)');
  await click(cdp, '[data-history-year="0"]');
  await waitFor(cdp, "document.querySelector('[data-history-section=\"financial-ledger\"]') !== null",
    'Financial Ledger section rendered inside existing History drawer');
  const onHistAudit = await evaluate(cdp, `(() => {
    const fin = document.querySelector('[data-history-section="financial-ledger"]');
    const txt = fin ? fin.textContent : '';
    const rows = fin ? fin.querySelectorAll('tbody tr').length : 0;
    return {
      hasTitle: txt.includes('Financial Ledger — Year 0'),
      hasRecordedPill: txt.includes('RECORDED'),
      hasFloorEntry: txt.includes('Minimum job floor'),
      hasCashDelta: txt.includes('€'),
      rowCount: rows,
    };
  })()`);
  check(onHistAudit.hasTitle && onHistAudit.hasRecordedPill && onHistAudit.hasFloorEntry && onHistAudit.hasCashDelta && onHistAudit.rowCount >= 2,
    `History drawer shows recorded Financial Ledger rows while Money is On (${JSON.stringify(onHistAudit)})`);
  await evaluate(cdp, "document.querySelector('[data-history-section=\"financial-ledger\"]')?.scrollIntoView({ block: 'start' })");
  await sleep(100);
  await shot(cdp, 'history-drawer-money-on');
  await click(cdp, '#history-close');
  await waitFor(cdp, "document.getElementById('history-drawer').hidden", 'History drawer closed (On)');

  // ---- 6. Upfront-cash block (drain cash below €2,500 via separate small 1-tree felling plans) ----
  let drainRes = await fetchJson(cdp, 'api/multi/view');
  let drainV = drainRes.payload.view;
  let drainEcon = drainRes.payload.economy;
  let drainIdx = 1;
  while (drainEcon.cashBalanceCents >= 250000 && drainIdx < 12) {
    const liveTrees = drainV.areas[0].trees.filter((t) => t.isLiving);
    const pick = liveTrees[drainIdx % liveTrees.length];
    drainIdx++;
    const addR = await fetchJson(cdp, 'api/multi/pending/add', {
      fence: fenceOf(drainV), areaId: 'MA-A', treeId: pick.treeId, reason: 'drain floor plan ' + drainIdx,
    });
    if (addR.status !== 200) continue;
    drainV = addR.payload.view;
    const appR = await fetchJson(cdp, 'api/multi/plan/approve', { fence: fenceOf(drainV) });
    if (appR.status !== 200) break;
    drainV = appR.payload.view;
    const exR = await fetchJson(cdp, 'api/multi/plan/execute', {
      fence: fenceOf(drainV), planId: drainV.approved.planId, executionKey: drainV.approved.executionKey,
    });
    if (exR.status !== 200) break;
    drainV = exR.payload.view;
    drainEcon = exR.payload.economy;
  }
  check(drainEcon.cashBalanceCents < 250000 && drainEcon.cashBalanceCents >= 0,
    `Cash reduced below €2,500 floor without ever going negative (${drainEcon.cashBalanceDisplay})`);

  // Queue another Fell tree -> upfront cash €2,500 > available -> UI blocks Approve + plain-English explanation + server 409 EconomyInsufficientCash
  const blockedTree = drainV.areas[0].trees.find((t) => t.isLiving);
  const blockedAdd = await fetchJson(cdp, 'api/multi/pending/add', {
    fence: fenceOf(drainV), areaId: 'MA-A', treeId: blockedTree.treeId, reason: 'upfront cash block probe',
  });
  drainV = blockedAdd.payload.view;

  // Server must refuse Approve with 409 EconomyInsufficientCash
  const blockedApprove = await fetchJson(cdp, 'api/multi/plan/approve', { fence: fenceOf(drainV) });
  check(blockedApprove.status === 409 && blockedApprove.payload?.refusal?.code === 'EconomyInsufficientCash',
    'Server refuses Approve with 409 EconomyInsufficientCash when upfront cash < required');

  // Re-read in UI and verify Approve is disabled + plain-English explanation visible
  await click(cdp, '#reread');
  await click(cdp, '[data-pane-button="plan"]');
  await waitFor(cdp, "document.getElementById('economy-verdict').textContent.includes('Insufficient upfront cash')",
    'Upfront-cash block verdict shown in Work Plan');
  const blockUi = await evaluate(cdp, `({
    approveDisabled: document.getElementById('approve').disabled,
    verdictClass: document.getElementById('economy-verdict').className,
    verdictText: document.getElementById('economy-verdict').textContent,
  })`);
  check(blockUi.approveDisabled && blockUi.verdictClass.includes('econ-verdict-blocked') &&
    blockUi.verdictText.includes('cannot pre-pay') && blockUi.verdictText.includes('no borrowing'),
    `UI blocks Approve with plain-English upfront-cash explanation (${blockUi.verdictText})`);
  await evaluate(cdp, "document.getElementById('pane-plan').scrollTop = 0");
  await sleep(100);
  await shot(cdp, 'upfront-cash-blocked');
  await checkLayout(cdp, 'Money On Blocked Quote');

  // ---- 7. Console, CSP, 404, 5xx audit ----
  collectEvents();
  check(consoleIssues.length === 0 && cspViolations.length === 0 && badNetwork.length === 0,
    `Zero console errors, CSP violations, 404s, or 5xx responses (console=${consoleIssues.length}, csp=${cspViolations.length}, net=${badNetwork.length})`);
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
    await runSuite(cdp);
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
