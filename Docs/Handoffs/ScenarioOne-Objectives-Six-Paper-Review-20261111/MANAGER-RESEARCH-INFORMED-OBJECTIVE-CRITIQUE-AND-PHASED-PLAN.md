# Scenario One — Six-paper research critique of objectives and phased plan

**Status:** PROPOSAL / RESEARCH-BASED MANAGEMENT REVIEW — NOT APPROVED IMPLEMENTATION  
**Prepared:** 2026-11-11  
**Owner:** Overall Manager  
**Scope:** Scenario One, Enlarged80 reference game, complete-loop three-species demo  
**Audience:** product owner, spec author, independent reviewer, eventual local implementer  
**Action now:** inspect and decide scope; no implementation, Unity qualification or thresholds changed by this document.

## Executive verdict

**Keep the accepted complete-loop Scenario One and its current numerical completion thresholds until a focused audit establishes a material false-positive or misleading outcome.** Improve the teaching and review of individual-tree judgement within already-approved Rack V2, MFM V1, S1-C and S1-D. Do not allow six foreign/general research papers to expand the pre-demo biological model.

The strongest direct evidence concerns the *difficulty of choosing crop-tree competitors* (P1) and *non-unique but defensible marking choices* (P2). The lowest-risk quantitative candidate is a **read-only** measure of spatial species mingling and diameter differentiation (P3). The canopy (P4), Bayesian (P5) and regeneration taxonomy (P6) papers are valuable **research/educational** references, not validated replacement Irish parameters.

**More important than a new coefficient:** an inspection of current code finds semantic limits in the objective evaluator. The scenario can certify a collection of snapshot conditions and management records but does not itself certify the ecological mechanism or quality of every choice. Some labels imply a stronger result than the underlying field measures.

## Authority and scope reviewed

- Accepted product intent: [Game Brief](../../../../Docs/Project/game-brief.md); [Decision Log](../../../../Docs/Project/decision-log.md), particularly D-003, D-004, D-010, D-042, D-057, D-059–D-065, D-066, D-071, D-072; [Current Milestone](../../../../Docs/Project/current-milestone.md).
- Implementation inspected on GitHub default branch: [ScenarioOneObjectives.cs](../../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs); [ScenarioOneManager.cs](../../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs); [ScenarioOneCompletionRules.cs](../../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs); [ScenarioOneDefinition.cs](../../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs); [ScenarioOne.asset](../../../../Assets/ForestPrototype/ScenarioOne/ScenarioOne.asset); [ForestTree.cs](../../../../Assets/ForestPrototype/ForestTree.cs). This is a source review, not Unity/runtime qualification.
- Existing accepted specs: [S1-C History](../../../../specs/scenario-one-review-history-v1/spec.md) (recorded facts vs explanations); [S1-D Repeat Interventions](../../../../specs/scenario-one-repeated-interventions-v1/spec.md); [Multi-Scale Forestry Management V1](../../../../specs/multiscale-forestry-management-v1/spec.md); [Rack V2](../../../../specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md).
- Research papers supplied by the product owner, located in project sources: P1 Pommerening, Maleki & Haufe (2021), individual-based management; P2 Pommerening et al. (2018), rating experiments; P3 Pommerening (2002), structural metrics; P4 Pommerening et al. (2021), CanopyShotNoise; P5 Myllymäki et al. (2024), Bayesian projections; P6 Pommerening, Szmyt & Duchiron (2024), regeneration methods.
- Full-paper crosswalk and extraction: user-supplied **CCF Six Complete Forestry Research Papers** and **CCF Six Papers Relevance Evidence Extraction**. Citation convention below is **P#:PDF-page** as explained in that extraction.

**Not freshly checked:** local game/Editor state, player experience, actual complete-loop accessibility on the newest personal UI branch, root machine ownership, current Unity activity or scenario metrics from a newly run simulation. The published milestone and coordination file include evolving separate work and possible local occupancy. These observations are proposals, not pass/fail evidence for a candidate build.

## What the current completion function does

The source-level logic is effectively:

C(y) =
    (y >= 25)
    AND (living original-species Sitka >= 240 on Enlarged80)
    AND (mean canopy >= 0.35)
    AND (occupied regeneration cells >= 12)
    AND (fallen deadwood >= 0.08 m3)
    AND (a succeeded FellTree order/event exists)
    AND (a succeeded Oak plant order/event exists AND Oak is presently represented)
    AND (a succeeded Beech plant order/event exists AND Beech is presently represented)
    AND (at least two distinct resolved management-plan years with a recorded review acknowledged between them).

The last condition is enforced by **ScenarioOneCompletionRules.HasReviewedTwoCycleSequence** in **ScenarioOneManager.EvaluateProgress**. It is not one of the eight read-only objective rows. The manager evaluates on year progression; cash exhaustion and the Year-100 limit are separate failure paths. The exact logic of each source predicate matters more than the UI label.

**Area calibration:** Enlarged80 is 80 x 80 m = 0.64 ha, with 256 ecology cells at 5 m resolution. For reference, 12/256 = **4.6875%** of cells; 0.08/0.64 = **0.125 m3/ha** of deadwood; 240/0.64 = **375 living original-species stems/ha**. These are arithmetic translations of GAMEPLAY CALIBRATION, **not biological minimums**.

**Century Review:** Year 100 is an aspirational comparison for Enlarged80, not an exact forecast, optimum or independent completion rule. Its current targets are 480 original-species stems, 40 presence units per each planted species, 48 regenerating cells, 2.0 m3 deadwood and mean canopy 0.65. Presence units there combine living trees, occupied regeneration cells and planted juveniles; do not call them 40 mature trees. The genuine frozen Reference Future belongs to Legacy40 and remains immutable.

## Source-level objective critique — what each check proves and fails to prove

### A. Year-25 elapsed time — KEEP

**Proves:** sufficient simulation years elapsed. **Does not prove:** any steady management cadence, regeneration persistence, or learning. This is appropriately a pacing gate, provided the history and two-cycle work are understandable. Do not replace with a fixed intervention interval; P1's 5–10-year horizon is not an Irish mandatory interval.

**Action:** keep; review idle-year explanation and how/why player decides *not* to intervene. Avoid rewards for arbitrary extra work.

### B. 'Retain original canopy trees' — VERIFY SEMANTICS; CONDITIONAL CORRECTION

**Source inspection:** the objective looks up the **original species ID** and takes that species' snapshot \`livingTrees\`. Snapshot construction aggregates every non-stump active tree by species, without a test against original-cohort tree identity. Dead trees are disabled by \`ForestTree.ApplyMortality\`, so the inspected active-only snapshot path is not inherently counting that dead-tree representation.

**What it proves:** a count of living **Sitka species** trees. **What the label appears to promise:** retention of the *original planted cohort*. These are not necessarily identical if new Sitka trees promote into individual stems. At minimum this is a terminology mismatch; whether it produces an achievable false-positive by Year 25 remains **UNVERIFIED**.

**Action:** design a provenance-specific fixture in which some original Sitka die and new Sitka promote, then prove the evaluator's count against original-ID count. **Default fix, if the accepted intent is original-species retention:** change label and explanation to 'Retain Sitka canopy trees' without changing eligibility. **Alternative only with product approval:** measure provenance-specific original trees and deliberately rebaseline Year-25 goals/fixtures. Do not silently choose the latter.

### C. 'Keep continuous canopy' — KEEP THRESHOLD; IMPROVE HONESTY

**Source inspection:** one current snapshot's area-mean cell canopy is compared against 0.35.

**Proves:** current mean canopy threshold. **Does not prove:** uninterrupted forest cover at every previous year, canopy spatial connectivity, absence of an exposed large clearing, or effective shelter of specific planting locations. A mathematically high spatial mean can coexist with large bare patches.

**Action:** retain current number for compatibility. Explain it as **mean canopy retained at review**, not proof of historic continuity. If gameplay is misleading, run offline per-year minimum/extent diagnostics before considering a continuity/pattern rule. P6 provides a conceptual shelter/edge/open framework but no Irish gap-width or optimum.

### D. 'Maintain regenerating cells' — KEEP AS PROXY; EXPLAIN WHAT IS COUNTED

**Source inspection:** counts cells with \`HasRegeneration\` plus unique cells containing live exact planted juveniles when those cells are otherwise unoccupied. A cell is a presence/occupancy unit; not a fixed number of independently established stems or species-specific 1.3 m ingrowth.

**Proves:** at least 12 currently occupied grid cells. **Does not prove:** established saplings survive, several cohorts are present, Oak/Beech are recruiting naturally, resistance to browsing, or a future forest generation. A recent pulse of weak recruitment could meet the number and disappear later. P5's 1.3 m ingrowth is specifically **not** equivalent to the game's germination/occupancy.

**Action:** keep target. Surface current cause-aware regeneration evidence if already recorded: species, origin, size/stage, browsing/protection and previous-vs-current occupancy. Do not add an unvalidated Irish seedling survival threshold. Explore a read-only one-/multi-year persistence signal after S1-C stability is qualified.

### E. Fallen deadwood 0.08 m3 — KEEP TARGET; STRENGTHEN INTERPRETATION

**Proves:** accumulated surviving fallen-deadwood record volume. **Does not prove:** deliberate retention by the player, deadwood spatial diversity, decay diversity, habitat occupancy or ecological sufficiency. At 0.125 m3/ha the completion threshold is plainly a very small game teaching check, **not** an empirical habitat prescription.

**Action:** keep. Distinguish naturally generated/self-thinning deadwood from intentionally retained felling outcomes in educational feedback where provenance exists. Avoid implying either quantity is a measured biodiversity score.

### F. 'Carry out a commissioned thinning' — CONDITIONAL SEMANTIC REVIEW

**Source inspection:** objective is satisfied by **any** succeeded \`FellTree\` resolution or completed \`FellTree\` order; no explicit crop-tree release rationale, retained target or treatment-quality condition is in that predicate. In principle a rack-clearance felling could satisfy the named thinning objective, if represented by that work type; prove this with a targeted fixture before claiming player-visible exploit.

**Action:** make 'one successful felling operation' versus 'one demonstrated selective thinning' distinct in narrative. Do **not** change completion automatically: MFM sample teaching and two-cycle history already aim to teach the missing judgement. If demonstrably possible to finish without a meaningful selective-treatment example, request a bounded specification decision.

### G. Oak and Beech introduction — KEEP; VERIFY PROVENANCE & SUCCESS LANGUAGE

**Source inspection:** for each purchased species, the objective needs a succeeded planting record and **present > 0**, where present is the sum of living trees, regeneration cells and planted juveniles of the species. The current presence calculation is species-level, **not necessarily the survival of the specific planted individuals linked to that earlier order**.

**Proves:** planting happened historically and the species is represented now. **Does not prove:** the particular planted individuals established, recruit survival, adequate growing space or long-term persistence. Compare the Year-100 broadleaf 'presence' units with mixed units rather than calling all of them trees.

**Action:** retain for now, but call the objective 'Plant and maintain Oak/Beech presence' or equivalent if wording is changed under the existing UX authority. Test the logical edge case where planted stock dies but a species remains via another origin; if unacceptable, propose separately an origin-aware condition with historic-data and save implications.

### H. Two reviewed cycles — KEEP; CURRENTLY A STRENGTH

**Source inspection:** the completion function already requires two distinct resolved plan years and an acknowledged annual review with valid event ordering between. This is stronger than the numerical objective rows alone.

**Proves:** a reviewed repeated-management sequence occurred. **Does not prove:** that the second decision responded intelligently to the first or that both interventions were silviculturally justified.

**Action:** retain exactly. Use S1-C FR-6/FR-7 and S1-D previous-vs-proposed views to make the difference observable and explainable. Do not add a new scored proficiency/test subsystem before the demo.

## Modelled critique: plausible false-positive and false-negative pathways

These are **logic-level scenarios**, not reported Unity run results. Reachability from a real seed/starting forest, cash and event order must be tested before a bug is asserted.

| Counterfactual | Current rules could say | Why interpretation may mislead | Cheapest discriminating test |
|---|---|---|---|
| F1: Retained original Sitka decline; promoted new Sitka compensate | 'Retain original canopy trees' achieved | Source counts species, not proven original cohort | Synthetic original-ID/promoted-Sitka snapshot comparison |
| F2: Most canopy lost earlier, but recovered over a sufficient area at review | 'Keep continuous canopy' achieved | Mean at one year is not a temporal-continuity record | Two histories with same end canopy but different low-canopy years |
| F3: A pulse of low-stage regeneration occupies 12 cells for one year | Regeneration achieved | Occupancy is not survival/persistence | Revisit same cell/age-band histories after later time steps |
| F4: Any completed FellTree is clearance, not crop-tree release | Thinning objective achieved | Event type lacks silvicultural intent | Rack-only FellTree classification fixture |
| F5: One Oak planting dies while a different Oak population remains | Oak establishment achieved | Species presence is not planted-stock survival | Origin-aware planted-ID fixture |
| F6: Two reviewed interventions take place, but second does not address first-cycle outcome | Two-cycle chronology passed | Process presence is not evidence of responsive judgement | Two decision-path comparisons with recorded outcomes |
| F7: Valid alternative competitor choice differs from tutorial example | Rigid tree-ID grader might reject it | P1/P2 show human selection disagreement | Compare local constraints/effects rather than exact IDs |
| F8: Broadleaves occur in one isolated block | Species richness increases | Local species mixing may still be low | Offline nearest-neighbour mingling by P3 |

**Diagnostic point:** The boolean completion model is **necessary as a simple gameplay milestone**, but should not be presented as a sufficient scientific test of ecological health or professional practice. The research argues for richer **explanations** of choices and consequences, not for replacing one scalar score with another.

## Evidence-to-design translation

| Paper | Strongest supported observation or method | Appropriate CCF use | Transfer caution |
|---|---|---|---|
| **P1:1–5,8–11** | Future crop-tree-focused local thinning; more agreement on frame trees than on their competitors | Explicit retained-tree → competing-neighbour teaching and later release review | UK training evidence; 50/60/80/100/150 trees/ha and h/d 40–80 are not Irish Scenario One targets |
| **P2:1,10–17** | 36 British marteloscope experiments; low to fair tree-ID agreement, especially crown thinning | Assess treatment intent, retained trees and consequences; explain non-unique valid choices | Agreement statistic ≠ forestry quality score |
| **P3:3–8,17** | Neighbour-based species mingling; DBH differentiation; boundary biases | **Offline**, read-only spatial structure proof first; optional later review panel | Neither metric directly measures habitat, biodiversity or forest health |
| **P4:5–14,23–25** | Crown interaction fields and simulated crown plasticity | Research-only sensitivity comparison of crown centre/footprint vs existing field | Swiss Beech fit and chosen simulation parameters ≠ Irish Oak/Beech constants; no supported root-network mechanic |
| **P5:3–12** | Spatial ingrowth/growth/mortality architecture and Bayesian predictive uncertainty | Offline competition model benchmark; honest long-range forecast caveats | Finnish Norway spruce; 49 ingrowth/ha/year at 1.3 m ≠ Irish germination; Finnish basal-area cutting thresholds not importable |
| **P6:2–5,6–11** | Shelter, margin, open principles; staged regeneration-system components | Explain conditions around a gap and why intervention outcomes differ | Regeneration *strip* ≠ machine extraction rack; no Irish gap size or establishment probability supplied |

## Options assessed

**Option A — New biological models and revised numerical targets before the demo:** high integration, calibration, persistence and regression risk, weak Irish parameter evidence, new full Tier-3 demands. **REJECT as current recommendation.**

**Option B — Keep current completion logic; improve explanations, review and teach-back within the existing S1-C/S1-D/MFM pathways:** strongest alignment with the papers, acceptable change boundary, minimal compatibility risk if copy/query-only. **RECOMMENDED.**

**Option C — Keep current logic but correct proven misleading labels or material objective bugs through a separately approved bounded scope:** sensible contingency. Start with offline counterexamples; only change semantics if genuine behavior is wrong and product owner accepts the implications. **RECOMMENDED CONDITIONALLY.**

**Option D — New weighted 'forestry quality' or structural biodiversity score:** risks false precision, prescriptive marking, contradictory trade-offs, more arbitrary calibration and feature creep. **DEFER / NOT RECOMMENDED for Scenario One V1.**

## Ordered work plan

### Phase 0 — Research/semantics audit (can begin without changing production)

**Purpose:** establish which differences are merely wording and which are real gameplay false-positives.

1. Pin current source revision and accepted objective/experience specs; read-only inspect eight evaluator predicates, snapshot construction, event/plan logic and present UI text.
2. Run Tier-1 offline synthetic/fixture checks F1–F8, with exact expected and observed results. Do not weaken current protected anchors.
3. Create a current-player-facing text inventory: objective title, success condition, explanation, review historical source and what is *not* known.
4. Compare that inventory with S1-C's L1–L4 provenance and MFM's 'Why proposed?' reason vocabulary. Identify duplications rather than creating a third explanation engine.
5. Return one verdict per issue: **WORDING ONLY**, **CORRECT AS DESIGNED**, **POTENTIAL LOGIC DEFECT** (requires bounded repro), **NEW PRODUCT FEATURE** (defer). Include executable fixtures and cite the published evidence.

**Exit:** signed Manager review of source-only evidence; no Unity/Windows or spec rewrite by default. Difficulty **Medium**, because repo inspection is straightforward but provenance/fixture semantics require care. Best local provider only if read-only source/fixture checks cannot be done via GitHub/doc review; do not dispatch now while product decision is pending.

### Phase 1 — Already-approved learning improvements before the first demo, subject to current spec/UX gate

Focus on minimum change:

- **Sample teaching:** separate retained Crop Tree quality from which neighbour is an actual competitor. Offer short inspectable reasons with declared current-state inputs, not inaccessible future outcomes.
- **Alternative choices:** avoid 'wrong tree ID' grading; prefer constraints, crown release and recorded trade-off feedback. Where quantitative suitability is unsupported, use human-readable advisory copy rather than falsely authoritative point scores.
- **MFM:** re-use existing candidate/confirmed/covered/excluded/proposed/approved/executed distinctions; one 3D spot-check remains mandatory. Do not convert proposal state to Fell marks.
- **Review/history:** show previous work, current observed consequence and derived comparison in their already-approved distinct provenance layers. Where causal attribution lacks recorded evidence, say 'observed after' rather than 'caused by'.
- **Regeneration:** a bounded explanatory choice around shelter/edge/open conditions can accompany existing light, browse and local competition facts. Never equate an extraction rack to the 'strip system' in P6.
- **Objectives:** clarify source-supported meanings (mean canopy, occupancy vs established cohort, species presence vs original cohort, deadwood volume). Do not alter target values.

**Exit:** one coherent guided first sample and at least two distinct later reviewed interventions, with credible reasoning, current/previous facts and no invented causal history. Scope should be constrained to changes actually missing from current implementation, not re-implementation of approved S1-C/S1-D/MFM. Difficulty **Medium** if wording/UI only; **High** if history, state or schema changes are needed, in which case move to Phase 2 and require a new approved spec.

### Phase 2 — Conditional objective correction (separate product approval only)

Trigger only when Phase 0 demonstrates materially misleading or exploitable conditions the user wishes to correct before external testing.

- **First choice:** precise UI labels/copy; does not change old saves, reference, objective achievements or thresholds.
- **Second choice:** minimal logic revision restricted to the identified predicate (e.g. species-vs-origin), accompanied by a written rebaseline and migration implications.
- **Do not** combine target increases, new species metrics and new explanatory UI in one ambiguous 'improve objectives' change.
- Any predicate/target revision must inspect historic objectiveAchievements, saved reviews and old-save semantics; protect Legacy40 and Reference Future; obtain an approved four-part spec **before** implementation.
- Failure to construct a real scenario reaching the supposed false-positive means **no logic change** on that hypothesis.

**Exit:** explicitly accepted design decision and proportionate verifications on the exact integrated candidate. Difficulty **High** for logic/persistence; not an automatic pre-demo prerequisite.

### Phase 3 — Post-demo structural measures proof (offline and read-only first)

- For each tree i, compute species mingling \`M_i = different-species neighbours / n\` with n=3 (P3 example).
- For a neighbouring DBH pair (i,j), compute \`T_ij = 1 - min(DBH_i,DBH_j)/max(DBH_i,DBH_j)\`.
- Use valid tree positions, DBH and species labels; explicitly handle missing neighbours at the real estate edge, equal distances/ties, zero/invalid DBH, living/dead filters and world geometry.
- Compare the **same geographic sample** before/after intervention; do not compare a sample to the entire property as if both were equal areas.
- Verify deterministic save/load readout, no ecology mutation, and separate treatment of Legacy40 and Enlarged80.
- Do **not** claim 'biodiversity increased 20%' because mingling increased.

**Exit:** a small standalone research chart/CSV or source report; user decides if a short Annual/Century Review visual is worth its complexity. Difficulty **Low–Medium** for offline formula; higher for production UI.

### Phase 4 — Explicitly deferred research

1. Replace Hegyi with Finnish RBA competition: **defer**; P5 exploratory variants did not establish superiority; benchmark offline if needed.
2. Refit Swiss Beech canopy dynamics/annual crown-centre movement into Irish species: **defer**; missing validation, visuals/ecology/save and performance risk.
3. Simulate crown 'superorganism' or mycorrhizal networks from P4: **do not adopt** on this evidence.
4. Full Bayesian future forests and 95% credible envelopes: **defer**; no calibrated Irish posterior or objective player need. Later deterministic sensitivity comparisons, honestly named, may suffice.
5. Formal multi-system shelterwood/strip/group regeneration choice UI: **defer** until a future scenario, since current player chooses concrete treatments; extraction racks are infrastructure not regeneration strips.
6. Increase numeric thresholds, introduce regeneration survival durations, define 'optimal' spatial mixing or add a single forestry-quality score: **defer** until the exact baseline and player outcome problem are demonstrated.

## Modelled prioritisation / selection rationale

Qualitative decision matrix, **Manager judgement**, not measured effort or probability. Scale: Benefit High/Medium/Low, Cost Low/Medium/High, Risk Low/Medium/High.

| Work | Educational benefit | Cost | Integration risk | Recommendation |
|---|---|---|---|---|
| Wording/provenance accuracy for objective meaning | High | Low | Low | Phase 0 + conditional Phase 1 |
| Retained-tree and competitor reasons | High | Low–Medium | Medium | Phase 1, check existing MFM first |
| Previous → proposed → observed treatment feedback | High | Medium | Medium | Existing S1-C/S1-D scope; no parallel system |
| Verify questionable completion predicate semantics | High reliability | Medium | Low if read-only | Phase 0 |
| Modify completion predicate logic | Conditional | Medium–High | High | Phase 2 only if demonstrated + approved |
| Mingling / DBH differentiation offline | Medium | Low–Medium | Low | Phase 3 |
| Rich structural/ecological scoreboard | Uncertain | High | High | Defer |
| Crown physics, Bayesian or external growth model import | Uncertain | High | High | Defer |
| Fixed scientifically 'correct' competitor marking IDs | Negative: teaches false uniqueness | Medium | High pedagogical | Reject |

## Verification and stop rules

**Tier 1 — FAST:**
- Compare objective fields at exact current code, including origins and thresholds.
- F1–F8 fixture facts; hand-calculated M and T on a four-tree synthetic plot.
- Regeneration occupied cell count includes exact planted juvenile cells as defined, not number of survived natural stems.
- One alternate but defensible competitor selection path is not rejected solely on ID difference.
- No state mutation from read-only research code, no change to frozen reference/hash fixtures.

**Tier 2 — FEATURE QUALIFICATION (only if UI/feature is changed):**
- One complete sample-retained-tree → competitor → Work Plan → annual outcome → review route.
- A different defensible marking path with honest feedback, still subject to same safeguards.
- At least two distinct later intervention years with acknowledged history and re-entry after save/load.
- 1280x720 and 1920x1080 views, readable grayscale, real relevant input as applicable.
- Old saves, Legacy40, Enlarged80, Reference Future, selected MFM candidate/proposal semantics, costs and history provenance unchanged except expressly approved change.

**Tier 3 — FULL INTEGRATED QUALIFICATION:** only the existing complete Scenario One release candidate after all accepted Rack/Parity/S1-C/S1-D/MFM gates, not for every small copy or offline metric. The user personally playtests the exact candidate before any authorised Windows build.

**Stop** if an idea needs new authoritative biology; a historical claim cannot be supported by recorded facts; a UI copy claims numeric Irish calibration; a change alters protected model/reference data; MFM's proposed marks become committed prematurely; or a new scope collides with a live owner. Return to Manager/product owner for a separate decision, not an opportunistic code change.

## Specific questions requiring product-owner choice

1. When the UI says **'Retain original canopy trees'**, is intended meaning **living Sitka species** or **the initial plantation cohort**? Recommendation: retain species-based behavior and clarify the label unless actual gameplay proves the distinction must be central.
2. Should Year-25 completion remain an explicitly **educational milestone**, not certification of professional CCF competence or ecological optimality? Recommendation: **yes**.
3. Should the next release include any **additional compulsory teaching step** beyond current approved S1-C/S1-D/MFM? Recommendation: **no**; use existing moments and require evidence of an actual learning deficiency before expanding.
4. Does the user want an offline, read-only structural-metrics demonstration after the current demo? Recommendation: **yes, later**, not a new score or gate.

## What changes now, and what does not

**This document changes:** the research interpretation and recommended backlog/order. It gives precise audit candidates and testable next steps.

**This document does not change:** current milestone, design decisions, approved specs, objective numbers or completion logic; code, Unity scene, save schemas, compatibility, ecology; ownership/locks; any task claim; or user acceptance.

**Current next step:** Manager/product-owner disposition of this draft. Then, if accepted, publish a bounded Phase-0 audit packet and one claim in a dedicated lane; treat any later production work as a separate approved and owned task. No local machine activity, Unity or Windows build is authorised by this proposal.

**Dashboard:** no dashboard ingestion/readback has been performed or verified for this document.
