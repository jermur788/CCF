# Manager packet — #406 Part 5 readout source qualification and provenance repair

**Date:** 12 November 2026. **Task claim:** [#406 — READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/406). **Parent review:** [Manager #400 disposition](MANAGER-PART5-BROWSER-400-SOURCE-AND-DRAFT-SPEC-DISPOSITION-20261112.md). **Purpose:** turn the browser-only source creation into independently reproducible offline C# evidence, repairing only objectively confirmed provenance-identity defects. **Task class:** PACKET ONLY — bounded verification repairs, not a new gameplay feature. **Difficulty:** MEDIUM, because historical event/order identity, exact production C# binding, negative tests and old-save honesty matter, but no scene/Unity/session/UI/persistence authoring is authorised.

**Best provider/model:** OpenAI GPT-6.1 Sol Medium in Codex/T3 Code local lane; if unavailable, an equivalently capable local C# provider. **Task execution:** user must dispatch to T3 Code (copy/paste line at bottom); this GitHub issue does not launch a worker. **Manager:** overall review/coordination. **No active machine-wide Unity lease granted or needed.**

## Exact starting source and lane

- Source: `feature/scenario-one-management-followon-browser-v1-20261112` ([browser #400 return](https://github.com/jermur788/CCF/blob/feature/scenario-one-management-followon-browser-v1-20261112/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/BROWSER-PART5-MANAGEMENT-FEATURES-SOURCE-AND-SPECS-HANDOFF.md)). **Fetch and pin its actual HEAD on entry**, log full SHA and inspect divergence before work. Do **not** use Manager docs branch as production base, even though it has the helper in ancestry.
- Create a **new isolated source-only** branch `qualify/scenario-one-management-followon-readout-20261112-v1` at that exact source HEAD. Preferred *new* worktree `/media/jer/Files/CCF/Worktrees/CCF-ManagementFollowonReadout-Qual`, **conditional on real mount/free-space, no existing worktree/path or owner and a clean checkout**. If path occupied, stop and report rather than reuse or overwrite. No existing worktree or lease is reserved by this packet.
- At start (and before asserting a missing lock) read live `Docs/Project/coordination/active-tasks.md` and physical `CCF-Ops/board/active.md`, check current claims #402/#404/#382 and actual local ownership. #402/#404 own `ResearchCopyQual391` for Unity; #382 is the user's personal lane. They are strictly excluded. Do not switch/clean/stash/reset any unrelated dirty work.
- Mark #406 **[RUNNING]** with real model, branch/worktree, starting HEAD, disk/head/owner preflight before editing. If preflight impossible, **[BLOCKED]** with evidence, do not improvise a worktree.

## Writable paths (exclusive)

1. `Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` — only minimal confirmed source fixes; do not add new product behaviour.
2. `Tools/Verification/ManagementFollowonReadout/**` — focused assertions/negative fixtures/launcher documentation; do not weaken or delete existing tests.
3. `Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-MANAGEMENT-FOLLOWON-READOUT-SOURCE-QUALIFICATION-RESULT.md` — one durable GitHub Markdown result (new file).

**Forbidden:** all other Assets, manager/history/event producer/save/world/reference identity/Unity scene/material/meta/UI/MFM geometry/approval, original scenarios/tests, specs, main, integration and personal branch. No Editor, Unity lock, Windows/Player build or automatic main merge. If source correctness requires changes outside allowed paths, stop as **BLOCKED** and return a precise Manager request.

## Steps and acceptance

**A — independent compile.** Run `python3 -I Tools/Verification/ManagementFollowonReadout/run_offline.py` with actual dotnet, capturing command, SDK/runtime, exit and stdout; this compiles the real helper against test signatures, not Unity. Run production-signature typecheck using the repository's existing `Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py` where viable, with safe external disposable output. Check command applicability and managed references before treating any failure as a code defect. Explicitly distinguish test adapter compile from actual Unity runtime binding.

**B — adversarial evidence identity.** Preserve existing assertions and add negative fixtures for:
- a `WorkResolved` event ID duplicated by an `OrderCreated` / `YearAdvanced` event visible in `YearHistoryView.Events`, even when resolved-work IDs alone are unique: helper must not promote that work to `Verified` or `ExecutedCoverage` if the global identity is ambiguous. The reader may inspect complete `Events` but must never edit them.
- a retained `ScenarioOneWorkOrder` and resolution `ScenarioManagementEvent` with matched ID/type/status/year/prescription but **different recorded `targetTreeId`** (and available species if appropriate): fail closed to inconsistent/unknown linkage, not an executed-unit claim. Test old historical records where target information is genuinely absent: do not fabricate missing identities.
- a successful fell/salvage work record in a completed year with absent annual `HarvestSettlement` or absent report: no claim a settlement was recorded, no per-order invented money. If needed rename/split `GroupedHarvestMoney` semantics while keeping API impact inside this new helper and its tests.
- existing access rack order (`rackId>0`) vs MFM non-rack prescription felling with real verified order provenance (`rackId=0`, `prescriptionId>0`): the former never qualifies as substantive stand treatment; latter remains **candidate only**, never achievement.
- duplicate completed years, duplicate resolved order/event IDs, no-work years, exact year gaps, source scalar copies, natural/planted metric distinction, missing species and old no-prescription records; tests must not silently turn missing into zeros or infer historical per-rack ecology.

**C — mutation evidence.** Perform at least two bounded negative/mutation checks that demonstrably fail assertions when the global-ID and order-target guards are removed/weakened. Never edit another test suite to get green. Keep exact before/after source diff and clean attribution.

**D — report/stop.** Record PASS/FAIL/NOT RUN for every gate, command/process exit, source heads, branch/tree changes, actual tool versions, no Editor/Unity, source-typecheck limitations and compatibility implications. If all offline gates pass, report **SOURCE QUALIFIED OFFLINE**, not Unity qualified/merged. If not, **NEEDS CHANGES or BLOCKED** and do not integrate. Include actual old-save coverage boundaries and whether a genuine old-save fixture was used. Publish/read back result on GitHub and mark #406 **[HANDED-OFF]**, close claim. Manager then independently reviews and selects next UI/MFM implementation gates.

## Exact return

`Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-MANAGEMENT-FOLLOWON-READOUT-SOURCE-QUALIFICATION-RESULT.md`

## Paste-ready dispatch — user action: send to T3 Code / Sol

> Execute CCF GitHub issue #406 (https://github.com/jermur788/CCF/issues/406) using the exact Manager packet https://github.com/jermur788/CCF/blob/docs/manager-part5-source-review-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-406-MANAGEMENT-READOUT-SOURCE-QUALIFICATION-PACKET.md . Use GPT-6.1 Sol Medium, a NEW isolated clean source-only worktree/branch derived from the actual browser #400 source HEAD, no Unity or cross-owner edits. Read live coordination and physical Ops board before running; mark #406 RUNNING with real preflight. Execute offline helper compile, production-signature checks, global event ID/target/harvest provenance negative tests and bounded mutations without weakening tests. Publish the specified GitHub result, hand off and close #406. Do not touch Fable #402/#404, personal #382, main or any UI/save/MFM production code. Stop and report any ownership or scope violation.
