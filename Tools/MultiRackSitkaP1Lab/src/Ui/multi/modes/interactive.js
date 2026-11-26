// S0c provenance: modes/interactive.js [L4] — section(s) 10 from multi.js
import { announce, applyPayload, call, clearAlert, currentFence, rereadButton, showAlert, showGuardRefusal, showLockAlert, showRefusal } from '../api.js';
import { el, fixed, lightIndexText, pair, plural, svg, table } from '../dom.js';
import { showPane } from '../panels/work-plan.js';
import { SLATE, approvedClears, approvedPlantings, areaName, byId, compactCellsFor, currentJuvenile, hooks, lightPaint, lightWord, matchingShelterOrder, pct, pct1, pendingClears, pendingPlantings, s4Area, shelterForJuvenile, speciesLabel, state } from '../store.js';
  // ---- Interactive modes: Inspect Cell, Plant, Spot Clear, and PJ Sapling inspection ----------------------------------

  export function setMapMode(mode, moveFocus) {
    if (!['tree', 'cell', 'plant', 'spot'].includes(mode)) return;
    if (state.mapMode === mode) return;
    state.mapMode = mode;
    state.cellPick += 1;
    state.cellLoading = false;
    if (mode !== 'plant') state.plantDraft = null;
    if (mode !== 'spot') state.spotDraft = null;
    showPane('tree');
    byId('pane-tree').scrollTop = 0;
    hooks.renderAll();
    if (moveFocus) {
      byId('mode-' + mode).focus();
    }
    const msgs = {
      tree: 'Select Tree mode active: clicking a crown or planted sapling inspects it. Draft Fell ticks preserved.',
      cell: 'Inspect Cell mode active: click any 2.5 m cell inside the area to inspect or queue ClearUndergrowth. Draft Fell ticks preserved; press Escape to return.',
      plant: 'Plant mode active (snap OFF): click any point on the map for a provisional Draft preview, then Queue to Work Plan. Draft Fell ticks preserved; press Escape to return.',
      spot: 'Spot Clear mode active (snap OFF): click any point on the map for a provisional seedling-free 0.564 m preview, then Queue Spot Clear. Draft Fell ticks preserved; press Escape to return.',
    };
    announce(msgs[mode]);
  }

  export function setOverlay(value) {
    if (!['off', 'light', 'undergrowth', 'seedlings'].includes(value)) return;
    state.overlay = value;
    hooks.renderAll();
    const names = { off: 'Off', light: 'Light Guide (Modelled light · not calibrated PAR)',
      undergrowth: 'Undergrowth (bramble and bracken hatches)',
      seedlings: 'Natural Seedlings (cell-level presence only)' };
    announce('Ground overlay: ' + names[value] + '.');
  }

  function insidePolygon(points, u, v) {
    let inside = false;
    for (let i = 0, j = points.length - 1; i < points.length; j = i++) {
      const a = points[i], b = points[j];
      if ((a.v > v) !== (b.v > v) && u < (b.u - a.u) * (v - a.v) / (b.v - a.v) + a.u) inside = !inside;
    }
    return inside;
  }

  export async function inspectCellAtPoint(areaId, u, v) {
    const s4 = s4Area(areaId);
    const cell = s4 ? s4.ground.find((c) => insidePolygon(c.polygon, u, v)) || null : null;
    if (!cell) {
      state.cellDetail = { areaId, missing: true };
      state.cellLoading = false;
      showPane('tree');
      hooks.renderAll();
      byId('cell-heading').focus();
      announce('No modelled 2.5 m cell at that point; nothing is estimated.');
      return;
    }
    const pick = ++state.cellPick;
    state.cellLoading = true;
    renderCellInspector();
    const outcome = await call('api/multi/cell', { fence: currentFence(), areaId, column: cell.column, row: cell.row });
    if (pick !== state.cellPick || state.mapMode !== 'cell') return;
    state.cellLoading = false;
    if (outcome.kind === 'ok') {
      clearAlert();
      state.cellDetail = outcome.payload.cell;
      applyPayload(outcome.payload, false);
      byId('cell-heading').focus();
      const d = outcome.payload.cell;
      announce('Inspected cell (' + d.column + ', ' + d.row + ') in ' + areaName(d.areaId) + ': light ' + lightIndexText(d.lightIndex) +
        ', cohorts ' + d.cohorts.length + '. Read-only; draft Fell ticks unchanged.');
    } else if (outcome.kind === 'refused') {
      state.cellDetail = null;
      applyPayload(outcome.payload, false);
      if (state.lock) showLockAlert('Refused: Inspect cell (' + outcome.payload.refusal.code + ').');
      else showRefusal('Inspect cell', outcome.payload.refusal);
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Inspect cell', outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: Inspect cell.');
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'Cell inspection could not reach the local session; nothing changed.'), rereadButton()]);
    }
  }

  export function renderCellInspector() {
    if (state.mapMode !== 'cell') return;
    const actionsBox = byId('cell-actions');
    const summaryBox = byId('cell-summary');
    const cohortsBox = byId('cell-cohorts');
    const treesBox = byId('cell-trees');
    const techBox = byId('cell-tech');
    if (state.cellLoading) {
      byId('cell-heading').textContent = 'Reading cell detail…';
      actionsBox.hidden = true;
      summaryBox.replaceChildren(el('p', { class: 'small-note' }, 'Querying server cell state (read-only)…'));
      cohortsBox.replaceChildren();
      treesBox.hidden = true;
      techBox.hidden = true;
      return;
    }
    const c = state.cellDetail && state.cellDetail.areaId === state.areaId ? state.cellDetail : null;
    if (!c) {
      byId('cell-heading').textContent = areaName(state.areaId) + ' · choose a 2.5 m cell';
      actionsBox.hidden = true;
      summaryBox.replaceChildren(
        el('p', null, 'Click any spot inside ' + areaName(state.areaId) + ' to fetch cell detail via POST api/multi/cell and optionally queue ClearUndergrowth.'),
        el('p', { class: 'small-note' }, 'Inspecting is read-only: draft Fell ticks and Work Plan are unchanged. Press Escape or Exit cell mode to return to Select Tree.'));
      cohortsBox.replaceChildren();
      treesBox.hidden = true;
      techBox.hidden = true;
      return;
    }
    if (c.missing) {
      byId('cell-heading').textContent = areaName(state.areaId) + ' · outside cells';
      actionsBox.hidden = true;
      summaryBox.replaceChildren(el('p', null, 'No in-polygon 2.5 m cell at that point. No value is shown or estimated.'));
      cohortsBox.replaceChildren();
      treesBox.hidden = true;
      techBox.hidden = true;
      return;
    }

    const hasPendingClear = pendingClears().some((o) => o.areaId === c.areaId && o.column === c.column && o.row === c.row);
    const hasApprovedClear = approvedClears().some((o) => o.areaId === c.areaId && o.column === c.column && o.row === c.row);
    const planLocked = Boolean(state.view && state.view.approved);
    actionsBox.hidden = false;
    byId('cell-clear-add').hidden = hasPendingClear || hasApprovedClear;
    byId('cell-clear-remove').hidden = !hasPendingClear;
    byId('cell-clear-status').textContent = hasPendingClear
      ? 'Pending ClearUndergrowth in Work Plan (zero undergrowth cleared yet).'
      : hasApprovedClear
        ? 'Approved ClearUndergrowth awaiting Execute (cancel the approved plan to edit).'
        : planLocked
          ? 'The plan is approved, so editing is locked. Cancel the approved plan to change it.'
          : 'Whole 2.5 m cell selective clear (bramble & bracken to 0 on Execute; spares seedlings).';

    byId('cell-heading').textContent = areaName(c.areaId) + ' · cell (' + c.column + ', ' + c.row + ')';
    const paint = lightPaint(c.lightIndex);
    const swatch = svg('svg', { class: 'ground-swatch', viewBox: '0 0 10 10', 'aria-hidden': 'true', focusable: 'false' });
    swatch.append(svg('rect', { width: 10, height: 10, fill: 'rgb(' + SLATE.join(',') + ')' }));
    if (paint) swatch.append(svg('rect', { width: 10, height: 10, fill: paint.colour, 'fill-opacity': paint.opacity }));
    const clearLabel = hasPendingClear ? 'Pending ClearUndergrowth (Whole 2.5 m cell · spares seedlings)'
      : hasApprovedClear ? 'Approved ClearUndergrowth (Whole 2.5 m cell · spares seedlings)' : 'None';

    // Sync live bramble/bracken from compact cells when available so post-Execute clearing reflects immediately.
    const compactRow = compactCellsFor(c.areaId).get(c.column + ':' + c.row);
    const liveBramble = compactRow ? compactRow[4] : c.bramble;
    const liveBracken = compactRow ? compactRow[5] : c.bracken;

    summaryBox.replaceChildren(
      el('p', { class: 'ground-level' }, swatch, lightWord(c.lightIndex)),
      el('p', { class: 'small-note' }, 'Modelled light · not calibrated PAR'),
      el('dl', null,
        el('dt', null, 'Light index'), el('dd', null, lightIndexText(c.lightIndex) + ' (0–1 modelled)'),
        el('dt', null, 'Canopy cover'), el('dd', null, pct(c.geometricCoveredFraction)),
        el('dt', null, 'Bramble cover'), el('dd', null, liveBramble === null ? 'not modelled' : pct1(liveBramble)),
        el('dt', null, 'Bracken cover'), el('dd', null, liveBracken === null ? 'not modelled' : pct1(liveBracken)),
        el('dt', null, 'Browse pressure'), el('dd', null, c.browsePressure === null ? 'not modelled' : fixed(c.browsePressure, 2)),
        el('dt', null, 'Clear state'), el('dd', null, clearLabel)));

    const cohorts = c.cohorts || [];
    if (cohorts.length === 0) {
      cohortsBox.replaceChildren(
        el('h3', null, 'Natural regeneration cohorts (0)'),
        el('p', { class: 'small-note' }, 'No natural regeneration bands in this cell yet. Density is abstract occupancy, never a literal stem count.'));
    } else {
      cohortsBox.replaceChildren(
        el('h3', null, 'Natural regeneration cohorts (' + cohorts.length + ')'),
        el('p', { class: 'small-note' }, 'Cell-level cohorts — Density is abstract occupancy, not a literal stem count.'),
        table('Cohorts in cell (' + c.column + ', ' + c.row + ')', ['Species', 'Est. yr', 'H (m)', 'Density', 'Browsed'],
          cohorts.map((b) => [speciesLabel(b.species), String(b.establishYear), fixed(b.heightM, 2), fixed(b.density, 3),
            b.lastBrowseAssessmentYear >= 0 ? pct1(b.lastBrowsedFraction) : '–'])));
    }

    const contributors = c.contributingTreeIds || [];
    treesBox.hidden = false;
    byId('cell-trees-summary').textContent = 'Contributing trees (' + contributors.length + ')';
    byId('cell-tree-ids').replaceChildren(...(contributors.length === 0 ? ['none (no living crown shades this cell centre)'] :
      contributors.flatMap((id, i) => [i > 0 ? ', ' : null, el('span', { class: 'id' }, id)].filter((part) => part !== null))));

    techBox.hidden = false;
    byId('cell-facts').replaceChildren(
      pair('Modelled light index', fixed(c.lightIndex, 4) + ' (0–1)'),
      pair('Overhead cover fraction', fixed(c.geometricCoveredFraction, 4)),
      pair('Clipped cell area', fixed(c.clippedAreaM2, 2) + ' m²'),
      pair('Cell centre', 'X ' + fixed(c.centreXM, 2) + ' m east, Z ' + fixed(c.centreZM, 2) + ' m north'),
      pair('Cross-border trees', String(c.crossBorderTrees)),
      pair('Polygon vertices', String((c.polygon || []).length)));
    byId('cell-caveat').textContent = 'Modelled light · not calibrated PAR. Inspecting changes nothing; ClearUndergrowth commits only on Execute.';
  }

  export function renderPlantDrawer() {
    if (state.mapMode !== 'plant') return;
    const d = state.plantDraft && state.plantDraft.areaId === state.areaId ? state.plantDraft : null;
    const planLocked = Boolean(state.view && state.view.approved);
    byId('plant-heading').textContent = d ? areaName(d.areaId) + ' · provisional planting point' : areaName(state.areaId) + ' · place a planting point';
    byId('plant-coords').textContent = d
      ? 'Provisional Draft (snap OFF · zero order queued): X ' + fixed(d.xM, 2) + ' m east, Z ' + fixed(d.zM, 2) + ' m north'
      : 'Click anywhere on the map to place a provisional Draft preview (snap OFF; zero order sent yet).';
    byId('plant-clear-draft').hidden = !d;
    byId('plant-note').textContent = planLocked
      ? 'The plan is approved, so editing is locked. Cancel the approved plan before queueing new planting orders.'
      : 'Queues Pending orders sequentially (Plant → Shelter → SpotClear) using each response’s fresh fence. Zero ecology change until Approved and Executed.';

    const box = byId('plant-result');
    const r = state.plantResult && state.plantResult.areaId === state.areaId ? state.plantResult : null;
    box.hidden = !r;
    if (!r) { box.replaceChildren(); return; }
    const stillPending = new Set(pendingPlantings().map((o) => o.orderId));
    const hasErr = Boolean(r.refused || r.guard || r.unconfirmed);
    const partial = hasErr && r.queued.length > 0;
    box.setAttribute('class', 'drawer-result' + (partial ? ' partial' : hasErr ? ' failed' : ''));
    const parts = [];
    if (partial) {
      parts.push(el('p', { class: 'alert-title' }, 'Stopped at ' + (r.refused ? r.refused.kind : r.guard ? r.guard.kind : r.unconfirmed.kind) +
        ' — earlier pending order(s) remain queued'));
    } else if (hasErr) {
      parts.push(el('p', { class: 'alert-title' }, 'Refused: ' + (r.refused ? r.refused.kind : r.guard ? r.guard.kind : r.unconfirmed.kind) + ' (nothing queued)'));
    } else {
      parts.push(el('p', { class: 'alert-title' }, 'Queued ' + plural(r.queued.length, 'pending order', 'pending orders') + ' (not executed)'));
    }
    if (r.refused) {
      parts.push(el('p', null, el('code', null, r.refused.code), ' — ' + r.refused.message));
    }
    if (r.guard) {
      parts.push(el('p', null, el('code', null, r.guard.code), ' (HTTP ' + r.guard.status + ') — ' + r.guard.message));
    }
    if (r.unconfirmed) {
      parts.push(el('p', null, 'Unconfirmed outcome — re-read required before further commands.'));
    }
    if (r.queued.length > 0) {
      parts.push(el('p', { class: 'small-note' }, partial ? 'Already queued (one-click remove below):' : 'Already queued in Work Plan:'));
      parts.push(el('ul', { class: 'partial-list' }, ...r.queued.map((o) => {
        const active = stillPending.has(o.orderId);
        return el('li', null,
          el('span', null, el('span', { class: 'mono' }, o.orderId), ' · ' + o.kind + (o.speciesId ? ' (' + speciesLabel(o.speciesId) + ')' : '') +
            (active ? '' : ' · removed')),
          active && !planLocked ? el('button', { type: 'button', class: 'small secondary', 'data-planting-remove': o.orderId,
            'aria-label': 'Remove pending order ' + o.orderId }, 'Remove') : null);
      })));
    }
    if (r.unattempted && r.unattempted.length > 0) {
      parts.push(el('p', { class: 'small-note' }, 'Not attempted: ' + r.unattempted.join(', ') + '.'));
    }
    box.replaceChildren(...parts);
  }

  export function renderSpotDrawer() {
    if (state.mapMode !== 'spot') return;
    const d = state.spotDraft && state.spotDraft.areaId === state.areaId ? state.spotDraft : null;
    const planLocked = Boolean(state.view && state.view.approved);
    byId('spot-heading').textContent = d ? areaName(d.areaId) + ' · provisional Spot Clear' : areaName(state.areaId) + ' · place a ~1 m² spot clear';
    byId('spot-coords').textContent = d
      ? 'Provisional Draft (snap OFF · seedling-free 0.564 m ring · zero order queued): X ' + fixed(d.xM, 2) + ' m east, Z ' + fixed(d.zM, 2) + ' m north'
      : 'Click anywhere on the map to place a provisional seedling-free 0.564 m circular preview (snap OFF; zero order sent yet).';
    byId('spot-clear-draft').hidden = !d;
    byId('spot-note').textContent = planLocked
      ? 'The plan is approved, so editing is locked. Cancel the approved plan before queueing new Spot Clear orders.'
      : 'Selective ~1 m² circle (r ≈ 0.564 m, spares natural seedlings). Queues Pending order only; Approve and Execute are separate.';

    const box = byId('spot-result');
    const r = state.spotResult && state.spotResult.areaId === state.areaId ? state.spotResult : null;
    box.hidden = !r;
    if (!r) { box.replaceChildren(); return; }
    const stillPending = new Set(pendingPlantings().map((o) => o.orderId));
    const active = r.order && stillPending.has(r.order.orderId);
    box.setAttribute('class', 'drawer-result' + (r.refused ? ' failed' : ''));
    if (r.refused) {
      box.replaceChildren(
        el('p', { class: 'alert-title' }, 'Refused: Spot Clear'),
        el('p', null, el('code', null, r.refused.code), ' — ' + r.refused.message));
    } else if (r.order) {
      box.replaceChildren(
        el('p', { class: 'alert-title' }, 'Queued pending SpotClear (not executed)'),
        el('ul', { class: 'partial-list' }, el('li', null,
          el('span', null, el('span', { class: 'mono' }, r.order.orderId), ' · X ' + fixed(r.order.xm, 2) + ', Z ' + fixed(r.order.zm, 2) +
            (active ? '' : ' · removed')),
          active && !planLocked ? el('button', { type: 'button', class: 'small secondary', 'data-planting-remove': r.order.orderId,
            'aria-label': 'Remove pending order ' + r.order.orderId }, 'Remove') : null)));
    }
  }

  export function renderJuvenileInspector() {
    const j = currentJuvenile();
    if (state.mapMode !== 'tree' || !j) return;
    const spName = speciesLabel(j.speciesId);
    byId('pj-area').textContent = areaName(j.areaId);
    byId('pj-id').textContent = j.juvenileId;
    const statusWord = j.isActive ? 'Living planted sapling' : j.promotedTreeId ? 'Promoted to adult ' + j.promotedTreeId : 'Dead (' + (j.deathCause || 'unknown') + ', year ' + j.deathYear + ')';
    byId('pj-summary').textContent = spName + ' · ' + statusWord;
    const facts = [
      pair('Species', spName + ' (' + j.speciesId + ')'),
      pair('Height', fixed(j.heightM, 2) + ' m'),
      pair('Age', fixed(j.ageYears, 1) + ' years'),
      pair('Planting year', String(j.plantingYear)),
      pair('Position', 'X ' + fixed(j.xm, 2) + ' m east, Z ' + fixed(j.zm, 2) + ' m north', 'wide'),
    ];
    if (j.lastAssessmentYear >= 0) {
      facts.push(pair('Last browse', (j.lastYearBrowsed ? 'Browsed' : 'Unbrowsed') + ' (p ' + pct1(j.lastBrowseProbability) + ', yr ' + j.lastAssessmentYear + ')', 'wide'));
      facts.push(pair('Veg loss prob.', pct1(j.lastVegetationLossProbability)));
    }
    byId('pj-facts').replaceChildren(...facts);

    const shelter = shelterForJuvenile(j);
    const hasActiveShelter = Boolean(shelter && shelter.isEffective);
    const pendingShelter = matchingShelterOrder(pendingPlantings(), j);
    const approvedShelter = matchingShelterOrder(approvedPlantings(), j);
    const planLocked = Boolean(state.view && state.view.approved);
    const addBtn = byId('pj-shelter-add');
    const remBtn = byId('pj-shelter-remove');
    if (hasActiveShelter) {
      byId('pj-shelter-state').textContent = 'Active · ' + shelter.shelterId;
      byId('pj-shelter-detail').textContent = 'Active 8-year shelter protecting ' + j.juvenileId + ' (installed year ' + shelter.installedYear + ', expires year ' + shelter.expiryYear + ').';
      addBtn.hidden = true;
      remBtn.hidden = true;
      byId('pj-shelter-note').textContent = 'Protected from browsing while effective (strictly before year ' + shelter.expiryYear + ').';
    } else if (!j.isActive) {
      byId('pj-shelter-state').textContent = 'Not eligible';
      byId('pj-shelter-detail').textContent = 'Only a living unpromoted planted sapling (PJ) can receive a shelter.';
      addBtn.hidden = true;
      remBtn.hidden = true;
      byId('pj-shelter-note').textContent = '';
    } else if (pendingShelter) {
      byId('pj-shelter-state').textContent = 'Pending · ' + pendingShelter.orderId;
      byId('pj-shelter-detail').textContent = '8-year shelter queued in Work Plan (not installed until Approved and Executed).';
      addBtn.hidden = true;
      remBtn.hidden = false;
      remBtn.setAttribute('data-pj-remove-order', pendingShelter.orderId);
      byId('pj-shelter-note').textContent = 'Queues Pending order only; zero ecology change until Execute.';
    } else if (approvedShelter) {
      byId('pj-shelter-state').textContent = 'Approved · ' + approvedShelter.orderId;
      byId('pj-shelter-detail').textContent = '8-year shelter approved and awaiting Execute.';
      addBtn.hidden = true;
      remBtn.hidden = true;
      byId('pj-shelter-note').textContent = 'Cancel the approved plan to change it.';
    } else {
      byId('pj-shelter-state').textContent = shelter ? 'Expired (' + shelter.shelterId + ')' : 'Unsheltered';
      byId('pj-shelter-detail').textContent = shelter
        ? 'Previous shelter expired in year ' + shelter.expiryYear + ' (map state only). This living sapling has no active shelter.'
        : 'This living sapling has no active browse shelter.';
      addBtn.hidden = false;
      remBtn.hidden = true;
      byId('pj-shelter-note').textContent = planLocked
        ? 'The plan is approved, so editing is locked. Cancel the approved plan to queue a shelter.'
        : 'Queues an 8-year shelter Pending order via POST api/multi/shelter/add. Zero ecology change until Execute.';
    }
  }

