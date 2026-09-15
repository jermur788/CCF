SITKA_POLE_01 — visual-only Sitka spruce pole-stage asset

Authored height: 8 metres. Blender uses metres, +Z up, origin at trunk centre
at ground level. Straight central leader. Trunk diameter at 1.3 m is 0.15 m.
Crown begins around 1.7–1.9 m. Lower branches are retained and shorter.
All three versions use the same branch layout with irregular whorls.

LOD0: 10,552 triangles
LOD1: 4,210 triangles
LOD2: 1,390 triangles
Each FBX contains one named parent and Trunk, Branches, Foliage meshes.
Exactly two material slots across those meshes: bark and foliage.
Only trunk and major branches use woody geometry. Needles/twigs are depicted
in the reused texture, not modelled individually. Foliage uses clustered,
shared-UV single-sided cards intended for a double-sided alpha-cutout material.
LOD reduction changes branch segmentation and card density, retaining branch tips.

FILES
Sitka_Pole_01.blend: editable source; LOD0 visible by default, LOD1/2 hidden.
Sitka_Pole_01_LOD0.fbx / LOD1.fbx / LOD2.fbx: separate selected-object exports.
Sitka_Pole_Bark_Albedo.png: 1024 x 1024, fine young-bark colour texture.
Sitka_Needles_Albedo.png: 1024 x 2048 RGBA, reused from the existing Sitka asset.
Matching .fbm folders: portable copies of linked textures; retain beside FBXs.
Preview PNGs, Asset_Info.json and Validation.json.
Build and validation scripts are supplied for reproducibility with Blender 4.0
and NumPy available to its Python. Scripts create a fresh scene; use background
Blender or a new document rather than running in an unsaved working file.

MANUAL UNITY URP SETUP (Unity has not been opened or modified)
Import the FBX files and PNGs. Use URP/Lit for both materials.
Bark: opaque, Base Map = Sitka_Pole_Bark_Albedo.png, low smoothness.
Foliage: opaque surface with Alpha Clipping ON, threshold about 0.35,
Render Face = Both, Base Map = Sitka_Needles_Albedo.png. Preserve source alpha.
Use the same two materials for all LODs. FBX does not configure URP shaders.
Place the three imported roots at the same local position under one parent
and assign their mesh renderers to a Unity LODGroup; tune distances in-game.
The files do not include an automatically configured Unity LODGroup.
For authoritative height H in metres, uniform parent scale = H / 8.
Requested 3.5–12 m range corresponds to scales 0.4375–1.5.
Do not infer simulation age, DBH, crown radius, competition or growth from
this visual asset. No biological-state logic or gameplay code is included.

EXPORT / VALIDATION
FBX uses -Z forward / Y up, metre scale, selected asset objects only, no
animation, no camera/light/collider export and no procedural shader dependency.
Source meshes have identity rotation and scale, zero local translation.
Bark/branch surfaces use smooth shading with angle-based normal handling.
Foliage UV overlap is intentional; there are no duplicated back-facing cards.
Each FBX is reimported independently into Blender to check height, ground
position, triangle budget, materials, UVs, texture paths and alpha channel.
Export-derived side and LOD2 previews accompany the source LOD0 preview.
Unity rendering, LOD transitions and performance are untested. This is a
prototype foliage texture, not a photographic scan; inspect it at your intended
camera distance. No normal maps, wind or collision meshes are supplied.
