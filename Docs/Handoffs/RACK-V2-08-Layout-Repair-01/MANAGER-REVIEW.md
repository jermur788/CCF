# RACK-V2-08-LAYOUT-REPAIR-01 — Manager review

**Task:** RACK-V2-08-LAYOUT-REPAIR-01 — fix the Stand Map and minimap Combined HUD layout regressions  
**Claim:** #59  
**Worker branch:** `fix/rack-v2-combined-hud-layout-v1`  
**Exact Unity-tested candidate:** `12acabd8bef9d9c86ccb82aec6e7fab4e400238a`  
**Worker handoff:** `Docs/Handoffs/RACK-V2-08-Layout-Repair-01/HANDOFF.md`  
**Manager verdict:** **APPROVED — PACKET ITEM 7 SATISFIED BY BOUNDED EVIDENCE + UNAFFECTED-PATH EXPLANATION**

## Decision

The layout repair is approved for integration.

The candidate changes only:

- `Assets/ForestPrototype/UI/StandMapView.cs`: moves the existing `map-tools` container onto the same horizontal line as `map-tabs`, preserving the tool container, button identity and handlers;
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`: changes only right-side panel placement geometry (`top` gap and remaining-height subtraction) so the minimap's reserved height is not double-counted.

No pointer/camera input, rack-confirmation input, work-order, save, simulation, or rack-planner execution code is changed.

## Packet item 7 disposition

Packet item 7 says that when a shared HUD/minimap composition path changes, the worker must run a bounded Guided/Sequential smoke **or explain why the existing exact path is unaffected**.

The model-0 guided attempt did not become fully green: it reached 396 `GUIDED_PASS`, including 288 layout checks, then failed seven assertions in the real-OS-input chain after the relative mouse movement failed to rotate the camera. The worker did not reproduce that failure on unmodified main, so this result is not accepted as proof that the guided gate itself is baseline-flaky.

However, that missing baseline reproduction is not required to accept this bounded layout task because the production diff directly establishes that the failed chain is outside the changed path:

- the Stand Map change is active inside the Stand Map modal and does not alter forest pointer/camera handling;
- the `ScenarioOneUiRoot` change is confined to `PlaceSidePanel` geometry and does not alter `HandleKeys`, pointer movement, camera rotation, rack confirmation, work creation, or `Show trees` state;
- the same candidate independently passes the protected Combined HUD model-0 layout gate at both resolutions and the Rack V2 Combined HUD layout gate, plus the targeted rack confirmation/map/editing preservation gates.

Therefore item 7 is satisfied by the packet's explicit alternative: **bounded smoke evidence plus an unaffected-path explanation**. No test waiver or weakened assertion is granted. The seven failed real-input assertions remain unclaimed and must still be re-exercised as part of RACK-V2-08 #49's own required Guided/Sequential qualification after this repair is integrated.

## Accepted verification

Exact candidate `12acabd8bef9d9c86ccb82aec6e7fab4e400238a`, Unity 6000.6.0f1:

- Combined HUD model-0: 225 PASS / 0 FAIL at 1280×720;
- Combined HUD model-0: 225 PASS / 0 FAIL at 1920×1080;
- Combined HUD Rack V2: 455 PASS / 0 FAIL across both required sizes;
- all `MAPWIN_*_TEXT_ENDS_INSIDE_THE_WINDOW` checks PASS;
- estimate and competitor clipping checks PASS at both sizes;
- RACK-V2-07 confirm/reopen/minimap: 213 checks PASS;
- RACK-V2-04 map planning: 142 checks PASS;
- editing: 326 checks PASS;
- bounded offline checks PASS.

The zero-slack competitor-list fit at 1920×1080 is accepted for this repair because the protected assertion passes, but it is recorded as a sensitivity point for future minimap-note or competitor-row growth.

## Harness integrity

The repair branch does not modify the protected `CombinedHudComposition.cs`. The executed Rack V2-capable harness variant is documented in the worker handoff; the clipping/window assertions used for this repair remained unchanged. No rebaseline is accepted.

## Integration rule

Integrate the exact tested production blobs only, together with the worker handoff and this Manager review. Do **not** carry the repair branch's stale `active-tasks.md` delta.

After integration, RACK-V2-08 #49 — remove competing first-person rack planning and make Stand Map authoritative — must resume in its existing branch/worktree, merge the repaired `main`, and run its full required qualification including model-0 and V2 Guided/Sequential gates. This review does not pre-approve those gates.

**Status:** IMPLEMENTED = YES; FEATURE-QUALIFIED = YES for this bounded repair; MANAGER REVIEWED = YES; INTEGRATED = pending this review's integration commit; WINDOWS BUILD = NOT RUN / NOT AUTHORISED.

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
