# Current state audit — Scenario One at `869ee92` (+ P3 candidate `0ab7399`)

Labels: see `README.md`. Code references are to `main` @ `869ee92` unless marked **P3**. "Gate" means an automated Unity harness recorded as PASS in the cited record; it does not mean a human accepted the experience.

## 1. Scenario One teaching

| Concept | State on main | Evidence | With P3 | What is still missing |
|---|---|---|---|---|
| **Positive selection** | **IMPLEMENTED, partial.** Help (F1, Tree Inspection) says Crop Trees are trees you choose to favour and to inspect competitors before removing. The interaction prompt still lists X before C; there is no CCF definition anywhere in the UI | `UI/MenuHelpView.cs:83–84` (P2 text); no "continuous-cover" string in `UI/*.cs` | Unchanged | P1 copy: CCF definition, "start with a tree worth keeping", C-before-X prompt order, post-C message (CANDIDATE `1a36ea9`) |
| **Suppressed ≠ remove** | **INTEGRATED + VERIFIED (automated).** Help sentence "being small or suppressed does not by itself make a tree a problem"; the competitor list ranks a suppressed neighbour low | `MenuHelpView.cs:84`; P2 gate `P2_SUPPRESSED_CASE_PASS` (P0710 rank 43/48, 1.1 %) in `P2ImplementationRecord.md` | Unchanged | Human smoke (P2 manual steps) |
| **Crop Tree reasoning** | **INTEGRATED + VERIFIED (automated).** Crop Tree side panel: top-5 competitors, shares, bands, distribution sentence, world rings/labels, keys 1–5 | `UI/CropTreeCompetition.cs`, `UI/CompetitorAssessment.cs`; P2 batch ×2 + interactive PASS, regression 24/24 | Work Plan reuses the same helper | Human readability review of rings/side panel |
| **Competitor / release reasoning** | **INTEGRATED + VERIFIED (automated).** "Competition now → after your Fell marks" on the card; HUD forecast panel with Crop Tree competition before → after | P2 record (release preview, forecast panel) | **CANDIDATE:** Work Plan "Crop Tree competition" row (same P2 calculation) | — |
| **Regeneration** | **IMPLEMENTED.** Ground "Why" diagnosis, map Regeneration layer, Annual Review regeneration lines, Model 2 vegetation pressure | `RegenerationDiagnosis.cs`, `ScenarioEcologyReviewLines.cs` | Clearance rows show affected young-tree groups | P1 "no seed reaches this cell" reason; per-cell regeneration history (not saved); review lines still show raw species ids (`ScenarioEcologyReviewLines.cs:40`) |
| **Planting** | **IMPLEMENTED.** Nursery purchase, ground-placed orders, executor choice, shelters, persistent juveniles | Overview "Integrated Scenario 1" | Planting cost appears in MONEY | No reason given why broadleaves must be planted (no broadleaf seed source); objective label shows raw id "Establish planted sessile-oak" (`ScenarioOneObjectives.cs:92`) |
| **Vegetation competition** | **ACCEPTED DESIGN + INTEGRATED** (D-049, Regeneration Model 2). Model-aware clearance explanation in the learning panel | `LearningObjectivesView.cs:131–136`; `Model2PedagogyVerification` in regression | Clearance rows: bramble/bracken cover, young trees above 1.5 m | Visual recognition of bramble/bracken and the human clearance playtest (D-049 "Open") |
| **Browsing** | **IMPLEMENTED** (pressure 0.2, shelters, map layer, review line) | D-023, Overview | — | Per-cause planted-loss line (light / browse / vegetation are separate runtime accounts but the review aggregates "lost") |
| **Clearance trade-offs** | **IMPLEMENTED.** Preview on aim, U to plan, Work Plan shows only cost, model-aware explanation | `ForestPlayer.cs` preview; `WorkPlanView.cs` clearance card | **CANDIDATE:** per-cell consequences (young trees removed, density before → after, cover returns) | Decision J3 (preview only after U) is still open |
| **Pruning** | **IMPLEMENTED.** Crop Tree eligibility, batch add, lifts recorded; no price premium | Learning topic 4; D-044 (premium deferred) | Pruning cost in MONEY | P1 copy explaining the purpose (clear stem, no premium in this scenario) |
| **Finances** | **IMPLEMENTED.** Harvest quote with assortments, small-job minimum note, owner hours; approval needs cash ≥ cost; timber is the only income | `ScenarioOneManager.cs:1145–1147`; `ScenarioOneDefinition.cs:30,38` (€12,000 start, €2,500 minimum) | **CANDIDATE:** MONEY block, expected cash after approved/pending work, dead-end warning, headroom line, minimum-charge explanation | Recovery route (product decision); warning *before* a nursery purchase (P3 shows it only after) |
| **Consequences over time** | **IMPLEMENTED, thin.** Annual Review shows the latest year only (three columns), a storm card, TRENDS (cash and canopy, last 12 years) and the objective list | `UI/AnnualReviewView.cs:45–74,185–197` | — | Places to inspect, regeneration/disturbance sections, self-thinning deaths, a history view, map place history, any prompt to return |

## 2. Interfaces

| Interface | On main | P3 adds | Missing for Scenario One |
|---|---|---|---|
| **Walking HUD** | Status, cash, "Forest objectives N of 8", browse/access/waypoint, P2 forecast panel, "Next lesson" from the learning checklist (`WalkingHudView.cs:125`, `LearningObjectivesView.cs:96`) | — | Objective content where play happens (only a count); Year-25 horizon visible; the next lesson is list-ordered, not play-ordered |
| **Tree Inspection** | Measurements, competition, storm band "· storms off", P2 Crop Tree side panel | — | P1 copy changes (Crop Tree line, footer with `[F1] Help`) |
| **Stand Map** | Layers (light, regeneration, browsing, marks), cell diagnosis, waypoint | — | Cell management history (none in `StandMapView.cs`) |
| **Work Plan** | Quotes per job type, nursery, approval, salvage | **CANDIDATE:** cash warning, MONEY, WHAT YOU ARE LEAVING | Terminology ("cohort", raw species ids); approval summary before approve-all (stall S6) |
| **Annual Review** | WORK DONE · MONEY · FOREST columns, storm card, TRENDS, OBJECTIVES, Century Review | — | v2 sections and PLACES TO INSPECT (below) |

## 3. Progression

| Item | State | Evidence |
|---|---|---|
| **Objective visibility** | **IMPLEMENTED, weak.** HUD shows only the count; the list appears in Annual Review › OBJECTIVES. `[O]` opens the *learning* checklist, not the forest objectives | `WalkingHudView.cs:46,125`; `ScenarioOneUiRoot.cs:310–314` |
| **First-use teaching** | **IMPLEMENTED.** Five first-use help screens (F1 revisit) and an 8-topic learning checklist. Progress is stored in PlayerPrefs per device, not per forest | `MenuHelpView.cs:38,58`; `LearningObjectivesView.cs:103,127` |
| **Assistance** | **IMPLEMENTED at one fixed level** (always full help). Tiered assistance is PROPOSED (`ScenarioProgression/ProgressiveAssistance.md`) and belongs to multi-scenario work | — |
| **Second-look trigger** | **MISSING.** PROPOSED in `SecondInterventionDesign.md` (RD ≥ 0.6 / light-limited regeneration / planting loss; floor 5, fallback 12 years) | No trigger code on main |
| **Second separated intervention** | **MISSING.** Nothing asks for or recognises it | `ScenarioOneObjectives.cs:69–75` counts one felling |
| **Completion state** | **IMPLEMENTED.** All 8 objectives at a year-end ⇒ `Completed`; play continues to Year 100. `Failed` if cash is exactly €0 with no approved orders, or at Year 100 without completion | `ScenarioOneManager.cs:2285–2306` |
| **No universal forestry score** | **Holds today.** No score, grade or aggregate exists. Century Review says "not an optimal score or prescription". P3 avoids scores. The proposed "no single score" decision (E1) is still not recorded | `AnnualReviewView.cs:246` |

What the 8 objectives prove (code at `ScenarioOneObjectives.cs:54–96`, thresholds in `ScenarioOne.asset:53–58`): Year ≥ 25; ≥ 60 living trees of the original species; final-year mean canopy ≥ 0.35; ≥ 3 regenerating cells; ≥ 0.02 m³ fallen deadwood; one completed felling; one planted beech and one planted sessile oak present. Five of these are met without management under the current growth model (readiness study [INF], not re-run here). **None requires a second intervention.**

## 4. Annual Review v2 (proposed sections) against main

| Section | Main | Data already saved? | Gap |
|---|---|---|---|
| **WORK DONE** | Yes (counts by type, failures with reasons) | Events with cell, position, tree, species, executor | Place names (cells) per job |
| **MONEY** | Yes (costs, revenue, minimum, owner time) | `ScenarioAnnualReport` | — |
| **FOREST** | Yes (`ScenarioEcologyReviewLines`) | Snapshots each year | Raw species ids; trees "−felled, −natural deaths" split |
| **REGENERATION** | Mixed into FOREST lines | Snapshot per species; juvenile records | Separate section; per-cause planted losses |
| **DISTURBANCE** | Storm card only (dormant in new games) | Tree mortality cause/year, deadwood records, `stormEvents` | **Self-thinning deaths are never named in the UI** (no "self-thinning" string in `UI/`) |
| **PLACES TO INSPECT** | **MISSING** | Cell indices on events, deadwood, juveniles; waypoint API exists in `StandMapView` | Whole section |

## 5. History / Forest Diary

| Question | Answer |
|---|---|
| **Already stored (saved per forest)** | Yearly `ScenarioEcologicalSnapshot` (trees, basal area, mean DBH, DBH CV, light, canopy, regenerating cells, deadwood, per-species outcomes); yearly `ScenarioAnnualReport`; every `ScenarioManagementEvent` (year, type, outcome, **cell index, world position, tree id**, species, costs, executor); tree mortality cause/year; deadwood records; planted juveniles; storm events (save 19). `ScenarioOneManager.cs:17–20,1316–1340` |
| **Visible** | Latest year's review; TRENDS bars for cash and canopy over the last 12 years; objective values; Century Review comparisons. Nothing else |
| **Not stored** | Per-cell ecological history (when regeneration appeared in a cell), standing volume per year, Crop Tree CI/DBH per year, per-year browse history |
| **Minimum useful P4 (now S1-C)** | Read-only helper over saved records: a whole-forest timeline (intervention years plus every 5th year), a cell history list (events/deadwood/planting/deaths in that cell) in the Stand Map side panel, and PLACES TO INSPECT (≤ 3, deterministic, Set waypoint). **No save change.** |

## 6. Economy

| Item | Finding |
|---|---|
| **€2,500 minimum-job dead end** | Real on main: approval requires cash ≥ full cost (`ScenarioOneManager.cs:1145`), harvest is contractor-only, every harvest costs ≥ €2,500, and timber sale is the only income. Below €2,500 with no approved harvest, no income can ever follow. Main reports it only at Year 100 ("objectives not reached") |
| **Prospective cash warning** | **CANDIDATE (P3).** Warning when approved or pending work would leave expected cash below the minimum; dead-end explanation when already below; headroom line. Purchases still spend without a pre-purchase warning (P3 limitation) |
| **Recovery path** | **MISSING, decision required** (O2: standing sale / net settlement / none). Not required for beginner testing if the trap is warned and explained and a new forest can be started |
| **Sapling price authority** | Two values: player pays €4.50 beech / €5.50 oak (`ScenarioOneDefinition.cs:47–48`, `ScenarioOne.asset:32,37`, [D] provisional); the Stage 1 price book holds €0.95 / €1.00 [E] wholesale. Inert today (no cash path reads the book value for these items) but a latent hazard. P3 `SaplingPriceAuthorityAudit.md` recommends documenting the definition price as the authority and letting the adapter override the book (no cash change). Economy-owner decision |
| **Solvent without a hidden trap?** | **With P3 + a per-purchase warning + a "start a new forest" path: yes.** €12,000 start; the reference completion path stays solvent (last recorded lowest cash €6,139.75 under growth 1 / regeneration 1). **Not re-verified under the current save-19 stack** (the storm integration log records the v19 completion hash `84CD51EB6A951D9E` but not its year or lowest cash). Recheck in the S1-D completion gate |
| **Thinning always loses money on 0.16 ha** | Expected under the €2,500 [C] minimum; P3 now states the minimum as a scenario rule and that same-year felling shares one visit. Whether testers conclude "never thin" is a beginner-test question, not a pre-test change |

## 7. P3 — what it resolves and what remains

**Resolves (CANDIDATE, Unity-verified on branch):** "what you are leaving" (trees, basal area, volume, opened cells, robust pattern sentence, Crop Trees kept, Crop Tree competition, seed trees, deadwood); clearance consequences with Model 2 cover; expected cash after approved and pending work; dead-end warning and explanation; contractor-minimum teaching; salvage counted as harvest. Evidence: `P3ImplementationRecord.md` (offline PASS; batch ×2 hash `F7C2FC966816BBAD`; rendered PASS at three sizes; Storm Core 107 / Storm UI 1,167 PASS).

**Does not resolve:** pre-purchase warning; recovery route; terminology (species ids, "cohort"); readability V2 (13 px); approval summary; anything in Annual Review, history, progression or completion.

**Open issues found by P3:**
1. **MenuTutorial "Help Escape also closed inspection"** failed in P3's 24-gate regression (23/24). P2's regression on the same production code passed 24/24. P3 changes no input path. **Unclassified: must be reproduced on main before any UI packet or external test.**
2. **Mixed-species SDI mortality (confirmed on main in this audit):** `ForestEcologyController.ApplyAdultDensityMortality` (`:1010–1056`) builds the stand from every living tree and applies the Sitka density-mortality equations to all of them. Promoted oak and beech can be killed as "self-thinning". An ecology-model correctness issue for the ecology owner, not a UI packet.

**Duplication check:** readiness packet P3 and pedagogy packet P3 (residual-stand block) are fully covered by the P3 candidate, except the terminology and readability items of readiness P3, which move to S1-A. Readiness P7 (plan comparison) would reuse P3's summary; it does not duplicate it. No later packet in this sequence rebuilds anything P3 builds.

## 8. Second intervention

| Aspect | State |
|---|---|
| Implemented | Nothing. The player *can* manage again in any later year, but nothing prompts it, recognises it or rewards it. Completion needs one felling |
| Proposed | Trigger (`SecondInterventionDesign.md` §2), completion model D (`ScenarioCompletionDesign.md` §3: D1–D7), per-forest stage progress (save), "No work needed" event (save) |
| Smallest implementation that closes **decide → time → observe → reassess twice** | (1) A pure "forest has changed — look again" check from current state and saved events (relative density ≥ onset via `SitkaGrowthModel.RelativeDensity`, light-limited young trees via `RegenerationDiagnosis`, planted losses; floor 5 years after the first thinning, fallback 12), shown in the Annual Review and HUD as a reason, **never an instruction**. (2) Completion requires a **second resolved management cycle of any work type at least 5 years after the first release**, derived from saved events. (3) The trigger is teaching only, not a completion condition, so no trigger state needs saving. **No save change.** Completion anchor changes (authorisation required) |

## 9. Storms

| Item | State |
|---|---|
| Core | **ACCEPTED + INTEGRATED, dormant** (D-050). New Scenario One games are storm model 0 |
| Player-facing UI already on main | Stable/Watch/Exposed band with "· storms off", forecast "wind exposure … (storms off)", salvage via X, Work Plan salvage, Annual Review storm card, waypoint. `StormUiVerification` PASS (1,167 checks) |
| Not player-ready | Activation policy, crown/root art, combined century rendering, standalone Player profiling, storm lessons (D-050 "open") |
| Blocks Scenario One external testing? | **No.** Storms are off in Scenario One and the progression study recommends keeping them off. One watch item for the beginner test: whether "storms off" wind labels confuse players (observe; do not pre-fix) |

## 10. Manual acceptance still required

| Check | Status |
|---|---|
| P2 human smoke (inspect, C, read list, 1–5, X/unmark, close, three resolutions) | **NOT PERFORMED** (P2 record) |
| P3 human review (read the block for realistic plans; warning appears/clears; clearance consequences) | **NOT PERFORMED** |
| MenuTutorial Help-Escape failure | **Unclassified** (see §7) |
| Model 2 human clearance decisions and bramble/bracken recognition | **Open** (D-049) |
| P1 copy rendered review on the current stack | Never done on v19 (P1 evidence is v16) |
| Worker-worktree Unity smoke gates | Open (Current Milestone) |
| Standalone Player run and profiling | **Never done** (no build script or build record in the repository) |

## 11. External testing facts

| Fact | Evidence |
|---|---|
| No standalone build has been produced | No `BuildPipeline`/build record in `Tools/`, `Docs/` or `Assets/` |
| Only `ForestTest.unity` is in Build Settings | `ProjectSettings/EditorBuildSettings.asset` |
| Save/load only via F5/F9; no autosave; no on-screen save path | `ForestSaveController.cs:33–36` |
| No quit and no "new forest" action in the UI | No `Application.Quit`; no reset entry point |
| Learning/help progress per device (PlayerPrefs) | A second tester on the same machine skips all introductions |
| Ultimate Nature visuals are a gitignored Asset Store dependency | `.gitignore:80–84`; `Assets/ForestPrototype/Docs/UltimateNatureSetup.md`. The build machine must have the pack; confirm the licence allows a private test build |
| Verification-only scripts in `Assets/` are editor-guarded | `#if UNITY_EDITOR` on `RecentAssetReviewVerification`, `ScenarioHabitatPresentationVerification`, `ScenarioOneInteractionVerification` |
| Regression runner depends on an unmerged commit | `Tools/Verification/WindthrowV1/run_regression.py:8` overlays harness sources from P1 `1a36ea9` |

## 12. Canonical documentation drift (Manager action, not this task)

- P2 is on main but is not recorded in the Unity Project Overview or the Decision Log; `current-milestone.md` still heads with the setup milestone.
- `active-tasks.md` has no P2/P3 rows.
- `PostScenarioRoadmap.md` (readiness branch) recommends Stage 2 before Scenario Two; the progression study marks this superseded. Record it if the user accepts the forestry-first direction.
