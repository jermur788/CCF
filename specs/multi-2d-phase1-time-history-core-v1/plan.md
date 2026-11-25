# CCF Multi 2D — Phase 1 Step 1: Time Pacing, History Query & Woodland Structure Descriptors — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-core-V1`
- **Spec status:** `IMPLEMENTING`
- **Plan owner:** Primary Implementer (Gemini 4 Neon, sole writer on `feat/multi-2d-phase1-time-history-core-20261124-v1`)
- **Base commit:** `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43`
- **Created:** 2026-11-25

---

## 1. Architecture and minimal-touch design

### 1.1 Narrow continuation wiring (`partial class MultiAreaContinuation`)
- Change `internal sealed class MultiAreaContinuation` to `internal sealed partial class MultiAreaContinuation` in `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` (single-line edit).
- Place the thin locking/delegation methods for each module inside its own new source file as a matching `internal sealed partial class MultiAreaContinuation` declaration:
  - `MultiAreaTimePacing.cs`: `PreviewNextReviewYear(MultiAreaFence)` and `AdvanceYears(MultiAreaFence, int, CancellationToken)` (plus internal test seam `AdvanceYears(MultiAreaFence, int, CancellationToken, Action<int>?)` for deterministic between-step cancellation testing).
  - `MultiAreaHistoryQuery.cs`: `ReadHistoryIndex(int pageSize = 10, int? beforeYear = null)` and `ReadHistoryYear(int year)`.
  - `WoodlandStructureMetrics.cs`: `ComputeStructure()`.
- Leave `MultiAreaContracts.cs`, `MultiAreaWorkPlanAdapter.cs`, `src/Core/Phase1/**`, `src/Ui/**`, and all biology/RNG/fixture files 100% untouched.
- Ensure none of the new `src/Core/` files contain any file-I/O API tokens banned by the `644 B-01` source guard in `tests/Core.Tests/Program.cs`.

### 1.2 Module A (`MultiAreaTimePacing.cs`)
- `TimePacingStopReason` enum with exactly 6 values: `ReachedReviewYear`, `WorkPlanOpen`, `InvalidTargetYear`, `StandCannotAdvance`, `CancelledBetweenYears`, `AnnualStepRefused`.
- `ReviewYearPreview`, `AdvanceYearStepSummary`, `AdvanceYearsResult` immutable sealed records.
- `MultiAreaTimePacing` static pure helper computing:
  - `LatestRecordedFellYear(MultiAreaRoot)` scanning `root.Events -> Areas -> Fells` for maximum `FellRecord.EcologicalYear`.
  - `HasOpenWork(MultiAreaRoot)` checking `root.HasUnexecutedWork` plus defensive checks on Stage 4/5/Phase 2 approved lists.
  - `CanAdvanceStand(MultiAreaRoot)` matching `MultiAreaContinuation.Advance`'s exact empty-stand check (`living adults || Stage5.HasAbundance || Phase2.HasJuveniles`).
  - `Preview(MultiAreaRoot)` applying the locked Option A rules (`SuggestedGapYears = 4`, `MaxTicksPerPress = 5`, pre-Fell quiet observation vs post-Fell `anchor + 4` vs overdue fallback `currentYear + 4`, clamped to `currentYear + 5`).
- `MultiAreaContinuation.AdvanceYears` holds `lock (sync)` across the loop, calls `RequireFence(fence)` once up front, refuses `WorkPlanOpen` and `InvalidTargetYear` with zero mutation, checks `cancellationToken.IsCancellationRequested` strictly before starting each annual step (never mid-step), mints a fresh `MultiAreaFence(sessionId, Token(root), root.PlanStateRevision)` per step, and delegates each step to the unchanged `Advance(stepFence, consentToCancelWork: false)`.

### 1.3 Module B (`MultiAreaHistoryQuery.cs`)
- `HistoryProvenance` enum with exactly 6 values (`Recorded`, `Current`, `Derived`, `Reconstructed`, `SessionOnly`, `NotRecorded`) plus `HistoryProvenanceCatalog` mapping each value to its exact wire label (`RECORDED`, `CURRENT`, `DERIVED`, `RECONSTRUCTED`, `SESSION-ONLY`, `NOT RECORDED`) and D-073 / S1-C tier.
- Pure `MultiAreaHistoryQuery` static class operating solely on an immutable `MultiAreaRoot` snapshot (plus optional immutable `AreaHa` lookup from `MultiAreaContinuation`):
  - `ReadIndex(MultiAreaRoot, int pageSize, int? beforeYear)`: builds the descending union of years reachable from `AnnualHistory` (`YearBefore`/`YearAfter`), `Events`, `Stage4.PruneLog`, `Stage5.ClearLog`, and `Phase2.WorkLog`; applies strict `< beforeYear` keyset filtering and `pageSize` bound; populates `ExecutedActionCounts`, `HasAnnualOutcome` (true iff `YearAfter == Year`), `HasObservedOutcomeNextYear` (true iff `YearBefore == Year && YearAfter == Year + 1`), and `LiveStemsEndOfYear` (`After.LivingCount` sum only when `HasAnnualOutcome == true`, else `null`).
  - `ReadYear(MultiAreaRoot, int year, IReadOnlyDictionary<string, float>? areaHaById)`: groups executed work in `WorkYear = year` by area and action type with exact counts and IDs (`RECORDED`); binds `AnnualOutcomeNextYear` (`YearBefore == year && YearAfter == year + 1`) and `AnnualRecordEndingInYear` (`YearAfter == year`), exposing recorded whole-woodland `Phase2PlantedAccount` totals without fabricated per-area splits; constructs `LaterObservedComparison` (`CURRENT` live stems, basal area, deadwood, natural cohort presence, planted juvenile outcomes + `DERIVED` deltas since `ObservedYear`) with the exact non-causal wording and zero weather/economy/euro fields; attaches explicit `NOT RECORDED` disclosures for past canopy cover, past tree-by-tree structure, and per-area `Phase2PlantedAccount` splits.

### 1.4 Module C (`WoodlandStructureMetrics.cs`)
- Pure static `WoodlandStructureMetrics.Compute(MultiAreaRoot root)`:
  - Collects living adult trees across all areas in `root.Areas` (`t.IsLiving`); excludes dead stems, stumps, deadwood, and unpromoted `PJ` saplings; zero off-map synthetic neighbours.
  - Resolves species from `root.Phase2?.TreeSpecies` (defaulting to `sitka-spruce`).
  - For each focal living tree when $N \ge 2$, selects the $k = \min(3, N - 1)$ nearest other living trees by squared Euclidean $(X, Z)$ distance in `double` with exact ties broken by `StringComparer.Ordinal` on `TreeId`.
  - Computes $M_i \in [0, 1]$ and $T_i \in [0, 1]$ (over valid pairs with $\max(d_i, d_j) > 0$; `null` when $m_i = 0$).
  - Aggregates per-area means by focal tree's owning `AreaId` and whole-woodland mean, returning `MaxNeighbourCountK = 3`, `EffectiveNeighbourCount`, `Provenance = HistoryProvenance.Derived`, and the exact honesty label.

---

## 2. Commit sequence

1. **Commit 1:** `spec: add multi-2d phase1 time-history core quartet`
   - `specs/multi-2d-phase1-time-history-core-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`
2. **Commit 2:** `feat(core): add dual time pacing, read-only history query and woodland structure descriptors`
   - `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/{MultiAreaTimePacing.cs,MultiAreaHistoryQuery.cs,WoodlandStructureMetrics.cs,MultiAreaContinuation.cs}`
3. **Commit 3:** `test(core): add P1-TH-01..08 and save-continuity gates`
   - `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase1TimeHistoryTests.cs` + 1 registration line in `tests/Core.Tests/Program.cs`
   - New Step 1 save-continuity cases in `tests/Phase1.Save.Tests/Program.cs` (preserving all 48 existing cases untouched)
4. **Commit 4:** `docs(phase1-2d): record Step 1 implementation and verification handoff`
   - `Docs/Handoffs/MultiRack-Simulation-20261116/phase1-2d/PHASE1-2D-STEP1-TIME-HISTORY-CORE-NEON-IMPLEMENTATION-20261124.md`

---

## 3. Verification plan

Using `~/.dotnet` (SDK 8.0.134) and external `TMPDIR=/home/jer/ccf-p1th-tmp`:
1. `dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers` → 0 warnings, 0 errors.
2. `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll` → 267 baseline + all new P1-TH cases passing, 0 failed, all 9 frozen anchors unchanged.
3. `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` → 48 baseline + new Step 1 continuity cases passing, 0 failed.
4. `dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers` → 0 warnings, 0 errors.
5. Verify `.p0ref/SitkaLab.Core.dll` sha256, `git diff --name-only 5438c94c..HEAD` scope compliance, `git status --short` empty, and protected listeners `:33767`, `:43649`, `:46593` alive.
