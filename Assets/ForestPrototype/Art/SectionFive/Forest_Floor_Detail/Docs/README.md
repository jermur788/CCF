# Forest Floor Detail

Reviewed completion revision, 29 September 2026. Five static forest-floor assets, each with three separate FBX LODs: oak, beech and mixed leaf-litter patches, plus two small-deadwood layouts. Authored dimensions describe meshes only; these assets contain no ecological or gameplay rules.

## Contents

- `Forest_Floor_Detail.blend`: editable asset-only source, packed assigned textures, 15 ground-level roots. Only the first LOD0 patch is initially visible; other collections/LODs can be unhidden for editing.
- `FBX/`: 15 exports; one mesh plus a root per file. Metre scale, clean source transforms, Blender +Z up, FBX -Z forward/Y up. No cameras, lights, animation or colliders are exported.
- `Textures/`: reusable leaf atlas and shared procedural decayed-wood colour texture.
- `Forest_Floor_Preview.blend`: separate staged LOD0 lineup with preview ground, labels, camera and light. Use the main source or FBXs for assets.
- `Previews/`: full lineup, mixed-leaf close view, deadwood close view, mixed-leaf/deadwood LOD comparison, low-angle ground profile and offline atlas mip review.
- `Manifest.json`, `Validation.json`, `Geometry_Counts.md`: export list, reimport checks and costs.
- `Delivery_SHA256.json`: checksum list prepared for delivery.

## Leaf material

Use **`Broadleaf_Litter_BaseColorAlpha_v2.png`**, a 1254 × 1254 RGBA, 2 × 2 atlas. In Blender UV coordinates, oak occupies tiles 0/2 (left column) and beech tiles 1/3 (right column). Leaf tips follow +V within each tile. The original unsanitised image is retained for provenance but is not assigned.

For the intended URP material setup: use the atlas as sRGB Base Map with alpha from the image, opaque surface with **Alpha Clipping**, cutoff **0.40**, and **Render Face: Both**. Use metallic 0 and approximately 0.06 smoothness. Leaves have upward-facing top normals and require double-sided rendering. FBX material conversion does not reliably recreate this shader configuration; these are setup instructions, not an import result.

The completion revision removed saturated red generated fringe pixels from alpha and extended valid leaf RGB 16 pixels into the borders. Assigned images are packed in the Blender source. Offline RGBA box-filtered mip images were checked at full size and 512/256/128/64/32 pixels; no strongly red pixels survive the 0.40 cutoff at those sizes. Small mip levels naturally lose fine stems/lobes. Keep alpha-aware mip generation/coverage preservation in mind when setting up the target material; texture compression, filtering, distant shimmer and actual Unity mip behaviour remain untested. No normal, wind or displacement map is supplied for the leaves.

## Deadwood material and placement

`Woodland_DecayedWood_Albedo.png` is a shared, opaque colour texture. Use metallic 0, high roughness and double-sided rendering for the thin bark fragments. Branches are low-poly tapered tubes; side branches intersect the curved parent stem. Each twig system now touches the authored ground plane instead of being suspended by a package-wide height offset. Overlaps between different systems are intentional static scatter intersections, not a physically settled simulation. Bark fragments are thin curved polygons, not solid pieces.

All roots are at `(0, 0, 0)` with the lowest vertex at ground level. Leaf patches are roughly 1.46–1.48 m wide and 1.13–1.18 m deep at LOD0, with approximately 2 cm of vertical layering. Twig scatters are roughly 1.82–1.96 m wide and 4.9 cm high. Place on local ground, rotate around the vertical axis and use mild scale variation. The geometry does not conform automatically to uneven terrain; large slopes/bumps require placement adjustment or authored fitting. Sparse litter is intended to overlay a forest-floor ground material, with gaps showing that material.

## LODs

Leaf LOD0 has 150 curled cards with three longitudinal segments (900 triangles); LOD1 retains the same cards with two segments (600); LOD2 retains 100 slightly larger, flat cards (200). Twig LOD0/1 use the same layout with reduced radial detail; LOD2 reduces both twig and side-branch counts. See `Geometry_Counts.md` for all measured bounds and costs. Exports do not contain an automatic LODGroup, billboards or gameplay/distance thresholds. Configure transitions and eventual culling in the target project.

## Review and reproducibility

Visual review covered every LOD0 asset in the lineup, close leaf edges/orientation, twig joins/fragments, low-angle placement and a mixed-litter/deadwood LOD0/1/2 comparison. These are procedural art assets, not scans or exact species-identification models.

Before repairs, the original source, exports, materials, build script and technical reports were preserved in the workspace sibling `Forest_Floor_Detail_Handoff_2026_09_29/`. Final changes are documented in the manifest and `Atlas_Repair.json`.

Run technical validation from any location with an absolute package path:

```bash
blender -t 6 -noaudio --background --factory-startup --python validate_package.py -- "/path/to/Forest_Floor_Detail"
```

The bundled validator checks all exports, finite geometry, nondegenerate faces, counts, UV/material presence, texture paths, ground bounds, source transforms and packed assigned images. It requires NumPy in Blender Python; its historical `/tmp/tree-variation-libs` path is optional if NumPy is installed normally.

To rerender, use Blender 4.0.2 and run `render_floor.py -- lineup`, `leaves_close`, `deadwood_close`, `lods` or `ground_profile` serially. The atlas review/repair scripts use Python Pillow. Rebuilding with `build_floor_detail.py` additionally requires sibling `Sitka_Mature_Benchmark_01/build_benchmark.py` and `Woodland_Assets/Textures/Woodland_DecayedWood_Albedo.png`. Saved `.blend` and FBXs are independent of those build-time dependencies. The repair/archive script is for provenance and is not required for import.

**Unity has not been opened or tested for this package.** URP import behaviour, game-view readability, mip/compression settings, LOD transitions, wind, collisions and forest performance are unverified.
