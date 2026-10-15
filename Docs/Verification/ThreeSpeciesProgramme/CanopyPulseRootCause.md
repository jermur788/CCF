# Density v3 canopy pulses: root cause and fix

Task branch `fix/density-v3-canopy-pulses-v1`. Base `bc81627e8ca47fb619f6fbabcbf618dba0508e11` on `integration/scenario-one-final-candidate-v1`. Context commit `c6dc136b7f1f8668d9f67f95f48968d290ad5067`. Verification tier 1 (offline, reduced model). **Not Unity-verified.**

## Question

Why does the generic broadleaf density mortality sometimes remove 25-37 % of a stand's basal area (BA) in one year (observed in the integrated Tier-2 run: dense Beech Legacy40 37.3 %, dense Beech Enlarged80 25.7 %), and what is the smallest principled fix that stops it without changing the accepted density architecture?

## What could and could not be reproduced

The two Tier-2 numbers come from a Unity run whose logs are not in this repository, and this session has no Unity. They therefore could **not** be decomposed year by year. What was done instead:

* the production planner (`MixedDensityModel.PlanComponents`, unchanged) was run through the existing reduced stand simulator (`DensityTrajectorySim.cs`) on the same fixtures the Unity qualification uses (regular grid, same DBH pattern, same ids), with the new decomposition tool `CanopyPulseDiagnostic.cs`;
* the reduced simulator feeds new adult trees from a smooth external series (25-year block means). That series is exactly why it never produced these pulses (see below), so recruitment timing was varied while keeping the same mean inflow.

Everything below is a **reduced-model result about a mechanism**. It shows that the mechanism exists, how large it can get and what bounds it. It does not say what the Unity run did in any particular year.

## Root cause (measured)

`PlanComponents` does three things in sequence:

1. total occupancy goes through the species-independent response `L(o) = Lmax (1 - exp(-k (o-1)^m))`, times the non-Reference share, giving a **fraction of stems**;
2. that fraction times the stems still standing is rounded to a **whole-stem quota** (floor plus one keyed Bernoulli);
3. the quota is filled by a weighted draw without replacement, weight = suppression^4.

The quota is a *count*, set by occupancy, which is dominated by the canopy. The pool that can cheaply pay it is the set of small suppressed stems (recruits). Those carry essentially all of the allocation weight (95-100 % in every year examined) but almost none of the basal area (a 5 cm recruit is 0.2 % of a 60 cm canopy stem). Nothing links the number of stems to the basal area they hold.

* When the supply of small stems at least equals the quota, the quota is paid by recruits and BA loss is about 1 % (the accepted behaviour).
* When fewer small stems exist than the quota (recruits arrive in a cohort and are then eaten, a gap in recruitment, a year with no seed), the **shortfall falls on canopy stems**. Every canopy stem has a tiny and almost equal weight, so the shortfall is spread nearly at random over the canopy. Each canopy victim then costs 1/N_canopy of the stand's BA: 4-8 % per stem in a 20-30 stem Legacy40 stand, 1-1.5 % per stem in Enlarged80, and a shortfall of 4-8 stems is 17-37 % of BA.

Measured in the reduced model, same mean recruitment, only its timing changed (tool: `run_canopy_pulse_diagnostic.py scan`; 8 seeds per cell; worst single-year BA loss over years 26-300; "stress" = the four regimes pulse:3, pulse:5, pulse:10, gap:100-120, range of the four cell medians):

| stand, geometry | even flow ("smooth"): median, max | stress regimes: range of medians, max of all 32 runs |
|---|---|---|
| dense Beech L40 | 0.085, 0.094 | 0.175-0.292, 0.353 |
| dense Beech E80 | 0.047, 0.061 | 0.149-0.252, 0.288 |
| young Beech L40 | 0.077, 0.091 | 0.171-0.249, 0.419 |

Classification of the five candidate causes in the task:

| cause | verdict | evidence |
|---|---|---|
| A response magnitude | **amplifier, not the root** | the quota is 20-40 % of stems in dense Beech; a quota above the small-stem pool is what matters, and any non-zero quota with an empty small pool kills a canopy stem. Weaker settings only move the threshold (sensitivity run at the end of this file). |
| B victim-selection weighting | **not the cause** | canopy victims are on average *smaller* than a random canopy stem (0.61-0.95 of the mean canopy stem BA). Selection already prefers the more suppressed. It simply has no information left to use once the small pool is empty. |
| C no BA-sensitive guard | **primary** | stem quota and BA are decoupled; the only bound on BA loss is the stem bound. |
| D recruit synchronisation | **trigger** | smooth recruitment: <= 9 % worst year; the same inflow as cohorts: up to 42 %. The accepted reduced-model checks used the smooth series and could not see this. |
| E interaction | yes: **C primary, D trigger, A amplifier** | |

## Predeclared acceptance (written before the post-fix runs; mine, not Manager-accepted)

Runs: pure fixtures A_OAK_DENSE, B_BEECH_DENSE, E_OAK_YOUNG, F_BEECH_YOUNG x {L40, E80} x recruitment {smooth, pulse:3, pulse:5, pulse:10, gap:100-120} x 8 seeds (default plus 7001-7007), 300 years; mixed C_MIXED_DENSE, G_MIXED_YOUNG x {L40, E80} x {smooth, pulse:5, gap:100-120} x 8 seeds; the existing density (91) and composite (61) suites and the 11 mutation tests.

* **G1 pulse bound (the defect).** For every generic step, BA lost by the generic component <= max(cap, BA of the single stem it removed first): proved by a property test over random states, and in the 320 pure runs no year >= 26 loses more than 0.15 of BA unless the stand had 10 stems or fewer.
* **G2 smooth behaviour preserved.** In the even-flow runs: R1, R3 and R4 verdicts unchanged from the base on every cell (pass stays pass); R2 not worse; Y300 BA within +-15 % and occupancy within +-0.15 of the base.
* **G3 flattening retained.** R1 (|occupancy slope| <= 0.0025 for Y201-300, <= 0.004 for Y151-250) passes in every stress cell where it passes at the base, and the pass count over stress cells is not lower.
* **G4 no runaway, no collapse.** R4 (25-year BA ratio >= 0.5; Y300/Y100 >= 0.5) in all runs; maximum occupancy at years >= 26 not above the base maximum + 0.15 in even-flow runs; living stems at Y300 not below half the base value.
* **G5 mixed intact.** All existing identity (I1-I7), accounting, shielding (C3, C4), continuity (C1, C2, C5) and mixed long-run (R2'-R5') checks pass unmodified; Sitka victim sets identical with 0/1/3/12/26 broadleaves; mixed stands meet the same pulse bound for the generic component.
* **G6 pure Sitka and everything else.** `SitkaGrowthModel.cs`, `SitkaSubsetDensitySignal.cs`, `ForestEcologyController.cs`, `ForestSaveData.cs`, the species and objective assets are byte-identical to the base; density suite 91/91 including the three legacy-route fingerprints; both isolated compiles pass; 11/11 original mutants still killed.

The five known reduced-model R2 failures (max annual all-stem loss 0.407-0.423 against 0.40, one 0.106 block change) are a different quantity (stem loss, not BA loss). They are not in scope of this fix and are reported unchanged.
