# Open-Source Satellite Map Plug V1 — Tasks and B01–B26 Matrix

**Spec:** `SPEC-multi-2d-maplibre-adapter-V1` · **Status:** `APPROVED / IMPLEMENTING`

## Part 1 (IMPLEMENTING)

| Task | Scope | Done when |
|---|---|---|
| T-01 | Vendor MapLibre GL JS 6.20.0 (4 files) + `provenance.json` | `verify-vendor.mjs` all PASS |
| T-02 | Spec quartet with locked defaults, EPSG:2157 rules, B01–B26 | this folder |
| T-03 | `geo-anchor.js` bridge + strict binding | `test-geo-anchor.mjs` all PASS (< 0.01 m) |
| T-04 | Adapter modules + `adapter.css` | B01–B26 |
| T-05 | Harness `run.sh` (vendor → geo → B01–B26) | one command, non-zero on any failure |
| T-06 | Evidence summary, handoff, fence check, commit, push feature + review branches | handoff file + `git ls-remote` |
| T-06R | Repair cycle 1 (independent review REQUEST CHANGES): sticky fallback vs pending imagery, antimeridian/Mercator footprint, caller-data integrity + empty-frame clearing, whole-run header gate, off-screen attribution, hidden focus + same-instance remount, B01 wording | B21–B26 pass; reviewer's four mutations fail on disposable copies; handoff "Repair cycle 1" section |

## Part 2 (QUEUED — do not implement under this packet)

| Task | Scope | Gate |
|---|---|---|
| T-07 | `/multi` toggle + opacity control, frame builder, `onPointer` wiring, `MultiRoutes.cs`, `PlayerApi.cs` CSP, `SitkaP1.Ui.csproj` resources, anchor provenance fields | separate integrator approval |

## B01–B26 (headless Chromium over CDP, strict CSP, loopback only, 80 × 80 m 5-lane fixture, 1280 × 720)

`B08`, `B20` and `B24` are evaluated after every other case, so they cover the whole run. Results are reported in case-id order.

| ID | Case | Pass condition |
|---|---|---|
| B01 | Five-method contract | real `assertMapRenderer` passes. A production adapter has exactly five own keys (`Reflect.ownKeys`): the five string-keyed methods and no symbols. The opt-in debug adapter has the same five string-keyed own properties plus exactly two non-enumerable symbols (`ccf.maplibre.debug`, `ccf.maplibre.debug.actions`). Object frozen |
| B02 | Frame immutability | caller frame unchanged; normalised frame deep-frozen and not aliased; a deeply frozen input renders; values copied verbatim |
| B03 | Fresh session default | Clean Woodland, no notice, canvas pixel = `#15251a`; zero vendor requests and zero workers |
| B04 | No validated location | missing anchor, centroid origin, digest mismatch, frame without identity, latitude out of range, NaN bearing → exact no-location copy, selection kept, 0 modals, still zero vendor requests |
| B05 | Lifecycle idempotency | calls before mount are no-ops; double mount = one root; destroy twice and calls after destroy are safe; remount works |
| B06 | Generation guard | a delayed module import superseded by Clean Woodland constructs no map; back-to-back satellite frames show the last frame with one map |
| B07 | Lazy module + same-origin module worker | `maplibre-gl.mjs`, worker and CSS served 200 only after the switch; every worker target is exactly the vendored worker URL (no blob/data); `setWorkerUrl` precedes `new Map`; zero CSP violations / console errors |
| B08 | Headers and network (whole run) | at the end of the run, every response carries the exact CSP + `Referrer-Policy: no-referrer`: browser page sessions, worker sessions, every server-written response (including held/aborted imagery), and the node-side 404. Zero non-loopback and zero failed requests |
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
| B20 | Session-only and audit | zero storage writes, cookies empty, no `[C]`/`[E]`/disclaimers; a fresh load is Clean Woodland with zero vendor requests and workers; whole run (B01–B26): 0 console errors, 0 CSP violations, 0 failed, 0 non-loopback |
| B21 | Antimeridian and high latitude | east crossing (anchor 179.9996° E), west crossing (179.9996° W, stand west of origin) and the Mercator limit (85.0508° N) → Clean Woodland + exact display copy, correct `footprint-unsupported` code, no map constructed, not sticky, selection kept, crowns finite, on stage, with one px/m ratio (spread < 5 %). Near-antimeridian 179.99° E/W and 84.9° N → Satellite with radius ≤ 2 mm, centres < 0.5 px, sane spread. A supported frame afterwards uses Satellite |
| B22 | Pending imagery vs display failure | (a) live map, new imagery request held by the server, WebGL context lost: the probe is aborted, and after the server releases the request the fallback and notice persist with no map rebuilt; a later satellite frame stays on Clean Woodland. (b) Startup with imagery held: Satellite comes up and the imagery notice appears after `imageryTimeoutMs` (< watchdog); release changes nothing. (c) A module import that never finishes → display fallback by the watchdog; the late import constructs no map |
| B23 | Caller integrity and empty frames | a caller frame with nested anchor provenance stays unchanged, unfrozen and mutable; the adapter's anchor copy is frozen and has no `provenance`; a non-primitive `lonDeg` is rejected without freezing the caller. `areas: []` (satellite and clean frames) leaves 0 crowns, empty Layers 4–8, no selection marks, no transform/matrix, 4 reserved slots, and a click at an old crown emits nothing; a populated frame afterwards draws again and clicks carry the new `frameKey` |
| B24 | Header-gate coverage and sensitivity | the browser saw the required response kinds (page, modules, CSS, vendor module/worker/CSS, imagery, non-image imagery, silent-worker fixture). Each worker target reported its own script response in its worker session. Held imagery and 404s are checked server-side. No server response is unaccounted for. Dropping CSP or Referrer-Policy from the last response, or CSP from the silent-worker response, is detected |
| B25 | Off-screen attribution | imagery loaded ~2 km outside the view at 65 % → attribution hidden and `data-imagery-visible=false`; after zooming out until it is on screen → attribution visible; after zooming back → hidden |
| B26 | Focus and remount lifecycle | `focusArea` on a lane hidden by `visibleAreaIds` moves the visible lane by < 0.5 px in both styles. `destroy` + `mount` on the same instance after Satellite → Clean Woodland, no fallback, notice and attribution hidden, no crowns, no map canvas, watchdog off; the next satellite frame rebuilds a map (constructed = 2) with centres < 0.5 px and attribution visible |
