# RACK-V2-03C — AUTHORITATIVE RACK RUNTIME / ORDER RECONCILIATION — HANDOFF

**RACK V2 AUTHORITATIVE RUNTIME QUALIFIED**

- Task: RACK-V2-03C (packet; spec `SPEC-scenario-one-objective-1-rack-planning-field-marking-v2-V1`, APPROVED; D-064)
- Worker: DeepSeek V4.1 Flash / OpenCode, Lane A primary
- Base: `c6342f79a3e89c040fadd232017a937132779385`
- Worktree: `/media/jer/ZX20/CCF-RackV2-03C` (registered lane, no recreation or rename)
- Branch: `feature/scenario-one-rack-v2-runtime-v1`
- **Implementation SHA:** `2178d392dd0fd285a30383e0bcf4d0df1745a8af`
- **Final tested HEAD:** `84f801850acc26906a7cd8dd1a452d0a8da3f90a` (the only delta from `2178d39` is 4 added harness lines; the production tree is identical)
- Unity: 6000.6.0f1, registered lane U1 detached at the tested SHA, restored afterwards to `7f708e4e30e61f3ce952a4ac75bde5fa2d8d38cd` (clean)
- Unity locks: `4bbb142cf7d5422880193ada78507e15` (2178d39 runtime gate and protected chain) and `4c165c718799473baea8d07d87870929` (84f8018 final runtime gate), both acquired/released normally; lock free, no Editor PIDs
- External evidence:
  - `/media/jer/Files/CCF/Verification/RackV2-03C-Runtime-Final-20261027T1204Z/` — final runtime gate at `84f8018`
  - `/media/jer/Files/CCF/Verification/RackV2-03C-Runtime-20261027T111151Z/` — first runtime gate at `2178d39` plus the protected chain

## What this task establishes

Confirmed rack records are the single source of truth. The runtime owns the D-064 lifecycle:

`Plan Racks → Confirm Rack Plan → Pending rack-removal work → field marking → rack.approve → execute → rack.result`

- `ConfirmRackPlan` creates/refreshes **Pending** rack-owned felling orders only. It never approves or executes work, and it never fells a tree.
- A never-worked draft freezes its removal membership at confirmation from the live stand. A reopened confirmed rack keeps its persisted geometry and frozen membership; unchanged reconfirmation is exact.
- Pre-approval geometry edits reconcile only the affected rack's Pending orders (keep retained orders, remove stale, add missing) without duplicate order IDs.
- Approved-but-unexecuted edits require an explicit `allowCancelApproved` acknowledgement. They cancel the affected rack's approved orders and recreate its missing work as Pending; other racks keep their approved order IDs and states.
- Any linked `Completed` order locks that rack against edit and delete. The plan may still reopen/reconfirm around the locked rack.
- Geometry changes clear the authoritative `entranceMarked` state and report `MarksCleared`; the marking lane owns the orange visual cleanup.
- Counters are derived: `PlannedRackCount`, `MarkedRackEntranceCount`; `RackTool.ConfirmedCount` no longer increments a private field.
- A V2 world refuses the superseded per-proposal `ConfirmRack` route, and rack-owned orders cannot be removed/cancelled individually outside the rack lifecycle. Model-0 worlds retain the legacy path.
- No save-schema change. Existing v20/v21 load policy, empty-plan world hashes and Reference Future v1 are untouched.

## Production changes

- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs` — authoritative V2 runtime, snapshots, counters, confirm/reopen/edit/delete, order reconciliation and entrance-mark state.
- `Assets/ForestPrototype/Racks/RackGeometry.cs` — pure `RackPlanStatus`, `RackOrderLifecycle`, `RackPlanRack`, `RackPlanOperationStatus`/`RackPlanOperationResult`, plus `AngleDegrees`, `LengthM` and `SameLine` helpers.
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` — rack-owned order removal/cancel core split; public removal refuses a rack-owned order.
- `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.Racks.cs` — `ConfirmRack` refuses to create a competing legacy rack in a V2 world.
- `Assets/ForestPrototype/Racks/RackTool.cs` — `ConfirmedCount` derives from the manager.
- `Assets/ForestPrototype/UI/LearningObjectivesView.cs` — the narrow lesson hook records `rack.create` from `PlannedRackCount` in V2 worlds (no redesign).
- `Tools/Verification/RackV2Runtime/RackV2RuntimeVerification.cs` — new disposable Tier-2 gate.

No scene, prefab, ScriptableObject, material, `ProjectSettings`, package, `.meta`, save-schema or generated-file change. No 02B-R-owned file was touched (headland/feeder, `ForestStartingStand`, stand geometry/presentation).

## API surface for later lanes

Read-only snapshots (ordered by stable rack id; each call returns a fresh immutable snapshot):

```csharp
bool RackPlanningV2Active
int RackPlanningModelVersion
bool RackPlanConfirmed
int NextRackId
IReadOnlyList<RackSaveData> Racks            // raw persisted records
int PlannedRackCount
int MarkedRackEntranceCount
int RackObjectCount                          // V2 racks, or legacy session groups
IReadOnlyList<RackPlanRack> RackPlan         // all racks, Draft/Confirmed
IReadOnlyList<RackPlanRack> ConfirmedRacks   // empty unless the plan is confirmed
bool TryGetRack(int rackId, out RackPlanRack rack)
RackOrderLifecycle RackOrderState(int rackId)
bool RackIsLocked(int rackId)
```

Each `RackPlanRack` exposes `Id`, `Entrance`, `Far`, `AngleDegrees`, `LengthM`, `WidthM`, `Status`, `RemovalTreeIds`, `EntranceTreeId`, `EntranceMarked`, `Orders` and `IsLocked`. Each `RackOrderLifecycle` exposes `Pending`, `Approved`, `Completed`, `Failed`, `Open`, `HasOpenWork`, `HasApprovedWork`, `HasCompletedWork` and `IsLocked`.

Authoring / lifecycle:

```csharp
RackPlanOperationResult AddRack(RackPoint entrance, RackPoint far)
RackPlanOperationResult UpdateRack(int rackId, RackPoint entrance, RackPoint far, bool allowCancelApproved = false)
RackPlanOperationResult DeleteRack(int rackId, bool allowCancelApproved = false)
RackPlanOperationResult ConfirmRackPlan(bool allowCancelApproved = false)
RackPlanOperationResult ReopenRackPlan()
RackPlanOperationResult MarkRackEntrance(int rackId)
RackPlanOperationResult ClearRackEntranceMark(int rackId)
```

`RackPlanOperationStatus` is `Applied`, `NoChange`, `RequiresApprovalWarning`, `Locked` or `Invalid`; the result carries `Message`, `RackId`, order/rack change counts and `MarksCleared`. `RackPlanTargetCount` is 5 (Scenario One gameplay calibration, not a forestry rule).

Later lanes must consume these APIs — not `RackGroup`, not the authored layout, not raw order scans — for confirmed rack geometry, stable IDs, centreline endpoints, frozen removal IDs, entrance tree, entrance-mark state and linked order lifecycle/lock state. MFM should read `ConfirmedRacks`/`RackPlanRack`.

## Verification actually run

### Runtime feature gate — PASS

`RackV2RuntimeVerification` at final HEAD `84f8018` (production tree `2178d39`): exit 0, 14 logged phases, `RACKV2RUNTIME_VERIFY_PASS`, 0 `error CS`.

Phases: fresh V2 identity; five draft racks without work; confirm creates Pending rack-owned work only; unchanged reconfirm exact; pre-approval edit reconciliation without duplicates; restored edit recreates only missing orders; delete before approval; delete/re-add with new stable id; approved-unexecuted warning changes nothing; approved-unexecuted cancel/recreate limited to the affected rack; entrance mark geometry-bound; completed-linked locking (including plan reopen); confirmed plan/frozen sets/marks/order links survive JSON capture + load; model-0 legacy `ConfirmRack` still works after a pre-v21 load.

### Protected / focused gates — PASS

| Gate | Result | Required evidence |
|---|---|---|
| `RackV2SaveVerification` | PASS | `RACKV2SAVE_VERIFY_PASS checks=35` |
| `StandGeometryVerification` (`CCF_STAND_GEOMETRY=0`) | PASS | `STAND_GEOMETRY_VERIFY_PASS checks=10`; `5546B9B3B50673B2` |
| `Enlarged80Verification` | PASS | `ENLARGED80_VERIFY_PASS checks=12`; `94589C749852F359`, `92F9635566BF6AFE` |
| `ScenarioReferenceVerification` (`CCF_STAND_GEOMETRY=0`) | PASS on required tokens | `REFERENCE_FUTURE_V1_PASS`, `REFERENCE_FUTURE_V1_CONTRACT_PASS`; `7AD177B3CC2F73C7`, `BFFF66015A6619DA`, `EBD49ADEF18C3F36` (see caveat 1) |
| `ScenarioOneCompletionVerification` growth 2 | PASS | `SCENARIO_ONE_COMPLETION_VERIFY_PASS`; `9C77EB4BEE427FB9` |
| `ScenarioOneCompletionVerification` historical v18 | PASS | `SCENARIO_ONE_COMPLETION_VERIFY_PASS`; `702766DECE591E21` |

Protected fingerprints, no rebaseline: `5546B9B3B50673B2`, `94589C749852F359`, `92F9635566BF6AFE`, `7AD177B3CC2F73C7`, `BFFF66015A6619DA`, `9C77EB4BEE427FB9`, `702766DECE591E21`; informational `EBD49ADEF18C3F36` exact.

### Offline / static

- `Tools/Verification/ScenarioOneRackUx/OfflineLogic/run_rack_check.py`: `RACK_OFFLINE_PASS checks=65`.
- `Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod` and `--mode tools`: no error in any changed file. The approximation reports unrelated `Painter2D` baseline drift in four untouched UI files; the real Unity import/compile is authoritative and had 0 `error CS`.

## Caveats and unresolved gates

1. **Reference gate shutdown hang.** The corrected Reference run printed `REFERENCE_FUTURE_V1_PASS` and all required hashes, then the Editor hung after `Cleanup mono`; the driver killed it at its 1500 s timeout (`exit -1`). This is the documented post-shutdown hang seen in earlier lanes, not a gate failure. The verdict rests on the required log tokens and exact hashes. An earlier attempt without `CCF_STAND_GEOMETRY=0` failed because it ran the Enlarged80 new-game world; the corrected run is the valid one.
2. **Save/load scope.** The runtime gate serialises with `JsonUtility` and loads through `LoadData` in memory; it does not touch the player's on-disk save slot. Cold-start Continue from a saved V2 world remains a RACK-V2-11 item. Disk JSON schema/load behaviour is covered by `RackV2SaveVerification`.
3. **Old rack UX gate.** `Tools/Verification/ScenarioOneRackUx/RackUxRack.cs` encodes the superseded first-person/map per-rack authoring flow and will not pass in a V2 world: it reaches the intentional `ConfirmRack` V2 guard. This is the F-10 disposition item for RACK-V2-07/08/12 (update or retire under G3), not a 03C production regression. It was not run as a 03C pass gate.
4. **Out of scope, not started or claimed:** map editor UI and Plan Racks mode (04/05/07), minimap (07), first-person authoring removal (08), orange spray visual and targeting (09), Help/objective copy (10), MFM consumption, whole-objective 12/13, rendered review, integration (14), Windows build, dashboard publication.
5. **02B-R headland.** The headland lane is not an ancestor of this base and was not merged or edited. The runtime consumes the persisted rack entrance endpoint, so the later headland integration does not require a runtime API change; deliberate joint integration/gates remain Manager work.
6. **Push/PR.** Nothing pushed; no PR; no merge; no build; no dashboard update.
7. **U1 cleanliness.** Known Editor re-import side effects (`.vscode/settings.json` and the SectionFive material family) occurred; both were captured byte-exact (`u1-editor-side-effects.patch`, final sha256 `d88bdf9d7d8c748ce3eb42697aad8126aec0458dbca2d9574732dbbec324839b`) and restored. U1 ended clean at `7f708e4` including untracked files; no harness or generated `.meta` remained.

## Exit

**RACK V2 AUTHORITATIVE RUNTIME QUALIFIED** for the RACK-V2-03C runtime/API scope. The remaining Objective 1 work is UI/marking/integration, not authoritative rack state or order lifecycle.
