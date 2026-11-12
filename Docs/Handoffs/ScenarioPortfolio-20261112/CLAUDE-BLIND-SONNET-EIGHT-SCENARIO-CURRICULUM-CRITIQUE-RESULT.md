# CCF #432 rerun — Blind (Sonnet) critique of the eight-scenario curriculum, nurse establishment, coppicing, fruit/nut boundary and estate capstone

**Date:** 12 November 2026
**Status:** INDEPENDENT RESEARCH CRITIQUE / NOT A PRODUCT DECISION. Nothing here approves an order, a mechanic, a spec, a save change or any code.
**Claim:** [#432](https://github.com/jermur788/CCF/issues/432) (closed; blind rerun noted there). **Overall Manager:** [#425](https://github.com/jermur788/CCF/issues/425).
**Reviewer (actual, from session metadata):** Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), effort setting `high`, served model `claude-sonnet-5-5`. This is a different model from the first #432 review (recorded there as Opus 5.5, medium).
**Lane:** GitHub API/MCP only plus read-only fetches of main files into a scratch folder. No Unity, no Editor, no machine lease, no build, no tests run.
**Branch:** `docs/claude-eight-scenario-blind-sonnet-critique-result-20261112-v2`, created from `main` at `bc38ff2461c6aa004dde6a73b84718c313f41c44` (branch did not exist before). **Writable scope:** this one file only.
**Proof of ordering:** section 1 below was committed on its own at `d8a9953948438823ca89c90a7de35d5aa0ca82b9` (2026-11-12 19:05:16 UTC), before any researcher or manager document was opened. This commit adds sections 2 onward and leaves section 1 unchanged.

**Headline result**

- **Recommended order:** O1, O2, O4, O3, O7, O6, O5, O8.
- **Runner-up:** O1, O2, O3, O4, O7, O6, O5, O8 (my own blind order from section 1; differs by one swap). Fallback if the O2 proof fails: O1, O4, O2, O3, O7, O6, O5, O8.
- **Research verdict:** NEEDS CHANGES. No blocking findings. The corrections are about framing and two unverified source claims, not about the order question.
- **Confidence:** low to moderate. Positions 3/4 and 6/7 are near-ties. See sections 2 and 8 for how much of my agreement with earlier reviews could be anchoring.

---

## 1. Blind provisional judgement (written before opening any researcher/manager material)

*(Reproduced unchanged from commit `d8a9953`.)*

**Written:** 2026-11-12, after reading only accepted canon on `main` (Decision Log incl. D-051–054, D-059/060, D-064/065, D-071, D-078; Game Brief; Current Milestone; Unity Project Overview save/species facts; `Docs/ScenarioTwoSpeciesFoundation.md`). The "original eight-case report" could not be located on `main` or in the project files, so the O1–O8 labels and one-line descriptors in my task brief were used as the case definitions.

**Provisional order (from first principles):** O1, O2, O3, O4, O7, O6, O5, O8.

Reasoning in brief:
- **O2 second** because it is the only scenario that needs *zero* new biology: Sitka, Oak and Beech are already parity-accepted (D-059), planting, browsing, deadwood and the economy exist. That isolates the one genuinely new thing a second scenario must prove, independent scenarios (a scenario loader, a per-scenario save identity), from any new-species or new-mechanic risk. Today there is one `forest-save.json` and a scenarioId mismatch only warns.
- **O3 third** because it reuses Oak/Beech form, Crop Trees, pruning and deadwood on a new authored start state; ash dieback and hazel coppice are the only new mechanics and both can be kept out of the minimum.
- **O4 fourth**: first scenario with a bare-land start, site suitability and establishment decisions. Pioneer/nurse species (Scots pine, downy birch, common alder) are on HOLD, so an honest establishment scenario with only Sitka/Oak/Beech is thin on the nurse question.
- **O7 fifth**: StormModel 1 exists but is dormant; D-071 parked complex access/extraction for "future independent scenarios", so O7 is its natural home. Needs activation qualification and standalone-Player profiling first.
- **O6 before O5**: O6 builds on existing browsing machinery and adds invasives/continuity; O5 needs hydrology, waterlogging and riparian species (alder/birch partial evidence) and is the heaviest new subsystem.
- **O8 last** as a finite capstone: needs compartments inside one scenario, the only place a shared ledger is allowed (D-078).

**Key questions I wanted answered by the other material:**
1. Is a scenario loader + per-scenario save identity really the smallest honest "second scenario" proof, and who owns it?
2. Can O4's nurse/pioneer teaching be honest with only Sitka/Oak/Beech sim-ready, and what Irish (not transferred) evidence supports simultaneous year-zero nurse + target planting?
3. Can O3 ship without ash or hazel (no ash species asset exists; no coppice regrowth mechanic), and what is the minimum honest ash-dieback representation?
4. Can StormModel 1 be activated per scenario without touching Scenario One's protected anchors, and does O7 force the deferred access/extraction work?
5. What is the smallest finite O8 architecture (compartment count, shared-resource boundary) that is not a generic property sim?

**Exposure before this section was written (disclosed in section 2):** while loading project memory I saw one line recording the earlier #432 result (its order and verdict), and I read the body of issue #425, which states a five-scenario lean recommendation by another reviewer. Neither was used as evidence, but the blind condition is therefore **partial, not clean**.

---

## 2. Scope, independence and exposure (honest disclosure)

**Accepted versus proposed.** Accepted: D-051 forestry-first; D-052 independent scenarios, sandbox later; D-053 small private/farm-forest audience; D-054 fruit/nut is the first Stage 2 slice; D-059/D-060 three-species parity and a complete Scenario One as the first external demo; D-064/D-065/D-071 Rack V2, MFM and the five-parallel-rack layout for Scenario One; D-078 eight independent Irish scenarios with sequential delivery and safe overlap. Proposed and **not** accepted: every order, every mechanic, ash/hydrology/invasive/storm/nurse/coppice models, any scenario loader or save design, any extra species.

**This rerun was not fully blind. What I saw, in order:**

1. **Before section 1 (two exposures).**
   - The project memory index, which I read as part of loading context, contains a line recording the first #432 review: its order (O1, O2, O4, O3, O7, O5, O6, O8) and verdict (NEEDS CHANGES). I could not unsee it.
   - I read the body of issue #425, which summarises a different five-scenario study and its lean recommendation.
   - The same memory note also told me "single save file, scenarioId mismatch only warns". I wrote that into section 1 as a fact **before** I had checked it in source. I have since verified it directly (section 7), but the claim in section 1 was memory-derived, not first-hand.
2. **Protocol deviation.** The packet says to read the original eight-case report first. I could not find it on `main` or in the project files, so section 1 was written from canon plus the case descriptors in my task brief. I found the report afterwards, on its own branch, while opening #431/#430/#428, and read it after section 1 was committed. I opened the #431, #430 and #428 issue bodies (which state the researcher's B recommendation) **before** I read the original report. That does not affect section 1, which was already committed.
3. **After section 1**, in this order: issues #431, #430, #428; the Manager #431 review and the critique packet; the original eight-case report; the #428 result; the #430 report; the coppicing/fruit-nut addendum; and, last, the first #432 result in full.
4. **What I could not verify.** Outbound access to `teagasc.ie` and `www.coford.ie` is blocked in this environment (proxy refusal), so I could **not** re-read COFORD (2012) or Teagasc WIS Element 4. Every statement below about those sources comes from the project's own summary PDF (`CCF_Instant_New_CCF_Nurse_Crop_Research_Summary.pdf`, which I read) or from other documents' descriptions, and is labelled so.

**What this means for independence.** My final order equals the first #432 review's order except for one swap (O6 and O5), and my blind order equals the researcher's B except for one swap (O6 and O5). Two reviewers with partial exposure landing next to the same answer is **weak** evidence. The parts I trust as mine are the ones I can tie to something I checked myself: the source checks in section 7, the species-registration finding, the "capstone must not introduce a new mechanic" argument, the "optional costs the same as required" point, and the O5/O6 swap. Section 8 lists them.

---

## 3. What changed between my blind view and my final view

| Item | Blind (section 1) | Final | Why |
|---|---|---|---|
| Position 2 | O2 | O2 (unchanged) | Reasoning held after reading: lowest new-biology cost and it isolates the loader/save proof. |
| Positions 3/4 | O3 then O4 | **O4 then O3** | Changed **after** seeing the first review's D. I re-derived the call on my own criteria (section 4.3) and landed there, but for **different** reasons from that review. I think its main reason ("O3 needs the establishment pathway O4 builds") is overstated, because Scenario One already underplants Oak/Beech. My reasons are: new-species cost (ash is not free), verb alternation, and audience fit. The call is a near-tie. |
| Position 5 | O7 | O7 (unchanged) | |
| Positions 6/7 | O6 then O5 | O6 then O5 (unchanged) | I kept my blind view against two earlier documents that put O5 first. See 4.3. |
| Position 8 | O8 | O8 (unchanged) | |
| Blind assumption that ash "can be kept out of O3's minimum" | Held | **Withdrawn** | Ash dieback is the Irish signature of O3. Without it the case loses most of its identity. Ash must be at least an authored staged state, and a non-ready species is never registered on main (section 7). |

A question the Manager's packet asked, which I only saw after section 1: *what is the best order for a novice small private woodland owner, ignoring code reuse?* My answer, written afterwards and therefore **not blind**: O1, O4, O3, O2, O5, O6, O7, O8, because a farm owner's first real decision is whether and what to plant. I do not recommend it, for reasons in 4.1 (A scorecard): it front-loads the largest new-world cliff immediately after a long, heavily guided Scenario One.

---

## 4. Scorecard, overlaps and blockers

### 4.1 Side-by-side (qualitative, reviewer judgement, not measured)

Orders under comparison (all keep O1 first and use each ID once):

- **A (beginner-to-owner):** O1, O4, O3, O5, O2, O6, O7, O8
- **B (forestry craft, researcher's pick):** O1, O2, O3, O4, O7, O5, O6, O8
- **C (ecology first):** O1, O5, O6, O4, O7, O3, O2, O8
- **D2 (this review):** O1, O2, O4, O3, O7, O6, O5, O8

| Criterion | A | B | C | D2 |
|---|---|---|---|---|
| Fit to small private/farm owner (D-053) | **High** | Medium | Medium | Medium–High |
| Chance the first post-O1 case is buildable without new species or disease/water state | Low | **High** (conditional on an honest irregular start) | Very low | **High** (same condition) |
| Verb variety across the first four post-O1 chapters | Medium | Low (three marking-type cases then plant) | Medium | **Medium–High** (mark, plant, mark+health) |
| New-player overload at position 2 | Medium–High | Medium–High | High | Medium–High |
| Front-loading of unbuilt systems (hydrology, species, disease, storm, invasives) | Medium | Medium | **High** | Medium |
| Evidence-honesty exposure early | Medium–High (site and seedling odds) | Medium | Very high (water, invasion numbers) | Medium |

**A — strengths:** (1) starts with the decision real farm owners face first; (2) gives early visible causal payoff (protection, browse, gaps); (3) breaks the marking run early. **Weaknesses:** (1) the first post-O1 case needs bare-land state, site masks and an area-planting order type before any reuse benefit; (2) O3 right after O4 puts disease and safety rules in front of a player who has not yet done a mature-stand cycle outside O1; (3) O2, the case that proves "maintenance" CCF, drifts to position 5, so the programme's central skill is tested last.

**B — strengths:** (1) lowest infrastructure risk for case 2; (2) tests whether O1's CCF skills transfer to a different start state; (3) uses the accepted Oak/Beech work early. **Weaknesses:** (1) O1, O2 and O3 all hinge on mark-and-release, which risks fatigue; (2) O3 carries ash, form, veterans and now coppice all at once before any establishment system exists; (3) B's own rationale is partly "more code exists", which is an engineering argument, not a teaching one.

**C — strengths:** (1) strongest scenery and site contrast early; (2) good for a conservation-led audience; (3) defers commercial depth, so no repeat of O1 early. **Weaknesses:** (1) wet-site and continuity state both have to exist before case 2; (2) commercial CCF disappears for five chapters, which weakens the connection to the game's core; (3) highest temptation to show numbers (flood, epiphytes, invasion) that no source supports.

**D2 — strengths:** (1) same cheap case 2 as B; (2) plant arrives third, so the first four post-O1 verbs alternate; (3) ash, with the heaviest species/evidence debt, gets the most research lead time. **Weaknesses:** (1) O4 still needs an area-planting order, site masks and open-ground vegetation competition one slot earlier than B; (2) it depends on the same unproven O2 irregular start as B; (3) with only Sitka/Oak/Beech ready, O4's species choice is narrow, so "site before species" has less to choose from.

### 4.2 Overlap checks

Each case needs one unique observable decision and consequence.

| Pair | Overlap risk | Unique decision to protect |
|---|---|---|
| O1 / O2 | O2 becomes "O1's late phase" if the start state lacks real size-class variety | O1 converts and creates structure with sample-to-comparable treatment. O2 **declines** to harvest a valuable seed tree to protect recruitment, with no rack planning. |
| O2 / O3 | Both are release-and-thin | O2 is cohort and regeneration-led. O3 is single-tree form, health and safety-led. |
| O3 / O6 | Both retain veterans and broadleaves | O3 improves productive quality. O6 is restraint: invasive control, browse and doing less. Build the veteran register once in O3 and reuse it in O6. |
| O4 / O2 | Both involve young trees | O4's young trees are **planted** and the question is whether and where to plant. O2's are natural recruits. |
| O5 / O7 | Both restrict access | O5's cause is wet soil and water sensitivity. O7's is exposure and stability. Make each restriction visibly caused by something different. |
| O8 | Becomes a recap checklist | A forced-omission rule: the budget must leave at least one known problem untreated, and the consequence must be reviewable. |

### 4.3 Why O4 before O3, and O6 before O5

I applied five criteria: new-species cost, new persisted state, verb variety, audience fit, and shared-layer reuse.

- **O4 before O3.** O4 needs no new species (Sitka, Oak and Beech are ready) but does need a bare-land start, site masks, ground-vegetation competition and an **area planting order**. The current planting path is ground-selected and per-tree (Unity overview), which would not scale to a farm-woodland planting without a new order type. I flag this as a hidden cost but will not put a stem count on it. O3 needs a new species (ash) plus a disease state plus a veteran register. Species registration on main refuses a species that is not simulation-ready (section 7), so ash cannot be added as a light asset. O4 also alternates the verb and fits the farm-owner audience. Against this, O3 reuses accepted Oak/Beech form and timber work (D-062) with less scale risk. The balance is close. **An owner-level tie-break would be a cheap costing spike: ash species plus staged-disease state versus area-planting order plus site masks.**
- **O6 before O5.** O5's functional contrast needs alder/birch/willow, which are HOLD or partial, plus a new wetness and soil-disturbance state. O6 needs a new invasive-cover state and browsing continuity, but its species are mainly Oak plus ecology-only understorey. O6 also reuses O3's veteran register and the browsing/protection machinery from O4. O5 shares an access/soil-disturbance layer with O7, which argues the other way, so this is also a near-tie. I keep O5 later because it concentrates the most unbuilt state and the weakest species evidence, and because "deliberate non-intervention" is a good late lesson.
- **O7 fifth in both.** It reuses the dormant StormModel 1 and is the natural home for the access/extraction complexity D-071 pushed out of Scenario One. Storm activation needs its own qualification before use (section 7).

### 4.4 Blockers per case and whether the order front-loads them

| Case | Blockers (what honest play needs that is not on main) | Front-loaded? |
|---|---|---|
| O2 | Authored irregular start; per-scenario save slot; cross-load rejection; size-class/cohort readout from live tree state | No (small) |
| O4 | Bare-land start; site/habitat masks; area-planting order; open-ground vegetation competition; establishment cost path; valid no-plant outcome | Moderate |
| O3 | Ash species (ready-state), authored staged disease state, contractor/safety route; veteran register; optionally coppice (see 5.2) | Moderate–high |
| O7 | Scenario-scoped storm activation and its qualification; exposure/rooting risk; salvage with contractor-only hazard; decision on where D-067–069 access research lands | High |
| O6 | Persistent invasive cover with bounded recolonisation; understorey/holly proxy; distinguishing saplings from seedling carpets; veteran/deadwood retention | High |
| O5 | Wetness/gley/riparian masks; soil-disturbance and access closure; alder/birch/willow readiness; no water-quality or flood claims | High (heaviest) |
| O8 | Estate partition (zones or compartment records); shared budget/labour; unique IDs; performance; per-compartment history | Highest, hence last |

---

## 5. Nurse, coppice, fruit/nut and O8

Classifications are **concept level only** (REQUIRED / OPTIONAL / DEFERRED). They say whether a lesson belongs in the curriculum, not whether anything is approved for building.

### 5.1 Simultaneous year-zero nurse and target planting in O4

**Method as the owner described it (preserved):** a fast temporary nurse and a slower long-term crop planted **at the same time** on bare land. No shelter exists on day one. Shelter grows with the nurse. Some nurses are removed before they mature or suppress the crop. It is one route beside pioneer-first-then-underplant, direct mixed planting with no dedicated nurse, and natural colonisation or leaving valuable open habitat unplanted. The 2023 webinar wording has not been transcribed by anyone, me included, and I attribute no quotes to it.

| Item | Verdict | Why |
|---|---|---|
| O4 core: site before species, no-plant as a valid success, direct planted groups versus natural colonisation | **REQUIRED** | Needs only Sitka/Oak/Beech and existing competition and browsing. Teaches the real first decision without a counterfeit mechanic. |
| Simultaneous nurse + target as a **lesson** | **OPTIONAL** (in the curriculum) | Worth teaching: the two-way failure (nurse too slow gives no shelter; nurse too fast suppresses the crop) is a genuine management dilemma. |
| Simultaneous nurse as a **playable mechanic in O4's first release** | **DEFERRED** | No nurse species is simulation-ready (below), and there is no positive-shelter mechanism on main. |
| Pioneer-first then underplant | **DEFERRED** | Needs pioneer species that are on HOLD. |

**Would Sitka as a stand-in nurse work?** I do not recommend it. The Irish observations cited in the project summary involve Scots pine, European larch and Norway spruce, not Sitka. Sitka is far more vigorous, so it would only demonstrate suppression, never the "too little shelter" direction. That is half the dilemma and would bias players towards early removal.

**Smallest honest version when it is eventually built** (not approved): persisted per-tree role (temporary nurse versus intended crop), distinct from Crop Tree and Fell marks; a simulation-ready nurse species; existing light/crown competition applied between nurse and crop; a positive shelter term **only if** a spec can defend one, labelled as a game abstraction; staged nurse removal as Work Plan orders with labour and cost; a save and history that records what was done; a render tied to the state; and an evidence gate showing **measurably different authoritative tree states under at least two removal histories**. Modelling competition alone with a text note that "shelter is not modelled" should not ship.

**Evidence classes**

- *Irish data (observation, site-specific):* COFORD, Guest & Huss (2012), *Nursing of oak in Ireland*, as summarised in the project PDF: pine sometimes gave inadequate shelter on heavy soils; larch or spruce could dominate where nurses outgrew the oak or were not removed in time. **Not re-read by me** (blocked). These are examples, not rates, spacings or removal years.
- *UK/continental transfer:* Pommerening & Murphy (2004) is a conceptual review; Martiník et al. (2024), a Czech two-year beech trial, shows conditional benefits only; CCFG guidance is UK practitioner advice.
- *Unsupported:* any nurse growth multiplier, nurse-to-crop ratio, removal year or frost-protection effect.
- **Unverified by me:** the first #432 review reports that the COFORD text calls Scots pine unsuitable as a same-time oak nurse and suggests nurse-first planting. If that is accurate it materially narrows #430's support for the *simultaneous* variant. I could not check it. **Someone with PDF access should re-read COFORD before the owner decides P-3.**

**Dependencies if later adopted.** *Save:* a per-tree role field and nurse-removal history. *Species:* one ready nurse species (larch is absent from main; Scots pine, downy birch and common alder exist only as HOLD assets). *Unity:* new planting-design UI; one serialized Unity lease; no change to Scenario One's protected anchors.

### 5.2 Hazel coppice in O3

| Item | Verdict | Why |
|---|---|---|
| O3 core: Oak/Beech form and release, veteran retention, ash triage with a professional/contractor route | **REQUIRED** | This is O3's identity. Ash is an authored staged state, not an epidemic model. |
| Coppice-with-standards as a Stage 1 **teaching skill** | **OPTIONAL** (curriculum level) | Legitimate forestry, and the project's own species selection treats hazel as "more naturally an understorey/coppice component". |
| A **real coppice mechanic** (stool identity, resprouting, browse on shoots, smallwood) | **DEFERRED** | Not in O3's first release, **and not in O8**. |
| An **authored former-coppice patch** in O3 as inspectable history (static multi-stem clumps, a recorded decision to retain, flag or leave) | **OPTIONAL** | Keeps the skill visible with no regrowth claim. |
| O6 as a no-coppice counterexample | **OPTIONAL** | Costs nothing if old stools and veterans are shown as do-not-cut continuity. |

**Where I differ from the first review:** it recommended deferring the real mechanic to O8 or a later O3 update. I would not put it in O8. A capstone should compose skills the player already has. Introducing a new core biological mechanic there both overloads the case and defeats its purpose as an integration test. The mechanic should wait for a separate, later spec after the eight.

**Why "optional" is not cheap.** A mechanic that is optional for players costs the same to build, serialize and qualify as a required one. "Small optional patch" is therefore not a saving.

**Hidden cost.** Main has no hazel species, no coppice code and no stool or multi-stem identity (searched the file tree on `main`). A real coppice lesson means a new species, a new persisted entity and a new Work Plan order type at once.

**Evidence classes.** *Irish guidance:* Teagasc WIS Element 4 and NPWS (2017) woodland guidelines, as cited by the addendum; **not re-read by me**. The first review reports that the Teagasc page names no species and frames coppice as broadleaf improvement after thinning on free-draining soils. I could not confirm that either. *Game abstraction / unsupported:* any rotation length, smallwood yield, price or grant rate.

**Dependencies if later adopted.** *Save:* stool identity, cut year, viability and shoot state. *Species:* a hazel definition at D-059 depth, no hazel exists today. *Unity:* new render for multi-stem regrowth; new Work Plan order; serialized lease.

### 5.3 Stage 1 fruit/nut recognition versus Stage 2 production (D-054)

| Item | Verdict | Why |
|---|---|---|
| The boundary itself: Stage 1 may name and place site-appropriate hazel or nut/fruit-bearing species **as forest species** (role, competition, protection), with no produce | **REQUIRED** | Keeps D-054 intact. |
| Static recognition facts or props with no growth role | **OPTIONAL** | Cheapest honest form. |
| Any produce number: flowering, fruit set, yield, harvest, processing, price, orchard or forest-garden profit and loss | **DEFERRED** to Stage 2 | Needs separately accepted crop physiology, market and workflow. |
| A fruit/nut species in the Stage 1 planting palette | **DEFERRED** (owner choice) | It would be species-model work at D-059 depth, not a label. |
| O8 showing fruit/nut opportunities as text only | **OPTIONAL** | Awareness without an economy. |

**Evidence class.** The 2026 Teagasc forest-garden pilot (up to 40% fruit/nut) and FT8 (up to 15%) figures are *Irish policy snapshots* with scheme-specific eligibility limits. They are not ecology and not game parameters.

**Dependencies.** *Save and species:* none for recognition-only. For any palette inclusion: a new species definition. *Unity:* none for text and props.

### 5.4 O8 as a finite estate capstone versus a generic property sim

| Item | Verdict | Why |
|---|---|---|
| Finite, authored estate of a small number of compartments, one estate-local budget and labour pool, forced omission, and a defended plan | **REQUIRED** | Matches D-078: any shared resource stays inside O8. |
| O8 introducing **no new biological mechanic**; every compartment uses only systems qualified in earlier cases | **REQUIRED** | Capstone integrates, it does not introduce. |
| Generic property or sandbox simulation | **DEFERRED** | Belongs to the sandbox that D-052 already puts later. |
| Choice of architecture: zones in one world versus compartment records under one estate save | **DEFERRED** (decide after O2 and O4) | The first review proposed zones in one world. That is plausible and avoids multiple running worlds, but it requires every earlier system to run side by side in one simulation, which is a performance and composability question I cannot judge from documents. The researcher's compartment-record alternative is equally unproven. |

**Dependencies.** *Save:* the highest-risk change in the programme, since O8 needs the first estate-local shared state. *Species:* none beyond earlier cases. *Unity:* the largest world and the most simultaneous systems, so performance profiling is a gate.

---

## 6. Recommended order, runner-up and owner decision points

**Recommended:**

1. **O1 Inherited Sitka Plantation** (fixed, full Scenario One, protected)
2. **O2 Working Mixed Forest**
3. **O4 New Farm Woodland**
4. **O3 Neglected Broadleaf Wood**
5. **O7 Exposed Upland Forest**
6. **O6 Atlantic Oakwood**
7. **O5 Wet Woodland and River Valley**
8. **O8 Complete Irish Forest Estate**

**Runner-up:** O1, O2, O3, O4, O7, O6, O5, O8 (my blind order). **Fallback if the O2 proof fails:** O1, O4, O2, O3, O7, O6, O5, O8. It is not C.

**Confidence caveats.** Moderate at best for position 2, low for positions 3/4 and 6/7. This is a qualitative ranking from documents and a few static source checks. No playtest, no novice usability test, no cost estimate and no Unity run supports it. A novice test of O2 could reverse the position-2 choice. Scenario One has no external-demo candidate yet (Current Milestone), so none of this can start building before that gate.

**Owner decision points (ranked)**

| # | Decision | My recommendation |
|---|---|---|
| P-1 | Accept an order, and decide whether to settle the two near-ties (O4/O3, O6/O5) by a cheap costing spike rather than argument | Accept D2 as the working order. Run the spike before position 3 is specced. |
| P-2 | Make the second-scenario infrastructure an explicit pre-spec step: per-scenario save slot, cross-load rejection before mutation, old saves routed only to Scenario One | **Yes**, and make it a named gate, not a side effect of O2. |
| P-3 | O4 nurse scope | Curriculum OPTIONAL, playable first release DEFERRED. Re-read COFORD first. |
| P-4 | O3 coppice | Real mechanic DEFERRED, **not** to O8. O3 gets an authored former-coppice patch at most. |
| P-5 | Stage 1 fruit/nut | Recognition-only boundary; no produce numbers; no palette inclusion yet. |
| P-6 | O8 shape, and where the D-067–069 access/spine research lands (D-071 pushed it to "future independent scenarios" without naming one) | Finite authored estate that composes earlier modules. Assign the access research to O7 explicitly. Decide the zone-versus-record architecture after O2 and O4. |

---

## 7. Earliest credible playable new-scenario proof and architecture gate

### 7.1 Smallest honest second-scenario proof (O2)

**What the player sees:** from the session menu they start "Working Mixed Forest" without touching their Scenario One save. They walk an authored mixed Sitka/Oak/Beech stand with several size classes and existing access, so there is no rack planning. They mark retention or removal around a mature seed tree or hold, commit through the Work Plan, and advance one and then five years. A size-class/cohort readout derived from live tree state shows successors recruiting or failing. They save, quit, load Scenario One (unchanged) and load O2 (unchanged). **Negative checks:** a Scenario One save refused by O2, an O2 save refused by Scenario One, and an old save never routed to O2.

**Not in the proof:** yield forecasts, new species, MFM, storms, hydrology, disease, estate features.

### 7.2 What I checked in source on `main` (`bc38ff2`), first-hand

- `ForestSaveController.cs`: a single constant file name, `forest-save.json`, in `Application.persistentDataPath`. A second scenario on the current path would overwrite the Scenario One save.
- `ScenarioOneManager.cs` lines 1481–1482: if the loaded save's `scenarioId` differs from the active definition, it logs a **warning** and continues restoring. There is no rejection before mutation.
- `ForestSaveData.CurrentVersion = 24`.
- `ScenarioOneManager.cs` line 475: new games set `StormModelVersion = StormModel.None`. Storms are off in new Scenario One games.
- `ForestTreeSpawner.cs` lines 44–100: a species that is not `IsSimulationReady` is **never registered or resolved**. Per `Docs/ScenarioTwoSpeciesFoundation.md`, Scots pine, downy birch and common alder exist as HOLD assets under `Assets/ForestPrototype/Species/ScenarioTwo/`, and none is registered in any scene or save.
- File-tree search on `main`: **no** ash, larch, hazel, coppice or stool assets or code.

**Not checked:** any Unity behaviour, any runtime, any test. Source presence is not Unity qualification.

### 7.3 Gate before any second-scenario code

1. A per-scenario save location or slot, with Scenario One's current path unchanged.
2. Cross-scenario load rejected before any state is mutated.
3. Old saves (v1–v24) route to Scenario One only.
4. Legacy40 and the frozen Reference Future v1 untouched.
5. Narrow new, old and cross-load negative tests.
6. Do not generalise `ScenarioOneManager` (141 KB) into a framework until O2 shows real duplication.

Whether item 1 needs a schema bump is a spec question. I do not assert either way.

### 7.4 Save, species and Unity dependencies by case (summary)

| Case | Save | Species | Unity |
|---|---|---|---|
| O2 | Per-scenario slot; scenario id handling | None new | Serial lease; Tier-2 feature qualification; no storm change |
| O4 | Site-mask identity; planting cohort/cost history | None new for first release | Area-planting UI; open-ground vegetation render |
| O3 | Ash health state; veteran register | Ash to ready state, or an authored non-growing representation, and its art | Disease render tied to saved state |
| O7 | Scenario-scoped storm activation; event history; salvage records | None new | Standalone-Player profiling before activation (D-050); storm render |
| O6 | Invasive cover and treatment history | Understorey proxy; no new canopy species | Plant render |
| O5 | Wetness/disturbance per cell or route | Alder/birch/willow readiness | Terrain/soil render |
| O8 | Estate-local shared state | None beyond earlier cases | Largest world; profiling gate |

### 7.5 Serial versus parallel

- **Serial:** playable delivery (D-078); any save, controller, ecology, scene edit; any Unity work (one lease per machine). #424 keeps Scenario One, Unity and `active-tasks.md`. No second-scenario implementation before Scenario One passes its own gates and the owner's personal playtest. No Windows build.
- **Parallel and safe:** research, spec drafts and art for the next case on disjoint files under claims. Examples: O4 site-mask research while O2 is built, ash evidence while O4 is built, and nurse-species evidence at any time.

---

## 8. Evidence classification and how my conclusions differ

### 8.1 Evidence classes used

| Claim area | Class | Note |
|---|---|---|
| D-078 eight scenarios; D-054; D-059; D-071; accepted Scenario One scope | Accepted product decision | Canon on `main`. |
| Single save file, warn-only scenario mismatch, storms off in new games, species not registered unless ready, no ash/larch/hazel/coppice code | Inspected source, `main` `bc38ff2` | Static reading only; not Unity-verified. |
| Irish nurse-crop observations (COFORD 2012) | Irish data (site-specific observation) | Read only via the project summary; not re-read. |
| Nurse establishment concepts (Pommerening & Murphy 2004; CCFG) | UK/continental transfer | Conceptual or practitioner guidance. |
| Simultaneous beech/pioneer trial (Martiník 2024) | UK/continental transfer | Short, two-year, conditional. |
| Coppice and ash handling (Teagasc, NPWS) | Irish guidance | As cited by others; not re-read. Grant rates are snapshots. |
| Irish native woodland classification, rhododendron control by repeat monitoring | Irish guidance | As cited by the researcher. |
| Forest-garden fruit/nut ratios | Irish policy snapshot | Not ecology. |
| Irregular mixed-stand yield | UK/continental transfer plus abstraction | Irish yield models are mainly even-aged (project research). |
| StormModel 1 parameters | Game abstraction | All are gameplay calibration (D-050). |
| Shelter bonus, nurse ratios, removal years, stool growth, rhododendron regrowth rates, flood or runoff effects, storm or windthrow probabilities, fruit economics, biodiversity scores | **Unsupported** | Must stay as explicit calibration or an unimplemented state, never silent realism. |
| Checkpoint horizons Y+1/5/20/50 and every proposed order | Game abstraction / proposal | Teaching horizons, not forecasts. |

### 8.2 Research review verdict: **NEEDS CHANGES** (no blocking findings)

| Sev | Where | Finding | Correction |
|---|---|---|---|
| **MAJOR-1** | #428 §5 species rows, #430 "existing production reuse" | Treats Scots pine, birch and alder as "research only". They exist on main as HOLD assets under a species-readiness policy, and a non-ready species is never registered. Larch, the main COFORD nurse, is absent. | Record the real status and cost the O4 nurse and O5 work as "HOLD to calibrated". |
| **MAJOR-2** | #428 §6 "first real post-S1 boundary" | Says the cross-scenario load policy needs an audit. The concrete facts (one fixed file name, warn-only mismatch) are directly readable. | State them as the first concrete second-scenario blocker (7.2). |
| **MAJOR-3** | #430 COFORD row; Manager §3.1 | Possible overstatement of Irish support for the *simultaneous* variant. **I could not verify** the first review's report that COFORD calls Scots pine unsuitable as a same-time oak nurse. | Re-read the COFORD pages. State the variant as species- and site-conditional. |
| **MAJOR-4** | #428 §2 B | Order B recommended partly on code reuse. The O1, O2, O3 marking run is noted but not resolved. | Compare against an order that alternates verbs (D2) or justify B on teaching grounds. |
| **MAJOR-5** | Addendum, Irish evidence item 1 | Teagasc WIS Element 4 may not support a hazel-specific lesson. **I could not verify** this either. | Re-attribute hazel framing to NPWS or label it as a design choice. |
| MINOR-1 | Addendum, O3 row | An optional coppice fork is costed as if cheaper than a required one. | Same build and qualification cost. |
| MINOR-2 | #428 §6 gate G5 | O7 storm activation needs standalone-Player profiling (D-050), which interacts with the Windows-build gate. Not stated. | Add the dependency. |
| MINOR-3 | #428 and Manager P-5 | O8 considered only as compartment records. | Also consider zones in one world; decide after O2/O4. |
| MINOR-4 | #428 and Manager | D-071 sent access/spine research to "future independent scenarios" and no document names which. | Name O7 or make it an owner decision (P-6). |

Did my review find the Manager #431's concerns independently? For O3 overload, O1/O2 overlap, the O8 recap risk and the engineering bias in B, yes: they are in my section 1 questions or arose from canon, but I make no priority claim given my exposure. The Manager review did not raise MAJOR-1, MAJOR-2 or MINOR-2 to MINOR-4.

### 8.3 In plain words: how my conclusions differ

**From the researcher (#428, order B: O1, O2, O3, O4, O7, O5, O6, O8).**
- I put O4 before O3 and O6 before O5. B is otherwise my blind order too.
- I would justify case 2 on isolating the loader/save proof, not on "more code exists".
- I think the researcher under-reported how concrete the save blocker is, and mislabelled the pioneer species as research-only.
- I disagree that the nurse lesson belongs in O4's first playable, and I would not move coppice to O8.

**From the first #432 review (order D: O1, O2, O4, O3, O7, O5, O6, O8, verdict NEEDS CHANGES).**
- **Order:** identical except I put O6 before O5. That is a low-confidence difference.
- **O3/O4:** same result, different reason. I do not accept that O3 depends on O4's establishment pathway, because Scenario One already underplants. My reasons are new-species cost, verb alternation and audience.
- **Coppice:** it deferred the real mechanic to O8 or a later O3 update. I defer it past the eight and keep O8 mechanic-free.
- **Nurse:** it called simultaneous nurse OPTIONAL. I split curriculum value (OPTIONAL) from first-release (DEFERRED) and say why Sitka cannot stand in.
- **Verdict:** same label. I add that none of my findings blocks using the research as input for the order decision, and I flag two of its source corrections (COFORD pine, Teagasc hazel) as unverified rather than adopting them.
- **Additions it did not make:** the species-registration consequence for ash, the area-planting cost for O4, the D-071 access-research ownership gap, and the standalone-Player profiling dependency for O7.

**What I would not claim.** That my agreement with earlier work is independent confirmation. It is partly that and partly anchoring, and I cannot tell the proportions apart.

---

## 9. Done / not done

**Done:** read canon and the original eight-case report, committed a provisional order first, read all researcher and manager material and the first #432 result, checked specific save, storm and species claims in source on `main`, wrote this critique, and posted the #432 comments.

**Not done:** re-reading COFORD (2012) or Teagasc WIS Element 4 (egress blocked); any Unity run, test or build; any code, spec, save-schema or canon edit; any change to `active-tasks.md`; dashboard or Drive sync (not checked, report as NOT VERIFIED). No product decision is made here, and none should be inferred.

**Sources read:** main `Docs/Project/{decision-log,game-brief,current-milestone,unity-project-overview,research-index}.md`; `Docs/ScenarioTwoSpeciesFoundation.md`; `Assets/ForestPrototype/{ForestSaveController,ForestSaveData,ForestTreeSpawner}.cs` and `ScenarioOne/ScenarioOneManager.cs`; the original eight-case study, the #428 result, the #430 report, the coppicing/fruit-nut addendum, the Manager #431 review and critique packet, the first #432 result; and the project files `CCF_Instant_New_CCF_Nurse_Crop_Research_Summary.pdf`, `Scenario Two Species Selection for Irish Continuous-Cover Forestry.pdf` and `Irish Atlantic Woodland Ecology for a Continuous-Cover Forestry Game Simulation.pdf` (searched for hazel, ash and rhododendron statements only).
