# MFM-08 Geometry Repair 01 — Segment Footprints / Area Waypoints

**Parent feature claim:** #156  
**Manager verdict:** `NEEDS CHANGES` — `Docs/Handoffs/MFM-08-Map-3D-Review-UX/TIER2-MANAGER-REVIEW-20261030.md`  
**Repair base:** exact MFM-08 production candidate `d55ec86a3f0bf13820f2080197b6b0f43d832e06`  
**Mode:** bounded production bug fix + focused verification  
**Difficulty:** **3/5** — geometry must remain correct for rotated rack plans and must share one authoritative derivation between drawing and waypoints, but the defect is localized.  
**Best suited:** **Ultra Code** / strongest local C# worker. Unity-capable is useful, but full Tier-2 belongs to the successor #156 qualification rather than this repair claim.

## Purpose

Repair the MFM-08 Stand Map treatment-review geometry exposed by Tier-2. The coverage model already distinguishes Low / Centre / High areas; the current presentation collapses all three to the full rack-served unit polygon and centroid.

This is a bug fix. Do not redesign MFM coverage, prescriptions or Rack V2.

## Evidence / root cause

On candidate `d55ec86a...`:

- `ScenarioOneCoverage.Area` carries authoritative `UnitIndex`, `RackId`, and `Segment`.
- the accepted coverage split is along the common rack axis at the sample anchor +/- `Calibration.SegmentHalfLengthM` into `Low`, `Centre`, `High`;
- `ScenarioOneRackSampleGeometry.CoverageUnit.Polygon` is intentionally one full-length strip per rack and is not itself segmented;
- `ScenarioOneRackSampleGeometry.Result.Teaching` is the separate fixed teaching/sample footprint;
- `StandMapTreatmentReview.Footprints()` and `TryAreaCentre()` currently select `CoverageUnit.Polygon` by unit/rack only and ignore `Area.Segment`.

Final Tier-2 evidence: `/media/jer/Files/CCF/Verification/MFM08-Tier2-20261030T203629Z-run8-final/`.

## Lane / ownership

Suggested branch/worktree:

- branch: `fix/mfm-08-segment-footprints-v1`
- worktree: `/media/jer/Videos/CCF-MFM08-FIX`
- exact start: `d55ec86a3f0bf13820f2080197b6b0f43d832e06`

At start, refresh `Docs/Project/coordination/active-tasks.md`, #156 and this repair claim. Confirm clean exact start and no writer owns the MFM-08 review geometry surface. Post `[RUNNING — FIX]` with provider/model/branch/worktree/start SHA before production edits.

## Writable production surface

Primary:

- `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs`

Allowed only if the smallest clean implementation genuinely needs it:

- one narrowly-scoped pure helper under `Assets/ForestPrototype/UI/Map/` plus `.meta`;
- focused verification under `Tools/Verification/MfmTreatmentReview*`.

Do **not** modify coverage classification/calibration/world adapters, `ScenarioOneRackSampleGeometry` semantics, Rack V2 persistence, save/schema/validator/hash/reference, prescriptions/approval behavior, Work Plan code, Annual Review/history, teaching/MFM-09, scenes/prefabs or Windows build.

If the worker concludes a coverage-model/API change is actually required, stop and return to Manager instead of widening this claim.

## Required behavior

### 1. Per-area polygons

Every non-remainder `ScenarioOneCoverage.Area` must map to an area-specific polygon using its authoritative `Segment`.

Use the existing common rack axis, sample anchor and accepted segment boundaries. The implementation must work for rotated/translated racks; do not assume the fixture is east-west or parse geometry from `R1L`/`R1C`/`R1H` strings when the `Segment` enum is available.

Low, Centre and High of one rack-served unit must not all share the same polygon.

### 2. Source sample

The source/sample representation must use the actual fixed teaching/sample footprint exposed by the existing rack/sample geometry, not the whole coverage-unit strip.

Do not enlarge or redefine the teaching footprint to make the map easier to draw.

### 3. One derivation for map + waypoint

Drawing and `Inspect in forest` must consume the same area-footprint derivation. Avoid separate geometry logic that can drift.

`TryAreaCentre` (or replacement) must produce an area-specific waypoint. Low / Centre / High for the same unit must not collapse to one centroid unless the authoritative polygons genuinely do so.

### 4. Remainders / existing semantics

Keep explicit remainders on their existing authoritative remainder polygons.

Preserve:

- candidate/proposal glyph positions and shapes;
- sample / Covered / Not Covered / Excluded / Manual state meaning;
- Covered = comparable, not selected or committed;
- candidate map flow has no bulk confirmation;
- proposal != Fell mark;
- Prepare creates prescription only;
- approval creates ordinary Pending work;
- Work Plan remains the commitment gate;
- manual/open-work conflicts remain held rather than overwritten.

## Focused verification required in this repair

Add deterministic checks that would fail on `d55ec86a...` and pass only after the fix:

1. Low/Centre/High polygons are distinct for each applicable rack unit.
2. Segment polygons respect the authoritative along-axis boundaries for both axis-aligned and rotated rack fixtures.
3. The source polygon matches the teaching/sample footprint rather than the whole unit.
4. Per-area centres/waypoints are computed from the same repaired polygons and are spatially distinct where expected.
5. Tags for distinct segment areas no longer stack merely because all three share one geometry.
6. Existing MFM-08 offline gate remains green without weakening any check.
7. MFM-04/05/06/07/08A focused offline regressions remain green.

Do not delete, bypass or relax the two defect assertions introduced by the returned Tier-2 harness. They become successor Tier-2 acceptance checks.

## Unity

A small Tier-1 compile/smoke is permitted if useful after fresh authoritative lock preflight, but it is not a substitute for the full successor #156 Tier-2 rerun. Do not consume the machine lane unnecessarily if the repair can be proven offline first.

## Handoff

Publish:

`Docs/Handoffs/MFM-08-Geometry-Repair-01/HANDOFF.md`

Include exact start/head, provider/model, branch/worktree, changed files, geometry derivation explanation, axis-aligned + rotated verification, commands/counts, regression results, any Unity used, and clean return state.

Success token:

`MFM-08 SEGMENT FOOTPRINT REPAIR IMPLEMENTED — READY FOR MANAGER REVIEW`

After Manager approval, #156 will be repinned to the repaired production candidate and must rerun the complete Tier-2 matrix plus the deferred Rack V2 / Reference Future / MFM-06/07 Unity / relevant UI regressions and the open-work conflict.

No Windows build.

## Copy/paste dispatch

`Run MFM-08 Geometry Repair 01 from Docs/Handoffs/MFM-08-Geometry-Repair-01/TASK-PACKET.md starting at exact d55ec86a3f0bf13820f2080197b6b0f43d832e06 after fresh ownership/worktree preflight. Fix only the Stand Map treatment-review area geometry: derive distinct authoritative Low/Centre/High polygons from Area.Segment and existing rack-axis/anchor boundaries, use the actual teaching/sample footprint for the source, and make map drawing plus Inspect-in-forest share the same footprint/centre derivation. Preserve coverage/calibration, prescriptions, Pending/Work Plan authority, Rack V2/save/reference semantics and all other MFM-08 behavior. Add axis-aligned + rotated focused checks and run MFM-04/05/06/07/08A/08 offline regressions. Publish HANDOFF.md and return MFM-08 SEGMENT FOOTPRINT REPAIR IMPLEMENTED — READY FOR MANAGER REVIEW or the exact blocker. No Windows build.`
