# Scenario One — beginner knowledge-gap test (Workstream A3)

**Status:** audit [REPO] plus design inference [INF]. Not decision authority.

## Assumed player

The player knows nothing about CCF and little about forestry. They know what a tree is and how to move in a first-person game (WASD, mouse look, E to interact). They do not read long text unless it is needed for the next action.

## Grading

Each step of the current journey is graded on six questions:

| Code | Question |
|---|---|
| G | Do they know **what** they are trying to achieve? |
| W | Do they know **why**? |
| I | Do they know **which information** matters? |
| U | Do they know **which interface** to use? |
| T | Do they understand the **forestry term**? |
| S | Can they tell whether they **succeeded**? |

**CLEAR** — answered without guessing. **PARTIAL** — answered for the interface, not the forestry. **MISSING** — not answered. **MISLEADING** — the game suggests a wrong answer.

## Journey grading

| # | Step in the current build | G | W | I | U | T | S | Main gap |
|---|---|---|---|---|---|---|---|---|
| 1 | Game starts; HUD introduction | PARTIAL | MISSING | PARTIAL | CLEAR | MISSING | MISSING | No statement of what the property is, what CCF means or what the player owns. "Forest objectives 2 of 8" is shown with no list |
| 2 | Walk; look at ground | MISSING | MISSING | PARTIAL | CLEAR | PARTIAL | MISSING | The ground report shows light 0.05 "deep shade" and "Regeneration: none". The clearance square appears at once with `[U] Plan area clearance` — **MISLEADING**: it suggests clearing is the normal response to ground |
| 3 | Aim at a tree | PARTIAL | MISSING | MISSING | CLEAR | PARTIAL | — | Prompt offers "Mark to Fell" first. Nothing suggests looking for a tree to keep |
| 4 | Inspect a tree (E) + introduction | PARTIAL | PARTIAL | PARTIAL | CLEAR | PARTIAL | MISSING | DBH explained. The CI number has no scale. "% potential diameter growth withheld" is informative but its cause (which neighbours) is hidden. Nothing about form/quality. "Not yet seed-bearing" unexplained |
| 5 | Mark Crop Tree (C) | PARTIAL | PARTIAL | MISSING | CLEAR | PARTIAL | MISSING | "Retain/favour" is stated, but not *for what* or how to choose. No feedback that the choice was reasonable |
| 6 | Mark Fell (X) | PARTIAL | MISSING | MISSING | CLEAR | PARTIAL | **MISLEADING** | The forecast line appears with "growth +y%" — a stand-wide average that rises when *any* trees are removed. A novice reads "more removal = more growth = good" |
| 7 | Open Map (M) + introduction | CLEAR | CLEAR | PARTIAL | CLEAR | PARTIAL | CLEAR | Best-taught screen. Light/regeneration/browse values have legends, but "what is a good value?" is left open — correctly |
| 8 | Waypoint journey | CLEAR | PARTIAL | CLEAR | CLEAR | CLEAR | CLEAR | Fine as navigation. It is 11 of the first 11 lesson steps, so the "next lesson" line keeps pointing at map practice instead of forestry |
| 9 | Open Work Plan (Tab) + introduction | PARTIAL | PARTIAL | PARTIAL | CLEAR | PARTIAL | PARTIAL | Contractor minimum explained in text. Net value is shown. The novice still does not know whether a **loss-making first thinning** is normal (in Irish practice first thinnings often barely pay — **not stated** in game) |
| 10 | Choose Sell / Keep / Leave as deadwood | PARTIAL | PARTIAL | PARTIAL | CLEAR | PARTIAL | MISSING | Deadwood value is stated as "the scenario's habitat record". Why deadwood matters ecologically is not stated |
| 11 | Approve | CLEAR | PARTIAL | CLEAR | CLEAR | CLEAR | CLEAR | Approve ≠ execute is well taught |
| 12 | Advance one year | CLEAR | MISSING | — | CLEAR | — | PARTIAL | Why one year? Why repeat? The game does not say that CCF is repeated small interventions |
| 13 | Annual Review + acknowledgement | PARTIAL | PARTIAL | PARTIAL | CLEAR | PARTIAL | PARTIAL | WORK/MONEY/FOREST structure is clear. FOREST lines ("Canopy 0.951 → 0.897; mean light …; Opened cells: 3 …") have no reference point for good or bad. That is correct for a non-prescriptive game, but the player gets no help interpreting the change |
| 14 | Walk back to the thinned area | MISSING | MISSING | PARTIAL | — | — | MISSING | Nothing asks the player to return. The checklist asks only for a deadwood log visit, not "look at your Crop Tree" |
| 15 | Notice regeneration (Year 1+) | MISSING | MISSING | PARTIAL | PARTIAL | MISSING | MISSING | Sitka regeneration appears naturally (seed rain begins as trees mature). The game never explains *why* it appeared, or that it is the plantation's own species |
| 16 | Decide whether to plant | **MISLEADING** | MISSING | PARTIAL | CLEAR | PARTIAL | PARTIAL | Completion *requires* Beech and Oak. The novice concludes "planting is how you do CCF". The genuine reason — no broadleaf seed source exists on the property — is never shown |
| 17 | Shelters | PARTIAL | PARTIAL | PARTIAL | CLEAR | PARTIAL | PARTIAL | Browsing pressure is fixed "low". Shelter value is visible only via later browse lines |
| 18 | Clearance | PARTIAL | **MISLEADING** | PARTIAL | CLEAR | MISLEADING | PARTIAL | "Competing vegetation" includes tree regeneration and saplings. A novice may clear the regeneration the stand needs |
| 19 | Pruning | PARTIAL | PARTIAL | CLEAR | CLEAR | PARTIAL | PARTIAL | Lift rules are honest. Pruning value (knot-free timber) is not stated; and no price premium is implemented (D-044), so the player pays with no visible return |
| 20 | Years 2–24 | MISSING | MISSING | MISSING | CLEAR | — | PARTIAL | No prompt to reassess, no second-intervention lesson, no history of a single tree or plot |
| 21 | Completion at Year 25 | PARTIAL | MISSING | — | — | — | PARTIAL | "Scenario objectives met" — reached partly by inaction (see ObjectiveGraph) |
| 22 | Century Review (Year 100) | PARTIAL | PARTIAL | PARTIAL | CLEAR | PARTIAL | PARTIAL | Compares with the frozen reference "not an optimal score" (good). It is far too late to inform earlier decisions |

## Totals

| Grade | Count (123 graded cells; 9 cells marked "—" do not apply) |
|---|---|
| CLEAR | 32 |
| PARTIAL | 60 |
| MISSING | 27 |
| MISLEADING | 4 (plus the clearance invitation at step 2, noted in its text) |

(Counted by script from the table above.)

**Pattern [INF]:** **U (interface)** is almost always CLEAR. **W (why)** and **S (success)** are mostly MISSING. The current build teaches *how to operate the game* far better than *why a forester would act*. This matches the existing record's own caveat that "source inspection cannot certify beginner comprehension".

## The six most damaging gaps

1. **No mental model of CCF** (steps 1, 12, 20). Proposed fix: a 3-sentence opening framing plus repeated-management staging (`ProposedTutorialArc.md` Stage 0–1).
2. **Removal-first marking** (steps 3, 6). Proposed fix: positive-selection sequence and Crop-Tree-centred feedback (`PositiveSelectionTeaching.md`).
3. **Stand-average forecast misread as "more cutting is better"** (step 6). Proposed fix: report Crop-Tree release separately from stand averages (`ResidualStandEvaluationPrototype.md`).
4. **Planting presented as mandatory and unexplained** (step 16). Proposed fix: explain the seed-source absence and frame enrichment planting as a targeted choice (`RegenerationTeaching.md`). The objective itself is a **PRODUCT DECISION REQUIRED** (see `DecisionMatrix.md`).
5. **Clearance labelled as generic "competing vegetation"** (steps 2, 18). Proposed fix: name affected tree regeneration explicitly in the preview, and stop showing the preview on every ground glance (copy + UI packet).
6. **No return-to-site and no second intervention** (steps 14, 20). Proposed fix: revisit and reassess stages (`RepeatedInterventionDesign.md`).

## Limits of this test

This is an expert walkthrough of the code and text, not a user study. A small playtest is the necessary check: 3–5 novices, think-aloud, measured against the questions above. A test protocol is in `ImplementationRoadmap.md` §Test plan.
