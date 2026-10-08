# P3.1 — Current Work Plan audit

**Base:** `origin/main` @ `1fbefd8` (Regeneration Model 2, save v18). Source: `UI/WorkPlanView.cs`, `ScenarioOneManager`, `ScenarioOneEconomyAdapter`, `WorkEconomy/*`. Static inspection; no Unity run.

## 1. Values shown today

| Where | Value | Source |
|---|---|---|
| Header | Year; cash; "Your time next year: x of 40 h" | `CashCents`, `PlannedOwnerMinutes`, `OwnerMinutesPerYear` |
| Summary line | open / approved counts · external cost · expected timber · net · cash after approved work | `GetWorkPlanSummary()`, `ReservedContractorCashCents` |
| Thinning card | tree count; table of assortment / volume / roadside value; tops and residue; retained usable timber; retained deadwood volume; timber sales; harvesting work; small-job minimum; net for the job; small-visit note; eligibility; unmarketed-species note | `GetHarvestQuote(false)` → `ScenarioHarvestJob` (`Yield`, `Resolution.Quote.Costs`, `RevenueCents`, `CostCents`) |
| Thinning rows | tree id · expected volume · status; Sell / Keep for use / Leave as deadwood; Remove / Cancel approval | `ScenarioOneWorkOrder` |
| Planting card | species counts; shelters on/off; executor (contractor / me); shelter price; stock used vs owned; labour; cost; per-order rows | orders, `PlanningPlantingMethod`, `GetStockQuantity` |
| Pruning card | Crop Trees designated; eligible count; batch-add button; labour; cost; rows with target clear-stem height | `CropTreeCount`, `EligibleCropTreePruningCount` |
| Clearance card | per cell: "All competing vegetation · cell · minutes · cost · status" | orders |
| Nursery card | price each · owned · reserved · Buy n | `ShopEntries`, inventory |
| Reference Future card | visit years 20 / 50 / 100 | archive |
| Footer | feedback + tutorial hint; Add marked trees; Approve pending work; Advance one year | manager |

## 2. Calculated but not shown

| Value | Where it exists |
|---|---|
| Stems, basal area and volume left standing after the plan | Derivable from trees + open Fell orders (the residual stand). The Annual Review snapshot uses the same basal-area formula |
| Approved-only harvest quote, separate from all-open | `GetHarvestQuote(true)` (used for reserved cash only) |
| Expected cash **after revenue** | Not computed. Approval and advance compare cash with **cost** only (`ApprovePendingWork`, `CanAdvanceYearNow`); revenue arrives at settlement |
| Intervention count (first / second / later thinning rates) | `GetHarvestQuote` (`interventions`) |
| Clearance targets (cohorts, planted saplings, ground patches) | `QueryClearance(ClearanceFootprint.Cell(...))`. Shown in the walking preview only, not in the Work Plan |
| Model 2 bramble / bracken cover per cell and exposure | `UnderstoreyCells[i].brambleCover/brackenCover`, `CompetitionCellExposure(i)` |
| Seed-producing trees | `ForestEcologyController.GetSeedPotential(tree) > 0` (drives seed rain) |
| Deadwood on site | `DeadwoodRecords[].remainingVolumeM3` |

## 3. Quote and economy inputs

- **Harvest:** one commissioned job per resolution year, grouping all FellTree orders. `TimberYieldCalculator` assortments × IFA-2024 roadside prices [E]. Combined harvest+forward €/t by intervention (€21 / €23 / €20) [E]. Minimum job adjustment `max(0, €2,500 − variable)` [C]. **Contractor-only:** owner execution is ineligible.
- **Planting:** contractor (€45/h × planting minutes) or owner time (no cash). Stock consumed from inventory (bought earlier at the definition price). Shelter €5 material [D], same executor.
- **Pruning / clearance:** legacy hourly pricing: €45/h × (base + per-metre or per-density minutes). No minimum.
- **Approval rule:** total cost of open (pending + approved) work ≤ cash. Revenue is not netted.
- **Cash inflow:** only harvest settlement (`SettleHarvestJob`). Every other path spends.

## 4. Removal counts and preview calculations

- Thinning card count = open valid FellTree orders. Marks become orders on Work Plan open or "Add marked trees".
- The walking forecast (`ForestTreeMarkingManager.UpdateTreatmentOutcome`) estimates kept volume, stand-average growth change, gap light, wind peak and opening, from **marks**, not orders. The Work Plan shows none of that.
- The clearance preview (walking) shows footprint and target counts. The Work Plan clearance row shows only cost.

## 5. Execution method

Shown for planting (contractor / me) and implied for harvest ("Contractor only"). Pruning and clearance are contractor (`executionMethod` default). Owner minutes are shown in the header and planting card.

## 6. Regeneration / clearance effects already available

- Model 2 (new games): clearance zeros cell bramble/bracken cover in the treatment year (`ResetCellCompetition`). Removes the cohorts and unpromoted planted juveniles in the cell. Cover regrows toward the light-dependent target. Vegetation survival loss applies to juveniles below the escape height (1.5 m, `CompetitionCalibration.EscapeHeight`).
- Model-aware wording already exists: `LearningObjectivesView.ClearanceExplanation(model)` (gated by `Model2PedagogyVerification`).
- Felling has **no** harvest-damage effect on young trees (unsupported; not shown).

## 7. Gaps P3 addresses

1. Nothing tells the player what **remains**.
2. No expected cash **after** revenue, and no warning that a plan can leave cash below the harvest minimum (the dead end).
3. The clearance row hides the young trees it removes and the cover it clears.
4. The minimum-charge note explains cost, but not that same-year felling shares one visit or that other work has no minimum.
