# TASK PACKET — PARITY-03-R Independent Mixed-Density Conformance Review

**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED  
**Task:** PARITY-03-R  
**Routing:** Claude independent reviewer  
**Review target:** inherited density implementation at `c6342f79a3e89c040fadd232017a937132779385` and the dedicated PARITY-03 branch if it advances while review is in progress  
**Nature:** read-only repository review; no production edit; no Unity lock required unless reviewer separately runs Unity (not required for this packet)

## Goal

Independently assess whether the inherited model-2 mixed-density implementation structurally conforms to the approved parity specification before the primary qualification worker spends time patching or running wider Unity gates.

This review does **not** replace PARITY-03 Tier-2 qualification and cannot mark density requalified by itself.

## Review questions

1. Does model 0/1 remain on legacy density behaviour?
2. Does model-2 pure Sitka use the exact protected Sitka route?
3. Is “pure Sitka” operationally based only on living individual trees, including promoted juveniles but excluding dead/stumps/sub-promotion regeneration?
4. Does any living non-Reference individual select the component route?
5. Are Reference/Intermediate/High weights exactly 1.0/1.5/2.0 and used only as calibrated ordinal occupancy, not as broadleaf SDI/max-density equations?
6. Is Oak > Beech represented only as a class choice, without empirical wording?
7. Is density victim selection species-neutral?
8. Is species `Ci50` absent from density mortality/victim ranking?
9. Does the implementation contain any DEC, DMC, mixture-productivity bonus or hidden species-specific density-capacity coefficient?
10. Are the existing near-pure continuity tests measuring the correct route seam, and are their assertions strong enough to detect a composition cliff?
11. Do old verification records make any claims that are no longer valid under the approved evidence labels?
12. Are any density source files currently overlapped by Rack V2 ownership or another live writer?

## Required evidence

Read the relevant source at the exact review SHA, the PARITY-01/PARITY-02 handoffs, the approved spec, and the existing `Tools/Verification/ThreeSpecies` density checks/ThreeSpeciesProgramme density records.

For every review question, report:

- PASS;
- NEEDS CHANGE;
- NOT PROVEN / QUALIFICATION REQUIRED.

Cite exact file/symbol locations and distinguish static proof from claims that still require Unity/runtime measurement.

## Restrictions

No production/source/asset edit.  
No spec change.  
No parameter retune.  
No rebaselining.  
No Rack V2 edit.  
No species YAML/scene edit.  
No Windows build.

If a material product ambiguity appears, stop and return it to the Overall Manager rather than proposing a silent interpretation.

## Handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03-Review/HANDOFF.md`

Include exact reviewed SHA, source/verification files inspected, the 12-question verdict table, any likely patch surface, remaining Unity/runtime gates, and final verdict:

- `STATIC CONFORMANCE PASS — UNITY REQUALIFICATION STILL REQUIRED`, or
- `NEEDS CHANGES BEFORE UNITY REQUALIFICATION`.
