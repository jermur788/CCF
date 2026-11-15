# MULTI-012 — Independent four-file DRAFT review: Sitka P1 CCF adult-rules comparator

**Issued:** 15 November 2026  
**Review issue:** [#531](https://github.com/jermur788/CCF/issues/531) — **READY / NOT RUNNING**. No actual reviewer, physical machine/worktree, lease or test has been claimed.  
**Task:** Independent **specification QUALITY review only** of the user-approved *DRAFT*, with an evidence-backed **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED** verdict. This **does not** approve the spec, resolve unreviewed product choices, release P1-01..05 or LAB-07 implementation, run Unity, or accept Irish growth predictions.  
**Difficulty:** **HIGH**, because CCF source truth, source-number float semantics, constrained ecology and annual ordering, versioned no-growth-P0 isolation and empirical claims must be evaluated across four interdependent files.  
**Recommended independent reviewer:** **Claude Opus 5.5 High in local T3 Code/Claude Code**, or an equivalently capable genuinely non-author forestry + .NET systems spec reviewer. **Draft author was OpenAI GPT-6 (GitHub-only); do not let the author self-approve.** Report actual model/provider and accessible tools rather than assuming the requested effort was exercised.

## 1. DRAFT to review and custody evidence

**Direct owner decision:** on 15 November 2026, the product owner replied **"approved"** to the Manager's narrowly worded request to **prepare a separate four-file P1 DRAFT only**. Intended Option A is a **faithful adult-only closed pure-Sitka CCF rules learning comparator, explicitly NOT an Irish forecast, with separate versioned P1 files/state and unchanged P0**. Owner did **not** adopt final C-1..C-7 design defaults, approve a P1 spec or authorise P1 coding.

**Authoring issue:** [MULTI-011 #528](https://github.com/jermur788/CCF/issues/528), **HANDED-OFF/CLOSED**; actual author GitHub-only OpenAI GPT-6 registered RUNNING before detailed source inspection/edits. [Full DRAFT handoff](https://github.com/jermur788/CCF/blob/docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-011-P1-ADULT-SITKA-DRAFT-SPEC-HANDOFF-20261115.md). Source branch \`docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1\` at final \`50178f9df0641a75547fbeb29033d18a69a5cbb7\`, based on actual main \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`.

**GitHub Manager-author custody:** fetched all five authored files at final branch; compared against starting main: **exactly five ADDED Markdown files**, zero modified/untracked source paths (4 new DRAFT + 1 handoff). Each file explicitly states not approved, includes m-1..m-8 and C-1..C-7. This is *structural coverage only*, **NOT an independent correctness or scientific source review**.

**Four files to review in full, same branch:**
- [DRAFT spec](https://github.com/jermur788/CCF/blob/docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md) — actual purpose, proposed model/stand state, C-1..C-7 and acceptance.
- [DRAFT plan](https://github.com/jermur788/CCF/blob/docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md) — work sequence, ownership/locks, oracle and verification gates, science boundaries.
- [DRAFT tasks](https://github.com/jermur788/CCF/blob/docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md) — P1-S0 independent review, P1-G0 future owner/Manager spec approval, later P1-01 oracle → P1-02 adapter → P1-03 annual ledger/save/replay, P1-04 stand model comparator → P1-05 UI (all proposed and not authorised).
- [DRAFT requirements](https://github.com/jermur788/CCF/blob/docs/multi-011-sitka-p1-adult-rules-draft-20261115-v1/specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md) — untested acceptance, negative, source/domain and independent review matrix; **all checkboxes intentionally unchecked**.

**Binding parent evidence (read rather than infer):**
- [MULTI-009 LAB-06 Codex source study](https://github.com/jermur788/CCF/blob/research/multi-009-sitka-p1-source-evidence-feasibility-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-009-LAB-06-SITKA-P1-SOURCE-AND-EVIDENCE-FEASIBILITY-HANDOFF-20261114.md).
- [MULTI-010 independent Claude Opus technical/forestry review](https://github.com/jermur788/CCF/blob/review/multi-010-lab06-sitka-p1-forestry-method-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-010-LAB-06-SITKA-P1-INDEPENDENT-FORESTRY-AND-METHOD-REVIEW-20261114.md), 0 blocking/major and eight independent MINOR issues; 9 arithmetic reconstructions independently reproduced, **NOT Unity parity**.
- [Manager APPROVED AS P1 EVIDENCE PLAN ONLY](https://github.com/jermur788/CCF/blob/docs/manager-multi-010-p1-evidence-plan-verdict-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-010-LAB-06-INDEPENDENT-REVIEW-VERDICT-AND-P1-PRODUCT-GATE-20261115.md).
- [Approved P0 synthetic no-growth spec](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md); P0 qualified independent [repair #511](https://github.com/jermur788/CCF/issues/511) and [Windows/WSL #514](https://github.com/jermur788/CCF/issues/514) remain unchanged.
- [Canonical Game Brief](https://github.com/jermur788/CCF/blob/main/Docs/Project/game-brief.md), [Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md), [Current Milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md): first external demo is complete three-species Scenario One, not the isolated Sitka lab. Never claim Windows Unity/Player build authority.

## 2. Review branch, worktree and safety

**Already-created untouched reviewer branch:**
\`review/multi-012-sitka-p1-four-file-draft-spec-20261115-v1\`
based **exactly** on draft source final \`50178f9df0641a75547fbeb29033d18a69a5cbb7\` (GitHub comparison initial 0 changes). A new source commit or different author branch is NOT silently incorporated.

**Proposed reviewer worktree, NOT CREATED / NOT RESERVED:**
\`/media/jer/Files/CCF/Operations/CCF-MULTI-012-SitkaP1-DraftSpecReview-20261115\`, only if the review occurs on \`jer-laptop\`, actual drive/ownership/space and physical processes are free. Otherwise use a **new WSL ext4 reviewer worktree** after checking live ownership. **Do not reuse** the previous Codex T3 worktree, the #525 independent reviewer worktree, or owner game checkout. No Unity lease in this docs task.

**Proposed off-repo evidence root**, only if actually created on laptop:
\`/media/jer/Files/CCF/Verification/MULTI-012-SitkaP1-DraftSpecReview-20261115/\`.
No source binary need be generated; bounded scratch recalculations if necessary are allowed but no broad compile or heavy test suite is justified.

**Only writable tracked file, on reviewer branch:**
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md\`

**READ-ONLY:** all four DRAFT source files, parent research and approved P0 specs, CCF \`Assets/**\`, \`Tools/**\`, \`Docs/Project/**\` (including **LIVE** \`coordination/active-tasks.md\`), main, Unity project, game saves, frozen Reference Future, dashboard and Drive. Do not rewrite spec in the reviewer branch, lower test assertions, manipulate machine-wide Unity or change Windows/WSL settings.

**Before any source inspection/tests or writing:**
1. Fresh-read #531, this packet, MULTI-011 handoff, exact draft branch HEAD, existing #525 report and **live** coordination/actual machine/worktree/process state. A blank board row is not permission to touch anyone else's work.
2. Verify the proposed worktree location is unused/owned, branch really pinned, checkout clean at start; never stash/reset/clean other workers' dirty paths or arbitrarily normalize Windows/WSL file modes. Stop and claim **[BLOCKED]** if ownership/source conflict or missing core reference source prevents credible review.
3. Register **actual reviewer model/provider, host/worktree/branch/full starting HEAD, inspected source references, permitted review-only scope and [RUNNING]** on #531 **before** inspection/tests/report edit. The issue is execution presence, not board lock; do not edit board.
4. Use **one** review writer and one output Markdown. Stop any process you personally started by PID only; do not interfere with the user's other terminals.

## 3. Independent review matrix — mandatory issues and source truth

Review must evaluate **what the four files actually say**, not merely detect identifiers. Each finding needs file/section/line, source or accepted evidence reference and severity **BLOCKING / MAJOR / MINOR**, smallest safe documentation-only correction and product decision ownership. Reviewer can return a narrower scope if necessary; may not authorise code.

### A. Product / authority and P0 isolation

- User approved **drafting only**, not a final spec, binary32 choice, P1 science/implementation or Scenario One integration. Is that unambiguous in all four docs and the proposed handoff?
- P0 is **calendar-only, strict labSchemaVersion=1, integer-mm, immutable synthetic fixtures, plan state and replay**; inspect \`Tools/MultiRackSitkaLab/src/Core\` if accessible. P1 must NOT repurpose existing \`ExperimentStepAdvanced\`, rewrite P0 hash/event schema, mutate P0 files or allow unintended P0/P1 cross-load. Decide whether proposed P1 separate format/extension/root is plausible while preserving handle-relative secure writes and same-user threat limits.
- Is an immutable input fork from original fixture versus post-executed P0 snapshot/plan handled clearly? The DRAFT marks **C-1A** open. Flag any ambiguity that makes separate save/replay impossible or loses essential work provenance.
- No native Windows lab save claim; Windows Edge + Ubuntu WSL2 P0 previously tested. No separate Unity Windows build, Scenario One reduced scope or Reference Future change.

### B. Current source and P1 numerical scope

- Confirm or challenge CCF source at pinned main \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`: \`Assets/ForestPrototype/SitkaGrowthModel.cs\`, \`ForestEcologyController.cs\`, \`ForestTree.cs\`, \`ForestSaveData\` and key tests. Check actual model1 vs model2 pure Sitka Reference-demand adult route, new-game growth model2 and frozen legacy growth0. Source save schema24/historical verifier23 mismatch must remain honestly classified and repaired only under new authority.
- All **eight independent minor corrections** must be genuinely addressed and traced across four docs:
  - **m-1** Mature-only stage-sensitive height getter; no young/sapling masquerade.
  - **m-2** site productivity exactly 1 in current code, not an independently varied Irish site parameter.
  - **m-3** P0 pre-inventory \`Dead\` records have no cause/year; no fabricated ecological death event.
  - **m-4** CCF persists floats; float32 + some double arithmetic, exact evaluation/roundtrip and hazard threshold flips.
  - **m-5** source square centred x/z stand vs non-square P0 polygon/XY and float-coordinate boundary.
  - **m-6** RNG version **separate** from growth model determines death roll, seed/year/full tree ID.
  - **m-7** the 113 nonresearch stands were used in **volume-equation** calibration, not a final growth-model holdout; printed vs PDF-relative page anchors.
  - **m-8** ~10% living P0 below action threshold; perf fixture RD near 0.6; synthetic initial SI/height mismatch and need dedicated fixtures.
- Validate numerical source factors are **cited as source/calibration**, not Irish field truth: DBH potential 1.2, Ci50 5, inclusive 8m Hegyi, Class III E(30)=20.4, crown relax 0.15, all-living Dq/N RD, .6/1 density thresholds, mean S^4, 0.5 cap and count≤1 guard. Spot-check problematic singleton tiny-area and float dependence; do not call nine Python reconstructions Unity runtime parity.
- Does the proposal erroneously treat all below-P0-threshold live stems as naturally mature? This is **a deliberate artificial restriction needing source parity justification and visible limitations**, not an empirical age model. Is closed population consistent with adult comparisons, and are natural deaths separated from executed felling? Is no growth on proposal/approval guaranteed?
- **Runtime oracle C-6:** The first eventual engineering task must capture actual current-source per-ID step with source/seed, source precision, CI, growth, crown, post-growth density, death roll/victim order, model versions and event chronology. Is the DRAFT's selected bounded Unity/verified-.NET oracle route feasible before P1 adapter? Any hidden test/lease risks or unsafe fixture changes?

### C. Scientific authority and reference comparison

- Read [GROWFOR Technical Evidence Extraction V1B](https://github.com/jermur788/CCF/blob/main/Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md) and MULTI-010 caveats; actual published PDFs may be accessible as Game Dev attachments on some hosts but were **NOT** available to the earlier independent reviewer. If original papers not accessible to this reviewer, leave their claims **EXTRACTION ONLY**, not primary verified.
- Confirm stand B (m²/ha), N (stems/ha), H (top height) under pure even-aged stand domain, thinned/unthinned, SI top height age30, inadequate irregular/mixed/per-ID external validation and the 2013 GROWFOR executable/version caveat. No "field forecast accuracy", no use of 2017 NFI RMSE as P1 pass threshold.
- Check top-height selection rule is **NOT VERIFIED against primary source**; game diagnostic top100 largest DBH stems/ha is merely a proposed convention. Site/stand inputs and polygon area must be labelled source-vs-adapter modifications; no implicit SI correction/tuning.
- The project-attached 2006 volume was targeted-spot-checked earlier by Manager for B/N/H, SI30 and the 113 **volume** observations, not full 2013/2017/manual/executable equations. Do not promote that to full primary-source audit. Real Irish spatial/longitudinal marteloscope archive #479 remains NOT VERIFIED.
- \`Tools/Verification/Growfor\` existing named BroadLynch2006_Sitka comparator had 24/24 +18/18 *offline internal consistency* from prior reviewer, not current active GROWFOR executable identity/field-validation. Is the comparison study reproducible with documented domain flags and no automatic calibration?

### D. The seven OPEN decisions and draft plan cross-check

Check that **all C-1…C-7 truly remain unresolved pending product/independent approval**, and that each has an explicit technically testable proposal (not an accepted default):
- **C-1** P1 distinct format + immutable P0 source/plan fork;
- **C-2** exact binary32/double numeric/serialization policy;
- **C-3** all-living CCF biological denominator vs actionable P0 subset;
- **C-4** stage/crown/dead/site/age/seed/RNG initialization;
- **C-5** source square parity geometry, polygon extension and SI/top-height conventions;
- **C-6** actual current-source oracle capture and stale source harness ownership/leases;
- **C-7** visible synthetic non-forecast labels throughout.
Clarify which decisions must be made **before P1-01** fixtures (not silently after code begins), which might require a further owner's high-level choice and which can be settled by accepted technical spec.

Check \`plan.md\` G0 evidence vs G1 docs vs G2 approval gates and \`tasks.md\` P1-S0/G0/01..05 labels for **unambiguous order**, future owners/claims, scope and proportional verification. P1-04 can share no source-write overlap, and may operate in parallel with P1-03 only after P1-02 stable. Any route that depends on unsafe Unity machine checks or on a Win32 native lab port violates the approved boundary. Verify \`requirements.md\` criteria are observable and not pre-checked as evidence.

## 4. Review output, verdict and handoff

Only report file:
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md\`

The report must contain: real reviewer/host/model/worktree/preflight registration, actual inspected source refs and branch heads, clear severity findings (with exact minimum doc repairs), source/science/architecture/contract consistency checks, all m1–m8 and C1–C7 dispositions, unresolved owner decisions, and the separate **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED** verdict. **A DRAFT QUALITY approval only means the document is suitable for Manager/owner decisions; it is not an approved code spec.**

Record what ran (static source reading, optionally small independent arithmetic) and what did NOT run (Unity harness, production code, empirical model validation). After report publication, **GitHub fetch/readback** and compare reviewer branch to pinned final draft branch: **EXACTLY ONE ADDED Markdown (report) and zero changes to the four DRAFT or source**. Then mark #531 [HANDED-OFF], close, notify [#528](https://github.com/jermur788/CCF/issues/528) and [Overall Manager #425](https://github.com/jermur788/CCF/issues/425), return the exact GitHub URL and evidence. If NEEDS CHANGES, reviewer does **not** repair the draft themselves; Manager creates a bounded docs-only follow-on claim after ruling. If APPROVED AS DRAFT QUALITY, Manager next assesses OPEN decisions and asks owner as needed, then potentially approves a separate spec. No automatic implementation.

**Dashboard/Drive mirror sync: NOT VERIFIED.** No canonical decision log/context snapshot update or Drive publication is requested or authorised.

## 5. Paste-ready reviewer dispatch

> Take ownership of **MULTI-012 / CCF #531** — independent quality review of the user-approved **four-file DRAFT-only** Sitka P1 adult-rules specification. Full binding packet: https://github.com/jermur788/CCF/blob/docs/multi-012-sitka-p1-independent-draft-spec-review-dispatch-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-SITKA-P1-FOUR-FILE-INDEPENDENT-DRAFT-REVIEW-PACKET-20261115.md . **HIGH difficulty; use Claude Opus 5.5 High or another independent non-GPT-6 source/science+software-spec reviewer.** The original authoring task [#528](https://github.com/jermur788/CCF/issues/528) is HANDED-OFF/CLOSED. Candidate review branch \`review/multi-012-sitka-p1-four-file-draft-spec-20261115-v1\` is untouched from published four-file DRAFT. Proposed **UNCREATED/UNRESERVED** laptop reviewer worktree \`/media/jer/Files/CCF/Operations/CCF-MULTI-012-SitkaP1-DraftSpecReview-20261115\` after real board/host/branch preflight; on Windows choose separate WSL-ext4 worktree. Register **actual model/provider/host/worktree/branch/full starting HEAD** as RUNNING on #531 before inspection/test/writing; BLOCKED on conflict. Read all four DRAFTs and source study/review, verify eight m-1…m-8 and seven C-1…C-7 open decisions, P0 unchanged, model2/pure-Sitka model1 source version, float/area/RNG/mortality/year ordering, source-vs-synthetic assumptions, GROWFOR stand B/N/H and 2006 volume evidence limits, and parity-oracle-before-code sequence. Return **APPROVED AS DRAFT QUALITY / NEEDS CHANGES / REJECTED** with exact file/line findings, no code/spec edits. Publish solely \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md\` on your reviewer branch, verify GitHub exactly one added report Markdown and readback, close #531 HANDED-OFF and return to Manager. **No P1-01..05 coding, P1 spec approval, Unity/Windows Player build, production saves, P0 modification, field data import or live board write.**
