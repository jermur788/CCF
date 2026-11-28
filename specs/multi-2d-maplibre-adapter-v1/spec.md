# Open-Source Satellite Map Plug — MapLibre Display Adapter and Ellipsoidal Geo-Anchor Bridge V1

- **Spec ID:** `SPEC-multi-2d-maplibre-adapter-V1`
- **Status:** `APPROVED / IMPLEMENTING` — Part 1 implemented on an isolated branch, repair cycles 1, 2 and 3 applied after independent review; **Part 2 `QUEUED`** behind separate integrator approval
- **Milestone:** CCF 2D Simulator — Playable Milestone 7, Part 1
- **Owner:** Overall Manager / 2D Delivery Manager
- **Base commit:** `b2006767cbe70fa2c822497470908251a0fd6ffd` (`origin/integration/2d-sim`, Scots Pine Part 2 merged green)
- **Context commit:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (`origin/main`, untouched)
- **Branch:** `feat/multi-2d-maplibre-adapter-v1`
- **Decision references:** D-020 (presentation derives from authoritative state; renderers compute no forestry), D-030 (integration-state words)

## 1. Intent and scope

Give the 2D simulator an optional, offline, open-source map style behind the existing five-method map renderer contract (`map/contract.js`, `MAP_RENDERER_METHODS`). The forestry engine stays in flat local metres (`xM`, `zM`). The map projection is a display concern only: it never feeds back into growth, competition, crowns, cells or money.

**Part 1 — this implementation (in scope):**

- MapLibre GL JS 6.20.0 (BSD-3-Clause) vendored unmodified, with SHA-256 provenance; no build step, no CDN.
- A self-contained adapter under `Tools/MultiRackSitkaP1Lab/src/Ui/multi/map/maplibre/` that imports nothing from the shared `/multi` shell.
- An ellipsoidal WGS84 geo-anchor bridge (`geo-anchor.js`).
- A standalone verification harness (`tests/maplibre-adapter/`): vendor integrity, geodesic oracle, and a strict-CSP offline headless-Chromium matrix `B01`–`B30` over an 80 × 80 m, 5-lane synthetic fixture (`B21`–`B26` added in repair cycle 1, `B27`–`B29` in repair cycle 2, `B30` in repair cycle 3).

**Part 2 — `QUEUED` (out of scope; needs a separate integrator-approved packet):**

- The live `/multi` style toggle and imagery-opacity control in the shared shell (`multi.html`, `multi.css`, `main.js`, `store.js`, panels).
- Building the adapter frame from `store.state` and wiring `deps.onPointer` to the page's selection logic.
- `MultiRoutes.cs` static routes, `PlayerApi.cs` CSP review (`worker-src 'self'`, `img-src data: blob: 'self'`), `SitkaP1.Ui.csproj` embedded resources.
- Supplying real validated anchors and local imagery for real woodlands.

## 2. Locked product defaults

1. **8-layer stack, bottom to top** (both styles, `layer-stack.js`):

   | # | id | Content |
   |---|---|---|
   | 1 | `woodland-canvas` | `#15251a` dark woodland canvas |
   | 2 | `georeferenced-imagery` | optional local, same-origin, georeferenced imagery with attribution; opacity control |
   | 3 | `stage2-reserved` | reserved, unimplemented slots: `drone-lidar`, `hillshade-contours`, `streams-rivers`, `roads-bridges-paths` |
   | 4 | `ground-overlays` | lane borders and 2.5 m ground cells (soil moisture is fixture-only in Part 1) |
   | 5 | `schematic-deadwood` | fixed-size schematic fallen logs (direction schematic, not simulated — as in `renderer-svg.js`) |
   | 6 | `living-crowns` | crowns at true physical `radiusM`; never fixed-pixel |
   | 7 | `planted-juveniles` | planted saplings (shelter ring when present) |
   | 8 | `marks-ranks-selection` | top: crop/fell marks, competitor ranks, selection; each mark has a shape or text, not colour alone |

2. **Imagery opacity:** default **65 %** (`imageryOpacity = 0.65`, a 0..1 fraction, clamped). It changes Layer 2 only; Layers 1 and 3–8 keep full opacity.
3. **Fallback copy (exact), never blocking, never `alert`/`confirm`/`prompt`, selection always kept:**
   - no validated location → Clean Woodland + `No map location set. Showing Clean Woodland.`
   - valid location, imagery unavailable → dark MapLibre canvas + `Imagery unavailable. Showing woodland map.`
   - WebGL / worker / module failure → Clean Woodland + `Map display unavailable. Showing Clean Woodland.`
4. **Session-only style:** every fresh session starts in **Clean Woodland** (`'clean-woodland'`). The adapter writes nothing to `localStorage`, `sessionStorage`, cookies or saves. **Zero MapLibre module, stylesheet, worker or imagery requests** happen until a frame asks for `'satellite'` *and* carries a valid anchor.

## 3. Renderer contract, frame and pointer payload

`createMapLibreAdapter(options?)` returns a frozen object; `assertMapRenderer` passes and there is no sixth method. A production adapter (no options) has exactly five own keys (`Reflect.ownKeys`), all string-keyed methods: `mount`, `render`, `setSelection`, `focusArea`, `destroy`. The test-only `exposeDebug: true` option keeps those five string-keyed methods and adds two non-enumerable symbol keys (`Symbol.for('ccf.maplibre.debug')`, `Symbol.for('ccf.maplibre.debug.actions')`).

- `mount(container, deps)` — builds the DOM once; a second `mount` only replaces `deps`. Calls before `mount` or after `destroy` are no-ops.
- `render(frame)` — copies the frame into adapter-owned plain data, normalises it and deep-freezes **only that copy** (`normalizeFrame`). The caller's objects are never mutated, frozen or referenced; from `geoAnchor` only the supported primitive fields are copied (`sourceId`, `geometryDigest`, `originKind`, `lonDeg`, `latDeg`, `bearingDeg`, `heightM`, `localOriginXM`, `localOriginZM`, `isCentroid`, `derivedFromCentroid`), and any other key (for example nested provenance) is ignored. Each call bumps a generation counter, and so does a display failure; async work (module import, imagery probe) started for an older generation is dropped. MapLibre events are guarded by map-instance identity instead, so later renders never deafen a live map. A frame with no drawable area clears Layers 4–8, the selection marks and the hit-test transform, so nothing from an earlier frame stays on screen and clicks emit nothing until a drawable frame arrives.
- `setSelection({kind: 'tree'|'cell'|'juvenile', areaId, id} | null)` — kept across renders, style switches and every fallback.
- `focusArea(areaId | null)` — frames one visible area (or the whole woodland); changes no data and keeps the selection. A lane that the current frame hides is ignored (the view stays put). A later frame that hides the focused lane clears the focus, and that render itself frames the whole visible woodland in both styles (Satellite refits its camera; no `focusArea(null)` call is needed).
- `destroy()` — idempotent; removes DOM, listeners, the resize observer, the watchdog and any MapLibre instance, aborts pending imagery probes and invalidates in-flight work. It also resets the transient display state (style, fallback notice, imagery state), so `mount` on the same instance starts in Clean Woodland, and a **healthy** adapter rebuilds the map on its next Satellite frame. An adapter with a sticky display failure (§8) stays on Clean Woodland instead. Only that sticky display failure and the per-URL probe results last for the adapter's lifetime.

**Style selection lives in the frame** (`frame.mapStyle`, default `'clean-woodland'`), so no extra method is needed.

**Frame slice consumed** (the Part 2 host builds this from `store.state`; Part 1 uses the fixture):

```
frameKey, sourceId, geometryDigest, mapStyle, imageryOpacity,
imagery: { url, attribution, boundsLocal: {minXM, minZM, maxXM, maxZM} } | null,
geoAnchor: { sourceId, geometryDigest, originKind, lonDeg, latDeg, bearingDeg, heightM?, localOriginXM?, localOriginZM? } | null,
activeAreaId, visibleAreaIds, overlay: 'off'|'light'|'undergrowth'|'seedlings'|'soil-moisture',
areas: [{ areaId, label, extent: {minXM, minZM, sideM}, polygon: [{xm, zm}],
          groundCells: [{column, row, polygon: [{u, v}], lightIndex?, bramble?, bracken?, seedlingMask?, soilMoistureClass?}],
          deadwood: [{deadwoodId, u, v}], trees: [{treeId, u, v, radiusM, species?, status?, isCrop?, paintOrder?}],
          juveniles: [{juvenileId, u, v, speciesId?, isActive?, hasActiveShelter?}], ranks: [{treeId, rank, chosen?}] }]
```

Records missing a value needed to draw them honestly (for example a tree without a finite positive `radiusM`) are **skipped, not given invented defaults**.

**Pointer callback** — `deps.onPointer` receives exactly `{ areaId, u, v, juvenileId, frameKey }` in flat local metres of `areaId` (`u` east of `extent.minXM`, `v` south of `extent.minZM + extent.sideM`). A click within a planted sapling's glyph (or 12 px) reports that sapling's own `u`, `v` and `juvenileId`; otherwise `juvenileId` is `null`. A click outside every visible area falls back to the active area, the same rule as `renderer-svg.js resolveMapPoint`. A drag of more than 5 px is a pan, not a click.

## 4. Rendering design

- **One overlay, two transforms.** Layers 3–8 are SVG groups drawn in metre units (`X = xM`, `Y = -zM`). A single `<g transform="matrix(a b c d e f)">` maps metres to stage pixels:
  - Clean Woodland: a uniform north-up scale that fits the focus box plus 4 m padding;
  - Satellite: the local derivative of *local metres → WGS84 (geo bridge) → MapLibre screen*, taken by central differences (±10 m) about the woodland centre and refreshed on every map `move`/`resize`.
- **Crowns:** `<circle r="radiusM">` — the radius attribute is the frame's metres, unchanged. Because the transform is the true local Jacobian, a ground circle is drawn as the (very slightly elliptical) shape it really has on the Web Mercator map. Drawing a screen-space circle instead is wrong at the 2 mm level: Web Mercator applied to WGS84 coordinates is not conformal on the ellipsoid. *Computed, not cited:* the north/east scale ratio is ν/ρ ≈ 1.0024 at 53.35° N, about ±4.5 mm on a 3.8 m radius. The affine approximation's own error over an 80 m woodland measured 0.21 mm (B11).
- **Constant-pixel marks** (badges, sapling glyphs, rings) are sized from metres-per-pixel; strokes use `vector-effect: non-scaling-stroke`. Panning only rewrites the transform; content is rebuilt when frame or selection change, or when zoom changes by more than 1 %.
- **Satellite canvas:** MapLibre draws Layer 1 (a `background` `#15251a`, plus an inline GeoJSON woodland-extent `fill` in the same colour) and, above it, Layer 2 (`image` source + `raster` layer, `raster-opacity` = frame opacity). The extent fill looks identical to the background. It is there because GeoJSON is tiled by the worker: the adapter counts the map *ready* only once that source has loaded. With a background-only style a dead worker would go unnoticed (the first harness run proved this).
- **Camera:** map bearing is set to the anchor's `bearingDeg`, so the woodland stays upright in both styles; rotation and pitch interactions are disabled; zoom and pan stay enabled.
- **Imagery:** same-origin URL plus local `boundsLocal` (its georeference in local metres) plus non-empty attribution are all required. Before handing the URL to MapLibre, the adapter fetches it once and checks that it decodes as an image (`createImageBitmap`); the result is cached per URL for the adapter's lifetime. The probe runs alongside the map start and never delays it. **The whole probe — fetch, body read and decode — shares one deadline**, `imageryTimeoutMs` (default `min(8 s, loadTimeoutMs / 2)`). At the deadline the probe settles to *unavailable* whatever step it is in and the request is aborted; a step that finishes later (for example a decode the browser releases late) changes nothing, and its bitmap is closed and discarded. The deadline timer alone is not trusted: when the main thread is busy past the deadline, a finished decode resumes as a microtask before the overdue timer task, so every step also checks the time elapsed since the probe started and an overdue step settles the probe as a timeout itself (repair cycle 3). A timed-out result is cached per URL like any other. A display failure or `destroy` aborts any probe still in flight.
- **Imagery eligibility comes first.** Before the footprint preflight, the imagery descriptor is checked on its own: the policy above, then its own footprint (`boundsLocal`, grown by 50 m) on the anchor's longitude branch. Imagery that fails either check is dropped (imagery-unavailable path, no request made) and never takes part in the stand's footprint preflight, so it cannot disqualify a supported stand.
- **Attribution** is shown only while imagery is visible, which requires all of the following:
  - satellite style;
  - MapLibre reports the image source loaded;
  - opacity > 0;
  - the projected imagery quad intersects the visible stage (a separating-axis test, re-evaluated on every map move and resize).

  At 0 %, and while the image lies wholly off-screen, the attribution is hidden.
- **One longitude branch.** Camera, overlay and imagery share the anchor's longitude branch. Before MapLibre starts, the stand footprint is checked: every area's bounds, grown by 50 m. A stand footprint that crosses the antimeridian (east or west, with longitudes unwrapped relative to the anchor) or reaches the Web Mercator latitude limit is not shown on MapLibre (§8). Imagery bounds are judged separately (see *Imagery eligibility*): imagery that does not fit is dropped, not the map.

## 5. Ellipsoidal geo-anchor bridge (`geo-anchor.js`)

- **Ellipsoid:** WGS84, `a = 6378137 m`, `f = 1/298.257223563`, `e² = f(2 − f)`. No GIS library.
- **Conventions:** `+xM` is local east and `+zM` local north when `bearingDeg = 0`. `bearingDeg` is the true-north clockwise azimuth of local `+zM`; local `+xM` then lies at `bearingDeg + 90°`.
  - forward: `(dx, dz) = (xM − localOriginXM, zM − localOriginZM)`; `E = dx·cosθ + dz·sinθ`, `N = −dx·sinθ + dz·cosθ`, `U = 0`; ENU at the anchor → ECEF → geodetic (iterative, exact to < 1e-14 rad).
  - inverse: geodetic at anchor height → ECEF → ENU → transpose rotation → `(xM, zM)`.
- **Binding and validation** — a bridge is created only when all of these hold; otherwise `NO_LOCATION`:
  - the frame has both `sourceId` and `geometryDigest` (`MISSING_FRAME_BINDING`);
  - the anchor's `sourceId` and `geometryDigest` are present and equal the frame's;
  - `originKind` is in the explicit allow-list `survey-corner | survey-point | site-origin`; anything containing `centroid`, or `isCentroid` / `derivedFromCentroid` set, is rejected as centroid-derived;
  - `lonDeg ∈ [−180, 180]`, `latDeg ∈ [−90, 90]` and within the Web Mercator limit ±85.0511°;
  - `bearingDeg`, `heightM` and the local-origin offsets are finite.
- **Accuracy gate:** round-trip error < 0.01 m, and distance agreement < 0.01 m with an independent Vincenty inverse (in the test, sharing no code) for 30 m, 80 m and 120 m stands at four latitudes and eight bearings. The azimuths of both local axes are also checked, so a mirrored frame cannot pass.

## 6. Offline EPSG:2157 (Irish Transverse Mercator) provenance rules

Irish woodland data often arrives in EPSG:2157 (IRENET95 / Irish Transverse Mercator). **Part 1 consumes WGS84 longitude/latitude anchors only.** It contains no projection library, does no CRS conversion and makes no runtime network request for one. The rules below bind any future offline importer that prepares anchors.

- **Projection parameters (as recorded in the EPSG registry for EPSG:2157; re-check against the EPSG dataset version actually used):** GRS 1980 ellipsoid (`a = 6378137 m`, `1/f = 298.257222101`); Transverse Mercator; latitude of origin 53°30′ N; central meridian 8° W; scale factor 0.99982; false easting 600 000 m; false northing 750 000 m. The datum is IRENET95, Ireland's realisation of ETRS89.
- **Datum shift — do not invent accuracy.**
  - ETRS89 is fixed to the Eurasian plate. WGS84 (as realised today) is aligned with ITRF. The two drift apart by a few centimetres per year, which amounts to roughly a metre by the mid-2020s. *Established in general; take the exact figure from a cited authority before relying on it.*
  - A "null" ETRS89 → WGS84 shift is acceptable only when the record says so, together with its stated accuracy (EPSG publishes such a transformation at about 1 m accuracy; quote the code and accuracy from the dataset used).
  - Never claim centimetre agreement with imagery or GNSS on that basis.
- **Each imported anchor must record:** source CRS code; EPSG dataset version; transformation code or method and its stated accuracy; coordinate epoch where relevant; tool name and version; who converted it and when. These provenance fields are **proposed for Part 2, not implemented in Part 1**.
- **Display only.** The bridge positions the map display; it never alters engine metres.

## 7. Security, CSP and network

- **Works unmodified under:** `default-src 'none'; script-src 'self'; style-src 'self'; connect-src 'self'; worker-src 'self'; img-src data: blob: 'self'; base-uri 'none'; form-action 'none'; frame-ancestors 'none'` with `Referrer-Policy: no-referrer`.
- **Worker:** `setWorkerUrl(new URL('./vendor/maplibre-gl-6.20.0/maplibre-gl-worker.mjs', import.meta.url).href)` is called before any `new Map(...)`. MapLibre 6.20.0 then constructs `new Worker(url, {type: 'module'})` directly for a same-origin URL: no blob or data workers.
- **No inline code or styles:** no `eval`, no inline `<style>`, no `style` attributes. CSS comes from `adapter.css` and the vendored `maplibre-gl.css`, the latter linked lazily and only for satellite.
- **Inline MapLibre style:** no `glyphs`, no `sprite`, no tile URLs, no remote sources, no telemetry. All text is in the SVG overlay.
- **Guardrail 7:** no `[C]`/`[E]` tags or calibration disclaimers on screen. Imagery attribution containing them is rejected.

## 8. Failure behaviour

| Trigger | Result | Notes |
|---|---|---|
| anchor missing or invalid, or frame has no identity | Clean Woodland + no-location copy | checked synchronously, before any MapLibre request |
| no imagery configured; imagery not same-origin, missing bounds or attribution; imagery bounds crossing the antimeridian or reaching the Mercator limit; URL does not decode as an image; MapLibre image-source error | dark MapLibre canvas + imagery copy | crowns, marks and selection still drawn; ineligible imagery is dropped before the footprint preflight and never requested |
| imagery probe (fetch, body read **or decode**) still pending after `imageryTimeoutMs` | dark MapLibre canvas + imagery copy | the probe settles at the deadline and the request is aborted; a late answer or late decode changes nothing (the late bitmap is closed), including a decode whose continuation runs before the overdue timer (elapsed-time check); the timeout is cached per URL and survives a same-instance remount |
| valid anchor, but the stand footprint crosses the antimeridian or reaches the Web Mercator latitude limit | Clean Woodland + display copy | checked synchronously before any MapLibre request; **per frame, not sticky** (a later frame with a supported footprint uses Satellite). Part 1 deliberately does not render across the antimeridian |
| WebGL unavailable (checked before import) | Clean Woodland + display copy | no vendor request |
| module import failure or hang; `Map` constructor throws; MapLibre error before ready; not ready within the watchdog (15 s default, armed when the satellite start begins, so it also covers the module import); WebGL context lost | Clean Woodland + display copy | **sticky**: the adapter then stays on Clean Woodland for its lifetime (no retry loop, survives same-instance remount); the failure bumps the generation and aborts pending imagery probes, so no later resolution can clear the notice or rebuild Satellite; a new adapter retries |

## 9. File fence

Writes are limited to `specs/multi-2d-maplibre-adapter-v1/**`, `Tools/MultiRackSitkaP1Lab/src/Ui/multi/map/maplibre/**`, `Tools/MultiRackSitkaP1Lab/tests/maplibre-adapter/**` and `Docs/Handoffs/MultiRack-Simulation-20261116/maplibre-adapter/**`.

The following are not touched: Unity (`Assets/`, `Packages/`, `ProjectSettings/`), `.p0ref`, all C#, `SitkaP1.Ui.csproj`, `wwwroot/`, every shared `/multi` shell file (including `map/contract.js`, `map/renderer-svg.js` and `map/minimap.js`), tests outside `maplibre-adapter/`, `build/`, `Docs/Project/`, the Strategic Roadmap, `.gitignore`, and any save data.

## 10. Acceptance criteria

- **AC-1** Vendored files match `provenance.json` sizes and SHA-256 hashes; the tarball hash and registry integrity are recorded.
- **AC-2** Geo bridge: round trip < 0.01 m and Vincenty agreement < 0.01 m at 30/80/120 m. All rejection cases fail with the expected code.
- **AC-3** `B01`–`B30` all pass in one run, with zero console errors (page and workers), zero CSP violations, zero failed requests, zero non-loopback requests, zero audit setup failures and zero unanswered requests. Every worker target (identified by CDP session and target id, not by URL) reports its own startup response of CDP resource type `Script` with the exact headers; an ordinary `Fetch` of the same URL never counts. Every response of the run carries the exact CSP and Referrer-Policy, checked at the end of the run: browser page and worker sessions, every server-written response, and the node-side 404.
- **AC-4** `git diff --name-only b2006767...HEAD` lists only the four prefixes; `git status --short` is clean; `origin/main` is unchanged.
- **AC-5 (Part 2, QUEUED)** Live `/multi` wiring, C# routes, CSP and resources verified under a separate approved packet.

## 11. Evidence classes

This feature is display-only and introduces no ecological rule. The fixture's trees, cells, moisture pattern, anchor and imagery are **synthetic test data** that describe no real woodland. The 65 % opacity, padding and mark sizes are presentation calibration, not ecological claims.
