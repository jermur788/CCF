# Multi #900 — Approved plan (engine-first)

**Status: APPROVED (D-094, 2026-11-23).** No map overlay, no Unity, no save change.

**Goal:** compute and prove honest canopy cover, growing-space diagnostics and a modelled light index inside the pure 2D engine, reacting to real Execute and annual crown change.

**P0 — Evidence (done, read-only):** #899 decided (C2); MA-A/B geometry and shared boundary traced; Unity kernel inspected; dead/stump policy verified; existing pure-Core contracts and fenced commands confirmed.

**P1 — Oracle (done/in progress, offline):** replicate the Unity 5 m kernel in a pure numeric oracle; confirm `b = 2.5 m` decoupling equals Unity at 5 m; define the 2.5 m default and clipped boundary weighting. No production code before the lane receipt.

**P2 — Approval (done, D-094):** owner fixed the light model as the labelled Unity proxy, 2.5 m default / 5.0 m parity, per-rack output with connected-woodland neighbours, and no overlay this pass.

**P3 — One implementation slice:** sole C# writer in its own D-091 worktree on base `1e4e0221`. Add a pure `CanopyLightEvaluator` in `src/Core` over immutable views; geometric union on 0.25 m; light on 2.5 m (5.0 m mode); metadata; recompute triggers; per-rack summaries. Do not restructure existing contracts, do not edit UI presentation, do not add routes or overlay.

**P4 — Proportional qualification:** 0/1/2-tree oracles, grid decoupling, dead/stump, exact 5.0 m parity, cross-border neighbour, outer-perimeter absence; matched control vs real 10-tree Fell at years 0/1/3; Approve-alone zero change; measured runtime. No full Q1, P0 L4, Unity build, Windows or broad scenario matrix.

**P5 — Handoff:** exact commit and metrics, model version and caveats, `IMPLEMENTED` / `NUMERICALLY QUALIFIED` / `REVIEWED` separated; one real read-out demonstration; route any future overlay as a separate approved slice.
