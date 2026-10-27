# Scenario One Three-Species Parity P0 Specification

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Status:** APPROVED
- **Owner:** Overall Manager (ChatGPT Game Dev)
- **Approved:** 2026-10-27
- **Decision log references:** D-059, D-061, D-062; compatibility constraints D-042, D-046–D-050, D-056–D-058
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness
- **Evidence authority:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Audit/HANDOFF.md`; `Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md`; `Docs/Research/ScenarioOneThreeSpeciesParityP0EvidenceClosure.md`; Manager reviews for PARITY-01/PARITY-02
- **Implementation baseline:** inherited Scenario One three-species lineage beginning at `integration/scenario-one-tier3-candidate` and carried into the current Rack V2 base; adopt + requalify + gap-close, not wholesale acceptance

## Product intent

Before the first external Scenario One demo, Sitka spruce, Oak and Beech must have comparable modelling depth and materially different behaviour where accepted evidence supports it. Comparable depth does not imply equal empirical precision. Sitka retains its stronger quantitative calibration; Oak and Beech may use explicitly labelled transfer, ordinal representation, model derivation, gameplay calibration or conservative fallback where evidence is weaker.

The player must not encounter Oak or Beech represented as Sitka with renamed labels, nor see game abstractions presented as measured Irish forestry coefficients.

## Current implementation baseline

`main` historically lacked production parity, but the active Scenario One/Rack lineage already contains substantial candidate three-species work: species-aware mixed density, Oak/Beech adult calibration, dormant site infrastructure, broadleaf timber/economics and related verification tooling. That lineage is already an ancestor of the Rack V2 implementation base.

Therefore PARITY-03…PARITY-09 are **requalification and bounded gap-closure tasks**, not greenfield feature development. Existing candidate values are retained only where this spec explicitly accepts their classification and still pass verification on the actual integration base.

## Goals

- Preserve exact pure-Sitka behaviour and protected Sitka/reference contracts.
- Requalify the inherited mixed-species density architecture under D-061.
- Give Oak and Beech accepted species-aware adult, regeneration, juvenile, browse, pruning and timber/economic treatment.
- Keep unsupported site/canopy/form mechanics explicitly neutral or deferred rather than inventing precision.
- Make the player-facing model honest about what is represented and what is not.
- Preserve old-save behaviour, Legacy40 and frozen Reference Future v1.
- Keep Multi-Scale Forestry Management quantitative treatment Sitka-adult-only unless its own spec is amended.

## Non-goals

- No Scenario Two implementation or roster expansion.
- No Oak/Beech SDI, maximum-density or self-thinning equations.
- No DEC, DMC or mixture-productivity bonus.
- No species-specific numerical site multiplier in P0.
- No species-specific canopy transmissivity or deciduous-season model in P0.
- No persisted fork/form/shaping quality state solely for premium pricing.
- No species-specific wind multipliers or storm activation.
- No broadleaf harvest-cost differentiation without new evidence.
- No parity-specific save field.
- No Windows build before the user's required playtest/authorization gate.

## Functional requirements

### FR-1 — exact pure-Sitka preservation

For growth model 2, the exact pure-Sitka route is used when **every living individual tree** — adults and promoted juveniles — is in the Reference growing-space class. Dead stems, stumps and sub-promotion regeneration cohorts do not switch the route. Models 0/1 remain legacy regardless of composition.

The protected Sitka density/controller arithmetic and `SitkaGrowthModel` behaviour must remain exact. Parity work must route around it rather than rewrite it.

### FR-2 — mixed density representation

Mixed stands use the inherited species-aware component architecture and three ordinal growing-space-demand classes:

- Sitka / Reference = 1.0
- Beech / Intermediate = 1.5
- Oak / High = 2.0

The class ordering is `ORDINAL REPRESENTATION`; Oak > Beech is weak/provisional. The numerical weights and mixed response constants are `GAMEPLAY CALIBRATION`, not empirical carrying-capacity coefficients.

Density victim selection is species-neutral. A common calibrated competition half-saturation and exponent select victims; species `Ci50` must not bias density mortality.

No DEC, DMC, mixture bonus or broadleaf density-capacity equation is permitted.

### FR-3 — adult traits, competition, site and canopy

Accepted adult calibration values remain:

- Oak: potential DBH growth 0.8 cm/year; potential height growth 0.65 m/year; max DBH 120 cm; max height 42.5 m; `Ci50` 5.
- Beech: potential DBH growth 0.9 cm/year; potential height growth 0.75 m/year; max DBH 120 cm; max height 42.5 m; `Ci50` 6.5.

All are `GAMEPLAY CALIBRATION` selected from a pre-registered calibration space, not fitted empirical coefficients. Oak/Beech keep the existing legacy asymptotic height law; Sitka retains its accepted Sitka height path.

The Beech provisional height-band miss is an accepted labelled limitation for P0 and is not retuned without a new evidence/calibration packet.

The species-site infrastructure remains dormant/neutral at exactly 1.0 in P0. Scenario One's site identity is a versioned compatibility/reference identity, not an asserted measured optimal site.

Canopy/light equations remain shared; species differences enter through existing crown geometry/relaxation only. No species opacity/transmissivity or leaf-off coefficient is invented.

The shared 8 m Hegyi neighbourhood remains a gameplay calibration. `Ci50` remains a growth/crown/suppression diagnostic trait and must not drive density-victim selection.

### FR-4 — regeneration and juveniles

Oak retains its distinct juvenile survival/establishment response. Beech retains the combined legacy survival/establishment response as `FALLBACK-CONSERVATIVE`; no invented Beech curve is added merely for structural symmetry.

Shared juvenile growth, promotion and density values may remain shared as labelled gameplay calibration. Species-specific light response, seed dispersal, maturity, mast multipliers, seed saturation and promotion-light rules remain where already present and accepted.

The current shared planting-stock values are treated as a **broadleaf stock abstraction v1**, not a Beech-specific measured stock model.

### FR-5 — browsing and protection

Keep the accepted ordinal browse susceptibility ordering Oak > Beech > Sitka as `TRANSFER`. Existing numerical palatability, escape-height, growth-loss and mortality values are gameplay-calibrated/transfer values as documented.

P0 does not add species-specific recovery/resilience mechanics or persisted browse-form damage.

### FR-6 — pruning and Crop Tree treatment

Sitka keeps its existing three-lift schedule exactly.

Broadleaves use a separate high-pruning branch:

- two lifts;
- target heights 3.5 m and 6.0 m;
- minimum 3 years between lifts;
- existing 8% crown-radius reduction per lift and 60% height cap remain gameplay calibration.

This is not formative shaping. Fork removal/form-state mechanics are deferred.

### FR-7 — broadleaf timber assortment

Oak uses the inherited five-rung broadleaf ladder:

1. veneer/planking;
2. beam;
3. fencing;
4. second planking;
5. firewood.

Beech uses:

1. second planking;
2. firewood.

Beech does not gain veneer, beam, fencing or pulp in P0.

Eligibility uses authoritative existing **size plus pruning/crown history** only. No fork/defect/browse-form state is implied. Oak veneer/beam dimensions backed by Teagasc guidance are `DIRECT`/management guidance; calibrated dimensions remain labelled gameplay calibration.

### FR-8 — economics

Keep the evidence-supported Oak/Beech price structure from the evidence closure. Teagasc “typical” quotations are `DIRECT` management guidance; selected reference values, firewood selection and roadside basis are `GAMEPLAY CALIBRATION`.

The 890 kg/m³ value is a **work-equivalent conversion proxy only**, never a claimed Oak/Beech green density.

No broadleaf harvest-cost multiplier is added without evidence.

The dormant flat broadleaf value of 6,500 cents/m³ is prohibited and must be removed or made unreachable.

A time-varying hardwood price deck is outside P0.

### FR-9 — player-facing truth

Inspection, teaching, Work Plan and timber UI must expose material species differences without overstating evidence.

Required limits include:

- timber quality wording says **size and pruning history in this version; fork/defect shape is not modelled**;
- no “straight enough”/form-quality claim unless the model actually represents it;
- site/canopy limits are stated honestly where relevant;
- wind presentation states storms are off/not modelled rather than presenting a species-calibrated wind band;
- premium Oak is described only as a **possible late, favourable, low-competition and pruning-dependent outcome**, not a target, promise or completion requirement.

### FR-10 — save and version policy

No parity-specific persisted field is added.

`GrowthModel.SpeciesAware = 2` is the behaviour gate. New Scenario One games may default to model 2. Old saves on growth models 0/1 retain their old behaviour.

The active Rack V2 base already uses save v21; parity must not bump the schema merely for species parameters.

Unknown saved species IDs must be rejected before world mutation. They must not silently resolve to Sitka.

Species-less timber-sale records are accepted for P0 while the affected Oak/Beech shared-class selected prices do not diverge. If future pricing makes species identity necessary, that becomes a later G5/schema task.

### FR-11 — compatibility and anchors

Must remain exact:

- protected pure-Sitka lifecycle/density behaviour and fingerprints;
- Legacy40 under its original configuration;
- Sitka timber behaviour/fingerprint;
- frozen Reference Future v1 archive bytes/integrity;
- old-save semantics for models 0/1.

Broadleaf-containing model-2 completion/economy/start anchors are **authorised-change candidates**, not automatic regressions. Any movement must be measured under G3, explained, and explicitly accepted. Reference continuation fingerprints remain diagnostic under D-042.

No protected anchor may be silently rebaselined.

### FR-12 — evidence traceability

The authoritative project evidence classes are:

- `DIRECT`
- `TRANSFER`
- `MODEL DERIVATION`
- `ORDINAL REPRESENTATION`
- `GAMEPLAY CALIBRATION`
- `FALLBACK-CONSERVATIVE`
- `UNSUPPORTED — NEEDS DECISION`

Runtime tags and `WorkEconomy.EvidenceLabel` are coarse implementation/UI labels only. They must map explicitly to the authoritative class. `UNSUPPORTED — NEEDS DECISION` authorises no runtime value.

For direct practitioner/official guidance such as Teagasc typical prices, runtime label `ManagementGuidance` is appropriate. Selected game reference values remain `GameplayCalibration` even where a direct range exists.

### FR-13 — Multi-Scale Forestry Management guard

Multi-Scale Forestry Management V1 quantitative treatment remains Sitka-adult-only. Oak/Beech must not enter its Sitka quantitative prescription rules unless that feature's own approved spec is amended later.

## Accepted P0 limitations / deferrals

Explicitly accepted and not blockers:

- Oak > Beech growing-space order is weak/provisional ordinal;
- Beech provisional height-band miss;
- no persisted formative/fork state;
- no Beech-specific survival/establishment curve;
- no species attribution in existing sale records while selected shared-class prices remain equal;
- roadside hardwood price basis is a labelled consistency calibration;
- generic broadleaf harvest costs;
- no measured Oak/Beech green density;
- neutral species-site multiplier;
- shared canopy opacity;
- shared wind multipliers/storms off.

These limitations must not be hidden in player-facing or verification claims.

## Concurrency / ownership

Shared species YAML and scene surfaces are single-writer. While Rack V2 core integration is active, parity production must not edit species assets or overlapping serialized scene data. Density requalification may proceed on non-overlapping source/verification files, but asset changes wait for the Rack single-writer surface to release.

## Acceptance criteria

- **AC-1:** exact pure-Sitka route and protected anchors pass.
- **AC-2:** mixed density demonstrates the accepted ordinal classes and species-neutral victim selection without DEC/DMC/mixture bonus/broadleaf density equation.
- **AC-3:** Oak/Beech adult, juvenile, browse and pruning behaviour matches the accepted evidence matrix and labels.
- **AC-4:** Oak five-rung and Beech two-rung timber/economic paths work without Sitka-copy prices or fabricated form quality.
- **AC-5:** player-facing species information states model limits and premium-Oak uncertainty honestly.
- **AC-6:** old growth models, Legacy40 and frozen Reference Future remain compatible; unknown species are rejected safely.
- **AC-7:** broadleaf-containing Scenario One completion/economy remains reachable/solvent or any material change receives explicit Manager acceptance after measurement.
- **AC-8:** Multi-Scale Forestry Management remains Sitka-adult-only.
- **AC-9:** every implemented value/rule maps to the authoritative evidence vocabulary; no unsupported coefficient is present.
- **AC-10:** independent review and applicable G2/G3/G4/G5 gates pass before `VERIFIED`.

## Stop conditions

Stop and return to the Manager if:

- exact pure-Sitka behaviour would need to change;
- implementation requires DEC/DMC/mixture bonus or a broadleaf density-capacity equation;
- an unsupported value would need to be invented;
- a new persisted field/schema bump becomes necessary;
- a task would edit a file currently owned by Rack V2 or another live writer;
- a player-facing claim would overstate Irish/UK practice, measured quality/form, site response or empirical support;
- frozen Reference Future archive bytes would change;
- a broadleaf-containing anchor moves materially without a measured, attributable explanation.

## Implementation authority

This specification is **APPROVED**. Production work may proceed only through the bounded tasks and their dependencies in `tasks.md`. Approval is not implementation, integration, verification or acceptance.
