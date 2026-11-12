# CCF #432 — Independent Claude critique of the eight-scenario curriculum, nurse establishment, coppicing, fruit/nut boundary and estate capstone

**Date:** 12 November 2026
**Status:** INDEPENDENT RESEARCH CRITIQUE / NOT A PRODUCT DECISION. Nothing here approves an order, a mechanic, a spec, a save change or any code.
**Claim:** [#432](https://github.com/jermur788/CCF/issues/432). **Overall Manager:** [#425](https://github.com/jermur788/CCF/issues/425). **Management intake:** [#431](https://github.com/jermur788/CCF/issues/431).
**Reviewer (actual, from session metadata, not inferred):** Anthropic Claude through a Claude Code cloud session. The serving model was `claude-opus-5-5` (Claude Opus 5.5) and the session effort selector read `medium`. This is **not** the preferred Sonnet 5.5 at high/xhigh. The owner stated a preference, not a requirement, and this reviewer records the real configuration.
**Lane:** GitHub only. A read-only sparse clone in a cloud container plus GitHub API and MCP. No Predator/ZX20 worktree, no Unity or Editor, no machine lease, no build.
**Branch:** `docs/claude-eight-scenario-independent-critique-result-20261112-v1`, created from packet branch `docs/overall-manager-431-curriculum-critique-prep-20261112-v1` at `2e28bce41890271b7118b38b8931e58517380a0f`. Main was at `bc38ff2461c6aa004dde6a73b84718c313f41c44` when inspected.
**Writable scope:** this one file only.

---

## 1. Scope and independence statement

### 1.1 Accepted product intent versus proposals

| Class | Items | Reviewer treatment |
|---|---|---|
| **Accepted (main, read 12 Nov 2026)** | D-051 forestry-first; D-052 independent scenarios, sandbox later; D-053 small private/farm-forest audience; D-054 fruit/nut is the first Stage 2 slice *after* forestry maturity; D-059 three-species parity without manufactured precision; D-060 complete Scenario One is the first external demo; D-064/D-065/D-071 Rack V2, MFM and the five-rack layout are Scenario One only; D-078 eight independent Irish scenarios delivered sequentially with safe overlap, and no shared money, trees or saves across the eight | Binding. Not reopened. |
| **Proposed, not accepted** | Exact order (A/B/C/D); case curricula; nurse, coppice, ash, storm activation, rhododendron and hydrology mechanics; extra species; scenario manager or save design; fruit/nut anything beyond D-054 direction; O8 estate shape | Critiqued here. Every recommendation below needs owner approval. |

### 1.2 Independence and blinding: honest disclosure

- **Partial blinding only.** The dispatch message that started this review contained the full A/B/C table and stated that the researcher recommends B, with A as runner-up. I saw both before I read any source. The packet itself also repeats them. So I cannot claim methodological independence from B.
- **What I did to limit anchoring.** I read accepted canon (Decision Log entries listed above, Game Brief, Current Milestone, Unity Project Overview) and the original eight-case report *before* opening #428, #430, the addendum, the #431 consolidated handoff or the Manager #431 verdict. I then wrote a provisional order and five questions to a scratch file, reproduced verbatim in §2. I read the Manager #431 verdict last.
- **What I formed independently**, meaning not stated in #428, #430, the addendum or the Manager #431 review:
  1. The engineering argument that **O4 before O3 and O7** is a real dependency, because ash reconstitution (O3) and storm reforestation (O7) both reuse the establishment system that O4 must build first.
  2. The **single save slot finding**: current main writes one fixed `forest-save.json`, and a mismatched `scenarioId` only logs a warning (§6.2).
  3. The finding that **Scots pine, downy birch and common alder already exist on main as HOLD, not simulation-ready, species assets**, and that this changes the cost of the O4 nurse and O5 wet-site work (§4.1, §7).
  4. The finding that **COFORD (2012) itself says Scots pine is not a suitable nurse for oak when the two are planted at the same time** (§4.1). This qualifies #430's use of that source.
  5. The finding that **Teagasc WIS Element 4 does not name hazel**, and that its eligibility and soil framing differ from the addendum's hazel-centred vignette (§4.2).
  6. The **O8 "compartments as zones in one world" alternative** (§4.4).
- **What overlaps with prior documents.** The O3 overload concern, the O1/O2 overlap risk, the O8 recap risk and the question "was B chosen because code exists?" all appear in the Manager #431 review. I reached them independently from the original report (they are listed in my scratch questions), but I make no priority claim.

---

## 2. Provisional independent order, recorded before reading #428's case for B

Recorded at about 18:58 UTC, after reading canon and the original eight-case report, and before opening #428, #430, the addendum or #431:

> **O1, O2, O4, O3, O7, O5, O6, O8**
> Questions: (1) does O2 overlap O1's late phase? (2) which case has the smallest honest engine delta, given three simulation-ready species, one `forest-save.json`, `ScenarioOneManager`/`ScenarioOneDefinition` and no loader? (3) is O3 overloaded? (4) is O7's dormant StormModel1 cheap Sitka reuse or qualification debt? (5) should O8 be an authored estate, and how do we stop it becoming a recap?

**Caveat:** this differs from B only by swapping O3 and O4. Because I had seen B, the closeness may partly reflect anchoring. After reading #428 in full I **kept** the swap and strengthened the argument for it (§3.5). I did not change any other position.

---

## 3. Side-by-side critique of A, B, C and a reviewed D

### 3.1 Scorecard (qualitative, relative, reviewer judgement; not measured)

| Criterion | A Beginner-owner (O1 O4 O3 O5 O2 O6 O7 O8) | B Forestry craft (O1 O2 O3 O4 O7 O5 O6 O8) | C Ecology first (O1 O5 O6 O4 O7 O3 O2 O8) | **D Establishment bridge (O1 O2 O4 O3 O7 O5 O6 O8)** |
|---|---|---|---|---|
| Smallest honest second-scenario delta | Medium–High (pre-forest site state plus first multi-scenario save at once) | **Low–Medium** (authored start plus diagnostics on existing three species) | High (wet-site state plus alder/birch/willow) | **Low–Medium** (same as B) |
| Distinct new dilemma per chapter | High | Medium (O1→O2→O3 are three marking chapters in a row) | High | **High** (marking, marking, planting, marking+health, risk, water, continuity, portfolio) |
| Transfer chain | Medium (O2 arrives late, after unrelated site lessons) | High for craft; weak at O3→O4 | Low (CCF craft returns only at position 7) | **High** (O4 establishment feeds O3 reconstitution and O7 reforestation) |
| Novice overload at position 2 | Medium–High | Medium–High (reading cohorts) | High | Medium–High (same O2) |
| Novice fatigue across 1–4 | Low | **High** (three consecutive selective-harvest chapters) | Low | Low |
| Front-loads unimplemented systems | Yes (site/establishment at 2, ash at 3, wet at 4) | Ash at 3, before establishment exists | Yes (wet at 2, invasives/continuity at 3) | Establishment at 3, ash at 4 |
| Fit for small private / farm owner (D-053) | **Highest early** | Medium (irregular working CCF is an uncommon Irish private starting point; reviewer judgement, not measured) | Medium for conservation owners | Medium–High (establishment at 3) |
| False-precision temptation | Medium–High | Medium–High | Very High | Medium–High |

### 3.2 A — Beginner-to-owner

**Skill progression:** transformation → establishment → rehabilitation and disease → water → maintenance → continuity → risk → portfolio.

**Strengths**
1. **O1→O4 gives the clearest new dilemma of any position-2 choice.** "Should this land become woodland at all?" cannot be mistaken for Scenario One.
2. **Audience fit.** Farm afforestation and protecting young trees are concrete D-053 situations.
3. **O6→O7 restraint transfer.** Moving from heritage restraint to "salvage is not automatically best" is a good adjacent pairing.

**Weaknesses**
1. **Two new big systems at once at position 2.** The first multi-scenario identity/save contract *and* a pre-forest site/habitat/establishment state would both land on the first post-O1 case. That doubles the risk for the first second-world proof.
2. **O3 at 3 puts ash dieback (disease state plus contractor safety) straight after O4.** That gives three novel systems by chapter 3.
3. **O5→O2 interrupts CCF.** The core craft (O2) waits until position 5, after two site-constraint chapters. The game's premise fades.
4. **O7 at 7** delays the case where dormant StormModel1 offers real reuse.

### 3.3 B — Forestry/CCF first (researcher recommendation)

**Strengths**
1. **Lowest-delta second world.** O2 can use the three existing simulation-ready species, marking, Work Plan, annual advance and Annual Review. The only new heavy item is scenario identity plus an authored start (§6).
2. **Clean transfer test.** "Transform" (O1) versus "maintain" (O2) asks whether O1 learning transfers without new controls.
3. **O7 at 5 uses existing dormant StormModel1 and the Sitka pure-stand models**, which are evidence-appropriate for a conifer upland case.
4. **O3 and O6 are separated** by three chapters, which reduces broadleaf déjà vu.

**Weaknesses**
1. **Three consecutive selective-harvest chapters (O1→O2→O3).** #428 itself names this (C3, C5) but does not resolve it. In practice a player marks crop and fell trees in mixed Oak/Beech/Sitka in O2, then marks crop and fell trees in Oak/Beech in O3. The scenery changes but the verb does not.
2. **O3→O4 order inverts a dependency.** O3's ash reconstitution means replanting replacement species on cleared ash patches, which is an *establishment* problem. In B that comes before the establishment system (O4) exists, so O3 must either build establishment early or hand-wave reconstitution.
3. **O2's credibility risk.** Irregular mixed-stand long-run behaviour sits on even-aged Sitka calibration plus ordinal Oak/Beech occupancy (D-061). Any numerical sustainable-yield readout would exceed evidence. #428 states this clearly, which is to its credit.
4. **Audience mismatch at position 2.** Inheriting an already functioning irregular CCF forest is uncommon for Irish private owners. This is reviewer judgement and should be checked against the Irish transformation evidence pack (ContinuFOR/TranSSFor) before it is relied on. O2 is therefore better framed as "the destination O1 is aiming for" than as a typical owner situation.

### 3.4 C — Ecology/site first

**Strengths**
1. **Strongest early contrast**: river valley and old oakwood against Sitka rows.
2. **O6 before O4** teaches that planting cannot buy continuity, which is a genuinely good ordering insight.
3. **Restraint and no-work are legitimised early.**

**Weaknesses**
1. **Highest early debt.** Wet-site state, alder/birch/willow (birch and alder are on HOLD; willow is absent), rhododendron state and continuity would all arrive before any second world exists.
2. **CCF craft disappears for positions 2–6.**
3. **Very high false-precision temptation.** Flood, water quality, epiphyte and invasion numbers are easy to fake and hard to defend.
4. **O3→O2 at 6–7** puts the two most similar marking chapters back to back, late.

### 3.5 D — "Establishment bridge" (O1, O2, O4, O3, O7, O5, O6, O8)

**Is it cosmetic?** No. It moves one case (O4 from 4 to 3), and the move carries three non-cosmetic effects:

1. **Engineering dependency.** O4 builds the establishment pathway (planting on open ground, site/no-plant masks, juvenile survival over years). O3 ash reconstitution and O7 post-storm reforestation both need that pathway. In B, O3 needs it before it exists.
2. **Breaks the marking run.** The sequence becomes mark, mark, **plant**, mark+health, risk, water, continuity, portfolio. No two adjacent chapters share the same primary verb.
3. **Delays the first disease system (ash) by one chapter**, so the first two post-O1 cases add no new ecological process at all. O2 adds an authored start plus diagnostics. O4 adds site masks plus establishment using already simulation-ready Sitka/Oak/Beech (§4.1).

**Costs and risks of D**
- O4 at 3 still needs pre-forest/no-plant state and an establishment cost path earlier than in B. That is the same total work, moved one slot earlier.
- O4 with only Sitka/Oak/Beech is an honest *minimum*. Irish afforestation mixes commonly combine conifer and broadleaf, so this is plausible. A richer native palette, and any nurse species, still waits for species calibration.
- If the O2 proof fails (irregular start not credible without speculative yield), D and B fail together. The fallback is **O1, O4, O2, O3, …** (an A/D hybrid). It is **not** C.

**Should O8 always be eighth?** Yes. O8 is the only case that needs estate-internal shared budget state, which is the highest-risk save change. Moving it earlier would front-load the riskiest architecture before the single-world systems are proven. What should change is O8's *shape*, not its position (§4.4).

### 3.6 Required overlap checks

| Pair | Risk | Required unique observable decision |
|---|---|---|
| **O1 / O2** | High. O1 ends around Year 25 at canopy ~0.35 with Oak/Beech juveniles, and its Year-100 review already shows a semi-irregular structure. | O2 must open with **no rack planning and no transformation tutorial**. The dilemma is "harvest a mature seed tree that is the only source above a thin successor cohort, take its neighbour, or wait", read through a **size-class/cohort readout derived from existing tree state**. If that readout does not exist, O2 is O1 with different trees. |
| **O2 / O3** | High in B, low in D. | O3's unique dilemma is **tree quality and health triage**: release a good-form oak, retain a hollow veteran, route dying ash to a contractor or monitor it. A pure crop-tree selection O3 without ash triage duplicates O2 (I agree with #428 §3 here). |
| **O3 / O6** | Medium. Oak, veterans and browsing appear in both. | O6's unique state is **rhododendron regrowth plus seedling-to-sapling stall plus authored, irreversible continuity**. If O6 ships without causal rhododendron state, it collapses into "O3 with moss". |
| **O4 / O2** | Low. | O2 is natural successors under canopy. O4 is planted or colonising trees on open ground with no canopy. Different state, different verbs. |
| **O5 / O7** | Medium. Both restrict access. | O7's cause is **wind/rooting/exposure plus an event**. O5's cause is **soil wetness plus traffic damage plus buffers**. Both must be different persisted states. A shared "access restricted" flag with two skins would be duplication. |
| **O8 recap risk** | High. | O8 must have **one budget/labour constraint that forces leaving a known problem untreated**, and an Annual Review that shows the consequence of that omission. If every compartment can be treated, O8 is a checklist. |

### 3.7 Feature and risk blockers for all eight (and whether each order front-loads them)

| Case | Blocking unimplemented systems (status on main, static inspection) | A pos | B pos | C pos | D pos |
|---|---|---|---|---|---|
| O1 | None new; #424 qualification gates only | 1 | 1 | 1 | 1 |
| O2 | Scenario identity/save routing (§6); authored multi-age start; cohort/size-class readout | 5 | 2 | 7 | 2 |
| O3 | Ash species identity (absent) plus authored health stage plus contractor route; veteran register; reconstitution planting (needs O4) | 3 | 3 | 6 | 4 |
| O4 | Pre-forest/no-plant masks; open-ground establishment and weed competition (grass competition not verified); establishment costs | 2 | 4 | 4 | 3 |
| O5 | Wet-soil/traffic state; alder/birch (HOLD), willow (absent) | 4 | 6 | 2 | 6 |
| O6 | Rhododendron state; holly/understorey; continuity register | 6 | 7 | 3 | 7 |
| O7 | StormModel1 activation (dormant; Overview requires standalone Player profiling before activation); exposure/peat; salvage; reforestation (needs O4) | 7 | 5 | 5 | 5 |
| O8 | Estate-internal budget and compartments | 8 | 8 | 8 | 8 |

**Front-loading verdict:** C front-loads hydrology, species and invasives. A front-loads establishment, disease and hydrology. B front-loads disease before establishment. D front-loads nothing beyond O2/O4's own minimums.

**Note on O7 and the build gate.** The Unity Overview says StormModel1 activation needs standalone Player and century profiling. The first Windows/Player build is gated on the owner's personal Scenario One playtest and explicit authorisation. O7's activation is therefore chained behind that gate. This is a further reason not to pull O7 earlier than position 5.

### 3.8 Does the chosen sequence serve the small private / farm owner better than a model-first curriculum?

Partly. D puts establishment (the most common farm-owner forestry act) at position 3, rather than 4 (B) or 2 (A). It keeps O2 second, which is a **model/engineering-first** choice. That choice is justified by second-world risk, not by audience. If the owner weights audience above engineering, A or the A/D hybrid (O1, O4, O2, O3, …) is the better pick, at a higher early cost. This is an owner decision (§5, P-1).

---

## 4. Nurse, coppice, fruit/nut and O8 critiques

### 4.1 Simultaneous nurse + target establishment (Pommerening variant)

**Method precision (preserved):** the owner's referent is fast temporary nurse and slower long-term target **planted at the same time on bare land**. There is no shelter on day one. Shelter grows with the nurse, and some nurses are selectively removed before maturity. This is one route among four: (i) pioneer-first then underplant later; (ii) direct target mixture with no temporary nurse; (iii) natural colonisation, or leaving valuable open habitat unplanted; plus the simultaneous route itself. The 2023 webinar wording was **not** transcribed by this reviewer either. No quotes are attributed to it.

**Source evaluation**

| Source | What it supports | Limit / correction |
|---|---|---|
| **Guest & Huss, COFORD Connects (2012), *Nursing of oak in Ireland*** ([PDF](https://www.coford.ie/media/coford/content/publications/projectreports/cofordconnects/SM21-LR.PDF)), read via automated page extraction 12 Nov 2026 | Irish stands with oak and nurse conifers established together. Scots pine in alternate lines often grew too slowly on heavy soils, giving little shelter (pine reportedly 0–0.8 m taller than oak after 9 seasons). European larch dominated (reportedly 1.4–2.8 m taller) and suppressed oak in places. Late removal of Norway spruce bands led to conifer dominance. Recommends bands of 3–4 oak lines. Suggests common alder (wet soils) and birch as alternative nurses. Timely primary thinning is critical. | **Correction to #430:** the extracted text states Scots pine is "not a suitable nurse species for oak when both species are planted at the same time". It suggests planting the nurse several years before the oak, framed as a question needing research rather than a tested result. So the main Irish source supports the two-way failure risk, **and also leans toward pioneer-first for pine**. It does not reject simultaneous planting generally (larch in robust bands remains suitable). Simultaneous success is **species- and site-conditional**. The figures are from a few historic sites, not rates. Exact page and figure references should be rechecked against the PDF by the Manager. |
| **Pommerening & Murphy (2004)**, *Forestry* 77:27–44, DOI 10.1093/forestry/77.1.27 | Conceptual review naming nurse-crop establishment for CCF afforestation/restocking. | Not re-read in full by this reviewer. Conceptual only. |
| **Martiník et al. (2024)**, *J. For. Sci.* 70:492–500 ([abstract](https://agriculturejournals.cz/artkey/jfs-202409-0004_preliminary-results-from-simultaneous-planting-of-fagus-sylvatica-and-pioneer-species-on-calamity-clearings.php)) | Czech, four plots, two years, beech with birch or aspen planted simultaneously. A positive effect appeared only where pioneer stock was much taller (abstract: ~100 cm against ~39 cm). Cost was lower or higher depending on the comparison density. | Continental, short-term and preliminary. Supports **conditional** benefit only. No effect size transfers. |
| **CCFG UK guidance (2022)** | Both staged and simultaneous approaches are considered. | UK practitioner guidance. Not re-read by this reviewer. |
| **2023 webinar notice** | Confirms the topic was discussed. | Wording unverified. |

**Product questions**

1. **Necessary in O4's first complete release?** **No. Make it OPTIONAL / a later O4 challenge.** Reasons:
   - (a) The suitable Irish simultaneous nurse for oak per COFORD is larch in bands, with birch or alder as alternatives. Larch is **absent** from main. Scots pine, downy birch and common alder exist only as **HOLD, not simulation-ready** assets (`Docs/ScenarioTwoSpeciesFoundation.md`; `Assets/ForestPrototype/Species/ScenarioTwo/`): growth and juvenile light are on HOLD, and they are not registered in any scene or save.
   - (b) Using Sitka spruce as a stand-in nurse is not supported by any reviewed source.
   - (c) There is no positive shelter mechanism on main.
   - O4's first release can teach "site before species", no-plant habitat, and direct planting versus natural colonisation with the existing three species. That is honest and already distinctive.
2. **Minimum authoritative causal model, if adopted later:**
   - persisted per-tree **role/intent** (temporary nurse versus intended crop), distinct from Crop Tree and Fell marks;
   - a simulation-ready nurse species with juvenile height/crown growth on the planted site class;
   - existing light/crown competition applied between nurse and target;
   - a **conditional positive shelter term** only where a spec defends it (for example, a reduction in exposure or ground-vegetation competition that depends on nurse height and proximity), labelled as gameplay abstraction with its transfer class;
   - staged nurse removal as Work Plan orders with labour and cost;
   - save identity and history recording what was done, not inferred intent;
   - a render tied to the state;
   - recorded causal limits shown in Annual Review;
   - an evidence gate showing **measurably different authoritative tree states under at least two removal histories** (agrees with #430).
3. **Smaller evidence-honest version?** Only partly. Modelling competition alone, with "shelter not modelled" shown as text, would teach half the dilemma and **bias players toward early removal**, which is misleading. **Do not ship the half model.** The release-timing skill (open crowns before suppression) is already taught honestly by crown release in O2 and O3. The nurse-specific twist should wait until both halves can be modelled.
4. **What current Sitka MFM or juvenile growth cannot do:** carry role intent; grow larch, or pine/birch on HOLD; produce any positive facilitation; apply MFM sample-to-comparable prescriptions to nurse/oak mixtures (MFM is Sitka-guarded and parallel-rack derived).
5. **Research recommendation versus code approval:** the research recommendation is to keep the simultaneous variant as a named, species-conditional O4 option. **Code approval: none.** A four-file spec is required only when O4 is scheduled.

### 4.2 Coppicing

**O3 load before coppice:** Oak/Beech form and competition, crown release, ash dieback triage, contractor safety and reconstitution, and veteran/habitat retention. That is already four new strands, of which two (ash, veteran register) need new persisted state. Adding a stool mechanic makes **five strands with three new persisted systems**, and O3 becomes an incoherent list. I agree with the Manager caution and go further: a required coppice mechanic in O3's first release should be rejected.

**Evidence check (Teagasc WIS Element 4, read via automated extraction 12 Nov 2026):** the page frames coppice as **woodland improvement for grant-aided broadleaf/mixed forests**. It requires a site-specific plan and pre-harvest marking of standards. It lists deer management, protection, invasive clearance and **singling stumped trees to "one strong straight stem per stump"**. It describes free-draining mineral soils as best suited, and it requires that WIS Element 1 (thinning) has already been received. It **names no species, hazel included**, and gives no rotations. The €1,200/ha figure is a 2023–2027 policy snapshot. **Correction to the addendum:** its hazel-coppice vignette is a design choice. It is plausible from the NPWS hazel guidance it cites (not re-read by this reviewer), but it is **not** supported by the Teagasc page. The NPWS 2017 Native Woodland Management Guidelines (cited, not re-read here) are the basis for "old stools may fail" and for sensitivity in old woodland.

**Placement comparison**

| Policy | Learning | Cost / risk | Verdict |
|---|---|---|---|
| 1. Required causal coppice in O3 first release | Strong unique skill (vegetative resprouting) | Stool identity, multi-shoot growth, cut/viability, browse on shoots, smallwood product and render all arrive in an already overloaded case. Highest save risk. | **Reject** |
| 2. Optional bounded O3 fork after core rehabilitation and ash | Good | Still needs the **whole** stool system even if few players use it. An optional feature costs the same to build and qualify as a required one. | Viable later, as an O3 update after O3 core qualifies |
| 3. Defer the real mechanic to O8; teach observation earlier | Weaker early, honest | Lowest early cost. O8 gains a real "repeat-labour rotation versus mature timber" budget trade-off. | **Recommended default** |

**Lower-cost authentic alternative for O3:** author a **former coppice patch as observable history**. Multi-stem clumps would be rendered as fixed authored structures with no regrowth simulation, plus an Annual Review/inspection fact ("overstood hazel coppice; restoration would need deer control and repeat labour"). The player can record a decision: retain as habitat, flag for future restoration, or leave. No regrowth is claimed. This keeps coppice visible as a future teaching skill without silently rejecting it. Singling to high forest needs multi-stem identity too, so it is deferred with the stool system.

**O6 counterexample:** keep it. It costs nothing if O6 simply presents old stools and veterans as do-not-cut continuity features.

### 4.3 Fruit/nut boundary (D-054)

**Permissible Stage 1 forestry:** recognising hazel, wild cherry or other nut/fruit-bearing native trees as **forest species**; planting design and protection; competition; smallwood (only once coppice exists); and "future opportunity" text.
**Not Stage 1:** flowering or fruit set, nut or fruit yields, harvest, processing, sales, orchard pruning, or a forest-garden profit and loss. Teagasc 2026 forest-garden (40%) and FT8 (15%) ratios are scheme snapshots, not ecology.

**Choices the owner must make so Stage 2 does not drift forward silently:**
1. Does any fruit/nut-bearing species enter the Stage 1 **planting palette** at all? That means species-model work at D-059 depth, not just a label.
2. If not, is recognition limited to **static inspection facts or props** with no growth role? (Cheapest; recommended for first releases.)
3. Is any **produce number** ever shown in Stage 1? Recommended: never, until Stage 2 is accepted.
4. Does O8 show fruit/nut patches only as text opportunities? Recommended: yes.

### 4.4 O8 estate capstone

- **Manageable as specified?** Only if it is finite and small. "Optimise many worlds" invites a generic property-management simulator and a new multi-world save schema, which is the highest-risk item in the programme.
- **Reviewer alternative (independent):** build O8 as **one forest world containing three or four map-zoned compartments** (commercial mixed-age, broadleaf, riparian strip, exposed conifer). It would have one save, one cash and labour pool, and per-compartment history tags. This reuses the single-world save model and the existing map, and avoids multiple running worlds. Each compartment may use only systems already qualified in earlier cases.
- **Anti-recap rule:** the budget must force at least one known problem to go untreated, and the consequence must be reviewable (§3.6).
- Eight separate scenario saves remain independent (D-078). The O8 budget is local to O8.

---

## 5. Recommended order, runner-up and owner decision points

**Recommended:** **D — O1, O2, O4, O3, O7, O5, O6, O8**
1. O1 Inherited Sitka Plantation (fixed)
2. O2 Working Mixed Forest
3. O4 New Farm Woodland
4. O3 Neglected Broadleaf Wood
5. O7 Exposed Upland Forest
6. O5 Wet Woodland and River Valley
7. O6 Atlantic Oakwood
8. O8 Complete Irish Forest Estate

**Runner-up:** **B — O1, O2, O3, O4, O7, O5, O6, O8.** Fallback if the O2 proof fails: **O1, O4, O2, O3, O7, O5, O6, O8.**

**Confidence caveats:** moderate at best. This is a qualitative ranking from static source inspection and documents. There has been no playtest, no novice usability study, no cost estimate and no Unity run. D's margin over B is one swap, justified by the establishment dependency and verb alternation. A real novice test of O2 could reverse the position-2 choice.

**Owner decision points**

| # | Decision | Reviewer recommendation |
|---|---|---|
| P-1 | Final order | D, runner-up B. O8 stays 8th. |
| P-2 | O4 simultaneous nurse | **OPTIONAL / later O4 challenge**, not in O4's first release. Revisit when a nurse species (larch, birch or alder) is simulation-ready. |
| P-3 | O3 coppice | **DEFERRED** real stool mechanic (O8 or a later O3 update). O3 gets observable coppice history only. |
| P-4 | Stage 1 fruit/nut | **REQUIRED boundary:** recognition only, with no produce numbers. Palette inclusion is a separate later choice. |
| P-5 | O8 shape | Finite, one world, three or four zoned compartments, one estate-local budget. |
| P-6 | Ash/storm/wet/invasive fidelity | Authored deterministic state plus causal minimum per case; no stochastic Irish rates. |

Concept-level classification: **REQUIRED:** O3 ash triage (core of O3), O4 no-plant option, the O8 forced-omission rule, the fruit/nut recognition boundary. **OPTIONAL:** O4 simultaneous nurse, O3 coppice fork (later), the O6 coppice counterexample. **DEFERRED:** real stool/regrowth, Stage 2 fruit/nut production, generic estate/sandbox.

---

## 6. Earliest credible playable new-scenario proof and architecture gate

### 6.1 Smallest honest second-scenario proof (O2 under B or D)

**What the user sees:** from the session menu, the player starts "Working Mixed Forest" without touching their Scenario One save. They walk an authored irregular Sitka/Oak/Beech stand that already has access, so there is no rack planning. They mark retention or removal around one mature seed tree and commit the Work Plan. They advance 1 and then 5 years. In the walk and the Annual Review, a size-class/cohort panel derived from live tree state shows successors either recruiting or failing. They then save, quit, load Scenario One (unchanged), and load O2 (unchanged).

**Not in the proof:** yield forecasts, new species, MFM, storms, or estate features.

### 6.2 Architecture dependency gate (static inspection of main `bc38ff2`)

- `ForestSaveController.cs` writes a **single fixed** `forest-save.json` (with `.tmp` and `.bak`) under `persistentDataPath`. A second scenario on the current code path would **overwrite the Scenario One save**.
- `ScenarioOneManager.cs` (around lines 1481–1482) **only logs a warning** when a save's `scenarioId` differs from the loaded definition, and then continues restoring. Cross-scenario loads are not rejected before mutation.
- `ForestSaveData.CurrentVersion = 24`, with all management state under `scenarioOne`.
- **Gate before any S2 code:**
  - (1) per-scenario save location or slot, keeping Scenario One's current path byte-identical;
  - (2) cross-scenario load **rejection before mutation**;
  - (3) old saves v1–v24 route to Scenario One only;
  - (4) Legacy40 and frozen Reference Future v1 untouched;
  - (5) narrow new/old/cross-load negative tests.
- Do **not** generalise `ScenarioOneManager` into a framework before S2 shows real duplication (agrees with #428 §6).

### 6.3 Serial versus parallel ownership

- **Serial:** playable delivery (D-078); save/controller/ecology/scene edits; any Unity lease (one per machine).
- **Parallel and safe:** research/spec/art for the next case on disjoint files under claims. For example, O4 site-mask research while O2 is built, ash evidence while O4 is built, and nurse-species calibration research at any time as research only.
- #424 keeps Scenario One, Unity and `active-tasks.md`. No S2 implementation starts before S1 is accepted under its own gates.

---

## 7. Research review verdict

**Overall verdict: NEEDS CHANGES.** The research is usable as input. The corrections below should be made, or acknowledged, before the owner decision memo.

| Sev | Where | Finding | Correction required |
|---|---|---|---|
| **MAJOR-1** | #430 "Primary and related source evidence" COFORD row and "Genuine alternative routes" A; Manager #431 §3.1 | COFORD (2012) is presented as Irish support for simultaneous planting with two-way risk. The extracted source text also states Scots pine is not a suitable nurse for oak when planted at the same time, and suggests nurse-first planting (as untested). | State that simultaneous planting is species-conditional. Name larch-in-bands as the Irish simultaneous example, and birch or alder as proposed alternatives. Recheck exact pages. |
| **MAJOR-2** | #428 §5 "O2–O8 scenario identity" row and §6 "First real post-S1 boundary" | Does not identify that main has one fixed save file and warn-only scenario mismatch. Treats the cross-load policy as "to audit". | Name the single-slot overwrite and warn-only restore as the first concrete S2 blocker (§6.2). |
| **MAJOR-3** | #428 §5 O5 species row ("Not shipped"/research-only framing); original report "Species priority"; #430 "Existing production reuse" | Misses that Scots pine, downy birch and common alder already exist on main as HOLD, not simulation-ready, assets with a species-readiness policy. Larch, the key COFORD nurse, is absent. | Record actual species status. Cost the O4 nurse and O5 work against "HOLD → calibrated", not "from nothing". |
| **MAJOR-4** | #428 §2 B, transitions 2→3 and 3→4, cons C3/C5 | The O1→O2→O3 marking run is acknowledged but unresolved. The O4→O3/O7 establishment dependency is missed. | Evaluate D (or justify B against it). Show how O3 reconstitution is delivered before establishment exists. |
| **MAJOR-5** | Addendum "Irish evidence and constraints" item 1; consolidated handoff §4 | Teagasc WIS Element 4 is cited for a hazel coppice lesson but names no species. It frames coppice as broadleaf woodland improvement after first thinning, on free-draining mineral soils, and includes singling. | Re-attribute the hazel framing to NPWS (verify pages) or label it as design. Note the scheme facts and the snapshot status of the grant. |
| MINOR-1 | #428 §2 B pros/cons | No audience check on how common irregular CCF is among Irish private owners. | Check the Irish transformation evidence. Frame O2 as the "destination" case. |
| MINOR-2 | #428 §6 gate G5 | StormModel1 activation's standalone Player profiling precondition interacts with the Windows build gate. Not stated. | Add the dependency. |
| MINOR-3 | #428 §4 O8 row; Manager #431 P-5 | Compartments as separate records is the only O8 shape considered. | Consider one world with zoned compartments (§4.4). |
| MINOR-4 | Addendum O3 row | An optional fork is costed as if it were cheaper than a required one. | Same build and qualification cost. Say so. |

**Did this critique independently find the Manager #431 concerns?** Yes for O3 overload, O1/O2 overlap, O8 recap and the engineering bias in B. The Manager review did **not** raise MAJOR-1, MAJOR-2, MAJOR-3 or MAJOR-5.

**Evidence classes used:** accepted intent (Decision Log, Brief); inspected repo source at main `bc38ff2` (save controller, manager, species assets; static only); Unity/Editor verified: **none, not run**; Irish empirical observation (COFORD 2012, a few historic sites); Irish management guidance (Teagasc WIS, NPWS 2017); continental transfer (Martiník 2024); gameplay abstraction (any shelter term, cohort readout, horizons); unsupported, needing a decision (all rates, ratios, removal years, stool growth, storm or mortality probabilities, fruit economics, biodiversity scores).

**Why none of these give perpetual game coefficients:** COFORD 2012 describes a few historic site outcomes, not replicated rates. Teagasc and NPWS are management guidance and time-bound grant rules. The 2026 forest-garden ratios are scheme eligibility limits. None measures growth response, survival probability or yield. No exact storm, mortality, fruit-economics or biodiversity number in any reviewed document is supported by the cited literature, and none should be inferred.

---

## 8. Sources, done / not done

**Read (GitHub):** main [Decision Log](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md), [Game Brief](https://github.com/jermur788/CCF/blob/main/Docs/Project/game-brief.md), [Current Milestone](https://github.com/jermur788/CCF/blob/main/Docs/Project/current-milestone.md), [Unity Overview](https://github.com/jermur788/CCF/blob/main/Docs/Project/unity-project-overview.md), [ScenarioTwoSpeciesFoundation](https://github.com/jermur788/CCF/blob/main/Docs/ScenarioTwoSpeciesFoundation.md), `Assets/ForestPrototype/ForestSaveController.cs`, `ForestSaveData.cs`, `ScenarioOne/ScenarioOneManager.cs`, `Species/ScenarioTwo/ScotsPine.asset`; [original eight](https://github.com/jermur788/CCF/blob/docs/independent-eight-irish-forest-scenarios-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/INDEPENDENT-EIGHT-IRISH-FOREST-MANAGEMENT-SCENARIOS-20261112.md); [#428](https://github.com/jermur788/CCF/blob/docs/eight-scenario-curriculum-refinement-result-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-CURRICULUM-DEPENDENCY-REFINEMENT-RESULT.md); [#430](https://github.com/jermur788/CCF/blob/docs/pommerening-instant-new-ccf-research-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/POMMERENING-INSTANT-NEW-CCF-BARE-LAND-RESEARCH-20261112.md); [addendum](https://github.com/jermur788/CCF/blob/docs/eight-scenario-coppice-fruitnut-addendum-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/EIGHT-SCENARIO-COPPICING-FRUIT-NUT-PLACEMENT-ADDENDUM-20261112.md); [#431 handoff](https://github.com/jermur788/CCF/blob/docs/overall-manager-eight-scenario-direction-handoff-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/OVERALL-MANAGER-EIGHT-SCENARIO-DIRECTION-AND-ADDITIONS-HANDOFF-20261112.md); [Manager #431 review](https://github.com/jermur788/CCF/blob/docs/overall-manager-431-curriculum-critique-prep-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/MANAGER-431-EIGHT-SCENARIO-ADDITIONS-REVIEW-AND-PRODUCT-DECISIONS.md) (read last); main `active-tasks.md` (read-only).

**Primary sources checked (automated page extraction, 12 Nov 2026):** [COFORD 2012](https://www.coford.ie/media/coford/content/publications/projectreports/cofordconnects/SM21-LR.PDF); [Teagasc WIS Element 4](https://teagasc.ie/crops/forestry/grants/woodland-improvement-scheme-2023-2027/wis-element-4-coppice-and-coppice-with-standards/); [Martiník et al. 2024](https://agriculturejournals.cz/artkey/jfs-202409-0004_preliminary-results-from-simultaneous-planting-of-fagus-sylvatica-and-pioneer-species-on-calamity-clearings.php).
**Not checked by this reviewer:** NPWS 2017 guidelines, Pommerening & Murphy 2004 full text, CCFG guidance, the webinar recording, the Teagasc forest-garden pages, the project's uploaded PDFs.

**Done:** preflight (packet head `2e28bce`, main `bc38ff2`, result branch and path absent, no conflicting PR); result branch created; #432 marked RUNNING with actual model and effort; canon and original report read before the research; provisional order recorded; A/B/C/D critique; nurse, coppice, fruit/nut and O8 critiques; verdict; this single file.
**Not done:** no code, Unity, Editor, build, save schema, spec, main or canon edits, no `active-tasks.md` edit (#424 sole writer; #432 is not registered there and this reviewer did not add it), no dispatch, no product decision. **Dashboard/Drive: NOT VERIFIED.**
