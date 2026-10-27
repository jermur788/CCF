# Scenario One Three-Species Parity P0 Implementation Plan

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Spec status:** APPROVED
- **Owner:** Overall Manager (ChatGPT Game Dev)
- **Date:** 2026-10-27

## Architecture and implementation baseline

The inherited three-species Scenario One lineage is the candidate implementation baseline. It is already an ancestor of the active Rack V2 base, so parity is not rebuilt from `main`.

The plan is therefore:

1. requalify the existing candidate architecture;
2. keep accepted structures/values with correct evidence labels;
3. close the identified gaps;
4. remove or reject unsupported behaviour;
5. qualify the combined result against pure-Sitka, save, economy and completion contracts.

Existing code is not automatically accepted merely because it exists.

## Accepted architecture

- Pure Sitka retains the exact existing Sitka density/growth path.
- Mixed stands use the inherited component density architecture with ordinal growing-space classes and species-neutral density victim selection.
- Oak/Beech adult values remain calibrated data on the species assets; no broadleaf self-thinning equation is added.
- Species-site infrastructure stays dormant/neutral in P0.
- Canopy/light equations stay shared except existing crown geometry/relaxation differences.
- Oak distinct juvenile curves remain; Beech keeps the labelled combined fallback.
- Broadleaf timber uses explicit Oak/Beech ladders, not Sitka multipliers.
- Broadleaf quality is limited to authoritative size + pruning/crown history.
- Growth model 2 is the parity behaviour gate; no parity-specific save field.

## Exact file groups / ownership

### Density writer

- `Assets/ForestPrototype/MixedDensityModel.cs`
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs`
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs`
- `Assets/ForestPrototype/ForestEcologyController.cs`
- `Assets/ForestPrototype/AdultMortalityAccount.cs`
- focused `Tools/Verification/ThreeSpecies/*` density harnesses

No Rack V2-owned file. Pure-Sitka fingerprints are mandatory before/after.

### Species data writer — blocked while Rack V2 core integration owns overlapping serialized surfaces

- `Assets/ForestPrototype/Species/SitkaSpruce.asset`
- `Assets/ForestPrototype/Species/SessileOak.asset`
- `Assets/ForestPrototype/Species/BeechSpecies.asset`
- scene/Spawner species references where genuinely required

Single-writer YAML. Do not copy serialized assets between worktrees. Sitka asset values are protected unless a specific accepted task says otherwise.

### Timber/economy writer

- `Assets/ForestPrototype/TimberYield/TimberYieldConfiguration.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldCalculator.cs`
- `Assets/ForestPrototype/TimberYield/TimberYieldEconomyAdapter.cs`
- `Assets/ForestPrototype/WorkEconomy/ForestryWorkDomain.cs`
- `Assets/ForestPrototype/WorkEconomy/Stage1EconomyDefaults.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneEconomyAdapter.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs`
- `ScenarioOne.asset` only under a serialized-asset single-writer lock

### Pruning / player-facing writer

- `Assets/ForestPrototype/ForestTree.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs`
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs`
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`
- `Assets/ForestPrototype/UI/WorkPlanView.cs`

Coordinate with S1-C/S1-D UI ownership before any shared UI file edit.

### Save validation writer

- `Assets/ForestPrototype/ForestSaveController.cs`
- `Assets/ForestPrototype/ForestSaveValidation.cs`
- related save-hardening verification

No new field is expected. The required change is unknown-species rejection before mutation.

## Merge / task order

1. **PARITY-01** current implementation audit — COMPLETE.
2. **PARITY-02** evidence/parameter closure — ACCEPTED.
3. **PARITY-03** mixed-density candidate requalification + bounded gap closure.
4. **PARITY-04** adult/species-data requalification after Rack V2 releases shared YAML; mostly label/verification closure unless a measured defect appears.
5. **PARITY-05** regeneration/juvenile/browse requalification and label closure.
6. **PARITY-06** broadleaf timber/economy + broadleaf pruning gap closure.
7. **PARITY-07** player-facing truth/teaching closure.
8. **PARITY-08** save/deterministic compatibility including unknown-species rejection.
9. **PARITY-09** integrated feature qualification and independent spec review.

PARITY-03 may begin on non-overlapping source/verification files while Rack V2 core work is active. Any species-YAML or shared scene edit waits for the Rack single-writer surface to release.

## PARITY-03 execution strategy

First compare the inherited density implementation on the actual task base against the approved spec. Prefer **no production change** if it already conforms.

Requalify:

- pure-Sitka route arithmetic/fingerprints;
- model 0/1 legacy routing;
- model-2 pure Sitka routing;
- mixed route selection with any living non-Reference tree;
- Reference/Intermediate/High class handling;
- species-neutral density victim selection;
- route continuity around near-pure compositions;
- no DEC/DMC/mixture productivity;
- no `Ci50` mortality coupling.

Only patch a demonstrated conformance defect. Do not retune response constants merely because they are gameplay calibration.

## Adult/species-data strategy

After Rack V2 core integration releases YAML ownership:

- verify Oak 0.8/0.65/120/42.5/5 and Beech 0.9/0.75/120/42.5/6.5 are present and labelled as calibration;
- retain Oak High / Beech Intermediate classes as weak/provisional ordinal;
- rename the shared planting abstraction in documentation/copy, not by inventing new values;
- keep site multiplier neutral 1.0;
- keep shared canopy opacity and 8 m competition neighbourhood;
- accept Beech height-band miss as a recorded limitation.

## Timber/economy/pruning strategy

Requalify the inherited Oak five-rung and Beech two-rung ladders. Correct evidence labels for Teagasc price guidance. Remove/block the dormant flat broadleaf value.

Broadleaf pruning must branch from Sitka:

- Oak/Beech: two high-pruning lifts at 3.5 m and 6.0 m, min 3 years apart;
- Sitka: existing schedule exact.

Do not add fork/form state. Timber eligibility/UI must describe the quality proxy as size + pruning history only.

Premium Oak is a possible late/favourable outcome, not a completion target.

## Save / compatibility strategy

No parity schema bump. Model 2 is already persisted through the existing growth-model field. Old saves with models 0/1 remain unchanged.

Add validation so an unknown saved `speciesId` fails load before mutation instead of falling back to Sitka.

Species-less sale records remain accepted for P0 under the current equal selected shared-class prices. If later price divergence makes species identity necessary, stop for a new G5 decision.

## Verification

### Tier 1 during work

- static/offline compile and focused arithmetic/logic tests;
- exact diff inspection;
- evidence-label review;
- pure-Sitka comparison after every density/adult slice.

### Tier 2 feature qualification

Required before parity is called qualified:

- pure-Sitka exact route/fingerprints;
- mixed density fixtures and route continuity;
- Oak/Beech adult/juvenile/browse/pruning fixtures;
- broadleaf timber/economy fixtures;
- unknown-species negative save fixtures;
- old growth-model save compatibility;
- Legacy40 and frozen Reference archive integrity;
- Scenario One completion/economy/solvency witness under model 2;
- rendered inspection/Work Plan checks where UI copy changed.

Broadleaf-containing anchors may legitimately move, but only with G3 measurement and Manager acceptance. Pure-Sitka/Legacy/archive anchors may not be rebaselined.

### Independent review

Claude or another independent reviewer must check:

- evidence classifications;
- no unsupported coefficient;
- no hidden Sitka inheritance presented as broadleaf evidence;
- no pseudo-empirical UI claim;
- no Multi-Scale Forestry Management broadleaf leakage;
- spec conformance.

## Gates

- **G2:** behaviour/acceptance.
- **G3:** density/calibration/economy measurement integrity.
- **G4:** integration/canonical acceptance.
- **G5:** required only if implementation unexpectedly needs new persisted state/history/schema; current approved design does not.

## Stop conditions

Stop before editing if ownership overlaps an active Rack/S1-C/S1-D writer. Stop if a required fix would alter protected pure-Sitka arithmetic, require a new broadleaf density equation, add unsupported precision, add persisted state, or require changing frozen Reference archive bytes.
