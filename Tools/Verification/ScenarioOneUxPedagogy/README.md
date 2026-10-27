# Scenario One UX / pedagogy verification (SCENARIO-ONE-UX-PEDAGOGY-V1)

Disposable evidence tooling for the intro redesign, the shared type scale, the progressive teaching pictures, the selected-tree treatment, the active-objective HUD widget, the Work Plan
focus and the mouse-look diagnosis. Everything here follows the repository's copy / run / remove workflow: a harness is copied from `Tools/Verification/` into `Assets/ForestPrototype/`, run in
the Editor under the machine-wide Unity lock, and removed together with its generated `.meta`. Nothing here is compiled into a build.

| File | What it is | Needs Unity |
|---|---|---|
| `OfflineLogic/TeachingDiagramPlanCheck.cs` + `run_diagram_check.py` | Structural checks of the nine teaching pictures (objective 1 is racks and standing trees only; the symbol vocabulary never shrinks; observation lessons 5 and 7 add no decision; the stand never moves; no white; label budgets) and, with `--design stand.json`, an exact item-by-item comparison with the approved design data | no (plain .NET SDK) |
| `UxPedagogyShots.cs` | Renders the intro, help windows, HUD, the Objectives card for all eight stages, the Work Plan at three stages and the selected-tree poses at 1280 x 720 and 1920 x 1080, with numeric audits (text size, containment, label overlap, legend fit, a pixel diff proving the selection adds no white) | yes |
| `CameraLookDiagnosis.cs` | Measures where mouse-look input or frame time is lost: synthetic and real (XTEST) pointer input, `WarpCursorPosition` cost, GC per frame and per component | yes |

## Run

```
# offline, any time
python3 -I Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic/run_diagram_check.py [--design <stand.json>]
python3 -I Tools/Verification/ScenarioOneUx/run_ux_check.py

# in the Editor (lane clean, Unity lock held): stage the harness, run, remove it
cp Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs Assets/ForestPrototype/
Unity -projectPath . -executeMethod UxPedagogyShots.Begin      # CCF_ACCEPTANCE_OUTPUT=<folder outside the repo>, CCF_UI_SIZES=1280x720,1920x1080
rm Assets/ForestPrototype/UxPedagogyShots.cs Assets/ForestPrototype/UxPedagogyShots.cs.meta
```

Run the Editor in a private Xephyr display (never on the user's desktop). Camera runs additionally need `CCF_LOOK_REAL=1` and an external XTEST driver for real pointer motion, and the Editor
window must fill the display so the Game view draws at 1:1 (a 0.25x Game view multiplies pointer travel and leaves the pointer outside the view). The Unity Editor rewrites
`.vscode/settings.json` and some SectionFive materials (the Forb_* and Bilberry_* `.mat` files) on every run. They are not part of any candidate: commit by explicit path so they are never
staged, and restore them (`git checkout -- <those paths>`) only when the lane's owner allows it.

## What this does not prove

The offline checks prove what is drawn and in which order, not that a picture renders, fits or is understood. The screenshots must be read, not only counted. Frame times from the software-rendered
nested display are not a player's frame times; the camera harness reports counts of lost input and the structure of the recentre, not a frame-rate claim. No learner has tested the pictures.

## Camera evidence correction (independent review R1/R2)

The experimental recenter-scheduling change was excluded from the final candidate. ForestPlayer.cs is byte-identical to accepted BASE 10d592f. Original cursor behaviour, including its known input-discard and possible late-landing snap, remains.

Real-input total absolute yaw includes erroneous warp rotation. Comparing it with driver travel does not measure preserved hand movement; the historical pathLossPct field and published before/after benefit interpretation are withdrawn for BOTH old and experimental runs. Future output calls this rotationGapIncludingWarpsPct and explicitly warns that it is unqualified. Retained CSVs can diagnose event timing, but require intended-motion/warp attribution before any benefit claim. The edge-prediction safety argument was not established; no camera improvement or native smoothness qualification is claimed.
