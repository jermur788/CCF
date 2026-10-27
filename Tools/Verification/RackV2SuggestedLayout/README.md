# Rack V2 suggested-layout API checks

RACK-V2-04A only. These checks add coverage; no existing target, fixture or fingerprint is changed.

## API

`ScenarioOneSuggestedRackLayout.Get(int rackPlanningModel, int standGeometryModel, IReadOnlyList<RackTree> plantationTrees)` returns `IReadOnlyList<SuggestedRackSegment>`.

Pass the live manager's `RackPlanningModelVersion`, ecology's `StandGeometryModelVersion` and a detached tree-position snapshot (including stumps). The foundation owns the five qualified entrance IDs; callers do not need them. Each immutable segment supplies `Entrance`, `Far` and `WidthM`, ordered by the qualified south-to-north route identity. It is guidance, not a player plan, and has no rack ID.

The exact geometry recipe is the existing combined gate's `AddQualifiedHeadlandRacks`: each qualified tree's live Z, headland X, stand half extent, existing corridor width. This adopts the qualified fixture identity in the foundation without changing the older `ScenarioOneRackLayout.Centres` or world generation. Current tree eligibility/marks do not redefine guidance. Model-0, Legacy40 and unknown model identities return empty. Missing/duplicate qualified IDs, non-finite Z or Z outside the headland also return empty: an incomplete snapshot must not fabricate a new layout. In a complete fresh V2 snapshot the result is exactly five.

## Offline

```bash
python3 Tools/Verification/RackV2SuggestedLayout/run_offline.py \
  --out /media/jer/Files/CCF/Verification/RackV2-04A-20261027/offline
```

Uses the installed .NET SDK and Unity 6000.6 CoreModule as a type reference, with no package additions or Unity process. Compiles the actual production suggested-layout, rack geometry, headland, rack identity and stand identity sources. Only the unrelated `ForestSaveData` type needed to compile `StandGeometryModel.ForSave` is stubbed; that method is not exercised. This proves detached geometry/read-only behaviour and explicit applicability, not live-world identity or serialization behaviour. Pass tokens: `SUGGESTED_LAYOUT_OFFLINE_PASS` and `SUGGESTED_LAYOUT_STATIC_PASS`.

## Focused Unity gate

Inspect `SuggestedLayoutVerification.cs` before staging. Run only on an exact committed candidate in an owned registered Unity lane under its normal machine-wide lock, after older queued PARITY-03 #10 has handed off/released the lane. No native input, Player build or rendered capture is required.

1. Copy `SuggestedLayoutVerification.cs` to `Assets/ForestPrototype/` in the validation lane.
2. Launch Unity 6000.6.0f1 with `-projectPath <validation-lane> -executeMethod SuggestedLayoutVerification.Begin -logFile <durable-evidence>/unity.log`, `CCF_TEST_DIRECT_ENTRY=1` and a separate `XDG_CONFIG_HOME` containing a copy of the current local licence. Do not use the player's save/preferences. Use the documented graphical private-display setup if needed.
3. Require normal Editor exit 0, no compiler/import error and `SUGGESTED_LAYOUT_UNITY_PASS`.
4. Remove the disposable copy and its generated `.meta`, preserve/restore only attributable Editor side effects, verify the exact candidate is clean, then release the task's own lock normally.

The gate runs a fresh V2 fixture and compares each segment independently to the five qualified headland routes using existing `RackGeometry.SameLine` tolerance. It compares complete serialized saves, world hashes and counters/ID allocator around 100 reads in each fixture. A second V2 fixture contains five confirmed player racks, pending rack orders and one entrance mark, created explicitly outside the getter. The model-0 Enlarged80 fixture uses the production Scenario One reset; it is an identity/non-mutation check, not a claim to requalify historical starting-tree generation. A genuine Reference Future Year-0 preview exercises Legacy40/model-0 rejection. The production loader checks original embedded world hashes; the existing pinned milestones/schedule and unchanged archive bytes are asserted without reserializing archived worlds to establish integrity.

All fixture changes are in memory; no `Save()`, scene save, archive write, ecological advance or reference continuation is performed. `SUGGESTED_LAYOUT_STATE` logs before/after SHA-256 pairs for the full captured live save, plus world hash/counters/order count. These are non-mutation evidence, not new protected anchors. A coroutine exception emits `SUGGESTED_LAYOUT_UNITY_FAIL` and exits 1.
