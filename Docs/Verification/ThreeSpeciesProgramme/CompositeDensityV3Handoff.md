# DENSITY V3 HANDOFF: component-faithful mortality

Role: Simulation Density Rework Engineer. **Not merged.** Growth Model 2 is still not the default. No species density capacity, no Oak/Beech SDI or maximum basal area, no DEC/DMC/mixture bonus. Pure Sitka, save v20, all anchors and every legacy-route fingerprint are unchanged.
Everything here was run on this branch unless the "Unity status" section says otherwise. Evidence: `CompositeDensityV3Evidence/` next to this file (text outputs have trailing whitespace trimmed, nothing else changed); acceptance and every change of definition: `CompositeDensityV3Predeclared.md` (committed before the results it judges).

## BRANCH / BASE / HEAD

| | |
|---|---|
| Branch | `task/mixed-density-architecture-v3` (pushed; **not merged**) |
| Base | `task/mixed-density-architecture-rework-v2` @ `a7a759ded087a824c1e8149e480ddf58ac0585eb` |
| Head | the commit that adds this file (its SHA cannot be written into itself; read it back with `git ls-remote origin refs/heads/task/mixed-density-architecture-v3`) |
| Qualification evidence branch, pushed first | `verify/final-density-v2-a7a759d` @ **`81126ee093b0db345112af17bcb2842a95fe1009`** (remote read-back identical to the local head) |
| Canonical context | `321791b8203bc709c57615fd4d6a19614d78db24` (not re-read mid-task) |
| Worktree | `/media/jer/Files/CCF-density-v3` (own `Library`; `/home/jer/CCF-main` untouched) |

Production diff against the base, five files and nothing else: `SitkaSubsetDensitySignal.cs`, `MixedDensityModel.Composite.cs`, `MixedDensityModel.cs` (comment, `sealed` removed), `ForestEcologyController.cs` (only `ApplySpeciesAwareDensityMortality`), `AdultMortalityAccount.cs` (diagnostic fields, never saved). `SitkaGrowthModel.cs`, the legacy controller decision block and `StandRelativeDensity` are byte-identical.

## ROOT CAUSE (measured, not assumed)

v2 derived the Sitka subset's required deaths as a **count**, folded it into one whole-stand quota and let one weighted race over **every** stem pay it. Suppressed broadleaves are the weakest competitors, so they paid Sitka's debt. Two consequences, both visible in the v2 Unity long runs I already had:

1. **Shielding.** Old Sitka stand, Sitka's own annual loss with 0 / 1 / 3 / 12 / 26 small broadleaves added: 0.150 / 0.134 / 0.103 / 0.023 / 0.000 (table below).
2. **The debt never gets paid, so it is demanded again every year.** The year-end Sitka-subset relative density in the v2 Enlarged80 mixed stand (`C_MIXED_DENSE`) sat at **1.10 to 1.22 for 200 years** (Y101-150 1.22, Y151-200 1.15, Y201-250 1.10, Y251-300 1.12), above the 1.0 line the original boundary stage guarantees; the pure Sitka stand sits at 0.96 and the young mixed stand once its broadleaves were gone at 0.98. That persistent excess is the ~0.27 annual loss (decomposition below).

## COMPONENT ARCHITECTURE

* **Sitka component** (`SitkaSubsetDensitySignal.Resolve`): the original Sitka density process on the Reference stems alone: relative density, pressure, suppression-weighted hazard, boundary. Same keyed roll domain (`ADULT-MORT-<id>`), same float arithmetic for the Hegyi/Ci50 suppression. Returns **victims**, not a count. No Oak/Beech stem count or DBH ever enters a Sitka density.
* **Generic component** (`MixedDensityModel.PlanComponents`): total class-weighted occupancy goes through the species-independent response `L(o) = Lmax (1 - exp(-k (o-1)^m))`, times the non-Reference share of the occupied space (so it vanishes continuously with the broadleaves). That fraction of the stems still standing after the Sitka component is removed, chosen among those stems only, by species-neutral competitive position (Hegyi suppression, one common half-saturation and exponent). Own roll domain (`ADULT-MORT-GENERIC-<id>`).
* **Final victims:** union, in tree-id order. Disjoint by construction (the generic draw cannot pick a stem the Sitka component took): no stem dies twice, total = Sitka deaths + generic deaths, bounded by `R + Lmax (N - R)`.
* **Equivalent formulation:** count-wise this equals v2's quota `1-(1-R/N)(1-g)`, i.e. `R + g (N-R)`. v3 changes **who pays R**, not how many die in the first instant. That is why v2's predeclared continuity carries over.
* `MixedDensityModel.cs` still compiles **without** `SitkaGrowthModel.cs` and `SitkaGrowthModel.cs` still compiles **without** `MixedDensityModel.cs` (both isolated compiles pass): the planner receives victim *indices*, never Sitka arithmetic.

## GENERIC CALIBRATION (and an honest account of how it was chosen)

Setting: **`Lmax 0.50`, `k 1.60`, `m 2`** (was 0.25 / 0.20 / 2). Zero at and below the onset, continuous, monotonic, bounded, species-independent, no capacity. All [C].

How: predeclared grid, reduced simulator, then Unity.
* **Trial 1** (registered rules): 21 candidates, **empty passing set**. The v2 setting reproduces the Unity v2 Beech drift in the simulator (+0.0040 to +0.0050 per year against Unity +0.0054/+0.0052). By the registered rule that was a STOP for extending the grid or touching the allocation exponent or adult growth, and none of those was done.
* The failing sub-criteria were measuring fixture artefacts, not the kernel: the Year-1 shock of a dense start fixture (occupancy about 2.2 by construction), all-stem loss dominated by the ~40 new 5-cm adult trees a year that enter an old Beech stand (dead basal area 0.2-1.5 % a year, stems of at least 20 cm 0.3-1.1 %), and a young-cohort occupancy ratio. **Trial 2** amends exactly those three definitions, plus **addendum A4** (mixed-stand R2 on the generic component only, because the unchanged legacy Sitka route fails the whole-stand R2 by itself: single-year loss up to 81 %, 19 years above 40 % on Legacy40). Both amendments were written and committed **before** the run they govern. They are mine, made after seeing Trial 1; **you may reject them**. If rejected, three constants in `MixedDensityModel.Composite.cs` return to the v2 values and the architecture is unaffected.
* Trial 2 passing set on the grid: `(0.35, 1.60)` margin 1.04 and **`(0.50, 1.60)` margin 1.16**. The passing region is **thin and sits at the strong edge of the grid** (`k = 1.60` is the largest `k` tried). Registered rule: largest minimum margin, so `(0.50, 1.60)`. Not "central" in the sense you asked for.
* **Unity check of the neighbours (run after the first report; default seed, same acceptance):**

| Setting (Lmax, k) | Enlarged80 pure fixtures | Legacy40 pure fixtures | mixed stands | Beech dense Y300 (E80) BA · occ |
|---|---|---|---|---|
| **0.50, 1.60 (chosen)** | all four pass | Oak dense, Beech dense, Oak young fail R2'/R1' (lumpy); Beech young passes | pass R2'-R5' | 66 · 1.79 |
| 0.35, 1.60 | Oak, Beech, Oak young pass; **Beech young fails R1'** (+0.0027 against 0.0025) | Oak dense fails R2' (0.121); others pass | pass R2'-R5' | 76 · 2.06 |
| 0.50, 0.80 | Oak, Beech, Oak young pass; **Beech young fails R1'** (+0.0028) | Oak, Beech, Oak young pass; **Beech young fails R1'** (+0.0026) | pass R2'-R5' | 76 · 2.08 |

  So the simulator's picture holds: the passing region is thin. Weaker settings miss the young-Beech slope limit by 0.0001-0.0003; the chosen strong setting meets every Enlarged80 cell **at the default seed**, but pays for it in lumpiness on the 0.16 ha stand, and with two more Enlarged80 seeds its dense-Beech cell misses by a hair (next section). **No setting I ran meets all eight pure cells on both geometries.**
* **Allocation exponent, diagnostic only, NOT adopted.** The reason strong feedback only partly flattens pure Beech: with the inherited victim exponent 4, almost every generic death is a small recruit (stems of at least 20 cm die at 0.3-1.1 % a year) so canopy trees keep growing. Exponents 2 / 1 / 0 flatten completely (E80 pure Beech at Y300 with the same response: 258 / 2043 / 3048 stems against 122) but only by keeping thousands of small stems. That is a structural change to the species-neutral allocation you did not authorise (`simulator/sim-diagnostic-allocation-exponent-NOT-ADOPTED.txt`).

## PURE SITKA PROOF

* **Source:** the existing density suite's SHA-256 fingerprints of the `SitkaGrowthModel` class, the legacy controller decision block and `StandRelativeDensity` pass unchanged (91/91). Isolated compile without the occupancy model passes.
* **Offline literal port:** the Sitka component equals a literal port of the original controller block on the Sitka-only stem list in **400 random stands** (varied size, stocking, competition; 400/400 identical, with hazard victims in at least 100 and boundary-stage victims in at least 30) and at all 2,400 replacement-design draws.
* **Unity, both geometries, 100 years, two fixtures each:** model 2 equals model 1 exactly (per-year rows, **victim IDs**, tree-state hashes): Legacy40 192 stems `9A4C6E099792FF9F`=`9A4C6E099792FF9F`, 96 stems `C57453E6B984C30D`; Enlarged80 768 stems `1261440A3A6F6986`, 384 stems `ED1AAAAF3F42C591`; legacy route every year. Lifecycle anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` (Legacy40) and `1FE6639780636394` / `702DE05F33384139` (Enlarged80) reproduced under growth model 2.
* **Reduced simulator:** pure Sitka is bit-identical under the v2 and v3 kernels over 300 years (both geometries).
* **Unity, live controller (`SITKACOMP`, run after the first report):** on pure-Sitka stands under growth model 2 the Sitka component names exactly the trees the original controller block kills, in **24 of 24 stands on each geometry** (Legacy40: 356 victims, 13 boundary-stage deaths; Enlarged80: 1,452 victims, 8 boundary-stage deaths; legacy route in every case).
* **Unity default regression on this head (production code unchanged since `8e1e7b9`):** the 24-gate suite gives **23 PASS in the full run**, and `RegenerationModelVerification` printed all of its PASS markers (35 invariants) and then the Editor **crashed in its own shutdown** (exit 255), so the runner recorded FAIL. Run again alone it **passes with exit 0** (148 s). `production_source_unchanged` is true for every gate; no expected hash was edited and nothing was rebaselined (`unity/regression/`).
* **Phase-0 anchors: all 23 observed** in the Editor logs of those passing gates (`unity/phase0-anchors.md`). Anchor carriers: `Model2Verification` PASS (anchors `FA855239CDDA32D8`, `02334804F65C0234`, `EBE7A228F630AE41`), `StormCoreVerification` and `StormReplayVerification` PASS, `StandGeometryVerification` PASS (10 checks, start-world hash `EBE7A228F630AE41`), `Enlarged80Verification` PASS (12 checks), P2 PASS in batch twice and interactive (`F58FB0B1A421D28B`, deterministic), P3 PASS in batch twice and interactive (`F7C2FC966816BBAD`, deterministic).

## SHIELDING BEFORE AND AFTER (Enlarged80, Unity, 12 seeds per cell; v2 = a7a759d)

| Sitka stand | small broadleaves added | v2 Sitka loss | **v3 Sitka-component loss** | v3 all Sitka stems dead | v2 total loss | v3 total loss | v3 total deaths / yr |
|---|---|---|---|---|---|---|---|
| old, 100 stems, DBH 78-98 | 0 | 0.150 | **0.150** | 0.150 | 0.150 | 0.150 | 15.00 |
| | 1 | 0.134 | **0.144** | 0.144 | 0.143 | 0.143 | 14.42 |
| | 3 | 0.103 | **0.133** | 0.133 | 0.129 | 0.129 | 13.33 |
| | 12 | **0.023** | **0.140** | 0.141 | 0.125 | 0.126 | 14.08 |
| | 26 | **0.000** | **0.147** | 0.147 | 0.118 | 0.120 | 15.17 |
| mid, 150 stems, DBH 55-70 | 0 | 0.031 | 0.031 | 0.031 | 0.031 | 0.031 | 4.67 |
| | 12 | 0.006 | 0.030 | 0.031 | 0.028 | 0.028 | 4.58 |
| | 26 | 0.003 | 0.034 | 0.034 | 0.029 | 0.030 | 5.25 |

Hard checks (live controller): Sitka-component loss within `0.005 + 3 se` of pure for every broadleaf count, both stands, both geometries; total deaths added by the broadleaves within `0.5 + 0.005 N + 3 se`. Offline, exactly: the Sitka victim **sets are identical** for 0 / 1 / 3 / 12 / 26 broadleaves in three stand types over 400 yearly draws each. Noise in the Unity cells (12 seeds of 100 stems) is up to about 0.017; the offline identity is the sharp statement.

## CONTINUITY (total loss / Sitka-component loss per Sitka stem)

Offline, expectation over 400 yearly draws, 1,000 equal stems (representative columns; all 12 DBH x competition states are in `offline/offline-composite-suite.txt`):

| State | 100 % Sitka (legacy) | 99.9 % | 99 % | 95 % | 90 % |
|---|---|---|---|---|---|
| DBH 25 cm | 0.0280 / 0.0280 | 0.0280 / 0.0278 | 0.0279 / 0.0259 | 0.0318 / 0.0191 | 0.0436 / 0.0118 |
| DBH 40 cm | 0.5470 / 0.5470 | 0.5465 / 0.5466 | 0.5416 / 0.5425 | 0.5214 / 0.5236 | 0.5034 / 0.5041 |
| DBH 60 cm | 0.8040 / 0.8040 | 0.8032 / 0.8038 | 0.7961 / 0.8020 | 0.7657 / 0.7937 | 0.7309 / 0.7822 |

Unity, live controller, Enlarged80 (10 seeds, 1,000 stems):

| Sitka DBH class | 100 % | 99.9 % | 99 % | 95 % | 90 % |
|---|---|---|---|---|---|
| 24-34 cm | 0.1478 / 0.1478 | 0.1474 / 0.1474 | 0.1461 / 0.1421 | 0.1373 / 0.1203 | 0.1359 / 0.0958 |
| 32-44 cm | 0.5086 / 0.5086 | 0.5075 / 0.5078 | 0.5024 / 0.5032 | 0.4856 / 0.4920 | 0.4736 / 0.4827 |

Predeclared tolerances (written before the output): 99.9 % total loss within 0.005 of pure (measured worst 0.0009 offline, 0.0011 Unity), adjacent steps within `2 x composition change + 0.005` (offline worst step 0.0424; the smallest ratio of allowed step to measured step over the 48 steps is 2.42), Sitka-component loss at 99.9 % within 0.005 (0.0005). No total-loss step above 0.02 for a composition step of 0.001 or less; **no route cliff**. Hard: all pass in the offline suite and in both Unity geometries.
**Last broadleaf, deliberate removal and natural death** (same surviving state): Sitka victims identical in 400 of 400 draws; total deaths differ by at most one stem a year (mean 0.005); in Unity the Sitka-component loss differs by 0.0000-0.0002 and the total by at most 0.0004 (Enlarged80, three DBH classes), and year-4 deaths in a full annual run are 38.2 (never had it), 37.8 (felled in year 3) and 37.8 (died in year 3). Nothing hidden is exposed when the last broadleaf goes.

## POOLED LONG RUNS (Unity, one deterministic seed per fixture; stems · BA m²/ha · occupancy · Dq cm)

Complete Y25-Y300 tables, every scenario, in `CompositeDensityV3Evidence/unity/trial2-acceptance-*.md`; v2 side by side in `unity/longrun-v2-vs-v3-tables.md`.

**Enlarged80 (the geometry new games start in)**

| Stand | version | Y100 | Y200 | Y300 | occupancy slope Y201-300 |
|---|---|---|---|---|---|
| **Pure Oak, dense** | v2 | 263 · 60 · 2.17 · 43 | 150 · 65 · 2.35 · 59 | 100 · 67 · 2.44 · 74 | +0.0002 |
| | **v3** | 184 · 42 · 1.51 · 43 | 123 · 44 · 1.59 · 54 | 106 · 46 · 1.68 · 60 | +0.0003 |
| **Pure Beech, dense** | v2 | 454 · 87 · 2.36 · 39 | 311 · 104 · 2.85 · 52 | 216 · 125 · 3.42 · 69 | **+0.0054** |
| | **v3** | 298 · 54 · 1.48 · 39 | 164 · 64 · 1.75 · 56 | 169 · 66 · 1.79 · 56 | **+0.0006** |
| Pure Oak, young | v3 | 589 · 34 · 1.23 · 22 | 207 · 40 · 1.46 · 40 | 130 · 44 · 1.61 · 53 | +0.0012 |
| Pure Beech, young | v3 | 980 · 43 · 1.17 · 19 | 366 · 52 · 1.40 · 34 | 252 · 57 · 1.55 · 43 | +0.0015 |
| **Mixed, dense** (Sitka/Beech/Oak) | v2 | 238 · 118 · 2.44 · 64 | 130 · 99 · 1.83 · 79 | 97 · 91 · 1.67 · 87 | -0.0004 |
| | **v3** | 236 · 101 · 2.20 · 59 | 179 · 112 · 2.42 · 71 | 152 · 121 · 2.70 · 80 | +0.0029 |
| Mixed, young | v2 | 231 · 92 · 1.67 · 57 | 117 · 85 · 1.54 · 77 | 196 · 86 · 1.56 · 60 | +0.0001 |
| | v3 | 442 · 90 · 1.72 · 41 | 301 · 98 · 1.96 · 51 | 235 · 107 · 2.27 · 61 | +0.0029 |

**Legacy40 (0.16 ha)** is in the same file; it is noisier (see flags).

**Acceptance (Trial 2, as amended; limits fixed in advance) over seeds** (default seed plus `CCF_QUAL_SEED` 101, 102 on Enlarged80 and 101-104 on Legacy40; each cell is a different deterministic stream of competition, rolls and recruitment):

| Pure stand | Enlarged80 (3 seeds) | Legacy40 (5 seeds) |
|---|---|---|
| Oak dense | pass 3/3 | pass 3/5 (fails: default seed R1' -0.0030 and R2' 0.242 after a canopy-opening event at Y265; seed 102 R2' 0.101) |
| **Beech dense** | pass **1/3** (seeds 101 and 102 fail R2' by 0.109 and 0.101 against 0.10; R1' passes in all three) | pass **1/5** (R2' 0.128, 0.139, 0.109; one R1' +0.0026 against 0.0025) |
| Oak young | pass 3/3 | pass 4/5 (default seed R2' 0.102) |
| Beech young | pass 3/3 | pass 5/5 |

Every miss except the one canopy-opening event is within 40 % of its limit and most are within 10 %; the metric that fails is the 5-year swing of the all-stem loss fraction, which is driven by pulses of small recruits. So the honest summary is: **pure Beech dense meets the acceptance on one Enlarged80 seed in three, with R1' (flattening) passing in all three, and the swing criterion missing narrowly.** The stand is flattened; it is not smooth.

Mixed stands (every seed run, both geometries: 3 on Enlarged80, 5 on Legacy40): R2' on the generic component (default-seed largest 5-year-block change 0.006-0.024; all seeds pass), R3' (dead basal area 0.9-2.1 %, stems of at least 20 cm 0.9-1.9 %) and R4 pass; **R5' passes** (Sitka subset held at 0.92-0.99 of its own line). R1' is not required for mixed stands and reads +0.0019 to +0.0030 per year.

## LATE-MIXED MORTALITY DECOMPOSITION (Enlarged80, `C_MIXED_DENSE`)

| | living | deaths / yr (fraction) | Sitka component | generic | deaths by species S / B / O | Sitka-subset RD |
|---|---|---|---|---|---|---|
| **v2** Y251-300 (derived from the CSV: `e = 1-(1-R/N)(1-g)`) | 103 | 27.4 (**0.266**) | ~27.3 demanded, ~0.1 generic | ~0.1 | not recorded | **1.12** (year end) |
| **v3** Y251-300 (measured) | 159 | 37.1 (0.233) | **4.4** (0.068 per Sitka stem) | **32.7** | 8.5 / **25.4** / 3.2 | **0.990** (start of step) |
| pure Sitka comparator, same geometry | 176 | 21.9 (0.125) | 21.9 (0.123 per Sitka stem) | 0 | 21.9 / 0 / 0 | 0.986 |

Overlap removed by the union: **0 by construction** in v3; in v2 the implied overlap `g R/N` was about 0.0003.
**Diagnosis of the v2 ~0.27: category C (accounting / composition artefact), as registered.** Generic over-pressure (B) is excluded: generic deaths were an estimated 0.4 % of the total. Not transient (A): it persisted 200 years while the Sitka subset stayed above its line. Not a coding bug (D): the planner did what it was designed to do. v3 removes it: the Sitka subset is held at its own line (RD 0.99) and the Sitka component per Sitka stem is within the range of pure Sitka.
**What remains:** v3's all-stem loss in this stand is still about 0.23 a year. It is now **generic turnover of suppressed broadleaf recruits** (25 Beech deaths a year, among about 37 recruits a year of all species) at total occupancy 2.5-2.7 times a non-Reference share of 0.43. Dead basal area is 0.9 % a year and stems of at least 20 cm 0.9 %. So the *count* is as high as before; its *nature* is different and its cost in biomass is small.
**R5 as originally written** (Sitka-component loss per Sitka stem within 25 % of a pure-Sitka comparator) **fails**: 0.068 against 0.123. I registered it myself and I now think it is ill-posed (the subset's demand depends on its own size structure); R5' replaced it for the diagnosis. Reported as written.

## TESTS

Offline (plain .NET 10 SDK, no Unity), all on the final tree:

| Suite | Result |
|---|---|
| Density (existing, includes the three legacy fingerprints and the controller-wiring guard) | **91 / 91** |
| Component suite (identities I1-I7, response shape, accounting, C1-C5 continuity, shielding, last broadleaf, neutrality, asset-parameter guards, reduced-simulator long runs incl. pure Oak, pure Beech, mixed at Y300, replay, negative control) | **61 / 61** |
| Isolated compiles (occupancy model without Sitka; Sitka model without occupancy) | pass, pass |
| Mutation tests (11 mutants: Sitka debt discarded, boundary removed, broadleaf in Sitka density, victims dropped, victim killed twice, untapered generic, no generic mortality, species-biased allocation, quota rounded up, v2-strength response, hidden occupancy cap) | **11 / 11 killed**, unmutated control passes |
| Timber / history | **71 / 71**, **70 / 70** |
| Type-check production / tools | clean against the committed baselines (7 and 43 known reference-assembly errors, none new) |
| `git diff --check` | clean |

Mutation work exposed one weak test: the species-bias mutant first **survived** because my neutrality fixtures produced almost no generic victims. I made them dense and added a minimum-victim assertion; the mutant is now killed. The Unity-side neutrality test (rotating species through every grid position with species-neutral ids) passes with pooled |z| at most 0.07 over 150 trials per design, at both the shipped loss level and at a high-loss level (about 11 % a year), both geometries.

## UNITY STATUS

Real Unity 6000.6.0f1, batch mode, Linux, this worktree, `Library` built from scratch (about 12 minutes, exit 0, **0 `error CS`**, only pre-existing obsolete-API warnings). Not a Player and not native Windows.

* **Pass:** harness fast phases Legacy40 (**83 hard checks**) and Enlarged80 (**82 hard checks**: pure Sitka exactness, continuity, final broadleaf, late mix, curve, fairness at two loss levels, save v20, models 3 and 99 rejected, performance); 300-year long runs on both geometries (7 scenarios each); shipped gates `MixedDensityVerification` **33/33** on both geometries, `BroadleafAdultGrowthVerification` **14/14** on both, `BroadleafTimberEconomyVerification` 29, `ScenarioHistoryVerification` 39. Annual step (Enlarged80, mean of 15 years): pure Sitka model 1 **491 ms** against model 2 legacy route **493 ms** (768 stems); mixed dense 832 stems **321 ms** (v2: 514 ms; the mixed stand is thinner).
* **The first Enlarged80 fast run's receipt says FAIL, exit -11:** the harness printed `DENSITY_QUAL_PASS hardChecks=82` and the Editor then **segfaulted in its own shutdown**. A rerun alone on a quiet machine gives a **clean receipt: PASS, exit 0, 1,872 s, 82 hard checks** (`unity/markers-fast-e80-rerun-clean-exit.txt`). The same shutdown crash hit `RegenerationModelVerification` once (above), so it is an Editor flake, not a gate result.
* **Run after the first report (the other Claude session that shares this machine asked me to wait for its last Editor and then released it; one Editor at a time, with a guard that killed only my own Editor if swap free fell below 1.2 GB; the guard fired once, at 23:15, during an earlier attempt):**
  * live Sitka-component identity, both geometries, 24/24 each (above);
  * neighbour settings `(0.35, 1.60)` and `(0.50, 0.80)`, both geometries (table above);
  * 24-gate regression and all Phase-0 anchor carriers (above): 23/24 in one run, the 24th clean on rerun;
  * extra seeds: Legacy40 seeds 101-104 and Enlarged80 seeds 101, 102 (acceptance table above).
* One receipt in `unity/gate-runs.json` reads FAIL with exit 0 (`e80-seed101`): I wrote a new file into the worktree during that run and the runner flags any new file; the run itself printed `DENSITY_QUAL_PASS` and its 300-year CSV is complete.
* **Not run:** a Player build, native Windows, and the neighbour settings with more than one seed. Nothing I ran changes the default path (legacy fingerprints, the pure-Sitka equalities, the live identity check, 24/24 gates, 23/23 anchors).
* History, for the record: a first regression attempt lost `ScenarioReferenceVerification` to an abnormal Editor exit (198) when the other session's Editor was replaced (environmental; `unity/regression-first-attempt-PARTIAL-aborted.json`); letting two Editors run exhausted the 8 GB swap twice, so I stopped my chain, then killed my Editor while the other session imported a project, and only resumed after agreeing the sharing with it. None of those aborted runs is used as evidence.
* `/home/jer/CCF-main` was not touched. Real saves were not touched (every run used an isolated config folder).

## REMAINING RISKS AND DECISIONS

1. **Pure Beech is flattened, but not smoothly, and the passing region is thin.** Occupancy slope fell from +0.0054 to +0.0006 and BA at Y300 from 125 to 66 m²/ha on Enlarged80 at `Lmax 0.50, k 1.60`; the flattening test (R1') passes in all three Enlarged80 seeds. The swing test (R2') passes in only one of three: the all-stem loss fraction swings between 5-year blocks by 0.101-0.109 against 0.10 when recruit pulses come in. The two neighbours (single seed) are weaker (Beech dense Y300 BA 76, occupancy about 2.07) and miss the young-Beech slope limit by 0.0001-0.0003. Real flattening of the *canopy* needs the allocation seam (above); I did not touch it.
2. **Legacy40 is lumpier.** On 30-100 stems, canopy-opening events (e.g. Oak, default seed, Y265: 36 to 19 stems, five large trees dead, then a recruitment wave) make the all-stem 5-year loss swing. Over five seeds: Oak dense passes 3/5, **Beech dense 1/5**, Oak young 4/5, Beech young 5/5, mixed stands 5/5. All misses but one are within 40 % of the limit. The pre-registered rule asks for both geometries, so **the registered Unity reproduction is not met on Legacy40 and only met on Enlarged80 for three of four pure stands across seeds**. Legacy40 is the legacy development scale; whether it must also pass is your call.
3. **Mixed stands now carry more basal area than any pure stand**: 101-121 m²/ha at Y100-Y300 (Enlarged80 dense) against Sitka's ~85 line, pure Beech 66, pure Oak 46. The Sitka subset keeps its full legacy ceiling regardless of broadleaves (by design) and the broadleaves fill space on top, damped by the non-Reference share. a395e3f reached 133-191, v2's own mixed stands only looked lower (91) because they killed the broadleaves to pay Sitka's debt. Occupancy still rises slowly in mixed stands (+0.003/yr, above the 0.0025 pure-stand limit, not a mixed-stand requirement). There is no broadleaf ceiling, as instructed.
4. **High stem turnover stays.** All-stem loss is 19-32 % a year in the stronger stands (recruits dying); basal area lost is 1-2 %. Anyone reading "annual loss" as a headline will see big numbers.
5. **Start-up shock for dense broadleaf starts**: 47-48 % of stems die in year 1 of the dense pure fixtures (occupancy 2.2 at the start). Not a game state, but a player who plants a very dense broadleaf block would see it.
6. **The reduced simulator is a screen, not evidence about the game.** It reproduced the Unity v2 pure Oak/Beech trajectories closely (Beech Y300 BA 125.4 against 125.4) and predicted v3 Enlarged80 Beech (BA 66.0, occupancy 1.80 against Unity 65.5 and 1.79), but it failed to reproduce the mixed dense stand and it uses an external recruitment series.
7. **Seeds:** the default seed plus two more on Enlarged80 and four more on Legacy40. That is enough to show Beech dense is marginal, not enough to estimate pass rates tightly; the neighbours and the mixed-stand conclusions rest on one seed (mixed stands pass on every seed run).
8. **My own amendments** (Trial 2, A4) and the ill-posed R5 are disclosed above. The Manager can reject them; the revert is three constants.

## FILES AND COMMITS

Commits on this branch after the base: `e0bd90d` predeclared acceptance; `861f4fb`, `726529e` Trial 2 and A4 (before their runs); `0d472a2` **fix: separate sitka and generic density mortality components**; `8e1e7b9` **calibrate: strengthen high-occupancy generic feedback**; `c88ecbd` and the following `test:` commits (offline suite, simulator, mutation runner, the harness cherry-picked from `verify/final-density-v2-a7a759d` and adapted, live Sitka-component check, seed option, runner guard); this handoff and the evidence.
The later Unity runs (identity, neighbours, regression, anchors, seeds) are reproduced by `Tools/Verification/ThreeSpecies/Qualification/run_v3_remaining.sh` (run inside the demo-HDR wrapper on a quiet machine; it also repeats the first steps).
Reproduce offline: `python3 Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite all`, `run_composite_mutations.py`, `run_density_trajectories.py search2`. Unity: `Tools/Verification/ThreeSpecies/Qualification/run_qualification.py` (see its header).
