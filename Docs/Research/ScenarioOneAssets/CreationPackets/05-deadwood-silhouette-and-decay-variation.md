# Future asset packet 05: Deadwood silhouette and decay variation

Priority P1; estimated complexity M.

Problem: Growth1 makes repeated mortality logs more prominent. Mechanic: deadwood / self-thinning.

Current substitute: fresh/decayed log and fungi/moss. Preferred method: PROCEDURAL VARIANT / MODIFY.

Required deliverable: inspect the active current substitute and demonstrate whether bounded reuse solves the problem; create/modify only the smallest justified set. See AssetCreationStrategy.md for custom geometry/material briefs. Dependencies: profiler and actual record sizes.

Acceptance: Diameter/length correct; distinct silhouettes; no duplicate rebuild or excessive dressing.

Source context: main a8596df9c52669a36709f09e85dfe6568640af49; audit 5c389cc; task/understorey-recruitment-causality. Before starting, fetch current main, record actual base/context and read current AGENTS/AgentWorkflow/canonical decisions and live ownership. This packet is future work, not an automatic authorisation to edit current main. Use an owned dedicated/reusable worktree. One Unity Editor at a time; no Blender concurrently with heavy Unity.

Style: restrained current Irish woodland prototype, plausible morphology/scale and readable silhouettes. Reuse current assets first; source/license register required for any original/generated/external content. No external downloads/packages without task authorisation. Preserve GUIDs, species/state IDs and Reference v1. Art does not define ecology.

Import/technical contract: Unity 6000.6.0f1 / current URP, 1unit=1m, root-collar/ground pivots documented, shared materials and 512–1K textures unless measured reason otherwise, mips/compression/alpha clipping checked; no leaf MeshColliders. Polycount report LOD0 separately from all-LOD sum. Runtime-generated geometry/materials must be measured, not zeroed by static inventory assumptions.

Verification: actual active ForestTest player/camera at 1280×720,1600×900,1920×1080, warmed shaders and gizmos off; before/after fixed scene; close/mid/distant view; named LOD/scale/material/collision/recognition checks; profiler comparison where cost changes. No save/Reference changes. Report every unverified requirement. Include source files, export, manifest, screenshots, provenance and exact import/usage test steps.

Ownership: asset folders/catalogue bindings explicitly assigned at task start; confirm current UI/pedagogy owner for UI/highlight/labels; ScenarioOneManager minimal integration only with confirmed ownership. Do not change biology or ecology categories to fit visuals. Stop if save schema, unrelated mechanic, uncertain redistribution rights, incompatible style/scale, Reference, or overlapping writer becomes necessary. Return concrete proposal and continue independent inspection.

## Specific production brief

Starting assets: Forestry/SectionFive/{SS_Log_Fresh_01,SS_Log_Decayed_01,SS_Stump_01,Moss_Deadwood_Patch_01,Deadwood_Bracket_Cluster_01}.prefab. Required2–3 log silhouettes only after scaling/rotation/reuse trial; tapered/curved trunks, ≤1500/500/120tris at3 LODs, ≤2 sharedmaterials,1K tiling bark +512 cut ends. Preserve actual record length/diameter and species context. Test25/100/300records and duplicate rebuild/save-load. No storm root plate or new standing-snag mechanic.
