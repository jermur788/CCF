namespace SitkaP1.Core.NaturalProcesses;

// Immutable request/result contracts for the Jay Acorn Dispersal, Nearby Oak Woodland, StormModel 1 adapter and salvage
// accounting engines (SPEC-multi-2d-jay-windstorms-V1, Part 1). Part 2 wires these into the annual pipeline; it must not
// need to change them. Coordinates are local metres: +X east, +Y north.

/// <summary>Stable refusal codes. Values are append-only so Part 2 can persist or display them.</summary>
public enum NaturalProcessRefusalCode
{
    None = 0,
    InvalidYear = 1,
    InvalidWoodlandBoundary = 2,
    DuplicateCandidateId = 3,
    NonOakSeedCandidate = 4,
    InvalidCandidate = 5,
    NearbyOakWoodlandRequiresJayDispersal = 6,
    InvalidOuterEdge = 7,
    UnknownStormModel = 8,
    StormYearAlreadyResolved = 9,
    DuplicateTreeId = 10,
    InvalidTreeInput = 11,
    StormSiteHazardNotNeutral = 12,
    StormExternalEdgeNotNeutral = 13,
    DuplicateDeadwoodId = 14,
    DeadwoodAlreadyExtracted = 15,
    UnknownDeadwood = 16,
    DuplicateSalvageOrder = 17,
    SalvageRequiresApprovedWorkPlan = 18,
    SalvageTargetNotWindthrow = 19,
    InvalidSalvageVolume = 20,
    InvalidSalvagePricing = 21,
    UnknownSalvageChoice = 22,
    /// <summary>An on-map candidate ID or source tree ID uses the reserved Nearby Oak Woodland namespace.</summary>
    ReservedCandidateNamespace = 23,
}

/// <summary>
/// The one way caller-supplied collections are copied (R3-01). Items are read by plain enumeration into a private
/// list, and only then is the final private array allocated. LINQ <c>ToArray()</c> and <c>new List&lt;T&gt;(source)</c>
/// are never used on caller input: both hand a newly allocated destination to the source's
/// <see cref="ICollection{T}.CopyTo"/>, and a hostile collection could keep that array and overwrite it later.
/// </summary>
internal static class SnapshotCopy
{
    /// <summary>A private array copy of <paramref name="source"/>; null gives an empty array.</summary>
    public static T[] ToArray<T>(IEnumerable<T>? source)
    {
        var items = new List<T>();
        if (source is null) return items.ToArray();
        foreach (T item in source) items.Add(item);
        return items.ToArray();
    }
}

/// <summary>
/// Defensive snapshots for every collection crossing the contract boundary. The caller's collection is copied into a
/// private array and exposed only through <see cref="SnapshotList{T}"/>, so mutating the original input, or casting a
/// returned list and writing to it, can never change a request or a result after construction.
/// </summary>
internal static class ContractSnapshot
{
    /// <summary>A read-only copy; null stays null so engines can still refuse a missing request list.</summary>
    public static IReadOnlyList<T> Of<T>(IEnumerable<T>? source) =>
        source is null ? null! : new SnapshotList<T>(source);
}

/// <summary>
/// An immutable list copy. It deliberately implements only <see cref="IReadOnlyList{T}"/>: not ICollection, IList or
/// their generic forms, so there is no <c>SyncRoot</c>, setter or Add/Clear that could reach the private array
/// (an array-backed <c>ReadOnlyCollection&lt;T&gt;</c> leaks its array through <c>ICollection.SyncRoot</c>).
/// Equality is by sequence, so records holding snapshots keep value equality.
/// </summary>
internal sealed class SnapshotList<T> : IReadOnlyList<T>, IEquatable<SnapshotList<T>>
{
    private readonly T[] _items;

    public SnapshotList(IEnumerable<T> source) => _items = SnapshotCopy.ToArray(source);

    public int Count => _items.Length;

    public T this[int index] => _items[index];

    public IEnumerator<T> GetEnumerator()
    {
        // A yield iterator rather than the array's own enumerator, so no array reference ever leaves this class.
        for (int i = 0; i < _items.Length; i++) yield return _items[i];
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(SnapshotList<T>? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (_items.Length != other._items.Length) return false;
        var comparer = EqualityComparer<T>.Default;
        for (int i = 0; i < _items.Length; i++)
            if (!comparer.Equals(_items[i], other._items[i])) return false;
        return true;
    }

    public override bool Equals(object? obj) => Equals(obj as SnapshotList<T>);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(_items.Length);
        var comparer = EqualityComparer<T>.Default;
        foreach (T item in _items) hash.Add(item, comparer);
        return hash.ToHashCode();
    }
}

/// <summary>A refusal: nothing was changed and no partial result is returned.</summary>
public sealed record NaturalProcessRefusal(NaturalProcessRefusalCode Code, string Detail);

/// <summary>A point in local woodland metres (+X east, +Y north).</summary>
public readonly record struct LocalPoint(double X, double Y);

/// <summary>The four outer edges of the woodland's axis-aligned extent.</summary>
public enum OuterEdge { North = 0, South = 1, East = 2, West = 3 }

// ---------------------------------------------------------------- Jay Acorn Dispersal / Nearby Oak Woodland

/// <summary>
/// World options. Both are OFF by default. NearbyOakWoodland adds an authored off-map oak source on one outer edge and
/// needs JayAcornDispersal ON, because jays are the only transport in this feature.
/// </summary>
public sealed record JayAcornDispersalSettings(
    bool JayAcornDispersal = false,
    bool NearbyOakWoodland = false,
    OuterEdge NearbyOakWoodlandEdge = OuterEdge.West)
{
    public static JayAcornDispersalSettings Default { get; } = new();
}

/// <summary>
/// The existing engine's oak mast outcome for this year, passed in by the caller. This feature never rolls its own mast.
/// <paramref name="EngineMastLabel"/> is the engine's label (for example "good") kept only for reporting.
/// </summary>
public sealed record OakMastYearOutcome(bool IsOakMastYear, string EngineMastLabel);

/// <summary>
/// One on-map oak seed candidate the existing engine already produced this year from a mature oak under its existing
/// maturity rules. Jay dispersal may divert some of these; it never creates extra on-map candidates.
/// </summary>
public sealed record OnMapOakSeedCandidate(string CandidateId, string SourceTreeId, string SpeciesId, LocalPoint SourcePosition);

public sealed record JayDispersalRequest(
    int Year,
    int Seed,
    JayAcornDispersalSettings Settings,
    OakMastYearOutcome OakMast,
    WoodlandFootprint Woodland,
    IReadOnlyList<OnMapOakSeedCandidate> OnMapOakCandidates)
{
    private readonly IReadOnlyList<OnMapOakSeedCandidate> _onMapOakCandidates = ContractSnapshot.Of(OnMapOakCandidates);
    public IReadOnlyList<OnMapOakSeedCandidate> OnMapOakCandidates { get => _onMapOakCandidates; init => _onMapOakCandidates = ContractSnapshot.Of(value); }
}

public enum JayCandidateOrigin { OnMapOakTree = 0, NearbyOakWoodland = 1 }

public enum JayCandidateFate { DeliveredInsideWoodland = 0, ExportedOutsideWoodland = 1 }

/// <summary>
/// One jay-carried acorn. On-map candidates keep their source tree ID; Nearby Oak Woodland candidates have no source tree
/// (<see cref="SourceTreeId"/> is null) and name the edge instead. A delivered acorn is only a candidate: the caller must
/// pass it through ordinary ground-suitability, light, bramble/bracken and deer-browsing evaluation.
/// </summary>
public sealed record JayAcornCandidate(
    string CandidateId,
    JayCandidateOrigin Origin,
    string? SourceTreeId,
    OuterEdge? SourceEdge,
    LocalPoint Source,
    double DistanceM,
    double BearingRadians,
    LocalPoint Destination,
    JayCandidateFate Fate);

public sealed record JayDispersalResult(
    NaturalProcessRefusal? Refusal,
    bool Ran,
    double WoodlandHectares,
    int OnMapCap,
    int OffMapCap,
    IReadOnlyList<string> DivertedOnMapCandidateIds,
    IReadOnlyList<JayAcornCandidate> Delivered,
    IReadOnlyList<JayAcornCandidate> Exported,
    int RngDrawCount)
{
    private readonly IReadOnlyList<string> _divertedOnMapCandidateIds = ContractSnapshot.Of(DivertedOnMapCandidateIds);
    private readonly IReadOnlyList<JayAcornCandidate> _delivered = ContractSnapshot.Of(Delivered);
    private readonly IReadOnlyList<JayAcornCandidate> _exported = ContractSnapshot.Of(Exported);
    public IReadOnlyList<string> DivertedOnMapCandidateIds { get => _divertedOnMapCandidateIds; init => _divertedOnMapCandidateIds = ContractSnapshot.Of(value); }
    public IReadOnlyList<JayAcornCandidate> Delivered { get => _delivered; init => _delivered = ContractSnapshot.Of(value); }
    public IReadOnlyList<JayAcornCandidate> Exported { get => _exported; init => _exported = ContractSnapshot.Of(value); }

    /// <summary>Every delivered candidate still faces ordinary establishment rules; jays grant no shortcut.</summary>
    public const bool RequiresOrdinaryEstablishmentEvaluation = true;
    public const double MicrositeBonus = 0.0;
    public const bool BrowseImmune = false;
    public const bool CreatesInstantSeedlings = false;
}

// ---------------------------------------------------------------- StormModel 1 adapter

/// <summary>StormModel 0 is storm-free and draws nothing. StormModel 1 is frozen D-050 WindthrowV1.</summary>
public enum StormModelVersion { StormFree = 0, WindthrowV1 = 1 }

/// <summary>Qualitative player feedback. Never a probability.</summary>
public enum StormExposureBand { Stable = 0, Watch = 1, Exposed = 2 }

/// <summary>
/// Caller-supplied state of one living tree at storm time. Lane is carried for reporting only: it never changes
/// occurrence, severity, direction or vulnerability. Under StormModel 1 site hazard must be 1 and edge facing 0 (D-050).
/// </summary>
public sealed record StormTreeInput(
    string TreeId,
    string LaneId,
    string CellId,
    float HeightM,
    float DbhCm,
    float LocalTopHeightM,
    float CellLight,
    float RecentOpening,
    float SiteHazard = 1f,
    float EdgeFacing = 0f);

public sealed record StormRequest(
    StormModelVersion Model,
    int Year,
    int Seed,
    IReadOnlyList<StormTreeInput> Trees,
    int? LastResolvedStormYear = null)
{
    private readonly IReadOnlyList<StormTreeInput> _trees = ContractSnapshot.Of(Trees);
    public IReadOnlyList<StormTreeInput> Trees { get => _trees; init => _trees = ContractSnapshot.Of(value); }
}

/// <summary>
/// The single woodland-wide storm of a year. <see cref="DirectionDegrees"/> is the bearing the storm blows towards
/// (0 north, 90 east; frozen D-050). <see cref="FallGlyphOrientationDegrees"/> equals <see cref="DirectionDegrees"/>;
/// a UI convention only, never a persisted per-tree fall bearing.
/// </summary>
public sealed record StormEvent(
    int Year,
    int SeverityLevel,
    float Severity,
    double DirectionDegrees,
    double FallGlyphOrientationDegrees);

public enum DeadwoodForm { Fallen = 0 }

/// <summary>Lying deadwood left by a windthrown tree (one per victim).</summary>
public sealed record FallenDeadwoodRecord(
    string DeadwoodId,
    string OriginTreeId,
    string MortalityCause,
    int FallenYear,
    DeadwoodForm Form,
    float HeightM,
    float DbhCm);

/// <summary>One canopy opening at the victim's cell; the caller adds it to the cell's recent opening (existing cap).</summary>
public sealed record CanopyOpening(string CellId, string CausedByTreeId, float RecentOpeningIncrement);

public sealed record WindthrowVictim(
    string TreeId,
    string LaneId,
    string MortalityCause,
    FallenDeadwoodRecord Deadwood,
    CanopyOpening Opening);

public sealed record TreeExposureFeedback(string TreeId, StormExposureBand Band, string Label);

public sealed record StormResult(
    NaturalProcessRefusal? Refusal,
    StormModelVersion Model,
    bool StormsOff,
    StormEvent? Event,
    IReadOnlyList<WindthrowVictim> Victims,
    IReadOnlyList<TreeExposureFeedback> Exposure,
    int RngDrawCount)
{
    private readonly IReadOnlyList<WindthrowVictim> _victims = ContractSnapshot.Of(Victims);
    private readonly IReadOnlyList<TreeExposureFeedback> _exposure = ContractSnapshot.Of(Exposure);
    public IReadOnlyList<WindthrowVictim> Victims { get => _victims; init => _victims = ContractSnapshot.Of(value); }
    public IReadOnlyList<TreeExposureFeedback> Exposure { get => _exposure; init => _exposure = ContractSnapshot.Of(value); }
}

// ---------------------------------------------------------------- Salvage accounting

public enum SalvageChoice { LeaveAsFallenDeadwood = 0, SellAndExtract = 1, KeepForUse = 2 }

/// <summary>
/// One salvage instruction. SellAndExtract and KeepForUse need an approved Work Plan reference. The recoverable volume
/// comes from the existing stem/yield calculators; this feature does not price or grade timber itself.
/// </summary>
public sealed record SalvageOrder(string DeadwoodId, SalvageChoice Choice, string? ApprovedWorkPlanId, double RecoverableVolumeM3);

/// <summary>
/// The existing price and contractor minimum, passed through unchanged (multiplier 1.00). Each must be between 0 and
/// <see cref="SalvageAccounting.MaxMoneyAmount"/>; anything else is refused as InvalidSalvagePricing.
/// </summary>
public sealed record SalvagePricing(decimal UnitSaleValuePerM3, decimal ContractorMinimum);

public sealed record SalvageRequest(
    IReadOnlyList<FallenDeadwoodRecord> Deadwood,
    IReadOnlyList<string> PreviouslyExtractedDeadwoodIds,
    IReadOnlyList<SalvageOrder> Orders,
    SalvagePricing Pricing)
{
    private readonly IReadOnlyList<FallenDeadwoodRecord> _deadwood = ContractSnapshot.Of(Deadwood);
    private readonly IReadOnlyList<string> _previouslyExtracted = ContractSnapshot.Of(PreviouslyExtractedDeadwoodIds);
    private readonly IReadOnlyList<SalvageOrder> _orders = ContractSnapshot.Of(Orders);
    public IReadOnlyList<FallenDeadwoodRecord> Deadwood { get => _deadwood; init => _deadwood = ContractSnapshot.Of(value); }
    public IReadOnlyList<string> PreviouslyExtractedDeadwoodIds { get => _previouslyExtracted; init => _previouslyExtracted = ContractSnapshot.Of(value); }
    public IReadOnlyList<SalvageOrder> Orders { get => _orders; init => _orders = ContractSnapshot.Of(value); }
}

public sealed record SalvageLine(
    string DeadwoodId,
    SalvageChoice Choice,
    string? ApprovedWorkPlanId,
    double VolumeM3,
    decimal SaleRevenue,
    double RetainedTimberM3);

public sealed record SalvageResult(
    NaturalProcessRefusal? Refusal,
    IReadOnlyList<FallenDeadwoodRecord> RemainingDeadwood,
    IReadOnlyList<SalvageLine> Lines,
    IReadOnlyList<string> ExtractedDeadwoodIds,
    double SoldVolumeM3,
    decimal GrossSaleRevenue,
    double RetainedTimberM3,
    SalvagePricing PricingApplied)
{
    private readonly IReadOnlyList<FallenDeadwoodRecord> _remaining = ContractSnapshot.Of(RemainingDeadwood);
    private readonly IReadOnlyList<SalvageLine> _lines = ContractSnapshot.Of(Lines);
    private readonly IReadOnlyList<string> _extracted = ContractSnapshot.Of(ExtractedDeadwoodIds);
    public IReadOnlyList<FallenDeadwoodRecord> RemainingDeadwood { get => _remaining; init => _remaining = ContractSnapshot.Of(value); }
    public IReadOnlyList<SalvageLine> Lines { get => _lines; init => _lines = ContractSnapshot.Of(value); }
    public IReadOnlyList<string> ExtractedDeadwoodIds { get => _extracted; init => _extracted = ContractSnapshot.Of(value); }
}

public sealed record DeadwoodMergeResult(NaturalProcessRefusal? Refusal, IReadOnlyList<FallenDeadwoodRecord> Deadwood)
{
    private readonly IReadOnlyList<FallenDeadwoodRecord> _deadwood = ContractSnapshot.Of(Deadwood);
    public IReadOnlyList<FallenDeadwoodRecord> Deadwood { get => _deadwood; init => _deadwood = ContractSnapshot.Of(value); }
}
