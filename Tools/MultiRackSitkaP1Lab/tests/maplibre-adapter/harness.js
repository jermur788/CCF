/**
 * Browser side of the B01-B29 harness. Loads the real map contract and the adapter, records
 * pointer callbacks, modal calls, storage writes and CSP violations, and exposes a small
 * control surface on window.__CCF_HARNESS__ for drive-maplibre-cdp.mjs.
 */

import { MAP_RENDERER_METHODS, assertMapRenderer } from '/map/contract.js';
import { createMapLibreAdapter, FALLBACK_COPY, LAYER_STACK } from '/map/maplibre/adapter.js';
import { verifyLayerStackOrder } from '/map/maplibre/layer-stack.js';
import { createFixtureFrame, FIXTURE_FRAME_KEY, IMAGERY_MARGIN_RGB } from '/harness/fixture-80x80.mjs';

const DEBUG = Symbol.for('ccf.maplibre.debug');
const ACTIONS = Symbol.for('ccf.maplibre.debug.actions');

const modalCalls = [];
window.alert = (m) => { modalCalls.push({ kind: 'alert', m: String(m) }); };
window.confirm = (m) => { modalCalls.push({ kind: 'confirm', m: String(m) }); return false; };
window.prompt = (m) => { modalCalls.push({ kind: 'prompt', m: String(m) }); return null; };

const storageWrites = [];
const realSetItem = Storage.prototype.setItem;
Storage.prototype.setItem = function setItem(key, value) {
  storageWrites.push(String(key));
  return realSetItem.call(this, key, value);
};

const cspViolations = [];
document.addEventListener('securitypolicyviolation', (e) => {
  cspViolations.push({ directive: e.violatedDirective, blocked: e.blockedURI, source: e.sourceFile });
});

// B27: hold image decodes (after the image bytes have arrived) until the driver releases them.
// Only decodes of image blobs are held, and only while holding is switched on. A released decode
// runs the real createImageBitmap; the bitmap it produced is kept so the driver can check that the
// adapter closed (discarded) it.
const realCreateImageBitmap = window.createImageBitmap.bind(window);
const heldDecodes = [];
const lateBitmaps = [];
let holdingDecodes = false;
window.createImageBitmap = function createImageBitmap(source, ...rest) {
  if (holdingDecodes && source instanceof Blob && /^image\//.test(source.type)) {
    return new Promise((resolve, reject) => {
      heldDecodes.push(() => realCreateImageBitmap(source, ...rest).then((bitmap) => { lateBitmaps.push(bitmap); resolve(bitmap); }, reject));
    });
  }
  return realCreateImageBitmap(source, ...rest);
};
let decodesHeldTotal = 0;

const host = document.getElementById('map-host');
const pointerEvents = [];
const diagnostics = [];
let adapter = null;
let realGetContext = null;

function create(mode = 'default', extra = {}) {
  if (adapter) adapter.destroy();
  if (realGetContext) { HTMLCanvasElement.prototype.getContext = realGetContext; realGetContext = null; }
  diagnostics.length = 0;
  const options = { exposeDebug: true, onDiagnostic: (e) => diagnostics.push({ ...e, at: performance.now() }), ...extra };
  if (mode === 'module-fail') options.loadMapLibre = async () => { throw new Error('simulated module load failure'); };
  if (mode === 'worker-silent') {
    options.workerUrl = new URL('/harness/fixtures/silent-worker.mjs', window.location.href).href;
    options.loadTimeoutMs = 2500;
  }
  if (mode === 'no-webgl') {
    realGetContext = HTMLCanvasElement.prototype.getContext;
    const original = realGetContext;
    HTMLCanvasElement.prototype.getContext = function getContext(type, ...rest) {
      return /webgl/i.test(String(type)) ? null : original.call(this, type, ...rest);
    };
  }
  if (mode === 'delayed-loader') {
    let release;
    const gate = new Promise((r) => { release = r; });
    options.loadMapLibre = async () => { await gate; return import('/map/maplibre/vendor/maplibre-gl-6.20.0/maplibre-gl.mjs'); };
    window.__CCF_RELEASE_LOADER__ = release;
  }
  adapter = createMapLibreAdapter(options);
  adapter.mount(host, { onPointer: (p) => pointerEvents.push(p) });
  return true;
}

create();
const firstFrame = createFixtureFrame();
const firstFrameJson = JSON.stringify(firstFrame);
adapter.render(firstFrame);

function treeIndex(frame) {
  const index = new Map();
  for (const area of frame.areas) {
    for (const tree of area.trees) {
      index.set(tree.treeId, { tree, xM: area.extent.minXM + tree.u, zM: area.extent.minZM + area.extent.sideM - tree.v });
    }
  }
  return index;
}

/**
 * Satellite crown check. For every drawn crown, take 24 points on its rendered rim (the
 * circle's real screen CTM, i.e. what the browser draws), convert each back to local metres
 * through MapLibre's own unproject + the inverse geo bridge, and compare the distance from the
 * tree centre with radiusM. Also compares the drawn centre with MapLibre's projection.
 */
function measureSatelliteCrowns() {
  const act = adapter[ACTIONS];
  const index = treeIndex(adapter[DEBUG]().frame);
  const stage = document.querySelector('.ccf-ml-stage').getBoundingClientRect();
  let maxErrM = 0;
  let maxCentrePx = 0;
  let rAttrMatches = true;
  const radiusPx = {};
  const circles = Array.from(document.querySelectorAll('[data-layer-index="6"] circle.ccf-crown'));
  for (const circle of circles) {
    const id = circle.getAttribute('data-tree-id');
    const { tree, xM, zM } = index.get(id);
    const r = Number(circle.getAttribute('r'));
    if (r !== tree.radiusM) rAttrMatches = false;
    const cx = Number(circle.getAttribute('cx'));
    const cy = Number(circle.getAttribute('cy'));
    const m = circle.getScreenCTM();
    const toStage = (x, y) => ({ x: m.a * x + m.c * y + m.e - stage.left, y: m.b * x + m.d * y + m.f - stage.top });
    const centre = toStage(cx, cy);
    const projected = act.project(xM, zM);
    maxCentrePx = Math.max(maxCentrePx, Math.hypot(centre.x - projected.x, centre.y - projected.y));
    let sumPx = 0;
    for (let k = 0; k < 24; k++) {
      const t = (2 * Math.PI * k) / 24;
      const rim = toStage(cx + r * Math.cos(t), cy + r * Math.sin(t));
      const local = act.unproject(rim.x, rim.y);
      maxErrM = Math.max(maxErrM, Math.abs(Math.hypot(local.xM - xM, local.zM - zM) - tree.radiusM));
      sumPx += Math.hypot(rim.x - centre.x, rim.y - centre.y);
    }
    radiusPx[id] = sumPx / 24;
  }
  return { count: circles.length, maxErrM, maxCentrePx, rAttrMatches, radiusPx };
}

/**
 * Style-agnostic sanity of the drawn crowns, read from the rendered SVG (what the browser draws):
 * every crown centre and pixel radius is finite and on the stage, and the pixel-per-metre ratio
 * is the same (within 5 %) for crown-to-crown distances and for crown radii. A wrapped longitude
 * or a broken transform shows up as a non-finite value, a crown off-screen or a huge ratio spread.
 */
function crownScreenSanity() {
  const index = treeIndex(adapter[DEBUG]().frame);
  const stage = document.querySelector('.ccf-ml-stage').getBoundingClientRect();
  const circles = Array.from(document.querySelectorAll('[data-layer-index="6"] circle.ccf-crown'));
  const crowns = circles.map((circle) => {
    const id = circle.getAttribute('data-tree-id');
    const { xM, zM, tree } = index.get(id);
    const m = circle.getScreenCTM();
    const cx = Number(circle.getAttribute('cx'));
    const cy = Number(circle.getAttribute('cy'));
    const x = m.a * cx + m.c * cy + m.e - stage.left;
    const y = m.b * cx + m.d * cy + m.f - stage.top;
    const rPx = tree.radiusM * Math.sqrt(Math.abs(m.a * m.d - m.b * m.c));
    return { id, xM, zM, radiusM: tree.radiusM, x, y, rPx };
  });
  const finite = crowns.every((c) => [c.x, c.y, c.rPx].every(Number.isFinite));
  const onStage = crowns.every((c) => c.x >= 0 && c.x <= stage.width && c.y >= 0 && c.y <= stage.height);
  const ratios = [];
  for (let i = 0; i < crowns.length; i++) {
    for (let j = i + 1; j < crowns.length; j++) {
      const dm = Math.hypot(crowns[i].xM - crowns[j].xM, crowns[i].zM - crowns[j].zM);
      if (dm > 1) ratios.push(Math.hypot(crowns[i].x - crowns[j].x, crowns[i].y - crowns[j].y) / dm);
    }
  }
  const radiusRatios = crowns.map((c) => c.rPx / c.radiusM);
  const all = ratios.concat(radiusRatios);
  const maxRatio = all.length ? Math.max(...all) : NaN;
  const minRatio = all.length ? Math.min(...all) : NaN;
  const maxStepPx = ratios.length ? Math.max(...ratios) * 120 : NaN; // largest px for a 120 m step
  return {
    count: crowns.length,
    finite,
    onStage,
    pxPerM: radiusRatios.length ? radiusRatios.reduce((a, b) => a + b, 0) / radiusRatios.length : NaN,
    ratioSpread: maxRatio / minRatio,
    maxStepPx,
    sane: crowns.length > 0 && finite && onStage && Number.isFinite(maxRatio) && maxRatio / minRatio < 1.05,
  };
}

/** Recursively list every object reachable from `value` that is frozen (caller-integrity check). */
function frozenObjectsIn(value, seen = new Set()) {
  if (!value || typeof value !== 'object' || seen.has(value)) return [];
  seen.add(value);
  const found = Object.isFrozen(value) ? [value] : [];
  for (const key of Object.keys(value)) found.push(...frozenObjectsIn(value[key], seen));
  return found;
}

/** Clean Woodland crown check: the transform must be a uniform scale equal to the fit scale. */
function measureCleanWoodlandCrowns() {
  const index = treeIndex(adapter[DEBUG]().frame);
  const stage = document.querySelector('.ccf-ml-stage');
  const expectedScale = Math.min(stage.clientWidth / 88, stage.clientHeight / 88); // 80 m + 2 x 4 m padding
  let maxErrM = 0;
  let isotropic = true;
  let rAttrMatches = true;
  const radiusPx = {};
  const circles = Array.from(document.querySelectorAll('[data-layer-index="6"] circle.ccf-crown'));
  for (const circle of circles) {
    const id = circle.getAttribute('data-tree-id');
    const { tree } = index.get(id);
    const r = Number(circle.getAttribute('r'));
    if (r !== tree.radiusM) rAttrMatches = false;
    const m = circle.getScreenCTM();
    if (Math.abs(m.a - m.d) > 1e-9 || Math.abs(m.b) > 1e-9 || Math.abs(m.c) > 1e-9) isotropic = false;
    radiusPx[id] = r * m.a;
    maxErrM = Math.max(maxErrM, Math.abs(radiusPx[id] / expectedScale - tree.radiusM));
  }
  return { count: circles.length, maxErrM, isotropic, rAttrMatches, expectedScale, radiusPx };
}

window.__CCF_HARNESS__ = {
  newAdapter: (options) => createMapLibreAdapter(options),
  measureSatelliteCrowns,
  measureCleanWoodlandCrowns,
  crownScreenSanity,
  frozenObjectsIn,
  MAP_RENDERER_METHODS,
  LAYER_STACK,
  FALLBACK_COPY,
  FIXTURE_FRAME_KEY,
  IMAGERY_MARGIN_RGB,
  assertMapRenderer,
  verifyLayerStackOrder,
  createFixtureFrame,
  create,
  adapter: () => adapter,
  debug: () => (adapter && adapter[DEBUG] ? adapter[DEBUG]() : null),
  actions: () => (adapter ? adapter[ACTIONS] : null),
  render: (overrides = {}) => { const f = createFixtureFrame(overrides); adapter.render(f); return true; },
  /** destroy() then mount() on the SAME adapter instance (B26 remount). */
  remountSame: () => { adapter.destroy(); adapter.mount(host, { onPointer: (p) => pointerEvents.push(p) }); return true; },
  firstFrame: () => firstFrame,
  firstFrameJson: () => firstFrameJson,
  pointerEvents: () => pointerEvents.slice(),
  clearPointerEvents: () => { pointerEvents.length = 0; },
  diagnostics: () => diagnostics.map((d) => ({ kind: d.kind, url: d.url, reason: d.reason, code: d.code, at: d.at })),
  constructedCount: () => diagnostics.filter((d) => d.kind === 'map-constructed').length,
  modalCalls: () => modalCalls.slice(),
  openDialogs: () => document.querySelectorAll('dialog[open], [role="alertdialog"]').length,
  storageWrites: () => storageWrites.slice(),
  cspViolations: () => cspViolations.slice(),
  holdDecodes: (on) => { holdingDecodes = Boolean(on); return holdingDecodes; },
  heldDecodeCount: () => heldDecodes.length + decodesHeldTotal,
  releaseDecodes: () => { const n = heldDecodes.length; decodesHeldTotal += n; heldDecodes.splice(0).forEach((run) => run()); return n; },
  /** Bitmaps produced by released decodes: [width, height]; a closed ImageBitmap reports 0 x 0. */
  lateBitmapSizes: () => lateBitmaps.map((b) => [b.width, b.height]),
  ready: true,
};
