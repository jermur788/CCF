/**
 * MapLibre Display Adapter (SPEC-multi-2d-maplibre-adapter-V1, Part 1).
 *
 * A map renderer with exactly the five MAP_RENDERER_METHODS (mount, render, setSelection,
 * focusArea, destroy). It draws the frame it is given and computes no forestry (D-020).
 *
 * Two styles, chosen per frame with frame.mapStyle:
 *   'clean-woodland' (default) - plain #15251a canvas + SVG overlay. Never loads MapLibre.
 *   'satellite'                - MapLibre GL canvas (background + optional local imagery) under
 *                                the same SVG overlay. MapLibre is imported lazily, only when a
 *                                satellite frame arrives with a valid geo-anchor.
 *
 * Layers 3-8 are one SVG group tree in metre units. A single affine transform maps metres to
 * screen pixels: in Clean Woodland a uniform north-up scale, in Satellite the local derivative
 * of (local metres -> WGS84 -> MapLibre screen). Crowns therefore keep their true radiusM in
 * both styles and are never fixed-pixel circles.
 *
 * Fallbacks never block, never use alert/confirm/prompt and keep the current selection.
 */

import { checkWebMercatorFootprint, createGeoAnchorBridge, validateGeoAnchor } from './geo-anchor.js';
import { LAYER_STACK } from './layer-stack.js';
import {
  DEFAULT_CANVAS_COLOR,
  DEFAULT_IMAGERY_OPACITY,
  FALLBACK_CODES,
  FALLBACK_COPY,
  quadIntersectsViewport,
  shouldShowAttribution,
  validateImagerySource,
} from './imagery-policy.js';
import { areasBounds, focusBox, normalizeFrame, resolveWorldPoint, areaUvToWorld } from './frame-projector.js';
import { createLayerGroups, fillLayerGroups, svgEl } from './overlay-layers.js';
import { cleanWoodlandMatrix } from './clean-woodland.js';

const VENDOR_DIR = './vendor/maplibre-gl-6.20.0/';
const MAPLIBRE_MODULE_URL = new URL(VENDOR_DIR + 'maplibre-gl.mjs', import.meta.url).href;
const MAPLIBRE_WORKER_URL = new URL(VENDOR_DIR + 'maplibre-gl-worker.mjs', import.meta.url).href;
const MAPLIBRE_CSS_URL = new URL(VENDOR_DIR + 'maplibre-gl.css', import.meta.url).href;

const IMAGERY_SOURCE_ID = 'ccf-local-imagery';
const IMAGERY_LAYER_ID = 'ccf-georeferenced-imagery';
const CANVAS_LAYER_ID = 'ccf-woodland-canvas';
// Part of Layer 1: the visible lane polygons filled with the same canvas colour. It is invisible
// against the background, but GeoJSON is tiled by the MapLibre worker, so this source loading
// proves the worker is alive. Without it a dead worker would go unnoticed.
const EXTENT_SOURCE_ID = 'ccf-woodland-extent';
const EXTENT_LAYER_ID = 'ccf-woodland-extent';
const CLICK_SLOP_PX = 5;
const FIT_PADDING_PX = 24;
const DEFAULT_LOAD_TIMEOUT_MS = 15000;
const DEFAULT_IMAGERY_TIMEOUT_MS = 8000;

function htmlEl(tag, attrs = null) {
  const node = document.createElement(tag);
  if (attrs) for (const [k, v] of Object.entries(attrs)) if (v !== null && v !== undefined) node.setAttribute(k, String(v));
  return node;
}

function webglAvailable() {
  try {
    const canvas = document.createElement('canvas');
    const gl = canvas.getContext('webgl2') || canvas.getContext('webgl');
    if (!gl) return false;
    const lose = gl.getExtension('WEBGL_lose_context');
    if (lose) lose.loseContext();
    return true;
  } catch {
    return false;
  }
}

/**
 * Fetch the imagery once and make sure the browser can decode it as an image.
 *
 * The whole probe (fetch, body read and decode) is bounded by one deadline: after timeoutMs the
 * probe settles to false whatever step it is in, and the request is aborted. A step that finishes
 * later (for example a decode the browser releases after the deadline) cannot change the answer;
 * its bitmap is closed and discarded. The controller lets the adapter abort early (display
 * failure, destroy), which also settles the probe to false.
 * @returns {{ promise: Promise<boolean>, controller: AbortController, settled: boolean, result: boolean|null,
 *   reason: 'ok'|'timeout'|'aborted'|'not-image'|'error'|null }}
 */
function probeImagery(url, timeoutMs) {
  const controller = new AbortController();
  const probe = { controller, settled: false, result: null, reason: null, promise: null };
  let resolveProbe;
  probe.promise = new Promise((resolve) => { resolveProbe = resolve; });
  let timer = null;
  const settle = (ok, reason) => {
    if (probe.settled) return false;
    probe.settled = true;
    probe.result = ok;
    probe.reason = reason;
    clearTimeout(timer);
    resolveProbe(ok);
    return true;
  };
  timer = setTimeout(() => { if (settle(false, 'timeout')) controller.abort('timeout'); }, timeoutMs);
  controller.signal.addEventListener('abort', () => settle(false, 'aborted'), { once: true });
  (async () => {
    let bitmap = null;
    try {
      const res = await fetch(url, { cache: 'no-store', credentials: 'same-origin', signal: controller.signal });
      if (!res.ok || !(res.headers.get('content-type') || '').startsWith('image/')) { settle(false, 'not-image'); return; }
      const blob = await res.blob();
      if (probe.settled) return;
      bitmap = await createImageBitmap(blob);
      settle(true, 'ok'); // a no-op when the deadline (or an abort) already settled the probe
    } catch {
      settle(false, 'error');
    } finally {
      if (bitmap) bitmap.close();
    }
  })();
  return probe;
}

function ensureMapLibreStylesheet() {
  if (document.head.querySelector('link[data-ccf-maplibre-css]')) return;
  document.head.append(htmlEl('link', { rel: 'stylesheet', href: MAPLIBRE_CSS_URL, 'data-ccf-maplibre-css': '6.20.0' }));
}

const matrixScale = (m) => Math.sqrt(Math.abs(m.a * m.d - m.b * m.c));
const isUsableMatrix = (m) => Boolean(m) && [m.a, m.b, m.c, m.d, m.e, m.f].every(Number.isFinite) && matrixScale(m) > 0;

/**
 * Create a MapLibre display adapter.
 *
 * Options exist for tests and diagnostics; production callers pass nothing.
 * @param {{
 *   loadMapLibre?: () => Promise<object>,  // replaces the lazy import (fault injection)
 *   workerUrl?: string,                    // replaces the vendored worker URL (fault injection)
 *   loadTimeoutMs?: number,                // readiness watchdog (module + style + worker); default 15 s
 *   imageryTimeoutMs?: number,             // imagery probe limit; default min(8 s, loadTimeoutMs / 2)
 *   onDiagnostic?: (event: object) => void,
 *   exposeDebug?: boolean,                 // adds a non-enumerable Symbol.for('ccf.maplibre.debug') handle
 * }} [options]
 */
export function createMapLibreAdapter(options = {}) {
  const loadMapLibre = options.loadMapLibre || (() => import(MAPLIBRE_MODULE_URL));
  const workerUrl = options.workerUrl || MAPLIBRE_WORKER_URL;
  const loadTimeoutMs = Number.isFinite(options.loadTimeoutMs) ? options.loadTimeoutMs : DEFAULT_LOAD_TIMEOUT_MS;
  const imageryTimeoutMs = Number.isFinite(options.imageryTimeoutMs)
    ? options.imageryTimeoutMs
    : Math.min(DEFAULT_IMAGERY_TIMEOUT_MS, loadTimeoutMs / 2);
  const onDiagnostic = typeof options.onDiagnostic === 'function' ? options.onDiagnostic : null;

  let mounted = false;
  let destroyed = false;
  let generation = 0; // bumped by render/destroy/display failure; async work started for an older value is dropped

  let deps = {};
  let host = null;
  let rootEl = null;
  let stageEl = null;
  let canvasEl = null;
  let imageryStateEl = null;
  let overlayEl = null;
  let worldEl = null;
  let layerGroups = [];
  let noticeEl = null;
  let attributionEl = null;
  let resizeObserver = null;
  let pointerDown = null;

  let frame = null;
  let selection = null;
  let focusedAreaId = null;

  let activeStyle = 'clean-woodland';
  let fallbackCode = FALLBACK_CODES.NONE;
  let displayFailed = false; // a WebGL/module/worker failure keeps this adapter on Clean Woodland for its lifetime

  let modulePromise = null;
  let mapModule = null;
  let map = null;
  let mapLoaded = false; // MapLibre 'load' fired: the style accepts sources/layers
  let mapReady = false;  // the worker-tiled extent source has loaded: the display really works
  let bridge = null;
  let anchorKey = null;
  let loadWatchdog = null; // bounds the whole satellite start: module import, style and worker
  let wantedImagery = null; // { url, attribution, boundsLocal } once probed, else null
  let imageryLoaded = false;
  const probeCache = new Map(); // url -> probe from probeImagery()

  let matrix = null;
  let built = { frame: null, selection: null, mpp: 0 };

  function diag(kind, detail = {}) {
    if (!onDiagnostic) return;
    try { onDiagnostic({ kind, generation, activeStyle, fallbackCode, ...detail }); } catch { /* never disturb rendering */ }
  }

  function stageSize() {
    return { width: Math.max(1, stageEl.clientWidth), height: Math.max(1, stageEl.clientHeight) };
  }

  // ---- status line: style, fallback notice, Layer 2 state and attribution ------------------

  function updateStatus() {
    if (!rootEl) return;
    const opacity = frame ? frame.imageryOpacity : DEFAULT_IMAGERY_OPACITY;
    const attribution = wantedImagery ? wantedImagery.attribution : '';
    const imageryVisible = shouldShowAttribution({ activeStyle, imageryLoaded, imageryOpacity: opacity, imageryInView: imageryInView(), attribution });
    rootEl.setAttribute('data-map-style', activeStyle);
    rootEl.setAttribute('data-fallback', fallbackCode);
    rootEl.setAttribute('data-frame-key', frame && frame.frameKey ? frame.frameKey : '');
    const copy = FALLBACK_COPY[fallbackCode];
    noticeEl.textContent = copy;
    noticeEl.hidden = copy.length === 0;
    imageryStateEl.setAttribute('data-imagery-visible', String(imageryVisible));
    imageryStateEl.setAttribute('data-imagery-opacity', String(opacity));
    attributionEl.textContent = imageryVisible ? attribution : '';
    attributionEl.hidden = !imageryVisible;
  }

  /** Does the imagery footprint, as MapLibre draws it now, overlap the visible stage? */
  function imageryInView() {
    if (!map || !bridge || !wantedImagery || activeStyle !== 'satellite') return false;
    try {
      const quad = imageryQuad(wantedImagery.boundsLocal).map((lngLat) => map.project(lngLat));
      const { width, height } = stageSize();
      return quadIntersectsViewport(quad, width, height);
    } catch {
      return false;
    }
  }

  // ---- overlay (Layers 3-8) -------------------------------------------------------------------

  /**
   * Remove every drawn mark and the hit-test transform. Used whenever the current frame has
   * nothing drawable, so nothing from a previous frame stays on screen or answers clicks.
   * Layer 3 keeps its (empty) reserved slots.
   */
  function clearOverlay() {
    matrix = null;
    built = { frame: null, selection: null, mpp: 0 };
    if (!worldEl) return;
    worldEl.removeAttribute('transform');
    for (const group of layerGroups.slice(1)) group.replaceChildren();
  }

  function drawOverlay(nextMatrix) {
    if (!worldEl) return;
    if (!frame || !frame.worldBounds || !isUsableMatrix(nextMatrix)) { clearOverlay(); return; }
    matrix = nextMatrix;
    worldEl.setAttribute('transform', 'matrix(' + [matrix.a, matrix.b, matrix.c, matrix.d, matrix.e, matrix.f].join(' ') + ')');
    const mpp = 1 / matrixScale(matrix);
    // Panning only moves the transform; contents are rebuilt when data changes or the zoom
    // changes enough that constant-pixel marks (badges, glyphs) would visibly drift in size.
    const zoomChanged = !built.mpp || Math.abs(Math.log(mpp / built.mpp)) > 0.01;
    if (built.frame !== frame || built.selection !== selection || zoomChanged) {
      fillLayerGroups(layerGroups, frame, selection, mpp);
      built = { frame, selection, mpp };
    }
  }

  function satelliteMatrix() {
    const wb = frame.worldBounds;
    const x0 = (wb.minXM + wb.maxXM) / 2;
    const z0 = (wb.minZM + wb.maxZM) / 2;
    const h = 10; // central differences over +-10 m; overlay units are X = xM, Y = -zM
    const at = (x, z) => map.project(bridge.localToWgs84(x, z));
    const p0 = at(x0, z0);
    const xp = at(x0 + h, z0);
    const xn = at(x0 - h, z0);
    const yp = at(x0, z0 - h);
    const yn = at(x0, z0 + h);
    const a = (xp.x - xn.x) / (2 * h);
    const b = (xp.y - xn.y) / (2 * h);
    const c = (yp.x - yn.x) / (2 * h);
    const d = (yp.y - yn.y) / (2 * h);
    return { a, b, c, d, e: p0.x - a * x0 - c * -z0, f: p0.y - b * x0 - d * -z0 };
  }

  function syncSatelliteOverlay() {
    if (!map || !bridge || !frame || activeStyle !== 'satellite' || !frame.worldBounds) return;
    drawOverlay(satelliteMatrix());
    updateStatus();
  }

  /** Redraw the overlay with the Clean Woodland transform (or clear it when nothing is drawable). */
  function redrawCleanWoodland() {
    const { width, height } = stageSize();
    drawOverlay(frame ? cleanWoodlandMatrix(frame, focusedAreaId, width, height) : null);
    updateStatus();
  }

  function showCleanWoodland(code) {
    clearWatchdog();
    teardownMap();
    activeStyle = 'clean-woodland';
    fallbackCode = code;
    redrawCleanWoodland();
    diag('clean-woodland', { code });
  }

  // ---- MapLibre lifecycle -----------------------------------------------------------------------

  function armWatchdog() {
    if (loadWatchdog || mapReady) return;
    loadWatchdog = setTimeout(() => {
      loadWatchdog = null;
      if (destroyed || displayFailed || mapReady || !frame || frame.mapStyle !== 'satellite') return;
      failDisplay('not ready in time (module, style or worker did not load)');
    }, loadTimeoutMs);
  }

  function clearWatchdog() {
    if (loadWatchdog) { clearTimeout(loadWatchdog); loadWatchdog = null; }
  }

  /** Abort imagery probes still in flight and forget them, so none can resolve into a later state. */
  function abortPendingProbes(reason) {
    for (const [url, probe] of probeCache) {
      if (probe.settled) continue;
      probe.controller.abort(reason);
      probeCache.delete(url);
    }
  }

  function teardownMap() {
    if (map) {
      const old = map;
      map = null;
      try { old.remove(); } catch { /* already gone */ }
    }
    mapLoaded = false;
    mapReady = false;
    bridge = null;
    anchorKey = null;
    imageryLoaded = false;
    if (canvasEl) canvasEl.replaceChildren();
  }

  function failDisplay(reason) {
    displayFailed = true;
    // Invalidate every pending async step (module import, imagery probe) and abort the probes:
    // the fallback is sticky, so nothing that resolves later may rebuild Satellite.
    generation += 1;
    abortPendingProbes('display-unavailable');
    diag('display-unavailable', { reason });
    showCleanWoodland(FALLBACK_CODES.DISPLAY_UNAVAILABLE);
  }

  function loadModuleOnce() {
    if (!modulePromise) {
      modulePromise = (async () => {
        const mod = await loadMapLibre();
        if (!mod || typeof mod.Map !== 'function' || typeof mod.setWorkerUrl !== 'function') {
          throw new Error('Vendored MapLibre module is missing Map or setWorkerUrl.');
        }
        // Must happen before any new Map(...): a same-origin module worker, never a blob worker.
        mod.setWorkerUrl(workerUrl);
        diag('worker-url-set', { url: workerUrl });
        return mod;
      })();
      modulePromise.catch(() => { modulePromise = null; });
    }
    return modulePromise;
  }

  /**
   * Decide, synchronously and before the footprint preflight, whether this frame's imagery can be
   * used at all: it must pass the imagery policy and its own footprint must fit the one longitude
   * branch. Imagery that fails is dropped (imagery-unavailable); it never disqualifies the stand.
   */
  function imageryEligibility(currentFrame, geo) {
    const check = validateImagerySource(currentFrame.imagery, window.location.href);
    if (!check.allowed) return check;
    const fit = checkWebMercatorFootprint(geo, [check.boundsLocal]);
    if (!fit.ok) return { allowed: false, reason: 'Imagery footprint unsupported: ' + fit.reason, code: fit.code };
    return check;
  }

  async function resolveImagery(check) {
    if (!check.allowed) {
      diag('imagery-not-used', { reason: check.reason, code: check.code });
      return null;
    }
    if (!probeCache.has(check.url)) {
      probeCache.set(check.url, probeImagery(check.url, imageryTimeoutMs));
      diag('imagery-probe-started', { url: check.url });
    }
    const probe = probeCache.get(check.url);
    const ok = await probe.promise;
    if (!ok) diag('imagery-probe-failed', { url: check.url, reason: probe.reason });
    return ok ? { url: check.url, attribution: check.attribution, boundsLocal: check.boundsLocal } : null;
  }

  function imageryQuad(boundsLocal) {
    const b = boundsLocal;
    // MapLibre image corners: top-left, top-right, bottom-right, bottom-left (image top = local north).
    return [
      bridge.localToWgs84(b.minXM, b.maxZM),
      bridge.localToWgs84(b.maxXM, b.maxZM),
      bridge.localToWgs84(b.maxXM, b.minZM),
      bridge.localToWgs84(b.minXM, b.minZM),
    ];
  }

  function extentGeoJson(currentFrame, geo) {
    return {
      type: 'FeatureCollection',
      features: currentFrame.areas.filter((a) => currentFrame.visibleAreaIds.includes(a.areaId)).map((a) => {
        const ring = a.polygon.map((p) => geo.localToWgs84(p.xm, p.zm));
        ring.push(ring[0]);
        return { type: 'Feature', properties: { areaId: a.areaId }, geometry: { type: 'Polygon', coordinates: [ring] } };
      }),
    };
  }

  function checkReady() {
    if (!map || mapReady || !mapLoaded) return;
    let loaded = false;
    try { loaded = map.isSourceLoaded(EXTENT_SOURCE_ID); } catch { loaded = false; }
    if (!loaded) return;
    mapReady = true;
    clearWatchdog();
    diag('map-ready');
  }

  /** Bring the MapLibre imagery source/layer in line with wantedImagery and the frame opacity. */
  function applyImagery() {
    if (!map || !mapLoaded) return;
    const source = map.getSource(IMAGERY_SOURCE_ID);
    if (!wantedImagery) {
      if (map.getLayer(IMAGERY_LAYER_ID)) map.removeLayer(IMAGERY_LAYER_ID);
      if (source) map.removeSource(IMAGERY_SOURCE_ID);
      imageryLoaded = false;
      return;
    }
    const coordinates = imageryQuad(wantedImagery.boundsLocal);
    if (!source) {
      map.addSource(IMAGERY_SOURCE_ID, { type: 'image', url: wantedImagery.url, coordinates });
      map.addLayer({
        id: IMAGERY_LAYER_ID,
        type: 'raster',
        source: IMAGERY_SOURCE_ID,
        paint: { 'raster-opacity': frame.imageryOpacity, 'raster-fade-duration': 0 },
      });
      imageryLoaded = false;
    } else if (source.url !== wantedImagery.url) {
      imageryLoaded = false;
      source.updateImage({ url: wantedImagery.url, coordinates });
    } else {
      source.setCoordinates(coordinates);
    }
    map.setPaintProperty(IMAGERY_LAYER_ID, 'raster-opacity', frame.imageryOpacity);
    checkImageryLoaded();
  }

  function checkImageryLoaded() {
    if (!map || !wantedImagery || !map.getSource(IMAGERY_SOURCE_ID)) return;
    let loaded = false;
    try { loaded = map.isSourceLoaded(IMAGERY_SOURCE_ID); } catch { loaded = false; }
    if (loaded !== imageryLoaded) {
      imageryLoaded = loaded;
      updateStatus();
    }
  }

  function imageryFailed(reason) {
    diag('imagery-failed', { reason });
    wantedImagery = null;
    fallbackCode = FALLBACK_CODES.IMAGERY_UNAVAILABLE;
    applyImagery();
    updateStatus();
  }

  /** Place the camera so the focus box fills the stage, rotated so the woodland stays upright. */
  function fitSatelliteCamera() {
    if (!map || !bridge || !frame) return;
    const box = focusBox(frame, focusedAreaId);
    if (!box) return;
    const { width, height } = stageSize();
    map.jumpTo({ center: bridge.localToWgs84((box.minXM + box.maxXM) / 2, (box.minZM + box.maxZM) / 2), bearing: bridge.anchor.bearingDeg });
    const target = Math.min((width - 2 * FIT_PADDING_PX) / Math.max(1, box.maxXM - box.minXM),
      (height - 2 * FIT_PADDING_PX) / Math.max(1, box.maxZM - box.minZM));
    for (let i = 0; i < 2; i++) {
      const current = matrixScale(satelliteMatrix());
      map.jumpTo({ zoom: map.getZoom() + Math.log2(Math.max(1e-6, target) / current) });
    }
    syncSatelliteOverlay();
  }

  function createMap(mod, currentFrame) {
    const box = focusBox(currentFrame, focusedAreaId);
    let created;
    try {
      created = new mod.Map({
        container: canvasEl,
        // Inline style only: no glyphs, no sprite, no remote sources. Text lives in the SVG overlay.
        style: {
          version: 8,
          sources: { [EXTENT_SOURCE_ID]: { type: 'geojson', data: extentGeoJson(currentFrame, bridge) } },
          layers: [
            { id: CANVAS_LAYER_ID, type: 'background', paint: { 'background-color': DEFAULT_CANVAS_COLOR } },
            { id: EXTENT_LAYER_ID, type: 'fill', source: EXTENT_SOURCE_ID, paint: { 'fill-color': DEFAULT_CANVAS_COLOR, 'fill-antialias': false } },
          ],
        },
        center: bridge.localToWgs84((box.minXM + box.maxXM) / 2, (box.minZM + box.maxZM) / 2),
        zoom: 17,
        bearing: bridge.anchor.bearingDeg,
        pitch: 0,
        maxPitch: 0,
        maxZoom: 23,
        attributionControl: false,
        dragRotate: false,
        pitchWithRotate: false,
        touchPitch: false,
        renderWorldCopies: false,
        fadeDuration: 0,
      });
    } catch (err) {
      failDisplay('map constructor: ' + (err && err.message ? err.message : String(err)));
      return;
    }
    diag('map-constructed');
    map = created;
    mapLoaded = false;
    created.touchZoomRotate.disableRotation();
    created.keyboard.disableRotation();

    const live = () => map === created && !destroyed;
    created.on('move', () => { if (live()) syncSatelliteOverlay(); });
    created.on('resize', () => { if (live()) syncSatelliteOverlay(); });
    created.on('load', () => {
      if (!live()) return;
      mapLoaded = true;
      applyImagery();
      checkReady();
      syncSatelliteOverlay();
      diag('map-loaded');
    });
    created.on('sourcedata', (e) => {
      if (!live()) return;
      if (e.sourceId === IMAGERY_SOURCE_ID) checkImageryLoaded();
      if (e.sourceId === EXTENT_SOURCE_ID) checkReady();
    });
    created.on('idle', () => { if (live()) { checkReady(); checkImageryLoaded(); } });
    created.on('error', (e) => {
      if (!live()) return;
      const message = String((e && e.error && e.error.message) || 'MapLibre error');
      if (e && e.sourceId === IMAGERY_SOURCE_ID) imageryFailed(message);
      else if (!mapReady) failDisplay(message);
      else diag('map-error', { message });
    });
    created.on('webglcontextlost', () => { if (live()) failDisplay('webglcontextlost'); });

    fitSatelliteCamera();
  }

  /**
   * Bring up (or update) Satellite for one frame. The readiness watchdog is armed first, so a
   * stuck module import, style or worker always ends in the display fallback. The imagery probe
   * runs alongside and never delays the map; it is time-limited and aborted on display failure.
   */
  async function startSatellite(currentFrame, nextBridge, myGeneration, imageryCheck) {
    const stale = () => myGeneration !== generation || destroyed || displayFailed;
    armWatchdog();
    const imageryPromise = resolveImagery(imageryCheck);
    let mod;
    try {
      mod = await loadModuleOnce();
    } catch (err) {
      if (!stale()) failDisplay('module: ' + (err && err.message ? err.message : String(err)));
      return;
    }
    if (stale()) return;

    mapModule = mod;
    if (activeStyle !== 'satellite') fallbackCode = FALLBACK_CODES.NONE;
    activeStyle = 'satellite';
    const nextKey = JSON.stringify(nextBridge.anchor);
    if (map && nextKey === anchorKey) {
      bridge = nextBridge;
      const extent = mapLoaded ? map.getSource(EXTENT_SOURCE_ID) : null;
      if (extent) extent.setData(extentGeoJson(currentFrame, bridge));
      applyImagery();
      syncSatelliteOverlay();
    } else {
      teardownMap();
      wantedImagery = null;
      bridge = nextBridge;
      anchorKey = nextKey;
      armWatchdog();
      updateStatus();
      createMap(mod, currentFrame);
      if (!map) return; // the constructor failed and the display fallback is already showing
    }

    const imagery = await imageryPromise;
    if (stale()) return;
    wantedImagery = imagery;
    fallbackCode = imagery ? FALLBACK_CODES.NONE : FALLBACK_CODES.IMAGERY_UNAVAILABLE;
    applyImagery();
    updateStatus();
  }

  // ---- pointer --------------------------------------------------------------------------------

  function onPointerDown(event) {
    pointerDown = event.button === 0 ? { x: event.clientX, y: event.clientY } : null;
  }

  function onPointerUp(event) {
    const start = pointerDown;
    pointerDown = null;
    if (!start || event.button !== 0 || !frame || !matrix || typeof deps.onPointer !== 'function') return;
    if (Math.hypot(event.clientX - start.x, event.clientY - start.y) > CLICK_SLOP_PX) return; // a drag/pan
    const rect = stageEl.getBoundingClientRect();
    const sx = event.clientX - rect.left - matrix.e;
    const sy = event.clientY - rect.top - matrix.f;
    const det = matrix.a * matrix.d - matrix.b * matrix.c;
    const X = (matrix.d * sx - matrix.c * sy) / det;
    const Y = (-matrix.b * sx + matrix.a * sy) / det;
    const xM = X;
    const zM = -Y;

    // Planted juveniles are small, so they win within their glyph (or 12 px, whichever is larger).
    const mpp = 1 / matrixScale(matrix);
    const reach = Math.max(Math.max(0.75, 18 * mpp) / 2, 12 * mpp);
    let best = null;
    for (const area of frame.areas) {
      if (!frame.visibleAreaIds.includes(area.areaId)) continue;
      for (const juv of area.juveniles) {
        if (!juv.isActive) continue;
        const w = areaUvToWorld(area.extent, juv.u, juv.v);
        const dist = Math.hypot(w.xM - xM, w.zM - zM);
        if (dist <= reach && (!best || dist < best.dist)) best = { dist, area, juv };
      }
    }
    const payload = best
      ? { areaId: best.area.areaId, u: best.juv.u, v: best.juv.v, juvenileId: best.juv.juvenileId, frameKey: frame.frameKey }
      : { ...resolveWorldPoint(frame, xM, zM), juvenileId: null, frameKey: frame.frameKey };
    deps.onPointer({ areaId: payload.areaId, u: payload.u, v: payload.v, juvenileId: payload.juvenileId, frameKey: payload.frameKey });
  }

  function onResize() {
    if (!mounted || destroyed || !frame) return;
    if (activeStyle === 'satellite' && map) {
      map.resize();
      syncSatelliteOverlay();
    } else {
      redrawCleanWoodland();
    }
  }

  // ---- the five contract methods ----------------------------------------------------------------

  function mount(container, nextDeps) {
    if (mounted) { // idempotent: a second mount only refreshes the callbacks
      deps = { ...(nextDeps || {}) };
      return;
    }
    if (!container || typeof container.replaceChildren !== 'function') throw new TypeError('mount(container, deps) needs a DOM element.');
    mounted = true;
    destroyed = false;
    host = container;
    deps = { ...(nextDeps || {}) };

    rootEl = htmlEl('div', { class: 'ccf-ml-root', 'data-map-style': 'clean-woodland', 'data-fallback': FALLBACK_CODES.NONE });
    stageEl = htmlEl('div', { class: 'ccf-ml-stage' });
    canvasEl = htmlEl('div', { class: 'ccf-ml-canvas', 'data-layer-index': 1, 'data-layer-id': 'woodland-canvas' });
    imageryStateEl = htmlEl('div', { class: 'ccf-ml-imagery-state', 'data-layer-index': 2, 'data-layer-id': 'georeferenced-imagery', 'aria-hidden': 'true' });
    overlayEl = svgEl('svg', { class: 'ccf-ml-overlay', role: 'img', 'aria-label': 'Woodland stand map' });
    worldEl = svgEl('g', { class: 'ccf-ml-world' });
    layerGroups = createLayerGroups();
    worldEl.append(...layerGroups);
    overlayEl.append(worldEl);
    stageEl.append(canvasEl, imageryStateEl, overlayEl);
    noticeEl = htmlEl('div', { class: 'ccf-ml-notice', role: 'status', 'aria-live': 'polite' });
    attributionEl = htmlEl('div', { class: 'ccf-ml-attribution', role: 'note', 'aria-label': 'Imagery attribution' });
    rootEl.append(stageEl, noticeEl, attributionEl);
    host.replaceChildren(rootEl);
    updateStatus();

    stageEl.addEventListener('pointerdown', onPointerDown);
    stageEl.addEventListener('pointerup', onPointerUp);
    resizeObserver = new ResizeObserver(onResize);
    resizeObserver.observe(stageEl);
  }

  function render(rawFrame) {
    if (!mounted || destroyed) return;
    frame = normalizeFrame(rawFrame);
    generation += 1;
    // A frame that hides the focused lane frames the whole visible woodland again (spec §3), in
    // both styles. Clean Woodland reads the focus on every redraw; Satellite needs a camera refit.
    const focusLost = Boolean(focusedAreaId) && !frame.visibleAreaIds.includes(focusedAreaId);
    if (focusLost) {
      diag('focus-cleared', { reason: 'focused area hidden by the frame', areaId: focusedAreaId });
      focusedAreaId = null;
    }
    if (frame.mapStyle !== 'satellite') { showCleanWoodland(FALLBACK_CODES.NONE); return; }
    if (displayFailed) { showCleanWoodland(FALLBACK_CODES.DISPLAY_UNAVAILABLE); return; }

    // Validate the anchor before anything touches MapLibre: no valid location, no MapLibre request.
    const binding = { sourceId: frame.sourceId, geometryDigest: frame.geometryDigest };
    const check = validateGeoAnchor(frame.geoAnchor, binding);
    if (!check.valid || !frame.worldBounds) {
      diag('geo-anchor-rejected', { code: check.valid ? 'NO_AREAS' : check.code });
      showCleanWoodland(FALLBACK_CODES.NO_LOCATION);
      return;
    }
    // Part 1 shows one Web Mercator world on one longitude branch. A stand footprint that crosses
    // the antimeridian or the Mercator latitude limit is not shown on MapLibre at all (not sticky:
    // the next frame with a supported footprint can use Satellite). Imagery is judged first and on
    // its own: imagery that is not eligible is dropped and never takes part in this preflight.
    const nextBridge = createGeoAnchorBridge(frame.geoAnchor, binding);
    const imageryCheck = imageryEligibility(frame, nextBridge);
    const fit = checkWebMercatorFootprint(nextBridge, [areasBounds(frame.areas)]);
    if (!fit.ok) {
      diag('footprint-unsupported', { code: fit.code, reason: fit.reason });
      showCleanWoodland(FALLBACK_CODES.DISPLAY_UNAVAILABLE);
      return;
    }
    if (!map && !webglAvailable()) { failDisplay('WebGL unavailable'); return; }
    ensureMapLibreStylesheet();

    // Keep what is on screen in step with the new frame while async work runs.
    if (activeStyle === 'satellite' && map) {
      if (focusLost) fitSatelliteCamera();
      else syncSatelliteOverlay();
    } else {
      redrawCleanWoodland();
    }
    void startSatellite(frame, nextBridge, generation, imageryCheck);
  }

  function setSelection(next) {
    if (!mounted || destroyed) return;
    selection = next && (next.kind === 'tree' || next.kind === 'cell' || next.kind === 'juvenile') && next.id
      ? Object.freeze({ kind: next.kind, areaId: typeof next.areaId === 'string' ? next.areaId : null, id: String(next.id) })
      : null;
    if (matrix && frame) drawOverlay(matrix);
  }

  function focusArea(areaId) {
    if (!mounted || destroyed) return;
    const next = typeof areaId === 'string' && areaId ? areaId : null;
    // A lane that the current frame hides is not framed: the visible lanes keep their place.
    if (next && frame && !frame.visibleAreaIds.includes(next)) {
      diag('focus-ignored', { reason: 'area not visible', areaId: next });
      return;
    }
    focusedAreaId = next;
    if (!frame) return;
    if (activeStyle === 'satellite' && map) fitSatelliteCamera();
    else redrawCleanWoodland();
  }

  function destroy() {
    if (!mounted) return;
    generation += 1;
    destroyed = true;
    mounted = false;
    if (resizeObserver) resizeObserver.disconnect();
    resizeObserver = null;
    if (stageEl) {
      stageEl.removeEventListener('pointerdown', onPointerDown);
      stageEl.removeEventListener('pointerup', onPointerUp);
    }
    clearWatchdog();
    abortPendingProbes('destroyed');
    teardownMap();
    if (host && rootEl && rootEl.parentNode === host) host.removeChild(rootEl);
    host = rootEl = stageEl = canvasEl = imageryStateEl = overlayEl = worldEl = noticeEl = attributionEl = null;
    layerGroups = [];
    frame = null;
    selection = null;
    focusedAreaId = null;
    matrix = null;
    built = { frame: null, selection: null, mpp: 0 };
    wantedImagery = null;
    deps = {};
    pointerDown = null;
    // Reset the transient display state so a remount starts cleanly in Clean Woodland. Only the
    // sticky display failure (and the per-URL probe results) last for the adapter's lifetime.
    activeStyle = 'clean-woodland';
    fallbackCode = FALLBACK_CODES.NONE;
  }

  const renderer = { mount, render, setSelection, focusArea, destroy };

  if (options.exposeDebug === true) {
    // Test-only inspection handle: non-enumerable and symbol-keyed, so the public shape stays five methods.
    Object.defineProperty(renderer, Symbol.for('ccf.maplibre.debug'), {
      enumerable: false,
      value: () => ({
        mounted, destroyed, generation, activeStyle, fallbackCode, displayFailed, mapLoaded, mapReady, imageryLoaded,
        hasMap: Boolean(map),
        watchdogArmed: Boolean(loadWatchdog),
        pendingProbes: Array.from(probeCache.values()).filter((p) => !p.settled).length,
        focusedAreaId,
        selection,
        frame,
        matrix,
        layerStack: LAYER_STACK,
        workerUrl: mapModule ? mapModule.getWorkerUrl() : null,
        style: map ? map.getStyle() : null,
        rasterOpacity: map && map.getLayer(IMAGERY_LAYER_ID) ? map.getPaintProperty(IMAGERY_LAYER_ID, 'raster-opacity') : null,
        zoom: map ? map.getZoom() : null,
        bearing: map ? map.getBearing() : null,
      }),
    });
    Object.defineProperty(renderer, Symbol.for('ccf.maplibre.debug.actions'), {
      enumerable: false,
      value: Object.freeze({
        /** Screen pixel of a local point through MapLibre itself (independent of the overlay matrix). */
        project: (xM, zM) => (map && bridge ? map.project(bridge.localToWgs84(xM, zM)) : null),
        /** Local metres of a stage pixel through MapLibre unproject + inverse geo bridge. */
        unproject: (px, py) => {
          if (!map || !bridge) return null;
          const ll = map.unproject([px, py]);
          return bridge.wgs84ToLocal(ll.lng, ll.lat);
        },
        /** Resolves after MapLibre has repainted and gone idle (or after timeoutMs). */
        whenIdle: (timeoutMs = 4000) => new Promise((resolve) => {
          if (!map) { resolve(false); return; }
          const timer = setTimeout(() => resolve(false), timeoutMs);
          map.once('idle', () => { clearTimeout(timer); resolve(true); });
          map.triggerRepaint();
        }),
        zoomBy: (dz) => { if (map) map.jumpTo({ zoom: map.getZoom() + dz }); },
        panBy: (dx, dy) => { if (map) map.panBy([dx, dy], { duration: 0 }); },
        loseContext: () => {
          // getContext with the type already in use returns MapLibre's own context.
          const canvas = map ? map.getCanvas() : null;
          const gl = canvas ? canvas.getContext('webgl2') || canvas.getContext('webgl') : null;
          const ext = gl ? gl.getExtension('WEBGL_lose_context') : null;
          if (ext) ext.loseContext();
          return Boolean(ext);
        },
      }),
    });
  }

  return Object.freeze(renderer);
}

export { LAYER_STACK } from './layer-stack.js';
export { DEFAULT_CANVAS_COLOR, DEFAULT_IMAGERY_OPACITY, DEFAULT_MAP_STYLE, FALLBACK_COPY } from './imagery-policy.js';
