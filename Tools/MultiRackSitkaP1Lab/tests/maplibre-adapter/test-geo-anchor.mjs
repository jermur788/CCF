#!/usr/bin/env node
/**
 * Ellipsoidal Geo-Anchor Bridge verification with an INDEPENDENT Vincenty inverse geodesic oracle.
 * Zero code shared with geo-anchor.js.
 *
 * Checks:
 * 1) Round-trip error < 0.01 m across a grid of offsets, bearings, and latitudes.
 * 2) Independent Vincenty inverse distance agreement < 0.01 m at 30 m, 80 m, and 120 m stands.
 * 3) Bearing / azimuth direction agreement against Vincenty forward azimuth.
 * 4) Strict binding validation rejections (centroid, missing identity, digest mismatch, out-of-range, non-finite).
 */

import { checkWebMercatorFootprint, createGeoAnchorBridge, validateGeoAnchor } from '../../src/Ui/multi/map/maplibre/geo-anchor.js';

// ---- Independent Vincenty Inverse Geodesic Oracle on WGS84 (no shared code with geo-anchor.js) ----
const ORACLE_A = 6378137.0;
const ORACLE_F = 1.0 / 298.257223563;
const ORACLE_B = ORACLE_A * (1.0 - ORACLE_F);

function vincentyInverse(lon1Deg, lat1Deg, lon2Deg, lat2Deg) {
  const rad = Math.PI / 180.0;
  const phi1 = lat1Deg * rad;
  const phi2 = lat2Deg * rad;
  const L = (lon2Deg - lon1Deg) * rad;

  if (Math.abs(phi1 - phi2) < 1e-15 && Math.abs(L) < 1e-15) {
    return { distanceM: 0, azimuth1Deg: 0 };
  }

  const U1 = Math.atan((1 - ORACLE_F) * Math.tan(phi1));
  const U2 = Math.atan((1 - ORACLE_F) * Math.tan(phi2));
  const sinU1 = Math.sin(U1);
  const cosU1 = Math.cos(U1);
  const sinU2 = Math.sin(U2);
  const cosU2 = Math.cos(U2);

  let lambda = L;
  let iter = 0;
  let sinSigma = 0;
  let cosSigma = 0;
  let sigma = 0;
  let sinAlpha = 0;
  let cosSqAlpha = 0;
  let cos2SigmaM = 0;

  while (iter++ < 200) {
    const sinLambda = Math.sin(lambda);
    const cosLambda = Math.cos(lambda);
    const t1 = cosU2 * sinLambda;
    const t2 = cosU1 * sinU2 - sinU1 * cosU2 * cosLambda;
    sinSigma = Math.sqrt(t1 * t1 + t2 * t2);
    if (sinSigma === 0) return { distanceM: 0, azimuth1Deg: 0 };
    cosSigma = sinU1 * sinU2 + cosU1 * cosU2 * cosLambda;
    sigma = Math.atan2(sinSigma, cosSigma);
    sinAlpha = (cosU1 * cosU2 * sinLambda) / sinSigma;
    cosSqAlpha = 1 - sinAlpha * sinAlpha;
    cos2SigmaM = cosSqAlpha !== 0 ? (cosSigma - (2 * sinU1 * sinU2) / cosSqAlpha) : 0;
    const C = (ORACLE_F / 16) * cosSqAlpha * (4 + ORACLE_F * (4 - 3 * cosSqAlpha));
    const prevLambda = lambda;
    lambda = L + (1 - C) * ORACLE_F * sinAlpha * (
      sigma + C * sinSigma * (cos2SigmaM + C * cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM))
    );
    if (Math.abs(lambda - prevLambda) < 1e-13) break;
  }

  const uSq = (cosSqAlpha * (ORACLE_A * ORACLE_A - ORACLE_B * ORACLE_B)) / (ORACLE_B * ORACLE_B);
  const A = 1 + (uSq / 16384) * (4096 + uSq * (-768 + uSq * (320 - 175 * uSq)));
  const B = (uSq / 1024) * (256 + uSq * (-128 + uSq * (74 - 47 * uSq)));
  const deltaSigma = B * sinSigma * (
    cos2SigmaM + (B / 4) * (
      cosSigma * (-1 + 2 * cos2SigmaM * cos2SigmaM) -
      (B / 6) * cos2SigmaM * (-3 + 4 * sinSigma * sinSigma) * (-3 + 4 * cos2SigmaM * cos2SigmaM)
    )
  );
  const distanceM = ORACLE_B * A * (sigma - deltaSigma);
  const azRad = Math.atan2(
    cosU2 * Math.sin(lambda),
    cosU1 * sinU2 - sinU1 * cosU2 * Math.cos(lambda),
  );
  const azimuth1Deg = ((azRad * 180) / Math.PI + 360) % 360;
  return { distanceM, azimuth1Deg };
}

let passed = 0;
let failed = 0;

function check(ok, msg) {
  if (ok) {
    passed++;
    console.log(`PASS [geo-anchor] ${msg}`);
  } else {
    failed++;
    console.error(`FAIL [geo-anchor] ${msg}`);
  }
}

const TOLERANCE_M = 0.01;
const TEST_LOCATIONS = [
  { name: 'Ireland-Kildare', lonDeg: -6.7421, latDeg: 53.3468 },
  { name: 'Equator', lonDeg: 10.25, latDeg: 0.0 },
  { name: 'Southern-NZ', lonDeg: 174.7762, latDeg: -41.2865 },
  { name: 'Northern-Iceland', lonDeg: -21.9426, latDeg: 64.1466 },
];
const TEST_BEARINGS = [0, 12.5, 45, 90, 142.75, 180, 237.3, 315];

// 1. Round-trip accuracy across offsets, bearings, and latitudes
let maxRoundTripErrM = 0;
let roundTripCount = 0;
for (const loc of TEST_LOCATIONS) {
  for (const bearingDeg of TEST_BEARINGS) {
    const bridge = createGeoAnchorBridge({
      sourceId: 'woodland-rt-test',
      geometryDigest: 'sha256:rt-digest-001',
      originKind: 'survey-corner',
      lonDeg: loc.lonDeg,
      latDeg: loc.latDeg,
      bearingDeg,
      localOriginXM: 10,
      localOriginZM: -5,
    }, { sourceId: 'woodland-rt-test', geometryDigest: 'sha256:rt-digest-001' });
    for (const xM of [-120, -40, 0, 10, 40, 80, 120]) {
      for (const zM of [-120, -40, 0, 15, 40, 80, 120]) {
        const [lon, lat] = bridge.localToWgs84(xM, zM);
        const back = bridge.wgs84ToLocal(lon, lat);
        const err = Math.hypot(back.xM - xM, back.zM - zM);
        if (err > maxRoundTripErrM) maxRoundTripErrM = err;
        roundTripCount++;
      }
    }
  }
}
check(maxRoundTripErrM < TOLERANCE_M,
  `Round-trip max error across ${roundTripCount} points (4 latitudes x 8 bearings) = ${maxRoundTripErrM.toExponential(3)} m < ${TOLERANCE_M} m`);

// 2. Independent Vincenty oracle distance agreement at 30 m, 80 m, and 120 m stands
const standSizes = [30, 80, 120];
const oracleMaxByStand = {};
let maxAzimuthErrDeg = 0;

for (const sizeM of standSizes) {
  let maxDistErrM = 0;
  for (const loc of TEST_LOCATIONS) {
    for (const bearingDeg of TEST_BEARINGS) {
      const bridge = createGeoAnchorBridge({
        sourceId: `stand-${sizeM}m`,
        geometryDigest: `sha256:stand-${sizeM}`,
        originKind: 'site-origin',
        lonDeg: loc.lonDeg,
        latDeg: loc.latDeg,
        bearingDeg,
      }, { sourceId: `stand-${sizeM}m`, geometryDigest: `sha256:stand-${sizeM}` });
      const samplePoints = [
        [sizeM, 0],
        [0, sizeM],
        [sizeM, sizeM],
        [sizeM * 0.5, sizeM * 0.5],
        [-sizeM * 0.5, sizeM * 0.75],
      ];
      const [lon0, lat0] = bridge.localToWgs84(0, 0);
      for (const [xM, zM] of samplePoints) {
        const expectedDist = Math.hypot(xM, zM);
        const [lon1, lat1] = bridge.localToWgs84(xM, zM);
        const oracle = vincentyInverse(lon0, lat0, lon1, lat1);
        const distErr = Math.abs(oracle.distanceM - expectedDist);
        if (distErr > maxDistErrM) maxDistErrM = distErr;

        // Axis convention: local +zM points at bearingDeg clockwise from True North and local +xM
        // at bearingDeg + 90. Checking both axes also catches a mirrored (handedness-flipped) frame,
        // which distance checks alone cannot see.
        const expectedAz = xM === 0 && zM === sizeM ? bearingDeg : xM === sizeM && zM === 0 ? bearingDeg + 90 : null;
        if (expectedAz !== null) {
          let azDiff = Math.abs(oracle.azimuth1Deg - (((expectedAz % 360) + 360) % 360));
          if (azDiff > 180) azDiff = 360 - azDiff;
          if (azDiff > maxAzimuthErrDeg) maxAzimuthErrDeg = azDiff;
        }
      }
      // Also check diagonal corner-to-corner distance between (0, sizeM) and (sizeM, 0)
      const [lonA, latA] = bridge.localToWgs84(0, sizeM);
      const [lonB, latB] = bridge.localToWgs84(sizeM, 0);
      const diagOracle = vincentyInverse(lonA, latA, lonB, latB);
      const diagErr = Math.abs(diagOracle.distanceM - Math.hypot(sizeM, sizeM));
      if (diagErr > maxDistErrM) maxDistErrM = diagErr;
    }
  }
  oracleMaxByStand[sizeM] = maxDistErrM;
  check(maxDistErrM < TOLERANCE_M,
    `Independent Vincenty oracle max distance error at ${sizeM} m stand = ${maxDistErrM.toExponential(3)} m < ${TOLERANCE_M} m`);
}

check(maxAzimuthErrDeg < 0.01,
  `True-north clockwise azimuth of local +zM (bearing) and +xM (bearing+90) max error vs Vincenty = ${maxAzimuthErrDeg.toExponential(3)} deg < 0.01 deg`);

// 3. Area (u, v) round-trip helper check
{
  const bridge = createGeoAnchorBridge({
    sourceId: 'stand-80m',
    geometryDigest: 'sha256:uv-test',
    originKind: 'survey-corner',
    lonDeg: -6.7421,
    latDeg: 53.3468,
    bearingDeg: 12.5,
  }, { sourceId: 'stand-80m', geometryDigest: 'sha256:uv-test' });
  const extent = { minXM: 32, minZM: 0, sideM: 80 };
  const [lon, lat] = bridge.areaUvToWgs84(extent, 8.25, 37.5);
  const uv = bridge.wgs84ToAreaUv(extent, lon, lat);
  const uvErr = Math.hypot(uv.u - 8.25, uv.v - 37.5);
  check(uvErr < 1e-6, `areaUvToWgs84 / wgs84ToAreaUv round-trip error = ${uvErr.toExponential(3)} m < 1e-6 m`);
}

// 4. Strict binding validation rejection cases
const baseValid = {
  sourceId: 'ccf-kildare-80x80',
  geometryDigest: 'sha256:abc123def456',
  originKind: 'survey-corner',
  lonDeg: -6.7421,
  latDeg: 53.3468,
  bearingDeg: 12.5,
};
const expectedBinding = {
  sourceId: 'ccf-kildare-80x80',
  geometryDigest: 'sha256:abc123def456',
};

const rejectionCases = [
  ['frame without binding (no sourceId/digest to bind to)', baseValid, null, 'MISSING_FRAME_BINDING'],
  ['frame binding missing digest', baseValid, { sourceId: 'ccf-kildare-80x80', geometryDigest: '' }, 'MISSING_FRAME_BINDING'],
  ['missing anchor (null)', null, expectedBinding, 'MISSING_ANCHOR'],
  ['missing sourceId', { ...baseValid, sourceId: '' }, expectedBinding, 'MISSING_SOURCE_ID'],
  ['sourceId mismatch', { ...baseValid, sourceId: 'other-woodland' }, expectedBinding, 'SOURCE_ID_MISMATCH'],
  ['missing geometryDigest', { ...baseValid, geometryDigest: '' }, expectedBinding, 'MISSING_GEOMETRY_DIGEST'],
  ['geometryDigest mismatch', { ...baseValid, geometryDigest: 'sha256:wrong' }, expectedBinding, 'GEOMETRY_DIGEST_MISMATCH'],
  ['centroid originKind rejected', { ...baseValid, originKind: 'centroid' }, expectedBinding, 'CENTROID_ANCHOR_REJECTED'],
  ['bbox-centroid originKind rejected', { ...baseValid, originKind: 'bbox-centroid' }, expectedBinding, 'CENTROID_ANCHOR_REJECTED'],
  ['unlisted originKind (bounding-box-center) rejected', { ...baseValid, originKind: 'bounding-box-center' }, expectedBinding, 'INVALID_ORIGIN_KIND'],
  ['isCentroid flag rejected', { ...baseValid, isCentroid: true }, expectedBinding, 'CENTROID_ANCHOR_REJECTED'],
  ['derivedFromCentroid flag rejected', { ...baseValid, derivedFromCentroid: true }, expectedBinding, 'CENTROID_ANCHOR_REJECTED'],
  ['missing originKind rejected', { ...baseValid, originKind: '' }, expectedBinding, 'INVALID_ORIGIN_KIND'],
  ['longitude > 180 rejected', { ...baseValid, lonDeg: 180.5 }, expectedBinding, 'INVALID_LONGITUDE'],
  ['longitude < -180 rejected', { ...baseValid, lonDeg: -181 }, expectedBinding, 'INVALID_LONGITUDE'],
  ['latitude > 90 rejected', { ...baseValid, latDeg: 91 }, expectedBinding, 'INVALID_LATITUDE'],
  ['latitude > Mercator limit rejected', { ...baseValid, latDeg: 86.0 }, expectedBinding, 'OUT_OF_MERCATOR_RANGE'],
  ['non-finite bearing NaN rejected', { ...baseValid, bearingDeg: Number.NaN }, expectedBinding, 'INVALID_BEARING'],
  ['non-finite bearing Infinity rejected', { ...baseValid, bearingDeg: Number.POSITIVE_INFINITY }, expectedBinding, 'INVALID_BEARING'],
  ['missing bearing rejected', { ...baseValid, bearingDeg: undefined }, expectedBinding, 'INVALID_BEARING'],
  ['non-finite localOriginXM rejected', { ...baseValid, localOriginXM: Number.NaN }, expectedBinding, 'INVALID_LOCAL_ORIGIN'],
];

for (const [label, candidate, binding, expectedCode] of rejectionCases) {
  const res = validateGeoAnchor(candidate, binding);
  let threw = false;
  try {
    createGeoAnchorBridge(candidate, binding);
  } catch {
    threw = true;
  }
  check(!res.valid && res.code === expectedCode && threw,
    `Rejection [${label}]: valid=${res.valid}, code=${res.code} (expected ${expectedCode}), throws=${threw}`);
}

const validCheck = validateGeoAnchor(baseValid, expectedBinding);
check(validCheck.valid, `Valid explicit non-centroid geo-anchor accepted (${baseValid.originKind})`);

// 5. Web Mercator footprint check (repair cycle 1): antimeridian east/west and the latitude limit
{
  const stand = { minXM: 0, minZM: 0, maxXM: 80, maxZM: 80 };
  const imagery = { minXM: -24, minZM: -24, maxXM: 104, maxZM: 104 };
  const footprintCases = [
    ['east crossing: anchor 179.9996 E, stand to the east', { lonDeg: 179.9996, latDeg: 53.3468, bearingDeg: 0 }, 'CROSSES_ANTIMERIDIAN'],
    ['west crossing: anchor 179.9996 W, stand to the west', { lonDeg: -179.9996, latDeg: 53.3468, bearingDeg: 0, localOriginXM: 80 }, 'CROSSES_ANTIMERIDIAN'],
    ['anchor exactly on 180, stand to the east', { lonDeg: 180, latDeg: 10, bearingDeg: 0 }, 'CROSSES_ANTIMERIDIAN'],
    ['rotated stand reaching across (bearing 90, anchor 179.9995 E)', { lonDeg: 179.9995, latDeg: 53.3468, bearingDeg: 90, localOriginZM: -80 }, 'CROSSES_ANTIMERIDIAN'],
    ['Mercator limit: anchor 85.0508 N, stand to the north', { lonDeg: 10, latDeg: 85.0508, bearingDeg: 0 }, 'OUTSIDE_MERCATOR_LATITUDE'],
    ['Mercator limit: anchor 85.0508 S, stand to the south', { lonDeg: 10, latDeg: -85.0508, bearingDeg: 0, localOriginZM: 80 }, 'OUTSIDE_MERCATOR_LATITUDE'],
    ['near antimeridian, east side 179.99 E', { lonDeg: 179.99, latDeg: 53.3468, bearingDeg: 0 }, null],
    ['near antimeridian, west side 179.99 W', { lonDeg: -179.99, latDeg: 53.3468, bearingDeg: 0, localOriginXM: 80 }, null],
    ['high latitude 84.9 N', { lonDeg: -6.7421, latDeg: 84.9, bearingDeg: 12.5 }, null],
    ['Kildare fixture anchor', { lonDeg: -6.7421, latDeg: 53.3468, bearingDeg: 12.5 }, null],
  ];
  for (const [label, patch, expected] of footprintCases) {
    const bridge = createGeoAnchorBridge({ ...baseValid, ...patch }, expectedBinding);
    const res = checkWebMercatorFootprint(bridge, [stand, imagery]);
    check(expected ? !res.ok && res.code === expected : res.ok,
      `Footprint [${label}]: ok=${res.ok}${res.ok ? '' : ', code=' + res.code} (expected ${expected || 'ok'})`);
  }
}

console.log(`GEO_ORACLE_SUMMARY: ${passed} passed, ${failed} failed, max_roundtrip_m=${maxRoundTripErrM.toExponential(4)}, oracle_30m=${oracleMaxByStand[30].toExponential(4)}, oracle_80m=${oracleMaxByStand[80].toExponential(4)}, oracle_120m=${oracleMaxByStand[120].toExponential(4)}`);
process.exit(failed === 0 ? 0 : 1);
