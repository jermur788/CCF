# Current species architecture audit (Part 1)

**Base:** `341ccbf` (StormModel1 dormant, save v19, RNG 1, regeneration 2, growth 1). Static inspection of actual execution paths. Unity was not run.

Classes:
- **A** genuinely species-generic (no species input needed);
- **B** species-data driven (works for any `TreeSpeciesDefinition`);
- **C** Sitka-specific;
- **D** hard-coded to the three current species;
- **E** scenario-specific rather than species-specific;
- **F** likely to break or duplicate at 10–20 species.

## 1. Summary

The core is **better than its name suggests**. Growth, competition, crowns, regeneration, seed rain, mast, juvenile survival, browsing, promotion, storms and save state already run per tree / per species from `TreeSpeciesDefinition` data, and the timber-yield and price-book *structures* are species-keyed. The problems are concentrated in five places:

1. **Light is species-blind.** Every tree shades by the same height × crown rule (`RecomputeCanopy`). Beech, birch and Sitka cast identical shade. Mixtures cannot differ in the one process CCF regeneration depends on most.
2. **Growth Model 1 is Sitka-only, but its density mortality is not.** Sitka follows the Class III height curve while other species use the legacy linear law. The British-Sitka SDI self-thinning (`SitkaGrowthModel`) is applied to **all living trees, including oak and beech**, in a mixed stand.
3. **There is no site state.** `ForestEcologyCell.SiteProductivity` and `SoilStability` are fixed at 1.0, never authored, never saved. Species–site matching is impossible today.
4. **Timber, economy, residue and deadwood visuals are Sitka-only by factory, not by structure.** `TimberYieldDefaults.CreateSitka()`, the Sitka-only price/density rows, `ScenarioOneEconomyAdapter` (non-Sitka = "unmarketed"), the inspection estimate, felling brash and log visuals.
5. **About 30 literal species-id checks in scenario, reference, habitat, soundscape and UI code** (§3). Each new species would need hand edits.

**Verdict:** a minimal Wave 1 does **not** need an ecology rewrite. The stop condition "current species model cannot support a minimal Wave 1 without broad rewrite" is **not** met. It does need three bounded foundations first: species shade casting, a species-generic height/density layer, and site data.

## 2. Subsystem table

| Subsystem | Execution path | Class | Notes / 10–20-species risk |
|---|---|---|---|
| Species data | `TreeSpeciesDefinition` (~45 serialized fields; 3 assets) | B | Good base. Missing: shade casting, site tolerance, longevity, height-curve, density, timber/quality, dispersal mode |
| Tree state | `ForestTree` (species ref, DBH, height, crown, marks, pruning, mortality cause/year, suppression history) | A/B | Species-agnostic. No quality state |
| Competition | `UpdateCompetition`: Hegyi on DBH ratio and distance, 8 m cutoff, all living trees, all species | A | Cross-species by default. No height asymmetry. No species tolerance except Ci50 in growth |
| DBH growth | `GrowAdults`: `PotentialDbhGrowth × site × size × 1/(1+CI/Ci50)` | B | Species-data driven; `site` is always 1 |
| Height growth | Sitka: `SitkaGrowthModel.NextHeight` (growth model 1). Others: legacy linear `PotentialHeightGrowth × (1 − H/Hmax)` | **C** + B | Two different height laws in one stand. Relative heights in mixtures are not credible |
| Crown | Per-species linear or power-law crown radius; relaxation | B | OK |
| **Canopy / light** | `RecomputeCanopy`: influence = lateral(crown reach) × min(1, H/8), opacity implicitly 1 for all species | **A (species-blind)** | **Core mixed-stand gap.** No crown transmission or leaf phenology. Light is per 5 m cell (adequate) |
| Adult density mortality | `ApplyAdultDensityMortality`: British-Sitka SDI/RD on **all** living trees; vulnerability from each species' Ci50 suppression | **C applied to all species** | Broadleaves die under a Sitka maximum-density line. **F**: must become species-weighted |
| Background / old-age mortality | none | — | Birch (short-lived) and oak (long-lived) are indistinguishable |
| Storm vulnerability | `StormWindthrow.Vulnerability`: H, DBH, local top height, light, recent opening, `StandWindSusceptibility`, `WindOpeningWeight` | B (data hooks all 1.0) | Generic. Rooting/soil differences absent (site-level `SiteFactor` only) |
| Seed production / mast | Per species maturity, crown, mast states by species | B | OK |
| Seed rain | `ComputeSpeciesSeedRain`: exponential kernel per species, per cell, per tree of that species | B | Cost ∝ species × cells × trees (§F). Single kernel shape: bird/animal dispersal not distinguished |
| Establishment / bands (Model 1/2) | species + origin + cell bands, ≤ 4 + accumulator | B | Cohort count ∝ species. Fine to ~15 |
| Juvenile light / survival | `JuvenileEcologyRules` with per-species light anchors (distinct responses opt-in) | B | Good base for shade tolerance |
| Vegetation competition (Model 2) | Shared response, all species (`strength × cover × height vulnerability`) | A (deliberately) | Keep shared until evidence differs |
| Browsing | Per-species palatability, vulnerability heights; shelters | B | OK |
| Promotion | Per-species promotion height, minimum light | B | OK |
| Planting (exact) | `TryPlantJuvenile(species)` generic. `TryPlantBeech` and `ForestPlayer` Beech/Oak constants (non-scenario hotbar) | B + **D** | Scenario One hotbar reads shop entries (E). Non-scenario hotbar is D |
| Nursery stock | `ScenarioOneDefinition.shopEntries` (beech/oak items) + price-book plant rows (sitka/beech/oak) | **E** + D | Shop is scenario data (good). Price-book duplicates (see the P3 sapling audit) |
| Timber yield | `TimberYieldCalculator` filters specs by `SpeciesId` (generic). Only factory `CreateSitka()` exists | B structure, **C** content | Adding a species = add a spec set |
| Economy / prices | `ForestryPriceBook` rows keyed by species. Only Sitka prices and density. Adapter treats non-Sitka as unmarketed; `HasUnmarketedSpecies` | B structure, **C** content + adapter | Adapter hard-codes Sitka density and price ids |
| Pruning | `ForestTree.CanPrune` + scenario lift heights (2.5/5/6.5 m) | **E** | Lift heights are scenario, not species. Broadleaf formative pruning differs |
| Deadwood | `ScenarioDeadwoodRecord` generic; log visual Sitka-only (`speciesId == "sitka-spruce"`) | A + **C** visual | Broadleaf logs fall back or disappear |
| Felling residue | `if (order.speciesId != "sitka-spruce") return;` | **C** | No broadleaf brash |
| Save / load | `TreeSaveData.speciesId` string; cohort keys species+origin; model versions | A | Robust; new species need no schema bump (§16) |
| Visual catalog | `ForestSpeciesVisualSet` per species (seedling/pole/mature/stump/pruning bases). Plantation/recent visuals Sitka-only; habitat juvenile prefabs `beechSapling` / `oak…` | B + **C/D** | **F**: art per species is the main cost (§15) |
| Tree inspection | Generic fields. Timber estimate Sitka-only; wind via storm API | B + **C** | — |
| Map / Work Plan | Generic by cell. Work Plan "No broadleaf timber market" | A + C copy | — |
| Objectives / tutorial | Iterate `ShopEntries` (E). Century review literal oak/beech; original species = spawner default | **E/D** | — |
| Reference runner / Future v1 | Literal species throughout | **D (frozen, acceptable)** | Reference v1 must not change (D-042) |
| Habitat / soundscape interpretation | Literal sitka/beech/oak | **D** | Use species tags (conifer/broadleaf/evergreen) instead |
| Site productivity | Cell `SiteProductivity`, `SoilStability` = 1.0 constant, unsaved | (absent) | **F** for any site matching |

## 3. Literal species-id sites (`grep`, production only, excluding Editor art tooling and Reference)

| File | Count | Nature |
|---|---|---|
| `ForestEcologyController.cs` | 3 | `TryPlantBeech`; Sitka height switch |
| `SitkaGrowthModel.cs` | 1 | Species id constant |
| `ForestTreeSpawner.cs` | 2 | Plantation/recent visuals only for Sitka |
| `ForestPlayer.cs` | 2 | Beech/Oak constants (non-scenario mode) |
| `ScenarioOne/ScenarioOneDefinition.cs` | 3 | Shop entries (data); timber value per species |
| `ScenarioOne/ScenarioOneEconomyAdapter.cs` | 4 | Sitka density/price/market |
| `ScenarioOne/ScenarioOneManager.cs` | 4 | Broadleaf list, log visual, residue, default species |
| `ScenarioOne/ScenarioOneObjectives.cs` | 3 | Century review oak/beech, "Sitka regeneration-control" label |
| `ScenarioOne/ScenarioHabitatVisuals.cs`, `ScenarioHabitatInterpretation.cs`, `ScenarioSoundscape.cs` | 6 | Literal species groupings |
| `UI/TreeInspectionView.cs` | 2 | Sitka timber estimate |
| `TimberYield/TimberYieldConfiguration.cs`, `WorkEconomy/Stage1EconomyDefaults.cs` | ~12 | Sitka assortment and price content (data in code) |
| `ScenarioOne/ScenarioReferenceRunner.cs` | ~20 | Frozen Reference schedule. **Leave** |

## 4. What this means for expansion

| Need for the next species | Already there | Missing |
|---|---|---|
| Grow and compete | Yes | Credible height law per species; site multiplier |
| Shade others / tolerate shade | Tolerance partly (juvenile light anchors, Ci50) | **Casting** (opacity/transmission) |
| Regenerate from seed | Yes | Dispersal mode (bird), longevity |
| Die appropriately | Self-thinning (Sitka law), storms | Species-weighted density, age/senescence |
| Be planted | Yes (exact planting) | Scenario shop entry + visuals |
| Be sold | Structure only | Assortment specs, prices, density per species/market |
| Be seen | Visual set slot | Assets |
| Be understood | Generic UI | Literal-id copy to replace with species tags/names |
