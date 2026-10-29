# MFM-06 Integration Task Packet

Parent: #91 PLAYER-TEST-OVERNIGHT-01
Claim: #113
Lane: ChatGPT Code / local T3 execution
Status: READY FOR DISPATCH

## Purpose

Integrate the independently approved MFM-06 v23 save/provenance foundation onto the current Scenario One umbrella integration lineage and prove that the merged candidate preserves save/load, Reference, Legacy and deterministic compatibility.

## Difficulty

High. This is a shared save-schema and integration boundary with post-merge Unity qualification.

## Preferred model/provider

Default: Claude Sonnet 5.5 Medium for bounded integration/qualification if capacity is available. Escalate to Claude Opus 5.5 Low if merge/save conflicts require deeper reasoning. OpenAI 6.1 Sol is suitable for difficult debugging or independent review, but is not a mandatory gate.

## Exact reviewed input

Production candidate: `13fd85452b8d3a55946fcb459af8b00e0326a579`
Branch: `feature/mfm-v1-save-provenance-v23`
Worktree: `/media/jer/Videos/CCF-MFM-06`
Historical starting umbrella base: `ac876a64969752a3864034521f42559671264b30`

The candidate was implemented under Claude Sonnet 5.5, reviewed independently by Claude Opus 5.5 Low, fixed after a first NEEDS CHANGES verdict, and round 2 APPROVED. The Manager accepts that independent review for integration.

## Before touching integration

1. Refresh #91, #106, #111, #113 and live `Docs/Project/coordination/active-tasks.md`.
2. Refresh all relevant worktrees and the actual current `integration/scenario-one-player-test-ready-v1` HEAD. Do not assume `ac876a6` is still current.
3. Verify clean state and single-writer ownership.
4. Record actual executor/model/provider, integration worktree, branch and starting HEAD in #113.
5. Refresh Unity lock state before any Unity action.

## Integration scope

Integrate exactly the reviewed MFM-06 production change, or a proven docs-only descendant. Preserve v23 ownership and all reviewed compatibility behavior. Do not absorb unrelated feature branches or stale history.

Two reviewed repairs are protected:
- objective-history changes must not leak into prescription-world hash identity;
- reason code `0` must remain unambiguous as unset/default rather than also being the only valid reason.

No MFM-07 proposal/approval behavior is implemented in this task.

## Required post-merge qualification

Run on the exact merged umbrella candidate:
- Unity import/compile;
- MFM-06 save/load/no-mutation gate;
- focused malformed-v23 rejection-before-mutation cases;
- hash dump against the correct pre-MFM-06 umbrella baseline;
- frozen Reference Future compatibility;
- Legacy/pre-feature save compatibility;
- interaction and N3, or the current canonical equivalents;
- any other focused gate needed to prove the merge did not alter the reviewed MFM-06 behavior.

If `progress` or `density-save` again reports a pass marker while the runner returns `pass:false`, investigate enough to classify it. Do not silently treat a real regression as green, but do not expand scope without evidence.

## Required handoff

Publish `Docs/Handoffs/MFM-06/INTEGRATION-HANDOFF.md` with:
- exact pre-integration umbrella SHA;
- exact reviewed input SHA;
- exact post-integration umbrella SHA;
- merge/cherry-pick method;
- production diff summary;
- post-merge gate outputs;
- protected hash/Reference/Legacy result;
- clean/lock state;
- any residuals.

Success token:

`MFM-06 V23 INTEGRATED — READY FOR MFM-07`

Stop if save-version ownership, protected compatibility, unrelated dirty work, or merge conflict cannot be resolved without changing accepted behavior.

No Windows build.

## Copy/paste dispatch

Run claim #113 from `Docs/Handoffs/MFM-06/INTEGRATION-TASK-PACKET.md`. Integrate exact reviewed MFM-06 candidate `13fd85452b8d3a55946fcb459af8b00e0326a579` onto the freshly inspected Scenario One umbrella head, then run the bounded post-merge Unity/save/hash/Reference/Legacy qualification on that exact merged candidate. Preserve the two reviewed repairs and do not start MFM-07 in this task. Publish `Docs/Handoffs/MFM-06/INTEGRATION-HANDOFF.md`. No Windows build.