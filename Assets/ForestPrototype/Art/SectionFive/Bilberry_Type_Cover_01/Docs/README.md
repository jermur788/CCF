# Dwarf shrub / bilberry-type cover

A low, multi-stem green shrub with real branching twigs, rounded oval leaves and a few small dark-blue berries with calyx detail. It replaces the pointed green-blue placeholder presentation. It is a generic bilberry-type visual, not a botanical identification model or a berry-production state.

## Delivered asset contents

- `Bilberry_Type_Cover_01`: LOD0/1/2.

Editable `Bilberry_Type_Cover_01.blend` is asset-only with packed assigned textures and clean transforms. Separate FBXs are under `FBX/`; each contains one mesh and a root, metre scale, Blender +Z up and FBX -Z forward/Y up. No cameras, lights, animation, modifiers or colliders are exported.

`Preview_hero.blend` and PNGs under `Previews/` are separate staged presentations. Ground, camera, lights and context props are preview-only. `Manifest.json`, `Validation.json`, `Geometry_Counts.md` and `Visual_Review.json` describe exact references, measured bounds/costs and review scope. `Delivery_SHA256.json` is prepared for verified delivery.

## Materials and placement

Foliage is opaque geometry and requires Render Face Both; alpha clipping is unnecessary. Berries are decorative geometry and are reduced/omitted at distance. All LODs retain rounded leaf outlines. Rotate/scale around the ground pivot for variation; terrain fitting and LOD distances are supplied by the target project.

See `Materials_URP.md` for every material and reusable texture map. The art contains no growth, age, habitat value, timber value, decay timer or spawning logic. Authored dimensions describe only these meshes.

## Review and scope

All exports independently passed reimport counts/bounds, finite/nondegenerate geometry, UV/material and package-relative texture references, ground pivots, source transforms and packed assigned images. Hero/detail and LOD comparisons were inspected offline. These are completed standalone art packages; Unity has not been opened/modified and the game-side placeholder references have not been wired here. URP behaviour, LOD transitions, wind/collision, terrain placement and runtime performance remain untested.

## Rebuild and render

Blender 4.0.2 with NumPy and system Python with Pillow are required for scripts. NumPy is currently installed temporarily under `/tmp/opencode/python-libs`; historical `/tmp/tree-variation-libs` is also searched. Saved `.blend`/FBX files do not depend on either temporary path. Bundled Tools are independent of the benchmark tree helper.

From this package folder:

```bash
python3 Tools/author_habitat_textures.py bilberry
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/build_habitat_assets.py -- bilberry
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python validate_package.py -- .
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python Tools/render_habitat_package.py -- . hero
```

Other render modes: `close <asset>`, `lods <asset>`, and `underside <asset>`; fungi also has `context`, requiring sibling `Woodland_Assets/` for the existing staged log. Render serially. Rebuild in an editable workspace revision, preserving prior delivered packages.
