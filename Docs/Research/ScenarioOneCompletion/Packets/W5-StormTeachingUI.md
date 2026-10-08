# W5 — Storm UI and teaching (UI owner)

```text
TASK PACKET
GOAL: Players see tree stability (Stable/Watch/Exposed with a reason), learn from
      the first storm through the Annual Review and a one-time explanation, can
      walk to the damage, and choose salvage or deadwood in the Work Plan.
BRANCH: task/scenario-one-storm-ui
```

**Start authority:** Sol's Storms W1–W4 integrated (core, calibration, visuals, salvage) and the storm packet's W5 handed to the UI owner; decisions K1, K2, PD-K1. Design: `StormTeaching.md`; storm `PlayerFeedback.md` and `TutorialHandoff.md` (`9a9f4fb`).

**Scope**

1. `TreeInspectionView`: replace "Wind exposure" with the stability band + reason line + H/D context (from Sol's V/band API; no probability).
2. Marking forecast and P3: "n trees move to Exposed for a few years (cells…)". Remove "wind peak".
3. Annual Review DISTURBANCE: storm section with co-occurrence wording; storm places at PLACES priority 1; Set waypoint to the largest new gap.
4. One-time explanation cards (first Watch/Exposed tree, first exposing thinning, first storm, first salvage decision): ≤ 3 sentences each; per-forest via the progress record if P6 has landed, otherwise per session.
5. Work Plan salvage card UI over Sol's salvage orders (Sell / Keep / Leave), including the cost-multiplier note.
6. Optional map "Stability" layer (counts per cell, opened-cell marker).

**Owned files:** `UI/TreeInspectionView.cs`, `UI/WorkPlanView.cs`, `UI/AnnualReviewView.cs`, `UI/StandMapView.cs`, `UI/MenuHelpView.cs`, `ForestTreeMarkingManager.cs` (forecast), `UI/ResidualStandSummary.cs`, harness.

**Locked:** storm core (Sol), manager, save, ecology, objectives.

**Exclusions:** storm parameters; a numeric storm chance anywhere; "your thinning caused" wording.

**Save impact:** none (beyond P6's progress record if used).

**Test plan:** harness forces a storm (editor-only API) in a fixture → review section text and places; string deny-list (probability %, "caused", "safe"); bands match Sol's API for 20 fixture trees; rendered at three resolutions; storm anchors unchanged.

**Manual review:** the first-storm experience on the default seed (confirm the first storm year per PD-K1).

**Stop conditions:** needing to change storm logic or parameters; anchor drift.

**Handoff:** IMPLEMENTATION HANDOFF.
