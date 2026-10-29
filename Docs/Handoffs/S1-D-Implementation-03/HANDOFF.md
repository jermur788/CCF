# S1-D T-03 — Decouple the teaching footprint cap from Approve and Advance — Handoff

**Task:** claim #103 under umbrella #91. **Spec:** `specs/scenario-one-repeated-interventions-v1/` T-03, AC-2, FR-A3, RD-5.
**Mode:** code + offline verification only. **No Unity run, no Unity lock, no Player/Windows build occurred or is claimed.**

## Who / where

| | |
|---|---|
| Actual model/provider | Claude Code (T3 Code harness) / Anthropic `claude-sonnet-5-5` (Sonnet 5.5), local session |
| Branch / worktree | `feature/s1-d-t03-teaching-cap-decouple-v1` / `/media/jer/Videos/CCF-S1D-T03` |
| Base | `23fe4b8` (umbrella integration) |
| Code + verification commit | `9d29d06652dba475ab52301496bbffac27baacb9` (all gates below ran on this tree) |
| Branch tip | the docs-only commit that adds this file on top of `9d29d06` |
| Durable evidence | `/media/jer/Files/CCF/Verification/S1D-T03-TeachingCap-20261029/` |

## Changed files

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/ScenarioOneTeachingCap.cs` (+ `.meta`, hand-written, GUID checked unique; Unity not run so import is unobserved) | **new** pure static policy, no `UnityEngine`: `Classify`, `SampleHistoricallyComplete`, `HardMessage`, `AdvisoryMessage` |
| `UI/LearningObjectivesView.Sequence.cs` | `RackPlanProblem()` is non-empty only for `Phase.Hard`; new `RackPlanAdvisory()` |
| `UI/ScenarioOneUiRoot.cs` | +`RackPlanAdvisory` pass-through; `ApproveWorkPlan` / `AdvanceFromWorkPlan` unchanged (still gate on `RackPlanProblem`) |
| `UI/WorkPlanView.cs` | advisory added to the redraw signature and prepended to the feedback line; buttons still enable on `rackProblem` only |
| `Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapOfflineCheck.cs`, `run_teaching_cap_offline.py` | **new** offline suite, runner, mutation harness, protected-path Git diff |

## Behaviour (AC-2)

- **Teaching sample open** (not historically complete): exactly the old rule. Hard when `Bay != null && AccessOpen && !CanopyKept`. Verified against the old predicate over a 42-row grid, 0 mismatches. The message now also names the rule, the footprint (`the middle rack`), the quantity (e.g. `52%`, limit `40% cumulative`) and the remedy.
- **Sample historically complete** = executed in-footprint release met the lesson's own executed target (`MinExecuted >= 0.30 x 0.9`) **and** both sold and deadwood outcomes executed **and** the Annual Review after that work acknowledged. The cap is *not* an input, so exceeding it cannot reopen or complete the activity. Over the cap, this gives an advisory note (no instruction, states it does not block) and Approve Work Plan / Advance stay enabled.
- `MaxRemovedBasalShare`, `CompetitionShare`, `CanopyKept`, `ScenarioOneRackBay` analysis, MFM ceilings: untouched. No new persisted field.

## Offline results (all on `9d29d06`, `python3 -I -B`)

| Suite | Result |
|---|---|
| **New** teaching cap | `TEACHING_CAP_OFFLINE_PASS checks=40 NOT_UNITY_VERIFIED`; `TEACHING_CAP_MUTATIONS_PASS caught=6 survived=0` |
| Protected-path diff vs `23fe4b8` | `TEACHING_CAP_PROTECTED_DIFF_PASS paths=17` (save data/validation, completion rules, objectives, rack bay/review/plan, history, cycle state, `AnnualReviewView`, TeachingSequence, RackBayText, Racks, WorkEconomy, scenes, ProjectSettings, Packages) |
| S1-D T-02 cycle state | `CYCLE_STATE_OFFLINE_PASS checks=93`; `CYCLE_STATE_MUTATIONS_PASS caught=7 survived=0` |
| N3 completion | `N3_OFFLINE_PASS checks=88` |
| S1-C history query | `S1C_HISTORY_QUERY_PASS checks=55` |
| Three-species history | `THREE_SPECIES_HISTORY_PASS checks=74` |
| Rack non-UI | keyboard 46 / rack bay 87 / rack sample 75 / approach 49 PASS |
| T-06 event-ID integrity | `EVENT_ID_OFFLINE_PASS checks=75 failures=0` |
| Runtime type-check (`--mode prod`) | exit 1, the same 4 `Painter2D` entries as T-02 reported (`StandMapCanvas`, `StandMapLegendView`, `OnboardingIcon`, `TeachingDiagram`); none in the files T-03 touched |

The T-01 guide/matrix checks are covered by the T-02/N3/history suites above; this base has no separate T-01 runner.

## Preserved anchors

Completion rules, the eight objectives, economy/ecology values, save/schema (no field, no `ForestSaveData*` diff), Rack V2 files, P06 pruning call sites, `AnnualReviewView` (S1-C owner) and Reference Future v1 have no diff against `23fe4b8`.

## BLOCKER / Manager decision — spec §17 stuck state is reachable by reading

This is the stop condition in spec §17 and the N-1 row from Implementation-01. T-03 follows the spec's "pre-completion unchanged", so it stays **Hard**:

> Release is executed **sold-only** (or short of the target), the footprint's living denominator then shrinks (mortality/storm), and removed share crosses 40 %. The sample is not complete, so the cap is hard. Executed removals cannot be cancelled, new deadwood work only adds to the share, so Approve **and** Advance stay disabled with nothing to undo. The offline suite pins this row (`STUCK-STATE ROW`).

Not confirmed in Unity. A fix is a product decision, not a call-site predicate: for example, make the cap hard only when planned/marked work in the footprint exists, so a purely executed excess blocks nothing. That changes pre-completion behaviour, so I did not make it. **Manager decision needed.**

## Other residuals (for Unity qualification / T-04)

1. **Window after a later cycle.** After a cycle-2 in-footprint release, `ReleaseReviewed` is keyed to the latest release year, so until that Annual Review is acknowledged the sample reads "not complete" and the cap can be hard again for Approve. Advance already requires the review. Unity should check this ordering; a first-release latch (T-04) would remove it.
2. **Guide regression is T-04's.** `ScenarioOneTeachingSequence.Current` still returns lesson 3 when `!CanopyKept`, and `ScenarioOneRackBayText` lesson text still says "Already executed excess cannot pass". Neither blocks Approve/Advance; both are the FR-B monotonicity work and are untouched.
3. `SampleHistoricallyComplete` reads the current bay analysis, so unmarking a Crop Tree later can change `MixedExecuted`/`MinExecuted`. T-04's persisted latch should replace this one function.

## Unity residual (not run)

Narrow regression per the task: RackLesson, MenuTutorial, TeachingCopy. Plus: (a) cycle-2 thinning in the footprint over 40 % approves and advances, advisory line visible in the Work Plan feedback; (b) during the lesson the original safeguard still disables both buttons with the new message; (c) the stuck-state row above; (d) import of `ScenarioOneTeachingCap.cs.meta`.

## Exit token

`S1-D T-03 IMPLEMENTED — NOT READY FOR UNITY QUALIFICATION: spec §17 STOP CONDITION REACHABLE (executed sold-only excess before completion); MANAGER DECISION REQUIRED`

If the Manager accepts that pre-completion row as is (or decides its fix), the token becomes `S1-D T-03 COMPLETE — READY FOR UNITY QUALIFICATION`.
