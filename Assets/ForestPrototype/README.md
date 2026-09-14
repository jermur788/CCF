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
