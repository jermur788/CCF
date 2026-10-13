# Required gates (results of the run that counts)

| Gate | Display | Result | Notes |
|---|---|---|---|
| ScenarioOneTeachingCopyVerification (`teaching-batch-1`) | private | **PASS** | 63.5 s |
| ScenarioOneTeachingCopyVerification (`teaching-batch-2`) | private | **PASS** | 38.6 s |
| ScenarioOneTeachingCopyVerification (`teaching-rendered`) | private | **PASS** | 72.1 s |
| MenuTutorialVerification (`menu-standalone-1`) | private | **PASS** | 101.7 s |
| MenuTutorialVerification (`menu-standalone-2`) | private | **PASS** | 91.9 s |
| ClearanceVerification (`clearance`) | private | **PASS** | 81.8 s |
| ScenarioOneRemovalVerification (`removal`) | private | **PASS** | 48.2 s |
| CropTreeCompetitorVerification (batch x2 + interactive) | private | **PASS** | determinism hash F58FB0B1A421D28B, identical in all 3 runs |
| WorkPlanResidualVerification (batch x2 + interactive) | private | **PASS** | determinism hash AF7405918195F0DE, identical in all 3 runs |
| S1B/SessionMenuVerification | desktop (batch-safe) | **PASS** | marker S1B_SESSION_MENU_VERIFY_PASS |
| StartupScreenVerification, batch | desktop | **PASS** | |

StartupScreenVerification, rendered: FAIL, pre-existing and identical without the patch (see `startup-gate-diagnosis.md`).
WorkPlanResidualVerification: all assertions pass; its determinism hash is `AF7405918195F0DE`, not the recorded anchor `F7C2FC966816BBAD`, because the hash covers the rendered Work Plan text and the patch rewrote that card's subtitle.
