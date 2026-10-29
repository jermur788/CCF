# Sequential teaching Tier 2

Use the normal U1 assignment, clean exact HEAD, machine-wide Unity lock and ZX20 caches/isolated configuration. No builds. Follow the existing Qualification runner and native-input workflow.

Offline: `python3 -B Tools/Verification/ScenarioOneUx/run_ux_check.py --out /media/jer/Files/CCF/Verification/<task>/offline-ux` includes production pure sequence rules and the original optional practice/copy checks. Rack geometry: `Tools/Verification/ScenarioOneRackUx/OfflineLogic/run_rack_check.py`.

Graphical gate: run `Tools/Verification/Qualification/run_qualification.py` with method `GuidedOnboardingVerification.Begin`, marker `GUIDED_ONBOARDING_PASS`, `--interactive --window 1920x1080`, and these `--file` arguments:

- `Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs`
- `Tools/Verification/ScenarioOneRackUx/RackUxShots.cs`
- `Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs`
- For V2, also `Tools/Verification/RackV2NoCompetingAuthor/GuidedRackV2Verification.cs` and `Tools/Verification/RackV2NoCompetingAuthor/RackAuthoringNativeInput.cs` (required: absence fails the V2 gate).

Pass environments `CCF_SEQUENCE_VERIFY=1`, `CCF_STAND_GEOMETRY=1`, `CCF_GROWTH_MODEL=2`, and an explicit `CCF_RACK_VERIFY_MODEL=legacy` or `v2`. Run BOTH model branches for RACK-V2-08 qualification. The legacy branch loads an explicit pre-v21/model-0 Enlarged80 compatibility fixture; geometry1 alone starts V2. A genuine fresh Legacy40 native-authoring/year-progression check is separately provided by `RackV2NoCompetingAuthor/run_gate.py --gate legacy40`. This extends the existing bounded native-pointer fixture with real world-order actions for each new teaching transition, cancellation, review chronology and save/load round trips. Without the companion file, the original guided gate still compiles and runs independently through an optional partial-method hook.

The native phase waits for real OS input; fixture positioning is a repeatable vantage, not a natural-spawn beginner playtest. For model 0 deliver B → choose start → aim → right-click/reselect → Esc cancel → B/reselect → confirm. V2 instead attempts the removed authoring routes with OS input, asserts no tool/plan/work mutation, exercises ordinary marks/inspection, and reaches the real Stand Map Plan Racks/confirm controls. Its progression reads the authoritative confirmed V2 teaching frame. See `RackV2NoCompetingAuthor/README.md` for the staged runners and approved G3 mapping. Telemetry and screenshot markers let the driver synchronize. Native origin/forward-only metrics are checked independently by the fixture. A private Xephyr display is the documented Linux input workflow. Never inject input into the user's normal desktop.

The extended fixture then performs crop selection, nearby competitor planning/cancel/replan, retained-deadwood release, annual review, planting with both shelter choices, later-year observation and ongoing synthesis via existing production APIs. Some APIs are used directly to bound fixture duration; this does not claim manual end-to-end play. It checks source-goal IDs/targets, schema v21, world hashes before/after presentation and save/load at year boundaries. It captures each state at 1280×720 and 1920×1080 and checks title/concept/action/progress bounds and text wrapping at both sizes.

Re-run original teaching-copy, MenuTutorial and ScenarioOneCompletion verification through the qualification runner. Current-model completion uses `CCF_GROWTH_MODEL=2` and the original gate's legacy geometry 0; it is a protected-behaviour regression, not a claim of natural play of the new curriculum. Do not run historical reconstruction or broad Tier 3 for this task.

Inspect and preserve all raw results; view the captures. Restore only diff-inspected, known Editor settings/SectionFive material rewrites byte-exactly, after Editor exit. Remove staged disposable source/meta using the runner; release the normal Unity lock and U1 after checking clean status. Handoff records exact source HEAD, receipts, coordination commit and explicit backup.
