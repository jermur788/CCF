namespace SitkaP1.Core;

// Retained (keep-for-use) timber: a PHYSICAL, non-cash balance. It never produces a cash delta and never a ledger line.

/// <summary>One kept stem: its engine-graded D-062 stem volume in exact integer cubic centimetres.</summary>
internal sealed record RetainedTimberStem(
    string AreaId,
    string TreeId,
    string SpeciesId,
    long StemVolumeCm3,
    int EcologicalYear,
    int WorkRevision,
    string ExecutionKey,
    string PlanId)
{
    /// <summary>Exact cubic metres (cm³ / 1,000,000 in decimal; no floating point).</summary>
    public decimal StemVolumeM3 => RetainedTimber.ToM3(StemVolumeCm3);
}

/// <summary>Retained timber of one species, in both exact units.</summary>
internal sealed record RetainedTimberAmount(string SpeciesId, long RetainedTimberCm3)
{
    public decimal RetainedTimberM3 => RetainedTimber.ToM3(RetainedTimberCm3);
}

/// <summary>
/// A retained-timber tally: a settlement's delta, a quote's expected delta, or the session's cumulative balance.
/// <see cref="RetainedTimberCm3"/> is the exact sum of the listed stems; <see cref="BySpecies"/> is keyed by species id
/// (ordinal order). Built only by the engine from graded stems — there is no constructor path that accepts a volume
/// from a player command. <see cref="BySpecies"/> is published as a ReadOnlyDictionary over a privately owned ordinal
/// SortedDictionary, so the authoritative balance cannot be changed through a cast to IDictionary.
/// </summary>
internal sealed record RetainedTimberTally(
    long RetainedTimberCm3,
    IReadOnlyDictionary<string, RetainedTimberAmount> BySpecies,
    IReadOnlyList<RetainedTimberStem> Stems)
{
    public decimal RetainedTimberM3 => RetainedTimber.ToM3(RetainedTimberCm3);

    public static readonly RetainedTimberTally Empty = new(
        0L,
        new System.Collections.ObjectModel.ReadOnlyDictionary<string, RetainedTimberAmount>(
            new SortedDictionary<string, RetainedTimberAmount>(StringComparer.Ordinal)),
        Array.Empty<RetainedTimberStem>());

    /// <summary>Exact tally of <paramref name="stems"/> in the given order.</summary>
    internal static RetainedTimberTally FromStems(IEnumerable<RetainedTimberStem> stems)
    {
        ArgumentNullException.ThrowIfNull(stems);
        var list = stems.ToArray();
        if (list.Length == 0) return Empty;
        var bySpecies = new SortedDictionary<string, long>(StringComparer.Ordinal);
        long total = 0L;
        checked
        {
            foreach (var stem in list)
            {
                if (stem.StemVolumeCm3 < 0L)
                    throw new InvalidOperationException("Retained timber stem volume cannot be negative.");
                total += stem.StemVolumeCm3;
                bySpecies[stem.SpeciesId] = bySpecies.TryGetValue(stem.SpeciesId, out var cur) ? cur + stem.StemVolumeCm3 : stem.StemVolumeCm3;
            }
        }
        var amounts = new SortedDictionary<string, RetainedTimberAmount>(StringComparer.Ordinal);
        foreach (var (species, cm3) in bySpecies)
            amounts[species] = new RetainedTimberAmount(species, cm3);
        return new RetainedTimberTally(total,
            new System.Collections.ObjectModel.ReadOnlyDictionary<string, RetainedTimberAmount>(amounts), Array.AsReadOnly(list));
    }

    /// <summary>This tally followed by <paramref name="delta"/> (cumulative balance update).</summary>
    internal RetainedTimberTally Plus(RetainedTimberTally delta)
    {
        ArgumentNullException.ThrowIfNull(delta);
        return delta.Stems.Count == 0 ? this : FromStems(Stems.Concat(delta.Stems));
    }

    /// <summary>Same stems with the committed execution identity stamped on each (like ledger entries at settlement).</summary>
    internal RetainedTimberTally StampedWith(int ecologicalYear, int workRevision, string executionKey, string planId) =>
        Stems.Count == 0 ? this : FromStems(Stems.Select(s => s with
        {
            EcologicalYear = ecologicalYear,
            WorkRevision = workRevision,
            ExecutionKey = executionKey,
            PlanId = planId,
        }));
}

internal static class RetainedTimber
{
    internal const long Cm3PerM3 = 1_000_000L;

    /// <summary>Exact conversion: decimal division by 10^6 is exact for every long cm³ value.</summary>
    internal static decimal ToM3(long cm3) => cm3 / (decimal)Cm3PerM3;
}
