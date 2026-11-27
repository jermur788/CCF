namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// Salvage accounting for windthrow deadwood. The default for every record is LeaveAsFallenDeadwood: a record with no
/// order stays as deadwood, so nothing is ever salvaged automatically. SellAndExtract and KeepForUse need an approved
/// Work Plan reference. Only SellAndExtract earns revenue; KeepForUse yields retained timber and no revenue. Prices and
/// the contractor minimum are passed through unchanged (multiplier 1.00); the contractor minimum itself is applied by the
/// existing grouped harvest settlement in Part 2. Any refusal changes nothing (all-or-nothing).
/// </summary>
public static class SalvageAccounting
{
    public const decimal PriceMultiplier = 1.00m;

    /// <summary>
    /// Largest accepted unit price or contractor minimum. With volumes capped at 1,000 m³ per order, one line is at most
    /// 1e12 and even int.MaxValue orders total under 2.2e21, far below decimal's 7.9e28, so revenue can never overflow.
    /// A representability bound only, not a price calibration.
    /// </summary>
    public const decimal MaxMoneyAmount = 1_000_000_000m;

    public const double MaxRecoverableVolumeM3 = 1_000.0;

    public static SalvageResult Apply(SalvageRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        IReadOnlyList<FallenDeadwoodRecord> deadwood = request.Deadwood ?? Array.Empty<FallenDeadwoodRecord>();
        var extracted = new HashSet<string>(request.PreviouslyExtractedDeadwoodIds ?? Array.Empty<string>(), StringComparer.Ordinal);
        IReadOnlyList<SalvageOrder> orders = request.Orders ?? Array.Empty<SalvageOrder>();

        NaturalProcessRefusal? refusal = Validate(deadwood, extracted, orders, request.Pricing);
        if (refusal is not null) return Refused(request, deadwood, extracted, refusal);

        var orderById = orders.ToDictionary(o => o.DeadwoodId, StringComparer.Ordinal);
        var remaining = new List<FallenDeadwoodRecord>();
        var lines = new List<SalvageLine>();
        double sold = 0.0, retained = 0.0;
        decimal revenue = 0m;

        foreach (FallenDeadwoodRecord record in deadwood.OrderBy(d => d.DeadwoodId, StringComparer.Ordinal))
        {
            if (!orderById.TryGetValue(record.DeadwoodId, out SalvageOrder? order) || order.Choice == SalvageChoice.LeaveAsFallenDeadwood)
            {
                remaining.Add(record);
                if (order is not null)
                    lines.Add(new SalvageLine(record.DeadwoodId, order.Choice, order.ApprovedWorkPlanId, 0.0, 0m, 0.0));
                continue;
            }

            extracted.Add(record.DeadwoodId);
            if (order.Choice == SalvageChoice.SellAndExtract)
            {
                decimal lineRevenue = Math.Round((decimal)order.RecoverableVolumeM3 * request.Pricing.UnitSaleValuePerM3 * PriceMultiplier,
                    2, MidpointRounding.AwayFromZero);
                sold += order.RecoverableVolumeM3;
                revenue += lineRevenue;
                lines.Add(new SalvageLine(record.DeadwoodId, order.Choice, order.ApprovedWorkPlanId, order.RecoverableVolumeM3, lineRevenue, 0.0));
            }
            else
            {
                retained += order.RecoverableVolumeM3;
                lines.Add(new SalvageLine(record.DeadwoodId, order.Choice, order.ApprovedWorkPlanId, order.RecoverableVolumeM3, 0m, order.RecoverableVolumeM3));
            }
        }

        return new SalvageResult(null, remaining, lines, Sorted(extracted), sold, revenue, retained, request.Pricing);
    }

    /// <summary>
    /// Adds new windthrow deadwood to an existing stock. Refuses an ID already present or already extracted (in the existing
    /// stock or in the additions), so a salvaged stem can never reappear as deadwood and no victim is counted twice.
    /// A refusal returns the existing stock unchanged.
    /// </summary>
    public static DeadwoodMergeResult MergeWindthrowDeadwood(
        IReadOnlyList<FallenDeadwoodRecord> existing,
        IReadOnlyList<string> extractedDeadwoodIds,
        IReadOnlyList<FallenDeadwoodRecord> added)
    {
        var extracted = new HashSet<string>(extractedDeadwoodIds ?? Array.Empty<string>(), StringComparer.Ordinal);
        var present = new HashSet<string>(StringComparer.Ordinal);
        foreach (FallenDeadwoodRecord d in existing ?? Array.Empty<FallenDeadwoodRecord>())
        {
            if (extracted.Contains(d.DeadwoodId))
                return MergeRefused(existing, NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, d.DeadwoodId);
            if (!present.Add(d.DeadwoodId))
                return MergeRefused(existing, NaturalProcessRefusalCode.DuplicateDeadwoodId, d.DeadwoodId);
        }
        foreach (FallenDeadwoodRecord d in added ?? Array.Empty<FallenDeadwoodRecord>())
        {
            if (extracted.Contains(d.DeadwoodId))
                return MergeRefused(existing, NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, d.DeadwoodId);
            if (!present.Add(d.DeadwoodId))
                return MergeRefused(existing, NaturalProcessRefusalCode.DuplicateDeadwoodId, d.DeadwoodId);
        }
        var merged = (existing ?? Array.Empty<FallenDeadwoodRecord>()).Concat(added ?? Array.Empty<FallenDeadwoodRecord>())
            .OrderBy(d => d.DeadwoodId, StringComparer.Ordinal).ToArray();
        return new DeadwoodMergeResult(null, merged);
    }

    private static NaturalProcessRefusal? Validate(
        IReadOnlyList<FallenDeadwoodRecord> deadwood, HashSet<string> extracted, IReadOnlyList<SalvageOrder> orders, SalvagePricing? pricing)
    {
        if (pricing is null || pricing.UnitSaleValuePerM3 < 0m || pricing.ContractorMinimum < 0m ||
            pricing.UnitSaleValuePerM3 > MaxMoneyAmount || pricing.ContractorMinimum > MaxMoneyAmount)
            return new(NaturalProcessRefusalCode.InvalidSalvagePricing, NaturalProcessText.RefusalPricingOutOfRange);

        var byId = new Dictionary<string, FallenDeadwoodRecord>(StringComparer.Ordinal);
        foreach (FallenDeadwoodRecord d in deadwood)
        {
            if (d is null || string.IsNullOrWhiteSpace(d.DeadwoodId))
                return new(NaturalProcessRefusalCode.UnknownDeadwood, "deadwood record without an ID");
            if (!byId.TryAdd(d.DeadwoodId, d))
                return new(NaturalProcessRefusalCode.DuplicateDeadwoodId, "duplicate deadwood ID " + d.DeadwoodId);
            if (extracted.Contains(d.DeadwoodId))
                return new(NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, "deadwood already extracted " + d.DeadwoodId);
        }

        var ordered = new HashSet<string>(StringComparer.Ordinal);
        foreach (SalvageOrder o in orders)
        {
            if (o is null || string.IsNullOrWhiteSpace(o.DeadwoodId))
                return new(NaturalProcessRefusalCode.UnknownDeadwood, "salvage order without a deadwood ID");
            if (!Enum.IsDefined(o.Choice))
                return new(NaturalProcessRefusalCode.UnknownSalvageChoice, "unknown salvage choice for " + o.DeadwoodId);
            if (!ordered.Add(o.DeadwoodId))
                return new(NaturalProcessRefusalCode.DuplicateSalvageOrder, "more than one order for " + o.DeadwoodId);
            if (extracted.Contains(o.DeadwoodId))
                return new(NaturalProcessRefusalCode.DeadwoodAlreadyExtracted, NaturalProcessText.RefusalAlreadyExtracted);
            if (!byId.TryGetValue(o.DeadwoodId, out FallenDeadwoodRecord? target))
                return new(NaturalProcessRefusalCode.UnknownDeadwood, "no deadwood record " + o.DeadwoodId);
            if (o.Choice == SalvageChoice.LeaveAsFallenDeadwood) continue;
            if (!string.Equals(target.MortalityCause, StormAdapter.WindthrowCause, StringComparison.Ordinal))
                return new(NaturalProcessRefusalCode.SalvageTargetNotWindthrow, "salvage is only for windthrow deadwood: " + o.DeadwoodId);
            if (string.IsNullOrWhiteSpace(o.ApprovedWorkPlanId))
                return new(NaturalProcessRefusalCode.SalvageRequiresApprovedWorkPlan, NaturalProcessText.RefusalNeedsWorkPlan);
            if (!double.IsFinite(o.RecoverableVolumeM3) || o.RecoverableVolumeM3 <= 0.0 || o.RecoverableVolumeM3 > MaxRecoverableVolumeM3)
                return new(NaturalProcessRefusalCode.InvalidSalvageVolume, "recoverable volume must be finite and positive: " + o.DeadwoodId);
        }
        return null;
    }

    private static SalvageResult Refused(SalvageRequest request, IReadOnlyList<FallenDeadwoodRecord> deadwood, HashSet<string> extracted,
        NaturalProcessRefusal refusal) =>
        new(refusal, deadwood.Where(d => d is not null).OrderBy(d => d.DeadwoodId, StringComparer.Ordinal).ToArray(),
            Array.Empty<SalvageLine>(), Sorted(extracted), 0.0, 0m, 0.0, request.Pricing);

    private static DeadwoodMergeResult MergeRefused(IReadOnlyList<FallenDeadwoodRecord>? existing, NaturalProcessRefusalCode code, string id) =>
        new(new NaturalProcessRefusal(code, "deadwood " + id), (existing ?? Array.Empty<FallenDeadwoodRecord>()).ToArray());

    private static string[] Sorted(IEnumerable<string> ids) => ids.OrderBy(id => id, StringComparer.Ordinal).ToArray();
}
