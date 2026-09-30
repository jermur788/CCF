# Crop-tree pruning variants

This set adds pruning-compatible **prototype art** to the 12 existing eligible crop-tree bases. Existing source files remain unchanged. Unity was not opened or modified.

Delivery location: `/media/jer/ZX20/Unity Assets/Crop_Tree_Pruning`.
Start with `Coverage.md` for the model list and `Asset_Manifest.json` for explicit state/LOD mappings.

## Contents

Each base has seven visual treatments:

- Unpruned.
- LowPruned — Recent and Healed.
- MediumPruned — Recent and Healed.
- HighPruned — Recent and Healed.

Each treatment includes three FBX LODs: **21 FBX files per base, 252 exports total**. That is 72 new pruned treatments plus 12 unpruned references. Editable `.blend` sources retain all states at the same pivot. Shared geometry is reused between recent/healed treatments where possible. The source initially shows unpruned LOD0; reveal the required collection/objects to edit another state.

Covered bases: seven Sitka (Mature 01/02/03, OldLarge 01/02, Pole 01, Young 02), three sessile oak (Young 01, Mature 01/02), and two mature beech (01/02).

Seedlings, saplings and habitat/defect trees are not designated prunable crop-tree bases in this delivery. They are deliberately excluded. This is the complete pruning scope from the current brief, not a claim that every possible forestry asset has been created.

## Model construction

States are derived from the existing geometry, not unrelated replacement trees. Entire lower branch groups and their associated foliage are removed in the offline authoring pass. Each cut receives a short collar/stub with a separate end surface. Recent cuts use an exposed-wood map; healed cuts have shorter profiles, a bark-covered collar and a smaller weathered centre. The retained upper crown is shared between recent/healed treatments and preserved between pruning states.

Low/medium/high are relative visual stages for each source. Manifests record actual clearance limits in authored metres. Older bases already have long clear boles, so their visual pruning limits are higher; these are not recommended forestry treatment heights. Original geometry does not contain historical scars below its first branch attachments. No earlier branch history has been invented there.

Broadleaf pruning follows the existing spreading branch groups rather than trimming a conifer-shaped volume. Branch association is reconstructed from the existing disconnected geometry. Review close overlaps in dense original crowns before final production use.

LOD reductions retain the pruned state, thin foliage and simplify woody geometry. Small scar geometry is retained at all LODs. All representations share metre scale, origin, orientation and authored height within the validation tolerances. A small common ground correction is recorded where an original prototype had geometry below soil level. No animation, cameras, lights, colliders, runtime branch deletion or biological state are exported.

## Files and integration

- `<base>/<base>_Pruning.blend`: editable tree states, at the soil pivot.
- `<base>/<base>_<state>_<treatment>_LOD<n>.fbx`: separate selected-object exports. Unpruned has no treatment suffix.
- `Textures/`: shared portable bark, foliage, cut-face and healed-scar PNGs.
- `<base>/Manifest.json`: state mapping, measured natural height, pruning extents, triangle counts and cut counts.
- `<base>/Validation.json`: independent reimport and source checks.
- `<base>/<base>_Pruning_Lineup.png`: unpruned, low, medium, high, left to right.
- `<base>/<base>_Scar_Comparison.png`: recent (left), healed (right).
- `<base>/<base>_Preview.blend`: separate offset lineup for inspection, not an export source.

Keep `Textures/` beside the base directories. FBX uses -Z forward / Y up. Use URP materials on later integration: Sitka foliage needs alpha clipping (about 0.35) and Render Face Both; broadleaf foliage needs double-sided rendering. Set bark/wood roughness high (low smoothness). Blender material settings do not automatically establish URP shaders. Configure LODGroups and explicit pruning-state references in Unity later; no importer scripts are included.

Do not derive simulation age or pruning history from object names or mesh bounds. Choose explicit visual states using the game's authoritative data. Scars are authored visual treatments, not a wound-healing simulation.

## Limits and verification

These extend the current procedural prototypes; they are not a photoreal production-art replacement. Original high-poly oak/beech models remain expensive, particularly LOD0. Runtime performance, LOD transitions and scene appearance still require Unity testing. No Unity testing is claimed.

Validation reimports every FBX, checks object/mesh counts, material and image references, UVs, finite coordinates, triangle counts, ground position and measured height, and checks source transforms and state counts. Duplicate leaf faces were removed where required, affected meshes were triangulated explicitly, and simplified feet were corrected at soil level. Final validation found zero degenerate faces. Ground tolerance is 5 mm; height tolerance is 2 cm. Visual previews inspect pruning silhouettes and scar treatment; they do not substitute for in-game acceptance.

Authoring scripts are included for traceability. They require Blender 4.0.2, NumPy and the original library at its recorded paths; editing the packed `.blend` files does not require those scripts or dependencies. Use a fresh background Blender session with `-noaudio` for scripts, which open/replace Blender documents.
