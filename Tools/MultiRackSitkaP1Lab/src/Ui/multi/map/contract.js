// S0c provenance: map/contract.js [L3] — map renderer contract (SPEC-multi-2d-s0-seam-extraction-V1 §7.3)
/**
 * Map renderer contract (S0c). Renderers draw exactly the frame they are given; they compute no forestry (D-020).
 * @typedef {object} MapFrame        The latest atomic composite payload slice already held in store.state (read-only).
 * @typedef {{kind: 'tree'|'cell'|'juvenile'|null, areaId: string|null, id: string|null}} MapSelection
 * @typedef {object} MapRendererDeps  Callbacks owned by main.js (e.g. onMapClick) and the fixed label lookup.
 * @typedef {object} MapRenderer
 * @property {(container: Element, deps: MapRendererDeps) => void} mount   Attach map DOM inside container; called once.
 * @property {(frame: MapFrame) => void} render                             Draw exactly this frame.
 * @property {(selection: MapSelection) => void} setSelection               Reflect a selection the page already decided.
 * @property {(areaId: string) => void} focusArea                           Frame the named area; changes no data.
 * @property {() => void} destroy                                           Detach listeners and DOM; idempotent.
 */
export const MAP_RENDERER_METHODS = Object.freeze(['mount', 'render', 'setSelection', 'focusArea', 'destroy']);

/** @param {unknown} candidate @returns {MapRenderer} @throws {TypeError} when a method is missing. */
export function assertMapRenderer(candidate) {
  if (!candidate || (typeof candidate !== 'object' && typeof candidate !== 'function')) {
    throw new TypeError('MapRenderer candidate must be a non-null object');
  }
  for (const method of MAP_RENDERER_METHODS) {
    if (typeof candidate[method] !== 'function') {
      throw new TypeError('MapRenderer missing required method: ' + method);
    }
  }
  return candidate;
}
