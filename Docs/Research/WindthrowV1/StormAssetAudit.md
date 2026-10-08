# Storm asset audit (static)

**Status:** static inspection of committed files at `a8596df`. Nothing imported, edited or opened in Unity or Blender. Triangle counts come from each package's `Docs/Geometry_Counts.md`.

| Asset | Path | Technical state | Current usage | Appropriate authoritative state | Missing variants | Likely performance issue |
|---|---|---|---|---|---|---|
| **SS_WindthrowBase_Fresh_01** (root plate + broken basal stem, exposed wood) | `Art/ForestryGround/FBX/…_LOD0-2.fbx`; prefab `Prefabs/Forestry/Ground/SS_WindthrowBase_Fresh_01.prefab` | 3 LODs (1,928 / 1,080 / 548 tris); URP materials `Forest_RootPlate_*`; pivot at footprint centre, ground level; **no terrain pit**; no collider (README: supply a simple collider separately) | **Not spawned** ("implies windthrow/uprooting events"). Its soil texture is reused for the ground material | Tree with `mortalityCause == "windthrow"`, years 0–~3 since the storm | Size variants (scale by DBH works); left/right mirror (procedural) | Negligible (≈ 2k tris; ≤ 100 instances) |
| **SS_WindthrowBase_Weathered_01** (mossy) | same folder | 3 LODs (2,026 / 1,143 / 583); moss patches | Not spawned | windthrow, ~4+ years (or deadwood `DecayClass ≥ 2`) | — | Negligible |
| **SS_Log_Fresh_01** | `Art/SectionFive/Woodland_Assets/SS_Log_Fresh_01/`; catalog `SectionFiveVisualCatalog.freshLog` | 3 LODs (576 / 300 / 128); 3.11 × 0.64 × 0.45 m natural size | **Spawned** for every deadwood record (`ScenarioFallenLogVisual`), scaled to 0.8 × height (stretched ~5× for a 20 m tree) | Fallen-deadwood record, DecayClass 0–1 | A **long-stem** variant (12–25 m) or a segmentable piece; a fresh-break end | Cheap geometry; **texture stretching** along a 16–20 m scaled log is the visible issue |
| **SS_Log_Decayed_01** | same family | 3 LODs (1,368 / 681 / 280) | Spawned for DecayClass ≥ 2 | Decayed deadwood | as above | Cheap |
| Procedural cylinder log | `ScenarioOneManager.SpawnFallenLogVisual` fallback | Primitive with bark material | Non-Sitka or a missing catalog | Fallback only | — | — |
| **SS_Stump_01** | `Art/SectionFive/Woodland_Assets/SS_Stump_01/`; catalog `sitkaStump` | 3 LODs (444 / 308 / 224) | Felled-tree stump presentation | Felled (cut) stump | **Not suitable for windthrow** (cut face). A snapped stump/snag is missing | Cheap |
| **SS_Brash_Green/Dry_01/02** | `Art/ForestryGround/` | 3 LODs (5,868 / 5,004 tris at LOD0) | Compact brash beside felled stumps | Felling **and salvage** residue | — | Moderate if hundreds; salvage ≤ 100 → acceptable |
| **Sitka_RingBarked_01 Fresh / Dead** | `Art/SitkaRingBarked/` | 3 LODs (Fresh 44k/20k/6.9k; Dead 18.8k/4.1k/1.4k); 26 m mature tree | Not spawned (no ring-barking state) | Standing dead (ring-barked) | — | Dead LOD0 18.8k fine. **Not a windthrow asset**, but a reference for a future snag/standing-dead state |
| **Small_Deadwood_Scatter_01/02**, **Moss_Deadwood_Patch_01**, **Deadwood_*_Cluster_01** (fungi) | `Art/SectionFive/…` | Prefabs with LODs | Habitat dressing derived from deadwood records | Deadwood age/decay | — | Habitat dressing per log: watch counts if hundreds of logs |
| **Living Sitka displays** (Plantation02 snapshots, SitkaMatureBenchmark ~40–44k tris LOD0, CropTreePruning) | `Art/SitkaPlantation02/` (FBX snapshots), `Art/SitkaMatureBenchmark/` | 3 LODs; foliage cards with alpha clip | Every living tree | Living tree | **Fallen whole tree with crown** (green → grey): missing | Reusing a living model rotated 90° per victim: ~40k tris LOD0 each. Acceptable at LOD1/2 distances for ≤ 100 victims; see `PerformancePlan.md` |
| Disturbed ground / soil pit | — | **missing** | — | Root-plate pit for years 0–5 | decal or low mesh | Decals: negligible |
| Snapped stem / high stump (snag) | — | **missing** | — | `windsnap` (v1.1 only) | — | — |

**Wiring facts [REPO]:**

- `ForestryAssetSetup` builds root-plate prefabs (`SS_Windthrow*` in its prop list; line 641 special-cases them).
- `ScenarioFallenLogVisual` scales a log to the record's height × 0.8 and DBH, and tints by decay class through a `MaterialPropertyBlock`.
- Logs are placed **centred on the stem position** with a hashed random heading (`SpawnFallenLogVisual`). Windthrow needs a **base-anchored**, storm-directed placement.
