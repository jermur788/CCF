/**
 * Ellipsoidal WGS84 tangent-plane Geo-Anchor Bridge (display-only).
 *
 * Axis & sign conventions (SPEC-multi-2d-maplibre-adapter-V1 §5):
 * - Engine flat local coordinates (xM, zM) are in metres:
 *     +xM = local east, +zM = local north (when bearingDeg === 0).
 * - Area local coordinates (u, v) match renderer-svg.js:
 *     u = xM - extent.minXM  (east, metres)
 *     v = (extent.minZM + extent.sideM) - zM  (south, metres)
 * - bearingDeg is the true-north clockwise azimuth (in degrees) of the woodland's
 *   local +zM axis relative to geodetic True North:
 *     bearingDeg =  0 => local +zM points North, local +xM points East.
 *     bearingDeg = 90 => local +zM points East,  local +xM points South.
 *   Rotation from local offsets (dx, dz) to tangent-plane ENU (E, N, U=0):
 *     E =  dx * cos(theta) + dz * sin(theta)
 *     N = -dx * sin(theta) + dz * cos(theta)
 *   Inverse rotation from ENU (E, N) to local offsets (dx, dz):
 *     dx =  E * cos(theta) - N * sin(theta)
 *     dz =  E * sin(theta) + N * cos(theta)
 *
 * Ellipsoid: WGS84 (a = 6378137.0 m, f = 1 / 298.257223563).
 * Display-only: zero forestry or ecological calculations occur here.
 */

export const WGS84_A = 6378137.0;
export const WGS84_F = 1 / 298.257223563;
export const WGS84_E2 = WGS84_F * (2 - WGS84_F);
export const MERCATOR_MAX_LAT_DEG = 85.0511287798066;

const DEG_TO_RAD = Math.PI / 180;
const RAD_TO_DEG = 180 / Math.PI;

/**
 * Explicit origin kinds accepted in Part 1. An anchor must name a deliberately placed origin
 * (a surveyed corner/point or a declared site origin). Anything derived from the geometry itself
 * (e.g. a centroid or bounding-box centre) is rejected, as is any kind not on this list.
 */
export const ALLOWED_ORIGIN_KINDS = Object.freeze(['survey-corner', 'survey-point', 'site-origin']);

function isFiniteNumber(v) {
  return typeof v === 'number' && Number.isFinite(v);
}

function nonEmptyString(v) {
  return typeof v === 'string' && v.trim().length > 0;
}

/**
 * Validate a geo-anchor record and its binding to a woodland source & geometry digest.
 * @param {unknown} geoAnchor
 * @param {{ sourceId: string, geometryDigest: string }} expectedBinding  Identity of the frame the anchor must belong to (required).
 * @param {{ requireMercatorRange?: boolean }} [options]
 * @returns {{ valid: true, anchor: Readonly<object> } | { valid: false, code: string, reason: string }}
 */
export function validateGeoAnchor(geoAnchor, expectedBinding, options = {}) {
  const requireMercatorRange = options.requireMercatorRange !== false;
  if (!expectedBinding || !nonEmptyString(expectedBinding.sourceId) || !nonEmptyString(expectedBinding.geometryDigest)) {
    return { valid: false, code: 'MISSING_FRAME_BINDING', reason: 'The woodland frame has no stable sourceId and geometryDigest to bind an anchor to.' };
  }
  if (!geoAnchor || typeof geoAnchor !== 'object') {
    return { valid: false, code: 'MISSING_ANCHOR', reason: 'Geo-anchor object is missing.' };
  }

  const sourceId = nonEmptyString(geoAnchor.sourceId) ? geoAnchor.sourceId.trim() : '';
  if (!sourceId) {
    return { valid: false, code: 'MISSING_SOURCE_ID', reason: 'Geo-anchor requires a stable woodland sourceId.' };
  }
  if (sourceId !== expectedBinding.sourceId.trim()) {
    return { valid: false, code: 'SOURCE_ID_MISMATCH', reason: 'Geo-anchor sourceId does not match frame sourceId.' };
  }

  const geometryDigest = nonEmptyString(geoAnchor.geometryDigest) ? geoAnchor.geometryDigest.trim() : '';
  if (!geometryDigest) {
    return { valid: false, code: 'MISSING_GEOMETRY_DIGEST', reason: 'Geo-anchor requires a geometryDigest.' };
  }
  if (geometryDigest !== expectedBinding.geometryDigest.trim()) {
    return { valid: false, code: 'GEOMETRY_DIGEST_MISMATCH', reason: 'Geo-anchor geometryDigest does not match frame geometryDigest.' };
  }

  const rawOriginKind = nonEmptyString(geoAnchor.originKind) ? geoAnchor.originKind.trim() : '';
  if (geoAnchor.isCentroid === true || geoAnchor.derivedFromCentroid === true || rawOriginKind.toLowerCase().includes('centroid')) {
    return { valid: false, code: 'CENTROID_ANCHOR_REJECTED', reason: 'Centroid-derived geo-anchors are not permitted.' };
  }
  if (!ALLOWED_ORIGIN_KINDS.includes(rawOriginKind)) {
    return { valid: false, code: 'INVALID_ORIGIN_KIND', reason: 'Geo-anchor originKind must be one of: ' + ALLOWED_ORIGIN_KINDS.join(', ') + '.' };
  }

  const { lonDeg, latDeg, bearingDeg } = geoAnchor;
  if (!isFiniteNumber(lonDeg) || lonDeg < -180 || lonDeg > 180) {
    return { valid: false, code: 'INVALID_LONGITUDE', reason: 'Geo-anchor lonDeg must be finite within [-180, 180].' };
  }
  if (!isFiniteNumber(latDeg) || latDeg < -90 || latDeg > 90) {
    return { valid: false, code: 'INVALID_LATITUDE', reason: 'Geo-anchor latDeg must be finite within [-90, 90].' };
  }
  if (requireMercatorRange && Math.abs(latDeg) > MERCATOR_MAX_LAT_DEG) {
    return { valid: false, code: 'OUT_OF_MERCATOR_RANGE', reason: 'Geo-anchor latDeg exceeds Web Mercator display latitude limit.' };
  }
  if (!isFiniteNumber(bearingDeg)) {
    return { valid: false, code: 'INVALID_BEARING', reason: 'Geo-anchor bearingDeg must be a finite number.' };
  }

  const heightM = geoAnchor.heightM === undefined ? 0 : geoAnchor.heightM;
  if (!isFiniteNumber(heightM)) {
    return { valid: false, code: 'INVALID_HEIGHT', reason: 'Geo-anchor heightM must be finite when provided.' };
  }

  const localOriginXM = geoAnchor.localOriginXM === undefined ? 0 : geoAnchor.localOriginXM;
  const localOriginZM = geoAnchor.localOriginZM === undefined ? 0 : geoAnchor.localOriginZM;
  if (!isFiniteNumber(localOriginXM) || !isFiniteNumber(localOriginZM)) {
    return { valid: false, code: 'INVALID_LOCAL_ORIGIN', reason: 'Geo-anchor localOriginXM and localOriginZM must be finite.' };
  }

  return {
    valid: true,
    anchor: Object.freeze({
      sourceId,
      geometryDigest,
      originKind: rawOriginKind,
      lonDeg,
      latDeg,
      heightM,
      bearingDeg,
      localOriginXM,
      localOriginZM,
    }),
  };
}

/**
 * Convert WGS84 geodetic coordinates to ECEF metres.
 * @param {number} lonDeg
 * @param {number} latDeg
 * @param {number} [heightM=0]
 * @returns {{ x: number, y: number, z: number }}
 */
export function geodeticToEcef(lonDeg, latDeg, heightM = 0) {
  const lon = lonDeg * DEG_TO_RAD;
  const lat = latDeg * DEG_TO_RAD;
  const sinLat = Math.sin(lat);
  const cosLat = Math.cos(lat);
  const sinLon = Math.sin(lon);
  const cosLon = Math.cos(lon);
  const primeVertical = WGS84_A / Math.sqrt(1 - WGS84_E2 * sinLat * sinLat);
  return {
    x: (primeVertical + heightM) * cosLat * cosLon,
    y: (primeVertical + heightM) * cosLat * sinLon,
    z: (primeVertical * (1 - WGS84_E2) + heightM) * sinLat,
  };
}

/**
 * Convert ECEF metres to WGS84 geodetic coordinates (iterative exact WGS84 inversion).
 * @param {number} x
 * @param {number} y
 * @param {number} z
 * @returns {{ lonDeg: number, latDeg: number, heightM: number }}
 */
export function ecefToGeodetic(x, y, z) {
  const lon = Math.atan2(y, x);
  const p = Math.hypot(x, y);
  if (p < 1e-12) {
    const lat = z >= 0 ? Math.PI / 2 : -Math.PI / 2;
    const semiMinor = WGS84_A * (1 - WGS84_F);
    return {
      lonDeg: lon * RAD_TO_DEG,
      latDeg: lat * RAD_TO_DEG,
      heightM: Math.abs(z) - semiMinor,
    };
  }

  let lat = Math.atan2(z, p * (1 - WGS84_E2));
  let primeVertical = WGS84_A;
  for (let i = 0; i < 12; i++) {
    const sinLat = Math.sin(lat);
    primeVertical = WGS84_A / Math.sqrt(1 - WGS84_E2 * sinLat * sinLat);
    const nextLat = Math.atan2(z + WGS84_E2 * primeVertical * sinLat, p);
    if (Math.abs(nextLat - lat) < 1e-14) {
      lat = nextLat;
      break;
    }
    lat = nextLat;
  }

  const cosLat = Math.cos(lat);
  const heightM = Math.abs(cosLat) > 1e-10
    ? (p / cosLat) - primeVertical
    : (Math.abs(z) / Math.abs(Math.sin(lat))) - primeVertical * (1 - WGS84_E2);

  return {
    lonDeg: lon * RAD_TO_DEG,
    latDeg: lat * RAD_TO_DEG,
    heightM,
  };
}

/**
 * Create an immutable ellipsoidal tangent-plane Geo-Anchor Bridge.
 * Throws when the geo-anchor fails strict binding validation.
 */
export function createGeoAnchorBridge(geoAnchor, expectedBinding, options = {}) {
  const check = validateGeoAnchor(geoAnchor, expectedBinding, options);
  if (!check.valid) {
    throw new Error(`Invalid geo-anchor (${check.code}): ${check.reason}`);
  }
  const anchor = check.anchor;

  const lon0 = anchor.lonDeg * DEG_TO_RAD;
  const lat0 = anchor.latDeg * DEG_TO_RAD;
  const sinLon0 = Math.sin(lon0);
  const cosLon0 = Math.cos(lon0);
  const sinLat0 = Math.sin(lat0);
  const cosLat0 = Math.cos(lat0);

  const ecef0 = geodeticToEcef(anchor.lonDeg, anchor.latDeg, anchor.heightM);
  const bearingRad = anchor.bearingDeg * DEG_TO_RAD;
  const cosB = Math.cos(bearingRad);
  const sinB = Math.sin(bearingRad);

  /** Project flat engine metres (xM, zM) -> WGS84 [lonDeg, latDeg]. */
  function localToWgs84(xM, zM) {
    if (!isFiniteNumber(xM) || !isFiniteNumber(zM)) {
      throw new TypeError('localToWgs84 requires finite numeric xM and zM');
    }
    const dx = xM - anchor.localOriginXM;
    const dz = zM - anchor.localOriginZM;
    // Rotate local (dx, dz) clockwise by bearingDeg onto tangent ENU plane (U = 0).
    const east = dx * cosB + dz * sinB;
    const north = -dx * sinB + dz * cosB;

    const x = ecef0.x - sinLon0 * east - sinLat0 * cosLon0 * north;
    const y = ecef0.y + cosLon0 * east - sinLat0 * sinLon0 * north;
    const z = ecef0.z + cosLat0 * north;

    const geo = ecefToGeodetic(x, y, z);
    return [geo.lonDeg, geo.latDeg];
  }

  /** Project WGS84 (lonDeg, latDeg) -> flat engine metres { xM, zM }. */
  function wgs84ToLocal(lonDeg, latDeg) {
    if (!isFiniteNumber(lonDeg) || !isFiniteNumber(latDeg)) {
      throw new TypeError('wgs84ToLocal requires finite numeric lonDeg and latDeg');
    }
    const ecef = geodeticToEcef(lonDeg, latDeg, anchor.heightM);
    const rx = ecef.x - ecef0.x;
    const ry = ecef.y - ecef0.y;
    const rz = ecef.z - ecef0.z;

    const east = -sinLon0 * rx + cosLon0 * ry;
    const north = -sinLat0 * cosLon0 * rx - sinLat0 * sinLon0 * ry + cosLat0 * rz;

    const dx = east * cosB - north * sinB;
    const dz = east * sinB + north * cosB;
    return {
      xM: anchor.localOriginXM + dx,
      zM: anchor.localOriginZM + dz,
    };
  }

  /** Project area-local (u east, v south) -> WGS84 [lonDeg, latDeg]. */
  function areaUvToWgs84(extent, u, v) {
    const xM = extent.minXM + u;
    const zM = extent.minZM + extent.sideM - v;
    return localToWgs84(xM, zM);
  }

  /** Project WGS84 (lonDeg, latDeg) -> area-local { u, v }. */
  function wgs84ToAreaUv(extent, lonDeg, latDeg) {
    const { xM, zM } = wgs84ToLocal(lonDeg, latDeg);
    return {
      u: xM - extent.minXM,
      v: extent.minZM + extent.sideM - zM,
    };
  }

  /**
   * Project local rectangular bounds into MapLibre image source corner order:
   * [top-left (NW), top-right (NE), bottom-right (SE), bottom-left (SW)].
   */
  function boundsToWgs84Quad(minXM, minZM, maxXM, maxZM) {
    return [
      localToWgs84(minXM, maxZM),
      localToWgs84(maxXM, maxZM),
      localToWgs84(maxXM, minZM),
      localToWgs84(minXM, minZM),
    ];
  }

  return Object.freeze({
    anchor,
    localToWgs84,
    wgs84ToLocal,
    areaUvToWgs84,
    wgs84ToAreaUv,
    boundsToWgs84Quad,
  });
}

/** Extra metres added around every footprint box before it is checked (covers the ±10 m Jacobian samples). */
export const FOOTPRINT_MARGIN_M = 50;

/**
 * Check that a woodland footprint can be shown on one Web Mercator world without wrapping.
 *
 * Part 1 keeps one longitude branch for camera, overlay and imagery: the footprint (every box,
 * grown by marginM) must stay on the anchor's side of the antimeridian and inside the Web
 * Mercator latitude limit. A footprint that crosses either limit is reported as unsupported, so
 * the adapter can fall back before MapLibre starts rather than draw positions from the wrong
 * side of the world. Longitudes are unwrapped relative to the anchor, so east and west crossings
 * are both detected. The boxes are small (metres to a few km), so corners and edge midpoints
 * bound the longitude and latitude ranges.
 *
 * @param {{ anchor: { lonDeg: number }, localToWgs84: (x: number, z: number) => number[] }} bridge
 * @param {Array<{minXM: number, minZM: number, maxXM: number, maxZM: number}>} boxes  local-metre boxes
 * @returns {{ ok: true } | { ok: false, code: 'CROSSES_ANTIMERIDIAN' | 'OUTSIDE_MERCATOR_LATITUDE' | 'NON_FINITE_FOOTPRINT', reason: string }}
 */
export function checkWebMercatorFootprint(bridge, boxes, marginM = FOOTPRINT_MARGIN_M) {
  const lon0 = bridge.anchor.lonDeg;
  for (const box of boxes) {
    if (!box) continue;
    const xs = [box.minXM - marginM, (box.minXM + box.maxXM) / 2, box.maxXM + marginM];
    const zs = [box.minZM - marginM, (box.minZM + box.maxZM) / 2, box.maxZM + marginM];
    for (const x of xs) {
      for (const z of zs) {
        if (!isFiniteNumber(x) || !isFiniteNumber(z)) {
          return { ok: false, code: 'NON_FINITE_FOOTPRINT', reason: 'Footprint bounds are not finite.' };
        }
        const [lon, lat] = bridge.localToWgs84(x, z);
        if (!isFiniteNumber(lon) || !isFiniteNumber(lat)) {
          return { ok: false, code: 'NON_FINITE_FOOTPRINT', reason: 'Footprint does not project to finite coordinates.' };
        }
        const unwrapped = lon0 + ((((lon - lon0) % 360) + 540) % 360) - 180;
        if (unwrapped > 180 || unwrapped < -180) {
          return { ok: false, code: 'CROSSES_ANTIMERIDIAN', reason: 'Woodland footprint crosses the 180° meridian.' };
        }
        if (Math.abs(lat) >= MERCATOR_MAX_LAT_DEG) {
          return { ok: false, code: 'OUTSIDE_MERCATOR_LATITUDE', reason: 'Woodland footprint reaches the Web Mercator latitude limit.' };
        }
      }
    }
  }
  return { ok: true };
}
