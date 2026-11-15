# Overall Manager — MULTI-010 independent LAB-06 review disposition and P1 product gate

**Date:** 15 November 2026  
**Manager decision:** **APPROVED AS P1 EVIDENCE PLAN — CONDITIONAL OPTION A**, for **study quality and a possible DRAFT SPEC only**.  
**Absolutely not approved:** LAB-07 growth implementation; a P1 spec; any Unity model parity; any Irish field/predictive accuracy; calibration/replacement of CCF growth; real marteloscope import; Scenario One integration; Windows Unity game build or change to the complete three-species first external demo.  
**Authorisation to start a P1 DRAFT:** **AWAITING EXPLICIT USER PRODUCT GO/NO-GO**, not inferred from review approval.  
**Review claim:** [#525](https://github.com/jermur788/CCF/issues/525) HANDED-OFF/CLOSED.  
**Source study claim:** [#522](https://github.com/jermur788/CCF/issues/522) HANDED-OFF/CLOSED.  
**Qualified P0:** standalone synthetic flat Sitka, no biological growth, including [Windows/Ubuntu WSL2 qualification #514](https://github.com/jermur788/CCF/issues/514).

## 1. Independent handoff and Manager custody

- [MULTI-009 Codex LAB-06 source/evidence feasibility](https://github.com/jermur788/CCF/blob/research/multi-009-sitka-p1-source-evidence-feasibility-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-009-LAB-06-SITKA-P1-SOURCE-AND-EVIDENCE-FEASIBILITY-HANDOFF-20261114.md), original OpenAI GPT-6.1 Sol source audit. Study head \`2212544e90fc5c48195f708c525e58be8a8cd936\`, based on main \`fc18190b976f1363f1b5d1b0881810db2f8c6e39\`. Manager previously inspected one-added-Markdown diff, 42,803-char GitHub readback, execution registration before work.
- [MULTI-010 independent Claude Opus 5.5 High forestry + method review](https://github.com/jermur788/CCF/blob/review/multi-010-lab06-sitka-p1-forestry-method-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-010-LAB-06-SITKA-P1-INDEPENDENT-FORESTRY-AND-METHOD-REVIEW-20261114.md), review branch \`review/multi-010-lab06-sitka-p1-forestry-method-20261114-v1\` final \`3d9699dc9665dd853bda88dbf6b1f1bd5e94530e\`. Independent non-author and separate Linux laptop worktree, RUNNING registered before inspection, HANDED-OFF and CLOSED. Manager fetched the entire report and compared the review branch with source study: **one added report Markdown only, no source/spec/game/board change**.
- Independent reviewer confirmed about **35 CCF source file/line mappings** on main source, source model-version routing, current save version 24 vs stale historical harness version 23; independently recalculated **all nine** Python arithmetic vectors (also binary32-like operations) and ran historical GROWFOR reconstruction suites **24/24 and 18/18** on a separate scratch copy. The **nine reproduced numbers are equations reconstructed in Python, not running Unity parity**; the remaining three vectors are source-derived only and two unavailable. No Unity verification/harness or P1 adapter execution. Review: **0 BLOCKING, 0 MAJOR, 8 MINOR**, with seven DRAFT contract decisions.
- Correct source distinction: current new CCF game selects growth model **2**; a closed, pure-Sitka, Reference-demand ecology population reaches the model-1-equivalent **adult density route** under model 2. This is a scoped source inference, not measured game runtime equivalence. Legacy growth 0 and frozen Reference Future must remain compatible. Existing P0 integer-mm/no-growth/schema-v1 values and hashes must not be reinterpreted.

**Manager verdict:** The independent review is fit for **evidence-plan approval** only. **Conditional Option A (faithfully reconstruct existing CCF adult Sitka rules in a separate versioned experimental layer)** is the lowest-assumption P1 research direction. Experimental Option B requires new per-ID allocation assumptions unsupported by present longitudinal spatial data. **NO-GO for coding** until new P1 spec, numerical/runtime parity oracle and separate approval. The owner may choose no P1 at all to prioritise Scenario One.

## 2. Limited primary-paper spot check (Manager; not a substitute for independent review)

The independent reviewer explicitly lacked the original GROWFOR/Broad–Lynch source PDFs on that laptop. **This limitation remains true of MULTI-010.** The Game Dev conversation attachments in the Manager environment include the original \`2006_VOL63_NO1&2-ss.pdf\`, 168-page *Irish Forestry* 2006 volume. The Manager used local PDF rendering and visually inspected targeted PDF pages 73, 76, 89, 100 and 112, plus extracted text for targeted orientation.

**Directly checked in that primary volume:**
- Broad & Lynch (2006a), *Growth models for Sitka spruce in Ireland*, starts **PDF p73 / printed journal p53**. On **PDF p76 / printed p56**, the growth state vector is explicitly \`x=(B,N,H)\`; **site index S is top height at age 30**. The paper describes separate thinned/unthinned stand-level empirical systems, not an individual-tree model.
- On **PDF p89 / printed p69**, the article describes **113 non-research observations used as calibration data while fitting the stand volume equation**, with iterative weights. This backs reviewer m-7: refer precisely to **volume-model calibration**, not all growth dynamics. Article context includes volume to 7 cm small-end diameter.
- Broad & Lynch (2006b), *Panel data validation using cross-sectional methods*, starts **PDF p100 / printed p80**. The abstract distinguishes research panel sampling/independence issues from non-research **stand-volume** cross-sectional comparison. The 2006b paper's **PDF p108 / printed p88** confirms **70 thinned and 43 unthinned** non-research volume plots.

**Still NOT verified here:** full equation/appendix transcription and numeric fits, GROWFOR user manual active-program version/coefficient identity, 2013 comparison or 2017 NFI original PDFs, every reported page claim or exact top-height inventory selection rule, current executable parity and Irish longitudinal holdout. The canonical [GROWFOR Technical Evidence Extraction V1B](https://github.com/jermur788/CCF/blob/main/Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md) remains research/evidence support rather than a model-adoption decision. These limited paper checks are Manager spot checks and do **not** retroactively change the independent review's “primaries NOT VERIFIED” execution label.

**Science boundary:** Broad–Lynch/GROWFOR provides only a **stand-level model reference for B, N and H** in its evidenced even-aged pure Sitka conditions. It is **not** Irish validation of individual-tree Hegyi response, crown release, cause-specific mortality, mixed/irregular CCF, regeneration or the game's density calibrations. 2006b is source-data validation, not an untouched longitudinal validation of the final model. The 113 stands' reuse in volume fitting is not evidence of independent full-model predictive validation.

## 3. All eight independent MINOR findings carried to any future P1 DRAFT

The future draft, if authorised, must explicitly dispose each; omission is NEEDS CHANGES:
- **m-1:** height getter is stage-dependent; restrict initial parity to **Mature** equivalents, not early-life stages.
- **m-2:** site productivity = 1 is **current-source behaviour**, not an optional experimentally sampled site regime.
- **m-3:** P0 dead trees have no cause/year; define pre-inventory-dead mapping and do not emit new annual death events for them.
- **m-4:** decide authoritative numeric type, float32 rounding and exact roundtrip vs float64 tolerance; persisted Unity values are float32, while some internal methods use double.
- **m-5:** source stand area is square and coordinate origin centred; use exact matching square fixtures for parity, declare polygon-area/coordinate transformation a **separate adapter extension**, not a claim of source byte parity.
- **m-6:** the mortality roll is keyed by **RNG model version**, ecological year and stable ID; do not conflate RNG version with growth model version.
- **m-7:** specify **volume-model** calibration for the 113 independent-of-research-panel observations and disambiguate PDF-relative vs printed article pages (partially checked in primary 2006 volume).
- **m-8:** P0 demo/performance fixtures differ materially in below-action-threshold living stems and density-onset proximity; new parity fixtures must explicitly include low/high density, source-matched area, stage/site and SI-policy anchors.

Do not treat these as retroactive implementation changes or authorise the stale Sitka version23 harness to be changed without a separate owner/review packet.

## 4. Seven required spec decisions (UNRESOLVED until separate approval)

**C-1 P1 identity:** distinct P1 \`format\` and schema, references immutable P0 fixture hash and snapshot, P0 source/save/events byte-identical.

**C-2 precision:** select source-faithful **binary32** (recommended for exact CCF parity) vs explicitly toleranced binary64, with exact roundtrip, rounding, mm/cm/m conversions and discrete-event flip policy.

**C-3 population:** **all living stems** in CI/RD/growth for CCF parity (recommended) vs actionable adult-threshold only; preserve separately named P0 eligible metrics.

**C-4 initial conditions:** Mature-only, crown estimation labelled assumption, source productivity=1, pre-inventory dead mapping, growth-version plus RNG-version/seed/year, closed no-recruitment/no-storm boundary.

**C-5 top-height and site:** separate CCF Class III height from Broad–Lynch SI30, declare initial SI for synthetic stands, select unverified top-height inventory convention and explicit domain flags.

**C-6 oracle/ownership:** approve an **independent current-source runtime-oracle capture route** (narrow Unity/test lane or code-isolated equivalent), fix stale save-schema assertion under separate approval, define exact parity/vector test ownership before changing lab model source.

**C-7 labels:** every future P1 UI/export/report must state **“CCF adult rules, closed pure-Sitka population; NOT an Irish field forecast.”** Broad–Lynch 2006 comparison is model-to-model research context, not accuracy validation.

These are **candidate choices to draft/review, not accepted product defaults**. The user can approve the high-level research purpose without deciding arbitrary numeric tolerances now; details must be independently specified and approved before LAB-07 code.

## 5. Product go/no-go; next task can be prepared but NOT started

**One owner-level gate remains.** Ask the user whether they approve **drafting only** a separate four-file P1 specification for **Option A**, deliberately **adult-only, closed, pure Sitka, no recruitment, no growth-model substitution, no Irish forecast claims**, without interfering with Scenario One's complete-loop three-species first external demo. Separate P1 files/state/save version must leave accepted P0 untouched.

- **YES:** dispatch a docs-only four-file **DRAFT** specification as a new claimed task, explicitly carrying m-1…m-8, C-1…C-7, staged P1-01 oracle → P1-02 bounded adapter → P1-03 separate ledger/save/replay → P1-04 stand B/N/H comparator → P1-05 P1 UI/negative regression; new independent spec review, Manager and user approval gates. No code before APPROVED P1 spec and source parity plan.
- **NO/DEFER:** keep P0 frozen and independently usable on Windows/Ubuntu WSL2; no loss of research evidence. Continue prioritising the authoritative full Scenario One complete-loop three-species demo.
- **If the user requires predictive Irish growth accuracy instead of a learning comparator:** NO-GO on this P1 route until a separately licensed, representative multi-year measured spatial dataset, empirical method and untouched field holdout are established. Existing GBIF raw-data [#479](https://github.com/jermur788/CCF/issues/479) is unverified.

**No P1 spec author, code worker, runtime model, physical worktree or task claim is yet authorised by this review.** Formal Manager decision is confined to **APPROVED AS P1 EVIDENCE PLAN**. Dashboard and Drive mirror sync: **NOT VERIFIED**; canonical Decision Log/current milestone and live board were not edited. First external Scenario One and personal playtest/Windows Unity build gates unchanged.
