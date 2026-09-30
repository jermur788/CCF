# Refined Sitka bent-stem set

Revision 02, reviewed 29 September 2026. Three independently authored stage architectures with LOD0/1/2: **Young 11 m**, **Mature 26 m**, and **PostMature/older 31 m**. Nine FBXs represent three visual bases, not nine unique trees. These are visual defect-reference trees, with no age, growth, timber grade or habitat rules encoded.

## Shape and scope

The lower/middle stem sweeps laterally, then recovers to an upright upper leader. Branch systems are placed at the bent stem's authored attachment positions; foliage cards are not globally sheared or vertically compressed. Young, mature and older forms have different crown widths, tier counts, root footprints and branch records. Small dead lower stubs remain. Pivots are at ground level with no retained below-ground roots.

This is a new package alongside the original `Sitka_Bent_Variants`; those prototypes are preserved. These trees are defect references and are not designated production crop-tree bases. They have no authored pruning treatments. Eligible production crop trees still use the separate pruning families.

## Files

- `SS_Bent_Refined_Set.blend`: editable asset-only source with nine LOD roots, packed assigned images, identity transforms and no cameras/lights/modifiers/animation. Mature LOD0 is initially visible; reveal other collections/LODs as needed.
- `FBX/`: nine separate exports, each with three meshes (trunk including buttresses, branches, foliage) and one root. Blender +Z up, FBX -Z forward/Y up, metre scale; no exported colliders, cameras, lights or animation.
- `Textures/`: benchmark-family v2 bark colour, tangent normal, roughness, Unity metallic/smoothness, and needle-sprig RGBA atlas. The cut-wood colour map is retained by the isolated benchmark helper but is not used by the exported bent tree.
- `Previews/`: same-scale three-stage lineup, three full-tree LOD comparisons, three lower-stem close views, and benchmark comparison.
- `Preview_lineup.blend` and `Preview_benchmark.blend`: separate staged review scenes. Preview offsets, labels, camera and lights are absent from the main source and FBXs.
- `Manifest.json`, `Geometry_Counts.md`, `Defect_Checks.json`, `Validation.json`, `Defect_Validation.json`, `Visual_Review.json`: export references, bounds/costs and review results.
- `Tools/`: bundled build, render and validation scripts. `Checkpoint_2026-09-29_Paused.md` is historical only.

## Materials for URP

Use the shared benchmark material family. FBX conversion does not establish the intended URP shader setup; assign materials explicitly:

| Material | Setup |
|---|---|
| Bark/branches | Opaque URP/Lit; `Sitka_Bark_BaseColor_v2.png` as sRGB Base Map; `Sitka_Bark_Normal_v2.png` as a tangent normal map, approximately 0.7 strength; `Sitka_Bark_MetallicSmoothness_v2.png` as a linear metallic map with smoothness from alpha. Metallic is 0. |
| Needles | Opaque surface with **Alpha Clipping**, cutoff **0.38**, **Render Face: Both**; `Sitka_NeedleSprigs_BaseColorAlpha.png` as sRGB Base Map, image alpha; low smoothness (approximately 0.14). |

`Sitka_Bark_Roughness_v2.png` is the separate non-colour roughness map used in Blender; the metallic/smoothness texture supplies the Unity convention. The five shared bark/needle files were checked byte-identical to the mature benchmark. The same-lighting offline comparison is `Previews/benchmark.png`: Benchmark left, Bent middle, Cavity right. Bark character, foliage colour and card scale were visually consistent; crown layouts intentionally differ. Included bark normals are approximate authored/baked detail, not a measured scan.

## Scale, placement and LOD

All local roots/transforms are clean. The pivot remains at the footprint origin, rather than following the offset crown. Rotate the whole tree around its base for variation. Dimensions and the 18/48/70 cm approximate authored DBH targets describe art only; they are not biological-age mappings or stage-switch thresholds.

The primary branch identities and stem path are shared within each stage's three LODs. LOD1 uses fewer radial segments and flatter foliage; LOD2 uses fewer/larger sprig clusters and simpler wood. LOD2 is a distance mesh with visible redistribution of individual clusters, not an exact near-view substitute. See `Geometry_Counts.md` for per-export costs and bounds. No billboard, automatic LODGroup, transition distances, wind, collision or interactions are included. Configure placement, transitions and eventual culling in the target project; these source meshes do not automatically conform to terrain.

## Validation and rebuilding

All nine FBXs independently passed technical reimport checks: counts, dimensions, ground bounds, finite/nondegenerate geometry, UVs/materials, referenced textures, source transforms and packed assigned images. `Defect_Validation.json` additionally checks five stem landmarks and the leader offset in every exported LOD, plus cross-LOD landmark agreement. Every stage's full-tree LOD comparison and lower-stem close view was inspected offline.

From the package folder, with NumPy available to Blender Python:

```bash
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/validate_package.py -- .
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/validate_refined_defects.py -- .
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/render_refined_defects.py -- . lineup
```

Use `lod_Young`, `lod_Mature`, `lod_PostMature`, `detail_Young`, `detail_Mature`, `detail_PostMature` or `benchmark` instead of `lineup` for other views. Run one render at a time. Rebuilding via `build_bent.py` and texture-identity validation require sibling `Sitka_Mature_Benchmark_01/`. Rerendering the benchmark comparison also requires sibling `SS_Cavity_Refined_Set/`. The saved sources, packed preview scenes and FBXs are independent of those script-time dependencies. The scripts retain the historical optional NumPy path `/tmp/tree-variation-libs`.

**Unity was not opened or tested.** Actual URP import, 10–20 m game-view readability, mip/compression behaviour, LOD fading, wind/collision and forest performance remain unverified.
