/**
 * Builds the SVG content of Layers 3-8 for one frame. Shared by Clean Woodland and Satellite.
 *
 * Every coordinate is written in overlay user units: X = xM, Y = -zM, 1 unit = 1 metre.
 * The adapter places all of it inside one <g transform="matrix(...)"> that maps metres to
 * screen pixels, so a crown <circle r="radiusM"> is a true-metre crown in both styles.
 * Items meant to stay a constant size on screen (badges, juvenile glyphs, rings) are sized
 * with `mpp` (metres per screen pixel). Strokes use vector-effect: non-scaling-stroke (CSS).
 *
 * No imports from the shared /multi shell. Presentation only: values are drawn as given.
 */

import { LAYER_STACK, STAGE2_RESERVED_SLOTS } from './layer-stack.js';
import { areaUvToOverlay } from './frame-projector.js';

const SVG_NS = 'http://www.w3.org/2000/svg';

export function svgEl(tag, attrs = null, text = null) {
  const node = document.createElementNS(SVG_NS, tag);
  if (attrs) {
    for (const [k, v] of Object.entries(attrs)) {
      if (v !== null && v !== undefined) node.setAttribute(k, String(v));
    }
  }
  if (text !== null && text !== undefined) node.textContent = String(text);
  return node;
}

const n4 = (v) => Number(v).toFixed(4);

// Moisture classes that the existing 2D shell already knows (renderer-svg.js ZONE_TINT).
// Any other class is drawn with the neutral style and keeps its class name as data.
const KNOWN_MOISTURE_CLASSES = new Set(['DryFreeDraining', 'FreshMoistMineral']);

/** Create the empty layer groups 3..8 in strict bottom-to-top order (ids from layer-stack.js). */
export function createLayerGroups() {
  return LAYER_STACK.filter((layer) => layer.index >= 3).map((layer) => {
    const g = svgEl('g', { 'data-layer-index': layer.index, 'data-layer-id': layer.id });
    if (layer.reserved) {
      g.setAttribute('data-reserved', 'true');
      for (const slot of STAGE2_RESERVED_SLOTS) g.append(svgEl('g', { 'data-reserved-slot': slot, 'data-implemented': 'false' }));
    }
    return g;
  });
}

function polygonPoints(points) {
  return points.map((p) => n4(p.x) + ',' + n4(p.y)).join(' ');
}

function groundCellNode(frame, area, cell) {
  const base = {
    'data-cell': cell.column + ':' + cell.row,
    'data-area-id': area.areaId,
    points: polygonPoints(cell.polygon.map((p) => areaUvToOverlay(area.extent, p.u, p.v))),
  };
  switch (frame.overlay) {
    case 'soil-moisture':
      if (!cell.soilMoistureClass) return null;
      return svgEl('polygon', {
        ...base,
        class: 'ccf-cell ccf-cell-moisture ccf-moisture-' + (KNOWN_MOISTURE_CLASSES.has(cell.soilMoistureClass) ? cell.soilMoistureClass : 'other'),
        'data-moisture-class': cell.soilMoistureClass,
      });
    case 'light':
      if (cell.lightIndex === null) return null;
      return svgEl('polygon', { ...base, class: 'ccf-cell ccf-cell-light', 'fill-opacity': n4(Math.min(1, Math.max(0, cell.lightIndex)) * 0.6) });
    case 'undergrowth':
      if (!(cell.bramble > 0 || cell.bracken > 0)) return null;
      return svgEl('polygon', { ...base, class: 'ccf-cell ccf-cell-undergrowth', 'fill-opacity': n4(Math.min(1, Math.max(cell.bramble || 0, cell.bracken || 0)) * 0.6) });
    case 'seedlings':
      if (!(cell.seedlingMask > 0)) return null;
      return svgEl('polygon', { ...base, class: 'ccf-cell ccf-cell-seedlings' });
    default:
      return null;
  }
}

function crownNode(area, tree) {
  const c = areaUvToOverlay(area.extent, tree.u, tree.v);
  const status = tree.status;
  const kind = status === 'Felled' ? 'felled' : status === 'NaturalDead' ? 'dead' : 'live';
  const g = svgEl('g', {
    class: 'ccf-tree ccf-tree-' + kind,
    'data-tree-id': tree.treeId,
    'data-area-id': area.areaId,
    'data-species': tree.species,
    'data-status': status,
  });
  // The crown disc: r is the true physical radius in metres, unchanged from the frame.
  g.append(svgEl('circle', {
    class: 'ccf-crown',
    cx: n4(c.x),
    cy: n4(c.y),
    r: tree.radiusM,
    'data-tree-id': tree.treeId,
    'data-radius-m': tree.radiusM,
  }));
  return g;
}

function statusMarks(area, tree, mpp) {
  const c = areaUvToOverlay(area.extent, tree.u, tree.v);
  const nodes = [];
  if (tree.isCrop) {
    const ringR = tree.radiusM + 4 * mpp;
    nodes.push(svgEl('circle', { class: 'ccf-mark-crop-ring', 'data-mark-kind': 'crop', 'data-tree-id': tree.treeId, cx: n4(c.x), cy: n4(c.y), r: n4(ringR) }));
    nodes.push(badge('ccf-mark-badge ccf-mark-badge-crop', 'C', c.x + ringR * 0.72, c.y - ringR * 0.72, mpp, {
      'data-mark-badge': 'C', 'data-tree-id': tree.treeId, 'aria-label': 'Crop tree ' + tree.treeId,
    }));
  }
  if (tree.status === 'PendingFell' || tree.status === 'ApprovedFell') {
    const approved = tree.status === 'ApprovedFell';
    nodes.push(svgEl('circle', {
      class: approved ? 'ccf-mark-approved-ring' : 'ccf-mark-pending-ring',
      'data-mark-kind': approved ? 'approved-fell' : 'pending-fell',
      'data-tree-id': tree.treeId,
      cx: n4(c.x), cy: n4(c.y), r: n4(tree.radiusM + 3 * mpp),
    }));
  }
  return nodes;
}

function badge(className, text, x, y, mpp, attrs) {
  const size = 15 * mpp;
  const g = svgEl('g', { class: className, role: 'img', ...attrs });
  g.append(
    svgEl('rect', { x: n4(x - size / 2), y: n4(y - size / 2), width: n4(size), height: n4(size), rx: n4(3 * mpp) }),
    svgEl('text', { x: n4(x), y: n4(y + 3.6 * mpp), 'font-size': n4(10 * mpp) }, text),
  );
  return g;
}

function juvenileNode(area, juv, mpp) {
  const c = areaUvToOverlay(area.extent, juv.u, juv.v);
  // Same sizing rule as renderer-svg.js planted saplings: at least 0.75 m, otherwise 18 px.
  const half = Math.max(0.75, 18 * mpp) / 2;
  const g = svgEl('g', {
    class: 'ccf-juvenile',
    'data-juvenile-id': juv.juvenileId,
    'data-area-id': area.areaId,
    'data-species': juv.speciesId,
    role: 'img',
    'aria-label': 'Planted sapling ' + juv.juvenileId,
  });
  g.append(
    svgEl('polygon', { class: 'ccf-juvenile-diamond', points: polygonPoints([
      { x: c.x, y: c.y - half }, { x: c.x + half, y: c.y }, { x: c.x, y: c.y + half }, { x: c.x - half, y: c.y }]) }),
    svgEl('circle', { class: 'ccf-juvenile-core', cx: n4(c.x), cy: n4(c.y), r: n4(half * 0.35) }),
  );
  if (juv.hasActiveShelter) {
    g.append(svgEl('circle', { class: 'ccf-juvenile-shelter', cx: n4(c.x), cy: n4(c.y), r: n4(half * 1.3) }));
  }
  return g;
}

function cornerBrackets(cx, cy, pad, arm) {
  let d = '';
  for (const [sx, sy] of [[-1, -1], [1, -1], [1, 1], [-1, 1]]) {
    d += 'M ' + n4(cx + sx * pad) + ' ' + n4(cy + sy * (pad - arm)) + ' L ' + n4(cx + sx * pad) + ' ' + n4(cy + sy * pad)
      + ' L ' + n4(cx + sx * (pad - arm)) + ' ' + n4(cy + sy * pad) + ' ';
  }
  return d;
}

function selectionNode(area, selection, mpp) {
  let centre = null;
  let pad = 0;
  if (selection.kind === 'tree') {
    const tree = area.trees.find((t) => t.treeId === selection.id);
    if (tree) { centre = areaUvToOverlay(area.extent, tree.u, tree.v); pad = tree.radiusM + 6 * mpp; }
  } else if (selection.kind === 'juvenile') {
    const juv = area.juveniles.find((j) => j.juvenileId === selection.id);
    if (juv) { centre = areaUvToOverlay(area.extent, juv.u, juv.v); pad = Math.max(0.75, 18 * mpp) / 2 + 6 * mpp; }
  } else if (selection.kind === 'cell') {
    const cell = area.groundCells.find((c) => c.column + ':' + c.row === selection.id);
    if (cell) {
      const pts = cell.polygon.map((p) => areaUvToOverlay(area.extent, p.u, p.v));
      const xs = pts.map((p) => p.x);
      const ys = pts.map((p) => p.y);
      centre = { x: (Math.min(...xs) + Math.max(...xs)) / 2, y: (Math.min(...ys) + Math.max(...ys)) / 2 };
      pad = Math.max(Math.max(...xs) - Math.min(...xs), Math.max(...ys) - Math.min(...ys)) / 2 + 4 * mpp;
    }
  }
  if (!centre) return null;
  const d = cornerBrackets(centre.x, centre.y, pad, Math.max(6 * mpp, pad * 0.42));
  const g = svgEl('g', {
    class: 'ccf-selection',
    'data-selection-kind': selection.kind,
    'data-selection-id': selection.id,
    'data-area-id': area.areaId,
    role: 'img',
    'aria-label': 'Selected ' + selection.kind + ' ' + selection.id,
  });
  g.append(
    svgEl('path', { class: 'ccf-selection-halo', d }),
    svgEl('path', { class: 'ccf-selection-frame', d }),
    svgEl('rect', { class: 'ccf-selection-tag', x: n4(centre.x - 13 * mpp), y: n4(centre.y - pad - 18 * mpp), width: n4(26 * mpp), height: n4(14 * mpp), rx: n4(2 * mpp) }),
    svgEl('text', { class: 'ccf-selection-tag-text', x: n4(centre.x), y: n4(centre.y - pad - 7.6 * mpp), 'font-size': n4(9.5 * mpp) }, 'SEL'),
  );
  return g;
}

/**
 * Fill layer groups 4..8 for `frame`. Layer 3 (Stage 2 reserved) stays empty by design.
 * @param {Element[]} groups  The six groups from createLayerGroups().
 * @param {object} frame      Normalised, frozen frame.
 * @param {{kind: string, areaId: string|null, id: string}|null} selection
 * @param {number} mpp        Metres per screen pixel at the current view.
 */
export function fillLayerGroups(groups, frame, selection, mpp) {
  const [, ground, deadwood, crowns, juveniles, marks] = groups;
  const groundNodes = [];
  const deadwoodNodes = [];
  const crownNodes = [];
  const juvenileNodes = [];
  const markNodes = [];
  const selectionNodes = [];

  for (const area of frame.areas) {
    if (!frame.visibleAreaIds.includes(area.areaId)) continue;
    const active = area.areaId === frame.activeAreaId;
    groundNodes.push(svgEl('polygon', {
      class: 'ccf-lane ' + (active ? 'ccf-lane-active' : 'ccf-lane-neighbour'),
      'data-lane-border': area.areaId,
      points: polygonPoints(area.polygon.map((p) => ({ x: p.xm, y: -p.zm }))),
    }));
    for (const cell of area.groundCells) {
      const node = groundCellNode(frame, area, cell);
      if (node) groundNodes.push(node);
    }
    // Deadwood is a fixed-size schematic log, as in renderer-svg.js (direction schematic, not simulated).
    for (const dw of area.deadwood) {
      const c = areaUvToOverlay(area.extent, dw.u, dw.v);
      const w = 26 * mpp;
      const h = 9 * mpp;
      const g = svgEl('g', { class: 'ccf-deadwood', 'data-deadwood-id': dw.deadwoodId, 'data-area-id': area.areaId, role: 'img', 'aria-label': 'Fallen log ' + dw.deadwoodId });
      g.append(svgEl('rect', { class: 'ccf-deadwood-log', x: n4(c.x - w / 2), y: n4(c.y - h / 2), width: n4(w), height: n4(h), rx: n4(h / 2) }));
      deadwoodNodes.push(g);
    }
    for (const tree of area.trees) {
      crownNodes.push(crownNode(area, tree));
      markNodes.push(...statusMarks(area, tree, mpp));
    }
    for (const juv of area.juveniles) {
      if (juv.isActive) juvenileNodes.push(juvenileNode(area, juv, mpp));
    }
    for (const entry of area.ranks) {
      const tree = area.trees.find((t) => t.treeId === entry.treeId);
      if (!tree) continue;
      const c = areaUvToOverlay(area.extent, tree.u, tree.v);
      const off = (tree.radiusM + 8 * mpp) * 0.72;
      markNodes.push(badge('ccf-rank-badge' + (entry.chosen ? ' ccf-rank-chosen' : ''), String(entry.rank), c.x - off, c.y - off, mpp, {
        'data-rank': entry.rank, 'data-tree-id': entry.treeId, 'aria-label': 'Competitor rank ' + entry.rank + (entry.chosen ? ', chosen' : ''),
      }));
    }
    if (selection && (!selection.areaId || selection.areaId === area.areaId)) {
      const node = selectionNode(area, selection, mpp);
      if (node) selectionNodes.push(node);
    }
  }

  ground.replaceChildren(...groundNodes);
  deadwood.replaceChildren(...deadwoodNodes);
  crowns.replaceChildren(...crownNodes);
  juveniles.replaceChildren(...juvenileNodes);
  // Selection is drawn last so it sits on top of every other mark.
  marks.replaceChildren(...markNodes, ...selectionNodes);
}
