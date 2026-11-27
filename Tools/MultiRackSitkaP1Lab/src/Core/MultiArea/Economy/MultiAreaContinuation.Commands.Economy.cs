namespace SitkaP1.Core;

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Revision-fenced (C-FENCE) read-only Economy snapshot built from ONE committed root under ONE lock acquisition.
    /// Refused when Economy is Off.
    /// </summary>
    internal MultiAreaEconomyView ReadEconomyView()
    {
        lock (sync)
        {
            var econ = RequireEconomy();
            return new MultiAreaEconomyView(
                sessionId,
                root.PlanStateRevision,
                Token(root),
                models,
                econ.Options,
                econ.CashBalanceCents,
                econ.UsedLandownerMinutesThisYear,
                econ.RemainingLandownerMinutes,
                econ.ExecutedHarvestPlansCount,
                new Dictionary<EconomyOrderKey, WorkExecutor>(econ.PendingOrderExecutors),
                econ.ApprovedQuote,
                econ.Settlements,
                econ.Ledger,
                econ.YearEndSummaries,
                EconomyEvidence.All)
            {
                PendingFellDispositions = FellingDispositionRules.ExplicitFor(econ.PendingFellDispositions, root.Pending),
                RetainedTimber = econ.RetainedTimber,
            };
        }
    }

    /// <summary>
    /// Pure, read-only Work Plan economic quote bound to <see cref="MultiAreaRoot.PlanStateRevision"/> (C-ECON).
    /// Acquires <c>sync</c> once, checks fence, and mutates zero state. Refused when Economy is Off.
    /// </summary>
    internal EconomyWorkPlanQuote QuoteWorkPlan(MultiAreaFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            RequireEconomy();
            return EconomyQuoteCalculator.ComputePending(root, Token(root));
        }
    }

    /// <summary>
    /// Pure read-only per-year economic history. When Economy is Off, returns <c>NOT RECORDED</c> for every money field
    /// (never <c>€0</c> or <c>0</c>) and never mutates state.
    /// </summary>
    internal EconomyYearHistory ReadEconomyHistoryYear(int year)
    {
        MultiAreaRoot snapshot;
        lock (sync) snapshot = root;
        return EconomyHistoryQuery.ReadYear(snapshot, year);
    }

    /// <summary>
    /// Assigns a <see cref="WorkExecutor"/> (<see cref="WorkExecutor.Contractor"/> or <see cref="WorkExecutor.Landowner"/>)
    /// to an existing pending Work Plan order. Refuses assigning <c>Fell</c> to <c>Landowner</c> with
    /// <c>EconomyFellContractorOnly</c> and refuses edits while an approved plan is awaiting execution.
    /// </summary>
    internal MultiAreaCommandResult SetOrderExecutor(MultiAreaFence fence, EconomyOrderKey orderKey, WorkExecutor executor)
    {
        lock (sync)
        {
            RequireFence(fence);
            var econ = RequireEconomy();
            if (!Enum.IsDefined(executor))
                throw new P1Refusal("EconomyExecutorUnknown", "unknown work executor");
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");

            if (orderKey.Kind == EconomyOrderKind.Fell && executor == WorkExecutor.Landowner)
                throw new P1Refusal("EconomyFellContractorOnly",
                    "commercial production felling is contractor-only; Fell cannot be assigned to Landowner");

            RequirePendingOrderExists(orderKey, executor);

            var nextMap = new Dictionary<EconomyOrderKey, WorkExecutor>(econ.PendingOrderExecutors)
            {
                [orderKey] = executor,
            };
            root = root with
            {
                Economy = econ with { PendingOrderExecutors = nextMap },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    /// <summary>
    /// Phase 2B-2b: chooses what happens to one pending Fell order's stem after the contractor fells it
    /// (<see cref="FellingDisposition.SellAndExtract"/> default, <see cref="FellingDisposition.KeepForUse"/> or
    /// <see cref="FellingDisposition.LeaveAsDeadwood"/>). The contractor cost is the same for all three. Refuses an
    /// undefined value (<c>EconomyDispositionUnknown</c>), any edit while a plan is approved (<c>WorkPlanAlreadyApproved</c>)
    /// and a tree with no pending Fell order (<c>UnknownWorkOrder</c>). Takes no volume, rate or price input.
    /// </summary>
    internal MultiAreaCommandResult SetFellingDisposition(MultiAreaFence fence, string areaId, string treeId, FellingDisposition disposition)
    {
        lock (sync)
        {
            RequireFence(fence);
            var econ = RequireEconomy();
            if (!Enum.IsDefined(disposition))
                throw new P1Refusal("EconomyDispositionUnknown", "unknown felling disposition");
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");
            var order = root.Pending.FirstOrDefault(o => string.Equals(o.AreaId, areaId, StringComparison.Ordinal) &&
                string.Equals(o.TreeId, treeId, StringComparison.Ordinal))
                ?? throw new P1Refusal("UnknownWorkOrder", "no pending Fell order matches that area and tree ID");

            // Keep only choices that still belong to a current pending order, then apply this one. The default is stored
            // as "no entry", so the map only ever holds explicit non-default choices.
            var key = EconomyOrderKey.ForFell(areaId, treeId);
            var next = new Dictionary<EconomyOrderKey, FellingDispositionChoice>();
            foreach (var (existingKey, choice) in econ.PendingFellDispositions)
            {
                if (root.Pending.Any(o => ReferenceEquals(o, choice.Order)) && existingKey != key)
                    next[existingKey] = choice;
            }
            if (disposition != FellingDisposition.SellAndExtract)
                next[key] = new FellingDispositionChoice(order, disposition);

            root = root with
            {
                Economy = econ with { PendingFellDispositions = next },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    /// <summary>Convenience fenced command for Fell executor assignment; always refuses <see cref="WorkExecutor.Landowner"/>.</summary>
    internal MultiAreaCommandResult SetFellExecutor(MultiAreaFence fence, string areaId, string treeId, WorkExecutor executor) =>
        SetOrderExecutor(fence, EconomyOrderKey.ForFell(areaId, treeId), executor);

    // Caller holds sync.
    private MultiAreaEconomyState RequireEconomy() => root.Economy ??
        throw new P1Refusal("EconomyModelRequired",
            "Economy reads, quotes, and labour assignments need EconomyModelVersion.Enabled; this session has Economy Off");

    // Caller holds sync.
    private void RequirePendingOrderExists(EconomyOrderKey key, WorkExecutor executor)
    {
        switch (key.Kind)
        {
            case EconomyOrderKind.Fell:
                if (!root.Pending.Any(o => string.Equals(o.AreaId, key.AreaId, StringComparison.Ordinal) &&
                    string.Equals(o.TreeId, key.TargetKey, StringComparison.Ordinal)))
                    throw new P1Refusal("UnknownWorkOrder", "no pending Fell order matches that area and tree ID");
                break;
            case EconomyOrderKind.Prune:
                var prune = root.Stage4?.PendingPrunes.FirstOrDefault(o =>
                    string.Equals(o.AreaId, key.AreaId, StringComparison.Ordinal) &&
                    string.Equals(o.TreeId, key.TargetKey, StringComparison.Ordinal))
                    ?? throw new P1Refusal("UnknownWorkOrder", "no pending Prune order matches that area and tree ID");
                if (executor == WorkExecutor.Landowner && !EconomyDefaults.IsLandownerEligible(EconomyOrderKind.Prune, prune.TargetCrownBaseHeightM))
                    throw new P1Refusal("EconomyLandownerTaskIneligible",
                        "pruning above the low-risk crown-base height limit is contractor-only");
                break;
            case EconomyOrderKind.ClearUndergrowth:
                bool clearExists = root.Stage5?.PendingClears.Any(o =>
                    string.Equals(o.AreaId, key.AreaId, StringComparison.Ordinal) &&
                    string.Equals(FormattableString.Invariant($"{o.Column},{o.Row}"), key.TargetKey, StringComparison.Ordinal)) ?? false;
                if (!clearExists)
                    throw new P1Refusal("UnknownWorkOrder", "no pending ClearUndergrowth order matches that cell");
                break;
            case EconomyOrderKind.Plant:
            case EconomyOrderKind.Shelter:
            case EconomyOrderKind.SpotClear:
                bool plantExists = root.Phase2?.PendingOrders.Any(o => EconomyOrderKey.ForPlanting(o) == key) ?? false;
                if (!plantExists)
                    throw new P1Refusal("UnknownWorkOrder", "no pending planting order matches that order key");
                break;
            default:
                throw new P1Refusal("EconomyOrderUnknown", "unknown work order kind");
        }
    }

    // Caller holds sync. Removes any pending executor override, and (Phase 2B-2b) any felling disposition, for a removed
    // pending order.
    private static MultiAreaEconomyState? WithoutPendingExecutor(MultiAreaEconomyState? econ, EconomyOrderKey key)
    {
        if (econ is null) return econ;
        if (econ.PendingFellDispositions.ContainsKey(key))
        {
            var nextDispositions = new Dictionary<EconomyOrderKey, FellingDispositionChoice>(econ.PendingFellDispositions);
            nextDispositions.Remove(key);
            econ = econ with { PendingFellDispositions = nextDispositions };
        }
        if (!econ.PendingOrderExecutors.ContainsKey(key)) return econ;
        var next = new Dictionary<EconomyOrderKey, WorkExecutor>(econ.PendingOrderExecutors);
        next.Remove(key);
        return econ with { PendingOrderExecutors = next };
    }
}
