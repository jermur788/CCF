# Multi Stage 4 — Adult Sitka parity, natural mortality and fallen deadwood (2D engine)

**Status: APPROVED — owner decisions D-095 (2026-11-23).** SPEC REQUIRED: extends adult growth, mortality and Work Plan semantics. Implementation is limited to `Tools/MultiRackSitkaP1Lab/src/Core` and its tests. No Unity, `/multi` design, save or P0 change.

**Base:** `4e60c8ab806df56441497b682ccf783badc6e0ea` (Stage 1–3 branch; not merged to `main`).

## 1. Verified Unity source facts (read-only audit, 2026-11-23)

From `Assets/ForestPrototype/SitkaGrowthModel.cs`:

```text
RelativeDensity(N/ha, Dq_cm) = N/ha * (Dq / 25)^2.063 / 1868
DensityPressure(RD)          = max(0, (RD - 0.6) / (1 - 0.6))
Suppression S                = 1 - 1 / (1 + CI / Ci50)
Vulnerability V(S)           = clamp01(S)^4
AnnualDeathProbability       = min(0.5, 0.08 * P^2 * V / mean(V))
Boundary                     = while RD(survivors) > 1.0: kill the most suppressed survivor
```

From `ForestEcologyController.cs`:

- Annual order: competition → pre-growth canopy/light → `GrowAdults` → `RelaxCrowns` → `ApplyAdultDensityMortality` → post-growth canopy/light.
- `GrowAdults`: `dbhPotential = 1.2 * site * clamp01(1 - DBH/120)`; `dbhGrowth = dbhPotential / (1 + CI/Ci50)`; height growth for Sitka is `NextHeight(h, age) - h` (site-class envelope), not the legacy potential-height law; age +1 after growth.
- Mortality uses the start-of-year competition index for suppression, post-growth DBH for RD, and rolls `SimulationRandom.Roll(rngModelVersion, "ADULT-MORT-" + TreeId, year, seed)`.
- Boundary victim order: suppression descending, tie by TreeId ascending; each removal decrements the survivor count and subtracts DBH².
- `RelaxCrowns`: `target = r_pot(DBH) * (1/(1+CI/Ci50))`; if `PruningLifts > 0`, `target *= 0.92^lifts`; relax at 0.15/yr, floor 0.1.

From `ForestTree.cs`:

- `MaxPruningLifts = 3`; `CrownRadiusReductionPerLift = 0.08`; `MinimumYearsBetweenLifts = 5`.
- `PrunedCrownTargetFactor = 0.92^lifts`.
- `TryPrune`: target must exceed the current crown base and stay below 60 % of height; immediate `r = max(0.1, r * 0.92)`; records the year. Eligibility is a living, choppable tree (mature/young), not a stump.
- `ApplyMortality`: idempotent; stores cause and year; hides the living representation; fires one event per death. It is not a felling and produces no timber.

From `SitkaSpruce.asset`:

- `potentialDbhGrowthCmPerYear 1.2`, `maxDbhCm 120`, `ci50 5`, `crownRadiusIntercept 0.9415`, `crownRadiusPerCmDbh 0.07635`, `crownRelaxationPerYear 0.15`, `formHeightRatio 0.5`.
- **No `growthAgePhases` are authored.** `HasGrowthAgePhases` is false, so the age-phase seam is inert for Sitka. It must exist and be testable with a synthetic non-empty table, but production Sitka uses 1.0/1.0.

From `ScenarioOneManager.OnTreeBiologicalDeath`: one deadwood record per biological death, with tree ID, species, world position, original volume/height/DBH, fallen year; felled stumps are not biological deaths.

## 2. Approved Stage 4 model

### 2.1 Versioning and defaults

- `AdultModelVersion { Legacy = 0, ConnectedWoodland = 1 }`
- `MortalityModelVersion { Legacy = 0, RecordedDeadwood = 1 }`
- Stage 4 behaviours require **all three** of `AdultModelVersion = 1`, `MortalityModelVersion = 1`, `CrownModelVersion = 1`.
- Every existing caller and `/multi` default stays on `AdultModelVersion 0`, `MortalityModelVersion 0`, `CrownModelVersion 0`. Existing Stage 1–3 model 1 remains reproducible as `AdultModelVersion 0` + `CrownModelVersion 1`.
- Model 0 fingerprints (`7e3a2a86…`, `a692a6d5…`) and the P0 pin (`9a99f781…`) remain byte-identical. New per-tree fields must not enter the model-0 fingerprint serialization.

### 2.2 Annual order (Stage 4)

1. Compute **woodland-wide** 8 m Hegyi CI over all living trees of both racks, ordinal TreeId order. Compute each rack's own CI for mortality suppression from the same start-of-year set.
2. Pre-growth canopy/light state is valid if queried.
3. **Grow adults:**
   - `dbhPotential = 1.2 * SiteProductivity(x) * clamp01(1 - DBH/120)`
   - `dbhGrowth = dbhPotential / (1 + CI_woodland / 5)`; DBH clamped `[1, 200]`
   - `heightGrowth = (NextHeight(h, age) - h) * SiteProductivity(x)`; age +1
   - apply `ApplyGrowthAgeFactors` seam (Sitka factors are 1.0; the seam is verified with a synthetic table)
4. **Relax crowns** (CrownModel 1): `r_target = max(0.1, (r_pot / sqrt(1 + CI_woodland/5)) * 0.92^lifts)`; `r += 0.15 * (r_target - r)`; floor 0.1.
5. **Per-rack adult density mortality:** for each rack alone, using its living Sitka stems and its exact polygon area in hectares:
   - `RD = N/ha * (Dq/25)^2.063 / 1868`, `P = max(0, (RD-0.6)/0.4)`
   - `S_i = 1 - 1/(1 + CI_rack,i/5)`, `V_i = S_i^4`, `p_i = min(0.5, 0.08 * P^2 * V_i / mean(V))`
   - roll `SimulationRandom.Roll(rngModelVersion, "ADULT-MORT-" + TreeId, year, areaSeed)`
   - while `RD(survivors) > 1.0`, kill the most suppressed survivor (S desc, TreeId asc)
   - each victim becomes `BiologicallyDead` with cause `DensitySuppression` and the current year, and emits exactly one fallen-deadwood record.
   - Area A and Area B are never pooled for RD.
6. **Invalidate and refresh** canopy/light so the post-mortality union and 2.5 m light index exclude the dead stems.

### 2.3 Biological death and fallen deadwood

- `LifeState { Living, FelledStump, BiologicallyDead }`; death cause and year are stored; transition is idempotent.
- Biologically dead trees contribute nothing to CI, growth, crown union, light or timber; they are not stumps and never enter felling yield.
- One `FallenDeadwoodRecord` per death: `DeadwoodId`, `TreeId`, `AreaId`, `SpeciesId`, `XM`, `ZM`, `DeathYear`, `Cause`, `DbhCm`, `HeightM`, `EstimatedVolumeM3`.
- `EstimatedVolumeM3 = DBH_cm^2 * 0.00007854 * Height_m * 0.5` (Sitka `formHeightRatio`). Float32 arithmetic, matching Unity's record.
- No decay timer, no habitat score, no economy value in this stage.

### 2.4 Pruning (engine-level, commitment-gated)

- Per-tree `PruningLifts`, `CrownBaseHeightM`, `LastPruningYear`.
- Work Plan order `Prune { TreeId, TargetCrownBaseHeightM }` follows the existing fenced Mark → Pending → Approve → Execute lifecycle:
  - Mark/queue creates Pending only and mutates no tree.
  - Approve mutates no ecology and no crown.
  - Execute applies one lift, does not advance the year, and invalidates canopy/light.
- Guards (reject cleanly): not living; `lifts >= 3`; fewer than 5 years since the last lift; target ≤ current crown base; target ≥ 60 % of height; target ≤ 0.
- Execute effect: `r = max(0.1, r * 0.92)`; `lifts++`; `crownBase = target`; `lastPruningYear = current year`.
- Future crown target multiplier `0.92^lifts` applies alongside CrownModel 1.
- No timber-quality multiplier, price bonus or economic effect.

### 2.5 Site productivity and age phases

- `SiteProductivity(x)` reads from the existing `SiteConditions`/per-position seam and **defaults to exactly 1.0** everywhere on Areas A and B. No synthetic soil map.
- Default 1.0 must reproduce the zero-override baseline identically.
- A synthetic non-1.0 test value may exercise the seam; it must scale DBH and height increments and nothing else.
- Sitka has no authored growth age phases; the seam must still apply factors when a synthetic table is supplied.

## 3. Contracts and reporting

- Out-Tray per tree: `LifeState`, `DeathCause`, `DeathYear`, `PruningLifts`, `CrownBaseHeightM`.
- Out-Tray per rack: `RelativeDensity`, annual and cumulative natural deaths, immutable `FallenDeadwoodRecord[]`.
- Model metadata: `AdultModelVersion`, `MortalityModelVersion`, `CrownModelVersion`; evidence labels `Comeau 2010 = UK/British transfer`, `mortality hazard = Gameplay Calibration`, `synthetic multi-rack stands = Experimental Demonstration, not an Irish field forecast`.
- In-Tray: `Prune` added without weakening Crop/Fell exclusivity, approval gates, stale-plan fences or per-rack scoping.
- Session-only; no save schema.

## 4. Acceptance evidence

1. Zero drift: all existing tests pass; model 0 fingerprints and P0 pin unchanged; `/multi` builds 0/0 and serves 200 with no exceptions.
2. Cross-rack growth: felling a large Area A competitor within 8 m of the shared border reduces CI and increases the next DBH increment of a border tree in Area B; a tree more than 8 m away is unaffected.
3. Mortality oracle: RD arithmetic matches Unity; RD < 0.6 gives zero hazard; 0.6–1.0 is suppression-biased; > 1.0 triggers deterministic boundary kills. Overstocked Area A self-thins, Area B's RD uses only Area B, each death yields one deadwood record, and dead stems leave competition/canopy/light.
4. Pruning: Pending/Approve zero change; Execute applies 0.92 immediately and does not advance the year; future targets use 0.92^lifts; invalid targets rejected.
5. Neutral site default identical to no override; a synthetic non-1.0 value scales growth only.
6. Determinism: identical seed and commands produce bit-identical multi-year output.
7. Independent review before owner acceptance.

## 5. Exclusions

No Unity/Editor, Compact30, Legacy40, Enlarged80, Reference Future v1, saves, P0, `/multi` layout/SVG/CSS, regeneration/seedlings/browsing, Oak/Beech, economy, wind/storms, deadwood decay, synthetic soil maps or 2D save files.

## 6. Honest limits

- Comeau 2010 is a British Sitka maximum size-density relationship transferred to Ireland; it is not an Irish field calibration.
- The mortality hazard is a gameplay calibration (onset 0.6, strength 0.08, exponent 4), not a measured annual mortality rate.
- The synthetic MA-1 stands are an experimental demonstration, not an Irish empirical forecast.
- The 2D height site-productivity multiplier is a neutral-by-default seam extension; at 1.0 it matches the Unity site-class envelope.
- No claim of deadwood decay, habitat value or timber grade is made in this stage.
