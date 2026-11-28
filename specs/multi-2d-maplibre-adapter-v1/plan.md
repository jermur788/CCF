# Open-Source Satellite Map Plug V1 — Implementation Plan

- **Spec:** [`SPEC-multi-2d-maplibre-adapter-V1`](spec.md)
- **Status:** `APPROVED / IMPLEMENTING` (Part 1 active; Part 2 `QUEUED`)
- **Base:** `b2006767cbe70fa2c822497470908251a0fd6ffd` (`origin/integration/2d-sim`) · **Context:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`)
- **Branch:** `feat/multi-2d-maplibre-adapter-v1` (review copy: `review/multi-2d-maplibre-adapter-v1`)

## 1. Modules (`src/Ui/multi/map/maplibre/`, plain browser ES modules, no shared-shell imports)

| File | One job |
|---|---|
| `adapter.js` | `createMapLibreAdapter`: the five contract methods, style switching, lazy MapLibre lifecycle, fallbacks, pointer resolution |
| `geo-anchor.js` | WGS84 geodetic ↔ ECEF ↔ local ENU bridge, anchor validation and binding |
| `frame-projector.js` | `normalizeFrame` (copy + deep freeze, skip incomplete records), u/v ↔ world helpers, point-in-area, focus box |
| `overlay-layers.js` | builds Layer 3–8 SVG content in metre units (shared by both styles) |
| `clean-woodland.js` | the Clean Woodland fit transform (uniform north-up scale) |
| `layer-stack.js` | frozen 8-layer descriptor list + reserved Stage 2 slots |
| `imagery-policy.js` | locked defaults, exact fallback copy, imagery validation, attribution rule |
| `adapter.css` | all adapter styling (external stylesheet) |
| `vendor/maplibre-gl-6.20.0/` | four unmodified upstream files + `provenance.json` |

## 2. Key decisions and trade-offs

1. **Metre-space overlay with a projected affine transform**, not per-point pixel projection. It keeps `r = radiusM` literally in metres, draws the true (slightly anisotropic) Mercator footprint, makes panning a single attribute update, and lets Clean Woodland and Satellite share one builder. Cost: a first-order approximation about the woodland centre, measured at 0.21 mm on the 80 m fixture (budget 2 mm). For much larger extents, re-centre the derivative on the view.
2. **Worker liveness via an invisible Layer-1 GeoJSON fill.** The first harness run showed that a background-only style reaches `load` with a dead worker. The extent source (same colour as the canvas) needs the worker, so "ready" means the worker answered.
3. **Sticky display failure.** After a WebGL/module/worker failure the adapter stays on Clean Woodland, so a host that re-renders often does not flicker or retry endlessly.
4. **Anchor allow-list.** `originKind` must be an explicit kind (`survey-corner`, `survey-point`, `site-origin`), and the frame must carry an identity to bind to. A blacklist of centroid words alone was rejected as too weak.
5. **Camera bearing = anchor bearing**, so the woodland looks the same way up in both styles.
6. **Debug handle opt-in.** `exposeDebug: true` (tests only) adds non-enumerable symbol-keyed inspection hooks; production adapters have exactly five own properties.

## 3. Verification (`tests/maplibre-adapter/`, plain Node ESM, no npm install)

`run.sh` is the single entry point:

1. `verify-vendor.mjs` — re-hashes the committed bytes; checks sizes, the file list, no static imports and no reference to the omitted shared module; checks the CSS uses `data:` URLs only.
2. `test-geo-anchor.mjs` — round trip, independent Vincenty oracle at 30/80/120 m, axis azimuths, rejection codes.
3. `drive-maplibre-cdp.mjs` — starts `server.mjs` on an ephemeral loopback port, then launches `/usr/bin/chromium` headless with SwiftShader WebGL, `--host-resolver-rules="MAP * ~NOTFOUND, EXCLUDE 127.0.0.1"` and a dead proxy, in its own process group. It auto-attaches workers and runs `B01`–`B20` (see [tasks.md](tasks.md)).
4. `write-summary.mjs` — writes the committed `Docs/Handoffs/.../maplibre-adapter/evidence/summary.json`. Full logs, screenshots and reports go to `/media/jer/Files/CCF/Verification/Multi2DMapLibreAdapterPart1-20261128/run-<UTC>/`.

**Not run by design:** .NET builds/tests, `Phase1.Save.Tests`, `build/2d-gate.sh`, `build/2d-gate/selftest.sh`, Unity.

## 4. Part 2 boundary (`QUEUED`)

- Shell wiring: style toggle and opacity control.
- A `store.state` → frame builder.
- `onPointer` → existing selection commands.
- `MultiRoutes.cs`, `PlayerApi.cs` CSP and `SitkaP1.Ui.csproj` resources.
- Real anchors with the §6 provenance fields.

Part 2 needs its own approved packet and must re-run this harness plus the shell gates.
