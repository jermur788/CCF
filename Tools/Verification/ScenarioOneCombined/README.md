# Scenario One combined-integration verification (SCENARIO-ONE-PLAYER-TEST-REMEDIATION-V1)

Disposable evidence tooling for the combination of the two approved lanes (Lane A UX / pedagogy `fc6fbb4`, Lane B shared Stand Map / minimap `59ab507`).
Neither lane's own harness measures the walking HUD where they meet, so this one does. Copy / run / remove, like every harness in `Tools/Verification/`:
copy `CombinedHudComposition.cs` into `Assets/ForestPrototype/`, run it in the Editor under the machine-wide Unity lock, delete it and its `.meta`.

```
Unity -projectPath . -executeMethod CombinedHudComposition.Begin     # CCF_ACCEPTANCE_OUTPUT=<folder outside the repo>, CCF_UI_SIZES=1280x720,1920x1080
```

It plays the new game in memory (no save is written) at 1280 x 720 and 1920 x 1080 and checks, against the real UI elements:

| Section | Checks |
|---|---|
| `hud-stages` | objective widget, status panel and minimap visible, on screen and mutually clear at all eight objective stages; the right column clears the minimap; the minimap takes no input |
| `estimate` | the felling-estimate panel (every ordinary E-inspection) stays above the minimap, unclipped, under the HUD column |
| `competitors` | the competitor list is unclipped; the minimap steps aside while it is open and returns afterwards (the R3 fix); tags never sit over the minimap |
| `focus` | cursor lock / visibility / player input / HUD visibility across inspect, More detail, Objectives, Stand Map, rack planning, Work Plan, help, a rapid burst, and a final turn |
| `perf` | DIAGNOSTIC frame-time and GC sample: map closed and walking, map closed and still, full Stand Map open. Editor / software rendering: relative comparison only, no threshold |

Results: `CHC_*` lines in the Editor log, `*.png` screenshots, `combined-hud-measure.csv`, `combined-hud-perf.csv`.

## What this does not prove

It drives the UI through its methods, not real keyboard and mouse (the guided real-input gate does that). It does not judge whether a screenshot is readable: read them.
It does not measure camera smoothness, and no camera improvement is claimed anywhere in this integration.

## RACK-V2-08 explicit model split

Set `CCF_RACK_VERIFY_MODEL=legacy` or `v2`, with `CCF_STAND_GEOMETRY=1` and `CCF_GROWTH_MODEL=2`; qualify BOTH branches. Legacy loads an explicit pre-v21/model-0 Enlarged80 fixture and retains the first-forest-rack prerequisite and backend assertions. V2 proves Plan Racks is available before any first-person rack, then uses real Stand Map reset/confirmation controls and checks the confirmed solid map/minimap contract. All unrelated sections remain active; leave `CCF_COMBINED_SECTIONS` unset for qualification. The focused runner `Tools/Verification/RackV2NoCompetingAuthor/run_gate.py --gate combined-legacy` / `--gate combined-v2` stages/runs/removes the gate on an exact clean locked Unity lane with isolated Files evidence. No protected legacy assertion is retired.
