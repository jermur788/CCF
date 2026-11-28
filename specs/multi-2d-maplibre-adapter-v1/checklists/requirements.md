# Open-Source Satellite Map Plug V1 — Requirements Checklist

| Requirement | Check | Evidence |
|---|---|---|
| MapLibre GL JS 6.20.0, BSD-3-Clause, vendored unmodified, no CDN/build | AC-1 | `verify-vendor.mjs`; `provenance.json` |
| WGS84 ellipsoidal bridge, documented axes/signs, forward + inverse, no GIS library | AC-2 | `geo-anchor.js`; `test-geo-anchor.mjs` |
| Anchor bound to source identity + geometry digest + explicit origin + finite bearing; centroid/missing/mismatch/out-of-range rejected | AC-2, B04 | `test-geo-anchor.mjs`; B04 |
| Exactly five contract methods; idempotent; generation-guarded; immutable frame; no forestry/money maths | B01, B02, B05, B06 | CDP report |
| Clean Woodland default; zero MapLibre requests until satellite + valid anchor; session-only | B03, B04, B20 | CDP report |
| Same-origin module worker set before `new Map`; strict CSP; loopback only; inline style | B07, B08, B09 | CDP report |
| 8-layer order with 4 reserved Stage 2 slots | B10, B19 | CDP report |
| Crowns at true `radiusM` (≤ 2 mm), re-measured after resize and zoom/pan | B11, B12, B13 | CDP report |
| Imagery 65 % default; 0/65/100 % affect Layer 2 only; attribution only while visible | B14, B15 | CDP report + screenshots |
| Three fallbacks with exact copy, zero modals, selection kept | B04, B16, B17 | CDP report |
| Pointer payload `{areaId, u, v, juvenileId, frameKey}` in local metres | B18 | CDP report |
| Accessible marks (shape/text, not colour alone) | B19 | CDP report |
| No `[C]`/`[E]` tags or calibration disclaimers | B20 | CDP report |
| EPSG:2157 offline provenance rules documented; no datum-shift accuracy invented | spec §6 | review |
| File fence: four prefixes only; no C#/save/shell/Unity change | AC-4 | `git diff --name-only` |

## Outside Part 1

- [ ] Independent (non-author) review of `review/multi-2d-maplibre-adapter-v1`.
- [ ] Part 2 (T-07) — **QUEUED** pending separate integrator approval.
