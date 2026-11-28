# Open-Source Satellite Map Plug V1 — Tasks and B01–B20 Matrix

**Spec:** `SPEC-multi-2d-maplibre-adapter-V1` · **Status:** `APPROVED / IMPLEMENTING`

## Part 1 (IMPLEMENTING)

| Task | Scope | Done when |
|---|---|---|
| T-01 | Vendor MapLibre GL JS 6.20.0 (4 files) + `provenance.json` | `verify-vendor.mjs` all PASS |
| T-02 | Spec quartet with locked defaults, EPSG:2157 rules, B01–B20 | this folder |
| T-03 | `geo-anchor.js` bridge + strict binding | `test-geo-anchor.mjs` all PASS (< 0.01 m) |
| T-04 | Adapter modules + `adapter.css` | B01–B20 |
| T-05 | Harness `run.sh` (vendor → geo → B01–B20) | one command, non-zero on any failure |
| T-06 | Evidence summary, handoff, fence check, commit, push feature + review branches | handoff file + `git ls-remote` |

## Part 2 (QUEUED — do not implement under this packet)

| Task | Scope | Gate |
|---|---|---|
| T-07 | `/multi` toggle + opacity control, frame builder, `onPointer` wiring, `MultiRoutes.cs`, `PlayerApi.cs` CSP, `SitkaP1.Ui.csproj` resources, anchor provenance fields | separate integrator approval |

## B01–B20 (headless Chromium over CDP, strict CSP, loopback only, 80 × 80 m 5-lane fixture, 1280 × 720)

| ID | Case | Pass condition |
|---|---|---|
| B01 | Five-method contract | real `assertMapRenderer` passes; own properties exactly the five methods (with and without the test debug hook); object frozen |
| B02 | Frame immutability | caller frame unchanged; normalised frame deep-frozen and not aliased; a deeply frozen input renders; values copied verbatim |
| B03 | Fresh session default | Clean Woodland, no notice, canvas pixel = `#15251a`; zero vendor requests and zero workers |
| B04 | No validated location | missing anchor, centroid origin, digest mismatch, frame without identity, latitude out of range, NaN bearing → exact no-location copy, selection kept, 0 modals, still zero vendor requests |
| B05 | Lifecycle idempotency | calls before mount are no-ops; double mount = one root; destroy twice and calls after destroy are safe; remount works |
| B06 | Generation guard | a delayed module import superseded by Clean Woodland constructs no map; back-to-back satellite frames show the last frame with one map |
| B07 | Lazy module + same-origin module worker | `maplibre-gl.mjs`, worker and CSS served 200 only after the switch; every worker target is exactly the vendored worker URL (no blob/data); `setWorkerUrl` precedes `new Map`; zero CSP violations / console errors |
| B08 | Headers and network | every browser response carries the exact CSP + `Referrer-Policy: no-referrer` (server 404 too); zero non-loopback and zero failed requests |
| B09 | Inline style | no glyphs, no sprite, no tiles; sources = inline GeoJSON extent + same-origin image; layers = background, extent fill, raster; both fills `#15251a` |
| B10 | Layer order | DOM `data-layer-index` 1..8 matches `LAYER_STACK`; four reserved slots present and empty; MapLibre order background < extent < raster; overlay above canvas; Layer 8 topmost |
| B11 | True-metre crowns | 20 crowns, `r` = `radiusM`; 24 rim points per crown taken through the rendered screen CTM, then MapLibre `unproject` plus the inverse bridge, are within **≤ 2 mm** of `radiusM`; centres within 0.5 px of `map.project` |
| B12 | Re-measure after resize | at 800 × 500 the satellite error stays ≤ 2 mm while pixel radius changes > 20 %; Clean Woodland transform is uniform, error ≤ 2 mm, pixel radius changes > 20 % |
| B13 | Zoom/pan alignment | after zoom +0.8 and a pan: centres < 0.5 px, radii ≤ 2 mm |
| B14 | Default 65 % imagery | `raster-opacity` 0.65 with no opacity in the frame; margin pixel matches 0.65·imagery + 0.35·canvas within ±4; exact attribution visible |
| B15 | 0 % and 100 % | paint 0 → pixel = canvas, attribution hidden; paint 1 → pixel = imagery, attribution shown; Layers 1, 4–8 and the overlay keep opacity 1 |
| B16 | Imagery unavailable | no imagery / non-image URL → dark MapLibre canvas pixel, exact imagery copy, no raster layer, attribution hidden, selection kept, 0 modals, crowns drawn |
| B17 | Display unavailable | WebGL unavailable (no vendor request), module import failure, worker never answers (readiness watchdog), context lost on a live map, then a later satellite frame → exact display copy, Clean Woodland, selection kept, 0 modals |
| B18 | Pointer payload | in both styles, clicks on a crown, a sapling and a crown in another lane give exactly `{areaId, u, v, juvenileId, frameKey}` in local metres; a 64 px drag pans with no callback |
| B19 | Layer content and focus | 5 lane borders; 1120 moisture cells (576 dry / 544 moist), each 2.5 × 2.5 m; 5 logs; 20 crowns across 5 statuses; 5 saplings (1 shelter); crop ring + "C", dashed pending vs solid approved ring, ranks 1–2, selection frame + "SEL" + aria label last in Layer 8; `focusArea` moves the view to lane centres x = 8 / 72 m without changing the frame or selection |
| B20 | Session-only and audit | zero storage writes, cookies empty, no `[C]`/`[E]`/disclaimers; a fresh load is Clean Woodland with zero vendor requests and workers; whole run: 0 console errors, 0 CSP violations, 0 failed, 0 non-loopback |
