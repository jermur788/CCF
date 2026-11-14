# MultiRackSitkaLab — Synthetic Sitka test stand (flat), P0

> **SYNTHETIC TEST FOREST — NO BIOLOGICAL GROWTH.**
> This is a standalone engineering/research tool. Every tree is an **artificial engineering fixture**,
> not a measured Irish tree, and nothing here is forestry advice, an ecological prediction, a validated
> growth model, a Scenario One feature or a Unity component. Approved scope: MULTI-001 #493 → MULTI-002 #494,
> **P0 only** (spec `specs/multirack-sitka-2d-reference-lab-v1/`).

What it does: one authoritative individual-tree forest (integer millimetres) with exact stand arithmetic,
explicit *mark → propose → approve → queue → execute* work planning, an ordered SHA-256-chained event log with
deterministic replay, lab-only saves, and a local 2D browser map. What it deliberately does **not** do: grow trees,
import real marteloscope/GBIF data, touch Unity, Scenario One, CCF game saves, or the network.

## Quick start (Linux/macOS; .NET SDK 8, any modern Chromium-family browser)

```bash
cd Tools/MultiRackSitkaLab
dotnet build src/Host
dotnet run --project src/Host            # prints: Listening on http://127.0.0.1:5187
# open http://127.0.0.1:5187/ in a browser (loopback only; Ctrl+C stops the host)
dotnet run --project src/Host -- --port 5200   # another port
```

Windows/WSL users run the same commands. This is a normal local test build — **not** a Windows game build.

## Layout

```text
src/Core/            C# class library, no Unity/web dependencies: geometry, metrics, state machine, canonical JSON,
                     lab-file reader/writer, replay, lab data store (path safety), display DTOs ("Views")
src/Host/            ASP.NET Core minimal API bound to 127.0.0.1 only + static file server for the UI
src/Ui/wwwroot/      index.html, styles.css, app.js — SVG map; draws and formats Core values, computes no forestry maths
tests/Core.Tests/    headless Tier-1 suite (dependency-free mini runner; no NuGet packages)
tests/Host.Tests/    headless host/API/security suite (real loopback listener, temp lab root)
tests/Ui/            Tier-2 real-browser driver (Node + Chromium DevTools Protocol, no npm packages)
tests/Shared/        the tiny test kit shared by both .NET suites
tests/*.py           schema check and mutation (negative) controls — run with `python3 -I -B`
tools/FixtureTool/   regenerates the checked-in fixtures from closed-form arithmetic (no random numbers)
fixtures/synthetic/  checked-in immutable fixtures + MANIFEST.sha256
schemas/             lab-file-v1.md (authoritative) and lab-file-v1.schema.json (structural)
.lab-data/           gitignored lab-only saves; created on first run
evidence/            README only; durable evidence lives off-repo and is referenced from the handoff
```

## Fixtures (all `ENGINEERING TEST FIXTURE`)

| fixtureId | trees | purpose |
|---|---|---|
| `oracle-quarter-ha` | 3 | 50 m × 50 m = 0.25 ha, live stems 20/30/40 cm → hand oracle: BA = π(0.01+0.0225+0.04) ≈ 0.227765 m², BA/ha ≈ 0.911062, N/ha = 12, Dq ≈ 31.0913 cm; felling the 40 cm stem removes ≈ 0.125664 m² |
| `sitka-flat-demo-v1` | 162 | skewed five-vertex polygon (0.5813 ha) with dead stems, below-threshold stems and boundary-on-edge stems; the default stand |
| `sitka-flat-perf-2000-v1` | 2,000 | 200 m × 100 m performance fixture for the ≤ 2 s selection requirement |

They are produced by index-driven arithmetic patterns (`tools/FixtureTool`), **not** a random generator, and then
frozen: tests pin each file's SHA-256 and snapshot hash. Their numbers carry no allometric or stocking meaning.
Regenerate (only as a deliberate, reviewed change): `dotnet run --project tools/FixtureTool -- "$PWD/fixtures/synthetic"`.

## Forest rules in one screen

* Eligible adult = **living** and `dbhMm >= adultThresholdMm`. Dead, felled and below-threshold stems stay visible
  but never count in N, BA, N/ha, BA/ha or Dq and can never be marked or worked.
* `BA_i = π (DBH_cm / 200)²` m²; `N/ha = N / areaHa`; `BA/ha = ΣBA / areaHa`; `Dq = √(Σ DBH_cm² / N)`
  (quadratic mean, `not applicable` when N = 0). No top height in P0.
* Area is exact (integer shoelace); the polygon must be one simple ring (either winding); stems exactly on the
  boundary are inside; any stem outside rejects the whole fixture.
* **Marking, proposing, approving and queueing never change the forest.** `Execute Test Work` is atomic: if any
  listed tree is dead/duplicate/missing/below threshold/unreserved, or the revision/epoch tokens are stale, **nothing**
  is felled and a single audit-only rejection event is recorded.
* `Advance experiment step (calendar only — NO BIOLOGICAL GROWTH)` bumps the step and forest revision and makes
  approved/pending plans `STALE`; age, DBH and height never change. Loading or resetting also stales old approvals.
* Full table and file format: [`schemas/lab-file-v1.md`](schemas/lab-file-v1.md).

## Saves and safety

* Saves exist only as `<name>.sitkalab.json` directly inside `Tools/MultiRackSitkaLab/.lab-data/`. The API takes a bare
  name (save) or a name from the lab-generated list (load). There is no path argument, no file picker and no way to name a
  CCF game save. Names are shape-checked *before* any file-system call; symlinks (including a symlinked root) are refused
  before opening; a file whose first three root properties are not `ccf-sitka-lab`/`1`/`Synthetic` is rejected before its
  body is parsed; a loaded file must replay from its named checked-in fixture and reproduce its `snapshotHash`.
* The host listens on `127.0.0.1` only (never all interfaces), requires `Host: 127.0.0.1:<port>`, rejects any other
  `Origin`, requires `Origin` + `X-Sitka-Lab` + `application/json` on POST, allows only GET/POST, sends no CORS headers,
  a restrictive CSP, 1 MB body limit and no stack traces. No accounts, telemetry or outbound requests; the browser code
  makes only same-origin calls. Nothing under `Assets/`, `ProjectSettings/` or any Unity/CCF save is read or written.

## API (all JSON, all computed by Core)

`GET /api/state` · `GET /api/fixtures` · `GET /api/saves` · `GET /api/replay` · `GET /api/export` ·
`POST /api/preview {treeIds}` · `POST /api/marks/mark|unmark {treeIds}` · `POST /api/plans/create {treeIds}` ·
`POST /api/plans/{planId}/propose|approve|queue|execute|reject|cancel` · `POST /api/step/advance` ·
`POST /api/fixture/load {fixtureId, confirm:true}` · `POST /api/saves/save {name}` · `POST /api/saves/load {fileName}` ·
`GET /api/health`. Refused actions return HTTP 409 with a stable `code` and a human message; the response carries the
fresh state.

## Verification commands

```bash
dotnet build MultiRackSitkaLab.sln                                  # warnings are errors
dotnet run --project tests/Core.Tests                                # Tier 1: Core (geometry, metrics, state machine, files, store, fuzz)
dotnet run --project tests/Host.Tests                                # Tier 1: host/API/security over a real loopback listener
python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json
python3 -I -B tests/run-mutation-controls.py "$TMPDIR/mut" results.json   # negative controls on a scratch copy
EVIDENCE_DIR=/path/to/evidence node tests/Ui/run-browser-tests.mjs  # Tier 2: real Chromium (needs: dotnet build src/Host)
```

The browser driver launches `$CHROMIUM` (default `/usr/bin/chromium`) headless with a temporary profile, starts the host
on a free loopback port, drives real mouse/wheel/drag events, compares what the page *displays* with what Core *returns*,
saves screenshots and a JSON receipt, and removes only the lab files it created.

## Dependencies and licences

* **No third-party packages.** .NET 8 SDK/runtime (MIT, Microsoft) with the in-box `Microsoft.AspNetCore.App` shared
  framework (MIT); the C# standard library only. No NuGet, npm or CDN dependency; the UI is hand-written HTML/CSS/JS.
* Test tooling: Node.js (built-in `WebSocket`, `child_process`) and a Chromium-family browser, both only on the developer
  machine; Python 3 standard library for the two helper scripts.
* Toolchain pinned by `global.json` (SDK 8.0.133, `rollForward: latestPatch`).

## Known limitations (P0)

* No ecology: no growth, mortality, regeneration, competition or top height. Calendar steps are bookkeeping only.
* Flat site, Sitka only, one polygon without holes, ≤ 20,000 trees. No terrain, racks, species mix, economics or 3D.
* Byte-identical canonical output is guaranteed for the same fixture, schema and pinned .NET runtime (not across
  intentional stale-plan transitions or future runtimes).
* The SVG map redraws every glyph on each state change; fine for the 2,000-tree fixture (see handoff for measured
  timings) but not designed for tens of thousands of stems.
* Single local operator; the in-memory session is not shared and a host restart returns to the default fixture.
* Accessibility is basic (shapes + letters + text labels, keyboard ID selection); no full certification.
