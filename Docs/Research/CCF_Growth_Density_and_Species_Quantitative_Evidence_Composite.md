# CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite

STATUS:
SUPPORTING RESEARCH — NOT DECISION AUTHORITY

V1B EVIDENCE UPDATE (23 October 2026):
Broad & Lynch 2006a is the primary model-development/calibration source; Broad & Lynch 2006b is the
primary empirical source-data / panel-data validation study. 2006b supports empirical diagnosis of site-index
representativeness, experimental dependence and cross-sectional volume-data behaviour. It does **not**
independently validate final GROWFOR dynamic forecasts. Classification: **empirical source-data validation
confirmed; final growth-model predictive validation not demonstrated.**

PURPOSE

A single project-facing retrieval document for quantitative growth, density/self-thinning, growing-space,
species-equivalence and crown/site evidence previously held across several research PDFs. It preserves the
distinction between empirical and transferred evidence, keeps source-specific equations, units, domains and
caveats, and does not promote any coefficient into gameplay. The Decision Log remains the authority.

Cross-references (retained individual, not duplicated):
- `GROWFOR_Technical_Evidence_Extraction_v1.md` holds the full BroadLynch2006_Sitka equations, coefficient
  matrices, volume/thinning/assortment coefficients, validity limits, the V1B 2006a/2006b evidence-role
  fields and G3/G4 NFI statistics.
- `CCF Species Evidence Pack v1 ...pdf` holds the species dossiers, GROWFOR coverage boundary and roster.
- `CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf` holds the four-paper pointer with
  DOIs (Conry1989, Dunbar2002, Moore2000, Pretzsch2019).

SOURCE MANIFEST

| # | Filename | Original title | Date | Pages/sections used | Original study/reference where supplied | DOI/URL | Evidence geography | Evidence class |
|---|---|---|---|---|---|---|---|---|
| G1 | Species-Specific Density.pdf | Species-Specific Density (multi-species architecture) | undated (upload Oct 2026) | pp. 1–12 (whole) | — (architecture synthesis) | none | project/architecture | SUPPORTING RESEARCH |
| G2 | MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | Multi-Species Density & Mixed Occupancy Analysis | undated (upload Oct 2026) | pp. 1–5 (whole) | Jones et al. 2026 (For. Ecol. Manag. 260:124163); Broad & Lynch 2006; Reineke 1933; Pretzsch & Zenner 2017; Black 2015 | none in file | mostly absent Irish data; transfer/modelled | SUPPORTING RESEARCH |
| G3 | height and diameter growth of Sessile oak and European beech.pdf | Height and diameter growth of Sessile oak and European beech | undated (upload Oct 2026) | pp. 1–5 (whole) | Štefančík et al. 2013 (Slovak beech thinning); Cedro 2007; European reviews [20,26,30,31,44,79] | none in file | continental Europe transfer; no Irish data | SUPPORTING RESEARCH (transferred) |
| G4 | Irish_Sitka_CCF_Deep_Research_Report.pdf (growth/density/crown/site sections) | Irish Sitka Spruce Plantation Transformation to Continuous Cover Forestry (Deep Research Report) | 14 September 2026 | pp. 3–11 (sections 1, 3–11, 14, 17, 24) | Teagasc thinning/transformation guidance; Frenchpark thinning trial; British individual-tree Sitka model; Irish windthrow model; CSO/NFI | none in file | Ireland (production, Frenchpark, NFI); Britain transfer | SUPPORTING RESEARCH |
| G5 | CCF_Primary_Literature_Synthesis_v1.pdf (site/height/crown sections — source removed after distribution) | CCF Primary Literature Synthesis v1 | 5 October 2026 | pp. 3–5, 8–11 | S4 Lekwadi et al. 2012; S7 Tabbush & White 1988; S1 Davies & Pommerening 2008 | 10.1016/j.foreco.2012.07.003; 10.1016/j.foreco.2008.03.052 | Irish Sitka (Lekwadi); British Sitka (Tabbush, Davies) | SUPPORTING RESEARCH |
| G6 | CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf (density sections — source retained individual, cross-referenced) | CCF Academic Paper Consolidation — Comprehensive Evidence Edition | undated (upload 8 Oct 2026) | pp. 1–3 | Pretzsch & del Río 2020 (Forestry 93(1):1–15, doi:10.1093/forestry/cpz069); Conry & Clinch 1989; Dunbar et al. 2002 | 10.1093/forestry/cpz069; 10.1093/forestry/62.4.397; 10.1093/forestry/75.5.537 | Central European density experiments; Irish site/YC | SUPPORTING RESEARCH |

---

## 1. Density architecture (proposed, non-authoritative)

- Replace the one-size Sitka self-thinning rule with a species-aware density interface; mixed stands compute
  occupancy relative to each species' capacity, then combine into an equivalent density for competition
  effects. Preserve the existing single-species Sitka model as a compatibility case.
  [SOURCE: Species-Specific Density.pdf | pp. 1, 6]
  Class: SIMULATION ABSTRACTION.
- Proposed density modes: `LegacySitkaExact` (current Sitka SDI/RD); `SpeciesMaxDensity` (per-species max
  trees vs Dq); `MixedOccupancyV1` (occupancy = N / Nmax_species_at_site, then a stand-equivalent occupancy);
  `Unsupported` (no density model). DMC (mixture bonus) excluded in v1.
  [SOURCE: Species-Specific Density.pdf | pp. 6, 7, 12]
  Class: SIMULATION ABSTRACTION.
- Pure-Sitka compatibility rule (design inference [I], not a finding): when composition is 100% Sitka, the
  generalized framework must reduce exactly to the current calibrated Sitka response; new density logic must
  reproduce the old output when species ≡ Sitka. [SOURCE: Species-Specific Density.pdf | pp. 6, 12; CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | p. 3]
  Class: SIMULATION DESIGN INFERENCE [I/C].
- Keep shade casting and shade tolerance as separate traits (density/light coupling).
  [SOURCE: Species-Specific Density.pdf | pp. 6, 7]
- Site must not collapse to one number: separate moisture/drainage, fertility, exposure and rooting/stability
  axes; productivity = f(species, site). [SOURCE: Species-Specific Density.pdf | pp. 4–5, 9]
  Class: SIMULATION DESIGN.
- Species-data schema (non-executable): SpeciesDefinition {SpeciesID, GrowthModelFamily, GrowthParameters,
  DensityProfile, CrownProfile, JuvenileLightResponse, SeedProfile, SeedDispersalProfile,
  EstablishmentProfile, BrowseProfile, TimberQualityProfile, AssortmentProfile, MarketProfile,
  WindTraitProfile}; site schema {Moisture/Drainage, Fertility, Exposure/Climate, Rooting/Stability}.
  [SOURCE: Species-Specific Density.pdf | pp. 3–4]
- Versioning: introduce a DensityModel version; GrowthModel2 for expanded adult growth; Regeneration Model 3
  only if algorithm changes; StormModel2 for species/site wind. Version algorithm semantics, not data
  calibration. [SOURCE: Species-Specific Density.pdf | p. 9]

---

## 2. Density evidence for the target Irish species

- Very little quantitative density data exist beyond Sitka. In a long-term Sitka thinning trial, plot basal
  area climbed to ~55 m²/ha in untreated stands (Jones et al. 2026). NO published self-thinning or SDI
  equations were found for Scots pine, Douglas fir, Norway spruce, oak, beech, birch or alder under Irish
  conditions. [SOURCE: MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | p. 1]
  Class: IRISH DIRECT for the Sitka BA observation; RESEARCH GAP (unsupported) for the rest.

| Species | Density evidence status | Class |
|---|---|---|
| Sitka spruce | No published Irish N–Dq/SDI; British/Irish yield models implicitly use an unpublished self-thinning line; empirical BA ~55 m²/ha under minimal thinning | [A/QNT] for BA; [T/QNT] for Reineke |
| Scots pine | No Irish density limits/self-thinning; Reineke slope assumed; intercept unknown | [U/INS] |
| Douglas fir | No Irish model; NA/UK growth curves but no self-thinning line found | [U/INS] |
| Norway spruce | Introduced/minor; no specific density data | [U/INS] |
| Oak (sessile + pedunculate) | No Irish self-thinning/stocking data; only ordinal guidance | [U/INS] |
| European beech | No quantitative density models; likely low maximum SDI | [U/INS] |
| Downy birch | No stocking models/SDI; even-aged birch thins out quickly | [U/INS] |
| Common alder | Limited Irish experience; no density/SDI data | [U/INS] |
[SOURCE: MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | pp. 1–2]
Class: RESEARCH GAP / UNSUPPORTED.

- Placeholder only: Reineke's universal exponent −1.605 (`log10 N = −1.605 log10 D + k`), with k tuned to
  pure Sitka (e.g. LDPI = 100). Without empirical k-values this yields only ordinal guidance.
  [SOURCE: MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | pp. 1, 3, 4]
  Class: BROADER TRANSFER [T/QNT] with CALIBRATION.
- Mixed-species research (primarily NW European) shows mixtures often overyield monocultures by ~10–30%;
  but no simple DEC/DMC rules exist for this species mix. Treat mixed occupancy initially as an uncalibrated
  abstraction [T/I]. [SOURCE: MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | pp. 1, 3]
  Class: BROADER TRANSFER + SIMULATION ABSTRACTION.
- Mixed-occupancy assessment: (1) species-specific normalization by own SDI is conceptually sound but
  needs empirical k; (2) DEC factors: none found for these pairs, additive occupancy (DEC = 1.0) is a strong
  assumption [U/INS]; (3) DMC mixture effects: no simple quantitative rules [I/ORD]; (4) overyielding: use
  additive occupancy ≤100% initially, no cap below 100% without calibration.
  [SOURCE: MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | pp. 2–4]
  Class: UNSUPPORTED / CALIBRATION.
- "Do not implement yet": species-specific density curves; fixed mixing coefficients (no "birch counts as
  0.5 stems of spruce"); overstocking A-line caps for mixed stands; pure-Sitka recalibration.
  [SOURCE: MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | p. 4]
  Class: IMPLEMENTATION GUARD.
- Readiness (as stated): adult Scots pine and Douglas fir can be coded with transferred coefficients for S3
  subject to calibration; birch and alder should remain low-status/juvenile until density/yield relationships
  exist; oak and beech start as low-stature/juvenile components. [SOURCE: MULTI-SPECIES DENSITY & MIXED OCCUPANCY ANALYSIS.pdf | pp. 3–4]
  Class: SUPPORTING RESEARCH — note: the accepted Scenario Two roster (Sitka, oak, beech, Scots pine, downy
  birch, common alder) excludes Douglas fir, so this source's Douglas-fir recommendation is NOT current
  species-selection authority; it is retained as evidence only.

---

## 3. Density-equivalence (DEC) and density-modification (DMC) evidence

- Pretzsch & del Río is the only paper in the packet that directly develops maximum-density/self-thinning
  relationships and mixed-species density normalization. Framework: species-specific maximum-density lines →
  Density Equivalence Coefficients (DEC) translate unlike species onto a common occupancy scale; Density
  Modification Coefficients (DMC) separately capture mixture-induced change in maximum density.
  [SOURCE: CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | pp. 1–2]
  Class: BROADER TRANSFER [T] (framework).
- Maximum-density form `N = a·dbq^b` (species-specific); equal- and unequal-diameter DEC formulations; DMC
  definition; full 95th-percentile regression coefficients for **Norway spruce, Scots pine, Douglas-fir,
  European beech and sessile oak**; DMC sample sizes, means and standard errors for five analysed species
  combinations. [SOURCE: CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | p. 2]
  Class: BROADER TRANSFER — coefficients NOT to be lifted into CCF.
- Domain/limitations (must be preserved): data came from Central European long-term experiments, NOT Irish
  stands; the maximum-density analysis covered 64 experiments, 328 plots, 2,627 surveys; the paper states the
  available data were too limited to model DMC confidently as a smooth function of diameter and mixture
  proportion; mixed-stand density–growth and thinning-response information is scarce.
  [SOURCE: CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | p. 2]
  Class: RESEARCH GAP / CAVEAT.
- Conservative interim conclusion: a universal Sitka-derived maximum-density relationship for oak, beech and
  other unlike species is NOT supported by this evidence packet. DEC is a useful architectural principle;
  numerical European DEC relationships are not silently promoted to Irish game parameters. A universal
  mixture modifier is not justified. [SOURCE: CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | p. 3]
  Class: BROADER TRANSFER → SIMULATION DESIGN.
- Grossor inventory cross-check: the ordinary (now superseded) consolidation described DMC as "something to
  investigate rather than inserting a generic mixed-stand bonus". [SOURCE: CCF Academic Paper Consolidation.pdf | p. 2 — SUPERSEDED, retained only as provenance]

---

## 4. Irish Sitka site productivity and potential height

- Primary Irish calibration source: Lekwadi et al. (2012), Irish Sitka site classification and top-height
  growth. Dataset: 578 modelling plots, 4,190 repeatedly measured top-height trees, 11,421 paired
  top-height/age observations, plus an independent validation set of 1,120 paired observations; public plots
  across seven Irish forest regions supplemented with private-forest data.
  [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | p. 3 | original: Lekwadi et al. 2012, For. Ecol. Manage. 283:56–65, doi:10.1016/j.foreco.2012.07.003]
  Class: IRISH DIRECT (A1).
- Top height is a site-productivity surrogate, largely independent of stocking density and normal thinning
  intensity — supports keeping competition effects on individual height weak relative to DBH/crown growth.
  [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | p. 3]
  Class: IRISH DIRECT.
- Five Irish productivity classes (I very good → V very poor), polymorphic Chapman-Richards. Published age-30
  top-height anchors: I 27.6 m (±2.4%); II 23.3 m (±2.9%); III 20.4 m (±2.8%); IV 17.4 m (±2.9%);
  V 16.0 m (±2.1%). [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | p. 3]
  Class: IRISH DIRECT.
- Rounded fitted Table 3 parameters (b0, b2, b3) with model form `H(t) = b0·(1 − exp(−b2·t))^b3`:
  I (q .99) 58.371 / 0.024 / 1.247; II (0.95) 49.523 / 0.025 / 1.247; III (0.50) 34.228 / 0.042 / 1.563;
  IV (0.12) 28.272 / 0.051 / 2.134; V (0.02) 22.557 / 0.060 / 2.619.
  [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | pp. 3–4]
  Class: IRISH DIRECT — with QA WARNING: the rounded coefficients do not exactly reproduce the published
  age-30 anchors using only rounded values; treat published anchors as validation and do not freeze a
  production curve from rounded coefficients. [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | pp. 4, 10]
- Other anchors: MAI/CAI intersections 0.89, 0.82, 0.75, 0.58 and 0.45 m/yr for classes I–V; age of maximum
  mean annual top-height increment 17 yr (class I) to 29 yr (class V); first thinning ~15–17 yr on class I,
  ~25–29 yr on class V. These are management/growth anchors, not game triggers.
  [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | p. 4]
  Class: IRISH DIRECT.
- Broader Irish Sitka site range: age-30 site index roughly 4.5 m on wet, nutrient-poor sites to 28.9 m on
  very moist, nutrient-rich sites; growth declines with both moisture deficit and excessive wetness.
  [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 3]
  Class: IRISH DIRECT.
- GROWFOR Sitka uses a polymorphic Bertalanffy–Richards height model, site index = top height at age 30;
  fitted global parameters a = 47.0216, c = 0.4571, t0 = −5.0 yr, H0 = 0 m (749 trajectories, 225 plots).
  [SOURCE: GROWFOR_Technical_Evidence_Extraction_v1.md | §5 (source retained individual)]
  Class: IRISH DIRECT (model specification).

---

## 5. Individual-tree growth: DBH versus height

- DBH should be the principal competition-responsive growth variable. Irish thinning experiments provide
  stand-level validation: at Frenchpark after 29 growing seasons, mean DBH rose from 23.7 cm (unthinned) to
  29.8 cm (light), 31.1 cm (medium) and 32.4 cm (heavy thinning); mean tree volume rose from 0.44 m³
  (unthinned) to 0.85 m³ (heavy). [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 4]
  Class: IRISH DIRECT.

| Treatment | Stems/ha | Mean DBH | Mean tree volume |
|---|---|---|---|
| Light thin | 925 | 29.8 cm | 0.70 m³ |
| Medium | 761 | 31.1 cm | 0.76 m³ |
| Heavy | 616 | 32.4 cm | 0.85 m³ |
| Unthinned | 1,762 | 23.7 cm | 0.44 m³ |
[SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 4]

- Growth forms (source-specific equations, units preserved):
  `Delta DBH_i = G_D(age_i, DBH_i, site_i) × competitionMultiplier_i × vitality_i`;
  `Delta H_i = G_H(age_i, H_i, site_i)` (height MVP with at most a weak competition effect; British
  individual-tree model did not retain competition for height). Released trees become stouter and
  broader-crowned, not suddenly taller. [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | pp. 4, 10]
  Class: BROADER TRANSFER + SIMULATION ABSTRACTION.
- Competition: distance-independent stand BA is insufficient for CCF; use a distance-dependent Hegyi index
  `CI_i = SUM_j [(DBH_j / DBH_i) / distance_ij]` with bounded response
  `competitionMultiplier_i = 1/(1 + CI_i/CI50)`. Hegyi mechanism evidence-based; response function and CI50
  are abstraction/calibration. [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | pp. 4, 10]
  Class: SIMULATION ABSTRACTION over an established method.
- Crown development: potential crown envelope (British open-grown Sitka)
  `CrownWidth_cm = 188.30 + 15.27 × DBH_cm`; `PotentialCrownRadius_m = 0.9415 + 0.07635 × DBH_cm`
  (r = 0.93; DBH 7–80 cm after exclusion). Realized: `TargetRadius = PotentialRadius(DBH) ×
  crownCompetitionResponse(CI)`; `Radius_(t+1) = Radius_t + k × (TargetRadius − Radius_t)` with k ~0.1–0.2/yr
  as a starting range. [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 5; CCF_Primary_Literature_Synthesis_v1.pdf | pp. 4–5]
  Class: CLOSE TRANSFER (UK open-grown) + SIMULATION ABSTRACTION (relaxation).
- Light: Sitka is intermediate in shade tolerance; ~20% relative light intensity (RLI) is a useful threshold
  for satisfactory Sitka seedling growth; ~30 m²/ha overstorey BA can sometimes provide this, but stocking
  and arrangement matter (fewer larger trees transmit more light). Relative-light interpretation bands:
  <10% strong suppression/poor recruitment; 10–20% survival possible/slow; ~20–25% transition to adequate
  growth; ~50–60% strong juvenile growth; 80–100% high light but establishment not automatically maximal.
  [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 5]
  Class: BROADER TRANSFER (B).
- Seed/reproduction anchors: reproductive onset ~20–25 yr (source smooths: <20 ≈ 0; 20–30 rising; >30 mature);
  good seed years ~4–8-yr interval; `SeedPotential_i = maturity(age_i) × crown/vitality_i ×
  annualMastModifier_t`. Dispersal: ~80% of seed within 60 m of a mature ~20 m Sitka; exponential kernel
  `K(distance) = exp(−distance/λ)` with λ ≈ 20 m and a 150 m computational cutoff.
  [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | pp. 6, 10]
  Class: BROADER TRANSFER (B) + TRANSFER (C for cutoff).
- Establishment: `P(establishment) = (1 − exp(−SeedRain/S50)) × LightResponse × EstablishmentSuitability`;
  S50 calibration. Dense natural regeneration is common; UK guidance recommends respacing toward
  ~2,000–2,500 stems/ha on suitable stable sites (silvicultural target, not a biological constant).
  [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | pp. 6, 10]
  Class: BROADER TRANSFER + SIMULATION ABSTRACTION.
- Stem volume: `BasalArea = π · DBH² / 4`; `StemVolume = BasalArea × FormHeight`;
  Teagasc equivalent `Volume_m3 = DBH_cm² × 0.00007854 × FormHeight_m`. Keep biological m³ and gameplay
  wood units separate (conversion = gameplay calibration). [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | pp. 8, 10]
  Class: IRISH DIRECT (A/B timber measurement) + CALIBRATION (conversion).

---

## 6. Windthrow and stability (quantitative context)

- Irish windthrow model: top height, regional wind exposure, soil, ground preparation and thinning status are
  major predictors; particularly strong positive risk effects for gley and peat relative to brown earth. The
  published Irish equation predicts whether more than ~3% of stems in comparable stands are windthrown by a
  specified top height — it is NOT an annual individual-tree probability and must not be pasted into
  `ForestTree.windthrowChance`. [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 7]
  Class: IRISH DIRECT (stand-level) + CAVEAT.
- Recommended two-level model:
  `StandWindSusceptibility = f(topHeight, soilStability, regionalExposure, stormSeverity)`;
  `TreeWindRisk_i = StandWindSusceptibility × slendernessResponse_i × localOpeningResponse_i ×
  recentThinningResponse_i`; `slenderness_i = height_i / DBH_i`. Mechanism evidence-based; multiplier
  functions/strengths are SIMULATION ABSTRACTIONS until calibrated. [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 7]
- Irish transformation trials: crown and GDT treatments had higher mean slenderness than low thinning, with
  crown thinning highest (a stability indicator, not a failure-rate model); Fossy Hill (wetter) had windfall
  while Ballycullen (deep, drained, sheltered) had practically none, and treatment effects could not be
  separated cleanly from site. [SOURCE: Irish Continuous-Cover Forestry Transformation Evidence...pdf | pp. 2, 17, 25]
  Class: IRISH DIRECT (Q2/Q4).
- Site matrix (Sitka, productivity vs stability kept separate): freely draining mineral/brown earth (high
  growth, good stability); moist mineral (high growth, moderate-good stability); wet mineral/gley (growth can
  be high, poor/shallow rooting); podzol (moderate, nutrient-dependent); peaty gley/shallow peat
  (moderate-low growth, poor stability); deep/blanket peat (often poor growth, very poor stability).
  [SOURCE: Irish_Sitka_CCF_Deep_Research_Report.pdf | p. 7]
  Class: IRISH DIRECT/TRANSFER.

---

## 7. Adult growth envelopes for sessile oak and European beech

- Evidence is limited outside model yield tables; most is TRANSFER/INDIRECT (European sites, managed stands);
  few direct Irish/UK trials. A Czech/Slovak beech thinning study [Štefančík et al. 2013] is the strongest
  single case. [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | pp. 1, 3]
  Class: BROADER TRANSFER.
- Sessile oak: very slow year-1 height (~15–20 cm); ~10–20 cm/yr for first 5–8 yr; ~30–50 cm/yr during age
  10–20 on good sites, then slowing; height increment sustained until ~100–120 yr; exemplar maxima ~35–40 m
  by 100 yr (records ~41 m UK); typical mature heights ~25–35 m. [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | pp. 1–2]
  Class: BROADER TRANSFER.
- Sessile oak DBH: dominant oak ~40–60 cm at 80–100 yr in managed stands (target log classes start ~45–60 cm);
  unthinned stands many <30 cm DBH at 100 yr; estimate crop-tree DBH ~30–50 cm by 50 yr, ~40–60 cm by 100 yr
  on good sites. [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | p. 2]
  Class: BROADER TRANSFER.
- European beech: ~4 m tall at 10 yr; likely ~15–20 m by age 30 in good stands; age 30 ~15–20 m,
  50 ~25–30 m, 75 ~30–35 m, 100 ~35–40 m (upper estimate 40–45 m for crop trees); managed (free-crown)
  100-yr crop trees ~38–41 m; heavy-thinned crop ~48 m. [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | pp. 2–3]
  Class: BROADER TRANSFER.
- European beech DBH: likely mean DBH ~30–50 cm at 50 yr, ~50–70 cm at 100 yr under good growth; Slovak
  study crop-tree beech under heavy thinning ~60 cm DBH at 100 yr versus ~36 cm in control; free-crown
  thinning ~38 m/59 cm and ~41 m/59 cm; unthinned ~39.5 m/35 cm.
  [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | pp. 3, 4]
  Class: BROADER TRANSFER.
- Cross-species: beech grows faster initially and attains greater heights earlier; at 50 yr beech stand height
  ~10–15% higher than oak on good sites; by 100 yr tops may be similar; in mixed stands beech often overtops
  oak; oak has a longevity advantage; beech responds strongly to thinning, oak also responds but less.
  [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | p. 3]
  Class: BROADER TRANSFER.
- Validation envelopes (TRANSFER, ex-British/Irish mature oak, continental CCF/high-forest): sessile oak good
  site age 50 dominant height ~20–25 m, crop-tree DBH ~30–40 cm; age 100 dominant height ~30–35 m,
  crop-tree DBH ~50–60 cm. European beech good site age 50 dominant height ~25–30 m, crop-tree DBH ~40–50 cm;
  age 100 dominant height ~35–40 m, crop-tree DBH ~60–70 cm (heavily thinned ~45–50 m).
  [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | p. 4]
  Class: BROADER TRANSFER.
- Scenario One applicability (critical guard): the envelopes refer to trees ALREADY of that age; Scenario One
  introduces young oak/beech (<40 yr) and must use juvenile growth rules. In a ~50-yr horizon expect oak to
  remain <50 cm DBH and <20 m tall, beech similarly ~25–30 m. Do NOT assume newly planted oak/beech "catch
  up" to mature stand heights/DBH in 30–50 yr. Premium oak (~50 cm mid-log DBH, standing tree ≥~60–70 cm at
  ages 120–150 yr) is beyond Scenario One. [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | p. 4]
  Class: BROADER TRANSFER + GAMEPLAY GUARD.
- Final gate stated by the source: "B — SUFFICIENT ONLY FOR ORDINAL RESPONSE CALIBRATION" (directions and
  rough ranges, not precise parameters); calibration worker MUST NOT claim exact coefficient values or that
  sites have linear multipliers beyond "better site → higher max height".
  [SOURCE: height and diameter growth of Sessile oak and European beech.pdf | pp. 4–5]
  Class: CALIBRATION LIMIT.

---

## 8. Crown geometry (cross-reference within composite)

- Tabbush & White (1988): open-grown Sitka crown-width/DBH potential envelope (see §5); sampled 73 trees
  (ages 9–71), 72 used after outlier removal, DBH 7–112 cm (7–80 cm in the regression table). Treat as a
  POTENTIAL envelope, not a plantation crown. [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | pp. 4, 8]
  Class: CLOSE TRANSFER.
- Davies & Pommerening (2008): 154 Sitka + 125 birch crowns in six stem-mapped Welsh plots; spatial variables
  improved crown models more consistently than non-spatial; Sitka crown radius efficiency 0.75→0.77,
  crown length 0.67→0.74; pooled radius Hegyi coefficient is counterintuitive and must not be used causally;
  crown-length model `CrownLength_m = H · exp(−max(0, 0.58557·(H/DBH_cm) + 0.30910·BA_5))`.
  [SOURCE: CCF_Primary_Literature_Synthesis_v1.pdf | pp. 4–5, 10 | original: Davies & Pommerening 2008]
  Class: CLOSE TRANSFER + CONFLICTING-EVIDENCE caution.

---

## 9. GROWFOR boundary and version caveat

- Full equations/coefficients/validity limits are retained in `GROWFOR_Technical_Evidence_Extraction_v1.md`.
  Summary anchors preserved here for retrieval:
  - Sitka state vector x = (B, N, H); published thinned/unthinned growth matrices; height params a = 47.0216,
    c = 0.4571, t0 = −5.0; volume equation with coefficients β0 = 1.0013, β1 = 0.5295, β2 = −1.6483,
    β3 = −0.0010, β6 = −0.2384 (adjusted R² = 0.9557); thinning-transition coefficients a = 10.0991,
    b = 0.2371, c = 0.2877, d = 0.5585. [SOURCE: GROWFOR_Technical_Evidence_Extraction_v1.md | §§5–9]
  - Runtime limit for the illustrated Sitka-thinned model: top height < 49.3 m; projections best from first
    thinning to clearfelling. [SOURCE: GROWFOR_Technical_Evidence_Extraction_v1.md | §§3.1, 11]
  - Version caveat: G3 reports the Sitka volume model lacked an asymptote and required upper-growth-limit
    fixes, so the 2006 coefficients are reproducible evidence for BroadLynch2006_Sitka but cannot be assumed
    identical to every later GROWFOR executable. [SOURCE: GROWFOR_Technical_Evidence_Extraction_v1.md | §12]
  - Independent NFI evaluation (G4): Sitka N = 200 plots (107 thinned, 93 no-thin), 183/200 with ingrowth;
    all-Sitka GROWFOR bias/volume −1.41%, RMSE 18.80%, R² 0.84; BA bias −1.96%, RMSE 12.81%, R² 0.81.
    [SOURCE: GROWFOR_Technical_Evidence_Extraction_v1.md | §§13.2–13.3]
  Class: IRISH DIRECT (published model) + INDEPENDENT CROSS-SECTIONAL NFI PERFORMANCE CONTEXT (not
  longitudinal final-model validation).
- **Broad & Lynch 2006b source-data validation (V1B).** The estate reference population was 44,217
  subcompartments (27,887 thinned, 16,330 unthinned). Research-versus-estate site-index distributions gave
  χ²(7) = 625.5 (thinned) and 1648.2 (unthinned), both p < 0.001; the lowest estate site-index classes were
  absent from the research plots. A separate 2003 nonresearch sample of 113 production stands (70 thinned,
  43 unthinned) gave a common-volume relationship test F(4,105) = 0.8835, so the common-equation null was
  not rejected. The 113 stands were independent of the original research experiments but were later used in
  2006a calibration/weight adjustment, so they are not an untouched final-model test set.
  [SOURCE: Broad & Lynch 2006b, Irish Forestry 63(1&2):80–95, pp. 82–89; Broad & Lynch 2006a, pp. 68–70]
  Class: EMPIRICAL SOURCE-DATA / PANEL-DATA VALIDATION; not final dynamic-model prediction validation.
- Cross-reference: GROWFOR covers Sitka, Douglas fir, Norway spruce (and GROWFOR boundary per Species
  Evidence Pack) plus Scots pine; the manual additionally shows larch and Ash models (7 named species /
  13 model options). Keep separate from composite 2's browsing material.
  [SOURCE cross-ref: GROWFOR_Technical_Evidence_Extraction_v1.md | §§3.1, 17; CCF Species Evidence Pack v1 ...pdf | pp. 9, 23]

### 9.1 Validation evidence scoring (V1B)

| Dimension | Score | Meaning |
|---|---:|---|
| `source_data_empirical_validation_score` | **2** (was 0) | Empirical estate/research distribution and cross-sectional volume-data diagnosis |
| `independent_predictive_validation_score` | **0** | No untouched longitudinal forecast test of the final model |
| `ccf_transfer_validation_score` | **0** | No irregular/continuous-cover transfer validation |

If a single ordinal field is used instead: `validation_strength_score = 2`, `validation_strength_max = 4`
(0 none; 1 documentation/rationale; 2 empirical diagnostic/calibration or source-data validation;
3 independent final-model validation on untouched data; 4 independent external/replicated validation).

Label: **empirical data/panel validation; final-model prediction unvalidated.** These are
evidence-completeness/evidence-tier scores, not estimates of model forecast accuracy.

---

## 10. Irish site/productivity evidence for other species (four-paper packet)

- Conry & Clinch (1989), Slieve Bloom: roughly 4,000 ha of pre-1968 State forest inventory demonstrating
  large Irish site-related differences in yield class, including substantial Sitka variation among brown
  earths, peaty gleys, peaty podzols and blanket peat; excludes very small sub-compartments and generally
  mixed stands unless one species exceeds 80% of the canopy; provides NO SDImax/self-thinning relationship.
  [SOURCE: CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | p. 2 | original: Conry & Clinch 1989, Forestry 62(4):397–408, doi:10.1093/forestry/62.4.397]
  Class: IRISH DIRECT (site productivity).
- Dunbar, Ní Dhubháin & Bulfin (2002): 117 Irish Douglas-fir stands; yield classes 8–22 m³ ha⁻¹ a⁻¹ (mean
  15.3); site model with parent material, exposure and aspect explained 24% of yield-class variance; stands
  over 1 ha required to reduce edge effects (NOT proof that 1 ha is the minimum valid SDI area).
  [SOURCE: CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | p. 2 | original: Dunbar et al. 2002, Forestry 75(5):537–545, doi:10.1093/forestry/75.5.537]
  Class: IRISH DIRECT (productivity) — Douglas fir NOT in the accepted Scenario Two roster; evidence only.
- Remaining gaps named by the consolidation: the precise Sitka maximum-density relationship; GROWFOR's
  species-specific mortality/stocking; Irish or closely transferable broadleaf density envelopes for oak,
  beech, birch and alder; density behaviour in irregular/multi-layer CCF; a defensible spatial calculation
  unit for small farm forests; pedunculate-versus-sessile oak occupancy differences; Irish pair-specific
  mixture-density modification. [SOURCE: CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf | p. 3]
  Class: RESEARCH GAP.

---

## 11. Consolidated quantitative table (units preserved)

| Item | Value/relationship | Source | Class | Domain/caveat |
|---|---|---|---|---|
| Sitka untreated BA | ~55 m²/ha | G2 p.1 | IRISH DIRECT | one long-term thinning trial |
| Reineke exponent | −1.605 | G2 p.1 | BROADER TRANSFER | placeholder; k unknown |
| Irish Sitka age-30 top heights I–V | 27.6/23.3/20.4/17.4/16.0 m | G5 p.3 | IRISH DIRECT | validation anchors |
| Lekwadi Table 3 (b0,b2,b3) | see §4 | G5 pp.3–4 | IRISH DIRECT | rounded; does not reproduce anchors exactly |
| GROWFOR Sitka height params | a 47.0216; c 0.4571; t0 −5.0 | G6/25 §5 | IRISH DIRECT | BroadLynch2006 |
| GROWFOR Sitka volume coeffs | β0 1.0013; β1 0.5295; β2 −1.6483; β3 −0.0010; β6 −0.2384 | G6/25 §8 | IRISH DIRECT | adj R² 0.9557; 727 obs |
| GROWFOR thinning coeffs | a 10.0991; b 0.2371; c 0.2877; d 0.5585 | G6/25 §9 | IRISH DIRECT | selection component; 459 obs |
| Frenchpark mean DBH | 23.7/29.8/31.1/32.4 cm | G4 p.4 | IRISH DIRECT | unthinned/light/medium/heavy, 29 growing seasons |
| Sitka juvenile light threshold | ~20% RLI | G4 p.5 | BROADER TRANSFER | British CCF evidence |
| Sitka seed onset | ~20–25 yr | G4 p.6 | BROADER TRANSFER | Britain/NW Europe |
| Sitka seed dispersal | ~80% within 60 m (λ ≈ 20 m) | G4 p.6 | BROADER TRANSFER | exponential kernel |
| Sitka crown envelope | R_m = 0.9415 + 0.07635·DBH_cm | G4 p.5; G5 p.4 | CLOSE TRANSFER | open-grown UK |
| Sitka crown relaxation k | 0.1–0.2/yr | G4 p.5 | CALIBRATION | starting range |
| Sitka stem volume | BA × FormHeight; DBH_cm²×0.00007854×FormHeight_m | G4 p.8 | IRISH DIRECT | Teagasc measurement |
| Oak good site age50/age100 dominant height | 20–25 m / 30–35 m | G3 p.4 | BROADER TRANSFER | UK/Ireland-type climate |
| Oak crop-tree DBH age50/age100 | 30–40 cm / 50–60 cm | G3 p.4 | BROADER TRANSFER | managed stands |
| Beech good site age50/age100 dominant height | 25–30 m / 35–40 m (thinned 45–50 m) | G3 p.4 | BROADER TRANSFER | continental |
| Beech crop-tree DBH age50/age100 | 40–50 cm / 60–70 cm | G3 p.4 | BROADER TRANSFER | intensive release |
| Beech heavy vs control 100 yr | ~48 m/60 cm vs ~39.5 m/35 cm | G3 pp.3–4 | BROADER TRANSFER | Slovak thinning study |
| Pretzsch maximum-density coverage | 64 experiments, 328 plots, 2,627 surveys | G6 p.2 | BROADER TRANSFER | Central Europe |
| Pretzsch DEC/DMC species | NS, SP, DF, beech, sessile oak | G6 p.2 | BROADER TRANSFER | do not import coefficients |
| Conry Slieve Bloom | ~4,000 ha pre-1968 inventory | G6 p.2 | IRISH DIRECT | no SDImax |
| Dunbar Douglas fir | 117 stands; YC 8–22 (mean 15.3) m³/ha/yr | G6 p.2 | IRISH DIRECT | 24% variance explained; DF not in roster |

END OF COMPOSITE 3
