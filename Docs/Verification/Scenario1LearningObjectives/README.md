# Learning-objectives verification

Base `466456f7148498a611da56695401bf58da606a16`; context `691dd18a56c0da21cb08909e22ac0d0625d556b1`; task branch `task/scenario-one-menu-tutorial`. Unity 6000.6.0f1. All five final gates compiled and ran with the final runtime source; source and log hashes are in [results.json](results.json).

| Gate | Result | Evidence |
| --- | --- | --- |
| Menu / learning playthrough | PASS, 66.36 s | Queued M opens only the map while aiming at a tree; X toggles Fell; O opens/closes objectives. Cell and four layer callbacks record map progress; waypoint creation does not teleport; a current-cell waypoint does not credit travel. Arrival/site inspection, explicit reading acknowledgement, profile reload, forest save/load, later-year operation progress and Reference isolation pass. |
| Scenario lifecycle | PASS, 42.57 s | Actual stock, pending/approved work and annual-report state replace obsolete numbered-hint assertions. The existing lifecycle checks pass. |
| Model-1 completion | PASS, 57.32 s | `00479F18970F9926` unchanged. |
| Frozen Reference | PASS, 69.19 s | Archive schedule `56C8B99FA1E8DDD1`; year-100 world `7AD177B3CC2F73C7`; continuation `9CDF21A541C5968D`, unchanged. Tampered archive negative control detected. |
| Model-0 completion | PASS, 57.33 s | `568922E1A6D73CDD` unchanged. |

The eight topics have 42 sub-objectives. The fixture completes actual planning, approval and successful results for felling, pruning, sheltered planting and clearance, records retained deadwood and a site visit, and leaves unacknowledged reading steps incomplete in later years. It backs up/restores the forest/save files and all 42 learning preference keys plus the five existing help preference keys in an isolated verification configuration.

The existing five contextual menu introductions render within bounds at 1280×720, 1600×900 and 1920×1080. The new learning panel and its scroll viewport are checked at all three sizes. Selected captures were inspected for readable text and reachable controls:

- [Map objective at 1280×720](learning-map-1280.png): simple tasks and their explanations, with more content reachable by scrolling.
- [Deadwood objective at 1920×1080](learning-deadwood.png): parent/sub-objective progress, material-choice explanation and explicit acknowledgement, alongside later-year completed actions.

Full captures and logs remain locally in ignored `Build/ClearanceVerification/`. Earlier fixture attempts failed an aiming precondition at a stand-edge/base target; one disposable fixture compile failed because Cursor was ambiguous between two Unity namespaces. The final fixture captures the cursor and aims at a central trunk; its diagnostic ray hits Trunk and queued M/X checks pass. Failed attempts are retained locally under `Build/ClearanceVerification/attempts/` and are not acceptance passes. The unneeded inspection-layout proposal was reverted; only its displayed Fell key changes in this follow-up.

Button callbacks are exercised directly through the existing test helper. This does not certify physical mouse delivery or beginner comprehension. The arrival check positions the player fixture to test destination detection; the map itself is checked not to move the player. Human movement/mouse/readability review remains required. See [manual smoke steps](../../Scenario1LearningObjectives.md).

Reproduce with Unity closed, from the task worktree:

```sh
python3 Tools/Verification/run_clearance_gate.py MenuTutorialVerification --interactive --capture
python3 Tools/Verification/run_clearance_gate.py ScenarioOneProgressVerification ScenarioOneCompletionVerification ScenarioReferenceVerification
python3 Tools/Verification/run_clearance_gate.py ScenarioOneCompletionVerification --rng 0
```

No forest save schema, ecological/economic rule, scenario success threshold or frozen Reference change. Four test-import material normalizations were restored to their pre-test source; the user's seven material and editor-setting changes remain unstaged and preserved. Main is unchanged. These are task-branch checks, not an integration approval.
