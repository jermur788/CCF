/**
 * Locked product defaults, imagery policy, opacity clamping, attribution gating and the exact
 * plain-English fallback copy (SPEC-multi-2d-maplibre-adapter-V1 §2 and §7).
 */

export const DEFAULT_CANVAS_COLOR = '#15251a';
/** Imagery (Layer 2) opacity as a 0..1 fraction. 0.65 = the locked 65% default. Affects imagery only. */
export const DEFAULT_IMAGERY_OPACITY = 0.65;
export const DEFAULT_MAP_STYLE = 'clean-woodland';
export const VALID_MAP_STYLES = Object.freeze(['clean-woodland', 'satellite']);

export const FALLBACK_CODES = Object.freeze({
  NONE: 'none',
  NO_LOCATION: 'no-location',
  IMAGERY_UNAVAILABLE: 'imagery-unavailable',
  DISPLAY_UNAVAILABLE: 'display-unavailable',
});

export const FALLBACK_COPY = Object.freeze({
  [FALLBACK_CODES.NONE]: '',
  [FALLBACK_CODES.NO_LOCATION]: 'No map location set. Showing Clean Woodland.',
  [FALLBACK_CODES.IMAGERY_UNAVAILABLE]: 'Imagery unavailable. Showing woodland map.',
  [FALLBACK_CODES.DISPLAY_UNAVAILABLE]: 'Map display unavailable. Showing Clean Woodland.',
});

// Guardrail 7: evidence tags and calibration disclaimers never appear on the live screen.
const GUARDRAIL7_FORBIDDEN_RE = /(\[(?:C|E)\]|calibration disclaimer|not empirically validated)/i;

/** Anything other than an explicit 'satellite' means the session default, Clean Woodland. */
export function normalizeMapStyle(raw) {
  return raw === 'satellite' ? 'satellite' : DEFAULT_MAP_STYLE;
}

/** Clamp imagery opacity to [0, 1]; missing or non-finite values give the 0.65 default. */
export function normalizeImageryOpacity(raw) {
  if (typeof raw !== 'number' || !Number.isFinite(raw)) return DEFAULT_IMAGERY_OPACITY;
  return Math.min(1, Math.max(0, raw));
}

/**
 * Validate a local, same-origin, georeferenced imagery descriptor.
 * Remote origins, non-http(s) URLs, missing local bounds, missing attribution and
 * Guardrail-7 text are all rejected (the adapter then shows the imagery-unavailable fallback).
 * @param {{url: string, attribution: string, boundsLocal: object|null} | null} imagery
 * @param {string} pageUrl  The page URL; only its origin is used.
 */
export function validateImagerySource(imagery, pageUrl) {
  if (!imagery) return { allowed: false, reason: 'No imagery source configured.' };
  if (!imagery.url || !imagery.url.trim()) return { allowed: false, reason: 'Imagery URL is missing.' };
  let resolved;
  try {
    resolved = new URL(imagery.url.trim(), pageUrl);
  } catch {
    return { allowed: false, reason: 'Imagery URL could not be parsed.' };
  }
  if (resolved.protocol !== 'http:' && resolved.protocol !== 'https:') {
    return { allowed: false, reason: 'Imagery URL must be same-origin http(s).' };
  }
  if (resolved.origin !== new URL(pageUrl).origin) {
    return { allowed: false, reason: 'Imagery from another origin is not allowed (offline, same-origin only).' };
  }
  if (!imagery.boundsLocal) return { allowed: false, reason: 'Imagery needs boundsLocal (its georeference in local metres).' };
  const attribution = (imagery.attribution || '').trim();
  if (!attribution) return { allowed: false, reason: 'Imagery attribution text is required.' };
  if (GUARDRAIL7_FORBIDDEN_RE.test(attribution)) {
    return { allowed: false, reason: 'Imagery attribution must not contain [C]/[E] tags or calibration disclaimers.' };
  }
  return { allowed: true, url: resolved.href, attribution, boundsLocal: imagery.boundsLocal };
}

/** Attribution is shown only while imagery is actually visible. */
export function shouldShowAttribution({ activeStyle, imageryLoaded, imageryOpacity, attribution }) {
  return activeStyle === 'satellite' && imageryLoaded === true && imageryOpacity > 0
    && typeof attribution === 'string' && attribution.trim().length > 0;
}
