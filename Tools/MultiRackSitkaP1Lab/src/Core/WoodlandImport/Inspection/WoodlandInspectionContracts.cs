using System.Collections.Immutable;

namespace SitkaP1.Core.WoodlandImport.Inspection;

// Every collection on these records is an ImmutableList: unlike a ReadOnlyCollection wrapper, it has no
// mutable backing list that a caller could reach (for example through ICollection.SyncRoot).

/// <summary>
/// One imported measurement as shown to the player: its provenance, its value (null when Missing),
/// and the raw source evidence copied verbatim from the importer.
/// </summary>
/// <typeparam name="T">A nullable value type or string, so that a Missing value is a real null.</typeparam>
public sealed record InspectedField<T>(
    FieldProvenance Provenance,
    T Value,
    string? SourceColumn,
    string? RawValue,
    string? Note);

/// <summary>One imported tree, exactly as observed at Year 0.</summary>
public sealed record InspectedTree
{
    public required string SourceTreeId { get; init; }
    public required InspectedField<string?> SpeciesName { get; init; }
    public required string SpeciesRawName { get; init; }
    public string? CanonicalSpeciesId { get; init; }
    public required SpeciesSupportCategory SpeciesCategory { get; init; }
    public required InspectedField<TreeVitalStatus?> VitalStatus { get; init; }
    public required InspectedField<LocalPoint2D?> PositionLocalM { get; init; }
    public required InspectedField<double?> DbhCm { get; init; }
    public required InspectedField<double?> HeightM { get; init; }
    public required InspectedField<double?> CrownRadiusM { get; init; }
    public required InspectedField<int?> AgeYears { get; init; }

    /// <summary>Same rule as the importer: unknown or missing vital status counts as living.</summary>
    public required bool IsConservativelyLiving { get; init; }
}

/// <summary>Stand identity. Absent (null on the view) when the import was Refused or the stand was withheld.</summary>
public sealed record InspectedStandIdentity
{
    public required int StandSchemaVersion { get; init; }
    public required string StandId { get; init; }
    public required string PlotName { get; init; }
    public string? CountryCode { get; init; }
    public required int TreeCount { get; init; }
    public required int LivingStemCount { get; init; }
    public required int DeadStemCount { get; init; }
    public required int UnknownStatusStemCount { get; init; }
}

/// <summary>
/// The boundary polygon the importer was given. <see cref="Observed"/> is false when no boundary was supplied;
/// in that case there are no vertices and no area, because CCF never infers a boundary from tree positions.
/// </summary>
public sealed record InspectedBoundary
{
    public required bool Observed { get; init; }
    public required ImmutableList<LocalPoint2D> VerticesLocalM { get; init; }
    public double? AreaM2 { get; init; }
    public double? AreaHa { get; init; }
    public BoundaryCoordinateSource? CoordinateSource { get; init; }
    public string? ProjectionNote { get; init; }
}

/// <summary>Everything observed at Year 0: trees in the stand's local metre frame, the origin rule, and the boundary.</summary>
public sealed record ObservedYear0Section
{
    public required ImmutableList<InspectedTree> Trees { get; init; }
    public LocalCoordinateOrigin? CoordinateOrigin { get; init; }
    public required InspectedBoundary Boundary { get; init; }
}

/// <summary>Tree counts for one species support category.</summary>
public sealed record SpeciesCategoryCount(SpeciesSupportCategory Category, int TotalTrees, int LivingTrees);

/// <summary>Tree counts for one raw species name, with its exact mapping.</summary>
public sealed record SpeciesRow(
    string RawName,
    string? CanonicalSpeciesId,
    SpeciesSupportCategory Category,
    int TotalTrees,
    int LivingTrees);

/// <summary>Species counts. <see cref="Categories"/> always lists all four categories in enum order.</summary>
public sealed record SpeciesBreakdown
{
    public required ImmutableList<SpeciesCategoryCount> Categories { get; init; }
    public required ImmutableList<SpeciesRow> Species { get; init; }
}

/// <summary>One tree-related microhabitat record taken from an importer "trem:" attribute.</summary>
public sealed record InspectedMicrohabitat(string SourceTreeId, string Code, string Value);

/// <summary>Licence and source attribution for the inspected stand.</summary>
public sealed record InspectedAttribution
{
    public required string DatasetTitle { get; init; }
    public required string Publisher { get; init; }
    public required string Licence { get; init; }
    public required string Citation { get; init; }
    public string? Doi { get; init; }
    public string? DatasetKey { get; init; }
    public string? ArchiveSha256 { get; init; }
    public string? DeclaredArchiveSha256 { get; init; }
    public string? RetrievalDateUtc { get; init; }
    public string? SourcePlotId { get; init; }
    public required ImmutableList<string> ObservationDates { get; init; }
}

/// <summary>
/// The line between observed data and simulation. In this read-only inspection nothing past Year 0 exists.
/// </summary>
public sealed record ProvenanceBoundarySection
{
    public const int ObservedYear = 0;
    public const int FirstSimulatedYear = 1;

    public required int ObservedStateYear { get; init; }
    public required ImmutableList<string> ObservationDates { get; init; }
    public required int SimulatedFutureStartsAtYear { get; init; }
    public required bool SimulatedFutureAvailable { get; init; }

    /// <summary>
    /// Number of tree fields in the import result carrying <see cref="FieldProvenance.Simulated"/>. Expected to be 0.
    /// When it is not 0 the projector withholds the whole stand, so the simulated values themselves are never shown.
    /// </summary>
    public required int SimulatedValueCount { get; init; }
    public required string Statement { get; init; }
}

/// <summary>
/// One reason the stand cannot advance time, already worded for the player, so a UI can render it without logic.
/// </summary>
public sealed record BlockerChecklistItem
{
    public required string Code { get; init; }
    public required IssueSeverity Severity { get; init; }
    public required string Title { get; init; }
    public required string Explanation { get; init; }
    public required int AffectedTreeCount { get; init; }
    public required ImmutableList<string> AffectedTreeIds { get; init; }
    public required ImmutableList<string> AffectedRawSpeciesNames { get; init; }
}

/// <summary>A non-blocking (Info or Warning) importer issue, copied verbatim.</summary>
public sealed record InspectionNote(
    string Code,
    IssueSeverity Severity,
    string Message,
    string? SourceTreeId,
    string? FieldName);

/// <summary>
/// Source disclosures that must travel with a built-in stand's inspection, so that a reader of the
/// inspection JSON alone sees them. They do not change any field's provenance label.
/// </summary>
public sealed record InspectionDisclosures
{
    /// <summary>The CC BY 4.0 section 3(a)(1)(B) disclosure of modifications, verbatim from ATTRIBUTION.json <c>changesMade</c>.</summary>
    public required string ModificationsDisclosure { get; init; }

    /// <summary>
    /// The ATTRIBUTION.json caveat that reading <c>verbatimCoordinates</c> as plot-local metres is an unverified
    /// inference. Tree positions labelled Observed are observed source values read under this inference.
    /// </summary>
    public required string CoordinateInterpretationCaveat { get; init; }
}

/// <summary>
/// Immutable, read-only Year-0 inspection of one imported woodland stand.
/// Built only by <see cref="WoodlandInspectionProjector"/>; it never contains simulated future values.
/// </summary>
public sealed record WoodlandInspectionView
{
    public const int CurrentSchemaVersion = 1;

    public int SchemaVersion { get; init; } = CurrentSchemaVersion;
    /// <summary>
    /// The importer's readiness, except that it is <see cref="WoodlandReadiness.Refused"/> when the projector
    /// withholds a stand because the stand holds a simulated value.
    /// </summary>
    public required WoodlandReadiness Readiness { get; init; }
    public required bool CanAdvanceTime { get; init; }
    public required string AdvanceTimeStatement { get; init; }
    public InspectedStandIdentity? Stand { get; init; }
    public required ObservedYear0Section ObservedYear0 { get; init; }
    public required SpeciesBreakdown SpeciesBreakdown { get; init; }
    public required ImmutableList<InspectedMicrohabitat> Microhabitats { get; init; }
    public InspectedAttribution? Attribution { get; init; }

    /// <summary>Source disclosures for a built-in stand; null when the caller supplied none.</summary>
    public InspectionDisclosures? Disclosures { get; init; }
    public required ProvenanceBoundarySection ProvenanceBoundary { get; init; }
    public required ImmutableList<BlockerChecklistItem> BlockerChecklist { get; init; }
    public required ImmutableList<InspectionNote> Notes { get; init; }
}
