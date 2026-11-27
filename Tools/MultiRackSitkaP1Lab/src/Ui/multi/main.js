// Two-area Stand Map (Pass 2 Map Integration, spec multi-map-integration-pass2-v1).
// Presentation authority (D-020): this script computes zero forestry, spacing, canopy, light, RD, DBH->radius or pruning rules.
// Every value comes from the server's atomic composite view/stage4/ecology/cell payload. Pure inline SVG only; zero WebP.
// S0c provenance: main.js [L5] — sections 7, 15, 17b(wire), 17b(bootstrap) from multi.js
import { announce, applyPayload, call, clearAlert, currentFence, refusedByLock, renderCommandGate, rereadButton, showAlert, showBudgetExceeded, showGuardRefusal, showLockAlert, showRefusal } from './api.js';
import { onAdd, onAddReference, onAdvance, onApprove, onCancelApproved, onCellClearAdd, onCrop, onEconomyToggleConfirmed, onExecuteConfirmed, onPjShelterAdd, onPlantQueue, onPruneAdd, onSetOrderExecutor, onSpotQueue, openEconomyToggleDialog, openExecuteDialog, removeClear, removePending, removePlanting, removePrune, reread, returnFocus, sendAdvance } from './commands.js';
import { confirmationStillShown, withdrawConfirmation } from './dialogs/execute-confirm.js';
import { askLoad, askRecover, onLoadConfirmed, onRecoverConfirmed, onSave, openSaves, updateSaveControls } from './dialogs/saves.js';
import { onWoodlandCancelDiscard, onWoodlandConfirmDiscard, onWoodlandCreate, onWoodlandFieldChanged, openWoodlandDialog } from './dialogs/woodland.js';
import { el, fill, fixed, label, n3, pair, plural, svg } from './dom.js';
import { renderMinimap } from './map/minimap.js';
import { onRankToggle, renderLegend, renderOverlayLegend, resolveMapPoint, setLegendHidden, svgRenderer } from './map/renderer-svg.js';
import { inspectCellAtPoint, renderCellInspector, renderJuvenileInspector, renderPlantDrawer, renderSpotDrawer, setMapMode, setOverlay } from './modes/interactive.js';
import { clearCompetition, clearDraft, competitionTag, onBatchResultClick, reconcileDrafts, refreshCompetition, renderBatchResult, renderCompetition, renderReasonPicker, resetReasonPicker, toggleDraft } from './panels/fell-draft.js';
import { renderInspector, renderOverlap, renderPruneGate } from './panels/inspector.js';
import { closeHistoryDrawer, loadHistoryIndex, loadHistoryYear, onSkipReview, openHistoryDrawer, refreshReviewPreview, refreshStructure, renderHistoryDrawer, renderSkipControl, renderSkipSummary, renderStructure } from './panels/time-history.js';
import { panMap, renderDetails, renderHistory, renderMapControls, renderPaneButtons, renderPlan, renderReceipt, renderTable, showPane, workTotals, zoomMap } from './panels/work-plan.js';
import { CHIPS, ZOOMS, approvedClears, approvedPlantings, approvedPrunes, areaName, areaOfTree, areaView, byId, currentArea, currentJuvenile, draftOf, draftTotal, ecoArea, hooks, isEconomyEnabled, isMultiLaneStand, juvenileById, livingIn, ordersIn, pct, pendingClears, pendingPlantings, pendingPrunes, readoutState, s4Area, selectedId, speciesLabel, speciesOf, state, treeIn, worldFromMap } from './store.js';
  // ---- rendering ------------------------------------------------------------------------------------------------------

  function renderAll() {
    if (!state.view || !state.labels) return;
    const active = document.activeElement;
    const focusKey = active && active.getAttribute ? active.getAttribute('data-focus-key') : null;
    renderStatus();
    renderSkipControl();
    renderModeAndOverlayControls();
    renderOverlayLegend();
    renderAreaSwitch();
    svgRenderer.render();
    renderMinimap(showArea);
    renderOverlap();
    renderCellInspector();
    renderPlantDrawer();
    renderSpotDrawer();
    renderJuvenileInspector();
    renderInspector();
    renderCompetition();
    renderBatchResult();
    renderPlan();
    renderReceipt();
    renderTable();
    renderHistory();
    renderDetails();
    renderStructure();
    if (state.historyOpen) renderHistoryDrawer();
    renderPaneButtons();
    renderCommandGate();
    renderMapControls();
    if (focusKey) {
      const again = document.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again && again !== document.activeElement) again.focus();
    }
  }

  function experimentBadgeText() {
    if (state.experimentBadge) return state.experimentBadge;
    if (state.ecology && state.ecology.experimentBadge) return state.ecology.experimentBadge;
    return state.view && state.view.experiment.experimentId === 'mad-ma1-mixed-demo-v1'
      ? 'Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)'
      : 'Synthetic MA-1 (all Sitka)';
  }

  function renderStatus() {
    const view = state.view;
    const isMulti = isMultiLaneStand();
    const badgeText = experimentBadgeText();
    const titlePrefix = isMulti ? 'Woodland Stand Map' : 'Two-area Stand Map';
    document.title = titlePrefix + ' — ' + badgeText;
    byId('page-title').textContent = titlePrefix;
    const badgeEl = byId('experiment-badge');
    badgeEl.textContent = badgeText;
    badgeEl.setAttribute('title', badgeText);
    byId('year-value').textContent = String(view.source.ecologicalYear);
    const econOn = isEconomyEnabled();
    const econ = state.economy;
    const econToggle = byId('economy-toggle');
    if (econToggle) econToggle.setAttribute('aria-pressed', String(econOn));
    const econToggleLabel = byId('economy-toggle-label');
    if (econToggleLabel) econToggleLabel.textContent = econOn ? 'Money: On' : 'Money: Off';
    const econStrip = byId('economy-strip');
    if (econStrip) {
      econStrip.hidden = !econOn;
      if (econOn) {
        byId('economy-cash').textContent = econ ? econ.cashBalanceDisplay : 'Unavailable — Re-read';
        byId('economy-hours').textContent = econ ? econ.landownerHoursDisplay : '–';
        byId('economy-stage').textContent = econ
          ? (econ.nextHarvestStageLabel + ' · ' + econ.minimumHarvestFloorDisplay + ' floor')
          : 'Economy read-out unavailable';
        const hoursWrap = byId('economy-hours-wrap');
        if (hoursWrap && econ && econ.landownerAllowanceTooltip) {
          hoursWrap.setAttribute('title', econ.landownerAllowanceTooltip);
        }
      }
    }
    byId('advance-label').textContent = label('MultiAdvanceYear');
    const advSub = byId('advance-sub');
    if (advSub) advSub.textContent = isMulti ? 'All lanes · does not execute work' : 'Both areas · does not execute work';
    const planEyebrow = byId('plan-card-eyebrow');
    if (planEyebrow) planEyebrow.textContent = isMulti ? 'Shared Work Plan · all lanes' : 'Shared Work Plan · both areas';
    const laneToggle = byId('lane-mode-toggle');
    if (laneToggle) {
      laneToggle.hidden = !isMulti;
      laneToggle.setAttribute('aria-pressed', String(state.singleLaneMode));
      laneToggle.textContent = state.singleLaneMode ? '1 lane (on)' : '1 lane';
    }
    const summaryEl = document.querySelector('.summary');
    if (summaryEl) summaryEl.classList.toggle('multi-lane', isMulti);

    if (isMulti) {
      byId('area-cards').replaceChildren(...view.areas.map((area) => {
        const s4 = s4Area(area.areaId);
        const cov = s4 ? pct(s4.canopy.geometricCoverFraction) : '–';
        const rd = s4 ? fixed(s4.canopy.relativeDensity, 2) : '–';
        const isCurrent = area.areaId === state.areaId;
        const shortName = area.areaId.startsWith('MA-') ? area.areaId.slice(3) : area.label;
        return el('button', {
          type: 'button',
          class: 'lane-strip-pill' + (isCurrent ? ' current' : ''),
          'data-strip-lane': area.areaId,
          'aria-pressed': String(isCurrent),
          title: area.label + ' · ' + area.stand.livingCount + ' living · Cover ' + cov + ' · RD ' + rd,
        },
        el('span', { class: 'lane-strip-name' }, 'Lane ' + shortName),
        el('span', { class: 'lane-strip-stat' }, String(area.stand.livingCount) + ' trees'),
        el('span', { class: 'lane-strip-sub' }, cov + ' · RD ' + rd));
      }));
    } else {
      byId('area-cards').replaceChildren(...view.areas.map((area) => el('div', {
        class: 'card area-card' + (area.areaId === state.areaId ? ' current' : '') },
        el('span', { class: 'dot', 'aria-hidden': 'true' }),
        el('div', { class: 'area-card-main' },
          el('p', { class: 'eyebrow' }, area.label + ' · living stand'),
          el('p', { class: 'area-card-count' }, el('strong', null, String(area.stand.livingCount)), ' ', el('span', null, 'living trees'))),
        el('div', { class: 'area-card-side' },
          el('strong', null, fixed(area.stand.basalAreaM2PerHa, 1)),
          el('span', null, 'm²/ha BA · ' + fixed(area.areaHa, 4) + ' ha')),
        areaReadouts(area.areaId))));
    }
    const approved = view.approved ? view.approved.orders.length : 0;
    const approvedP = approvedPrunes().length;
    const approvedC = approvedClears().length;
    const approvedPl = approvedPlantings();
    const pendingP = pendingPrunes().length;
    const pendingC = pendingClears().length;
    const pendingPl = pendingPlantings();
    const totalApproved = approved + approvedP + approvedC + approvedPl.length;
    const totalPending = view.pending.length + pendingP + pendingC + pendingPl.length;
    const drafts = draftTotal();
    byId('plan-summary').textContent = totalApproved > 0
      ? workTotals(approved, approvedP, approvedC, approvedPl) + ' approved'
      : totalPending > 0
        ? workTotals(view.pending.length, pendingP, pendingC, pendingPl) + ' pending'
        : 'No queued work';
    byId('plan-summary-sub').textContent = [totalApproved > 0 ? 'nothing executed yet' : totalPending > 0 ? 'not approved' : '',
      drafts > 0 ? plural(drafts, 'draft choice', 'draft choices') : ''].filter(Boolean).join(' · ');
    byId('notice-line').textContent = badgeText + ' · Site unknown · Modelled light · not calibrated PAR';
    const readout = readoutState();
    byId('disclosure').textContent = readout === 'phase2' || readout === 'stage4'
      ? (view.experiment.experimentId === 'mad-ma1-mixed-demo-v1' ? view.experiment.disclosure : label('MultiDisclosureStage4'))
      : readout === 'legacy' ? label('MultiDisclosure') :
      'Model read-out unavailable for this view, so the model statements are not shown. Choose Re-read. ' +
      [label('MultiSyntheticStatement'), label('MultiNotComparable'), label('MultiSiteUnknown'), label('MultiNoTransfer')].join('. ') + '.';
    const chips = [];
    const chipList = (readout === 'phase2' || readout === 'stage4')
      ? CHIPS.map((pair) => pair[0] === 'MultiChipEdgeExcluded' ? ['MultiChipConnectedWoodland', 'MultiConnectedWoodland'] : pair)
      : readout === 'legacy' ? CHIPS : CHIPS.filter((pair) => pair[0] !== 'MultiChipEdgeExcluded');
    chipList.forEach(([chip, full], index) => {
      if (index > 0) chips.push(' · ');
      chips.push(el('abbr', { title: label(full) }, label(chip)));
    });
    byId('flags').replaceChildren(...chips);
    byId('session-note').replaceChildren(el('abbr', { title: label('MultiSessionOnly') }, label('MultiChipSessionOnly')));
  }

  function areaReadouts(areaId) {
    const s4 = s4Area(areaId);
    const readout = state.stage4;
    if (!s4 || !readout) {
      return el('p', { class: 'area-readouts' }, 'Engine read-out unavailable for this view — choose Re-read.');
    }
    const c = s4.canopy;
    const m = readout.models;
    const ea = ecoArea(areaId);
    const em = state.ecology ? state.ecology.models : null;
    const tuple = em ? [em.adult, em.mortality, em.crown, em.regeneration, em.species].join('/')
      : [m.adultModelVersion, m.mortalityModelVersion, m.crownModelVersion].join('/');
    let speciesText = null;
    if (ea && ea.liveAdultsBySpecies) {
      const s = ea.liveAdultsBySpecies['sitka-spruce'] || 0;
      const o = ea.liveAdultsBySpecies['sessile-oak'] || 0;
      const b = ea.liveAdultsBySpecies['beech'] || 0;
      speciesText = (o > 0 || b > 0) ? (s + ' S · ' + o + ' O · ' + b + ' B') : (s + ' Sitka');
    }
    return el('dl', { class: 'area-readouts' },
      el('div', null, el('dt', null, 'Cover'), el('dd', null, pct(c.geometricCoverFraction), ' ',
        el('small', null, '· open ' + fixed(c.geometricUncoveredM2, 0) + ' m²'))),
      el('div', null, el('dt', null, 'RD'), el('dd', null, fixed(c.relativeDensity, 2))),
      el('div', null, el('dt', null, 'Deaths'), el('dd', null, c.annualNaturalDeaths + '/yr', ' ',
        el('small', null, '· ' + plural(c.fallenDeadwoodCount, 'log', 'logs')))),
      speciesText ? el('div', null, el('dt', null, 'Species'), el('dd', null, speciesText)) : null,
      el('div', { class: 'model-badge-extra' }, el('dt', { class: 'sr-only' }, 'Model'), el('dd', { class: 'model-badge' },
        el('abbr', { title: 'Model tuple ' + tuple + ' · ' + m.lightModelVersion + ' (' + fixed(m.lightCellM, 1) + ' m cells)' }, 'Model ' + tuple),
        el('abbr', { title: m.notCalibratedCaveat }, 'Light: not PAR'))));
  }

  function renderModeAndOverlayControls() {
    const mode = state.mapMode;
    byId('mode-tree').setAttribute('aria-pressed', String(mode === 'tree'));
    byId('mode-cell').setAttribute('aria-pressed', String(mode === 'cell'));
    byId('mode-plant').setAttribute('aria-pressed', String(mode === 'plant'));
    byId('mode-spot').setAttribute('aria-pressed', String(mode === 'spot'));
    const pane = byId('map-pane');
    pane.classList.toggle('cell-mode', mode === 'cell');
    pane.classList.toggle('plant-mode', mode === 'plant');
    pane.classList.toggle('spot-mode', mode === 'spot');
    const sel = byId('overlay-select');
    if (sel.value !== state.overlay) sel.value = state.overlay;
    // Explicit swap of the right-hand inspector body: exactly ONE panel is visible at a time (never stacks).
    const showPj = mode === 'tree' && currentJuvenile() !== null;
    byId('cell-panel').hidden = mode !== 'cell';
    byId('plant-panel').hidden = mode !== 'plant';
    byId('spot-panel').hidden = mode !== 'spot';
    byId('juvenile-panel').hidden = !showPj;
    byId('tree-panel').hidden = mode !== 'tree' || showPj;
  }

  function areaThumb(area) {
    const xs = area.polygon.map((p) => p.xm);
    const zs = area.polygon.map((p) => -p.zm);
    const minX = Math.min(...xs), minZ = Math.min(...zs);
    const w = Math.max(...xs) - minX || 1, h = Math.max(...zs) - minZ || 1;
    const pad = Math.max(w, h) * .06;
    const thumb = svg('svg', { class: 'area-thumb', viewBox: [minX - pad, minZ - pad, w + 2 * pad, h + 2 * pad].map(n3).join(' '),
      'aria-hidden': 'true', focusable: 'false' });
    thumb.append(svg('polygon', { points: area.polygon.map((p) => n3(p.xm) + ',' + n3(-p.zm)).join(' ') }));
    return thumb;
  }

  function renderAreaSwitch() {
    const view = state.view;
    byId('area-switch').replaceChildren(...view.areas.map((area) => {
      const inPlan = ordersIn(view.pending, area.areaId).length + (view.approved ? ordersIn(view.approved.orders, area.areaId).length : 0) +
        ordersIn(pendingPrunes(), area.areaId).length + ordersIn(approvedPrunes(), area.areaId).length +
        ordersIn(pendingClears(), area.areaId).length + ordersIn(approvedClears(), area.areaId).length +
        ordersIn(pendingPlantings(), area.areaId).length + ordersIn(approvedPlantings(), area.areaId).length;
      return el('button', { type: 'button', class: 'area-tab', 'data-area': area.areaId, 'data-focus-key': 'area:' + area.areaId,
        'aria-pressed': String(area.areaId === state.areaId) },
        areaThumb(area),
        el('span', { class: 'area-name' }, area.label),
        inPlan > 0 ? el('span', { class: 'area-plan' }, inPlan + ' in plan') : null,
        el('span', { class: 'sr-only' }, ' · ' + area.stand.livingCount + ' living · ' + fixed(area.areaHa, 4) + ' ha'));
    }));
  }

  // ---- selection & Escape precedence ----------------------------------------------------------------------------------

  function waitForBatch() { announce('Please wait: the selected trees are still being added to the Fell plan.'); }

  function showArea(areaId) {
    if (!areaView(areaId) || areaId === state.areaId) return;
    if (state.batch || state.queueRunning) { waitForBatch(); return; }
    state.mapPick += 1;
    state.cellPick += 1;
    state.areaId = areaId;
    state.zoom = ZOOMS[0];
    state.centre = null;
    state.overlap = null;
    state.selectedJuvenile = null;
    state.cellDetail = null;
    state.cellLoading = false;
    state.plantDraft = null;
    state.spotDraft = null;
    resetReasonPicker();
    const id = selectedId();
    if (id && livingIn(areaId, id)) {
      state.competitionTarget = { areaId, treeId: id };
      renderAll();
      refreshCompetition();
    } else {
      clearCompetition();
      renderAll();
    }
    const drafts = draftOf(areaId).length;
    announce('Showing ' + areaName(areaId) + '.' + (id ? ' Selected tree ' + id + '.' : '') +
      (drafts ? ' ' + plural(drafts, 'draft choice', 'draft choices') + ' kept for this reference.' : ''));
  }

  function chooseTree(areaId, treeId, fromList) {
    if (!treeIn(areaId, treeId)) return;
    if (state.batch || state.queueRunning) { waitForBatch(); return; }
    state.mapPick += 1;
    state.selectedJuvenile = null;
    if (state.mapMode !== 'tree') {
      state.plantDraft = null;
      state.spotDraft = null;
      state.mapMode = 'tree';
    }
    if (areaId !== state.areaId) { state.areaId = areaId; state.zoom = ZOOMS[0]; state.centre = null; }
    let cleared = '';
    const draft = state.drafts[areaId];
    if (draft && draft.ref !== treeId) {
      if (draft.ids.length > 0) cleared = ' Cleared ' + plural(draft.ids.length, 'unsent draft choice', 'unsent draft choices') +
        ' for the previous reference ' + draft.ref + '.';
      delete state.drafts[areaId];
    }
    if (state.batchResult && state.batchResult.areaId === areaId && state.batchResult.ref !== treeId) state.batchResult = null;
    state.draftNotice = cleared ? { areaId, text: cleared.trim() } : null;
    state.selected[areaId] = treeId;
    if (fromList) state.overlap = null;
    resetReasonPicker();
    if (livingIn(areaId, treeId)) state.competitionTarget = { areaId, treeId };
    else clearCompetition();
    showPane('tree');
    renderAll();
    byId('pane-tree').scrollTop = 0;
    announce('Selected tree ' + treeId + ' (' + speciesLabel(speciesOf(treeId)) + ') in ' + areaName(areaId) + '.' + cleared);
    if (state.competitionTarget) refreshCompetition();
  }

  function chooseJuvenile(areaId, juvenileId) {
    const j = juvenileById(juvenileId);
    if (!j) return;
    if (state.batch || state.queueRunning) { waitForBatch(); return; }
    state.mapPick += 1;
    if (state.mapMode !== 'tree') {
      state.plantDraft = null;
      state.spotDraft = null;
      state.mapMode = 'tree';
    }
    if (j.areaId !== state.areaId) { state.areaId = j.areaId; state.zoom = ZOOMS[0]; state.centre = null; }
    state.selectedJuvenile = { areaId: j.areaId, juvenileId: j.juvenileId };
    showPane('tree');
    renderAll();
    byId('pane-tree').scrollTop = 0;
    byId('pj-heading').focus();
    announce('Selected planted sapling ' + j.juvenileId + ' (' + speciesLabel(j.speciesId) + ') in ' + areaName(j.areaId) + '. Draft Fell ticks preserved.');
  }

  function closeJuvenileInspection(moveFocus) {
    if (!state.selectedJuvenile) return false;
    const id = state.selectedJuvenile.juvenileId;
    state.selectedJuvenile = null;
    renderAll();
    if (moveFocus) byId('mode-tree').focus();
    announce('Closed inspection of planted sapling ' + id + '. Draft Fell ticks preserved.');
    return true;
  }

  async function onMapClick(event) {
    if (state.batch || state.queueRunning) { waitForBatch(); return; }
    if (!state.view || refusedByLock('Map selection') || state.busy) return;
    const matrix = byId('map').getScreenCTM();
    if (!matrix) return;
    const svgPoint = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());
    const resolved = resolveMapPoint(svgPoint.x, svgPoint.y);
    const area = resolved.area;
    if (!area) return;
    const localU = resolved.u;
    const localV = resolved.v;

    // Inspect Cell mode: click queries POST api/multi/cell cleanly without altering selection or Fell draft.
    if (state.mapMode === 'cell') {
      if (area.areaId !== state.areaId) { state.areaId = area.areaId; }
      await inspectCellAtPoint(area.areaId, localU, localV);
      return;
    }

    // Plant mode: click places a provisional local Draft preview only (dashed slate, snap OFF, zero engine order).
    if (state.mapMode === 'plant') {
      if (area.areaId !== state.areaId) { state.areaId = area.areaId; }
      const world = worldFromMap(area, localU, localV);
      state.plantDraft = { areaId: area.areaId, u: localU, v: localV, xM: world.xM, zM: world.zM };
      state.plantResult = null;
      showPane('tree');
      renderAll();
      byId('plant-heading').focus();
      announce('Provisional Draft planting point in ' + areaName(area.areaId) + ' at X ' + fixed(world.xM, 2) + ' m, Z ' + fixed(world.zM, 2) + ' m (snap OFF; zero order queued).');
      return;
    }

    // Spot Clear mode: click places a provisional seedling-free 0.564 m circular preview at exact (X, Z) (snap OFF).
    if (state.mapMode === 'spot') {
      if (area.areaId !== state.areaId) { state.areaId = area.areaId; }
      const world = worldFromMap(area, localU, localV);
      state.spotDraft = { areaId: area.areaId, u: localU, v: localV, xM: world.xM, zM: world.zM };
      state.spotResult = null;
      showPane('tree');
      renderAll();
      byId('spot-heading').focus();
      announce('Provisional Draft Spot Clear (~1 m², 0.564 m ring) in ' + areaName(area.areaId) + ' at X ' + fixed(world.xM, 2) + ' m, Z ' + fixed(world.zM, 2) + ' m (snap OFF; zero order queued).');
      return;
    }

    // Select Tree mode (default): clicking a rendered PJ sapling inspects it (preserving adult Fell drafts); otherwise crown clicks win via POST api/multi/hit.
    const pjEl = event.target && event.target.closest ? event.target.closest('[data-juvenile-id]') : null;
    if (pjEl) {
      chooseJuvenile(pjEl.getAttribute('data-area-id') || area.areaId, pjEl.getAttribute('data-juvenile-id'));
      return;
    }

    const pick = ++state.mapPick;
    state.busy = true;
    const outcome = await call('api/multi/hit', { fence: currentFence(), areaId: area.areaId, u: localU, v: localV });
    state.busy = false;
    if (pick !== state.mapPick) {
      if (state.competitionTarget && !state.competition && !state.lock && !state.batch) refreshCompetition();
      return;
    }
    if (outcome.kind === 'ok') {
      clearAlert();
      const candidates = outcome.payload.candidates;
      if (candidates.length === 0) {
        state.overlap = null;
        applyPayload(outcome.payload, false);
        announce('No crown at this point — the selection is unchanged.');
        return;
      }
      state.overlap = candidates.length > 1 ? { areaId: area.areaId, candidates } : null;
      applyPayload(outcome.payload, true);
      chooseTree(area.areaId, candidates[0].treeId, false);
      if (candidates.length > 1) announce('Selected tree ' + candidates[0].treeId + '. ' + fill(label('CrownsOverlap'), { n: candidates.length }) + '.');
    } else if (outcome.kind === 'refused') {
      applyPayload(outcome.payload, false);
      if (state.lock) showLockAlert('Refused: Map selection (' + outcome.payload.refusal.code + '). Nothing was selected.');
      else showRefusal('Map selection', outcome.payload.refusal);
    } else if (outcome.kind === 'budget') {
      showBudgetExceeded('Map selection', outcome.budget);
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Map selection', outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: Map selection.');
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'Map selection could not be checked with the local session; nothing was selected.'), rereadButton()]);
    }
  }

  function closeInspection() {
    state.mapPick += 1;
    const areaId = state.areaId;
    const id = selectedId();
    if (!id) return false;
    if (state.batch || state.queueRunning) { waitForBatch(); return true; }
    const drafts = draftOf(areaId).length;
    delete state.selected[areaId];
    delete state.drafts[areaId];
    if (state.batchResult && state.batchResult.areaId === areaId) state.batchResult = null;
    state.draftNotice = null;
    state.overlap = null;
    clearCompetition();
    resetReasonPicker();
    renderAll();
    byId('map-heading').focus();
    announce('Closed the inspection of ' + id + '.' + (drafts ? ' Cleared ' + plural(drafts, 'unsent draft choice', 'unsent draft choices') + '.' : ''));
    return true;
  }

  // Escape precedence (Pass 2 spec §4.3 + Stage 4 R3 F-05 + Phase 1 Step 2):
  // 1) open modal dialog closes itself (leaves pending map hit-test alone);
  // 2) open on-demand History drawer closes WITHOUT clearing draft Fell ticks, selection, map mode or overlay;
  // 3) non-empty search box clears itself;
  // 4) Plant / Spot Clear / Inspect Cell preview & mode exits to Select Tree WITHOUT clearing draft Fell ticks or overlay;
  // 5) PJ sapling inspection closes back to adult tree inspector WITHOUT clearing draft Fell ticks;
  // 6) adult tree selection closes. Work Plan is NEVER mutated by Escape.
  function onEscape(event) {
    if (event.key !== 'Escape') return;
    if (document.querySelector('dialog[open]')) return;
    if (state.historyOpen) {
      event.preventDefault();
      closeHistoryDrawer(true);
      return;
    }
    state.mapPick += 1;
    state.cellPick += 1;
    if (event.defaultPrevented) return;
    const target = event.target;
    if (target && target.localName === 'input' && target.getAttribute('type') === 'search' && target.value !== '') return;
    if (state.mapMode !== 'tree') {
      event.preventDefault();
      state.plantDraft = null;
      state.spotDraft = null;
      setMapMode('tree', true);
      return;
    }
    if (state.selectedJuvenile) {
      event.preventDefault();
      closeJuvenileInspection(true);
      return;
    }
    if (closeInspection()) event.preventDefault();
  }

  function exactIdKey(inputId) {
    return (event) => {
      if (event.key !== 'Enter') return;
      event.preventDefault();
      const typed = byId(inputId).value;
      const area = state.view ? areaOfTree(typed) : null;
      if (area) {
        clearAlert();
        chooseTree(area.areaId, typed, true);
        if (!state.batch) byId('inspector-heading').focus();
        return;
      }
      const pj = juvenileById(typed);
      if (pj) {
        clearAlert();
        chooseJuvenile(pj.areaId, pj.juvenileId);
        return;
      }
      showAlert([el('p', { class: 'alert-title' }, 'No exact match — the selection is unchanged'),
        el('p', null, 'No tree or planted sapling in either area has the exact case-sensitive ID "' + typed + '".')], byId(inputId));
    };
  }

  function wire() {
    byId('area-switch').addEventListener('click', (event) => {
      const button = event.target.closest('[data-area]');
      if (button) showArea(button.getAttribute('data-area'));
    });
    byId('area-cards').addEventListener('click', (event) => {
      const pill = event.target.closest('[data-strip-lane]');
      if (pill) showArea(pill.getAttribute('data-strip-lane'));
    });
    byId('lane-mode-toggle').addEventListener('click', () => {
      state.singleLaneMode = !state.singleLaneMode;
      state.zoom = ZOOMS[0];
      state.centre = null;
      renderAll();
      announce(state.singleLaneMode ? 'Showing 1 active lane only.' : 'Showing active lane and visible neighbours.');
    });
    byId('woodland-open').addEventListener('click', openWoodlandDialog);
    byId('woodland-cancel').addEventListener('click', () => byId('woodland-dialog').close());
    byId('woodland-dialog').addEventListener('close', () => {
      byId('woodland-confirm-box').hidden = true;
      returnFocus();
    });
    byId('woodland-choices').addEventListener('change', onWoodlandFieldChanged);
    byId('woodland-custom-fields').addEventListener('input', onWoodlandFieldChanged);
    byId('woodland-custom-fields').addEventListener('change', onWoodlandFieldChanged);
    byId('woodland-create').addEventListener('click', onWoodlandCreate);
    byId('woodland-confirm-discard').addEventListener('click', onWoodlandConfirmDiscard);
    byId('woodland-confirm-cancel').addEventListener('click', onWoodlandCancelDiscard);
    byId('map').addEventListener('click', onMapClick);
    byId('mode-tree').addEventListener('click', () => setMapMode('tree', true));
    byId('mode-cell').addEventListener('click', () => setMapMode('cell', true));
    byId('mode-plant').addEventListener('click', () => setMapMode('plant', true));
    byId('mode-spot').addEventListener('click', () => setMapMode('spot', true));
    byId('cell-close').addEventListener('click', () => setMapMode('tree', true));
    byId('plant-close').addEventListener('click', () => setMapMode('tree', true));
    byId('spot-close').addEventListener('click', () => setMapMode('tree', true));
    byId('pj-close').addEventListener('click', () => closeJuvenileInspection(true));
    byId('cell-clear-add').addEventListener('click', onCellClearAdd);
    byId('cell-clear-remove').addEventListener('click', () => {
      const c = state.cellDetail;
      if (c && !c.missing) removeClear(c.areaId, c.column, c.row);
    });
    byId('plant-opt-shelter').addEventListener('change', () => svgRenderer.render());
    byId('plant-opt-spot').addEventListener('change', () => svgRenderer.render());
    byId('plant-queue').addEventListener('click', onPlantQueue);
    byId('plant-clear-draft').addEventListener('click', () => { state.plantDraft = null; renderAll(); });
    byId('plant-result').addEventListener('click', (event) => {
      const rm = event.target.closest('[data-planting-remove]');
      if (rm) removePlanting(rm.getAttribute('data-planting-remove'));
    });
    byId('spot-queue').addEventListener('click', onSpotQueue);
    byId('spot-clear-draft').addEventListener('click', () => { state.spotDraft = null; renderAll(); });
    byId('spot-result').addEventListener('click', (event) => {
      const rm = event.target.closest('[data-planting-remove]');
      if (rm) removePlanting(rm.getAttribute('data-planting-remove'));
    });
    byId('pj-shelter-add').addEventListener('click', onPjShelterAdd);
    byId('pj-shelter-remove').addEventListener('click', () => {
      const orderId = byId('pj-shelter-remove').getAttribute('data-pj-remove-order');
      if (orderId) removePlanting(orderId);
    });
    byId('planted-list').addEventListener('click', (event) => {
      const btn = event.target.closest('[data-pj-select]');
      if (btn) chooseJuvenile(state.areaId, btn.getAttribute('data-pj-select'));
    });
    byId('overlay-select').addEventListener('change', (event) => setOverlay(event.target.value));
    byId('crop-button').addEventListener('click', onCrop);
    byId('reread').addEventListener('click', () => reread(true));
    byId('economy-toggle').addEventListener('click', openEconomyToggleDialog);
    byId('economy-toggle-cancel').addEventListener('click', () => {
      state.economyToggleTarget = null;
      byId('economy-toggle-dialog').close();
    });
    byId('economy-toggle-confirm').addEventListener('click', onEconomyToggleConfirmed);
    byId('economy-toggle-dialog').addEventListener('cancel', () => { state.economyToggleTarget = null; });
    byId('economy-toggle-dialog').addEventListener('close', returnFocus);
    byId('advance').addEventListener('click', onAdvance);
    byId('skip-review').addEventListener('click', onSkipReview);
    byId('history-open').addEventListener('click', openHistoryDrawer);
    byId('history-close').addEventListener('click', () => closeHistoryDrawer(true));
    byId('history-backdrop').addEventListener('click', () => closeHistoryDrawer(true));
    byId('history-load-older').addEventListener('click', () => {
      if (state.historyNextCursor !== null && state.historyNextCursor !== undefined && !state.historyLoading) {
        loadHistoryIndex(state.historyNextCursor, true);
      }
    });
    byId('history-year-list').addEventListener('click', (event) => {
      const btn = event.target.closest('[data-history-year]');
      if (btn) loadHistoryYear(Number(btn.getAttribute('data-history-year')));
    });
    byId('structure-refresh').addEventListener('click', refreshStructure);
    byId('notice-structure').addEventListener('click', () => {
      showPane('details');
      refreshStructure();
      byId('structure-heading').focus();
    });
    byId('saves-open').addEventListener('click', openSaves);
    byId('saves-close').addEventListener('click', () => byId('saves-dialog').close());
    byId('saves-dialog').addEventListener('close', () => {
      byId('load-confirm').hidden = true;
      byId('recover-confirm').hidden = true;
      state.loadSlot = null;
      state.recoverSlot = null;
      returnFocus();
    });
    byId('save-slot').addEventListener('input', updateSaveControls);
    byId('save-overwrite').addEventListener('change', updateSaveControls);
    byId('save-do').addEventListener('click', onSave);
    byId('saves-rows').addEventListener('click', (event) => {
      const load = event.target.closest('[data-load]');
      if (load) { askLoad(load.getAttribute('data-load')); return; }
      const recover = event.target.closest('[data-recover]');
      if (recover) askRecover(recover.getAttribute('data-recover'));
    });
    byId('recover-cancel').addEventListener('click', () => {
      byId('recover-confirm').hidden = true;
      state.recoverSlot = null;
      byId('save-slot').focus();
    });
    byId('recover-do').addEventListener('click', onRecoverConfirmed);
    byId('load-cancel').addEventListener('click', () => {
      byId('load-confirm').hidden = true;
      state.loadSlot = null;
      byId('save-slot').focus();
    });
    byId('load-do').addEventListener('click', onLoadConfirmed);
    byId('view-plan').addEventListener('click', () => { showPane('plan'); byId('plan-heading').focus(); });
    byId('notice-provenance').addEventListener('click', () => { showPane('details'); byId('details-heading').focus(); });
    byId('rank-layer').addEventListener('click', onRankToggle);
    byId('inspect-close').addEventListener('click', closeInspection);
    byId('readout-reread').addEventListener('click', () => reread(true));
    document.documentElement.addEventListener('keydown', onEscape);
    byId('legend-hide').addEventListener('click', () => setLegendHidden(true, true));
    byId('legend-show').addEventListener('click', () => setLegendHidden(false, true));
    byId('competition-content').addEventListener('change', (event) => {
      const box = event.target.closest('[data-draft]');
      if (box) toggleDraft(box.getAttribute('data-draft'), box.checked);
    });
    byId('draft-clear').addEventListener('click', clearDraft);
    byId('batch-result').addEventListener('click', onBatchResultClick);
    byId('add-reference').addEventListener('click', onAddReference);
    byId('overlap-list').addEventListener('click', (event) => {
      const button = event.target.closest('[data-pick]');
      if (button) chooseTree(state.areaId, button.getAttribute('data-pick'), false);
    });
    byId('tree-rows').addEventListener('click', (event) => {
      const button = event.target.closest('[data-select]');
      if (button) {
        chooseTree(state.areaId, button.getAttribute('data-select'), true);
        if (!state.batch) byId('inspector-heading').focus();
      }
    });
    byId('tree-search').addEventListener('input', renderTable);
    byId('tree-search').addEventListener('keydown', exactIdKey('tree-search'));
    byId('quick-id').addEventListener('keydown', exactIdKey('quick-id'));
    byId('reason-release').addEventListener('change', renderReasonPicker);
    byId('reason').addEventListener('input', renderReasonPicker);
    byId('add-form').addEventListener('submit', onAdd);
    byId('insp-remove').addEventListener('click', () => removePending(state.areaId, selectedId()));
    byId('pending-areas').addEventListener('click', (event) => {
      const execBtn = event.target.closest('[data-executor-value]');
      if (execBtn) {
        onSetOrderExecutor(
          execBtn.getAttribute('data-executor-kind'),
          execBtn.getAttribute('data-executor-area'),
          execBtn.getAttribute('data-executor-target'),
          execBtn.getAttribute('data-executor-value'));
        return;
      }
      const rm = event.target.closest('[data-remove]');
      if (rm) { removePending(rm.getAttribute('data-area'), rm.getAttribute('data-remove')); return; }
      const pr = event.target.closest('[data-prune-remove]');
      if (pr) { removePrune(pr.getAttribute('data-area'), pr.getAttribute('data-prune-remove')); return; }
      const cl = event.target.closest('[data-clear-remove]');
      if (cl) { removeClear(cl.getAttribute('data-area'), cl.getAttribute('data-clear-col'), cl.getAttribute('data-clear-row')); return; }
      const pl = event.target.closest('[data-planting-remove]');
      if (pl) removePlanting(pl.getAttribute('data-planting-remove'));
    });
    byId('prune-add').addEventListener('click', onPruneAdd);
    byId('prune-remove').addEventListener('click', () => removePrune(state.areaId, selectedId()));
    byId('approve').addEventListener('click', onApprove);
    byId('cancel-approved').addEventListener('click', onCancelApproved);
    byId('execute').addEventListener('click', openExecuteDialog);
    byId('execute-back').addEventListener('click', () => { state.confirmation = null; byId('execute-dialog').close(); });
    byId('execute-confirm').addEventListener('click', onExecuteConfirmed);
    byId('execute-dialog').addEventListener('cancel', () => { state.confirmation = null; });
    byId('execute-dialog').addEventListener('close', () => {
      if (!byId('execute-dialog').open) state.confirmation = null;
      returnFocus();
    });
    byId('consent-keep').addEventListener('click', () => {
      byId('consent-dialog').close();
      announce(label('MultiNothingChanged') + ' — still year ' + state.view.source.ecologicalYear + '.');
    });
    byId('consent-advance').addEventListener('click', () => { byId('consent-dialog').close(); sendAdvance(state.consentWork); });
    byId('consent-dialog').addEventListener('close', returnFocus);
    document.querySelector('.view-tabs').addEventListener('click', (event) => {
      const button = event.target.closest('[data-pane-button]');
      if (button) showPane(button.getAttribute('data-pane-button'));
    });
    byId('skip-trees').addEventListener('click', () => showPane('trees'));
    byId('zoom-in').addEventListener('click', () => zoomMap(1));
    byId('zoom-out').addEventListener('click', () => zoomMap(-1));
    byId('zoom-fit').addEventListener('click', () => { state.zoom = ZOOMS[0]; state.centre = null; svgRenderer.render(); renderMapControls(); });
    byId('pan-north').addEventListener('click', () => panMap(0, -1));
    byId('pan-south').addEventListener('click', () => panMap(0, 1));
    byId('pan-west').addEventListener('click', () => panMap(-1, 0));
    byId('pan-east').addEventListener('click', () => panMap(1, 0));
    let frame = 0;
    const reframe = () => {
      if (frame) return;
      frame = window.requestAnimationFrame(() => { frame = 0; if (state.view && state.labels) { svgRenderer.render(); renderMapControls(); } });
    };
    if (typeof ResizeObserver === 'function') new ResizeObserver(reframe).observe(byId('map-pane'));
    else window.addEventListener('resize', reframe);
  }

  // S0c P4 hook assignments
  hooks.clearCompetition = clearCompetition;
  hooks.competitionTag = competitionTag;
  hooks.confirmationStillShown = confirmationStillShown;
  hooks.experimentBadgeText = experimentBadgeText;
  hooks.fill = fill;
  hooks.fixed = fixed;
  hooks.label = label;
  hooks.loadHistoryIndex = loadHistoryIndex;
  hooks.reconcileDrafts = reconcileDrafts;
  hooks.refreshCompetition = refreshCompetition;
  hooks.refreshReviewPreview = refreshReviewPreview;
  hooks.refreshStructure = refreshStructure;
  hooks.renderAll = renderAll;
  hooks.renderPruneGate = renderPruneGate;
  hooks.renderSkipSummary = renderSkipSummary;
  hooks.reread = reread;
  hooks.toggleDraft = toggleDraft;
  hooks.withdrawConfirmation = withdrawConfirmation;
  hooks.workTotals = workTotals;
  svgRenderer.mount();
  renderLegend();
  wire();
  reread(false);
