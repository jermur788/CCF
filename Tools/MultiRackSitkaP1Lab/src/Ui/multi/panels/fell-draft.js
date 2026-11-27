// S0c provenance: panels/fell-draft.js [L4] — section(s) 13 from multi.js
import { addAvailability, announce, applyPayload, call, currentFence, lockCommands, refreshMultiStandFrame, showBudgetExceeded } from '../api.js';
import { el, fixed, label, listIds, plural } from '../dom.js';
import { DEFAULT_REASON, SPECIES_SHORT, areaName, areaView, blockedReason, byId, currentReport, draftOf, hooks, isMultiLaneStand, isStage4, livingIn, referenceIn, selectedId, shortBlockedReason, speciesOf, state } from '../store.js';
  // ---- Competition and Fell draft -------------------------------------------------------------------------------------

  export function competitionTag() { return JSON.stringify([state.view ? currentFence() : null, state.competitionTarget]); }
  export function clearCompetition() {
    state.competitionTarget = null;
    state.competition = null;
    state.competitionTag = null;
    state.competitionRequest += 1;
  }

  export async function refreshCompetition() {
    const target = state.competitionTarget;
    if (!state.view || !target || state.lock) return;
    const tag = competitionTag();
    const request = ++state.competitionRequest;
    state.competition = null;
    hooks.renderAll();
    const outcome = await call('api/multi/competition', { fence: currentFence(), areaId: target.areaId, treeId: target.treeId });
    if (request !== state.competitionRequest || tag !== competitionTag() || state.lock || state.busy) return;
    if (outcome.kind === 'ok') {
      state.competition = { areaId: outcome.payload.areaId, report: outcome.payload.competition };
      state.competitionTag = tag;
      const dropped = reconcileDrafts();
      hooks.renderAll();
      if (dropped) announce(dropped);
    } else {
      clearCompetition();
      if (outcome.kind === 'budget') {
        showBudgetExceeded('Competition', outcome.budget);
      } else if (outcome.kind === 'refused') {
        if (isMultiLaneStand()) await refreshMultiStandFrame(state.areaId, state.singleLaneMode, 'Competition');
        else applyPayload(outcome.payload, false);
      }
      hooks.renderAll();
      byId('competition-content').append(el('p', { class: 'note' }, 'Competition unavailable or the view changed. Select the tree again.'));
    }
  }

  function inspectedCrop() {
    const comp = state.competition;
    if (!comp || state.lock) return null;
    const area = areaView(comp.areaId);
    return area && area.cropTreeIds.includes(comp.report.targetTreeId) && !comp.report.targetPlannedForFell ? comp : null;
  }

  function releaseReason() {
    const comp = inspectedCrop();
    return comp && comp.areaId === state.areaId && comp.report.targetTreeId === referenceIn(state.areaId)
      ? 'Competition release for retained Crop Tree ' + comp.report.targetTreeId + ' (operator intent)' : '';
  }

  export function renderCompetition() {
    const box = byId('competition-content');
    const areaId = state.areaId;
    const id = selectedId();
    const report = currentReport();
    const parts = [];
    const more = [];
    let sub = '';
    if (report) {
      const chosen = new Set(draftOf(areaId));
      const planLocked = state.view.approved !== null;
      const frozenNow = state.lock !== null || state.batch !== null;
      sub = '· ' + (report.topFive.length < 5 ? report.topFive.length + ' of 5 · ' : '') +
        report.neighbours.length + ' within ' + report.cutoffM + ' m';
      if (report.topFive.length === 0) {
        parts.push(el('p', { class: 'comp-empty' }, 'No living competitor inside the ' + report.cutoffM + ' m cutoff.'));
      } else {
        parts.push(el('p', { class: 'comp-head', 'aria-hidden': 'true' }, el('span', null, ''), el('span', null, 'Tree · DBH · CI share'),
          el('span', null, 'Distance'), el('span', null, 'Hegyi CI')));
        parts.push(el('ol', { class: 'comp-rows', 'aria-label': 'Five strongest competitors of ' + report.targetTreeId },
          ...report.topFive.map((n, index) => {
            const why = blockedReason(areaId, n.treeId);
            const ticked = chosen.has(n.treeId) && !why;
            const disabled = Boolean(why) || planLocked || frozenNow;
            const input = el('input', { type: 'checkbox', 'data-draft': n.treeId, 'data-focus-key': 'draft:' + n.treeId,
              checked: ticked, disabled });
            const spTag = SPECIES_SHORT[speciesOf(n.treeId)] || 'S';
            const meta = spTag + ' · ' + fixed(n.dbhCm, 1) + ' cm · ' + fixed(n.sharePercent, 1) + '%';
            return el('li', { class: 'comp-row' + (ticked ? ' chosen' : '') + (why ? ' blocked' : '') },
              el('label', why ? { title: 'Competitor ' + (index + 1) + ' · ' + n.treeId + ' · ' + why + ' · ' + meta } : null, input,
                el('span', { class: 'comp-id' }, el('span', { class: 'rank' }, String(index + 1)), el('span', { class: 'mono' }, n.treeId)),
                el('span', { class: 'comp-num comp-dist' }, fixed(n.distanceM, 2) + ' m'),
                el('span', { class: 'comp-num comp-ci' }, fixed(n.contribution, 4), el('span', { class: 'comp-strength' }, n.strength)),
                el('span', { class: 'comp-line' },
                  why ? el('span', { class: 'comp-why', 'aria-hidden': 'true' }, shortBlockedReason(why)) : null,
                  why ? el('span', { class: 'sr-only' }, 'Cannot be chosen: ' + why + '. ') : null,
                  why ? el('span', { 'aria-hidden': 'true' }, ' · ') : null,
                  el('span', { class: 'comp-meta' }, meta))));
          })));
      }
      more.push(el('p', { class: 'competition-totals' }, 'Current total Hegyi CI ' + fixed(report.currentTotal, 4) +
        (report.targetPlannedForFell ? ' · This tree is planned for Fell: no release estimate.' :
          ' → ' + fixed(report.hypotheticalTotal, 4) + ' if its pending or approved neighbours were felled.')));
      more.push(el('p', { class: 'small-note' }, 'The planned figure counts only queued or approved orders, never unsent draft choices.'));
      if (isStage4()) more.push(el('p', { class: 'small-note' }, label('MultiCompetitionAreaOnly') + '.'));
      more.push(el('p', { class: 'small-note' }, 'Other neighbours: ' + report.remainingCount + ' · combined ' +
        fixed(report.remainingSharePercent, 1) + '% share.'));
    } else if (id && livingIn(areaId, id) && state.competitionTarget && !state.lock) {
      parts.push(el('p', { class: 'comp-empty' }, 'Reading competition…'));
    } else if (id && !livingIn(areaId, id)) {
      parts.push(el('p', { class: 'comp-empty' }, 'Competition is shown for living trees only.'));
    } else if (state.lock) {
      parts.push(el('p', { class: 'comp-empty' }, 'Re-read the current state to see competition again.'));
    }
    byId('competition-sub').textContent = sub;
    byId('competition-sub').hidden = sub === '';
    box.replaceChildren(...parts);
    byId('competition-more').replaceChildren(...more);
    byId('competition-details').hidden = more.length === 0;
    renderAddForm(report);
  }

  function renderAddForm(report) {
    const areaId = state.areaId;
    const form = byId('add-form');
    form.hidden = !report || report.topFive.length === 0;
    if (form.hidden) return;
    const count = draftOf(areaId).filter((id) => !blockedReason(areaId, id)).length;
    const availability = addAvailability();
    byId('draft-count').textContent = state.batch ? 'Adding — please wait' : plural(count, 'selected', 'selected') + ' · draft only';
    const notice = state.draftNotice && state.draftNotice.areaId === areaId ? state.draftNotice.text : '';
    byId('add-button').textContent = state.batch
      ? 'Adding ' + Math.min(state.batch.confirmed.length + 1, state.batch.ids.length) + ' of ' + state.batch.ids.length + '…'
      : count > 0 ? 'Add selected ' + count + ' to Fell plan' : 'Add selected to Fell plan';
    byId('add-caption').textContent = notice || (availability.ok || count === 0 && !state.view.approved && !state.lock
      ? 'Queues proposals only. Approve and Execute are separate.' : availability.why);
    renderReasonPicker();
  }

  export function reasonToSend(forReferenceItself) {
    const note = byId('reason').value.trim();
    return note || (!forReferenceItself && byId('reason-release').checked ? releaseReason() : '') || DEFAULT_REASON;
  }

  export function renderReasonPicker() {
    const release = releaseReason();
    const checkbox = byId('reason-release');
    checkbox.disabled = release === '' || state.batch !== null;
    if (release === '') checkbox.checked = false;
    byId('release-wrap').hidden = release === '';
    byId('release-label').textContent = release === '' ? '' :
      'Record this as competition release for retained Crop Tree ' + state.competition.report.targetTreeId;
    byId('reason').disabled = state.batch !== null;
    const reason = reasonToSend(false);
    const summary = reason.length > 42 ? reason.slice(0, 40) + '…' : reason;
    if (byId('reason-summary').textContent !== summary) byId('reason-summary').textContent = summary;
    const preview = 'Recorded reason for each tree added here: ' + reason;
    if (byId('reason-preview').textContent !== preview) byId('reason-preview').textContent = preview;
    showReasonError(null);
  }

  function showReasonError(message) {
    const box = byId('reason-error');
    box.hidden = message === null;
    box.textContent = message || '';
    byId('reason').setAttribute('aria-invalid', String(message !== null));
  }

  export function resetReasonPicker() {
    byId('reason').value = '';
    byId('reason-release').checked = false;
    byId('reason-details').open = false;
    showReasonError(null);
  }

  export function reconcileDrafts() {
    const notes = [];
    for (const areaId of Object.keys(state.drafts)) {
      const draft = state.drafts[areaId];
      if (!draft) continue;
      if (!areaView(areaId)) continue;
      if (!livingIn(areaId, draft.ref)) {
        if (draft.ids.length > 0) notes.push('Cleared draft for ' + draft.ref + ' (no longer living).');
        delete state.drafts[areaId];
        continue;
      }
      const keep = draft.ids.filter((id) => !blockedReason(areaId, id));
      if (keep.length !== draft.ids.length) {
        const dropped = draft.ids.filter((id) => !keep.includes(id));
        notes.push('Dropped from draft (' + areaName(areaId) + '): ' + dropped.join(', ') + '.');
        draft.ids = keep;
      }
    }
    state.draftNotice = notes.length ? { areaId: state.areaId, text: notes.join(' ') } : null;
    return notes.join(' ');
  }

  export function toggleDraft(treeId, on) {
    const areaId = state.areaId;
    const ref = referenceIn(areaId);
    if (!ref || state.batch || state.lock || state.view.approved) return;
    if (on && blockedReason(areaId, treeId)) return;
    if (state.batchResult) dismissBatchResult(false);
    const draft = state.drafts[areaId] && state.drafts[areaId].ref === ref ? state.drafts[areaId] : { ref, ids: [] };
    const had = draft.ids.includes(treeId);
    if (on && !had) draft.ids = draft.ids.concat(treeId);
    else if (!on && had) draft.ids = draft.ids.filter((id) => id !== treeId);
    state.drafts[areaId] = draft;
    state.draftNotice = null;
    hooks.renderAll();
    announce((on ? 'Added ' : 'Removed ') + treeId + (on ? ' to' : ' from') + ' the Fell draft (' + draft.ids.length + ' selected; not yet queued).');
  }

  export function clearDraft() {
    const areaId = state.areaId;
    const ref = referenceIn(areaId);
    if (!ref || state.batch) return;
    const count = draftOf(areaId).length;
    if (count === 0) return;
    state.drafts[areaId] = { ref, ids: [] };
    state.draftNotice = null;
    hooks.renderAll();
    announce('Cleared ' + plural(count, 'draft choice', 'draft choices') + ' for ' + ref + '.');
  }

  export async function addSequence(areaId, ref, ids, reason, kind) {
    const result = { areaId, ref, kind, ids: ids.slice(), reason, confirmed: [], refused: null, guard: null, unconfirmed: null, unattempted: [] };
    state.batch = result;
    state.batchResult = null;
    state.batchResultOpen = false;
    state.busy = true;
    document.documentElement.classList.add('busy');
    hooks.renderAll();
    let firstNotSent = ids.length;
    for (let index = 0; index < ids.length; index++) {
      const treeId = ids[index];
      const outcome = await call('api/multi/pending/add', { fence: currentFence(), areaId, treeId, reason });
      if (outcome.kind === 'ok') {
        result.confirmed.push(treeId);
        if (state.drafts[areaId] && state.drafts[areaId].ref === ref) {
          state.drafts[areaId].ids = state.drafts[areaId].ids.filter((id) => id !== treeId);
        }
        if (isMultiLaneStand()) await refreshMultiStandFrame(state.areaId, state.singleLaneMode, 'Add to Fell plan');
        else applyPayload(outcome.payload, false);
        continue;
      }
      firstNotSent = index + 1;
      if (outcome.kind === 'refused') {
        result.refused = { treeId, code: outcome.payload.refusal.code, message: outcome.payload.refusal.message };
        if (isMultiLaneStand()) await refreshMultiStandFrame(state.areaId, state.singleLaneMode, 'Add to Fell plan');
        else applyPayload(outcome.payload, false);
      } else if (outcome.kind === 'budget') {
        result.guard = { treeId, status: 503, code: 'MultiViewPayloadBudgetExceeded', message: 'response exceeded active payload budget' };
        showBudgetExceeded('Add to Fell plan', outcome.budget);
      } else if (outcome.kind === 'guard') {
        result.guard = { treeId, status: outcome.status, code: outcome.error.code, message: outcome.error.message };
      } else if (outcome.kind === 'locked') {
        firstNotSent = index;
      } else {
        result.unconfirmed = { treeId, detail: outcome.kind === 'unconfirmed' ? outcome.error.message : outcome.detail };
      }
      break;
    }
    result.unattempted = result.ids.slice(firstNotSent);
    state.batch = null;
    state.busy = false;
    document.documentElement.classList.remove('busy');
    state.batchResult = result;
    if (result.confirmed.length === result.ids.length) resetReasonPicker();
    if (result.unconfirmed) {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        'Adding ' + result.unconfirmed.treeId + ' to the Work Plan: no confirmed result was received.');
    } else {
      hooks.renderAll();
    }
    announce(batchSummary(result));
    if (!result.unconfirmed) {
      const add = byId('add-button');
      const rendered = typeof add.getClientRects !== 'function' || add.getClientRects().length > 0;
      if (add.disabled || add.closest('[hidden]') || !rendered) byId('batch-result').focus();
      else add.focus();
    }
    if (!state.lock && state.competitionTarget && state.competitionTag !== competitionTag()) refreshCompetition();
    if (!state.lock && !result.unconfirmed) hooks.refreshReviewPreview();
  }

  function batchSummary(result) {
    const n = result.ids.length;
    if (result.confirmed.length === n) {
      return 'Added ' + plural(n, 'tree', 'trees') + ' to the Fell plan as pending orders (not felled): ' + listIds(result.confirmed) +
        '. Total pending ' + state.view.pending.length + '.';
    }
    const stopped = result.refused ? 'refused ' + result.refused.treeId : result.guard ? 'refused ' + result.guard.treeId :
      result.unconfirmed ? 'no confirmed result for ' + result.unconfirmed.treeId : 'not sent';
    return 'Stopped after ' + result.confirmed.length + ' of ' + n + ': ' + stopped + '.';
  }

  export function renderBatchResult() {
    const box = byId('batch-result');
    const result = state.batchResult;
    const show = result && result.areaId === state.areaId && result.ref === referenceIn(state.areaId);
    box.hidden = !show;
    byId('add-dock').classList.toggle('showing-result', Boolean(show));
    if (!show) { box.replaceChildren(); return; }
    const n = result.ids.length;
    const complete = result.confirmed.length === n;
    box.setAttribute('class', 'batch-result' + (complete ? '' : result.confirmed.length > 0 ? ' partial' : ' failed') +
      (state.batchResultOpen ? ' open' : ''));
    const stopped = result.refused ? ' · refused ' + result.refused.treeId : result.guard ? ' · refused ' + result.guard.treeId :
      result.unconfirmed ? ' · unconfirmed ' + result.unconfirmed.treeId : '';
    const title = complete ? 'Added ' + plural(n, 'tree', 'trees') + ' to the Fell plan' : 'Stopped after ' + result.confirmed.length + ' of ' + n + stopped;
    const head = el('div', { class: 'batch-head' }, el('p', { class: 'alert-title', title }, title),
      el('button', { type: 'button', class: 'link-button batch-more', 'data-batch': 'more', 'aria-expanded': String(Boolean(state.batchResultOpen)),
        'aria-controls': 'batch-body' }, state.batchResultOpen ? 'Hide details' : 'Details'),
      el('button', { type: 'button', class: 'link-button', 'data-batch': 'dismiss', 'aria-label': 'Dismiss the Add result' }, '× Dismiss'));
    const parts = [el('p', null, 'Added as pending (not felled): ', el('span', { class: 'mono' }, listIds(result.confirmed)))];
    if (result.refused) {
      parts.push(el('p', null, 'Refused: ', el('span', { class: 'mono' }, result.refused.treeId), ' — ',
        el('code', null, result.refused.code), ' ' + result.refused.message));
    }
    if (result.guard) {
      parts.push(el('p', null, 'Refused by server: ', el('span', { class: 'mono' }, result.guard.treeId), ' — ',
        el('code', null, result.guard.code), ' (HTTP ' + result.guard.status + ') ' + result.guard.message));
    }
    if (result.unconfirmed) {
      parts.push(el('p', null, 'Unconfirmed: ', el('span', { class: 'mono' }, result.unconfirmed.treeId)));
    }
    if (result.unattempted.length > 0) {
      parts.push(el('p', null, 'Not attempted: ', el('span', { class: 'mono' }, listIds(result.unattempted))));
    }
    box.replaceChildren(head, el('div', { id: 'batch-body', class: 'batch-body' }, ...parts));
  }

  function dismissBatchResult(focusAdd) {
    if (!state.batchResult) return;
    state.batchResult = null;
    state.batchResultOpen = false;
    renderBatchResult();
    if (focusAdd) {
      const add = byId('add-button');
      if (!add.disabled && !add.closest('[hidden]')) add.focus();
      else byId('inspector-heading').focus();
    }
  }

  export function onBatchResultClick(event) {
    const button = event.target.closest('[data-batch]');
    if (!button) return;
    if (button.getAttribute('data-batch') === 'dismiss') {
      const added = state.batchResult ? state.batchResult.confirmed.length : 0;
      dismissBatchResult(true);
      announce('Dismissed the Add result.' + (added > 0 ? ' Orders already added stay pending in the Work Plan.' : ''));
      return;
    }
    state.batchResultOpen = !state.batchResultOpen;
    renderBatchResult();
    const again = byId('batch-result').querySelector('[data-batch="more"]');
    if (again) again.focus();
  }

