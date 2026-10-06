# Regeneration model 1 (age bands) — implementation record

Implements the Manager decision "REGENERATION MODEL 1" on `task/regeneration-population-budget`. Base 402a2b4 (main) plus evidence commits 70f5cc7, 3e9afb4 and ad5c95e. Not merged.

Juvenile abundance remains an **abstract relative abundance (occupancy)**. Nothing converts it to stems/ha or uses density × area. Promotion is an abstract representation handoff to one exact tree.

## Policy

| Item | Behaviour |
|---|---|
| Save version | 16; new root field `regenerationModel` (int) |
| Missing field / explicit 0 | Model 0, legacy regeneration, unchanged |
| Saves v1–v15 | Always model 0 (the loader ignores the field below v16; no automatic migration) |
| New Scenario One games | Model 1 (`ScenarioOneManager.NewGameRegenerationModel`, set in `Awake` beside the RNG model) |
| Reference Future v1 | Model 0 (historical saves load as model 0; the player's model is restored after preview) |

## Model 1 rules

- **Band key:** species + origin + establishment year. Natural and Planted are never merged. Saved cohort records may repeat a species; restore keeps every band (`ForestEcologyCell.InsertBand`, ordered by species, then origin, then year).
- **Maximum bands:** 4 per species + origin + cell (`ForestEcologyCell.MaxBandsPerSpeciesOrigin`). Validation rejects duplicate keys and a fifth band.
- **Overflow merge:**
  - The adjacent pair with the smallest establishment-year gap merges; ties go to the oldest pair.
  - Abundance is the exact sum. Height is abundance-weighted.
  - Establishment year and origin year are the abundance-weighted mean, rounded half up (`floor(x + 0.5)`).
  - The merged year lies between the two source years, so keys stay unique. No new fields are needed.
  - Species and origin are preserved; the browse diagnostic is abundance-weighted.
- **Unsourced infill:** removed. Existing bands survive, grow, age, get browsed, cleared or promoted, but their abundance never rises without seed. Model 0 keeps its infill.
- **Late recruitment:** each year's accepted natural recruitment is a new band with its own establishment year, initial height (`RegenInitialHeightM`) and Natural origin. It never joins an older band except through the bounded merge.
- **Origin bug:** fixed. A promoted band is removed, so no stale origin record survives; later natural recruitment is Natural.
- **Capacity:**
  - `ForestEcologyCell.AdmitDensity` admits at most the free shared occupancy and never shrinks existing stock.
  - requested = accepted + rejected, recorded per species per year.
  - A fully rejected request creates no band and sets no establishment year.
  - No density-dependent loss of existing juveniles is modelled.
- **Promotion:** in each cell and year, the oldest band of a species that meets the existing `CanPromote` rule (ties: Natural first) becomes one exact tree and is removed. Younger bands are untouched and can promote in later years.
- **0.01 threshold:** kept. Under model 1 a band below 0.01 after survival is removed, and the loss is recorded as `ThresholdExtinction`. It cannot create abundance.
- **Seed rain:** moved to a per-cell, per-species map (`SeedRainFor`). Model 0 still carries its legacy copy on the cohort record, so its arithmetic is unchanged.
- **Accounting:** `ForestEcologyController.LastRegenerationAccount` reports per-species flows for each annual step in both models: seed arrival, requested, accepted, rejected, legacy infill, capacity contraction, light/browse loss, threshold extinction, bands created/merged, promotions, exported abundance and exact trees. It is diagnostic only and never saved.

## Verification (Unity 6000.6.0f1 batchmode, isolated config per process)

`RegenerationModelVerification` passed 25/25 checks in three separate processes:
- **Policy:** new game is model 1; missing field, v15 and explicit-0 loads are model 0; Reference v1 previews are model 0 and the player model is restored.
- **Accounting invariants:** NO_SEED_NO_NEW_ABUNDANCE (light 0.9 / 0.3 / 0.05), NEW_RECRUIT_STARTS_AT_AGE_ZERO, NEW_RECRUIT_GETS_INITIAL_HEIGHT, NATURAL_RECRUIT_REMAINS_NATURAL, PLANTED_RECRUIT_REMAINS_PLANTED, CAPACITY_REJECTION_DOES_NOT_REMOVE_EXISTING_STOCK, FULL_REJECTION_CREATES_NO_COHORT, EXTINCTION_THRESHOLD_ACCOUNTED.
- **Bands and promotion:** BAND_MERGE_CONSERVES_ABUNDANCE (tie-break and weighted state checked), BAND_MERGE_PRESERVES_ORIGIN, MAX_BAND_COUNT_RESPECTED, PROMOTION_REMOVES_ONLY_PROMOTED_BAND, YOUNGER_BANDS_SURVIVE_PROMOTION.
- **Save, determinism and viability:** SAVE_LOAD_PRESERVES_ALL_BANDS, save validation of band keys, save/load deterministic around the threshold, MODEL1_DETERMINISTIC, legacy model-0 anchors, CLEARANCE_RELEASES_CAPACITY.

Model 0 is **byte-identical** to before. The legacy BEFORE evidence harness, rerun after the change with model 0 pinned, reproduced the 100-year ledger (SHA-256 `71d61b94…`) and every other evidence file exactly.

## Model 0 vs model 1 funnels

Scenario One stand, seed 20260914, browse pressure 0.2, RNG model 1. Cumulative from year 1; relative-abundance units, except seed arrival (dispersal intensity) and trees (counts). Natural regeneration is Sitka only (no broadleaf seed sources in the starting stand). Data: `Evidence/model0_vs_model1_funnels.csv`.

| Treatment | Model | Year | Seed arrival | Requested | Capacity rejected | Accepted | Legacy infill | Survival loss (light+browse) | Threshold extinction | Promotion exported | Exact trees | Remaining abundance | Remaining bands | Recruited trees alive |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| unthinned | 0 | 10 | 11,452 | 21.9 | 9.0 | 13.0 | 3.0 | 0.7 | 0.27 | 0.0 | 0 | 15.01 | 28 | 0 |
| unthinned | 0 | 25 | 40,692 | 67.2 | 39.1 | 28.1 | 5.1 | 4.5 | 1.50 | 12.0 | 8 | 15.28 | 37 | 8 |
| unthinned | 0 | 50 | 92,014 | 109.2 | 62.1 | 47.0 | 7.3 | 19.3 | 2.92 | 24.0 | 16 | 8.09 | 27 | 16 |
| unthinned | 0 | 100 | 234,633 | 132.5 | 63.7 | 68.8 | 7.7 | 46.8 | 3.37 | 25.1 | 21 | 1.24 | 15 | 21 |
| unthinned | 1 | 10 | 11,452 | 21.9 | 7.0 | 15.0 | 0.0 | 0.7 | 0.50 | 0.0 | 0 | 13.81 | 78 | 0 |
| unthinned | 1 | 25 | 40,655 | 62.3 | 30.5 | 31.8 | 0.0 | 4.9 | 2.51 | 9.3 | 19 | 15.05 | 92 | 19 |
| unthinned | 1 | 50 | 92,357 | 85.0 | 37.4 | 47.6 | 0.0 | 25.8 | 5.87 | 12.9 | 25 | 3.03 | 72 | 25 |
| unthinned | 1 | 100 | 235,939 | 99.5 | 37.4 | 62.1 | 0.0 | 37.0 | 11.88 | 12.9 | 26 | 0.30 | 22 | 26 |
| 20 % removed | 0 | 10 | 9,998 | 32.6 | 12.7 | 19.9 | 5.6 | 0.7 | 0.38 | 0.0 | 0 | 24.40 | 35 | 0 |
| 20 % removed | 0 | 25 | 37,286 | 101.9 | 59.9 | 42.0 | 8.6 | 6.1 | 1.25 | 18.0 | 12 | 25.15 | 41 | 12 |
| 20 % removed | 0 | 50 | 86,778 | 169.4 | 98.3 | 71.1 | 11.5 | 27.4 | 1.96 | 41.2 | 29 | 12.09 | 34 | 29 |
| 20 % removed | 0 | 100 | 225,879 | 212.1 | 113.1 | 99.0 | 12.4 | 58.1 | 2.20 | 49.3 | 38 | 1.84 | 23 | 38 |
| 20 % removed | 1 | 10 | 9,998 | 32.6 | 9.2 | 23.5 | 0.0 | 0.7 | 0.60 | 0.0 | 0 | 22.17 | 108 | 0 |
| 20 % removed | 1 | 25 | 37,225 | 93.3 | 46.0 | 47.4 | 0.0 | 6.4 | 2.69 | 16.0 | 30 | 22.32 | 126 | 30 |
| 20 % removed | 1 | 50 | 87,067 | 130.8 | 63.8 | 67.0 | 0.0 | 30.7 | 6.48 | 22.6 | 45 | 7.23 | 93 | 45 |
| 20 % removed | 1 | 100 | 226,710 | 156.2 | 65.6 | 90.6 | 0.0 | 53.1 | 13.98 | 22.7 | 47 | 0.79 | 41 | 47 |
| 60 % removed | 0 | 10 | 6,084 | 51.7 | 14.6 | 37.1 | 12.3 | 1.2 | 0.34 | 0.0 | 0 | 47.83 | 54 | 0 |
| 60 % removed | 0 | 25 | 25,219 | 167.6 | 87.3 | 80.2 | 21.3 | 13.1 | 0.50 | 42.0 | 28 | 45.87 | 54 | 28 |
| 60 % removed | 0 | 50 | 63,397 | 272.1 | 144.1 | 127.9 | 26.8 | 57.4 | 0.53 | 77.3 | 52 | 19.47 | 53 | 52 |
| 60 % removed | 0 | 100 | 176,189 | 338.4 | 157.8 | 180.7 | 28.9 | 120.2 | 0.60 | 85.2 | 60 | 3.59 | 46 | 60 |
| 60 % removed | 1 | 10 | 6,084 | 51.7 | 6.6 | 45.0 | 0.0 | 1.1 | 0.82 | 0.0 | 0 | 43.08 | 205 | 0 |
| 60 % removed | 1 | 25 | 25,139 | 159.9 | 70.0 | 89.9 | 0.0 | 10.5 | 2.29 | 32.2 | 64 | 44.89 | 209 | 64 |
| 60 % removed | 1 | 50 | 64,771 | 221.4 | 96.3 | 125.1 | 0.0 | 65.7 | 5.95 | 42.6 | 87 | 10.83 | 180 | 87 |
| 60 % removed | 1 | 100 | 182,170 | 259.0 | 96.5 | 162.5 | 0.0 | 98.3 | 20.54 | 42.9 | 88 | 0.71 | 60 | 88 |

Reading:
- **Infill:** model 1 has no infill.
- **Rejection:** about half of model 0's requested establishment was rejected; under model 1, rejection falls to 37 %, 42 % and 37 % (unthinned / 20 % / 60 %).
- **Exact recruited trees:** model 1 produces more (26 / 47 / 88 against 21 / 38 / 60), because younger bands promote independently instead of being compressed into one cohort per species. Mean abundance exported per tree is about 0.5 under model 1 (12.9/26, 22.7/47, 42.9/88); under model 0 the median was a full cohort (1.5).
- **Threshold extinction:** rises (11.9–20.5 against 0.6–3.4) because small bands now die individually instead of being topped up by infill.
- **Seed arrival** differs slightly between models after year 10, because the recruited trees differ.

## Scenario 1 / tutorial viability (model 1, new-game default)

- **Completion:** completed in Year 25, all 8 objectives met (regeneration 49 cells against a target of 3; introduced beech and sessile oak both met). Lowest cash 589,771 cents (€5,897.71). Two runs identical (`F1EBFFEFCE753FAF`). No thresholds or economy changed.
- **Natural regeneration:** occurs (Sitka; see funnels).
- **Planting:** CCFPlanting (model-1 band planting) and ScenarioOnePlanting pass.
- **Clearance:** CLEARANCE_RELEASES_CAPACITY passes. The UI-level ClearanceVerification fails identically on unchanged main (pre-existing).
- **Shelters:** BrowsingProtection passes (exact planted juveniles are unaffected by the model).
- **Finances:** economy integration (120 assertions) and economy viability (64 assertions) pass.

## Findings for the Manager

1. **Sub-threshold recruitment cannot accumulate (decision item).** With one band per year and the kept 0.01 extinction threshold, any species whose accepted recruitment in a year is below about 0.01 cannot establish naturally. The wave is removed the following year.
   - In the MixedSpeciesTest stand, beech produced 6 natural recruits in 100 years under model 0, but about 95 % of that abundance came from legacy infill (1.80 against 0.10 seed-sourced).
   - Under model 1 it produced 15 bands totalling 0.046, all extinguished, and 0 recruits.
   - Scenario 1 completion is unaffected (its beech and oak objectives are met by planting). Later natural broadleaf recruitment from planted parents, or any low-seed species, would face the same limit.
   - The threshold was **not** recalibrated, as instructed. Options for a later decision: let same-year sub-threshold recruitment join the youngest band within a short window; apply the threshold to a species' total juvenile abundance in a cell; or lower the threshold.
2. **Understorey gap (recorded, deferred):** current understorey cover has no causal effect on regeneration (bit-identical trace in RuntimeReproduction.md). Vegetation competition remains a separate future mechanism; no bramble/bracken coefficient was introduced.
3. **Format-dependent hashes:** world hashes are JSON hashes of a captured save, so the v16 field changes them even when biology is identical. `ScenarioReferenceArchive.LegacyV15WorldHash` re-hashes model-0 worlds in the exact v15 layout. With it, the legacy completion (`568922E1A6D73CDD`; RNG 1 + regen 0 `00479F18970F9926`) and Reference continuation (`9CDF21A541C5968D`) anchors reproduce exactly. In v16 layout the same worlds hash `BD0C7E847CA59488`, `9AF2112C3E15F7A3` and `C5EF2C4CC3443208`.
