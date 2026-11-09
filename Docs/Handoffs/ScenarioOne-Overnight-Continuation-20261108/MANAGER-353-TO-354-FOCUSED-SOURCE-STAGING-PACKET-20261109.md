# Manager #200 → #354 — Stage the accepted Scenario One sources into ONE reviewable candidate (no Unity)

**Date:** 9 November 2026  
**Classification:** PACKET ONLY. Source/diff integration staging, **not** an approved game integration or a new product specification.  
**Execution claim:** [#354](https://github.com/jermur788/CCF/issues/354) — **READY / NOT RUNNING**.  
**Dependency:** [#353 accepted Reference Preview fix](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-353-REFERENCE-PREVIEW-FOCUSED-ACCEPTANCE-20261109.md), [#334 source-only integration study](https://github.com/jermur788/CCF/blob/9c0228174e3a44756361619efd4dee804d9ec0ea/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/FOCUSED-INTEGRATION-AND-QUALIFICATION-READINESS-REFRESH.md), existing [#315 focused qualification](https://github.com/jermur788/CCF/issues/315) and [#316 independent review](https://github.com/jermur788/CCF/issues/316). All remain distinct.

## Why this is next

The Reference Preview player-pose correction passed exactly one focused Unity regression on its tested source; repeating B3 or reinvestigating the controller is unnecessary unless a new failure emerges. The remaining progress gate is to combine the already source-reviewed Workbench, five-rack Objective/map guidance and CTA verifier with that correction into **one exact inspectable candidate**. Use a staging-only task so expensive Editor qualification is not repeated on divergent feature branches.

This **does not** treat the prior failed #287 Workbench Tier-2 gate as PASS. That gate, native visual proof and the A–O integrated checklist remain pending. This task is permitted ahead of #287 qualification **only as isolated source staging and offline inspection**; #315's final integration and qualification admission conditions are not waived.

## Assigned task, capability, difficulty

**Purpose in plain English:** prepare one exact combined game candidate for subsequent targeted Workbench and integrated UI testing, without launching Unity or altering the running user's project.

**Difficulty: HIGH** — source branches diverge, scene and save compatibility are protected, and some source-stage checkers intentionally cannot treat a new combined scene as their original byte-identical stage.

**Best-suited executor:** T3 Code / OpenAI Codex GPT-6.1 Sol High, or an equivalently capable observed local Git/C#/Python worker. Record actual provider, model and effort; do not assume availability. **No separate orchestrator required for this one bounded task.**

**Single writable lane, conditional on real preflight:**
- Branch: `integration/scenario-one-focused-playtest-20261109-v1` (new, absent locally and remotely).
- Canonical-store Files worktree: `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, using `/media/jer/Files/CCF/Repository.git`.
- Start from EXACT `52bb32a6e36c7e0528f679cf97b3276a83537500` (the #353 report-only tip). Its source parent `b82cb2405fa245ed49e48eb363b4b664b1d85d5e` is the exact Unity-tested C2. Prove report successor is docs-only and production source matches C2 before staging any other changes.
- This is **NOT** `/media/jer/ZX20/CCF-U3`, the original dirty Workbench, an existing Unity lane or `main`.

## Inputs and exact transfer boundaries

1. **Existing Workbench and 1.1.1 environment:** C2 is an observed descendant of reviewed Workbench source `a4019e6b96b50e03d2`; GitHub comparison shows five additional commits and only the Reference Preview source, report, two package files and B3 verification files. Verify actual Workbench implementation + approved verifier and reviewed conditional Linux 1.1.1 manifest/lock lineage on C2. Reuse the existing content; do **not** cherry-pick Workbench again or silently adopt the package versions as canonical/main policy.
2. **D-072 Objective next-action, Stand Map headland and five-rack diagram:** source-approved branch `feature/scenario-one-next-action-guidance-v1`, exact pin `9789369d02963b4444f8530b071413d066bf1ddf`. The independent #310 review and Manager Stage B approval support SOURCE ONLY, not Unity render/native behaviour. Reconstruct the exact intended file changes through an explicit file-level allowlist and ancestry/diff manifest. Do not merge the whole divergent branch or unrelated authored history. Preserve old-save V2 guards and correct five-rack diagram semantics.
3. **CTA/source checker:** source-approved-with-disclosed benign inline-comment false-red limit from #330/#331/Manager #332. Tested source `990ec43ec6bf5ec7063fb530f80b82e37501c582`, publication `9867144c30d059433fb124aa016af6a5b20e1085`. Transfer only the required approved `Tools/Verification/OvernightPlaytestFeedback/` verification files/deltas; **do not merge PR #300 or its historical UI ancestry**. Explicitly preserve the accepted 884 negative/112 benign controls and known documented false-red, without launching a new adversarial mutation campaign.
4. **Protected:** frozen Reference Future v1 archive/identity, Legacy40, Enlarged80 Rack V2, Work Plan commitment, MFM proposal-vs-Fell distinction, Sitka/Oak/Beech accepted parity, original user saves and the accepted parallel five-rack layout. No content, hashes or expected results may be silently rebaselined.

## Work procedure

1. **Read-only preflight first:** fetch current live `Docs/Project/coordination/active-tasks.md` and GitHub #354/#315, inspect true remote refs and source commit identities, working Git store, and user/agent ownership. Confirm proposed branch/path are absent and the Files mount is safe. STOP on conflict or ambiguity, no force, stash, reset, cleanup or existing-worktree reuse.
2. Mark [RUNNING] on #354 with **actual** model/provider/effort, exact branch/worktree and starting full Git HEAD before any edits. Only #200 may change `active-tasks.md`.
3. Make a source/branch diff manifest, compare each proposed transferred path to authoritative source bytes and source review, and identify any overlapping source hunk **before** committing. The prior #334 no-conflict trial used older heads: repeat the relevant candidate-specific dry-run without treating it as proven.
4. Build one new isolated candidate on the new branch using only approved, traced source/test deltas. Do not rewrite history or move feature branches, user-test branches or `main`. If source hunks overlap but can be reconciled mechanically, record the exact reconciliation and preserve both accepted behaviours; if substantive interpretation is required, STOP and return to Manager.
5. **Offline only**, run narrowly relevant static/compile-independent verification for transferred sources. Show each exact command/exit, file counts and source hashes. Check the expected source and scene changes against allowed paths, and verify the C2 Reference Preview fix is retained verbatim unless a source overlap is explicitly resolved. Check Stage-specific source byte identity before transfer.
6. **Known qualification caveat from #334 D-1:** the original five-rack and headland stage-inventory checkers pin source/scene bytes from *before* Workbench scene changes, and therefore can fail against a legitimate combined tree. Do not weaken, rewrite, waive or misleadingly report those stage verifiers as globally PASS. Preserve their stage-specific original evidence and produce an exact candidate scene-geometry/allowed-delta witness proposal for Manager G4. CTA must be run against the new actual integrated Assets, not assumed green from the old branch.
7. Do **not** run Unity, acquire global lease, import packages, create a Library, launch native tests, make a Player build or qualify any A–O integrated gate. Zero broad source-security or B3 replay campaigns.

## Required handoff

Publish one Markdown report at exactly:

`Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/FOCUSED-SOURCE-STAGING-RESULT-20261109.md`

on the new candidate branch, containing:
- exact initial/base and resulting source + report commits, branch/worktree, clean/dirt status;
- every transferred source path with origin pin and whether byte-identical, mechanically reconciled or excluded, plus any conflict;
- C2 fix retained and Workbench/1.1.1 lineage, no unexpected scenario/scene/package changes;
- concise offline commands, expected versus actual results, protected history/Legacy40 source facts;
- stage-specific diagram/scene checker mismatch disposition with no weakened tests;
- zero Unity/Editor, zero lease, zero new native render or integrated A–O PASS;
- explicit **READY FOR MANAGER G4 SOURCE REVIEW** or **BLOCKED**, residual risks and exact one next verification recommendation.

Mark [HANDED-OFF] and close #354 after publication, or [BLOCKED] with an exact reason and no partial integration claim. Stop for Manager #200; no automatic #315 or #316 dispatch.

**Pass condition:** a traceable, bounded **source-staged candidate**, not a Unity-qualified, integrated/main or user-accepted release. No speculative tests or user permission to build.

## Copy/paste dispatch line

> T3 Code: Execute CCF [#354](https://github.com/jermur788/CCF/issues/354) exactly as the [Manager #353→#354 focused source-staging packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-353-TO-354-FOCUSED-SOURCE-STAGING-PACKET-20261109.md) specifies. Use one clean new Files canonical-store worktree and branch from the report-only C2 tip, transfer only approved guidance and CTA source/checker deltas, preserve Workbench and C2, perform offline checks only, publish the exact result, hand off and STOP for Manager. No Unity, lease, main merge, full checker campaigns or builds.

**User action required:** paste the dispatch into one T3 Code session. No product decision required at this stage.
