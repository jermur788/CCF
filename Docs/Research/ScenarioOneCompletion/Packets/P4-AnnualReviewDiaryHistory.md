# P4 — Annual Review v2, Forest Diary v1, map cell history

```text
TASK PACKET
GOAL: After each year the player sees what they did, what it cost, what changed,
      and up to three places worth walking to; can read the forest's history;
      and can see a cell's management history on the map.
BRANCH: task/scenario-one-p4-review-history
```

**Start authority:** P3 integrated; decisions G1, H1, I1. Design: `AnnualReviewV2.md`, `ForestDiaryV1.md`, `MapHistoryDesign.md`. Must land **before** the storm UI (W5).

**Scope**

1. New read-only helper `UI/ForestHistory.cs`: property timeline rows (snapshots, reports, events, mortality cause/year, deadwood records, planted records); cell history entries; PLACES TO INSPECT ranking (deterministic priority, `AnnualReviewV2.md` §4).
2. `AnnualReviewView`: sections WORK DONE · MONEY · FOREST · REGENERATION · DISTURBANCE (self-thinning deaths now; storms later) · PLACES TO INSPECT (≤ 3, Set waypoint via `StandMapView`'s existing waypoint API) · progress. A History tab (the diary).
3. `StandMapView.BuildSide`: History list (≤ 8 lines) for the selected cell; optional "History" layer with work-year counts (numeral + colour).
4. Soft review nudge for later years (PLACES scrolled into view before "Walk the forest" is highlighted). Nothing blocks.

**Owned files:** `UI/AnnualReviewView.cs`, `UI/StandMapView.cs`, `UI/WorkPlanView.cs` (History button only), new `UI/ForestHistory.cs`, new harness.

**Locked:** manager, save, ecology, objectives, `ScenarioEcologyReviewLines` logic (may be called, not changed beyond P3's names).

**Exclusions:** storm content; recommendations; projections; saved history additions (H2 decision).

**Save impact:** none.

**Test plan**

- Batch: a 15-year scripted game (thinning Y1, planting Y6, clearance Y8). The timeline equals saved data; the cell history for the thinned cells lists the felling events; PLACES order is deterministic; save/load → identical text.
- Batch: legacy v15/v16/v17 saves render a timeline (missing fields tolerated); Reference preview shows no player history.
- Batch: forbidden causal pattern deny-list ("caused", "because you", "helped the seedlings", "should").
- Interactive: Set waypoint from PLACES → HUD arrow → arrival detection; rendered at three resolutions.
- Anchors unchanged.

**Manual review:** play 10 years; does PLACES send you somewhere worth looking at? Does the diary answer "what happened here"?

**Stop conditions:** needing a saved field; anchor drift.

**Handoff:** IMPLEMENTATION HANDOFF.
