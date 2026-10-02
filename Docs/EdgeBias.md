# Edge bias of the isolated 40 m stand

Diagnostic only; no simulation change. Run with `Tools/Verification/EdgeBiasDiagnostic.cs` (copy into `Assets/ForestPrototype`, `-executeMethod EdgeBiasDiagnostic.Begin`, read `EDGE_BIAS_*` log lines, remove the copy and its `.meta`).

## Method
A 100 m stand of 2,510 stems (2,510 stems/ha, denser than the 2,100/ha anchor stand) is generated with one recipe: jittered 1.9 m lattice, 5% omitted, DBH classes by hash, age 20. The central 40 m window (417 trees) is measured twice from identical state: once with the surrounding trees present, once after every tree outside the window is removed, which is the isolated-patch situation. Same trees, same cells, same year; only the surroundings differ.

## Result: Hegyi competition index (mean per distance-to-edge band)
| Band (m from edge) | Trees | With surroundings | Isolated | Isolated as % of full |
|---|---|---|---|---|
| 0-5 | 161 | 33.65 | 18.65 | 55% |
| 5-10 | 144 | 33.25 | 24.60 | 74% |
| 10-15 | 77 | 32.54 | 28.29 | 87% |
| 15-20 | 35 | 40.02 | 38.22 | 96% |

73% of window trees are within 10 m of the edge. Edge trees see about half their real competition, and the effect fades by about 15-20 m, consistent with the 20 m competition cutoff.

## What that means for growth
Growth is scaled by `1 / (1 + CI / Ci50)` with `Ci50 = 3`. Applying that to the mean CIs above (an approximation: it uses band means, not per-tree values), the isolated patch lets trees grow roughly 69% faster in the 0-5 m band, 31% in 5-10 m, 14% in 10-15 m and 4% in 15-20 m than they would inside continuous forest. A mortality rule driven by competition would be biased the same way, in the opposite direction.

## Not measured
- Light and seed rain: this closed, unthinned, age-20 stand has zero cell light and zero seed rain everywhere, so those columns carry no information. They need a thinned or older stand to measure.
- Per-tree growth through an annual step, and any multi-year drift.
- Stand density and structure other than this synthetic recipe.
