# Residual-stand metric audit (Workstreams D1–D4)

**Status:** audit [REPO] plus harness evidence [HARNESS]. No production variable is added or proposed here.

Classification:

- **AUTHORITATIVE NOW** — stored in simulation or save state and read directly.
- **DERIVABLE NOW** — computed from authoritative state with existing production formulas, with no new state.
- **FUTURE ONLY** — needs new saved or simulated state.
- **UNSUPPORTED** — no basis in the simulation. Showing it would invent ecology.

## 1. Metric classification (D1)

| Metric | Class | Source / formula | Notes |
|---|---|---|---|
| Retained tree count | AUTHORITATIVE NOW | living `ForestTree`s minus marked | — |
| Retained basal area (m²/ha) | DERIVABLE NOW | Σ π(DBH/200)² / stand ha (same as `ScenarioEcologicalSnapshot.basalAreaM2PerHa`) | snapshot stores the annual value |
| Retained standing volume | DERIVABLE NOW | Σ `ForestTree.BiologicalStemVolumeM3` | not stored per year |
| Marked timber value (gross, cost, minimum, net) | DERIVABLE NOW | `ScenarioOneManager.GetHarvestQuote` (production WorkEconomy + TimberYield) | the harness uses the production quote |
| Retained "value" | DERIVABLE NOW, **with caveat** | the same quote over all retained trees, i.e. gross roadside value if everything were cut now | not a market or forest valuation; label it "notional" |
| Crop Trees retained | AUTHORITATIVE NOW | `TreeMarkType.CropTree` | — |
| Crop Trees released | DERIVABLE NOW | per Crop Tree: CI before (production `GetCompetitionIndex`) vs after (Σ `HegyiTerm` over retained neighbours within 8 m) | §2 |
| Local competition around Crop Trees | DERIVABLE NOW | as above | — |
| Predicted Crop-Tree DBH response (next year) | DERIVABLE NOW | production `GrowAdults` expression with CI after | §2 |
| Seed sources retained | DERIVABLE NOW | `TreeSpeciesDefinition.Maturity(age) > 0` | 0 at Year 0; all 336 at Year 10 |
| Species composition | AUTHORITATIVE NOW | `ForestTree.Species` | all Sitka at start |
| Gap distribution | DERIVABLE NOW | forecast cell light (production canopy formula), cells touched, largest connected opened patch, Clark–Evans index | §4 |
| Regeneration cells | AUTHORITATIVE NOW | cell bands; `RegeneratingCellCount` | — |
| Browse / protection status | AUTHORITATIVE NOW | `BrowsingConditions`, shelters | stem-level, planted only |
| Deadwood | AUTHORITATIVE NOW | deadwood records; felling-outcome choice for planned deadwood | — |
| H/D (slenderness) | AUTHORITATIVE NOW | height / DBH | — |
| Wind diagnostic | DERIVABLE NOW, **not discriminating** | production `GetWindRisk` formula with forecast light and opening | every tree reads "high" at Year 0 (scene thresholds 5/12; values 17–30). Not a windthrow probability |
| Light distribution | DERIVABLE NOW | production canopy formula | per cell |
| Crop-Tree growth since designation | FUTURE ONLY | needs DBH at designation (save) | see the Forest Diary paper |
| Standing volume / value per past year | FUTURE ONLY | not in snapshots | — |
| Crown overlap / crown class | UNSUPPORTED | no crown-position state | do not infer from height alone |
| Stem form / timber quality | UNSUPPORTED | no state | — |
| Harvest damage to residual trees or regeneration | UNSUPPORTED | not simulated | — |
| Microhabitats (cavities, veteran features) | UNSUPPORTED | no state | relevant to the Bio Tree decision |

## 2. Crop-tree release (D2)

**Question:** "Did this intervention actually release the selected Crop Trees?"

**Measures (all production relationships):**

1. Per-Crop-Tree competition before → after, and the percentage change.
2. **Count of Crop Trees released by ≥10 % and by ≥25 %.** These are presentation bands, not ecological thresholds; they must be labelled.
3. Predicted next-year DBH growth before → after (potential × site × size × 1/(1 + CI/Ci50)).
4. Removal efficiency: Σ Hegyi terms removed from Crop Trees per m²/ha of basal area removed.
5. Removals not touching any Crop Tree (no Crop Tree within 8 m).

**No new growth bonus.** The predicted response is the existing equation with fewer neighbours.

**Evidence [HARNESS], Year 0, 16 Crop Trees:**

| | T1 clean-up | T2 release | T3 heavy | T4 conservative | T5 gap |
|---|---|---|---|---|---|
| Removed | 84 | 30 | 87 | 15 | 48 |
| Mean Crop-Tree competition change | −16.6 % | −19.7 % | −44.5 % | −11.1 % | −16.0 % |
| Crop Trees released ≥10 % | 14 | **16** | 16 | 10 | **7** |
| … not released at all | 0 | 0 | 0 | 0 | **9** |
| Mean predicted Crop-Tree growth change | +9.3 % | +11.7 % | +30.8 % | +6.4 % | **+13.1 %** |
| Stand-wide growth change (what the HUD shows now) | **+12.2 %** | +6.7 % | +20.5 % | +3.3 % | +4.0 % |
| Release per m²/ha removed | 2.57 | 3.95 | 3.32 | **4.25** | 2.82 |
| Removals each under 5 % of every nearby Crop Tree's competition | 77 | 0 | 26 | 0 | 39 |

Lessons for the metric design:

- **A mean hides concentration.** T5's mean growth gain beats T2's, but 9 of 16 Crop Trees get nothing. Report the released count alongside the mean.
- **The stand-wide forecast inverts the ranking.** T1 looks best on the current HUD line. Show the Crop-Tree figure first.
- **Efficiency favours targeted removals.** T4 and T2 release the most per unit removed.

## 3. Productive capital (D3)

The summary must separate **value harvested** from **capital left growing**:

| Year 0 | Harvested gross | Net of contractor | Retained basal area | Retained volume | Retained notional value |
|---|---|---|---|---|---|
| T0 | — | — | 41.1 m²/ha | 39.9 m³ | €1,702 |
| T4 | €134 | −€2,366 | 38.6 | 37.1 | €1,568 |
| T2 | €250 | −€2,250 | 36.2 | 34.7 | €1,452 |
| T5 | €216 | −€2,284 | 35.6 | 34.6 | €1,486 |
| T1 | €153 | −€2,347 | 34.8 | 35.0 | €1,549 |
| T3 | €629 | −€1,871 | 28.0 | 26.3 | €1,073 |

At Year 10 (untreated) the whole stand is worth €4,063 notional, and a T2-style thinning grosses €560 [HARNESS].

**Do not imply that maximum retained capital is best.** T0 keeps the most capital and releases nothing. The capital figure is one dimension among several.

## 4. Spatial opening (D4)

Same removed volume, different pattern [HARNESS]:

| | T2 distributed (5.19 m³) | T5 concentrated (5.22 m³) |
|---|---|---|
| Trees removed | 30 | 48 |
| Cells touched | 23 | 11 |
| Max removals in one cell | 2 | 7 |
| Clark–Evans index of removals (1 = random, < 1 clustered) | 0.76 | 0.58 |
| Mean ground light after | 0.060 | 0.130 |
| Cells with light ≥ 0.20 | 5 | 11 |
| Largest connected opened patch (cells with light +0.05 or more) | 3 | 9 |
| Regenerating cells: Year 1 / Year 20 | 17 / 40 | 23 / 40 |

The existing spatial state (positions, cell light, recent opening) is sufficient. **No production variable is missing for D4.**

## 5. Missing observations (D7) — exact list

| Needed for | Missing observation | Class |
|---|---|---|
| "How much did my Crop Tree grow since the thinning?" | DBH at designation or at the intervention year | FUTURE ONLY (save) |
| "When did regeneration start here?" | cell-level regeneration history | FUTURE ONLY (save) |
| Excessive exposure lesson | a discriminating wind label | presentation calibration (not a new variable) |
| Wolf-tree / form lessons | stem form / quality | UNSUPPORTED (new state; product decision) |
| Bole protection by small trees | — | UNSUPPORTED (not simulated) |

**No production variable was added to gather this evidence.**
