# Astra asset brief — crop-tree pruning states

## Priority and scope

Pruning compatibility is a required part of the **first production visual replacement pass**, not a later polish task. Every production model used for a prunable crop tree must have authored pruning-state variants. Use complete asset swaps for the first version; do not depend on runtime deletion of individual branches.

This brief extends the existing species, age-stage and visual-variation requirements. It does not mark the current procedural prototypes as production-ready. Work remains standalone Blender sources and Unity-compatible FBX exports; do not open or modify Unity as part of asset authoring.

## Required states

For each eligible base model, supply:

| State | Visual requirement |
|---|---|
| Unpruned | Original branch architecture and foliage distribution. |
| LowPruned | Deliberately cleared lower bole, with visible cut sites. |
| MediumPruned | Higher cleared bole, retaining the same individual tree and upper crown. |
| HighPruned | Highest approved pruning extent for this tree's stage, preserving an appropriate living crown. |

Sitka is the first priority. Author low, medium and high as explicit, progressively higher branch-clearance states. Record the actual pruning extent in metres at authored scale for each model; these labels must not imply identical absolute heights across age stages. Do not strip a small tree simply to match a mature tree's clearance height. These are visual specifications, not forestry treatment prescriptions.

Oak and beech require species-specific selective lower-branch removal and crown raising. Preserve their spreading scaffold architecture; do not reuse a conifer-shaped trim or leave a uniformly clipped foliage boundary. Retained major limbs and crown asymmetry should identify the same individual in every state.

## Cut sites and long-term management history

- Removed branches must leave believable short stubs, branch-collar forms or scar geometry, rather than a newly smooth pole or long broken snags.
- Place cut sites at the actual removed branch attachments. Remove the associated branch and foliage together; no floating leaves or buried leftover branches.
- Recent cuts should read as deliberately pruned, with restrained exposed-wood detail rather than torn storm damage.
- Provide a healed-scar treatment for each pruned state so the tree can retain readable management history years later. It may share geometry and use a material/texture variant, with geometry changes where needed for the scar profile.
- Healed scars should be less prominent than recent cuts, while the cleared bole and residual scar pattern still distinguish management from an untouched tree at walking distance.
- Keep pruning extent and scar age separate. A high-pruned state is not automatically older than a low-pruned state.
- Do not create a runtime wound-healing system or encode biological state in the mesh. Deliver explicit visual choices for later simulation-driven selection.

## Asset continuity and LODs

Derive states from the same underlying tree. Keep the trunk, retained branches, crown position, natural height, ground-level pivot, orientation and applied transforms aligned. Switching pruning state must not move the tree, change its overall scale or replace its upper crown with a different tree.

Deliver LOD0, LOD1 and LOD2 for each state wherever the production family uses LODs. LOD1 must preserve the cleared-bole silhouette and readable management character. LOD2 may simplify individual scars but must not restore removed lower foliage. Match state and scar treatment across all LODs.

Reuse bark, foliage and exposed/healed-wood material families. Keep UVs and normals complete and maintain the existing double-sided foliage convention. Sources must be editable .blend files; exports must be selected-object FBX with metre scale, ground-level stem-centre pivot, Unity-compatible axes, no cameras, lights, animation or colliders. Bake or apply export dependencies.

Clean branch organization in the Blender source is useful for future modular authoring, but exported standalone variants are the required first-version delivery. Runtime procedural pruning is out of scope.

## Naming and manifest

Example export names:

```text
Sitka_Mature_02_Unpruned_LOD0.fbx
Sitka_Mature_02_LowPruned_Recent_LOD0.fbx
Sitka_Mature_02_LowPruned_Healed_LOD0.fbx
Sitka_Mature_02_MediumPruned_Recent_LOD0.fbx
Sitka_Mature_02_HighPruned_Recent_LOD0.fbx
```

Repeat applicable states/treatments across LODs. If healed appearance uses shared geometry, document the explicit material/texture assignment instead of duplicating identical FBX files.

Include a manifest mapping base asset, species, visual stage, pruning state, scar treatment, authored height, pruning extent, LOD, file, triangle count and material dependencies. Unity must select explicit asset references; simulation rules must not depend on parsing Blender names or inferring pruning history from mesh bounds.

## Coverage of the current library

Audit all intended crop-tree bases, including originals as well as new variants. Initial candidates are:

- Sitka_Mature_01, Sitka_Mature_02, Sitka_Mature_03.
- Sitka_Pole_01 and Sitka_Young_02.
- Sitka_OldLarge_01 and Sitka_OldLarge_02, including retained management history where used as older crop trees.
- Sessile_Oak_Mature_01 and Sessile_Oak_Mature_02.
- Beech_Mature_01 and Beech_Mature_02.
- Sessile_Oak_Young_01 if used as a prunable crop-tree visual.

Seedlings and broadleaf saplings are not automatically candidates for all four states. Record eligibility explicitly in the asset manifest. Habitat/defect trees are not automatically crop trees; if any is made prunable, it needs equivalent coverage too. Never silently substitute an unrelated unpruned model for a missing state.

The original tree-variation package supplies only unpruned prototypes. Its species/variant counts do **not** satisfy this pruning requirement on their own. The separate `Crop_Tree_Pruning` delivery now supplies all four states and recent/healed treatments for the 12 eligible bases listed above. It extends the existing prototype geometry; final production art quality and Unity acceptance remain separate checks. See that delivery's coverage and validation reports.

## Acceptance before production replacement

1. Show the same individual side by side in all four pruning states, at identical scale and orientation, with recent/healed close-ups.
2. Compare from several ground-level angles: deliberate clearance must be readable, with no floating foliage, abrupt holes in retained crowns, or long accidental-looking stubs.
3. Show an older healed example that still communicates timber management without relying on labels or inspection UI.
4. Check LOD0/1/2 consistency: pruning extent must not reverse at distance, and state changes must not jump the trunk or retained crown.
5. Reimport exports and validate pivots, measured height, transforms, UVs, normals, textures and triangle counts.
6. Document missing coverage and performance limitations. Treat a prunable base as incomplete until all required states and scar treatments are present.

Blender inspection can verify the asset delivery. Later Unity integration must separately verify state switching, LOD transitions, material appearance and runtime cost; do not claim those tests occurred during offline authoring.
