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
Code under test (repair cycle 2): ce7732a93d01ebd1fb37167591ca787d7000a225 (fix(2d-sim): repair cycle 2)
  Repair cycle 1 code: e4b33fc1066a83d4eb2b03c7afccd268dbad4021 (delta re-review: REQUEST CHANGES, 2 blocking + 2 non-blocking + 1 nit)
  Original Part 1 code: b6cb1461fc28470cff064f55aad8934d71c8080a (reviewed by GPT-6.2 Sol: REQUEST CHANGES)
HEAD: the commit that updates this file (docs + evidence summary only; no code change after ce7732a9)
GITHUB HANDOFF PATH: https://github.com/jermur788/CCF/blob/feat/multi-2d-maplibre-adapter-v1/Docs/Handoffs/MultiRack-Simulation-20261116/maplibre-adapter/IMPLEMENTATION-HANDOFF-PART1.md
DURABLE EVIDENCE (repair cycle 2): /media/jer/Files/CCF/Verification/Multi2DMapLibreAdapterPart1-20261128/run-20261128T133605Z/
  (mutants: run-20261128T133605Z/mutants/; repair cycle 1: run-20261128T125152Z/; original Part 1 run: run-20261128T120314Z/)
```

**State (D-030):** isolated branch, implemented, repaired twice after independent review (repair cycle 2 answers the delta re-review), and re-verified by its own harness. It still needs an independent non-author delta re-review. NOT merged to integration/2d-sim or main. Not wired into `/multi`. Part 2 remains **QUEUED**.

## Repair cycle 2 (after the delta re-review: REQUEST CHANGES)

Sole writer: Claude Opus 5.5. Fix commit `ce7732a9`. Same file fence; no C#, shell, save, Unity, `.meta`, build, package or `Docs/Project` change. No existing case was deleted or loosened. B24 and B26 were strengthened, and B27–B29 were added.

Gate run on clean `ce7732a9` (`run.sh`, exit 0, 2026-11-28T13:36Z):

- vendor integrity **16/16**;
- geo oracle **38/38**;
- browser matrix **B01–B29 29/29**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests, 0 audit setup failures, 0 unanswered requests;
- 165 browser and 169 server responses plus the node-side 404, all with the exact headers;
- 22 worker targets, each matched to its own script response.

| # | Finding | Fix | Regression test | Evidence (run-20261128T133605Z) |
|---|---|---|---|---|
| B1 | A stalled `createImageBitmap` (after the image bytes arrived) kept the probe pending past its deadline. The imagery notice appeared only when the decode was released | `probeImagery` is now one settle-once promise. The deadline timer settles it to *unavailable* (reason `timeout`) whatever step it is in, then aborts the fetch. A later decode is a no-op: its bitmap is closed and discarded. An abort (display failure, destroy) also settles it. The timeout result stays cached per URL. The diagnostics now carry `imagery-probe-started` and the reason the probe failed | **B27**: the harness holds image decodes in the page (`holdDecodes`). The case checks: the server sent 200 for the image, the decode was held, the probe settled at its own deadline, the exact notice appeared, the map stayed usable, and a late release changed nothing. The same URL again was not refetched | PASS. Decode held at 643 ms. The probe started at 280.1 ms (`render()` first runs the synchronous WebGL check) and settled **2199.9 ms later with reason `timeout`**. The notice followed at 2480.1 ms with the decode **still held** and 0 pending probes. The map was ready with the watchdog off and no raster; zoom +0.5 kept centres at 0.0024 px. After release: still `imagery-unavailable`, `imageryLoaded=false`, late bitmap closed (0 × 0). Same URL again: 0 refetches, 0 new decodes |
| B2 | B24 matched worker responses by script URL, so one worker's response satisfied every worker with that URL. The reviewer dropped one response and the matrix still exited 0 | The audit records each worker under its CDP session id and target id. In order, it enables Runtime, Log and Network, then calls `runIfWaitingForDebugger`, awaiting each step. Responses are filed against the session that reported them. `workerCoverageFailures` requires, for every target, a 200 script response in its own session with the target URL and the exact headers. The following all go into `setupFailures`, which fails B20 and B24 and the run exit code: a target without session or target id, an unexpected target type, a worker not paused for the debugger, a failed enable or resume, events from an unattached session, and an http(s) request with neither a response nor a loading failure. A worker's script request is announced in the page session and answered in the worker session under the same `requestId`; it counts as answered only there | **B24** (per-target coverage; in-run self-tests: removing one worker's response, or lending it a same-URL twin's, is caught for exactly that worker) and **B20** (setup failures, unanswered requests). **B26**'s Satellite half now runs on a fresh adapter, so it cannot inherit focus state | PASS. 22 targets, 22 distinct sessions, 2 script URLs, 0 coverage failures, 0 setup failures, 0 unanswered. Self-tests: drop one → 1 failure; borrow a twin's response → 1 failure. B26 Satellite half: focus before = `null`, crown moved 0.000 px, focus stays `null` |
| N3 | Focusing MA-A and then rendering a frame that shows only MA-E kept the old Satellite camera | `render()` clears a focus that the new frame hides. When a map exists it refits the Satellite camera on the visible woodland. Clean Woodland already reframed through `focusBox`; it now also clears the focus | **B28** (both styles: the hide-render result is compared with an explicit `focusArea(null)`) | PASS. Satellite: MA-E-T01 moved from x = 1147.6 (focused) to 610.5; the reference is 610.5 (0.000 px, zoom difference 1.2e-10), on stage, no new map. Clean Woodland: 611.4 against 611.4, same matrix |
| N4 | A rejected remote imagery URL with antimeridian-crossing bounds pushed a supported stand to Clean Woodland | `imageryEligibility()` runs **before** the stand preflight. It applies the imagery policy and then checks the imagery's own footprint. Imagery that fails is dropped (imagery-unavailable path, never requested). The stand preflight now uses the area bounds only | **B29** (anchor 179.99° E: a remote URL with crossing bounds; same-origin imagery whose own bounds cross; a control with no imagery) | PASS 3/3. All three cases stay in Satellite with the exact imagery copy, no raster, no `footprint-unsupported`, and crowns within 0.0017 px. The same-origin case reports `CROSSES_ANTIMERIDIAN`. The dropped same-origin URL was requested 0 times |
| NIT5 | §3 remount wording | §3 now says a **healthy** adapter rebuilds on its next Satellite frame, and that one with a sticky display failure (§8) stays on Clean Woodland | — | Wording only. §4 and §8 were also updated for N3/N4 and the whole-probe deadline |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs in `run-20261128T133605Z/mutants/`):**

| Mutation (disposable copy) | Finding | Matrix result |
|---|---|---|
| (a) `worker-script-omitted`: the CDP collector discards ONE vendored worker's own script response (the reviewer's omission) | B2 | exit 1. **B24 FAIL**: 1 coverage failure naming target `2D9A459A…` ("no script response in its own session"). **B20 FAIL**: 1 unanswered request. All other cases pass |
| (b) `decode-not-bounded`: `probeImagery` restored to the repair-cycle-1 version (the timer only aborts fetch; the decode is awaited) | B1 | exit 1. **B27 FAIL** alone: decode held at 645 ms, no notice by the deadline ("never"), no probe settlement |
| (c) `hidden-focus-no-refit`: the hide-render keeps the old Satellite camera (`syncSatelliteOverlay` only) | N3 | exit 1. **B28 FAIL** alone: Satellite crown stays at x = 1147.6 against the 610.5 reference (537 px); the Clean Woodland half passes |
| (d) `imagery-bounds-in-preflight`: imagery bounds put back into the stand footprint preflight | N4 | exit 1. **B29 FAIL** alone (1/3): both crossing-imagery cases fall to Clean Woodland with `footprint-unsupported`; the control passes |
| (e) `satellite-hidden-focus` (extra): a hidden lane is focused and framed only while Satellite is active | B2 (B26 independence) | exit 1. **B26 FAIL** alone: the Satellite half on a fresh adapter moved the crown 268.5 px (focus = MA-A); the Clean Woodland half passes |

Scripts: `run-mutants.sh` and `mutate.py` (copied into `mutants/`, with `mutants-summary.txt`). Every mutation asserts that its patch applied. No Chromium process or working copy was left behind.

**Repair-cycle-2 choices a reviewer should check:**

- **Imagery whose own footprint does not fit is dropped, not the map.** The reviewer asked for invalid imagery to be dropped. Same-origin imagery that crosses the antimeridian is treated the same way, because it cannot be drawn on the one longitude branch. Only the stand footprint can now cause the display fallback.
- **A hidden focus is cleared, not remembered.** If the lane becomes visible again, the view stays on the whole woodland until `focusArea` is called again.
- **The B27 timing bound is relative to the probe's own start.** Under SwiftShader, the synchronous WebGL availability check in `render()` takes about 280 ms before the probe starts, so the absolute bound from `render()` is only a sanity limit (≤ deadline + 1000 ms).

## Repair cycle 1 (after GPT-6.2 Sol review: REQUEST CHANGES)

Sole writer: Claude Opus 5.5. Fix commit `e4b33fc1`. Same file fence; no C#, shell, save, Unity, `.meta`, build or `Docs/Project` change.

Gate run on clean `e4b33fc1` (`run.sh`, exit 0, 2026-11-28T12:51Z):

- vendor integrity **16/16**;
- geo oracle **38/38** (28 before, plus 10 footprint cases);
- browser matrix **B01–B26 26/26**;
- whole run: 0 console errors, 0 CSP violations, 0 failed requests, 0 non-loopback requests.

| # | Finding | Fix | Regression test | Result |
|---|---|---|---|---|
| B1 | An in-flight imagery probe could undo the WebGL fallback; a held probe at startup kept Clean Woodland with no notice past the watchdog | `failDisplay` bumps the generation and aborts every pending probe (`AbortController`). `startSatellite` treats `displayFailed` as stale. The watchdog is armed when the satellite start begins, so it also covers the module import. The imagery probe runs alongside the map start (it no longer gates `new Map`) and is limited to `imageryTimeoutMs` = `min(8 s, loadTimeoutMs / 2)` | **B22**: (a) the server holds a new imagery response, then a forced WebGL context loss, then release; (b) startup with the imagery held; (c) a module import that never finishes | PASS. (a) The pending probe was aborted (1 → 0); after release the notice persisted, with no map and maps constructed 1 → 1; a later satellite frame stayed on Clean Woodland. (b) The imagery notice appeared after 1885 ms (< watchdog), and release changed nothing. (c) The watchdog fired after 2053 ms and the late import built no map |
| B2 | Satellite projection broke across the antimeridian (Jacobian sampled wrapped longitudes) | New `checkWebMercatorFootprint`, run before any MapLibre work. The footprint is every area plus the imagery bounds, grown by 50 m, with longitudes unwrapped relative to the anchor. A footprint crossing ±180° (east or west) or reaching ±85.0511° falls back to Clean Woodland with the display copy. The fallback is per frame and not sticky | **B21** (east crossing 179.9996° E; west crossing 179.9996° W; Mercator limit 85.0508° N; near-antimeridian 179.99° E/W and 84.9° N in Satellite), plus 10 geo-oracle footprint cases | PASS. Crossings: Clean Woodland, the correct code, 0 maps, crowns finite and on stage, px/m spread 1.00000. Near-antimeridian: radius error 0.216 mm, centres 0.0017 px. 84.9° N: radius error 1.750 mm (≤ 2 mm), centres 0.014 px. A supported frame afterwards uses Satellite |
| B3a | `normalizeFrame` aliased and froze nested caller anchor data | Only the supported anchor fields are copied, and only as primitives (a non-primitive becomes `null` and is rejected). Only the adapter's own copy is deep-frozen | **B23** (nested `provenance` + extra nested data; non-primitive `lonDeg`) | PASS. Caller unchanged, 0 frozen caller objects, still mutable; the adapter copy is frozen and has no `provenance` |
| B3b | `areas: []` left the old crowns and transform; clicks emitted coordinates from the old frame | New `clearOverlay()` empties Layers 4–8 (reserved slots kept), the selection marks, the transform and the hit-test matrix whenever the frame has no drawable area or the transform is unusable | **B23** (`areas: []` in both styles, click at the old crown position) | PASS. 0 crowns, layers [0,0,0,0,0], matrix null, 0 clicks emitted. The refilled frame draws 20 crowns and its click carries the new `frameKey` |
| B4 | The CSP gate passed after a late response lost its header | **B08** now runs after every other case. It covers every browser response (page **and worker sessions**: workers auto-attach paused so their own script fetch is reported), every server-written response (headers recorded by the server, including held/aborted imagery) and the node-side 404. **B24** adds a coverage check (required response kinds, incl. the silent-worker fixture), worker-script accounting (each of 17 worker targets reported its own script), "no unaccounted server response", and a pure sensitivity self-test | **B08**, **B24**, plus the disposable-copy mutation `late-csp-header` | PASS. 141 browser + 145 server responses + the 404, all exact; 12/12 kinds; 0 unaccounted. Removing one header is detected (1 mismatch each) |
| N5 | Attribution stayed visible with the image wholly off-screen | `shouldShowAttribution` also requires `imageryInView`: the projected imagery quad intersects the stage (separating-axis test, re-evaluated on move/resize) | **B25** | PASS. Off-screen: hidden. Zoomed out until the image is on screen: shown. Zoomed back: hidden |
| N6a | Focusing a hidden lane moved the visible lane | `focusArea` ignores an area the current frame hides; `focusBox` only frames visible areas | **B26** | PASS. The visible crown moved 0.000 px (both styles) |
| N6b | Same-instance destroy + mount kept the Satellite status on an empty canvas | `destroy` resets the style, fallback, imagery state and watchdog, and aborts probes. Only the sticky display failure and the probe cache persist | **B26** | PASS. Remount shows Clean Woodland with no notice, no attribution, no crowns and no canvas; the next satellite frame rebuilds (maps constructed = 2) |
| N7 | B01/tasks wording | Spec, plan and tasks now say: production adapter = exactly five string-keyed own methods (`Reflect.ownKeys`); the opt-in debug handle adds two non-enumerable symbols. B01 asserts both | **B01** | PASS (no behaviour change) |

**Sensitivity (disposable copies under `/tmp`, matrix run directly; logs in `mutants/`):**

| Mutation (disposable copy) | Reviewer finding | Matrix result |
|---|---|---|
| `late-csp-header`: server drops CSP from the later B16 non-image response | B4 | exit 1. **B08 FAIL** (browser 1 + server 1 mismatch); B24 also FAILs, because its sensitivity counts become 2 |
| `pending-imagery-vs-webgl-loss`: no generation bump, no probe abort, `displayFailed` not treated as stale (the pre-repair behaviour) | B1 | exit 1. **B22 FAIL**: after release the notice is cleared (`""`); in (c) the late import flips the style to `satellite` |
| `antimeridian-no-footprint-check`: footprint check disabled | B2 | exit 1. **B21 FAIL** 3/6: crossings go to Satellite with crowns off stage, or show no notice |
| `empty-frame-stale-overlay`: `drawOverlay` returns early instead of clearing | B3b | exit 1. **B23 FAIL**: 20 crowns left, layers [1125,5,20,5,7], 1 stale click emitted |
| `attribution-ignores-viewport` (extra) | N5 | exit 1. **B25 FAIL** (attribution shown with the image off-screen) |
| `focus-hidden-lane` (extra) | N6a | exit 1. **B26 FAIL** (the visible crown moved 261.8 px) |
| `caller-anchor-aliased` (extra): shallow anchor copy restored | B3a | exit 1. **B23 FAIL** (3 caller objects frozen, caller no longer mutable) |

Scripts: `run-mutants.sh` and `apply-mutant.py` (copied into the evidence run folder); summary in `mutants/mutants-summary.txt`. No Chromium process or working copy was left behind.

**Repair-cycle choices a reviewer should check:**

- **Antimeridian:** reject rather than wrap. Rendering across ±180° would need wrapped camera, overlay and image-source handling inside the vendored MapLibre, which Part 1 does not attempt. The fallback reuses the locked display copy (`Map display unavailable. Showing Clean Woodland.`) instead of inventing a fourth string. It is not sticky.
- **Sticky display failure survives same-instance remount** (the spec says "for its lifetime"). Everything else resets on `destroy`.
- **Imagery timeout:** a timed-out probe is cached as unavailable for the adapter's lifetime, the same as any other probe result.
- **High latitude:** at 84.9° N the single-affine overlay measures 1.75 mm against the 2 mm bar on the 80 m fixture. That is the expected second-order term (∝ tan φ · distance²). A larger stand at very high latitude would need a re-centred derivative, so this is noted as a limitation and not addressed.

---

*The sections below describe the original Part 1 delivery (code `b6cb1461`) and are kept for history; the repair table above supersedes their verification numbers.*

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

- **Repair cycle 2 additions, not tested:** a real slow image decoder (the stall is simulated by wrapping `createImageBitmap` in the harness page); non-Chromium worker auditing.
- **Repair cycle 1 additions, not tested:** rendering across the antimeridian (rejected by design); hardware-GPU context loss (SwiftShader `WEBGL_lose_context` only); real slow networks (the stall is simulated by a held loopback response).

## Next authorised step

An independent, non-author **delta re-review** of `review/multi-2d-maplibre-adapter-v1` (repair cycle 2, code `ce7732a9`) against the delta re-review findings, this handoff and the spec. Part 2 stays QUEUED until an integrator approves a separate packet.

NOT merged to integration/2d-sim or main.
