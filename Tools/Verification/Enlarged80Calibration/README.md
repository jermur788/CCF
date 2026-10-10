# Enlarged80 area-calibration verification

These are disposable Editor gates. They do not save scenes or write the player's save file. Use Unity 6000.6.0f1, this worktree's independent Library, the installed licence and the existing project dependencies. One Unity Editor runs at a time.

From the task worktree:

```sh
python3 Tools/Verification/Enlarged80Calibration/run_calibration.py --gate AreaCalibrationVerification --label calibration-1
python3 Tools/Verification/Enlarged80Calibration/run_calibration.py --gate AreaCalibrationVerification --label calibration-2
python3 Tools/Verification/Enlarged80Calibration/run_calibration.py --gate Enlarged80ViabilityVerification --label viability-1
python3 Tools/Verification/Enlarged80Calibration/run_calibration.py --gate Enlarged80ViabilityVerification --label viability-2
python3 Tools/Verification/Enlarged80Calibration/run_calibration.py --gate AreaCalibrationReview --label rendered
```

The runner stages only the selected gate under Assets, removes it and its meta in finally, uses an isolated licence/config directory, and records distinct logs and JSON in Build/Enlarged80Calibration. Rendered review needs DISPLAY and saves captures there. Use a fresh label to preserve earlier results. Audit Unity-generated tracked changes after the Editor exits; preserve unrelated work.

AreaCalibrationVerification checks both target sets, unchanged means/time/cash/job minimum/owner capacity, objective targetValue, fallback century values/copy, manager target source, unknown geometry rejection, and unchanged save/definition identities.

Enlarged80ViabilityVerification uses the established completion gate's largest stem per 10 m block and nearest non-crop competitor thinning (27% starting basal area, then 20% at Year 16). It retains 16 first-intervention stems as deadwood (same allocation per property area as the Legacy40 script's four) and plants eight Oak and eight Beech with alternating owner/shelter and contractor/exposed work. A one-tree quote is included in the eventual first annual harvest job. All work uses ordinary manager APIs and real cash/time limits. It records completion at the existing Year-25 scripted horizon and final values at Year 30. It is one feasibility witness, not an optimal or prescribed forestry solution. It is run twice in separate fresh Editor processes.

AreaCalibrationReview renders Objectives, Annual Review objectives and aspirational Century Review at 1280x720 and 1920x1080, then the real Legacy40 frozen-reference comparison. The century presentation fixture uses reflection only on the disposable in-memory review record. It does not simulate a century to test copy. The original captured world is restored before exit. Inspect captures as well as the layout/text assertions.

Existing historical fixtures/tolerances, ScenarioOne.asset, operational economy, save v20, definition scenario-one-v13 and Reference Future v1 remain unchanged. Legacy regression is explicitly run with CCF_STAND_GEOMETRY=0; Enlarged80 gates run with the production default.

Restored pre-calibration Enlarged80 aspirational reviews refresh target values and achieved flags using the same target layer. Recorded measurements/history, Legacy40 reviews and real frozen-reference reviews remain unchanged; no save fields/version are added. The calibration gate reproduces this restore boundary. Rendered shutdown leaves Play Mode and uses SessionState across the domain reload before exiting from an Editor update, avoiding native window teardown inside a Game View repaint.
