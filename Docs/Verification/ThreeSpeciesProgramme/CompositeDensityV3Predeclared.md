# Mixed density v3 (component-faithful mortality): predeclared acceptance

Written and committed BEFORE any calibration search, long-run trajectory or v3 continuity output was run. Base: `a7a759ded087a824c1e8149e480ddf58ac0585eb` (v2). Qualification evidence it answers: `verify/final-density-v2-a7a759d` @ `81126ee093b0db345112af17bcb2842a95fe1009`. Failures stay visible; a number is not changed after output without a new, explicitly labelled predeclared trial.

## 1. What v3 is (definitions the tests encode)

* **Sitka component.** The original growth-model-1 Sitka process (relative density, pressure, suppression-weighted hazard, boundary) run on the Reference-class stems alone, with the same per-tree roll domain and suppression arithmetic as the original controller block. Its victims are Sitka stems. Broadleaves never enter it (no Oak/Beech stem count or DBH in any Sitka relative density).
* **Generic component.** Total class-weighted occupancy goes through the species-independent response `L(o) = Lmax * (1 - exp(-k * (o - 1)^m))` for `o > 1`, else 0. The expected loss fraction is `L(o) * s`, `s` = non-Reference share of the occupied space. That fraction of the stems left standing after the Sitka component is removed, chosen among those stems only, by species-neutral competitive position (common Hegyi half-saturation 5, exponent 4). No species, class or Ci50 enters the choice.
* **Final victims.** The union; disjoint by construction; no stem dies twice.
* **Equivalent formulation.** Count-wise this is the v2 quota `R + g (N - R)` (`R` Sitka-component deaths, `g = L s`, `N` living stems). v3 changes WHO pays `R`, not how many die in the first instant.

## 2. Exact identities (no tolerance; any failure is a failure)

* I1 Sitka-component victim IDs equal the victims of a literal port of the original controller block applied to the Sitka-only stem list (at least 300 random states, varied sizes, competition and stocking, including boundary-stage cases).
* I2 The Sitka-component victim set is unchanged by adding, removing or altering any non-Reference stem (identical IDs, same roll).
* I3 Sitka and generic victim sets are disjoint; the union has no duplicate; total = Sitka + generic.
* I4 Every Sitka-component victim is a Reference stem; no Oak/Beech stem is ever a Sitka-component victim.
* I5 Permuting the input order, or swapping the class labels of non-Reference stems while keeping occupancy and the Sitka victims equal, leaves every victim unchanged.
* I6 Zero non-Reference share gives zero generic deaths.
* I7 Same inputs give identical output (replay).
* Generic response: zero for occupancy at or below 1, continuous at 1, strictly increasing above it, below `Lmax` for all finite occupancy up to 1000, function of occupancy only.

## 3. Continuity (statistical; expectations over 400 yearly roll seeds)

Replacement design (as v2): 1000 stems, Sitka share 100 / 99.9 / 99 / 95 / 90 %, DBH 15 / 25 / 40 / 60 cm, Hegyi index 1 / 3 / 8.
* C1 total expected loss: `|T(99.9%) - T(100%)| <= 0.005`; adjacent steps `<= 2 x composition change + 0.005`.
* C2 Sitka-component loss per Sitka stem: `|S(99.9%) - S(100%)| <= 0.005`.

Addition design: one fixed Sitka stand (old and mid-size cases) plus 0 / 1 / 3 / 12 / 26 small suppressed broadleaves (scaled by area).
* C3 Sitka-component deaths are IDENTICAL for every number of broadleaves (I2 as a measurement), so Sitka loss per Sitka stem does not change at all.
* C4 total expected deaths minus pure-stand deaths is `<= 0.5 + 0.005 N` for every case and never negative.

Last broadleaf (deliberate removal and natural death give the same surviving state):
* C5 Sitka victims identical with and without the last broadleaf; total deaths differ by at most 1 stem in every seed and by `<= 0.005` of the stand in expectation.

No step: across all states above, an adjacent total-loss step larger than 0.02 while the composition step is at most 0.001 is a failure.

## 4. Pure Sitka (absolute gate)

Unchanged, in this order of authority: SHA-256 fingerprints of `SitkaGrowthModel` class, the legacy controller decision block and `StandRelativeDensity`; pure-Sitka model 2 equals model 1 for 100 years on both geometries (victim IDs, row hashes, tree-state hashes); every Phase-0 anchor. Any movement is a STOP.

## 5. Long-run acceptance for the generic response

Scenarios (each geometry, Legacy40 and Enlarged80): pure Oak dense, pure Beech dense, pure Oak/Beech young, mixed three-species dense and young. 300 years.
Per year record: living stems, basal area, occupancy after the step, density deaths, dead basal area as a fraction of living basal area, mortality of stems at least 20 cm DBH.

For PURE Oak and PURE Beech:
* R1 flattening: `|OLS slope of occupancy over Y201-Y300| <= 0.0025` per year, `|slope over Y151-Y250| <= 0.004` per year, and occupancy(Y300) `<= 1.10 x` occupancy(Y200).
* R2 no violent oscillation: the 5-year mean stem loss fraction never changes by more than 0.10 from one 5-year block to the next, and no year loses more than 40 % of the stems.
* R3 no prolonged extreme loss: every 25-year window after Y25 has mean annual dead basal area `<= 4 %` of living basal area and mean annual stem loss `<= 30 %`.
* R4 no collapse: living basal area never falls by more than 50 % within any 25 years, and Y300 basal area `>= 50 %` of Y100.

For MIXED stands: R2-R4 on the whole stand, plus
* R5 the Sitka-component loss per Sitka stem, averaged over Y201-Y300, is within 25 % of the pure-Sitka comparator run in the same harness on the same geometry (this is the late-mixed diagnosis, section 6).

Candidate grid (all [C], none a measurement): `Lmax` in {0.25, 0.35, 0.50}; `k` in {0.20, 0.40, 0.80, 1.60}; `m` in {2}; plus `m = 1` with `k` in {0.10, 0.20, 0.40} at the same three `Lmax`. The v2 setting (`Lmax 0.25, k 0.20, m 2`) is the baseline.
Selection: the candidates passing R1-R4 on EVERY pure fixture form the passing set P. Choose the CENTRAL candidate: among those of P whose grid neighbours (one step in `k`, one step in `Lmax`, same `m`) all pass or lie outside the grid, the one with the largest minimum normalised margin over R1-R4. If no candidate has all neighbours passing, take the passing candidate with the largest minimum margin and report the passing region as thin. If P is empty the generic kernel cannot flatten the pure trajectories: STOP and report, do not add a species capacity, and do not change the victim exponent or adult growth without Manager approval.
The selected candidate and its two nearest passing neighbours must then reproduce R1-R5 in the real Unity harness (not only the reduced simulator).

Do NOT optimise to one Y300 number: R1 uses two windows; R2-R4 use all windows.

## 6. Late-mixed diagnosis (v2), categories fixed in advance

The v2 late mixed stand lost about 0.27 of its stems a year against about 0.125 for pure Sitka. Decomposition by component, then:
* A: mathematically expected and transient (returns to the pure-Sitka band within 50 years without any change);
* B: sustained generic over-pressure (generic deaths are at least half of all density deaths);
* C: accounting / composition artefact (the Sitka subset's own required deaths are not paid by the Sitka stems, so the subset stays above its line and the demand repeats every year);
* D: another bug.
v3 must show the same stand with the Sitka-component loss per Sitka stem inside the R5 band, otherwise the diagnosis is not closed.

## 7. Limits stated in advance

The reduced simulator uses a regular-grid fixture, the production Hegyi/growth equations, species parameters read from the assets, and an EXTERNAL recruitment series taken from the Unity v2 long runs; it has no canopy light, seed rain or juvenile cohorts. It screens candidates; it is not evidence about the game. Only the Unity harness counts for R1-R5.
