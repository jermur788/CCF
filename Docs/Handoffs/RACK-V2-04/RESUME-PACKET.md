# RESUME PACKET — RACK-V2-04 Map Planning Mode

**Feature:** Scenario One Objective 1 Rack Planning & Field Marking V2 (D-064)  
**Spec:** `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` — APPROVED  
**Task:** `RACK-V2-04`  
**Parent packet:** `Docs/Handoffs/RACK-V2-04/TASK-PACKET.md`  
**Prerequisite:** RACK-V2-04A APPROVED / INTEGRATED through PR #29 at `821789afcf198ab143b91d9fb98ed74c83adfe53`  
**Status:** READY FOR FRESH EXECUTION

## Why this packet exists

The first RACK-V2-04 execution correctly stopped with `CROSS-LANE API REQUIRED` and closed claim #15 without production edits. The missing read-only suggested-layout API is now integrated. Resume under a **new** claim; do not reopen or reuse #15.

## Starting point and lane

Start from current `main` descending from `821789afcf198ab143b91d9fb98ed74c83adfe53` and the Manager approval at `Docs/Handoffs/RACK-V2-04A/MANAGER-REVIEW.md`.

Recommended fresh branch:

`feature/scenario-one-rack-v2-map-planning-mode-v2`

Use a dedicated clean worktree. Preserve all unrelated dirty work.

## Binding API

The suggested layer must consume:

`ScenarioOneSuggestedRackLayout.Get(rackPlanningModel, standGeometryModel, plantationTrees)`

from `Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs`.

Do **not** duplicate the five entrance IDs, reconstruct the route-selection policy, or add another suggested-layout state/counter/collection in UI. Suggestions are detached guidance only and never count as player planned/confirmed racks.

## Scope

Implement the original RACK-V2-04 packet only:

- `Plan Racks` as a map tool/mode control, visually separate from navigation tabs;
- unmistakable active state (`Planning Racks — Active` or equivalent);
- first-entry suggested rack layer in applicable V2 Enlarged80 worlds;
- suggested racks faint/dashed and visually distinct from authoritative player racks;
- legend includes the approved distinction `Dashed = suggested` and `Solid = your planned rack`;
- existing map navigation remains navigation.

Primary implementation surface remains `Assets/ForestPrototype/UI/StandMapView.cs` and/or existing `Assets/ForestPrototype/UI/Map/*` components. Consume Rack V2 state read-only.

## Exclusions

No RACK-V2-05 editing/select/move/rotate/delete; no spacing teaching; no Confirm/Reopen/OPEN-3; no minimap feature expansion; no first-person authoring removal; no field marking; no Help/teaching rewrite; no MFM coverage/proposals; no S1-C/S1-D/parity/economy/ecology; no save/schema; no Windows build.

## Verification

Before Unity:

- prove toolbar/mode state is structurally separate from navigation-tab selection;
- prove simply displaying suggestions leaves authoritative rack state/counters unchanged;
- prove no duplicated rack/suggestion persistence or authoritative collection was introduced;
- run focused static/offline checks appropriate to the UI change.

Unity/rendered qualification is required at 1280×720 and 1920×1080. Capture at minimum:

1. Stand Map before planning mode;
2. planning mode active;
3. first-entry dashed suggested layer and legend;
4. a non-destructive authoritative rack fixture showing dashed suggestions versus solid player racks.

Require no compiler/import errors and normal lock cleanup. Do not claim RACK-V2-05 or whole Objective 1 playability.

## Unity sequencing

Refresh `Docs/Project/coordination/active-tasks.md` immediately before Unity. PARITY-04-UNITY-QUALIFICATION #27 is already READY/DISPATCHED and has earlier qualification priority if it is running or has acquired the lock. Static/UI implementation may proceed while another task owns Unity; never bypass the lock.

## Handoff

Publish/replace the stopped-task handoff with the completed execution evidence at:

`Docs/Handoffs/RACK-V2-04/HANDOFF.md`

The completed handoff must distinguish the earlier blocker history from the new implementation result and identify the exact tested HEAD, changed files, API consumption, non-mutation proof, rendered captures, lock record and recommendation.

## Exit

`MAP PLANNING MODE READY — READY FOR MANAGER REVIEW`

or a precise blocker.
