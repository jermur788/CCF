# Scenario One stand expansion — Phase 1 read-only architecture audit

Task branch `task/scenario-one-80m-stand`, worktree `/home/jer/CCF-s1-80m`, BASE `11c3596072a0422d5426291e370129d209c56342`, context `8bed3996aefd1780c62744b648094efe5b394feb`. Audit date 2026-10-10 (Europe/Dublin). **No production file was modified.** Only this audit, a disposable measurement harness (`Tools/Verification/StandExpansion/`) and its evidence were added.

## 0. Result

**STOP — decision required.** The accepted ≥ 80 × 80 m expansion cannot be implemented inside the packet's decision rule, because the audit found that it needs four things the packet reserves for the Manager:

1. **A persisted stand-geometry identity (save-schema change).** Every cell-indexed record is geometry-dependent, the validator does not check geometry, and Reference Future v1 loads into an 80 m grid *successfully but wrongly* (section 6).
2. **Economy/objective recalibration decisions.** Several absolute values change meaning at four times the area; the €2,500 minimum harvest-job fee in particular changes the first-thinning lesson (section 7).
3. **New current-world anchors, with legacy anchors kept alive.** P2/P3 and most regression fixtures assert the 336-tree world, so their hashes necessarily change; keeping them valid needs a legacy-geometry replay switch, which is the same versioning work (section 8).
4. **A performance decision.** The density-preserving 80 m stand is playable but costs about 8–9× per annual step and about 56 % more frame time in the Editor (section 5). The packet forbids lowering stocking to hide this, so the options are listed separately.

A bounded, explicit design that satisfies all four is proposed in section 9 for approval. Nothing was implemented.

## 1. Method and limits

- Read-only inspection of code, the committed `ForestTest` scene YAML and saved data structures.
- One disposable harness, `Tools/Verification/StandExpansion/StandExpansionAudit.cs` (staged into `Assets/ForestPrototype` by `run_stand_audit.py`, then removed with its `.meta`), run interactively in the Unity 6000.6.0f1 Editor. It measures the current 40 m scene, then builds a **density-preserving 80 m candidate in play mode only** (nothing saved; the scene file is not written) by re-using the production `ForestStartingStand` generator with new serialized values (lattice 41, 1.9 m spacing, 1,344 stems) and the production ecology grid at 80 m. The candidate is a **benchmark, not the implementation**.
- All timings are **Unity Editor play mode on Linux** (i5-6300HQ, GTX 980M, 15.5 GB), 1280 × 720, with the Editor open. They are not Player, Wine or native Windows numbers and make no such claim. Run-to-run variation is roughly ±10 %.
- Evidence: `Evidence/` (summary JSONs, per-world tree CSVs, renders). Three harness runs are kept (`run1` with rendered frame times; `run2` annual-step split; `run3` adds compatibility probes).

## 2. World / scene geometry (committed `ForestTest.unity`)

| Item | Current value |
|---|---|
| Ground | one box, 40 × 1 × 40 m at (0, −0.5, 0) |
| Property boundary | four ridge boxes 5 m high: North/South at z = ±20 (42 × 5 × 1), East/West at x = ±20 (1 × 5 × 40) |
| Player start | (−0.43, 0.1, 5.25); camera far plane 120 m in the scene builder (scene value to be confirmed at implementation) |
| Path | 52 "Dirt Path" planks (2.6 × 0.01 × ~0.65) from (−0.93, −14.75) to (−2.71, 10.75) |
| Work clearing | "Forest Clearing" 9 × 9 m at (−2.77, 0.01, 11.0) |
| Hidden construction sites (fac7820) | seven `ForestBuildable` roots + Plank Rack within about x −7…2.5, z 9…13; hidden and non-solid, but their positions still reserve a 2.2 m exclusion radius in the starting-stand generator (and a 3 m exclusion in the Reference-preview player-position search) |
| Environmental detail | "Nature Detail": 16 pack-asset mushroom patches placed within ±19 m (missing in this pack-less worktree; decorative) |
| 40 m-tied code | `ForestEcologyController.standSizeMeters = 40`, `ForestStartingStand` width/depth 40 (both **serialized in the scene**), `ScenarioOneManager.MoveToReferenceView` sweeps x, z ∈ [−17, 17] (Reference preview only), `ForestSceneBuilder` (Editor) still authors 40 m ground, 2 m ridges (the live scene already has 5 m, so the builder has drifted) and the legacy construction sites |

Nothing in the scene uses a navmesh or terrain; ground is a flat box, so an 80 m ground and boundary are a scale/position change of five objects. `ForestSceneBuilder.Create` must **not** be run to do this.

## 3. Starting stand (production generator, deterministic)

A 21 × 21 planting lattice at 1.9 m spacing (span ±19 m), per-slot jitter hashed from the tree ID `P{row:D2}{col:D2}`, 36 slots omitted for road/clearing/player/sites (2.2 m, road 1.5 m), then a hash-ranked "mortality" budget of 69 omits to land exactly on `targetTreeCount = 336`. DBH classes come from a smooth vigour field plus occupancy-neighbour adjustment; all trees are age 20.

| Current 40 m stand | |
|---|---|
| Living trees / area / density | 336 / 0.16 ha / 2,100 stems ha⁻¹ |
| DBH cm (p10 / p50 / p90; mean) | 12.0 / 15.5 / 19.0; 15.59 (range 9.6–21.1) |
| Height m (p10 / p50 / p90; mean) | 9.5 / 11.6 / 13.6; 11.65 |
| Mean Hegyi competition index | 8.24 |
| Trees within 0–5 m / 5–10 m / > 10 m of the property edge | **155 (46.1 %) / 109 (32.4 %) / 72 (21.4 %)** |
| Trees within the 8 m competition cutoff of an edge | 224 (66.7 %); true interior (> 8 m) 112 (33.3 %) |
| Area share beyond 5 / 8 / 10 m from an edge | 56.3 % / 36.0 % / 25.0 % |

So the 40 m stand is edge-dominated: two thirds of its trees have part of their competition neighbourhood outside the property, as the Manager's rationale states.

**Re-parameterising the generator does not preserve the central stand.** In the 80 m candidate, **none** of the 336 current tree positions reappears and only 296 trees fall inside the old ±19.4 m footprint, because the IDs (and therefore the ID-hashed jitter and mortality ranking) change with the lattice. Preserving the central authored area and its IDs needs a deliberate design (section 9). The lattice itself does contain every old lattice point (same 1.9 m spacing; −19 is a lattice coordinate of the larger lattice).

## 4. Ecology grid

- `standSizeMeters` 40, `cellSizeMeters` 5, grid centred on the world origin, `cellsPerAxis = ceil(40/5) = 8`, index = `z * cellsPerAxis + x` (south-west origin), 64 cells. At 80 m the same code gives 16 × 16 = 256 cells; index 9 means a different place.
- Canopy/light: `RecomputeCanopy` is **cells × trees** (each crown shades cells out to crown radius + half a cell). Seed rain: **cells × trees** per enabled species with the Sitka path first. Both call `tree.transform.position` (a native call) inside the inner loop.
- Hegyi competition: all-pairs `N × (N − 1)` loop with a squared-distance early-out at the 8 m cutoff (already optimised once, "spatial pass v1").
- Model 2 understorey (bramble/bracken cover per cell), local clearance patches, planted juveniles, deadwood records, management events, work orders and browsing shelters: generic over `CellCount`/`CellsPerAxis`/`StandBounds`, but **five of these persist cell indices** (section 6).
- Random domains: no stream is keyed by cell index. Model 0 consumes one stream sequentially in cell-iteration order for promotion offsets, so replaying legacy runs requires the legacy grid.
- Cell labels (`UiKit.CellLabel`) are `A`–`Z` plus a row number, so 16 columns work.

| Cell light by distance of cell centre to edge (year 0) | 40 m | 80 m |
|---|---|---|
| 0–5 m: cells / mean light | 28 / 0.066 | 60 / 0.013 |
| 5–10 m | 20 / 0.006 | 52 / 0.005 |
| > 10 m | 16 / 0.072 | 144 / 0.012 |

At year 0 every cell is under a near-closed canopy (canopy 0.93–0.995), so the edge shows up in **competition and neighbourhood geometry, not in light**: the 40 m interior light values are lifted by the central road and clearing, not by an edge effect. Mean Hegyi index by tree band: 40 m 7.37 / 9.08 / 8.85 (0–5 / 5–10 / > 10 m, with only 72 interior trees); 80 m 6.55 / 9.02 / 9.67, i.e. edge trees carry about 32 % less competition than interior trees at 80 m (17 % at 40 m).

## 5. Competition and performance (Editor, ms; density-preserving candidate)

| | Current 40 m | 80 m, 2,100 stems ha⁻¹ | Ratio |
|---|---|---|---|
| Living trees / cells | 336 / 64 | 1,344 / 256 | 4× / 4× |
| Competition pair iterations N(N−1) | 112,560 | 1,804,992 | 16× |
| Ordered pairs within 8 m (mean neighbours) | 11,648 (34.7) | 51,450 (38.3) | 4.4× |
| Canopy/seed inner iterations (cells × trees) | 21,504 | 344,064 | 16× |
| `RecomputeCanopy` (median) | 10.7 | 205.6 | 19× |
| `RecomputeSeedRain` | 4.5 | 58.5 | 13× |
| Hegyi pass | 2.1 | 19.0 | 9× |
| Ecology-only annual step (`AdvanceOneYear`, 3 runs) | 114–124 | 952–1,020 | ~8× |
| Full manager step (`AdvanceYear`, steady state) | 189–191 | 1,613–1,805 | ~9× |
| First three manager steps in a session (year 1 / thinning year / year 3) | 304 / 356 / 247 | 2,322 / 2,626 / 1,507 | 7.6–8.6× |
| Canopy rebuilds per annual step | 2 | 2 | — |
| Start-of-game generation (candidate `Generate`, incl. canopy and seed rain) | not separately timed for the current scene | 2,344 | — |
| Rendered frame, median / p95 (four headings, 1280 × 720) | 22.96 / 34.6 (43.6 fps) | 35.87 / 47.3 (27.9 fps) | +56 % |
| Process RSS before / after the scripted years | 4,564 → 4,618 MB | 4,713 → 4,880 MB | +149 MB at start |
| Unity allocated before / after | 1,824 → 1,866 MB | 1,924 → 2,062 MB | +99 MB at start |

Reading: the 16× growth in cells × trees and in pair iterations is the whole story. Canopy and seed rain account for about 0.47 s of the roughly 1.0 s ecology-only step at 80 m; the remaining ~0.5 s of the ecology-only step is other per-tree and per-cell ecology (growth, crown relaxation, regeneration, promotion; not separately profiled), and about 0.65–0.8 s more is manager-side work (annual snapshot, habitat rebuild for 256 cells, economy and report; also not separately profiled). Frame time is the other cost: 1,344 LOD'd spruces cost about 13 ms more per frame in the Editor.

Prior art for scale: the storm-integration performance gate measured 336 / 1,300 / 5,000 *logic-only* trees in the fixed 64-cell grid at 62 / 313 / 1,745 ms; it does not capture the cells × trees growth (256 cells) or rendering that this audit measured.

**Is it acceptable?** That is a Manager judgement. A 1.6–2.6 s Editor pause on "advance year" and about 28 fps at the start camera are playable but noticeably worse than today, and no Player or native Windows measurement is possible here. Per the packet, stocking was **not** reduced to hide this. Simplest behaviour-preserving options, none implemented or measured as changes:

1. Cache per-tree position, height and crown radius once per rebuild and early-out beyond reach in the canopy and seed-rain loops (the same technique already used for Hegyi, which removed ~113k interop calls). Both loops currently call `tree.transform.position` inside the cell × tree inner loop. Bit-exactness against the existing anchors would have to be proven, as it was for Hegyi.
2. Bucket cells by position so a crown or seed source only visits cells inside its reach (reach is a few metres against an 80 m stand).
3. Avoid the second canopy rebuild when nothing changed between growth and mortality.
4. For frame time: LOD bias or culling distance for far trees, tested in a Player build.

## 6. Save / load / Reference Future

**Where cell indices are persisted:** `ForestSaveData.cells[].index`, `scenarioOne.understoreyCells[].cellIndex` (validated as the exact sequence `0…cellCount−1`), `PlantedJuvenileSaveData.cellIndex`, `ScenarioDeadwoodRecord.cellIndex`, `ScenarioManagementEvent.cellIndex` and work orders' `cellIndex`. **No geometry identity is saved**; the current save version is 19.

**Observed behaviour of the current production code against an 80 m grid** (harness `CompatibilityProbes`, `Evidence/run3/audit-summary.json`):

| Probe | Result |
|---|---|
| A genuine 40 m v19 Model-2 save validated against 256 cells | **Rejected**: "model 2 competitor grid is missing or incomplete" (64 understorey records ≠ 256). Safe, but every existing save becomes unloadable |
| The same world presented as a pre-Model-2 (v15, model 0) save | **Accepted** by validation (only `index < cellCount` is checked) and loaded: indices silently refer to different places |
| `ScenarioReferenceArchive.Matches(definition, ecology)` | **true**: it checks scenario ID, definition version, save version and seed, never geometry |
| `TryBeginReferencePreview(100)` in the 80 m grid | **Opened successfully** ("Scenario management state loaded.") with 256 live cells against an archive of 64 (max index 63). The archive's heaviest-regeneration cell, index 1, sits at (−12.5, −17.5) in the 40 m grid and lands at **(−32.5, −37.5)** in the 80 m grid: 24 m away, in the corner outside the archive's ±20 m forest |

So an unversioned geometry change would make Reference Future v1 display a wrong forest without any error, and would silently misplace the ecology of old model 0/1 saves. The archive also pins `definitionVersion == "scenario-one-v12"` and accepts live v12 or v13, so **`definitionVersion` must not be bumped** without also changing the archive contract.

Is a new save/schema/definition/geometry version required? **Yes**: there is no way to tell a 40 m save from an 80 m save without a field, and inferring geometry from record counts or maximum index would be "smuggling geometry into an existing version", which the packet forbids.

## 7. Gameplay, economy and objectives

Measured with a proportional first thinning (14.3 % of trees marked: 48 at 40 m, 192 at 80 m), real `ScenarioOneManager` path (`Evidence/run1`):

| | 40 m | 80 m |
|---|---|---|
| Fell marks → work orders → approved | 48 → 48 → yes | 192 → 192 → yes |
| Contractor cost charged | €2,500.00 (minimum job fee) | €2,500.00 (minimum job fee) |
| Underlying labour cost (fee − "minimum adjustment") | €119.47 | €494.13 |
| Timber revenue / volume | €283.41 / 6.39 m³ | €1,185.02 / 26.44 m³ |
| Cash after the thinning year (start €12,000) | €9,783.41 (−€2,216.59) | €10,685.02 (−€1,314.98) |
| Fee ÷ revenue | 8.8× | 2.1× |
| Owner minutes used | 0 of 2,400 | 0 of 2,400 |

Absolute values in `ScenarioOneDefinition` (v13) and what a 4× area does to them:

| Value | Setting | At 336 trees / 64 cells | At 1,344 trees / 256 cells |
|---|---|---|---|
| `minimumRetainedOriginalTrees` | 60 | 17.9 % of the stand | 4.5 % (nearly trivial) |
| `minimumRegenerationCells` | 3 | 4.7 % of cells | 1.2 % |
| `referenceOriginalTrees` (century target) | 120 | 35.7 % | 8.9 % |
| `referenceRegenerationCells` | 12 | 18.8 % | 4.7 % |
| `referenceBroadleafPresence` | 10 | absolute count | scale unclear |
| `minimumDeadwoodVolumeM3` / `referenceDeadwoodVolumeM3` | 0.02 / 0.5 | absolute volume | per-area meaning halves |
| `ownerMinutesPerYear` | 2,400 (40 h) | marking/planting work scales with area | 4× the work in the same hours |
| `startingCashCents` / `minimumHarvestJobCents` | €12,000 / €2,500 | fee binds below ~€2,500 of labour in both worlds | see table above |
| `minimumMeanCanopy` 0.35, `referenceMeanCanopy` 0.65 | fractions | area-independent | area-independent |

These are product/calibration decisions, not implementation details, and the packet says to record the consequence and stop. The most important is the minimum job fee: at 40 m a small thinning is punished 8.8× its revenue, which is a teaching point of the current cash lesson; at 80 m the same proportional thinning is punished 2.1×. Workload also changes: a player marking 14 % of the stand marks 192 trees instead of 48.

Navigation arithmetic (to be confirmed in the playthrough, not a conclusion): walk speed 4 m/s, run 7 m/s, so crossing 80 m is about 20 s walking (about 11 s running) against 10 s (6 s) at 40 m; the diagonal is about 28 s walking.

## 8. UI and navigation

- **Stand Map:** builds `CellsPerAxis²` buttons; cell size = clamp(640 / n, 30, 80) px, so 16 columns are 40 px wide (a 640 px grid beside the 330 px side panel in the 1600 × 900 reference panel) against 80 px now. On paper it fits; text legibility of cell values ("none", "P1 1.2m") at 40 px, and the cost of rebuilding 256 buttons per refresh, are **not yet measured**. Labels `A–P` × `1–16` work.
- **Waypoints and selected cell:** generic over cells; arrival radius is half a cell (2.5 m). Inspection, Work Plan (`ResidualStand`, `WorkPlanOverview`), Annual Review and objectives read `CellsPerAxis`, `CellSizeMeters` and `StandAreaHectares` generically; per-area quantities scale automatically.
- **Hard-coded 40 m:** `MoveToReferenceView` (preview camera), the stale `ForestSceneBuilder`, and the scene's serialized `standSizeMeters`/starting-stand values.
- Habitat visuals merge into eight combined meshes, so draw calls stay flat; vertex counts and rebuild cost scale with cells.

## 9. Decision-rule evaluation and the smallest explicit versioning design

| Packet rule | Finding |
|---|---|
| Save-schema change | **Required** (geometry identity) |
| Model-version change | Required in the repo's established form (a new model version field) |
| Scenario One economy recalibration | **Decision needed** (section 7) |
| Objective/completion recalibration | **Decision needed** (section 7) |
| New ecological coefficient | None needed |
| Reference Future reinterpretation | Not required if geometry is versioned; **unavoidable if it is not** |
| Substantial performance architecture rewrite | Not required; behaviour-preserving loop optimisation is optional and separate |

**Proposed smallest design, following the existing model-version precedent (D-046/047/048/049/050): `standGeometryModel`.**

- Save v20 adds one root field `standGeometryModel`: **0 = Legacy40** (today's 40 × 40 m, 5 m cells, 8 × 8, 21-lattice 336-tree start, current ground/ridges), **1 = Enlarged** (≥ 80 × 80 m). A missing field and every save ≤ v19 mean 0, with no automatic migration (same convention as `growthModel`). **New Scenario One games use 1.** Reference Future v1 is 0 and is never reinterpreted.
- `ForestEcologyController` gets a `StandGeometryModelVersion` property that sets `standSizeMeters` and rebuilds the grid. `LoadData` applies the saved model **before** validation and cell restore, and validation checks every cell-indexed record against that model's cell count (closing the silent-misload hole for legacy saves). Loading an old save therefore switches the world to the 40 m geometry it was made in (ground/ridge objects and the start-stand parameter set follow the model), so old saves and Reference Future preview/continuation stay exactly valid.
- `ScenarioReferenceArchive.Matches` additionally requires model 0; preview/continuation switch to model 0 and restore model 1 on exit. `definitionVersion` stays `scenario-one-v13`.
- The generator gets two parameter sets: **Legacy40 = exactly today's values** (336 trees, identical IDs/positions), **Enlarged** = new. To meet the packet's preference of preserving the central authored area, the Enlarged set can generate the current 336-tree central block unchanged and then fill the outer ring at the same stocking (about 1,008 more stems, 1,344 total); edge trees of the old block will then correctly gain real neighbours, so their neighbour-count DBH adjustment may change. This is a design choice for the Manager.
- A replay switch `CCF_STAND_GEOMETRY=0` (precedent: `CCF_RNG_MODEL`, `CCF_REGEN_MODEL`, `CCF_GROWTH_MODEL`) lets every existing gate, P2, P3 and the 24-gate regression run on Legacy40 **with unchanged anchors**. New gates and explicitly labelled enlarged-stand anchors cover model 1. No historical anchor is overwritten.
- Scene work is small: five ground/boundary objects become model-dependent (a small component applying the model's ground scale and ridge positions), plus the starting-stand and ecology serialized values; hidden construction sites stay hidden.

## 10. Options for the Manager

**A. Versioning (recommended): `standGeometryModel` as above.** Honours every save/Reference guarantee and keeps all legacy anchors. Alternatives, not recommended: (B) replace 40 m outright, which breaks the immutable Reference contract and invalidates all saves; (C) a separate "Scenario One Enlarged" scene/scenario ID, which avoids a field but needs scene switching on load and duplicates the scene.

**D. Economy and objectives:** (i) keep every absolute value and accept the consequences in section 7; (ii) scale selected values with area in a dedicated, separately verified calibration packet (candidates: minimum harvest-job fee, retained-originals, regeneration cells, reference targets, owner minutes); (iii) per-geometry values in the definition. Do not do this implicitly inside the implementation task.

**P. Performance:** (i) accept the measured cost; (ii) authorise the behaviour-preserving loop optimisation first (section 5, options 1–3), proven bit-exact against the existing anchors, then re-measure; (iii) Player/Windows measurement before deciding, since the figures here are Editor-only.

**Suggested sequencing for the next packet:** (1) geometry versioning and the `CCF_STAND_GEOMETRY=0` replay with **zero behaviour change** (all 24 gates, P2, P3 and the three compatibility hashes unchanged); (2) Enlarged parameter set, scene objects and generator; (3) validation, new labelled anchors, map/waypoint playthrough and Player/Windows performance; (4) calibration decisions D and P applied only as approved.

## 11. Not done / not verified

No production change; no stand implemented; no first-cycle playthrough in an implemented enlarged stand; no Stand Map render at 16 × 16; no Player, Wine or native Windows measurement; economics measured for one proportional thinning only (no planting/pruning/regeneration-removal owner-work case); the Nature Detail mushrooms (pack assets) and the stale `ForestSceneBuilder` were not touched; P2/P3 hashes were not re-baselined (they cannot be while the current world is unchanged, and would necessarily change if it were enlarged without the replay switch).
