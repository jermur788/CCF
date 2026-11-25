# CCF Multi 2D — Phase 1 Step 1: Time Pacing, Decision→Outcome History Query & Woodland Structure Descriptors — Specification

- **Spec ID:** `SPEC-multi-2d-phase1-time-history-core-V1`
- **Feature ID:** `MULTI-2D-PHASE1-TIME-HISTORY-CORE-V1`
- **Status:** `IMPLEMENTING`
- **Owner:** Project owner (product decisions); Primary Implementer (Gemini 4 Neon) under single-writer lane custody
- **Created:** 2026-11-25
- **Last updated:** 2026-11-25
- **Base commit:** `5438c94c66dfab14e3c1a1e924f9d90b0b0e5f43` (`feat/multi-2d-phase1-save-e2e-20261124-v1`, accepted unmerged baseline)
- **Task branch:** `feat/multi-2d-phase1-time-history-core-20261124-v1`
- **Decision log references:** D-004 (core loop), D-008 (discrete annual steps), D-020 (presentation authority), D-030 (state honesty), D-063 (selective SDD), D-073 (decision → outcome history priority & provenance tiers), D-074 (multi-year CCF intervention spacing framing), D-075 (dual time-advance workflow), D-077 (4-year suggested gap / 5-tick cap GAMEPLAY CALIBRATION, no fabricated acknowledgements, year-by-year authoritative stepping)
- **Milestone:** MultiRack Sitka 2D standalone simulator (`Tools/MultiRackSitkaP1Lab`) — Phase 1 Step 1 (Core-only time pacing, history query, and spatial structure descriptors)
- **Supersedes / superseded by:** Builds strictly additively on the Phase 1 Save/Load baseline (`5438c94c…`). Does not alter Save SchemaVersion 2 (`src/Core/Phase1/`), UI (`src/Ui/`), or any biology/anchor.

---

## 1. Authority

This specification is task-level implementation authority for Phase 1 Step 1 only. It remains subordinate to:

1. Latest direct user / product-owner instruction;
2. `Docs/Project/decision-log.md` (notably D-073, D-074, D-075, D-077);
3. `Docs/Project/current-milestone.md`;
4. `Docs/Project/game-brief.md`.

Implementation truth remains the inspected repository state (`Tools/MultiRackSitkaP1Lab/src/Core/`). A specification states intended behaviour and acceptance criteria; it never proves runtime verification or acceptance.

---

## 2. Product intent and scope

The 2D two-area simulator (`Tools/MultiRackSitkaP1Lab`) now supports full Phase 2 mixed-species ecology (`1/1/1/1/1`), interactive six-verb Work Plans (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`), and local SchemaVersion 2 Save/Load. However, its Core engine only exposes single-year stepping (`Advance`), raw ledger arrays (`Events`, `AnnualHistory`, `PruneLog`, `ClearLog`, `WorkLog`), and no spatial stand-structure descriptors.

Phase 1 Step 1 adds three purely Core, deterministic, read-only/authoritative modules inside `SitkaP1.Core`:

1. **Module A — Dual Time Pacing (`MultiAreaTimePacing.cs`):** deterministic preview of the next suggested management review year and bounded multi-year stepping (`AdvanceYears`, 1–5 years per press) that loops the existing authoritative single-year `Advance` under `MultiAreaContinuation`'s single lock without reimplementing biology, mutating on refusal, or fabricating records.
2. **Module B — Read-Only Decision → Outcome History Query (`MultiAreaHistoryQuery.cs`):** newest-first, cursor-stable paged year index and per-year decision → outcome detail over one immutable committed `MultiAreaRoot`, preserving real 2D timing (`WorkYear = Y` vs `ObservedYear = Y + 1`), a six-value provenance vocabulary aligned to D-073, non-causal framing, and explicit `NOT RECORDED` disclosures.
3. **Module C — Woodland Structure Descriptors (`WoodlandStructureMetrics.cs`):** pure, non-mutating computation of nearest-neighbour Species Mingling ($M_i$) and Diameter Differentiation ($T_i$) up to $k = 3$ living adult trees across the connected woodland, with per-area and whole-woodland means and explicit non-biodiversity/non-forecast honesty labelling.

---

## 3. Out of scope and strict prohibitions

Phase 1 Step 1 MUST NOT modify or introduce any of the following:

- **UI / HTTP / HTML / CSS / JS (`Tools/MultiRackSitkaP1Lab/src/Ui/**`):** zero edits in Step 1. UI wiring belongs to a separate subsequent step.
- **Save format and persistence (`Tools/MultiRackSitkaP1Lab/src/Core/Phase1/**`):** `Phase1SaveFormat.SchemaVersion = 2`, `Phase1SaveCodec`, `Phase1SaveValidator`, `Phase1SaveSlots`, and `Phase1RootMapping` stay 100% untouched. Step 1 persists zero new schema fields; all pacing, history, and structure queries derive deterministically from already-persisted SchemaVersion 2 state.
- **Core biology, constants, RNG streams, fixtures, `.p0ref`, and frozen anchors:** zero changes to `AnnualRules`, `CrownModel`, `Stage4*`, `Stage5*`, `Phase2*`, `MultiAreaGenerator`, `SitkaGrowthModel`, `SimulationRandom`, or `.p0ref/SitkaLab.Core.dll` (`9a99f781…34b3`).
- **Unity project, packages, dependencies, and repo-level governance:** zero edits to `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/coordination/**`, `main`, tags, PRs, or `.vscode/`. Zero new packages, projects, DI containers, or service locators.
- **Unmodelled 2D concepts:** economy, money, prices, revenue, cash, `€0` placeholders, weather, storms, teaching-gate states, unread-review locks, scenario year caps, and objective/outcome triggers do NOT exist in 2D Phase 1 and MUST NOT be fabricated or added as enum values or DTO fields.

---

## 4. Module A — Dual Time Pacing (`MultiAreaTimePacing.cs`)

### 4.1 Callable surface

Exposed as `internal` methods on `MultiAreaContinuation` (via `partial class MultiAreaContinuation` in `MultiAreaTimePacing.cs`), both acquiring `lock (sync)` once and validating the caller's fence via the engine's existing `RequireFence(fence)`:

```csharp
internal ReviewYearPreview PreviewNextReviewYear(MultiAreaFence fence);
internal AdvanceYearsResult AdvanceYears(MultiAreaFence fence, int requestedTargetYear, CancellationToken cancellationToken = default);
```

A stale or mismatched fence throws the engine's existing `P1Refusal` (`PlayerSessionMismatch`, `PlayerSourceStale`, or `PlayerPlanStateStale`) with zero mutation.

### 4.2 Constants and GAMEPLAY CALIBRATION framing (Owner Option A, LOCKED)

- `SuggestedGapYears = 4`
- `MaxTicksPerPress = 5`
- Both constants are explicitly framed as **GAMEPLAY CALIBRATION** (D-077), NOT universal Irish forestry advice, NOT a field thinning prescription, and NOT a compulsory intervention interval.
- Exact `CalibrationNote` string exposed on both `ReviewYearPreview` and `AdvanceYearsResult`:
  `"GAMEPLAY CALIBRATION — 4-year suggested review gap and 5-year per-press cap are gameplay calibration for pacing observation intervals, not universal Irish forestry advice or a compulsory thinning interval."`

### 4.3 StopReason vocabulary (exact 6 values)

`internal enum TimePacingStopReason` defines **only** these six values:

| Value | Meaning |
|---|---|
| `ReachedReviewYear` | All requested annual steps committed cleanly and `DestinationYear == RequestedTargetYear`. |
| `WorkPlanOpen` | Unexecuted work exists (`Pending`, `Approved`, Stage 4 pending/approved Prune, Stage 5 pending/approved ClearUndergrowth, or Phase 2 pending/approved Plant/Shelter/SpotClear). Zero mutation; never auto-approves, auto-executes, or cancels work. |
| `InvalidTargetYear` | `requestedTargetYear <= currentYear` or `(long)requestedTargetYear - currentYear > 5`. Zero mutation. |
| `StandCannotAdvance` | Single-year `Advance` refused with `ContinuationNoLivingStems` (every stand has no living adult stems and no Stage 5 / Phase 2 juvenile regeneration to advance). Pre-step root intact. |
| `CancelledBetweenYears` | `cancellationToken.IsCancellationRequested` observed before starting the next annual step (strictly between completed annual steps). Returns years already committed cleanly with zero partial year or fabricated remainder records. |
| `AnnualStepRefused` | Any other typed `P1Refusal` thrown by the authoritative single-year `Advance`. Pre-step root intact. |

**Forbidden stop reasons:** `StormEvent`, `TeachingGateIncomplete`, `ReviewUnread`, `YearCapReached`, `ObjectiveOrOutcomeReached` MUST NOT exist anywhere in `TimePacingStopReason`.

### 4.4 Preview contract (`ReviewYearPreview`)

Fields:
- `int CurrentYear` — `root.EcologicalYear`.
- `bool Eligible` — `true` unless unexecuted work is open (`WorkPlanOpen`) or the stand cannot advance (`StandCannotAdvance`).
- `TimePacingStopReason? StopReason` — `null` when `Eligible == true`; `WorkPlanOpen` when open work exists (checked first); `StandCannotAdvance` when no open work exists but every stand is empty with no regeneration.
- `int? AnchorFellYear` — maximum `MultiAreaFellRecord.EcologicalYear` across every `MultiAreaWorkEvent.Areas[].Fells` in `root.Events`; `null` when no successful recorded Fell exists. Non-Fell events (e.g. Prune, ClearUndergrowth, Plant, Shelter, SpotClear only) do NOT set an anchor. Rack-access felling counts for navigation anchoring only and is never relabelled as a substantive CCF intervention (D-077 P-2).
- `bool IsObservationPeriod` — `AnchorFellYear is null`.
- `int SuggestedTargetYear`:
  - **Pre-first-Fell (`AnchorFellYear is null`):** `Math.Min(currentYear + 4, currentYear + 5)` (= `currentYear + 4`). Framed explicitly as a quiet **OBSERVATION** period, NOT a completed silvicultural thinning cycle.
  - **Post-Fell (`AnchorFellYear is int anchor`):** `candidate = anchor + 4`. If `candidate > currentYear`, `Math.Min(candidate, currentYear + 5)`. If `anchor + 4 <= currentYear` (overdue), `Math.Min(currentYear + 4, currentYear + 5)` (= `currentYear + 4`).
- `int SuggestedGapYears` (`= 4`), `int MaxTicksPerPress` (`= 5`), `string CalibrationNote`, `string Label`.

### 4.5 Advance contract (`AdvanceYearsResult`)

Fields:
- `int StartYear`, `int DestinationYear`, `int RequestedTargetYear`
- `IReadOnlyList<AdvanceYearStepSummary> CompletedYears` — one entry per committed annual step (`YearBefore`, `YearAfter`, committed `MultiAreaYearRecord`, `MultiAreaAnnualResult`)
- `bool ReachedTarget`
- `TimePacingStopReason StopReason`
- `MultiAreaFence FreshFence` (`SessionId`, `Token(root)`, `root.PlanStateRevision` from the post-call committed root)
- `bool IsObservationPeriod`, `int? AnchorFellYear`, `string CalibrationNote`
- `string? StopDetail` (nullable diagnostic reason when stopped early or refused)

Behaviour:
1. Acquire `lock (sync)` once; run `RequireFence(fence)` (stale fence throws `P1Refusal` with zero mutation).
2. If any unexecuted work exists (`WorkPlanOpen`), return immediately with `DestinationYear = StartYear`, empty `CompletedYears`, `ReachedTarget = false`, `StopReason = WorkPlanOpen`, zero mutation. Never pass `consentToCancelWork: true`.
3. If `requestedTargetYear <= StartYear` or `(long)requestedTargetYear - StartYear > MaxTicksPerPress`, return immediately with `DestinationYear = StartYear`, empty `CompletedYears`, `ReachedTarget = false`, `StopReason = InvalidTargetYear`, zero mutation.
4. Loop while `root.EcologicalYear < requestedTargetYear`:
   - Check `cancellationToken.IsCancellationRequested` **strictly between annual steps** (never mid-step). If cancelled, return `CompletedYears` already committed, `DestinationYear = root.EcologicalYear`, `ReachedTarget = false`, `StopReason = CancelledBetweenYears`.
   - Mint fresh fence from the current committed root (`new MultiAreaFence(sessionId, Token(root), root.PlanStateRevision)`) and invoke the engine's authoritative single-year `Advance(stepFence, consentToCancelWork: false)`.
   - Catch `P1Refusal`: map `ContinuationNoLivingStems` → `StandCannotAdvance`, `WorkPendingConsentRequired` → `WorkPlanOpen`, any other refusal → `AnnualStepRefused`; return already-completed years with the pre-step root intact and `ReachedTarget = false`.
   - Append `(yearRecord.YearBefore, yearRecord.YearAfter, yearRecord, cmd.Annual!)` to `CompletedYears`.
5. When `root.EcologicalYear == requestedTargetYear`, return `ReachedTarget = true`, `StopReason = ReachedReviewYear`.

---

## 5. Module B — Read-Only Decision → Outcome History Query (`MultiAreaHistoryQuery.cs`)

### 5.1 Callable surface and purity

Pure read-only queries over one immutable committed `MultiAreaRoot` (zero mutation, zero ecological re-simulation, zero RNG, zero cache mutation). Thin continuation wrappers lock `sync` once, snapshot `root` (and immutable per-area `AreaHa`), and delegate outside or cleanly over the snapshot:

```csharp
internal HistoryIndexPage ReadHistoryIndex(int pageSize = 10, int? beforeYear = null);
internal HistoryYearDetail ReadHistoryYear(int year);
```

Direct pure entry points (`MultiAreaHistoryQuery.ReadIndex(...)`, `MultiAreaHistoryQuery.ReadYear(...)`) are also callable on any `MultiAreaRoot` snapshot.

### 5.2 Six-value Provenance vocabulary (D-073 alignment)

`internal enum HistoryProvenance` has **exactly six values**:

| Enum value | Wire label | D-073 / S1-C tier | Meaning |
|---|---|---|---|
| `Recorded` | `RECORDED` | L1 Recorded historical fact | Committed at the time work executed or an annual step closed (`Events`, `AnnualHistory`, `Stage4.PruneLog`, `Stage5.ClearLog`, `Phase2.WorkLog`, `Stage5.Accounts`, `Phase2.Accounts`). |
| `Current` | `CURRENT` | L2 Current observed state | Read directly from the current committed `MultiAreaRoot` (live stems, current basal area, current deadwood, current natural cohort presence, current planted juvenile state). |
| `Derived` | `DERIVED` | L3 Derived comparison | Computed deterministically at query time from `RECORDED` and/or `CURRENT` state (deltas since observed year, counts, woodland structure descriptors). |
| `Reconstructed` | `RECONSTRUCTED` | L3 Reconstructed | Assembled by cross-referencing related committed records rather than a dedicated event field (e.g., tracing current status of planted juveniles back to `WorkYear` planting records via `JuvenileId`). |
| `SessionOnly` | `SESSION-ONLY` | L2 / Session-only | Ephemeral session cache or non-historical read-out (e.g., current canopy/light cache) that is not recorded as a historical per-year archive. |
| `NotRecorded` | `NOT RECORDED` | Absent (never fabricated) | Historical quantity not stored by the 2D engine; explicitly reported as `NOT RECORDED` and never back-filled from current state. |

### 5.3 Real 2D timing (`WorkYear = Y` vs `ObservedYear = Y + 1`)

In the 2D simulator, executing an approved Work Plan does **not** advance the calendar (`EcologicalYear` stays `Y`):
- `MultiAreaFellRecord.EcologicalYear`, `MultiAreaPruneRecord.EcologicalYear`, `MultiAreaClearUndergrowthRecord.EcologicalYear`, and `Phase2WorkRecord.EcologicalYear` all record `WorkYear = Y`.
- The subsequent annual ecological step transitions `YearBefore = Y` → `YearAfter = Y + 1`, committing a `MultiAreaYearRecord` (and Stage 5 / Phase 2 annual accounts) for `ObservedYear = Y + 1`.
- `HistoryYearDetail` MUST preserve this separation:
  - **Executed work in `WorkYear = Y`:** grouped by `AreaId` and action type (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`), tagged `RECORDED` in Year `Y`, with exact counts and recorded IDs (`TreeId`, `(Column, Row)`, `OrderId`, `JuvenileId`, `ShelterId`, `PatchId`, `SpeciesId`). Never relabel Year `Y` cuts as Year `Y + 1`.
  - **Annual outcome in `ObservedYear = Y + 1`:** the `MultiAreaYearRecord` with `YearBefore == Y && YearAfter == Y + 1` (plus matching Stage 5 / Phase 2 annual accounts when present), tagged `RECORDED`.
  - **Year `Y` annual record when present:** the `MultiAreaYearRecord` with `YearAfter == Y` (the annual step ending at Year `Y`; absent for `Y == 0`).

### 5.4 Paged History Index (`HistoryIndexPage`)

- **Recorded year set:** distinct union of every year reachable from `AnnualHistory` (`YearBefore` and `YearAfter`), `Events` (`EcologicalYear` and `FellRecord.EcologicalYear`), `Stage4.PruneLog` (`EcologicalYear`), `Stage5.ClearLog` (`EcologicalYear`), and `Phase2.WorkLog` (`EcologicalYear`), ordered strictly **newest-first** (descending).
- **Pagination:** keyset cursor `beforeYear` returns years strictly `< beforeYear`, capped at `pageSize` (refuses `pageSize < 1` with `HistoryPageSizeInvalid`). `NextCursorBeforeYear` is the last year on the returned page when older recorded years remain, or `null` on the final page. Keyset paging is cursor-stable even when new years are appended at the top of a long run.
- **Entry fields (`HistoryIndexEntry`):**
  - `int Year`
  - `bool HasExecutedWork`
  - `ExecutedActionCounts ExecutedActionCounts` (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`)
  - `bool HasAnnualOutcome` — `true` iff a `MultiAreaYearRecord` with `YearAfter == Year` exists (i.e., an annual step closed at `Year`). If a year has work (e.g. Year 0 before advance, or Year 0 genesis work) and no annual record ending at `Year`, `HasAnnualOutcome = false` and `LiveStemsEndOfYear = null` (never invented).
  - `bool HasObservedOutcomeNextYear` — `true` iff a `MultiAreaYearRecord` with `YearBefore == Year && YearAfter == Year + 1` exists.
  - `int? LiveStemsEndOfYear` — `record.Areas.Sum(a => a.After.LivingCount)` when `HasAnnualOutcome == true`; `null` otherwise.
- **Page fields:** `Items`, `PageSize`, `BeforeYear`, `NextCursorBeforeYear`, `TotalRecordedYears`.

### 5.5 Later observed comparison, honesty rules, and explicit omissions

- `LaterObservedComparison` exposes current live stems, current basal area, current deadwood (when Stage 4 active), current natural cohort presence/abundance (when Stage 5 active), and current planted juvenile survival/promotions (when Phase 2 active), tagged `CURRENT`, plus derived deltas against `ObservedYear = Y + 1` tagged `DERIVED`.
- **Non-causal wording (exact required text):**
  `"Observed changes reflect ongoing natural growth, competition, seeding, and light alongside recorded management — not a single-cause effect."`
  No mention of weather (2D does not model weather).
- **Mandatory `NOT RECORDED` disclosures:**
  1. Full past-year overhead canopy cover (`NOT RECORDED`);
  2. Past tree-by-tree structure (`NOT RECORDED`);
  3. Per-area splits of species-only `Phase2PlantedAccount` totals (`NOT RECORDED`; recorded whole-woodland species totals are exposed honestly and never split across areas).
- **Omitted fields:** economy, money, prices, revenue, cash, budget, objectives, and euro (`€` / `€0`) fields are omitted completely from every history DTO and text string until Phase 2B.

---

## 6. Module C — Woodland Structure Descriptors (`WoodlandStructureMetrics.cs`)

### 6.1 Callable surface and purity

```csharp
internal static WoodlandStructureSummary Compute(MultiAreaRoot root); // on WoodlandStructureMetrics
internal WoodlandStructureSummary ComputeStructure();                 // on MultiAreaContinuation
```

Pure read-only, deterministic, non-mutating. `ComputeStructure()` locks `sync`, snapshots `root`, and delegates to `WoodlandStructureMetrics.Compute(snapshot)`. NEVER called from `Advance` or any mutation path.

### 6.2 Exact mathematical definition

- **Population scope:** all living adult trees across the connected simulated woodland: `root.Areas.SelectMany(a => a.State.Trees).Where(t => t.IsLiving)`. Excludes felled stumps, standing dead, fallen deadwood, and unpromoted `PJ` saplings (`Phase2.Juveniles`). Zero off-map synthetic neighbours.
- **Neighbour count:** let $N = \text{living count}$. When $N \ge 2$, every focal living tree $i$ uses $k_i = \min(3, N - 1)$ nearest other living trees ($j \neq i$) across **all areas** (cross-rack borders included), ordered by Euclidean distance in $(X, Z)$: $d_{ij}^2 = (X_j - X_i)^2 + (Z_j - Z_i)^2$. Exact distance ties break deterministically by ordinal `TreeId` (`StringComparer.Ordinal`).
- **Species determination:** `sitka-spruce` unless `root.Phase2.TreeSpecies` maps `TreeId` to a broadleaf species (`sessile-oak` or `beech`). Pure Sitka stands (`Phase2 == null` or no broadleaf entries) therefore yield $M_i = 0.0$ whenever $N \ge 2$.
- **Species Mingling ($M_i$):** for $k_i \ge 1$,
  $$M_i = \frac{1}{k_i} \sum_{j=1}^{k_i} \mathbb{I}(s_i \neq s_j) \in [0, 1].$$
- **Diameter Differentiation ($T_i$):** over valid neighbour pairs $j \in \{1, \dots, k_i\}$ where $\max(d_i, d_j) > 0$ (and $d_i, d_j \ge 0$ finite), let $m_i$ be the count of valid pairs:
  $$T_i = \frac{1}{m_i} \sum_{\text{valid } j} \left(1 - \frac{\min(d_i, d_j)}{\max(d_i, d_j)}\right) \in [0, 1] \quad (m_i > 0), \qquad T_i = \text{null} \quad (m_i = 0).$$
- **Small counts:**
  - $N \in \{2, 3\}$: computes with $k_i = 1$ or $2$, reporting `MaxNeighbourCountK = 3` and `EffectiveNeighbourCount = k_i`.
  - $N \in \{0, 1\}$: returns `MeanMingling = null`, `MeanDiameterDifferentiation = null`, `EffectiveNeighbourCount = 0`.
  - For $T_i$ specifically: returns `null` when no focal tree in the aggregation has any valid positive-DBH neighbour pair.
- **Aggregation:** per-area means grouped by each focal tree's owning `AreaId` (preserving `root.Areas` order; an area with 0 living trees reports `FocalLivingTreeCount = 0`, `EffectiveNeighbourCount = 0`, `MeanMingling = null`, `MeanDiameterDifferentiation = null`), plus the whole-woodland mean over all $N$ focal living trees.
- **Provenance & exact honesty label:**
  - `Provenance = HistoryProvenance.Derived` (`"DERIVED"`)
  - `HonestyLabel = "Spatial structure descriptor (up to k=3 nearest living trees) — not measured biodiversity, habitat quality, or an Irish forecast."`

---

## 7. Frozen anchors (all 9 must remain exact)

| # | Anchor | Expected SHA-256 |
|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` |
| 3 | Multi-area `0/0/1/0` (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` |
| 8 | P0 Core binary pin (`.p0ref/SitkaLab.Core.dll`) | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` |

---

## 8. Acceptance criteria (`P1-TH-01`..`P1-TH-08`, `P1-TH-REG`, `P1-TH-REVIEW`)

- **P1-TH-01:** Year-0 observation preview pre-first-Fell (`SuggestedTargetYear = currentYear + 4`, `IsObservationPeriod = true`, observation and GAMEPLAY CALIBRATION labels); post-Fell anchoring to latest recorded `FellRecord.EcologicalYear` (`Y + 4`, `MaxTicksPerPress = 5`); overdue fallback (`anchor + 4 <= currentYear` → `currentYear + 4`, clamped `+5`); non-Fell work does not set a Fell anchor; stale fence refused via `RequireFence`.
- **P1-TH-02:** Dual skip-vs-manual parity on both `MA-1` and `MA-1-MIXED`, covering pre-Fell observation skip and post-Fell review skip: full canonical state (`CaptureSave` + `Phase1SaveCodec` canonical bytes with session authority normalised) and annual history match $N$ direct single-year `Advance` calls byte for byte.
- **P1-TH-03:** `WorkPlanOpen` refusal for Pending and Approved orders of every type (`Fell`, `Prune`, `ClearUndergrowth`, `Plant`, `Shelter`, `SpotClear`) with zero mutation; `InvalidTargetYear` refusal for target $\le$ currentYear and target $>$ currentYear + 5 with zero mutation; `CancellationToken` honoured strictly between annual steps returning `CancelledBetweenYears`, completed years only, and zero partial year or fabricated remainder entries.
- **P1-TH-04:** SchemaVersion 2 Save → Load round-trip continuity both pre-Fell and post-Fell (+ several years): loading into a fresh `MultiAreaContinuation` yields identical `PreviewNextReviewYear`, `AdvanceYears`, `ReadHistoryIndex`, `ReadHistoryYear`, and `ComputeStructure` results compared to an uninterrupted control; `Phase1SaveFormat.SchemaVersion == 2` and `src/Core/Phase1/` remain untouched.
- **P1-TH-05:** Newest-first keyset paging with cursor stability over $\ge 15$ years; `WorkYear = Y` vs `ObservedYear = Y + 1` timing preserved without relabelling; all six provenance labels present and mapped onto D-073 tiers; canopy history, past tree-by-tree structure, and per-area `Phase2PlantedAccount` splits marked `NOT RECORDED`; recorded whole-woodland `Phase2PlantedAccount` exposed without fabricated per-area splits; non-causal wording present; zero weather claim; zero economy/money/price/objective/euro fields.
- **P1-TH-06:** Hand-checked analytical oracles for `WoodlandStructureMetrics`: pure Sitka $M_i = 0.0$; mixed 4-tree and 6-tree fixtures matching exact analytical $M_i$ and $T_i$; 2-tree and 3-tree stands using $k_i = 1, 2$ with `MaxNeighbourCountK = 3`; 0-tree, 1-tree, and zero-positive-DBH-pair ($T_i$) stands returning `null`; exact distance ties broken by ordinal `TreeId`; stumps/dead/unpromoted `PJ` excluded; cross-rack neighbours included across shared borders; `Compute` mutates nothing (full canonical root identical before/after).
- **P1-TH-07:** Honesty / Gate G3 assertions for `GAMEPLAY CALIBRATION`, quiet observation period (not completed thinning cycle), non-causal history (no weather), spatial-structure-only non-biodiversity/non-forecast label, and absence of unmodelled 3D `StopReason` values.
- **P1-TH-08:** Scope compliance: zero changes under `src/Ui/`, `src/Core/Phase1/`, and `Assets/` relative to base `5438c94c…`.
- **P1-TH-REG:** Clean build with `--disable-build-servers` and external `TMPDIR`, 0 warnings / 0 errors across `MultiRackSitkaP1Lab.sln`, `SitkaP1.Ui.csproj`, and `Phase1Save.Tests.csproj`; all 267 baseline Core tests, all 48 baseline Save tests, and all new Step 1 tests pass; all 9 frozen anchors unchanged; protected listeners `:33767`, `:43649`, `:46593` remain listening.
- **P1-TH-REVIEW:** Independent reviewer verification and checklist sign-off (every row remains `[ ]` until independent review returns a verdict).

---

## 9. Stop conditions

Stop immediately and report (never work around) if:
1. Base HEAD (`5438c94c…`), branch, clean status, or `.p0ref/SitkaLab.Core.dll` hash (`9a99f781…34b3`) does not match.
2. Implementing Step 1 requires editing `src/Core/Phase1/`, `Phase1SaveFormat.SchemaVersion`, `src/Ui/`, `Assets/`, Core biology, frozen anchors, or `main`.
3. Any build or protected test/anchor fails for a reason unfixable within the write allowlist.
4. Branch push credentials fail.
