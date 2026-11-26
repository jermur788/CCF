using System.Text.Json;
using SitkaP1.Core.WoodlandImport;

namespace SitkaP1.Core.WoodlandImport.Tests;

internal static class FixtureQualificationTests
{
    private const string Suite = "qualified";

    private const string ExpectedIeSha256 = "4ee2f4a125d1a06474d7086ccf5a6ca96735e16124ad3e4f9a11647ddcee853d";
    private const string ExpectedLuSha256 = "3c275b0115b6330ebabdad422ea9a25451489c93949dd5d80be1fb5e8b57b1ae";

    private static readonly LocalPoint2D[] Marteloscope50x100 =
    [
        new(0.0, 0.0),
        new(50.0, 0.0),
        new(50.0, 100.0),
        new(0.0, 100.0),
    ];

    private static readonly LocalPoint2D[] Marteloscope100x100 =
    [
        new(0.0, 0.0),
        new(100.0, 0.0),
        new(100.0, 100.0),
        new(0.0, 100.0),
    ];

    public static void Register()
    {
        TestRunner.Add(Suite, "qualified-attribution: ATTRIBUTION.json and both CC BY 4.0 fixture ZIPs match pinned SHA-256 offline", () =>
        {
            string dir = FindQualifiedFixturesDir();
            string attrPath = Path.Combine(dir, "ATTRIBUTION.json");
            TestRunner.True(File.Exists(attrPath), "ATTRIBUTION.json exists");

            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(attrPath));
            JsonElement root = doc.RootElement;
            TestRunner.Eq("10.15468/ocof3v", root.GetProperty("doi").GetString(), "DOI");
            TestRunner.Eq("2e102194-f384-4712-89a4-5db7a3fc409a", root.GetProperty("gbifDatasetKey").GetString(), "GBIF dataset key");
            TestRunner.True(root.GetProperty("licence").GetString()!.Contains("CC BY 4.0", StringComparison.Ordinal), "CC BY 4.0 licence");

            byte[] ieBytes = File.ReadAllBytes(Path.Combine(dir, "ie-donadea-mhf-subset.dwca.zip"));
            byte[] luBytes = File.ReadAllBytes(Path.Combine(dir, "lu-mersch-oak-beech-subset.dwca.zip"));
            TestRunner.Eq(ExpectedIeSha256, WoodlandImportSerializer.ComputeSha256Hex(ieBytes), "IE fixture SHA-256");
            TestRunner.Eq(ExpectedLuSha256, WoodlandImportSerializer.ComputeSha256Hex(luBytes), "LU fixture SHA-256");
        });

        TestRunner.Add(Suite, "qualified-ie-donadea: real Irish plot imports cleanly as InventoryOnly with all 20 verbatim trees retained", () =>
        {
            string dir = FindQualifiedFixturesDir();
            byte[] ieBytes = File.ReadAllBytes(Path.Combine(dir, "ie-donadea-mhf-subset.dwca.zip"));
            var reader = new IntegrateMarteloscopeReader();

            // 1. Without boundary -> InventoryOnly (BoundaryMissing + UnsupportedLivingSpecies)
            WoodlandImportResult noBoundary = reader.Read(new WoodlandImportRequest { ArchiveBytes = ieBytes });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, noBoundary.Readiness, "IE no-boundary readiness");
            TestRunner.True(!noBoundary.IsAdmissibleForSimulation, "IE not admissible");
            TestRunner.True(noBoundary.Stand is not null, "IE stand retained");
            TestRunner.Eq(20, noBoundary.Stand!.Trees.Count, "all 20 Irish trees retained");
            TestRunner.Eq(18, noBoundary.Stand.LivingStemCount, "IE living count");
            TestRunner.Eq(2, noBoundary.Stand.DeadStemCount, "IE dead count");
            TestRunner.Eq("IE", noBoundary.Stand.CountryCode, "country IE");
            TestRunner.True(noBoundary.Issues.Any(i => i.Code == "BoundaryMissing"), "BoundaryMissing present");
            TestRunner.True(noBoundary.Issues.Any(i => i.Code == "UnsupportedLivingSpecies"), "UnsupportedLivingSpecies present");

            // 2. With 50x100 m boundary -> STILL InventoryOnly because unsupported living Irish species block simulation (Option A #1)
            WoodlandImportResult withBoundary = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = ieBytes,
                RawLocalBoundaryVertices = Marteloscope50x100,
            });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, withBoundary.Readiness, "IE with-boundary still InventoryOnly");
            TestRunner.Eq(20, withBoundary.Stand!.Trees.Count, "zero silent deletion");
            TestRunner.ApproxEq(5000.0, withBoundary.Stand.Boundary!.AreaM2, 1e-9, "0.5 ha boundary");
            TestRunner.True(!withBoundary.Issues.Any(i => i.Code == "BoundaryMissing"), "boundary resolved");
            TestRunner.True(!withBoundary.Issues.Any(i => i.Code == "TreeOutOfBounds"), "all 20 trees inside 50x100 m");
            TestRunner.True(withBoundary.Issues.Any(i => i.Code == "UnsupportedLivingSpecies"), "unsupported living species still block admission");

            // Verify beech trees mapped to ModelledAtBase while ash/spruce/elm/oak/rowan/hawthorn are preserved unmodelled
            int beechCount = withBoundary.Stand.Trees.Count(t => t.SpeciesMapping.CanonicalSpeciesId == "beech");
            int unmodelledCount = withBoundary.Stand.Trees.Count(t => t.SpeciesMapping.Category == SpeciesSupportCategory.UnmodelledSpecies);
            TestRunner.Eq(12, beechCount, "12 Fagus sylvatica mapped to beech");
            TestRunner.Eq(8, unmodelledCount, "8 unmodelled Irish species preserved verbatim");
        });

        TestRunner.Add(Suite, "qualified-lu-mersch: compatible European Oak-Beech plot is InventoryOnly without boundary and CandidateForSimulationIntegration with boundary", () =>
        {
            string dir = FindQualifiedFixturesDir();
            byte[] luBytes = File.ReadAllBytes(Path.Combine(dir, "lu-mersch-oak-beech-subset.dwca.zip"));
            var reader = new IntegrateMarteloscopeReader();

            // 1. Without boundary -> Option A #3 requires InventoryOnly (no boundary inference)
            WoodlandImportResult noBoundary = reader.Read(new WoodlandImportRequest { ArchiveBytes = luBytes });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, noBoundary.Readiness, "LU without boundary -> InventoryOnly");
            TestRunner.Eq(1, noBoundary.Issues.Count, "only BoundaryMissing blocker");
            TestRunner.Eq("BoundaryMissing", noBoundary.Issues[0].Code, "BoundaryMissing");

            // 2. With surveyed 100x100 m boundary -> CandidateForSimulationIntegration
            WoodlandImportResult withBoundary = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = luBytes,
                RawLocalBoundaryVertices = Marteloscope100x100,
            });
            TestRunner.Eq(WoodlandReadiness.CandidateForSimulationIntegration, withBoundary.Readiness, "LU with boundary -> CandidateForSimulationIntegration");
            TestRunner.True(withBoundary.IsAdmissibleForSimulation, "admissible for future simulation integration");
            TestRunner.Eq(0, withBoundary.Issues.Count, "zero validation issues");
            TestRunner.Eq(20, withBoundary.Stand!.Trees.Count, "all 20 trees retained");
            TestRunner.Eq(17, withBoundary.Stand.LivingStemCount, "17 living stems");
            TestRunner.Eq(3, withBoundary.Stand.DeadStemCount, "3 dead stems");
            TestRunner.Eq("LU", withBoundary.Stand.CountryCode, "country LU");
            TestRunner.ApproxEq(10000.0, withBoundary.Stand.Boundary!.AreaM2, 1e-9, "1.0 ha marteloscope");
            TestRunner.Eq(9, withBoundary.Stand.Trees.Count(t => t.SpeciesMapping.CanonicalSpeciesId == "beech"), "9 beech");
            TestRunner.Eq(11, withBoundary.Stand.Trees.Count(t => t.SpeciesMapping.CanonicalSpeciesId == "sessile-oak"), "11 sessile oak");
        });

        TestRunner.Add(Suite, "qualified-determinism: two independent offline imports of both CC BY 4.0 fixtures yield byte-identical JSON and SHA-256", () =>
        {
            string dir = FindQualifiedFixturesDir();
            var reader = new IntegrateMarteloscopeReader();

            byte[] ieBytes = File.ReadAllBytes(Path.Combine(dir, "ie-donadea-mhf-subset.dwca.zip"));
            var ieReq = new WoodlandImportRequest { ArchiveBytes = ieBytes, RawLocalBoundaryVertices = Marteloscope50x100 };
            var ie1 = reader.Read(ieReq);
            var ie2 = reader.Read(ieReq);
            byte[] ieJson1 = WoodlandImportSerializer.SerializeResultUtf8(ie1);
            byte[] ieJson2 = WoodlandImportSerializer.SerializeResultUtf8(ie2);
            string ieHash1 = WoodlandImportSerializer.ComputeResultSha256Hex(ie1);
            string ieHash2 = WoodlandImportSerializer.ComputeResultSha256Hex(ie2);
            TestRunner.True(ieJson1.AsSpan().SequenceEqual(ieJson2), "IE canonical JSON byte-identical");
            TestRunner.Eq(ieHash1, ieHash2, "IE SHA-256 identical");
            Console.WriteLine($"  [determinism] ie-donadea-canonical-sha256={ieHash1} ({ieJson1.Length} bytes)");

            byte[] luBytes = File.ReadAllBytes(Path.Combine(dir, "lu-mersch-oak-beech-subset.dwca.zip"));
            var luReq = new WoodlandImportRequest { ArchiveBytes = luBytes, RawLocalBoundaryVertices = Marteloscope100x100 };
            var lu1 = reader.Read(luReq);
            var lu2 = reader.Read(luReq);
            byte[] luJson1 = WoodlandImportSerializer.SerializeResultUtf8(lu1);
            byte[] luJson2 = WoodlandImportSerializer.SerializeResultUtf8(lu2);
            string luHash1 = WoodlandImportSerializer.ComputeResultSha256Hex(lu1);
            string luHash2 = WoodlandImportSerializer.ComputeResultSha256Hex(lu2);
            TestRunner.True(luJson1.AsSpan().SequenceEqual(luJson2), "LU canonical JSON byte-identical");
            TestRunner.Eq(luHash1, luHash2, "LU SHA-256 identical");
            Console.WriteLine($"  [determinism] lu-mersch-canonical-sha256={luHash1} ({luJson1.Length} bytes)");
        });
    }

    private static string FindQualifiedFixturesDir()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null)
        {
            string candidate = Path.Combine(d.FullName, "tests", "Core", "WoodlandImport", "Fixtures", "Qualified");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
            string direct = Path.Combine(d.FullName, "Fixtures", "Qualified");
            if (Directory.Exists(direct))
            {
                return direct;
            }
            d = d.Parent;
        }
        throw new DirectoryNotFoundException("Could not locate Fixtures/Qualified directory.");
    }
}
