# Objective 2 — #442 NEEDS CHANGES: bounded DRAFT-revision packet

**Date:** 12 November 2026. **Status:** READY / NOT RUNNING; no gameplay or product approval.

**Purpose:** Correct the #441 whole-rack forestry research and four-file DRAFT specification in one documentation-only revision, responding to **two blocking, six major and seven minor** findings from the independent Claude Opus 5.5 #442 review.

**Exact review, including section 5 checklist:** [Claude #442 review](https://github.com/jermur788/CCF/blob/docs/claude-442-objective2-whole-rack-review-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/CLAUDE-442-OBJECTIVE2-WHOLE-RACK-INDEPENDENT-REVIEW-20261112.md). Read it fully; its section 2 code/source confirmations need not be repeated unless edited.

**Difficulty: HIGH** due to a real extrapolation-rule ambiguity, yearly Work Plan chronology, source provenance and old-save semantics. **Best author:** GPT-6 High docs worker or equivalent Claude; the original author may revise, but the delta re-review must be performed by someone **other than the revision author**. **Branch:** docs/objective2-whole-rack-442-revision-20261112-v1, forked from the precise reviewed #441 head (the full source identifier is in #442 review). **Local worktree / Unity:** NONE. On actual start register provider/model, branch, starting HEAD, and [RUNNING] claim before writing. Manager #424 is the sole writer of live active-tasks.md; revision worker must not edit it.

## Exact writable files
- Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-FULL-RACK-RESEARCH-AND-EXTRAPOLATION-EVIDENCE.md (EV)
- specs/scenario-one-objective2-whole-rack-sampling-v1/spec.md (SP)
- specs/scenario-one-objective2-whole-rack-sampling-v1/plan.md (PL)
- specs/scenario-one-objective2-whole-rack-sampling-v1/tasks.md (TK)
- specs/scenario-one-objective2-whole-rack-sampling-v1/checklists/requirements.md (RQ)
- Optional audit correction/addendum to the original #441 final worker handoff, while keeping the historical record clear.
- New final result: Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-442-DRAFT-REVISION-HANDOFF-20261112.md.

Do not alter the original #441 branch or #442 review branch, main, approved D-065, D-076, decision log, current milestone, game code, Unity, saves, user session, or any #440 UI files.

## Required corrections for one revision and narrow re-review

**B1 — Source-to-target release rule, SP FR-07 + EV/PL/RQ.**
Define the proposed *new* rule version separately from MFM-V1-RULE-1. Specify which source Crop Trees count toward the transfer statistic (released-only vs all with zero; reviewer recommends released-only), how individual executed release Share is aggregated (minimum/quantile/median as alternatives, **not an accepted choice**), a proposed minimum count of genuinely released Crop Trees (numeric threshold is calibration, not evidence), and explicit status for legitimate no-release Crop Trees. Below that threshold show "Insufficient source evidence"; do not manufacture a positive target. Define the spatial denominator of the 40% cumulative BA safeguard: current D-065 applies it per coverage unit, while whole-rack average can conceal extreme local removal. Propose unit-level plus Crop Tree neighbourhood checks without treating a new unit size or ceiling as accepted. Reframe **OPEN-3** to include both lesson credit and transfer derivation/aggregation/local cap. Add positive/negative tests **AC-E01b** (mix of release/no-release), **AC-E01c** (minimum sample below threshold), extreme local area despite safe rack-wide ratio, double counted competitors and source baseline validity.

**B2 — Objectives 2–5 and D-073/074/077 chronology.**
Publish a table that explicitly maps existing Objective/Lesson 2,3,4,5 displayed sequence and their *real stable persisted step IDs* to proposed new objective stages: whole-source walking/Crop Tree judgement, competitor release, source Work Plan approval/execution/review, four-target coverage/confirmation, prescription approval, destination Work Plan and actual execution. Inspect current code; DO NOT INVENT IDs or save schema versions. Distinguish old saved ID semantics from new guided IDs; never migrate D-066 historical three-tree completion into whole-rack completion. Add year/campaign columns. **D-074** cautions against major management thinning in consecutive years. **D-077** makes Skip to Next Management Year unavailable while pending/approved work remains and proposes four-year recurrence. **D-073** governs multi-year outcome review, not the user's Editor session. New **OPEN-6** is a product choice: (A) source in year N and destination in N+1 are deliberately *one staged intervention campaign*, or (B) destination release is deferred to the next permitted management year with fresh source/current-state proof. Reviewer favours A but **neither is ratified**. Tests must verify real Work Plan year semantics, skipped-year eligibility, no duplicate/carry-forward work, and repeated D-076 cycle.

**M1 — Canonical CCF Book.**
Cite [canonical photographed-book reference](https://github.com/jermur788/CCF/blob/main/Docs/Research/CCFBook/CCF_Book_Definitive_Reference.md) in EV and SP with correct evidence classes and source caveat: original publication title/author/date not fully known.
- p40: context-dependent 16–20m rack-spacing examples, NOT exact machine reach or universal standard.
- p75: sample plots measured before felling, marking in and around, marked-tree BA quantified and marking intensity adjusted; general guidance at most 20% pre-harvest BA and no more than increment (needs two inventories), NOT a supersession of game's existing 40% BA teaching safeguard.
- p87: conifer quality trees generally selected at **second** thinning, broadleaf at first, though any stage is possible. Explicitly label first-cycle Sitka crop designation in game as a possible pedagogical timing abstraction.
- [Research index D-065](https://github.com/jermur788/CCF/blob/main/Docs/Project/research-index.md) records evidence-informed analogies and warnings about "operator selection" and cab-visibility/individual judgement.

**M2 — Replace false within-rack "thirds".**
Existing C-01 Low/Centre/High sections split at the **old sample anchor ±12m**, not equal thirds of rack length. New whole-rack coverage needs an explicitly proposed new finite longitudinal segmentation/tiles rule, new calibration and source-unit version; do not silently reuse the anchor-tied 15 areas as a valid within-rack comparable classifier. Change SP FR-06, PL Phase 3, EV, RQ AC-E02, AC-E09, AC-G04. Protect historical coverage/source identities and old prescriptions.

**M3 — Destination candidate workload.**
Cite D-065 §8.2 historical MFM-01 estimate of **several dozen** proposed candidates at default scale. It is only prior order-of-magnitude data, not a measured whole-rack count. T03 must enumerate actual new source crop count k, competitor decisions f, each destination queue m_i and first-time completion time from a real pinned fixture *before OPEN-5*. Increased workload is plausible but not measured; do not assert quantitative direction without data.

**M4 — Jones 2026 accuracy.**
[Primary DOI](https://doi.org/10.1016/j.foreco.2026.124163) supports rapid BA recovery (around 55 m²/ha in some treatment), higher crown-thinned tree slenderness/stability caution, and **no natural regeneration after 14 years** in studied stands. Do not attribute specific Q-tree selection directly to Jones unless a primary passage proves it; label it as a claim from supporting synthesis instead. Mention the stability/regeneration warnings in teaching text without inventing a game slenderness/wind model. Link rapidly changing conditions to D-076 refresh without promoting a fixed timetable.

**M5 — Off-repo project sources.**
Three uploaded Project-source files were hashed from the mounted original attachments in this task-preparation session; this proves their identity, NOT GitHub accessibility:
- CCF_Six_Papers_Relevance_Evidence_Extraction.pdf — SHA-256 f8a699d96d5146d05f74ec1c5b2b2ba2206986665b0019472c61a3ba3d29f8bf.
- Delegated Marking and Operator Selection in Irish, UK and European Forestry.pdf — SHA-256 1ea082abe9706b4df792489ce7b734b61f682425f3a64e2bbd0e9427f5f1e797.
- CCF_CCF_Practice_Economics_and_Transformation_Evidence_Composite.md — SHA-256 cb1c2421d348fc664547badd36d46db6862bc1fe10d377fd37a582d23bc173a6.
Add source manifest (Project attachment name, bytes/hash, source pages and external stable location **only if verified**); explicitly say no repo copy exists. A GitHub-only reviewer may not have the attachments; downgrade unique claims supported only by them to **UNVERIFIED SECONDARY — SOURCE ACCESS REQUIRED**. No invented Drive links or assumption mounted paths are universally accessible.

**M6 — Fix D-073.**
Replace "D-073 protected personal playtest" everywhere with user-owned protected Scenario One personal Editor session / Manager #424 / historical #382 candidate. No verified terminal lease-release receipt means no Unity use. Correctly cite D-073 under truthful multi-year decision/outcome review FR-13; D-074 and D-077 are distinct cadence and skip authorities.

**Minors m1–m7 (ALL required).**
m1: lesson source is **on**, not **after**, third rack sorted by normal offset.
m2: "one DAFM WIS control plot per homogeneous unit" was not verified in primary extracted text; remove/downgrade absent exact primary page/table. Consider DAFM WIS **Element 3 CCF / Annex 4** as closer conifer transformation analogue, distinguish Annex 2 broadleaves.
m3: cite Forest Research as Forestry Commission **Internal Project Information Note 13/06**, plus cab visibility limitations on crown assessment.
m4: Pommerening 2021 is a **Tamm Review including 26-site marteloscope analysis**.
m5: use "D-066 teaching-completion latch (schema version to verify on implementation HEAD)" rather than an unsupported "v24" assertion.
m6: add handoff audit correction with explicit Base/Context/Reviewed HEAD/GitHub handoff commit identifiers (available in original #441 claim and #442 review), without implying source implementation or fabricating runtime evidence.
m7: in SP authority list and FR-14, D-066 latch stays **one-way** and never certifies a full-rack survey.

## OPEN-1..6 and result validation

Keep **all OPEN-1..6 unratified**. Include #442 §4 reviewer options, clearly labelled recommendations, without choosing. The user explicitly prioritises **Objective 1 implementation first**; Objective 2 implementation remains deferred until independent re-review, product acceptance, source ownership transfer and safe Editor handback.

The revision worker must produce a **finding-by-finding change matrix** (B1/B2/M1–M6/m1–m7), exact changed sections/files, source/evidence caveats and a GitHub read-back + branch-diff proving only allowed Markdown changes. No code tests or Unity runs for this docs task. Any unverified save ID/code fact is a disclosed future gate, never guessed.

**Review after revision:** an independent **delta reviewer**, not the revision author, reads only the corrections under #442 §5 and checks for regressions. Issues **APPROVED / NEEDS CHANGES / REJECTED for DRAFT quality**, not user/product acceptance. #425/user then consider OPEN-1..6, #424 keeps live task and downstream source ownership. #418 stays a separate unresolved provenance gate. Dashboard and Drive sync remain UNVERIFIED absent receipts.

**Final mandatory handoff:** Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-442-DRAFT-REVISION-HANDOFF-20261112.md on the revision branch.

**Paste-ready dispatch:** Take the CCF Objective 2 #442 NEEDS CHANGES documentation revision. Read this complete GitHub packet, register your actual provider/model and claim [RUNNING] on the assigned docs branch, repair both blockers, six majors and seven minors across the allowed research/four-file DRAFT, publish the stated GitHub handoff and verified diff. Do not decide OPEN-1..6; no local worktree, Unity, production source, #440 files, active-tasks.md, or approved authority edits. A different reviewer will assess only your delta.
