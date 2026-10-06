# Adult background / suppression mortality — design (Phases 9–13, 18)

## The no-mortality problem (production as it stands)

| Stand age | Unthinned stems/ha | Basal area m²/ha | SDI | Volume m³/ha | Mean CI |
|---|---|---|---|---|---|
| 20 | 2,100 | 41.1 | 1,004 | 249 | 8.24 |
| 40 | 2,206 | 93.1 | 1,953 | 802 | 8.60 |
| 60 | 2,269 | 160.1 | 3,036 | 1,696 | 9.07 |
| 120 | 2,300 | 416.2 | 6,552 | 5,937 | 8.97 |

No tree ever dies (regeneration even adds stems). Thinned regimes reach 265–373 m²/ha by age 120.

The level at which this becomes implausible depends on maximum-density evidence that is not in the supplied research. As a working band [I], closed Sitka stands carry well under 80–90 m²/ha, which places the failure at about age 35–40 unthinned.

**Root cause.** The Hegyi index sums `(Dj/Di)/distance`. Uniform growth leaves every ratio and distance unchanged, so mean CI stays at about 8–9 for 100 years: competition never intensifies as the stand fills. DBH growth falls only through the `(1 − D/120)` size term (0.40 → 0.30 cm/yr). No suppression-based rule can produce self-thinning from this index, because suppression does not rise with crowding.

## Alternatives tested (prototypes in the diagnostics harness; production untouched)

All prototypes call production `ForestTree.ApplyMortality`. Suppression is `S = 1 − 1/(1 + CI/Ci50)`. Rolls use the model-1 per-tree domain (`SimulationRandom.Roll(1, "MORT-"+id, year, seed)`). Parameters are [I] placeholders.

| ID | Rule | Parameters [I] |
|---|---|---|
| A | Individual suppression hazard `p = 0.10 · ((S − 0.5)/0.5)²` | Onset S = 0.5, maximum 0.10 (sensitivity 0.05 / 0.20) |
| B | Chronic low realised growth `p = 0.10 · ((0.15 − g)/0.15)²` | Growth floor 0.15 cm/yr |
| C | Self-thinning: Reineke `SDI = N·(Dq/25)^1.605`; above SDImax, kill the most suppressed first | SDImax 1,800 (sensitivity 1,500); slope 1.605 [B, Reineke] |
| D | A plus a C backstop | SDImax 2,200 |
| E | None (storms only, later) | — |

### Results at age 120 (unthinned / moderate thinning)

| | Stems/ha | Basal area | Deaths (stand) | Deadwood m³/ha | Deaths, unthinned → heavy |
|---|---|---|---|---|---|
| E | 2,300 / 1,300 | 416 / 275 | 0 | 0 | — |
| A | 1,606 / 988 | 365 / 258 | 134 / 99 | 388 / 25 | 134 → 70 |
| A, hazard 0.20 | 1,475 | 356 | 169 | 412 | — |
| B | 2,275 | 416 | 5 | 0.3 | 5 → 19 |
| C (1,800) | 369 / 356 | 129 / 131 | 484 / 333 | 2,002 / 861 | 484 → 303 |
| C (1,500) | 306 | 109 | 561 | 1,838 | — |
| D | 475 / 463 | 156 / 158 | 395 / 273 | 2,076 / 926 | 395 → 202 |

Notes:
- Deaths are counts in the 0.16 ha stand.
- **Determinism:** repeat runs of E and A were identical (state hashes match).
- **Duplicate deaths:** guarded; none occurred.

## Scoring (1 poor – 5 good)

| Criterion | A suppression | B low growth | C self-thinning | D A + cap | E none |
|---|---|---|---|---|---|
| Ecological support | 4 (causal chain consistent with literature direction; coefficients [I]) | 2 (inert under this model) | 3 (Reineke slope [B]; max-density level unsourced) | 4 | 1 |
| Player legibility | 5 (crowded tree dies; thinning visibly saves trees) | 3 | 2 (stand-level culling) | 4 | 2 |
| Determinism | 5 | 5 | 5 | 5 | 5 |
| Parameter support | 2 | 1 | 2 | 2 | — |
| Save need | None (stateless hazard) | None | None | None | None |
| Testability | 5 | 4 | 5 | 5 | 5 |
| Thinning response | 5 (134 → 70 deaths) | 1 | 3 | 4 | 1 |
| Future storm compatibility | 5 (separate cause; shares the death/deadwood path) | 4 | 3 | 5 | 5 |
| Performance | 5 (O(n), reuses CI) | 5 | 5 | 5 | 5 |
| **Controls long-run density?** | **No** (BA 365 at 120) | No | Yes | Yes | No |

## Recommendation

1. **Architecture: D — individual suppression mortality as the causal mechanism, plus a bounded stand-level density safety constraint.**
   - The causal chain is competitive suppression → reduced growth → mortality risk → biological death → the existing deadwood path.
   - It must use the existing `ApplyMortality` (cause `suppression`, current year) and create one `ScenarioDeadwoodRecord` per death through the existing record type.
   - Storm mortality stays a separate cause.
2. **Not production-ready, because the parameters are unsupported.** The hazard coefficients are [I], and the safety-constraint level cannot come from the supplied evidence. At SDI 2,200 it still allows 156 m²/ha at age 120. An Irish/UK maximum-density or yield-table source is required.
3. **A prior architecture decision is recommended.** Fixing competition scale-invariance first would let the individual mechanism (A) regulate density by itself, and would also fix late absolute DBH growth. One option is a competition term that grows with neighbour crown/size relative to spacing. In that case the stand-level constraint becomes a backstop only.
   - Do **not** adopt a universal basal-area cap as the main mechanism.
4. **B is rejected:** inert under the current growth law.
5. **E (no background mortality) is rejected:** it fails the long-run density test outright.

## Deadwood (Phase 13)

- **Current gap:** biological death currently creates no deadwood record (only felled-and-retained trees do).
- **Proposed fix:** one subscriber on the existing `ForestTree.MortalityApplied` event (or the mortality step itself) creates a `ScenarioDeadwoodRecord` from the tree's volume, position and death year, through the existing visual, decay, annual-review and save paths. "Created once" is guaranteed by `ApplyMortality` returning false for already-dead trees.
- **Standing snags** would need new state and are not proposed.

## Performance (Phase 18; `Evidence/performance.csv`)

| Trees | Annual step (existing) | Suppression hazard pass | SDI pass |
|---|---|---|---|
| 336 | 147 ms | 0.54 ms | 0.18 ms |
| 1,300 | 585 ms | 2.76 ms | 0.69 ms |
| 3,000 | 1,640 ms | 7.05 ms | 2.17 ms |
| 5,000 | 2,991 ms | 14.80 ms | 4.37 ms |

- **Evaluation cost:** both passes are linear and reuse the existing competition index (no new O(n²) work), at under 0.5 % of the step.
- **Measured separately:** handling each death (canopy rebuild queue, scene query in the prototype) cost about 13–25 ms per dying tree in the prototype runs (A: 3,338 ms for 134 deaths; C: 6,510 ms for 484). A production version should batch deaths inside one change batch.
