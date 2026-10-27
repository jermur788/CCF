# TASK PACKET — PARITY-03 Mixed-Density Candidate Requalification + Gap Closure

**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED on canonical `main`  
**Task:** PARITY-03  
**Routing:** DeepSeek primary for implementation/qualification; Claude independent review  
**Branch base:** `c6342f79a3e89c040fadd232017a937132779385`  
**Canonical packet:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/TASK-PACKET.md` on current `main`

## Purpose

Requalify the inherited growth-model-2 mixed-density implementation against the approved three-species parity spec. This is not a greenfield rewrite and not permission to retune calibrated constants by preference.

Prefer a zero-production-diff result if the inherited implementation already conforms.

## Preflight

1. Refresh live `Docs/Project/coordination/active-tasks.md` from `main`.
2. Verify exact branch/HEAD/worktree and preserve unrelated dirty work.
3. Confirm no current writer owns any file you intend to change.
4. Do not touch Rack V2-owned files.
5. Do not edit species YAML or serialized scene data in this task.
6. Acquire/release the machine-wide Unity lock for any Unity execution.

## Owned surface

Only as needed:

- `Assets/ForestPrototype/MixedDensityModel.cs`
- `Assets/ForestPrototype/MixedDensityModel.Composite.cs`
- `Assets/ForestPrototype/SitkaSubsetDensitySignal.cs`
- density-specific sections of `Assets/ForestPrototype/ForestEcologyController.cs`
- `Assets/ForestPrototype/AdultMortalityAccount.cs`
- focused `Tools/Verification/ThreeSpecies/*` density verification
- this task's handoff/evidence docs

## Forbidden surface

No species assets/scene YAML, Rack V2 files, timber/economy/pruning/UI, save schema, Multi-Scale Forestry Management, completion-rule edits, Reference Future archive edits or Windows build.

## Binding behaviour

- Models 0/1 stay legacy.
- Model-2 pure Sitka means every living individual tree is Reference class; dead/stumps/sub-promotion regeneration do not switch route.
- Pure-Sitka controller/SitkaGrowthModel arithmetic is protected exact behaviour.
- Mixed route classes: Sitka Reference 1.0; Beech Intermediate 1.5; Oak High 2.0.
- Oak > Beech is weak/provisional ordinal; weights/response constants are gameplay calibration.
- Any living non-Reference tree selects the component route.
- Density victim selection is species-neutral; species `Ci50` must not influence density mortality.
- No DEC, DMC, mixture-productivity bonus or broadleaf SDI/max-density equation.

## Verification

Static/offline: route selection, legacy/model2 separation, living-tree definition, `Ci50` exclusion, focused density checks and protected-source comparison.

Re-run the near-pure composition continuity sweep on the tested HEAD; do not reuse an old claim without rerunning it.

Unity/Tier 2 where available: pure Sitka, near-pure mixed, Oak/Beech mixed, protected Sitka density fingerprints, compile/import clean. If Unity is unavailable, hand off explicitly as NOT UNITY-QUALIFIED and do not claim task completion.

## Patch policy

Patch only a demonstrated spec-conformance defect. Do not retune calibrated density constants without measured evidence and Manager approval. A conforming inherited implementation should produce no production diff plus refreshed qualification evidence.

## Handoff

Publish `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-03/HANDOFF.md` with exact base/tested HEAD, production diff, inspected symbols, route proof, fingerprints, continuity measurements, mixed-fixture results, commands/gates actually run, failures, evidence locations and verdict.

## Exit

`MIXED DENSITY PARITY REQUALIFIED`

or an explicit Manager blocker.
