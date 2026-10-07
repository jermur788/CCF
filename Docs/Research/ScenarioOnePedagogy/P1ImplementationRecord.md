# Scenario One pedagogy P0 + P1 — implementation record

**Branch:** `task/scenario-one-pedagogy-p1` · **Worktree:** `/home/jer/CCF-pedagogy-p1` · **Base:** `origin/main` @ `3e4ee400af0673aae615317a2e2e1f548041c197` (also the context commit). **Status:** implemented on the task branch; ready for Manager review; not integrated.

## Start conditions (recorded at task start)

| Check | Result |
|---|---|
| `origin/main` | `3e4ee40` "Record Regeneration Model 1 integration and accepted decision (D-047)" |
| Context SHA | `3e4ee40` (latest commit touching `Docs/Project`, `AGENTS.md`, `CLAUDE.md`, `Tools/ProjectContext`) |
| Sol's growth/site/mortality work | **Not in main.** `task/sitka-site-height-adult-mortality` @ `187079c`; an integration worktree (`/home/jer/CCF-integration-growth`, `integration/growth-model1` @ `187079c`) was running Unity during this packet |
| Sol-owned files | `ForestEcologyController`, `ForestSaveController`, `ForestSaveData`, `ForestSaveValidation`, `ScenarioOneManager`, `ScenarioReferenceArchive`, `ScenarioOneInteractionVerification`, new `SitkaGrowthModel`/`AdultMortalityAccount`, several gate harnesses |
| Overlap with this packet | **None.** No file edited here appears in Sol's diff (checked against both the task and integration branches) |
| Live task locks | No active lock on `Assets/ForestPrototype/UI` (the UI redesign row is completed and cleared) |

Research branch handling: only commit `46af237` (harness fixes + `BaselineRedGateInvestigation.md`) was cherry-picked, as `db21642`. The residual-stand evidence and harness were **not** ported; their numbers must be regenerated after the growth model lands.

## P0 — verification run-mode contract

- `Docs/Verification/ScenarioOneGateModes.md` classifies every Scenario One harness: BATCH-SAFE, INTERACTIVE-REQUIRED or HEADLESS-DIAGNOSTIC. None is OBSOLETE. Each entry has its reason, command pattern, pass token, what a failure means, and what it does not mean.
- **Removal:** current contract (U = speciesless area clearance). Batch **PASS**.
- **Clearance:** first-use help dismissed as a player would; menu preferences restored; batch fails fast. Interactive **PASS** (with rendered captures at 1280/1600/1920).
- **MenuTutorial:** batch fails fast. Interactive **PASS**.
- **Finding:** `run_clearance_gate.py`'s built-in completion anchors predate D-047. They are documented in the gate-mode file, not changed here.

## P1 — teaching copy and framing (production changes)

All changes are strings or presentation. Mechanics, formulas, objectives, economy, ecology and save are untouched.

| Item | Where | Change |
|---|---|---|
| P1.1 CCF | `MenuHelpView` (first-use HUD help, F1) | New title "Your forest · continuous-cover forestry". CCF defined as repeated selective work that keeps tree cover and renewal; a process, not one thinning or one ideal shape; judge decisions by the forest left behind |
| P1.2 Positive selection | `MenuHelpView` (Tree Inspection), `LearningObjectivesView` (tree.crop, topic 2), `ForestTreeMarkingManager` (C message, prompt order) | "Start with a tree worth keeping", then inspect its competitors. "A tree being smaller or suppressed is not, on its own, a reason to remove it." The prompt now lists `[C] Keep as Crop Tree` before `[X] Mark to Fell`. After C: "P0707 is now a Crop Tree. Inspect its neighbours…" |
| P1.3 Tree Inspection | `MenuHelpView`, `TreeInspectionView`, `LearningObjectivesView` (tree.read) | DBH (1.3 m), crown, competition ("bigger, closer neighbours count most; small or distant ones little"), Crop Tree intent. A Crop Tree line on marked trees. Footer reordered, with `[F1] Help` |
| P1.4 Forecast | `ForestTreeMarkingManager.UpdateTreatmentOutcome` (string only) | "Estimate … (information, not advice)". Growth is labelled as the average of **all retained trees**, plus "Release is local: check your Crop Trees' own neighbours. Felling more is not automatically better; it also removes growing stock." Same computed values. `m3` → `m³` |
| P1.5 Map | `MenuHelpView` | Layer → cell → information → Set waypoint → close → walk → inspect the actual trees; navigation, not decision. Waypoint mechanics unchanged |
| P1.6 Work Plan | `MenuHelpView`, `WorkPlanView` | Forest decides what; Work Plan reviews how and at what cost. The minimum-charge note no longer suggests felling more trees to spread the cost. **No promise of future features** |
| P1.7 Annual Review | `MenuHelpView` | Four questions: work, money, forest, *what to inspect next*. Walk back; one thinning does not finish CCF |
| P1.8 Regeneration | `LearningObjectivesView` (map.regeneration), `ScenarioOneUiFacts.Why(d, ecology, cell)` | Natural regeneration = young trees from seed already produced in or near the forest; more is not automatically better. **Ground/map "Why" now names the absence of seed** where no species' seed rain reaches the cell (e.g. Year 0) |
| P1.9 Planting | `LearningObjectivesView` (topic 6, plant.ground), `AnnualReviewView` | Targeted, not automatically an improvement. An explanatory line beside an open planting objective: no oak or beech seed trees exist on the property. **Objective unchanged; flagged for P7** |
| P1.10 Browsing | `LearningObjectivesView` (map.browse) | Deer eating young shoots; what matters is survival and recruitment. Bands only, no densities |
| P1.11 Clearance | `ClearancePreview.Label`, `VegetationClearance.Summary`, `WorkPlanView`, `AnnualReviewView`, `LearningObjectivesView` (topic 7, clear.*) | Names **young trees** among the removed vegetation; vegetation grows back; review the footprint. Explicit: "In this version, clearing does not change how well seedlings survive or grow." No benefit claim anywhere (string-tested) |
| P1.12 Pruning | `LearningObjectivesView` (topic 4, prune.result) | Raises a knot-free clear stem for future timber quality; records lift and clear-stem height; trims the crown slightly (8 % per lift, existing model); **no timber price change in this scenario** |
| P1.13 Repeated management | Annual Review help, topics 3 and 8, fell.result, forecast | One thinning does not finish CCF; openings close; reassess later. **No objective logic** |
| P1.14 Help revisit | existing F1 / Help buttons | All five screens reopenable (tested). No new panel |

Not changed, but noted: the forest-objective labels still show species ids ("planted sessile-oak") and the FOREST review line still shows "sitka-spruce". Fixing those touches `ScenarioOneObjectives` (serialised in the century review) and `ScenarioEcologyReviewLines`, so they are left for a later packet. The wind label still reads "high" for every tree (calibration decision pending).

## Files changed

Production (UI/copy only):
- `Assets/ForestPrototype/UI/` — `MenuHelpView.cs`, `LearningObjectivesView.cs`, `TreeInspectionView.cs`, `WalkingHudView.cs`, `StandMapView.cs`, `WorkPlanView.cs`, `AnnualReviewView.cs`, `ScenarioOneUiFacts.cs`
- `Assets/ForestPrototype/ForestTreeMarkingManager.cs` (message, prompt and forecast strings)
- `Assets/ForestPrototype/ScenarioOne/ClearancePreview.cs`, `VegetationClearance.cs` (label strings)

Verification / docs:
- `Tools/Verification/ScenarioOneTeachingCopyVerification.cs` (new)
- `Tools/Verification/{ClearanceVerification,MenuTutorialVerification,ScenarioOneRemovalVerification}.cs` (ported fixes)
- `Docs/Verification/ScenarioOneGateModes.md`
- `Docs/Verification/ScenarioOnePedagogyP1/*.jpg` (12 compressed captures)
- `Docs/Research/ScenarioOnePedagogy/BaselineRedGateInvestigation.md` (ported)
- this record

No scene, prefab, ProjectSettings, package, save or definition asset changed.

## Verification

Unity 6000.6.0f1, isolated profile per run; batch runs use `-batchmode -nographics`; interactive runs use `DISPLAY=:0` without batch flags. Fresh Library import: 0 compiler errors.

| Gate | Mode | Result |
|---|---|---|
| ScenarioOneTeachingCopyVerification | batch | **PASS**: content (CCF, positive selection, suppressed ≠ reason, map rule, Work Plan purpose, review questions, clearance honesty, no prescriptive phrasing), seed line, help revisit ×5, contracts unchanged (objectives, economy, ecology, save v16), rendered layout skipped |
| ScenarioOneTeachingCopyVerification | interactive | **PASS**, plus `TEACHING_COPY_RENDERED_PASS` forecast at 1280×720, 1600×900, 1920×1080 (visible, inside viewport, no overlap with the marking summary) |
| ScenarioOneRemovalVerification | batch | **PASS** |
| ClearanceVerification | interactive + capture | **PASS** (walking preview, A–J, circle, 64-cell matrix, annual, rendered UI at three resolutions) |
| MenuTutorialVerification | interactive | **PASS** (all five help screens rendered at three resolutions, map/waypoint, annual gate, lessons, preferences) |
| ScenarioOneCompletionVerification | batch | **PASS**, `SCENARIO_ONE_COMPLETION_HASH 6F84AF319D301F87` (unchanged), completed Year 25, minimum cash 589,773 |
| ScenarioReferenceVerification | batch | **PASS**, Year 100 `7AD177B3CC2F73C7`, schedule `56C8B99FA1E8DDD1` (unchanged) |
| ScenarioOneInteractionVerification | batch | **PASS**, canonical `BFC55473C1506067` (unchanged) |
| ScenarioOnePlantingVerification / PruningVerification / ProgressVerification | batch | **PASS** |

The final marking-summary key reorder (`[C] Crop  [X] Fell  [G] Plant`) was made after the main regression. It was re-verified: interactive Teaching Copy **PASS** (forecast rendered at all three resolutions) and batch Completion **PASS** with `6F84AF319D301F87`.

## Manual review (rendered walkthrough — not a human playtest)

The required "fresh-profile smoke" was done as a **scripted rendered walkthrough** (MenuTutorialVerification + TeachingCopyVerification on a fresh isolated profile, about the first 10–15 minutes of the tutorial path). I then **reviewed the captures myself**. This is not a substitute for a human playtest; human mouse input and beginner comprehension remain unverified.

| Screen | Trigger | Text (gist) | Action | Enough information? |
|---|---|---|---|---|
| Walking HUD help | game start | CCF definition, process, forest left behind, start with trees to keep, keys incl. F5/F9 | Continue / Esc | **Yes** for purpose and controls. Four short paragraphs, fits at 1280 |
| Ground report | first ground aim | light, browse band, regeneration, "Why: No seed is reaching this spot yet, and it is too dark…" | look | **Yes**: explains *why* nothing grows |
| Tree Inspection help | first E | DBH, crown, competition, Crop Tree intent, suppressed ≠ reason | Continue | **Yes**. Fits at 1280 |
| Tree card | inspection | fields plus footer `[C] Crop Tree [X] Fell …` | C / X | **Partly**: "Wind exposure: high" on every tree is uninformative (pending decision) |
| Marking forecast | ≥1 Fell mark | estimate, not advice; average of retained trees; release is local | mark | **Yes**, but small muted text over a busy scene is hard to read (pre-existing style; presentation follow-up) |
| Stand Map help | first M | patterns → cell → waypoint → walk → inspect; not a decision tool | Continue | **Yes**. Seed-aware "Why" visible in the side panel |
| Clearance preview | ground aim | "removes ground plants and young trees inside; Affected now: … young-tree groups · … planted sapling · … ground-plant patches" | U | **Yes**. Still shown on every ground glance (decision #9 unchanged) |
| Work Plan help | first Tab | forest decides; plan reviews how and cost; minimum charge | Continue | **Yes** |
| Annual Review help | first results | four questions, walk back, one thinning does not finish CCF | Read, acknowledge | **Yes**. FOREST column still shows the raw id "sitka-spruce" |
| Lessons | O | revised topics 2, 3, 4, 6, 7, 8 | read | **Yes**. Ordering is still map-first (P6 will re-sequence) |

## Flags for later packets

- **P7:** mandatory Beech + Oak objective (now explained, not changed); objective competence redesign.
- Decision #26: the wind label is uninformative. Presentation: the forecast line's readability.
- Species ids in objective labels and review lines.
- Decision #9: the clearance preview is shown on every ground glance.
- Launcher anchor table (`run_clearance_gate.py`) predates D-047.
- After Sol's integration: set `CCF_EXPECTED_SAVE_VERSION=17` for the copy gate, re-run all gates, and regenerate the residual-stand evidence.
