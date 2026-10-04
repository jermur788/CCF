# Corrected ground-moss mat — shoot/cushion revision 03

`Ground_Moss_ShootMat_03` replaces the flat rug/paint-cutout presentation with **visible small stems/leaves, crossed tuft clusters and overlapping domed tuft crowns**. It has no broad horizontal support mesh. The previous `Ground_Moss_Flat_01` and `Ground_Moss_Natural_02` deliveries are preserved.

## What changed

- Real three-dimensional moss shoots supply near detail; small crossed tuft clusters retain structure farther away.
- The top crowns overlap into uneven low cushions instead of reading as sparse weeds or flat green stains.
- RGBA side/top tuft textures and tangent-normal atlases are baked from **104 editable modelled shoots**, rather than painted fern/star silhouettes. Palette variation includes dark stems, greener tips and restrained dry accents.
- Individual tuft heights and placement vary, with soil gaps and a porous fringe. Extreme fringe sites are retained through all LODs to avoid a shrinking patch outline.

This is generic authored moss-like turf, not a scan or exact moss-species identification. There is no runtime ecology, growth, habitat value or gameplay behaviour encoded.

## Package

- Editable `Ground_Moss_Shoots_03.blend`: three clean ground-level LOD roots, packed assigned textures; LOD0 initially visible.
- Three separate FBXs in `FBX/`, one mesh and root each; metres, Blender +Z up, FBX -Z forward/Y up; no exported cameras/lights, animation, colliders or modifiers.
- Reusable 2048² side/top RGBA and normal atlases, roughness and Unity metallic/smoothness maps.
- `Moss_Tuft_Atlas_Source.blend` and `Moss_Crown_Atlas_Source.blend`: **high-detail baking sources only**, with bake-only camera/light. Do not import them as game mats. `*_Normal_Raw.png` files are bake diagnostics; use the prepared normal maps listed in the manifest.
- `Preview_hero.blend`, `Preview_comparison.blend`, six rendered review PNGs and a contact sheet. Comparison is initial rug left / earlier broken cutouts middle / corrected shoot mat right, at their authored scales under the same light.
- `Manifest.json`, technical/structure validation, geometry costs, visual review and delivery checksum.

Authored footprint is approximately **1.22 × 0.97 m**. The low tuft/card geometry rises approximately **29–35 mm** across the LODs; the visible shoot canopy is lower than the transparent card bounds. Costs: **48,640 / 8,554 / 2,760 triangles** at LOD0/1/2. LOD0 is the near-detail asset; LOD1/2 reduce shoots/card subdivisions and retain the footprint. Counts do not measure alpha overdraw or engine performance.

## Materials for the coding agent

The four small shoot materials are opaque, double-sided, matte colours recorded in linear RGB in `Manifest.json`. Both atlas tuft materials use opaque URP/Lit with **Alpha Clipping 0.40**, **Render Face: Both**. Assign the matching side or crown RGBA atlas as sRGB Base Map with image alpha, the corresponding prepared normal as a normal map (approximately 0.55 strength), and `Moss_Tuft_MetallicSmoothness.png` as a linear metallic map with smoothness from alpha. Metallic is zero, smoothness approximately 0.067. `Moss_Tuft_Roughness.png` is the separate Blender roughness map.

Prepared atlas RGB is extended 14 pixels into transparent borders; alpha is retained. Normals use the forward hemisphere expected for normal-map decoding. Explicit URP material assignment is required; FBX conversion does not establish clipping/double-sided settings. Ordinary alpha blending is not the intended setup.

Keep the root at local ground level and fit to terrain rather than raising the mat as a slab. No automatic terrain conformity, wind, collider, prefab or LODGroup is supplied. Source textures have no baked ground plane. Unity placeholder/material wiring stays with the coding agent; Unity rendering, mips/compression, LOD fading and runtime performance remain untested.

## Review and rebuilding

Hero, close-up, top, low-profile, LOD and old/new comparison renders were inspected offline. All three FBXs independently passed reimport counts/bounds, finite/nondegenerate geometry, UV/material/relative-texture references, source transforms and packed-image checks. `Structure_Validation.json` additionally verifies small geometry primitives, near opaque shoots, low height and absence of a broad support plane.

Rebuild order: Blender `bake_atlas.py`, Blender `bake_top_atlas.py`, system Python/Pillow `prepare_atlas.py`, Blender `build_moss.py`. Blender 4.0.2 and NumPy are required for export/validation; the workspace dependency is `Forest_Asset_Tools/python-libs`, with a local `Tools/python-libs` alternative. `Tools/habitat_mesh.py` is bundled. Saved Blender/FBX files do not depend on these Python directories.

Render serially with `blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python render_moss.py -- hero`; other modes are `close`, `top`, `profile`, `lods`, `comparison`. The comparison rerender requires the two preserved sibling moss packages. Rebuild in a new workspace revision, preserving delivered packages.
