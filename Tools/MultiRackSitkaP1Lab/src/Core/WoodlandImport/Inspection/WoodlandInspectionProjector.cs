namespace SitkaP1.Core.WoodlandImport.Inspection;

/// <summary>
/// Turns a <see cref="WoodlandImportResult"/> into a read-only Year-0 <see cref="WoodlandInspectionView"/>.
/// The projector only copies and counts: it never estimates, rounds, re-normalises or fills in a value.
/// </summary>
public static class WoodlandInspectionProjector
{
    public const string ProvenanceStatement =
        "Everything shown is the woodland as observed at Year 0. No simulated future is shown: nothing past Year 0 has been calculated.";

    private const string MicrohabitatKeyPrefix = "trem:";
    private const string EventDateKey = "eventDate";

    public static WoodlandInspectionView Project(WoodlandImportResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        WoodlandImportStand? stand = result.Stand;
        IReadOnlyList<ImportedTreeRecord> trees = stand?.Trees ?? Array.Empty<ImportedTreeRecord>();

        int simulatedValueCount = CountSimulatedFields(trees);
        IReadOnlyList<string> observationDates = CollectObservationDates(trees);

        // A stand may only advance time when the importer admitted it AND it holds no simulated value.
        // The second check is a defensive guard in case a result was assembled without the validator.
        bool canAdvanceTime =
            result.Readiness == WoodlandReadiness.CandidateForSimulationIntegration &&
            stand is not null &&
            simulatedValueCount == 0;

        return new WoodlandInspectionView
        {
            Readiness = result.Readiness,
            CanAdvanceTime = canAdvanceTime,
            AdvanceTimeStatement = DescribeAdvanceTime(result.Readiness, canAdvanceTime, stand is not null),
            Stand = stand is null ? null : ProjectIdentity(stand),
            ObservedYear0 = new ObservedYear0Section
            {
                Trees = trees.Select(ProjectTree).ToList().AsReadOnly(),
                CoordinateOrigin = stand?.CoordinateOrigin,
                Boundary = ProjectBoundary(stand?.Boundary),
            },
            SpeciesBreakdown = BuildSpeciesBreakdown(trees),
            Microhabitats = CollectMicrohabitats(trees),
            Attribution = stand is null ? null : ProjectAttribution(stand.Attribution, observationDates),
            ProvenanceBoundary = new ProvenanceBoundarySection
            {
                ObservedStateYear = ProvenanceBoundarySection.ObservedYear,
                ObservationDates = observationDates,
                SimulatedFutureStartsAtYear = ProvenanceBoundarySection.FirstSimulatedYear,
                SimulatedFutureAvailable = false,
                SimulatedValueCount = simulatedValueCount,
                Statement = ProvenanceStatement,
            },
            BlockerChecklist = BuildBlockerChecklist(result.Issues, trees),
            Notes = result.Issues
                .Where(i => !IsBlocking(i.Severity))
                .Select(i => new InspectionNote(i.Code, i.Severity, i.Message, i.SourceTreeId, i.FieldName))
                .ToList()
                .AsReadOnly(),
        };
    }

    private static string DescribeAdvanceTime(WoodlandReadiness readiness, bool canAdvanceTime, bool hasStand)
    {
        if (canAdvanceTime)
        {
            return "This stand meets the import requirements for simulation. Advancing time is not part of this read-only inspection.";
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
                "Time cannot advance because the Year-0 data contains a value marked as simulated.",
        };
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
                VerticesLocalM = Array.Empty<LocalPoint2D>(),
            };
        }

        return new InspectedBoundary
        {
            Observed = true,
            VerticesLocalM = boundary.VerticesLocalM.ToList().AsReadOnly(),
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
            .ToList()
            .AsReadOnly();

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
            .ToList()
            .AsReadOnly();

        return new SpeciesBreakdown { Categories = categories, Species = rows };
    }

    private static IReadOnlyList<InspectedMicrohabitat> CollectMicrohabitats(IReadOnlyList<ImportedTreeRecord> trees)
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
            .ToList()
            .AsReadOnly();
    }

    private static IReadOnlyList<string> CollectObservationDates(IReadOnlyList<ImportedTreeRecord> trees) =>
        trees
            .Select(t => t.ExtraAttributes.TryGetValue(EventDateKey, out string? date) ? date : null)
            .Where(d => !string.IsNullOrWhiteSpace(d))
            .Select(d => d!)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(d => d, StringComparer.Ordinal)
            .ToList()
            .AsReadOnly();

    private static InspectedAttribution ProjectAttribution(ImportSourceAttribution attribution, IReadOnlyList<string> observationDates) => new()
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

    private static int CountSimulatedFields(IReadOnlyList<ImportedTreeRecord> trees)
    {
        int count = 0;
        foreach (ImportedTreeRecord t in trees)
        {
            FieldProvenance[] provenances =
            [
                t.RawSpeciesName.Provenance,
                t.VitalStatus.Provenance,
                t.PositionLocalM.Provenance,
                t.DbhCm.Provenance,
                t.HeightM.Provenance,
                t.CrownRadiusM.Provenance,
                t.AgeYears.Provenance,
            ];
            count += provenances.Count(p => p == FieldProvenance.Simulated);
        }
        return count;
    }

    private static bool IsBlocking(IssueSeverity severity) =>
        severity == IssueSeverity.InventoryOnlyBlocker || severity == IssueSeverity.Refusal;

    private static IReadOnlyList<BlockerChecklistItem> BuildBlockerChecklist(
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
                    .ToList();

                var speciesNames = treeIds
                    .Where(speciesByTreeId.ContainsKey)
                    .Select(id => speciesByTreeId[id])
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(n => n, StringComparer.Ordinal)
                    .ToList();

                return new BlockerChecklistItem
                {
                    Code = group.Key,
                    Severity = severity,
                    Title = BlockerExplanations.TitleFor(group.Key),
                    Explanation = BlockerExplanations.ExplanationFor(group.Key, first.Message),
                    AffectedTreeCount = treeIds.Count,
                    AffectedTreeIds = treeIds.AsReadOnly(),
                    AffectedRawSpeciesNames = speciesNames.AsReadOnly(),
                };
            })
            .OrderBy(item => item.Severity == IssueSeverity.Refusal ? 0 : 1)
            .ThenBy(item => item.Code, StringComparer.Ordinal)
            .ToList()
            .AsReadOnly();
    }
}
