# Sen-1 #890 — Manager corrected-source review and focused Unity gate

**Date:** 2026-11-20 (after 22:18Z owner source report).  
**Task:** Restore approved existing 3-page Scenario One introduction to the real `CCF → Player Test → Play Compact30` Editor path.  
**Disposition:** **APPROVED FOR FOCUSED NATIVE UNITY QUALIFICATION ONLY** / SOURCE IMPLEMENTED / OFFLINE COMPILE AUTHOR-REPORTED PASS / UI RUNTIME NOT VERIFIED / NOT INTEGRATED / NOT OWNER-ACCEPTED. This is a manager source-readback disposition, not an Editor review.  
**Canonical issue:** https://github.com/jermur788/CCF/issues/890.  
**Author's complete direct source result:** https://github.com/jermur788/CCF/blob/fix/sen1-890-onboarding-entry-predicate-20261120-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261120/Sen-1-890-ONBOARDING-ENTRY-PREDICATE-SOURCE-RESULT-20261120.md.

## Verified source identity

- Parent = #886 tree collision candidate `81e65590fe15832cfc95c74ef0f02fcfb91d10d1`.
- Author's isolated branch = `fix/sen1-890-onboarding-entry-predicate-20261120-v1`; production commit `1b197c69`, handoff branch head owner reports `33c5e0ba`.
- Independent manager readback from GitHub branch: `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` blob `3306955128be28a9fa23999e517137fb076c24e0`; handoff blob `a4c47a4072c39f5e34b2bb192e3762ea7ea3e9ee`.
- GitHub compare from #886 to branch returned ahead 2, behind 0, **one modified production file (8 added, 1 removed lines)** and exactly one added Markdown handoff. No other C# code, scene/prefab/material, `.meta`, package or save change in comparison.
- `ScenarioOneUiRoot.Start()` retains the fresh-year-empty-work/reports predicate and changes generic menu-host exclusion to:
  ```csharp
  bool startupScreenAvailable = FindFirstObjectByType<StandaloneSessionMenu>() != null
      && FindFirstObjectByType<ForestSaveController>() != null;
  if (!startupScreenAvailable && manager != null
      && manager.CurrentEcologicalYear == 0 && manager.WorkOrders.Count == 0 && manager.AnnualReports.Count == 0)
      help.ShowScenarioIntroduction();
  ```
- Direct source check: the startup menu creates a DontDestroyOnLoad host at AfterSceneLoad; its `Open()` requires the save controller. On the actual no-save Compact30, the extra save dependency makes `startupScreenAvailable` false despite host presence. Existing `StandaloneSessionMenu.StartNewScenario()` expressly invokes `ShowScenarioIntroduction()` and `ContinueSavedForest()` dismisses it; in a normal saved standalone scene, both host and save controller mean the root does not independently invoke it. This resolves the independent review's actual Compact entry objection *at source level*. The source change does not alter page copy or art.
- The author's local handoff reports offline Roslyn full non-Editor assembly against Unity 6000.6.0f1 references: `GAME_ASSEMBLY_OK errors=0`, `CHECK_ASSEMBLY_OK errors=0`, `P3_OFFLINE_PASS`. **Manager did not run or inspect the native harness logs**; classify as credible author report, not independently reproduced compilation.

## Residual risks, no inflated claims

1. **Actual appearance and lifecycle remain UNKNOWN** until Unity Editor launch. A menu-host timeout and Unity UI Toolkit ordering are not proven by static inspection alone. Inspect no blank overlay or double-open.
2. **Pre-Begin input isolation must be tested.** The prior review's blanket claim that movement/marking are unblocked was incorrect: `ScenarioOneUiRoot.Update()` synchronizes `manager.SetAuxiliaryPanelOpen(... || help.IsOpen ...)`, which calls `ApplyPanelInputState()` to disable `ForestPlayer` and unlock cursor; existing `MenuHelpView.ShowScenarioIntroduction()` sets `ModalOpen`. The first Start→Update frame may still leak input due lifecycle timing; test directly rather than declaring it broken or correct.
3. **Future Compact save policy:** No save controller is now deliberate. A future save/load addition could change this predicate and accidentally suppress Compact onboarding; require review when an approved save spec adds a controller.
4. **Repeated visual trials:** Every fresh Compact Editor play opens the three pages intentionally. Owner #891 art trial therefore needs three clicks, not a bypass unless owner explicitly chooses one; avoid changing product experience for convenience.
5. No runtime/Editor Unity test, no owner acceptance, no production integration, no Windows build. No source cross-branch cherry-pick from previously rejected `bb1c5cec`.

## Focused single-lane native Unity qualification — next packet (READY, NOT CLAIMED)

**Purpose:** prove the exact corrected source candidate's onboarding as the owner experiences it, not perform a broad Tier-3 regression. **Difficulty:** LOW–MEDIUM. **Provider:** one authorised native Unity 6000.6.0f1 worker (Sol/Codex or separately approved local worker), **only after** live #891 art Unity custody/lease is free; in a task-owned isolated candidate with exact #886 source + approved `1b197c69` delta, not the user's protected personal Editor checkout. Start by refreshing real `Docs/Project/coordination/active-tasks.md`, local CCF-Ops D-091 physical locks, actual #891 status, branch/HEAD and dirty inventory. Do not claim a lock from GitHub issue comments.

**Checks in one minimal Editor session:**

1. Open default Editor entry `CCF → Player Test → Play Compact30` **without** `CCF_TEST_DIRECT_ENTRY`; verify existing 3 pages actually render, with existing PNGs and Back/Next/Begin, and no hidden/stalled overlay/double-open.
2. While page 1/2/3 is open attempt WASD/mouse-look, E/click, X/C and Tab/M plus one Compact teaching key: no movement, marking, inspection or state mutation. Once Begin is chosen, cursor and movement resume; walk to H1 and perform one marking interaction. If any first-frame leak, stop and report exact repro rather than silently broadening source path.
3. Short existing-world negative: standalone Start New opens once, Continue does not reopen; check Compact remains no racks and #886 tree collision still sensible. Do not modify any save or PlayerPrefs in the owner's protected checkout; use isolated fixture where necessary.
4. Check zero compile/import errors and concise evidence (log + screenshots and exact SHAs); mark **UNITY QUALIFIED** only for the behaviors actually observed. Stop on compile error, unexpected modal/duplicate, save interference, asset/write conflicts or active Editor lease.

**Explicit exclusions:** no art imports/textures (#891 separate), scene/prefab edits, SaveController creation, persistence, simulation formulas, tests weakened for green, broad harness/Windows builds, main merge, owner personal save/Prefs. A failing check becomes a narrow new source packet with separately authorised path(s). **Owner tests the corrected visual candidate before any independent visual redesign review.**

**Handoff expectation:** one direct GitHub Markdown author result with real Unity logs/screenshot pointers/branch/commit and lease-release evidence, and manager assesses whether it can be combined later with #891 by a single integrator. No actor was launched, no Unity lock claimed, and this document is not an approval to execute while #891 holds an active lease.

## Action

- Manager #425: recognise #890 as SOURCE IMPLEMENTED / GITHUB COMPARED / AUTHOR-REPORTED OFFLINE COMPILE PASS, with **focused Unity qualification pending**; coordinate the minimum native run after #891 lease free and future integration of one exact source commit. Do not accept earlier failed `bb1c5cec`.
- #887/#891 art: remain on your approved distinct visual file set. Do not open the protected #890 source worktree or merge from it to simplify art testing.

**Direct continuation handoff:** https://github.com/jermur788/CCF/blob/docs/sen1-890-corrected-source-manager-disposition-20261120-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261120/Sen-1-890-MANAGER-CORRECTED-SOURCE-REVIEW-AND-FOCUSED-UNITY-GATE-20261120.md
