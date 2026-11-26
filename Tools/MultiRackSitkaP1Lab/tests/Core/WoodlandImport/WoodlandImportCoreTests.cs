using System.IO.Compression;
using System.Text;
using SitkaP1.Core.WoodlandImport;

namespace SitkaP1.Core.WoodlandImport.Tests;

internal static class WoodlandImportCoreTests
{
    private const string Suite = "synthetic";

    private static readonly LocalPoint2D[] Square100m =
    [
        new(0.0, 0.0),
        new(100.0, 0.0),
        new(100.0, 100.0),
        new(0.0, 100.0),
    ];

    public static void Register()
    {
        // 1. Positive import
        TestRunner.Add(Suite, "positive: supported mixed stand with surveyed boundary yields CandidateForSimulationIntegration", () =>
        {
            byte[] zip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1003_IPlus_Synth", "Fagus sylvatica L.", "42.5 , 61.25", "34.2", "24.5", "1", "DE", "Beech-oak", ExtraMicrohabitat: "CV21"),
                new("1001_IPlus_Synth", "Picea sitchensis", "12.0 , 18.0", "28.0", "21.0", "1", "IE", "Sitka-beech-oak"),
                new("1002_IPlus_Synth", "Quercus petraea (Matt.) Liebl.", "75.0 , 33.5", "41.5", "26.0", "1", "DE", "Beech-oak"),
                new("1004_IPlus_Synth", "Picea abies", "50.0 , 50.0", "19.0", "12.0", "0", "DE", "Beech-oak"), // dead unsupported tree must NOT block admission
            ]);

            var reader = new IntegrateMarteloscopeReader();
            WoodlandImportResult res = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = zip,
                RawLocalBoundaryVertices = Square100m,
            });

            TestRunner.Eq(WoodlandReadiness.CandidateForSimulationIntegration, res.Readiness, "readiness");
            TestRunner.True(res.IsAdmissibleForSimulation, "admissible");
            TestRunner.True(res.Stand is not null, "stand present");
            TestRunner.Eq(0, res.Issues.Count, "no issues");
            TestRunner.Eq(4, res.Stand!.Trees.Count, "all 4 trees retained including dead unsupported");
            TestRunner.Eq(3, res.Stand.LivingStemCount, "living count");
            TestRunner.Eq(1, res.Stand.DeadStemCount, "dead count");
            TestRunner.Eq(0, res.Stand.UnknownStatusStemCount, "unknown count");

            // Canonical ordinal sorting by SourceTreeId
            TestRunner.Eq("1001_IPlus_Synth", res.Stand.Trees[0].SourceTreeId, "order[0]");
            TestRunner.Eq("1002_IPlus_Synth", res.Stand.Trees[1].SourceTreeId, "order[1]");
            TestRunner.Eq("1003_IPlus_Synth", res.Stand.Trees[2].SourceTreeId, "order[2]");
            TestRunner.Eq("1004_IPlus_Synth", res.Stand.Trees[3].SourceTreeId, "order[3]");
            TestRunner.Eq("sitka-spruce", res.Stand.Trees[0].SpeciesMapping.CanonicalSpeciesId, "sitka mapped");
            TestRunner.Eq("sessile-oak", res.Stand.Trees[1].SpeciesMapping.CanonicalSpeciesId, "oak mapped");
            TestRunner.Eq("beech", res.Stand.Trees[2].SpeciesMapping.CanonicalSpeciesId, "beech mapped");
            TestRunner.Eq(SpeciesSupportCategory.UnmodelledSpecies, res.Stand.Trees[3].SpeciesMapping.Category, "dead spruce unmodelled");
            TestRunner.Eq("1", res.Stand.Trees[2].ExtraAttributes["trem:CV21"], "microhabitat preserved");
        });

        // 2. Malformed archives
        TestRunner.Add(Suite, "malformed: empty, non-zip, missing meta.xml, broken XML, missing core/ext files refused", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            var rEmpty = reader.Read(new WoodlandImportRequest { ArchiveBytes = Array.Empty<byte>() });
            TestRunner.Eq(WoodlandReadiness.Refused, rEmpty.Readiness, "empty refused");
            TestRunner.True(rEmpty.Stand is null, "empty stand null");
            TestRunner.True(rEmpty.Issues.Any(i => i.Code == "ArchiveEmpty"), "ArchiveEmpty code");

            var rGarbage = reader.Read(new WoodlandImportRequest { ArchiveBytes = "not a zip archive"u8.ToArray() });
            TestRunner.Eq(WoodlandReadiness.Refused, rGarbage.Readiness, "non-zip refused");
            TestRunner.True(rGarbage.Issues.Any(i => i.Code == "ArchiveMalformedZip"), "ArchiveMalformedZip code");

            var rNoMeta = reader.Read(new WoodlandImportRequest { ArchiveBytes = SyntheticArchiveBuilder.BuildRawZip(("occurrence.txt", "id\n1\n"u8.ToArray())) });
            TestRunner.Eq(WoodlandReadiness.Refused, rNoMeta.Readiness, "no meta refused");
            TestRunner.True(rNoMeta.Issues.Any(i => i.Code == "ArchiveMissingMetaXml"), "ArchiveMissingMetaXml code");

            var rBadXml = reader.Read(new WoodlandImportRequest { ArchiveBytes = SyntheticArchiveBuilder.BuildRawZip(("meta.xml", "<archive><core"u8.ToArray())) });
            TestRunner.Eq(WoodlandReadiness.Refused, rBadXml.Readiness, "broken xml refused");
            TestRunner.True(rBadXml.Issues.Any(i => i.Code == "ArchiveMalformedMetaXml"), "ArchiveMalformedMetaXml code");

            string meta = SyntheticArchiveBuilder.BuildDefaultMetaXml("\\t", "", includeExtension: true);
            var rMissingCore = reader.Read(new WoodlandImportRequest { ArchiveBytes = SyntheticArchiveBuilder.BuildRawZip(("meta.xml", Encoding.UTF8.GetBytes(meta))) });
            TestRunner.Eq(WoodlandReadiness.Refused, rMissingCore.Readiness, "missing core refused");
            TestRunner.True(rMissingCore.Issues.Any(i => i.Code == "ArchiveMissingCoreFile"), "ArchiveMissingCoreFile code");

            var rMissingExt = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = SyntheticArchiveBuilder.BuildRawZip(
                    ("meta.xml", Encoding.UTF8.GetBytes(meta)),
                    ("occurrence.txt", "occurrenceID\tscientificName\tverbatimCoordinates\tcountryCode\thabitat\tdatasetID\tdatasetName\n1_P\tFagus sylvatica\t10,10\tDE\th\td\tn\n"u8.ToArray()))
            });
            TestRunner.Eq(WoodlandReadiness.Refused, rMissingExt.Readiness, "missing ext refused");
            TestRunner.True(rMissingExt.Issues.Any(i => i.Code == "ArchiveMissingExtensionFile"), "ArchiveMissingExtensionFile code");
        });

        // 3. Refusal cases (duplicate IDs, bad numbers, Year-0 Simulated provenance)
        TestRunner.Add(Suite, "refusal: duplicate ID, non-positive DBH, malformed coords, and Year-0 Simulated provenance refused", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // Duplicate ID
            byte[] dupZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "10 , 10", "25", "18", "1"),
                new("1_PlotA", "Fagus sylvatica", "20 , 20", "26", "19", "1"),
            ]);
            var rDup = reader.Read(new WoodlandImportRequest { ArchiveBytes = dupZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rDup.Readiness, "duplicate refused");
            TestRunner.True(rDup.Issues.Any(i => i.Code == "DuplicateSourceTreeId"), "DuplicateSourceTreeId");

            // Non-positive DBH
            byte[] negDbhZip = SyntheticArchiveBuilder.BuildTreMsArchive([new("1_PlotA", "Fagus sylvatica", "10 , 10", "0", "18", "1")]);
            var rNegDbh = reader.Read(new WoodlandImportRequest { ArchiveBytes = negDbhZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rNegDbh.Readiness, "zero dbh refused");
            TestRunner.True(rNegDbh.Issues.Any(i => i.Code == "InvalidDbhValue"), "InvalidDbhValue");

            // Malformed coordinates
            byte[] badCoordZip = SyntheticArchiveBuilder.BuildTreMsArchive([new("1_PlotA", "Fagus sylvatica", "not-a-coord", "25", "18", "1")]);
            var rBadCoord = reader.Read(new WoodlandImportRequest { ArchiveBytes = badCoordZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rBadCoord.Readiness, "bad coord refused");
            TestRunner.True(rBadCoord.Issues.Any(i => i.Code == "MalformedTreeCoordinates"), "MalformedTreeCoordinates");

            // Year-0 Simulated provenance via archive reader
            byte[] simZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "10 , 10", "25", "18", "1", MeasurementMethodNote: "provenance=simulated"),
            ]);
            var rSim = reader.Read(new WoodlandImportRequest { ArchiveBytes = simZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rSim.Readiness, "Year-0 simulated refused");
            TestRunner.True(rSim.Stand is null, "simulated refused stand is null");
            TestRunner.True(rSim.Issues.Any(i => i.Code == "SimulatedFieldAtYearZero" && i.SourceTreeId == "1_PlotA"), "SimulatedFieldAtYearZero emitted");

            // Direct validator refusal of Year-0 Simulated even if stand is otherwise valid
            WoodlandBoundary.TryCreateLocal(Square100m, out var validBoundary, out _);
            var standWithSim = new WoodlandImportStand
            {
                StandId = "direct-sim",
                PlotName = "direct-sim",
                Attribution = new ImportSourceAttribution
                {
                    DatasetTitle = "test",
                    Publisher = "test",
                    Licence = "CC BY 4.0",
                    Citation = "test",
                },
                Boundary = validBoundary,
                Trees =
                [
                    new ImportedTreeRecord
                    {
                        SourceTreeId = "T1",
                        RawSpeciesName = ImportField<string>.Observed("Fagus sylvatica"),
                        SpeciesMapping = WoodlandSpeciesMapper.Map("Fagus sylvatica"),
                        VitalStatus = ImportField<TreeVitalStatus>.Observed(TreeVitalStatus.Alive),
                        PositionLocalM = ImportField<LocalPoint2D>.Observed(new LocalPoint2D(10, 10)),
                        DbhCm = ImportField<double>.Observed(25.0),
                        HeightM = ImportField<double>.Simulated(18.0),
                        CrownRadiusM = ImportField<double>.Missing(),
                        AgeYears = ImportField<int>.Missing(),
                    },
                ],
                LivingStemCount = 1,
            };
            var rDirectSim = WoodlandImportValidator.Validate(standWithSim, WoodlandImportOptions.Default);
            TestRunner.Eq(WoodlandReadiness.Refused, rDirectSim.Readiness, "direct validator refuses Simulated at Year 0");
            TestRunner.True(rDirectSim.Issues.Any(i => i.Code == "SimulatedFieldAtYearZero" && i.FieldName == "HeightM"), "HeightM SimulatedFieldAtYearZero");
        });

        // 4. InventoryOnly cases (Option A #1, #2, #3)
        TestRunner.Add(Suite, "inventory-only: unsupported living species retained without deletion, missing measurements never estimated, missing boundary never inferred", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // 4a. Unsupported living species -> InventoryOnly, all trees retained
            byte[] unsupZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "22", "1"),
                new("2_PlotA", "Fraxinus excelsior", "20 , 20", "25", "19", "1"),
                new("3_PlotA", "Pinus sylvestris", "30 , 30", "28", "20", "1"),
            ]);
            var rUnsup = reader.Read(new WoodlandImportRequest { ArchiveBytes = unsupZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, rUnsup.Readiness, "unsupported living -> InventoryOnly");
            TestRunner.True(!rUnsup.IsAdmissibleForSimulation, "not admissible");
            TestRunner.Eq(3, rUnsup.Stand!.Trees.Count, "zero silent deletion: all 3 retained");
            TestRunner.Eq("Fraxinus excelsior", rUnsup.Stand.Trees[1].RawSpeciesName.Value, "no species substitution");
            TestRunner.True(rUnsup.Issues.Any(i => i.Code == "UnsupportedLivingSpecies" && i.SourceTreeId == "2_PlotA"), "2_PlotA unsupported blocker");
            TestRunner.True(rUnsup.Issues.Any(i => i.Code == "PendingModelSupportLivingSpecies" && i.SourceTreeId == "3_PlotA"), "3_PlotA pending blocker");

            // 4b. Missing measurements (height missing) -> FieldProvenance.Missing, InventoryOnly, no allometric fill
            byte[] missZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", null, "1"),
            ]);
            var rMiss = reader.Read(new WoodlandImportRequest { ArchiveBytes = missZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, rMiss.Readiness, "missing height -> InventoryOnly");
            TestRunner.Eq(FieldProvenance.Missing, rMiss.Stand!.Trees[0].HeightM.Provenance, "height honestly Missing");
            TestRunner.True(!rMiss.Stand.Trees[0].HeightM.HasValue, "no allometric estimate filled");
            TestRunner.True(rMiss.Issues.Any(i => i.Code == "MissingRequiredHeight"), "MissingRequiredHeight blocker");

            // 4c. Missing boundary -> InventoryOnly, Boundary stays null (no convex-hull/bbox inference)
            byte[] okTreeZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Picea sitchensis", "10 , 10", "30", "22", "1"),
            ]);
            var rNoBoundary = reader.Read(new WoodlandImportRequest { ArchiveBytes = okTreeZip });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, rNoBoundary.Readiness, "missing boundary -> InventoryOnly");
            TestRunner.True(rNoBoundary.Stand!.Boundary is null, "boundary not inferred");
            TestRunner.True(rNoBoundary.Issues.Any(i => i.Code == "BoundaryMissing"), "BoundaryMissing blocker");

            // 4d. Invalid self-intersecting boundary supplied -> InventoryOnly (boundary rejected, not inferred)
            LocalPoint2D[] bowtie = [new(0, 0), new(50, 50), new(50, 0), new(0, 50)];
            var rBadBoundary = reader.Read(new WoodlandImportRequest { ArchiveBytes = okTreeZip, RawLocalBoundaryVertices = bowtie });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, rBadBoundary.Readiness, "invalid boundary -> InventoryOnly");
            TestRunner.True(rBadBoundary.Stand!.Boundary is null, "invalid boundary rejected");
            TestRunner.True(rBadBoundary.Issues.Any(i => i.Code == "BoundarySelfIntersecting"), "BoundarySelfIntersecting recorded");
        });

        // 5. Species mapper (all 6 target species + unmodelled + PLANTAE/genus/higher-rank/unidentified + zero fuzzy matching; F-08)
        TestRunner.Add(Suite, "species: all 6 target species, synonyms, authors, unmodelled taxa, genus/higher-rank/PLANTAE unidentified, zero fuzzy matching", () =>
        {
            void AssertMap(string input, SpeciesSupportCategory expectedCat, string? expectedId)
            {
                SpeciesMappingResult r = WoodlandSpeciesMapper.Map(input);
                TestRunner.Eq(expectedCat, r.Category, $"category for '{input}'");
                TestRunner.Eq(expectedId, r.CanonicalSpeciesId, $"id for '{input}'");
            }

            // ModelledAtBase (3)
            AssertMap("Picea sitchensis", SpeciesSupportCategory.ModelledAtBase, "sitka-spruce");
            AssertMap("PICEA SITCHENSIS (Bong.) Carrière", SpeciesSupportCategory.ModelledAtBase, "sitka-spruce");
            AssertMap("Sitka spruce", SpeciesSupportCategory.ModelledAtBase, "sitka-spruce");

            AssertMap("Quercus petraea", SpeciesSupportCategory.ModelledAtBase, "sessile-oak");
            AssertMap("Quercus petraea (Matt.) Liebl.", SpeciesSupportCategory.ModelledAtBase, "sessile-oak");
            AssertMap("Sessile Oak", SpeciesSupportCategory.ModelledAtBase, "sessile-oak");
            AssertMap("Durmast oak", SpeciesSupportCategory.ModelledAtBase, "sessile-oak");

            AssertMap("Fagus sylvatica", SpeciesSupportCategory.ModelledAtBase, "beech");
            AssertMap("Fagus sylvatica L.", SpeciesSupportCategory.ModelledAtBase, "beech");
            AssertMap("European beech", SpeciesSupportCategory.ModelledAtBase, "beech");
            AssertMap("Beech", SpeciesSupportCategory.ModelledAtBase, "beech");

            // PendingModelSupport (3)
            AssertMap("Pinus sylvestris", SpeciesSupportCategory.PendingModelSupport, "scots-pine");
            AssertMap("Pinus sylvestris L.", SpeciesSupportCategory.PendingModelSupport, "scots-pine");
            AssertMap("Scots pine", SpeciesSupportCategory.PendingModelSupport, "scots-pine");

            AssertMap("Betula pubescens", SpeciesSupportCategory.PendingModelSupport, "downy-birch");
            AssertMap("Betula pubescens Ehrh.", SpeciesSupportCategory.PendingModelSupport, "downy-birch");
            AssertMap("Downy birch", SpeciesSupportCategory.PendingModelSupport, "downy-birch");

            AssertMap("Alnus glutinosa", SpeciesSupportCategory.PendingModelSupport, "common-alder");
            AssertMap("Alnus glutinosa (L.) Gaertn.", SpeciesSupportCategory.PendingModelSupport, "common-alder");
            AssertMap("Common alder", SpeciesSupportCategory.PendingModelSupport, "common-alder");

            // Unmodelled named taxa (including near-spellings & hybrids -> zero fuzzy matching!)
            AssertMap("Picea abies (L.) H.Karst.", SpeciesSupportCategory.UnmodelledSpecies, null);
            AssertMap("Quercus robur L.", SpeciesSupportCategory.UnmodelledSpecies, null);
            AssertMap("Fraxinus excelsior", SpeciesSupportCategory.UnmodelledSpecies, null);
            AssertMap("Norway spruce", SpeciesSupportCategory.UnmodelledSpecies, null);
            AssertMap("Quercus petraea x robur", SpeciesSupportCategory.UnmodelledSpecies, null);
            AssertMap("Quercus petrae", SpeciesSupportCategory.UnmodelledSpecies, null); // typo must NOT fuzzy-match sessile-oak
            AssertMap("Fagus sylvatic", SpeciesSupportCategory.UnmodelledSpecies, null); // typo must NOT fuzzy-match beech

            // Unidentified (F-08: bare genus, genus+author, higher taxonomic ranks, PLANTAE, placeholders)
            AssertMap("PLANTAE", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Plantae", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Quercus", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Fagus", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Betula", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Fagaceae", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Pinaceae", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Fagaceae Dumort.", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Quercus L.", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Tilia L.", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("   ", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("unknown", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("Quercus sp.", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("12345", SpeciesSupportCategory.UnidentifiedSpecies, null);
            AssertMap("???", SpeciesSupportCategory.UnidentifiedSpecies, null);
        });

        // 6. Boundary shoelace area (non-rectangular), geographic projection, polygon refusals, ray casting
        TestRunner.Add(Suite, "boundary: non-rectangular shoelace area vs hand value, geographic tangent plane, invalid polygons refused, edge-inclusive ray casting", () =>
        {
            // Non-rectangular L-shaped polygon:
            // (0,0) -> (40,0) -> (40,20) -> (20,20) -> (20,50) -> (0,50)
            // Bounding box = 40 * 50 = 2000 m²; Exact shoelace area = (40*20) + (20*30) = 1400 m² = 0.14 ha.
            LocalPoint2D[] lShape =
            [
                new(0, 0),
                new(40, 0),
                new(40, 20),
                new(20, 20),
                new(20, 50),
                new(0, 50),
            ];
            TestRunner.True(WoodlandBoundary.TryCreateLocal(lShape, out WoodlandBoundary? b, out var issues), "L-shape valid");
            TestRunner.Eq(0, issues.Count, "no issues");
            TestRunner.ApproxEq(1400.0, b!.AreaM2, 1e-9, "exact shoelace m2 (not 2000 bbox)");
            TestRunner.ApproxEq(0.14, b.AreaHa, 1e-12, "exact shoelace ha");

            // Ray-casting containment checks (boundary-inclusive rule)
            TestRunner.True(b.Contains(new LocalPoint2D(10, 10)), "interior lower-left");
            TestRunner.True(b.Contains(new LocalPoint2D(10, 40)), "interior upper-left");
            TestRunner.True(b.Contains(new LocalPoint2D(30, 10)), "interior lower-right");
            TestRunner.True(!b.Contains(new LocalPoint2D(30, 35)), "concave notch is outside");
            TestRunner.True(b.Contains(new LocalPoint2D(20, 20)), "reflex vertex boundary-inclusive");
            TestRunner.True(b.Contains(new LocalPoint2D(20, 35)), "vertical edge boundary-inclusive");
            TestRunner.True(b.Contains(new LocalPoint2D(30, 20)), "horizontal edge boundary-inclusive");
            TestRunner.True(!b.Contains(new LocalPoint2D(-0.001, 10)), "outside west");

            // Invalid polygons rejected
            TestRunner.True(!WoodlandBoundary.TryCreateLocal([new(0, 0), new(10, 0)], out _, out var tooFew), "<3 vertices rejected");
            TestRunner.True(tooFew.Any(i => i.Code == "BoundaryTooFewVertices"), "BoundaryTooFewVertices");

            TestRunner.True(!WoodlandBoundary.TryCreateLocal([new(0, 0), new(10, 0), new(double.NaN, 10)], out _, out var nonFin), "NaN rejected");
            TestRunner.True(nonFin.Any(i => i.Code == "BoundaryNonFiniteVertex"), "BoundaryNonFiniteVertex");

            TestRunner.True(!WoodlandBoundary.TryCreateLocal([new(0, 0), new(10, 0), new(20, 0)], out _, out var colin), "zero area collinear rejected");
            TestRunner.True(colin.Any(i => i.Code == "BoundaryZeroArea"), "BoundaryZeroArea");

            TestRunner.True(!WoodlandBoundary.TryCreateLocal([new(0, 0), new(10, 10), new(10, 0), new(0, 10)], out _, out var bowtie), "self-intersecting rejected");
            TestRunner.True(bowtie.Any(i => i.Code == "BoundarySelfIntersecting"), "BoundarySelfIntersecting");

            // Geographic tangent-plane projection around 53.324°N, -6.745°E
            double lat0 = 53.3240;
            double lat1 = 53.3249;
            double lon0 = -6.7450;
            double lon1 = -6.7435;
            GeoPoint2D[] geoRect = [new(lat0, lon0), new(lat0, lon1), new(lat1, lon1), new(lat1, lon0)];
            TestRunner.True(WoodlandBoundary.TryCreateFromGeographic(geoRect, out WoodlandBoundary? geoB, out var geoIssues), "geo boundary valid");
            TestRunner.Eq(0, geoIssues.Count, "geo no issues");
            double meanLat = (lat0 + lat1) * 0.5;
            double mPerDeg = WoodlandBoundary.EarthMeanRadiusM * Math.PI / 180.0;
            double expectedWidth = (lon1 - lon0) * Math.Cos(meanLat * Math.PI / 180.0) * mPerDeg;
            double expectedHeight = (lat1 - lat0) * mPerDeg;
            TestRunner.ApproxEq(expectedWidth * expectedHeight, geoB!.AreaM2, 1e-6, "geo shoelace area matches tangent-plane hand calculation");
            TestRunner.Eq(BoundaryCoordinateSource.GeographicTangentPlane, geoB.CoordinateSource, "coordinate source tagged");
            TestRunner.True(geoB.ProjectionNote.Contains("cos(centroid_lat)", StringComparison.Ordinal), "projection documented");
        });

        // 7. Out-of-bounds refusal without clamping + translation-only distance preservation
        TestRunner.Add(Suite, "bounds-and-coords: out-of-bounds trees refused without clamping; translation preserves distances exactly", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // Shifted boundary: [120, 250] to [170, 300] (50x50 m)
            LocalPoint2D[] shiftedBoundary = [new(120, 250), new(170, 250), new(170, 300), new(120, 300)];
            byte[] inBoundsZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "130.25 , 265.50", "30", "22", "1"),
                new("2_PlotA", "Quercus petraea", "158.75 , 289.00", "35", "24", "1"),
            ]);

            var rOk = reader.Read(new WoodlandImportRequest { ArchiveBytes = inBoundsZip, RawLocalBoundaryVertices = shiftedBoundary });
            TestRunner.Eq(WoodlandReadiness.CandidateForSimulationIntegration, rOk.Readiness, "in-bounds valid");
            TestRunner.ApproxEq(120.0, rOk.Stand!.CoordinateOrigin!.OriginXM, 1e-12, "origin X");
            TestRunner.ApproxEq(250.0, rOk.Stand.CoordinateOrigin.OriginYM, 1e-12, "origin Y");
            LocalPoint2D p1 = rOk.Stand.Trees[0].PositionLocalM.Value;
            LocalPoint2D p2 = rOk.Stand.Trees[1].PositionLocalM.Value;
            TestRunner.ApproxEq(10.25, p1.XM, 1e-12, "1_PlotA normalized X");
            TestRunner.ApproxEq(15.50, p1.YM, 1e-12, "1_PlotA normalized Y");
            double rawDist = double.Hypot(158.75 - 130.25, 289.00 - 265.50);
            double normDist = double.Hypot(p2.XM - p1.XM, p2.YM - p1.YM);
            TestRunner.ApproxEq(rawDist, normDist, 1e-12, "pairwise distance preserved identically");

            // Two out-of-bounds trees -> Refused with exactly one stable TreeOutOfBounds issue per out-of-bounds tree
            byte[] oobZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "140 , 270", "30", "22", "1"),
                new("2_PlotA", "Fagus sylvatica", "175 , 270", "30", "22", "1"),
                new("3_PlotA", "Quercus petraea", "110 , 240", "35", "24", "1"),
            ]);
            var rOob = reader.Read(new WoodlandImportRequest { ArchiveBytes = oobZip, RawLocalBoundaryVertices = shiftedBoundary });
            TestRunner.Eq(WoodlandReadiness.Refused, rOob.Readiness, "out-of-bounds refused without clamping");
            TestRunner.True(rOob.Stand is null, "refused stand is null");
            var oobIssues = rOob.Issues.Where(i => i.Code == "TreeOutOfBounds").ToList();
            TestRunner.Eq(2, oobIssues.Count, "exactly 2 TreeOutOfBounds issues");
            TestRunner.Eq("2_PlotA", oobIssues[0].SourceTreeId, "stable sorted first oob id");
            TestRunner.Eq("3_PlotA", oobIssues[1].SourceTreeId, "stable sorted second oob id");
        });

        // 8. All four hard caps (living stems incl. unknown status, total records, measurement rows, uncompressed bytes)
        TestRunner.Add(Suite, "caps: MaxLivingStems (unknown counted as living), MaxTotalRecords, MaxMeasurementRows, MaxUncompressedBytes", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // 8a. MaxLivingStems=2 with 2 alive + 1 unknown + 2 dead -> 3 conservatively living > 2 -> Refused
            byte[] capLivingZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "10 , 10", "20", "15", "1"),
                new("2_PlotA", "Fagus sylvatica", "20 , 10", "20", "15", "1"),
                new("3_PlotA", "Fagus sylvatica", "30 , 10", "20", "15", null), // unknown status -> conservatively living!
                new("4_PlotA", "Fagus sylvatica", "40 , 10", "20", "15", "0"),  // dead
                new("5_PlotA", "Fagus sylvatica", "50 , 10", "20", "15", "0"),  // dead
            ]);
            var rCapLiving = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = capLivingZip,
                RawLocalBoundaryVertices = Square100m,
                Options = WoodlandImportOptions.Default with { MaxLivingStems = 2 },
            });
            TestRunner.Eq(WoodlandReadiness.Refused, rCapLiving.Readiness, "living cap refused");
            TestRunner.True(rCapLiving.Issues.Any(i => i.Code == "CapLivingStemsExceeded"), "CapLivingStemsExceeded");

            // Control: same archive with MaxLivingStems=3 succeeds past the cap (still InventoryOnly due to unknown status on 3_PlotA)
            var rCapLivingOk = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = capLivingZip,
                RawLocalBoundaryVertices = Square100m,
                Options = WoodlandImportOptions.Default with { MaxLivingStems = 3 },
            });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, rCapLivingOk.Readiness, "within living cap -> InventoryOnly");
            TestRunner.Eq(3, rCapLivingOk.Stand!.LivingStemCount, "3 conservatively living");
            TestRunner.Eq(1, rCapLivingOk.Stand.UnknownStatusStemCount, "1 unknown counted in living");
            TestRunner.Eq(2, rCapLivingOk.Stand.DeadStemCount, "2 dead");

            // 8b. MaxTotalRecords=3 with 5 records -> Refused
            var rCapTotal = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = capLivingZip,
                RawLocalBoundaryVertices = Square100m,
                Options = WoodlandImportOptions.Default with { MaxTotalRecords = 3 },
            });
            TestRunner.Eq(WoodlandReadiness.Refused, rCapTotal.Readiness, "total records cap refused");
            TestRunner.True(rCapTotal.Issues.Any(i => i.Code == "CapTotalRecordsExceeded"), "CapTotalRecordsExceeded");

            // 8c. MaxMeasurementRows=5 with 14 measurement rows -> Refused
            var rCapMeas = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = capLivingZip,
                RawLocalBoundaryVertices = Square100m,
                Options = WoodlandImportOptions.Default with { MaxMeasurementRows = 5 },
            });
            TestRunner.Eq(WoodlandReadiness.Refused, rCapMeas.Readiness, "measurement rows cap refused");
            TestRunner.True(rCapMeas.Issues.Any(i => i.Code == "CapMeasurementRowsExceeded"), "CapMeasurementRowsExceeded");

            // 8d. MaxUncompressedBytes cap (declared size exceeds byte cap)
            byte[] bigPayload = Encoding.ASCII.GetBytes(new string('A', 4096));
            byte[] zipBig = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "20", "15", "1")],
                extraEntries: [("padding.txt", bigPayload)],
                compression: CompressionLevel.Optimal);
            var rCapBytes = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = zipBig,
                RawLocalBoundaryVertices = Square100m,
                Options = WoodlandImportOptions.Default with { MaxUncompressedBytes = 2048 },
            });
            TestRunner.Eq(WoodlandReadiness.Refused, rCapBytes.Readiness, "uncompressed bytes cap refused");
            TestRunner.True(rCapBytes.Issues.Any(i => i.Code == "CapUncompressedBytesExceeded"), "CapUncompressedBytesExceeded");
        });

        // 9. XXE / DOCTYPE payload rejected with zero external fetch
        TestRunner.Add(Suite, "security: XXE / DOCTYPE declaration in meta.xml prohibited with zero external fetch", () =>
        {
            const string xxeMeta = """
                <?xml version="1.0" encoding="UTF-8"?>
                <!DOCTYPE archive [
                  <!ENTITY xxe SYSTEM "http://127.0.0.1:1/should-never-be-fetched">
                ]>
                <archive xmlns="http://rs.tdwg.org/dwc/text/">
                  <core encoding="UTF-8" fieldsTerminatedBy="\t" linesTerminatedBy="\n" ignoreHeaderLines="1" rowType="http://rs.tdwg.org/dwc/terms/Occurrence">
                    <files><location>occurrence.txt</location></files>
                    <id index="0" />
                    <field index="0" term="http://rs.tdwg.org/dwc/terms/occurrenceID" />
                  </core>
                </archive>
                """;

            byte[] xxeZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "20", "15", "1")],
                customMetaXml: xxeMeta);

            var reader = new IntegrateMarteloscopeReader();
            var res = reader.Read(new WoodlandImportRequest { ArchiveBytes = xxeZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, res.Readiness, "XXE refused");
            TestRunner.True(res.Stand is null, "XXE stand null");
            TestRunner.True(res.Issues.Any(i => i.Code == "ArchiveXmlDtdProhibited"), "ArchiveXmlDtdProhibited");
        });

        // 10. Determinism: two independent imports produce byte-identical canonical JSON + identical SHA-256
        TestRunner.Add(Suite, "determinism: two imports produce byte-identical canonical JSON and identical SHA-256", () =>
        {
            byte[] zip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("300_PlotA", "Quercus petraea", "55.5 , 44.25", "38.4", "25.1", "1", "DE", "Oak-beech"),
                new("100_PlotA", "Fagus sylvatica", "12.0 , 19.75", "29.0", "21.5", "1", "DE", "Oak-beech"),
                new("200_PlotA", "Picea sitchensis", "80.0 , 70.0", "31.2", "23.0", "1", "DE", "Oak-beech"),
            ], fieldDelimiter: ",", quoteChar: "\"");

            var reader = new IntegrateMarteloscopeReader();
            var req = new WoodlandImportRequest { ArchiveBytes = zip, RawLocalBoundaryVertices = Square100m };

            WoodlandImportResult r1 = reader.Read(req);
            WoodlandImportResult r2 = reader.Read(req);

            byte[] j1 = WoodlandImportSerializer.SerializeResultUtf8(r1);
            byte[] j2 = WoodlandImportSerializer.SerializeResultUtf8(r2);
            string h1 = WoodlandImportSerializer.ComputeResultSha256Hex(r1);
            string h2 = WoodlandImportSerializer.ComputeResultSha256Hex(r2);

            TestRunner.Eq(WoodlandReadiness.CandidateForSimulationIntegration, r1.Readiness, "readiness");
            TestRunner.True(j1.AsSpan().SequenceEqual(j2), "byte-identical canonical JSON");
            TestRunner.Eq(h1, h2, "identical SHA-256");
            Console.WriteLine($"  [determinism] synthetic-canonical-sha256={h1} ({j1.Length} bytes)");
        });

        // 11. Production source hygiene (protects P1 #644 B-01 no-file-API rule and zero wall-clock calls)
        TestRunner.Add(Suite, "hygiene: src/Core/WoodlandImport contains zero banned file APIs and zero wall-clock calls", () =>
        {
            string? dir = FindWoodlandImportSourceDir();
            TestRunner.True(dir is not null, "located src/Core/WoodlandImport");

            string[] bannedFileApis =
            [
                "File.Write", "File.Create", "File.Open", "File.Append", "File.Copy", "File.Move", "File.Delete",
                "File.Replace", "File.ReadAll", "FileStream", "Directory.Create", "Directory.Delete", "Directory.Move", "StreamWriter",
            ];
            string[] bannedClockApis =
            [
                "DateTime.Now", "DateTime.UtcNow", "DateTimeOffset.Now", "DateTimeOffset.UtcNow", "Guid.NewGuid", "Random.Shared",
            ];

            string[] files = Directory.GetFiles(dir!, "*.cs", SearchOption.AllDirectories);
            TestRunner.True(files.Length >= 8, "at least 8 production source files");

            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                foreach (string b in bannedFileApis)
                {
                    TestRunner.True(!content.Contains(b, StringComparison.Ordinal), $"{Path.GetFileName(file)} must not reference '{b}'");
                }
                foreach (string b in bannedClockApis)
                {
                    TestRunner.True(!content.Contains(b, StringComparison.Ordinal), $"{Path.GetFileName(file)} must not reference '{b}'");
                }
            }
        });

        // ------------------------------------------------------------------
        // Repair Cycle 1 Regression Tests (F-01 .. F-07, F-11, F-16)
        // ------------------------------------------------------------------

        // F-01: Orphan extension rows & blank extension coreid refused (never silently dropped)
        TestRunner.Add(Suite, "F-01: orphan extension rows and blank coreid extension rows are refused deterministically", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // Orphan coreid "GHOST_PlotA" with 3 measurement rows
            byte[] orphanZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1")],
                extraMofRows:
                [
                    new("GHOST_PlotA", "Tree diameter", "diameter", "40", "cm", "field"),
                    new("GHOST_PlotA", "tree height", "height", "25", "m", "field"),
                    new("GHOST_PlotA", "Tree status (dead or living)", "status", "1", "", "field"),
                ]);
            var rOrphan = reader.Read(new WoodlandImportRequest { ArchiveBytes = orphanZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rOrphan.Readiness, "orphan extension refused");
            TestRunner.True(rOrphan.Stand is null, "orphan stand null");
            var orphanIssue = rOrphan.Issues.FirstOrDefault(i => i.Code == "ArchiveOrphanExtensionRow");
            TestRunner.True(orphanIssue is not null, "ArchiveOrphanExtensionRow emitted");
            TestRunner.Eq("GHOST_PlotA", orphanIssue!.SourceTreeId, "orphan SourceTreeId recorded");
            TestRunner.True(orphanIssue.Message.Contains("3 extension row(s)", StringComparison.Ordinal), "orphan count recorded");

            // Blank coreid extension row
            byte[] blankKeyZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1")],
                extraMofRows:
                [
                    new("", "Tree diameter", "diameter", "77", "cm", "field"),
                ]);
            var rBlank = reader.Read(new WoodlandImportRequest { ArchiveBytes = blankKeyZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rBlank.Readiness, "blank coreid extension refused");
            TestRunner.True(rBlank.Issues.Any(i => i.Code == "ArchiveBlankExtensionCoreId"), "ArchiveBlankExtensionCoreId emitted");
        });

        // F-02: Repeated/conflicting measurement rows refused deterministically (order-independent)
        TestRunner.Add(Suite, "F-02: repeated/conflicting measurement rows for same tree refused identically regardless of row order", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            byte[] zipOrder1 = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1")],
                extraMofRows: [new("1_PlotA", "Tree diameter", "diameter", "55", "cm", "field")]);

            byte[] zipOrder2 = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "55", "20", "1")],
                extraMofRows: [new("1_PlotA", "Tree diameter", "diameter", "30", "cm", "field")]);

            var r1 = reader.Read(new WoodlandImportRequest { ArchiveBytes = zipOrder1, RawLocalBoundaryVertices = Square100m });
            var r2 = reader.Read(new WoodlandImportRequest { ArchiveBytes = zipOrder2, RawLocalBoundaryVertices = Square100m });

            TestRunner.Eq(WoodlandReadiness.Refused, r1.Readiness, "order 1 refused");
            TestRunner.Eq(WoodlandReadiness.Refused, r2.Readiness, "order 2 refused");
            TestRunner.True(r1.Issues.Any(i => i.Code == "DuplicateMeasurementRow" && i.FieldName == "DbhCm"), "DuplicateMeasurementRow on DbhCm");
            TestRunner.Eq(
                WoodlandImportSerializer.ComputeResultSha256Hex(r1),
                WoodlandImportSerializer.ComputeResultSha256Hex(r2),
                "conflict refusal JSON identical regardless of row order");

            // Duplicate TreM code also refused
            byte[] dupTremZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1", ExtraMicrohabitat: "CV21")],
                extraMofRows: [new("1_PlotA", "Tree related micro-habitat", "CV21", "2", "", "field")]);
            var rTrem = reader.Read(new WoodlandImportRequest { ArchiveBytes = dupTremZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rTrem.Readiness, "duplicate TreM refused");
            TestRunner.True(rTrem.Issues.Any(i => i.Code == "DuplicateMeasurementRow" && i.FieldName == "trem:CV21"), "DuplicateMeasurementRow on trem:CV21");
        });

        // F-03: Lying ZIP uncompressed-size header triggers streamed-bytes cap OR size-mismatch refusal (never silently truncates)
        TestRunner.Add(Suite, "F-03: lying ZIP uncompressed-size header refused via streamed cap or ArchiveEntrySizeMismatch without silent truncation", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // Build a deflated archive whose occurrence.txt is ~10 KB (150 rows), then patch occurrence.txt's declared size to 100 bytes.
            var manyTrees = new List<SyntheticArchiveBuilder.SyntheticTreeSpec>();
            for (int i = 1; i <= 150; i++)
            {
                manyTrees.Add(new($"{i}_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1"));
            }
            byte[] honestZip = SyntheticArchiveBuilder.BuildTreMsArchive(manyTrees, compression: CompressionLevel.Optimal);
            byte[] lyingZip = SyntheticArchiveBuilder.PatchZipDeclaredUncompressedSize(honestZip, "occurrence.txt", fakeUncompressedSize: 100u);

            // Case A: cap is above actual decompressed size -> streamed decompression completes and catches ArchiveEntrySizeMismatch!
            var rMismatch = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = lyingZip,
                RawLocalBoundaryVertices = Square100m,
                Options = WoodlandImportOptions.Default with { MaxUncompressedBytes = 200_000 },
            });
            TestRunner.Eq(WoodlandReadiness.Refused, rMismatch.Readiness, "lying header refused (not truncated)");
            TestRunner.True(rMismatch.Stand is null, "lying header stand null");
            TestRunner.True(rMismatch.Issues.Any(i => i.Code == "ArchiveEntrySizeMismatch"), "ArchiveEntrySizeMismatch emitted");

            // Case B: cap is below actual decompressed size (4096 B) while declared header lies (100 B) -> streamed-bytes cap triggers!
            var rStreamCap = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = lyingZip,
                RawLocalBoundaryVertices = Square100m,
                Options = WoodlandImportOptions.Default with { MaxUncompressedBytes = 4096 },
            });
            TestRunner.Eq(WoodlandReadiness.Refused, rStreamCap.Readiness, "lying header exceeding streamed cap refused");
            TestRunner.True(rStreamCap.Issues.Any(i => i.Code == "CapUncompressedBytesExceeded"), "streamed CapUncompressedBytesExceeded emitted");
        });

        // F-04: Multi-plot archive without TargetPlotId is refused; with TargetPlotId selects one plot cleanly
        TestRunner.Add(Suite, "F-04: multi-plot archive without TargetPlotId is refused; specifying TargetPlotId succeeds for that plot", () =>
        {
            var reader = new IntegrateMarteloscopeReader();
            byte[] multiPlotZip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1"),
                new("2_PlotB", "Quercus petraea", "10 , 10", "35", "22", "1"),
            ]);

            var rNoTarget = reader.Read(new WoodlandImportRequest { ArchiveBytes = multiPlotZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rNoTarget.Readiness, "multi-plot without target refused");
            TestRunner.True(rNoTarget.Stand is null, "multi-plot stand null");
            TestRunner.True(rNoTarget.Issues.Any(i => i.Code == "MultiplePlotsRequireTargetPlotId"), "MultiplePlotsRequireTargetPlotId emitted");

            var rPlotA = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = multiPlotZip,
                TargetPlotId = "PlotA",
                RawLocalBoundaryVertices = Square100m,
            });
            TestRunner.Eq(WoodlandReadiness.CandidateForSimulationIntegration, rPlotA.Readiness, "targeted PlotA succeeds");
            TestRunner.Eq(1, rPlotA.Stand!.Trees.Count, "only PlotA tree selected");
            TestRunner.Eq("1_PlotA", rPlotA.Stand.Trees[0].SourceTreeId, "selected PlotA tree id");
        });

        // F-05: Geographic boundary combined with non-georeferenced plot-local trees is NOT silently admitted
        TestRunner.Add(Suite, "F-05: geographic boundary combined with non-georeferenced local trees yields InventoryOnly with BoundaryTreeFrameMismatch", () =>
        {
            var reader = new IntegrateMarteloscopeReader();
            byte[] zip = SyntheticArchiveBuilder.BuildTreMsArchive(
            [
                new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1"),
                new("2_PlotA", "Quercus petraea", "20 , 20", "35", "22", "1"),
            ]);

            GeoPoint2D[] geoRect =
            [
                new(53.3240, -6.7450),
                new(53.3240, -6.7435),
                new(53.3249, -6.7435),
                new(53.3249, -6.7450),
            ];

            var res = reader.Read(new WoodlandImportRequest
            {
                ArchiveBytes = zip,
                RawGeoBoundaryVertices = geoRect,
            });

            TestRunner.Eq(WoodlandReadiness.InventoryOnly, res.Readiness, "unregistered geo boundary + local trees -> InventoryOnly");
            TestRunner.True(!res.IsAdmissibleForSimulation, "not admissible");
            TestRunner.True(res.Stand!.Boundary is null, "unregistered boundary withheld");
            TestRunner.Eq(2, res.Stand.Trees.Count, "all trees retained in inventory");
            TestRunner.True(res.Issues.Any(i => i.Code == "BoundaryTreeFrameMismatch"), "BoundaryTreeFrameMismatch emitted");
        });

        // F-06: Provenance preserved honestly (no Inferred->Observed laundering; core Simulated applies to species/position)
        TestRunner.Add(Suite, "F-06: unknown status preserves Inferred provenance; core provenance=simulated applies to species and position and refuses", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // F-06a: unrecognized status "7" with provenance=inferred stays FieldProvenance.Inferred
            byte[] inferredStatusZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "7", MeasurementMethodNote: "provenance=inferred")]);
            var rInf = reader.Read(new WoodlandImportRequest { ArchiveBytes = inferredStatusZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.InventoryOnly, rInf.Readiness, "unknown inferred status -> InventoryOnly");
            TestRunner.Eq(FieldProvenance.Inferred, rInf.Stand!.Trees[0].VitalStatus.Provenance, "Inferred provenance NOT laundered to Observed");
            TestRunner.Eq(TreeVitalStatus.Unknown, rInf.Stand.Trees[0].VitalStatus.Value, "status value Unknown");

            // F-06b: core-only row with provenance=simulated applies to RawSpeciesName and PositionLocalM -> Refused!
            byte[] coreSimZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", null, null, null, CoreProvenance: "simulated")],
                includeExtension: false,
                includeCoreProvenanceColumn: true);
            var rCoreSim = reader.Read(new WoodlandImportRequest { ArchiveBytes = coreSimZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rCoreSim.Readiness, "core provenance=simulated refused");
            TestRunner.True(rCoreSim.Stand is null, "core simulated stand null");
            TestRunner.True(rCoreSim.Issues.Any(i => i.Code == "SimulatedFieldAtYearZero" && i.FieldName == "RawSpeciesName"), "RawSpeciesName SimulatedFieldAtYearZero");
            TestRunner.True(rCoreSim.Issues.Any(i => i.Code == "SimulatedFieldAtYearZero" && i.FieldName == "PositionLocalM"), "PositionLocalM SimulatedFieldAtYearZero");
        });

        // F-07: Unknown measurement units refused (never silently passed through as cm/m)
        TestRunner.Add(Suite, "F-07: unknown measurement unit (inch) refused with UnsupportedMeasurementUnit", () =>
        {
            var reader = new IntegrateMarteloscopeReader();
            byte[] inchZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", null, "20", "1")],
                extraMofRows: [new("1_PlotA", "Tree diameter", "diameter", "12", "inch", "field")]);

            var res = reader.Read(new WoodlandImportRequest { ArchiveBytes = inchZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, res.Readiness, "unsupported unit refused");
            TestRunner.True(res.Stand is null, "stand null on unsupported unit");
            TestRunner.True(res.Issues.Any(i => i.Code == "UnsupportedMeasurementUnit" && i.FieldName == "DbhCm"), "UnsupportedMeasurementUnit on DbhCm");
        });

        // F-11: Negative ignoreHeaderLines and leading blank lines before header refused explicitly
        TestRunner.Add(Suite, "F-11: negative ignoreHeaderLines and leading blank line before header refused with format codes", () =>
        {
            var reader = new IntegrateMarteloscopeReader();

            // Negative ignoreHeaderLines
            string negMeta = SyntheticArchiveBuilder.BuildDefaultMetaXml("\\t", "", includeExtension: true, ignoreHeaderLines: "-1");
            byte[] negZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1")],
                customMetaXml: negMeta);
            var rNeg = reader.Read(new WoodlandImportRequest { ArchiveBytes = negZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rNeg.Readiness, "negative ignoreHeaderLines refused");
            TestRunner.True(rNeg.Issues.Any(i => i.Code == "ArchiveMalformedMetaXml"), "ArchiveMalformedMetaXml emitted");

            // Leading blank line in occurrence.txt
            byte[] blankLeadZip = SyntheticArchiveBuilder.BuildTreMsArchive(
                [new("1_PlotA", "Fagus sylvatica", "10 , 10", "30", "20", "1")],
                corePrefix: "\n");
            var rLead = reader.Read(new WoodlandImportRequest { ArchiveBytes = blankLeadZip, RawLocalBoundaryVertices = Square100m });
            TestRunner.Eq(WoodlandReadiness.Refused, rLead.Readiness, "leading blank line refused");
            TestRunner.True(rLead.Issues.Any(i => i.Code == "ArchiveMalformedHeader"), "ArchiveMalformedHeader emitted");
        });
    }

    private static string? FindWoodlandImportSourceDir()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null)
        {
            string candidate = Path.Combine(d.FullName, "src", "Core", "WoodlandImport");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
            d = d.Parent;
        }
        return null;
    }
}
