# P1 adult Sitka comparator — DRAFT plan and qualification gates

**Status: DRAFT / NOT APPROVED / NO IMPLEMENTATION.** Companion [spec](spec.md), [task breakdown](tasks.md) and [requirements](checklists/requirements.md). Owner approved writing the four-file DRAFT **only** on 15 November 2026. Cross-system persistence and annual biology require a separately approved specification **before** any LAB-07 implementation or Unity source operation.

## 1. Work classification and authority

**SPEC REQUIRED**, not a quick repair packet. The intended P1 work would add a new ecological time/state and persistence contract to an otherwise qualified, non-biological research laboratory. The Plan is deliberately split into an evidence oracle, adult source adapter, separate replay/save, stand reference comparison and minimal user-facing output. Every prospective activity below is **a proposal for later authorisation**, not a runnable assignment today.

Authority chain: direct owner DRAFT-only choice → [MULTI-009 source/evidence LAB-06](https://github.com/jermur788/CCF/blob/research/multi-009-sitka-p1-source-evidence-feasibility-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-009-LAB-06-SITKA-P1-SOURCE-AND-EVIDENCE-FEASIBILITY-HANDOFF-20261114.md) → [MULTI-010 independent forestry/method review](https://github.com/jermur788/CCF/blob/review/multi-010-lab06-sitka-p1-forestry-method-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-010-LAB-06-SITKA-P1-INDEPENDENT-FORESTRY-AND-METHOD-REVIEW-20261114.md) → [Manager APPROVED AS P1 EVIDENCE PLAN](https://github.com/jermur788/CCF/blob/docs/manager-multi-010-p1-evidence-plan-verdict-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-010-LAB-06-INDEPENDENT-REVIEW-VERDICT-AND-P1-PRODUCT-GATE-20261115.md). **None** is an approved P1 code branch or game integration authority.

The current source pinned in LAB-06 is a reference for spec drafting only. Future work must pin the **then-current inspected CCF source**, its actual source-only/game integration lineage and the separately qualified lab branch. Main or a prior report alone does not prove current Unity integration or local clean state.

## 2. Proposed architecture and file ownership

\`\`\`text
Immutable P0 synthetic fixture + validated P0 hash
             |  read-only, explicit import/fork
             v
      [separate P1 input manifest]
             |
     source-faithful candidate adapter         independent source-runtime oracle
     (per-ID adult CCF rules only)        <----  (current CCF exact candidate, never historical guess)
             |
  P1 annual transaction: after committed work -> CI -> DBH/height -> crown -> mortality
             |                                    |
             v                                    v
  new P1 tree/snapshot & event ledger      all-living + actionable subset metrics
             |
     distinct P1 format/save/replayer      BroadLynch2006_Sitka stand B/N/H comparison
             |
     explicit opt-in P1 UI/export
\`\`\`

This is not a second copy of the Unity forest and not a new source of truth for the production game. Proposed new P1 files live under an independently allocated sub-namespace of \`Tools/MultiRackSitkaLab/\` **after** approval. Existing \`src/Core/Engine.cs\`, schema1 P0 \`LabFile\`, P0 events/saves, approved lab fixture hashes, Host security and API must remain stable. Reviewer must inspect whether a separate binary/Host route and separate P1 file namespace are needed to avoid any P0 behaviour change; the specification makes no code-level integration decision by itself.

**Proposed single-writer ownership after approval (not a lock now):**
- P1-01 oracle source capture: **one current-Unity/source-verification owner** under a fresh local code lane, using the Scenario One integrator/Unity machine gate if Unity executable access is required. The pure-.NET source binding alternative must be independently validated before selecting it. Any stale schema-23 expectation must have its own reviewed repair and must not be weakened.
- P1-02/P1-03 P1 Core, state, format, file safety and tests: **one primary P1 lab source writer** across overlapping P1 C# files; avoid a simultaneous second C# writer. Do not borrow the P0 R-1/2 repair worktree or user Windows game files.
- P1-04 scientific comparator: **one independent evidence/numeric owner** on existing \`Tools/Verification/Growfor\` via read-only scratch output until a separate approved writer lane is necessary; preserve the existing versioned 2006 implementation, do not rewrite it by default.
- P1-05 UI/presentation: only after stable DTO/API/data contract, one UI owner can work in a separate non-overlapping file set, with P0 screens and Host routing protected. End-to-end qualification requires actual Windows/WSL browser and source lineage pin.
- Reviews: a source-independent reviewer after each substantive code handoff, plus proportionate aggregate/science and file-security review; evidence and approval claims must stay separated.

**Machine safety:** Live \`Docs/Project/coordination/active-tasks.md\` remains sole active ownership authority; workers refresh it, check host/branch/worktrees/Unity leases and mark actual task issue [RUNNING] before any source edit/test. No worktree/lock created by this DRAFT. Do not kill, clean, stash or overwrite other lanes. Heavy Unity qualification requires an exclusive per-machine phase lock. No Windows Unity Player/game build before the user's personal Scenario One playtest and specific approval.

## 3. Explicit dependency and release sequence

| Gate | Proposed owner and difficulty | Inputs | Exit evidence | State today |
|---|---|---|---|---|
| **G0 Evidence plan** | Codex research + independent Opus review; HIGH | current CCF code, GROWFOR extraction, P0 lab | 9 formula recomputations independently checked, source methods traced, 8 minor corrections | **COMPLETE: approved as evidence-plan quality ONLY** |
| **G1 Four-file DRAFT + independent spec review** | docs-only GPT-6 author + independent non-author critical spec reviewer; HIGH | owner DRAFT-only approval, eight minor and seven open decisions | no-implementation four-file DRAFT, complete cross-file review and diff, findings disposition | **IN PROGRESS / NOT APPROVED** |
| **G2 P1 product+spec approval** | product owner and Overall Manager | independent spec verdict; C-1…C-7 decisions explicitly resolved or justified | separate accepted decisions / approval, exact APPROVED status and implementation packet release | **BLOCKED UNTIL APPROVAL** |
| **G3 P1-01 real runtime oracle** | bounded source/Unity verification owner, HIGH | G2, source HEAD, physical safe lane, permitted test source and precision contract | per-ID annual oracle including float expression/order, DBH/H/CI/crown/roll/victims, approved exact/tolerance criteria, no source/game mutation | **NOT AUTHORISED** |
| **G4 P1-02 adapter** | P1 C# lab sole writer, HIGH | G3 accepted source vectors and C-2…C-5 contract | independent numeric per-ID parity, stable source+parameter manifest; no P0 mutation | **NOT AUTHORISED** |
| **G5 P1-03 event/persistence** | same P1 Core writer, HIGH | G4, reviewed P1 schema/version and P0 preservation proof | deterministic one-year and repeated-year replay, save/reload parity, refusal/security and history provenance | **NOT AUTHORISED** |
| **G6 P1-04 stand comparison** | forestry/evidence lead separate read-only lane, HIGH | G4 deterministic outputs; GROWFOR comparator requalified | matched pure even-aged B/N/H time series with source/age/SI/thinning/domain caveats; no field forecast claims | **NOT AUTHORISED** (may overlap G5 once G4 stable) |
| **G7 P1-05 minimal user interface + qualification** | non-overlapping UI owner and independent reviewer, MEDIUM–HIGH | G5, G6 scientific labels, defined API | clearly labelled per-ID changes/BA/N/H history; P0 exact regressions; Windows Edge/WSL end-to-end and valid source evidence | **NOT AUTHORISED** |
| **G8 Any game integration** | Scenario One manager, new approved multi-system spec and full qualification | separate owner decision and approved game feature | production parity, saves/Reference, integration, Unity, render and user acceptance | **OUT OF SCOPE** |

**Stop at any failed gate.** A research model can be left isolated at P0/P1 prototype status; passing G4 does not justify G5/7 automatically if P0 integrity or source semantics fail. G6 numerical match is *model-to-model plausibility*, not Irish ground truth.

## 4. Decisions and assumptions to close BEFORE P1-01

The [spec decision register](spec.md#7-draft-decision-register-c-1c-7--not-yet-adopted) deliberately lists C-1…C-7 as **OPEN**. For G2 the independent reviewer and owner/Manager must address:

1. **C-1 separate P1 identity:** source P0 fixture hash and exact P0 snapshot, no write to P0, distinct P1 format/root, import/fork plan history policy. No P0 field migration disguised as schema1 extension.
2. **C-2 binary32/double contract:** exact CCF operation types and deterministic serialization vs tolerated binary64. A 1e-8 drift can switch \`roll < p\`, a tie or 8 m boundary, so "approximately same" cannot guarantee annual event parity. Define an independent oracle comparison budget with hard invariants for victim sets.
3. **C-3 population:** all living CCF stems in CI/RD including below-threshold (review found ~10% P0 live below threshold), actionable subset as independent metrics. Mature-equivalent for ALL included live stems must be clearly declared, not passed off as real measured age class.
4. **C-4 input assumptions:** no P0 cause/year for Dead; treat as pre-inventory-dead with an explicit non-event provenance, stage Mature, crown init rule labelled synthetic, site=1 current code fact, growth/RNG version and seed/year identity.
5. **C-5 area/site/top-height:** source square centred area and coordinates for hard parity; polygon and site-index comparisons are separate declared changes. Broad–Lynch top-height field definition unresolved; no fabricated exact equivalence.
6. **C-6 oracle capture ownership:** choose narrow permitted source harness or reviewed equivalent, keep stale v23 assertion repair independently owned, no unsafe Unity machine contention.
7. **C-7 visible scientific labels:** synthetic rules not field forecasts, domains and unavailable regimes visible throughout UI/export/figures.

**Eight reviewer minor corrections m-1…m-8** must be reflected in \`spec.md\` §3–7 and the requirements table. An independent spec reviewer must check their dispositions, not merely count the identifiers.

## 5. Numeric, biological and event verification design

**Independent scalar baseline:** Prior Codex V01–V09 plus Opus binary32/64 recomputation are useful positive controls, not an oracle. Each future current-source capture must report actual CCF inputs, model and RNG versions, source+fixture hashes, precision at each calculation, ecological age/year and exact per-ID outputs. V10–12 are source-derived/historical only; V13–V14 remain unavailable.

**Future oracle cases, at minimum:** low/high CI; 8 m inclusive/exclusive and same-location 0.5 m distance floor; stable ordinal order with ties and neighbour list permutations; age-phase refusal and normal Mature height; 20→21 site curve; all-living vs below-eligible threshold; .6 onset and >1 density reduction; extreme one-stem/tiny-area; no-thin/one-thin/repeated-thin including felling timing; short/long ages; pre-inventory dead; natural-death-vs-fell; RNG model0 vs 1; source growth1 vs pure-Sitka Reference-demand growth2; source coordinate-centre and area.

**Runtime comparison criteria (DRAFT proposal):**
- Exact event **topology and IDs**: eligibility, affected tree count, plan status, death vs fell, victim selection, cause and year, deterministic roll inputs and output for a pinned source policy; no substitutions when numerically close.
- Numeric equality: **bit parity wherever obtainable** for source float computations under a pinned environment, otherwise a review-approved small error tolerance tied to IEEE float ulps and the *minimum distance to branch thresholds*, with separate hard guards on threshold/case outcomes. Do not arbitrarily declare \`1e-6\` globally safe. Record actual float32 and double intermediate values when possible.
- Annual ledger: all living after step obeys \`N_end=N_start−executedFell−modelNaturalDeaths\` for a no-recruitment closed stand, with applied BA changes partitioned by actual size/time. No natural mortality on no-growth P0. Dead/felled never regrow, do not receive duplicate awards and do not become recruits.
- Replay: uninterrupted and split at arbitrary year (save/reload) must agree in ordered per-tree state/event hashes, model+RNG versions, plan staleness and no-repeat victims. No wall-clock/host-session reseeding.
- Persistence: separate P1 reader rejects P0/no-format/unknown-version/path/special-file/oversize/CCF-game saves, never follows write-path symlink, preserves mode 0600, GUID temp cleanup and trustworthy HTTP status, and does not open a network listener beyond literal loopback.

**P0 protected regression proof:** Recheck immutable fixture SHA manifest, original P0 Core 82/Host 13 baseline and 15 Chromium in proportion to any shared Host/UI surface changes, plus original 35 mutation controls and selected R-1/R-2 races only where modified security paths warrant it. Do not run every possible game/full Unity test or weaken tests just to get green. Windows WSL2 Edge and source Linux x64 are separate host records; new P1 browser operation only after bounded scoped implementation.

## 6. Irish comparator and provenance controls

- Use existing \`Tools/Verification/Growfor\` named historical \`BroadLynch2006_Sitka\` stand comparator; earlier independent review ran its **24+18 offline tests**, showing internal correctness against extracted equations, not actual executable or field verification.
- Match pure even-aged stand B m²/ha, N stems/ha, H top height m; set age, historical thinning and first-thinning domain, SI30 and top-height sample convention explicitly. Avoid comparing P0 action-eligible BA to GROWFOR full-stand BA without relabelling.
- A large model difference is research evidence, not automatically a code defect or an invitation to retune CCF. No Irish predictive accuracy threshold can be borrowed from the 2017 study RMSE, no individual mortality/hazard confidence can be inferred from a stand N model.
- Original 2006 volume selected-page spot check covered stand state/age30 SI, 113 **volume-model** calibration observations and G2b's 70/43 plot split. Full original GROWFOR manual/executable, full 2013/2017 originals and precise field H definition remain unchecked. Mark every cross-source accuracy assertion with its level of verification.
- No GBIF plot import: [#479](https://github.com/jermur788/CCF/issues/479) did not establish complete licensed Irish spatial tree data or longitudinal observations.

### Exact independent review trace (m-1…m-8)

The spec review must confirm **m-1** Mature-only stage-sensitive height, **m-2** fixed source productivity, **m-3** synthetic pre-inventory dead provenance, **m-4** float32/double and serialization, **m-5** square centred source geometry versus P0 polygon, **m-6** RNG-version-separated mortality rolls, **m-7** 113 stand observations used in *volume-equation* calibration and page-base clarification, and **m-8** quantitative threshold/density/SI fixture risk. Every one must remain testable and independently reviewed before G2 spec approval.

## 7. Expected handoffs and integration verdicts

For each later substantive execution or independent review, publish a **durable GitHub Markdown handoff** with exact branch/source HEAD, actual owner/model/host, live lock/claim, exact file set, commands/observations and true verdict. Separate **IMPLEMENTED**, **REVIEWED**, **INTEGRATED**, **VERIFIED** and **ACCEPTED**. Manager review labels **APPROVED / NEEDS CHANGES / REJECTED** do not automatically imply user product acceptance or game integration.

Every dispatch must specify difficulty, best provider/capability, precise lane/worktree ownership, task claim, exact handoff path, and paste-ready GitHub URL; never leave the recipient to discover context. Any result requiring a new product choice/independent review halts coding pending that gate.

**Today:** Only the four Markdown **DRAFT** files under this spec and a source handoff are authorised by [#528](https://github.com/jermur788/CCF/issues/528). P1-01 through P1-05 tasks are proposed and **NOT RUNNING / NOT AUTHORISED**. Dashboard/Drive mirror NOT VERIFIED.
