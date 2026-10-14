# Oak/Beech ordinal [C] production calibration v1: Unity qualification

Role: Unity Integration & Qualification Engineer. **Not merged. No production file changed by me. No anchor rebaselined. Nothing pushed to `main`.** Everything below ran in real Unity 6000.6.0f1 on the worktree named here, unless it says otherwise.

## RESULT: B — QUALIFIED AS AN ORDINAL [C] CALIBRATION, WITH THE WEAK-IDENTIFIABILITY CAVEAT, AND WITH INTENTIONAL ANCHOR CONSEQUENCES THE MANAGER MUST ACCEPT

What passes: the shipped rows reach the runtime exactly; the controller follows its documented law with them bit-exactly; the release response, height independence, bounded growth, promotion, determinism and save/load properties the packet lists all hold; Sitka and every pure-Sitka anchor I could observe are unchanged.

What the Manager has to know before integrating (none of it is a defect in the calibration itself):
1. **It changes any play that contains planted or reference Oak/Beech.** Two regression gates fail on exact hashes (`ScenarioOneCompletionVerification`, `ScenarioReferenceVerification`'s continuation anchor). Old-rows A/B runs in the same worktree prove the calibration is the only cause (section 2A).
2. **Existing saves change behaviour on resume**: species rows are not in the save, so an old save's Oak/Beech trees grow with the new rows (section 2E). There is no migration.
3. The lab's "stock" fixture assumed a 0.15 m sapling; the shipped planting uses a higher stock height, so promotion is **one year earlier** than the lab's age 10 (section 2B).
4. Weak identifiability stays: these are search-grid centre values, not fitted. I did not promote them to empirical status, and nothing here could.

Rule I applied (so it can be overruled): *A* = every packet property holds and nothing the Manager has to decide remains beyond "[C] stays [C]"; *B* = every packet property holds but a caveat or a consequence needs a decision; *C* = a packet property fails; *D* = unsafe.

## IDENTITY

| | |
|---|---|
| Source branch / head | `task/oak-beech-ordinal-production-calibration-v1` @ `796da99029190ef71e68d5fa722f25240e696318` (equal to `origin/` at `git fetch` on 2026-10-14) |
| Qualification branch | `verify/oak-beech-production-calibration-796da99` (new, from `796da99`) |
| Production diff vs source head | **none**: `git diff 796da99 HEAD -- Assets Packages ProjectSettings` is empty. Harness commits touch only `Tools/Verification/OakBeech/`, `Tools/Verification/Qualification/` and this folder. |
| Worktree | `/media/jer/Files/CCF-verify-oak-beech-796da99` (own `Library`, imported from scratch inside the demo-HDR quarantine wrapper) |
| Unity | 6000.6.0f1, one Editor at a time on this project, `systemd-run` scope capped at 8 GB, isolated `XDG_CONFIG_HOME` per run (the real save slot was never touched) |
| Geometry / models | New-game default Enlarged80, growth model 1 (the default), RNG 1, regeneration 2. Models 0 and 2 also run. Historical gates ran in Legacy40 where they require it. |
| Context commit | The packet gave none. I locked to the exact source head above. |

**Compile:** PASS. Fresh import and compile, exit 0, 0 `error CS`. Evidence: `evidence/results-*.json`, runs started from a clean tree.

## 2A — ABSOLUTE REGRESSION BOUNDARY

The production diff against its own base (`b4894cc`) is **two files, 18 changed lines**: `Species/SessileOak.asset` and `Species/BeechSpecies.asset` (five growth rows each, plus `[C]` comments). `git diff b4894cc 796da99 -- Assets` shows nothing else. Relative to `origin/main` (`c1564b8`) the branch also carries the overnight lineage (mixed-density model, Oak/Beech timber economy, history); that lineage is qualified by other lanes. This packet qualifies only the calibration delta.

Held constant, proven at runtime and by hash:
- **Sitka rows**: runtime `Pd 1.2 / Ph 0.45 / Dmax 120 / Hmax 35 / Ci50 5`, equal to the asset and unchanged; the lab runner also matches seven protected files by SHA-256 (`SitkaGrowthModel.cs`, `SitkaSpruce.asset`, `ForestEcologyController.cs`, `TreeSpeciesDefinition.cs`, `MixedDensityModel.cs`, `ScenarioOneObjectives.cs`, `ForestSaveData.cs`).
- Save version 20; growth-model constants unchanged; the default growth model for a new game is still 1.
- Timber thresholds and prices: no file under `WorkEconomy/` or `TimberYield/` differs from the base. `BroadleafTimberEconomyVerification` PASS and the offline timber suite (71 checks) PASS.
- Density architecture and numerical site response: no file differs.

**Pure-Sitka anchors observed unchanged** (`evidence/phase0-anchors.md`): lifecycle `BFC55473C1506067`, `3485B6630C9EA448`, `962846D2F517B293`, `FBB8F470D85FF815`; growth-model-1 pure Sitka `7E57B9DAEF5BF4D0`, `35E2BF1C2F55C2DE` (also in `MixedDensityVerification`, which passes on Legacy40, 22 checks); Reference Future v1 archive `A564039D9B7CE31D`, `F7DF7DAB53B6FD32`, `D5E75D6D21D631AC`, `7AD177B3CC2F73C7`, schedule `56C8B99FA1E8DDD1`; Legacy40 start world `EBE7A228F630AE41`; Enlarged80 start/year-3 `04A78188A6F8E6F9`, `081DB58CE3034648`; storm anchor 1 `8333BAA4126E8A09`.

**Intentionally changed anchors (consequence of changing Oak/Beech growth):**

| anchor | before | after | proof that the calibration is the only cause |
|---|---|---|---|
| Model2 completion world hash (`SCENARIO_ONE_COMPLETION_V18_COMPAT_HASH`) | `702766DECE591E21` | `D99EC9FC2C635064` | same gate, same head, pre-calibration rows applied in memory (never saved): PASS with `702766DECE591E21`; also equals the value the pre-U4 head gave in the U4 packet |
| Reference continuation (v15 layout hash) | `9CDF21A541C5968D` | `BFFF66015A6619DA` | `ScenarioReferenceVerification` with old rows applied: `legacyV15LayoutHash=9CDF21A541C5968D`. The frozen archive and preview anchors above are unchanged, so **the frozen Reference Future v1 bytes are untouched**; only the live-rules continuation of the archive's Oak/Beech trees moves |

`ScenarioOneCompletionVerification` with the new rows: eight objectives still met at year 25 (`retained 210/60, canopy 0.889, regen 55, deadwood 4.93 m³` against `209, 0.885, 54, 4.405` before), but the exact-hash assertion fails. So the calibration also nudges the Sitka-side ecology of a Scenario One play with planted broadleaves; it does not do so for a pure-Sitka stand.

**24-gate regression** (the main-line runner, Legacy40): 21 PASS, 3 not PASS: `ScenarioReferenceVerification` and `ScenarioOneCompletionVerification` as above, and `Model2PedagogyVerification`, which printed `MODEL2_PEDAGOGY_PASS checks=20` and then did not exit (a hung graphical Editor); I killed it, so the runner recorded exit -9. `ClearanceVerification` and `MenuTutorialVerification` first failed in 0.4 s because my nested X display had died; both PASS on re-run. Evidence: `evidence/regression/`.

## 2B — CONTROLLED GROWTH FIXTURES

Fixture: a cleared Enlarged80 stand, real juveniles planted through the manager (purchase, exact planting, approve, `AdvanceYear`), browsing set to 0 and natural recruits removed each year (fixture controls, not game behaviour). Three 5 × 5 single-species grids per run: **dense** (2 m spacing, competition index ≈ 7.0–7.9), **moderately released** (4 m, CI ≈ 2.2–2.3), **strongly released** (6 m, CI ≈ 0.67). The numbers are the centre tree. Each run is 125 years under the default model; models 0 and 2 were also run for 60 years and give **identical rows** (no density mortality acts on a pure Oak or Beech stand; 0 deaths).

**Kernel check:** before every annual step the competition index actually acting on each tree was read back, and the documented law (`DBH: Pd × site × (1 − D/Dmax) / (1 + CI/Ci50)`, `height: Ph × site × (1 − H/Hmax)`) was recomputed in the same float arithmetic. Maximum difference over 8 700 tree-years per species: **0.0 cm and 0.0 m** (exact), in every regime, model and through the release step. So the shipped parameters are the ones applied and the controller does what its comment says.

Default growth model, centre tree (full tables with 5-year increments, CI and DBH/H ratio for all three regimes and models: `evidence/trajectory-tables.md`, raw CSV `evidence/rows-trajectories.csv`):

| species | regime | age 10 | 20 | 30 | 40 | 50 | 75 | 100 | 125 |
|---|---|---|---|---|---|---|---|---|---|
| Sessile Oak | dense, DBH cm / H m | 5.95 / 4.34 | 9.04 / 9.79 | 11.98 / 14.46 | 14.81 / 18.47 | 17.54 / 21.90 | 24.01 / 28.49 | 30.03 / 32.97 | 35.66 / 36.02 |
| | moderate | 6.15 | 11.29 | 16.18 | 20.84 | 25.28 | 35.55 | 44.70 | 52.86 |
| | strong | 6.30 | 12.81 | 18.95 | 24.74 | 30.20 | 42.51 | 53.14 | 62.31 |
| European Beech | dense | 6.04 / 4.43 | 10.05 / 10.64 | 13.84 / 15.84 | 17.47 / 20.19 | 20.95 / 23.83 | 29.12 / 30.53 | 36.59 / 34.83 | 43.45 / 37.59 |
| | moderate | 6.27 | 12.46 | 18.29 | 23.81 | 29.03 | 40.86 | 51.16 | 60.12 |
| | strong | 6.40 | 13.90 | 20.90 | 27.44 | 33.54 | 47.11 | 58.54 | 68.18 |

(The moderate and strong rows give DBH only; height is identical in every regime, see below.) Periodic DBH increment, Oak: 0.31 → 0.22 cm/y (dense), 0.51 → 0.31 (moderate), 0.64 → 0.35 (strong) over ages 20–125; Beech 0.39 → 0.27, 0.61 → 0.34, 0.74 → 0.36.

Required qualitative results, each a hard check in the gate:
- **Positive DBH response to release**: strong > moderate > dense at every checkpoint from age 20 on, for both species (e.g. Oak at 100: 53.1 / 44.7 / 30.0 cm).
- **No artificial release multiplier on height**: height identical across regimes to 1e-3 m at every checkpoint (Oak 32.97 m, Beech 34.83 m at 100).
- **No pathological early saturation**: at 50 y DBH ≤ 33.5 of 120 cm and height ≤ 23.8 of 42.5 m in every regime; monotone and within Dmax/Hmax everywhere.
- **Young cohorts stay young and small**: at 25 y DBH is 10.5–15.9 cm (Oak) and 12.0–17.5 cm (Beech), all below 20 cm.
- **Oak and Beech do not behave identically**: Beech is larger at every age and regime (dense, age 100: 36.6 vs 30.0 cm, 34.8 vs 33.0 m), as the rows (Pd 0.9 vs 0.8, Ph 0.75 vs 0.65, Ci50 6.5 vs 5) imply.
- **Promotion**: sizes are continuous. Planted stock becomes a tree in year **9** at DBH 5.625 cm and height 3.75 m, because the shipped planting starts at a higher stock height than the lab's 0.15 m fixture (the lab says age 10, 5.475 cm, 3.65 m). The height **rate** steps once at promotion from the juvenile 0.35 m/y to the tree law: 0.58 m/y for Oak (×1.67) and 0.67 m/y for Beech (×1.92). The pre-calibration rows gave 0.40 m/y (×1.14). So the calibration makes the existing kink at promotion visible. I count it a bounded caveat, not a discontinuity in size.

Size thresholds, for the Manager's economics question (the lab's "D60" Oak proxy; veneer mid-log 45 cm): Oak reaches DBH 45 cm at age 81 only under strong release (101 moderate, never by 125 dense) and DBH 60 cm at age **119** (strong release only). Beech reaches 45 cm at 71 / 85 and 60 cm at 104 / 125 (strong / moderate). No planted cohort in the real stand gets near either size (2C).

## 2C — SCENARIO ONE COHORT CHECK

Plan A as in the U4 packet (27 % basal-area thinning plus four sheltered/unsheltered pairs each of Oak and Beech, one planting session, resolves in year 1), Enlarged80, default model 1, normal browsing (0.2) and shelters. Measured +10 to +50 years after planting; the same play with the pre-calibration rows applied in memory is alongside. Growth model 2 gives almost the same numbers (`evidence/evidence-cohort-m2.txt`).

| after planting | Oak promoted (of 8), DBH / H, new | Oak, pre-calibration | Beech promoted (of 8), DBH / H, new | Beech, pre-calibration |
|---|---|---|---|---|
| +10 y | 8: 5.9 cm / 4.7 m | 8: 5.7 / 4.3 | 4 (+4 juveniles): 6.0 / 5.1 | 4 (+4): 5.7 / 4.4 |
| +20 y | 6: 7.7 / 10.1 | 6: 6.3 / 7.8 | 6: 7.5 / 9.4 | 6: 5.9 / 6.8 |
| +30 y | 2: 9.6 / 14.9 | 1: 6.9 / 10.9 | 2: 11.6 / 16.3 | 1: 6.8 / 10.9 |
| +40 y | 0 | 0 | 1: 15.3 / 20.6 | 0 |
| +50 y | 0 | 0 | 1: 19.9 / 24.2 | 0 |

- **No planted Oak or Beech approaches the premium size** (largest living planted tree at +50: Beech 19.9 cm; Oak none). The lab's Oak D60 proxy is not reached.
- **Growth is plausible under real competition**: mean competition index 10–19 for the cohort up to +30 y, positive increments, all sizes finite and inside Dmax/Hmax, no zero or explosive values. The calibration raises size at a given age by about 25 % at +20 y and extends survival slightly (+30 y: 2 against 1).
- **Observation, not a calibration finding:** planted Oak and Beech die out under the retained Sitka canopy (Oak 0 of 8 alive at +40 y, Beech 1 of 8), with the old rows as well. A scenario whose objectives need a planted broadleaf still present at completion is therefore sensitive to *when* it completes. That matters for U4, which pushes completion later.
- **Beech does not inherit Oak economics**: `BroadleafTimberEconomyVerification` PASS (Beech has only second-quality planking and firewood; no veneer, beam or fencing).

## 2D — RELEASE RESPONSE

Three identical dense stands per species, matched to ≤1e-4 at year 50 (CI 7.7 Oak / 7.65 Beech). At year 50 the Work Plan fells the nearest neighbours of one stand's centre tree (8 trees, moderate) and all neighbours of another (24, strong); the third is the suppressed control. All three centre trees stay alive to year 100.

| | year 55 | year 75 | year 100 | increment 50→51 |
|---|---|---|---|---|
| Oak DBH strong / moderate / suppressed (cm) | 20.9 / 19.4 / 18.9 | 33.3 / 26.5 / 24.0 | **46.7 / 34.7 / 30.0** | 0.683 vs 0.268 cm |
| Beech DBH strong / moderate / suppressed | 24.6 / 23.2 / 22.6 | 37.9 / 31.8 / 29.1 | **52.0 / 41.4 / 36.6** | 0.743 vs 0.341 cm |

Released DBH trajectory > suppressed in every checked year, for both species and both release strengths, as preregistered. Height is **identical** across the three (Oak 32.968 m, Beech 34.833 m at 100), so no height response to release was invented. The strongly released step grows with CI = 0 exactly as the law says.

## 2E — SAVE / LOAD

- Real `Save()`/`Load()`: species rows resolve identically after load (`Pd 0.8 …` and `0.9 …`), the cohort origin survives (16 saplings, planting year 1, ages and promoted tree IDs), and the trajectory **continues deterministically**: the year-30 world hash after a load of the year-20 file equals the uninterrupted run (`0DE1095C98BC24ED`), both in-process and **across a process relaunch** (a second Editor loaded the file written by the first).
- No migration: saved `version` is 20 and the JSON contains no species coefficient (`potentialDbhGrowthCmPerYear`, `maxDbhCm`, `ci50` absent).
- **Behaviour change on resume** (documented by the author, confirmed here): a save made under the old rows and loaded under the new rows continues with the new rows. Its year-30 hash differs from both the old-rows and the new-rows uninterrupted runs, as it must.

## EXISTING GATES

| gate | head | result |
|---|---|---|
| `BroadleafTimberEconomyVerification` | this | PASS |
| `MixedDensityVerification`, Legacy40 | this | PASS (22 checks) |
| `MixedDensityVerification`, Enlarged80 | this | FAIL on two assertions that expect Legacy40 anchors (`7E57…`, `35E2…`): it prints the Enlarged80 values `1FE6639780636394` / `702DE05F33384139`, the same ones recorded in the density v2 packet. Not caused by the calibration |
| `BroadleafAdultGrowthVerification`, both geometries | this | **FAIL, and fails identically with the pre-calibration rows applied in memory.** The gate (written in a cloud session, never run before) does not remove natural recruits: between years 43 and 50 recruits (12 to 28 of them, depending on the rows) join its nine-tree stand, so "sparse stand lost a tree" and the probe-tree law check break. This is a harness defect, not a calibration effect. My own gate (`OakBeechQualification`) removes recruits and covers the same law. The density-v3 session's summary shows this gate exiting 0 on its own branch; I did not investigate that difference |
| offline: N3 67, density 35, timber 71, history 70, UX 176, growth lab 360 rows | merged state | PASS (plain .NET / Python, not Unity) |

## LIMITS

- One seed per fixture; Enlarged80 default for the Unity runs, Legacy40 for the historical gates.
- The grid fixtures use same-species neighbours and zero browsing; the cohort run uses the real stand and browsing. Neither claims empirical agreement. I compared nothing with measured Irish growth data and did not try to.
- P2/P3 interactive anchors (`F58FB0B1A421D28B`, `F7C2FC966816BBAD`) and the Model2 start/year-1 anchors were not observed (see `evidence/phase0-anchors.md`; the Model2 and storm anchor re-runs are recorded in the final section if they completed).
- The promotion-year and lab-comparison checks in my gate were first derived from the lab's 0.15 m assumption; they failed and were corrected to use the runtime juvenile law. The first runs are kept in `evidence/superseded/`.
- Interactive regression gates need a display; my nested display died once and I re-ran those gates.

## DECISIONS NEEDED (Manager)

1. Accept the two intentional anchor changes above (completion `702766DECE591E21` → `D99EC9FC2C635064`, Reference continuation `9CDF21A541C5968D` → `BFFF66015A6619DA`) and record them, or stop the calibration. I rebaselined nothing.
2. Whether the promotion kink (juvenile 0.35 m/y to 0.58 / 0.67 m/y) is acceptable, or the juvenile rate should be revisited. It is not a calibration parameter in this packet.
3. Whether existing Oak/Beech saves picking up the new rows on resume is acceptable (no migration exists).
4. Fix or retire `BroadleafAdultGrowthVerification` (recruit removal), and give `MixedDensityVerification` an Enlarged80 expectation.

## EVIDENCE

`evidence/`: `evidence-rows.txt` (154 hard checks), `evidence-release.txt`, `evidence-cohort*.txt` (14 hard checks each), `evidence-save1/2.txt`, `trajectory-tables.md` and `rows-trajectories.csv`, `phase0-anchors.md`, `results-*.json` (exit code, run head, staged-gate SHA, working-tree changes) and `*.markers.txt` per run, `regression/` (24-gate results and the markers of the three non-PASS gates), `ab-old-rows/` (the in-memory old-rows A/B runs, including the broadleaf-growth gate debug runs), `superseded/` (first runs of the three phases whose gate expectations I corrected).
