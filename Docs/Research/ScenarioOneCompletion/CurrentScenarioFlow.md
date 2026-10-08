# Scenario One — current play flow, reconstructed from code (Workstream A)

**Status:** audit. Not decision authority. No production file changed.
**Base:** `origin/main` @ `a8596df9c52669a36709f09e85dfe6568640af49` (Growth Model 1, save v17). Every **[REPO]** claim was checked against code at this SHA.
**Candidate overlay:** where the unintegrated pedagogy P1 branch (`task/scenario-one-pedagogy-p1` @ `1a36ea9`) changes player-facing text, this is marked **[P1-CAND]**. P1 is *not* on main. It merges cleanly into `a8596df` (checked with `git merge-tree`), but its copy gate still expects save v16.

Evidence labels used throughout this folder:

| Label | Meaning |
|---|---|
| **[REPO]** | Read in code/data at `a8596df` |
| **[P1-CAND]** | On the unintegrated P1 branch only |
| **[M2-CAND]** | On the unintegrated Regeneration Model 2 / understorey branch (`902903f`) only |
| **[STORM-DESIGN]** | Proposal on `task/windthrow-readiness` @ `9a9f4fb`; nothing implemented |
| **[HARNESS-Y0]** | Unity harness evidence from the pedagogy branch (`60674f1`) for the **Year-0** stand. Still valid at `a8596df`: Growth Model 1 does not change the authored starting stand, Hegyi competition, light or prices. Later-year harness numbers are **not** reused |
| **[PROTO]** | This task's offline script `Tools/Verification/ScenarioOneCompletion/year0_release_and_pattern.py` |
| **[LOG]** | A recorded Unity gate result (named) |
| **[ECON]** | *Irish Forestry Economics, Labour and Contractor Operations for CCF Stage 1* (PDF, SHA-256 `fc51dc48…`, local copy only) |
| **[ACC]** | Accepted decision (Decision Log ID) |
| **[INF]** | Design inference by this audit |

---

## 1. Fixed facts about the property [REPO][HARNESS-Y0]

| Fact | Value | Source |
|---|---|---|
| Property | 40 × 40 m = **0.16 ha**, 64 ecology cells of 5 × 5 m (A1–H8) | stand evidence; `CellsPerAxis` |
| Start | 336 Sitka, age 20, 2,100 stems/ha, basal area 41.1 m²/ha, mean DBH 15.6 cm, mean canopy 0.951, mean ground light 0.049 | `input-residual-stand.json` baseline |
| Seed | Sitka seed maturity begins at age 20 (`SitkaSpruce.asset maturityOnsetYears: 20`). Beech and Sessile Oak begin at **40** | species assets |
| Broadleaf seed source | None on the property. Planting is the only route for Beech/Oak | starting stand |
| Cash | €12,000 start; contractor €45/h; harvest job minimum **€2,500 per year's harvest** | `ScenarioOne.asset` |
| Timber value of the whole Year-0 stand | about **€1,702** notional roadside | [HARNESS-Y0] T0 |
| Model stack for new games | RNG 1, regeneration 1, growth 1, browse pressure 0.2, save v17 | D-046/047/048 |
| Completion gate | Completes at Year 25 with all 8 objectives; lowest cash €6,139.75 | [LOG] `ScenarioOneCompletionVerification`, `D7C4DDD36B53FCCE` |

## 2. Stage-by-stage flow

Each row: **trigger · UI · player action · objective state · simulation state · economic state · completion condition · known fragility**.

### 2.1 New game

| Field | Current behaviour [REPO] |
|---|---|
| Trigger | Scene load; `ScenarioOneManager.InitializeNewScenario` (new-game model stack), `ScenarioOneUiRoot.Start` builds the UI |
| UI | Walking HUD. First-use modal **Walking HUD help** opens immediately (`MenuHelpView.Introduce`, keyed `CCF.MenuHelp.v1.WalkingHud` in PlayerPrefs) |
| Player action | Read and close (Esc/Continue) |
| Objective state | 2 of 8 forest objectives already met (`retained-canopy` 336 ≥ 60, `continuous-canopy` 0.95 ≥ 0.35). HUD shows "Forest objectives 2 of 8" |
| Simulation | Year 0; no seed-bearing trees yet; regeneration 0 cells |
| Economy | €12,000 |
| Completion | n/a |
| Fragility | The display name "…Sitka Plantation to Continuous-Cover Forest" exists in the definition but is **never shown** [REPO]. Main never defines CCF. **[P1-CAND]** adds a CCF definition to this help. Introductions are **per device**, so a second player on the same machine never sees them |

### 2.2 Orientation (map and waypoint)

| Field | Current behaviour |
|---|---|
| Trigger | Player presses M, or follows the HUD "Next lesson" line (the first unfinished learning step, always `map.open` at the start) |
| UI | Stand Map (Light / Regeneration / Browsing / Marks layers), side-panel diagnosis, Set Waypoint; HUD waypoint panel with arrow |
| Player action | Select a cell, set a waypoint, walk there, inspect |
| Objective state | Learning steps `map.*` (11 of 42 steps) complete on UI events. No forest objective is affected |
| Simulation | None |
| Economy | None |
| Completion | `map.arrive` needs a journey started from another cell |
| Fragility | Map is the **first and largest** learning topic, so navigation is taught before any forestry reasoning. Well built and honest ("does not make the forestry decision for you") |

### 2.3 Inspection

| Field | Current behaviour |
|---|---|
| Trigger | Aim at a tree, press E |
| UI | Tree card: species, ID, origin, mark chip, DBH, height, stem volume, age, ground light, **Competition label + CI**, last-year DBH growth, **Wind exposure**, pruning, reproduction, "Why" sentence, "If felled now" timber estimate |
| Player action | Read |
| Objective state | Learning `tree.inspect`, `tree.read` (reading) |
| Simulation / economy | None |
| Fragility | At Year 0, **336/336** trees read "Wind exposure: high" and **328/336** read "crowded" [HARNESS-Y0], so neither label helps choose between trees. The card shows *how constrained* a tree is but **not which neighbours constrain it** (Workstream B) |

### 2.4 Marking

| Field | Current behaviour |
|---|---|
| Trigger | Aim at a tree; X (Fell) or C (Crop Tree), exclusive (D-013) |
| UI | Blue/red markers; marking summary "FELL n · CROP TREE n · fell volume"; **treatment forecast** line once ≥ 1 Fell mark |
| Player action | Mark / unmark |
| Objective state | Learning `tree.crop` completes when **any** living Crop Tree exists; `fell.mark` when any Fell mark exists. No forest objective |
| Simulation | Marks are saved tree state; nothing else changes |
| Fragility | The forecast (`ForestTreeMarkingManager.UpdateTreatmentOutcome`, `:285`) reports growth as the **mean over all retained trees**, not the Crop Trees, plus "wind peak" and "opening x of 2" with no explanation. Main prompt order is `[X] Mark to Fell | [C] Retain as Crop Tree` (Fell first); **[P1-CAND]** reverses it and labels the forecast "information, not advice". The forecast is small muted text over the scene |

### 2.5 Work Plan

| Field | Current behaviour |
|---|---|
| Trigger | Tab |
| UI | Thinning job card (assortments, roadside value, harvesting work, small-job minimum, net), planting, pruning (batch add of eligible Crop Trees), vegetation clearance, nursery, Reference Future |
| Player action | Review, set felling outcome (Sell / Keep / Leave as deadwood), choose planting executor and shelters, buy stock, **Approve pending work** |
| Objective state | Learning `fell.plan`, `fell.cost`, `plant.*`, `prune.*`, `plan.*` |
| Economy | Approval requires **cash ≥ total cost** including the harvest minimum. Timber revenue is not netted at approval (`ApprovePendingWork`, `:1111`) |
| Fragility | "Approve pending work" approves **everything** pending in one press. The thinning card's net figure is always negative early (see §3). Pruning is a batch button over all eligible Crop Trees |

### 2.6 Execution and time advance

| Field | Current behaviour |
|---|---|
| Trigger | "Advance one year" in the Work Plan |
| UI | Work Plan button → Annual Review opens automatically |
| Simulation | Approved work resolves (one batched harvest settlement), then exactly one ecology year (`AdvanceYear`, `:1143`): competition, light, growth, Sitka density mortality, regeneration, browsing; planted juveniles; understorey; deadwood decay. Snapshot and annual report recorded; `EvaluateProgress` runs |
| Economy | Harvest settled as one job (minimum applied once per year); planting/pruning/clearance settle per order |
| Fragility | Advance is blocked only (a) by the **first** unread Annual Review, (b) if approved cost exceeds cash, (c) after failure or Year 100 |

### 2.7 Annual Review

| Field | Current behaviour |
|---|---|
| Trigger | Automatically after each advance; also from Work Plan |
| UI | WORK DONE · MONEY · FOREST columns, TRENDS (cash, canopy), OBJECTIVES list, CENTURY REVIEW at Year 100 |
| Player action | First review only: "I've read the annual results" (saved per forest: `annualReviewSeen`) |
| Fragility | Only the **first** review needs acknowledging, so later years teach "skim and continue". FOREST lines show **raw species IDs** ("sitka-spruce 14", `ScenarioEcologyReviewLines`). The OBJECTIVES footer falls back to `TutorialHint`. Nothing points the player **back to a place** in the forest |

### 2.8 Regeneration

| Field | Current behaviour |
|---|---|
| Trigger | Automatic: Sitka seed rain from Year 1 (age-21 trees) |
| UI | Ground report (light, browse, regeneration, "Why"), map Regeneration layer, Annual Review "Regenerating cells" |
| Objective state | `regeneration ≥ 3 cells` is met **from Year 1 with no action** (14 cells at Year 1 untreated [HARNESS], pre-Growth-Model-1 run; regeneration at Year 1 does not depend on adult height) |
| Fragility | Regeneration "succeeds" with no management, so the objective teaches nothing. **Broadleaves cannot regenerate naturally** within the 25-year window: there is no seed source, and planted Beech/Oak reach seed maturity only at age 40 [REPO] |

### 2.9 Planting

| Field | Current behaviour |
|---|---|
| Trigger | Buy stock (Work Plan nursery) → G planting mode → click ground spots |
| UI | Hotbar, 1 m² circle preview, planting markers, Work Plan planting card |
| Objective state | `introduced-beech` and `introduced-sessile-oak`: **one** surviving planted Beech and **one** Oak meet them (`ScenarioOneObjectives.cs:76-94`) |
| Economy | €4.50 / €5.50 per sapling + 10 min labour (contractor €7.50, or owner time from 40 h/yr) + optional €5 shelter [D] |
| Fragility | Planting both species is **mandatory** for completion. The reason (no broadleaf seed source) is never stated on main; **[P1-CAND]** adds one line. Objective labels show raw IDs ("Establish planted sessile-oak") |

### 2.10 Browsing and protection

| Field | Current behaviour |
|---|---|
| Trigger | Automatic every year (background pressure 0.2 = "low") |
| UI | HUD "Browsing: low · shelters n effective", ground report browse state, map Browse layer, Annual Review browse line |
| Player action | Shelters ON in the Work Plan planting card (per order, 8 effective years) |
| Objective state | None. Browsing is never required to be understood |
| Fragility | Natural cohorts cannot be sheltered (individual shelters only; no fencing gameplay, D-044) |

### 2.11 Vegetation clearance

| Field | Current behaviour |
|---|---|
| Trigger | **Any ground aim** shows a 5 × 5 m preview headed "Clear competing vegetation" (`ForestPlayer.cs:438`) → U designates |
| UI | Footprint, target pins, "Affected now: n cohorts · n saplings · n ground patches" |
| Objective state | None |
| Simulation | Removes cohorts, unpromoted juveniles and competing cover in the footprint. Under regeneration model 1, **clearance has no causal survival or growth benefit** (D-047 open item). **[M2-CAND]** adds that benefit |
| Fragility | The preview on every glance invites clearance everywhere. Natural tree regeneration is counted inside "competing vegetation" |

### 2.12 Pruning

| Field | Current behaviour |
|---|---|
| Trigger | Work Plan "Add n eligible crop tree pruning task(s)" |
| Rules | Lifts to 2.5 / 5 / 6.5 m, ≥ 5 years apart, crown base < 60 % of height [D]. Cost about €9.75 / €13.50 / €15.75 per tree per lift (`8 min + 2 min/m` at €45/h) |
| Objective state | None |
| Fragility | No price premium exists (accepted, honest), so pruning has **no visible payoff** inside Scenario One. It is taught as an interface only |

### 2.13 Later intervention

| Field | Current behaviour |
|---|---|
| Trigger | None. Nothing asks for a second intervention |
| Objective state | `managed-opening` stays satisfied by the first felled tree |
| Fragility | The completion reference plan does make a second intervention, but the player is never asked to. Repeated management is enforced only by the 25-year minimum |

### 2.14 Completion

| Field | Current behaviour |
|---|---|
| Trigger | `EvaluateProgress` after each year: all 8 objectives true at Year ≥ 25 → `Completed` |
| UI | HUD phase "objectives met"; Annual Review line "Scenario objectives met in year N. Continue to the Century Review" |
| After completion | Play continues to Year 100 (Century Review against Reference Future v1) |
| Failure | (a) cash **exactly** €0.00 with no approved orders (`ScenarioOneManager.cs:2221`); (b) Year 100 reached without completion (`:2225`) |
| Fragility | See `PlayerStallPoints.md` S1: a player whose cash falls below the €2,500 harvest minimum **before their first thinning** can never complete, but is told only at Year 100 |

## 3. Economic shape of the loop [REPO][HARNESS-Y0][ECON]

- Every Year-0 thinning plan tested loses money: from −€1,871 (heavy, T3) to −€2,366 (conservative, T4). The €2,500 minimum binds in every case [HARNESS-Y0].
- Sol's Growth Model 1 evidence: a 30 % thinning stays net-negative up to scenario Year 25. It first nets positive around Year 40 (+€495) (`Docs/Research/SitkaGrowthMortality/EconomyTutorialImpact.md`).
- [ECON]: a €2,500 minimum stops binding at about 119 t, roughly 1.9 ha of first-thinning removal. **Scenario One is 0.16 ha.** At this size the small-job penalty is permanent, not a lesson the player can grow out of inside the scenario.

## 4. Classified findings (the Workstream A checklist)

### 4.1 Mechanics implemented but poorly introduced

| Mechanic | Why it is poorly introduced |
|---|---|
| Treatment forecast | Unexplained stand-average growth, "wind peak", "opening x of 2" |
| Competition CI | No scale; label thresholds 1/3 hidden; 328/336 "crowded" |
| Wind exposure | Saturated: all trees "high" |
| Seed maturity | "not yet seed-bearing" is never linked to regeneration (P1 adds a seed line to "Why") |
| Contractor minimum | Explained in help, but only as cost. Never as "this property is small", and never tied to how often to intervene |
| Recent opening | Raw internal state in the forecast |
| F5/F9 save/load | Untaught on main (P1 adds it to the HUD help) |
| Reference Future preview | Hidden at the bottom of the Work Plan; never introduced as a comparison tool |

### 4.2 Tutorial text without meaningful action

- Reading steps (8 of 42) end in "I've read and understood this". Nothing checks understanding.
- `ScenarioOneManager.TutorialHint` numbered steps 1–5 are dead in normal play (the UI root always exists), and step 2 is prescriptive ("Buy Beech and Sessile Oak").
- `prune.read` explains lift rules but leads to no decision that matters inside the scenario (no premium, no visible quality state).

### 4.3 Objectives that can auto-complete

| Objective | How |
|---|---|
| `retained-canopy` | True at Year 0 (336 ≥ 60); also counts recruited Sitka as "original" |
| `continuous-canopy` | True at Year 0 (0.95 ≥ 0.35) |
| `regeneration` | True from Year 1 with no action (natural Sitka) |
| `minimum-year` | Time only |
| Learning `tree.crop` / `fell.mark` | Any mark, including marks restored from a save |
| Learning `plant.ground` | First glance at the ground |
| All learning steps | Already done on this device in another forest |

### 4.4 Required actions that are not evidence of understanding

`managed-opening` (one felled tree, any tree), `introduced-*` (one sapling each, anywhere), `fallen-deadwood` (≥ 0.02 m³, one small log), `tree.crop` (any tree). Each is a counter, not a decision (`ObjectiveRedesign.md`).

### 4.5 Duplicate teaching

Approval ≠ execution: Work Plan help, `plan.read`, `fell.approve`, `plant.approve`. Ground vs crown light: inspection help, `map.light`, `tree.read`. Consistent, but four channels say it with different wording.

### 4.6 Gaps between stages

| From → to | Gap |
|---|---|
| Inspection → marking | Nothing connects "this tree is constrained" to "these neighbours constrain it" |
| Marking → Work Plan | The Work Plan shows only what is removed and its cost, never what remains (P3) |
| Annual Review → next decision | No "where to look" pointer, no history, no link back to places |
| First intervention → second | No trigger, no prompt, no objective |
| Planting → outcome | Survival is visible only as aggregate review lines. No revisit prompt to planted sites |
| Clearance → outcome | On main there is no outcome to observe (no causal effect) |

### 4.7 Places the player can become stuck without knowing why

See `PlayerStallPoints.md`. Most severe: **S1** (cash below the harvest minimum before the first thinning → completion impossible, failure only at Year 100); **S2** (objective list unreachable from the HUD except as a count); **S3** (mandatory broadleaf planting with no stated reason on main).
