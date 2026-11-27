// S0c provenance: store.js [L0] — section(s) 0, 2 from multi.js
  export const SVG_NS = 'http://www.w3.org/2000/svg';
  export const SPOT_PREVIEW_RADIUS_M = Math.sqrt(1 / Math.PI); // visual radius (~0.564 m) for ~1 m2 Spot Clear circle preview
  const STATUS_LABEL = { Living: 'StatusLiving', PendingFell: 'StatusPendingFell', ApprovedFell: 'StatusApprovedFell',
    Felled: 'StatusFelled', NaturalDead: 'StatusNaturalDead' };
  const SPECIES_NAME = { 'sitka-spruce': 'Sitka spruce', 'sessile-oak': 'Sessile oak', beech: 'European beech' };
  export const SPECIES_SHORT = { 'sitka-spruce': 'S', 'sessile-oak': 'O', beech: 'B' };
  export const SPECIES_SYMBOL = { 'sitka-spruce': 'ccf-sitka', 'sessile-oak': 'ccf-oak', beech: 'ccf-beech' };
  export const SAPLING_SYMBOL = { 'sitka-spruce': 'ccf-sapling-sitka', 'sessile-oak': 'ccf-sapling-oak', beech: 'ccf-sapling-beech' };
  export const ZOOMS = Array.from({ length: 11 }, (_, i) => Math.pow(4, i / 10));
  export const PANES = ['tree', 'navigator', 'plan', 'trees', 'history', 'details'];
  export const PANE_NAMES = { tree: 'Tree', navigator: 'Navigator', plan: 'Work Plan', trees: 'Trees', history: 'Results', details: 'Details' };
  export const LENS_IDS = ['woodland', 'trees', 'regeneration', 'harvesting', 'habitat'];
  export const LENSES = Object.freeze({
    woodland: Object.freeze({
      id: 'woodland',
      key: '1',
      icon: '◈',
      label: 'Woodland',
      shortDesc: 'Overview · canopy, living stems & lane orientation',
      defaultOverlay: 'off',
      tools: ['tree', 'cell'],
      legendKeys: ['sitka', 'oak', 'beech', 'crop', 'pending', 'approved', 'boundary'],
      extensionSlots: [],
    }),
    trees: Object.freeze({
      id: 'trees',
      key: '2',
      icon: '◎',
      label: 'Trees',
      shortDesc: 'Dimensions, competition, crop-tree marking, pruning & timber quality',
      defaultOverlay: 'off',
      tools: ['tree'],
      legendKeys: ['sitka', 'oak', 'beech', 'crop', 'reference', 'pending', 'approved'],
      extensionSlots: [
        { id: 'scots-pine-profile', feature: 'Scots Pine & Soil Moisture', label: 'Scots pine crown, growth & pruning eligibility' },
        { id: 'birch-alder-profiles', feature: 'Downy Birch & Common Alder', label: 'Downy birch & common alder crown/stem traits' },
        { id: 'real-woodland-measurements', feature: 'Real-World Woodland Inspection', label: 'Year-0 measured survey stem provenance' },
      ],
    }),
    regeneration: Object.freeze({
      id: 'regeneration',
      key: '3',
      icon: '❖',
      label: 'Regeneration',
      shortDesc: 'Light, soil moisture, seedlings, planting, shelters & undergrowth',
      defaultOverlay: 'light',
      tools: ['tree', 'cell', 'plant', 'spot'],
      legendKeys: ['seedlings', 'shelter-active', 'shelter-expired', 'spot-clear', 'clear-undergrowth'],
      extensionSlots: [
        { id: 'soil-moisture-overlay', feature: 'Scots Pine & Soil Moisture', label: '5-level soil moisture suitability layer' },
        { id: 'jay-acorn-dispersal', feature: 'Jay Acorn Dispersal & 2D Windstorms', label: 'Mast-year oak acorn landings & nearby oak edge' },
        { id: 'birch-alder-planting', feature: 'Downy Birch & Common Alder', label: 'Additional native nursery planting species' },
      ],
    }),
    harvesting: Object.freeze({
      id: 'harvesting',
      key: '4',
      icon: '◧',
      label: 'Harvesting',
      shortDesc: 'Proposed felling, timber grades, 3-way destinations, quotes & salvage',
      defaultOverlay: 'off',
      tools: ['tree'],
      legendKeys: ['pending', 'approved', 'felled', 'crop', 'reference'],
      extensionSlots: [
        { id: 'scots-pine-timber-grader', feature: 'Scots Pine & Soil Moisture', label: 'Scots pine Sawlog / SRW / Pulp timber grades' },
        { id: 'windstorm-salvage', feature: 'Jay Acorn Dispersal & 2D Windstorms', label: 'Fallen storm-stem salvage & windblow market note' },
      ],
    }),
    habitat: Object.freeze({
      id: 'habitat',
      key: '5',
      icon: '◐',
      label: 'Habitat',
      shortDesc: 'Deadwood, diversity, microhabitats, sensitive ground & resilience',
      defaultOverlay: 'undergrowth',
      tools: ['tree', 'cell'],
      legendKeys: ['standing-dead', 'fallen-log', 'sitka', 'oak', 'beech', 'clear-undergrowth'],
      extensionSlots: [
        { id: 'real-woodland-microhabitats', feature: 'Real-World Woodland Inspection', label: 'TreMs microhabitats & survey provenance' },
        { id: 'windstorm-deadwood-resilience', feature: 'Jay Acorn Dispersal & 2D Windstorms', label: 'Wind-aligned fallen deadwood & resilience' },
        { id: 'sensitive-wet-ground', feature: 'Scots Pine & Soil Moisture', label: 'Sensitive / wet-ground suitability markers' },
      ],
    }),
  });
  export const ALERT_TIERS = Object.freeze({
    blocking: Object.freeze({ rank: 1, icon: '⛔', label: 'Blocking', badge: 'BLOCKING' }),
    attention: Object.freeze({ rank: 2, icon: '▲', label: 'Attention Required', badge: 'ATTENTION' }),
    event: Object.freeze({ rank: 3, icon: '◆', label: 'Significant Event', badge: 'EVENT' }),
    info: Object.freeze({ rank: 4, icon: 'ⓘ', label: 'Information', badge: 'INFO' }),
    historical: Object.freeze({ rank: 5, icon: '◷', label: 'Historical', badge: 'HISTORY' }),
  });
  export const EXPLANATIONS = Object.freeze({
    'lenses-tools': Object.freeze({
      title: 'Lenses vs active tools',
      body: 'Lenses change map emphasis, default overlay, and available tools only. Switching a lens never changes the forest, never approves or executes work, and always returns to safe Select Tree inspection.',
      tag: 'SAFE PRESENTATION',
    }),
    'hegyi-ci': Object.freeze({
      title: 'Hegyi Competition Index (CI)',
      body: 'Server-computed local crowding score from living neighbours inside the 8 m cutoff, weighted by neighbour DBH and distance. Higher CI means stronger local competition on the reference tree.',
      tag: 'ENGINE READOUT',
    }),
    'crop-tree': Object.freeze({
      title: 'Retained Crop Tree (C)',
      body: 'A living tree designated for long-term retention (double gold ring + C badge). Retained Crop Trees cannot be queued for felling unless unmarked first.',
      tag: 'CCF MARKING',
    }),
    'work-plan-gate': Object.freeze({
      title: 'Work Plan commitment gate',
      body: 'Every action follows Mark → Quote → Approve → Execute → Advance Time. Pending and approved orders change zero ecology until Execute is explicitly confirmed.',
      tag: 'SAFETY GATE',
    }),
    'harvest-floor': Object.freeze({
      title: '€2,500 minimum contractor harvest floor',
      body: 'Commercial contractor felling applies a calibrated €2,500 minimum job fee. Small cuts on small woods can net a loss when variable tonnage falls below the floor.',
      tag: 'GAMEPLAY CALIBRATION',
    }),
    'felling-choices': Object.freeze({
      title: '3-way felling destinations',
      body: 'Each pending Fell stem can be set to Sell & Extract (roadside timber sale), Keep for Own Use (engine-graded retained timber, non-cash), or Leave as Deadwood (fallen habitat log).',
      tag: 'WORK PLAN',
    }),
    'modelled-light': Object.freeze({
      title: 'Modelled light (not calibrated PAR)',
      body: 'Relative 0–1 canopy-gap light estimate on 2.5 m ground cells computed by the engine. It compares shade and openings inside this simulator rather than measured field PAR.',
      tag: 'MODEL CAVEAT',
    }),
    'navigator-scope': Object.freeze({
      title: 'Visible lanes vs whole-woodland scope',
      body: 'Multi-lane woodlands load 1–3 detailed lanes at a time for fast display. Lanes and Work Plan counts cover the whole woodland; Marked Crop Tree rows list trees in currently loaded visible lanes.',
      tag: 'COUNT SCOPE',
    }),
  });
  export const SHORTCUTS = Object.freeze([
    ['1 – 5', 'Switch presentation lens (Woodland, Trees, Regeneration, Harvesting, Habitat)'],
    ['[ / ]', 'Switch to previous / next lane or area'],
    ['H', 'Open or close the Woodland History drawer'],
    ['W', 'Open the Work Plan tab in the right-hand panel'],
    ['M', 'Toggle 1-lane precision vs adjacent-lanes view (multi-lane stands)'],
    ['?', 'Open or close this keyboard shortcuts guide'],
    ['Esc', 'Dismiss topmost layer (Dialog → Pinned explanation → Full legend → History → Tool → Selection)'],
  ]);
  export const COMMAND_IDS = ['crop-button', 'add-reference', 'insp-remove', 'approve', 'cancel-approved', 'execute',
    'execute-confirm', 'advance', 'skip-review', 'consent-advance', 'prune-add', 'prune-remove',
    'cell-clear-add', 'cell-clear-remove', 'plant-queue', 'spot-queue', 'pj-shelter-add', 'pj-shelter-remove'];
  const LIGHT_STOPS = [[0x7f, 0x3b, 0x4a], [0xda, 0x79, 0x6d], [0xff, 0xd9, 0xbd]];
  const LIGHT_EASE = 0.65;
  const LIGHT_ALPHA_MAX = 0.9;
  const LIGHT_WORDS = [[0.1, 'Very shaded'], [0.35, 'Low'], [0.65, 'Moderate'], [Infinity, 'Bright']];
  export const SLATE = [0x1b, 0x24, 0x30];
  export const STATEMENTS = ['MultiSyntheticStatement', 'MultiNotComparable', 'MultiSiteUnknown', 'MultiEdgeExcluded',
    'MultiNoTransfer', 'MultiSessionOnly'];
  export const CHIPS = [['MultiChipSynthetic', 'MultiSyntheticStatement'], ['MultiChipNotComparable', 'MultiNotComparable'],
    ['MultiChipSiteUnknown', 'MultiSiteUnknown'], ['MultiChipEdgeExcluded', 'MultiEdgeExcluded'],
    ['MultiChipNoTransfer', 'MultiNoTransfer']];
  export const DEFAULT_REASON = 'Owner-selected felling';

  export const state = {
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
    // Phase 2B-2a Economy UI state (economyEnabled is explicit server truth; economy is null when Off; zero JS math).
    economyEnabled: false,
    economy: null,
    economyToggleTarget: null,
    historyEconomyDetail: null,
    historyEconomyError: null,
    // Phase 2A-2 Forest Maker & Multi-Lane UI state (presentation only; all forestry data from Core).
    singleLaneMode: false,
    standOverview: null,
    laneRequest: 0,
    woodlandOptions: null,
    hasUnsavedProgress: false,
    recipe: null,
    experimentBadge: null,
    // Map-First Menu & Lenses UI state (strictly presentation/navigation only; zero forestry math).
    lens: 'woodland',
    legendExpanded: false,
    alertsExpanded: false,
    dismissedAlerts: {},
    manualAlert: null,
    extensionAlerts: [],
    navigatorCursor: { crops: -1, work: -1 },
  };

  export function isEconomyEnabled() {
    return state.economyEnabled === true;
  }

  // Whole-woodland area list: for >=3-area stands rendered via C-FENCE lane-filtered reads,
  // state.standOverview holds all lanes while state.view.areas holds only the 1-3 loaded detail lanes.
  export function allAreas() {
    if (state.standOverview && Array.isArray(state.standOverview.areas) && state.standOverview.areas.length > 0) {
      return state.standOverview.areas;
    }
    return state.view && Array.isArray(state.view.areas) ? state.view.areas : [];
  }

  export function overviewArea(areaId) {
    return areaId ? allAreas().find((area) => area.areaId === areaId) || null : null;
  }

  export function isMultiLaneStand() {
    if (allAreas().length >= 3) return true;
    return Boolean(state.view && state.view.source && Array.isArray(state.view.source.areaIds) && state.view.source.areaIds.length >= 3);
  }

  export function maxAdjacentLanes(viewportWidth) {
    const w = typeof viewportWidth === 'number' && viewportWidth > 0
      ? viewportWidth
      : (typeof window !== 'undefined' && window.innerWidth ? window.innerWidth : 1920);
    return w >= 1440 ? 3 : 2;
  }

  export function visibleLaneIds(areas, activeAreaId, singleLaneMode, viewportWidth) {
    if (!areas || areas.length === 0) return [];
    const active = areas.some((a) => a.areaId === activeAreaId) ? activeAreaId : areas[0].areaId;
    if (areas.length <= 2 || singleLaneMode) return [active];
    const idx = areas.findIndex((a) => a.areaId === active);
    const maxLanes = maxAdjacentLanes(viewportWidth);
    if (maxLanes <= 2) {
      if (idx < areas.length - 1) return [areas[idx].areaId, areas[idx + 1].areaId];
      return [areas[idx - 1].areaId, areas[idx].areaId];
    }
    const lo = Math.max(0, idx - 1);
    const hi = Math.min(areas.length - 1, idx + 1);
    return areas.slice(lo, hi + 1).map((a) => a.areaId);
  }

  export function currentLens() {
    return LENSES[state.lens] || LENSES.woodland;
  }

  // Drops every local selection, draft, preview, in-flight request token and cached panel state when the server
  // replaces the active session (Save/Load restore or launch-time Money On/Off fresh Year-0 session).
  export function resetSessionUiState() {
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
    state.historyEconomyDetail = null;
    state.historyEconomyError = null;
    state.historyLoading = false;
    state.historyDetailLoading = false;
    state.historyRequest += 1;
    state.historyDetailRequest += 1;
    state.structure = null;
    state.structureLoading = false;
    state.structureError = null;
    state.structureRootTag = null;
    state.structureRequest += 1;
    state.economyEnabled = false;
    state.economy = null;
    state.economyToggleTarget = null;
    state.singleLaneMode = false;
    state.standOverview = null;
    state.laneRequest += 1;
    state.hasUnsavedProgress = false;
    state.recipe = null;
    state.experimentBadge = null;
    state.lens = 'woodland';
    state.legendExpanded = false;
    state.alertsExpanded = false;
    state.dismissedAlerts = {};
    state.manualAlert = null;
    state.extensionAlerts = [];
    state.navigatorCursor = { crops: -1, work: -1 };
  }

  export const byId = (id) => document.getElementById(id);

  // ---- lookups in the latest atomic composite payload -----------------------------------------------------------------

  export function areaView(areaId) {
    return state.view && areaId ? state.view.areas.find((area) => area.areaId === areaId) || null : null;
  }
  export function currentArea() { return areaView(state.areaId); }
  export function treeIn(areaId, treeId) {
    const area = areaView(areaId);
    return area && treeId ? area.trees.find((tree) => tree.treeId === treeId) || null : null;
  }
  export function livingIn(areaId, treeId) {
    const tree = treeIn(areaId, treeId);
    return Boolean(tree && tree.isLiving);
  }
  export function areaOfTree(treeId) {
    return state.view ? (state.view.areas.find((area) => area.trees.some((tree) => tree.treeId === treeId)) || null) : null;
  }
  export function sceneOf(areaId) { return (state.scenes && state.scenes[areaId]) || []; }
  export function itemOf(areaId, treeId) { return sceneOf(areaId).find((item) => item.treeId === treeId) || null; }
  export function areaName(areaId) { const area = areaView(areaId) || overviewArea(areaId); return area ? area.label : String(areaId); }
  export function areaShort(areaId) { const area = areaView(areaId) || overviewArea(areaId); return area ? area.label : String(areaId); }
  export function selectedId() { return state.areaId ? state.selected[state.areaId] || null : null; }
  export function ordersIn(orders, areaId) { return orders.filter((order) => order.areaId === areaId); }

  export function speciesOf(treeId) {
    if (state.ecology && state.ecology.speciesByTreeId && state.ecology.speciesByTreeId[treeId]) {
      return state.ecology.speciesByTreeId[treeId];
    }
    return 'sitka-spruce';
  }
  export function speciesLabel(speciesId) { return SPECIES_NAME[speciesId] || speciesId; }

  export function s4Area(areaId) {
    const readout = state.stage4;
    return readout && areaId ? readout.areas.find((area) => area.areaId === areaId) || null : null;
  }
  export function s4Tree(areaId, treeId) {
    const area = s4Area(areaId);
    return area && treeId ? area.trees.find((tree) => tree.treeId === treeId) || null : null;
  }
  export function ecoArea(areaId) {
    const eco = state.ecology;
    return eco && areaId ? eco.areas.find((area) => area.areaId === areaId) || null : null;
  }
  // Engine-authoritative species-aware prune status (prefers ecology.pruneStatusByTreeId, falls back to stage4).
  export function pruneStatusOf(areaId, treeId) {
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
  export function pendingPrunes() { return state.stage4 ? state.stage4.pendingPrunes : []; }
  export function approvedPrunes() { return state.stage4 ? state.stage4.approvedPrunes : []; }
  export function pendingClears() { return state.ecology ? (state.ecology.pendingClear || []) : []; }
  export function approvedClears() { return state.ecology ? (state.ecology.approvedClear || []) : []; }
  export function pendingPlantings() { return state.ecology ? (state.ecology.pendingPlanting || []) : []; }
  export function approvedPlantings() { return state.ecology ? (state.ecology.approvedPlanting || []) : []; }
  export function worldFromMap(area, u, v) {
    return { xM: area.extent.minXM + u, zM: area.extent.minZM + area.extent.sideM - v };
  }
  export function plantedIn(areaId) {
    return state.ecology ? (state.ecology.planted || []).filter((j) => j.areaId === areaId) : [];
  }
  export function juvenileById(juvenileId) {
    return state.ecology && juvenileId ? (state.ecology.planted || []).find((j) => j.juvenileId === juvenileId) || null : null;
  }
  export function currentJuvenile() {
    const sel = state.selectedJuvenile;
    if (!sel || sel.areaId !== state.areaId) return null;
    const j = juvenileById(sel.juvenileId);
    return j && j.areaId === state.areaId ? j : null;
  }
  export function shelterForJuvenile(j) {
    if (!state.ecology || !j) return null;
    const list = (state.ecology.shelters || []).filter((s) => s.juvenileId === j.juvenileId);
    return list.find((s) => s.isEffective) || list[list.length - 1] || null;
  }
  export function matchingShelterOrder(orders, j) {
    if (!j) return null;
    return orders.find((o) => o.kind === 'Shelter' && o.areaId === j.areaId &&
      Math.abs(o.xm - j.xm) < 1e-4 && Math.abs(o.zm - j.zm) < 1e-4) || null;
  }
  export function isStage4() { return Boolean(state.stage4 && state.stage4.models.adultModelVersion === 1); }
  export function isPhase2() { return Boolean(state.ecology && state.ecology.models && state.ecology.models.species === 1); }
  export function readoutState() { return !state.stage4 ? 'unavailable' : isPhase2() ? 'phase2' : isStage4() ? 'stage4' : 'legacy'; }

  export function compactCellsFor(areaId) {
    if (!state.ecology || !state.view) return new Map();
    const areaIdx = state.view.areas.findIndex((a) => a.areaId === areaId);
    if (areaIdx < 0) return new Map();
    const map = new Map();
    for (const row of state.ecology.cellsCompact) {
      if (row[0] === areaIdx) map.set(row[1] + ':' + row[2], row);
    }
    return map;
  }

  export function lightPaint(index) {
    if (!(index > 0)) return null;
    const t = Math.pow(Math.min(1, index), LIGHT_EASE);
    const segment = t <= 0.5 ? 0 : 1;
    const f = segment === 0 ? t / 0.5 : (t - 0.5) / 0.5;
    const rgb = LIGHT_STOPS[segment].map((low, i) => Math.round(low + (LIGHT_STOPS[segment + 1][i] - low) * f));
    return { colour: '#' + rgb.map((v) => v.toString(16).padStart(2, '0')).join(''), opacity: opacityText(LIGHT_ALPHA_MAX * t) };
  }
  export function opacityText(opacity) {
    const decimals = Math.max(4, 2 - Math.floor(Math.log10(opacity)));
    return decimals <= 100 ? opacity.toFixed(decimals) : opacity.toExponential(3);
  }
  export function lightWord(index) {
    const word = LIGHT_WORDS.find(([below]) => index < below)[1];
    return index > 0 ? word : word + ' · no modelled light';
  }
  export function groundCell(areaId, column, row) {
    const area = s4Area(areaId);
    return area ? area.ground.find((cell) => cell.column === column && cell.row === row) || null : null;
  }
  export function pct(fraction) { return hooks.fixed(fraction * 100, 0) + '%'; }
  export function pct1(fraction) { return hooks.fixed(fraction * 100, 1) + '%'; }
  export function baseStatusText(item) {
    const template = hooks.label(STATUS_LABEL[item.status]);
    return template ? hooks.fill(template, { y: item.deathYear }) : String(item.status);
  }
  export function statusText(item) { return item.isCrop ? hooks.label('CropRetained') : baseStatusText(item); }

  export function referenceIn(areaId) {
    const id = state.selected[areaId];
    return id && livingIn(areaId, id) ? id : null;
  }
  export function draftOf(areaId) {
    const draft = state.drafts[areaId];
    const reference = referenceIn(areaId);
    return draft && reference && draft.ref === reference ? draft.ids : [];
  }
  export function draftTotal() {
    return allAreas().reduce((sum, area) => sum + draftOf(area.areaId).length, 0);
  }
  export function currentReport() {
    const comp = state.competition;
    if (!comp || state.lock || comp.areaId !== state.areaId) return null;
    return comp.report && comp.report.targetTreeId === referenceIn(state.areaId) ? comp.report : null;
  }

  export function blockedReason(areaId, treeId) {
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
  export function shortBlockedReason(why) { return SHORT_BLOCKED[why] || why; }


  // S0c P4 hooks registry
  export const hooks = Object.seal({
    clearCompetition: null,
    competitionTag: null,
    confirmationStillShown: null,
    experimentBadgeText: null,
    fill: null,
    fixed: null,
    label: null,
    loadHistoryIndex: null,
    reconcileDrafts: null,
    refreshCompetition: null,
    refreshReviewPreview: null,
    refreshStructure: null,
    renderAll: null,
    renderPruneGate: null,
    renderSkipSummary: null,
    reread: null,
    toggleDraft: null,
    withdrawConfirmation: null,
    workTotals: null,
  });
