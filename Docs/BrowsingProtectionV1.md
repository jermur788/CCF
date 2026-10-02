# Browsing & Protection v1

Task: Regeneration Bottlenecks v1A — Browsing & Protection.
Branch: `task/regeneration-browsing-protection-v1`. Base: `bb314a4`. Context commit: `db2b4f1`.

Browsing/protection **ecology** is authoritative here. Costs, labour, materials, contractor work and maintenance of fences/shelters belong to the work/economy system (`task/stage1-work-economy-foundation-v1`), which will later create and update the protection records defined below.

Evidence tags used in code and in this document:

| Tag | Meaning | Browsing report class |
|---|---|---|
| [E] | Empirical measurement (Irish unless marked "transferred") | A, or C when transferred from Britain/NW Europe |
| [G] | Management guidance | B |
| [I] | Evidence-derived inference | — |
| [S] | Simulation abstraction | D |
| [C] | Gameplay calibration | E |

Primary source: `Irish_CCF_Browsing_Protection_v1_Report.pdf` (1 Oct 2026). Supporting: `Irish Atlantic Woodland Ecology…`, `Atlantic Temperate Rainforest Under Continuous-Cover Forestry…`, `Understorey_Dynamics_Irish_CCF_Report.pdf` (concealment interface only). `Irish_CCF_ecology.pdf` has no quantitative browsing content.

## CURRENT IMPLEMENTATION INVENTORY

Inspected at `bb314a4` before any change.

**Shared biology.** `Assets/ForestPrototype/JuvenileEcologyRules.cs` owns juvenile light response, survival response, height growth, individual survival realisation, promotion eligibility and promotion DBH. Origin never enters the rules.

**Natural cohort path.** `ForestEcologyController.GrowExistingRegeneration` (annual step 6, after adult growth and the canopy refresh). It runs per cell and per species cohort: `GrowHeight` on the cohort's single mean height, then `Density *= SurvivalResponse` as a deterministic expected value with no RNG. A capped +0.05 infill applies when light survival is full (`ForestEcologyCell.AddDensityWithSharedCapacity`), and the cohort is cleared below 0.01 density. Establishment is in `EstablishNewCohorts` (step 9). Promotion is in `PromoteSpeciesCohorts` (step 10): one tree per cohort at `PromotionHeightM` (3.5 m) and `PromotionMinimumLight`, with a seeded per-species System.Random for the in-cell offset.

**Legacy planted cohort path.** The same code as natural cohorts. `Origin = Planted` is diagnostic only, with a back-dated establishment year.

**Exact planted individual path.** `ScenarioOneManager.AdvancePlantedJuveniles` runs immediately after `ForestEcologyController.AdvanceOneYear` in Scenario One's annual sequence. For each juvenile in juvenile-id order it calls `GrowHeight` using its cell's light, then `Survives` against `SimulationRandom.Roll(model, juvenileId, year, seed)`, then promotes at `CanPromote` into a tree with id `PL-<juvenileId>`.

**Mortality.** `ForestTree.ApplyMortality(cause, year)` applies to adult trees only, is explicit, and has no automatic trigger. Juvenile death is density loss (cohorts) or `alive = false` (individuals).

**Density/capacity.** Cohort density is relative per cell (max `RegenDensityMax`), with shared occupancy capped at 1. Exact individuals are not counted against cohort capacity.

**Species parameters** (`TreeSpeciesDefinition`, assets in `Assets/ForestPrototype/Species/`). All three regenerating species share 0.35 m/yr juvenile growth in full light and 3.5 m promotion height. Oak uses distinct survival/establishment light responses and promotion minimum light 0.2. Beech has a shade-tolerant light-response curve.

**RNG/hash/version.** `SimulationRandom` model 0 is the legacy default and model 1 is opt-in. `Roll(model, id, year, seed)` is a stateless string-keyed draw. The canonical lifecycle hash is computed from explicit tree and ecology state, not save JSON.

**Save representation (v14).** Cohort: species, density, height, establishYear, origin, originYear. Planted juvenile: id, species, position, cell, plantingYear, age, height, alive, stock, promotedTreeId, legacy flag. Tree: v14 mortality fields. Every addition of a persisted field so far came with a version bump (v9 origin, v14 mortality).

**Insertion point.** A single browsing response fits inside `JuvenileEcologyRules` between "potential growth from light" and "survival". Both adapters already call those rules with identical inputs, so browsing needs no second formula and no change to the juvenile architecture.

## Evidence extraction for v1

| Effect | v1 decision | Basis |
|---|---|---|
| Site-level browse pressure | **Essential.** Normalised 0–1 stand scalar, not deer density | Report §2: impact/activity, not counts; a deer-density threshold must not be hard-coded [G/E transferred] |
| Species differences | **Essential**, as palatability only. Recovery and resilience are not differentiated | Palatability ordering oak > beech > Sitka [E transferred, NatureScot]; numbers are [S] |
| Juvenile-stage vulnerability / height escape | **Essential.** Full vulnerability, then a linear taper to zero | Large-deer reach ~1.8 m [E transferred]; broadleaf breakpoints 1.2–1.8 m [S]; Sitka leader browsing rare from ~0.8 m [E transferred, W. Scotland] |
| Leader browsing → height-growth reduction / promotion delay | **Essential. The central mechanic** | Report §3, §6; Killarney [E]; Sitka leader-browse trials [E transferred] |
| Repeated browsing | **Emergent.** Short juveniles stay in the vulnerable zone and keep being browsed | Report §6 feedback loop [I] |
| Survival effect | **Essential but small** | "Increase mortality at severe/repeated pressure, but not make mortality the dominant effect" [G]; exact value [C] |
| Natural vs planted susceptibility | **Same response.** No origin term | Report §9 architecture rule |
| Fencing (intact/breached) | **Essential.** Access barrier, binary | Teagasc: a single breach admits deer [G] |
| Individual shelters | **Essential** for exact planted juveniles, with a working life and failure | Irish schemes support deer shelters; ~6–8-year establishment measure [G] |
| Fence service life (IS436 ~15 yr) | **Deferred.** A maintenance-risk horizon, not an expiry | [G]; needs maintenance/condition system |
| Persistent form damage | **Deferred.** Needs new persisted state (see Persistence) | Sitka multi-leader/forking after repeated browsing [E transferred] |
| Bramble concealment | **Deferred.** The rule has a `vegetationExposure` input fixed at 1 | Killarney fence breach: 49% weeded vs 11% vegetated browsed [E]; understorey is not authoritative yet |
| Regeneration density modifier | **Unsupported/off** | Direction ambiguous; the report says leave it out |
| Gap-attracts-deer | **Unsupported/off** | Not justified for a 40 m stand (report §7) |
| Hares/rabbits, livestock, deer management, bark stripping | **Deferred** | Separate channels/systems (report §1, §17, §20) |

## Model

Order within each juvenile's annual update (report spec §4):

1. Assess browsing at the juvenile's height **before** this year's growth.
2. Compute protection/access.
3. Compute `p`.
4. Apply the browse event or fraction.
5. Grow height (light × site potential, minus the browsed loss).
6. Apply survival (light survival × browse mortality).
7. Check promotion.

```
effective = backgroundPressure × palatability(species) × heightVulnerability(species, height)
            × vegetationExposure × protectionAccess
p         = min(0.95, effective)
increment = potential(light, site) × (1 − browsedFraction × 0.9)
survival  = lightSurvival × (1 − browsedFraction × 0.04)
```

- Cohorts use `browsedFraction = p`, the expected value with no RNG. This is the same convention as existing cohort survival.
- Exact individuals draw one event per year: `Roll(model, juvenileId + "/browse", year, seed) < p`. Then `browsedFraction` is 0 or 1.
- The `/browse` suffix makes this a browse-specific domain inside the existing RNG models. No project-wide RNG model or version changed. The harness confirms the browse and survival rolls are independent.
- With pressure 0, the rule delegates to the unchanged light-only functions, so the result is bit-identical.

### Current parameters

| Parameter | Value | Tag | Note |
|---|---|---|---|
| `backgroundBrowsePressure` (ForestEcologyController) | 0 (off) | [S] | Scenario level is a product decision (see below) |
| Pressure bands for UI | <0.3 low, 0.3–0.65 moderate, >0.65 high | [C] | Report table; interpretation only |
| Oak palatability | 1.0 | [E transferred] ordering, [S] number | Report range 0.8–1.0 |
| Beech palatability | 0.5 | [E transferred]/[S] | Report range 0.35–0.7 |
| Sitka palatability | 0.15 | [E transferred]/[S] | Report range 0.1–0.35 |
| Oak/beech full vulnerability → escape | 1.2 m → 1.8 m | [S] taper; 1.8 m deer reach [E transferred] | |
| Sitka full vulnerability → escape | 0.5 m → 0.8 m | [E transferred] | Report sensitivity 0.6–1.0 m |
| Probability cap | 0.95 | [C] | |
| Height-increment loss per browse | 0.9 | [C] | Major sensitivity parameter |
| Extra mortality per browse | 0.04 | [C] | Kept small by design |
| Vegetation exposure | 1 | [S] | Understorey concealment hook |
| Intact fence access | 0 | [G] | Binary barrier |
| Breached fence access | 1 | [G] | Normal local pressure |
| Effective shelter access | 0 | [G]/[E transferred] | Leader enclosed |
| Shelter working life | 8 years (configurable) | [G] | Report "~6–8 yr initial assumption" |

Species default palatability is 0, so any species without configured values is never browsed.

## Protection

`Assets/ForestPrototype/BrowsingConditions.cs`:

- `BrowseProtectedArea`: a closed XZ polygon with `installedYear` and `breachedYear`. Deer are excluded while installed and not breached. Cohorts use the fraction of their 5 m cell inside intact fences (5×5 sample points [S]). Individuals use point-in-polygon.
- `BrowseShelter`: a position, `installedYear`, `effectiveYears` and `failedYear`. It protects the exact-position juvenile within 0.25 m while effective, then reports `ExpiredShelter` or `FailedShelter`. Shelters do not apply to cohorts.
- **Protection never creates growth.** It only sets `protectionAccess`. The harness checks that every fenced outcome equals the browse-free trajectory exactly. A shelter or fence also becomes irrelevant once the juvenile is above escape height: a breach after escape changes nothing.
- Fence deterioration from condition/age (the IS436 15-year horizon) is not modelled. That needs a condition/inspection/repair system, which is a work/maintenance task. The ecological interface is already there: whoever owns maintenance sets `breachedYear`, or removes the area.

Pressure and protection are **configuration** in v1, held on `ForestEcologyController.Browsing` and not saved. That is why production keeps pressure at 0 and adds no protection: a player-created fence or shelter would have to be saved (see Persistence).

## Natural / planted shared response

Both adapters call the same rule functions with the same inputs (species, pre-growth height, light, site, pressure, access). Equivalence is checked in `BrowsingProtectionVerification`:

- Natural and legacy-planted cohorts give bit-identical height and density under browsing.
- 4,000 exact individuals versus one cohort, with equal inputs and p = 0.6:
  - at light 0.55: mean increment 0.1600 vs cohort 0.1610; survival 0.980 vs expected 0.976;
  - at light 0.12: increment 0.0163 vs 0.0164; survival 0.797 vs 0.791.
  - The browse rate was 0.603 against p = 0.600. All values are within 4 standard errors.

Over many years a cohort's single mean height smooths individual variation. Under high pressure the cohort crosses the escape taper earlier than the median individual: strong-light oak cohort at year 22 versus individual median at year 27. This is a property of the existing cohort representation [S], not a second biology.

## Persistent state

**No new persisted fields.** Save schema stays **v14**.

What already persists the biological consequence:
- reduced height growth → cohort `height` / juvenile `heightMeters`;
- browse mortality → cohort `density` / juvenile `alive`;
- promotion delay → the absence of a promoted tree.

Save/load continuation is verified to be identical with browsing on (pressure 0.7, sheltered planted oak/beech). Browse probability, protection status and its reason are recomputable at any time from saved state plus configuration.

Not persisted, and so **not implemented as biology**:
- browse history (`yearsBrowsed`, `consecutiveBrowseYears`, `lastBrowseYear`);
- form damage;
- player-created protection records.

The diagnostic flags `LastBrowsedFraction` (cohort) and `lastYearBrowsed` / `lastBrowseAssessment` (planted juvenile) describe the most recent annual step only, and reset on load. No biology reads them.

**Schema decision needed (v15).** Adding these fields under v14 would let an older v14 build load a newer save and silently drop protection and browse history. The project's convention is a version bump per persisted addition. Proposed v15 additions, with defaults that are correct for older saves:

| Record | Fields |
|---|---|
| Planted juvenile | `yearsBrowsed`, `consecutiveBrowseYears`, `lastBrowseYear`, `formDamage` (0 none / 1 recoverable / 2 persistent) |
| Cohort | `yearsBrowsed`, `lastBrowseYear`, `formDamageFraction` |
| Tree | `formDamage`, inherited at promotion, for crop-tree selection |
| Stand | `backgroundBrowsePressure`, once deer management can change it |
| Fences | `areaId`, polygon, `installedYear`, `breachedYear` (later: condition) |
| Shelters | `shelterId`, `juvenileId`/position, `installedYear`, `effectiveYears`, `failedYear` |

## Form damage (Phase J)

Repeated leader browsing of Sitka is associated with multiple leaders, forks and smaller main stems many years later [E transferred]. A three-state indicator (none / recoverable / persistent) would give crop-tree selection and pruning a real input. It requires persisted browse history on juveniles and trees (above), so it is **deferred to the v15 decision**. Timber-price consequences remain out of scope.

## Calibration (BrowsingProtectionVerification)

Matrix:
- species: Sitka, oak, beech;
- light: 0.08 / 0.18 / 0.35 / 0.70;
- pressure: 0 / 0.2 / 0.5 / 0.85;
- protection: none / shelter / fence;
- run for 40 years with snapshots at 5/10/20/40.

Each condition has a natural cohort (from establishment height) and 60 exact planted juveniles (0.6 m, age 3), run through the production adapters at fixed light. Selected 10-year results for individuals ("prom" = promoted fraction, "browse" = mean browse events per juvenile):

| Condition | Survival | Prom 10y | Prom 20y | Median promotion yr | Browse events 10y | ≥3 events |
|---|---:|---:|---:|---:|---:|---:|
| Oak strong, none | 1.00 | 1.00 | 1.00 | 9 | 0 | 0 |
| Oak strong, low 0.2 | 0.93 | 0.70 | 0.93 | 9 | 1.3 | 0.18 |
| Oak strong, moderate 0.5 | 0.88 | 0.40 | 0.67 | 13 | 4.0 | 0.50 |
| Oak strong, high 0.85 | 0.78 | 0.07 | 0.35 | 27 | 6.8 | 0.80 |
| Oak strong, high, shelter or fence | 1.00 | 1.00 | 1.00 | 9 | 0 | 0 |
| Oak moderate light, high | 0.78 | 0.00 | 0.25 | 38 | 7.1 | 0.83 |
| Oak marginal 0.18, high, fence | 0.77 | 0.00 | 0.00 (40 y) | — | 0 | 0 |
| Beech strong, high | 0.92 | 0.48 | 0.78 | 11 | 3.4 | 0.45 |
| Sitka strong, high | 1.00 | 0.90 | 1.00 | 10 | 0.23 | 0.05 |

Required outcomes, all asserted by the gate:

1. **Poor light is not rescued by protection.** Fenced outcomes equal the browse-free baseline exactly in every cell of the matrix. Oak and Sitka at 0.08 light never recruit, protected or not. Beech's existing shade-tolerant curve lets it recruit slowly at 0.08 light (year ~31–36); protection never improves on that.
2. **Good light with high pressure delays or prevents recruitment.** Oak 10-year promotion falls from 100% to 7%, with 80% of juveniles browsed three or more times.
3. **Protection improves outcomes under pressure.** Sheltered or fenced oak returns to 100%.
4. **Protection does not guarantee success.** Fenced marginal-light oak never recruits (below oak's promotion light), and survival still follows light.
5. **Low pressure stays close to baseline.** Median promotion is unchanged (year 9 for oak).
6. **High pressure is not instant death.** 10-year survival is 0.78 for oak.
7. **Species ordering.** Browse events at year 10 under high pressure: oak 6.8 > beech 3.4 > Sitka 0.23. The recruitment-delay ordering matches.
8. **Shared natural/planted response** (above).
9. **Later reclosure.** Fenced oak with three good years then 0.08 light never recruits.

Experiments:
- **Early fence breach** (year 3): exposes juveniles still below 1.8 m. The outcome falls between open and intact.
- **Late breach** (year 8): juveniles have escaped, so the outcome is identical to an intact fence.
- **Shelter life** at 0.35 light and high pressure: a 3-year shelter lets browsing resume (7.7 events by year 20); a 40-year shelter gives none.

Sensitivity (expected-value oak, strong light, years to 3.5 m):
- increment loss 0.6 / 0.8 / 0.9 / 1.0 at pressure 0.85 gives 12 / 14 / 17 / 21 years (baseline 9);
- extra mortality 0.02–0.08 moves 17-year survival from 0.84 to 0.49 at loss 0.9.

The chosen 0.9 / 0.04 sits in the middle: heavy pressure becomes a long delay with modest mortality rather than certain death.

Determinism and hashes:
- `BROWSE_MATRIX_HASH BEC549ED9EBA18B7` and `BROWSE_MEASUREMENT_HASH 32224555690DD824` are identical across separate Unity processes.
- Neutral canonical lifecycle: `BFC55473C1506067`, unchanged.
- Lifecycle with pressure 0.6: `5863F0BF89C817B5`. Deterministic, diagnostic only.

## Performance

Measured in Editor batch mode with an oak and a Sitka cohort in every cell. Browsing-on runs include a fence over a quarter of the stand.

| Stand | Cells | Regeneration step off / on | Annual step off / on |
|---|---:|---:|---:|
| 40 m | 64 | 0.09 / 0.74 ms | 64 / 67 ms |
| 80 m | 256 | 0.25 / 2.9 ms | 917 / 923 ms |
| 100 m | 400 | 0.43 / 4.6 ms | 2865 / 2710 ms |

Browsing adds well under 1% of the annual step, which is dominated by the existing adult competition/canopy work. No per-juvenile spatial search is involved: cohorts sample their own cell, and individuals test only the configured fences and shelters. No optimisation was made.

## Player-facing diagnostics

- `ForestEcologyController.AssessCohortBrowse(cell, species, height)` and `AssessIndividualBrowse(position, species, height)` return a `BrowseAssessment`. It holds pressure and band, palatability, height vulnerability, protection access and state (none / intact fence / breached fence / effective, expired or failed shelter), probability, and a reason (no pressure, not palatable, above browse reach, protected, exposed).
- `RegenerationReportLine` adds "browsing <band> (leaders at risk N%/yr)" only when pressure is above 0.
- Questions answered:
  - Is pressure high here? Yes, via the band.
  - Is this juvenile protected? Yes, via the protection state.
  - Is browsing delaying regeneration? Yes, via probability and reason.
  - Why did it fail or persist? Light vs browse reason, plus last-year flags.
  - Has it been repeatedly browsed? **Not across saves** until v15 persists history.

## Known limitations

- Production pressure is 0. Scenario One shows no browsing until the level is decided and the player can protect juveniles.
- There is no persisted browse history or form damage, and no player-created protection persistence (v15 decision).
- Cohort mean-height smoothing (see above). Cohort infill continues under browsing, because it represents new seedling arrival; browse losses still apply to existing density.
- One stand-level pressure; no edge, gap-attraction or density effects (deliberately off, per the report).
- No hares/rabbits, livestock, deer management, bark stripping or fraying.
- No understorey concealment (`vegetationExposure = 1`).
- Shelter microclimate growth effects are deliberately excluded.

## Future interfaces

- **Understorey:** set `vegetationExposure` from dense bramble/obstructive cover only, and keep competition separate (report §8; understorey report §8).
- **Work/economy:** create/remove `BrowseProtectedArea` and `BrowseShelter` records from completed work orders, and set `breachedYear`/`failedYear` from condition, inspection and repair. Costs, labour and materials stay in that system.
- **Deer management:** a work action that moves `backgroundBrowsePressure` toward a managed target and relaxes it back toward a regional baseline. Needs the stand pressure to become saved state.
- **Crop-tree selection/pruning:** consume tree `formDamage` once persisted.

## Verification

Copy `Tools/Verification/BrowsingProtectionVerification.cs` into `Assets/ForestPrototype/`, run `-executeMethod BrowsingProtectionVerification.Begin` in Unity 6000.6.0f1 batch mode, then remove the copy and its `.meta`. The gate:
- captures the scene world in memory, restores it and checks the world hash at the end;
- never writes the save slot;
- ends with `BROWSING_PROTECTION_VERIFY_PASS`.
