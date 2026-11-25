// Two-area Stand Map (Pass 2 Map Integration, spec multi-map-integration-pass2-v1).
// Presentation authority (D-020): this script computes zero forestry, spacing, canopy, light, RD, DBH->radius or pruning rules.
// Every value comes from the server's atomic composite view/stage4/ecology/cell payload. Pure inline SVG only; zero WebP.
'use strict';

(function () {
  const SVG_NS = 'http://www.w3.org/2000/svg';
  const SPOT_PREVIEW_RADIUS_M = Math.sqrt(1 / Math.PI); // visual radius (~0.564 m) for ~1 m2 Spot Clear circle preview
  const STATUS_LABEL = { Living: 'StatusLiving', PendingFell: 'StatusPendingFell', ApprovedFell: 'StatusApprovedFell',
    Felled: 'StatusFelled', NaturalDead: 'StatusNaturalDead' };
  const SPECIES_NAME = { 'sitka-spruce': 'Sitka spruce', 'sessile-oak': 'Sessile oak', beech: 'European beech' };
  const SPECIES_SHORT = { 'sitka-spruce': 'S', 'sessile-oak': 'O', beech: 'B' };
  const SPECIES_SYMBOL = { 'sitka-spruce': 'ccf-sitka', 'sessile-oak': 'ccf-oak', beech: 'ccf-beech' };
  const SAPLING_SYMBOL = { 'sitka-spruce': 'ccf-sapling-sitka', 'sessile-oak': 'ccf-sapling-oak', beech: 'ccf-sapling-beech' };
  const ZOOMS = Array.from({ length: 11 }, (_, i) => Math.pow(4, i / 10));
  const PANES = ['tree', 'plan', 'trees', 'history', 'details'];
  const PANE_NAMES = { tree: 'Tree', plan: 'Work Plan', trees: 'Trees', history: 'Results', details: 'Details' };
  const COMMAND_IDS = ['crop-button', 'add-reference', 'insp-remove', 'approve', 'cancel-approved', 'execute',
    'execute-confirm', 'advance', 'skip-review', 'consent-advance', 'prune-add', 'prune-remove',
    'cell-clear-add', 'cell-clear-remove', 'plant-queue', 'spot-queue', 'pj-shelter-add', 'pj-shelter-remove'];
  const LIGHT_STOPS = [[0x7f, 0x3b, 0x4a], [0xda, 0x79, 0x6d], [0xff, 0xd9, 0xbd]];
  const LIGHT_EASE = 0.65;
  const LIGHT_ALPHA_MAX = 0.9;
  const LIGHT_WORDS = [[0.1, 'Very shaded'], [0.35, 'Low'], [0.65, 'Moderate'], [Infinity, 'Bright']];
  const SLATE = [0x1b, 0x24, 0x30];
  const STATEMENTS = ['MultiSyntheticStatement', 'MultiNotComparable', 'MultiSiteUnknown', 'MultiEdgeExcluded',
    'MultiNoTransfer', 'MultiSessionOnly'];
  const CHIPS = [['MultiChipSynthetic', 'MultiSyntheticStatement'], ['MultiChipNotComparable', 'MultiNotComparable'],
    ['MultiChipSiteUnknown', 'MultiSiteUnknown'], ['MultiChipEdgeExcluded', 'MultiEdgeExcluded'],
    ['MultiChipNoTransfer', 'MultiNoTransfer']];
  const DEFAULT_REASON = 'Owner-selected felling';

  const state = {
    view: null,
    scenes: {},
    labels: null,
    stage4: null,
    ecology: null,
    areaId: null,
    selected: {},
    selectedJuvenile: null, // { areaId, juvenileId } when inspecting a planted sapling in Select Tree mode
    drafts: {},
    draftNotice: null,
    overlap: null,
    competition: null,
    competitionTarget: null,
    competitionTag: null,
    competitionRequest: 0,
    busy: false,
    queueRunning: false,
    mapPick: 0,
    cellPick: 0,
    batchResultOpen: false,
    batch: null,
    batchResult: null,
    lock: null,
    lockCount: 0,
    confirmation: null,
    consentWork: false,
    opener: null,
    lastExecution: null,
    lastWasReplay: false,
    zoom: 1,
    centre: null,
    legendHidden: false,
    mapMode: 'tree', // 'tree' | 'cell' | 'plant' | 'spot'
    overlay: 'off', // 'off' | 'light' | 'undergrowth' | 'seedlings'
    cellDetail: null, // server cell detail from POST api/multi/cell, or { areaId, missing: true }
    cellLoading: false,
    plantDraft: null, // provisional local preview { areaId, u, v, xM, zM } (snap OFF, zero engine order)
    plantResult: null, // sequential Plant -> Shelter -> SpotClear queue result + partial disclosure
    spotDraft: null, // provisional local preview { areaId, u, v, xM, zM } (snap OFF, zero engine order)
    spotResult: null,
    saves: [], // GET api/multi/saves rows: slot, label, year, savedUtc, models, status, canLoad, backup state, recoverable
    loadSlot: null, // slot awaiting the player's explicit "replace the current forest" confirmation
    recoverSlot: null, // slot awaiting the player's explicit "restore its backup" confirmation
    // Phase 1 Step 2: bounded view metadata, review preview/skip, on-demand History drawer, and on-demand Structure panel.
    historyTruncated: false,
    payloadBudgetExceeded: false,
    totalRecordedYears: 0,
    reviewPreview: null,
    previewRequest: 0,
    lastSkip: null,
    historyOpen: false,
    historyLoading: false,
    historyDetailLoading: false,
    historyItems: [],
    historyNextCursor: null,
    historyTotalRecorded: 0,
    historySelectedYear: null,
    historyDetail: null,
    historyError: null,
    historyRequest: 0,
    historyDetailRequest: 0,
    structure: null,
    structureLoading: false,
    structureError: null,
    structureRequest: 0,
    structureRootTag: null,
  };

  const byId = (id) => document.getElementById(id);

  // ---- small DOM helpers ----------------------------------------------------------------------------------------------

  function setAttributes(node, attributes) {
    for (const [name, value] of Object.entries(attributes || {})) {
      if (value !== null && value !== undefined && value !== false) node.setAttribute(name, String(value));
    }
  }

  function el(tag, attributes, ...children) {
    const node = document.createElement(tag);
    setAttributes(node, attributes);
    for (const child of children) {
      if (child === null || child === undefined) continue;
      node.append(child instanceof Node ? child : String(child));
    }
    return node;
  }

  function svg(tag, attributes, textValue) {
    const node = document.createElementNS(SVG_NS, tag);
    setAttributes(node, attributes);
    if (textValue !== undefined) node.textContent = String(textValue);
    return node;
  }

  function useIcon(id, size) {
    const box = svg('svg', { viewBox: '0 0 32 32', width: size || 16, height: size || 16, 'aria-hidden': 'true', focusable: 'false' });
    box.append(svg('use', { href: '#' + id, x: 0, y: 0, width: 32, height: 32 }));
    return box;
  }

  function pair(term, value, className) {
    return el('div', className ? { class: className } : null, el('dt', null, term), el('dd', null, value));
  }

  function label(name) {
    return state.labels && typeof state.labels[name] === 'string' ? state.labels[name] : '';
  }

  function fill(template, values) {
    return template.replace(/\{(\w+)\}/g, (match, name) => (name in values ? String(values[name]) : match));
  }

  function fixed(value, digits) {
    return typeof value === 'number' ? value.toFixed(digits) : String(value);
  }

  function lightIndexText(value) {
    return typeof value === 'number' && value > 0 && value < 0.0005 ? '< 0.001' : fixed(value, 3);
  }

  function plural(n, one, many) { return n + ' ' + (n === 1 ? one : many); }
  function listIds(ids) { return ids.length === 0 ? 'none' : ids.join(', '); }
  const n3 = (value) => Math.round(value * 1000) / 1000;

  function expandable(full) {
    const value = String(full);
    return el('details', { class: 'expand' },
      el('summary', null, el('span', { class: 'mono' }, value.slice(0, 12) + '…'), el('span', { class: 'sr-only' }, ' show the full value')),
      el('code', { class: 'full' }, value));
  }

  function technicalPlan(planId, executionKey) {
    return el('details', { class: 'technical-plan' }, el('summary', null, 'Technical reference'),
      el('p', null, 'PlanId ', el('code', null, planId)), el('p', null, 'Execution key ', el('code', null, executionKey)));
  }

  function table(caption, headers, rows) {
    return el('div', { class: 'table-wrap', role: 'region', 'aria-label': caption, tabindex: '0' },
      el('table', null, el('caption', null, caption),
        el('thead', null, el('tr', null, ...headers.map((header) => el('th', { scope: 'col' }, header)))),
        el('tbody', null, ...rows.map((cells) => el('tr', null, el('th', { scope: 'row' }, cells[0]),
          ...cells.slice(1).map((cell) => el('td', null, cell)))))));
  }

  // ---- lookups in the latest atomic composite payload -----------------------------------------------------------------

  function areaView(areaId) {
    return state.view && areaId ? state.view.areas.find((area) => area.areaId === areaId) || null : null;
  }
  function currentArea() { return areaView(state.areaId); }
  function treeIn(areaId, treeId) {
    const area = areaView(areaId);
    return area && treeId ? area.trees.find((tree) => tree.treeId === treeId) || null : null;
  }
  function livingIn(areaId, treeId) {
    const tree = treeIn(areaId, treeId);
    return Boolean(tree && tree.isLiving);
  }
  function areaOfTree(treeId) {
    return state.view ? (state.view.areas.find((area) => area.trees.some((tree) => tree.treeId === treeId)) || null) : null;
  }
  function sceneOf(areaId) { return (state.scenes && state.scenes[areaId]) || []; }
  function itemOf(areaId, treeId) { return sceneOf(areaId).find((item) => item.treeId === treeId) || null; }
  function areaName(areaId) { const area = areaView(areaId); return area ? area.label : String(areaId); }
  function areaShort(areaId) { const area = areaView(areaId); return area ? area.label : String(areaId); }
  function selectedId() { return state.areaId ? state.selected[state.areaId] || null : null; }
  function ordersIn(orders, areaId) { return orders.filter((order) => order.areaId === areaId); }

  function speciesOf(treeId) {
    if (state.ecology && state.ecology.speciesByTreeId && state.ecology.speciesByTreeId[treeId]) {
      return state.ecology.speciesByTreeId[treeId];
    }
    return 'sitka-spruce';
  }
  function speciesLabel(speciesId) { return SPECIES_NAME[speciesId] || speciesId; }

  function s4Area(areaId) {
    const readout = state.stage4;
    return readout && areaId ? readout.areas.find((area) => area.areaId === areaId) || null : null;
  }
  function s4Tree(areaId, treeId) {
    const area = s4Area(areaId);
    return area && treeId ? area.trees.find((tree) => tree.treeId === treeId) || null : null;
  }
  function ecoArea(areaId) {
    const eco = state.ecology;
    return eco && areaId ? eco.areas.find((area) => area.areaId === areaId) || null : null;
  }
  // Engine-authoritative species-aware prune status (prefers ecology.pruneStatusByTreeId, falls back to stage4).
  function pruneStatusOf(areaId, treeId) {
    if (state.ecology && state.ecology.pruneStatusByTreeId && state.ecology.pruneStatusByTreeId[treeId]) {
      return state.ecology.pruneStatusByTreeId[treeId];
    }
    const s4 = s4Tree(areaId, treeId);
    if (!s4) return null;
    return {
      speciesId: speciesOf(treeId),
      eligible: s4.pruneEligible,
      lifts: s4.pruningLifts,
      maxLifts: s4.maxPruningLifts,
      minIntervalYears: 5,
      nextTargetM: s4.nextPruneTargetM,
      nextEligibleYear: s4.nextPruneEligibleYear,
      refusalCode: s4.pruneRefusalCode,
      refusalReason: s4.pruneRefusalReason,
      planState: s4.prunePlanState,
    };
  }
  function pendingPrunes() { return state.stage4 ? state.stage4.pendingPrunes : []; }
  function approvedPrunes() { return state.stage4 ? state.stage4.approvedPrunes : []; }
  function pendingClears() { return state.ecology ? (state.ecology.pendingClear || []) : []; }
  function approvedClears() { return state.ecology ? (state.ecology.approvedClear || []) : []; }
  function pendingPlantings() { return state.ecology ? (state.ecology.pendingPlanting || []) : []; }
  function approvedPlantings() { return state.ecology ? (state.ecology.approvedPlanting || []) : []; }
  function worldFromMap(area, u, v) {
    return { xM: area.extent.minXM + u, zM: area.extent.minZM + area.extent.sideM - v };
  }
  function plantedIn(areaId) {
    return state.ecology ? (state.ecology.planted || []).filter((j) => j.areaId === areaId) : [];
  }
  function juvenileById(juvenileId) {
    return state.ecology && juvenileId ? (state.ecology.planted || []).find((j) => j.juvenileId === juvenileId) || null : null;
  }
  function currentJuvenile() {
    const sel = state.selectedJuvenile;
    if (!sel || sel.areaId !== state.areaId) return null;
    const j = juvenileById(sel.juvenileId);
    return j && j.areaId === state.areaId ? j : null;
  }
  function shelterForJuvenile(j) {
    if (!state.ecology || !j) return null;
    const list = (state.ecology.shelters || []).filter((s) => s.juvenileId === j.juvenileId);
    return list.find((s) => s.isEffective) || list[list.length - 1] || null;
  }
  function matchingShelterOrder(orders, j) {
    if (!j) return null;
    return orders.find((o) => o.kind === 'Shelter' && o.areaId === j.areaId &&
      Math.abs(o.xm - j.xm) < 1e-4 && Math.abs(o.zm - j.zm) < 1e-4) || null;
  }
  function isStage4() { return Boolean(state.stage4 && state.stage4.models.adultModelVersion === 1); }
  function isPhase2() { return Boolean(state.ecology && state.ecology.models && state.ecology.models.species === 1); }
  function readoutState() { return !state.stage4 ? 'unavailable' : isPhase2() ? 'phase2' : isStage4() ? 'stage4' : 'legacy'; }

  function compactCellsFor(areaId) {
    if (!state.ecology || !state.view) return new Map();
    const areaIdx = state.view.areas.findIndex((a) => a.areaId === areaId);
    if (areaIdx < 0) return new Map();
    const map = new Map();
    for (const row of state.ecology.cellsCompact) {
      if (row[0] === areaIdx) map.set(row[1] + ':' + row[2], row);
    }
    return map;
  }

  function lightPaint(index) {
    if (!(index > 0)) return null;
    const t = Math.pow(Math.min(1, index), LIGHT_EASE);
    const segment = t <= 0.5 ? 0 : 1;
    const f = segment === 0 ? t / 0.5 : (t - 0.5) / 0.5;
    const rgb = LIGHT_STOPS[segment].map((low, i) => Math.round(low + (LIGHT_STOPS[segment + 1][i] - low) * f));
    return { colour: '#' + rgb.map((v) => v.toString(16).padStart(2, '0')).join(''), opacity: opacityText(LIGHT_ALPHA_MAX * t) };
  }
  function opacityText(opacity) {
    const decimals = Math.max(4, 2 - Math.floor(Math.log10(opacity)));
    return decimals <= 100 ? opacity.toFixed(decimals) : opacity.toExponential(3);
  }
  function lightWord(index) {
    const word = LIGHT_WORDS.find(([below]) => index < below)[1];
    return index > 0 ? word : word + ' · no modelled light';
  }
  function groundCell(areaId, column, row) {
    const area = s4Area(areaId);
    return area ? area.ground.find((cell) => cell.column === column && cell.row === row) || null : null;
  }
  function pct(fraction) { return fixed(fraction * 100, 0) + '%'; }
  function pct1(fraction) { return fixed(fraction * 100, 1) + '%'; }
  function baseStatusText(item) {
    const template = label(STATUS_LABEL[item.status]);
    return template ? fill(template, { y: item.deathYear }) : String(item.status);
  }
  function statusText(item) { return item.isCrop ? label('CropRetained') : baseStatusText(item); }

  function referenceIn(areaId) {
    const id = state.selected[areaId];
    return id && livingIn(areaId, id) ? id : null;
  }
  function draftOf(areaId) {
    const draft = state.drafts[areaId];
    const reference = referenceIn(areaId);
    return draft && reference && draft.ref === reference ? draft.ids : [];
  }
  function draftTotal() {
    return state.view ? state.view.areas.reduce((sum, area) => sum + draftOf(area.areaId).length, 0) : 0;
  }
  function currentReport() {
    const comp = state.competition;
    if (!comp || state.lock || comp.areaId !== state.areaId) return null;
    return comp.report && comp.report.targetTreeId === referenceIn(state.areaId) ? comp.report : null;
  }

  function blockedReason(areaId, treeId) {
    const item = itemOf(areaId, treeId);
    if (!item) return 'Not in this area';
    if (item.isCrop) return 'Retained Crop — not offered for Fell';
    switch (item.status) {
      case 'PendingFell': return 'Already pending Fell';
      case 'ApprovedFell': return 'In the approved plan';
      case 'Felled': return 'Felled';
      case 'NaturalDead': return 'Dead';
      default: break;
    }
    const ps = pruneStatusOf(areaId, treeId);
    if (ps && ps.planState) return 'In the Prune plan';
    if (treeId === referenceIn(areaId)) return 'Reference tree';
    return null;
  }

  const SHORT_BLOCKED = {
    'Retained Crop — not offered for Fell': 'Retained Crop', 'Already pending Fell': 'Pending Fell',
    'In the approved plan': 'Approved plan', 'In the Prune plan': 'Prune plan', 'Reference tree': 'Reference', 'Not in this area': 'Other area'
  };
  function shortBlockedReason(why) { return SHORT_BLOCKED[why] || why; }

  // ---- talking to the local session process ---------------------------------------------------------------------------

  function currentFence() {
    const view = state.view;
    return { sessionId: view.sessionId, source: view.source, planStateRevision: view.planStateRevision };
  }

  async function call(path, body) {
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

  function applyPayload(payload, keepOverlap) {
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
    if (target && !livingIn(target.areaId, target.treeId)) clearCompetition();
    const refresh = state.competitionTarget !== null && state.competitionTag !== competitionTag();
    if (refresh && !state.batch) state.competition = null;
    reconcileDrafts();
    renderAll();
    const confirmation = state.confirmation;
    if (confirmation && !byId('execute-dialog').open) state.confirmation = null;
    else if (confirmation && !confirmationStillShown(confirmation, state.view)) withdrawConfirmation(confirmation);
    if (refresh && !state.lock && !state.batch) refreshCompetition();
    if (prevTreeTag && nextTreeTag !== prevTreeTag && !state.lock && !state.batch && !state.queueRunning &&
        byId('side').getAttribute('data-pane') === 'details') {
      refreshStructure();
    }
  }

  async function run(path, body, what, onCommitted, focusAfter) {
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
      refreshReviewPreview();
      if (state.historyOpen) loadHistoryIndex(null, false);
    } else if (outcome.kind === 'refused') {
      applyPayload(outcome.payload, false);
      if (state.lock) showLockAlert('Refused: ' + what + ' (' + outcome.payload.refusal.code + '). Nothing changed.');
      else showRefusal(what, outcome.payload.refusal);
      refreshReviewPreview();
    } else if (outcome.kind === 'guard') {
      showGuardRefusal(what, outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: ' + what + '.');
    } else {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        what + ': no confirmed result was received. Do not assume that it happened, or that it did not.');
    }
  }

  // ---- Phase 1 Save/Load (server-managed named slots; no path, no upload, no download) --------------------------------

  const SAVE_STATE = {
    valid: ['Ready to load', 'state-valid'],
    incompatible: ['Different forest, model or engine', 'state-incompatible'],
    corrupt: ['Corrupt — not loadable', 'state-corrupt'],
    missing: ['Missing file', 'state-missing'],
  };

  function savedSlot(slot) { return (state.saves || []).find((row) => row.slot === slot) || null; }

  function saveStatus(text, kind) {
    const box = byId('saves-status');
    box.textContent = text;
    box.className = 'save-status' + (kind ? ' ' + kind : '');
  }

  // Save/Load routes answer their own payload shapes (no composite view on a save, a composite view on a load), so they
  // use this small JSON helper instead of call().
  async function jsonCall(path, method, body) {
    const init = { method, cache: 'no-store', credentials: 'omit', redirect: 'error' };
    if (body !== undefined) {
      init.headers = { 'Content-Type': 'application/json' };
      init.body = JSON.stringify(body);
    }
    let response;
    try { response = await fetch(path, init); } catch (failure) {
      return { kind: 'transport', detail: String((failure && failure.message) || failure) };
    }
    let payload = null;
    try { payload = await response.json(); } catch { payload = null; }
    const okStatus = Boolean(response.ok) || (response.status >= 200 && response.status < 300);
    if (okStatus && payload) return { kind: 'ok', status: response.status, payload };
    if (payload && payload.view && payload.refusal) return { kind: 'refused', status: response.status, payload };
    if (payload && (payload.refusal || payload.error)) return { kind: 'guard', status: response.status, payload };
    return { kind: 'transport', status: response.status, detail: 'HTTP ' + response.status };
  }

  function noticeOf(outcome) {
    if (outcome.payload) {
      if (outcome.payload.refusal) return outcome.payload.refusal;
      if (outcome.payload.error) return outcome.payload.error;
    }
    return { code: 'Transport', message: outcome.detail || 'no confirmed answer from the local server' };
  }

  async function openSaves() {
    byId('saves-title').textContent = label('MultiSavesTitle') || 'Saved games';
    byId('saves-intro').textContent = label('MultiSavesIntro');
    byId('save-slot-hint').textContent = label('MultiSavesNameHint');
    byId('save-overwrite-text').textContent = label('MultiSavesOverwrite');
    byId('save-do').textContent = label('MultiSavesSave');
    byId('load-confirm').hidden = true;
    byId('load-confirm-text').textContent = '';
    saveStatus('', null);
    openDialog(byId('saves-dialog'), byId('save-slot'));
    await refreshSaves();
  }

  async function refreshSaves() {
    const outcome = await jsonCall('api/multi/saves', 'GET');
    if (outcome.kind !== 'ok' || !outcome.payload) {
      const notice = noticeOf(outcome);
      saveStatus('Could not read the saved games list (' + notice.code + ').', 'bad');
      return;
    }
    state.saves = outcome.payload.slots || [];
    renderSaves();
  }

  function renderSaves() {
    const rows = (state.saves || []).map((slot) => {
      const [word, stateClass] = SAVE_STATE[slot.status] || [slot.status, 'state-corrupt'];
      const actions = [];
      if (slot.canLoad) {
        actions.push(el('button', { type: 'button', class: 'small', 'data-load': slot.slot,
          'aria-label': 'Load saved slot ' + slot.slot }, label('MultiSavesLoad') || 'Load'));
      }
      if (slot.backupExists && slot.recoverable) {
        actions.push(el('button', { type: 'button', class: 'small', 'data-recover': slot.slot,
          'aria-label': 'Restore the verified backup of ' + slot.slot }, label('MultiSavesRecover') || 'Restore backup'));
      }
      return el('tr', null,
        el('td', { class: 'slot-cell' }, el('code', null, slot.slot)),
        el('td', { class: 'num' }, slot.ecologicalYear === null || slot.ecologicalYear === undefined ? '–' : String(slot.ecologicalYear)),
        el('td', { class: 'num' }, slot.savedUtc || '–'),
        el('td', { class: stateClass }, word,
          slot.refusalCode ? el('span', { class: 'note' }, ' (' + slot.refusalCode + ')') : null,
          slot.backupExists && slot.backupLoadable ? el('span', { class: 'note' }, ' · backup available') : null),
        el('td', null, el('div', { class: 'slot-actions' }, ...actions)));
    });
    byId('saves-rows').replaceChildren(...(rows.length > 0 ? rows
      : [el('tr', null, el('td', { colspan: 5, class: 'save-empty' }, label('MultiSavesEmpty') || 'No saved slots yet.'))]));
    updateSaveControls();
  }

  function saveNameValid(name) { return /^[a-z0-9][a-z0-9-]{0,47}$/.test(name); }

  function updateSaveControls() {
    const name = byId('save-slot').value.trim();
    const existing = saveNameValid(name) ? savedSlot(name) : null;
    byId('save-overwrite-wrap').hidden = !existing;
    if (!existing) byId('save-overwrite').checked = false;
    byId('save-do').disabled = !saveNameValid(name) || state.busy;
  }

  async function onSave() {
    if (!state.view || state.busy) return;
    const slot = byId('save-slot').value.trim();
    if (!saveNameValid(slot)) {
      saveStatus('A slot name is 1–48 lowercase letters, digits or hyphens, starting with a letter or digit. Nothing was written.', 'bad');
      return;
    }
    if (state.lock) {
      saveStatus('Outcome unconfirmed — re-read the current state before saving. Nothing was written.', 'bad');
      return;
    }
    const existing = savedSlot(slot);
    if (existing && !byId('save-overwrite').checked) {
      saveStatus('Slot "' + slot + '" already holds a save. Tick "' + label('MultiSavesOverwrite') + '" to replace it. Nothing was written.', 'warn');
      return;
    }
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await jsonCall('api/multi/save', 'POST', { fence: currentFence(), slot, replaceExisting: Boolean(existing) });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    if (outcome.kind === 'ok' && outcome.payload && outcome.payload.saved) {
      const saved = outcome.payload.saved;
      const durable = saved.status === 'durablyCommitted';
      saveStatus('Saved "' + saved.slot + '" · year ' + saved.ecologicalYear + ' · ' + saved.savedUtc + ' · ' +
        (durable ? label('MultiSavesDurable') : label('MultiSavesDurabilityUnconfirmed')), durable ? 'ok' : 'warn');
      byId('save-overwrite').checked = false;
      announce('Saved slot ' + saved.slot + ' in year ' + saved.ecologicalYear + '.');
      await refreshSaves();
      return;
    }
    const notice = noticeOf(outcome);
    saveStatus('Not saved (' + notice.code + '): ' + notice.message, 'bad');
  }

  function askLoad(slot) {
    const row = savedSlot(slot);
    if (!row || !row.canLoad || state.busy) return;
    byId('recover-confirm').hidden = true;
    state.recoverSlot = null;
    state.loadSlot = slot;
    byId('load-confirm-text').textContent =
      fill(label('MultiSavesLoadConfirm'), { slot: slot });
    byId('load-confirm').hidden = false;
    byId('load-do').focus();
  }

  // A load replaces the whole live forest, so every local selection, draft, preview, in-flight request token and pending
  // confirmation of the old session is dropped here BEFORE the new payload is applied.
  function resetForLoad() {
    state.selected = {};
    state.selectedJuvenile = null;
    state.drafts = {};
    state.draftNotice = null;
    state.overlap = null;
    state.competition = null;
    state.competitionTarget = null;
    state.competitionTag = null;
    state.competitionRequest += 1;
    state.cellDetail = null;
    state.cellLoading = false;
    state.batch = null;
    state.batchResult = null;
    state.batchResultOpen = false;
    state.plantDraft = null;
    state.plantResult = null;
    state.spotDraft = null;
    state.spotResult = null;
    state.confirmation = null;
    state.consentWork = false;
    state.lastExecution = null;
    state.lastWasReplay = false;
    state.lock = null;
    state.lockCount = 0;
    state.mapPick += 1;
    state.cellPick += 1;
    state.zoom = 1;
    state.centre = null;
    state.mapMode = 'tree';
    state.overlay = 'off';
    state.loadSlot = null;
    state.historyTruncated = false;
    state.totalRecordedYears = 0;
    state.reviewPreview = null;
    state.previewRequest += 1;
    state.lastSkip = null;
    state.historyItems = [];
    state.historyNextCursor = null;
    state.historyTotalRecorded = 0;
    state.historySelectedYear = null;
    state.historyDetail = null;
    state.historyError = null;
    state.historyLoading = false;
    state.historyDetailLoading = false;
    state.historyRequest += 1;
    state.historyDetailRequest += 1;
    state.structure = null;
    state.structureLoading = false;
    state.structureError = null;
    state.structureRootTag = null;
    state.structureRequest += 1;
  }

  async function onLoadConfirmed() {
    const slot = state.loadSlot;
    if (!slot || !state.view || state.busy) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await jsonCall('api/multi/load', 'POST', { fence: currentFence(), slot });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    byId('load-confirm').hidden = true;
    if (outcome.kind === 'ok' && outcome.payload && outcome.payload.view) {
      resetForLoad();
      applyPayload(outcome.payload, false);
      byId('saves-dialog').close();
      const info = outcome.payload.restored;
      const draft = info && info.draftOrderCount > 0 ? fill(label('MultiSavesPendingDraft'), { n: info.draftOrderCount }) : null;
      clearAlert();
      showAlert([
        el('p', { class: 'alert-title' }, 'Loaded "' + slot + '"'),
        el('p', null, 'Year ' + state.view.source.ecologicalYear + ', both areas restored from the save; the session has a new authority, so every earlier token is refused.'),
        draft ? el('p', { class: 'warning' }, draft) : null,
        el('p', null, 'Advance or approve and execute work to continue from here.'),
      ]);
      announce('Loaded ' + slot + ' in year ' + state.view.source.ecologicalYear + '.' + (draft ? ' ' + draft : ''));
      refreshReviewPreview();
      if (state.historyOpen) loadHistoryIndex(null, false);
      if (byId('side').getAttribute('data-pane') === 'details') refreshStructure();
      return;
    }
    if (outcome.kind === 'refused' && outcome.payload && outcome.payload.view) {
      // The server replaced nothing: it answered with the unchanged live session, so the page keeps working authority.
      applyPayload(outcome.payload, false);
      const notice = noticeOf(outcome);
      saveStatus('Load refused (' + notice.code + '): ' + notice.message + ' The current forest is unchanged.', 'bad');
      return;
    }
    const notice = noticeOf(outcome);
    saveStatus('Load not confirmed (' + notice.code + '): ' + notice.message + ' Re-read the current state before trying again.', 'bad');
  }

  function askRecover(slot) {
    const row = savedSlot(slot);
    if (!row || !row.recoverable || state.busy) return;
    byId('load-confirm').hidden = true;
    state.loadSlot = null;
    state.recoverSlot = slot;
    byId('recover-confirm-text').textContent = fill(label('MultiSavesRecoverConfirm'), { slot: slot });
    byId('recover-confirm').hidden = false;
    byId('recover-do').focus();
  }

  async function onRecoverConfirmed() {
    const slot = state.recoverSlot;
    if (!slot || state.busy) return;
    state.busy = true;
    document.documentElement.classList.add('busy');
    const outcome = await jsonCall('api/multi/saves/recover', 'POST', { slot });
    state.busy = false;
    document.documentElement.classList.remove('busy');
    byId('recover-confirm').hidden = true;
    state.recoverSlot = null;
    if (outcome.kind === 'ok') {
      saveStatus('Restored the verified backup of "' + slot + '" over its unreadable primary file. Load it to play that forest.', 'ok');
      await refreshSaves();
      return;
    }
    const notice = noticeOf(outcome);
    saveStatus('Backup not restored (' + notice.code + '): ' + notice.message, 'bad');
  }

  // ---- alerts, announcements and the re-read lock ---------------------------------------------------------------------

  function clearAlert() { const box = byId('alert'); box.replaceChildren(); box.classList.remove('shown'); }
  function showAlert(parts, focusTarget) {
    const box = byId('alert');
    box.replaceChildren(...parts.filter((part) => part !== null));
    box.classList.add('shown');
    (focusTarget || box).focus();
  }
  function announce(message) {
    const region = byId('announce');
    region.textContent = '';
    window.setTimeout(() => { region.textContent = message; }, 50);
  }
  function rereadButton() {
    const button = el('button', { type: 'button' }, 'Re-read current state');
    button.addEventListener('click', () => reread(true));
    return button;
  }

  function recoveryFor(code) {
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

  function showRefusal(what, refusal) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused: ' + what),
      el('p', null, el('code', null, refusal.code), ' — ' + refusal.message),
      el('p', null, label('MultiNothingChanged') + '. Current ecological year ' + state.view.source.ecologicalYear + '.'),
      el('p', null, recoveryFor(refusal.code)),
    ]);
  }

  function showGuardRefusal(what, status, error) {
    showAlert([
      el('p', { class: 'alert-title' }, 'Refused by the local page server: ' + what),
      el('p', null, el('code', null, error.code), ' (HTTP ' + status + ') — ' + error.message),
      el('p', null, 'The request did not reach the engine.'),
      rereadButton(),
    ]);
  }

  function lockCommands(title, detail) {
    state.lock = { title, detail };
    state.lockCount += 1;
    renderAll();
    showLockAlert(null);
  }

  function showLockAlert(extra) {
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

  function refusedByLock(what) {
    if (!state.lock) return false;
    showLockAlert('Not sent: ' + what + '.');
    return true;
  }

  function addAvailability() {
    const areaId = state.areaId;
    if (state.lock) return { ok: false, why: 'Re-read the current state before adding.' };
    if (state.view && state.view.approved) return { ok: false, why: 'The plan is approved, so editing is locked. Cancel the approved plan to change it.' };
    if (!currentReport()) return { ok: false, why: 'Waiting for the current competition reading.' };
    const eligible = draftOf(areaId).filter((id) => !blockedReason(areaId, id));
    if (eligible.length === 0) return { ok: false, why: 'Tick one or more competitors to add.' };
    return { ok: true, why: null, count: eligible.length };
  }

  function renderCommandGate() {
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
      renderPruneGate();
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

  // ---- Execute confirmation -------------------------------------------------------------------------------------------

  function frozen(value) {
    if (value && typeof value === 'object') {
      for (const inner of Object.values(value)) frozen(inner);
      Object.freeze(value);
    }
    return value;
  }

  function confirmationOf(view) {
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

  function confirmationStillShown(confirmation, view) {
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

  function withdrawConfirmation(confirmation) {
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
    renderMap();
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
    if (state.ecology && state.ecology.experimentBadge) return state.ecology.experimentBadge;
    return state.view && state.view.experiment.experimentId === 'mad-ma1-mixed-demo-v1'
      ? 'Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)'
      : 'Synthetic MA-1 (all Sitka)';
  }

  function renderStatus() {
    const view = state.view;
    const badgeText = experimentBadgeText();
    document.title = 'Two-area Stand Map — ' + badgeText;
    byId('page-title').textContent = 'Two-area Stand Map';
    const badgeEl = byId('experiment-badge');
    badgeEl.textContent = badgeText;
    badgeEl.setAttribute('title', badgeText);
    byId('year-value').textContent = String(view.source.ecologicalYear);
    byId('advance-label').textContent = label('MultiAdvanceYear');
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

  // ---- the map: 1 SVG user unit = 1 metre; U east, V south ------------------------------------------------------------

  function fullBox(area) {
    const side = area.extent.sideM;
    return { x: -3.4, y: -2.2, w: side + 6.0, h: side + 5.4 };
  }

  function mapFrame(area) {
    const full = fullBox(area);
    const pane = byId('map-pane');
    const dock = byId('map-dock');
    const W = Math.max(pane.clientWidth, 1);
    const H = Math.max(pane.clientHeight, 1);
    const dockRight = dock.offsetLeft + dock.offsetWidth + 10;
    const dockBottom = dock.offsetTop + dock.offsetHeight + 10;
    const besideScale = Math.min((W - dockRight) / full.w, H / full.h);
    const belowScale = Math.min(W / full.w, (H - dockBottom) / full.h);
    const beside = besideScale >= belowScale;
    return { full, W, H, fit: Math.max(beside ? besideScale : belowScale, 1e-3),
      reserveLeft: beside ? dockRight : 0, reserveTop: beside ? 0 : dockBottom };
  }

  function viewAxis(centre, usable, reserve, start, length) {
    if (usable >= length - 1e-9) return { pos: start - reserve - (usable - length) / 2, back: false, forward: false };
    const lo = start - reserve;
    const hi = start + length - usable - reserve;
    const pos = Math.min(hi, Math.max(lo, centre - usable / 2 - reserve));
    return { pos, back: pos > lo + 1e-6, forward: pos < hi - 1e-6 };
  }

  function viewBox(area) {
    const frame = mapFrame(area);
    const full = frame.full;
    const scale = frame.fit * state.zoom;
    const w = frame.W / scale, h = frame.H / scale;
    const rl = frame.reserveLeft / scale, rt = frame.reserveTop / scale;
    const uw = w - rl, uh = h - rt;
    const centre = state.centre || { u: full.x + full.w / 2, v: full.y + full.h / 2 };
    const ax = viewAxis(centre.u, uw, rl, full.x, full.w);
    const ay = viewAxis(centre.v, uh, rt, full.y, full.h);
    return { x: ax.pos, y: ay.pos, w, h, scale, uw, uh, rl, rt,
      west: ax.back, east: ax.forward, north: ay.back, south: ay.forward };
  }

  function usableCentre(box) { return { u: box.x + box.rl + box.uw / 2, v: box.y + box.rt + box.uh / 2 }; }

  const FOCUS_SCALE = 2;
  const FOCUS_GAP_PX = 0;
  const FOCUS_TOUCH = .8;
  function spreadFocus(areaId, report, mpp, box) {
    const ids = [report.targetTreeId, ...report.topFive.map((n) => n.treeId)];
    const base = ids.map((id) => itemOf(areaId, id)).filter(Boolean);
    if (!base.length || base[0].treeId !== report.targetTreeId) return new Map();
    const lo = { u: box.x + box.rl + 34 * mpp, v: box.y + box.rt + 34 * mpp };
    const hi = { u: box.x + box.w - 34 * mpp, v: box.y + box.h - 48 * mpp };
    const nodes = base.map((item) => ({ ...item, radiusM: item.radiusM * FOCUS_SCALE }));
    const within = (x, a, b) => (a > b ? (a + b) / 2 : Math.min(Math.max(x, a), b));
    for (let round = 0; round < 8; round++) {
      separate(nodes, FOCUS_GAP_PX * mpp);
      let held = false;
      for (const n of nodes.slice(1)) {
        const u = within(n.u, lo.u + n.radiusM, hi.u - n.radiusM), v = within(n.v, lo.v + n.radiusM, hi.v - n.radiusM);
        if (u !== n.u || v !== n.v) { n.u = u; n.v = v; held = true; }
      }
      if (!held) break;
    }
    return new Map(nodes.map((node) => [node.treeId, node]));
  }

  function separate(nodes, pad) {
    const anchor = nodes[0];
    for (let pass = 0; pass < 80; pass++) {
      let moved = false;
      for (let i = 0; i < nodes.length; i++) for (let j = i + 1; j < nodes.length; j++) {
        const a = nodes[i], b = nodes[j];
        let dx = b.u - a.u, dy = b.v - a.v, d = Math.hypot(dx, dy);
        if (d < 1e-6) { dx = Math.cos(j * 2.399); dy = Math.sin(j * 2.399); d = 0; } else { dx /= d; dy /= d; }
        const need = (a.radiusM + b.radiusM) * FOCUS_TOUCH + pad - d;
        if (need <= 0) continue;
        moved = true;
        const shareA = a === anchor ? 0 : b === anchor ? 1 : .5;
        a.u -= dx * need * shareA; a.v -= dy * need * shareA;
        b.u += dx * need * (1 - shareA); b.v += dy * need * (1 - shareA);
      }
      if (!moved) break;
    }
    return nodes;
  }

  const crownCache = new Map();

  function canopyHash(a, b, salt) {
    let h = Math.imul(a ^ 0x9e3779b1, 0x85ebca77) ^ Math.imul(b ^ salt, 0xc2b2ae3d);
    h = Math.imul(h ^ (h >>> 15), 0x2c1b3c6d);
    h = Math.imul(h ^ (h >>> 12), 0x297a2d39);
    return (h ^ (h >>> 15)) >>> 0;
  }
  function canopySeed(id) {
    let seed = 7;
    for (const ch of String(id || 'legend')) seed = canopyHash(seed, ch.charCodeAt(0), 7);
    return seed;
  }
  function canopyRandom(seed, channel) { return (canopyHash(seed, channel, 2) & 0xffffff) / 16777216; }

  function groundNoise(x, y, cells, seed) {
    const fx = x * cells, fy = y * cells;
    const ix = Math.floor(fx), iy = Math.floor(fy);
    const smooth = (t) => t * t * (3 - 2 * t);
    const tx = smooth(fx - ix), ty = smooth(fy - iy);
    const at = (a, b) => (canopyHash(a % cells, b % cells, seed) & 0xffffff) / 16777216;
    const lerp = (a, b, t) => a + (b - a) * t;
    return lerp(lerp(at(ix, iy), at(ix + 1, iy), tx), lerp(at(ix, iy + 1), at(ix + 1, iy + 1), tx), ty);
  }

  function initFloorDefs() {
    const defs = byId('map').querySelector('defs');
    if (byId('slate-ground')) return;
    [12.8, 9.7].forEach((period, index) => {
      const tile = svg('pattern', { id: 'slate-' + index, patternUnits: 'userSpaceOnUse', width: period, height: period });
      const seed = index === 0 ? 0x51ed270b : 0x9e3779b9;
      const size = 24, cell = period / size;
      for (let y = 0; y < size; y++) for (let x = 0; x < size; x++) {
        const n = 255 * (.65 * groundNoise(x / size, y / size, 14, seed) +
          .35 * groundNoise(x / size, y / size, 5, seed ^ 0xabcd)) - 128;
        const colour = SLATE.map((v) => Math.round(v + n / 14));
        tile.append(svg('rect', { x: n3(x * cell), y: n3(y * cell), width: n3(cell + .005), height: n3(cell + .005),
          fill: 'rgb(' + colour.join(',') + ')' }));
      }
      defs.append(tile);
    });
    const floor = svg('pattern', { id: 'slate-ground', patternUnits: 'userSpaceOnUse', width: 124.16, height: 124.16 });
    floor.append(svg('rect', { width: 124.16, height: 124.16, fill: 'url(#slate-0)' }),
      svg('rect', { width: 124.16, height: 124.16, fill: 'url(#slate-1)', opacity: .4 }));
    defs.append(floor);
  }

  // Strictly circular crown geometry (spec §5.6): x = u - r, y = v - r, width = height = 2 * r, where r = item.radiusM.
  // Zero elliptical stretch; zero DBH->radius maths in JS. Deterministic seeded rotation around (u, v) is allowed.
  function crownDecoration(item, sp) {
    const u = item.u, v = item.v, r = item.radiusM;
    if (item.status === 'Felled') {
      return [
        svg('circle', { class: 'stump-disc deco', cx: n3(u), cy: n3(v), r: n3(r) }),
        svg('circle', { class: 'stump-ring deco', cx: n3(u), cy: n3(v), r: n3(r * .65) }),
        svg('circle', { class: 'stump-ring deco', cx: n3(u), cy: n3(v), r: n3(r * .32) }),
      ];
    }
    const seed = canopySeed(item.treeId);
    const turn = Math.round(canopyRandom(seed, 21) * 360);
    const dead = item.status === 'NaturalDead';
    const symbolId = dead ? 'ccf-standing-dead' : (SPECIES_SYMBOL[sp] || 'ccf-sitka');
    const diam = n3(2 * r);
    return [
      svg('circle', { class: 'deco', cx: n3(u + r * .16), cy: n3(v + r * .18), r: n3(r * 1.08),
        fill: 'url(#canopy-shadow)', opacity: dead ? .4 : .85 }),
      svg('use', {
        class: 'deco crown-art',
        href: '#' + symbolId,
        x: n3(u - r),
        y: n3(v - r),
        width: diam,
        height: diam,
        transform: 'rotate(' + turn + ' ' + n3(u) + ' ' + n3(v) + ')',
        'data-radius-m': n3(r),
        'data-species': sp,
      }),
    ];
  }

  function renderMap() {
    const area = currentArea();
    if (!area) return;
    const side = area.extent.sideM;
    const box = viewBox(area);
    const mpp = 1 / box.scale;
    byId('map').setAttribute('viewBox', [box.x, box.y, box.w, box.h].map((n) => n.toFixed(4)).join(' '));
    setAttributes(byId('ground'), { x: 0, y: 0, width: side, height: side });
    const toU = (x) => x - area.extent.minXM;
    const toV = (z) => area.extent.minZM + side - z;
    const outline = area.polygon.map((p) => toU(p.xm) + ',' + toV(p.zm)).join(' ');
    byId('area-outline').setAttribute('points', outline);
    byId('area-clip-shape').setAttribute('points', outline);
    drawGrid(area, mpp, box.scale);

    const inTreeMode = state.mapMode === 'tree';
    const report = inTreeMode ? currentReport() : null;
    const reference = referenceIn(area.areaId);
    const draft = new Set(draftOf(area.areaId));
    const focusIds = report ? new Set([report.targetTreeId, ...report.topFive.map((n) => n.treeId)]) : null;
    const crowns = [];
    const focusCrowns = [];
    const marks = [];
    const ranks = [];
    const spread = focusIds ? spreadFocus(area.areaId, report, mpp, box) : new Map();
    const items = new Map(sceneOf(area.areaId).map((item) => [item.treeId, spread.get(item.treeId) || item]));
    for (const [treeId, shown] of spread) {
      const real = items.get(treeId) === shown ? itemOf(area.areaId, treeId) : null;
      if (real && Math.hypot(shown.u - real.u, shown.v - real.v) > 2 * mpp) {
        marks.push(svg('path', { class: 'spread-leader', d: 'M ' + n3(real.u) + ' ' + n3(real.v) + ' L ' + n3(shown.u) + ' ' + n3(shown.v) }),
          svg('circle', { class: 'spread-origin', cx: n3(real.u), cy: n3(real.v), r: n3(2.5 * mpp) }));
      }
    }
    for (const raw of sceneOf(area.areaId).slice().sort((a, b) => a.paintOrder - b.paintOrder)) {
      const item = items.get(raw.treeId);
      const sp = speciesOf(item.treeId);
      const focused = item !== raw;
      const dimmed = focusIds && !focused;
      const cacheKey = area.areaId + '|' + item.treeId;
      const drawKey = [item.status, sp, n3(item.u), n3(item.v), n3(item.radiusM)].join('|');
      let cached = crownCache.get(cacheKey);
      if (!cached || cached.drawKey !== drawKey) {
        const group = svg('g', { 'data-tree-id': item.treeId, 'data-species': sp });
        // Transparent true-radius hit disc preserved underneath crown art
        group.append(svg('circle', { class: 'crown', cx: n3(item.u), cy: n3(item.v), r: n3(item.radiusM) }));
        group.append(...crownDecoration(item, sp));
        cached = { drawKey, node: group };
        crownCache.set(cacheKey, cached);
      }
      const className = 'tree s-' + item.status + (dimmed ? ' dimmed' : '');
      if (cached.node.getAttribute('class') !== className) cached.node.setAttribute('class', className);
      (focused ? focusCrowns : crowns).push(cached.node);
      marks.push(...statusMarks(item, mpp), ...pruneMarks(area.areaId, item, mpp));
    }
    if (report) {
      const taken = [];
      const focus = [...spread.values()];
      for (const f of focus) {
        if (f.isCrop) taken.push(tileCentre(f, TILE.crop, mpp));
        if (f.status === 'PendingFell' || f.status === 'ApprovedFell') taken.push(tileCentre(f, TILE.status, mpp));
      }
      const target = items.get(report.targetTreeId);
      if (target) {
        taken.push(tileCentre(target, TILE.reference, mpp));
        for (const dx of [-30, -10, 10, 30]) taken.push([target.u + dx * mpp, target.v - target.radiusM - 22 * mpp]);
      }
      const free = (x, y) => taken.every(([tx, ty]) => Math.hypot(tx - x, ty - y) >= 34 * mpp) &&
        focus.every((f) => Math.hypot(f.u - x, f.v - y) >= f.radiusM * .55);
      report.topFive.forEach((entry, rank) => {
        const item = items.get(entry.treeId);
        if (!item) return;
        const chosen = draft.has(entry.treeId);
        const fits = (a) => { const [x, y] = tileCentre(item, a, mpp); return free(x, y) && (!chosen || free(x - 23 * mpp, y)); };
        const angle = RANK_ANGLES.find(fits) ?? TILE.rank;
        const [x, y] = tileCentre(item, angle, mpp);
        taken.push([x, y]);
        if (chosen) taken.push([x - 23 * mpp, y]);
        marks.push(underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0));
        ranks.push({ treeId: entry.treeId, rank: rank + 1, x, y, chosen });
      });
    }
    crowns.push(...focusCrowns);
    const host = byId('crowns');
    const unchanged = host.childNodes.length === crowns.length && crowns.every((node, index) => host.childNodes[index] === node);
    if (!unchanged) host.replaceChildren(...crowns);

    renderOverlayCells(area.areaId, mpp);
    renderClearCells(area.areaId);
    renderSpotPatches(area.areaId);
    renderDeadwoodLayer(area.areaId, mpp);
    renderPlantedLayer(area.areaId, mpp);
    renderDraftPreview(area.areaId, mpp);
    renderGroundMark(area.areaId);
    byId('marks').replaceChildren(...marks);
    renderRankLayer(ranks, box);

    const chosen = inTreeMode && !currentJuvenile() && selectedId() ? items.get(selectedId()) : null;
    const lit = chosen && !state.lock ? (chosen.treeId === reference ? referenceMarks(chosen, mpp, true) : inspectMarks(chosen, mpp, true)) : [];
    byId('selection').replaceChildren(...lit);

    byId('map-desc').textContent = 'North-up map of ' + areaName(area.areaId) + ', ' + area.areaM2Exact + ' square metres. ' +
      'Ecological year ' + state.view.source.ecologicalYear + '. Living trees ' + area.stand.livingCount + '.';
    byId('map-subtitle').textContent = area.label + ' · ' + fixed(area.areaHa, 4) + ' ha';
    byId('map-note').textContent = areaName(area.areaId) + ' · ' + (state.zoom === 1 ? 'Fit whole area' :
      fixed(state.zoom, 2) + '× Fit · view ' + fixed(box.uw, 1) + ' × ' + fixed(box.uh, 1) + ' m');
    byId('zoom-state').textContent = state.zoom === 1 ? 'Fit' : fixed(state.zoom, 2) + '×';
    renderScale(box.scale);
    renderMapStatus(reference, draft.size);
  }

  function renderOverlayCells(areaId, mpp) {
    const host = byId('overlay-cells');
    const s4 = s4Area(areaId);
    if (state.overlay === 'off' || !s4) { host.replaceChildren(); return; }
    const nodes = [];
    if (state.overlay === 'light') {
      for (const cell of s4.ground) {
        const paint = lightPaint(cell.lightIndex);
        if (!paint) continue;
        nodes.push(svg('polygon', { class: 'light-cell', 'data-cell': cell.column + ':' + cell.row, 'data-light': cell.lightIndex,
          fill: paint.colour, 'fill-opacity': paint.opacity, points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
      }
    } else if (state.overlay === 'undergrowth') {
      const compact = compactCellsFor(areaId);
      for (const cell of s4.ground) {
        const row = compact.get(cell.column + ':' + cell.row);
        if (!row) continue;
        const bramble = row[4], bracken = row[5];
        const pts = cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ');
        if (bramble > 0) {
          nodes.push(svg('polygon', { class: 'undergrowth-cell', 'data-kind': 'bramble', fill: 'url(#ccf-bramble-hatch)',
            'fill-opacity': opacityText(Math.min(0.95, 0.2 + 0.75 * bramble)), points: pts }));
        }
        if (bracken > 0) {
          nodes.push(svg('polygon', { class: 'undergrowth-cell', 'data-kind': 'bracken', fill: 'url(#ccf-bracken-hatch)',
            'fill-opacity': opacityText(Math.min(0.95, 0.2 + 0.75 * bracken)), points: pts }));
        }
      }
    } else if (state.overlay === 'seedlings') {
      // Cell-level presence glyph only: never fabricated individual stem dots; never treats Density as a literal stem count.
      const compact = compactCellsFor(areaId);
      for (const cell of s4.ground) {
        const row = compact.get(cell.column + ':' + cell.row);
        if (!row || row[7] <= 0) continue;
        const mask = row[6];
        const tags = [];
        if (mask & 1) tags.push('S');
        if (mask & 2) tags.push('O');
        if (mask & 4) tags.push('B');
        const gSize = Math.min(1.6, Math.max(0.9, 18 * mpp));
        nodes.push(svg('polygon', { fill: '#355c2b', 'fill-opacity': '0.32', stroke: '#7bc45c', 'stroke-opacity': '0.55',
          'stroke-width': n3(1 * mpp), points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
        nodes.push(svg('use', { href: '#ccf-natural-seedlings', x: n3(cell.centreU - gSize / 2), y: n3(cell.centreV - gSize * 0.6),
          width: n3(gSize), height: n3(gSize) }));
        if (tags.length > 0) {
          nodes.push(svg('text', { class: 'seedling-tag', x: n3(cell.centreU), y: n3(cell.centreV + gSize * 0.45),
            'font-size': n3(Math.min(0.75, 9 * mpp)) }, tags.join('·')));
        }
      }
    }
    host.replaceChildren(...nodes);
  }

  // Rule 1: ClearUndergrowth highlights the whole 2.5 m cell (sparing seedlings inside), never a tree-crown badge.
  function renderClearCells(areaId) {
    const host = byId('clear-cells');
    if (!state.ecology || !s4Area(areaId)) { host.replaceChildren(); return; }
    const nodes = [];
    const drawClear = (orders, cls) => {
      for (const o of orders) {
        if (o.areaId !== areaId) continue;
        const cell = groundCell(areaId, o.column, o.row);
        if (!cell) continue;
        nodes.push(svg('polygon', { class: cls, 'data-clear-cell': o.column + ':' + o.row,
          points: cell.polygon.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') }));
      }
    };
    drawClear(state.ecology.pendingClear || [], 'clear-cell-pending');
    drawClear(state.ecology.approvedClear || [], 'clear-cell-approved');
    host.replaceChildren(...nodes);
  }

  // Rule 2: SpotClear (~1 m2): circular dashed ring drawn to exact world scale r = patch.radiusM (~0.564 m)
  // over bare soil/twigs with NO seedling inside, rendered in its own layer BENEATH planted PJ saplings.
  function renderSpotPatches(areaId) {
    const host = byId('spot-patches');
    if (!state.ecology) { host.replaceChildren(); return; }
    const nodes = [];
    for (const p of (state.ecology.patches || [])) {
      if (p.areaId !== areaId) continue;
      const r = p.radiusM;
      nodes.push(svg('use', { href: '#ccf-spot-clear', 'data-patch-id': p.patchId, 'data-radius-m': n3(r),
        x: n3(p.u - r), y: n3(p.v - r), width: n3(2 * r), height: n3(2 * r) }));
    }
    const drawQueuedSpot = (orders, cls, stateTag) => {
      for (const o of orders) {
        if (o.areaId !== areaId || o.kind !== 'SpotClear') continue;
        const r = SPOT_PREVIEW_RADIUS_M;
        const g = svg('g', { 'data-spot-order': o.orderId, 'data-order-state': stateTag });
        g.append(svg('title', null, o.orderId + ' · ' + stateTag + ' SpotClear (~1 m², r ≈ 0.564 m, no seedling)'));
        g.append(svg('circle', { class: cls, cx: n3(o.u), cy: n3(o.v), r: n3(r) }));
        nodes.push(g);
      }
    };
    drawQueuedSpot(pendingPlantings(), 'spot-order-pending', 'Pending');
    drawQueuedSpot(approvedPlantings(), 'spot-order-approved', 'Approved');
    host.replaceChildren(...nodes);
  }

  function renderDeadwoodLayer(areaId, mpp) {
    const host = byId('deadwood');
    const s4 = s4Area(areaId);
    if (!s4) { host.replaceChildren(); return; }
    const w = 26 * mpp, h = 10 * mpp;
    const nodes = s4.canopy.fallenDeadwood.map((record) => {
      const sp = speciesLabel(speciesOf(record.treeId));
      const g = svg('g', { class: 'log', 'data-deadwood-id': record.deadwoodId, 'data-species': speciesOf(record.treeId) });
      g.append(svg('title', null, record.deadwoodId + ' · ' + sp + ' fallen log (year ' + record.deathYear + ', direction schematic, not simulated)'));
      g.append(svg('use', { href: '#ccf-log', x: n3(record.u - w / 2), y: n3(record.v - h / 2), width: n3(w), height: n3(h) }));
      return g;
    });
    host.replaceChildren(...nodes);
  }

  // Planted juveniles (PJ), active/expired shelters, and queued Plant/Shelter orders ABOVE SpotClear patches.
  function renderPlantedLayer(areaId, mpp) {
    const host = byId('planted-layer');
    if (!state.ecology) { host.replaceChildren(); return; }
    const nodes = [];
    const sSize = Math.max(0.75, 18 * mpp);
    const selectedPj = currentJuvenile();
    for (const j of (state.ecology.planted || [])) {
      if (j.areaId !== areaId || !j.isActive) continue;
      const sym = SAPLING_SYMBOL[j.speciesId] || 'ccf-sapling-oak';
      const g = svg('g', { class: 'planted-pj', 'data-juvenile-id': j.juvenileId, 'data-species': j.speciesId });
      g.append(svg('title', null, j.juvenileId + ' · ' + speciesLabel(j.speciesId) + ' planted sapling, H ' + fixed(j.heightM, 2) + ' m (click to inspect)'));
      g.append(svg('circle', { class: 'pj-hit', cx: n3(j.u), cy: n3(j.v), r: n3(Math.max(0.45, sSize * 0.6)) }));
      g.append(svg('use', { href: '#' + sym, x: n3(j.u - sSize / 2), y: n3(j.v - sSize / 2), width: n3(sSize), height: n3(sSize) }));
      if (selectedPj && selectedPj.juvenileId === j.juvenileId) {
        g.append(svg('circle', { class: 'pj-selected-ring', cx: n3(j.u), cy: n3(j.v), r: n3(sSize * 0.65) }));
      }
      nodes.push(g);
    }
    const shSize = Math.max(0.95, 22 * mpp);
    for (const sh of (state.ecology.shelters || [])) {
      if (sh.areaId !== areaId) continue;
      const sym = sh.isEffective ? 'ccf-shelter-active' : 'ccf-shelter-expired';
      const g = svg('g', { class: 'shelter-mark', 'data-shelter-id': sh.shelterId, 'data-juvenile-target': sh.juvenileId,
        'data-effective': String(sh.isEffective) });
      g.append(svg('title', null, sh.shelterId + ' (' + (sh.isEffective ? 'Active shelter · expires year ' + sh.expiryYear : 'Expired shelter · expired year ' + sh.expiryYear) + ')'));
      g.append(svg('use', { href: '#' + sym, x: n3(sh.u - shSize / 2), y: n3(sh.v - shSize / 2), width: n3(shSize), height: n3(shSize) }));
      nodes.push(g);
    }
    const qSize = Math.max(0.75, 18 * mpp);
    const drawQueuedPlanting = (orders, approved) => {
      const ringCls = approved ? 'plant-order-ring-approved' : 'plant-order-ring-pending';
      const prefix = approved ? '✓' : '◷';
      const stateWord = approved ? 'Approved' : 'Pending';
      for (const o of orders) {
        if (o.areaId !== areaId || o.kind === 'SpotClear') continue;
        const iconId = o.kind === 'Plant' ? 'ccf-action-plant' : 'ccf-action-shelter';
        const dx = o.kind === 'Shelter' ? qSize * 0.42 : 0;
        const g = svg('g', { 'data-planting-order': o.orderId, 'data-order-kind': o.kind, 'data-order-state': stateWord });
        g.append(svg('title', null, o.orderId + ' · ' + prefix + ' ' + stateWord + ' ' + o.kind +
          (o.speciesId ? ' (' + speciesLabel(o.speciesId) + ')' : '')));
        g.append(svg('circle', { class: ringCls, cx: n3(o.u + dx), cy: n3(o.v), r: n3(qSize * 0.56) }));
        g.append(svg('use', { href: '#' + iconId, x: n3(o.u + dx - qSize * 0.44), y: n3(o.v - qSize * 0.44),
          width: n3(qSize * 0.88), height: n3(qSize * 0.88) }));
        nodes.push(g);
      }
    };
    drawQueuedPlanting(pendingPlantings(), false);
    drawQueuedPlanting(approvedPlantings(), true);
    host.replaceChildren(...nodes);
  }

  // Provisional local Draft previews (dashed slate outline, snap OFF, zero engine order).
  function renderDraftPreview(areaId, mpp) {
    const host = byId('draft-preview');
    const nodes = [];
    const arm = Math.max(0.32, 7 * mpp);
    if (state.mapMode === 'plant' && state.plantDraft && state.plantDraft.areaId === areaId) {
      const d = state.plantDraft;
      const wantSpot = Boolean(byId('plant-opt-spot') && byId('plant-opt-spot').checked);
      const wantShelter = Boolean(byId('plant-opt-shelter') && byId('plant-opt-shelter').checked);
      const g = svg('g', { id: 'draft-plant-preview', 'data-draft-kind': 'plant' });
      if (wantSpot) {
        g.append(svg('circle', { class: 'draft-ring', 'data-draft-spot-ring': 'true', cx: n3(d.u), cy: n3(d.v), r: n3(SPOT_PREVIEW_RADIUS_M) }));
      }
      g.append(svg('circle', { class: 'draft-ring', cx: n3(d.u), cy: n3(d.v), r: n3(Math.max(0.28, 8 * mpp)) }));
      g.append(svg('path', { class: 'draft-cross', d: 'M ' + n3(d.u - arm) + ' ' + n3(d.v) + ' L ' + n3(d.u + arm) + ' ' + n3(d.v) +
        ' M ' + n3(d.u) + ' ' + n3(d.v - arm) + ' L ' + n3(d.u) + ' ' + n3(d.v + arm) }));
      const labelText = 'Draft Plant' + (wantShelter ? ' + Shelter' : '') + (wantSpot ? ' + Spot' : '');
      g.append(svg('text', { class: 'draft-label', x: n3(d.u), y: n3(d.v - Math.max(0.75, 15 * mpp)), 'font-size': n3(Math.max(0.45, 10 * mpp)) }, labelText));
      nodes.push(g);
    } else if (state.mapMode === 'spot' && state.spotDraft && state.spotDraft.areaId === areaId) {
      const d = state.spotDraft;
      const g = svg('g', { id: 'draft-spot-preview', 'data-draft-kind': 'spot' });
      // Seedling-free 0.564 m circular preview at exact (X, Z)
      g.append(svg('circle', { class: 'draft-ring', 'data-draft-spot-ring': 'true', cx: n3(d.u), cy: n3(d.v), r: n3(SPOT_PREVIEW_RADIUS_M) }));
      g.append(svg('path', { class: 'draft-cross', d: 'M ' + n3(d.u - arm * 0.6) + ' ' + n3(d.v) + ' L ' + n3(d.u + arm * 0.6) + ' ' + n3(d.v) +
        ' M ' + n3(d.u) + ' ' + n3(d.v - arm * 0.6) + ' L ' + n3(d.u) + ' ' + n3(d.v + arm * 0.6) }));
      g.append(svg('text', { class: 'draft-label', x: n3(d.u), y: n3(d.v - Math.max(0.75, 15 * mpp)), 'font-size': n3(Math.max(0.45, 10 * mpp)) }, 'Draft Spot Clear (~1 m²)'));
      nodes.push(g);
    }
    host.replaceChildren(...nodes);
  }

  function renderGroundMark(areaId) {
    const c = state.cellDetail;
    const poly = state.mapMode === 'cell' && c && !c.missing && c.areaId === areaId ? c.polygon : null;
    const points = poly ? poly.map((p) => n3(p.u) + ',' + n3(p.v)).join(' ') : null;
    byId('ground-mark').replaceChildren(...(points ? [svg('polygon', { class: 'ground-pick-halo', points }),
      svg('polygon', { class: 'ground-pick', points })] : []));
  }

  const RAMP_SAMPLES = [0, 0.002, 0.005, 0.01, 0.02, 0.05, 0.1, 0.2, 0.35, 0.5, 0.65, 0.8, 0.9, 1];
  function renderOverlayLegend() {
    const box = byId('overlay-legend');
    if (state.overlay === 'off') { box.hidden = true; box.replaceChildren(); return; }
    box.hidden = false;
    if (state.overlay === 'light') {
      const ramp = svg('svg', { id: 'light-ramp', class: 'light-ramp', viewBox: '0 0 100 10', preserveAspectRatio: 'none', role: 'img',
        'aria-label': 'Light Guide colour ramp: shaded to bright' });
      const grad = svg('linearGradient', { id: 'light-ramp-fill', x1: 0, y1: 0, x2: 1, y2: 0 });
      for (const value of RAMP_SAMPLES) {
        const paint = lightPaint(value);
        grad.append(svg('stop', { offset: value, 'data-l': value, 'stop-color': paint ? paint.colour : '#7f3b4a',
          'stop-opacity': paint ? paint.opacity : 0 }));
      }
      const defs = svg('defs');
      defs.append(grad);
      ramp.append(defs, svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'rgb(' + SLATE.join(',') + ')' }),
        svg('rect', { x: 0, y: 0, width: 100, height: 10, fill: 'url(#light-ramp-fill)' }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Light Guide'),
        el('span', { class: 'ramp-end', 'aria-hidden': 'true' }, 'Shaded'),
        ramp,
        el('span', { class: 'ramp-end', 'aria-hidden': 'true' }, 'Bright'),
        el('p', { class: 'overlay-legend-note' }, 'Modelled light · not calibrated PAR'));
    } else if (state.overlay === 'undergrowth') {
      const swBramble = svg('svg', { class: 'overlay-swatch', viewBox: '0 0 4 2.5', 'aria-hidden': 'true' });
      swBramble.append(svg('rect', { width: 4, height: 2.5, fill: '#1b2430' }), svg('rect', { width: 4, height: 2.5, fill: 'url(#ccf-bramble-hatch)' }));
      const swBracken = svg('svg', { class: 'overlay-swatch', viewBox: '0 0 4 2.5', 'aria-hidden': 'true' });
      swBracken.append(svg('rect', { width: 4, height: 2.5, fill: '#1b2430' }), svg('rect', { width: 4, height: 2.5, fill: 'url(#ccf-bracken-hatch)' }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Undergrowth'),
        el('span', null, swBramble, ' Bramble'),
        el('span', null, swBracken, ' Bracken'),
        el('p', { class: 'overlay-legend-note' }, 'Hatch opacity scales with cell cover (0–1). ClearUndergrowth: Whole 2.5 m cell · spares seedlings.'));
    } else if (state.overlay === 'seedlings') {
      const swSeed = svg('svg', { class: 'overlay-swatch', viewBox: '-24 -24 48 48', 'aria-hidden': 'true' });
      swSeed.append(svg('use', { href: '#ccf-natural-seedlings', x: -24, y: -24, width: 48, height: 48 }));
      box.replaceChildren(
        el('p', { class: 'overlay-legend-title' }, 'Natural Seedlings'),
        el('span', null, swSeed, ' Cell presence (S Sitka · O Oak · B Beech)'),
        el('p', { class: 'overlay-legend-note' }, label('MultiNaturalSeedlingsNote') || 'Cell-level presence only — Density is abstract occupancy, not a literal stem count'));
    }
  }

  function renderScale(scale) {
    const metres = [0.5, 1, 2, 5, 10, 20, 50, 100].find((m) => m * scale >= 56) || 100;
    const width = Math.max(8, Math.round(metres * scale));
    const bar = byId('scale-bar');
    setAttributes(bar, { width, height: 9, viewBox: '0 0 ' + width + ' 9' });
    byId('scale-path').setAttribute('d', 'M 1 1 V 8 H ' + (width - 1) + ' V 1');
    byId('scale-text').textContent = metres + ' m';
  }

  function renderMapStatus(reference, drafts) {
    const box = byId('map-status');
    const view = state.view;
    let text = '';
    if (state.mapMode === 'cell') text = 'Inspect Cell mode: click any 2.5 m cell inside the area · Esc returns to Select Tree (draft Fell ticks kept)';
    else if (state.mapMode === 'plant') text = 'Plant mode (snap OFF): click map for provisional Draft preview · Esc returns to Select Tree (draft Fell ticks kept)';
    else if (state.mapMode === 'spot') text = 'Spot Clear mode (snap OFF): click map for seedling-free 0.564 m Draft preview · Esc returns to Select Tree (draft Fell ticks kept)';
    else if (state.batch) text = 'Adding ' + (state.batch.confirmed.length + 1) + ' of ' + state.batch.ids.length + ' to the Fell plan…';
    else if (state.queueRunning) text = 'Queueing planting Work Plan orders in sequence…';
    else if (view.approved) {
      const summary = workTotals(view.approved.orders.length, approvedPrunes().length, approvedClears().length, approvedPlantings());
      text = summary + ' approved · nothing executed yet';
    }
    else if (currentJuvenile()) text = 'Sapling ' + currentJuvenile().juvenileId + ' · ' + speciesLabel(currentJuvenile().speciesId);
    else if (reference) text = 'Reference ' + reference + ' · ' + plural(drafts, 'draft choice', 'draft choices');
    box.hidden = text === '';
    box.textContent = text;
  }

  // ---- status marks ---------------------------------------------------------------------------------------------------

  function markPath(cls, d) { return svg('path', { class: 'ring ' + cls, d }); }
  function at(item, dx, dy) { return n3(item.u + dx) + ' ' + n3(item.v + dy); }

  function cornerFrame(item, cls, mpp) {
    const h = item.radiusM + 7 * mpp, arm = 10 * mpp;
    let d = '';
    for (const [sx, sy] of [[-1, -1], [1, -1], [1, 1], [-1, 1]]) {
      d += 'M ' + at(item, sx * h, sy * (h - arm)) + ' L ' + at(item, sx * h, sy * h) + ' L ' + at(item, sx * (h - arm), sy * h) + ' ';
    }
    return markPath(cls, d);
  }

  function sideBrackets(item, cls, mpp) {
    const x = item.radiusM + 7 * mpp, y = 9 * mpp, arm = 5 * mpp;
    let d = '';
    for (const s of [-1, 1]) {
      d += 'M ' + at(item, s * (x - arm), -y) + ' L ' + at(item, s * x, -y) + ' L ' + at(item, s * x, y) + ' L ' + at(item, s * (x - arm), y) + ' ';
    }
    return markPath(cls, d);
  }

  function underline(item, cls, mpp, extraPx) {
    const y = item.radiusM * .8 + (extraPx || 0) * mpp, half = 10 * mpp;
    return markPath(cls, 'M ' + at(item, -half, y) + ' L ' + at(item, half, y));
  }

  const TILE = { rank: -45, status: 45, crop: 135, reference: -135 };
  const RANK_ANGLES = [-45, 45, -135, 135, 0, 180, -90, 90];

  function tileCentre(item, angleDeg, mpp) {
    const angle = angleDeg * Math.PI / 180, distance = item.radiusM + 9 * mpp;
    return [item.u + Math.cos(angle) * distance, item.v + Math.sin(angle) * distance];
  }

  function badge(item, angleDeg, text, kind, mpp, extra) {
    const [cx, cy] = tileCentre(item, angleDeg, mpp);
    const g = svg('g', { class: 'badge b-' + kind + (extra ? ' ' + extra : '') });
    g.append(svg('rect', { class: 'badge-bg', x: n3(cx - 10 * mpp), y: n3(cy - 10 * mpp), width: n3(20 * mpp), height: n3(20 * mpp), rx: n3(2 * mpp) }),
      svg('text', { class: 'badge-text', x: n3(cx), y: n3(cy + .5 * mpp), 'font-size': n3(10.5 * mpp) }, text));
    return g;
  }

  function tag(x, y, text, mpp) {
    const width = (text.length * 6.4 + 14) * mpp, height = 19 * mpp;
    const g = svg('g', { class: 'map-tag' });
    g.append(svg('rect', { x: n3(x - width / 2), y: n3(y - height / 2), width: n3(width), height: n3(height), rx: n3(4 * mpp) }),
      svg('text', { x: n3(x), y: n3(y + .5 * mpp), 'font-size': n3(11 * mpp) }, text));
    return g;
  }

  function tick(item, mpp, angle) {
    const [rx, ry] = tileCentre(item, angle ?? TILE.rank, mpp);
    const cx = rx + 11 * mpp, cy = ry - 11 * mpp;
    const p = (dx, dy) => n3(cx + dx * mpp) + ' ' + n3(cy + dy * mpp);
    return [svg('circle', { class: 'tick-box', cx: n3(cx), cy: n3(cy), r: n3(7 * mpp) }),
      svg('path', { class: 'tick', d: 'M ' + p(-3, 0) + ' L ' + p(-1, 2.5) + ' L ' + p(3.5, -3) })];
  }

  function statusMarks(item, mpp) {
    const marks = [];
    // Rule 5: Retained crop tree framed by double gold ring + small C badge (#ccf-crop-ring), never sawn lumber.
    if (item.isCrop) {
      const span = Math.max(item.radiusM * 1.22, item.radiusM + 5 * mpp);
      marks.push(svg('use', { class: 'deco crop-ring', href: '#ccf-crop-ring',
        x: n3(item.u - span), y: n3(item.v - span), width: n3(2 * span), height: n3(2 * span) }));
    }
    // Rule 3: Pending = dashed gold outline + clock/P; Approved = solid pale-blue badge + check/A.
    if (item.status === 'PendingFell') {
      marks.push(underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '◷P', 'pending', mpp));
    } else if (item.status === 'ApprovedFell') {
      marks.push(underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8),
        badge(item, TILE.status, '✓A', 'approved', mpp));
    } else if (item.status === 'NaturalDead') {
      marks.push(badge(item, TILE.status, 'D', 'dead', mpp));
    } else if (item.status === 'Felled') {
      const d = Math.min(item.radiusM * .45, 6 * mpp);
      marks.push(svg('path', { class: 'stump-x', d: 'M ' + n3(item.u - d) + ' ' + n3(item.v - d) + ' L ' + n3(item.u + d) + ' ' +
        n3(item.v + d) + ' M ' + n3(item.u - d) + ' ' + n3(item.v + d) + ' L ' + n3(item.u + d) + ' ' + n3(item.v - d) }),
        badge(item, TILE.status, 'X', 'executed', mpp));
    }
    return marks;
  }

  function competitorMarks(item, rank, chosen, mpp, angle) {
    const marks = [underline(item, chosen ? 'ring-draft' : 'ring-competitor', mpp, 0)];
    marks.push(badge(item, angle ?? TILE.rank, String(rank), 'rank', mpp, chosen ? 'chosen' : null));
    if (chosen) marks.push(...tick(item, mpp, angle));
    return marks;
  }

  function renderRankLayer(ranks, box) {
    const layer = byId('rank-layer');
    const active = document.activeElement;
    const focusKey = active && layer.contains(active) ? active.getAttribute('data-focus-key') : null;
    const planLocked = state.view.approved !== null;
    const frozenNow = state.lock !== null || state.batch !== null;
    const buttons = ranks.map((r) => {
      const why = blockedReason(state.areaId, r.treeId) ||
        (planLocked ? 'The plan is approved, so editing is locked' : frozenNow ? 'Please wait' : null);
      const button = el('button', { type: 'button', class: 'rank-toggle', 'data-rank-toggle': r.treeId, 'data-rank': r.rank,
        'data-focus-key': 'rank:' + r.treeId, 'aria-pressed': String(r.chosen && !why), 'aria-disabled': why ? 'true' : null,
        'aria-label': 'Competitor ' + r.rank + ', tree ' + r.treeId + ', Fell draft' + (why ? ' (' + why + ')' : ''),
        title: 'Competitor ' + r.rank + ' · ' + r.treeId + (why ? ' · ' + why : r.chosen ? ' · in the Fell draft (click to remove)' : ' · click to add to the Fell draft') },
      String(r.rank));
      if (button.style) {
        button.style.left = n3((r.x - box.x) * box.scale) + 'px';
        button.style.top = n3((r.y - box.y) * box.scale) + 'px';
      }
      return button;
    });
    layer.replaceChildren(...buttons);
    layer.hidden = buttons.length === 0;
    if (focusKey) {
      const again = layer.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
      if (again) again.focus();
    }
  }

  function onRankToggle(event) {
    const button = event.target.closest('[data-rank-toggle]');
    if (!button) return;
    event.stopPropagation();
    const treeId = button.getAttribute('data-rank-toggle');
    if (button.getAttribute('aria-disabled') === 'true') {
      announce('Competitor ' + button.getAttribute('data-rank') + ', ' + treeId + ', cannot be chosen: ' +
        (blockedReason(state.areaId, treeId) || (state.view.approved ? 'the plan is approved, so editing is locked' : 'please wait')) + '.');
      return;
    }
    toggleDraft(treeId, button.getAttribute('aria-pressed') !== 'true');
  }

  function referenceMarks(item, mpp, withTag) {
    const marks = [cornerFrame(item, 'ring-halo', mpp), cornerFrame(item, 'ring-reference', mpp),
      badge(item, TILE.reference, 'R', 'reference', mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Reference' + (item.isCrop ? ' · Crop' : ''), mpp));
    return marks;
  }

  function inspectMarks(item, mpp, withTag) {
    const marks = [sideBrackets(item, 'ring-halo', mpp), sideBrackets(item, 'ring-inspect', mpp),
      badge(item, TILE.reference, 'I', 'inspect', mpp)];
    if (withTag) marks.push(tag(item.u, item.v - item.radiusM - 22 * mpp, 'Selected', mpp));
    return marks;
  }

  function pruneMarks(areaId, item, mpp) {
    const ps = pruneStatusOf(areaId, item.treeId);
    if (!ps || !ps.planState) return [];
    return ps.planState === 'PendingPrune'
      ? [underline(item, 'ring-pending', mpp, 4), badge(item, TILE.status, '◷✂', 'pending', mpp)]
      : [underline(item, 'ring-approved', mpp, 4), underline(item, 'ring-approved', mpp, 8), badge(item, TILE.status, '✓✂', 'approved', mpp)];
  }

  // ---- Interactive modes: Inspect Cell, Plant, Spot Clear, and PJ Sapling inspection ----------------------------------

  function setMapMode(mode, moveFocus) {
    if (!['tree', 'cell', 'plant', 'spot'].includes(mode)) return;
    if (state.mapMode === mode) return;
    state.mapMode = mode;
    state.cellPick += 1;
    state.cellLoading = false;
    if (mode !== 'plant') state.plantDraft = null;
    if (mode !== 'spot') state.spotDraft = null;
    showPane('tree');
    byId('pane-tree').scrollTop = 0;
    renderAll();
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

  function setOverlay(value) {
    if (!['off', 'light', 'undergrowth', 'seedlings'].includes(value)) return;
    state.overlay = value;
    renderAll();
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

  async function inspectCellAtPoint(areaId, u, v) {
    const s4 = s4Area(areaId);
    const cell = s4 ? s4.ground.find((c) => insidePolygon(c.polygon, u, v)) || null : null;
    if (!cell) {
      state.cellDetail = { areaId, missing: true };
      state.cellLoading = false;
      showPane('tree');
      renderAll();
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

  function renderCellInspector() {
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

  function renderPlantDrawer() {
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

  function renderSpotDrawer() {
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

  function renderJuvenileInspector() {
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

  // ---- grid and legend ------------------------------------------------------------------------------------------------

  function drawGrid(area, mpp, scale) {
    const side = area.extent.sideM;
    const minX = area.extent.minXM;
    const minZ = area.extent.minZM;
    const step = 5 * scale >= 26 ? 5 : 10;
    const reach = 80;
    const lines = [];
    const labels = [];
    const font = n3(10 * mpp);
    for (let x = Math.ceil((minX - reach) / step) * step; x <= minX + side + reach; x += step) {
      const u = x - minX;
      lines.push(svg('line', { class: 'grid-line', x1: n3(u), y1: -reach, x2: n3(u), y2: side + reach }));
      if (x >= minX - 1e-9 && x <= minX + side + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(u), y: n3(side + 14 * mpp), 'text-anchor': 'middle', 'font-size': font }, x));
      }
    }
    for (let z = Math.ceil((minZ - reach) / step) * step; z <= minZ + side + reach; z += step) {
      const v = minZ + side - z;
      lines.push(svg('line', { class: 'grid-line', x1: -reach, y1: n3(v), x2: side + reach, y2: n3(v) }));
      if (z >= minZ - 1e-9 && z <= minZ + side + 1e-9) {
        labels.push(svg('text', { class: 'grid-label', x: n3(-7 * mpp), y: n3(v + 3.5 * mpp), 'text-anchor': 'end', 'font-size': font }, z));
      }
    }
    labels.push(svg('text', { class: 'grid-label', x: n3(side / 2), y: n3(side + 30 * mpp), 'text-anchor': 'middle', 'font-size': font },
      'X metres east · Z metres north'));
    byId('grid').replaceChildren(...lines);
    byId('grid-in').replaceChildren(...lines.map((line) => line.cloneNode(false)));
    byId('furniture').replaceChildren(...labels);
  }

  function legendIcon(draw) {
    const icon = svg('svg', { viewBox: '-1.9 -1.9 3.8 3.8', 'aria-hidden': 'true', focusable: 'false' });
    const mpp = .075;
    const item = { u: 0, v: 0, radiusM: 1, treeId: 'legend', status: 'Living', isCrop: false };
    icon.append(svg('circle', { class: 'swatch-ground', cx: 0, cy: 0, r: 1.85 }));
    icon.append(...draw(item, mpp));
    return icon;
  }

  function legendUse(symbolId, span) {
    const s = span || 1.35;
    return [svg('use', { href: '#' + symbolId, x: -s, y: -s, width: 2 * s, height: 2 * s })];
  }

  function renderLegend() {
    const rows = [
      ['Sitka spruce', 'Radial conifer crown', () => legendUse('ccf-sitka', 1.25)],
      ['Sessile oak', 'Lobed broadleaf crown', () => legendUse('ccf-oak', 1.25)],
      ['European beech', 'Dense rounded broadleaf', () => legendUse('ccf-beech', 1.25)],
      ['Retained Crop · C', 'Double gold ring + C badge', (item, mpp) => [...legendUse('ccf-sitka', 1.05),
        ...statusMarks(Object.assign({}, item, { isCrop: true }), mpp)]],
      ['Reference · R / Comp. 1–5', 'Pinned reference & top 5', (item, mpp) => [...legendUse('ccf-sitka', 1.0), ...referenceMarks(item, mpp, false)]],
      ['Pending · ◷', 'Dashed gold outline; queued', (item, mpp) => [...legendUse('ccf-sitka', 1.0),
        ...statusMarks(Object.assign({}, item, { status: 'PendingFell' }), mpp)]],
      ['Approved · ✓', 'Solid pale-blue; awaiting Execute', (item, mpp) => [...legendUse('ccf-sitka', 1.0),
        ...statusMarks(Object.assign({}, item, { status: 'ApprovedFell' }), mpp)]],
      ['Felled stump · X', 'Executed work (History Log)', (item, mpp) => [...crownDecoration(Object.assign({}, item, { status: 'Felled' }), 'sitka-spruce'),
        ...statusMarks(Object.assign({}, item, { status: 'Felled' }), mpp)]],
      ['Standing dead · D', 'Natural mortality crown', (item, mpp) => [...legendUse('ccf-standing-dead', 1.15),
        ...statusMarks(Object.assign({}, item, { status: 'NaturalDead' }), mpp)]],
      ['Fallen log (schematic)', 'Direction schematic, not simulated', () => [svg('use', { href: '#ccf-log', x: -1.45, y: -0.55, width: 2.9, height: 1.1 })]],
      ['Active shelter (< 8 yr)', 'Cyan mesh tube protecting PJ', () => legendUse('ccf-shelter-active', 1.35)],
      ['Expired shelter (8+ yr)', 'Dashed grey + X · map state only', () => legendUse('ccf-shelter-expired', 1.35)],
      ['SpotClear (~1 m²)', 'Cleared soil + twigs, no seedling', () => legendUse('ccf-spot-clear', 1.25)],
      ['ClearUndergrowth', 'Whole 2.5 m cell · spares seedlings', () => legendUse('ccf-action-clear', 1.35)],
      ['Natural seedlings', 'Cell-level presence glyph only', () => legendUse('ccf-natural-seedlings', 1.35)],
    ];
    const items = rows.map(([name, description, draw]) => el('li', null, legendIcon(draw),
      el('span', null, el('span', { class: 'legend-name' }, name), el('span', { class: 'legend-desc' }, description))));
    const boundary = svg('svg', { viewBox: '-1.9 -1.9 3.8 3.8', 'aria-hidden': 'true', focusable: 'false' });
    boundary.append(svg('rect', { class: 'legend-exterior', x: -1.85, y: -1.85, width: 3.7, height: 3.7, rx: .3 }),
      svg('polygon', { class: 'legend-area', points: '-1.3,-1 1.2,-1.35 1.35,1.2 -1.2,1.3' }));
    items.push(el('li', null, boundary, el('span', null, el('span', { class: 'legend-name' }, 'Area boundary'),
      el('span', { class: 'legend-desc' }, 'Actual management extent'))));
    byId('legend').replaceChildren(...items);
  }

  function setLegendHidden(hidden, moveFocus) {
    state.legendHidden = hidden;
    byId('workspace').setAttribute('data-legend', hidden ? 'hidden' : 'shown');
    byId('legend-show').hidden = !hidden;
    byId('legend-show').setAttribute('aria-expanded', String(!hidden));
    byId('legend-hide').setAttribute('aria-expanded', String(!hidden));
    if (moveFocus) byId(hidden ? 'legend-show' : 'legend-hide').focus();
    announce(hidden ? 'Legend hidden. Show legend is at the top right of the map.' : 'Legend shown.');
  }

  // ---- Tree inspector, Prune box (species-aware) and Competition ------------------------------------------------------

  function renderOverlap() {
    const box = byId('overlap');
    const overlap = state.overlap;
    if (!overlap || overlap.areaId !== state.areaId || state.mapMode !== 'tree') {
      box.hidden = true;
      byId('overlap-list').replaceChildren();
      return;
    }
    box.hidden = false;
    byId('overlap-heading').textContent = fill(label('CrownsOverlap'), { n: overlap.candidates.length });
    byId('overlap-list').replaceChildren(...overlap.candidates.map((candidate, index) => {
      const current = candidate.treeId === selectedId();
      return el('li', null, el('button', { type: 'button', class: 'small', 'data-pick': candidate.treeId,
        'data-focus-key': 'pick:' + candidate.treeId, 'aria-label': (current ? 'Selected ' : 'Select ') + candidate.treeId },
        current ? 'Selected' : 'Select'), ' ', el('span', { class: 'mono' }, candidate.treeId),
        ' (' + speciesLabel(speciesOf(candidate.treeId)) + ') — ' + statusText(candidate) + (index === 0 ? ' (topmost)' : ''));
    }));
  }

  function recordedReason(areaId, treeId) {
    const view = state.view;
    const pending = view.pending.find((order) => order.areaId === areaId && order.treeId === treeId);
    if (pending) return pending.reason + ' (pending order)';
    const approved = view.approved ? view.approved.orders.find((order) => order.areaId === areaId && order.treeId === treeId) : null;
    if (approved) return approved.reason + ' (approved order)';
    for (const event of view.events) {
      for (const change of event.areas) {
        const fell = change.fells.find((record) => record.treeId === treeId);
        if (fell) return fell.reason + ' (executed Fell, work revision ' + fell.workRevision + ')';
      }
    }
    return null;
  }

  function keyFact(term, value, unit) {
    return el('div', null, el('dt', null, term), el('dd', null, value, ' ', el('small', null, unit)));
  }

  function renderInspector() {
    const areaId = state.areaId;
    const id = selectedId();
    const tree = treeIn(areaId, id);
    const item = tree ? itemOf(areaId, id) : null;
    byId('inspector-empty').hidden = Boolean(tree && item);
    byId('inspector-body').hidden = !(tree && item);
    if (!tree || !item) return;
    const sp = speciesOf(tree.treeId);
    const spName = speciesLabel(sp);
    const isReference = referenceIn(areaId) === id;
    byId('insp-role').textContent = (isReference ? 'Reference tree' : 'Selected tree') + ' · ' + spName;
    byId('insp-area').textContent = areaName(areaId);
    byId('insp-id').textContent = tree.treeId;
    const chip = byId('insp-chip');
    chip.textContent = item.isCrop ? 'C · ' + label('CropRetained') : baseStatusText(item);
    chip.setAttribute('class', 'chip s-' + item.status + (item.isCrop ? ' crop' : ''));
    chip.setAttribute('title', chip.textContent);
    chip.hidden = !item.isCrop && item.status === 'Living';
    byId('insp-summary').textContent = spName + ' · ' + baseStatusText(item);
    byId('insp-key').replaceChildren(
      keyFact('DBH', fixed(tree.dbhCm, 1), 'cm'),
      keyFact('Height', fixed(tree.heightM, 1), 'm'),
      keyFact('Crown r', fixed(tree.crownRadiusM, 2), 'm'));
    byId('inspect-close').setAttribute('aria-label', 'Close inspection of ' + tree.treeId + ' (Escape)');
    byId('insp-pinned').textContent = isReference ? 'Reference stays pinned while choosing' : '';
    const facts = [
      pair('Species', spName + ' (' + sp + ')'),
      pair('DBH', fixed(tree.dbhCm, 2) + ' cm'),
      pair('Height', fixed(tree.heightM, 2) + ' m'),
      pair('Crown radius', fixed(tree.crownRadiusM, 3) + ' m'),
      pair('Age', tree.ageYears + ' years'),
      pair('Position', 'X ' + fixed(tree.xm, 2) + ' m east, Z ' + fixed(tree.zm, 2) + ' m north', 'wide'),
    ];
    if (tree.death) {
      facts.push(pair('Death year', tree.death.ecologicalYear));
      facts.push(pair('Cause', tree.death.cause + ' (' + tree.death.mechanism + ')'));
    }
    const s4 = s4Tree(areaId, tree.treeId);
    const ps = pruneStatusOf(areaId, tree.treeId);
    const dw = s4Area(areaId) ? s4Area(areaId).canopy.fallenDeadwood.find((d) => d.treeId === tree.treeId) : null;
    if (s4) {
      facts.push(pair('Life state', s4.lifeState + (s4.deathCause ? ' · ' + s4.deathCause + ', year ' + s4.deathYear : '')));
    } else if (tree.death && state.stage4) {
      const lifeState = tree.death.lifeState || (item.status === 'Felled' ? 'FelledStump' : 'BiologicallyDead');
      facts.push(pair('Life state', lifeState === 'BiologicallyDead'
        ? 'BiologicallyDead · ' + tree.death.cause + ', year ' + tree.death.ecologicalYear
        : lifeState));
    }
    if (tree.isLiving && ps) {
      facts.push(pair('Pruning lifts', ps.lifts + ' / ' + ps.maxLifts + ' (' + ps.minIntervalYears + '-yr wait)' +
        (s4 && s4.pruningLifts > 0 ? ' · crown base ' + fixed(s4.crownBaseHeightM, 1) + ' m · last lift year ' + s4.lastPruningYear : ''), 'wide'));
    }
    if (dw) {
      facts.push(pair('Fallen deadwood', dw.deadwoodId + ' · ' + spName + ' · vol ' + fixed(dw.estimatedVolumeM3, 3) + ' m³ · direction schematic, not simulated', 'wide'));
    }
    const reason = recordedReason(areaId, tree.treeId);
    if (reason !== null) facts.push(pair('Recorded reason', reason, 'wide'));
    byId('insp-facts').replaceChildren(...facts);
    const crop = byId('crop-button');
    crop.textContent = label(item.isCrop ? 'MultiUnmarkCrop' : 'MultiMarkCrop');
    crop.hidden = !tree.isLiving;
    byId('crop-note').textContent = item.isCrop ? label('MultiCropFellRefused') + '.' :
      item.status === 'PendingFell' || item.status === 'ApprovedFell' ? 'Remove the pending Fell or cancel its approved plan before designating Crop.' :
        tree.isLiving ? label('MultiCropSeparate') + '.' : 'Dead trees cannot retain a Crop designation.';
    const addSelf = item.status === 'Living' && !item.isCrop;
    byId('add-reference-wrap').hidden = !addSelf;
    byId('add-reference').textContent = 'Fell this tree instead';
    byId('add-reference').setAttribute('aria-label', 'Add ' + tree.treeId + ' itself to the Fell plan');
    byId('insp-remove-wrap').hidden = item.status !== 'PendingFell';
    byId('insp-remove').textContent = label('MultiRemoveFromPlan');
    byId('insp-approved-note').hidden = item.status !== 'ApprovedFell';
    byId('insp-threshold').textContent = label('MultiThresholdInformational');
    byId('insp-threshold').hidden = !tree.isLiving;
    renderPruneBox(areaId, tree, item, ps);
  }

  function renderPruneBox(areaId, tree, item, ps) {
    const box = byId('prune-box');
    box.hidden = !tree.isLiving || !ps;
    if (box.hidden) return;
    byId('prune-lifts').textContent = speciesLabel(ps.speciesId) + ' · ' + ps.lifts + ' / ' + ps.maxLifts + ' lifts (' + ps.minIntervalYears + '-yr wait)';
    let status;
    if (ps.planState === 'PendingPrune') status = 'Pending Prune — no crown is cut until the approved plan is executed.';
    else if (ps.planState === 'ApprovedPrune') status = 'Approved Prune — the crown is cut only when the plan is executed.';
    else if (ps.eligible) status = 'Eligible now: next lift raises the crown base to ' + fixed(ps.nextTargetM, 1) + ' m.';
    else status = 'Not eligible: ' + ps.refusalReason + (ps.nextEligibleYear !== undefined && ps.nextEligibleYear !== null
      ? ' (next lift possible in year ' + ps.nextEligibleYear + ')' : '') + '.';
    byId('prune-status').textContent = status;
    const add = byId('prune-add');
    add.textContent = label('MultiAddToPrunePlan');
    add.hidden = Boolean(ps.planState);
    const remove = byId('prune-remove');
    remove.hidden = ps.planState !== 'PendingPrune';
    remove.textContent = 'Remove Prune from Work Plan';
    remove.setAttribute('aria-label', 'Remove the pending Prune of ' + tree.treeId);
    byId('prune-why').textContent = pruneAvailability(areaId, item, ps).why || label('MultiPrunePendingNote') + '.';
  }

  function pruneAvailability(areaId, item, ps) {
    if (!ps || !item) return { ok: false, why: 'Select a living tree.' };
    if (!ps.eligible) return { ok: false, why: 'Not eligible (engine): ' + ps.refusalReason + '.' };
    if (ps.planState) return { ok: false, why: 'Already in the Work Plan as a Prune.' };
    if (item.status === 'PendingFell' || item.status === 'ApprovedFell') return { ok: false, why: 'This tree has a Fell order; one order per tree.' };
    if (state.view.approved) return { ok: false, why: 'The plan is approved, so editing is locked. Cancel the approved plan to change it.' };
    return { ok: true, why: null };
  }

  function renderPruneGate() {
    const areaId = state.areaId;
    const id = selectedId();
    const ps = id ? pruneStatusOf(areaId, id) : null;
    byId('prune-add').disabled = !pruneAvailability(areaId, id ? itemOf(areaId, id) : null, ps).ok;
  }

  // ---- Competition and Fell draft -------------------------------------------------------------------------------------

  function competitionTag() { return JSON.stringify([state.view ? currentFence() : null, state.competitionTarget]); }
  function clearCompetition() {
    state.competitionTarget = null;
    state.competition = null;
    state.competitionTag = null;
    state.competitionRequest += 1;
  }

  async function refreshCompetition() {
    const target = state.competitionTarget;
    if (!state.view || !target || state.lock) return;
    const tag = competitionTag();
    const request = ++state.competitionRequest;
    state.competition = null;
    renderAll();
    const outcome = await call('api/multi/competition', { fence: currentFence(), areaId: target.areaId, treeId: target.treeId });
    if (request !== state.competitionRequest || tag !== competitionTag() || state.lock || state.busy) return;
    if (outcome.kind === 'ok') {
      state.competition = { areaId: outcome.payload.areaId, report: outcome.payload.competition };
      state.competitionTag = tag;
      const dropped = reconcileDrafts();
      renderAll();
      if (dropped) announce(dropped);
    } else {
      clearCompetition();
      if (outcome.kind === 'refused') applyPayload(outcome.payload, false);
      renderAll();
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

  function renderCompetition() {
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

  function reasonToSend(forReferenceItself) {
    const note = byId('reason').value.trim();
    return note || (!forReferenceItself && byId('reason-release').checked ? releaseReason() : '') || DEFAULT_REASON;
  }

  function renderReasonPicker() {
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

  function resetReasonPicker() {
    byId('reason').value = '';
    byId('reason-release').checked = false;
    byId('reason-details').open = false;
    showReasonError(null);
  }

  function reconcileDrafts() {
    const notes = [];
    for (const areaId of Object.keys(state.drafts)) {
      const draft = state.drafts[areaId];
      if (!draft) continue;
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

  function toggleDraft(treeId, on) {
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
    renderAll();
    announce((on ? 'Added ' : 'Removed ') + treeId + (on ? ' to' : ' from') + ' the Fell draft (' + draft.ids.length + ' selected; not yet queued).');
  }

  function clearDraft() {
    const areaId = state.areaId;
    const ref = referenceIn(areaId);
    if (!ref || state.batch) return;
    const count = draftOf(areaId).length;
    if (count === 0) return;
    state.drafts[areaId] = { ref, ids: [] };
    state.draftNotice = null;
    renderAll();
    announce('Cleared ' + plural(count, 'draft choice', 'draft choices') + ' for ' + ref + '.');
  }

  async function addSequence(areaId, ref, ids, reason, kind) {
    const result = { areaId, ref, kind, ids: ids.slice(), reason, confirmed: [], refused: null, guard: null, unconfirmed: null, unattempted: [] };
    state.batch = result;
    state.batchResult = null;
    state.batchResultOpen = false;
    state.busy = true;
    document.documentElement.classList.add('busy');
    renderAll();
    let firstNotSent = ids.length;
    for (let index = 0; index < ids.length; index++) {
      const treeId = ids[index];
      const outcome = await call('api/multi/pending/add', { fence: currentFence(), areaId, treeId, reason });
      if (outcome.kind === 'ok') {
        result.confirmed.push(treeId);
        if (state.drafts[areaId] && state.drafts[areaId].ref === ref) {
          state.drafts[areaId].ids = state.drafts[areaId].ids.filter((id) => id !== treeId);
        }
        applyPayload(outcome.payload, false);
        continue;
      }
      firstNotSent = index + 1;
      if (outcome.kind === 'refused') {
        result.refused = { treeId, code: outcome.payload.refusal.code, message: outcome.payload.refusal.message };
        applyPayload(outcome.payload, false);
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
      renderAll();
    }
    announce(batchSummary(result));
    if (!result.unconfirmed) {
      const add = byId('add-button');
      const rendered = typeof add.getClientRects !== 'function' || add.getClientRects().length > 0;
      if (add.disabled || add.closest('[hidden]') || !rendered) byId('batch-result').focus();
      else add.focus();
    }
    if (!state.lock && state.competitionTarget && state.competitionTag !== competitionTag()) refreshCompetition();
    if (!state.lock && !result.unconfirmed) refreshReviewPreview();
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

  function renderBatchResult() {
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

  function onBatchResultClick(event) {
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

  // ---- Work Plan, Receipt, Results, Trees, Details --------------------------------------------------------------------

  function statePill(approved, actionIconId, actionWord) {
    return el('span', { class: 'wp-pill ' + (approved ? 'wp-approved' : 'wp-pending') },
      useIcon(actionIconId, 15),
      approved ? ('✓ Approved ' + actionWord) : ('◷ Pending ' + actionWord));
  }

  function orderList(orders, approved, removable) {
    return el('ul', { class: 'list orders ' + (approved ? 'approved-orders' : 'pending-orders') }, ...orders.map((order) => el('li', null,
      el('span', { class: 'order-head' }, statePill(approved, 'ccf-action-fell', 'Fell'),
        el('span', { class: 'mono' }, order.treeId)),
      el('span', { class: 'order-reason' }, order.reason),
      removable ? el('button', { type: 'button', class: 'small', 'data-remove': order.treeId, 'data-area': order.areaId,
        'data-focus-key': 'remove:' + order.treeId, 'aria-label': label('MultiRemoveFromPlan') + ': ' + order.treeId },
      label('MultiRemoveFromPlan')) : null)));
  }

  function pruneList(orders, approved, removable) {
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
  function ecoOrderList(clears, plantings, approved, removable) {
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

  function workTotals(fells, prunes, clears, plantings) {
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

  function renderPlan() {
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

  function renderReceipt() {
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

  function renderHistory() {
    renderSkipSummary();
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

  function renderTable() {
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

  function renderDetails() {
    const view = state.view;
    const e = view.experiment;
    const s = view.source;
    const readout = readoutState();
    const statements = (readout === 'phase2' || readout === 'stage4')
      ? STATEMENTS.map((name) => (name === 'MultiEdgeExcluded' ? 'MultiConnectedWoodland' : name))
      : readout === 'legacy' ? STATEMENTS : STATEMENTS.filter((name) => name !== 'MultiEdgeExcluded');
    byId('statements').replaceChildren(
      el('li', null, experimentBadgeText()),
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
      pair('Experiment', e.experimentId), pair('Badge', experimentBadgeText(), 'wide'),
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

  function renderPaneButtons() {
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

  function showPane(name) {
    if (!PANES.includes(name)) return;
    byId('side').setAttribute('data-pane', name);
    for (const button of document.querySelectorAll('[data-pane-button]')) {
      button.setAttribute('aria-pressed', String(button.getAttribute('data-pane-button') === name));
    }
    if (name === 'details' && state.view && !state.structure && !state.structureLoading && !state.lock) {
      refreshStructure();
    }
  }

  function renderMapControls() {
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

  function zoomMap(step) {
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
    renderMap();
    renderMapControls();
  }

  function panMap(east, south) {
    const area = currentArea();
    if (!area || state.zoom === ZOOMS[0]) return;
    const box = viewBox(area);
    const centre = usableCentre(box);
    state.centre = { u: centre.u + east * box.uw / 4, v: centre.v + south * box.uh / 4 };
    state.centre = usableCentre(viewBox(area));
    renderMap();
    renderMapControls();
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
    const area = currentArea();
    const matrix = byId('map').getScreenCTM();
    if (!area || !matrix) return;
    const point = new DOMPoint(event.clientX, event.clientY).matrixTransform(matrix.inverse());

    // Inspect Cell mode: click queries POST api/multi/cell cleanly without altering selection or Fell draft.
    if (state.mapMode === 'cell') {
      await inspectCellAtPoint(area.areaId, point.x, point.y);
      return;
    }

    // Plant mode: click places a provisional local Draft preview only (dashed slate, snap OFF, zero engine order).
    if (state.mapMode === 'plant') {
      const world = worldFromMap(area, point.x, point.y);
      state.plantDraft = { areaId: area.areaId, u: point.x, v: point.y, xM: world.xM, zM: world.zM };
      state.plantResult = null;
      showPane('tree');
      renderAll();
      byId('plant-heading').focus();
      announce('Provisional Draft planting point at X ' + fixed(world.xM, 2) + ' m, Z ' + fixed(world.zM, 2) + ' m (snap OFF; zero order queued).');
      return;
    }

    // Spot Clear mode: click places a provisional seedling-free 0.564 m circular preview at exact (X, Z) (snap OFF).
    if (state.mapMode === 'spot') {
      const world = worldFromMap(area, point.x, point.y);
      state.spotDraft = { areaId: area.areaId, u: point.x, v: point.y, xM: world.xM, zM: world.zM };
      state.spotResult = null;
      showPane('tree');
      renderAll();
      byId('spot-heading').focus();
      announce('Provisional Draft Spot Clear (~1 m², 0.564 m ring) at X ' + fixed(world.xM, 2) + ' m, Z ' + fixed(world.zM, 2) + ' m (snap OFF; zero order queued).');
      return;
    }

    // Select Tree mode (default): clicking a rendered PJ sapling inspects it (preserving adult Fell drafts); otherwise crown clicks win via POST api/multi/hit.
    const pjEl = event.target && event.target.closest ? event.target.closest('[data-juvenile-id]') : null;
    if (pjEl) {
      chooseJuvenile(area.areaId, pjEl.getAttribute('data-juvenile-id'));
      return;
    }

    const pick = ++state.mapPick;
    state.busy = true;
    const outcome = await call('api/multi/hit', { fence: currentFence(), areaId: area.areaId, u: point.x, v: point.y });
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

  // ---- Work Plan commands (Fell, Prune, ClearUndergrowth, Plant, Shelter, SpotClear, Crop, Approve, Cancel, Execute, Year +1) ----

  function onCellClearAdd() {
    const c = state.cellDetail;
    if (!c || c.missing || !state.view || state.batch || state.queueRunning) return;
    run('api/multi/clear/add', { fence: currentFence(), areaId: c.areaId, column: c.column, row: c.row },
      'Queue ClearUndergrowth for cell (' + c.column + ', ' + c.row + ')',
      () => announce('Done: cell (' + c.column + ', ' + c.row + ') queued as pending ClearUndergrowth (spares seedlings; nothing cleared yet).'),
      'cell-clear-remove');
  }

  function removeClear(areaId, column, row) {
    if (!areaId || state.batch || state.queueRunning) return;
    run('api/multi/clear/remove', { fence: currentFence(), areaId, column: Number(column), row: Number(row) },
      'Remove pending ClearUndergrowth for cell (' + column + ', ' + row + ')',
      () => announce('Done: pending ClearUndergrowth for cell (' + column + ', ' + row + ') removed.'),
      state.mapMode === 'cell' ? 'cell-clear-add' : 'plan-heading');
  }

  function removePlanting(orderId) {
    if (!orderId || state.batch || state.queueRunning) return;
    run('api/multi/planting/remove', { fence: currentFence(), orderId },
      'Remove pending order ' + orderId,
      () => announce('Done: pending order ' + orderId + ' removed from Work Plan.'),
      state.mapMode === 'plant' ? 'plant-heading' : state.mapMode === 'spot' ? 'spot-heading' :
        currentJuvenile() ? 'pj-heading' : 'plan-heading');
  }

  // Sequential fence-chained planting queue: (1) plant/add, (2) if ticked shelter/add, (3) if ticked spot-clear/add.
  // Stops immediately on step 2/3 409, surfaces exact Core refusal, and discloses earlier pending orders with one-click remove.
  async function onPlantQueue() {
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
    renderAll();
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
      refreshReviewPreview();
    }
  }

  async function onSpotQueue() {
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
      renderAll();
      byId('pane-tree').scrollTop = 0;
      announce('Queued pending SpotClear ' + (added ? added.orderId : '') + ' (not executed).');
      refreshReviewPreview();
    } else if (outcome.kind === 'refused') {
      state.spotResult = { areaId, order: null, refused: outcome.payload.refusal };
      applyPayload(outcome.payload, false);
      byId('pane-tree').scrollTop = 0;
      showRefusal('Queue Spot Clear', outcome.payload.refusal);
      announce('Refused SpotClear (' + outcome.payload.refusal.code + '). Nothing changed.');
      refreshReviewPreview();
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Queue Spot Clear', outcome.status, outcome.error);
    } else if (outcome.kind === 'locked') {
      showLockAlert('Not sent: Queue Spot Clear.');
    } else {
      lockCommands(label('MultiOutcomeUnconfirmed') || 'Outcome unconfirmed — re-read current state',
        'Queue Spot Clear: no confirmed result was received. Do not assume that it happened, or that it did not.');
    }
  }

  function onPjShelterAdd() {
    const j = currentJuvenile();
    if (!j || !state.view || state.batch || state.queueRunning) return;
    run('api/multi/shelter/add', { fence: currentFence(), areaId: j.areaId, xM: j.xm, zM: j.zm },
      'Queue 8-year Shelter for ' + j.juvenileId,
      () => announce('Done: 8-year shelter queued as pending for ' + j.juvenileId + ' (not installed until Execute).'),
      'pj-shelter-remove');
  }

  function onCrop() {
    const areaId = state.areaId;
    const treeId = selectedId();
    if (!treeId || !state.view || state.batch) return;
    const marked = areaView(areaId).cropTreeIds.includes(treeId);
    run(marked ? 'api/multi/crop/unmark' : 'api/multi/crop/mark', { fence: currentFence(), areaId, treeId },
      label(marked ? 'MultiUnmarkCrop' : 'MultiMarkCrop') + ' (' + treeId + ')',
      () => announce((marked ? 'Removed Crop designation: ' : 'Retained Crop Tree: ') + treeId + ' in ' + areaName(areaId) + '.'));
  }

  function onAdd(event) {
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

  function onAddReference() {
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

  function removePending(areaId, treeId) {
    if (!areaId || !treeId || state.batch) return;
    run('api/multi/pending/remove', { fence: currentFence(), areaId, treeId }, 'Remove ' + treeId + ' from the Work Plan',
      () => announce('Done: ' + label('MultiRemoveFromPlan') + ' (' + treeId + ').'), 'plan-heading');
  }

  function onPruneAdd() {
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

  function removePrune(areaId, treeId) {
    if (!areaId || !treeId || state.batch) return;
    run('api/multi/prune/remove', { fence: currentFence(), areaId, treeId }, 'Remove the pending Prune of ' + treeId,
      () => announce('Done: pending Prune of ' + treeId + ' removed. Nothing was cut.'), 'plan-heading');
  }

  function onApprove() {
    run('api/multi/plan/approve', { fence: currentFence() }, label('MultiApprove'), (payload) => {
      const prunes = payload.stage4 ? payload.stage4.approvedPrunes.map((o) => o.treeId) : [];
      const clears = payload.ecology ? (payload.ecology.approvedClear || []).map((c) => 'Cell(' + c.column + ',' + c.row + ')') : [];
      const plants = payload.ecology ? (payload.ecology.approvedPlanting || []).map((p) => p.orderId) : [];
      announce('Done: ' + label('MultiApprove') + '. Approved: ' +
        payload.view.approved.orders.map((o) => o.treeId).concat(prunes, clears, plants).join(', ') +
        '. Zero ecology changed.');
    }, 'plan-heading');
  }

  function onCancelApproved() {
    run('api/multi/plan/cancel', { fence: currentFence() }, label('MultiCancelApproved'),
      () => announce('Done: ' + label('MultiCancelApproved') + '. Its orders were discarded, not returned to pending.'), 'plan-heading');
  }

  function openDialog(dialog, initialFocus) { state.opener = document.activeElement; dialog.showModal(); initialFocus.focus(); }
  function returnFocus() {
    const opener = state.opener;
    state.opener = null;
    if (byId('alert').contains(document.activeElement)) return;
    if (opener && opener.isConnected && !opener.closest('[hidden]') && !opener.disabled) opener.focus();
  }

  function openExecuteDialog() {
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

  function onExecuteConfirmed() {
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

  function sendAdvance(consent) {
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
      renderAll();
      const record = payload.view.annualHistory[payload.view.annualHistory.length - 1];
      announce('Done: year ' + record.yearBefore + ' → ' + record.yearAfter + '.' +
        (drafts ? ' ' + plural(drafts, 'unsent draft choice was', 'unsent draft choices were') + ' cleared.' : ''));
      showPane('history');
    }, 'year-heading');
  }

  function onAdvance() {
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
      'removes ALL pending and approved orders in BOTH areas and executes nothing.' : 'The Work Plan has no unexecuted work. Advancing the year executes nothing.';
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

  async function reread(announceIt) {
    if (state.busy) return;
    const lockCountAtStart = state.lockCount;
    state.busy = true;
    const outcome = await call('api/multi/view');
    state.busy = false;
    if (outcome.kind === 'ok') {
      clearAlert();
      state.lastWasReplay = false;
      applyPayload(outcome.payload, false);
      if (state.lock && state.lockCount === lockCountAtStart) {
        state.lock = null;
        renderAll();
        if (state.competitionTarget && !state.competition) refreshCompetition();
      }
      if (announceIt && !state.lock) {
        announce('Re-read: ecological year ' + state.view.source.ecologicalYear + ', work revision ' + state.view.source.workRevision +
          ', plan-state revision ' + state.view.planStateRevision + '.');
        refreshReviewPreview();
        if (state.historyOpen) loadHistoryIndex(null, false);
        if (byId('side').getAttribute('data-pane') === 'details') refreshStructure();
      }
    } else if (state.lock) {
      showLockAlert(outcome.kind === 'guard' ? 'The re-read was refused (' + outcome.error.code + '). Try again.' :
        'The re-read got no answer from the local session process. Try again.');
    } else if (outcome.kind === 'guard') {
      showGuardRefusal('Re-read', outcome.status, outcome.error);
    } else {
      showAlert([el('p', { class: 'alert-title' }, 'The local session process did not answer the read.'), rereadButton()]);
    }
  }

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

  function renderSkipControl() {
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

  async function refreshReviewPreview() {
    if (!state.view || state.lock) return;
    const req = ++state.previewRequest;
    const outcome = await jsonCall('api/multi/year/preview-review', 'POST', { fence: currentFence() });
    if (req !== state.previewRequest || state.lock) return;
    if (outcome.kind === 'ok' && outcome.payload && typeof outcome.payload.suggestedTargetYear === 'number') {
      state.reviewPreview = outcome.payload;
      renderSkipControl();
    }
  }

  function renderSkipSummary() {
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

  async function onSkipReview() {
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

  function openHistoryDrawer() {
    if (!state.view) return;
    state.historyOpen = true;
    byId('history-backdrop').hidden = false;
    byId('history-drawer').hidden = false;
    byId('history-open').setAttribute('aria-expanded', 'true');
    byId('history-drawer-title').focus();
    loadHistoryIndex(null, false);
  }

  function closeHistoryDrawer(returnToOpener) {
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

  async function loadHistoryIndex(beforeYear, append) {
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

  async function loadHistoryYear(year) {
    if (!state.view || state.lock) return;
    const req = ++state.historyDetailRequest;
    const reqSession = state.view.sessionId;
    state.historySelectedYear = year;
    state.historyDetailLoading = true;
    state.historyDetail = null;
    state.historyError = null;
    renderHistoryDrawer();
    const outcome = await jsonCall('api/multi/history/year', 'POST', { fence: currentFence(), year: Number(year) });
    if (req !== state.historyDetailRequest || !state.view || state.view.sessionId !== reqSession) return;
    state.historyDetailLoading = false;
    if (outcome.kind === 'ok' && outcome.payload && typeof outcome.payload.workYear === 'number') {
      state.historyDetail = outcome.payload;
      state.historyError = null;
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

  function renderHistoryDrawer() {
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

  async function refreshStructure() {
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

  function renderStructure() {
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

  function wire() {
    byId('area-switch').addEventListener('click', (event) => {
      const button = event.target.closest('[data-area]');
      if (button) showArea(button.getAttribute('data-area'));
    });
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
    byId('plant-opt-shelter').addEventListener('change', () => renderMap());
    byId('plant-opt-spot').addEventListener('change', () => renderMap());
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
    byId('zoom-fit').addEventListener('click', () => { state.zoom = ZOOMS[0]; state.centre = null; renderMap(); renderMapControls(); });
    byId('pan-north').addEventListener('click', () => panMap(0, -1));
    byId('pan-south').addEventListener('click', () => panMap(0, 1));
    byId('pan-west').addEventListener('click', () => panMap(-1, 0));
    byId('pan-east').addEventListener('click', () => panMap(1, 0));
    let frame = 0;
    const reframe = () => {
      if (frame) return;
      frame = window.requestAnimationFrame(() => { frame = 0; if (state.view && state.labels) { renderMap(); renderMapControls(); } });
    };
    if (typeof ResizeObserver === 'function') new ResizeObserver(reframe).observe(byId('map-pane'));
    else window.addEventListener('resize', reframe);
  }

  initFloorDefs();
  renderLegend();
  wire();
  reread(false);
})();