# Current model audit — Sitka growth, height and adult mortality

Base `origin/main` @ 3e4ee40 (Regeneration Model 1 @ 4670e73). Branch `task/sitka-site-height-adult-mortality`. **No production code was changed in this task.**

Evidence grades follow the synthesis key:
- **A** — Irish empirical;
- **B** — British/near empirical;
- **C** — review / modelled;
- **D** — design/calibration decision;
- **I** — inferred / assumption.

## Parameters and mechanisms

| Item | Value | Code location | Source status | Grade | Current effect |
|---|---|---|---|---|---|
| Site productivity | 1.0 in every cell; never set anywhere | `ForestEcologyCell.SiteProductivity` (default), read by `ForestEcologyController.GetSiteProductivity` | None; placeholder | D | Multiplies DBH and height growth and juvenile growth; currently inert |
| Adult height growth | `dH/dt = 0.45 · site · (1 − H/35)` m/yr; age-independent; no competition term | `GrowAdults`; `SitkaSpruce.asset` `potentialHeightGrowthMPerYear` 0.45, `maxHeightM` 35 | Not evidence-calibrated (earlier calibration explicitly left "height vs yield-class curves" open) | D | Top height 14.6 → 17.1 m (age 20 → 30), 22.1 m at 60, 29.0 m at 120 |
| Potential vs realised height | Not separated: every tree follows the same Mitscherlich law from its own height | `GrowAdults` | — | D | Suppressed trees converge on dominants (mean/top 0.80 at 20 → 0.97 at 120) |
| Adult DBH growth | `1.2 · site · (1 − D/120) / (1 + CI/Ci50)` cm/yr, Ci50 5 | `GrowAdults`; asset `potentialDbhGrowthCmPerYear` 1.2, `maxDbhCm` 120, `ci50` 5 | Calibrated (C8): Frenchpark thinning ratios reproduced; absolute value only loosely constrained (0.9–1.5 range) | C (response) / I (absolute) | Unthinned same-tree growth 0.40 cm/yr (age 20–30) → 0.30 (80–120) |
| Competition | Hegyi `Σ (Dj/Di)/dist`, 8 m cutoff | `UpdateCompetition`, `HegyiCutoffMeters` | Established index (C8 report); cutoff [C] | B (method) / C (cutoff) | Mean CI 8.2 at age 20 and 8.6–9.2 throughout 100 unthinned years (scale-invariant, see below) |
| Crown | Potential `0.9415 + 0.07635 · DBH` m (Tabbush & White), reduced by `1/(1 + CI/Ci50)`, relaxation 0.15/yr | `RelaxCrowns`; asset crown fields | Tabbush open-grown envelope; competition reduction is a model choice | B (envelope) / D (reduction) | Independent of height |
| Canopy light | Crown reach × vertical term `clamp01(H/8)` | `RecomputeCanopy` | k10a10 calibration | C | Saturates at 8 m, so adult height changes do not change light |
| Stem volume | `D² · 0.00007854 · H · 0.5` | `ForestTree.BiologicalStemVolumeM3` (`formHeightRatio` 0.5) | Simple form factor | D | 249 m³/ha at start; 5,937 m³/ha at 120 unthinned (no mortality) |
| Biological mortality | `ApplyMortality(cause, year)`: idempotent, hides the tree, invokes `MortalityApplied` | `ForestTree.ApplyMortality` | Foundation API only; **no annual trigger calls it** | D | No adult tree ever dies biologically |
| Death cause/year | `biologicallyDead`, `mortalityCause`, `mortalityYear` | `ForestTree`, `TreeSaveData` (v14+) | — | D | Saved; no producer |
| Ecology reaction to death | Removes CI/growth/seed entries; queues a canopy rebuild | `ForestEcologyController.OnTreeMortality` | — | D | Neighbours are released next step |
| Deadwood | `ScenarioDeadwoodRecord` (volume, decay, fallen year, visual), saved in `scenarioOne.deadwoodRecords` | `ScenarioOneManager` felling path | Created **only** for felled trees retained as deadwood | D | Biological death creates **no** deadwood record or log |
| Suppression history | `equivalentSuppressedYears` += annual suppression | `ForestTree.RecordSuppressionYear`, saved since v6 | Diagnostic only, documented "never fed back into growth in v1" | D | Cumulative; never decreases after release |
| Starting stand | 336 stems on a 21×21 lattice (1.9 m) in 40 × 40 m, all age 20; DBH by neighbour-class bands; `H = 2 + 0.62 · DBH ± 0.6` (minimum 3.5) | `ForestStartingStand` | Authored [D] anchors ("~12 m at 16 cm DBH") | D | See starting-stand table |
| Save fields (trees) | DBH, height, crown, age, suppression history, mortality cause/year | `TreeSaveData` v16 | — | — | Enough for a stateless mortality hazard (see SaveVersioningAssessment) |
| Reference Future v1 | Frozen archive; model-0 regeneration; continuation replays current growth from Year 50 | `ScenarioReferenceArchive` | D-042 | — | A growth change would change the continuation hash (diagnostic, not archive integrity) |

## Starting stand (Phase 2; production generator, measured)

| Measure | Value |
|---|---|
| Stand age | 20 (all trees) |
| Trees / stocking | 336 / 2,100 stems/ha |
| Top height (mean of the 100 largest-DBH stems/ha = 16 trees) | 14.63 m |
| Dominant height (mean of the 100 tallest/ha) | 14.75 m |
| Mean height (p10 / p50 / p90) | 11.65 m (9.50 / 11.64 / 13.60) |
| Mean DBH / quadratic mean DBH | 15.59 / 15.79 cm |
| DBH p10 / p50 / p90 | 12.02 / 15.49 / 19.02 cm |
| Basal area | 41.1 m²/ha |
| Reineke SDI (Dq 25 cm reference) | 1,004 |
| Stem volume (model formula) | 249 m³/ha |
| H/D mean / p90 / top-height trees | 75.1 / 79.7 / 71.9 |
| Hegyi CI mean / p90 | 8.24 / 11.47 |
| Site productivity | 1.0 (all cells) |

## Structural findings

1. **The starting stand and the growth law imply different sites.** The authored top height at age 20 (14.6 m) matches Irish Class III (14.2 m on the anchor-matched curve). The production growth law then reaches 17.1 m at age 30, i.e. Class IV–V (17.4 / 16.0 m). See IrishHeightValidation.md.
2. **Competition is scale-invariant.** Hegyi uses DBH ratios and fixed distances, so uniform growth leaves CI unchanged (mean 8.2 → about 9 over 100 years). Neither DBH growth nor any suppression-based mortality responds to a filling stand. There is no carrying capacity anywhere; basal area reaches 416 m²/ha at age 120 unthinned.
3. **Height is already independent of competition and thinning.** Top height at age 120 is 28.99 m unthinned against 28.91–29.11 m in the thinned regimes. This complies with the research direction.
4. **No adult mortality and no deadwood path for biological death** exist in production.
