/**
 * The strict 8-layer bottom-to-top visual stack (SPEC-multi-2d-maplibre-adapter-V1 §2).
 * Both styles use this order. Layers 1-2 live in the MapLibre canvas (Satellite) or the plain
 * canvas element (Clean Woodland); Layers 3-8 are SVG groups in the shared metre-space overlay.
 */

export const STAGE2_RESERVED_SLOTS = Object.freeze([
  'drone-lidar',
  'hillshade-contours',
  'streams-rivers',
  'roads-bridges-paths',
]);

export const LAYER_STACK = Object.freeze([
  Object.freeze({ index: 1, id: 'woodland-canvas', label: 'Dark woodland canvas (#15251a)', reserved: false }),
  Object.freeze({ index: 2, id: 'georeferenced-imagery', label: 'Optional local georeferenced imagery', reserved: false }),
  Object.freeze({ index: 3, id: 'stage2-reserved', label: 'Reserved Stage 2 slots (unimplemented)', reserved: true }),
  Object.freeze({ index: 4, id: 'ground-overlays', label: 'Ground overlays (2.5 m cells)', reserved: false }),
  Object.freeze({ index: 5, id: 'schematic-deadwood', label: 'Schematic deadwood', reserved: false }),
  Object.freeze({ index: 6, id: 'living-crowns', label: 'Living crowns (true radiusM)', reserved: false }),
  Object.freeze({ index: 7, id: 'planted-juveniles', label: 'Planted juveniles', reserved: false }),
  Object.freeze({ index: 8, id: 'marks-ranks-selection', label: 'Marks, ranks and selection (top)', reserved: false }),
]);

export const LAYER_IDS_ORDERED = Object.freeze(LAYER_STACK.map((layer) => layer.id));

/** True when `observedIds` is exactly the bottom-to-top stack order. */
export function verifyLayerStackOrder(observedIds) {
  const observed = Array.from(observedIds || []);
  const ok = observed.length === LAYER_IDS_ORDERED.length && observed.every((id, i) => id === LAYER_IDS_ORDERED[i]);
  return { ok, expected: LAYER_IDS_ORDERED, observed };
}
