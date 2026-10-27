# Sequential teaching Tier 2

Use the normal U1 assignment, clean exact HEAD, machine-wide Unity lock and ZX20 caches/isolated configuration. No builds. Follow the existing Qualification runner and native-input workflow.

Offline: `python3 -B Tools/Verification/ScenarioOneUx/run_ux_check.py --out <ZX20 scratch>` includes production pure sequence rules and the original optional practice/copy checks. Rack geometry: `Tools/Verification/ScenarioOneRackUx/OfflineLogic/run_rack_check.py`.

Graphical gate: run `Tools/Verification/Qualification/run_qualification.py` with method `GuidedOnboardingVerification.Begin`, marker `GUIDED_ONBOARDING_PASS`, `--interactive --window 1920x1080`, and all three `--file` arguments:

- `Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs`
- `Tools/Verification/ScenarioOneRackUx/RackUxShots.cs`
- `Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs`

Pass environments `CCF_SEQUENCE_VERIFY=1`, `CCF_STAND_GEOMETRY=1`, `CCF_GROWTH_MODEL=2`. This extends the existing bounded native-pointer fixture with real world-order actions for each new teaching transition, cancellation, review chronology and save/load round trips. Without the companion file, the original guided gate still compiles and runs independently through an optional partial-method hook.

The native phase waits for real OS input; fixture positioning is a repeatable vantage, not a natural-spawn beginner playtest. Deliver B → choose start → aim → right-click/reselect → Esc cancel → B/reselect → confirm. Telemetry and screenshot markers let the driver synchronize. Native origin/forward-only metrics are checked independently by the fixture. A private Xephyr display is the documented Linux input workflow. Never inject input into the user's normal desktop.

The extended fixture then performs crop selection, nearby competitor planning/cancel/replan, retained-deadwood release, annual review, planting with both shelter choices, later-year observation and ongoing synthesis via existing production APIs. Some APIs are used directly to bound fixture duration; this does not claim manual end-to-end play. It checks source-goal IDs/targets, schema v20, world hashes before/after presentation and save/load at year boundaries. It captures each state at 1280×720 and 1920×1080 and checks title/concept/action/progress bounds and text wrapping at both sizes.

Re-run original teaching-copy, MenuTutorial and ScenarioOneCompletion verification through the qualification runner. Current-model completion uses `CCF_GROWTH_MODEL=2` and the original gate's legacy geometry 0; it is a protected-behaviour regression, not a claim of natural play of the new curriculum. Do not run historical reconstruction or broad Tier 3 for this task.

Inspect and preserve all raw results; view the captures. Restore only diff-inspected, known Editor settings/SectionFive material rewrites byte-exactly, after Editor exit. Remove staged disposable source/meta using the runner; release the normal Unity lock and U1 after checking clean status. Handoff records exact source HEAD, receipts, coordination commit and explicit backup.
