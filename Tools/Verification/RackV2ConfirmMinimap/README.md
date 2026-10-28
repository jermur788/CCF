# Rack V2 confirm, reopen and minimap verification (RACK-V2-07)

Spec: `specs/scenario-one-objective-1-rack-planning-field-marking-v2/`; packet `Docs/Handoffs/RACK-V2-07/TASK-PACKET.md`.

## What it proves

`python3 -I Tools/Verification/RackV2ConfirmMinimap/run_static.py [--out DIR]` is read-only (it only reads files and runs read-only `git` commands). It checks that the exact-five and validity gate sits in `CanConfirmRackPlan`, that `ConfirmRackPlanFromMap` only delegates to the existing `ConfirmRackPlan` (no approval, execution, order creation or flag of its own), that the authoritative rack/save sources are byte-identical to the starting main, that the counter is derived (no incremented rack counters), that the map model and the minimap only read the authoritative sources and never call a mutator, that the legacy/model-0 shapes and wording are gated behind `RackPlanV2`/`!v2`, that the first-time note is non-interactive, that approved-work cancellation is never granted automatically, and that no disposable harness is left in `Assets/`.

`run_gate.py --gate confirm --out <evidence>/unity-confirm --sizes 1280x720,1920x1080` is the Unity gate. Part A (no pointer) covers the authoritative semantics; Part B drives the rendered Stand Map and minimap with the real desktop pointer (`../RackV2Editing/pointer_driver.py` under xdotool in a private Xephyr display) and captures each state at each size. The full interaction flow runs at the first size; later sizes repeat the note, guidance, confirm and solid-minimap states. Run it only in a clean registered Unity lane at the exact candidate, under the machine-wide Unity lock (`/media/jer/ZX20/CCF-Ops/scripts/unity-lock`). It stages the harness into `Assets/ForestPrototype/`, and removes the script and its generated `.meta` afterwards. Inspect `run_gate.py` before use: it creates an isolated Unity config and a private display, and may reap the Editor by PID only after the documented shutdown marker.

Preservation gates use the same runner and are unchanged from their owning tasks: `--gate spacing` (RACK-V2-06, which includes the RACK-V2-05 editing checks), `--gate editing` (RACK-V2-05), `--gate map` (RACK-V2-04) and `--gate runtime` (RACK-V2-03). The protected offline checks are `RackV2Editing/run_offline.py`, `RackV2Spacing/run_offline.py`, `RackV2SuggestedLayout/run_offline.py` and `StandMap/OfflineLogic/run_standmap_check.py`.

## Intentionally superseded assertions

Two RACK-V2-04 assertions in `StandMap/OfflineLogic/StandMapOfflineCheck.cs` stated that V2 racks do not expand the minimap ("V2 presentation does not expand the minimap" and "a harvested V2 rack does not expand the minimap"). The approved RACK-V2-07 packet (requirement 8) requires the minimap to draw the same dashed/solid language as the Stand Map, so both were rewritten as stronger assertions about what the minimap now draws, with a comment naming the supersession, and new checks were added for the draft/confirmed/harvested split. No other protected assertion was changed.

## Evidence boundaries

The first-time note is session memory only (no save/schema change belongs to this task), so a new session shows it again. Marks are exercised through `ScenarioOneManager.MarkRackEntrance`, the existing authoritative API; player spraying is RACK-V2-09. Executed removals are fixture-set by completing an existing rack-owned order, as the RACK-V2-05 gate does.
