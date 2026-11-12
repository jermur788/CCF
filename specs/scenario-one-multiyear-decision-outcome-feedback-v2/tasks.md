# Scenario One — Multi-Year Decision and Outcome Feedback V2 Bounded Tasks

- **Spec ID:** SPEC-scenario-one-multiyear-decision-outcome-feedback-V2
- **Spec status:** APPROVED — BOUNDED D-073 ANNUAL REVIEW V2
- **Task list owner:** Overall Manager
- **Last updated:** 2026-11-12 (HP #409 refinement)

Tasks are implementation units, not new product decisions. **The spec is approved, but no code task may start without its own approved packet, exclusive owner and exact reconciled base.** Each stays inside [spec.md](spec.md) and [plan.md](plan.md). A task that exposes a product ambiguity stops and returns it to the Manager. Owners below are routing suggestions, not assignments.

Already done and not repeated as tasks: the static source audit ([#394](https://github.com/jermur788/CCF/issues/394)), the helper's offline qualification ([#406](https://github.com/jermur788/CCF/issues/406), Manager-approved for offline scope) and this draft refinement ([#409](https://github.com/jermur788/CCF/issues/409)).

## T-00 — Manager decision on the spec

- **Task ID:** T-00
- **Status:** COMPLETE — Manager-approved bounded scope and six product rulings on 12 November 2026
- **Goal:** Record APPROVED, NEEDS CHANGES or REJECTED for this four-file draft, and answer the six open product decisions in the spec.
- **Scope:** `spec.md` status line and open-decisions section; matching edits to the other three files if an answer changes them.
- **Exclusions:** No implementation release by implication; no D-074 or D-076 decision.
- **Owner / routing:** Overall Manager — FREE HERE.
- **Dependencies:** —
- **Verification:** The four files agree on status, FR/AC numbering and the chosen answers.
- **Stop conditions:** An answer needs new saved data, per-rack history or causal wording.
- **Completion state:** COMPLETE; approved status, six decisions, truth and unit clarifications written in `spec.md`.

## T-01 — Cadence release, integration base and single UI owner

- **Task ID:** T-01
- **Status:** READY FOR BASE RECONCILIATION / NOT RUNNING — #402/#408 handed off and Manager approved isolated cadence verification; exact joint cadence-plus-helper base and new single UI owner still pending.
- **Goal:** Name one exact base commit and one UI owner for this feature.
- **Scope:** Record in live coordination: the cadence lane's release of `AnnualReviewView.cs`, `WorkPlanView.cs` and `ScenarioOneUiRoot.cs`; the base branch and full HEAD; the UI owner, branch and worktree.
- **Exclusions:** No source edits. No merge to `main` or to the personal playtest lane (#382).
- **Owner / routing:** Overall Manager, with an integrator if the base must be built.
- **Dependencies:** T-00 = APPROVED; #408 returned with a result the Manager accepts.
- **Verification:** `git rev-parse <base>:Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` prints `2c7bdb3c6b6f13790a325ebabc187c497b335b52`, and the base contains the accepted cadence commit.
- **Stop conditions:** The helper blob differs; the cadence lane is still RUNNING; two writers would share a UI file.
- **Completion state:** Base HEAD, owner, branch and worktree are in `active-tasks.md`; no other claim names the UI file.

## T-02 — Re-run the helper's offline checks on the base

- **Task ID:** T-02
- **Status:** TODO
- **Goal:** Show the approved helper still passes, unchanged, on the T-01 base.
- **Scope:** Run the existing `Tools/Verification/ManagementFollowonReadout/run_offline.py` and the existing production type-binding check. Publish commands, exits and output.
- **Exclusions:** No source, fixture or helper change. No Unity.
- **Owner / routing:** SOL/CODEX or any local C#-capable worker; Tier 1.
- **Dependencies:** T-01.
- **Verification:** 53 assertions PASS; type binding reports 0 errors, with the underlying compiler exit inspected (the wrapper can return 0 on errors).
- **Stop conditions:** Any assertion fails or the helper no longer binds — report the exact error; do not repair inside this task.
- **Completion state:** A GitHub handoff with the command evidence on the exact base HEAD.

## T-03 — Text mapper and offline fixture

- **Task ID:** T-03
- **Status:** TODO
- **Goal:** Implement every display rule of FR-4 to FR-12 as a pure function that can be asserted without Unity.
- **Scope:** One new static class and `.meta` under `Assets/ForestPrototype/ScenarioOne/`; one new offline fixture under `Tools/Verification/`.
- **Exclusions:** No `AnnualReviewView.cs` edit; no helper edit; no `UnityEngine` reference in the mapper; no existing test weakened.
- **Owner / routing:** the T-01 UI owner; Tier 1.
- **Dependencies:** T-02.
- **Verification:** Mapper fixture asserts FR-9 state priority and exact wording, FR-4 year filtering, FR-5 exclusions and nonduplicated warnings, FR-7 canopy percentage-point conversion, FR-8 one-sided missing species, FR-10 DERIVED/RECORDED labels, FR-12 no currency, AC-12 event-source parity on clean versus malformed fixtures, and FR-14 input preservation.
- **Stop conditions:** A rule cannot be expressed from the `Result` as it stands.
- **Completion state:** Fixture PASS output published; type binding still 0 errors; changed paths are exactly the new files.

## T-04 — Wire the card into the Annual Review

- **Task ID:** T-04
- **Status:** BLOCKED — on T-01
- **Goal:** Show the card in the existing review (FR-1, FR-2, FR-3, FR-10, FR-14).
- **Scope:** `Assets/ForestPrototype/UI/AnnualReviewView.cs` only: one private build method, its call inside the change-key-guarded part of `Refresh`, one species foldout and its open-state key.
- **Exclusions:** No other file. No new style unless T-05 shows 720p needs one, in which case stop and report. No change to title, buttons, footer or the cadence skip summary.
- **Owner / routing:** the T-01 UI owner.
- **Dependencies:** T-03; T-01 completion state still true at start.
- **Verification:** The diff touches one file; `Build` is called once per rebuild and not outside the guard; `AcknowledgeAnnualReview` gains no new caller; offline type binding 0 errors.
- **Stop conditions:** Any spec stop condition; the base has moved under the call site.
- **Completion state:** Branch pushed, handoff published and marked **not Unity-verified**.

## T-05 — Unity qualification

- **Task ID:** T-05
- **Status:** TODO
- **Goal:** Check on the exact T-04 head everything that only Unity can show.
- **Scope:** Compile and import; then, at 1280×720 and 1920×1080 with real input: AC-2, AC-3, AC-7, AC-8, AC-9, AC-10 and AC-12.
- **Exclusions:** No production edits; no merge; no Windows or Player build; no Tier 3.
- **Owner / routing:** a Unity-capable worker **other than the T-04 implementer**; one machine-wide lease; Tier 2.
- **Dependencies:** T-04.
- **Verification:**
  - compile and import clean on the exact head;
  - screenshots at both resolutions of: latest year, an earlier year with full comparison, an earlier year with a missing snapshot;
  - management-event count and world hash equal before and after opening, stepping and closing the review (AC-7);
  - card rows captured after N one-year advances equal those after one long advance over the same years (AC-8);
  - a current save, a genuine older save, Legacy40 and frozen Reference Future v1 each load, with reference previews unchanged (AC-10);
  - card counts equal event-counted Work-column categories only on complete and consistent event/order fixtures; report-derived and ambiguous cases must show reasoned divergences (AC-12).
- **Stop conditions:** Footer or year buttons displaced at 720p; any hash or event difference; any old-save load difference.
- **Completion state:** A full PASS / FAIL / NOT RUN table per item, with evidence paths, on the exact head. A row that was not run is written NOT RUN.

## T-06 — Review, verdict and integration decision

- **Task ID:** T-06
- **Status:** TODO
- **Goal:** Independent review of T-03 to T-05, then the Manager's verdict.
- **Scope:** Diff and evidence review against the checklist; integration decision; coordination and dashboard update.
- **Exclusions:** No merge to `main` without the Manager's separate decision and the user's playtest position; no Windows build.
- **Owner / routing:** independent reviewer (REGULAR CLAUDE VALUE), then Overall Manager.
- **Dependencies:** T-05.
- **Verification:** Every checklist line has evidence or a stated reason.
- **Stop conditions:** Evidence from a different head; a checked line without evidence.
- **Completion state:** APPROVED / NEEDS CHANGES / REJECTED recorded with the reviewed HEAD.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| T-00 | COMPLETE | Overall Manager | — |
| T-01 | READY / NOT RUNNING | Overall Manager / integrator | T-00, approved #402/#408; joint base pending |
| T-02 | TODO | SOL/CODEX, Tier 1 | T-01 |
| T-03 | TODO | UI owner, Tier 1 | T-02 |
| T-04 | BLOCKED | UI owner | T-03, T-01 |
| T-05 | TODO | Unity worker, not the implementer, Tier 2 | T-04 |
| T-06 | TODO | Independent reviewer, then Manager | T-05 |

Out of scope for every task: Retain-why UX, MFM rule changes, D-074 completion policy, the time-skip controller, save schema or world hash, historical per-rack ecology, and any Windows build.
