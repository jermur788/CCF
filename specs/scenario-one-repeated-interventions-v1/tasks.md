# Scenario One Second and Subsequent Interventions (S1-D) Bounded Tasks

- **Spec ID:** SPEC-scenario-one-repeated-interventions-V1
- **Spec status:** DRAFT — **no task may be dispatched until the spec is APPROVED**
- **Task list owner:** Overall Manager; drafted by the S1-D Primary specification author
- **Last updated:** 2026-10-27

Tasks are implementation units, not new product decisions. They may be drafted before approval, but implementation must not be dispatched until the spec is `APPROVED`. Each task stays inside the approved spec and plan. If a task exposes a material product ambiguity, stop and return it to the Manager/spec. A task marked BLOCKED names the `OPEN-n` that must be decided first.

Common exclusions for every task: no change to `ScenarioOneCompletionRules.cs`, the eight objective predicates or targets, economy/ecology values, Reference Future v1, scenes/prefabs/USS/UXML/ProjectSettings/packages, or Rack V2 lane-owned files. No Windows build. Disposable verification scripts follow the `Tools/Verification/` copy/run/remove workflow in AGENTS.md.

## T-00 — Resolve the product decisions

- **Task ID:** T-00
- **Status:** TODO (Manager/user action; not an implementation task)
- **Goal:** Decide `OPEN-1` through `OPEN-11` (spec §12) and record the outcome in the Decision Log, Current Milestone and, once implemented, the Overview.
- **Scope:** Decisions and canonical-document updates only.
- **Exclusions:** Any code.
- **Owner / routing:** Overall Manager / user. FREE HERE.
- **Dependencies:** — (`OPEN-1` first; it conditions every other task)
- **Verification:** Decision-log entries exist; spec updated to remove or resolve each `OPEN-n`; status moved to `APPROVED`.
- **Stop conditions:** The S1-C draft contradicts spec §9 → reconcile both specs before approval.
- **Completion state:** Spec `APPROVED` with no unresolved blocking `OPEN-n`.

## T-01 — Re-verify the baseline and the assumption register

- **Task ID:** T-01
- **Status:** BLOCKED on T-00 (`OPEN-1`) and on the Rack V2 / S1-C integration order
- **Goal:** Confirm at the actual implementation base that F-1..F-12 and R-1..R-5 still hold, and replace every `[CODE c6342f7]` with the real SHA.
- **Scope:** Read-only inspection; run the existing offline suites (N3, ScenarioHistory offline, RackBay offline); no production change.
- **Exclusions:** Any edit.
- **Owner / routing:** Reviewer-capable worker with repository tools. TBD.
- **Dependencies:** T-00
- **Verification:** A table of finding → still true / changed / new, with SHA and command output; N3 normalised-source hash reproduced unchanged.
- **Stop conditions:** A finding is no longer true or a new blocker appears → return to Manager; do not silently adapt.
- **Completion state:** Signed finding table attached to the hand-off; spec facts corrected if needed.

## T-02 — Pure derived cycle state and readiness

- **Task ID:** T-02
- **Status:** BLOCKED on T-00 (`OPEN-3`, `OPEN-4`)
- **Goal:** FR-D1..D3, FR-F1. A `UnityEngine`-free static class (working name `ScenarioCycleState`) returning named cycle components and reasons from persisted records only.
- **Scope:** New class beside `ScenarioInterventionHistory`; read accessor on `ScenarioOneManager.History.cs`; offline suite in the Phase 7–9 style including the per-year acknowledgement derivation (EI-5).
- **Exclusions:** Any new persisted field; any blocking behaviour; any wording that instructs the player to intervene or states an interval as universal.
- **Owner / routing:** TBD (code-only capable).
- **Dependencies:** T-00, T-01
- **Verification:** Offline checks: every state yields an explanation (no empty hint), fourth cycle treated like the first, idle years add no intervention, rebuild is non-mutating and identical on a field-for-field copy (AC-4, AC-9).
- **Stop conditions:** Needs persisted state → `OPEN-6`/`OPEN-7`, stop.
- **Completion state:** Offline suite green; no save diff; checklist FR-D, FR-F1 evidenced.

## T-03 — Decouple the teaching footprint cap from Approve and Advance

- **Task ID:** T-03
- **Status:** BLOCKED on T-00 (`OPEN-5`)
- **Goal:** FR-A3, AC-2. After the owning lesson, the cumulative 40 % teaching-footprint rule no longer hard-blocks Approve/Advance except as decided; any remaining block explains itself.
- **Scope:** `LearningObjectivesView.RackPlanProblem()` and its call sites in `ScenarioOneUiRoot` and `WorkPlanView`.
- **Exclusions:** Changing `MaxRemovedBasalShare`, `CompetitionShare`, the sample lesson's own teaching behaviour during the lesson, or MFM's prescription ceiling.
- **Owner / routing:** TBD (Unity-capable for the gate).
- **Dependencies:** T-00, T-01
- **Verification:** Offline decision table (lesson active/closed × cap exceeded × cycle); Unity: cycle-2 thinning in the footprint approves and advances; during the lesson the original safeguard still applies; message names rule, footprint, quantity, remedy. Narrow regression: RackLesson, MenuTutorial, TeachingCopy.
- **Stop conditions:** Needs edits in `ScenarioOneRackBay.cs` beyond the call-site predicate, or in rack lifecycle files → stop (cross-lane).
- **Completion state:** AC-2 evidenced; no change to completion or protected anchors.

## T-04 — Guide monotonicity

- **Task ID:** T-04
- **Status:** BLOCKED on T-00 (`OPEN-7`) and on the MFM insertion order
- **Goal:** FR-B1, FR-B2, AC-3. The guide does not regress on Crop Tree count changes, later in-footprint felling, executed-share drift or load.
- **Scope:** `ScenarioOneTeachingSequence.Current` and `LearningObjectivesView.Sequence.cs` per the mechanism chosen in the plan (§2.2); new IDs only.
- **Exclusions:** Renaming or reusing any step/lesson ID; touching `ScenarioOneCompletionRules`; adding a persisted field unless `OPEN-7(b)` is approved.
- **Owner / routing:** TBD. Must be the single writer of the curriculum files at the time.
- **Dependencies:** T-00, T-01, MFM sequencing decision
- **Verification:** Offline matrix over the four regression triggers and lessons 1–8; Unity: save at lessons 6–8, reload, guide index unchanged. Teaching copy verification must still pass.
- **Stop conditions:** Mechanism needs a schema change not approved → stop; MFM rewrites `Current()` first → rebase task.
- **Completion state:** AC-3 evidenced; step-ID diff shows no rename or reuse.

## T-05 — Previous versus proposed presentation

- **Task ID:** T-05
- **Status:** BLOCKED on T-00 (`OPEN-6`) and on S1-C UI ownership
- **Goal:** FR-C1..C5, AC-5.
- **Scope:** Work Plan grouping (Approved / Pending new / Pending carried over); links to the Annual Review year; no designation-time wording; map past-work layer only if `OPEN-6` includes it.
- **Exclusions:** New persisted fields (unless approved); automatic cancellation or approval; causal claims.
- **Owner / routing:** TBD; coordinate with the S1-C writer for `WorkPlanView.cs`/`AnnualReviewView.cs`.
- **Dependencies:** T-02, T-00, S1-C interface (spec §9)
- **Verification:** Rendered check at 1280×720 and 1920×1080 (`ScenarioOneUx`); offline wording checks; Unity: three-cycle fixture shows the five classes distinctly.
- **Stop conditions:** The S1-C design already owns the card → integrate through it, do not duplicate.
- **Completion state:** AC-5 evidenced with captures.

## T-06 — Management-event ID integrity

- **Task ID:** T-06
- **Status:** BLOCKED on T-00 (G5 approval of a load-path change)
- **Goal:** FR-E3, AC-7.
- **Scope:** Pre-mutation validation (reject with message) or deterministic repair for non-positive or duplicate `ScenarioManagementEvent.eventId`; fixture scan first.
- **Exclusions:** Changing the completion rule; relaxing any existing validator.
- **Owner / routing:** TBD; independent reviewer required (shared load path).
- **Dependencies:** T-00, T-01
- **Verification:** SaveHardening and CanLoad/LoadData parity gates; negative fixtures (zero, duplicate); positive fixtures from every shipped save version; no world or disk mutation on rejection.
- **Stop conditions:** A legitimate shipped fixture is ambiguous → stop, Manager decision.
- **Completion state:** AC-7 evidenced; protected anchors unchanged.

## T-07 — Conditional persistence

- **Task ID:** T-07
- **Status:** BLOCKED — **only exists if** `OPEN-6(b)` or `OPEN-7(b)` is chosen; otherwise CANCELLED
- **Goal:** FR-F2.
- **Scope:** The minimal field(s) approved; next free schema version; defaults; validators; hash-compat path.
- **Exclusions:** Any field not named in the approved decision.
- **Owner / routing:** TBD; single writer of `ForestSaveData`; independent review (G5).
- **Dependencies:** T-00, Rack V2 persistence integrated, MFM schema order
- **Verification:** Old-save load with defaults; save/load round trip; Reference/Legacy40/Enlarged80 anchors exact; CanLoad/LoadData parity.
- **Stop conditions:** Schema number contention with Rack V2 or MFM → Manager.
- **Completion state:** AC-8 evidenced.

## T-08 — Multi-cycle witness and compatibility check

- **Task ID:** T-08
- **Status:** BLOCKED on T-02..T-06
- **Goal:** AC-1, AC-4, AC-6, AC-8.
- **Scope:** One Unity gate: three real cycles with idle years, a stale pending order, save/load at each stage, completion on the third; D-057 solvent witness re-run; narrow regression set; protected anchor comparison.
- **Exclusions:** Rebaselining any anchor; adding thresholds to the gate to make it pass.
- **Owner / routing:** Unity-capable lane under the machine-wide Unity lock. TBD.
- **Dependencies:** T-02..T-06
- **Verification:** Gate tokens, logs and anchor comparison attached; one stated uncertainty ("does repetition keep completion reachable and solvent?") and stop when answered.
- **Stop conditions:** Completion unreachable or unrecoverable without a coefficient change → Manager (spec §17).
- **Completion state:** Evidence recorded; Tier-2 handoff.

## T-09 — Independent spec-conformance review and hand-off

- **Task ID:** T-09
- **Status:** BLOCKED on T-08
- **Goal:** Close the spec checklist; confirm no hidden automation and no unlabelled coefficient (AC-10).
- **Scope:** Review only.
- **Exclusions:** Any implementation.
- **Owner / routing:** Independent reviewer (not the implementing thread). REGULAR CLAUDE VALUE or equivalent.
- **Dependencies:** T-08
- **Verification:** `checklists/requirements.md` fully evidenced; verdict APPROVED / NEEDS CHANGES / REJECTED.
- **Stop conditions:** Any mandatory gate unmet.
- **Completion state:** Checklist verdict recorded; spec moved to `VERIFYING`/`VERIFIED` by the Manager.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| T-00 | TODO | Manager / user | — |
| T-01 | BLOCKED (T-00) | TBD reviewer-capable | T-00 |
| T-02 | BLOCKED (OPEN-3/4) | TBD | T-00, T-01 |
| T-03 | BLOCKED (OPEN-5) | TBD | T-00, T-01 |
| T-04 | BLOCKED (OPEN-7, MFM order) | TBD | T-00, T-01 |
| T-05 | BLOCKED (OPEN-6, S1-C) | TBD | T-00, T-02 |
| T-06 | BLOCKED (G5) | TBD | T-00, T-01 |
| T-07 | BLOCKED / maybe CANCELLED | TBD | T-00 |
| T-08 | BLOCKED | Unity lane | T-02..T-06 |
| T-09 | BLOCKED | Independent reviewer | T-08 |
