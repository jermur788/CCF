# MFM-08 Geometry Repair 01 — Manager Review

**Date:** 2026-10-30  
**Claim:** #192  
**Parent feature:** #156  
**Verdict:** **APPROVED**  
**Accepted result:** `MFM-08 SEGMENT FOOTPRINT REPAIR IMPLEMENTED — READY FOR MANAGER REVIEW`

## Reviewed lineage

- Repair base: `d55ec86a3f0bf13820f2080197b6b0f43d832e06`
- Repair production head: `e5fb424635f16182748efcb0539e8a49539495a9`
- Worker handoff/docs successor: `e8b9e55e0a76ae74f46b801362ccdfed8a680c3d`
- Repair branch: `fix/mfm-08-segment-footprints-v1`
- Worker: Claude Code / Sonnet 5.5
- Updated Tier-2 verification branch: `qual/mfm-08-unity-v1` at `630c6c3db3c67d7e75cf06bd8d0661deb1e2e5c6`

## Scope and implementation review

GitHub compare confirms the production repair is one commit from `d55ec86a...` and is bounded to:

- new pure `Assets/ForestPrototype/UI/Map/StandMapAreaFootprints.cs` + `.meta`;
- `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs`;
- focused `Tools/Verification/MfmTreatmentReview/` additions/updates.

No coverage classification/calibration, world adapter, `ScenarioOneRackSampleGeometry`, Rack V2 persistence, save/schema/validator/hash/reference, prescription/approval, Work Plan, history, teaching/MFM-09, scene/prefab or Windows-build surface changed.

The helper consumes the authoritative `ScenarioOneCoverage.Area.Segment`, rack common axis, sample anchor and `Calibration.SegmentHalfLengthM`. Low/Centre/High are derived by clipping the existing coverage-unit polygon at the same along-axis boundaries used by coverage. Rotated plans are handled from axis vectors rather than fixture orientation or parsed area IDs.

The source uses the existing fixed teaching frame (`geometry.Teaching`) rather than the full coverage strip. Remainders preserve their existing authoritative remainder polygons. One derived footprint instance now feeds map polygons/tags and `Inspect in forest` centres, removing the prior duplicated unit-centroid logic.

## Verification review

Accepted focused evidence:

- MFM-08 offline gate: `297/297`, zero failures, byte-identical repeat;
- 191 new geometry checks across east-west, 35°, -20° and 90° plans;
- segment-boundary, tiling, source-footprint, centre/waypoint, tag-separation and remainder checks;
- negative control for the old whole-unit derivation;
- mutation A (ignore segment) and mutation B (source as whole unit) both caught;
- MFM-04/05/06/07/08A offline regressions all green.

A bounded Unity compile/smoke also compiled the repair under Unity 6000.6.0f1 and returned `MFM08_UNITY_PASS checks=353`. The two original defect assertions remained intact and now pass; live segment footprints matched the expected cuts with 0.00 m reported deviation and tag overlaps fell from 15/15 to 0/15 at the two required render sizes.

The verification-harness edit at `630c6c3...` is accepted. It removes the obsolete expectation that every area footprint must equal the whole coverage-unit polygon and replaces it with the accepted segment-cut/teaching-footprint expectation. The two defect assertions are unchanged; this is a semantic correction to the expected geometry, not a test weakening. Runner parameterisation for candidate/lock task is non-semantic.

## Boundary

This approval accepts the **bounded production repair only**. It does **not** qualify MFM-08 Tier-2 as a feature. The repair handoff explicitly leaves the deferred authoritative regressions and open-work conflict for the successor #156 run.

The current Scenario One integration remains `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`; the repair is not integrated yet. #156 must qualify exact repaired production head `e5fb424635f16182748efcb0539e8a49539495a9` before Manager integration review.

## Successor requirements

#156 must now rerun full Tier-2 on exact `e5fb4246...`, including:

- the complete 353-check MFM-08 Unity gate with both repaired geometry assertions;
- rendered review at 1280×720 and 1920×1080;
- open-work conflict in addition to manual-Fell conflict;
- authoritative Rack V2 current regressions;
- MFM-06 and MFM-07 Unity regressions;
- relevant Scenario One UI regression gate;
- frozen Reference Future v1 / protected compatibility checks;
- clean save/world/lane/lock return;
- all previously accepted candidate/proposal/3D spot-check/Pending-before-Work-Plan behavior.

No production repair is authorised inside that qualification run. A new genuine defect stops and returns to Manager.

## Verdict

**APPROVED.** Claim #192 may close.

`MFM-08 GEOMETRY REPAIR 01 — MANAGER ACCEPTED`

No Windows build is authorised.
