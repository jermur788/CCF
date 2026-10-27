# TASK PACKET — PARITY-03-R Independent Mixed-Density Conformance Review

**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED  
**Task:** `PARITY-03-R`  
**Status:** READY / DISPATCHED  
**Routing:** Claude independent reviewer  
**Review target:** PARITY-03 static handoff at `b22c75f439e34d5cf5bcd91a1b4303c40ceec2f8`; tested production HEAD `e1515e165eb40b40b5200992398984d69e0e8e8e`  
**Nature:** read-only repository review; no Unity lock required

## Goal

Independently assess whether the zero-production-diff PARITY-03 static conclusion is justified by the approved parity specification and evidence policy.

This review does **not** replace the focused Tier-2 Unity qualification and cannot mark density requalified by itself.

Primary evidence:

- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/HANDOFF.md`
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/MANAGER-REVIEW.md`
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md`
- approved `specs/scenario-one-three-species-parity-p0/`

## Review questions

1. Does model 0/1 remain on legacy density behaviour?
2. Does model-2 pure Sitka use the exact protected Sitka route?
3. Is “pure Sitka” based only on living individuals, including promoted juveniles but excluding dead/stumps/sub-promotion cohorts?
4. Does any living non-Reference individual select the component route?
5. Are Reference/Intermediate/High weights exactly 1.0/1.5/2.0 and used only as calibrated ordinal occupancy, not broadleaf SDI/max-density equations?
6. Is Oak > Beech represented only as the accepted weak/provisional ordinal choice?
7. Is density victim selection species-neutral and is species `Ci50` absent from victim ranking?
8. Is there any DEC, DMC, mixture-productivity bonus or hidden species-specific density-capacity coefficient?
9. Does the measured 99.9% Sitka continuity result support the absence of a material route cliff?
10. Do the three reported Beech R2 misses match the already-accepted provisional Beech height-band limitation, rather than exposing a new density-conformance defect?
11. Does zero production diff genuinely preserve protected pure-Sitka arithmetic and model-0/1 behavior?
12. Is the worker correct to stop short of `MIXED DENSITY PARITY REQUALIFIED` until Unity Tier-2 runs?

For every question report PASS / NEEDS CHANGE / NOT PROVEN — UNITY REQUIRED, with exact file/symbol/evidence references.

## Restrictions

No production/source/asset edit. No spec change. No parameter retune. No rebaselining. No Rack V2 edit. No species YAML/scene edit. No Unity. No Windows build.

If a material contradiction appears, return it to the Overall Manager rather than silently interpreting it.

## Handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Review/HANDOFF.md`

Return:

- `PARITY-03 STATIC REVIEW APPROVED — UNITY QUALIFICATION STILL REQUIRED`, or
- `PARITY-03 STATIC REVIEW NEEDS CHANGES — <precise reason>`.
