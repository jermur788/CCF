# Sen-1 #890 — onboarding entry predicate: source result (code only)

**Date:** 2026-11-20. **Owner instruction:** one-file repair inside the previously granted path, no Unity.
**Status:** SOURCE IMPLEMENTED / OFFLINE TYPE-CHECK PASSED / UNITY NOT RUN / NOT INTEGRATED / NOT OWNER-ACCEPTED.
**Issue:** [#890](https://github.com/jermur788/CCF/issues/890).
**Supersedes:** the withdrawn `bb1c5cec` one-line change (reverted at `2aae107f`), and the source review of that attempt.

## Exact source identity

- Base: #886 `81e65590fe15832cfc95c74ef0f02fcfb91d10d1`.
- Branch: `fix/sen1-890-onboarding-entry-predicate-20261120-v1`.
- Production commit: `1b197c69`.
- **Sole changed production file:** `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` — the same file the previous grant covered. No second production file, no scene, prefab, asset, `.meta`, package, save or settings change.

## What changed and why

`ScenarioOneUiRoot.Start()` previously required `FindFirstObjectByType<StandaloneSessionMenu>() == null` before it would show the introduction. That test cannot work for Compact30:

- `StandaloneSessionMenu.Bootstrap()` creates a persistent host in **every** scene unless `CCF_TEST_DIRECT_ENTRY=1`.
- `StandaloneSessionMenu.Open()` needs a `ForestSaveController`; Compact30 has none, so in Compact30 the host never opens, logs `CCF_STARTUP_SCREEN_UNAVAILABLE` after its 600-frame wait, and stays present.

So both the withdrawn edit and any "remove the host test" variant are inert: the host object exists throughout.

The repair keeps the same intent but tests whether the standalone screen can **actually offer** the introduction:

```csharp
bool startupScreenAvailable = FindFirstObjectByType<StandaloneSessionMenu>() != null
    && FindFirstObjectByType<ForestSaveController>() != null;
if (!startupScreenAvailable && manager != null
    && manager.CurrentEcologicalYear == 0 && manager.WorkOrders.Count == 0 && manager.AnnualReports.Count == 0)
    help.ShowScenarioIntroduction();
```

In Compact30 no save controller exists, so a fresh session reaches the existing `ShowScenarioIntroduction()` from the real `CCF → Player Test → Play Compact30` entry. Page components, copy and `Resources/Onboarding/page-{1,2,3}.png` are untouched and reused exactly as they are.

## Behaviour matrix (source-reasoned, not runtime-observed)

| Context | Before | After | Double-open? |
|---|---|---|---|
| `CCF → Player Test → Play Compact30` (no env var) | no introduction | introduction on fresh session (year 0, no orders/reports) | no — host cannot call Start New without a save controller |
| Compact30 with `CCF_TEST_DIRECT_ENTRY=1` | no introduction (host absent) | introduction | no |
| Standalone `ForestTest` normal Play | introduction from Start New | unchanged | no — host + save controller present, guard false |
| Standalone `Continue` on a saved forest | dismissed/not replayed | unchanged | no — `ContinueSavedForest` still calls `DismissScenarioIntroduction` |
| `MixedSpeciesTest` / Reference Future preview | unchanged | unchanged | no — save controller present |

## Verification actually performed

- `git diff 81e65590 1b197c69`: one production file; 10 insertions, 1 deletion.
- `git status` clean in the task worktree apart from the commit; no generated `.meta`.
- Offline Roslyn type-check of all non-Editor `Assets/**` sources against Unity 6000.6.0f1 references (`Tools/Verification/ScenarioOneCompletion/P3/OfflineLogic/run_offline_check.py`): **`GAME_ASSEMBLY_OK errors=0`**, `CHECK_ASSEMBLY_OK errors=0`, `P3_OFFLINE_PASS`. This type-checks the changed file in the real assembly context; it does not execute it.
- Source checks: `ForestSaveController` is serialized in `Assets/Scenes/ForestTest.unity` (guid `760d3ace7f61434adadef80ffdacb9a0`) and absent from `ScenarioOneCompact30.unity`; `ecologicalYear` is serialized `0` on a fresh runtime-created `ForestEcologyController`; `InitializeNewScenario()` clears work orders and reports.
- Harness review: the Compact harnesses (`Sen1CompactCollision`, `Sen1CompactE2Reuse`, `Sen1CompactPlaytest`, `Sen1MarkerScale`) drive APIs directly and disable/move the player themselves, so the modal should not block them — this is reasoning, not a run.

## NOT performed

Unity 6000.6.0f1 import/compile, Editor Play, rendered pages, real input isolation, save/Continue, Legacy40/Enlarged80, determinism, or any performance check. No Unity lease, no Editor, no save or PlayerPrefs touched.

## One correction to the earlier review

The first review claimed movement/inspect/marking were not blocked while the onboarding is open. That was wrong: `ScenarioOneUiRoot.Update()` syncs `manager.SetAuxiliaryPanelOpen(... || help.IsOpen ...)`, and `ApplyPanelInputState()` disables `ForestPlayer` and frees the cursor each frame. The remaining item is only the 0-1 frame window between `ShowScenarioIntroduction()` in `Start()` and the first `Update()`. Treat it as a runtime observation, not a known defect.

## Exact downstream checks (one authorised native Unity worker, separate step)

1. Default environment, no `CCF_TEST_DIRECT_ENTRY`: `CCF → Player Test → Play Compact30`, focus Game view. Page 1, 2, 3 appear in order with the existing art; Next/Back/Begin work at 720p and 1080p; no blank overlay, stall or second copy.
2. Before Begin, attempt WASD/Shift/Space, mouse look, E/left-click, X/C, Tab, M/O, F2/F3/1-4, J/Y/N: no player movement/look and no gameplay mutation. On Begin, walk/H1/marking/Work Plan resume, cursor re-locks.
3. Standalone: fresh Start New shows the introduction once; a supported Continue does not replay it; no change to existing help or learning preferences.
4. Confirm the dormant session host in Compact30 produces no visible overlay at any point before or after the pages.
5. Re-run the disposable Compact harnesses (`Sen1CompactCollision`, `Sen1CompactE2Reuse`) and at least one #876 marker check, since a modal now exists for their first frames.
6. Preserve #886 collision/stump, #876 markers, 175 Sitka, four fixed G1 sites, no-rack rule, Legacy40/Enlarged80, protected saves, frozen Reference Future v1 and determinism.

## Residual risk to record

- The predicate infers "no title screen" from "no save controller". If Compact30 gains save/load later, the introduction would silently stop again; the code comment says so and the predicate must be revisited then.
- The introduction is session-only by design, so every fresh Compact30 launch shows three pages. That is the intended teaching behaviour, but it adds three clicks to repeated owner visual tests of #891.

## Boundaries respected

No art, PNG, copy, theme, scene, prefab, persistence, package, platform-build or main-history change. `Compact30Bootstrap.cs`, `StandaloneSessionMenu*.cs`, `ForestPlayer.cs` and `ForestTreeMarkingManager.cs` were **not** edited. #891 art ownership is untouched and no Unity lease was taken.
