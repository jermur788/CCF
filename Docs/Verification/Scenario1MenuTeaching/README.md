# Part N menu teaching evidence

Base: clearance branch commit `8c5380dd95b397281bedc9bd153a69086b9aad9f`. Task branch: `task/scenario-one-menu-tutorial`. Context: `691dd18a56c0da21cb08909e22ac0d0625d556b1`. No merge/integration approval. Human mouse-input and beginner-comprehension smoke remains pending.

## Beginner questions answered

| Question | Contextual introduction |
| --- | --- |
| Where do I inspect an individual tree? | Tree Inspection: E, size/growth/competition, DBH, mark meanings and E to return. |
| Where do I look for spatial patterns? | Stand Map: light, regeneration, browsing/protection and Fell/Crop layers. |
| How do I navigate to an area? | Map: select cell → read information → Set waypoint → close → HUD direction/distance → walk and inspect. No remote forestry. |
| Where do I approve planned work? | Work Plan: review tasks, executors, labour/materials, timber income, contractor minimum and approval; year advance executes. |
| Where do I see what happened after time advances? | Annual Review: WORK DONE, MONEY, FOREST; outcomes and what to inspect next. |
| Where do I see current objectives/progress? | Walking HUD: year, cash, objective progress, ground/tree information and active waypoint. |

Each introduction is contextual and appears once per local player/device. Help/F1 remains available. Tree Inspection uses contextual HUD Help/F1, preserving its existing layout. Escape closes Help first; the forest cannot receive that same-frame action. Opaque teaching panels prevent underlying map/review text showing through.

## Playthrough and boundaries

`MENU_TUTORIAL_PLAYTHROUGH_PASS` verifies first-use HUD and inspection, map waypoint without teleporting, Work Plan and first real Annual Review, the locked next year before acknowledgement, unlocked subsequent cycle after acknowledgement, unread/acknowledged save/load, learned preferences, repeat suppression, F1/Escape and Reference isolation.

All five introductions have rendered/bounds assertions at 1280×720, 1600×900 and 1920×1080 (15 help captures). Four additional captures show actual inspection, waypoint, results and locked Work Plan. Full-size visual checks cover each introduction at the smallest size, larger map/review panels and the underlying screens. These are scripted captures, not a claimed human-input acceptance pass.

Keyboard events are queued through the installed Input System. UI button behavior is tested through the production registered Clickable callback via reflection: synthetic mouse/navigation events did not reach the runtime panel in this Editor fixture. This verifies the callback's world/UI effect, not physical mouse delivery. Manual mouse smoke is therefore explicit. No production input workaround was added.

Initial fixture compile and click-delivery diagnostics are retained locally under ignored `Build/ClearanceVerification/attempts/`. An Editor Search indexing exception occurs in some graphical logs; it is outside game code. Early debug-colour captures were replaced by final captures with Game-view gizmos disabled. No forest asset/style change was made for teaching.

## Selected screenshots (15.1 MiB)

- [Walking HUD introduction](WalkingHud-1280.png)
- [Tree Inspection introduction](TreeInspection-1280.png)
- [Stand Map introduction](StandMap-1280.png)
- [Work Plan introduction](WorkPlan-1280.png)
- [Annual Review introduction](AnnualReview-1280.png)
- [Actual waypoint navigation information](map-waypoint.png)
- [Actual first annual results and acknowledgement](annual-results-unread.png)
- [Further year advance locked until review](work-plan-review-gate.png)

[Implementation record and manual smoke](../../Scenario1MenuTeaching.md). [Machine-readable gate markers and verified source hashes](results.json) record the final results. Current full logs and all captures remain under ignored `Build/ClearanceVerification/`.

Reproduce in this isolated worktree with Unity 6000.6.0f1:

```sh
python3 Tools/Verification/run_clearance_gate.py MenuTutorialVerification --interactive --capture
```

The launcher stages only the requested disposable script and removes it/meta on exit. Verification uses its own configuration/save slot and Library, two workers and bounded memory. The fixture restores world, save/backup/temp files and menu preferences. Save schema v15, ecology, economics, existing simulation APIs and Reference Future v1 remain unchanged.

## Gate results

| Gate / mode | Status | Seconds |
| --- | --- | ---: |
| Import (default) | PASS | 505.95 |
| MenuTutorialVerification (default-rendered) | PASS | 82.65 |
| ScenarioOneInteractionVerification (default) | PASS | 69.89 |
| ScenarioOneCompletionVerification (default) | PASS | 58.27 |
| ScenarioOneCompletionVerification (model0) | PASS | 63.81 |
| SaveHardeningVerification (default) | PASS | 51.47 |
| ScenarioReferenceVerification (default) | PASS | 68.94 |

Model-1 completion: `00479F18970F9926`; model-0 completion: `568922E1A6D73CDD`. Canonical neutral lifecycle: `BFC55473C1506067`; historical continuation: `9CDF21A541C5968D`. Frozen Reference year 100: `7AD177B3CC2F73C7`; archive FNV: `1A42C7BD0439E72F`. All pinned anchors are unchanged. The final UI playthrough and model-0 completion include the final Escape guard; the other regressions precede that keyboard-only guard. No biology or economy change followed those regressions.
