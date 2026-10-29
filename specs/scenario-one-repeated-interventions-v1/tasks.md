# Scenario One Second and Subsequent Interventions (S1-D) Bounded Tasks

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** **APPROVED** — implementation is authorised within each task's stated scope and dependencies only. T-01 remains the mandatory baseline re-verification task for this spec; the 2026-10-29 D-066 amendment adds one bounded persistence prerequisite before T-04 can resume.
- **Task list owner:** Overall Manager; drafted by the S1-D Primary specification author and amended by browser claim #136
- **Last updated:** 2026-10-29
- **Decision authority:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/MANAGER-REVIEW.md` @ `457461b9b92da2e96101f5b0eb9940b4e92fec0a`; amendment `AMENDMENT-PACKET.md` @ `771620113487894bd8e09c69309318cc603ae063`; resolved decisions RD-1…RD-12 in spec §12; D-066 minimal durable aggregate teaching-history persistence.

Tasks are implementation units, not new product decisions. Each task stays inside the approved spec and plan. If a task exposes a material product ambiguity, stop and return it to the Manager/spec. There are no `OPEN-n` blockers: every product decision is recorded as RD-1…RD-12 or D-066.

Common exclusions for every task: no change to `ScenarioOneCompletionRules.cs`, the eight objective predicates or targets, economy/ecology values, Reference Future v1, scenes/prefabs/USS/UXML/ProjectSettings/packages, or Rack V2 lane-owned files. No Windows build. **The only new persisted S1-D V1 state authorised is the D-066 minimum aggregate teaching-sample/review-completion history; no broad cycle-state blob and no individual `[J]/[K]` persistence.** No fixed intervention interval, no automatic treatment choice, no hidden automation replacing player judgement. Disposable verification scripts follow the `Tools/Verification/` copy/run/remove workflow in AGENTS.md.

## T-00 — Resolve the product decisions

- **Task ID:** T-00
- **Status:** **COMPLETE** — original Manager decisions plus D-066 are recorded in canonical authority.
- **Goal:** Decide the original `OPEN-1` through `OPEN-11`, the management-event-ID question, and the later persistence ambiguity exposed by T-04.
- **Scope:** Decisions and canonical-document updates only.
- **Exclusions:** Any code.
- **Owner / routing:** Overall Manager / user.
- **Dependencies:** —
- **Verification:** `MANAGER-REVIEW.md` records the original decisions; D-066 records the accepted persistence amendment; no unresolved product decision remains for T-04.
- **Stop conditions:** None remaining.
- **Completion state:** **Achieved.**

## T-01 — Re-verify the baseline and the assumption register

- **Task ID:** T-01
- **Status:** **COMPLETE / MANAGER-ACCEPTED on the current programme lineage.** The D-066 prerequisite still refreshes the exact save/schema/ownership state immediately before production edits.
- **Goal:** Confirm at the actual integrated base that F-1..F-12 and R-1..R-5 still hold, replace every `[CODE c6342f7]` with the real SHA, and report the S1-C reconciliation findings of spec §9.4.
- **Scope:** Read-only inspection; run the existing offline suites (N3, ScenarioHistory offline, RackBay offline); no production change.
- **Exclusions:** Any edit.
- **Owner / routing:** Reviewer-capable worker with repository tools.
- **Dependencies:** T-00 complete; Rack V2 core integrated; S1-C contract approved.
- **Verification:** A table of finding → still true / changed / new, with SHA and command output; N3 normalised-source hash reproduced unchanged; exactly one derived-history implementation confirmed; grouping unit and schema ordering reported per spec §9.4.
- **Stop conditions:** A finding is no longer true or a new blocker appears → return to Manager; a second derived-history implementation, competing store or duplicated history UI exists → STOP; do not silently adapt.
- **Completion state:** Accepted baseline finding handoff.

## T-02 — Pure derived cycle state and readiness

- **Task ID:** T-02
- **Status:** **AUTHORISED after T-01.**
- **Goal:** FR-D1..D3, FR-F1. A `UnityEngine`-free static class (working name `ScenarioCycleState`) returning named, advisory cycle components and reasons from persisted authoritative records.
- **Scope:** New class beside `ScenarioInterventionHistory`; read accessor on `ScenarioOneManager.History.cs`; offline suite in the Phase 7–9 style including the per-year acknowledgement derivation (EI-5).
- **Exclusions:** Persisting advisory/readiness state; any blocking behaviour; any single prescriptive yes/no; any wording that instructs the player to intervene or states an interval as universal. The D-066 teaching-history record is not an advisory cache.
- **Owner / routing:** TBD (code-only capable).
- **Dependencies:** T-01
- **Verification:** Offline checks: every state yields an explanation (no empty hint); fourth cycle treated like the first; idle years add no intervention; an outstanding later review acknowledgement never gates Approve/Advance; rebuild is non-mutating and identical on a field-for-field copy (AC-4, AC-9).
- **Stop conditions:** The advisory state itself would need persistence → stop; D-066 does not authorise that.
- **Completion state:** Offline suite green; checklist FR-D and FR-F1 evidenced.

## T-03 — Decouple the teaching footprint cap from Approve and Advance

- **Task ID:** T-03
- **Status:** **AUTHORISED after T-01; sequencing with the shared D-066 completion record must be respected.**
- **Goal:** FR-A3, AC-2. The 40 % cumulative teaching-footprint rule is hard **only while its owning lesson/sample activity is active**; after that activity is historically complete it is at most an advisory warning and cannot disable Approve Work Plan or Advance.
- **Scope:** `LearningObjectivesView.RackPlanProblem()` and its call sites in `ScenarioOneUiRoot` and `WorkPlanView`; the activity-complete condition reuses the same teaching-sample completion semantics as FR-B1.
- **Exclusions:** Changing `MaxRemovedBasalShare`, `CompetitionShare`, the sample lesson's own teaching behaviour during the lesson, or MFM's prescription ceiling. No lifetime hard block.
- **Owner / routing:** TBD (Unity-capable for the gate).
- **Dependencies:** T-01; if implementation requires the durable completion fact, consume the #143 API rather than create a second store.
- **Verification:** Offline decision table (lesson active/closed × cap exceeded × cycle); Unity: cycle-2 thinning in the footprint approves and advances; during the lesson the original safeguard still applies; message names rule, footprint, quantity, remedy. Narrow regression: RackLesson, MenuTutorial, TeachingCopy.
- **Stop conditions:** The fix needs to change `ScenarioOneRackBay` analysis semantics beyond the call-site predicate, or edit rack lifecycle files → stop (cross-lane); the retained in-activity constraint can trap a player who can never complete the owning activity → stop (spec §17).
- **Completion state:** AC-2 evidenced; no change to completion or protected anchors.

## T-04P — Minimum durable teaching-history persistence prerequisite

- **Task ID:** T-04P
- **Claim:** #143
- **Status:** **PREPARED — next executable local persistence task after browser claim #136 publishes the packet; no ownership until the worker marks #143 RUNNING and refreshes live coordination.**
- **Goal:** D-066, FR-B1/FR-F2, AC-3 save/load portion. Add the smallest durable aggregate historical record needed to distinguish incomplete from completed teaching sample/review progression across save/load.
- **Scope:** A stable cycle/teaching-cycle key plus sample/review completion and only the aggregate counters/outcomes required to establish that completion. Append-only/monotone historical evidence. Save capture/restore and pre-mutation validation. Focused deterministic + Unity save→load→continue verification.
- **Exclusions:** Individual `[J]/[K]` choices, inspected-tree choices, `PlayerPrefs`, broad `ScenarioOneManagementCycleState`, guide/curriculum replay, T-05 UI, PARITY-08, MFM/Rack behaviour, completion-rule changes, coefficient changes.
- **Owner / routing:** OpenAI Sol/Codex local lane preferred for the bounded save/validation slice; one save writer only.
- **Production base:** exact post-MFM-07 qualified umbrella `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, currently `ForestSaveData.CurrentVersion = 23`.
- **Schema expectation:** v24 **only if** live task-start inspection still shows v23 and no competing save writer. Otherwise STOP and return to Manager.
- **Backward compatibility:** v23 and earlier lack the record → no durable teaching completion is recorded; do not fabricate completion. Preserve Legacy40, pre-v20 paths, frozen Reference Future v1, MFM-06/07 prescription/provenance.
- **Malformed-data rule:** ambiguous or malformed new-version teaching-history data is rejected before world/disk mutation; no silent repair.
- **Verification:** offline roundtrip/validation matrix; Unity import/compile; focused save→load→continue for incomplete and completed sample states; repeated reads/calls do not consume another sample; v23/pre-amendment fixtures load; negative v24 fixtures reject before mutation; protected reference/hash gates remain unchanged where applicable.
- **Stop conditions:** broader persistence required; another save writer appears; `CurrentVersion` is no longer 23 at start; exact production umbrella changed without a reviewed rebase; MFM-06/07 save/provenance or Reference Future compatibility would need reinterpretation.
- **Completion state:** GitHub `HANDOFF.md` with exact production SHA, schema, test evidence and no residual ownership.

## T-04 — Guide monotonicity

- **Task ID:** T-04
- **Claim:** #124
- **Status:** **BLOCKED on T-04P/#143 integration.** Existing `b9d204c` is unqualified evidence only.
- **Goal:** FR-B1, FR-B2, AC-3. The guide does not regress on Crop Tree count changes, later in-footprint felling, executed-share drift or load.
- **Scope:** Rebase/replay the bounded guide/curriculum change after the T-04P aggregate-history prerequisite is integrated. Use the durable aggregate completion fact for the historical teaching boundary; current advisory/recommendation display remains derived. New IDs only.
- **Exclusions:** Renaming or reusing any step/lesson ID; touching `ScenarioOneCompletionRules`; creating another persisted state; `PlayerPrefs` as progression authority; persisting individual `[J]/[K]`; stacking `b9d204c` blindly without replay/review.
- **Owner / routing:** Existing #124 curriculum writer when re-released; sole writer of the curriculum files.
- **Dependencies:** T-01; **T-04P/#143 integrated and qualified**; MFM sequencing/ownership.
- **Verification:** Offline matrix over regression triggers and lessons 1–8; Unity save at relevant later lessons, reload, guide index unchanged; before durable completion the existing in-lesson behaviour remains; repeated calls in one state do not consume another sample. Teaching copy verification must still pass.
- **Stop conditions:** Aggregate-history API is insufficient or ambiguous → return to Manager/#143 owner; MFM rewrites `Current()` first → rebase task; any broader persistence request → stop.
- **Completion state:** AC-3 evidenced; step-ID diff shows no rename/reuse; Manager review accepts replayed exact candidate.

## T-05 — Previous versus proposed presentation

- **Task ID:** T-05
- **Status:** **AUTHORISED — starts after T-01 and T-02, with S1-C implementation/UI ownership agreed.**
- **Goal:** FR-C1..C5, AC-5.
- **Scope:** Work Plan grouping (Approved / Pending new / Pending carried over); links to the Annual Review year and the S1-C history surface; no designation-time wording; spatial review only through S1-C history/waypoint surfaces where recorded locations already exist.
- **Exclusions:** Any new persisted field, including a Crop Tree designation year; any S1-D dated past-work map layer; automatic cancellation or approval; causal claims; duplicating the S1-C history UI.
- **Owner / routing:** TBD; coordinate with the S1-C writer for `WorkPlanView.cs`/`AnnualReviewView.cs`.
- **Dependencies:** T-01, T-02; S1-C approved contract and UI ownership.
- **Verification:** Rendered check at 1280×720 and 1920×1080 (`ScenarioOneUx`); offline wording checks; Unity: three-cycle fixture shows the five classes distinctly.
- **Stop conditions:** The S1-C design already owns the card → integrate through it, do not duplicate.
- **Completion state:** AC-5 evidenced with captures.

## T-06 — Management-event ID integrity

- **Task ID:** T-06
- **Status:** **AUTHORISED after T-01; independent review required for the shared load-path change. Sequence so T-04P and T-06 never have concurrent save/validator writers.**
- **Goal:** FR-E3, AC-7.
- **Scope:** Pre-mutation validation that **rejects** a save with non-positive or duplicate `ScenarioManagementEvent.eventId`, with a clear validation error. Fixture scan first; CanLoad/LoadData parity.
- **Exclusions:** Repairing, renumbering or reinterpreting historical event identity; any additional schema field; changing the completion rule; relaxing any existing validator.
- **Owner / routing:** TBD; independent reviewer required (shared load path).
- **Dependencies:** T-01; no concurrent T-04P save writer.
- **Verification:** SaveHardening and CanLoad/LoadData parity gates; negative fixtures (zero, duplicate); positive fixtures from every shipped save version; no world or disk mutation on rejection.
- **Stop conditions:** A legitimate shipped or accepted fixture is ambiguous → STOP and return to Manager before changing compatibility policy.
- **Completion state:** AC-7 evidenced; protected anchors unchanged.

## T-07 — Broad persisted cycle-state expansion

- **Task ID:** T-07
- **Status:** **CANCELLED for S1-D V1.** D-066 does **not** reopen it.
- **Goal:** None. Retained as a record of the cancelled broad-state option.
- **Scope:** —
- **Exclusions:** A broad mutable `ScenarioOneManagementCycleState` or equivalent, advisory/readiness cache, individual `[J]/[K]` persistence, or any persistence beyond T-04P.
- **Owner / routing:** —
- **Dependencies:** —
- **Verification:** N/A — no work authorised.
- **Stop conditions:** A proposal to expand persistence beyond D-066 → return to Manager.
- **Completion state:** Cancelled.

## T-08 — Multi-cycle witness and compatibility check

- **Task ID:** T-08
- **Status:** **BLOCKED on the required implementation tasks, including accepted T-04P and T-04.**
- **Goal:** AC-1, AC-4, AC-6, AC-8, AC-12, AC-14, AC-15.
- **Scope:** One Unity gate: three real cycles with idle years, a stale pending order, save/load at each stage, completion on the third and continued management after Completed; D-057 solvent witness re-run; narrow regression set; protected anchor comparison.
- **Exclusions:** Rebaselining any anchor; adding thresholds to the gate to make it pass.
- **Owner / routing:** Unity-capable lane under the machine-wide Unity lock. TBD.
- **Dependencies:** required S1-D implementation tasks complete on one exact lineage, including T-04P/T-04.
- **Verification:** Gate tokens, logs and anchor comparison attached; one stated uncertainty ("does repetition keep completion reachable and solvent?") and stop when answered.
- **Stop conditions:** Completion unreachable or unrecoverable without a coefficient change → Manager (spec §17).
- **Completion state:** Evidence recorded; Tier-2 handoff.

## T-09 — Independent spec-conformance review and hand-off

- **Task ID:** T-09
- **Status:** **BLOCKED on T-08.**
- **Goal:** Close the spec checklist; confirm no hidden automation and no unlabelled coefficient (AC-10, AC-11, AC-13).
- **Scope:** Review only.
- **Exclusions:** Any implementation.
- **Owner / routing:** Independent reviewer (not the implementing thread).
- **Dependencies:** T-08
- **Verification:** `checklists/requirements.md` fully evidenced; verdict APPROVED / NEEDS CHANGES / REJECTED.
- **Stop conditions:** Any mandatory gate unmet.
- **Completion state:** Checklist verdict recorded; spec moved to `VERIFYING`/`VERIFIED` by the Manager.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| T-00 | COMPLETE | Manager / user | — |
| T-01 | COMPLETE / accepted current lineage | Reviewer-capable worker | T-00 + integration base |
| T-02 | AUTHORISED after T-01 | TBD | T-01 |
| T-03 | AUTHORISED after T-01; consume shared completion API | TBD (Unity-capable) | T-01 / #143 API if required |
| **T-04P (#143)** | **PREPARED — next local persistence prerequisite** | OpenAI Sol/Codex local preferred | D-066 + live save/schema refresh |
| **T-04 (#124)** | **BLOCKED** | Existing curriculum writer | T-04P integrated + ownership |
| T-05 | AUTHORISED after T-01/T-02 | TBD + S1-C UI owner | T-01, T-02, S1-C |
| T-06 | AUTHORISED; serialize with T-04P | TBD + independent reviewer | T-01 + free save lane |
| T-07 | **CANCELLED broad state (V1)** | — | — |
| T-08 | BLOCKED | Unity lane | required tasks incl. T-04P/T-04 |
| T-09 | BLOCKED | Independent reviewer | T-08 |