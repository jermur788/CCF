namespace SitkaP1.Core;

// Pure, read-only C-ECON quote computation, atomic Execute settlement builder, and year-end summary/allowance reset.
// Never acquires locks, never mutates MultiAreaRoot, never uses floating-point currency math.

internal static class EconomyQuoteCalculator
{
    /// <summary>Computes a pure, read-only quote over the pending Work Plan orders on <paramref name="root"/>.</summary>
    internal static EconomyWorkPlanQuote ComputePending(MultiAreaRoot root, MultiAreaSourceToken token)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(token);
        var econ = root.Economy ?? throw new P1Refusal("EconomyModelRequired", "Economy must be enabled to compute a quote");
        var pendingPrunes = root.Stage4?.PendingPrunes ?? Array.Empty<MultiAreaPruneOrder>();
        var pendingClears = root.Stage5?.PendingClears ?? Array.Empty<MultiAreaClearUndergrowthOrder>();
        var pendingPlanting = root.Phase2?.PendingOrders ?? Array.Empty<MultiAreaPlantingOrder>();
        var dispositions = FellingDispositionRules.Resolve(econ.PendingFellDispositions, root.Pending);
        return ComputeCore(root, token, econ, root.Pending, pendingPrunes, pendingClears, pendingPlanting,
            econ.PendingOrderExecutors, dispositions,
            executionKey: string.Empty, planId: string.Empty, workRevision: root.WorkRevision);
    }

    /// <summary>Recomputes a verification quote over the currently approved orders using the frozen executor choices.</summary>
    internal static EconomyWorkPlanQuote ComputeForApproved(
        MultiAreaRoot root,
        MultiAreaSourceToken token,
        MultiAreaApprovedPlan approvedPlan,
        EconomyApprovedQuote frozen,
        int workRevision)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(approvedPlan);
        ArgumentNullException.ThrowIfNull(frozen);
        var econ = root.Economy ?? throw new P1Refusal("EconomyModelRequired", "Economy must be enabled to settle an approved plan");
        var approvedPrunes = root.Stage4?.ApprovedPrunes ?? Array.Empty<MultiAreaPruneOrder>();
        var approvedClears = root.Stage5?.ApprovedClears ?? Array.Empty<MultiAreaClearUndergrowthOrder>();
        var approvedPlanting = root.Phase2?.ApprovedOrders ?? Array.Empty<MultiAreaPlantingOrder>();
        return ComputeCore(root, token, econ, approvedPlan.Orders, approvedPrunes, approvedClears, approvedPlanting,
            frozen.FrozenOrderExecutors, frozen.FrozenFellDispositions,
            executionKey: approvedPlan.ExecutionKey, planId: approvedPlan.PlanId, workRevision: workRevision);
    }

    /// <summary>Throws the first blocking economic refusal if <paramref name="quote"/> is ineligible.</summary>
    internal static void RequireEligible(EconomyWorkPlanQuote quote)
    {
        if (quote.Issues.Count == 0) return;
        var first = quote.Issues[0];
        throw new P1Refusal(first.Code, first.Message);
    }

    /// <summary>
    /// Validates the frozen quote against live committed state and builds the atomic settlement + next economy state
    /// for Execute. Pure: returns new immutable values without mutating <paramref name="root"/>.
    /// </summary>
    internal static (EconomySettlementReceipt Receipt, MultiAreaEconomyState NextEconomy) BuildSettlement(
        MultiAreaRoot root,
        MultiAreaSourceToken token,
        MultiAreaApprovedPlan approvedPlan,
        int workRevision)
    {
        var econ = root.Economy ?? throw new P1Refusal("EconomyModelRequired", "Economy must be enabled to settle");
        var frozen = econ.ApprovedQuote;
        if (frozen is null ||
            !string.Equals(frozen.PlanId, approvedPlan.PlanId, StringComparison.Ordinal) ||
            !string.Equals(frozen.ExecutionKey, approvedPlan.ExecutionKey, StringComparison.Ordinal))
        {
            throw new P1Refusal("EconomyApprovedQuoteMissing",
                "the approved plan has no matching frozen Economy quote; cancel and re-approve");
        }

        // Recompute against current root with the frozen assignments and verify all preconditions all-or-nothing.
        var verified = ComputeForApproved(root, token, approvedPlan, frozen, workRevision);
        RequireEligible(verified);

        var frozenQuote = frozen.Quote;
        if (verified.TotalExternalCostCents != frozenQuote.TotalExternalCostCents ||
            verified.TimberSaleRevenueCents != frozenQuote.TimberSaleRevenueCents ||
            verified.TotalOwnerMinutesRequired != frozenQuote.TotalOwnerMinutesRequired)
        {
            throw new P1Refusal("EconomyQuoteDrift",
                "live stand or plan state diverged from the frozen Economy approval quote; cancel and re-approve");
        }

        // Phase 2B-2b: the felling dispositions are frozen at Approve. A live disposition map that no longer matches the
        // frozen one, or a physical outcome (retained volume, deadwood intents) that differs, refuses instead of repricing.
        var liveDispositions = FellingDispositionRules.Resolve(econ.PendingFellDispositions, approvedPlan.Orders);
        var verifiedJob = verified.HarvestJob;
        var frozenJob = frozenQuote.HarvestJob;
        if (!FellingDispositionRules.SameResolved(liveDispositions, frozen.FrozenFellDispositions) ||
            !FellingDispositionRules.SameResolved(verified.FellDispositions, frozen.FrozenFellDispositions) ||
            (verifiedJob?.RetainedTimber.RetainedTimberCm3 ?? 0L) != (frozenJob?.RetainedTimber.RetainedTimberCm3 ?? 0L) ||
            (verifiedJob?.DeadwoodIntents.Count ?? 0) != (frozenJob?.DeadwoodIntents.Count ?? 0))
        {
            throw new P1Refusal("EconomyQuoteDrift",
                "felling dispositions diverged from the frozen Economy approval quote; cancel and re-approve");
        }

        checked
        {
            long cashBefore = econ.CashBalanceCents;
            if (cashBefore < frozenQuote.UpfrontCashRequiredCents)
                throw new P1Refusal("EconomyInsufficientCash",
                    $"upfront cash {EconomyDefaults.FormatEuros(cashBefore)} cannot cover required external costs {EconomyDefaults.FormatEuros(frozenQuote.UpfrontCashRequiredCents)}; expected timber revenue never pre-finances work");

            int ownerBefore = econ.RemainingLandownerMinutes;
            if (ownerBefore < frozenQuote.TotalOwnerMinutesRequired)
                throw new P1Refusal("EconomyInsufficientLandownerAllowance",
                    $"remaining landowner allowance ({ownerBefore} min) is less than required ({frozenQuote.TotalOwnerMinutesRequired} min)");

            long cashAfter = cashBefore - frozenQuote.TotalExternalCostCents + frozenQuote.TimberSaleRevenueCents;
            if (cashAfter < 0L)
                throw new P1Refusal("EconomyNegativeBalanceForbidden", "cash balance may never become negative");

            int ownerAfter = ownerBefore - frozenQuote.TotalOwnerMinutesRequired;
            if (ownerAfter < 0)
                throw new P1Refusal("EconomyInsufficientLandownerAllowance", "landowner allowance may never become negative");

            // Stamp ledger entries with the committed execution identity and verify exact reconciliation.
            var ledgerEntries = verified.EstimatedLedger
                .Select(e => e with
                {
                    EcologicalYear = root.EcologicalYear,
                    WorkRevision = workRevision,
                    ExecutionKey = approvedPlan.ExecutionKey,
                    PlanId = approvedPlan.PlanId,
                })
                .ToArray();

            long ledgerSum = 0L;
            foreach (var entry in ledgerEntries) ledgerSum += entry.CashDeltaCents;
            if (ledgerSum != verified.NetCashDeltaCents)
                throw new InvalidOperationException("Economy ledger reconciliation invariant violated.");

            var receipt = new EconomySettlementReceipt(
                ExecutionKey: approvedPlan.ExecutionKey,
                PlanId: approvedPlan.PlanId,
                EcologicalYear: root.EcologicalYear,
                WorkRevision: workRevision,
                CashBeforeCents: cashBefore,
                TotalExternalCostCents: frozenQuote.TotalExternalCostCents,
                TimberSaleRevenueCents: frozenQuote.TimberSaleRevenueCents,
                NetCashDeltaCents: frozenQuote.NetCashDeltaCents,
                CashAfterCents: cashAfter,
                OwnerMinutesBefore: ownerBefore,
                OwnerMinutesConsumed: frozenQuote.TotalOwnerMinutesRequired,
                OwnerMinutesAfter: ownerAfter,
                HarvestJob: frozenQuote.HarvestJob,
                NonHarvestLines: frozenQuote.NonHarvestLines,
                LedgerEntries: Array.AsReadOnly(ledgerEntries))
            {
                // Physical, non-cash outcomes of the dispositions: never part of the cash ledger reconciliation above.
                RetainedTimberDelta = (verifiedJob?.RetainedTimber ?? RetainedTimberTally.Empty)
                    .StampedWith(root.EcologicalYear, workRevision, approvedPlan.ExecutionKey, approvedPlan.PlanId),
                DeadwoodIntents = Array.AsReadOnly((verifiedJob?.DeadwoodIntents ?? Array.Empty<FelledStemDeadwoodIntent>())
                    .Select(i => i with
                    {
                        EcologicalYear = root.EcologicalYear,
                        WorkRevision = workRevision,
                        ExecutionKey = approvedPlan.ExecutionKey,
                        PlanId = approvedPlan.PlanId,
                    })
                    .ToArray()),
            };

            int nextHarvestCount = econ.ExecutedHarvestPlansCount + (frozenQuote.HarvestJob is not null ? 1 : 0);
            var nextEconomy = econ with
            {
                CashBalanceCents = cashAfter,
                UsedLandownerMinutesThisYear = econ.UsedLandownerMinutesThisYear + frozenQuote.TotalOwnerMinutesRequired,
                RemainingLandownerMinutes = ownerAfter,
                ExecutedHarvestPlansCount = nextHarvestCount,
                ApprovedQuote = null,
                Settlements = Array.AsReadOnly(econ.Settlements.Append(receipt).ToArray()),
                Ledger = Array.AsReadOnly(econ.Ledger.Concat(ledgerEntries).ToArray()),
                PendingFellDispositions = FellingDispositionRules.EmptyChoices,
                RetainedTimber = econ.RetainedTimber.Plus(receipt.RetainedTimberDelta),
            };

            return (receipt, nextEconomy);
        }
    }

    private static EconomyWorkPlanQuote ComputeCore(
        MultiAreaRoot root,
        MultiAreaSourceToken token,
        MultiAreaEconomyState econ,
        IReadOnlyList<MultiAreaWorkOrder> fells,
        IReadOnlyList<MultiAreaPruneOrder> prunes,
        IReadOnlyList<MultiAreaClearUndergrowthOrder> clears,
        IReadOnlyList<MultiAreaPlantingOrder> plantingOrders,
        IReadOnlyDictionary<EconomyOrderKey, WorkExecutor> orderExecutors,
        IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> fellDispositions,
        string executionKey,
        string planId,
        int workRevision)
    {
        checked
        {
            var issues = new List<EconomyQuoteIssue>();
            var ledger = new List<EconomyLedgerEntry>();

            // Index trees by (AreaId, TreeId) for fast deterministic lookup.
            var treesByKey = new Dictionary<(string AreaId, string TreeId), AnnualTree>();
            foreach (var area in root.Areas)
            {
                foreach (var tree in area.State.Trees)
                    treesByKey[(area.AreaId, tree.TreeId)] = tree;
            }

            // 1. Grouped contractor harvest job (all Fell orders in this Work Plan).
            EconomyHarvestQuote? harvestQuote = null;
            long harvestCostCents = 0L;
            long timberRevenueCents = 0L;
            var resolvedDispositions = new SortedDictionary<EconomyOrderKey, FellingDisposition>();
            foreach (var fell in fells)
            {
                var key = EconomyOrderKey.ForFell(fell.AreaId, fell.TreeId);
                resolvedDispositions[key] = FellingDispositionRules.Of(fellDispositions, key);
            }

            if (fells.Count > 0)
            {
                foreach (var fell in fells.OrderBy(o => o.AreaId, StringComparer.Ordinal).ThenBy(o => o.TreeId, StringComparer.Ordinal))
                {
                    var key = EconomyOrderKey.ForFell(fell.AreaId, fell.TreeId);
                    if (orderExecutors.TryGetValue(key, out var assigned) && assigned == WorkExecutor.Landowner)
                    {
                        issues.Add(new EconomyQuoteIssue(
                            "EconomyFellContractorOnly",
                            $"Fell order for {fell.TreeId} cannot be assigned to Landowner; commercial felling is contractor-only"));
                    }
                }

                var stage = EconomyDefaults.AutoHarvestStage(econ.ExecutedHarvestPlansCount);
                long stageRate = EconomyDefaults.HarvestRateCentsPerTonne(stage);
                var stemYields = new List<D062StemYield>(fells.Count);
                var soldYields = new List<D062StemYield>(fells.Count);
                var stemLines = new List<FelledStemDispositionLine>(fells.Count);
                var retainedStems = new List<RetainedTimberStem>();
                var deadwoodIntents = new List<FelledStemDeadwoodIntent>();
                long totalStemVol = 0L;
                long totalSaleVol = 0L;
                long totalResidualVol = 0L;
                long totalHarvestedGrams = 0L;
                long retainedVol = 0L;
                long deadwoodVol = 0L;

                foreach (var fell in fells.OrderBy(o => o.TreeId, StringComparer.Ordinal))
                {
                    if (!treesByKey.TryGetValue((fell.AreaId, fell.TreeId), out var tree)) continue;
                    string speciesId = root.Phase2?.SpeciesIdOf(tree.TreeId) ?? Phase2Species.SitkaId;
                    var pruning = root.Stage4?.PruningOf(tree.TreeId) ?? TreePruningState.Unpruned;
                    var yield = D062TimberAssortmentEngine.GradeStem(tree, speciesId, pruning);
                    stemYields.Add(yield);
                    // The contractor fells and handles every stem whatever its disposition: same mass, same cost basis.
                    totalStemVol += yield.StemVolumeCm3;
                    totalHarvestedGrams += yield.HarvestedGreenGrams;

                    var disposition = resolvedDispositions[EconomyOrderKey.ForFell(fell.AreaId, fell.TreeId)];
                    switch (disposition)
                    {
                        case FellingDisposition.SellAndExtract:
                            soldYields.Add(yield);
                            totalSaleVol += yield.SaleVolumeCm3;
                            totalResidualVol += yield.ResidualVolumeCm3;
                            timberRevenueCents += yield.TimberRevenueCents;
                            break;
                        case FellingDisposition.KeepForUse:
                            retainedStems.Add(new RetainedTimberStem(fell.AreaId, tree.TreeId, speciesId, yield.StemVolumeCm3,
                                root.EcologicalYear, workRevision, executionKey, planId));
                            retainedVol += yield.StemVolumeCm3;
                            break;
                        case FellingDisposition.LeaveAsDeadwood:
                            if (root.Stage4 is null)
                                throw new P1Refusal("EconomyDeadwoodRequiresStage4",
                                    "LeaveAsDeadwood requires Stage 4 deadwood state; this session has no Stage 4 deadwood tracking");
                            deadwoodIntents.Add(FelledStemDeadwood.IntentFor(fell.AreaId, tree, speciesId,
                                root.EcologicalYear, workRevision, executionKey, planId));
                            deadwoodVol += yield.StemVolumeCm3;
                            break;
                        default:
                            throw new P1Refusal("EconomyDispositionUnknown", "unknown felling disposition");
                    }
                    stemLines.Add(new FelledStemDispositionLine(fell.AreaId, tree.TreeId, speciesId, disposition,
                        yield.StemVolumeCm3, yield.HarvestedGreenGrams,
                        CreditedTimberRevenueCents: disposition == FellingDisposition.SellAndExtract ? yield.TimberRevenueCents : 0L,
                        RetainedTimberCm3: disposition == FellingDisposition.KeepForUse ? yield.StemVolumeCm3 : 0L,
                        EmitsDeadwoodIntent: disposition == FellingDisposition.LeaveAsDeadwood));
                }

                long variableCost = EconomyDefaults.RoundRatio(totalHarvestedGrams, stageRate, EconomyDefaults.GramsPerTonne);
                long floor = econ.Options.MinimumHarvestJobCents;
                long minAdj = Math.Max(0L, floor - variableCost);
                harvestCostCents = variableCost + minAdj;

                // Only sold stems produce assortments, TimberSale ledger lines and revenue.
                var assortments = D062TimberAssortmentEngine.Summarize(soldYields);
                harvestQuote = new EconomyHarvestQuote(
                    Stage: stage,
                    StageRateCentsPerTonne: stageRate,
                    FelledStemCount: stemYields.Count,
                    TotalStemVolumeCm3: totalStemVol,
                    SaleVolumeCm3: totalSaleVol,
                    ResidualVolumeCm3: totalResidualVol,
                    HarvestedGreenGrams: totalHarvestedGrams,
                    VariableHarvestCostCents: variableCost,
                    MinimumJobFloorCents: floor,
                    MinimumJobAdjustmentCents: minAdj,
                    TotalHarvestContractorCostCents: harvestCostCents,
                    TimberSaleRevenueCents: timberRevenueCents,
                    Stems: Array.AsReadOnly(stemYields.ToArray()),
                    Assortments: assortments)
                {
                    StemDispositions = Array.AsReadOnly(stemLines.ToArray()),
                    RetainedStemVolumeCm3 = retainedVol,
                    DeadwoodStemVolumeCm3 = deadwoodVol,
                    RetainedTimber = RetainedTimberTally.FromStems(
                        retainedStems.OrderBy(r => r.AreaId, StringComparer.Ordinal).ThenBy(r => r.TreeId, StringComparer.Ordinal)),
                    DeadwoodIntents = Array.AsReadOnly(deadwoodIntents
                        .OrderBy(i => i.AreaId, StringComparer.Ordinal).ThenBy(i => i.TreeId, StringComparer.Ordinal).ToArray()),
                };

                if (variableCost > 0L)
                    ledger.Add(new EconomyLedgerEntry(EconomyLedgerCategory.HarvestingAndForwarding,
                        "harvest-variable", -variableCost, root.EcologicalYear, workRevision, executionKey, planId));
                if (minAdj > 0L)
                    ledger.Add(new EconomyLedgerEntry(EconomyLedgerCategory.MinimumJobAdjustment,
                        "harvest-minimum-floor", -minAdj, root.EcologicalYear, workRevision, executionKey, planId));
                foreach (var a in assortments)
                {
                    if (a.RevenueCents > 0L)
                        ledger.Add(new EconomyLedgerEntry(EconomyLedgerCategory.TimberSale,
                            $"{a.SpeciesId}:{a.Assortment}", a.RevenueCents, root.EcologicalYear, workRevision, executionKey, planId));
                }
            }

            // 2. Non-harvest Work Plan orders (Prune, ClearUndergrowth, Plant, Shelter, SpotClear).
            var lines = new List<EconomyNonHarvestLineQuote>();
            int totalOwnerMinutes = 0;
            int totalContractorMinutes = 0;
            long totalNonHarvestLabourCents = 0L;
            long totalMaterialCents = 0L;

            void AddLine(EconomyOrderKey key, WorkExecutor exec, int personMin, string? materialId, long materialCents, float pruneTargetM = 0f)
            {
                if (exec == WorkExecutor.Landowner && !EconomyDefaults.IsLandownerEligible(key.Kind, pruneTargetM))
                {
                    issues.Add(new EconomyQuoteIssue(
                        "EconomyLandownerTaskIneligible",
                        $"{key.Kind} ({key.TargetKey}) is not eligible for landowner DIY execution"));
                }
                int ownerMin = exec == WorkExecutor.Landowner ? personMin : 0;
                int contractorMin = exec == WorkExecutor.Contractor ? personMin : 0;
                long labourCents = contractorMin > 0
                    ? EconomyDefaults.RoundRatio(contractorMin, econ.Options.ContractorHourlyRateCents, 60L)
                    : 0L;
                long lineExternal = labourCents + materialCents;

                lines.Add(new EconomyNonHarvestLineQuote(key, exec, personMin, ownerMin, contractorMin,
                    labourCents, materialId, materialCents, lineExternal));

                totalOwnerMinutes += ownerMin;
                totalContractorMinutes += contractorMin;
                totalNonHarvestLabourCents += labourCents;
                totalMaterialCents += materialCents;

                if (labourCents > 0L)
                    ledger.Add(new EconomyLedgerEntry(EconomyLedgerCategory.ContractorLabour,
                        $"{key.Kind}:{key.TargetKey}", -labourCents, root.EcologicalYear, workRevision, executionKey, planId));
                if (materialCents > 0L && materialId is not null)
                    ledger.Add(new EconomyLedgerEntry(EconomyLedgerCategory.Materials,
                        $"{materialId}:{key.TargetKey}", -materialCents, root.EcologicalYear, workRevision, executionKey, planId));
            }

            // Prunes (4 min/tree = 60 min / 15 trees)
            foreach (var prune in prunes.OrderBy(p => p.AreaId, StringComparer.Ordinal).ThenBy(p => p.TreeId, StringComparer.Ordinal))
            {
                var key = EconomyOrderKey.ForPrune(prune.AreaId, prune.TreeId);
                var exec = orderExecutors.TryGetValue(key, out var e) ? e : WorkExecutor.Contractor;
                int mins = (int)EconomyDefaults.CeilRatio(1L, EconomyDefaults.PruneMinutesPerReference, EconomyDefaults.PruneReferenceItems);
                AddLine(key, exec, mins, null, 0L, prune.TargetCrownBaseHeightM);
            }

            // ClearUndergrowth (12 min/cell)
            foreach (var clear in clears.OrderBy(c => c.AreaId, StringComparer.Ordinal).ThenBy(c => c.Column).ThenBy(c => c.Row))
            {
                var key = EconomyOrderKey.ForClear(clear.AreaId, clear.Column, clear.Row);
                var exec = orderExecutors.TryGetValue(key, out var e) ? e : WorkExecutor.Contractor;
                AddLine(key, exec, EconomyDefaults.ClearUndergrowthMinutesPerCell, null, 0L);
            }

            // Phase 2 planting orders in canonical resolution order (SpotClear -> Plant -> Shelter, then OrderId)
            foreach (var order in plantingOrders.OrderBy(o => o.ResolveRank).ThenBy(o => o.OrderId, StringComparer.Ordinal))
            {
                var key = EconomyOrderKey.ForPlanting(order);
                var exec = orderExecutors.TryGetValue(key, out var e) ? e : WorkExecutor.Contractor;
                switch (order)
                {
                    case MultiAreaSpotClearOrder:
                        AddLine(key, exec, EconomyDefaults.SpotClearMinutesPerPatch, null, 0L);
                        break;
                    case MultiAreaPlantOrder plant:
                        var (matId, matCents) = EconomyDefaults.SaplingMaterialFor(plant.SpeciesId);
                        AddLine(key, exec, EconomyDefaults.PlantMinutesPerSapling, matId, matCents);
                        break;
                    case MultiAreaShelterOrder:
                        int shelterMins = (int)EconomyDefaults.CeilRatio(1L, EconomyDefaults.ShelterMinutesPerReference, EconomyDefaults.ShelterReferenceItems);
                        AddLine(key, exec, shelterMins, EconomyDefaults.ShelterMaterialId, EconomyDefaults.ShelterMaterialCents);
                        break;
                }
            }

            long totalExternalCostCents = harvestCostCents + totalNonHarvestLabourCents + totalMaterialCents;
            long upfrontCashRequired = totalExternalCostCents;
            long netCashDelta = timberRevenueCents - totalExternalCostCents;

            // 3. Upfront cash (no debt, expected timber revenue NEVER pre-finances) and landowner allowance guards.
            if (econ.CashBalanceCents < upfrontCashRequired)
            {
                issues.Add(new EconomyQuoteIssue(
                    "EconomyInsufficientCash",
                    $"upfront cash {EconomyDefaults.FormatEuros(econ.CashBalanceCents)} is insufficient for external costs {EconomyDefaults.FormatEuros(upfrontCashRequired)}; expected timber revenue ({EconomyDefaults.FormatEuros(timberRevenueCents)}) does not pre-finance work",
                    Required: upfrontCashRequired,
                    Available: econ.CashBalanceCents));
            }
            if (econ.RemainingLandownerMinutes < totalOwnerMinutes)
            {
                issues.Add(new EconomyQuoteIssue(
                    "EconomyInsufficientLandownerAllowance",
                    $"remaining annual landowner allowance ({econ.RemainingLandownerMinutes} min) is insufficient for {totalOwnerMinutes} min of assigned landowner work",
                    Required: totalOwnerMinutes,
                    Available: econ.RemainingLandownerMinutes));
            }

            // Reconcile estimated ledger sum == netCashDelta.
            long estSum = 0L;
            foreach (var entry in ledger) estSum += entry.CashDeltaCents;
            if (estSum != netCashDelta)
                throw new InvalidOperationException("Estimated Economy ledger does not reconcile with net cash delta.");

            return new EconomyWorkPlanQuote(
                PlanStateRevision: root.PlanStateRevision,
                Source: token,
                EcologicalYear: root.EcologicalYear,
                AvailableCashBeforeCents: econ.CashBalanceCents,
                AvailableLandownerMinutesBefore: econ.RemainingLandownerMinutes,
                HarvestJob: harvestQuote,
                NonHarvestLines: Array.AsReadOnly(lines.ToArray()),
                ContractorHarvestCostCents: harvestCostCents,
                ContractorNonHarvestLabourCents: totalNonHarvestLabourCents,
                MaterialCostCents: totalMaterialCents,
                TotalExternalCostCents: totalExternalCostCents,
                UpfrontCashRequiredCents: upfrontCashRequired,
                TimberSaleRevenueCents: timberRevenueCents,
                NetCashDeltaCents: netCashDelta,
                TotalOwnerMinutesRequired: totalOwnerMinutes,
                TotalContractorMinutes: totalContractorMinutes,
                EstimatedLedger: Array.AsReadOnly(ledger.ToArray()),
                Issues: Array.AsReadOnly(issues.ToArray()),
                EvidenceLabels: EconomyEvidence.All)
            {
                FellDispositions = FellingDispositionRules.ReadOnly(resolvedDispositions),
            };
        }
    }
}

/// <summary>
/// Pure year-end economy transition invoked exclusively by <see cref="ReservedPhase2BYearEndSummaryStep"/>.
/// Records the closing annual summary, resets the 40.0 h annual landowner allowance, and clears cancelled work metadata.
/// </summary>
internal static class EconomyAnnualSummary
{
    internal static MultiAreaEconomyState BuildYearEnd(MultiAreaEconomyState current, int yearBefore, int yearAfter, bool cancelledWork)
    {
        ArgumentNullException.ThrowIfNull(current);
        checked
        {
            long yearCost = 0L;
            long yearRev = 0L;
            int harvestPlansInYear = 0;
            int settledInYear = 0;
            long? firstBefore = null;
            foreach (var s in current.Settlements)
            {
                if (s.EcologicalYear != yearBefore) continue;
                firstBefore ??= s.CashBeforeCents;
                yearCost += s.TotalExternalCostCents;
                yearRev += s.TimberSaleRevenueCents;
                if (s.HarvestJob is not null) harvestPlansInYear++;
                settledInYear++;
            }
            long openingCash = firstBefore ?? current.CashBalanceCents;
            long yearNet = yearRev - yearCost;

            var summary = new EconomyYearEndSummary(
                YearBefore: yearBefore,
                YearAfter: yearAfter,
                OpeningCashCents: openingCash,
                ClosingCashCents: current.CashBalanceCents,
                YearExternalCostCents: yearCost,
                YearTimberRevenueCents: yearRev,
                YearNetCashDeltaCents: yearNet,
                YearLandownerMinutesUsed: current.UsedLandownerMinutesThisYear,
                YearLandownerMinutesRemainingBeforeReset: current.RemainingLandownerMinutes,
                AnnualLandownerAllowanceMinutesAfterReset: current.Options.AnnualLandownerAllowanceMinutes,
                HarvestPlansExecutedInYear: harvestPlansInYear,
                TotalPlansSettledInYear: settledInYear);

            return current with
            {
                UsedLandownerMinutesThisYear = 0,
                RemainingLandownerMinutes = current.Options.AnnualLandownerAllowanceMinutes,
                PendingOrderExecutors = cancelledWork
                    ? new Dictionary<EconomyOrderKey, WorkExecutor>()
                    : current.PendingOrderExecutors,
                ApprovedQuote = cancelledWork ? null : current.ApprovedQuote,
                PendingFellDispositions = cancelledWork ? FellingDispositionRules.EmptyChoices : current.PendingFellDispositions,
                YearEndSummaries = Array.AsReadOnly(current.YearEndSummaries.Append(summary).ToArray()),
            };
        }
    }
}
