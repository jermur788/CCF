# PARITY-08 independent review precheck — before final Unity residuals

**Claim:** #128  
**Target:** #126 exact candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`  
**Status:** PRECHECK ONLY — FINAL VERDICT DEFERRED UNTIL RESIDUAL UNITY EVIDENCE

## Browser-review findings so far

The exact pushed implementation is suitable for independent static review now. The candidate is rooted from accepted v24 integration `185200f5` and changes the save-validation path to reject unknown authoritative species IDs before world mutation, through the shared `FindLoadProblem` path used by both `CanLoad` and `LoadData`.

Published evidence already supports:

- new offline species-validation suite 23/23 with mutation controls;
- current v24 MFM provenance compatibility;
- clean Unity import/compile;
- clean SaveHardening unknown-species rejection/no-mutation witness;
- save/provenance 159;
- hash dump byte-identical to accepted baseline;
- frozen Reference Future v1 PASS;
- v24 teaching-latch round trip PASS;
- approval/N3/Legacy40 interaction PASS;
- after #165, clean progress, model-1 completion, model-2 completion/density-save and protected Legacy40 density anchors.

No parity-specific schema field, anchor rebaseline or Reference Future change is reported.

## Why final #128 verdict is deferred

The binding PARITY-08 packet requires positive model-0/model-1/model-2 compatibility and protected Sitka timber anchors. The implementation handoff itself identifies model-0 Unity replay and Sitka timber-anchor Unity execution as still required. Those are being closed by the bounded residual qualification packet:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/RESIDUAL-UNITY-QUALIFICATION-20261030.md`

Final `APPROVED / NEEDS CHANGES / REJECTED` under #128 must pin the same `3eb9c954...` candidate and consume the returned residual Unity evidence. If that evidence is green and no source-review defect is found, no implementation rewrite is implied by this precheck.

No Windows build.
