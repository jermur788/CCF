# RACK-V2-04 — MANAGER REVIEW

**Verdict: NEEDS CHANGES — CROSS-LANE FOUNDATION API REQUIRED.**

Returned handoff: `Docs/Handoffs/RACK-V2-04/HANDOFF.md` on `feature/scenario-one-rack-v2-map-planning-mode-v1`, publication commit `a17c080cede33b2e11b493d7bd3415deee4c32f6`.  
Task claim #15: `[HANDED-OFF]`, closed after the packet-mandated stop.  
Production edits: none. Unity runs: none.

## Manager finding

The stop is valid. RACK-V2-04 requires the Stand Map to display the approved five-rack V2 suggested layout without creating authoritative player racks and without duplicating geometry policy in UI code. Current production exposes:

- persisted/player rack snapshots through the Rack V2 runtime;
- the older authored `ScenarioOneRackLayout.Centres()` layout;
- V2 headland infrastructure geometry;
- but no read-only production API for the **qualified five V2 suggestion segments**.

The qualified combined/headland verification currently constructs those routes from the five qualified headland entrances, but that logic lives in verification helpers and calls `AddRack`, so it is not a valid UI data source.

The UI worker was therefore correct to stop instead of hard-coding fixture IDs/coordinates, reusing the legacy authored lines, or mutating authoritative rack state merely to draw suggestions.

## Required prerequisite

Create `RACK-V2-04A` as a bounded Rack foundation/API task. The API must expose the qualified suggested rack geometry as detached read-only data for applicable V2 worlds.

Required contract:

1. Return exactly the five qualified suggestion segments for the current Scenario One V2 layout, each with entrance, far endpoint and corridor width (or an equivalent immutable/read-only geometry object sufficient for map drawing).
2. Geometry selection belongs to the Rack/headland foundation, not `StandMapView` / `UI/Map/*`.
3. Return no V2 suggestions for Legacy40, Reference Future v1, or pre-V2/model-0 continuations.
4. Reading suggestions must not allocate rack IDs, add/modify player rack records, confirm a plan, mark entrances, create/reconcile work orders, change counters, mutate save data, or alter world state.
5. Repeated reads are deterministic and side-effect free.
6. Focused verification must prove the returned fresh-V2 geometry matches the already-qualified five headland routes and that model-0/Legacy40/reference behaviour remains unchanged.
7. Do not change the accepted five-rack target, ~18 m teaching intent, headland geometry, Rack V2 persistence semantics, OPEN-3, or any protected fingerprint.

A new small production foundation type/file is acceptable if that is cleaner than overloading the legacy `ScenarioOneRackLayout` class. Do not make the UI own fixture IDs or geometry constants.

## Scope interpretation

This is an API gap exposed by RACK-V2-04, not a new product decision. D-064/spec already requires a five-rack suggested layer sourced from authoritative geometry and explicitly prohibits duplicated rack state. The prerequisite can therefore proceed as a bounded repair/API packet.

RACK-V2-04 remains **BLOCKED** until RACK-V2-04A is integrated. After integration, create a new execution claim for the map-planning task; do not reuse closed claim #15.

## Verification sequencing

RACK-V2-04A may implement/static-test in a dedicated worktree while another task uses Unity. Any Unity verification must obey the machine-wide lock and current queue. The subsequent RACK-V2-04 rendered gate remains required and must consume the new API rather than duplicating it.

No Windows build.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
