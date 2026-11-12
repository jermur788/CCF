# Handoff to Overall Development Manager #425 — eight-scenario direction and new woodland-management additions

**Date:** 12 November 2026  
**Prepared for:** [Overall Development Manager #425](https://github.com/jermur788/CCF/issues/425), strategic portfolio and product decisions only.  
**Scope:** Direct user discussion of eight Irish forest-management scenarios; refined three-way curriculum #428; Pommerening's simultaneously established nurse crops #430; coppicing and fruit/nut placement follow-on.  
**Status:** HANDOFF / REVIEW REQUESTED; **no new feature acceptance or final order**. The accepted eight-scenario **count** remains the decision; the curriculum's exact chapter order and these two new additions are design proposals. No Scenario One delivery change, no code/Unity save/spec work authorised.

## 1. What is accepted and must not be re-opened inadvertently

1. **D-078:** Eight **independent Irish forestry-management scenarios** are the accepted long-term Stage 1 direction, delivered as playable scenarios **sequentially**, with safely overlapping later-case research/design/approved disjoint work. Only player learning transfers between scenarios; no common money, forest or inventory across the eight. A future estate case may contain *its own* shared property budget across compartments. **Eight case count is chosen, order/teaching detail not yet chosen.**
2. **D-051 / D-052 / D-053:** Forestry-first; separately authored woodland properties; small private forest and farm-forest owners as main audience; custom sandbox later. Skills progress through diagnose → choose → work/hold → annual ecology → walk/review → justify/revise, not XP points.
3. **D-054:** Fruit/nut **production** is the **first Stage 2 expanded land-management slice**, **after** a mature Stage 1 forestry simulator. Existing fruit/nut-bearing trees or establishment-stage considerations are not equivalent to a full food-production simulator.
4. **Scenario One #424 boundary unchanged:** The first external demo is the **complete** existing Sitka/Oak/Beech plantation-transformation Scenario One. Five parallel Rack V2 routes, physical entrance marking, Work Plan/MFM commitment and repeated intervention history remain under Scenario One Delivery Manager #424. Protected Legacy40 and frozen Reference Future v1; no Windows build before personal playtest and explicit authorisation. Overall Manager must not send a parallel coder, touch local Unity locks or edit live `active-tasks.md` (sole coordination writer #424).
5. **Research is not product authority.** The source studies and new addenda propose gameplay; they cannot silently change Decision Log, Milestone, approved specs, starting species models or scenario order.

## 2. Where the curriculum discussion currently stands

The [original eight-case proposal](https://github.com/jermur788/CCF/blob/docs/independent-eight-irish-forest-scenarios-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/INDEPENDENT-EIGHT-IRISH-FOREST-MANAGEMENT-SCENARIOS-20261112.md) defined these **eight unchanged forest situations** in its provisional original order:

1. **Inherited Sitka Plantation** — existing reference / transformation to CCF.
2. **Working Mixed Forest** — sustaining already irregular productive CCF.
3. **Neglected Broadleaf Wood** — timber quality, competition, disease and habitat.
4. **New Farm Woodland** — establishment and site fit, including valid non-plant choices.
5. **Wet Woodland and River Valley** — soil, water and access constraints.
6. **Atlantic Oakwood** — regeneration, native habitat continuity and invasives.
7. **Exposed Upland Forest** — wind/stability, contractor safety and recovery.
8. **Complete Irish Forest Estate** — portfolio budget and prioritisation within one estate.

The original numbering **does not make this order accepted**. User subsequently demanded three **materially distinct complete progressions**, not just one sequence rearranged cosmetically; [completed research task #428](https://github.com/jermur788/CCF/issues/428) published a full comparison. Its **proposed** strongest choice is:

**B, forestry craft first:** Sitka → Working Mixed → Neglected Broadleaf → New Farm → Exposed Upland → Wet Valley → Atlantic Oakwood → Estate.

**Runner-up A, beginner owner:** Sitka → New Farm → Neglected Broadleaf → Wet Valley → Working Mixed → Atlantic Oakwood → Exposed Upland → Estate.

**C, ecology first:** Sitka → Wet Valley → Atlantic Oakwood → New Farm → Exposed Upland → Neglected Broadleaf → Working Mixed → Estate.

[Full three-progression comparison and feature dependency results](https://github.com/jermur788/CCF/blob/docs/eight-scenario-curriculum-refinement-result-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-CURRICULUM-DEPENDENCY-REFINEMENT-RESULT.md): pros/cons, seven transitions per option, learner journeys, case differentiation, minimum causal proofs, feature blockers, safe serial/parallel windows and source limits. #428 is **HANDED-OFF/CLOSED**, **not independently Claude-reviewed or product-approved**.

**Manager action:** Issue a formal **APPROVED / NEEDS CHANGES / REJECTED** research-quality assessment. Then create a *separate independent Claude critique packet* with **original eight study + #428 refinement + this handoff + evidence corpus**. Explicitly ask Claude to challenge whether B is too close to O1 early, whether A is better for novice owners, the real system costs, and whether the two additions below improve or overload the proposed chapter lessons. Preserve all eight cases whatever the critique's preferred ordering. Final choice returns to user for explicit acceptance.

## 3. User-highlighted Pommerening approach: simultaneous nurse-crop and target planting

**User clarification is precise and must be preserved:** In Pommerening's *Coppice, Carbon and Catastrophes* webinar, he describes one variant of creating a CCF stand **from bare land** by **planting fast-growing temporary shelter/nurse trees and slower-growing intended long-term crop trees AT THE SAME TIME**. The nurses are **not** intended to grow to full final crop size. They develop more quickly, progressively provide useful shelter, and should be *selectively thinned away* before they suppress the target crop. **This is one variation, not the entire definition of Instant New CCF.**

Do **not** mistake this for the separate method of planting the pioneer crop years earlier and underplanting a target crop later. Do **not** assume instant mature shelter on planting day. The positive effects depend on site/spacing, juvenile form, crown emergence and browse; nurses may grow too slowly to protect (e.g. reported Scots pine on heavy soils) or dominate target oak if left too long (reported larch/spruce). Teacher's core dilemma is **retain nurse cover vs release main crop at the correct local time**, observable over repeated annual walks/reviews and committed via Work Plan.

**Strong relevance:** O4 **New Farm Woodland** as an **establishment option**, alongside (1) pioneer-first then underplant; (2) direct long-term mixed planting without dedicated nurses; (3) natural regeneration or intentionally leave valuable open habitat unplanted. This must not become a ninth scenario or a mandatory universal recipe.

**Research status:** [Pommerening research handoff #430](https://github.com/jermur788/CCF/blob/docs/pommerening-instant-new-ccf-research-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/POMMERENING-INSTANT-NEW-CCF-BARE-LAND-RESEARCH-20261112.md) and [issue #430](https://github.com/jermur788/CCF/issues/430) (HANDED-OFF/CLOSED). Primary Irish COFORD *Nursing of oak in Ireland* (Guest & Huss, 2012) documents simultaneous oak plus Scots pine/larch/nurse spruce examples and opposite shelter/competition risks. Pommerening & Murphy (2004) and 2024 European simultaneous beech/pioneer research are corroborating **transfer/context**, not Irish fixed parameters. The **exact timestamp/spoken words from webinar were not independently transcribed**; rely on user's description for which variation he meant, not on unverified quotations.

**Not implemented:** bare land eligibility, intended crop versus temporary nurse purpose, scenario-specific tree planting groups, credible positive shelter effects, nurse-stem removal/release, new save/provenance and required protected tests. Existing juvenile planting/growth/browse/light alone does not certify a real shelter effect. Do not auto-award a growth multiplier. If later approved, requires four-file spec and proportional verification, separate from #424.

## 4. Coppicing and fruit/nut trees: placement without expanding the eight case count

**User asked explicitly if these can be included:** Answer is **yes, but divide forestry silviculture from food production and keep stages distinct**. Published [coppicing/fruit-nut placement addendum](https://github.com/jermur788/CCF/blob/docs/eight-scenario-coppice-fruitnut-addendum-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-COPPICING-FRUIT-NUT-PLACEMENT-ADDENDUM-20261112.md) — **proposal, not acceptance**.

| Case | Proposed curriculum addition | Distinct teaching proof / guard |
|---|---|---|
| **O3 Neglected Broadleaf Wood** | **Primary coppice or coppice-with-standards lesson**, ideally a suitable former hazel coppice area | Compare reviving viable stools vs leaving sensitive/failed stools vs another treatment. Plan small rotational coupe and retain standards. Following years show **real resprouting**, browse, stump/stool condition, light, multiple shoots, repeated management and usable smallwood. Old stools may fail to resprout; coppice should never be universal. |
| **O4 New Farm Woodland** | Site-appropriate **hazel/other suitable productive trees** and possible forest-garden **planning/recognition** as a bridge; **simultaneous nurse planting** is a separate core forestry-establishment method | Observe establishment, light, species suitability, space, browse and non-plant habitat. No orchard yield, harvest, processing or blanket fruit/nut ROI during Stage 1. |
| **O6 Atlantic Oakwood** | **Coppice counterexample** | Careful conservation decision; old trees, epiphytes, old habitat/stools and continuity may make resuming coppicing harmful. No mandatory coppice to "improve biodiversity." |
| **O8 Complete Irish Forest Estate** | **Mixed-use decision** over a limited coppice woodlot and future productive-tree area | Budget labour/harvest/access vs restoration and future yields **within estate only**; a strategic preview of Stage 2, not a full orchard economy. |
| **Stage 2 first expansion, D-054** | **Full fruit, nut, orchard/forest-garden management** after mature forestry | Distinct flowering/fruit set, produce, pruning, pests, harvest, processing, sale/income, and layered management under credible new species/market/economic models. This is the first Stage 2 slice by existing user decision. |

**Research confidence:** Teagasc's Irish Woodland Improvement Scheme coppice/coppice-with-standards section and NPWS Native Woodland Management Guidelines are management guidance for selective coppice and habitat sensitivities. Teagasc 2026 Forest Garden agroforestry pilot is a real policy example, but grant ratios/prices must **not** become Irish permanent ecology coefficients or universal management guidance. Existing `Fell` and mortality do not represent live **coppice stool identity** or stored regrowth. Coppicing is a new causal silvicultural action with persistence, standards-retention and browsing challenges. Full fruit/nut production needs separate crop physiology/economy and is **not yet implemented**.

**Critical manager nuance:** O3 already proposes tree quality, neglected management, ash dieback and habitat retention. Adding coppice *could overload a single chapter* with too many separate new systems. Ask Claude to determine whether coppice is best as a **bounded optional management fork** inside O3, as a later O8 challenge, or another delivery sequencing option **without losing the eight core cases**. O4 already has pre-forest site and establishment blockers; simultaneous nurse method may be the distinctive teaching activity, but do not require four different implementations in the first release. Prefer one true causal version over four fake menu descriptions.

## 5. Evidence and implementation safeguards

- Use explicit categories: **accepted decision**, **inspected/verified production at a specified candidate**, **Irish observed evidence**, **Irish practitioner guidance**, **UK/continental transfer**, **gameplay abstraction**, **unsupported/needs research**. Do not conflate research-ready Scots pine/downy birch/alder with shipped species models, and do not apply Sitka density/economy or sample-to-comparable MFM rules indiscriminately to nurse/coppice/broadleaf structures.
- Preserve meaningful **do-nothing/monitor** options: no planting on inappropriate habitat; no unnecessary release/clearance of heritage woodland; no unsafe owner windblow work.
- For new mechanics, demand **authoritative causal and persistent state** before showing visual feedback. Nurse benefit vs suppression, coppice regrowth and crop-role intention must not be painted-on outcomes or fabricated historical reasons. Protected saves/Reference and single-writer/Unity locks apply to future code.
- Don't claim a full ecological or agricultural simulator simply because O4 references productive trees or O6 has native habitat art. Stage 2 remains beyond the mature eight-forest forestry-first programme.
- Each scenario receives minimum demonstrable loop **inspect → decide/plan/hold → Work Plan approval (if applicable) → annual ecology/economy → walk and real review → revise**. Avoid invented annual probability, unverified economic yields or single-optimum tree marks.
- No full Tier 3 qualification until genuine milestone/release candidate. Research only now; no code/Unity or speculative multi-scenario framework.
- **No release-order change** until product owner accepts chapter sequence; keep #424 focused on first external demo.

## 6. Requested Manager actions and ownership

**Immediate (research/management only):**
1. **Read** original eight proposal, #428 refined three orders, #430 simultaneous nurse research, and coppicing/fruit-nut addendum. This handoff is the synthesis, not a replacement for source detail.
2. **Quality verdict** on #428 and newer addenda: **APPROVED AS RESEARCH INPUT / NEEDS CHANGES / REJECTED** (not product approval). Check whether O3 coppice would make its lesson incoherent and whether O4 nurse approach can be modelled credibly at bounded cost.
3. **Prepare an independent Claude critique**. Give Claude **both original and refined eight reports** plus the two supplementary studies; ask for unbiased ordering challenge, case overload/duplicates, actual ecology mechanisms, conservation and small-owner learning, reuse and engineering costs, safe overlap and lower-cost alternatives.
4. Return a concise **product-owner decision memo**, ideally choices: (a) final A/B/C or modified order; (b) whether O3 bounded coppicing and O4 simultaneous nurse planting are explicit intended lessons; (c) whether fruit/nut Stage 1 stays limited to recognition/establishment (recommended) and the Stage 2 production work remains separate; (d) O8 estate-internal capstone complexity. Do not re-ask whether there should be eight.
5. On explicit acceptance, update decisions/brief through normal authorised canonical-context sync rules and commission specs *only when relevant to a scheduled scenario*. #424 retains current active coordination/Unity ownership; another scenario worker must not be started here.

**Completion and routing:** This is a **low-risk, medium-difficulty** GitHub/browser strategic-intake handoff (Medium because multiple existing research outputs and current product authority must be reconciled; no code work). **Best suited:** the existing Overall Development Manager #425, ChatGPT GPT-6 High if available, on its already running **browser/GitHub strategy management lane** (no local worktree; existing #425 management branch; unique future manager intake documents only). Manager must check live issue/ownership before any writes. **User action to dispatch:** paste the link to this handoff into the existing Overall Manager #425 chat. No gameplay code or user testing required for reading/review.

## 7. Source links and exact status

- [D-078 and D-054 accepted authority](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md) and [Game Brief](https://github.com/jermur788/CCF/blob/main/Docs/Project/game-brief.md).
- [Original eight-case report](https://github.com/jermur788/CCF/blob/docs/independent-eight-irish-forest-scenarios-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/INDEPENDENT-EIGHT-IRISH-FOREST-MANAGEMENT-SCENARIOS-20261112.md).
- [#428 three-order/dependency research](https://github.com/jermur788/CCF/blob/docs/eight-scenario-curriculum-refinement-result-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-CURRICULUM-DEPENDENCY-REFINEMENT-RESULT.md) — research done, issue closed, Claude review pending.
- [#430 Pommerening simultaneous nurse-crop research](https://github.com/jermur788/CCF/blob/docs/pommerening-instant-new-ccf-research-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/POMMERENING-INSTANT-NEW-CCF-BARE-LAND-RESEARCH-20261112.md) — research done, issue closed, user clarification retained.
- [Coppicing and fruit/nut curriculum addendum](https://github.com/jermur788/CCF/blob/docs/eight-scenario-coppice-fruitnut-addendum-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-COPPICING-FRUIT-NUT-PLACEMENT-ADDENDUM-20261112.md) — unapproved proposal, document-only.
- [#425 Overall Development Manager](https://github.com/jermur788/CCF/issues/425) running strategy; [#424 Scenario One Delivery Manager](https://github.com/jermur788/CCF/issues/424) owns delivery/Unity/live locks.

**Changes made by this handoff:** one new GitHub Markdown coordination artifact on isolated docs branch (to be verified); no canon changes, playable code, Unity, tests or product acceptance. Dashboard and Drive sync **not verified**.
