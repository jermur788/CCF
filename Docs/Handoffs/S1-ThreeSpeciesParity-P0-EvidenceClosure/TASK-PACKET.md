# TASK PACKET — PARITY-02 Evidence and Parameter Closure

## Identity

**Task:** PARITY-02 — Scenario One three-species evidence and parameter closure  
**Feature:** Scenario One Three-Species Parity P0  
**Primary:** Claude (evidence/repository synthesis)  
**Task class:** SPEC REQUIRED — research/specification only; **no production implementation**  
**Branch:** `research/scenario-one-three-species-parity-p0-evidence-closure-v1`  
**Branch base:** Manager-reviewed PARITY-01 audit at `c2dbb03777d22c378209ce16cadda48fd8bfc980`  
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`

## Read first

1. `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Audit/HANDOFF.md`
2. `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Audit/MANAGER-REVIEW.md`
3. `specs/scenario-one-three-species-parity-p0/{spec.md,plan.md,tasks.md,checklists/requirements.md}` — still DRAFT
4. Existing three-species candidate lineage, especially `integration/scenario-one-tier3-candidate` / its descendant Scenario One lineage. Treat code and verification claims as **candidate evidence**, not accepted truth.
5. Canonical research/evidence summaries already in the repository, especially the growth/density/species, ecology/regeneration, browsing/protection, and economics/transformation composites.
6. Primary-source evidence already indexed by the project where the composite marks a point uncertain or transferred.

## Manager decision already made

The existing three-species implementation lineage is **ADOPTED AS THE CANDIDATE IMPLEMENTATION BASELINE, SUBJECT TO REQUALIFICATION AND GAP CLOSURE**.

Do not propose a greenfield rebuild from `main` merely because `main` lacks the code.

Do not assume candidate-lineage values are accepted. The purpose of this task is to decide which candidate choices survive, which need recalibration or relabelling, and which must be rejected.

## Goal

Produce an explicit evidence/parameter/policy matrix for Sitka spruce, Oak and Beech sufficient for the Overall Manager to amend and approve the parity spec without pseudo-empirical precision.

The output must let later implementation/requalification workers answer, for every material species-sensitive behaviour:

- what the accepted rule/value/class is;
- why it is allowed;
- its evidence class;
- whether the existing candidate lineage already implements it correctly;
- whether it needs code change, calibration change, wording only, or no change;
- which protected behaviour/anchor it may legitimately move.

## Non-negotiable boundaries

- Preserve the exact pure-Sitka calibrated density/self-thinning path.
- No Oak/Beech SDI/max-density/self-thinning equation may be invented.
- No DEC/DMC.
- No mixture-productivity bonus in P0.
- Comparable modelling depth does not mean equal empirical precision.
- Oak/Beech timber/economics must not silently reuse Sitka assortment boundaries/prices.
- Multi-Scale Forestry Management remains Sitka-only unless separately amended.
- Reference Future v1 archive/integrity remains frozen.
- No Windows build.
- No production code, Unity asset or save change in this task.

## Evidence classes

Every accepted or proposed item must be labelled exactly as one of:

- **DIRECT** — primary evidence directly supports the claim/value for the relevant species/context.
- **TRANSFER** — transferred from related species/region/context; limitations stated.
- **MODEL DERIVATION** — derived from accepted model/data rather than directly observed.
- **ORDINAL REPRESENTATION** — evidence supports ordering/class only, not a numeric equation.
- **GAMEPLAY CALIBRATION** — chosen for playability/scale while respecting evidence bounds.
- **FALLBACK-CONSERVATIVE** — deliberately conservative default because evidence is insufficient.
- **UNSUPPORTED — NEEDS DECISION** — do not silently resolve.

Also define a mapping to the existing runtime/UI evidence vocabularies (`[A][B][C][D][E][S][I]` and `WorkEconomy.EvidenceLabel`) so implementation does not conflate incompatible labels.

## Questions to close

### Density / competition

**Q-01 — Growing-space demand and pure-Sitka routing**
- Decide the number of ordinal growing-space-demand classes and Sitka/Oak/Beech class ordering.
- Define operationally what qualifies as **100% Sitka** for the exact legacy/calibrated density path: which living stems count, treatment of promoted juveniles, and whether non-living stems matter.
- Review the candidate `GrowthModel.SpeciesAware = 2` / ordinal growing-space implementation.
- Avoid a route-switch cliff without inventing broadleaf self-thinning equations.

**Q-02 — Density victim selection / `Ci50`**
- Decide whether species `Ci50` belongs only to individual growth suppression or may influence density-mortality victim selection.
- Review the inherited species-neutral victim-selection correction and the reported broadleaf bias from reusing `Ci50`.
- Any differential mortality claim needs evidence; otherwise prefer a neutral rule.

### Adult growth / site / canopy

**Q-03 — Oak/Beech adult DBH and height**
- Decide ordinal ranking and any P0 numeric calibration that evidence legitimately supports.
- Review inherited Oak/Beech asset changes (including increased DBH/height potentials and `Ci50`).
- Separate empirical support from gameplay calibration.
- Check 25-year Scenario One usefulness and 100-year plausibility without presenting the game as a predictive growth model.

**Q-04 — Species × site response**
- Decide whether P0 needs a wired species-site multiplier on the single Scenario One Class III context.
- Review inherited dormant `SpeciesSiteResponse` infrastructure.
- If evidence does not justify a live quantitative multiplier, prefer honest neutral/dormant infrastructure over pseudo-precision.

**Q-05 — Canopy/light differences**
- Decide whether species transmissivity/deciduous effects are material enough for P0.
- If not implemented, state the limitation explicitly rather than inventing coefficients.

**Q-06 — Competition radius / `Ci50` provenance**
- Review the shared 8 m Hegyi neighbourhood and Oak/Beech `Ci50` values.
- Decide what remains shared and what is species-specific, with evidence labels.

### Regeneration / juveniles / browsing

**Q-07 — Regeneration and juvenile parameters**
- Decide which Oak/Beech differences need distinct establishment/survival/light/growth/promotion treatment.
- Resolve the current Oak-vs-Beech structural asymmetry where Oak has separate curves and Beech uses the combined legacy response.
- Review seed/mast parameters and whether shared probabilities are acceptable.

**Q-08 — Nursery stock**
- Decide whether Oak and Beech require different stock age/height/density assumptions or whether a shared labelled abstraction is acceptable.

**Q-09 — Browsing/protection**
- Confirm the qualitative ordering and any numeric palatability/escape-height values.
- Decide whether recovery/mortality response must differ by species in P0.
- Do not introduce persisted browse-form damage under this task.

### Timber / economics / management

**Q-10 — Broadleaf timber assortment**
- Define P0 Oak and Beech product ladders/thresholds using only evidence that can be defended.
- State which existing authoritative quality/history inputs may be used. Current authoritative signals include size and pruning/crown history; no fork/defect/browse-damage quality state may be invented solely for pricing.
- Review inherited appended assortment enum values and broadleaf ladders.

**Q-11 — Broadleaf economics**
- Decide species/assortment values and their evidence basis, including roadside-vs-stumpage basis.
- Review the inherited Oak/Beech price tables.
- Explicitly resolve the Sitka-derived 890 kg/m³ work proxy and the dormant flat 6,500 cents/m³ broadleaf value. Neither is accepted by default.
- Do not manufacture Irish market precision.

**Q-12 — Pruning / Crop Tree treatment**
- Decide whether the Sitka pruning-lift schedule is acceptable for Oak/Beech, should be species-specific, or should broadleaf pruning be excluded from P0 pending evidence.

### Governance / compatibility / presentation

**Q-13 — Evidence vocabulary mapping**
- Provide a binding mapping or separation policy between the project evidence classes above and existing runtime/UI enums/tags.

**Q-14 — Persisted model/version gating**
- Decide the model-gating policy for species-aware behaviour.
- Review the inherited `GrowthModel.SpeciesAware = 2` approach as the leading candidate.
- Cover unknown species IDs, species-specific timber-sale representation, enum-extension compatibility, and old-save behaviour.
- No schema field is required merely because asset parameters differ, but old saves must not silently acquire new behaviour without the accepted gate.

**Q-15 — Anchor/change authority**
- Partition anchors into:
  - must remain exact;
  - expected/authorised to move under accepted broadleaf behaviour;
  - diagnostic only.
- Pure-Sitka lifecycle and frozen Reference archive integrity remain protected.
- Broadleaf-containing completion/reference-continuation outputs may be authorised-change candidates only with explicit rationale.

**Q-16 — Player-facing wind exposure**
- Decide whether showing a species-specific-looking wind-exposure line is honest while all three species share wind multipliers and the relevant storm model may be dormant.

## Candidate-lineage review requirements

For each accepted decision above, add a column:

`Candidate lineage: KEEP / KEEP + RELABEL / RECALIBRATE / REPLACE RULE / REMOVE / NOT IMPLEMENTED`

At minimum inspect these inherited areas:

- `GrowthModel.SpeciesAware = 2` and pure-Sitka routing;
- growing-space-demand classes / `MixedDensityModel*` / `SitkaSubsetDensitySignal`;
- density victim-selection changes;
- Oak/Beech asset growth changes;
- `SpeciesSiteResponse`;
- broadleaf assortment enum extension, ladders and prices;
- evidence/verification docs under `Docs/Verification/ThreeSpeciesProgramme/`;
- relevant `Tools/Verification/ThreeSpecies/` harnesses.

Do not requalify the whole programme in Unity here. This is evidence/spec closure.

## Required deliverables

Create:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md`

and, if useful, a detailed matrix under:

`Docs/Research/ScenarioOneThreeSpeciesParityP0EvidenceClosure.md`

The handoff must include:

1. exact branch/HEAD and sources inspected;
2. completed Q-01…Q-16 decision/evidence matrix;
3. candidate-lineage KEEP/CHANGE verdicts;
4. all remaining `UNSUPPORTED — NEEDS DECISION` items;
5. exact corrections required to the DRAFT spec/plan/tasks/checklist;
6. recommended PARITY-03…PARITY-08 requalification/implementation sequence;
7. affected shared assets/files and proposed single-writer ownership order;
8. anchor/change-authority table;
9. no-code/no-Unity confirmation;
10. dashboard status.

## Stop conditions

Stop and return to the Manager if:

- a required numeric value has no credible evidence/calibration basis and cannot be represented ordinally or as a conservative labelled fallback;
- candidate lineage violates the exact pure-Sitka path in a way that cannot be isolated without redesign;
- a proposed timber/economy rule depends on new quality state not already authorised;
- model gating requires a new persistence policy beyond the existing accepted save architecture;
- evidence materially contradicts D-059/D-061/D-062 or another canonical decision.

Do **not** resolve these by convenience.

## Exit condition

`SPECIES PARAMETER MATRIX READY FOR MANAGER ACCEPTANCE`

This does not approve the spec or authorise production implementation. The Overall Manager reviews this handoff, resolves remaining decisions, amends the DRAFT spec and only then may mark it APPROVED.
