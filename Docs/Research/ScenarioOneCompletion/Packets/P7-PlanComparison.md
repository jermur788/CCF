# P7 — Plan comparison (marteloscope B-lite)

```text
TASK PACKET
GOAL: Before approving a thinning, the player can keep up to three alternative
      mark sets, switch between them in the forest, and compare their immediate
      consequences side by side, without advancing time.
BRANCH: task/scenario-one-p7-plan-comparison
```

**Start authority:** P3 integrated; decision **Q1** (B-lite now, C later, no time-advanced copy). Design: `MarteloscopeFinalDesign.md` §3.

**Scope**

1. `ForestTreeMarkingManager`: named mark sets A/B/C (session-only in v1): store the current Fell + Crop marks as a set; restore a set (replacing current marks); delete a set.
2. `WorkPlanView`: "Compare plans" card: per set, the P3 summary lines in columns (no totals, no ranking); "Use plan A" restores its marks (then the normal Add marked trees/approve flow).
3. Optional HUD indicator "Plan B active".

**Owned files:** `ForestTreeMarkingManager.cs` (mark-set API), `UI/WorkPlanView.cs` (card), `UI/WalkingHudView.cs` (indicator), new harness.

**Locked:** manager, save, ecology, objectives.

**Exclusions:** time advance on copies; practice mode (Q2); saving mark sets (later decision).

**Save impact:** none (session-only sets; lost on reload, stated in the UI).

**Test plan:** batch: store T2 as A and T5 as B; switch; the summaries equal P3 fixtures; restoring A gives identical marks (ids); determinism. Interactive: switching sets updates world marks; no stale markers. Anchors unchanged.

**Manual review:** does comparison invite "optimising a hidden score"? Check the wording.

**Stop conditions:** mark restore touching save/manager; anchor drift.

**Handoff:** IMPLEMENTATION HANDOFF.
