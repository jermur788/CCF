# Coarse-branched Sitka — three stages

Completes the previously missing coarse-branched class in the five-type Sitka defect family.

| Asset stage | Authored height | Target DBH | Largest primary branch diameter |
|---|---:|---:|---:|
| Young | 11.5 m | 18 cm | 8.4 cm |
| Mature | 25 m | 46 cm | 23 cm |
| Post-mature | 31 m | 69 cm | 36 cm |

The lower five branch tiers retain unusually heavy persistent branches with foliage concentrated further out, exposing the branch structure. Higher branches return to narrower Sitka architecture. The three stages have distinct branch arrangements, proportions and crown density. Shared benchmark bark and needle materials keep them in the same population.

Nine FBXs, three LODs per stage, are accompanied by editable SS_CoarseBranch_Set.blend and a separate CoarseBranch_Preview.blend. The preview lineup is young, mature, older from left to right. Defining heavy branch axes are retained through LOD1 and LOD2. Pruning-state assets are deliberately not supplied for this habitat/defect reference set; these are not replacements for the separate crop-tree pruning family.

Branch diameters and stage labels describe the visual asset, not authoritative biological state. Recognisability was reviewed in Blender; the specified 10–20 m player-distance acceptance still requires in-game testing.

## Source and export

One Blender unit is one metre. Source mesh and parent transforms are identity, +Z up, with a ground-level pivot. FBX uses -Z forward / Y up, selected meshes/empties only, no cameras, lights, animation, modifiers or colliders. Three LODs per asset; choose explicit asset references rather than inferring simulation state from mesh names or dimensions.

The main `.blend` packs assigned images. Keep FBX/ and Textures/ together when moving exports. Preview scenes contain layout offsets and lighting; those are not exported. Scripts are included for reproducibility and require Blender 4.0.2 plus NumPy. Build scripts use the neighbouring Sitka_Mature_Benchmark_01 geometry helpers; saved sources and FBXs work independently. Run scripts only in fresh background sessions because they replace the active document.

## Unity URP setup and limits

Unity was not opened or modified. Use URP/Lit, zero metallic and low smoothness. Assign provided colour maps and import normal maps as Normal Map. Needle foliage uses BaseColorAlpha, alpha clipping (start around 0.38), Render Face Both and mipmaps. Other surfaces are opaque. Retain both-sided rendering for thin foliage and moss surfaces. Configure LODGroups manually and assess transitions and forest performance in-game. No wind, collider, gameplay or simulation implementation is included.

These are procedural game-art prototypes, with some generated colour textures; they are not scanned assets. Blender previews were inspected. Validation.json records independent FBX reimport checks for triangle counts, materials, texture paths, UVs, finite geometry, nonzero face areas, dimensions and ground alignment, plus clean source transforms and packed assigned textures. Unity appearance, LOD transitions, collision placement and performance remain untested.
