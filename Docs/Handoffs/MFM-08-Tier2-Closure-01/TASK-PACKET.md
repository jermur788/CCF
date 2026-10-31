# MFM-08 Tier-2 Closure 01 — Task Packet

**Claim:** #195  
**Parent feature:** #156  
**Mode:** bounded verification closure; no production repair authority  
**Difficulty:** 2/5 — targeted Unity regression + lineage diagnosis  
**Best-suited worker:** Claude Code / Sonnet or Sol-class local Unity verification worker

## Purpose

Close the one material qualification gap left by the returned MFM-08 successor Tier-2 and determine whether the disclosed Rack V2 red gates are still present on the current Scenario One integration lineage.

This task is not allowed to repair production or weaken verification.

## Exact refs

- MFM-08 repaired candidate already qualified: `e5fb424635f16182748efcb0539e8a49539495a9`
- Returned #156 verification head: `qual/mfm-08-unity-v1` @ `d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727`
- Original feature start / inherited-red baseline: `cfdc93bf3943125d21570bc5749a9338f52d1cce`
- Current Scenario One integration: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`
- Manager review: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/SUCCESSOR-TIER2-MANAGER-REVIEW-20261031.md`

Suggested branch: `verify/mfm-08-tier2-closure-v1`.

Use a clean registered Unity lane only after fresh live ownership, free-space, Editor-process and authoritative lock checks. A previous free receipt does not reserve the lane.

## Required work

### A. Missing direct qualification gate

On exact `e5fb4246...`, run the repository's authoritative **RackV2MapPlanning** Unity regression without altering production or weakening the runner/gate.

Record command, exact gate source/head, result token/check counts, evidence path and clean return.

If it fails on the MFM candidate, stop and return the exact failure unless a comparison to the exact feature start can be performed without changing production.

### B. Current-lineage Rack disposition

On exact current integration `ce673714...`, run the repository's current versions of:

- `guided-v2`
- `combined-v2`
- `combined-legacy`

This is diagnostic evidence only.

The returned #156 worker already established:

- candidate `e5fb4246...` combined-v2 = 447 pass / 11 fail;
- exact feature start `cfdc93bf...` combined-v2 = 447 / 11;
- candidate and `cfdc93bf...` guided-v2 fail the same annual-advance assertion;
- candidate combined-legacy = 440 / 8; `cfdc93bf...` = 416 / 32.

Do not rerun those old-baseline comparisons unless needed to resolve contradictory evidence.

If `ce673714...` is green, identify the later lineage evidence that plausibly explains the correction (commit/file-level evidence is enough; do not reverse-engineer unrelated systems).

If `ce673714...` is red, report exact failures and stop. Do not repair them under #195. Manager will create a separate Rack V2 repair claim if necessary.

### C. Preserve accepted MFM evidence

Do **not** rerun the full MFM-08 373-check gate by default. Its accepted evidence remains valid on exact `e5fb4246...`.

Only rerun it if a new finding creates a material ambiguity about that exact candidate.

## Must not change

- any `Assets/ForestPrototype/` file;
- Rack V2 implementation or persistence;
- MFM production code;
- save/schema/validator/hash/reference data;
- accepted gameplay/calibration thresholds;
- scenes/prefabs/packages/project settings;
- test expectations merely to obtain green;
- Windows build state.

Temporary staged verification files are allowed only through the normal gate runners and must be cleaned before return.

## Handoff

Publish:

`Docs/Handoffs/MFM-08-Tier2-Closure-01/HANDOFF.md`

Include:

- worker/provider/model;
- branch/worktree/lane;
- exact start and all exact refs executed;
- lock receipt and final release state;
- RackV2MapPlanning result on `e5fb4246...`;
- current-lineage results for guided-v2 / combined-v2 / combined-legacy on `ce673714...`;
- any relevant commit/file evidence explaining lineage differences;
- changed-file boundary proving no production edits;
- exact evidence paths;
- whether #156 can now be upgraded to Manager APPROVED.

Success token:

`MFM-08 TIER-2 CLOSURE PASS — READY FOR MANAGER REVIEW`

If blocked or red, return the exact gate/failure instead of the success token.

## Copy/paste dispatch

Run MFM-08 Tier-2 Closure 01 from `Docs/Handoffs/MFM-08-Tier2-Closure-01/TASK-PACKET.md` under claim #195. Fresh-preflight a clean registered Unity lane and acquire the authoritative lock. On exact MFM-08 candidate `e5fb424635f16182748efcb0539e8a49539495a9`, run the authoritative RackV2MapPlanning Unity regression. Then, diagnostically on exact current integration `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`, run current guided-v2, combined-v2 and combined-legacy to determine whether the Rack failures reported from old base `cfdc93bf...` still exist on the live integration lineage. Do not repair production, weaken tests, rerun the full MFM-08 gate unless materially necessary, or build Windows. Publish `Docs/Handoffs/MFM-08-Tier2-Closure-01/HANDOFF.md` and return `MFM-08 TIER-2 CLOSURE PASS — READY FOR MANAGER REVIEW` or the exact blocker.
