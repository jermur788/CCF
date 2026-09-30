# Woodland grass and rush clumps

Five low-growing vegetation assets: three woodland grass tuft shapes and two taller rush clumps. These complement the existing bracken, fern, bramble and moss library. They are generic habitat vegetation, not species-identification models; distribution or habitat suitability is not encoded in the assets.

Grass uses tapered, curved, folded blade geometry and a shared leaf-colour texture. Rush uses slender tapered stems with restrained side seed clusters. A few dry stems/blades add variation. No individual needle geometry or alpha cards are used. Each model has three LODs, an identity-transform ground-level pivot, and metre units. Lowest geometry is at Z=0, +Z up. All models are visual only.

## Delivery

- Woodland_Grass_Rush.blend: editable source, all 15 LOD roots; first grass LOD0 visible initially.
- Vegetation_Preview.blend: separate five-asset lineup with preview-only camera, light and offsets.
- FBX/: 15 selected-object exports, -Z forward / Y up, no lights, cameras, animation, modifiers or colliders.
- Textures/: shared woodland leaf colour. Assigned textures are packed in the main source as well.
- Previews/: same-scale lineup and LOD comparison.
- Manifest.json, Geometry_Counts.md and Validation.json: dimensions, counts and completed export checks.

The lineup runs grass 01, 02, 03, rush 01, 02. In the LOD comparison grass is in front and rush behind; LOD0, 1, 2 run left to right.

## Unity URP

Unity was not opened or modified. Use URP/Lit with low smoothness and zero metallic. Assign the woodland leaf texture to green grass; dry blades, rush stems and seed clusters use the stored material colours. Render grass as double-sided (Render Face Both). Alpha clipping is unnecessary because the silhouettes are geometry. Configure LODGroups separately, keep all local transforms aligned and tune transition distances in the actual forest. No wind shader, placement system, habitat simulation or colliders are supplied.

At distance, LOD2 intentionally uses very few blades/stems. Crossfade and distance choices need in-game review. These are lightweight procedural prototypes, not scans; dense vegetation overdraw, motion and lighting have not been tested in Unity.

## Validation and build

All 15 FBXs were reimported independently to check dimensions, ground alignment, triangle counts, UVs, assigned materials, relative texture paths, finite geometry and nonzero face area. The source has clean transforms and packed assigned images. See Validation.json.

Build scripts use Blender 4.0.2 plus NumPy and the neighbouring Sitka_Mature_Benchmark_01 mesh helpers. The existing Woodland_Assets texture is copied during building; saved sources and FBXs are independently editable. Use fresh background Blender sessions because scripts replace the active document.
