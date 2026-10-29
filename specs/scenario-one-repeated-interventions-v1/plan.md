# Scenario One Second and Subsequent Interventions (S1-D) Implementation Plan

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** APPROVED (Manager decisions in `MANAGER-REVIEW.md` @ `457461b`; amendment packet `7716201`; D-066 persistence amendment 2026-10-29). This plan implements the spec; it creates no product requirement. **Nothing is dispatchable until T-01 has passed on the integrated base** (`tasks.md`).
- **Plan owner:** Overall Manager
- **Created:** 2026-10-27
- **Last updated:** 2026-10-29

## 1. Architecture / current-state findings

Everything below is `[CODE c6342f7]` — static reading of Git objects, not run in Unity by the spec author, not on `main`, and, per RD-1, **partial implementation evidence, not canonical completion** (spec §3). The full register is spec §4; this plan restates only what shapes the approach.

**The loop already works mechanically.** Marks, Work Plan, pruning, planting, clearance and salvage create ordinary orders with no cycle-count assumption (R-1, R-2). History is a pure function of persisted records and treats every resolved plan alike (R-4). Stale orders are surfaced and revalidated (R-3). Completion is existential and monotone (F-9).

**The first-cycle residue is in the teaching layer and a few gaps**, not in the simulation:

| Layer | Residue | Spec | Decision |
|---|---|---|---|
| Approve/Advance gating (UI) | `RackPlanProblem` is a lifetime, game-wide block driven by a cumulative teaching-footprint basal-area share | F-1 | RD-5 |
| Teaching guide (`ScenarioOneTeachingSequence.Current`) | Non-monotone projection of the *current* world; regresses on Crop Tree count ≠ 3, further in-footprint felling, executed-share drift, and every load (session-only `ScenarioOneRackReview`) | F-2..F-4 | RD-7 + D-066 |
| Review chronology | Only the first acknowledgement gates play | F-5 | RD-3 |
| Readiness | Text hints only | F-6 | RD-4 |
| Provenance | Marks carry no time; no dated past-work view | F-7 | RD-6 |
| Device state | Practice steps in `PlayerPrefs` | F-8 | RD-7 |
| Integrity | Completion chronology assumes unique positive event IDs; validators do not check | F-9 | RD-12 |

**Economy and outcome are already cycle-aware and stay untouched** (F-10, F-11). The N3 harness pins the normalised `ScenarioOneCompletionRules` source, so that file is out of bounds.

## 2. Proposed technical approach

Principle: **derived-first, with one narrowly additive durable teaching-history exception.** Current recommendation/readiness displays remain derived from authoritative records. D-066 adds only the minimum aggregate historical record required to make the teaching-sample/review completion boundary survive save/load. It does not revive a broad mutable management-cycle state blob, and individual `[J]/[K]` choices remain non-persistent.

### 2.1 Decouple the teaching cap from commitment (FR-A3, RD-5, T-03)
Make `RackPlanProblem()` hard only while the owning lesson/sample activity is active, and an advisory warning afterwards. "Activity complete" uses the **same durable aggregate teaching-history completion condition** as the guide latch (2.2), so there is one definition of "this teaching sample is done". UI-layer change in `LearningObjectivesView.Sequence.cs` plus the call sites (`ScenarioOneUiRoot.ApproveWorkPlan`/advance path, `WorkPlanView`). No simulation coefficient changes. Prescription ceilings owned by MFM are untouched. T-03 must also check the stuck-state edge in spec §17 (activity never completable while the hard constraint stays on).

### 2.2 Make the guide monotone with minimum durable aggregate history (FR-B1, FR-B3, D-066, T-04)
Current world/history remains the source for recomputable guide facts. For the specific release/review teaching boundary that cannot be reconstructed faithfully after save/load, persist the **minimum aggregate completion evidence per teaching/management cycle**: a stable cycle key plus sample/review completion and only the aggregate counters/outcomes needed to distinguish incomplete from completed progression. Do **not** persist individual `[J]/[K]` choices, inspected-tree choices, or a general mutable cycle-state snapshot.

The guide may then take the maximum of its current derived result and this durable historical completion floor. Before the aggregate record establishes completion, existing in-lesson behaviour is unchanged. Repeated reads/calls must not consume another sample. The new record is append-only/monotone historical evidence, not a second history database and not an advisory-state cache.

### 2.3 Advisory cycle state (FR-D1..D3, RD-3, RD-4, T-02)
Add one pure, `UnityEngine`-free static class (working name `ScenarioCycleState`) beside the history code, taking the persisted lists plus `Outcome`/year and returning **named components with reasons**: approved work awaiting execution; pending work including carried-over; stale/invalid orders; latest resolved intervention reviewed or not; years since the latest resolved intervention (descriptive only). No single yes/no, no gate, no interval. It is the S1-D **cycle-state adapter** of spec §9.4: read-only, holding no mutable/advisory state, deriving "reviewed" from the existing `AnnualReviewAcknowledged` events (or an S1-C accessor if one exists). Output is labelled L3 (derived). D-066 does not require this advisory state itself to be persisted.

### 2.4 Previous versus proposed presentation (FR-C, RD-6, T-05)
Reuse S1-C's history surface (year-browsable review, intervention record, waypoints) and the existing `StaleOrders`. Add only what S1-C does not provide: Work Plan grouping of open orders as *Approved*, *Pending (new)*, *Pending (carried over)* using `createdYear` and the existing stale rule, and links into the S1-C record. **No** designation-year field, **no** dated map layer, **no** duplicate history UI. Copy states no Crop Tree designation time.

### 2.5 Reject ambiguous event IDs before mutation (FR-E3, RD-12, T-06)
Add a pre-mutation check that management-event IDs are positive and unique. It rejects the save with a clear validation error; it **does not repair, renumber or reinterpret**. Scan every shipped/accepted fixture first; if any is ambiguous, STOP and return to the Manager. Touches the shared load path (`ForestSaveValidation*`), so it needs CanLoad/LoadData parity verification and an independent reviewer. No additional schema field beyond the D-066 teaching-history slice.

### 2.6 Persistence — narrow D-066 prerequisite; broad T-07 remains cancelled
D-066 reopens persistence **only** for the minimum aggregate historical teaching-sample/review-complete evidence required by FR-B1/AC-3. This is a bounded prerequisite before the T-04 guide candidate is replayed.

On the exact post-MFM-07 qualified umbrella `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, `ForestSaveData.CurrentVersion = 23`. If live ownership and source inspection still show v23 and no competing save writer at task start, the prerequisite is expected to use **v24**. If either fact changed, STOP and return to the Manager; do not reuse a consumed version.

Pre-amendment saves must remain loadable. Absence of the new record means **no durable teaching completion is recorded**; the loader must not fabricate completed samples from ambiguous old state. New-version malformed or ambiguous teaching-history data must be rejected before world/disk mutation. Save → load → continue must preserve the aggregate completion exactly.

**T-07 remains cancelled.** No broad `ScenarioOneManagementCycleState` (or equivalent mutable cycle-truth blob) is authorised.

### 2.7 Rejected or deferred
- **Re-gating every advance on a modal review** — rejected (RD-3).
- **A second history database, identity scheme, or broad mutable cycle state machine; duplicated history UI** — rejected (RD-1, FR-F2 as amended).
- **Persisting `PlayerPrefs` practice credits or individual `[J]/[K]` decisions into the save** — rejected (RD-7, D-066).
- **A dated past-work map layer; Crop Tree designation year** — rejected for V1 (RD-6).
- **Repairing/renumbering ambiguous event IDs** — rejected (RD-12).
- **Repeating the Objective 1 rack workflow per cycle; deciding rack reopening** — out of scope (RD-9).
- **A second MFM prescription; new per-cycle objectives; a fixed interval** — out of scope (RD-11, RD-2, RD-4).

## 3. Integration boundaries

- **May touch (provisional):** `LearningObjectivesView.Sequence.cs`, `LearningObjectivesView.Curriculum.cs` (new IDs only), `ScenarioOneTeachingSequence.cs`, `ScenarioOneUiRoot.cs`, `WorkPlanView.cs`, `AnnualReviewView.cs` (only via the S1-C surface/owner), `ScenarioOneManager.History.cs`, the new pure cycle-state class, `ForestSaveValidation*.cs` (FR-E3 and D-066 validation only), `ForestSaveData.cs` / the minimum Scenario One save adapter for the D-066 aggregate record only, new verification under `Tools/Verification/`.
- **Must not touch:** `ScenarioOneCompletionRules.cs` and the eight objective predicates/targets (`ScenarioOneObjectives.cs`); economy/work-economy values; ecology, growth, regeneration, mortality; Reference Future v1 and its archive; `Racks/*`, `ScenarioOneManager.RackPlan.cs`, `ScenarioOneRackLayout.cs`, rack lifecycle (Rack V2 lanes — **CROSS-LANE API REQUIRED**); S1-C-owned history surfaces and files except through an agreed API; MFM files; scenes, prefabs, USS/UXML, ProjectSettings, packages; any persistence beyond the D-066 aggregate teaching-history slice.
- **Contracts used:** the S1-C consumed contract (spec §9); stable saved IDs; the pre-mutation save-validation contract; MFM-06/07 prescription/provenance save compatibility; protected Reference Future and geometry anchors; `CompetitionShare` untouched.

## 4. Affected systems / files (provisional; reconfirm in T-01 and at the D-066 prerequisite start)

`Assets/ForestPrototype/UI/` — `LearningObjectivesView.Sequence.cs`, `.Curriculum.cs`, `ScenarioOneTeachingSequence.cs`, `ScenarioOneUiRoot.cs`, `WorkPlanView.cs`, `ActiveObjectiveHudWidget.cs`. `Assets/ForestPrototype/ScenarioOne/` — `ScenarioOneManager.History.cs`, the new cycle-state class, `ScenarioOneRackBay.cs` only if the cap's lesson-scoping must live inside the analysis, and the smallest save capture/restore adapter needed for D-066. `Assets/ForestPrototype/ForestSaveData.cs`, `ForestSaveController.cs`, `ForestSaveValidation*.cs` only within the bounded persistence/validation scope. `Tools/Verification/` — new save-history and multi-cycle gates.

## 5. Ownership / concurrency and sequencing

**Current amended sequence (D-066 / browser claim #136):**

1. S1-D T-01 baseline re-verification — complete/accepted on the current programme lineage.
2. T-02/T-03 accepted slices remain separate from this amendment.
3. **Claim #143:** implement and qualify the minimum D-066 aggregate teaching-history persistence prerequisite from the exact qualified post-MFM-07 production umbrella, subject to live lock/schema refresh.
4. Rebase/replay the existing #124 T-04 guide-monotonicity candidate against the integrated #143 prerequisite. `b9d204c` is unqualified evidence only and must not be stacked blindly.
5. Complete #124 offline + Unity qualification and Manager review.
6. Only then re-release PARITY-08 #126 on the resulting exact integrated lineage; #126 must not be a concurrent save writer.
7. #128 remains the mandatory independent PARITY-08 review after #126 hands off.
8. T-05 coordinates with the S1-C UI owner and reuses its history surface.
9. T-07 remains cancelled for broad cycle-state persistence.
10. T-08/T-09 follow the amended dependencies.

**Hot files with other writers:** S1-C history/review surfaces, MFM, Rack V2 and PARITY-08. Parallel writers on one file set are not permitted (AgentWorkflow). The Manager registers ownership in `active-tasks.md` before each lane starts. #143 has no production ownership while `[PREPARED]`.

**Unity:** any Unity gate runs in a Unity-capable lane under the machine-wide Unity lock; parallel Unity is never permitted. This browser amendment runs no Unity.

## 6. Migration / save implications

The D-066 prerequisite is the only new S1-D persisted state authorised for V1. The post-MFM-07 qualified production base is schema v23; expected next version is v24 only after the worker rechecks live ownership and source immediately before editing.

For saves written before the amendment, missing teaching-history aggregate data means **not recorded / incomplete for the durable latch**. Do not backfill a completed sample from session state, `PlayerPrefs`, current geometry, marks or a heuristic reconstruction. Existing management history, objective achievements, MFM-06/07 prescription/provenance state and older compatibility paths remain authoritative and unchanged.

For saves at the new version, the teaching-history aggregate must be internally valid, monotone and unambiguous. Malformed or ambiguous new records are rejected before mutation; they are not silently repaired. Reference Future v1, Legacy40 and pre-v20 compatibility must not regress. Individual `[J]/[K]` choices remain session-only.

## 7. Verification strategy

- **Implementation correctness (AgentWorkflow Tier 1 during work, Tier 2 at hand-off):**
  - offline pure-logic suites on plain .NET in the existing style (`Tools/Verification/ThreeSpecies/OfflineLogic/ScenarioHistoryOfflineCheck.cs`, `ScenarioOneNonUi/OfflineLogic/RackBayOfflineCheck.cs`) for the D-066 aggregate record, cycle-state class, the gating decision, guide monotonicity and the event-ID check;
  - the N3 offline suite (`ScenarioOneCompletion/N3/run_n3_offline.py`) must stay green **with its pinned normalised hash unchanged**;
  - focused D-066 save → load → continue verification proving incomplete vs completed aggregate teaching state, repeated-call non-consumption, pre-amendment-save defaults, malformed-record rejection and preservation of MFM-06/07 save/provenance;
  - one Unity multi-cycle witness (three cycles, idle years between, a stale pending order, save/load at each stage, completion, then continued management) extending `Tools/Verification/ThreeSpecies/ScenarioHistoryVerification.cs`;
  - narrow regression: MenuTutorial, TeachingCopy, RackLesson, ScenarioOneProgress, ScenarioOneCompletion, SaveHardening;
  - rendered check at 1280×720 and 1920×1080 for any new Work Plan or HUD element.
- **Spec conformance:** `checklists/requirements.md`, one evidence line per FR/AC/RD; independent reviewer (not the implementing thread) for the load-path change and the guide change.
- **Verification tier:** Tier 2. Tier 3 only at the integrated pre-demo candidate (D-060 sequence), or earlier on a named trigger: a failed gate, a threat to a protected anchor/save/determinism contract, or Manager approval. G3 applies to any change to the N3 harness or a pinned hash; G5 applies to the load-path change and any schema or completion-policy change.

## 8. Risks

1. **Baseline drift.** Rack V2, MFM, S1-C and parity work can move nearby save/lesson files; recheck immediately before edits.
2. **Unintegrated or stale candidate evidence.** `b9d204c` is not qualified/integrated and cannot define the new base.
3. **Two history authorities.** The D-066 record must be a narrow append-only teaching-completion record, not a duplicate intervention/history database or broad cycle snapshot.
4. **Teaching/completion coupling.** The guide reads `ReviewedTwoCycles`; a guide change must not alter the completion rule. Mitigation: N3 hash gate.
5. **Over-claiming old saves.** Absence of D-066 data must not be interpreted as historical completion. Mitigation: explicit pre-amendment default plus save fixtures.
6. **Cap stuck-state.** The retained in-activity hard constraint could trap a player who never completes the sample lesson. Mitigation: T-03/shared completion definition verification.
7. **Terminal failure under repeated spending** (F-11). Economy is out of scope; flagged for playtest.
8. **Load/schema conflict.** Another writer or schema bump invalidates expected v24. Mitigation: live refresh + STOP, no version reuse.
9. **Malformed new history.** Mitigation: pre-mutation validation, no silent repair, CanLoad/LoadData parity.

## 9. Implementation sequence

Each step maps to tasks in `tasks.md`.

1. Manager decisions — **complete** (T-00 plus D-066 amendment).
2. Re-verify the baseline and F-1..F-12 on the integrated base (T-01) — **complete/accepted for the current programme lineage**; the #143 worker still refreshes live save/schema/ownership before editing.
3. Advisory cycle-state adapter and offline suite (T-02).
4. Decouple the teaching cap from Approve/Advance (T-03).
5. **D-066 persistence prerequisite (#143):** minimum aggregate teaching sample/review completion, schema-safe save/load/validation, focused qualification.
6. **T-04 (#124):** replay/rebase guide monotonicity only after #143 is integrated; then qualify save/load AC-3 against the durable aggregate record.
7. Previous-versus-proposed presentation over the S1-C surface (T-05).
8. Reject ambiguous event IDs pre-mutation (T-06), sequenced so there is one save writer.
9. ~~Broad persisted cycle-state expansion~~ remains cancelled (T-07).
10. Multi-cycle witness, narrow regression, compatibility and anchor check (T-08).
11. Independent spec-conformance review and hand-off (T-09).