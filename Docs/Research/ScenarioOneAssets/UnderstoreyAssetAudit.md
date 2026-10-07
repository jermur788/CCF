# Understorey and tree-regeneration distinction

Biology must determine categories. Current persisted ferns/grasses/forbs/shrubs/mosses/fungi are habitat proxies; visual bracken/bramble/dwarf shrub are derived presentation classes. Displayed bracken-type cover is not independently measured bracken abundance. There is no adopted competitor-state model in this task.

| Group | Actual asset coverage | Readiness / clearance |
|---|---|---|
| Bramble | Bramble_Clump_01, 7,574 all-LOD triangles | B/C; tangled low form, density placement; competes only if future biological input says so |
| Bracken | Bracken_Clump_01, 13,258 all-LOD triangles | B; recognisable frond family, verify distinction from shade fern |
| Ordinary shade fern | ShadeFern_Clump_01, 4,272 | B; do not label generically harmful |
| Grass / rush | Three grass and two rush prefabs, 1,884–9,484 totals | B; ecological competitor subtype cannot be inferred just from visual occupancy |
| Herbs | Rosette/flowering prefabs | B; low layer; no generic penalty |
| Woody shrub / bilberry | Bilberry-type/current shrub silhouettes | C; potential confusion with young broadleaves, species/interaction cues needed |
| Moss | Patches/cushions/shoot mat | B visual, WATCH cost; 59,954-triangle shoot mat across LODs and many instances |
| Fungi | Brackets/mushrooms, attach to deadwood | B; retain with deadwood, no competition penalty |
| Litter / small branches | Current recent litter/scatter/brash | B/C; use for cleared aftermath sparingly, never create sterilised-soil effect |

Current area clearance removes selected ground vegetation and young-tree representation; moss/fungi are preserved. Circle suppression is spatial and treatment-year only. Procedural placement/count already supports LOW/MEDIUM/HIGH visual density: reuse a small silhouette set, fixed seed, bounded rotation/scale, and pooled/shared material instances; do not build three bespoke 'cover' meshes per species. Cover→count calibration is presentation D and should never enter biological mortality.

P0 validation requirement from reported player confusion: put oak/beech regeneration beside woody understorey at the same size and distance, with the actual selection/inspection prompts. First determine whether inexpensive interaction/label/highlight cues and height-normalised current meshes solve it. If not, make distinct single-leader juvenile trees and branched shrub silhouettes. This is a gameplay legibility issue, not evidence for competitive biology.

## Measured P0: natural beech display height

The final disposable runtime measurement set recorded every species at0.6m. Sitka natural rendered0.6000m; oak natural0.60003m; beech natural1.2000m. Exact planted oak and beech both rendered0.6000m. Evidence/juvenile_display_heights.csv records target, world renderer bounds and root scale. ForestTest binds the2m Beech_Sapling_01 prefab for natural beech; ForestEcologyController.SyncSeedlingVisuals applies recorded height as a direct local-scale multiplier. ScenarioHabitatVisuals.PlaceJuveniles normalises authored bounds for exact planting. This is a verified presentation scaling inconsistency, not an ecological height error or a demand for a new beech mesh.

Priority: a future owned, bounded display-scale normalisation fix plus paired tree/shrub recognition checks. Reuse current beech mesh first. The asset audit did not modify this production path; the first creation packet explicitly includes the correction/verification before authoring replacements.
