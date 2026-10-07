# Salvage design

**Status:** proposal reusing existing work, economy and timber-yield concepts. **No new economy subsystem.**

## 1. Player choices

| Choice | Meaning | Existing concept reused |
|---|---|---|
| **LEAVE AS FALLEN DEADWOOD** (default) | Do nothing; the windthrown stem stays as a deadwood record and decays | The record already created at death; `FellingMaterialOutcome.RetainAsFallenDeadwood` semantics |
| **SELL / EXTRACT** | Contractor extracts and sells the stem | Harvest job quote/settlement (`QuoteHarvest`, `SettleHarvestJob`), timber yield with `WindDamage` quality sections |
| **KEEP FOR USE** | Extract to the landowner's retained-timber stock | `KeepForUse` disposition; retained timber |
| **PARTIAL SALVAGE** | The player chooses *which* windthrown stems to salvage; the rest stay as deadwood | Per-stem orders (like per-tree felling orders). **No fractional-stem state** |

Default stays deadwood because the game's honest default is "nature did this"; salvage is a decision.

## 2. Work-order shape (minimal extension)

- New `ScenarioWorkType.SalvageDeadwood` order. Target = deadwood record id (or the record's `treeId`, already a work-order field). Outcome = Sell / KeepForUse.
- **Created in the world:** aim at a windthrown log and press X ("Mark for salvage"). The same proposal → Work Plan → approval flow as felling (D-010: spatial choice while walking).
- **Bulk convenience:** the Work Plan offers "Add all windthrow from Year N" as a review-side convenience. It is not remote spatial design: the list is fixed by the storm. Whether that is acceptable under D-010 is a **PRODUCT DECISION**.
- **Eligibility:** the record's stem came from a `windthrow` death (tree cause), and `DecayClass ≤ 1` (about the first 4–7 years under 3 %/yr decay) [C]. After that, it is deadwood only.

## 3. Pricing (existing calculators)

1. **Yield:** reuse `MerchantableStemModel` + `TimberYieldCalculator` with the dead stem's recorded dimensions.
   - Mark the **butt section (0–1 m)** and the **top third** with `StemQualityFlags.WindDamage` [C]: root-plate shakes and top breakage.
   - The mid-stem keeps sawlog/pallet eligibility.
   - `WindDamage` already excludes sawlog/pallet/stake (pulp only) in the Sitka configuration.
2. **Delay loss:** each year after the storm, extend the damaged sections [C] (e.g. +1 m butt and −10 % sound length per year). It is simple, deterministic and uses existing flags. **Alternative:** a flat value multiplier. Rejected: it bypasses the yield model.
3. **Work cost:** the same `ForestryWorkCalculator` harvest schedule (`HarvestOperationContext` by intervention count), with `ForestryTask.SiteCostBasisPoints = 12500` (×1.25) for windblown extraction difficulty [C]. This uses the existing per-task multiplier, not a new rate table.
4. **Contractor minimum (€2,500):** salvage and thinning orders in the same year join **one commissioned harvest job**, as felling orders already do, so a storm year can be combined with a planned thinning to spread the minimum.
5. **Quote API:** `QuoteHarvest` currently requires `tree.IsLiving && CanChop`. **Change:** accept salvage orders whose target is a windthrow deadwood record. Build the stem from the record's `originalHeightMeters`, `originalDiameterCm` and **remaining volume**.

## 4. World effect of salvage

- Remove the deadwood record (Sell/Keep) and its log + root-plate visual. Leave the **root plate**, which marks the event [C presentation; optional].
- Spawn the existing compact brash residue at the base.
- Record a `ScenarioManagementEvent` `WorkResolved` with taskType `SalvageDeadwood` and an ecological treatment such as `WindthrowSalvaged` (enum addition; ints are save-compatible).
- `report.harvestedVolumeM3` / kept-for-use as for felling. Deadwood totals fall accordingly.

## 5. Trade-offs the player sees (no score)

| Dimension | Salvage | Leave |
|---|---|---|
| Money | Revenue (downgraded; falls with delay) − work (×1.25) − minimum unless combined | €0 |
| Deadwood / habitat | Lose the lying deadwood | Gain a deadwood pulse (counts toward the existing deadwood objective) |
| Regeneration gap | Unchanged: the gap is made by the storm, not by salvage | Unchanged |
| Safety/pest | Not simulated (UKFS notes both) — say so | — |
| Visual | Cleared logs, brash | Logs + root plates, decaying |

**Value-loss sanity check [C]:** the whole Year-0 stand is worth only about €1,702 notional (pedagogy harness), so early-storm salvage will rarely beat the €2,500 minimum on its own. That is honest. Salvage becomes worthwhile with larger stems (Year 20+), or when combined with a planned thinning. Calibrate in Unity and report the break-even year.

## 6. Interaction with objectives

- Windthrow deadwood counts toward the existing fallen-deadwood objective. That is legitimate: the objective measures deadwood on site.
- Windthrown original Sitka are no longer "retained canopy trees". A severe storm can reduce `retained-canopy` (≥ 60 required). **Calibration must check completion viability** (`LongRunCalibrationPlan.md`).
