# Plantation Sitka — lower-branch/pruning revision 02

Requested by `SitkaPlantationLowerBranchesBrief.md` (1 October 2026). Two **independently authored ordinary plantation bases**, preserving every earlier delivery:

| Base ID | Authored height | Approximate authored DBH target | Architecture |
|---|---:|---:|---|
| `SS_Plantation_Pole_02` | 8 m | 12 cm | Young/pole form, substantial mostly-live lower whorls |
| `SS_Plantation_FirstThinning_02` | 12 m | 16 cm | First-thinning proportions, long retained dead lower systems mixed with some live foliage |

Lower attachments span approximately 0.4–2.2 m and ordinary lateral reaches approximately 0.8–1.5 m. Directions/gaps vary. These are not coarse-branch defect specimens or globally reduced versions of the 26 m benchmark. Full branches reach ankle/knee, torso and shoulder/head regions; pruning opens the lower bole and surrounding space without any management mark.

## Coverage and files

**474 FBXs are export components, not 474 trees:**

- **42 complete tree snapshots:** two bases × seven treatments × three LODs.
- **420 branch-system modules:** 60 pole and 80 first-thinning systems, each at LOD0/1/2. Wood and each system's attached foliage are separable from the trunk. All branch systems are supplied, including upper ones, so shorter scaled trees can still use world-height pruning targets.
- **6 tree-core exports:** trunk/buttresses and leader foliage, three LODs per base.
- **6 reusable scar pieces:** Recent/Healed, three LODs each, with explicit per-branch placement metadata.

Treatments: `Unpruned`; `LowPruned_Recent/Healed`; `MediumPruned_Recent/Healed`; `HighPruned_Recent/Healed`. Requested authored clearances are **2.5 / 5 / 6.5 m**. Achieved branch clearances are approximately **2.55 / 5.05 / 6.55 m**; exact per-base/state values are in the manifest and `Coverage.md`. Scars are excluded from branch-clearance measurements.

- `Sitka_Plantation_02.blend`: editable asset-only source, packed assigned textures, clean transforms; first-thinning Unpruned LOD0 initially visible. Other snapshot/module/core/scar collections are hidden for editing convenience.
- `FBX/Snapshots/`, `FBX/Modules/`, `FBX/Cores/`, `FBX/Scars/`: separately selected exports; no cameras/lights, animation, modifiers or colliders.
- `Textures/`: the refined benchmark v2 bark/needle material family, unchanged byte-for-byte, plus cut wood. Needles retain RGBA alpha.
- `Manifest.json`: exact base/state/LOD/root/object/material-slot mappings and all costs/bounds.
- `Branch_Modules.json`, `Branch_Contact.json`, `Scar_Placement.json`: explicit assembly, stable IDs, axes/envelopes and scar placement.
- `ScaleAware_Pruning_Examples.json`: 17 geometry-only examples after uniform scaling.
- `Previews/`, `Review_Sheets/`, `Preview_Scenes/`, `Eye_Level_Validation.json`: 60 rendered views and compact separate scenes. Main comparisons use a **1.65 m camera eye** and measured **1.8 m human reference**; scar macros include an eye-level context inset. Module panels are technical component views.
- `Validation.json`, `Geometry_Counts.md`, `Geometry_Counts.csv`, `Visual_Review.json`: technical/continuity results, costs and reviewed scope.
- `Integration_Handoff.md`: coding-agent instructions; Unity integration remains outside this art package.

## Scale-aware use

All tree and branch roots share the authored **ground origin**, metre units, Blender +Z up and FBX -Z forward/Y up. Branch geometry lies above ground intentionally: **do not recenter or ground-shift individual modules**. No trunk geometry is baked into a branch module.

Use **either** a complete snapshot **or** one matching core plus retained branch modules and scars. Adding modules over an already-complete unpruned tree would duplicate branches. The exact lookup is provided, not inferred from names.

At native authored scale, snapshots show the requested lifts. At another scale, uniformly scaling a snapshot also scales its clearance. Instead, choose whole modules against the world pruning height using recorded all-LOD bounds and attachment data, then place scars at removed sites. The same stem, remaining systems, pivot and leader are preserved. This does not make a pruning lift eligible: gameplay retains work eligibility and crop-tree designation alone must not remove branches.

Contact records are **LOD-invariant**. They describe woody capsule segments and optional flexible-foliage envelopes, with stable base/branch/segment IDs, local endpoints/radii, attachment heights/axes and state removal membership. They supply geometry, not speed reduction, blocking thresholds, growth or habitat rules. Optional foliage envelopes include card-support extents and are conservative light-contact aids, not hard whole-canopy walls.

## Materials

Use matte URP/Lit bark: sRGB `Sitka_Bark_BaseColor_v2.png`, tangent `Sitka_Bark_Normal_v2.png` (approximately 0.7 strength), linear `Sitka_Bark_MetallicSmoothness_v2.png` with smoothness from alpha. `Sitka_Bark_Roughness_v2.png` is the separate Blender/non-colour roughness map. Metallic is zero.

Needles: opaque surface with **Alpha Clipping 0.38**, **Render Face: Both**, `Sitka_NeedleSprigs_BaseColorAlpha.png` as sRGB Base Map with image alpha, low smoothness. Recent scars use `Sitka_CutWood_BaseColor.png`; healed centres use the recorded matte colour. Material slots are mapped per mesh in each export row. FBX conversion does not establish URP shader configuration; assign it explicitly during integration.

## Review and verification

All **474 FBXs** were independently reimported and checked for counts/bounds, finite/nondegenerate geometry, UV/material/texture references and role-appropriate pivots. Source transforms and packed images passed. All **42 snapshots** preserve the stem and match the exact union of their retained modules; recent/healed treatments alter scars only. Woody axes align with exported wood, optional foliage envelopes contain the three-LOD foliage union, and metadata is unchanged across LOD switches.

Reviewed: all pruning lifts, Recent/Healed scar pairs, identical nine-tree patches at 1.9 m spacing before/after Low pruning, wider 4.2 m patches, Unpruned LOD0/1/2 at nominal 3/6/10 m and LowPruned LODs at 3/10 m, contact overlays and isolated modules. The low feature survives LOD reductions. These offline checks do **not** establish Unity movement, import, in-game readability or 336-tree stand performance acceptance.

**Variant coverage:** ordinary pole and first-thinning only. New low-branch bent/cavity treatments are **not supplied**. The existing refined defects are preserved but do not meet this low-branch requirement; do not silently fall back to older prototypes or overlay an extra straight trunk/tree. See `Coverage.md`.

## Rebuild

Blender 4.0.2 with NumPy; system Python with Pillow for review sheets. The workspace NumPy dependency is under `Forest_Asset_Tools/python-libs`; an independent authoring copy can install it under `Tools/python-libs`. The builder uses only the mesh/material utility prefix of sibling `Sitka_Mature_Benchmark_01/build_benchmark.py`. Keep that sibling for rebuilding or texture-identity checks. Saved Blender/FBX assets and packed review scenes are independent of Python build-time dependencies.

```bash
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python build_plantation.py
blender -t 6 -noaudio --background --factory-startup --python-exit-code 1 --python validate_plantation.py
python3 render_review_batch.py
python3 collect_preview_review.py
```

Render **one preview at a time**. Rebuild in a new editable workspace revision rather than replacing prior deliveries. Source brief is preserved under `Source_Reference/`.
