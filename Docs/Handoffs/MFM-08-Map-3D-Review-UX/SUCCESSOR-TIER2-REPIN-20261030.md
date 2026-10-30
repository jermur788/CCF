# MFM-08 — Successor Tier-2 Repin After Geometry Repair 01

**Date:** 2026-10-30  
**Parent feature claim:** #156  
**Repair claim:** #192 — Manager APPROVED / ready to close  
**Status:** **RELEASED FOR FULL TIER-2 AFTER FRESH PREFLIGHT**

## Exact candidate

Qualify exact repaired production head:

`e5fb424635f16182748efcb0539e8a49539495a9`

This is one bounded repair commit on top of the previously returned MFM-08 candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06`.

The repair handoff/docs successor is `e8b9e55e0a76ae74f46b801362ccdfed8a680c3d`; it contains no additional production change.

Current Scenario One integration remains:

`ce6737146a7aa9ea953d7db0ca93b103f6c2c996`

Do not integrate MFM-08 before this successor Tier-2 receives Manager APPROVED.

## Verification harness

Use the updated MFM-08 Tier-2 harness from:

- branch `qual/mfm-08-unity-v1`
- head `630c6c3db3c67d7e75cf06bd8d0661deb1e2e5c6`

The harness retains the two defect assertions from the returned run. One obsolete comparison was corrected from “area equals whole unit polygon” to the accepted segment-cut/teaching-footprint expectation. Do not weaken or remove those checks.

## Difficulty / best executor

- **Difficulty:** 4/5 — full runtime/rendered qualification plus cross-feature regressions and protected compatibility.
- **Preferred executor:** Ultra Code / strongest Unity-capable local verification worker.
- **Mode:** qualification only; no production repair authority.

## Start gate / lane

At actual start:

1. refresh #156, live `Docs/Project/coordination/active-tasks.md`, current integration and this repin;
2. verify exact candidate `e5fb4246...` in a clean registered Unity lane;
3. confirm no conflicting owner on the MFM-08/Stand Map surface;
4. recheck free space, Editor processes and the authoritative `unity-lock` status;
5. acquire the machine-wide Unity lock normally;
6. only after acquire, post `[RUNNING — UNITY]` on #156 with provider/model, lane/worktree, exact candidate, harness head and lock receipt.

The prior free-state receipt from repair #192 is historical only and does not reserve the lane.

## Required full Tier-2 matrix

Run and evidence all of the following on exact repaired candidate `e5fb4246...`:

### MFM-08 live gate

- complete updated MFM-08 Unity gate, expected 353 checks or a larger non-weakened successor;
- repaired Low/Centre/High footprints remain distinct;
- source sample remains the fixed teaching footprint;
- tags do not collapse/stack because of shared whole-unit geometry;
- `Inspect in forest` uses the repaired area-specific centre;
- 1280×720 and 1920×1080 rendered states are legible;
- Previous/Next bounded and non-wrapping;
- real `[C]` Crop Tree confirmation and `[N]` reject/reopen;
- no map confirm-all/bulk confirmation;
- Prepare creates prescription/snapshot only;
- proposals remain explicitly noncommitted and not Fell marks;
- waypoint alone does not satisfy the required 3D spot-check;
- real 3D inspection does satisfy the gate using fresh authoritative coverage;
- approval creates ordinary Pending work with provenance;
- Work Plan remains the commitment gate;
- repeated prescription approval is refused.

### Conflict coverage

- retain the manual-Fell conflict witness;
- additionally exercise an **open-work conflict** explicitly and prove it is held/surfaced without overwriting player/work state.

### Deferred authoritative regressions

Run the smallest current authoritative Unity regressions for:

- Rack V2 map/runtime/field behavior relevant to this surface;
- MFM-06 save/provenance;
- MFM-07 prescription approval boundary;
- relevant Scenario One UI/navigation gate;
- frozen Reference Future v1 / protected hash-anchor compatibility;
- any required Legacy40 compatibility check exercised by the current MFM/Rack gate set.

Retain the green offline MFM-04/05/06/07/08A/08 evidence; rerun if the qualification harness or candidate requires a fresh boundary receipt.

## Clean-return requirements

Record exact commands/results, capture manifest, protected/reference receipts, save/world restoration, Editor-process cleanup, lane cleanliness and normal Unity-lock release.

No production repair is permitted under this qualification run. A genuine defect stops with exact evidence and returns to Manager.

## Handoff

Update/publish:

`Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`

with a clearly separated successor Tier-2 section and exact repaired candidate/harness lineage.

Required success token:

`MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW`

No Windows build.

## Copy/paste dispatch

`Resume MFM-08 #156 full Tier-2 from Docs/Handoffs/MFM-08-Map-3D-Review-UX/SUCCESSOR-TIER2-REPIN-20261030.md. Qualify exact repaired production head e5fb424635f16182748efcb0539e8a49539495a9 using qual/mfm-08-unity-v1 harness head 630c6c3db3c67d7e75cf06bd8d0661deb1e2e5c6. Fresh-preflight a clean registered Unity lane, active-tasks, free space, Editor processes and authoritative unity-lock; acquire normally before posting [RUNNING — UNITY]. Qualification only: run the complete repaired 353-check MFM-08 gate at 1280x720 + 1920x1080, explicit open-work plus manual-Fell conflicts, Rack V2, MFM-06/07 Unity, relevant UI and frozen Reference Future/protected compatibility, then clean save/world/lane/lock return. Do not fix production or weaken tests under #156. Publish the successor HANDOFF.md section and return MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW or the exact blocker. No Windows build.`
