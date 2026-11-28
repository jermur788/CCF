/**
 * Browser side of the B01-B20 harness. Loads the real map contract and the adapter, records
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

const host = document.getElementById('map-host');
const pointerEvents = [];
const diagnostics = [];
let adapter = null;
let realGetContext = null;

function create(mode = 'default', extra = {}) {
  if (adapter) adapter.destroy();
  if (realGetContext) { HTMLCanvasElement.prototype.getContext = realGetContext; realGetContext = null; }
  diagnostics.length = 0;
  const options = { exposeDebug: true, onDiagnostic: (e) => diagnostics.push(e), ...extra };
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
  firstFrame: () => firstFrame,
  firstFrameJson: () => firstFrameJson,
  pointerEvents: () => pointerEvents.slice(),
  clearPointerEvents: () => { pointerEvents.length = 0; },
  diagnostics: () => diagnostics.map((d) => ({ kind: d.kind, url: d.url, reason: d.reason, code: d.code })),
  modalCalls: () => modalCalls.slice(),
  openDialogs: () => document.querySelectorAll('dialog[open], [role="alertdialog"]').length,
  storageWrites: () => storageWrites.slice(),
  cspViolations: () => cspViolations.slice(),
  ready: true,
};
