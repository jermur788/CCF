# Manager #424 — #441 whole-rack Objective 2 research vs #442 independent review

**12 November 2026. FORMAL MANAGER VERDICT: NEEDS CHANGES — RESEARCH AND DRAFT SPECIFICATION ONLY.** This neither rejects the direct user's whole-rack Objective 2 product intent nor authorises implementation. It does not settle OPEN-1..OPEN-6.

## Actual evidence inspected

- [#441 research + DRAFT handoff](https://github.com/jermur788/CCF/blob/docs/manager-424-full-rack-objective2-research-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-WHOLE-RACK-RESEARCH-DRAFT-SPEC-HANDOFF-20261112.md), [original evidence report](https://github.com/jermur788/CCF/blob/docs/manager-424-full-rack-objective2-research-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-FULL-RACK-RESEARCH-AND-EXTRAPOLATION-EVIDENCE.md), and its four-file DRAFT under `specs/scenario-one-objective2-whole-rack-sampling-v1/`.
- [Independent Claude Opus 5.5 #442 review](https://github.com/jermur788/CCF/blob/docs/claude-442-objective2-whole-rack-review-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/CLAUDE-442-OBJECTIVE2-WHOLE-RACK-INDEPENDENT-REVIEW-20261112.md): actual independent reviewer registered RUNNING before work in [#442](https://github.com/jermur788/CCF/issues/442#issuecomment-6838053421), returned documented NEEDS CHANGES in [handoff comment](https://github.com/jermur788/CCF/issues/442#issuecomment-6838232172), #442 HANDED-OFF / CLOSED. GitHub compared reviewer output to its pinned reviewed baseline: **one new Markdown file only**. No code changes, Unity, tests or player test took place.
- Canonical accepted D-065 and D-076 DRAFT, decision log D-073/D-074/D-077 are cited within the reviewer. The reviewer checked selected C-01 source and external references; this Manager verdict **does not assert independent repeat PDF extraction or current Unity behaviour**.
- GitHub reviewed #441 branch still matches the reviewed original source HEAD; the successor docs-only revision branch is rooted at the reviewed version, not at the reviewer output.

## Decision

**APPROVE the #441 docs-only delivery and #442 independent review as accurately scoped research/handoffs. NEEDS CHANGES on the substance of the Objective 2 DRAFT before specification promotion or implementation.** The external source and inspected code facts the reviewer confirmed need not be re-researched absent changes. Two blockers and six major/seven minor corrections remain.

### Blocking conditions

**B1 — derive a meaningful treatment rule from a whole-rack source:** the current prescription uses the **smallest executed release**. New whole-rack gameplay permits some Crop Trees to be retained with no release, so a minimum over all selections is zero/undefined and can distort the four-rack proposed treatment. The revised draft must explicitly define *which releases enter the calculation*, propose/compare defensible aggregation and minimum evidence count, specify how absence of releases gives `InsufficientEvidence` or a separately justified no-treatment case, and define the **40% basal-area cap at an actual spatial unit** with local safeguards. New rule/calibration identity required; do not reuse old `MFM-V1-RULE-1` for changed meaning. These are **proposals to be product-approved**, not manager-assigned invented thresholds.

**B2 — lesson and annual timing contract:** enumerate each old Objective 2–5 step/ID, its new desired player action, completion and retained history, Year/Work Plan execution timing and save/read transition. Cite D-073 annual outcomes, D-074 spaced major management and D-077 conditional time skip. Show source-work then four-rack destination-work as possibly adjacent years. Add **OPEN-6** whether both are one staged intervention campaign or destination work should wait for later management year. Neither option is automatically accepted; no invented catch-up execution/skip around pending approved Work Plan.

### Major corrections

1. Cite canonical CCF Book **p.40 rack context, p.75 sample plot marking/measured basal-area calibration, and p.87 conifer Crop Tree timing**, and research-index scope/evidence labels. A first-year Sitka quality-tree choice is potentially a **teaching abstraction**, not a standard first-thinning rule.
2. Replace old anchor-based Low/Centre/High bands (±12m from teaching sample, not thirds of each full rack) with an explicit proposed whole-rack segmentation rule and new calibration/version, then test real within-rack variation.
3. Cite existing D-065 §8.2 **several-dozen candidate** estimate as historical, not measured new outcome; measure actual new source/destination decision burden in a future authentic fixture before OPEN-5 delegation policy.
4. Correct Jones et al. 2026 attribution and incorporate slenderness/stability and **no regeneration after 14 years** evidence without claiming in-game model effects not implemented.
5. Three evidence documents described as "supplied" were not verifiable from Git repository: identify reproducible original storage and version/hashes where permissible, or downgrade unsupported claims. Do **not** silently publish private user uploads/large source PDFs into GitHub. Recheck unsupported DAFM one-plot-per-unit text or remove it.
6. Correct "D-073" misuse: it is an accepted **product decision** on decision→work→year-review, not the name of the user's personal playtest. Refer to [#424](https://github.com/jermur788/CCF/issues/424) / original protected test; cite D-073 properly for outcomes/history.

**Seven minor findings** in #442 §3 also required: third-rack precise ordering; unverified DAFM plot-frequency claim; official IPIN 13/06 naming and cab-visibility caution; correct Pommerening 2021 Tamm Review classification; save-schema version **NOT VERIFIED** (do not hardcode v24); complete handoff audit fields; D-066 historical teaching latch one-way/non-repurposable. Do not weaken original test/compatibility requirements.

## Delivery and priority ruling

**Latest direct user priority:** Objective 1 FIRST. The real implementation task is [#443](https://github.com/jermur788/CCF/issues/443) (blocked pending guarded user Editor save-integrity and lease-release receipt), with #438 rack-mark association as its initial safety diagnostic. Objective 2 **docs-only revision may run concurrently**, but neither the pending #442 corrections nor #425 draft review authorise **Objective 2 C#/Unity/acceptance/active test**. No source writer overlap and no second Unity lane.

**Next:** assign **ONE bounded docs-only #441 successor revision** to original GPT-6 author or a capable documentation specialist on its own revision branch. Produce corrected research and all four DRAFT files and a new handoff; initial review should use #442 §5 as a checklist and avoid redoing already confirmed facts. Then **independent non-author delta re-review**. Only after positive review can #425/user decide OPEN-1..6, amend canonical authority and approve the four-file spec if ready; #424 then routes future Objective 2 implementation **after** Objective 1's safe source handoff and owner switch.

**What passed:** #441 docs-only handoff existence, independent reviewer registration/handoff, GitHub diff single-file review scope, accurate confirmed code/evidence facts as reported. **Failed:** DRAFT quality gate: two blockers, six majors and seven minors, with no feature tests. **Meaning:** NEEDS CHANGES, not feature rejection, not product approval. **User action now:** none for Objective 2 research revision; protected launcher final receipt still required before any Objective 1 local Unity work. **Dashboard/Drive sync:** NOT VERIFIED.
