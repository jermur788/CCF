# Ring-barked Sitka spruce

Two visual states of the same mature 26 m Sitka spruce, each with three LODs:

- **Fresh:** green living crown, pale longitudinal exposed wood, damaged bark edges.
- **Dead:** bare branches, weathered exposed band and retained bark. This is a later standing-dead appearance, not a specified time since treatment.

The band is actual recessed stem geometry around the full circumference, with irregular margins. Its maximum recession is approximately 10 mm. Fresh and dead stems share geometry, metre scale, ground-level pivot and height. The models contain no biological state, mortality timing, gameplay code or colliders. Dimensions describe the visual asset only.

## Files

- `Sitka_RingBarked_01.blend`: editable source with packed assigned textures; six state/LOD roots. Fresh LOD0 initially visible.
- `RingBarked_Preview.blend`: separate fresh/dead lineup with preview-only offsets and lighting.
- `FBX/`: six selected-object exports, -Z forward / Y up, no lights, cameras, animation or unapplied modifiers.
- `Textures/`: reusable bark, foliage, exposed-wood and tangent-normal maps. Keep this folder beside FBX.
- `Wood_Material_Bake_Source.blend`: editable normal-bake source. Wood colour is generated; tonal relief is an artistic approximation, not scan data.
- `Previews/comparison.png` and `Previews/closeup.png`: fresh left, dead right.
- `Manifest.json`, `Geometry_Counts.md` and `Validation.json`: explicit file mapping, geometry costs and checks.

The source trees use foliage cards, not individual needle geometry. LOD0 is for close inspection; lower LODs simplify wood and cards while retaining the ring band. The bare dead crown intentionally retains the branch skeleton; later branch shedding or fallen snag states are outside this set.

## Unity URP integration notes

Unity was not opened or modified. FBX does not automatically recreate complete URP shader settings.

Use URP/Lit for bark and exposed wood, assign their base colours and normal maps, and import normal maps as Normal Map. Use low smoothness and zero metalness. The shared bark MetallicSmoothness map stores metalness in RGB and smoothness in alpha; import it with sRGB disabled. Weathered wood and dead branches use grey/brown material tints; preserve those tints when rebuilding materials.

For fresh foliage, use the BaseColorAlpha atlas with alpha clipping (starting threshold 0.38), Render Face Both and mipmaps. Configure an LODGroup using explicit state assets. Transition distances, alpha behaviour, performance and wind remain untested in Unity. No wind shader is supplied.

## Verification

Six FBXs were independently reimported to check triangle and mesh counts, UVs, texture references, finite geometry, zero degenerate faces, ground alignment and 26 m height. Source transforms are clean. Fresh/dead trunk vertices match exactly. Radial ray checks in 16 directions per LOD verify the band recedes approximately 10 mm compared with the unmodified benchmark trunk. See Validation.json.

Blender previews were visually reviewed. In-game acceptance and forest performance still require Unity testing.

## Rebuilding

Scripts use Blender 4.0.2 and NumPy. `build_ringbarked.py` reuses the neighbouring `Sitka_Mature_Benchmark_01/build_benchmark.py` geometry definitions. Both packages must be siblings to rebuild or run the comparison validator. Saved blend files and exported FBXs are independently editable. Run scripts only in fresh background Blender sessions; they replace the active document.
