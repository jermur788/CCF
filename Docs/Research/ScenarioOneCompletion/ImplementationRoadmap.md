# Implementation roadmap (Workstream Z)

**Status:** proposal for Manager review. The order is derived from **file ownership, save-queue position, anchor impact and teaching dependencies**. It is not taken from the packet's candidate numbering. Packet files are in `Packets/`.

## 1. Dependency facts

| Fact | Consequence |
|---|---|
| P1 (copy) is a **candidate** on `3e4ee40`. It merges cleanly into `a8596df` (`git merge-tree`), but its copy gate expects save v16 | Every UI packet edits the same UI files → **integrate P1 first** (re-gate at v17) |
| Sol owns ecology integration: Model 2 (save v18) then Storms (v19). The storm packet locks `UI/*` and `ForestTreeMarkingManager` to the pedagogy/UI owner; Sol locks `ScenarioOneManager`, save and ecology | UI-only packets (P2, P3, P4, P5-A) can run **in parallel** with Sol. Anything editing the manager, save or objectives waits for the storm integration |
| Model 2 changes clearance meaning | Model-dependent clearance copy ships **with or immediately after** M2 integration |
| Storms need a DISTURBANCE section and stability labels | Annual Review v2 (P4) should land **before** the storm UI (W5) |
| Completion and failure changes alter the completion anchor and outcome semantics | **Last**, with explicit authorisation, after storms (viability calibration) |
| The beginner playtest needs P1 + P2 + P3 + readability blockers + the S1 warning + asset packets 01/07 | Playtest 1 sits after P3, **before** progression/completion redesign, so evidence shapes P5/P6 |
| Fixed new-game seed + storms → the same storm years for every player | A Storm teaching decision is needed before the storm UI packet |

## 2. Recommended sequence

```
 0  P1-INT   Integrate pedagogy P0+P1 copy onto main (re-gate at v17)        [UI owner]
 1  P2       Crop Tree competitor reasoning + forecast panel                 [UI owner]   ‖ Sol: M2 integration (v18)
 2  P3       "What you are leaving" + S1 cash warning + terminology fixes    [UI owner]   ‖ Sol: M2 integration
 3  H1       Beginner playtest 1 + forester review (human)                   [Manager/user] (needs asset packets 01, 07)
 4  P4       Annual Review v2 + Forest Diary + map history                   [UI owner]   ‖ Sol: Storms W1/W2 (v19)
 5  P5       Progression restructure, Tier A (UI only) + M2 teaching copy    [UI owner]   (M2 copy after M2 lands)
 6  P7       Plan comparison (marteloscope B-lite)                           [UI owner]   (optional; after P3)
 7  W5       Storm UI and teaching (stability bands, DISTURBANCE, salvage UI) [UI owner, after Sol W1–W4]
 8  P6       Second intervention + completion model + failure/recovery       [single writer: manager/objectives/save; after storms]
 9  H2/P8    Beginner playtest 2 → playtest fixes                            [UI owner + Manager]
10  Feature-complete gate (ScenarioOneDefinitionOfDone Tier 3)
```

`‖` = can run in parallel because file ownership does not overlap.

## 3. What can be built before and after storms

| Before storms | After storms |
|---|---|
| P1-INT, P2, P3, P4 (with an empty DISTURBANCE section that already shows self-thinning deaths), P5 Tier A, P7 B-lite, M2 teaching copy (after M2), playtest 1 | Stability bands in inspection/forecast/P3, storm DISTURBANCE content, salvage teaching, storm second-look trigger, completion calibration under storms (P6), training-mode cases 6 and 11 |

## 4. File ownership matrix

● writes · ○ reads · — untouched

| File | P1-INT | P2 | P3 | P4 | P5 | P7 | W5 | P6 | P8 | Sol M2 / Storms |
|---|---|---|---|---|---|---|---|---|---|---|
| `UI/TreeInspectionView.cs` | ● | ● | — | — | — | — | ● | — | ● | locked |
| `UI/WorkPlanView.cs` | ● | — | ● | ● (History button) | — | ● | ● (salvage) | — | ● | locked |
| `UI/AnnualReviewView.cs` | ● | — | — | ● | ○ | — | ● | ● (completion view) | ● | locked |
| `UI/StandMapView.cs` | ● | — | ○ | ● | — | — | ● (layer) | — | ● | locked |
| `UI/WalkingHudView.cs` | ● | ● (forecast panel) | ○ | — | ● | — | — | — | ● | locked |
| `UI/MenuHelpView.cs`, `UI/LearningObjectivesView.cs` | ● | — | — | — | ● | — | ● | — | ● | locked |
| `UI/ScenarioOneUiRoot.cs` | — | — | — | — | ● | ● | — | — | ● | locked |
| new `UI/CropTreeNeighbours.cs` (helper) | — | ● | ○ | — | — | ○ | ○ | ○ | — | — |
| new `UI/ResidualStandSummary.cs` (helper) | — | — | ● | — | — | ○ | ○ | — | — | — |
| new `UI/ForestHistory.cs` (helper) | — | — | — | ● | ○ | — | ○ | ○ | — | — |
| new `UI/ScenarioProgress.cs` (stages) | — | — | — | — | ● | — | ○ | ● | — | — |
| `ForestTreeMarkingManager.cs` | ● | ● | — | — | — | ● (mark sets) | ● | — | — | locked |
| `ScenarioOne/ClearancePreview.cs`, `VegetationClearance.cs` | ● | — | — | — | ● (M2 copy) | — | — | — | — | ○ |
| `ForestPlayer.cs` | — | — | — | — | ◐ (preview behind U, if decided) | — | — | — | — | — |
| `ScenarioOne/ScenarioOneManager.cs` | ○ | ○ | ○ | ○ | ○ | ○ | ○ | **●** | — | **● Sol** |
| `ScenarioOne/ScenarioOneObjectives.cs` | — | — | — | — | ○ | — | — | **●** | — | ○ |
| Save files | — | — | — | — | — | — | — | ● (progress record, if decided) | — | **● Sol** |
| `ForestEcologyController.cs` | ○ | ○ | ○ | ○ | ○ | ○ | ○ | ○ | — | **● Sol** |
| Scenes / prefabs / ProjectSettings / packages | — | — | — | — | — | — | — | — | — | — |
| `Tools/Verification/*` | ● copy gate | new | new | new | ● MenuTutorial | new | new | ● Completion (anchor) | ● | ● Sol gates |

**Hot spots:** WorkPlanView (P1, P3, P4, P7, W5, P8) and TreeInspectionView (P1, P2, W5, P8). Same owner: run sequentially, never in parallel worktrees.

## 5. Anchors

P1-INT, P2, P3, P4, P5-A, P7 and W5 are presentation only: **every lifecycle, completion and Reference anchor must stay unchanged** (a drift is a stop condition). P6 intentionally changes the completion anchor, only with explicit authorisation, and records the new value.

## 6. Human gates

- H1 (after P3): beginner playtest 1 + forester review → P8a fixes (copy/UI), and evidence for P5/P6 decisions.
- H2 (after P6): beginner playtest 2 → P8b.
