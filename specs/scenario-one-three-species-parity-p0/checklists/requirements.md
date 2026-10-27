# Scenario One Three-Species Parity P0 Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Spec status:** DRAFT
- **Checklist owner:** Overall Manager / assigned integrator
- **Implementation under test:** not yet implemented
- **Date:** 2026-10-27

Use this checklist against the finished implementation. A checked item requires evidence.

## Functional requirements

- [ ] FR-1 Pure-Sitka preservation — evidence:
- [ ] FR-2 Mixed ordinal species-aware density representation; no DEC/DMC/mixture-productivity bonus/invented broadleaf SDI — evidence:
- [ ] FR-3 Species-sensitive adult traits — evidence:
- [ ] FR-4 Species-sensitive regeneration/juvenile traits — evidence:
- [ ] FR-5 Species/stage browsing/protection parity — evidence:
- [ ] FR-6 Oak/Beech timber assortment not copied from Sitka — evidence:
- [ ] FR-7 Oak/Beech economic layer with evidence/transfer/calibration labels — evidence:
- [ ] FR-8 Player-facing species truth — evidence:
- [ ] FR-9 Old-save/Legacy40/Reference Future compatibility — evidence:
- [ ] FR-10 Determinism/reference protection — evidence:
- [ ] FR-11 Evidence traceability for every new species value/class — evidence:
- [ ] FR-12 Multi-Scale Forestry Management remains Sitka-adult-only unless separately amended — evidence:

## Player / user workflow

- [ ] Sitka, Oak and Beech inspection communicates materially different behaviour where the implementation differs.
- [ ] Mixed-stand management uses the species-aware representation without implying one universal prescription.
- [ ] Oak/Beech planting-to-adult behaviour remains within the shared authoritative ecology.
- [ ] Oak/Beech timber can enter an appropriate assortment/value path.
- [ ] Required failure/empty states are understandable and do not fabricate unavailable evidence or value.
- [ ] No unapproved player-facing behaviour was added.

## Authority and evidence

- [ ] Implementation follows the approved version of this spec and plan.
- [ ] D-059 comparable-depth requirement is satisfied.
- [ ] D-061 pure-Sitka and mixed-density constraints are satisfied.
- [ ] D-062 timber/economic requirements are satisfied.
- [ ] Every new species coefficient/class/value has an explicit evidence classification.
- [ ] No unlabelled pseudo-empirical coefficients were introduced.
- [ ] Broad & Lynch/GROWFOR is not misrepresented as individual-tree CCF/Oak/Beech validation.

## Save / backward compatibility

- [ ] Save impact matches the approved spec.
- [ ] Existing/missing-field saves behave correctly.
- [ ] Legacy40 remains compatible.
- [ ] Reference Future v1 remains immutable and loadable under D-042.
- [ ] No unapproved migration or schema change.
- [ ] Malformed new species data is safely rejected/defaulted if new persisted state exists.

## Density / protected Sitka behaviour

- [ ] 100% Sitka routes through the existing exact calibrated density implementation.
- [ ] Protected pure-Sitka arithmetic/anchors required by the packet are exact.
- [ ] Mixed fixtures demonstrate ordinal species-aware growing-space demand.
- [ ] No DEC.
- [ ] No DMC.
- [ ] No mixture-productivity bonus.
- [ ] No invented Oak/Beech maximum-density/self-thinning equation.

## Timber and economics

- [ ] Oak assortment eligibility differs substantively from Sitka where specified.
- [ ] Beech assortment eligibility differs substantively from Sitka where specified.
- [ ] Oak/Beech prices/values are not implemented as simple Sitka species multipliers.
- [ ] Size/form/quality/history inputs used by the model are authoritative rather than fabricated.
- [ ] Empirical, transfer and calibrated values are distinguishable in evidence/handoff records.
- [ ] Scenario One remains economically viable or any material change has explicit Manager acceptance.

## Acceptance criteria

- [ ] AC-1 Pure-Sitka exact preservation — evidence:
- [ ] AC-2 Mixed density representation — evidence:
- [ ] AC-3 Species-aware adult/regeneration/juvenile/browse behaviour — evidence:
- [ ] AC-4 Broadleaf timber/economic layer — evidence:
- [ ] AC-5 Player-facing truth and evidence limits — evidence:
- [ ] AC-6 Save/Legacy40/Reference Future compatibility — evidence:
- [ ] AC-7 Scenario One completion/economy qualification — evidence:
- [ ] AC-8 Multi-Scale Forestry Management Sitka-only guard — evidence:
- [ ] AC-9 Independent evidence/spec-conformance review — evidence:

## Review gates

- [ ] G2 acceptance evidence complete for species-sensitive behaviour.
- [ ] G3 measurement integrity complete for density/calibration/expected-value changes.
- [ ] G4 integration/canonical review complete.
- [ ] G5 complete if save/history/product-scope changed.

## Spec conformance

- [ ] Implementation matches the approved spec.
- [ ] Approved deviations are recorded in `spec.md` and the handoff.
- [ ] Unresolved requirements are listed, not hidden.

## Verdict

- **Spec conformance:** NOT VERIFIED
- **Implementation correctness:** NOT VERIFIED
- **Notes:** Draft checklist; production implementation is not authorised while the spec remains DRAFT.
