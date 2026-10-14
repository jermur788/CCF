# U3 — FINAL DENSITY V2 UNITY QUALIFICATION HANDOFF

Role: Unity Integration & Qualification Engineer. **Not merged. Growth Model 2 not adopted. No anchor rebaselined. No production C# changed.**
Everything below was run in real Unity 6000.6.0f1 on the worktree named here. Items I could not or did not verify are listed under LIMITS.

**RESULT: B — QUALIFIED WITH FLAGS.** Every hard property I defined for U3.1 and U3.3 passes, and the route cliff and lost Sitka ceiling that earned a395e3f its Result C are gone. Rule I applied, stated so it can be overruled: *A* = no flags, *B* = hard properties pass and the open items are decisions for the Manager, *C* = a hard property or a packet "Reject" criterion fails. Two places admit a stricter reading of the packet, and under either of them that item is C (nothing else in this report would change):
- **Flag 1** — pure Beech occupancy is still rising slowly at Y300 (packet: "reject continuing runaway occupancy").
- **Flag 2** — "no next-year mass Sitka mortality discontinuity" passes for the *total* loss and for the last single broadleaf, but measured in *Sitka-only* terms the Sitka loss of an old stand steps from about 0.02 back to 0.15 when a dozen shielding broadleaves are gone.

## IDENTITY

| | |
|---|---|
| Source branch / head | `task/mixed-density-architecture-rework-v2` @ `a7a759ded087a824c1e8149e480ddf58ac0585eb` (equal to `origin/task/mixed-density-architecture-rework-v2` at the last `git ls-remote` before this report) |
| Qualification branch | `verify/final-density-v2-a7a759d` (local only; **not pushed, not merged**) |
| Production diff vs source head | **none**: `git diff a7a759d HEAD -- Assets Packages ProjectSettings` is empty. The four files that differ are all under `Tools/Verification/ThreeSpecies/Qualification/` (harness, probe, runner, completion generator) |
| Harness commits | `6745899` harness lifted from the a395e3f qualification; `5552de3` working allocation counter; `2622fe0` adapted to the v2 composite route (`SitkaSubsetDensitySignal.RequiredDeaths` + `MixedDensityModel.PlanComposite`); `06d61a2` continuity + high-loss fairness phases; `c862709`, `58c0d9b` late-stand phase (total-loss continuity hard, Sitka shielding measured, counts scaled by area). Evidence and this file are added by the commit on top |
| Unity | 6000.6.0f1 (`/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity`), one Editor at a time, `systemd-run` 8 GB cap, isolated `XDG_CONFIG_HOME` per run |
| Worktree | `/media/jer/Files/CCF-verify-final-density-v2-a7a759d` (own `Library`; `/home/jer/CCF-main` untouched) |
| Real saves | `~/.config/unity3d/DefaultCompany/CCF/forest-save.json`, `.bak`, `prefs` have modification times of 2026-10-02/03, i.e. none was written by any run on 2026-10-14 |

## COMPILE

PASS. `Library` imported from scratch in this worktree (about 11 minutes), exit 0, **0 `error CS`**, 481 `CS0618` obsolete-API warnings (pre-existing class). Editor re-serialisation of `.vscode/settings.json` and the `Art/SectionFive` materials was reverted after every run (diffs kept in the ignored `Build/U3/editor-normalisation-*.diff`); the tree is clean apart from this evidence.

## PURE SITKA (U3.1) — PASS

- **Exact Model 1 == Model 2 for 100 years** on both geometries, two fixtures each (Enlarged80 768 / 384 stems; Legacy40 192 / 96): per-year row hash, tree-state hash, **victim IDs** all equal and the route is `LegacySitkaExact` every year (`PURE_SITKA_{DENSE,MODERATE}_100Y_MODEL2_EXACTLY_EQUALS_MODEL1`; e.g. E80 dense `1261440A3A6F6986` = `1261440A3A6F6986`, L40 dense `9A4C6E099792FF9F` = `9A4C6E099792FF9F`).
- Lifecycle hashes `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` reproduced under growth model 2 on Legacy40; the 80-year model 1 / model 2 lifecycle on Enlarged80 is bit-identical (neutral `1FE6639780636394`, normal `702DE05F33384139`).
- Deterministic ordering: adjacent roll seeds are uncorrelated (corr 0.0007); every long run replays exactly.
- **Phase-0 anchors: 23/23** found in the real Editor logs of gates that passed in this run (`evidence/phase0-anchors.md`). Note the P3 value here is `F7C2FC966816BBAD` because this branch does not carry the U1 presentation fix (`AF7405918195F0DE` lives on `ux/scenario-one-small-fixes-84e881a`); the world hash `EBE7A228F630AE41` is unchanged either way.

## MIXED CONTINUITY (U3.1) — PASS, no route cliff

`CONTINUITY` phase, three DBH classes, Sitka share 100 / 99.9 / 99 / 95 / 90 % (`continuity-*.csv`). Only exactly 100 % Sitka takes the legacy route; any broadleaf takes the composite route. Annual loss, Enlarged80:

| Sitka DBH class | 100 % (legacy) | 99.9 % | 99 % | 95 % | 90 % |
|---|---|---|---|---|---|
| 16–24 cm | 0.000 | 0.000 | 0.000 | 0.000 | 0.000 |
| 24–34 cm | 0.1478 | 0.1472 | 0.1410 | 0.1166 | 0.0917 |
| 32–44 cm | 0.5086 | 0.5073 | 0.4995 | 0.4734 | 0.4478 |

100 → 99.9 % differs by **0.0000 / 0.0006 / 0.0013**; every adjacent step is inside its predeclared tolerance (2 × the composition change + 0.005 + 3 se; the measured difference is at most 0.26 of its tolerance in every Enlarged80 cell). Legacy40 passes the same checks (50 hard checks). a395e3f's cliff (when the last broadleaf died, the all-Sitka stand lost 39 % the next year) is not reproduced.

## FINAL BROADLEAF (U3.1) — PASS

Both removal paths, Enlarged80 loss per year with the broadleaf present → felled → died naturally → never present: `DBH16_24 0 / 0 / 0 / 0`, `DBH24_34 0.1497 / 0.1489 / 0.1492 / 0.1492`, `DBH32_44 0.5048 / 0.5041 / 0.5043 / 0.5043`. Loss difference ≤ 0.0002, ≤ 0.3 deaths per 1000. Year-4 deaths in a full run: never 42.3, felled 42.5, died 42.5 (inside 5 % + 5 stems). **No next-year Sitka mortality discontinuity.** Legacy40 (denser fixture, higher losses) shows the same: 16–24 cm `0.5457 / 0.5447 / 0.5451 / 0.5451`, 24–34 cm `0.7959 / 0.7949 / 0.7956 / 0.7956`, 32–44 cm `0.8819 / 0.8809 / 0.8817 / 0.8817`.

## LATE-STAND MIXING (extra phase, added after the first long runs)

Fixtures: large old Sitka at high relative density, plus 1 / 3 / 12 / 26 small broadleaves (counts scaled by area). **Disclosure:** my first version of this phase asserted that a few broadleaves leave the *Sitka* loss as in the pure stand. It failed (E80: Sitka loss 0.150 → 0.103 / 0.023 / 0.000 with 3 / 12 / 26 broadleaves) and the Legacy40 fixture was not area-scaled. The expectation was wrong for a design that gives required deaths to the weakest competitors, so I replaced it with a hard check that the *total* loss stays continuous and recorded the Sitka loss as a measure. No tolerance was loosened to make a failing check pass; the property was changed and the failed run's logs are kept (`evidence/markers-first-late-run-*`, raw `Build/U3/late-*-first-hardchecks-superseded/`).

| Old Sitka (N100, DBH 78–98) | pure | +1 | +3 | +12 | +26 |
|---|---|---|---|---|---|
| total annual loss | 0.150 | 0.143 | 0.129 | 0.125 | 0.118 |
| Sitka loss | 0.150 | 0.134 | 0.103 | 0.023 | **0.000** |
| share of broadleaves dying | | 1.00 | 1.00 | 0.97 | 0.57 |

Hard check (total loss continuous within tolerance): PASS, 8/8 per geometry. **Measured:** the deaths a mixed stand owes are spent on whichever stems are the weakest competitors, so small broadleaves *shield* old Sitka (Sitka loss 0.150 → 0.000 at 26 broadleaves) and are themselves consumed (flag 2).

## LONG RUN (U3.2) — recorded, not tuned

Fixtures are the shipped `MixedDensityVerification` stands; one deterministic seed each. Enlarged80, 300 years, `n` stems (0.64 ha), BA m²/ha, occupancy (1.0 = onset), Dq cm, mixed stands show Sitka / Beech / Oak living. Complete tables, Y25 to Y300 every 25 years, with the older a395e3f 100-year file for comparison: `evidence/longrun-tables-E80-300y-vs-a395e3f-100y.md`, `…L40-300y-vs-a395e3f-300y.md`; per-year CSVs beside them.

| Scenario | Y25 | Y50 | Y100 | Y200 | Y300 |
|---|---|---|---|---|---|
| A pure Oak, dense | 423 · 47 · 1.70 · 30 | 399 · 54 · 1.98 · 33 | 263 · 60 · 2.17 · 43 | 150 · 65 · 2.35 · 59 | 100 · 67 · 2.44 · 74 |
| B pure Beech, dense | 536 · 60 · 1.64 · 30 | 663 · 70 · 1.92 · 29 | 454 · 87 · 2.36 · 39 | 311 · 105 · 2.85 · 52 | 216 · 125 · 3.42 · 69 |
| C mixed, dense | 568 · 76 · 1.91 · 33 (182/225/161) | 537 · 101 · 2.40 · 39 (174/232/131) | 238 · 118 · 2.44 · 64 (137/56/45) | 130 · 99 · 1.83 · 79 (104/26/0) | 97 · 91 · 1.67 · 87 (85/12/0) |
| G mixed, young | 1592 · 51 · 1.23 · 16 (531/530/531) | 1318 · 83 · 1.82 · 23 (492/416/410) | 231 · 92 · 1.67 · 57 (224/7/0) | 117 · 85 · 1.54 · 77 (117/0/0) | 196 · 86 · 1.56 · 60 (196/0/0) |

Annual density mortality (deaths ÷ living, E80): A 0.025 → 0.088; B 0.021 → 0.175; C 0.025 → 0.111 (Y100) → 0.27 (Y251–300); G 0.001 → 0.087 (Y100) → 0.12.

Against a395e3f (same stands, same geometry; its Legacy40 300-year file for B and C):

| | a395e3f | v2 (a7a759d) |
|---|---|---|
| C mixed dense, BA at Y100 (E80) | **133** | 118 (peak 125 at Y82, then falls to 91 at Y300) |
| G mixed young, BA at Y100 (E80) | **132** | 92 (Y300: 86, with a pure Sitka stand left) |
| C mixed dense, BA / occupancy at Y300 (L40) | 191 / 3.80 | 83 / 1.50 |
| B pure Beech, occupancy at Y300 (L40) | 4.65, slope +0.0114/yr | 3.35, slope +0.0032/yr (last 50), +0.0052 (last 100) |
| A pure Oak, occupancy at Y300 (L40) | 2.60 (peak 3.01) | 2.30 (peak 2.48), slope −0.0016/yr |
| route flip, stand G | 39 % loss in one year | Y113–119 loss 0.083, 0.084, 0.092, 0.066, 0.084, 0.074, 0.057 (E80), no step |

Against the packet's four rejections:
- **Continuing runaway occupancy** — A: no (slope −0.0005/yr last 50 years, peak 2.52, BA 67). C, G: no (declining or flat). **B pure Beech: still rising at Y300** at +0.0058/yr (E80), +0.0032/yr (L40) with death fraction climbing 0.02 → 0.175; roughly half the old rate and a lower level, but no plateau. See flag 1.
- **Mass oscillatory collapse** — no. Largest year-to-year change in the death fraction after Y25 is 0.122 in C (E80) and 0.016 in A; the 0.71 (G, E80, into Y282), 0.77 (G, L40, Y231) and 0.79 (pure Sitka D, L40, Y245) figures are year-to-year swings of the legacy Sitka boundary-stage pulses on stands of 50–200 stems. The pure Sitka run D is on the legacy route, which I proved bit-identical to model 1 for 100 years (not for 300), and has 39 years above 15 % and a mean death fraction of 0.125 over its last 50 years.
- **Route-switch cliff** — no (table above; Legacy40 G shows 0.196 and 0.186 on either side of the flip at Y119–Y121, 42–46 stems, same size on both routes).
- **Artificial species bias** — no (below).

Observed annual loss follows the model curve (`curve-*.csv`): pure Oak, model value 0.0001 / 0.0020 / 0.0123 / 0.0454 / 0.1380 at occupancy 1.05 / 1.2 / 1.5 / 2.0 / 3.0 against observed 0.0000 / 0.0018 / 0.0126 / 0.0456 / 0.1381; largest |z| over all 21 rows is 0.76 (Enlarged80) and 0.70 (Legacy40). The three-species mix loses less than pure Beech at equal occupancy (0.1069 v 0.1379 at 3.0) because the generic loss scales with the non-Reference share, as designed.

## FAIRNESS (artificial species bias) — PASS

Species rotated through every grid position with species-neutral tree IDs, 150 trials per design, at the shipped loss level (occupancy 1.6, 1.4 % a year) and at a high-loss level (occupancy 3.0, about 11 % a year; the species-neutrality of the allocation could otherwise only be argued at low loss). Competition index bit-identical when species are swapped (max abs difference 0.0e+0); controller victims equal the pure plan in every trial (16 003 and 17 052 victims at high loss on E80); pooled deaths match Bernoulli expectation (pooled |z| 0.16–0.32 at the shipped level on both geometries, 0.03–0.06 at high loss); no species departs from the competition-alone expectation (largest species |z| relative to the pooled ratio: 1.59 shipped, 1.91 high loss, limit 4). The high-loss phase ran in the first late-stand run, whose LATEMIX checks were superseded as described above (`evidence/markers-first-late-run-*`). Victims are the more suppressed stems (AUC 0.623; mean CI victims 5.18 v survivors 4.79). Species elimination in C and G is competition, not class mechanics: Oak reaches 0 in C by Y200 while Beech and Sitka remain.

## REGRESSION (U3.3)

| Gate | Result |
|---|---|
| 24-gate suite (`Tools/Verification/WindthrowV1/run_regression.py`, default growth model 1) | **24/24 PASS** (`evidence/regression-24-results.json`), `production_source_unchanged` true for every gate |
| `MixedDensityVerification` Legacy40 / Enlarged80, shipped, unmodified | **PASS 33 / PASS 33** |
| `BroadleafAdultGrowthVerification` Legacy40 / Enlarged80 | **PASS 14 / PASS 14** |
| `BroadleafTimberEconomyVerification` | PASS (29 assertions) |
| `ScenarioHistoryVerification` | PASS (39 assertions; first run failed in 1 s because my nested display had died, not in the gate; rerun on a fresh display passed) |
| `Model2Verification` / `StormCore` / `StormReplay` / `Enlarged80` / `StandGeometry` / P2 / P3 | PASS (anchor carriers) |
| Save v20, model 2 mixed | no new persisted field (96 keys == model 1); disk round trip continues identically (`1547EB06D7B40192` = `1547EB06D7B40192`); restore through a dying stand reproduces every year's deaths |
| Growth models 3 and 99 | refused, world unchanged ("unknown growth model") |
| Harness `DensityQualification` | Enlarged80 **49 hard checks PASS**, Legacy40 **50 PASS** (fast phases); late-stand phase 8 / 8 PASS; the long runs record measures, not hard checks (`gate-runs.json`) |

Timber/economy and history are unchanged by v2 (no diff in the code they exercise).

## PERFORMANCE SANITY

No regression beyond noise. Editor batch, 15 years per case. Annual step (mean, ms): Legacy40 pure Sitka 192 stems model 1 **41.9** vs model 2 legacy route **41.4**, mixed dense 208 stems **41.1**, pure Oak 160 stems 26.8; Enlarged80 pure Sitka 768 stems **494** vs **495**, mixed dense 832 stems **514** (p95 693), pure Oak 640 stems 351. a395e3f measured 524 / 505 / 476 on the same geometry, i.e. the mixed case is 8 % slower, inside the run-to-run spread of this machine. Pure `PlanComposite` time for 832 stems 1.69 ms per plan. Mono heap growth per year 0.2–1.5 MB.

## FLAGS (decisions for the Manager, nothing changed)

1. **Pure Beech has no equilibrium within 300 years.** E80 occupancy 1.64 → 3.42, BA 125 m²/ha at Dq 69 cm, slope +0.0058/yr; L40 3.35. It is a different order from a395e3f (4.65, +0.0114), and BA rising at ≈ +11 m²/ha per 50 years is consistent with approaching the species' size caps (Beech max 120 cm), but I did not prove that it saturates. There is **no ecological Beech ceiling in the repo** and I did not invent one; the question is whether BA ≈ 125 for old pure Beech is acceptable, which is a research/calibration call.
2. **Sitka shielding, and a Sitka-only step when it ends.** Adding up to 26 small broadleaves to an old Sitka stand cuts the Sitka loss from 0.150 to 0.000 while the *total* loss stays continuous (0.150 → 0.118). By construction the required deaths go to the weakest competitors; it is neutral between species (fairness passes). The consequence is that the Sitka loss of the old stand is about 0.02 with 12 broadleaves present and **0.15 once they are gone**, while total deaths move only 0.125 → 0.150. My FINALBROADLEAF property (one last broadleaf, felled or dead) passes with a difference of at most 0.0008, but I did **not** run a "twelve broadleaves cleared in one year" case, so the Sitka-only step is inferred from the shielding table, not measured as a transition. Gameplay-visible in plans that underplant old Sitka with Oak/Beech. Whether the property should be read in total or in Sitka-only terms is the Manager's call.
3. **High turnover in the late mixed stand.** E80 C Y251–300: 1370 density deaths in 50 years on a mean 103 living (0.27 per year, all on the hazard route, none on the boundary stage), against 0.125 for pure Sitka at occupancy 1.5; the mixed stand sits at occupancy 1.7–1.8, BA 91–99. Not collapse (living stems fall only 110 → 97 between Y250 and Y300 while about 27 stems a year die, so recruitment of roughly that many a year must balance it; inferred from the counts, not measured), but I did not isolate how much is the higher occupancy and how much the way the Sitka subset signal and the generic loss are combined. Recommended follow-up: one controlled pair at equal Sitka stems ± 12 Beech.
4. **Single deterministic seed per fixture** in all long runs (see LIMITS).

## LIMITS — not verified

- One seed per long-run fixture; no seed-to-seed spread. Conclusions about flag 1 and flag 3 would be firmer with 5–10 seeds.
- Long runs are fixture stands from the density gate, not the Scenario One playthrough. I did not run the two-cycle completion (U4 waits for an implementation head) or a rendered by-eye review of the density change, because the v2 branch changes no UI.
- Editor batch evidence only for density; no Player, no native Windows.
- Timings come from a thermally throttling laptop GPU/CPU machine and a shared desktop; use them for ratios only.
- The packet's "Result C" class definitions were not restated in the packet; the A/B/C rule above is mine.

## EVIDENCE INDEX (`Docs/Verification/CompositeDensityV2Qualification/evidence/`)

`markers-*.txt` (hard-check and measure lines per run) · `curve-*.csv`, `continuity-*.csv`, `finalbroadleaf-*.csv`, `latemix-*.csv` · `longrun-E80-100y.csv`, `longrun-E80-300y.csv`, `longrun-L40-300y.csv` and the three `longrun-tables-*.md` generated by `Tools/Verification/ThreeSpecies/Qualification/analyze_longrun.py` · `regression-24-results.json` · `gate-runs.json` · `phase0-anchors.md`. Raw Editor logs stay in the ignored `Build/U3/`.
