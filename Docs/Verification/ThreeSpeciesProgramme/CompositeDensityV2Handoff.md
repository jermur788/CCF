# N1 mixed-density architecture rework v2 handoff

Source: `a395e3f4fd8017281281a620cd0fb6867ebe0831` (`claude/magical-volta-ssw54w`). Branch: `task/mixed-density-architecture-rework-v2`. Worktree: `/media/jer/Files/CCF-density-rework-v2`. Canonical context: `321791b8203bc709c57615fd4d6a19614d78db24`. **NON-UNITY VERIFICATION ONLY.** No Library/import/Editor/Player run, no main integration.

## Architecture

Pure Sitka and models 0/1 retain the original controller legacy route and unchanged `SitkaGrowthModel` numerics. Existing fingerprint guards verify the legacy class, decision block and StandRelativeDensity method unchanged. The mixed model-2 route now takes two independent signals:

1. `SitkaSubsetDensitySignal.RequiredDeaths`: filters Reference stems only; invokes existing Sitka RD/pressure/hazard functions and original suppression/boundary policy on that subset. No Oak/Beech stem count/DBH enters Sitka RD. Its required-death count is a signal, not a rule that only Sitka victims die in mixed stands.
2. Total generic occupancy: geometric cross-section and ordinal class weights 1/1.5/2, normalization 55 [C]. A smooth squared-excess response `0.25*(1-exp(-0.20*max(0,occupancy-1)^2))` [C] is multiplied by the non-Reference occupied-space share. This contribution vanishes in the pure-Sitka limit and gives pure broadleaves their own generic pressure.

One combined annual fraction is `1-(1-referenceFraction)*(1-genericContribution)`. A keyed fractional quota resolves whole-stem granularity, retaining at least the Sitka-subset required-death count. One weighted-race allocator chooses all mixed victims using species-neutral Hegyi suppression with the common half-saturation 5 and exponent 4 [C]. No species Ci50 enters that allocation. No Oak/Beech SDImax, max BA, universal broadleaf RD ceiling, DEC/DMC or mixture bonus.

The .25 upper bound applies to the **generic contribution**, not total composite mortality. Severe Sitka overstocking can demand larger losses because the exact legacy boundary does so too; calling that a new broadleaf annual ceiling would be incorrect. Ordinal occupancy is not a forestry maximum or a fixed cap. The reference required-death signal is discretized, so "continuous" means continuity within the declared stem-granularity tolerances, not an analytic mortality-count function.

## Predeclared continuity

See `CompositeDensityV2Predeclared.md`, written before new test output. 1,000 stems; 100/99.9/99/95/90% Sitka; DBH15/25/40/60 and CI1/3/8 (60 states). All predeclared checks pass: 99.9% versus pure expected-loss difference <=.005; adjacent expected-loss difference <=2*composition change+.005; final single-broadleaf removal/death-state comparison <=.005 and <=5 deaths per 1,000. Worst 99.9% versus pure recorded expected-loss difference is about .000902 (0.0902 percentage points), below .5 percentage points. Pure computed reference quota matches the original helper loss count and fraction.

Deliberate removal and natural death are controlled **offline surviving-state** probes; they are not executions of Unity's felling/death callbacks. That runtime wiring remains U3. The mixed allocator's victim IDs need not match the pure-Sitka legacy allocator even where total loss converges. Pure route victim IDs remain unchanged because that route never calls the new planner.

## Reduced long-run results

Six trajectories: pure Oak, pure Beech and mixed, each without recruitment and with three new 2-cm stems/year. The fixture reproduces the existing DBH size-brake/Hegyi formula using current asset potentials (.4 broadleaf, 1.2 Sitka), Dmax120, CI fixed3, site1. It is **not** the full ecology: no live canopy, Hegyi feedback, juvenile survival, promotion, seed rain or real regeneration loop. Stand time checkpoints are simulation years from the controlled starting population, not new age-growth evidence.

| Fixture | Y25 occupancy | Y50 | Y100 | Y200 | Y300 | Last50 max/min | Last50 slope/year |
|---|---:|---:|---:|---:|---:|---:|---:|
| Oak, no recruits | 1.468 | 1.520 | 1.449 | 1.343 | 1.261 | 1.0595 | -.001184 |
| Oak, recruits | 1.466 | 1.533 | 1.472 | 1.418 | 1.415 | 1.0593 | +.001352 |
| Beech, no recruits | 1.229 | 1.470 | 1.443 | 1.329 | 1.275 | 1.0413 | -.000734 |
| Beech, recruits | 1.233 | 1.483 | 1.463 | 1.411 | 1.402 | 1.0449 | +.000888 |
| Mixed, no recruits | 1.548 | 1.800 | 1.706 | 1.501 | 1.379 | 1.0670 | -.001446 |
| Mixed, recruits | 1.556 | 1.836 | 1.771 | 1.637 | 1.660 | 1.0546 | +.000996 |

All satisfy predeclared last50 ratio<=1.20 and absolute slope<.005. Recruitment trajectories still fluctuate and may have positive local slope; do not call them proven ecological equilibria. There is no imposed occupancy cap. Y300 BA is 34.7/38.9 Oak, 46.7/51.4 Beech, 60.8/73.8 mixed m²/ha (no recruits/recruits). These are reduced-model outputs, not empirical maxima or predicted Irish stand values.

## Verification and test fixes

- Existing density suite: **91/91 PASS**; isolated generic+composite compilation without Sitka; isolated Sitka compilation without mixed code.
- New composite suite: **1,917/1,917 PASS** (includes repeated finite-state checks; not 1,917 independent ecological scenarios).
- Mutation tests: **5/5 killed**: discarded reference pressure, untapered generic contribution, no generic mortality, species-biased allocation, broadleaf leakage into Sitka RD. Mutation work exposed a missing pressure assertion and two non-discriminating fixtures; assertions/fixtures were strengthened without retuning production, and all five rerun. Original positive control rerun green afterward.
- Timber **71/71 PASS**; history **70/70 PASS**.
- Production type-check: **no new errors**, 7 known reference-assembly baseline errors; tools **no new errors**, 43 baseline errors. This is approximate type-checking, not Unity compilation. Baselines not updated.
- Reviewed test-only patches `7b697d29805ea261bd1a8c3b2e97b61f0e8b996f` and `32ae1974969929c26b6ff3a2e8e796fe7e9a9ff3` ported from accessible local commits. No assertion removed. The v2 generic mortality bound additionally replaces two v1 20% calibration assumptions with the declared .25 bound plus integer rounding; no claim these Unity assertions were executed here.
- Initial density run's sole failure was a source guard requiring the old `.Plan` name. Updated to require `.PlanComposite` and the explicit subset adapter while retaining every no-direct-Sitka-arithmetic guard. Initial failure evidence retained.
- `git diff --check` required before commit. No package/settings/scene/prefab/species-asset/save/default/growth/timber/history production change. New source `.meta` GUIDs only; no Unity reserialization.

## Runtime risks / U3

Ready for Unity U3 review/qualification, **not Unity-qualified or integration-approved**. U3 must run real pure-Sitka anchors; reproduce near-pure transition states with tree-ID outcomes; repeat pure Oak/Beech/mixed Y25/50/100/200/300 with actual regeneration/canopy/competition; check final broadleaf manual and natural loss; inspect self-thinning ordering, finances, save/load and combined scene behaviour. The ordinal calibration could still over-thin or drift under feedback absent from the reduced model. No external-demo readiness claim.

Run `Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite all` and `run_composite_mutations.py`; approximate type-check commands unchanged. Evidence is under `CompositeDensityV2Evidence/`. Exact published HEAD is supplied by final Git read-back, not a self-referential SHA embedded in its own commit.

Committed text evidence trims trailing whitespace only; values/order are unchanged. Raw output digests and committed digests are in `CompositeDensityV2Evidence/manifest.json`; raw copies remain outside Git in the local combined-task evidence folder.
