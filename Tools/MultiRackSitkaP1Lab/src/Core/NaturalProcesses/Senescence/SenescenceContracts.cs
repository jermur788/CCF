using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.NaturalProcesses.Senescence;

/// <summary>
/// Version gate for natural old-age mortality (SPEC-multi-2d-senescence-late-growth-v1).
/// Default is <see cref="Disabled"/> (0), which makes zero RNG draws and performs no species or age evaluation so all
/// nine frozen baselines and legacy saves remain byte-identical.
/// </summary>
public enum SenescenceModelVersion
{
    Disabled = 0,
    OldAgeV1 = 1,
}

/// <summary>
/// Versioned stream/salt constants owned by this isolated module in Part 1.
/// Strictly prefix-free against all existing RngSalts and NaturalProcessStreams families; Part 2 registers this prefix
/// in RngSalts.cs without altering its value.
/// </summary>
public static class SenescenceStreams
{
    public const string OldAgeTreePrefix = "SENESCENCE-OLD-AGE-v1-";

    /// <summary>
    /// Builds the deterministic injective key for one tree's annual old-age roll from its species ID and tree ID.
    /// Only the six supported species IDs (the <see cref="SpeciesMoistureTable"/> constants) are accepted. None of them
    /// contains '/', so the first '/' after the prefix always ends the species ID and
    /// <c>prefix + speciesId + "/" + treeId</c> uniquely identifies the (species, tree) pair, even when a tree ID itself
    /// contains '/'.
    /// </summary>
    public static string KeyFor(string speciesId, string treeId)
    {
        if (!SenescenceHazard.IsSupportedSpecies(speciesId))
            throw new ArgumentException($"Unknown senescence species id '{speciesId}'.", nameof(speciesId));
        if (string.IsNullOrWhiteSpace(treeId))
            throw new ArgumentException("Tree ID must be provided.", nameof(treeId));

        return OldAgeTreePrefix + speciesId + "/" + treeId;
    }
}

/// <summary>
/// Machine-readable cause identifiers and plain natural forestry player-facing text.
/// Player-facing text intentionally contains no evidence tags, calibration labels, or academic disclaimers.
/// </summary>
public static class SenescenceText
{
    public const string CauseOldAge = "old-age";
    public const string PlayerCauseOldAge = "Died of old age";
}

/// <summary>
/// Read-only per-species old-age mortality hazard profile so tests and Part 2 can inspect parameters without duplicating
/// numeric literals. All numeric parameters are gameplay calibration ([C]), not fitted Irish mortality curves.
/// </summary>
public sealed record SenescenceSpeciesProfile(
    string SpeciesId,
    int OnsetAgeYears,
    int RampYears,
    double MaximumAnnualHazard,
    EvidenceTag Evidence);

/// <summary>One living tree submitted for annual old-age mortality evaluation.</summary>
public sealed record SenescenceTreeInput(string TreeId, string SpeciesId, int AgeYears);

/// <summary>
/// Request for one annual senescence evaluation. Caller collections are defensively snapshotted by plain enumeration so
/// later mutation of the caller's collection cannot alter the request.
/// </summary>
public sealed record SenescenceRequest(
    int Year,
    int Seed,
    SenescenceModelVersion Model,
    IReadOnlyList<SenescenceTreeInput> Trees)
{
    private readonly IReadOnlyList<SenescenceTreeInput> _trees = SenescenceSnapshot.Of(Trees);

    /// <summary>Convenience constructor defaulting <see cref="Model"/> to <see cref="SenescenceModelVersion.Disabled"/>.</summary>
    public SenescenceRequest(int year, int seed, IReadOnlyList<SenescenceTreeInput> trees)
        : this(year, seed, SenescenceModelVersion.Disabled, trees)
    {
    }

    public IReadOnlyList<SenescenceTreeInput> Trees
    {
        get => _trees;
        init => _trees = SenescenceSnapshot.Of(value);
    }
}

/// <summary>Computed annual old-age hazard for one evaluated tree.</summary>
public sealed record SenescenceHazardEvaluation(
    string TreeId,
    string SpeciesId,
    int AgeYears,
    double AnnualHazard,
    EvidenceTag Evidence);

/// <summary>
/// One biological old-age death emitted by <see cref="SenescenceEngine"/>.
/// Cleanly separate from <c>Fell()</c>: awards no timber revenue, clears crop-tree marks, never counts as a fell, and
/// carries plain natural forestry text ready for Part 2 deadwood and UI pathways.
/// The cause, the player wording and the four flags are fixed by the type rather than passed in, so no caller can
/// build an old-age death record with any other cause or semantics.
/// </summary>
public sealed record SenescenceDeathRecord(
    string TreeId,
    string SpeciesId,
    int Year)
{
    public string Cause => SenescenceText.CauseOldAge;
    public string PlayerCauseText => SenescenceText.PlayerCauseOldAge;
    public bool BiologicalDeath => true;
    public decimal TimberRevenueEur => 0m;
    public bool ClearsCropTreeMarks => true;
    public bool CountsAsFell => false;
}

/// <summary>Immutable result of one annual senescence evaluation.</summary>
public sealed record SenescenceResult(
    SenescenceModelVersion Model,
    IReadOnlyList<SenescenceDeathRecord> Deaths,
    int RngDrawCount,
    int TreesEvaluated,
    IReadOnlyList<SenescenceHazardEvaluation> HazardEvaluations)
{
    private readonly IReadOnlyList<SenescenceDeathRecord> _deaths = SenescenceSnapshot.Of(Deaths);
    private readonly IReadOnlyList<SenescenceHazardEvaluation> _hazardEvaluations = SenescenceSnapshot.Of(HazardEvaluations);

    public IReadOnlyList<SenescenceDeathRecord> Deaths
    {
        get => _deaths;
        init => _deaths = SenescenceSnapshot.Of(value);
    }

    public IReadOnlyList<SenescenceHazardEvaluation> HazardEvaluations
    {
        get => _hazardEvaluations;
        init => _hazardEvaluations = SenescenceSnapshot.Of(value);
    }
}

/// <summary>
/// Defensive snapshots for collections crossing the contract boundary. Copies via plain <c>foreach</c> enumeration into
/// a private array so hostile <see cref="ICollection{T}.CopyTo"/> implementations cannot retain or mutate the backing
/// storage.
/// </summary>
internal static class SenescenceSnapshot
{
    public static IReadOnlyList<T> Of<T>(IEnumerable<T>? source) =>
        source is null ? null! : new SenescenceSnapshotList<T>(source);

    public static T[] ToArray<T>(IEnumerable<T>? source)
    {
        var items = new List<T>();
        if (source is null)
            return items.ToArray();
        foreach (T item in source)
            items.Add(item);
        return items.ToArray();
    }
}

/// <summary>Immutable read-only list copy implementing only <see cref="IReadOnlyList{T}"/> with sequence equality.</summary>
internal sealed class SenescenceSnapshotList<T> : IReadOnlyList<T>, IEquatable<SenescenceSnapshotList<T>>
{
    private readonly T[] _items;

    public SenescenceSnapshotList(IEnumerable<T> source) => _items = SenescenceSnapshot.ToArray(source);

    public int Count => _items.Length;

    public T this[int index] => _items[index];

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < _items.Length; i++)
            yield return _items[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(SenescenceSnapshotList<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_items.Length != other._items.Length) return false;
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _items.Length; i++)
        {
            if (!comparer.Equals(_items[i], other._items[i]))
                return false;
        }
        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as SenescenceSnapshotList<T>);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(_items.Length);
        var comparer = EqualityComparer<T>.Default;
        foreach (T item in _items)
            hash.Add(item, comparer);
        return hash.ToHashCode();
    }
}
