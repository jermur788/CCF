# Scenario One Review / Management History — Implementation Plan

- **Spec ID:** SPEC-scenario-one-review-history-v1-V1
- **Spec status:** APPROVED (Manager review `65d354a`; decisions R-1…R-4 and save-version sequencing applied per the amendment packet)
- **Plan owner:** Overall Manager; drafted by the Lane C primary specification author
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27 (approved)

## 1. Architecture / current-state findings

Inspected read-only on `spec/scenario-one-review-history-v1` (packet commit `8177cf6`, base `3b499aa`). Line numbers refer to that tree. **Verified** = read from source; **Inference** = reasoned, labelled.

### 1.1 The record layer (what history is already durable)

| Record | Where | Key content | Written |
|---|---|---|---|
| `ScenarioManagementEvent` (append-only log) | `ScenarioOne/ScenarioManagementEvent.cs:38-65`; list `ScenarioOneSaveData.managementEvents` (`ForestSaveData.cs:60`) | `eventId`, `year`, `eventType`, `outcome`, `ecologicalTreatment`, `workOrderId`, `taskType`, `speciesId`, `targetTreeId`, `cellIndex`, `worldPosition`, `stockItemId`, quantity/stock, `estimatedContractorCostCents`, `contractorCostCents`, `timberRevenueCents`, `biologicalVolumeM3`, `regenerationDensityRemoved`, `cashDeltaCents`, `closingCashCents`, `failureReason`, `executionMethod`, `ownerMinutes` | one per creation/approval/cancellation/resolution/stock purchase/year advance/scenario outcome/century review (`ScenarioOneManager.cs` `RecordEvent`/`RecordOrderEvent`, ~2355-2385) |
| `ScenarioOneWorkOrder` (orders persist forever) | `ScenarioOne/ScenarioOneWorkOrder.cs:29-56`; list `ForestSaveData.cs:57` | full lifecycle fields incl. `createdYear`, `resolvedYear`, `validationMessage`, `fellingOutcome`, `executionMethod`, `harvestJobId` | created/approved at plan time; resolved at `AdvanceYear`; completed/failed orders are never pruned |
| `ScenarioAnnualReport` | `ForestSaveData.cs:90-108` | per-year `completedTasks`, `failedTasks`, `contractorCostCents`, `timberRevenueCents`, `harvestedVolumeM3`, regeneration-removal totals, `deadwoodCreated(M3)`, `deadwoodDecayedM3`, `keptForUseVolumeM3`, `closingCashCents`, `harvestMinimumAdjustmentCents`, `ownerMinutes`, `timberSales[]` | one per `AdvanceYear` (`ScenarioOneManager.cs:1272-1274`) |
| `ScenarioEcologicalSnapshot` | `ScenarioOne/ScenarioEcologicalSnapshot.cs:20-44` | year-tagged stand state incl. `meanCanopy`, `occupiedRegenerationCells`, `deadwoodVolumeM3`, per-species `ScenarioSpeciesOutcome` | one per year (`RecordEcologicalSnapshot`, `ScenarioOneManager.cs:2482-2600`) |
| `StormEventRecord` | `StormModel.cs:15-25`; list `ForestSaveData.cs:79` | `year`, `severity`, `directionDegrees`, `cropTreesLost` (exactly D-050's shape) | per resolved storm (`ScenarioOneManager.Storms.cs:45-52`) |
| `ScenarioDeadwoodRecord` | `ScenarioOne/ScenarioDeadwoodRecord.cs:9-23` | `deadwoodId`, `treeId`, volumes, `fallenYear`, `lastDecayYear` | biological/storm death and felling-retained deadwood |
| Per-tree history fields | `ForestSaveData.cs:119-146` (`TreeSaveData`) | `pruningLifts`, `crownBaseHeightM`, `lastPruningYear`, `mortalityCause`, `mortalityYear`, `equivalentSuppressedYears`, `markType` (current mark only) | as events occur |
| Scenario outcome | `ForestSaveData.cs:65-68` | `outcome`, `outcomeYear`, `outcomeReason`, `annualReviewSeen` | `SetOutcome` (`ScenarioOneManager.cs:2332-2346`) |
| `ScenarioCenturyReview` | `ScenarioOneObjectives.cs:24-34` | `year`, `outcome`, `completedYear`, `referenceId`, `referenceComparisons`, `managementComparisons` | built once at review year (`ScenarioOneManager.cs:2312-2329`) |

Sequence counters `nextWorkOrderId`, `nextManagementEventId`, `nextDeadwoodId` persist (`ForestSaveData.cs:55-56, 64`) and are defensively recomputed on load (`ScenarioOneManager.cs:1420-1427`). Save schema version is **20** (`ForestSaveData.cs:7`).

### 1.2 Work-order lifecycle (verified)

`ScenarioWorkStatus = { Pending, Approved, Completed, Failed }` (`ScenarioOneWorkOrder.cs:13-19`). There is no `Executed`/`Cancelled`/`Invalid` status:

- **Cancelled** orders are *deleted* from `workOrders` (`RemovePendingOrder` / `CancelApprovedOrder`, `ScenarioOneManager.cs:1107-1133`) and leave an `OrderCancelled` event carrying the order's identity fields. History of a cancellation therefore lives only in the event log.
- **Invalid open orders** carry `validationMessage` (`ValidateOpenOrders`); resolution failure sets `Failed` + `validationMessage` + `resolvedYear` (`Fail`, `ScenarioOneManager.cs:2028-2034`).
- Resolution mutates the order: `status`, `resolvedYear`, and overwrites `expectedVolumeM3` with the **actual** stem volume (`ScenarioOneManager.cs:1547-1549`); pruning zeroes it. Income/cost are **not** written per order — money lives on the paired `WorkResolved` event and the annual report.

### 1.3 Known precision limits (must be surfaced honestly)

1. **Grouped harvest settlement** (`SettleHarvestJob`, `ScenarioOneManager.cs:1290-1305`): one settlement per resolution-year job; it overwrites `contractorCostCents`/`timberRevenueCents`/`cashDeltaCents`/`closingCashCents` on the **last** felling event's `WorkResolved` record. Per-order money inside a grouped job is not authoritative. `harvestJobId = CurrentYear + 1` (`ScenarioOneManager.cs:1469`) is a resolution-year label, not a unique sequence.
2. **Storm review is reconstructed**: `StormDamageSummary` counts victims from live `windVictims` (`tree.MortalityYear == year`), `deadwoodRecords` and `workOrders` (`ScenarioOneManager.Storms.cs:22-43`); its own comment says "Reconstructed review data, never another saved history or victim list".
3. **Session-only facts**: `PlantedJuvenile.lastYearBrowsed` / `lastBrowseAssessmentYear` (`[NonSerialized]`, `PlantedJuvenile.cs:25-27`), `ForestEcologyController.annualDbhGrowth` ("DBH growth last year", runtime dictionary), cohort browse assessment fields. Shown in `TreeInspectionView` / `ScenarioEcologyReviewLines` / `RegenerationDiagnosis`; they read "n/a" after load.
4. **Mark history absent**: `markType` is current state only (`ForestSaveData.cs:140`); no event timestamps mark application/removal. Fell marks are consumed into orders (`ScenarioOneManager.cs:1098`); Crop designations persist as current state.
5. **Objective results are current-state only**: `ScenarioObjectiveResult` has no achieved year; `ScenarioOneObjectives.Evaluate` (`ScenarioOneObjectives.cs:49-97`) re-derives from a snapshot + events (+ optional orders) on every access (`ScenarioOneManager.cs:292-295`). Only scenario-level `outcomeYear` is recorded.
6. **Trends are the only multi-year view**: `AnnualReviewView.BuildTrends` (`:185-198`) shows the last 12 `closingCashCents` and `meanCanopy` values.

### 1.4 Review surfaces today (verified)

- `UI/AnnualReviewView.cs` — screen 5 "annual review". `Refresh` (`:45-74`) renders **only** `AnnualReports[Count-1]` (`:55`). Sections: Work done (`BuildWork`, re-sums that year's `WorkResolved` events), Money (`BuildMoney`, report fields + event re-sums), Forest (`BuildForest`, snapshot delta via `ScenarioEcologyReviewLines`, live ecology, retained timber), Storm (`BuildStorm`, reconstructed), Trends (12-year strips), Objectives (derived), Century (`BuildCentury`, iterates `referenceComparisons` only — `managementComparisons` are computed and saved but never rendered). Acknowledgement gate `annualReviewSeen` blocks the next advance (`ScenarioOneUiRoot.NeedsAnnualReview`).
- `UI/WorkPlanView.cs` — forward-looking current-year plan (open orders only, `IsOpen` filters at `:120, 193, 246, 270`) plus the frozen Reference Future card (`BuildReference`, `:326-333`).
- `UI/WorkPlanOverview.cs` / `CashOutlook` / `ResidualStand` — estimates before resolution; points to Annual Review for actuals.
- `UI/TreeInspectionView.cs` — per-tree current state; origin label derived from ID strings (`RegenerationDiagnostics.TreeOriginLabel`); pruning lifts from saved tree fields; DBH growth last year session-only.
- `UI/LearningObjectivesView.cs` — lesson progress in `PlayerPrefs` (`CCF.Learning.v1.*`), device-wide, deliberately not in the forest save.
- `ScenarioReferenceRunner` / `ScenarioReferenceArchive` — the frozen Reference Future v1 archive (milestone worlds with their own history) and its authoring/verification driver; player-facing access is read-only milestone preview (`TryBeginReferencePreview`, `ScenarioOneManager.cs:608-659`) plus the Century Review comparison semantics (frozen reference vs aspirational design targets, `ScenarioOneObjectives.cs:132-152`).
- **No browsable multi-year history/timeline exists.** (`managementEvents` is a complete log but is only consumed filtered.)

### 1.5 Save/identity facts (verified)

Full schema map: `ForestSaveData.cs` (root + `ScenarioOneSaveData` + nested records). Identity keys and their generators are tabled in `spec.md` §9. Cell indices are geometry-relative (`StandGeometryModel.cs:44-56`); `standGeometryModel` (v20) decides how a save's cells are interpreted. Reference Future v1 is a frozen Legacy40 archive (`referenceId = "reference-future-v1"`, `definitionVersion = "scenario-one-v12"`, `saveVersion = 12`) whose integrity is checked against **original embedded JSON** hashes (`ScenarioReferenceArchive.Load`, `:43-106`) — never re-serialize it.

### 1.6 Retrospective derivation option (verified feasibility)

`ScenarioOneObjectives.Evaluate` is a pure function of `(definition, snapshot, events, originalSpeciesId, orders, geometry)`. Because `ecologicalSnapshots` and `managementEvents` are year-tagged and resolved orders carry `resolvedYear`, an approximate "first year achieved" can be derived for old saves by evaluating per snapshot year with events/orders filtered to `year <= Y`. **Inference:** this is faithful for objectives driven by snapshot values and past events (`managed-opening`, `introduced-<species>`), and approximate for anything that depends on *open-order* state at that time (open orders are current state). Label derived values; never present them as recorded.

## 2. Proposed technical approach

**Extend the existing Annual Review into a year-browsable review/history surface**, backed by a small read-only history query layer over the existing saved records. No second history store.

1. **History query layer (read-only).** A focused helper (e.g. `ScenarioManagementHistory`) that, given `ScenarioOneManager` state, returns per-year views: the year's `ScenarioAnnualReport`, its `WorkResolved`/`Order*` events, resolved orders by `resolvedYear`, snapshot deltas vs the previous snapshot, storm records for the year, and objective-achievement entries (recorded or labelled-derived). It must not mutate anything and must classify each returned item's layer (L1/L3).
2. **Year navigation in the existing review surface.** `AnnualReviewView` gains year browsing (previous/next year, year list) while keeping the current latest-year-first flow and the acknowledgement gate semantics. Latest-year behaviour is preserved; earlier years show the same card structure built from that year's records.
3. **Intervention detail (drill-down).** From a year's Work card, an intervention row expands to its recorded targets (tree IDs/cell/position), execution method, outcome/failure reason, and money shown per FR-9 (grouped settlement attributed to the job).
4. **Bounded L4 guidance card.** A short "What this means for your next decision" view, generated from recorded treatment history + current observation templates. Waypoint reuse (existing mechanism used by the storm card) where a recorded location exists.
5. **Objective achievement recording (additive schema per spec §11).** In `EvaluateProgress`, append `{objectiveId, achievedYear, targetValueAtAchievement}` on first achievement. Old saves follow R-1: derive a labelled year only where exact first-achievement is recomputable from year-tagged L1 records plus accepted objective logic; otherwise "not recorded".
6. **Labelling pass.** Reconstructed (storm), derived (deltas/trends/retrospective objectives), and session-only (browse/DBH growth) content gets explicit labels; missing provenance renders "not recorded".

**Rejected alternatives (trade-offs noted):**

- *A separate Management History system/panel with its own store* — duplicates the record layer and risks a second authority; rejected by the spec's "extend existing surfaces" constraint.
- *Persisting a full per-year Work Plan snapshot list* — plan evolution is already reconstructible from OrderCreated/Approved/Cancelled events; a snapshot list would duplicate history and grow saves.
- *Map history overlay / new cartography* — frozen professional map; spatial question answered by waypoints + walking (core loop); deferred as a possible later extension.
- *Reconstructing objective dates silently for all saves* — fabricates provenance; rejected in favour of recorded-at-the-time + R-1's exact-or-nothing labelled derivation.

## 3. Integration boundaries

**May touch:**

- `Assets/ForestPrototype/UI/AnnualReviewView.cs` (year browsing, drill-down, labels) and, if needed, `UI/ScenarioOneUiRoot.cs` (review navigation/acknowledgement wiring only).
- New read-only query helper under `Assets/ForestPrototype/ScenarioOne/`.
- `ScenarioOneManager.EvaluateProgress` call site for the additive objective-achievement append (one bounded write, spec §11).
- `ForestSaveData.ScenarioOneSaveData` additive field + clone round-trip (`CaptureSaveData`/`RestoreSaveData`) + a save-version bump to the **next available save-schema version on the integration base** (Rack V2 reserves v21; S1-C expected v22 if Rack V2 integrates first — never reuse an owned version) + old-save defaulting. This change is **approved** (FR-10) and gated by save/reference review (G2/G3/G4).

**Must not touch:**

- Ecology/economy rules (`ForestEcologyController`, `SitkaGrowthModel`, `RegenerationModel`, `StormModel`, `UnderstoreyCompetition`, `WorkEconomy/*` calculators, price books).
- Objective definitions/targets (`ScenarioOneDefinition` values), completion semantics beyond reading them.
- `ScenarioReferenceArchive` content, hashes, load verification, or Reference preview behaviour.
- Save loader validation order and destructive paths (`ForestSaveController` validation block) except additive field cloning/defaults.
- Existing record shapes (`ScenarioManagementEvent`, `ScenarioAnnualReport`, `ScenarioOneWorkOrder`, snapshots, storm/deadwood records).
- The approved professional map surfaces (`UI/Map/*`, `StandMapView.cs` cartography) beyond reusing the existing waypoint mechanism.
- Learning-progress storage (`PlayerPrefs`) semantics.

**Interfaces used (read-only):** `ScenarioOneManager` lists (`ManagementEvents`, `WorkOrders`, `AnnualReports`, `EcologicalSnapshots`, storm/deadwood accessors), `ScenarioOneObjectives.Evaluate`, `ScenarioEcologyReviewLines`, `ScenarioOneUiFacts`, existing UiKit/`ScenarioOneUi.uss` patterns.

## 4. Affected systems / files

- `Assets/ForestPrototype/UI/AnnualReviewView.cs` (primary)
- `Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs` (new, read-only)
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` (one additive record append in `EvaluateProgress`; clone helpers)
- `Assets/ForestPrototype/ForestSaveData.cs` (additive field; version bump — only under approved FR-10 path)
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` (navigation wiring only, if needed)
- Verification harnesses under `Tools/Verification/` (later tasks; disposable workflow)

## 5. Ownership / concurrency considerations

- Annual Review / Work Plan / shared UI files are historically high-conflict across lanes (see `Docs/Project/coordination/active-tasks.md` UI rows). S1-C implementation needs a single declared writer for `AnnualReviewView.cs` and coordinated access to `ScenarioOneUiRoot.cs` / `ScenarioOneUi.uss`.
- The additive `EvaluateProgress`/save-clone change overlaps the protected save surface: one writer, G2/G3/G4 gates per AgentWorkflow (save + measured evidence).
- Rack V2 (D-064) and Multi-Scale (D-065) lanes may be active on other surfaces; S1-C must not depend on their unmerged state and must not edit their files.
- No Unity Editor use is required to write this spec; later implementation tasks declare Unity lanes/locks per packet.

## 6. Migration / save implications

- Core history: **no migration**; all content already persists at v20.
- FR-10 additive record (approved): new field on `ScenarioOneSaveData`; missing/empty = not recorded; no retro-writing. **Save version: no fixed number.** Immediately before implementing the persistence change, inspect the actual integration base and use the next available save-schema version; never reuse a version already owned by an accepted/qualified concurrent feature. **Rack Planning & Field Marking V2 currently reserves/implements v21**, so if Rack V2 integrates first, S1-C is expected to use **v22**. v20 and older loads unchanged; Reference Future v1 untouched (frozen `saveVersion` 12 archive; D-042). Deterministic anchors that key off save bytes/schema may need re-labeling only when the field is adopted — flag at implementation time (G3), do not silently re-baseline.
- Old-save history display (R-1): exact labelled derivation only where first-achievement is recomputable from year-tagged L1 records + accepted objective logic; otherwise "not recorded". Never synthesized or approximate-as-fact.

## 7. Verification strategy

This task is documentation-only. For the future implementation (planning now):

- **Implementation correctness:** Tier 1–2. Offline harness over the history query layer (per-year views over a fixture save; grouped-harvest attribution checks; objective-achievement append + clone round-trip); Tier 2 Unity feature qualification: multi-year session, rendered review at 1280x720 and 1920x1080, genuine old-save (v20 pre-feature) load, save/load round-trip stability.
- **Spec conformance:** checklist `specs/scenario-one-review-history-v1/checklists/requirements.md` run against the implementation; every FR/AC needs evidence; layer-labelling and "no fabricated past" cases are mandatory.
- **Verification tier:** Tier 2 for the feature; escalate to Tier 3 only with a failed gate, new concrete risk, protected save/reference threat, or Manager approval (AgentWorkflow).
- Protected gates re-run where save surface changes: save compatibility, Reference archive integrity, completion hash **label** review (any anchor movement is an explicit decision, never silent).

## 8. Risks

- **UI ownership conflicts** on `AnnualReviewView` / `ScenarioOneUiRoot` with other active lanes.
- **Grouped-harvest attribution** mis-displayed as per-order fact → player misled; mitigated by FR-9 and AC-7.
- **Derived objective dates** mistaken for recorded facts → provenance erosion; mitigated by R-1's exact-or-nothing rule, mandatory labelling and AC-4.
- **Save-version collision** with a concurrent schema owner (Rack V2 v21) → mitigated by the next-available-version rule and the pre-implementation integration-base check in T-05.
- **Scope creep toward analytics** — trend strips are the explicit bound (FR-15, AC-11).
- **Save-surface risk** from the additive record: mitigated by keeping it the only schema change, append-only, with old-save defaults and G2/G3/G4 gates.
- **Causal over-claiming in L4 copy** → mitigated by FR-6 wording rules and review of the guidance card.

## 9. Implementation sequence

1. **T-01** History query layer (read-only per-year views; layer classification; FR-1–FR-4 data plumbing).
2. **T-02** Year browsing in Annual Review + preservation of latest-year flow and acknowledgement gate (FR-1, AC-2).
3. **T-03** Intervention drill-down + money-attribution honesty (FR-2, FR-8, FR-9; AC-1, AC-7).
4. **T-04** Labelling pass: reconstructed/derived/session-only/"not recorded" (FR-4, FR-11, FR-13; AC-3).
5. **T-05** Objective-achievement recording + history display (FR-10, FR-12; additive schema per §6 with the next-available save version; AC-6).
6. **T-06** Bounded L4 next-decision guidance + waypoint reuse (FR-7, FR-6; AC-5).
7. **T-07** S1-D contract doc + identity/retention stability checks (FR-14; AC-10).
8. **T-08** Feature qualification: harnesses, old-save load, save round-trip, two-resolution rendered review, protected gates (AC-1–AC-11), then spec-conformance checklist.

Each task maps to `tasks.md`. Implementation must not start before the spec is `APPROVED` (AgentWorkflow: `DRAFT` confers no authority).
