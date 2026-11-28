/**
 * Deterministic 80 x 80 m, 5-lane test frame for B01-B20.
 *
 * SYNTHETIC TEST DATA. The anchor coordinates, trees, cells and imagery are made up for
 * verification only; they describe no real woodland and carry no ecological meaning.
 * Lanes MA-A..MA-E are 16 m wide strips side by side (x 0..80 m, z 0..80 m).
 */

export const FIXTURE_SOURCE_ID = 'ccf-fixture-80x80-5lane';
export const FIXTURE_GEOMETRY_DIGEST = 'sha256:fixture-80x80-5lane-v1';
export const FIXTURE_FRAME_KEY = 'fixture-80x80-5lane-y0';

export const VALID_GEO_ANCHOR = Object.freeze({
  sourceId: FIXTURE_SOURCE_ID,
  geometryDigest: FIXTURE_GEOMETRY_DIGEST,
  originKind: 'survey-corner',
  lonDeg: -6.7421,
  latDeg: 53.3468,
  heightM: 85,
  bearingDeg: 12.5,
  localOriginXM: 0,
  localOriginZM: 0,
});

/** Imagery covers a 128 m square (24 m margin around the woodland) so pure imagery can be sampled. */
export const SYNTHETIC_IMAGERY = Object.freeze({
  url: '/fixtures/synthetic-imagery.png',
  attribution: 'Synthetic local test imagery (not a real photograph)',
  boundsLocal: Object.freeze({ minXM: -24, minZM: -24, maxXM: 104, maxZM: 104 }),
});

/** The uniform colour of the synthetic imagery margin (see synthetic-png.mjs). */
export const IMAGERY_MARGIN_RGB = Object.freeze([196, 168, 92]);

const CELL_M = 2.5;
const LANE_WIDTH_M = 16;
const SIDE_M = 80;

function laneCells(laneIndex) {
  const cells = [];
  const columns = Math.ceil(LANE_WIDTH_M / CELL_M); // 7: the last column is 1 m wide, clipped at the lane edge
  for (let column = 0; column < columns; column++) {
    for (let row = 0; row < SIDE_M / CELL_M; row++) {
      const u0 = column * CELL_M;
      const u1 = Math.min(LANE_WIDTH_M, u0 + CELL_M);
      const v0 = row * CELL_M;
      cells.push({
        column,
        row,
        polygon: [{ u: u0, v: v0 }, { u: u1, v: v0 }, { u: u1, v: v0 + CELL_M }, { u: u0, v: v0 + CELL_M }],
        lightIndex: ((column + row) % 3) / 2,
        bramble: (column + row) % 4 === 0 ? 0.5 : 0,
        bracken: row % 3 === 0 ? 0.35 : 0,
        seedlingMask: (column + row) % 5 === 0 ? 1 : 0,
        // Fixture-only moisture pattern: western half dry, eastern half moist.
        soilMoistureClass: laneIndex * LANE_WIDTH_M + u0 < 40 ? 'DryFreeDraining' : 'FreshMoistMineral',
      });
    }
  }
  return cells;
}

function lane(index, letter) {
  const areaId = 'MA-' + letter;
  const minXM = index * LANE_WIDTH_M;
  const trees = [
    { treeId: areaId + '-T01', u: 4.5, v: 18, radiusM: 1.2, species: 'sitka-spruce', status: 'Living', isCrop: false, paintOrder: 1 },
    { treeId: areaId + '-T02', u: 8.2, v: 36, radiusM: 2.4, species: letter === 'C' ? 'sessile-oak' : 'sitka-spruce', status: 'Living', isCrop: letter === 'C', paintOrder: 2 },
    { treeId: areaId + '-T03', u: 11.4, v: 54, radiusM: 3.8, species: letter === 'A' ? 'scots-pine' : 'european-beech', status: letter === 'B' ? 'PendingFell' : 'Living', isCrop: false, paintOrder: 3 },
    { treeId: areaId + '-T04', u: 6, v: 68, radiusM: 1.85, species: 'sitka-spruce', status: { D: 'ApprovedFell', E: 'NaturalDead', A: 'Felled' }[letter] || 'Living', isCrop: false, paintOrder: 4 },
  ];
  return {
    areaId,
    label: 'Lane ' + letter,
    extent: { minXM, minZM: 0, sideM: SIDE_M },
    polygon: [{ xm: minXM, zm: 0 }, { xm: minXM + LANE_WIDTH_M, zm: 0 }, { xm: minXM + LANE_WIDTH_M, zm: SIDE_M }, { xm: minXM, zm: SIDE_M }],
    groundCells: laneCells(index),
    deadwood: [{ deadwoodId: areaId + '-DW01', u: 10, v: 26 }],
    trees,
    juveniles: [{ juvenileId: areaId + '-PJ01', u: 5.5, v: 44, speciesId: 'sessile-oak', isActive: true, hasActiveShelter: letter === 'C' }],
    ranks: letter === 'C' ? [{ treeId: 'MA-C-T01', rank: 1, chosen: true }, { treeId: 'MA-C-T03', rank: 2, chosen: false }] : [],
  };
}

export function createFixtureFrame(overrides = {}) {
  return {
    frameKey: FIXTURE_FRAME_KEY,
    sourceId: FIXTURE_SOURCE_ID,
    geometryDigest: FIXTURE_GEOMETRY_DIGEST,
    mapStyle: 'clean-woodland',
    imageryOpacity: 0.65,
    imagery: { ...SYNTHETIC_IMAGERY, boundsLocal: { ...SYNTHETIC_IMAGERY.boundsLocal } },
    geoAnchor: { ...VALID_GEO_ANCHOR },
    activeAreaId: 'MA-C',
    visibleAreaIds: ['MA-A', 'MA-B', 'MA-C', 'MA-D', 'MA-E'],
    overlay: 'soil-moisture',
    areas: ['A', 'B', 'C', 'D', 'E'].map((letter, i) => lane(i, letter)),
    ...overrides,
  };
}
