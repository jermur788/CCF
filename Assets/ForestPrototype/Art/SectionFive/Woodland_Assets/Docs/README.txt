WOODLAND ASSET SET — SEVEN ASSETS

SS_Log_Fresh_01: fallen/felled Sitka log with bark, branch stubs and end grain.
SS_Log_Decayed_01: ragged hollow log, dark recessed interior and moss patches.
SS_Stump_01: low Sitka stump, cut surface and irregular buttress roots.
Bracken_Clump_01: tall stalks with triangular, divided upper fronds.
ShadeFern_Clump_01: low rosette of arching, narrow pinnate fronds.
Bramble_Clump_01: arching canes, grouped toothed leaves and small thorns.
Moss_Deadwood_Patch_01: shallow moss cushions, shoots and wood fragments.

DELIVERABLES
Each asset folder contains an editable standalone .blend source, three
separate FBXs (LOD0/1/2), companion .fbm texture folders and a preview.
Textures/ contains five reusable 1024 x 1024 colour PNGs.
Woodland_Preview.blend is a same-scale lineup scene, with preview-only lighting,
camera and neutral ground. Its layout offsets are not present in exports.
Asset_Info.json lists measured dimensions, materials and triangle counts.
Validation.json records the completed export checks. Asset_Summary.md is a
human-readable size/triangle table.

This is an initial stylised/prototype art set. Textures are authored colour
maps, not photographic scans; no normal maps or wind deformation are included.
The shade fern is a generic visual clump, not a species-identification model.
The fresh log/stump have cut end-grain surfaces, while the decayed log has
actual hollow geometry and ragged ends. Moss has thin surface geometry.

SOURCE AND SCALE
One Blender unit is one metre. Each source contains all three LODs with LOD0
visible initially. Source transforms are applied/identity; parent/mesh local
locations are zero. Origins are near the footprint centre at ground level;
the lowest geometry is Z=0. Blender +Z up, FBX -Z forward / Y up.
Log length runs along local X. Standalone sources and exported FBXs are
separate. Export includes only the selected parent and asset mesh, with no
camera, light, animation, collider, preview ground or unapplied modifier.
Preserve the authored pivot when placing and scaling these assets.

UNITY URP MANUAL SETUP
Unity was not opened or modified. No Unity assets were imported or committed.
Import each FBX with its .fbm directory, or assign textures from Textures/.
Create shared URP/Lit materials, using the included PNGs as Base Maps and low
smoothness. Bark, end grain, decayed wood and moss use their named PNGs.
Fern/bracken/bramble leaves share Woodland_Leaf_Albedo.png; stems/canes and
log interiors use ordinary material colours stored in the FBX.
All textures are opaque. Foliage silhouettes come from polygon geometry;
alpha clipping is unnecessary for this set. Set Render Face Both for leaves,
fern leaflets and moss surfaces. Do not enable back-face culling on foliage.
Configure one LODGroup per asset, assigning its LOD0/1/2 mesh renderers at the
same local transform. Choose transition distances and fading in the actual
forest. Lower plant LODs deliberately reduce frond/cane density.
No automatic Unity prefab or LODGroup is provided.

COLLISION AND GAMEPLAY
Visual assets only. Use optional simple box/capsule collisions for logs and
stumps if required by the game. Ground vegetation typically needs none.
No full-mesh collision, harvesting, habitat values, decay state, spawning,
ecological state, inventory or interactions are encoded in these models.
Fresh/decayed are separate visual variants; choose them using explicit asset
references rather than interpreting Blender names as game state.

VALIDATION
All 21 FBXs reimported into fresh Blender scenes. Checks cover triangle counts,
UV layers, nonzero face areas, material assignments, texture files, dimensions,
ground-level geometry, applied source transforms and absence of modifiers.
Open foliage/moss surfaces are intentional and require double-sided materials.
Preview lighting and lineup are not exported. Unity rendering, performance,
LOD transitions and placement on uneven terrain remain untested.
Build/validation scripts require Blender 4.0 and NumPy. Run in a new background
session with -noaudio because scripts clear/open scenes. The build script's
--build-only option saves sources/exports without rendering; render_asset.py
renders saved assets separately.
