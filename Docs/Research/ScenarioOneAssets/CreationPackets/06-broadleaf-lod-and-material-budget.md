# Future asset packet 06: Broadleaf LOD and material budget

Priority P1; estimated complexity L.

Problem: Mature beech no LOD and oak high-poly imported meshes. Mechanic: growth / pruning.

Current substitute: current mature/pruning sets. Preferred method: MODIFY CURRENT ASSET.

Required deliverable: inspect the active current substitute and demonstrate whether bounded reuse solves the problem; create/modify only the smallest justified set. See AssetCreationStrategy.md for custom geometry/material briefs. Dependencies: profiling active renderer paths; preserve GUIDs.

Acceptance: Measured frame-cost reduction; acceptable silhouettes at transitions; pruning state preserved.

Source context: main a8596df9c52669a36709f09e85dfe6568640af49; audit 5c389cc; task/understorey-recruitment-causality. Before starting, fetch current main, record actual base/context and read current AGENTS/AgentWorkflow/canonical decisions and live ownership. This packet is future work, not an automatic authorisation to edit current main. Use an owned dedicated/reusable worktree. One Unity Editor at a time; no Blender concurrently with heavy Unity.

Style: restrained current Irish woodland prototype, plausible morphology/scale and readable silhouettes. Reuse current assets first; source/license register required for any original/generated/external content. No external downloads/packages without task authorisation. Preserve GUIDs, species/state IDs and Reference v1. Art does not define ecology.

Import/technical contract: Unity 6000.6.0f1 / current URP, 1unit=1m, root-collar/ground pivots documented, shared materials and 512–1K textures unless measured reason otherwise, mips/compression/alpha clipping checked; no leaf MeshColliders. Polycount report LOD0 separately from all-LOD sum. Runtime-generated geometry/materials must be measured, not zeroed by static inventory assumptions.

Verification: actual active ForestTest player/camera at 1280×720,1600×900,1920×1080, warmed shaders and gizmos off; before/after fixed scene; close/mid/distant view; named LOD/scale/material/collision/recognition checks; profiler comparison where cost changes. No save/Reference changes. Report every unverified requirement. Include source files, export, manifest, screenshots, provenance and exact import/usage test steps.

Ownership: asset folders/catalogue bindings explicitly assigned at task start; confirm current UI/pedagogy owner for UI/highlight/labels; ScenarioOneManager minimal integration only with confirmed ownership. Do not change biology or ecology categories to fit visuals. Stop if save schema, unrelated mechanic, uncertain redistribution rights, incompatible style/scale, Reference, or overlapping writer becomes necessary. Return concrete proposal and continue independent inspection.

## Specific production brief

Starting assets: Assets/ForestPrototype/Prefabs/BeechMature01.prefab, SessileOakMature01.prefab and Forestry/Pruning broadleaf folders. First measure active LOD0 triangles and GPU/CPU cost of1/10/50 instances at fixed distances; do not use all-LOD sum as frame budget. Proposed targets for trial only: current silhouette-preserving hero LOD0≤60k,LOD1≤15k,LOD2≤3k, far impostor/cull if package-free and validated; shared materials≤3 per active tree. Preserve pruning branch-state masks/exports/GUID bindings. No gain claimed without identical-camera before/after profiler and pruning gate.
