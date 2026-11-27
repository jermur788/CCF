// Phase 2A-2: New Woodland modal dialog (spec §7.1–§7.2; D-020 zero JS forestry/bounds/count math).
// Every option, bound, preview count, area, stem list, and refusal message comes verbatim from the Core server.
import { announce, applyPayload, clearAlert, currentFence, refreshMultiStandFrame, showBudgetExceeded } from '../api.js';
import { openDialog } from '../commands.js';
import { el, fixed } from '../dom.js';
import { byId, hooks, isMultiLaneStand, resetSessionUiState, state } from '../store.js';
import { jsonCall, noticeOf } from './saves.js';

let previewTimer = 0;
let previewSeq = 0;
let pendingCreate = null;

function setWoodlandStatus(text, kind) {
  const box = byId('woodland-status');
  if (!box) return;
  box.textContent = text || '';
  box.className = 'save-status' + (kind ? ' ' + kind : '');
}

function selectedChoice() {
  const checked = document.querySelector('input[name="woodland-choice"]:checked');
  return checked ? checked.value : 'preset:fm-064-sitka-edge-broadleaves-v1';
}

function parseChoiceValue(val) {
  if (val === 'ma-1' || val === 'ma-1-mixed') return { choice: val, presetId: null };
  if (val === 'custom') return { choice: 'custom', presetId: null };
  if (val.startsWith('preset:')) return { choice: 'preset', presetId: val.slice('preset:'.length) };
  return { choice: 'ma-1', presetId: null };
}

function readCustomRecipeInput() {
  const opts = state.woodlandOptions;
  const bounds = opts && opts.bounds ? opts.bounds : null;
  const intVal = (id) => {
    const raw = (byId(id).value || '').trim();
    if (!/^-?\d+$/.test(raw)) return null;
    return Number(raw);
  };
  const mix = byId('fm-species-mix').value;
  return {
    generatorId: bounds ? bounds.generatorId : null,
    generatorVersion: bounds ? bounds.generatorVersion : null,
    presetId: null,
    widthM: intVal('fm-width'),
    lengthM: intVal('fm-length'),
    laneCount: intVal('fm-lanes'),
    startAgeYears: intVal('fm-age'),
    stemsPerHa: intVal('fm-stems-ha'),
    seed: intVal('fm-seed'),
    speciesMix: mix,
    oakPercent: mix === 'CustomSplit' ? intVal('fm-oak-pct') : 0,
    beechPercent: mix === 'CustomSplit' ? intVal('fm-beech-pct') : 0,
  };
}

function renderCustomVisibility() {
  const { choice } = parseChoiceValue(selectedChoice());
  byId('woodland-custom-fields').hidden = choice !== 'custom';
  const mix = byId('fm-species-mix').value;
  byId('fm-split-fields').hidden = choice !== 'custom' || mix !== 'CustomSplit';
  byId('woodland-confirm-box').hidden = true;
  pendingCreate = null;
}

function renderPreviewBox(preview, refusal, builtIn) {
  const box = byId('woodland-preview');
  if (!box) return;
  if (builtIn) {
    box.className = 'woodland-preview ok';
    box.replaceChildren(
      el('p', { class: 'woodland-preview-title' }, builtIn.title),
      el('p', { class: 'small-note' }, builtIn.badge),
      el('p', { class: 'small-note' }, 'Built-in 2-area stand. Saves use frozen Save v2 when Money is Off.'));
    return;
  }
  if (refusal) {
    box.className = 'woodland-preview bad';
    box.replaceChildren(
      el('p', { class: 'woodland-preview-title' }, 'Refused by Core (' + refusal.code + ')'),
      el('p', null, refusal.message));
    return;
  }
  if (!preview) {
    box.className = 'woodland-preview';
    box.replaceChildren(el('p', { class: 'small-note' }, 'Requesting server preview…'));
    return;
  }
  box.className = 'woodland-preview ok';
  const laneBits = (preview.lanes || []).map((l) =>
    l.label + ': ' + l.totalStems + ' (' + l.sitkaCount + ' S' +
    (l.oakCount > 0 ? ', ' + l.oakCount + ' O' : '') +
    (l.beechCount > 0 ? ', ' + l.beechCount + ' B' : '') + ')');
  box.replaceChildren(
    el('p', { class: 'woodland-preview-title' },
      fixed(preview.totalAreaHa, 2) + ' ha · ' + preview.laneCount + ' lanes · ' +
      preview.totalStems + ' living stems · ' + preview.cellCount + ' ground cells'),
    el('p', { class: 'small-note' }, preview.badge),
    el('p', { class: 'small-note mono' }, 'ID: ' + preview.experimentId),
    el('p', { class: 'small-note' }, laneBits.join(' · ')),
    el('p', { class: 'small-note' }, preview.disclosure));
}

export async function requestWoodlandPreview() {
  const opts = state.woodlandOptions;
  if (!opts) return;
  const { choice, presetId } = parseChoiceValue(selectedChoice());
  if (choice === 'ma-1' || choice === 'ma-1-mixed') {
    const builtIn = (opts.builtIns || []).find((b) => b.key === choice) || null;
    renderPreviewBox(null, null, builtIn);
    byId('woodland-create').disabled = false;
    return;
  }
  let recipe = null;
  if (choice === 'preset') {
    const preset = (opts.presets || []).find((p) => p.presetId === presetId);
    if (preset) recipe = preset.recipe;
  } else {
    recipe = readCustomRecipeInput();
  }
  if (!recipe) return;

  const seq = ++previewSeq;
  renderPreviewBox(null, null, null);
  const outcome = await jsonCall('api/multi/woodland/preview', 'POST', { recipe });
  if (seq !== previewSeq) return;
  if (outcome.kind === 'ok' && outcome.payload && outcome.payload.valid && outcome.payload.preview) {
    renderPreviewBox(outcome.payload.preview, null, null);
    byId('woodland-create').disabled = false;
  } else {
    const notice = noticeOf(outcome);
    renderPreviewBox(null, notice, null);
    byId('woodland-create').disabled = true;
  }
}

export function onWoodlandFieldChanged() {
  renderCustomVisibility();
  if (previewTimer) window.clearTimeout(previewTimer);
  previewTimer = window.setTimeout(() => {
    previewTimer = 0;
    requestWoodlandPreview();
  }, 150);
}

function populateOptionsUi(opts) {
  state.woodlandOptions = opts;
  const list = byId('woodland-choices');
  const rows = [];
  const addRadio = (value, title, sub, checked) => {
    const id = 'wc-' + value.replace(/[^a-z0-9]+/gi, '-');
    const input = el('input', { type: 'radio', name: 'woodland-choice', id, value });
    if (checked) input.checked = true;
    rows.push(el('label', { class: 'woodland-choice', for: id },
      input,
      el('span', { class: 'woodland-choice-text' },
        el('strong', null, title),
        el('span', { class: 'small-note' }, sub))));
  };

  for (const preset of (opts.presets || [])) {
    addRadio('preset:' + preset.presetId, preset.title,
      preset.totalStems + ' stems · ' + preset.cellCount + ' cells · ' + preset.experimentId,
      preset.presetId === 'fm-064-sitka-edge-broadleaves-v1');
  }
  for (const b of (opts.builtIns || [])) {
    addRadio(b.key, b.title, b.badge, false);
  }
  addRadio('custom', 'Custom rectangle', 'Configure dimensions, lanes, age, stocking, seed and species mix (validated by Core)', false);
  list.replaceChildren(...rows);

  // Populate display-only bounds from Core (D-020: zero hardcoded forestry bounds in JS).
  const b = opts.bounds;
  if (b) {
    const setAttr = (id, min, max, fallback) => {
      const inp = byId(id);
      if (!inp) return;
      inp.min = String(min);
      inp.max = String(max);
      if (!inp.value && fallback !== undefined) inp.value = String(fallback);
    };
    const defaultRecipe = (opts.presets && opts.presets[0] && opts.presets[0].recipe) || null;
    setAttr('fm-width', b.minWidthM, b.maxWidthM, defaultRecipe ? defaultRecipe.widthM : undefined);
    setAttr('fm-length', b.minLengthM, b.maxLengthM, defaultRecipe ? defaultRecipe.lengthM : undefined);
    setAttr('fm-lanes', b.minLaneCount, b.maxLaneCount, defaultRecipe ? defaultRecipe.laneCount : undefined);
    setAttr('fm-age', b.minStartAgeYears, b.maxStartAgeYears, defaultRecipe ? defaultRecipe.startAgeYears : undefined);
    setAttr('fm-stems-ha', b.minStemsPerHa, b.maxStemsPerHa, defaultRecipe ? defaultRecipe.stemsPerHa : undefined);
    if (!byId('fm-seed').value && defaultRecipe) byId('fm-seed').value = String(defaultRecipe.seed);
    byId('fm-bounds-hint').textContent =
      'Core bounds: width/length ' + b.minWidthM + '–' + b.maxWidthM + ' m, lanes ' + b.minLaneCount + '–' + b.maxLaneCount +
      ', age ' + b.minStartAgeYears + '–' + b.maxStartAgeYears + ' yr, stocking ' + b.minStemsPerHa + '–' + b.maxStemsPerHa +
      ' trees/ha, hard cap ' + b.maxLivingStems + ' living stems.';
    const sel = byId('fm-species-mix');
    const prev = sel.value;
    sel.replaceChildren(...(b.speciesMixes || []).map((opt) => el('option', { value: opt.value }, opt.label)));
    if (prev && [...sel.options].some((o) => o.value === prev)) sel.value = prev;
  }

  byId('woodland-economy').checked = Boolean(opts.current && opts.current.economyEnabled);
  state.hasUnsavedProgress = Boolean(opts.current && opts.current.hasUnsavedProgress);
  renderCustomVisibility();
}

export async function openWoodlandDialog() {
  if (!state.view || state.busy || state.batch || state.queueRunning) return;
  setWoodlandStatus('', null);
  byId('woodland-confirm-box').hidden = true;
  pendingCreate = null;
  openDialog(byId('woodland-dialog'), byId('woodland-cancel'));
  const outcome = await jsonCall('api/multi/woodland/options', 'GET');
  if (outcome.kind !== 'ok' || !outcome.payload) {
    const notice = noticeOf(outcome);
    setWoodlandStatus('Could not load woodland options (' + notice.code + '): ' + notice.message, 'bad');
    return;
  }
  populateOptionsUi(outcome.payload);
  await requestWoodlandPreview();
}

async function submitNewWoodland(confirmDiscardUnsaved) {
  if (!state.view || state.busy) return;
  const { choice, presetId } = parseChoiceValue(selectedChoice());
  const body = {
    fence: currentFence(),
    choice,
    presetId,
    recipe: choice === 'custom' ? readCustomRecipeInput() : null,
    economyEnabled: Boolean(byId('woodland-economy').checked),
    confirmDiscardUnsaved: Boolean(confirmDiscardUnsaved),
  };

  state.busy = true;
  document.documentElement.classList.add('busy');
  const outcome = await jsonCall('api/multi/woodland/new', 'POST', body);
  state.busy = false;
  document.documentElement.classList.remove('busy');

  if (outcome.kind === 'ok' && outcome.payload && outcome.payload.view) {
    const useLaneReads = outcome.payload.view.areas && outcome.payload.view.areas.length >= 3;
    resetSessionUiState();
    clearAlert();
    byId('woodland-dialog').close();
    if (useLaneReads) {
      const frame = await refreshMultiStandFrame(null, false, 'New woodland');
      if (frame.kind !== 'ok') return;
    } else {
      applyPayload(outcome.payload, false);
    }
    announce('Started new woodland: ' + (outcome.payload.experimentBadge || state.experimentBadge || state.view.experiment.experimentId) + '.');
    hooks.refreshReviewPreview();
    if (state.historyOpen) hooks.loadHistoryIndex(null, false);
    if (byId('side').getAttribute('data-pane') === 'details') hooks.refreshStructure();
    return;
  }

  if (outcome.kind === 'budget') {
    byId('woodland-dialog').close();
    showBudgetExceeded('New woodland', outcome.budget);
    return;
  }

  const notice = noticeOf(outcome);
  if (notice.code === 'WoodlandUnsavedProgress') {
    pendingCreate = body;
    byId('woodland-confirm-text').textContent = notice.message || 'Discard unsaved progress and start a new woodland?';
    byId('woodland-confirm-box').hidden = false;
    byId('woodland-confirm-discard').focus();
    setWoodlandStatus('Unsaved progress detected — confirm discard to replace the current woodland.', 'warn');
    return;
  }
  if (outcome.kind === 'refused' && outcome.payload && outcome.payload.view) {
    if (isMultiLaneStand()) await refreshMultiStandFrame(state.areaId, state.singleLaneMode, 'New woodland');
    else applyPayload(outcome.payload, false);
  }
  setWoodlandStatus('Refused (' + notice.code + '): ' + notice.message, 'bad');
}

export async function onWoodlandCreate() {
  if (!state.view || state.busy) return;
  if (state.hasUnsavedProgress) {
    byId('woodland-confirm-text').textContent = 'Discard unsaved progress and start a new woodland?';
    byId('woodland-confirm-box').hidden = false;
    byId('woodland-confirm-discard').focus();
    return;
  }
  await submitNewWoodland(false);
}

export async function onWoodlandConfirmDiscard() {
  byId('woodland-confirm-box').hidden = true;
  pendingCreate = null;
  await submitNewWoodland(true);
}

export function onWoodlandCancelDiscard() {
  byId('woodland-confirm-box').hidden = true;
  pendingCreate = null;
  byId('woodland-create').focus();
}
