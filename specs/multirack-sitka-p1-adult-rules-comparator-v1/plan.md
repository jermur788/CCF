# P1 adult Sitka comparator — DRAFT plan and qualification gates

**Status: DRAFT / NOT APPROVED / NO IMPLEMENTATION.** Companion [spec](spec.md), [task breakdown](tasks.md) and [requirements](checklists/requirements.md). Owner approved writing the four-file DRAFT **only** on 15 November 2026. Revised docs-only under MULTI-013 [#536](https://github.com/jermur788/CCF/issues/536) for independent review findings R-1…R-10; still **DRAFT / NOT APPROVED**, pending a focused independent delta review. Cross-system persistence and annual biology require a separately approved specification **before** any LAB-07 implementation or Unity source operation.

## 1. Work classification and authority

**SPEC REQUIRED**, not a quick repair packet. The intended P1 work would add a new ecological time/state and persistence contract to an otherwise qualified, non-biological research laboratory. The Plan is deliberately split into an evidence oracle, adult source adapter, separate replay/save, stand reference comparison and minimal user-facing output. Every prospective activity below is **a proposal for later authorisation**, not a runnable assignment today.

Authority chain: direct owner DRAFT-only choice → [MULTI-009 source/evidence LAB-06](https://github.com/jermur788/CCF/blob/research/multi-009-sitka-p1-source-evidence-feasibility-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-009-LAB-06-SITKA-P1-SOURCE-AND-EVIDENCE-FEASIBILITY-HANDOFF-20261114.md) → [MULTI-010 independent forestry/method review](https://github.com/jermur788/CCF/blob/review/multi-010-lab06-sitka-p1-forestry-method-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-010-LAB-06-SITKA-P1-INDEPENDENT-FORESTRY-AND-METHOD-REVIEW-20261114.md) → [Manager APPROVED AS P1 EVIDENCE PLAN](https://github.com/jermur788/CCF/blob/docs/manager-multi-010-p1-evidence-plan-verdict-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-010-LAB-06-INDEPENDENT-REVIEW-VERDICT-AND-P1-PRODUCT-GATE-20261115.md). **None** is an approved P1 code branch or game integration authority.

The current source pinned in LAB-06 is a reference for spec drafting only. Future work must pin the **then-current inspected CCF source**, its actual source-only/game integration lineage and the separately qualified lab branch. Main or a prior report alone does not prove current Unity integration or local clean state.

## 2. Proposed architecture and file ownership

```text
Immutable P0 synthetic fixture + validated P0 hash
             |  read-only, explicit import/fork
             v
      [separate P1 input manifest]
             |
     source-faithful candidate adapter         Unity Editor (Mono) capture = reference of record
     (per-ID adult CCF rules only)        <----  (pinned current CCF source; .NET re-run only after
             |                                    reproducing the same capture vectors)
             |
  P1 annual transaction: after committed work -> CI -> DBH/height -> crown -> mortality
             |                                    |
             v                                    v
  new P1 tree/snapshot & event ledger      all-living + actionable subset metrics
             |
     distinct P1 format/save/replayer      BroadLynch2006_Sitka stand B/N/H comparison
             |
     explicit opt-in P1 UI/export
```

This is not a second copy of the Unity forest and not a new source of truth for the production game. Proposed new P1 files would live under an independently allocated sub-namespace of `Tools/MultiRackSitkaLab/` **after** approval. **P0 lineage caveat (R-9):** that directory is **not on `main`** (`fc18190b` has 0 paths there). The qualified P0 lab exists only on `feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1` (`2659db99`) and its lineage. P0 integration lineage and the P1 base branch must be decided before P1-02. `Tools/MultiRackSitkaLab/.gitignore` is a **P0-owned shared file**: any `.lab-p1/` entry needs explicit ownership, or P1 must use an isolated alternative. Existing `src/Core/Engine.cs`, schema1 P0 `LabFile`, P0 events/saves, approved lab fixture hashes, Host security and API must remain stable. Reviewer must inspect whether a separate binary/Host route and separate P1 file namespace are needed to avoid any P0 behaviour change; the specification makes no code-level integration decision by itself.

**Proposed single-writer ownership after approval (not a lock now):**
- P1-01 oracle source capture: **one current-Unity/source-verification owner** on a fresh local lane.
  - **A bounded Unity Editor (Mono) capture is required** as the reference of record (R-1). It runs under the Scenario One integrator/Unity machine gate, after a separate **owner scheduling decision** and a physical CCF-Ops Unity lock.
  - A pure-.NET binding may only accelerate re-runs **after reproducing the same Unity capture vectors**. It never replaces the capture.
  - The stale `version==23` assertion is a separate, optional/conditional repair with its own owner. It matters only if the capture reuses that harness, and must never be weakened (R-6).
- P1-02/P1-03 P1 Core, state, format, file safety and tests: **one primary P1 lab source writer** across overlapping P1 C# files; avoid a simultaneous second C# writer. Do not borrow the P0 R-1/2 repair worktree or user Windows game files.
- P1-04 scientific comparator: **one independent evidence/numeric owner** on existing `Tools/Verification/Growfor` via read-only scratch output until a separate approved writer lane is necessary; preserve the existing versioned 2006 implementation, do not rewrite it by default.
- P1-05 UI/presentation: only after stable DTO/API/data contract, one UI owner can work in a separate non-overlapping file set, with P0 screens and Host routing protected. End-to-end qualification requires actual Windows/WSL browser and source lineage pin.
- Reviews: a source-independent reviewer after each substantive code handoff, plus proportionate aggregate/science and file-security review; evidence and approval claims must stay separated.

**Machine safety:** Live `Docs/Project/coordination/active-tasks.md` remains sole active ownership authority; workers refresh it, check host/branch/worktrees/Unity leases and mark actual task issue [RUNNING] before any source edit/test. No worktree/lock created by this DRAFT. Do not kill, clean, stash or overwrite other lanes. Heavy Unity qualification requires an exclusive per-machine phase lock. No Windows Unity Player/game build before the user's personal Scenario One playtest and specific approval.

## 3. Explicit dependency and release sequence

Gate crosswalk (R-4), the same in all four files:

| tasks.md | plan.md |
|---|---|
| — | G0 (evidence, complete) |
| **P1-S0** | **G1** |
| **P1-G2** | **G2** |
| **P1-01 … P1-05** | **G3 … G7** |

| Gate | Proposed owner and difficulty | Inputs | Exit evidence | State today |
|---|---|---|---|---|
| **G0 Evidence plan** (no task row) | Codex research + independent Opus review; HIGH | current CCF code, GROWFOR extraction, P0 lab | 9 formula recomputations independently checked, source methods traced, 8 minor corrections | **COMPLETE: approved as evidence-plan quality ONLY** |
| **G1 = P1-S0 Four-file DRAFT + independent spec review** | docs-only GPT-6 author + independent non-author critical spec reviewer; MULTI-013 docs repair + focused independent delta review; HIGH | owner DRAFT-only approval, eight minor and seven open decisions, MULTI-012 R-1…R-10 | no-implementation four-file DRAFT, complete cross-file review and diff, findings disposition, delta review of repairs | **IN PROGRESS / NOT APPROVED** (MULTI-012 NEEDS CHANGES; MULTI-013 repair awaiting delta review) |
| **G2 = P1-G2 P1 product+spec approval** | **product owner** decision, supported by independent review; Overall Manager coordinates but, as DRAFT author, cannot self-approve (R-8) | independent spec + delta verdicts; owner-level C-1A/C-6/C-7 choices; technical C-items resolved or timed per spec §7.1 | separate accepted decisions / owner approval, exact APPROVED status and implementation packet release | **BLOCKED UNTIL APPROVAL** |
| **G3 = P1-01 Unity runtime oracle** | bounded source/Unity verification owner, HIGH | G2; **owner scheduling decision + physical exclusive Unity lock**; source HEAD; C-1A (if snapshots), C-2a, C-3, C-4, C-5 geometry, C-6; approved oracle configuration contract | per-ID annual **Unity Editor (Mono)** capture with raw float bits, DBH/H/CI/crown/RD/p/roll/victims, configuration A/B showing adult-output neutrality, no source/game mutation | **NOT AUTHORISED** |
| **G4 = P1-02 adapter** | P1 C# lab sole writer, HIGH | G3 accepted Unity vectors; **C-2b** parity target reviewed after G3; C-3…C-5 contract; decided P0 lineage/P1 base branch | independent numeric per-ID parity under C-2b, stable source+parameter manifest; no P0 mutation | **NOT AUTHORISED** |
| **G5 = P1-03 event/persistence** | same P1 Core writer, HIGH | G4, reviewed P1 schema/version (C-1, C-1A approval binding) and P0 preservation proof | deterministic one-year and repeated-year replay, save/reload biological equivalence in Mode A/Mode B (§5), refusal/security and history provenance | **NOT AUTHORISED** |
| **G6 = P1-04 stand comparison** | forestry/evidence lead separate read-only lane, HIGH | G4 deterministic outputs; GROWFOR comparator requalified; C-5 SI/H convention; owner-approved C-7 labels | matched pure even-aged B/N/H time series with source/age/SI/thinning/domain caveats; no field forecast claims | **NOT AUTHORISED** (may overlap G5 once G4 stable) |
| **G7 = P1-05 minimal user interface + qualification** | non-overlapping UI owner and independent reviewer, MEDIUM–HIGH | G5, G6 scientific labels, defined API | clearly labelled per-ID changes/BA/N/H history; P0 exact regressions; Windows Edge/WSL end-to-end and valid source evidence | **NOT AUTHORISED** |
| **G8 Any game integration** | Scenario One manager, new approved multi-system spec and full qualification | separate owner decision and approved game feature | production parity, saves/Reference, integration, Unity, render and user acceptance | **OUT OF SCOPE** |

**Stop at any failed gate.** A research model can be left isolated at P0/P1 prototype status; passing G4 does not justify G5/7 automatically if P0 integrity or source semantics fail. G6 numerical match is *model-to-model plausibility*, not Irish ground truth.

## 4. Decisions and assumptions, with timing

The [spec decision register](spec.md#7-draft-decision-register-c-1c-7--not-yet-adopted) deliberately lists C-1…C-7 as **OPEN**. Spec §7.1 fixes **when** each is needed and **who** owns it (R-5).
- Most items must be settled before P1-01 (G3).
- **C-2b** is decided only after the P1-01 Unity capture, before P1-02 acceptance.
- **C-5 SI/H** is needed before the P1-04 comparison.
- **C-1 format** is needed before P1-03.
- **C-7** is needed before any user-visible output.
- **C-1A** and **C-6** are **product-owner** decisions. C-6 reserves scarce Unity time against Scenario One.

The independent reviewer and owner/Manager must address:

1. **C-1 separate P1 identity:** source P0 fixture hash and exact P0 snapshot, no write to P0, distinct P1 format/root, import/fork plan history policy. No P0 field migration disguised as schema1 extension. **C-1A (owner)** also decides the approval binding: reuse P0's session epoch with `PlanStale(SessionLoaded)` on load, or a distinct reviewed P1 policy (R-2).
2. **C-2a / C-2b numeric contract (R-1):**
   - **C-2a, before capture:** units/inputs, raw float-bit recording, identity fields and serialisation/display policy.
   - **C-2b, after the Unity capture:** exact bits where shown achievable, else reviewed tolerances with branch margins.

   A 1e-8 drift can switch `roll < p`, a tie or an 8 m boundary, so "approximately same" cannot guarantee annual event parity. C-2b must hold victim sets, boundary order, plan IDs and branch outcomes identical. "Binary32" is a hypothesis to test, not a guarantee, because Unity Mono float results are code-generation-sensitive (spec §3).
3. **C-3 population:** all living CCF stems in CI/RD including below-threshold (review found ~10% P0 live below threshold), actionable subset as independent metrics. Mature-equivalent for ALL included live stems must be clearly declared, not passed off as real measured age class.
4. **C-4 input assumptions:** no P0 cause/year for Dead; treat as pre-inventory-dead with an explicit non-event provenance, stage Mature, crown init rule labelled synthetic, site=1 current code fact, growth/RNG version and seed/year identity.
5. **C-5 area/site/top-height:** source square centred area and coordinates for hard parity; polygon and site-index comparisons are separate declared changes. Broad–Lynch top-height field definition unresolved; no fabricated exact equivalence.
6. **C-6 oracle capture route and ownership:** a bounded **Unity Editor capture** is the reference of record (.NET only as a cross-checked re-run); the **owner** schedules the exclusive Unity lane; the oracle configuration contract (spec §6 item 2: regeneration route, no spawner recruitment, storm 0, `ecologicalYear`/seed/RNG mapping, configuration A/B) is fixed in the approved P1-01 packet; the stale v23 assertion repair is separate and optional unless that harness is reused; no unsafe Unity machine contention.
7. **C-7 visible scientific labels:** synthetic rules not field forecasts, domains and unavailable regimes visible throughout UI/export/figures.

**Eight reviewer minor corrections m-1…m-8** must be reflected in `spec.md` §3–7 and the requirements table. An independent spec reviewer must check their dispositions, not merely count the identifiers.

## 5. Numeric, biological and event verification design

**Independent scalar baseline:** Prior Codex V01–V09 plus Opus binary32/64 recomputation are useful positive controls, not an oracle. The MULTI-012 two-semantics probe (94/153 and 981/1,893 first-year DBH increments differing in bits) shows that float evaluation semantics matter. It is not Unity output. Each future **Unity Editor** capture must report:
- actual CCF inputs;
- growth and RNG model versions, and seed;
- source commit, Unity Editor version and fixture hashes;
- per-ID values with **raw float bits** for float and double intermediates (C-2a);
- ecological age/year;
- the configuration used, with its adult-neutrality A/B result.

The integer mortality roll is runtime-portable. CI/DBH/height/crown/RD/p are float-sensitive. V10–12 are source-derived/historical only; V13–V14 remain unavailable.

**Future oracle cases, at minimum:** low/high CI; 8 m inclusive/exclusive and same-location 0.5 m distance floor; stable ordinal order with ties and neighbour list permutations; age-phase refusal and normal Mature height; 20→21 site curve; all-living vs below-eligible threshold; .6 onset and >1 density reduction; extreme one-stem/tiny-area; no-thin/one-thin/repeated-thin including felling timing; short/long ages; pre-inventory dead; natural-death-vs-fell; RNG model0 vs 1; source growth1 vs pure-Sitka Reference-demand growth2; source coordinate-centre and area; DBH `[1,200]` cm / height `[0.1,60]` m / crown 0.1 m floor clamps; mortality suppression from the **pre-growth** CI against **post-growth** RD; oracle configuration A/B (regeneration route, no spawner recruitment, storm 0, `ecologicalYear` mapping) showing adult-output neutrality (R-7).

**Runtime comparison criteria (DRAFT proposal):**
- Exact event **topology and IDs**: eligibility, affected tree count, plan status, death vs fell, victim selection, cause and year, deterministic roll inputs and output for a pinned source policy; no substitutions when numerically close.
- Numeric equality, decided as **C-2b only after the Unity capture**: **bit parity where the evidence shows it achievable** against the Unity reference; otherwise a review-approved tolerance tied to IEEE float ulps and the *minimum distance to branch thresholds*, with hard guards that keep threshold/case outcomes, death sets and victim order identical. Do not arbitrarily declare `1e-6` globally safe. The capture records float32 and double intermediates as raw bits.
- Annual ledger: all living after step obeys `N_end=N_start−executedFell−modelNaturalDeaths` for a no-recruitment closed stand, with applied BA changes partitioned by actual size/time. No natural mortality on no-growth P0. Dead/felled never regrow, do not receive duplicate awards and do not become recruits.
- Replay (R-2): uninterrupted and split-at-arbitrary-year (save/reload) runs must be **biologically and work-outcome equivalent**: the same per-tree biological state, ecological year, model+RNG versions and seed, mortality/felling IDs, causes, years, roll inputs and legally equivalent executed work, with no-repeat victims. **Raw event-history hash equality applies only to complete identical canonical histories** (ordered event bytes, sequence, previous/post hashes and hashed approval-session fields). Biological projection, whole-snapshot hash and audit/history hash are distinct. A different load event or a later approval with a fresh session epoch may change the raw history and hashed snapshot fields without changing the biological outcome; each actual chain must validate independently. A normalized projected comparison, if later desired, requires a separately named/versioned C-1/C-1A policy defining field retention/exclusion, session-epoch handling, sequence renumbering and deterministic rehashing before P1-03:
  - **Mode A** (no open APPROVED/PENDING_WORK plan at the split): compare biological state and legally equivalent work outcomes. **Do not assert matching raw chain hashes if a load event is recorded or a later approval introduces a new session epoch**, even when no plan was open at the split. Raw hashes must match only for identical complete canonical histories; otherwise independently validate both chains. Counterexamples: recorded load audit event; approval after the split carrying distinct epoch-bound payloads. No normalized projection is approved here.
  - **Mode B** (open plans): under C-1A Option 1 (P0 semantics), exactly one `PlanStale(SessionLoaded)` per affected plan at load. Compare the lifecycle by named event semantics; raw hashes are **expected to differ** from the uninterrupted run, where the plan stales at the next step. Under Option 2, apply that policy's own reviewed oracle.

  No wall-clock/host-session reseeding.
- Persistence: separate P1 reader rejects P0/no-format/unknown-version/path/special-file/oversize/CCF-game saves, never follows write-path symlink, preserves mode 0600, GUID temp cleanup and trustworthy HTTP status, and does not open a network listener beyond literal loopback.

**P0 protected regression proof:** Recheck immutable fixture SHA manifest, original P0 Core 82/Host 13 baseline and 15 Chromium in proportion to any shared Host/UI surface changes, plus original 35 mutation controls and selected R-1/R-2 races only where modified security paths warrant it. Do not run every possible game/full Unity test or weaken tests just to get green. Windows WSL2 Edge and source Linux x64 are separate host records; new P1 browser operation only after bounded scoped implementation.

## 6. Irish comparator and provenance controls

- Use existing `Tools/Verification/Growfor` named historical `BroadLynch2006_Sitka` stand comparator; earlier independent review ran its **24+18 offline tests**, showing internal correctness against extracted equations, not actual executable or field verification.
- Match pure even-aged stand B m²/ha, N stems/ha, H top height m; set age, historical thinning and first-thinning domain, SI30 and top-height sample convention explicitly. Avoid comparing P0 action-eligible BA to GROWFOR full-stand BA without relabelling.
- A large model difference is research evidence, not automatically a code defect or an invitation to retune CCF. No Irish predictive accuracy threshold can be borrowed from the 2017 study RMSE, no individual mortality/hazard confidence can be inferred from a stand N model.
- Original 2006 volume selected-page spot check covered stand state/age30 SI, 113 **volume-model** calibration observations and G2b's 70/43 plot split. Full original GROWFOR manual/executable, full 2013/2017 originals and precise field H definition remain unchecked. Mark every cross-source accuracy assertion with its level of verification.
- No GBIF plot import: [#479](https://github.com/jermur788/CCF/issues/479) did not establish complete licensed Irish spatial tree data or longitudinal observations.

### Exact independent review trace (m-1…m-8)

The spec review must confirm **m-1** Mature-only stage-sensitive height, **m-2** fixed source productivity, **m-3** synthetic pre-inventory dead provenance, **m-4** float32/double and serialization (superseded in part by R-1: Unity capture is the reference of record; C-2a/C-2b timing), **m-5** square centred source geometry versus P0 polygon, **m-6** RNG-version-separated mortality rolls, **m-7** 113 stand observations used in *volume-equation* calibration and page-base clarification, and **m-8** quantitative threshold/density/SI fixture risk. Every one must remain testable and independently reviewed before G2 (P1-G2) spec approval.

## 7. Expected handoffs and integration verdicts

For each later substantive execution or independent review, publish a **durable GitHub Markdown handoff** with exact branch/source HEAD, actual owner/model/host, live lock/claim, exact file set, commands/observations and true verdict. Separate **IMPLEMENTED**, **REVIEWED**, **INTEGRATED**, **VERIFIED** and **ACCEPTED**. Manager review labels **APPROVED / NEEDS CHANGES / REJECTED** do not automatically imply user product acceptance or game integration.

Every dispatch must specify difficulty, best provider/capability, precise lane/worktree ownership, task claim, exact handoff path, and paste-ready GitHub URL; never leave the recipient to discover context. Any result requiring a new product choice/independent review halts coding pending that gate.

**Today:** Only the four Markdown **DRAFT** files under this spec and a source handoff were authorised by [#528](https://github.com/jermur788/CCF/issues/528); their docs-only R-1…R-10 revision and one revision handoff are authorised by [#536](https://github.com/jermur788/CCF/issues/536). P1-01 through P1-05 tasks are proposed and **NOT RUNNING / NOT AUTHORISED**. Dashboard/Drive mirror NOT VERIFIED.
