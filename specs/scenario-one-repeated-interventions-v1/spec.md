# Scenario One Second and Subsequent Interventions (S1-D) Specification

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Status:** DRAFT — not approved; authorises no implementation (specs/README.md)
- **Owner:** Overall Manager (ChatGPT Game Dev); drafted by the S1-D Primary specification author
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27
- **Decision log references:** D-004, D-008, D-009, D-010 (amended in part by D-065), D-013, D-014, D-042, D-056, D-060, D-063, D-064, D-065
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness; this is the **S1-D** pre-demo item (D-060)
- **Related evidence:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/HANDOFF.md`; sibling draft `spec/scenario-one-review-history-v1` (S1-C, packet only at the time of writing); `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`; `specs/multiscale-forestry-management-v1/`
- **Supersedes / superseded by:** none
- **Packet:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/TASK-PACKET.md` — base `3b499aa459b92212f7bb06897836bae5914b5230`, context `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`
- **Task class:** SPEC REQUIRED — specification only. No production code, Unity edits, coefficients or Windows build are part of this draft.

## How to read this draft

Every requirement carries an **Authority** tag:

- `[D-nnn]`, `[Milestone]`, `[Brief]` — settled by accepted project authority;
- `[CODE c6342f7]` — an inspected implementation fact (see §3 for what that means and its limits);
- `[PROPOSED]` — a drafting proposal that no accepted authority settles. It is a proposal, not a decision. Where it hides a genuine product choice it points to an `OPEN-n`.

Where accepted authority does not settle an item the draft says `OPEN — MANAGER/USER DECISION REQUIRED` and gives the narrowest decision statement (§12). Recommendations are labelled **non-binding**. Nothing was chosen merely to simplify implementation.

## 1. Product intent

The player must be able to repeat

`assess → decide/mark/plan → Work Plan → execute → advance time → observe/review → decide again`

without resetting the scenario, fabricating a new forest, or treating the first intervention as a one-shot tutorial that blocks later management. This is the accepted loop `decide → advance time → walk → observe → understand why → decide again` [D-004] applied repeatedly, and it is a **pre-demo requirement** because the first external demo contains the complete Scenario One, in which the player makes "second and later interventions", repeats the management cycle and reaches completion, and "by scenario completion … has learned the basic CCF management loop through repeated intervention and adaptation" [D-060, Milestone completion checks].

## 2. Problem

Accepted authority states the requirement (D-060) and that S1-D is "not implemented" (Milestone, context `eb218ea`). It does not define: when a further cycle becomes available, what "ready for next intervention" means, how earlier work is told apart from new decisions, or how repeated cycles interact with the lesson sequence, completion and saves. Inspection (§4) shows that on the current Scenario One code lineage the underlying management actions already repeat, but a small number of first-cycle assumptions can still block, mislead or regress the player on re-entry, and several design questions have no home at all. This spec fixes the requirement, records the verified assumptions, and isolates the product decisions that are genuinely open.

## 3. Inspection basis — read this before relying on any finding

**Finding L-1 (lineage).** The packet base `3b499aa` (`main`) does **not** contain the lesson curriculum these requirements concern. `main` has save `CurrentVersion = 20`, no `rack.approve`/`rack.result` steps, and none of the history, completion-chronology or sequential-lesson code described below. That code lives on the Scenario One integration lineage that is not an ancestor of `main` (merge-base `c1564b8`). The current tip of that lineage, and the registered BASE of the active Rack V2 lanes in `active-tasks.md`, is `c6342f79a3e89c040fadd232017a937132779385` (save `CurrentVersion = 21`; RACK-V2-03B reports a Unity gate chain PASS at this commit).

Therefore:

- **`[CODE c6342f7]`** means: read statically from the Git object at `c6342f7`, **not run in Unity** by this author, and **not integrated into `main`**. File and symbol names are given rather than line numbers because the Rack V2 lanes are changing lesson-sequence code (03C may make "narrow rack lifecycle lesson-sequence changes"; 02B-R is 3 commits ahead; 03B 1 ahead).
- **Implementation facts must be re-verified at the actual implementation base** before any task is dispatched (T-01).
- The canonical Unity Project Overview records none of this code. Under the authority hierarchy the Overview describes what exists *on its recorded baseline*; the status of the candidate lineage is therefore `OPEN-1`.

**Finding L-2 (prior work).** The candidate lineage already holds code and documents that anticipate S1-C and S1-D: derived management history (`ScenarioInterventionHistory`, `ScenarioOneManager.History.cs`, `ManagementHistoryText`; commit `7a023eb`, 2026-10-12); a **reviewed two-cycle completion rule** in save v20 (`ScenarioOneCompletionRules`, `7840872`, 2026-10-14) pinned by a normalised-source hash in `Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py`; completion guidance (`b87b52e`); the derived eight-lesson `ScenarioOneTeachingSequence` (`420fcb6`, 2026-10-20); `Docs/Design/ScenarioOneSequentialObjectivesV1.md`; and `Docs/Verification/ThreeSpeciesProgramme/Phase7to9-HistoryAndRepeatedInterventions.md`, which lists "canonical updates required (not made)". The Phase 7–9 record is explicitly *CLOUD / NOT UNITY-VERIFIED*. None of it is in the Decision Log, Milestone or Overview, and the design document asserts "No new S1-C/S1-D development needed", which conflicts with the Milestone's "S1-C and S1-D are not implemented". This draft **does not resolve that conflict**; it extends the lineage conditionally on `OPEN-1`.

**Finding L-3 (sibling spec).** The S1-C branch `spec/scenario-one-review-history-v1` (`8177cf6`) contained only its task packet when inspected. The history interface in §9 is therefore derived from the S1-C packet's stated intent and from the existing derived-history code, and **must be reconciled with the S1-C draft** when it exists.

## 4. Verified first-cycle assumption register

Class key: **B** can block Approve/Advance or a management action; **R** can regress or mislead progression state; **G** design gap (nothing exists to rely on); **I** informational dependency.

| ID | Cls | Finding `[CODE c6342f7]` | Why it matters for re-entry |
|---|---|---|---|
| F-1 | **B** | `LearningObjectivesView.RackPlanProblem()` returns a non-empty problem when `Facts.Bay != null && AccessOpen && !Bay.CanopyKept`. `ScenarioOneUiRoot.ApproveWorkPlan()` and the advance path return false while it is non-empty, and `WorkPlanView` disables Approve and Advance. It has **no lesson-stage condition**. `Bay.RemovedBasalShare` is **cumulative over the whole game**: all completed felling in the teaching footprint (served by a Crop Tree or not), plus planned and marked trees, over everything the footprint held (`ScenarioOneRackBay.Analyse`). Constant `MaxRemovedBasalShare = 0.40`. | A teaching safeguard ("interactive planning safeguard only") becomes a lifetime Approve/Advance ceiling on the middle-rack footprint. A legitimate later cycle that thins there is blocked game-wide until the player cancels footprint work. Intent versus accident is not recorded: `OPEN-5`. |
| F-2 | R | `ScenarioOneRackBay.Analysis.HasEnoughFuture` requires **exactly** `FutureTreesWanted = 3` valid, non-too-close Crop Trees spread across near/middle/far zones and both sides. `ScenarioOneTeachingSequence.Current` returns lesson 2 when `Bay != null && !HasEnoughFuture`. Separately, `ReleaseExecuted` is recomputed from *current* shares (`MinExecuted >= 0.30 × ExecutedTolerance 0.9`); the code comment accepts only "modest growth" as neighbours outgrow stumps ("not an unlimited completion guarantee"). | A fourth Crop Tree in the footprint, or the loss of one, returns the guide to "Choose promising trees" after the player has finished it; over many years the executed-release measure can also drift below threshold and pull the guide back to lesson 3 or 4. Pedagogical only (`Facts` are "never new scenario completion requirements"). |
| F-3 | R | `Analysis.ReleaseYear` is the latest year of an in-footprint removal that serves a Crop Tree; `ReleaseReviewed = Reviewed(ReleaseYear)`. `ReleaseKey` is built from **every** in-footprint completed felling, and `ScenarioOneRackReview.Synchronize` resets the session review whenever it changes. `Current` returns lesson 5 while `!ReleaseReviewed` or `!RackReviewComplete`. | Any later in-footprint felling re-opens "Return and review" (by a new `ReleaseYear` when it serves a Crop Tree, otherwise by the session reset). A second cycle in the same footprint regresses the guide. |
| F-4 | R | `ScenarioOneRackReview` (the `[J]`/`[K]` decisions, inspected trees, six stand portions) is **session-only**; `TeachingWorldVersion` change resets it; `Current` requires `RackReviewComplete` when a bay applies. Lesson-5 copy: "After loading, repeat this inspection review; it is not stored in the forest save." | After every load the guide returns to lesson 5 until six portions and three trees are re-reviewed, however late the game is. Also relevant to MFM §15 (review choices must be deliberately persisted before they can feed a prescription). |
| F-5 | G | `ScenarioOneUiRoot.NeedsAnnualReview = AnnualReports.Count > 0 && !AnnualReviewSeen`; `annualReviewSeen` is a sticky bool set by `MarkAnnualReviewSeen`. It gates the **first** advance only. Later `AnnualReviewAcknowledged` events can be recorded after each resolution (`ShouldRecordAcknowledgement`) but nothing forces them; only the completion chronology and teaching indices 1, 4 and 6 read them. | Whether review is a recurring commitment or an optional habit is not defined: `OPEN-3`. |
| F-6 | G | "Readiness" exists only as text: `ScenarioInterventionHistory.NextStepHint`, `CompletionGuidanceState`, `StaleOrders`, `AdvanceGuard`. No derived state names where the player is in a cycle. | Required by the packet; `OPEN-4`. |
| F-7 | G | `ForestTree.markType` is a single enum (None/Fell/CropTree) with no year or provenance. Fell marks are **consumed** on import (`AddMarkedTreesToWorkPlan` → `marking.ClearAll()`); Crop Tree marks persist indefinitely. Work orders do carry `createdYear`, `resolvedYear`, `status`, `rackId`. The Stand Map layers are Access/Light/Regeneration/Browse/Marks; stumps are drawn as a state, with no dated past-work layer. | Previous Crop Tree designations cannot be told from new ones; executed work is visible as stumps and history text but not as "where and when" on the map: `OPEN-6`. |
| F-8 | R | Learning practice steps are stored in device-wide `PlayerPrefs`: `LearningObjectivesView.Record(id)` writes `PreferencePrefix + id` (`"CCF.Learning.v1."`) and `ReloadProgress` rebuilds `done` from them. They are not in the save. `ScenarioOneManager.TutorialHint` embeds `Learning.Summary`. The sequential guide itself is derived from the save (design doc), so the exposure is the practice-step record and hint text. | A second save or a new game on the same device inherits practice credits; prefs cannot be cycle authority. |
| F-9 | I | `ScenarioOneCompletionRules.HasReviewedTwoCycleSequence`: two **successful** resolved plans (any task type) in distinct years with an `AnnualReviewAcknowledged` event between them by event-ID order. `EvaluateProgress` sets Completed only when all eight objectives are met **and** this holds. It is existential and therefore monotone. `Plans()` returns an empty list if any event ID is `<= 0` or duplicated. The management-event validators in `ForestSaveValidation` check only cell index, owner minutes and execution method; none checks event IDs. | Repeated cycles can only help the rule. A save with ambiguous IDs can load and never complete. The normalised source is hash-pinned by the N3 harness, so editing it is a G3 matter. |
| F-10 | I | `GetHarvestQuote` counts distinct years with a successful `FellTree` event and passes it as `previousInterventions`; `ScenarioOneEconomyAdapter` maps 0/1/≥2 to `FirstThinning`/`SecondThinning`/`LaterThinning` cost schedules (`Stage1EconomyDefaults`). The history ordinal is deliberately separate. | The economy is already cycle-count-aware. Anything that changes what counts as a felling year changes quotes. |
| F-11 | I | `CanManage()` is true while `outcome != Failed && CurrentYear < ReviewYear` (`ReviewYear = max(MinimumCompletionYear, CenturyReviewYear)`, 100 by default). **Completed does not stop management.** Failed is terminal and is set by: no cash exactly `0` with no approved order; or year `>= ReviewYear` with objectives unmet. | Repeated spending can reach terminal failure. Repetition after completion is currently allowed; that decision is recorded only in a Phase 7–9 note (`OPEN-8`). |
| F-12 | I | Rack V2 persistence exists at `c6342f7` (`rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, `racks`, `ScenarioOneWorkOrder.rackId`). The rack objective is lesson 1; reopening a confirmed plan after work has progressed is Rack V2 spec §39.7 OPEN-3. | Rack work must not silently repeat, and S1-D must not pre-empt `OPEN-3`. |

### 4.1 Verified: already repeatable (no first-cycle blocker found)

| ID | `[CODE c6342f7]` |
|---|---|
| R-1 | Marking and Work Plan: `ForestTreeMarkingManager` and `ScenarioOneManager` have no reference to lesson/teaching state (only `TutorialHint` text does). Fell mark → `AddMarkedTreesToWorkPlan` → pending order (IDs from `nextWorkOrderId`) → `ApprovePendingWork` → `AdvanceYear`; Crop Tree designation persists. |
| R-2 | Planting (`TryPurchaseStock`, `TryDesignateExactPlanting`, shelters), vegetation clearance, regeneration removal, pruning (`TryDesignatePruning`, `BatchPruneCropTrees`), salvage create orders with no cycle-count assumption. (Pruning lift limits are stated in lesson text; not separately verified in code.) |
| R-3 | Open orders persist across years; `RevalidateOpenOrders` rechecks them against today's forest; `StaleOrders` surfaces orders with a validation problem or `>= StaleAfterYears (2)` years old. Nothing is auto-expired. |
| R-4 | `ScenarioInterventionHistory.Build` treats every resolved plan alike (no two-cycle special case); idle years add no intervention. Pure function of persisted records, hence identical after save/load. |
| R-5 | `AdvanceGuard` warns (never blocks) when planned work exists and none is approved; "Advance anyway" is always offered. |

## 5. Definitions (drafting vocabulary)

- **Management cycle** `[PROPOSED]`: one pass of the §1 loop. A cycle may legitimately end with **no** intervention (a year in which the player reassesses and decides not to cut; "retaining trees without another cut is success" is existing Lesson 5 copy).
- **Intervention** `[CODE c6342f7]`: a *resolved work plan* — a year in which at least one commissioned task succeeded. A presentation ordinal, deliberately separate from the economy's thinning tiers (F-10). Retained unchanged.
- **Previous work** `[PROPOSED]`: Executed (resolved, with year and outcome, including failed tasks and reasons).
- **Current commitment** `[PROPOSED]`: Approved orders awaiting the next advance.
- **Proposed work** `[PROPOSED]`: Pending orders (current and carried over from earlier years) and Fell marks not yet in the Work Plan. Crop Tree designations are a standing designation (D-013/D-014), not a work proposal.
- **Re-entry** `[PROPOSED]`: beginning any cycle after the first, in the same save, including after a save/load and after any number of idle years.
- **Ready for next intervention**: see §7.4 and `OPEN-4`. No accepted definition exists.

## 6. Goals

1. A player who has completed the first intervention can re-enter the cycle, in the same forest, indefinitely until the scenario's management horizon, with no reset and no lesson-state blocker.
2. The player can always tell previous work from current proposals, and can see what earlier treatment bears on the next decision (via the S1-C interface, §9).
3. Completion remains reachable, and understandable, after any number of repeated cycles.
4. The save is the sole authority on where the player is; nothing is fabricated for old saves.
5. Genuine product choices are surfaced, not buried.

## 7. Non-goals

- Production code, Unity or scene/prefab edits, Windows build.
- New ecology, economy, objective-target or calibration coefficients; changing the eight objective predicates or D-057 effective targets.
- General Scenario One curriculum redesign beyond what re-entry requires.
- Multi-Scale Forestry Management implementation (D-065), Rack V2 implementation (D-064), three-species parity (D-059/D-061/D-062), or the S1-C history UI itself.
- Mandating a repeat interval or a "correct" second treatment [Brief, "no universally correct prescription"].
- Scenario Two, sandbox, property-level multi-stand play.

## 8. Player workflows

### 8.1 Repeat cycle (target flow)

| Step | Player does | Exists today `[CODE c6342f7]`? | S1-D concern |
|---|---|---|---|
| 1. Assess | Walk, inspect (E), map layers, read history | Yes | Previous-vs-new clarity (FR-C) |
| 2. Decide/mark/plan | Fell (X) / Crop (C) marks, nursery, planting, pruning, clearance | Yes (R-1, R-2) | Not lesson-gated (FR-A2) |
| 3. Work Plan | Import marks, review cost and residual stand, choose executor and material, approve | Yes | F-1 must not block it (FR-A3) |
| 4. Execute and advance | Advance one year; approved work resolves | Yes | Idle advance remains legitimate |
| 5. Observe/review | Annual Review, history cards, walk the changed forest | Yes (derived history) | Recurring-review rule `OPEN-3` |
| 6. Decide again | Return to step 1 | Partly — guide can regress (F-2..F-4) | FR-B1 |

### 8.2 Entry conditions for the next cycle

Mechanically the next cycle is available as soon as the advance completes, for as long as the scenario is not Failed and the year is below the review year (F-11). Whether anything *further* gates it (acknowledged review, minimum observation interval, cleared stale orders) is `OPEN-3` and `OPEN-4`.

## 9. Interface expected from S1-C (Review / Management History)

S1-D consumes history; it does not build a second history system [S1-C packet; Phase 7–9 "derived, not stored"]. The interface S1-D **relies on** is:

| Id | Provided by S1-C | Present at `c6342f7` as |
|---|---|---|
| EI-1 | Ordered intervention list: ordinal, year, task counts by type and species, failed tasks with reasons | `ScenarioOneManager.Interventions` / `InterventionSummary` |
| EI-2 | Per intervention: cost, timber income, net, small-job minimum, owner minutes, closing cash | `InterventionSummary` |
| EI-3 | Per intervention: ecological snapshots before / after / latest; tree deaths not caused by felling (cause not recoverable) | `Before`, `After`, `Latest`, `OtherDeaths*` |
| EI-4 | Open orders with `createdYear`, validation message and stale flag | `StaleOrders`, `RevalidateOpenOrders` |
| EI-5 | Per resolved year: whether its Annual Review has been acknowledged | `AnnualReviewAcknowledged` events (no accessor yet — gap) |
| EI-6 | Completion summary at the completion year | `CompletionReview` |
| EI-7 | Single "what next" line and completion-guidance state | `NextStepHint`, `CompletionGuidance` |

Rules: S1-D MUST NOT depend on unimplemented MFM state [S1-C packet]; history displays MUST NOT claim causation the simulation does not contain (`ManagementHistoryText.NotCausalNote`); where provenance is absent S1-D MUST NOT synthesise it. **EI-5 is a gap:** no accessor reports "year Y acknowledged"; S1-C should supply it or S1-D will add a read-only derivation.

## 10. Functional requirements

Priority words follow RFC 2119. Requirements marked `[PROPOSED]` need Manager approval of this spec before they bind.

### A. Re-entry and action availability

- **FR-A1** After the first resolved intervention the player MUST be able to begin further cycles in the same save without scenario reset, forest regeneration, or loss of any earlier mark, order, event, report, snapshot or deadwood record. `[D-060][D-004]`
- **FR-A2** Every management action available in the first cycle (Fell mark, Crop Tree designation, Work Plan import/approve/advance, pruning, nursery purchase, exact planting and shelters, vegetation clearance, regeneration removal, salvage) MUST remain available in every later year while the Outcome is not Failed and year `< ReviewYear`, and MUST NOT depend on lesson, `PlayerPrefs` or session-only state. Exceptions only as decided under `OPEN-2` and `OPEN-9`. `[D-060][CODE R-1,R-2]`
- **FR-A3** No teaching-derived rule MAY block Approve Work Plan or Advance once the lesson that owns the rule has been completed, unless the Manager accepts that rule as permanent gameplay (`OPEN-5`). If accepted as permanent it MUST be explained at the point it blocks, MUST name the footprint and the quantity, and MUST say what the player can do. `[CODE F-1]`

### B. Progression state

- **FR-B1** The guide / active-objective state MUST NOT regress because of legitimate later management (additional or lost Crop Trees, further felling in the teaching footprint, further cycles) or because the save was loaded. `[CODE F-2..F-4]` The mechanism is a plan-level choice constrained by `OPEN-7`.
- **FR-B2** Stable saved IDs (lesson step IDs, objective IDs, management event types, work order IDs) MUST NOT be renamed or reused; new states or lessons MUST add new IDs. Visible lesson numbering MUST NOT be treated as persistent identity. `[packet][LearningObjectivesView.Curriculum][MFM §16.7]`
- **FR-B3** `PlayerPrefs` and other device-wide stores MUST NOT supply cycle or progression authority; the save is the authority. `[CODE F-8][PROPOSED]`

### C. Previous work versus current proposals

- **FR-C1** For every work item the player MUST be able to see which of these it is: Executed (with year and outcome), Approved (awaiting the next advance), Pending (planned, not approved), Pending carried over from an earlier year, or a Fell mark not yet in the Work Plan. `[PROPOSED]` (vocabulary §5; presentation owned jointly with S1-C)
- **FR-C2** Orders carried over from earlier years MUST be revalidated against the present forest before approval and MUST be surfaced when stale; nothing may be auto-cancelled or auto-approved. `[CODE R-3]` — retain existing behaviour.
- **FR-C3** Executed work MUST NOT be presented as a current proposal; failed tasks MUST remain visible with their reason. `[CODE R-4][PROPOSED]`
- **FR-C4** Where the persisted record lacks provenance (notably Crop Tree designation time), the UI MUST NOT state or imply it. `[PROPOSED]` See `OPEN-6`.
- **FR-C5** Previous interventions SHOULD be locatable spatially (where and when) from the Stand Map or the walked forest. `[PROPOSED]` The existing stump state and deadwood are partial; a dated layer is a new feature and is part of `OPEN-6`.

### D. Cycle state and readiness

- **FR-D1** The game SHOULD provide a derived, explainable account of the current cycle state from persisted records only, naming at least: approved work awaiting execution; pending and stale orders; whether the latest resolved plan's Annual Review has been acknowledged; and years since the latest resolved plan. `[PROPOSED]` Content of "ready" is `OPEN-4`.
- **FR-D2** Any readiness display MUST NOT instruct the player to intervene, MUST NOT present an interval as universal, and MUST keep "wait and observe" a legitimate, non-penalised choice. `[Brief][Milestone, Model 2: do not teach a repeat interval][CODE R-5]`
- **FR-D3** Whether readiness is a gate or advisory is `OPEN-3`/`OPEN-4`; until decided it MUST be advisory only (no new blocking).

### E. Completion

- **FR-E1** Completion MUST remain reachable after any number of repeated cycles: no repetition may make a satisfied chronology condition unsatisfied, and no teaching-only state may be required for completion. `[CODE F-9][Milestone completion checks]`
- **FR-E2** The eight objective predicates, D-057 effective targets, the Year-25 horizon, the review year and the `ScenarioOneCompletionRules` normalised source MUST NOT change under this spec. Any change is G3/G5. `[D-057][CODE F-9]`
- **FR-E3** A loaded save with ambiguous management-event IDs MUST NOT silently become un-completable: it MUST either be rejected with a clear message or be repaired deterministically, as decided in the plan. `[PROPOSED]` (hardening; no behaviour change for valid saves; touches `ForestSaveValidation`, so G5)
- **FR-E4** The minimum number of reviewed cycles for completion, and whether completion happens before or after more than two cycles, is `OPEN-8`. Until decided the current rule stands.

### F. Save and history re-entry

- **FR-F1** After save and load at any point in the cycle the cycle state, history, previous/proposed classification and next-step guidance MUST be reproduced from persisted records, and reading them MUST NOT write any field. `[CODE R-4][PROPOSED]`
- **FR-F2** Any new persisted field MUST use the next available schema version after Rack V2 (v21) and MFM, have a documented default, never fabricate state in older saves, and be justified in the plan against `OPEN-6`/`OPEN-7`. The version number MUST NOT be assumed. `[MFM §14][Rack V2 §34]`
- **FR-F3** Reference Future v1 (archive, preview, continuation, hashes) and Legacy40 anchors MUST be unchanged. `[D-042][D-056]`

### G. Neighbouring work

- **FR-G1** MFM: prescription-generated orders are ordinary Work Plan orders. The cycle model MUST treat them as ordinary orders and MAY use their provenance for display only. S1-D MUST NOT depend on MFM and MFM does not satisfy S1-D. `[D-065]`
- **FR-G2** Rack V2: the Objective 1 rack workflow MUST NOT repeat automatically in later cycles. `[packet]` Availability of rack planning after Objective 1 is `OPEN-9`.
- **FR-G3** Three-species parity: later cycles may involve Oak and Beech work. S1-D MUST NOT assume Sitka-only, MUST NOT add species-specific coefficients, and MUST NOT present Oak/Beech behaviour that parity work has not delivered. `[D-059][D-065]`

### H. Compatibility

- **FR-H1** Legacy40 saves and Reference Future v1 MUST keep their existing viable progression path; S1-D MUST NOT migrate them to Enlarged80/V2 curriculum. Which derived/presentation features also apply to Legacy40 is `OPEN-10`. `[D-064][D-065][D-042]`
- **FR-H2** Pre-feature Enlarged80 saves MUST load without fabricated history, cycle state or provenance. `[D-065]`

## 11. Authority / source of truth

- **Accepted product intent:** D-004, D-009, D-010 (as amended), D-013, D-014, D-042, D-056, D-060, D-063, D-064, D-065; Milestone (S1-D pre-demo; completion checks); Brief (loop; no single correct prescription).
- **Implementation facts:** Overview records *none* of §4. All §4 facts are `[CODE c6342f7]`, static, unintegrated (§3).
- **Supporting evidence:** `Docs/Design/ScenarioOneSequentialObjectivesV1.md`, `Docs/Verification/ThreeSpeciesProgramme/Phase7to9-HistoryAndRepeatedInterventions.md`, RACK-V2-03B handoff — design/verification records, not accepted decisions.
- **Spec authority:** when `APPROVED`, task-level authority for this scope only, subordinate to the project hierarchy. `DRAFT` authorises nothing.

## 12. Open product decisions

Each is `OPEN — MANAGER/USER DECISION REQUIRED`. **Blocking** means plan tasks cannot be dispatched until decided.

### OPEN-1 — Status of the candidate-lineage S1-C/S1-D baseline (**blocking**)
- **Fact:** §3 L-1, L-2. Milestone says not implemented; the lineage carries derived history, a reviewed two-cycle completion rule hash-pinned by N3, completion guidance and a derived eight-lesson guide, none recorded canonically.
- **Decision:** Are (a) the derived-history architecture, (b) the reviewed-two-cycle completion rule, (c) the derived sequential-lesson guide, and (d) the `c6342f7` lineage accepted as the baseline this spec extends, or are they proposals to be re-specified? Also: which canonical documents record the answer.
- **Non-binding recommendation:** accept (a)–(c) as the baseline *by decision-log entry*, because the code, N3 harness and RACK-V2-03B gate chain already depend on them; do not let the Overview stay silent.

### OPEN-2 — Management mode of later interventions
- **Fact:** Authority fixes the loop and "no single correct prescription" but not whether later cycles are objective-driven, free, or hybrid. At `c6342f7` the eight goals are objective-driven current-state tests; lessons 6–8 are advisory; completion needs two reviewed cycles; management is otherwise free.
- **Decision:** After the first guided cycle are later interventions (A) free management with advisory guidance only, (B) objective-driven, each cycle with a stated learning objective, or (C) a bounded hybrid (a guided minimum, free beyond)?
- **Non-binding recommendation:** (C), because it is the present behaviour and meets D-060's "learned … through repeated intervention and adaptation" without prescribing a treatment.

### OPEN-3 — Is a recurring Annual Review acknowledgement required?
- **Fact:** F-5. Only the first advance is gated.
- **Decision:** Before approving or advancing again, must the player acknowledge the latest resolved plan's Annual Review (a recurring gate), or is acknowledgement advisory except where the completion chronology needs it?
- **Non-binding recommendation:** advisory, with the existing completion guidance, because a recurring modal gate cuts across "no fixed interval" and "waiting is legitimate".

### OPEN-4 — What "ready for next intervention" means (**blocking** for FR-D)
- **Fact:** F-6. No accepted definition. Authority forbids a universal interval.
- **Decision:** Is readiness (a) an advisory derived state with named reasons, (b) a gate, or (c) not a concept in the player UI; and does any minimum observation interval (beyond the existing guide-only "one further year after planting") exist? If (a) or (b), which of these are components: latest plan reviewed; no unapproved pending; no stale orders; ≥1 year since last intervention.
- **Non-binding recommendation:** (a) with components "latest plan reviewed" and "no stale/invalid orders", and no interval.

### OPEN-5 — The 40 % teaching-footprint safeguard after the lesson
- **Fact:** F-1; MFM §5 item 7 keeps "40 % cumulative basal-area safeguard in the sample".
- **Decision:** After the sample lessons are complete does the cumulative 40 % trunk-area limit on the teaching footprint (a) cease to block Approve/Advance, (b) become an advisory warning, or (c) remain a permanent hard rule (with explanation)?
- **Non-binding recommendation:** (b). If MFM keeps the safeguard as a prescription ceiling, (c) may apply to prescription-generated work only.

### OPEN-6 — Provenance of marks and where earlier work is shown
- **Fact:** F-7. No Crop Tree designation time; no dated past-work map layer.
- **Decision:** Is "previous vs new" for Crop Trees (a) derived only from order and event history (no designation time), (b) backed by a new persisted designation year, and does the Stand Map gain a dated past-work layer?
- **Non-binding recommendation:** (a) for V1, no new persisted field, since D-013/D-014 treat a Crop Tree as a standing designation and what matters is what work followed; the past-work map layer is a separate presentation decision with S1-C.

### OPEN-7 — Authority for lesson and review progress (**blocking** for FR-B1 design)
- **Fact:** F-2..F-4, F-8. Guide state is derived; review decisions are session-only; practice credits are device-wide.
- **Decision:** May guide stability be achieved (a) by deriving "ever satisfied" only from persisted history, (b) by a small new per-save progress record (G5 save-policy), or (c) by freezing the sample-lesson facts once a defined history event occurs? And are `[J]/[K]` decisions persisted (MFM §15 requires this before they feed a prescription)?
- **Non-binding recommendation:** (a)/(c) first; (b) only if (a)/(c) cannot meet FR-B1.

### OPEN-8 — Completion rule and the horizon
- **Fact:** F-9, F-11. Completion needs two reviewed cycles; management continues after completion until year 100; Phase 7–9 records "Manager resolution 5" (Completed does not stop management) outside canon.
- **Decision:** Is the two-cycle minimum the intended meaning of D-060 "make second and later interventions, repeat the management cycle", or should completion require more; and is continued management after Completed an accepted behaviour?
- **Non-binding recommendation:** leave the protected rule unchanged and record continued management as accepted.

### OPEN-9 — Rack planning after Objective 1
- **Fact:** F-12; Rack V2 spec §39.7 OPEN-3 is itself open.
- **Decision:** After Objective 1 completes, may the player plan additional racks or reopen a confirmed plan in later cycles?
- **Non-binding recommendation:** defer to Rack V2 spec §39.7 OPEN-3; S1-D MUST NOT decide it.

### OPEN-10 — Scope across stand geometries
- **Fact:** D-064/D-065 keep Legacy40 off the V2 curriculum; the teaching-bay path is absent for stands without a rack layout.
- **Decision:** Do the derived/presentation re-entry features (cycle state, previous-vs-new) apply to Legacy40 saves or to Enlarged80 only?
- **Non-binding recommendation:** apply derived presentation features to both (they need no geometry); sample and prescription features stay Enlarged80-only.

### OPEN-11 — Second prescription (MFM) as part of S1-D
- **Fact:** D-065 says MFM may contribute but does not satisfy S1-D; MFM §15 allows a later second prescription only from persisted inputs.
- **Decision:** Is re-deriving a prescription in a later cycle part of S1-D, or a later MFM amendment?
- **Non-binding recommendation:** out of S1-D; an MFM amendment.

## 13. Evidence classification

- **Established / accepted authority:** D-060 requirement; D-004 loop; D-013/D-014 mark semantics.
- **Model derivation:** history and cycle state computed from persisted records.
- **Gameplay calibration (inherited, unchanged here):** `StaleAfterYears = 2` [C]; 30 % release / 40 % safeguard / three Crop Trees / 8 m spacing [D]; two-cycle completion.
- **Evidence-informed game design:** observe-then-reassess framing. This draft introduces **no ecological claim** and **no coefficient**. The statement that waiting can be correct is existing Lesson 5 copy, not new evidence.

## 14. Save / backward compatibility

- **Default posture:** no new persisted state. History and cycle state are derived from records already saved (events, reports, snapshots, orders, deadwood, `outcome`). `[CODE R-4]`
- **Possible schema impact (only if the Manager chooses it):** a per-save progress record (`OPEN-7(b)`) or a Crop Tree designation year (`OPEN-6(b)`). Each would need the next free schema version after v21/MFM, an explicit default, no fabricated values in old saves, and G5.
- **Old saves:** missing fields mean no cycle-specific state. Pre-feature Enlarged80 and Legacy40 saves keep their viable progression path. Reference Future v1 is unaffected.
- **Risk to carry:** F-9 ambiguous-event-ID saves (`FR-E3`).

## 15. Security / write-back

Not applicable in the trust-boundary sense (single-player, local save). Load validation is relevant to `FR-E3`: it must reject or repair, never silently degrade, and must not mutate world or disk on rejection (existing pre-mutation validation contract, D-058 / save hardening).

## 16. Acceptance criteria

Checkable; Unity-dependent items require a Unity-capable lane and are **not claimed by this draft**.

- **AC-1** Three consecutive real cycles (one with idle years between) run through the real manager produce the expected events, orders, reports and history, and every action in FR-A2 works in cycle 3 with no lesson state set. *(Unity multi-cycle witness; extends the existing `ScenarioHistoryVerification` / `N3CompletionVerification` approach.)*
- **AC-2** With a rack layout present, after the teaching lessons are complete, Approve and Advance are not disabled by the teaching footprint rule except as decided in `OPEN-5`; when disabled the message names the rule, footprint, quantity and remedy.
- **AC-3** The guide does not regress after (i) a fourth Crop Tree in the footprint, (ii) a Crop Tree being lost, (iii) a second in-footprint felling year, (iv) save and load at lessons 6–8 — per `OPEN-7`.
- **AC-4** The same save loaded twice yields identical history, cycle state and next-step text; no field is written by reading them.
- **AC-5** Previous (executed) work, approved work, pending work, carried-over pending work and unplanned Fell marks are visibly distinct (FR-C1); no display states a Crop Tree designation time unless persisted (FR-C4).
- **AC-6** A scripted ≥3-cycle path completes Scenario One, remains solvent, and leaves the eight objective predicates, D-057 targets and the N3 normalised-source hash unchanged. The D-057 solvent feasibility witness (three intervention years, Year 25, minimum cash €11,451.16; one witness, not an optimum) is re-run on the implementation baseline and its result reported, not assumed.
- **AC-7** A save with duplicate or non-positive management-event IDs is rejected with a clear message or repaired deterministically (`FR-E3`), without world or disk mutation on rejection.
- **AC-8** Reference Future v1 contract, Legacy40 start/year-3 anchors, the Enlarged80 anchors and the pre-existing protected fingerprints are unchanged; no new schema field unless approved.
- **AC-9** No readiness display blocks play or tells the player to intervene on a timetable (FR-D2/D3).
- **AC-10** Independent review finds no hidden automation that replaces the player's forestry judgement, and no copy that implies causation the simulation does not hold.

## 17. Stop conditions

Return to the Manager rather than assuming if:

- `OPEN-1` is unresolved when planning would depend on the baseline;
- the implementation baseline cannot be established (the lineage tip moved and §4 facts no longer hold);
- a fix requires changing the normalised `ScenarioOneCompletionRules` source, an objective predicate or target, or economy values (G3/G5);
- a fix requires a new schema field not approved under `OPEN-6`/`OPEN-7`;
- a fix needs Rack V2 lane-owned files (`RackGeometry`, `RackPlanner`, `ScenarioOneManager.RackPlan.cs`, rack lifecycle) — **CROSS-LANE API REQUIRED**;
- repeated cycles are found to make completion unreachable, or economically unrecoverable, without a coefficient change;
- the S1-C draft contradicts §9;
- the work would add species-specific coefficients or a second authoritative history/state machine.
