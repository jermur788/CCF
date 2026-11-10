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

## #367 review-priority consumer repair (10 November 2026)

Owner OpenAI GPT-6.1 Sol / High, Codex harness in T3 Code, direct user dispatch. Same clean source lane/branch;
base `94486d9b7641c6b6dbd8f50491517be7795aee4f`, source context `63044dfbbeb869996ceecec8a47619dcba048b0b`.
This is PACKET ONLY. Production allowlist is exactly `LearningObjectivesView.cs`, `ScenarioOneRackBayText.cs`,
`ScenarioOneUiRoot.cs`. Presentation only; original sequence/scoring/acknowledgement/economics/save/geometry untouched.

Verification: `Checks.cs` remains byte-identical (all 160 old checks). `run_checks.py` now also runs `consumer_paths.py`
with `ConsumerPaths.cs`: exact source property/card statements and the later-window reason/target branch are compiled
with plain-data fixture inputs and label sinks. This tests actual consumer references without a Unity process. It is not
full Unity view/event execution: TeachingFacts/lesson selection and layout are fixture inputs; the unchanged Objective1
navigation branch is source-pinned separately. The original seven typed mutants and HUD source negative still run.
The new typed `review-first-omits-newest-unread` must fail specifically at
`R367_LATEST_UNREAD_EARLIER_ACK_CURRENT_SHORT_REVIEW_FIRST`; three new source negatives pin callsites.
No test-generated file belongs under Assets. Use durable Files paths for every output, e.g.:

```sh
receipt=/media/jer/Files/CCF/Verification/ScenarioOnePlayerFeedback-367-Repair-20261110
python3 -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out "$receipt/compile"
CCF_FEEDBACK_SCRATCH="$receipt/scratch" python3 -B Tools/Verification/PlayerFeedbackOvernight/run_checks.py "$receipt/compile/bin/Release/netstandard2.1/check.dll" "$receipt/checks" "$PWD"
CCF_FEEDBACK_SCRATCH="$receipt/scratch" CCF_FEEDBACK_COMPILE="$receipt/compile" CCF_FEEDBACK_MUTANTS="$receipt/mutants" python3 -B Tools/Verification/PlayerFeedbackOvernight/mutations.py
```

Baseline adverse-case reproduction (requires the preserved three pre-repair UI files under the receipt):

```sh
python3 -B Tools/Verification/PlayerFeedbackOvernight/consumer_paths.py "$receipt/compile/bin/Release/netstandard2.1/check.dll" "$receipt/before-consumers" "$PWD" --baseline-ui "$receipt"
```

Expected baseline exit 134 with the named R367 current-short failure; corrected consumers exit 0. The baseline mode
uses actual original UI bodies with compatibility properties only for new probe APIs, not to inject a passing priority.
No Unity, lock, scenery, save, preference, licensed asset, frozen Reference, main merge or Player build action authorised.
