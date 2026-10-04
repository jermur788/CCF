# Scenario 1 five-screen UI — implementation audit (before coding)

Branch `task/scenario-one-ui-redesign`, from `main` @ `a123eec` (presentation baseline integrated). Source: `CCF_UI_Mockups_source.zip` (Main, TreeInspect, StandMap, WorkPlan, AnnualReview `.dc.html`) and the UI direction handoff. **No production code has been changed yet.**

## 1. Current UI: who owns what

Every Scenario 1 screen today is IMGUI (`OnGUI`), drawn inside simulation-owning components:

| Current owner / file | Size | Draws today |
|---|---|---|
| `ScenarioOne/ScenarioOneManager.cs` (3,020 lines, of which about 630 are `OnGUI` and `Draw*`) | 184 GUI calls | Corner HUD (cash, stock), Work Plan (one scroll column: annual review, Reference buttons, nursery, executor choice, harvest quote, spatial summary, order list, pruning), Reference-preview banner |
| `ForestPlayer.cs` (1,175 lines) | 49 GUI calls | Tree inspection card, interaction prompt, carried-wood panel, planting-mode text |
| `ForestTreeMarkingManager.cs` (694 lines) | 11 GUI calls | Mark prompt, marking summary line, ground report, juvenile diagnosis line, treatment outcome |
| `ForestHud.cs` | small | Shared scale (`ForestHud.Scale`) and panel background |
| `ForestEcologyController.cs` | debug | Time-lapse label and debug cell grid (**not** a player map) |

No UI Toolkit or uGUI screens exist. `com.unity.modules.uielements` (built in) and `com.unity.ugui` 2.6.0 are already in the manifest, so no new package is needed for either.

## 2. Screen-by-screen audit

| Target screen | Current owner / file | Data source (authoritative) | Action source | Keep / replace | Proposed technology |
|---|---|---|---|---|---|
| **1 Walking HUD** (scenario/year/phase, cash, objectives, browse/protection summary, Map and Work Plan access, ground report, tool, marking summary) | `ScenarioOneManager.DrawMainHud`, `ForestTreeMarkingManager.OnGUI` (summary, ground report, diagnosis), `ForestPlayer.OnGUI` (prompt, carried wood) | `ScenarioOneManager` (cash, `Objectives`, stock, `Shelters`), `ForestEcologyController.RegenerationReportLine`, `RegenerationDiagnostics`, `AssessIndividualBrowse` / `AssessCohortBrowse`, marking counts | existing key handlers (Tab, M, C, E, G) stay in their components | **Replace** the drawing; keep all inputs | UI Toolkit |
| **2 Tree Inspection** | `ForestPlayer` inspection card | `ForestTree` (id, species, origin, mark, DBH, height, `BiologicalStemVolumeM3`, pruning), `ForestEcologyController` (CI and competition label, cell light, last-year DBH growth, wind label), `TimberYieldCalculator` (if-felled estimate) | `ForestTreeMarkingManager.Mark`, Fell/Crop keys | **Replace** the card; keep `InspectTree` state | UI Toolkit |
| **3 Stand Map** | **does not exist** (only a debug grid) | `ForestEcologyController.Cells` (light, cohorts), browse/protection assessments, shelters/areas, tree marks and positions, `StandBounds`, track geometry | read-only; "Set waypoint" is a presentation marker | **New** | UI Toolkit (+ a generated texture or element grid) |
| **4 Work Plan** | `ScenarioOneManager.OnGUI` Work Plan column | `workOrders`, `GetHarvestQuote` (WorkEconomy/TimberYield resolution), `GetPlantingQuote`, definition (prices, owner minutes, shelter material), inventory | existing public API: `ApprovePendingWork`, `AdvanceYear`, `RemovePendingOrder`, `CancelApprovedOrder`, `SetPlantingExecution`, `SetPlantingShelters`, `TryPurchaseStock`, `BatchPruneCropTrees`, Reference preview | **Replace** the drawing; actions unchanged | UI Toolkit |
| **5 Annual Review** | `ScenarioOneManager.DrawAnnualReview` (inside the Work Plan scroll) | `AnnualReports` (sales by product, costs, minimum adjustment, kept-for-use volume, deadwood created, owner minutes), `EcologicalSnapshots`, `ScenarioEcologyReviewLines`, `Objectives` | open/close, "Open Work Plan", "Walk the forest" | **Replace**; its own screen | UI Toolkit |

## 3. Recommendation: UI Toolkit (runtime `UIDocument`) for these five screens

**Why:**
- **Responsive layout.** The flex layout and USS give consistent margins and wrapping at 1280/1600/1920 without hand-computed rectangles. The presentation review just fixed three IMGUI clipping and overlap bugs caused by fixed sizes.
- **Shared visual tokens.** One USS theme covers colours, panel style and type scale, matching the mockups' visual language.
- **Separation.** Five small presenter classes read authoritative state and call existing public actions. About 800 lines of drawing code leave `ScenarioOneManager`, `ForestPlayer` and `ForestTreeMarkingManager`, which reduces the 3,000-line manager instead of growing it.
- **Testability.** Presenters can be checked by querying the visual tree (labels and values) in a harness, not only by screenshots.
- **Cost.** No package dependency. Unity 6000.6 runtime UI Toolkit is mature.

**Effort and risks:**
- **Input.** The project uses the new Input System; runtime UI Toolkit needs an `EventSystem` with `InputSystemUIInputModule`, or the panel's own input handling. Pointer and keyboard focus must not steal the existing gameplay keys.
- **Assets.** One `PanelSettings` asset plus UXML/USS files are new serialized assets.
- **Scene.** Adds a scene object, the UI root, to `ForestTest.unity`. That is a high-conflict serialized file, needing single-writer coordination.
- **Bridge.** During migration each old IMGUI block is disabled as its replacement lands. No permanent duplication.

**Alternative (bounded IMGUI cleanup):** cheaper to start, but it keeps the drawing in the simulation classes. Adding a map and a three-section review there would grow `ScenarioOneManager` further. Not recommended.

## 4. Mockup items that cannot be shown truthfully today

| Mockup item | Status | Handling |
|---|---|---|
| "Leader risk 9%/yr" | per the handoff | show qualitative Low/Moderate/High bands from the assessed probability (`BrowsingConditions.PressureBand`-style thresholds), plus protection state |
| "Thinning the 3 trees to the south would raise…" | prescriptive | removed; diagnosis only |
| "Walk to this cell" | movement | "Set waypoint" (presentation marker, never moves the player) |
| Fencing recommendation | not gameplay | not shown |
| "harvester and forwarder crew" | machinery not simulated | "Contractor only — specialist harvesting work" |
| Tonnes in the harvest table | **partly real**: WorkEconomy prices harvest/forwarding work on green mass (density × volume); TimberYield products are cm³ | products and values shown in **m³** (production unit); the work line may show green tonnes because that is the actual cost basis; to confirm |
| "Landowner option for pruning comes later" | true (contractor only) | state only "Contractor"; no future-promise text |
| **Diameter growth per year, last 6 years** | **no per-tree history exists**: only last year's ΔDBH, cleared on load | **decision needed**: (a) show last year's growth only; (b) a presentation-side history that resets on load; (c) a saved history (new save fields, out of scope) |
| "Crown light" | no per-crown light; the tree's ground cell light is the available proxy | label it "light at the tree's cell" |
| Cell names "D5" | cells are indices | a derived label (column letter + row number) is presentation-only and fine |
| Shelter price / life | `ScenarioOneDefinition.TreeShelterMaterialCents`; the effective life is set where the shelter is created (8, literal in `ResolvePlanting`) | read the price from the definition. **The 8-year life is a literal in production code, not configuration**: expose it as a named read-only constant (behaviour-identical) so the UI reads it, or leave the duration unstated |
| Map key "[M]" | **conflict**: M already marks Fell | needs a new key (proposal: **N** for "navigation map"; free) |

## 5. Proposed file ownership

New, owned by this task:
- `Assets/ForestPrototype/UI/` holding `ScenarioOneUi.uss` (tokens), `PanelSettings`, `*.uxml`;
- presenters `WalkingHudPresenter.cs`, `TreeInspectionPresenter.cs`, `WorkPlanPresenter.cs`, `AnnualReviewPresenter.cs`, `StandMapPresenter.cs`;
- `ScenarioOneUiRoot.cs` (screen switching, key routing for Tab / N / Esc).

Edited, shared:
- `ScenarioOneManager.cs` (remove `OnGUI` drawing blocks; expose any missing read-only accessors);
- `ForestPlayer.cs` (remove card drawing; expose inspection target);
- `ForestTreeMarkingManager.cs` (remove HUD text drawing; expose report data);
- `ForestTest.unity` (add the UI root object).

**Coordination with the active RNG branch** (`task/scenario-one-rng-model1-default`):
- it changes only `ScenarioOneManager.Awake` and the `NewGameRngModel` constant, which the UI task will not touch;
- merge order: RNG first if it is approved in time; otherwise rebase the UI branch, since the hunks do not overlap.

## 6. Migration sequence (one screen at a time, old block removed in the same commit)

1. Shared tokens, layout primitives, UI root, input routing (Tab / N / Esc) and `PanelSettings`. Verify keyboard and mouse coexistence with gameplay.
2. Walking HUD (replaces `DrawMainHud`, marking summary, ground report text).
3. Tree Inspection (replaces the `ForestPlayer` card).
4. Work Plan (replaces the Work Plan column; Annual Review temporarily a section).
5. Annual Review as its own screen: Work done / Money / Forest, retained usable timber and retained deadwood lines, compact trends.
6. Stand Map: light, regeneration, browse/protection and marks layers; cell inspection; waypoint.
7. Remove the remaining superseded IMGUI; keep debug and non-scenario IMGUI (survival modes, time-lapse, Reference banner if not migrated).
8. Full regression (anchors unchanged), save/load, Reference, rendered acceptance at 1280/1600/1920, and performance.

## 7. Decisions requested before coding

1. **Technology:** UI Toolkit (recommended) vs bounded IMGUI cleanup.
2. **Tree growth history:** last-year only / presentation-side (resets on load) / saved (out of scope).
3. **Map key:** N (proposed), since M is Fell.
4. **Font:** use Unity's built-in runtime font (no licensing work) unless a specific licensed font is supplied.
