# Future asset packet 03: Competitor vegetation category legibility

Priority P1; estimated complexity M.

Problem: Bramble/bracken/competitive graminoids must match accepted biology. Mechanic: future competition.

Current substitute: current bramble/bracken/grass/rush/fern assets. Preferred method: REUSE / MODIFY.

Required deliverable: inspect the active current substitute and demonstrate whether bounded reuse solves the problem; create/modify only the smallest justified set. See AssetCreationStrategy.md for custom geometry/material briefs. Dependencies: manager competition-input decision.

Acceptance: Distinct competitor and ordinary fern/herb classes; count variation does not alter simulation.

Source context: main a8596df9c52669a36709f09e85dfe6568640af49; audit 5c389cc; task/understorey-recruitment-causality. Before starting, fetch current main, record actual base/context and read current AGENTS/AgentWorkflow/canonical decisions and live ownership. This packet is future work, not an automatic authorisation to edit current main. Use an owned dedicated/reusable worktree. One Unity Editor at a time; no Blender concurrently with heavy Unity.

Style: restrained current Irish woodland prototype, plausible morphology/scale and readable silhouettes. Reuse current assets first; source/license register required for any original/generated/external content. No external downloads/packages without task authorisation. Preserve GUIDs, species/state IDs and Reference v1. Art does not define ecology.

Import/technical contract: Unity 6000.6.0f1 / current URP, 1unit=1m, root-collar/ground pivots documented, shared materials and 512–1K textures unless measured reason otherwise, mips/compression/alpha clipping checked; no leaf MeshColliders. Polycount report LOD0 separately from all-LOD sum. Runtime-generated geometry/materials must be measured, not zeroed by static inventory assumptions.

Verification: actual active ForestTest player/camera at 1280×720,1600×900,1920×1080, warmed shaders and gizmos off; before/after fixed scene; close/mid/distant view; named LOD/scale/material/collision/recognition checks; profiler comparison where cost changes. No save/Reference changes. Report every unverified requirement. Include source files, export, manifest, screenshots, provenance and exact import/usage test steps.

Ownership: asset folders/catalogue bindings explicitly assigned at task start; confirm current UI/pedagogy owner for UI/highlight/labels; ScenarioOneManager minimal integration only with confirmed ownership. Do not change biology or ecology categories to fit visuals. Stop if save schema, unrelated mechanic, uncertain redistribution rights, incompatible style/scale, Reference, or overlapping writer becomes necessary. Return concrete proposal and continue independent inspection.

## Specific production brief

Starting assets: Assets/ForestPrototype/Prefabs/Forestry/SectionFive/{Bramble_Clump_01,Bracken_Clump_01,ShadeFern_Clump_01,Woodland_Grass_Tuft_01,Woodland_Rush_Clump_01}.prefab. Required output is a competitor-versus-noncompetitor recognition scene after ecological category acceptance, plus low/medium/high procedural placement counts using existing saved biology. Do not name generic shrubs/bramble or generic ferns/bracken as biological fact. Use2–3 silhouettes/group via current mesh reuse, shared512–1K atlas, ≤2 material types, bound transparent layering; separate patch budgets measured before/after, not decorative density→mortality code. Stop biology-dependent implementation until competitor input decision.
