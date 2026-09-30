# Flat ground-moss carpet

`Ground_Moss_Patch_01` completes the moss-only ground-cover presentation as a separate package. It has an irregular cutout perimeter, dense fine-leaf texture, normal/roughness detail and millimetre-scale folded tips. There are no logs, grass blades or solid-green square silhouettes. The original `Woodland_Assets/Moss_Deadwood_Patch_01` is preserved.

## Contents and scale

- Editable `Ground_Moss_Flat_01.blend`, with LOD0/1/2 at identical ground-level roots; assigned images packed. LOD0 initially visible.
- Three separate FBXs in `FBX/`, one mesh and a root each. Metres; Blender +Z up, FBX -Z forward/Y up; clean transforms; no exported camera, lights, animation, colliders or modifiers.
- Reusable 2048² BaseColorAlpha, tangent normal, roughness and Unity metallic/smoothness PNGs. `Ground_Moss_Height_Source.png` is texture-authoring provenance, not a displacement requirement.
- `Moss_Preview.blend` is a separate presentation scene with preview-only floor/camera/light. `Previews/` contains hero, top, close, LOD comparison and low-profile views.
- `Manifest.json`, `Validation.json`, `Geometry_Counts.md`, `Visual_Review.json` and a bundled validator record the exports/review. `Delivery_SHA256.json` is prepared for verified delivery.

The authoring mesh footprint is **2.0 × 1.6 m**; the visible alpha-cut moss area is smaller and organically irregular. LOD0/1 rise less than **15 mm**, LOD2 less than **5 mm**. LOD0/1/2 cost **6,816 / 1,562 / 168 triangles**. LOD1 reduces the small tips, while LOD2 retains the shared carpet texture on a shallow surface. The low-profile geometry and outline are for local ground placement, not a raised cushion or a deadwood object.

## Material setup

Intended URP/Lit setup: **opaque surface with Alpha Clipping**, cutoff **0.45**, **Render Face: Both**. Use `Ground_Moss_BaseColorAlpha.png` as sRGB Base Map with image alpha, `Ground_Moss_Normal.png` as a normal map (approximately 0.65 strength), and `Ground_Moss_MetallicSmoothness.png` as a linear metallic map with smoothness from alpha. Metallic is zero. `Ground_Moss_Roughness.png` is the separate non-colour roughness image used in Blender. Avoid ordinary alpha blending; it can produce sorting issues for layered ground cover.

The bitmap detail is procedurally authored generic fine moss, not a scan or exact moss species. The normal map approximates surface fibre relief; no runtime height/displacement shader, ecology or gameplay state is encoded.

## Placement and review

Place at local soil height; rotate around the ground pivot for variation. The outer mesh perimeter is at Z=0; visible moss edge sits only millimetres above it. The surface is not automatically terrain-conforming: fit placement to uneven ground and avoid steep slopes or large offsets. Alpha clipping hides the support mesh outside the moss outline. Use the delivered texture/material configuration to avoid reverting to a solid rectangle. LOD transitions, mip/alpha coverage, compression and overlapping patch performance require target-project testing.

Hero/top/close, all three LODs and the low-profile view were inspected in offline Blender. All three FBXs independently passed reimport counts/bounds, UV/material/texture references, finite/nondegenerate geometry, ground pivots, source transforms and packed-image checks. This is an asset delivery; Unity was not opened or modified, and the game's placeholder reference has not been replaced here.

## Rebuild

System Python with Pillow runs `author_moss_texture.py`; Blender 4.0.2 with NumPy runs `build_moss.py`. The current temporary NumPy installation is `/tmp/opencode/python-libs`; scripts also retain the historical `/tmp/tree-variation-libs` fallback. Saved Blender/FBX assets do not depend on these temporary directories.

```bash
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python validate_package.py -- .
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python render_moss.py -- hero
```

Other render modes: `top`, `close`, `lods`, `profile`. Render one preview at a time.
