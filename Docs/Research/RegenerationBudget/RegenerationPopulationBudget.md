# Regeneration accounting — initial audit and decision hold

Base 402a2b41108dc9aa91e225db9047956b0a91a97e; context 691dd18a56c0da21cb08909e22ac0d0625d556b1; branch task/regeneration-population-budget. Worker: Sol 6.1, ecology implementation/diagnostic owner. Current main and origin/main matched at task start. Tutorial/UI/clearance are integrated. ScenarioOneManager.NewGameRngModel is MixedModel (1). Live coordination declares no active regeneration/ecology owner; prior ecology integration locks are cleared. Isolated worktree created without user material/settings/recovery changes or copied Library. Production work stopped before unit/save decisions.

## Current-source findings

1. Seed-independent infill is a retained calibration shortcut: GrowExistingRegeneration adds 0.05 in favourable light after survival, without current seed input. Existing persistence is legitimate; this extra density is new recruitment with no source decrement. Removing it changes ecological trajectories; Reference v1 must remain frozen and model 1 is not itself permission to fork legacy biology.
2. Late-arrival age/height inheritance is a representation defect: EstablishNewCohorts increases the one species density without creating a new age/height. Full capacity rejection can also initialize an empty cohort's age/height before accepted density is known.
3. Capacity is normalized occupancy, not juvenile slots or physical stems. Sequential per-species requests are admitted up to remaining occupancy; requested/accepted/rejected are not explicitly recorded. The Min expression can lower pre-existing density if a fixture/state is already over capacity. Such a change must be recorded as existing-stock contraction, not negative recruitment acceptance.
4. Promotion consumes the entire relative cohort and produces exactly one exact tree. There is no defined conversion or retained remainder. IDs use origin prefix/year/cell/species; successful spawn resets density/height/year, but not Origin/OriginYear. Next natural recruitment into a retained planted-origin record is an origin/age hazard. Deterministic identity is not proof of a physically conserved budget.
5. Density below 0.01 is zeroed; this numerical threshold must be recorded as a distinct loss.

These are source-verified on current main. Historical runtime findings are supporting evidence from 7f5861801f4dcfe8bbdfdac09b78880e87ad4644 and 6f252e2fbb7153f137abe7d1fa829a9dcb3011b9; current-main runtime reproduction is outstanding. Historical 55–59% rejection must not be reported as a fresh measurement.

## Ledger contract proposal

One row per cell/species/year, with relative intensity and abundance explicitly distinguished. Never subtract summed arrival from stand seed potential as seed conservation: the dispersal kernel is not normalized.

Fields: year/model/species/cell; stand species seed potential (scope label prevents duplicate aggregation); cell arrival; density before; expected light loss; expected additional browse loss; accepted infill; rejected infill; representation threshold loss; establishment requested; accepted; capacity rejected; any contraction of old stock; density before promotion; exported relative density; exact trees created; clearance relative removal; exact planted deaths; remaining relative density. Include height/establishment/origin fields before/after and promotion IDs for provenance.

Relative identity:

remaining = before - lightLoss - browseLoss + acceptedInfill - thresholdLoss + acceptedEstablishment - capacityContraction - exportedDensity - clearanceRemoval.

Request identity: requested = accepted + rejected. Exact planted individuals require a separate counted ledger: aliveStart + planted - deaths - clearanceDeaths - exactPromotions = aliveEnd. Relative density exports and exact tree counts are separate columns until the unit decision defines their relationship. Use actual stage deltas, preserve floating-point operation order, and compare instrumented traces against ordinary AdvanceOneYear. Round only presentation outputs.

Current invariants expected to fail: zero-seed/no-new-abundance due to infill; late-recruit age; physical population conservation at promotion (undefined). No-negative and monotonic survival checks must distinguish capacity contraction and threshold loss. Clearing removes cohorts/individuals and must appear as an explicit management export/loss rather than survival.

## Natural and planted compatibility

| Response | Natural/legacy cohort | Exact planted juvenile | Classification |
|---|---|---|---|
| Light growth/survival | Shared JuvenileEcologyRules | Same | Compatible shared biology |
| Browse/protection | Expected browsed fraction and cell access | Annual individual event and position/shelter access | Representation/scale difference; test boundaries |
| Height/age | One height/year for whole species cohort | Per individual metres/age | Mixed-age cohort defect; exact age is explicit |
| Survival | Expected fractional density | Deterministic alive/dead draw | Intentional realization difference; model-1 event domains |
| Competition | Light/site; no direct habitat-cover multiplier | Same light/site | Missing causal vegetation competition connection |
| Capacity | Shared relative occupancy | Separate planting location/overlap rules | Representation/design difference, not assumed equivalent |
| Promotion | All density → one tree | One individual → one tree, promotedTreeId guard | Cohort unit decision; exact adapter is countable |

Clearance really reduces regeneration density and kills qualifying exact planted competitors, thereby releasing relative occupancy. Area clearance also resets habitat cover and permits recolonisation. Neither juvenile adapter reads fern/grass/shrub cover as a direct competition factor; browse vegetationExposure remains the v1 default hook. Clearance can therefore affect regeneration through removal/capacity and existing light responses, but removing habitat cover alone has no demonstrated direct juvenile survival benefit. Expose this as a future causal-competition proposal; adding a new coefficient is excluded.

## Stop and handoff status

BLOCKED on representation decisions before production correction. Packet stop clauses: physical count conversion if needed for promotion; new persisted mixed-age state if not safely reconstructable. PopulationUnitDecision.md sets out concrete alternatives. No schema, coefficients, height/DBH, adult mortality or frozen Reference changes made. Full fixtures/funnels/regressions remain outstanding; no pass claim.

Height-task inputs to collect later: species, recruit origin/year, cell/site index, age band, height distributions, light/browse/protection, export age/height and survival histories. The packet's Irish age-30 top-height anchors (27.6/23.3/20.4/17.4/16.0 m) are unverified until the synthesis is supplied; retain as future validation requirements, not per-tree growth equations.

Research-index proposal only: add CCF Primary Literature Synthesis v1 with exact source path/version, bibliographic register, evidence strength and limitations after receipt. Label supporting evidence, never decision authority. Canonical research index unchanged.
