# MULTI Phase 2 — Sessile Oak and European Beech engine path: independent review (P2-11)

**Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), independent of the implementer (commits `cc67361f` and `401dab18` are authored by "Claude Opus 5.5 (Phase 2 Step B)"). I did not write or edit any reviewed code, and I did not commit to the feature branch. This file is the only thing I publish.
**Scope:** `d1db81b5..401dab18` (12 files, `+3563 / −32`), read against the approved spec quartet at `8cec7c4d`. The Unity files the spec cites were read at the same tree.
**Method:** I rebuilt clean and ran the suite twice myself, re-derived one density quantity from the spec formulas, ran throw-away probes and **36 single-site mutations** on `/tmp` copies, and checked the implementer's six self-resolved ambiguities against the Unity source. Nothing I ran touched the feature worktree, the parent checkout or any branch.

```
INTEGRATION DECISION
Status: APPROVED
        (the engine code, the Core tests and the numerical qualification of P2-01 … P2-10 as worded.
         §9 lists owner decisions and process conditions. None is a code repair to the reviewed head,
         and none is blocking. Two of them (F1, F9) I would want settled before the owner calls Phase 2 "qualified".)
Gate: G2 (acceptance): PASS
        P2-01 … P2-10 each hold as worded (§10). Build 0 warnings / 0 errors, suite 242 / 242 twice,
        all six frozen anchors reproduced, Q6 equivalence identical, density recompute exact on 40 / 40 rack-years.
        Evidence caveats, not failures: P2-06 is demonstrated at the p_veg level only, never at the survival level (F1);
        the plant → promote lifecycle is never exercised end to end (F3).
Gate: G3 (measurement integrity): PASS
        item 1 only existing-test edits ... legitimate (Stage5Tests.cs: +1 constant, +1 assertion, FullRun overload; Program.cs +1 line)
        item 2 one oracle changed ......... legitimate (V(h) taper width is Unity's float expression 1.8f − 1.2f)
        item 3 tolerances ................. legitimate (1e-6 default, 1e-5 at five sites; none fitted to a residual)
        item 4 test-only seam in production  acceptable, inert, limited reach (F6)
        item 5 fixture rule ............... legitimate (Q5 share exact; "fixed before measurement" is claimed, not provable, §8)
        item 6 "first-run failures fixed without loosening" ... not provable from history (one test commit); final oracles show none
Reviewed branch: feat/multi-phase2-oak-beech-20261124-v1
Reviewed HEAD: 401dab1835128f70d1ecc28f54ae5db8ac68bf69
Spec: specs/multi-phase2-oak-beech-v1 @ 8cec7c4d7d1882056477c18ed9ca7a3de3fdd5b9 (APPROVED, amended). It is NOT on the reviewed branch (§1).
Spec conformance: PASS
Reasons: F1 … F12 below (no blocking finding)
Integration target: none — no merge authority
```

---

## 1. Topology: spec and implementation live on separate branches

The feature branch was cut from `d1db81b5` and therefore contains **no copy of the spec quartet**. I read the spec from `origin/docs/multi-phase2-oak-beech-spec-20261124-v1` @ `8cec7c4d`. My *local* ref of that branch name was stale at the DRAFT commit `3a1eca6c`, so I used the remote tip, which GitHub confirms live.

| Fact | Result |
|---|---|
| Common ancestor | `d1db81b5` (Stage 5 documentation tip) |
| Files changed on the docs branch | `specs/multi-phase2-oak-beech-v1/**` (4) and `Docs/Handoffs/…/phase2-oak-beech/PHASE2-PREFLIGHT-AUDIT-20261124.md` |
| Files changed on the feature branch | 12 files, all under `Tools/MultiRackSitkaP1Lab/` |
| Overlap | **None**. `git merge-tree --write-tree` of the two heads is clean (tree `86b6dd61…`) |
| Implementation handoff | **None** on any remote ref. The remote has the docs branch, the feature branch and the DRAFT tag, nothing else for Phase 2 (F9) |
| Spec checklist | Still unticked and still says "Implementation under test: TBD", so the spec does not yet know the reviewed head |

Integrating the feature branch alone would leave no spec in the integrated tree (F8).

## 2. What I ran

All commands ran in WSL (`~/.dotnet/dotnet` 8.0.134) with `TMPDIR` outside any repository.

| Check | Result |
|---|---|
| Live refs | feature `401dab18…` ✓ (the reviewed SHA); docs `8cec7c4d…` ✓ |
| Method correction (disclosed) | My first build used a `git archive` copy. It built, but 7 + 1 tests failed because the P0 *source-trust* tests call `git cat-file` / `git rev-parse HEAD:Assets/…` and read `Tools/MultiRackSitkaLab/fixtures/…` and `Assets/ForestPrototype/Species/SitkaSpruce.asset`, none of which a bare archive has. That was my environment, not the feature. I rebuilt in an isolated sparse clone at `401dab18` that includes those paths |
| Clean rebuild, `--no-incremental`, isolated clone | **0 Warning(s), 0 Error(s)**, exit 0 (an earlier `git archive` build was also 0 / 0) |
| `.p0ref/SitkaLab.Core.dll` sha256 | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` ✓ (the P0 pin; also printed by P2-01) |
| Suite run A | **242 / 242 passed, 0 failed**, exit 0, 43 s |
| Suite run B | **242 / 242 passed, 0 failed**, exit 0, 42 s |
| Run-to-run difference | none: the `PASS`/`FAIL` line sets are identical |
| Composition | 33 Phase 2 tests + 209 pre-existing (the Stage 5 review recorded 209 at `5b757d9b`; `d1db81b5` is docs-only on top) |
| Mutation control `m00` (harness, unmodified tree) | 242 / 242 and the identical 30-year fingerprint, so the harness is sound |

Note: the task-runner's "completed (exit code 0)" notifications report the wrapper script, not the test. I used the recorded `TEST_EXIT_CODE`.

## 3. Frozen anchors, reproduced independently

I compared the values the suite prints against the values in the task, not against the constants in the code.

| Anchor | Expected | Observed |
|---|---|---|
| Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | ✓ identical |
| `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | ✓ identical |
| `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | ✓ identical |
| `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | ✓ identical (also reproduced inside the `1/1/1/1` run) |
| `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | ✓ identical, **now pinned** as `PinnedStage5FullRunHash`; the `1/1/1/1/0` five-field form gives the same value |
| P0 Core pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | ✓ identical |
| Q6: `1/1/1/1/1` on unmodified all-Sitka MA-1 vs `1/1/1/1/0` | equal | ✓ both `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` (trees, bands, accounts, blocks, deadwood, over 30 years with thinning and a clear) |
| Phase 2 30-year fingerprint (printed, not an anchor) | n/a | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f`, stable across both suite runs and the mutation control |

Token (Q1): the species version appears only when `!= 0` (`…+regeneration-v1+species-v1`); the four earlier tokens and the manifest hash are unchanged.

## 4. Independent density recompute (brief item 4)

I wrote a separate probe that recomputes, from the spec §5.1 formulas only, every mixed rack-year of a fresh 30-year `MA-1-MIXED-v1` session and compares it with the engine's `Phase2DensityRecord` and the actual deaths. It uses weights 2.0 / 1.5 / 1.0, normaliser 55, `G(O) = 0.5·(1 − exp(−1.60·max(0, O−1)²))`, `S = CI/(CI+5)`, weight `S⁴`, the exponential race on `ADULT-MORT-GENERIC-{treeId}`, the quota roll on `ADULT-MORT-GENERIC-QUOTA-{areaId}`, and the `0.125 × stand basal area` budget. The CI is the audit row's `PreGrowthCi`, the DBH is the post-growth `After.DbhCm`.

| | Result |
|---|---|
| Mixed rack-years recomputed | **40** (MA-B, years 1–30 where mixed) |
| Eligible set, quota, **exact generic victim set** | **40 / 40 match, 0 mismatches** |
| Max relative deviation | occupancy `5.4e-16`, G(O) `5.5e-15`, nonReferenceShare `2.9e-16`, expected count `5.5e-15`, stand basal area `0` |
| Quota roll drawn (fractional remainder) | 40 / 40 rack-years |
| Basal-area budget passed a stem over | **0** of 40 |
| One-stem fallback | **0** of 40 |

Worked example (year 3, MA-B, 72 stems, 0.15 ha): `O` = 1.1708 (mine and code), `G(O)` = 0.0228, `nonReferenceShare` = 0.7787, eligible 72, expected = 0.0228 × 0.7787 × 72 = **1.2791**, quota **2**, stand BA 6.4046 m², budget `0.125 ×` = 0.8006 m², generic BA removed 0.0789 m², 2 victims. All equal the code's.

So the arithmetic is right. What the 30-year run does **not** show is the budget binding or the fallback firing; those are exercised only by the hand-built cases in `M-05` (F7).

## 5. The eight scrutiny items

### 5.1 Non-vacuity of P2-06 (brief item 1)

**The answer, in three parts.**

1. **The committed test does show that the patch changes `p_veg`.** At the shared point, year 1, `p_veg` is **0.0000** (spot), **0.0351** (unweeded) and **0.0127** (whole-cell ClearUndergrowth). `S-01` asserts the spot value is exactly 0 and the unweeded value is positive (`Phase2Tests.cs:775–776`) and checks every later year against the formula `0.35·exposure·clamp01((1.5−h)/1.2)`. Mutations that break the patch itself are caught (m15 year-guard, m16 patch ignored, m17 radius).
2. **It does not show that the patch changes survival, and the stem dies regardless.** In its own scenario (cell light 0.113) all three planted Oaks die in **year 1 from *light*** (`cause=light`), on the same keyed roll, because Oak's light survival at 0.113 is 0.790 and the roll exceeds it. The printed `planted Oak alive: spot False, unweeded False, whole-cell False` is therefore uninformative about vegetation. **Mutation m18 makes `p_veg` unable ever to kill a planted juvenile (p_veg still computed and stored) and all 242 tests still pass.** The only vegetation assertions are on the stored probability.
3. **The engine does deliver the effect.** A paired probe plants 800 Oaks on two empty racks at fixed light with identical keyed rolls across arms (12 years, browse pressure 0):

| Light | Unweeded alive | Spot patch alive | Whole-cell clear alive | Vegetation deaths (unweeded / spot / whole-cell) |
|---|---|---|---|---|
| 1.0 | 531 / 800 (66 %) | **756 / 800 (95 %)** | 690 / 800 (86 %) | 269 / 44 / 110 |
| 0.5 | 650 (81 %) | **771 (96 %)** | 743 (93 %) | 150 / 29 / 57 |
| 0.3 | 654 (82 %) | **757 (95 %)** | 731 (91 %) | 146 / 43 / 69 |

My hand estimate at light 1.0 (year-1 guard, then 0.30 gain toward T = 0.8) is 0.66 / 0.94 / 0.85, which the probe matches. In a real managed gap on the shipped fixture (34 Sitka felled, cell light 1.0), an Oak with **no** aids dies of vegetation in year 2, Shelter alone also dies in year 2, and **SpotClear alone rescues it** (promoted in year 9), as does SpotClear + Shelter.

**So the patch can be shown to help, but not by the committed test.** The criterion as worded ("changes the planted stem's `p_veg` accordingly") is met. I record the survival gap as F1, an owner decision.

### 5.2 Reachability of planting (brief item 2)

- **P2-07's promotion tests are seeded-state tests**, not lifecycle tests. `P-01` uses `FixedLight = 1` and a juvenile created at 3.4 m, age 5; `P-02` uses a juvenile at 3.6 m, age 9, at light 0.19 / 0.2 / 0.05. The nursery default (0.6 m, age 3) is never carried to 3.5 m by any committed test. That is within the criterion's wording ("A planted Oak/Beech *reaching* H ≥ 3.5 m …") but it is contrived.
- **`P2-10`'s four plantings all die of light** because the recipe picks `FindPoint(c, "MA-A", 1.0f, …)`, the first valid point of a row-major scan, in the pure-Sitka rack. Area A's median cell light is **0.005** at year 0. **1 of 144** cells reaches the Oak gate 0.2 and **none** reaches 0.5 at year 0; at year 10 it is 9 cells at 0.2 and still none at 0.5. `PJ0001–0004` are `dead/light`.
- **Plant / Shelter do have viable use in the shipped fixture.** Area B (the mixed rack) has median light 0.107, **51 of 239 cells ≥ 0.2** and **11 ≥ 0.5** at year 0 (up to 26 cells ≥ 0.5 at year 10). Planting Oak or Beech with Shelter + SpotClear in the three brightest cells (light ≈ 0.76) promotes to `PL-PJ0001` in **year 9** every time, via the full fenced lifecycle (`h` 0.95, 1.30, 1.65, 2.00, 2.35, 2.70, 3.05, 3.40, 3.75). So the end-to-end path works and takes 9 years at full growth. No committed test shows it (F3).

### 5.3 The `[ThreadStatic] DrawLog` seam (brief item 3)

- **Acceptable.** It is `internal`, thread-local, null by default, and the production cost is one null check (`Phase2Rules.cs:168`, `:173`). It is observation only: it cannot change a roll, so it cannot change a result. It is labelled test-only in its comment.
- **Truly inert in normal use:** a grep over the whole lab shows it is only *read* at `Phase2Rules.cs:168` and only *set* by the test at `Phase2Tests.cs:223` and `:226`, which also resets it to null in a `finally`; `Roll` is stateless; `[ThreadStatic]` has no initialiser (the usual trap).
- **Limited reach, so it cannot be the only guard.** It sees only draws routed through `Phase2Random.Roll`. Mutation m26 (a pure rack draws a generic key through `Phase2Random`) is caught by `P2-03 M-01`; **m27 (the same draw made directly through `SimulationRandom.Roll`) is not caught**, because the seam cannot see it. Since keyed rolls are stateless such a draw is harmless to results, but it means `M-01`'s "draws no generic key" clause is evidence about the routed path only. I do not think it can mask a defect that matters, because the same test also requires the Area A report, victims and rolls to equal the Stage 4 step bit for bit (F6).

### 5.4 The six ambiguities the implementer resolved (brief item 4)

Each judged against the spec and the Unity source it claims to port, not against convenience.

| # | Ambiguity | Implementation | Source check | My judgement |
|---|---|---|---|---|
| 1 | Generic occupancy, share and basal area over **all** living post-growth stems; only the eligible set drops this year's Sitka victims. Spec §5.1: "on the stems still standing after step 1" | `Phase2MixedDensity.cs:196–208` | Unity `PlanComponents` sums `occupied`, `nonReference` and `standBasalArea` over **every** stem (`MixedDensityModel.Composite.cs:150–157`, `:205–207`) and uses `taken[]` only for eligibility (`:166`, `:182`). §5 states the port | **Unity-faithful, so right.** The spec phrase attaches naturally to the *eligible* set and `fraction · eligible`, and the spec's own provenance is "ported from `PlanComponents`". Effect: the alternative reading would have changed **0 of 40** quotas on the same states (max Δexpected 0.0036 stems). `M-03` case 2 encodes the chosen reading; mutation m07 is caught |
| 2 | Both mortality components use the rack's own Hegyi index over **every** living stem | `Phase2MixedDensity.cs:66`, `:125–127` | Unity passes the stand-wide `competitionIndex` into `DensityStem` (`ForestEcologyController.cs:1180–1187`) and the Sitka signal consumes it as given (`SitkaSubsetDensitySignal.cs:65`) | **Unity-faithful.** But **no test pins it**: `M-02` deliberately puts Oak > 8 m from every Sitka stem so the choice cannot matter, and mutation m10 (CI over Sitka only) passes 242 / 242 while changing the 30-year fingerprint (F5) |
| 3 | A new clearance patch's guard year, the shelter `InstalledYear` and the `PlantingYear` are Y+1, so a patch stays 0 through the first annual step | `Phase2Planting.cs:125`, `:146`; `Phase2Annual.cs:152` | Unity creates the patch with `competitionUpdatedYear = year` (`ScenarioOneManager.cs:1935`), skips it while `year <= competitionUpdatedYear` (`ScenarioOneCompetition.cs:32`), and sets `installedYear = report.year` (`:1912`) | **Faithful.** It also gives a shelter its full 8 annual steps (Y+1 … Y+8). The spec sentence "holds 0 until its first annual step" is ambiguous; the engine and `S-01` read it as "through". Worth one sentence in the spec |
| 4 | New refusal `Phase2PointWithoutGroundCell` for a point inside the polygon but in no in-polygon 2.5 m cell | `Phase2Planting.cs:54–56` | Not in spec §7.2. Reachable: **3.07 % (Area A) and 3.42 % (Area B)** of strictly-inside 0.25 m lattice points have no in-polygon cell. **No test** | **Protective, unlisted, untested.** Without it `AdvancePlantedJuveniles` would silently `continue` (`Phase2Annual.cs:497`) and the juvenile would never age, grow or die. But it narrows the valid-point domain by about 3 % beyond what §7.2 says. **Owner decision (F2)** |
| 5 | Shelter target: nearest active juvenile, then lowest ID | `Phase2Planting.cs:142–144` | Two active juveniles within 0.25 m of one point would need to be < 0.5 m apart, which §7.2 forbids. They can tie only at exactly 0.5 m with the point at the exact midpoint | **Harmless, effectively unreachable** |
| 6 | Broadleaves ignore the Sitka-only `GrowthAgePhases` seam | `Phase2Rules.cs:16–26` | Unity applies phases only `if (treeSpecies.HasGrowthAgePhases)` (`ForestEcologyController.cs:1024`); neither `SessileOak.asset` nor `BeechSpecies.asset` authors any | **Correct today.** Latent only if phases are ever authored for them (nit F11) |

### 5.5 Did five tests "fail on first run and get fixed without loosening any criterion"? (brief item 5)

I **cannot verify the history**: the tests are one commit, so I never saw the failing first run. I can verify the outcome. I read every final oracle and tolerance against the spec formulas and found **no weakened criterion**.

- **The one oracle change is legitimate.** `P2-05 B-01` computes `V(h)` as `1f − (h − 1.2f) / (1.8f − 1.2f)` (`Phase2Tests.cs:598`). Unity computes the taper width as `escape − full` in float (`JuvenileEcologyRules.cs:59–65`, with `escape = Mathf.Max(escape, full + 0.01f)` at `TreeSpeciesDefinition.cs:170`), and `1.8f − 1.2f` is not the literal `0.6f` (about 1 ulp). The test compares **bit patterns**, so the oracle has to be Unity's expression. The production code and the oracle now use the same float expression. This makes the oracle *stricter* than a tolerance would, and it matches Unity.
- **Tolerances:** `Close` defaults to `1e-6` relative; five sites use `1e-5` (V(1.5), patch recovery ×2, p_veg ×2, seed arrival). All are float-precision comparisons, none fitted to a residual.
- **No `Skip`, no commented-out assertions**, and `git diff --check` is clean.

### 5.6 The only edit to an existing test file (brief item 6)

Not quite "only": **two** existing test files changed.

- `Stage5Tests.cs` has exactly **three hunks**: `+PinnedStage5FullRunHash` (6 lines), `+T.Eq(PinnedStage5FullRunHash, first, …)` (1 line), and `FullRun()` split into an `internal FullRun()` plus `FullRun(MultiAreaContinuation c)` (4 added, 2 removed: the signature and `var c = Session();`). `numstat` is `11 / 2`. Nothing else moved.
- `Program.cs` gained **one line**: `Phase2Tests.Register(manifestPath);`.

No existing assertion was removed or weakened.

### 5.7 Species imbalance (brief item 7)

The imbalance is **real, follows from the ratified tables, and is not a defect.** I checked the arithmetic instead of attributing it to calibration.

**`P2-09 K-01`: Beech 10 / Oak 1 / Sitka 1 promotions in 14 years.** Light is fixed at 0.35. `LR_growth(0.35)` is **1.000 for Beech** (a table point) and **0.700 for Oak** (interpolated between 0.3 → 0.6 and 0.4 → 0.8). Height gain per year is `0.35 · LR`: Beech 0.350, Oak 0.245. A recruit established in year *y* needs `⌈(3.5 − h₀)/gain⌉` growth steps:

- Beech from 0.15 m: `(3.5 − 0.15)/0.35 = 9.6` → **10 steps → promotes in year y + 10** (inside the window for year-1 recruits).
- Oak from 0.18 m: `(3.5 − 0.18)/0.245 = 13.6` → **14 steps → year y + 14 = 15 for the earliest recruits, past the 14-year window.**

So **no Oak recruit established in K-01 can promote inside the window**; the single Oak promotion is consistent with the pre-seeded 2 m band (`(3.5 − 2)/0.245 = 6.1` → 7 steps). Beech's ten are consistent with its recruits from year 11 on plus its own pre-seeded band (`(3.5 − 2)/0.35 = 4.3` → 5 steps). I did not trace each promotion individually. Within a block, ties go to oldest then tallest, so a faster-growing Beech cohort is also the taller one.

**30-year `MA-1-MIXED-v1` (no plantings, my probe):** initial Area B = 25 Oak / 24 Beech / 24 Sitka.

| | Beech | Oak | Sitka |
|---|---|---|---|
| Seed arrival (30 y) | 4585 | 2804 | 188135 |
| Establishment accepted | 39.3 | **58.2** | 121.5 |
| Natural promotions | **197** | 40 | 88 |
| Deadwood, genesis stems / recruits | 10 / 161 | 5 / 34 | 119 / 75 |
| Living adults at year 30 | 50 | 26 | 89 |

Oak *establishes more* than Beech (its distinct establishment table is ≥ 0.75 at any light) but **promotes about 5× less**, because its growth table is far less shade-tolerant: at Area B's median light 0.107, `LR_Oak = 0.081` against `LR_Beech = 0.371` (a 4.6× ratio, the observed promotion ratio is 4.9×). Oak also faces a 0.2 promotion light gate and a light survival of 0.77 at 0.107 where Beech's legacy rule gives 1.0. Beech disperses further (λ 6 vs 4 m: 1.4× more seed at 4 m, 2.3× at 10 m).

**Deadwood Beech 173 / Oak 37 / Sitka 191 (the cached run with plantings):** the no-planting run gives 171 / 39 / 194. Almost all of it is **small recruits** (mean DBH 6–7 cm); victim choice is species-neutral (`S = CI/(CI+5)`, weight `S⁴`), so deaths simply track how many small suppressed stems each species has. The growing-space weights 2.0 / 1.5 set **how many** stems die (through occupancy and `nonReferenceShare`) but not **which**; `Ci50 = 5` vs `6.5` is irrelevant to victim choice by design. Admission order (Beech first) is not the driver, because Oak's accepted establishment is the larger of the two.

I would not call this a calibration error: the Oak and Beech tables match the Unity assets field for field (§5.9). It is the plan's own foreseen risk ("Oak `PromotionMinimumLight = 0.2` may make Oak promotion rare… a calibration consequence, reported, not retuned"). The owner should know it exists (F10).

### 5.8 Scope (brief item 8)

| Check | Result |
|---|---|
| Files outside `Tools/MultiRackSitkaP1Lab/` | **none** (12 / 12 files inside) |
| `Assets/`, `.meta`, `ProjectSettings`, `wwwroot`, `/multi` HTML/JS/CSS, `Docs/`, `specs/`, `*.json` | **none** |
| Mode-only churn | **none** (`git diff --summary` shows only `create mode 100644` for the eight new files) |
| Line endings | all LF (`git ls-files --eol`) |
| `git diff --check` | clean |
| Stage 4 / Stage 5 / annual-rules / crown / canopy source files | **none changed**, except `Stage4Models.cs` (the selection record; 28 lines changed) and `MultiAreaContinuation.cs` (the orchestration; 228 lines changed) |
| Existing code that branches on the selection | uses `IsStage4` / `IsStage5` / `IsPhase2` predicates (checked every site), so `1/1/1/1/1` flows correctly and no equality comparison can mistake it for an unknown selection |
| Unity files cited by the spec | the 12 cited files and `Species/` are byte-identical between `5b757d9b` and the reviewed head |
| Save schema | untouched (state is session-only; no serialiser change) |

### 5.9 Constants, checked against the Unity assets (not part of the brief; I did it because no test does)

I compared every `Phase2Species` field with `SessileOak.asset` and `BeechSpecies.asset` at the reviewed tree: growth `Pd/Dmax/Ph/Hmax/Ci50`, crown (Oak linear 0.1 + 0.1045·DBH; Beech power law 0.05 / 1.01; Beech's serialised linear fields 0.9415 / 0.07635 are **not** stored), relaxation 0.12 / 0.15, maturity 40 / 60, `S50` 5, λ / cutoff 4 / 80 and 6 / 50, mast probabilities and multipliers (1 / 0.3 / 0.075 and 1 / 0.4 / 0.1), all three Oak light tables and Beech's shared table, regen initial height 0.18 / 0.15, `PromotionMinimumLight` 0.2 / default 0, palatability 1 / 0.5, browse heights 1.2 / 1.8, growing-space classes (Oak High 2.0, Beech Intermediate 1.5; Unity `ReferenceWeight / IntermediateWeight / HighWeight` = 1.0 / 1.5 / 2.0). **All match.** The shared Stage 5 constants the broadleaves reuse also match the spec (`SeedPotentialPerMatureTree` 1, `SeedCrownReferenceRadiusM` 4, `MinimumEstablishment` 0.01, `EstablishmentSuitability` 1, `BrowseProbabilityCap` 0.95, `BrowseHeightIncrementLoss` 0.9, `BrowseExtraMortality` 0.04; `Stage5Models.cs`). I also read `InterpolateResponse`, `Maturity`, `PotentialCrownRadiusM`, `DistinctJuvenileSurvivalResponse`, `JuvenileEstablishmentResponse`, `BrowseHeightVulnerability`, `SurvivalResponse(…browsedFraction)` and `GrowAdults` and found each ported with the same float operation order. Unity's band limit is `MaxBandsPerSpeciesOrigin = 4`, "per species + origin + cell" (`ForestEcologyCell.cs:133–134`), which is what the engine implements (per species in a cell).

## 6. Mutation checks (36 single-site mutations on `/tmp` copies)

Each is one exact edit, a clean rebuild and the **full** suite. "Caught" means at least one new failure against the 242 / 242 control. `fp` is the P2-10 30-year fingerprint relative to baseline; it includes the density diagnostics, so "changed" does not by itself prove the dynamics changed.

| Area | ID | Mutation | Result | Caught by |
|---|---|---|---|---|
| P2-02 adult | m01 | Beech growth Ci50 6.5 → 5 | **caught** | A-01, M-04 |
| | m02 | height growth wrongly competition-reduced | **caught** | A-01 |
| | m03 | C2 crown target uses Ci50 5 for every species | **caught** | A-02 |
| P2-03 density | m04 | no basal-area budget | **caught** | M-05 |
| | m05 | victim suppression Ci50 5 → 6.5 | **caught** | M-04 |
| | m06 | drop the `nonReferenceShare` factor | **caught** | M-03 |
| | m07 | ambiguity 1 alternative: occupancy/share/BA over survivors only | **caught** | M-03 |
| | m08 | pure-Sitka racks forced through the mixed code path | **caught** | M-01, M-08 |
| | m09 | victim weight `S⁴` → `S²` | **caught** | M-04 |
| | **m10** | **ambiguity 2 alternative: Hegyi CI over Sitka stems only** | **SURVIVED** (fp changed) | none |
| | **m29** | **Oak growing-space weight 2.0 → 1.0** | **SURVIVED** (fp changed) | none |
| | **m30** | **Beech growing-space weight 1.5 → 1.0** | **SURVIVED** (fp changed) | none |
| seam | m26 | pure rack draws a generic key through `Phase2Random` | **caught** | M-01 (via the seam) |
| | **m27** | **same draw made directly through `SimulationRandom`** | **SURVIVED** (fp same) | none (stateless, harmless) |
| P2-04/05 | m11 | shelter expiry `<` → `<=` | **caught** | B-02 |
| | m12 | planting spacing 0.5 → 0.4 | **caught** | C-02 |
| | m13 | adult spacing 0.7 → 0.5 | **caught** | C-02 |
| | m14 | planted juvenile ignores shelters | **caught** | B-01 |
| | m35 | cohorts get shelter access 0 | **caught** | B-03 |
| | m32 | Oak palatability 1.0 → 0.5 | **caught** | B-01, B-03 |
| | m38 | browsed planted juvenile loses no extra 4 % survival | **caught** | B-01 |
| P2-06 spot | m15 | patch year guard removed | **caught** | S-01 |
| | m16 | patch exposure ignored for planted juveniles | **caught** | S-01 |
| | m17 | spot radius 0.564 → 1.0 m | **caught** | S-01 |
| | **m18** | **`p_veg` can never kill a planted juvenile** | **SURVIVED** (fp same) | none |
| P2-07 | m19 | Oak promotion light gate removed | **caught** | P-02 |
| | m20 | recruit crown = open-grown `r_pot`, not C2 target | **caught** | P-01 |
| P2-08 | **m21** | **Oak seed cutoff 80 → 20 m** | **SURVIVED** (fp changed) | none |
| | **m22** | **all species share the `STAGE5-MAST` stream** | **SURVIVED** (fp changed) | none |
| | **m31** | **Oak survival-table entry 0.75 → 0.5** | **SURVIVED** (fp changed) | none |
| P2-09 | m23 | capacity per species instead of shared | **caught** | K-01 |
| | m24 | winner tie-break `SpeciesId` removed | **caught** | K-03 |
| | m36 | tallest before oldest | **caught** | K-03, M-08 |
| P2-10 | m37 | `InStateOrder` returns the caller's area order (real rack-order dependence) | **caught** | R-01, D-02 |
| P2-01 | m25 | Species 0 token appends `+species-v0` | **caught** | F-02, plus eight older tests (frozen paths) |
| (control) | m00 | none | 242 / 242 | n/a |
| (invalid) | m33 | only the 5-arg `Step` overload: **equivalent** (the 6-arg overload re-normalises). My mistake; replaced by m37 | n/a | n/a |

**Tally:** 36 mutations run, one of them (m33) equivalent and therefore void, leaving 35 valid: **27 caught, 8 survived**. Every mutation to a rule's *logic* is caught except m18 (and m27, which is unobservable by construction). The other six survivors change a *constant* or a *reading* that no test asserts. The survivors are of three kinds:

- **Constants and choices no test asserts** (m10, m21, m22, m29, m30, m31): the engine reads them and my §5.9 check shows today's values are right, but nothing would notice a drift. This includes two numbers P2-08 names outright: in `R-01` the oracle reads `species.SeedDispersalCutoffM` / `SeedDispersalScaleM` from the code under test, so "Oak 4/80 m, Beech 6/50 m" is never asserted (m21). `R-02` varies the stream **seeds**, so it cannot see a shared stream **key** (m22); I confirmed separately that the shipped code uses distinct streams at default seeds (0 of 12 years share a roll). No test mentions `GrowingSpaceWeight`, mast probabilities, `S50`, maturity or the survival/establishment tables.
- **A behaviour the committed scenarios never exercise** (m18).
- **A harmless unobservable** (m27).

These mutations show sensitivity to specific errors, not completeness.

## 7. G3: the changes I audited

| # | Change | Verdict | Evidence |
|---|---|---|---|
| 1 | `Stage5Tests.cs`: pin the `1/1/1/1` hash; `FullRun` overload | **Legitimate.** Declared in the plan (closes Stage 5 reviewer nit R8). The value is the one Stage 5 printed and I reproduced; no other hunk | §5.6 |
| 2 | `Program.cs`: register `Phase2Tests` | Legitimate | §5.6 |
| 3 | `V(h)` oracle uses `1.8f − 1.2f` | **Legitimate**, matches Unity | §5.5 |
| 4 | Tolerances `1e-6` / `1e-5` | Legitimate | §5.5 |
| 5 | Test-only `DrawLog` in production | Acceptable (F6) | §5.3 |
| 6 | `Phase2Options.GenesisSpecies` is a *production* option used only by the fixture | Acceptable; default empty; documented (F11) | `Phase2Models.cs:36–41` |
| 7 | Fixture `MA-1-MIXED-v1` | **Matches Q5 exactly**: Area A 100 % Sitka; Area B by ordinal `TreeId` index `i mod 3` → 0 Oak, 1 Beech, 2 Sitka; reassigned stems age 60; position, DBH and height unchanged (25 / 24 / 24 of 73). The rule is in `Phase2Support.MixedFixture`'s comment. "Fixed before any number was measured" is the implementer's claim; one commit cannot prove ordering | `Phase2Support.cs:18–44` |
| 8 | Declared test changes beyond the pin | none found | `git diff` |

## 8. Findings

| # | Severity | Finding | Evidence |
|---|---|---|---|
| F1 | **Owner decision** | **P2-06 is demonstrated at the `p_veg` level only.** The committed scenario's three planted Oaks all die in year 1 from *light* on one shared roll, so the printed `alive False / False / False` says nothing about vegetation, and a mutation that stops `p_veg` ever killing a planted stem passes all 242 tests (m18). The engine does raise survival 66 → 95 % at light 1.0 (§5.1), so this is a gap in evidence, not a bug. I recommend one paired test (it is test-only, about 40 lines, and my probe passes on the current code); whether the criterion needs it is the owner's call | `Phase2Tests.cs:713–790`, `:780`; m18; §5.1 |
| F2 | **Owner decision** | **Unlisted refusal `Phase2PointWithoutGroundCell`.** It protects against a juvenile that would otherwise be silently frozen, but it refuses about 3 % of points that §7.2 calls valid, and no test exercises it (or `Phase2PatchOutsideGround`, `Phase2LatticeChanged`). Ratify it (and add one test), or direct a different treatment | `Phase2Planting.cs:54–56`; `Phase2Annual.cs:497`; §5.4 row 4 |
| F3 | Low | **The plant → promote lifecycle is never exercised end to end.** `P2-07` is seeded-state; `P2-10`'s recipe plants all four juveniles in the dark pure-Sitka rack where they die of light in year 1. My probes show the path works on the shipped fixture (Area B's bright cells, a managed gap; promotion in year 9). Suggest the `P2-10` recipe plant in a cell with light ≥ 0.5 and assert a `PL-*` appears | `Phase2Tests.cs:1093–1101`; `Phase2Support.cs:91`; §5.2 |
| F4 | Low | **Constants and tables are unpinned.** Oak/Beech weights, mast probabilities and multipliers, `S50`, maturity, λ / cutoff and the survival/establishment tables are asserted nowhere, and `R-01`'s oracle reads its constants from the code under test (m10, m21, m22, m29, m30, m31 survive). Stage 5 pins its Sitka sources in `CcfSourcePins.cs`; the same treatment for `SessileOak.asset` and `BeechSpecies.asset`, plus a few literal-value assertions, would close it. All present values are correct (§5.9) | `CcfSourcePins.cs:14–25`; §6 |
| F5 | Low | **The Hegyi neighbourhood of the Sitka component is unpinned.** The engine follows Unity (stand-wide CI), but `M-02` is built so the choice cannot matter, and the alternative (m10) passes. A test with Oak inside the 8 m radius of a Sitka stem would pin it | `Phase2Tests.cs:244–250`; m10; §5.4 row 2 |
| F6 | Low | **`DrawLog` sees only routed draws** (m27 passes), so `M-01`'s "no generic key drawn" is evidence about `Phase2Random.Roll` only. Acceptable because keyed rolls are stateless and the same test requires bit-identical reports | `Phase2Rules.cs:168`, `:173`; §5.3 |
| F7 | Low | **Budget and fallback never fire in the integration run** (0 / 40 rack-years); only the hand cases bind them. The criterion "never exceeds 0.125 … except the documented fallback" is therefore vacuous at integration level and real at unit level | `Phase2Tests.cs:332–355`; §4 |
| F8 | Owner decision (process) | **Spec and implementation are on separate branches.** They merge cleanly and are disjoint, but the feature branch alone carries no spec, and the checklist still reads "Implementation under test: TBD" with every box unticked. The status words `IMPLEMENTED` ≠ `NUMERICALLY QUALIFIED` ≠ `REVIEWED` need updating on the integrated tree | §1 |
| F9 | Owner decision (process) | **No GitHub-tracked implementation handoff exists.** `AGENTS.md` requires one; checklist items **H4, H5** and **G05**, and the plan's "Stage 1 confirmations recorded in handoff", cannot be checked. I re-derived four of the five Stage 1 confirmations from Unity myself (§5.4, §5.9); the RNG call shape I only read | remote refs (§1) |
| F10 | Owner decision (acknowledge) | **Oak recruits and promotes about 5× less than Beech** under the ratified Unity tables (K-01 10 / 1 / 1; 30-year 197 vs 40). It follows from `LR_growth`, the 0.2 gate, light survival and the dispersal kernel (§5.7). No change proposed; the plan said to report, not retune | §5.7 |
| F11 | Nit | `GenesisSpecies` is a production option used only by the fixture; broadleaves ignore `GrowthAgePhases` (faithful today). Both are documented | `Phase2Models.cs:36–41`; `Phase2Rules.cs:16–26` |
| F12 | Nit | `IsStage5` is now also true for `1/1/1/1/1` (intended). I checked every call site uses predicates; none compares selections by equality | `Stage4Models.cs:76`, `:80` |

**Blocking findings: none.** I looked for each of these and did not find it: a weakened or removed assertion; a changed frozen constant; a hash that moved at `Species = Off`; an RNG key drawn at `Species = Off`; a Unity port that differs in operation order; an out-of-scope file; a test that passes only because of a tolerance.

## 9. Owner-decision items (decisions, not code repairs; I have made none of them)

1. **F1:** accept the `p_veg`-level evidence for P2-06 as the criterion is worded, or require one paired survival test before calling it qualified. My recommendation is to add it.
2. **F2:** ratify `Phase2PointWithoutGroundCell` as a documented refusal (and test it), or choose another treatment for points in boundary slivers.
3. **F8, F9:** decide how the spec branch and the feature branch integrate, and require the GitHub-tracked implementation handoff (H4, H5, G05) before status words are updated.
4. **F10:** acknowledge that Oak is a rare recruit and promoter under the ratified tables, so no one reads the 197-vs-40 split as a bug later.
5. **Optional, test-only (F3–F5, F7):** a lifecycle test in a bright cell; asset pins and literal-value assertions for the Oak/Beech constants; a Sitka-with-nearby-Oak case for the Hegyi neighbourhood. None changes production code.

## 10. Per-criterion table

| ID | Verdict | Basis |
|---|---|---|
| **P2-01** Frozen paths | **PASS** | Clean build 0 / 0; 242 / 242 twice; all six anchors reproduced against the task's values; `1/1/1/1` now pinned; five tuples accepted and every other `Species = 1` combination is `SpeciesModelIncomplete` (32-combination sweep); tokens unchanged; m25 caught by 9 tests |
| **P2-02** Broadleaf adults | **PASS** | Oak / Beech increments equal §3.1 on 5 hand cases (open, competed, near `Dmax` / `Hmax`); Beech `Ci50` 6.5 asserted against 5; `r_pot` both forms; Beech's linear fields poisoned and never read; C2 target and relaxation; no light transmissivity term. m01–m03 caught. Constants verified against the assets (§5.9) |
| **P2-03.1** Pure Sitka rack inside a mixed woodland | **PASS** | Area A report equals the Stage 4 step bit for bit for 6 years; no generic key through the seam (F6) |
| **P2-03.2** Sitka component on the Sitka subset | **PASS** | Victims and rolls equal the Comeau result on the subset (F5 on the CI neighbourhood) |
| **P2-03.3** Occupancy, G(O), share, quota, remainder | **PASS** | Hand cases **and my independent recompute on 40 / 40 rack-years** (§4) |
| **P2-03.4** Neutrality | **PASS** | Equal weight at equal CI and DBH across species; Beech growth 6.5 |
| **P2-03.5** 0.125 budget | **PASS** (F7) | Unit cases bind it and the fallback; the integration run never does |
| **P2-03.6** Broadleaf pruning | **PASS** | 2 lifts, 3-year interval, exactly 3.5 / 6.0 m; Sitka pruning compared on 45 combinations |
| **P2-03.7** Deadwood | **PASS** | One record per density death with `SpeciesId`; volume bit-exact |
| **P2-03.8** Q6 (mandatory) | **PASS** | `776a68df…` identical |
| **P2-04** Commands | **PASS** | Pending and Approve change nothing (reference-equal arrays); Execute keeps year 0; engine-minted IDs and key; replay answered and root reference unchanged; stale fence refused; whole-batch refusals incl. a batch injected after approval; no economy or stock field (reflection over 12 types). F2 on the extra refusal |
| **P2-05** Browsing and shelters | **PASS** | `p`, 0.90 loss and 0.96 survival bit-checked; sheltered `p = 0`; expiry exact (years installed … +7 on, +8 off); `FailedYear` stays −1; Oak 1.0 vs Beech 0.5; `V(h)`; cohorts ignore shelters; pressure 0 bit-identical. m11, m14, m32, m35, m38 caught |
| **P2-06** Spot clearance | **PASS as worded** (F1) | Radius `Mathf.Sqrt(1f/Mathf.PI)`, area 1.0 m², zero cover only inside, 0.30 / 0.45 recovery, `p_veg` changes, selective (all cohorts unchanged), divergence from Unity stated in the test name. **Survival effect not shown** (§5.1) |
| **P2-07** Planted promotion | **PASS as worded** (F3) | `PL-*` at exact X/Z in the same block and year as a natural promotion; block credit unchanged; Oak gate 0.19 no / 0.2 yes; Beech none; C2 radius not `r_pot`; `Age`, `DBH`; no same-year canopy recompute. Seeded states only |
| **P2-08** Seed rain and mast | **PASS as worded** (F4) | Cross-rack seed rain both ways; zero from outer grassland; reversed order identical; mast independent by seed (and by key at default seeds, per my probe); adult-empty rack advances and promotes in year 14 |
| **P2-09** Capacity and block cap | **PASS** | Occupancy ≤ 1 with all three species and the cap binding (max 1.0000, 0.178 rejected); ≤ 1 promotion per block-year across species; partial edge credit; tie order tested on constructed cases and a real tie; cohort export same-species same-year only. m23, m24, m36 caught |
| **P2-10** Determinism | **PASS** (F3) | Two 30-year runs identical (`93e9abda…`); reversed processing recomputed every year; replay no-op; forced pre-commit failure leaves the root reference intact for Execute and Year+1 and the retried year equals an uninterrupted twin; all recorded `ADULT-MORT` rolls recompute. m37 caught |
| **P2-11** Independent review | **This document** | |
| **H1** Scope | **PASS** | §5.8 |
| **H2** Evidence labels | **PASS** | Every constant group carries a class label in code; none is `IRISH DIRECT` (the only occurrence is the disclaimer) |
| **H3** 0.7 m rule | **PASS** | Labelled "NEW OWNER RULE" in the constants, in `Phase2Evidence.AdultSpacing` and in the refusal text |
| **H4** Handoff says Plant/Shelter/SpotClear are not `ResolvePlanting` | **NOT VERIFIABLE** | The statement exists in code (`Phase2Evidence.NotResolvePlanting`) but there is no handoff (F9) |
| **H5** Handoff lists what was not verified | **NOT VERIFIABLE** | Same |
| **G05** D-091 receipt and baselines | **NOT VERIFIABLE** | No receipt on the branch; baselines I did record (§3) |

## 11. What I did not verify

- **Anything in Unity,** `Assets/` behaviour, the `/multi` UI, a browser, a save, or any protected port. I read Unity source only; none of it was run.
- **The history of the tests.** One commit, so I could not see the five first-run failures, nor confirm the fixture rule was fixed before any number was measured. I checked the final oracles instead.
- **Cross-process, cross-OS or CI determinism.** Two suite runs and several probe runs on one host.
- **The owner's own words for Q1–Q6 and A-1.** I saw them only as rendered in the amendment record. The `i mod 3` assignment and the four-planting recipe are the implementer's choices; the 7 m gap and the brightest-cell plantings are my probes' choices, not the owner's.
- **Spec quartet beyond `8cec7c4d`,** the Stage 5 handoffs, `Docs/Project/**`, `active-tasks.md`, and D-091 lane admission.
- **The RNG call shape** (`Stage5Random.Stream` and `SimulationRandom.Roll` argument order) beyond reading it; the tests recompute recorded rolls from keys, which is supporting evidence.
- **Mutation coverage.** 36 single-site edits (35 valid). They show sensitivity to those specific errors, not completeness. Mutations were run on `/tmp` copies of the reviewed tree; the 8 survivors are listed rather than repaired.
- **My probes are throw-away and not committed.** Their outputs are reported above; the method of each is described (density recompute, three-arm and paired patch runs, reachability, imbalance accounting, ground-cell fraction, default-seed mast rolls). I did not publish their source.
- **Whether the owner wants F1 / F2 resolved before integration.** Those are theirs.
- **GitHub rendering of this file,** beyond reading the blob back byte for byte.

reviewed head 401dab1835128f70d1ecc28f54ae5db8ac68bf69, verdict APPROVED
