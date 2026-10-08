# Implementation sequence after P3

**Status:** recommendation. None of these packets is authorised. Each would be issued by the Manager with a fresh BASE after the previous packet integrates. Item numbers refer to `RemainingWorkMatrix.md`.

## 0. Before any packet (not a packet)

1. Reproduce the MenuTutorial "Help Escape also closed inspection" failure on main `869ee92` (interactive ×2) and classify it (#2).
2. Integrate P3 (#1).
3. Human smokes for P2 and P3 (#3, #4).
4. Manager docs: record P2/P3 in Overview/Decision Log, refresh milestone header and live locks (#5); record E1 "no single forestry score" (#6).

## 1. Order

```text
P3 integrate ─► S1-A Teaching, terminology & objectives ─► S1-B Private test build ─► H1 beginner test 1 (+ optional early forester look at marking)
                                                                                         │
                                                       S1-C Review & history  ◄──────────┘ (may start during H1)
                                                            │
                         decisions N1/N2/L3/O1/M1 ──► S1-D Second cycle & completion
                                                            │
                                                  P8 fixes ─► forester review ─► H2 beginner test 2 ─► FEATURE COMPLETE
```

All four packets edit Scenario One UI files; run them **sequentially with one UI owner**, never in parallel worktrees. S1-D is the only packet that touches the manager/objectives and changes an anchor.

---

## S1-A — Teaching, terminology and objectives

**EXACT PURPOSE**
Main teaches what CCF is and starts from positive selection, makes no false statement for the active regeneration model, shows species by name, shows the forest objectives and the 25-year horizon where play happens, and warns before any purchase that would enter the cash dead end. The P1 harness fixes become tracked sources on main.

**BASE ASSUMPTION**
`main` with P3 integrated (save 19; RNG 1 / regeneration 2 / growth 1 / storm 0 for new games). MenuTutorial failure classified. P1 `1a36ea9` used as a **source**, not merged wholesale; P2 overlap notes applied.

**FILES/SYSTEMS LIKELY TO CHANGE**
- `UI/MenuHelpView.cs`, `UI/LearningObjectivesView.cs`, `UI/TreeInspectionView.cs`, `UI/WalkingHudView.cs`, `UI/StandMapView.cs`, `UI/WorkPlanView.cs`, `UI/WorkPlanOverview.cs` (P3; wording only), `UI/AnnualReviewView.cs`, `UI/ScenarioOneUiFacts.cs`, `UI/ScenarioOneUiRoot.cs` (`[O]` shows forest objectives with the learning list), `UI/Resources/ScenarioOneUi.uss` (font sizes only).
- Strings only: `ForestTreeMarkingManager.cs`, `ScenarioOne/ClearancePreview.cs`, `ScenarioOne/VegetationClearance.cs`, `ScenarioEcologyReviewLines.cs` (display names).
- Nursery buttons: pre-purchase use of P3's `CashOutlook` (#16).
- Verification: `Tools/Verification/{ScenarioOneTeachingCopyVerification (re-based to v19), ClearanceVerification, MenuTutorialVerification, ScenarioOneRemovalVerification}.cs`, `Docs/Verification/ScenarioOneGateModes.md`; remove the P0 overlay from `Tools/Verification/WindthrowV1/run_regression.py`.
- If J3 is accepted: `ForestPlayer.cs` preview trigger only.

**EXCLUSIONS**
No change to stored objective ids/display names (`ScenarioOneObjectives` values are serialized in the Century Review; map at render time). No objective logic, completion, economy values, ecology, save, scenes, prefabs or `ScenarioOne.asset`. No stage engine (S1–S12). No new mechanics. No storm wording changes beyond what is needed to avoid a false statement.

**ACCEPTANCE BEHAVIOUR**
- First-use HUD help defines CCF as repeated, selective work that keeps cover and renewal.
- The interaction prompt lists `[C] Keep as Crop Tree` before `[X] Mark to Fell`.
- Every clearance text in a new game says that dense bramble/bracken can reduce small young trees' survival, that clearing also removes young trees in the area, and that vegetation returns. Legacy model 0/1 saves keep the "no survival effect" wording.
- No raw species id in any rendered text; stored ids unchanged.
- `[O]` (or the HUD) shows the forest objectives by name with current/target values, the 25-year horizon and why broadleaves must be planted.
- Buying stock that would leave expected cash below the harvest minimum before a first thinning shows the warning **before** the purchase; the purchase is still allowed.
- No information-bearing text below 13 px.

**AUTOMATED VERIFICATION**
Teaching-copy gate (batch + rendered at 1280×720 / 1600×900 / 1920×1080), per-model clearance string gate (0/1 vs 2), raw-species-id sweep, deny-list (`should`, `best`, `correct`, `cut this`), pre-purchase warning fixture, Clearance (interactive), MenuTutorial (interactive), Removal (batch), P2 and P3 gates, full regression **from tracked sources only**; all anchors unchanged (completion v19 `84CD51EB6A951D9E`, v18-compatible `702766DECE591E21`, growth 1 `7E57B9DAEF5BF4D0`/`35E2BF1C2F55C2DE`, legacy `BFC55473C1506067`, Reference `7AD177B3CC2F73C7`/`9CDF21A541C5968D`).

**MANUAL VERIFICATION**
Fresh profile: read the five help screens; mark a Crop Tree and a neighbour; plan a clearance; buy stock to near the warning; open objectives. Does any sentence read as an instruction or a verdict?

**STOP CONDITIONS**
Any anchor drift; the copy gate needing more than a version/expectation change plus the documented P2 merge; any need to change objective logic or stored names; the MenuTutorial failure turning out to be production behaviour that needs an input redesign (return to Manager).

---

## S1-B — Private test build

**EXACT PURPOSE**
A tester can install, start, save, quit, resume and restart Scenario One on their own machine, with help shown fresh, and the build's identity and performance are recorded.

**BASE ASSUMPTION**
S1-A integrated. A build machine with Unity 6000.6.0f1 and the licensed Ultimate Nature pack at the GUIDs in `UltimateNatureSetup.md`.

**FILES/SYSTEMS LIKELY TO CHANGE**
- New editor-only build script (e.g. `Assets/ForestPrototype/Editor/ScenarioOneTestBuild.cs`) writing to the ignored `Build/` folder; it must not leave `ProjectSettings` changes behind.
- New `UI/SessionMenuView.cs` (+ `.meta`): Save, Load, Start a new forest (confirmation; reloads the scene without loading the save), Reset help for a new tester, Quit (confirmation). Opened from Esc when no other screen is open (resolve against the classified Esc behaviour).
- `UI/ScenarioOneUiRoot.cs` (hook), `ForestSaveController.cs` (save/load result message only), `UI/WalkingHudView.cs` (build id label only).
- Docs: build record template, updated `BeginnerPlaytestProtocol.md` port.

**EXCLUSIONS**
No main-menu scene, options/graphics settings menu, autosave, cloud saves, installer, telemetry, new scene or scene edits, package changes, Git LFS. No gameplay change.

**ACCEPTANCE BEHAVIOUR**
- A clean checkout plus the pack builds with one command; the HUD shows the source SHA.
- Save → quit → relaunch → Load restores the same world; Start a new forest returns to Year 0 with €12,000; Reset help makes all first-use screens appear again.
- No verification harness or auto-start script is present in the Player.

**AUTOMATED VERIFICATION**
Build succeeds headless; a harness (Editor) checks save → reload scene → load world-hash equality and that new-forest restores the Year-0 world hash; regression unchanged (presentation/session only). Player run for Years 0→25 logs frame-time percentiles, the annual-step time and peak memory.

**MANUAL VERIFICATION**
Install on a second machine (not the dev machine); play 15 minutes; use every session-menu action; confirm Esc never discards work silently.

**STOP CONDITIONS**
The build needs persistent `ProjectSettings` or scene changes (return to Manager: single-writer serialized assets); the licence does not cover a private build; Player profiling shows annual advance > 2 s or walking median > 33 ms on the reference machine (report; do not optimise inside this packet).

---

## S1-C — Annual Review v2, Forest Diary and place history

**EXACT PURPOSE**
After each year the player sees what they did, what it cost, what changed (including natural deaths), and up to three places worth walking to; they can read the whole forest's history and a cell's management history. All from saved data.

**BASE ASSUMPTION**
S1-A and S1-B integrated; beginner test 1 report available or running (copy adjusts with P8). No save change.

**FILES/SYSTEMS LIKELY TO CHANGE**
New `UI/ForestHistory.cs` (+ `.meta`): pure read-only derivations over snapshots, reports, events, mortality records, deadwood and planted records, taking a year range. `UI/AnnualReviewView.cs` (sections WORK DONE · MONEY · FOREST · REGENERATION · DISTURBANCE · PLACES TO INSPECT; History tab), `UI/StandMapView.cs` (cell history list), `UI/WorkPlanView.cs` (History button only). New harness `ForestHistoryVerification`.

**EXCLUSIONS**
No save fields; no recommendations or projections; no causal sentence the simulation did not compute; no clearance-benefit statement in history; no stage engine; no inherited-history boundary or scenario id (Scenario Two); no sample plots.

**ACCEPTANCE BEHAVIOUR**
- Sections in fixed order; empty sections collapse to one line.
- DISTURBANCE names self-thinning deaths ("n trees died from crowding") and hosts the existing storm card unchanged.
- PLACES TO INSPECT: ≤ 3, deterministic priority (`AnnualReviewV2.md` §4), each with Set waypoint; descriptions say what is there, never what to do.
- History tab: intervention years plus every 5th year; "show all".
- Stand Map: selecting a cell lists its events (felled, planted, cleared, deaths, deadwood) by year.
- The first-review acknowledgement gate is unchanged.

**AUTOMATED VERIFICATION**
Scripted 15-year game (thinning Y1, planting Y6, clearance Y8): timeline equals saved data; cell history lists the felling events; PLACES order deterministic; identical text before and after save/load; legacy v15–v18 saves render; Reference preview shows no player history; deny-list ("caused", "because you", "should", "helped"); rendered at three resolutions; anchors unchanged.

**MANUAL VERIFICATION**
Play 10 years; does PLACES send you somewhere worth looking at; does the diary answer "what happened here?"

**STOP CONDITIONS**
Any need for a save field; anchor drift; derivations that cannot reproduce saved values exactly.

---

## S1-D — Second look, second cycle and completion

**EXACT PURPOSE**
Close **decide → time → observe → reassess** twice. The forest tells the player when it has changed enough to look again (a reason, never "thin now"); Scenario One completes only after two separated management cycles with continuous cover and standing capital kept and a deliberate broadleaf introduction surviving; nothing ends in "Failed".

**BASE ASSUMPTION**
S1-C integrated (PLACES TO INSPECT hosts the trigger reason). Decisions recorded: N1 (model D variant below), N2 (deadwood objective removed from completion, still reported), L3 (one broadleaf species surviving 5 years), O1 (no hard fail), M1 thresholds [C]. Storms remain off. Single writer for the manager/objectives during the packet; independent review required.

**FILES/SYSTEMS LIKELY TO CHANGE**
`ScenarioOne/ScenarioOneObjectives.cs` (named condition evaluators returning met/unmet + reason), `ScenarioOne/ScenarioOneManager.cs` (`EvaluateProgress`, outcome wording; old `Failed` display mapping), new pure `ScenarioOne/SecondLookCheck.cs` (+ `.meta`), `ScenarioOne/ScenarioOneDefinition.cs` (new thresholds as code-default fields; **do not hand-edit `ScenarioOne.asset`**), `UI/AnnualReviewView.cs` (completion view, trigger reason), `UI/WalkingHudView.cs` (one line), `Tools/Verification/ScenarioOneCompletionVerification.cs` (new anchor), `ScenarioOneProgressVerification.cs`, new negative-control harness. Optional, if the economy owner approves: the one-line sapling price adapter override (#35).

**Completion definition (recommended minimum):**
1. Year ≥ 25.
2. First release: a resolved felling within 8 m of a tree that is a Crop Tree at evaluation (derived from event positions and saved marks).
3. Second cycle: resolved management work **of any type** at least 5 years after the first release, so the cash dead end cannot block completion after a first thinning.
4. Continuous cover: mean canopy ≥ 0.35 in **every** snapshot year.
5. Standing capital: ≥ 60 original-plantation trees (P-prefixed) living, as the Century Review already counts.
6. Renewal: ≥ 3 regenerating cells **and** ≥ 1 planted broadleaf alive ≥ 5 years after planting.

The trigger (RD ≥ 0.6 via `SitkaGrowthModel.RelativeDensity` from current trees, ≥ 3 cells with light-limited young trees, or planted losses ≥ 25 %; floor 5 years after the first thinning; fallback 12) is **teaching only**, not a completion condition, so no trigger state is saved.

**EXCLUSIONS**
No save schema change; no per-forest stage progress; no "no work needed" event; no recovery mechanic (O2); no storm activation; no ecology or economy value change; no player-declared goals.

**ACCEPTANCE BEHAVIOUR**
- Unmanaged play never completes and the review names the unmet conditions.
- One thinning only → not complete ("second cycle").
- Clear-fell → not complete ("continuous cover").
- Planting that dies before 5 years → not complete ("renewal").
- A reference two-cycle plan completes; completion view shows dimensions with no total.
- Year 100 without completion reads "Century Review — scenario goals not completed"; old saves stored as `Failed` display the same wording.
- The trigger fires between Years 6 and 20 for T0–T5-type plans.

**AUTOMATED VERIFICATION**
Completion gate under the current stack ×2 processes: **new completion anchor recorded with explicit authorisation**; lowest cash and completion year logged. Negative controls above. Trigger calibration across T0–T5. Save: v1 → current load; old `Failed` display. Full regression; lifecycle and Reference anchors unchanged.

**MANUAL VERIFICATION**
Two contrasting play-throughs (conservative and heavy). Does the trigger reason make sense in the forest? Does the completion screen read as a description, not a grade?

**STOP CONDITIONS**
Any lifecycle or Reference anchor drift; any need for a save field; the trigger not firing within Years 6–20 for the plan family (report; do not tune silently); the reference two-cycle plan failing on cash.

---

## 2. Special architecture check — finish now so Scenario Two does not duplicate it

| Scenario One work | Why Scenario Two would otherwise duplicate or generalise it immediately | How to finish it cleanly now (without Scenario Two infrastructure) |
|---|---|---|
| History derivation (S1-C) | Scenario Two (*The Inherited Stand*, PROPOSAL) is about reading management history | `ForestHistory` as pure functions over saved records with a year-range input; no Scenario-One literals in the derivation. Scenario Two only adds a boundary year |
| Completion conditions (S1-D) | Every later scenario needs separated cycles + guardrails + focus evidence | Named condition evaluators (met / unmet + reason) in one place, read from saved data. Do **not** build a profile system |
| Second-look trigger (S1-D) | Reassessment is Scenario Two's skill | One pure function from current state and events; no UI coupling |
| No-hard-fail outcome semantics (S1-D) | Every scenario needs the same outcome wording and old-save mapping | Decide and implement once |
| Species display names (S1-A) | New species arrive from Scenario Three | One mapping (`ScenarioOneUiFacts.SpeciesName`) used by every view; zero raw ids |
| Tracked harness sources (S1-A) | All future gates extend them | Remove the runner overlay from `1a36ea9` |
| Standalone build path (S1-B) | Every later external test | Keep the build script scenario-agnostic (scene list, SHA stamp) |
| Mixed-species SDI mortality (#34) | Any multi-species stand inherits the error | Ecology-owner diagnosis now; fix before multi-species work |
| Sapling price authority (#35) | Scenario Two economy reuses the price book | Document the authority; adapter override |

**Do not build now:** scenario id in the save, start-from-snapshot, scenario select, inherited-history boundary, assistance tiers, per-scenario completion/assistance profiles, per-forest progress record, plan-comparison mark sets, training stand.

## 3. What each packet deliberately leaves

| Packet | Leaves for later |
|---|---|
| S1-A | Stage engine, plan comparison, assistance tiers |
| S1-B | Main menu, settings, autosave, installer |
| S1-C | Per-cell ecological history (save), sample plots, standing volume per year |
| S1-D | Per-forest progress record, "no work needed" event, recovery mechanic, storms |

## 4. What can wait

| Wait until | Work |
|---|---|
| **Beginner external testing** | S1–S12 stage engine / Tier A progression; plan comparison (B-lite); assistance changes; approval summary before approve-all; per-cause planted-loss line if testers do not ask "why did it die"; storm-off label wording; asset packets 01/07 and bramble/bracken recognition fixes (only if testers fail recognition); J3 if not decided earlier; competition label thresholds |
| **Professional forester feedback** | Cash recovery route (standing sale / net settlement); pruning purpose beyond honest copy; sample plots; minimum-charge level and holding size; any change to completion thresholds; harvest-damage modelling |
| **Multi-species foundation** | Mixed-species density mortality fix (if the diagnostic shows Scenario One impact, earlier); species-aware shade/occupancy; new species; site map |
| **Scenario Two** | Scenario package v0 (id in save, start snapshot, inherited boundary), scenario select, per-scenario completion and assistance profiles, per-forest progress record, "no work needed" event, plan comparison in every scenario, training stand, storm branch scenario (B1) and storm teaching, storm activation policy |
| **Later visual polish** | Asset packets 02–10, deadwood volume line, moss cushions, crown/root storm art, broadleaf LOD/material budget, track contact, UI symbol consistency |
