# MULTI Map Integration Pass 2 — Implementation Plan

- **Spec ID:** SPEC-multi-map-integration-pass2-V1
- **Spec status:** IMPLEMENTING
- **Plan owner:** Primary Implementer (sole writer on `feat/multi-map-integration-pass2-20261124-v1`)
- **Created:** 2026-11-24
- **Last updated:** 2026-11-24

## 1. Architecture and zero-Core-edit design

- Base commit `1f13182659769f19e7c91159dd7ad475667e0d62` already contains all six fenced Core commands on `MultiAreaContinuation` (`AddClearUndergrowth`, `RemoveClearUndergrowth`, `AddPlant`, `AddShelter`, `AddSpotClear`, `RemovePlanting`), already projects `pendingClear`, `approvedClear`, `pendingPlanting`, `approvedPlanting`, `planted`, `shelters`, and `patches` inside `MultiAreaEcologyDto`, and already grants `SitkaP1.Core` internals to `sitkap1-ui` via `[assembly: InternalsVisibleTo("sitkap1-ui")]` (`PlayerCompetitionDiagnostics.cs:4`).
- `MultiAreaWorkPlanAdapter` does not expose those six methods directly, and its exact member list is pinned by `MultiAreaTests.cs` (`MA-W12`).
- Because `MultiAreaWorkPlanAdapter(MultiAreaContinuation session, MultiAreaExperiment experiment, string experimentKey, string? experimentBadge)`, `MultiAreaGenerator.Generate()`, `MultiAreaDemoExperiments.CreateMixedDemo()`, and `MultiAreaContinuation` are all `internal` in `SitkaP1.Core` and visible to `sitkap1-ui`, `Tools/MultiRackSitkaP1Lab/src/Ui/Program.cs` opens the experiment by creating ONE `MultiAreaContinuation` instance and ONE `MultiAreaWorkPlanAdapter` wrapping that exact same instance, passing both into `PlayerApi`.
- `PlayerMultiApi.cs` wraps the six `MultiAreaContinuation` calls with a private `RefusableMulti(Func<MultiAreaCommandResult>)` helper identical to `MultiAreaWorkPlanAdapter.Refusable`: catching `P1Refusal` and returning `new MultiAreaCommandResult(multiArea.Read(), null, null, refusal.Code, refusal.Message)`, then pairing the response atomically via `multiArea.CompositeFor(result.View)`.
- Result: **ZERO edits to `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/**`**, zero edits to `MA-W12`, zero fault seams, and zero caller-built plan surfaces.

## 2. Commit sequence

### Commit 1 — Spec quartet (`specs/multi-map-integration-pass2-v1/**`)
- Author self-contained `spec.md`, `plan.md`, `tasks.md`, and unticked `checklists/requirements.md` (`MI-P2-01`..`MI-P2-06`, `MI-REG`, `MI-P2-REVIEW`).

### Commit 2 — Six fenced HTTP endpoints (`Tools/MultiRackSitkaP1Lab/src/Ui/{Program,PlayerApi,PlayerMultiApi}.cs`)
- Update `Program.cs` (`UiProcess.OpenMultiExperiment`) to construct `(MultiAreaWorkPlanAdapter, MultiAreaContinuation)` from one shared `MultiAreaContinuation` session for both `ma-1` and `ma-1-mixed` (exact same logic as `MultiAreaWorkPlanAdapter.OpenExperiment`).
- Pass `MultiAreaContinuation` into `PlayerApi` (with a fallback overload keeping single-area untouched).
- Register the six routes in `PlayerMultiApi.cs`:
  - `POST api/multi/clear/add` (`MultiCellMembers` / `MultiCellRequest`)
  - `POST api/multi/clear/remove` (`MultiCellMembers` / `MultiCellRequest`)
  - `POST api/multi/plant/add` (`MultiPlantMembers` / `MultiPlantRequest` with `[JsonPropertyName("xM")]`, `[JsonPropertyName("zM")]`, `speciesId`)
  - `POST api/multi/shelter/add` (`MultiPointMembers` / `MultiPointRequest` with `[JsonPropertyName("xM")]`, `[JsonPropertyName("zM")]`)
  - `POST api/multi/spot-clear/add` (`MultiPointMembers` / `MultiPointRequest` with `[JsonPropertyName("xM")]`, `[JsonPropertyName("zM")]`)
  - `POST api/multi/planting/remove` (`MultiPlantingRemoveMembers` / `MultiPlantingRemoveRequest` with `orderId`)
- Extend `RequireBoundedText` with `("speciesId", MaxIdLength)` and `("orderId", MaxIdLength)`.
- Validate integer `column`/`row` and finite-JSON-number `xM`/`zM` (`double?` narrowed to `float` so float32 overflow reaches Core's `Phase2CoordinatesInvalid` refusal).
- Verify clean builds (`.sln` + `SitkaP1.Ui.csproj`) before proceeding.

### Commit 3 — Interactive modes, drawers, Work Plan panel, map previews & Escape order (`Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`)
- **Workspace mode toolbar:** expand `.mode-switch` cleanly to 4 buttons (`Select Tree`, `Inspect Cell`, `Plant`, `Spot Clear`) with compact 1280×720 padding so `.workspace-head` remains single-row and preserves R3 fold margins (`lastRowBottom <= dockTop` and `dockBottom <= 720`).
- **Right-hand inspector drawers (swapped inside `#pane-tree`, never stacked):**
  - `#tree-panel` (default `Select Tree` adult inspector; R3 fold geometry unchanged)
  - `#juvenile-panel` (active `PJ` sapling inspector in `Select Tree` mode with `Queue 8-year Shelter` / `Remove pending shelter`)
  - `#cell-panel` (`Inspect Cell` with `Clear undergrowth (spares seedlings)` and `Remove pending clear`)
  - `#plant-panel` (`Plant` drawer: `Sessile Oak` | `European Beech`, `[ ] Add 8-year shelter`, `[ ] Spot-clear ~1 m²`, exact world `(X, Z)` readout, `Queue to Work Plan`, partial-disclosure result box with one-click remove)
  - `#spot-panel` (`Spot Clear` drawer: exact world `(X, Z)` readout, seedling-free `0.564 m` preview note, `Queue Spot Clear (~1 m²)`)
- **Trees tab:** keep `#tree-rows` strictly for adult trees (171 / 73 rows) and render a separate `#planted-section` below the table whenever `ecology.planted` has entries, offering one-click `Select` to inspect any `PJ` sapling.
- **Map SVG rendering:**
  - Add `#draft-preview` layer for provisional dashed-slate local previews (`Plant` point + optional `0.564 m` ring + optional shelter badge; `Spot Clear` seedling-free `0.564 m` ring; snap OFF).
  - Extend `#spot-patches` (beneath `#planted-layer`) to draw `Pending` (dashed gold) and `Approved` (pale-blue) `SpotClear` orders alongside executed `#ccf-spot-clear` patches (exact world-scale `r = patch.radiusM`).
  - Extend `#planted-layer` / `#marks` to draw `Pending` (dashed gold) and `Approved` (pale-blue) `Plant` and `Shelter` orders, plus clickable active `PJ` saplings in `Select Tree` mode.
  - Keep `#ccf-spot-clear` ring aligned with world scale and keep N-6 stump `X` untouched.
- **Sequential fence-chained planting queue:** `plant/add` → `shelter/add` → `spot-clear/add`, applying each `200 OK` payload to refresh `currentFence()` before the next step; stop immediately on `409`/`4xx`/`500` with exact refusal display and one-click remove for every earlier pending order.
- **Work Plan panel, Execute dialog, Receipt, Year+1 consent, Command Gate, Refusal recovery, and Escape precedence:**
  - Update all counts, lists, targeted remove buttons, frozen confirmation checks, consent checks, command lock gating, and `recoveryFor(code)` messages across all six order types.
  - Toolbar mode clicks preserve focus on the clicked mode toggle (resolving Pass 1 `N-4` cleanly) and reset `#pane-tree` scroll to `0`; map picks focus the active drawer heading; `Escape` follows `modal → non-empty search → exit Plant/Spot Clear/Inspect Cell preview/mode → close PJ inspection → clear tree selection`, never discarding draft Fell ticks and never mutating the Work Plan.

### Commit 4 — Core/API tests & smoke-test updates (`Tools/MultiRackSitkaP1Lab/tests/**`)
- Add `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/MapIntegrationPass2Tests.cs` (registered in `Core.Tests/Program.cs`):
  - Full commitment-gate proofs across all 6 order types (Pending = zero ecology change; Approve = zero ecology change; Execute = single atomic commit, `SpotClear → Plant → Shelter` resolve order, zero year advance).
  - Explicit tests for all required Core refusal codes (`Phase2CoordinatesInvalid`, `Phase2PointOutsideArea`, `Phase2PointWithoutGroundCell`, `Phase2DuplicateOrder`, `Phase2SpeciesUnknown`, `PlantSpacingTooClose`, `PlantTooCloseToAdult`, `ShelterNoTarget`, `ShelterAlreadyPresent`, `ClearCellNotInArea`, `ClearDuplicateCell`, `UnknownWorkOrder`, `WorkPlanAlreadyApproved`, `PlayerPlanStateStale`).
  - Payload byte budgets on `ma-1` and `ma-1-mixed` at Year 0 and Year +5 **with Pass 2 orders pending and after Execute** (`view <= 524,288 B`, `ecology <= 153,600 B`).
  - Sequential `Plant → Shelter → SpotClear` fence-chaining and partial-failure recovery (`RemovePlanting`).
- Update `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh`:
  - Restore Pass 1 `N-3` (non-Crop status + disabled release option, cross-checked rank-vs-row IDs, exact-equality release label & preview text) and `N-4` (focus-on-entry + `scrollTop === 0` assertions on mode toggle entry and Escape return).
  - Add Pass 2 HTTP workflow (`W` checks) for all 6 new endpoints (`400` malformed/unknown/duplicate/wrong-type/oversized, `409` every refusal code with fresh `view + ecology` and zero mutation, `200` add/remove/approve/execute/year+1 commitment gate, payload budgets after pending & execute).
  - Add Pass 2 page-witness (`J` / `P2` checks) exercising all 4 modes, provisional previews, sequential `Plant + Shelter + SpotClear` queueing + step-3 `409` partial disclosure with one-click remove, `Inspect Cell` clear/remove, `PJ` sapling `Queue 8-year Shelter`, `Escape` order preserving draft Fell ticks, Work Plan targeted removals, Execute confirmation, and Year+1.

### Commit 5 — Full verification, real Edge matrix, and durable handoff (`Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass2/**`)
- Clean builds (`--no-incremental --disable-build-servers`, external `TMPDIR`), 2 identical Core suite runs, 9 frozen anchor checks, `ui-smoke-multiarea.sh`, raw byte payload checks, and real headless Edge CDP matrix (`1920×1080` and `1280×720` on `ma-1` and `ma-1-mixed`), archiving evidence outside Git in `/home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124` (mode 700).
- Confirm servers stopped, protected listeners alive, clean `git status --short`, commit durable handoff, and push `feat/multi-map-integration-pass2-20261124-v1`.
