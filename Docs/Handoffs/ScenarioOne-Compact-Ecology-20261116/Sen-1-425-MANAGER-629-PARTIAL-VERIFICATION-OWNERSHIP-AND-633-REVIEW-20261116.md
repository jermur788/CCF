# Sen-1 #425 — #629 source evidence verdict, ownership exception and next independent review

**16 November 2026; Overall Manager GPT-6, GitHub/browser only.** Inspected the exact [#629 implementation/handoff](https://github.com/jermur788/CCF/blob/sen-1/629-l1-geometry-dispatch-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-629-L1-EXHAUSTIVE-GEOMETRY-DISPATCH-SOURCE-HARDENING-RESULT-20261116.md), its three C# source files, [#629 issue](https://github.com/jermur788/CCF/issues/629), #544 and #424 ownership, and GitHub comparison from the unmerged save-line baseline. **Verdict: APPROVED AS AN EVIDENCE-BASED SOURCE-ONLY CANDIDATE FOR INDEPENDENT REVIEW; NOT APPROVED FOR INTEGRATION/UNITY OR AN EXHAUSTIVE THIRD-GEOMETRY SOLUTION.** Candidate retains **IMPLEMENTED / PARTIALLY VERIFIED — NOT UNITY-VERIFIED**.

## What changed and checks

Candidate child branch `sen-1/629-l1-geometry-dispatch-20261116-v1` is rooted in unmerged shared save profile line. It adds three C# modifications plus one durable Markdown:
- `StandGeometryModel.StandSizeMeters` explicitly dispatches known Legacy40→40m and Enlarged80→80m; unknown throws; `CellIndex` refuses unknown with -1.
- `StandWorldBoundary.Apply` refuses unknown before mutating the applied world; `ForestStartingStand.Generate` logs and returns for unknown.
- `IsKnown` and new-game geometry still allow **only 0/1**, reserved Compact=2 remains unavailable. No scene, geometry-2, asset, saved-data or historical content patch.

Author reports **100/100 differential stubbed tests pass** for known/unknown, 50 base mismatches on deliberately unsafe cases, 4/4 author mutants killed (five additional reviewer mutants), and **zero Roslyn typecheck errors** under player/Editor defines. Same-family fresh reviewer APPROVED WITH NOTES; it identified a previously surviving mutation, later fixed in the harness. **Manager directly read GitHub report and source but did not execute external local logs**, so these are *reported offline evidence*, not independently reproduced or proof of native Unity compile/saves.

**Scope judgement:** three-file patch may be appropriate now if all remaining positive-only tests are genuinely guarded or feature-off. The original #626 grep inventory of ~18 files is not, by itself, an instruction to edit every file. Different-family review needs to check `ForestSaveValidation.cs:221` reorder fragility, unknown error branch and positive-only behaviour before deciding.

## Process deviation — no retrospective normalisation

The issue explicitly required #424 and #544 source-owner signoff, plus an actual [RUNNING] claim and live board entry before writing. Author says the project owner responded **"I approve both"** during the local run and used a clean *child branch* rather than edit the owner save-line worktree. That is evidence of stated direct owner permission, but **does not document a pre-existing live board registration or single-writer handoff by #424/#544**. No #424/#544 approval comment was posted and the physical #553 board remained stale. **Do not backdate** any approvals or say the worker complied with that prerequisite. Preserve the source branch intact; require a **forward-looking** ownership and integration ruling from #424 and the save-line owner #544 before incorporation, plus physical machine/board admission for native follow-up.

## Exact next task

[**#633 cross-family review**](https://github.com/jermur788/CCF/issues/633), **READY / NOT RUNNING**. **Difficulty MEDIUM**, because only three source files changed but callers and source/test validity matter. **Preferred:** GPT-6.1 Sol High / Codex High in Linux local read-only review, different model family from Claude author and first reviewer. Proposed reviewer branch `review/sen-1-633-l1-geometry-cross-family-20261116-v1`, exact worktree/starting HEAD must be registered by actual worker; review only one Markdown at `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-633-INDEPENDENT-629-L1-SOURCE-AND-OWNERSHIP-REVIEW-20261116.md`. Inspect three-file diff, actual offline evidence and caller graphs, return separate technical vs ownership/integration verdicts; no source edits, Unity lease, merge or Windows build.

**Paste-ready dispatch:** Take CCF Scenario One #633 via https://github.com/jermur788/CCF/issues/633 . As a *different-family independent reviewer*, inspect the three-source-file L-1 unknown-geometry hardening on the unmerged save-line child, check the offline harness/negative controls and the validation-order/feature-off call paths. Record technical approval separately from the missing pre-edit #424/#544/board registration; do not mark Unity-verified or merge the source. Register actual model, isolated review branch/worktree, starting HEAD, and publish the one named Markdown result.

## Hold / next implementation conditions

No native Unity Tier-2 until clean exclusive Linux lane/Library/Prefs and CCF-Ops #548/553 board/lease admission, confirmed host ownership and one short old-world E80/Legacy40/frozen Reference/save regression at exact candidate. No Compact2 `IsKnown` change, no startup picker/scene activation, no new prefab or art. #622 source geometry, #625 gameplay choices, #624 D-083 main review remain separate. Source-only work need not idle behind unresolved final art.

**User action:** only dispatch independent review; no new owner gameplay decision for L-1 itself. **Dashboard/Drive:** not verified.
