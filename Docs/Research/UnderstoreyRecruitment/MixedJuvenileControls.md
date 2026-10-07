# Mixed natural/planted/protection and timing controls

Actual production model1/1/1, 8 treatments × 2 repeats × 100 years = 4,800 species/year rows. Bare synthetic initial world with four cells each containing .2 relative natural abundance and one .6m exact planted individual of each of Sitka/Oak/Beech (12 natural bands/12 planted individuals). No initial adult seed source. Any later seed derives from promoted trees; there is no injected continuing seed stream or seed-independent infill. No work costs/history are settled. This is a controlled open-world juvenile experiment, not a realistic starting plantation or a recommendation to replace the default Scenario One stand.

Treatments: browse0 with no clear/year1 clear/year3 clear/year1+every5year clear; browse.8 unprotected/protected with no clear or year3 clear. Protection is an intact synthetic whole-plot polygon, not an installed fence or shelter budget/lifespan simulation. Clearance targets the four starting cells through real QueryClearance/ApplyClearance. Every 10/25/50/100 biological state comparison repeated identically (64 sampled horizon states and32 repeat comparisons in code). Evidence/mixed_fixture_result.json contains all 16 run-success markers.

| Browse/protection/clearance | Species | Year100 natural promotions | Planted promotions / original4 | Planted deaths | Natural remaining |
|---|---|---:|---:|---:|---:|
| browse0_protectFalse_clear0 | sitka-spruce | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear0 | sessile-oak | 4 | 4 | 0 | 0.20889 |
| browse0_protectFalse_clear0 | beech | 272 | 4 | 0 | 1.16002 |
| browse0_protectFalse_clear1 | sitka-spruce | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear1 | sessile-oak | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear1 | beech | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear3 | sitka-spruce | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear3 | sessile-oak | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear3 | beech | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear5 | sitka-spruce | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear5 | sessile-oak | 0 | 0 | 4 | 0.00000 |
| browse0_protectFalse_clear5 | beech | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectFalse_clear0 | sitka-spruce | 221 | 4 | 0 | 32.36771 |
| browse0.800000011920929_protectFalse_clear0 | sessile-oak | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectFalse_clear0 | beech | 7 | 4 | 0 | 0.01973 |
| browse0.800000011920929_protectFalse_clear3 | sitka-spruce | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectFalse_clear3 | sessile-oak | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectFalse_clear3 | beech | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectTrue_clear0 | sitka-spruce | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectTrue_clear0 | sessile-oak | 4 | 4 | 0 | 0.20889 |
| browse0.800000011920929_protectTrue_clear0 | beech | 272 | 4 | 0 | 1.16002 |
| browse0.800000011920929_protectTrue_clear3 | sitka-spruce | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectTrue_clear3 | sessile-oak | 0 | 0 | 4 | 0.00000 |
| browse0.800000011920929_protectTrue_clear3 | beech | 0 | 0 | 4 | 0.00000 |

Before/after clearance removed the stocked juveniles and prevented subsequent recruitment in those worlds. Protected high-browse/no-clear matched low-browse/no-clear biological outputs, verifying independent browse access. The unprotected high-browse world followed a different mixed-species canopy trajectory: more Sitka promotions, fewer broadleaf promotions. This does not establish a field species sensitivity ranking or that browsing benefits Sitka. Canopy competition and eventual seed availability change during the run; controlled single-state fixtures are the appropriate causal comparison. Relative abundance and representative exact promotions are separate units.

The two stand_total_clearance_removed columns repeat the stand-level yearly total on each species row for context: do NOT sum them over species. Per-species accepted/rejected/light/browse/export/remaining and exact planted status are species-level. No vegetation-loss term exists in unchanged production. Candidate model-integrated per-cell ledger and ecological/economic acceptance remain pending input/model selection.
