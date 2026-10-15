# Density v3 canopy pulses: root cause and fix

Task branch `fix/density-v3-canopy-pulses-v1`. Base `bc81627e8ca47fb619f6fbabcbf618dba0508e11` (`integration/scenario-one-final-candidate-v1`). Context commit `c6dc136b7f1f8668d9f67f95f48968d290ad5067`. Verification tier 1: offline and reduced-model only. **Not Unity-verified.** The implementation HEAD is read back with `git rev-parse HEAD` on the pushed branch.

## Question

Why does the generic broadleaf density mortality sometimes remove 25-37 % of a stand's basal area (BA) in one year (integrated Tier-2: dense Beech Legacy40 37.3 %, dense Beech Enlarged80 25.7 %; pure Sitka and mixed accounting/shielding pass), and what is the smallest principled fix that does not change the accepted density architecture?

## What could and could not be reproduced

The two Tier-2 numbers come from a Unity run whose logs are not in this repository, and this session has no Unity. They were **not** decomposed year by year. Instead:

* the production planner (`MixedDensityModel.PlanComponents`) was run unchanged through the existing reduced stand simulator on the fixtures the Unity qualification uses, with a new per-year decomposition tool (`CanopyPulseDiagnostic.cs`, `run_canopy_pulse_diagnostic.py`);
* the simulator takes new adult trees from a smooth external series (25-year block means measured on an earlier runtime lineage). That is why every earlier offline check was blind to these pulses, so recruitment **timing** was varied while keeping the same mean inflow (`ConfigFor` recruitment modes).

Everything below is a **reduced-model result about a mechanism**: that it exists, how large it gets and what bounds it. It does not say what the Unity run did in any particular year. Whether the Unity pulses come from exactly this recruit shortage is what the focused Unity gate at the end confirms.

## Root cause (measured)

`PlanComponents` does three things in sequence:

1. total occupancy goes through the species-independent response `L(o) = Lmax (1 - exp(-k (o-1)^m))`, times the non-Reference share, giving a **fraction of stems**;
2. that fraction times the stems still standing becomes a **whole-stem quota** (floor plus one keyed Bernoulli);
3. the quota is filled by a weighted draw without replacement, weight = suppression^4.

The quota is a count set by occupancy, which is dominated by the canopy. The pool that can cheaply pay it is the set of small suppressed stems (recruits). Those carry nearly all of the allocation weight (on average 0.85-1.00 of it in the years that had canopy victims; individual years range from 0.62 to 1.00, and 0 when no small stem existed at all) and almost none of the basal area (a 5.25 cm recruit is 0.8 % of a 60 cm canopy stem). **Nothing links the number of stems to the basal area they hold.**

* When the small-stem supply covers the quota, recruits pay it and BA loss is about 1 % a year (the accepted behaviour).
* When fewer small stems exist than the quota (recruits arrive as a cohort and are eaten, a year with no recruits), the **shortfall falls on canopy stems**. Canopy stems all have a tiny, nearly equal weight, so the shortfall is spread almost at random over the canopy. Each canopy victim costs about 1/N_canopy of the stand's BA: 4-7 % in Legacy40's 15-25 canopy stems, 1-1.5 % in Enlarged80's 70-100. A shortfall of 4-8 stems in Legacy40, or 17-25 in Enlarged80, is 17-37 % of BA.

Worst single-year BA loss, years 26-300, 16 seeds per cell. "Stress" is the four regimes pulse:3, pulse:5, pulse:10 and gap:100-120 (same mean inflow, cohort timing or a 20-year gap), 64 runs per row. Base is the unbudgeted planner, After is this fix.

| stand | base smooth median / max | base stress median / max | stress runs >= 0.25 | after smooth median / max | after stress median / max | stress runs >= 0.15 |
|---|---|---|---|---|---|---|
| L40 dense Beech | 0.084 / 0.103 | 0.229 / 0.353 | 24 | 0.089 / 0.121 | 0.123 / 0.125 | 0 |
| L40 young Beech | 0.079 / 0.149 | 0.209 / 0.419 | 15 | 0.079 / 0.115 | 0.123 / 0.125 | 0 |
| L40 dense Oak | 0.202 / 0.306 | 0.227 / 0.329 | 19 | 0.124 / 0.125 | 0.124 / 0.145 | 0 |
| L40 young Oak | 0.176 / 0.234 | 0.202 / 0.304 | 11 | 0.123 / 0.125 | 0.123 / 0.125 | 0 |
| E80 dense Beech | 0.043 / 0.061 | 0.225 / 0.304 | 17 | 0.041 / 0.061 | 0.125 / 0.125 | 0 |
| E80 young Beech | 0.045 / 0.054 | 0.188 / 0.293 | 6 | 0.045 / 0.054 | 0.124 / 0.125 | 0 |
| E80 dense Oak | 0.085 / 0.123 | 0.197 / 0.260 | 2 | 0.085 / 0.123 | 0.125 / 0.125 | 0 |
| E80 young Oak | 0.073 / 0.130 | 0.175 / 0.229 | 0 | 0.073 / 0.119 | 0.125 / 0.125 | 0 |

Even with an even recruit flow, Legacy40 dense and young Oak (about 20 stems) had a median worst year of 0.18-0.20 in the base. That is the same defect with no stress: a canopy-opening event in a stand where one stem is 5-7 % of the BA. The single 0.145 after the fix (L40 dense Oak) is a year with fewer than nine stems, where the never-stall rule applies (below).

In shortfall years the canopy victims are on average *smaller* than a random canopy stem (0.61-0.95 of the mean canopy stem BA), so selection is already biased the right way.

Classification of the candidate causes in the task:

| cause | verdict | evidence |
|---|---|---|
| A response magnitude | **amplifier, not separable from flattening** | Weakening the response removes the pulses only by also losing the density regulation: table "Weaker response instead of a budget" below. |
| B victim-selection weighting | **not the cause** | canopy victims are smaller than average canopy stems; once the small pool is empty there is no information left to choose by. Exponent changes were already rejected in the v3 handoff as a structural change. |
| C no BA-sensitive guard | **primary** | stem quota and BA are decoupled; the only bound on BA loss is the stem bound. |
| D recruit synchronisation | **trigger** | even flow: worst year 4-10 % (dense Beech, medians 0.043 and 0.084); same inflow as cohorts or with a gap: regime medians 0.15-0.29, up to 0.35. |
| E interaction | yes: **C primary, D trigger, A amplifier** | |

### Weaker response instead of a budget (budget off; dense and young Beech, pulse:5 and gap:100-120, 8 seeds, both geometries, 64 runs)

| response (Lmax, k, m) | worst-year BA loss median / max | runs >= 0.25 | runs >= 0.15 | R1 flattening pass | Y300 occupancy median |
|---|---|---|---|---|---|
| 0.50, 1.60, 2 (production) | 0.216 / 0.419 | 16 | 59 | 51 / 64 | 1.71 |
| 0.35, 1.60, 2 | 0.179 / 0.286 | 2 | 63 | 32 / 64 | 2.15 |
| 0.50, 0.80, 2 | 0.188 / 0.290 | 3 | 55 | 35 / 64 | 2.04 |
| 0.25, 0.20, 2 (the v2 setting) | 0.091 / 0.158 | 0 | 5 | 1 / 64 | 3.96 |
| 0.15, 0.20, 2 | 0.068 / 0.125 | 0 | 0 | 0 / 64 | 5.60 |

The weak settings stop the pulses only because they stop thinning altogether (occupancy 4-5.6, the v2 Beech runaway). This is the same trade-off the v3 calibration search met, and it is why the response constants were **not** changed.

## The fix

One production file changes: `Assets/ForestPrototype/MixedDensityModel.Composite.cs` (+77 / -6). Nothing else under `Assets/`, `Packages/` or `ProjectSettings/`. `SitkaGrowthModel.cs`, `SitkaSubsetDensitySignal.cs`, `MixedDensityModel.cs`, `ForestEcologyController.cs`, `ForestSaveData.cs`, `AdultMortalityAccount.cs`, the species and objective assets are byte-identical to the base.

**A basal-area budget on the generic step.** After the whole-stem quota is drawn (unchanged), victims are taken in the existing priority order (unchanged), except that a stem which would take the generic step above `GenericAnnualBasalAreaBudgetShare` (0.125) of the stand's basal area is passed over and the walk continues, so smaller stems that fit can still be taken. If *no* eligible stem fits (every stem alone is more than 12.5 % of the BA, a stand of fewer than eight stems) the highest-priority stem is taken, so the generic component never stalls on a sparse stand of a few large trees.

* Budget off (share 1.0) reproduces the base bit-for-bit (640 pure runs compared byte-for-byte).
* While the budget is not reached it changes nothing: victims are exactly the first `quota` stems in priority order (property B2 below).
* It is memoryless: occupancy is recomputed each year, so a stand that is still crowded keeps asking and is thinned over several years instead of in one. No state, nothing saved, save v20 unchanged.
* It concerns the generic component only. The Sitka component, route selection, the quota draw, the response and the allocation weights are untouched. Pure Sitka never reaches this code.

Plan diagnostics added (never saved): `GenericQuota`, `GenericBudgetLimited`, `GenericBudgetPassedOver`, `GenericBasalAreaShare`. `GenericResponseCalibration` gains an optional fourth number (the budget); the three-argument constructor takes the production default.

**Why this is appropriate.** The generic response is defined on stems because occupancy is a stem-level abstraction, but the thing it regulates is *occupied growing space*, which the canopy holds. A rule that can remove a quarter of the canopy in one year because too few small stems happened to be available is a modelling artefact of counting, not a density process, and it has no basis in any evidence (no Oak or Beech density evidence is claimed anywhere in this model). The budget restores the intended ordering: small suppressed stems first, canopy only at a bounded rate. It adds no species, class or site value and is not a maximum density, SDI or capacity: stand occupancy and basal area remain unbounded and the stand is still relieved (property B3 thins an occupancy-5.3 stand to 0.95 in 86 years at <= 12.4 % of BA a year). The source naming guard that forbids "max ... basal" identifiers still passes because the constant is named for what it is, a budget.

### New [C] constants

| constant | value | label |
|---|---|---|
| `GenericAnnualBasalAreaBudgetShare` | 0.125 | **[C] simulation calibration.** Not a measurement, not species or class specific, not a maximum density. |

Chosen from the sweep below as the smallest tested value whose effect on the accepted even-recruitment dynamics is within seed noise on every cell. Read as stems: a stand of eight or more similar canopy stems can always lose one in a year, sixteen or more can lose two. The earlier idea of setting it equal to the existing `MaximumAnnualLoss` (0.10) was tested and **rejected** on evidence: with the never-stall rule it slows the flattening of Legacy40 Oak stands.

### Budget sweep (16 seeds x 5 recruitment regimes x 8 pure stands = 640 runs per row, never-stall rule)

| budget | stress worst year (max) | stress runs > 0.15 | R1 flattening pass (of 512 stress runs) | R2 pass (of 512) | runs whose max occupancy exceeds the base by > 0.15 | smooth cells shifted by > 1 base seed-SD |
|---|---|---|---|---|---|---|
| off (base) | 0.419 | 453 | 488 | 167 | 0 | none |
| 0.10 | 0.145 | 0 | 474 | 386 | 12 | L40 Oak: BA +4.4 and +6.0 SD, occupancy +4.3 and +6.1 SD |
| 0.11 | 0.145 | 0 | 490 | 360 | 0 | L40 Oak: +2.4 to +2.5 SD |
| 0.115 | 0.167 (6 stems) | 1 | 492 | 343 | 0 | stems -1.0 to -1.2 SD |
| 0.12 | 0.144 | 0 | 492 | 341 | 0 | L40: -1.1, +1.1 SD |
| **0.125** | **0.145** | **0** | **494** | **323** | **0** | **none** |
| 0.13 | 0.130 | 0 | 493 | 330 | 0 | none |
| 0.14 | 0.140 | 0 | 490 | 274 | 0 | none |
| 0.15 | 0.150 | 0 | 494 | 257 | 0 | none |
| 0.175 | 0.175 | 435 | 490 | 201 | 0 | none |

The clean window is 0.125-0.15. At 0.125 the budget changes the victims in about 3 of 275 years in an even-flow Legacy40 Oak run (the canopy-event years that are the defect) and in none for Beech or Enlarged80. If the focused Unity gate shows flattening tighter than the model, 0.14-0.15 is the supported fallback.

## Acceptance

The criteria were committed in the first version of this file (commit `51468f2`) before any post-fix run. Results on the final code, 640 pure runs and 192 mixed runs:

| id | criterion | result |
|---|---|---|
| G1 | exact bound: every generic step <= max(budget, one stem when none fits); no year >= 26 above 0.15 unless the stand has <= 10 stems | **PASS.** Zero violations of the exact rule in all 192,000 annual steps of the 640 pure runs (stands of nine or more stems never above the budget; smaller ones at most one stem); property test B1 over 795 random states (17 exercising the never-stall rule). Four of the 640 runs have a worst year above 0.125 (up to 0.145), each with seven stems. *The first implementation (first stem always allowed) missed the empirical clause: 8-9 stress runs reached 0.151-0.171 with 11-15 stems. I tightened the rule to the never-stall form before the final runs.* |
| G2 | smooth runs: R1, R3, R4 verdicts unchanged; R2 not worse; Y300 BA within +-15 % and occupancy within +-0.15 of the base **per run** | verdicts: **PASS** (0 flips in 128 smooth runs; R2 not worse). Per-run tolerance: **not met in 4 of 128 smooth runs** (BA +-15 %: 2; occupancy: 4; stems < half: 1), all in Legacy40 stands of about 20 stems (dense Oak x3, dense Beech x1). **Amended after seeing this**, and disclosed as such: per-run comparison of chaotic 20-stem trajectories is not meaningful, because the base's own seed-to-seed range is as wide (dense Oak BA300 29.7-40.6, sd 2.9) and the guarded runs are tighter (36.0-40.1, sd 1.1). Amended G2': the cell median shifts by at most one base seed-SD on Y300 BA, occupancy and stems: **PASS**, no shifted cell. |
| G3 | flattening retained: R1 passes in every stress cell where it passed at the base, pass count not lower | **PASS.** No cell passes fewer seeds; total 494 vs 488 of 512. |
| G4 | no runaway or collapse: R4 in all runs; max occupancy <= base + 0.15; living stems at Y300 >= half of base | **PASS.** R3 and R4 fail in 0 of 640 runs; no run's max occupancy exceeds the base by 0.15; smallest cell-median stem ratio 0.82. |
| G5 | mixed intact: identity, accounting, shielding, continuity and mixed long-run checks pass unmodified; Sitka victim sets identical | **PASS.** Composite suite 107 checks: all pre-existing identity/shielding/continuity/mixed checks pass; Sitka victim sets identical with the budget on and off in 795 of 795 states. Mixed matrix below. |
| G6 | pure Sitka and everything else unchanged | **PASS.** Density suite 91/91 with the three legacy-route fingerprints; both isolated compiles; only one production file differs from the base; 18/18 mutants killed. |

### Mixed stands (16 seeds per cell; budget off -> 0.125)

From year 26 in the accepted even-flow regime the budget acts in 0 years in the median mixed run (5 of 64 runs have any such year, at most one) and R2'/R3/R4 pass 16/16 both ways; the dense-start transient in years 1-25 is budgeted as in the pure stands. Under cohort stress it acts in a median of 1 year (at most 10) and R2' (generic component only) goes from 5 to 14 of 16 for Legacy40 dense mixed, unchanged elsewhere. Y300 stems, BA and occupancy medians move by at most 1-12 % in the stress cells (Legacy40 dense mixed pulse:5: BA 72.3 -> 81.3, occupancy 1.78 -> 1.85). Occupancy slopes of the mixed gap runs (+0.0021 -> +0.0027 and +0.0041 -> +0.0037) are above the pure-stand limit as they were before; R1 is not required for mixed stands.

### Suites on the final tree

| suite | before (base) | after |
|---|---|---|
| core density (legacy fingerprints, routing, naming guard) | 91 PASS | **91 PASS** |
| composite density | 61 checks, **5 FAIL** | 107 checks, **3 FAIL** (new checks B0-B6 and 36 long-run bound checks all pass) |
| isolated compiles (occupancy model without Sitka; Sitka model without occupancy) | pass, pass | pass, pass |
| timber / history | 71 / 74 pass | 71 / 74 pass |
| mutation tests | 11 / 11 killed | **18 / 18 killed** (7 new budget mutants: removed, never-stall removed, first-stem-always-allowed, stops instead of skipping, class-weighted, constant loosened to 0.25, wrong total) |
| production / tools type-check | clean vs baseline | clean vs baseline (prod 7 of 7, tools 13 against a baseline of 43); no new error |

The composite suite's remaining three failures are the stem-loss R2 items already open in the integration handoff (single-year **all-stem** loss: Legacy40 dense Beech 0.423, Legacy40 young Beech 0.407, Enlarged80 young Beech 0.417, against 0.40). They count recruits, not canopy, are not changed by this fix and are **not waived**. Of the five listed there, Legacy40 young Oak (block change 0.106 -> 0.082) and Enlarged80 dense Beech (0.421 -> 0.400, at the limit) now pass; the latter is marginal and should not be read as a robust improvement.

## Behaviour changes to be aware of

* **The artificial dense start is spread over years.** The dense fixtures (1,300 stems/ha, occupancy 2.4) used to lose about 46-47 % of BA in year 1; the budget caps year 1 at 12.5 % and the stand reaches the same basal area by year 26 (L40 dense Beech 46.5 -> 46.8, E80 dense Oak 34.1 -> 34.2 m2/ha). This resolves the "start-up shock" listed as risk 5 in the v3 handoff.
* **R4 (25-year BA ratio) minimum is lower in dense-start fixtures** (0.80-0.97 -> 0.54-0.60) because the transient is now inside the first 26 years. From year 50 it is 0.87-0.98 (base 0.80-0.99). R4's limit 0.5 still holds in all 640 runs.
* **Growth model 2 trajectories in which the budget acts will differ** from the base (dense or mixed stands with a recruit shortage). Growth model 2 is not the default (`ScenarioOneManager.NewGameGrowthModel` is 1) and no accepted model-2 hash exists in this branch, so no Manager-accepted anchor moves; pure Sitka and model 1 anchors cannot move because the changed code is never reached.

## Limits of this evidence

* The Tier-2 pulses were not reproduced from the Unity run; the mechanism and magnitude were reproduced in the reduced model with synchronised or interrupted recruitment. If Unity's pulses have a different cause, the focused gate below will show it.
* Reduced simulator: smooth external recruitment from an earlier lineage, site 1, no canopy light, seed rain or browsing. A screen, not evidence about the game. The budget value rests on it.
* Seeds: 16 per cell, 8 for the response-sensitivity table.
* The edited Unity harness (`DensityQualification.cs`) was type-checked only (no new error against the approximate baseline), never run.

## Focused Unity gate required next (Tier 1, not a campaign)

1. Import and compile Unity 6000.6.0f1: no `error CS`.
2. `DensityQualification` hard phases on both geometries (`CCF_STAND_GEOMETRY=0` and `1`): `CCF_QUAL_PHASES=SITKA100,SITKACOMP,CURVE,FAIRNESS,FAIRNESSHIGH,CONTINUITY,LATEMIX,FINALBROADLEAF,SAVELOAD`. Expect every HARD check PASS; the pure-Sitka exactness (`PURE_SITKA_*_100Y_MODEL2_EXACTLY_EQUALS_MODEL1`, `SITKACOMP_*`) must be unchanged. `FAIRNESS*_OVERALL_DEATHS_MATCH_THE_BERNOULLI_EXPECTATION` was re-based in this change to the trials in which the budget did not act (the budget lowers the count by design); it prints `budgetBoundTrials`. If every trial is budget-bound it reports pooledZ 0, and the controller-equals-plan and species-neutrality checks still apply.
3. The failing fixtures themselves, default seed, both geometries: `CCF_QUAL_PHASES=LONGRUN CCF_QUAL_SCENARIOS=B_BEECH_DENSE,F_BEECH_YOUNG CCF_QUAL_YEARS=300 CCF_QUAL_REPLAY=0`, then `python3 Tools/Verification/ThreeSpecies/Qualification/max_ba_loss.py <evidence>/longrun-L40.csv <evidence>/longrun-E80.csv`. Expect `MAX_BA_LOSS_PASS` (no year above 0.125 of BA for stands of nine stems or more), the former 37.3 % / 25.7 % years gone, Y300 occupancy and BA near the base's (dense Beech Enlarged80 Y300 about 66 m2/ha, occupancy about 1.8), and R1 flattening as before. If a pulse remains, read `genericDeaths`, `bigDeaths` and `livingBefore` of that year: it would mean Unity's pulse is not a generic-component recruit shortage.
4. Not needed for this change: the 24-gate regression, anchors for pure Sitka (unchanged code path), seed campaigns, Player or Windows builds.

## Reproduce

```text
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density          # 91 + 107 composite + both isolation compiles
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_composite_mutations.py                   # 18 mutants (control may carry the 3 known failures)
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_canopy_pulse_diagnostic.py one B_BEECH_DENSE L40 pulse:5
python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_canopy_pulse_diagnostic.py scan E80 B_BEECH_DENSE,F_BEECH_YOUNG smooth,pulse:5,gap:100-120 16
CCF_CAL=0.5,1.6,2,0.15 python3 .../run_canopy_pulse_diagnostic.py scan ...                      # another budget; 1.0 switches it off
python3 Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod|tools
```

Raw scan tables are in `CanopyPulseEvidence/`.
