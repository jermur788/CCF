# #363 source allowlist

Owner OpenAI gpt-6.1-sol / medium; branch feature/scenario-one-player-feedback-overnight-20261110-v1; reused clean registered FocusedSourceStage worktree. Resume HEAD 58f694ab; game base fb599eec, report successor 8c49460d.

Allowed: Assets/ForestPrototype/UI/*.cs, UI/Map/*.cs, UI/Resources/ScenarioOneUi.uss; focused Tools/Verification/PlayerFeedbackOvernight; this handoff folder. New UI pure helpers and their meta only. Read-only presentation, no changes to scenario predicates, authoritative geometry, marking/approval actions, Work Plan quote calculation, MFM, saves, scenes, packages, assets or reference fixtures.

Protected baseline: /tmp/ccf363-preflight/protected-before.json (17,350 SHA-256 entries covering all tracked source/U3/original Workbench files, ignored U3 Assets, Unity save/preferences directories). No Unity run or lock acquired. Phase 5 requires fresh admission; absent explicit #361 release, lock and save isolation proof => NOT RUN.

## #365 repair addendum (10 November 2026)

Owner for #365: Anthropic Claude Fable 5 (`claude-fable-5-2`), same branch and worktree, starting HEAD 9cbe8be4 (source bfd8e2a4).

Production files changed by #365, all already in the #363 allowlist above: `LearningObjectivesView.Sequence.cs`, `ScenarioOneRackBayText.cs` (pure HUD word selector), `TeachingDiagramPlan.cs`, `Map/StandMapLegend.cs`, `Map/StandMapCanvas.cs`, `ScenarioOneUiRoot.cs`, `LearningObjectivesView.cs`, `WorkPlanView.cs`, `LearningObjectivesView.Curriculum.cs`.

Verification files: `Checks.cs` (the original 67 assertions untouched, new ones appended), `run_checks.py`, `mutations.py`, `pixel_check.py` (scratch folder can be set with `CCF_FEEDBACK_SCRATCH`), new `hud_contract.py` (source-text contract, not a rendered HUD).

Reproduce from the repository root, always with `python3 -B`:

```sh
python3 -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out /tmp/ccf365-compile
CCF_FEEDBACK_SCRATCH=/tmp/ccf365-scratch python3 -B Tools/Verification/PlayerFeedbackOvernight/run_checks.py /tmp/ccf365-compile/bin/Release/netstandard2.1/check.dll /tmp/ccf365-checks
CCF_FEEDBACK_SCRATCH=/tmp/ccf365-scratch python3 -B Tools/Verification/PlayerFeedbackOvernight/pixel_check.py
CCF_FEEDBACK_COMPILE=/tmp/ccf365-compile CCF_FEEDBACK_MUTANTS=/tmp/ccf365-mutants python3 -B Tools/Verification/PlayerFeedbackOvernight/mutations.py
```

No Unity run, no lock, no save access.
