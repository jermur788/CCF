// /multi Stage 1–4 UI catch-up: real-browser verification in headless Microsoft Edge over CDP.
// Usage: node scenario.mjs <evidenceDir>
import fs from 'node:fs';
import path from 'node:path';
import { launch, reachable, redact, sleep } from './cdp.mjs';

const OUT = process.argv[2];
fs.mkdirSync(OUT, { recursive: true });
const base = fs.readFileSync('\\\\wsl.localhost\\Ubuntu\\tmp\\ui900\\run\\url', 'utf8').trim();
const results = [];
const facts = {};
let failures = 0;
function check(ok, label, detail) {
  if (!ok) failures++;
  const line = (ok ? 'PASS ' : 'FAIL ') + label + (detail === undefined ? '' : ' :: ' + redact(typeof detail === 'string' ? detail : JSON.stringify(detail)));
  results.push(line);
  console.log(line);
}
const shots = [];
async function shot(page, name) {
  const file = path.join(OUT, name);
  const bytes = await page.shot(file);
  shots.push({ name, bytes });
  console.log('SHOT ' + name + ' ' + bytes + ' bytes');
}

// ---- in-page helpers (all read the engine payload or the DOM; they compute nothing ecological) ------------------------
const API = `(async () => (await (await fetch('api/multi/view', { cache: 'no-store' })).json()))()`;
const apiPick = (body) => `(async () => { const p = await (await fetch('api/multi/view', { cache: 'no-store' })).json(); ${body} })()`;
const LAYOUT = `(() => { const d = document.documentElement; const dock = document.getElementById('map-dock').getBoundingClientRect();
  const stand = document.getElementById('area-outline').getBoundingClientRect(); const pane = document.getElementById('map-pane').getBoundingClientRect();
  const overlap = !(stand.right <= dock.left || stand.left >= dock.right || stand.bottom <= dock.top || stand.top >= dock.bottom);
  const r = (b) => [b.left, b.top, b.right, b.bottom].map(Math.round);
  return { scrollWidth: d.scrollWidth, innerWidth, bodyScrollWidth: document.body.scrollWidth, overlap,
    standInPane: stand.left >= pane.left - 1 && stand.right <= pane.right + 1 && stand.top >= pane.top - 1 && stand.bottom <= pane.bottom + 1,
    dock: r(dock), stand: r(stand), pane: r(pane), zoom: document.getElementById('zoom-state').textContent,
    legend: document.getElementById('workspace').dataset.legend, light: document.getElementById('light-toggle').getAttribute('aria-pressed'),
    bandNumbers: document.querySelectorAll('#light-cells .lc-text').length, cells: document.querySelectorAll('#light-cells polygon').length,
    badgeLetters: [...new Set([...document.querySelectorAll('#marks .badge-text')].map((t) => t.textContent))],
    legendNames: [...document.querySelectorAll('#legend .legend-name')].map((n) => n.textContent),
    rampItems: [...document.querySelectorAll('#light-ramp li')].map((n) => n.textContent),
    readouts: [...document.querySelectorAll('.area-readouts')].map((n) => n.textContent) }; })()`;
const READOUTS = `[...document.querySelectorAll('.area-card')].map((c) => c.textContent)`;
const num = (text, re) => { const m = re.exec(text); return m ? Number(m[1]) : NaN; };
const coverOf = (card) => num(card, /Canopy cover(\d+)%/);
const openOf = (card) => num(card, /open (\d+) m²/);

async function layoutAll(page, tag) {
  for (const [w, h] of [[1920, 1080], [1280, 720]]) {
    await page.viewport(w, h);
    for (const legendHidden of [false, true]) {
      for (const light of [false, true]) {
        const cur = await page.eval(LAYOUT);
        if ((cur.legend === 'hidden') !== legendHidden) await page.click(legendHidden ? '#legend-hide' : '#legend-show');
        if ((cur.light === 'true') !== light) await page.click('#light-toggle');
        await page.click('#zoom-fit');
        await sleep(350);
        const L = await page.eval(LAYOUT);
        const label = `${tag} ${w}x${h} legend ${legendHidden ? 'hidden' : 'shown'} light ${light ? 'on' : 'off'}`;
        check(L.scrollWidth <= L.innerWidth && L.bodyScrollWidth <= L.innerWidth, label + ': no sideways scroll',
          { scrollWidth: L.scrollWidth, innerWidth: L.innerWidth });
        check(!L.overlap && L.standInPane && L.zoom === 'Fit', label + ': stand clear of the navigation dock at Fit', { stand: L.stand, dock: L.dock });
        if (light) check(L.cells > 0 && L.bandNumbers === L.cells && L.rampItems.length === 5, label + ': light cells carry band numbers + 5-band legend',
          { cells: L.cells, numbers: L.bandNumbers });
      }
    }
    // restore: legend shown, light off
    const cur = await page.eval(LAYOUT);
    if (cur.legend === 'hidden') await page.click('#legend-show');
    if (cur.light === 'true') await page.click('#light-toggle');
  }
}

async function selectTree(page, treeId) {
  await page.click('[data-pane-button="trees"]');
  await page.eval(`(() => { const i = document.getElementById('tree-search'); i.value = ${JSON.stringify(treeId)}; i.dispatchEvent(new Event('input')); })()`);
  await page.click(`[data-select="${treeId}"]`);
  await page.waitFor(`document.getElementById('insp-id').textContent === ${JSON.stringify(treeId)}`, 'inspector ' + treeId);
  await page.eval(`(() => { const i = document.getElementById('tree-search'); i.value = ''; i.dispatchEvent(new Event('input')); })()`);
}

async function showArea(page, areaId) {
  await page.click(`.area-tab[data-area="${areaId}"]`);
  await page.waitFor(`document.querySelector('.area-tab[data-area="${areaId}"]').getAttribute('aria-pressed') === 'true'`, 'area ' + areaId);
  await sleep(200);
}

async function clickCell(page, key) {
  const r = await page.eval(`(() => { const e = document.querySelector('#light-cells [data-cell="${key}"]'); if (!e) return null;
    const b = e.getBoundingClientRect(); return { x: b.left + b.width / 2, y: b.top + b.height / 2 }; })()`);
  if (!r) throw new Error('no cell ' + key);
  await page.clickAt(r.x, r.y);
  await page.waitFor(`!document.getElementById('ground-panel').hidden`, 'ground inspector');
}

const groundText = `(() => ({ heading: document.getElementById('ground-heading').textContent, label: document.getElementById('ground-label').textContent,
  facts: document.getElementById('ground-facts').textContent, caveat: document.getElementById('ground-caveat').textContent }))()`;

async function addAndExecute(page, expectText) {
  await page.click('[data-pane-button="plan"]');
  await page.click('#approve');
  await page.idle();
  await page.waitFor(`!document.getElementById('approved-box').hidden`, 'approved plan');
  await page.click('#execute');
  await page.waitFor(`document.getElementById('execute-dialog').open`, 'execute confirmation');
  const text = await page.eval(`document.getElementById('execute-text').textContent + ' | ' + document.getElementById('execute-confirm').textContent`);
  if (expectText) check(text.includes(expectText), 'Execute confirmation names the work', text);
  await page.click('#execute-confirm');
  await page.idle();
  await page.waitFor(`document.getElementById('approved-box').hidden`, 'execution done');
  return text;
}

async function main() {
  if (!(await reachable(base + 'multi'))) throw new Error('server not reachable from Windows');
  const page = await launch();
  try {
    await page.viewport(1920, 1080);
    await page.goto(base + 'multi');
    await page.waitFor(`document.getElementById('tree-rows').childNodes.length > 0 && document.querySelectorAll('.area-readouts dt').length >= 6`, 'first view');
    await sleep(500);

    // ---- S0 baseline: 1/1/1, realistic C2 radii, readouts, layout --------------------------------------------------
    const v0 = await page.eval(apiPick(`const m = p.stage4.models; const radii = p.view.areas.map((a) => a.trees.map((t) => t.crownRadiusM));
      return { models: [m.crownModelVersion, m.adultModelVersion, m.mortalityModelVersion, m.lightModelVersion, m.calibratedPar],
        radius: radii.map((r) => ({ min: Math.min(...r), max: Math.max(...r), mean: r.reduce((a, b) => a + b, 0) / r.length })),
        cover: p.stage4.areas.map((a) => a.canopy.geometricCoverFraction), year: p.view.source.ecologicalYear };`));
    facts.baseline = v0;
    check(v0.models.join() === '1,1,1,unity-k10a10-decoupled-v1,false', '/multi loads on 1/1/1 with the k10a10 decoupled light model, not calibrated PAR', v0.models);
    check(v0.radius.every((r) => r.min > 0.5 && r.max < 6), 'Area A/B crown radii are C2-realistic', v0.radius);
    const cards0 = await page.eval(READOUTS);
    facts.cards0 = cards0;
    check(cards0.every((c) => /Canopy cover\d+%/.test(c) && /open \d+ m²/.test(c) && /RD \d\.\d\d/.test(c) && /\d+ this year/.test(c) &&
      /logs? total/.test(c) && /Model 1\/1\/1/.test(c) && c.includes('Synthetic stand') && c.includes('Light: not PAR')), 'both rack cards show the four readouts', cards0);
    const disclosure = await page.eval(`document.getElementById('disclosure').textContent + ' | ' + document.getElementById('flags').textContent`);
    check(disclosure.includes('CONNECTED WOODLAND') && disclosure.includes('Connected woodland') && !disclosure.includes('EDGE NEIGHBOURS EXCLUDED'),
      'the disclosure states the connected-woodland Stage 4 model (no stale edge-exclusion claim)', disclosure);
    await layoutAll(page, 'S0');
    await page.viewport(1920, 1080);
    await page.click('#zoom-fit');
    await shot(page, 'a-baseline-1920x1080.png');
    await page.viewport(1280, 720);
    await page.click('#zoom-fit');
    await shot(page, 'a-baseline-1280x720.png');
    await page.viewport(1920, 1080);

    // ---- S1 Area A: reference + 2 ticked Fell drafts, Light Guide, ground cell inspection keeps the drafts ------------
    const plan = await page.eval(apiPick(`
      const a = p.stage4.areas[0], b = p.stage4.areas[1];
      // Area A cell with the most contributors near the middle of the stand; the B border cell shaded most by A trees.
      const cells = a.ground.slice().sort((x, y) => y.contributingTreeIds.length - x.contributingTreeIds.length);
      const cell = cells[Math.floor(cells.length / 4)];
      const border = b.ground.filter((c) => c.contributingTreeIds.some((id) => id.startsWith('MA-A-')))
        .sort((x, y) => y.contributingTreeIds.filter((id) => id.startsWith('MA-A-')).length - x.contributingTreeIds.filter((id) => id.startsWith('MA-A-')).length)[0];
      return { cellKey: cell.column + ':' + cell.row, cellLight: cell.lightIndex, contributors: cell.contributingTreeIds,
        borderKey: border ? border.column + ':' + border.row : null, borderLight: border ? border.lightIndex : null,
        borderA: border ? border.contributingTreeIds.filter((id) => id.startsWith('MA-A-')) : [] };`));
    facts.plan = plan;
    check(plan.borderKey !== null, 'an Area B ground cell is shaded by Area A trees (connected woodland light)', plan);
    const r1 = plan.contributors[0];
    await selectTree(page, r1);
    await page.click('[data-pane-button="tree"]');
    await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0`, 'competitors of ' + r1);
    const drafts = await page.eval(`[...document.querySelectorAll('#competition-content [data-draft]:not(:disabled)')].slice(0, 2).map((e) => e.getAttribute('data-draft'))`);
    for (const id of drafts) await page.click(`[data-draft="${id}"]`);
    await page.waitFor(`document.getElementById('draft-count').textContent.startsWith('2 selected')`, 'two ticked drafts');
    await page.click('#light-toggle');
    await page.waitFor(`document.querySelectorAll('#light-cells polygon').length > 0`, 'light cells');
    await clickCell(page, plan.cellKey);
    const g0 = await page.eval(groundText);
    facts.ground0 = g0;
    check(g0.label === 'Modelled Light Guide (Unity k10a10 proxy — not calibrated PAR)' && g0.facts.includes('Modelled Light Index (0–1)') &&
      g0.facts.includes('Overhead cover fraction') && g0.facts.includes('Contributing trees') && g0.caveat.includes('NOT measured or calibrated PAR'),
      'the ground inspector shows the exact label, light index, cover fraction, contributors and the caveat', g0);
    const lightLabel = await page.eval(`document.getElementById('light-legend-title').textContent`);
    check(lightLabel === 'Modelled Light Guide (Unity k10a10 proxy — not calibrated PAR)', 'the Light Guide legend carries the exact label', lightLabel);
    const kept = await page.eval(`({ count: document.getElementById('draft-count').textContent, ref: document.getElementById('insp-id').textContent,
      ticked: [...document.querySelectorAll('#competition-content [data-draft]')].filter((e) => e.checked).map((e) => e.getAttribute('data-draft')) })`);
    check(kept.count.startsWith('2 selected') && kept.ref === r1 && kept.ticked.join() === drafts.join(), 'inspecting a cell kept the reference and the unsent ticked Fell drafts', kept);
    await shot(page, 'b-light-guide-ground-1920x1080.png');
    await page.viewport(1280, 720);
    await page.click('#zoom-fit');
    await shot(page, 'b-light-guide-ground-1280x720.png');
    await page.viewport(1920, 1080);

    // ---- S2 Prune gate in Area B ---------------------------------------------------------------------------------------
    await showArea(page, 'MA-B');
    const pTree = await page.eval(apiPick(`return p.stage4.areas[1].trees.find((t) => t.pruneEligible).treeId;`));
    await selectTree(page, pTree);
    await page.click('[data-pane-button="tree"]');
    const before = await page.eval(apiPick(`const t = p.view.areas[1].trees.find((x) => x.treeId === ${JSON.stringify(pTree)});
      return { r: t.crownRadiusM, cover: p.stage4.areas[1].canopy.geometricCoverFraction, light: p.stage4.areas[1].canopy.meanLightIndex,
        year: p.view.source.ecologicalYear, trees: JSON.stringify(p.view.areas.map((a) => a.trees)) };`));
    const status0 = await page.eval(`document.getElementById('prune-lifts').textContent + ' | ' + document.getElementById('prune-status').textContent`);
    check(status0.includes('Pruning lifts 0 / 3') && status0.includes('Eligible now') && status0.includes('2.5 m'), 'the inspector shows lifts X / 3 and the engine eligibility', status0);
    await page.click('#prune-add');
    await page.idle();
    await page.waitFor(`document.getElementById('prune-status').textContent.includes('Pending Prune')`, 'pending prune');
    const afterPending = await page.eval(apiPick(`return { trees: JSON.stringify(p.view.areas.map((a) => a.trees)), cover: p.stage4.areas[1].canopy.geometricCoverFraction,
      light: p.stage4.areas[1].canopy.meanLightIndex, pending: p.stage4.pendingPrunes };`));
    check(afterPending.trees === before.trees && afterPending.cover === before.cover && afterPending.light === before.light && afterPending.pending.length === 1,
      'queuing Prune creates Pending only: no crown, cover or light change', afterPending.pending);
    await page.click('[data-pane-button="plan"]');
    const planPending = await page.eval(`document.getElementById('pending-areas').textContent`);
    check(planPending.includes('✂ Pending Prune') && planPending.includes(pTree), 'the Work Plan lists "✂ Pending Prune" with icon and text', planPending.slice(0, 200));
    await shot(page, 'c0-prune-pending-1920x1080.png');
    await page.viewport(1280, 720);
    await page.click('#zoom-fit');
    await shot(page, 'c0-prune-pending-1280x720.png');
    await page.viewport(1920, 1080);
    await page.click('#approve');
    await page.idle();
    await page.waitFor(`!document.getElementById('approved-box').hidden`, 'approved');
    const afterApprove = await page.eval(apiPick(`return { trees: JSON.stringify(p.view.areas.map((a) => a.trees)), cover: p.stage4.areas[1].canopy.geometricCoverFraction,
      approved: p.stage4.approvedPrunes.length, year: p.view.source.ecologicalYear };`));
    check(afterApprove.trees === before.trees && afterApprove.cover === before.cover && afterApprove.approved === 1, 'Approve changes no ecology', afterApprove.approved);
    const planApproved = await page.eval(`document.getElementById('approved-areas').textContent`);
    check(planApproved.includes('✂ Approved Prune') && planApproved.includes(pTree), 'the Work Plan lists "✂ Approved Prune"', planApproved.slice(0, 200));
    await shot(page, 'c-prune-workplan-1920x1080.png');
    await page.viewport(1280, 720);
    await page.click('#zoom-fit');
    await shot(page, 'c-prune-workplan-1280x720.png');
    await page.viewport(1920, 1080);
    await page.click('#execute');
    await page.waitFor(`document.getElementById('execute-dialog').open`, 'execute confirmation');
    const ptext = await page.eval(`document.getElementById('execute-text').textContent + ' | ' + document.getElementById('execute-confirm').textContent`);
    check(ptext.includes('Prunes exactly 1 named tree') && ptext.includes('Confirm pruning'), 'Execute asks for confirmation of the Prune', ptext);
    await page.click('#execute-confirm');
    await page.idle();
    await page.waitFor(`document.getElementById('approved-box').hidden`, 'prune executed');
    const after = await page.eval(apiPick(`const t = p.view.areas[1].trees.find((x) => x.treeId === ${JSON.stringify(pTree)});
      const s = p.stage4.areas[1].trees.find((x) => x.treeId === ${JSON.stringify(pTree)});
      return { r: t.crownRadiusM, cover: p.stage4.areas[1].canopy.geometricCoverFraction, light: p.stage4.areas[1].canopy.meanLightIndex,
        year: p.view.source.ecologicalYear, s, log: p.stage4.pruneLog };`));
    facts.prune = { tree: pTree, before: { r: before.r, cover: before.cover, light: before.light }, after: { r: after.r, cover: after.cover, light: after.light, year: after.year }, row: after.s };
    check(Math.abs(after.r - Math.max(0.1, before.r * 0.92)) < 1e-5 && after.year === before.year, 'Execute: radius × 0.92 immediately, year unchanged',
      { before: before.r, after: after.r, year: after.year });
    check(after.cover < before.cover && after.light > before.light, 'canopy cover and modelled light read-outs updated after the Prune',
      { cover: [before.cover, after.cover], light: [before.light, after.light] });
    const receipt = await page.eval(`document.getElementById('receipt').textContent`);
    check(receipt.includes('Pruned in this Execute') && receipt.includes(pTree), 'the receipt lists the engine prune record', receipt.slice(0, 160));
    await page.click('[data-pane-button="tree"]');
    const status1 = await page.eval(`({ text: document.getElementById('prune-status').textContent, why: document.getElementById('prune-why').textContent,
      disabled: document.getElementById('prune-add').disabled, lifts: document.getElementById('prune-lifts').textContent })`);
    check(status1.lifts.includes('1 / 3') && status1.disabled && status1.text.includes('five years') && status1.text.includes('year 5') &&
      status1.why.includes('Not eligible (engine)'), 'the 5-year guard then reports ineligible; Add is disabled with the engine reason', status1);
    // The deliberate engine-refusal probe runs at the very end, so its expected 409 network log line is kept apart.

    // ---- S3 Fell 10+ trees in Area A (incl. A trees shading the B border cell); cover, open ground and local light ------
    await showArea(page, 'MA-B');
    await clickCell(page, plan.borderKey);
    const gb0 = await page.eval(groundText);
    await showArea(page, 'MA-A');
    await page.waitFor(`document.getElementById('insp-id').textContent === ${JSON.stringify(r1)}`, 'remembered A reference');
    const cardsBefore = await page.eval(READOUTS);
    await clickCell(page, plan.cellKey);
    const ga0 = await page.eval(groundText);
    await page.waitFor(`document.querySelectorAll('#competition-content [data-draft]').length > 0`, 'competitors again');
    const more = await page.eval(`[...document.querySelectorAll('#competition-content [data-draft]:not(:disabled)')].filter((e) => !e.checked).map((e) => e.getAttribute('data-draft'))`);
    for (const id of more) await page.click(`[data-draft="${id}"]`);
    await page.click('#add-button');
    await page.idle();
    await page.waitFor(`!document.getElementById('batch-result').hidden`, 'batch result');
    await page.click('#add-reference');
    await page.idle();
    // The A trees that shade the B border cell first (cross-rack check), then more shaders of the inspected A cell.
    const extra = plan.borderA.concat(plan.contributors.slice(1));
    let pendingCount = await page.eval(`(async () => (await (await fetch('api/multi/view')).json()).view.pending.length)()`);
    for (const id of extra) {
      if (pendingCount >= 16) break;
      const state = await page.eval(apiPick(`const t = p.view.areas[0].trees.find((x) => x.treeId === ${JSON.stringify(id)});
        return t && t.isLiving && !p.view.pending.some((o) => o.treeId === t.treeId) && !p.view.areas[0].cropTreeIds.includes(t.treeId);`));
      if (!state) continue;
      await selectTree(page, id);
      await page.click('[data-pane-button="tree"]');
      await page.click('#add-reference');
      await page.idle();
      pendingCount = await page.eval(`(async () => (await (await fetch('api/multi/view')).json()).view.pending.length)()`);
    }
    facts.fellCount = pendingCount;
    check(pendingCount >= 10, 'ten or more trees queued for Fell through the page (multi-tree ticked flow plus single adds)', pendingCount);
    const pendingSections = await page.eval(`[...document.querySelectorAll('#pending-areas .plan-area h4')].map((h) => h.textContent)`);
    check(pendingSections.some((t) => t.includes('Area B') && t.includes('0 pending')) || pendingSections.length === 2, 'the Work Plan stays per area', pendingSections);
    const fellText = await addAndExecute(page, 'Fells exactly ' + pendingCount + ' named trees');
    facts.fellConfirmation = fellText;
    const cardsAfter = await page.eval(READOUTS);
    facts.cardsBeforeFell = cardsBefore; facts.cardsAfterFell = cardsAfter;
    check(coverOf(cardsAfter[0]) < coverOf(cardsBefore[0]) && openOf(cardsAfter[0]) > openOf(cardsBefore[0]),
      'Area A cover drops and open ground rises after the multi-tree Fell', { before: cardsBefore[0], after: cardsAfter[0] });
    await page.click('[data-pane-button="tree"]');
    await clickCell(page, plan.cellKey);
    const ga1 = await page.eval(groundText);
    const li = (g) => num(g.facts, /Modelled Light Index \(0–1\)([0-9.]+)/);
    facts.localLight = { before: li(ga0), after: li(ga1), cell: plan.cellKey };
    check(li(ga1) > li(ga0) && ga1.caveat.includes('NOT measured or calibrated PAR') && ga1.label.includes('not calibrated PAR'),
      'the local light index lifts at the inspected cell; caveat visible', facts.localLight);
    await showArea(page, 'MA-B');
    await clickCell(page, plan.borderKey);
    const gb1 = await page.eval(groundText);
    facts.borderLight = { before: li(gb0), after: li(gb1), cell: plan.borderKey, felledShaders: plan.borderA };
    check(li(gb1) > li(gb0), 'cross-rack: felling Area A trees lifts modelled light in an Area B border cell (connected woodland)', facts.borderLight);
    const cardsB = await page.eval(READOUTS);
    check(/0 this year/.test(cardsB[1]), 'rack summaries stay per area (Area B counts only its own deaths)', cardsB[1]);
    await showArea(page, 'MA-A');

    // ---- S4 Years: gradual re-closure, natural deaths, fallen-log markers ---------------------------------------------
    await page.click('#light-toggle');
    const yearly = [];
    for (let y = 0; y < 6; y++) {
      const year0 = await page.eval(`document.getElementById('year-value').textContent`);
      await page.click('#advance');
      await sleep(150);
      if (await page.eval(`document.getElementById('consent-dialog').open`)) await page.click('#consent-advance');
      await page.idle();
      await page.waitFor(`document.getElementById('year-value').textContent !== ${JSON.stringify(year0)}`, 'year advanced');
      const cards = await page.eval(READOUTS);
      const logs = await page.eval(`document.querySelectorAll('#deadwood .log').length`);
      const api = await page.eval(apiPick(`const a = p.stage4.areas[0]; const cell = a.ground.find((c) => c.column + ':' + c.row === ${JSON.stringify(plan.cellKey)});
        const t = p.view.areas[0].trees.filter((x) => x.isLiving && Math.hypot(x.xm - cell.centreXM, x.zm - cell.centreZM) <= 6);
        return { year: p.view.source.ecologicalYear, coverA: a.canopy.geometricCoverFraction,
        openA: a.canopy.geometricUncoveredM2, deathsA: a.canopy.annualNaturalDeaths, largestOpeningA: a.canopy.largestOpeningAreaM2,
        logsA: a.canopy.fallenDeadwoodCount, deathsB: p.stage4.areas[1].canopy.cumulativeNaturalDeaths,
        gapCellCover: cell.geometricCoveredFraction, gapCellLight: cell.lightIndex,
        gapEdgeMeanRadius: t.reduce((s, x) => s + x.crownRadiusM, 0) / Math.max(1, t.length), gapEdgeTrees: t.length };`));
      yearly.push({ ...api, card: cards[0], drawnLogs: logs });
    }
    facts.yearly = yearly;
    console.log('YEARS ' + JSON.stringify(yearly.map((y) => [y.year, +y.coverA.toFixed(4), +y.openA.toFixed(1), y.deathsA, y.logsA, y.drawnLogs,
      +y.gapCellCover.toFixed(3), +y.gapCellLight.toFixed(3), +y.gapEdgeMeanRadius.toFixed(3), y.gapEdgeTrees])));
    // Local re-closure of the felled gap: the crowns around it widen and the gap cell's cover rises / light falls year on
    // year. The area-level cover is reported as observed (natural deaths elsewhere also open the canopy).
    check(yearly.every((y, i) => i === 0 || (y.gapCellCover >= yearly[i - 1].gapCellCover && y.gapCellLight <= yearly[i - 1].gapCellLight + 1e-6)) &&
      yearly[yearly.length - 1].gapCellCover > yearly[0].gapCellCover,
      'the felled gap re-closes gradually (gap-cell cover rises, its light falls, year on year)',
      yearly.map((y) => [y.year, y.gapCellCover.toFixed(3), y.gapCellLight.toFixed(3), y.gapEdgeMeanRadius.toFixed(3)]));
    console.log('INFO Area A cover by year (engine): ' + yearly.map((y) => y.year + ':' + y.coverA.toFixed(4)).join(' ') +
      ' — natural deaths ' + yearly.map((y) => y.deathsA).join('/'));
    const last = yearly[yearly.length - 1];
    check(last.logsA > 0 && last.drawnLogs === last.logsA, 'natural deaths produce fallen-log markers, one per deadwood record', { logs: last.logsA, drawn: last.drawnLogs });
    check(yearly.every((y) => y.card.includes(y.deathsA + ' this year') && y.card.includes(y.logsA + (y.logsA === 1 ? ' log' : ' logs') + ' total')),
      'the rack summary counts natural deaths this year and total logs', yearly.map((y) => [y.deathsA, y.logsA]));
    const legendNames = await page.eval(`[...document.querySelectorAll('#legend .legend-name')].map((n) => n.textContent)`);
    check(['Naturally dead · D', 'Felled tree · X', 'Fallen deadwood · log', 'Pending Prune · ✂', 'Approved Prune · ✂'].every((n) => legendNames.includes(n)),
      'legend entries for standing dead (D), stumps (X), fallen logs and Prune states', legendNames);
    await page.click('#zoom-fit');
    await shot(page, 'd-thinning-gaps-deadwood-1920x1080.png');
    await page.viewport(1280, 720);
    await page.click('#zoom-fit');
    await shot(page, 'd-thinning-gaps-deadwood-1280x720.png');
    await layoutAll(page, 'S4');

    // ---- console / CSP / network -------------------------------------------------------------------------------------
    const csp = await page.eval(`window.__csp`);
    facts.console = page.log;
    check(page.log.console.filter((l) => l.startsWith('error')).length === 0 && page.log.exceptions.length === 0 &&
      page.log.logEntries.filter((l) => l.startsWith('error')).length === 0, '0 console errors / exceptions', page.log);
    check(csp.length === 0 && !page.log.logEntries.some((l) => /Content Security Policy|Refused to/i.test(l)), '0 CSP violations', csp);
    check(page.log.http5xx.length === 0, '0 server 5xx responses', { statuses: page.log.statuses, requests: page.log.requests });

    // Deliberate engine refusal through the browser's own network stack (an expected 409, logged by the browser).
    const logBefore = page.log.logEntries.length;
    // The probe tree is one the engine's read-out currently marks ineligible (a felled stump of this run).
    const refusal = await page.eval(`(async () => { const p = await (await fetch('api/multi/view')).json(); const v = p.view;
      const t = p.stage4.areas[0].trees.find((x) => !x.pruneEligible && x.lifeState === 'FelledStump');
      const r = await fetch('api/multi/prune/add', { method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ fence: { sessionId: v.sessionId, source: v.source, planStateRevision: v.planStateRevision }, areaId: 'MA-A', treeId: t.treeId }) });
      const j = await r.json(); return { tree: t.treeId, readoutCode: t.pruneRefusalCode, readoutReason: t.pruneRefusalReason, status: r.status,
        code: j.refusal && j.refusal.code, message: j.refusal && j.refusal.message, sameRevision: j.view.planStateRevision === v.planStateRevision }; })()`);
    await sleep(300);
    facts.refusalProbe = { ...refusal, browserLogLinesAdded: page.log.logEntries.slice(logBefore) };
    // Codes must match; the wording may differ (the engine's command guard and PruningRules carry two texts for one code).
    check(refusal.status === 409 && refusal.code === refusal.readoutCode && Boolean(refusal.message) && refusal.sameRevision,
      'an ineligible tree is refused by the engine with the refusal code the read-out showed (HTTP 409, nothing changed)', facts.refusalProbe);
  } finally {
    await page.close();
  }
  facts.shots = shots;
  fs.writeFileSync(path.join(OUT, 'browser-results.json'), JSON.stringify({ results, facts }, null, 1).replace(/\/s\/[0-9a-f]{64}\//g, '/s/<capability>/'));
  console.log('OVERALL ' + (failures === 0 ? 'PASS' : 'FAIL') + ' (' + results.filter((r) => r.startsWith('PASS')).length + ' passed, ' + failures + ' failed)');
  process.exit(failures === 0 ? 0 : 1);
}

main().catch((e) => { console.log('ERROR ' + redact(e.stack || e)); process.exit(2); });
