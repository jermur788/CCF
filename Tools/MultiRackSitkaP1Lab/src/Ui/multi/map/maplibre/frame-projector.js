/**
 * Frame normalisation and local-metre geometry helpers for the MapLibre Display Adapter.
 *
 * Presentation only (D-020): no forestry, competition, growth or money maths happens here.
 * Numbers are copied verbatim from the caller's frame. Records that lack the values needed to
 * draw them honestly (for example a tree without a finite positive radiusM) are skipped rather
 * than given invented defaults.
 *
 * Coordinate conventions (same as renderer-svg.js):
 *   world:      xM east, zM north (flat local metres)
 *   area-local: u = xM - extent.minXM (east), v = extent.minZM + extent.sideM - zM (south)
 *   overlay:    X = xM, Y = -zM  (1 SVG user unit = 1 metre, Y grows downwards like the screen)
 */

import { normalizeImageryOpacity, normalizeMapStyle } from './imagery-policy.js';

export const VALID_OVERLAYS = Object.freeze(['off', 'light', 'undergrowth', 'seedlings', 'soil-moisture']);

/** Recursively freeze an object graph in place and return it. */
export function deepFreeze(value, seen = new WeakSet()) {
  if (!value || typeof value !== 'object' || seen.has(value)) return value;
  seen.add(value);
  for (const key of Object.keys(value)) deepFreeze(value[key], seen);
  return Object.freeze(value);
}

const isFiniteNumber = (v) => typeof v === 'number' && Number.isFinite(v);
const nonEmptyString = (v) => typeof v === 'string' && v.trim().length > 0;
const stringOrNull = (v) => (nonEmptyString(v) ? v : null);
const numberOrNull = (v) => (isFiniteNumber(v) ? v : null);

function normalizePolygon(points) {
  if (!Array.isArray(points)) return [];
  return points
    .filter((p) => p && isFiniteNumber(p.xm) && isFiniteNumber(p.zm))
    .map((p) => ({ xm: p.xm, zm: p.zm }));
}

function normalizeGroundCell(cell) {
  if (!cell || !Array.isArray(cell.polygon)) return null;
  const polygon = cell.polygon
    .filter((p) => p && isFiniteNumber(p.u) && isFiniteNumber(p.v))
    .map((p) => ({ u: p.u, v: p.v }));
  if (polygon.length < 3 || !isFiniteNumber(cell.column) || !isFiniteNumber(cell.row)) return null;
  return {
    column: cell.column,
    row: cell.row,
    polygon,
    lightIndex: numberOrNull(cell.lightIndex),
    bramble: numberOrNull(cell.bramble),
    bracken: numberOrNull(cell.bracken),
    seedlingMask: numberOrNull(cell.seedlingMask),
    soilMoistureClass: stringOrNull(cell.soilMoistureClass),
  };
}

function normalizeTree(tree) {
  if (!tree || !nonEmptyString(tree.treeId) || !isFiniteNumber(tree.u) || !isFiniteNumber(tree.v)) return null;
  if (!isFiniteNumber(tree.radiusM) || tree.radiusM <= 0) return null;
  return {
    treeId: tree.treeId,
    u: tree.u,
    v: tree.v,
    radiusM: tree.radiusM,
    species: stringOrNull(tree.species),
    status: nonEmptyString(tree.status) ? tree.status : 'Living',
    isCrop: tree.isCrop === true,
    paintOrder: isFiniteNumber(tree.paintOrder) ? tree.paintOrder : 0,
  };
}

function normalizeDeadwood(item) {
  if (!item || !nonEmptyString(item.deadwoodId) || !isFiniteNumber(item.u) || !isFiniteNumber(item.v)) return null;
  return { deadwoodId: item.deadwoodId, u: item.u, v: item.v };
}

function normalizeJuvenile(item) {
  if (!item || !nonEmptyString(item.juvenileId) || !isFiniteNumber(item.u) || !isFiniteNumber(item.v)) return null;
  return {
    juvenileId: item.juvenileId,
    u: item.u,
    v: item.v,
    speciesId: stringOrNull(item.speciesId),
    isActive: item.isActive !== false,
    hasActiveShelter: item.hasActiveShelter === true,
  };
}

function normalizeRank(item) {
  if (!item || !nonEmptyString(item.treeId) || !isFiniteNumber(item.rank)) return null;
  return { treeId: item.treeId, rank: item.rank, chosen: item.chosen === true };
}

const compact = (list, fn) => (Array.isArray(list) ? list.map(fn).filter(Boolean) : []);

function normalizeArea(area) {
  if (!area || !nonEmptyString(area.areaId)) return null;
  const ext = area.extent;
  if (!ext || !isFiniteNumber(ext.minXM) || !isFiniteNumber(ext.minZM) || !isFiniteNumber(ext.sideM) || ext.sideM <= 0) return null;
  const polygon = normalizePolygon(area.polygon);
  if (polygon.length < 3) return null;
  return {
    areaId: area.areaId,
    label: nonEmptyString(area.label) ? area.label : area.areaId,
    extent: { minXM: ext.minXM, minZM: ext.minZM, sideM: ext.sideM },
    polygon,
    groundCells: compact(area.groundCells, normalizeGroundCell),
    deadwood: compact(area.deadwood, normalizeDeadwood),
    trees: compact(area.trees, normalizeTree).sort((a, b) => a.paintOrder - b.paintOrder),
    juveniles: compact(area.juveniles, normalizeJuvenile),
    ranks: compact(area.ranks, normalizeRank),
  };
}

/** Local-metre bounding box of the areas' polygons, or null when there are none. */
export function areasBounds(areas) {
  const pts = areas.flatMap((a) => a.polygon);
  if (pts.length === 0) return null;
  const xs = pts.map((p) => p.xm);
  const zs = pts.map((p) => p.zm);
  return { minXM: Math.min(...xs), maxXM: Math.max(...xs), minZM: Math.min(...zs), maxZM: Math.max(...zs) };
}

function normalizeImagery(raw) {
  if (!raw || typeof raw !== 'object') return null;
  const b = raw.boundsLocal;
  const boundsLocal = b && [b.minXM, b.minZM, b.maxXM, b.maxZM].every(isFiniteNumber) && b.maxXM > b.minXM && b.maxZM > b.minZM
    ? { minXM: b.minXM, minZM: b.minZM, maxXM: b.maxXM, maxZM: b.maxZM }
    : null;
  return {
    url: typeof raw.url === 'string' ? raw.url : '',
    attribution: typeof raw.attribution === 'string' ? raw.attribution : '',
    boundsLocal,
  };
}

// The anchor fields geo-anchor.js reads. Only these are copied, and only primitive values, so the
// adapter never holds (or freezes) a reference to any object inside the caller's anchor.
const GEO_ANCHOR_FIELDS = Object.freeze([
  'sourceId', 'geometryDigest', 'originKind', 'lonDeg', 'latDeg', 'bearingDeg', 'heightM',
  'localOriginXM', 'localOriginZM', 'isCentroid', 'derivedFromCentroid',
]);

function normalizeGeoAnchor(raw) {
  if (!raw || typeof raw !== 'object') return null;
  const anchor = {};
  for (const key of GEO_ANCHOR_FIELDS) {
    if (!Object.prototype.hasOwnProperty.call(raw, key)) continue;
    const value = raw[key];
    if (value === undefined) continue;
    const primitive = value === null || typeof value === 'string' || typeof value === 'number' || typeof value === 'boolean';
    // A non-primitive becomes null: the validator then rejects it instead of the adapter keeping a reference.
    anchor[key] = primitive ? value : null;
  }
  return anchor;
}

/**
 * Copy, normalise and deep-freeze an incoming frame. Every value is copied into adapter-owned
 * plain data; the caller's objects are never mutated, frozen or referenced.
 * @param {unknown} rawFrame
 */
export function normalizeFrame(rawFrame) {
  if (!rawFrame || typeof rawFrame !== 'object') throw new TypeError('MapFrame must be a non-null object.');

  const areas = compact(rawFrame.areas, normalizeArea);
  const areaIds = areas.map((a) => a.areaId);
  const requestedVisible = Array.isArray(rawFrame.visibleAreaIds) ? rawFrame.visibleAreaIds.filter((id) => areaIds.includes(id)) : [];
  const visibleAreaIds = requestedVisible.length > 0 ? requestedVisible : areaIds;
  const activeAreaId = areaIds.includes(rawFrame.activeAreaId) ? rawFrame.activeAreaId : (visibleAreaIds[0] ?? null);
  const visibleAreas = areas.filter((a) => visibleAreaIds.includes(a.areaId));

  return deepFreeze({
    frameKey: nonEmptyString(rawFrame.frameKey) ? rawFrame.frameKey : null,
    sourceId: nonEmptyString(rawFrame.sourceId) ? rawFrame.sourceId.trim() : null,
    geometryDigest: nonEmptyString(rawFrame.geometryDigest) ? rawFrame.geometryDigest.trim() : null,
    mapStyle: normalizeMapStyle(rawFrame.mapStyle),
    imageryOpacity: normalizeImageryOpacity(rawFrame.imageryOpacity),
    imagery: normalizeImagery(rawFrame.imagery),
    // Supported primitive fields only; geo-anchor.js performs the strict validation before any use.
    geoAnchor: normalizeGeoAnchor(rawFrame.geoAnchor),
    activeAreaId,
    visibleAreaIds,
    overlay: VALID_OVERLAYS.includes(rawFrame.overlay) ? rawFrame.overlay : 'off',
    areas,
    worldBounds: areasBounds(visibleAreas),
  });
}

/** Area-local (u east, v south) -> flat world metres. */
export function areaUvToWorld(extent, u, v) {
  return { xM: extent.minXM + u, zM: extent.minZM + extent.sideM - v };
}

/** Area-local (u, v) -> overlay user units (X = xM, Y = -zM). */
export function areaUvToOverlay(extent, u, v) {
  return { x: extent.minXM + u, y: -(extent.minZM + extent.sideM - v) };
}

function pointInPolygon(xM, zM, polygon) {
  let inside = false;
  for (let i = 0, j = polygon.length - 1; i < polygon.length; j = i++) {
    const a = polygon[i];
    const b = polygon[j];
    if ((a.zm > zM) !== (b.zm > zM) && xM < ((b.xm - a.xm) * (zM - a.zm)) / (b.zm - a.zm) + a.xm) inside = !inside;
  }
  return inside;
}

function onPolygonEdge(xM, zM, polygon, tol = 1e-6) {
  for (let i = 0, j = polygon.length - 1; i < polygon.length; j = i++) {
    const a = polygon[j];
    const b = polygon[i];
    const len = Math.hypot(b.xm - a.xm, b.zm - a.zm);
    if (len === 0) continue;
    const t = ((xM - a.xm) * (b.xm - a.xm) + (zM - a.zm) * (b.zm - a.zm)) / (len * len);
    if (t < 0 || t > 1) continue;
    if (Math.hypot(a.xm + t * (b.xm - a.xm) - xM, a.zm + t * (b.zm - a.zm) - zM) <= tol) return true;
  }
  return false;
}

/**
 * Resolve a world point to the visible area containing it and that area's local (u, v).
 * Outside every visible area it falls back to the active area (same rule as renderer-svg.js
 * resolveMapPoint), so u/v may then lie outside 0..sideM; the host decides what that means.
 */
export function resolveWorldPoint(frame, xM, zM) {
  const visible = frame.areas.filter((a) => frame.visibleAreaIds.includes(a.areaId));
  const hit = visible.find((a) => pointInPolygon(xM, zM, a.polygon) || onPolygonEdge(xM, zM, a.polygon))
    || visible.find((a) => a.areaId === frame.activeAreaId)
    || visible[0];
  if (!hit) return { areaId: null, u: xM, v: zM };
  return { areaId: hit.areaId, u: xM - hit.extent.minXM, v: hit.extent.minZM + hit.extent.sideM - zM };
}

/**
 * Local-metre box to frame: the named area when it is visible, otherwise the whole visible
 * woodland. A hidden area is never framed, so focusing a filtered-out lane cannot push the
 * visible lanes off-centre.
 */
export function focusBox(frame, areaId) {
  const area = areaId && frame.visibleAreaIds.includes(areaId) ? frame.areas.find((a) => a.areaId === areaId) : null;
  return (area && areasBounds([area])) || frame.worldBounds;
}
