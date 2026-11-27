using System.Text.Json;

namespace SitkaP1.Core.WoodlandImport.Inspection;

/// <summary>Fixed facts about one built-in woodland, pinned in source code.</summary>
public sealed record BuiltInWoodlandDefinition(
    string PlotId,
    string DisplayName,
    string CountryCode,
    string FixtureFileName,
    string ArchiveSha256,
    int SubsetRecordCount,
    int GbifPlotTotalRecords);

/// <summary>One loaded built-in woodland: its pinned definition, attribution metadata and the real import result.</summary>
public sealed record BuiltInWoodlandEntry
{
    public required BuiltInWoodlandDefinition Definition { get; init; }
    public string PlotId => Definition.PlotId;
    public string DisplayName => Definition.DisplayName;
    public string CountryCode => Definition.CountryCode;

    public required int TreeCount { get; init; }
    public required int LivingStemCount { get; init; }

    /// <summary>For example "20 of 271 GBIF records for this plot".</summary>
    public required string SubsetOfGbifRecords { get; init; }
    public required WoodlandReadiness Readiness { get; init; }

    public required string LicenceSummary { get; init; }
    public required string LicenceUrl { get; init; }
    public required string DatasetTitle { get; init; }
    public required string Publisher { get; init; }
    public required string Citation { get; init; }
    public required string Doi { get; init; }
    public required string DatasetKey { get; init; }
    public required string RetrievalDateUtc { get; init; }

    /// <summary>The pinned SHA-256 of the fixture ZIP (the importer refuses the archive if its bytes differ).</summary>
    public required string ArchiveSha256 { get; init; }
    public required IReadOnlyList<string> ObservationDates { get; init; }

    /// <summary>The CC BY 4.0 section 3(a)(1)(B) disclosure of modifications, verbatim from ATTRIBUTION.json.</summary>
    public required string ModificationsDisclosure { get; init; }
    public required IReadOnlyList<string> Caveats { get; init; }

    public required WoodlandImportResult ImportResult { get; init; }
}

/// <summary>
/// The fixed, offline list of qualified real woodlands CCF ships with. Donadea (Ireland) is always first.
/// Fixture files are read through a caller-supplied function so this production code never touches the
/// file system itself (P1 #644 B-01). No boundary is ever passed to the importer: neither plot has a
/// surveyed boundary, and CCF does not fabricate one.
/// </summary>
public static class BuiltInWoodlandCatalogue
{
    public const string AttributionFileName = "ATTRIBUTION.json";
    public const string ExpectedLicenceSummary = "CC BY 4.0";

    public const string NoBoundaryCaveat =
        "No surveyed boundary is available for this plot, so CCF imports it without one and it cannot be simulated.";

    public static IReadOnlyList<BuiltInWoodlandDefinition> Definitions { get; } = new List<BuiltInWoodlandDefinition>
    {
        new(
            PlotId: "IPlus_Donadea_MHF_2",
            DisplayName: "Donadea MHF 2 plot (Ireland)",
            CountryCode: "IE",
            FixtureFileName: "ie-donadea-mhf-subset.dwca.zip",
            ArchiveSha256: "4ee2f4a125d1a06474d7086ccf5a6ca96735e16124ad3e4f9a11647ddcee853d",
            SubsetRecordCount: 20,
            GbifPlotTotalRecords: 271),
        new(
            PlotId: "IPlus_Mersch",
            DisplayName: "Mersch oak-beech plot (Luxembourg)",
            CountryCode: "LU",
            FixtureFileName: "lu-mersch-oak-beech-subset.dwca.zip",
            ArchiveSha256: "3c275b0115b6330ebabdad422ea9a25451489c93949dd5d80be1fb5e8b57b1ae",
            SubsetRecordCount: 20,
            GbifPlotTotalRecords: 134),
    }.AsReadOnly();

    /// <summary>
    /// Loads every built-in woodland in catalogue order.
    /// </summary>
    /// <param name="readFixtureFile">Returns the bytes of a file in the qualified fixtures folder, given its file name.</param>
    /// <exception cref="InvalidDataException">ATTRIBUTION.json is missing a field or disagrees with a pinned value.</exception>
    public static IReadOnlyList<BuiltInWoodlandEntry> Load(Func<string, byte[]> readFixtureFile)
    {
        ArgumentNullException.ThrowIfNull(readFixtureFile);

        using JsonDocument attributionDoc = JsonDocument.Parse(readFixtureFile(AttributionFileName));
        JsonElement root = attributionDoc.RootElement;

        string datasetTitle = RequireString(root, "datasetTitle");
        string publisher = RequireString(root, "publisher");
        string licence = RequireString(root, "licence");
        string citation = RequireString(root, "citation");
        string doi = RequireString(root, "doi");
        string datasetKey = RequireString(root, "gbifDatasetKey");
        string retrievalDate = RequireString(root, "retrievalDateUtc");
        string changesMade = RequireString(root, "changesMade");
        string? downloadStatus = OptionalString(root, "publisherArchiveDownloadStatus");
        (string licenceSummary, string licenceUrl) = SplitLicence(licence);

        var caveats = new List<string>();
        // ATTRIBUTION.json keeps its coordinate/test-polygon caveat in a "Note:" sentence inside changesMade.
        int noteIndex = changesMade.IndexOf("Note:", StringComparison.Ordinal);
        if (noteIndex >= 0)
        {
            caveats.Add(changesMade.Substring(noteIndex).Trim());
        }
        if (downloadStatus is not null)
        {
            caveats.Add(downloadStatus);
        }
        caveats.Add(NoBoundaryCaveat);
        IReadOnlyList<string> sharedCaveats = caveats.AsReadOnly();

        var reader = new IntegrateMarteloscopeReader();
        var entries = new List<BuiltInWoodlandEntry>(Definitions.Count);

        foreach (BuiltInWoodlandDefinition definition in Definitions)
        {
            VerifyFixtureRecord(root, definition);

            var attributionOverride = new ImportSourceAttribution
            {
                DatasetTitle = datasetTitle,
                Publisher = publisher,
                Licence = licence,
                Citation = citation,
                Doi = doi,
                DatasetKey = datasetKey,
                DeclaredArchiveSha256 = definition.ArchiveSha256,
                RetrievalDateUtc = retrievalDate,
                SourcePlotId = definition.PlotId,
            };

            WoodlandImportResult result = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = readFixtureFile(definition.FixtureFileName),
                TargetPlotId = definition.PlotId,
                AttributionOverride = attributionOverride,
                // Deliberately no SurveyedBoundary / RawLocalBoundaryVertices / RawGeoBoundaryVertices.
            });

            WoodlandInspectionView view = WoodlandInspectionProjector.Project(result);

            entries.Add(new BuiltInWoodlandEntry
            {
                Definition = definition,
                TreeCount = result.Stand?.Trees.Count ?? 0,
                LivingStemCount = result.Stand?.LivingStemCount ?? 0,
                SubsetOfGbifRecords = $"{definition.SubsetRecordCount} of {definition.GbifPlotTotalRecords} GBIF records for this plot",
                Readiness = result.Readiness,
                LicenceSummary = licenceSummary,
                LicenceUrl = licenceUrl,
                DatasetTitle = datasetTitle,
                Publisher = publisher,
                Citation = citation,
                Doi = doi,
                DatasetKey = datasetKey,
                RetrievalDateUtc = retrievalDate,
                ArchiveSha256 = definition.ArchiveSha256,
                ObservationDates = view.ProvenanceBoundary.ObservationDates,
                ModificationsDisclosure = changesMade,
                Caveats = sharedCaveats,
                ImportResult = result,
            });
        }

        return entries.AsReadOnly();
    }

    private static void VerifyFixtureRecord(JsonElement root, BuiltInWoodlandDefinition definition)
    {
        if (!root.TryGetProperty("fixtures", out JsonElement fixtures) || fixtures.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidDataException("ATTRIBUTION.json has no 'fixtures' array.");
        }

        foreach (JsonElement fixture in fixtures.EnumerateArray())
        {
            if (RequireString(fixture, "fileName") != definition.FixtureFileName)
            {
                continue;
            }

            string sha = RequireString(fixture, "sha256");
            if (!sha.Equals(definition.ArchiveSha256, StringComparison.Ordinal))
            {
                throw new InvalidDataException(
                    $"ATTRIBUTION.json SHA-256 for '{definition.FixtureFileName}' is '{sha}', but the catalogue pins '{definition.ArchiveSha256}'.");
            }
            if (RequireInt(fixture, "subsetRecordCount") != definition.SubsetRecordCount ||
                RequireInt(fixture, "plotTotalRecordsInGbif") != definition.GbifPlotTotalRecords)
            {
                throw new InvalidDataException(
                    $"ATTRIBUTION.json record counts for '{definition.FixtureFileName}' disagree with the catalogue.");
            }
            return;
        }

        throw new InvalidDataException($"ATTRIBUTION.json has no fixture entry for '{definition.FixtureFileName}'.");
    }

    // ATTRIBUTION.json writes the licence as "CC BY 4.0 (http://...)".
    private static (string Summary, string Url) SplitLicence(string licence)
    {
        int open = licence.IndexOf(" (", StringComparison.Ordinal);
        if (open < 0 || !licence.EndsWith(')'))
        {
            throw new InvalidDataException($"ATTRIBUTION.json licence '{licence}' is not in the form 'NAME (URL)'.");
        }

        string summary = licence.Substring(0, open);
        string url = licence.Substring(open + 2, licence.Length - open - 3);
        if (summary != ExpectedLicenceSummary)
        {
            throw new InvalidDataException($"ATTRIBUTION.json licence is '{summary}', expected '{ExpectedLicenceSummary}'.");
        }
        return (summary, url);
    }

    private static string RequireString(JsonElement element, string name)
    {
        string? value = OptionalString(element, name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidDataException($"ATTRIBUTION.json is missing required string '{name}'.");
        }
        return value;
    }

    private static string? OptionalString(JsonElement element, string name) =>
        element.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static int RequireInt(JsonElement element, string name)
    {
        if (element.TryGetProperty(name, out JsonElement value) && value.TryGetInt32(out int number))
        {
            return number;
        }
        throw new InvalidDataException($"ATTRIBUTION.json is missing required integer '{name}'.");
    }
}
