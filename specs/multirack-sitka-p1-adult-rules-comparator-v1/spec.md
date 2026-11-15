# Sitka P1 — CCF adult-rules experimental comparator (separate from the P0 lab)

- **Feature ID:** `MULTI-P1-ADULT-SITKA-RULES-COMPARATOR-V1`.
- **Status: DRAFT — NOT APPROVED FOR IMPLEMENTATION.** Product owner approved **preparing a DRAFT only** on 15 November 2026. Neither this text nor independent LAB-06 evidence-plan approval is permission to code LAB-07, run Unity, alter P0, integrate the game or claim Irish prediction accuracy.
- **DRAFT author/claim:** OpenAI GPT-6 Overall Manager, docs-only [MULTI-011 #528](https://github.com/jermur788/CCF/issues/528).
- **Revision:** docs-only [MULTI-013 #536](https://github.com/jermur788/CCF/issues/536) repair of the independent [MULTI-012 review](https://github.com/jermur788/CCF/blob/review/multi-012-sitka-p1-four-file-draft-spec-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md) findings R-1…R-10, under the [Manager NEEDS CHANGES ruling](https://github.com/jermur788/CCF/blob/docs/manager-multi-012-p1-draft-needs-changes-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-012-P1-DRAFT-NEEDS-CHANGES-AND-REPAIR-DECISION-20261115.md). Still **DRAFT — NOT APPROVED**. The revision awaits a focused independent delta review.
- **Approval separation (R-8):** the Overall Manager authored this DRAFT, so the Manager may coordinate and issue review verdicts but **cannot make this specification executable by self-approval**. Approval requires (a) an independent spec review, plus a delta review after any repair, **and** (b) an explicit **product-owner** decision. A Manager verdict alone never substitutes for (b).
- **Authority:** [MULTI-010 Manager evidence-plan verdict](https://github.com/jermur788/CCF/blob/docs/manager-multi-010-p1-evidence-plan-verdict-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-010-LAB-06-INDEPENDENT-REVIEW-VERDICT-AND-P1-PRODUCT-GATE-20261115.md); [independent source/science review](https://github.com/jermur788/CCF/blob/review/multi-010-lab06-sitka-p1-forestry-method-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-010-LAB-06-SITKA-P1-INDEPENDENT-FORESTRY-AND-METHOD-REVIEW-20261114.md) with eight MINOR findings m-1…m-8 and seven contract conditions C-1…C-7; [Codex source feasibility study](https://github.com/jermur788/CCF/blob/research/multi-009-sitka-p1-source-evidence-feasibility-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-009-LAB-06-SITKA-P1-SOURCE-AND-EVIDENCE-FEASIBILITY-HANDOFF-20261114.md).
- **Existing product boundary:** [approved synthetic no-growth P0 lab spec](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md), which remains unchanged. [Scenario One milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md) continues to require the complete three-species Sitka/Oak/Beech loop, repeated management and multi-scale management before the first external demo.
- **Implementation reference (inspection only, not frozen for production):** CCF main source snapshot `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; approved P0 lab code and WSL qualification branch `feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1` (P0 lab head `2659db99`). Any implementation would need a **new exact candidate pin and parity baseline** after independent spec approval.
- **P0 lineage (R-9):** the CCF `main` snapshot `fc18190b` does **not** contain `Tools/MultiRackSitkaLab/`; `git ls-tree` lists 0 paths there against 52 at `2659db99`. The qualified P0 lab exists only on its feature/qualification branch lineage. Before any P1-02 source work, a separate decision must fix **P0 integration lineage and the P1 base branch**. This DRAFT assumes neither "P0 is on main" nor any particular merge.
- **Scientific category:** proposed **MODEL-DERIVED / GAMEPLAY-RULES RECONSTRUCTION**, on explicitly synthetic fixtures; **NOT IRISH EMPIRICAL FORECAST**, individual-tree validation, or standard forestry guidance.

## 1. Purpose, scope and non-goals

**Research question:** Can a separately versioned annual experiment reproduce the *existing* CCF adult Sitka rules (DBH increment, local Hegyi competition, relative height, crown relaxation and density-based natural mortality) on a restricted, closed, fully mature-equivalent, pure-Sitka synthetic stand, with deterministic per-tree replay, without modifying the accepted P0 laboratory? What aggregate **B/N/H** behaviour emerges under no-thin and controlled thinning compared with the historical Broad–Lynch 2006 stand reference?

**Approved for DRAFT development only:** develop a testable design, not an executable model. A future approved P1 layer would consume a **read-only, verified** P0 synthetic fixture/snapshot and fork a wholly separate experiment, whose model and data identity are explicit. It may offer year-by-year output and stand metrics, but not silent "better" forestry recommendations.

**Out of scope and expressly not authorised here:** adding biological growth to P0's **Advance experiment step (calendar only)**, modifying existing P0 fixture bytes/hashes/events/saves/schema1/UI behaviour; new growth coefficients or independent biological calibration; juveniles/saplings/young stages, seed/recruitment, light, browsing, storm, site variation, disease, regeneration, planted trees, Oak/Beech, mixed stands, plot slope, rack geometry/five-rack extrapolation, 3D/Unity synchronization, contractor/economy/timber effects, measured Irish marteloscope import, empirical longitudinal forecast validation, production CCF saves, frozen Reference Future or Unity/Windows Player builds. Nothing here modifies Scenario One's existing accepted adult mechanics.

**Honest output label (C-7; proposed binding upon approval):**
> **CCF adult rules — closed pure-Sitka experimental population. Synthetic inputs. No recruitment or storms. NOT an Irish field forecast.**

Every P1 screen, export, chart and report must retain this distinction even when compared with a 2006 empirical stand-model output. P0 continues to say **SYNTHETIC TEST FOREST — NO BIOLOGICAL GROWTH**.

## 2. Evidence and validation ladder

| Evidence class | What is actually available | What it does / does not demonstrate |
|---|---|---|
| **DIRECT SOURCE CHECK** | MULTI-009 mapped current CCF adult Sitka growth/competition/density/event order; independent MULTI-010 checked about 35 file/line citations at source snapshot | Rules can be specified; not executable adapter parity |
| **REPRODUCED ARITHMETIC** | 9 of 14 cases reconstructed off-repo by Codex and recomputed independently by reviewer in binary64 and float32-like arithmetic | Formula extractability, not running Unity, whole-year parity or cross-save determinism |
| **SOURCE-DERIVED** | 3 of 14 cases: cause/provenance, model selection, historical hashes | Not current-runtime reproduction; old hashes are historical-only |
| **UNAVAILABLE** | 2 of 14: exact Unity/adapter state+victim+hash parity, licensed Irish longitudinal held-out tree/stand data | Cannot support a runtime parity claim or field forecast claim |
| **STAND-LEVEL REFERENCE** | [GROWFOR evidence extraction V1B](https://github.com/jermur788/CCF/blob/main/Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md), historic `Tools/Verification/Growfor` comparator | Historical Broad–Lynch 2006 pure even-aged stand **B,N,H** reference; not per-ID release, crowns, mortality cause, mixed or uneven-aged CCF validation |
| **LIMITED PRIMARY SPOT CHECK** | Manager visually checked selected pages of 2006 *Irish Forestry* volume: stand B/N/H, SI30, 113 nonresearch **volume-equation** calibration observations | Does not close all primary-page, later published model, active GROWFOR executable or top-height inventory-definition gaps |

**Stand evidence boundaries:** site index uses *top height at age 30* in Broad–Lynch, not CCF cell productivity; the thinned and unthinned models have different domains and mainly even-aged low-thinned data. The illustrated manual top-height <49.3 m restriction is not a universal joint admissibility box. The **2006b** independent research-panel comparison reused 113 production stands in fitting the **volume equation**; it was not an untouched final-model longitudinal validation holdout. British relative-density transfer and CCF thresholds/crown release remain gameplay-calibrated/transfer assumptions. NFI study marginal age/BA/N/DBH ranges and RMSE are neither joint model-safe limits nor P1 pass thresholds. `#479` has not verified a licensed mapped Irish plot.

P1-04 may compare separate model projections **only after** specifying matched starting B/N/H, age, SI policy, stand membership, thinning chronology and the 2006 reference implementation/version. A GROWFOR discrepancy is **reported**, not silently tuned away. Never describe the P1 experiment as a GROWFOR individual-tree implementation.

## 3. Source-faithful biological and timing contract — proposal, not parity verdict

At the pinned CCF source snapshot:

- Adult DBH for the Sitka asset: potential 1.2 cm/yr, max-scale 120 cm, Ci50 5. The source's function is `ΔD=1.2·site·clamp01(1−D/120)/(1+CI/5)`; the current `site` value is **always 1** because `ForestEcologyCell.SiteProductivity` has initializer 1 and no source writer (m-2). This is **current-game parity**, not a variable empirical site class.
- Hegyi CI: sum living non-stump neighbours within **inclusive 8 m**, stable ordinal ID ordering, with term `(D_neighbour/max(1,D_target))/max(0.5,distance_m)`; source uses a slightly enlarged candidate-query radius but exact 8 m final check. Geometry uses Unity horizontal x/z metres, not P0 x/y labels or elevation. A site/coordinate policy must handle float differences and edges.
- Sitka height in growth model ≥1 uses source Class III `E(age)`, anchored at 20.4 m at age 30, and multiplies individual height by `E(a+1)/E(a)`; it is **not** the Broad–Lynch GROWFOR height/site-index function. The current tree `Height` getter is **stage-dependent** for Sapling/Young; therefore **Mature-equivalent only** is the DRAFT scope (m-1). Max 60 m clamp is game policy, not an Irish data validity limit. Source clamps in `ForestTree.ApplyGrowth` are DBH `[1, 200]` cm and height `[0.1, 60]` m; crown radius has a 0.1 m floor (`RelaxCrownRadius`). The oracle must exercise them.
- Crowns relax 15% toward a target computed from *post-growth* DBH potential `0.9415+0.07635·DBH_cm` and *pre-growth* competition; initial crown size cannot be reconstructed from P0 and must be **labelled synthetic initialisation**. No independent physiological crown-release evidence follows from this rule.
- CCF density uses **all living trees** (including P0 below action threshold), `Dq=sqrt(mean(DBH_cm²))`, `N/ha=numberLiving/standAreaHa`, and `RD=(N/ha)·(Dq/25)^2.063/1868`. The source uses a **square** world `standSizeMeters²`, not the existing non-square P0 demo polygon (m-5). The exponent and 1868 coefficient are labelled British transfer, not validated Irish stand limits.
- Density pressure `P=max(0,(RD−0.6)/0.4)` from **post-growth** RD (post-growth DBH, current living set); suppression `S=1−1/(1+CI/5)` from the **pre-growth** CI computed at the start of the year and **not recomputed** after growth (`ForestEcologyController.cs:475`, `:1088–1092`); conditional hazard `p=min(0.5,0.08·P²·S⁴/mean(S⁴))` when P and mean denominator nonzero. The ecological year, seed, full ordinal tree ID and **RNG model version**, separately from growth model version, determine each roll (m-6). Then survivors are removed by suppression priority to RD≤1 **or at most one living tree**; tiny area/singleton can remain RD>1 and must be tested, not incorrectly "fixed" in a port.
- Source schedule: committed work/felling **before** annual advance affects the **next** annual pre-growth neighbour set. Inside annual advance: increment ecological year → pre-growth competition → adult DBH/height → crown relax → post-growth density/death, with natural death batch followed by accounting. No second intra-year growth for survivors after mortality. Storm, juveniles, regeneration and other current-game systems are deliberately absent; therefore this comparator **is not full game parity**.
- New CCF games select growth model **2**; on a closed all-`Reference`-demand Sitka population, the adult ecological density route uses the same model-1 path. A non-Sitka living tree/non-Reference demand changes the route. Growth model 0 is historical/diagnostic only and must not be labelled the P1 target. Old game saves with missing/old model fields and frozen Reference Future remain outside the lab and unaltered. Source save schema was **24** while the historical Sitka verification harness expected **23** (stale assertion, not a production source defect). Any harness repair requires a separate owned/approved task, never weakening the check. That repair is **optional/conditional** for P1. It gates P1-01 only if the oracle chooses to reuse that historical harness. A new dedicated capture harness does not depend on it (R-6).

**Numerical reference of record (R-1).** Faithful parity is defined against **per-ID values captured from the pinned CCF source running in the Unity Editor (Mono) runtime**, not against a language data type. Evidence:
- **Source observation, not a new Unity run.** The CCF source records that Unity Mono's float results depend on code generation. `ForestEcologyController.cs:990–993`: routing an identical Hegyi expression through a shared function changed the 80-year anchor hash, even with `AggressiveInlining`. `:1021–1023`: legacy Mono anchors "depend on the exact floating-point evaluation". `Tools/Verification/BrowsingProtectionVerification.cs:289–291` and `Docs/ScenarioTwoSpeciesFoundation.md:143` say the same.
- **Probe, not Unity output.** The MULTI-012 reviewer compared two *plausible* float evaluation semantics on the synthetic P0 fixtures. 94/153 (demo) and 981/1,893 (perf) first-year DBH increments differ in binary32 bits. Neither semantics is verified Unity behaviour.
- **Consequence.** "Use binary32" is a **hypothesis about how to approach parity, not a guarantee of it**. The C# type does not fix the runtime's evaluation width or order. A future adapter should preserve the source's mixed float32/double steps *as far as the capture verifies them*.
- **Portable part.** The adult mortality roll (`SimulationRandom.Roll`) is pure integer hashing that returns an exact 24-bit float. It is runtime-portable when model version, ID, year and seed match. The float-sensitive quantities are CI, DBH, height, crown, RD/P/S/p and the comparisons against thresholds.

The 9 formula examples are **entry-point probes**, not enough to qualify a whole-year adapter. Update ordering, clamps, tie-breaks and rounding also need that runtime capture.

## 4. Versioned fork, P0 isolation and separate P1 state (C-1)

**Proposed design for independent review (NOT YET ACCEPTED):**
- A P1 experiment reads a **pre-validated** immutable P0 synthetic fixture/snapshot, retaining `sourceFixtureFileSha256`, P0 snapshot hash, individual stable `treeId`, synthetic `sourceClass`, area/XY/DBH/height/age at inventory and declared limitations. The actual P0 file and its event log remain unchanged; no backward writes or hidden P0 upgrade.
- P1 uses a **distinct root JSON `format` string and version** (candidate `ccf-sitka-adult-p1`, schemaVersion 1 **within its own namespace**) and **a non-P0 file extension or segregated directory** (candidate `.sitkap1.json` under a dedicated ignored `.lab-p1/`), validated/guarded independently. These are **proposed identity values** for C-1 review, not current schema values. The P0 reader already refuses such files: `LabFile.CheckHeader` requires `format="ccf-sitka-lab"` and `labSchemaVersion==1`, and the P0 save listing accepts only `<name>.sitkalab.json`. **Shared-file note (R-9):** ignoring `.lab-p1/` needs an entry in `Tools/MultiRackSitkaLab/.gitignore`, which today ignores only `.lab-data/`. That file is **P0-owned**. Either a future task gets explicit ownership of that shared edit, or P1 chooses an isolated alternative, such as a P1-owned ignore file or a location covered by existing rules. This DRAFT authorises no P0 edit. P0 reader must reject P1, P1 reader must reject P0 except through explicit read-only verified import. No path-picker or CCF production save support.
- Separate `P1ExperimentState`, `P1TreeState`, `P1AnnualEvent`, `P1SnapshotHash` and replay grammar. Reference and P0 hashes must not change. Initial P0 marks/approved plans either undergo an **explicit deterministic transformation** into P1 equivalents or are expressly excluded at initial fork; never silently treat a P0 `ExperimentStepAdvanced` as ecological growth. **OPEN C-1A (owner decision):** whether P1 begins from the initial fixture only or imports a validated P0 post-intervention snapshot, and how work-plan provenance is represented. C-1A also contains an explicit **approval-binding sub-decision (R-2)**:
  - **Option 1:** P1 reuses P0's semantics. An approval binds to a fresh random session epoch (`LabSession.cs:69–70`, `:221–225`). On load, every open APPROVED/PENDING_WORK plan becomes STALE through a recorded `PlanStale(SessionLoaded)` event (`LabSession.cs:75–85`, `:144–149`).
  - **Option 2:** P1 defines a distinct, independently reviewed approval policy.

  P0 itself stays unchanged either way. "Silently persisting approvals across load" is **not** an acceptable fix, so Option 2 must be specified and tested on its own terms.
- File persistence must preserve the Linux x64 / WSL2 security posture verified in MULTI-006/007/008: no free paths, `127.0.0.1` Host/Origin/CSRF, no-follow handles, atomic save, size limits, fail closed on unsupported OS, no writes to game saves. Do **not** assume merely placing files in a separate root automatically reuses those guarantees. Any P1 file writer needs explicit ownership, tests and independent review.
- If a P1 experiment is restarted or a snapshot is loaded, model/RNG versions, source identity, age/year, exact numeric state and ledger must restore without reseeding, duplicated mortality or retroactive event application. Unknown versions reject **closed**, never coerce into P0 or default a missing new model to production.
- Events and hashes are deterministic **under a pinned runtime, algorithm version, numeric policy, fixture hash and event stream**. Do not promise byte identity across independent Unity/.NET runtimes before verified parity.
- **Split-run equivalence (R-2).** Two properties must be kept apart:
  - **Biological and work-outcome equivalence:** the same per-ID biological state (DBH, height, crown, age, living status), ecological year, seed and growth/RNG model versions, mortality/felling ledger (IDs, causes, years, roll inputs and rolls), executed-work outcomes and source provenance, after equivalent legal actions.
  - **Raw canonical-history hash equality:** is required **only when the complete canonical histories are identical**, including event order, payload bytes, sequence, previous/post-state hash references and any hashed session-epoch fields. A P1 biological projection, a whole snapshot hash and a raw audit/event-history hash are **different comparisons**: a recorded load event may leave the biological state unchanged while changing the audit history; a later approval can introduce a new session epoch into hashed state. Validate every raw chain independently when histories differ.
  - **Optional normalized comparison (OPEN C-1/C-1A):** a projected-history or projected-snapshot equality rule is **not defined or approved here**. If later needed, it must be separately named and versioned, specify retained/excluded fields, approval-session treatment, ordering/renumbering, deterministic reserialization and rehashing, and receive independent review before P1-03 acceptance. Never claim that deleting a load event alone creates a comparable canonical hash.

  A load can legitimately append events, for example P0-style `PlanStale(SessionLoaded)`. So save → load → continue is tested in two modes:
  - **Mode A — no open APPROVED/PENDING_WORK plan at the split point.** Require matching per-ID biological state, ecological year, model/RNG/seed, mortality/felling ledger and legally equivalent executed-work outcomes. **Do not require raw canonical-history or complete-snapshot hash equality merely because no plan was open at the split.** A recorded load event changes the history; a new approval **after** the split may embed a different random session epoch even if no load event was recorded. Preserve and independently validate each actual audit/history chain. **Only** in a restricted test where the entire canonical event history and all hashed session-dependent payloads are identical may the corresponding raw history hashes be required to match. Any normalized/projected hash comparison remains the separate OPEN C-1/C-1A decision described above.
  - **Mode B — one or more open APPROVED/PENDING_WORK plans at the split point.** If C-1A Option 1 is chosen, require **exactly one recorded `PlanStale(SessionLoaded)` per affected plan** at load, compare biological projections, and compare the plan lifecycle by named event semantics. In the uninterrupted run the same plan stales at the next annual step instead, so the two raw chain hashes are **expected to differ** and must not be declared equal. If Option 2 is chosen, its own approved test oracle applies.

## 5. Annual P1 state, provenance and metrics (C-2…C-5)

A future independently reviewed P1 schema must represent:
- **Experiment:** synthetic fixture file hash and P0 snapshot hash, CCF source reference, P1 algorithm/parameter version, declared growth and RNG versions **separately**, seed, ecological year and age anchor, declared fixed stage/site/area convention, geometry/origin/rounding policy, no-recruitment/no-storm exclusions, input provenance and source classification.
- **Per stable ID:** species/demand/stage and true living status; P0 initial pre-inventory-dead flag (P0 dead has **no cause/year**); before/after **authoritative numerical** DBH, height, crown radius, age, spatial XY and exact eligibility; pre-growth CI and suppression, applied increments, p/roll, natural mortality cause/year or separate work felling event/plan provenance, no growing/death repeat for dead/felled. Pre-inventory dead is excluded from living and must **not** become a newly simulated mortality event. **Proposed** synthetic cause label `pre-inventory` with a distinct no-year sentinel, pending C-4 approval.
- **Per stand/year:** all-living N and N/ha; all-living BA and BA/ha, Dq; independently reported P0 **action-eligible** adult subset; initial/post-felling/pre-growth/post-growth/post-mortality counts and BA; defensible top-height definition/H, data-domain flags, natural-death vs work-fell IDs and BA, growth additions, total change reconciliation.
- **Per event:** `P1ExperimentCreated`, explicit work-plan transitions, `P1AnnualAdvanced` with pre/post state hashes and per-ID accounting, cause-specific death, plan staleness/cancellation with origin, and replay checks. These are candidate event names only, not an approved schema.

**Numeric OPEN C-2, split into two timed parts (R-1):**
- **C-2a — decide BEFORE the P1-01 capture.** Covers:
  - input units and the exact source inputs (cm, m, centred x/z metres from P0 mm);
  - the oracle capture format, which must **record raw float bit patterns** (hex) alongside decimal values for every captured float/double intermediate;
  - identity fields in every record: stable ID, ecological year, growth model, RNG model, seed, source commit and Unity Editor version;
  - the P1 serialisation representation (bit pattern or shortest exact round-trip decimal), IEEE special-value handling, and display/export rounding to cm/m/mm.

  Never round authoritative yearly state to P0 integer mm: those millimetres are display/export conversions only.
- **C-2b — decide only AFTER the Unity capture exists.** An independently reviewed parity target:
  - **exact bit equality** where the evidence shows it is actually achievable; otherwise
  - **reviewed tolerances with explicit branch margins**, under which death sets, boundary-victim order, plan/selection IDs, 8 m inclusion, RD 0.6/1 branches and `roll < p` outcomes must remain **identical**, plus a reproducible event policy.

  C-2b **cannot** be deemed resolved before P1-01. "Source-faithful binary32" is the *starting hypothesis* to test against the capture, not a pre-decided answer. A binary64 adapter is likewise acceptable only under a C-2b tolerance and branch-margin policy.

**Population OPEN C-3:** **all living** stems, including roughly 10% below the P0 action threshold, are recommended in **CCF-like CI/RD/growth and natural mortality**. P0 action eligibility remains a separate label; no silent filtering of seedlings/young is allowed under the Mature-only restriction—every included live tree must be explicitly treated as a **synthetic Mature-equivalent**, which is not an empirical age-class conclusion. Alternatives require a prominently labelled non-parity mode or rejection. Dead/felled excluded.

**Initial conditions OPEN C-4:** all living source stems assumed **Mature-equivalent**, Sitka species + Reference growing-space demand, no pruning, site productivity 1 (current source), synthetic crown initialisation from DBH potential (or independently specified initial crown), fixed explicit ecological age/seed/RNG version, an explicit starting `ecologicalYear` mapping (independent of P0 `inventoryYear` 2026, see §6 item 2), and closed recruitment. Pre-inventory P0 dead map to a documented non-event state. No field inference about cause or age.

**Area and top-height OPEN C-5:** use **square, source-equivalent** parity fixtures (source centred origin, standSize² hectares). Polygon area and arbitrary P0 pentagons are separately labelled **adapter extension**, not bit-equivalent CCF geometry. The existing P0 `oracle-quarter-ha` fixture is already a 50 m square (0.25 ha, 3 trees), but in **corner** coordinates `(0,0)…(50 000, 50 000)` mm. Re-centred on the origin, it is a candidate first source-parity fixture. The re-centring rule is part of C-5 and C-2a, because translation changes float distances near the 8 m edge. The CCF Class III E(30)=20.4 m rule is independent of GROWFOR's SI30. Proposed diagnostic H is the mean height of a declared top 100 **largest DBH stems per hectare** with deterministic tie/order/rounding; its equivalence to the original published top-height field method remains **NOT VERIFIED**. Explicitly flag empty/tiny-area samples. Synthetic P0 fixture heights imply a different initial SI envelope; derive/report inferred SI rather than force-match. No automatic correction of height/age or hidden coefficients.

**Measured fixture caveat m-8:** independently computed P0 demo has 153 living, 9 pre-inventory dead, 15 living below P0 action threshold; `sitka-flat-perf-2000-v1` 1893 living, 107 dead, 182 below threshold. Under CCF all-living density with polygon area, demo is about **RD 0.1698**, perf about **RD 0.6085**, versus adult-threshold-only **0.1689/0.6057**; these are **reviewer arithmetic on synthetic fixtures**, not source runtime results. The performance stand is near the 0.6 onset and must have separate carefully controlled high-/low-density test conditions. The demo will not exercise density mortality at its starting state.

## 6. C-6 source-parity oracle, phased qualification and stop gates

**No P1 source task can start until the following are positively released in a later APPROVED spec/implementation packet.**
1. Freeze the exact current CCF reference, the game model/seed/version policy, all relevant parameter files and the verified P0 fixture source. The source-harness mismatch (schema 24 vs the stale `version==23` assertion) is a **separate, optional/conditional** source-verification repair with its own owner and independent review (R-6). It blocks P1-01 **only if** the oracle reuses that historical harness. Tests may never be weakened or silently skipped, and this DRAFT edits no harness.
2. Approve the **P1-01** parity capture (R-1):
   - **Reference of record:** a **bounded capture from the pinned CCF source running in the Unity Editor (Mono)**, recording exact inputs and per-ID annual CI, DBH, height, crown, N/RD, P, S, p, roll, victim/cause/sequence and hashes, with raw float bits per C-2a, under explicit growth-model-2, pure-Sitka, Reference-demand conditions.
   - **.NET is not a substitute.** A pure CoreCLR/.NET harness, even one binding the real CCF assemblies, runs under different float code generation. It may be used only to accelerate re-runs or implement the adapter, and only **after it reproduces the same Unity capture vectors**.
   - **Owner-level scheduling gate:** the capture needs a short, **exclusive** Unity Editor lane and a physical CCF-Ops Unity lock. That shares finite machine and owner time with the primary **three-species Scenario One** milestone, so it needs a separate owner scheduling decision. This DRAFT books no machine, reserves no lock and authorises no Unity run. The owner may defer P1 and keep P0 intact. No integrated build or Windows Player build is implied. Without a capture, the faithful-parity claim stays blocked.
   - **Oracle configuration contract (R-7), to be fixed in the approved P1-01 packet:**
     - Choose a valid regeneration route. Regeneration model 2, the new-game default, throws `InvalidOperationException` without a `ScenarioOneManager` (`ForestEcologyController.cs:483–486`). Either provide that manager or select a regeneration model that does not require it, and record which.
     - Prevent recruitment. `PromoteCohorts` does nothing without a `ForestTreeSpawner` (`:1699–1703`). Otherwise promoted recruits at DBH 2–20 cm join RD and CI in later years.
     - Storm model 0. `ResolveStormForYear` runs before competition (`:472`).
     - Pin the RNG model and seed.
     - Map the controller's `ecologicalYear` explicitly. It increments **before** competition and the mortality roll (`:467`, roll key `:1103`). It is independent of P0's calendar `inventoryYear` (2026), which is **not** an ecological year.
     - All trees Mature, Sitka asset, Reference demand, no pruning or age phases.
     - Run a scoped **configuration A/B** (for example, one year with two admissible non-adult configurations) showing the configuration does not change adult outputs. Production code is not modified to achieve this.
3. Require adversarial vectors for 8 m inclusive edge, coincident trees, stable ID ordering, low/high competition, small/huge age, suppression, 0.6/1 density boundaries, singleton tiny area where RD may remain >1, pre-inventory dead, below-threshold Mature-equivalent, declared SI, no-thin, one-/repeated-thin, partial/out-of-plan execution and mortality-vs-felling. Record **source-derived**, **runtime captured**, **adapter reproduced** separately.
4. **P1-02** independent P1 adult adapter only after the verified Unity oracle **and** a reviewed C-2b parity target, with exact or independently justified numerical tolerances including threshold flips. **P1-03** distinct deterministic annual ledger/persistence/save/load and plan invalidation/replay; **P1-04** stand-level historical 2006 B/N/H comparator (can run alongside later P1-03 after P1-02 outputs); **P1-05** minimal clearly-labelled P1 viewer/export and P0 regression/security/browser gates. None is authorised merely by this DRAFT.
5. Safety/behaviour gates: P0 same fixture hashes and snap/replay before/after P1 integration, malformed and CCF-shaped paths refused, unknown P1 versions refused, only safe `.lab-p1` root, 127.0.0.1-only host, no external data/telemetry, idempotent death/fell distinction, canonical P1 event chain, post-load continuation biologically/work-outcome equivalent to an uninterrupted run under pinned source (Mode A/Mode B in §4; raw hash equality only for identical event histories), no false 200/403 save response regression, no assumptions about fd-based root persistence beyond current threat model.
6. Empirical test: compare stand trajectories only with eligible **pure even-aged** Broad–Lynch 2006 reference conditions, report differences with source/units, domain flags, SI definition and derivation, no field-accuracy claim. Requalify rather than duplicate `Tools/Verification/Growfor`; current GROWFOR executable identity, original source range and top-height field definition still need verification for stronger scientific labels.

**Automatic BLOCK / NO-GO** if the owner instead requires a predictive Irish individual-tree model without licensed repeated-measure spatial field data; if parity cannot be reliably reproduced; if source model switching depends on mixed demand; if P0 byte/hash/history safety is compromised; if unknown/unsupported platform I/O falls back to weak path-based writes; if scope expands to player workflow/Unity/racks/mixed species without a new approved spec; or if evidence is presented as field validation without held-out observations.

### Reviewer finding crosswalk — complete m-1…m-8

These independent findings have **not** been independently resolved by this DRAFT; they define what a spec reviewer must check. **Status meanings (R-10):**
- **DRAFT** means the DRAFT text carries a concrete disposition that needs no further owner/spec choice, only approval of the spec as a whole.
- **OPEN** means the disposition depends on an unresolved C-decision. The linked C-item says which.

MULTI-012 judged m-1, m-2, m-3, m-5, m-6, m-7 and m-8 adequately addressed, and m-4 partially addressed (superseded by R-1).

| Finding | Explicit design coverage | Status |
|---|---|---|
| **m-1** Stage-sensitive height | Mature-equivalent only; refuse Sapling/Young, §3 and §5 | **DRAFT** |
| **m-2** Site productivity is always 1 in current source | Treat as source parity, not a site-class claim, §3/§5 | **DRAFT** |
| **m-3** Pre-existing dead P0 trees lack cause/year | Synthetic pre-inventory mapping, no new death event, §5 | **OPEN** |
| **m-4** Mixed float32/double source and float saves | Superseded in part by R-1: Unity Editor capture is the reference of record; C-2a capture/serialisation before P1-01, C-2b parity target after it; binary32 is a hypothesis only, §3/§5–6 | **OPEN** (C-2a/C-2b) |
| **m-5** Square centred CCF stand vs polygon P0 | Square source-parity fixture before polygon extension, §3/§5 | **OPEN** |
| **m-6** RNG version determines death roll independently of growth | Record separate RNG + growth versions and test both, §3/§5 | **OPEN** |
| **m-7** 113 observations used in **volume-equation** calibration | Explicit model and PDF vs print-page attribution, §2 | **DRAFT** |
| **m-8** Below-threshold population and near-onset density fixture | Separate all-living/P0 eligible metrics and fixture policies, §5–6 | **OPEN** |

## 7. DRAFT decision register (C-1…C-7) — NOT YET ADOPTED

| ID | Reviewer-required decision | DRAFT proposal for independent scrutiny | Status |
|---|---|---|---|
| **C-1** | Separate P1 file identity, immutable P0 fork and plan origin | `ccf-sitka-adult-p1`, schema 1 in P1 namespace, distinct extension/root (shared P0 `.gitignore` ownership or isolated alternative), P0 fixture hash + snapshot. **C-1A (owner):** fork from base fixture vs post-intervention snapshot, and **approval binding** (reuse P0 session-epoch + `PlanStale(SessionLoaded)` vs a distinct reviewed P1 policy) | **OPEN** |
| **C-2a** | Pre-capture numeric inputs, capture format and serialisation | Units/exact inputs, raw float-bit recording in the oracle format, ID/year/model/RNG/seed/source/Editor-version fields, P1 serialisation and display rounding | **OPEN** |
| **C-2b** | Post-capture parity target | Exact bits where the Unity capture shows them achievable, else reviewed tolerances with branch margins and identical death/plan/selection IDs; binary32 is a hypothesis to test, not a guarantee | **OPEN — cannot close before P1-01** |
| **C-3** | All-living CCF growth/CI/RD vs P0 action threshold | Include **all living Mature-equivalent** stems, show actionable subsets separately; explicitly reject non-Mature and non-Sitka | **OPEN** |
| **C-4** | Crown/site/stage/dead/RNG initial state | Mature-only; site=1 source fact; crown explicit synthetic initialization; pre-inventory dead sentinel; separate seed+RNG/growth models | **OPEN** |
| **C-5** | Square vs polygon parity, coordinates, synthetic SI and H convention | Square centred source-parity fixtures first; polygon separate labelled extension; explicit SI30 and unverified H definition | **OPEN** |
| **C-6** | Runtime oracle route, Unity scheduling and stale verifier / phased ownership | Bounded **Unity Editor capture as reference of record** before the adapter (.NET only after cross-checking the same vectors); owner schedules the exclusive Unity lane; oracle configuration contract (§6 item 2); stale `version==23` harness repair separate and optional unless reused; single writer per stage; no Unity lease presumed | **OPEN** |
| **C-7** | Ethical/scientific UI and export labels | Always "CCF adult rules — closed pure Sitka; NOT an Irish field forecast" and model-to-model reference label | **OPEN** |

### 7.1 When each decision is needed, and who owns it (R-5)

All rows remain **OPEN**. The table fixes *timing and ownership only*; it adopts no default.

| Decision | Must be decided before | Owner level | Notes |
|---|---|---|---|
| **C-1A** fork source + approval binding | Scheduling P1-01 if post-intervention snapshots are in scope; otherwise before P1-03 | **Product owner** | Decides what a P1 experiment means. P0 unchanged either way. |
| **C-1** format/extension/root | P1-03 (persistence) | Technical spec; shared-`.gitignore` ownership is a coordination decision | P0 already refuses non-`ccf-sitka-lab` files |
| **C-2a** capture/serialisation | P1-01 capture | Technical spec | Raw float bits in the capture format |
| **C-2b** parity target | P1-02 adapter acceptance; **only after** the P1-01 Unity capture | Technical spec + independent numeric review | Not a P1-01 prerequisite, so not circular |
| **C-3** population | P1-01 fixtures | Technical spec (all-living is the source rule); **owner** only if a non-parity actionable-only mode is wanted | Material near RD 0.6 (m-8) |
| **C-4** initial state | P1-01 fixtures | Technical spec | Includes the ecological-year/seed mapping (§6 item 2) |
| **C-5** geometry/coordinates | P1-01 fixtures | Technical spec | Square, centred; re-centring rule |
| **C-5** SI/top-height convention | P1-04 B/N/H comparison | Technical spec + science review | Top-height rule stays NOT VERIFIED |
| **C-6** oracle route + Unity lane | Scheduling P1-01 | **Product owner** (scarce Unity time vs Scenario One) + Manager coordination | No lane booked by this DRAFT |
| **C-7** labels | Any user-visible P1 output (P1-04 report, P1-05 UI/export) | **Product owner** approves the wording | Consistent with P0 A8 |

**Independent review of this four-file DRAFT is mandatory.** A reviewer judges which options are scientifically defensible and technically testable, and may recommend changes. This spec becomes APPROVED only when **both** of these hold (R-8):
- an independent spec review, plus a focused delta review after any repair, supports it; **and**
- the **product owner** explicitly approves it.

The Overall Manager authored this DRAFT. It may coordinate and record verdicts, but it cannot approve its own draft. Even after approval, implementation requires new claimed/owned work packets and safe local tests. The owner's 15 November approval is **DRAFT AUTHORING ONLY**, not C-1…C-7 resolution.

## 8. DRAFT acceptance criteria to be made executable only after approval

- **AC-P1-01:** P0 fixtures/schema1/saves/history/labels and closed-loop API remain byte-/hash-/behaviour-identical under the qualified P0 candidate; P1 is additive, opt-in and distinctly versioned.
- **AC-P1-02:** A current-CCF pure-Sitka adult oracle exists, **captured from the pinned source running in the Unity Editor (Mono)** under the approved configuration contract. It has a reproducible per-ID numeric/event/ordering trace with raw float bits (C-2a). Neither Python formulas nor a .NET harness that has not been cross-checked against those same Unity vectors can satisfy this.
- **AC-P1-03:** P1 annual per-tree DBH/height/crown/CI/density/hazard/roll/death matches the accepted Unity oracle under the **C-2b** parity target, which is decided after capture: exact bits where achievable, else reviewed tolerances with branch margins and identical death/boundary-victim/plan IDs and `roll < p` outcomes. Old growth0 is not silently migrated.
- **AC-P1-04:** At least no-cut and intervention-first schedules record time-ordered cause-specific death/felling, exact stand N/BA reconciliations, and one-time effect with no advance on propose/approve/queue.
- **AC-P1-05:** Save → load → continue from an approved P1 state is **biologically and work-outcome equivalent** to an uninterrupted run: the same next-year per-ID values, death/fell IDs, causes, years and roll inputs. The two event modes are tested separately (§4):
  - **Mode A** (no open APPROVED/PENDING_WORK plan at the split): biological and legally equivalent executed-work outcomes must match. Independently validate both raw histories; **demand identical raw history hashes only for identical complete canonical events and hashed session data**, not when a load event is added or an approval made later uses a new session epoch. No projected equality is defined until C-1/C-1A approves a versioned projection. Negative cases include a load audit event and a later approval after the split.
  - **Mode B** (open plans): under C-1A Option 1, exactly one `PlanStale(SessionLoaded)` per affected plan, with the lifecycle compared by named event semantics and **no claim that raw chain hashes match**. Under Option 2, that policy's own oracle applies.

  Unknown versions and foreign files fail closed.
- **AC-P1-06:** Below-action-threshold live Mature-equivalent, pre-inventory dead, 0.6/1 RD, singleton/tiny area, site/age, 8 m edge, neighbour release, RNG model0/1, source model1/2 routes and title/domain boundaries are tested; no automatic subset dropping.
- **AC-P1-07:** Stand B/N/H comparison is **model-to-model**, with explicit domain/age/thinning/SI/source version and no false Irish forecast or top-height definition claim.
- **AC-P1-08:** Linux x64 and qualified Windows/WSL2 lab-only file and loopback security unchanged, no production game saves, P1 files cannot be loaded as P0 and vice versa.
- **AC-P1-09:** Full source provenance and all m-1…m-8/C-1…C-7 dispositions, audit-only failure events and clear "UNSUPPORTED/NOT IMPLEMENTED" where appropriate.
- **AC-P1-10:** Bounded independent numeric review, model-science review and implementation qualification issue **APPROVED / NEEDS CHANGES / REJECTED** separately. Neither a green unit test nor a model-to-model agreement is Irish empirical validation.

**Status:** All criteria above are **PROPOSED, NOT TESTED, NOT ACCEPTED**. This draft itself authorises no production edit, Unity session or execution.

**Gate naming used across the four files (R-4):**

| tasks.md | plan.md | Meaning |
|---|---|---|
| — | G0 | Evidence plan, **COMPLETE** |
| P1-S0 | G1 | Independent DRAFT review, plus delta review after repairs |
| P1-G2 | G2 | Spec approval: independent review + product owner |
| P1-01 … P1-05 | G3 … G7 | Each **NOT AUTHORISED** |
