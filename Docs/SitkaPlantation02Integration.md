# Plantation Sitka 02 — Unity integration

> **Scenario 1 port note (integration/scenario-one-complete).** This document was written for the uncommitted `/home/jer/CCF` integration. The Scenario 1 port carries the presentation parts only: legacy Sitka retirement, Plantation02 render forms, the runtime asset subset and the habitat-gate checks. It does **not** wire `PlantationBranchMovement` into `ForestPlayer`, so the slowdown and blocking described under "Branch movement" are not active. Compact felling residue is also not ported. The hash figures below are from that original worktree; see `Docs/Scenario1StartingStandAssetFix.md` for the integrated results.


Integrated in `/home/jer/CCF`, October 2026, from the delivered
`/media/jer/ZX20/Unity Assets/Sitka_Plantation_02/` package.

## Asset and scene wiring

- All 648 manifest-listed delivery hashes verified; 474 FBXs imported with
  explicit mesh/material mappings. Source art remains unchanged.
- Imported art/docs: `Assets/ForestPrototype/Art/SitkaPlantation02/` (~48 MB).
- Unity-metre component meshes: `Assets/ForestPrototype/Meshes/Plantation02/`
  (~18 MB). Imported FBX transforms are baked once into these reusable meshes.
- Runtime data: `ScenarioOne/Resources/PlantationVisualCatalog.asset` under
  `Assets/ForestPrototype/`. Two prefabs live in `Prefabs/Forestry/Plantation02/`.
- Both `ForestTest` and `MixedSpeciesTest` enable the plantation renderer.
  Scene creation, forestry rewiring and recent-delivery rewiring retain it.
- Render ranges [D]: below 12 m, `SS_Plantation_Pole_02`; 12–<20 m,
  `SS_Plantation_FirstThinning_02`. The mature benchmark and existing refined
  bent/cavity variants remain later-tree references. Young stock uses the
  ordinary plantation forms because bent/cavity lower-branch coverage was not
  delivered. Retired model families remain excluded from the active pools.

Import menu: **Tools → Forest Prototype → Integrate Plantation Sitka 02**,
or batch entry point `PlantationAssetSetup.BuildAndWire`.

## Geometry, pruning and scars

`PlantationTreeVisual` assembles one matching core plus retained whole branch
systems. It never adds modules on top of a complete snapshot. Each LOD is
combined by material; identical branch masks share meshes. The initial
336-tree stand shares two unpruned assemblies, not thousands of twig objects.
Unused cached assemblies are released when their instances are destroyed or
switch treatment. Full snapshots are imported as reference exports.

The actual Unity FBX basis was confirmed from asymmetric module bounds:
**Unity `(x,y,z) = (-authorX, authorZ, -authorY)`**, in metres. All 140 module
unions match the converted delivered bounds, and woody capsule axes align
with the imported wood. Scar placement conjugates the delivered local basis
through this same conversion, retaining per-LOD sites and piece scale.

On plantation models, recorded `CrownBaseHeightM` determines retained systems
after transforming all-LOD bounds to world space. A system extending below
the target is removed together with its attached foliage. Recent/healed
scars are placed at removed sites; the existing five-year visual scar-age
rule is retained. Crop Tree designation does not prune or clear contact.
Current pruning eligibility and ecological crown-radius effects are retained.

## Branch movement

`PlantationBranchMovement` uses LOD-invariant delivered capsule metadata and
a 4 m spatial grid. Queries are separate from Physics and aim raycasts:
render meshes have no colliders, and trunk inspection/marking/work targets
and ground planting remain on their original proxies.

- Ordinary actual low-branch/foliage contact reduces movement speed to 35%
  [D]. Clear ground and overhead contact do not slow ground walking.
- Hard passage blocking requires woody contact from at least two unpruned
  young-stock instances, at least three branch systems, and contact across
  both sides of the body-width route [D]. Foliage envelopes alone cannot
  form a hard wall; mere proximity of two trunks is insufficient.
- Horizontal movement is swept in steps no larger than 0.08 m, spending
  movement budget according to contact. Vertical movement remains governed
  by the existing CharacterController/gravity path.
- A player already inside a blocked patch can move out under slowdown. Once
  clear, entry into another dense patch is blocked normally.
- Branch masks and contact rebind on visual refresh, including growth,
  completed pruning, loading and model changes. Disabled/felled/removed
  visuals unregister immediately, including during reference-preview swaps.

These thresholds are gameplay calibration, not new forestry biology. This
package covers pole/first-thinning stock, not a new seedling/juvenile pruning
system or low-branch defect variants.

## Verification

- `PlantationIntegrationVerification.Begin`: full 336-tree start, two shared
  assemblies, collision-free visuals, all 17 delivered scale/pruning examples,
  ordinary slowdown, clear/overhead space, a dense pair at 1.9 m spacing,
  fast movement, exits, designation, real pruning, felling and LOD-independent
  contact pass.
- Contact-query profile on the 336-tree fixture: 1,000 movement queries in
  approximately **121 ms**, or **0.12 ms/query**. This measures query CPU time,
  not whole-game frame time or GPU performance.
- `PlantationIntegrationVerification.BeginRendered`: URP/OpenGL eye-level
  captures of the actual stand, unpruned LOD0/1/2, a completed 2.5 m lift,
  and a close-planted before/after pair. Reviewed for lower-branch visibility
  and the visibly cleared bole. Captures are under `/tmp/opencode/` as
  `plantation-stand.png`, `plantation-unpruned-lod0/1/2.png`,
  `plantation-low-pruned.png`, `plantation-dense-unpruned.png` and
  `plantation-dense-pruned.png` (temporary verification outputs).
- `ScenarioHabitatPresentationVerification.Begin`: Years 0/20/50/100,
  returned player world, retained-system signature, active visual count and
  contact-registry consistency pass. Later-tree cosmetic variants remain
  available; Year-0 stock is now ordinary plantation art throughout.
- `ScenarioOneInteractionVerification.Begin`: actual work completion,
  planting, marks, save/load and the legacy continuation pass. Added checks
  verify restored branch masks and contact-instance registration.
- Canonical lifecycle hash remains **`7E39B70A14959FAD`**; the frozen
  Year-100 reference remains **`7AD177B3CC2F73C7`**. The historical P0601
  same-year prune/fell exception remains documented by the existing gate.

Player review should calibrate how the slowdown and dense blocking feel in
the normal Game view. The import and integration are local, uncommitted work.

## Combined review follow-up integration

`feature/forestry-ecology` in `/home/jer/CCF` was fast-forwarded to `a31ec62`,
bringing in `3b71e4b` (save hardening/growth cache), `8aede39` (versioned RNG),
`457c897` (batch recomputation/shared forecasting term) and `a31ec62`
(edge-bias diagnostic). The local plantation, contact and compact-brash work
was reapplied without conflicts. No new integration commit was made.

Combined Unity checks passed:

- Save hardening/growth-readout checks; the existing on-disk save and backup
  were restored byte-for-byte after testing.
- RNG checks: mast lag-1 correlation -0.392 (legacy) versus 0.053 (mixed);
  median survival-roll lag-1 0.659 versus -0.010.
- Batch versus per-felling canopy/seed-rain equivalence, including nested
  batches. The edge diagnostic reproduced its committed band results.
- Plantation geometry/movement checks and a mixed-model save round trip:
  a model-1 player can view the historical model-0 reference and return to
  model 1 with the same world, retained systems and contact registrations.
- Habitat previews at Years 0/20/50/100, full interaction regression,
  compact residue checks (including 311 century-replay piles), canonical
  lifecycle `7E39B70A14959FAD` and frozen reference `7AD177B3CC2F73C7`.

Legacy RNG model 0 remains the default; mixed model 1 is opt-in and recorded
in save data. The edge-bias addition is diagnostic only. These commits do
not add boundary wrapping or adult mortality. Temporary verification scripts
were returned to `Tools/Verification/`; no temporary copies remain in Assets.
