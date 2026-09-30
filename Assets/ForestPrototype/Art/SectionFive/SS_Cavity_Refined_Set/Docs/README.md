# Refined Sitka cavity/stem-wound set

Revision 02, reviewed 29 September 2026. Three independently authored stage architectures with LOD0/1/2: **Young 11 m**, **Mature 26 m**, and **PostMature/older 31 m**. Nine FBXs represent three visual bases. Stage labels and cavity dimensions describe art, not cavity development by biological age or any gameplay rule.

## Cavity construction

Each tree has a genuine lower-stem recess facing Blender **-Y**, with an irregular elongated opening, retained back wall and a shallow raised bark callus. The recess is cut into the stem mesh, not drawn as a dark flat card. The Boolean is applied and the temporary cutter is removed. The cavity's connected stem component remains closed; the overlay callus is intentionally an open surface embedded at its outside edge.

The callus follows the real cut boundary. Its UVs are sampled barycentrically from the uncut bark surface so the grain aligns with adjacent bark. A new procedural weathered-fibre colour texture supplies exposed wood; broad regular stripes from the initial draft were replaced with uneven fibres, flecks and short cracks. This is authored art, not a scan or an exact biological cavity model. It is a blind stem recess, not a hollow tube throughout the whole bole.

| Visual base | Height | Opening centre Z | Approximate opening width × height | Authored centre recess |
|---|---:|---:|---:|---:|
| Young | 11 m | 0.85 m | 0.095 × 0.34 m | 0.055 m |
| Mature | 26 m | 1.50 m | 0.25 × 0.92 m | 0.16 m |
| PostMature/older | 31 m | 1.90 m | 0.38 × 1.38 m | 0.23 m |

These opening sizes are authoring targets; irregular cut intersections produce slightly different measured silhouettes. Five rays per exported LOD confirm substantial recession, interior material, retained backing and unchanged rear surface. All three LODs retain the wound, rather than replacing the distant cavity with a painted mark. Full-tree visibility of the small young wound depends on distance and camera angle.

The original `Sitka_Hollow_Variants` is preserved. The new set includes a genuine young plantation-height tree and an older tree, and fixes the original mature below-ground root extension through newly authored ground-level geometry. These are defect-reference trees, not designated production crop-tree bases; no pruning treatments, habitat values, timber rules, decay timing or simulation state are supplied.

## Files

- `SS_Cavity_Refined_Set.blend`: editable asset-only source, nine LOD roots, packed assigned images, clean transforms and no cameras/lights/modifiers/animation. Mature LOD0 initially visible.
- `FBX/`: nine exports; each has trunk/callus/buttress geometry in one mesh, a branch mesh, foliage mesh and a root. Metres, Blender +Z up, FBX -Z forward/Y up; no exported preview objects or colliders.
- `Textures/`: shared refined benchmark bark/needle family and `Sitka_Cavity_DecayedWood_BaseColor.png`, the new 1024² procedural cavity-wood colour map. The helper's cut-wood colour map is retained but is not assigned to exported cavity surfaces.
- `Previews/`: full three-stage lineup, full-tree LOD comparison for each stage, close cavity LOD comparison for each stage, oblique mature detail, and a copy of the benchmark comparison PNG.
- `Preview_lineup.blend`: separate review scene. The shared benchmark comparison scene is delivered with the bent set.
- `Manifest.json`, `Geometry_Counts.md`, `Defect_Checks.json`, `Validation.json`, `Defect_Validation.json`, `Visual_Review.json`: exact export references, technical results and visual-review scope.
- `Tools/`: bundled authoring/render/validation scripts.

## Materials for URP

| Material | Setup |
|---|---|
| Bark/branches/callus | Opaque URP/Lit; `Sitka_Bark_BaseColor_v2.png` as sRGB Base Map; `Sitka_Bark_Normal_v2.png` as a tangent normal map (approximately 0.7 strength); `Sitka_Bark_MetallicSmoothness_v2.png` as a linear metallic map, smoothness from alpha; metallic 0. |
| Needle sprigs | Opaque surface with **Alpha Clipping**, cutoff **0.38**, **Render Face: Both**; `Sitka_NeedleSprigs_BaseColorAlpha.png` as sRGB Base Map with image alpha; low smoothness (approximately 0.14). |
| Cavity wood | Opaque URP/Lit; `Sitka_Cavity_DecayedWood_BaseColor.png` as sRGB Base Map; metallic 0, smoothness approximately 0.02. No separate cavity normal map is supplied. The cavity shape supplies actual depth. |

The trunk has two material slots; branches and foliage each have one. FBX conversion does not recreate all URP shader settings, so explicit material setup remains required. `Sitka_Bark_Roughness_v2.png` is the separate Blender roughness map; Unity uses the supplied metallic/smoothness convention. The five shared bark/needle files were checked byte-identical to the benchmark, and the same-lighting offline comparison was inspected: Benchmark left, Bent middle, Cavity right. Bark normals are authored/baked approximations.

## Placement and LOD

Roots are at ground-level `(0,0,0)` with identity transforms and lowest geometry at Z=0. Rotate the whole tree to vary cavity orientation. Approximate 18/48/70 cm authored DBH targets and all mesh bounds are visual dimensions only.

LOD0/1/2 share each stage's branch records and wound parameters. LOD1 reduces wood radial detail and flattens foliage cards; LOD2 uses fewer/larger sprig clusters and simpler wood/cavity topology. LOD2 is for distance: individual cluster placement and the callus faceting visibly differ up close. Counts and measured bounds are in `Geometry_Counts.md`. No automatic LODGroup, transition distances, billboard, wind, collider or ecological/gameplay component is included. Choose placement and terrain fitting in the target project; no terrain holes are created.

## Validation and rebuilding

All nine final FBXs independently passed the generic reimport checks for counts/bounds, UV/material presence, finite coordinates, nondegenerate faces, relative texture references, ground pivots and clean packed source assets. Additional exported-mesh checks confirm five cavity depth probes per LOD, correct exposed-wood material, retained backing, unchanged rear surface and the closed connected stem component. Visual review covered all stages/LODs, close rims/UVs, interior appearance and oblique depth.

From the package folder, with NumPy available to Blender Python:

```bash
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/validate_package.py -- .
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/validate_refined_defects.py -- .
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/render_refined_defects.py -- . detail_Mature
```

Other modes: `lineup`, `lod_Young`, `lod_Mature`, `lod_PostMature`, `detail_Young`, `detail_PostMature`, `oblique_Mature`, `benchmark`. Render serially. Rebuilding via `build_cavity.py` and texture-identity validation require sibling `Sitka_Mature_Benchmark_01/`; benchmark rerendering also needs sibling `SS_Bent_Refined_Set/`. The saved source/preview and FBXs are independent of those script-time dependencies. Scripts retain the historical optional NumPy path `/tmp/tree-variation-libs`.

**Unity was not opened or tested.** URP import, 10–20 m game-view readability, alpha/mip/compression settings, LOD fading, wind, collision and forest performance remain unverified.
