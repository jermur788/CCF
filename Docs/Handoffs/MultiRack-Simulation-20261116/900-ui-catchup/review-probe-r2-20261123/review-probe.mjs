// Independent reviewer probe for MULTI Stage 4 owner UX repair, Phase A, candidate 5f3b5bf3 (R2 review).
// Written by the reviewer; it does not import the writer's scenario. Usage (Windows node v26):
//   node review-probe.mjs <outDir> <fileHoldingBaseUrl> <W>x<H> [sectionList]
// Every output line goes through redact(); the capability URL is never printed or stored in the results.
import fs from 'node:fs';
import path from 'node:path';
import { launch, reachable, redact, sleep } from './cdp-review.mjs';

const [OUT, URL_FILE, SIZE, ONLY] = process.argv.slice(2);
const [W, H] = (SIZE || '1280x720').split('x').map(Number);
fs.mkdirSync(OUT, { recursive: true });
const base = fs.readFileSync(URL_FILE, 'utf8').trim();
const HELPERS = fs.readFileSync(new URL('./page-helpers.js', import.meta.url), 'utf8');
const tag = W + 'x' + H;
const J = JSON.stringify;
const results = [];
let failures = 0;
function line(kind, label, detail) {
  const text = kind + ' [' + tag + '] ' + label + (detail === undefined ? '' : ' :: ' + redact(typeof detail === 'string' ? detail : J(detail)));
  results.push(text);
  console.log(text);
}
const info = (label, detail) => line('INFO', label, detail);
function check(ok, label, detail) { if (!ok) failures++; line(ok ? 'PASS' : 'FAIL', label, detail); return ok; }
const shots = [];
let page;
async function shot(name) {
  const file = path.join(OUT, name + '-' + tag + '.png');
  const bytes = await page.shot(file);
  shots.push({ name: path.basename(file), bytes });
  console.log('SHOT ' + path.basename(file) + ' ' + bytes);
}
const ev = (expr) => page.eval(expr);
const rvx = (expr) => page.eval(`(async () => { const rv = window.__rv; return await (${expr}); })()`);
const want = (name) => !ONLY || ONLY.split(',').includes(name);
async function section(name, fn) {
  if (!want(name)) return;
  console.log('--- section ' + name);
  try { await fn(); } catch (e) {
    check(false, name + ': section aborted by an exception', redact(String(e && e.stack || e).slice(0, 700)));
    try {
      await shot('ERR-' + name);
      info(name + ' state at abort', await ev(`({ state: window.__rv.state(), competition: document.getElementById('competition-content').innerText.slice(0, 300).replace(/\\s+/g, ' '), pane: document.querySelector('.view-tab[aria-pressed="true"]') && document.querySelector('.view-tab[aria-pressed="true"]').textContent,
        alert: document.getElementById('alert').innerText.slice(0, 300).replace(/\\s+/g, ' '), lockHint: document.getElementById('alert').className, dialogs: [...document.querySelectorAll('dialog[open]')].map((d) => d.id) })`));
    } catch { /* best effort */ }
  }
}

// ---------------------------------------------------------------------------------------------------------------------
async function fresh() {
  await page.goto(base + 'multi');
  await page.waitFor(`document.getElementById('tree-rows').childNodes.length > 0 && document.querySelectorAll('.area-readouts dt').length >= 6`, 'first view');
  await sleep(500);
  await ev(HELPERS);
}
const showTree = () => page.click('[data-pane-button="tree"]');
async function settle(ms = 250) { await sleep(ms); await page.waitFor(`!document.documentElement.classList.contains('busy')`, 'idle', 20000); }
async function selectById(id, timeout = 9000) {
  if (await ev(`getComputedStyle(document.getElementById('pane-tree')).display === 'none'`)) await showTree();
  await ev(`(() => { const q = document.getElementById('quick-id'); q.value = ${J(id)}; q.focus(); })()`);
  await page.key('Enter');
  await page.waitFor(`(() => { const id = window.__rv.inspected(); return id === ${J(id)} && (document.querySelectorAll('#competition-content [data-draft]').length > 0 || !!document.querySelector('#competition-content .comp-empty') || !!document.querySelector('#competition-content .note')); })()`, 'inspect ' + id, timeout);
  await sleep(60);
}
async function setLight(on) {
  const now = (await ev(`document.getElementById('light-toggle').getAttribute('aria-pressed')`)) === 'true';
  if (now !== on) { await page.click('#light-toggle'); await sleep(200); }
}
async function setGround(on) {
  const now = (await ev(`document.getElementById('ground-mode').getAttribute('aria-pressed')`)) === 'true';
  if (now !== on) { await page.click('#ground-mode'); await sleep(200); }
}
async function showArea(areaId) {
  const cur = await ev(`document.getElementById('map-subtitle').textContent`);
  if (cur.startsWith(areaId === 'MA-A' ? 'Area A' : 'Area B')) return;
  await page.click('.area-tab[data-area="' + areaId + '"]');
  await page.waitFor(`document.getElementById('map-subtitle').textContent.startsWith(${J(areaId === 'MA-A' ? 'Area A' : 'Area B')})`, 'area ' + areaId);
  await sleep(250);
}
async function closeInspect() {
  if (await ev(`window.__rv.inspected()`)) { await page.click('#inspect-close'); await sleep(200); }
}
async function crownClick(areaId, treeId) {
  const item = await rvx(`rv.view().then((p) => p.scenes[${J(areaId)}].find((i) => i.treeId === ${J(treeId)}))`);
  if (!item) return { clicked: false, why: 'no scene item' };
  const at = await ev(`window.__rv.screenOf(${item.u}, ${item.v})`);
  if (!(at.inView && at.onCrown && !at.onRank)) return { clicked: false, why: at.top };
  await page.clickAt(at.x, at.y);
  await sleep(250);
  return { clicked: true, at };
}
async function waitCompetition(timeout = 8000) {
  await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0 || !!document.querySelector('#competition-content .comp-empty') || !!document.querySelector('#competition-content .note')`, 'competition', timeout);
}
const foldOk = (f) => (f.n === 0 ? f.formHidden || true : f.rowsReachable && f.addReachable) && f.scrollTop === 0 && f.refReachable && f.closeReachable &&
  (f.n === 0 || (f.lastBottom <= f.dockTop + 1 && f.addBottom <= Math.min(f.innerHeight, f.paneBottom + 1) && f.firstTop >= f.paneTop));
const livingIds = () => rvx(`rv.view().then((p) => p.view.areas.map((a) => ({ areaId: a.areaId, ids: a.trees.filter((t) => t.isLiving).map((t) => t.treeId) })))`);

// ---------------------------------------------------------------------------------------------------------------------
let FREE_REF = null;   // a living tree with five free competitors, found by the sweep
const sweepRows = [];

async function main() {
  if (!(await reachable(base + 'multi'))) throw new Error('server not reachable from Windows');
  page = await launch();
  try {
    await page.viewport(W, H);
    await fresh();
    const s0 = await rvx(`rv.snap()`);
    info('fresh session baseline', { year: s0.year, rev: s0.rev, pending: s0.pending.length, living: s0.living });

    // ===== P0: defaults, notice, navigation geometry, overflow ==========================================================
    await section('P0', async () => {
      const a = await ev(`({ light: document.getElementById('light-toggle').getAttribute('aria-pressed'), cells: document.getElementById('light-cells').childNodes.length,
        groundHidden: document.getElementById('ground-mode').hidden, legendHidden: document.getElementById('light-legend').hidden,
        ramp: document.getElementById('light-ramp').childNodes.length })`);
      check(a.light === 'false' && a.cells === 0 && a.groundHidden && a.legendHidden, 'AC09: Light Guide is OFF by default (no cells drawn, no legend, no Inspect ground button)', a);
      const n = await ev(`(() => { const e = document.querySelector('.notice'); return { line: document.getElementById('notice-line').textContent, h: Math.round(e.getBoundingClientRect().height), open: document.getElementById('notice-details').open,
        mapPaneText: document.getElementById('map-pane').innerText }; })()`);
      check(n.line === 'Synthetic Sitka experiment · 2 areas · Site unknown' && n.h <= 40 && !n.open, 'AC07: one short notice line, Details closed', n);
      await page.click('#notice-details > summary');
      const d = await ev(`(() => ({ open: document.getElementById('notice-details').open, text: document.querySelector('.notice-body').innerText, h: Math.round(document.querySelector('.notice-body').getBoundingClientRect().height) }))()`);
      const need = [/not a surveyed/i, /not assessed comparable|not comparable/i, /site unknown/i, /connected woodland/i, /per area|per-area/i, /no treatment transfer/i, /not unity/i];
      check(d.open && need.every((r) => r.test(d.text)), 'AC07: Details holds every caveat named in UX-04', { missing: need.filter((r) => !r.test(d.text)).map(String), text: d.text.replace(/\s+/g, ' ').slice(0, 900) });
      await page.click('#notice-details > summary');
      const mapText = await ev(`document.getElementById('map-pane').innerText`);
      check(!/not measured sunlight|modelled light|calibrat|k10a10|not a surveyed|PAR/i.test(mapText), 'AC07/AC14: the map pane carries no repeated warning text (Light Guide off)', mapText.replace(/\s+/g, ' ').slice(0, 300));
      const dock = await ev(`(() => { const r = (id) => { const b = document.getElementById(id).getBoundingClientRect(); return { l: b.left, t: b.top, r: b.right, b: b.bottom, w: b.width, h: b.height }; };
        return { n: r('pan-north'), w: r('pan-west'), e: r('pan-east'), s: r('pan-south'), out: r('zoom-out'), fit: r('zoom-fit'), inn: r('zoom-in'), dock: r('map-dock'),
          tabs: [...document.querySelectorAll('.area-tab')].map((t) => { const b = t.getBoundingClientRect(); return { l: b.left, t: b.top, w: b.width, h: b.height }; }) }; })()`);
      const near = (x, y) => Math.abs(x - y) <= 1.5;
      check(near(dock.w.t, dock.e.t) && dock.n.b <= dock.w.t + 1 && dock.s.t >= dock.w.b - 1 && near((dock.n.l + dock.n.r) / 2, (dock.s.l + dock.s.r) / 2) && dock.w.r <= dock.n.l + 1 && dock.e.l >= dock.n.r - 1,
        'AC08: compass N top-centre, W/E middle row, S bottom-centre', dock);
      check(near(dock.out.t, dock.fit.t) && near(dock.fit.t, dock.inn.t) && dock.out.r <= dock.fit.l && dock.fit.r <= dock.inn.l && dock.out.t >= dock.s.b - 1, 'AC08: one horizontal − | Fit | + row directly under the compass', { out: dock.out.t, fit: dock.fit.t, inn: dock.inn.t });
      check(dock.tabs.length === 2 && near(dock.tabs[0].t, dock.tabs[1].t) && dock.tabs[0].t >= dock.out.b - 1 && dock.tabs[0].l < dock.tabs[1].l, 'AC08: Area A | Area B on one horizontal row below', dock.tabs);
      check([dock.n, dock.w, dock.e, dock.s, dock.out, dock.fit, dock.inn].every((b) => b.w >= 44 && b.h >= 44) && dock.tabs.every((t) => t.h >= 44), 'AC08: pointer targets are at least 44 px', { dockW: Math.round(dock.dock.w) });
      const lay = await ev(`(() => { const d = document.documentElement; return { sw: d.scrollWidth, bw: document.body.scrollWidth, iw: innerWidth, sh: d.scrollHeight, ih: innerHeight }; })()`);
      check(lay.sw <= lay.iw && lay.bw <= lay.iw, 'AC16: no horizontal page overflow at the start', lay);
      await shot('P0-initial');
    });

    // ===== P0b: every navigation button, dock vs stand, legend hide/show, selected-state semantics ========================
    await section('P0b', async () => {
      const vbOf = () => ev(`document.getElementById('map').getAttribute('viewBox').split(' ').map(Number)`);
      const zoomText = () => ev(`document.getElementById('zoom-state').textContent`);
      const overlap = async (areaId) => { await showArea(areaId); await page.click('#zoom-fit'); await sleep(250);
        return ev(`(() => { const s = document.getElementById('area-outline').getBoundingClientRect(); const d = document.getElementById('map-dock').getBoundingClientRect(); return { standLeft: Math.round(s.left), dockRight: Math.round(d.right), overlap: !(s.right <= d.left || s.left >= d.right || s.bottom <= d.top || s.top >= d.bottom) }; })()`); };
      const oA = await overlap('MA-A'), oB = await overlap('MA-B');
      check(!oA.overlap && !oB.overlap, 'AC16: at Fit the stand outline never lies under the navigation dock (either area)', { A: oA, B: oB });
      const tabs = await ev(`[...document.querySelectorAll('.area-tab')].map((t) => [t.dataset.area, t.getAttribute('aria-pressed')])`);
      check(tabs.find((t) => t[0] === 'MA-B')[1] === 'true' && tabs.find((t) => t[0] === 'MA-A')[1] === 'false', 'AC16: the shown area is exposed as the pressed Area button (state is not colour alone)', tabs);
      await showArea('MA-A'); await page.click('#zoom-fit'); await sleep(200);
      const vb0 = await vbOf(), z0 = await zoomText();
      const pansDisabledAtFit = await ev(`['pan-north', 'pan-south', 'pan-west', 'pan-east'].every((id) => document.getElementById(id).disabled)`);
      await page.click('#zoom-in'); await page.click('#zoom-in'); await page.click('#zoom-in'); await sleep(150);
      const vb1 = await vbOf(), z1 = await zoomText();
      const res = {};
      for (const [id, axis, sign] of [['pan-east', 0, 1], ['pan-south', 1, 1], ['pan-west', 0, -1], ['pan-north', 1, -1]]) {
        if (await ev(`document.getElementById('${id}').disabled`)) { res[id] = 'disabled'; continue; }
        const b = await vbOf(); await page.click('#' + id); const a = await vbOf(); res[id] = Math.sign(a[axis] - b[axis]) === sign ? 'moved' : 'WRONG DIRECTION'; }
      await page.click('#zoom-out'); const z2 = await zoomText();
      await page.click('#zoom-fit'); const z3 = await zoomText(); const vb3 = await vbOf();
      check(pansDisabledAtFit && vb1[2] < vb0[2] && z1 !== z0 && !Object.values(res).includes('WRONG DIRECTION') && Object.values(res).filter((v) => v === 'moved').length >= 3 && z2 !== z1 && z3 === 'Fit' && J(vb3) === J(vb0),
        'AC08: navigation buttons work: N/W/E/S pan in their own direction (north up), + narrows, − steps back, Fit restores the exact original view; pans are disabled at Fit (original behaviour)', { z0, z1, z2, z3, res, pansDisabledAtFit });
      const idA = (await livingIds())[0].ids[5];
      await selectById(idA);
      const withLegend = await ev(`window.__rv.fold()`);
      await page.click('#legend-hide'); await sleep(350);
      const hid = await ev(`({ fold: window.__rv.fold(), sw: document.documentElement.scrollWidth, iw: innerWidth })`);
      await page.click('#legend-show'); await sleep(350);
      const shown = await ev(`({ fold: window.__rv.fold(), sw: document.documentElement.scrollWidth, iw: innerWidth })`);
      check(foldOk(withLegend) && foldOk(hid.fold) && foldOk(shown.fold) && hid.sw <= hid.iw && shown.sw <= shown.iw, 'AC16: map legend Hide then Show: the primary block stays above the fold with no scrolling and nothing overflows horizontally', { withLegend: foldOk(withLegend), hidden: foldOk(hid.fold), shown: foldOk(shown.fold), overflowHidden: hid.sw > hid.iw, overflowShown: shown.sw > shown.iw });
      await closeInspect();
    });

    // ===== P1: AC01 sweep. Every living tree, both areas, by exact ID; then crowns by real click ========================
    await section('P1', async () => {
      const lists = await livingIds();
      info('living trees', lists.map((l) => l.areaId + ':' + l.ids.length));
      let total = 0, bad = [], minDock = 1e9, minAdd = 1e9, maxScroll = 0, fiveRows = 0;
      for (const l of lists) {
        await showArea(l.areaId);
        for (const id of l.ids) {
          await selectById(id);
          const f = await ev(`window.__rv.fold()`);
          total++;
          if (f.n === 5) fiveRows++;
          if (f.n > 0) { minDock = Math.min(minDock, f.dockTop - f.lastBottom); minAdd = Math.min(minAdd, f.innerHeight - f.addBottom); }
          maxScroll = Math.max(maxScroll, f.scrollTop);
          if (!foldOk(f)) bad.push({ id, f });
          if (f.n === 5 && !FREE_REF && l.areaId === 'MA-A') { const rows = await ev(`window.__rv.rows()`); if (rows.every((r) => !r.disabled)) FREE_REF = { areaId: l.areaId, id }; }
        }
      }
      check(bad.length === 0, 'AC01 (exact-ID selection, Light Guide off): for EVERY living tree in both areas the ' + 'reference, competitor rows and Add button are reachable above the fold with no inspector scroll', { trees: total, withFiveRows: fiveRows, failures: bad.slice(0, 5), minGapRowsToDock: minDock, minGapAddToBottom: minAdd, maxScrollTop: maxScroll });
      await closeInspect();
      // real crown clicks: a spread of trees per area
      let clicked = 0, mismatches = [], badc = [];
      for (const l of lists) {
        await showArea(l.areaId);
        const pick = l.ids.filter((_, i) => i % Math.max(1, Math.floor(l.ids.length / 14)) === 0).slice(0, 14);
        for (const id of pick) {
          await setGround(false);
          const r = await crownClick(l.areaId, id);
          if (!r.clicked) continue;
          clicked++;
          await waitCompetition().catch(() => {});
          const got = await ev(`window.__rv.inspected()`);
          const f = await ev(`window.__rv.fold()`);
          if (got !== id) mismatches.push([id, got]);
          if (got && !foldOk(f)) badc.push({ got, f });
        }
      }
      check(clicked >= 20 && badc.length === 0, 'AC01 (real crown clicks, a spread of trees): the primary block is above the fold every time', { clicked, overlapped: mismatches.length, failures: badc.slice(0, 3) });
      await closeInspect();
    });

    // ===== P2: ranks on the map =========================================================================================
    await section('P2', async () => {
      await showArea('MA-A');
      if (!FREE_REF) { FREE_REF = { areaId: 'MA-A', id: (await livingIds())[0].ids[0] }; }
      await selectById(FREE_REF.id);
      const topFive = await rvx(`rv.topFive(${J(FREE_REF.areaId)}, ${J(FREE_REF.id)})`);
      let ranks = await ev(`window.__rv.ranks()`);
      const rows0 = await ev(`window.__rv.rows()`);
      check(ranks.length === 5 && ranks.map((r) => r.id).join() === topFive.join() && rows0.map((r) => r.id).join() === topFive.join() && ranks.every((r, i) => r.rank === i + 1 && r.tag === 'button' && r.pressed === 'false' && /^Competitor \d, tree MA-/.test(r.label) && r.w >= 24 && r.h >= 24),
        'AC02: five real <button> rank markers 1-5 carry the ENGINE tree IDs in the engine rank order, ARIA labelled, targets >= 24 px', { topFive, ranks: ranks.map((r) => [r.rank, r.id, r.w + 'x' + r.h, r.label]) });
      const before = await rvx(`rv.snap()`);
      // each rank by real mouse click; every click must send NOTHING
      let allZero = true; const per = [];
      for (let k = 0; k < 5; k++) {
        ranks = await ev(`window.__rv.ranks()`);
        const reqBefore = page.log.all.length, pageReq = await ev(`window.__reqs.length`);
        await page.clickAt(ranks[k].x, ranks[k].y);
        await sleep(450);
        const sent = page.log.all.length - reqBefore, pageSent = (await ev(`window.__reqs.length`)) - pageReq;
        const after = await ev(`({ ranks: window.__rv.ranks().map((r) => r.pressed), rows: window.__rv.rows().map((r) => r.checked), insp: window.__rv.inspected(), add: document.getElementById('add-button').textContent.trim() })`);
        const ok = after.ranks[k] === 'true' && after.rows[k] === true && after.insp === FREE_REF.id && after.ranks.filter((x) => x === 'true').length === k + 1 && after.add.includes('Add selected ' + (k + 1));
        if (sent !== 0 || pageSent !== 0 || !ok) allZero = false;
        per.push({ rank: k + 1, id: topFive[k], sentCdp: sent, sentPage: pageSent, ok });
      }
      check(allZero, 'AC02: clicking map ranks 1,2,3,4,5 toggles exactly that engine ID on (multi-select), reference unchanged, checkbox follows, and the click sends NO request at all (CDP + page level)', per);
      // untick two from the map; untick one from the inspector; both directions
      ranks = await ev(`window.__rv.ranks()`);
      await page.clickAt(ranks[1].x, ranks[1].y); await sleep(200);
      let st = await ev(`({ ranks: window.__rv.ranks().map((r) => r.pressed), rows: window.__rv.rows().map((r) => r.checked) })`);
      check(st.ranks[1] === 'false' && st.rows[1] === false && st.ranks.filter((x) => x === 'true').length === 4, 'AC02: clicking rank 2 again clears only that ID', st);
      const reqB = page.log.all.length;
      await page.click(`#competition-content [data-draft="${topFive[3]}"]`); await sleep(350);
      st = await ev(`({ ranks: window.__rv.ranks().map((r) => r.pressed), rows: window.__rv.rows().map((r) => r.checked) })`);
      check(st.ranks[3] === 'false' && st.rows[3] === false && st.ranks[0] === 'true' && st.ranks[2] === 'true' && st.ranks[4] === 'true' && page.log.all.length === reqB,
        'AC03: unticking row 4 in the inspector clears map rank 4 (inspector -> map), no request sent', st);
      // keyboard: Tab to a rank, Space toggles, focus kept, visible outline
      await ev(`document.querySelector('#rank-layer [data-rank="1"]').focus()`);
      await page.key('Tab');
      const kb = await ev(`(() => { const a = document.activeElement; const s = getComputedStyle(a); return { key: a.getAttribute('data-focus-key'), outline: s.outlineStyle, w: s.outlineWidth }; })()`);
      await page.key(' ');
      const kb2 = await ev(`({ pressed: window.__rv.ranks().map((r) => r.pressed), key: document.activeElement.getAttribute('data-focus-key'), ann: document.getElementById('announce').textContent })`);
      check(kb.key === 'rank:' + topFive[1] && kb.outline === 'solid' && parseFloat(kb.w) >= 2 && kb2.pressed[1] === 'true' && kb2.key === 'rank:' + topFive[1] && /selected for Fell, draft only/.test(kb2.ann),
        'AC02/AC16: keyboard Tab reaches rank 2 with a visible focus outline, Space toggles it, focus stays on it, the count is announced', { kb, kb2: { pressed: kb2.pressed, key: kb2.key, ann: kb2.ann } });
      await shot('P2-ranks-chosen');
      // AC03 across an area switch and a new reference
      const chosenNow = (await ev(`window.__rv.ranks()`)).filter((r) => r.pressed === 'true').map((r) => r.id);
      await showArea('MA-B'); await showArea('MA-A'); await sleep(500);
      const afterArea = await ev(`({ insp: window.__rv.inspected(), ranks: window.__rv.ranks().filter((r) => r.pressed === 'true').map((r) => r.id), rows: window.__rv.rows().filter((r) => r.checked).map((r) => r.id) })`);
      check(afterArea.insp === FREE_REF.id && afterArea.ranks.join() === chosenNow.join() && afterArea.rows.join() === chosenNow.join(), 'AC03: after switching to Area B and back, the draft is intact and map ranks and inspector ticks still agree', { chosenNow, afterArea });
      // Add queues exactly the chosen IDs
      await page.click('#add-button');
      await page.waitFor(`document.getElementById('batch-result').hidden === false`, 'add result', 15000);
      await settle(400);
      const s1 = await rvx(`rv.snap()`);
      check(s1.pending.map((p) => p.split(':')[1]).sort().join() === chosenNow.slice().sort().join(), 'AC04: Add queued exactly the chosen IDs (not all competitors); nothing approved or executed', { chosenNow, pending: s1.pending, approved: s1.approved, events: s1.events });
      check(s1.approved === null && s1.events === 0 && s1.living.join() === s0.living.join(), 'AC04: Pending is separate: no Approve and no Execute happened', s1);
      // a new reference clears the old draft in both views
      const f2 = await ev(`window.__rv.fold()`);
      check(foldOk(f2), 'AC01: with the post-Add batch result shown, the rows and Add are still reachable above the fold', f2);
      await shot('P2-after-add');
      const other = (await livingIds())[0].ids.find((id) => id !== FREE_REF.id && !chosenNow.includes(id) && !topFive.includes(id));
      await selectById(other);
      const nr = await ev(`({ ranks: window.__rv.ranks().filter((r) => r.pressed === 'true').length, rows: window.__rv.rows().filter((r) => r.checked).length, add: document.getElementById('add-button').textContent.trim() })`);
      check(nr.ranks === 0 && nr.rows === 0, 'AC03: selecting a new reference clears both the map ranks and the inspector ticks', nr);
      await closeInspect();
    });

    // ===== P1b: AC01 again in a LOADED session (queued orders and Crop designations make competitor rows taller) ==========
    const sweepLoaded = async (label) => {
      const lists = await livingIds(); const recs = [];
      for (const l of lists) { await showArea(l.areaId);
        for (const id of l.ids) { await selectById(id); const f = await ev(`window.__rv.fold()`);
          recs.push({ id, area: l.areaId, n: f.n, blocked: f.blocked, ok: foldOk(f), rowsReach: f.rowsReachableCount, addReach: f.addReachable, lastBottom: f.lastBottom, dockTop: f.dockTop, addBottom: f.addBottom, ih: f.innerHeight, closeRight: f.closeRight, paneRight: f.paneClientRight }); } }
      await closeInspect();
      const bad = recs.filter((r) => !r.ok);
      const byBlocked = {};
      for (const r of recs) { const k = String(r.blocked); byBlocked[k] = byBlocked[k] || { trees: 0, fail: 0 }; byBlocked[k].trees++; if (!r.ok) byBlocked[k].fail++; }
      const addLost = recs.filter((r) => r.n > 0 && !r.addReach).length;
      const closeClipped = recs.filter((r) => r.closeRight > r.paneRight).length;
      info('loaded sweep detail (' + label + ')', { trees: recs.length, failing: bad.length, byBlockedRows: byBlocked, addButtonUnreachable: addLost, closeButtonPastPaneEdge: closeClipped, sampleFail: bad.slice(0, 3) });
      return { recs, bad, byBlocked, addLost, closeClipped };
    };
    await section('P1b', async () => {
      const s = await sweepLoaded('after 4 queued Fell orders in Area A');
      check(s.bad.length === 0, 'AC01 (LOADED session: 4 Fell orders queued, so some competitors read "Already pending Fell"): for every living tree the five rows and Add are reachable above the fold', { trees: s.recs.length, failing: s.bad.length, ofWhichHaveBlockedRows: s.bad.filter((r) => r.blocked > 0).length, byBlockedRows: s.byBlocked, addButtonUnreachable: s.addLost });
      check(s.closeClipped === 0, 'AC03/AC16: "× Close inspection" stays inside the inspector pane for every tree (no clipping)', { treesWhereRightEdgeIsPastPane: s.closeClipped });
    });

    // ===== P3: Escape precedence and Close inspection ===================================================================
    await section('P3', async () => {
      await showArea('MA-A');
      const ref = FREE_REF ? FREE_REF.id : (await livingIds())[0].ids[0];
      const pendingNow = (await rvx(`rv.snap()`)).pending.map((p) => p.split(':')[1]);
      const refB = (await livingIds())[0].ids.filter((id) => id !== ref && !pendingNow.includes(id))[40];
      await selectById(refB);
      await page.click('#crop-button'); await settle(500);
      await page.waitFor(`document.getElementById('crop-button').textContent === 'Remove Crop designation'`, 'crop designation', 8000);
      await page.click('#zoom-in'); await page.click('#zoom-in'); await page.click('#zoom-in'); await sleep(150);
      if (await ev(`!document.getElementById('pan-east').disabled`)) await page.click('#pan-east');
      await sleep(200);
      await setLight(true); await sleep(400);     // legend height changes the map aspect: switch it on BEFORE the baseline, not between baseline and close
      const keep = await ev(`({ zoom: document.getElementById('zoom-state').textContent, vb: document.getElementById('map').getAttribute('viewBox'), area: document.getElementById('map-subtitle').textContent })`);
      const sKeep = await rvx(`rv.snap()`);
      await setLight(true);
      // a modal first
      await page.click('#advance');
      await page.waitFor(`document.getElementById('consent-dialog').open`, 'consent dialog', 8000);
      await page.key('Escape');
      const e1 = await ev(`({ dialog: document.getElementById('consent-dialog').open, insp: window.__rv.inspected() })`);
      check(!e1.dialog && e1.insp === refB, 'AC06: with a dialog open Escape dismisses ONLY the dialog; the inspection stays open', e1);
      // ground mode second
      await setGround(true);
      await page.key('Escape');
      const e2 = await ev(`({ ground: document.getElementById('ground-mode').getAttribute('aria-pressed'), light: document.getElementById('light-toggle').getAttribute('aria-pressed'), insp: window.__rv.inspected(), cells: document.querySelectorAll('#light-cells polygon').length })`);
      check(e2.ground === 'false' && e2.light === 'true' && e2.insp === refB && e2.cells > 0, 'UX-03/UX-08: next Escape stops Inspect ground only; Light Guide stays on and visible; the inspection stays open', e2);
      // inspection third
      await page.key('Escape');
      const e3 = await ev(`({ empty: !document.getElementById('inspector-empty').hidden, ranks: document.getElementById('rank-layer').hidden, dimmed: document.querySelectorAll('#crowns .tree.dimmed').length, leaders: document.querySelectorAll('#marks .spread-leader').length,
        zoom: document.getElementById('zoom-state').textContent, vb: document.getElementById('map').getAttribute('viewBox'), area: document.getElementById('map-subtitle').textContent, light: document.getElementById('light-toggle').getAttribute('aria-pressed') })`);
      const sClosed = await rvx(`rv.snap()`);
      check(e3.empty && e3.ranks && e3.dimmed === 0 && e3.leaders === 0, 'AC05: the third Escape closes the inspection: normal (non-enlarged) map, no rank layer, no dimming', e3);
      check(e3.zoom === keep.zoom && e3.vb === keep.vb && e3.area === keep.area && e3.light === 'true' && JSON.stringify(sClosed) === JSON.stringify(sKeep) && sClosed.crops[0].includes(refB) && sClosed.pending.length > 0,
        'AC05/AC21: closing preserved zoom, pan, area, Light Guide, the Crop designation, the queued orders and the year; the server view is byte-identical', { keep, kept: { zoom: e3.zoom, vb: e3.vb }, crops: sClosed.crops[0], pending: sClosed.pending.length });
      // Close inspection button with a draft: cleared and not restored
      await selectById(ref);
      const rk = await ev(`window.__rv.ranks()`);
      if (rk.length) { await page.clickAt(rk[0].x, rk[0].y); await sleep(150); }
      await page.click('#inspect-close'); await sleep(250);
      const c1 = await ev(`({ empty: !document.getElementById('inspector-empty').hidden, zoom: document.getElementById('zoom-state').textContent })`);
      await selectById(ref);
      const c2 = await ev(`window.__rv.rows().filter((r) => r.checked).length`);
      check(c1.empty && c2 === 0, 'AC05: "× Close inspection" closes the same way and the temporary draft does not come back', { c1, ticksAfterReselect: c2 });
      await closeInspect();
      await setLight(false);
      await page.click('#zoom-fit'); await sleep(150);
    });

    // ===== P4: Light Guide ramp, real-layer pixels ======================================================================
    await section('P4', async () => {
      await page.click('#reread'); await settle(450);               // clears any leftover alert so the layout cannot shift between frames
      await showArea('MA-B'); await closeInspect(); await page.click('#zoom-fit'); await setLight(false); await setGround(false);
      await sleep(400);
      check(!(await ev(`document.getElementById('alert').classList.contains('shown')`)), 'P4 setup: no alert is showing (stable layout for the pixel diff)');
      const nodeTags = () => ev(`(() => { const t = {}; for (const e of document.getElementById('map').querySelectorAll('*')) t[e.localName] = (t[e.localName] || 0) + 1; return t; })()`);
      const tagsOff = await nodeTags();
      const geo = await ev(`(async () => { const p = await window.__rv.view(); const m = document.getElementById('map').getScreenCTM(); const mr = document.getElementById('map').getBoundingClientRect();
        const area = p.stage4.areas.find((a) => a.areaId === 'MA-B');
        return { clip: { x: Math.floor(mr.left), y: Math.floor(mr.top), width: Math.ceil(mr.width), height: Math.ceil(mr.height) }, mapRect: [mr.left, mr.top, mr.width, mr.height],
          cells: area.ground.map((c) => ({ key: c.column + ':' + c.row, L: c.lightIndex, poly: c.polygon.map((q) => [m.a * q.u + m.c * q.v + m.e, m.b * q.u + m.d * q.v + m.f]) })) }; })()`);
      const offImg = await page.capture(geo.clip);
      const nodesOff = await ev(`document.getElementById('map').querySelectorAll('*').length`);
      const inside = (poly, x, y) => { let ins = false; for (let i = 0, j = poly.length - 1; i < poly.length; j = i++) { const a = poly[i], b = poly[j]; if ((a[1] > y) !== (b[1] > y) && x < (b[0] - a[0]) * (y - a[1]) / (b[1] - a[1]) + a[0]) ins = !ins; } return ins; };
      const bbox = (poly) => { const xs = poly.map((q) => q[0]), ys = poly.map((q) => q[1]); return [Math.floor(Math.min(...xs)) - 1, Math.floor(Math.min(...ys)) - 1, Math.ceil(Math.max(...xs)) + 1, Math.ceil(Math.max(...ys)) + 1]; };
      const cx = geo.clip.x, cy = geo.clip.y;
      const interiorPx = (cell) => { const out = []; const [x0, y0, x1, y1] = bbox(cell.poly);
        for (let y = y0; y <= y1; y++) for (let x = x0; x <= x1; x++) { const px = x + .5, py = y + .5; if (px - cx < 1 || py - cy < 1 || px - cx >= geo.clip.width - 1 || py - cy >= geo.clip.height - 1) continue;
          if ([[0, 0], [1.5, 1.5], [-1.5, 1.5], [1.5, -1.5], [-1.5, -1.5]].every(([dx, dy]) => inside(cell.poly, px + dx, py + dy))) out.push([x - cx, y - cy]); }
        return out; };
      // the modal OFF colour inside the cells is the floor colour
      const tally = new Map();
      for (const c of geo.cells) for (const [x, y] of interiorPx(c)) { const k = offImg.px(x, y).join(','); tally.set(k, (tally.get(k) || 0) + 1); }
      const floor = [...tally.entries()].sort((a, b) => b[1] - a[1])[0][0].split(',').map(Number);
      info('floor colour (modal Light-Guide-off pixel inside the cells)', floor);
      // pick the cells with most clean floor pixels as injection targets
      const nearFloor = (p) => p.every((v, i) => Math.abs(v - floor[i]) <= 3);
      // "clean" floor = the whole 5x5 neighbourhood is floor-coloured in the OFF frame, so no overlay edge, halo or
      // semi-transparent mark can sit on or beside the pixel
      // Chebyshev distance (px) from every pixel to the nearest NON-floor pixel of the Light-Guide-off frame (a crown, mark, grid line or any overlay)
      const Wc = geo.clip.width, Hc = geo.clip.height;
      const dist = new Int32Array(Wc * Hc);
      for (let y = 0; y < Hc; y++) for (let x = 0; x < Wc; x++) dist[y * Wc + x] = nearFloor(offImg.px(x, y)) ? 1e9 : 0;
      for (let y = 0; y < Hc; y++) for (let x = 0; x < Wc; x++) { const i = y * Wc + x; if (dist[i] === 0) continue; let m = dist[i];
        if (x > 0) m = Math.min(m, dist[i - 1] + 1); if (y > 0) { m = Math.min(m, dist[i - Wc] + 1); if (x > 0) m = Math.min(m, dist[i - Wc - 1] + 1); if (x < Wc - 1) m = Math.min(m, dist[i - Wc + 1] + 1); } dist[i] = m; }
      for (let y = Hc - 1; y >= 0; y--) for (let x = Wc - 1; x >= 0; x--) { const i = y * Wc + x; if (dist[i] === 0) continue; let m = dist[i];
        if (x < Wc - 1) m = Math.min(m, dist[i + 1] + 1); if (y < Hc - 1) { m = Math.min(m, dist[i + Wc] + 1); if (x < Wc - 1) m = Math.min(m, dist[i + Wc + 1] + 1); if (x > 0) m = Math.min(m, dist[i + Wc - 1] + 1); } dist[i] = m; }
      const distAt = (x, y) => dist[y * Wc + x];
      const cleanAt = (x, y) => distAt(x, y) >= 3;
      const floorPx = (c) => interiorPx(c).filter(([x, y]) => cleanAt(x, y));
      const ranked = geo.cells.map((c) => ({ c, px: floorPx(c) })).sort((a, b) => b.px.length - a.px.length);
      info('cells with >= 60 clean floor pixels', ranked.filter((r) => r.px.length >= 60).length);
      const VALUES = [0, 1e-9, 0.0004, 0.01, 0.05, 0.2, 0.5, 1];
      const targets = ranked.slice(0, VALUES.length);
      check(targets.every((t) => t.px.length >= 12), 'P4 setup: eight Area B cells have enough clean floor pixels to read the real layer', targets.map((t) => t.px.length));
      // inject the chosen L values for those cells into ONE re-read (display only); a normal re-read restores the engine values
      const map = {}; targets.forEach((t, i) => { map[t.c.key] = VALUES[i]; });
      await ev(`window.__mutateView = (j) => { const cells = j.stage4.areas.find((a) => a.areaId === 'MA-B').ground; const m = ${J(map)}; for (const c of cells) { const k = c.column + ':' + c.row; if (k in m) c.lightIndex = m[k]; } }`);
      await page.click('#reread'); await settle(400);
      await setLight(true); await sleep(400);
      // The legend strip (a sibling of the map, not part of the layer) takes height from the map pane when the guide is on.
      // Hide ONLY that strip for the capture so the map has the same size in both frames; it is restored afterwards.
      await ev(`document.getElementById('light-legend').hidden = true`); await sleep(300);
      const geoOn = await ev(`(() => { const mr = document.getElementById('map').getBoundingClientRect(); return [mr.left, mr.top, mr.width, mr.height]; })()`);
      check(JSON.stringify(geoOn.map(Math.round)) === JSON.stringify(geo.mapRect.map(Math.round)), 'P4 setup: switching the Light Guide on does not move or resize the map (pixel diff is valid)', { off: geo.mapRect, on: geoOn });
      const onImg = await page.capture(geo.clip);
      await ev(`document.getElementById('light-legend').hidden = false`);
      const dom = await ev(`[...document.querySelectorAll('#light-cells > *')].map((e) => ({ tag: e.localName, cls: e.getAttribute('class'), cell: e.dataset.cell, L: Number(e.dataset.light), fill: e.getAttribute('fill'), op: e.getAttribute('fill-opacity'), computed: getComputedStyle(e).fillOpacity }))`);
      const nodesOn = await ev(`document.getElementById('map').querySelectorAll('*').length`);
      const tagsOn = await nodeTags();
      const tagDelta = {}; for (const k of new Set([...Object.keys(tagsOff), ...Object.keys(tagsOn)])) if ((tagsOn[k] || 0) !== (tagsOff[k] || 0)) tagDelta[k] = (tagsOn[k] || 0) - (tagsOff[k] || 0);
      check(dom.every((d) => d.tag === 'polygon' && d.cls === 'light-cell') && nodesOn - nodesOff === dom.length && JSON.stringify(tagDelta) === J({ polygon: dom.length }), 'AC09: the Light Guide adds ONLY light-cell polygons to the map (no discs, circles, text, numbered band markers)', { polygons: dom.length, addedNodes: nodesOn - nodesOff, perTagDelta: tagDelta });
      const byCell = new Map(dom.map((d) => [d.cell, d]));
      // per-cell composite check on the clean floor pixels, for EVERY drawn cell with enough pixels
      let sampled = 0, maxErr = 0, cellsChecked = 0, bad = 0; const worst = []; const badCells = {}; const bins = {};
      for (const r of ranked) { const d = byCell.get(r.c.key); if (!d || r.px.length < 8) continue; cellsChecked++;
        const a = parseFloat(d.op); const col = [1, 3, 5].map((i) => parseInt(d.fill.slice(i, i + 2), 16));
        for (const [x, y] of r.px) { const got = onImg.px(x, y); const under = offImg.px(x, y); const exp = under.map((b, i) => b * (1 - a) + col[i] * a); const err = Math.max(...got.map((g, i) => Math.abs(g - exp[i]))); sampled++;
          const dd = distAt(x, y); const bin = dd <= 4 ? '3-4' : dd <= 8 ? '5-8' : dd <= 16 ? '9-16' : dd <= 30 ? '17-30' : '>30'; bins[bin] = bins[bin] || { n: 0, bad: 0, maxErr: 0 }; bins[bin].n++; if (err > 3) bins[bin].bad++; if (err > bins[bin].maxErr) bins[bin].maxErr = +err.toFixed(1);
          if (err > 3) { bad++; badCells[r.c.key] = (badCells[r.c.key] || 0) + 1; if (worst.length < 3) worst.push({ cell: r.c.key, L: d.L, at: [x, y], dist: dd, off: under, on: got, expected: exp.map(Math.round), err: +err.toFixed(1) }); }
          if (err > maxErr) maxErr = err; } }
      const farBins = Object.entries(bins).filter(([k]) => k === '17-30' || k === '>30');
      check(maxErr <= 3 || farBins.every(([, v]) => v.maxErr <= 3), 'AC10: on the REAL layer the composite equals under*(1-a)+colour*a for that cell\'s own opacity and colour; any deviation is confined to pixels within 16 px of an overlay (crown/mark/shadow), none farther away (no glow, no hidden bright spot)', { cells: cellsChecked, pixels: sampled, maxChannelError: +maxErr.toFixed(2), badPixels: bad, errorByDistanceToNearestOverlayPx: bins, worst });
      // nothing changes outside the L>0 cells (no halo, no glow, no marker)
      const inMask = new Uint8Array(geo.clip.width * geo.clip.height);
      for (const c of geo.cells) { if (!(c.L > 0) && !(c.key in map && map[c.key] > 0)) continue; const [x0, y0, x1, y1] = bbox(c.poly);
        for (let y = y0 - 2; y <= y1 + 2; y++) for (let x = x0 - 2; x <= x1 + 2; x++) { const px = x + .5, py = y + .5; const lx = x - cx, ly = y - cy; if (lx < 0 || ly < 0 || lx >= geo.clip.width || ly >= geo.clip.height) continue;
          if ([[0, 0], [2, 2], [-2, 2], [2, -2], [-2, -2], [2, 0], [-2, 0], [0, 2], [0, -2]].some(([dx, dy]) => inside(c.poly, px + dx, py + dy))) inMask[ly * geo.clip.width + lx] = 1; } }
      let outsideDiff = 0, firstOut = null;
      for (let y = 0; y < geo.clip.height; y++) for (let x = 0; x < geo.clip.width; x++) { if (inMask[y * geo.clip.width + x]) continue; const a = offImg.px(x, y), b = onImg.px(x, y); if (a[0] !== b[0] || a[1] !== b[1] || a[2] !== b[2]) { outsideDiff++; if (!firstOut) firstOut = [x, y, a, b]; } }
      check(outsideDiff === 0, 'AC09/F-3: Light Guide on vs off differs ONLY inside the drawn cells (no halo, glow, disc or marker anywhere else on the map)', { outsideDiff, firstOut });
      // the injected values: L=0 absent, tiny L positive-but-invisible, monotone and continuous
      const rows = targets.map((t, i) => { const L = VALUES[i]; const d = byCell.get(t.c.key); let delta = 0;
        for (const [x, y] of t.px) { const a = offImg.px(x, y), b = onImg.px(x, y); delta += Math.abs(a[0] - b[0]) + Math.abs(a[1] - b[1]) + Math.abs(a[2] - b[2]); }
        return { L, drawn: !!d, op: d ? d.op : null, fill: d ? d.fill : null, computed: d ? d.computed : null, meanSummedRgbDelta: +(delta / t.px.length).toFixed(2) }; });
      info('ramp on the real layer (L -> written opacity, fill, computed, mean summed RGB change per floor pixel)', rows);
      const drawn = rows.filter((r) => r.L > 0);
      const ops = drawn.map((r) => parseFloat(r.op));
      check(!rows[0].drawn && rows[0].meanSummedRgbDelta === 0, 'AC10: L = 0 draws nothing and changes no pixel', rows[0]);
      check(drawn.every((r) => r.drawn && parseFloat(r.op) > 0 && !/^0*(\.0*)?$/.test(r.op) && Number(r.computed) < 1), 'AC10: EVERY positive L (down to 1e-9) has a strictly positive written opacity that never reads "0" and Edge parses it (computed value is not the default 1)', drawn.map((r) => [r.L, r.op, r.computed]));
      check(ops.every((v, i) => i === 0 || v > ops[i - 1]), 'AC10: opacity strictly increases over L = 1e-9, 0.0004, 0.01, 0.05, 0.2, 0.5, 1 (no plateau, no floor)', ops);
      const dl = drawn.map((r) => r.meanSummedRgbDelta);
      check(dl.every((v, i) => i === 0 || v >= dl[i - 1]) && dl[dl.length - 1] > dl[dl.length - 2] && drawn.find((r) => r.L === 0.01).meanSummedRgbDelta > 0 && drawn.find((r) => r.L === 0.01).meanSummedRgbDelta <= 24,
        'AC10: measured pixel change rises monotonically; L = 0.01 is faint but nonzero; L = 1 is the brightest', dl);
      const r1 = rows.find((r) => r.L === 1);
      check(r1.fill === '#ffd9bd' && parseFloat(r1.op) >= 0.85, 'AC10: L = 1 is bright pale peach (#ffd9bd at opacity >= 0.85)', r1);
      // the whole real-cell population (no injection effects): monotone paint over all drawn cells
      const sorted = dom.slice().sort((p, q) => p.L - q.L);
      const lum = (hex) => [1, 3, 5].reduce((s, i) => s + parseInt(hex.slice(i, i + 2), 16), 0);
      check(sorted.every((c, i) => i === 0 || (parseFloat(c.op) >= parseFloat(sorted[i - 1].op) && lum(c.fill) >= lum(sorted[i - 1].fill))), 'AC10: across all drawn cells opacity and colour brightness never decrease as L increases', { cells: sorted.length });
      // hue / contrast against the canopy
      const hue = (hex) => { const [r, g, b] = [1, 3, 5].map((i) => parseInt(hex.slice(i, i + 2), 16) / 255); const mx = Math.max(r, g, b), mn = Math.min(r, g, b), d = mx - mn; if (!d) return 0; let h = mx === r ? ((g - b) / d) % 6 : mx === g ? (b - r) / d + 2 : (r - g) / d + 4; h *= 60; return Math.round(h < 0 ? h + 360 : h); };
      // sample the REAL canopy pixels (Light-Guide-off frame) at the centres of the visible crowns and compare with the ramp's brightest composites
      const crownPts = await ev(`(async () => { const p = await window.__rv.view(); return p.scenes['MA-B'].filter((t) => t.status === 'Living').slice(0, 40).map((t) => window.__rv.screenOf(t.u, t.v)).filter((a) => a.inView && a.onCrown).map((a) => [a.x, a.y]); })()`);
      const rel = (rgb) => { const f = (c) => { c /= 255; return c <= 0.03928 ? c / 12.92 : Math.pow((c + 0.055) / 1.055, 2.4); }; return 0.2126 * f(rgb[0]) + 0.7152 * f(rgb[1]) + 0.0722 * f(rgb[2]); };
      const contrast = (a, b) => { const x = rel(a), y = rel(b); return +((Math.max(x, y) + 0.05) / (Math.min(x, y) + 0.05)).toFixed(2); };
      const cs = crownPts.map(([x, y]) => offImg.px(Math.round(x - cx), Math.round(y - cy))).filter((p) => Math.max(...p) < 140);
      const crownMean = cs.length ? [0, 1, 2].map((i) => Math.round(cs.reduce((s, p) => s + p[i], 0) / cs.length)) : null;
      const compo = (L) => { const t = Math.pow(L, 0.65); const stops = [[0x7f, 0x3b, 0x4a], [0xda, 0x79, 0x6d], [0xff, 0xd9, 0xbd]]; const seg = t <= 0.5 ? 0 : 1; const f = seg === 0 ? t / 0.5 : (t - 0.5) / 0.5; const c = stops[seg].map((lo, i) => lo + (stops[seg + 1][i] - lo) * f); const a = 0.9 * t; return floor.map((b, i) => Math.round(b * (1 - a) + c[i] * a)); };
      info('hues of the ramp (degrees), canopy sample and WCAG contrast of composited light over the real floor vs the canopy', { rose: hue('#7f3b4a'), coral: hue('#da796d'), peach: hue('#ffd9bd'), canopyMeanRgb: crownMean, canopySamples: cs.length,
        compositeL05: compo(0.5), compositeL1: compo(1), contrastL1VsFloor: contrast(compo(1), floor), contrastL05VsFloor: contrast(compo(0.5), floor), contrastL1VsCanopy: crownMean ? contrast(compo(1), crownMean) : null, contrastFloorVsCanopy: crownMean ? contrast(floor, crownMean) : null });
      await shot('P4-light-area-b-injected');
      await page.eval(`window.__mutateView = null`);
      await page.click('#reread'); await settle(400);
      const restored = await ev(`document.querySelectorAll('#light-cells polygon').length`);
      const real = geo.cells.filter((c) => c.L > 0).length;
      check(restored === real, 'P4: a normal re-read restores the engine values (every real positive cell drawn, nothing injected remains)', { restored, real });
      // area A: the real thing, no injection: only L>0 cells drawn
      await showArea('MA-A'); await sleep(300);
      const a = await ev(`(async () => { const p = await window.__rv.view(); const cells = p.stage4.areas.find((x) => x.areaId === 'MA-A').ground; const drawn = [...document.querySelectorAll('#light-cells polygon')];
        return { cells: cells.length, positive: cells.filter((c) => c.lightIndex > 0).length, zero: cells.filter((c) => c.lightIndex === 0).length, drawn: drawn.length, minL: Math.min(...cells.map((c) => c.lightIndex)), maxL: Math.max(...cells.map((c) => c.lightIndex)),
          legend: document.getElementById('light-legend').innerText.replace(/\\s+/g, ' '), mapText: document.getElementById('map-pane').innerText.replace(/\\s+/g, ' ') }; })()`);
      check(a.drawn === a.positive, 'AC10: Area A (real engine values, almost entirely very dark) draws exactly the cells with L > 0; no light is invented', a);
      check(/^Light Guide/.test(a.legend) && /Shaded/.test(a.legend) && /Bright/.test(a.legend) && !/band|not measured|PAR|calibrat|k10a10|modelled light/i.test(a.mapText), 'AC14/UX-09: with the Light Guide on the map says only "Light Guide", Shaded->Bright, and repeats no disclaimer', a.legend);
      await shot('P4-light-area-a');
      await setLight(false);
    });
    await section('P4b', async () => {
      await showArea('MA-B'); await closeInspect(); await setLight(true); await sleep(300);
      const key = await ev(`document.querySelector('#light-cells polygon').dataset.cell`);
      const [col, row] = key.split(':').map(Number);
      await ev(`window.__mutateView = (j) => { for (const c of j.stage4.areas.find((a) => a.areaId === 'MA-B').ground) if (c.column === ${col} && c.row === ${row}) c.lightIndex = 1e-200; }`);
      await page.click('#reread'); await settle(450);
      const r = await ev(`(() => { const e = document.querySelector('#light-cells polygon[data-cell="${key}"]'); return e ? { op: e.getAttribute('fill-opacity'), computed: getComputedStyle(e).fillOpacity, L: e.dataset.light } : null; })()`);
      info('L = 1e-200 (far below any real cell): written opacity and the value Edge computes from it', r);
      check(r && parseFloat(r.op) > 0 && !/^0*(\.0*)?$/.test(r.op), 'AC10: even L = 1e-200 is written as a positive opacity (the text never reads "0")', r);
      await page.click('#reread'); await settle(450); await setLight(false);
    });

    // ----- shared helpers for P5..P9 ---------------------------------------------------------------------------------------
    const crownTarget = async (areaId, exclude = []) => {
      const items = await rvx(`rv.view().then((p) => p.scenes[${J(areaId)}].filter((t) => t.status === 'Living').map((t) => ({ treeId: t.treeId, u: t.u, v: t.v })))`);
      for (const t of items) {
        if (exclude.includes(t.treeId)) continue;
        const at = await ev(`window.__rv.screenOf(${t.u}, ${t.v})`);
        if (!(at.inView && at.onCrown && !at.onRank)) continue;
        const hit = await rvx(`rv.hit(${J(areaId)}, ${t.u}, ${t.v})`);
        if (hit && hit.length > 0 && !exclude.includes(hit[0])) return { tree: t.treeId, u: t.u, v: t.v, hit, at };
      }
      return null;
    };
    const openCells = (areaId) => ev(`(async () => { const rv = window.__rv; const p = await rv.view(); const fence = rv.fence(p.view); const area = p.stage4.areas.find((a) => a.areaId === ${J(areaId)});
      const inside = (pts, u, v) => { let ins = false; for (let i = 0, j = pts.length - 1; i < pts.length; j = i++) { const a = pts[i], b = pts[j]; if ((a.v > v) !== (b.v > v) && u < (b.u - a.u) * (v - a.v) / (b.v - a.v) + a.u) ins = !ins; } return ins; };
      const crowns = p.scenes[${J(areaId)}].filter((t) => t.status === 'Living'); const out = [];
      for (const c of area.ground) { const u = c.polygon.reduce((s, q) => s + q.u, 0) / c.polygon.length, v = c.polygon.reduce((s, q) => s + q.v, 0) / c.polygon.length; if (!inside(c.polygon, u, v)) continue;
        const r = await rv.post('api/multi/hit', { fence, areaId: ${J(areaId)}, u, v });
        if (r.json && r.json.candidates && r.json.candidates.length === 0) out.push({ u, v, key: c.column + ':' + c.row, col: c.column, row: c.row, L: c.lightIndex, cover: c.geometricCoveredFraction, n: c.contributingTreeIds.length, ids: c.contributingTreeIds,
          clear: Math.min(...crowns.map((t) => Math.hypot(t.u - u, t.v - v) - t.radiusM)) }); }
      return { cells: area.ground.length, open: out }; })()`);
    const advanceFence = async (areaId, treeId) => {
      const r = await rvx(`rv.cropMark(${J(areaId)}, ${J(treeId)})`);
      if (r.status === 200) return 'marked';
      const u = await ev(`(async () => { const p = await window.__rv.view(); return window.__rv.post('api/multi/crop/unmark', { fence: window.__rv.fence(p.view), areaId: ${J(areaId)}, treeId: ${J(treeId)} }); })()`);
      return u.status === 200 ? 'unmarked' : 'failed:' + r.status + '/' + u.status;
    };

    // ===== P5: crown wins; ground inspector content ====================================================================
    let GROUND_SPOT = null;
    await section('P5', async () => {
      await showArea('MA-B'); await closeInspect(); await page.click('#zoom-fit'); await setGround(false); await setLight(true);
      const sBefore = await rvx(`rv.snap()`);
      const t1 = await crownTarget('MA-B', []);
      await page.clickAt(t1.at.x, t1.at.y); await sleep(300); await waitCompetition();
      check(await ev(`window.__rv.inspected()`) === t1.hit[0] && (await ev(`document.querySelectorAll('#light-cells polygon').length`)) > 0, 'AC12: with the Light Guide ON a click on a crown still selects that tree (hit-test topmost ' + t1.hit[0] + ')', await ev(`window.__rv.state()`));
      const rk = await ev(`window.__rv.ranks()`);
      await page.clickAt(rk[0].x, rk[0].y); await sleep(200);
      check((await ev(`window.__rv.ranks()`))[0].pressed === 'true', 'AC12: numbered rank selection still works with the Light Guide ON');
      await setGround(true);
      const t2 = await crownTarget('MA-B', [t1.hit[0]]);
      await page.clickAt(t2.at.x, t2.at.y); await sleep(300); await waitCompetition();
      const g2 = await ev(`({ insp: window.__rv.inspected(), pick: document.querySelectorAll('#ground-mark polygon').length, ground: document.getElementById('ground-mode').getAttribute('aria-pressed'), heading: document.getElementById('ground-heading').textContent, techHidden: document.getElementById('ground-tech').hidden })`);
      check(g2.insp === t2.hit[0] && g2.pick === 0 && g2.ground === 'true' && g2.techHidden, 'AC12/UX-08: Inspect ground ON: a crown click SELECTS the tree (crown wins), no ground read appears, the mode stays on', g2);
      const rk2 = await ev(`window.__rv.ranks()`); const pb = rk2.map((r) => r.pressed);
      const free = rk2.findIndex((r) => r.disabled !== 'true' && r.pressed === 'false');
      await page.clickAt(rk2[free].x, rk2[free].y); await sleep(200);
      const pa = (await ev(`window.__rv.ranks()`)).map((r) => r.pressed);
      check(pa.filter((p, i) => p !== pb[i]).length === 1 && pa[free] === 'true' && (await ev(`window.__rv.inspected()`)) === t2.hit[0] && (await ev(`document.querySelectorAll('#ground-mark polygon').length`)) === 0,
        'AC12: with Inspect ground ON a rank click toggles only that ID; no selection change, no ground read (rank outranks crown outranks ground)', { before: pb, after: pa });
      // how much open ground is there? the owner-accepted UX consequence, measured independently
      const oA = await openCells('MA-A'), oB = await openCells('MA-B');
      info('independent measurement: cell centres with NO crown under the server hit-test', { A: oA.open.length + ' of ' + oA.cells + ' (' + (100 * oA.open.length / oA.cells).toFixed(1) + '%)', B: oB.open.length + ' of ' + oB.cells + ' (' + (100 * oB.open.length / oB.cells).toFixed(1) + '%)' });
      // read an open spot that is on screen, on the map, under no crown/rank/dock
      let spot = null;
      for (const o of oB.open.sort((p, q) => q.clear - p.clear)) { const at = await ev(`window.__rv.screenOf(${o.u}, ${o.v})`); if (at.inView && at.onMap && !at.onCrown && !at.onRank && at.top === 'map' || (at.inView && at.onMap && !at.onCrown && !at.onRank)) { spot = { ...o, at }; break; } }
      check(spot !== null, 'P5 setup: an on-screen open-ground spot exists in Area B', spot && { key: spot.key, clearM: +spot.clear.toFixed(2), L: spot.L });
      GROUND_SPOT = spot;
      const before = { insp: await ev(`window.__rv.inspected()`), add: await ev(`document.getElementById('add-button').textContent.trim()`), ranks: await ev(`window.__rv.ranks().map((r) => r.pressed).join()`), snap: await rvx(`rv.snap()`) };
      await page.clickAt(spot.at.x, spot.at.y); await sleep(350);
      const g = await ev(`({ heading: document.getElementById('ground-heading').textContent, summary: document.getElementById('ground-summary').innerText.replace(/\\s+/g, ' '), treesOpen: document.getElementById('ground-trees').open, techOpen: document.getElementById('ground-tech').open,
        ids: document.getElementById('ground-tree-ids').textContent, treesSummary: document.getElementById('ground-trees-summary').textContent, tech: document.getElementById('ground-facts').textContent, caveat: document.getElementById('ground-caveat').textContent,
        pick: document.querySelectorAll('#ground-mark polygon').length, insp: window.__rv.inspected(), add: document.getElementById('add-button').textContent.trim(), ranks: window.__rv.ranks().map((r) => r.pressed).join() })`);
      const afterSnap = await rvx(`rv.snap()`);
      check(g.pick === 2 && /ground at X/.test(g.heading) && g.insp === before.insp && g.add === before.add && g.ranks === before.ranks && afterSnap.hash === before.snap.hash,
        'AC12/AC21: an open-ground click shows the ground inspector for that cell; selection, draft, queued orders and the server view are untouched', { heading: g.heading, insp: g.insp });
      const word = /Very shaded|Low|Moderate|Bright/.exec(g.summary);
      check(word && g.summary.includes('Light index ' + spot.L.toFixed(3)) && g.summary.includes('Canopy cover ' + (spot.cover * 100).toFixed(0) + '%') && g.summary.includes('Shading trees ' + (spot.n || 'none')) && !spot.ids.some((id) => g.summary.includes(id)),
        'AC13/UX-07: concise summary = friendly level word + the engine light index + the SEPARATE engine canopy cover + contributing-tree count; no tree IDs in the top summary', g.summary);
      check(!g.treesOpen && !g.techOpen && g.treesSummary === 'Contributing trees (' + spot.n + ')' && spot.ids.every((id) => g.ids.includes(id)) && g.tech.includes('column ' + spot.col + ', row ' + spot.row) && /2\.5 m cells/.test(g.tech) && /k10a10/i.test(g.tech) && /Calibrated PAR/.test(g.tech) && g.caveat.length > 20,
        'AC13: contributing IDs, cell coordinates, grid size, model/provenance and calibration status are kept behind closed disclosures (moved, not removed)', { treesSummary: g.treesSummary, tech: g.tech.slice(0, 400), caveat: g.caveat.slice(0, 160) });
      await shot('P5-ground-read');
      // a tiny positive L, injected into this one cell (display only), read in the concise summary
      for (const L of [0.0004, 0.00004]) {
        await ev(`window.__mutateView = (j) => { for (const c of j.stage4.areas.find((a) => a.areaId === 'MA-B').ground) if (c.column === ${spot.col} && c.row === ${spot.row}) c.lightIndex = ${L}; }`);
        await page.click('#reread'); await settle(350);
        const t = await ev(`({ summary: document.getElementById('ground-summary').innerText.replace(/\\s+/g, ' '), tech: document.getElementById('ground-facts').textContent })`);
        info('tiny positive L injected into the inspected cell (display check only)', { L, summary: t.summary.slice(0, 160), technicalIndex: /Modelled light index([0-9.]+)/.exec(t.tech.replace(/\s+/g, ''))?.[1] });
      }
      await page.click('#reread'); await settle(350);
      // outside the area: no sample
      const outside = await ev(`(async () => { const p = await window.__rv.view(); const a = p.view.areas.find((x) => x.areaId === 'MA-B'); const side = a.extent.sideM; const low = a.polygon.map((q) => ({ u: q.xm - a.extent.minXM, v: a.extent.minZM + side - q.zm })).sort((x, y) => y.v - x.v)[0]; return window.__rv.screenOf(low.u, low.v + 1.2); })()`);
      if (outside.inView && outside.onMap && !outside.onCrown) { await page.clickAt(outside.x, outside.y); await sleep(350);
        const o = await ev(`({ heading: document.getElementById('ground-heading').textContent, summary: document.getElementById('ground-summary').innerText.replace(/\\s+/g, ' '), tech: document.getElementById('ground-tech').hidden })`);
        check(/no sample/.test(o.heading) && /No modelled ground sample/.test(o.summary) && o.tech, 'UX-08: a point with no ground cell reports "no sample" and synthesises nothing', o); }
      else info('outside-area point not on screen in this viewport', outside);
      // a modal swallows map clicks in ground mode: zero requests
      await page.click('#advance'); await page.waitFor(`document.getElementById('consent-dialog').open`, 'consent dialog', 8000);
      const rq = page.log.all.length, st0 = await ev(`window.__rv.state()`);
      await page.clickAt(spot.at.x, spot.at.y); await sleep(350);
      const st1 = await ev(`window.__rv.state()`);
      check(page.log.all.length === rq && await ev(`document.getElementById('consent-dialog').open`) && st1.insp === st0.insp, 'UX-08: with a modal dialog open a map click is swallowed: zero requests, no read, no selection', { requests: page.log.all.length - rq });
      await page.key('Escape');
      const sAfter = await rvx(`rv.snap()`);
      check(sAfter.hash === sBefore.hash || JSON.stringify({ ...sAfter, hash: 0 }) === JSON.stringify({ ...sBefore, hash: 0 }), 'AC21: Light Guide, ground mode and inspection (a whole section of them) changed nothing on the server', { before: sBefore.rev, after: sAfter.rev });
      await setGround(false); await closeInspect();
    });

    // ===== P6: layout with Inspect ground ON (the repaired defect 1) ===================================================
    await section('P6', async () => {
      await showArea('MA-B'); await closeInspect(); await page.click('#zoom-fit'); await setLight(true); await setGround(true);
      const nothing = await ev(`({ parent: document.getElementById('ground-panel').parentNode.id, first: document.getElementById('pane-tree').firstElementChild.id, hidden: document.getElementById('ground-panel').hidden })`);
      check(nothing.parent === 'pane-tree' && nothing.first === 'ground-panel' && !nothing.hidden, 'P6: Inspect ground ON with nothing inspected: the panel sits at the top of the Tree pane', nothing);
      const used = []; const rows = [];
      for (let i = 0; i < 6; i++) {
        const t = await crownTarget('MA-B', used); if (!t) break;
        await page.clickAt(t.at.x, t.at.y); await sleep(300); await waitCompetition();
        used.push(t.hit[0], t.tree);
        const f = await ev(`window.__rv.fold()`);
        rows.push({ tree: await ev(`window.__rv.inspected()`), ok: foldOk(f), n: f.n, scrollTop: f.scrollTop, lastBottom: f.lastBottom, dockTop: f.dockTop, addBottom: f.addBottom, innerHeight: f.innerHeight, groundParent: f.groundParent, groundTop: f.groundTop, dockBottom: f.dockBottom });
      }
      check(rows.length >= 5 && rows.every((r) => r.ok && r.groundParent === 'ground-slot' && r.groundTop >= r.dockBottom - 1), 'AC01 (attack 1): with Inspect ground ON, EVERY crown selection leaves the reference, five rows and Add reachable above the fold with no scrolling; the ground panel waits below the Add bar', rows);
      await shot('P6-ground-on-crown-selected');
      // all five ticked
      const rk = await ev(`window.__rv.ranks()`);
      for (const r of rk) { if (r.pressed === 'false' && r.disabled !== 'true') { await page.clickAt(r.x, r.y); await sleep(120); } }
      const f5 = await ev(`window.__rv.fold()`);
      check(foldOk(f5) && /Add selected \d/.test(f5.addText), 'AC01 (attack 1): with ranks ticked and ground ON the one-line "' + f5.addText + '" button and every row are still above the fold', f5);
      // scrolled inspector, then another tree by MAP, then by EXACT ID
      const scrollDown = () => ev(`(() => { const p = document.getElementById('pane-tree'); p.scrollTop = p.scrollHeight; return p.scrollTop; })()`);
      const s1 = await scrollDown();
      const tm = await crownTarget('MA-B', used);
      await page.clickAt(tm.at.x, tm.at.y); await sleep(300); await waitCompetition();
      const fm = await ev(`window.__rv.fold()`);
      check(s1 > 0 && foldOk(fm), 'AC01 (attack 1): inspector scrolled down (scrollTop ' + s1 + '), then a DIFFERENT tree selected BY MAP: the primary block is visible again with no manual scrolling', { scrolledFrom: s1, fold: fm });
      used.push(tm.hit[0], tm.tree);
      const s2 = await scrollDown();
      const idsB = (await livingIds()).find((l) => l.areaId === 'MA-B').ids.filter((id) => !used.includes(id));
      await selectById(idsB[3]);
      const fe = await ev(`window.__rv.fold()`);
      check(s2 > 0 && foldOk(fe), 'AC01 (attack 1): inspector scrolled down (scrollTop ' + s2 + '), then a different tree selected BY EXACT ID: primary block visible again', { scrolledFrom: s2, fold: fe });
      // open-ground read with a tree inspected: what is on screen afterwards?
      if (GROUND_SPOT) {
        const at = await ev(`window.__rv.screenOf(${GROUND_SPOT.u}, ${GROUND_SPOT.v})`);
        await page.clickAt(at.x, at.y); await sleep(400);
        const fr = await ev(`window.__rv.fold()`);
        const vis = await ev(`(() => { const reach = window.__rv.reach; return { refId: reach(document.getElementById('insp-id')), close: reach(document.getElementById('inspect-close')), rowsReachable: [...document.querySelectorAll('#competition-content .comp-row')].filter((r) => reach(r)).length, rowsTotal: document.querySelectorAll('#competition-content .comp-row').length,
          add: reach(document.getElementById('add-button')), groundHeading: reach(document.getElementById('ground-heading')), groundSummary: reach(document.getElementById('ground-summary')), focus: document.activeElement.id, scrollTop: document.getElementById('pane-tree').scrollTop }; })()`);
        info('after an OPEN-GROUND read with a tree inspected (writer chose scrollIntoView on the reading)', vis);
        info('geometry after the read', { paneTop: fr.paneTop, paneBottom: fr.paneBottom, groundTop: fr.groundTop, groundBottom: fr.groundBottom, addTop: fr.addTop, addBottom: fr.addBottom, innerHeight: fr.innerHeight });
        check(vis.groundSummary && vis.add && vis.groundHeading, 'UX-07/AC13: after an open-ground read the reading (heading + summary) is visible and the Add bar is still reachable', vis);
        await shot('P6-ground-read-with-tree-inspected');
        const tn = await crownTarget('MA-B', used.concat(idsB[3]));
        await page.clickAt(tn.at.x, tn.at.y); await sleep(300); await waitCompetition();
        const fn = await ev(`window.__rv.fold()`);
        check(foldOk(fn), 'AC01 (attack 1): after that scrolled ground reading, the NEXT tree selection restores the primary block with no manual scrolling', fn);
      }
      // keyboard: the ground toggle with Space; focus behaviour with a tree inspected
      await setGround(false);
      await ev(`document.getElementById('ground-mode').focus()`);
      await page.key(' ');
      const kb = await ev(`({ pressed: document.getElementById('ground-mode').getAttribute('aria-pressed'), focus: document.activeElement.id, scrollTop: document.getElementById('pane-tree').scrollTop, parent: document.getElementById('ground-panel').parentNode.id })`);
      check(kb.pressed === 'true' && kb.focus === 'ground-mode' && kb.scrollTop === 0 && kb.parent === 'ground-slot', 'UX-08: Inspect ground is keyboard operable (Space), shows an obvious pressed state, keeps focus on the toggle and does not scroll the inspector when a tree is inspected', kb);
      await setGround(false); await closeInspect();
    });

    // ===== P7: stale hit-test answers (the repaired defect 2) ==========================================================
    await section('P7', async () => {
      await setLight(false); await setGround(false);
      await page.click('#reread'); await settle(350); await closeInspect();
      const T = [];
      const modeOf = { ok: 'hold-ans', refused: 'hold-req', guard: 'guard', transport: 'transport' };
      async function staleCase(kind, action, areaId) {
        await page.click('#reread'); await settle(350);
        await setGround(false); await closeInspect(); await showArea(areaId);
        const other = areaId === 'MA-A' ? 'MA-B' : 'MA-A';
        const ids = (await livingIds()).find((l) => l.areaId === areaId).ids;
        const R0 = ids[0], R1 = ids[1], C0 = ids[12];
        await selectById(R0);
        const rk = await ev(`window.__rv.ranks()`);
        if (rk.length && action !== 'escape' && action !== 'close') { await page.clickAt(rk[0].x, rk[0].y); await sleep(150); }
        const draft0 = await ev(`document.getElementById('add-button').textContent.trim()`);
        const target = await crownTarget(areaId, [R0, R1, C0]);
        const S0 = await rvx(`rv.snap()`);
        await ev(`window.__held = 0; window.__mode = ${J(modeOf[kind])}`);
        await page.clickAt(target.at.x, target.at.y);
        await page.waitFor(`window.__held >= 1`, 'held ' + kind, 8000);
        const reqMark = await ev(`window.__reqs.length`);
        let srv = S0, advanced = null;
        if (kind === 'refused') { advanced = await advanceFence(areaId, C0); srv = await rvx(`rv.snap()`); }
        if (action === 'escape') await page.key('Escape');
        else if (action === 'close') await page.click('#inspect-close');
        else if (action === 'newer') { await ev(`(() => { const q = document.getElementById('quick-id'); q.value = ${J(R1)}; q.focus(); })()`); await page.key('Enter'); await page.waitFor(`window.__rv.inspected() === ${J(R1)}`, 'newer selection', 5000); }
        else if (action === 'area') { await page.click('.area-tab[data-area="' + other + '"]'); await sleep(200); }
        await sleep(250);
        const P1 = await ev(`window.__rv.state()`);
        await ev(`window.__open()`); await sleep(900);
        if (action === 'newer') await waitCompetition(7000).catch(() => {});
        const P2 = await ev(`window.__rv.state()`);
        const reqs = (await ev(`window.__reqs.slice(${reqMark})`));
        const S1 = await rvx(`rv.snap()`);
        let after = null;
        if (action === 'area') { await showArea(areaId); await sleep(500); after = await ev(`({ insp: window.__rv.inspected(), add: document.getElementById('add-button').textContent.trim() })`); }
        const cropRow = await ev(`(() => { const tr = [...document.querySelectorAll('#tree-rows tr')].find((r) => r.textContent.includes(${J(C0)})); return tr ? /crop/i.test(tr.textContent) : null; })()`);
        const row = { kind, action, areaId, hitTopmost: target.hit[0], R0, R1, advanced, P1: { insp: P1.insp, area: P1.area, announce: P1.announce.slice(0, 70) }, P2: { insp: P2.insp, area: P2.area, announce: P2.announce.slice(0, 70), alert: P2.alert.slice(0, 80), alertShown: P2.alertShown },
          reqsAfterRelease: reqs, serverUnchanged: S1.hash === srv.hash, pageShowsAdvancedCrop: cropRow, draft0, after };
        T.push(row); return { row, P1, P2, S0, srv, S1, reqs, R0, R1, target, after, draft0 };
      }
      const stale = (kind, action) => async () => { };
      const matrix = [];
      for (const kind of ['ok', 'refused', 'guard', 'transport']) for (const action of ['escape', 'close', 'newer', 'area']) matrix.push([kind, action]);
      const verdicts = [];
      for (const [kind, action] of matrix) {
        let c; try { c = await staleCase(kind, action, action === 'area' || action === 'newer' ? 'MA-B' : 'MA-A'); } catch (e) { verdicts.push({ kind, action, error: String(e).slice(0, 160) }); continue; }
        const { row, P1, P2, srv, S1, reqs, R0, R1, target, after, draft0 } = c;
        const expectInsp = action === 'escape' || action === 'close' ? null : action === 'newer' ? R1 : undefined;
        const noWrite = reqs.every((r) => r === 'POST api/multi/competition');
        const okInsp = expectInsp === undefined ? (P2.area === P1.area && after && after.insp === R0 && after.add === draft0) : (P2.insp === expectInsp);
        const okAlert = P2.alert === '' && !P2.alertShown;
        const okAnn = P2.announce === P1.announce && !P2.announce.includes(target.hit[0]);
        const ok = okInsp && okAlert && okAnn && noWrite && row.serverUnchanged;
        verdicts.push({ kind, action, ok, okInsp, okAlert, okAnn, noWrite, serverUnchanged: row.serverUnchanged, reqs: [...new Set(reqs)], pageShowsAdvancedCrop: row.pageShowsAdvancedCrop });
      }
      check(verdicts.length === 16 && verdicts.every((v) => v.ok), 'DEFECT-2: for ok / refused / guard / transport answers, each held in flight while Escape, Close inspection, an area switch or a newer selection happens, the late answer is dropped whole: nothing reselected, no draft cleared, no alert, no announcement, no write request, server unchanged', verdicts);
      // controls: the SAME answers, not stale, must be visible - proves the silence above is the stale check, not a broken injection
      const ctrl = [];
      for (const kind of ['ok', 'refused', 'guard', 'transport']) {
        await page.click('#reread'); await settle(350); await setGround(false); await closeInspect(); await showArea('MA-B');
        const ids = (await livingIds()).find((l) => l.areaId === 'MA-B').ids; const R0 = ids[0], C0 = ids[12];
        await selectById(R0);
        const target = await crownTarget('MA-B', [R0, ids[1], C0]);
        await ev(`window.__held = 0; window.__mode = ${J(modeOf[kind])}`);
        await page.clickAt(target.at.x, target.at.y);
        await page.waitFor(`window.__held >= 1`, 'held control ' + kind, 8000);
        let adv = null; if (kind === 'refused') adv = await advanceFence('MA-B', C0);
        await ev(`window.__open()`); await sleep(1000);
        const s = await ev(`window.__rv.state()`);
        const cropRow = await ev(`(() => { const tr = [...document.querySelectorAll('#tree-rows tr')].find((r) => r.textContent.includes(${J(C0)})); return tr ? /crop/i.test(tr.textContent) : null; })()`);
        ctrl.push({ kind, adv, insp: s.insp, expectHit: target.hit[0], alertShown: s.alertShown, alert: s.alert.slice(0, 120), pageShowsAdvancedCrop: cropRow });
      }
      check(ctrl[0].insp === ctrl[0].expectHit && ctrl.slice(1).every((c) => c.alertShown && c.alert.length > 10 && c.insp !== c.expectHit), 'DEFECT-2 control: the same answers, NOT stale, ARE applied (ok selects the tree) or shown (refused / guard / transport raise a visible alert and select nothing)', ctrl);
      info('stale-REFUSED side effect: the newer server view the refused answer carried', { staleDropPageShowsAdvancedCrop: verdicts.find((v) => v.kind === 'refused' && v.action === 'escape').pageShowsAdvancedCrop, nonStaleControlShowsIt: ctrl[1].pageShowsAdvancedCrop });
      // fail-closed follow-up: a page left on an older view cannot write through it
      await page.click('#reread'); await settle(350); await setGround(false); await closeInspect(); await showArea('MA-B');
      {
        const ids = (await livingIds()).find((l) => l.areaId === 'MA-B').ids; const R0 = ids[0], R1 = ids[1], C0 = ids[12];
        await selectById(R0);
        const target = await crownTarget('MA-B', [R0, R1, C0]);
        await ev(`window.__held = 0; window.__mode = 'hold-req'`);
        await page.clickAt(target.at.x, target.at.y); await page.waitFor(`window.__held >= 1`, 'held', 8000);
        await advanceFence('MA-B', C0);
        await page.key('Escape'); await sleep(200);
        await ev(`window.__open()`); await sleep(900);
        await selectById(R1);
        const srvBefore = await rvx(`rv.snap()`);
        await page.click('#crop-button'); await sleep(900);
        const fo = await ev(`({ alert: document.getElementById('alert').textContent.slice(0, 200), shown: document.getElementById('alert').classList.contains('shown') })`);
        const srvAfter = await rvx(`rv.snap()`);
        check(fo.shown && /Refused|stale|PlayerPlanStateStale|PlayerSourceStale/i.test(fo.alert) && JSON.stringify(srvAfter.crops) === JSON.stringify(srvBefore.crops) && srvAfter.rev === srvBefore.rev,
          'AC15/DEFECT-2: after a stale REFUSED answer was dropped, the page is on an older view; its next fenced command (Crop on ' + R1 + ') is refused by the server (409), nothing is written, and the refusal is shown (fails closed)', { alert: fo.alert, revBefore: srvBefore.rev, revAfter: srvAfter.rev });
      }
      // exploratory races suggested by reading the code (no pass/fail: reported as observations)
      await page.click('#reread'); await settle(350); await setGround(false); await closeInspect(); await showArea('MA-A');
      {
        const ids = (await livingIds()).find((l) => l.areaId === 'MA-A').ids; const R0 = ids[0];
        await selectById(R0); const t = await crownTarget('MA-A', [R0, ids[1]]);
        await ev(`window.__held = 0; window.__mode = 'hold-ans'`);
        await page.clickAt(t.at.x, t.at.y); await page.waitFor(`window.__held >= 1`, 'held', 8000);
        let opened = false; try { await page.click('#advance'); await page.waitFor(`document.getElementById('consent-dialog').open`, 'dialog', 2500); opened = true; } catch { opened = false; }
        if (opened) await page.key('Escape'); await sleep(200);
        await ev(`window.__open()`); await sleep(900);
        info('EXPLORE E1: a dialog-only Escape pressed while a map click is in flight', { dialogOpenedWhileBusy: opened, selectedAfter: await ev(`window.__rv.inspected()`), earlierReference: R0, lateTopmost: t.hit[0], interpretation: 'if selectedAfter equals earlierReference the map click was silently cancelled by an Escape that only dismissed a dialog' });
        await ev(`document.getElementById('consent-dialog').open && document.getElementById('consent-dialog').close()`);
      }
      await page.click('#reread'); await settle(350); await closeInspect(); await showArea('MA-A');
      {
        const ids = (await livingIds()).find((l) => l.areaId === 'MA-A').ids; const R0 = ids[0];
        await selectById(R0); const rk = await ev(`window.__rv.ranks()`);
        const t = await crownTarget('MA-A', [R0, ids[1]]);
        await ev(`window.__held = 0; window.__mode = 'hold-ans'`);
        await page.clickAt(t.at.x, t.at.y); await page.waitFor(`window.__held >= 1`, 'held', 8000);
        const rk2 = await ev(`window.__rv.ranks()`);
        if (rk2.length) { await page.clickAt(rk2[0].x, rk2[0].y); await sleep(200); }
        const mid = await ev(`({ add: document.getElementById('add-button').textContent.trim(), pressed: window.__rv.ranks().map((r) => r.pressed).join() })`);
        await ev(`window.__open()`); await sleep(900);
        const end = await ev(`({ insp: window.__rv.inspected(), add: document.getElementById('add-button').textContent.trim(), announce: document.getElementById('announce').textContent.slice(0, 120) })`);
        info('EXPLORE E2: a rank toggled while a map click is in flight', { reference: R0, lateTopmost: t.hit[0], whileHeld: mid, afterRelease: end });
      }
    });

    // ===== P7b: stale REFUSED answer dropped with NOTHING selected afterwards -> the next fenced command must fail closed ====
    await section('P7b', async () => {
      await setLight(false); await setGround(false); await closeInspect();
      await page.click('#reread'); await settle(400);
      await showArea('MA-B');
      const idsB = (await livingIds()).find((l) => l.areaId === 'MA-B').ids;
      const R0 = idsB[0], C0 = idsB[12];
      await selectById(R0);
      const free = (await ev(`window.__rv.ranks()`)).find((r) => r.disabled !== 'true');
      await page.clickAt(free.x, free.y); await sleep(150);
      await page.click('#add-button'); await page.waitFor(`document.getElementById('batch-result').hidden === false`, 'add result', 15000); await settle(400);
      const target = await crownTarget('MA-B', [R0, C0]);
      await ev(`window.__held = 0; window.__mode = 'hold-req'`);
      await page.clickAt(target.at.x, target.at.y); await page.waitFor(`window.__held >= 1`, 'held request', 8000);
      const adv = await advanceFence('MA-B', C0);
      const moved = await rvx(`rv.snap()`);
      await page.key('Escape'); await sleep(200);
      const reqMark = await ev(`window.__reqs.length`);
      await ev(`window.__open()`); await sleep(1000);
      const st = await ev(`window.__rv.state()`);
      const reqs = await ev(`window.__reqs.slice(${reqMark})`);
      check(st.insp === null && st.alert === '' && !st.alertShown && reqs.length <= 1 && reqs.every((r) => r === 'POST api/multi/hit'), 'DEFECT-2: stale REFUSED answer + Escape, nothing selected afterwards: no alert, no selection, no re-read, no write', { adv, st: { insp: st.insp, alert: st.alert, announce: st.announce.slice(0, 60) }, reqs });
      await page.click('[data-pane-button="plan"]'); await sleep(250);
      const before = await rvx(`rv.snap()`);
      const approveEnabled = await ev(`!document.getElementById('approve').disabled`);
      await page.click('#approve'); await sleep(1000);
      const after = await rvx(`rv.snap()`);
      const fo = await ev(`({ alert: document.getElementById('alert').innerText.replace(/\\s+/g, ' ').slice(0, 220), shown: document.getElementById('alert').classList.contains('shown') })`);
      check(approveEnabled && before.approved === null && after.approved === null && fo.shown && /Refused/i.test(fo.alert) && after.rev === moved.rev && after.pending.join() === before.pending.join(),
        'AC15/DEFECT-2: the page was left on an older view (the newer refused view was dropped); its next fenced command (Approve) is REFUSED by the server (409), nothing is approved or written, and the refusal is shown (fails closed)', { approveEnabled, alert: fo.alert, serverRev: { moved: moved.rev, after: after.rev }, approvedAfter: after.approved });
      // exploratory E2 again, in a clean state: a rank toggled while a map click is in flight
      await page.click('[data-pane-button="tree"]'); await page.click('#reread'); await settle(400); await closeInspect();
      let T = null;
      for (const id of idsB.slice(30)) { await selectById(id); const rk = await ev(`window.__rv.ranks()`); if (rk.length === 5 && rk.every((r) => r.disabled !== 'true')) { T = id; break; } }
      if (T) {
        const t = await crownTarget('MA-B', [T, R0, C0]);
        await ev(`window.__held = 0; window.__mode = 'hold-ans'`);
        await page.clickAt(t.at.x, t.at.y); await page.waitFor(`window.__held >= 1`, 'held', 8000);
        const rk2 = await ev(`window.__rv.ranks()`);
        await page.clickAt(rk2[0].x, rk2[0].y); await sleep(250);
        const mid = await ev(`({ add: document.getElementById('add-button').textContent.trim(), pressed: window.__rv.ranks().map((r) => r.pressed).join(',') })`);
        await ev(`window.__open()`); await sleep(900);
        const end = await ev(`({ insp: window.__rv.inspected(), add: document.getElementById('add-button').textContent.trim(), announce: document.getElementById('announce').textContent.slice(0, 140) })`);
        info('EXPLORE E2 (clean state): a rank toggled while a map click is in flight', { reference: T, lateTopmost: t.hit[0], whileHeld: mid, afterRelease: end });
      } else info('EXPLORE E2: no reference with five free ranks found', {});
    });

    // ===== P8/P9: F-2, Work Plan semantics, real small-L data across years ===========================================
    await section('P9', async () => {
      await setLight(false); await setGround(false); await closeInspect();
      await page.click('#reread'); await settle(350);
      // F-2 with an approved plan: Approve first
      await page.click('[data-pane-button="plan"]'); await sleep(200);
      const s0 = await rvx(`rv.snap()`);
      info('before approve', { pending: s0.pending.length, living: s0.living });
      await page.click('#approve'); await settle(500);
      await page.waitFor(`!document.getElementById('approved-box').hidden`, 'approved', 8000);
      const sA = await rvx(`rv.snap()`);
      check(sA.approved && sA.approved.length === s0.pending.length && sA.events === 0 && sA.living.join() === s0.living.join(), 'AC04: Approve approves exactly the queued orders and fells nothing', { approved: sA.approved && sA.approved.length, living: sA.living });
      const sApproved = await sweepLoaded('with an APPROVED plan (Fell orders approved) and a Crop designation');
      check(sApproved.bad.length === 0, 'AC01 (LOADED session, plan APPROVED: competitors read "In the approved plan"): for every living tree the five rows and Add are reachable above the fold', { trees: sApproved.recs.length, failing: sApproved.bad.length, byBlockedRows: sApproved.byBlocked });
      await page.click('[data-pane-button="plan"]'); await sleep(200);
      await ev(`window.__mutateView = (j) => { j.stage4 = null; }`);
      await page.click('#reread'); await settle(500);
      const f2 = await ev(`({ exec: document.getElementById('execute').disabled, execText: document.getElementById('execute').textContent, approve: document.getElementById('approve').disabled, banner: !document.getElementById('readout-unavailable').hidden,
        text: [document.getElementById('disclosure').textContent, document.getElementById('flags').textContent, document.getElementById('details-note').textContent, document.getElementById('statements').textContent, document.getElementById('area-cards').textContent].join(' | '),
        bannerText: document.getElementById('readout-unavailable').textContent.replace(/\\s+/g, ' ') })`);
      check(f2.exec && f2.banner && /unavailable|re-read/i.test(f2.bannerText), 'AC15: with stage4 null the page says the read-out is UNAVAILABLE and offers no Execute', { exec: f2.exec, execText: f2.execText, approveDisabled: f2.approve, banner: f2.bannerText.slice(0, 160) });
      check(!/EDGE NEIGHBOURS EXCLUDED|Light[^|]{0,30}NOT MODELLED|model 0/i.test(f2.text) && /unavailable/i.test(f2.text), 'AC15: stage4 null does not masquerade as model 0 / legacy (no edge-exclusion or "Light NOT MODELLED" statement)', f2.text.slice(0, 300));
      await shot('P9-f2-unavailable');
      await page.click('#readout-reread'); await settle(500);
      check(!(await ev(`document.getElementById('execute').disabled`)), 'AC15: a paired re-read restores Execute');
      // server stale fence is still refused (fail closed): a command with an old fence
      const stale = await ev(`(async () => { const p = await window.__rv.view(); const old = window.__rv.fence(p.view); await window.__rv.cropMark('MA-B', ${J('MA-B-040')}); return window.__rv.post('api/multi/crop/mark', { fence: old, areaId: 'MA-B', treeId: 'MA-B-041' }); })()`);
      check(stale.status === 409 && stale.json && stale.json.refusal, 'AC15: the server still refuses a command carrying a stale fence (HTTP 409 with a refusal)', { status: stale.status, code: stale.json && stale.json.refusal && stale.json.refusal.code });
      // The side-channel crop above advanced the server, so the PAGE is now on a stale view. A real Execute from it must fail closed.
      const evBefore = await rvx(`rv.snap()`);
      await page.click('#execute'); await page.waitFor(`document.getElementById('execute-dialog').open`, 'execute dialog (stale view)', 8000);
      await page.click('#execute-confirm'); await settle(900);
      const evAfter = await rvx(`rv.snap()`);
      const al = await ev(`({ shown: document.getElementById('alert').classList.contains('shown'), text: document.getElementById('alert').innerText.replace(/\\s+/g, ' ').slice(0, 260) })`);
      check(evAfter.events === 0 && evAfter.living.join() === evBefore.living.join() && al.shown && /Refused: Execute/.test(al.text) && /PlayerPlanStateStale/.test(al.text),
        'AC15/AC21: a real Execute sent from a STALE view is refused by the server (PlayerPlanStateStale); nothing is felled, no receipt, and the refusal is shown (fails closed)', { events: evAfter.events, living: evAfter.living, alert: al.text });
      await page.click('#reread'); await settle(500);
      // Execute: real engine semantics
      await page.click('#execute'); await page.waitFor(`document.getElementById('execute-dialog').open`, 'execute dialog', 8000);
      const txt = await ev(`document.getElementById('execute-text').textContent`);
      await page.click('#execute-confirm'); await settle(800);
      await page.waitFor(`document.getElementById('approved-box').hidden`, 'executed', 10000);
      const sE = await rvx(`rv.snap()`);
      const nA = sA.approved.filter((id) => id.startsWith('MA-A')).length, nB = sA.approved.filter((id) => id.startsWith('MA-B')).length;
      check(sE.events === 1 && sE.living[0] === s0.living[0] - nA && sE.living[1] === s0.living[1] - nB && sE.year === 0, 'AC21: Execute fells exactly the approved trees (A -' + nA + ', B -' + nB + '), one receipt, year unchanged', { living: sE.living, events: sE.events, text: txt.slice(0, 120) });
      // advance years; record the smallest real L every year (is a "0.000" display reachable with real data?)
      const minL = async () => ev(`(async () => { const p = await window.__rv.view(); return p.stage4.areas.map((a) => ({ area: a.areaId, min: Math.min(...a.ground.map((c) => c.lightIndex)), below0005: a.ground.filter((c) => c.lightIndex > 0 && c.lightIndex < 0.0005).length, zero: a.ground.filter((c) => c.lightIndex === 0).length, max: Math.max(...a.ground.map((c) => c.lightIndex)) })); })()`);
      const years = [{ year: 0, ...{ m: await minL() } }];
      for (let y = 1; y <= 3; y++) {
        await page.click('#advance'); await sleep(300);
        if (await ev(`document.getElementById('consent-dialog').open`)) await page.click('#consent-advance');
        await settle(800);
        await page.waitFor(`document.getElementById('year-value').textContent === '${y}'`, 'year ' + y, 20000);
        years.push({ year: y, m: await minL() });
      }
      info('smallest REAL engine light index per area by year (does a positive L below 0.0005, shown as "0.000", occur?)', years);
      check((await rvx(`rv.snap()`)).year === 3, 'AC21: Year+1 still advances one ecological year at a time (reached year 3)');
    });

    // ===== PN: narrow widths (not required; reported honestly) ========================================================
    await section('PN', async () => {
      const lists = await livingIds(); const out = [];
      await showArea('MA-B');
      for (const id of lists[1].ids.slice(0, 6)) { await selectById(id); const f = await ev(`window.__rv.fold()`); out.push({ id, ok: foldOk(f), n: f.n, scrollTop: f.scrollTop, rowsReachable: f.rowsReachable, addReachable: f.addReachable, lastBottom: f.lastBottom, addBottom: f.addBottom, ih: f.innerHeight }); }
      const lay = await ev(`(() => { const d = document.documentElement; return { sw: d.scrollWidth, iw: innerWidth }; })()`);
      info('narrow viewport sample (not an acceptance criterion)', { width: W, height: H, overflowX: lay.sw > lay.iw, trees: out });
      await shot('PN-narrow');
    });

    // ===== global: CSP, console, 5xx, overflow at the end =============================================================
    await section('PZ', async () => {
      const csp = await ev('window.__csp');
      check(csp.length === 0, 'AC20: no CSP violation', csp);
      const non409 = page.log.logEntries.filter((l) => !/status of 409 \(Conflict\)/.test(l));
      check(page.log.exceptions.length === 0 && page.log.console.length === 0 && non409.length === 0, 'AC20: no script exception and no console error/warning other than the browser\'s own "409" resource lines', { ex: page.log.exceptions, console: page.log.console, other: non409 });
      info('console lines that are only the browser reporting a 409 the reviewer provoked (stale-fence probes)', { count: page.log.logEntries.length - non409.length, http409: page.log.statuses[409] || 0 });
      check(page.log.http5xx.length === 0, 'AC20: no HTTP 5xx', page.log.http5xx);
      const lay = await ev(`(() => { const d = document.documentElement; return { sw: d.scrollWidth, bw: document.body.scrollWidth, iw: innerWidth }; })()`);
      check(lay.sw <= lay.iw && lay.bw <= lay.iw, 'AC16: no horizontal page overflow at the end of the run', lay);
      info('HTTP statuses seen', page.log.statuses);
    });
  } finally {
    await page.close();
  }
}

main().then(() => {
  fs.writeFileSync(path.join(OUT, 'review-results-' + tag + '.json'), J({ viewport: tag, failures, results, shots }, null, 1));
  const checks = results.filter((r) => !r.startsWith('INFO ')).length;
  console.log('RESULT ' + tag + ': ' + (checks - failures) + '/' + checks + ' PASS');
  process.exit(failures === 0 ? 0 : 1);
}).catch((error) => {
  console.log('ABORT ' + redact(error && error.stack || error));
  fs.writeFileSync(path.join(OUT, 'review-results-' + tag + '.json'), J({ viewport: tag, aborted: redact(String(error)), failures: failures + 1, results, shots }, null, 1));
  process.exit(2);
});
