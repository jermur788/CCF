# Recommended scenario sequence

**Status:** RECOMMENDATION. Working titles only.

## 1. The sequence

**Core spine (six scenarios)** — each requires the reasoning of the previous ones.

| # | Working title | One-line purpose |
|---|---|---|
| 1 | **First Steps in CCF** (exists) | Choose what to keep, release it, start a new generation, come back once and decide again |
| 2 | **The Inherited Stand** | Read someone else's management from the forest; reassess; decide where *not* to work; invest in Crop Trees |
| 3 | **Right Tree, Right Place** | Species strategies meet site: what belongs where, and what is already trying to grow |
| 4 | **The Next Generation** | Diagnose why young trees succeed or fail; choose a protection and regeneration strategy |
| 5 | **The Irregular Forest** (graduation) | Manage an already-irregular forest without turning it back into a plantation |
| 6 | **The Farm Woodland** (integrated) | A realistic small holding with mixed sites, species, history, cash and risk; no focus lesson |

**Branch scenarios** — optional, unlocked by knowledge rather than required for the spine.

| # | Working title | Unlocked after | Purpose |
|---|---|---|---|
| B1 | **After the Storm** | Scenario 2 | Inherit a windthrown Sitka stand; salvage vs retention vs regeneration vs stability |
| B2 | **The Old Oakwood** | Scenario 4 | Semi-natural woodland with an ecological objective; restraint, browse at scale, open habitat, deadwood |

**Training (not a scenario):** plan comparison inside scenarios from Scenario 1; a separate Training Stand mode after Scenario 1 (`ScenarioAuthoringNeeds.md` §4).

**Sandbox:** after Scenario 6 is built; unlocked to players after Scenario 5 is completed (`ScenarioAuthoringNeeds.md` §3).

## 2. Scenario matrix

| Scenario | Starting forest | Assumed knowledge | Main lesson | New systems | Reused lessons | Assistance level | Completion evidence | Implementation dependency |
|---|---|---|---|---|---|---|---|---|
| **1 First Steps** | Even-aged Sitka, ~20 yrs, 0.16 ha, Class III, uniform site | None | Positive selection, release, light → regeneration, planting/protection, coming back | None beyond the P-series (P3 residual, P4 history, P5 progression, P6 second cycle) | — | **High** throughout; reduced for cycle 2 | Two release cycles ≥ 5 yrs apart; cover and capital held; renewal under way (model D, PROPOSAL) | P-series completion (in progress) |
| **2 Inherited Stand** | Sitka-dominated stand ~15–20 yrs into transformation by a previous owner: good and poor past choices, a few planted oak/beech (some sheltered, some browsed), patches of bramble, advance Sitka regeneration under a closing canopy, one over-opened patch | Scenario 1 | Read history from the forest; reassess past decisions; release advance regeneration; decide not to intervene where nothing is needed; pruning as investment | **Scenario package** (start snapshot + definition + scenario id); inherited-history boundary; per-scenario completion profile; assistance tier; "no work needed" decision record | C3–C5, C10–C13, C22 | **Medium**: focus (history, reassessment) high; selection/thinning reduced | ≥ 2 decision cycles (one may be a recorded "no work here" in at least one area); history consulted; cover and capital held; advance regeneration status changed by a player decision | Current simulation + small extensions only |
| **3 Right Tree, Right Place** | Mixed-age conifer stand on a farm with real site variation: wet gley hollow, drier knoll, exposed edge; Sitka with some Scots pine; birch colonising openings; alder in the wet hollow | 1–2 | Species strategies × site axes; seed source; mixtures not automatically better | **Multi-species foundation** (species tags, shade casting/occupancy, species-aware density), **site map** (moisture, fertility, exposure; ordinal), Scots pine, downy birch, common alder; Douglas fir as a planting option | Positive selection now across species; regeneration from light; planting; history | **Medium**: species/site high; others reduced | ≥ 2 cycles; at least one enrichment or regeneration decision on each of ≥ 2 site types; species–site outcome observed after ≥ 5 yrs; no completion requirement on species mix | W0 + W1-lite + GROWFOR extraction research (Scots pine, Douglas fir) |
| **4 Next Generation** | Older mixed stand ready to regenerate: seed sources uneven by species, high deer pressure, heavy vegetation in some gaps, beech advance regeneration under canopy, rowan held back by browse | 1–3 | Diagnose *why* regeneration fails (seed / light / vegetation / browse / site) and choose a strategy (shelters, fencing, planting, accepting, waiting) | Fencing gameplay (D-044), species-specific browse classes, rowan (and holly as understorey shade), "evidence not verdict" diagnosis tier | Species/site, light, vegetation, planting, economics | **Medium-low**: diagnosis gives evidence, not conclusions | ≥ 2 cycles; recruitment established in ≥ N places by ≥ 2 different routes chosen by the player; protection cost/benefit observed; no prescribed method | W1 (rowan, holly ordinal), fencing, diagnosis tiering |
| **5 Irregular Forest** (graduation) | Established irregular, multi-aged mixed conifer–broadleaf forest (Sitka, Douglas fir, Norway spruce, oak, beech, birch) with recruitment in several layers | 1–4 | Keep a forest irregular: what to harvest, where to open, how to keep recruitment, quality-led selection, regular small harvest income | **Timber quality state** (form class, browse history), species assortments/markets for DF/NS (no fabricated prices), layer diagnostics, Norway spruce | Everything | **Low**: evidence and consequences only | ≥ 3 cycles; irregular structure *maintained* (≥ 3 layers present throughout, recruitment into each); solvency; no single target distribution | W2, W3 (DF/NS), W4 quality |
| **6 Farm Woodland** (integrated) | A realistic small holding: two or three compartments with different sites, species, histories (one neglected, one partly converted, one young broadleaf planting), finite cash, background storm and browse risk | 1–5 | Integrated judgement: priorities across compartments, sequencing, liquidity, risk | None new (content only); storms on at Model 1 profile; site-dependent exposure if available | Everything | **None prescriptive**; full information a real manager could obtain | Owner-set priorities reviewed against outcomes; ≥ 3 cycles across compartments; solvency; no forest-shape target | All previous waves |
| **B1 After the Storm** | Sitka stand 1 year after a severe windthrow event: uprooted groups, a new exposed edge, deadwood, light | 1–2 | Salvage vs retention vs regeneration opportunity vs future stability, under small-job economics | Storm activation for this scenario (Model 1, frozen profile), authored disturbed start; nothing else | Release, regeneration, economics, history | **Medium** | ≥ 2 cycles; a recorded salvage/retention decision; regeneration in the gap observed; stand still standing | Scenario package (as for 2) + storm on |
| **B2 Old Oakwood** | Semi-natural oak–birch woodland with holly and rowan, heavy deer pressure, little timber value, a valuable open glade | 1–4 | Ecological objectives: when intervention is unnecessary or harmful; browse at landscape scale; deadwood and open habitat | Open-habitat state (glade value), standing deadwood, hazel optional (resprouting), possibly deer-management abstraction | Diagnosis, protection, species/site | **Low** | Owner-set ecological objective reviewed; regeneration and structure trend; open habitat not planted out; no "plant more = better" | W1 full, fencing, open-habitat state (NEW), standing deadwood (NEW) |

## 3. Part 4 — the second-intervention question

Current Scenario One completion direction (PROPOSAL, `ScenarioCompletionDesign.md` model D and `P6-SecondInterventionCompletion.md`) already requires two release cycles at least five years apart, with a second-look trigger driven by stand state.

| Option | What it teaches | Strength | Weakness |
|---|---|---|---|
| **A. Scenario One contains two full cycles** | Consequences of *my own* earlier decision | Directly closes the D-004 loop; already designed; no new content | The player only ever reassesses decisions they remember making and whose reasons they know. Second cycle risks being a repeat of the first with less help |
| **B. Scenario One teaches basics; Scenario Two is the second intervention** | Repetition as its own lesson | Clear focus | Duplicates A (which is already designed into Scenario One). Weakens Scenario One: a CCF scenario that ends after one thinning teaches that CCF *is* one thinning |
| **C. (Recommended) A + an inherited stand** | Scenario One: consequences of my own decision. Scenario Two: reading and reassessing *someone else's* decisions, with partial knowledge | Two genuinely different judgements: **evaluate my plan** vs **diagnose a forest I did not create**. The second is what real owners face when buying or inheriting woodland and is the bridge to independent management | Needs a scenario package and an authored history (small; see `ScenarioAuthoringNeeds.md`) |

**Recommendation: C.** Scenario One keeps both cycles (do not strip the second cycle out to give Scenario Two a purpose). Scenario Two is the **reassessment** scenario: it starts mid-transformation, its history is partly recorded (a previous owner's notes and work records) and partly only readable in the forest (stumps, brash, shelter tubes, browse-forked oak, a crowded patch nobody thinned). The player must judge which past decisions to continue, which to correct and which areas need nothing.

Why Scenario Two must come **before** multi-species expansion:

1. **It can be built now.** Same species, uniform site, existing ecology. It exercises the scenario-package architecture on the cheapest possible content before that architecture has to carry species and site data.
2. **It separates two kinds of new difficulty.** If species/site arrive in Scenario Two, a player who struggles cannot tell whether the difficulty is "I don't know this species" or "I don't know how to reassess a stand". Keeping Scenario Two species-simple isolates reassessment.
3. **It tests transfer early.** Positive selection learned on a uniform plantation must now be applied to a stand that is already partly irregular, with advance regeneration that may itself need releasing. If players cannot do that, species expansion will not help.
4. **It answers the expansion roadmap's risk directly**: the earlier proposal moves from Scenario One straight to mixed transformation with a site map and two or more new conifers, which needs the whole W0–W3 programme before the second scenario exists.

## 4. Comparison with earlier proposals

| Earlier proposal | Its order | What this study keeps | What it changes, and why |
|---|---|---|---|
| `ForestryExpansion/ScenarioRoadmap.md` (PROPOSAL) | 1 → Mixed transformation → Native restoration → Irregular productive → (optional) After the storm | Native restoration exists; irregular productive is the advanced scenario; storm is optional; one core reused | Inserts a species-simple reassessment scenario before mixtures; splits species/site from regeneration diagnosis; moves native restoration to a branch (heaviest new systems, tests restraint rather than core CCF); moves the storm branch earlier (cheapest to build, timely) |
| Manager's conceptual sequence (PROPOSAL) | First steps → Second intervention → Species & site → Establishing next generation/browsing → Irregular → Disturbance → Integrated | Almost all of it, including order 1 → 3 → 4 → 5 → 7 | Scenario 2 becomes *inherited-stand reassessment* (not a repeat of Scenario One's second cycle); disturbance becomes an early branch plus background risk in 5–6 rather than a late core scenario; native woodland is added as a branch |
| `ScenarioOneCompletion/PostScenarioRoadmap.md` (PROPOSAL) | Training mode → Stage 2 slice → art → Scenario Two | Training mode early | **Superseded by user direction** (packet): forestry is developed substantially before broad Stage 2. Recommend the manager marks that recommendation superseded |

## 5. Why six core scenarios and not more or fewer

- **Fewer than six** collapses either species/site with regeneration diagnosis (too many new causes at once; players cannot attribute outcomes) or irregular management with the integrated challenge (no clean graduation test).
- **More than six** in the spine starts adding "the same lesson with a new species", which the quality bar rules out. Further variety belongs in branches, training stands and the sandbox.
- The forestry-mature gate proposed in `ImplementationWaves.md` §3 (M3: at least two scenarios beyond the plantation case) is met by Scenario 3 onward; this sequence exceeds it deliberately because Scenarios 2 and 6 test judgement rather than new content.
