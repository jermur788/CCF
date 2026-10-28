# RACK-V2-04 — Map planning mode: implementation and rendered qualification

**Exit: `MAP PLANNING MODE READY — READY FOR MANAGER REVIEW`**

**Recommendation: `APPROVE` for RACK-V2-04 only.**

The Stand Map now offers `Plan Racks` as a tool/mode control, separate from its navigation tabs, with an unmistakable `Planning Racks — Active` state and the approved first-entry suggested layer from `ScenarioOneSuggestedRackLayout.Get(...)`. Suggested routes are faint/dashed, authoritative player racks are solid, and the legend says `Dashed = suggested` / `Solid = your planned rack`. Displaying guidance changes no authoritative rack, counter, mark, order or save field.

## Earlier stopped execution — history, not current state

The first RACK-V2-04 execution (claim #15) correctly stopped with `CROSS-LANE API REQUIRED` because no read-only suggested-layout foundation existed; no production code was changed. RACK-V2-04A then delivered and integrated that API (PR #29 at `821789afcf198ab143b91d9fb98ed74c83adfe53`). This resumed execution used a new claim (#30) and did not reopen #15. The blocker is resolved; the earlier handoff text is superseded by this file.

## Execution identity and candidate

- Worker: T3 Code / OpenCode harness / `opencode-go/deepseek-v4.1-flash`.
- Starting current main: `81a037e35126bbc256fcda076019af47e8e0954f`, a descendant of `821789af`.
- Branch: `feature/scenario-one-rack-v2-map-planning-mode-v2`.
- Implementation worktree: `/media/jer/ZX20/CCF-RackV2-04V2`.
- Production checkpoint: `bcb4f345544e50cbb32b8fecd9f82a33a284b0e2`.
- Exact Unity/rendered-tested candidate: `68bcf6d0b1f0f6907d1dc0ba5a220583e4ade058` — `bcb4f34` plus the focused verification gate only. `git diff bcb4f34 68bcf6d -- Assets Packages ProjectSettings` is empty.
- Claim #30 was marked `[RUNNING]` with resumed HEAD and lock details before Unity execution, then handed off and closed with this evidence.

## Production changes (`bcb4f34`)

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/StandMapView.cs` | `Plan Racks` moves into a `map-tools` row sibling to `map-tabs`; V2 active label `Planning Racks — Active`; V2 mode only drives presentation, not editing. |
| `Assets/ForestPrototype/UI/Map/StandMapSource.cs` | In a V2 world, reads suggestions from `ScenarioOneSuggestedRackLayout.Get(...)` only while planning mode is active, then reads authoritative `Manager.RackPlan` records. Legacy layout/group lines become mini-map-only full-map omissions. |
| `Assets/ForestPrototype/UI/Map/StandMapModel.cs` | Adds `MapSymbol.RackSuggested` / `RackPlayer`, per-rack `WidthM` and `MiniOnly`; includes both in the static hash. |
| `Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs` | Full map hides mini-only legacy racks; rack band width comes from the rack record instead of a constant. |
| `Assets/ForestPrototype/UI/Map/StandMapLegend.cs` | V2 planning legend entries `Dashed = suggested` and `Solid = your planned rack`; mini-only racks excluded. |
| `Assets/ForestPrototype/UI/Map/StandMapLegendView.cs` | Passes the V2 planning flag into the shared legend table. |
| `Assets/ForestPrototype/UI/Map/StandMapSymbols.cs` | One symbol table entry each for the faint dashed suggestion and the solid amber player rack. |
| `Tools/Verification/StandMap/OfflineLogic/StandMapOfflineCheck.cs` | Focused presentation checks for the V2 distinction. |

Latitude authorised by the packet was kept: no editing/select/move/rotate/delete, no spacing teaching, no Confirm/Reopen/OPEN-3, no minimap feature expansion, no first-person authoring removal, no field marking, no save/schema change, no MFM/S1-C/S1-D/parity/economy/ecology change.

## Suggested-layout API consumption

The UI calls exactly:

```csharp
ScenarioOneSuggestedRackLayout.Get(
    manager.RackPlanningModelVersion,
    ecology.StandGeometryModelVersion,
    plantationTreeSnapshotIncludingStumps)
```

No entrance IDs, route policy, offsets or five-rack geometry exist in map/UI code. The Unity gate independently called the same API and compared each displayed dashed route (`Start`, `End`, `WidthM`) to the returned `SuggestedRackSegment` at every stage: all five matched. The suggestion list is rebuilt from detached read-only geometry; it never enters `ScenarioOneManager.RackPlan`, `ConfirmedRacks`, `NextRackId` or the saved rack collection.

## Toolbar/tab separation and behaviour

Live checks at both resolutions proved:

- `map-tools` and `map-tabs` are sibling rows; the `Plan Racks` button's parent is `map-tools` and it is not one of the five navigation tabs;
- inactive label is `Plan Racks`; active label is `Planning Racks — Active` with the selected style;
- activating the mode does not change the selected navigation tab;
- choosing Light and returning to Access while active leaves the mode and the suggested layer in place — tabs remain navigation, not mode selectors;
- toggling the mode off removes only the guidance layer; re-entering restores it.

## Live non-mutation proof

The gate created ONE authoritative player-rack fixture with `AddRack(...)` in the fresh model-1 Enlarged80 world (rack 1, 30 removal trees), then snapshotted the complete live save before any map display work:

- fixture save SHA-256 `b5e91e5031d6712f545652be5579cdebcd674aa0ea8a01d1d1d8021111f2d6fb`;
- world hash `CE21F222D1F189D9`;
- counters `planned/next-id/marked/confirmed/orders = 1/2/0/False/0`.

After opening the map, entering/leaving planning mode, rendering both sizes, navigating layers and re-entering the mode, the complete serialized save, world hash, counters, rack ids/geometry/status and order lifecycles were identical (`after` hash equals `before` hash). Only the deliberate fixture rack existed; zero confirmed racks and zero work orders. No `Save()`, scene save, archive write, annual advance or reference continuation was performed.

## Verification and evidence

Evidence root: `/media/jer/Files/CCF/Verification/RackV2-04V2-20261027/`.

Offline gates re-run at the resumed candidate:

- shared-map offline check: **137 PASS**, `STANDMAP_OFFLINE_PASS` (`map-offline.log`);
- suggested-layout offline check: **147 PASS**, `SUGGESTED_LAYOUT_OFFLINE_PASS` + `SUGGESTED_LAYOUT_STATIC_PASS` (`suggested-offline.log`);
- runtime source type compilation: **168 sources, 116 refs, 0 errors** (`runtime-compile.log`).

Unity gate (Unity 6000.6.0f1, isolated `XDG_CONFIG_HOME`, private display, machine-wide lock held):

- Editor exit **0**; no compiler/import errors; `RACK_V2_MAP_PLANNING_UNITY_PASS checks=142 shots=8`;
- staged harness and generated `.meta` removed; U1 side effects restored clean at `68bcf6d`;
- captures at **1280×720** and **1920×1080**, four per size:
  - `…-map-before-planning-*.png` — Stand Map, mode off, solid fixture rack, no suggestions;
  - `…-planning-active-*.png` — active tool state, five dashed routes + solid player rack;
  - `…-suggested-layer-legend-*.png` — guidance and legend after navigation round-trip;
  - `…-suggested-vs-player-*.png` — dashed suggestions beside the solid authoritative rack.
- `unity/evidence/shots.json` records pixel dimensions, control/map rectangles, active state and rack counts; `unity/RackV2MapPlanningVerification.log` holds all 142 checks.

Visual review: at both sizes the active tool is visually separate from the tabs and unmistakable; five faint dashed routes are legible without reading as completed racks; the solid amber fixture rack is clearly heavier/solid; both required legend lines are visible; no map content or legend is clipped. At 1280 the whole map, overview, legend and side card remain inside the window.

## Lock record

- First attempt lock `19f63983c1384e76adfa2fe76aa401f1` (`f6ccc0f`, 2026-10-27T23:21:51Z) exited 0 with 138 checks passing, but a first-use map help window obscured the 1280×720 legend. The harness was fixed (`68bcf6d`) to mark help seen and require a visible legend; the lock was released normally after restoring U1 clean.
- Qualification lock `8d1940bfd51c4faf93292d80a7fbaf36` (`68bcf6d`, 2026-10-27T23:27:12Z) ran the clean rerun and was released normally with PASS. The final lock status is `free`, no Editor process, U1 clean.
- Receipts: `lock-acquire.json`, `lock-release-first.json`, `lock-acquire-second.json`, `lock-final.json`, `lock-release.json`, `lock-after-release.json`.

## Verification files added on the branch

- `Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs` — disposable in-Editor gate (staged then removed).
- `Tools/Verification/RackV2MapPlanning/README.md` — scope, invariants and exact run command.
- `Docs/Handoffs/RACK-V2-04/HANDOFF.md` — this handoff.

## Caveats and explicit non-claims

- The authoritative rack is an in-memory verification fixture, not a player-authored saved session. This gate proves display non-mutation and visual distinction, not rack authoring.
- The first 1280 capture set is preserved under `unity-first-run/` as evidence of the harness defect; only the rerun set in `unity/evidence/` is qualification evidence.
- No RACK-V2-05 editing, spacing, confirmation, field marking, OPEN-3, minimap expansion or whole Objective 1 playability is claimed or implemented.
- Legacy worlds retain their existing legacy map authoring and mini-map symbols; V2 adds the mode and guidance layer without deleting old map content.
- No Player build, native input, Windows build, save migration or user acceptance was tested.
- Dashboard synchronization remains unavailable/not done; no generated dashboard state was hand-edited.

## Manager handoff

Recommendation: **APPROVE — RACK-V2-04 only.** The task is ready for Manager review; integration, broader Rack V2 verification and any subsequent task remain separate decisions.
