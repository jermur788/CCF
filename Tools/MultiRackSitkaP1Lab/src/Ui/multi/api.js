// S0c provenance: api.js [L2] — section(s) 3, 5 from multi.js
import { el, label } from './dom.js';
import { COMMAND_IDS, allAreas, areaView, blockedReason, byId, currentReport, draftOf, groundCell, hooks, isEconomyEnabled, isMultiLaneStand, isPhase2, juvenileById, livingIn, readoutState, state, treeIn, visibleLaneIds } from './store.js';
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
    if (response.status === 503 && payload && payload.code === 'MultiViewPayloadBudgetExceeded') {
      return { kind: 'budget', status: 503, budget: payload };
    }
    if (payload && payload.error) {
      const unconfirmed = payload.error.code === 'OutcomeUnconfirmed';
      return { kind: unconfirmed ? 'unconfirmed' : 'guard', status: response.status, error: payload.error };
    }
    return { kind: 'transport', detail: 'HTTP ' + response.status };
  }

  export function expandCompactLanePayload(payload) {
    if (!payload) return payload;
    if (payload.stage4 && Array.isArray(payload.stage4.areas)) {
      for (const area of payload.stage4.areas) {
        if (!area.ground && Array.isArray(area.groundCompact)) {
          const viewArea = payload.view && Array.isArray(payload.view.areas)
            ? payload.view.areas.find((a) => a.areaId === area.areaId)
            : null;
          const ext = viewArea ? viewArea.extent : null;
          area.ground = area.groundCompact.map((row) => {
            const polygon = [];
            for (let i = 10; i + 1 < row.length; i += 2) {
              const u = row[i], v = row[i + 1];
              polygon.push({
                xm: ext ? ext.minXM + u : u,
                zm: ext ? ext.minZM + ext.sideM - v : v,
                u,
                v,
              });
            }
            return {
              column: row[0],
              row: row[1],
              centreXM: row[2],
              centreZM: row[3],
              centreU: row[4],
              centreV: row[5],
              clippedAreaM2: row[6],
              lightIndex: row[7],
              geometricCoveredFraction: row[8],
              crossBorderTrees: row[9],
              polygon,
            };
          });
        }
      }
    }
    if (payload.ecology && payload.ecology.pruneStatusByTreeId && Array.isArray(payload.pruneReasonTable)) {
      const table = payload.pruneReasonTable;
      for (const status of Object.values(payload.ecology.pruneStatusByTreeId)) {
        if (status && typeof status.refusalReasonIndex === 'number' && !status.refusalReason) {
          status.refusalReason = table[status.refusalReasonIndex] || null;
        }
      }
    }
    return payload;
  }

  // C-FENCE read of GET api/multi/stand overview (populates mini-map stems & whole-stand summaries only if fence matches).
  export async function syncStandOverview() {
    if (!state.view) return null;
    try {
      const response = await fetch('api/multi/stand', { method: 'GET', cache: 'no-store', credentials: 'omit', redirect: 'error' });
      if (response.status !== 200) return null;
      const overview = await response.json();
      if (!overview || !overview.fence || !state.view) return null;
      if (JSON.stringify(overview.fence) !== JSON.stringify(currentFence())) return null;
      state.standOverview = overview;
      if (typeof overview.hasUnsavedProgress === 'boolean') state.hasUnsavedProgress = overview.hasUnsavedProgress;
      if (overview.experimentBadge) state.experimentBadge = overview.experimentBadge;
      hooks.renderAll();
      return overview;
    } catch {
      return null;
    }
  }

  // C-FENCE split frame composer: pairs GET api/multi/stand (overview) + GET api/multi/stand?lanes=... (lane-filtered
  // detail for active lane + immediate neighbours, or 1 lane when singleLaneMode is on). Verifies byte-identical fence
  // equality (JSON.stringify(f1) === JSON.stringify(f2)); on any mismatch discards BOTH responses and re-reads.
  export async function fetchMultiStandCFence(activeAreaId, singleLaneMode) {
    for (let attempt = 0; attempt < 3; attempt++) {
      let ovRes;
      try {
        ovRes = await fetch('api/multi/stand', { method: 'GET', cache: 'no-store', credentials: 'omit', redirect: 'error' });
      } catch (err) {
        return { kind: 'transport', detail: String((err && err.message) || err) };
      }
      let overview = null;
      try { overview = await ovRes.json(); } catch { overview = null; }
      if (ovRes.status === 503 && overview && overview.code === 'MultiViewPayloadBudgetExceeded') {
        return { kind: 'budget', status: 503, budget: overview };
      }
      if (ovRes.status !== 200 || !overview || !overview.fence || !Array.isArray(overview.areas)) {
        return { kind: 'transport', detail: 'HTTP ' + ovRes.status };
      }
      if (overview.areas.length <= 2) {
        return { kind: 'two-area', overview };
      }

      const lanesList = visibleLaneIds(overview.areas, activeAreaId, Boolean(singleLaneMode));
      const f = overview.fence;
      const qs = 'lanes=' + encodeURIComponent(lanesList.join(',')) +
        '&sessionId=' + encodeURIComponent(f.sessionId) +
        '&planStateRevision=' + encodeURIComponent(String(f.planStateRevision)) +
        '&ecologicalYear=' + encodeURIComponent(String(f.source.ecologicalYear)) +
        '&annualRevision=' + encodeURIComponent(String(f.source.annualRevision)) +
        '&workRevision=' + encodeURIComponent(String(f.source.workRevision));

      let lnRes;
      try {
        lnRes = await fetch('api/multi/stand?' + qs, {
          method: 'GET',
          cache: 'no-store',
          credentials: 'omit',
          redirect: 'error',
        });
      } catch (err) {
        return { kind: 'transport', detail: String((err && err.message) || err) };
      }
      let lanesPayload = null;
      try { lanesPayload = await lnRes.json(); } catch { lanesPayload = null; }
      if (lnRes.status === 503 && lanesPayload && lanesPayload.code === 'MultiViewPayloadBudgetExceeded') {
        return { kind: 'budget', status: 503, budget: lanesPayload };
      }
      if (lnRes.status === 200 && lanesPayload && lanesPayload.fence && lanesPayload.view &&
          JSON.stringify(overview.fence) === JSON.stringify(lanesPayload.fence)) {
        expandCompactLanePayload(lanesPayload);
        return { kind: 'ok', overview, lanesPayload, requestedLanes: lanesList };
      }
      // Fence moved concurrently between overview and lane read: discard both and retry cleanly.
    }
    return { kind: 'transport', detail: 'C-FENCE fence mismatch persisted across retries' };
  }

  // Composes a >=3-area frame strictly from byte-identical C-FENCE overview + lane-filtered responses.
  export function applyComposedStandFrame(overview, lanesPayload, keepOverlap, preferredAreaId) {
    if (!overview || !overview.fence || !lanesPayload || !lanesPayload.fence || !lanesPayload.view) return false;
    if (JSON.stringify(overview.fence) !== JSON.stringify(lanesPayload.fence)) return false;
    expandCompactLanePayload(lanesPayload);
    state.standOverview = overview;
    if (preferredAreaId && lanesPayload.view.areas && lanesPayload.view.areas.some((a) => a.areaId === preferredAreaId)) {
      state.areaId = preferredAreaId;
    }
    const composed = {
      view: lanesPayload.view,
      scenes: lanesPayload.scenes,
      stage4: lanesPayload.stage4,
      ecology: lanesPayload.ecology,
      labels: overview.labels || state.labels,
      historyTruncated: lanesPayload.historyTruncated,
      totalRecordedYears: lanesPayload.totalRecordedYears,
      reviewPreview: overview.reviewPreview,
      projectionCompacted: lanesPayload.projectionCompacted,
      stage4Compacted: lanesPayload.stage4Compacted,
      payloadBudgetExceeded: false,
      economyEnabled: lanesPayload.economyEnabled,
      economy: lanesPayload.economy,
      hasUnsavedProgress: lanesPayload.hasUnsavedProgress,
      experimentBadge: overview.experimentBadge || state.experimentBadge,
      recipe: 'recipe' in lanesPayload ? lanesPayload.recipe : overview.recipe,
    };
    applyPayload(composed, keepOverlap);
    return true;
  }

  // Fetches and applies a C-FENCE lane-filtered frame for >=3-area stands with monotonic request superseding.
  export async function refreshMultiStandFrame(activeAreaId, singleLaneMode, what, options) {
    const opts = options || {};
    const req = ++state.laneRequest;
    const outcome = await fetchMultiStandCFence(activeAreaId, singleLaneMode);
    if (req !== state.laneRequest) return { kind: 'superseded' };
    if (outcome.kind === 'ok') {
      const applied = applyComposedStandFrame(outcome.overview, outcome.lanesPayload, Boolean(opts.keepOverlap), activeAreaId);
      return applied ? outcome : { kind: 'transport', detail: 'C-FENCE frame fence mismatch' };
    }
    if (outcome.kind === 'budget' && !opts.silentBudget) {
      showBudgetExceeded(what || 'Read lanes', outcome.budget);
    }
    return outcome;
  }

  export function showBudgetExceeded(what, budgetPayload) {
    const bytes = budgetPayload && typeof budgetPayload.bytes === 'number' ? budgetPayload.bytes : 0;
    const budget = budgetPayload && typeof budgetPayload.budget === 'number' ? budgetPayload.budget : 0;
    showAlert([
      el('p', { class: 'alert-title' }, 'Payload budget exceeded: ' + what),
      el('p', null, el('code', null, 'MultiViewPayloadBudgetExceeded'),
        ' (HTTP 503) — response size ' + bytes + ' B exceeds the active ' + budget + ' B payload budget. The last good view is kept.'),
      rereadButton(),
    ]);
  }

  export function applyPayload(payload, keepOverlap) {
    expandCompactLanePayload(payload);
    const prevTreeTag = state.view ? (state.view.sessionId + ':' + state.view.source.annualRevision + ':' + state.view.source.workRevision) : null;
    state.view = payload.view;
    state.scenes = payload.scenes || {};
    state.stage4 = payload.stage4 || null;
    state.ecology = payload.ecology || null;
    if (typeof payload.economyEnabled === 'boolean') state.economyEnabled = payload.economyEnabled;
    state.economy = state.economyEnabled ? (payload.economy || null) : null;
    if (typeof payload.hasUnsavedProgress === 'boolean') state.hasUnsavedProgress = payload.hasUnsavedProgress;
    if ('recipe' in payload) state.recipe = payload.recipe || null;
    if (payload.experimentBadge) state.experimentBadge = payload.experimentBadge;
    else if (payload.ecology && payload.ecology.experimentBadge) state.experimentBadge = payload.ecology.experimentBadge;
    if (state.standOverview && (!state.view || !Array.isArray(state.standOverview.areas) || state.standOverview.areas.length < 3 ||
        JSON.stringify(state.standOverview.fence) !== JSON.stringify(currentFence()))) {
      state.standOverview = null;
    }
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
      if (areaView(areaId) && !treeIn(areaId, state.selected[areaId])) delete state.selected[areaId];
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
    let outcome;
    let postAction = null;
    try {
      outcome = await call(path, body);
      if (outcome.kind === 'ok') {
        clearAlert();
        state.lastWasReplay = Boolean(outcome.payload.execution && outcome.payload.execution.replayed);
        const useLaneReads = isMultiLaneStand() || (outcome.payload.view && outcome.payload.view.areas && outcome.payload.view.areas.length >= 3);
        if (useLaneReads) {
          const frame = await refreshMultiStandFrame(state.areaId, state.singleLaneMode, what);
          if (frame.kind !== 'ok') {
            if (frame.kind !== 'budget' && frame.kind !== 'superseded') {
              showAlert([el('p', { class: 'alert-title' }, 'Lane frame refresh failed after ' + what + '.'), rereadButton()]);
            }
            return;
          }
        } else {
          applyPayload(outcome.payload, false);
        }
        postAction = 'ok';
      } else if (outcome.kind === 'refused') {
        const useLaneReads = isMultiLaneStand() || (outcome.payload.view && outcome.payload.view.areas && outcome.payload.view.areas.length >= 3);
        if (useLaneReads) {
          await refreshMultiStandFrame(state.areaId, state.singleLaneMode, what);
        } else {
          applyPayload(outcome.payload, false);
        }
        postAction = 'refused';
      }
    } finally {
      state.busy = false;
      document.documentElement.classList.remove('busy');
    }
    if (postAction === 'ok') {
      if (onCommitted) onCommitted(outcome.payload);
      if (focusAfter) keepFocusOr(focusAfter);
      hooks.refreshReviewPreview();
      if (state.historyOpen) hooks.loadHistoryIndex(null, false);
    } else if (postAction === 'refused') {
      if (state.lock) showLockAlert('Refused: ' + what + ' (' + outcome.payload.refusal.code + '). Nothing changed.');
      else showRefusal(what, outcome.payload.refusal);
      hooks.refreshReviewPreview();
    } else if (outcome.kind === 'budget') {
      showBudgetExceeded(what, outcome.budget);
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
      case 'EconomyInsufficientCash':
        return 'Upfront cash must cover all external contractor and material costs before approval or execution. Expected timber revenue cannot pre-pay work and no borrowing is allowed.';
      case 'EconomyInsufficientLandownerAllowance':
        return 'Remaining annual landowner hours cannot cover the assigned tasks. Switch some tasks to Contractor or advance to next year to reset the ' +
          (state.economy && state.economy.annualLandownerAllowanceDisplay ? state.economy.annualLandownerAllowanceDisplay + ' ' : '') +
          'annual landowner allowance.';
      case 'EconomyFellContractorOnly':
        return 'Commercial production felling is contractor-only and cannot be assigned to Landowner.';
      case 'EconomyLandownerTaskIneligible':
        return 'That task is outside the low-risk landowner DIY boundary and must be performed by a Contractor.';
      case 'EconomyModelRequired':
        return 'Turn Money On to use economy views, quotes, or labour assignments.';
      case 'EconomyResetConfirmationRequired':
        return 'Confirm discarding unsaved progress before starting a fresh Year-0 session.';
      case 'SaveEconomyRequiresV3':
        return 'Built-in stands (MA-1 / MA-1-MIXED) use frozen Save v2 and cannot be saved while Money is On. Start a Forest Maker woodland for Save v3, or switch Money Off for Save v2.';
      case 'EconomyDispositionUnknown':
        return 'Choose Sell & Extract, Keep for Own Use, or Leave as Deadwood and try again.';
      case 'EconomyDeadwoodRequiresStage4':
        return 'Leave as Deadwood requires Stage 4 fallen-deadwood tracking, which is unavailable on this legacy stand.';
      case 'EconomyQuoteDrift':
      case 'EconomyApprovedQuoteMissing':
        return 'Cancel the approved plan, review the updated quote, and approve again.';
      case 'WoodlandUnsavedProgress':
        return 'Save the current woodland first or confirm discarding unsaved progress.';
      case 'MultiViewPayloadBudgetExceeded':
        return 'The view payload exceeded its byte limit; the previous valid view was kept.';
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
    const econBlocked = isEconomyEnabled() && (!state.economy || (state.economy.activeQuote && !state.economy.activeQuote.isEligible));
    if (econBlocked) {
      if (!planLocked) byId('approve').disabled = true;
      else {
        byId('execute').disabled = true;
        byId('execute-confirm').disabled = true;
      }
    }
    const econToggle = byId('economy-toggle');
    if (econToggle) econToggle.disabled = state.batch !== null || state.queueRunning || state.lock !== null;
    for (const button of document.querySelectorAll('[data-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-prune-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-clear-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-planting-remove]')) button.disabled = blocked;
    for (const button of document.querySelectorAll('[data-executor-value]')) button.disabled = blocked || planLocked;
    for (const button of document.querySelectorAll('[data-disposition-value]')) button.disabled = blocked || planLocked;
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

