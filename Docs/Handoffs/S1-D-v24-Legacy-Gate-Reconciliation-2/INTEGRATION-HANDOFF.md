# S1-D v24 Legacy Gate Reconciliation 2 — Integration Handoff

**Status:** APPROVED / INTEGRATED  
**Claim:** #165  
**Pre-integration umbrella:** `185200f57140c65ed4d540ff0c4d173451170d14`  
**Reviewed worker tip:** `508c2d07741f97c47d9ed008306e30eb70666f72`  
**Integration result:** fast-forward to `508c2d07741f97c47d9ed008306e30eb70666f72`

## Manager verdict

**APPROVED.** The branch is verification-only and is rooted directly on the prior integration tip. The v23→v24 expectation edits preserve all non-version assertions. The `progress` and `density-save` isolated-save allowlist additions are accepted because those gates explicitly call `ForestSaveController.Save()` for their own isolated disk round trips; host-save invariance, cleanup, harness removal, pass-marker, exit-code and lane-clean requirements remain unchanged.

No production, save/schema/validator/hash/reference, scene/prefab/package/ProjectSettings files changed. No anchor or baseline was rebaselined.

## Accepted qualification evidence

On exact PARITY-08 candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`, after the bounded verification repairs:

- compile: PASS / exit 0;
- progress: `SCENARIO_ONE_PROGRESS_VERIFY_PASS` / exit 0;
- completion: `SCENARIO_ONE_COMPLETION_VERIFY_PASS` / exit 0;
- completion-model1: `SCENARIO_ONE_COMPLETION_VERIFY_PASS` / exit 0;
- density-save: `DENSITY_QUAL_PASS` / exit 0;
- Legacy40 protected density anchors remained `7E57B9DAEF5BF4D0/35E2BF1C2F55C2DE`;
- host saves unchanged and staged harness/meta removed in every accepted receipt.

Earlier failed/hung runs remain preserved and are not counted as passes. The clean reruns are the accepted receipts.

## Residuals

Other stale v23 verification pins remain outside this claim and should be repaired only when their owning gates are scheduled or through a separately bounded systematic sweep. This integration does **not** qualify PARITY-08 by itself.

PARITY-08 still requires its explicit residual Unity evidence for positive model-0 compatibility and protected Sitka timber anchors, plus independent review #128, before integration.

No Windows build is authorised.
