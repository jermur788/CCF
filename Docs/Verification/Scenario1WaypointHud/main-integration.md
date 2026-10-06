# Main integration decision

APPROVED by user after interactive waypoint smoke, with explicit instruction to push to main if ready (2026-10-06).

Main fast-forwarded from `691dd18a56c0da21cb08909e22ac0d0625d556b1` to tested gameplay commit `b3fa29dd242842efb1f54ee7fed7dd3d044778a4`. Remote main was confirmed at the same base before integration. No merge conflict or source alteration; this integrates the UI Toolkit redesign, clearance correction/preview, menu teaching, self-paced learning and HUD waypoint follow-ups already on the tested task branch.

Post-integration rendered MenuTutorialVerification passed on `/home/jer/CCF-main` with Unity 6000.6.0f1. Total launcher time 495.87 seconds includes rebuilding the main worktree asset cache. A previous cold-import attempt was killed by its 8 GiB memory cap before Play; its diagnostic is retained under ignored Build/ClearanceVerification/attempts/main-cold-import. The retry completed without gameplay/source fixes. All three resolutions, waypoint arrow/arrival/clear, menus, lesson progress, annual-review acknowledgement/save reload and local preference restoration passed. Results: main-integration-results.json.

Existing simulation and frozen Reference gate results are recorded in Scenario1LearningObjectives; main uses exactly that tested underlying simulation source. This publication does not claim another full ecology/reference run. No packages, save schema or canonical project context changed during integration. No temporary verification scripts/meta remain in Assets. Local settings, seven plant materials and Assets/_Recovery files were preserved and excluded. Four verification-generated fungi import differences were restored after the disposable Editor exited.

User smoke passed before integration; ready for publication to origin/main. Coordination and this record are documentation-only changes after the verified gameplay commit.
