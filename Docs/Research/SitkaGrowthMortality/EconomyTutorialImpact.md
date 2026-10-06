# Economy and tutorial impact (Phases 16–17)

Production prices, costs and objectives are unchanged.

Harvest quotes use the production `ScenarioOneEconomyAdapter.QuoteHarvest`, which calls `TimberYieldCalculator` and the work-economy resolver. Trees grow under production; candidate heights are applied in memory before quoting and restored afterwards. Contractor execution, sell-and-extract. Data: `Evidence/timber_impact.csv`.

## Thinning revenue (30 % of stems from below)

| Scenario year (stand age) | Old | A-III (recommended) | C-III | Sold volume old → A-III | Cost (all) |
|---|---|---|---|---|---|
| 1 (21) | €230.77 | €235.07 (+1.9 %) | €244.65 | 5.56 → 5.67 m³ | €2,500 |
| 16 (36) | €857.17 | €969.28 (+13.1 %) | €1,219.54 | 15.82 → 18.03 m³ | €2,500 |
| 25 (45) | €1,339.36 | €1,540.22 (+15.0 %) | €2,019.85 | 23.31 → 25.69 m³ (sawlog 0.86 → 3.29 m³) | €2,500 |
| 40 (60) | €2,793.43 | €2,994.87 (+7.2 %) | €4,027.51 | 40.07 → 42.50 m³ | €2,500 |

For the 49 most crowded trees the pattern is the same: +2.8 % at year 1, +12.7 % at year 16, +14.5 % at year 25, +3.2 % at year 40.

- **Thinning economics:** the €2,500 small-job minimum dominates every thinning up to year 25 under every candidate, so net stays negative. Only by year 40 does a 30 % thinning become profitable (+€293 old, +€495 A-III).
- **Minimum cash** occurs at the year-1 thinning. Heights there are the authored ones (candidates differ by under 2 % in revenue, about €4), so minimum cash is effectively unchanged: 589,773 cents in the current model-1 completion gate.
- **Completion viability:** no Scenario One objective reads tree height (retained canopy, continuous canopy, regeneration cells, deadwood, managed opening, introduced species). A height-only change cannot block completion.
- **Not run:** the full completion path with the candidate model and adult mortality. That requires the production implementation, which is stopped. Adult mortality would add deadwood (a completion objective, which helps) and remove canopy trees (the retained-canopy objective needs 60 trees). Prototype A (unthinned) killed 23 of 336 trees (6.8 %) by scenario year 10 and 42 (12.5 %) by year 20. The tutorial path thins at year 1 and keeps far more than 60 canopy trees, so the margin looks ample, but this must be verified after implementation.

## Tutorial effect

| Item | Effect |
|---|---|
| First useful thinning | Unchanged: year 1 on the authored stand. Class III's first-thinning age (~20–25 by interpolation of S4's I/V guidance) agrees |
| Pruning timing | Unchanged: lifts to 2.5 / 5 / 6.5 m need trees over 4.2 / 8.3 / 10.8 m, already true of crop trees at year 0 |
| Later intervention | Year-16/25 thinnings are worth about 13–15 % more; still below the contractor minimum |
| Regeneration observation | Unchanged by height (adult light saturates at 8 m). Adult mortality would open small gaps over time (more light where suppressed trees die), which is not yet quantified for the tutorial window |
| Completion year | Expected unchanged (Year 25) for height alone; to be re-verified with mortality |

No tutorial objectives were edited.
