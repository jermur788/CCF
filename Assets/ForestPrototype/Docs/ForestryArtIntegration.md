# Forestry art integration — ZX20 deliveries

Integration of five prototype-art deliveries from `/media/jer/ZX20/Unity Assets`
into the playable stand. All are **presentation derived from authoritative
state**; nothing here changes Forestry biology, save data or the frozen v12
Reference Future (`7AD177B3CC2F73C7`; canonical Sitka `7E39B70A14959FAD`).
Source `.blend` files and authoring scripts stay on ZX20 and are not imported.

## Imported packages

| Art folder | Contents | Wired into gameplay |
| --- | --- | --- |
| `Art/CropTreePruning/` | 12 tree bases × 7 pruning states × 3 LODs (252 FBX) | yes |
| `Art/SitkaMatureBenchmark/` | refined mature Sitka × 7 pruning states × 3 LODs | yes |
| `Art/ForestryGround/` | 4 brash piles + 2 windthrow root plates × 3 LODs | brash yes; root plates ready, not spawned |
| `Art/CoarseBranchSet/` | 3 coarse-branch defect stages × 3 LODs | ready, not spawned |
| `Art/SitkaRingBarked/` | fresh/dead ring-barked Sitka × 3 LODs | ready, not spawned |

Built by `Tools → Forest Prototype → Build All Forestry Art`
(`Editor/ForestryAssetSetup.cs`): URP materials per package, then LODGroup
prefabs under `Prefabs/Forestry/{Pruning,Ground,Defects}`, then scene wiring
for `ForestTest` and `MixedSpeciesTest`. Rebuild is idempotent.

## Pruning states = authoritative pruning data

Each tree deterministically keeps one base model (picking is hashed on its
persistent tree id, like the two-model `PickVisual` variety), and swaps only
its pruning state:

| Authoritative data | Visual state |
| --- | --- |
| `PruningLifts == 0` | Unpruned |
| `PruningLifts == 1/2/3` | Low / Medium / High pruned |
| years since `LastPruningYear` < 5 [D] | recent cut collars (exposed wood) |
| ≥ 5 years | healed scar history |

The five-year scar threshold is visual-only [D] and mirrors the pruning
recovery interval. Base assignment:

- Sitka mature: benchmark, Mature 01/02/03, OldLarge 01/02 (6 looks).
- Sitka pole: Pole 01, Young 02.
- Beech: Mature 01/02. Sessile Oak mature: Mature 01/02; pole: Young 01.

Trees keep legacy `SitkaMature01`/`SitkaPole01`/… prefabs as fallback where no
pruning base is wired. Clearances recorded in `Crop_Tree_Pruning/Coverage.md`
are asset measurements, not forestry prescriptions.

## Felling residue (brash)

Every completed Sitka felling order leaves a brash pile near the stump — including
sell, `KeepForUse` and fallen-deadwood outcomes, because branches are not
extracted with the stem. Green brash for five years [D], dry afterwards;
placement/variant is hashed on the work-order id and rebuilt from saved orders
on load and each annual step. Windthrow root plates are **not** placed: no
uprooting process is simulated.

## Not spawned (awaiting a simulated state)

- `Sitka_RingBarked_01` fresh/dead — implies a ring-barking treatment and
  standing-dead/snag state that Scenario One does not record.
- `SS_CoarseBranch_Set` — defect-tree look; the pruning brief requires
  pruning states for any crop-tree visual, which this set does not have.
- `SS_WindthrowBase_*` — implies windthrow/uprooting events.

Importing them as ready prefabs costs nothing; spawn them only when the
matching causal state exists (deferred per the Flora & Fauna decision log).

## Materials and import settings

Per-package `Materials/` map every FBX slot explicitly (33 slot names verified
against the imported models; table in `ForestryAssetSetup.CanonicalMaterial`).
URP/Lit throughout: needles/moss alpha-clipped (0.35–0.38) with Render Face
Both, bark with base colour + normal (0.7 strength), constant low smoothness
(0.15) and zero metalness. The delivered `Sitka_Bark_MetallicSmoothness_v2.png`
is 32×32 RGB black with **no alpha channel**, so it carries no smoothness
despite the package README; feeding it to URP as "metallic alpha" made alpha
default to 1 and rendered mirror-polished, sky-blue trunks on benchmark-base
trees. The map is therefore not used. FBX import: no
animation/colliders/lights/cameras, ImportStandard materials into the model,
metre scale, ground pivot. `ForestryAssetSetup` re-applies material
configuration on every build, so rebuilt materials cannot regress.

## Repository note

This import adds ~794 MB of FBX/PNG under `Art/` (909 MB `Art/` total). Every
file is well under GitHub's per-file limit, but committing them is a deliberate
choice: either commit the binaries like the existing `Art/Trees/` FBX, or
gitignore these folders and keep the ZX20 copy as the local dependency (the
same pattern as `Assets/InnerverseInteractive/`). Not decided here.

## Verification run (batch, shared worktree with Ultimate Nature installed)

- `ForestryAssetSetup.BuildAllArt` — 91 pruning prefabs + 11 ground/defect
  prefabs + 44 materials, both scenes wired, `ForestSceneBuilder.Validate` pass.
- `ScenarioOneInteractionVerification` — marks, planting, clearance,
  KeepForUse, v12 replay, canonical hash pass.
- `ScenarioHabitatPresentationVerification` — Year 0/20/50/100 presentation
  unchanged and pass.
- `CCFIntegrationVerificationTemp` — fresh 336-stem stand, interactions,
  save/load, lifecycle `7E39B70A14959FAD` pass.
- `CCFOakPlayerPlantingVerification` — planted oak promotes through the new
  pruning-state LOD prefabs (young → mature swap) and saves/loads; pass.

Still owed: an in-game walk to judge silhouette, scar readability, LOD
transitions and forest frame time — the deliveries explicitly leave those to
Unity review.

## Section 5 addition

`SectionFiveIntegration.md` documents the subsequently integrated reviewed
forest-floor delivery, seven legacy woodland props and Beech sapling. These
add 13 LOD prefabs and replace the relevant procedural vegetation/log displays
without changing authoritative ecology. Bent/cavity asset revisions remain
with the separate authoring agent.

The subsequently delivered refined bent/cavity stages and grass/rush package
are covered by `RecentDeliveriesIntegration.md`. Grass follows the existing
habitat presentation, rush clumps are authored stand dressing, and the refined
tree families supply deterministic cosmetic variants on ordinary Sitkas.
Crop Trees retain the authored pruning-compatible family. All eleven models
can also be walked and LOD-compared in
`Assets/Scenes/ForestryAssetReview.unity`.
