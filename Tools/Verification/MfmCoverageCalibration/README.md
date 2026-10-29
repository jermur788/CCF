# MFM-03 coverage calibration measurement (offline, measurement only)

Tooling for `MFM-03 — Coverage calibration measurement` (Multi-Scale Forestry Management V1, D-065). It measures the MFM-02 rack-served
coverage units on the accepted Scenario One V2 fixture and tests candidate coverage tolerances and Crop Tree candidate spacings.

**This is not production code.** It implements no prescription, coverage decision, proposal, candidate state, save field or UI. Nothing
here is copied into `Assets/`. Every tolerance and spacing it tests is a **GAMEPLAY CALIBRATION** candidate. The candidate "rejections"
are a deterministic hash-of-tree-ID workload simulation, not a quality model; no DBH, competition or dominance value is used to order or
reject candidates.

## Run

```sh
python3 -I -B Tools/Verification/MfmCoverageCalibration/run_coverage_calibration.py [--out DIR]
```

Default output: `/media/jer/Files/CCF/Verification/MFM03-CoverageCalibration-20261029/` (`export.json`, `results.json`, `tables.md`).
Success prints `MFM03_EXPORT_PASS`, `MFM03_ANALYSIS_PASS checks=N failed=0` and `MFM03_TABLES_WRITTEN`. Needs the .NET 8 SDK and
Python 3 (standard library only). No Unity, no Library, no package; the C# build happens in a temporary directory outside the repo.

## What runs

1. `runner/CoverageExport.cs` is compiled together with the **unmodified** production sources `ScenarioOneRackSampleGeometry.cs`,
   `ScenarioOneRackBay.cs`, `CompetitionShare.cs`, `RackGeometry.cs` and the existing offline `ForestEcologyController` stand-in
   (`ScenarioOneNonUi/OfflineLogic/OfflineStubs.cs`). It re-checks the stand-in against the real Hegyi source text, derives the MFM-02
   units/remainders with `ScenarioOneRackSampleGeometry.Derive`, selects rack removal sets with `RackGeometry.Select`, takes the teaching
   footprint from the derived `Teaching` frame and every competition index from `CompetitionShare.Measure`. It fails unless the
   footprint reproduces the live MFM-02 Unity measurement exactly (75 stems, 1.4417531 m², footprint SHA `c2f2fd5418794915`).
2. `analyse_coverage.py` masks infrastructure (headland/feeder from the production constants, track planks and south approach, the
   visible work clearing, player start and structures), measures every unit under three partitions, sweeps tolerances, measures the
   former-edge band, stand-edge truncation, remainders, a historical regeneration diagnostic and candidate-spacing workload.
3. `write_tables.py` renders `results.json` as the Markdown tables used in the handoff.

## Inputs (`inputs/`, see `inputs/PROVENANCE.md`)

- `fixture-stand.csv` — 1,344-stem fresh Enlarged80 Rack V2 starting stand (year 0).
- `fixture-infrastructure.json` — authored scene anchors the starting-stand generator clears around.
- `historical-lesson5-regeneration.json` — natural-regeneration cohorts from a pre-V2 Lesson-5 save; **historical diagnostic only**.

## Limits

Same-year, offline state: the stand at the start of a V2 game, before and after the rack access is removed, with no growth, mortality,
regeneration or player management. Post-Lesson-5 live state is a Unity-only residual (see the MFM-03 handoff). The remainder probes use
hypothetical rack layouts that are **not** Rack V2-qualified plans; they only illustrate non-empty remainders.
