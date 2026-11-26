// S0c provenance: panels/time-history.js [L4] — section(s) 17a from multi.js
import { announce, applyPayload, call, clearAlert, currentFence, lockCommands, recoveryFor, refusedByLock, showAlert, showGuardRefusal, showLockAlert, showRefusal } from '../api.js';
import { reread } from '../commands.js';
import { jsonCall, noticeOf } from '../dialogs/saves.js';
import { el, fixed, label, lightIndexText, plural, table } from '../dom.js';
import { areaShort, byId, draftTotal, isEconomyEnabled, pct1, speciesLabel, state } from '../store.js';
import { showPane } from './work-plan.js';
  // ---- Phase 1 Step 2: Skip to Review, On-Demand History Drawer, and On-Demand Structure Panel -----------------------

  const PROV_META = {
    Recorded: ['RECORDED', 'prov-recorded', 'L1 Recorded historical fact — committed when work executed or an annual step closed'],
    Current: ['CURRENT', 'prov-current', 'L2 Current observed state — read directly from the current committed root'],
    Derived: ['DERIVED', 'prov-derived', 'L3 Derived comparison — computed deterministically from recorded and/or current state'],
    Reconstructed: ['RECONSTRUCTED', 'prov-reconstructed', 'L3 Reconstructed — cross-referenced from related committed records'],
    SessionOnly: ['SESSION-ONLY', 'prov-session-only', 'L2 Session-only — ephemeral session read-out not archived per past year'],
    NotRecorded: ['NOT RECORDED', 'prov-not-recorded', 'Absent — not stored by the 2D engine and never fabricated from current state'],
    RECORDED: ['RECORDED', 'prov-recorded', 'L1 Recorded historical fact'],
    CURRENT: ['CURRENT', 'prov-current', 'L2 Current observed state'],
    DERIVED: ['DERIVED', 'prov-derived', 'L3 Derived comparison'],
    RECONSTRUCTED: ['RECONSTRUCTED', 'prov-reconstructed', 'L3 Reconstructed'],
    'SESSION-ONLY': ['SESSION-ONLY', 'prov-session-only', 'L2 Session-only'],
    'NOT RECORDED': ['NOT RECORDED', 'prov-not-recorded', 'Absent (NOT RECORDED — never fabricated)'],
  };

  function provPill(value, customTitle) {
    const [text, cls, defTitle] = PROV_META[value] || [String(value || ''), 'prov-derived', ''];
    return el('span', { class: 'prov-pill ' + cls, title: customTitle || defTitle }, text);
  }

  export function renderSkipControl() {
    const btn = byId('skip-review');
    const lbl = byId('skip-review-label');
    const sub = byId('skip-review-sub');
    if (!btn || !lbl || !sub) return;
    const p = state.reviewPreview;
    if (!p) {
      lbl.textContent = 'Skip to review';
      sub.textContent = 'GAMEPLAY CALIBRATION';
      btn.classList.remove('blocked');
      return;
    }
    const targetText = 'Skip → Yr ' + p.suggestedTargetYear + (p.isObservationPeriod ? ' (Obs.)' : '');
    lbl.textContent = targetText;
    if (!p.eligible && p.stopReason === 'WorkPlanOpen') {
      sub.textContent = (p.isObservationPeriod ? 'Quiet obs. · ' : '') + 'Work plan open · GAMEPLAY CALIBRATION';
    } else if (!p.eligible && p.stopReason === 'StandCannotAdvance') {
      sub.textContent = 'Stand cannot advance · GAMEPLAY CALIBRATION';
    } else if (p.isObservationPeriod) {
      sub.textContent = 'Quiet observation · GAMEPLAY CALIBRATION';
    } else if (p.anchorFellYear !== null && p.anchorFellYear !== undefined) {
      sub.textContent = (p.isOverdue ? 'Review (overdue) · ' : 'Review (Fell Yr ' + p.anchorFellYear + ') · ') + 'GAMEPLAY CALIBRATION';
    } else {
      sub.textContent = 'GAMEPLAY CALIBRATION';
    }
    btn.classList.toggle('blocked', !p.eligible);
    btn.setAttribute('title', (p.label ? p.label + ' ' : '') + (p.calibrationNote || ''));
  }

  export async function refreshReviewPreview() {
    if (!state.view || state.lock) return;
    const req = ++state.previewRequest;
    const outcome = await jsonCall('api/multi/year/preview-review', 'POST', { fence: currentFence() });
    if (req !== state.previewRequest || state.lock) return;
    if (outcome.kind === 'ok' && outcome.payload && typeof outcome.payload.suggestedTargetYear === 'number') {
      state.reviewPreview = outcome.payload;
      renderSkipControl();
    }
  }

  export function renderSkipSummary() {
    const box = byId('skip-summary');
    if (!box) return;
    const s = state.lastSkip;
    box.hidden = !s;
    if (!s) { box.replaceChildren(); return; }
    const stepText = 'Year ' + s.startYear + ' → Year ' + s.destinationYear + ' (' + plural(s.completedYearsCount, 'year stepped', 'years stepped') + ')';
    const periodNote = s.isObservationPeriod
      ? 'Quiet observation period (pre-first-Fell) — not a completed silvicultural thinning cycle.'
      : (s.anchorFellYear !== null && s.anchorFellYear !== undefined
        ? 'Anchored to last recorded Fell in Year ' + s.anchorFellYear + '.'
        : 'Management review interval.');
    box.replaceChildren(...[
      el('p', { class: 'alert-title' }, 'Skip summary: ' + stepText + ' · Stop reason: ', el('code', null, s.stopReason)),
      s.stopDetail ? el('p', null, s.stopDetail) : null,
      el('p', { class: 'small-note' }, periodNote),
      el('p', { class: 'small-note' }, s.calibrationNote),
    ].filter(Boolean));
  }

  export async function onSkipReview() {
    if (!state.view || state.busy || state.batch || state.queueRunning || refusedByLock('Skip to review')) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const previewOutcome = await jsonCall('api/multi/year/preview-review', 'POST', { fence: currentFence() });
    if (previewOutcome.kind !== 'ok' || !previewOutcome.payload) {
      state.busy = false;
      document.documentElement.classList.remove('busy');
      if (previewOutcome.kind === 'refused' && previewOutcome.payload && previewOutcome.payload.refusal) {
        showRefusal('Skip to review (preview)', previewOutcome.payload.refusal);
        reread(false);
      } else if (previewOutcome.kind === 'guard') {
        const notice = noticeOf(previewOutcome);
        showGuardRefusal('Skip to review (preview)', previewOutcome.status || 400, notice);
      } else {
        lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
          'Skip preview: no confirmed result was received.');
      }
      return;
    }

    const preview = previewOutcome.payload;
    state.reviewPreview = preview;
    renderSkipControl();

    if (!preview.eligible) {
      state.busy = false;
      document.documentElement.classList.remove('busy');
      const code = preview.stopReason || 'WorkPlanOpen';
      const viewPlanBtn = code === 'WorkPlanOpen' ? el('button', { type: 'button', class: 'secondary small' }, 'Open Work Plan') : null;
      if (viewPlanBtn) viewPlanBtn.addEventListener('click', () => { showPane('plan'); byId('plan-heading').focus(); });
      showAlert([
        el('p', { class: 'alert-title' }, 'Refused: Skip to review (' + code + ')'),
        el('p', null, el('code', null, code), ' — ' + (code === 'WorkPlanOpen'
          ? 'Open Pending or Approved work must be executed, cancelled, or removed before skipping ahead. Skip never cancels open work.'
          : (preview.label || recoveryFor(code)))),
        el('p', { class: 'small-note' }, preview.calibrationNote),
        el('p', null, label('MultiNothingChanged') + '. Current ecological year ' + state.view.source.ecologicalYear + '.'),
        viewPlanBtn,
      ]);
      announce('Refused Skip to review (' + code + '). ' + (code === 'WorkPlanOpen' ? 'Resolve open Pending or Approved work first.' : '') + ' Nothing changed.');
      return;
    }

    const skipOutcome = await call('api/multi/year/skip', { fence: currentFence(), targetYear: preview.suggestedTargetYear });
    state.busy = false;
    document.documentElement.classList.remove('busy');

    if (skipOutcome.kind === 'ok') {
      const p = skipOutcome.payload;
      clearAlert();
      const drafts = draftTotal();
      state.drafts = {};
      state.draftNotice = null;
      state.batchResult = null;
      state.cellDetail = null;
      state.plantDraft = null;
      state.plantResult = null;
      state.spotDraft = null;
      state.spotResult = null;
      state.lastSkip = {
        startYear: p.startYear,
        destinationYear: p.destinationYear,
        requestedTargetYear: p.requestedTargetYear,
        completedYearsCount: (p.completedYears || []).length,
        stopReason: p.stopReason,
        stopDetail: p.stopDetail || null,
        isObservationPeriod: Boolean(p.isObservationPeriod),
        anchorFellYear: p.anchorFellYear,
        calibrationNote: p.calibrationNote || preview.calibrationNote,
      };
      applyPayload(p, false);
      showPane('history');
      byId('year-heading').focus();
      announce('Skipped Year ' + p.startYear + ' → Year ' + p.destinationYear + ' (' +
        plural((p.completedYears || []).length, 'year', 'years') + ', ' + p.stopReason + ').' +
        (drafts ? ' ' + plural(drafts, 'unsent draft choice was', 'unsent draft choices were') + ' cleared.' : ''));
      refreshReviewPreview();
      if (state.historyOpen) loadHistoryIndex(null, false);
    } else if (skipOutcome.kind === 'refused') {
      applyPayload(skipOutcome.payload, false);
      if (state.lock) showLockAlert('Refused: Skip to review (' + skipOutcome.payload.refusal.code + '). Nothing changed.');
      else showRefusal('Skip to review', skipOutcome.payload.refusal);
      refreshReviewPreview();
    } else if (skipOutcome.kind === 'guard') {
      showGuardRefusal('Skip to review', skipOutcome.status, skipOutcome.error);
    } else if (skipOutcome.kind === 'locked') {
      showLockAlert('Not sent: Skip to review.');
    } else {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        'Skip to review: no confirmed result was received. Do not assume that it happened, or that it did not.');
    }
  }

  export function openHistoryDrawer() {
    if (!state.view) return;
    state.historyOpen = true;
    byId('history-backdrop').hidden = false;
    byId('history-drawer').hidden = false;
    byId('history-open').setAttribute('aria-expanded', 'true');
    byId('history-drawer-title').focus();
    loadHistoryIndex(null, false);
  }

  export function closeHistoryDrawer(returnToOpener) {
    if (!state.historyOpen) return false;
    state.historyOpen = false;
    byId('history-backdrop').hidden = true;
    byId('history-drawer').hidden = true;
    byId('history-open').setAttribute('aria-expanded', 'false');
    if (returnToOpener) {
      const opener = byId('history-open');
      if (opener && opener.isConnected && !opener.disabled) opener.focus();
    }
    announce('Closed History drawer. Draft Fell ticks preserved.');
    return true;
  }

  export async function loadHistoryIndex(beforeYear, append) {
    if (!state.view) return;
    const req = ++state.historyRequest;
    state.historyLoading = true;
    state.historyError = null;
    if (!append) {
      state.historyDetailRequest++;
      state.historyDetailLoading = false;
      state.historyItems = [];
      state.historyNextCursor = null;
      state.historySelectedYear = null;
      state.historyDetail = null;
      state.historyEconomyDetail = null;
      state.historyEconomyError = null;
    }
    renderHistoryDrawer();
    const qs = beforeYear === null || beforeYear === undefined
      ? 'api/multi/history/index?pageSize=10'
      : 'api/multi/history/index?pageSize=10&beforeYear=' + encodeURIComponent(String(beforeYear));
    const outcome = await jsonCall(qs, 'GET');
    if (req !== state.historyRequest) return;
    state.historyLoading = false;
    if (outcome.kind === 'ok' && outcome.payload && Array.isArray(outcome.payload.items)) {
      const incoming = outcome.payload.items;
      if (append) {
        const seen = new Set(state.historyItems.map((item) => item.year));
        state.historyItems = state.historyItems.concat(incoming.filter((item) => !seen.has(item.year)));
      } else {
        state.historyItems = incoming;
      }
      state.historyNextCursor = outcome.payload.nextCursorBeforeYear === undefined ? null : outcome.payload.nextCursorBeforeYear;
      state.historyTotalRecorded = outcome.payload.totalRecordedYears || 0;
      state.historyError = null;
    } else {
      const notice = noticeOf(outcome);
      state.historyError = 'Could not load history index (' + notice.code + '): ' + notice.message;
    }
    renderHistoryDrawer();
  }

  export async function loadHistoryYear(year) {
    if (!state.view || state.lock) return;
    const req = ++state.historyDetailRequest;
    const reqSession = state.view.sessionId;
    state.historySelectedYear = year;
    state.historyDetailLoading = true;
    state.historyDetail = null;
    state.historyError = null;
    state.historyEconomyDetail = null;
    state.historyEconomyError = null;
    renderHistoryDrawer();
    const fence = currentFence();
    const econEnabled = isEconomyEnabled();
    const calls = [jsonCall('api/multi/history/year', 'POST', { fence, year: Number(year) })];
    if (econEnabled) calls.push(jsonCall('api/multi/economy/history/year', 'POST', { fence, year: Number(year) }));
    const [outcome, econOutcome] = await Promise.all(calls);
    if (req !== state.historyDetailRequest || !state.view || state.view.sessionId !== reqSession) return;
    state.historyDetailLoading = false;
    if (outcome.kind === 'ok' && outcome.payload && typeof outcome.payload.workYear === 'number') {
      state.historyDetail = outcome.payload;
      state.historyError = null;
      if (econEnabled && econOutcome) {
        if (econOutcome.kind === 'ok' && econOutcome.payload && typeof econOutcome.payload.year === 'number') {
          state.historyEconomyDetail = econOutcome.payload;
        } else {
          const eNotice = noticeOf(econOutcome);
          state.historyEconomyError = 'Financial ledger unavailable (' + eNotice.code + '): ' + eNotice.message;
        }
      }
    } else {
      const notice = noticeOf(outcome);
      state.historyError = 'History year ' + year + ' refused (' + notice.code + '): ' + notice.message;
      state.historyDetail = null;
    }
    renderHistoryDrawer();
  }

  function formatActionSummary(counts) {
    if (!counts) return 'No work';
    const parts = [];
    if (counts.fell > 0) parts.push(counts.fell + ' Fell');
    if (counts.prune > 0) parts.push(counts.prune + ' Prune');
    if (counts.clearUndergrowth > 0) parts.push(counts.clearUndergrowth + ' Clear');
    if (counts.plant > 0) parts.push(counts.plant + ' Plant');
    if (counts.shelter > 0) parts.push(counts.shelter + ' Shelter');
    if (counts.spotClear > 0) parts.push(counts.spotClear + ' Spot');
    return parts.length > 0 ? parts.join(' · ') : 'No work';
  }

  export function renderHistoryDrawer() {
    if (!state.historyOpen) return;
    byId('history-drawer-count').textContent = plural(state.historyTotalRecorded, 'recorded year', 'recorded years');
    const status = byId('history-drawer-status');
    status.textContent = state.historyError || (state.historyLoading ? 'Loading recorded years…' :
      state.historyDetailLoading ? 'Reading Year ' + state.historySelectedYear + ' detail…' : '');

    const list = byId('history-year-list');
    if (state.historyItems.length === 0) {
      list.replaceChildren(el('li', { class: 'small-note' }, state.historyLoading
        ? 'Loading…'
        : 'No recorded work or completed annual years yet. Execute a Work Plan or advance a year to start history.'));
    } else {
      list.replaceChildren(...state.historyItems.map((entry) => {
        const selected = state.historySelectedYear === entry.year;
        const stemsNote = entry.hasAnnualOutcome && entry.liveStemsEndOfYear !== null && entry.liveStemsEndOfYear !== undefined
          ? 'End stems: ' + entry.liveStemsEndOfYear
          : 'No annual step ending in Yr ' + entry.year;
        const nextNote = entry.hasObservedOutcomeNextYear ? ' · Obs. Yr ' + (entry.year + 1) : '';
        return el('li', null, el('button', {
          type: 'button',
          class: 'history-year-btn',
          'data-history-year': String(entry.year),
          'aria-current': selected ? 'true' : null,
        },
          el('span', { class: 'history-year-row-top' },
            el('span', null, 'Year ' + entry.year),
            el('span', { class: 'tag' }, entry.hasExecutedWork ? 'Work' : 'Quiet')),
          el('span', { class: 'history-year-row-sub' }, formatActionSummary(entry.executedActionCounts)),
          el('span', { class: 'history-year-row-sub' }, stemsNote + nextNote)));
      }));
    }

    byId('history-index-foot').textContent = state.historyTotalRecorded > 0
      ? 'Showing ' + state.historyItems.length + ' of ' + state.historyTotalRecorded + ' recorded years (newest first).'
      : '';
    const loadOlder = byId('history-load-older');
    loadOlder.hidden = state.historyNextCursor === null || state.historyNextCursor === undefined;
    loadOlder.disabled = state.historyLoading;

    const detailPane = byId('history-detail-pane');
    if (state.historyDetailLoading) {
      detailPane.replaceChildren(el('p', { class: 'small-note' }, 'Loading Decision → Outcome detail for Year ' + state.historySelectedYear + '…'));
      return;
    }
    if (!state.historyDetail) {
      detailPane.replaceChildren(el('p', { class: 'small-note' }, state.historyError
        ? state.historyError
        : 'Select a recorded year on the left to inspect Work Executed (Year Y) vs Observed Outcome (Year Y+1).'));
      return;
    }
    detailPane.replaceChildren(...renderHistoryDetailNodes(state.historyDetail));
  }

  function renderHistoryDetailNodes(d) {
    const cards = [];

    // 1. Work Executed (WorkYear = Y) — clearly separated from Observed Outcome (Year Y + 1).
    const workParts = [
      el('div', { class: 'history-card-head' },
        el('h3', null, 'Work Executed — Year ' + d.workYear),
        el('div', { class: 'history-pills' }, provPill(d.workProvenance || 'RECORDED'))),
      el('p', { class: 'small-note' }, '2D timing: Execute commits inside WorkYear ' + d.workYear + ' without advancing the calendar; ecological response appears in Observed Year ' + d.observedYear + '.'),
    ];
    const wc = d.totalWorkCounts || {};
    const totalActions = (wc.fell || 0) + (wc.prune || 0) + (wc.clearUndergrowth || 0) + (wc.plant || 0) + (wc.shelter || 0) + (wc.spotClear || 0);
    if (totalActions === 0) {
      workParts.push(el('p', { class: 'small-note' }, 'No work executed in Year ' + d.workYear + '.'));
    } else {
      workParts.push(el('p', null, 'Total executed in Year ' + d.workYear + ': ', el('strong', null, formatActionSummary(wc))));
      const workRows = [];
      for (const area of (d.workByArea || [])) {
        for (const grp of (area.actionGroups || [])) {
          const ids = (grp.treeIds && grp.treeIds.length ? grp.treeIds : [])
            .concat((grp.cells || []).map((c) => '(' + c.column + ',' + c.row + ')'))
            .concat(grp.juvenileIds || [])
            .concat(grp.shelterIds || [])
            .concat(grp.patchIds || [])
            .concat(grp.orderIds || []);
          workRows.push([areaShort(area.areaId), grp.actionType, String(grp.count), ids.join(', ') || '–']);
        }
      }
      if (workRows.length > 0) {
        workParts.push(table('Work executed in Year ' + d.workYear, ['Area', 'Action', 'Count', 'Recorded IDs / cells'], workRows));
      }
    }
    if (d.plantedWorkFollowUp && d.plantedWorkFollowUp.length > 0) {
      workParts.push(el('div', { class: 'history-card-head' },
        el('h4', null, 'Planted juvenile follow-up (planted in Year ' + d.workYear + ')'),
        provPill('RECONSTRUCTED')));
      workParts.push(table('Reconstructed planted juvenile follow-up', ['Juvenile', 'Area', 'Species', 'Current status'],
        d.plantedWorkFollowUp.map((f) => [
          f.juvenileId,
          areaShort(f.areaId),
          speciesLabel(f.speciesId),
          f.promotedTreeId ? 'Promoted → ' + f.promotedTreeId
            : f.currentlyActive ? 'Alive (active juvenile)'
              : 'Dead (Yr ' + f.deathYear + (f.deathCause ? ', ' + f.deathCause : '') + ')',
        ])));
    }
    cards.push(el('section', { class: 'history-card history-card-work', 'data-history-section': 'work-executed' }, ...workParts));

    // 2. Observed Outcome (ObservedYear = Y + 1) — clearly separated from Work Executed (Year Y).
    const outParts = [
      el('div', { class: 'history-card-head' },
        el('h3', null, 'Observed Outcome — Year ' + d.workYear + ' → Year ' + d.observedYear),
        el('div', { class: 'history-pills' }, provPill((d.annualOutcomeNextYear && d.annualOutcomeNextYear.provenance) || 'RECORDED'))),
    ];
    if (!d.annualOutcomeNextYear) {
      outParts.push(el('p', { class: 'small-note' }, 'Observed Year ' + d.observedYear + ' transition (' + d.workYear + ' → ' + d.observedYear + ') has not been completed yet.'));
    } else {
      const out = d.annualOutcomeNextYear;
      outParts.push(el('p', { class: 'small-note' }, 'Annual revision ' + out.annualRevisionAfter + ' · work revision ' + out.workRevision +
        (out.cancelledUnexecutedWork ? ' · unexecuted work cancelled by consent at zero Fell.' : '.')));
      outParts.push(table('Observed ecological outcome (Year ' + out.yearBefore + ' → Year ' + out.yearAfter + ')',
        ['Area', 'Living stems', 'Basal area (m²/ha)', 'Natural deaths', 'Mean bramble / bracken / light'],
        (out.areas || []).map((a) => [
          areaShort(a.areaId),
          a.before.livingCount + ' → ' + a.after.livingCount,
          fixed(a.before.basalAreaM2PerHa, 2) + ' → ' + fixed(a.after.basalAreaM2PerHa, 2),
          a.noLivingStems ? 'no living stems' : String(a.naturalDeathCount),
          (a.meanBramble !== null && a.meanBramble !== undefined ? pct1(a.meanBramble) : '–') + ' / ' +
          (a.meanBracken !== null && a.meanBracken !== undefined ? pct1(a.meanBracken) : '–') + ' / ' +
          (a.meanRegenStepLightIndex !== null && a.meanRegenStepLightIndex !== undefined ? lightIndexText(a.meanRegenStepLightIndex) : '–'),
        ])));
      const regenRows = [];
      for (const a of (out.areas || [])) {
        for (const r of (a.naturalRegeneration || [])) {
          if (r.startingAbundance > 0 || r.endingAbundance > 0 || r.promotionsCount > 0) {
            regenRows.push([areaShort(a.areaId), speciesLabel(r.speciesId), fixed(r.startingAbundance, 3) + ' → ' + fixed(r.endingAbundance, 3), String(r.promotionsCount)]);
          }
        }
      }
      if (regenRows.length > 0) {
        outParts.push(table('Natural regeneration accounts (Observed Year ' + out.yearAfter + ')', ['Area', 'Species', 'Occupancy', 'Promotions'], regenRows));
      }
      if (out.wholeWoodlandPlantedAccounts && out.wholeWoodlandPlantedAccounts.length > 0) {
        outParts.push(el('div', { class: 'history-card-head' },
          el('h4', null, 'Planted juvenile accounts (Whole-woodland scope)'),
          el('div', { class: 'history-pills' }, provPill('RECORDED'), provPill('NOT RECORDED'))));
        outParts.push(table('Whole-woodland planted accounts (per-area split NOT RECORDED)',
          ['Species', 'Scope', 'Start', 'Remaining', 'Promoted', 'Deaths (veg/light/browse)'],
          out.wholeWoodlandPlantedAccounts.map((p) => [
            speciesLabel(p.speciesId),
            p.scope,
            String(p.starting),
            String(p.remaining),
            String(p.promoted),
            p.vegetationDeaths + ' / ' + p.lightDeaths + ' / ' + p.browseDeaths,
          ])));
        outParts.push(el('p', { class: 'small-note' }, provPill('NOT RECORDED'), ' ', out.perAreaPlantedSplitNote));
      }
    }
    if (d.annualRecordEndingInYear) {
      const endRec = d.annualRecordEndingInYear;
      outParts.push(el('details', { class: 'more' },
        el('summary', null, 'Earlier annual transition closing at Year ' + endRec.yearAfter + ' (' + endRec.yearBefore + ' → ' + endRec.yearAfter + ')'),
        table('Annual transition ' + endRec.yearBefore + ' → ' + endRec.yearAfter, ['Area', 'Living stems', 'Basal area (m²/ha)', 'Natural deaths'],
          (endRec.areas || []).map((a) => [areaShort(a.areaId), a.before.livingCount + ' → ' + a.after.livingCount,
            fixed(a.before.basalAreaM2PerHa, 2) + ' → ' + fixed(a.after.basalAreaM2PerHa, 2), String(a.naturalDeathCount)]))));
    }
    cards.push(el('section', { class: 'history-card history-card-outcome', 'data-history-section': 'observed-outcome' }, ...outParts));

    // 3. Later Observed Comparison (CURRENT + DERIVED + non-causal note, no weather mention)
    if (d.laterObservedComparison) {
      const cmp = d.laterObservedComparison;
      const cmpParts = [
        el('div', { class: 'history-card-head' },
          el('h3', null, 'Later Observed Comparison (as of Year ' + cmp.asOfEcologicalYear + ')'),
          el('div', { class: 'history-pills' }, provPill(cmp.currentProvenance || 'CURRENT'), provPill(cmp.deltaProvenance || 'DERIVED'))),
        el('p', { class: 'non-causal-box', 'data-non-causal': 'true' }, cmp.nonCausalStatement),
        table('Current stand vs Observed Year ' + d.observedYear,
          ['Area', 'Current live (CURRENT)', 'Stem delta (DERIVED)', 'Current BA m²/ha', 'BA delta', 'Deadwood logs', 'Planted (act/prom/dead)'],
          (cmp.areas || []).map((a) => [
            areaShort(a.areaId),
            String(a.currentLiveStems),
            a.liveStemDeltaSinceObservedYear === null || a.liveStemDeltaSinceObservedYear === undefined ? '–'
              : (a.liveStemDeltaSinceObservedYear >= 0 ? '+' : '') + a.liveStemDeltaSinceObservedYear,
            a.currentBasalAreaM2PerHa === null || a.currentBasalAreaM2PerHa === undefined ? '–' : fixed(a.currentBasalAreaM2PerHa, 2),
            a.basalAreaM2PerHaDeltaSinceObservedYear === null || a.basalAreaM2PerHaDeltaSinceObservedYear === undefined ? '–'
              : (a.basalAreaM2PerHaDeltaSinceObservedYear >= 0 ? '+' : '') + fixed(a.basalAreaM2PerHaDeltaSinceObservedYear, 2),
            a.currentDeadwoodCount === null || a.currentDeadwoodCount === undefined ? '–' : String(a.currentDeadwoodCount),
            a.plantedJuvenilesAliveActive === null || a.plantedJuvenilesAliveActive === undefined ? '–'
              : a.plantedJuvenilesAliveActive + ' / ' + a.plantedJuvenilesPromoted + ' / ' + a.plantedJuvenilesDead,
          ])),
        el('p', { class: 'small-note' }, provPill('SESSION-ONLY'), ' Current overhead canopy cover and modelled light are session-only live map read-outs and are never back-filled into past years.'),
      ];
      cards.push(el('section', { class: 'history-card history-card-current', 'data-history-section': 'later-comparison' }, ...cmpParts));
    }

    // 3b. Recorded Financial Ledger (folded into the SAME History drawer ONLY when Money is On; untouched when Off).
    if (isEconomyEnabled()) {
      const eh = state.historyEconomyDetail;
      if (eh && eh.year === d.workYear && eh.economyActive) {
        const finParts = [
          el('div', { class: 'history-card-head' },
            el('h3', null, 'Financial Ledger — Year ' + eh.year),
            el('div', { class: 'history-pills' }, provPill(eh.provenanceLabel || 'RECORDED'))),
          table('Year ' + eh.year + ' financial summary', ['Metric', 'Value'], [
            ['Opening cash', eh.openingCashDisplay],
            ['Closing cash', eh.closingCashDisplay],
            ['External costs paid', eh.totalExternalCostDisplay],
            ['Timber revenue credited', eh.totalTimberRevenueDisplay],
            ['Net cash delta', eh.netCashDeltaDisplay],
            ['Landowner time used', eh.landownerMinutesUsedDisplay],
          ]),
        ];
        if (Array.isArray(eh.ledger) && eh.ledger.length > 0) {
          finParts.push(table('Recorded ledger entries — Year ' + eh.year,
            ['Category', 'Reference', 'Cash delta', 'Work rev'],
            eh.ledger.map((row) => [row.categoryLabel, row.referenceId, row.cashDeltaDisplay, String(row.workRevision)])));
        } else {
          finParts.push(el('p', { class: 'small-note' }, 'No financial settlements recorded in Year ' + eh.year + '.'));
        }
        if (eh.disclosure) finParts.push(el('p', { class: 'small-note' }, eh.disclosure));
        cards.push(el('section', { class: 'history-card history-card-finance', 'data-history-section': 'financial-ledger' }, ...finParts));
      } else if (state.historyEconomyError) {
        cards.push(el('section', { class: 'history-card history-card-finance', 'data-history-section': 'financial-ledger' },
          el('div', { class: 'history-card-head' }, el('h3', null, 'Financial Ledger — Year ' + d.workYear)),
          el('p', { class: 'warning' }, state.historyEconomyError)));
      }
    }

    // 4. Honest NOT RECORDED disclosures + Provenance Glossary
    const discParts = [
      el('div', { class: 'history-card-head' },
        el('h3', null, 'Not Recorded in 2D History'),
        provPill('NOT RECORDED')),
      el('ul', { class: 'disclosure-list' }, ...(d.notRecordedDisclosures || []).map((item) => el('li', null,
        el('div', null, provPill(item.provenanceLabel || 'NOT RECORDED'), ' ', el('strong', null, item.fieldKey)),
        el('span', null, item.explanation)))),
    ];
    if (d.provenanceGlossary && d.provenanceGlossary.length > 0) {
      discParts.push(el('details', { class: 'more' },
        el('summary', null, 'Provenance tier definitions (all 6 tiers)'),
        el('ul', { class: 'disclosure-list' }, ...d.provenanceGlossary.map((g) => el('li', null,
          el('div', null, provPill(g.label), ' ', el('strong', null, g.d073Tier)),
          el('span', { class: 'small-note' }, g.description))))));
    }
    cards.push(el('section', { class: 'history-card history-card-disclosures', 'data-history-section': 'not-recorded' }, ...discParts));

    return cards;
  }

  function formatStructureCell(value) {
    if (value === null || value === undefined || typeof value !== 'number' || !Number.isFinite(value)) {
      return el('span', { class: 'structure-metric-unavail' }, 'Unavailable');
    }
    return fixed(value, 2);
  }

  export async function refreshStructure() {
    if (!state.view) return;
    const req = ++state.structureRequest;
    const reqTag = state.view.sessionId + ':' + state.view.source.annualRevision + ':' + state.view.source.workRevision;
    state.structureLoading = true;
    state.structureError = null;
    renderStructure();
    const outcome = await jsonCall('api/multi/structure', 'GET');
    const nowTag = state.view ? (state.view.sessionId + ':' + state.view.source.annualRevision + ':' + state.view.source.workRevision) : null;
    if (req !== state.structureRequest || nowTag !== reqTag) return;
    state.structureLoading = false;
    if (outcome.kind === 'ok' && outcome.payload && typeof outcome.payload.maxNeighbourCountK === 'number') {
      state.structure = outcome.payload;
      state.structureRootTag = reqTag;
      state.structureError = null;
    } else {
      const notice = noticeOf(outcome);
      state.structureError = 'Could not read structure (' + notice.code + '): ' + notice.message;
    }
    renderStructure();
  }

  export function renderStructure() {
    const box = byId('structure-content');
    const disc = byId('structure-disclaimer');
    if (!box || !disc) return;
    if (state.structureLoading) {
      box.replaceChildren(el('p', { class: 'small-note' }, 'Computing nearest-neighbour woodland structure…'));
      return;
    }
    if (state.structureError) {
      box.replaceChildren(el('p', { class: 'warning' }, state.structureError));
      return;
    }
    const s = state.structure;
    if (!s) {
      box.replaceChildren(el('p', { class: 'small-note' }, 'Loaded on demand when Details is opened. Press Refresh structure to compute M_i and T_i.'));
      return;
    }
    if (s.honestyLabel) disc.textContent = s.honestyLabel;
    const rows = (s.areas || []).map((a) => [
      areaShort(a.areaId),
      String(a.focalLivingTreeCount),
      a.effectiveNeighbourCount + ' / ' + a.maxNeighbourCountK,
      formatStructureCell(a.meanMingling),
      formatStructureCell(a.meanDiameterDifferentiation),
    ]);
    rows.push([
      'Whole woodland',
      String(s.focalLivingTreeCount),
      s.effectiveNeighbourCount + ' / ' + s.maxNeighbourCountK,
      formatStructureCell(s.meanMingling),
      formatStructureCell(s.meanDiameterDifferentiation),
    ]);
    box.replaceChildren(
      table('M_i (species mingling) & T_i (diameter diff.), k=eff/max',
        ['Scope', 'Live', 'k', 'M_i', 'T_i'],
        rows),
      el('p', { class: 'small-note' }, provPill(s.provenanceLabel || s.provenance || 'DERIVED'),
        ' Living adult trees across connected woodland (excludes stumps, deadwood, and unpromoted PJ saplings). Null values display as Unavailable, never 0.00.'));
  }

