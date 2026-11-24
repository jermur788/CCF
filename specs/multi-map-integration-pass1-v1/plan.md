# MULTI Map Integration Pass 1 — Implementation Plan

- **Spec ID:** SPEC-multi-map-integration-pass1-V1
- **Spec status:** IMPLEMENTING
- **Plan owner:** Primary Implementer (sole writer on `feat/multi-map-integration-pass1-20261124-v1`)
- **Created:** 2026-11-24
- **Last updated:** 2026-11-24

## 1. Architecture and verified baseline (`cf6fe7a5`)

- Base commit `cf6fe7a52aa096e824827c568f43fe05724dcce9` holds the closed Phase 2 engine (`1/1/1/1/1`) in `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/**` with baseline `0 Warning(s) / 0 Error(s)` and `245/245 passed, 0 failed` under `/home/jer/.dotnet/dotnet` (SDK 8.0.134) with external `TMPDIR=/home/jer/tmp` and pinned `.p0ref/SitkaLab.Core.dll` (`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`).
- Stage 4 Owner UX commit `0e7774557dbb2947b29993cf032c084aa3b7227f` diverges from common ancestor `e7c1a57da519ed1ee03ccfbad240196a66df0905`. Verified by `git apply --check` that the functional diff (`src/Ui/PlayerMultiApi.cs` and `tests/ui-smoke-multiarea.sh`) applies cleanly onto `cf6fe7a5` with zero conflicts and zero biology impact. The 16 Stage 4 PNGs and Markdown/JSON logs under `Docs/Handoffs/.../900-ui-catchup/` remain on `0e777455` (outside Pass 1 writable scope and avoided to prevent branch bloat).
- `SitkaP1.Ui.csproj` is built explicitly in addition to `MultiRackSitkaP1Lab.sln`.
- `MultiAreaTests.cs` (`MA-W12`) checks the exact method list of `MultiAreaWorkPlanAdapter` to prove no fault seam or caller-built plan exists; additive read-only/factory methods on the adapter will be registered there without weakening any seam check.
- `Program.cs` (Core.Tests) checks that `src/**/*.cs` contains no banned file-writing or path-based file APIs; all Pass 1 additions remain purely in-memory.

## 2. Proposed technical design

### 2.1 Commit 1 — Spec quartet + owner visual reference assets
- Commit `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` (all items unticked `[ ]`) and the four binary PNGs in `specs/multi-map-integration-pass1-v1/assets/`.

### 2.2 Commit 2 — Stage 4 Owner UX reconciliation
- Apply `e7c1a57d..0e777455` for `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` and `Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh`.
- Verify clean builds (`.sln` + `SitkaP1.Ui.csproj`) and `245/245` Core tests before any Pass 1 code edits.

### 2.3 Commit 3 — Additive Core additions (`src/Core/MultiArea/**`)
1. **Demo factory (`MultiAreaDemoFactory.cs`):**
   - `OpenExperiment(string key)` / `CreateMixedDemo()` in `src/Core/MultiArea`: builds `mad-ma1-mixed-demo-v1` from `MultiAreaGenerator.Generate()` with Area A 100 % Sitka and Area B sorted by `TreeId` (`StringComparer.Ordinal`) round-robin `i % 3 == 0 -> sitka-spruce`, `1 -> sessile-oak` (age 60), `2 -> beech` (age 60) via `Phase2Options.GenesisSpecies`. Deterministic; leaves `MultiAreaGenerator.Generate()` and `Phase2Support.MixedFixture()` completely untouched.
   - Optional `DisclosureOverride` on `MultiAreaExperiment` (default `null`, falling back to `MultiAreaIdentity.Disclosure`) so `MA-1` manifest bytes and `contentHash` (`c66d566c…f1f95`) stay byte-identical while `MA-1-MIXED` carries truthful mixed-stand disclosure.
2. **Species-aware prune readout (`MultiAreaUiReadout.cs` & `MultiAreaEcology.cs`):**
   - Pure helper computing prune eligibility, lifts, max lifts (`2` for Oak/Beech, `3` for Sitka), minimum interval (`3` yr for Oak/Beech, `5` yr for Sitka), next scheduled target (`3.5 / 6.0 m` for Oak/Beech, `2.5 / 5.0 / 6.5 m` for Sitka), next eligible year, refusal code/reason, and plan state directly via `Phase2Pruning` / `PruningRules`.
   - Passed optional `MultiAreaPhase2State?` into `MultiAreaUiReadouts.Build` so `stage4` and `ecology` agree 100 % on Phase 2 sessions while remaining identical on Stage 4/5/Legacy.
3. **Atomic composite snapshot & compact `ecology` DTO (`MultiAreaEcology.cs`, `MultiAreaContinuation.cs`, `MultiAreaWorkPlanAdapter.cs`):**
   - `MultiAreaContinuation.ReadCompositeUiSnapshot()` acquires `lock (sync)` once and produces `MultiAreaCompositeUiSnapshot` containing `View`, `Readout` (Stage 4 UI readout), `Ecology` (`MultiAreaEcologyDto?`, `null` when `SpeciesModelVersion == 0`), plus `Stage4View`, `Stage5View?`, `Phase2View?` all sharing one exact `sessionId`, `source`, `planStateRevision`.
   - `MultiAreaContinuation.ReadCompositeUiFor(source, planStateRevision)` acquires `lock (sync)` once and returns `(Readout, Ecology)` only when the root still matches, else `null` (zero torn frames on command responses).
   - `MultiAreaEcologyDto` follows §5.4 of `spec.md`: compact numeric `cellsCompact` matching `cellSchema`, sparse `speciesByTreeId`, engine-authoritative `pruneStatusByTreeId`, per-area species summaries & cell counts, `planted[]`, `shelters[]` (with `expiryYear` and `isEffective`), `patches[]`, and read-only `pendingClear[]`/`approvedClear[]`/`pendingPlanting[]`/`approvedPlanting[]`. Positive non-zero floating-point values are formatted/rounded with a minimum positive floor (`1e-4`) so no positive value ever rounds to `0`.
4. **Read-only cell detail (`MultiAreaContinuation.ReadCellDetail` & `MultiAreaWorkPlanAdapter.ReadCellDetail`):**
   - Acquires `lock (sync)` once; validates `fence` (session, full source token, `planStateRevision`), `areaId`, and `(column, row)` in-polygon ground cell.
   - Returns full `MultiAreaCellDetail` (clipped polygon, clipped area, light index, geometric cover, bramble, bracken, browse pressure, contributing tree IDs, all natural Sitka + broadleaf `cohorts[]` bands with exact fields, clear plan state) + fresh `MultiAreaCompositeUiSnapshot` without mutating any state or revision.
5. **Default `/multi` session:**
   - `MultiAreaWorkPlanAdapter.OpenSynthetic(models = null)` defaults to `MultiAreaModelSelection.Phase2` (`1/1/1/1/1`); `OpenExperiment(string experimentKey, MultiAreaModelSelection? models = null)` handles `"ma-1"` and `"ma-1-mixed"`, defaulting to `MultiAreaModelSelection.Phase2`. `new MultiAreaContinuation(experiment)` stays `0/0/0/0/0`.

### 2.4 Commit 4 — UI, CLI, pure inline SVG system, `POST api/multi/cell`, and tests
1. **CLI (`src/Ui/Program.cs`):**
   - Parse `<derivedFixtureDir>` and optional `--multi-experiment=ma-1|ma-1-mixed` (any order); reject unknown/duplicate/malformed args with exit 64 + usage message.
   - Pass experiment key and badge metadata into `MultiAreaWorkPlanAdapter` / `PlayerApi`.
2. **HTTP API (`src/Ui/PlayerMultiApi.cs`):**
   - Every view/command/competition/hit response carries additive `ecology` alongside unbroken `view`, `scenes`, `labels`, `stage4`, obtained atomically via `ReadCompositeUiSnapshot` / `ReadCompositeFor`.
   - `GET api/multi/manifest` serves truthful experiment badge metadata (`experimentBadge`, `experimentKey`, `models`) appended after `"contentHash"` in pure ASCII JSON (preserving M07 canonical body-hash verification for `MA-1` while carrying the exact required strings for both `ma-1` and `ma-1-mixed`).
   - `POST api/multi/cell` behind unchanged `PlayerApi` guards using strict camelCase parser (`MultiCellMembers`: full fence + `areaId` + `column` + `row`, integer type check on `column`/`row`). Returns `200` with full cell detail + fresh composite view, `400` on malformed/unknown/duplicate JSON, `409` with fresh composite view on stale fence or invalid/out-of-bounds cell.
3. **Pure inline SVG system (`MultiAreaPage` in `src/Ui/PlayerMultiApi.cs`):**
   - Delete every `data:image/webp;base64,...` constant.
   - Author crisp, layered inline SVG `<symbol>` and `<pattern>` definitions in `<defs>` for all 21 required IDs, aligned with the 4 committed reference boards and 5 locked visual-semantics rules.
   - Render strictly circular crowns (`x = u - r`, `y = v - r`, `width = height = 2 * r`, seeded rotation allowed, zero elliptical stretch, true-radius hit disc preserved).
   - Map layer order (bottom to top): grid -> ground -> inner grid -> ground overlay (`Light Guide` / `Undergrowth` / `Natural Seedlings`) -> `ClearUndergrowth` cell highlights -> `SpotClear` patches (world-scale `r = patch.radiusM`, beneath `PJ` saplings) -> area outline -> living/standing-dead crowns -> fallen deadwood logs -> planted juveniles (`PJ`) + shelters -> Work Plan marks & Crop rings (`#ccf-crop-ring`) -> selection -> furniture.
4. **Modes, overlays, inspectors, and preserved Stage 4 UX (`MultiAreaPage`):**
   - Truthful header experiment badge (`Synthetic MA-1 (all Sitka)` vs `Synthetic MA-1-MIXED demo — Area A Sitka; Area B ⅓ Sitka / ⅓ Oak / ⅓ Beech (not a surveyed Irish forecast)`).
   - Two explicit modes (`Select Tree`, `Inspect Cell`); cell mode swaps the right-hand inspector body cleanly (never stacks above the tree panel) and fetches cell detail via `POST api/multi/cell`; `Escape` exits cell mode without clearing draft Fell ticks.
   - Single dominant ground overlay selector: `Off | Light Guide | Undergrowth | Natural Seedlings` with compact legend strip and honest labels (`Modelled light · not calibrated PAR`, cell-level presence disclaimer for Natural Seedlings).
   - Zero Pass 2 write buttons or stub controls.
   - Species-aware Prune box in tree inspector (uses `ecology.pruneStatusByTreeId`), species breakdown in area cards/details, fallen deadwood species + `direction schematic, not simulated`.
   - Maintain Stage 4 R3 fold margins at 1280×720 and 1920×1080 (zero horizontal scroll, all 5 blocked competitor rows + `Add` above the fold).
5. **Tests (`tests/Core.Tests/MapIntegrationPass1Tests.cs`, `tests/ui-smoke-multiarea.sh`):**
   - Core unit/integration tests covering `MA-1-MIXED` determinism & species assignment, atomic composite snapshot parity, `ecology` DTO correctness (`species == 0` -> `null`, non-zero floor, species-aware Oak/Beech/Sitka prune readouts, byte-budget assertions on both experiments at Year 0 and Year +5), read-only `ReadCellDetail` (200/409 semantics, zero mutation), and explicit verification of all 9 frozen anchors.
   - Extend `ui-smoke-multiarea.sh` with Pass 1 HTTP checks (`POST api/multi/cell` 200/400/409, zero WebP, SVG IDs, payload byte budgets, CLI flag rejection, `ma-1-mixed` startup).

### 2.5 Commit 5 — Verification, real Edge matrix, and durable handoff
- Run clean builds (`--no-incremental --disable-build-servers`), two consecutive full Core test runs, `ui-smoke-multiarea.sh`, raw byte payload measurements, and dependency-free Windows Node + headless Microsoft Edge CDP verification at `1920×1080` and `1280×720` on both `ma-1` and `ma-1-mixed` (saving raw logs/screenshots in external mode-700 evidence directory `/home/jer/Development/CCF-Multi-MapIntegration-Pass1-Evidence-20261124`).
- Confirm ephemeral server stopped and protected listeners `:33767`, `:43649`, `:46593` remain alive.
- Commit durable handoff at `Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass1/MULTI-MAP-INTEGRATION-PASS1-NEON-IMPLEMENTATION-20261124.md` and push `feat/multi-map-integration-pass1-20261124-v1`.

## 3. Payload budget risk and mitigation

Baseline `GET api/multi/view` size is measured before implementing the `ecology` DTO. Because `stage4` already transmits clipped cell polygons (`ground[]`), `ecology.cellsCompact` omits all polygon vertices and per-cell contributor IDs, encoding only 8 compact numbers per cell (`[areaIndex, col, row, light, bramble, bracken, speciesMask, bandCount]`). Full per-cell cohort bands and contributors are loaded lazily via `POST api/multi/cell`. If total payload approaches 512 KiB, compact float rounding (preserving non-zero floor) and sparse maps (`speciesByTreeId` omitting Sitka) keep both `ecology` well under 150 KiB and total view well under 512 KiB.
