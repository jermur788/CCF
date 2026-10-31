# MFM-08 MapPlanning Contract Reconciliation 01 — Task Packet

## Purpose

Repair only the stale RackV2MapPlanning navigation-tab expectation exposed by MFM-08 Tier-2 Closure 01, then rerun the complete authoritative MapPlanning gate on the exact MFM-08 production candidate.

This is a **bounded verification-contract repair**, not a production feature change.

## Authority

Manager review: `Docs/Handoffs/MFM-08-Tier2-Closure-01/MANAGER-REVIEW-20261031.md`.

Claim: **#196**.

Exact production candidate: `e5fb424635f16182748efcb0539e8a49539495a9`.

Historical feature start used by #195: `cfdc93bf3943125d21570bc5749a9338f52d1cce`.

## Problem established by #195

RackV2MapPlanning stops at check 17 on the exact-five navigation-tab assertion. The accepted MFM-08 V2 Stand Map adds the `Treatment review` navigation layer. The original five navigation tabs must remain valid; the new layer is additive.

The feature-start gate passes 142/142, so the repair must preserve that historical/non-MFM contract rather than globally replacing an expected count.

## Allowed edits

Verification harness/test files required to express this exact contract only.

Do **not** edit:

- production/UI code;
- scenes/prefabs/assets;
- save schema or fixtures unrelated to this assertion;
- calibration/economy/ecology;
- specs/Decision Log;
- protected expected values or anchors unrelated to navigation-tab composition.

## Required contract

The revised verification must prove all of the following:

1. the original five navigation tabs still exist in the MFM/V2 state;
2. each retains its intended navigation role;
3. `Treatment review` exists as an additional authorised navigation layer in the MFM/V2 state;
4. it does not replace or rename one of the original five;
5. the non-MFM/historical state still requires the original five-tab behavior where the harness can express that state;
6. no unrelated RackV2MapPlanning check is removed, bypassed or weakened.

Do not implement this as a blind count-only `5 -> 6` edit.

## Execution

- **Difficulty:** Medium — small diff, but verification weakening would create false confidence and the result requires serialized Unity.
- **Preferred provider/model:** Claude Code / Sonnet 5.5 Medium or equivalently capable local Code/Unity worker.
- **Branch:** `verify/mfm-08-mapplanning-contract-reconcile-v1`.
- **Intended Unity lane/worktree:** `/media/jer/ZX20/CCF-U1`.
- **Starting production candidate:** `e5fb424635f16182748efcb0539e8a49539495a9`.
- **Claim:** #196.

Before edits/Unity, refresh `Docs/Project/coordination/active-tasks.md`, mark #196 `[RUNNING]` with provider/model, branch/worktree and starting HEAD, confirm the lane is clean, fresh-preflight the authoritative Unity lock and acquire normally before Unity.

A prior free-lock receipt is historical only.

## Verification

1. Review the pre-patch assertion and MFM-08 tab construction/read path.
2. Make the minimum state-aware verification-only patch.
3. Show the exact diff and explain why it cannot hide loss/renaming of one of the original five tabs.
4. Run the complete authoritative RackV2MapPlanning gate on exact production candidate `e5fb4246...` plus only this patch.
5. All checks must execute and pass. If another failure appears, stop and return the exact blocker; do not widen scope automatically.
6. Record compile/import status required by the gate/harness.
7. Restore/revert Editor side effects, release the Unity lock normally and return the lane clean.

The full 353/373 MFM-08 gates do not need repetition unless a new material ambiguity is discovered.

## Handoff

Publish:

`Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/HANDOFF.md`

Include:

- start and final SHAs;
- exact changed files/diff summary;
- old vs new contract explanation;
- full MapPlanning check count/result;
- compile/import receipt if applicable;
- lock/lane cleanup receipt;
- any residual caveat;
- no-production-change statement.

At handoff mark #196 `[HANDED-OFF]`, link the handoff and close the execution claim.

## Success token

`MFM-08 MAPPLANNING CONTRACT RECONCILED — READY FOR MANAGER REVIEW`

## Copy/paste dispatch line

`Run MFM-08 MapPlanning Contract Reconciliation 01 from Docs/Handoffs/MFM-08-MapPlanning-Contract-Reconciliation-01/TASK-PACKET.md under claim #196. Start from exact production candidate e5fb424635f16182748efcb0539e8a49539495a9 on verify/mfm-08-mapplanning-contract-reconcile-v1. Repair only the stale RackV2MapPlanning navigation-tab contract: retain/assert the original five tabs, add state-aware assertion of the authorised V2 Treatment review tab, and preserve the non-MFM five-tab contract. Fresh-preflight/acquire Unity and run the complete authoritative RackV2MapPlanning gate. No production edits, no unrelated test weakening, no Windows build. Publish the required HANDOFF.md and return the exact success token or blocker.`
