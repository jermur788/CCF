# Current wind system audit

**Base:** `origin/main` @ `a8596df9c52669a36709f09e85dfe6568640af49` (Growth Model 1 integrated, D-048; save v17). Static inspection only; no Unity run.
**Labels:** [REPO] inspected code/data · [HIST] git history · [PROTO] offline prototype in `Tools/Verification/WindthrowV1/` · [I] inference.

## 1. Summary

1. **There is no windthrow in the simulation.** No storm event, no wind mortality, no wind-damaged timber and no spawned windthrow visuals exist [REPO].
2. **The only wind quantity is a diagnostic index**, `ForestEcologyController.GetWindRisk` (`:1625`). The code describes it as "calibration, not an annual mortality probability".
3. **The player-facing label is meaningless in the current stand.** Every Scenario One tree reads "Wind exposure: high", because the band thresholds (5/12) were calibrated on a different starting stand one day before the plantation stand replaced it, and were never revisited [HIST].
4. **Useful building blocks already exist:**
   - the mortality API, with a cause and year;
   - the death → fallen-deadwood record path (Growth Model 1);
   - saved per-cell recent-opening state with decay;
   - batched canopy rebuilds;
   - a `WindDamage` timber-quality flag that downgrades logs to pulp;
   - per-task harvest cost multipliers;
   - two authored root-plate assets.

## 2. Implemented calculation [REPO]

```
GetWindRisk(tree) = StandWindSusceptibility            (Sitka asset: 1)
                  × slenderness   = H / (DBH / 100)    (H/D, dimensionless)
                  × opening       = 0.25 + 0.75 × cell.Light
                  × recent        = 1 + WindOpeningWeight (1) × cell.RecentOpening
```

| Input | Source | Notes |
|---|---|---|
| H, DBH | `ForestTree` | Growth Model 1 height follows the Irish Class III envelope; DBH responds to Hegyi competition |
| `cell.Light` | `RecomputeCanopy` gap product, 5 m cells | **Ground** light, a proxy for openness, not wind speed. The `0.25` floor makes unthinned stands "not risk-free" [D] |
| `cell.RecentOpening` | `OnTreeFelled` (`:370`): +1 per felled tree in the cell, capped at `maxRecentOpeningPerCell` = 2. `DecayRecentOpening` (`:1617`, annual step 12): × 0.5^(1/3) per year, a 3-year half-life (`windThinningHalfLifeYears`) | **Saved** (`ForestCellSaveData.recentOpening`). **Not incremented by biological mortality** (`OnTreeMortality` `:377`). Also feeds establishment suitability, understorey colonisation, the Annual Review "Opened cells" line and the snapshot `totalRecentOpening` |
| `StandWindSusceptibility`, `WindOpeningWeight` | `TreeSpeciesDefinition` (both 1) | Tooltip: "The Irish empirical model is stand-level and is not used as annual individual mortality" |
| Site / soil | `ForestEcologyCell.SoilStability` = 1, `SiteProductivity` = 1 | **Not saved and not varied.** `SoilStability` is used only by provisional understorey colonisation. Not used by wind |
| Stand top height | — | **Not used.** Neither is relative height (dominance) |
| Edge / boundary | — | No term. The isolated 40 m stand has no surroundings (see `BoundaryDecision.md`) |

Consumers:

| Consumer | Use |
|---|---|
| `TreeInspectionView:62` | "Wind exposure: low / moderate / high" |
| `ForestTreeMarkingManager.UpdateTreatmentOutcome` | "wind peak x (band)" on the marking forecast |
| `ForestPlayer` IMGUI card (non-Scenario modes) | "Wind vulnerability" |
| `MaxWindRisk`, `LogSummary`, `StandDiagnostics` | logs only |

## 3. UI thresholds [REPO][HIST]

`WindRiskBandLabel`: low < `windLabelLowAt` (5), moderate < `windLabelHighAt` (12), high otherwise. Both values are serialized in `Assets/Scenes/ForestTest.unity` (lines 5784–5785).

Tooltip (`ForestEcologyController.cs:19`): *"Calibrated so an unthinned control stand (max ~7.8–8.6) reads moderate and a concentrated opening reads high."*

History:

| Commit | Date | Event |
|---|---|---|
| `6a726f0` | 2026-09-14 | `GetWindRisk` formula introduced (unchanged since) |
| `1cc312c` | 2026-09-14 | Bands 5/12 calibrated against the stand of that time: the original 68-tree demo scene with varied, stouter trees |
| `1a7e239` | 2026-09-15 | **"Rebuild the starting stand as a first-thinning Sitka plantation"**: 336 stems, age 20, H/D 69–85 |
| `b3766ca` | 2026-10-02 | k10a10 light and C8 competition calibration; wind bands untouched |
| `2cfbabc` | 2026-10-07 | Growth Model 1: H/D rises to about 83–86 at age 30–40; wind bands untouched |

## 4. Why every tree reads "high"

At Year 0 the plantation has H/D median 75 (range 69–85) and cell light about 0.05 [REPO; pedagogy harness `3e4ee40`]:

```
risk ≈ 1 × 75 × (0.25 + 0.75 × 0.05) × 1 ≈ 21.6   ≥ 12 → "high"
```

- Even the stoutest tree (H/D 69) in full shade scores about 17. **The index cannot fall below the "high" threshold** anywhere in this stand. The minimum possible value for an H/D-69 tree is 69 × 0.25 = 17.3.
- The index has **no height term**, so a 14.6 m stand at age 20 scores almost the same as a 24 m stand at age 40 [PROTO: about 21 vs 24].
- Under Growth Model 1, H/D peaks at about 83–86 (age 30–40, Scenario Years 10–20), so the index *rises*. It stays "high" for every tree until well past age 100 [PROTO stand trend: 20.8 → 24.1 → 12.5 at age 120].

**Three things are separated here:**

| Layer | What it is | Status |
|---|---|---|
| Implemented calculation | A dimensionless product of H/D, openness and recent opening | Works as coded, but has the wrong shape for windthrow: no height, no dominance, no acclimation of established edges [PROTO: 7/11 required orderings] |
| UI thresholds | Bands 5/12, calibrated for a stand that no longer exists | Stale; saturated at "high" |
| Actual causal windthrow | None | Not implemented (D-044 defers storms/windthrow) |

## 5. Mortality and deadwood path (relevant for windthrow outcome)

- `ForestTree.ApplyMortality(cause, year)` (`:534`) sets `biologicallyDead`, cause and year; clears marks; **deactivates the tree GameObject**; raises `MortalityApplied`. It is idempotent. Saved in `TreeSaveData` (`biologicallyDead`, `mortalityCause`, `mortalityYear`).
- `ForestEcologyController.OnTreeMortality` removes the tree's competition, growth and seed entries and queues a canopy/seed rebuild (batched inside `BeginChangeBatch`/`EndChangeBatch`). It does **not** add `RecentOpening`.
- `ScenarioOneManager.OnTreeBiologicalDeath` (growth model ≥ 1): creates **one `ScenarioDeadwoodRecord`** (volume from DBH, height and form factor) and spawns the fallen-log visual via `SpawnFallenLogVisual`. Records decay 3 %/yr to a 12 % floor [D]. **The record has no cause field**; the cause is recoverable through `treeId` → tree save data.
- Growth Model 1 self-thinning (`ApplyAdultDensityMortality`, annual step 4b) already uses this path in a batch. It is the template for storm victims.

## 6. Felling, economy and timber quality hooks

- **Felling outcomes** (`FellingMaterialOutcome`): SellAndExtract / KeepForUse / RetainAsFallenDeadwood. `ResolveFelling` creates the deadwood record for retained stems.
- **Quoting:** `ScenarioOneEconomyAdapter.QuoteHarvest` accepts only **living, choppable** trees. Salvage of dead stems needs a small extension (see `SalvageDesign.md`).
- **`StemQualityFlags.WindDamage`** exists in `TimberYieldDomain`. The Sitka configuration excludes it from sawlog, pallet and stake, leaving pulp only. Quality can be applied to **stem sections** (`QualitySections`). It is never set today.
- **`HarvestOperationContext`** (First/Second/Later thinning, Clearfell) prices harvesting per tonne. `ForestryTask.SiteCostBasisPoints` (default 10000 = 1.0) is an existing cost multiplier.
- **Contractor minimum** €2,500 per commissioned harvest job (`minimumHarvestJobCents`).

## 7. Determinism and RNG [REPO]

`SimulationRandom.Roll(model, id, year, seed)` gives an order-independent uniform draw per string id. Model 1 uses splitmix64. Adult mortality already uses `Roll(rngModel, "ADULT-MORT-" + treeId, year, seed)`. String-id rolls do not consume the shared annual `System.Random` stream, so **adding new id-keyed rolls does not perturb existing draws**.

## 8. Assets (detail in `StormAssetAudit.md`)

`SS_WindthrowBase_Fresh_01` and `_Weathered_01` root-plate prefabs exist, with LOD groups, about 2k triangles at LOD0, **not spawned** ("implies windthrow/uprooting events"). Fallen logs (`SS_Log_Fresh_01`, `SS_Log_Decayed_01`, 576/1,368 triangles at LOD0) are spawned for deadwood records. No fallen whole tree with a crown, and no snapped stem.
