# Species trait architecture, site model, generalisation order, save impact (Parts 4, 5, 12, 16)

**Status:** proposal. **No production change.**

Evidence classes for parameters:
- **EMP**: empirical (measured);
- **MG**: management guidance;
- **TR**: transfer (other region/species);
- **INF**: inference;
- **CAL**: gameplay calibration.

## 1. Principle

Keep **one** `TreeSpeciesDefinition` asset per species. Add only the parameters that a recommended species *needs* to behave differently in a way that changes a player decision. Group the new parameters, and never add a field "for completeness".

## 2. Trait model (Part 4)

### 2.1 Parameters that must be species-specific

| Group | Parameter (new ★ / existing) | Needed by | Evidence class (typical) | Note |
|---|---|---|---|---|
| Height | ★ **Site-index height curve** (Chapman–Richards b0..b3, or anchors at ages) replacing the linear law; Sitka keeps its Growth Model 1 curve as data | All Wave B conifers; credible strata in mixtures | EMP/TR (yield tables) | The single biggest realism gain after shade |
| Diameter | `PotentialDbhGrowth`, `MaxDbh` (existing) | All | TR/CAL | Keep |
| Competition response | `Ci50` (existing) | All | CAL | Keep. Shade tolerance is **not** encoded here alone |
| **Shade casting** | ★ **Crown light transmission** (0–1) or leaf-area class (light / medium / dense; deciduous flag) | Mixtures (beech/hemlock dense; birch/larch light) | TR/INF (literature on transmittance) | Changes `RecomputeCanopy` per tree |
| Shade tolerance (juvenile) | Light anchors: establishment / survival / growth (existing, opt-in distinct) | All | TR/MG | Already the right shape |
| Seed | Maturity onset/full, seed potential, mast (existing) | All | MG/TR | Keep |
| **Dispersal mode** | ★ Kernel family: wind-exponential (existing) / **bird long-tail** / heavy-local | Rowan, holly, cherry | INF/CAL | One enum plus a scale; not a bird model |
| Establishment | S50, initial height, density max (existing) | All | CAL | Keep |
| Juvenile growth | Regeneration height growth, promotion height/light (existing) | All | TR/CAL | Keep |
| Browsing | Palatability, vulnerability heights (existing) | All | TR (ordering), CAL (numbers) | Keep |
| **Longevity** | ★ Age at senescence onset + annual senescence hazard | Birch (short-lived), oak (long-lived) | MG/INF | Background mortality; currently absent |
| **Density** | ★ Max-density relationship (SDI slope, SDImax) or species group | Mixed-stand self-thinning | EMP (Sitka) / TR | Replaces Sitka-for-all (§5 G3) |
| **Site tolerance** | ★ Envelope per site factor (moisture, nutrients, exposure): optimal / tolerated / unsuitable | Species–site matching | MG (Native Forest Framework, site guides) | §3 |
| Storm | `StandWindSusceptibility` (existing); ★ optional rooting factor by wetness class | Later | TR/INF | Keep 1.0 until evidence |
| Timber | ★ Market/assortment set id, green density, form factor (existing `formHeightRatio`) | Productive species | EMP (prices) / MG | Data, not code (`TimberQualityEconomyRoadmap.md`) |
| Regeneration pathway | ★ Resprout capability (Wave C) | Hazel, cut birch | INF | Later |
| Presentation | ★ Tags: conifer/broadleaf, evergreen/deciduous, strategy (pioneer / tolerant / understorey) | UI, habitat, soundscape, residue | — | Replaces literal-id checks |

### 2.2 Parameters that should **not** be species-specific

| Parameter | Why shared |
|---|---|
| Hegyi cutoff (8 m) and formula | Calibrated structure. Per-species cutoffs would create asymmetric pair sets and break reconciliation |
| Model 2 vegetation competition response | Evidence does not separate species [Model2Handoff]; keep shared |
| Promotion as a band → individual handoff | Abstraction, not biology |
| Cell size / light grid | Engine scale |
| Shelter effectiveness | Equipment, not species (sizes later) |
| Storm event probability / severity | Site/scenario |
| Contractor rates / minimum | Economy, not species |
| Representation threshold 0.01 | Engine |

### 2.3 Precision rule

Every new numeric parameter carries its evidence class in its tooltip (the existing project pattern [A]/[B]/[C]/[D]). Where only an **ordering** is supported (e.g. "hemlock more tolerant than Douglas fir"), use ordinal classes (3–5 levels) mapped to numbers in **one** calibration table, not invented per-species decimals.

## 3. Site suitability (Part 5)

### 3.1 Factors (only those that change decisions)

| Factor | Classes | Why it matters | Source of truth |
|---|---|---|---|
| **Soil moisture regime** | dry / fresh / moist / wet (4) | Alder vs Douglas fir vs birch; storm rooting later | Scenario-authored per cell |
| **Nutrient regime** | poor / medium / rich (3) | Hazel/cherry/ash-type sites vs birch/pine; Native Forest Framework soil templates [R:SD] | Scenario-authored per cell |
| **Exposure** | sheltered / moderate / exposed (3) | Conifer choice, storm risk | Scenario-level (optionally per edge) |
| Light | Continuous (existing) | Already | Ecology |
| Browse pressure | Existing band | Already | Scenario |
| Elevation / climate | **Not modelled**; scenario-level choice of species pool | Avoids building a climate engine | Scenario |
| Soil stability | **Fold into moisture** (wet → unstable) | Avoid a 4th factor | — |

### 3.2 Where data lives

| Data | Home |
|---|---|
| Tolerance envelopes (per factor: optimal / tolerated / unsuitable) | **Species** asset |
| Per-cell moisture and nutrient class | **Site** data authored in the **scenario** asset (deterministic map; not saved, because it is constant) |
| Exposure, browse pressure, species pool, nursery offers, markets | **Scenario** asset |
| Multipliers for tolerated/unsuitable (growth, establishment, survival) | **Calibration** table (one place, CAL) |

### 3.3 Behaviour

`site multiplier = min over factors (envelope class → multiplier)`. It applies to:
- DBH and height potential (replacing today's constant `SiteProductivity` = 1);
- establishment;
- juvenile survival.

**Unsuitable planting is allowed.** The planting preview and the Work Plan say so ("Poorly suited: wet ground; expect slow growth and losses"). Outcomes then show it. This keeps the Game Brief principle that planting is not automatically beneficial, and teaches species–site matching without prohibition.

Scenario One gets a site map consistent with its current behaviour (Sitka at Class III, all cells "fresh / medium / moderate"). That makes the change behaviour-neutral for Scenario One until a scenario authors variation.

## 4. Mixed-stand implications

See `MixedStandDesign.md`. The trait model above is what those mechanics read.

## 5. Generalisation order (Part 12) — "generalise only when the next species/scenario needs it"

| # | Refactor | Current problem | Next concrete use | Files / subsystems | Migration risk | Save impact | Testing | Backward compatible? |
|---|---|---|---|---|---|---|---|---|
| G1 | **Species tags + remove literal ids** in UI/habitat/soundscape/residue/log/inspection | ~30 literal checks; new species silently fall back | Wave A (birch must not be "unmarketed broadleaf Sitka-like" in copy) | `ScenarioHabitat*`, `ScenarioSoundscape`, `ScenarioOneManager` (residue, log, broadleaf list), `TreeInspectionView`, objectives labels | Low | None | String/visual gates; Scenario One anchors unchanged | Yes |
| G2 | **Species shade casting** (`RecomputeCanopy` uses per-species transmission) | Species-blind light | Wave A (holly understorey, birch light crowns), and beech meaningfully | `ForestEcologyController.RecomputeCanopy`, marking forecast light, storm light context | **Medium**: changes light for existing species unless Sitka/oak/beech keep transmission 0 (= today) | **Model version** (behaviour), no new field if folded into a forest-model version (§6) | Lifecycle anchors for model 0/1 unchanged; new anchors | Yes, via versioning |
| G3 | **Species-aware density mortality** (additive relative density across species; species SDI or group default) | Sitka SDI applied to broadleaves | Wave A (mixtures with oak/beech/birch) | `ApplyAdultDensityMortality`, `SitkaGrowthModel` → species density data | Medium | Model version | Pure-Sitka runs bit-identical (sum reduces to the Sitka term); mixed fixtures | Yes |
| G4 | **Site data + tolerance envelopes** | `SiteProductivity` ≡ 1 | Wave A (birch/alder wet, holly podzol), Scenario 2 | `ForestEcologyCell`, scenario asset, `GrowAdults`, establishment, juvenile rules | Medium | **None** (scenario-authored constant map) | Scenario One neutral-map bit-identical | Yes |
| G5 | **Dispersal kernel family** (bird long-tail) | One exponential kernel | Rowan, holly | `ComputeSpeciesSeedRain` | Low | None | Seed-rain fixtures | Yes (default = exponential) |
| G6 | **Longevity / senescence hazard** | No age mortality | Birch | New step near density mortality | Low–Medium | Model version | Cohort survival fixtures | Yes |
| G7 | **Species height curve** (generalise `SitkaGrowthModel` into data) | Two height laws | **Wave B** (conifers) | `GrowAdults`, `SitkaGrowthModel` | Medium | Model version | Sitka curve bit-identical | Yes |
| G8 | **Timber: species assortment registry + generic adapter** | `CreateSitka`, Sitka-only adapter | Wave B (Douglas fir sawlog) | `TimberYieldConfiguration`, `Stage1EconomyDefaults`, `ScenarioOneEconomyAdapter`, inspection estimate | Medium (economy anchors) | None | Economy gates; Sitka quotes identical | Yes |
| G9 | **Resprouting pathway** | None | Wave C hazel | Regeneration bands (origin "sprout") | Medium | Possibly a band origin enum value (int; save-compatible) | Band fixtures | Yes |
| G10 | **Tree quality state** | None | Quality wave | `ForestTree`, save | Medium–High | **Version bump** (per-tree quality fields) | Save/load, yield | Yes, with defaults |

Order: G1 → G2 → G3 → G4 → G5 → G6 (Foundation + Wave A); G7 → G8 (Wave B); G9 (Wave C); G10 (quality wave).

## 6. Save and versioning (Part 16)

| Data | Status |
|---|---|
| Species identity per tree / cohort / juvenile | **Already persists** (string ids). New species need **no** schema change |
| Species parameters, site map, tolerance, markets | **Asset data**; not saved; must be version-stable (changing them changes behaviour of saved games, so treat like calibration: changes go with a model version) |
| Canopy light, competition, density, site multiplier | **Derived** on load |
| Model behaviour selection | Existing ints: `rngModelVersion`, `regenerationModel`, `growthModel`, `stormModel`. **Recommendation:** one new **`forestModel`** int (or extend `growthModel` semantics to "forest dynamics") covering G2/G3/G6/G7. That is **one** version bump for the whole Foundation, not one per refactor |
| Per-tree quality, standing-dead state, resprout stool identity | **Would require persistence** (quality wave; standing deadwood; coppice) |
| Reference Future v1 | **Frozen**: stays on the legacy model ids; no edits (D-042) |

**A version bump is genuinely necessary only** for: (1) the Foundation model selector (one int, if not folded into `growthModel`); (2) the quality wave; (3) standing deadwood. No unavoidable foundational migration was found, so the stop condition is not met.
