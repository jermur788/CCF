# Scenario One Second and Subsequent Interventions (S1-D) Bounded Tasks

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** **APPROVED** — implementation is authorised within each task's stated scope and dependencies only. **T-01 is the mandatory first execution task** on the actual integrated Rack V2 + approved S1-C contract base; no other task starts before it passes.
- **Task list owner:** Overall Manager; drafted by the S1-D Primary specification author
- **Last updated:** 2026-10-27
- **Decision authority:** `Docs/Handoffs/S1-D-RepeatedInterventions-Spec/MANAGER-REVIEW.md` @ `457461b9b92da2e96101f5b0eb9940b4e92fec0a`; amendment `AMENDMENT-PACKET.md` @ `771620113487894bd8e09c69309318cc603ae063`; resolved decisions RD-1…RD-12 in spec §12.

Tasks are implementation units, not new product decisions. Each task stays inside the approved spec and plan. If a task exposes a material product ambiguity, stop and return it to the Manager/spec. There are no `OPEN-n` blockers: every product decision is recorded as RD-1…RD-12.

Common exclusions for every task: no change to `ScenarioOneCompletionRules.cs`, the eight objective predicates or targets, economy/ecology values, Reference Future v1, scenes/prefabs/USS/UXML/ProjectSettings/packages, or Rack V2 lane-owned files. No Windows build. No new persisted field in S1-D V1. No fixed intervention interval, no automatic treatment choice, no hidden automation replacing player judgement. Disposable verification scripts follow the `Tools/Verification/` copy/run/remove workflow in AGENTS.md.

## T-00 — Resolve the product decisions

- **Task ID:** T-00
- **Status:** **COMPLETE** — Manager decisions recorded in `MANAGER-REVIEW.md`; amendment applied to this package.
- **Goal:** Decide `OPEN-1` through `OPEN-11` and the management-event-ID question, and record the outcome.
- **Scope:** Decisions and canonical-document updates only.
- **Exclusions:** Any code.
- **Owner / routing:** Overall Manager / user.
- **Dependencies:** —
- **Verification:** `MANAGER-REVIEW.md` records all eleven decisions plus the event-ID policy; spec §12 records them as RD-1…RD-12; no unresolved blocking `OPEN-n` remains in this package.
- **Stop conditions:** None remaining.
- **Completion state:** **Achieved.** Spec `APPROVED`; decisions available to the dependent tasks.

## T-01 — Re-verify the baseline and the assumption register

- **Task ID:** T-01
- **Status:** **AUTHORISED — mandatory first execution task.** Starts only after the active Rack V2 core work is integrated and the S1-C contract is `APPROVED`; T-02…T-06 remain dependent on it.
- **Goal:** Confirm at the actual integrated base that F-1..F-12 and R-1..R-5 still hold, replace every `[CODE c6342f7]` with the real SHA, and report the S1-C reconciliation findings of spec §9.4.
- **Scope:** Read-only inspection; run the existing offline suites (N3, ScenarioHistory offline, RackBay offline); no production change.
- **Exclusions:** Any edit.
- **Owner / routing:** Reviewer-capable worker with repository tools. TBD.
- **Dependencies:** T-00 complete; Rack V2 core integrated; S1-C contract `APPROVED`.
- **Verification:** A table of finding → still true / changed / new, with SHA and command output; N3 normalised-source hash reproduced unchanged; exactly one derived-history implementation confirmed; grouping unit and schema ordering reported per spec §9.4.
- **Stop conditions:** A finding is no longer true or a new blocker appears → return to Manager; a second derived-history implementation, competing store or duplicated history UI exists → STOP; do not silently adapt.
- **Completion state:** Signed finding table attached to the hand-off; spec facts corrected if needed.

## T-02 — Pure derived cycle state and readiness

- **Task ID:** T-02
- **Status:** **AUTHORISED — starts after T-01 passes.**
- **Goal:** FR-D1..D3, FR-F1. A `UnityEngine`-free static class (working name `ScenarioCycleState`) returning named, advisory cycle components and reasons from persisted records only.
- **Scope:** New class beside `ScenarioInterventionHistory`; read accessor on `ScenarioOneManager.History.cs`; offline suite in the Phase 7–9 style including the per-year acknowledgement derivation (EI-5).
- **Exclusions:** Any new persisted field; any blocking behaviour; any single prescriptive yes/no; any wording that instructs the player to intervene or states an interval as universal.
- **Owner / routing:** TBD (code-only capable).
- **Dependencies:** T-01
- **Verification:** Offline checks: every state yields an explanation (no empty hint); fourth cycle treated like the first; idle years add no intervention; an outstanding later review acknowledgement never gates Approve/Advance; rebuild is non-mutating and identical on a field-for-field copy (AC-4, AC-9).
- **Stop conditions:** Needs persisted state → stop; V1 allows none (RD-6, RD-7).
- **Completion state:** Offline suite green; no save diff; checklist FR-D and FR-F1 evidenced.

## T-03 — Decouple the teaching footprint cap from Approve and Advance

- **Task ID:** T-03
- **Status:** **AUTHORISED — starts after T-01 passes.**
- **Goal:** FR-A3, AC-2. The 40 % cumulative teaching-footprint rule is hard **only while its owning lesson/sample activity is active**; after that activity is historically complete it is at most an advisory warning and cannot disable Approve Work Plan or Advance.
- **Scope:** `LearningObjectivesView.RackPlanProblem()` and its call sites in `ScenarioOneUiRoot` and `WorkPlanView`; the activity-complete condition reuses the persisted-history latch of FR-B1 so there is one definition.
- **Exclusions:** Changing `MaxRemovedBasalShare`, `CompetitionShare`, the sample lesson's own teaching behaviour during the lesson, or MFM's prescription ceiling. No lifetime hard block.
- **Owner / routing:** TBD (Unity-capable for the gate).
- **Dependencies:** T-01
- **Verification:** Offline decision table (lesson active/closed × cap exceeded × cycle); Unity: cycle-2 thinning in the footprint approves and advances; during the lesson the original safeguard still applies; message names rule, footprint, quantity, remedy. Narrow regression: RackLesson, MenuTutorial, TeachingCopy.
- **Stop conditions:** The fix needs to change `ScenarioOneRackBay` analysis semantics beyond the call-site predicate, or edit rack lifecycle files → stop (cross-lane); the retained in-activity constraint can trap a player who can never complete the owning activity → stop (spec §17).
- **Completion state:** AC-2 evidenced; no change to completion or protected anchors.

## T-04 — Guide monotonicity

- **Task ID:** T-04
- **Status:** **AUTHORISED — starts after T-01 passes; land before any MFM curriculum insertion that touches the same guide/current-state files.**
- **Goal:** FR-B1, FR-B2, AC-3. The guide does not regress on Crop Tree count changes, later in-footprint felling, executed-share drift or load.
- **Scope:** `ScenarioOneTeachingSequence.Current` and `LearningObjectivesView.Sequence.cs` per the mechanism chosen in plan §2.2. Latch on historical "ever satisfied" persisted facts and/or a frozen teaching-sample completion condition; distinguish before/after historical completion; new IDs only.
- **Exclusions:** Renaming or reusing any step/lesson ID; touching `ScenarioOneCompletionRules`; adding a persisted field (V1 allows none); `PlayerPrefs` or session-only `[J]/[K]` as progression authority; persisting `[J]/[K]`.
- **Owner / routing:** TBD. Must be the single writer of the curriculum files at the time.
- **Dependencies:** T-01; MFM sequencing/ownership.
- **Verification:** Offline matrix over the regression triggers and lessons 1–8; Unity: save at lessons 6–8, reload, guide index unchanged. Teaching copy verification must still pass.
- **Stop conditions:** No persisted fact faithfully establishes completion in a state → keep existing behaviour there and report, do not invent a latch; mechanism needs a schema change → stop; MFM rewrites `Current()` first → rebase task.
- **Completion state:** AC-3 evidenced; step-ID diff shows no rename or reuse.

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
- **Status:** **AUTHORISED — starts after T-01 passes; independent review required for the shared load-path change.**
- **Goal:** FR-E3, AC-7.
- **Scope:** Pre-mutation validation that **rejects** a save with non-positive or duplicate `ScenarioManagementEvent.eventId`, with a clear validation error. Fixture scan first; CanLoad/LoadData parity.
- **Exclusions:** Repairing, renumbering or reinterpreting historical event identity; a schema field; changing the completion rule; relaxing any existing validator.
- **Owner / routing:** TBD; independent reviewer required (shared load path).
- **Dependencies:** T-01
- **Verification:** SaveHardening and CanLoad/LoadData parity gates; negative fixtures (zero, duplicate); positive fixtures from every shipped save version; no world or disk mutation on rejection.
- **Stop conditions:** A legitimate shipped or accepted fixture is ambiguous → STOP and return to Manager before changing compatibility policy.
- **Completion state:** AC-7 evidenced; protected anchors unchanged.

## T-07 — Conditional persistence

- **Task ID:** T-07
- **Status:** **CANCELLED for S1-D V1** (RD-6, RD-7). Reopening requires a later explicit Manager decision.
- **Goal:** None. Retained as a record of the cancelled option.
- **Scope:** —
- **Exclusions:** Any schema or persistence work under S1-D V1.
- **Owner / routing:** —
- **Dependencies:** —
- **Verification:** N/A — no work authorised.
- **Stop conditions:** A proposal to add persisted state → return to Manager.
- **Completion state:** Cancelled.

## T-08 — Multi-cycle witness and compatibility check

- **Task ID:** T-08
- **Status:** **BLOCKED on T-02..T-06.**
- **Goal:** AC-1, AC-4, AC-6, AC-8, AC-12, AC-14, AC-15.
- **Scope:** One Unity gate: three real cycles with idle years, a stale pending order, save/load at each stage, completion on the third and continued management after Completed; D-057 solvent witness re-run; narrow regression set; protected anchor comparison.
- **Exclusions:** Rebaselining any anchor; adding thresholds to the gate to make it pass.
- **Owner / routing:** Unity-capable lane under the machine-wide Unity lock. TBD.
- **Dependencies:** T-02..T-06
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
| T-01 | AUTHORISED — mandatory first | Reviewer-capable worker | T-00 + integration base |
| T-02 | AUTHORISED after T-01 | TBD | T-01 |
| T-03 | AUTHORISED after T-01 | TBD (Unity-capable) | T-01 |
| T-04 | AUTHORISED after T-01 | Single curriculum writer | T-01 + MFM order |
| T-05 | AUTHORISED after T-01/T-02 | TBD + S1-C UI owner | T-01, T-02, S1-C |
| T-06 | AUTHORISED after T-01 | TBD + independent reviewer | T-01 |
| T-07 | CANCELLED (V1) | — | — |
| T-08 | BLOCKED | Unity lane | T-02..T-06 |
| T-09 | BLOCKED | Independent reviewer | T-08 |
