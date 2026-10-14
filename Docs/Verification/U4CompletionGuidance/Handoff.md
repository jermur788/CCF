# U4 completion guidance (follow-up to the U4 Unity qualification, result B)

Branch `task/scenario-one-completion-guidance-v1`, base `eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac` (the U4 implementation head). **Presentation only. No Unity run. Offline tests only.**

## What changed

The U4 qualification found that after one intervention the game shows "Forest objectives 8 of 8" and one generic line, and never says a reviewed second cycle is required. This adds the missing guidance and nothing else.

| file | change |
|---|---|
| `ScenarioOne/ScenarioOneCompletionRules.cs` | new pure `CompletionGuidanceState` enum, `GuidanceState(events, orders, allObjectivesMet)` and `GuidanceText(state)`. It reads the same chronology as `HasReviewedTwoCycleSequence` and `NeedsReviewAcknowledgement`; it changes nothing. |
| `ScenarioOne/ScenarioInterventionHistory.cs` | `NextStepHint` takes an optional trailing `completionGuidance`. Completion and stale orders keep their priority; null or empty leaves every older result unchanged. |
| `ScenarioOne/ScenarioOneManager.History.cs` | `CompletionGuidance` property (Active scenario only) and `NextStepHint` passes the text through. |
| `UI/AnnualReviewView.cs` | the acknowledgement footer shows the "acknowledge" wording while state 2 applies, and gives the first-review wording back otherwise. |

## The three states (precedence: 2, then 1, then 3)

| state | applies when | text (summary) |
|---|---|---|
| 2 `AcknowledgeReview` | a qualifying Work Plan exists and no acknowledgement is recorded after the latest one (objectives met or not) | Open the Annual Review and choose "I've read the annual results"; completion needs a reviewed intervention followed by another. Also shown in the footer. |
| 1 `ObjectivesMetOneCycle` | reviewed, all eight objectives met, exactly one qualifying plan | All eight objectives are met, but a second management decision is needed after you review the first. Read what your intervention did in the Annual Review, then carry out another intervention when you judge the forest needs it. |
| 3 `AnotherIntervention` | reviewed, second cycle not yet resolved (objectives not all met, or two plans with only a late review) | Return to the forest and carry out another intervention when you judge it needs it; completion comes once the objectives are met and that second intervention is done. |

Nothing is shown before a qualifying plan exists, or once the reviewed two-cycle sequence exists. The second intervention is never named (no felling, pruning, planting, species or "thin"); an offline test asserts that. The hint order is: completed, stale orders, completion guidance, the existing history hint. The hint reaches the Work Plan header and the objectives card of the Annual Review through `TutorialHint`. I did not touch the HUD or the Objectives panel (the UX candidate edits those files).

## Not changed (checked by diff and by offline assertion)

Completion logic (`EvaluateProgress`, `HasReviewedTwoCycleSequence`, `ShouldRecordAcknowledgement`), the eight objectives, `ScenarioManagementEvent` (event 9), save data/controller/validation, the acknowledgement handler, `RestoreSaveData`. No automatic acknowledgement and no tutorial state machine. `git diff eb7fd3d HEAD` is empty for `ScenarioOneObjectives.cs`, `ScenarioOneManager.cs`, `ScenarioManagementEvent.cs`, `ForestSaveData.cs`, `ForestSaveController.cs`, `ForestSaveValidation.cs`. `run_n3_offline.py` now also asserts that "Guidance" appears in none of `EvaluateProgress`, `RestoreSaveData` or `MarkAnnualReviewSeen`.

## Tests run (all offline, plain .NET / approximate type-check; **no Unity**)

- `Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py`: `N3_OFFLINE_PASS checks=88` (67 before, 21 new: every state, the boundaries from the U4 negative cases, the real acknowledgement handler moving state 2 to state 3, text content, non-prescription).
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py`: density 35, timber 71, history **74** (70 before, 4 new: hint unchanged without guidance, guidance replaces the history hint, stale orders and completion keep priority).
- `Tools/Verification/CloudTypeCheck/run_typecheck.py` `prod` and `tools`: no new errors against the baselines (7 and 43).
- `git merge-tree` against the UX candidate `549eae3` and the growth candidate `796da99`: no conflicts.

## Not verified

Rendering, the footer text in a real Annual Review, line wrapping and the HUD. The U4 qualification harness (`verify/scenario-one-two-cycle-eb7fd3d`) has a graphical phase that reads the Annual Review text; re-running it on a build that contains this commit is the check that the strings actually appear. Not run here, by instruction.
