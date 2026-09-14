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

Construction no longer requires every log to be carried to the site. `ForestBuildable` pays carried wood first and pulls the remainder from active `ForestWoodStorage` racks within `storageSearchRadius` (6 m default), summing the active stores in range; the shortage message reports carried and nearby stored separately, and the build notification records the split. `ForestWoodStorage.TakeStoredWood(amount)` is the only new API. The save format is unchanged (version 3), and the shelter persists by `buildId` through the existing buildables list.

A **Basic Shelter** site (build id `shelter-01`, cost 20 wood, prerequisite Forestry Workbench) now sits 5.35 m west of the Log Rack in `ForestTest`, and `ForestSceneBuilder.CreateShelter` generates the same arrangement for fresh scenes. It follows the placeholder style of the other sites: stone footings and corner stakes when unbuilt, and a built lean-to with posts, beams, a slanted roof and a plank back wall.

Verified in Play Mode through the live MCP session (agent-run, deterministic state; UI text confirmed by game-view screenshots): mixed payment 5 carried + 15 stored built the shelter spending exactly 20 wood; a 0 + 10 shortage refused with both totals unchanged; 20 carried + 0 stored built with nothing left; a rack moved 20 m out of range refused with the store untouched; save then load preserved the built shelter and the 5-wood remainder; deposit and withdraw still round-trip 10 wood. The input trigger path (E/left click, cursor lock, raycast) is unchanged from the user-verified workbench and rack interactions; it was not re-driven with synthetic input because KitWright's simulated key events did not reach the game's Input System in this environment.
