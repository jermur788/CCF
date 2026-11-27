using System.Collections.Immutable;

namespace SitkaP1.Core.WoodlandImport.Inspection;

/// <summary>
/// Turns a <see cref="WoodlandImportResult"/> into a read-only Year-0 <see cref="WoodlandInspectionView"/>.
/// The projector only copies and counts: it never estimates, rounds, re-normalises or fills in a value.
/// </summary>
/// <remarks>
/// Two rules hold at this public boundary even for a result that was assembled by hand without the validator:
/// a Refused result projects no stand data, and a stand holding any Simulated field is withheld entirely
/// (reported as Refused with a <see cref="SimulatedFieldAtYearZeroCode"/> checklist item).
/// </remarks>
public static class WoodlandInspectionProjector
{
    public const string ProvenanceStatement =
        "Everything shown is the woodland as observed at Year 0. No simulated future is shown: nothing past Year 0 has been calculated.";

    /// <summary>The importer's issue code for a Simulated field at Year 0; reused when the projector withholds a stand.</summary>
    public const string SimulatedFieldAtYearZeroCode = "SimulatedFieldAtYearZero";

    private const string MicrohabitatKeyPrefix = "trem:";
    private const string EventDateKey = "eventDate";

    public static WoodlandInspectionView Project(WoodlandImportResult result) => Project(result, disclosures: null);

    /// <param name="result">The importer's result. It is read, never modified.</param>
    /// <param name="disclosures">Source disclosures to carry into the view and its JSON (built-in stands supply them).</param>
    public static WoodlandInspectionView Project(WoodlandImportResult result, InspectionDisclosures? disclosures)
    {
        ArgumentNullException.ThrowIfNull(result);

        IReadOnlyList<ImportedTreeRecord> sourceTrees = result.Stand?.Trees ?? Array.Empty<ImportedTreeRecord>();
        int simulatedValueCount = CountSimulatedFields(sourceTrees);

        // Year 0 must be observed data only. The validator already refuses a Simulated field; this guard covers
        // a result built without it, so a simulated measurement can never reach the Year-0 payload.
        bool holdsSimulatedValue = simulatedValueCount > 0;
        bool importRefused = result.Readiness == WoodlandReadiness.Refused;

        // Refused => no stand data, whatever the result object carries.
        WoodlandImportStand? stand = importRefused || holdsSimulatedValue ? null : result.Stand;
        IReadOnlyList<ImportedTreeRecord> trees = stand?.Trees ?? Array.Empty<ImportedTreeRecord>();

        WoodlandReadiness readiness = holdsSimulatedValue ? WoodlandReadiness.Refused : result.Readiness;
        IReadOnlyList<ImportValidationIssue> issues = holdsSimulatedValue
            ? AddSimulatedFieldRefusals(result.Issues, sourceTrees)
            : result.Issues;

        ImmutableList<string> observationDates = CollectObservationDates(trees);
        bool canAdvanceTime = readiness == WoodlandReadiness.CandidateForSimulationIntegration && stand is not null;

        return new WoodlandInspectionView
        {
            Readiness = readiness,
            CanAdvanceTime = canAdvanceTime,
            AdvanceTimeStatement = DescribeAdvanceTime(readiness, canAdvanceTime, stand is not null, holdsSimulatedValue),
            Stand = stand is null ? null : ProjectIdentity(stand),
            ObservedYear0 = new ObservedYear0Section
            {
                Trees = trees.Select(ProjectTree).ToImmutableList(),
                CoordinateOrigin = stand?.CoordinateOrigin,
                Boundary = ProjectBoundary(stand?.Boundary),
            },
            SpeciesBreakdown = BuildSpeciesBreakdown(trees),
            Microhabitats = CollectMicrohabitats(trees),
            Attribution = stand is null ? null : ProjectAttribution(stand.Attribution, observationDates),
            Disclosures = disclosures,
            ProvenanceBoundary = new ProvenanceBoundarySection
            {
                ObservedStateYear = ProvenanceBoundarySection.ObservedYear,
                ObservationDates = observationDates,
                SimulatedFutureStartsAtYear = ProvenanceBoundarySection.FirstSimulatedYear,
                SimulatedFutureAvailable = false,
                SimulatedValueCount = simulatedValueCount,
                Statement = ProvenanceStatement,
            },
            BlockerChecklist = BuildBlockerChecklist(issues, trees),
            Notes = issues
                .Where(i => !IsBlocking(i.Severity))
                .Select(i => new InspectionNote(i.Code, i.Severity, i.Message, i.SourceTreeId, i.FieldName))
                .ToImmutableList(),
        };
    }

    private static string DescribeAdvanceTime(WoodlandReadiness readiness, bool canAdvanceTime, bool hasStand, bool holdsSimulatedValue)
    {
        if (canAdvanceTime)
        {
            return "This stand meets the import requirements for simulation. Advancing time is not part of this read-only inspection.";
        }

        if (holdsSimulatedValue)
        {
            return "The Year-0 data contains a value marked as simulated, so the stand is withheld from this inspection and cannot be simulated. See the checklist for the reason.";
        }

        return readiness switch
        {
            WoodlandReadiness.Refused =>
                "The import was refused, so there is no stand to inspect or simulate. See the checklist for the reasons.",
            WoodlandReadiness.InventoryOnly =>
                "This stand can be inspected but not simulated. Time cannot advance until every item in the checklist is resolved.",
            _ when !hasStand =>
                "There is no stand to simulate.",
            _ =>
                "Time cannot advance.",
        };
    }

    // One Refusal issue per Simulated field (unless the result already lists it as a Refusal), so the withheld
    // stand still gets a renderable checklist reason that names the affected trees but none of their values.
    // Only a matching Refusal counts as already listed: a Warning or Info with the same code is shown as a note,
    // not a checklist item, and must not leave a withheld stand without a refusal reason.
    private static IReadOnlyList<ImportValidationIssue> AddSimulatedFieldRefusals(
        IReadOnlyList<ImportValidationIssue> issues,
        IReadOnlyList<ImportedTreeRecord> trees)
    {
        var combined = new List<ImportValidationIssue>(issues);
        var alreadyListed = new HashSet<(string?, string?)>(
            issues
                .Where(i => i.Code == SimulatedFieldAtYearZeroCode && i.Severity == IssueSeverity.Refusal)
                .Select(i => (i.SourceTreeId, i.FieldName)));

        foreach (ImportedTreeRecord tree in trees)
        {
            foreach ((string fieldName, FieldProvenance provenance) in FieldProvenances(tree))
            {
                if (provenance == FieldProvenance.Simulated && alreadyListed.Add((tree.SourceTreeId, fieldName)))
                {
                    combined.Add(new ImportValidationIssue(
                        SimulatedFieldAtYearZeroCode,
                        IssueSeverity.Refusal,
                        $"Tree '{tree.SourceTreeId}' field '{fieldName}' carries FieldProvenance.Simulated, which is forbidden at Year 0; the inspection withheld the stand.",
                        SourceTreeId: tree.SourceTreeId,
                        FieldName: fieldName));
                }
            }
        }

        return combined;
    }

    private static InspectedStandIdentity ProjectIdentity(WoodlandImportStand stand) => new()
    {
        StandSchemaVersion = stand.SchemaVersion,
        StandId = stand.StandId,
        PlotName = stand.PlotName,
        CountryCode = stand.CountryCode,
        TreeCount = stand.Trees.Count,
        LivingStemCount = stand.LivingStemCount,
        DeadStemCount = stand.DeadStemCount,
        UnknownStatusStemCount = stand.UnknownStatusStemCount,
    };

    private static InspectedTree ProjectTree(ImportedTreeRecord tree) => new()
    {
        SourceTreeId = tree.SourceTreeId,
        SpeciesName = ProjectReference(tree.RawSpeciesName),
        SpeciesRawName = tree.SpeciesMapping.RawName,
        CanonicalSpeciesId = tree.SpeciesMapping.CanonicalSpeciesId,
        SpeciesCategory = tree.SpeciesMapping.Category,
        VitalStatus = ProjectValue(tree.VitalStatus),
        PositionLocalM = ProjectValue(tree.PositionLocalM),
        DbhCm = ProjectValue(tree.DbhCm),
        HeightM = ProjectValue(tree.HeightM),
        CrownRadiusM = ProjectValue(tree.CrownRadiusM),
        AgeYears = ProjectValue(tree.AgeYears),
        IsConservativelyLiving = tree.IsConservativelyLiving,
    };

    // Separate methods for value types and strings: C# cannot express "T? means Nullable<T>" for an
    // unconstrained T, and a Missing double must become a real null rather than 0.0.
    private static InspectedField<T?> ProjectValue<T>(ImportField<T> field) where T : struct =>
        new(
            field.Provenance,
            field.IsMissing ? null : field.Value,
            field.Evidence?.SourceColumn,
            field.Evidence?.RawValue,
            field.Evidence?.Note);

    private static InspectedField<string?> ProjectReference(ImportField<string> field) =>
        new(
            field.Provenance,
            field.IsMissing ? null : field.Value,
            field.Evidence?.SourceColumn,
            field.Evidence?.RawValue,
            field.Evidence?.Note);

    private static InspectedBoundary ProjectBoundary(WoodlandBoundary? boundary)
    {
        if (boundary is null)
        {
            return new InspectedBoundary
            {
                Observed = false,
                VerticesLocalM = ImmutableList<LocalPoint2D>.Empty,
            };
        }

        return new InspectedBoundary
        {
            Observed = true,
            VerticesLocalM = boundary.VerticesLocalM.ToImmutableList(),
            AreaM2 = boundary.AreaM2,
            AreaHa = boundary.AreaHa,
            CoordinateSource = boundary.CoordinateSource,
            ProjectionNote = boundary.ProjectionNote,
        };
    }

    private static SpeciesBreakdown BuildSpeciesBreakdown(IReadOnlyList<ImportedTreeRecord> trees)
    {
        // All four categories are always listed, even with zero trees, so a UI never has to add missing rows.
        var categories = Enum.GetValues<SpeciesSupportCategory>()
            .OrderBy(c => (int)c)
            .Select(c => new SpeciesCategoryCount(
                c,
                trees.Count(t => t.SpeciesMapping.Category == c),
                trees.Count(t => t.SpeciesMapping.Category == c && t.IsConservativelyLiving)))
            .ToImmutableList();

        var rows = trees
            .GroupBy(t => t.SpeciesMapping.RawName, StringComparer.Ordinal)
            .Select(g =>
            {
                SpeciesMappingResult mapping = g.First().SpeciesMapping;
                return new SpeciesRow(
                    g.Key,
                    mapping.CanonicalSpeciesId,
                    mapping.Category,
                    g.Count(),
                    g.Count(t => t.IsConservativelyLiving));
            })
            .OrderBy(r => (int)r.Category)
            .ThenBy(r => r.RawName, StringComparer.Ordinal)
            .ToImmutableList();

        return new SpeciesBreakdown { Categories = categories, Species = rows };
    }

    private static ImmutableList<InspectedMicrohabitat> CollectMicrohabitats(IReadOnlyList<ImportedTreeRecord> trees)
    {
        var list = new List<InspectedMicrohabitat>();
        foreach (ImportedTreeRecord tree in trees)
        {
            foreach (KeyValuePair<string, string> attribute in tree.ExtraAttributes)
            {
                if (attribute.Key.StartsWith(MicrohabitatKeyPrefix, StringComparison.Ordinal))
                {
                    list.Add(new InspectedMicrohabitat(
                        tree.SourceTreeId,
                        attribute.Key.Substring(MicrohabitatKeyPrefix.Length),
                        attribute.Value));
                }
            }
        }

        return list
            .OrderBy(m => m.SourceTreeId, StringComparer.Ordinal)
            .ThenBy(m => m.Code, StringComparer.Ordinal)
            .ThenBy(m => m.Value, StringComparer.Ordinal)
            .ToImmutableList();
    }

    private static ImmutableList<string> CollectObservationDates(IReadOnlyList<ImportedTreeRecord> trees) =>
        trees
            .Select(t => t.ExtraAttributes.TryGetValue(EventDateKey, out string? date) ? date : null)
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Select(d => d!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(d => d, StringComparer.Ordinal)
            .ToImmutableList();

    private static InspectedAttribution ProjectAttribution(ImportSourceAttribution attribution, ImmutableList<string> observationDates) => new()
    {
        DatasetTitle = attribution.DatasetTitle,
        Publisher = attribution.Publisher,
        Licence = attribution.Licence,
        Citation = attribution.Citation,
        Doi = attribution.Doi,
        DatasetKey = attribution.DatasetKey,
        ArchiveSha256 = attribution.ArchiveSha256,
        DeclaredArchiveSha256 = attribution.DeclaredArchiveSha256,
        RetrievalDateUtc = attribution.RetrievalDateUtc,
        SourcePlotId = attribution.SourcePlotId,
        ObservationDates = observationDates,
    };

    private static int CountSimulatedFields(IReadOnlyList<ImportedTreeRecord> trees) =>
        trees.Sum(t => FieldProvenances(t).Count(f => f.Provenance == FieldProvenance.Simulated));

    // Field names match the validator's (nameof on the importer record).
    private static (string FieldName, FieldProvenance Provenance)[] FieldProvenances(ImportedTreeRecord t) =>
    [
        (nameof(ImportedTreeRecord.RawSpeciesName), t.RawSpeciesName.Provenance),
        (nameof(ImportedTreeRecord.VitalStatus), t.VitalStatus.Provenance),
        (nameof(ImportedTreeRecord.PositionLocalM), t.PositionLocalM.Provenance),
        (nameof(ImportedTreeRecord.DbhCm), t.DbhCm.Provenance),
        (nameof(ImportedTreeRecord.HeightM), t.HeightM.Provenance),
        (nameof(ImportedTreeRecord.CrownRadiusM), t.CrownRadiusM.Provenance),
        (nameof(ImportedTreeRecord.AgeYears), t.AgeYears.Provenance),
    ];

    private static bool IsBlocking(IssueSeverity severity) =>
        severity == IssueSeverity.InventoryOnlyBlocker || severity == IssueSeverity.Refusal;

    private static ImmutableList<BlockerChecklistItem> BuildBlockerChecklist(
        IReadOnlyList<ImportValidationIssue> issues,
        IReadOnlyList<ImportedTreeRecord> trees)
    {
        var speciesByTreeId = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (ImportedTreeRecord tree in trees)
        {
            // TryAdd: a duplicate id is itself a refusal reason; the first record's species is enough here.
            speciesByTreeId.TryAdd(tree.SourceTreeId, tree.SpeciesMapping.RawName);
        }

        return issues
            .Where(i => IsBlocking(i.Severity))
            .GroupBy(i => i.Code, StringComparer.Ordinal)
            .Select(group =>
            {
                // Importer issues are already canonically sorted, so the first message is deterministic.
                ImportValidationIssue first = group.First();
                IssueSeverity severity = group.Max(i => i.Severity);

                var treeIds = group
                    .Select(i => i.SourceTreeId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Select(id => id!)
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(id => id, StringComparer.Ordinal)
                    .ToImmutableList();

                var speciesNames = treeIds
                    .Where(speciesByTreeId.ContainsKey)
                    .Select(id => speciesByTreeId[id])
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(n => n, StringComparer.Ordinal)
                    .ToImmutableList();

                return new BlockerChecklistItem
                {
                    Code = group.Key,
                    Severity = severity,
                    Title = BlockerExplanations.TitleFor(group.Key),
                    Explanation = BlockerExplanations.ExplanationFor(group.Key, first.Message),
                    AffectedTreeCount = treeIds.Count,
                    AffectedTreeIds = treeIds,
                    AffectedRawSpeciesNames = speciesNames,
                };
            })
            .OrderBy(item => item.Severity == IssueSeverity.Refusal ? 0 : 1)
            .ThenBy(item => item.Code, StringComparer.Ordinal)
            .ToImmutableList();
    }
}
