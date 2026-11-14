// Tier 2 real-browser qualification for the Synthetic Sitka lab (P0).
// Starts the real host on a free loopback port, drives a real headless Chromium through DevTools input events
// (real mouse clicks, wheel, drag), compares what the page DISPLAYS with what the Core API RETURNS, captures
// real screenshots, measures the 2,000-tree interaction latency and records every result as JSON.
//
//   node tests/Ui/run-browser-tests.mjs            (after: dotnet build src/Host)
//   EVIDENCE_DIR=/path  CHROMIUM=/usr/bin/chromium  node tests/Ui/run-browser-tests.mjs
import { spawn, execFileSync } from 'node:child_process';
import fs from 'node:fs';
import http from 'node:http';
import net from 'node:net';
import os from 'node:os';
import path from 'node:path';
import { fileURLToPath } from 'node:url';
import { Browser } from './cdp.mjs';

const here = path.dirname(fileURLToPath(import.meta.url));
const toolRoot = path.resolve(here, '..', '..');
const hostDll = path.join(toolRoot, 'src', 'Host', 'bin', 'Debug', 'net8.0', 'SitkaLab.Host.dll');
const evidence = process.env.EVIDENCE_DIR || fs.mkdtempSync(path.join(os.tmpdir(), 'sitkalab-ui-evidence-'));
fs.mkdirSync(evidence, { recursive: true });
const labData = path.join(toolRoot, '.lab-data');
const stamp = String(Date.now());
const createdSaves = [];

const results = [];
const log = (...a) => console.log(...a);

function freePort() {
  return new Promise((res, rej) => { const s = net.createServer(); s.listen(0, '127.0.0.1', () => { const p = s.address().port; s.close(() => res(p)); }); s.on('error', rej); });
}
const sleep = (ms) => new Promise((r) => setTimeout(r, ms));

function rawHttp({ port, method = 'GET', pathName = '/', headers = {}, body }) {
  return new Promise((resolve) => {
    const req = http.request({ host: '127.0.0.1', port, method, path: pathName, headers, setHost: false }, (res) => {
      let data = ''; res.on('data', (c) => (data += c)); res.on('end', () => resolve({ status: res.statusCode, headers: res.headers, body: data }));
    });
    req.on('error', (e) => resolve({ status: 0, error: String(e), headers: {}, body: '' }));
    if (body) req.write(body);
    req.end();
  });
}

async function check(id, title, fn) {
  const t0 = Date.now();
  try {
    const detail = (await fn()) || {};
    results.push({ id, title, verdict: 'PASS', ms: Date.now() - t0, ...detail });
    log(`PASS  ${id}  ${title}  (${Date.now() - t0} ms)`);
  } catch (e) {
    results.push({ id, title, verdict: 'FAIL', ms: Date.now() - t0, error: String(e.message || e) });
    log(`FAIL  ${id}  ${title}\n      ${e.message || e}`);
  }
}
const assert = (c, m) => { if (!c) throw new Error(m); };
const eq = (a, b, m) => { if (JSON.stringify(a) !== JSON.stringify(b)) throw new Error(`${m}: expected ${JSON.stringify(b)} but got ${JSON.stringify(a)}`); };

// ---------- start host ----------
const port = await freePort();
const base = `http://127.0.0.1:${port}`;
const hostProc = spawn('dotnet', [hostDll, '--port', String(port)], { stdio: ['ignore', 'pipe', 'pipe'], cwd: toolRoot });
let hostLog = ''; hostProc.stdout.on('data', (d) => (hostLog += d)); hostProc.stderr.on('data', (d) => (hostLog += d));
for (let i = 0; i < 100; i++) { const r = await rawHttp({ port, pathName: '/api/health', headers: { Host: `127.0.0.1:${port}` } }); if (r.status === 200) break; await sleep(100); }

const core = async (p) => JSON.parse((await rawHttp({ port, pathName: p, headers: { Host: `127.0.0.1:${port}` } })).body);
const corePost = async (p, obj) => JSON.parse((await rawHttp({ port, method: 'POST', pathName: p, headers: { Host: `127.0.0.1:${port}`, Origin: base, 'Content-Type': 'application/json', 'X-Sitka-Lab': '1' }, body: JSON.stringify(obj) })).body);
const resetFixture = (id) => corePost('/api/fixture/load', { fixtureId: id, confirm: true });

const browser = await Browser.launch({ width: 1440, height: 900 });
const ver = await browser.version();
const page = await browser.newPage(1440, 900);
const requestedUrls = [];
browser.listeners.push((m) => { if (m.sessionId === page.sessionId && m.method === 'Network.requestWillBeSent') requestedUrls.push(m.params.request.url); });
await page.send('Network.enable');
const consoleErrors = [];
browser.listeners.push((m) => { if (m.sessionId === page.sessionId && (m.method === 'Runtime.exceptionThrown' || (m.method === 'Runtime.consoleAPICalled' && m.params.type === 'error'))) consoleErrors.push(JSON.stringify(m.params).slice(0, 300)); });

// The page announces "Ready" only after its first state + list + preview fetches complete; wait for it so no later async update races a test step.
async function gotoApp(p = page) {
  await p.goto(base + '/');
  await p.waitFor(`document.querySelectorAll('g.tree').length > 0 && document.getElementById('message').textContent.startsWith('OK: Ready')`, { what: 'app ready' });
}
async function load(fixtureId) {
  if (fixtureId) await resetFixture(fixtureId);
  await gotoApp();
}
async function pending() { await page.eval(`document.getElementById('message').dataset.code='__pending__'`); }
async function messageAfterClick(selector, opts) {
  await pending(); await page.click(selector, opts);
  await page.waitFor(`document.getElementById('message').dataset.code !== '__pending__'`, { what: 'message after click ' + selector });
  return page.eval(`({code: document.getElementById('message').dataset.code, text: document.getElementById('message').textContent, cls: document.getElementById('message').className})`);
}
const planState = (id) => page.eval(`(document.querySelector('.plan[data-plan-id="${id}"]')||{dataset:{}}).dataset.planState || null`);
async function waitPlan(id, st) { await page.waitFor(`(document.querySelector('.plan[data-plan-id="${id}"]')||{dataset:{}}).dataset.planState === '${st}'`, { what: `${id} → ${st}` }); }
const shot = (name) => page.screenshot(path.join(evidence, name + '.png'));
const adultsFree = (st) => st.trees.filter((t) => t.eligible && t.markState === 'UNMARKED');
const metricsKey = (m) => JSON.stringify([m.eligibleLiveN, m.baM2, m.baM2PerHa, m.nPerHa, m.dqCm, m.liveBelowThreshold, m.dead, m.felled]);
const uiMetrics = () => page.eval(`(() => { const g = (i) => document.getElementById(i).dataset.raw; return { n: g('m-n'), nha: g('m-nha'), ba: g('m-ba'), baha: g('m-baha'), dq: g('m-dq'), areaM2: g('m-area-m2'), areaHa: g('m-area-ha') }; })()`);
async function assertMetricParity(label) {
  const c = (await core('/api/state')).metrics; const u = await uiMetrics();
  eq(Number(u.n), c.eligibleLiveN, label + ' N'); eq(Number(u.ba), c.baM2, label + ' BA'); eq(Number(u.baha), c.baM2PerHa, label + ' BA/ha');
  eq(Number(u.nha), c.nPerHa, label + ' N/ha'); eq(u.dq === '' ? null : Number(u.dq), c.dqCm, label + ' Dq'); eq(Number(u.areaM2), c.areaM2, label + ' area m2'); eq(Number(u.areaHa), c.areaHa, label + ' area ha');
  return c;
}

// ======================================================================================================
await load('sitka-flat-demo-v1');
await shot('desktop-01-initial');

await check('UI-01', 'honest labels, Core/UI identity parity of forest, tree IDs and metrics', async () => {
  const st = await core('/api/state');
  const ui = await page.eval(`({ title: document.getElementById('app-title').textContent, pageTitle: document.title, src: document.getElementById('banner-source').textContent, growth: document.getElementById('banner-growth').textContent,
    forestLine: document.getElementById('forest-line').textContent, advance: document.getElementById('btn-advance').textContent, ids: [...document.querySelectorAll('g.tree')].map(g => g.dataset.treeId).sort(), body: document.body.innerText })`);
  eq(ui.title, 'Synthetic Sitka test stand (flat)', 'title'); eq(ui.src, 'SYNTHETIC TEST FOREST', 'source banner'); eq(ui.growth, 'NO BIOLOGICAL GROWTH', 'growth banner');
  eq(ui.advance, 'Advance experiment step (calendar only — NO BIOLOGICAL GROWTH)', 'advance button label');
  assert(ui.forestLine.includes(st.forestId) && ui.forestLine.includes(st.stateHash.slice(0, 10)), 'forest line shows Core forest id and state hash');
  eq(ui.ids, st.trees.map((t) => t.treeId).sort(), 'displayed tree IDs == Core tree IDs');
  await assertMetricParity('initial');
  return { treeCount: ui.ids.length, forestId: st.forestId, stateHash: st.stateHash };
});

await check('UI-02', 'real mouse click selects a real tree; inspector equals Core', async () => {
  const st = await core('/api/state'); const t = adultsFree(st)[10];
  await page.clickTree(t.treeId);
  await page.waitFor(`document.getElementById('inspector-body').dataset.treeId === '${t.treeId}'`, { what: 'inspector shows ' + t.treeId });
  const fields = await page.eval(`Object.fromEntries([...document.querySelectorAll('#inspector-body td')].map(td => [td.dataset.field, td.textContent]))`);
  eq(fields['Tree ID'], t.treeId, 'ID'); eq(fields['DBH'], `${t.dbhCm.toFixed(1)} cm`, 'DBH'); eq(fields['Height'], `${t.heightM.toFixed(2)} m`, 'height');
  assert(fields['Position'].includes(t.xMetres.toFixed(3)) && fields['Position'].includes(t.yMetres.toFixed(3)), 'position');
  eq(fields['Mark state'], 'UNMARKED', 'mark state'); assert(fields['Biological status'].startsWith('live'), 'status'); assert(fields['Provenance'].includes('SYNTHETIC TEST FOREST'), 'provenance');
  eq(await page.eval(`document.getElementById('sel-summary').dataset.ids`), t.treeId, 'selection summary');
  await shot('desktop-02-selected-tree');
  return { treeId: t.treeId };
});

await check('UI-03', 'multi-select (shift-click) shows Core preview identical to the API and changes nothing', async () => {
  const st = await core('/api/state'); const free = adultsFree(st); const pick = [free[10], free[11], free[20], free[30]];
  const before = metricsKey(st.metrics);
  await page.clickTree(pick[0].treeId);
  for (const t of pick.slice(1)) await page.clickTree(t.treeId, { modifiers: 8 });
  const ids = pick.map((t) => t.treeId).sort();
  await page.waitFor(`document.getElementById('sel-summary').dataset.ids === '${ids.join(',')}' && document.getElementById('preview').dataset.valid === 'true'`, { what: 'preview for 4 trees' });
  const api = await corePost('/api/preview', { treeIds: ids });
  const ui = await page.eval(`({ n: document.getElementById('preview').dataset.removedN, ba: document.getElementById('preview').dataset.removedBa, afterN: document.getElementById('preview').dataset.afterN, afterBa: document.getElementById('preview').dataset.afterBa })`);
  eq(Number(ui.n), api.removedN, 'removed N'); eq(Number(ui.ba), api.removedBaM2, 'removed BA'); eq(Number(ui.afterN), api.after.eligibleLiveN, 'after N'); eq(Number(ui.afterBa), api.after.baM2, 'after BA');
  eq(metricsKey((await core('/api/state')).metrics), before, 'actual metrics unchanged by a preview');
  eq(await page.eval(`document.querySelectorAll('g.tree.selected').length`), 4, 'four glyphs carry the selected outline');
  await shot('desktop-03-multiselect-preview');
  return { ids, removedBaM2: api.removedBaM2 };
});

await check('UI-04', 'metre-scale pan/zoom: wheel zoom, drag pan, fit, scale bar and cursor readout', async () => {
  await page.eval(`window.scrollTo(0, 0)`);
  const label0 = await page.eval(`document.getElementById('scale-label').textContent`);
  const st = await core('/api/state'); const t = st.trees[40];
  const NS = { scroll: false };
  const posBefore = await page.center(`g.tree[data-tree-id="${t.treeId}"] circle.hit`, NS);
  const m = await page.center('#map', NS);
  await page.mouse('mouseWheel', m.x, m.y, { deltaY: -120 }); await page.mouse('mouseWheel', m.x, m.y, { deltaY: -120 });
  await sleep(150);
  const posZoom = await page.center(`g.tree[data-tree-id="${t.treeId}"] circle.hit`, NS);
  const dist = (a, b) => Math.hypot(a.x - m.x, a.y - m.y) - Math.hypot(b.x - m.x, b.y - m.y);
  assert(Math.abs(dist(posZoom, posBefore)) > 5, 'zooming moved the glyph relative to the cursor');
  const label1 = await page.eval(`document.getElementById('scale-label').textContent`);
  assert(label1 !== label0, `scale bar relabelled after zoom (${label0} → ${label1})`);
  // drag-pan
  const a = await page.center(`g.tree[data-tree-id="${t.treeId}"] circle.hit`, NS);
  const px = m.x + 150, py = m.y + 150; // an empty spot inside the map
  await page.mouse('mouseMoved', px, py); await page.mouse('mousePressed', px, py, { buttons: 1 });
  for (let i = 1; i <= 8; i++) await page.mouse('mouseMoved', px - i * 10, py - i * 6, { buttons: 1 });
  await page.mouse('mouseReleased', px - 80, py - 48, { buttons: 0 });
  await sleep(100);
  const b = await page.center(`g.tree[data-tree-id="${t.treeId}"] circle.hit`, NS);
  assert(Math.abs((b.x - a.x) + 80) < 3 && Math.abs((b.y - a.y) + 48) < 3, `pan moved the glyph by (-80,-48): got (${(b.x - a.x).toFixed(1)}, ${(b.y - a.y).toFixed(1)})`);
  const readout = await page.eval(`document.getElementById('cursor-readout').textContent`);
  assert(/x -?\d+\.\d\d m, y -?\d+\.\d\d m/.test(readout), 'cursor readout in metres: ' + readout);
  await page.click('#zoom-fit'); await page.eval(`window.scrollTo(0, 0)`); await sleep(100);
  const fit = await page.center(`g.tree[data-tree-id="${t.treeId}"] circle.hit`, NS);
  assert(Math.abs(fit.x - posBefore.x) < 3 && Math.abs(fit.y - posBefore.y) < 3, 'Fit plot restores the initial view');
  return { scaleLabels: [label0, label1], readout };
});

await check('UI-05', 'explicit tree-ID selection; below-threshold and dead stems are displayed, inspectable and refused for work', async () => {
  const st = await core('/api/state');
  const small = st.trees.find((t) => t.belowThreshold); const dead = st.trees.find((t) => t.status === 'dead');
  await page.click('#sel-clear');
  await page.type('#id-input', small.treeId); await page.click('#id-go');
  await page.waitFor(`document.getElementById('inspector-body').dataset.treeId === '${small.treeId}'`, { what: 'ID select' });
  const insp = await page.eval(`document.getElementById('inspector-body').innerText`);
  assert(insp.includes('below adult threshold'), 'inspector explains the stem is not actionable');
  await page.waitFor(`document.getElementById('preview').dataset.valid === 'false'`, { what: 'Core preview verdict for the small tree' });
  const prev = await page.eval(`document.getElementById('preview').textContent`);
  assert(prev.includes('TreeBelowThreshold'), 'Core preview refuses: ' + prev);
  const r = await messageAfterClick('#btn-mark');
  eq(r.cls.includes('err'), true, 'refusal styled as error'); assert(r.text.includes('below the adult DBH threshold'), 'message: ' + r.text);
  await page.click('#sel-clear'); await page.type('#id-input', dead.treeId); await page.click('#id-go');
  await page.waitFor(`document.getElementById('inspector-body').dataset.treeId === '${dead.treeId}'`, { what: 'dead select' });
  await page.waitFor(`document.getElementById('preview').textContent.includes('TreeNotLive')`, { what: 'Core preview verdict for the dead tree' });
  const bad = await messageAfterClick('#btn-mark'); assert(bad.text.includes('dead'), 'dead mark refused: ' + bad.text);
  await page.type('#id-input', 'NoSuchTree'); const nf = await messageAfterClick('#id-go'); assert(nf.code === 'TreeMissing', 'unknown ID message');
  eq((await core('/api/state')).eventCount, 0, 'no refused action produced an event');
  return { belowThreshold: small.treeId, dead: dead.treeId };
});

// ---------- full explicit workflow ----------
let workflow = {};
await check('UI-06', 'mark → plan → propose → approve → queue change nothing; execute fells all and only; Core/UI parity at every step', async () => {
  await load('sitka-flat-demo-v1');
  const st0 = await core('/api/state'); const free = adultsFree(st0); const pick = free.slice(20, 26).map((t) => t.treeId).sort();
  const base = metricsKey(st0.metrics);
  const baseStatuses = JSON.stringify(st0.trees.map((t) => [t.treeId, t.status]));
  await page.clickTree(pick[0]); for (const id of pick.slice(1)) await page.clickTree(id, { modifiers: 8 });
  await page.waitFor(`document.getElementById('sel-summary').dataset.ids === '${pick.join(',')}' && document.getElementById('preview').dataset.valid === 'true'`);
  const previewApi = await corePost('/api/preview', { treeIds: pick });
  const same = async (step) => { const s = await core('/api/state'); eq(metricsKey(s.metrics), base, step + ': metrics'); eq(JSON.stringify(s.trees.map((t) => [t.treeId, t.status])), baseStatuses, step + ': statuses'); eq(s.forestRevision, 0, step + ': revision'); await assertMetricParity(step); };

  await messageAfterClick('#btn-mark');
  await page.waitFor(`[...document.querySelectorAll('g.tree[data-mark="PLAYER_MARKED"]')].length === ${pick.length}`, { what: 'marked glyphs' });
  assert(await page.eval(`[...document.querySelectorAll('g.tree[data-mark="PLAYER_MARKED"]')].every(g => g.querySelector('text.letter')?.textContent === 'M' && g.querySelector('.ring-MARKED'))`), 'marked trees carry the M letter and dashed ring (not colour alone)');
  await same('after mark'); await shot('desktop-04-marked');

  const created = await messageAfterClick('#btn-create-plan'); assert(created.text.includes('Created P0001'), created.text);
  await waitPlan('P0001', 'DRAFT'); await same('after create');
  await messageAfterClick('#plan-P0001-propose'); await waitPlan('P0001', 'PROPOSED'); await same('after propose');
  await messageAfterClick('#plan-P0001-approve'); await waitPlan('P0001', 'APPROVED'); await same('after approve');
  await messageAfterClick('#plan-P0001-queue'); await waitPlan('P0001', 'PENDING_WORK'); await same('after queue');
  assert(await page.eval(`[...document.querySelectorAll('g.tree[data-plan-state="PENDING_WORK"]')].length === ${pick.length} && [...document.querySelectorAll('g.tree[data-plan-state="PENDING_WORK"]')].every(g => g.querySelector('text.letter').textContent === 'W')`), 'pending trees show W');
  const cardPreview = await page.eval(`document.querySelector('.plan[data-plan-id="P0001"] .preview').dataset.removedBa`);
  eq(Number(cardPreview), previewApi.removedBaM2, 'plan card preview comes from Core and equals the selection preview');
  await shot('desktop-05-plan-pending');
  eq((await core('/api/state')).eventCount, 5, 'events so far: mark, create, propose, approve, queue');
  const evBefore = (await core('/api/state')).eventCount;

  const ex = await messageAfterClick('#plan-P0001-execute'); assert(ex.cls.includes('ok'), 'execute accepted: ' + ex.text);
  await waitPlan('P0001', 'EXECUTED');
  const s1 = await core('/api/state');
  eq(s1.trees.filter((t) => t.status === 'felled').map((t) => t.treeId).sort(), pick, 'felled == plan IDs exactly');
  eq(s1.metrics.eligibleLiveN, st0.metrics.eligibleLiveN - pick.length, 'N reduced by plan size');
  eq(s1.metrics.baM2, previewApi.after.baM2, 'executed BA equals the earlier Core preview');
  eq(s1.forestRevision, 1, 'revision +1'); eq(s1.eventCount, evBefore + 1, 'one TestWorkExecuted event');
  await page.waitFor(`document.querySelectorAll('g.tree[data-status="felled"]').length === ${pick.length}`, { what: 'felled glyphs' });
  await assertMetricParity('after execute');
  await shot('desktop-06-after-execute');
  workflow = { pick, afterBa: s1.metrics.baM2, eventCount: s1.eventCount };
  return { felled: pick, removedBaM2: previewApi.removedBaM2, afterBa: s1.metrics.baM2 };
});

await check('UI-07', 'duplicate execution is refused: audit-only event, nothing more felled', async () => {
  const s0 = await core('/api/state');
  const r = await messageAfterClick('#plan-P0001-execute');
  eq(r.code, 'PlanAlreadyExecuted', 'code'); assert(r.cls.includes('err'), 'error style'); assert(r.text.includes('no tree was felled'), r.text);
  const s1 = await core('/api/state');
  eq(s1.eventCount, s0.eventCount + 1, 'one audit event'); eq(s1.stateHash, s0.stateHash, 'forest state hash unchanged');
  eq(s1.trees.filter((t) => t.status === 'felled').length, s0.trees.filter((t) => t.status === 'felled').length, 'felled count unchanged');
  const last = s1.recentEvents.at(-1); eq(last.eventType, 'PlanExecutionRejected', 'audit event type'); eq(last.prevStateHash, last.postStateHash, 'audit-only');
  assert((await page.eval(`document.getElementById('event-list').textContent`)).includes('audit-only'), 'event log shows audit-only');
  return { code: r.code };
});

await check('UI-08', 'reject and cancel paths release trees; refused transitions explain themselves', async () => {
  const st = await core('/api/state'); const free = adultsFree(st).map((t) => t.treeId);
  const a = free.slice(0, 3), b = free.slice(3, 5);
  // plan A → proposed → rejected
  await corePost('/api/marks/mark', { treeIds: a }); await gotoApp();
  await messageAfterClick('#btn-create-plan'); await waitPlan('P0002', 'DRAFT');
  const bad = await messageAfterClick('#plan-P0002-approve'); eq(bad.code, 'Rejected', 'approve a DRAFT refused'); assert(bad.text.includes('only a PROPOSED plan can be approved'), bad.text);
  await messageAfterClick('#plan-P0002-propose'); await waitPlan('P0002', 'PROPOSED');
  await messageAfterClick('#plan-P0002-reject'); await waitPlan('P0002', 'REJECTED');
  eq(await page.eval(`document.querySelectorAll('g.tree[data-mark="PLAYER_MARKED"]').length`), 0, 'rejected plan released its trees');
  // plan B → cancelled after approval
  await corePost('/api/marks/mark', { treeIds: b }); await gotoApp();
  await messageAfterClick('#btn-create-plan'); await waitPlan('P0003', 'DRAFT');
  await messageAfterClick('#plan-P0003-propose'); await messageAfterClick('#plan-P0003-approve'); await waitPlan('P0003', 'APPROVED');
  await messageAfterClick('#plan-P0003-cancel'); await waitPlan('P0003', 'CANCELLED');
  const exe = await messageAfterClick('#plan-P0003-execute'); eq(exe.code, 'PlanCancelledOrRejected', 'cancelled plan cannot execute');
  const exe2 = await messageAfterClick('#plan-P0002-execute'); eq(exe2.code, 'PlanCancelledOrRejected', 'rejected plan cannot execute');
  eq((await core('/api/state')).trees.filter((t) => t.status === 'felled').length, workflow.pick.length, 'no extra felling');
  await shot('desktop-07-rejected-cancelled');
  return { codes: [bad.code, exe.code, exe2.code] };
});

await check('UI-09', 'calendar step makes approved/pending plans STALE, grows nothing, and a stale plan cannot execute', async () => {
  const st = await core('/api/state'); const free = adultsFree(st).map((t) => t.treeId); const ids = free.slice(0, 3);
  await corePost('/api/marks/mark', { treeIds: ids }); await gotoApp();
  await messageAfterClick('#btn-create-plan'); const pid = (await core('/api/state')).plans.at(-1).planId;
  await messageAfterClick(`#plan-${pid}-propose`); await messageAfterClick(`#plan-${pid}-approve`); await messageAfterClick(`#plan-${pid}-queue`); await waitPlan(pid, 'PENDING_WORK');
  const grow0 = JSON.stringify((await core('/api/state')).trees.map((t) => [t.treeId, t.ageAtInventoryYears, t.dbhCm, t.heightM, t.xMetres, t.yMetres, t.status]));
  const m0 = metricsKey((await core('/api/state')).metrics); const rev0 = (await core('/api/state')).forestRevision;
  const adv = await messageAfterClick('#btn-advance'); assert(adv.cls.includes('ok'), adv.text);
  await waitPlan(pid, 'STALE');
  const s1 = await core('/api/state');
  eq(JSON.stringify(s1.trees.map((t) => [t.treeId, t.ageAtInventoryYears, t.dbhCm, t.heightM, t.xMetres, t.yMetres, t.status])), grow0, 'age, DBH, height, position, status unchanged');
  eq(metricsKey(s1.metrics), m0, 'stand metrics unchanged'); eq(s1.forestRevision, rev0 + 1, 'revision +1');
  assert(s1.trees.filter((t) => ids.includes(t.treeId)).every((t) => t.markState === 'UNMARKED'), 'stale plan trees released');
  await shot('desktop-08-stale-after-calendar-step');
  const evBefore = s1.eventCount;
  const ex = await messageAfterClick(`#plan-${pid}-execute`); eq(ex.code, 'PlanStale', 'execute stale refused');
  const qa = await messageAfterClick(`#plan-${pid}-approve`); eq(qa.code, 'Rejected', 're-approve stale refused');
  const s2 = await core('/api/state'); eq(s2.eventCount, evBefore + 1, 'only the execute attempt produced an audit event');
  eq(s2.trees.filter((t) => t.status === 'felled').length, workflow.pick.length, 'nothing felled by the stale attempt');
  assert((await page.eval(`document.getElementById('banner-growth').textContent`)) === 'NO BIOLOGICAL GROWTH', 'banner still shown');
  return { plan: pid, executeCode: ex.code };
});

await check('UI-10', 'save → reset (confirmation) → load → replay verified; approved plan becomes STALE on load', async () => {
  await load('sitka-flat-demo-v1');
  const free = adultsFree(await core('/api/state')).map((t) => t.treeId);
  // build: executed plan + an approved plan outstanding
  await corePost('/api/marks/mark', { treeIds: free.slice(0, 2) }); await corePost('/api/plans/create', { treeIds: free.slice(0, 2) });
  for (const a of ['propose', 'approve', 'queue', 'execute']) await corePost('/api/plans/P0001/' + a, {});
  await corePost('/api/marks/mark', { treeIds: free.slice(5, 7) }); await corePost('/api/plans/create', { treeIds: free.slice(5, 7) });
  for (const a of ['propose', 'approve']) await corePost('/api/plans/P0002/' + a, {});
  await gotoApp();
  const saved = await core('/api/state');
  const name = 'browser-test-' + stamp; createdSaves.push(name + '.sitkalab.json');
  await page.eval(`document.getElementById('save-name').value = ${JSON.stringify(name)}`);
  const sv = await messageAfterClick('#btn-save'); assert(sv.cls.includes('ok'), sv.text);
  assert(fs.existsSync(path.join(labData, name + '.sitkalab.json')), 'file exists under .lab-data');
  // reset through the confirmation flow
  eq(await page.eval(`getComputedStyle(document.getElementById('reset-confirm')).display`), 'none', 'reset confirmation is NOT visible until requested');
  await page.click('#btn-reset'); eq(await page.eval(`getComputedStyle(document.getElementById('reset-confirm')).display`), 'flex', 'confirmation shown after Reset…');
  await page.click('#btn-reset-no'); eq((await core('/api/state')).eventCount, saved.eventCount, 'Keep current changed nothing');
  await page.click('#btn-reset'); const rr = await messageAfterClick('#btn-reset-yes'); assert(rr.cls.includes('ok'), rr.text);
  await page.waitFor(`document.querySelectorAll('.plan').length === 0`, { what: 'plans gone after reset' });
  const reset = await core('/api/state'); eq(reset.eventCount, 0, 'reset state has no events'); eq(reset.plans.length, 0, 'no plans after reset');
  // load the saved file by name from the lab list
  await page.eval(`(() => { const s = document.getElementById('load-select'); s.value = ${JSON.stringify(name + '.sitkalab.json')}; })()`);
  const ld = await messageAfterClick('#btn-load'); assert(ld.cls.includes('ok'), ld.text);
  await page.waitFor(`document.querySelectorAll('.plan').length === 2`, { what: 'plans after load' });
  const loaded = await core('/api/state');
  eq(loaded.plans.find((p) => p.planId === 'P0001').planState, 'EXECUTED', 'executed plan preserved');
  eq(loaded.plans.find((p) => p.planId === 'P0002').planState, 'STALE', 'approved plan is STALE after load');
  eq(loaded.eventCount, saved.eventCount + 1, 'one PlanStale/SessionLoaded event on activation');
  assert(loaded.stateHash !== saved.stateHash, 'intentional stale transition changes the state hash');
  eq(loaded.trees.filter((t) => t.status === 'felled').map((t) => t.treeId).sort(), free.slice(0, 2).sort(), 'felled trees preserved');
  assert(loaded.sessionEpoch !== saved.sessionEpoch, 'new session epoch');
  await page.click('#btn-replay'); await page.waitFor(`document.getElementById('replay-result').dataset.ok === 'true'`, { what: 'replay ok' });
  const rtxt = await page.eval(`document.getElementById('replay-result').textContent`); assert(rtxt.includes('matches'), rtxt);
  await assertMetricParity('after load');
  return { savedHash: saved.stateHash, loadedHash: loaded.stateHash, replay: rtxt };
});

await check('UI-11', 'export provenance manifest + event log is well-formed and matches Core', async () => {
  const exp = await (async () => JSON.parse((await rawHttp({ port, pathName: '/api/export', headers: { Host: `127.0.0.1:${port}` } })).body))();
  const st = await core('/api/state');
  eq(exp.manifest.format, 'ccf-sitka-lab', 'format'); eq(exp.manifest.sourceClass, 'Synthetic', 'source class'); eq(exp.eventLog.length, st.eventCount, 'event count');
  assert(exp.manifest.evidenceClass.includes('ENGINEERING TEST FIXTURE'), 'evidence class'); eq(exp.manifest.snapshotHash, st.stateHash, 'hash');
  const r = await messageAfterClick('#btn-export'); assert(r.cls.includes('ok'), r.text);
  return { events: exp.eventLog.length };
});

await check('UI-12', 'constrained viewport: stacked layout, no horizontal overflow, controls reachable', async () => {
  const narrow = await browser.newPage(390, 844);
  await gotoApp(narrow);
  const o = await narrow.eval(`({ sw: document.documentElement.scrollWidth, cw: document.documentElement.clientWidth, cols: getComputedStyle(document.querySelector('main')).gridTemplateColumns.split(' ').length })`);
  assert(o.sw <= o.cw + 1, `no horizontal page scroll (scrollWidth ${o.sw} vs ${o.cw})`); eq(o.cols, 1, 'single column layout');
  const st = await core('/api/state'); const t = adultsFree(st)[0];
  await narrow.clickTree(t.treeId); await narrow.waitFor(`document.getElementById('inspector-body').dataset.treeId === '${t.treeId}'`);
  await narrow.screenshot(path.join(evidence, 'narrow-390x844-selected.png'));
  const mid = await browser.newPage(800, 900);
  await gotoApp(mid);
  const o2 = await mid.eval(`({ sw: document.documentElement.scrollWidth, cw: document.documentElement.clientWidth })`);
  assert(o2.sw <= o2.cw + 1, 'no horizontal overflow at 800px');
  await mid.screenshot(path.join(evidence, 'narrow-800x900.png'));
  await narrow.close(); await mid.close();
  return { narrow: o, mid: o2 };
});

// ---------- 2,000 tree performance ----------
const perf = { samples: [] };
await check('UI-13', '2,000-tree fixture: every ID displayed; selection + inspector + Core preview ≤ 2 s', async () => {
  await load('sitka-flat-demo-v1');
  await page.eval(`document.getElementById('fixture-select').value = 'sitka-flat-perf-2000-v1'`);
  await page.click('#btn-reset'); await messageAfterClick('#btn-reset-yes');
  await page.waitFor(`document.querySelectorAll('g.tree').length === 2000`, { what: '2000 glyphs', timeout: 15000 });
  const st = await core('/api/state');
  eq(st.trees.length, 2000, 'Core has 2000 trees');
  eq(await page.eval(`[...document.querySelectorAll('g.tree')].map(g => g.dataset.treeId).sort()`), st.trees.map((t) => t.treeId).sort(), 'no missing visible tree IDs');
  await assertMetricParity('2000');
  await shot('desktop-09-perf-2000-initial');
  const adults = adultsFree(st);
  const timed = async (label, action, expectIds) => {
    const t0 = Date.now(); await action();
    await page.waitFor(`document.getElementById('sel-summary').dataset.ids === '${expectIds.slice().sort().join(',')}' && document.getElementById('preview').dataset.valid === 'true' && document.getElementById('inspector-body').dataset.treeId !== ''`, { timeout: 5000, what: label });
    const wall = Date.now() - t0;
    const inPage = Number(await page.eval(`document.getElementById('perf-line').dataset.ms`));
    perf.samples.push({ label, wallMs: wall, inPageMs: inPage }); return wall;
  };
  // zoom in so the clicked glyphs are well separated, as a user would; fit view keeps 10 px pitch which is still clickable
  const walls = [];
  for (let i = 0; i < 8; i++) { const t = adults[100 + i * 37]; walls.push(await timed(`single click ${t.treeId}`, () => page.clickTree(t.treeId), [t.treeId])); }
  let sel = [adults[300].treeId]; await timed('base click', () => page.clickTree(sel[0]), sel);
  for (let i = 1; i <= 12; i++) { const t = adults[300 + i * 11]; sel.push(t.treeId); walls.push(await timed(`shift-click #${i} → ${sel.length} selected`, () => page.clickTree(t.treeId, { modifiers: 8 }), sel)); }
  const max = Math.max(...walls);
  assert(max <= 2000, `max selection→preview latency ${max} ms exceeds the 2000 ms budget`);
  perf.maxWallMs = max; perf.meanWallMs = Math.round(walls.reduce((a, b) => a + b, 0) / walls.length);
  perf.maxInPageMs = Math.max(...perf.samples.map((s) => s.inPageMs));
  const mark = await messageAfterClick('#btn-mark'); assert(mark.cls.includes('ok'), mark.text);
  await shot('desktop-10-perf-2000-selection');
  return { maxWallMs: perf.maxWallMs, meanWallMs: perf.meanWallMs, maxInPageMs: perf.maxInPageMs, samples: perf.samples.length };
});

await check('UI-14', 'security: only 127.0.0.1 is listening; hostile Host/Origin/method/CORS requests refused; no external requests', async () => {
  const sock = execFileSync('ss', ['-ltnH']).toString().split('\n').filter((l) => l.includes(`:${port} `) || l.endsWith(`:${port}`));
  assert(sock.length >= 1 && sock.every((l) => l.includes(`127.0.0.1:${port}`)), 'listening only on 127.0.0.1: ' + sock.join(' | '));
  const H = { Host: `127.0.0.1:${port}` };
  const cases = [
    ['bad Host', { pathName: '/api/state', headers: { Host: 'evil.example' } }, 403],
    ['localhost Host (DNS-rebinding shape)', { pathName: '/api/state', headers: { Host: `localhost:${port}` } }, 403],
    ['foreign Origin GET', { pathName: '/api/state', headers: { ...H, Origin: 'http://evil.example' } }, 403],
    ['foreign Origin POST', { method: 'POST', pathName: '/api/step/advance', headers: { ...H, Origin: 'http://evil.example', 'Content-Type': 'application/json', 'X-Sitka-Lab': '1' }, body: '{}' }, 403],
    ['POST without Origin', { method: 'POST', pathName: '/api/step/advance', headers: { ...H, 'Content-Type': 'application/json', 'X-Sitka-Lab': '1' }, body: '{}' }, 403],
    ['POST without lab header', { method: 'POST', pathName: '/api/step/advance', headers: { ...H, Origin: base, 'Content-Type': 'application/json' }, body: '{}' }, 403],
    ['POST text/plain', { method: 'POST', pathName: '/api/step/advance', headers: { ...H, Origin: base, 'Content-Type': 'text/plain', 'X-Sitka-Lab': '1' }, body: '{}' }, 415],
    ['cross-site fetch metadata', { pathName: '/api/state', headers: { ...H, 'Sec-Fetch-Site': 'cross-site' } }, 403],
    ['CORS preflight OPTIONS', { method: 'OPTIONS', pathName: '/api/state', headers: { ...H, Origin: 'http://evil.example', 'Access-Control-Request-Method': 'POST' } }, 405],
    ['PUT', { method: 'PUT', pathName: '/api/state', headers: { ...H, Origin: base } }, 405],
    ['DELETE', { method: 'DELETE', pathName: '/api/state', headers: H }, 405],
    ['path traversal static', { pathName: '/../../../../etc/passwd', headers: H }, [400, 404]],
    ['encoded traversal static', { pathName: '/%2e%2e/%2e%2e/etc/passwd', headers: H }, [400, 404]],
    ['arbitrary file via save API', { method: 'POST', pathName: '/api/saves/load', headers: { ...H, Origin: base, 'Content-Type': 'application/json', 'X-Sitka-Lab': '1' }, body: JSON.stringify({ fileName: '../../../../etc/passwd' }) }, 403],
    ['absolute path via save API', { method: 'POST', pathName: '/api/saves/load', headers: { ...H, Origin: base, 'Content-Type': 'application/json', 'X-Sitka-Lab': '1' }, body: JSON.stringify({ fileName: '/etc/passwd' }) }, 403],
    ['save with path name', { method: 'POST', pathName: '/api/saves/save', headers: { ...H, Origin: base, 'Content-Type': 'application/json', 'X-Sitka-Lab': '1' }, body: JSON.stringify({ name: '../escape' }) }, 400],
  ];
  const got = [];
  for (const [name, req, want] of cases) {
    const r = await rawHttp({ port, ...req });
    const ok = Array.isArray(want) ? want.includes(r.status) : r.status === want;
    got.push({ name, status: r.status, want });
    assert(ok, `${name}: expected ${JSON.stringify(want)} but got ${r.status}`);
    assert(!r.headers['access-control-allow-origin'], `${name}: must not send CORS allow headers`);
  }
  assert(!fs.existsSync(path.join(labData, '..', 'escape.sitkalab.json')), 'no file escaped the lab root');
  const health = JSON.parse((await rawHttp({ port, pathName: '/api/health', headers: H })).body); eq(health.bind, '127.0.0.1', 'reported bind');
  const bad = requestedUrls.filter((u) => !(u.startsWith(base + '/') || u.startsWith('data:') || u.startsWith('blob:') || u.startsWith('about:')));
  assert(bad.length === 0, 'page made non-loopback requests: ' + bad.join(', '));
  return { requests: requestedUrls.length, cases: got };
});

await check('UI-15', 'no unexpected console errors or exceptions in the page', async () => { assert(consoleErrors.length === 0, consoleErrors.join('\n')); return {}; });

// ---------- receipts ----------
let dotnetVersion = ''; try { dotnetVersion = execFileSync('dotnet', ['--version'], { cwd: toolRoot }).toString().trim(); } catch { /* ignore */ }
const cpu = (fs.readFileSync('/proc/cpuinfo', 'utf8').match(/model name\s*:\s*(.+)/) || [])[1];
const summary = {
  tool: 'MultiRackSitkaLab P0 browser qualification', when: new Date().toISOString(),
  host: { hostname: os.hostname(), os: `${os.type()} ${os.release()}`, cpu, cores: os.cpus().length, memGB: Math.round(os.totalmem() / 2 ** 30), dotnetSdk: dotnetVersion, node: process.version },
  browser: { product: ver.product, userAgent: ver.userAgent, mode: 'headless=new, 1440x900 desktop viewport, real DevTools input events' },
  hostUrl: base, evidenceDir: evidence,
  perf2000: { budgetMs: 2000, maxWallMs: perf.maxWallMs, meanWallMs: perf.meanWallMs, maxInPageMs: perf.maxInPageMs, samples: perf.samples },
  counts: { pass: results.filter((r) => r.verdict === 'PASS').length, fail: results.filter((r) => r.verdict === 'FAIL').length },
  results,
};
fs.writeFileSync(path.join(evidence, 'browser-results.json'), JSON.stringify(summary, null, 2));
fs.writeFileSync(path.join(evidence, 'host-console.log'), hostLog);

await page.close(); await browser.close();
hostProc.kill('SIGTERM'); await sleep(300);
for (const f of createdSaves) { const p = path.join(labData, f); if (fs.existsSync(p)) fs.rmSync(p); } // remove only files this run created
log(`\n==== browser qualification: ${summary.counts.pass} passed, ${summary.counts.fail} failed — evidence: ${evidence}`);
process.exit(summary.counts.fail === 0 ? 0 : 1);
