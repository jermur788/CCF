# Scenario One Second and Subsequent Interventions (S1-D) Specification

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Status:** APPROVED — Overall Manager decisions recorded in `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/MANAGER-REVIEW.md` (review commit `457461b9b92da2e96101f5b0eb9940b4e92fec0a`), applied by `AMENDMENT-PACKET.md` (`771620113487894bd8e09c69309318cc603ae063`). Was `DRAFT` at `666cb67cfc3e17fc22df5607ac4043c8fbd7ee7d`.
- **Implementation authority:** task-level, within scope and within each task's stated dependencies only (`tasks.md`). **No production task may be dispatched until T-01 has passed** on the integrated implementation base (§17).
- **Owner:** Overall Manager (ChatGPT Game Dev)
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27
- **Decision log references:** D-004, D-008, D-009, D-010 (amended in part by D-065), D-013, D-014, D-042, D-056, D-060, D-063, D-064, D-065; resolved decisions RD-1…RD-12 (§12)
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness; this is the **S1-D** pre-demo item (D-060)
- **Related evidence:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/HANDOFF.md`, `MANAGER-REVIEW.md`; S1-C spec `specs/scenario-one-review-history-v1/` (branch `spec/scenario-one-review-history-v1`); `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`; `specs/multiscale-forestry-management-v1/`
- **Supersedes / superseded by:** none
- **Packet:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/TASK-PACKET.md` — base `3b499aa459b92212f7bb06897836bae5914b5230`, context `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`
- **Task class:** SPEC REQUIRED. This package is specification only: no production code, Unity edit, coefficient or Windows build.

## How to read this spec

Every requirement carries an **Authority** tag:

- `[D-nnn]`, `[Milestone]`, `[Brief]` — settled by accepted project authority;
- `[RD-n]` — settled by an Overall Manager decision recorded in `MANAGER-REVIEW.md` and §12;
- `[CODE c6342f7]` — an inspected implementation fact (see §3 for its limits);
- `[SPEC]` — a requirement introduced by this spec and approved with it; no earlier accepted authority covers it.

No product decision remains open in this spec. Where a finding could be mistaken for an open choice it is recorded as a stop condition (§17), not as an assumption.

## 1. Product intent

The player must be able to repeat

`assess → decide/mark/plan → Work Plan → execute → advance time → observe/review → decide again`

without resetting the scenario, fabricating a new forest, or treating the first intervention as a one-shot tutorial that blocks later management. This is the accepted loop `decide → advance time → walk → observe → understand why → decide again` [D-004] applied repeatedly, and it is a **pre-demo requirement** because the first external demo contains the complete Scenario One, in which the player makes "second and later interventions", repeats the management cycle and reaches completion, and "by scenario completion … has learned the basic CCF management loop through repeated intervention and adaptation" [D-060, Milestone completion checks].

## 2. Problem

Accepted authority states the requirement (D-060) and that S1-D is "not implemented" (Milestone, context `eb218ea`). It did not define when a further cycle becomes available, what "ready for next intervention" means, how earlier work is told apart from new decisions, or how repeated cycles interact with the lesson sequence, completion and saves. Inspection (§4) shows that on the current Scenario One code lineage the underlying management actions already repeat, but a small number of first-cycle assumptions can still block, mislead or regress the player on re-entry. This spec fixes the requirement, records the verified assumptions, records the Manager's decisions (§12) and bounds the work.

## 3. Inspection basis — read this before relying on any finding

**Finding L-1 (lineage).** The packet base `3b499aa` (`main`) does **not** contain the lesson curriculum these requirements concern. `main` has save `CurrentVersion = 20`, no `rack.approve`/`rack.result` steps, and none of the history, completion-chronology or sequential-lesson code described below. That code lives on the Scenario One integration lineage that is not an ancestor of `main` (merge-base `c1564b8`). The current tip of that lineage, and the registered BASE of the active Rack V2 lanes in `active-tasks.md`, is `c6342f79a3e89c040fadd232017a937132779385` (save `CurrentVersion = 21`; RACK-V2-03B reports a Unity gate chain PASS at this commit).

Therefore:

- **`[CODE c6342f7]`** means: read statically from the Git object at `c6342f7`, **not run in Unity** by the spec author, and **not integrated into `main`**. File and symbol names are given rather than line numbers because the Rack V2 lanes are changing lesson-sequence code.
- **Implementation facts must be re-verified at the actual integrated implementation base before any production task is dispatched (T-01, mandatory).**
- The canonical Unity Project Overview records none of this code.

**Finding L-2 (partial implementation baseline — decided, RD-1).** The candidate lineage already holds code and documents that anticipate S1-C and S1-D: derived management history (`ScenarioInterventionHistory`, `ScenarioOneManager.History.cs`, `ManagementHistoryText`; `7a023eb`); a reviewed two-cycle completion rule in save v20 (`ScenarioOneCompletionRules`, `7840872`) whose normalised source is hash-pinned in `Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py`; completion guidance (`b87b52e`); the derived eight-lesson `ScenarioOneTeachingSequence` (`420fcb6`); `Docs/Design/ScenarioOneSequentialObjectivesV1.md`; and `Docs/Verification/ThreeSpeciesProgramme/Phase7to9-HistoryAndRepeatedInterventions.md` (explicitly *CLOUD / NOT UNITY-VERIFIED*). Per **RD-1** these four items are the **partial implementation baseline** this spec extends, subject to T-01. They are **not** evidence that S1-C or S1-D are complete: the Milestone's wording that both remain pre-demo work stays correct until their approved specs are implemented, integrated and feature-qualified. The design note's statement "No new S1-C/S1-D development needed" is **not accepted as product authority**.

**Finding L-3 (S1-C contract — reconciled in §9).** The S1-C specification now exists (`9b96f57`), with a Manager review (`65d354a`, "accepted in substance; narrow amendment only", the amendment concerning save-version sequencing) and an amendment packet (`b7193a5`). At the time this amendment was written the S1-C package was not yet `APPROVED`. S1-D consumes the S1-C contract as described in §9 and defers to S1-C's final approved text on any difference. Note that S1-C inspected `main` (`3b499aa`) and does not mention the candidate lineage's derived-history code; reconciling the two is a T-01 duty (§9.4).

## 4. Verified first-cycle assumption register

Class key: **B** can block Approve/Advance or a management action; **R** can regress or mislead progression state; **G** design gap (nothing existed to rely on); **I** informational dependency. The last column gives the treatment decided in §12.

| ID | Cls | Finding `[CODE c6342f7]` | Why it matters for re-entry → treatment |
|---|---|---|---|
| F-1 | **B** | `LearningObjectivesView.RackPlanProblem()` returns a non-empty problem when `Facts.Bay != null && AccessOpen && !Bay.CanopyKept`. `ScenarioOneUiRoot.ApproveWorkPlan()` and the advance path return false while it is non-empty, and `WorkPlanView` disables Approve and Advance. It has **no lesson-stage condition**. `Bay.RemovedBasalShare` is **cumulative over the whole game**: all completed felling in the teaching footprint (served by a Crop Tree or not), plus planned and marked trees, over everything the footprint held (`ScenarioOneRackBay.Analyse`). Constant `MaxRemovedBasalShare = 0.40`. | A teaching safeguard ("interactive planning safeguard only") works as a lifetime Approve/Advance ceiling on the middle-rack footprint. → **RD-5**: hard only during its owning teaching activity; advisory afterwards (FR-A3, T-03). |
| F-2 | R | `ScenarioOneRackBay.Analysis.HasEnoughFuture` requires **exactly** `FutureTreesWanted = 3` valid, non-too-close Crop Trees spread across near/middle/far zones and both sides. `ScenarioOneTeachingSequence.Current` returns lesson 2 when `Bay != null && !HasEnoughFuture`. Separately, `ReleaseExecuted` is recomputed from *current* shares (`MinExecuted >= 0.30 × ExecutedTolerance 0.9`); the code comment accepts only "modest growth" as neighbours outgrow stumps ("not an unlimited completion guarantee"). | A fourth Crop Tree in the footprint, or the loss of one, returns the guide to "Choose promising trees" after the player has finished it; over many years the executed-release measure can drift below threshold and pull the guide back to lesson 3 or 4. Pedagogical only. → **RD-7** (FR-B1, T-04). |
| F-3 | R | `Analysis.ReleaseYear` is the latest year of an in-footprint removal that serves a Crop Tree; `ReleaseReviewed = Reviewed(ReleaseYear)`. `ReleaseKey` is built from **every** in-footprint completed felling, and `ScenarioOneRackReview.Synchronize` resets the session review whenever it changes. `Current` returns lesson 5 while `!ReleaseReviewed` or `!RackReviewComplete`. | Any later in-footprint felling re-opens "Return and review" (by a new `ReleaseYear` when it serves a Crop Tree, otherwise by the session reset). → **RD-7**. |
| F-4 | R | `ScenarioOneRackReview` (the `[J]`/`[K]` decisions, inspected trees, six stand portions) is **session-only**; a `TeachingWorldVersion` change resets it; `Current` requires `RackReviewComplete` when a bay applies. Lesson-5 copy: "After loading, repeat this inspection review; it is not stored in the forest save." | After every load the guide returns to lesson 5. → **RD-7**: do not persist `[J]/[K]` for S1-D; latch from durable history once the lesson has historically completed. |
| F-5 | G | `ScenarioOneUiRoot.NeedsAnnualReview = AnnualReports.Count > 0 && !AnnualReviewSeen`; `annualReviewSeen` is a sticky bool set by `MarkAnnualReviewSeen`. It gates the **first** advance only. Later `AnnualReviewAcknowledged` events can be recorded after each resolution (`ShouldRecordAcknowledgement`) but nothing forces them; only the completion chronology and teaching indices 1, 4 and 6 read them. | → **RD-3**: later acknowledgement is advisory, never a hard gate (FR-D3). |
| F-6 | G | "Readiness" exists only as text: `ScenarioInterventionHistory.NextStepHint`, `CompletionGuidanceState`, `StaleOrders`, `AdvanceGuard`. | → **RD-4**: advisory derived component/reason state (FR-D1/D2, T-02). |
| F-7 | G | `ForestTree.markType` is a single enum (None/Fell/CropTree) with no year or provenance. Fell marks are **consumed** on import (`AddMarkedTreesToWorkPlan` → `marking.ClearAll()`); Crop Tree marks persist indefinitely. Work orders do carry `createdYear`, `resolvedYear`, `status`, `rackId`. The Stand Map layers are Access/Light/Regeneration/Browse/Marks; stumps are drawn as a state, with no dated past-work layer. | → **RD-6**: derived from existing orders/events/history only; no designation-year field, no dated map layer (FR-C). |
| F-8 | R | Learning practice steps are stored in device-wide `PlayerPrefs`: `LearningObjectivesView.Record(id)` writes `PreferencePrefix + id` (`"CCF.Learning.v1."`) and `ReloadProgress` rebuilds `done` from them. They are not in the save. `ScenarioOneManager.TutorialHint` embeds `Learning.Summary`. The sequential guide itself is derived from the save (design doc). | A second save or new game on the same device inherits practice credits. → **RD-7**: not progression authority after historical lesson completion (FR-B3). |
| F-9 | I | `ScenarioOneCompletionRules.HasReviewedTwoCycleSequence`: two **successful** resolved plans (any task type) in distinct years with an `AnnualReviewAcknowledged` event between them by event-ID order. `EvaluateProgress` sets Completed only when all eight objectives are met **and** this holds. Existential, therefore monotone. `Plans()` returns an empty list if any event ID is `<= 0` or duplicated. The management-event validators in `ForestSaveValidation` check only cell index, owner minutes and execution method; none checks event IDs. | Repeated cycles can only help the rule; a save with ambiguous IDs can load and never complete. The normalised source is hash-pinned by N3. → **RD-8** (rule unchanged) and **RD-12** (reject ambiguous IDs before mutation; FR-E3, T-06). |
| F-10 | I | `GetHarvestQuote` counts distinct years with a successful `FellTree` event and passes it as `previousInterventions`; `ScenarioOneEconomyAdapter` maps 0/1/≥2 to `FirstThinning`/`SecondThinning`/`LaterThinning` cost schedules (`Stage1EconomyDefaults`). The history ordinal is deliberately separate. | The economy is already cycle-count-aware and is not changed. |
| F-11 | I | `CanManage()` is true while `outcome != Failed && CurrentYear < ReviewYear` (`ReviewYear = max(MinimumCompletionYear, CenturyReviewYear)`, 100 by default). **Completed does not stop management.** Failed is terminal and is set by: no cash exactly `0` with no approved order; or year `>= ReviewYear` with objectives unmet. | → **RD-8**: continued management after Completed is accepted; Completed is an achievement, not a lockout. |
| F-12 | I | Rack V2 persistence exists at `c6342f7` (`rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, `racks`, `ScenarioOneWorkOrder.rackId`). The rack objective is lesson 1; reopening a confirmed plan after work has progressed is an open item in the Rack V2 spec (§39.7), not an S1-D decision. | → **RD-9**: rack reopening and additional-rack authoring belong to Rack V2; S1-D only requires that Objective 1 rack teaching not auto-repeat. |

### 4.1 Verified: already repeatable (no first-cycle blocker found)

| ID | `[CODE c6342f7]` |
|---|---|
| R-1 | Marking and Work Plan: `ForestTreeMarkingManager` and `ScenarioOneManager` have no reference to lesson/teaching state (only `TutorialHint` text does). Fell mark → `AddMarkedTreesToWorkPlan` → pending order (IDs from `nextWorkOrderId`) → `ApprovePendingWork` → `AdvanceYear`; Crop Tree designation persists. |
| R-2 | Planting (`TryPurchaseStock`, `TryDesignateExactPlanting`, shelters), vegetation clearance, regeneration removal, pruning (`TryDesignatePruning`, `BatchPruneCropTrees`), salvage create orders with no cycle-count assumption. (Pruning lift limits are stated in lesson text; not separately verified in code.) |
| R-3 | Open orders persist across years; `RevalidateOpenOrders` rechecks them against today's forest; `StaleOrders` surfaces orders with a validation problem or `>= StaleAfterYears (2)` years old. Nothing is auto-expired. |
| R-4 | `ScenarioInterventionHistory.Build` treats every resolved plan alike (no two-cycle special case); idle years add no intervention. Pure function of persisted records, hence identical after save/load. |
| R-5 | `AdvanceGuard` warns (never blocks) when planned work exists and none is approved; "Advance anyway" is always offered. |

## 5. Definitions

- **Management cycle** `[SPEC]`: one pass of the §1 loop. A cycle may legitimately end with **no** intervention (reassessing and deciding not to cut is valid; "retaining trees without another cut is success" is existing Lesson 5 copy).
- **Intervention (cycle level)** `[CODE c6342f7]`: a *resolved work plan* — a year in which at least one commissioned task succeeded. A presentation ordinal, deliberately separate from the economy's thinning tiers (F-10). Retained. S1-D uses the grouping unit the approved S1-C contract exposes where it differs (§9.4); order-level records are the drill-down beneath it.
- **Previous work** `[SPEC]`: Executed (resolved, with year and outcome, including failed tasks and reasons).
- **Current commitment** `[SPEC]`: Approved orders awaiting the next advance.
- **Proposed work** `[SPEC]`: Pending orders (current and carried over from earlier years) and Fell marks not yet in the Work Plan. Crop Tree designations are a standing designation (D-013/D-014), not a work proposal.
- **Re-entry** `[SPEC]`: beginning any cycle after the first, in the same save, including after a save/load and after any number of idle years.
- **Readiness** `[RD-4]`: an advisory derived state reporting components and reasons (FR-D1). Never a gate, never a timetable.

## 6. Goals

1. A player who has completed the first intervention can re-enter the cycle, in the same forest, until the scenario's management horizon, with no reset and no lesson-state blocker.
2. The player can always tell previous work from current proposals, and can see what earlier treatment bears on the next decision (via the S1-C contract, §9).
3. Completion remains reachable, and understandable, after any number of repeated cycles.
4. The save is the sole authority on where the player is; nothing is fabricated for old saves; S1-D V1 adds no persisted field.
5. Management after the first guided sequence stays player-directed, with advisory guidance only.

## 7. Non-goals

- Production code, Unity or scene/prefab edits, Windows build.
- New ecology, economy, objective-target or calibration coefficients; changing the eight objective predicates or D-057 effective targets.
- A fixed intervention interval, a minimum observation period, or a "correct" second treatment [Brief; RD-4].
- A new prescriptive objective for every cycle [RD-2].
- **Any new persisted field**, including a Crop Tree designation year and any per-save progress record [RD-6, RD-7].
- A dated past-work map layer [RD-6].
- Persisting the `[J]/[K]` review decisions for S1-D [RD-7].
- A second history store, a competing identity scheme, or duplicating S1-C's history UI [RD-1, §9].
- Rack reopening or additional-rack authoring (Rack V2), a second Multi-Scale prescription (MFM) [RD-9, RD-11].
- Multi-Scale Forestry Management, Rack V2, three-species parity, or the S1-C history UI itself.
- Scenario Two, sandbox, property-level multi-stand play.

## 8. Player workflows

### 8.1 Repeat cycle (target flow)

| Step | Player does | Exists today `[CODE c6342f7]`? | S1-D concern |
|---|---|---|---|
| 1. Assess | Walk, inspect (E), map layers, read history | Yes | Previous-versus-new clarity (FR-C) |
| 2. Decide/mark/plan | Fell (X) / Crop (C) marks, nursery, planting, pruning, clearance | Yes (R-1, R-2) | Not lesson-gated (FR-A2) |
| 3. Work Plan | Import marks, review cost and residual stand, choose executor and material, approve | Yes | F-1 must not block it after the teaching activity (FR-A3) |
| 4. Execute and advance | Advance one year; approved work resolves | Yes | Idle advance remains legitimate |
| 5. Observe/review | Annual Review, history, walk the changed forest | Yes (derived history) | Review is advisory after the first (FR-D3) |
| 6. Decide again | Return to step 1 | Partly — guide can regress (F-2..F-4) | FR-B1 |

### 8.2 Entry conditions for the next cycle

The next cycle is available as soon as the advance completes, for as long as the scenario is not Failed and the year is below the review year (F-11). **Nothing further gates it:** the one-time first-advance acknowledgement is unchanged, but no later acknowledgement, observation interval or cleared order is required [RD-3, RD-4].

## 9. Interface to S1-C (Review / Management History) — consumed contract

S1-D **consumes** the S1-C history capability; it does not define, store or present a competing one [RD-1; S1-C FR-14, §9].

### 9.1 Status of the contract

S1-C draft `9b96f57`; Manager review `65d354a` ("accepted in substance", narrow amendment: save-version sequencing); amendment packet `b7193a5`. Not yet `APPROVED` when this amendment was written. S1-D relies only on the parts of the S1-C contract that the Manager review accepts, listed in §9.2. Where S1-C's final approved text differs, S1-C controls and T-01 records the difference.

### 9.2 Accepted S1-C boundaries S1-D relies on

1. **One read-only history capability** over existing authoritative records. No competing history store.
2. **Stable IDs and append-only / read-only history surfaces** are the compatibility contract.
3. **Four content layers stay distinguishable:** L1 recorded historical fact, L2 current observed state, L3 derived comparison, L4 explanatory teaching.
4. **Aggregate-first** intervention detail with **drill-down** to recorded targets; no default per-tree dumps.
5. **Session-only facts** (browse assessment, annual DBH growth) remain session-only and are labelled after load.
6. **Pre-feature objective-achievement dates** are shown only where exact reconstruction from persisted year-tagged records is evidence-complete, labelled as derived; otherwise `not recorded`.
7. **Full mark/retain decision provenance is not required**; the existing commitment/result trail is sufficient.

### 9.3 What S1-D consumes (EI-1…EI-7)

| Id | Consumed from the S1-C contract | Rule for S1-D |
|---|---|---|
| EI-1 | **Stable identity keys** (S1-C §9): tree IDs, work-order ID, management-event ID, planted-juvenile ID, deadwood ID, shelter ID, storm year; cell index only relative to the save's `standGeometryModel` | MUST use them; MUST NOT introduce another identity scheme for trees, orders, events, juveniles, deadwood, years |
| EI-2 | **Read surfaces:** `managementEvents`, `workOrders` (incl. `createdYear`/`resolvedYear`/status), `annualReports`, `ecologicalSnapshots`, `stormEvents`, `deadwoodRecords`, per-tree history fields, `outcome`/`outcomeYear`/`outcomeReason`, `centuryReview` | Read-only; MUST NOT rewrite, repurpose or append to them |
| EI-3 | **Year-browsable review and intervention record** (S1-C FR-1..3, FR-8), aggregate-first with drill-down | S1-D's previous-versus-proposed presentation MUST link to and reuse this surface; MUST NOT duplicate it |
| EI-4 | **Layer labelling** (L1–L4) | Cycle-state components are L3 (derived) and MUST be labelled so; any readiness wording is L4-bounded: no causal claim, no prescription |
| EI-5 | **Annual Review acknowledgement** is an existing L1 record (`AnnualReviewAcknowledged` management events) | S1-D derives "latest resolved intervention reviewed" read-only from it, via an S1-C accessor if one exists, otherwise inside the cycle-state adapter; MUST NOT create another store |
| EI-6 | **Objective-achievement history** (S1-C FR-10/12) where present; `not recorded` otherwise | S1-D has **no dependency** on it and writes nothing to it |
| EI-7 | **Next-step / completion guidance and completion summary** (candidate-lineage `NextStepHint`, `CompletionGuidance`, `CompletionReview`, or S1-C's equivalent) | S1-D consumes them; the cycle-state adapter does not replace them |

### 9.4 The cycle-state adapter and the T-01 reconciliation duty

S1-D MAY add one small, read-only, `UnityEngine`-free **cycle-state adapter** over §9.3. It MUST hold no persisted state, MUST NOT be a second history authority, and MUST NOT duplicate S1-C UI.

T-01 MUST establish, on the integrated base, and report as a finding (not decide):

- **(a) One derived-history implementation.** S1-C inspected `main` and does not mention the candidate lineage's `ScenarioInterventionHistory` / `ManagementHistoryText` / `NextStepHint`. Exactly one derived-history implementation may exist on the integrated base. If S1-C's implementation introduces a second, **STOP** and return to the Manager (§17).
- **(b) Grouping unit.** S1-C FR-2 describes intervention records at order level; the candidate lineage groups by resolved work plan. S1-D uses the unit the integrated S1-C surface exposes, with order-level records as drill-down.
- **(c) Schema ordering.** S1-C's additive objective-achievement record, if implemented, is S1-C's schema and sequencing. S1-D neither requires nor writes it, and claims no schema version.
- **(d) Tree-death cause.** S1-D asserts nothing about death cause beyond what S1-C exposes from per-tree `mortalityCause`/`mortalityYear` where recorded.

## 10. Functional requirements

Priority words follow RFC 2119.

### A. Re-entry and action availability

- **FR-A1** After the first resolved intervention the player MUST be able to begin further cycles in the same save without scenario reset, forest regeneration, or loss of any earlier mark, order, event, report, snapshot or deadwood record. `[D-060][D-004]`
- **FR-A2** Every management action available in the first cycle (Fell mark, Crop Tree designation, Work Plan import/approve/advance, pruning, nursery purchase, exact planting and shelters, vegetation clearance, regeneration removal, salvage) MUST remain available in every later year while the Outcome is not Failed and year `< ReviewYear`, and MUST NOT depend on lesson, `PlayerPrefs` or session-only state. Rack reopening and additional-rack authoring are governed by Rack V2, not by this spec. `[D-060][CODE R-1,R-2][RD-9]`
- **FR-A3** The teaching-footprint 40 % cumulative trunk-area safeguard MUST remain a **hard** constraint only while the lesson/sample activity that owns it is active. After that activity is complete, exceeding the historical footprint threshold MAY be shown as an **advisory warning** for manual later work but MUST NOT disable Approve Work Plan or Advance. This MUST NOT weaken a separately approved Multi-Scale Forestry Management prescription ceiling, which that feature owns. "Activity complete" is judged by the persisted-history completion condition of FR-B1. `[RD-5][CODE F-1]`

### B. Progression state

- **FR-B1** The guide / active-objective state MUST be **monotone** across legitimate later management (additional or lost Crop Trees, further felling in the teaching footprint, further cycles, executed-share drift) and across save/load, by using historical "ever satisfied" facts derived from persisted history and/or a frozen teaching-sample completion condition. A durable Annual Review / history fact MAY be the latch where it faithfully establishes that the release/review lesson has been completed. Before a lesson is historically complete, its existing in-lesson behaviour (including repeating the session `[J]/[K]` review after a load) is unchanged. If no persisted fact faithfully establishes completion in some state, the guide MUST keep its existing behaviour in that state and T-04 MUST report it; it MUST NOT invent a latch. `[RD-7][CODE F-2..F-4]`
- **FR-B2** Stable saved IDs (lesson step IDs, objective IDs, management event types, work-order IDs) MUST NOT be renamed or reused; new states or lessons MUST add new IDs. Visible lesson numbering MUST NOT be treated as persistent identity. `[packet][LearningObjectivesView.Curriculum][MFM §16.7]`
- **FR-B3** Session-only `[J]/[K]` inspection state and device-wide `PlayerPrefs` MUST NOT be progression or cycle authority after the associated lesson has historically been completed, and MUST NOT supply cycle state at any time. S1-D MUST NOT persist `[J]/[K]` decisions; if Multi-Scale Forestry Management needs them as prescription inputs, that persistence is owned by the Multi-Scale spec. `[RD-7][CODE F-8]`

### C. Previous work versus current proposals

- **FR-C1** For every work item the player MUST be able to see which of these it is, derived from existing orders, events and history only: Executed (with year and outcome), Approved (awaiting the next advance), Pending (planned, not approved), Pending carried over from an earlier year, or a Fell mark not yet in the Work Plan. `[RD-6][SPEC]`
- **FR-C2** Orders carried over from earlier years MUST be revalidated against the present forest before approval and MUST be surfaced when stale; nothing may be auto-cancelled or auto-approved. `[CODE R-3]` — retain existing behaviour.
- **FR-C3** Executed work MUST NOT be presented as a current proposal; failed tasks MUST remain visible with their reason. `[CODE R-4][SPEC]`
- **FR-C4** Where the persisted record lacks a designation time (Crop Trees), the UI MUST say nothing that states or implies a date. `[RD-6]`
- **FR-C5** S1-D V1 MUST NOT add a Crop Tree designation-year field and MUST NOT add a dated past-work map layer. Spatial review of earlier interventions MAY reuse S1-C history and waypoint surfaces where recorded locations already exist. `[RD-6]`

### D. Cycle state, readiness and review

- **FR-D1** The game MUST provide a derived, explainable **advisory** cycle state from persisted records only. It MUST report components and reasons, not a single prescriptive yes/no, and MUST distinguish at least: approved work awaiting execution; pending work including carried-over work; stale or invalid orders requiring attention; whether the latest resolved intervention has been reviewed; and years since the latest resolved intervention as descriptive context only. `[RD-4]`
- **FR-D2** Readiness MUST NEVER be a gate or a timetable. No minimum observation interval is introduced. "Wait and observe" MUST remain a valid, non-penalised choice indefinitely while the scenario is manageable. Readiness MUST NOT instruct the player to intervene or present an interval as universal. `[RD-4][Brief][Milestone, Model 2: do not teach a repeat interval][CODE R-5]`
- **FR-D3** After the first advance, an outstanding Annual Review acknowledgement MUST be surfaced as advisory information when the latest intervention has not been reviewed, and MUST NOT block Approve Work Plan or Advance. The existing one-time first-advance acknowledgement is unchanged. The protected completion chronology MAY continue to require reviewed cycles. `[RD-3][CODE F-5]`
- **FR-D4** After the first guided learning sequence, later interventions MUST be ordinary player-directed management with advisory guidance and the existing global scenario objectives and completion conditions. The spec MUST NOT add a prescriptive objective for every cycle and MUST NOT imply a single correct treatment. `[RD-2][Brief]`

### E. Completion

- **FR-E1** Completion MUST remain reachable after any number of repeated cycles: no repetition may make a satisfied chronology condition unsatisfied, and no teaching-only state may be required for completion. `[CODE F-9][Milestone completion checks]`
- **FR-E2** Protected behaviour (§10.1) MUST NOT change under this spec. `[RD-8][D-057]`
- **FR-E3** A loaded save with duplicate or non-positive management-event IDs MUST be rejected **before any world or disk mutation**, with a clear validation error. The game MUST NOT silently repair, renumber or reinterpret historical event identity. If a shipped or accepted fixture is found to have ambiguous IDs, work MUST STOP and return to the Manager before any compatibility policy is changed. This adds no schema field and requires normal load-path verification (CanLoad/LoadData parity). `[RD-12]`
- **FR-E4** The protected minimum of **two reviewed successful intervention cycles in distinct years** remains the completion chronology requirement and MUST NOT be increased. Continued management after `Completed` is accepted until the existing scenario horizon or terminal conditions; Completed is an achievement state, not an automatic lockout. `[RD-8]`

### F. Save and history re-entry

- **FR-F1** After save and load at any point in the cycle, the cycle state, history, previous/proposed classification and next-step guidance MUST be reproduced from persisted records, and reading them MUST NOT write any field. `[CODE R-4][SPEC]`
- **FR-F2** S1-D V1 MUST add **no new persisted field**. Any future S1-D schema work (outside V1) MUST use the next available schema version on the actual integration base and MUST NOT silently reuse a version owned by another accepted or qualified feature. No fixed version number is named by this spec. `[RD-6][RD-7][MFM §14][Rack V2 §34]`
- **FR-F3** Reference Future v1 (archive, preview, continuation, hashes) and Legacy40 anchors MUST be unchanged. `[D-042][D-056]`

### G. Neighbouring work and S1-C

- **FR-G1** Multi-Scale Forestry Management: prescription-generated Work Plan orders are **ordinary orders**; the cycle model uses their provenance for display only. A second prescription / re-derivation is **not** part of S1-D V1 and belongs to a Multi-Scale amendment. S1-D MUST NOT depend on MFM, and MFM does not satisfy S1-D. `[D-065][RD-11]`
- **FR-G2** Rack V2: the Objective 1 rack teaching MUST NOT automatically repeat in every management cycle. Rack reopening and additional-rack authoring are decided by Rack V2 and MUST NOT be decided here. `[packet][RD-9]`
- **FR-G3** Three-species parity: later cycles may involve Oak and Beech work. S1-D MUST NOT assume Sitka-only, MUST NOT add species-specific coefficients, and MUST NOT present Oak/Beech behaviour that parity work has not delivered. `[D-059][D-065]`
- **FR-G4** S1-C: S1-D MUST consume the history contract in §9 and MUST NOT create a competing history store, identity scheme or duplicate history UI. Only the read-only cycle-state adapter of §9.4 is permitted. `[RD-1][S1-C FR-14]`

### H. Compatibility

- **FR-H1** Geometry-independent derived presentation and re-entry features MAY work for Legacy40 saves where the required historical records exist. Enlarged80-only rack, sample and curriculum features MUST NOT be migrated into Legacy40. Reference Future v1 remains frozen. `[RD-10][D-064][D-065][D-042]`
- **FR-H2** Pre-feature Enlarged80 saves MUST load without fabricated history, cycle state or provenance. `[D-065]`

### 10.1 Protected behaviour (unchanged by this spec)

- `ScenarioOneCompletionRules` normalised source and its N3-pinned hash;
- the eight objective predicates and targets, and the D-057 geometry-aware effective targets;
- economy and ecology coefficients, work-economy values and thinning-tier schedules;
- Reference Future v1 (frozen, immutable, D-042) and the Legacy40 / Enlarged80 protected anchors;
- no Windows build;
- no fixed intervention interval;
- no automatic treatment choice, and no hidden automation that replaces the player's forestry judgement.

## 11. Authority / source of truth

- **Accepted product intent:** D-004, D-009, D-010 (as amended), D-013, D-014, D-042, D-056, D-060, D-063, D-064, D-065; Milestone (S1-D pre-demo; completion checks); Brief (loop; no single correct prescription); Manager decisions RD-1…RD-12 in `MANAGER-REVIEW.md`.
- **Implementation facts:** the Overview records none of §4. All §4 facts are `[CODE c6342f7]`: static, unintegrated, **partial implementation evidence, not canonical completion** (§3).
- **Supporting evidence:** `Docs/Design/ScenarioOneSequentialObjectivesV1.md`, `Docs/Verification/ThreeSpeciesProgramme/Phase7to9-HistoryAndRepeatedInterventions.md`, the RACK-V2-03B handoff — design/verification records, not accepted decisions.
- **Spec authority:** `APPROVED`: task-level implementation authority for this scope only, subordinate to the project hierarchy, and only within each task's stated dependencies (T-01 first).

## 12. Resolved decision register (formerly OPEN-1…OPEN-11, plus the event-ID decision)

All decisions are the Overall Manager's, recorded in `MANAGER-REVIEW.md` at `457461b9b92da2e96101f5b0eb9940b4e92fec0a`. **No decision remains open.**

| Id | Topic | Decision | Effect |
|---|---|---|---|
| RD-1 (OPEN-1) | Candidate-lineage baseline | Accept from the `c6342f7` lineage, as the **partial implementation baseline** S1-D extends and subject to T-01 re-verification on the actual integrated base: the derived management/intervention history architecture; the protected reviewed-two-cycle completion rule; completion guidance; the derived sequential lesson/guide architecture. This does **not** mean S1-C or S1-D are complete; the Milestone's pre-demo wording stays correct until their approved specs are implemented, integrated and feature-qualified. "No new S1-C/S1-D development needed" is not product authority. The two-cycle rule and its normalised-source hash stay unchanged unless a future explicit Manager decision opens them. | §3 L-2, §9, FR-G4, T-01 |
| RD-2 (OPEN-2) | Later-cycle mode | **Bounded hybrid.** The first guided sequence is the minimum teaching path; afterwards interventions are ordinary player-directed management with advisory guidance and the existing global objectives/completion. No new prescriptive objective per cycle; no implied single correct treatment. | FR-D4 |
| RD-3 (OPEN-3) | Recurring Annual Review acknowledgement | **Advisory, not a recurring hard gate.** Surface when the latest intervention is unreviewed; the protected completion chronology may continue to require reviewed cycles; later Work Plan approval/advance must not be blocked merely because an acknowledgement is outstanding. | FR-D3, T-02 |
| RD-4 (OPEN-4) | "Ready for next intervention" | An **advisory derived state**, never a gate and never a timetable; reports components/reasons, not one prescriptive yes/no; at minimum: approved work awaiting execution; pending incl. carried-over; stale/invalid orders; whether the latest resolved intervention has been reviewed; years since the latest resolved intervention as descriptive context only. No minimum observation interval; "wait and observe" is valid indefinitely while the scenario is manageable. | FR-D1, FR-D2, T-02 |
| RD-5 (OPEN-5) | 40 % teaching-footprint cap | Must **not** remain a lifetime hard block on ordinary later management. Retain the existing hard constraint during the owning lesson/sample activity; afterwards exceeding the historical threshold may be an advisory warning for manual later work but must not disable Approve Work Plan or Advance. Does not weaken a separately approved MFM prescription ceiling. | FR-A3, T-03 |
| RD-6 (OPEN-6) | Previous-versus-new provenance | **Derived from existing orders/events/history only.** No Crop Tree designation-year persistence; no dated past-work map layer under S1-D V1. Where designation time is absent say nothing implying a date. Spatial review may reuse S1-C history/waypoint surfaces where recorded locations exist. | FR-C1..C5, T-05 |
| RD-7 (OPEN-7) | Guide / lesson progress authority | Keep the guide **derived from persisted history; no new save progress field for S1-D V1.** Make the sequence monotone using historical "ever satisfied" facts / a frozen teaching-sample completion condition. Session-only `[J]/[K]` and device-wide `PlayerPrefs` are not progression authority after the lesson has historically completed. A durable Annual Review/history fact may be the latch where it faithfully establishes completion. Do not persist `[J]/[K]` merely to prevent regression; any MFM need for them is owned by the MFM spec. | FR-B1, FR-B3, T-04 |
| RD-8 (OPEN-8) | Completion and post-completion management | Keep the protected minimum of **two reviewed successful intervention cycles in distinct years**; do not increase it. Continued management after `Completed` is accepted until the existing horizon/terminal conditions; Completed is an achievement, not a lockout. | FR-E1, FR-E2, FR-E4 |
| RD-9 (OPEN-9) | Racks in later cycles | S1-D does not decide rack reopening/additional-rack authoring; defer to Rack V2. S1-D only requires that Objective 1 rack teaching does **not automatically repeat** in every management cycle. | FR-A2, FR-G2 |
| RD-10 (OPEN-10) | Legacy40 scope | Geometry-independent derived presentation/re-entry features may work for Legacy40 where the required records exist. Enlarged80-only rack/sample/curriculum features are not migrated to Legacy40. Reference Future v1 stays frozen. | FR-H1 |
| RD-11 (OPEN-11) | Second Multi-Scale prescription | **Not part of S1-D V1.** MFM-generated Work Plan orders are ordinary orders. Re-derivation belongs to an MFM amendment. | FR-G1 |
| RD-12 (additional) | Ambiguous management-event IDs | T-06 hardening approved in principle: **reject before mutation** a save with duplicate or non-positive management-event IDs, with a clear validation error; do not silently repair, renumber or reinterpret historical identity; if a shipped/accepted fixture is ambiguous, **STOP** and return to the Manager. Save-history integrity gate; normal load-path verification; no schema field. | FR-E3, T-06 |

## 13. Evidence classification

- **Established / accepted authority:** D-060 requirement; D-004 loop; D-013/D-014 mark semantics; Manager decisions RD-1…RD-12.
- **Model derivation:** history and cycle state computed from persisted records.
- **Gameplay calibration (inherited, unchanged here):** `StaleAfterYears = 2` [C]; 30 % release / 40 % safeguard / three Crop Trees / 8 m spacing [D]; the two-cycle completion minimum.
- **Evidence-informed game design:** observe-then-reassess framing. This spec introduces **no ecological claim** and **no coefficient**. That waiting can be correct is existing Lesson 5 copy, not new evidence.

## 14. Save / backward compatibility

- **S1-D V1 adds no persisted field.** History and cycle state are derived from records already saved (events, reports, snapshots, orders, deadwood, `outcome`) `[CODE R-4]`. `CurrentVersion` is neither read for decisions nor changed by S1-D V1.
- **Any future S1-D schema work** (outside V1) must inspect the actual integration base immediately beforehand, use the next available schema version, never reuse a version owned by another accepted or qualified feature, give every field a documented default and fabricate nothing in old saves. This spec names no fixed future version. (Rack V2 persistence currently occupies a version at `c6342f7`; S1-C and MFM each sequence their own schema work.)
- **Old saves:** missing fields mean no cycle-specific state. Pre-feature Enlarged80 and Legacy40 saves keep their viable progression path. Reference Future v1 is unaffected.
- **Load integrity:** FR-E3 adds a rejection rule, not a field.

## 15. Security / write-back

Not applicable in the trust-boundary sense (single-player, local save). Load validation is relevant to FR-E3: rejection must occur before any world mutation and without touching disk bytes (the existing pre-mutation contract shared by CanLoad and LoadData), with a clear error and no repair.

## 16. Acceptance criteria

Checkable; Unity-dependent items require a Unity-capable lane and are **not claimed by this specification**.

- **AC-1** Three consecutive real cycles (one with idle years between) run through the real manager produce the expected events, orders, reports and history, and every action in FR-A2 works in cycle 3 with no lesson state set. *(Unity multi-cycle witness; extends the existing `ScenarioHistoryVerification` / `N3CompletionVerification` approach.)*
- **AC-2** With a rack layout present, once the owning teaching activity is complete, exceeding the historical footprint threshold shows an advisory warning and Approve Work Plan and Advance remain enabled; during the activity the existing hard constraint still applies and its message names the rule, footprint, quantity and remedy. (FR-A3)
- **AC-3** After the teaching sample lesson is historically complete, the guide does not regress after (i) a fourth Crop Tree in the footprint, (ii) a Crop Tree being lost, (iii) a second in-footprint felling year, (iv) executed-share drift, (v) save and load at lessons 6–8. Before historical completion the existing in-lesson behaviour is unchanged. No new save field or `PlayerPrefs` read supplies the latch. (FR-B1, FR-B3)
- **AC-4** The same save loaded twice yields identical history, cycle state and next-step text; reading writes no field. (FR-F1)
- **AC-5** Executed, approved, pending, carried-over-pending and unplanned Fell-mark work are visibly distinct; no display states or implies a Crop Tree designation time; no dated past-work map layer exists in S1-D. (FR-C1..C5)
- **AC-6** A scripted ≥3-cycle path completes Scenario One and stays solvent; the eight objective predicates, D-057 targets and the N3 normalised-source hash are unchanged. The D-057 solvent feasibility witness (three intervention years, Year 25, minimum cash €11,451.16; one witness, not an optimum) is re-run on the implementation base and its result reported, not assumed. (FR-E1, FR-E2)
- **AC-7** A save with a duplicate or non-positive management-event ID is rejected before any world or disk mutation with a clear message and is **not** repaired or renumbered; CanLoad and LoadData agree. If a shipped fixture proves ambiguous, work stopped and the Manager was consulted. (FR-E3)
- **AC-8** Reference Future v1 contract, Legacy40 start/year-3 anchors, the Enlarged80 anchors and the pre-existing protected fingerprints are unchanged; **the S1-D diff adds no persisted field**. (FR-F2, FR-F3)
- **AC-9** Readiness reports components and reasons; it never blocks play, never instructs the player to intervene, states no interval, and leaves "wait" valid. An outstanding review acknowledgement after the first advance never disables Approve or Advance. (FR-D1..D3)
- **AC-10** Independent review finds no hidden automation that replaces the player's forestry judgement and no copy that implies causation the simulation does not hold. (§10.1)
- **AC-11** There is exactly one derived-history implementation and no second history store, identity scheme or duplicated history UI on the integrated base; the cycle-state adapter is read-only and `UnityEngine`-free. (FR-G4, §9.4)
- **AC-12** Objective 1 rack teaching does not re-trigger in later cycles; S1-D adds no rack-reopening or additional-rack behaviour. (FR-G2)
- **AC-13** Derived presentation features work on a Legacy40 save where the records exist; no Enlarged80 rack/sample/curriculum feature appears in Legacy40; Reference Future v1 is unaffected. (FR-H1)
- **AC-14** After `Completed`, management continues until the existing horizon/terminal condition; Completed does not lock the Work Plan. (FR-E4)
- **AC-15** Multi-Scale-generated orders (when present) behave as ordinary orders in the cycle model; S1-D contains no second-prescription logic. (FR-G1)

## 17. Stop conditions

Return to the Manager rather than assuming if:

- **T-01 fails or cannot be run** on the integrated Rack V2 + approved S1-C contract base, or a §4 finding is no longer true;
- the implementation baseline cannot be established or the lineage tip moved so that §4 facts no longer hold;
- a shipped or accepted save fixture has duplicate or non-positive management-event IDs (RD-12);
- the integrated base would contain **two** derived-history implementations, a competing history store, or duplicated history UI (§9.4);
- the S1-C approved text contradicts §9.2 in a way that changes what S1-D consumes;
- a fix requires changing the normalised `ScenarioOneCompletionRules` source, an objective predicate or target, or economy/ecology values;
- a fix would add any persisted field (V1 allows none) or reuse a schema version owned by another feature;
- a fix needs Rack V2 lane-owned files (`RackGeometry`, `RackPlanner`, `ScenarioOneManager.RackPlan.cs`, rack lifecycle) — **CROSS-LANE API REQUIRED**;
- T-03 finds a reachable state in which the owning teaching activity can never be completed and the retained hard constraint therefore blocks all management indefinitely;
- repeated cycles make completion unreachable, or economically unrecoverable, without a coefficient change;
- the work would add species-specific coefficients, a fixed intervention interval, an automatic treatment choice, or a second authoritative history/state machine.
