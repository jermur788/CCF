using SitkaP1.Core.WoodlandImport;
using SitkaP1.Core.WoodlandImport.Inspection;

namespace SitkaP1.Core.WoodlandImport.Inspection.Tests;

/// <summary>
/// Test-side file access and synthetic stand helpers. File reading lives here, not in production code,
/// because src/Core/WoodlandImport/** must stay free of file-system APIs (P1 #644 B-01).
/// </summary>
internal static class TestFixtures
{
    // Constructed test-harness boundary (NO survey source for the Mersch plot boundary exists).
    // Used only to show that the inspection view reports CanAdvanceTime once a valid polygon is supplied.
    public static readonly LocalPoint2D[] ConstructedTestBoundary100x100 =
    [
        new(0.0, 0.0),
        new(100.0, 0.0),
        new(100.0, 100.0),
        new(0.0, 100.0),
    ];

    private static string? cachedDir;

    /// <summary>Walks up from the test binary to tests/Core/WoodlandImport/Fixtures/Qualified (read-only).</summary>
    public static string QualifiedFixturesDir()
    {
        if (cachedDir is not null)
        {
            return cachedDir;
        }

        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null)
        {
            string candidate = Path.Combine(d.FullName, "tests", "Core", "WoodlandImport", "Fixtures", "Qualified");
            if (Directory.Exists(candidate))
            {
                cachedDir = candidate;
                return candidate;
            }
            d = d.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate tests/Core/WoodlandImport/Fixtures/Qualified.");
    }

    public static byte[] ReadFixture(string fileName) =>
        File.ReadAllBytes(Path.Combine(QualifiedFixturesDir(), fileName));

    /// <summary>Directory convenience wrapper around <see cref="BuiltInWoodlandCatalogue.Load"/>.</summary>
    public static IReadOnlyList<BuiltInWoodlandEntry> LoadFromDirectory(string qualifiedFixturesDir) =>
        BuiltInWoodlandCatalogue.Load(name => File.ReadAllBytes(Path.Combine(qualifiedFixturesDir, name)));

    public static IReadOnlyList<BuiltInWoodlandEntry> LoadCatalogue() => LoadFromDirectory(QualifiedFixturesDir());

    // ------------------------------------------------------------------
    // Synthetic stands (built directly, then run through the real validator)
    // ------------------------------------------------------------------

    public static ImportedTreeRecord Tree(
        string id,
        string species,
        double x,
        double y,
        double? dbhCm = 30.0,
        double? heightM = 20.0,
        TreeVitalStatus? status = TreeVitalStatus.Alive,
        IReadOnlyDictionary<string, string>? extra = null,
        FieldProvenance heightProvenance = FieldProvenance.Observed)
    {
        return new ImportedTreeRecord
        {
            SourceTreeId = id,
            RawSpeciesName = ImportField<string>.Observed(species, new FieldEvidence("scientificName", species)),
            SpeciesMapping = WoodlandSpeciesMapper.Map(species),
            VitalStatus = status is null
                ? ImportField<TreeVitalStatus>.Missing(new FieldEvidence("status", null))
                : ImportField<TreeVitalStatus>.Observed(status.Value, new FieldEvidence("status", status == TreeVitalStatus.Alive ? "1" : "0")),
            PositionLocalM = ImportField<LocalPoint2D>.Observed(new LocalPoint2D(x, y), new FieldEvidence("verbatimCoordinates", $"{x} , {y}")),
            DbhCm = dbhCm is null
                ? ImportField<double>.Missing(new FieldEvidence("diameter", null))
                : ImportField<double>.Observed(dbhCm.Value, new FieldEvidence("diameter", $"{dbhCm.Value} cm")),
            HeightM = heightM is null
                ? ImportField<double>.Missing(new FieldEvidence("height", null))
                : new ImportField<double>
                {
                    Provenance = heightProvenance,
                    HasValue = true,
                    Value = heightM.Value,
                    Evidence = new FieldEvidence("height", $"{heightM.Value} m", heightProvenance == FieldProvenance.Simulated ? "synthetic simulated value" : null),
                },
            // Crown radius and age are left Missing on every synthetic tree: nothing may fill them in.
            CrownRadiusM = ImportField<double>.Missing(new FieldEvidence("crownRadius", null)),
            AgeYears = ImportField<int>.Missing(new FieldEvidence("age", null)),
            ExtraAttributes = extra ?? ImportedTreeRecord.EmptyAttributes,
        };
    }

    public static WoodlandImportStand Stand(IReadOnlyList<ImportedTreeRecord> trees, WoodlandBoundary? boundary = null)
    {
        var sorted = trees.OrderBy(t => t.SourceTreeId, StringComparer.Ordinal).ToList();
        return new WoodlandImportStand
        {
            StandId = "wi-v1-synthetic-000000000000",
            PlotName = "synthetic",
            CountryCode = "XX",
            Attribution = new ImportSourceAttribution
            {
                DatasetTitle = "Synthetic test stand",
                Publisher = "CCF tests",
                Licence = "test-only",
                Citation = "Synthetic test stand (no real source).",
            },
            Boundary = boundary,
            CoordinateOrigin = new LocalCoordinateOrigin(0.0, 0.0, "synthetic-test-origin"),
            Trees = sorted.AsReadOnly(),
            TotalArchiveRowsScanned = sorted.Count,
            LivingStemCount = sorted.Count(t => t.IsConservativelyLiving),
            DeadStemCount = sorted.Count(t => !t.IsConservativelyLiving),
        };
    }

    public static WoodlandImportResult Validate(WoodlandImportStand stand) =>
        WoodlandImportValidator.Validate(stand, WoodlandImportOptions.Default);

    public static WoodlandBoundary ConstructedBoundary()
    {
        if (!WoodlandBoundary.TryCreateLocal(ConstructedTestBoundary100x100, out WoodlandBoundary? b, out _))
        {
            throw new InvalidOperationException("constructed test boundary should be valid");
        }
        return b!;
    }
}
