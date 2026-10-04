# Coding-agent handoff — plantation Sitka 02

## Start here

Read `README.md`, `Coverage.md` and the exact mappings in `Manifest.json`. The delivered ordinary bases are `SS_Plantation_Pole_02` (8 m) and `SS_Plantation_FirstThinning_02` (12 m). They are new IDs; do not re-enable retired Mature 01/02/03, OldLarge 01/02 or original Pole 01 families in the active stand. The 26 m benchmark remains a later-tree reference.

Unity code, scenes, prefabs, model selection, eligibility and movement remain **coding-agent work**. This art pass has not opened or changed Unity or CCF code. No Unity behaviour is claimed as tested.

## Rendering and pruning

1. Import with the supplied textures and explicit material slots. Set matte bark and double-sided alpha-clipped needle sprigs; avoid render-mesh colliders.
2. A complete snapshot is already a whole tree. For scale-aware assembly use **one matching core + retained branch-system modules + scar pieces**, all at identity local transform under the same tree ground pivot. Never add another complete tree/trunk to obtain low branches.
3. `Branch_Modules.json` supplies exact files, root/mesh objects and slots. Scope stable IDs by tree instance as well as base/branch/segment; asset branch IDs repeat legitimately across instances.
4. Authored snapshot lift targets are 2.5/5/6.5 m and their actual clearances are recorded. After scaling to the gameplay tree height, do not assume a snapshot label means that same world clearance. Use `ScaleAware_Pruning_Examples.json`: transform the branch bounds using the actual visual transform and select/remove whole systems against the world target. The wood and attached foliage belong to one removable system. All upper systems are available for smaller scaled trees.
5. Place the selected Recent/Healed scar at each removed site using `Scar_Placement.json`: recorded position/basis, piece scale and per-LOD reusable piece. Local scar +Z points outward. Stem and remaining crown/module geometry stay unchanged.
6. Keep current pruning eligibility. Supplied juvenile/pole art states do not automatically create juvenile work targets. Crop Tree designation/blue marks alone do not alter branch visibility or contact.

## Contact geometry

`Branch_Contact.json` provides tree-local **metre** coordinates, attachment height/axis, stable IDs, woody capsule endpoints/radii, optional flexible foliage capsules and exact retained/removed state membership. The authoritative source frame is Blender +Z up. An explicit author-to-FBX export-axis matrix is included; confirm the actual Unity importer/root basis and handedness, then transform metadata once with the same scale/rotation/translation as the corresponding render tree. Do not reinterpret already-converted FBX units as raw centimetres.

Contact data is independent of render LOD. Use it as a simplified spatial/path query representation; do not infer traversability from the changed LOD2 card density, every twig triangle, stem proximity or a whole-canopy cylinder. Optional foliage envelopes conservatively include alpha-card support extents; they are light-contact aids and must not alone create a hard circular wall.

Preserve the user's requirement:

> blocks movement only when saplings are close together and have not been pruned, otherwise slows movement

Engine interpretation from the brief: slow only actual visible low-branch contact; block only genuinely closed dense interlocking young-stock passages at body height/width. Keep clear ground/overhead branches walkable and routes around lone trees. Sweep the attempted movement path to prevent tunnelling, while allowing exits from occupied patches. Calibrate speed and dense-passage thresholds in game [D]. Keep movement queries separate from trunk inspection/marking/work-order aim raycasts and ground planting.

Rebuild/rebind after **completed pruning, felling, growth, loading, visual-base changes and preview-reference changes**. Marking/unmarking alone does not clear contact. Remove the affected systems/contact after completed pruning and standing-tree contact after felling. Batch/instance retained render meshes or cache assemblies as appropriate; the modular export layout is an authoring contract, not a requirement to create one physics collider/draw call per twig.

## Acceptance still owned by integration

- Identify unpruned/pruned trees at 3–10 m in game, with no reliance on management marks.
- Verify dense/ordinary contact, open routes, fast movement and exits; pruning/felling removal; LOD-independent traversability.
- Keep trunk interaction proxies working; test marking, actual work completion, save/load and preview return.
- Profile the complete 336-tree stand, not just the nine-tree offline review subset.
- Preserve canonical lifecycle hash **`7E39B70A14959FAD`**, frozen Year-100 reference hash **`7AD177B3CC2F73C7`**, simulation values and existing eligibility. These hashes are the brief's contract, not hashes re-computed by this art pass.

**Bent/cavity low-branch coverage is absent.** Those variants need their own compatible authored treatment before meeting this requirement. Do not silently substitute an older defect prototype or attach a second straight tree.
