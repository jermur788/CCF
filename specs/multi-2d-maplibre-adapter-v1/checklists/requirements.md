# Open-Source Satellite Map Plug V1 — Requirements Checklist

| Requirement | Check | Evidence |
|---|---|---|
| MapLibre GL JS 6.20.0, BSD-3-Clause, vendored unmodified, no CDN/build | AC-1 | `verify-vendor.mjs`; `provenance.json` |
| WGS84 ellipsoidal bridge, documented axes/signs, forward + inverse, no GIS library | AC-2 | `geo-anchor.js`; `test-geo-anchor.mjs` |
| Anchor bound to source identity + geometry digest + explicit origin + finite bearing; centroid/missing/mismatch/out-of-range rejected | AC-2, B04 | `test-geo-anchor.mjs`; B04 |
| Exactly five contract methods; idempotent; generation-guarded; immutable frame; no forestry/money maths | B01, B02, B05, B06 | CDP report |
| Caller objects never frozen/mutated/aliased; empty frame clears overlay + hit-test transform | B23 | CDP report (repair cycle 1) |
| Pending imagery probe cannot undo the sticky display fallback; stuck module/probe bounded | B22 | CDP report (repair cycle 1) |
| Whole imagery probe (fetch, body, decode) settles at its deadline; a late decode is discarded; timeout cached per URL | B27 | CDP report (repair cycle 2; render-relative bound relaxed in repair cycle 3) |
| A decode whose continuation runs before the overdue deadline timer is still rejected (elapsed-time check); no resurrection after a healthy same-instance remount | B30 | CDP report (repair cycle 3) |
| Ineligible imagery (policy or its own footprint) is dropped before the stand footprint preflight | B29 | CDP report (repair cycle 2) |
| Antimeridian (east/west) and Mercator-limit footprints rejected before MapLibre; near-antimeridian and 84.9° N stay ≤ 2 mm | B21; geo oracle footprint cases | CDP report + `test-geo-anchor.mjs` |
| Hidden-lane focus keeps the view; same-instance destroy + mount resets display state | B26 | CDP report (repair cycle 1; Satellite half isolated in repair cycle 2) |
| A frame that hides the focused lane re-frames the visible woodland in both styles | B28 | CDP report (repair cycle 2) |
| Clean Woodland default; zero MapLibre requests until satellite + valid anchor; session-only | B03, B04, B20 | CDP report |
| Same-origin module worker set before `new Map`; strict CSP; loopback only; inline style | B07, B08, B09 | CDP report |
| Exact CSP + Referrer-Policy on every response of the whole run (page, workers, server, 404), coverage and sensitivity; per-worker-target startup coverage by CDP session/target id and resource type `Script` (a same-URL `Fetch` never counts, in-run self-test); audit setup failures fail loudly | B08, B20, B24 | CDP report + `browser-responses.json` |
| 8-layer order with 4 reserved Stage 2 slots | B10, B19 | CDP report |
| Crowns at true `radiusM` (≤ 2 mm), re-measured after resize and zoom/pan | B11, B12, B13 | CDP report |
| Imagery 65 % default; 0/65/100 % affect Layer 2 only; attribution only while visible (incl. off-screen imagery) | B14, B15, B25 | CDP report + screenshots |
| Three fallbacks with exact copy, zero modals, selection kept | B04, B16, B17 | CDP report |
| Pointer payload `{areaId, u, v, juvenileId, frameKey}` in local metres | B18 | CDP report |
| Accessible marks (shape/text, not colour alone) | B19 | CDP report |
| No `[C]`/`[E]` tags or calibration disclaimers | B20 | CDP report |
| EPSG:2157 offline provenance rules documented; no datum-shift accuracy invented | spec §6 | review |
| File fence: four prefixes only; no C#/save/shell/Unity change | AC-4 | `git diff --name-only` |

## Outside Part 1

- [ ] Independent (non-author) review of `review/multi-2d-maplibre-adapter-v1`.
- [ ] Part 2 (T-07) — **QUEUED** pending separate integrator approval.
