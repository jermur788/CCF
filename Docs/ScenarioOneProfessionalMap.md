# Scenario One property map

The Stand Map and the HUD minimap are two presentations of one plan of the property. This note says what the plan draws, where each thing comes from, what it deliberately does not draw, and how the pieces fit, so the next change starts from facts.

Task: SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1. Visual direction: a forestry-management / survey-style property plan (reference sheet `Griffin's Peak Forestry Map UI(1).png`, used for tone, composition and legend treatment only, **not** for its geography). Architecture baseline: the Lane B shared map (`59ab507`), kept.

## What is drawn, and where it comes from

| On the map | Source (nothing is copied or invented) | Full | Mini |
|---|---|---|---|
| Property boundary, dashed | `ForestEcologyController.StandBounds` (80 x 80 m for new games, 40 x 40 m for old saves) | yes | yes |
| Every tree as a crown | each `ForestTree`: position, `SimulationCrownRadiusMeters`, height, species, living / standing dead / stump | yes | yes (no stumps) |
| Conifer or broadleaf look | the species id (Sitka spruce and Scots pine conifer; beech, sessile oak, downy birch, common alder broadleaf); an unknown species is "other broadleaf" | yes | yes |
| Local grid every 10 m, easting and northing labels, scale bar | derived from the property bounds (metres from the south-west corner); not latitude / longitude | yes | no |
| North arrow | north is up | yes | yes |
| Forest path, with a dark edge | the scene's "Dirt Path NN" planks, at their real width | yes | yes |
| Clearing (work area) | the scene's "Forest Clearing" | yes | yes |
| Racks, at their real 2.4 m corridor width | `RackLayoutTracker` and the session's confirmed racks | yes | yes (not the rack being planned) |
| You (dot) and which way you face (wedge) | the player's position and body direction | yes | yes |
| Destination | the waypoint chosen on the Stand Map | yes | yes |
| Planted juveniles | `ScenarioOneManager.PlantedJuveniles` (alive, not yet promoted), at their exact places | yes | no |
| Layers: Light, Regeneration | a translucent field, one colour per 5 m square from `ForestEcologyCell` | yes | no |
| Layers: Browsing, Marks | rings on the real shelters / the real marked trees | yes | no |
| Stand figures (trees, stems per ha, mean DBH, basal area) | computed from the trees on the map | yes | no |

### Not drawn, because the scenario does not have it

No contours, hillshade or elevation (the scene has no Terrain: the ground is level, and the overview says so). No water, wetland or rock (not represented; the site-condition classes are dormant). No separate vehicle track or footpath (the data is one path). No latitude / longitude (no real-world location). No individual work-area structures (they are the player's facilities; the clearing they stand in is drawn). The legend lists only what is on the map right now.

## How it is built

```
live game --StandMapSource--> StandMapModel (plain data, world metres, three version counters)
                                  |
          +-----------------------+------------------------+
          v                       v                        v
 StandMapCanopyRaster     StandMapDrawPlan            StandMapOverlayColours / StandMapLegend
 (forest picture)         (vector shapes: static      (layer colours, legend rows)
                           layer + dynamic layer)
          v                       v
 StandMapCanopyLayer      StandMapCanvas  --- both inside ---> StandMapSurface  (the full map and the minimap are one of these each)
```

* **One model, one transform, one symbol table.** `StandMapModel` holds the data; `StandMapTransform` is the one world-to-map mapping for the vector shapes, the labels and the 5 m squares (the forest picture applies the same origin and scale to its own texture pixels, in `StandMapCanopyRaster`, because a raster is south-up; the offline check and the live verification compare the picture with the transform, so the picture, the lines and the squares are checked not to drift apart); `StandMapSymbols` is the only place a symbol's colour, width, dash pattern and legend wording is defined. Mini and Full differ only in the symbol filter and the size.
* **Plain logic is separate from Unity.** The model, transform, symbols, grid, draw plan, forest raster, overlay colours and legend use no UnityEngine, so `Tools/Verification/StandMap/OfflineLogic` checks them on plain .NET.
* **The forest picture** (`StandMapCanopyRaster`) draws each crown from a cached "sprite" (a lit, textured crown of one size and species class), shortest tree first so a tall crown covers a short one. Shading, outline shape and texture variety come from a number made from the tree's id and carry no data. It is redrawn in slices (a few dozen trees a frame) when the *canopy* changes (a tree felled, died or grown, `CanopyVersion`), never because you walked, and never for a mark change.
* **Two vector layers** (`StandMapCanvas`): *static* (grid, rack corridors, path, rack lines, juveniles, shelters, marks, boundary, scale) is laid out when its content changes; *dynamic* (you, the destination, the rack being planned) is the only thing that changes while walking.
* **When the trees are re-read** (`StandMapSource`): at once when a tree is felled or dies (static events), a year passes or a mark changes; otherwise once a second as a safety net (a loaded game raises no event). Opening the full map finishes any redraw that is part-way.
* **The 5 m ecology squares** are still the unit of selection, waypoint and rack placement and the data behind Light and Regeneration. They are invisible buttons placed by the same transform and show only an outline when pointed at or selected. Pointing at the map prints the square and its figures under the map.
* **Style.** The map window and the minimap frame use a charcoal / slate palette scoped to `.modal-map` / `.hud-minimap` in `ScenarioOneUi.uss`; no other window changes.

## What changed for players and what did not

Changed: how the map looks; the map window is wider (three columns: the plan, an overview with legend, the square / rack card); square figures are read by pointing; the legend is data-driven.
Not changed: the rack-planning behaviour and the rack tool, the waypoint, the objectives, the Work Plan, saves (no schema change), the ecology and economy, the reference scenario, and the known debt that an off-layout rack is lost after a save/load or after harvest pruning.

## Verifying it

* Offline (no Unity): `python3 -I Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py`
* In the Editor (copy / run / remove, under the Unity lock; see `Tools/Verification/ScenarioOneProfessionalMap/README.md`): `ProfessionalMapVerification` (live data, picture placement, overlays, redraw counters, layout, input), `StandMapParity` (map / minimap parity and rack planning; adapted where noted in its header), `MapRedesignShots` (matched before / after screenshots) and `MenuShots`.
