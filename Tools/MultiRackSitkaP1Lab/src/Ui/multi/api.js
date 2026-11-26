// S0c provenance: api.js [L2] — section(s) 3, 5 from multi.js
import { el, label } from './dom.js';
import { COMMAND_IDS, areaView, blockedReason, byId, currentReport, draftOf, groundCell, hooks, isPhase2, juvenileById, livingIn, readoutState, state, treeIn } from './store.js';
  // ---- talking to the local session process ---------------------------------------------------------------------------

  export function currentFence() {
    const view = state.view;
    return { sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision };
  }

  export async function call(path, body) {
    if (body !== undefined && state.lock) return { kind: 'locked' };
    const init = { method: body === undefined ? 'GET' : 'POST', cache: 'no-store', credentials: 'omit', redirect: 'error' };
    if (body !== undefined) {
      init.headers = { 'Content-Type': 'application/json' };
      init.body = JSON.stringify(body);
    }
    let response;
    try {
      response = await fetch(path, init);
    } catch (failure) {
      return { kind: 'transport', detail: String((failure && failure.message) || failure) };
    }
    let payload = null;
    try { payload = await response.json(); } catch { payload = null; }
    if (response.status === 200 && payload && payload.view) return { kind: 'ok', payload };
    if (response.status === 409 && payload && payload.view && payload.refusal) return { kind: 'refused', payload };
    if (payload && payload.error) {
      const unconfirmed = payload.error.code === 'OutcomeUnconfirmed';
      return { kind: unconfirmed ? 'unconfirmed' : 'guard', status: response.status, error: payload.error };
    }
    return { kind: 'transport', detail: 'HTTP ' + response.status };
  }

  export function applyPayload(payload, keepOverlap) {
    const prevTreeTag = state.view ? (state.view.sessionId + ':' + state.view.source.annualRevision + ':' + state.view.source.workRevision) : null;
    state.view = payload.view;
    state.scenes = payload.scenes || {};
    state.stage4 = payload.stage4 || null;
    state.ecology = payload.ecology || null;
    if (payload.labels) state.labels = payload.labels;
    if (typeof payload.historyTruncated === 'boolean') state.historyTruncated = payload.historyTruncated;
    if (typeof payload.payloadBudgetExceeded === 'boolean') state.payloadBudgetExceeded = payload.payloadBudgetExceeded;
    if (typeof payload.totalRecordedYears === 'number') state.totalRecordedYears = payload.totalRecordedYears;
    if (payload.reviewPreview) state.reviewPreview = payload.reviewPreview;
    const nextTreeTag = state.view ? (state.view.sessionId + ':' + state.view.source.annualRevision + ':' + state.view.source.workRevision) : null;
    if (prevTreeTag && nextTreeTag !== prevTreeTag) {
      state.structureRequest++;
      state.structureLoading = false;
      state.structure = null;
      state.structureRootTag = null;
    }
    if (state.cellDetail && !state.cellDetail.missing &&
        !groundCell(state.cellDetail.areaId, state.cellDetail.column, state.cellDetail.row)) {
      state.cellDetail = null;
    }
    if (state.selectedJuvenile && !juvenileById(state.selectedJuvenile.juvenileId)) {
      state.selectedJuvenile = null;
    }
    if (!areaView(state.areaId)) state.areaId = state.view.areas.length > 0 ? state.view.areas[0].areaId : null;
    for (const areaId of Object.keys(state.selected)) {
      if (!treeIn(areaId, state.selected[areaId])) delete state.selected[areaId];
    }
    if (!keepOverlap) state.overlap = null;
    const target = state.competitionTarget;
    if (target && !livingIn(target.areaId, target.treeId)) hooks.clearCompetition();
    const refresh = state.competitionTarget !== null && state.competitionTag !== hooks.competitionTag();
    if (refresh && !state.batch) state.competition = null;
    hooks.reconcileDrafts();
    hooks.renderAll();
    const confirmation = state.confirmation;
    if (confirmation && !byId('execute-dialog').open) state.confirmation = null;
    else if (confirmation && !hooks.confirmationStillShown(confirmation, state.view)) hooks.withdrawConfirmation(confirmation);
    if (refresh && !state.lock && !state.batch) hooks.refreshCompetition();
    if (prevTreeTag && nextTreeTag !== prevTreeTag && !state.lock && !state.batch && !state.queueRunning &&
        byId('side').getAttribute('data-pane') === 'details') {
      hooks.refreshStructure();
    }
  }

  export async function run(path, body, what, onCommitted, focusAfter) {
    if (!state.view || refusedByLock(what) || state.busy) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await call(path, body);
    state.busy = false;
    document.documentElement.classList.remove('busy');
    if (outcome.kind === 'ok') {
      clearAlert();
      state.lastWasReplay = Boolean(outcome.payload.execution && outcome.payload.execution.replayed);
      applyPayload(outcome.payload, false);
      if (onCommitted) onCommitted(outcome.payload);
      if (focusAfter) keepFocusOr(focusAfter);
      hooks.refreshReviewPreview();
      if (state.historyOpen) hooks.loadHistoryIndex(null, false);
    } else if (outcome.kind === 'refused') {
      applyPayload(outcome.payload, false);
      if (state.lock) showLockAlert('Refused: ' + what + ' (' + outcome.payload.refusal.code + '). Nothing changed.');
      else showRefusal(what, outcome.payload.refusal);
      hooks.refreshReviewPreview();
    } else if (outcome.kind === 'guard') {
      showGuardRefusal(what, outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: ' + what + '.');
    } else {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        what + ': no confirmed result was received. Do not assume that it happened, or that it did not.');
    }
  }

  // ---- alerts, announcements and the re-read lock ---------------------------------------------------------------------

  export function clearAlert() { const box = byId('alert'); box.replaceChildren(); box.classList.remove('shown'); }
  export function showAlert(parts, focusTarget) {
    const box = byId('alert');
    box.replaceChildren(...parts.filter((part) => part !== null));
    box.classList.add('shown');
    (focusTarget || box).focus();
  }
  export function announce(message) {
    const region = byId('announce');
    region.textContent = '';
    window.setTimeout(() => { region.textContent = message; }, 50);
  }
  export function rereadButton() {
    const button = el('button', { type: 'button' }, 'Re-read current state');
    button.addEventListener('click', () => hooks.reread(true));
    return button;
  }

  export function recoveryFor(code) {
    switch (code) {
      case 'PlayerSourceStale':
      case 'PlayerPlanStateStale':
      case 'PlayerSessionMismatch':
        return 'The page now shows the current view. Review it, then try again.';
      case 'WorkReasonRequired':
        return 'Choose or write a reason for this tree, then add it again.';
      case 'WorkPlanAlreadyApproved':
        return 'Cancel the approved plan before editing the Work Plan.';
      case 'WorkPendingConsentRequired':
        return 'Choose whether to keep the orders or cancel all of them before the year advances.';
      case 'WorkPlanOpen':
        return 'Open Pending or Approved work exists in the Work Plan. Execute, cancel, or remove open orders first — Skip to Review never silently cancels open work.';
      case 'InvalidTargetYear':
        return 'Skip to Review advances between 1 and 5 ecological years per press. Re-read the preview target and try again.';
      case 'StandCannotAdvance':
        return 'Every area has no living stems and no juvenile regeneration remaining.';
      case 'HistoryYearInvalid':
      case 'HistoryYearNotRecorded':
        return 'Select a recorded ecological year from the History index.';
      case 'WorkWouldLeaveNoLivingStems':
        return label('MultiLastLivingRefused') + '. Cancel the approved plan and approve a smaller one.';
      case 'MultiAreaWrongArea':
        return 'That target belongs to another area. Switch to its area and try again.';
      case 'CropTreeRetained':
        return label('MultiCropFellRefused') + '.';
      case 'ContinuationNoLivingStems':
        return 'Every area has no living tree, so there is no year to advance.';
      case 'Phase2CoordinatesInvalid':
        return 'Choose a finite map point inside the area polygon and try again.';
      case 'Phase2PointOutsideArea':
        return 'Pick a point strictly inside the active area boundary and try again.';
      case 'Phase2PointWithoutGroundCell':
        return 'That edge sliver has no modelled 2.5 m ground cell. Choose a point further inside the area.';
      case 'Phase2DuplicateOrder':
        return 'An order of this kind is already queued at that exact point. Remove the existing order or pick a different point.';
      case 'Phase2SpeciesUnknown':
        return 'Choose Sessile Oak (sessile-oak) or European Beech (beech) and try again.';
      case 'PlantSpacingTooClose':
        return 'Choose a planting point further away from existing planted saplings or queued Plant orders.';
      case 'PlantTooCloseToAdult':
        return 'Choose a planting point further away from living adult stems.';
      case 'ShelterNoTarget':
        return 'A shelter requires a living planted sapling (PJ) or a queued Plant order at that point.';
      case 'ShelterAlreadyPresent':
        return 'An active or queued shelter already protects that point.';
      case 'ClearCellNotInArea':
        return 'Choose an in-polygon 2.5 m ground cell of the active area.';
      case 'ClearDuplicateCell':
        return 'That ground cell is already queued for ClearUndergrowth.';
      case 'UnknownWorkOrder':
        return 'That pending order is no longer in the Work Plan. Review the current Work Plan.';
      default:
        return 'Review the current view, then choose again.';
    }
  }

  export function showRefusal(what, refusal) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused: ' + what),
      el('p', null, el('code', null, refusal.code), ' — ' + refusal.message),
      el('p', null, label('MultiNothingChanged') + '. Current ecological year ' + state.view.source.ecologicalYear + '.'),
      el('p', null, recoveryFor(refusal.code)),
    ]);
  }

  export function showGuardRefusal(what, status, error) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused by the local page server: ' + what),
      el('p', null, el('code', null, error.code), ' (HTTP ' + status + ') — ' + error.message),
      el('p', null, 'The request did not reach the engine.'),
      rereadButton(),
    ]);
  }

  export function lockCommands(title, detail) {
    state.lock = { title, detail };
    state.lockCount += 1;
    hooks.renderAll();
    showLockAlert(null);
  }

  export function showLockAlert(extra) {
    const lock = state.lock;
    if (!lock) return;
    showAlert([
      el('p', { class: 'alert-title' }, lock.title),
      el('p', null, lock.detail),
      extra ? el('p', null, extra) : null,
      el('p', null, 'Every command stays unavailable until a re-read of the current state succeeds.'),
      rereadButton(),
    ]);
  }

  export function refusedByLock(what) {
    if (!state.lock) return false;
    showLockAlert('Not sent: ' + what + '.');
    return true;
  }

  export function addAvailability() {
    const areaId = state.areaId;
    if (state.lock) return { ok: false, why: 'Re-read the current state before adding.' };
    if (state.view && state.view.approved) return { ok: false, why: 'The plan is approved, so editing is locked. Cancel the approved plan to change it.' };
    if (!currentReport()) return { ok: false, why: 'Waiting for the current competition reading.' };
    const eligible = draftOf(areaId).filter((id) => !blockedReason(areaId, id));
    if (eligible.length === 0) return { ok: false, why: 'Tick one or more competitors to add.' };
    return { ok: true, why: null, count: eligible.length };
  }

  export function renderCommandGate() {
    const blocked = state.lock !== null || state.batch !== null || state.queueRunning;
    const planLocked = Boolean(state.view && state.view.approved);
    for (const id of COMMAND_IDS) {
      const node = byId(id);
      if (node) node.disabled = blocked;
    }
    if (readoutState() === 'unavailable') for (const id of ['approve', 'execute', 'execute-confirm']) byId(id).disabled = true;
    for (const button of document.querySelectorAll('[data-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-prune-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-clear-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-planting-remove]')) button.disabled = blocked;
    if (!blocked) {
      hooks.renderPruneGate();
      byId('cell-clear-add').disabled = planLocked || !isPhase2();
      byId('cell-clear-remove').disabled = planLocked;
      byId('plant-queue').disabled = planLocked || !isPhase2() || !state.plantDraft || state.plantDraft.areaId !== state.areaId;
      byId('spot-queue').disabled = planLocked || !isPhase2() || !state.spotDraft || state.spotDraft.areaId !== state.areaId;
      byId('pj-shelter-add').disabled = planLocked || !isPhase2();
      byId('pj-shelter-remove').disabled = planLocked;
    }
    byId('add-button').disabled = state.batch !== null || state.queueRunning || !addAvailability().ok;
    byId('draft-clear').disabled = state.batch !== null || state.queueRunning || draftOf(state.areaId).length === 0;
    byId('reread').disabled = state.batch !== null || state.queueRunning;
    for (const button of document.querySelectorAll('[data-area]')) {
      if (button.classList.contains('area-tab')) button.disabled = state.batch !== null || state.queueRunning;
    }
  }

  function keepFocusOr(id) {
    const active = document.activeElement;
    const usable = active && active !== document.body && active.isConnected && !active.closest('[hidden]') && !active.disabled;
    if (!usable) byId(id).focus();
  }

