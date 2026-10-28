# RACK-V2-04 — MANAGER REVIEW

**Task:** `RACK-V2-04 — Map planning mode`  
**Original stopped claim:** #15  
**Qualified execution claim:** #30  
**Publication follow-up:** #40  
**Worker handoff:** `Docs/Handoffs/RACK-V2-04/HANDOFF.md`  
**Production checkpoint:** `bcb4f345544e50cbb32b8fecd9f82a33a284b0e2`  
**Exact Unity/rendered-tested candidate:** `68bcf6d0b1f0f6907d1dc0ba5a220583e4ade058`  
**Published branch tip:** `61a5924a9b4fa31d84e3b739e0c3405b630d9f37`  
**Manager verdict:** **APPROVED — NOT YET INTEGRATED**

## Accepted evidence

- Unity 6000.6.0f1 exited 0 with no compile/import errors.
- `RACK_V2_MAP_PLANNING_UNITY_PASS checks=142 shots=8` passed.
- Four rendered captures were produced at each of 1280×720 and 1920×1080 after the first-use-help obstruction was corrected in harness-only code.
- `Plan Racks` is a tool/mode distinct from navigation tabs and reports `Planning Racks — Active`.
- Five dashed suggested routes are sourced from `ScenarioOneSuggestedRackLayout.Get(...)`; the map legend distinguishes `Dashed = suggested` and `Solid = your planned rack`.
- Live display/navigation/toggle work preserved the complete save hash, world hash, counters and authoritative rack records/orders.
- Offline map checks, suggested-layout checks and runtime type compilation passed.
- The machine-wide Unity lock was released normally and U1 was clean.
- The exact tested candidate and production checkpoint are byte-identical under `Assets/`, `Packages/` and `ProjectSettings/`; the tested-candidate delta is verification/harness-only.
- Claim #40 verified the published handoff blob against the already-qualified local result without rerunning Unity.

## Scope interpretation

This approves **RACK-V2-04 only**. It does not approve editing, spacing teaching, confirmation/reopen behaviour, field marking, minimap expansion or whole Objective 1 playability.

## Integration status

PR #41 was opened from the qualified v2 branch but is not mergeable against current `main`. A comparison from the branch merge base to current `main` shows no intervening change to the qualified RACK-V2-04 production/map files. The branch does, however, carry one worker-side edit to Manager-owned `Docs/Project/coordination/active-tasks.md`; that edit is noncanonical and must not be imported.

A bounded integration-preparation task must rebase/rebuild the qualified branch on current `main`, preserve the qualified production/verification/handoff content, and drop the worker coordination edit. No Unity rerun is required if the resulting production bytes remain identical to `bcb4f34` and no current-main production overlap is introduced. Any production-byte drift or newly discovered overlap returns to the Manager before integration.

## Next Rack V2 dependency

`RACK-V2-05 — Editing` remains blocked by approved-spec **OPEN-3**: reopening after confirmation/approval/execution is still unresolved. No RACK-V2-05 production work is authorised until OPEN-3 is resolved in Decision Log/spec authority.

**Verdict:** `APPROVED — NOT YET INTEGRATED`

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
