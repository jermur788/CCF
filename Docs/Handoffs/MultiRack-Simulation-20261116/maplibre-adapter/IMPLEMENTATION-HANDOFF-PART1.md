# Open-Source Satellite Map Plug — MapLibre Display Adapter + Geo-Anchor Bridge — Part 1 Implementation Handoff

```text
IMPLEMENTATION HANDOFF
Task: CCF 2D Simulator — Playable Milestone 7 (Part 1), fallback builder
Worker: Claude Opus 5.5 (claude-opus-5-5), effort as configured by the dispatching thread; sole implementation writer.
        Took over an interrupted, uncommitted draft by Gemini 4 Neon (high), which stopped on a provider quota error.
        The draft was audited and largely rewritten (see "Audit of the inherited draft"). Neither author has reviewed the other's work.
Branch: feat/multi-2d-maplibre-adapter-v1   (review copy: review/multi-2d-maplibre-adapter-v1)
Base: b2006767cbe70fa2c822497470908251a0fd6ffd (origin/integration/2d-sim, Scots Pine Part 2 merged green)
Context: origin/main = bdeb6a324acaf4129c10fb0846faac8b64974360 (untouched)
Code under test: b6cb1461fc28470cff064f55aad8934d71c8080a (spec 1ec9649a, adapter dc735173, harness b6cb1461)
HEAD: the commit that adds this file (docs only; no code change after b6cb1461)
GITHUB HANDOFF PATH: https://github.com/jermur788/CCF/blob/feat/multi-2d-maplibre-adapter-v1/Docs/Handoffs/MultiRack-Simulation-20261116/maplibre-adapter/IMPLEMENTATION-HANDOFF-PART1.md
DURABLE EVIDENCE: /media/jer/Files/CCF/Verification/Multi2DMapLibreAdapterPart1-20261128/run-20261128T120314Z/
```

**State (D-030):** isolated branch implemented and verified by its own harness. NOT merged to integration/2d-sim or main. Not wired into `/multi`. Part 2 remains **QUEUED**.

## What changed

- **Spec quartet** `specs/multi-2d-maplibre-adapter-v1/`, status `APPROVED / IMPLEMENTING`.
  - Part 2 is explicitly QUEUED: live `/multi` toggle, `MultiRoutes.cs`, `PlayerApi.cs` CSP, `SitkaP1.Ui.csproj` resources.
  - The four locked defaults are recorded: 8-layer stack; 65 % imagery-only opacity; exact fallback copy; session-only style (Clean Woodland default, no MapLibre load until switched).
  - Also included: B01–B20 definitions in `tasks.md`, and an offline EPSG:2157 provenance section (spec §6). That section lists the parameters and the datum-shift rules, invents no datum-shift accuracy, and states that Part 1 consumes WGS84 anchors only.
- **Vendored MapLibre GL JS 6.20.0** (BSD-3-Clause), unmodified:
  - `maplibre-gl.mjs`, `maplibre-gl-worker.mjs`, `maplibre-gl.css`, `LICENSE.txt` and `provenance.json`;
  - `provenance.json` records the tarball SHA-256, the registry `integrity`/`shasum`, per-file SHA-256 and size, how the files were extracted, and that the 0-byte `maplibre-gl-shared.mjs` was omitted.
- **Adapter** (`map/maplibre/`):
  - `createMapLibreAdapter()` has exactly the five contract methods. The style is chosen per frame (`frame.mapStyle`), so there is no sixth method.
  - Layers 3–8 are one SVG overlay in metres, placed by a single affine transform. In Clean Woodland that is a uniform scale; in Satellite it is the projected Jacobian. A crown is therefore `<circle r = radiusM>` in both styles.
  - MapLibre is imported lazily, only for satellite with a valid anchor. `setWorkerUrl(...)` runs before `new Map`.
  - The MapLibre style is inline only: a background, an invisible same-colour GeoJSON extent fill that proves the worker is alive, and the optional image/raster.
  - Three fallbacks with the exact copy; a readiness watchdog; a sticky display-failure state.
  - No shared-shell imports.
- **Geo-anchor bridge** `geo-anchor.js`:
  - WGS84 geodetic ↔ ECEF ↔ ENU with true-north clockwise bearing, forward and inverse.
  - Bound to the frame's `sourceId` and `geometryDigest`. `originKind` must be on an explicit allow-list (`survey-corner | survey-point | site-origin`). Centroid-derived anchors, missing identity, mismatches, out-of-range lon/lat and non-finite bearing are all rejected.
- **Harness** `tests/maplibre-adapter/`: `run.sh` runs vendor integrity, then the geo oracle, then the strict-CSP offline headless-Chromium matrix B01–B20. It writes evidence outside the repo plus `evidence/summary.json` here.

## Files changed (vs base b2006767)

`git diff --name-only b2006767...HEAD` gives 31 files, all inside the four authorised prefixes:

- `specs/multi-2d-maplibre-adapter-v1/{spec.md, plan.md, tasks.md, checklists/requirements.md}`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/map/maplibre/{adapter.js, adapter.css, clean-woodland.js, frame-projector.js, geo-anchor.js, imagery-policy.js, layer-stack.js, overlay-layers.js}`
- `Tools/MultiRackSitkaP1Lab/src/Ui/multi/map/maplibre/vendor/maplibre-gl-6.20.0/{maplibre-gl.mjs, maplibre-gl-worker.mjs, maplibre-gl.css, LICENSE.txt, provenance.json}`
- `Tools/MultiRackSitkaP1Lab/tests/maplibre-adapter/{run.sh, server.mjs, drive-maplibre-cdp.mjs, harness.html, harness.js, harness.css, fixture-80x80.mjs, synthetic-png.mjs, test-geo-anchor.mjs, verify-vendor.mjs, write-summary.mjs, fixtures/silent-worker.mjs}`
- `Docs/Handoffs/MultiRack-Simulation-20261116/maplibre-adapter/{IMPLEMENTATION-HANDOFF-PART1.md, evidence/summary.json}`

No existing file was modified. Nothing was touched among: the shared shell (`multi.html`, `main.js`, `store.js`, `map/contract.js`, `map/renderer-svg.js`, `map/minimap.js`, …), C#, `.csproj`, `wwwroot/`, save data, Unity, `.meta`, `build/` or `Docs/Project/`.

## Player-facing result

None in the live game yet: Part 1 is a standalone module that `/multi` does not load.

In the harness page the player-equivalent view works as follows (screenshots are in the evidence folder):

- The woodland opens in **Clean Woodland**: dark `#15251a` canvas, lane borders, moisture cells, logs, crowns at true size, saplings, crop/fell marks, ranks and selection.
- Switching a frame to **Satellite** (with a valid anchor) shows the same overlay over a MapLibre canvas, with the local imagery at 65 % and its attribution.
- If something is missing, the map degrades to a one-line notice instead of a popup, and the selection stays.

## Simulation / save impact

None. Presentation only (D-020): no forestry, competition, growth or money maths. No save fields. No C# changes. No RNG or engine changes.

## Verification (run on clean commit b6cb1461, 2026-11-28T12:03Z)

Command: `Tools/MultiRackSitkaP1Lab/tests/maplibre-adapter/run.sh` (exit 0). Environment: Linux node v26.13.1 (`~/.local/share/vite-plus/bin`), Chromium 156.0.8078.79, WebGL2 through ANGLE/SwiftShader (Vulkan 1.3.0, SwiftShader Device (Subzero)). The full transcript is `run-sh-transcript.txt` in the evidence folder. Verbatim summary lines:

```text
VENDOR_INTEGRITY_SUMMARY: 16 passed, 0 failed, files_checked=4
GEO_ORACLE_SUMMARY: 28 passed, 0 failed, max_roundtrip_m=7.3065e-8, oracle_30m=3.9981e-7, oracle_80m=4.4374e-7, oracle_120m=3.3160e-7
WEBGL2_READY {"ok":true,"version":"WebGL 2.0 (OpenGL ES 3.0 Chromium)","renderer":"ANGLE (Google, Vulkan 1.3.0 (SwiftShader Device (Subzero) (0x0000C0DE)), SwiftShader driver)"}
PASS B01: assertMapRenderer passes; own properties=[destroy,focusArea,mount,render,setSelection]; production adapter (no debug) properties=[destroy,focusArea,mount,render,setSelection]; frozen=true
PASS B02: callerFrameUnmutated=true, normalisedFrameDeepFrozen=true, copiedNotAliased=true, frozenInputAccepted=true, radiusCopiedVerbatim=true
PASS B03: style=clean-woodland, notice hidden=true, canvas pixel=(21.0,37.0,26.0) (want #15251a), layer1=rgb(21, 37, 26), vendorRequests=0, workers=0, crowns=20
PASS B04: 6/6 invalid anchors -> "No map location set. Showing Clean Woodland." with selection kept, 0 modals [missing anchor:ok; centroid origin:ok; digest mismatch:ok; frame without source identity:ok; latitude out of range:ok; non-finite bearing:ok]; vendorRequests=0
PASS B05: callsBeforeMount safe=true; roots after double mount=1, after destroy=0; second destroy + calls after destroy safe=true (roots=0); remount roots=1
PASS B06: stale delayed module load ignored (style=clean-woodland, map=false, constructed=0); back-to-back satellite frames -> shown frameKey=key-B, maps constructed=1
PASS B07: module/worker/css 200 after switch; worker targets=1 all at /map/maplibre/vendor/maplibre-gl-6.20.0/maplibre-gl-worker.mjs (no blob/data); getWorkerUrl matches; setWorkerUrl before new Map=true; CSP violations=0; console errors=0
PASS B08: 18 browser responses all carry the exact CSP + Referrer-Policy (mismatches=0); server 404 also exact=true; http requests=19, non-loopback=0, failed=0
PASS B09: glyphs=null, sprite=null, sources=["geojson inline data","image /fixtures/synthetic-imagery.png"], layers=[ccf-woodland-canvas:background,ccf-woodland-extent:fill,ccf-georeferenced-imagery:raster], background=#15251a, extent fill=#15251a (Layer 1, worker-tiled)
PASS B10: DOM bottom->top [woodland-canvas > georeferenced-imagery > stage2-reserved > ground-overlays > schematic-deadwood > living-crowns > planted-juveniles > marks-ranks-selection] matches descriptors; reserved slots drone-lidar=false/0, hillshade-contours=false/0, streams-rivers=false/0, roads-bridges-paths=false/0; MapLibre canvas order background<extent fill (Layer 1)<raster (Layer 2); overlay above canvas=true; marks topmost=true
PASS B11: 20 crowns, r attribute == radiusM metres: true; rendered rim -> MapLibre unproject -> local metres: max radius error 0.2115 mm (<= 2 mm); centre vs map.project max 0.0017 px; MA-C-T03 (3.8 m) = 31.92 px
PASS B12: satellite 800x500: max error 0.2111 mm, MA-C-T03 31.92 -> 21.47 px (x0.673); clean woodland uniform scale, max error 0.0001 mm, 31.09 px @1280x720 -> 21.59 px @800x500 (x0.694)
PASS B13: after zoom +0.8 and pan (120,-60) px: centre error max 0.0030 px (< 0.5), radius error max 0.2095 mm
PASS B14: default raster-opacity=0.65; imagery pixel (135.0,122.0,69.0) vs 65% blend (134.8,122.2,68.9) (+-4); attribution visible "Synthetic local test imagery (not a real photograph)"
PASS B15: 0%: paint=0, pixel (21.0,37.0,26.0) (= canvas #15251a), attribution hidden=true; 100%: paint=1, pixel (196.0,168.0,92.0) (= imagery), attribution visible=true; layers 1,4-8 + overlay opacity stay 1=true
PASS B16: 2/2 -> "Imagery unavailable. Showing woodland map." on the dark MapLibre canvas (pixels (21.0,37.0,26.0), (21.0,37.0,26.0)), no raster layer, attribution hidden, selection kept, 0 modals [no imagery configured:ok; imagery URL returns a non-image:ok]
PASS B17: 5/5 -> "Map display unavailable. Showing Clean Woodland.", selection kept, 0 modals [WebGL unavailable (no vendor request):ok; module import failure:ok; worker never responds (load watchdog):ok; WebGL context lost on a live map:ok; later satellite frame stays on Clean Woodland (no retry loop):ok]
PASS B18: satellite [["MA-C",8.2,36,null],["MA-C",5.5,44,"MA-C-PJ01"],["MA-E",4.5,18,null]]; clean woodland [["MA-C",8.2,36,null],["MA-C",5.5,44,"MA-C-PJ01"],["MA-E",4.5,18,null]]; keys exactly {areaId,u,v,juvenileId,frameKey}; 64 px drag panned the map=true and emitted 0 callbacks
PASS B19: L4 lanes=5, moisture cells dry=576/moist=544, cell 2.50 x 2.50 m; L5 deadwood=5; L6 crowns=20 [ApprovedFell,Felled,Living,NaturalDead,PendingFell]; L7 juveniles=5 (shelter 1); L8 crop ring+C=true, pending dashed=true/approved solid=true, ranks=1,2, selection on top with frame+SEL+aria=true; focusArea centres x=8.00 (MA-A) -> 72.00 (MA-E), frame unchanged=true, selection kept=true
PASS B20: storage writes=0, local/session items=0/0, cookies="", no [C]/[E]/disclaimers=true; fresh load style=clean-woodland, vendor requests=0, workers=0; whole run: console errors=0, CSP violations=0, failed requests=0, non-loopback=0
CDP_MATRIX_SUMMARY: 20 passed, 0 failed, total=20, console_errors=0, csp_violations=0, failed_requests=0, non_loopback=0
OVERALL: PASS
```

| Gate | Result |
|---|---|
| Vendor integrity | **16/16 PASS**: 4 files hashed (SHA-256 + size), file list, no static imports, shared module unreferenced, CSS `data:` only |
| Upstream re-check (one-off, network) | tarball re-downloaded; SHA-256 equals `provenance.json`; all 4 files byte-identical (`upstream-reverification.txt`) |
| Geo oracle | **28/28 PASS**: max round trip 7.3e-8 m; Vincenty max 4.0e-7 m (30 m), 4.4e-7 m (80 m), 3.3e-7 m (120 m); both axis azimuths 3.7e-9°; 20 rejection cases + 1 accept |
| B01–B20 | **20/20 PASS**; 0 console errors (page + workers), 0 CSP violations, 0 failed requests, 0 non-loopback requests |
| Harness sensitivity (mutants, temp copy) | screen-isotropic crowns → B11/B12/B13 FAIL (72.5 mm); attribution always on → B15 FAIL; eager MapLibre import → B03/B04/B17/B20 FAIL (logs in `mutants/`) |

**Exact CSP header** (sent on every response, including 404):
`default-src 'none'; script-src 'self'; style-src 'self'; connect-src 'self'; worker-src 'self'; img-src data: blob: 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'` plus `Referrer-Policy: no-referrer`.

**Network fence:** `--host-resolver-rules="MAP * ~NOTFOUND, EXCLUDE 127.0.0.1"`, plus a dead proxy (`--proxy-server=http://127.0.0.1:9`) so that even IP-literal outbound requests would fail and be counted. Ports are ephemeral loopback only; the protected listeners 33767/43649/46593 were not used. Chromium runs in its own process group and is stopped by the driver, with a `trap EXIT` safety net. No Chromium process was left after any run.

## Audit of the inherited draft (what was wrong and fixed)

- `Object.freeze(renderer)` was followed by `Object.defineProperty(renderer, …)`, which throws a TypeError, so the adapter could not be created at all. The debug handle is now opt-in (`exposeDebug`) and added before freezing; production adapters have exactly five own properties.
- MapLibre `move`/`load` handlers were generation-guarded, so any later `render` stopped the overlay following pans and could leave the map never marked loaded. They are now guarded by map-instance identity.
- Crowns were drawn as screen circles from averaged east/north scales. Web Mercator on WGS84 is anisotropic (ν/ρ ≈ 1.0024 at 53°N, *computed*), which misses 2 mm on large crowns. The overlay now uses the projected Jacobian, measured at 0.21 mm.
- The anchor binding was skipped when the frame had no identity, and the centroid check was a blacklist. Both are now strict: the frame binding is required and `originKind` must be on an allow-list.
- The normaliser invented values (radius 1 m, log length 3.2 m, species `sitka-spruce`) and the overlay invented moisture classes not used elsewhere. Incomplete records are now skipped; moisture styling covers only the two classes the shell knows.
- Imagery-unavailable was tested with a deliberate 404, while the auditor ignored *any* network 404 log line. The 404 is replaced by a 200 non-image response, so the "zero failed requests" rule stays strict.
- MapLibre drew its own lane fill/outline over the imagery, tinting it. It was removed; the lanes are drawn only in the SVG overlay.
- The worker-failure case was untestable because the background-only style never needs the worker. The invisible extent source plus a readiness watchdog now make worker failure detectable (B17).
- `run.sh` used `node -e` with ESM imports and had a no-op cleanup trap. Replaced by `write-summary.mjs` and a real process-group cleanup.

## Deviations and interpretations (for the reviewer)

1. **Attribution at 0 %.** The brief says both "attribution only while imagery visible" and "imagery visible with attribution at 0/65/100 %". At 0 % the imagery is not visible, so the attribution is **hidden** (B15 asserts this). Flip `shouldShowAttribution` if the owner wants it shown at 0 %.
2. **Layer 1 in Satellite** also contains an inline GeoJSON woodland-extent fill in `#15251a` (invisible against the background) so worker liveness is observable. MapLibre's layer list is therefore background → extent fill → raster.
3. **Layer 2 DOM marker.** The imagery is drawn inside the MapLibre canvas. The `data-layer-index="2"` element is a zero-size state marker (`data-imagery-visible`, `data-imagery-opacity`); B09/B10 also assert the canvas layer order.
4. **Satellite camera bearing = anchor `bearingDeg`**, so the woodland stays upright in both styles (the map is not north-up when the bearing ≠ 0).
5. **Display failure is sticky** for the adapter's lifetime (no retry loop); a new adapter retries.
6. **Pointer outside every area** falls back to the active area (same as `renderer-svg.js resolveMapPoint`), and the callback name is `deps.onPointer`. Part 2 maps it to the shell's selection logic.
7. **2 mm is a geometric criterion.** At about 8 px/m, 2 mm is far below one pixel. B11–B13 measure it through the rendered SVG transform and MapLibre's own `unproject` + the inverse bridge, not by counting screenshot pixels. The opacity checks *are* pixel comparisons.
8. One extra file outside the brief's list: `write-summary.mjs`. Two extra adapter files: `overlay-layers.js` and `clean-woodland.js` (focused siblings, allowed by the brief).

## NOT TESTED / skipped

- **Skipped by instruction:** .NET builds/tests, `Phase1.Save.Tests`, `build/2d-gate.sh`, `build/2d-gate/selftest.sh`, Unity.
- **Hardware GPU WebGL:** only SwiftShader was exercised.
- **Other browsers** (Firefox/Safari): not tested.
- **Touch input:** not tested.
- **Live `/multi` integration, the C# route/CSP/resource path, and real anchors or real imagery:** Part 2, QUEUED.
- **Real-world accuracy:** imagery georeferencing accuracy and datum-shift accuracy are out of scope. The fixture is synthetic and only proves internal consistency.
- **Part 2 extents:** stands larger than about 120 m. The affine error grows roughly with the square of the distance from the woodland centre; not measured beyond the 80 m fixture in the browser.

## Next authorised step

An independent non-author review of `review/multi-2d-maplibre-adapter-v1` against this handoff and spec. Part 2 stays QUEUED until an integrator approves a separate packet.

NOT merged to integration/2d-sim or main.
