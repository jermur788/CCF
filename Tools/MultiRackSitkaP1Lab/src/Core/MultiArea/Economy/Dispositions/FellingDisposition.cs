namespace SitkaP1.Core;

// CCF Multi 2D — Phase 2B-2b ENGINE HALF: three-way felling dispositions.
// Pure .NET, integer money, no UI. Economy-enabled sessions only; when Economy is Off none of this state exists.

/// <summary>
/// What happens to a felled stem after the contractor fells it. Chosen per pending Fell order; the default (0) is
/// <see cref="SellAndExtract"/>, so every caller that never sets a disposition keeps the Phase 2B-1 behaviour exactly.
/// Every disposition is felled by the same grouped contractor job at the same stage rate and floor: felling is never free.
/// </summary>
internal enum FellingDisposition
{
    /// <summary>Extract and sell at the D-062 price book (Phase 2B-1 behaviour). Revenue only from these stems.</summary>
    SellAndExtract = 0,

    /// <summary>Extract and keep for the landowner's own use: no revenue; the engine-graded stem volume is retained timber.</summary>
    KeepForUse = 1,

    /// <summary>Leave the felled stem on site as fallen deadwood: no revenue, no retained timber, one deadwood intent per stem.</summary>
    LeaveAsDeadwood = 2,
}

/// <summary>
/// One explicit pending disposition choice. It belongs to the exact pending <see cref="MultiAreaWorkOrder"/> instance it
/// was set on (reference identity), so an order that is removed, cancelled with its approval, or cancelled at year end
/// and later re-added as a new order starts again at the default <see cref="FellingDisposition.SellAndExtract"/>.
/// </summary>
internal sealed record FellingDispositionChoice(MultiAreaWorkOrder Order, FellingDisposition Disposition);

/// <summary>Per-stem disposition outcome inside one grouped harvest job (quote and settlement).</summary>
internal sealed record FelledStemDispositionLine(
    string AreaId,
    string TreeId,
    string SpeciesId,
    FellingDisposition Disposition,
    long StemVolumeCm3,
    long HarvestedGreenGrams,
    long CreditedTimberRevenueCents,
    long RetainedTimberCm3,
    bool EmitsDeadwoodIntent);

/// <summary>Evidence labels for the Phase 2B-2b disposition rules (spec multi-2d-phase2b-economy-v1 §2 tiers).</summary>
internal static class FellingDispositionEvidence
{
    internal const string SameContractorCost =
        "[SIMULATION ABSTRACTION / GAMEPLAY CALIBRATION] Every disposition is felled and handled by the same grouped contractor job at the engine-derived stage rate and the same single €2,500 floor; no cheaper fell-only rate is modelled (that would be a future calibration decision, not a guess).";
    internal const string KeepForUse =
        "[SIMULATION ABSTRACTION] Keep-for-use timber earns no cash; its volume is the engine-graded D-062 stem volume in exact integer cm³ (no player override) held as a physical, non-cash balance.";
    internal const string LeaveAsDeadwood =
        "[MANAGEMENT GUIDANCE / SIMULATION ABSTRACTION] A felled stem left on site becomes one fallen-deadwood record using the same biological stem-volume form as natural deaths; no decay, habitat score or economic value is modelled here.";
    internal const string NoClearfell =
        "[MANAGEMENT GUIDANCE] Dispositions never relax CCF safeguards: felling stays contractor-only, a plan that would leave an area with no living stems is still refused, and no harvest-rate or per-stem volume override exists.";

    internal static readonly IReadOnlyList<string> All = Array.AsReadOnly(new[]
    {
        SameContractorCost,
        KeepForUse,
        LeaveAsDeadwood,
        NoClearfell,
    });
}

/// <summary>Pure helpers that resolve which disposition applies to each Fell order.</summary>
internal static class FellingDispositionRules
{
    internal static readonly IReadOnlyDictionary<EconomyOrderKey, FellingDispositionChoice> EmptyChoices =
        new Dictionary<EconomyOrderKey, FellingDispositionChoice>();

    internal static readonly IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> EmptyResolved =
        new SortedDictionary<EconomyOrderKey, FellingDisposition>();

    /// <summary>
    /// Resolves the disposition of every Fell order in <paramref name="orders"/>: an explicit choice applies only when it
    /// was made on that exact order instance; everything else is <see cref="FellingDisposition.SellAndExtract"/>.
    /// The result is sorted by <see cref="EconomyOrderKey"/> so it enumerates deterministically.
    /// </summary>
    internal static IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> Resolve(
        IReadOnlyDictionary<EconomyOrderKey, FellingDispositionChoice> choices,
        IReadOnlyList<MultiAreaWorkOrder> orders)
    {
        ArgumentNullException.ThrowIfNull(choices);
        ArgumentNullException.ThrowIfNull(orders);
        var resolved = new SortedDictionary<EconomyOrderKey, FellingDisposition>();
        foreach (var order in orders)
        {
            var key = EconomyOrderKey.ForFell(order.AreaId, order.TreeId);
            resolved[key] = choices.TryGetValue(key, out var choice) && ReferenceEquals(choice.Order, order)
                ? choice.Disposition
                : FellingDisposition.SellAndExtract;
        }
        return resolved;
    }

    /// <summary>Only the explicitly chosen, non-default dispositions that still belong to a current pending order.</summary>
    internal static IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> ExplicitFor(
        IReadOnlyDictionary<EconomyOrderKey, FellingDispositionChoice> choices,
        IReadOnlyList<MultiAreaWorkOrder> orders)
    {
        var result = new SortedDictionary<EconomyOrderKey, FellingDisposition>();
        foreach (var (key, disposition) in Resolve(choices, orders))
        {
            if (disposition != FellingDisposition.SellAndExtract)
                result[key] = disposition;
        }
        return result;
    }

    /// <summary>True when two resolved maps assign exactly the same disposition to exactly the same orders.</summary>
    internal static bool SameResolved(
        IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> a,
        IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> b)
    {
        if (a.Count != b.Count) return false;
        foreach (var (key, disposition) in a)
        {
            if (!b.TryGetValue(key, out var other) || other != disposition) return false;
        }
        return true;
    }

    /// <summary>Disposition of one order inside a resolved map (missing → default <see cref="FellingDisposition.SellAndExtract"/>).</summary>
    internal static FellingDisposition Of(IReadOnlyDictionary<EconomyOrderKey, FellingDisposition> resolved, EconomyOrderKey key) =>
        resolved.TryGetValue(key, out var disposition) ? disposition : FellingDisposition.SellAndExtract;
}
