# Scenario 1 — starting-stand asset fix

Presentation-only fix on `integration/scenario-one-complete` (base `e06ef1f`), for rendered-smoke failures in the Year-0 ForestTest stand. No ecology rule, tree position or count, economy, save field, browsing, regeneration or objective changed.

## Root causes

**Sitka ("old Sitka back", "two representations").**
- The intended Sitka presentation was integrated in `/home/jer/CCF` (`feature/forestry-ecology`, base `a31ec62`) but was **never committed**. It existed only as uncommitted working-tree changes (`Docs/SitkaPlantation02Integration.md`: "The import and integration are local, uncommitted work").
- `main` and the Scenario 1 branches therefore kept the committed spawner pools. Those mixed the legacy `Sitka_Mature_01/02/03`, `Sitka_OldLarge_01/02` and `Sitka_Pole_01` families with the `Sitka_Mature_Benchmark_01` family.
- A Year-0 probe found one `PolishedVisual` per tree (336), no second visual child and no foreign tall renderers at tree positions. The "two representations" were therefore two visibly different Sitka families (dark benchmark bark versus pale and grey legacy trunks) interleaved through the same rows, not a doubled hierarchy.

**Moss ("green rug").** The habitat catalog's `groundMoss` was `Ground_Moss_Patch_01` (`Ground_Moss_Flat_01`): a zero-thickness alpha sheet about 3 × 3 m, placed 179 times at Year 0.

**Floor ("flat brown plane").** `Forest Ground.mat` was a flat colour (RGB 0.30 / 0.23 / 0.12) with no texture, on a single 40 × 40 m plane. Vegetation instances do exist, but they are sparse by design at a closed Year-0 canopy (about 30 non-moss items stand-wide).

## Changes

**Sitka.** The uncommitted presentation fix was ported from `/home/jer/CCF`, using a three-way apply of only its presentation files plus the runtime asset closure:
- Spawner pools restricted to `Sitka_Mature_Benchmark_01` (mature) and `Sitka_Young_02` (pole). This is the `ForestryAssetSetup.WireSitkaVisuals` / "Remove Legacy Sitka From Active Stand" change.
- Plantation Sitka 02 render forms for trees under 20 m (`PlantationTreeVisual`, `PlantationVisualCatalog`, `ForestTree.SetPlantationVisuals`, `ForestTreeSpawner.usePlantationVisuals`). These forms are `SS_Plantation_Pole_02` under 12 m and `SS_Plantation_FirstThinning_02` at 12–20 m.
- Year-0 result: 197 `Pole_02` and 139 `FirstThinning_02`, one assembled visual per tree. No legacy family remains in the stand.
- Scenes (`ForestTest`, `MixedSpeciesTest`): spawner field changes only (visual/pole prefab, pruning-base pools, `usePlantationVisuals: 1`).
- `ScenarioHabitatPresentationVerification`: the port's retained-system and contact-registry checks.

Deliberately **not** ported, because they are outside this presentation packet:
- **Player branch-contact movement** (`ForestPlayer` → `PlantationBranchMovement.Resolve`: 35% slowdown and hard blocking in dense young stock [D]). This is a gameplay change. `PlantationBranchMovement.cs` is included only because `PlantationTreeVisual` registers contacts with it; with no player hook it does not affect movement.
- **Compact felling-residue piles** (`ScenarioOneManager`, OpenCode-owned) and the matching interaction-harness check. These were later ported separately; see `Scenario1CompactBrashFix.md`.
- `PlantationIntegrationVerification` (it tests the movement hook), the `_Recovery` folder and unrelated documents.

**Moss.** Adopted `Ground_Moss_Shoots_03` (`Ground_Moss_ShootMat_03`, LOD0/1/2):
- Imported with `SectionFiveAssetSetup.CopyPackage`, which checks SHA-256 against the delivery. Materials follow the delivered spec: opaque double-sided shoot colours; alpha clip 0.40 double-sided tuft and crown atlases; normal 0.55; MS map. Raw-normal bake diagnostics and atlas source `.blend` files are not imported.
- `ScenarioHabitatVisuals` places each existing moss site (same habitat-derived cells and counts) as a deterministic cluster of three cushions (scale 0.7–1.15, ±0.9 m, varied yaw [D]) instead of one wide sheet.

**Floor.** `Forest Ground.mat` now uses the existing project soil texture (`Art/ForestryGround/Textures/Forest_RootPlate_Soil_BaseColor.png`), tiled about every 2.5 m (16×) and tinted toward conifer needle litter [D]. No new floor population was added. Both moss and floor are reproducible with **Tools → Forest Prototype → Scenario 1 Floor Presentation** (`ScenarioOneFloorPresentationSetup.BuildAndWire`).

## External assets adopted (project-authored deliveries)

| Asset | Source | Provenance / licence | Repository size |
|---|---|---|---|
| Plantation Sitka 02 runtime subset | `/media/jer/ZX20/Unity Assets/Sitka_Plantation_02` via the `/home/jer/CCF` import | Project-commissioned Blender build (README, `Delivery_SHA256.json`, 648 manifest hashes verified at import); no third-party licence | about 23.6 MB: baked meshes 13.8, materials/textures 9.3, catalog and prefabs, provenance docs. The 474 source FBXs (re-import only) are not committed; re-import needs the delivery package |
| Ground_Moss_Shoots_03 | `/media/jer/ZX20/Unity Assets/Ground_Moss_Shoots_03` (handoff `Ground_Moss_Shoots_03_Fix`) | Project-commissioned Blender build: "generic authored moss-like turf, not a scan"; SHA-256 delivery manifest; no third-party licence | about 9.6 MB: 3 FBX, 6 textures, docs |

No package dependency was added and Git LFS is still not used. All assets are ordinary Git objects, consistent with existing art.

## Rendered evidence (graphics batch mode, URP)

At player height in Year-0 ForestTest:

| View | Result |
|---|---|
| A. Starting stand | Uniform plantation Sitka |
| B. Close Sitka | Bark, retained lower branches, one tree model |
| C. Several Sitka | Rows of one family; no pale or grey legacy trunks |
| D. Moss | Low porous cushions on textured needle litter, grounded, no rug outline |
| E. Wider floor | Textured needle litter with moss patches and fungi |

There are no pink materials and no floating vegetation. Track and work-clearing materials are unchanged, and markers, juveniles and shelter tubes stay readable (the shelter render is from the earlier smoke).

## Performance

Render-only timing, 20 renders at 1600 × 900 from the player view, batch-mode editor on this machine: about **28–31 ms with moss** against about **23–24 ms without**, for 537 cushions at Year 0. That is about 4–7 ms for moss.

This is not a frame-time profile; LOD0 (48.6k triangles) is used only at close range. If it proves too heavy in interactive play, the mitigations are two cushions per site or a tighter LOD0 threshold. Neither changes ecology.

## Regression

Results are in the handoff:
- canonical lifecycle;
- browsing gate;
- completion gate;
- habitat presentation;
- interaction;
- Reference Future.
