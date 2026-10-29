# PARITY-03 Q10 — Manager Review of Static Evidence

**Verdict:** `APPROVED — STATIC EVIDENCE PHASE ONLY / CONTROLLER UNITY MEASUREMENT STILL REQUIRED`

**Reviewed candidate:** `3383e763e8ccff945dd00250366b7f334fe57331`

**Task:** PARITY-03 Q10 — strengthen mixed-density continuity evidence without changing production behaviour.

## Scope review

The candidate is one commit ahead of its recorded base `c5a269004a6b1efc3b6ffb134ca20070785f8e5e` and changes only:

- `Tools/Verification/ThreeSpecies/OfflineLogic/CompositeDensityOfflineCheck.cs`;
- `Tools/Verification/ThreeSpecies/MixedDensityVerification.cs`;
- `Tools/Verification/ThreeSpecies/OfflineLogic/run_composite_mutations.py`;
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Q10/HANDOFF.md`.

No production file under `Assets/ForestPrototype/`, species data, scene, prefab, save/schema, calibration value or protected anchor is changed.

## Static evidence disposition

The returned evidence satisfies the packet's static/offline requirements:

- both Legacy40 and Enlarged80 are covered from production geometry sources;
- both Intermediate/Beech-class and High/Oak-class replacement routes are covered;
- deterministic heterogeneous DBH/competition fixtures are added, including low-loss states;
- the exact pure-Sitka → one non-Reference seam is measured;
- existing continuity assertions/results are preserved unchanged;
- pure-Sitka/Sitka-component arithmetic remains exact in the reported checks;
- absolute and relative seam criteria are labelled as verification tolerances/gameplay calibration rather than forestry evidence;
- the new criteria kill deliberately introduced seam/fixture defects that the earlier continuity evidence did not reliably catch;
- measurement replay is deterministic.

The reported worst one-stem seam movement (0.118 expected deaths/year; 3.53% relative) is comfortably inside the new 0.5-stem/year and 10% relative verification bounds. These bounds are accepted for this verification purpose only; they are not empirical forestry thresholds and do not authorise model retuning.

The reported larger relative response at 10% Oak-class replacement in a low-loss stand is recorded as an observation. Because the response is smooth in composition, the one-stem seam remains bounded, pure-Sitka arithmetic is unchanged, and no production value was changed, it does not trigger Q10's production-discontinuity stop condition. It may be revisited only through a separately authorised calibration/evidence decision if later integrated behaviour makes it material.

The competition-index heterogeneity guard at 1.5 is accepted only as a bounded verification threshold distinguishing the new heterogeneous fixture from the prior homogeneous fixture; it is not a forestry parameter.

## Known red baseline checks

The three pure-Beech R2 misses and the identifier/string scan hit on `MaxRemovedBasalShare` are not rebaselined or waived by this review. They are recorded as pre-existing/main-reproducible results, not Q10 regressions. Q10's acceptance remains limited to its new evidence.

## Remaining mandatory gate

The controller-level `SeamTransition` stage in `MixedDensityVerification.cs` is written and focused-type-checked but has **not run in Unity**. Q10 therefore remains incomplete.

When the machine-wide Unity queue releases this task, run the exact reviewed verification candidate (or a proven descendant with byte-identical Q10 verification files) under both Legacy40 and Enlarged80, reproduce the `Q10_CONTROLLER_*` expectations, preserve the previously accepted PARITY-03 checks, and publish the final Q10 handoff.

Current machine-wide priority is RACK-V2-09 / claim #90, not the historical #49 wording in the worker handoff. Claim #74 remains open for the controller Unity phase.

No Windows build.

**Manager state:** STATIC REVIEWED / APPROVED; UNITY VERIFIED NO; Q10 COMPLETE NO; PRODUCTION CHANGED NO; EXISTING PARITY-03 ACCEPTANCE INVALIDATED NO.
