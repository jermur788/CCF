# TASK PACKET — PARITY-03 Mixed-Density Candidate Requalification + Gap Closure

**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED  
**Task:** PARITY-03  
**Routing:** DeepSeek primary for implementation/qualification; Claude independent review  
**Candidate code base:** `c6342f79a3e89c040fadd232017a937132779385` or the exact later Rack-integrated descendant chosen by the Manager  
**Context:** Git is current authority; previously published `eb218ea` attachment snapshot is stale after S1-C/S1-D/parity approvals

## Purpose

Requalify the inherited growth-model-2 mixed-density implementation against the approved three-species parity spec. This is **not** a greenfield rewrite and **not** permission to retune calibrated constants by preference.

Prefer a zero-production-diff result if the inherited implementation already conforms.

## Preflight

Before editing:

1. refresh live `Docs/Project/coordination/active-tasks.md`;
2. verify exact branch/HEAD/worktree and preserve unrelated dirty work;
3. confirm no current writer owns any file you intend to change;
4. do **not** touch Rack V2-owned files;
5. do **not** edit species YAML or serialized scene data in this task;
6. acquire the machine-wide Unity lock before any Unity execution and release it afterward.

If the selected base is not the current integrated Scenario One/Rack candidate, record why and do not claim integration qualification.

## Owned surface

Only as needed:

- `Assets/ForestPrototype/MixedDensityModel.cs`
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs`
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs`
- density-specific sections of `Assets/ForestPrototype/ForestEcologyController.cs`
- `Assets/ForestPrototype/AdultMortalityAccount.cs`
- focused `Tools/Verification/ThreeSpecies/*` density verification
- handoff/evidence docs for this task

If a conforming implementation requires no production edit, leave production source unchanged.

## Forbidden surface

- `Assets/ForestPrototype/Species/*.asset`
- scene/prefab YAML
- Rack V2 geometry/runtime/headland/field-marking files
- timber/economy/pruning/UI
- save schema
- Multi-Scale Forestry Management
- completion rules/objective targets
- Reference Future archive bytes
- Windows build

## Binding requirements

### Pure Sitka

For growth model 2, pure Sitka means every **living individual tree** — adult or promoted juvenile — has Reference growing-space class. Dead stems, stumps and sub-promotion regeneration cohorts do not switch the route.

Models 0/1 remain legacy.

The existing pure-Sitka controller block and `SitkaGrowthModel` arithmetic are protected. Do not modify them to make mixed density work.

### Mixed route

Accepted ordinal classes:

- Reference/Sitka = 1.0
- Intermediate/Beech = 1.5
- High/Oak = 2.0

Oak > Beech is a weak/provisional **ordinal representation**. The weights and response constants are **gameplay calibration**, not empirical carrying-capacity parameters.

A single living non-Reference individual selects the species-aware component route.

### Victim selection

Density victim selection must be species-neutral. Species `Ci50` is reserved for growth/crown/suppression diagnostics and must not bias density mortality.

### Prohibited mechanics

No:

- DEC;
- DMC;
- mixture-productivity bonus;
- Oak/Beech SDI;
- Oak/Beech maximum-density/self-thinning equation;
- empirical claim for the calibrated class weights/response constants.

## Required verification

### Static/offline

- inspect exact route-selection code and candidate constants;
- prove model 0/1 never enter the model-2 mixed route;
- prove model-2 pure Sitka enters the exact protected route;
- prove a living Intermediate/High stem switches to the component route;
- prove dead/stump/sub-promotion cohorts do not switch it;
- confirm species `Ci50` is absent from density-victim selection;
- compile/run existing density offline checks where available;
- compare source of protected pure-Sitka arithmetic before/after.

### Route-continuity measurement

Re-run or reproduce the existing near-pure composition sweep. Report the actual measured discontinuity/annual-loss behaviour; do not quote the old candidate claim without rerunning it on the tested HEAD.

### Unity / Tier 2 density feature qualification

If a configured Unity lane is available, run the focused mixed-density qualification on the exact candidate HEAD under the machine-wide lock. Include:

- pure Sitka fixture;
- near-pure mixed fixtures;
- Oak/Beech mixed fixtures;
- protected Sitka density fingerprints;
- no compiler/import errors.

If Unity cannot be run in the worker environment, publish the implementation/static result explicitly as **NOT UNITY-QUALIFIED** and route to an available Unity lane. Do not claim the task exit state until required qualification is complete.

## Patch policy

Patch only a demonstrated spec-conformance defect.

Do not retune occupancy normalization, response strength, exponent, annual-loss ceiling/basal-area budget, class weights or victim-selection constants merely because they are calibration values. A retune requires measured evidence and Manager approval.

If the inherited implementation already conforms, the correct production result is **no code change** plus refreshed qualification evidence.

## Handoff

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/HANDOFF.md`

Include:

- exact base and tested HEAD;
- production diff (including an explicit `NONE` if unchanged);
- exact files/symbols inspected;
- route-selection proof;
- pure-Sitka source/fingerprint results;
- near-pure continuity measurements;
- mixed-fixture results;
- commands/tests/Unity gates actually run;
- failures and unresolved items;
- evidence locations;
- verdict.

## Exit

`MIXED DENSITY PARITY REQUALIFIED`

or a precise blocker requiring Manager action.

No claim of full three-species parity, integration or milestone verification is permitted from this task alone.
