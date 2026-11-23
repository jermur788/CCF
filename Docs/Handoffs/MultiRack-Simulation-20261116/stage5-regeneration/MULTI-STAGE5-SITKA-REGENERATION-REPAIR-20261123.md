# MULTI Stage 5 — Option A amendment and bounded repair: handoff

**State: IMPLEMENTED ON TASK BRANCH — STOP CONDITION TRIGGERED (`S5-07A` NOT MET) — OWNER DECISION REQUIRED. NOT NUMERICALLY QUALIFIED. NOT VERIFIED, NOT INTEGRATED, NOT ACCEPTED. Delta re-review pending.**

**In one paragraph.** The owner's Option A is now in the spec and in the code, exactly as specified. A promotion now takes the whole cohort of its 5 m block, and edge blocks get an area-proportional budget. The 5.0 m grid is bit-identical to the first pass, and that is proved against a fingerprint measured on the old engine. The new `S5-07A` gate proves that both grids read identical light in every year, and accepted abundance passes it (0.961, 0.994). **Promotions do not pass:** the pooled/mean ratio is 1.575/1.575 under managed thinning and 2.008/2.009 under no work, against `[0.75, 1.33]`. Interior blocks pass (1.146 and 1.075). The excess comes from 12 boundary blocks that exist on the 2.5 m grid but have no 5.0 m cell at all. The area budget almost never binds there, because blocks run out of cohorts before they run out of budget. Per the packet's stop rule I changed nothing further: the tolerance, the rule and the control are untouched. The `S5-07A` test is committed **red on purpose**, so the unmet gate stays visible.

---

## 1. Identity, SHAs and links

| Item | Value |
|---|---|
| Worker | Claude Code (Claude Opus 5.5, `claude-opus-5-5`) in T3 Code on the owner's Windows host; the lane is in WSL Ubuntu. No subagents |
| Role | Sole implementation writer for the bounded Option A repair |
| Repair base | `cb739cbde06d293aa76ac1b0d4dc14b76ed02489` (clean, equal to origin at start) |
| **Spec amendment (docs branch)** | `c71f3a0f37a3d28f59dcf7ba69daf22b23c667d6` on `docs/multi-stage5-sitka-regeneration-spec-20261123-v1` (parent `85d85a8d`). https://github.com/jermur788/CCF/commit/c71f3a0f37a3d28f59dcf7ba69daf22b23c667d6 |
| Amendment on the feature branch | `0bc5078d…` (cherry-pick `-x` of `c71f3a0f`; the four spec blobs are byte-identical to the docs branch) |
| **Code + tests commit** | `743b7d06…` — every number below was measured on this tree (`c09395f7…`) |
| Handoff commit | the commit that adds this file, on top of `743b7d06`; branch head = this commit |
| Feature branch | https://github.com/jermur788/CCF/tree/feat/multi-stage5-sitka-regeneration-20261123-v1 |
| Docs branch | https://github.com/jermur788/CCF/tree/docs/multi-stage5-sitka-regeneration-spec-20261123-v1 |
| Inputs read | First-pass handoff `MULTI-STAGE5-SITKA-REGENERATION-IMPLEMENTATION-20261123.md` (this folder); independent review on `review/multi-stage5-sitka-regen-20261123-v1` @ `77b29925` (findings F1–F11) |
| Coordination snapshot | `origin/main` = `37ceb264` at 2026-11-23T23:06Z. `active-tasks.md` blob `cf44a075…` is unchanged since 18:23Z, and no other writer is on `Tools/MultiRackSitkaP1Lab/src/Core/**` |

**Commit-message disclosure.** The first attempt at the code commit hit a shell-quoting error, so the commit message was cut off after its first lines and had no trailer. The commit was local and had never been pushed. I amended **only the message** (`git commit --amend -F`) and checked that the tree hash was unchanged (`c09395f7…` before and after). No published history was rewritten and no force push was made.

## 2. The rule change, in plain English

Both parts apply at Regeneration `1` only. Regeneration `0` is unchanged, and its pins prove it.

**A1 — block-cohort export.** When an aligned 5 m block promotes, its winning band becomes one tree, as before. Now the other cells of the same block also lose their band **of the same cohort**, and that abundance goes into the same tree. A **cohort is a band's `EstablishYear`.** Within one cell, the ported Unity `NormalizeBands` invariant keeps establishment years unique, so a cohort has at most one represented band per cell. Sub-threshold accumulators and other cohorts are untouched, and later cohorts can still promote in later years. The code is `Stage5Promotion.CohortBand` and `Stage5Annual.Promote`.

*No double counting.* Each band lives in one cell and is removed once. The account identity `starting − losses + accepted − export = ending` holds in every area-year: the worst residual is 8.3e-8 at 2.5 m and 1.6e-7 at 5.0 m, against a bound of 1e-4 (test P-07). The per-area cohort exports also equal the per-record totals.

**A2 — area-proportional block budget, realised as an integer block credit.** Let `C = (5/cell)²` (4 at 2.5 m, 1 at 5.0 m) and `n` = the in-polygon cells of the block. Each year:
1. `K ← K + n`;
2. if the block has a candidate and `K ≥ C`, it promotes (one tree at most) and `K ← K − C`;
3. `K ← min(K, C)`.

*Long-run rate:* with a candidate always present, exactly `n/C` promotions per year — every year for a full block, every 2nd year for n = 2, every 4th year for n = 1. In any `T` consecutive years a block promotes at most `1 + ⌊n·T/C⌋` times. A full block, and every 5.0 m cell, always holds `K ≥ C`, so its budget never binds. The arithmetic is integer, with no damping constant. The only number is `C`, and the grid fixes it. Test P-06 checks these schedules exactly against hand-derived expectations (17/18/19, 18/20/22, 20/24/28; after an idle spell, 41 then 44).

**Owner purity criterion (5.0 m).** All forty 5.0 m study runs (20 seeds × 2 scenarios × 30 years) reproduce the fingerprint **`c67edc6b…53d8`**. I measured that value on the unmodified engine at `cb739cbd` *before any edit*, using a temporary capture test that was never committed and was reverted.

## 3. `S5-07A` — BLOCKING — result: **FAIL (stop condition)**

**Control (spec §9 Q4, fixed before measurement).** For each scenario, the shared field is the **regeneration-free adult canopy**: the post-adult-step roster of a `1/1/1/0` run of the same woodland and scenario in that year. It is read with the unchanged kernel at every aligned 5 m block centre, and every cell of both grids reads its block's value.

*What is excluded, in both grids alike:* the shading of Stage 5 promoted trees on the regeneration layer. Each run keeps its own adult step, seed rain and promoted adults.

*Proof that the light is identical.*
- 121,200 coincident block-years were compared, and all are bit-equal (asserted).
- The test's year loop reproduces `MultiAreaContinuation.Advance` bit for bit under model light, at 2.5 m (managed thinning) and at 5.0 m (no work), over 30 years (asserted).
- The `1/1/1/0` reference starts from the same year-0 roster (asserted).

| Scenario | Metric | Pooled (Σ2.5 / Σ5.0) | Mean | Per seed min – median – max | Gate |
|---|---|---|---|---|---|
| managed thinning | accepted abundance | **0.961** (5028.5 / 5232.6) | **0.961** | 0.959 – 0.961 – 0.963 | pass |
| managed thinning | promotions | **1.575** (3543 / 2249) | **1.575** | 1.460 – 1.567 – 1.713 | **FAIL** |
| no work | accepted abundance | **0.994** (3199.2 / 3219.9) | **0.994** | 0.990 – 0.994 – 0.996 | pass |
| no work | promotions | **2.008** (1550 / 772) | **2.009** | 1.900 – 2.026 – 2.108 | **FAIL** |

The other A requirements pass:
- zero out-of-polygon trees over all 160 study runs;
- the 5.0 m purity pin matches;
- 5.0 m has `a = 1.0` with no edge-gate reduction (P-06 and the pin);
- no damping constant (by code reading: the only new number is `C`).

**Interior and edge sub-ratios, shared field (pooled; mean in brackets):**

| Scenario | Interior accepted | Interior promotions | Edge accepted | Edge promotions |
|---|---|---|---|---|
| managed thinning | 1.002 (1.002) | **1.146** (1.145) — 1414 / 1234 | 0.877 (0.877) | **2.098** (2.100) — 2129 / 1015 |
| no work | 1.004 (1.004) | **1.075** (1.077) — 258 / 240 | 0.975 (0.975) | **2.429** (2.430) — 1292 / 532 |

### 3.1 Why it fails (diagnostic, read-only)

I ran one extra diagnostic in a temporary test that was never committed; the file was restored byte for byte. It used the same shared-field runs. Lattice: 113 blocks at 2.5 m, of which 77 are interior. The 36 edge blocks have 3 in-polygon cells (8 blocks), 2 cells (23) or 1 cell (5). The 5.0 m grid has 101 cells, 77 of them in interior blocks.

| Managed thinning / no work | 2.5 m | 5.0 m |
|---|---|---|
| Edge blocks **that also have a 5.0 m cell** (24) | 1106 / 561 promotions | 1015 / 532 (24 edge cells) → ratio **1.09 / 1.05** |
| Edge blocks **with no 5.0 m cell** (12: their 5 m centre is outside the polygon) | **1023 / 731** promotions | none |
| Budget-withheld block-years at 2.5 m (out of 21,600 edge block-years) | **76 / 24** | — |
| Promotions per block per 30-year run, n = 1 / 2 / 3 | 4.51 / 2.75 / 2.58 (no work: 3.24 / 1.69 / 1.18) | 2.11 per edge cell (no work: 1.11) |

What the diagnostic shows:
- **The budget (A2) almost never binds.** Blocks run out of cohorts before they run out of budget. Cohort supply per edge block is about 0.1–0.25 per year, at or below the smallest budget of 0.25 per year. Only n = 1 blocks come near their cap (4.51 against about 5).
- **Where both grids represent a block, the grids agree:** 1.05–1.09 at the edge and 1.08–1.15 in the interior.
- **The residual is lattice coverage at the boundary.** Twelve blocks hold in-polygon 2.5 m cells, but the 5.0 m lattice has no cell there. Under the A control those cells read light at a block centre that lies **outside** the polygon, which the review already warned may be brighter (review §4.4). Those 12 blocks alone promote about as many trees as all 24 of the 5.0 m grid's edge cells.
- *Computed from these same runs; not a gate and not re-measured:* restricted to blocks present on both lattices, the pooled promotion ratio would be (1414 + 1106) / 2249 = **1.12** and (258 + 561) / 772 = **1.06**.

### 3.2 Owner decision needed (not implemented, not measured)

The tolerance, the rule and the control were not touched (stop rule). Options:

1. Define the `S5-07A` whole-stand comparison over the blocks represented on **both** lattices. The estimate above is 1.12 / 1.06 pooled; the means were not computed.
2. Change the A control for blocks with no coincident point, for example sampling at the cell's own centre. This is a control-definition change made after seeing data, so it is a G3 matter for the owner.
3. Change the rule, for example a per-cohort fractional export for edge blocks, or no promotion budget for blocks whose 5 m centre is outside every polygon. This would be new rule text.
4. Run Stage 5 regeneration on the 5.0 m grid only.

*Implementer's view, labelled as such:* the evidence points to option 1 or 2. Where both grids hold the block, the promotion policy is already near parity, and the remaining gap is a sampling and coverage artefact of the boundary, not the policy. This is the owner's call.

## 4. `S5-07B` — MANDATORY, NON-BLOCKING DIAGNOSTIC (each grid's own model light)

There is no threshold and no assertion; it passed as a diagnostic.

| Scenario | Metric | Pooled | Mean | Per seed min – median – max |
|---|---|---|---|---|
| managed thinning | accepted abundance | 0.896 (4157.4 / 4638.1) | 0.896 | 0.880 – 0.893 – 0.917 |
| managed thinning | promotions | 1.654 (2530 / 1530) | 1.655 | 1.562 – 1.665 – 1.789 |
| no work | accepted abundance | 0.882 (2592.8 / 2938.7) | 0.882 | 0.857 – 0.882 – 0.899 |
| no work | promotions | 2.124 (1268 / 597) | 2.147 | 1.800 – 2.068 – 2.652 |

| Real light, pooled (mean) | Interior accepted | Interior promotions | Edge accepted | Edge promotions |
|---|---|---|---|---|
| managed thinning | 1.007 (1.007) | 1.749 (1.751) | 0.656 (0.656) | 1.535 (1.539) |
| no work | 1.022 (1.023) | 3.418 (3.505) | 0.649 (0.649) | 1.501 (1.517) |

**Statement (as the spec requires).** Light is the modelled index `unity-k10a10-decoupled-v1`, not calibrated PAR. Under real light, each grid samples its own canopy at its own cell centres, so the 2.5 m grid resolves gaps more finely. A higher 2.5 m real-light recruitment reflects that finer gap-resolution sampling, not a scaling defect.

*Precision, to avoid over-claiming:* the difference between A and B combines two things. One is cell-centre against block-centre sampling. The other is the run's own canopy, including its own promoted adults, against the shared regeneration-free canopy. I did not separate the two. The interior real-light excess (1.75 / 3.42) is the same "finer sampling" effect the review measured (F2). Note also that the 5.0 m model runs reproduce the first-pass counts exactly (for example seed 1: 76 and 30), as the purity pin implies.

## 5. Before and after

| Ratio, pooled | First pass `cb739cbd` (old rule) | This repair `743b7d06` (Option A) |
|---|---|---|
| Promotions, model light (thinning / no work) | 2.072 / 2.412 | 1.654 / 2.124 (S5-07B) |
| Promotions, shared control (thinning / no work) | 2.652 / 3.467 *(old control: block-centre sampling of each run's own field)* | 1.575 / 2.008 (S5-07A, *new control: shared regeneration-free canopy*) |
| Accepted abundance, model light | 0.827 / 0.860 | 0.896 / 0.882 |
| Accepted abundance, shared control | 0.846 / 0.921 *(old control)* | 0.961 / 0.994 *(new control)* |

The two shared-control rows use different control definitions (review F10), so they are not a like-for-like pair.

## 6. Findings F3–F11: dispositions

| # | Disposition | Evidence |
|---|---|---|
| F1 | Option A implemented; `S5-07A` **still fails** on promotions (§3) | `S5-07A G-01` red |
| F2 | Confirmed and refined. Under the shared field the interior is in tolerance (1.15 / 1.08); the residual is the 12 boundary blocks with no 5.0 m cell (§3.1) | diagnostic |
| F3 | **Documented and asserted.** `Stage5Cell.Light` is the light the year's rules used: the post-adult-step woodland, without that year's new adults and without later Fells. Over a 30-year thinned run it equals the canopy read-out in all 12 years without a promotion. In all 18 promotion years it equals the light of the committed roster without the new adults; 581 cell-years differ from the canopy read-out, by design | `S5-08 P-05`; XML doc on `Stage5Cell` |
| F4 | **Changed, and now fails, honestly.** The check uses each dense cell's own **year-12** light, the year in which suppression is compared. **7 of 12** dense cells are in the poor-light band. Their mean light that year is 0.134 (LR 0.168; poor-light needs LR < 0.15) and the maximum is 0.441. 12 of 12 are poor-light in years 1–2, 11 of 12 in years 3–4, 7 of 12 in years 5–14 and 6 of 12 in year 15, as Area A self-thins. Every comparative S5-02 criterion passes: the interior never reaches 3.5 m or promotes, and it is shorter and sparser than the gap in year 12; the gap reaches 3.24 m. **Owner decision:** is "suppressed" absolute (then S5-02 fails in this geometry) or comparative? | `S5-02 G-01` red: "7/12 are" |
| F5 | **Removed from production options.** `Stage5LightSampling` and `Stage5Options.LightSampling` are deleted. The control now uses an explicit-light overload of the pure `Stage5Annual.Step`; sessions call only the model-light overload. *Disclosure:* the overload is `internal` and therefore technically callable from the UI assembly (`InternalsVisibleTo("sitkap1-ui")`), but no option, session or save path can select it, and calling it changes no session | `Stage5Models.cs`, `Stage5Annual.cs` |
| F6 | `(Column, Row)` tie-break tested both in the engine (the loser comes first in lattice order) and on the key directly with reversed input. All-racks-empty year tested (advances, no seed, promotes in year 13, then the adult step resumes; refused when there is no abundance). The three missing refusal selections (`0/1/0`, `0/1/1`, `1/0/0`) added. *Not added:* a dedicated unit test for an A/B-straddling block; the code groups woodland-wide, and the MA-1 study exercises it | `S5-08 P-04`, `S5-09 E-02`, `S5-01 F-03` |
| F7 | Spec Q4 rewritten: 30-year horizon, and the stale "10 years" is marked superseded | `c71f3a0f` |
| F8 | Recorded explicitly: the first approval corrected the §4 block-winner order to owner Q5; the draft said the opposite | `c71f3a0f` |
| F9 | Unchanged (float-order interpretations); out of scope | — |
| F10 | Closed by the new control: the light *field* is identical in every year, not only the sampling position, and this is asserted | §3 |
| F11 | The suite still exits non-zero: 2 deliberate reds (`S5-07A`, `S5-02`) | §8 |

## 7. G3 (measurement-integrity) items I touched

1. **`S5-07` → `S5-07A` / `S5-07B`** (owner definition). The A gate is now **pooled and mean** in `[0.75, 1.33]`, as the owner defined it; the first pass required every seed to be inside. The per-seed spread is still printed. B has no assertion.
2. **Shared-light control redefined** (F10). It is now the regeneration-free `1/1/1/0` canopy at block centres, chosen and written into the spec (`c71f3a0f`) *before* the first measurement. The test runs Stage 5 through a test-side year loop that calls the same production functions as `Advance`, and that loop is proved bit-identical to `Advance` under model light. *Known property:* the 12 boundary blocks with no 5.0 m cell read light at a block centre outside the polygon (§3.1). I did not change this after seeing the data.
3. **Horizon:** 30 years, unchanged, and now written into the spec (F7).
4. **S5-02 poor-light criterion** changed from the 15-year mean to each dense cell's own year-12 light (F4). After the first run I moved this assertion to the end of the test, so that the comparative criteria are still evaluated and shown. The criterion itself did not change.
5. **S5-08 P-02 expectations changed by the approved rule.** The year-3 sibling now leaves with the winner (cohort export 1, export 0.1 + 0.1). Deferred cells go from 2 to 1. Year 21 promotes the younger year-4 cohort, and year 22 promotes nothing.
6. **`BlockCapDeferredCells` semantics** are now "qualifying band neither promoted nor exported". The value is unchanged at 5.0 m; the purity pin proves it.
7. **`1/1/1/1` fingerprint re-recorded:** `b48aa8fc…` → `c4b670ed…`. The recipe is unchanged; the fingerprint text also carries the Option A fields (`WriteAmendment`). It is recorded, not pinned.
8. **The "5.0 m control equals 5.0 m model" check** is replaced by the stronger **5.0 m purity pin** `c67edc6b…`, measured at `cb739cbd`.
9. **New tolerance in P-07:** an abundance residual of at most 1e-4 per area-year. The derivation: losses are exact float differences (Sterbenz), so only merge rounding remains, at about 0.5 ulp × a few merges × at most 383 cells, which is below 3e-5. One mis-booked band would be at least 2.5e-3. Observed: 8.3e-8 and 1.6e-7.
10. **Strengthened:** the three extra refusal selections in F-03.

## 8. Hashes, build and tests (code tree `743b7d06`)

| Anchor | Expected | Observed |
|---|---|---|
| Single-area Model 0 | `7e3a2a86…` | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` ✓ |
| Multi-area Model 0 | `a692a6d5…` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` ✓ |
| Stage 3 C2 `0/0/1` | `814e8165…` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` ✓ |
| Experiment | `c66d566c…` | `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` ✓ |
| P0 pin (`sha256sum .p0ref/SitkaLab.Core.dll`) | `9a99f781…` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` ✓ |
| `1/1/1/0` full run | `8a926776…` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` ✓ (default recipe, explicit `1/1/1/0`, and `1/1/1/1` over 10 years) |
| 5.0 m purity (new pin) | `c67edc6b…` (measured at `cb739cbd`) | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` ✓ |
| `1/1/1/1` full run (re-recorded) | — | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` (two runs identical, S5-10 D-01) |

- **Build:** solution `--no-incremental` gives **0 warnings, 0 errors** (`TreatWarningsAsErrors`). `src/Ui/SitkaP1.Ui.csproj` also gives 0 / 0; it was compiled only, not run.
- **Tests:** **206 / 208 passed.** All 184 pre-existing tests pass, and 22 of the 24 Stage 5 tests pass. The two **deliberate reds** are `S5-07A G-01` (the stop condition) and `S5-02 G-01` (the F4 finding). Runtime is about 40 s.
- **New tests:** `S5-07A G-01`, `S5-07B G-02`, `S5-08 P-04` / `P-05` / `P-06` / `P-07`, `S5-09 E-02`. Removed: the first-pass `S5-07 G-01`.
- **Hygiene:** `git status --short` is empty after the commit. The diff `cb739cbd..743b7d06` touches 5 `src/Core/MultiArea` files, 3 `tests/Core.Tests` files and the 4 spec files. Nothing is under `Assets/`, `*.meta`, `ProjectSettings/`, `src/Ui`, wwwroot, `.sln`, `.csproj`, `.p0ref` or a save schema.

## 9. Exact commands (WSL, from the lane)

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage5-SitkaRegen-20261123/Tools/MultiRackSitkaP1Lab
sha256sum .p0ref/SitkaLab.Core.dll
TMPDIR=/tmp/stage5-repair /home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers --no-incremental
TMPDIR=/tmp/stage5-repair /home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
TMPDIR=/tmp/stage5-repair /home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers   # compile only
```

The order of work, for the record:
1. Baseline capture on `cb739cbd` (a temporary test, reverted).
2. Spec amendment in a temporary docs worktree (`/tmp/stage5-spec-amend`, removed afterwards); pushed `c71f3a0f`; cherry-picked as `0bc5078d`.
3. Code.
4. **One** measurement run.
5. One read-only diagnostic (a temporary test, reverted).
6. Moved the S5-02 assertion to the end.
7. Final evidence run.
8. Commit.

## 10. Not verified

- Unity runtime parity of any Stage 5 rule. Nothing was run in Unity, and no Unity file, process or lock was touched.
- Ecological validity. Every constant is gameplay calibration or transfer, and light is not PAR.
- How the mean ratios would look under option 1 of §3.2. Only pooled estimates were computed, from the same runs.
- Any alternative control or rule. None was run, by the stop rule.
- UI runtime and `/multi`: the UI project was compiled, not run. No listener was started.
- The owner's own words for Option A: taken from the task packet.
- Windows `git.exe` (not used, by instruction), CI, other operating systems, and GitHub rendering of this file.
- Cross-process determinism beyond the identical hashes from separate runs on this host.

## Appendix — per-seed output (verbatim, final run on `743b7d06`)

```text
S5-07A managed thinning seed  1: accepted 2.5/5.0 0.961; promotions 177/109 = 1.624 (interior 68/61, edge 109/48)
S5-07A managed thinning seed  2: accepted 2.5/5.0 0.963; promotions 197/115 = 1.713 (interior 76/62, edge 121/53)
S5-07A managed thinning seed  3: accepted 2.5/5.0 0.959; promotions 185/113 = 1.637 (interior 74/62, edge 111/51)
S5-07A managed thinning seed  4: accepted 2.5/5.0 0.961; promotions 178/107 = 1.664 (interior 72/59, edge 106/48)
S5-07A managed thinning seed  5: accepted 2.5/5.0 0.959; promotions 179/116 = 1.543 (interior 71/64, edge 108/52)
S5-07A managed thinning seed  6: accepted 2.5/5.0 0.961; promotions 185/114 = 1.623 (interior 71/61, edge 114/53)
S5-07A managed thinning seed  7: accepted 2.5/5.0 0.961; promotions 181/124 = 1.460 (interior 71/66, edge 110/58)
S5-07A managed thinning seed  8: accepted 2.5/5.0 0.960; promotions 155/101 = 1.535 (interior 60/57, edge 95/44)
S5-07A managed thinning seed  9: accepted 2.5/5.0 0.963; promotions 175/116 = 1.509 (interior 73/64, edge 102/52)
S5-07A managed thinning seed 10: accepted 2.5/5.0 0.962; promotions 152/102 = 1.490 (interior 62/57, edge 90/45)
S5-07A managed thinning seed 11: accepted 2.5/5.0 0.961; promotions 183/113 = 1.619 (interior 76/62, edge 107/51)
S5-07A managed thinning seed 12: accepted 2.5/5.0 0.961; promotions 163/107 = 1.523 (interior 66/61, edge 97/46)
S5-07A managed thinning seed 13: accepted 2.5/5.0 0.962; promotions 167/112 = 1.491 (interior 64/60, edge 103/52)
S5-07A managed thinning seed 14: accepted 2.5/5.0 0.961; promotions 194/123 = 1.577 (interior 75/65, edge 119/58)
S5-07A managed thinning seed 15: accepted 2.5/5.0 0.961; promotions 182/111 = 1.640 (interior 74/62, edge 108/49)
S5-07A managed thinning seed 16: accepted 2.5/5.0 0.961; promotions 175/109 = 1.606 (interior 70/60, edge 105/49)
S5-07A managed thinning seed 17: accepted 2.5/5.0 0.959; promotions 182/118 = 1.542 (interior 74/65, edge 108/53)
S5-07A managed thinning seed 18: accepted 2.5/5.0 0.961; promotions 160/104 = 1.538 (interior 63/57, edge 97/47)
S5-07A managed thinning seed 19: accepted 2.5/5.0 0.961; promotions 176/113 = 1.558 (interior 76/63, edge 100/50)
S5-07A managed thinning seed 20: accepted 2.5/5.0 0.962; promotions 197/122 = 1.615 (interior 78/66, edge 119/56)
S5-07A no work seed  1: accepted 2.5/5.0 0.993; promotions 77/38 = 2.026 (interior 14/12, edge 63/26)
S5-07A no work seed  2: accepted 2.5/5.0 0.994; promotions 77/38 = 2.026 (interior 13/11, edge 64/27)
S5-07A no work seed  3: accepted 2.5/5.0 0.994; promotions 78/38 = 2.053 (interior 14/12, edge 64/26)
S5-07A no work seed  4: accepted 2.5/5.0 0.993; promotions 75/39 = 1.923 (interior 13/13, edge 62/26)
S5-07A no work seed  5: accepted 2.5/5.0 0.993; promotions 76/40 = 1.900 (interior 13/13, edge 63/27)
S5-07A no work seed  6: accepted 2.5/5.0 0.995; promotions 80/39 = 2.051 (interior 14/13, edge 66/26)
S5-07A no work seed  7: accepted 2.5/5.0 0.992; promotions 79/40 = 1.975 (interior 13/12, edge 66/28)
S5-07A no work seed  8: accepted 2.5/5.0 0.995; promotions 83/42 = 1.976 (interior 13/13, edge 70/29)
S5-07A no work seed  9: accepted 2.5/5.0 0.994; promotions 77/39 = 1.974 (interior 13/12, edge 64/27)
S5-07A no work seed 10: accepted 2.5/5.0 0.995; promotions 71/37 = 1.919 (interior 13/12, edge 58/25)
S5-07A no work seed 11: accepted 2.5/5.0 0.990; promotions 78/39 = 2.000 (interior 13/12, edge 65/27)
S5-07A no work seed 12: accepted 2.5/5.0 0.994; promotions 77/40 = 1.925 (interior 12/12, edge 65/28)
S5-07A no work seed 13: accepted 2.5/5.0 0.994; promotions 80/38 = 2.105 (interior 15/12, edge 65/26)
S5-07A no work seed 14: accepted 2.5/5.0 0.993; promotions 73/38 = 1.921 (interior 12/12, edge 61/26)
S5-07A no work seed 15: accepted 2.5/5.0 0.994; promotions 78/37 = 2.108 (interior 11/10, edge 67/27)
S5-07A no work seed 16: accepted 2.5/5.0 0.990; promotions 80/38 = 2.105 (interior 13/12, edge 67/26)
S5-07A no work seed 17: accepted 2.5/5.0 0.992; promotions 81/40 = 2.025 (interior 13/13, edge 68/27)
S5-07A no work seed 18: accepted 2.5/5.0 0.994; promotions 79/38 = 2.079 (interior 14/13, edge 65/25)
S5-07A no work seed 19: accepted 2.5/5.0 0.996; promotions 75/37 = 2.027 (interior 11/11, edge 64/26)
S5-07A no work seed 20: accepted 2.5/5.0 0.996; promotions 76/37 = 2.054 (interior 11/10, edge 65/27)
```

The S5-07B per-seed lines (40) are printed by the same run in the same format. Their summaries are in §4, and the 5.0 m promotion counts equal the first-pass appendix exactly (thinning 76, 79, 81, 74, 79, … ; no work 30, 27, 30, 33, 35, …).

---

PR none, head (this handoff commit on `feat/multi-stage5-sitka-regeneration-20261123-v1`), checks and reviews not run (delta re-review pending)
