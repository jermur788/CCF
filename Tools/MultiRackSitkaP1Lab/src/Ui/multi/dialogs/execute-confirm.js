// S0c provenance: dialogs/execute-confirm.js [L4] — section(s) 6 from multi.js
import { lockCommands } from '../api.js';
import { approvedClears, approvedPlantings, approvedPrunes, byId, readoutState, state } from '../store.js';
  // ---- Execute confirmation -------------------------------------------------------------------------------------------

  function frozen(value) {
    if (value && typeof value === 'object') {
      for (const inner of Object.values(value)) frozen(inner);
      Object.freeze(value);
    }
    return value;
  }

  export function confirmationOf(view) {
    const plan = view.approved;
    return frozen({
      sessionId: view.sessionId,
      source: Object.assign({}, view.source),
      planStateRevision: view.planStateRevision,
      planId: plan.planId,
      executionKey: plan.executionKey,
      year: view.source.ecologicalYear,
      orders: plan.orders.map((order) => ({ areaId: order.areaId, treeId: order.treeId, reason: order.reason })),
      prunes: approvedPrunes().map((order) => ({ areaId: order.areaId, treeId: order.treeId,
        targetCrownBaseHeightM: order.targetCrownBaseHeightM })),
      clears: approvedClears().map((order) => ({ areaId: order.areaId, column: order.column, row: order.row })),
      plantings: approvedPlantings().map((order) => ({ orderId: order.orderId, kind: order.kind, areaId: order.areaId,
        speciesId: order.speciesId || null, xm: order.xm, zm: order.zm })),
    });
  }

  function sameSource(a, b) {
    for (const name of new Set(Object.keys(a).concat(Object.keys(b)))) if (a[name] !== b[name]) return false;
    return true;
  }

  export function confirmationStillShown(confirmation, view) {
    const plan = view.approved;
    return view.sessionId === confirmation.sessionId && view.planStateRevision === confirmation.planStateRevision &&
      sameSource(view.source, confirmation.source) && plan !== null && plan.planId === confirmation.planId &&
      plan.executionKey === confirmation.executionKey && plan.orders.length === confirmation.orders.length &&
      plan.orders.every((order, index) => order.areaId === confirmation.orders[index].areaId &&
        order.treeId === confirmation.orders[index].treeId && order.reason === confirmation.orders[index].reason) &&
      samePrunes(confirmation.prunes) && sameClears(confirmation.clears) && samePlantings(confirmation.plantings);
  }

  function samePrunes(frozenPrunes) {
    if (!state.stage4) return false;
    const now = approvedPrunes();
    return now.length === frozenPrunes.length && now.every((order, index) => order.areaId === frozenPrunes[index].areaId &&
      order.treeId === frozenPrunes[index].treeId && order.targetCrownBaseHeightM === frozenPrunes[index].targetCrownBaseHeightM);
  }

  function sameClears(frozenClears) {
    if (frozenClears.length > 0 && !state.ecology) return false;
    const now = approvedClears();
    return now.length === frozenClears.length && now.every((o, i) => o.areaId === frozenClears[i].areaId &&
      o.column === frozenClears[i].column && o.row === frozenClears[i].row);
  }

  function samePlantings(frozenPlantings) {
    if (frozenPlantings.length > 0 && !state.ecology) return false;
    const now = approvedPlantings();
    return now.length === frozenPlantings.length && now.every((o, i) => o.orderId === frozenPlantings[i].orderId &&
      o.kind === frozenPlantings[i].kind && o.areaId === frozenPlantings[i].areaId);
  }

  export function withdrawConfirmation(confirmation) {
    state.confirmation = null;
    const dialog = byId('execute-dialog');
    if (dialog.open) dialog.close();
    const unpaired = readoutState() === 'unavailable';
    const names = confirmation.orders.concat(confirmation.prunes).map((order) => order.treeId)
      .concat(confirmation.clears.map((c) => 'Cell(' + c.column + ',' + c.row + ')'))
      .concat(confirmation.plantings.map((p) => p.orderId));
    lockCommands(unpaired ? 'Confirmation withdrawn — the read-out of this view is unavailable' : 'Confirmation withdrawn — the approved plan changed',
      'The Execute confirmation showed ' + (names.join(', ') || 'no orders') +
      '. ' + (unpaired ? 'The view that has just arrived came without its engine read-out, so its orders cannot be checked'
        : 'The view that has just arrived no longer shows that plan with that fence') + ', so nothing was sent. Review the plan now ' +
      'shown, then choose Re-read; after a successful re-read you can choose Execute again.');
  }

