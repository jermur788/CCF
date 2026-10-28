# RACK-V2-04 map planning mode Unity/rendered gate

Disposable Unity harness for the Stand Map `Plan Racks` tool/mode and its suggested dashed layer.
It is staged into `Assets/ForestPrototype/` for one run and removed with its generated `.meta`; the
evidence folder lives outside the repository.

## What it proves

- In a fresh model-1 Enlarged80 world, it adds ONE authoritative player rack as a fixture and snapshots
  the complete live save (racks, ids, geometry, marks, order lifecycle, counters, world hash).
- `Plan Racks` is a sibling tool row control, not a navigation tab; its inactive and active labels are
  `Plan Racks` and `Planning Racks — Active`, and activating it does not change tab selection.
- The dashed layer is exactly `ScenarioOneSuggestedRackLayout.Get(manager.RackPlanningModelVersion,
  ecology.StandGeometryModelVersion, plantation-tree snapshot)`; no geometry is duplicated in the UI.
- The legend carries `Dashed = suggested` and `Solid = your planned rack`.
- Navigation tabs still change layers while planning mode remains active.
- After all rendered work at 1280x720 and 1920x1080, the full serialized save, world hash, planned and
  entrance-mark counters, next rack id, confirmation state, rack records and order lifecycles are
  unchanged by displaying suggestions. Only the deliberate fixture rack exists.

Captures per resolution: `map-before-planning`, `planning-active`, `suggested-layer-legend`,
`suggested-vs-player`.

## Run

The machine-wide Unity lock must already be held by this task. Use a registered Unity lane detached at
the exact candidate; never run Unity from this non-Unity worktree.

```bash
python3 Tools/Verification/ScenarioOneProfessionalMap/run_unity_harness.py \
  --name RackV2MapPlanningVerification \
  --method RackV2MapPlanningVerification.Begin \
  --files RackV2MapPlanning/RackV2MapPlanningVerification.cs \
  --out /media/jer/Files/CCF/Verification/RackV2-04V2-20261027/unity \
  --private-display --keep-focus --screen 1920x1080 --sizes 1280x720,1920x1080 \
  --timeout 1800 \
  --env CCF_TEST_DIRECT_ENTRY=1
```

Require Editor exit 0, no `error CS`/compilation failure, `RACK_V2_MAP_PLANNING_UNITY_PASS`, four PNGs
at each size, and a fully removed staged script plus `.meta`. Inspect the captures before claiming
readiness. No Player build, native input, scene save or annual advance is used.
