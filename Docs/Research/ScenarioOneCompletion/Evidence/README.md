# Evidence

| File | Kind | Provenance |
|---|---|---|
| `input-y0-trees.csv` | Input | Year-0 rows of `Docs/Research/ScenarioOnePedagogy/Evidence/residual-stand-trees.csv` on `task/scenario-one-pedagogy-overnight` @ `60674f161e9ea9c8123993d0932aea4c7edf38d6` (Unity harness `ResidualStandEvaluation`, RNG 1, regeneration 1, context `3e4ee40`) |
| `input-y0-cells.csv` | Input | Year-0 rows of `residual-stand-cells.csv`, same commit |
| `input-residual-stand.json` | Input | `residual-stand.json`, same commit (treatments T0–T5, Unity immediate results) |
| `crop-neighbour-ranking.csv` | Output | `year0_release_and_pattern.py` |
| `release-metric.csv` | Output | same |
| `spatial-pattern.csv` | Output | same |
| `year0-prototype-output.txt` | Output | same (console summary) |
| `storm-timeline-output.txt` | Output | `storm_timeline.py` |

**Why Year-0 inputs remain valid at `a8596df`:** Growth Model 1 (D-048) changes annual Sitka height growth and adds density mortality. It does not change the authored starting stand, the Hegyi formula or cutoff, the light model or prices. Validation: the offline CI reproduction matches Unity's exported Crop Tree CI (mean 6.0925 vs 6.0924; after-T2 4.892 vs 4.8916). 9 of 336 non-crop trees differ by one boundary neighbour (0.07–0.21), because positions are rounded to 2 dp at the 8 m cutoff. **Later-year values in the source branch predate Growth Model 1 and are not used.**

`storm_timeline.py` reproduces `SimulationRandom.Roll` (RNG model 1) from `a8596df` and applies the *proposed* (unimplemented) storm roll from `task/windthrow-readiness` @ `9a9f4fb`. Occurrence years depend only on the roll id, seed and year. The severity order is an assumption about the future implementation.
