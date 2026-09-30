# Tree variation additions

Eight missing regular-tree assets, created after auditing the existing set.
Previously completed files are retained and are not duplicated in this archive.
Timber-defect variants are counted separately from these ordinary visual variants.

## Production pruning requirement — separate delivery

This package contains **unpruned prototypes only**. Production crop-tree
replacement now requires Unpruned, LowPruned, MediumPruned and HighPruned
variants, matching LODs, and recent/healed pruning-scar treatments. These
requirements apply to the first visual replacement pass. The current species
counts do not establish pruning compatibility or production readiness.
See [the Astra pruning brief](../Astra_Crop_Tree_Pruning_Brief.md) and
[the separate pruning delivery](../Crop_Tree_Pruning/README.md), which supplies
the authored states for eligible bases. Geometry in this original package
remains unchanged; use the separate delivery for pruning variants.

| Requested group | Already present | Added here | Total |
|---|---|---|---:|
| Mature Sitka | Sitka_Mature_01 | Sitka_Mature_02, Sitka_Mature_03 | 3 |
| Old/large Sitka | No standard old/large version | Sitka_OldLarge_01, Sitka_OldLarge_02 | 2 |
| Young Sitka | Sitka_Pole_01 (8 m) | Sitka_Young_02 (10.5 m) | 2 |
| Mature sessile oak | Sessile_Oak_Mature_01 | Sessile_Oak_Mature_02 | 2 |
| Mature beech | Beech_Mature_01 | Beech_Mature_02 | 2 |
| Broadleaf saplings | Sessile_Oak_Sapling_01 | Beech_Sapling_01 | Oak + beech |

Existing oak assets are in ../Sessile_Oak_Three_Stages/; other existing assets
are in their named sibling folders in the original workspace. They are not
bundled again. No original, defect, or Unity project files were changed.

## New assets

- Sitka_Mature_02: 26 m, narrower upright crown and longer clear lower bole.
- Sitka_Mature_03: 24 m, broader uneven crown and fuller low foliage.
- Sitka_OldLarge_01: 32 m, taller old form, substantial base and high crown.
- Sitka_OldLarge_02: 30 m, stockier stem, broader asymmetric crown.
- Sitka_Young_02: 10.5 m, slender stem with retained low branches.
- Sessile_Oak_Mature_02: 22 m, spreading irregular crown and lobed leaves.
- Beech_Mature_02: 23 m, broad offset crown and smooth grey trunk.
- Beech_Sapling_01: 2 m, juvenile leader, fine branches and oval foliage.

Every asset has an editable standalone .blend and three separate FBX LODs.
LOD0 is visible initially in each source. Asset_Info.json lists triangle
counts and natural heights; Validation.json records the export checks.
Tree_Variations_Preview.blend is a same-scale lineup with preview-only offsets,
lighting and camera. Standalone sources/exports retain ground-level pivots
and zero local translations. PNG previews accompany the models.

These are procedural modelling prototypes in the existing material family,
not scans or photoreal finished assets. New branch/foliage layouts distinguish
the variants beyond simple rotation or uniform scaling. The new broadleaf
models use lighter geometry than the earlier high-poly prototypes; the older
models themselves have not been optimized or replaced. Broadleaf leaf size
is exaggerated for visual readability. No new reference sheets were generated.

## Unity URP integration

Unity was not opened, modified or tested. Nothing was committed to a repository.
Use URP/Lit materials, low smoothness, and the corresponding included albedos.
Sitka foliage: opaque surface with Alpha Clipping ON, threshold about 0.35,
Render Face Both, preserving the needle PNG alpha channel.
Oak and beech foliage: opaque, geometric leaf silhouettes; use Render Face
Both without alpha clipping. Green material colours are carried in the FBX.
Textures/ contains reusable source PNGs; retain each FBX's .fbm directory or
assign the shared textures manually. Material setup and LODGroups must be
configured in Unity. Keep all LODs at the same local transform and tune fading
and distances in the forest. Collision/wind/interaction systems are not included.
Optional simple trunk capsules should be supplied by Unity; avoid mesh colliders.

One Blender unit is one metre, +Z up; FBX uses -Z forward / Y up. Mesh rotation
and scale are applied. Lowest geometry is at ground level near the trunk centre.
Use desired visual height / authored natural height for uniform scaling.
Visual stage labels and mesh dimensions do not define biological age, DBH,
competition, growth or stage-switch thresholds. Choose explicit visual asset
references without coupling simulation logic to Blender names.

## Validation

All 24 FBXs independently reimported in Blender. Checked triangle counts,
UVs, nonzero face areas, materials/texture files, measured height, soil pivot,
source transforms and absence of unapplied modifiers. Exported objects contain
no camera/light/animation/collider or other LODs. Foliage is intentionally
single-sided geometry for double-sided materials. Unity appearance, LOD
transitions, forest density and runtime performance remain untested.
Build/validation scripts require Blender 4.0 and NumPy. Use -noaudio in a
fresh background session; scripts clear/open Blender documents. Sources
have packed textures, and previews are separate from source/export geometry.
