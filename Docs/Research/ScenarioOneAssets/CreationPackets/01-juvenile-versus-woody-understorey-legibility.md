# Future asset packet 01: Juvenile versus woody-understorey legibility

Priority P0; estimated complexity M.

Problem: Reported player confusion; actionable regeneration must read distinctly. Mechanic: regeneration / planting.

Current substitute: current oak/beech sapling and shrub silhouettes. Preferred method: REUSE / MODIFY then custom only if needed.

Required deliverable: inspect the active current substitute and demonstrate whether bounded reuse solves the problem; create/modify only the smallest justified set. See AssetCreationStrategy.md for custom geometry/material briefs. Dependencies: biological height normalisation; pedagogy ownership for any cues.

Acceptance: Identify tree vs shrub at .15/.4/.6/1.2/2m, 3 distances and all viewports; save state unchanged.

Source context: main a8596df9c52669a36709f09e85dfe6568640af49; audit 5c389cc; task/understorey-recruitment-causality. Before starting, fetch current main, record actual base/context and read current AGENTS/AgentWorkflow/canonical decisions and live ownership. This packet is future work, not an automatic authorisation to edit current main. Use an owned dedicated/reusable worktree. One Unity Editor at a time; no Blender concurrently with heavy Unity.

Style: restrained current Irish woodland prototype, plausible morphology/scale and readable silhouettes. Reuse current assets first; source/license register required for any original/generated/external content. No external downloads/packages without task authorisation. Preserve GUIDs, species/state IDs and Reference v1. Art does not define ecology.

Import/technical contract: Unity 6000.6.0f1 / current URP, 1unit=1m, root-collar/ground pivots documented, shared materials and 512–1K textures unless measured reason otherwise, mips/compression/alpha clipping checked; no leaf MeshColliders. Polycount report LOD0 separately from all-LOD sum. Runtime-generated geometry/materials must be measured, not zeroed by static inventory assumptions.

Verification: actual active ForestTest player/camera at 1280×720,1600×900,1920×1080, warmed shaders and gizmos off; before/after fixed scene; close/mid/distant view; named LOD/scale/material/collision/recognition checks; profiler comparison where cost changes. No save/Reference changes. Report every unverified requirement. Include source files, export, manifest, screenshots, provenance and exact import/usage test steps.

Ownership: asset folders/catalogue bindings explicitly assigned at task start; confirm current UI/pedagogy owner for UI/highlight/labels; ScenarioOneManager minimal integration only with confirmed ownership. Do not change biology or ecology categories to fit visuals. Stop if save schema, unrelated mechanic, uncertain redistribution rights, incompatible style/scale, Reference, or overlapping writer becomes necessary. Return concrete proposal and continue independent inspection.

## Specific production brief

Active starting assets: Assets/ForestPrototype/Prefabs/SessileOakSapling01.prefab and Assets/ForestPrototype/Prefabs/Forestry/SectionFive/Beech_Sapling_01.prefab; compare alongside Bramble_Clump_01 and current bilberry catalogues. Trace ForestTest speciesVisualSets and SectionFiveVisualCatalog before changing bindings. Required paired recognition scenes: natural and exact planting at .15/.4/.6/1.2/2m with actual prompts, no labels first then labelled interaction; report recognition failures separately from rendering scale.

If replacement required: three single-leader silhouettes/species, canonical authored height1m (or explicit measured height metadata), ground root-collar pivot. Seedling tris LOD0≤1500/LOD1≤500/LOD2≤120; sapling≤3000/900/200; two shared materials max,512–1K leaf/stem atlas. Oak sessile reference leaves and beech alternate leaves reviewed against source photos. FBX + Blender source + mesh/import/provenance manifest and view captures. No damage/seasonal pack. Preserve existing representative/individual simulation units.

Verified starting defect on a8596df: natural beech target0.6m rendered1.2m; exact planted beech target0.6m rendered0.6m. Source: ForestEcologyController.SyncSeedlingVisuals direct-height multiplier versus ScenarioHabitatVisuals.PlaceJuveniles bounds normalisation. Required first action after ownership: bounded natural representative height-normalisation correction, preserve species/display/save IDs and current oak/Sitka dimensions, inspect authored pivots, verify at0.15/.4/.6/1.2/2m. This is an ecology presentation code ownership point, not a reason to alter model height, habitat cover, tutorial thresholds or source mesh geometry. Do not touch ScenarioOneManager. Only author replacement silhouettes if measured recognition still fails afterwards.
