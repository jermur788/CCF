// Stage 4 owner UX repair, Phase A: real-browser verification in headless Microsoft Edge over CDP.
// R1 rewrote section G (crown wins over ground). R2 adds the R2 section after G: the AC01 fold with Inspect ground ON/OFF,
// all five ticked and scrolled inspectors; three deterministic stale hit-test races (Escape, newer selection, area switch;
// the page's fetch holds the real hit-test answer until the driver releases it); and the ramp table from injected L values.
// Usage (Windows node v26): node scenario-owner-ux.mjs <evidenceDir> <fileHoldingTheBaseUrl> <W>x<H>
// Run once per viewport against a FRESH server session (year 0, empty plan). The base URL file is outside the repository
// and is never printed: every output line goes through redact().
import fs from 'node:fs';
import path from 'node:path';
import { launch, reachable, redact, sleep } from './cdp-ux.mjs';

const [OUT, URL_FILE, SIZE] = process.argv.slice(2);
const [W, H] = (SIZE || '1280x720').split('x').map(Number);
fs.mkdirSync(OUT, { recursive: true });
const base = fs.readFileSync(URL_FILE, 'utf8').trim();
const tag = W + 'x' + H;
const results = [];
let failures = 0;
function info(text) {
  const line = 'INFO [' + tag + '] ' + redact(text);
  results.push(line);
  console.log(line);
}
function check(ok, label, detail) {
  if (!ok) failures++;
  const line = (ok ? 'PASS ' : 'FAIL ') + '[' + tag + '] ' + label + (detail === undefined ? '' : ' :: ' + redact(typeof detail === 'string' ? detail : JSON.stringify(detail)));
  results.push(line);
  console.log(line);
}
const shots = [];
async function shot(page, name) {
  const file = path.join(OUT, name + '-' + tag + '.png');
  const bytes = await page.shot(file);
  shots.push({ name: path.basename(file), bytes });
  console.log('SHOT ' + path.basename(file) + ' ' + bytes + ' bytes');
}

// ---- in-page helpers: they read the engine payload or the DOM and compute nothing ecological -----------------------------
const VIEW = `(async () => (await (await fetch('api/multi/view', { cache: 'no-store' })).json()))()`;
const server = (page) => page.eval(`(async () => { const p = await (await fetch('api/multi/view', { cache: 'no-store' })).json(); const v = p.view;
  return { year: v.source.ecologicalYear, rev: v.planStateRevision, work: v.source.workRevision, pending: v.pending.map((o) => o.areaId + ':' + o.treeId + ':' + o.reason),
    approved: v.approved ? v.approved.orders.map((o) => o.treeId) : null, events: v.events.length, crops: v.areas.map((a) => a.cropTreeIds),
    living: v.areas.map((a) => a.stand.livingCount) }; })()`);
const LAYOUT = `(() => { const d = document.documentElement; const r = (e) => { const b = e.getBoundingClientRect(); return [b.left, b.top, b.right, b.bottom].map(Math.round); };
  const dock = document.getElementById('map-dock').getBoundingClientRect(); const stand = document.getElementById('area-outline').getBoundingClientRect();
  return { scrollWidth: d.scrollWidth, bodyScrollWidth: document.body.scrollWidth, innerWidth, zoom: document.getElementById('zoom-state').textContent,
    overlap: !(stand.right <= dock.left || stand.left >= dock.right || stand.bottom <= dock.top || stand.top >= dock.bottom), dock: r(document.getElementById('map-dock')) }; })()`;
const inspected = (page) => page.eval(`document.getElementById('inspector-body').hidden ? null : document.getElementById('insp-id').textContent`);
const ranksDom = (page) => page.eval(`[...document.querySelectorAll('#rank-layer [data-rank-toggle]')].map((b) => { const r = b.getBoundingClientRect();
  return { id: b.dataset.rankToggle, rank: Number(b.dataset.rank), pressed: b.getAttribute('aria-pressed'), disabled: b.getAttribute('aria-disabled'), label: b.getAttribute('aria-label'),
    w: Math.round(r.width), h: Math.round(r.height), x: r.left + r.width / 2, y: r.top + r.height / 2 }; })`);
const rowsDom = (page) => page.eval(`[...document.querySelectorAll('#competition-content [data-draft]')].map((i) => ({ id: i.dataset.draft, checked: i.checked, disabled: i.disabled }))`);
const draftLabel = (page) => page.eval(`document.getElementById('add-button').textContent`);
const viewBoxOf = (page) => page.eval(`document.getElementById('map').getAttribute('viewBox').split(' ').map(Number)`);

// The screen point of map metres (u, v), by the browser's own SVG transform.
const screenOf = (page, u, v) => page.eval(`(() => { const m = document.getElementById('map').getScreenCTM(); const x = m.a * ${u} + m.c * ${v} + m.e, y = m.b * ${u} + m.d * ${v} + m.f;
  const e = document.elementFromPoint(x, y); return { x, y, onMap: !!(e && e.closest && e.closest('#map')) }; })()`);

// Clicks the centre of a tree's drawn crown (its true scene position) with the real mouse, and returns the tree the page
// then inspects (the server's hit-test decides which crown is topmost there).
async function clickCrown(page, areaId, treeId) {
  const item = await page.eval(`(async () => { const p = await (await fetch('api/multi/view')).json(); return p.scenes[${JSON.stringify(areaId)}].find((i) => i.treeId === ${JSON.stringify(treeId)}); })()`);
  const at = await screenOf(page, item.u, item.v);
  if (!at.onMap) return { clicked: false };
  await page.clickAt(at.x, at.y);
  await sleep(150);
  await page.idle();
  return { clicked: true, at };
}

// A living tree of the area whose crown centre is clickable on the map and which the hit-test selects, with five ranked
// competitors none of which is blocked.
async function selectReference(page, areaId, exclude) {
  const ids = await page.eval(`(async () => { const p = await (await fetch('api/multi/view')).json(); return p.view.areas.find((a) => a.areaId === ${JSON.stringify(areaId)}).trees.filter((t) => t.isLiving).map((t) => t.treeId); })()`);
  for (const id of ids) {
    if (exclude && exclude.includes(id)) continue;
    const r = await clickCrown(page, areaId, id);
    if (!r.clicked) continue;
    try { await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0 || document.querySelector('#competition-content .comp-empty')`, 'competition of ' + id, 8000); } catch { continue; }
    const got = await inspected(page);
    const rows = await rowsDom(page);
    if (got === id && rows.length === 5 && rows.every((row) => !row.disabled)) return id;
  }
  throw new Error('no clickable reference with five free competitors in ' + areaId);
}

async function main() {
  if (!(await reachable(base + 'multi'))) throw new Error('server not reachable from Windows');
  const page = await launch();
  try {
    await page.viewport(W, H);
    await page.goto(base + 'multi');
    await page.waitFor(`document.getElementById('tree-rows').childNodes.length > 0 && document.querySelectorAll('.area-readouts dt').length >= 6`, 'first view');
    await sleep(500);
    const s0 = await server(page);
    check(s0.year === 0 && s0.pending.length === 0 && s0.approved === null && s0.events === 0, 'a fresh session: year 0, no plan', s0);

    // ---- A. defaults, notice, navigation layout -------------------------------------------------------------------------
    const a1 = await page.eval(`({ light: document.getElementById('light-toggle').getAttribute('aria-pressed'), cells: document.getElementById('light-cells').childNodes.length,
      ground: document.getElementById('ground-mode').hidden, legend: document.getElementById('light-legend').hidden })`);
    check(a1.light === 'false' && a1.cells === 0 && a1.ground && a1.legend, 'UX-06: the Light Guide is OFF by default (no cells, no legend, no Inspect ground)', a1);
    const n1 = await page.eval(`(() => { const n = document.querySelector('.notice'); return { line: document.getElementById('notice-line').textContent, h: Math.round(n.getBoundingClientRect().height),
      open: document.getElementById('notice-details').open }; })()`);
    check(n1.line === 'Synthetic Sitka experiment · 2 areas · Site unknown' && n1.h <= 40 && !n1.open, 'UX-04: one unobtrusive notice line, Details closed', n1);
    await page.click('#notice-details > summary');
    const n2 = await page.eval(`(() => ({ open: document.getElementById('notice-details').open, text: document.querySelector('.notice-body').textContent,
      visible: document.querySelector('.notice-body').getBoundingClientRect().height > 0 }))()`);
    const caveats = ['NOT A SURVEYED IRISH FOREST', 'AREAS NOT ASSESSED COMPARABLE', 'SITE UNKNOWN', 'CONNECTED WOODLAND: GROWTH AND CROWNS USE BOTH AREAS',
      'DENSITY MORTALITY, WORK PLANS AND SUMMARIES STAY PER AREA', 'NO TREATMENT TRANSFER BETWEEN AREAS', 'NOT UNITY NUMERICAL OR GEOMETRY PARITY', 'Session only'];
    check(n2.open && n2.visible && caveats.every((c) => n2.text.includes(c)) && !n2.text.includes('EDGE NEIGHBOURS EXCLUDED'),
      'UX-04: Details keeps every caveat verbatim (synthetic, not comparable, site unknown, connected woodland, per-area mortality/plans/summaries, no transfer, no Unity parity)',
      caveats.filter((c) => !n2.text.includes(c)));
    await shot(page, '08-notice-details');
    await page.click('#notice-details > summary');
    const dock = await page.eval(`(() => { const r = (id) => { const b = document.getElementById(id).getBoundingClientRect(); return { l: b.left, t: b.top, r: b.right, b: b.bottom, w: b.width, h: b.height }; };
      return { n: r('pan-north'), w: r('pan-west'), e: r('pan-east'), s: r('pan-south'), out: r('zoom-out'), fit: r('zoom-fit'), in: r('zoom-in'),
        a: r('area-switch').l, tabs: [...document.querySelectorAll('.area-tab')].map((t) => { const b = t.getBoundingClientRect(); return { l: b.left, t: b.top, w: b.width, h: b.height }; }),
        dock: r('map-dock') }; })()`);
    const near = (a, b) => Math.abs(a - b) <= 1.5;
    check(near(dock.w.t, dock.e.t) && dock.n.b <= dock.w.t + 1 && dock.s.t >= dock.w.b - 1 && near((dock.n.l + dock.n.r) / 2, (dock.s.l + dock.s.r) / 2) &&
      dock.w.r <= dock.n.l + 1 && dock.e.l >= dock.n.r - 1, 'UX-05: compass in a 3 x 3 grid (N top-centre, W/E middle row, S bottom-centre)', dock);
    check(near(dock.out.t, dock.fit.t) && near(dock.fit.t, dock.in.t) && dock.out.r <= dock.fit.l && dock.fit.r <= dock.in.l && dock.out.t >= dock.s.b,
      'UX-05: one horizontal − | Fit | + row directly below the compass', { out: dock.out, fit: dock.fit, in: dock.in });
    check(dock.tabs.length === 2 && near(dock.tabs[0].t, dock.tabs[1].t) && dock.tabs[0].t >= dock.out.b, 'UX-05: Area A | Area B on one row below the zoom row', dock.tabs);
    check([dock.n, dock.w, dock.e, dock.s, dock.out, dock.fit, dock.in].every((b) => b.w >= 44 && b.h >= 44) && dock.tabs.every((t) => t.h >= 44) && dock.dock.w <= 170,
      'UX-05: the dock is narrow (≤ 170 px; was 212 px) and every target stays ≥ 44 px', { dockWidth: dock.dock.w });
    const l0 = await page.eval(LAYOUT);
    check(l0.scrollWidth <= l0.innerWidth && l0.bodyScrollWidth <= l0.innerWidth && !l0.overlap && l0.zoom === 'Fit', 'no sideways scroll; the stand is clear of the dock at Fit', l0);
    await shot(page, '01-initial-map');

    // ---- B. navigation buttons ------------------------------------------------------------------------------------------
    const vb0 = await viewBoxOf(page);
    await page.click('#zoom-in'); await page.click('#zoom-in'); await page.click('#zoom-in');
    const vb1 = await viewBoxOf(page);
    const z1 = await page.eval(`document.getElementById('zoom-state').textContent`);
    check(vb1[2] < vb0[2] && z1 !== 'Fit', 'zoom + narrows the view', { z1 });
    const panStep = async (id, axis, sign) => {
      const before = await viewBoxOf(page);
      const enabled = await page.eval(`!document.getElementById('${id}').disabled`);
      if (!enabled) return 'disabled';
      await page.click('#' + id);
      const after = await viewBoxOf(page);
      return Math.sign(after[axis] - before[axis]) === sign ? 'moved' : 'not moved';
    };
    const pans = { east: await panStep('pan-east', 0, 1), south: await panStep('pan-south', 1, 1), west: await panStep('pan-west', 0, -1), north: await panStep('pan-north', 1, -1) };
    check(Object.values(pans).every((p) => p === 'moved'), 'N/W/E/S pan the map in their own direction (north up)', pans);
    await page.click('#zoom-out');
    const z2 = await page.eval(`document.getElementById('zoom-state').textContent`);
    await page.click('#zoom-fit');
    const z3 = await page.eval(`({ z: document.getElementById('zoom-state').textContent, panDisabled: ['pan-north', 'pan-south', 'pan-west', 'pan-east'].every((id) => document.getElementById(id).disabled) })`);
    check(z2 !== z1 && z3.z === 'Fit' && z3.panDisabled, 'zoom − steps back and Fit restores the whole area (pans disabled at Fit, as before)', { z1, z2, z3 });
    await page.click('.area-tab[data-area="MA-B"]');
    await page.waitFor(`document.getElementById('map-subtitle').textContent.startsWith('Area B')`, 'Area B');
    await page.click('.area-tab[data-area="MA-A"]');
    await page.waitFor(`document.getElementById('map-subtitle').textContent.startsWith('Area A')`, 'Area A');
    check(true, 'Area A | Area B switch the map');

    // ---- C. select a reference: five ranks and Add selected above the fold (UX-01) --------------------------------------
    const ref = await selectReference(page, 'MA-A');
    const comp = await page.eval(`(async () => { const p = await (await fetch('api/multi/view')).json(); const v = p.view;
      const r = await (await fetch('api/multi/competition', { method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ fence: { sessionId: v.sessionId, source: v.source, planStateRevision: v.planStateRevision }, areaId: 'MA-A', treeId: ${JSON.stringify(ref)} }) })).json();
      return r.competition.topFive.map((n) => n.treeId); })()`);
    const fold = await page.eval(`(() => { const rows = [...document.querySelectorAll('.comp-row')].map((e) => e.getBoundingClientRect());
      const dock = document.querySelector('.add-dock').getBoundingClientRect(); const add = document.getElementById('add-button').getBoundingClientRect();
      const pane = document.getElementById('pane-tree'); const close = document.getElementById('inspect-close').getBoundingClientRect();
      return { rows: rows.length, lastRowBottom: Math.round(rows[rows.length - 1].bottom), firstRowTop: Math.round(rows[0].top), dockTop: Math.round(dock.top),
        addTop: Math.round(add.top), addBottom: Math.round(add.bottom), innerHeight, scrollTop: pane.scrollTop, paneTop: Math.round(pane.getBoundingClientRect().top),
        closeVisible: close.width > 0 && close.top >= 0 && close.bottom <= innerHeight, closeText: document.getElementById('inspect-close').textContent.trim() }; })()`);
    check(fold.rows === 5 && fold.scrollTop === 0 && fold.firstRowTop >= fold.paneTop && fold.lastRowBottom <= fold.dockTop + 1 && fold.addBottom <= fold.innerHeight,
      'UX-01: after clicking a living tree, ranks 1-5 and an actionable "Add selected to Fell plan" are visible without scrolling', fold);
    check(fold.closeVisible && fold.closeText.includes('Close inspection'), 'UX-03: "× Close inspection" is visible beside the reference ID', fold.closeText);
    let ranks = await ranksDom(page);
    const rows0 = await rowsDom(page);
    check(ranks.length === 5 && ranks.map((r) => r.id).join() === comp.join() && rows0.map((r) => r.id).join() === comp.join() &&
      ranks.every((r, i) => r.rank === i + 1 && r.pressed === 'false' && r.label.startsWith('Competitor ' + (i + 1) + ', tree ' + r.id) && r.w >= 24 && r.h >= 24),
      'UX-02: five map rank buttons, numbered 1-5, each carrying the exact engine ID of the engine\'s rank (same order as the inspector), with an ARIA label', ranks.map((r) => [r.rank, r.id, r.w]));
    await shot(page, '02-selected-five-ranks');

    // ---- D. rank toggles on exact IDs, both directions, multi-select, keyboard, no server write -------------------------
    const sBeforeToggles = await server(page);
    for (let k = 0; k < 5; k++) {
      const r = (await ranksDom(page))[k];
      await page.clickAt(r.x, r.y);
      await sleep(120);
      const after = await ranksDom(page);
      const rows = await rowsDom(page);
      const ok = after[k].id === comp[k] && after[k].pressed === 'true' && rows.find((row) => row.id === comp[k]).checked &&
        (await inspected(page)) === ref && (await draftLabel(page)).includes('Add selected ' + (k + 1));
      check(ok, 'UX-02: map rank ' + (k + 1) + ' toggles exactly ' + comp[k] + ' on; its inspector checkbox follows; the reference stays ' + ref, { label: await draftLabel(page) });
    }
    const fold5 = await page.eval(`(() => { const rows = [...document.querySelectorAll('.comp-row')].map((e) => e.getBoundingClientRect());
      const dock = document.querySelector('.add-dock').getBoundingClientRect(); const add = document.getElementById('add-button');
      return { lastRowBottom: Math.round(rows[rows.length - 1].bottom), dockTop: Math.round(dock.top), addBottom: Math.round(add.getBoundingClientRect().bottom),
        addHeight: Math.round(add.getBoundingClientRect().height), label: add.textContent, scrollTop: document.getElementById('pane-tree').scrollTop }; })()`);
    check(fold5.lastRowBottom <= fold5.dockTop + 1 && fold5.addBottom <= H && fold5.addHeight <= 48 && fold5.scrollTop === 0,
      'UX-01: with all five chosen the rows and the one-line "' + fold5.label + '" button still fit without scrolling', fold5);
    ranks = await ranksDom(page);
    await page.clickAt(ranks[1].x, ranks[1].y);
    await sleep(120);
    let rows = await rowsDom(page);
    ranks = await ranksDom(page);
    check(ranks[1].pressed === 'false' && !rows.find((row) => row.id === comp[1]).checked && ranks.filter((r) => r.pressed === 'true').length === 4,
      'UX-02: clicking rank 2 again clears only ' + comp[1] + ' (multi-select kept)');
    await page.click('#competition-content [data-draft="' + comp[2] + '"]');
    await sleep(120);
    ranks = await ranksDom(page);
    check(ranks[2].pressed === 'false' && ranks.filter((r) => r.pressed === 'true').length === 3, 'UX-02: unticking ' + comp[2] + ' in the inspector clears map rank 3');
    await page.eval(`document.querySelector('#rank-layer [data-rank="1"]').focus()`);
    await page.key('Tab');
    const kb = await page.eval(`(() => { const a = document.activeElement; const s = getComputedStyle(a); return { key: a.getAttribute('data-focus-key'), outline: s.outlineStyle, width: s.outlineWidth }; })()`);
    check(kb.key === 'rank:' + comp[1] && kb.outline === 'solid' && kb.width === '3px', 'UX-02: Tab moves focus to rank 2 with a visible 3 px focus outline', kb);
    await page.key(' ');
    ranks = await ranksDom(page);
    const kb2 = await page.eval(`document.activeElement.getAttribute('data-focus-key')`);
    check(ranks[1].pressed === 'true' && kb2 === 'rank:' + comp[1], 'UX-02: Space on the focused rank toggles it; focus stays on it after the redraw', { kb2 });
    const announced = await page.eval(`document.getElementById('announce').textContent`);
    check(/\d trees? selected for Fell, draft only/.test(announced), 'the selection count is announced (polite live region)', announced);
    const sAfterToggles = await server(page);
    check(JSON.stringify(sAfterToggles) === JSON.stringify(sBeforeToggles) && !page.log.posts.some((p) => !/^api\/multi\/(competition|hit)$/.test(p)),
      'rank toggles are draft only: no Work Plan write, no Fell, the server view is unchanged', { posts: [...new Set(page.log.posts)] });
    const chosen = (await ranksDom(page)).filter((r) => r.pressed === 'true').map((r) => r.id);
    await shot(page, '03-ranks-chosen-synced');
    await page.click('#add-button');
    await page.idle();
    await page.waitFor(`document.getElementById('batch-result').hidden === false`, 'the add result');
    const sAdded = await server(page);
    check(sAdded.pending.map((p) => p.split(':')[1]).sort().join() === chosen.slice().sort().join() && sAdded.pending.every((p) => p.endsWith(':Owner-selected felling')),
      'AC04: Add queued exactly the chosen IDs (' + chosen.join(', ') + '), not all competitors', sAdded.pending);

    // ---- E. Escape precedence and Close inspection ------------------------------------------------------------------------
    await page.click('#crop-button');
    await page.idle();
    await page.waitFor(`document.getElementById('crop-button').textContent === 'Remove Crop designation'`, 'Crop designation');
    await page.click('#zoom-in'); await page.click('#zoom-in');
    const keep = await page.eval(`({ zoom: document.getElementById('zoom-state').textContent, area: document.getElementById('map-subtitle').textContent, vb: document.getElementById('map').getAttribute('viewBox') })`);
    const sKeep = await server(page);
    await page.click('#advance');
    await page.waitFor(`document.getElementById('consent-dialog').open`, 'the Year+1 consent dialog');
    await page.key('Escape');
    const e1 = await page.eval(`({ dialog: document.getElementById('consent-dialog').open, id: document.getElementById('inspector-body').hidden ? null : document.getElementById('insp-id').textContent })`);
    check(!e1.dialog && e1.id === ref, 'UX-03/AC06: Escape with a dialog open closes only the dialog; the inspection stays open', e1);
    await page.key('Escape');
    const e2 = await page.eval(`({ empty: !document.getElementById('inspector-empty').hidden, ranks: document.getElementById('rank-layer').hidden, dimmed: document.querySelectorAll('#crowns .tree.dimmed').length,
      leaders: document.querySelectorAll('#marks .spread-leader').length, zoom: document.getElementById('zoom-state').textContent, area: document.getElementById('map-subtitle').textContent,
      vb: document.getElementById('map').getAttribute('viewBox'), focus: document.activeElement.id })`);
    const sClosed = await server(page);
    check(e2.empty && e2.ranks && e2.dimmed === 0 && e2.leaders === 0, 'UX-03: the next Escape exits inspection; ranks, enlargement and dimming are gone (normal tree rendering)', e2);
    check(e2.zoom === keep.zoom && e2.area === keep.area && e2.vb === keep.vb && JSON.stringify(sClosed) === JSON.stringify(sKeep) && sClosed.crops[0].includes(ref),
      'UX-03: closing keeps zoom, pan, area, the Crop designation, the queued orders and the year', { keep, e2: { zoom: e2.zoom, vb: e2.vb }, crops: sClosed.crops[0] });
    await shot(page, '04-inspection-closed');
    // The × button does the same, and the cleared draft does not come back.
    const ref2 = await selectReference(page, 'MA-A', [ref, ...comp]);
    ranks = await ranksDom(page);
    await page.clickAt(ranks[0].x, ranks[0].y);
    await sleep(120);
    await page.click('#inspect-close');
    const x1 = await page.eval(`({ empty: !document.getElementById('inspector-empty').hidden, zoom: document.getElementById('zoom-state').textContent })`);
    await clickCrown(page, 'MA-A', ref2);
    await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0`, 'the competitors again');
    const x2 = (await rowsDom(page)).filter((r) => r.checked).length;
    check(x1.empty && x1.zoom === keep.zoom && x2 === 0, 'UX-03: "× Close inspection" exits the same way and clears the temporary draft choice', { x1, checkedAfterReselect: x2 });

    // ---- F. Light Guide: continuous coral-peach from each cell's engine value ---------------------------------------------
    await page.click('#zoom-fit');
    await page.click('#light-toggle');
    await sleep(200);
    const lightA = await page.eval(`(async () => { const p = await (await fetch('api/multi/view')).json(); const cells = p.stage4.areas.find((a) => a.areaId === 'MA-A').ground;
      const drawn = [...document.querySelectorAll('#light-cells polygon')].map((e) => ({ L: Number(e.dataset.light), op: Number(e.getAttribute('fill-opacity')), fill: e.getAttribute('fill'), cls: e.getAttribute('class') }));
      const layer = document.getElementById('light-cells'); const crowns = document.getElementById('crowns');
      return { positive: cells.filter((c) => c.lightIndex > 0).length, zero: cells.filter((c) => c.lightIndex === 0).length, drawn, texts: layer.querySelectorAll('text').length,
        under: !!(layer.compareDocumentPosition(crowns) & Node.DOCUMENT_POSITION_FOLLOWING), pe: getComputedStyle(layer.querySelector('polygon') || layer).pointerEvents,
        legend: document.getElementById('light-legend').textContent.replace(/\\s+/g, ' ').trim(), ground: !document.getElementById('ground-mode').hidden }; })()`);
    check(lightA.drawn.length === lightA.positive && lightA.drawn.every((c) => c.L > 0 && c.op > 0), 'UX-06: every cell with L > 0 is drawn with a strictly positive opacity; L = 0 cells are not drawn',
      { drawn: lightA.drawn.length, positive: lightA.positive, zero: lightA.zero, minL: Math.min(...lightA.drawn.map((c) => c.L)), minOpacity: Math.min(...lightA.drawn.map((c) => c.op)) });
    check(lightA.texts === 0 && lightA.drawn.every((c) => c.cls === 'light-cell') && lightA.under && lightA.pe === 'none',
      'UX-06/F-3: no numbered band markers or band classes; the layer lies under the crowns and takes no clicks', { texts: lightA.texts, under: lightA.under, pe: lightA.pe });
    const sorted = lightA.drawn.slice().sort((a, b) => a.L - b.L);
    const lum = (hex) => [1, 3, 5].reduce((s, i) => s + parseInt(hex.slice(i, i + 2), 16), 0);
    const mono = sorted.every((c, i) => i === 0 || (c.op >= sorted[i - 1].op && lum(c.fill) >= lum(sorted[i - 1].fill)));
    check(mono, 'UX-06: across the real cells opacity and colour brightness never decrease as L increases');
    check(lightA.legend.startsWith('Light Guide') && lightA.legend.includes('Shaded') && lightA.legend.includes('Bright') && !/band|not measured|PAR|k10a10/i.test(lightA.legend) && lightA.ground,
      'UX-09: the legend says "Light Guide" with a Shaded→Bright ramp; no band labels or calibration text on the map', lightA.legend);
    // The ramp exposes the same mapping at chosen L values; draw those exact paints over the floor colour and read pixels.
    const stops = await page.eval(`[...document.querySelectorAll('#light-ramp stop')].map((s) => ({ L: Number(s.dataset.l), colour: s.getAttribute('stop-color'), op: Number(s.getAttribute('stop-opacity')) }))`);
    const at = (L) => stops.find((s) => Math.abs(s.L - L) < 1e-9);
    const p0 = at(0), p01 = at(0.01), p05 = at(0.05), p5 = at(0.5), p1 = at(1);
    check(p0.op === 0 && p01.op > 0 && p01.op < 0.06 && p05.op > p01.op && p5.op > p05.op && p1.op > p5.op && p1.colour === '#ffd9bd' && p1.op >= 0.85,
      'UX-06: the mapping at L = 0 / 0.01 / 0.05 / 0.5 / 1 is invisible / extremely faint but nonzero / rising / rising / brightest pale peach', [p0, p01, p05, p5, p1]);
    await page.eval(`(() => { const host = document.createElement('div'); host.id = 'test-swatches'; host.style.position = 'fixed'; host.style.left = '0'; host.style.top = '0'; host.style.zIndex = '99';
      const NS = 'http://www.w3.org/2000/svg'; const s = document.createElementNS(NS, 'svg'); s.setAttribute('width', '300'); s.setAttribute('height', '40');
      ${JSON.stringify([p0, p01, p05, p5, p1])}.forEach((p, i) => { const bg = document.createElementNS(NS, 'rect'); bg.setAttribute('x', i * 60); bg.setAttribute('width', 60); bg.setAttribute('height', 40); bg.setAttribute('fill', 'rgb(27,36,48)');
        const fg = document.createElementNS(NS, 'rect'); fg.setAttribute('x', i * 60 + 10); fg.setAttribute('y', 10); fg.setAttribute('width', 40); fg.setAttribute('height', 20); fg.setAttribute('fill', p.colour); fg.setAttribute('fill-opacity', p.op); s.append(bg, fg); });
      host.append(s); document.body.append(host); })()`);
    await sleep(150);
    const img = await page.capture({ x: 0, y: 0, width: 300, height: 40 });
    await page.eval(`document.getElementById('test-swatches').remove()`);
    const px = [0, 1, 2, 3, 4].map((i) => ({ bg: img.px(i * 60 + 4, 4), fg: img.px(i * 60 + 30, 20) }));
    const diff = px.map((p) => p.fg.reduce((s, v, i) => s + Math.abs(v - p.bg[i]), 0));
    check(diff[0] === 0 && diff[1] > 0 && diff[1] <= 24 && diff[2] > diff[1] && diff[3] > diff[2] && diff[4] > diff[3],
      'UX-06 pixels: L = 0 leaves the floor unchanged; L = 0.01 changes it faintly but visibly on inspection; brightness rises through 0.05, 0.5 and 1',
      { diffs: diff, rgb: px.map((p) => p.fg) });
    await shot(page, '05-light-guide-area-a');
    await page.click('.area-tab[data-area="MA-B"]');
    await sleep(300);
    const lightB = await page.eval(`({ drawn: document.querySelectorAll('#light-cells polygon').length, maxOp: Math.max(...[...document.querySelectorAll('#light-cells polygon')].map((e) => Number(e.getAttribute('fill-opacity')))) })`);
    check(lightB.drawn > 0 && lightB.maxOp > 0.6, 'Area B shows its brighter real openings (no light invented in Area A)', lightB);
    await shot(page, '06-light-guide-area-b');

    // ---- G. With the Light Guide on: crowns still select; Inspect ground reads only open ground (UX-07/UX-08) ------------
    // Owner decision 2026-11-23 (literal spec): modal dialog > rank button > crown selection > ground read (ground mode only).
    const refB = await selectReference(page, 'MA-B');
    check((await inspected(page)) === refB && (await page.eval(`document.getElementById('ground-panel').hidden`)),
      'UX-08: with the Light Guide on, clicking a crown selects that tree (no ground panel)', refB);
    ranks = await ranksDom(page);
    await page.clickAt(ranks[0].x, ranks[0].y);
    await sleep(120);
    check((await ranksDom(page))[0].pressed === 'true', 'UX-08: rank toggles work with the Light Guide on');
    await page.click('#ground-mode');
    const gm = await page.eval(`({ pressed: document.getElementById('ground-mode').getAttribute('aria-pressed'), text: document.getElementById('ground-mode-text').textContent,
      panel: !document.getElementById('ground-panel').hidden, cls: document.getElementById('map-pane').classList.contains('ground-mode'), status: document.getElementById('map-status').textContent,
      mapCursor: getComputedStyle(document.getElementById('map')).cursor, crownCursor: getComputedStyle(document.querySelector('#crowns .tree .crown')).cursor,
      hint: document.getElementById('ground-summary').textContent })`);
    check(gm.pressed === 'true' && gm.text === 'Inspect ground: On' && gm.panel && gm.cls && gm.status.includes('Inspect ground is on'),
      'UX-08: Inspect ground has an obvious On state (pressed button "Inspect ground: On", panel, status line)', gm);
    check(gm.status.includes('crowns still select trees') && gm.status.includes('open ground') && !/paused/.test(gm.status + gm.hint) &&
      gm.hint.includes('Clicking a crown still selects that tree') && gm.mapCursor === 'crosshair' && gm.crownCursor === 'pointer',
      'UX-08: the hint and cursor are truthful: crosshair over open ground, pointer over crowns, text says crowns still select', gm);
    // How much of each area's cell grid is open ground for the server's hit-test (probed at every cell centre, read-only).
    const probeOpen = (areaId) => page.eval(`(async () => { const p = await (await fetch('api/multi/view', { cache: 'no-store' })).json(); const v = p.view;
      const fence = { sessionId: v.sessionId, source: v.source, planStateRevision: v.planStateRevision };
      const inside = (pts, u, v) => { let ins = false; for (let i = 0, j = pts.length - 1; i < pts.length; j = i++) { const a = pts[i], b = pts[j]; if ((a.v > v) !== (b.v > v) && u < (b.u - a.u) * (v - a.v) / (b.v - a.v) + a.u) ins = !ins; } return ins; };
      const cells = p.stage4.areas.find((a) => a.areaId === ${JSON.stringify(areaId)}).ground; const open = [];
      for (const c of cells) { const u = c.polygon.reduce((s, q) => s + q.u, 0) / c.polygon.length, w = c.polygon.reduce((s, q) => s + q.v, 0) / c.polygon.length;
        if (!inside(c.polygon, u, w)) continue;
        const r = await (await fetch('api/multi/hit', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ fence, areaId: ${JSON.stringify(areaId)}, u, v: w }) })).json();
        if (r.candidates && r.candidates.length === 0) open.push({ u, v: w, cell: c }); }
      return { cells: cells.length, open }; })()`);
    const openA = await probeOpen('MA-A');
    info('Area A open ground for the hit-test: ' + openA.open.length + ' of ' + openA.cells + ' cell centres (' + (100 * openA.open.length / openA.cells).toFixed(1) + '%) have no crown');
    const openB = await probeOpen('MA-B');
    info('Area B open ground for the hit-test: ' + openB.open.length + ' of ' + openB.cells + ' cell centres (' + (100 * openB.open.length / openB.cells).toFixed(1) + '%) have no crown');
    // An open-ground point that is on screen, on the map and not under a drawn crown or a rank button. A real click lands
    // on a whole pixel, so the test prefers the hit-test-empty cell centre farthest from any crown edge (drawn radii are
    // used only to choose a test point; the server's hit-test above decided it is open).
    const sceneB = await page.eval(`(async () => (await (await fetch('api/multi/view')).json()).scenes['MA-B'].filter((t) => t.status === 'Living').map((t) => ({ u: t.u, v: t.v, r: t.radiusM })))()`);
    const clearance = (o) => Math.min(...sceneB.map((t) => Math.hypot(t.u - o.u, t.v - o.v) - t.r));
    const candidatesOpen = openB.open.map((o) => ({ ...o, clear: clearance(o) })).sort((a, b) => b.clear - a.clear);
    let open = null, openAt = null;
    for (const o of candidatesOpen) {
      const at = await screenOf(page, o.u, o.v);
      const ok = at.onMap && at.x > 0 && at.y > 0 && at.x < W && at.y < H && await page.eval(`(() => { const e = document.elementFromPoint(${at.x}, ${at.y}); return !!e && !e.closest('#crowns') && !e.closest('[data-rank-toggle]'); })()`);
      if (ok) { open = o; openAt = at; break; }
    }
    check(open !== null, 'an on-screen open-ground point exists in Area B (hit-test reports no crown there)', open && { u: open.u, v: open.v, clearanceM: Number(open.clear.toFixed(2)) });
    // Inspect ground ON + open ground: reads the ground cell; selection, draft and the server are unchanged.
    const snapshotUi = async () => ({ insp: await inspected(page), label: await draftLabel(page), rows: JSON.stringify(await rowsDom(page)),
      ranks: JSON.stringify((await ranksDom(page)).map((r) => [r.id, r.pressed])), server: await server(page) });
    const before = await snapshotUi();
    await page.clickAt(openAt.x, openAt.y);
    await sleep(150);
    await page.idle();
    const g1 = await page.eval(`({ heading: document.getElementById('ground-heading').textContent, summary: document.getElementById('ground-summary').textContent,
      treesOpen: document.getElementById('ground-trees').open, techOpen: document.getElementById('ground-tech').open, treesSummary: document.getElementById('ground-trees-summary').textContent,
      ids: document.getElementById('ground-tree-ids').textContent, tech: document.getElementById('ground-facts').textContent, insp: document.getElementById('insp-id').textContent,
      pick: document.querySelectorAll('#ground-mark polygon').length })`);
    const c = open.cell;
    const expectIndex = c.lightIndex.toFixed(3), expectCover = (c.geometricCoveredFraction * 100).toFixed(0) + '%';
    const afterOpen = await snapshotUi();
    check(g1.heading.includes('ground at X ' + c.centreXM.toFixed(1)) && g1.pick === 2,
      'UX-08: Inspect ground ON, a click on open ground (no crown) shows the ground inspector for that cell', { heading: g1.heading });
    check(afterOpen.insp === refB && afterOpen.label === before.label && afterOpen.rows === before.rows && afterOpen.ranks === before.ranks &&
      JSON.stringify(afterOpen.server) === JSON.stringify(before.server), 'UX-08: the open-ground read changes no selection, no draft and nothing on the server', { insp: afterOpen.insp, label: afterOpen.label });
    check(/Very shaded|Low|Moderate|Bright/.test(g1.summary) && g1.summary.includes('Light index' + expectIndex) && g1.summary.includes('Canopy cover' + expectCover) &&
      g1.summary.includes('Shading trees' + (c.contributingTreeIds.length || 'none')) && !g1.summary.includes(c.contributingTreeIds[0] || '§'),
      'UX-07: concise summary: level word, the engine light index, the separate canopy cover and the shading-tree count (no IDs on top)', g1.summary);
    check(!g1.treesOpen && !g1.techOpen && g1.treesSummary === 'Contributing trees (' + c.contributingTreeIds.length + ')' &&
      c.contributingTreeIds.every((id) => g1.ids.includes(id)) && g1.tech.includes('column ' + c.column + ', row ' + c.row) && g1.tech.includes('k10a10') && g1.tech.includes('Calibrated PARno'),
      'UX-07: contributing IDs, cell, grid, model provenance and calibration are kept behind closed disclosures', { treesSummary: g1.treesSummary });
    const gv = await page.eval(`(() => { const p = document.getElementById('pane-tree').getBoundingClientRect(); const g = document.getElementById('ground-panel'); const r = g.getBoundingClientRect();
      const d = document.querySelector('.add-dock').getBoundingClientRect();
      return { slot: g.parentNode.id, top: Math.round(r.top), bottom: Math.round(r.bottom), dockBottom: Math.round(d.bottom), paneTop: Math.round(p.top), paneBottom: Math.round(p.bottom),
        scrollTop: document.getElementById('pane-tree').scrollTop, focus: document.activeElement.id }; })()`);
    check(gv.slot === 'ground-slot' && gv.top >= gv.dockBottom - 1 && gv.top >= gv.paneTop - 1 && gv.top < gv.paneBottom && gv.focus === 'ground-heading',
      'R2: with a tree inspected, the open-ground read sits below the primary block (after the Add bar) and is scrolled into view with its heading focused', gv);
    await page.click('#ground-trees > summary');
    await shot(page, '07-ground-inspection');
    // A rank button still toggles only its own tree ID in ground mode (rank > crown > ground) and leaves the ground reading alone.
    ranks = await ranksDom(page);
    const r1 = ranks.find((r) => r.disabled !== 'true' && r.pressed === 'false');
    if (r1) {
      const pressedBefore = ranks.map((r) => [r.id, r.pressed]);
      await page.clickAt(r1.x, r1.y);
      await sleep(150);
      const g2 = await page.eval(`document.getElementById('ground-heading').textContent`);
      const pressedAfter = (await ranksDom(page)).map((r) => [r.id, r.pressed]);
      const changed = pressedAfter.filter((p, i) => p[1] !== pressedBefore[i][1]).map((p) => p[0]);
      check(changed.length === 1 && changed[0] === r1.id && (await inspected(page)) === refB && g2 === g1.heading,
        'UX-08: with Inspect ground ON a rank click toggles only its own tree ID (' + r1.id + '), keeps the reference and does not read the ground', { changed });
    } else check(false, 'UX-08: a free rank button exists for the ground-mode rank check');
    // Outside the area there is no crown and no sample, and nothing is estimated.
    const outside = await page.eval(`(async () => { const p = await (await fetch('api/multi/view')).json(); const a = p.view.areas.find((x) => x.areaId === 'MA-B');
      const side = a.extent.sideM; const low = a.polygon.map((q) => ({ u: q.xm - a.extent.minXM, v: a.extent.minZM + side - q.zm })).sort((x, y) => y.v - x.v)[0]; return { u: low.u, v: low.v + 1.2 }; })()`);
    const outAt = await screenOf(page, outside.u, outside.v);
    await page.clickAt(outAt.x, outAt.y);
    await sleep(150);
    await page.idle();
    const g3 = await page.eval(`({ heading: document.getElementById('ground-heading').textContent, summary: document.getElementById('ground-summary').textContent, tech: document.getElementById('ground-tech').hidden })`);
    check(outAt.onMap && g3.heading.endsWith('no sample') && g3.summary.includes('No modelled ground sample') && g3.tech, 'UX-08: a spot with no ground sample says so; no value is synthesised', g3);
    // An open modal dialog swallows a map click, also in ground mode.
    await page.clickAt(openAt.x, openAt.y);
    await sleep(150);
    await page.idle();
    const preModal = { heading: await page.eval(`document.getElementById('ground-heading').textContent`), insp: await inspected(page), server: await server(page) };
    await page.click('#advance');
    await page.waitFor(`document.getElementById('consent-dialog').open`, 'the Year+1 consent dialog (ground mode)');
    const postsBefore = page.log.posts.length;
    await page.clickAt(outAt.x, outAt.y);
    await sleep(250);
    const m1 = { open: await page.eval(`document.getElementById('consent-dialog').open`), heading: await page.eval(`document.getElementById('ground-heading').textContent`),
      insp: await inspected(page), posts: page.log.posts.slice(postsBefore) };
    check(m1.open && m1.heading === preModal.heading && m1.insp === preModal.insp && m1.posts.length === 0,
      'UX-08: with a modal dialog open (ground mode ON) a map click is swallowed: no hit-test, no ground read, no selection change', m1);
    await page.key('Escape');
    const m2 = await page.eval(`({ open: document.getElementById('consent-dialog').open, ground: document.getElementById('ground-mode').getAttribute('aria-pressed') })`);
    check(!m2.open && m2.ground === 'true' && JSON.stringify(await server(page)) === JSON.stringify(preModal.server), 'Escape closes only the dialog; Inspect ground stays on', m2);
    // Inspect ground ON + a crown: the crown wins. The tree is selected exactly as a normal selection; no ground read is shown.
    // A crown point that is visible, on the drawn crowns (not under a rank button) and whose server hit-test topmost is a
    // tree outside `exclude`, so clicking it must change the selection if crowns win.
    const crownTarget = (exclude) => page.eval(`(async () => { const p = await (await fetch('api/multi/view', { cache: 'no-store' })).json(); const v = p.view;
      const fence = { sessionId: v.sessionId, source: v.source, planStateRevision: v.planStateRevision }; const m = document.getElementById('map').getScreenCTM();
      const skip = new Set(${JSON.stringify(exclude)});
      for (const t of p.scenes['MA-B']) {
        if (skip.has(t.treeId) || t.status !== 'Living') continue;
        const x = m.a * t.u + m.c * t.v + m.e, y = m.b * t.u + m.d * t.v + m.f;
        if (x < 0 || y < 0 || x > innerWidth || y > innerHeight) continue;
        const e = document.elementFromPoint(x, y);
        if (!(e && e.closest && e.closest('#crowns') && !e.closest('[data-rank-toggle]'))) continue;
        const r = await (await fetch('api/multi/hit', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ fence, areaId: 'MA-B', u: t.u, v: t.v }) })).json();
        const hit = (r.candidates || []).map((c) => c.treeId);
        if (hit.length > 0 && !skip.has(hit[0])) return { tree: t.treeId, u: t.u, v: t.v, hit };
      }
      return null; })()`);
    const other = await crownTarget([refB]);
    const otherAt = await screenOf(page, other.u, other.v);
    const hitAtOther = other.hit;
    const sCrown = await server(page);
    await page.clickAt(otherAt.x, otherAt.y);
    await sleep(200);
    await page.idle();
    await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0 || document.querySelector('#competition-content .comp-empty')`, 'competition (ground mode crown)', 8000);
    const g7 = await page.eval(`({ insp: document.getElementById('inspector-body').hidden ? null : document.getElementById('insp-id').textContent, heading: document.getElementById('ground-heading').textContent,
      pick: document.querySelectorAll('#ground-mark polygon').length, ground: document.getElementById('ground-mode').getAttribute('aria-pressed'), techHidden: document.getElementById('ground-tech').hidden,
      status: document.getElementById('map-status').textContent })`);
    check(hitAtOther.length > 0 && g7.insp === hitAtOther[0] && g7.insp !== refB && g7.heading.endsWith('choose a spot') && g7.pick === 0 && g7.techHidden && g7.ground === 'true',
      'UX-08: Inspect ground ON, clicking a crown SELECTS that tree (the hit-test\'s topmost, ' + (hitAtOther[0] || '?') + ') and shows no ground read; the mode stays on', g7);
    check(JSON.stringify(await server(page)) === JSON.stringify(sCrown), 'UX-08: the ground-mode crown selection sent nothing that changes the server view');
    const selectedInGround = g7.insp;
    // Escape stops Inspect ground first (Light Guide stays on), then closes the inspection.
    await page.key('Escape');
    const g4 = await page.eval(`({ ground: document.getElementById('ground-mode').getAttribute('aria-pressed'), panel: document.getElementById('ground-panel').hidden,
      light: document.getElementById('light-toggle').getAttribute('aria-pressed'), cells: document.querySelectorAll('#light-cells polygon').length, insp: document.getElementById('insp-id').textContent,
      body: document.getElementById('inspector-body').hidden })`);
    check(g4.ground === 'false' && g4.panel && g4.light === 'true' && g4.cells > 0 && !g4.body && g4.insp === selectedInGround,
      'UX-03/UX-08 precedence: Escape first stops Inspect ground; the Light Guide stays on and the inspection stays open', g4);
    const backTarget = await crownTarget([selectedInGround]);
    const backAt = await screenOf(page, backTarget.u, backTarget.v);
    await page.clickAt(backAt.x, backAt.y);
    await sleep(200);
    await page.idle();
    const g5 = await inspected(page);
    check(g5 === backTarget.hit[0] && g5 !== selectedInGround, 'UX-08: after leaving ground mode a crown click selects that tree as before (' + backTarget.hit[0] + ')', { now: g5 });
    await page.key('Escape');
    const g6 = await page.eval(`({ body: document.getElementById('inspector-body').hidden, light: document.getElementById('light-toggle').getAttribute('aria-pressed') })`);
    check(g6.body && g6.light === 'true', 'Escape then closes the inspection; the Light Guide is never switched off by Escape', g6);
    const sLight = await server(page);
    check(JSON.stringify(sLight) === JSON.stringify(sClosed), 'AC21: Light Guide, Inspect ground and inspection changes had no server side effect', sLight);

    // ---- R2. Repair 2 regressions: AC01 fold with Inspect ground ON/OFF and scrolled inspectors; stale hit-test answers;
    // the continuous light ramp. Area B, Light Guide on, nothing inspected at this point.
    const foldNow = () => page.eval(`(() => { const rows = [...document.querySelectorAll('.comp-row')].map((e) => e.getBoundingClientRect());
      const dock = document.querySelector('.add-dock').getBoundingClientRect(); const add = document.getElementById('add-button').getBoundingClientRect();
      const pane = document.getElementById('pane-tree'); const pr = pane.getBoundingClientRect(); const ref = document.querySelector('.ref-card').getBoundingClientRect();
      const g = document.getElementById('ground-panel'); const gr = g.getBoundingClientRect(); const label = document.querySelector('.comp-row label');
      return { rows: rows.length, firstRowTop: rows.length ? Math.round(rows[0].top) : null, lastRowBottom: rows.length ? Math.round(rows[rows.length - 1].bottom) : null,
        rowH: rows.length ? Math.round(rows[0].height) : null, rowFontPx: label ? parseFloat(getComputedStyle(label).fontSize) : null,
        refTop: Math.round(ref.top), paneTop: Math.round(pr.top), paneBottom: Math.round(pr.bottom), dockTop: Math.round(dock.top), dockBottom: Math.round(dock.bottom),
        addTop: Math.round(add.top), addBottom: Math.round(add.bottom), addH: Math.round(add.height), addText: document.getElementById('add-button').textContent, innerHeight,
        scrollTop: pane.scrollTop, groundHidden: g.hidden, groundParent: g.parentNode.id, groundTop: Math.round(gr.top),
        groundPressed: document.getElementById('ground-mode').getAttribute('aria-pressed'), light: document.getElementById('light-toggle').getAttribute('aria-pressed') }; })()`);
    const foldOk = (f) => f.rows === 5 && f.scrollTop === 0 && f.refTop >= f.paneTop && f.firstRowTop >= f.paneTop && f.lastRowBottom <= f.dockTop + 1 &&
      f.addBottom <= f.innerHeight && f.addBottom <= f.paneBottom + 1 && f.rowH >= 36 && f.rowFontPx >= 12 && f.addH >= 40;
    const scrollToBottom = () => page.eval(`(() => { const p = document.getElementById('pane-tree'); p.scrollTop = p.scrollHeight; return p.scrollTop; })()`);
    await page.click('#ground-mode');
    const r2a = await page.eval(`({ parent: document.getElementById('ground-panel').parentNode.id, first: document.getElementById('pane-tree').firstElementChild.id, hidden: document.getElementById('ground-panel').hidden,
      pressed: document.getElementById('ground-mode').getAttribute('aria-pressed'), focus: document.activeElement.id })`);
    check(r2a.parent === 'pane-tree' && r2a.first === 'ground-panel' && !r2a.hidden && r2a.pressed === 'true' && r2a.focus === 'ground-heading',
      'R2: Inspect ground ON with nothing inspected shows its panel at the top of the Tree pane (heading focused)', r2a);
    const usedB = [refB, selectedInGround, backTarget.hit[0]];
    const refR2 = await selectReference(page, 'MA-B', usedB);
    let f = await foldNow();
    check(foldOk(f) && f.groundPressed === 'true' && f.light === 'true' && f.groundParent === 'ground-slot' && !f.groundHidden && f.groundTop >= f.dockBottom - 1,
      'R2/AC01: Inspect ground ON + crown click (' + refR2 + '): reference, ranks 1-5 and Add are above the fold with no scrolling; the ground panel sits below them; both modes stay on', f);
    await shot(page, '10-ground-mode-crown-selects');
    for (let k = 0; k < 5; k++) { const r = (await ranksDom(page))[k]; await page.clickAt(r.x, r.y); await sleep(120); }
    f = await foldNow();
    check(foldOk(f) && f.addText.includes('Add selected 5') && f.groundPressed === 'true',
      'R2/AC01: Inspect ground ON with all five ranks ticked: the rows and "' + f.addText + '" still fit without scrolling', f);
    const scrolled1 = await scrollToBottom();
    const refR2b = await selectReference(page, 'MA-B', [...usedB, refR2]);
    f = await foldNow();
    check(scrolled1 > 0 && foldOk(f) && f.groundPressed === 'true' && f.groundParent === 'ground-slot',
      'R2: Inspect ground ON, inspector scrolled down (scrollTop ' + scrolled1 + ') then another crown (' + refR2b + '): the primary block is back in view with no manual scrolling', f);
    // A ground read with a tree inspected scrolls the reading into view; the next crown click puts the inspector back at the top.
    let openR2 = null;
    for (const o of candidatesOpen) {
      const at = await screenOf(page, o.u, o.v);
      if (at.onMap && at.x > 0 && at.y > 0 && at.x < W && at.y < H && await page.eval(`(() => { const e = document.elementFromPoint(${at.x}, ${at.y}); return !!e && !e.closest('#crowns') && !e.closest('[data-rank-toggle]') && !!e.closest('#map'); })()`)) { openR2 = at; break; }
    }
    if (openR2) {
      await page.clickAt(openR2.x, openR2.y);
      await sleep(150);
      await page.idle();
      const gr = await page.eval(`({ heading: document.getElementById('ground-heading').textContent, scrollTop: document.getElementById('pane-tree').scrollTop, insp: document.getElementById('insp-id').textContent })`);
      const refR2c = await selectReference(page, 'MA-B', [...usedB, refR2, refR2b]);
      f = await foldNow();
      check(gr.heading.includes('ground at') && gr.insp === refR2b && foldOk(f) && f.groundPressed === 'true',
        'R2: after an open-ground read scrolled the inspector (scrollTop ' + gr.scrollTop + '), a crown click (' + refR2c + ') shows the primary block again without scrolling', { gr, f });
    } else check(false, 'R2: an on-screen open-ground point exists with a tree inspected');
    await page.click('#ground-mode');
    const scrolled2 = await scrollToBottom();
    const refR2d = await selectReference(page, 'MA-B', [...usedB, refR2, refR2b]);
    f = await foldNow();
    check(scrolled2 > 0 && foldOk(f) && f.groundPressed === 'false' && f.groundHidden && f.light === 'true',
      'R2: Inspect ground OFF, inspector scrolled down (scrollTop ' + scrolled2 + ') then another crown (' + refR2d + '): the primary block is back in view with no manual scrolling', f);

    // Stale hit-test answers. Deterministic race: a fetch wrapper lets the real api/multi/hit request reach the server and
    // answer, then holds that answer inside the page until the driver releases it, after the interfering action.
    await page.eval(`(() => { const real = window.fetch.bind(window); window.__holdHit = false; window.__hitHeld = 0;
      window.fetch = async (input, init) => { const r = await real(input, init);
        if (window.__holdHit && String(input).endsWith('api/multi/hit')) { window.__hitHeld += 1; await new Promise((resolve) => { window.__releaseHit = resolve; }); }
        return r; }; })()`);
    const raceState = () => page.eval(`({ body: document.getElementById('inspector-body').hidden, empty: !document.getElementById('inspector-empty').hidden,
      insp: document.getElementById('inspector-body').hidden ? null : document.getElementById('insp-id').textContent, announce: document.getElementById('announce').textContent,
      alert: document.getElementById('alert').textContent, area: document.getElementById('map-subtitle').textContent, add: document.getElementById('add-button').textContent,
      ranks: document.getElementById('rank-layer').hidden })`);
    const holdAndClick = async (exclude, n) => {
      const t = await crownTarget(exclude);
      const at = await screenOf(page, t.u, t.v);
      await page.eval(`window.__holdHit = true`);
      await page.clickAt(at.x, at.y);
      await page.waitFor(`window.__hitHeld === ${n}`, 'the held hit-test answer ' + n);
      return t;
    };
    const release = async () => { await page.eval(`window.__holdHit = false; window.__releaseHit()`); await sleep(700); await page.idle(); };
    // (a) Escape while the answer is held.
    const sRace = await server(page);
    let postMark = page.log.posts.length;
    const tA = await holdAndClick([refR2d], 1);
    await page.key('Escape');
    const ra0 = await raceState();
    await sleep(150);
    const ra1 = await raceState();
    await release();
    const ra2 = await raceState();
    const compPostsA = page.log.posts.slice(postMark).filter((p) => p === 'api/multi/competition').length;
    check(ra0.body && ra0.empty, 'R2/UX-03: Escape closes the inspection immediately while the hit-test answer is still pending', ra0);
    check(ra2.body && ra2.empty && ra2.ranks && ra2.announce === ra1.announce && !ra2.announce.startsWith('Selected') && ra2.alert === '' && compPostsA === 0,
      'R2/UX-03: the late hit-test answer (topmost ' + tA.hit[0] + ') is discarded: inspection stays closed, no reselection, no announcement, no competitor read', { ra1: ra1.announce, ra2, compPostsA });
    // (b) A newer selection (exact ID + Enter) while the answer is held.
    await selectReference(page, 'MA-B', [...usedB, refR2d]);
    const tB = await holdAndClick([refR2, await inspected(page)], 2);
    await page.eval(`(() => { const q = document.getElementById('quick-id'); q.value = ${JSON.stringify(refR2)}; q.focus(); })()`);
    await page.key('Enter');
    const rb0 = await raceState();
    await release();
    await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0`, 'the newer selection\'s competitors after the stale answer', 8000);
    const rb1 = await raceState();
    check(rb0.insp === refR2 && rb1.insp === refR2 && !rb1.announce.includes(tB.hit[0]) && rb1.alert === '' && (await rowsDom(page)).length === 5,
      'R2: a newer selection (' + refR2 + ') made while the hit-test answer was pending is kept; the late answer (topmost ' + tB.hit[0] + ') changes nothing and the competitors load', { rb0: rb0.insp, rb1 });
    // (c) An area switch while the answer is held; the newer draft in Area B survives.
    ranks = await ranksDom(page);
    await page.clickAt(ranks[0].x, ranks[0].y);
    await sleep(150);
    const draftB = await draftLabel(page);
    const tC = await holdAndClick([refR2], 3);
    await page.click('.area-tab[data-area="MA-A"]');
    await release();
    const rc0 = await raceState();
    await page.click('.area-tab[data-area="MA-B"]');
    await page.waitFor(`document.getElementById('map-subtitle').textContent.startsWith('Area B')`, 'Area B again');
    await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0`, 'the Area B reference competitors', 8000);
    const rc1 = await raceState();
    check(draftB.includes('Add selected 1') && rc0.area.startsWith('Area A') && !rc0.announce.includes(tC.hit[0]) && rc1.insp === refR2 && rc1.add === draftB,
      'R2: an area switch while the hit-test answer was pending: the page stays on Area A; back in B the reference ' + refR2 + ' and its draft ("' + draftB + '") are intact (late topmost ' + tC.hit[0] + ' ignored)', { rc0: [rc0.area, rc0.announce], rc1: [rc1.insp, rc1.add] });
    check(JSON.stringify(await server(page)) === JSON.stringify(sRace), 'R2: the three races changed nothing on the server');

    // The continuous ramp: the page's own lightPaint for chosen L values, by injecting them into one read-out (display
    // only, like the F-2 injection below) and reading the drawn cells; then a normal re-read restores the engine values.
    await page.click('.area-tab[data-area="MA-A"]');
    await page.waitFor(`document.getElementById('map-subtitle').textContent.startsWith('Area A')`, 'Area A for the ramp');
    const RAMP_L = [0, 1e-200, 1e-9, 0.0001, 0.01, 0.05, 0.5, 1];
    await page.eval(`(() => { window.__rampNext = false; const real = window.fetch.bind(window);
      window.fetch = async (input, init) => { const r = await real(input, init); if (window.__rampNext && String(input).endsWith('api/multi/view')) { window.__rampNext = false;
        const j = await r.json(); const cells = j.stage4.areas.find((a) => a.areaId === 'MA-A').ground; ${JSON.stringify(RAMP_L)}.forEach((L, i) => { cells[i].lightIndex = L; });
        return new Response(JSON.stringify(j), { status: r.status, headers: { 'Content-Type': 'application/json' } }); } return r; }; })()`);
    const positiveBefore = await page.eval(`document.querySelectorAll('#light-cells polygon').length`);
    await page.eval(`window.__rampNext = true`);
    await page.click('#reread');
    await page.idle();
    await sleep(300);
    const ramp = await page.eval(`(() => { const all = [...document.querySelectorAll('#light-cells polygon')];
      return { drawn: all.length, cells: ${JSON.stringify(RAMP_L)}.map((L) => { const e = all.find((p) => Number(p.dataset.light) === L);
        return e ? { L, op: e.getAttribute('fill-opacity'), fill: e.getAttribute('fill'), computed: getComputedStyle(e).fillOpacity } : { L, op: null }; }) }; })()`);
    const byL = (L) => ramp.cells.find((c) => c.L === L);
    const positives = ramp.cells.filter((c) => c.L > 0);
    const zeroText = (s) => /^0*\.?0*$/.test(s) || /^0(\.0+)?$/.test(s);
    check(byL(0).op === null && ramp.drawn === positiveBefore - 1, 'R2 ramp: L = 0 draws nothing (one injected zero cell disappears; every other cell is drawn)', { drawn: ramp.drawn, before: positiveBefore });
    // The computed value shows the browser parsed the attribute (an unparsable value would fall back to the default 1); the
    // browser may serialise a tiny computed number as 0, so positivity of the computed value is required only from 1e-4 up.
    check(positives.every((c) => c.op !== null && Number(c.op) > 0 && !zeroText(c.op) && Number(c.computed) < 1 && (c.L < 0.0001 || Number(c.computed) > 0)),
      'R2 ramp: every L > 0 has a strictly positive opacity text that never reads "0", and the browser parses it (computed value is not the default 1)', positives.map((c) => [c.L, c.op, c.computed]));
    const ordered = positives.map((c) => Number(c.op));
    check(ordered.every((v, i) => i === 0 || v > ordered[i - 1]) && Math.abs(Number(byL(1e-200).op) - 0.9 * Math.pow(1e-200, 0.65)) / (0.9 * Math.pow(1e-200, 0.65)) < 1e-3,
      'R2 ramp: continuous at 0 (no floor: opacity at L = 1e-200 is 0.9·L^0.65 to 3 significant figures) and strictly increasing over the chosen L', ordered);
    const sw = [0, 0.0001, 0.01, 0.05, 0.5, 1].map((L) => (L === 0 ? { colour: '#7f3b4a', op: 0 } : { colour: byL(L).fill, op: byL(L).op }));
    await page.eval(`(() => { const host = document.createElement('div'); host.id = 'test-swatches2'; host.style.position = 'fixed'; host.style.left = '0'; host.style.top = '0'; host.style.zIndex = '99';
      const NS = 'http://www.w3.org/2000/svg'; const s = document.createElementNS(NS, 'svg'); s.setAttribute('width', '360'); s.setAttribute('height', '40');
      ${JSON.stringify(sw)}.forEach((p, i) => { const bg = document.createElementNS(NS, 'rect'); bg.setAttribute('x', i * 60); bg.setAttribute('width', 60); bg.setAttribute('height', 40); bg.setAttribute('fill', 'rgb(27,36,48)');
        const fg = document.createElementNS(NS, 'rect'); fg.setAttribute('x', i * 60 + 10); fg.setAttribute('y', 10); fg.setAttribute('width', 40); fg.setAttribute('height', 20); fg.setAttribute('fill', p.colour); fg.setAttribute('fill-opacity', p.op); s.append(bg, fg); });
      host.append(s); document.body.append(host); })()`);
    await sleep(150);
    const img2 = await page.capture({ x: 0, y: 0, width: 360, height: 40 });
    await page.eval(`document.getElementById('test-swatches2').remove()`);
    const deltas = [0, 1, 2, 3, 4, 5].map((i) => { const bg = img2.px(i * 60 + 4, 4), fg = img2.px(i * 60 + 30, 20); return fg.reduce((s, v, k) => s + Math.abs(v - bg[k]), 0); });
    info('ramp table (L → opacity text, colour, summed RGB change over the floor): ' + [0, 0.0001, 0.01, 0.05, 0.5, 1].map((L, i) => L + ' → ' + (L === 0 ? 'not drawn' : byL(L).op + ' ' + byL(L).fill) + ', Δ' + deltas[i]).join(' | ') +
      ' | 1e-9 → ' + byL(1e-9).op + ' | 1e-200 → ' + byL(1e-200).op + ' (computed ' + byL(1e-200).computed + ')');
    check(deltas[0] === 0 && deltas[2] > 0 && deltas[2] <= 24 && deltas[3] > deltas[2] && deltas[4] > deltas[3] && deltas[5] > deltas[4] && deltas[1] <= deltas[2],
      'R2 ramp pixels: L = 0 unchanged; L = 0.0001 at most as visible as 0.01; 0.01 faint but visible; rising through 0.05, 0.5 and 1', deltas);
    await page.click('#reread');
    await page.idle();
    await sleep(300);
    check((await page.eval(`document.querySelectorAll('#light-cells polygon').length`)) === positiveBefore, 'R2 ramp: a normal re-read restores the engine values (all real cells drawn again)');

    // ---- H. Work Plan: Pending → Approve → Execute unchanged ---------------------------------------------------------------
    await page.click('#light-toggle');
    await page.click('[data-pane-button="plan"]');
    await page.click('#approve');
    await page.idle();
    await page.waitFor(`!document.getElementById('approved-box').hidden`, 'the approved plan');
    const sAppr = await server(page);
    check(sAppr.approved && sAppr.approved.slice().sort().join() === chosen.slice().sort().join() && sAppr.events === 0 && sAppr.living.join() === s0.living.join(),
      'Approve fells nothing and approves exactly the queued IDs', sAppr);

    // ---- I. F-2: an unpaired read-out is "unavailable", never model 0; Execute withdrawn until a paired re-read ----------
    await page.eval(`(() => { window.__unpairNext = false; const real = window.fetch.bind(window);
      window.fetch = async (input, init) => { const r = await real(input, init); if (window.__unpairNext && String(input).endsWith('api/multi/view')) { window.__unpairNext = false;
        const j = await r.json(); j.stage4 = null; return new Response(JSON.stringify(j), { status: r.status, headers: { 'Content-Type': 'application/json' } }); } return r; }; })()`);
    await page.eval(`window.__unpairNext = true`);
    await page.click('#reread');
    await page.idle();
    await page.waitFor(`!document.getElementById('readout-unavailable').hidden`, 'the unavailable state');
    const f2 = await page.eval(`({ exec: document.getElementById('execute').disabled, execText: document.getElementById('execute').textContent, approve: document.getElementById('approve').disabled,
      disclosure: document.getElementById('disclosure').textContent + ' | ' + document.getElementById('flags').textContent, note: document.getElementById('details-note').textContent,
      statements: document.getElementById('statements').textContent, cards: document.getElementById('area-cards').textContent })`);
    check(f2.exec && f2.execText.includes('re-read needed') && f2.approve, 'F-2: with stage4 null, Execute (and Approve) are withdrawn', f2.execText);
    check(!/EDGE NEIGHBOURS EXCLUDED|Edge neighbours excluded/i.test(f2.disclosure + f2.statements) && !f2.note.includes('Light, seed') && f2.disclosure.includes('unavailable') &&
      f2.cards.includes('read-out unavailable'), 'F-2: no model 0 / legacy claim (no edge exclusion, no "Light NOT MODELLED"); the page says unavailable', f2.note);
    await shot(page, '09-f2-readout-unavailable');
    await page.click('#readout-reread');
    await page.idle();
    await page.waitFor(`document.getElementById('readout-unavailable').hidden && !document.getElementById('execute').disabled`, 'a paired re-read');
    check(true, 'F-2: a paired re-read restores Execute');
    // An Execute confirmation open when an unpaired view arrives is withdrawn unsent.
    await page.click('#execute');
    await page.waitFor(`document.getElementById('execute-dialog').open`, 'the Execute confirmation');
    await page.eval(`window.__unpairNext = true; document.getElementById('reread').click()`);
    await sleep(600);
    const f3 = await page.eval(`({ open: document.getElementById('execute-dialog').open, alert: document.getElementById('alert').textContent })`);
    const sF3 = await server(page);
    check(!f3.open && f3.alert.includes('Confirmation withdrawn') && f3.alert.includes('read-out of this view is unavailable') && sF3.events === 0,
      'F-2: an open Execute confirmation is withdrawn unsent when the read-out becomes unavailable', f3.alert.slice(0, 120));
    await page.click('#alert button');
    await page.idle();
    await page.waitFor(`!document.getElementById('execute').disabled`, 'the lock lifted by a paired re-read');
    // Execute for real: the engine's fenced semantics, unchanged.
    await page.click('#execute');
    await page.waitFor(`document.getElementById('execute-dialog').open`, 'the Execute confirmation');
    const execText = await page.eval(`document.getElementById('execute-text').textContent`);
    await page.click('#execute-confirm');
    await page.idle();
    await page.waitFor(`document.getElementById('approved-box').hidden`, 'execution');
    const sExec = await server(page);
    check(sExec.events === 1 && sExec.living[0] === s0.living[0] - chosen.length && execText.includes('Fells exactly ' + chosen.length),
      'Execute fells exactly the confirmed trees through the unchanged fenced path', { execText, living: sExec.living });
    await page.click('#advance');
    await sleep(200);
    // The Area B draft choice from section G is local and unsent, so Year+1 first asks to discard it.
    if (await page.eval(`document.getElementById('consent-dialog').open`)) await page.click('#consent-advance');
    await page.idle();
    await page.waitFor(`document.getElementById('year-value').textContent === '1'`, 'Year+1');
    check((await server(page)).year === 1, 'Year+1 still advances one ecological year');

    // ---- J. global ---------------------------------------------------------------------------------------------------------
    const lEnd = await page.eval(LAYOUT);
    check(lEnd.scrollWidth <= lEnd.innerWidth && lEnd.bodyScrollWidth <= lEnd.innerWidth, 'no sideways scroll at the end', lEnd);
    const csp = await page.eval('window.__csp');
    check(csp.length === 0, 'no CSP violation (the rank buttons are positioned through CSSOM, not style attributes)', csp);
    check(page.log.exceptions.length === 0 && page.log.console.length === 0, 'no script exception and no console error or warning', { ex: page.log.exceptions, console: page.log.console });
    check(page.log.http5xx.length === 0, 'no HTTP 5xx', page.log.http5xx);
    console.log('STATUSES ' + JSON.stringify(page.log.statuses));
  } finally {
    await page.close();
  }
}

main().then(() => {
  fs.writeFileSync(path.join(OUT, 'browser-results-' + tag + '.json'), JSON.stringify({ viewport: tag, failures, results, shots }, null, 1));
  const checks = results.filter((r) => !r.startsWith('INFO ')).length;
  console.log('RESULT ' + tag + ': ' + (checks - failures) + '/' + checks + ' PASS');
  process.exit(failures === 0 ? 0 : 1);
}).catch((error) => {
  console.log('ABORT ' + redact(error && error.stack || error));
  fs.writeFileSync(path.join(OUT, 'browser-results-' + tag + '.json'), JSON.stringify({ viewport: tag, aborted: redact(String(error)), failures: failures + 1, results, shots }, null, 1));
  process.exit(2);
});
