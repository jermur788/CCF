# Multi #885 MA-1 — Q0 equation, state and research crosswalk (candidate `4cecf873`)

**Stage Q0 of the MA-1 multi-year scientific qualification. GitHub/Git read-only inventory. This file is not a code change, not an engine run, not a review and not an acceptance.**

## 0. Stage labels, scope and custody

| Label | Status |
|---|---|
| **Q0 COMPLETE** | **YES**: read-only equation/parameter/state inventory and research crosswalk, delivered in this file |
| **Q1+Q2 EXECUTED** | **NO**. Q1 is BLOCKED: there is no D-091 custodian receipt and no qualification lease. No engine experiment was run. This file contains no engine output. |
| **Q4 REVIEWED** | **PENDING**. No independent review has happened yet. |
| **MODEL CALIBRATED** | **NOT CLAIMED** |
| **OWNER ACCEPTED** | **NOT CLAIMED** |
| **G1 ecological validation** | **UNKNOWN** |

| Item | Value |
|---|---|
| Candidate source (all code anchors) | `4cecf87339bea4d32907955a8d5625d1f27f96bd` ("fix(ui): history heading says what actually ran for an empty area (review N1)", 2026-11-21 02:51:19 +0000) |
| Provider / model | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code running as a T3 Code child thread. I am the single Q0 worker. The orchestrator is DeepSeek V4.1 Flash; it does not implement. No subagents were used. |
| Session | Claude Code session `73d21cf6-37a2-405a-9509-ef3c72b56ba0`. The T3 child-thread id was not visible to me. |
| Report lane | branch `docs/multi-885-q0-equation-crosswalk-20261121-v1`, sole path `Docs/Handoffs/MultiRack-Simulation-20261116/885-q0-qualification-20261121/Q0-EQUATION-AND-RESEARCH-CROSSWALK.md` |
| Branch base (`main` at publication) | `3a67d0987be0cf40d007892739d2ad295c84f0e0` |
| Date | 2026-11-21 |
| Method | `git -C /home/jer/Development/CCF-HP show 4cecf873:<path>` and `git ls-tree` on the local clone (read only), `gh api` for the #885 next-action packets. I created no worktree and ran no build, test, Unity, server or engine. I made no write in any Git worktree: scratch copies live in `%TEMP%\opencode\q0`, outside Git. |
| Governing packet | `01-MA1-MULTIYEAR-QUALIFICATION-DEEPSEEK-ORCHESTRATION-PACKET.md` (branch `docs/multi-885-next-action-orchestration-research-packets-20261121-v1`, head `7a2a13fa8ab850571fb40868fb2ae7f188922b9e`), plus packets 02 and 03 in the same folder |

**Path abbreviations.**
- `LAB/` = `Tools/MultiRackSitkaP1Lab/` at `4cecf873`.
- `CCF/` = `Assets/ForestPrototype/` at `4cecf873`.
- `RES/` = `Docs/Research/` at `4cecf873`.
- `SGM/` = `Docs/Research/SitkaGrowthMortality/` at `4cecf873`.

Line numbers are those of the blob at `4cecf873`.

**Linked source integrity (verified by reading).** `LAB/src/Core/SitkaP1.Core.csproj:27-28` compiles `CCF/SitkaGrowthModel.cs` and `CCF/SimulationRandom.cs` unchanged into the lab. Their blob ids at `4cecf873` are `e4c73402…` and `bb83d7f8…`. Both equal the pins in `LAB/src/Core/CcfSourcePins.cs:16-19`. `SitkaSpruce.asset` (`e638e28e…`) and `TreeSpeciesDefinition.cs` (`51faaa0d…`) also equal their pins (`CcfSourcePins.cs:20-23`).

**Unavailable inputs.** Three plain-text research returns were uploaded to the conversation, not to the repository:
- *Evidence on Irish Sitka CCF thinning and transformation* (Teagasc Frenchpark, Page et al. 2001, Stokes 2009, CONTINUCOVER);
- *MA-2 Comparable-Area Prescription Transfer*;
- *Multi-Area Sitka Spruce Data Assessment*.

**These are NOT in the repository tree and were UNAVAILABLE to this worker.** I did not read, reconstruct or paraphrase them. Every claim that would depend on them is marked **UNKNOWN**. `git grep` over `Docs/` at `4cecf873` finds **no** mention of Page et al. 2001, Stokes 2009 or CONTINUCOVER. The only repository description of them is one sentence each in packet 02 (§"Input" and R1), which is a packet summary, not source evidence.

---

## 1. What the engine is (one paragraph)

MA-1 is an in-memory, session-only, deterministic simulator of **adult Sitka spruce only**, over two synthetic authored polygons (MA-A, MA-B). Its pieces:
- **Annual step.** Each living area runs the frozen single-area annual function `AnnualRules.Calculate` once per shared ecological year, with its own trees, seed and hectares (`LAB/src/Core/MultiArea/MultiAreaContinuation.cs:340-405`, `MultiAreaAnnual.Step` at `:29-37`).
- **Per-tree biology.** Hegyi competition → DBH growth, Class III height-envelope tracking and crown relaxation, then stand relative density on the British maximum size–density line → stochastic suppression-weighted self-thinning plus a deterministic boundary at RD 1.
- **Player work.** One typed intervention exists: **Fell** of named living trees through a Work Plan. The plan goes pending → approved (0 Fell) → executed (Fell) and is published atomically across both areas.
- **Out of scope.** There is no recruitment, no light, no storm, no browsing, no volume or economics, and no cross-area ecology.

---

## 2. Variable / parameter / equation / unit inventory

### 2.1 Per-tree state (`AnnualTree`)

| Variable | Type / unit | Where defined | How it changes | Notes |
|---|---|---|---|---|
| `TreeId` | string, case-sensitive ordinal | `LAB/src/Core/AnnualEngine.cs:9` | never | MA IDs are `MA-A-nnn` / `MA-B-nnn`. The prefix and area are enforced in `MultiAreaExperiment.cs:88-97`. |
| `XM`, `ZM` | float32 m | `AnnualEngine.cs:9-10` | never: trees do not move | MA genesis uses `(float)(mm/1000.0)` (`MultiAreaExperiment.cs:116-123`) |
| `AgeYears` | int, years | `AnnualEngine.cs:9` | `+1` per year, `checked` (`AnnualRules.cs:99`) | Genesis range is 1..10000 (`MultiAreaExperiment.cs:101`). It is checked at genesis only, not afterwards. |
| `DbhCm` | float32 cm | `AnnualEngine.cs:10` | `AnnualRules.Grow` (`:88-91`), clamped to [1, 200] | Genesis value is `(float)(dbhMm/10.0)` (`DerivedManifest.cs:77`) |
| `HeightM` | float32 m | `AnnualEngine.cs:10` | `SitkaGrowthModel.NextHeight` (`AnnualRules.cs:92-93`), clamped to [0.1, 60] | Genesis value is `(float)(heightMm/1000.0)` (`DerivedManifest.cs:78`) |
| `CrownRadiusM` | float32 m | `AnnualEngine.cs:10` | relaxation (`AnnualRules.cs:94-98`) | Genesis value is the linear potential at CI = 0 (`DerivedManifest.cs:80-85`). **Not read by competition, growth or mortality.** It is output and display only. |
| `Death` | `AnnualDeath(TreeId, EcologicalYear, Cause, Mechanism)` or null | `AnnualEngine.cs:7,10-12` | set once and never cleared | Causes: `self-thinning`/`hazard` (`AnnualRules.cs:49`), `self-thinning`/`boundary` (`:138`), `fell`/`manual-work-plan` (`WorkContract.cs:50-54`). A dead tree's last post-growth fields stay frozen in the roster (`AnnualRules.cs:62-63`). |

There is **no** per-tree crown ratio, vigour, suppression-history, seed, damage, stem-quality, volume or value field.

### 2.2 Per-area and experiment parameters

| Quantity | Value / rule | Anchor |
|---|---|---|
| Area polygons | integer-mm, strictly convex CCW. MA-A is a pentagon of **915 m²**; MA-B is a quadrilateral of **1500 m²**. They share one edge. | `MultiAreaGenerator.cs:38-51`; exact areas asserted in `LAB/tests/Core.Tests/MultiAreaTests.cs:64-65` |
| `areaHa` | `(float)((double)twiceShoelaceMm2/2e10)`. This is the denominator for N/ha, BA/ha and RD. | `MultiAreaGeometry.cs:130`; `MultiAreaExperiment.cs:45` |
| Trees at genesis | MA-A 171, MA-B 73 (test assertion) | `MultiAreaTests.cs:68` |
| Cohorts ("strata") | **Generator-only.** MA-A is one cohort aged 24. MA-B is 70 % aged 47 and 30 % aged 29. **The engine has no stratum or cohort variable**: after genesis a cohort is only each tree's own `AgeYears`. | `MultiAreaGenerator.cs:42,47-51`; `MultiAreaTests.cs:71-72` |
| Per-area simulation seed | MA-A `885101`, MA-B `885202`, unique and fixed in the experiment definition. The generator seed `8850001` is separate. | `MultiAreaGenerator.cs:28,41,46`; uniqueness at `MultiAreaExperiment.cs:81` |
| Site | Reported as `UNKNOWN`. The engine uses constant `siteProductivity = 1.0f` for every area. | `MultiAreaExperiment.cs:20,28-29`; `DerivedManifest.cs:50` |
| Edge model | "isolated per area": neighbours across the boundary are excluded, with no buffer or edge correction | `MultiAreaExperiment.cs:25-27` |
| Fell eligibility DBH threshold | 170 mm carried as **metadata only**. It does not restrict Fell (D-093). | `MultiAreaExperiment.cs:34`; `MultiAreaContracts.cs:152` |
| Excluded systems (declared) | browsing, canopyLight, juvenileLifecycle, mixedSpecies, pruning, recruitment, regeneration, storm | `DerivedManifest.cs:58-59` |
| Author-reported genesis stand values (author console print, **not re-verified by me**) | MA-A: 1,868.9 stems/ha, BA 74.02 m²/ha, Dq 22.46 cm, RD 0.802. MA-B: 486.7 stems/ha, BA 39.50 m²/ha, Dq 32.15 cm, RD 0.438. | MA-1 author handoff `Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-MA1-TWO-AREA-SIMULATOR-SOURCE-IMPLEMENTATION-HANDOFF-20261121.md` §1.1. The tests assert only the inequalities "A RD > 0.6 > B RD" (`MultiAreaTests.cs:77-78`). |

### 2.3 Annual equations (in execution order), `LAB/src/Core/AnnualRules.cs`

| # | Step | Equation (as coded) | Units | Anchor | Constant status (from research docs) |
|---|---|---|---|---|---|
| E1 | Year | `year = EcologicalYear + 1` (checked) | yr | `:15` | — |
| E2 | Living set | living trees, ordinal by `TreeId` | — | `:16` | — |
| E3 | Hegyi CI (pre-growth, area-local) | `CI_i = Σ_{j≠i, d_ij ≤ 8 m} (D_j / max(1, D_i)) / max(0.5, d_ij)` | dimensionless | `:67-82`; constants `:6-7` | Method [B]; 8 m cutoff [C] (`SGM/EvidenceMatrix.csv` row 16) |
| E4 | Growth response | `r_i = 1 / (1 + CI_i / 5)` | — | `:88`; `Ci50 = 5` `:8` | Response form and CI50 are abstraction/calibration (`RES/CCF_Growth_Density…Composite.md` §5) |
| E5 | DBH increment | `ΔD = 1.2 · site · clamp(1 − D/120, 0, 1) · r_i`, then `D ← clamp(D+ΔD, 1, 200)` | cm/yr | `:89-91`; constants `:9-10` | 1.2 cm/yr [C], evidence range 0.9–1.5 (EvidenceMatrix row 10). `site` = 1.0f (`DerivedManifest.cs:50`). **Age does not enter DBH growth.** |
| E6 | Height | `H ← clamp(H · E(max(1,age)+1) / E(max(1,age)), 0.1, 60)`, with `E(t) = b0 (1 − e^{−0.042 t})^{1.563}` and `b0 = 20.4 / (1 − e^{−0.042·30})^{1.563}` | m | `AnnualRules.cs:92-93`; `CCF/SitkaGrowthModel.cs:37-60` | Lekwadi et al. 2012 Class III, b2/b3 [A], b0 anchor-matched to 20.4 m [A/C] (`SGM/SitkaGrowthModel1.md` lines 31-33). **No competition term; no site term.** |
| E7 | Crown | `R_pot = max(0.1, 0.9415 + 0.07635·D)`, `R_target = R_pot · r_i`, `R ← max(0.1, R + 0.15 (R_target − R))` | m | `:94-98`; `CcfSourcePins.cs:28-30` | Tabbush & White envelope [B] (EvidenceMatrix row 13). Relaxation 0.15 lies inside the composite's 0.1–0.2/yr starting range (§5). Competition reduction is a model choice [D]. |
| E8 | Age | `age ← age + 1` | yr | `:99` | — |
| E9 | Stand aggregates (post-growth) | `N/ha = count/areaHa`; `Dq = sqrt(ΣD²/count)`; `BA/ha = ΣD² · π/40000 / areaHa` | stems/ha, cm, m²/ha | `:102-118` (duplicated byte-for-byte in arithmetic as `WorkRules.Summarize`, `WorkContract.cs:122-138`) | `count/areaHa` is evaluated as int÷float32, then widened (`:113-114`) |
| E10 | Relative density | `RD = N/ha · (Dq/25)^{2.063} / 1868` | dimensionless | `SitkaGrowthModel.cs:67-75` | Comeau et al. 2010, British [B]. Irish transfer unverified (EvidenceMatrix row 23; `SGM/AdultMortalityCalibration.md` lines 15-17). |
| E11 | Density pressure | `P = max(0, (RD − 0.6)/0.4)`, using **post-growth** RD | — | `AnnualRules.cs:19-20`; `SitkaGrowthModel.cs:89,98-101` | onset 0.6 [C] (row 24) |
| E12 | Suppression (only if P > 0) | `S_i = 1 − 1/(1 + CI_i/5)`, using **pre-growth** CI | — | `AnnualRules.cs:27-33` | — |
| E13 | Hazard | `p_i = min(0.5, 0.08 · P² · S_i⁴ / mean(S⁴))` | /yr | `AnnualRules.cs:39-40`; `SitkaGrowthModel.cs:90-96,103-114` | strength 0.08 [C], exponent 4 [C], cap 0.5 numerical (rows 25-26) |
| E14 | Mortality draw | Drawn only when `p_i > 0`: `u = SimulationRandom.Roll(model 1, "ADULT-MORT-"+TreeId, year, areaSeed)`. The tree dies if `u < p_i`. | uniform [0,1) with 24-bit resolution | `AnnualRules.cs:41-53`; `CCF/SimulationRandom.cs:41-55` | — |
| E15 | Boundary (only if P > 0 and survivors > 1) | While survivor RD > 1, kill the survivor with the highest pre-growth S (ties by ordinal ID). The last survivor is always kept. | — | `AnnualRules.cs:58-59,120-142` | RD ≤ 1 is the maximum line [B] |
| E16 | Next state | dead trees carried forward unchanged; report holds Before / PostGrowth / After stands and a per-tree audit | — | `:60-64`; `MultiAreaContinuation.cs:33-35` | — |

### 2.4 Unit, numeric and fixed-point conventions

- **Source integers.** Positions, DBH and height enter as **integer millimetres** (`DerivedManifest.cs:41-43,77-79`; `MultiAreaExperiment.cs:38,116-123`). Polygon geometry is exact `long`/`Int128` (`MultiAreaGeometry.cs:19-91`).
- **Tree state.** Tree state is **float32** (`AnnualTree`). Authoritative serialised float32 values are stored as IEEE-754 bit patterns (`F32.cs:10-28`). `areaHa` for P1 is the bit pattern `0x3db851ec` (`DerivedManifest.cs:46`).
- **Stand aggregates.** These are **binary64** (`AnnualRules.cs:105-117`). Height-envelope maths is binary64, then cast back to float32 (`SitkaGrowthModel.cs:55-60`).
- **Exposure.** No fixed-point or quantised biology is used beyond float32 storage. Float32 DBH spacing near 100 cm is about 7.6·10⁻⁶ cm, far below the annual increments.

### 2.5 Determinism and seeding

- **RNG and the roll key.** The RNG is model 1 (splitmix64) (`DerivedManifest.cs:49`; `SimulationRandom.cs:41-66`). The roll key is **(TreeId, ecological year, area seed)**. A tree therefore draws the same `u` in a given year whatever was felled around it. Paired control/treatment sessions automatically share per-tree-year random numbers (common random numbers). The death **probability** still differs because P and S differ.
- **Independence of areas.** Each area depends only on its own state, seed and hectares. Evaluation order cannot matter (asserted in `MultiAreaTests.cs:209-213`).
- **No hidden inputs.** No wall clock, no `System.Random` and no session identity enters biology. The random `sessionId` (`MultiAreaContinuation.cs:55-57`) is identity only.
- **Generator.** Integer SplitMix64 over (generator seed, area index, cell, salt) (`MultiAreaGenerator.cs:100-120`). The experiment content hash is pinned at `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` (`MultiAreaTests.cs:20`).
- **Replication consequence (important for Q2).** The canonical experiment has **exactly one seed per area**. Stochastic replication (different seeds) needs a *variant* experiment definition. That variant has a different content hash and is outside the approved canonical MA-1 design. See §6.

### 2.6 Competition and mortality contracts

- **Competition is area-local.** It uses DBH and position only, never height or crown. The read-only competition diagnostic runs the frozen single-area `PlayerCompetitionDiagnostics.Measure` on a per-area projection (`MultiAreaContinuation.cs:414-431`; `MultiAreaProjection.cs:13-32`). Its CI equals the annual pre-growth CI (`MultiAreaTests.cs:234-256`; `PlayerCompetitionTests.cs:11`). Cross-boundary exclusion is proven: the author handoff reports 13 A–B pairs within 8 m, and `MultiAreaTests.cs:239-253` asserts the CI would be larger if B trees were counted.
- **Mortality is stateless.** It is recomputed each year from current CI and RD. Natural death mechanisms are `hazard` and `boundary` only.

### 2.7 Removal (Fell) and Work Plan contract states

| State / action | Effect | Refusals (zero mutation) | Anchor |
|---|---|---|---|
| Crop mark / unmark | Plan-state only. Protects a tree from Fell. A mark on a tree that dies is cleared in the same Year+1 publish. | unknown, dead, wrong-area, already planned, already marked | `MultiAreaContinuation.cs:88-131,380-384` |
| AddPending (Fell) | Adds an order (area, tree, reason). **Fells nothing.** | blank reason, duplicate, dead, wrong-area, Crop-retained, plan already approved | `:136-158` |
| RemovePending | Removes an order | wrong area, unknown order, approved | `:161-178` |
| **Approve0** | Freezes the whole cross-area batch, mints `PlanId` `<session>:ma-plan:<n>` and key `<PlanId>:execute`. **0 Fell.** | empty plan, already approved, stale fence | `:181-197` |
| CancelApproved | Withdraws the plan. **0 Fell.** Orders are *not* returned to pending. | none approved | `:200-210` |
| **Execute1** | Revalidates the whole batch (area, Fell action, duplicate, living exact ID in its own area, Crop, reason). Refuses whole if **any populated area would be left empty**. Builds every area's roster and before/after stand, then publishes **one** event (`MultiAreaWorkEvent`) and WorkRevision+1. The ecological year does **not** change. | stale token, wrong PlanId, unminted key, `WorkWouldLeaveNoLivingStems`, etc. | `:219-330`; empty guard `:268-275` |
| Replay | Same key, PlanId and source returns the stored receipt with `Replayed=true`. **No second Fell.** | different payload → `WorkExecutionKeyConflict` | `:229-235` |
| Advance (Year+1) | Unexecuted work needs consent. Consent cancels **all** of it at 0 Fell (`CancelledUnexecutedWork=true`). A naturally empty area is carried forward, not simulated. **Refused only when every area is empty.** | `WorkPendingConsentRequired`, `ContinuationNoLivingStems` | `:340-405`; `:346-351,372-379` |
| Fell record | `fell` / `manual-work-plan`, reason, `EcologicalYear = y` (the year *before* the next advance), work revision, key, PlanId | — | `:291-296`; `MultiAreaContracts.cs:37-38` |

### 2.8 Year+1 chronology (as coded)

1. In year **y**: mark, then plan, then Approve (0 Fell), then Execute. Felled trees get `Death.EcologicalYear = y`; the year and annual revision are unchanged (`MultiAreaContinuation.cs:278-279,293`; asserted `MultiAreaTests.cs:385-386`).
2. Advance: each living area runs E1–E16 on its **post-Fell** roster, giving year **y+1**. Natural deaths carry year y+1 (`AnnualRules.cs:15,49,138`). The Year+1 report equals `Calculate` on the post-Fell roster (asserted `MultiAreaTests.cs:407-427`).
3. Within one annual step, CI is computed **before** growth on the pre-growth DBH. Density pressure uses the **post-growth** stand. Hazard and boundary use pre-growth suppression (`AnnualRules.cs:17-33,132`).

### 2.9 Maximum lawful horizon (from code)

- **No year cap in MA-1.** `MultiAreaContinuation.Advance` has no year cap (`:340-405`). It stops only when every area is empty (`:350-351`) or on `checked` int32 overflow of year, revisions or age (`:354`, `:395`, `AnnualRules.cs:15,99`). That overflow is about 2.1·10⁹ years.
- **50 years is permitted by the rules.** Neither the code nor any MA-1 refusal stops at a year count. A single-area test explicitly asserts "no ten-year cap" at 12 years (`ContinuationTests.cs:44-53`).
- **Legacy one-year paths do not apply.** The legacy `AnnualEngine.Advance` (CLI `annual`) is a one-year experiment (`AnnualEngine.cs:84-85`; `LAB/src/Cli/Program.cs:56-81`). It is not the MA-1 path.
- **Exercised horizons so far.** Author tests exercise at most **10** multi-area years (`MultiAreaTests.cs:216-232`) and 12 single-area synthetic years. **No 20- or 50-year MA-1 run exists in evidence.**
- **How 50 years would be verified in Q1 (not done).** Run two fresh sessions to year 50 and assert:
  - `EcologicalYear == AnnualRevision == AnnualHistory.Count == 50` with no refusal;
  - byte-identical views and annual results across the two sessions at every year;
  - every area either living, or carried forward as `NoLivingStems`.

  If both areas empty first, the lawful horizon for that scenario is the last committed year. Report that as an outcome; never alter rules to continue.
- **Execution route.** **No CLI route exists for MA-1.** It is reachable only through:
  - the UI process routes (`LAB/src/Ui/PlayerMultiApi.cs:61-79`, `api/multi/year/advance` one year per POST), or
  - internal types visible to the test assembly (`AnnualEngine.cs:3`, `InternalsVisibleTo("SitkaP1.Core.Tests")`).

  Either route is a Q1 decision under a lease. Per packet 01 §"Ownership", any harness added to the repository needs a new bounded source packet.
- **Science horizon is separate from the code horizon.** GROWFOR's illustrated Sitka runtime window is top height < 49.3 m and "first thinning to clearfelling" (`SGM/GROWFOR_Technical_Evidence_Extraction_v1.md` §2 item 5, §11.1). The Class III anchor-matched envelope beyond age 30 is modelled, not measured (`SGM/SitkaGrowthModel1.md` line 32). MA-B's 47-year cohort would reach age 97 at year 50.

---

## 3. State and history model

**Kept in the single commit root.** `MultiAreaRoot` (`MultiAreaContinuation.cs:16-26`) holds:
- the shared year, annual/work/plan-state revisions;
- per area: the full current roster (living and dead, frozen at death) and Crop marks;
- pending orders and the approved plan;
- the work-event ledger (per-area before/after `AnnualStand` and Fell records with reasons), and the executed-key map;
- `AnnualHistory`: per year, per area, Before/After `AnnualStand`, natural-death list, `NoLivingStems`, work revision and `CancelledUnexecutedWork` (`MultiAreaContracts.cs:56-61`).

**Returned once, NOT persisted in history.** Each Advance returns the full per-area `AnnualReport` (`MultiAreaContracts.cs:64-67`):
- per-tree pre-growth CI and neighbour count, suppression, death probability and roll;
- post-growth stand, density pressure and mean vulnerability.

The root keeps only the current roster plus stand-level history. **Per-tree yearly trajectories (DBH, height, crown, CI, p, u) are therefore lost unless the caller captures every returned report.** A Q1 harness must log every report.

**Also lost or never recorded.**
- Removed pending orders.
- Content of cancelled plans: only the flag on consented-advance cancellation is kept.
- The approval/cancel sequence: only `PlanStateRevision` counts it.
- Any per-tree history beyond the frozen death state.

**Durability.** None. The session is in memory only: "Session only — nothing is saved" (`MultiAreaContracts.cs:120`). `AnnualContinuation` (single-area, `LAB/src/Core/AnnualContinuation.cs:16-27,279-314`) has the same pattern with one undivided scope.

---

## 4. Absent subsystems: NOT MODELLED vs stated simplification

| Subsystem | Status in MA-1 at `4cecf873` | Evidence |
|---|---|---|
| Natural regeneration / recruitment / ingrowth / juveniles | **NOT MODELLED.** Stems are only removed, never added. N/ha cannot rise. | `DerivedManifest.cs:58-59`. No recruitment code exists in `LAB/src/Core`. |
| Windthrow / storm | **NOT MODELLED.** No wind, no slenderness risk, no post-thinning exposure. | `DerivedManifest.cs:59` (`storm`). Unity WindthrowV1 is a separate Unity system that is "off for new games" (`RES/WindthrowV1/README.md` lines 1-5) and not linked into the lab. |
| Light interception / canopy light | **NOT MODELLED.** Crown radius is computed but feeds nothing. | `DerivedManifest.cs:59` (`canopyLight`); `AnnualRules.cs:94-98` vs `:67-82` |
| Browsing | **NOT MODELLED** | `DerivedManifest.cs:59` |
| Timber volume, grades, sawlog / assortments | **NOT MODELLED.** No volume, form height or SED is computed. | grep of `LAB/src/Core` finds no volume or assortment code |
| Economics (costs, revenue, prices) | **NOT MODELLED.** Shown as "UNAVAILABLE (not €0)". | `LAB/src/Core/PlayerSessionView.cs:67` |
| Cross-area ecology (shared edge competition, seed, wind, light) | **NOT MODELLED.** Areas are isolated by design. | `MultiAreaExperiment.cs:25-27` |
| Pruning, mixed species, deadwood/snags, harvest/extraction damage, soil/site axes | **NOT MODELLED** | `DerivedManifest.cs:59`. No deadwood or damage code exists in `LAB/src/Core`. |
| Site productivity | **Modelled at a stated simplification.** The constant `site = 1.0f` scales DBH only. Height follows the hard-coded Class III envelope regardless of the "site UNKNOWN" label. | `AnnualRules.cs:89-92`; `SitkaGrowthModel.cs:37-60` |
| Competition | **Modelled at a stated simplification.** Distance-dependent Hegyi, DBH only, 8 m cutoff, area-local, no edge correction. | E3 |
| Height growth | **Modelled at a stated simplification.** Each tree keeps its ratio to the Class III envelope; no competition or thinning effect. | E6 |
| Crown | **Modelled at a stated simplification.** Potential × competition response with relaxation. Output only. | E7 |
| Self-thinning mortality | **Modelled at a stated simplification.** British max-line RD, calibrated hazard [C], boundary at RD 1. | E10–E15 |
| Thinning | **Modelled at a stated simplification.** Only player-named single-tree Fell. No prescription, no BA target, no thinning-shock state. | §2.7 |

---

## 5. Crosswalk: engine quantity → research

Columns:
- **Composite** = `RES/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`;
- **GROWFOR** = `SGM/GROWFOR_Technical_Evidence_Extraction_v1.md`;
- **EM** = `SGM/EvidenceMatrix.csv`, where the row number is the file line.

Verdicts:
- **AGREE** = the engine form matches the cited direction or form;
- **SIMPLIFICATION** = modelled more simply than the evidence form;
- **ABSENT** = not modelled.

"Calibrated" is never claimed.

| # | Engine quantity (anchor) | Composite | GROWFOR | EM | Verdict and note |
|---|---|---|---|---|---|
| X1 | Height envelope Class III, `b2 0.042`, `b3 1.563`, H30 20.4 m (`SitkaGrowthModel.cs:37-49`) | §4 (Lekwadi 2012; anchors I–V; QA warning on rounded b0) | §5 (GROWFOR's own height model is a different Bertalanffy–Richards fit, a 47.0216, c 0.4571) | 2, 3 | **AGREE** with the Irish Class III anchor at age 30. Values beyond age 30 are modelled (`SGM/IrishHeightValidation.md` table; `SitkaGrowthModel1.md` l.32). The engine does **not** use the GROWFOR height equation. |
| X2 | Height has no competition or thinning term (`SitkaGrowthModel.cs:51-60`) | §4 bullet 2 (top height largely independent of stocking and normal thinning); §5 (`ΔH = G_H(...)`, weak or no competition) | §4.5 (site by top height) | 4 | **AGREE** in direction. **SIMPLIFICATION**: individual height is fully determined by initial height and age, so the relative heights of suppressed trees never change. |
| X3 | DBH increment (E5), DBH the only competition-responsive variable | §5 (`ΔDBH = G_D(age, DBH, site) × competitionMultiplier × vitality`; DBH principal) | §14.2 (individual-tree increments under release NOT validated by GROWFOR) | 10, 12 | **SIMPLIFICATION**: no age term, no vitality term, site fixed at 1.0. Potential 1.2 cm/yr is [C]. Unthinned absolute growth after about 16 cm is "later unsupported (no density feedback)" (EM 12). |
| X4 | Hegyi CI with `r = 1/(1+CI/5)` (E3–E4) | §5 (Hegyi mechanism evidence-based; response and CI50 are abstraction) | §2A.4 / §14.2 (local Hegyi release not validated by GROWFOR) | 16 | **AGREE** (form). Known limitation: **scale-invariant**. Uniform growth leaves CI unchanged, so DBH growth does not slow as the stand fills; only mortality feels density (`SGM/CurrentModelAudit.md` structural finding 2, l.54). |
| X5 | Thinning response of retained-tree DBH (DBH via CI drop after Fell) | §5 Frenchpark table: after 29 growing seasons, mean DBH 23.7 / 29.8 / 31.1 / 32.4 cm (unthinned / light / medium / heavy) | §7 (stand-level thinning-shock state not retained; "not evidence that individual retained trees do not respond") | 11 | **AGREE in direction only, as a prior Unity calibration.** EM 11 records ratios 1.28/1.38 against observed 1.26/1.37 under the **Unity C8 calibration** on the Unity stand. **UNKNOWN for MA-1's synthetic areas.** The equation lineage is the same (`AnnualRules.cs:87`), but it was not re-checked here. Frenchpark is stand **mean** DBH, which includes the selection effect of removing small trees. The engine reports Dq, not mean DBH (E9). Only the *sign* of the retained-tree increment response is a valid MA-1 check. |
| X6 | Crown potential `0.9415 + 0.07635·D`, relaxation 0.15 (E7) | §5, §8 (Tabbush & White; k about 0.1–0.2/yr) | — | 13, 14, 15 | **AGREE** (envelope and k range). **SIMPLIFICATION**: the crown is output only (no light). Davies & Pommerening's pooled positive Hegyi coefficient is correctly not used (EM 14). The crown-length model is not implemented (EM 15). |
| X7 | RD on the British max size–density line (E10) | §2 (no Irish N–Dq/SDI line published); §3 (DEC/DMC not for pure Sitka) | §2A.3 (GROWFOR N embeds stand mortality, no line given) | 17, 18, 23 | **SIMPLIFICATION / transfer**: British line [B], Irish transfer unverified. |
| X8 | Self-thinning hazard and boundary (E11–E15) | — (composite has no mortality hazard) | §2A.3: GROWFOR provides **no** individual-tree or cause-specific hazard; it is "not evidence for CCF's suppression-biased individual-tree mortality allocation" | 19, 24, 25, 26, 27 | **SIMPLIFICATION, calibration [C]**: onset 0.6, strength 0.08, S⁴, all tuned on the **Unity 40 m Scenario One stand** (`SGM/AdultMortalityCalibration.md` l.51-60). Forest Yield trajectories were not obtained (EM 27). There is **no evidence** that these constants transfer to 0.09–0.15 ha synthetic areas. |
| X9 | Stand BA/ha, N/ha, Dq (E9) | §2 (Sitka untreated BA about 55 m²/ha in one long-term trial, Jones et al. 2026) | §4.1 (B, N, H state; aggregate comparison is GROWFOR's valid role, §14.3) | 21 | **AGREE** as definitions. Note: author-reported MA-A genesis BA is 74.02 m²/ha at age 24. That is above the single cited untreated trial observation of about 55. Synthetic authored, site unknown: **flag, not a verdict**. EM 21 (an 80–90 m²/ha ceiling) is ungraded [I] and was dropped (`AdultMortalityCalibration.md` l.3). |
| X10 | Top height / site index framing | §4; §9 (site index = top height at 30) | §3.2, §4.5, §11.1 (top height < 49.3 m runtime window) | 2, 9 | **SIMPLIFICATION**: the engine has no top-height or site-index variable. A harness can derive top height (EM 9's convention needs checking against S4). |
| X11 | Volume / assortments / sawlog | §5 (stem volume = BA × form height; Teagasc form) | §8, §10 | — | **ABSENT**: NOT MODELLED |
| X12 | Regeneration / recruitment / light thresholds | §5 (about 20 % RLI threshold; seed onset; dispersal; establishment) | §14.2 (not validated) | — | **ABSENT**: NOT MODELLED |
| X13 | Windthrow / stability | §6 (Irish stand-level >3 % model; "must not be pasted into windthrowChance") | — | 22 | **ABSENT**: NOT MODELLED |
| X14 | Thinning-transition / prescription by BA or stocking | — | §3.3, §9 (thinning by material removed or retained state) | — | **ABSENT** as a prescription. Only manual per-tree Fell exists. |
| X15 | Page et al. 2001; Stokes 2009; CONTINUCOVER | **not in any repo composite** | — | — | **UNKNOWN**: sources unavailable to this worker. Per packet 02 they concern British regeneration, extraction damage and an Irish transformation trial. MA-1 models none of those processes, so no valid MA-1 crosswalk row exists either way. |

### 5.1 Named claims that are NOT checks for this engine

These are rejected as checks. MA-1 does not model the outcome, and the cited repository evidence does not establish them as universal laws (packet 01 Q3; packet 02 R2; composite §5–§6):
1. **"BA > 30 ⇒ seedling growth zero"**: there are no seedlings or light (NOT MODELLED). The composite (§5 Light) says about 30 m²/ha "can sometimes provide" about 20 % RLI and that "stocking and arrangement matter". That is not a threshold law.
2. **"> 50 % removal ⇒ windthrow"**: there is no wind (NOT MODELLED). The composite §6 Irish model is stand-level and site-dependent and must not become a tree rule.
3. **"Heavy thinning ⇒ 2× per-tree volume"**: there is no volume (NOT MODELLED). Frenchpark heavy/unthinned mean tree volume 0.85/0.44 m³ (§5) is one trial's 29-season stand-mean outcome, with a selection effect. It is not a universal multiplier.

---

## 6. Proposed experiment protocol for Q1/Q2: NOT RUN

**Preconditions (all currently unmet).**
- A D-091 custodian receipt and a non-overlapping qualification lease.
- An exact integrated/verified SHA. Without one, record the candidate SHA as `4cecf873` and say "candidate, not integrated".
- A run route chosen under the lease (§2.9).
- Publication of the harness bytes and SHA-256 with the results.

**Design.**
- **Sessions, all from the canonical experiment (content hash `c66d566c…`).** Use the same area seeds in every session, so tree-year random numbers are shared (§2.5).
  - **C**: no-work control.
  - **L**: a lighter manual thinning.
  - **M**: a genuinely heavier manual thinning.
  - **D**: delayed. The same rule as L, applied first at year 5.
  - **S**: staged. L's rule applied at year 0 and again at year 10, against one equivalent single intervention.
  - **I**: isolation. Thin MA-A only; leave MA-B untouched.
  - L and M use **one shared two-area Work Plan** per intervention, which is the required two-area shared-plan case.
- **Intensity rule.** Decide the qualitative selection rule before running. Two examples: (a) Crop-tree release, felling the listed Hegyi competitors of chosen Crop trees as returned by the read-only competition diagnostic; (b) removal from below by DBH rank among allowable trees. The **numeric size of each intervention is chosen at run time from the actual allowable set and disclosed exactly**: number of Crop trees, competitors per Crop, and the IDs felled. The allowable set is living, in-area, not Crop, not duplicate, and not emptying an area. Also record the realised removed stems, removed BA and removed fraction of both, per area, from the engine's own `MultiAreaWorkEvent` before/after.
  - **No percentage is fixed in this protocol.**
  - "Genuinely different" means M's removed BA exceeds L's in **both** areas. State the difference; do not assert it.
- **Horizons.** Record years **1, 5, 10, 20** and **50**. 50 is lawful by the code (§2.9) but has never been exercised. If both areas empty before 50, report the last committed year and stop.
- **Repeats.**
  - (i) Determinism: two fresh sessions per scenario must be byte-identical at every year.
  - (ii) Order: compare against `MultiAreaAnnual.Step` per area in reverse order.
  - (iii) Stochastic replication: optional. It needs **variant experiments** that differ only in area seeds, which changes the content hash. Run these only if the Manager authorises a non-canonical variant, label them "variant, not MA-1 canonical", and keep them separate from canonical results.
- **Chronology checks, per intervention.**
  - Approve leaves the living count and the year unchanged.
  - Execute keeps `EcologicalYear` and `AnnualRevision`, and moves `WorkRevision` +1.
  - Felled trees carry `Death.EcologicalYear == y`, cause `fell`.
  - The Year+1 report `Before` equals the event's per-area `After`.
  - Felled IDs are absent from every neighbour list in year y→y+1.
  - Natural deaths carry year y+1.
- **Valid-case boundaries, each attempted and recorded.**
  - Fell-to-empty: a plan containing every living tree of one area is refused as `WorkWouldLeaveNoLivingStems`, and the approval stays (`:268-275`).
  - Cancelled plan: Approve then Cancel gives 0 Fell, PlanState +1, and orders not returned.
  - Consented advance with pending work: all orders cancelled, 0 Fell, `CancelledUnexecutedWork=true`.
  - Replay of an executed key: `Replayed=true` and no change.
  - All-empty Year+1 (`ContinuationNoLivingStems`): **not reachable** on the canonical experiment by player action, because Fell cannot empty an area. It is covered by author test MA-E05 only (`MultiAreaTests.cs:258-286`, test-only experiment). Report it as "not exercised on canonical data" unless a disclosed variant is authorised.
- **Measurements, per area per recorded year, from every returned report.**
  - Per tree: ID, age, DBH, height, crown radius, pre-growth CI, neighbour count, S, p, u, death year/cause/mechanism.
  - Counts: living, hazard deaths, boundary deaths, cumulative natural deaths, cumulative Fell.
  - Stand: N/ha, **areaHa-normalised BA**, Dq, arithmetic mean DBH (computed by the harness, for Frenchpark-style direction), RD, density pressure, mean vulnerability.
  - Plan and work ledger: work ledger, plan IDs and keys.
  - **No-double-application check**: every Fell ID appears once in the ledger and dies once. The sum of Fell plus natural deaths equals genesis minus living.
  - **Isolation check**: in session I, MA-B's roster and reports are byte-identical to session C's MA-B at every year.
- **Valid comparisons.** Compare only the *direction* of retained-tree DBH increment and stand mean DBH between C, L and M, against Frenchpark (composite §5) and EM 11. State explicitly that these are synthetic stands, not yield plots, and not comparable to Frenchpark's site, age or duration. No BA-threshold, windthrow, recruitment or volume check (§5.1).

---

## 7. Weakness list (time-boxed, falsifiable, ranked by consequence)

Each item is a hypothesis to test in Q1/Q2. None is a measured result.

| Rank | Weakness | Anchor | Falsifying test (time box) |
|---|---|---|---|
| W1 | No recruitment: N/ha is non-increasing, so CCF transformation and regeneration outcomes cannot appear at any horizon | `DerivedManifest.cs:58-59` | Assert living count is non-increasing in every scenario to year 50. It is refuted only if any year's living count exceeds the previous year's. (Analysis of Q2 logs, 0.5 h) |
| W2 | Hegyi scale invariance: DBH growth does not slow as the stand fills; density acts only through mortality | `AnnualRules.cs:67-91`; `SGM/CurrentModelAudit.md` l.54 | In session C, plot mean CI against RD over 20 years. The weakness is confirmed if mean CI stays roughly flat while RD rises toward 1 before deaths begin. (1 h) |
| W3 | Mortality constants [C] were calibrated on the Unity 40 m stand and transferred to 0.09–0.15 ha synthetic areas with no evidence | `SitkaGrowthModel.cs:89-96`; `SGM/AdultMortalityCalibration.md` l.51-60 | Compare year-by-year hazard versus boundary death shares in MA-A against the "hazard does most of the thinning" claim. A boundary-dominated MA-A shows the calibration does not carry over. (1 h) |
| W4 | Edge exclusion with no buffer or correction: boundary trees have systematically lower CI and faster growth in small polygons | `MultiAreaExperiment.cs:25-27`; `AnnualRules.cs:71-80` | Bin session-C trees by distance to their polygon edge (< 8 m vs ≥ 8 m) and compare mean CI and 10-year DBH increment. A significant edge excess confirms bias. (1.5 h) |
| W5 | "Site UNKNOWN" label versus a hard-coded Irish Class III height trajectory and `site = 1.0` DBH | `MultiAreaExperiment.cs:20,28-29`; `SitkaGrowthModel.cs:37-60`; `AnnualRules.cs:89` | Check `H_i(t)/E(age_i(t))` is constant per tree for every year in C, L and M. Constant ratios show height is site-fixed, not unknown, and the disclosure needs wording. (0.5 h) |
| W6 | Single seed per area: canonical runs give one stochastic realisation, so C-versus-L differences can be seed artefacts | `MultiAreaGenerator.cs:41,46` | Under an authorised variant, rerun C and L with k alternative area seeds. If the sign of the C–L difference in deaths or Dq flips across seeds, single-seed conclusions are invalid. (2 h, variant authorisation required) |
| W7 | No age term in DBH growth and the 120 cm DBH scale: old and young trees of equal DBH and CI grow identically | `AnnualRules.cs:89-91`; composite §5 `G_D(age,…)` | Find MA-B pairs of a 47-year and a 29-year tree with similar DBH and CI. Equal increments confirm. (0.5 h) |
| W8 | Crown is output only: no light or crown feedback, so release changes crowns visually but crowns never feed back | `AnnualRules.cs:94-98` vs `:67-82` | Code-read confirmation exists already. Runtime check: the per-tree DBH and death sequences of two sessions are identical whatever their crown values. Crown-only perturbation needs a variant, so read-only evidence is sufficient. (0.25 h) |
| W9 | Mixed chronology: hazard and boundary use pre-growth suppression with post-growth RD | `AnnualRules.cs:17-33,132` | Document only. Recompute S from post-growth CI offline on logged rows and count how many boundary-order choices would change. (1 h) |
| W10 | Small-area granularity: one stem is about 10.9 /ha in MA-A and about 6.7 /ha in MA-B, so stand metrics move in coarse steps | `MultiAreaGeometry.cs:130`; E9 | Report N/ha step size alongside every intensity, so readers do not over-read small differences. (0.25 h) |
| W11 | History loss: per-tree yearly audit rows are not stored, so the experiment is irreproducible unless the harness logs each report | `MultiAreaContracts.cs:56-67` | Check that the harness log has `years × trees` rows and that its replayed digest matches. (0.25 h) |
| W12 | Documentation drift: `LAB/README.md:153` says CLI `annual` is "always REFUSED in F0", but `AnnualEngine.RequireAnnualDomain` lets an accepted fixture through (`AnnualEngine.cs:57-63`) | as cited | Q1 runs `annual` once against the accepted fixture and records refused or not. Outcome UNKNOWN until run. (0.1 h) |

---

## 8. Limitations and uncertainty

**Verified by reading code at `4cecf873`.** Every equation, constant, refusal, state field and chronology statement in §2–§4 and §6–§7 with a file:line anchor. Also the blob equality of the four linked or cited CCF files with their pins.

**Read from tests, not executed.** Genesis tree counts, exact areas, the content hash and the behaviours asserted by the named tests. I did **not** run the tests. The author's pass claims are in the MA-1 handoff, not verified here.

**Author-reported, not verified.** MA-A and MA-B genesis N/ha, BA, Dq and RD (§2.2), and the 13 cross-boundary pairs.

**Inferred.**
- The common-random-number property (from the roll key).
- That no year cap exists (from the absence of one in the read code paths).
- That the all-empty refusal is unreachable by player action on canonical data.
- The W1–W12 hypotheses.

**Needs a run (Q1/Q2, blocked).** Every trajectory, horizon outcome, death count, edge effect, intensity value and the behaviour of the README `annual` command.

**UNKNOWN.**
- G1 ecological validation.
- Content of the three uploaded research returns.
- Page et al. 2001, Stokes 2009 and CONTINUCOVER evidence.
- Whether the Unity C8 DBH thinning-response calibration (EM 11) holds on MA-1's synthetic areas.
- Whether the mortality constants transfer to small areas.
- Whether 50 years completes without both areas emptying.
- Whether any MA-1 trajectory resembles an Irish stand.

**No empirical comparability claim.** MA-A and MA-B are synthetic authored stands. They are not yield plots, not surveyed Irish forest, and not assessed comparable to each other or to Frenchpark.

## 9. Files read

**At `4cecf873`, under `LAB/`:**
- `src/Core/AnnualRules.cs`, `AnnualEngine.cs`, `Domain.cs`, `F32.cs`, `WorkContract.cs`, `AnnualContinuation.cs`, `CcfSourcePins.cs`, `DerivedManifest.cs` (l.1-120 plus grep), `SitkaP1.Core.csproj`;
- `src/Core/MultiArea/MultiAreaContinuation.cs`, `MultiAreaContracts.cs`, `MultiAreaExperiment.cs`, `MultiAreaGenerator.cs`, `MultiAreaGeometry.cs`, `MultiAreaProjection.cs`, `MultiAreaWorkPlanAdapter.cs`;
- `src/Cli/Program.cs`;
- `src/Ui/PlayerMultiApi.cs` (routes and limits, by grep);
- `README.md` (grep);
- `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` (header);
- `tests/Core.Tests/MultiAreaTests.cs` (l.1-300, 377-430, 680-830 plus test index), `ContinuationTests.cs` (l.1-60), and the test-name index of every test file.

**At `4cecf873`, under `CCF/`:** `SitkaGrowthModel.cs`, `SimulationRandom.cs`.

**Research at `4cecf873`:**
- `RES/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md` (whole);
- `SGM/GROWFOR_Technical_Evidence_Extraction_v1.md` (§2, §2A.3–2A.4, §3–4.1, §7, §11, §14, plus headings);
- `SGM/EvidenceMatrix.csv` (whole);
- `SGM/CurrentModelAudit.md` (whole), `SitkaGrowthModel1.md` (l.1-80), `AdultMortalityCalibration.md` (whole), `Scenario1SiteProductivityDecision.md` (whole), `IrishHeightValidation.md` (l.1-30), `StandDevelopmentBeforeAfter.md` (l.1-30);
- `RES/WindthrowV1/README.md` (l.1-40), `RecentThinningAssessment.md` (l.1-25).

**Handoffs:**
- `Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-MA1-TWO-AREA-SIMULATOR-SOURCE-IMPLEMENTATION-HANDOFF-20261121.md` (l.1-60 plus grep). It is the only file in that folder at `4cecf873`; I found no separate "approved spec" file there.
- Packets 01, 02 and 03 at `docs/multi-885-next-action-orchestration-research-packets-20261121-v1@7a2a13fa`.

**Not read.** The `AdultMortalityDesign`, `HeightCandidateComparison`, `EconomyTutorialImpact`, `SaveVersioningAssessment` and `CanonicalDecisionProposal` files (other than grep hits), the remaining `WindthrowV1/*.md`, and all `Evidence/*.csv`. None was needed for an anchored claim above.
