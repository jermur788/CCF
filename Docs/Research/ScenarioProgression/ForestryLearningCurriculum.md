# Forestry learning curriculum

**Status:** RECOMMENDATION. Concepts are listed because they change decisions a small-woodland owner makes. A concept is taught by the simulation only when the simulation actually represents it (D-020: presentation must not invent causal state).

## 1. What the curriculum is for

The target player, at the end of the core sequence, can walk into an unfamiliar small woodland and:

1. read what is there (species, sizes, layers, quality, regeneration, ground, damage, history);
2. name what is limiting the forest they want (light, seed, site, vegetation, browse, stability, money);
3. choose a treatment, *including no treatment*, and explain why;
4. predict roughly what will change, return later, and revise.

The curriculum therefore teaches **diagnosis and reasoning**, not a list of operations. Mechanics are vehicles for reasoning.

## 2. Concept inventory

Status column: **IMPL** = represented on main `869ee92` (per canonical docs); **PART** = partly represented; **PROP** = proposed in existing research; **NEW** = not represented and not yet designed.

| # | Concept | What the player must be able to reason about | Simulation status |
|---|---|---|---|
| C1 | Reading a tree | Species, DBH, height, crown, competition, (later) form and defects | IMPL (inspection card); form/defects NEW |
| C2 | Reading a stand | Density, canopy, light, basal area, stems/ha, layers, regeneration presence | PART (Annual Review, map, ground report; layer diagnostics PROP) |
| C3 | Positive selection / Crop Tree choice | Start from the trees you want to keep, not the trees you want to remove | IMPL (marks D-013; P2 competitor reasoning on main) |
| C4 | Competition and release | Which neighbours actually compete; "suppressed ≠ remove" | IMPL (P2, Hegyi breakdown) |
| C5 | Thinning intensity and residual stand | How much to remove, and what is left standing | PART (P3 residual summary on branch) |
| C6 | Observing consequences | Walking the result years later; comparing to expectation | IMPL (loop D-004; Annual Review) |
| C7 | Repeated intervention | CCF is a series of light interventions; the second look may need a different treatment | PROP (P6 second-look trigger and completion model D) |
| C8 | Management history | Reading what was done before, by whom and why; history visible in the stand | PART (events/history saved; Diary P4 PROP) |
| C9 | Deciding not to intervene | A deliberate "no work needed" is a management decision | PROP (saved "no work" event suggested; not built) |
| C10 | Light and natural regeneration | Openings create light; seed must already be present | IMPL (Regeneration Model 1/2) |
| C11 | Vegetation competition | Bramble/bracken suppress short juveniles; clearance can help, waste money or harm | IMPL (Model 2, D-049) |
| C12 | Browsing and protection | Deer hold young trees below escape height; shelters protect individuals | IMPL (pressure 0.2, shelters); fencing gameplay deferred (D-044) |
| C13 | Planting and enrichment | Adding species the stand lacks, at chosen positions | IMPL (exact planting, oak/beech) |
| C14 | Species strategies | Shade tolerance vs shade casting; pioneer vs late-successional | NEW (W0/W1 PROP) |
| C15 | Species–site matching | Moisture/drainage, fertility, exposure as separate axes | NEW (G4 site map PROP) |
| C16 | Mixtures | Who regenerates under whom; mixtures are not automatically better | NEW (W2 PROP) |
| C17 | Seed source and dispersal | Retaining seed trees of minority species; "broadleaves do not appear simply because light rises" | PART (per-species kernels exist; bird dispersal PROP) |
| C18 | Canopy layers and irregular structure | Overstorey/midstorey/understorey; diameter distribution as a description, not a target | PART (emergent; diagnostics PROP) |
| C19 | Gap, group and single-tree approaches | Different opening sizes favour different species and outcomes | PART (marking clusters makes gaps; no dedicated tool, D-010) |
| C20 | Pruning and stem quality | What you do to a tree now affects its value decades later | PART (pruning work exists; no quality state; premium deferred D-044) |
| C21 | Timber assortments and value | Size + form + species → assortment → price; biological growth ≠ merchantable value | PART (Sitka assortments only) |
| C22 | Contractor and economic constraints | Small-job minimums, grouping work, liquidity, owner labour vs contractor | IMPL (economy, minimum, owner time) |
| C23 | Deadwood and habitat retention | Retaining fallen/standing deadwood; habitat trees | PART (fallen only; standing deadwood NEW) |
| C24 | Disturbance and windthrow | Exposure, edges, storm damage | IMPL dormant (Storm Model 1, D-050) |
| C25 | Salvage versus retention | Economic recovery vs deadwood vs regeneration opportunity vs stability | IMPL dormant (salvage via grouped harvest) |
| C26 | Resilience and risk spreading | Structure, species and stability reduce exposure to single shocks | NEW (emergent at best; no score, per MixedStandDesign) |
| C27 | Long-term planning | Sequencing work over decades under cash and labour limits | PART (time + economy; no planning aid) |
| C28 | Open habitat and non-intervention value | Not every gap should be planted; some open ground is valuable | NEW (no open-habitat state) |

## 3. Learning levels

Concepts are grouped by what the player must already understand to reason about them, not by mechanic.

| Level | Name | Concepts | First taught in |
|---|---|---|---|
| L1 | **Seeing** | C1, C2, C6 | Scenario 1 |
| L2 | **Choosing what to keep** | C3, C4, C5 | Scenario 1 |
| L3 | **Making the next generation possible** | C10, C11, C12 (basic), C13, C22 (basic) | Scenario 1 |
| L4 | **Coming back** | C7, C8, C9, C20 (basic), C23 (basic) | Scenario 1 (own forest) → Scenario 2 (someone else's forest) |
| L5 | **What belongs here** | C14, C15, C16, C17 | Scenario 3 |
| L6 | **Why young trees fail** | C10–C13, C17 combined into diagnosis; C12 central | Scenario 4 |
| L7 | **Keeping a forest irregular** | C18, C19, C20/C21 (quality), C22 (strategic), C27 | Scenario 5 |
| L8 | **When things go wrong** | C24, C25, C23, C26 | Branch B1 (storm); background risk in 5–6 |
| L9 | **Restraint and ecological objectives** | C9, C23, C28, C12 (landscape) | Branch B2 (native woodland); S2 and S5 in smaller form |
| L10 | **Managing independently** | All, combined | Scenario 6 |

Not every concept needs its own explicit tutorial. C2, C6 and C27 are learned through repetition of the core loop; C26 should never be taught as a rule (no resilience score) but should be *noticeable* when a storm or browse event hits a uniform stand harder than a varied one.

## 4. Curriculum dependency graph

```text
L1 SEEING (tree, stand, walking the result)
 │
 ▼
L2 CHOOSING WHAT TO KEEP (positive selection → competition → release → residual stand)
 │
 ├──────────────► L3 NEXT GENERATION, BASIC (light, seed present, vegetation, light browse, planting)
 │                 │
 ▼                 ▼
L4 COMING BACK ◄───┘  (own forest: Scenario 1, cycle 2)
 │                     (inherited forest: Scenario 2 — reading history, reassessing, not intervening,
 │                      pruning as investment)
 │
 ├──────────────► [B1 storm branch: L8 with Sitka only]
 ▼
L5 WHAT BELONGS HERE (species strategies × site axes × mixtures × seed source)
 │
 ▼
L6 WHY YOUNG TREES FAIL (diagnosis: seed vs light vs vegetation vs browse vs site; protection strategy)
 │
 ├──────────────► [B2 native-woodland branch: L9 restraint, open habitat, browse at scale]
 ▼
L7 KEEPING A FOREST IRREGULAR (layers, gaps/groups/single trees, recruitment, quality, harvest income)
 │        ◄── quality/timber depth (C20/C21) joins here
 ▼
L10 INDEPENDENT INTEGRATED MANAGEMENT (all, with disturbance risk and finite cash)
```

Differences from the manager's draft graph:

- **Disturbance is not a level between irregular management and integration.** Disturbance reasoning (L8) needs only L1–L4 and can be taught early as a branch; it then recurs as *background risk* in L7 and L10. Placing it late would delay the most timely real-world lesson for Irish owners (Storm Éowyn, 2025) without pedagogical gain.
- **Regeneration diagnosis follows species and site**, as in the draft, because "why are young trees failing?" has its most important answers (wrong species for the site; no seed source of that species) only once species and site exist.
- **Quality and economics are not a final add-on.** Basic pruning appears at L4; quality-driven selection and strategic economics arrive with irregular management (L7), where they actually change decisions.

## 5. Assessment of the proposed teaching pattern

Proposed: **SHOW → EXPLAIN → GUIDED PRACTICE → CONSEQUENCE → REASSESS → REDUCE ASSISTANCE → COMBINE PROBLEMS → INDEPENDENT MANAGEMENT**.

**Verdict: the right overall arc across the whole sequence, but the wrong shape inside a scenario.** Three corrections.

1. **Observe before show.** CCF teaching (marteloscope practice, Pro Silva field days) starts by *looking at the stand* before anyone explains a rule. "Show" risks becoming "here is the answer". Within a lesson, prefer: **OBSERVE → EXPLAIN THE MECHANISM (not the prescription) → TRY → CONSEQUENCE → COMPARE → REASSESS**. The mechanism explanation is the existing P2 approach ("describes, never prescribes").
2. **Assistance fades per concept, not per scenario.** In Scenario 3 the species/site lesson is new and fully supported, while positive selection (learned in Scenarios 1–2) is already at reduced support. A scenario therefore has a *profile*: one or two focus concepts at high support, older concepts at reduced support (see `ProgressiveAssistance.md`).
3. **Add transfer and contrast.** The strongest evidence of judgement is reusing a concept in a new context (positive selection among *mixed species* in Scenario 3; release of *advance regeneration* rather than Crop Trees in Scenario 2) and comparing contrasting cases (two similar trees, two similar gaps, two plans). The pattern needs explicit **COMPARE** and **TRANSFER** steps. "Combine problems" alone tends to create difficulty, not understanding.

Revised arc across the sequence:

```text
within a scenario's focus lesson:  OBSERVE → EXPLAIN MECHANISM → TRY → CONSEQUENCE → COMPARE → REASSESS
across scenarios:                  FOCUS (high support) → TRANSFER (new context, reduced support)
                                   → COMBINE (several limits at once) → INDEPENDENT (evidence, no verdict)
```

Not every scenario follows the same pattern:

- Scenario 2 is mostly REASSESS and TRANSFER (an existing forest, little new mechanism).
- Branch B1 is CONSEQUENCE-first (the disturbance has already happened).
- Scenario 5 is almost entirely INDEPENDENT on familiar mechanics plus one new idea (maintaining irregularity).
- Scenario 6 has no focus lesson at all.
