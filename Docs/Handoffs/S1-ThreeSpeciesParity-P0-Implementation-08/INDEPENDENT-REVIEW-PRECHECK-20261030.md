# PARITY-08 independent review precheck — before final Unity residuals

**Claim:** #128  
**Target:** #126 exact candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`  
**Base:** `185200f57140c65ed4d540ff0c4d173451170d14`  
**Status:** STATIC/SOURCE PRECHECK COMPLETE — FINAL VERDICT DEFERRED UNTIL #166 RESIDUAL UNITY EVIDENCE

## Independent browser/source findings

The exact candidate is one commit ahead of the accepted v24 base and changes only:

- `Assets/ForestPrototype/ForestSaveController.cs` (+3 production lines);
- `Assets/ForestPrototype/ForestSaveValidation.cs` (+41 production lines);
- focused PARITY-08 / SaveHardening verification and handoff files.

No `ForestSaveData`, schema/version, Reference Future resource, protected hash comparator/pin, scene, prefab, package, ProjectSettings, UI, S1-D or MFM production file is changed.

Source inspection confirms:

1. `CanLoad` and `LoadData` both call the same `FindLoadProblem(...)` helper.
2. `FindLoadProblem` calls `ForestSaveValidation.ValidateSpecies(...)` before `LoadData` reaches the first world mutation.
3. The validator rejects unknown non-empty species IDs on the authoritative fields at their first loader-active save versions: trees v7+, positive-density regeneration cohorts v8+, and planted juveniles plus Pending/Approved work orders v10+.
4. Empty IDs retain the historical default-species meaning; fields from versions where the loader does not read species are not retroactively reinterpreted.
5. `ForestTreeSpawner.ResolveSpecies` returns null for unknown IDs and for definitions that are not simulation-ready, so the validation delegate does not accept a half-configured species as a disguised known species.
6. Completed/Failed work-order species are treated as history rather than executable authority; species-less timber-sale records remain unchanged.
7. The old tree-load Sitka fallback remains in production but is unreachable for an unknown non-empty tree species in the normal Scenario One path because the shared pre-mutation validator rejects it first.

The focused Unity SaveHardening case independently exercises unknown tree, regeneration-cohort, planted-juvenile and Pending-work-order records. For each, both `CanLoad` and `LoadData` reject; the test then checks the ecological year and tree count remain unchanged. Static source ordering additionally establishes that the rejection occurs before the first mutation, rather than relying only on those two postconditions.

### Review note, not a current blocker

`FindLoadProblem` invokes the new species validation only when a `ForestTreeSpawner` is present. A scene with no spawner and zero saved trees therefore does not use this species resolver for cohort/juvenile/order IDs. This preserves prior non-tree-scene behavior, and the accepted Scenario One runtime/Unity witness uses the normal spawner-backed scene. Final #128 should keep this boundary explicit rather than generalising the guarantee to arbitrary no-spawner scenes.

## Verification evidence already accepted/read

- new offline species-validation suite 23/23 with mutation controls;
- current v24 MFM provenance compatibility;
- clean Unity import/compile;
- clean SaveHardening unknown-species rejection/no-mutation witness;
- save/provenance 159;
- hash dump byte-identical to accepted baseline;
- frozen Reference Future v1 PASS;
- v24 teaching-latch round trip PASS;
- approval / N3 / Legacy40 interaction PASS;
- #165 Manager-approved reconciliation: clean progress, model-1 completion, model-2 completion/density-save and protected Legacy40 density anchors, all with exit 0; no anchor rebaseline.

No source defect requiring a #126 production rewrite was found in this precheck.

## Why final #128 verdict is deferred

The binding PARITY-08 packet requires positive model-0/model-1/model-2 compatibility and protected Sitka timber anchors. Model-1/model-2 and protected density evidence are now clean. The implementation handoff itself still identifies two Unity items as unproven:

- positive model-0 compatibility/save-load replay;
- protected Sitka timber-anchor execution.

They are assigned to verification-only claim #166 under:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-08/RESIDUAL-UNITY-QUALIFICATION-20261030.md`

Final `APPROVED / NEEDS CHANGES / REJECTED` under #128 must pin the same `3eb9c954...` candidate and consume #166's exact returned receipts. If those two gates are green and do not expose a production/anchor defect, this precheck has found no other blocker to approval.

No Windows build.
