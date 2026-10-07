# Tree assets by species and life stage

No current family is declared Tier A production-ready: actual gameplay and technical coverage exists but comprehensive player recognition/performance testing is pending. Tiers B usable now, C prototype improvement needed, D missing/misleading requirement.

| Species / state | Current coverage | Tier and next check |
|---|---|---|
| Sitka seedling / small juvenile | SitkaSeedling01, three LODs; height-scaled cell representative | B/C; demonstrate .15/.4/.6 m recognisability and distinguish from non-actionable vegetation |
| Sitka sapling / pole | Pole and recent plantation stage/variant logic | B; check biological threshold transitions at fixed camera distance |
| Sitka adult / retained mature | Plantation02 variants, mature and benchmark sets, procedural stems | B; crowns and suppressed/released morphology repeated; use seed/scale/branch variants first |
| Sitka pruned | Dedicated pruning branch-state variants | B; see pruning gate; verify silhouette at minimum viewport |
| Sitka dead/fallen/stump | Shared record/log and SectionFive stump/decay assets | B/C; natural mortality uses fallen representation, not standing snag; no standing-dead asset demand without adopted mechanic |
| Oak seedling / juvenile | Species seedling reference uses sapling-derived silhouette | C; height-scaled model can resemble woody shrubs; prioritise species-labelled paired visibility test |
| Oak young / adult | Sapling01/Young01/Mature01 plus pruned sets | B visual/C technical; very high triangle totals require profiling |
| Beech seedling / juvenile | SectionFive juvenile and species visual references | C; distinguish regeneration from bilberry/bramble silhouettes at .15–2 m |
| Beech adult / pruned | Mature01 and pruning sets | B visual/C technical; mature prefab has no LOD and roughly 529k triangles |
| Oak/Beech damaged/browsed | No validated specific damage progression identified | D future optional; browse can be taught by surviving state/diagnostics without inventing deer damage art now |
| Broadleaf deadwood | Generic log record/scale rather than fully verified species-specific bark/cut wood | C; species distinction secondary to valid record/decay/diameter |

Runtime natural representative is one tallest represented band per cell/species and is scaled by height clamped .15–3. This is a display proxy, not a literal census of all juveniles. An authored 2 m sapling mesh multiplied directly by a .4 height scale does not automatically equal a .4 m model: verify actual bounds. Exact planted visuals have their own catalogue scaling. Age alone is not the stage selector; stunted trees can remain small. A one-year seedling must not simply display an adult crown shrunk into a shrub.

Triangle examples (sum distinct meshes across LODs): Sitka seedling 3,143; pole 16,152; mature 49,888/one LOD; oak sapling 54,694/three; oak young 365,088/three; oak mature 812,946/three; pruned oak mature 989,319/three. Oak mature LOD0 source alone ≈607,328. Beech Mature01 ≈529,022/zero LOD. Imported collider counts often zero because runtime ForestTree adds interaction colliders: do not diagnose missing interaction solely from prefab inventory.

Cheapest maintainable plan: retain current Sitka prototypes, validate height normalisation/LOD/material sharing, improve two or three juvenile silhouettes if recognition fails, and simplify broadleaf LOD meshes before generating more variants. Broadleaf seasonal packs and damaged forms wait for a demonstrated gameplay need.

## Measured P0: natural beech display height

The final disposable runtime measurement set recorded every species at0.6m. Sitka natural rendered0.6000m; oak natural0.60003m; beech natural1.2000m. Exact planted oak and beech both rendered0.6000m. Evidence/juvenile_display_heights.csv records target, world renderer bounds and root scale. ForestTest binds the2m Beech_Sapling_01 prefab for natural beech; ForestEcologyController.SyncSeedlingVisuals applies recorded height as a direct local-scale multiplier. ScenarioHabitatVisuals.PlaceJuveniles normalises authored bounds for exact planting. This is a verified presentation scaling inconsistency, not an ecological height error or a demand for a new beech mesh.

Priority: a future owned, bounded display-scale normalisation fix plus paired tree/shrub recognition checks. Reuse current beech mesh first. The asset audit did not modify this production path; the first creation packet explicitly includes the correction/verification before authoring replacements.
