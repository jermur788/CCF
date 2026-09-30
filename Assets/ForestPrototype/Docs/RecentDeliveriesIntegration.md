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
  family (roughly 10% bent, 10% cavity); existing height/DBH selects the
  renderer stage [D]. Trees retain their simulation IDs, positions, physical
  state and biological rules. No disease, cavity development, timber penalty
  or habitat bonus is simulated. Year 0 has 32 bent and 29 cavity looks;
  the frozen Year-100 preview has 13 and 10 respectively.
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
