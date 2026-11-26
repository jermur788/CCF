using System.Collections.ObjectModel;

namespace SitkaP1.Core.WoodlandImport;

/// <summary>
/// Provenance of a field on an imported tree or stand record.
/// Year-0 simulated values are never permitted on import and are refused by validation.
/// </summary>
public enum FieldProvenance
{
    Missing = 0,
    Observed = 1,
    Inferred = 2,
    Simulated = 3,
}

/// <summary>
/// Authoritative readiness outcome for an imported woodland stand (Option A policy).
/// </summary>
public enum WoodlandReadiness
{
    /// <summary>Structurally untrustworthy input (malformed archive, cap exceeded, XXE, out-of-bounds, Year-0 Simulated, etc.).</summary>
    Refused = 0,

    /// <summary>Structurally sound inventory retained in full for inspection, but blocked from simulation admission.</summary>
    InventoryOnly = 1,

    /// <summary>Structurally valid stand with surveyed boundary, supported living species, and required simulation measurements observed.</summary>
    CandidateForSimulationIntegration = 2,
}

/// <summary>Severity classification of a deterministic import validation issue.</summary>
public enum IssueSeverity
{
    Info = 0,
    Warning = 1,
    InventoryOnlyBlocker = 2,
    Refusal = 3,
}

/// <summary>Vital status of an imported tree. Unknown is counted conservatively as living for caps and admission checks.</summary>
public enum TreeVitalStatus
{
    Unknown = 0,
    Alive = 1,
    Dead = 2,
}

/// <summary>Mapping classification for an imported species name against CCF simulator support.</summary>
public enum SpeciesSupportCategory
{
    ModelledAtBase = 0,
    PendingModelSupport = 1,
    UnmodelledSpecies = 2,
    UnidentifiedSpecies = 3,
}

/// <summary>2D local Cartesian point in metres (+X east, +Y north).</summary>
public readonly record struct LocalPoint2D(double XM, double YM)
{
    public bool IsFinite => double.IsFinite(XM) && double.IsFinite(YM);
}

/// <summary>2D geographic coordinate in WGS-84 decimal degrees.</summary>
public readonly record struct GeoPoint2D(double LatitudeDeg, double LongitudeDeg)
{
    public bool IsFinite => double.IsFinite(LatitudeDeg) && double.IsFinite(LongitudeDeg);
}

/// <summary>Traceable raw source evidence for a single imported field.</summary>
public sealed record FieldEvidence(
    string SourceColumn,
    string? RawValue,
    string? Note = null);

/// <summary>Typed imported field paired with explicit provenance and raw source evidence.</summary>
public sealed record ImportField<T>
{
    public FieldProvenance Provenance { get; init; }
    public bool HasValue { get; init; }
    public T? Value { get; init; }
    public FieldEvidence? Evidence { get; init; }

    public bool IsMissing => Provenance == FieldProvenance.Missing || !HasValue;

    public static ImportField<T> Missing(FieldEvidence? evidence = null) => new()
    {
        Provenance = FieldProvenance.Missing,
        HasValue = false,
        Value = default,
        Evidence = evidence,
    };

    public static ImportField<T> Observed(T value, FieldEvidence? evidence = null) => new()
    {
        Provenance = FieldProvenance.Observed,
        HasValue = true,
        Value = value,
        Evidence = evidence,
    };

    public static ImportField<T> Inferred(T value, FieldEvidence? evidence = null) => new()
    {
        Provenance = FieldProvenance.Inferred,
        HasValue = true,
        Value = value,
        Evidence = evidence,
    };

    public static ImportField<T> Simulated(T value, FieldEvidence? evidence = null) => new()
    {
        Provenance = FieldProvenance.Simulated,
        HasValue = true,
        Value = value,
        Evidence = evidence,
    };
}

/// <summary>Deterministic validation diagnostic attached to an import result.</summary>
public sealed record ImportValidationIssue(
    string Code,
    IssueSeverity Severity,
    string Message,
    string? SourceTreeId = null,
    string? FieldName = null);

/// <summary>Source attribution and licence metadata carried alongside an imported woodland stand.</summary>
public sealed record ImportSourceAttribution
{
    public required string DatasetTitle { get; init; }
    public required string Publisher { get; init; }
    public required string Licence { get; init; }
    public required string Citation { get; init; }
    public string? Doi { get; init; }
    public string? DatasetKey { get; init; }
    public string? ArchiveSha256 { get; init; }
    public string? RetrievalDateUtc { get; init; }
    public string? SourcePlotId { get; init; }
}

/// <summary>Deterministic species mapping result for a single raw species string.</summary>
public sealed record SpeciesMappingResult(
    string? CanonicalSpeciesId,
    SpeciesSupportCategory Category,
    string RawName,
    string NormalizedInput,
    string? MatchedScientificName);

/// <summary>Deterministic translation origin applied by <see cref="LocalCoordinateNormalizer"/>.</summary>
public sealed record LocalCoordinateOrigin(
    double OriginXM,
    double OriginYM,
    string DerivationRule);

/// <summary>Single measured or observed tree retained verbatim in an imported woodland inventory.</summary>
public sealed record ImportedTreeRecord
{
    public static readonly IReadOnlyDictionary<string, string> EmptyAttributes =
        new ReadOnlyDictionary<string, string>(new Dictionary<string, string>(StringComparer.Ordinal));

    public required string SourceTreeId { get; init; }
    public required ImportField<string> RawSpeciesName { get; init; }
    public required SpeciesMappingResult SpeciesMapping { get; init; }
    public required ImportField<TreeVitalStatus> VitalStatus { get; init; }
    public required ImportField<LocalPoint2D> PositionLocalM { get; init; }
    public required ImportField<double> DbhCm { get; init; }
    public required ImportField<double> HeightM { get; init; }
    public required ImportField<double> CrownRadiusM { get; init; }
    public required ImportField<int> AgeYears { get; init; }
    public IReadOnlyDictionary<string, string> ExtraAttributes { get; init; } = EmptyAttributes;

    /// <summary>
    /// Unknown or missing vital status is counted conservatively as living for cap enforcement and admission gating.
    /// </summary>
    public bool IsConservativelyLiving =>
        VitalStatus.IsMissing || VitalStatus.Value != TreeVitalStatus.Dead;
}

/// <summary>Immutable v1 woodland import stand snapshot.</summary>
public sealed record WoodlandImportStand
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;
    public required string StandId { get; init; }
    public required string PlotName { get; init; }
    public string? CountryCode { get; init; }
    public required ImportSourceAttribution Attribution { get; init; }
    public WoodlandBoundary? Boundary { get; init; }
    public LocalCoordinateOrigin? CoordinateOrigin { get; init; }
    public required IReadOnlyList<ImportedTreeRecord> Trees { get; init; }
    public int TotalArchiveRowsScanned { get; init; }
    public int TotalMeasurementRowsScanned { get; init; }
    public long ArchiveUncompressedBytes { get; init; }
    public int LivingStemCount { get; init; }
    public int DeadStemCount { get; init; }
    public int UnknownStatusStemCount { get; init; }
}

/// <summary>Deterministic bounds and policy options for woodland import.</summary>
public sealed record WoodlandImportOptions
{
    public const int DefaultMaxLivingStems = 3_000;
    public const int DefaultMaxTotalRecords = 20_000;
    public const int DefaultMaxMeasurementRows = 250_000;
    public const long DefaultMaxUncompressedBytes = 67_108_864L; // 64 MiB

    public int MaxLivingStems { get; init; } = DefaultMaxLivingStems;
    public int MaxTotalRecords { get; init; } = DefaultMaxTotalRecords;
    public int MaxMeasurementRows { get; init; } = DefaultMaxMeasurementRows;
    public long MaxUncompressedBytes { get; init; } = DefaultMaxUncompressedBytes;

    /// <summary>
    /// When true (default), living stems must have an observed positive height to be admitted for simulation.
    /// Missing height never triggers allometric estimation; it keeps the stand at InventoryOnly.
    /// </summary>
    public bool RequireHeightForSimulation { get; init; } = true;

    /// <summary>
    /// When false (default), only <see cref="SpeciesSupportCategory.ModelledAtBase"/> living species qualify
    /// for <see cref="WoodlandReadiness.CandidateForSimulationIntegration"/>.
    /// </summary>
    public bool AllowPendingModelSupportForSimulation { get; init; } = false;

    public static WoodlandImportOptions Default { get; } = new();
}

/// <summary>Self-contained, memory-backed request for a woodland archive import.</summary>
public sealed record WoodlandImportRequest
{
    public required ReadOnlyMemory<byte> ArchiveBytes { get; init; }
    public string? TargetPlotId { get; init; }
    public WoodlandBoundary? SurveyedBoundary { get; init; }
    public IReadOnlyList<LocalPoint2D>? RawLocalBoundaryVertices { get; init; }
    public IReadOnlyList<GeoPoint2D>? RawGeoBoundaryVertices { get; init; }
    public ImportSourceAttribution? AttributionOverride { get; init; }
    public WoodlandImportOptions Options { get; init; } = WoodlandImportOptions.Default;
}

/// <summary>Immutable outcome of a woodland import attempt.</summary>
public sealed record WoodlandImportResult
{
    public required WoodlandReadiness Readiness { get; init; }
    public WoodlandImportStand? Stand { get; init; }
    public required IReadOnlyList<ImportValidationIssue> Issues { get; init; }

    public bool IsAdmissibleForSimulation =>
        Readiness == WoodlandReadiness.CandidateForSimulationIntegration;
}

/// <summary>Contract implemented by woodland archive readers.</summary>
public interface IWoodlandImportReader
{
    WoodlandImportResult Read(WoodlandImportRequest request);
}
