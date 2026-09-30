# Mature Sitka benchmark — refinement 01

A separate, refined mature Sitka asset for reviewing the visual standard of the CCF tree family. Existing trees and Unity projects were not changed. The model is authored at approximately **26 m** tall, with an approximately **48 cm stem diameter at 1.3 m**, metre units and a soil-level pivot. These measurements describe the art only; they do not define simulation state.

## What changed

- A less regular crown, with varied branch spacing, asymmetry, drooping lateral shoots and foliage clusters angled through the crown.
- Four needle-sprig images baked from detailed 3D shoots into a reusable 2K alpha atlas. Exported trees use cards; they do not contain individual needle geometry.
- A generated natural bark base colour, with a tangent normal map baked from its tonal detail, a roughness map and a Unity metallic/smoothness channel-packed map. The relief is an artistic approximation, not measured scan data. Bark UVs use a consistent physical texture scale.
- Short lower dead-branch remnants in the unpruned tree; these become visible management scars in the pruned states, including near walking height.
- Elliptical cut collars, exposed end-grain and shallower healed scars with smaller centres. Healed scars retain management history without leaving large tube-like cavities.
- Three purpose-built LODs. Close geometry retains bent foliage cards and selected woody lateral shoots; lower LODs simplify those features and retain the cleared-bole silhouette.

This is a refinement of the mature Sitka family, with the same 26 m authoring scale as `Sitka_Mature_02`. It is a new revision of the architecture, not a geometry-identical replacement. `Previews/before_after.png` shows the previous mature 02 on the left and this benchmark on the right.

## Pruning delivery

Seven treatments, each at LOD0/1/2, give **21 FBX exports**:

- Unpruned.
- LowPruned — Recent / Healed, clearance limit 6.7 m.
- MediumPruned — Recent / Healed, clearance limit 8.2 m.
- HighPruned — Recent / Healed, clearance limit 9.8 m.

The three clearance limits are visual model states, not recommendations for forestry operations. State switches retain the trunk, pivot and upper crown. The unpruned tree's lower dead-branch attachment sites provide the locations of the low-bole pruning scars. Recent/healed treatments express scar appearance only, not a specified number of elapsed years.

`Manifest.json` maps every state, treatment and LOD to an explicit FBX filename and triangle count. It also preserves branch records for offline authoring. Do not parse names or mesh bounds to infer biological age or management decisions in the simulation.

## Source files

- `Sitka_Mature_Benchmark_01.blend`: editable tree, all 21 state/LOD roots at the same pivot. Unpruned LOD0 is visible initially; other objects are hidden to avoid overlap.
- `Pruning_Preview.blend`: separate four-state lineup with preview-only offsets, camera and lighting.
- `Bark_Material_Bake_Source.blend`: editable material-baking setup for the bark normal map.
- `Needle_Atlas_Source.blend`: detailed shoot geometry and lighting used only to bake the foliage atlas. **Do not import this source as a tree asset.**
- `FBX/`: selected tree objects only, -Z forward / Y up, applied rotation/scale, no cameras, lights, animation, colliders or modifiers.
- `Textures/`: portable PNG textures; the main Blender source also packs its assigned textures.
- `Previews/`: full tree, before/after, foliage and bark close-ups, recent/healed scars, pruning states and LOD comparison.

In the scar comparison, recent is left and healed is right. State lineups run unpruned → low → medium → high from left to right. The LOD lineup runs LOD0 → LOD1 → LOD2.

## Later Unity URP setup

Unity was not opened or modified. Keep `FBX/` and `Textures/` together when copying this package.

| Material | Setup |
|---|---|
| Bark | URP/Lit; assign bark BaseColor, Normal (import as a normal map, initial strength 0.7) and MetallicSmoothness. Set smoothness source to metallic alpha, smoothness multiplier 1, metallic 0. |
| Needles | URP/Lit, opaque surface with alpha clipping, starting threshold 0.38; Render Face Both; assign BaseColorAlpha atlas; low smoothness. Retain alpha and use mipmaps. |
| Recent cut wood | URP/Lit, CutWood BaseColor, metallic 0 and low smoothness. |
| Healed scar centre | Restrained weathered brown/grey, metallic 0, low smoothness. |

The metallic/smoothness map stores zero metalness in RGB and `1 - roughness` in alpha. Import this data map with sRGB disabled. Use the `_v2` bark maps; earlier procedural bark maps are superseded. The separate roughness map is provided for Blender and other pipelines. No custom Unity shaders or packages are required for the static material setup.

Configure one LODGroup per treatment and choose state assets through explicit references. Suitable switching distances depend on the camera, resolution and forest density; they have not been validated in Unity. The asset does not yet contain a tested wind shader, animation, interaction or collision setup. Runtime wind and forest performance remain separate integration work.

## Verification and limits

All 21 FBXs are independently reimported and checked for mesh/triangle counts, UVs, finite geometry, zero degenerate faces, texture paths, metre height and ground alignment. The source is checked for 21 roots with clean transforms. At each LOD, retained geometry above 12 m is compared across all pruning treatments and is identical. See `Validation.json` for results.

Blender previews were inspected for canopy volume, pruning readability and scar treatment. This is an authored procedural benchmark, not a photogrammetry scan. It is ready for visual review and import testing; production acceptance, wind response, alpha/mipmap behaviour, LOD transitions and frame time still require in-game review. LOD0 is intended for close inspection, not for every tree across an entire forest.

Botanical visual reference: [Forestry England — Sitka spruce](https://www.forestryengland.uk/article/sitka-spruce), including its description of grey-brown scaly mature bark; [Forest Research — Sitka spruce](https://www.forestresearch.gov.uk/tools-and-resources/tree-species-database/131584-sitka-spruce-ss-2/) for plantation species context. These references guide appearance rather than prescribe the model's pruning heights.

Build scripts use Blender 4.0.2 and NumPy. They replace the active Blender document, so run them in fresh background sessions with `-noaudio`. Direct editing of the saved `.blend` does not require those scripts. The before/after render additionally references the previous asset in the original workspace.
