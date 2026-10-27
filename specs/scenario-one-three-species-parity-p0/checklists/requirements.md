# Scenario One Three-Species Parity P0 Requirements / Acceptance Checklist

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Spec status:** APPROVED
- **Checklist owner:** Overall Manager / assigned integrator
- **Date:** 2026-10-27

A checked item requires evidence.

## Density / pure-Sitka protection

- [ ] 100% Sitka operational definition uses living individual trees only; dead/stumps/sub-promotion cohorts do not switch route.
- [ ] Models 0/1 retain legacy behaviour.
- [ ] Model-2 pure Sitka routes through the exact protected Sitka implementation.
- [ ] Protected pure-Sitka lifecycle/density fingerprints remain exact.
- [ ] Mixed density uses Reference 1.0 / Beech Intermediate 1.5 / Oak High 2.0.
- [ ] Oak > Beech is labelled weak/provisional ordinal, not empirical.
- [ ] Density victim selection is species-neutral and does not use species `Ci50`.
- [ ] Near-pure route continuity is measured.
- [ ] No DEC, DMC, mixture-productivity bonus or broadleaf SDI/max-density equation exists.

## Adult / site / canopy / competition

- [ ] Oak adult values 0.8/0.65/120/42.5/5 are present and labelled gameplay calibration.
- [ ] Beech adult values 0.9/0.75/120/42.5/6.5 are present and labelled gameplay calibration.
- [ ] Beech provisional height-band miss is recorded as an accepted limitation, not silently retuned.
- [ ] Oak/Beech use the accepted legacy asymptotic height path; Sitka path remains exact.
- [ ] Species-site multiplier remains dormant/neutral at 1.0.
- [ ] Canopy/light remains shared except existing crown geometry/relaxation differences.
- [ ] No invented species opacity/transmissivity or deciduous-season coefficient.
- [ ] 8 m Hegyi neighbourhood remains shared/calibrated and is not described as measured broadleaf evidence.

## Regeneration / juvenile / browse

- [ ] Oak distinct juvenile survival/establishment curves remain active where intended.
- [ ] Beech combined juvenile survival/establishment response is explicitly labelled fallback-conservative.
- [ ] Shared juvenile growth/promotion/density values are labelled calibration rather than species evidence.
- [ ] Shared planting stock is described as broadleaf stock abstraction v1, not Beech-specific evidence.
- [ ] Browse ordering Oak > Beech > Sitka is preserved and classified as transfer/ordinal evidence.
- [ ] Shared browse loss/mortality values are labelled gameplay calibration.
- [ ] No persisted browse-form or species-recovery mechanic is introduced.

## Pruning

- [ ] Sitka three-lift schedule remains exact.
- [ ] Oak/Beech use two high-pruning lifts at 3.5 m and 6.0 m.
- [ ] Broadleaf pruning requires at least 3 years between lifts.
- [ ] Existing 8% crown-radius reduction and 60% height cap are labelled gameplay calibration.
- [ ] No formative shaping/fork state is implied or persisted.

## Timber / economics

- [ ] Oak has veneer/planking, beam, fencing, second planking and firewood.
- [ ] Beech has second planking and firewood only.
- [ ] Beech does not silently inherit Sitka/Oak premium assortments.
- [ ] Timber quality uses authoritative size + pruning/crown history only.
- [ ] UI does not claim fork/defect/straightness quality that the model does not represent.
- [ ] Teagasc typical quotations are labelled `ManagementGuidance` / direct guidance.
- [ ] Selected reference prices, firewood selection and roadside basis are labelled gameplay calibration.
- [ ] 890 kg/m3 is described only as a work-equivalent proxy, never measured Oak/Beech green density.
- [ ] Dormant flat broadleaf value 6,500 c/m3 is removed or unreachable.
- [ ] No unsupported broadleaf harvest-cost multiplier.
- [ ] Premium Oak is described only as a possible late/favourable outcome, not a target/guarantee.

## Player-facing truth

- [ ] Inspection/teaching/Work Plan copy reflects the actual represented species differences.
- [ ] Copy says timber quality uses size + pruning history and fork/defect shape is not modelled.
- [ ] Site/canopy limitations are not hidden or presented as empirical species response.
- [ ] Wind line states storms are off/not modelled where applicable.
- [ ] No pseudo-empirical Irish/UK practice claim.
- [ ] 720p and 1080p rendered checks pass where UI is changed.

## Save / compatibility

- [ ] No parity-specific save field or schema bump.
- [ ] Growth model 2 is the parity behaviour gate for new games.
- [ ] Old growth models 0/1 retain prior behaviour.
- [ ] Unknown saved species IDs are rejected before world mutation.
- [ ] CanLoad/LoadData validation behaviour is consistent.
- [ ] Species-less timber-sale records remain acceptable under current P0 price assumptions.
- [ ] Legacy40 remains exact under its original configuration.
- [ ] Frozen Reference Future v1 archive bytes/integrity remain exact.
- [ ] No historical save is silently reinterpreted.

## Anchor / measurement policy

- [ ] Pure-Sitka, Legacy40, Sitka timber and Reference archive protected anchors are exact.
- [ ] Broadleaf-containing model-2 anchor movement is measured and attributable.
- [ ] Any accepted broadleaf-containing anchor change has explicit Manager approval.
- [ ] Reference continuation remains diagnostic, not silently rebased.
- [ ] G3 measurement integrity evidence accompanies material density/economy/completion movement.

## Evidence vocabulary

- [ ] Every implemented value/rule maps to exactly one authoritative primary class: DIRECT / TRANSFER / MODEL DERIVATION / ORDINAL REPRESENTATION / GAMEPLAY CALIBRATION / FALLBACK-CONSERVATIVE / UNSUPPORTED — NEEDS DECISION.
- [ ] Runtime/UI evidence labels are explicitly mapped and not treated as research authority.
- [ ] `UNSUPPORTED — NEEDS DECISION` has no runtime value.
- [ ] Broad & Lynch/GROWFOR is not presented as Oak/Beech or individual-tree CCF validation.

## Multi-Scale Forestry Management guard

- [ ] Multi-Scale Forestry Management quantitative treatment remains Sitka-adult-only.
- [ ] Oak/Beech cannot enter Sitka-only prescription rules through species-parity changes.

## Integrated acceptance

- [ ] Scenario One remains completable under the qualified model-2 candidate.
- [ ] Scenario One remains solvent or any material economic change is explicitly accepted.
- [ ] Oak/Beech receive comparable modelling depth without fabricated precision.
- [ ] No live Rack/S1-C/S1-D ownership was overwritten during implementation.
- [ ] Independent reviewer confirms spec conformance.
- [ ] G2 acceptance complete.
- [ ] G3 measurement integrity complete where applicable.
- [ ] G4 integration/canonical review complete.
- [ ] G5 complete only if an unexpected persistence/history/schema change occurred.

## Verdict

- **Spec conformance:** NOT YET VERIFIED
- **Implementation correctness:** NOT YET VERIFIED
- **Current state:** APPROVED spec; PARITY-03 is the first production requalification task.
