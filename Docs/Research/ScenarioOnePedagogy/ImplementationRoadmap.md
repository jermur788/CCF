# Implementation roadmap (Workstreams G3–G5)

**Status:** proposal for Manager review. Nothing here is accepted. Each packet's acceptance depends on the decisions in `DecisionMatrix.md`.

## 1. Dependency facts that drive the order

| Fact | Consequence |
|---|---|
| Sol's branch `task/sitka-site-height-adult-mortality` (@ `187079c`, unmerged) changes `ForestEcologyController`, `ForestSaveController`, `ForestSaveData` (save **v17**), `ForestSaveValidation`, `ScenarioOneManager`, `ScenarioReferenceArchive` and several gate harnesses. It does **not** touch `Assets/ForestPrototype/UI/*`, `ForestTreeMarkingManager`, `ForestPlayer`, or the three harnesses fixed here | UI-only packets can proceed in parallel with Sol. Anything touching the save, the manager or ecology waits |
| The save schema is single-writer | Per-forest progress, the "no thinning" event, purpose notes, plots and Bio Tree all wait for Sol's v17 to land |
| Objective changes alter the completion anchor | Objective redesign is last, with explicit anchor authorisation |
| The harness numbers depend on growth/mortality | Re-run `ResidualStandEvaluation` after Sol's integration before final copy numbers are fixed |
| Sol's evidence: the €2,500 minimum keeps every thinning net-negative until about Year 40 (unmerged `EconomyTutorialImpact.md`, consistent with this work's Year-0 finding) | "Harvest for income" cannot be experienced inside the Year-25 tutorial window. Economy decision #28 |

## 2. Recommended sequence

```
P0  Gate run-mode manifest (tooling)                       ── now, parallel
P1  Teaching copy + framing (UI strings only)              ── now, parallel with Sol
P2  Positive-selection feedback (inspection + forecast)    ── after P1 (shares HUD/inspection files)
P3  Residual-stand Work Plan block                          ── after P2 (shares the read-only metric helper)
P5  Annual Review history, Phase 1 (no save)               ── after P1; parallel with P2/P3 if file ownership holds
—— Sol's growth/mortality + save v17 integrated; re-run the residual harness ——
P6  Tutorial arc re-sequencing + per-forest stage progress (save)   ── decision #8
P4  Practice mode (marteloscope), C1 then C2               ── decision #16
P7  Objective redesign (anchor change)                      ── decisions #21/#22
P8  Permanent sample plots (save)                           ── decision #19
P9  Optional Bio Tree                                       ── decision #17, when habitat-tree state exists
Calibration: wind/competition label thresholds              ── decisions #26/#27, after Sol's height model
```

The example ordering in the task packet ran 1 Tutorial content → 2 Tree Inspection → 3 Work Plan → 4 Marteloscope → 5 Annual Review → 6 Bio Tree. The dependency-based order differs in two places:

- **Annual Review history Phase 1 moves earlier.** It needs no save change and uses data that already exists.
- **Tutorial arc re-sequencing moves later.** Its per-forest progress needs a save change, so it waits for Sol's v17.

Copy and framing (P1) can still go first.

## 3. File ownership per packet (G4)

Legend: ● writes, ○ reads only, — untouched.

| Files / subsystem | P0 | P1 | P2 | P3 | P5 | P6 | P4 | P7 | P8 | P9 | Sol (current) |
|---|---|---|---|---|---|---|---|---|---|---|---|
| `UI/MenuHelpView.cs` | — | ● | — | — | — | ● | — | — | — | — | — |
| `UI/LearningObjectivesView.cs` | — | ● (text) | — | — | — | ● | ○ | — | — | — | — |
| `UI/WalkingHudView.cs` | — | ● | ● | — | ● (place history) | ○ | ● (banner) | — | ● | ● | — |
| `UI/TreeInspectionView.cs` | — | ● | ● | — | — | — | — | — | — | ● | — |
| `UI/StandMapView.cs` | — | ● | — | — | ● | — | — | — | ● | ● | — |
| `UI/WorkPlanView.cs` | — | ● | — | ● | — | ● | ● | — | — | — | — |
| `UI/AnnualReviewView.cs` | — | ● | — | — | ● | — | — | ● | ● | — | — |
| `UI/ScenarioOneUiFacts.cs` | — | ● | ○ | ○ | ○ | — | — | — | — | — | — |
| `UI/ScenarioOneUiRoot.cs` | — | — | — | — | — | ● | ● | — | — | — | — |
| new `UI/ResidualStandSummary.cs` (read-only helper) | — | — | ● | ● | — | — | ○ | — | — | — | — |
| new `UI/ForestHistory.cs` (read-only helper) | — | — | — | — | ● | ○ | — | — | ○ | — | — |
| `ForestTreeMarkingManager.cs` | — | ● (messages) | ● (forecast split) | — | — | — | ○ | — | — | ● | — |
| `ForestPlayer.cs` | — | ◐ preview-on-U (decision #9) | — | — | — | — | ● (practice input guard) | — | ● (plot placement) | — | — |
| `ScenarioOne/ScenarioOneManager.cs` | — | — | — | — | — | ● (events) | ● (sandbox) | ● | ● | ● | **● Sol** |
| `ScenarioOne/ScenarioOneObjectives.cs` | — | — | — | — | — | — | — | ● | — | — | — |
| `ForestSaveData.cs` / `ForestSaveValidation.cs` / `ForestSaveController.cs` | — | — | — | — | — | ● | ● (save block) | — | ● | ● | **● Sol (v17)** |
| `ForestEcologyController.cs` | — | — | ○ | ○ | ○ | — | ○ | — | ○ | — | **● Sol** |
| `ForestTree.cs` / marks enum | — | — | — | — | — | — | — | — | — | ● | — |
| Economy (`WorkEconomy/*`, definition prices) | — | — | — | — | — | — | — | — | — | — | — |
| Scenes / prefabs / ProjectSettings | — | — | — | — | — | — | — | — | — | — | — |
| `Tools/Verification/*` | ● launcher | ● Clearance/MenuTutorial if #9 | new UI checks | new | new | ● MenuTutorial | new | ● Completion (anchor) | new | ● | ● Sol (gate harnesses) |

**Conflict hot-spots:** `WalkingHudView` (P1, P2, P5, P4, P8, P9), `ScenarioOneManager` (Sol, then P6, P4, P7, P8, P9) and the save files (Sol, then P6, P4, P8, P9). Sequence packets on each hot-spot and do not run two of them in parallel.

## 4. Test plan for future implementation (G5)

Durable tests belong in disposable `Tools/Verification` harnesses (the project pattern), run in the stated mode.

### Beginner flow (P1, P6) — **interactive mode**

1. No forest objective or lesson stage completes without its observed action (fresh profile; scripted no-op years).
2. Each menu introduction appears exactly once per profile and is revisitable with F1 (existing `MENU_HUD_PASS` pattern).
3. The map lesson requires a waypoint set from another cell, arrival and on-foot inspection (existing checks kept).
4. The later-intervention stage cannot be completed by the first intervention or by two approvals in one year.
5. The Annual Review acknowledgement gate is unchanged (`MENU_ANNUAL_GATE_PASS`).
6. String tests: the opening card contains a CCF definition; the clearance text names young trees; no string claims a clearance growth benefit; no prescriptive phrases ("you should cut", "correct tree").

### Positive selection (P2) — **batch**

7. The relationship line's rank and share equal a `HegyiTerm` recomputation for the fixture Crop Tree.
8. Fixture: Crop Tree P0707 — removing P0710 changes its competition by ≤2 %; removing P0706 by ≥7 % (re-baseline after Sol).
9. The forecast shows the Crop-Tree figure separately from the stand figure; T1/T2 fixture marks show the documented inversion.

### Residual stand (P3) — **batch**

10. Equal removed volume with a different spatial pattern gives different spatial lines (T2 vs T5 fixture).
11. Harvested value and retained capital are both present for any non-empty plan.
12. No "score", "grade" or "rating" field exists (static test over the review model).
13. Determinism: the same marks give identical review text across two processes.

### History (P5) — **batch**

14. History is identical before and after save/load (derived only from saved data).
15. "What to inspect next" contains no imperative from the deny-list; ordering is deterministic.
16. The Reference preview shows no player history.

### Practice mode (P4) — **batch + interactive**

17. Deterministic reset: restore → world hash equals the pre-practice hash (pattern `RESIDUAL_STAND_WORLD_RESTORED`).
18. The same marking gives the same immediate assessment in two processes.
19. Long-term repeat (5/20 years) is deterministic in-process and across processes (pattern: this harness).
20. F5 is blocked during practice; objectives and learning progress are unchanged.

### Anchors

21. P1–P3 and P5 must leave every lifecycle, completion and Reference anchor unchanged (they are presentation only).
22. P7 changes the completion anchor **only with explicit authorisation**; record the new anchor.

## 5. Playtest protocol (beginner comprehension)

Source inspection cannot certify comprehension. After P1–P3:

- 3–5 novices, think-aloud, 45 minutes, from a fresh profile.
- Ask at three points (after the first marking, after the first Annual Review, after Year 6): *"What are you trying to achieve? Why did you choose those trees? What would you do differently?"*
- Score each answer against outcomes O1–O7 (`LearningOutcomes.md`) as **explained / partly / not**. This is research data, not an in-game score.
- Record any moment where a player says a number "tells them what to cut". That is a prescriptiveness failure.
