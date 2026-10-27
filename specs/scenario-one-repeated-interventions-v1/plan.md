# Scenario One Second and Subsequent Interventions (S1-D) Implementation Plan

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** DRAFT — this plan is provisional and **dispatches nothing**. It implements the spec; it creates no product requirement. Tasks that depend on an `OPEN-n` are marked BLOCKED in `tasks.md`.
- **Plan owner:** Overall Manager; drafted by the S1-D Primary specification author
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27

## 1. Architecture / current-state findings

Everything below is `[CODE c6342f7]` — static reading of Git objects, not run in Unity by this author, not on `main` (spec §3, L-1). The full register is spec §4; this plan only restates what shapes the approach.

**The loop already works mechanically.** Marks, Work Plan, pruning, planting, clearance and salvage create ordinary orders with no cycle-count assumption (spec R-1, R-2). History is a pure function of persisted records (`ScenarioInterventionHistory`) and treats every resolved plan alike (R-4). Stale orders are surfaced and revalidated (R-3). Completion is existential and monotone (F-9).

**The first-cycle residue is in the teaching layer and a few gaps**, not in the simulation:

| Layer | Residue | Spec |
|---|---|---|
| Approve/Advance gating (UI) | `RackPlanProblem` is a lifetime, game-wide block driven by a cumulative teaching-footprint basal-area share | F-1 |
| Teaching guide (`ScenarioOneTeachingSequence.Current`) | Non-monotone projection of the *current* world; regresses on Crop Tree count ≠ 3, further in-footprint felling, executed-share drift, and every load (session-only `ScenarioOneRackReview`) | F-2, F-3, F-4 |
| Review chronology | Only the first acknowledgement gates play; later ones are optional events the completion rule reads | F-5 |
| Readiness | Text hints only; no derived state | F-6 |
| Provenance | Marks carry no time; no dated past-work view | F-7 |
| Device state | Practice steps in `PlayerPrefs` | F-8 |
| Integrity | Completion chronology assumes unique positive event IDs; validators do not check | F-9 |

**Economy and outcome are already cycle-aware and stay untouched** (F-10, F-11). The N3 harness pins the normalised `ScenarioOneCompletionRules` source, so that file is out of bounds.

## 2. Proposed technical approach

Principle: **derived-first, additive, no new persisted state unless the Manager approves it.** The save already carries what re-entry needs; the fixes are to stop the UI contradicting it.

### 2.1 Decouple teaching safeguards from commitment (FR-A3, `OPEN-5`)
Make `RackPlanProblem()` depend on whether the owning lesson is still active (or demote it to a warning) rather than on `Bay != null && AccessOpen`. This is a UI-layer change in `LearningObjectivesView.Sequence.cs` plus the two call sites (`ScenarioOneUiRoot.ApproveWorkPlan`/advance path, `WorkPlanView`). No simulation file changes. Which variant (a/b/c) is `OPEN-5`.

### 2.2 Make the guide monotone (FR-B1, `OPEN-7`)
Three candidate mechanisms, none chosen here:

- **A — latch from history.** Compute `Current(f)` as before, then raise it to the highest lesson whose *prerequisite facts are history-only* (e.g. a completed `PlantJuvenile` order or an `AnnualReviewAcknowledged` after the planting year can never un-happen). Pure and persistence-free. Does not protect a player who loads while still inside lesson 5, who repeats the `[J]/[K]` review as today.
- **B — small persisted record.** A per-save "sample lessons closed" flag. Fully stable, but a save-schema change (G5) and must not fabricate state in old saves.
- **C — freeze the sample facts** once a defined history event occurs, so later Crop Tree or felling changes in the footprint cannot re-open earlier lessons.

Recommendation (non-binding, pending `OPEN-7`): A, then C if A leaves visible regressions; B only if neither meets FR-B1. Whatever is chosen must keep step IDs stable (FR-B2) and must not touch `ScenarioOneCompletionRules`.

### 2.3 Derived cycle state and readiness (FR-D, `OPEN-3`, `OPEN-4`)
Add one pure, `UnityEngine`-free static class (working name `ScenarioCycleState`) beside `ScenarioInterventionHistory`, taking the same persisted lists plus `Outcome`/year and returning named components with reasons (approved work awaiting execution; pending and stale orders; latest-plan review acknowledged; years since the last resolved plan). It follows the Phase 7–9 pattern (derived, not stored, offline-testable on plain .NET with the `UnityEngine` shim). It is advisory until `OPEN-3`/`OPEN-4` say otherwise. EI-5 (per-year acknowledgement) is a read-only derivation here unless S1-C supplies it.

### 2.4 Previous versus proposed presentation (FR-C, `OPEN-6`)
Reuse the existing derived history and `StaleOrders`. Work Plan: group open orders as *Approved*, *Pending (new)*, *Pending (carried over)* using `createdYear` and the existing stale rule; link executed work to the Annual Review year card. Crop Trees stay a standing designation, with no designation-time claim (FR-C4). A dated past-work map layer is a new feature and is held for `OPEN-6` and the S1-C design. Walked-forest cues (stumps, deadwood) already exist.

### 2.5 Event-ID integrity (FR-E3)
Add a pre-mutation check that management-event IDs are positive and unique, either rejecting the save with a message or repairing deterministically on restore. Requires scanning shipped fixtures for any legitimately ambiguous save first. It touches `ForestSaveValidation` (shared load path), so G5, and must keep CanLoad and LoadData parity (existing contract).

### 2.6 Persistence (conditional only)
If `OPEN-6(b)` or `OPEN-7(b)` is chosen: next free schema version after Rack V2 (v21) and MFM, fields declared last and adjacent, documented defaults, validators, and the world-hash compatibility path used for the v21 rack fields (`ScenarioReferenceArchive.WithoutRackV2Fields` pattern). Otherwise this section is empty.

### 2.7 Rejected or deferred
- **Re-gating every advance on a modal review** — rejected pending `OPEN-3`; it conflicts with "waiting is legitimate".
- **A second history database or cycle state machine** — rejected; S1-C's derived history is the single source (spec §9).
- **Persisting `PlayerPrefs` practice credits into the save** — rejected; they are optional practice, not cycle authority.
- **Repeating the Objective 1 rack workflow per cycle** — out of scope (packet; `OPEN-9`).

## 3. Integration boundaries

- **May touch (provisional):** `LearningObjectivesView.Sequence.cs`, `LearningObjectivesView.Curriculum.cs` (new IDs only), `ScenarioOneTeachingSequence.cs`, `ScenarioOneUiRoot.cs`, `WorkPlanView.cs`, `AnnualReviewView.cs` (coordinate with S1-C), `ScenarioOneManager.History.cs`, a new pure cycle-state class, `ForestSaveValidation*.cs` (FR-E3), new verification under `Tools/Verification/`.
- **Must not touch:** `ScenarioOneCompletionRules.cs` and the eight objective predicates/targets (`ScenarioOneObjectives.cs`); economy/work-economy values; ecology, growth, regeneration, mortality; Reference Future v1 and its archive; `Racks/*`, `ScenarioOneManager.RackPlan.cs`, `ScenarioOneRackLayout.cs`, rack lifecycle (Rack V2 lanes — **CROSS-LANE API REQUIRED** if needed); scenes, prefabs, USS/UXML, ProjectSettings, packages.
- **Contracts used:** S1-C derived-history interface (spec §9); stable saved IDs; the pre-mutation save-validation contract; `CompetitionShare` untouched.

## 4. Affected systems / files (provisional, to be reconfirmed at the implementation base)

`Assets/ForestPrototype/UI/` — `LearningObjectivesView.Sequence.cs`, `.Curriculum.cs`, `ScenarioOneTeachingSequence.cs`, `ScenarioOneUiRoot.cs`, `WorkPlanView.cs`, `AnnualReviewView.cs`, `ActiveObjectiveHudWidget.cs`. `Assets/ForestPrototype/ScenarioOne/` — `ScenarioOneManager.History.cs`, new cycle-state class, `ScenarioOneRackBay.cs` only if `OPEN-5` makes the cap lesson-scoped inside the analysis. `Assets/ForestPrototype/ForestSaveValidation*.cs` (FR-E3). `Tools/Verification/` — new multi-cycle gate and offline suite.

## 5. Ownership / concurrency considerations

- **Hot files with other active writers.** Rack V2 03C (`RackGeometry`, `RackPlanner`, `ScenarioOneManager.RackPlan.cs`, and "narrow rack lifecycle lesson-sequence changes if required"), 02B-R (headland), MFM (adds a lesson after Lesson 5, so it edits the curriculum and `Current()`), and S1-C (Annual Review/Work Plan views). S1-D's guide and gating work edits the same files as MFM and S1-C.
- **Recommended sequencing (Manager decision):** Rack V2 integrated → S1-C → S1-D guide/gating fixes → MFM lesson insertion, **or** MFM before S1-D guide stability if MFM rewrites `Current()`. Parallel writers on the same file set are not permitted (AgentWorkflow: one writable lane per owner; single-writer high-conflict files).
- **Unity:** any Unity gate runs in a Unity-capable lane under the machine-wide Unity lock; parallel Unity is never permitted. This draft ran no Unity.
- **Locks:** no `active-tasks.md` row exists for this lane at `origin/main` `3b499aa`; the Manager (single writer) registers ownership before implementation.

## 6. Migration / save implications

Default: none (`CurrentVersion` neither read nor assumed). Conditional additions are listed in §2.6 and are gated by `OPEN-6`/`OPEN-7` and G5. In all cases: missing field means no state, old saves never gain fabricated history or provenance, Legacy40 and Reference Future v1 are unaffected, and the world-hash compatibility path must be preserved so protected anchors do not move.

## 7. Verification strategy

- **Implementation correctness (AgentWorkflow Tier 1 during work, Tier 2 at hand-off):**
  - offline pure-logic suites on plain .NET in the existing style (`Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs`, `ScenarioOneNonUi/OfflineLogic/RackBayOfflineCheck.cs`) for the cycle-state class, gating decision, guide monotonicity and the event-ID check;
  - the N3 offline suite (`ScenarioOneCompletion/N3/run_n3_offline.py`) must stay green **with its pinned normalised hash unchanged**;
  - one Unity multi-cycle witness (three cycles, idle years between, a stale pending order, save/load at each stage) extending `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs`;
  - narrow regression: MenuTutorial, TeachingCopy, RackLesson, ScenarioOneProgress, ScenarioOneCompletion;
  - rendered check at 1280×720 and 1920×1080 for any new Work Plan or HUD element.
- **Spec conformance:** `checklists/requirements.md`, one evidence line per FR/AC; independent reviewer (not the implementing thread) per AgentWorkflow for save and shared-load changes.
- **Verification tier:** Tier 2. Tier 3 only at the integrated pre-demo candidate (D-060 sequence), or earlier only on a named trigger: a failed gate, a threat to a protected anchor/save/determinism contract, or Manager approval. G3 applies to any proposed change to the N3 harness or a pinned hash; G5 applies to schema and completion-policy changes.

## 8. Risks

1. **Baseline drift.** Rack V2, MFM and S1-C are rewriting the same lesson-sequence files; §4 facts may be stale by dispatch. Mitigation: T-01 re-verification, no line-number reliance.
2. **Unintegrated, partly cloud-verified baseline** (spec L-1/L-2). Mitigation: `OPEN-1` before planning.
3. **Teaching/completion coupling.** The guide reads `ReviewedTwoCycles`; a guide change must not alter the completion rule. Mitigation: N3 hash gate.
4. **Footprint cap intent.** Treating F-1 as a bug could remove a safeguard the Manager wants; treating it as a feature could trap players. Mitigation: `OPEN-5`.
5. **Persisting review choices.** `[J]/[K]` persistence interacts with MFM §15. Mitigation: `OPEN-7` and `OPEN-11`.
6. **Terminal failure under repeated spending** (F-11). Out of scope to change economy; flagged for playtest.
7. **Load-path change.** `FR-E3` touches shared validation. Mitigation: fixture scan, CanLoad/LoadData parity gate, G5.

## 9. Implementation sequence

Each step maps to tasks in `tasks.md`.

1. Manager decisions: `OPEN-1`, then `OPEN-5`, `OPEN-7`, `OPEN-3`, `OPEN-4`, `OPEN-6`, `OPEN-8..11` (T-00).
2. Re-verify the baseline and F-1..F-12 at the implementation base (T-01).
3. Pure cycle-state class and offline suite (T-02).
4. Decouple teaching safeguards from Approve/Advance (T-03).
5. Guide monotonicity (T-04).
6. Previous-versus-proposed presentation (T-05), coordinated with S1-C.
7. Event-ID integrity (T-06).
8. Conditional persistence (T-07).
9. Multi-cycle witness, narrow regression, compatibility and anchor check (T-08).
10. Independent spec-conformance review and hand-off (T-09).
