// Woodland Inspection Part 2 (spec multi-2d-woodland-inspection-v1): the offline built-in real-woodland catalogue and
// the read-only Year-0 survey screen. Presentation only (D-020): every value, plain-English label, readiness word and
// blocker comes from the server's Part 1 inspection view. This module computes no forestry, never estimates a missing
// value, never infers a boundary, and only frames the map around the recorded tree positions.
import { announce, clearAlert } from '../api.js';
import { openDialog } from '../commands.js';
import { el, plural, svg } from '../dom.js';
import { byId, hooks, state } from '../store.js';
import { jsonCall, noticeOf } from './saves.js';

const TREE_FIELDS = ['speciesName', 'vitalStatus', 'positionLocalM', 'dbhCm', 'heightM', 'crownRadiusM', 'ageYears'];
let renderedKey = null;

function setStatus(text, kind) {
  const box = byId('inspect-status');
  box.textContent = text || '';
  box.className = 'save-status' + (kind ? ' ' + kind : '');
}

function link(href, text) {
  return el('a', { href, target: '_blank', rel: 'noopener noreferrer' }, text);
}

// Display rounding only: at most two decimals, trailing zeros dropped (no value is derived or estimated).
function num(value) {
  return typeof value === 'number' ? String(Number(value.toFixed(2))) : String(value);
}

function labelsOf() {
  return state.inspection.labels;
}

function treesOf() {
  return state.inspection.view.observedYear0.trees;
}

function treeById(treeId) {
  return treesOf().find((t) => t.sourceTreeId === treeId) || null;
}

// ---- catalogue dialog ---------------------------------------------------------------------------------------------

export async function openInspectionDialog() {
  if (!state.view || state.busy || state.batch || state.queueRunning) return;
  setStatus('Reading the built-in survey plots…', null);
  byId('inspect-choices').replaceChildren();
  byId('inspect-load').disabled = true;
  openDialog(byId('inspect-dialog'), byId('inspect-cancel'));
  const outcome = await jsonCall('api/multi/woodland/inspection/catalogue', 'GET');
  if (outcome.kind !== 'ok' || !outcome.payload || !Array.isArray(outcome.payload.entries)) {
    setStatus('The built-in survey plots could not be read: ' + noticeOf(outcome).message, 'bad');
    return;
  }
  const entries = outcome.payload.entries;
  const current = outcome.payload.inspection && outcome.payload.inspection.active ? outcome.payload.inspection.plotId : null;
  byId('inspect-choices').replaceChildren(...entries.map((entry, index) => {
    const id = 'inspect-choice-' + index;
    const input = el('input', { type: 'radio', name: 'inspect-choice', id, value: entry.plotId });
    if ((current && entry.plotId === current) || (!current && index === 0)) input.checked = true;
    const dates = entry.observationDates.length > 0 ? 'Surveyed ' + entry.observationDates.join(', ') : 'Survey date not recorded';
    return el('label', { class: 'woodland-choice', for: id, 'data-inspect-plot': entry.plotId },
      input,
      el('span', { class: 'woodland-choice-text' },
        el('strong', null, entry.displayName),
        el('span', { class: 'small-note' }, plural(entry.treeCount, 'tree', 'trees') + ' surveyed (' + entry.livingStemCount +
          ' living) · ' + entry.subsetOfGbifRecords + ' · ' + dates),
        el('span', { class: 'small-note' }, entry.readinessLabel + ' · ' + entry.licenceSummary + ' · ' + entry.publisher)));
  }));
  setStatus(entries.length > 0 ? '' : 'No built-in survey plots are available in this build.', entries.length > 0 ? null : 'bad');
  byId('inspect-load').disabled = entries.length === 0;
}

export async function onInspectionLoad() {
  const checked = document.querySelector('input[name="inspect-choice"]:checked');
  if (!checked || state.busy) return;
  state.busy = true;
  byId('inspect-load').disabled = true;
  setStatus('Opening the survey…', null);
  const outcome = await jsonCall('api/multi/woodland/inspection/load', 'POST', { plotId: checked.value });
  state.busy = false;
  byId('inspect-load').disabled = false;
  if (outcome.kind !== 'ok' || !outcome.payload || !outcome.payload.current) {
    setStatus('The survey could not be opened: ' + noticeOf(outcome).message, 'bad');
    return;
  }
  enterInspection(outcome.payload.current);
  // The dialog's close handler returns focus to state.opener, so point it at the banner the player now needs to read.
  state.opener = byId('inspection-banner-title');
  byId('inspect-dialog').close();
  const entry = outcome.payload.current.entry;
  announce('Survey inspection mode, Year 0: ' + entry.displayName + '. ' + entry.advanceTimeStatement);
  byId('inspection-banner-title').focus();
}

// ---- entering, restoring and leaving inspection ------------------------------------------------------------------

function enterInspection(current) {
  const trees = current.view.observedYear0.trees;
  const firstPlotted = trees.find((t) => t.positionLocalM.value !== null) || trees[0] || null;
  state.inspection = { ...current, selectedTreeId: firstPlotted ? firstPlotted.sourceTreeId : null };
  renderedKey = null;
  clearAlert();
  hooks.renderAll();
}

// After a page reload the server still knows whether a survey is open, so the page shows exactly that (no stale screen).
export async function restoreInspection() {
  const outcome = await jsonCall('api/multi/woodland/inspection/current', 'GET');
  if (outcome.kind === 'ok' && outcome.payload && outcome.payload.current) enterInspection(outcome.payload.current);
}

export async function onInspectionExit() {
  if (!state.inspection || state.busy) return;
  const button = byId('inspection-exit');
  button.disabled = true;
  const outcome = await jsonCall('api/multi/woodland/inspection/exit', 'POST', {});
  button.disabled = false;
  if (outcome.kind !== 'ok' || !outcome.payload || !outcome.payload.inspection || outcome.payload.inspection.active) {
    announce('Could not leave survey inspection mode: ' + noticeOf(outcome).message);
    return;
  }
  state.inspection = null;
  renderedKey = null;
  hooks.renderAll();
  await hooks.reread(false);
  hooks.refreshReviewPreview();
  announce('Returned to your woodland' + (state.view ? ' at year ' + state.view.source.ecologicalYear : '') + '. Advance and the Work Plan are available again.');
  byId('map-heading').focus();
}

export function selectInspectedTree(treeId) {
  if (!state.inspection || !treeById(treeId)) return;
  const active = document.activeElement;
  const focusKey = active && active.getAttribute ? active.getAttribute('data-focus-key') : null;
  state.inspection.selectedTreeId = treeId;
  renderInspection();
  if (focusKey) {
    const again = document.querySelector('[data-focus-key="' + CSS.escape(focusKey) + '"]');
    if (again) again.focus();
  }
  announce('Selected surveyed tree ' + treeId + '.');
}

// ---- rendering ----------------------------------------------------------------------------------------------------

// Called from renderAll. Swaps the stand map workspace for the survey screen; rebuilds content only when it changed.
export function renderInspection() {
  const inspection = state.inspection;
  const active = Boolean(inspection);
  document.documentElement.classList.toggle('inspection-mode', active);
  byId('inspection-banner').hidden = !active;
  byId('inspection-view').hidden = !active;
  byId('workspace').hidden = active;
  for (const node of document.querySelectorAll('.summary, .notice')) node.hidden = active;
  if (active) byId('economy-strip').hidden = true;
  if (!active) {
    renderedKey = null;
    return;
  }
  const key = inspection.viewSha256 + '|' + inspection.selectedTreeId;
  if (key === renderedKey) return;
  const plotChanged = !renderedKey || !renderedKey.startsWith(inspection.viewSha256 + '|');
  renderedKey = key;
  if (plotChanged) {
    renderBanner();
    renderSpecies();
    renderMicrohabitats();
    renderSource();
  }
  renderMap();
  renderTreeInspector();
  renderTreeList();
}

function renderBanner() {
  const { entry, view } = state.inspection;
  const labels = labelsOf();
  byId('inspection-banner-title').textContent = labels.bannerTitle;
  byId('inspection-banner-title').setAttribute('tabindex', '-1');
  byId('inspection-banner-plot').textContent = entry.displayName + ' · ' + entry.readinessLabel;
  byId('inspection-banner-lead').textContent = labels.bannerLead + ' ' + entry.advanceTimeStatement;
  const items = view.blockerChecklist.map((item) => el('li', { class: 'inspection-blocker' },
    el('strong', null, item.title),
    el('span', { class: 'tag' }, labels.severity[item.severity] || ''),
    el('span', { class: 'inspection-blocker-text' }, ' ' + item.explanation),
    item.affectedTreeCount > 0
      ? el('span', { class: 'small-note' }, ' Affects ' + plural(item.affectedTreeCount, 'tree', 'trees') +
        (item.affectedRawSpeciesNames.length > 0 ? ': ' + item.affectedRawSpeciesNames.join(', ') : '') + '.')
      : null));
  byId('inspection-blockers').replaceChildren(...(items.length > 0 ? items : [el('li', null, entry.advanceTimeStatement)]));
  byId('inspection-attribution-line').replaceChildren(
    'Survey data: ', el('cite', null, entry.datasetTitle), ' — ' + entry.publisher + '. Licence: ',
    link(entry.licenceUrl, entry.licenceSummary), ' · DOI ', link(entry.doiUrl, entry.doi),
    ' · Modified by CCF: ' + entry.subsetOfGbifRecords + ' (details under Source, licence and changes).');
}

function categoryLegendItems() {
  const labels = labelsOf();
  return state.inspection.view.speciesBreakdown.categories.map((c) => el('li', null,
    el('span', { class: 'insp-swatch cat-' + c.category, 'aria-hidden': 'true' }),
    labels.speciesCategories[c.category] + ' (' + c.totalTrees + ')'));
}

function renderMap() {
  const { view, summary, entry } = state.inspection;
  const mapSvg = byId('inspection-map');
  const trees = view.observedYear0.trees.filter((t) => t.positionLocalM.value !== null);
  const boundary = view.observedYear0.boundary;
  const points = trees.map((t) => t.positionLocalM.value);
  if (boundary.observed) points.push(...boundary.verticesLocalM);
  byId('inspection-map-heading').textContent = 'Year-0 survey map — ' + entry.displayName;
  byId('inspection-map-note').textContent = 'Flat local metres exactly as stored: X runs east, Y runs north. ' +
    (boundary.observed
      ? 'The dashed outline is the surveyed boundary supplied with the data.'
      : 'No surveyed boundary was recorded, so none is drawn; the frame simply fits the recorded tree positions.');
  if (points.length === 0) {
    mapSvg.replaceChildren();
    mapSvg.setAttribute('viewBox', '0 0 10 10');
  } else {
    const xs = points.map((p) => p.xM);
    const ys = points.map((p) => p.yM);
    const minX = Math.min(...xs), maxX = Math.max(...xs), minY = Math.min(...ys), maxY = Math.max(...ys);
    const span = Math.max(maxX - minX, maxY - minY, 10);
    const pad = span * 0.08;
    const marker = span / 55;
    const font = span / 30;
    // A caption row above the trees (axis names) and a label row below them (east grid values), so text never overlaps.
    const left = minX - pad, top = -(maxY + pad + font * 1.6), width = maxX - minX + 2 * pad, height = maxY - minY + 2 * pad + font * 3;
    mapSvg.setAttribute('viewBox', [left, top, width, height].map(num).join(' '));
    mapSvg.setAttribute('preserveAspectRatio', 'xMidYMid meet');
    const nodes = [];
    // 10 m reference grid, labelled in the stored local metres (view framing only).
    const step = span > 150 ? 20 : 10;
    const gridTop = -(maxY + pad), gridBottom = -(minY - pad);
    for (let x = Math.ceil(left / step) * step; x <= left + width; x += step) {
      nodes.push(svg('line', { class: 'insp-grid', x1: num(x), y1: num(gridTop), x2: num(x), y2: num(gridBottom) }));
      nodes.push(svg('text', { class: 'insp-grid-label', x: num(x), y: num(gridBottom + font * 1.1), 'font-size': num(font), 'text-anchor': 'middle' }, num(x)));
    }
    for (let y = Math.ceil((minY - pad) / step) * step; y <= maxY + pad; y += step) {
      nodes.push(svg('line', { class: 'insp-grid', x1: num(left), y1: num(-y), x2: num(left + width), y2: num(-y) }));
      nodes.push(svg('text', { class: 'insp-grid-label', x: num(left + font * 0.3), y: num(-y - font * 0.3), 'font-size': num(font) }, num(y)));
    }
    nodes.push(svg('text', { class: 'insp-axis-label', x: num(left + font * 0.3), y: num(top + font * 1.1), 'font-size': num(font * 1.05) }, '↑ Y north (m)'));
    nodes.push(svg('text', { class: 'insp-axis-label', x: num(left + width - font * 0.3), y: num(top + font * 1.1), 'font-size': num(font * 1.05), 'text-anchor': 'end' }, 'X east (m) →'));
    if (boundary.observed && boundary.verticesLocalM.length >= 3) {
      nodes.push(svg('polygon', { class: 'insp-boundary', points: boundary.verticesLocalM.map((p) => num(p.xM) + ',' + num(-p.yM)).join(' ') }));
    }
    for (const t of trees) {
      const p = t.positionLocalM.value;
      const dead = t.vitalStatus.value === 'Dead';
      const selected = t.sourceTreeId === state.inspection.selectedTreeId;
      const g = svg('g', {
        class: 'insp-tree cat-' + t.speciesCategory + (dead ? ' dead' : '') + (selected ? ' selected' : ''),
        'data-insp-tree': t.sourceTreeId, tabindex: '0', role: 'button', 'data-focus-key': 'insp-map:' + t.sourceTreeId,
        'aria-label': 'Surveyed tree ' + t.sourceTreeId + ', ' + (t.speciesName.value || labelsOf().notRecorded) + ', ' +
          (t.vitalStatus.value ? labelsOf().vitalStatus[t.vitalStatus.value] : labelsOf().notRecorded),
        'aria-pressed': String(selected),
      });
      if (selected) g.append(svg('circle', { class: 'insp-select-ring', cx: num(p.xM), cy: num(-p.yM), r: num(marker * 2.1) }));
      g.append(svg('circle', { class: 'insp-marker', cx: num(p.xM), cy: num(-p.yM), r: num(marker) }));
      g.append(svg('title', null, t.sourceTreeId + ' · ' + (t.speciesName.value || labelsOf().notRecorded)));
      nodes.push(g);
    }
    mapSvg.replaceChildren(...nodes);
  }
  byId('inspection-legend').replaceChildren(...categoryLegendItems(),
    el('li', null, el('span', { class: 'insp-swatch dead', 'aria-hidden': 'true' }), 'Hollow marker: recorded as dead'),
    el('li', null, 'Markers are positions only, not crown sizes (no crown radius was recorded).'));
  byId('inspection-unplotted').textContent = summary.unplottedTreeCount === 0
    ? 'All ' + plural(summary.treeCount, 'surveyed tree has', 'surveyed trees have') + ' a recorded position and ' +
      (summary.treeCount === 1 ? 'is' : 'are') + ' plotted.'
    : summary.plottedTreeCount + ' of ' + summary.treeCount + ' surveyed trees are plotted. ' +
      plural(summary.unplottedTreeCount, 'tree has', 'trees have') + ' no recorded position, so ' +
      (summary.unplottedTreeCount === 1 ? 'it is' : 'they are') + ' not drawn (listed under All surveyed trees).';
}

function fieldValueText(name, field) {
  const labels = labelsOf();
  if (field.value === null || field.value === undefined) return null;
  if (name === 'vitalStatus') return labels.vitalStatus[field.value] || labels.notRecorded;
  if (name === 'positionLocalM') return 'X ' + num(field.value.xM) + ' m east, Y ' + num(field.value.yM) + ' m north';
  if (name === 'speciesName') return field.value;
  const unit = labels.units[name];
  return num(field.value) + (unit ? ' ' + unit : '');
}

function fieldSourceText(field) {
  const labels = labelsOf();
  const parts = [labels.provenance[field.provenance] || labels.notRecorded];
  if (field.sourceColumn) parts.push('source column ' + field.sourceColumn);
  if (field.rawValue !== null && field.rawValue !== undefined) parts.push('recorded as “' + field.rawValue + '”');
  if (field.note && !/^unit=/.test(field.note)) parts.push(field.note);
  return parts.join(' · ');
}

function renderTreeInspector() {
  const box = byId('inspection-tree');
  const tree = state.inspection.selectedTreeId ? treeById(state.inspection.selectedTreeId) : null;
  const labels = labelsOf();
  if (!tree) {
    box.replaceChildren(el('p', { class: 'small-note' }, 'Choose a tree on the map or in the list.'));
    return;
  }
  const rows = TREE_FIELDS.map((name) => {
    const field = tree[name];
    const value = fieldValueText(name, field);
    return el('tr', { 'data-insp-field': name },
      el('th', { scope: 'row' }, labels.fields[name]),
      el('td', { class: value === null ? 'not-recorded' : 'recorded' }, value === null ? labels.notRecorded : value),
      el('td', { class: 'small-note' }, fieldSourceText(field)));
  });
  const origin = state.inspection.view.observedYear0.coordinateOrigin;
  box.replaceChildren(
    el('p', { class: 'insp-tree-title' }, el('strong', null, tree.speciesName.value || ('Species ' + labels.notRecorded.toLowerCase()))),
    el('p', { class: 'small-note' }, 'Survey tree ID ', el('span', { class: 'mono' }, tree.sourceTreeId)),
    el('p', { class: 'small-note' }, labels.speciesCategories[tree.speciesCategory] + ' — ' + labels.speciesCategoryNotes[tree.speciesCategory]),
    el('table', { class: 'insp-field-table' },
      el('thead', null, el('tr', null, el('th', { scope: 'col' }, 'Measurement'), el('th', { scope: 'col' }, 'Value'),
        el('th', { scope: 'col' }, 'Where it comes from'))),
      el('tbody', null, ...rows)),
    el('p', { class: 'small-note' }, '“' + labels.notRecorded + '” means the survey holds no value. CCF never estimates height, age, crown size, position or species.'),
    origin && tree.positionLocalM.value !== null
      ? el('p', { class: 'small-note' }, 'Positions are the recorded coordinates shifted to a local origin for this plot (a translation only: no scaling, rotation or snapping).')
      : null);
}

function renderTreeList() {
  const labels = labelsOf();
  const trees = treesOf();
  const rows = trees.map((t) => {
    const selected = t.sourceTreeId === state.inspection.selectedTreeId;
    const cell = (name) => {
      const value = fieldValueText(name, t[name]);
      return el('td', { class: value === null ? 'not-recorded' : 'num' }, value === null ? labels.notRecorded : value);
    };
    return el('tr', { class: selected ? 'selected' : null },
      el('td', null, el('button', { type: 'button', class: 'small link-like', 'data-insp-select': t.sourceTreeId,
        'data-focus-key': 'insp-list:' + t.sourceTreeId, 'aria-pressed': String(selected) }, t.sourceTreeId)),
      el('td', null, t.speciesName.value || labels.notRecorded),
      el('td', null, t.vitalStatus.value ? labels.vitalStatus[t.vitalStatus.value] : labels.notRecorded),
      cell('dbhCm'),
      cell('heightM'),
      el('td', null, t.positionLocalM.value !== null ? 'Plotted' : 'No position recorded'));
  });
  byId('inspection-tree-list').replaceChildren(el('table', { class: 'insp-list-table' },
    el('caption', { class: 'sr-only' }, 'Every surveyed tree in this plot'),
    el('thead', null, el('tr', null, ...['Tree', 'Species', 'Status', 'DBH', 'Height', 'Map'].map((h) => el('th', { scope: 'col' }, h)))),
    el('tbody', null, ...rows)));
}

function renderSpecies() {
  const labels = labelsOf();
  const breakdown = state.inspection.view.speciesBreakdown;
  const categoryRows = breakdown.categories.map((c) => el('tr', { 'data-insp-category': c.category },
    el('th', { scope: 'row' }, el('span', { class: 'insp-swatch cat-' + c.category, 'aria-hidden': 'true' }), labels.speciesCategories[c.category]),
    el('td', { class: 'num' }, String(c.totalTrees)),
    el('td', { class: 'num' }, String(c.livingTrees)),
    el('td', { class: 'small-note' }, labels.speciesCategoryNotes[c.category])));
  const speciesRows = breakdown.species.map((row) => el('tr', null,
    el('th', { scope: 'row' }, el('em', null, row.rawName)),
    el('td', null, labels.speciesCategories[row.category]),
    el('td', { class: 'num' }, String(row.totalTrees)),
    el('td', { class: 'num' }, String(row.livingTrees))));
  byId('inspection-species').replaceChildren(
    el('p', { class: 'small-note' }, 'How each recorded species name maps onto what CCF can simulate today. This describes CCF’s model coverage, not whether a species suits the site.'),
    el('table', { class: 'insp-species-table' },
      el('thead', null, el('tr', null, el('th', { scope: 'col' }, 'Category'), el('th', { scope: 'col' }, 'Trees'),
        el('th', { scope: 'col' }, 'Living'), el('th', { scope: 'col' }, 'Meaning'))),
      el('tbody', null, ...categoryRows)),
    el('h3', null, 'By recorded species name'),
    el('table', { class: 'insp-species-table' },
      el('thead', null, el('tr', null, el('th', { scope: 'col' }, 'Recorded name'), el('th', { scope: 'col' }, 'Category'),
        el('th', { scope: 'col' }, 'Trees'), el('th', { scope: 'col' }, 'Living'))),
      el('tbody', null, ...speciesRows)),
    el('p', { class: 'small-note' }, 'Trees whose living or dead status was not recognised are counted as living, to be safe.'));
}

function renderMicrohabitats() {
  const records = state.inspection.view.microhabitats;
  const box = byId('inspection-microhabitats');
  if (records.length === 0) {
    box.replaceChildren(el('p', { class: 'small-note' }, 'No tree-related microhabitats were recorded for these trees.'));
    return;
  }
  const treeCount = new Set(records.map((r) => r.sourceTreeId)).size;
  const rows = records.map((r) => {
    const tree = treeById(r.sourceTreeId);
    return el('tr', null,
      el('td', null, el('button', { type: 'button', class: 'small link-like', 'data-insp-select': r.sourceTreeId }, r.sourceTreeId)),
      el('td', null, tree && tree.speciesName.value ? tree.speciesName.value : labelsOf().notRecorded),
      el('td', { class: 'mono' }, r.code),
      el('td', { class: 'num' }, r.value));
  });
  box.replaceChildren(
    el('p', { class: 'small-note' }, plural(records.length, 'record', 'records') + ' on ' + plural(treeCount, 'tree', 'trees') +
      '. Codes are the dataset’s own tree-related microhabitat (TreMs) typology codes, shown exactly as recorded; CCF does not interpret them.'),
    el('div', { class: 'inspection-table-wrap' }, el('table', { class: 'insp-mh-table' },
      el('thead', null, el('tr', null, el('th', { scope: 'col' }, 'Tree'), el('th', { scope: 'col' }, 'Species'),
        el('th', { scope: 'col' }, 'Source code'), el('th', { scope: 'col' }, 'Recorded value'))),
      el('tbody', null, ...rows))));
}

function renderSource() {
  const { entry, view } = state.inspection;
  const dl = el('dl', { class: 'insp-source-list' },
    el('dt', null, 'Dataset'), el('dd', null, el('cite', null, entry.datasetTitle)),
    el('dt', null, 'Publisher'), el('dd', null, entry.publisher),
    el('dt', null, 'Licence'), el('dd', null, link(entry.licenceUrl, entry.licenceSummary),
      ' — Creative Commons Attribution 4.0 International. You may share and adapt this data with attribution.'),
    el('dt', null, 'Citation'), el('dd', null, entry.citation),
    el('dt', null, 'DOI'), el('dd', null, link(entry.doiUrl, entry.doi)),
    el('dt', null, 'GBIF dataset'), el('dd', { class: 'mono' }, entry.datasetKey),
    el('dt', null, 'Survey plot'), el('dd', { class: 'mono' }, entry.plotId),
    el('dt', null, 'Observation date'), el('dd', null, entry.observationDates.length > 0 ? entry.observationDates.join(', ') : labelsOf().notRecorded),
    el('dt', null, 'Retrieved'), el('dd', null, entry.retrievalDateUtc),
    el('dt', null, 'Subset'), el('dd', null, entry.subsetOfGbifRecords),
    el('dt', null, 'Archive fingerprint'), el('dd', { class: 'mono small-note' }, 'SHA-256 ' + entry.archiveSha256));
  byId('inspection-source').replaceChildren(dl,
    el('h3', null, 'Changes made by CCF'),
    el('p', { class: 'small-note' }, entry.modificationsDisclosure),
    el('h3', null, 'Caveats'),
    el('ul', { class: 'insp-caveats' }, ...entry.caveats.map((c) => el('li', null, c))),
    el('h3', null, 'Observed data only'),
    el('p', { class: 'small-note' }, view.provenanceBoundary.statement));
}

// ---- wiring -------------------------------------------------------------------------------------------------------

export function wireInspection() {
  byId('inspect-open').addEventListener('click', openInspectionDialog);
  byId('inspect-cancel').addEventListener('click', () => byId('inspect-dialog').close());
  byId('inspect-load').addEventListener('click', onInspectionLoad);
  byId('inspection-exit').addEventListener('click', onInspectionExit);
  const pick = (event) => {
    const target = event.target.closest('[data-insp-tree], [data-insp-select]');
    if (!target) return;
    selectInspectedTree(target.getAttribute('data-insp-tree') || target.getAttribute('data-insp-select'));
  };
  byId('inspection-map').addEventListener('click', pick);
  byId('inspection-map').addEventListener('keydown', (event) => {
    if (event.key !== 'Enter' && event.key !== ' ') return;
    event.preventDefault();
    pick(event);
  });
  byId('inspection-tree-list').addEventListener('click', pick);
  byId('inspection-microhabitats').addEventListener('click', pick);
}
