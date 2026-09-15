SITKA_SEEDLING_01 — visual-only Sitka spruce seedling / young sapling

SOURCE AND EXPORTS
Sitka_Seedling_01.blend is the editable source. Separate LOD0, LOD1 and LOD2
FBXs are provided for Unity. Nothing has been imported into Unity or committed
to a repository. The existing pole-stage package likewise retains a separate
.blend source and three FBX exports.

Authored natural height: 1 metre; Blender unit scale: 1 metre.
Ground-level origin at central stem; stem bottom Z=0; +Z up in Blender.
Source rotations and scales applied; zero object locations.
Thin tapered stem, retained low branches from about 7–11 cm, short irregular
whorls, compact foliage and a terminal leader. No individual needle geometry.

LOD0: 2,188 triangles; Stem, Branches, Foliage meshes.
LOD1: 797 triangles; Stem, Branches, Foliage meshes.
LOD2: 158 triangles; Stem and Foliage meshes, no branch geometry.
LOD1 retains outer foliage cards. LOD2 uses sparse cards in varied directions.
All stages retain the 1 m stem/leader height and soil-level pivot.
LOD0 is visible in the Blender source; LOD1/LOD2 start hidden.
Preview camera and light are included in the .blend only, never in the FBXs.

MATERIALS
Two materials: simple stem and alpha-clipped foliage.
Sitka_Pole_Bark_Albedo.png: 1024 x 1024, reused fine young-stem texture.
Sitka_Needles_Albedo.png: 1024 x 2048 RGBA, same Sitka foliage texture family
as the pole-stage and mature assets. Shared foliage UVs overlap intentionally.
Wood uses smooth shading with angle-based normal handling. Cards are open
surfaces intentionally; double-sided rendering avoids duplicate back faces.
There are no unapplied modifiers, procedural shader dependencies, animation,
colliders or gameplay components in the exported files.

MANUAL UNITY URP SETUP
Import FBX files with their accompanying .fbm folders/textures.
Use URP/Lit, assigning the respective textures to Base Map.
Stem: opaque, low smoothness.
Foliage: opaque surface, Alpha Clipping enabled (start at 0.35), Render Face
Both. Preserve the PNG alpha channel. Use the same two materials across LODs.
Create a parent and LODGroup, placing all three LOD roots at local zero, and
assign the corresponding renderers. Tune LOD distances and fading in-game.
An automatically configured Unity prefab/LODGroup is not supplied.

SIMULATION SEPARATION
This is art only. Switch heights, biological state and aggregated regeneration
remain entirely in Unity configuration/code. Do not use Blender object names
or crown/mesh dimensions as biological state. Use explicit visual-stage asset
references, the shared ground pivot and measured natural height.
For desired height H metres, uniform visual scale is H / 1 m. Thus the intended
0.3–3 m range uses scales 0.3–3.0. No stage-switch thresholds are encoded.
Only add source/art files to the project or version control after deciding
which belong there; these exports do not change the Unity project.

VALIDATION AND LIMITATIONS
All FBXs independently reimported into Blender: metre height, ground pivot,
triangle budget, material counts, UVs, nonzero face areas, texture files and
alpha range checked. Source transforms and lack of modifiers checked.
Front and exported side/LOD2 previews are included. Unity rendering, LOD
transitions and performance remain untested. Foliage is a prototype texture,
not a photographic scan. No wind deformation or normal maps are included.
Build/validation scripts require Blender 4.0 and NumPy; run in a fresh
background session because the build clears its current scene.
