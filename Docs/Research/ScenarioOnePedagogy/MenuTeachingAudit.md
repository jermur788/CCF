# Scenario One — menu teaching audit (Workstream A5)

**Status:** audit [REPO] plus proposals [INF]. Not decision authority. No production text was changed.

Each interface is described twice: **CURRENT** (what the build communicates) and **PROPOSED** (the role the interface should teach). The proposed text is collected in `TutorialCopyBank.md`.

The accepted constraints these roles must respect:

- D-004: decide → advance → walk → observe → understand → decide again.
- D-010: the Work Plan reviews and approves; it does not design the forest spatially.
- D-021: add choices inside interfaces rather than piling up shortcuts.
- The map principle: **THE MAP HELPS YOU FIND WHERE TO LOOK. IT DOES NOT MAKE THE FORESTRY DECISION FOR YOU.**

---

## 1. Walking HUD

| Aspect | CURRENT [REPO] | PROPOSED [INF] |
|---|---|---|
| Purpose | "your forest at a glance": year, cash, objective count, browsing, waypoint, ground report, marks, prompts | **Where forestry decisions are made.** The HUD supports looking, not deciding |
| When to use | Always while walking | Always. Most time should be spent here |
| What it tells you | Year, cash, "Forest objectives N of 8", browse band, shelters, next lesson, ground report (light/browse/regen/why), marks summary, treatment forecast | Same, plus: the *current stage goal* in one line (from the tutorial arc), and Crop-Tree release on the forecast line |
| Decisions that belong here | Mark Fell (X), Crop Tree (C), plan clearance (U), plant (G), inspect (E) | Same. Positive selection first: C before X |
| Decisions that do not | Approving, paying, choosing executor (in Work Plan) | Same |
| How to leave | n/a (base state). Esc releases the mouse | Same |
| How to revisit help | F1 / Help button | Same. Add F5/F9 save/load to the help text |
| Communicates the role? | **Partly.** The HUD help says "The forest is where you make management decisions" — good. But the always-on clearance square and "Fell first" prompt order teach removal-first | Show the clearance preview only after U is pressed once, or in a clearance mode (bounded UI change). Reorder the prompt to `[C] Keep as Crop Tree · [X] Mark to Fell · [E] Inspect` |

## 2. Tree Inspection

| Aspect | CURRENT [REPO] | PROPOSED [INF] |
|---|---|---|
| Purpose | "understand one tree" | **Judge one tree's role relative to its neighbours** |
| When to use | Before marking | Before choosing a Crop Tree, and before every Fell mark near one |
| What it tells you | Size, age, volume, cell light, CI + label, last-year growth, wind band, pruning, reproduction, Why (growth withheld), If felled now (assortments) | Same, plus derivable-now facts only (see `ResidualStandMetricAudit.md`): **largest competitors** (top neighbours by Hegyi term, with distance), **is it competing with a Crop Tree?** (its Hegyi term on each Crop Tree within 8 m), height relative to neighbours |
| Decisions that belong here | Mark/unmark Crop or Fell | Same |
| Decisions that do not | Execution, cost approval | Same; and no "cut this tree" recommendation |
| How to leave | E (or walk away) | Same |
| How to revisit help | F1 | Same |
| Communicates the role? | **Partly.** It is causal and non-prescriptive ("this screen does not select a correct answer"). It describes the tree in isolation. The relationship that matters for CCF marking — *who competes with whom* — is invisible | Add the neighbour relationship (a derivable-now presentation, no new state) |

## 3. Stand Map

| Aspect | CURRENT [REPO] | PROPOSED [INF] |
|---|---|---|
| Purpose | "find where to look" | Unchanged |
| When to use | Looking for a site to inspect | Same; also to find your Crop Trees and earlier work again |
| What it tells you | Per-cell ground light, canopy, regeneration (planted count + tallest cohort), browse band + shelters, Fell/Crop counts; side-panel causal "Why" | Same. Optional later layer: "recent work" (cells with completed management events, from saved history — derivable now) |
| Decisions that belong here | Choose a waypoint | Same |
| Decisions that do not | Marking, clearing, planting, approving — the map **cannot** do any of these | Same |
| How to leave | M / Esc / Back to forest | Same |
| How to revisit help | F1 / Help button | Same |
| Communicates the map principle? | **Yes, explicitly and in behaviour.** The help says "The map helps you find where to look… it does not make the forestry decision for you". The `map.compare` reading repeats it. The code offers no remote action. The waypoint never moves the player | Keep. One risk: the map is the first and largest lesson topic (11 steps), so it is taught before the player has any forestry question to take to it. Move the map lesson to *after* the first tree judgement ("find your Crop Tree again", "find a bright cell") |

## 4. Work Plan

| Aspect | CURRENT [REPO] | PROPOSED [INF] |
|---|---|---|
| Purpose | "review and approve jobs" | **Review what your marks will do and cost, then approve** |
| When to use | After marking or planting | Same; plus to read the residual-stand summary before approving |
| What it tells you | Owner time; totals (external cost, expected timber, net, cash after); thinning card (products, sales, harvesting cost, small-job minimum, net); planting; pruning eligibility; clearance; nursery; Reference Future | Same, plus a **"What you are leaving"** block (retained basal area/volume, Crop Trees released, seed sources retained, opening pattern) — see `ResidualStandDecision.md` |
| Decisions that belong here | Remove/cancel jobs, felling outcome, planting executor, shelters, buy stock, approve, advance | Same |
| Decisions that do not | Choosing which trees or where — "add new ones in the forest" | Same (D-010) |
| How to leave | Tab / Back to forest / Close | Same |
| How to revisit help | F1 | Same |
| Communicates the role? | **Yes** for execution and cost. **No** for consequences: the Work Plan reports the harvest (volumes, money), not the forest left behind | Add the residual-stand block (Packet 3 in the roadmap) |

## 5. Annual Review

| Aspect | CURRENT [REPO] | PROPOSED [INF] |
|---|---|---|
| Purpose | "learn from the year" | **Connect what you did with what changed, and decide what to inspect next** |
| When to use | After each advance (opens automatically) | Same |
| What it tells you | WORK DONE, MONEY, FOREST (canopy/light, opened cells, regeneration by species, planted fate, browsing, protection, retained material), TRENDS (cash, canopy), OBJECTIVES, CENTURY REVIEW | Same, plus: a **WHAT TO INSPECT NEXT** list built from authoritative changes (e.g. "Crop Tree P0712: DBH growth 0.41 → 0.66 cm since the thinning"), and a link to earlier years (`AnnualReviewHistoryIntegration.md`) |
| Decisions that belong here | None (records outcomes) | None. It points to places; it does not recommend actions |
| Decisions that do not | All forestry actions | Same |
| How to leave | Esc / Walk the forest / Open Work Plan | Same |
| How to revisit help | F1 | Same |
| Communicates the role? | **Mostly.** The three-question structure is good. FOREST values have no reference point and no link to *where*; the year-to-year trend covers only cash and canopy | Add the inspect-next list and the history entry point |

---

## 6. Cross-interface issues

1. **Two "objective" systems** (learning checklist vs forest objectives) are both labelled *objectives* on the HUD. Rename the checklist "Lessons" or "Field skills" [INF].
2. **Terminology drift:** Fell / harvest / thinning / felling; clearance / regeneration removal / competing vegetation. See `ForestryTerminologyAudit.md`.
3. **No screen shows management history for a place.** The data exists: `ScenarioManagementEvent` carries year, task, tree id and cell index. It is not surfaced (`ForestDiaryConcept.md`).
4. **The Work Plan and Annual Review report money and stand averages. The Crop Tree — the thing the player chose to favour — has no feedback anywhere after it is marked**, except pruning eligibility.
