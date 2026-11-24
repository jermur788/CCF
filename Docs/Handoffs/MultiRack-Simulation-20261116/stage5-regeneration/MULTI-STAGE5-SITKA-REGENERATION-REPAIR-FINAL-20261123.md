# MULTI Stage 5 — final bounded repair (matched-area `S5-07A`, comparative `S5-02`): handoff

**State: IMPLEMENTED ON TASK BRANCH — PENDING DELTA RE-REVIEW.** Not verified, not integrated, not accepted. The suite is all green: **209 / 209**.

**In one paragraph.** The owner made two decisions to close the last two red tests. I implemented both and changed nothing else.
- **`S5-07A`** is now gated on the blocks that both lattices represent, per hectare of matched in-polygon area. It **passes** in both scenarios, on pooled and mean. Promotions are the closest to the limit, at 1.247 (managed thinning) and 1.181 (no work) against an upper bound of 1.33.
- **`S5-02`** is now a comparative suppression test. Its margins come from spec constants and were written into the spec before this pass measured anything. It **passes** with wide margins. The poor-light census (7 of 12 cells in year 12) is printed and never asserted.
- **A new test** covers an aligned 5 m block that straddles the two racks, the one untested `F6` corner.

No production code changed, so every protected hash is unchanged, including the `1/1/1/1` fingerprint `c4b670ed…`.

> **Read this before relying on the `S5-07A` pass.** It is a pass on the owner's population and units. It does **not** show that the two grids agree everywhere:
> - **Matched edge blocks** (24 blocks, not gated) show a per-hectare promotion ratio of **1.90 / 1.84**, against **1.15 / 1.08** in the interior. The gate passes because interior ground is 1925 of the 2268.75 m² (2.5 m) and 2525 m² (5.0 m) matched areas. The edge excess is diluted by the interior, not resolved (§4.3).
> - **Area normalisation pushes promotions up, not down.** The matched raw-count ratio is 1.120 / 1.061, and the area factor `2525 / 2268.75 = 1.113` raises it to 1.247 / 1.181.
> - **Under each grid's own model light** (`S5-07B`, not gated), matched promotions per hectare are **1.555 / 2.030**, which is outside `[0.75, 1.33]`.

---

## 1. Identity, SHAs and links

| Item | Value |
|---|---|
| Worker | Claude Code (Claude Opus 5.5, `claude-opus-5-5`) in T3 Code on the owner's Windows host. The lane is in WSL Ubuntu. No subagents |
| Role | Sole implementation writer, final bounded repair |
| Repair base | `a3d86aa27e268d89290987eb08f8f92f2582dd3b` (clean, equal to origin at start; origin re-checked before the push) |
| **Spec amendment (docs branch)** | `46c08e1f5863d51a3d68000af9eb909956d9e3bd` on `docs/multi-stage5-sitka-regeneration-spec-20261123-v1`, parent `c71f3a0f`. https://github.com/jermur788/CCF/commit/46c08e1f5863d51a3d68000af9eb909956d9e3bd |
| Amendment on the feature branch | `6bac1e4416d39a8133b08a5f52f384edfd2b2560`, a cherry-pick `-x` of `46c08e1f`. The four spec files are byte-identical to the docs branch (checked with `cmp`) |
| **Test commit** | `b5eadd19c63eda133f2f99dc328e1c21b65ad406` (tree `74676bf3…`). Every number below was measured on this tree, in one evidence run |
| Handoff commit | The commit that adds this file, on top of `b5eadd19`. The branch head is this commit |
| Feature branch | https://github.com/jermur788/CCF/tree/feat/multi-stage5-sitka-regeneration-20261123-v1 |
| Docs branch | https://github.com/jermur788/CCF/tree/docs/multi-stage5-sitka-regeneration-spec-20261123-v1 |
| Inputs | Task packet "FINAL BOUNDED REPAIR"; previous handoff `MULTI-STAGE5-SITKA-REGENERATION-REPAIR-20261123.md`, which is left unchanged as history |

**Order of work:**
1. Spec amendment, written in a temporary detached worktree `/tmp/stage5-spec-final` (removed afterwards).
2. Pushed it, then cherry-picked it onto the feature branch.
3. Test code.
4. One build and **one** measurement run.
5. UI compile check.
6. Commit.

No diagnostic or temporary test was used in this pass.

**Disclosure.** My first attempt at the test commit failed before git ran: an apostrophe in the inline commit message broke the shell quoting. Nothing was committed. I wrote the message to a file and committed once.

## 2. The two owner decisions

Neither decision changes a rule, a constant, the tolerance or the shared-light control. Production code is untouched: the diff `a3d86aa2..b5eadd19` contains only the four spec files and two test files.

### D1 — `S5-07A` is matched-area normalised

**Why.** The two lattices do not cover the same boundary ground. Twelve aligned 5 m blocks hold in-polygon 2.5 m cells, but their 5 m centre is outside every polygon, so the 5.0 m grid has no cell there. A whole-stand total therefore compares different ground.

**Definition** (spec §9 Q4, `46c08e1f`):
- **Population.** The aligned 5 m blocks whose 5 m centre `((X + 0.5)·5, (Z + 0.5)·5)` is strictly inside a polygon, under the lattices' own centre-in-polygon rule. These are the blocks represented on both lattices.
- **Units.** Per hectare of matched in-polygon area, computed from each lattice's own in-polygon cells in those blocks: 6.25 m² per 2.5 m cell and 25 m² per 5.0 m cell.
- **Gate.** Pooled `(Σ_s x_2.5,s / A_2.5) / (Σ_s x_5.0,s / A_5.0)` **and** the mean over seeds of the per-seed ratio must lie in `[0.75, 1.33]`, for accepted abundance and promotions, in both scenarios.

**Asserted, not assumed** (`S5-07A G-01`, `LatticeFacts.Verify`):
- every 5.0 m block holds exactly one 5.0 m cell, and there are as many blocks as cells;
- every 5.0 m cell is centred exactly on its 5 m block centre;
- the 2.5 m blocks with an in-polygon 5 m centre are **set-equal** to the blocks of the 5.0 m cells: 1:1, and each holds at least one 2.5 m cell;
- there are 101 of them on MA-1;
- every interior block is matched, and every 2.5 m block is either matched or unmatched;
- both matched areas are > 0;
- every 5.0 m study run has zero unmatched ground.

**Matched area totals (MA-1, measured):**

| | Blocks | In-polygon cells | Matched in-polygon area | For information only: clipped polygon area of those cells |
|---|---|---|---|---|
| 2.5 m | 101 = 77 interior + 24 edge (edge blocks with 3 / 2 / 1 cells: 8 / 15 / 1) | 363 | **2268.75 m²** (0.2269 ha) | 2219.88 m² |
| 5.0 m | 101 (one cell each) | 101 | **2525.00 m²** (0.2525 ha) | 2270.06 m² |
| *Unmatched (2.5 m only)* | 12 (8 with 2 cells, 4 with 1) | 20 | 125.00 m² | 99.37 m² |

The lattices total 383 cells in 113 blocks at 2.5 m and 101 cells in 101 blocks at 5.0 m.

### D2 — `S5-02` is comparative suppression

The absolute proxy ("every dense test cell in the poor-light band in year 12") was never an owner criterion. It is now a printed diagnostic. The blocking criterion, written into the spec's second amendment record *before* this pass measured anything, is:

1. **The gap raises the light index.** In year 1 the gap's mean light exceeds the dense interior's by more than 0.2. This is unchanged from the first pass and its margin is not re-derived.
2. **The dense interior never reaches promotion height.** In years 1–15, no band of a dense test cell reaches 3.5 m and no dense test cell promotes.
3. **The order never inverts.** In every year 1–15:
   - the dense interior's tallest cohort is ≤ the tallest cohort the gap has reached (a gap cohort that has promoted counts at its promotion height);
   - the dense standing abundance is ≤ the gap's.
4. **Materially shorter and sparser.** In years 12 and 15:
   - the gap leads by **≥ 0.315 m**;
   - the dense abundance is **≤ 0.8 ×** the gap's.
5. **The gap grows toward promotion.** In year 12 the gap's tallest cohort is ≥ 1.5 m. This is unchanged.

**Where the margins come from:**
- **0.315 m = 0.35 × 0.90** is one maximal year of §3.2 growth, at the peak of LightResponse. A lead at least this large cannot be explained by cohorts established one year apart.
- **0.8** is the §3.2 poor-light survival factor, the spec's own one-year suppression penalty.
- **Year 12** is the first year a year-1 cohort can reach 3.5 m at maximal growth: `0.15 + 11 × 0.315 = 3.615`. **Year 15** is the end of the horizon.
- **Standing abundance** does not credit the gap with abundance its cohorts exported by promotion. That makes criteria 3 and 4 conservative.

*Disclosure of prior knowledge.* Before choosing these margins I had read the previous handoff. It reports the gap's 3.24 m tallest band in year 12 and the 7/12 census. It does not report the dense interior's heights or either abundance. The margins come from spec constants, and none was changed after the run.

## 3. `S5-07A` — BLOCKING — result: **PASS**

### 3.1 Gate (shared field, matched area, per hectare)

| Scenario | Metric | Pooled | Mean | Per seed min – median – max | Gate |
|---|---|---|---|---|---|
| managed thinning | accepted abundance | **0.956** | **0.956** | 0.954 – 0.956 – 0.958 | pass |
| managed thinning | promotions | **1.247** | **1.246** | 1.163 – 1.243 – 1.355 | pass (2 of 20 seeds above 1.33; per-seed values are informational under the owner's definition) |
| no work | accepted abundance | **0.952** | **0.952** | 0.949 – 0.952 – 0.955 | pass |
| no work | promotions | **1.181** | **1.181** | 1.085 – 1.183 – 1.259 | pass |

**Totals over 20 seeds.**

| Scenario | Metric | 2.5 m total over 2268.75 m² | 5.0 m total over 2525 m² |
|---|---|---|---|
| managed thinning | accepted abundance | 4496.0 (mean 990.8 / ha per run) | 5232.6 (1036.2 / ha) |
| managed thinning | promotions | 2520 (555.4 / ha) | 2249 (445.3 / ha) |
| no work | accepted abundance | 2754.4 (607.0 / ha) | 3219.9 (637.6 / ha) |
| no work | promotions | 819 (180.5 / ha) | 772 (152.9 / ha) |

**Other `S5-07A` requirements, all asserted and passing:**
- *Identical light.* 121,200 block-year comparisons, which is every matched block in every year of every run, are all bit-equal. The assertion is now an exact count, `= 30 × 101` per run; it was `≥ 30 × 77` before.
- *Year loop.* The test's year loop equals `Advance` bit for bit under model light, at 2.5 m (managed thinning) and at 5.0 m (no work), over 30 years.
- *Reference.* The `1/1/1/0` reference starts from the same year-0 roster.
- *Polygons.* Zero out-of-polygon trees over all 160 study runs.
- *5.0 m purity.* The purity pin `c67edc6b…` matches, so 5.0 m keeps `a = 1.0` and is bit-identical to the first and current pass.
- *No damping constant.* There is no production change at all.

### 3.2 Before and after

| Pooled (mean) | Option A repair `a3d86aa2`: whole stand, raw totals | Matched blocks, raw counts *(printed, not gated)* | **This pass `b5eadd19`: matched, per ha (gated)** |
|---|---|---|---|
| Managed thinning, accepted | 0.961 (0.961) | 0.859 | **0.956 (0.956)** |
| Managed thinning, promotions | **1.575 (1.575) FAIL** | 1.120 | **1.247 (1.246)** |
| No work, accepted | 0.994 (0.994) | 0.855 | **0.952 (0.952)** |
| No work, promotions | **2.008 (2.009) FAIL** | 1.061 | **1.181 (1.181)** |

The whole-stand figures are reproduced exactly by this run and printed as information. The underlying runs are unchanged: the per-seed whole-stand promotion counts equal the previous appendix seed for seed.

The middle column confirms the previous handoff's unverified estimate of 1.12 / 1.06, now measured. Area normalisation multiplies every matched raw ratio by `A_5.0 / A_2.5 = 2525 / 2268.75 = 1.113`. That brings accepted abundance near 1. It also brings promotions closer to the upper bound.

### 3.3 Sub-ratios and sensitivity (shared field, per hectare, not gated)

| Pooled (mean) | Interior (77 blocks; 1925 m² on both grids) | Matched edge (24 blocks; 343.75 m² vs 600 m²) |
|---|---|---|
| Managed thinning, accepted | 1.002 (1.002) | 0.991 (0.991) |
| Managed thinning, promotions | 1.146 (1.145): 1414 / 1234 | **1.902 (1.901)**: raw 1106 / 1015 |
| No work, accepted | 1.004 (1.004) | 1.036 (1.036) |
| No work, promotions | 1.075 (1.077): 258 / 240 | **1.841 (1.841)**: raw 561 / 532 |

*What this means.* In matched edge blocks the two grids promote about the same **number** of trees (1.09 / 1.05 raw). The 2.5 m grid does it on far less in-polygon ground (343.75 m² against 600 m²). A2's budget scales with that ground, but it rarely binds, because blocks run out of cohorts before they run out of credit (previous handoff, §3.1). Per hectare, the 2.5 m edge therefore promotes about 1.9 times as densely.

This was not a gated population, and I changed nothing about it. A reviewer should know the pass rests on the interior's 1.15 / 1.08 carrying the edge's 1.9.

*Area-basis sensitivity.* This is computed from the printed totals; it is not gated and is not the owner's unit. If each lattice's matched area were its cells' **clipped** polygon area (2219.88 m² against 2270.06 m²), the pooled ratios would be:
- managed thinning: accepted 0.879, promotions 1.146;
- no work: accepted 0.875, promotions 1.085.

The owner's unit is whole in-polygon cells (6.25 m² and 25 m²), and that is what the gate uses.

## 4. `S5-07B` — MANDATORY, NON-BLOCKING DIAGNOSTIC (no assertion)

### 4.1 Each grid's own model light

| Pooled (mean) | Whole stand | Matched area, per ha |
|---|---|---|
| Managed thinning, accepted | 0.896 (0.896) | 0.921 (0.921) |
| Managed thinning, promotions | 1.654 (1.655) | **1.555 (1.556)** |
| No work, accepted | 0.882 (0.882) | 0.891 (0.891) |
| No work, promotions | 2.124 (2.147) | **2.030 (2.051)** |

**Per-seed min – median – max under model light.**

| Scenario | Metric | Whole stand | Matched area, per ha |
|---|---|---|---|
| managed thinning | accepted | 0.880 – 0.893 – 0.917 | 0.903 – 0.916 – 0.943 |
| managed thinning | promotions | 1.562 – 1.665 – 1.789 | 1.470 – 1.554 – 1.743 |
| no work | accepted | 0.857 – 0.882 – 0.899 | 0.865 – 0.889 – 0.910 |
| no work | promotions | 1.800 – 2.068 – 2.652 | 1.781 – 1.952 – 2.538 |

### 4.2 Interior and edge sub-ratios under both light bases (pooled; means in the test output)

| | Interior, model light | Interior, shared field | Matched edge per ha, model light | Matched edge per ha, shared field | All edge raw, model light | All edge raw, shared field |
|---|---|---|---|---|---|---|
| Managed thinning, accepted | 1.007 | 1.002 | 0.763 | 0.991 | 0.656 | 0.877 |
| Managed thinning, promotions | 1.749 | 1.146 | 1.676 | 1.902 | 1.535 | 2.098 |
| No work, accepted | 1.022 | 1.004 | 0.753 | 1.036 | 0.649 | 0.975 |
| No work, promotions | 3.418 | 1.075 | 1.845 | 1.841 | 1.501 | 2.429 |

### 4.3 Unmatched-block disclosure (new)

The 12 blocks hold 20 in-polygon 2.5 m cells, 125.00 m² (clipped 99.37 m²). The 5.0 m grid has no cell there.

| Light basis, scenario | Promotions Σ over 20 seeds (per seed min – median – max) | Share of 2.5 m whole-stand promotions | Accepted abundance Σ | Share of 2.5 m whole-stand per-cell acceptance |
|---|---|---|---|---|
| Model light, managed thinning | 392 (17 – 19.5 – 22) | 15.5 % | 321.0 | 7.7 % |
| Model light, no work | 179 (7 – 9.0 – 10) | 14.1 % | 240.2 | 9.3 % |
| Shared field, managed thinning | 1023 (42 – 51.0 – 57) | 28.9 % | 532.6 | 10.6 % |
| Shared field, no work | 731 (33 – 37.0 – 40) | 47.2 % | 444.7 | 13.9 % |

**Statement, as printed by the test.** The whole-stand 2.5 m figures exceed the 5.0 m figures partly because the finer lattice represents boundary ground the coarser lattice cannot. Twelve aligned 5 m blocks hold in-polygon 2.5 m cells, but their 5 m centre lies outside every polygon, so the 5.0 m grid has no cell there. Their promotions and abundance count in the 2.5 m whole stand with nothing on the 5.0 m side to compare against. That is why `S5-07A` gates the matched-area population, per hectare (owner decision D1).

Under the shared field these blocks read light at a block centre outside the polygon, as the review warned (review §4.4). This is why their shared-field promotions (1023 / 731) are much higher than under model light (392 / 179).

**Retained statements.**
- *Non-PAR.* Light is the modelled index `unity-k10a10-decoupled-v1`, not calibrated PAR.
- *Finer sampling.* Under model light each grid samples its own canopy, including its own promoted adults, at its own cell centres, so the 2.5 m grid resolves gaps more finely. A higher 2.5 m real-light recruitment reflects that finer gap-resolution sampling, not a scaling defect.
- *Not separated.* As before, I did not separate the effect of sampling position from the effect of the run's own canopy, including its own promoted adults.

## 5. `S5-02` — BLOCKING — result: **PASS** (criterion as now worded, §2 D2)

The test cells are 12 Area A cells. The gap was made by felling 33 stems. In year 1 the mean light is 0.003 in the dense interior and 0.980 in the gap. Over the horizon, 3 gap promotions come from the test cells and none from the dense test cells.

| Year | Dense tallest / gap tallest reached (m) | Dense / gap standing abundance | Dense mean light | *Diagnostic: dense cells with LR < 0.15* |
|---|---|---|---|---|
| 1 | 0.00 / 0.15 | 0.000 / 1.068 | 0.003 | 12/12 |
| 2 | 0.15 / 0.43 | 0.017 / 1.676 | 0.021 | 12/12 |
| 3 | 0.18 / 0.72 | 0.052 / 2.204 | 0.035 | 11/12 |
| 4 | 0.21 / 1.01 | 0.094 / 2.967 | 0.034 | 11/12 |
| 5 | 0.34 / 1.18 | 0.238 / 3.340 | 0.085 | 7/12 |
| 6 | 0.53 / 1.47 | 0.423 / 3.959 | 0.085 | 7/12 |
| 7 | 0.71 / 1.77 | 0.665 / 4.447 | 0.117 | 7/12 |
| 8 | 0.85 / 2.06 | 0.922 / 4.500 | 0.142 | 7/12 |
| 9 | 1.08 / 2.36 | 1.136 / 4.500 | 0.135 | 7/12 |
| 10 | 1.30 / 2.66 | 1.354 / 4.500 | 0.148 | 7/12 |
| 11 | 1.51 / 2.96 | 1.474 / 4.500 | 0.141 | 7/12 |
| **12** | **1.74 / 3.24** | **1.569 / 4.500** | 0.134 | **7/12** |
| 13 | 1.99 / 3.53 (a promotion) | 1.613 / 3.800 | 0.129 | 7/12 |
| 14 | 2.24 / 3.78 | 1.646 / 4.275 | 0.123 | 7/12 |
| **15** | **2.48 / 3.78** | **1.851 / 4.145** | 0.207 | 6/12 |

**Results against the margins:**
- **Year 12:** the gap leads by **1.509 m** (margin ≥ 0.315); the dense/gap abundance ratio is **0.349** (margin ≤ 0.8).
- **Year 15:** the lead is **1.307 m**; the ratio is **0.446**.
- The order never inverts in any year. The dense interior's maximum over 15 years is 2.48 m, below 3.5 m.
- The gap is 3.24 m in year 12, above 1.5 m.

The census is printed and never asserted. It shows the interior's light rising as Area A self-thins: 12/12 cells in years 1–2, then 7/12 from year 5, then 6/12 in year 15.

## 6. Straddling-block test (`S5-08 P-08`, new; closes the `F6` corner)

**Set-up.** Two touching, treeless 7.5 × 7.5 m racks (light 1, no seed) share the edge x = 7.5 m. That edge splits aligned block column 1:
- block **(1,0)** = MA-A (2,0), (2,1) + MA-B (3,0), (3,1): a full block, n = 4;
- block **(1,1)** = MA-A (2,2) + MA-B (3,2): n = 2.

There are nine bands (cohorts 3, 5 and 7, spread over both racks).

**Asserted and passing:**
- **Credit across the boundary.** The genesis blocks are `(1,0) n = 4` and `(1,1) n = 2`, counted over both racks; each rack alone holds 2 and 1. The 5.0 m lattice has no cell in block column 1, because its centre lies on the shared edge: an unmatched block, as in `S5-07`.
- **Promotion schedule:** `17:R17-MA-A-2-0+1, 18:R18-MA-B-3-1+1, 18:R18-MA-B-3-2+1, 19:R19-MA-B-3-0+0, 20:R20-MA-A-2-2+0, 22:R22-MA-B-3-2+0`.
  - The full straddling block promotes every year (budget 1), and its winner alternates between the racks.
  - The 2-cell straddling block promotes every 2nd year (budget 0.5). Counted per rack, both blocks would promote less often.
- **Budget.** Block (1,1) is withheld in years 17, 19 and 21. Each withholding is booked to the rack of that year's would-be winner: MA-A 1, MA-B 2. Credit after the idle years is capped at one full block, 4.
- **Cohort export across the edge.**
  - Year 17: the MA-A winner's record carries the MA-B cohort-3 band (0.1). MA-B books it (cohort bands 1, export 0.1), and MA-A books only its own winner. MA-B cell (3,1) keeps cohort 5.
  - Year 18: both MA-B winners take their cohort's MA-A band, and MA-A books 2 exported bands.
- **One tree per cohort.** Nine bands become six trees, with three bands exported, and nothing is left over. Every tree is strictly inside its own rack.
- **Accounting.** The abundance identity holds in each rack in every year (residual ≤ 1e-6), and the racks book exactly the abundance the records carry (≤ 1e-9).

## 7. G3 (measurement-integrity) items I touched

1. **`S5-07A` population** changed from the whole stand to the matched blocks (owner D1). The previous whole-stand gate is printed as information.
2. **`S5-07A` units** changed from raw totals to per hectare of matched in-polygon area, using whole cells (6.25 m² and 25 m²) as the owner specified. Clipped areas are printed for information. The clipped-basis sensitivity in §3.3 is computed by hand and not gated.
3. **The 1:1 correspondence** between matched blocks and 5.0 m cells is now asserted (§2), together with "every interior block is matched", "both areas > 0" and "5.0 m runs have no unmatched ground". These checks are new.
4. **The light-identity assertion is strengthened** from `coincident ≥ 30 × interior blocks` to `coincident = 30 × matched blocks`. The proof now covers the whole gated population.
5. **`S5-07B` now prints shared-field splits itself**, using memoised `S5-07A` runs, so it shows both light bases. It adds matched-area and matched-edge per-hectare lines and the unmatched-block disclosure. It still has no assertion; its `Print` path never calls `T`.
6. **`S5-02`: the absolute poor-light assertion was removed** and is now a printed diagnostic (owner D2).
7. **`S5-02`: new comparative criteria** with margins 0.315 m and 0.8, evaluated in years 12 and 15, with a never-inverts check in every year. These are definition choices within D2, written into the spec (`46c08e1f`) before measurement. Two choices are mine:
   - "gap tallest reached" counts a promoted gap cohort at its promotion height;
   - "standing abundance" excludes exported abundance.

   Both are stated in the spec. The previous strict year-12 checks (`gap > dense` in height and abundance) are implied by the stronger margins and were replaced.
8. **`S5-02`: the 3.5 m check** now reads `Stage5Constants.PromotionHeightM` (= 3.5) instead of the literal `3.5f`. The value is unchanged.
9. **New test `S5-08 P-08`.** Its tolerances are 1e-6 on the per-rack abundance identity and 1e-9 on booking. They are generous because nothing establishes, merges or dies in that set-up, so all densities are exact floats.

There is no change to any rule, constant, the `[0.75, 1.33]` tolerance, the shared-light control, the seeds, the horizon, or the scenarios.

## 8. Hashes, build and tests (test tree `b5eadd19`)

| Anchor | Expected | Observed |
|---|---|---|
| Single-area Model 0 | `7e3a2a86…` | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` ✓ |
| Multi-area Model 0 | `a692a6d5…` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` ✓ |
| Stage 3 C2 `0/0/1` | `814e8165…` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` ✓ |
| Experiment | `c66d566c…` | `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` ✓ |
| P0 pin (`sha256sum .p0ref/SitkaLab.Core.dll`) | `9a99f781…` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` ✓ |
| `1/1/1/0` full run | `8a926776…` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` ✓ (S4 run, S5-01 explicit and default recipe) |
| 5.0 m purity pin | `c67edc6b…` | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` ✓ |
| **`1/1/1/1` full run** (must not move) | `c4b670ed…` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` ✓ unchanged |

- **Build.** The solution built with `--no-incremental` gives **0 warnings, 0 errors** (`TreatWarningsAsErrors`). `src/Ui/SitkaP1.Ui.csproj` also gives 0 / 0; it was compiled only, not run.
- **Tests: 209 / 209 passed, 0 failed, exit code 0.** The count was 208 before; this pass adds `S5-08 P-08`. `S5-07A G-01` and `S5-02 G-01` are now green, and no test is red.
- **Hygiene.** `git status --short` is empty after each commit. The diff `a3d86aa2..b5eadd19` is the 4 files under `specs/multi-stage5-sitka-regeneration-v1/**` plus `tests/Core.Tests/Stage5GridStudy.cs` and `tests/Core.Tests/Stage5Tests.cs`. This handoff adds one file under `Docs/Handoffs/…/stage5-regeneration/`. There is nothing in `src/Core/**`, `Assets/`, `*.meta`, `ProjectSettings/`, `src/Ui`, wwwroot, `.sln`, `.csproj`, `.p0ref` or a save schema.

## 9. Exact commands (WSL, from the lane)

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage5-SitkaRegen-20261123/Tools/MultiRackSitkaP1Lab
sha256sum .p0ref/SitkaLab.Core.dll
TMPDIR=/tmp/stage5-final /home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers --no-incremental
TMPDIR=/tmp/stage5-final /home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
TMPDIR=/tmp/stage5-final /home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers   # compile only
```

The relevant output lines are the ones starting with `S5-07`, `S5-07A`, `S5-07B`, `S5-02` and `S5-08 P-08`.

## 10. Not verified

- **Unity runtime parity** of any Stage 5 rule. Nothing ran in Unity, and no Unity file, process or lock was touched.
- **Ecological validity.** Every constant is gameplay calibration or transfer; light is not PAR. `S5-02` shows the stated rules' numerical behaviour, not a forest forecast.
- **That the grids agree on edge ground.** The matched-edge per-hectare promotion ratio (1.90 / 1.84) is outside the tolerance and not gated (§3.3). The model-light matched ratios (1.555 / 2.030) are also outside and not gated (§4.1).
- **Any alternative population, area basis, control or rule.** None was run; the clipped-area figures in §3.3 are arithmetic on printed totals.
- **UI runtime and `/multi`.** The UI project was compiled, not run, and no listener was started.
- **The owner's own words** for D1 and D2. They were taken from the task packet.
- **Other environments.** Windows `git.exe` (not used, by instruction), CI, other operating systems, and GitHub rendering of this file were not checked.
- **Cross-process determinism** beyond identical hashes from separate runs on this host.

## Appendix — `S5-07A` per-seed output (verbatim, evidence run on `b5eadd19`)

```text
S5-07A managed thinning seed  1: matched per ha: accepted 1015.0 vs 1059.8 = 0.958; promotions 125 (551.0/ha) vs 109 (431.7/ha) = 1.276. Whole stand: accepted 0.961, promotions 177/109 = 1.624 (interior 68/61, matched edge 57/48, 2.5 m-only blocks 52)
S5-07A managed thinning seed  2: matched per ha: accepted 998.1 vs 1041.6 = 0.958; promotions 140 (617.1/ha) vs 115 (455.4/ha) = 1.355. Whole stand: accepted 0.963, promotions 197/115 = 1.713 (interior 76/62, matched edge 64/53, 2.5 m-only blocks 57)
S5-07A managed thinning seed  3: matched per ha: accepted 1004.5 vs 1052.2 = 0.955; promotions 136 (599.4/ha) vs 113 (447.5/ha) = 1.339. Whole stand: accepted 0.959, promotions 185/113 = 1.637 (interior 74/62, matched edge 62/51, 2.5 m-only blocks 49)
S5-07A managed thinning seed  4: matched per ha: accepted 988.0 vs 1034.2 = 0.955; promotions 123 (542.1/ha) vs 107 (423.8/ha) = 1.279. Whole stand: accepted 0.961, promotions 178/107 = 1.664 (interior 72/59, matched edge 51/48, 2.5 m-only blocks 55)
S5-07A managed thinning seed  5: matched per ha: accepted 996.5 vs 1043.0 = 0.955; promotions 126 (555.4/ha) vs 116 (459.4/ha) = 1.209. Whole stand: accepted 0.959, promotions 179/116 = 1.543 (interior 71/64, matched edge 55/52, 2.5 m-only blocks 53)
S5-07A managed thinning seed  6: matched per ha: accepted 984.1 vs 1029.3 = 0.956; promotions 129 (568.6/ha) vs 114 (451.5/ha) = 1.259. Whole stand: accepted 0.961, promotions 185/114 = 1.623 (interior 71/61, matched edge 58/53, 2.5 m-only blocks 56)
S5-07A managed thinning seed  7: matched per ha: accepted 976.5 vs 1021.1 = 0.956; promotions 130 (573.0/ha) vs 124 (491.1/ha) = 1.167. Whole stand: accepted 0.961, promotions 181/124 = 1.460 (interior 71/66, matched edge 59/58, 2.5 m-only blocks 51)
S5-07A managed thinning seed  8: matched per ha: accepted 986.0 vs 1033.6 = 0.954; promotions 106 (467.2/ha) vs 101 (400.0/ha) = 1.168. Whole stand: accepted 0.960, promotions 155/101 = 1.535 (interior 60/57, matched edge 46/44, 2.5 m-only blocks 49)
S5-07A managed thinning seed  9: matched per ha: accepted 966.0 vs 1009.5 = 0.957; promotions 125 (551.0/ha) vs 116 (459.4/ha) = 1.199. Whole stand: accepted 0.963, promotions 175/116 = 1.509 (interior 73/64, matched edge 52/52, 2.5 m-only blocks 50)
S5-07A managed thinning seed 10: matched per ha: accepted 996.4 vs 1039.8 = 0.958; promotions 110 (484.8/ha) vs 102 (404.0/ha) = 1.200. Whole stand: accepted 0.962, promotions 152/102 = 1.490 (interior 62/57, matched edge 48/45, 2.5 m-only blocks 42)
S5-07A managed thinning seed 11: matched per ha: accepted 995.5 vs 1041.0 = 0.956; promotions 132 (581.8/ha) vs 113 (447.5/ha) = 1.300. Whole stand: accepted 0.961, promotions 183/113 = 1.619 (interior 76/62, matched edge 56/51, 2.5 m-only blocks 51)
S5-07A managed thinning seed 12: matched per ha: accepted 994.5 vs 1040.3 = 0.956; promotions 114 (502.5/ha) vs 107 (423.8/ha) = 1.186. Whole stand: accepted 0.961, promotions 163/107 = 1.523 (interior 66/61, matched edge 48/46, 2.5 m-only blocks 49)
S5-07A managed thinning seed 13: matched per ha: accepted 976.3 vs 1021.1 = 0.956; promotions 117 (515.7/ha) vs 112 (443.6/ha) = 1.163. Whole stand: accepted 0.962, promotions 167/112 = 1.491 (interior 64/60, matched edge 53/52, 2.5 m-only blocks 50)
S5-07A managed thinning seed 14: matched per ha: accepted 997.8 vs 1042.9 = 0.957; promotions 141 (621.5/ha) vs 123 (487.1/ha) = 1.276. Whole stand: accepted 0.961, promotions 194/123 = 1.577 (interior 75/65, matched edge 66/58, 2.5 m-only blocks 53)
S5-07A managed thinning seed 15: matched per ha: accepted 988.2 vs 1033.7 = 0.956; promotions 131 (577.4/ha) vs 111 (439.6/ha) = 1.313. Whole stand: accepted 0.961, promotions 182/111 = 1.640 (interior 74/62, matched edge 57/49, 2.5 m-only blocks 51)
S5-07A managed thinning seed 16: matched per ha: accepted 979.7 vs 1024.6 = 0.956; promotions 121 (533.3/ha) vs 109 (431.7/ha) = 1.235. Whole stand: accepted 0.961, promotions 175/109 = 1.606 (interior 70/60, matched edge 51/49, 2.5 m-only blocks 54)
S5-07A managed thinning seed 17: matched per ha: accepted 998.3 vs 1045.4 = 0.955; promotions 130 (573.0/ha) vs 118 (467.3/ha) = 1.226. Whole stand: accepted 0.959, promotions 182/118 = 1.542 (interior 74/65, matched edge 56/53, 2.5 m-only blocks 52)
S5-07A managed thinning seed 18: matched per ha: accepted 984.3 vs 1029.9 = 0.956; promotions 114 (502.5/ha) vs 104 (411.9/ha) = 1.220. Whole stand: accepted 0.961, promotions 160/104 = 1.538 (interior 63/57, matched edge 51/47, 2.5 m-only blocks 46)
S5-07A managed thinning seed 19: matched per ha: accepted 997.0 vs 1042.2 = 0.957; promotions 127 (559.8/ha) vs 113 (447.5/ha) = 1.251. Whole stand: accepted 0.961, promotions 176/113 = 1.558 (interior 76/63, matched edge 51/50, 2.5 m-only blocks 49)
S5-07A managed thinning seed 20: matched per ha: accepted 994.2 vs 1038.1 = 0.958; promotions 143 (630.3/ha) vs 122 (483.2/ha) = 1.305. Whole stand: accepted 0.962, promotions 197/122 = 1.615 (interior 78/66, matched edge 65/56, 2.5 m-only blocks 54)
S5-07A no work seed  1: matched per ha: accepted 625.2 vs 655.6 = 0.954; promotions 42 (185.1/ha) vs 38 (150.5/ha) = 1.230. Whole stand: accepted 0.993, promotions 77/38 = 2.026 (interior 14/12, matched edge 28/26, 2.5 m-only blocks 35)
S5-07A no work seed  2: matched per ha: accepted 606.4 vs 636.9 = 0.952; promotions 41 (180.7/ha) vs 38 (150.5/ha) = 1.201. Whole stand: accepted 0.994, promotions 77/38 = 2.026 (interior 13/11, matched edge 28/27, 2.5 m-only blocks 36)
S5-07A no work seed  3: matched per ha: accepted 612.4 vs 643.9 = 0.951; promotions 41 (180.7/ha) vs 38 (150.5/ha) = 1.201. Whole stand: accepted 0.994, promotions 78/38 = 2.053 (interior 14/12, matched edge 27/26, 2.5 m-only blocks 37)
S5-07A no work seed  4: matched per ha: accepted 605.2 vs 635.9 = 0.952; promotions 40 (176.3/ha) vs 39 (154.5/ha) = 1.141. Whole stand: accepted 0.993, promotions 75/39 = 1.923 (interior 13/13, matched edge 27/26, 2.5 m-only blocks 35)
S5-07A no work seed  5: matched per ha: accepted 610.5 vs 642.2 = 0.951; promotions 40 (176.3/ha) vs 40 (158.4/ha) = 1.113. Whole stand: accepted 0.993, promotions 76/40 = 1.900 (interior 13/13, matched edge 27/27, 2.5 m-only blocks 36)
S5-07A no work seed  6: matched per ha: accepted 595.8 vs 626.0 = 0.952; promotions 43 (189.5/ha) vs 39 (154.5/ha) = 1.227. Whole stand: accepted 0.995, promotions 80/39 = 2.051 (interior 14/13, matched edge 29/26, 2.5 m-only blocks 37)
S5-07A no work seed  7: matched per ha: accepted 597.5 vs 628.4 = 0.951; promotions 41 (180.7/ha) vs 40 (158.4/ha) = 1.141. Whole stand: accepted 0.992, promotions 79/40 = 1.975 (interior 13/12, matched edge 28/28, 2.5 m-only blocks 38)
S5-07A no work seed  8: matched per ha: accepted 605.5 vs 635.2 = 0.953; promotions 45 (198.3/ha) vs 42 (166.3/ha) = 1.192. Whole stand: accepted 0.995, promotions 83/42 = 1.976 (interior 13/13, matched edge 32/29, 2.5 m-only blocks 38)
S5-07A no work seed  9: matched per ha: accepted 594.3 vs 624.2 = 0.952; promotions 40 (176.3/ha) vs 39 (154.5/ha) = 1.141. Whole stand: accepted 0.994, promotions 77/39 = 1.974 (interior 13/12, matched edge 27/27, 2.5 m-only blocks 37)
S5-07A no work seed 10: matched per ha: accepted 613.1 vs 642.3 = 0.955; promotions 38 (167.5/ha) vs 37 (146.5/ha) = 1.143. Whole stand: accepted 0.995, promotions 71/37 = 1.919 (interior 13/12, matched edge 25/25, 2.5 m-only blocks 33)
S5-07A no work seed 11: matched per ha: accepted 612.0 vs 645.0 = 0.949; promotions 42 (185.1/ha) vs 39 (154.5/ha) = 1.199. Whole stand: accepted 0.990, promotions 78/39 = 2.000 (interior 13/12, matched edge 29/27, 2.5 m-only blocks 36)
S5-07A no work seed 12: matched per ha: accepted 610.3 vs 641.0 = 0.952; promotions 39 (171.9/ha) vs 40 (158.4/ha) = 1.085. Whole stand: accepted 0.994, promotions 77/40 = 1.925 (interior 12/12, matched edge 27/28, 2.5 m-only blocks 38)
S5-07A no work seed 13: matched per ha: accepted 594.9 vs 625.5 = 0.951; promotions 43 (189.5/ha) vs 38 (150.5/ha) = 1.259. Whole stand: accepted 0.994, promotions 80/38 = 2.105 (interior 15/12, matched edge 28/26, 2.5 m-only blocks 37)
S5-07A no work seed 14: matched per ha: accepted 614.3 vs 644.7 = 0.953; promotions 38 (167.5/ha) vs 38 (150.5/ha) = 1.113. Whole stand: accepted 0.993, promotions 73/38 = 1.921 (interior 12/12, matched edge 26/26, 2.5 m-only blocks 35)
S5-07A no work seed 15: matched per ha: accepted 605.1 vs 635.8 = 0.952; promotions 41 (180.7/ha) vs 37 (146.5/ha) = 1.233. Whole stand: accepted 0.994, promotions 78/37 = 2.108 (interior 11/10, matched edge 30/27, 2.5 m-only blocks 37)
S5-07A no work seed 16: matched per ha: accepted 611.3 vs 642.8 = 0.951; promotions 40 (176.3/ha) vs 38 (150.5/ha) = 1.172. Whole stand: accepted 0.990, promotions 80/38 = 2.105 (interior 13/12, matched edge 27/26, 2.5 m-only blocks 40)
S5-07A no work seed 17: matched per ha: accepted 613.1 vs 644.5 = 0.951; promotions 44 (193.9/ha) vs 40 (158.4/ha) = 1.224. Whole stand: accepted 0.992, promotions 81/40 = 2.025 (interior 13/13, matched edge 31/27, 2.5 m-only blocks 37)
S5-07A no work seed 18: matched per ha: accepted 599.9 vs 630.1 = 0.952; promotions 43 (189.5/ha) vs 38 (150.5/ha) = 1.259. Whole stand: accepted 0.994, promotions 79/38 = 2.079 (interior 14/13, matched edge 29/25, 2.5 m-only blocks 36)
S5-07A no work seed 19: matched per ha: accepted 609.3 vs 638.1 = 0.955; promotions 39 (171.9/ha) vs 37 (146.5/ha) = 1.173. Whole stand: accepted 0.996, promotions 75/37 = 2.027 (interior 11/11, matched edge 28/26, 2.5 m-only blocks 36)
S5-07A no work seed 20: matched per ha: accepted 604.8 vs 633.9 = 0.954; promotions 39 (171.9/ha) vs 37 (146.5/ha) = 1.173. Whole stand: accepted 0.996, promotions 76/37 = 2.054 (interior 11/10, matched edge 28/27, 2.5 m-only blocks 37)
```

The same run prints the 40 `S5-07B model light` per-seed lines in the same format; their summaries are in §4.

---

PR none, head (this handoff commit on `feat/multi-stage5-sitka-regeneration-20261123-v1`), checks and reviews not run (delta re-review pending)

---

## Addendum (2026-11-24) — owner record; read before quoting this handoff

This handoff is kept unchanged above as history. Three of its statements are now qualified by the owner record of 2026-11-24 (`MULTI-STAGE5-SITKA-REGENERATION-OWNER-RECORD-20261124.md`, spec third amendment record):
- **"Per hectare of matched in-polygon area"** is a whole-cell, nominal-grid-area unit, not forest area. The owner kept it as the gate and rejected the clipped and same-true-ground alternatives.
- **The `S5-07A` pass is aggregate-only.** It is an owner-accepted limitation that matched edge blocks are not equivalent (1.902 / 1.841; all 40 seed-runs above 1.33). The pass is not an edge-equivalence claim.
- **"Written into the spec before this pass measured anything"** (§0 summary, §2 D2) must not be read as "owner-set in advance". The implementing worker chose the `S5-02` margins after reading the previous handoff's 3.24 m and 7/12 figures, and they became the owner's binding margins only on 2026-11-24.

The "matched raw-count ratio" in the summary and §3.2 is the same-true-ground per-hectare ratio.
