# S1 Three-Species Parity P0 — PARITY-02 Evidence and Parameter Closure — Handoff

**Date:** 2026-10-27
**Worker / role:** Evidence/repository synthesis (read-only)
**Task:** `PARITY-02 — Scenario One three-species evidence and parameter closure`
**Spec:** `specs/scenario-one-three-species-parity-p0/` — DRAFT (not approved, not changed by this task)
**Branch:** `research/scenario-one-three-species-parity-p0-evidence-closure-v1`
**Branch base:** `c2dbb03777d22c378209ce16cadda48fd8bfc980` (Manager-reviewed PARITY-01)
**Context:** `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`
**Inspected branch HEAD:** `72a2a2a83b9d1fd814135c19f24330baffc28818`
**Inspected candidate lineage HEAD:** `8f9ed8025243254fe7378b6d7771a5f1e7efaccc` (`integration/scenario-one-tier3-candidate`)
**Active descendant check:** `8f9ed80` is an ancestor of the Rack V2 base `c6342f79a3e89c040fadd232017a937132779385` (save v21 there; the candidate itself is save v20).

## Verdict

`SPECIES PARAMETER MATRIX READY FOR MANAGER ACCEPTANCE`

This is an evidence/specification decision record. It does not approve the spec, does not authorise production implementation, and does not claim any candidate lineage value is empirically validated. No production code, Unity asset, scene, prefab, save or package was edited. Unity was not run.

The Manager decision that the inherited three-species lineage is the **candidate implementation baseline, subject to requalification and gap closure**, is applied throughout. Candidate values are treated as candidates to keep, relabel, recalibrate or reject, never as defaults.

---

## 1. Source identity and method

| Item | Result |
|---|---|
| Branch identity | Evidence branch HEAD `72a2a2a`; task packet only on top of `c2dbb03`. |
| Candidate identity | `8f9ed80` on `integration/scenario-one-tier3-candidate`; includes density v3 (`e377a7d`), Oak/Beech growth calibration (`b3d33ab`/`ea52050`), site infrastructure, broadleaf timber (`b857bc6`/`97b83dd`) and history phases. |
| Active lineage | `8f9ed80` is an ancestor of `c6342f79` (Rack V2 base, save v21, zero commits behind). This task did **not** review the Rack V2 descendant line-by-line. |
| Production state | This task changed no `Assets/`, `Packages/` or `ProjectSettings/` file; only docs were added. |
| Evidence method | Read the candidate code at `8f9ed80`, the programme's verification records, the canonical research composite, the Oak/Beech growth/site research, the Oak/Beech timber evidence pack, the species/browse/regeneration docs, and the accepted Decision Log. Re-fetched the Teagasc hardwood grading page to verify prices and sale basis. Ran stdlib-Python arithmetic on the candidate growth kernel (no repository writes). |
| Not run | No Unity launch, import, compile, test, harness or build. No candidate harness was re-run. No save was loaded. |

**Evidence labels used below:** `DIRECT`, `TRANSFER`, `MODEL DERIVATION`, `ORDINAL REPRESENTATION`, `GAMEPLAY CALIBRATION`, `FALLBACK-CONSERVATIVE`, `UNSUPPORTED — NEEDS DECISION`. Every row has exactly one primary label; secondary labels are shown where a rule combines an evidence direction with a calibrated magnitude.

---

## 2. Decision summary

1. **The candidate architecture survives.** Pure-Sitka exact routing, species-neutral density victims, ordinal growing-space classes, dormant neutral site infrastructure, species-explicit broadleaf ladders and the `GrowthModel.SpeciesAware = 2` gate are the right shape.
2. **Most inherited values survive as labelled calibration**, not as empirical coefficients. Oak/Beech adult growth, `Ci50`, class weights and density-response constants are `GAMEPLAY CALIBRATION`; the direction/rank is at best `TRANSFER`/`ORDINAL REPRESENTATION`.
3. **The Oak > Beech growing-space ordering is the weakest inherited claim.** It is retained only as a provisional ordinal representation; the Manager may collapse Oak and Beech to one descriptive class without changing the architecture.
4. **Beech does not get invented survival/establishment curves.** Oak's distinct curves stay; Beech's combined legacy response stays as a labelled conservative fallback.
5. **No species × site multiplier is activated.** The canonical Scenario One site remains a versioned Reference identity; the multidimensional site seam stays dormant and returns exactly 1.0.
6. **Canopy/light stays shared** (species differences only through crown radius/relaxation); the limitation must be stated, not papered over with invented transmissivity.
7. **The broadleaf timber ladders are broadly right**: Oak five rungs, Beech two rungs, no Beech veneer/beam/fencing/pulp. Quality is currently a **pruning-only proxy**; fork/defect/form state is not authorised and must not be implied in UI copy.
8. **The broadleaf prices are direct Irish trade guidance** (Teagasc “typical” quotations), not `Inference`; the selected reference values are `GAMEPLAY CALIBRATION`; the sale basis is `ROADSIDE` by consistency choice [C]. The dormant flat 6,500 c/m³ value must be removed or blocked.
9. **The Sitka pruning schedule is not acceptable for broadleaves.** Replace it with a broadleaf high-pruning branch (3.5 m / 6.0 m, two lifts) and defer formative shaping/fork state.
10. **The new default (`NewGameGrowthModel = SpeciesAware`) is accepted for the external demo**, while old saves on models 0/1 retain old behaviour. No parity-specific save field is needed; the active Rack V2 base is v21 and parity must add no field.
11. **Unknown saved species must be rejected**, not silently loaded as Sitka.
12. **Wind exposure must not present a species-looking band while storms are off.** Replace it with an explicit “storms off; not modelled” line.

---

## 3. Completed Q-01…Q-16 decision matrix

| Q | Decision / accepted rule | Evidence class | Candidate lineage verdict | Change needed | Anchor impact |
|---|---|---|---|---|---|
| **Q-01** Growing-space demand and pure-Sitka routing | Retain three ordinal classes: **Reference (Sitka) = 1.0, Intermediate (Beech) = 1.5, High (Oak) = 2.0**. Oak > Beech is **provisional ordinal** and must be labelled weak. Operationally, “100 % Sitka” means **every living individual tree** (adults plus promoted juveniles) is Reference; dead stems, stumps and sub-promotion regeneration cohorts do not count. Any living non-Reference tree switches the stand to the component route. No DEC, DMC or mixture bonus. | `ORDINAL REPRESENTATION` (rank); `GAMEPLAY CALIBRATION` (weights and response constants); `MODEL DERIVATION` (cross-section geometry and route) | **KEEP** architecture and weights; **KEEP + RELABEL** Oak > Beech as weak/provisional | Labels/docs; no numeric change unless the Manager collapses Oak/Beech. Do not add a per-species density capacity. | Pure-Sitka exact path unchanged; mixed model-2 anchors may move |
| **Q-02** Density victim selection / `Ci50` | `Ci50` remains a species **growth, crown-target and suppression-diagnostic** trait. Density victim selection is **species-neutral**: one common Hegyi half-saturation (5) and exponent (4) decide who dies. `Ci50` must not influence density mortality. | `MODEL DERIVATION` (double-counting argument); `GAMEPLAY CALIBRATION` (seam values) | **KEEP** species-neutral selection | None | Mixed only |
| **Q-03** Oak/Beech adult DBH and height | Keep candidate values: Oak `Pd 0.8`, `Ph 0.65`, `Dmax 120`, `Hmax 42.5`, `Ci50 5`; Beech `Pd 0.9`, `Ph 0.75`, `Dmax 120`, `Hmax 42.5`, `Ci50 6.5`. Label every value `GAMEPLAY CALIBRATION` (pre-registered grid centre), not an empirical coefficient. Keep the shared legacy asymptotic height law; do **not** copy the Sitka age/site envelope to broadleaves. | `TRANSFER` (Oak/Beech direction and rough envelopes; final source gate B, ordinal only); `MODEL DERIVATION` (trajectory arithmetic); `GAMEPLAY CALIBRATION` (all selected values) | **KEEP + RELABEL** values as calibration | Asset comments already say `[C]`; docs/spec must stop implying evidence-grade coefficients. Correct the stale “premium Oak unreachable” conclusion (§5). | Broadleaf-containing model-2 anchors; pure Sitka unchanged |
| **Q-04** Species × site response | **No wired multiplier in P0.** Keep `SpeciesSiteResponse`/`ForestSiteProfile`/`ScenarioSiteProfiles` dormant and neutral (exactly 1.0). The Scenario One site is a versioned Reference compatibility identity, not a soil survey or “optimal” site. Do not reset or repurpose `cell.SiteProductivity`. | `DIRECT`/`TRANSFER` for ordinal site directions; `GAMEPLAY CALIBRATION` for the neutral 1.0 identity; no numerical magnitude accepted | **KEEP** dormant neutral infrastructure | None in P0; if a site note is shown, label it qualitative | None |
| **Q-05** Canopy / light | Keep the shared canopy/light equation and implicit opacity 1.0. Species differences remain only through crown radius and crown relaxation. Do not invent transmissivity or deciduous-season coefficients. State the limitation explicitly. | `MODEL DERIVATION` / `GAMEPLAY CALIBRATION`; no species attenuation evidence in the repository | **KEEP + RELABEL** limitation | Documentation/UI wording only | Canopy/light hashes unchanged |
| **Q-06** Competition radius and `Ci50` provenance | Keep the shared 8 m Hegyi cutoff and the species `Ci50` values as **calibration**, not measured traits. The cutoff is Sitka-calibrated; no broadleaf neighbourhood radius is evidenced. `Ci50` values are grid-centre calibration and also affect crown target, so changing them is a cross-system change. | `GAMEPLAY CALIBRATION`; `ORDINAL REPRESENTATION` only if a rank is claimed | **KEEP + RELABEL** | Docs; no numeric change | Mixed only |
| **Q-07** Regeneration / juveniles / mast | Keep Oak's distinct survival/establishment curves. Keep Beech on the combined legacy survival/establishment response; label it `FALLBACK-CONSERVATIVE` and explain that Beech shade tolerance is expressed through its light-response curve, not a new curve. Keep shared 0.35 m/yr juvenile growth, 3.5 m promotion, density parameters and mast probabilities (0.15/0.35) as [C]. Keep species dispersal, maturity, mast multipliers, S50 and promotion-light values. | `TRANSFER`/`ORDINAL REPRESENTATION` (shade tolerance, Oak rank); `GAMEPLAY CALIBRATION` (shared values and probabilities); `FALLBACK-CONSERVATIVE` (Beech combined response) | **KEEP + RELABEL** | Documentation/UI label only | Broadleaf-containing regeneration/completion anchors |
| **Q-08** Nursery stock | Keep one shared broadleaf planting-stock abstraction: 3 years, 0.6 m, relative density 1.0. Relabel it from “Beech v1” to “broadleaf stock abstraction v1” because Oak uses it too. No per-species stock values without evidence. | `FALLBACK-CONSERVATIVE` / `GAMEPLAY CALIBRATION` | **KEEP + RELABEL** | Comment/UI label | Planting/completion anchors |
| **Q-09** Browsing / protection | Keep the palatability ordering Oak 1.0 > Beech 0.5 > Sitka 0.15 (`TRANSFER`) and escape heights 1.2–1.8 m (broadleaf) / 0.5–0.8 m (Sitka) (`TRANSFER`/simulation). Keep shared 0.9 height-increment loss and 0.04 extra mortality as `GAMEPLAY CALIBRATION`. Do not add species recovery/resilience differences or persisted browse-form damage in P0. | `TRANSFER` (ordering); `GAMEPLAY CALIBRATION` (numbers); `FALLBACK-CONSERVATIVE` (no recovery difference) | **KEEP + RELABEL** | Docs/UI label only | Browsing anchors |
| **Q-10** Broadleaf timber assortment | Keep Oak's five-rung ladder (veneer/planking → beam → fencing → second planking → firewood) and Beech's two-rung ladder (second planking → firewood). No Beech veneer, beam, fencing or pulp. Eligibility uses **size + pruning/crown history only**; no fork/defect/browse-damage state is authorised. Oak veneer/beam dimensions are direct Teagasc guidance; fencing/second/firewood dimensions are [C]. Label the pruning-only quality proxy. | `DIRECT` (Teagasc dimensions); `GAMEPLAY CALIBRATION` ([C] dimensions, quality proxy); `FALLBACK-CONSERVATIVE` (no fork state) | **KEEP + RELABEL** | Fix evidence labels and UI wording; no ladder change | Broadleaf harvest/economy anchors; Sitka unchanged |
| **Q-11** Broadleaf economics | Keep Oak/Beech prices as Teagasc “typical” €/m³ quotations: Oak veneer 180–280 (reference 230), beam 145, fencing 100, second planking 60; Beech second planking 60; firewood 30 [C] (20–40). Use **roadside** basis [C] for consistency with Sitka. Keep the 890 kg/m³ only as a **work-equivalent proxy**, not a density. No species harvest-cost multiplier. Remove/block the dormant flat 6,500 c/m³ value. A time-varying price deck is P1, not P0. | `DIRECT`/`MANAGEMENT GUIDANCE` (Teagasc quotations); `GAMEPLAY CALIBRATION` (selected reference, firewood, roadside basis); `MODEL DERIVATION` (unit conversion) | **KEEP + RELABEL**; **REMOVE** dormant flat value | `RangeEvidence` should be `ManagementGuidance`, not `Inference`; source/basis notes; remove flat value | Broadleaf economy/solvency; Sitka price book exact |
| **Q-12** Pruning / Crop Tree | Replace the species-blind Sitka schedule for broadleaves with a broadleaf **high-pruning** branch: two lifts at **3.5 m and 6.0 m**, minimum **3 years** apart, existing 8 % crown-radius reduction and 60 % height cap as [C]. Keep the Sitka schedule exactly (2.5/5/6.5 m, 5-year interval, three lifts). Formative shaping/fork removal is not P0. | `DIRECT`/`MANAGEMENT GUIDANCE` (3.5/6 m, 2–4 years); `GAMEPLAY CALIBRATION` (3-year interval, 8 %, cap); `FALLBACK-CONSERVATIVE` (no shaping) | **REPLACE RULE** | Species-specific lift targets/limits; UI copy | Broadleaf pruning/timber/completion anchors |
| **Q-13** Evidence vocabulary mapping | Project classes are authoritative. Runtime `[A][B][C][D][E][S][I]` and `WorkEconomy.EvidenceLabel` are coarse implementation/UI labels. Every parameter’s `Source`/comment must carry the project class. `UNSUPPORTED` has no runtime label and blocks implementation. Mapping table in §7. | Policy (not empirical) | **KEEP + RELABEL** | Fix price labels; add mapping to spec | None (labels are not persisted) |
| **Q-14** Persisted model/version gating | Keep `GrowthModel.SpeciesAware = 2` and `Latest = 2`; new games default 2; models 0/1 and old saves keep old behaviour. No parity-specific schema field. `TimberAssortment` stays append-only. Keep species-less `ScenarioTimberSale` for P0 while Oak/Beech shared classes have equal selected prices; add species at a later schema only if prices diverge. **Reject unknown saved species IDs** rather than loading Sitka. | `MODEL DERIVATION` / `GAMEPLAY CALIBRATION`; compatibility policy | **KEEP** gate/enum/save; **REPLACE RULE** unknown-species handling | Validation change; docs | Old-save/reference exact; new model-2 anchors |
| **Q-15** Anchor / change authority | Partition anchors in §9. Pure-Sitka lifecycle, Reference archive bytes, Legacy40 start/save hardening, pure-Sitka density fingerprints and Sitka timber fingerprints remain exact. Broadleaf-containing completion/economy/model-2 anchors are authorised-change candidates only with Manager approval. Reference continuation hashes are diagnostic under D-042. | Compatibility policy | **KEEP + LABEL** | Verification/doc updates | See §9 |
| **Q-16** Player-facing wind exposure | While `StormModel.None`, do not show a `Stable/Watch/Exposed` band. Show “Storms are off in Scenario One; wind exposure is not modelled for any species.” When storms are active, label the shared model and the absence of species multipliers. No species wind multipliers in P0. | `GAMEPLAY CALIBRATION`; no species wind evidence | **REPLACE RULE** | UI line only | Presentation only |

---

## 4. Candidate-lineage KEEP / CHANGE verdicts

| Inherited area | Verdict | Reason |
|---|---|---|
| `GrowthModel.SpeciesAware = 2`, pure-Sitka routing | **KEEP** | Correct gate; pure Sitka remains the original statements; v3 claims exact equality. |
| `MixedDensityModel*`, `SitkaSubsetDensitySignal`, component route | **KEEP** | No per-species capacity; Sitka component separated; generic response species-neutral. |
| Class weights 1.0 / 1.5 / 2.0 and Oak > Beech | **KEEP + RELABEL** | Weights are calibration; Oak > Beech is weak provisional ordinal. |
| Density victim selection (neutral Hegyi) | **KEEP** | Removes the old `Ci50`/S⁴ broadleaf bias; no differential mortality evidence. |
| Oak/Beech asset growth changes | **KEEP + RELABEL** | Grid-centre calibration, not empirical; direction supported, values not. |
| `SpeciesSiteResponse` / `ForestSiteProfile` / `ScenarioSiteProfiles` | **KEEP** | Dormant, neutral, versioned; no invented multiplier. |
| Broadleaf `TimberAssortment` extension and ladders | **KEEP + RELABEL** | Ladder shape matches evidence; quality proxy and labels need correction. |
| Broadleaf prices, densities, market bindings | **KEEP + RELABEL** | Values match Teagasc; units correct; `Inference` label is wrong; roadside basis [C]. |
| 890 kg/m³ work proxy | **KEEP + RELABEL** | Correct as a work-equivalent unit convention; not a density. |
| Dormant flat `broadleafTimberValueCentsPerCubicMetre` | **REMOVE** | No caller; exact “flat broadleaf rate” D-062 forbids. Remove or block in PARITY-06. |
| Species-blind pruning schedule | **REPLACE RULE** | Sitka clear-stem practice is not broadleaf high pruning; use 3.5/6.0 m two-lift branch. |
| Unknown species load | **REPLACE RULE** | Silent fallback to Sitka mutates meaning; reject instead. |
| Species-less `ScenarioTimberSale` | **KEEP** (condition) | P0 shared classes have equal selected prices; no schema change. Revisit if prices diverge. |
| New-game growth model 2 | **KEEP** | External demo needs parity; old saves remain 0/1. |
| Beech combined juvenile survival/establishment | **KEEP + RELABEL** | No Beech-specific curve evidence; shade tolerance is in the light response. |
| “Beech v1” planting stock | **KEEP + RELABEL** | Shared by Oak; relabel the abstraction. |
| Shared canopy/light | **KEEP + RELABEL** | No transmissivity evidence; document limitation. |
| 8 m Hegyi cutoff / `Ci50` | **KEEP + RELABEL** | Calibration, not species evidence; cross-system coupling remains. |
| Shared mast probabilities | **KEEP + RELABEL** | No species-specific probability evidence; label. |
| Timber inspection/Work Plan wording | **REPLACE RULE** | Must not imply form/straightness grading or a species market where none exists. |
| Wind exposure line | **REPLACE RULE** | Band looks species-specific while storms are off and multipliers are shared. |

---

## 5. Confirmed implementation findings that change the draft assumptions

### 5.1 Premium Oak is not necessarily “unreachable” after the growth calibration

The inherited Phase 6 record (“premium Oak close to unreachable”) was computed with the **pre-calibration** `Pd = 0.4` values. With the adopted candidate `Pd = 0.8`/`Ph = 0.65`, a simple repetition of the production kernel from a promoted 3.5 m/5.25 cm stem (age ≈10, site 1, no browsing) gives [D]:

| Species | CI | Age 50 | Age 100 | Age 112 |
|---|---:|---|---|---|
| Oak | 0 | D 32.2 cm, H 21.4 m | D 57.2 cm, H 32.8 m | D 62.0 cm, H 34.4 m |
| Oak | 2 | D 25.2 cm, H 21.4 m | D 45.3 cm, H 32.8 m | D 49.5 cm, H 34.4 m |
| Beech | 0 | D 35.1 cm, H 23.4 m | D 61.7 cm, H 34.6 m | D 66.8 cm, H 36.2 m |
| Beech | 2 | D 28.8 cm, H 23.4 m | D 51.6 cm, H 34.6 m | D 56.2 cm, H 36.2 m |

These are **model derivations from the candidate constants**, not simulation output and not empirical expectations. They say only that, at low competition, pruned Oak can plausibly reach a 45 cm mid-log inside a Century Review; at moderate competition it is marginal. The Phase 6 “unreachable” statement must be corrected to “rare/late, low-competition and pruning-dependent, and not guaranteed”. This is a teaching decision for the Manager, not a licence to accelerate growth or lower the assortment threshold.

### 5.2 The candidate price labels are wrong

The Teagasc page (re-fetched 2026-10-27) states the Oak first-grade, beam, fencing and second-quality planking prices as “typical prices quoted” per m³ (first grade “€180–€280/m³ (or roughly per tonne)”). These are **DIRECT Irish trade guidance** (`ManagementGuidance` in the runtime vocabulary), not `Inference`. The candidate uses `EvidenceLabel.Inference` for all non-firewood broadleaf prices. The selected reference values remain `GameplayCalibration`. The sale basis is unstated in the source; the candidate’s roadside treatment is a labelled consistency choice [C].

### 5.3 The candidate’s quality model is pruning-only

`ScenarioOneEconomyAdapter.PrunedSections` is the only live quality input for Oak; no fork, sweep, defect or browse-form state exists on living trees. The ladder therefore cannot distinguish a straight pruned Oak from a forked pruned Oak. This is allowed by the PARITY-02 boundary (no new quality state solely for pricing), but the UI must say so. The current inspection copy says “size and form” and “large and straight enough”; that overstates what is modelled. It must say “size and pruning history in this version; fork/defect shape is not yet modelled.”

### 5.4 Beech height calibration is marginal, not failed

The calibration record reports a Beech provisional-height distance of 0.188 (Oak 0.0), with Beech at ≈23.4 m at age 50 and ≈34.6 m at age 100 in the low-competition arithmetic, against provisional 25–30 m and 35–40 m bands. The values are still the best available labelled calibration; the band miss is a known limitation, not an empirical rejection. Do not “fix” it by inventing a new height model; record it and review after PARITY-03/04 requalification.

---

## 6. Remaining `UNSUPPORTED — NEEDS DECISION`

These are deliberately not resolved by convenience. None blocks the evidence matrix; each is either deferred with a stated P0 boundary or requires an explicit Manager choice before the named task.

| ID | Item | Status / recommended handling |
|---|---|---|
| U-01 | Oak vs Beech growing-space class ordering | Retained only as provisional ordinal (Oak High, Beech Intermediate). If the Manager requires evidence-only distinctions, collapse both to Intermediate (asset value change, no architecture change). |
| U-02 | Formative shaping and permanent fork state | Not P0. Needs a form/fork origin and persisted state; do not invent it solely for premium pricing (D-044 boundary). |
| U-03 | Beech distinct juvenile survival/establishment curves | Not implemented. Keep combined legacy response labelled; if structural symmetry is required, that is a new calibration with its own evidence packet. |
| U-04 | Species attribution in persisted timber-sale records | Not required while Oak/Beech shared classes have equal selected prices. Add species at the next schema only if prices diverge; would be G5. |
| U-05 | Standing vs roadside price basis | Source silent. Roadside [C] chosen to match Sitka; a standing-sale basis would be a new calibration, not evidence. |
| U-06 | Broadleaf harvest-cost differentiation | No Irish rate series found. Keep generic cost; future evidence only. |
| U-07 | Oak/Beech green density | No measurement found. Keep 890 kg/m³ work-equivalent proxy; never present as density. |
| U-08 | Species × site numerical response | No magnitude evidence. Keep dormant/neutral; future site identity and calibration required. |
| U-09 | Species canopy transmissivity / deciduous season | No evidence. Keep shared opacity; document limitation. |
| U-10 | Species wind multipliers / storm activation | All multipliers are 1 and storms are off. Keep shared; future storm/species evidence only. |
| U-11 | Beech provisional height-band miss | Accept as labelled calibration limitation; recalibrate only with a new evidence packet. |
| U-12 | Premium Oak reachability and Century Review teaching | Arithmetic says rare/low-competition/pruned Oak can reach veneer size late; decide whether to teach that, without changing growth or thresholds. |

---

## 7. Evidence-vocabulary mapping (binding policy)

**Authority rule:** the project class (`DIRECT`, `TRANSFER`, `MODEL DERIVATION`, `ORDINAL REPRESENTATION`, `GAMEPLAY CALIBRATION`, `FALLBACK-CONSERVATIVE`, `UNSUPPORTED — NEEDS DECISION`) is the authoritative research label. Runtime tags and `WorkEconomy.EvidenceLabel` are coarse implementation/UI labels and must never be used as the research classification.

| Project class | Runtime tag(s) | `WorkEconomy.EvidenceLabel` | Rule |
|---|---|---|---|
| `DIRECT` | `[A]` | `Empirical` or `ManagementGuidance` | Use `Empirical` for measured data; `ManagementGuidance` for official/practitioner guidance and “typical” quotations. |
| `TRANSFER` | `[E]` (or `[B]` with a transfer note) | `Inference` or `ManagementGuidance` | The `Source` must name the transfer and its limits. |
| `MODEL DERIVATION` | `[B]` / `[I]` | `Inference` or `SimulationAbstraction` | The `Source` must name the accepted model/data and assumptions. |
| `ORDINAL REPRESENTATION` | `[B]` / `[I]` | `SimulationAbstraction` | Rank/class only; no numeric magnitude claim. |
| `GAMEPLAY CALIBRATION` | `[C]` / `[D]` / `[S]` | `GameplayCalibration` | The selected value is a game choice; show the range/evidence separately where one exists. |
| `FALLBACK-CONSERVATIVE` | `[C]` / `[S]` | `SimulationAbstraction` or `GameplayCalibration` | Deliberately cautious default; state why evidence is insufficient. |
| `UNSUPPORTED — NEEDS DECISION` | `[U]` | **no runtime value** | Do not implement; stop and return to the Manager. |

**Implementation consequences:** `[C]` is not automatically `SimulationAbstraction`; it is `GAMEPLAY CALIBRATION` unless the item is only an ordinal/structural representation. `[S]` can be `ORDINAL REPRESENTATION`, `MODEL DERIVATION` or `GAMEPLAY CALIBRATION`, and the `Source` string must say which. The `EvidenceLabel` enum is not persisted, so extending it is save-safe; do not extend it merely to encode research classes. Fix the Teagasc price labels to `ManagementGuidance`.

---

## 8. Exact corrections required to the DRAFT spec / plan / tasks / checklist

The audit's C-01…C-12 remain required as accepted by the Manager review; C-02 is expanded by the mapping in §7. This table adds the evidence-closure corrections.

| ID | File / section | Required correction |
|---|---|---|
| C-13 | `spec.md` Problem, Goals, Open decisions | Record the adopted candidate lineage and the adopt/requalify decision. Scope “P0 not started” to `main` only. Remove the implication that PARITY-03…06 are greenfield. |
| C-14 | `spec.md` FR-1 / AC-1 | Define “100 % Sitka” operationally: living individual trees (adults + promoted juveniles) only; dead stems, stumps and sub-promotion cohorts excluded; model <2 always legacy. State that the protected statements are the existing controller block and `SitkaGrowthModel` bytes. |
| C-15 | `spec.md` FR-2 / AC-2 | Fix the class table: Reference/Beech/Oak = 1.0/1.5/2.0; Oak > Beech is provisional ordinal, not evidence; no DEC/DMC/mixture bonus; state the route-continuity test. |
| C-16 | `spec.md` FR-3 | State the accepted adult values are `GAMEPLAY CALIBRATION`; keep the legacy asymptotic height law; no species×site multiplier; canopy/light shared; `Ci50` remains growth/crown/suppression, not density victim selection. |
| C-17 | `spec.md` FR-4 | Oak distinct curves stay; Beech combined legacy response is a labelled fallback; shared juvenile growth/promotion/density values; mast probabilities shared and labelled. |
| C-18 | `spec.md` FR-5 | State the browse ordering (`TRANSFER`), the number tags, and that recovery/resilience and persistent form damage are not differentiated in P0. |
| C-19 | `spec.md` FR-6 / AC-4 | State the exact Oak five-rung and Beech two-rung ladders; quality inputs are size + pruning/crown history only; no fork/defect state; Oak veneer/beam dimensions direct, other dimensions [C]. |
| C-20 | `spec.md` FR-7 | State prices as Teagasc typical €/m³, roadside [C], selected reference [C], firewood [C], 890 work proxy [S], no flat 6,500 value, no price deck in P0. |
| C-21 | `spec.md` FR-8 | Require the UI to say “size and pruning history; fork/defect shape not modelled”; no “straight enough” claim; wind line labelled shared/storms off; site/canopy limitations stated. |
| C-22 | `spec.md` FR-10 / AC-1 | Replace the anchor sentence with the §9 partition: pure Sitka/reference archive exact; broadleaf-containing anchors authorised-change candidates; continuation diagnostic. |
| C-23 | `spec.md` FR-11 | Insert the §7 mapping and the rule that `UNSUPPORTED` blocks implementation. |
| C-24 | `spec.md` FR-12 / AC-8 | Add pruning to the species-blind audit surface: broadleaf high-pruning branch (3.5/6.0 m, two lifts); MSFM still Sitka-adult-only. |
| C-25 | `spec.md` Save / backward compatibility | No parity-specific schema field. `GrowthModel.SpeciesAware = 2` is the gate; old saves 0/1 unchanged; active Rack V2 base is v21, parity adds no field; reject unknown species; species-less sale records accepted for P0 with the U-04 condition. |
| C-26 | `spec.md` AC-1…AC-9 | Attach the decisions above to each AC; AC-9 must include the evidence-class mapping and the no-pseudo-precision check. |
| C-27 | `plan.md` Architecture / current-state findings | Replace the stale “current main lacks parity” framing with the adopted candidate lineage, its save v20/v21 relationship, and the requalification requirement. |
| C-28 | `plan.md` Affected systems / Ownership | Use §10 exact file groups and single-writer order; note species assets/scene YAML are high-conflict. |
| C-29 | `plan.md` Implementation sequence | Recast PARITY-03…09 as requalification + gap closure; put density first, label-only adult/early-life next, timber/pruning/UI after, save/determinism and integrated qualification last. |
| C-30 | `tasks.md` | Mark PARITY-01 `COMPLETE` and PARITY-02 `COMPLETE — READY FOR MANAGER ACCEPTANCE`; change PARITY-02 completion state from `SPECIES PARAMETER MATRIX ACCEPTED` to `SPECIES PARAMETER MATRIX READY FOR MANAGER ACCEPTANCE`; add the pruning/UI/unknown-species work. |
| C-31 | `checklists/requirements.md` | Add checks for the §7 mapping, §9 anchors, broadleaf pruning branch, wind wording, unknown-species rejection, sale-record condition, and “no unsupported coefficient” review. |
| C-32 | `Docs/Project/{decision-log,unity-project-overview,current-milestone}.md` | Manager-owned: record the adopt/requalify decision, the PARITY-02 decisions, the new default model 2, the pruning branch and the corrected premium-Oak finding. Do not edit them in a worker task without authority. |

---

## 9. Anchor / change-authority table

Hash values are as recorded in `unity-project-overview.md`; this task did not re-run them.

| Group | Members | Authority |
|---|---|---|
| **Must remain exact** | Pure-Sitka lifecycle `BFC55473C1506067` (0/0/0), `3485B6630C9EA448` (browse 0.2), `7E57B9DAEF5BF4D0` (1/1/1), `35E2BF1C2F55C2DE` (Scenario One 1/1/1); Legacy40 start `EBE7A228F630AE41`; pure-Sitka density fingerprints `1a81640d…`, `ee7dc266…`, `80ef453c…`; Sitka timber fingerprint; Reference archive bytes/integrity. | Protected. No parity task may rebaseline. |
| **Authorised-change candidates** | `702766DECE591E21` (Model2 completion), Enlarged80 start `04A78188A6F8E6F9` / year 3 `081DB58CE3034648`, model-2 start/year1 `FA855239CDDA32D8` / `02334804F65C0234`, completion/economy/solvency witnesses, new mixed-stand anchors. | May move only with explicit Manager approval and labelled new anchors; measure under G3. |
| **Diagnostic only** | Reference Y100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D` (D-042), historical model-1 completion `D7C4DDD36B53FCCE`, performance timings, old mixed-density candidate anchors. | Record and explain, never use as a silent pass/fail gate. |
| **Legacy/old-save** | Legacy40 start/save hardening; v16/v17/v20/v21 layout anchors in their original configurations. | Exact under their original configuration; parity must not reinterpret them. |

A pure-Sitka density change can be checked by the pure-Sitka anchors alone; **completion-level behaviour cannot**, because the completion fixtures contain broadleaves. That is why the broadleaf group needs its own labelled model-2 anchors.

---

## 10. Affected shared files and proposed single-writer order

| Order | File / group | Writer / conflict note |
|---:|---|---|
| 1 | `specs/scenario-one-three-species-parity-p0/*`, this handoff and the research matrix | Overall Manager / evidence worker; no production. |
| 2 | Density: `Assets/ForestPrototype/MixedDensityModel.cs`, `MixedDensityModel.Composite.cs`, `SitkaSubsetDensitySignal.cs`, `ForestEcologyController.cs`, `AdultMortalityAccount.cs` | One density writer. Do not overlap Rack V2. Pure-Sitka fingerprints must be checked before/after. |
| 3 | Species data: `Assets/ForestPrototype/Species/{SessileOak,BeechSpecies,SitkaSpruce}.asset`, the scene's `ForestTreeSpawner` `KnownSpecies`/visual sets | **Single-writer YAML**, high conflict. Sitka asset is protected. Coordinate with Rack V2 and Scenario Two; do not copy assets between worktrees. |
| 4 | Timber/economy: `TimberYield/{TimberYieldConfiguration,TimberYieldCalculator,TimberYieldEconomyAdapter}.cs`, `WorkEconomy/{ForestryWorkDomain,Stage1EconomyDefaults}.cs`, `ScenarioOne/ScenarioOneEconomyAdapter.cs`, `ScenarioOne/ScenarioOneDefinition.cs` + `ScenarioOne.asset` | One economy writer; price/ladder content; asset edit for the dormant flat value. |
| 5 | Pruning/UI: `ForestTree.cs`, `ScenarioOne/ScenarioOneManager.cs`, `ScenarioOne/ScenarioOneDefinition.cs`, `UI/TreeInspectionView.cs`, `UI/WorkPlanView.cs` | One writer; pruning and wording are coupled to timber eligibility. |
| 6 | Save/validation: `ForestSaveData.cs`, `ForestSaveController.cs`, `ForestSaveValidation.cs` | One writer; unknown-species rejection; no parity field. G5 if any field is added. |
| 7 | Verification: `Tools/Verification/ThreeSpecies/*`, `Tools/Verification/*` | One verifier; run after production slices; do not leave disposable scripts in `Assets/`. |
| 8 | Canonical docs (`decision-log.md`, `unity-project-overview.md`, `current-milestone.md`) | Manager-owned; update after acceptance. |

**Merge order:** spec approval → density → species assets → timber/pruning/UI → save → integrated qualification. If Rack V2 core integration is still active, freeze species asset/scene edits until its single writer releases them; rebase the parity branch rather than resolving YAML mechanically. The active Rack V2 base is save v21; parity adds no field.

---

## 11. Recommended PARITY-03…PARITY-09 sequence

| Task | Scope | Why this order |
|---|---|---|
| **PARITY-03** | Density requalification: adopt the v3 component route; retain the class table (or collapse Oak/Beech if the Manager rejects U-01); prove pure-Sitka exactness; define 100 % Sitka in code/docs; measure a mixed stand and record new labelled anchors. | Highest risk; anything later can move mixed anchors. |
| **PARITY-04** | Adult/canopy/site requalification: keep the calibrated Oak/Beech values, label them; keep the legacy height law; keep site dormant; document `Ci50` and the 8 m cutoff; run adult growth/site-neutral gates. | Mostly label/verify; no new model. |
| **PARITY-05** | Early-life/browse requalification: keep Oak curves; label Beech combined response; relabel the stock abstraction; keep browse ordering/numbers; run regeneration/browse gates under model 2. | Depends on density for mixed stands, but not on timber. |
| **PARITY-06** | Broadleaf timber/economy: keep ladders; fix evidence labels; roadside basis; remove/block the flat value; decide sale-record condition; run timber/economy gates and solvency measurement. | Depends on Q-10/Q-11; independent of pruning code. |
| **PARITY-06b** | Pruning and player-facing timber wording: broadleaf high-pruning branch; replace “form/straight” claims; wind line. | Depends on the timber eligibility model and Q-12. |
| **PARITY-07** | Player-facing parity: inspection/teaching/Work Plan copy across species, with evidence limits and the pruning-only quality caveat. | After actual behaviour is fixed. |
| **PARITY-08** | Save/determinism: unknown-species rejection; enum append-only check; model gate; old-save/Legacy40/Reference archive; new model-2 anchors. | Depends on all behaviour; G5 only if a field is added. |
| **PARITY-09** | Integrated qualification: pure/mixed fixtures, completion/economy, rendered review, MSFM Sitka-only guard, independent evidence/spec-conformance review. | Final gate; no external Windows build. |

**Sequencing notes:** PARITY-04 and PARITY-05 are mostly label/verify and can run in parallel with PARITY-06 if they touch different files. PARITY-03 must precede any mixed-stand measurement. PARITY-06b must precede PARITY-07. PARITY-08 must follow PARITY-03…06. Do not dispatch PARITY-09 until the Manager approves the amended spec and the broadleaf anchor list.

---

## 12. Verification actually performed

**Performed (read-only):**
- Confirmed the evidence branch base and HEAD (`c2dbb03` → `72a2a2a`); confirmed `8f9ed80` is an ancestor of `c6342f79`; confirmed the candidate is save v20 and the Rack V2 base is save v21.
- Read the candidate code and docs: `MixedDensityModel*`, `SitkaSubsetDensitySignal`, `ForestEcologyController` density/growth/canopy/pruning paths, `SitkaGrowthModel`, `TreeSpeciesDefinition`, `JuvenileEcologyRules`, `SpeciesSiteResponse`/`ForestSiteProfile`/`ScenarioSiteProfiles`, `TimberYieldConfiguration`, `Stage1EconomyDefaults`, `ScenarioOneEconomyAdapter`, `ForestSaveData`/`Controller`/`Validation`, `TreeInspectionView`, `WorkPlanView`, species assets, and the `ThreeSpecies` verification records.
- Read the canonical composite, the Oak/Beech growth/site research (`OakBeechAdultGrowthSiteSpecification_v2`, preregistration, calibration design, production calibration handoff), the Oak/Beech timber evidence pack, the Beech/Oak/browse/regeneration docs, the Decision Log, and the current overview.
- Re-fetched and read the Teagasc hardwood grading page (2026-10-27) to verify the “typical” €/m³ prices and the standing-vs-roadside statements.
- Ran stdlib-Python arithmetic on the candidate growth kernel for the trajectories in §5.1 (site 1, CI 0/2, promoted 3.5 m/5.25 cm at age ≈10, no browsing, annual discrete steps). These are model derivations, not Unity output.

**Not performed:**
- No Unity launch, import, compile, test, harness or build. No candidate qualification claim was re-run.
- No Unity save/load, Reference replay, rendering or performance check.
- No review of the Rack V2 descendant code or the full Scenario Two lineage.
- No inspection of the PDFs named in the research index; the indexed composite/evidence pack and the Teagasc page were used.
- No dashboard sync; no `active-tasks.md` edit (Manager-owned).

`DASHBOARD SYNC: NOT AVAILABLE`
`Published dashboard reflects this task: NO`

---

## 13. Standard handoff blocks

```text
IMPLEMENTATION HANDOFF
Task: PARITY-02 — Scenario One three-species evidence and parameter closure (READ-ONLY)
Worker: Evidence/repository synthesis
Role: Primary evidence auditor
Branch: research/scenario-one-three-species-parity-p0-evidence-closure-v1
Base: c2dbb03777d22c378209ce16cadda48fd8bfc980
Context commit: eb218eab8b8e200a81cc99bfef7e8906bc21a88d
HEAD: 72a2a2a83b9d1fd814135c19f24330baffc28818 inspected; handoff commit = the commit adding this file (read back with git log)
GITHUB HANDOFF PATH: Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md
EXTERNAL EVIDENCE ARCHIVE: none (research/code reads; Teagasc page re-fetched)
WHAT CHANGED: this handoff and the detailed evidence matrix only
FILES CHANGED: Docs/Handoffs/S1-ThreeSpeciesParity-P0-EvidenceClosure/HANDOFF.md; Docs/Research/ScenarioOneThreeSpeciesParityP0EvidenceClosure.md
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none
Package/ProjectSettings changes: none
MUST-NOT-CHANGE HASHES / ANCHORS: pure-Sitka lifecycle, Reference archive, Legacy40 start, pure-Sitka density and Sitka timber fingerprints
INTENTIONALLY CHANGED HASHES / ANCHORS: none
Tests actually run: none (static evidence audit + stdlib arithmetic)
UNITY GATES STILL REQUIRED: all parity implementation gates; candidate qualification is not re-verified here
```

```text
SPEC-DRIVEN HANDOFF
Spec: specs/scenario-one-three-species-parity-p0/ — DRAFT
ID / status: SPEC-scenario-one-three-species-parity-p0-V1 / DRAFT (unchanged by this task)
Conformance: NOT VERIFIED — evidence/decision task; no implementation exists on this branch
Reasons: this task closes evidence/parameter questions; C-13…C-32 are proposals for the Manager
Required fixes / remaining gates: Manager acceptance of the matrix; spec amendments; PARITY-03 requalification; G2–G5 as stated
Integration target: none
```

`SPECIES PARAMETER MATRIX READY FOR MANAGER ACCEPTANCE`

**Return to Overall Manager.** Recommended immediate action: accept or amend the §3 decisions, resolve U-01/U-11/U-12, then issue PARITY-03 (density requalification) with the §9 anchor authority and the §10 single-writer order. PARITY-04/05 can run as label-only requalification in parallel; PARITY-06/06b follow once the spec is amended.
