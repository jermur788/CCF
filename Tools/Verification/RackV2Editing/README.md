# RACK-V2-05 direct rack editing — verification

Gates for the Stand Map's direct manipulation of authoritative player racks (create, select, move, rotate, delete, one-level
undo, reset to the suggested layout, soft snapping, refusal of invalid geometry, executed-rack locking).

| Gate | Needs | Proves | Does not prove |
|---|---|---|---|
| `run_offline.py` | `dotnet` only | The Unity-free rules in `Racks/RackEditing.cs`: headland entrance resolution, every refusal (too short, off-headland, outside the stand, duplicate, overlap, non-finite), that a move keeps shape, that snapping is soft/advisory and disableable | The live plan, the facade, the map, the pointer |
| `StandMap/OfflineLogic/run_standmap_check.py` | `dotnet` only | Shared map model/draw plan/legend, including that the ghost, selection outline, entrance square and handle are dynamic-layer only and add no legend row, and that a harvested rack has no handle | Painting, layout, input |
| `RackV2SuggestedLayout/run_offline.py` | `dotnet` only | The suggestion API stays read-only and free of UI/mutation calls | Live world |
| Runtime type compile (`make_compile_project` pattern, evidence folder) | `dotnet` + Unity 6000.6 player reference DLLs | All `Assets/**/*.cs` (non-Editor) compile against Unity 6000.6 references | It is not Unity's compiler run |
| `run_gate.py --gate editing` | Unity lock, a registered Unity lane, Xephyr/openbox/xdotool | Part A: facade semantics in a fresh model-1 world (stable ids, undo for every edit kind, refusals leave the whole save byte-identical, no suggestion mutation, no counter/work-order duplication, confirmed-plan and executed-rack locks, approved-work warning, model-0 refusal). Part B: the **real desktop pointer and keyboard** drive the Stand Map at each requested size | Player build, native Windows input, user acceptance, full rendered-appearance acceptance (RACK-V2-13) |
| `run_gate.py --gate map` / `--gate runtime` | Unity lock + lane | The unchanged RACK-V2-04 map-planning gate and the RACK-V2-03C runtime gate still pass (preservation) | — |

## Real-pointer mechanics (Part B)

`pointer_driver.py` is a dumb executor: it performs `xdotool` pointer/keyboard operations from `request-NNNN.json` files in a shared
folder. All decisions live in the harness: it first **calibrates** the OS pointer against `Mouse.current.position` (three screen
points, linear fit, third point as validation within 3 px), then converts UI coordinates to screen coordinates. A `sync` operation
pauses the driver with the mouse button still down so the harness can assert the ghost/refusal state and capture the rendered frame.
No Button callback is invoked and no Input System state is queued for Part B.

## Run (lane must be clean, detached at the exact candidate; the machine-wide Unity lock must already be held)

```bash
python3 -I -B Tools/Verification/RackV2Editing/run_gate.py --gate editing --out <evidence>/unity-editing --sizes 1280x720,1920x1080
python3 -I -B Tools/Verification/RackV2Editing/run_gate.py --gate map     --out <evidence>/unity-map
python3 -I -B Tools/Verification/RackV2Editing/run_gate.py --gate runtime --out <evidence>/unity-runtime
```

Require Editor exit 0 (or the documented reaper line plus the PASS marker), no `error CS`, `RACK_V2_EDITING_UNITY_PASS`, seventeen PNGs
for the editing gate (eight per size, plus one harvested-rack capture at the last size), and no staged script or `.meta` left under
`Assets/ForestPrototype/`. The runner deletes its copy of the Unity licence from the evidence folder and prints only a sha prefix.
