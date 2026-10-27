# Scenario One Second and Subsequent Interventions (S1-D) Implementation Plan

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** APPROVED (Manager decisions in `MANAGER-REVIEW.md` @ `457461b`; amendment packet `7716201`). This plan implements the spec; it creates no product requirement. **Nothing is dispatchable until T-01 has passed on the integrated base** (`tasks.md`).
- **Plan owner:** Overall Manager
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27

## 1. Architecture / current-state findings

Everything below is `[CODE c6342f7]` — static reading of Git objects, not run in Unity by the spec author, not on `main`, and, per RD-1, **partial implementation evidence, not canonical completion** (spec §3). The full register is spec §4; this plan restates only what shapes the approach.

**The loop already works mechanically.** Marks, Work Plan, pruning, planting, clearance and salvage create ordinary orders with no cycle-count assumption (R-1, R-2). History is a pure function of persisted records and treats every resolved plan alike (R-4). Stale orders are surfaced and revalidated (R-3). Completion is existential and monotone (F-9).

**The first-cycle residue is in the teaching layer and a few gaps**, not in the simulation:

| Layer | Residue | Spec | Decision |
|---|---|---|---|
| Approve/Advance gating (UI) | `RackPlanProblem` is a lifetime, game-wide block driven by a cumulative teaching-footprint basal-area share | F-1 | RD-5 |
| Teaching guide (`ScenarioOneTeachingSequence.Current`) | Non-monotone projection of the *current* world; regresses on Crop Tree count ≠ 3, further in-footprint felling, executed-share drift, and every load (session-only `ScenarioOneRackReview`) | F-2..F-4 | RD-7 |
| Review chronology | Only the first acknowledgement gates play | F-5 | RD-3 |
| Readiness | Text hints only | F-6 | RD-4 |
| Provenance | Marks carry no time; no dated past-work view | F-7 | RD-6 |
| Device state | Practice steps in `PlayerPrefs` | F-8 | RD-7 |
| Integrity | Completion chronology assumes unique positive event IDs; validators do not check | F-9 | RD-12 |

**Economy and outcome are already cycle-aware and stay untouched** (F-10, F-11). The N3 harness pins the normalised `ScenarioOneCompletionRules` source, so that file is out of bounds.

## 2. Proposed technical approach

Principle: **derived-first, additive, no new persisted state.** S1-D V1 adds no field (RD-6, RD-7). The save already carries what re-entry needs; the fixes stop the UI contradicting it.

### 2.1 Decouple the teaching cap from commitment (FR-A3, RD-5, T-03)
Make `RackPlanProblem()` hard only while the owning lesson/sample activity is active, and an advisory warning afterwards. "Activity complete" uses the **same persisted-history condition** as the guide latch (2.2), so there is one definition of "this teaching sample is done". UI-layer change in `LearningObjectivesView.Sequence.cs` plus the call sites (`ScenarioOneUiRoot.ApproveWorkPlan`/advance path, `WorkPlanView`). No simulation file changes. Prescription ceilings owned by MFM are untouched. T-03 must also check the stuck-state edge in spec §17 (activity never completable while the hard constraint stays on).

### 2.2 Make the guide monotone from persisted history (FR-B1, FR-B3, RD-7, T-04)
Compute `Current(f)` as today, then take the maximum with a **history-only latch**: lessons whose prerequisite facts cannot un-happen (completed orders and their `resolvedYear`, `AnnualReviewAcknowledged` events by event ID, `PlantJuvenile` successes) hold the guide at or above that lesson. The latch for the release/review lesson uses a durable Annual Review/history fact only where it **faithfully** establishes completion; T-04 proves faithfulness with an offline matrix and reports any state where no persisted fact does, leaving existing behaviour there rather than inventing a latch. Before historical completion the existing in-lesson behaviour (including repeating the session `[J]/[K]` review after load) is unchanged. No new save field, no `[J]/[K]` persistence, no `PlayerPrefs` authority. Step IDs stay stable; new IDs only. Pure and offline-testable.

### 2.3 Advisory cycle state (FR-D1..D3, RD-3, RD-4, T-02)
Add one pure, `UnityEngine`-free static class (working name `ScenarioCycleState`) beside the history code, taking the persisted lists plus `Outcome`/year and returning **named components with reasons**: approved work awaiting execution; pending work including carried-over; stale/invalid orders; latest resolved intervention reviewed or not; years since the latest resolved intervention (descriptive only). No single yes/no, no gate, no interval. It is the S1-D **cycle-state adapter** of spec §9.4: read-only, holding no state, deriving "reviewed" from the existing `AnnualReviewAcknowledged` events (or an S1-C accessor if one exists). Output is labelled L3 (derived).

### 2.4 Previous versus proposed presentation (FR-C, RD-6, T-05)
Reuse S1-C's history surface (year-browsable review, intervention record, waypoints) and the existing `StaleOrders`. Add only what S1-C does not provide: Work Plan grouping of open orders as *Approved*, *Pending (new)*, *Pending (carried over)* using `createdYear` and the existing stale rule, and links into the S1-C record. **No** designation-year field, **no** dated map layer, **no** duplicate history UI. Copy states no Crop Tree designation time.

### 2.5 Reject ambiguous event IDs before mutation (FR-E3, RD-12, T-06)
Add a pre-mutation check that management-event IDs are positive and unique. It rejects the save with a clear validation error; it **does not repair, renumber or reinterpret**. Scan every shipped/accepted fixture first; if any is ambiguous, STOP and return to the Manager. Touches the shared load path (`ForestSaveValidation*`), so it needs CanLoad/LoadData parity verification and an independent reviewer. No schema field.

### 2.6 Persistence — cancelled for V1
No persisted state is added (RD-6, RD-7). T-07 is cancelled. A later explicit Manager decision would be required to reopen it, and any such work would take the next available schema version on the actual integration base.

### 2.7 Rejected or deferred
- **Re-gating every advance on a modal review** — rejected (RD-3).
- **A second history database, identity scheme, or cycle state machine; duplicated history UI** — rejected (RD-1, §9).
- **Persisting `PlayerPrefs` practice credits or `[J]/[K]` decisions into the save** — rejected (RD-7).
- **A dated past-work map layer; Crop Tree designation year** — rejected for V1 (RD-6).
- **Repairing/renumbering ambiguous event IDs** — rejected (RD-12).
- **Repeating the Objective 1 rack workflow per cycle; deciding rack reopening** — out of scope (RD-9).
- **A second MFM prescription; new per-cycle objectives; a fixed interval** — out of scope (RD-11, RD-2, RD-4).

## 3. Integration boundaries

- **May touch (provisional):** `LearningObjectivesView.Sequence.cs`, `LearningObjectivesView.Curriculum.cs` (new IDs only), `ScenarioOneTeachingSequence.cs`, `ScenarioOneUiRoot.cs`, `WorkPlanView.cs`, `AnnualReviewView.cs` (only via the S1-C surface/owner), `ScenarioOneManager.History.cs`, the new pure cycle-state class, `ForestSaveValidation*.cs` (FR-E3 only), new verification under `Tools/Verification/`.
- **Must not touch:** `ScenarioOneCompletionRules.cs` and the eight objective predicates/targets (`ScenarioOneObjectives.cs`); economy/work-economy values; ecology, growth, regeneration, mortality; Reference Future v1 and its archive; `Racks/*`, `ScenarioOneManager.RackPlan.cs`, `ScenarioOneRackLayout.cs`, rack lifecycle (Rack V2 lanes — **CROSS-LANE API REQUIRED**); S1-C-owned history surfaces and files; MFM files; scenes, prefabs, USS/UXML, ProjectSettings, packages; `ForestSaveData` (no new field).
- **Contracts used:** the S1-C consumed contract (spec §9); stable saved IDs; the pre-mutation save-validation contract; `CompetitionShare` untouched.

## 4. Affected systems / files (provisional; reconfirm in T-01)

`Assets/ForestPrototype/UI/` — `LearningObjectivesView.Sequence.cs`, `.Curriculum.cs`, `ScenarioOneTeachingSequence.cs`, `ScenarioOneUiRoot.cs`, `WorkPlanView.cs`, `ActiveObjectiveHudWidget.cs`. `Assets/ForestPrototype/ScenarioOne/` — `ScenarioOneManager.History.cs`, the new cycle-state class, `ScenarioOneRackBay.cs` only if the cap's lesson-scoping must live inside the analysis. `Assets/ForestPrototype/ForestSaveValidation*.cs` (FR-E3). `Tools/Verification/` — new multi-cycle gate and offline suite.

## 5. Ownership / concurrency and sequencing

**Sequencing (Manager, `MANAGER-REVIEW.md` "Sequencing / implementation base"):**

1. Finish and integrate the active Rack V2 core work.
2. Complete the S1-C amendment so its contract is `APPROVED`.
3. Amend/approve S1-D (this package).
4. Run **T-01** against the **actual integrated Rack V2 + approved S1-C contract base** before any production coding.
5. If T-01 passes, **T-02, T-03 and T-06** may be prepared as separate non-overlapping implementation lanes, subject to live ownership.
6. **T-04** (guide monotonicity) should land **before** the later Multi-Scale curriculum insertion when both touch the same guide/current-state files.
7. **T-05** coordinates with the S1-C UI owner and reuses its history surface.
8. **T-07** is cancelled for V1.

**Hot files with other writers:** Rack V2 03C (rack runtime and "narrow rack lifecycle lesson-sequence changes if required"), 02B-R (headland), MFM (adds a lesson after Lesson 5, so edits the curriculum and `Current()`), S1-C (Annual Review/Work Plan/history views). Parallel writers on one file set are not permitted (AgentWorkflow). The Manager registers ownership in `active-tasks.md` (single writer) before each lane starts; no S1-D row exists at `origin/main` `3b499aa`.

**Unity:** any Unity gate runs in a Unity-capable lane under the machine-wide Unity lock; parallel Unity is never permitted. This planning and amendment work ran no Unity.

## 6. Migration / save implications

None for V1: no new field, no schema change, `CurrentVersion` neither read for decisions nor changed. Old saves never gain fabricated history or provenance; Legacy40, Reference Future v1 and pre-feature Enlarged80 saves are unaffected. The only load-path change is the FR-E3 rejection rule. Any future S1-D schema work takes the next available version on the actual integration base and never reuses a version owned by another accepted or qualified feature; no number is fixed here.

## 7. Verification strategy

- **Implementation correctness (AgentWorkflow Tier 1 during work, Tier 2 at hand-off):**
  - offline pure-logic suites on plain .NET in the existing style (`Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs`, `ScenarioOneNonUi/OfflineLogic/RackBayOfflineCheck.cs`) for the cycle-state class, the gating decision, guide monotonicity and the event-ID check;
  - the N3 offline suite (`ScenarioOneCompletion/N3/run_n3_offline.py`) must stay green **with its pinned normalised hash unchanged**;
  - one Unity multi-cycle witness (three cycles, idle years between, a stale pending order, save/load at each stage, completion, then continued management) extending `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs`;
  - narrow regression: MenuTutorial, TeachingCopy, RackLesson, ScenarioOneProgress, ScenarioOneCompletion, SaveHardening;
  - rendered check at 1280×720 and 1920×1080 for any new Work Plan or HUD element.
- **Spec conformance:** `checklists/requirements.md`, one evidence line per FR/AC/RD; independent reviewer (not the implementing thread) for the load-path change and the guide change.
- **Verification tier:** Tier 2. Tier 3 only at the integrated pre-demo candidate (D-060 sequence), or earlier on a named trigger: a failed gate, a threat to a protected anchor/save/determinism contract, or Manager approval. G3 applies to any change to the N3 harness or a pinned hash; G5 applies to the load-path change and any schema or completion-policy change.

## 8. Risks

1. **Baseline drift.** Rack V2, MFM and S1-C are rewriting the same lesson-sequence files; §4 facts may be stale by dispatch. Mitigation: mandatory T-01, no line-number reliance.
2. **Unintegrated, partly cloud-verified baseline** (spec L-1, L-2). Mitigation: RD-1 treats it as partial evidence; T-01 re-verifies.
3. **Two derived-history implementations.** S1-C inspected `main` and does not mention the candidate lineage's history code. Mitigation: spec §9.4(a), AC-11, stop condition.
4. **Teaching/completion coupling.** The guide reads `ReviewedTwoCycles`; a guide change must not alter the completion rule. Mitigation: N3 hash gate.
5. **Faithful latch.** A latch that over-claims lesson completion would hide a lesson from a player who has not done it. Mitigation: T-04 offline matrix; fall back to existing behaviour where no persisted fact is faithful.
6. **Cap stuck-state.** The retained in-activity hard constraint could trap a player who never completes the sample lesson. Mitigation: T-03 verifies and stops if reachable.
7. **Terminal failure under repeated spending** (F-11). Economy is out of scope; flagged for playtest.
8. **Load-path change.** FR-E3 touches shared validation. Mitigation: fixture scan, parity gate, independent review, STOP on ambiguous fixtures.

## 9. Implementation sequence

Each step maps to tasks in `tasks.md`; none starts before T-01 passes.

1. Manager decisions — **complete** (T-00).
2. Re-verify the baseline and F-1..F-12 on the integrated base (T-01) — mandatory first.
3. Advisory cycle-state adapter and offline suite (T-02).
4. Decouple the teaching cap from Approve/Advance (T-03).
5. Guide monotonicity (T-04), before any MFM curriculum insertion that touches the same files.
6. Previous-versus-proposed presentation over the S1-C surface (T-05).
7. Reject ambiguous event IDs pre-mutation (T-06).
8. ~~Conditional persistence~~ cancelled (T-07).
9. Multi-cycle witness, narrow regression, compatibility and anchor check (T-08).
10. Independent spec-conformance review and hand-off (T-09).
