# Future asset packet 09: Ground flora material/density cost pass

Priority P2; estimated complexity M.

Problem: Many layered moss/foliage instances; heavy-cover clutter. Mechanic: habitat presentation.

Current substitute: SectionFive moss/grass/rush/herb. Preferred method: REUSE / MODIFY CURRENT ASSET.

Required deliverable: inspect the active current substitute and demonstrate whether bounded reuse solves the problem; create/modify only the smallest justified set. See AssetCreationStrategy.md for custom geometry/material briefs. Dependencies: render profiler; ecology categories fixed.

Acceptance: Preserve recognisability at lower cost; seeded density levels; no ecology values changed.

Source context: main a8596df9c52669a36709f09e85dfe6568640af49; audit 5c389cc; task/understorey-recruitment-causality. Before starting, fetch current main, record actual base/context and read current AGENTS/AgentWorkflow/canonical decisions and live ownership. This packet is future work, not an automatic authorisation to edit current main. Use an owned dedicated/reusable worktree. One Unity Editor at a time; no Blender concurrently with heavy Unity.

Style: restrained current Irish woodland prototype, plausible morphology/scale and readable silhouettes. Reuse current assets first; source/license register required for any original/generated/external content. No external downloads/packages without task authorisation. Preserve GUIDs, species/state IDs and Reference v1. Art does not define ecology.

Import/technical contract: Unity 6000.6.0f1 / current URP, 1unit=1m, root-collar/ground pivots documented, shared materials and 512–1K textures unless measured reason otherwise, mips/compression/alpha clipping checked; no leaf MeshColliders. Polycount report LOD0 separately from all-LOD sum. Runtime-generated geometry/materials must be measured, not zeroed by static inventory assumptions.

Verification: actual active ForestTest player/camera at 1280×720,1600×900,1920×1080, warmed shaders and gizmos off; before/after fixed scene; close/mid/distant view; named LOD/scale/material/collision/recognition checks; profiler comparison where cost changes. No save/Reference changes. Report every unverified requirement. Include source files, export, manifest, screenshots, provenance and exact import/usage test steps.

Ownership: asset folders/catalogue bindings explicitly assigned at task start; confirm current UI/pedagogy owner for UI/highlight/labels; ScenarioOneManager minimal integration only with confirmed ownership. Do not change biology or ecology categories to fit visuals. Stop if save schema, unrelated mechanic, uncertain redistribution rights, incompatible style/scale, Reference, or overlapping writer becomes necessary. Return concrete proposal and continue independent inspection.

## Specific production brief

Starting assets: SectionFive/{Ground_Moss_ShootMat_03,Ground_Moss_Patch_01,Woodland_Grass_Tuft_01,Woodland_Rush_Clump_01,Woodland_Forb_Rosette_01}.prefab and ScenarioHabitatVisuals actual catalogue. Baseline warm near/mid/far heavy-cover draw/triangle/material/shadow/overdraw measurements. Reuse/simplify current variants and shared512–1Katlas; density placement seeded and no biological field changes. Low/medium/high views must remain categorically recognisable. Do not automatically remove moss, change cover targets or turn generic grass into competition state.
