// Stage 4 owner UX repair, Phase A, R3: the UX-01 fold for EVERY living tree, in a clean AND a loaded session, in real
// headless Microsoft Edge over CDP. Written after the independent review found (F-01) that the earlier driver only ever
// picked references whose five competitors were all unblocked, so a blocked row's extra line was never measured.
//
// One fresh server session, three states, each swept over every living tree of both areas, selected by exact ID with the
// real Enter key:
//   clean     year 0, empty plan;
//   queued    4 Fell orders added through the page's own Add (real mouse on map ranks 1-4 of one Area A reference R_A,
//             so trees with 1, 2, 3 and 4 blocked rows exist); the post-Add card (F-02) is checked here;
//   approved  plus Crop on R_A's rank-5 competitor and on ranks 1-2 of one Area B reference R_B, then the plan approved
//             ("Approved plan" and "Retained Crop" rows; R_A then has all five rows blocked).
// In clean and queued, each tree is measured untouched and again with every unblocked row ticked (DOM click on the
// checkboxes; the main scenario does the worst cases with the real mouse). An approved plan locks the draft.
// A tree passes when, with no scrolling, the reference card, ranks 1-k and the rendered Add button are inside the Tree
// pane and the viewport, the last row ends above the Add dock, every row is at least 36 px and all rows share one height.
// Usage (Windows node v26): node fold-sweep-owner-ux.mjs <evidenceDir> <fileHoldingTheBaseUrl> <W>x<H>
// Every output line goes through redact(); the base URL file is outside the repository and is never printed.
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
function info(text) { const line = 'INFO [' + tag + '] ' + redact(text); results.push(line); console.log(line); }
function check(ok, label, detail) {
  if (!ok) failures++;
  const line = (ok ? 'PASS ' : 'FAIL ') + '[' + tag + '] ' + label + (detail === undefined ? '' : ' :: ' + redact(typeof detail === 'string' ? detail : JSON.stringify(detail)));
  results.push(line);
  console.log(line);
}
const shots = [];

const MEASURE = `(() => { const rows = [...document.querySelectorAll('#competition-content .comp-row')]; const rr = rows.map((e) => e.getBoundingClientRect());
  const pane = document.getElementById('pane-tree'); const pr = pane.getBoundingClientRect(); const dock = (document.getElementById('add-dock') || document.querySelector('.add-dock')).getBoundingClientRect();
  const add = document.getElementById('add-button'); const ab = add.getBoundingClientRect(); const ref = document.querySelector('.ref-card').getBoundingClientRect();
  const close = document.getElementById('inspect-close').getBoundingClientRect(); const card = document.getElementById('batch-result'); const cb = card.getBoundingClientRect();
  const empty = document.querySelector('#competition-content .comp-empty');
  return { id: document.getElementById('insp-id').textContent, rows: rows.length, blocked: rows.filter((e) => e.classList.contains('blocked')).length,
    ticked: rows.filter((e) => e.classList.contains('chosen')).length, rowH: rr.map((b) => Math.round(b.height)),
    firstTop: rr.length ? Math.round(rr[0].top) : null, lastBottom: rr.length ? Math.round(rr[rr.length - 1].bottom) : null,
    paneTop: Math.round(pr.top), paneBottom: Math.round(pr.bottom), dockTop: Math.round(dock.top), dockBottom: Math.round(dock.bottom),
    addTop: Math.round(ab.top), addBottom: Math.round(ab.bottom), addH: Math.round(ab.height), addText: add.textContent, addRendered: ab.height > 0,
    scrollTop: pane.scrollTop, refTop: Math.round(ref.top), closeTop: Math.round(close.top), closeBottom: Math.round(close.bottom), innerHeight,
    cardShown: !card.hidden, cardTop: Math.round(cb.top), cardBottom: Math.round(cb.bottom), cardH: Math.round(cb.height),
    empty: empty ? empty.textContent : null, why: rows.map((e) => { const w = e.querySelector('.comp-why'); return w ? w.textContent : null; }),
    sr: rows.map((e) => { const s = e.querySelector('.comp-line .sr-only'); return s ? s.textContent : null; }) }; })()`;

// The fold rule (see the header). gap = rows to dock; room = free pane space below the dock.
function foldPass(m) {
  if (m.rows === 0) return m.scrollTop === 0 && m.empty !== null;
  return m.scrollTop === 0 && m.refTop >= m.paneTop && m.closeTop >= m.paneTop && m.firstTop >= m.paneTop && m.lastBottom <= m.dockTop + 1 &&
    m.addRendered && m.addTop >= m.lastBottom && m.addBottom <= Math.min(m.innerHeight, m.paneBottom + 1) && m.addH >= 40 &&
    Math.min(...m.rowH) >= 36 && Math.max(...m.rowH) - Math.min(...m.rowH) <= 1;
}

async function selectById(page, id) {
  await page.eval(`(() => { const q = document.getElementById('quick-id'); q.focus(); q.value = ${JSON.stringify(id)}; })()`);
  await page.key('Enter');
  await page.waitFor(`document.getElementById('insp-id').textContent === ${JSON.stringify(id)} && !document.getElementById('inspector-body').hidden &&
    !document.documentElement.classList.contains('busy') && (document.querySelector('#competition-content .comp-rows') ||
    [...document.querySelectorAll('#competition-content .comp-empty')].some((e) => !e.textContent.startsWith('Reading')))`, 'the competitors of ' + id, 10000);
  await sleep(60);
}

async function sweep(page, phase, tick) {
  const ids = await page.eval(`(async () => { const p = await (await fetch('api/multi/view', { cache: 'no-store' })).json();
    return p.view.areas.flatMap((a) => a.trees.filter((t) => t.isLiving).map((t) => t.treeId)); })()`);
  const records = [];
  for (const id of ids) {
    await selectById(page, id);
    const m = await page.eval(MEASURE);
    m.pass = foldPass(m);
    m.state = 'untouched';
    records.push(m);
    if (tick && m.rows > 0) {
      // One at a time: each tick re-renders the rows, so the next free checkbox is looked up again.
      const n = await page.eval(`(() => { let n = 0; for (let k = 0; k < 5; k++) {
        const free = [...document.querySelectorAll('#competition-content [data-draft]')].find((i) => !i.disabled && !i.checked);
        if (!free) break; free.click(); n++; } return n; })()`);
      if (n > 0) {
        await sleep(60);
        const t = await page.eval(MEASURE);
        t.pass = foldPass(t) && t.ticked === n && t.addText === 'Add selected ' + n + ' to Fell plan';
        t.state = 'ticked';
        records.push(t);
      }
    }
  }
  const withRows = records.filter((r) => r.rows > 0);
  const byBlocked = {};
  for (const r of withRows) {
    const k = r.blocked;
    byBlocked[k] = byBlocked[k] || { measured: 0, pass: 0, fail: 0, trees: new Set() };
    byBlocked[k].measured++;
    byBlocked[k].trees.add(r.id);
    if (r.pass) byBlocked[k].pass++; else byBlocked[k].fail++;
  }
  const histogram = Object.fromEntries(Object.entries(byBlocked).map(([k, v]) => [k, { trees: v.trees.size, measured: v.measured, pass: v.pass, fail: v.fail }]));
  const failed = records.filter((r) => !r.pass);
  const gaps = withRows.map((r) => r.dockTop - r.lastBottom);
  const rooms = withRows.map((r) => r.paneBottom - r.dockBottom);
  const minGap = Math.min(...gaps);
  const minRoom = Math.min(...rooms);
  const worstGap = withRows[gaps.indexOf(minGap)];
  const treesWithBlocked = new Set(withRows.filter((r) => r.blocked > 0).map((r) => r.id)).size;
  const summary = { phase, treesSwept: ids.length, treesWithRows: new Set(withRows.map((r) => r.id)).size,
    treesWithoutCompetitors: records.filter((r) => r.rows === 0).length, treesWithBlockedRows: treesWithBlocked,
    measurements: records.length, tickedMeasurements: records.filter((r) => r.state === 'ticked').length, pass: records.length - failed.length, fail: failed.length,
    byBlockedRows: histogram, minRowsToDockGapPx: minGap, worstGapTree: worstGap && worstGap.id + ' (' + worstGap.state + ', ' + worstGap.blocked + ' blocked)',
    minFreeRoomBelowDockPx: minRoom, rowHeights: [Math.min(...withRows.flatMap((r) => r.rowH)), Math.max(...withRows.flatMap((r) => r.rowH))],
    failed: failed.slice(0, 20).map((r) => ({ id: r.id, state: r.state, blocked: r.blocked, lastBottom: r.lastBottom, dockTop: r.dockTop, addBottom: r.addBottom, rowH: r.rowH })) };
  info('SWEEP ' + phase + ' ' + JSON.stringify(summary));
  check(failed.length === 0 && ids.length > 0,
    'F-01/UX-01 ' + phase + ': all ' + ids.length + ' living trees (' + treesWithBlocked + ' with blocked rows; ' + summary.measurements + ' measurements) keep ranks 1-k and Add above the fold with no scrolling',
    { fail: failed.length, minGap, minRoom });
  return { summary, records };
}

async function main() {
  if (!(await reachable(base + 'multi'))) throw new Error('server not reachable from Windows');
  const page = await launch();
  const phases = [];
  try {
    await page.viewport(W, H);
    await page.goto(base + 'multi');
    await page.waitFor(`document.getElementById('tree-rows').childNodes.length > 0 && document.querySelectorAll('.area-readouts dt').length >= 6`, 'first view');
    await sleep(500);
    const v0 = await page.eval(`(async () => { const v = (await (await fetch('api/multi/view')).json()).view; return { year: v.source.ecologicalYear, pending: v.pending.length, approved: v.approved }; })()`);
    check(v0.year === 0 && v0.pending === 0 && v0.approved === null, 'a fresh session: year 0, empty plan', v0);

    phases.push(await sweep(page, 'clean', true));

    // R_A and R_B: the first tree of each area (exact-ID order) with five ranked competitors (read-only reads).
    const refs = await page.eval(`(async () => { const p = await (await fetch('api/multi/view')).json(); const v = p.view;
      const fence = { sessionId: v.sessionId, source: v.source, planStateRevision: v.planStateRevision }; const out = {};
      for (const a of v.areas) { for (const t of a.trees.filter((x) => x.isLiving)) {
        const r = await (await fetch('api/multi/competition', { method: 'POST', headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ fence, areaId: a.areaId, treeId: t.treeId }) })).json();
        if (r.competition && r.competition.topFive.length === 5) { out[a.areaId] = { ref: t.treeId, top: r.competition.topFive.map((n) => n.treeId) }; break; } } }
      return out; })()`);
    const RA = refs['MA-A'];
    const RB = refs['MA-B'];
    info('R_A ' + RA.ref + ' top five ' + RA.top.join(', ') + '; R_B ' + RB.ref + ' top five ' + RB.top.join(', '));

    // queued: ranks 1-4 of R_A with the real mouse on the map rank buttons, then the real Add button.
    await selectById(page, RA.ref);
    for (let k = 1; k <= 4; k++) {
      const b = await page.eval(`(() => { const e = document.querySelector('#rank-layer [data-rank="${k}"]'); const r = e.getBoundingClientRect(); return { x: r.left + r.width / 2, y: r.top + r.height / 2 }; })()`);
      await page.clickAt(b.x, b.y);
      await sleep(120);
    }
    const before = await page.eval(MEASURE);
    check(before.addText === 'Add selected 4 to Fell plan' && foldPass(before), 'four ranks ticked on ' + RA.ref + ' with the real mouse; the fold holds', before.addText);
    await page.click('#add-button');
    await page.idle();
    await page.waitFor(`!document.getElementById('batch-result').hidden && document.querySelectorAll('#competition-content .comp-row').length === 5 &&
      document.querySelectorAll('#competition-content .comp-row.blocked').length === 4`, 'the Add result with four blocked rows', 10000);
    await sleep(300);
    const post = await page.eval(MEASURE);
    const pending = await page.eval(`(async () => (await (await fetch('api/multi/view')).json()).view.pending.map((o) => o.treeId))()`);
    check(pending.slice().sort().join() === RA.top.slice(0, 4).sort().join(), 'the page\'s Add queued exactly ranks 1-4 of ' + RA.ref, pending);
    const short = H <= 820;
    // F-02: after Add the card stands in for the Add form; on a short desktop it is one line. Rows 1-5 stay above it.
    check(post.cardShown && !post.addRendered && post.rows === 5 && post.blocked === 4 && post.scrollTop === 0 && post.lastBottom <= post.cardTop + 1 &&
      post.lastBottom <= post.dockTop + 1 && post.dockBottom <= Math.min(post.innerHeight, post.paneBottom + 1) && Math.max(...post.rowH) - Math.min(...post.rowH) <= 1 &&
      (!short || post.cardH <= 40),
      'F-02: after Add, ranks 1-5 (4 now blocked) stay above the result card, which takes the Add form\'s place' + (short ? ' as one line' : ''),
      { lastBottom: post.lastBottom, cardTop: post.cardTop, cardH: post.cardH, dock: [post.dockTop, post.dockBottom], rowH: post.rowH });
    check(post.why.slice(0, 4).every((w) => w === 'Pending Fell') && post.sr.slice(0, 4).every((s) => s === 'Cannot be chosen: Already pending Fell. ') && post.why[4] === null,
      'F-01: each blocked row shows "Pending Fell" at the start of its meta line; the full reason is screen-reader text', { why: post.why, sr: post.sr });
    const cardShot = await page.eval(`(() => { const b = document.querySelector('.side').getBoundingClientRect(); return { x: Math.round(b.left), y: Math.round(b.top), width: Math.round(b.width), height: Math.round(b.height) }; })()`);
    const f1 = path.join(OUT, '12-after-add-card-' + tag + '.png');
    shots.push({ name: path.basename(f1), bytes: await page.shotClip(f1, cardShot) });
    const hasDetails = await page.eval(`!!document.querySelector('#batch-result [data-batch="more"]')`);
    if (short && !hasDetails) check(false, 'F-02: the post-Add card has a Details control');
    if (short && hasDetails) {
      await page.click('#batch-result [data-batch="more"]');
      const open = await page.eval(MEASURE);
      const expanded = await page.eval(`document.querySelector('#batch-result [data-batch="more"]').getAttribute('aria-expanded')`);
      await page.click('#batch-result [data-batch="more"]');
      const closed = await page.eval(MEASURE);
      check(expanded === 'true' && open.cardH > post.cardH && closed.cardH === post.cardH && closed.lastBottom <= closed.dockTop + 1,
        'F-02: Details opens the card\'s lines on request and closes them again', { open: open.cardH, closed: closed.cardH });
    }
    // Ticking the free fifth rank ends the card and brings the Add form back; unticking leaves the session as queued.
    const b5 = await page.eval(`(() => { const e = document.querySelector('#rank-layer [data-rank="5"]'); const r = e.getBoundingClientRect(); return { x: r.left + r.width / 2, y: r.top + r.height / 2 }; })()`);
    await page.clickAt(b5.x, b5.y);
    await sleep(150);
    const t5 = await page.eval(MEASURE);
    check(!t5.cardShown && t5.addRendered && t5.addText === 'Add selected 1 to Fell plan' && foldPass(t5),
      'F-02: a draft change ends the card; the Add form is back and the fold holds', { card: t5.cardShown, add: t5.addText });
    await page.clickAt(b5.x, b5.y);
    await sleep(150);
    const f2 = path.join(OUT, '11-blocked-rows-' + tag + '.png');
    shots.push({ name: path.basename(f2), bytes: await page.shotClip(f2, cardShot) });

    phases.push(await sweep(page, 'queued', true));

    // approved: Crop on R_A's rank 5 and R_B's ranks 1-2 (the page's Crop button), then Approve in the Work Plan pane.
    for (const [areaId, id] of [['MA-A', RA.top[4]], ['MA-B', RB.top[0]], ['MA-B', RB.top[1]]]) {
      await selectById(page, id);
      await page.click('#crop-button');
      await page.idle();
      await page.waitFor(`document.getElementById('crop-button').textContent === 'Remove Crop designation'`, 'Crop on ' + id + ' in ' + areaId);
    }
    await page.click('[data-pane-button="plan"]');
    await page.click('#approve');
    await page.idle();
    await page.waitFor(`!document.getElementById('approved-box').hidden`, 'the approved plan');
    await page.click('[data-pane-button="tree"]');
    const v2 = await page.eval(`(async () => { const v = (await (await fetch('api/multi/view')).json()).view; return { approved: v.approved && v.approved.orders.map((o) => o.treeId), crops: v.areas.map((a) => a.cropTreeIds) }; })()`);
    check(v2.approved && v2.approved.length === 4 && v2.crops[0].includes(RA.top[4]) && v2.crops[1].includes(RB.top[0]) && v2.crops[1].includes(RB.top[1]),
      'approved plan of four plus three Crop designations', v2);
    await selectById(page, RA.ref);
    const all5 = await page.eval(MEASURE);
    check(all5.blocked === 5 && foldPass(all5), 'F-01 worst case: ' + RA.ref + ' with all five rows blocked (4 approved + 1 Crop) keeps ranks 1-5 and Add above the fold',
      { why: all5.why, lastBottom: all5.lastBottom, dockTop: all5.dockTop, rowH: all5.rowH });
    phases.push(await sweep(page, 'approved', false));

    const end = await page.eval(`({ sw: document.documentElement.scrollWidth, iw: innerWidth })`);
    check(end.sw <= end.iw, 'no sideways scroll at the end', end);
    check(page.log.exceptions.length === 0 && page.log.console.length === 0 && page.log.http5xx.length === 0, 'no script exception, console error or HTTP 5xx',
      { ex: page.log.exceptions, console: page.log.console, http5xx: page.log.http5xx });
  } finally {
    await page.close();
  }
  return phases;
}

main().then((phases) => {
  fs.writeFileSync(path.join(OUT, 'fold-sweep-' + tag + '.json'), redact(JSON.stringify({ viewport: tag, failures, results, shots, phases: phases.map((p) => p.summary) }, null, 1)));
  fs.writeFileSync(path.join(OUT, 'fold-sweep-records-' + tag + '.json'), redact(JSON.stringify(phases.map((p) => ({ phase: p.summary.phase, records: p.records })))));
  const checks = results.filter((r) => !r.startsWith('INFO ')).length;
  console.log('RESULT ' + tag + ': ' + (checks - failures) + '/' + checks + ' PASS');
  process.exit(failures === 0 ? 0 : 1);
}).catch((error) => {
  console.log('ABORT ' + redact(error && error.stack || error));
  fs.writeFileSync(path.join(OUT, 'fold-sweep-' + tag + '.json'), redact(JSON.stringify({ viewport: tag, aborted: String(error), failures: failures + 1, results, shots }, null, 1)));
  process.exit(2);
});
