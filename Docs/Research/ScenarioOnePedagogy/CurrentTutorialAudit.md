# Scenario One — current tutorial and teaching audit (Workstream A1)

**Status:** research / audit. Not decision authority. No production file was changed.
**Base inspected:** `origin/main` @ `3e4ee400af0673aae615317a2e2e1f548041c197` (context commit `3e4ee40`).
**Method:** repository inspection of every player-facing string and teaching trigger, plus existing verification logs. Previous handoffs were used only to find code. They were not taken as evidence of behaviour.

Evidence labels used in this folder:

| Label | Meaning |
|---|---|
| **[REPO]** | Verified by reading code or data at `3e4ee40` |
| **[LOG]** | Observed in a Unity verification log (named) |
| **[PRAC]** | Practitioner guidance: Pro Silva synthesis, 6 Oct 2026. Supporting only |
| **[EMP]** | Empirical ecology: CCF Primary Literature Synthesis v1. Supporting only |
| **[INF]** | Game-design inference by this audit |
| **[ACC]** | Accepted project decision (Decision Log ID given) |
| **[HARNESS]** | Measured by this work's read-only harness (`ResidualStandEvaluationPrototype.md`, `Evidence/`) |

---

## 1. Summary of findings

1. **Seven teaching channels run in parallel, and nothing coordinates them [REPO].** They are: five one-time menu introductions, a 42-step learning checklist, a HUD "next lesson" line, eight scenario-success objectives, a Work Plan hint line, contextual prompts, and one gate that requires the player to read the Annual Review. Each channel has a different trigger, storage and success definition. The player sees two separate kinds of "objective": a learning checklist and forest objectives.
2. **The game never explains what CCF is [REPO].** The scenario's display name ("…Sitka Plantation to Continuous-Cover Forest") is defined in `ScenarioOneDefinition` but never shown in the UI. The HUD title reads only "Scenario One · Year N". The only player-facing string that contains "continuous-cover" is the **failure** message at Year 100: "Year 100 ended before the continuous-cover objectives were reached" (`ScenarioOneManager.cs:2182`). No string explains CCF, why the plantation is being converted, or what success looks like ecologically.
3. **The forest objectives reward counts, not competence [REPO][LOG].** Doing nothing for 25 years meets 4 of the 8 objectives. The completion gate's unmanaged control at Year 25 had `retained-canopy=356/60`, `continuous-canopy=0.952/0.35`, `regeneration=37/3` and `minimum-year=25/25` all marked achieved (`SCENARIO_ONE_COMPLETION_PHASE P11`). One felled tree satisfies "Carry out a commissioned thinning". One surviving sapling of each species satisfies "Establish planted …".
4. **Planting is required to finish the scenario, whatever the site [REPO].** Completion requires planted Beech **and** planted Sessile Oak (`ScenarioOneObjectives.Evaluate`, `introduced-<species>`). This conflicts with the game brief's principle that "planting trees is not automatically ecologically beneficial". It also conflicts with Irish practitioner guidance that enrichment planting is targeted insurance, not a default [PRAC §11.8]. In this stand, though, broadleaf seed sources are genuinely absent, so planting is the only route by which broadleaves can arrive. The objective is defensible for this stand, but the game never says *why*.
5. **The checklist teaches each interface well but teaches forestry reasoning very little [REPO][INF].** Felling, pruning, deadwood, planting and clearance are each taught as *proposal → plan → approve → result*. Nowhere is the player asked to choose a tree to favour first, to find its competitors, or to look at the forest left behind.
6. **Crop Tree marking exists, but it is taught as an alternative to Fell, not as the starting point [REPO].** The Crop Tree step comes before felling in the checklist. But nothing links a Crop Tree to its competitors, and the marking forecast averages growth over *all* retained trees, not the Crop Trees.
7. **Clearance is offered every time the player looks at the ground [REPO].** Aiming at any ground shows a 5 × 5 m square headed "Clear competing vegetation". The prompt `[U] Plan area clearance` appears with a count of affected tree cohorts and saplings. Natural tree regeneration is therefore labelled "competing vegetation" by default.
8. **Progress is per device, not per forest [REPO].** First-use introductions and learning progress are stored in `PlayerPrefs` (`CCF.MenuHelp.v1.*`, `CCF.Learning.v1.*`). A second player or a new forest on the same machine inherits all progress and never sees the introductions. Help is still available through F1. This was deliberate (`Docs/Scenario1MenuTeaching.md`), but it is a failure mode for a classroom or shared machine.
9. **Repeated management is enforced only by the 25-year minimum [REPO][INF].** No objective or lesson asks for a *second* intervention, or for a revisit after time has passed. The completion reference plan does make a second intervention (P9 `secondIntervention=52`), but the player is never asked to.

---

## 2. Inventory (A1)

Fields: NAME · TRIGGER · TEXT (summary; full text is in the cited file) · PLAYER ACTION · COMPLETION · DEPENDENCIES · CODE OWNER · SAVE STATE · REPEATABLE? · SKIPPABLE? · FAILURE MODE.

### 2.1 Menu first-use introductions (`MenuHelpView`)

Five entries, one per screen. Code: `Assets/ForestPrototype/UI/MenuHelpView.cs:76` (`Explanation`), triggered from `ScenarioOneUiRoot.cs:138` (`help.Introduce(context)`).

| NAME | TRIGGER | TEXT (gist) | PLAYER ACTION | COMPLETION | SAVE | FAILURE MODE |
|---|---|---|---|---|---|---|
| Walking HUD | First frame in which no other screen is open (game start) | Status, ground report, E to inspect, M/Tab/O/F1, mouse release | Read; Continue/Esc | Closing sets `CCF.MenuHelp.v1.WalkingHud=1` | Device PlayerPrefs | Five paragraphs appear before the player has seen the forest. Mentions "objectives" without saying which set. Never mentions F5/F9 save/load |
| Tree Inspection | First tree inspected with E | DBH, crown/light, competition, Crop Tree vs Fell, "you choose" | Read; Esc | Same key, TreeInspection | Device | Explains fields but gives no reason to favour a tree. "Competition shows whether neighbours restrict growth" gives no scale for the CI number shown |
| Stand Map | First M press | Layers, select → waypoint → return → inspect; "does not make the forestry decision for you" | Read; Esc | Same key, StandMap | Device | Good. States the map principle explicitly (see MenuTeachingAudit) |
| Work Plan | First Tab | Forest decides, plan reviews; contractor minimum; approve ≠ execute | Read; Esc | Same key, WorkPlan | Device | Introduces the contractor minimum before the player has any job, so the warning lacks context |
| Annual Review | First review screen **after** at least one annual report exists | WORK DONE / MONEY / FOREST questions; acknowledge to unlock | Read; Esc, then acknowledge | Same key, AnnualReview | Device | Good. Correctly delayed until results exist |

Dependencies: `ScenarioOneUiRoot.HelpContext()` (`:207`). Repeatable: yes, via F1 or the Help button. Skippable: yes. Not shown during Reference preview.

### 2.2 Learning objectives checklist (`LearningObjectivesView`)

Code: `Assets/ForestPrototype/UI/LearningObjectivesView.cs:25` (topics), `:167` (`Observe`). Opened with O or the HUD/Map/Plan buttons. Saved per device in `CCF.Learning.v1.<id>`. Every step can be repeated as reading, but completion is one-way. Every step can be skipped: nothing gates forestry on it.

| Topic | Steps | Kind | Completion (actual code condition) | Notable failure mode |
|---|---|---|---|---|
| 1 Stand Map | map.open, map.select, map.light, map.waypoint, map.return, map.arrive, map.inspectsite, map.regeneration, map.browse, map.marks, map.compare(read) | 10 action, 1 reading | Each is a UI event: open map, click cell, click layer, set waypoint, close with waypoint, arrive from outside the target cell (`journeyStartedAway`), aim at ground or inspect a tree in the target cell | Map is the first and largest topic (11 of 42 steps), so navigation is taught before any forestry reasoning |
| 2 Tree inspection & Crop Trees | tree.inspect, tree.read(read), tree.crop | 2 action, 1 reading | Any living inspected tree; **any** living Crop Tree exists while walking | `tree.crop` completes on a Crop Tree restored from a save, or one marked at random. No reason for the choice is asked for |
| 3 Felling | fell.mark, fell.plan, fell.cost(read), fell.approve, fell.result | 4 action, 1 reading | Any Fell mark; any FellTree order seen with Work Plan open; Approved/Completed order; any successful FellTree event seen in Review | Completes on **any** tree. The `fell.mark` text tells the player to "propose a tree", not "find its competitor" |
| 4 Pruning | prune.read(read, req tree.crop), prune.plan, prune.approve, prune.result | 3 action, 1 reading | Order exists / approved / succeeded | States the scenario's lift rules honestly as "scenario rules, not a recommendation" (good). Gives no timber-quality reason [EMP §6: pruning is not a guaranteed premium] |
| 5 Fallen deadwood | deadwood.read(read, req fell.plan), deadwood.plan, deadwood.result, deadwood.visit | 3 action, 1 reading | Felling order set to RetainAsFallenDeadwood; succeeded event; within 6 m of any deadwood record | Frames deadwood only as a felling outcome. Good: it does not over-claim habitat value |
| 6 Planting & protection | plant.ground, plant.stock, plant.plan, plant.execution(read, req plant.plan), plant.shelter, plant.approve, plant.result | 6 action, 1 reading | Aim at ground while walking; stock purchased; planting order exists; any order with shelter; approved; succeeded | `plant.ground` completes the first time the player looks at the ground, before reading anything. "Check existing growth before deciding whether planting is appropriate" is the only anti-default statement |
| 7 Vegetation clearance | clear.read(read, req plant.ground), clear.plan, clear.approve, clear.result | 3 action, 1 reading | Any RemoveRegeneration order; approved; succeeded | Legacy species-specific removal orders also count. "Do not clear a cell just because the map is dark" is good, but the ground preview invites clearance everywhere (§1 item 7) |
| 8 Work Plan & Annual Review | plan.open, plan.read(read), review.open, review.read | 3 action, 1 reading | Work Plan open; Review open with ≥1 report; `AnnualReviewSeen` | `review.read` mirrors the per-forest acknowledgement |

Totals: 42 steps, 34 observed actions, 8 readings. **No step asks the player to explain, compare or predict.** Readings are acknowledged with "I've read and understood this", which does not test understanding (the existing record already notes this).

### 2.3 HUD learning summary

`WalkingHudView.cs:118` shows `ui.Learning.Summary`: "Next lesson: <first unfinished step> · [O] Objectives". It is always visible while walking. **Failure mode:** with no ordering beyond the topic list, the next lesson is usually a map step, even after the player has done forestry work.

### 2.4 Forest (scenario-success) objectives

Code: `ScenarioOne/ScenarioOneObjectives.cs:49`. HUD shows only the count: "Forest objectives N of 8" (`WalkingHudView.cs:117`). The list with values appears only in Annual Review › OBJECTIVES (`AnnualReviewView.cs:210`). Stored as derived state: re-evaluated from saved snapshots, events and orders, and per forest. Completion triggers `ScenarioOneOutcome.Completed` (`ScenarioOneManager.EvaluateProgress`).

| ID | Display text | Target | Measures | Met at Year 0? | Met with no management by Y25? |
|---|---|---|---|---|---|
| minimum-year | Reach the management review year | year ≥ 25 | Time | no | yes |
| retained-canopy | Retain original canopy trees | living Sitka ≥ 60 | Count of *all* living Sitka, including recruited trees | **yes (336)** | yes (356) [LOG P11] |
| continuous-canopy | Keep continuous canopy | mean canopy ≥ 0.35 | Mean cell canopy | **yes (0.951)** | yes (0.952) |
| regeneration | Maintain regenerating cells | ≥ 3 occupied cells | Any species, any origin | no (no seed source at age 20) — **but yes from Year 1 (14 cells) with no action** [HARNESS] | **yes (37)** |
| fallen-deadwood | Retain fallen deadwood | ≥ 0.02 m³ | Volume on site | no | no |
| managed-opening | Carry out a commissioned thinning | ≥ 1 completed FellTree | **One tree** | no | no |
| introduced-beech | Establish planted beech | planted + present > 0 | One sapling or one promoted tree | no | no |
| introduced-sessile-oak | Establish planted sessile-oak | as above | as above | no | no |

Notes [REPO]: the display uses raw species ids ("planted sessile-oak"). "Retain original canopy trees" counts Sitka recruits as "original": the code counts living trees of the original species, not P-prefixed plantation trees. The Century Review does count P-prefixed trees separately (`legacy-sitka`), so the two screens define "original" differently.

### 2.5 Work Plan hint line (`ScenarioOneManager.TutorialHint`, `:2121`)

Shown in the Work Plan footer, after the latest feedback message. **Live branch:** the unread Annual Review instruction. Otherwise the HUD learning summary. **Dead text [REPO]:** the numbered steps "1. Inspect … mark with X", "2. Buy Beech and Sessile Oak saplings", "3. Press G …", "4. Advance …", "5. Open the annual review". These are reached only when no `ScenarioOneUiRoot` exists. In normal play the manager always adds one (`:435`), so the text runs only in harnesses. Step 2 is also prescriptive ("Buy Beech and Sessile Oak"), which conflicts with the non-prescriptive UI.

### 2.6 Annual Review acknowledgement gate

`ScenarioOneUiRoot.AdvanceFromWorkPlan` (`:242`) and `NeedsAnnualReview` (`:33`). After the first annual report, "Advance one year" is disabled until the player presses "I've read the annual results". Stored per forest in the save field `annualReviewSeen`. It gates only the first review; later reviews open automatically but need no acknowledgement. Verified in the rendered gate at `b3fa29d` (`MENU_ANNUAL_GATE_PASS`). **Failure mode:** a single acknowledgement, so later years teach "skim and continue".

### 2.7 Contextual prompts and messages

| NAME | TRIGGER | TEXT | Owner | Notes |
|---|---|---|---|---|
| Tree aim prompt | Aiming at a living tree | `[E] Inspect · [X] Mark to Fell \| [C] Retain as Crop Tree · [Tab] Plan` | `WalkingHudView.cs` + `ForestTreeMarkingManager.MarkPromptText` | Puts Fell first. Crop Tree is phrased as "Retain", which is good |
| Ground prompt + clearance preview | Aiming at ground (not planting) | `Clear competing vegetation · 5 × 5 m · Affected now: N cohorts · N saplings · N ground patches` then `[U] Plan area clearance · [Esc] Cancel preview` then `[G] Planting mode \| [Tab] Buy saplings` | `ForestPlayer.InteractionPromptText`, `ClearancePreview.Label` | Always on. Tree regeneration is counted as "competing vegetation" (§1 item 7) |
| Planting hotbar | Planting mode | `▶ [1] Beech (n ready) [2] Oak …` | `ForestPlayer.PlantingHotbarText` | Interface only |
| F in Scenario One | F on tree | "Mark with [X], then fell through [Tab] Work Plan." | `ForestPlayer.TryHarvestInteraction` | Good redirect, but F is never mentioned elsewhere |
| Mark messages | X/C | "Marked P0712 as harvest (n marked)" | `ForestTreeMarkingManager.Mark` | Uses "harvest" while prompts say "Fell" (term mismatch) |
| Marking summary | Always while walking | `Marked: FELL n (red) · CROP TREE n (blue) · fell volume x m³ [X] Fell [C] Crop [G] Plant` | `WalkingHudView.cs` | Good visibility |
| Treatment forecast | ≥1 Fell mark | `If felled now: keep x m3 · growth +y% · gap light a → b · wind peak w (band) · opening o of 2` | `ForestTreeMarkingManager.UpdateTreatmentOutcome` | **Most important teaching surface for thinning, and it is unexplained.** "growth +y%" is the mean over *all* retained trees, not the Crop Trees. "opening o of 2" and "wind peak" have no explanation |
| Save keys | F5/F9 | — | `ForestSaveController` | **Not taught anywhere** |
| Time-lapse T | — | — | `ForestEcologyController` | Correctly disabled in Scenario One (`managementAnnualControl`) |

### 2.8 Tree Inspection card (`TreeInspectionView`)

Fields: species, id, origin, mark chip, shelter chip, DBH, height, stem volume, age, ground light at the tree's cell, competition label + CI, last year's DBH growth, wind exposure band, pruning, reproduction state. Then a **Why** sentence (`Diagnosis`, `:89`: share of potential diameter growth withheld by competition) and an **If felled now** timber estimate.

Teaching value: high for "is this tree constrained?". **Missing** for positive selection [INF]: which neighbours cause the competition, whether this tree is competing with a nearby Crop Tree, and whether it is a seed source *for the future*. Sitka maturity starts at age 20 (`SitkaSpruce.asset maturityOnsetYears: 20`), so every tree reads "not yet seed-bearing" at Year 0.

### 2.9 Stand Map, Work Plan, Annual Review

Covered in `MenuTeachingAudit.md`. The forestry-teaching strings there are: the map legend and side-panel "Why"; Work Plan card subtitles ("This visit is small, so the contractor's minimum charge dominates…"); Annual Review FOREST lines (`ScenarioEcologyReviewLines.Lines`), TRENDS (cash, canopy), OBJECTIVES and CENTURY REVIEW.

### 2.10 Reference Future preview

The Work Plan card "Reference Future v1 (frozen)" and the preview banner. It says "Walk the verified historical forest at a milestone… not an optimal score or prescription". This is the only existing *compare against another management path* mechanic. It is relevant to the marteloscope design (`LongTermTrainingFlow.md`).

---

## 3. Failure modes by severity

| Severity | Failure | Evidence |
|---|---|---|
| High | CCF is never explained, so the player cannot know what they are trying to achieve | §1 item 2 |
| High | Objectives can be satisfied by trivial actions (1 tree, 1 sapling each) and by inaction (4 of 8) | §2.4, [LOG P11] |
| High | No positive-selection teaching: Crop Tree → competitor → residual stand is never connected | §2.2 topics 2–3, §2.7 forecast |
| High | Planting both broadleaves is mandatory, and the reason (no broadleaf seed source) is never stated | §2.4 |
| Medium | Clearance preview on every ground glance labels tree regeneration as competing vegetation | §2.7 |
| Medium | The treatment forecast is unexplained and averages growth over the whole stand | §2.7 |
| Medium | Device-level progress hides introductions from new players on a shared machine | §2.1, §2.2 |
| Medium | Inspection labels do not discriminate: every plantation tree reads "Wind exposure: high" (336/336) and 328/336 read "crowded" at Year 0, so they cannot guide a choice | [HARNESS] `Evidence/residual-stand-trees.csv`; scene thresholds 5/12 |
| Medium | Economics: the €2,500 harvest minimum exceeds the whole stand's notional roadside value (€1,702) at Year 0, so every first thinning loses money whatever is marked. Copy never explains this | [HARNESS]; `ResidualStandMetricAudit.md` §3 |
| Medium | Only one Annual Review needs acknowledging, and no second intervention is asked for | §2.6, §1 item 9 |
| Low | Term mismatches: "harvest"/"Fell"/"thinning"; "regeneration cohort removed" vs "vegetation clearance"; raw species ids | §2.4, §2.7 |
| Low | Dead numbered tutorial text in `TutorialHint` | §2.5 |
| Low | F5/F9 save/load untaught | §2.7 |

## 4. What works well and should be kept

- The map principle is stated explicitly, and the map really cannot act remotely (`MenuHelpView`, `StandMapView`).
- Proposal, approval and execution are kept separate, which matches D-009 and D-010.
- The inspection screen is causal and non-prescriptive: "it never names trees to fell".
- Reading steps require an explicit acknowledgement, and failed work does not count as success.
- The game is honest about scenario-specific rules (pruning lifts, "no broadleaf market").
- No first-year deadline, and learning can wait until a decision fits the forest.

These are good foundations for the proposals in Workstream B. The recommended changes are mostly *sequencing, framing and feedback*, not new ecology.
