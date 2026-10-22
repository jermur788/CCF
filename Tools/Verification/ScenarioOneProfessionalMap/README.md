# Scenario One professional map: verification tooling (SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1)

Disposable evidence tooling. Every harness follows the repository's copy / run / remove workflow: copy it from `Tools/Verification/` into `Assets/ForestPrototype/`, run it in the Editor under the machine-wide Unity lock, then remove it **and its generated `.meta`**. Nothing here is compiled into a build, and nothing is left under `Assets/ForestPrototype/` afterwards.

| File | What it is | Needs Unity |
|---|---|---|
| `ProfessionalMapVerification.cs` | Rendered and functional qualification of the new map against the LIVE game: trees and species mapping, the forest picture's placement in both directions, vectors, overlays square by square, redraw counters (stand / walk / turn / felling / load), input isolation, layout at both sizes, rack planning, microbenchmarks. Anything it cannot check is `PMV_UNVERIFIED`, never a pass. | yes |
| `MapRedesignShots.cs` | Matched screenshots: the same deterministic management story at three stages, photographed on every layer at each size. Run it on the OLD map and the NEW one; each run records a world hash per stage, and the two must match (the map does not touch the simulation). | yes |
| `MenuShots.cs` | The startup / title screen, its controls card and the F10 session card (needs `CCF_STARTUP_SCREEN=1`). | yes |
| `run_unity_harness.py` | Stages a harness, runs the Editor (optionally in a private Xephyr display), and always removes the staged files. The caller holds the Unity lock. | yes |
| `../StandMap/OfflineLogic/StandMapOfflineCheck.cs` | The shared map logic on plain .NET (129 checks). | no |

## Run

```
# offline, any time
python3 -I Tools/Verification/StandMap/OfflineLogic/run_standmap_check.py

# in the Editor (lane clean, Unity lock held): stage, run, remove -- run_unity_harness.py does all three
python3 -I Tools/Verification/ScenarioOneProfessionalMap/run_unity_harness.py --name ProfessionalMapVerification \
  --method ProfessionalMapVerification.Begin --files ScenarioOneProfessionalMap/ProfessionalMapVerification.cs \
  --out <folder outside the repo> --private-display --screen 1920x1080 --keep-focus --sizes 1920x1080,1280x720
```

`MapRedesignShots` and `MenuShots` run the same way (`--method MapRedesignShots.Begin` / `MenuShots.Begin --env CCF_STARTUP_SCREEN=1`).

The runner copies the Unity licence into a throwaway config folder inside `--out`: **delete that `config/` folder before keeping the evidence.** The Editor also rewrites `.vscode/settings.json` and some SectionFive materials on every run (documented repository behaviour); they are not part of any candidate, so commit by explicit path and restore them.

## What this does not prove

Frame times in a nested, software-presented Editor are not a player's frame times; the cost figures are counters (what is redrawn when) and diagnostic microbenchmarks, with no threshold invented. It does not judge whether a screenshot is attractive or readable: read the pictures. It does not run a Windows build or measure camera smoothness.
