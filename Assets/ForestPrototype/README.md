# Forest movement prototype

Open `Assets/Scenes/ForestTest.unity` in the CCF project, enter Play Mode, and click the Game view to capture the mouse.

- WASD: move at 4 metres/second (diagonal movement is normalized).
- Hold either Shift key: run at 7 metres/second.
- Space: jump from the ground; release and press Space again while airborne for one higher upward boost. Landing resets the double jump.
- Mouse: turn and look up/down (pitch limited to 85 degrees).
- Escape: release the mouse and stop horizontal movement.
- Left click in the Game view: capture the mouse again.

The scene contains a 40 x 40 metre ground, 68 conifer trees, four visible stone boundary ridges, one directional light and one first-person player. A winding dirt path leads from the spawn into a small clearing, with varied tree spacing and sizes. Ground, trunks and borders have collision; the thin path and clearing surfaces are decorative and have no colliders. Movement includes walking, running and jumping. Trees can be inspected and harvested with four timed chops; harvested trees become stumps that stay harvested, and add wood to the HUD. The clearing also contains an unbuilt Forestry Workbench site: aim at it and press E or left click, then spend 8 wood to reveal the workbench. A shortage message appears when the player has insufficient wood. Gravity uses a CharacterController; the camera is a child of the player.

## Project inspection

- Unity 6000.6.0f1, revision f7f8ed4d1e24.
- Universal Render Pipeline 17.6.0. PC quality uses PC_RPAsset; Mobile quality uses Mobile_RPAsset. The GraphicsSettings fallback pipeline is unassigned, but both quality levels select URP.
- Input System 1.20.0 only (`activeInputHandler: 1`). ForestPlayer reads Keyboard and Mouse from this existing package. The template InputSystem_Actions asset remains unchanged.
- The original SampleScene contains Main Camera, Directional Light and Global Volume, without a player/controller.
- Other installed packages include AI Navigation 2.0.14, Test Framework 1.8.0, Timeline 6.6.0, uGUI 2.6.0 and Visual Scripting 1.9.12. No packages were added or changed.

`ForestPlayer.cs` is the existing runtime controller and remains separate from `ForestBuildable.cs`, which owns the prototype workbench interaction. `Editor/ForestSceneBuilder.cs` creates the baseline scene through Tools > Forest Prototype > Create Test Scene, and refuses to overwrite an existing ForestTest scene. The scene is saved with ordinary editable objects, so it needs no runtime generation. `ForestTest.unity` is the only enabled build scene; `SampleScene.unity` remains in the project as a template.

## Verification (13 September 2026)

Unity 6000.6.0f1 compiled the full project in a temporary copy with the same Assets, Packages and ProjectSettings. Scene creation exited successfully (exit 0). A separate Unity run reopened the saved ForestTest scene and validated its scripts, material/shader references and player camera reference, with exactly one player, camera and audio listener (exit 0). The installed scene and scripts match those verified files byte for byte. Static checks on the scene in CCF also found no unresolved internal or asset GUID references. The package manifest, package lock, quality settings and original SampleScene match their original copies.

Interactive Play Mode movement, harvesting, inspection, collision and visual appearance were verified by the user. Stumps remain inspectable after harvesting. Open ForestTest, press Play, click Game view, then verify the workbench shortage message and the funded build transition. No additional setup is required.

Validation encountered an existing-editor project lock and a standalone licensing failure; both were handled by using an isolated copy launched through Unity Hub's Flatpak environment. An additive-scene startup error was fixed in the builder. One validation process exited with code 137; a retry with two worker threads passed. No C# compilation errors remain in the successful validation runs.

## Jump and run update

Added Space to jump when grounded and either Shift key to run while held. Ceiling collisions cancel upward velocity. Run Speed and Jump Height can be adjusted on the Player component. The updated runtime assembly compiled without errors using the installed Unity compiler and the project's existing compiler options and references. These new controls have not been interactively play-tested by the assistant. The user confirmed the original movement prototype worked.

## Double jump and forest detail update

A second Space press while airborne adds one upward impulse (Second Jump Height defaults to 2 metres). There is no third jump until landing. Boundary ridges are now 5 metres high to contain the stronger jump. The 68 trees have varied spacing, heights and canopy sizes, with a winding dirt path and a small clearing. ForestSceneBuilder remains the original baseline scene generator and does not overwrite this edited scene.

The updated controller compiled against the existing Unity project references. Static scene checks verified object/component links, hierarchy, one player and camera, and non-colliding path surfaces. This update has not been visually inspected in Unity or interactively play-tested by the assistant. Exit Play Mode and reopen ForestTest to load the updated layout.

## Tree state and simplified regeneration (14 September 2026)

Every tree now carries a `ForestTree` component with a stable `treeId`, cached trunk/canopy references, chop progress, and a stage (`Mature`, `Stump`, `Sapling`, `Young`). `ForestPlayer` resolves trees through this component instead of names or hierarchy, and stump inspection reads the same state. `ForestSceneBuilder` attaches the component when generating a fresh scene, and its validator checks the tree references.

Regrowth was deliberately simplified gameplay, not researched forest ecology. Placeholder timings were 20 seconds from stump to sapling and 30 seconds from sapling to young tree. Phase 5 removed this cycle: harvesting now leaves a stump indefinitely, and future regeneration will create new individuals with new ids rather than recycling the harvested tree. The enum values remain for save compatibility, and legacy saplings or young trees in old saves still render and can be chopped.

## Save and load (14 September 2026)

Press F5 to save and F9 to load. The save file is `forest-save.json` under `Application.persistentDataPath`. It stores the wood count, every tree's stable id with stage, stage timer and chop progress, and each buildable's id with its built flag. Trees are matched by `treeId`, not by scene object name. The controller lives on the `Game State` object.

## Tree simulation data (14 September 2026)

Each tree carries authoritative simulation values on `ForestTree`: `heightMeters`, `diameterCm` and `crownRadiusMeters`, seeded once from the placeholder geometry during the phase-1 migration (derived visuals matched the existing scene with zero drift). Height, trunk diameter, crown radius and wood yield are read from this data; the primitive trunk and canopy only visualise it, and `Awake` re-applies the mature shape from the data. Scaling or replacing a placeholder mesh therefore cannot change a tree's height, timber quantity or ecological state. The scene builder writes the same values for generated trees, and the validator rejects non-positive tree data. `Height` stays stage-aware so legacy stump, sapling and young visuals keep their placeholder dimensions.

## Tree species (14 September 2026)

`TreeSpeciesDefinition` is a small ScriptableObject holding only identity: `speciesId`, `displayName` and `latinName`. The first species asset is `Assets/ForestPrototype/Species/SitkaSpruce.asset` (Sitka spruce, Picea sitchensis), and all 68 plantation trees reference it. The inspection card shows the tree's species instead of a hardcoded name. The scene builder creates the asset when missing and assigns it to generated trees, and the validator rejects trees with a missing species and duplicate tree ids.

## Conifer placeholder (14 September 2026)

The spherical broadleaf canopy was replaced with a placeholder conifer silhouette: a generated `ConiferCone` mesh (`Assets/ForestPrototype/Meshes/ConiferCone.asset`, 14 segments) stacked three times in `Assets/ForestPrototype/Prefabs/PF_ConiferCanopy.prefab`. Every tree's `Canopy` child is now an instance of that prefab, keeping its previous position and scale, so the stand reads as one conifer plantation from player height. Gameplay is unchanged: felling still deactivates the canopy, and the stump, sapling and young stages reuse the same prefab at their placeholder scales. The scene builder creates the mesh and prefab when missing and instantiates them for generated trees.

## Harvest stays harvested (14 September 2026)

Automatic stump-to-sapling-to-young regrowth was removed. `ForestTree` no longer has an `Update` method; a chopped tree stays a stump with its canopy hidden. The stage enum values are kept so existing saves load, and legacy sapling/young trees from old saves still render and can be chopped, but nothing progresses on its own. Verified in Play Mode: a chopped tree stayed a stump past the old 20 second threshold (48 seconds of game time), and save then load preserved the stump. Future ecological regeneration will create new individuals with new ids.

## Versioned saves and dynamic discovery (14 September 2026)

`ForestSaveData` now carries a `version` field (currently 1). Saves written before versioning load as version 1, a newer version logs a warning and loads best-effort, and future migrations have a marked place in `ForestSaveController.Load`. The controller no longer caches the player, trees or buildables in `Awake`; it discovers them at save and load time with `FindObjectsByType(FindObjectsInactive.Include, ...)`, so runtime-created trees and objects participate in saves. Verified: an unversioned save loaded correctly, a tree spawned at runtime (T69) was discovered, appeared in the new save with its chop progress, and was restored on load.

## Ecological time (14 September 2026)

`ForestEcologyController` on the `Game State` object holds an `ecologicalYear` and exposes one explicit step, `AdvanceOneYear()`, also available through its context menu, so the editor and MCP can advance exactly one year. Nothing in normal gameplay advances ecological time: the component has no `Update`, and one ecological year is deliberately not tied to a game day or a real-time minute. Verified: two calls moved the year from 0 to 2 with no change to `Time.timeScale`, and the validator now requires exactly one ecology controller.

## Local ecology grid (14 September 2026)

`ForestEcologyController` also owns a regeneration/environment grid over the 40 x 40 m stand: `standSizeMeters` and `cellSizeMeters` are serialized and configurable (5 m cells give 8 x 8 = 64 cells; 10 m cells give 16). Each `ForestEcologyCell` stores its centre position and canopy/light state. The initial pass was coarse placeholder coverage from nearby living crowns, replaced by the phase-9 crown-influence calculation below. A toggleable debug view (`ShowDebugGrid`) draws a top-down heatmap of canopy/light in the top-right corner, labelled with the year and cell size. Verified: 64 cells built with varied values (0.00 to 0.69 canopy), the heatmap shows different local conditions, and the cell size rebuild was tested at 10 m (16 cells) before restoring 5 m.

## Local canopy and light (14 September 2026)

The grid's canopy value now comes from local crown influence: each living tree shades a cell by a lateral falloff from its authoritative position, crown radius and height, reaching half a cell beyond the crown, combined as fractional cover (`1 - product of gaps`); light is the complement. There is no global canopy percentage. `ForestTree` raises a static `Felled` event, and `ForestEcologyController` recomputes on that event, on `AdvanceOneYear`, and after a save load, so the ecological maths never lives in `ForestPlayer`. Verified: felling T01 lowered only cells within 4.3 m (max drop 0.266) while cells 15 m or more away changed by 0.0000; felling T40 instead affected completely different cells (1, 8 and 9 versus 39 and 47); both sessions started from the same deterministic baseline (canopy sum 39.04).

## Milestone A: wood logistics (14 September 2026)

Wood is now a carried resource with a limit. `ForestPlayer` owns `maxCarriedWood` (provisional calibration: 20) and `carriedWood`, with `CanCarryWood`, `TryAddWood`, `TrySpendWood` and `RestoreCarriedWood`; nothing else writes the value. The felling stroke only completes when the whole tree yield fits, so no timber is lost or partially collected - the tree keeps its chop progress and the HUD warns with "Need N free wood capacity. Free space: M.". The HUD shows "Carried Wood" and `N / M`, turning warm when full. `RestoreCarriedWood` deliberately bypasses the limit so version-1 saves that carry more than the new capacity keep every unit; they simply cannot collect more until they spend or deposit down to the limit.

`ForestBuildable` gained an optional `requiredBuildable` prerequisite and spends wood only through `TrySpendWood`. The new `ForestWoodStorage` (id `log-rack-01`, display name "Log Rack", capacity 100) activates after its `ForestBuildable` is constructed, deposits as much carried wood as fits on E, withdraws as much as the player can carry on F, and shows three placeholder log layers (low, medium, full) driven by stored amount. A "Log Rack" site (build id `log-rack-build-01`, cost 6, prerequisite Forestry Workbench) now sits beside the workbench in `ForestTest`, and the scene builder creates the same workbench and rack arrangement for fresh scenes. Save data is version 2 with a `storages` list; loading restores carried wood, buildable states, storage amounts and the visual fill, then recomputes the ecology grid. Validator additions cover duplicate/missing buildable and storage ids, invalid capacities, storage without a buildable or prerequisite, and invalid player capacity.

## Sitka regeneration and growth (14 September 2026)

The stand now runs the Sitka report's annual pipeline in `ForestEcologyController.AdvanceOneYear()`, in the report's order: competition -> canopy/light -> adult DBH/height -> crown relaxation -> existing regeneration -> mast -> seed rain -> new establishment -> recruitment -> suitability -> exposure decay -> diagnostics.

- `ForestTree` carries `ageYears` and explicit simulation writers (`ApplyGrowth`, `RelaxCrownRadius`, `SetSimulationState`); placeholder mesh scale never drives state.
- `TreeSpeciesDefinition` holds the full Sitka schema, with provenance tags in every tooltip: [A] empirical anchor, [B] evidence-derived relationship, [C] simulation abstraction, [D] gameplay/model calibration. `CI50`, crown relaxation, `S50`, mast strength, promotion height and wind multipliers are calibrations, not measured constants.
- Competition is local Hegyi `CI = sum(DBH_neighbour / DBH_target / distance)` with a 20 m cutoff (performance abstraction). DBH growth is reduced by `1 / (1 + CI / CI50)`; height growth follows age and site and is deliberately not multiplied by the same factor, so thinning affects girth far more than height.
- Crown radius targets the evidence-derived `0.9415 + 0.07635 x DBH_cm`, reduced by competition, and relaxes at `crownRelaxationPerYear` (15%) rather than snapping.
- Reproduction: maturity ramps 20 -> 30 years, mast is deterministic per (seed, year) with good years ~15% and poor ~35%, dispersal is `exp(-distance / 20 m)` with a 150 m performance cutoff, and establishment is saturated seed rain `(1 - exp(-seedRain / S50))` times juvenile light response times site suitability. The juvenile light curve uses the report's 10 / 20 / 25 / 50-60% RLI anchors; interpolation is simulation design.
- Regeneration is one aggregated cohort per cell (density proxy, height, establishment year). At `promotionHeightM` (3.5 m calibration) the cohort recruits a single persistent `ForestTree` with a unique `R{year}-{cell}` id through `ForestTreeSpawner`, so no seedling GameObjects exist.
- Cell site state: `siteProductivity`, `soilStability` and `establishmentSuitability` (simple defaults; suitability dips after heavy disturbance so maximum light is deliberately not always optimal).
- Wind risk is diagnostic only: `susceptibility x slenderness x local opening x recent opening`; no stochastic storm mortality is applied.
- Save data is version 3 and persists ecological year, simulation seed, per-tree simulation state and position, and regeneration/exposure cell state. Loading removes recruited trees that are not in the save and resets unsaved cells, so the same save is deterministic. Legacy v1/v2 saves still load, keeping scene trees' simulation defaults. Validator additions cover species growth data and light curves, tree age/dimensions, ecology grid configuration, and the spawner references.

Validation experiments (all agent-run, deterministic seed):

- Competition release: removing three neighbours around T01 cut its CI from 2.55 to 1.92. Over 20 years the released tree gained +3.91 cm DBH and +1.93 m crown radius versus +2.65 cm / +0.77 m for an equivalent unreleased tree (1.5x DBH, 2.5x crown), while height increments were equal (6.79 m vs 6.71 m).
- Seed-tree retention: with mature seed trees retained around one gap and removed to 30 m around the other, the first good mast year gave 2.8x more seed rain (1.83 vs 0.66) and 1.8x higher regeneration density after 16 years. The 40 x 40 m stand limits absolute contrast because a 20 m dispersal kernel reaches almost everywhere.
- Thinning intensity and stability: a sudden heavy opening produced the greatest short-term exposure (max wind risk 10.3 one year later vs 5.4 moderate vs 5.6 unthinned), the unthinned group stayed non-zero, and repeated moderate thinning produced a smaller second pulse that decayed with the 3-year half-life.
- Spatial harvesting: the same eight trees removed dispersed versus concentrated diverged in light variance (0.056 vs 0.084), seed variance (0.68 vs 0.84), mean competition (3.33 vs 3.57) and maximum wind risk (6.2 vs 9.1).
- Determinism: save -> advance 5 years -> load -> advance 5 years reproduced the exact same stand (DBH sum 4651.8810, seed max 8.0132, regeneration sum 41.7313). A recruited tree survived save/load exactly once with identical position, age, DBH, height and crown. A realistic version-2 save still loads (carried wood, stages, builds, storage; scene trees keep simulation defaults; no duplicates).

## Stored-timber construction and the Basic Shelter (14 September 2026)

Construction no longer requires every log to be carried to the site. `ForestBuildable` pays carried wood first and pulls the remainder from active `ForestWoodStorage` racks within `storageSearchRadius` (6 m default), summing the active stores in range; the shortage message reports carried and nearby stored separately, and the build notification records the split. `ForestWoodStorage.TakeStoredWood(amount)` is the only new API. The shelter persists by `buildId` through the existing buildables list; this feature adds no save fields (the marking system below raises the file to version 4).

A **Basic Shelter** site (build id `shelter-01`, cost 20 wood, prerequisite Forestry Workbench) now sits 5.35 m west of the Log Rack in `ForestTest`, and `ForestSceneBuilder.CreateShelter` generates the same arrangement for fresh scenes. It follows the placeholder style of the other sites: stone footings and corner stakes when unbuilt, and a built **walk-in** lean-to with posts, beams, a slanted roof and a plank back wall (2.2 m front posts, 2.7 m back posts, roof front edge about 2.12 m). While unbuilt, a site-volume collider on the Unbuilt Site is the aim target; when built it deactivates and only the structure is solid (posts, wall boards and roof carry colliders), so the open front stays walkable. Physics checks in Play Mode confirmed the doorway and interior are clear for the 1.8 m player, the back wall blocks, and the roof sits 0.42 m above eye height inside.

Verified in Play Mode through the live MCP session (agent-run, deterministic state; UI text confirmed by game-view screenshots): mixed payment 5 carried + 15 stored built the shelter spending exactly 20 wood; a 0 + 10 shortage refused with both totals unchanged; 20 carried + 0 stored built with nothing left; a rack moved 20 m out of range refused with the store untouched; save then load preserved the built shelter and the 5-wood remainder; deposit and withdraw still round-trip 10 wood. The input trigger path (E/left click, cursor lock, raycast) is unchanged from the user-verified workbench and rack interactions; it was not re-driven with synthetic input because KitWright's simulated key events did not reach the game's Input System in this environment.

## Forestry marking and timber interface (14 September 2026)

- `ForestTreeMarkingManager` (on the `Game State` object) is the smallest maintainable marking system: aim at a living tree and press **M** to mark or unmark it. Marking never fells; the manager subscribes to `ForestTree.Felled` and clears a tree's mark once it is actually harvested, so the list always means "marked but not yet cut". Markers are runtime-created (an unlit diamond above the crown plus a ground disc) parented to the marked tree, so the visual follows it and disappears on unmark or felling. Marks are keyed by the persistent tree id and `ForestTree`'s public API is unchanged.
- Save format is version 4 and adds `markedTreeIds`. Loading restores marks (and their markers) after scene and recruited trees are restored, after clearing any runtime marks, so reloading is deterministic. Version 3 and older saves still load and simply carry no marks.
- `ForestTree.BiologicalStemVolumeM3` is the additive biological timber output: `DBH_cm^2 x 0.00007854 x formHeight_m`, where form height is tree height times a [D] calibration `formHeightRatio` on the species (default 0.5). `WoodYield` and the existing harvesting contract are unchanged; stumps report 0 m3.
- Verified: M marks and unmarks through the real input path; markers appear on exactly the marked trees; the counter tracks marks; felling auto-clears the mark and still yields wood; save v4 wrote `markedTreeIds: [T03, T07]` and load restored marks and markers; a legacy v3 save loaded with marks cleared and no errors; T02's volume matched the formula exactly (0.6857 m3) and a felled tree reported 0 m3.

## Volume-based wood yield (14 September 2026)

The wood a felled tree gives is now a Survival-side conversion of Forestry's biological stem volume: `units = clamp(round(volume_m3 / cubicMetersPerWoodUnit), 3, 10)`, with `cubicMetersPerWoodUnit = 0.10` ([D] calibration, user-approved) on `ForestPlayer`. The inspection card's "Potential Wood Yield" shows the same conversion; the shortage message and the Timber! notification report the converted amount. `ForestTree.WoodYield` (height-based) is no longer read by gameplay and is left for the Forestry stream to retire or reuse. No save-format change: yield is derived at harvest time. Verified in Play Mode through the live MCP session (agent-run): felling T02 (0.6857 m3) awarded exactly 7 wood (round(6.857) = 7, wood conserved, tree became a stump); with 15 of 20 carried, a final stroke on Tree 31 (10 units) was refused with the tree, its chop progress and the carried amount unchanged — partial collection remains impossible and carrying capacity stays enforced.
## Inspection enrichment (14 September 2026)

- The inspect card (E while aiming) now completes the `inspect -> compare -> mark` loop. Added lines: age, biological stem volume, marked state ("MARKED for harvest (M to unmark)" appended to the status line when marked), local crowding as a player-readable band with the competition index in brackets, recent DBH growth (only shown once a nonzero annual growth has been computed), a wind-vulnerability band, and reproduction status derived from the species maturity curve ("not yet seed-bearing" / "maturing — some seed" / "seed-bearing"). The card grew from 250 px to 380 px to fit; all ForestPlayer changes are additive internals, no public API changes.
- `ForestEcologyController` gained player-readable interpretation helpers: `GetCompetitionLabel` (open / released below CI 1, moderate below 3, crowded above — [D] thresholds) and `GetWindRiskLabel` (low / moderate / high — [D] thresholds). Raw simulation internals and calibration constants stay off the card.
- Competition is computed during the annual update; for inspection before the first year tick it is now computed lazily on demand (`competitionCurrent` flag + `InvalidateCompetition()`), so the card is correct even right after scene load. Save format is untouched.
- Verified in Play Mode on T01: card shows Species / Status / Age 45 / Height / Diameter / Stem volume 1.10 m3 (matches formula) / crowding "moderate (CI 2.5)" / wind "low" / reproduction "seed-bearing" (maturity 1.0 at 45 y); marking via the manager flips the status line to "MARKED for harvest (M to unmark)", bumps the marked counter and shows the ground marker, unmarking restores all three; growth line correctly hidden while no annual tick has run; no console errors.

## Marked-treatment summary (14 September 2026)

- The marking HUD (bottom-left) is now a treatment summary: "Marked for harvest: N — X.X m³ [M] mark / unmark", where N is the trees still standing with a mark and X.X is their summed `ForestTree.BiologicalStemVolumeM3`. Marking selects a treatment; it never fells, and the volume readout stays biological — any volume-to-wood conversion remains out of Forestry.
- `ForestTreeMarkingManager` gained `LivingMarkedCount` and `MarkedVolumeM3`, recomputed each frame from a cached mark list that is rebuilt whenever the mark set changes and at least twice a second. A periodic sweep prunes marks whose tree disappeared without a felling event (and recreates a lost marker visual on a still-marked living tree), so destroyed trees cannot linger in the count or the volume. `Save`/`Load` and the felling auto-clear path needed no changes; save version stays 4.
- Verified in Play Mode: marking T01 + T03 gave exactly the formula sum (1.7443 m3); unmarking T01 left exactly T03's volume (0.6472); felling T03 through `Fell()` cleared the mark, dropped count and volume to 0, and the stump reports 0 m3; save wrote `markedTreeIds: [T02]`, clear-then-load restored it with count 1 and volume 0.6857; destroying a marked tree's GameObject without a felling event was pruned by the sweep within one sweep interval; HUD visually shows "Marked for harvest: 2 — 2.5 m3"; no console errors.

## Timber Sledge: earned carrying capacity (14 September 2026)

The carrying limit is no longer fixed. `ForestPlayer.MaxCarriedWood` is derived as the serialized base (20) plus the `carriedWoodCapacityBonus` of every built `ForestBuildable`, so purchased capacity survives save/load through the existing built flags with no new save fields. The first buyer is the **Timber Sledge** (build id `timber-sledge-01`, cost 12 wood, prerequisite Forestry Workbench, bonus +15) beside the Log Rack in `ForestTest`; `ForestSceneBuilder.CreateTimberSledge` generates the same arrangement for fresh scenes. All limit enforcement now reads the derived value: `CanCarryWood`, `TryAddWood`/`FreeWoodCapacity`, the felling free-space check, the HUD warm colour and the HUD readout.

Verified in Play Mode through the live MCP session (agent-run, deterministic state): base capacity 20; building the sledge from stored timber (25 -> 13) raised the limit to 35; at 30/35 free space read 5 and adding 3 wood landed exactly 33; felling a 10-unit tree at 15/35 awarded exactly 25 (wood conserved, limit enforced); save then load restored the sledge's built flag and the 35 limit with no save-format change. One defect was caught and fixed during this verification: the enforcement paths and the HUD still read the raw base field, which would have made the bonus unusable — the deterministic test exposed it (free space 0 at 30/35, HUD "25 / 20") before it could ship.

## Saw Pit: planks and the second rack (14 September 2026)

Planks enter the game with no save-format change (Option A, user-approved): the **Plank Rack** is an ordinary `ForestWoodStorage` (`plank-rack-01`, capacity 40) flagged `storesPlanks`, so the existing storages list persists it, wood-cost pulls ignore it (planks are never spent as wood), and hand-carried deposit/withdraw is skipped for it. The **Saw Pit** (build id `saw-pit-01`, cost 6 wood, prerequisite Forestry Workbench) sits west of the workbench; once built, aiming at it and pressing E converts one batch of `logsPerBatch` (5, [D]) stored logs from active wood racks within `logRackSearchRadius` (8 m) into `planksPerBatch` (2, [D]) planks. `ForestWoodStorage` gained `AddStoredWood` for processing output and a `DisplayName` getter. The first plank use is **Log Rack II** (build id `log-rack-build-02`, 4 wood + 4 planks, prerequisite Saw Pit), an extra 100-capacity rack east of the clearing; `ForestBuildable` now supports an optional `plankCost` with an explicit `plankSource` reference, reported in the prompt and the build notification. Verified in Play Mode through the live MCP session (agent-run): 21 stored logs became a saw pit (-6) and three exact 5->2 batches (10/2, 5/4, 0/6) with a fourth batch refused; Log Rack II built for exactly 4 carried wood + 4 planks; a plank-shortage rebuild refused with planks untouched; a wood-only shelter build with planks nearby refused with planks unspent (planks are not wood); save then load restored the saw pit, the second rack and the plank quantity with no save-format change.

## Click-capture view bounce fix (14 September 2026)

Clicking to capture the mouse (after Escape or focus loss) locked the cursor, whose centre warp arrived as a delta spike with no suppression on that path, throwing the view away from the aimed tree; the periodic recentre could also leak part of its spike when the OS split the warp delta across frames. The grace window now discards every delta for its two frames (about 33 ms of hand movement, imperceptible) instead of only spikes above 250 px, and the grace is armed in three places: play start (OnEnable), the capture click, and the periodic recentre. Verified by compile and Play Mode sanity run; the interactive feel of clicking on trees and aiming across them needs the user's confirmation.

## Marking state hygiene + spatial treatment verification (14 September 2026)

- Bug fix in `ForestTreeMarkingManager`: the mark set and marker registry are static, and play-mode sessions in one editor run do not domain-reload, so a fresh play session inherited the previous session's pending marks (a stale mark from HUD testing entered the first experiment run and was felled with the treatment). `Awake` now clears both statics so every session starts with an empty pending-treatment list. This touches no save data and no other stream's code.

Spatial treatment verification (all agent-run Play Mode, deterministic seed 20260914, identical scene baseline: 68 trees, 71.6576 m3 standing, mean CI 3.704, mean cell light 0.390, max wind 7.78; treatments selected through the real marking manager and felled through `Fell()`; marking summary cross-checked exact in every run — 8/8, 11/11, 16/16 and volume sums exact; 10 ecological years each; control y5 regen sum 41.731 reproduces the earlier determinism anchor):

| year 10 | control | dispersed-8 | group-8 | seed-tree-11 | heavy dispersed-16 |
| --- | --- | --- | --- | --- | --- |
| removed m3 | 0 | 7.612 | 8.520 | 10.296 (1 seed tree retained in block) | 15.659 |
| living trees | 68 | 60 | 60 | 57 | 52 |
| mean CI | 3.696 | 3.413 | 3.545 | 3.386 | 3.005 |
| mean DBH growth cm/yr | 0.172 | 0.177 | 0.176 | 0.178 | 0.188 |
| mean cell light | 0.126 | 0.177 | 0.185 | 0.177 | 0.203 |
| light variance | 0.029 | 0.052 | 0.056 | 0.046 | 0.064 |
| regen density sum | 39.25 | 43.09 | 46.17 | 48.43 | 49.36 |
| max wind year 1 | 8.05 | 9.51 | 24.50 | 16.68 | 9.43 |
| max wind year 10 | 8.63 | 7.01 | 10.63 | 8.48 | 7.36 |
| standing volume m3 | 121.86 | 109.16 | 107.59 | 104.46 | 95.47 |

Useful trade-offs (visibly and numerically different, same removal count for the first two):

- **Dispersed selection (8)** is the low-exposure treatment: modest regeneration gain (+10% over control), the best per-tree competition relief among 8-tree treatments (mean CI 3.413), a small wind spike (9.5) that decays below baseline by year 10 (7.0), and the least total production loss (removed + standing = 116.8 vs 121.9 control).
- **Small group opening (8, clustered)** produces the strongest regeneration response per tree removed (+18% over control, 46.2) but a 24.5 max-wind spike that still sits at 10.6 after ten years — the opening's edge trees read far into the "high" band. A real trade-off: canopy-gap regeneration versus exposure, exactly what CCF planning needs to reason about.
- **Seed-tree retention (12-block minus the central T17)** combines the two: regeneration higher than the plain group opening from year 6 on (50.5 vs 48.7 peak, 48.4 vs 46.2 at y10) with wind max between the two (16.7). The retained seed tree measurably lifts gap seed rain — the mark/retain decision has a numeric consequence.
- **Heavier dispersed opening (16)** gives the strongest growth release (+9.3% mean DBH growth, CI 3.01) and the highest regeneration (49.4) while keeping max wind near dispersed-8 levels (9.4) — spreading the same or double removal across the stand avoids the per-cell opening accumulation that clusters create. The cost is standing production: 95.5 m3 at y10 (−21.7% vs control).

Responses too weak or too strong ([D] observations, nothing changed):

- Too weak: the DBH growth response saturates. Even doubling removal intensity only lifts mean growth from 0.172 to 0.188 cm/yr (+9%) via the `1/(1 + CI/Ci50)` response; thinning barely feels rewarding at the stand level. If release should feel more significant, `Ci50` (or a steeper response curve) is the [D] lever — proposal only.
- Too strong: clustered removals stack `RecentOpening` ~2 per cell, and `GetWindRisk` multiplies it linearly (`1 + WindOpeningWeight x RecentOpening`), so an 8-tree group opening reads as a 24.5 "high" — 3.2x the dispersed response and 2.5x the y10 value. Proposal [D]: cap or normalize per-cell `RecentOpening` (for example cap 2.0) or lower `WindOpeningWeight` so a legitimate group opening is serious but not catastrophic.
- Wind band calibration: the unthinned control already reports max risk 7.8-8.6 ("high" >= 7) with no treatment at all, so the diagnostic bands read alarmist at rest. Proposal [D]: recalibrate the high threshold (or scale the susceptibility constant) so "high" means treatment-relevant exposure rather than the dense-stand norm.
