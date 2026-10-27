# S1 Three-Species Parity P0 — PARITY-01 Current-Main Implementation Audit — Handoff

**Date:** 2026-10-27
**Worker / role:** Claude Sonnet 5.5 (Claude Code) — primary read-only auditor
**Task:** `PARITY-01 — Current-main implementation audit`
**Spec:** `specs/scenario-one-three-species-parity-p0/` — DRAFT (not approved, not changed by this task)
**Branch:** `audit/scenario-one-three-species-parity-p0-current-main`
**Base:** `c3df69c8e28478993cfd1f9d3304e5f4d453e612`
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d` (verified ancestor of base)
**Inspected HEAD:** `920b4d83efe6e71fd4be559950efb3ac4e6aef4f` (base + the audit task packet; docs only)

## Verdict

`CURRENT SPECIES IMPLEMENTATION MAPPED`

This closes the implementation audit of **`main`** only. Nothing here claims the parity feature is implemented, verified or approved. No production file, scene, prefab, asset, package or setting was edited and Unity was not run.

> ### ⚠ MANAGER DECISION REQUIRED before PARITY-03 is dispatched (does not block PARITY-02 evidence work)
>
> `main` has no three-species parity code, which matches the draft spec and D-059/D-061/D-062. **But the repository already holds a large, unmerged three-species implementation lineage** (growth model 2, ordinal growing-space-demand density, dormant species-site infrastructure, Oak/Beech timber ladders and prices, Oak/Beech growth recalibration) on `integration/scenario-one-tier3-candidate` and related branches, dated 2026-10-12 to 2026-10-18. `main`'s canonical docs and the draft spec/plan/tasks never mention it (`git grep` at `c3df69c` over `Docs/Project`, `specs`, `AGENTS.md` finds no reference), and the plan sequences PARITY-03…06 as if greenfield. The Manager must decide **adopt / requalify / replace** for that lineage before any production task is dispatched, otherwise PARITY-03…06 will duplicate it or collide with it. Inventory in §7; this audit did **not** review that lineage line-by-line.
>
> None of the four literal stop conditions in the task packet fired against `main`. This is a governance discrepancy, reported rather than resolved.

---

## 1. Source identity and method

| Item | Result |
|---|---|
| Production-tree identity | `git diff c1564b85..HEAD -- Assets Tools ProjectSettings Packages` is **empty**. Production code on this HEAD, on `origin/main` (`3b499aa`) and at the 2026-10-12 audit base (`c1564b85`) is **byte-identical**. Only `Docs/`, `specs/` and `AGENTS.md` changed since. |
| Consequence | Every code fact in the Overview's "Species-parity audit facts" section (`unity-project-overview.md:393–404`) **still holds**, including its line numbers (`ForestEcologyController.cs:1014`, `:1040`, `:1122–1133`). The earlier audit covered only growth/density/verification; this one covers the remaining systems. |
| Worktree | New sparse worktree `/media/jer/ZX20/CCF-Parity01` (excludes `*.fbx/png/jpg`; all source, YAML assets and docs present). `/media/jer/ZX20/CCF-Primary` and its uncommitted changes (`AGENTS.md`, `active-tasks.md`, `StorageArchitecture.md`, `ScenarioTwoPreProductionAudit.md`) were not touched. |
| Locks | No lane or lock for this task in `active-tasks.md` at HEAD (read-only; none needed). |
| Method | Read the code and assets; trace each species-keyed value to its consumers; grep every `sitka-spruce`/`beech`/`sessile-oak` literal in runtime code; read tests/harness assertions; read species docs for evidence labels; run stdlib-Python arithmetic on constants read from source (§10). |

**Classification key** (from the task packet):
**A** already species-specific and adequate for P0 · **B** species-specific, evidence/calibration needs closure · **C** shared intentionally and defensibly · **D** shared incorrectly / Sitka-derived · **E** absent for Oak/Beech · **F** uncertain — Manager/spec decision.

Evidence tags used below: **[V]** verified by reading the cited code · **[D]** derived arithmetic from verified constants (not a Unity run) · **[U]** unverified / not inspected.

---

## 2. Premise check against D-059 / D-061 / D-062

| Premise | Result on `main` |
|---|---|
| D-059: Oak/Beech must not silently inherit inappropriate Sitka assumptions | **Confirmed that they currently do** in density (G-01/G-02), timber work-cost proxy (G-08), pruning (G-10), stem-form ratio (G-07), canopy/light (G-05). |
| D-061 (1): 100 % Sitka routes through the existing exact implementation | On `main` there is **no routing**: the single existing path (`ForestEcologyController.cs:1040–1133` + `SitkaGrowthModel.cs`) is the pure-Sitka path and is applied to every composition. "Preserve" therefore means keep those statements byte-identical and branch *around* them. |
| D-061: Oak/Beech inherit Sitka density; no Oak/Beech density equation exists | **Confirmed.** `ApplyAdultDensityMortality` collects all `IsLiving` trees with no species filter (`:1044–1047`); `StandRelativeDensity` feeds all of them to `SitkaGrowthModel.RelativeDensity` (`:1122–1133`). No Oak/Beech max-density or SDI constant exists anywhere in `main`. |
| D-062: timber assortments and prices Sitka-only; Oak/Beech explicitly unmarketed | **Confirmed**, with sharper detail in G-06…G-09. A dormant flat `broadleafTimberValueCentsPerCubicMetre = 6500` exists but has **no production caller** (G-09). |
| Draft spec: "milestone states P0 not started" | True of `main`; **misleading for the repository** (see Manager decision above). |

---

## 3. Current-behaviour matrix

### 3.1 Species data actually serialised (assets `Assets/ForestPrototype/Species/*.asset`) [V]

Only fields that differ or matter. Sitka / Oak (`sessile-oak`) / Beech (`beech`).

| Field | Sitka | Oak | Beech | Note |
|---|---|---|---|---|
| Potential DBH growth (cm/yr) | 1.2 | 0.4 | 0.4 | Oak ≡ Beech |
| Potential height growth (m/yr) / max height (m) | 0.45 / 35 | 0.4 / 40 | 0.4 / 40 | Oak ≡ Beech. Sitka's is superseded by Class III envelope under growth model 1 |
| Max DBH (cm) | 120 | 120 | 120 | all equal |
| Ci50 (Hegyi half-response) | 5 | 3 | 3 | tagged [D]; Oak ≡ Beech |
| Crown radius | 0.9415 + 0.07635·DBH | 0.10 + 0.1045·DBH | power law ln(CPA)=0.05+1.01·ln(DBH) | tags [B]; Oak source not stated beyond `SessileOakV1.md:21–22` |
| Crown relaxation /yr | 0.15 | 0.12 | 0.15 | |
| Form-height ratio | 0.5 | 0.5 | 0.5 | all equal (G-07) |
| Maturity onset→full (yr) | 20→30 | 40→60 | 40→60 | |
| Seed dispersal scale / cutoff (m) | 20 / 150 | 4 / 80 | 6 / 50 | |
| Mast multipliers good/normal/poor | 1.5 / 1 / 0.4 | 1 / 0.3 / 0.075 | 1 / 0.4 / 0.1 | mast **probabilities** 0.15 / 0.35 identical for all three |
| Seed saturation S50 | 10 | 5 | 5 | script default is 5; Sitka asset overrides |
| Initial juvenile height (m) | 0.15 | 0.18 | 0.15 | |
| Juvenile height growth (m/yr), promotion height, density params | 0.35, 3.5 m, 0.5 / 1.5 | same | same | **identical for all three** |
| Juvenile light response (growth) | 6 anchors, peaks 0.9 at 0.55 | 10 anchors, reaches 1.0 at ≥0.5 | 7 anchors, reaches 1.0 at ≥0.35 | |
| Distinct survival / establishment responses | no (combined legacy) | **yes** | **no** (combined legacy) | asymmetry (G-16) |
| Promotion minimum light | 0 | 0.20 | 0 | |
| Browse palatability | 0.15 | 1.0 | 0.5 | ordering [E transferred], numbers [S] (`BrowsingProtectionV1.md:96–98`) |
| Browse full-vulnerability → escape height (m) | 0.5 → 0.8 | 1.2 → 1.8 | 1.2 → 1.8 | |
| Wind susceptibility / opening weight / half-life | 1 / 1 / 3 | 1 / 1 / 3 | 1 / 1 / 3 | identical; dormant (storm model 0) |

Planting-stock age (3 yr), height (0.6 m) and density (1.0) are **one shared set** on the ecology controller, labelled "Beech v1" (`ForestEcologyController.cs:29–35`, used for Oak too at `ScenarioOneManager.cs:1793–1794`).

### 3.2 System matrix

| # | System | Sitka (current) | Oak (current) | Beech (current) | Class | Evidence / refs |
|---|---|---|---|---|---|---|
| 1 | **Adult DBH growth** | `pot × site × (1−D/Dmax) × 1/(1+CI/Ci50)` with 1.2 cm/yr, Ci50 5 | same formula, 0.4, Ci50 3 | identical to Oak | Formula **C**; Oak/Beech values **B** (and Oak ≡ Beech: **B/F**) | `ForestEcologyController.cs:993–1024`; docs `BeechSpeciesParameters.md:14–18` (B/C, D), `SessileOakV1.md:20` |
| 2 | **Adult height growth** | Growth model 1: Class III site envelope `H=34.362(1−e^(−0.042a))^1.563`, each tree keeps relative height | Legacy asymptotic law `0.4·site·(1−H/40)` | identical to Oak | Sitka **A**; Oak/Beech **E** (no site-class/age model) + **B** (values) | gate at `:1014–1015` (`SameSpeciesId(…,"sitka-spruce")`); `SitkaGrowthModel.cs:31–54` |
| 3 | **Site productivity / suitability** | scalar `cell.SiteProductivity`, default 1.0, never assigned in production code | same scalar | same scalar | **E** (no species×site response); need = **F** | `ForestEcologyCell.cs:48`; only readers `:1004`, `:1315`, `ScenarioOneManager.cs:1919`, `ForestTreeMarkingManager.cs:379` |
| 4 | **Crown geometry** | linear, relax 0.15 | linear (different coefficients), relax 0.12 | power-law, relax 0.15 | **A/B** (species-specific; Oak provenance thin) | `TreeSpeciesDefinition.cs:157–166`; `ForestEcologyController.cs:1137–1153` |
| 5 | **Canopy / light** | lateral `1−d/(CR+2.5 m)`, vertical `clamp(H/8)`, combined `1−Π(1−influence)`; opacity implicit 1.0 | same | same | **D/F** — only species input is crown radius; no transmissivity, no deciduous state | `:812–841` (reach `:831`, vertical `:833`); `CanopyShadeReachPerCrownRadius :908` Sitka-calibrated k10a10 [C] |
| 6 | **Competition (Hegyi)** | neighbour term `(DBH_n/DBH_t)/dist`, 8 m cutoff; neighbour species ignored | same | same | Formula/cutoff **C** (cutoff calibrated on 16–40 cm Sitka, `:899–902`); `Ci50` **B** | `:920–989` (inline term `:984`); `ForestTreeMarkingManager.cs:377–397` and `UI/CompetitorAssessment.cs:160` consume per-species `Ci50` |
| 7 | **Density / self-thinning** | One stand-wide British-Sitka RD over **all** living stems; onset RD 0.6, strength 0.08, S⁴, boundary RD 1.0 | counted in the same RD, same hazard, same boundary | same | **D** (the D-061 target) | `:1040–1133`; `SitkaGrowthModel.cs:61–108`; runs only when `growthModel ≥ 1` (`:470–471`, `:141`) |
| 8 | **Density victim weighting** | suppression `S=1−1/(1+CI/Ci50)` uses the tree's **own** `Ci50`, then `S⁴`/stand mean | Ci50 3 ⇒ higher S at equal CI | same | **D** — hidden species bias (G-02, H-01) | `:1056–1064`, `:1070`, sort `:1082–1086` |
| 9 | **Pruning / Crop Tree** | 3 lifts to 2.5/5/6.5 m, 8 % crown loss per lift, 5-yr interval, cap 60 % of height — "common Sitka clear-stem practice" | identical, any Crop Tree of any species | identical | **D** (species-blind, Sitka-derived) / **F** | `ForestTree.cs:172, 180–187`; `ScenarioOneDefinition.cs:70–71`; `ScenarioOneManager.cs:240–291, 1027–1066`; `ForestEcologyController.cs:1149–1150` |
| 10 | **Seed / mast** | species-keyed maturity, kernel, mast; isolated RNG phases for non-default species | same | same | **B** (species-specific; mast probabilities shared; crown factor `clamp(CR/4)` shared; seed only from in-stand trees; no jay/long-distance dispersal — `SessileOakV1.md:27–29`) | `:1175–1228`, `:1230–1283` (`:1257`) |
| 11 | **Juvenile light/survival/establishment** | combined legacy response | **distinct** survival + establishment + growth curves; promotion min light 0.20 | combined legacy response (shade-tolerant curve) | Oak **B**; Beech **B/F** (structural asymmetry) | `TreeSpeciesDefinition.cs:178–203`; `JuvenileEcologyRules.cs:14–20, 33–34`; `ForestEcologyController.cs:1314–1330, 1376–1377, 1416–1417` |
| 12 | **Juvenile growth / promotion** | 0.35 m/yr × light response × site; promote at 3.5 m; promoted DBH `clamp(1.5·h,2,20)` | same numbers | same numbers | **C by accident**: identical values, no stated evidence they should be → **B/F** | `JuvenileEcologyRules.cs:22–26, 37`; `ForestEcologyController.cs:1618–1665`; `ScenarioOneManager.cs:1933–1939` |
| 13 | **Planting stock** | not player-planted | 3 yr / 0.6 m shared; price-book 100¢ vs shop 550¢ | 3 yr / 0.6 m shared; price-book 95¢ vs shop 450¢ | **D** (Beech-derived shared set) / **B** | `ForestEcologyController.cs:29–35, 689–690`; `Stage1EconomyDefaults.cs:50–55`; `ScenarioOneDefinition.cs:45–49`; `ScenarioOneEconomyAdapter.cs:46–48` |
| 14 | **Browsing** | palatability 0.15; reach 0.5→0.8 m | 1.0; 1.2→1.8 m | 0.5; 1.2→1.8 m | **A** for P0 (documented, harness-verified, labelled); numbers **B**. Shared: height-increment loss 0.9, extra mortality 0.04, no recovery/resilience difference (explicit in doc). Persistent form damage **E** (deferred D-044) | `JuvenileEcologyRules.cs:45–107`; `BrowsingProtectionV1.md:48–59, 96–110` |
| 15 | **Protection (shelters/fences)** | shared geometry, species enters only through height vulnerability | same | same | **C** | `BrowseProtectionGeometry.cs`; `ScenarioOneManager.cs:1798–1803` |
| 16 | **Understorey competition** | shared bramble/bracken loss on juveniles | same | same | **C** — accepted shared abstraction (D-049) | `ForestEcologyController.cs:1305–1313`; `ScenarioOneManager.cs:1899–1910` |
| 17 | **Wind / storm** | one diagnostic formula, species multipliers all 1 | same | same | **C** (dormant, storm model 0). Inspection "Wind exposure" shows it for all species → wording **F** | `:1700–1713`; `StormModel.cs:99`; `UI/TreeInspectionView.cs:87` |
| 18 | **Stem volume / form** | `DBH²·0.00007854·H·0.5` (form ratio [D]); deadwood volume uses same | same ratio | same ratio | **D** for broadleaves (unverified neutral form; one ratio for all) | `ForestTree.cs:227–235`; `ScenarioOneManager.cs:494–495`; `TimberYieldConfiguration.cs:56` ("Neutral form is an unverified assumption") |
| 19 | **Timber assortment** | 4 specs: sawlog, pallet, stake, pulp (Teagasc dims; `SelectionEvidence=SimulationAbstraction`) | **none** → whole stem is `Unallocated` residue | **none** | **E** (honest absence; engine is species-keyed with "no automatic broadleaf transfer") | `TimberYieldConfiguration.cs:10, 48–69`; `TimberYieldCalculator.cs:15, 35–37`; verified by `Tools/Verification/TimberAssortmentYieldVerification.cs:159–160` |
| 20 | **Roadside price / density** | 4 IFA-2024 prices; fresh density 890 kg/m³ (Sitka only) | none | none | **E**; dormant flat 6,500¢/m³ in `ScenarioOne.asset:24` unread (G-09) | `Stage1EconomyDefaults.cs:13–30`; `ScenarioOneDefinition.cs:159–164` |
| 21 | **Harvest settlement (Work Plan)** | priced Sitka logs only; revenue − combined €/t cost − minimum job | all stems cost-only; work mass uses the **Sitka** 890 kg/m³ as a "work-equivalent proxy"; `HasUnmarketedSpecies` | same | **D** (Sitka density proxy, labelled [S]) / **F** | `ScenarioOneEconomyAdapter.cs:104–109, 111, 123–134, 144–145` |
| 22 | **Yield quality inputs** | none for standing trees (`Quality=None`); only windthrow salvage sets damage sections | same | same | **E** for all species; pruning/suppression history are persisted but **not** wired in | `WindthrowSalvage.cs:15` is the only `StemQualityFlags` use outside `TimberYield/`; `ForestTree.cs:101–103, 173–178` |
| 23 | **Inspection card** | name, DBH/H/volume/age, light, competition, growth, wind, pruning, reproduction, "Why", timber estimate | same fields; timber estimate = "No timber market is configured for this species in Scenario One." | same | Honesty **A**; species-comparison/teaching content **E** | `UI/TreeInspectionView.cs:61–121, 198–241` |
| 24 | **Regeneration diagnosis / map side panel** | per-species escape height, promotion light, light survival | same, species values | same | **A** (already species-parameterised) | `RegenerationDiagnosis.cs:248–270`; `UI/StandMapView.cs:218` |
| 25 | **Work Plan copy** | product names are enum names | "No broadleaf timber market is configured: those stems are charged for felling but earn no sale revenue." | same | Honesty **A**; generic "broadleaf" lumps two species **F** | `UI/WorkPlanView.cs:164–165` |
| 26 | **Habitat / soundscape / reviews** | "conifer" = `sitka-spruce` | everything else = "broadleaf" (÷45 [D]) | same | **C** for P0; hard-codes will mis-handle any further species | `ScenarioHabitatInterpretation.cs:11–16`; `ScenarioSoundscape.cs:100–118`; `ScenarioEcologyReviewLines.cs:37–46` |
| 27 | **Visual-only Sitka gating** | authored fallen-log and brash visuals | no brash; log visual only for Sitka | same | presentation only; out of P0 | `ScenarioOneManager.cs:1573, 1666` |
| 28 | **Legacy wood yield** | `WoodYield = clamp(round(H),3,10)` | same | same | dead property in production (no consumers) | `ForestTree.cs:166` |

---

## 4. Confirmed implementation gaps

Severity: **P0-core** = required by D-059/D-061/D-062; **P0-guard** = must be handled so parity does not break something protected; **note** = real but minor.

| ID | Gap | Sev | Evidence |
|---|---|---|---|
| G-01 | Adult density/self-thinning is species-blind: Sitka RD, hazard and boundary applied to every living stem. | P0-core | `ForestEcologyController.cs:1044–1050, 1078, 1122–1133` |
| G-02 | The density hazard weights victims by `S⁴` where `S` uses each tree's **own** `Ci50`. At equal competition a broadleaf reads as more suppressed: weight ratio 6.1× at CI 0.5, 5.1× at CI 1, 3.8× at CI 2, 3.2× at CI 3, 2.4× at CI 5, 1.95× at CI 8 [D]. Under pressure (RD>0.6) this removes broadleaves first, and the boundary pass sorts by the same `S` (`:1082–1086`). This is a second, hidden inheritance of Sitka calibration that D-061's wording does not mention. | P0-core | `:1056–1064`; `SitkaGrowthModel.cs:97–108` |
| G-03 | Oak and Beech have **identical** adult growth, competition, size-max and wind parameters; Oak/Beech height uses the legacy asymptotic law with no age/site model. Unsuppressed, site 1, after promotion at 3.5 m/5.25 cm (tree age ≈12): DBH 18.9 / 34.5 / 37.8 cm and height 15.1 / 24.9 / 26.6 m at ages 50 / 100 / 112, **the same for both species** [D]. The evidence composite (§7, TRANSFER, "ordinal response calibration only") says beech is faster early and taller (≈10–15 % at 50 yr), with 100-yr crop-tree DBH 50–60 cm (oak) and 60–70 cm (beech). Whether this gap needs closing is a calibration question (Q-03), not an assertion of error. | P0-core | assets §3.1; `:1007–1011` |
| G-04 | No species×site response exists on `main`; `SiteProductivity` is a constant 1.0 and the starting site is explicitly Irish Sitka Class III (D-048). | P0-core / F | `ForestEcologyCell.cs:48`; grep: no production assignment |
| G-05 | Canopy/light is species-blind except through crown radius; no species transmissivity, no deciduous/seasonal state. | P0-core / F | `ForestEcologyController.cs:812–841` |
| G-06 | No Oak/Beech assortment specifications, no market bindings, no prices, no green density. `TimberYieldDefaults.CreateSitka()` is hard-coded at **two** call sites (harvest quote and inspection estimate); sold logs are filtered to `"sitka-spruce"`; bindings are generated only for Sitka; `TimberAssortment` is a four-member Sitka vocabulary. | P0-core | `ScenarioOneEconomyAdapter.cs:111, 126, 133–134`; `UI/TreeInspectionView.cs:200, 219`; `ForestryWorkDomain.cs:9` |
| G-07 | Quality/form/management history never reach the yield engine for standing trees: `StemMeasurements.Quality` is always `None`; pruning lifts/crown base, suppression history and any browse history are not mapped to sections; the stem volume uses one form ratio 0.5 for every species. The engine **can** consume quality flags and sections (used only by windthrow salvage). Persisted per-tree form state is limited to pruning lifts, crown base height, last pruning year and recorded suppression years; no fork/sweep/browse-damage state exists (D-044). | P0-core | `ScenarioOneEconomyAdapter.cs:86–88`; `MerchantableStemModel.FromMetres`; `ForestTree.cs:101–103, 173–178` |
| G-08 | Oak/Beech harvest work is charged using the Sitka fresh density (890 kg/m³) as a proxy and earns nothing; the UI says only "broadleaf". | P0-core / F | `ScenarioOneEconomyAdapter.cs:104–109`; `UI/WorkPlanView.cs:164–165` |
| G-09 | A dormant, unlabelled flat broadleaf price (`broadleafTimberValueCentsPerCubicMetre: 6500`) is serialised in `ScenarioOne.asset` and exposed by `TimberValueCentsPerCubicMetre(speciesId)`; no production caller (only a Sitka-id verification reference). It is the exact "simple species price" FR-7 forbids and is a trap for an implementer. | P0-guard | `ScenarioOneDefinition.cs:34–35, 159–164`; `ScenarioOne.asset:23–24`; `Tools/Verification/ScenarioOneDeadwoodVerification.cs:174` |
| G-10 | Pruning schedule and the Crop Tree mechanism are species-blind and Sitka-derived; any species can be marked and pruned to Sitka lift heights. | P0-guard (FR-12/AC-8) | see matrix row 9 |
| G-11 | One planting-stock age/height/density set is applied to both Oak and Beech, documented only as Beech evidence. | note / B | `ForestEcologyController.cs:29–35` |
| G-12 | Verification gaps: Oak, Beech, Oak-player-planting and browsing harnesses are pinned to `GrowthModel.Legacy` (`CCFOakVerification.cs:361`, `CCFBeechVerification.cs:25`, `CCFOakPlayerPlantingVerification.cs:75`, `BrowsingProtectionVerification.cs:974`), so no Oak/Beech test exercises `ApplyAdultDensityMortality`; there is **no mixed-species density test**; `SitkaGrowthMortality/` verification contains no broadleaf case. Existing tests also encode "unmarketed" as expected behaviour (§5 H-09). | P0-guard | greps in §10 |
| G-13 | Persistence: tree `speciesId` is **not validated** against the registry; an unknown id loads as the default species (Sitka) with only a warning (`ForestSaveController.cs:382–394`); `ScenarioTimberSale` has no species field and is validated by `Enum.IsDefined(TimberAssortment)` with one record per assortment per report; `growthModel` is validated against `GrowthModel.Latest = 1`. | P0-guard | `ForestSaveData.cs:111–117`; `ForestSaveValidation.cs:80, 296–299`; `SitkaGrowthModel.cs:13` |
| G-14 | Player-facing species information is limited to name, maturity, regeneration diagnosis and honest "no market" lines. There is no species comparison, shade/browse/timber-role teaching, or evidence-limit wording for Oak/Beech. | P0-core (FR-8) | matrix rows 23–25 |
| G-15 | Species parameters live in unversioned ScriptableObject assets shared by every growth/regeneration model and every save. Changing Oak/Beech asset values changes behaviour of old saves, Legacy40 and the Reference Future v1 continuation unless the code gates by a persisted model value. | P0-guard (FR-9/FR-10) | see H-02 |
| G-16 | Beech uses the combined legacy juvenile response (survival derived from the growth curve, 20 % annual loss below response 0.15), whereas Oak has distinct survival/establishment curves. | B / Q-07 | `JuvenileEcologyRules.cs:14–20`; assets |
| G-17 | Mast good/poor probabilities (0.15 / 0.35) are identical for all three species. | B / Q-07 | assets |

---

## 5. Hidden cross-system dependencies

| ID | Dependency | Why it matters for parity |
|---|---|---|
| H-01 | **`Ci50` has five jobs**: DBH response (`:1009`), crown target (`:1147`), recorded suppression (`:1019`), density victim weighting (`:1060`), and every UI/forecast that reports "growth withheld" (`ForestTreeMarkingManager.cs:384, 397`; `UI/CompetitorAssessment.cs:160`; `UI/CropTreeCompetition.cs:236`). | Changing Oak/Beech `Ci50` for adult parity silently changes who dies (G-02), crown size/light and what the player is told. PARITY-04 and PARITY-03 are not independent. |
| H-02 | Species assets are not version-gated. | A parity change cannot be "asset only": FR-9/FR-10 need a persisted gate (see §9 C-04). |
| H-03 | Species registry = `ForestTreeSpawner` scene component (`KnownSpecies` from `defaultSpecies` + `speciesVisualSets`); `RegenerationSpecies` and spawn visuals depend on it. | New species data or new fields are edits to high-conflict single-writer YAML (`Species/*.asset`, scene, `ScenarioOne.asset`), per `AGENTS.md`. |
| H-04 | Promoted juveniles join the adult stand immediately (DBH `clamp(1.5·h,2,20)`; 5.25 cm at 3.5 m). | Any regeneration/promotion change (PARITY-05) feeds the density signal and S⁴ weighting (PARITY-03) in the same year. |
| H-05 | `OnTreeBiologicalDeath` creates a deadwood record with the species form ratio whenever growth model ≥ 1. | Broadleaf self-thinning creates deadwood volume that feeds objectives and habitat; changing the density route changes deadwood totals. |
| H-06 | Century/completion objectives count "presence" per planted species as `livingTrees + regenerationCells + plantedJuveniles`, target 40 per species on Enlarged80 (D-057). | Any establishment/browse/density change moves AC-7 solvability and objective reachability; calibration was explicitly continuity, not empirical. |
| H-07 | Harvest cost and the €2,500 minimum job are coupled to mass at Sitka density; completion solvency anchors (min cash €5,897.70 / €11,451.16 witnesses) depend on Oak/Beech being cost-only. | Introducing Oak/Beech revenue or a different density changes solvency and completion hashes; measure under G3. |
| H-08 | `TimberAssortment` integer enum is **persisted** (`ScenarioTimberSale`) and `Enum.GetValues(TimberAssortment)` drives Sitka binding generation (`"sitka-" + name`), `WorkPlanView` parses names back to the enum, `Enum.IsDefined` validates saves. | New products must be **appended** (never inserted/reordered); decide whether enum-domain extension and species-less sale records count as a schema change (G5). |
| H-09 | Tests/UI that assert "Oak/Beech unmarketed": `TimberAssortmentYieldVerification.cs:159–160`, `ScenarioOneEconomyIntegrationVerification.cs:128–133`, `WorkPlanView.cs:164–165`, `TreeInspectionView.cs:200–203`. | They will legitimately change under FR-6/FR-7. They need an explicit Manager-authorised update list, not a silent rebaseline. |
| H-10 | **Anchor partition** (see §8): Sitka-only lifecycle fixtures do not contain broadleaves; completion gates, Model2 completion, Reference Y100 and continuation do. | Oak/Beech asset edits cannot move the former but can move the latter. |
| H-11 | Non-default species draw randomness from isolated phases (`SpeciesRandom(speciesId, phase)`, `UpdateAllMastStates`, `PromoteCohorts`), keeping the default (Sitka) stream untouched (`:1175–1228, 1603–1615`). Density victims use `SimulationRandom.Roll(…"ADULT-MORT-"+treeId…)` (`:1071`). | Any new Oak/Beech random draw must follow the same isolation, or pure-Sitka hashes drift. |
| H-12 | MSFM V1 (D-065) is Sitka-adult-only, but `main` has no species gate in Crop Tree marking, pruning orders or `ScenarioOneManager` work-order creation. | FR-12/AC-8 needs an explicit guard in the MSFM implementation, not only in parity. |
| H-13 | Two parallel nursery price sources: price book (`Stage1EconomyDefaults` 95¢/100¢, None-So-Hardy 2025–26) and shop entries (450¢/550¢). `PriceBook(definition)` only adds shop prices when the price book lacks the item, so the price-book value would apply to a purchase-for-job quote while stock is bought at shop price. | Not a Sitka/broadleaf copy, but a parity-adjacent inconsistency for any economy review. |
| H-14 | Frozen reference: the Reference schedule plants Oak and Beech as well as Sitka (`ScenarioOneReferenceScheduleV1.json`), and the archive is embedded bytes. | Reference *integrity* is unaffected by asset edits; the *continuation* hash `9CDF21A541C5968D` is diagnostic only (D-042) but still listed as a protected anchor. |

---

## 6. Files and symbols inspected

All paths under `Assets/ForestPrototype/` unless noted. Read in full unless marked *(excerpt)*.

**Species model:** `TreeSpeciesDefinition.cs`; `Species/SitkaSpruce.asset`, `SessileOak.asset`, `BeechSpecies.asset`; `SitkaGrowthModel.cs` (`GrowthModel`, `SitkaGrowthModel.SiteTopHeightM/NextHeight/RelativeDensity/DensityPressure/Vulnerability/AnnualDeathProbability`); `ForestTree.cs` *(excerpt: species, pruning, `BiologicalStemVolumeM3`, `WoodYield`)*; `ForestTreeSpawner.cs` *(excerpt: `DefaultSpecies`, `KnownSpecies`, `ResolveSpecies`)*; `ForestEcologyCell.cs` *(excerpt: `SiteProductivity`, capacity)*; `AdultMortalityAccount.cs`; `RegenerationModel.cs` *(excerpt)*; `JuvenileEcologyRules.cs`; `ForestPlanting.cs`; `ScenarioOne/PlantedJuvenile.cs`.

**Ecology controller** `ForestEcologyController.cs` *(excerpts)*: `AdvanceOneYear :448–490`; `HegyiCutoffMeters :902`; `CanopyShadeReachPerCrownRadius :908`; `UpdateCompetition :920–989`; `GrowAdults :993–1024`; `SameSpeciesId :1026`; `ApplyAdultDensityMortality :1040–1120`; `StandRelativeDensity :1122–1133`; `RelaxCrowns :1137–1153`; `RegenerationSpecies :1155`; `UpdateAllMastStates/UpdateMast/SpeciesRandom :1175–1228`; `ComputeSeedRain/ComputeSpeciesSeedRain :1230–1283`; `GrowExistingRegeneration :1285–1356`; `EstablishNewCohorts/EstablishNewBands :1358–1426`; `PromoteCohorts/PromoteSpeciesCohorts/OldestQualifyingBand :1603–1681`; `GetWindRisk :1700`; `GetCompetitionIndex/GetCurrentSuppression/GetCompetitionLabel :1715–1744`; `RegenerationReportLine :1799–1859`; `RecomputeCanopy :812–841`; `TryPlantBeech/TryPlantJuvenile :621, 629`; planting fields `:29–35`. Also `ForestEcologyController.Storms.cs`, `StormModel.cs` *(species-reference grep)*.

**Scenario One:** `ScenarioOne/ScenarioOneDefinition.cs`; `ScenarioOne/ScenarioOne.asset`; `ScenarioOne/ScenarioOneManager.cs` *(excerpts: `TryDesignateExactPlanting :94–150`, pruning `:240–291, 1027–1066`, `OnTreeBiologicalDeath :488–510`, planting resolution `:1783–1805`, planted-juvenile annual step `:1880–1947`, visuals `:1573, 1666`, objectives/outcome `:2496–2597`)*; `ScenarioOne/ScenarioOneEconomyAdapter.cs`; `ScenarioOne/ScenarioOneObjectives.cs` *(species-reference grep)*; `ScenarioOne/WindthrowSalvage.cs`; `ScenarioHabitatInterpretation.cs`, `ScenarioSoundscape.cs`, `ScenarioEcologyReviewLines.cs`, `ScenarioHabitatVisuals.cs` *(species-reference grep)*; `ScenarioOne/Resources/ScenarioOneReferenceScheduleV1.json` *(species ids)*; `ForestStandScenarios.cs` (`ApplyLifecycleFixture`); `ForestStartingStand.cs`, `MixedSpeciesTestStand.cs` *(species grep)*.

**Timber/economy:** `TimberYield/{TimberYieldDomain, TimberYieldConfiguration, TimberYieldCalculator, TimberYieldEconomyAdapter, TimberYieldValidation, MerchantableStemModel}.cs`; `WorkEconomy/{ForestryWorkDomain, ForestryPriceBook, Stage1EconomyDefaults}.cs`; `ForestryWorkCalculator.cs` *(not read; species-agnostic by grep)*.

**UI / teaching:** `UI/TreeInspectionView.cs`; `UI/WorkPlanView.cs` *(excerpt)*; `UI/ScenarioOneUiFacts.cs`; `UI/CompetitorAssessment.cs`, `UI/CropTreeCompetition.cs`, `UI/StandMapView.cs`, `UI/LearningObjectivesView.cs`, `RegenerationDiagnosis.cs` *(species-reference grep)*; `ForestTreeMarkingManager.cs` *(excerpt `:377–397`)*.

**Persistence:** `ForestSaveData.cs`; `ForestSaveController.cs` *(excerpts `:142, 382–394, 483–485`)*; `ForestSaveValidation.cs` *(excerpts `:60–100, 288–304`)*, `ForestSaveValidation.{Geometry,Storms}.cs` *(species grep)*.

**Verification / docs:** `Tools/Verification/` (all 47 entries listed; species/growth-model greps over the Oak, Beech, planting, browsing, completion, economy, timber-yield, Sitka-growth harnesses); `Docs/Project/{decision-log, current-milestone, unity-project-overview}.md`; `Docs/BrowsingProtectionV1.md`, `Docs/BeechSpeciesParameters.md`, `Assets/ForestPrototype/Docs/SessileOakV1.md`; `Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md` §7–§8 *(read)*; `specs/scenario-one-three-species-parity-p0/*` and the Spec HANDOFF.

---

## 7. Unmerged three-species lineage (NOT main — inventory only)

Facts below are from `git log` / `git diff` / `git show` against `origin/main` (`3b499aa`); they were **not** reviewed line-by-line and their qualification claims were **not** re-verified.

| Item | Fact |
|---|---|
| Merge-base with `main` | `c1564b8` (the 2026-10-12 audit base). None of it is on `main`. |
| Main branch carrying it | `integration/scenario-one-tier3-candidate` @ `8f9ed80` (2026-10-18), 63 commits ahead of `main`; pushed. Also `tooling/linux-player-tier3` @ `5010613` (32 ahead). The Rack V2 save-foundation/runtime/headland lanes and the Scenario Two species lane (`feature/scenario-two-species-quantitative-closure-runtime-v1` @ `e26516f`) contain the same species files (`SpeciesSiteResponse.cs` etc.), i.e. they descend from it. |
| Programme record | `Docs/Verification/ThreeSpeciesProgramme/` on that branch (README, `Phase1-DensityRework.md`, `Phase6-BroadleafTimber.md`, `CompositeDensityV3Handoff.md`, …). README states it began as a cloud, **not Unity-verified** programme "CCF Overnight Master Implementation Programme — Scenario One Full Playable Demo" on base `c1564b8`; later density-v3 sections report Unity runs. |
| Manager-side awareness | `origin/audit/three-species-integration-readiness-v1` @ `26d2bf8` (2026-10-14) is a Manager-authored dependency/merge-order audit of exactly these lanes. Also unmerged. |
| Density | `GrowthModel.SpeciesAware = 2` (`Latest`); `TreeSpeciesDefinition.growingSpaceDemand` class (Reference/Intermediate/High; Oak = 2, Beech = 1 in the asset diff); `MixedDensityModel*.cs`, `SitkaSubsetDensitySignal.cs`; pure Sitka documented as a byte-identical legacy route; commits `e377a7d`, `639ae2c`, `271193c`; branches `task/mixed-density-architecture-rework-v2`, `…-v3`, `fix/density-v3-canopy-pulses-v1`. Its own docs state the first implementation (Sitka arithmetic × class weight) was **rejected** as an invented per-species max-density, and that victim selection became species-neutral because reusing `Ci50` produced a "28σ removal bias against broadleaves" — the same effect as G-02. |
| Site | `SpeciesSiteResponse.cs`, multidimensional site profile (`b6dbe23`, `c297008`) — dormant infrastructure, neutral response, no wired multiplier. |
| Timber/economy | `TimberAssortment` extended by appending `VeneerPlanking, Beam, Fencing, SecondPlanking, Firewood`; Oak/Beech ladders and prices (`b857bc6`, `97b83dd`) gated by growth model ≥ 2; Phase 6 doc records unverified assumptions (price basis roadside vs stumpage; 890 kg/m³ proxy). |
| Adult growth | `b3d33ab` "calibrate oak beech growth with conservative ordinal values". Asset diff vs `main`: Oak DBH 0.4→0.8, height 0.4→0.65, max H 40→42.5, Ci50 3→5; Beech DBH 0.4→0.9, height 0.4→0.75, max H 40→42.5, Ci50 3→6.5. **These are shared assets, so they apply to every growth model unless code-gated.** Research branches: `research/oak-beech-growth-preregistration-v1`, `research/oak-beech-ordinal-growth-calibration-v1`, `tools/oak-beech-growth-calibration-lab-v1`. |
| Other | Management history/two-cycle completion (save v20 stays 20), UX/teaching revisions, Linux/Windows build tooling. Out of scope here. |

**Manager options (not decided here):** (1) adopt the lineage as PARITY-03…06 candidates and make the parity tasks *requalification + gap-closure* work; (2) requalify only selected parts (e.g. density routing, timber ladders); (3) discard and rebuild from `main`. Whichever is chosen must be written into the spec's Problem/Plan and tasks, and the D-061/D-062 compliance of the lineage (class weights, Ci50 reuse, price basis) needs the independent review the spec already requires.

---

## 8. Protected anchors — what Oak/Beech work can and cannot move

Taken from `unity-project-overview.md` and the harnesses; hash values not re-emitted by this task.

| Group | Members | Contains broadleaves? | Moved by Oak/Beech asset edits? |
|---|---|---|---|
| Sitka-only lifecycle | neutral and normal 80-yr lifecycles (`ApplyLifecycleFixture` spawns 68 `DefaultSpecies` trees, `ForestStandScenarios.cs:109`); `BFC55473C1506067`, `3485B6630C9EA448`, regen-1 variants | No | **No** (non-default species have isolated RNG and no seed source) |
| Broadleaf-containing | Scenario One completion gates (plant Oak/Beech pairs: `ScenarioOneCompletionVerification.cs:238–243`), Model2 `702766DECE591E21`, Enlarged80 progression/economy gates | Yes | **Yes** |
| Reference continuation | Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D` (schedule plants Oak and Beech) | Yes | Yes — diagnostic only under D-042 |
| Reference integrity | embedded archive `ScenarioOneReferenceFutureV1.bytes` | n/a | No (bytes frozen; never reserialise) |
| Geometry/old-save | Legacy40 and Enlarged80 start hashes, save hardening | Sitka start | No |

A pure-Sitka density change cannot be proved safe by the lifecycle anchors alone for completion-level behaviour: those include broadleaves.

---

## 9. Evidence / parameter questions for PARITY-02

Questions only. No values are proposed. "Prior input" names evidence already in the repository.

| ID | Question | Prior input |
|---|---|---|
| Q-01 | **Growing-space-demand classes:** number of ordinal classes; Oak vs Beech vs Sitka rank and its evidence grade; definition of "pure Sitka" for the exact path (any non-Sitka living stem? basal-area share? promoted DBH 5 cm stems?); how to avoid a cliff at the route switch **without** DEC/DMC or a mixture bonus; whether classes feed only density or also hazard. | D-061; composite §1–§3; lineage `Phase1-DensityRework.md` (rank Oak>Beech called "weakest-supported") |
| Q-02 | Should density victim selection keep species `Ci50` (G-02) or use a species-neutral competitive index? Evidence for any differential broadleaf mortality. | G-02; H-01 |
| Q-03 | Oak and Beech adult DBH/height: ordinal ranking and horizon-appropriate targets (25-yr completion, 100-yr century); whether Oak ≡ Beech is acceptable; role of the unmodified `MaxDbh/MaxHeight`; whether the unsuppressed ≈35 cm DBH at age 100 is a defect or an intended Scenario One guard ("do not assume catch-up"). | composite §7 (TRANSFER, ordinal only, gate B); §3.1; G-03 |
| Q-04 | Is any species×site treatment needed for P0 given a single Class III Sitka site? Irish site requirements for Oak/Beech vs that site; simplest honest labelling if not modelled. | D-048; G-04; lineage `SpeciesSiteResponse` (dormant) |
| Q-05 | Do species differences in crown light transmission/shade (and leaf-off season) justify a species term in `RecomputeCanopy`, or is shared opacity an acceptable labelled limitation? | G-05; Overview k10a10 note |
| Q-06 | Is the 8 m Hegyi cutoff (Sitka-calibrated) appropriate for Oak/Beech crown reach, and what is the provenance of Oak/Beech `Ci50 = 3` [D]? | `:899–902`; `BeechSpeciesParameters.md:18` |
| Q-07 | Regeneration/juvenile: does Beech warrant distinct survival/establishment curves like Oak? Evidence for differing juvenile height growth (all 0.35), promotion height/light, initial height, mast probabilities and Oak/Beech seed sources; seed-saturation S50 (Sitka 10 vs 5). | G-16, G-17; `SessileOakV1.md`; `BeechRegeneration.md` |
| Q-08 | Nursery stock age/height/density per species (currently one "Beech v1" set). | G-11 |
| Q-09 | Browse: confirm palatability/escape numbers; whether recovery/resilience should differ by species; whether the shared 0.9 increment loss and 0.04 mortality are acceptable; persisted form damage stays deferred (D-044). | `BrowsingProtectionV1.md:48–59` |
| Q-10 | Timber assortment: Oak and Beech product ladders, thresholds, length/diameter rules, which quality inputs are authoritative (size and pruning history exist; fork/defect/browse-damage do not); stem form factor/taper for broadleaves; whether premium classes are reachable inside Scenario One horizons at current growth. | G-06, G-07; composite §7 guard |
| Q-11 | Economics: Irish roadside price evidence by Oak/Beech product (and basis: roadside vs stumpage); green density for Oak/Beech (current price model is €/t with a Sitka density); harvest and minimum-job treatment for hardwood; fate of the dormant 6,500¢/m³ value. | G-08, G-09; `Stage1EconomyDefaults.cs` |
| Q-12 | Pruning/Crop Tree: is a Sitka lift schedule defensible for Oak/Beech, and should Oak/Beech be excluded or given their own treatment? | G-10 |
| Q-13 | Evidence vocabulary mapping (see C-02). | §9 C-02 |
| Q-14 | Persistence/gating (see C-04): mechanism for versioning species behaviour; enum-extension policy; species-aware sale records; unknown-species handling. | G-13, G-15 |
| Q-15 | Which anchors may legitimately change under accepted parity (§8 groups 2–3) and who authorises each. | §8 |
| Q-16 | Should the inspection "Wind exposure" line be shown for Oak/Beech while all species share one set of wind multipliers? | row 17 |

**PARITY-02 can start now without waiting** on the Manager's lineage decision for Q-03…Q-12 (evidence closure is lineage-independent). Q-01, Q-02, Q-14 and Q-15 depend on the adopt/requalify/replace decision.

---

## 10. Corrections required to the draft parity spec / plan / tasks

| ID | Where | Correction |
|---|---|---|
| C-01 | `spec.md` Problem; `plan.md` Architecture & Sequence | Record the unmerged lineage (§7) and the Manager's adopt/requalify/replace decision. As written, T-03…T-06 are greenfield and will duplicate or conflict with it. The statement "P0 not started" must be scoped to `main`. |
| C-02 | `spec.md` FR-11 | The seven classes (`DIRECT, TRANSFER, MODEL DERIVATION, ORDINAL REPRESENTATION, GAMEPLAY CALIBRATION, FALLBACK-CONSERVATIVE, UNSUPPORTED`) match **none** of the code vocabularies: tooltip tags `[A][B][C][D][E][S][I]` on `TreeSpeciesDefinition`/docs, and `WorkEconomy.EvidenceLabel {Empirical, ManagementGuidance, Inference, SimulationAbstraction, GameplayCalibration}` (no Transfer, Ordinal or Unsupported). `EvidenceLabel` is not persisted (verified), so extending it is safe for saves, but a mapping table or enum extension must be specified. |
| C-03 | `plan.md` "Affected systems / files" | Replace the expected list with §6/§3.2 exact paths. Add: `ScenarioOneEconomyAdapter.cs`, `TimberYieldConfiguration.cs`, `TreeInspectionView.cs` (2 `CreateSitka` sites), `WorkPlanView.cs`, `ScenarioOneDefinition.cs`/`ScenarioOne.asset` (dormant price), `Species/*.asset`, `ForestSaveValidation.cs`/`ForestSaveController.cs`, `SitkaGrowthModel.cs` (`GrowthModel`), pruning (`ForestTree.cs`, `ScenarioOneManager.cs`). |
| C-04 | `spec.md` "Save / backward compatibility" | Answer the question the spec asks: species-specific **parameters** need no new persisted field, **but** because assets are unversioned (G-15/H-02) behaviour must be gated by a persisted model value. The smallest persisted change is extending the valid domain of the existing `growthModel` (currently `Latest = 1`, validated at `ForestSaveValidation.cs:80`) or an equivalent gate — a G5 decision even though no field is added. Separately: `ScenarioTimberSale` is species-less and enum-keyed (H-08); unknown `speciesId` loads as Sitka (G-13). |
| C-05 | `spec.md` FR-1 / AC-1 | State that `main` has no routing; "pure-Sitka path" = the existing statements `ForestEcologyController.cs:1040–1133` and `SitkaGrowthModel.cs`, which must stay byte-identical. Define "100 % Sitka" operationally (Q-01), including promoted juveniles and dead stems. |
| C-06 | `spec.md` FR-3 / `tasks.md` PARITY-04 | Add the `Ci50` coupling (H-01/G-02): adult-trait parity and density parity share a parameter and must be sequenced or co-specified. |
| C-07 | `spec.md` FR-6 / `tasks.md` PARITY-06 | "Form/quality/history that already exist authoritatively" are, on `main`, only pruning lifts/crown base and recorded suppression years. No fork/defect/browse-damage state exists, and standing-tree `Quality` is never populated. State which signals P0 may use, or that quality eligibility waits on a separately approved state. |
| C-08 | `spec.md` FR-7 | Name the dormant `broadleafTimberValueCentsPerCubicMetre` (6,500¢) as prohibited input or require its removal; name the Sitka 890 kg/m³ work proxy as a Sitka-derived assumption to be replaced or labelled. |
| C-09 | `spec.md` AC-1 / FR-10 | Partition "protected anchors" per §8; only the Sitka-only lifecycle group is insulated from Oak/Beech asset edits. |
| C-10 | `spec.md` AC-3/AC-4 and `tasks.md` | List the existing assertions that encode "unmarketed" (H-09) as authorised-change targets, not regressions. |
| C-11 | `spec.md` FR-12 / AC-8 | `main` has no species gate in Crop Tree/pruning/work-order creation; the guard must be built where MSFM lands. Add pruning (G-10) to the audited-species-blind list. |
| C-12 | `plan.md` Ownership | Species assets, the scene `ForestTreeSpawner` and `ScenarioOne.asset` are single-writer YAML; Rack V2 and Scenario Two lanes already carry modified copies of the same species files. State the single writer and merge order. |

---

## 11. Verification actually performed

**Performed (read-only):**
- `git fetch`; confirmed `origin/audit/…` tip `920b4d8` unchanged before publishing; confirmed context commit is an ancestor of base.
- `git diff c1564b85..HEAD` over `Assets Tools ProjectSettings Packages` → empty (production identity).
- Read the code and assets listed in §6; grepped all runtime species literals and species-referencing UI strings; read harness assertions (`Tools/Verification/`).
- Arithmetic with stdlib Python (`python3 -I -B`, no files written in the repo) on constants read from source: the `S⁴` weight table in G-02, juvenile time-to-promotion by light, and the unsuppressed adult DBH/height trajectory in G-03. These are **model derivations from verified constants, not simulation output**; assumptions: site 1, CI 0, promotion at 3.5 m/5.25 cm, tree age ≈12, no browsing, discrete annual steps as coded.
- `git log` / `git diff` / `git show` inventory of unmerged branches (§7).

**Not performed:**
- No Unity launch, compile, import, test, harness or build. No runtime claim is made.
- No review of the unmerged lineage beyond the §7 inventory; its Unity-qualification statements are unverified here.
- Not read: `ForestryWorkCalculator.cs` body, scene/prefab YAML (the `ForestTreeSpawner.speciesVisualSets` content is inferred from code), `ScenarioReferenceRunner/Archive` internals, Editor setup scripts, the PDFs named in the spec's evidence list. Unread areas were covered only by grep for species references.
- No dashboard sync, no `active-tasks.md` edit (Manager-owned).

`DASHBOARD SYNC: NOT PERFORMED`
`Published dashboard reflects this task: NO`

---

## 12. Standard handoff blocks

```text
IMPLEMENTATION HANDOFF
Task: PARITY-01 — Current-main implementation audit (READ-ONLY)
Worker: Claude Sonnet 5.5 (Claude Code)
Role: Primary read-only auditor
Branch: audit/scenario-one-three-species-parity-p0-current-main
Base: c3df69c8e28478993cfd1f9d3304e5f4d453e612
Context commit: eb218eab8b8e200a81cc99bfef7e8906bc21a88d
HEAD: 920b4d83efe6e71fd4be559950efb3ac4e6aef4f inspected; handoff commit = the commit adding this file (read back with git log)
GITHUB HANDOFF PATH: Docs/Handoffs/S1-ThreeSpeciesParity-P0-Audit/HANDOFF.md
EXTERNAL EVIDENCE ARCHIVE: none
WHAT CHANGED: this handoff document only
FILES CHANGED: Docs/Handoffs/S1-ThreeSpeciesParity-P0-Audit/HANDOFF.md
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none
Package/ProjectSettings changes: none
MUST-NOT-CHANGE HASHES / ANCHORS: all (none re-emitted; see §8 partition)
INTENTIONALLY CHANGED HASHES / ANCHORS: none
Tests actually run: none (static repository audit + stdlib arithmetic)
UNITY GATES STILL REQUIRED: none for this task; all parity implementation gates remain per the spec
```

```text
SPEC-DRIVEN HANDOFF
Spec: specs/scenario-one-three-species-parity-p0/ — DRAFT
ID / status: SPEC-scenario-one-three-species-parity-p0-V1 / DRAFT (unchanged by this task)
Conformance: NOT VERIFIED — audit task; no implementation exists to conform
Reasons: this task maps current behaviour; corrections C-01…C-12 are proposals for the Manager
Required fixes / remaining gates: Manager decision on the unmerged lineage; PARITY-02 evidence closure; spec approval; G2–G5 as the spec states
Integration target: none
```

`CURRENT SPECIES IMPLEMENTATION MAPPED`

**Return to Overall Manager.** Suggested immediate issue: PARITY-02 (evidence/parameter closure) for Q-03…Q-12 now, in parallel with the Manager's adopt/requalify/replace decision on the §7 lineage that gates Q-01, Q-02, Q-14, Q-15 and PARITY-03.
