// S0c provenance: commands.js [L4] — section(s) 16 from multi.js
import { addAvailability, announce, applyPayload, call, clearAlert, currentFence, lockCommands, recoveryFor, refusedByLock, renderCommandGate, rereadButton, run, showAlert, showBudgetExceeded, showGuardRefusal, showLockAlert, showRefusal, syncStandOverview } from './api.js';
import { confirmationOf, confirmationStillShown, withdrawConfirmation } from './dialogs/execute-confirm.js';
import { el, fill, fixed, label, plural, technicalPlan } from './dom.js';
import { addSequence, reasonToSend, refreshCompetition } from './panels/fell-draft.js';
import { pruneAvailability } from './panels/inspector.js';
import { ecoOrderList, orderList, pruneList, renderReceipt, showPane } from './panels/work-plan.js';
import { approvedClears, approvedPlantings, approvedPrunes, areaName, areaView, blockedReason, byId, currentJuvenile, currentReport, draftOf, draftTotal, hooks, isEconomyEnabled, itemOf, ordersIn, pendingClears, pendingPlantings, pendingPrunes, pruneStatusOf, readoutState, referenceIn, resetSessionUiState, selectedId, state } from './store.js';
  // ---- Work Plan commands (Fell, Prune, ClearUndergrowth, Plant, Shelter, SpotClear, Crop, Approve, Cancel, Execute, Year +1) ----

  export function onCellClearAdd() {
    const c = state.cellDetail;
    if (!c || c.missing || !state.view || state.batch || state.queueRunning) return;
    run('api/multi/clear/add', { fence: currentFence(), areaId: c.areaId, column: c.column, row: c.row },
      'Queue ClearUndergrowth for cell (' + c.column + ', ' + c.row + ')',
      () => announce('Done: cell (' + c.column + ', ' + c.row + ') queued as pending ClearUndergrowth (spares seedlings; nothing cleared yet).'),
      'cell-clear-remove');
  }

  export function removeClear(areaId, column, row) {
    if (!areaId || state.batch || state.queueRunning) return;
    run('api/multi/clear/remove', { fence: currentFence(), areaId, column: Number(column), row: Number(row) },
      'Remove pending ClearUndergrowth for cell (' + column + ', ' + row + ')',
      () => announce('Done: pending ClearUndergrowth for cell (' + column + ', ' + row + ') removed.'),
      state.mapMode === 'cell' ? 'cell-clear-add' : 'plan-heading');
  }

  export function removePlanting(orderId) {
    if (!orderId || state.batch || state.queueRunning) return;
    run('api/multi/planting/remove', { fence: currentFence(), orderId },
      'Remove pending order ' + orderId,
      () => announce('Done: pending order ' + orderId + ' removed from Work Plan.'),
      state.mapMode === 'plant' ? 'plant-heading' : state.mapMode === 'spot' ? 'spot-heading' :
        currentJuvenile() ? 'pj-heading' : 'plan-heading');
  }

  // Sequential fence-chained planting queue: (1) plant/add, (2) if ticked shelter/add, (3) if ticked spot-clear/add.
  // Stops immediately on step 2/3 409, surfaces exact Core refusal, and discloses earlier pending orders with one-click remove.
  export async function onPlantQueue() {
    if (!state.view || !state.plantDraft || state.busy || state.batch || state.queueRunning) return;
    if (refusedByLock('Queue planting order')) return;
    if (state.view.approved) {
      showAlert([el('p', { class: 'alert-title' }, 'The plan is approved'), el('p', null, recoveryFor('WorkPlanAlreadyApproved'))]);
      return;
    }
    const { areaId, xM, zM } = state.plantDraft;
    const speciesId = byId('plant-species').value;
    const wantShelter = Boolean(byId('plant-opt-shelter').checked);
    const wantSpot = Boolean(byId('plant-opt-spot').checked);
    const steps = [{ kind: 'Plant', path: 'api/multi/plant/add', body: (fence) => ({ fence, areaId, speciesId, xM, zM }) }];
    if (wantShelter) steps.push({ kind: 'Shelter', path: 'api/multi/shelter/add', body: (fence) => ({ fence, areaId, xM, zM }) });
    if (wantSpot) steps.push({ kind: 'SpotClear', path: 'api/multi/spot-clear/add', body: (fence) => ({ fence, areaId, xM, zM }) });

    const result = { areaId, speciesId, xM, zM, queued: [], refused: null, guard: null, unconfirmed: null, unattempted: [] };
    state.queueRunning = true;
    state.busy = true;
    state.plantResult = null;
    document.documentElement.classList.add('busy');
    renderCommandGate();

    for (let i = 0; i < steps.length; i++) {
      const step = steps[i];
      const beforeIds = new Set(pendingPlantings().map((o) => o.orderId));
      const outcome = await call(step.path, step.body(currentFence()));
      if (outcome.kind === 'ok') {
        clearAlert();
        applyPayload(outcome.payload, false);
        const added = pendingPlantings().find((o) => !beforeIds.has(o.orderId));
        result.queued.push(added || { orderId: '(queued)', kind: step.kind, areaId, speciesId: step.kind === 'Plant' ? speciesId : null, xm: xM, zm: zM });
        continue;
      }
      result.unattempted = steps.slice(i + 1).map((s) => s.kind);
      if (outcome.kind === 'refused') {
        result.refused = { kind: step.kind, code: outcome.payload.refusal.code, message: outcome.payload.refusal.message };
        applyPayload(outcome.payload, false);
        showRefusal('Queue ' + step.kind, outcome.payload.refusal);
      } else if (outcome.kind === 'guard') {
        result.guard = { kind: step.kind, status: outcome.status, code: outcome.error.code, message: outcome.error.message };
        showGuardRefusal('Queue ' + step.kind, outcome.status, outcome.error);
      } else if (outcome.kind === 'locked') {
        result.unattempted = steps.slice(i).map((s) => s.kind);
        showLockAlert('Not sent: Queue ' + step.kind + '.');
      } else {
        result.unconfirmed = { kind: step.kind, detail: outcome.kind === 'unconfirmed' ? outcome.error.message : outcome.detail };
        lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
          'Queue ' + step.kind + ': no confirmed result was received. Do not assume that it happened, or that it did not.');
      }
      break;
    }

    state.queueRunning = false;
    state.busy = false;
    document.documentElement.classList.remove('busy');
    if (!result.refused && !result.guard && !result.unconfirmed) {
      state.plantDraft = null;
    }
    state.plantResult = result;
    hooks.renderAll();
    // NB-2 polish: keep exact refusal code and Already queued Remove buttons immediately visible without scrolling.
    byId('pane-tree').scrollTop = 0;
    const plantResBox = byId('plant-result');
    if (!plantResBox.hidden && typeof plantResBox.scrollIntoView === 'function') {
      plantResBox.scrollIntoView({ block: 'nearest' });
    }
    if (!result.unconfirmed) {
      if (result.refused) {
        const queuedText = result.queued.length > 0
          ? ' Earlier pending orders queued: ' + result.queued.map((o) => o.orderId + ' (' + o.kind + ')').join(', ') + '.'
          : ' Nothing queued.';
        announce('Refused ' + result.refused.kind + ' (' + result.refused.code + ').' + queuedText);
      } else if (!result.guard) {
        announce('Queued as pending (not executed): ' + result.queued.map((o) => o.orderId + ' (' + o.kind + ')').join(', ') + '.');
      }
      hooks.refreshReviewPreview();
    }
  }

  export async function onSpotQueue() {
    if (!state.view || !state.spotDraft || state.busy || state.batch || state.queueRunning) return;
    if (refusedByLock('Queue Spot Clear')) return;
    if (state.view.approved) {
      showAlert([el('p', { class: 'alert-title' }, 'The plan is approved'), el('p', null, recoveryFor('WorkPlanAlreadyApproved'))]);
      return;
    }
    const { areaId, xM, zM } = state.spotDraft;
    const beforeIds = new Set(pendingPlantings().map((o) => o.orderId));
    state.busy = true;
    state.spotResult = null;
    document.documentElement.classList.add('busy');
    const outcome = await call('api/multi/spot-clear/add', { fence: currentFence(), areaId, xM, zM });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    if (outcome.kind === 'ok') {
      clearAlert();
      state.spotDraft = null;
      applyPayload(outcome.payload, false);
      const added = pendingPlantings().find((o) => !beforeIds.has(o.orderId));
      state.spotResult = { areaId, order: added || { orderId: '(queued)', kind: 'SpotClear', areaId, xm: xM, zm: zM }, refused: null };
      hooks.renderAll();
      byId('pane-tree').scrollTop = 0;
      announce('Queued pending SpotClear ' + (added ? added.orderId : '') + ' (not executed).');
      hooks.refreshReviewPreview();
    } else if (outcome.kind === 'refused') {
      state.spotResult = { areaId, order: null, refused: outcome.payload.refusal };
      applyPayload(outcome.payload, false);
      byId('pane-tree').scrollTop = 0;
      showRefusal('Queue Spot Clear', outcome.payload.refusal);
      announce('Refused SpotClear (' + outcome.payload.refusal.code + '). Nothing changed.');
      hooks.refreshReviewPreview();
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Queue Spot Clear', outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: Queue Spot Clear.');
    } else {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        'Queue Spot Clear: no confirmed result was received. Do not assume that it happened, or that it did not.');
    }
  }

  export function onPjShelterAdd() {
    const j = currentJuvenile();
    if (!j || !state.view || state.batch || state.queueRunning) return;
    run('api/multi/shelter/add', { fence: currentFence(), areaId: j.areaId, xM: j.xm, zM: j.zm },
      'Queue 8-year Shelter for ' + j.juvenileId,
      () => announce('Done: 8-year shelter queued as pending for ' + j.juvenileId + ' (not installed until Execute).'),
      'pj-shelter-remove');
  }

  export function onCrop() {
    const areaId = state.areaId;
    const treeId = selectedId();
    if (!treeId || !state.view || state.batch) return;
    const marked = areaView(areaId).cropTreeIds.includes(treeId);
    run(marked ? 'api/multi/crop/unmark' : 'api/multi/crop/mark', { fence: currentFence(), areaId, treeId },
      label(marked ? 'MultiUnmarkCrop' : 'MultiMarkCrop') + ' (' + treeId + ')',
      () => announce((marked ? 'Removed Crop designation: ' : 'Retained Crop Tree: ') + treeId + ' in ' + areaName(areaId) + '.'));
  }

  export function onAdd(event) {
    event.preventDefault();
    const areaId = state.areaId;
    const reference = referenceIn(areaId);
    const report = currentReport();
    if (!state.view || state.busy || state.batch || !reference || !report) return;
    if (refusedByLock('Add selected trees to the Work Plan')) return;
    if (!addAvailability().ok) return;
    const chosen = new Set(draftOf(areaId));
    const ids = report.topFive.map((n) => n.treeId).filter((id) => chosen.has(id) && !blockedReason(areaId, id));
    if (ids.length === 0) return;
    addSequence(areaId, reference, ids, reasonToSend(false), 'competitors');
  }

  export function onAddReference() {
    const areaId = state.areaId;
    const treeId = selectedId();
    const item = treeId ? itemOf(areaId, treeId) : null;
    if (!state.view || state.busy || state.batch || !item || item.status !== 'Living' || item.isCrop) return;
    if (refusedByLock('Add ' + treeId + ' to the Work Plan')) return;
    if (state.view.approved) {
      showAlert([el('p', { class: 'alert-title' }, 'The plan is approved'), el('p', null, recoveryFor('WorkPlanAlreadyApproved'))]);
      return;
    }
    addSequence(areaId, referenceIn(areaId), [treeId], reasonToSend(true), 'reference');
  }

  export function removePending(areaId, treeId) {
    if (!areaId || !treeId || state.batch) return;
    run('api/multi/pending/remove', { fence: currentFence(), areaId, treeId }, 'Remove ' + treeId + ' from the Work Plan',
      () => announce('Done: ' + label('MultiRemoveFromPlan') + ' (' + treeId + ').'), 'plan-heading');
  }

  export function onPruneAdd() {
    const areaId = state.areaId;
    const treeId = selectedId();
    const ps = pruneStatusOf(areaId, treeId);
    if (!state.view || state.busy || state.batch || !treeId || !ps) return;
    if (refusedByLock('Add ' + treeId + ' to the Prune plan')) return;
    if (!pruneAvailability(areaId, itemOf(areaId, treeId), ps).ok) return;
    run('api/multi/prune/add', { fence: currentFence(), areaId, treeId }, 'Add ' + treeId + ' to the Prune plan', (payload) => {
      const order = payload.stage4 ? payload.stage4.pendingPrunes.find((o) => o.treeId === treeId) : null;
      announce('Done: ' + treeId + ' added as a pending Prune' + (order ? ' (crown base to ' + fixed(order.targetCrownBaseHeightM, 1) + ' m)' : '') +
        '. No crown was cut; Approve and Execute are separate.');
    }, 'prune-remove');
  }

  export function removePrune(areaId, treeId) {
    if (!areaId || !treeId || state.batch) return;
    run('api/multi/prune/remove', { fence: currentFence(), areaId, treeId }, 'Remove the pending Prune of ' + treeId,
      () => announce('Done: pending Prune of ' + treeId + ' removed. Nothing was cut.'), 'plan-heading');
  }

  export function onSetOrderExecutor(kind, areaId, targetKey, executor) {
    if (!kind || !areaId || !targetKey || !executor || !state.view || state.batch || state.queueRunning) return;
    run('api/multi/economy/executor', { fence: currentFence(), kind, areaId, targetKey, executor },
      'Assign ' + kind + ' (' + targetKey + ') to ' + executor,
      () => announce('Assigned ' + kind + ' ' + targetKey + ' to ' + executor + '. Quote updated.'),
      'plan-heading');
  }

  export function openEconomyToggleDialog() {
    const view = state.view;
    if (!view || state.busy || state.batch || state.queueRunning || refusedByLock('Switch Money mode')) return;
    const currentlyOn = isEconomyEnabled();
    const targetEnabled = !currentlyOn;
    state.economyToggleTarget = targetEnabled;
    const pendingCount = view.pending.length + pendingPrunes().length + pendingClears().length + pendingPlantings().length;
    const hasApproved = view.approved !== null;
    const drafts = draftTotal();
    const dirtyParts = [];
    if (view.source.ecologicalYear > 0) dirtyParts.push('Year ' + view.source.ecologicalYear);
    if (pendingCount > 0) dirtyParts.push(plural(pendingCount, 'pending order', 'pending orders'));
    if (hasApproved) dirtyParts.push('an approved Work Plan');
    if (view.events.length > 0) dirtyParts.push(plural(view.events.length, 'executed work event', 'executed work events'));
    if (drafts > 0) dirtyParts.push(plural(drafts, 'unsent draft choice', 'unsent draft choices'));
    const dirty = dirtyParts.length > 0;
    byId('economy-toggle-title').textContent = targetEnabled ? 'Turn Money On (fresh Year-0 session)' : 'Turn Money Off (fresh Year-0 session)';
    byId('economy-toggle-text').textContent = targetEnabled
      ? 'Money is a launch-time opt-in. Starting with Money On opens a fresh Year-0 session of this stand with the engine\'s opening cash balance and annual landowner hour allowance.'
      : 'Turning Money Off opens a fresh Year-0 session of this stand with Economy disabled (financial history reads NOT RECORDED and Save v2 slots are available).';
    const warn = byId('economy-toggle-warning');
    warn.hidden = !dirty;
    warn.textContent = dirty
      ? 'Unsaved progress will be discarded: ' + dirtyParts.join(', ') + '. Confirm to reset to a fresh Year-0 session.'
      : '';
    byId('economy-toggle-note').textContent = targetEnabled
      ? 'Note: Economy sessions require Save v3 (deferred to Phase 2B-2b), so Save v2 slots cannot be saved or loaded while Money is On. Earlier session tokens will be invalidated.'
      : 'Earlier session tokens will be invalidated and local selections will reset to Year 0.';
    byId('economy-toggle-confirm').textContent = dirty
      ? (targetEnabled ? 'Discard progress and turn Money On' : 'Discard progress and turn Money Off')
      : (targetEnabled ? 'Start fresh Year-0 session (Money On)' : 'Start fresh Year-0 session (Money Off)');
    openDialog(byId('economy-toggle-dialog'), byId('economy-toggle-cancel'));
  }

  export async function onEconomyToggleConfirmed() {
    const targetEnabled = state.economyToggleTarget;
    state.economyToggleTarget = null;
    byId('economy-toggle-dialog').close();
    if (targetEnabled === null || targetEnabled === undefined || !state.view || state.busy) return;
    if (refusedByLock('Switch Money mode')) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await call('api/multi/economy/session', {
      fence: currentFence(),
      enabled: Boolean(targetEnabled),
      confirmReset: true,
    });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    if (outcome.kind === 'ok') {
      clearAlert();
      resetSessionUiState();
      applyPayload(outcome.payload, false);
      announce(targetEnabled
        ? 'Started fresh Year-0 session with Money On (' + (state.economy ? state.economy.cashBalanceDisplay + ', ' + state.economy.landownerHoursDisplay : 'active') + ').'
        : 'Started fresh Year-0 session with Money Off.');
      hooks.refreshReviewPreview();
      if (state.historyOpen) hooks.loadHistoryIndex(null, false);
      if (byId('side').getAttribute('data-pane') === 'details') hooks.refreshStructure();
    } else if (outcome.kind === 'refused') {
      applyPayload(outcome.payload, false);
      showRefusal('Switch Money mode', outcome.payload.refusal);
      hooks.refreshReviewPreview();
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Switch Money mode', outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: Switch Money mode.');
    } else {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        'Switch Money mode: no confirmed result was received. Do not assume that it happened, or that it did not.');
    }
  }

  export function onApprove() {
    run('api/multi/plan/approve', { fence: currentFence() }, label('MultiApprove'), (payload) => {
      const prunes = payload.stage4 ? payload.stage4.approvedPrunes.map((o) => o.treeId) : [];
      const clears = payload.ecology ? (payload.ecology.approvedClear || []).map((c) => 'Cell(' + c.column + ',' + c.row + ')') : [];
      const plants = payload.ecology ? (payload.ecology.approvedPlanting || []).map((p) => p.orderId) : [];
      announce('Done: ' + label('MultiApprove') + '. Approved: ' +
        payload.view.approved.orders.map((o) => o.treeId).concat(prunes, clears, plants).join(', ') +
        '. Zero ecology changed.');
    }, 'plan-heading');
  }

  export function onCancelApproved() {
    run('api/multi/plan/cancel', { fence: currentFence() }, label('MultiCancelApproved'),
      () => announce('Done: ' + label('MultiCancelApproved') + '. Its orders were discarded, not returned to pending.'), 'plan-heading');
  }

  export function openDialog(dialog, initialFocus) { state.opener = document.activeElement; dialog.showModal(); initialFocus.focus(); }
  export function returnFocus() {
    const opener = state.opener;
    state.opener = null;
    if (byId('alert').contains(document.activeElement)) return;
    if (opener && opener.isConnected && !opener.closest('[hidden]') && !opener.disabled) opener.focus();
  }

  export function openExecuteDialog() {
    const view = state.view;
    if (!view || !view.approved || state.batch || state.queueRunning || refusedByLock(label('MultiExecute'))) return;
    if (readoutState() === 'unavailable') {
      showAlert([el('p', { class: 'alert-title' }, 'Execute needs a re-read'),
        el('p', null, 'The engine read-out for this view could not be paired with it, so the approved plan cannot be confirmed exactly. Nothing was sent.'),
        rereadButton()]);
      return;
    }
    const confirmation = confirmationOf(view);
    state.confirmation = confirmation;
    const n = confirmation.orders.length;
    const m = confirmation.prunes.length;
    const c = confirmation.clears.length;
    const p = confirmation.plantings.length;
    const areas = new Set(confirmation.orders.map((order) => order.areaId)).size;
    byId('execute-title').textContent = label('MultiExecute');
    const fellText = n === 0 ? '' : fill(label(n === 1 ? 'MultiExecuteConfirmationOneTree' : 'MultiExecuteConfirmation'),
      { n, a: areas, y: confirmation.year }) + '.';
    const pruneText = m === 0 ? '' : n === 0 && c === 0 && p === 0 ? fill(label('MultiExecutePruneConfirmation'), { n: m, y: confirmation.year })
      .replace('named trees', m === 1 ? 'named tree' : 'named trees') + '.'
      : 'Prunes ' + plural(m, 'named tree', 'named trees') + ' (one lift each).';
    const ecoParts = [];
    if (c > 0) ecoParts.push('clears undergrowth on ' + plural(c, 'cell', 'cells') + ' (spares seedlings)');
    if (p > 0) ecoParts.push('commits ' + plural(p, 'planting order', 'planting orders') + ' (SpotClear → Plant → Shelter)');
    const ecoText = ecoParts.length === 0 ? '' :
      (n === 0 && m === 0 ? 'Atomically ' : 'Also ') + ecoParts.join(' and ') + ' in Year ' + confirmation.year + ', or refuses all.';
    byId('execute-text').textContent = [fellText, pruneText, ecoText].filter(Boolean).join(' ');
    const econBox = byId('execute-economy-summary');
    if (econBox) {
      const q = isEconomyEnabled() && state.economy ? state.economy.activeQuote : null;
      econBox.hidden = !q;
      if (q) {
        econBox.replaceChildren(
          el('p', null, el('strong', null, 'Frozen financial settlement: '),
            'Upfront external cost ' + q.upfrontCashRequiredDisplay +
            ' · Expected timber income ' + q.timberSaleRevenueDisplay +
            ' · Net cash impact ' + q.netCashDeltaDisplay + '.'),
          el('p', { class: 'small-note' },
            'Landowner time: ' + q.totalOwnerHoursRequiredDisplay +
            '. Settles atomically with physical forestry work; replaying never double-charges.'));
      } else {
        econBox.replaceChildren();
      }
    }
    byId('execute-confirm').textContent = (c > 0 || p > 0) ? 'Confirm Work Plan execution'
      : n > 0 && m > 0 ? 'Confirm felling and pruning' : m > 0 ? 'Confirm pruning' : 'Confirm felling';
    byId('execute-areas').replaceChildren(...view.areas.map((area) => {
      const orders = confirmation.orders.filter((order) => order.areaId === area.areaId);
      const prunes = confirmation.prunes.filter((order) => order.areaId === area.areaId);
      const clears = confirmation.clears.filter((order) => order.areaId === area.areaId);
      const plants = confirmation.plantings.filter((order) => order.areaId === area.areaId);
      const total = orders.length + prunes.length + clears.length + plants.length;
      return el('section', { class: 'plan-area' }, el('h3', null, areaName(area.areaId) + ' — ' + total),
        total === 0 ? el('p', { class: 'note' }, 'No orders in this area.') : null,
        orders.length === 0 ? null : orderList(orders, true, false),
        prunes.length === 0 ? null : pruneList(prunes, true, false),
        ecoOrderList(clears, plants, true, false));
    }));
    byId('execute-plan').replaceChildren(label('MultiLastLivingRefused') + '. The ecological year does not change. ',
      technicalPlan(confirmation.planId, confirmation.executionKey));
    openDialog(byId('execute-dialog'), byId('execute-back'));
  }

  export function onExecuteConfirmed() {
    const confirmation = state.confirmation;
    if (state.busy && confirmation) { announce('Still waiting for the local session process. Nothing was sent.'); return; }
    state.confirmation = null;
    byId('execute-dialog').close();
    if (!confirmation || refusedByLock(label('MultiExecute'))) return;
    if (!confirmationStillShown(confirmation, state.view)) { withdrawConfirmation(confirmation); return; }
    const request = {
      fence: { sessionId: confirmation.sessionId, source: confirmation.source, planStateRevision: confirmation.planStateRevision },
      planId: confirmation.planId,
      executionKey: confirmation.executionKey,
    };
    run('api/multi/plan/execute', request, label('MultiExecute'), (payload) => {
      const execution = payload.execution;
      state.lastExecution = {
        executionKey: execution.executionKey,
        workRevisionBefore: request.fence.source.workRevision,
        workRevisionAfter: execution.workRevision,
        clears: confirmation.clears,
        plantings: confirmation.plantings,
      };
      state.plantResult = null;
      state.spotResult = null;
      renderReceipt();
      if (execution.replayed) { announce(label('MultiAlreadyExecuted') + '.'); return; }
      announce('Done: ' + label('MultiExecute') + '. Completed · History Log updated.');
    }, 'receipt-heading');
    showPane('plan');
  }

  export function sendAdvance(consent) {
    run('api/multi/year/advance', { fence: currentFence(), consentToCancelWork: consent }, label('MultiAdvanceYear'), (payload) => {
      const drafts = draftTotal();
      state.drafts = {};
      state.draftNotice = null;
      state.batchResult = null;
      state.cellDetail = null;
      state.plantDraft = null;
      state.plantResult = null;
      state.spotDraft = null;
      state.spotResult = null;
      hooks.renderAll();
      const record = payload.view.annualHistory[payload.view.annualHistory.length - 1];
      announce('Done: year ' + record.yearBefore + ' → ' + record.yearAfter + '.' +
        (drafts ? ' ' + plural(drafts, 'unsent draft choice was', 'unsent draft choices were') + ' cleared.' : ''));
      showPane('history');
    }, 'year-heading');
  }

  export function onAdvance() {
    const view = state.view;
    if (!view || state.batch || state.queueRunning || refusedByLock(label('MultiAdvanceYear'))) return;
    const work = view.pending.length > 0 || view.approved !== null || pendingPrunes().length > 0 ||
      pendingClears().length > 0 || pendingPlantings().length > 0;
    const drafts = draftTotal();
    if (work || drafts > 0) openConsentDialog(work, drafts);
    else sendAdvance(false);
  }

  function openConsentDialog(work, drafts) {
    const view = state.view;
    state.consentWork = work;
    byId('consent-title').textContent = label('MultiAdvanceYear');
    byId('consent-text').textContent = work ? 'The Work Plan has unexecuted work. Advancing the year needs your explicit choice; cancelling ' +
      'removes ALL pending and approved orders in ' + (view.areas.length >= 3 ? 'ALL lanes' : 'BOTH areas') + ' and executes nothing.' : 'The Work Plan has no unexecuted work. Advancing the year executes nothing.';
    byId('consent-areas').replaceChildren(...(work ? view.areas.map((area) => {
      const pending = ordersIn(view.pending, area.areaId).map((order) => order.treeId)
        .concat(ordersIn(pendingPrunes(), area.areaId).map((order) => order.treeId + ' (Prune)'))
        .concat(ordersIn(pendingClears(), area.areaId).map((c) => 'Cell(' + c.column + ',' + c.row + ') (Clear)'))
        .concat(ordersIn(pendingPlantings(), area.areaId).map((p) => p.orderId + ' (' + p.kind + ')'));
      const approved = (view.approved ? ordersIn(view.approved.orders, area.areaId).map((order) => order.treeId) : [])
        .concat(ordersIn(approvedPrunes(), area.areaId).map((order) => order.treeId + ' (Prune)'))
        .concat(ordersIn(approvedClears(), area.areaId).map((c) => 'Cell(' + c.column + ',' + c.row + ') (Clear)'))
        .concat(ordersIn(approvedPlantings(), area.areaId).map((p) => p.orderId + ' (' + p.kind + ')'));
      return el('p', null, areaName(area.areaId) + ': ' + (pending.length ? 'pending ' + pending.join(', ') : 'no pending orders') + '; ' +
        (approved.length ? 'approved ' + approved.join(', ') : 'no approved orders') + '.');
    }) : []));
    const draftsBox = byId('consent-drafts');
    draftsBox.hidden = drafts === 0;
    draftsBox.textContent = drafts === 0 ? '' : 'You also have ' + plural(drafts, 'unsent draft choice', 'unsent draft choices') +
      '. Draft choices are local selections, not orders; they will be cleared when the year advances.';
    byId('consent-keep').textContent = work ? fill(label('MultiConsentKeep'), { y: view.source.ecologicalYear }) :
      'Stay in year ' + view.source.ecologicalYear;
    byId('consent-advance').textContent = work ? label('MultiConsentCancel') : 'Discard draft choices and advance';
    openDialog(byId('consent-dialog'), byId('consent-keep'));
  }

  export async function reread(announceIt) {
    if (state.busy) return;
    const lockCountAtStart = state.lockCount;
    state.busy = true;
    const outcome = await call('api/multi/view');
    state.busy = false;
    if (outcome.kind === 'ok') {
      clearAlert();
      state.lastWasReplay = false;
      applyPayload(outcome.payload, false);
      if (state.view && state.view.areas && state.view.areas.length >= 3) {
        await syncStandOverview();
      }
      if (state.lock && state.lockCount === lockCountAtStart) {
        state.lock = null;
        hooks.renderAll();
        if (state.competitionTarget && !state.competition) refreshCompetition();
      }
      if (announceIt && !state.lock) {
        announce('Re-read: ecological year ' + state.view.source.ecologicalYear + ', work revision ' + state.view.source.workRevision +
          ', plan-state revision ' + state.view.planStateRevision + '.');
        hooks.refreshReviewPreview();
        if (state.historyOpen) hooks.loadHistoryIndex(null, false);
        if (byId('side').getAttribute('data-pane') === 'details') hooks.refreshStructure();
      }
    } else if (outcome.kind === 'budget') {
      showBudgetExceeded('Re-read', outcome.budget);
    } else if (state.lock) {
      showLockAlert(outcome.kind === 'guard' ? 'The re-read was refused (' + outcome.error.code + '). Try again.' :
        'The re-read got no answer from the local session process. Try again.');
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Re-read', outcome.status, outcome.error);
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'The local session process did not answer the read.'), rereadButton()]);
    }
  }

