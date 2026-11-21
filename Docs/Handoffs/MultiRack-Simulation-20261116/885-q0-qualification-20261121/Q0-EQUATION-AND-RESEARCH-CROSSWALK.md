# Multi #885 MA-1 — Q0 equation, state and research crosswalk (candidate `4cecf873`)

**Stage Q0 of the MA-1 multi-year scientific qualification. GitHub/Git read-only inventory. This file is not a code change, not an engine run, not a review and not an acceptance.**

## 0. Stage labels, scope and custody

| Label | Status |
|---|---|
| **Q0 COMPLETE** | **YES**: read-only equation/parameter/state inventory and research crosswalk, delivered in this file |
| **Q1+Q2 EXECUTED** | **NO**. Q1 is BLOCKED: there is no D-091 custodian receipt and no qualification lease. No engine experiment was run. This file contains no engine output. |
| **Q4 REVIEWED** | **PENDING**. One independent nonauthor review happened: OpenAI GPT-6.1 Sol, `Q0-INDEPENDENT-REVIEW.md` on branch `docs/multi-885-q0-crosswalk-review-20261121-v1` (blob `cb78f649…`, SHA-256 `1a3f7ed8…`). It reviewed revision 1 (blob `bd125f54…`) and returned **NEEDS CHANGES** (0 BLOCKING, 5 MAJOR, 3 MINOR). Revision 2 (blob `98039297…`) applied F1–F8. The changed-lines recheck, `Q0-INDEPENDENT-RECHECK.md` on the same review branch (blob `0dd5bfe8…`, SHA-256 `4dc91fb3…`), returned **NEEDS CHANGES** on revision 2: F1–F8 6 PASS / 2 PARTIAL (F4, F5), plus new findings R1, R2 (MAJOR) and R3 (NOTE). **This is revision 3**, which applies R1–R3 (§0.1). The **final** recheck has **not** returned. **No approval is claimed.** |
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
| Revision | **3**. Revision 2 applied the Q4 review corrections F1–F8; revision 3 applies the recheck corrections R1–R3 (§0.1). It is the same author: Claude Opus 5.5, Claude Code as a T3 child thread, single writer, no subagents. Revision 3 was written in Claude Code session `fd2fd02b-9a3a-4caf-9b54-b1fcfb69ab37`; revisions 1–2 in the session named above. Revision 1 was at branch head `4be3b2f3cddcbf9c10a60e17769d3d6e1be1846d`, blob `bd125f54fdcd0d8c0ee2f3a481b59728e0570455`. Revision 2 was at branch head `fc1c00f4dc2d69722d41d0cecd1146c04698f3d0`, blob `980392974029cecd75e1640a664ed6cfcc704e16`. All code anchors are still at candidate `4cecf873`. |
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

### 0.1 Corrections applied in revisions 2 and 3

The F1–F8 corrections (revision 2) follow the Q4 independent review (GPT-6.1 Sol). The R1–R3 corrections (revision 3, subsection below) follow its changed-lines recheck. Finding IDs are kept unchanged. No correction changes the engine, adds a coefficient or reports a measurement. The final recheck should verify the revision-3 sections named below.

**Revision 2 (F1–F8).**

| Finding | Sections changed | What changed |
|---|---|---|
| F1 (MAJOR) repeat equality | §2.5 "No hidden inputs" and new "Cross-session comparison projection"; §2.9 Q1 verification; §6 Repeats and Isolation check; §7 W13 | Byte-identical full views are no longer required, because the random `sessionId` appears in views and is embedded in `PlanId`, execution keys and Fell records. Repeats now use a deterministic comparison projection: annual reports, area views and history are compared directly, session-derived IDs are checked within their own session and then normalised for the ledger comparison, and serialisation, ordering and float representation are fixed. Random session generation is kept. |
| F2 (MAJOR) units | §2.3 E3, E4, E12, E13 (units column and notes); §2.4 Units; §6 Measurements | CI is now stated as m⁻¹ (a DBH ratio divided by distance in metres), with CI50 = 5 m⁻¹ in the same convention. `p` is now a dimensionless Bernoulli probability for one annual step, not a per-year instantaneous hazard. No unit conversion is made or implied. |
| F3 (MAJOR) growth overstatement | §5 X3, X4; §7 W2 (now an H-W2 hypothesis), W7 | The "DBH growth does not slow" and "DBH the only competition-responsive variable" claims are narrowed to the defensible one: growth has no direct occupancy/RD term. The clamp(1 − D/120) size taper is kept. DBH is the principal competition-responsive growth variable, and the crown target also responds to CI. The assumed pre-mortality window is removed, because MA-A starts above the RD onset. |
| F4 (MAJOR) weakness list | §7, restructured into 7.1 tiers and ranking rationale, 7.2 established limits, 7.3 falsifiable hypotheses | Established limits are now separate from falsifiable hypotheses. Each hypothesis has a rejection criterion, a tolerance and named confounders. W3 is labelled a model diagnostic that cannot prove or disprove empirical transfer. W4 has a decision rule with age/DBH/spacing/dependence handling. W5 has a float32-aware tolerance. W7 drops the matched-pair test. W11 is a retention/replay statement with empty-area/null handling. The ranking rationale is explicit, and the F1/F5 defects (W13, W14) rank above documentation drift. |
| F5 (MAJOR) protocol | §6 Design (S, intensity rule), Logging (new), Measurements, Valid comparisons | "Equivalent single intervention" is defined (removed stems as the primary match; BA, IDs and timing reported as mismatches). There is a pre-registered, outcome-independent selection and stopping rule over the allowable set, and chosen IDs are disclosed before each Execute. Retained-tree growth is now compared on matched trees, separately from selection-only mean shifts. Null means unevaluated, never zero. Every year is captured, not only checkpoints, and every command and result is logged for replay. |
| F6 (MINOR) refusal language | §2.7 Advance row; §2.9 "No year cap" | "Refused only" and "stops only" are qualified: fence/session mismatch and missing work consent refuse before the all-empty check. Biological stopping is separated from a checked int32 overflow, which is an exception, not a refusal code. |
| F7 (MINOR) citation | §2.2 author-reported genesis row; §8 | The citation is corrected to handoff §1 "IMPLEMENTED" item 1 (§1.1 is Deviations). The author-reported / not re-verified label is kept. |
| F8 (MINOR) precision | §2.3 (new precision column notes on E3–E7, E9, E12–E14); §2.4 | Precision is now stated per step: float32 CI accumulation, float32 DBH/crown/area, double stand sums, and a double height envelope cast back to float32 per step. E6 is labelled algebra, not bit-exact evaluation. The S/p null and zero guards are stated. The float32-spacing claim is corrected: ΔD is 0 at D ≥ 120 cm, and rounding can stop small increments. |
| Missing consequential weaknesses | §2.3 E15; §6 boundaries; §7 W13–W15 | The F1/F5 consequences are added as W13/W14. The boundary's last-survivor guard is no longer generalised: hazard draws alone can kill every living tree of an area (W15). |

**Revision 3 (R1–R3, from the changed-lines recheck).** The recheck found F4 and F5 PARTIAL because of R1 and R2. R1 and R2 are the remaining parts of those findings, so F4 and F5 have no separate revision-3 rows.

| Finding | Sections changed | What changed |
|---|---|---|
| R1 (MAJOR) H-W4 not executable | §7.3 H-W4 (split into H-W4-CI and H-W4-G); §8 Protocol choices | The CI and growth parts are now two hypotheses with separate decision rules and separate conclusions. H-W4-CI keeps the negative-coefficient rule with the 20 % interior-mean-CI tolerance. H-W4-G has its own **positive** criterion: the edge coefficient on ΔD must be > 0 and exceed 20 % of the interior mean ΔD, over the explicit growth window year 0 → year 10. Each stratum-year (CI) or stratum (growth) returns **NOT ESTIMABLE**, with its group counts and the reason, when either group has fewer than 2 trees, the design is rank-deficient, or the denominator is ≤ 0. Any NOT ESTIMABLE stratum makes the overall result **INCOMPLETE**; it is never counted as a confirmation or a rejection. The `ln(D)` term is labelled a chosen adjustment: it does not show that DBH confounding or edge causality is removed, and the inference is descriptive and specific to the model. |
| R2 (MAJOR) shared-competitor traversal | §6 Selection rule (renumbered steps 1–10), S1 definition, I | There is now one algorithm. Every Crop's competitor list is frozen from the same intervention-time state before any selection. Crops are traversed in ascending ordinal `TreeId` within each area, and areas in ordinal `AreaId` order. The removal sequence is built rank layer by rank layer (all Crops' rank-1, then all rank-2, …). It is deduplicated at first occurrence **without replacement**: a later Crop never moves to its next competitor. L, M, D, S and S1 all use this same sequence. The engine command order is fixed: Crop marks first, then orders, then Approve. An empty order set is recorded as a **no-intervention** result, and Approve is not called. No ecological coefficient or protected rule is changed. |
| R3 (NOTE) W15 transition year | §7.2 W15; §2.3 E15; §2.9 50-year assertions; §6 Null and empty semantics, Measurements | Extinction is now identified from the report's `After.LivingCount == 0`. The first carry-forward year (`Report = null`, `NoLivingStems = true`) is logged separately, and so is the case where no carry-forward year commits because every area is empty. "Independent draws" is replaced by "separately keyed draws". No claim is made that the deterministic keyed rolls have been shown to be statistically independent. Simultaneous extinction of both areas is an unverified possibility, not a canonical result. |

**Precision note (not a disagreement).** One shorthand of F8 reads "CI … float32 with double accumulation". At `4cecf873` the CI sum itself is accumulated in float32 (`AnnualRules.cs:69-81`). Double accumulation applies to the stand ΣD² and the vulnerability mean (`:30-32,105-110,125-128`). §2.3 states it that way.

**Disagreement (one sentence, correction applied anyway).** For W4, MA-A and MA-B are complete censuses of synthetic stands, not samples, so a sampling significance test has no defined population. I therefore give a pre-registered magnitude decision rule with explicit dependence handling in place of a p-value threshold, and I name the confounders as F4 asks.

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
| Author-reported genesis stand values (author console print, **not re-verified by me**) | MA-A: 1,868.9 stems/ha, BA 74.02 m²/ha, Dq 22.46 cm, RD 0.802. MA-B: 486.7 stems/ha, BA 39.50 m²/ha, Dq 32.15 cm, RD 0.438. | MA-1 author handoff `Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-MA1-TWO-AREA-SIMULATOR-SOURCE-IMPLEMENTATION-HANDOFF-20261121.md` §1 "IMPLEMENTED", item 1 (handoff l.38 for MA-A, l.39 for MA-B). Revision 1 wrongly cited §1.1, which is "Deviations". These values remain **author-reported and not re-verified**. The tests assert only the inequalities "A RD > 0.6 > B RD" (`MultiAreaTests.cs:77-78`). |

### 2.3 Annual equations (in execution order), `LAB/src/Core/AnnualRules.cs`

| # | Step | Equation (as coded) | Units | Anchor | Constant status (from research docs) |
|---|---|---|---|---|---|
| E1 | Year | `year = EcologicalYear + 1` (checked) | yr | `:15` | — |
| E2 | Living set | living trees, ordinal by `TreeId` | — | `:16` | — |
| E3 | Hegyi CI (pre-growth, area-local) | `CI_i = Σ_{j≠i, d_ij ≤ 8 m} (D_j / max(1, D_i)) / max(0.5, d_ij)` | **m⁻¹**. The DBH ratio is dimensionless (both cm, floor 1 cm), and it is divided by a distance in metres (floor 0.5 m). | `:67-82`; constants `:6-7` | Method [B]; 8 m cutoff [C] (`SGM/EvidenceMatrix.csv` row 16). **Precision:** float32 throughout. `dx`, `dz` and `dx·dx + dz·dz` are float32; `Math.Sqrt` runs in double and is cast back to float32; the sum is accumulated in float32 over living trees in ordinal `TreeId` order (`:69-81`). |
| E4 | Growth response | `r_i = 1 / (1 + CI_i / CI50)`, with **CI50 = 5 m⁻¹**: the same metre-based convention as CI, so `CI/CI50` and `r` are dimensionless | dimensionless | `:88`; `Ci50 = 5` `:8` | Response form and CI50 are abstraction/calibration (`RES/CCF_Growth_Density…Composite.md` §5). CI50 is meaningful only with distances in metres and DBH ratios as coded. Re-expressing CI in another length unit would change both CI and CI50; **no such conversion is made or implied here.** float32 (`1f/(1f + ci/5f)`). |
| E5 | DBH increment | `ΔD = 1.2 · site · clamp(1 − D/120, 0, 1) · r_i`, then `D ← clamp(D+ΔD, 1, 200)` | cm per annual step | `:89-91`; constants `:9-10` | 1.2 cm/yr [C], evidence range 0.9–1.5 (EvidenceMatrix row 10). `site` = 1.0f (`DerivedManifest.cs:50`). **Age does not enter DBH growth.** The explicit size taper `clamp(1 − D/120)` slows growth as D rises and gives **ΔD = 0 at D ≥ 120 cm**. There is no direct N/ha or RD term. float32 throughout. |
| E6 | Height | **Algebraic form:** `H ← clamp(H · E(max(1,age)+1) / E(max(1,age)), 0.1, 60)`, with `E(t) = b0 (1 − e^{−0.042 t})^{1.563}` and `b0 = 20.4 / (1 − e^{−0.042·30})^{1.563}`. **Evaluation as coded (not bit-identical to the algebra):** `h1 = (float)(H · E(a+1) / E(a))` in double with `H` widened, then float32 `g = h1 − H`, then float32 `H + g`, then the float32 clamp. | m | `AnnualRules.cs:92-93`; `CCF/SitkaGrowthModel.cs:37-60` | Lekwadi et al. 2012 Class III, b2/b3 [A], b0 anchor-matched to 20.4 m [A/C] (`SGM/SitkaGrowthModel1.md` lines 31-33). **No competition term; no site term.** The double envelope is cast back to float32 every step (`SitkaGrowthModel.cs:55-60`), so the ratio H/E is preserved only to float32 rounding (see W5). Reproducing exact bits needs the coded order, not the algebra. |
| E7 | Crown | `R_pot = max(0.1, 0.9415 + 0.07635·D)`, `R_target = R_pot · r_i`, `R ← max(0.1, R + 0.15 (R_target − R))` | m | `:94-98`; `CcfSourcePins.cs:28-30` | Tabbush & White envelope [B] (EvidenceMatrix row 13). Relaxation 0.15 lies inside the composite's 0.1–0.2/yr starting range (§5). Competition reduction is a model choice [D]. Uses the **post-growth** D and the pre-growth CI response; **the crown target responds to CI**, but crown feeds nothing back (output only). float32. |
| E8 | Age | `age ← age + 1` | yr | `:99` | — |
| E9 | Stand aggregates (post-growth) | `N/ha = count/areaHa`; `Dq = sqrt(ΣD²/count)`; `BA/ha = ΣD² · π/40000 / areaHa` | stems/ha, cm, m²/ha | `:102-118` (duplicated byte-for-byte in arithmetic as `WorkRules.Summarize`, `WorkContract.cs:122-138`) | `count/areaHa` is evaluated as int÷float32, then widened (`:113-114`). ΣD² is accumulated in double from float32 DBH (`:105-110`). Dq, BA/ha and RD are double. An empty living set returns an all-zero stand (`:111`). |
| E10 | Relative density | `RD = N/ha · (Dq/25)^{2.063} / 1868` | dimensionless | `SitkaGrowthModel.cs:67-75` | Comeau et al. 2010, British [B]. Irish transfer unverified (EvidenceMatrix row 23; `SGM/AdultMortalityCalibration.md` lines 15-17). |
| E11 | Density pressure | `P = max(0, (RD − 0.6)/0.4)`, using **post-growth** RD | — | `AnnualRules.cs:19-20`; `SitkaGrowthModel.cs:89,98-101` | onset 0.6 [C] (row 24) |
| E12 | Suppression (only if P > 0) | `S_i = 1 − 1/(1 + CI_i/CI50)`, using **pre-growth** CI | dimensionless | `AnnualRules.cs:27-33` | **Null guard:** when P ≤ 0, S is not computed and the audit holds `null` (unevaluated), not 0 (`:25-27,38`). **Precision:** `CI_i/CI50` is a float32 quotient; the rest is double (`:29`). |
| E13 | Annual death probability (named `hazard` in code) | `p_i = min(0.5, 0.08 · P² · V(S_i) / mean(V(S)))`, with `V(S) = clamp(S,0,1)⁴` | **dimensionless**: the Bernoulli probability of death in **one annual step**. It is **not** an instantaneous hazard rate per year, and no conversion to a rate is made or implied. | `AnnualRules.cs:39-40`; `SitkaGrowthModel.cs:90-96,103-114` | strength 0.08 [C], exponent 4 [C], cap 0.5 numerical (rows 25-26). **Zero/null guards:** p is `null` when S is null (P ≤ 0) (`AnnualRules.cs:39-40`). p is exactly 0 when P ≤ 0 or `mean(V) ≤ 0`, for example when no living tree has a neighbour within 8 m (`SitkaGrowthModel.cs:111`). The mean of V is accumulated in double over all living trees (`AnnualRules.cs:30-32`). |
| E14 | Mortality draw | Drawn only when `p_i > 0`: `u = SimulationRandom.Roll(model 1, "ADULT-MORT-"+TreeId, year, areaSeed)`. The tree dies if `u < p_i`. | uniform [0,1) with 24-bit resolution | `AnnualRules.cs:41-53`; `CCF/SimulationRandom.cs:41-55` | `u` is `null` (not drawn) when p is null or 0 (`:41-44`). `u` is float32 and compared against the double p. |
| E15 | Boundary (only if P > 0 and survivors > 1) | While survivor RD > 1, kill the survivor with the highest pre-growth S (ties by ordinal ID). **Within this boundary step** the last survivor is kept (`:124,134`). | — | `AnnualRules.cs:58-59,120-142` | RD ≤ 1 is the maximum line [B]. **Do not generalise the last-survivor guard.** The hazard step (E13–E14) has no survivor guard: its separately keyed per-tree draws can in principle kill every living tree of an area in one year (`:35-56`), and the boundary then returns at once with ≤ 1 survivor (`:124`). See W15. |
| E16 | Next state | dead trees carried forward unchanged; report holds Before / PostGrowth / After stands and a per-tree audit | — | `:60-64`; `MultiAreaContinuation.cs:33-35` | Audit rows for trees already dead at the start of the year carry the placeholders `CI = 0f` and neighbours 0, with S/p/u `null` (`:62-63`). These are **not computed values** and must be logged as "not evaluated". |

### 2.4 Unit, numeric and fixed-point conventions

- **Source integers.** Positions, DBH and height enter as **integer millimetres** (`DerivedManifest.cs:41-43,77-79`; `MultiAreaExperiment.cs:38,116-123`). Polygon geometry is exact `long`/`Int128` (`MultiAreaGeometry.cs:19-91`).
- **Units of the derived quantities.** CI is m⁻¹ and CI50 = 5 m⁻¹ (E3–E4). r, S, P and RD are dimensionless. p is a dimensionless probability per annual step (E13). ΔD is cm per annual step.
- **Tree state.** Tree state is **float32** (`AnnualTree`). Authoritative serialised float32 values are stored as IEEE-754 bit patterns (`F32.cs:10-28`). `areaHa` for P1 is the bit pattern `0x3db851ec` (`DerivedManifest.cs:46`). MA areas use their own float32 `areaHa` from geometry (§2.2); this is not the P1 value.
- **Per-step precision.** These are as coded at `4cecf873`:
  - CI, r, ΔD, DBH, crown and `areaHa` are float32. The CI sum is accumulated in **float32** (E3).
  - Height: a binary64 envelope ratio is cast back to float32 **every step**, followed by a float32 subtract, add and clamp (E6).
  - Stand aggregates use double accumulation of ΣD² from float32 DBH. Dq, BA/ha and RD are binary64. **N/ha is the exception**: an int ÷ float32 division, then widened (`AnnualRules.cs:105-117`).
  - S takes a float32 `CI/CI50` quotient into double. p, P and mean V are double (E12–E13).
  - "Stored as float32 bits" describes storage. It does not prove that every UI or JSON output uses that representation.
- **Exposure.** No fixed-point or quantised biology is used beyond float32 storage. Float32 DBH spacing at 64–128 cm is 2⁻¹⁷ cm ≈ 7.63·10⁻⁶ cm. **This does not guarantee that every increment registers.** ΔD is exactly 0 at D ≥ 120 cm (E5 taper). As D approaches 120 cm, `ΔD` can fall below half the local float32 spacing, so `D + ΔD` rounds to D and growth stops before the taper reaches zero.

### 2.5 Determinism and seeding

- **RNG and the roll key.** The RNG is model 1 (splitmix64) (`DerivedManifest.cs:49`; `SimulationRandom.cs:41-66`). The roll key is **(TreeId, ecological year, area seed)**. A tree therefore draws the same `u` in a given year whatever was felled around it. Paired control/treatment sessions automatically share per-tree-year random numbers (common random numbers). The death **probability** still differs because P and S differ.
- **Independence of areas.** Each area depends only on its own state, seed and hectares. Evaluation order cannot matter (asserted in `MultiAreaTests.cs:209-213`).
- **No hidden inputs.** No wall clock, no `System.Random` and no session identity enters biology. The random `sessionId` (`MultiAreaContinuation.cs:55-57`, 16 cryptographically random bytes as lowercase hex) does not enter biology, the source token (`:504-507`) or the annual report. **It is not absent from outputs**, though. It is `View.SessionId` (`:532`; `MultiAreaContracts.cs:88-90`), it is required in every fence (`:462-466`), and it is embedded in every minted `PlanId` `<sessionId>:ma-plan:<n>` and execution key `<PlanId>:execute` (`:191-192,510`). Those IDs are copied into `Approved`, every `MultiAreaWorkEvent` and every `MultiAreaFellRecord` (`:295,309`; `MultiAreaContracts.cs:37-38,45-46`). **Two fresh sessions therefore do not produce byte-identical full views, even at genesis**, barring a 128-bit random ID collision.
- **Cross-session comparison projection (replaces full-view equality; F1).** Repeat and isolation checks compare this deterministic projection, never the raw view. Random session generation is kept unchanged.
  1. **Compared directly, bytes must be equal.**
     - `MultiAreaCommandResult.Annual` (all per-area reports, including `null` for a carried-forward area);
     - `View.Areas` (rosters, stands, Crop marks, geometry);
     - `View.Source` (the token; it has no session field);
     - `View.Experiment`, `View.PlanStateRevision` and `View.AnnualHistory`;
     - `RefusalCode` for each command.

     Author test MA-E03 compares exactly `Annual` and `View.Areas` for ten no-work years (`MultiAreaTests.cs:216-232`). This projection extends that.
  2. **Checked within each session, then excluded from the cross-session comparison.**
     - `View.SessionId` is 32 lowercase hex characters, constant for the session, and equal to every fence the harness sends.
     - Every `PlanId` equals `SessionId + ":ma-plan:" + n`, where n is the `PlanStateRevision` produced by that Approve.
     - Every execution key equals `PlanId + ":execute"`.
     - Every Fell record's `PlanId`/`ExecutionKey` equals its event's.

     Any violation is a reported defect, not a normalisation case.
  3. **Normalised, then compared.** In `Pending`, `Approved`, `Events` (with their Fell records) and every `Execution` result, replace the exact leading `SessionId + ":"` of each `PlanId` and execution key with the fixed literal `SESSION:`. Then require byte equality of the normalised ledgers. Nothing else is rewritten.
  4. **Serialisation and order.** Use `System.Text.Json` `JsonSerializer.SerializeToUtf8Bytes` with default options, as the author tests do (`MultiAreaTests.cs:702`). Run on the same source SHA, build and runtime for both sessions. Property order is record declaration order. Collection order is the engine's own:
     - areas ordinal by `AreaId`;
     - audit rows and rosters ordinal by `TreeId`;
     - history and events in commit order;
     - Fell records ordinal by `TreeId` (`:305`).

     The harness must not re-sort.
  5. **Float representation.** Alongside the JSON bytes, the harness log records every float32 as its `F32.Bits` 32-bit pattern (8 hex digits) and every double as its 64-bit IEEE-754 pattern (16 hex digits). Equality is bit equality, so the decision never depends on decimal formatting. A JSON-byte mismatch with all bit patterns equal is reported as a serialiser difference, not a biological one.
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
| Advance (Year+1) | Unexecuted work needs consent. Consent cancels **all** of it at 0 Fell (`CancelledUnexecutedWork=true`). A naturally empty area is carried forward, not simulated. **Biological refusal: only when every area is empty.** The check order is: first the fence (session, source token, plan-state revision), then work consent, then the all-empty check. So a valid fence and any required consent come first (`:345-351`, `:451-466`). | `PlayerSessionMismatch`, `PlayerSourceStale`, `PlayerPlanStateStale`, `WorkPendingConsentRequired`, then `ContinuationNoLivingStems`. Separately, a `checked` int32 overflow raises an overflow exception; it is not a refusal code (`:354,375,387,395,401`). | `:340-405`; `:346-351,372-379` |
| Fell record | `fell` / `manual-work-plan`, reason, `EcologicalYear = y` (the year *before* the next advance), work revision, key, PlanId | — | `:291-296`; `MultiAreaContracts.cs:37-38` |

### 2.8 Year+1 chronology (as coded)

1. In year **y**: mark, then plan, then Approve (0 Fell), then Execute. Felled trees get `Death.EcologicalYear = y`; the year and annual revision are unchanged (`MultiAreaContinuation.cs:278-279,293`; asserted `MultiAreaTests.cs:385-386`).
2. Advance: each living area runs E1–E16 on its **post-Fell** roster, giving year **y+1**. Natural deaths carry year y+1 (`AnnualRules.cs:15,49,138`). The Year+1 report equals `Calculate` on the post-Fell roster (asserted `MultiAreaTests.cs:407-427`).
3. Within one annual step, CI is computed **before** growth on the pre-growth DBH. Density pressure uses the **post-growth** stand. Hazard and boundary use pre-growth suppression (`AnnualRules.cs:17-33,132`).

### 2.9 Maximum lawful horizon (from code)

- **No year cap in MA-1.** `MultiAreaContinuation.Advance` has no year cap (`:340-405`). Its stopping conditions are of three different kinds:
  - **Biological stop.** Given a valid fence and any required work consent, the only biological stopping condition is every area being empty: `ContinuationNoLivingStems` (`:350-351`).
  - **Procedural refusals.** These refuse a single Advance before the all-empty check and do not end the horizon: a stale or mismatched fence (`PlayerSessionMismatch`, `PlayerSourceStale`, `PlayerPlanStateStale`; `:345`, `:451-466`), and unexecuted work without consent (`WorkPendingConsentRequired`; `:346-349`).
  - **Arithmetic limit.** A `checked` int32 overflow of year, revisions or age (`:354`, `:395`, `AnnualRules.cs:15,99`) raises an overflow exception, not a refusal code. It occurs at about 2.1·10⁹ years. How a route surfaces that exception was not inspected.
- **50 years is permitted by the rules.** Neither the code nor any MA-1 refusal stops at a year count. A single-area test explicitly asserts "no ten-year cap" at 12 years (`ContinuationTests.cs:44-53`).
- **Legacy one-year paths do not apply.** The legacy `AnnualEngine.Advance` (CLI `annual`) is a one-year experiment (`AnnualEngine.cs:84-85`; `LAB/src/Cli/Program.cs:56-81`). It is not the MA-1 path.
- **Exercised horizons so far.** Author tests exercise at most **10** multi-area years (`MultiAreaTests.cs:216-232`) and 12 single-area synthetic years. **No 20- or 50-year MA-1 run exists in evidence.**
- **How 50 years would be verified in Q1 (not done).** Run two fresh sessions to year 50 and assert:
  - `EcologicalYear == AnnualRevision == AnnualHistory.Count == 50` with no refusal;
  - equality of the §2.5 **cross-session comparison projection** at every year. Full views are **not** compared byte-for-byte: they differ by the random `SessionId` and the session-derived `PlanId`/key by design (F1);
  - every area in each year is in one of three states: living; extinct in that year, meaning a non-null report with `After.LivingCount == 0` and history `NoLivingStems = false`; or carried forward as `NoLivingStems = true` with a `null` report in a later year (W15).

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

The root keeps only the current roster plus stand-level history. **Per-tree yearly trajectories (DBH, height, crown, CI, p, u) are therefore not retained by the session unless the caller captures every returned report.** A Q1 harness must log every report. In principle they can be regenerated by deterministic replay of the full ordered command transcript on the same source SHA, experiment definition, build and runtime (§2.5; §6 Logging). That replay equivalence is itself unverified until run. This is a retention risk, not absolute irreproducibility (W11). For an area carried forward as empty, the report is `null` (`MultiAreaContracts.cs:63-64`), so there are no per-tree rows for that area-year.

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
| X3 | DBH increment (E5). DBH is the **principal** competition-responsive growth variable; the crown target also responds to CI (E7), but crown is output only. | §5 (`ΔDBH = G_D(age, DBH, site) × competitionMultiplier × vitality`; DBH principal) | §14.2 (individual-tree increments under release NOT validated by GROWFOR) | 10, 12 | **SIMPLIFICATION**: no age term, no vitality term, site fixed at 1.0. Potential 1.2 cm/yr is [C]. The size taper `clamp(1 − D/120)` is the only size/age-like modifier. Unthinned absolute growth after about 16 cm is "later unsupported (no density feedback)" (EM 12). |
| X4 | Hegyi CI (m⁻¹) with `r = 1/(1+CI/CI50)`, CI50 = 5 m⁻¹ (E3–E4) | §5 (Hegyi mechanism evidence-based; response and CI50 are abstraction) | §2A.4 / §14.2 (local Hegyi release not validated by GROWFOR) | 16 | **AGREE** (form). Known limitation, narrowed: on a **fixed roster**, uniform proportional DBH scaling leaves CI unchanged (exactly in algebra, while every D_i ≥ 1 cm), so CI carries **no direct occupancy or RD signal**, and DBH growth has **no direct N/ha or RD term**. DBH growth **does** still slow with size through the explicit `clamp(1 − D/120)` taper (E5). CI also changes when DBH ratios diverge or neighbours die or are felled. Stand density acts directly only through mortality (E11–E15). Source: `SGM/CurrentModelAudit.md` structural finding 2, l.54. That historical audit predates the current mortality model, and this row does not claim its other conclusions. |
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
  - **S**: staged. L's rule applied at year 0 and again at year 10, against one equivalent single intervention **S1** (defined below).
  - **I**: isolation. Thin MA-A only; leave MA-B untouched.
  - L and M use **one shared two-area Work Plan** per intervention, which is the required two-area shared-plan case.
- **Selection rule (pre-registered; frozen before any session is run; F5, R2).** Rule (a), Crop-tree release, is the primary rule. Every step is computed from one **intervention-time state**: the roster at the moment of intervention, before any Crop mark or order of that intervention is issued. No step refers to any later outcome. **One algorithm is used for L, M, D, S and S1 (and for MA-A in I): frozen lists, a fixed traversal, then first-occurrence deduplication without replacement.** Sequential planning with exclusion before ranking is **not** used.
  1. **Crop selection.** Rank the area's living trees by DBH, descending, with ties broken by ordinal `TreeId`. Walk that list once, top to bottom. Accept a tree as Crop if its float32 E3 distance to every already-accepted Crop is **> 8 m** (the engine's own Hegyi cutoff; no new coefficient). **Stopping rule:** stop when the ranked list is exhausted. Crops therefore never appear in each other's 8 m neighbour sets. The DBH order is used **only** to select the Crop set; it is not the traversal order of the later steps.
  2. **Frozen competitor lists.** For **every** Crop, from the same intervention-time state and before any selection, list its living neighbours within 8 m (the same set E3 uses). Rank them by their Hegyi term `D_j / max(1, D_crop) / max(0.5, d)`, computed in float32 as in E3, descending, with ties broken by ordinal `TreeId`. A tree that appears on several Crops' lists stays on all of them; nothing is excluded at this stage. By step 1 no Crop can appear on any list; if one does, report it as a defect.
  3. **Removal sequence (fixed traversal and deduplication).** Work per area, with areas in ordinal `AreaId` order. Build the sequence by **rank layer**. Layer k contains, for each Crop in **ascending ordinal Crop `TreeId`**, that Crop's rank-k competitor, if it has one. Concatenate layer 1, then layer 2, then layer 3, and so on. Then **deduplicate at first occurrence**: when a `TreeId` is already earlier in the sequence, drop the later entry. The affected Crop **does not** move on to its next competitor; the dropped slot is **not** replaced. Log every dropped duplicate, with its Crop and layer.
  4. **Intensity.**
     - **L** takes the deduplicated layer-1 entries.
     - **M** takes the deduplicated entries of layers 1–3, or fewer where a Crop has fewer competitors.
     - Because layer 1 comes first in the one sequence, every L entry is also an M entry.
     - These counts are **protocol design choices, not ecological coefficients**. The Manager may change them before Q2 starts, never after any outcome is seen. **No percentage is fixed.**
  5. **Engine command order and the empty case.**
     - (a) **Issue the Crop marks first.** Mark every Crop of the treated areas, areas in ordinal `AreaId` order and Crops in ascending ordinal `TreeId`. Do this before any `AddPending`: `MarkCrop` refuses a tree that is already planned (`CropAlreadyPlannedForFell`, `MultiAreaContinuation.cs:96-98`), and `AddPending` refuses a Crop-retained tree (`:136-158`). At an S year-10 intervention, the surviving year-0 marks are still in place and are not re-issued; re-marking would be refused as already marked.
     - (b) **Issue the orders.** Call `AddPending` for each selected entry in sequence order: areas in `AreaId` order, then the step-3 order within each area.
     - (c) **Empty order set.** If the selected order set over all treated areas is empty, do **not** call Approve, because Approve would refuse `WorkEmptyPlan` (`:188-189`). Record a **"no intervention"** result for that scheduled intervention, with its Crop lists and frozen competitor lists, and continue the scenario. If only one area has no orders, record "no orders in area X"; the plan is then single-area.
     - (d) Otherwise, Approve, disclose (step 6) and Execute. Crops are never ordered, so every populated area keeps at least one living tree. Rule (a) therefore cannot trigger `WorkWouldLeaveNoLivingStems`.
  6. **Disclosure before each Execute.** Log the Crop list, each Crop's frozen ranked competitor list, the deduplicated removal sequence with its dropped duplicates, and the exact `Approved.Orders` (area, `TreeId`, reason), plus the counts per area. Log these **before** calling Execute.
  7. **Realised intensity.** Record removed stems, removed BA and the removed fraction of both, per area, from the engine's own `MultiAreaWorkEvent` per-area Before/After (`MultiAreaContinuation.cs:302-305`).
  8. **"Genuinely different."** With one Crop set and one deduplicated sequence, M's orders ⊇ L's, so M's removed BA ≥ L's. M is "genuinely heavier" only if its removed BA is strictly greater in **both** areas. If not, report "not genuinely different in area X" and do not force it.
  9. **Rule (b) is not used.** Removal from below by DBH rank would need a numeric stopping threshold that this Q0 cannot justify. It may be added only with a Manager-approved threshold pre-registered before any run.
  10. **D and S timing.**
      - D applies steps 1–8, with L's intensity, to the year-5 state.
      - S applies steps 1–8, with L's intensity, at year 0.
      - At year 10, S keeps the surviving year-0 Crops; marks of dead Crops are cleared by the engine (`:380-384`). It freezes their competitor lists from the year-10 state (step 2), builds the year-10 sequence (step 3) and takes its deduplicated layer-1 entries. No new Crop is added at year 10.
      - **I** applies steps 1–8, with L's intensity, to MA-A only. It issues no Crop mark and no order in MA-B, so MA-B's plan state stays comparable with C for the isolation check.
- **Equivalent single intervention S1 (F5, R2).**
  - **Definition.** S1 is one intervention at year 0 from the same Crop set. It fells the first `n_A` and `n_B` entries of the **same** year-0 deduplicated removal sequence of steps 2–3, over all layers. `n_A` and `n_B` are the **total realised stems** S removed per area across years 0 and 10. **The primary equivalence is therefore removed stems per area, which is exact.**
  - **Sizing.** S1 is sized from S's realised counts only, never from any S1 outcome.
  - **Mismatches, reported and not hidden.** Per area:
    - (i) removed BA of S (year-0 BA plus year-10 BA, each at its time of removal) versus S1, as absolute and relative differences;
    - (ii) removed-ID identity: the S-only IDs, S1-only IDs and the overlap count;
    - (iii) timing: two removal years versus one.
  - **Not constructible.** If the year-0 deduplicated removal sequence of area X holds fewer than `n_X` trees, S1 is reported as "not constructible in area X" with the shortfall. No substitute is made.
- **Horizons.** **Capture every committed year 1…H** (F5). Years **1, 5, 10, 20** and **50** are only the summary checkpoints computed from that all-year log; they are not the only years recorded. 50 is lawful by the code (§2.9) but has never been exercised. If both areas empty before 50, report the last committed year and stop.
- **Repeats.**
  - (i) Determinism: two fresh sessions per scenario must have an equal §2.5 **cross-session comparison projection** at every year and after every command. Raw full views are not compared, because `SessionId` and the session-derived `PlanId`/key differ by design (F1).
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
  - All-empty Year+1 (`ContinuationNoLivingStems`): **not reachable** on the canonical experiment by player action, because Fell cannot empty an area. It is covered by author test MA-E05 only (`MultiAreaTests.cs:258-286`, test-only experiment). It is **not impossible by natural mortality**: the hazard step has no survivor guard (E15, W15). Whether any canonical scenario empties an area is UNKNOWN until run. Report it as "not exercised on canonical data" unless it occurs naturally or a disclosed variant is authorised.
- **Logging (F5).** The harness keeps a complete, ordered, append-only transcript. This is what makes deterministic replay and the W11 check possible.
  - **Every command** in issue order, with:
    - its type (read view, Crop mark/unmark, `AddPending`, `RemovePending`, `Approve`, `CancelApproved`, `Execute`, `Advance`, `Competition`);
    - every argument (area, `TreeId`, reason, consent flag, `PlanId`, execution key);
    - the fence values sent.
  - **Every `MultiAreaCommandResult`**: `RefusalCode`/`RefusalMessage`, `Execution`, `Annual`, and the §2.5 projection of `View` (raw `SessionId` recorded once per session).
  - **Header**: source SHA, experiment content hash, harness SHA-256, build and runtime identity, and session start order.
- **Null and empty semantics (F5).**
  - **`null` means "not evaluated", never zero.** S, p and u are `null` when P ≤ 0. u is `null` when p is `null` or 0. p = 0 is a real evaluated zero (E12–E14).
  - Dead carried rows hold placeholder CI 0 and neighbours 0. These are logged as "not evaluated", not as measured zeros (E16).
  - **Extinction year (R3).** In the year an area's last living trees die, its report is still non-null, with `After.LivingCount == 0` (`AnnualEngine.cs:17`). Its history entry has `NoLivingStems = false` (`MultiAreaContinuation.cs:364-370`). That year has normal per-tree rows and is logged as the area's extinction year.
  - **Empty area (carry-forward).** An area carried forward empty in a **later** committed year has `Report = null` (`MultiAreaContracts.cs:63-64`) and history `NoLivingStems = true` (`MultiAreaContinuation.cs:372-378`). The log records one "carried forward, no per-tree audit" row for that area-year and zero per-tree rows. This is not missing data. Its stand is the all-zero stand (E9). If every area is empty, the next Advance is refused `ContinuationNoLivingStems` (`:350-351`) and no carry-forward year commits.
  - Means over an empty set (for example, mean DBH of zero living trees) are reported as `null`, never 0.
- **Measurements, per area, for every committed year, from every returned report.**
  - Per tree: ID, age, DBH, height, crown radius, pre-growth CI (m⁻¹), neighbour count, S, p (per annual step), u, death year/cause/mechanism, with the null semantics above.
  - Counts: living, hazard deaths, boundary deaths, cumulative natural deaths, cumulative Fell. Per area, also the extinction year and the first carry-forward year, each logged separately or recorded as "none" (W15).
  - Stand: N/ha, **areaHa-normalised BA**, Dq, arithmetic mean DBH (computed by the harness, for Frenchpark-style direction, with the decomposition below), RD, density pressure, mean vulnerability.
  - Plan and work ledger: work ledger, plan IDs and keys (raw, plus the §2.5 normalised form).
  - **No-double-application check**: every Fell ID appears once in the ledger and dies once. The sum of Fell plus natural deaths equals genesis minus living.
  - **Isolation check**: in session I versus session C, at every year, these must be byte- and bit-equal: MA-B's `View.Areas[B]`, `Annual.Areas[B].Report` and every history `Areas[B]` entry. Global fields that legitimately differ (`WorkRevision`, `PlanStateRevision`, the event ledger, `CancelledUnexecutedWork`, `SessionId`) are excluded and listed.
- **Valid comparisons (F5).**
  - **Retained-tree growth.** The **matched retained set** for a window [t₀, t₁] is the set of `TreeId`s living at t₀ and at t₁ in **every** compared session, and never felled in any of them. This is valid because every session starts from the same canonical genesis IDs. For each such tree, report `ΔD = D(t₁) − D(t₀)` and the paired difference `ΔD_L − ΔD_C` (likewise for M and C). The set is conditional on survival in all sessions; report its size and its share of genesis.
  - **Selection-only shift.** At the instant of each Execute, with the year unchanged and no growth, report the arithmetic mean DBH and Dq of the post-Fell living set minus those of the pre-Fell set, per area, from the event's Before/After and the roster.
  - **Stand mean DBH.** This is reported only alongside both components above. It mixes retained growth, removal selection and natural-mortality selection, and is never used alone as evidence of a growth response.
  - **Frenchpark comparison.** Only the **sign** of the matched retained-tree increment difference is compared with Frenchpark (composite §5) and EM 11. State explicitly that these are synthetic stands, not yield plots, and not comparable to Frenchpark's site, age or duration.
  - **Excluded.** No BA-threshold, windthrow, recruitment or volume check (§5.1).

---

## 7. Weakness list (time-boxed, falsifiable, ranked by consequence)

Revision 2 (F4) separates **established limits** from **falsifiable hypotheses**:
- Established limits are facts read from code or documentation. They need no hypothesis test, only optional runtime confirmation.
- Hypotheses have a rejection criterion, a tolerance and named confounders.

The W1–W12 identifiers from revision 1 are kept for traceability. W13–W15 are added from the review's "missing consequential weaknesses". **None is a measured result.** Every tolerance and decision threshold below is a **protocol choice made here, not an ecological coefficient**. The Manager may change any of them before Q2 starts, never after any outcome is seen.

### 7.1 Consequence ranking and its rationale

Items are grouped in tiers by **what they can invalidate**. Order *between* tiers is the claim. Order *within* a tier is presentation only; no ordinal consequence within a tier is claimed.

| Tier | Criterion | Items |
|---|---|---|
| T1 | Makes a v1 qualification gate fail or be undetermined **regardless of biology** | W13 repeat-gate defect (F1); W14 treatment underdetermination (F5) |
| T2 | Removes a whole class of CCF outcomes at **every** horizon | W1 no recruitment |
| T3 | Could reverse the **sign or direction** of the treatment comparisons §6 allows | W3 mortality calibration transfer; W6 single seed per area; W4 edge exclusion |
| T4 | Changes magnitude or interpretation of growth/mortality, not the protocol's validity | W2 no direct occupancy feedback in growth; W15 hazard can empty an area; W7 no age term; W8 crown output only |
| T5 | Implementation conventions and resolution | W5 site-fixed height; W9 mixed chronology; W10 granularity |
| T6 | Record keeping and documentation | W11 retention/replay; W12 documentation drift |

W13 and W14 rank above W12 because a defective gate invalidates conclusions, whereas README drift misleads a reader but changes no result. Revision 2 corrects both defects in the protocol text (§2.5, §6). Revision 3 completes the W14 correction by fixing one selection traversal (R2). Their residual risk is that a Q1 harness fails to implement those corrections, so they stay listed.

### 7.2 Established limits (code or documentation facts; no hypothesis test)

| ID | Limit | Anchor | Optional runtime confirmation (time box) |
|---|---|---|---|
| W13 | **v1 repeat gate was structurally impossible.** Revision 1 required byte-identical full views across fresh sessions, but `SessionId` is random and embedded in `PlanId`, keys and Fell records. That gate fails regardless of biology. Corrected by the §2.5 projection. | `MultiAreaContinuation.cs:55-57,191-192,510,532`; `MultiAreaContracts.cs:88-90` | Q1 asserts the projection's within-session ID checks and cross-session equality (§2.5). (0.5 h) |
| W14 | **v1 treatments were underdetermined.** "Equivalent" single intervention, adaptive intensity and stand-mean comparisons were undefined, so treatment conclusions could not be fixed in advance. Corrected by the §6 pre-registered rule, S1 definition, matched-tree comparison and logging. Revision 2 still left the shared-competitor traversal open; revision 3 fixes it with one algorithm (§6 steps 2–5; R2). | §6 | Q1 logs the disclosure record, including the dropped duplicates, before every Execute (§6). (0.25 h) |
| W1 | **No recruitment.** N/ha is non-increasing, so CCF transformation and regeneration outcomes cannot appear at any horizon. | `DerivedManifest.cs:58-59`; no recruitment code in `LAB/src/Core` | Assert living count is non-increasing year on year in every scenario. Any increase is a defect report, not a biological finding. (Q2 log analysis, 0.5 h) |
| W15 | **Last-survivor guard is boundary-only.** The boundary step keeps its last survivor, but hazard draws have no guard (p ≤ 0.5 per tree, with u keyed separately per tree, year and area seed), so natural mortality can in principle empty an area. In the year an area empties, its report is non-null with `After.LivingCount == 0` and `NoLivingStems = false`. Only in a **later** committed year is it carried forward with `Report = null` and `NoLivingStems = true`. Whether any of this happens on canonical data is UNKNOWN. | `AnnualRules.cs:35-56,124,134`; `AnnualEngine.cs:17`; `MultiAreaContinuation.cs:350-351,364-379` | Per scenario and area (R3): (i) the **extinction year**: the first committed year whose report has `After.LivingCount == 0`; (ii) **separately**, the first carry-forward year (`NoLivingStems = true`). This is **not** the extinction year, and it never commits if every area is empty, because the next Advance is then refused `ContinuationNoLivingStems`; record "no carry-forward year (all areas empty)". The rolls are deterministic and keyed, so they support **no claim** that statistical independence between trees or areas has been demonstrated. **Simultaneous extinction** of both areas in one year is an unverified possibility, **not a canonical result**. (0.1 h) |
| W7 | **No age term in DBH growth.** E5 uses only D, CI and site (with the D/120 taper), so equal-D, equal-CI trees grow identically whatever their age. Revision 1's matched 47/29-year pair test is dropped: such pairs need not exist, and the fact follows from code. | `AnnualRules.cs:88-91`; composite §5 `G_D(age,…)` | Covered by the H-W2(b) recomputation below: a tree-year whose ΔD departs from `f(D, CI)` would refute it. (0 h extra) |
| W8 | **Crown is output only.** The crown target responds to CI, but crown feeds no competition, growth or mortality term. | `AnnualRules.cs:94-98` vs `:17-33,67-91` | Read-only evidence is sufficient. Perturbing crown alone would need a variant. (0 h) |
| W5 | **Site label versus fixed trajectory.** Site is reported `UNKNOWN`, but height follows the hard-coded Class III envelope and DBH uses `site = 1.0f`. That disclosure needs wording; the ratio check below cannot decide whether "site unknown" is right. | `MultiAreaExperiment.cs:20,28-29`; `SitkaGrowthModel.cs:37-60`; `AnnualRules.cs:89` | **Implementation check (not a hypothesis).** Let `ρ_i(t) = H_i(t)/E(age_i(t))` be computed in double from logged float32 H. Expect `|ρ_i(t)/ρ_i(0) − 1| ≤ 8·t·2⁻²⁴` for every living tree, with t the number of steps. That allows for float32 rounding of about 3–4 operations per step; exact ratio equality is **not** expected (E6). Trees that touch the 0.1 m or 60 m clamp are excluded and listed. A larger drift is reported as a defect in the E6 description. (0.5 h) |
| W9 | **Mixed chronology.** Hazard and boundary use pre-growth suppression with post-growth RD. This is an implementation convention. | `AnnualRules.cs:17-33,132` | Descriptive only. Recompute S from post-growth CI offline on logged rows and count how many boundary-order choices would change. No pass/fail. (1 h) |
| W10 | **Small-area granularity.** One stem is about 10.9/ha in MA-A and about 6.7/ha in MA-B (nominal, before float32 `areaHa` rounding), so stand metrics move in coarse steps. | `MultiAreaGeometry.cs:130`; E9 | Report the N/ha step size alongside every intensity. (0.25 h) |
| W11 | **Retention/replay, not absolute irreproducibility.** Per-tree yearly audit rows are not retained by the session (§3). If the harness does not retain every report, trajectories are recoverable only by deterministic replay of the full logged transcript on the same source SHA, definition, build and runtime, and that replay equivalence is unverified until run. | `MultiAreaContracts.cs:56-67` | (a) **Row count.** The expected per-tree rows are Σ over area-years with non-null `Report` of that report's audit-row count, which includes dead carried rows. A `null` report contributes **zero** rows plus one "carried forward" marker row. The log must match exactly. (b) **Replay.** Replay the transcript in a fresh session and require §2.5 projection equality at every step. (0.5 h) |
| W12 | **Documentation drift.** `LAB/README.md:153` says CLI `annual` is "always REFUSED in F0", but `AnnualEngine.RequireAnnualDomain` admits an accepted fixture (`AnnualEngine.cs:57-63`). The review notes that the README's exact command lacks the fixture directory: per code it returns usage exit 64 after admission, and the same command with a directory supplied opens and advances (review anchor 40). | as cited; `LAB/src/Cli/Program.cs:56-81` | Q1 runs both command forms once and records the exit code and output. Runtime outcome UNKNOWN until run. (0.1 h) |

### 7.3 Falsifiable hypotheses (rejection criterion, tolerance, confounders)

| ID | Hypothesis | Anchor | Test, rejection criterion and tolerance | Confounders and limits (time box) |
|---|---|---|---|---|
| H-W2 (narrowed W2; F3) | (a) On a fixed roster, CI is invariant to uniform proportional DBH scaling, so CI carries no direct occupancy/RD signal. (b) Annual ΔD is a function of pre-growth D and CI only (and site): there is no direct N/ha or RD term, while growth still slows through the `clamp(1 − D/120)` taper. | `AnnualRules.cs:67-91`; `SGM/CurrentModelAudit.md` l.54 | (a) Offline, on logged year-t rosters of sessions C and M: recompute every living tree's CI from logged positions and DBH, then again with all DBH × λ, λ ∈ {1.1, 1.5}. **Reject (a)** if any tree with `λ·D_i ≥ 1 cm` has `|CI_λ − CI| > 1·10⁻⁵ · max(CI, 1 m⁻¹)`. That tolerance covers float32 accumulation over the roughly 40 neighbours (π·8² m² ÷ 2.24² m²) a 2.24 m grid places within 8 m. (b) For every logged living tree-year, recompute ΔD from logged pre-growth D and CI with the E4–E5 operations in float32, in the coded order. **Reject (b)** if any `ΔD_obs` differs from `ΔD_pred` by more than 1 ULP of D(t+1). | No assumed pre-mortality interval: MA-A starts **above** the RD onset (`MultiAreaTests.cs:77-78`), so CI and mortality are confounded from year 1, and (a) is tested on fixed rosters offline instead. (b) is a model-identity test; it cannot show that real Sitka lacks density feedback. (1 h) |
| H-W3 (model diagnostic, not a transfer test) | In canonical MA-A, the hazard step, not the boundary, does most of the natural thinning, as the calibration intended on the Unity 40 m stand. | `SitkaGrowthModel.cs:77-96`; `SGM/AdultMortalityCalibration.md` l.51-60 | Per scenario, count natural deaths by mechanism over each consecutive 10-year window and the whole horizon. **Diagnostic flag** if boundary deaths exceed 50 % of natural deaths in any window with ≥ 10 natural deaths. Windows with < 10 deaths are reported but not flagged. | **This diagnoses internal model behaviour at this area size only.** Neither outcome proves or disproves empirical transfer of the [C] constants to Irish stands, and a "hazard-dominated" result does **not** validate them. Confounders: initial RD (MA-A starts at author-reported 0.802); area size and stem count (W10); a single seed (W6). (1 h) |
| H-W4-CI (edge exclusion, competition; R1) | Trees within 8 m of their own polygon edge have systematically **lower** pre-growth CI than interior trees. Cause: neighbours outside the polygon are excluded, with no buffer or correction. | `MultiAreaExperiment.cs:25-27`; `AnnualRules.cs:71-80` | **Data.** Session C. Each area separately, at t = 0 and t = 10 **separately** (not pooled), stratified by genesis cohort (MA-A: 24; MA-B: 29 and 47). This gives 3 strata × 2 years = 6 stratum-years. CI_i is the logged pre-growth CI of tree i computed on the year-t state, i.e. the audit of the Advance from year t to t+1. D_i is the year-t roster DBH. Only trees living at year t are used. `edge_i = 1` if the double-precision distance from the tree's genesis integer-mm position to the nearest edge of its own polygon is < 8 m (trees do not move); otherwise 0. **Fit** `CI_i = a + b·edge_i + c·ln(D_i)` per stratum-year by ordinary least squares in double. **Per stratum-year outcome**, with the denominator `m_int = mean CI of interior trees` in that stratum-year: **BIAS** if `b < 0` and `|b| > 0.20·m_int`; **SMALL** if `b < 0` and `|b| ≤ 0.20·m_int`; **NO BIAS** if `b ≥ 0`; **NOT ESTIMABLE** under the shared rules below. **Overall CI conclusion:** **CONFIRM** if all 6 stratum-years are BIAS. **REJECT** if all 6 are NO BIAS. **INCOMPLETE** if any is NOT ESTIMABLE; the estimable stratum-years' outcomes are still reported. **MIXED** otherwise. | See "H-W4 shared rules" below the table. (0.75 h) |
| H-W4-G (edge exclusion, growth; R1) | Edge trees have **larger** 10-year DBH increments than interior trees on the retained set. This is a separate hypothesis from H-W4-CI, with the opposite sign. | as H-W4-CI; `AnnualRules.cs:88-91` | **Growth window:** one window, from t₀ = year 0 (the genesis roster) to t₁ = year 10 (the committed year-10 roster). **Retained set:** trees of the stratum living at year 0 and at year 10 in session C. C has no Fell, so this is the §6 matched retained set with C as the only session. Report its size and its share of each group. `ΔD_i = D_i(10) − D_i(0)`, computed in double from the logged float32 values. `edge_i` and the cohort strata are as in H-W4-CI, giving 3 strata. **Fit** `ΔD_i = a + g·edge_i + c·ln(D_i(0))` per stratum by OLS in double. **Per stratum outcome**, with the denominator `ΔD_int = mean ΔD of interior retained trees` in that stratum: **GROWTH ADVANTAGE** if `g > 0` and `g > 0.20·ΔD_int`; **SMALL** if `0 < g ≤ 0.20·ΔD_int`; **NO ADVANTAGE** if `g ≤ 0`; **NOT ESTIMABLE** under the shared rules below. **Overall growth conclusion:** **CONFIRM** if all 3 strata are GROWTH ADVANTAGE. **REJECT** if all 3 are NO ADVANTAGE. **INCOMPLETE** if any is NOT ESTIMABLE; the estimable strata's outcomes are still reported. **MIXED** otherwise. The negative-coefficient rule of H-W4-CI is **not** reused. | See "H-W4 shared rules" below the table. **Survival conditioning:** the retained set is conditional on surviving 10 years in C, and edge and interior trees may survive at different rates, so the retained share per group is reported next to `g`. (0.75 h) |
| H-W6 (single seed) | C-versus-L differences seen in the canonical single-seed run hold across stochastic realisations. | `MultiAreaGenerator.cs:41,46` | Requires a **Manager-authorised variant** differing only in area seeds; genesis is unchanged because the generator seed is separate. Pre-register k ≥ 10 seed pairs. For each per-area outcome (cumulative natural deaths at year 20; Dq at year 20; mean matched-retained ΔD over 0–20), record the sign of C − L per seed. **Reject robustness** of a canonical sign if any of the k seeds gives the opposite sign. Exact zeros are reported as "tie", not as agreement. | The variant has a different content hash and is labelled "variant, not MA-1 canonical". Common random numbers within a seed (§2.5) reduce, but do not remove, within-seed noise. (2 h, variant authorisation required) |

**H-W4 shared rules (R1).** These apply to both H-W4-CI and H-W4-G.
- **NOT ESTIMABLE.** A stratum-year (CI) or stratum (G) is NOT ESTIMABLE if any of these holds:
  - (i) it has fewer than 2 edge trees or fewer than 2 interior trees (for G, retained trees);
  - (ii) the n × 3 design matrix `[1, edge, ln D]` is rank-deficient, judged by its smallest singular value being ≤ 1·10⁻⁹ × its largest;
  - (iii) the denominator (`m_int` or `ΔD_int`) is ≤ 0.

  Each NOT ESTIMABLE entry is reported with `n_edge`, `n_interior` and the reason (i), (ii) or (iii). It is never counted towards CONFIRM or REJECT. Any NOT ESTIMABLE entry makes that conclusion INCOMPLETE, so the every-stratum rule is still defined when a stratum cannot be fitted. The minimum counts and the rank tolerance are protocol choices.
- **Separate conclusions.** The CI conclusion and the growth conclusion are reported separately. Neither one implies the other, and they are not combined into a single verdict.
- **What the adjustment is.** The `ln(D)` term is a **chosen adjustment** for the DBH ratio inside CI and for size-dependent growth. It does **not** show that DBH confounding has been removed, and it does not show that the edge causes the difference. Edge and interior groups can still differ in DBH distribution, generator jitter or neighbour count, and the linear form may be wrong. Every H-W4 result is a **descriptive, model-specific** summary of this synthetic census under this linear model. It is not a causal estimate.
- **Census, not sample.** No p-value is computed (see the §0.1 disagreement sentence).
- **Dependence.** Neighbouring trees share neighbours and successive years share trees, so trees are not treated as independent replicates and years are not pooled.
- **Age.** Stratify by cohort.
- **Spacing.** Generator jitter (2.24 m and 4.3 m grids) is uniform by design. Report the neighbour count per group, so that truncation, the effect itself, is visible.
- **The shared A–B edge.** It has real excluded neighbours (13 pairs, author-reported). The outer edges have none by design, so that part of the effect is a stated design limit, not an ecological edge effect.


---

## 8. Limitations and uncertainty

**Verified by reading code at `4cecf873`.** Every equation, constant, refusal, state field and chronology statement in §2–§4 and §6–§7 with a file:line anchor. Also the blob equality of the four linked or cited CCF files with their pins.

**Read from tests, not executed.** Genesis tree counts, exact areas, the content hash and the behaviours asserted by the named tests. I did **not** run the tests. The author's pass claims are in the MA-1 handoff, not verified here.

**Author-reported, not verified.** MA-A and MA-B genesis N/ha, BA, Dq and RD (§2.2; handoff §1 "IMPLEMENTED" item 1), and the 13 cross-boundary pairs.

**Inferred.**
- The common-random-number property (from the roll key).
- That no year cap exists (from the absence of one in the read code paths).
- That the all-empty refusal is unreachable by player action on canonical data. It is reachable in principle by natural hazard mortality (W15).
- The §7.3 hypotheses and the §7.2 runtime confirmations.

**Protocol choices, not evidence.** None of these is a coefficient, measurement or calibration:
- the §2.5 normalisation literal;
- the §6 selection counts (rank 1 for L, ranks 1–3 for M);
- the §6 traversal order (ordinal `AreaId`, ascending Crop `TreeId`, rank layers) and first-occurrence deduplication without replacement (revision 3);
- every §7 tolerance and decision threshold, including the H-W4 strata, the 0 → 10 growth window, the minimum group count of 2, the rank tolerance and the two 20 % thresholds (revision 3).

**Not checked in this revision.** I did not inspect how a route surfaces a `checked` overflow exception (§2.9). I did not re-read the ranking internals of `PlayerCompetitionDiagnostics.Measure`. The §6 rule computes its own competitor ranking from logged positions and DBH instead.

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

**Read again for revision 2 (all at `4cecf873`, read only):**
- `LAB/src/Core/AnnualRules.cs` (whole);
- `CCF/SitkaGrowthModel.cs` l.30-115;
- `LAB/src/Core/MultiArea/MultiAreaContinuation.cs` l.40-70, 180-240, 260-470, 504-545;
- `MultiAreaContracts.cs` l.12-100;
- `LAB/tests/Core.Tests/MultiAreaTests.cs` l.60-80, 160-232, 639, 702;
- the MA-1 author handoff, l.1-60 and l.33-43.

**Also read for revision 2:** the Q4 review `Docs/Handoffs/MultiRack-Simulation-20261116/885-q0-qualification-20261121/Q0-INDEPENDENT-REVIEW.md` on branch `docs/multi-885-q0-crosswalk-review-20261121-v1` (blob `cb78f64975a6915a062ae733c11b36fabc665367`, SHA-256 `1a3f7ed8031ad1889d0b1567e8b9f0ce607aaca0ba09d01e106f81ce5c1e4f1b`, 150 lines), fetched by `gh api`.

**Read again for revision 3 (all at `4cecf873`, read only):**
- `LAB/src/Core/MultiArea/MultiAreaContinuation.cs` l.88-100 (Crop mark refusal), 181-197 (Approve, empty-plan refusal) and 340-385 (Advance preamble, living and carried-forward paths, Crop clearing);
- `MultiAreaContracts.cs` l.56-67;
- `LAB/src/Core/AnnualEngine.cs` l.17 and 23 (`AnnualStand.LivingCount`, `AnnualReport`).

**Also read for revision 3:** the changed-lines recheck `Q0-INDEPENDENT-RECHECK.md` on branch `docs/multi-885-q0-crosswalk-review-20261121-v1` (blob `0dd5bfe862ff1b4235fff1c5528428dfce9dbabf`, SHA-256 `4dc91fb3909326df906924521517408a9f9e44f8566326dbf36c44babd22a77e`, 77 lines), fetched by `gh api`.

**Not read.** The `AdultMortalityDesign`, `HeightCandidateComparison`, `EconomyTutorialImpact`, `SaveVersioningAssessment` and `CanonicalDecisionProposal` files (other than grep hits), the remaining `WindthrowV1/*.md`, and all `Evidence/*.csv`. None was needed for an anchored claim above.
