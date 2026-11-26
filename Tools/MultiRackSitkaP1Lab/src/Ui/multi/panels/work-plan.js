// S0c provenance: panels/work-plan.js [L4] — section(s) 14 from multi.js
import { el, expandable, fixed, label, pair, plural, table, technicalPlan, useIcon } from '../dom.js';
import { svgRenderer, usableCentre, viewBox } from '../map/renderer-svg.js';
import { PANES, PANE_NAMES, STATEMENTS, ZOOMS, approvedClears, approvedPlantings, approvedPrunes, areaName, areaShort, byId, currentArea, currentJuvenile, hooks, itemOf, ordersIn, pendingClears, pendingPlantings, pendingPrunes, plantedIn, pruneStatusOf, readoutState, selectedId, shelterForJuvenile, speciesLabel, speciesOf, state, statusText } from '../store.js';
  // ---- Work Plan, Receipt, Results, Trees, Details --------------------------------------------------------------------

  function statePill(approved, actionIconId, actionWord) {
    return el('span', { class: 'wp-pill ' + (approved ? 'wp-approved' : 'wp-pending') },
      useIcon(actionIconId, 15),
      approved ? ('✓ Approved ' + actionWord) : ('◷ Pending ' + actionWord));
  }

  export function orderList(orders, approved, removable) {
    return el('ul', { class: 'list orders ' + (approved ? 'approved-orders' : 'pending-orders') }, ...orders.map((order) => el('li', null,
      el('span', { class: 'order-head' }, statePill(approved, 'ccf-action-fell', 'Fell'),
        el('span', { class: 'mono' }, order.treeId)),
      el('span', { class: 'order-reason' }, order.reason),
      removable ? el('button', { type: 'button', class: 'small', 'data-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'remove:' + order.treeId, 'aria-label': label('MultiRemoveFromPlan') + ': ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
  }

  export function pruneList(orders, approved, removable) {
    return el('ul', { class: 'list orders prune-orders' }, ...orders.map((order) => el('li', null,
      el('span', { class: 'order-head' }, statePill(approved, 'ccf-action-prune', 'Prune'),
        el('span', { class: 'mono' }, order.treeId)),
      el('span', { class: 'order-reason' }, 'one lift · crown base to ' + fixed(order.targetCrownBaseHeightM, 1) + ' m' +
        (approved ? ' · cut on Execute' : ' · not cut')),
      removable ? el('button', { type: 'button', class: 'small', 'data-prune-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'prune-remove:' + order.treeId, 'aria-label': 'Remove the pending Prune of ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
  }

  // ClearUndergrowth / Plant / Shelter / SpotClear Work Plan rows, with targeted remove buttons before Approve.
  export function ecoOrderList(clears, plantings, approved, removable) {
    if (clears.length === 0 && plantings.length === 0) return null;
    const rows = [];
    for (const c of clears) {
      rows.push(el('li', null,
        el('span', { class: 'order-head' }, statePill(approved, 'ccf-action-clear', 'ClearUndergrowth'),
          el('span', { class: 'mono' }, 'Cell (' + c.column + ', ' + c.row + ')')),
        el('span', { class: 'order-reason' }, 'Whole 2.5 m cell · spares seedlings' + (approved ? ' · cleared on Execute' : ' · pending')),
        removable ? el('button', { type: 'button', class: 'small', 'data-clear-remove': c.column + ':' + c.row,
          'data-clear-col': String(c.column), 'data-clear-row': String(c.row), 'data-area': c.areaId,
          'data-focus-key': 'clear-remove:' + c.areaId + ':' + c.column + ':' + c.row,
          'aria-label': 'Remove pending ClearUndergrowth for cell (' + c.column + ', ' + c.row + ')' },
        label('MultiRemoveFromPlan')) : null));
    }
    for (const p of plantings) {
      const icon = p.kind === 'Plant' ? 'ccf-action-plant' : p.kind === 'Shelter' ? 'ccf-action-shelter' : 'ccf-action-spot';
      rows.push(el('li', null,
        el('span', { class: 'order-head' }, statePill(approved, icon, p.kind),
          el('span', { class: 'mono' }, p.orderId)),
        el('span', { class: 'order-reason' }, (p.speciesId ? speciesLabel(p.speciesId) + ' · ' : '') +
          'X ' + fixed(p.xm, 2) + ', Z ' + fixed(p.zm, 2) + (approved ? ' · commits on Execute' : ' · pending')),
        removable ? el('button', { type: 'button', class: 'small', 'data-planting-remove': p.orderId,
          'data-focus-key': 'planting-remove:' + p.orderId,
          'aria-label': 'Remove pending ' + p.kind + ' order ' + p.orderId },
        label('MultiRemoveFromPlan')) : null));
    }
    return el('ul', { class: 'list orders eco-orders' }, ...rows);
  }

  export function workTotals(fells, prunes, clears, plantings) {
    const c = clears || 0;
    const pl = plantings || [];
    const nPlant = pl.filter((o) => o.kind === 'Plant').length;
    const nShelter = pl.filter((o) => o.kind === 'Shelter').length;
    const nSpot = pl.filter((o) => o.kind === 'SpotClear').length;
    return [
      fells > 0 ? fells + ' Fell' : '',
      prunes > 0 ? prunes + ' Prune' : '',
      c > 0 ? c + ' Clear' : '',
      nPlant > 0 ? nPlant + ' Plant' : '',
      nShelter > 0 ? nShelter + ' Shelter' : '',
      nSpot > 0 ? nSpot + ' SpotClear' : '',
    ].filter(Boolean).join(' · ');
  }
  function perAreaCounts(orders) {
    return state.view.areas.map((area) => area.label + ' ' + ordersIn(orders, area.areaId).length).join(' · ');
  }

  export function renderPlan() {
    const view = state.view;
    const plan = view.approved;
    const pendingP = pendingPrunes();
    const approvedP = approvedPrunes();
    const pendingC = pendingClears();
    const approvedC = approvedClears();
    const pendingPl = pendingPlantings();
    const approvedPl = approvedPlantings();
    const pendingCount = view.pending.length + pendingP.length + pendingC.length + pendingPl.length;
    byId('plan-heading').textContent = plan ? 'Approved work plan' : 'Work plan';
    byId('step-pending').className = !plan && pendingCount > 0 ? 'now' : '';
    byId('step-approved').className = plan ? 'now' : '';
    byId('plan-scope').textContent = label('MultiOnePlan') + '. ' + label('MultiLastLivingRefused') + '.';
    byId('readout-unavailable').hidden = readoutState() !== 'unavailable';
    byId('pending-box').hidden = plan !== null && pendingCount === 0;
    byId('pending-total').textContent = pendingCount === 0 ? 'No pending orders' :
      pendingP.length === 0 && pendingC.length === 0 && pendingPl.length === 0
        ? view.pending.length + ' pending Fell · not felled (' + perAreaCounts(view.pending) + ')'
        : workTotals(view.pending.length, pendingP.length, pendingC.length, pendingPl) + ' pending · nothing executed yet';
    byId('pending-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(view.pending, area.areaId);
      const prunes = ordersIn(pendingP, area.areaId);
      const clears = ordersIn(pendingC, area.areaId);
      const plants = ordersIn(pendingPl, area.areaId);
      const n = orders.length + prunes.length + clears.length + plants.length;
      return el('section', { class: 'plan-area', 'aria-label': 'Pending in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + plural(n, 'pending order', 'pending orders')),
        n === 0 ? el('p', { class: 'small-note' }, 'No pending orders in this area.') : null,
        orders.length === 0 ? null : orderList(orders, false, true),
        prunes.length === 0 ? null : pruneList(prunes, false, true),
        ecoOrderList(clears, plants, false, true));
    }));
    const approve = byId('approve');
    approve.textContent = label('MultiApprove') + ' · ' + plural(pendingCount, 'order', 'orders');
    approve.hidden = pendingCount === 0 || plan !== null;
    byId('approved-empty').hidden = plan !== null;
    byId('approved-box').hidden = plan === null;
    if (plan === null) {
      byId('approved-areas').replaceChildren();
      byId('approved-total').textContent = '';
      return;
    }
    const approvedCount = plan.orders.length + approvedP.length + approvedC.length + approvedPl.length;
    byId('approved-total').textContent = approvedP.length === 0 && approvedC.length === 0 && approvedPl.length === 0
      ? plan.orders.length + ' approved · still standing'
      : workTotals(plan.orders.length, approvedP.length, approvedC.length, approvedPl) + ' approved · nothing executed yet';
    byId('approved-sub').textContent = 'Execution has not happened. Year remains ' + view.source.ecologicalYear + ' · approved in year ' + plan.source.ecologicalYear + '.';
    byId('approved-areas').replaceChildren(...view.areas.map((area) => {
      const orders = ordersIn(plan.orders, area.areaId);
      const prunes = ordersIn(approvedP, area.areaId);
      const clears = ordersIn(approvedC, area.areaId);
      const plants = ordersIn(approvedPl, area.areaId);
      const n = orders.length + prunes.length + clears.length + plants.length;
      return el('section', { class: 'plan-area', 'aria-label': 'Approved in ' + areaName(area.areaId) },
        el('h4', null, areaName(area.areaId) + ' · ' + (n === 0 ? 'no orders' : plural(n, 'order', 'orders'))),
        orders.length === 0 ? null : orderList(orders, true, false),
        prunes.length === 0 ? null : pruneList(prunes, true, false),
        ecoOrderList(clears, plants, true, false));
    }), technicalPlan(plan.planId, plan.executionKey));
    byId('cancel-approved').textContent = label('MultiCancelApproved');
    byId('execute').textContent = readoutState() === 'unavailable' ? label('MultiExecute') + ' · re-read needed' :
      label('MultiExecute') + ' · ' + plural(approvedCount, 'order', 'orders');
  }

  export function renderReceipt() {
    const view = state.view;
    const parts = [];
    if (view.events.length === 0) {
      parts.push(el('p', { class: 'small-note' }, 'No work has been executed in this session.'));
    } else {
      const event = view.events[view.events.length - 1];
      if (state.lastWasReplay) parts.push(el('p', { class: 'warning' }, label('MultiAlreadyExecuted')));
      const seen = state.lastExecution;
      const revision = seen && seen.executionKey === event.executionKey ? seen.workRevisionBefore + ' → ' + seen.workRevisionAfter : String(event.workRevision);
      parts.push(el('p', null, 'Completed · ecological year ' + event.ecologicalYear + ' · work revision ' + revision + ' · ' +
        event.areas.reduce((sum, change) => sum + change.fells.length, 0) + ' trees felled'));
      parts.push(table('Receipt per area (actual before → actual after)', ['Area', 'Living trees', 'Basal area (m²/ha)', 'Felled (recorded reason)'],
        event.areas.map((change) => [areaShort(change.areaId), change.before.livingCount + ' → ' + change.after.livingCount,
          fixed(change.before.basalAreaM2PerHa, 2) + ' → ' + fixed(change.after.basalAreaM2PerHa, 2),
          change.fells.length === 0 ? 'none' : change.fells.map((fell) => fell.treeId + ' (' + fell.cause + '; ' + fell.reason + ')').join('; ')])));
      const pruned = state.stage4 ? state.stage4.pruneLog.filter((record) => record.executionKey === event.executionKey) : [];
      if (pruned.length > 0) {
        parts.push(table('Pruned in this Execute (engine record)', ['Tree', 'Species', 'Crown radius (m)', 'Crown base', 'Lifts'],
          pruned.map((record) => {
            const ps = pruneStatusOf(record.areaId, record.treeId);
            const maxLifts = ps ? ps.maxLifts : 3;
            return [record.treeId, speciesLabel(speciesOf(record.treeId)), fixed(record.crownRadiusBeforeM, 3) + ' → ' + fixed(record.crownRadiusAfterM, 3),
              fixed(record.targetCrownBaseHeightM, 1) + ' m', record.liftsAfter + ' / ' + maxLifts];
          })));
      }
      if (seen && seen.executionKey === event.executionKey && ((seen.clears && seen.clears.length > 0) || (seen.plantings && seen.plantings.length > 0))) {
        const ecoRows = [];
        for (const c of (seen.clears || [])) {
          ecoRows.push(['ClearUndergrowth', areaShort(c.areaId), 'Cell (' + c.column + ', ' + c.row + ')', 'Whole 2.5 m cell · spares seedlings']);
        }
        for (const p of (seen.plantings || [])) {
          ecoRows.push([p.kind, areaShort(p.areaId), p.orderId, (p.speciesId ? speciesLabel(p.speciesId) + ' · ' : '') + 'X ' + fixed(p.xm, 2) + ', Z ' + fixed(p.zm, 2)]);
        }
        parts.push(table('Clearing and planting committed in this Execute', ['Action', 'Area', 'Order / Cell', 'Detail'], ecoRows));
      }
      if (state.ecology && ((state.ecology.planted && state.ecology.planted.length > 0) ||
          (state.ecology.shelters && state.ecology.shelters.length > 0) ||
          (state.ecology.patches && state.ecology.patches.length > 0))) {
        const activePj = state.ecology.planted.filter((j) => j.isActive).length;
        const activeSh = state.ecology.shelters.filter((s) => s.isEffective).length;
        parts.push(el('p', { class: 'small-note' }, 'Committed Phase 2 objects: ' +
          plural(activePj, 'active PJ sapling', 'active PJ saplings') + ' · ' +
          plural(activeSh, 'active shelter', 'active shelters') + ' · ' +
          plural(state.ecology.patches.length, 'spot-clear patch', 'spot-clear patches') + '.'));
      }
      parts.push(technicalPlan(event.planId, event.executionKey));
    }
    byId('receipt').replaceChildren(...parts);
  }

  export function renderHistory() {
    hooks.renderSkipSummary();
    const view = state.view;
    const years = view.annualHistory;
    const truncNote = byId('history-truncated-note');
    if (truncNote) {
      truncNote.hidden = !state.historyTruncated && !state.payloadBudgetExceeded;
      const parts = [];
      if (state.historyTruncated) {
        parts.push('Compact view shows the most recent tail only; open History in the header for all ' + state.totalRecordedYears + ' recorded years.');
      }
      if (state.payloadBudgetExceeded) {
        parts.push('Notice: this view payload exceeded the 512 KiB / 150 KiB transport budget (see server log).');
      }
      truncNote.textContent = parts.join(' ');
    }
    const latest = byId('year-latest');
    if (years.length === 0) {
      const hasAdvancedYear = Boolean(view.source && view.source.ecologicalYear > 0);
      if (state.historyTruncated && hasAdvancedYear) {
        const count = state.totalRecordedYears > 0 ? state.totalRecordedYears : (view.source.ecologicalYear + 1);
        latest.replaceChildren(el('p', { class: 'small-note' },
          'Compact view omits annual history rows to stay within payload budget; open History in the header to inspect all ' +
          plural(count, 'recorded year', 'recorded years') + '.'));
      } else if (hasAdvancedYear) {
        const count = state.totalRecordedYears > 0 ? state.totalRecordedYears : (view.source.ecologicalYear + 1);
        latest.replaceChildren(el('p', { class: 'small-note' },
          'Annual history rows are not included in this view; open History in the header to inspect all ' +
          plural(count, 'recorded year', 'recorded years') + '.'));
      } else {
        latest.replaceChildren(el('p', { class: 'small-note' }, 'No ecological year has been advanced in this session.'));
      }
    } else {
      const record = years[years.length - 1];
      latest.replaceChildren(
        el('p', null, 'Latest year ' + record.yearBefore + ' → ' + record.yearAfter + (record.cancelledUnexecutedWork ?
          ' · unexecuted work in both areas was cancelled at zero Fell.' : '.')),
        table('Year ' + record.yearBefore + ' → ' + record.yearAfter + ' per area', ['Area', 'Living trees', 'Basal area (m²/ha)', 'Natural deaths'],
          record.areas.map((year) => [areaShort(year.areaId), year.before.livingCount + ' → ' + year.after.livingCount,
            fixed(year.before.basalAreaM2PerHa, 2) + ' → ' + fixed(year.after.basalAreaM2PerHa, 2),
            year.noLivingStems ? label('MultiAreaNoLivingStems') : year.naturalDeaths.length === 0 ? label('MultiNoNaturalDeaths') :
              year.naturalDeaths.map((death) => death.treeId + ' (' + speciesLabel(speciesOf(death.treeId)) + ', ' + death.mechanism + ')').join('; ')])));
    }
    const history = [];
    if (years.length > 0) {
      history.push(table('Every committed year, per area', ['Year', 'Area', 'Living trees', 'Basal area (m²/ha)', 'Natural deaths'],
        years.flatMap((record) => record.areas.map((year) => [record.yearBefore + ' → ' + record.yearAfter, areaShort(year.areaId),
          year.before.livingCount + ' → ' + year.after.livingCount,
          fixed(year.before.basalAreaM2PerHa, 2) + ' → ' + fixed(year.after.basalAreaM2PerHa, 2),
          year.noLivingStems ? 'no living stems' : String(year.naturalDeaths.length)]))));
    }
    if (view.events.length > 0) {
      history.push(el('p', null, el('span', { class: 'wp-pill wp-completed' }, '☑ Completed · History Log')));
      history.push(table('Executed work events', ['Year', 'Work revision', 'Felled per area'],
        view.events.map((event) => [String(event.ecologicalYear), String(event.workRevision),
          event.areas.map((change) => change.areaId + ': ' + (change.fells.length === 0 ? 'none' : change.fells.map((fell) => fell.treeId).join(', '))).join(' · ')])));
    }
    byId('history').replaceChildren(...history);
  }

  export function renderTable() {
    const area = currentArea();
    if (!area) return;
    const query = byId('tree-search').value;
    const rows = [];
    for (const tree of area.trees) {
      const item = itemOf(area.areaId, tree.treeId);
      if (!item || (query !== '' && !tree.treeId.includes(query))) continue;
      const selected = tree.treeId === selectedId();
      rows.push(el('tr', { class: selected ? 'current' : null, 'aria-current': selected ? 'true' : null },
        el('td', null, el('button', { type: 'button', class: 'small', 'data-select': tree.treeId, 'data-focus-key': 'select:' + tree.treeId,
          'aria-label': (selected ? 'Selected ' : 'Select ') + tree.treeId }, selected ? 'Selected' : 'Select')),
        el('th', { scope: 'row', class: 'mono' }, tree.treeId),
        el('td', null, speciesLabel(speciesOf(tree.treeId))),
        el('td', null, statusText(item)),
        el('td', null, fixed(tree.dbhCm, 1)),
        el('td', null, fixed(tree.heightM, 1)),
        el('td', null, fixed(tree.crownRadiusM, 2)),
        el('td', null, fixed(tree.xm, 2)),
        el('td', null, fixed(tree.zm, 2))));
    }
    byId('tree-rows').replaceChildren(...rows);
    byId('tree-caption').textContent = 'Every tree of ' + areaName(area.areaId) + ', with its exact ID and species';
    byId('rows-shown').textContent = rows.length + ' rows shown for ' + area.label + (query !== '' ? ' (filtered)' : '');
    const pjs = plantedIn(area.areaId);
    const pjSec = byId('planted-section');
    pjSec.hidden = pjs.length === 0;
    if (pjs.length > 0) {
      const selPj = currentJuvenile();
      byId('planted-list').replaceChildren(...pjs.map((j) => {
        const sh = shelterForJuvenile(j);
        const active = selPj && selPj.juvenileId === j.juvenileId;
        const shNote = sh && sh.isEffective ? 'active shelter ' + sh.shelterId : sh ? 'expired shelter' : 'unsheltered';
        return el('li', null,
          el('span', { class: 'order-head' }, el('span', { class: 'mono' }, j.juvenileId), ' · ' + speciesLabel(j.speciesId)),
          el('span', { class: 'order-reason' }, (j.isActive ? 'H ' + fixed(j.heightM, 2) + ' m · ' + shNote : j.promotedTreeId ? 'Promoted ' + j.promotedTreeId : 'Dead')),
          el('button', { type: 'button', class: 'small', 'data-pj-select': j.juvenileId,
            'aria-label': (active ? 'Selected ' : 'Select ') + j.juvenileId }, active ? 'Selected' : 'Select'));
      }));
    }
  }

  export function renderDetails() {
    const view = state.view;
    const e = view.experiment;
    const s = view.source;
    const readout = readoutState();
    const statements = (readout === 'phase2' || readout === 'stage4')
      ? STATEMENTS.map((name) => (name === 'MultiEdgeExcluded' ? 'MultiConnectedWoodland' : name))
      : readout === 'legacy' ? STATEMENTS : STATEMENTS.filter((name) => name !== 'MultiEdgeExcluded');
    byId('statements').replaceChildren(
      el('li', null, hooks.experimentBadgeText()),
      el('li', null, e.disclosure),
      ...(readout === 'unavailable' ? [el('li', null, 'Model read-out unavailable for this view. Choose Re-read.')] : []),
      ...statements.concat(['MultiReviewStatus', 'MultiEmptyAreaRule'])
        .map((name) => label(name)).filter((text) => text !== '').map((text) => el('li', null, text)));
    const m = state.stage4 ? state.stage4.models : null;
    const em = state.ecology ? state.ecology.models : null;
    byId('stage4-statements').replaceChildren(...(m ? [
      'Models: ' + (em ? [em.adult, em.mortality, em.crown, em.regeneration, em.species].join('/') : [m.adultModelVersion, m.mortalityModelVersion, m.crownModelVersion].join('/')) +
        ' · light ' + m.lightModelVersion + ' (' + fixed(m.lightCellM, 1) + ' m cells, shade buffer b = ' + fixed(m.shadeBufferM, 1) + ' m)',
      'Modelled light · not calibrated PAR — ' + m.notCalibratedCaveat,
      'Fallen deadwood: ' + label('MultiDeadwoodNote') + ' (direction schematic, not simulated).',
    ].concat(m.evidenceLabels, m.lightUncertainties) : []).map((text) => el('li', null, text)));
    byId('details-note').textContent = readout === 'phase2' ? label('MultiNotModelledPhase2') :
      readout === 'stage4' ? label('MultiNotModelledStage4') :
        readout === 'legacy' ? 'Light, seed, regeneration, site and money: NOT MODELLED. Saving keeps this forest in named slots on this computer.' :
          'Model read-out unavailable for this view; choose Re-read to see what the active engine models.';
    byId('facts').replaceChildren(
      pair('Experiment', e.experimentId), pair('Badge', hooks.experimentBadgeText(), 'wide'),
      pair('Format', e.format + ' v' + e.schemaVersion), pair('Domain', e.domain),
      pair('Source class', e.sourceClass), pair('Manifest content hash', expandable(e.manifestContentHash)),
      pair('Generator', e.generatorId + ' v' + e.generatorVersion + ', seed ' + e.generatorSeed),
      pair('Review status', e.reviewStatus), pair('Edge model', e.edgeModel, 'wide'), pair('Site', e.siteNote, 'wide'),
      ...view.areas.map((area) => pair(area.label + ' (' + area.areaId + ')', area.areaM2Exact + ' m² (' + fixed(area.areaHa, 4) + ' ha) · seed ' +
        area.simulationSeed + ' · ' + area.trees.length + ' tree records')),
      pair('Models', 'RNG v' + s.rngModelVersion + ' · growth model v' + s.growthModelVersion + ' · rules ' + s.p1AlgorithmVersion +
        ' · ' + s.areaModelVersion, 'wide'),
      pair('Ecological year', s.ecologicalYear), pair('Annual revision', s.annualRevision),
      pair('Work revision', s.workRevision), pair('Plan-state revision', view.planStateRevision),
      pair('Session', expandable(view.sessionId)));
  }

  export function renderPaneButtons() {
    const view = state.view;
    const counts = {
      plan: view.pending.length + (view.approved ? view.approved.orders.length : 0) + pendingPrunes().length +
        approvedPrunes().length + pendingClears().length + approvedClears().length +
        pendingPlantings().length + approvedPlantings().length,
      history: view.annualHistory.length,
    };
    for (const button of document.querySelectorAll('[data-pane-button]')) {
      const name = button.getAttribute('data-pane-button');
      const wanted = PANE_NAMES[name] + (name in counts ? '|' + counts[name] : '');
      if (button.getAttribute('data-text') === wanted) continue;
      button.setAttribute('data-text', wanted);
      button.replaceChildren(PANE_NAMES[name], ...(name in counts ? [' ', el('span', { class: 'count' }, String(counts[name]))] : []));
    }
  }

  export function showPane(name) {
    if (!PANES.includes(name)) return;
    byId('side').setAttribute('data-pane', name);
    for (const button of document.querySelectorAll('[data-pane-button]')) {
      button.setAttribute('aria-pressed', String(button.getAttribute('data-pane-button') === name));
    }
    if (name === 'details' && state.view && !state.structure && !state.structureLoading && !state.lock) {
      hooks.refreshStructure();
    }
  }

  export function renderMapControls() {
    const area = currentArea();
    if (!area) return;
    const fit = state.zoom === ZOOMS[0];
    const box = viewBox(area);
    byId('zoom-in').disabled = state.zoom === ZOOMS[ZOOMS.length - 1];
    byId('zoom-out').disabled = fit;
    byId('pan-north').disabled = fit || !box.north;
    byId('pan-south').disabled = fit || !box.south;
    byId('pan-west').disabled = fit || !box.west;
    byId('pan-east').disabled = fit || !box.east;
  }

  export function zoomMap(step) {
    const area = currentArea();
    if (!area) return;
    const index = ZOOMS.indexOf(state.zoom);
    const next = ZOOMS[Math.min(ZOOMS.length - 1, Math.max(0, index + step))];
    if (next === state.zoom) return;
    if (state.zoom === ZOOMS[0]) {
      const item = selectedId() ? itemOf(area.areaId, selectedId()) : null;
      state.centre = item ? { u: item.u, v: item.v } : null;
    } else {
      state.centre = usableCentre(viewBox(area));
    }
    state.zoom = next;
    if (next === ZOOMS[0]) state.centre = null;
    else state.centre = usableCentre(viewBox(area));
    svgRenderer.render();
    renderMapControls();
  }

  export function panMap(east, south) {
    const area = currentArea();
    if (!area || state.zoom === ZOOMS[0]) return;
    const box = viewBox(area);
    const centre = usableCentre(box);
    state.centre = { u: centre.u + east * box.uw / 4, v: centre.v + south * box.uh / 4 };
    state.centre = usableCentre(viewBox(area));
    svgRenderer.render();
    renderMapControls();
  }

