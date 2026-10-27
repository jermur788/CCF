# S1 Three-Species Parity P0 Spec — Manager Handoff

**Date:** 2026-10-27
**Owner:** Overall Manager (ChatGPT Game Dev)
**Task:** Author the D-059/D-061/D-062 implementation specification directly rather than delegate spec authoring.
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`
**Base main:** `3b499aa459b92212f7bb06897836bae5914b5230`
**Branch:** `spec/scenario-one-three-species-parity-p0-v1`

## Result

Created a four-file DRAFT spec package:

- `specs/scenario-one-three-species-parity-p0/spec.md`
- `specs/scenario-one-three-species-parity-p0/plan.md`
- `specs/scenario-one-three-species-parity-p0/tasks.md`
- `specs/scenario-one-three-species-parity-p0/checklists/requirements.md`

The package converts accepted D-059, D-061 and D-062 into a bounded implementation structure without choosing unsupported Oak/Beech numerical coefficients.

## Key preserved constraints

- exact pure-Sitka density/self-thinning path remains protected;
- mixed density uses ordinal species-aware growing-space-demand classes;
- no DEC, DMC or mixture-productivity bonus in P0;
- no invented Oak/Beech SDI/maximum-density equations;
- Oak/Beech timber assortment/economics cannot simply copy Sitka boundaries/prices;
- empirical evidence, transfer, ordinal representation, model derivation and gameplay calibration remain explicitly separated;
- Reference Future v1 stays frozen;
- old-save/Legacy40 compatibility remains mandatory;
- Multi-Scale Forestry Management V1 remains Sitka-adult-only unless separately amended.

## Deliberately unresolved before production

The draft explicitly leaves these for evidence/current-main audit and Manager acceptance:

1. Oak/Beech ordinal growing-space-demand class values.
2. Oak/Beech adult trait values where direction is supported but a number is not yet accepted.
3. Oak/Beech timber assortment thresholds/quality rules.
4. Oak/Beech economic values where Irish market evidence is incomplete.
5. Whether a save-schema change is actually needed.

No worker is authorised to fill these gaps by convenience.

## Task structure

- PARITY-01 current-main implementation audit
- PARITY-02 evidence/parameter closure
- PARITY-03 mixed density
- PARITY-04 adult/canopy/site parity
- PARITY-05 regeneration/juvenile/browse parity
- PARITY-06 broadleaf timber/economy
- PARITY-07 player-facing parity
- PARITY-08 save/deterministic compatibility
- PARITY-09 integrated parity qualification

## Verification performed

Documentation/spec authoring only. No Unity run, build, runtime claim, save mutation or production source edit.

The package follows the canonical spec templates and accepted authority hierarchy. It remains **DRAFT**, so production implementation is not authorised yet.

## Next step

Manager review the draft against the accepted decisions/current evidence. If satisfactory, complete PARITY-01/PARITY-02 and amend/approve the spec with accepted values before dispatching production tasks.

## Dashboard

`DASHBOARD SYNC: NOT AVAILABLE`

`Published dashboard reflects this task: NO`

## Verdict

`THREE-SPECIES PARITY P0 SPEC DRAFT READY FOR MANAGER REVIEW`
