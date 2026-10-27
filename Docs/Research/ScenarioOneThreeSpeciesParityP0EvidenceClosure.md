# Scenario One Three-Species Parity P0 — PARITY-02 Evidence Closure Detail

**Status:** supporting decision record for `Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md`
**Branch / inspected HEAD:** `research/scenario-one-three-species-parity-p0-evidence-closure-v1` @ `72a2a2a`
**Candidate lineage inspected:** `integration/scenario-one-tier3-candidate` @ `8f9ed80`
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`
**Nature:** read-only evidence/specification synthesis. No production, Unity, save or asset change.

This document holds the detailed reasoning behind the Q-01…Q-16 decisions. The handoff is the authority for the decision table; this file is the evidence trail.

---

## 1. Source map

| Source | What it establishes | Class / limit |
|---|---|---|
| `Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md` §2, §7, §8, §11 | No Irish Oak/Beech SDI/self-thinning; Oak/Beech growth envelopes are British/European transfer; final source gate “B — sufficient only for ordinal response calibration”; crown evidence is Sitka/birch transfer. | `TRANSFER` / `ORDINAL REPRESENTATION`; not a fitted coefficient set. |
| `Docs/Research/OakBeechAdultParity/OakBeechAdultGrowthSiteSpecification_v2.md` | Ten proposed growth values are calibration only; site axes must stay separate; neutral 1.0 reference; no active species×site curve; premium Oak normally beyond ordinary completion. | Research recommendation; final gate NO for changed growth rows; architecture retained. |
| `Docs/Research/OakBeechAdultParity/OakBeechCalibrationPreregistration_v1.md` | Pre-registered search grid and guards; values are [C] search ranges; no empirical coefficient claim. | `GAMEPLAY CALIBRATION`. |
| `Docs/Research/OakBeechAdultParity/OakBeechOrdinalGrowthCalibrationDesign_v1.md` | Corrected Štefančík 2013 reading; rejects doubled-height and column-confused Beech values; accepts release direction, not magnitude. | `TRANSFER` / `ORDINAL REPRESENTATION`. |
| `Docs/Verification/Calibration/OakBeechProductionCalibrationV1Handoff.md` (commit `ea52050`) | Grid-centre values: Oak 0.8/0.65/120/42.5/5; Beech 0.9/0.75/120/42.5/6.5; labels [C]; 360 checkpoints; not Unity-validated. | `GAMEPLAY CALIBRATION`; selection record, not biological validation. |
| `Docs/Research/OakBeechTimberEconomy/OakBeechTimberEconomyEvidence_v1.md` (commit `2e6e836`) | Oak five-rung ladder, Beech two-rung ladder; Teagasc direct Irish “typical” prices; quality-state recommendation; no Beech veneer/beam; price deck recommendation; roadside basis [C]. | `DIRECT` (Teagasc) + `GAMEPLAY CALIBRATION` (selection/basis) + `FALLBACK-CONSERVATIVE` (no form state). |
| Teagasc, *Felling, presentation and grading of hardwood timber* (re-fetched 2026-10-27) | First-grade Oak >2.7 m / 45 cm mid-diameter / clean; Oak beam >2.5 m / 30 cm; Oak fencing ~€100/m³; second-quality planking ~€60/m³; first grade €180–280/m³ “or roughly per tonne”; standing vs roadside statements. | `DIRECT` Irish trade guidance; “typical” not a tariff; basis of quotations unstated. |
| `Docs/BrowsingProtectionV1.md` | Palatability ordering Oak > Beech > Sitka [E transferred]; numbers [S]; shared 0.9 increment loss and 0.04 extra mortality [C]; persistent form damage deferred. | `TRANSFER` ordering + `GAMEPLAY CALIBRATION` numbers. |
| `Docs/BeechSpeciesParameters.md`, `Docs/BeechRegeneration.md`, `Assets/ForestPrototype/Docs/SessileOakV1.md` | Beech shade-tolerant light response; Oak distinct survival/establishment curves and promotion light 0.2; shared juvenile growth/promotion; Beech regeneration enabled. | `TRANSFER` / `ORDINAL` / `GAMEPLAY CALIBRATION`; no Beech-specific survival curve. |
| `Docs/Verification/ThreeSpeciesProgramme/Phase1-DensityRework.md`, `CompositeDensityV3Handoff.md`, `CompositeDensityV3Predeclared.md`, `CanopyPulseRootCause.md` | Candidate density evolution; v3 component route; pure-Sitka exactness claims; generic response calibration; known Beech swing limitation. | Candidate evidence; not re-run here; claims accepted only as candidate evidence. |
| `Docs/Verification/ThreeSpeciesProgramme/Phase6-BroadleafTimber.md` | Candidate broadleaf ladders/prices/quality model; price basis open; premium Oak “close to unreachable” under old growth. | Candidate evidence; premium-Oak conclusion superseded by later growth calibration (§5). |
| `Docs/Project/decision-log.md` D-059/D-061/D-062/D-065 | Product authority: comparable depth, ordinal density, no DEC/DMC, broadleaf timber layer, MSFM Sitka-only. | Accepted decision authority. |
| `Docs/Project/unity-project-overview.md` | Current anchors, save versions, growth models, canopy/competition constants, pruning and timber architecture. | Verified implementation facts (as recorded). |

---

## 2. Candidate baseline actually inspected

### 2.1 Growth/density

- `SitkaGrowthModel.GrowthModel` = {Legacy 0, SiteClassDensity 1, SpeciesAware 2, Latest 2} (`SitkaGrowthModel.cs:16–19`).
- `ScenarioOneManager.NewGameGrowthModel = GrowthModel.SpeciesAware` (`ScenarioOneManager.cs:495`); new games therefore use model 2. Old saves keep their saved model.
- `ForestEcologyController.ApplyAdultDensityMortality` (`:1040` onward) selects:
  - `LegacySitkaExact` for models 0/1 and for model 2 with only Reference (Sitka) living trees;
  - `SpeciesAwareOccupancy` when any living tree has a non-Reference class (`:1056–1059`, `HasNonReferenceDemand :1141`).
- The species-aware route (`:1158`) builds `DensityStem` records and calls `SitkaSubsetDensitySignal.Resolve` plus `MixedDensityModel.PlanComponents`. The Sitka component is the original Sitka density process on Sitka stems only; the generic component applies a species-neutral occupancy response to the remaining stems; victims are unioned.
- Class weights: Reference 1.0, Intermediate 1.5, High 2.0 (`MixedDensityModel.cs:138–140`).
- Occupancy normalization 55 (`MixedDensityModel.cs:144`); the v3 generic response uses strength 1.60, maximum loss 0.50, exponent 2, basal-area budget 0.125 (`MixedDensityModel.Composite.cs:77–96`). The single-quota primitive still has `MaximumAnnualLoss = 0.10` (`MixedDensityModel.cs:148`), but the controller uses `PlanComponents`, not the primitive.
- `Species/SessileOak.asset`: `growingSpaceDemand: 2` (High). `Species/BeechSpecies.asset`: `growingSpaceDemand: 1` (Intermediate). Sitka has no field and defaults to Reference.
- Pure-Sitka fingerprints and isolated compiles are claimed in the v3 record; this task did not re-run them.

### 2.2 Adult traits and site

- Oak asset: Pd 0.8, Ph 0.65, Dmax 120, Hmax 42.5, Ci50 5.
- Beech asset: Pd 0.9, Ph 0.75, Dmax 120, Hmax 42.5, Ci50 6.5.
- Sitka asset unchanged: Pd 1.2, Ph 0.45, Dmax 120, Hmax 35, Ci50 5.
- `GrowAdults` applies DBH competition via `1/(1+CI/Ci50)`; height has no competition term; Sitka under model ≥1 uses the Class III height envelope; Oak/Beech use the legacy asymptotic height law (`ForestEcologyController.cs:1007–1024`).
- `SpeciesSiteResponse` is dormant: `NeutralGrowthMultiplier = 1f`, `NumericallyActive => false`, `GrowthMultiplier` returns 1.0 (`SpeciesSiteResponse.cs:87–115`). `ScenarioSiteProfiles.ScenarioOneReference` is a versioned Reference identity (`ScenarioSiteProfiles.cs:14–27`).
- Canopy: `HegyiCutoffMeters = 8f`, `CanopyShadeReachPerCrownRadius = 1f` (`ForestEcologyController.cs:905, 911`); lateral reach is `CrownRadius * 1 + cellSize*0.5`; vertical factor `clamp(H/8)`; no species opacity.

### 2.3 Regeneration, juveniles, browse, pruning

- `JuvenileEcologyRules.SurvivalResponse` uses `species.UsesDistinctJuvenileLightResponses`; Oak opts in, Beech and Sitka do not (`JuvenileEcologyRules.cs:14–20`). Beech uses the combined legacy rule (`LightResponse < RegenPoorLightThreshold ? 1 − RegenMortalityUnderPoorLight : 1`).
- Oak asset has `useDistinctJuvenileLightResponses: 1`, survival and establishment anchors, promotion minimum light 0.2. Beech asset has no distinct flags; promotion minimum light 0.
- Shared juvenile values: 0.35 m/yr, promotion 3.5 m, `regenDensityPerEstablishment 0.5`, `regenDensityMax 1.5`, `regenMortalityUnderPoorLight 0.2`, `regenPoorLightThreshold 0.15`.
- Browse ordering: Oak 1.0, Beech 0.5, Sitka 0.15; broadleaf full vulnerability/escape 1.2/1.8 m; Sitka 0.5/0.8 m (`BrowsingProtectionV1.md`; assets).
- Planting stock: `plantedJuvenileAgeYears = 3`, `plantedJuvenileHeightM = 0.6`, `plantedJuvenileDensity = 1f`, header “Planting (Beech v1)” (`ForestEcologyController.cs:29–35`).
- Pruning: shared `MaxPruningLifts = 3`, `CrownRadiusReductionPerLift = 0.08`, `MinimumYearsBetweenLifts = 5` (`ForestTree.cs:181–188`); shared lift targets `{2.5, 5, 6.5}` with tooltip “following common Sitka clear-stem practice” (`ScenarioOneDefinition.cs:70–71`).

### 2.4 Timber and economy

- `TimberYieldDefaults.CreateScenarioOne()` = `CreateSitka()` plus `BroadleafSpecifications()` (`TimberYieldConfiguration.cs:77–83`).
- Oak: veneer/planking priority 10, midline 450 mm, length 2.8 m, required `Pruned`, excluded any defect; beam 20, midline 300 mm, 2.6 m, structural exclusions; fencing 30, small end 120–250 mm, 1.8/2.4 m; second planking 40, small end 250 mm, 2.4 m; firewood 50, small end 70 mm, 2.0 m (`:103–117`).
- Beech: second planking 10, small end 250 mm, 2.4 m, sound; firewood 20, small end 70 mm, 2.0 m (`:119–123`). No Beech veneer/beam/fencing.
- Prices: Oak veneer 180/230/280 €/m³, beam 145, fencing 100, second 60, firewood 20/30/40 [C]; Beech second 60, firewood 20/30/40 [C] (`Stage1EconomyDefaults.cs:87–104`). The helper converts €/m³ to €/t at 890 kg/m³ (`CentsPerTonneAtProxy :128–132`); this is a unit convention, not a density.
- Non-firewood prices are labelled `EvidenceLabel.Inference`; firewood `GameplayCalibration`. The direct Teagasc source supports `ManagementGuidance` for the former.
- `ScenarioOneEconomyAdapter` turns the broadleaf market on at model 2 (`BroadleafMarketActive`), markets species with a price entry, and uses the Sitka 890 work proxy for all species (`:119–122`). The dormant flat `broadleafTimberValueCentsPerCubicMetre = 6500` remains in `ScenarioOneDefinition.cs:34–35` and `ScenarioOne.asset:23–24`; no production caller.
- Quality: `PrunedSections` supplies the clear section from recorded pruning; no fork/defect state. UI says “size and form” and “large and straight enough” (`TreeInspectionView.cs:323, 330`), which overstates the model.

### 2.5 Persistence

- Candidate save version 20 (`ForestSaveData.CurrentVersion = 20`).
- Active Rack V2 base `c6342f79` is save v21 (rack planning/field-marking); `8f9ed80` is its ancestor.
- `ForestSaveValidation` accepts `growthModel` 0…`GrowthModel.Latest` (=2) (`ForestSaveValidation.cs:80`).
- Unknown saved species: `ForestSaveController` resolves the registry; if missing, it logs a warning and loads `spawner.DefaultSpecies` (Sitka) (`:382–386`). There is no rejection.
- `ScenarioTimberSale` stores assortment + volume + revenue, no species (`ForestSaveData.cs:111–115`); `TimberAssortment` appends VeneerPlanking, Beam, Fencing, SecondPlanking, Firewood after the four Sitka values (`WorkEconomy/ForestryWorkDomain.cs:11`).

---

## 3. Q-by-Q evidence notes

### Q-01 — Density classes and pure-Sitka routing

**Decision:** 3 classes, Reference 1.0 / Intermediate 1.5 / High 2.0; Sitka Reference, Beech Intermediate, Oak High; Oak > Beech provisional ordinal.

**Why:** the evidence supports a species-aware ordinal representation, not an Oak/Beech maximum-density equation. The composite §2 records no Irish Oak/Beech self-thinning/SDI and no quantitative density model for Beech; the only defensible statement is ordinal. The candidate’s own Phase 1 record says “Oak above Beech is the weakest-supported call”; the composite also notes Beech “likely low maximum SDI”, which could imply the reverse. The weights 1.0/1.5/2.0 and the occupancy normalization/response are simulation calibration, not measurements. The 3-class architecture is retained because it is implemented and testable; the Oak/Beech ordering is labelled provisional (U-01).

**Pure-Sitka definition:** “100 % Sitka” = every **living individual tree** (adult or promoted juvenile) has `GrowingSpaceDemand.Reference`. Dead stems, stumps and sub-promotion regeneration cohorts are excluded. This matches `HasNonReferenceDemand(living)` and `SelectRoute`. A single living Oak/Beech switches the whole stand to the component route. The route is a regime change by design; the v3 record claims continuity at 99.9 % composition (total loss within 0.005, no step >0.02 for a 0.001 composition step) and no cliff. Re-qualify under Unity before acceptance.

**Candidate:** keep architecture/weights; label the ordering. No per-species capacity, DEC, DMC or mixture bonus.

### Q-02 — Victim selection / Ci50

**Decision:** species-neutral victim selection; one common half-saturation 5 and exponent 4; `Ci50` only growth/crown/suppression diagnostics.

**Why:** using each species’ own `Ci50` in mortality double-counts the growth response and produces the old 28σ removal bias against broadleaves (candidate Phase 1). There is no differential-mortality evidence for Oak/Beech. A common half-saturation is a calibration; it decides who dies, never how many. `Ci50` still affects DBH growth (`:1012`), crown target (`:1220`), recorded suppression (`:1022`) and UI forecasts (`ForestTreeMarkingManager.cs:404, 420`; `UI/CompetitorAssessment.cs:160`), so changing it is not a density-only change (H-01).

### Q-03 — Adult growth

**Decision:** keep the candidate values as `GAMEPLAY CALIBRATION`; keep the legacy height law; do not claim empirical coefficients.

**Why:** the composite §7 says the growth evidence is TRANSFER, mostly European managed stands, with a final gate of “B — sufficient only for ordinal response calibration”. The research v2 says none of the ten exact values is established as a fitted coefficient; the production calibration selected grid centres and labelled them [C]. The direction “Beech faster/taller early, Oak more light-demanding” is supported; the exact numbers are not. The values satisfy the pre-registered hard guards and provisional height bands (Oak distance 0.0; Beech 0.188). The legacy height law is a coarse saturating trajectory; a new age/site envelope for broadleaves would be a model change without evidence. Keep it and label the limitation.

**Premium Oak correction:** the Phase 6 “close to unreachable” statement used the pre-calibration 0.4 values. The arithmetic in the handoff §5.1 (candidate kernel, site 1, CI 0/2, promoted at 3.5 m/5.25 cm) shows Oak at ~57 cm DBH / 32.8 m at age 100 with CI 0, and ~45 cm / 32.8 m with CI 2. A pruned, low-competition Oak can therefore plausibly reach a 45 cm mid-log late in a century run; it is rare, late and not guaranteed. Correct the teaching, not the growth.

**Beech height caveat:** the same arithmetic gives Beech ~23.4 m at age 50 and ~34.6 m at age 100, below the provisional 25–30/35–40 bands; this is the 0.188 distance. Accept as a labelled calibration limitation (U-11), not a reason to invent a new curve.

### Q-04 — Species × site

**Decision:** no active multiplier; dormant neutral infrastructure.

**Why:** the research v2 says the canonical site has no identity in the data/docs, the existing `SiteProductivity`/`SoilStability` are 1.0 everywhere in production, and no species×site magnitude is evidenced. The site descriptors have supported ordinal directions (Oak freer-draining; Beech moist/well-aerated; both drought/waterlogging limitations) but no numbers. The candidate’s `SpeciesSiteResponse` returns exactly 1.0 and is not read by the simulation. Activating it at a uniform 1.0 site would be inert; activating a non-neutral value would invent magnitude. Keep dormant, and do not repurpose the existing cell scalar. If a future site identity is added, it needs its own calibration/version.

### Q-05 — Canopy / light

**Decision:** shared equation, shared implicit opacity 1.0; species differences only via crown radius/relaxation; document the limitation.

**Why:** the repository has no species-specific light attenuation or deciduous-season evidence. The k10a10/canopy calibration is anchored in hashes. Adding an opacity field defaulted to 1.0 would be inert; adding species values would be invention. Crown radius already differs (Oak linear, Beech power law), which is the defensible species effect. State that shade cast is not species-differentiated.

### Q-06 — Competition radius / Ci50

**Decision:** keep 8 m shared cutoff and species Ci50 as calibration.

**Why:** the 8 m Hegyi cutoff is Sitka-calibrated (audit). No broadleaf neighbourhood-radius evidence exists. The Oak/Beech Ci50 values are grid-centre calibration, not measured competition tolerances. They remain species-specific because they set the DBH response and crown target; they are not used for density victim selection (Q-02). Label both. The cross-system coupling is real: Ci50 changes growth, crown size/light, suppression diagnostics and UI, not just one number.

### Q-07 — Regeneration / juveniles / mast

**Decision:** keep Oak distinct curves; keep Beech combined legacy response; keep shared juvenile growth/promotion/density and shared mast probabilities; label.

**Why:** Oak’s distinct survival/establishment curves are a species-specific facility (Oak asset `useDistinctJuvenileLightResponses: 1`; promotion light 0.2). Beech is shade-tolerant (its light-response curve reaches 1.0 at ≥35 % light, with a 0.15 response at 5 %), but no Beech-specific survival/establishment curve exists in the evidence. The combined legacy rule gives high survival above the 15 % light-response threshold and 20 % annual loss below it; this is a conservative simplification, not an empirical Beech curve. Do not invent one. Shared 0.35 m/yr, 3.5 m promotion and density parameters are calibration; Oak/Beech do not have evidence for different values. Mast probabilities 0.15/0.35 are shared calibration; species multipliers/dispersal/S50/maturity already differ. The Oak-vs-Beech structural asymmetry is therefore intentional and documented, not a hidden Sitka inheritance.

### Q-08 — Nursery stock

**Decision:** shared 3 yr / 0.6 m / density 1.0; relabel “Beech v1” to “broadleaf stock abstraction v1”.

**Why:** the evidence pack gives nursery prices, not species-specific stock age/height/density. The current constants are [B]/[C] and are applied to Oak. A shared labelled abstraction is more honest than inventing per-species values. The 3-year age is evidence-derived for bare-root/container stock; 0.6 m is a simulation abstraction; density 1.0 is a representation choice.

### Q-09 — Browsing / protection

**Decision:** keep ordering Oak 1.0 > Beech 0.5 > Sitka 0.15; keep escape heights; keep shared 0.9/0.04; no recovery/resilience difference; no persisted form damage.

**Why:** the browsing report supports the qualitative ordering as transferred evidence; the numbers are simulation abstraction/calibration. The report explicitly leaves recovery/resilience undifferentiated and defers persistent form damage. Protection geometry is shared and species enters only through height vulnerability. This is adequate for P0 and should be labelled, not “improved” with invented species coefficients.

### Q-10 — Timber assortment

**Decision:** Oak five rungs, Beech two rungs; size + pruning only; no fork/defect state; Oak veneer/beam dimensions direct; other dimensions [C]; label the pruning-only proxy.

**Why:** Teagasc directly supports Oak veneer/planking (>2.7 m, 45 cm mid-diameter, clean), beam (>2.5 m, 30 cm), fencing quality (~€100/m³), and second-quality planking (~€60/m³, oak/beech/ash/sycamore/Norway maple). Beech is absent from the first-grade list; it should not inherit Oak’s veneer/beam/fencing. The candidate’s ladder matches this. The only authoritative live quality signals are size and pruning/crown history; there is no fork/sweep/defect/browse-form state on living trees, and the task forbids inventing one solely for pricing. The veneer `Pruned` requirement is a pruning-only proxy, not a form grade. Player copy must say so.

**Premium Oak:** with the calibrated values, a low-competition pruned Oak can plausibly reach veneer size late; the earlier “unreachable” statement is stale. Do not lower the 45 cm threshold or accelerate growth; teach it as a long-rotation, low-competition, pruning-dependent outcome.

### Q-11 — Economics

**Decision:** keep Teagasc typical prices; roadside [C]; selected reference [C]; 890 work proxy; no flat 6,500; no price deck in P0.

**Why:** Teagasc states the prices as “typical prices quoted” per m³, with first grade “or roughly per tonne”. The candidate treats them as €/m³ and converts to €/t at 890 kg/m³; because the price is per m³ and the engine needs per tonne, the conversion preserves €/m³ revenue. The 890 is a unit convention, not a density. The source does not state standing vs roadside; the evidence pack recommends roadside to match Sitka, and the candidate does that. That is a `GAMEPLAY CALIBRATION`/consistency choice, not an empirical basis. No Irish broadleaf harvest-rate series exists; keep the generic cost. Firewood is a [C] fallback; no stumpage evidence. The dormant flat 6,500 c/m³ is the exact “flat broadleaf rate” D-062 forbids; remove or block. A time-varying price deck is recommended by the evidence pack but is P1; the `EconomicParameter` Low/High already exposes the range.

**Label correction:** the Teagasc non-firewood prices are `DIRECT`/`ManagementGuidance`, not `Inference`. The selected values remain `GameplayCalibration`.

### Q-12 — Pruning / Crop Tree

**Decision:** species-specific broadleaf high-pruning branch: 3.5 m and 6.0 m, two lifts, minimum 3 years; keep 8 % crown reduction and 60 % cap as [C]; Sitka schedule unchanged; no formative shaping in P0.

**Why:** Teagasc high pruning uses two lifts at 3.5 m and 6 m, 2–4 years apart, and distinguishes formative shaping (1–2 m, single leader, fork removal) from high pruning. The existing 2.5/5/6.5 m three-lift schedule is labelled “common Sitka clear-stem practice” and is not broadleaf evidence. Applying it to Oak/Beech is Sitka-derived. A broadleaf branch uses the existing `PruningLifts`, `CrownBaseHeightM`, `LastPruningYear` state and needs no new persisted field. The 3-year interval and 8 % crown loss are calibration; the 60 % height cap is existing. Formative shaping/fork permanence needs a new form state and is deferred (U-02).

### Q-13 — Evidence vocabulary

**Decision:** project class authoritative; runtime labels coarse; `Source`/comments carry the exact class; `UNSUPPORTED` blocks implementation.

**Why:** the project classes and the runtime vocabularies are not one-to-one. `[C]`/`[D]`/`[S]` can all map to `GameplayCalibration` or `SimulationAbstraction` depending on whether the item is a calibrated value or an ordinal/structural representation. `[A]` can be `Empirical` or `ManagementGuidance`. `[E]`/`[B]`/`[I]` can be `TRANSFER`, `MODEL DERIVATION` or `ORDINAL REPRESENTATION`. A binding many-to-one table is in the handoff §7. The runtime `EvidenceLabel` is not persisted, so extending it is save-safe, but do not extend it just to encode research classes. Fix the Teagasc price labels to `ManagementGuidance`.

### Q-14 — Persistence / gating

**Decision:** keep `SpeciesAware = 2`; new games 2; old saves 0/1 unchanged; no parity-specific field; append-only enum; species-less sale records for P0; reject unknown species.

**Why:** the candidate already persists the model number and uses it for density and the broadleaf market. The active Rack V2 base is save v21; parity adds no field. Old saves on 0/1 keep old behaviour; no silent acquisition. `TimberAssortment` appends values after the four Sitka values; old builds reject new values (intended). `ScenarioTimberSale` is species-less; Oak and Beech share `SecondPlanking` and `Firewood` with the same selected prices, so revenue is unambiguous in P0. If prices diverge or species-level reporting is required, add species at the next schema (G5). Unknown species currently load as Sitka; that mutates meaning and must be rejected. `ForestSaveValidation` already rejects growth models outside 0…2.

### Q-15 — Anchors

**Decision:** see handoff §9. Pure-Sitka lifecycle/reference archive/Legacy40/pure-Sitka density/Sitka timber exact; broadleaf-containing completion/economy/model-2 anchors authorised-change candidates; continuation diagnostic.

**Why:** the audit’s anchor partition still holds. Pure-Sitka fixtures contain no broadleaves; completion fixtures do. A pure-Sitka density change can be proved by the pure-Sitka anchors, but completion-level behaviour cannot. The Reference archive bytes are frozen; the continuation hash is diagnostic under D-042. New model-2 anchors must be labelled and approved.

### Q-16 — Wind exposure

**Decision:** when storms are off, show “Storms are off in Scenario One; wind exposure is not modelled for any species”; do not show a band. When storms are active, label the shared model and the absence of species multipliers.

**Why:** all species wind multipliers are 1.0; storms are off in new Scenario One (`ScenarioOneManager.cs:473`). The current line `Stable/Watch/Exposed · storms off` (`ForestEcologyController.Storms.cs:135–136`) looks species-specific and is not. The fix is wording/visibility, not a new wind model.

---

## 4. Arithmetic and unit checks

### 4.1 Adult trajectory

Kernel: `ΔD = Pd × site × clamp01(1 − D/Dmax) × 1/(1 + CI/Ci50)`; `ΔH = Ph × site × clamp01(1 − H/Hmax)`; site 1; start 5.25 cm / 3.5 m at age 10 (promoted stock); no browsing or mortality; annual steps.

| Species | CI | Age 50 | Age 100 | Age 112 |
|---|---:|---|---|---|
| Oak | 0 | 32.2 cm / 21.4 m | 57.2 cm / 32.8 m | 62.0 cm / 34.4 m |
| Oak | 1 | 28.2 / 21.4 | 50.5 / 32.8 | 55.0 / 34.4 |
| Oak | 2 | 25.2 / 21.4 | 45.3 / 32.8 | 49.5 / 34.4 |
| Beech | 0 | 35.1 / 23.4 | 61.7 / 34.6 | 66.8 / 36.2 |
| Beech | 1 | 31.6 / 23.4 | 56.2 / 34.6 | 61.0 / 36.2 |
| Beech | 2 | 28.8 / 23.4 | 51.6 / 34.6 | 56.2 / 36.2 |

These are model derivations, not simulation output. The candidate’s production calibration record reports similar age-25 ranges (Oak 9.8–14.7 cm; Beech 11.1–16.1 cm over dense/moderate/strong competition). The arithmetic is used only to correct the stale premium-Oak statement and to show the Beech height band miss.

### 4.2 Price conversion

`CentsPerTonneAtProxy = cents_per_m³ × 1000 / 890`. This preserves €/m³ revenue: `price_per_t × 0.89 t/m³ = price_per_m³`. Example: 230 €/m³ → 25,800 c/t ≈ €258/t → €229.6/m³ after rounding. The 890 is a work-equivalent proxy, not a measured Oak/Beech green density.

### 4.3 Occupancy

`occupied space = class weight × π/4 × DBH_m²`; `occupancy = Σ occupied space / area / 55`; pressure `max(0, 1 − 1/occupancy)`. The class weight is the only species input; the response is species-independent. The v3 component route uses the generic response `Lmax (1 − exp(−k (o−1)^m))` with `Lmax 0.50`, `k 1.60`, `m 2`, and a basal-area budget 0.125. These are [C]; no per-species capacity.

---

## 5. Evidence conflicts and corrections

1. **Old Phase 6 premium-Oak conclusion.** “Close to unreachable” was based on the pre-calibration 0.4 cm/yr values; the calibrated 0.8/0.65 values change the arithmetic. Correct the statement; do not change growth/thresholds to force or prevent premium.
2. **Price labels.** Teagasc is direct Irish trade guidance; `Inference` is too weak. Use `ManagementGuidance` for the range, `GameplayCalibration` for the selected reference.
3. **Quality wording.** The model has no form/straightness state. “Size and form” and “straight enough” overstate it. Say size + pruning history; fork/defect shape not modelled.
4. **Beech height bands.** The candidate values sit slightly below the provisional bands at 50/100. This is a calibration limitation, not an empirical rejection; do not invent a new height model.
5. **Oak/Beech density ordering.** The evidence does not establish Oak > Beech; the candidate itself calls it the weakest call. The composite’s “Beech likely low maximum SDI” could imply the reverse. Keep it provisional or collapse to one class.
6. **Site axes.** The existence of ordinal directions does not justify a numerical multiplier. Keep neutral/dormant.
7. **Price deck.** The evidence pack recommends a deck; P0 can use a labelled reference with Low/High, but the selected value is not a constant market truth. Do not present it as such.

---

## 6. Candidate verification status (not re-run here)

| Claim | Source | Status for this task |
|---|---|---|
| Pure-Sitka model 2 equals model 1 exactly; legacy route only | `CompositeDensityV3Handoff.md`; `MixedDensityVerification` | Candidate evidence; not re-run. |
| Density v3 Unity checks (91 offline, 61 component, 33/33 gate, pure Sitka identities) | `CompositeDensityV3Handoff.md` | Candidate evidence; not re-run. |
| `BroadleafAdultGrowthVerification` 14/14 | v3 handoff | Candidate evidence; not re-run. |
| `BroadleafTimberEconomyVerification` 29 | v3 handoff | Candidate evidence; not re-run. |
| 24-gate regression / Phase-0 anchors | v3 handoff | Candidate evidence; not re-run. |
| Model-2 Scenario One completion/economy/long-run | Phase 5 runbook; not clearly run | **Unverified**; PARITY-09 must measure. |
| Save v20/v21, unknown species, sale records | code inspection | Code facts; no runtime test. |
| Teagasc prices/basis | primary page re-fetched 2026-10-27 | Verified text; “typical” not a tariff; basis unstated. |

---

## 7. Open items cross-reference

The handoff §6 lists U-01…U-12. The most material are U-01 (Oak/Beech ordering), U-02 (fork/form state), U-03 (Beech curves), U-11 (Beech height band) and U-12 (premium Oak teaching). None changes the matrix; each needs a Manager choice or a later evidence packet.
