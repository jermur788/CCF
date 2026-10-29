# S1-D T-03 — Decouple the teaching footprint cap from Approve and Advance — Handoff

**Task:** claim #103 under umbrella #91. **Spec:** `specs/scenario-one-repeated-interventions-v1/` T-03, AC-2, FR-A3, RD-5.
**Mode:** code + offline verification only. **No Unity run, no Unity lock, no Player/Windows build occurred or is claimed.**

## Who / where

| | |
|---|---|
| Actual model/provider | Claude Code (T3 Code harness) / Anthropic `claude-sonnet-5-5` (Sonnet 5.5), local session |
| Branch / worktree | `feature/s1-d-t03-teaching-cap-decouple-v1` / `/media/jer/Videos/CCF-S1D-T03` |
| Base | `23fe4b8` (umbrella integration) |
| Code + verification commits | `9d29d06` (first cut), then `af00d87cec679c9f0b532b88d7d78ca96efc8aa8` (the decision; all gates below ran on this tree) |
| Branch tip | the docs-only commit that adds this file on top of `9d29d06` |
| Durable evidence | `/media/jer/Files/CCF/Verification/S1D-T03-TeachingCap-20261029/` |

## Changed files

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/ScenarioOneTeachingCap.cs` (+ `.meta`, hand-written, GUID checked unique; Unity not run so import is unobserved) | **new** pure static policy, no `UnityEngine`: `Classify`, `SampleHistoricallyComplete`, `HardMessage`, `AdvisoryMessage` |
| `ScenarioOne/ScenarioOneRackBay.cs` | +5 lines: additive `ActionableRemovals` counter (see decision) |
| `UI/LearningObjectivesView.Sequence.cs` | `RackPlanProblem()` is non-empty only for `Phase.Hard`; new `RackPlanAdvisory()` |
| `UI/ScenarioOneUiRoot.cs` | +`RackPlanAdvisory` pass-through; `ApproveWorkPlan` / `AdvanceFromWorkPlan` unchanged (still gate on `RackPlanProblem`) |
| `UI/WorkPlanView.cs` | advisory added to the redraw signature and prepended to the feedback line; buttons still enable on `rackProblem` only |
| `Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapOfflineCheck.cs`, `run_teaching_cap_offline.py` | **new** offline suite, runner, mutation harness, protected-path Git diff |

## Behaviour (AC-2)

- **Teaching sample open** (not historically complete): hard when `Bay != null && AccessOpen && !CanopyKept` **and the player still has planned or marked living trees in the footprint** (`Analysis.ActionableRemovals > 0`). With actionable work the answer equals the old predicate (84-row grid, 0 mismatches). With none (excess already executed, e.g. after mortality drift) it is an advisory, never a deadlock. The message now also names the rule, the footprint (`the middle rack`), the quantity (e.g. `52%`, limit `40% cumulative`) and the remedy.
- **Sample historically complete** = executed in-footprint release met the lesson's own executed target (`MinExecuted >= 0.30 x 0.9`) **and** both sold and deadwood outcomes executed **and** the Annual Review after that work acknowledged. The cap is *not* an input, so exceeding it cannot reopen or complete the activity. Over the cap, this gives an advisory note (no instruction, states it does not block) and Approve Work Plan / Advance stay enabled.
- `MaxRemovedBasalShare`, `CompetitionShare`, `CanopyKept`, `ScenarioOneRackBay` analysis, MFM ceilings: untouched. No new persisted field.

## Offline results (all on `9d29d06`, `python3 -I -B`)

| Suite | Result |
|---|---|
| **New** teaching cap | `TEACHING_CAP_OFFLINE_PASS checks=48 NOT_UNITY_VERIFIED`; `TEACHING_CAP_MUTATIONS_PASS caught=8 survived=0` |
| Protected-path diff vs `23fe4b8` | `TEACHING_CAP_PROTECTED_DIFF_PASS paths=16` + RackBay additive-only check (save data/validation, completion rules, objectives, rack review/plan, history, cycle state, `AnnualReviewView`, TeachingSequence, RackBayText, Racks, WorkEconomy, scenes, ProjectSettings, Packages) |
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

## Manager decision applied: no deadlock before completion (spec §17)

Decision: the cap is hard before completion **only while planned or marked work exists in the footprint**. A sold-only or short release followed by mortality/storm drift, with nothing left to cancel, now shows the advisory and permits Approve and Advance. As soon as the player plans or marks another in-footprint tree, the hard safeguard returns for the open sample. Post-completion behaviour (AC-2) is unchanged.

- Needed one additive field: `ScenarioOneRackBay.Analysis.ActionableRemovals` = living in-footprint trees that are planned or marked (served by a Crop Tree or not, which is exactly the part of `RemovedBasalShare` the player can still undo). The existing `plannedIds`/`PlannedRemovals`/`MarkedRemovals` count only trees serving a Crop Tree, so they could have wrongly released the block. The change is 5 added lines, 0 deleted; no threshold, share or `CanopyKept` change. The runner asserts zero deleted lines in that file.
- Regression rows in the suite: sold-only + drift, short release + drift, and drift with no release executed, each with nothing actionable -> Advisory (results read or not). The same state plus one planned/marked tree -> Hard. Complete sample with actionable work -> Advisory. Real `Analyse` check: executed-only removal adds share but `ActionableRemovals == 0`; one planned + one marked inside the footprint = 2, a planned tree outside = not counted.

## Other residuals (for Unity qualification / T-04)

1. **Window after a later cycle.** After a cycle-2 in-footprint release, `ReleaseReviewed` is keyed to the latest release year, so until that Annual Review is acknowledged the sample reads "not complete" and the cap can be hard again for Approve. Advance already requires the review. Unity should check this ordering; a first-release latch (T-04) would remove it.
2. **Guide regression is T-04's.** `ScenarioOneTeachingSequence.Current` still returns lesson 3 when `!CanopyKept`, and `ScenarioOneRackBayText` lesson text still says "Already executed excess cannot pass". Neither blocks Approve/Advance; both are the FR-B monotonicity work and are untouched.
3. The §17 deadlock is closed by the decision above; the lesson guide can still sit at lesson 3 in that state (T-04).
4. `SampleHistoricallyComplete` reads the current bay analysis, so unmarking a Crop Tree later can change `MixedExecuted`/`MinExecuted`. T-04's persisted latch should replace this one function.

## Unity residual (not run)

Narrow regression per the task: RackLesson, MenuTutorial, TeachingCopy. Plus: (a) cycle-2 thinning in the footprint over 40 % approves and advances, advisory line visible in the Work Plan feedback; (b) during the lesson the original safeguard still disables both buttons with the new message; (c) the drift regression: after a sold-only release is pushed over 40% by mortality with nothing planned or marked, the Work Plan shows the advisory and both buttons work; (d) import of `ScenarioOneTeachingCap.cs.meta`.

## Exit token

`S1-D T-03 COMPLETE — READY FOR UNITY QUALIFICATION`

## Unity qualification (Sonnet 5.5 / Claude provider; candidate code f629be3, verification-only fixes through fe166d0)

Evidence root `/media/jer/Files/CCF/Verification/S1D-T03-Unity-20261029/`. U1 held at `f629be3` (= integration `aca74ac` + T-03 code, byte-identical production), no Windows build, lock released, U1 clean, harness/metas/licence copies removed.

| Gate | Result | Evidence |
|---|---|---|
| cap (production Rack V2) | PASS, `TEACHING_CAP_UNITY_PASS checks=223 shots=12`, 312 s (cap-v3; harness bytes unchanged, carried) | `cap-v3/` incl. cycle-2 over-cap approve+advance, lesson-time hard safeguard, sold-only/short-mixed drift advisory with both buttons enabled, one-more-mark hard, 12 captures at 1280x720 and 1920x1080 |
| TeachingCopy (`teaching`) | PASS `S1A_TEACHING_VERIFY_PASS checks=61`, 14 captures, lane clean | `teaching/` (v1 FAIL kept in `teaching-v1-fail/`) |
| MenuTutorial (`menu`) | PASS (pass marker, exit 0, lane clean) | `menu/` |
| Rack runtime lifecycle (`rack`) | PASS | `rack/` |
| S1-D T-06 `event-id` | PASS | `event-id/` |
| S1-C T-03 `history`, S1-D T-02 `history-browse` | PASS, PASS | `history/`, `history-browse/` |
| RackLesson (`lesson`) | **FAIL, pre-existing, identical on base `aca74ac` (no T-03 code)**: `accepted live middle-rack half-width remains 8.00385 m [half width=7.995]` | `lesson/`, `lesson-base-aca74ac/` |

### Classification of the teaching failure
Harness/setup only, not a production regression. `help outside viewport (0,0,0,0)` was a layout race: the Game-view resize is applied asynchronously on the Xephyr display and the harness asserted after a fixed 12 frames. Fix (assertions unchanged): wait for the requested `Screen` size, plus a 0.4 s real-time settle. Same pattern fixed in MenuTutorial's `CaptureScreen`.

### Other verification-only fixes (no production change)
- MenuTutorial predates the three-page scenario introduction (merged after the last Menu qualification at 1398887): the first capture showed the introduction, not the HUD help window. The harness now pages through the introduction with the player's Next action, asserts it completed, then asserts and captures the HUD help window.
- Runner restores every SectionFive material import rewrite (Forb, Bilberry, Berry, Fungi; patches retained as evidence) instead of a fixed three-file list.

### Open exception: RackLesson (manager decision needed)
`RackLessonVerification` hard-codes the accepted live half-width 8.00385 m; the integrated base measures 7.995 m (footprint 69 trees and 2.0 m lattice checks pass just before). Identical result without T-03, so not attributable to T-03, and no protected expected value was changed. The gate aborts at that check, so its later checks did not run on this base. Decide: re-accept 7.995 m on this base (separate task) or accept the exception.

## Exit token

`S1-D T-03 UNITY QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW` (T-03 cap and all other gates pass; exception above is pre-existing on the base)
