# Phase 3: disposable merge rehearsal (growth + U4 + UX), result

**Disposable. Not an integration candidate. Nothing published; the temporary branch `tmp/rehearsal-u4-gr-ux` was deleted after the evidence was captured.**

Merged on a temporary branch, from the U4 verify head `b8f9570`: growth source `796da99` and the already-reviewed UX candidate `549eae3318f1bb4d01a2853384b58f282f7f4e73`. Density was not included. Rehearsal head `1a4d019` (scratch).

## Textual / semantic conflicts

- `git merge` of the growth head: clean. `git merge` of the UX head: clean (auto-merged `ScenarioOneManager.cs` and `AnnualReviewView.cs` only). **0 conflicts.**
- Both intended edits are present in the merged text: U4's footer condition (`ui.NeedsAnnualReview || m.NeedsCompletionReviewAcknowledgement`) and the UX candidate's `MenuHelpView.AnnualReviewReadPrompt` footer string.
- The UX candidate rewrote `TutorialHint`; it still reads `NextStepHint`, so the U4 guidance branch (`task/scenario-one-completion-guidance-v1`) flows through it. `git merge-tree` of that branch with `549eae3` and with `796da99` is also conflict-free.

## Offline gates on the merged state (plain .NET / Python, not Unity)

N3 rules 67, density 35, timber 71, history 70, UX offline check 176 (`SCENARIO_ONE_UX_PASS`), growth lab 360 rows: all PASS. The approximate type-check reports one artefact: the UX qualification harness needs `UnityEngine.InputSystem.LowLevel`, which the checker does not stub, and it stops the checker before it reports the usual 43 baseline errors. It is not a Unity compile result; the Unity run below compiled and ran the merged tree.

## Unity gates on the merged state (batch except the UI phase; real Unity 6000.6.0f1, the U4 worktree's Library)

| gate | result |
|---|---|
| compile (script reload of the merged tree, inside the first gate) | PASS, 0 `error CS` |
| U4 `core` (112 hard checks: shallow path, four valid two-cycle paths, negatives, failed terminal, history, reload) | **PASS 112/112**, 1 open presentation finding (unchanged: the guidance branch is not in this merge) |
| U4 `ui` (graphical, legacy-save footer, acknowledgement click) | PASS 8/8 |
| `ScenarioHistoryVerification` | PASS (39 assertions) |
| shipped `N3CompletionVerification` | PASS |
| `BroadleafTimberEconomyVerification` | PASS (29 assertions) |
| `MixedDensityVerification`, Legacy40 | PASS (22 checks) |
| my `OakBeechQualification` rows phase (kernel law, regimes, promotion, release direction; default model) | PASS (66 checks); kernel difference 0.0 cm / 0.0 m over 8 700 tree-years per species |

What this shows about the combination: with the calibrated Oak/Beech rows, the reviewed two-cycle completion still completes at year 25 on the same plans (eight objectives met, `eight=True`), and the shallow path still does not. The merged-state world hash of the shortest valid path at year 25 is `009588E5ACCB4F8F` (U4 alone: `572025058B227E92`); it differs because the calibrated broadleaves grow differently. It is an observation, not an anchor.

## Not run on the merged state

The 24-gate regression, the P2/P3 gates, the UX candidate's own rendered gates (`UxQualification`, 25 states x 3 sizes, startup), and the density lane. The earlier "two-cycle completion pushes completion later, planted broadleaves die out under the canopy" concern was not tested beyond year 25.

Evidence: `evidence/merge-rehearsal/` (results JSON per gate, markers, `evidence-core.txt`, `evidence-rows-merged.txt`, the driver's `reh-summary.txt`).
