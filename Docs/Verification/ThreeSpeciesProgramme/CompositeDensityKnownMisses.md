# Composite density — known accepted reduced-simulator misses

Status: **known calibration limitation of the reduced offline simulator — NOT a passing result.**
This branch changes no production code and no threshold.

- Threshold: R2 = no single year loses more than **0.40** of stems (and block-change <= 0.10).
  Defined in `Tools/Verification/ThreeSpecies/OfflineLogic/DensityTrajectorySim.cs`
  (`v.R2 = v.MaxBlockChange26 <= 0.10 && v.MaxYearLoss26 <= 0.40;`). **No relaxation.**
- Three retained misses (pure-Beech reduced-simulator thickets):

| Fixture | Geometry | `maxYearLoss26` | vs 0.40 |
|---|---|---|---|
| `B_BEECH_DENSE` | Legacy40 | 0.423 | +0.023 |
| `F_BEECH_YOUNG` | Legacy40 | 0.407 | +0.007 |
| `F_BEECH_YOUNG` | Enlarged80 | 0.417 | +0.017 |

- Receipt: `COMPOSITE_DENSITY_FAIL checks=107 failures=3` — reproduced on this branch by the
  unmutated control run of `Tools/Verification/ThreeSpecies/OfflineLogic/run_composite_mutations.py`.
- Failing check names: `LONG_RUN_L40_B_BEECH_DENSE_MEETS_R1_R4`,
  `LONG_RUN_L40_F_BEECH_YOUNG_MEETS_R1_R4`, `LONG_RUN_E80_F_BEECH_YOUNG_MEETS_R1_R4`
  (each `R1=True R2=False R3=True R4=True`).
- Interpretation: R2 is a reduced-simulator, single-year **all-stem** stem-count diagnostic that
  includes annual recruit inflow. The misses are marginal (1.8–5.8 % overshoot), occur in
  pure-Beech **non-game** fixtures at occupancy ~2.2, and every case passes the oscillation clause,
  basal-area retention (R3) and no-collapse (R4). The Unity LONGRUN R2 budget (0.125) passes in real
  Editor data.
- Existing retained documentation: `Docs/Verification/ThreeSpeciesProgramme/CanopyPulseRootCause.md`
  and the runner docstring in `run_composite_mutations.py`.

Do not present `E80 B_BEECH_DENSE = 0.400` as a robust pass (it is exactly at the limit).
