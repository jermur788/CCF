SITKA_MATURE_01 — Unity-compatible export, no Unity changes

Mature Sitka spruce prototype interpreted from the generated reference sheet.
Approximate height: 28 metres. Single tapered trunk, irregular branch tiers, sparse lower branches, needle-card crown and root flare.

Contents:
- Sitka_Mature_01.fbx: bark/branch mesh and foliage mesh only.
- Sitka_Bark_Albedo.png: 2048 x 2048 baked bark base colour.
- Sitka_Needles_Albedo.png: 1024 x 2048 needle-shoot image with transparency.
- Sitka_Mature_01.fbm: FBX texture copies; keep alongside the FBX.
- Sitka_Mature_01.blend: editable source, including the needle-shoot scene used to render the foliage texture.
- Preview and original modelling reference sheet.

Manual material setup after import:
Use URP/Lit if your Unity project uses URP. Assign the corresponding image to each material's Base Map.
Bark: opaque, low smoothness.
Needles: opaque with Alpha Clipping enabled, threshold about 0.35, Render Face Both. Preserve the PNG alpha channel. Do not render the foliage as ordinary opaque rectangles.

Mesh count: 2. Total triangles: 49,888 (27,264 bark/branches; 22,624 foliage).
Metre-scale model, ground-level origin, Y-up FBX export.
No LOD Groups, collision, wind, growth stages or chopping scripts are supplied. This package contains the mature tree only.
This is a modelling prototype inspired by the reference, not a photogrammetric or botanically measured specimen. Base-colour textures are included; no normal maps are supplied.

The FBX was re-imported and rendered in Blender to check geometry, texture links, UVs and scale. Unity has not been opened or modified, and Unity rendering/gameplay are untested.
