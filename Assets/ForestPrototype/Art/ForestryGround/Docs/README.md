# Forestry ground additions

Six complementary ground assets:

- SS_WindthrowBase_Fresh_01: uprooted soil/root plate attached to a broken basal stem, with bark and exposed wood.
- SS_WindthrowBase_Weathered_01: matching weathered wood form with restrained moss patches.
- SS_Brash_Green_01 and 02: two different arrangements of cut spruce boughs with needles.
- SS_Brash_Dry_01 and 02: bare dry woody counterparts.

These add 18 FBXs across three LODs. The main Forestry_Ground_Additions.blend retains all six assets, with separate roots and brash preview scenes. The windthrow bases are short basal fragments, not full fallen trees; they complement existing fallen-log assets. Their pivot is at the footprint centre at ground level. Brash is a static visual pile, not a physics simulation of individually packed branches; natural overlapping branch geometry is intentional.

Root plates include irregular closed earth geometry and actual branching roots. The generated soil image adds surface detail. Weathered wood and moss reuse the existing woodland material family. Fresh and weathered describe appearance only and do not encode elapsed years or decay rules.

Root preview: fresh in front, weathered behind. Brash preview: green pair on the left, dry pair on the right. Root plates need local terrain placement review; they do not cut a hole in terrain or include the soil pit. Simple optional collision should be supplied separately in Unity, not a full mesh collider.

## Source and export

One Blender unit is one metre. Source mesh and parent transforms are identity, +Z up, with a ground-level pivot. FBX uses -Z forward / Y up, selected meshes/empties only, no cameras, lights, animation, modifiers or colliders. Three LODs per asset; choose explicit asset references rather than inferring simulation state from mesh names or dimensions.

The main `.blend` packs assigned images. Keep FBX/ and Textures/ together when moving exports. Preview scenes contain layout offsets and lighting; those are not exported. Scripts are included for reproducibility and require Blender 4.0.2 plus NumPy. Build scripts use the neighbouring Sitka_Mature_Benchmark_01 geometry helpers; saved sources and FBXs work independently. Run scripts only in fresh background sessions because they replace the active document.

## Unity URP setup and limits

Unity was not opened or modified. Use URP/Lit, zero metallic and low smoothness. Assign provided colour maps and import normal maps as Normal Map. Needle foliage uses BaseColorAlpha, alpha clipping (start around 0.38), Render Face Both and mipmaps. Other surfaces are opaque. Retain both-sided rendering for thin foliage and moss surfaces. Configure LODGroups manually and assess transitions and forest performance in-game. No wind, collider, gameplay or simulation implementation is included.

These are procedural game-art prototypes, with some generated colour textures; they are not scanned assets. Blender previews were inspected. Validation.json records independent FBX reimport checks for triangle counts, materials, texture paths, UVs, finite geometry, nonzero face areas, dimensions and ground alignment, plus clean source transforms and packed assigned textures. Unity appearance, LOD transitions, collision placement and performance remain untested.
