# Consolidated deliveries — grass/rush and refined bent/cavity integration

Shared Unity project: `/home/jer/CCF`. Inputs are the delivered FBXs/textures
from `Woodland_Grass_Rush`, `SS_Bent_Refined_Set` and `SS_Cavity_Refined_Set`.
All 105 manifest-listed payload hashes checked clean before integration.
Source modelling/Blender files remain untouched; this is engine-side work.

## Assets and use

33 FBXs produce **11 three-LOD prefabs** under
`Assets/ForestPrototype/Prefabs/Forestry/SectionFive/`. Imported payloads/docs
live in corresponding package folders under `Art/SectionFive/`. References
are stored in `ScenarioOne/Resources/RecentAssetVisualCatalog.asset`.

- Three woodland grass tufts now replace the procedural grass silhouette in
  `ScenarioHabitatVisuals`. Existing grass cover controls counts/placement;
  cell/plant indices choose a repeatable variant. The frozen reference shows
  0/19/16/11 patches at Years 0/20/50/100. No new competition rule or save data.
- Refined bent/cavity stages now appear on ordinary Sitkas in **ForestTest
  and MixedSpeciesTest**. A persistent-tree-ID hash selects each cosmetic
  family on later trees (roughly 10% bent, 10% cavity); existing height/DBH selects the
  renderer stage [D]. Trees retain their simulation IDs, positions, physical
  state and biological rules. No disease, cavity development, timber penalty
  or habitat bonus is simulated. Plantation 02 now supplies all young-stock
  looks below 20 m [D], because low-branch defect treatments were not delivered.
  The Year-0 stand has no defect substitutions; the current frozen Year-100
  preview has 11 bent and 8 cavity looks on later trees.
- Both rush models now have authored floor-dressing placements near the
  stand's southern corners: four clumps, with anchors stored in
  `RecentAssetVisualCatalog.rushDressingPositions`. These are scenery accents,
  not observations of drainage, wetland habitat or new rush populations.
- Two rush clumps and all six refined bent/cavity stage models are instantiated
  in the walkable **`Assets/Scenes/ForestryAssetReview.unity`**. Specimens retain
  their delivered metre scale and ground pivots. Tree labels display the
  authored stage; they are not biological-age or timber/habitat classifications.
- Crop Tree designation, and any existing pruning history, select the fully
  authored pruning family instead of defect-reference models. Clearing an
  unpruned Crop Tree designation restores its original cosmetic model. This
  may change the visual silhouette, but never the tree's identity or biology.
  The delivered references have no pruning treatments; none are inferred or
  fabricated in the engine.

## Render setup

### Active Sitka library (October 2026)

The main stand now uses only `Sitka_Mature_Benchmark_01` for straight mature
trees and `Sitka_Young_02` for straight young trees, including their seven
authored pruning states. The approved refined bent/cavity variants retain
their existing deterministic distribution. The older Mature 01/02/03,
OldLarge 01/02 and Pole 01 families are removed from the active spawner pools;
the standalone `SitkaMature01` / `SitkaPole01` fallbacks are also replaced.
The imported historical art remains available in the library.

All scene creation and art-rewiring paths use the same restricted pools.
Existing scenes can be updated using **Tools → Forest Prototype → Remove
Legacy Sitka From Active Stand** (`ForestryAssetSetup.RemoveLegacySitkaFromStand`).
Presentation verification rejects retired living-tree visuals at Years
0/20/50/100 and checks exactly one active display per Sitka, including
same-frame stage and Crop Tree swaps. The interaction gate also checks after
real pruning and save/load. Tree identity and ecological state are unchanged.

Player review identified a separate lower-branch architecture problem in the
benchmark: its unpruned low bole retains only short stubs. The requested art
revision and density-dependent movement requirements are specified in
[`Docs/SitkaPlantationLowerBranchesBrief.md`](../../../Docs/SitkaPlantationLowerBranchesBrief.md).
The ordinary pole/first-thinning revision is now integrated, with scale-aware
pruning and branch-contact movement. See
[`Docs/SitkaPlantation02Integration.md`](../../../Docs/SitkaPlantation02Integration.md)
for ranges, variant coverage and verification.

Materials are explicit URP/Lit: zero metallic and low smoothness. Geometric
grass/rush silhouettes are opaque and double-sided, using the shared green
leaf texture for grass and the exported dry/rush/seed colours. Refined bark
uses the supplied colour/normal maps, normal strength 0.7, smoothness 0.15;
the RGB-only benchmark gloss map is left unassigned to avoid the earlier
blue mirror-shine. Needle sprigs use image alpha, cutoff **0.38**, Render Face
Both and mip-coverage preservation. Cavity wood uses its delivered weathered
fibre map with smoothness 0.02.

All display prefabs are collision-free and have three hard LODs. Tree LOD
thresholds are 0.55/0.20/0.04; vegetation 0.35/0.12/0.015 [D], to be tuned
after rendered review. They introduce no wind shader or animation.

## Review controls

Open `ForestryAssetReview.unity`, press Play and click the Game view.
**WASD** walks, **Shift** runs, mouse looks, **Esc** releases the cursor.
**1/2/3** force LOD0/1/2 for comparison; **0** restores automatic LOD selection.
Bent stages are on the left; cavity stages on the right; grass/rush clumps
are in front. The scene has one walking player/camera/listener and no forest
simulation, ForestTree records or save controller. It cannot modify the
player's scenario save.

The builder preserves an existing review scene layout on re-run. New assets
with differing imported bytes stop for inspection instead of overwriting work.

## Verification

- `RecentDeliveriesAssetSetup.BuildAndWire`: 33 models/11 prefabs; explicit
  catalog and review-scene construction pass.
- Asset validation: grounded metre scale, stable LOD heights, decreasing
  triangle costs, matte materials and needle cutout/normal settings pass.
- `RecentAssetReviewVerification.Begin`: review Play Mode startup, camera,
  player ground collision, all 11 specimens, no missing materials and no
  authoritative simulation/save components pass.
- Four-year habitat presentation and interaction gates pass; frozen archive
  Year-100 hash remains `7AD177B3CC2F73C7`, canonical Sitka remains
  `7E39B70A14959FAD`. The legacy continuation retains its documented P0601
  same-year pruning/felling exception rather than claiming v13 byte parity.
- Main-scene checks confirm both tree families at all four milestones, all
  six stage references on ordinary ForestTrees using temporary size fixtures,
  pruning-compatible Crop Tree display, both rush variants in the stand, and
  unchanged per-tree model assignment after each reference-preview return.

Player review is still needed for actual pixels: cavity depth at 10–20 m,
bend silhouettes, foliage mips, LOD changes and rendered frame time.

Import menu: **Tools → Forest Prototype → Integrate Recent Delivered Grass
and Defect Assets** (`RecentDeliveriesAssetSetup.BuildAndWire`).

Existing prefabs can be enabled/re-wired without reimport via **Tools → Forest
Prototype → Use Recent Assets in Main Game**
(`RecentDeliveriesAssetSetup.WireMainGame`). Fresh scene creation preserves
the main-game wiring when the delivered catalog exists.
