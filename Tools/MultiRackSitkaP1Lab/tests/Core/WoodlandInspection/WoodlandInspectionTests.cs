using System.Collections;
using System.Text;
using System.Text.RegularExpressions;
using SitkaP1.Core.WoodlandImport;
using SitkaP1.Core.WoodlandImport.Inspection;

namespace SitkaP1.Core.WoodlandImport.Inspection.Tests;

internal static class WoodlandInspectionTests
{
    private const string Catalogue = "catalogue";
    private const string Projection = "projection";
    private const string Determinism = "determinism";
    private const string Synthetic = "synthetic";
    private const string Hygiene = "hygiene";

    private const string DonadeaPlotId = "IPlus_Donadea_MHF_2";
    private const string MerschPlotId = "IPlus_Mersch";

    // Golden SHA-256 of the canonical JSON of each built-in stand's InspectionView (no boundary, as shipped,
    // with its source disclosures). Observed on SDK 8.0.133 on 2026-11-27 and pinned; any change to the view
    // shape, wording or fixture data changes them.
    // Repair R1 re-pin: Donadea 2649a712... -> b53c6339..., Mersch 00d9f760... -> e01bbfad..., because the JSON
    // gained the "disclosures" object (WI-02) and the BoundaryMissing title/explanation were reworded (WI-05).
    private const string GoldenDonadeaViewSha256 = "b53c6339242f6a0abc22a5292a042920fd0dee17d595f3a567782abd99babeb6";
    private const string GoldenMerschViewSha256 = "e01bbfada73ab2260300ff4f5e092b1d537e76ac96a72ecab3df1d9ef1d0b9a4";

    public static void Register()
    {
        RegisterCatalogueTests();
        RegisterProjectionTests();
        RegisterDeterminismTests();
        RegisterSyntheticTests();
        RegisterHygieneTests();
    }

    // ------------------------------------------------------------------
    // 1, 2: catalogue
    // ------------------------------------------------------------------
    private static void RegisterCatalogueTests()
    {
        TestRunner.Add(Catalogue, "T1 order and metadata: exactly Donadea then Mersch, offline, 20 trees, IE/LU, InventoryOnly, CC BY 4.0, pinned SHA, full citation", () =>
        {
            IReadOnlyList<BuiltInWoodlandEntry> entries = TestFixtures.LoadCatalogue();
            string attributionJson = Encoding.UTF8.GetString(TestFixtures.ReadFixture(BuiltInWoodlandCatalogue.AttributionFileName));
            using var doc = System.Text.Json.JsonDocument.Parse(attributionJson);
            string fullCitation = doc.RootElement.GetProperty("citation").GetString()!;
            string changesMade = doc.RootElement.GetProperty("changesMade").GetString()!;

            TestRunner.Eq(2, entries.Count, "exactly two built-in woodlands");
            TestRunner.Eq(2, BuiltInWoodlandCatalogue.Definitions.Count, "exactly two definitions");
            TestRunner.Eq(DonadeaPlotId, entries[0].PlotId, "Donadea first");
            TestRunner.Eq(MerschPlotId, entries[1].PlotId, "Mersch second");
            TestRunner.Eq("IE", entries[0].CountryCode, "Donadea IE");
            TestRunner.Eq("LU", entries[1].CountryCode, "Mersch LU");
            TestRunner.Eq("Donadea MHF 2 plot (Ireland)", entries[0].DisplayName, "Donadea display name");
            TestRunner.Eq("Mersch oak-beech plot (Luxembourg)", entries[1].DisplayName, "Mersch display name");
            TestRunner.Eq("20 of 271 GBIF records for this plot", entries[0].SubsetOfGbifRecords, "Donadea subset text");
            TestRunner.Eq("20 of 134 GBIF records for this plot", entries[1].SubsetOfGbifRecords, "Mersch subset text");

            foreach (BuiltInWoodlandEntry e in entries)
            {
                TestRunner.True(e.ImportResult.Stand is not null, $"{e.PlotId} stand present");
                WoodlandImportStand stand = e.ImportResult.Stand!;
                TestRunner.Eq(20, e.TreeCount, $"{e.PlotId} 20 trees");
                TestRunner.Eq(20, stand.Trees.Count, $"{e.PlotId} stand trees");
                TestRunner.Eq(e.CountryCode, stand.CountryCode, $"{e.PlotId} stand country");
                TestRunner.Eq(WoodlandReadiness.InventoryOnly, e.Readiness, $"{e.PlotId} InventoryOnly");
                TestRunner.Eq("CC BY 4.0", e.LicenceSummary, $"{e.PlotId} licence summary");
                TestRunner.Eq("http://creativecommons.org/licenses/by/4.0/legalcode", e.LicenceUrl, $"{e.PlotId} licence URL (as in ATTRIBUTION.json)");
                TestRunner.Eq(e.Definition.ArchiveSha256, stand.Attribution.ArchiveSha256, $"{e.PlotId} computed archive SHA equals pinned");
                TestRunner.Eq(e.Definition.ArchiveSha256, stand.Attribution.DeclaredArchiveSha256, $"{e.PlotId} declared SHA verified");
                TestRunner.Eq(e.Definition.ArchiveSha256, e.ArchiveSha256, $"{e.PlotId} entry SHA");
                TestRunner.Eq(fullCitation, e.Citation, $"{e.PlotId} full citation on entry");
                TestRunner.Eq(fullCitation, stand.Attribution.Citation, $"{e.PlotId} full citation carried into import");
                TestRunner.Eq("10.15468/ocof3v", e.Doi, $"{e.PlotId} DOI");
                TestRunner.Eq("2e102194-f384-4712-89a4-5db7a3fc409a", e.DatasetKey, $"{e.PlotId} dataset key");
                TestRunner.Eq("2026-11-26", e.RetrievalDateUtc, $"{e.PlotId} retrieval date");
                TestRunner.Eq(e.PlotId, stand.Attribution.SourcePlotId, $"{e.PlotId} source plot id");
                TestRunner.Eq(changesMade, e.ModificationsDisclosure, $"{e.PlotId} modifications disclosure verbatim");
                TestRunner.True(e.Caveats.Any(c => c.Contains("unverified inference", StringComparison.Ordinal)), $"{e.PlotId} coordinate caveat present");
                TestRunner.True(e.Caveats.Contains(BuiltInWoodlandCatalogue.NoBoundaryCaveat), $"{e.PlotId} no-boundary caveat present");
                TestRunner.True(stand.Boundary is null, $"{e.PlotId} no boundary was passed or fabricated");
                TestRunner.Eq(stand.LivingStemCount, e.LivingStemCount, $"{e.PlotId} living count");
            }

            TestRunner.Eq(18, entries[0].LivingStemCount, "Donadea 18 living");
            TestRunner.Eq(17, entries[1].LivingStemCount, "Mersch 17 living");
            TestRunner.Eq("2019", string.Join(",", entries[0].ObservationDates), "Donadea observation date");
            TestRunner.Eq("2020", string.Join(",", entries[1].ObservationDates), "Mersch observation date");
        });

        TestRunner.Add(Catalogue, "T2 tampered archive bytes (one byte flipped in memory) are Refused and cannot advance time", () =>
        {
            string target = BuiltInWoodlandCatalogue.Definitions[0].FixtureFileName;

            // Offset 10 is the first local header's modification time: the ZIP still opens, only the hash changes.
            foreach (int offset in new[] { 10, 1000 })
            {
                byte[] Read(string name)
                {
                    byte[] bytes = TestFixtures.ReadFixture(name);
                    if (name == target)
                    {
                        bytes[offset] ^= 0x01;
                    }
                    return bytes;
                }

                IReadOnlyList<BuiltInWoodlandEntry> entries = BuiltInWoodlandCatalogue.Load(Read);
                BuiltInWoodlandEntry tampered = entries[0];
                TestRunner.Eq(WoodlandReadiness.Refused, tampered.Readiness, $"offset {offset}: tampered Donadea refused");
                TestRunner.Eq(0, tampered.TreeCount, $"offset {offset}: no trees");

                WoodlandInspectionView view = WoodlandInspectionProjector.Project(tampered.ImportResult);
                TestRunner.True(!view.CanAdvanceTime, $"offset {offset}: CanAdvanceTime false");
                TestRunner.Eq(0, view.ObservedYear0.Trees.Count, $"offset {offset}: no trees in view");
                TestRunner.True(view.BlockerChecklist.Count > 0 && view.BlockerChecklist[0].Severity == IssueSeverity.Refusal, $"offset {offset}: refusal listed first");
                if (offset == 10)
                {
                    TestRunner.True(view.BlockerChecklist.Any(b => b.Code == "ArchiveSha256Mismatch"), "header-byte flip refused by the pinned SHA-256 check");
                }
                else
                {
                    // Offset 1000 lies inside compressed entry data, so decompression itself fails.
                    Console.WriteLine($"  [tamper] offset {offset} codes={string.Join(",", view.BlockerChecklist.Select(b => b.Code))}");
                    TestRunner.True(view.BlockerChecklist.Any(b => b.Code == "ArchiveCorruptEntry"), "data-byte flip refused as ArchiveCorruptEntry");
                    BlockerChecklistItem corrupt = view.BlockerChecklist.Single(b => b.Code == "ArchiveCorruptEntry");
                    TestRunner.True(BlockerExplanations.IsKnown("ArchiveCorruptEntry"), "ArchiveCorruptEntry has plain-English text");
                    TestRunner.Eq("Archive contains a damaged file", corrupt.Title, "ArchiveCorruptEntry title is not the raw code");
                    TestRunner.True(!corrupt.Explanation.Contains("failed decompression", StringComparison.Ordinal), "explanation is not the raw importer message");
                }

                TestRunner.Eq(WoodlandReadiness.InventoryOnly, entries[1].Readiness, $"offset {offset}: untouched Mersch unaffected");
            }
        });

        TestRunner.Add(Catalogue, "T2b ATTRIBUTION.json disagreeing with a pinned SHA-256 is rejected before import", () =>
        {
            byte[] Read(string name)
            {
                byte[] bytes = TestFixtures.ReadFixture(name);
                if (name == BuiltInWoodlandCatalogue.AttributionFileName)
                {
                    string text = Encoding.UTF8.GetString(bytes)
                        .Replace("4ee2f4a125d1a06474d7086ccf5a6ca96735e16124ad3e4f9a11647ddcee853d", "0000000000000000000000000000000000000000000000000000000000000000", StringComparison.Ordinal);
                    return Encoding.UTF8.GetBytes(text);
                }
                return bytes;
            }

            bool threw = false;
            try
            {
                BuiltInWoodlandCatalogue.Load(Read);
            }
            catch (InvalidDataException)
            {
                threw = true;
            }
            TestRunner.True(threw, "InvalidDataException for mismatched ATTRIBUTION SHA-256");
        });

        TestRunner.Add(Catalogue, "T1b built-in view and its JSON carry the CC BY 4.0 modifications disclosure and the verbatimCoordinates caveat; importer fields unchanged", () =>
        {
            string attributionJson = Encoding.UTF8.GetString(TestFixtures.ReadFixture(BuiltInWoodlandCatalogue.AttributionFileName));
            using var attribution = System.Text.Json.JsonDocument.Parse(attributionJson);
            string changesMade = attribution.RootElement.GetProperty("changesMade").GetString()!;
            string expectedCaveat = changesMade.Substring(changesMade.IndexOf("Note:", StringComparison.Ordinal)).Trim();

            foreach (BuiltInWoodlandEntry e in TestFixtures.LoadCatalogue())
            {
                WoodlandInspectionView view = e.InspectionView;
                TestRunner.True(view.Disclosures is not null, $"{e.PlotId} view has disclosures");
                TestRunner.Eq(changesMade, view.Disclosures!.ModificationsDisclosure, $"{e.PlotId} modifications disclosure verbatim on the view");
                TestRunner.Eq(expectedCaveat, view.Disclosures.CoordinateInterpretationCaveat, $"{e.PlotId} coordinate caveat on the view");
                TestRunner.True(view.Disclosures.CoordinateInterpretationCaveat.Contains("verbatimCoordinates", StringComparison.Ordinal) &&
                    view.Disclosures.CoordinateInterpretationCaveat.Contains("unverified inference", StringComparison.Ordinal), $"{e.PlotId} caveat names the unverified verbatimCoordinates inference");
                TestRunner.Eq(e.Disclosures, view.Disclosures, $"{e.PlotId} entry and view disclosures agree");

                // A reader of the JSON alone sees both disclosures.
                using var json = System.Text.Json.JsonDocument.Parse(WoodlandInspectionSerializer.SerializeUtf8(view));
                System.Text.Json.JsonElement d = json.RootElement.GetProperty("disclosures");
                TestRunner.Eq(changesMade, d.GetProperty("modificationsDisclosure").GetString(), $"{e.PlotId} JSON modificationsDisclosure");
                TestRunner.Eq(expectedCaveat, d.GetProperty("coordinateInterpretationCaveat").GetString(), $"{e.PlotId} JSON coordinateInterpretationCaveat");

                // Provenance labels and raw evidence stay exactly as the importer produced them.
                System.Text.Json.JsonElement firstTree = json.RootElement.GetProperty("observedYear0").GetProperty("trees")[0];
                ImportedTreeRecord src = e.ImportResult.Stand!.Trees[0];
                TestRunner.Eq(src.PositionLocalM.Provenance.ToString(), firstTree.GetProperty("positionLocalM").GetProperty("provenance").GetString(), $"{e.PlotId} position provenance not relabelled");
                TestRunner.Eq(src.PositionLocalM.Evidence?.RawValue, firstTree.GetProperty("positionLocalM").GetProperty("rawValue").GetString(), $"{e.PlotId} raw position evidence unchanged");

                // The plain projection of the same result is identical apart from the disclosures it was given.
                byte[] withDisclosures = WoodlandInspectionSerializer.SerializeUtf8(WoodlandInspectionProjector.Project(e.ImportResult, e.Disclosures));
                TestRunner.True(withDisclosures.AsSpan().SequenceEqual(WoodlandInspectionSerializer.SerializeUtf8(view)), $"{e.PlotId} Project(result, disclosures) reproduces the built-in view");
                TestRunner.True(WoodlandInspectionProjector.Project(e.ImportResult).Disclosures is null, $"{e.PlotId} no disclosures unless supplied");
            }

            // A built-in catalogue without the coordinate caveat is rejected rather than shown without it.
            byte[] ReadWithoutNote(string name)
            {
                byte[] bytes = TestFixtures.ReadFixture(name);
                return name == BuiltInWoodlandCatalogue.AttributionFileName
                    ? Encoding.UTF8.GetBytes(Encoding.UTF8.GetString(bytes).Replace(" Note: interpreting", " interpreting", StringComparison.Ordinal))
                    : bytes;
            }
            bool threw = false;
            try
            {
                BuiltInWoodlandCatalogue.Load(ReadWithoutNote);
            }
            catch (InvalidDataException)
            {
                threw = true;
            }
            TestRunner.True(threw, "InvalidDataException when ATTRIBUTION.json lacks the coordinate caveat");
        });
    }

    // ------------------------------------------------------------------
    // 4, 7, 8, 9, 10 (real fixtures)
    // ------------------------------------------------------------------
    private static void RegisterProjectionTests()
    {
        TestRunner.Add(Projection, "T4 blocker checklists: Donadea has BoundaryMissing + UnsupportedLivingSpecies naming unmodelled species; Mersch is exactly [BoundaryMissing]", () =>
        {
            IReadOnlyList<BuiltInWoodlandEntry> entries = TestFixtures.LoadCatalogue();
            WoodlandInspectionView ie = WoodlandInspectionProjector.Project(entries[0].ImportResult);
            WoodlandInspectionView lu = WoodlandInspectionProjector.Project(entries[1].ImportResult);

            TestRunner.True(!ie.CanAdvanceTime, "IE cannot advance time");
            TestRunner.True(!lu.CanAdvanceTime, "LU cannot advance time");

            string ieCodes = string.Join(",", ie.BlockerChecklist.Select(b => b.Code));
            Console.WriteLine($"  [checklist] ie-donadea codes={ieCodes}");
            TestRunner.Eq("BoundaryMissing,UnsupportedLivingSpecies", ieCodes, "IE checklist codes in order");

            BlockerChecklistItem boundary = ie.BlockerChecklist[0];
            TestRunner.Eq(IssueSeverity.InventoryOnlyBlocker, boundary.Severity, "BoundaryMissing severity");
            TestRunner.Eq("No boundary supplied", boundary.Title, "BoundaryMissing title");
            TestRunner.True(boundary.Explanation.StartsWith("No boundary polygon was supplied", StringComparison.Ordinal), "BoundaryMissing says none was supplied");
            TestRunner.Eq(0, boundary.AffectedTreeCount, "BoundaryMissing affects the stand, not trees");

            BlockerChecklistItem species = ie.BlockerChecklist[1];
            WoodlandImportStand ieStand = entries[0].ImportResult.Stand!;
            var expectedIds = ieStand.Trees
                .Where(t => t.IsConservativelyLiving && t.SpeciesMapping.Category == SpeciesSupportCategory.UnmodelledSpecies)
                .Select(t => t.SourceTreeId)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToList();
            var expectedNames = ieStand.Trees
                .Where(t => t.IsConservativelyLiving && t.SpeciesMapping.Category == SpeciesSupportCategory.UnmodelledSpecies)
                .Select(t => t.SpeciesMapping.RawName)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(n => n, StringComparer.Ordinal)
                .ToList();
            Console.WriteLine($"  [checklist] ie-donadea unmodelled living species={string.Join(" | ", species.AffectedRawSpeciesNames)}");
            TestRunner.True(expectedNames.Count > 0, "IE has living unmodelled species");
            TestRunner.Eq(string.Join("|", expectedIds), string.Join("|", species.AffectedTreeIds), "UnsupportedLivingSpecies tree ids");
            TestRunner.Eq(expectedIds.Count, species.AffectedTreeCount, "UnsupportedLivingSpecies tree count");
            TestRunner.Eq(string.Join("|", expectedNames), string.Join("|", species.AffectedRawSpeciesNames), "UnsupportedLivingSpecies names");
            TestRunner.Eq("Crataegus monogyna|Fraxinus excelsior|Picea abies|Quercus robur|Sorbus aucuparia|Ulmus glabra",
                string.Join("|", species.AffectedRawSpeciesNames), "pinned unmodelled living species observed in the Donadea subset");
            TestRunner.True(!species.AffectedRawSpeciesNames.Contains("Fagus sylvatica"), "modelled beech is not named as unsupported");
            TestRunner.True(species.Explanation.Contains("none is deleted", StringComparison.Ordinal), "plain-English explanation");

            TestRunner.Eq(1, lu.BlockerChecklist.Count, "LU exactly one blocker");
            TestRunner.Eq("BoundaryMissing", lu.BlockerChecklist[0].Code, "LU BoundaryMissing");
            TestRunner.Eq(0, ie.Notes.Count + lu.Notes.Count, "no non-blocking notes on either fixture");
        });

        TestRunner.Add(Projection, "T7 provenance preservation: every projected field equals its source record; no fabricated height/age/crown; no Simulated provenance in JSON", () =>
        {
            foreach (BuiltInWoodlandEntry e in TestFixtures.LoadCatalogue())
            {
                WoodlandImportStand stand = e.ImportResult.Stand!;
                WoodlandInspectionView view = WoodlandInspectionProjector.Project(e.ImportResult);
                TestRunner.Eq(stand.Trees.Count, view.ObservedYear0.Trees.Count, $"{e.PlotId} tree count");

                for (int i = 0; i < stand.Trees.Count; i++)
                {
                    ImportedTreeRecord src = stand.Trees[i];
                    InspectedTree dst = view.ObservedYear0.Trees[i];
                    string id = src.SourceTreeId;
                    TestRunner.Eq(id, dst.SourceTreeId, "tree order preserved");
                    TestRunner.Eq(src.SpeciesMapping.RawName, dst.SpeciesRawName, $"{id} raw species");
                    TestRunner.Eq(src.SpeciesMapping.CanonicalSpeciesId, dst.CanonicalSpeciesId, $"{id} canonical id");
                    TestRunner.Eq(src.SpeciesMapping.Category, dst.SpeciesCategory, $"{id} category");
                    TestRunner.Eq(src.IsConservativelyLiving, dst.IsConservativelyLiving, $"{id} living");

                    AssertRefField(src.RawSpeciesName, dst.SpeciesName, $"{id} species name");
                    AssertField(src.VitalStatus, dst.VitalStatus, $"{id} vital status");
                    AssertField(src.PositionLocalM, dst.PositionLocalM, $"{id} position");
                    AssertField(src.DbhCm, dst.DbhCm, $"{id} dbh");
                    AssertField(src.HeightM, dst.HeightM, $"{id} height");
                    AssertField(src.CrownRadiusM, dst.CrownRadiusM, $"{id} crown");
                    AssertField(src.AgeYears, dst.AgeYears, $"{id} age");

                    if (src.HeightM.IsMissing) TestRunner.True(dst.HeightM.Value is null, $"{id} no fabricated height");
                    if (src.AgeYears.IsMissing) TestRunner.True(dst.AgeYears.Value is null, $"{id} no fabricated age");
                    if (src.CrownRadiusM.IsMissing) TestRunner.True(dst.CrownRadiusM.Value is null, $"{id} no fabricated crown");
                }

                TestRunner.Eq(stand.CoordinateOrigin, view.ObservedYear0.CoordinateOrigin, $"{e.PlotId} coordinate origin passed through");

                string json = WoodlandInspectionSerializer.SerializeJson(view);
                TestRunner.True(!json.Contains("\"provenance\":\"Simulated\"", StringComparison.Ordinal), $"{e.PlotId} JSON has no Simulated provenance");
                TestRunner.True(json.Contains("\"provenance\":\"Observed\"", StringComparison.Ordinal), $"{e.PlotId} JSON has Observed provenance");
            }
        });

        TestRunner.Add(Projection, "T8a Year-0 boundary on both fixtures: observed year 0, future starts at year 1, no simulated future, zero simulated values", () =>
        {
            foreach (BuiltInWoodlandEntry e in TestFixtures.LoadCatalogue())
            {
                WoodlandInspectionView view = WoodlandInspectionProjector.Project(e.ImportResult);
                ProvenanceBoundarySection p = view.ProvenanceBoundary;
                TestRunner.Eq(0, p.ObservedStateYear, $"{e.PlotId} observed year");
                TestRunner.Eq(1, p.SimulatedFutureStartsAtYear, $"{e.PlotId} future starts at year 1");
                TestRunner.True(!p.SimulatedFutureAvailable, $"{e.PlotId} no simulated future");
                TestRunner.Eq(0, p.SimulatedValueCount, $"{e.PlotId} zero simulated values");
                TestRunner.Eq(WoodlandInspectionProjector.ProvenanceStatement, p.Statement, $"{e.PlotId} statement");
                TestRunner.True(p.Statement.Contains("nothing past Year 0", StringComparison.Ordinal), "statement says nothing past Year 0");
                TestRunner.Eq(string.Join(",", e.ObservationDates), string.Join(",", p.ObservationDates), $"{e.PlotId} observation dates");
                TestRunner.Eq(string.Join(",", p.ObservationDates), string.Join(",", view.Attribution!.ObservationDates), $"{e.PlotId} attribution dates");
                TestRunner.True(!view.ObservedYear0.Boundary.Observed, $"{e.PlotId} boundary not observed");
                TestRunner.Eq(0, view.ObservedYear0.Boundary.VerticesLocalM.Count, $"{e.PlotId} no vertices");
                TestRunner.True(view.ObservedYear0.Boundary.AreaM2 is null, $"{e.PlotId} no area");
            }
        });

        TestRunner.Add(Projection, "T9 constructed-test-boundary Mersch (100x100 m CONSTRUCTED TEST CONSTANT, no survey source) is Candidate and CanAdvanceTime", () =>
        {
            BuiltInWoodlandDefinition mersch = BuiltInWoodlandCatalogue.Definitions[1];
            WoodlandImportResult result = new IntegrateMarteloscopeReader().Read(new WoodlandImportRequest
            {
                ArchiveBytes = TestFixtures.ReadFixture(mersch.FixtureFileName),
                TargetPlotId = mersch.PlotId,
                RawLocalBoundaryVertices = TestFixtures.ConstructedTestBoundary100x100,
            });
            WoodlandInspectionView view = WoodlandInspectionProjector.Project(result);

            TestRunner.Eq(WoodlandReadiness.CandidateForSimulationIntegration, view.Readiness, "Candidate");
            TestRunner.True(view.CanAdvanceTime, "CanAdvanceTime true");
            TestRunner.Eq(0, view.BlockerChecklist.Count, "empty checklist");
            TestRunner.True(view.ObservedYear0.Boundary.Observed, "boundary observed");
            TestRunner.Eq(4, view.ObservedYear0.Boundary.VerticesLocalM.Count, "4 vertices");
            TestRunner.ApproxEq(10000.0, view.ObservedYear0.Boundary.AreaM2!.Value, 1e-9, "1 ha");
            TestRunner.Eq(BoundaryCoordinateSource.LocalMetres, view.ObservedYear0.Boundary.CoordinateSource, "local metres");
            TestRunner.True(!view.ProvenanceBoundary.SimulatedFutureAvailable, "still no simulated future in inspection");
        });

        TestRunner.Add(Projection, "T10a real-fixture species breakdown and microhabitats match the source records", () =>
        {
            foreach (BuiltInWoodlandEntry e in TestFixtures.LoadCatalogue())
            {
                WoodlandImportStand stand = e.ImportResult.Stand!;
                WoodlandInspectionView view = WoodlandInspectionProjector.Project(e.ImportResult);

                TestRunner.Eq("ModelledAtBase,PendingModelSupport,UnmodelledSpecies,UnidentifiedSpecies",
                    string.Join(",", view.SpeciesBreakdown.Categories.Select(c => c.Category)), $"{e.PlotId} all four categories in order");
                TestRunner.Eq(20, view.SpeciesBreakdown.Categories.Sum(c => c.TotalTrees), $"{e.PlotId} category totals");
                TestRunner.Eq(stand.LivingStemCount, view.SpeciesBreakdown.Categories.Sum(c => c.LivingTrees), $"{e.PlotId} category living");
                TestRunner.Eq(20, view.SpeciesBreakdown.Species.Sum(s => s.TotalTrees), $"{e.PlotId} species totals");

                int expectedTrems = stand.Trees.Sum(t => t.ExtraAttributes.Keys.Count(k => k.StartsWith("trem:", StringComparison.Ordinal)));
                Console.WriteLine($"  [microhabitats] {e.PlotId} count={view.Microhabitats.Count}");
                TestRunner.True(expectedTrems > 0, $"{e.PlotId} fixture has microhabitats");
                TestRunner.Eq(expectedTrems, view.Microhabitats.Count, $"{e.PlotId} microhabitat count");
                foreach (InspectedMicrohabitat m in view.Microhabitats)
                {
                    ImportedTreeRecord tree = stand.Trees.Single(t => t.SourceTreeId == m.SourceTreeId);
                    TestRunner.Eq(tree.ExtraAttributes["trem:" + m.Code], m.Value, $"{e.PlotId} microhabitat value verbatim");
                }
                AssertSortedOrdinal(view.Microhabitats.Select(m => m.SourceTreeId + "\u0000" + m.Code).ToList(), $"{e.PlotId} microhabitats sorted");
            }

            WoodlandInspectionView lu = WoodlandInspectionProjector.Project(TestFixtures.LoadCatalogue()[1].ImportResult);
            TestRunner.Eq("Fagus sylvatica:9/ModelledAtBase,Quercus petraea:11/ModelledAtBase",
                string.Join(",", lu.SpeciesBreakdown.Species.Select(s => $"{s.RawName}:{s.TotalTrees}/{s.Category}")), "LU species rows");
        });
    }

    // ------------------------------------------------------------------
    // 3: determinism + golden hashes
    // ------------------------------------------------------------------
    private static void RegisterDeterminismTests()
    {
        TestRunner.Add(Determinism, "T3 each built-in view is byte-identical across repeated projection and independent catalogue loads, and matches golden SHA-256", () =>
        {
            IReadOnlyList<BuiltInWoodlandEntry> loadA = TestFixtures.LoadCatalogue();
            IReadOnlyList<BuiltInWoodlandEntry> loadB = TestFixtures.LoadCatalogue();
            string[] golden = [GoldenDonadeaViewSha256, GoldenMerschViewSha256];
            string[] labels = ["ie-donadea", "lu-mersch"];

            for (int i = 0; i < 2; i++)
            {
                // The golden is the built-in view (with its disclosures), as a UI would receive it from the catalogue.
                byte[] a1 = WoodlandInspectionSerializer.SerializeUtf8(loadA[i].InspectionView);
                byte[] a2 = WoodlandInspectionSerializer.SerializeUtf8(WoodlandInspectionProjector.Project(loadA[i].ImportResult, loadA[i].Disclosures));
                byte[] b1 = WoodlandInspectionSerializer.SerializeUtf8(loadB[i].InspectionView);
                TestRunner.True(a1.AsSpan().SequenceEqual(a2), $"{labels[i]} same-load JSON identical");
                TestRunner.True(a1.AsSpan().SequenceEqual(b1), $"{labels[i]} cross-load JSON identical");

                string hashA = WoodlandInspectionSerializer.ComputeSha256Hex(loadA[i].InspectionView);
                string hashB = WoodlandInspectionSerializer.ComputeSha256Hex(WoodlandInspectionProjector.Project(loadB[i].ImportResult, loadB[i].Disclosures));
                TestRunner.Eq(hashA, hashB, $"{labels[i]} SHA-256 identical");
                Console.WriteLine($"  [determinism] {labels[i]}-inspection-sha256={hashA} ({a1.Length} bytes)");
                TestRunner.Eq(golden[i], hashA, $"{labels[i]} golden SHA-256");
            }
        });

        TestRunner.Add(Determinism, "T3b serializer writes -0.0 as 0 and invariant numbers", () =>
        {
            ImportedTreeRecord tree = TestFixtures.Tree("t1", "Fagus sylvatica", -0.0, 1.5);
            WoodlandInspectionView view = WoodlandInspectionProjector.Project(TestFixtures.Validate(TestFixtures.Stand([tree])));
            string json = WoodlandInspectionSerializer.SerializeJson(view);
            TestRunner.True(json.Contains("\"xM\":0,\"yM\":1.5", StringComparison.Ordinal), "negative zero normalised; invariant decimal point");
            TestRunner.True(!json.Contains(":-0", StringComparison.Ordinal), "no -0 number anywhere");
        });
    }

    // ------------------------------------------------------------------
    // 5, 6, 8, 10 (synthetic)
    // ------------------------------------------------------------------
    private static void RegisterSyntheticTests()
    {
        TestRunner.Add(Synthetic, "T5 garbage archive through the real reader: Refused view has no trees, refusal blockers listed, cannot advance", () =>
        {
            byte[] garbage = Encoding.UTF8.GetBytes("this is not a zip archive");
            WoodlandImportResult result = new IntegrateMarteloscopeReader().Read(new WoodlandImportRequest { ArchiveBytes = garbage });
            WoodlandInspectionView view = WoodlandInspectionProjector.Project(result);

            TestRunner.Eq(WoodlandReadiness.Refused, view.Readiness, "Refused");
            TestRunner.True(!view.CanAdvanceTime, "cannot advance");
            TestRunner.True(view.Stand is null, "no stand identity");
            TestRunner.True(view.Attribution is null, "no attribution");
            TestRunner.Eq(0, view.ObservedYear0.Trees.Count, "no trees");
            TestRunner.True(!view.ObservedYear0.Boundary.Observed, "no boundary");
            TestRunner.True(view.BlockerChecklist.Count > 0, "blockers listed");
            TestRunner.True(view.BlockerChecklist.All(b => b.Severity == IssueSeverity.Refusal), "all refusal blockers");
            TestRunner.Eq(result.Issues.Where(i => i.Severity == IssueSeverity.Refusal).Select(i => i.Code).Distinct().Count(),
                view.BlockerChecklist.Count, "one item per distinct refusal code");
            TestRunner.True(view.AdvanceTimeStatement.Contains("refused", StringComparison.Ordinal), "statement explains refusal");
            TestRunner.Eq(0, view.SpeciesBreakdown.Categories.Sum(c => c.TotalTrees), "zero species counts");
            TestRunner.Eq(4, view.SpeciesBreakdown.Categories.Count, "categories still listed");
            Console.WriteLine($"  [refused] codes={string.Join(",", view.BlockerChecklist.Select(b => b.Code))}");
            _ = WoodlandInspectionSerializer.SerializeUtf8(view); // must not throw on a Refused view
        });

        TestRunner.Add(Synthetic, "T6 missing height/age/crown and missing boundary stay null + Missing with a MissingRequiredHeight blocker", () =>
        {
            ImportedTreeRecord noHeight = TestFixtures.Tree("t1", "Fagus sylvatica", 1, 2, heightM: null);
            ImportedTreeRecord withHeight = TestFixtures.Tree("t2", "Fagus sylvatica", 3, 4);
            WoodlandImportResult result = TestFixtures.Validate(TestFixtures.Stand([noHeight, withHeight]));
            WoodlandInspectionView view = WoodlandInspectionProjector.Project(result);

            TestRunner.Eq(WoodlandReadiness.InventoryOnly, view.Readiness, "InventoryOnly");
            TestRunner.True(!view.CanAdvanceTime, "cannot advance");
            InspectedTree t1 = view.ObservedYear0.Trees[0];
            TestRunner.Eq("t1", t1.SourceTreeId, "t1 first");
            TestRunner.Eq(FieldProvenance.Missing, t1.HeightM.Provenance, "height Missing");
            TestRunner.True(t1.HeightM.Value is null, "height null");
            TestRunner.Eq("height", t1.HeightM.SourceColumn, "height evidence column kept");
            foreach (InspectedTree t in view.ObservedYear0.Trees)
            {
                TestRunner.Eq(FieldProvenance.Missing, t.AgeYears.Provenance, $"{t.SourceTreeId} age Missing");
                TestRunner.True(t.AgeYears.Value is null, $"{t.SourceTreeId} age null");
                TestRunner.Eq(FieldProvenance.Missing, t.CrownRadiusM.Provenance, $"{t.SourceTreeId} crown Missing");
                TestRunner.True(t.CrownRadiusM.Value is null, $"{t.SourceTreeId} crown null");
            }
            TestRunner.True(!view.ObservedYear0.Boundary.Observed, "boundary not observed");
            TestRunner.Eq(0, view.ObservedYear0.Boundary.VerticesLocalM.Count, "no vertices fabricated");

            TestRunner.Eq("BoundaryMissing,MissingRequiredHeight", string.Join(",", view.BlockerChecklist.Select(b => b.Code)), "checklist codes");
            BlockerChecklistItem height = view.BlockerChecklist[1];
            TestRunner.Eq("t1", string.Join(",", height.AffectedTreeIds), "affected tree");
            TestRunner.Eq("Fagus sylvatica", string.Join(",", height.AffectedRawSpeciesNames), "affected species");
            TestRunner.Eq("Living trees without a measured height", height.Title, "title");

            string json = WoodlandInspectionSerializer.SerializeJson(view);
            TestRunner.True(json.Contains("\"heightM\":{\"provenance\":\"Missing\",\"value\":null", StringComparison.Ordinal), "JSON writes null height");
        });

        TestRunner.Add(Synthetic, "T8b a Simulated field at Year 0 never reaches the view (validator Refused; a hand-built Candidate is withheld with a renderable refusal)", () =>
        {
            const double simulatedHeight = 123.45;
            ImportedTreeRecord simulated = TestFixtures.Tree("t1", "Fagus sylvatica", 10, 10, heightM: simulatedHeight, heightProvenance: FieldProvenance.Simulated);
            WoodlandImportStand stand = TestFixtures.Stand([simulated], TestFixtures.ConstructedBoundary());

            WoodlandInspectionView validated = WoodlandInspectionProjector.Project(TestFixtures.Validate(stand));
            TestRunner.Eq(WoodlandReadiness.Refused, validated.Readiness, "validator refuses Year-0 Simulated");
            TestRunner.True(!validated.CanAdvanceTime, "refused cannot advance");
            TestRunner.True(validated.BlockerChecklist.Any(b => b.Code == "SimulatedFieldAtYearZero"), "SimulatedFieldAtYearZero listed");

            // Bypass the validator to prove the projector's own guard.
            var forged = new WoodlandImportResult
            {
                Readiness = WoodlandReadiness.CandidateForSimulationIntegration,
                Stand = stand,
                Issues = Array.Empty<ImportValidationIssue>(),
            };
            WoodlandInspectionView guarded = WoodlandInspectionProjector.Project(forged);
            TestRunner.True(!guarded.CanAdvanceTime, "Candidate with a Simulated field still cannot advance");
            TestRunner.Eq(WoodlandReadiness.Refused, guarded.Readiness, "contaminated Candidate reported as Refused");
            TestRunner.Eq(1, guarded.ProvenanceBoundary.SimulatedValueCount, "simulated value counted");
            TestRunner.True(!guarded.ProvenanceBoundary.SimulatedFutureAvailable, "no simulated future");

            // The contaminated stand is withheld from the Year-0 payload.
            TestRunner.Eq(0, guarded.ObservedYear0.Trees.Count, "no trees in the Year-0 payload");
            TestRunner.True(guarded.Stand is null, "no stand identity");
            TestRunner.True(guarded.Attribution is null, "no attribution");
            TestRunner.True(guarded.ObservedYear0.CoordinateOrigin is null, "no coordinate origin");
            TestRunner.True(!guarded.ObservedYear0.Boundary.Observed, "no boundary");
            TestRunner.Eq(0, guarded.SpeciesBreakdown.Categories.Sum(c => c.TotalTrees), "no species counts");

            // ... with a renderable refusal reason.
            TestRunner.Eq(1, guarded.BlockerChecklist.Count, "one checklist item");
            BlockerChecklistItem reason = guarded.BlockerChecklist[0];
            TestRunner.Eq(WoodlandInspectionProjector.SimulatedFieldAtYearZeroCode, reason.Code, "SimulatedFieldAtYearZero listed");
            TestRunner.Eq(IssueSeverity.Refusal, reason.Severity, "as a Refusal");
            TestRunner.Eq("Simulated value found in Year-0 data", reason.Title, "plain-English title");
            TestRunner.True(BlockerExplanations.IsKnown(reason.Code) && reason.Explanation.Length > 0, "plain-English explanation");
            TestRunner.Eq("t1", string.Join(",", reason.AffectedTreeIds), "affected tree named");
            TestRunner.Eq(0, reason.AffectedRawSpeciesNames.Count, "no withheld stand data in the checklist");
            TestRunner.True(guarded.AdvanceTimeStatement.Contains("simulated", StringComparison.Ordinal), "statement explains the withheld stand");

            // The simulated measurement is absent from both the view and its JSON.
            string guardedJson = WoodlandInspectionSerializer.SerializeJson(guarded);
            string simulatedText = simulatedHeight.ToString(System.Globalization.CultureInfo.InvariantCulture);
            TestRunner.True(!guardedJson.Contains(simulatedText, StringComparison.Ordinal), "simulated height absent from JSON");
            TestRunner.True(!guardedJson.Contains("\"provenance\":\"Simulated\"", StringComparison.Ordinal), "no Simulated provenance in JSON");
            TestRunner.True(!guardedJson.Contains("synthetic simulated value", StringComparison.Ordinal), "simulated evidence note absent from JSON");
            TestRunner.True(guarded.ObservedYear0.Trees.All(t => t.HeightM.Value != simulatedHeight), "simulated height absent from the view");

            // A result that already lists the refusal is not listed twice.
            WoodlandInspectionView listed = WoodlandInspectionProjector.Project(forged with
            {
                Issues = new[] { new ImportValidationIssue(WoodlandInspectionProjector.SimulatedFieldAtYearZeroCode, IssueSeverity.Refusal, "already listed", "t1", nameof(ImportedTreeRecord.HeightM)) },
            });
            TestRunner.Eq(1, listed.BlockerChecklist.Count, "existing refusal reused");
            TestRunner.Eq("t1", string.Join(",", listed.BlockerChecklist[0].AffectedTreeIds), "existing refusal tree");

            // A clean synthetic Candidate does advance.
            WoodlandInspectionView clean = WoodlandInspectionProjector.Project(
                TestFixtures.Validate(TestFixtures.Stand([TestFixtures.Tree("t1", "Fagus sylvatica", 10, 10)], TestFixtures.ConstructedBoundary())));
            TestRunner.Eq(WoodlandReadiness.CandidateForSimulationIntegration, clean.Readiness, "clean candidate");
            TestRunner.True(clean.CanAdvanceTime, "clean candidate can advance");
        });

        TestRunner.Add(Synthetic, "T8c a Refused result that still carries a stand projects no stand data", () =>
        {
            BuiltInWoodlandEntry donadea = TestFixtures.LoadCatalogue()[0];
            WoodlandImportStand stand = donadea.ImportResult.Stand!;
            var refused = new WoodlandImportResult
            {
                Readiness = WoodlandReadiness.Refused,
                Stand = stand,
                // A stand-level refusal: source tree ids embed the plot name, so none is named here.
                Issues = new[] { new ImportValidationIssue("ArchiveSha256Mismatch", IssueSeverity.Refusal, "hand-built refusal") },
            };
            WoodlandInspectionView view = WoodlandInspectionProjector.Project(refused);

            TestRunner.Eq(WoodlandReadiness.Refused, view.Readiness, "Refused");
            TestRunner.True(!view.CanAdvanceTime, "cannot advance");
            TestRunner.True(view.Stand is null, "no stand identity");
            TestRunner.True(view.Attribution is null, "no attribution");
            TestRunner.Eq(0, view.ObservedYear0.Trees.Count, "no trees");
            TestRunner.True(view.ObservedYear0.CoordinateOrigin is null, "no coordinate origin");
            TestRunner.True(!view.ObservedYear0.Boundary.Observed, "no boundary");
            TestRunner.Eq(0, view.Microhabitats.Count, "no microhabitats");
            TestRunner.Eq(0, view.SpeciesBreakdown.Species.Count, "no species rows");
            TestRunner.Eq(0, view.SpeciesBreakdown.Categories.Sum(c => c.TotalTrees), "no species counts");
            TestRunner.Eq(0, view.ProvenanceBoundary.ObservationDates.Count, "no observation dates");
            TestRunner.Eq("ArchiveSha256Mismatch", string.Join(",", view.BlockerChecklist.Select(b => b.Code)), "refusal reason listed");
            TestRunner.Eq(0, view.BlockerChecklist[0].AffectedRawSpeciesNames.Count, "no species names from the refused stand");
            TestRunner.True(view.AdvanceTimeStatement.Contains("refused", StringComparison.Ordinal), "statement explains refusal");

            string json = WoodlandInspectionSerializer.SerializeJson(view);
            TestRunner.True(!json.Contains(stand.PlotName, StringComparison.Ordinal), "plot name (and every tree id, which embeds it) absent from JSON");
            TestRunner.True(stand.Trees.All(t => !json.Contains(t.SourceTreeId, StringComparison.Ordinal)), "no tree id in JSON");
            TestRunner.True(!json.Contains(stand.StandId, StringComparison.Ordinal), "stand id absent from JSON");
            TestRunner.True(!json.Contains(stand.Attribution.Citation, StringComparison.Ordinal), "citation absent from JSON");
            TestRunner.True(!json.Contains("Fagus sylvatica", StringComparison.Ordinal), "species absent from JSON");
            TestRunner.True(!json.Contains("\"provenance\":", StringComparison.Ordinal), "no tree fields in JSON");
        });

        TestRunner.Add(Synthetic, "T10b species breakdown covers all four categories; synthetic trem: microhabitats projected; collections read-only", () =>
        {
            var extra = new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(
                new SortedDictionary<string, string>(StringComparer.Ordinal)
                {
                    ["eventDate"] = "2021",
                    ["trem:CV11"] = "1",
                    ["trem:BA01"] = "2",
                    ["habitat"] = "test",
                });

            ImportedTreeRecord[] trees =
            [
                TestFixtures.Tree("a1", "Fagus sylvatica", 1, 1, extra: extra),
                TestFixtures.Tree("a2", "Fagus sylvatica", 2, 1, status: TreeVitalStatus.Dead),
                TestFixtures.Tree("b1", "Pinus sylvestris", 3, 1),
                TestFixtures.Tree("c1", "Tilia cordata", 4, 1),
                TestFixtures.Tree("d1", "PLANTAE", 5, 1),
                TestFixtures.Tree("d2", "Quercus", 6, 1, status: TreeVitalStatus.Dead),
            ];
            WoodlandInspectionView view = WoodlandInspectionProjector.Project(TestFixtures.Validate(TestFixtures.Stand(trees)));

            string cats = string.Join(",", view.SpeciesBreakdown.Categories.Select(c => $"{c.Category}:{c.TotalTrees}/{c.LivingTrees}"));
            TestRunner.Eq("ModelledAtBase:2/1,PendingModelSupport:1/1,UnmodelledSpecies:1/1,UnidentifiedSpecies:2/1", cats, "category counts");

            string rows = string.Join(",", view.SpeciesBreakdown.Species.Select(s => $"{s.RawName}|{s.CanonicalSpeciesId ?? "-"}|{s.Category}|{s.TotalTrees}/{s.LivingTrees}"));
            TestRunner.Eq(
                "Fagus sylvatica|beech|ModelledAtBase|2/1,Pinus sylvestris|scots-pine|PendingModelSupport|1/1,Tilia cordata|-|UnmodelledSpecies|1/1,PLANTAE|-|UnidentifiedSpecies|1/1,Quercus|-|UnidentifiedSpecies|1/0",
                rows, "species rows sorted by category then raw name");

            TestRunner.Eq("a1/BA01=2,a1/CV11=1", string.Join(",", view.Microhabitats.Select(m => $"{m.SourceTreeId}/{m.Code}={m.Value}")), "microhabitats from trem: keys, sorted");
            TestRunner.Eq("2021", string.Join(",", view.ProvenanceBoundary.ObservationDates), "eventDate observation date");

            string codes = string.Join(",", view.BlockerChecklist.Select(b => b.Code));
            TestRunner.Eq("BoundaryMissing,PendingModelSupportLivingSpecies,UnidentifiedLivingSpecies,UnsupportedLivingSpecies", codes, "species blockers");
            TestRunner.Eq("PLANTAE", string.Join(",", view.BlockerChecklist.Single(b => b.Code == "UnidentifiedLivingSpecies").AffectedRawSpeciesNames), "dead genus-only tree not a blocker");

            AssertReadOnly(view.ObservedYear0.Trees, "trees");
            AssertReadOnly(view.ObservedYear0.Boundary.VerticesLocalM, "boundary vertices");
            AssertReadOnly(view.SpeciesBreakdown.Categories, "categories");
            AssertReadOnly(view.SpeciesBreakdown.Species, "species");
            AssertReadOnly(view.Microhabitats, "microhabitats");
            AssertReadOnly(view.BlockerChecklist, "checklist");
            AssertReadOnly(view.BlockerChecklist[0].AffectedTreeIds, "affected ids");
            AssertReadOnly(view.BlockerChecklist[0].AffectedRawSpeciesNames, "affected names");
            AssertReadOnly(view.Notes, "notes");
            AssertReadOnly(view.ProvenanceBoundary.ObservationDates, "observation dates");
            AssertReadOnly(view.Attribution!.ObservationDates, "attribution dates");
            AssertReadOnly(BuiltInWoodlandCatalogue.Definitions, "catalogue definitions");
            IReadOnlyList<BuiltInWoodlandEntry> entries = TestFixtures.LoadCatalogue();
            AssertReadOnly(entries, "catalogue entries");
            AssertReadOnly(entries[0].Caveats, "caveats");
        });

        TestRunner.Add(Synthetic, "T10c unknown codes fall back to the importer message; Refusal items sort before blockers", () =>
        {
            var result = new WoodlandImportResult
            {
                Readiness = WoodlandReadiness.Refused,
                Stand = null,
                Issues = new List<ImportValidationIssue>
                {
                    new("ZzzUnknownRefusal", IssueSeverity.Refusal, "importer said zzz"),
                    new("AaaUnknownBlocker", IssueSeverity.InventoryOnlyBlocker, "importer said aaa", "t9"),
                    new("SomeInfo", IssueSeverity.Info, "just a note"),
                }.AsReadOnly(),
            };
            WoodlandInspectionView view = WoodlandInspectionProjector.Project(result);
            TestRunner.Eq("ZzzUnknownRefusal,AaaUnknownBlocker", string.Join(",", view.BlockerChecklist.Select(b => b.Code)), "Refusal first");
            TestRunner.Eq("importer said zzz", view.BlockerChecklist[0].Explanation, "fallback explanation");
            TestRunner.Eq("ZzzUnknownRefusal", view.BlockerChecklist[0].Title, "fallback title");
            TestRunner.Eq("t9", string.Join(",", view.BlockerChecklist[1].AffectedTreeIds), "affected id kept without a stand");
            TestRunner.Eq(0, view.BlockerChecklist[1].AffectedRawSpeciesNames.Count, "no species without a stand");
            TestRunner.Eq("SomeInfo", string.Join(",", view.Notes.Select(n => n.Code)), "info goes to notes");
        });

        TestRunner.Add(Synthetic, "T10d every blocker and refusal code the closed importer can emit has plain-English text", () =>
        {
            IReadOnlyList<string> codes = ImporterIssueCodes();
            Console.WriteLine($"  [codes] importer emits {codes.Count} distinct blocker/refusal codes");
            // Guards the source scan itself: R1 counted 57 distinct codes in src/Core/WoodlandImport/*.cs.
            TestRunner.Eq(57, codes.Count, "distinct importer codes found by the source scan");
            foreach (string code in codes)
            {
                TestRunner.True(BlockerExplanations.IsKnown(code), $"{code} has plain-English text");
                TestRunner.True(BlockerExplanations.TitleFor(code) != code, $"{code} title is not the raw code");
            }
        });

        TestRunner.Add(Synthetic, "T10e no backing collection reachable through ICollection.SyncRoot can reorder the catalogue or alter a view or its hash", () =>
        {
            // The probe must detect the old leak: a ReadOnlyCollection exposes its List through SyncRoot.
            TestRunner.True(TryMutateThroughSyncRoot(new List<int> { 1, 2 }.AsReadOnly()), "probe detects a ReadOnlyCollection backing list");
            TestRunner.True(TryMutateThroughSyncRoot(Array.AsReadOnly(new[] { 1, 2 })), "probe detects a ReadOnlyCollection backing array");
            TestRunner.True(TryMutateThroughSyncRoot(new System.Collections.ObjectModel.ReadOnlyDictionary<string, string>(new Dictionary<string, string> { ["a"] = "b" })), "probe detects a ReadOnlyDictionary backing dictionary");

            IReadOnlyList<BuiltInWoodlandEntry> entries = TestFixtures.LoadCatalogue();
            string[] hashesBefore = entries.Select(e => WoodlandInspectionSerializer.ComputeSha256Hex(e.InspectionView)).ToArray();
            string[] goldens = [GoldenDonadeaViewSha256, GoldenMerschViewSha256];

            // A caller-built view is just as closed: its collections are immutable too.
            WoodlandInspectionView synthetic = WoodlandInspectionProjector.Project(TestFixtures.Validate(TestFixtures.Stand(
                [TestFixtures.Tree("t1", "Fagus sylvatica", 10, 10), TestFixtures.Tree("t2", "Fagus sylvatica", 20, 20)], TestFixtures.ConstructedBoundary())));
            string syntheticBefore = WoodlandInspectionSerializer.ComputeSha256Hex(synthetic);

            var probes = new List<(string Label, object Collection)>
            {
                ("catalogue definitions", BuiltInWoodlandCatalogue.Definitions),
                ("loaded entries", entries),
            };
            foreach (BuiltInWoodlandEntry e in entries)
            {
                probes.Add(($"{e.PlotId} caveats", e.Caveats));
                probes.Add(($"{e.PlotId} observation dates", e.ObservationDates));
                AddViewProbes(probes, e.PlotId, e.InspectionView);

                WoodlandImportResult nested = e.ImportResult;
                probes.Add(($"{e.PlotId} nested issues", nested.Issues));
                probes.Add(($"{e.PlotId} nested trees", nested.Stand!.Trees));
                foreach (ImportedTreeRecord tree in nested.Stand.Trees)
                {
                    probes.Add(($"{e.PlotId} nested {tree.SourceTreeId} attributes", tree.ExtraAttributes));
                }
            }
            AddViewProbes(probes, "synthetic", synthetic);

            foreach ((string label, object collection) in probes)
            {
                TestRunner.True(!TryMutateThroughSyncRoot(collection), $"{label}: no mutable backing collection");
            }

            TestRunner.Eq($"{DonadeaPlotId},{MerschPlotId}", string.Join(",", BuiltInWoodlandCatalogue.Definitions.Select(d => d.PlotId)), "catalogue order unchanged");
            TestRunner.Eq($"{DonadeaPlotId},{MerschPlotId}", string.Join(",", entries.Select(e => e.PlotId)), "entry order unchanged");
            for (int i = 0; i < entries.Count; i++)
            {
                TestRunner.Eq(hashesBefore[i], WoodlandInspectionSerializer.ComputeSha256Hex(entries[i].InspectionView), $"{entries[i].PlotId} view hash unchanged");
                TestRunner.Eq(goldens[i], WoodlandInspectionSerializer.ComputeSha256Hex(WoodlandInspectionProjector.Project(entries[i].ImportResult, entries[i].Disclosures)), $"{entries[i].PlotId} re-projected nested result still matches golden");
                TestRunner.Eq(20, entries[i].ImportResult.Stand!.Trees.Count, $"{entries[i].PlotId} nested trees intact");
            }
            TestRunner.Eq(syntheticBefore, WoodlandInspectionSerializer.ComputeSha256Hex(synthetic), "synthetic view hash unchanged");

            // Changing the caller's own input after projection cannot reach the view either.
            var callerTrees = new List<ImportedTreeRecord> { TestFixtures.Tree("t1", "Fagus sylvatica", 10, 10), TestFixtures.Tree("t2", "Fagus sylvatica", 20, 20) };
            WoodlandImportResult callerResult = TestFixtures.Validate(TestFixtures.Stand(callerTrees, TestFixtures.ConstructedBoundary()));
            var mutableTrees = new List<ImportedTreeRecord>(callerResult.Stand!.Trees);
            WoodlandImportResult callerOwned = callerResult with { Stand = callerResult.Stand with { Trees = mutableTrees } };
            WoodlandInspectionView callerView = WoodlandInspectionProjector.Project(callerOwned);
            string callerBefore = WoodlandInspectionSerializer.ComputeSha256Hex(callerView);
            mutableTrees.Reverse();
            mutableTrees.RemoveAt(0);
            TestRunner.Eq(callerBefore, WoodlandInspectionSerializer.ComputeSha256Hex(callerView), "caller input changes do not reach the projected view");
            TestRunner.Eq("t1,t2", string.Join(",", callerView.ObservedYear0.Trees.Select(t => t.SourceTreeId)), "projected tree order unchanged");
        });
    }

    // ------------------------------------------------------------------
    // Static hygiene of the new production files
    // ------------------------------------------------------------------
    private static void RegisterHygieneTests()
    {
        TestRunner.Add(Hygiene, "inspection sources use no file, clock, network, Unity or simulation-wiring APIs", () =>
        {
            string dir = Path.GetFullPath(Path.Combine(TestFixtures.QualifiedFixturesDir(), "..", "..", "..", "..", "..", "src", "Core", "WoodlandImport", "Inspection"));
            TestRunner.True(Directory.Exists(dir), "located src/Core/WoodlandImport/Inspection");
            string[] banned =
            [
                "File.Write", "File.Create", "File.Open", "File.Append", "File.Copy", "File.Move", "File.Delete",
                "File.Replace", "File.ReadAll", "FileStream", "Directory.", "StreamWriter",
                "DateTime.Now", "DateTime.UtcNow", "DateTimeOffset.Now", "DateTimeOffset.UtcNow", "Guid.NewGuid", "Random",
                "System.Net", "HttpClient", "UnityEngine", "MultiAreaContinuation", "AnnualPipeline",
            ];
            string[] files = Directory.GetFiles(dir, "*.cs", SearchOption.AllDirectories);
            TestRunner.Eq(5, files.Length, "five inspection source files");
            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                foreach (string b in banned)
                {
                    TestRunner.True(!content.Contains(b, StringComparison.Ordinal), $"{Path.GetFileName(file)} must not reference '{b}'");
                }
            }
        });
    }

    // ------------------------------------------------------------------
    // Assertion helpers
    // ------------------------------------------------------------------
    private static void AssertField<T>(ImportField<T> src, InspectedField<T?> dst, string label) where T : struct
    {
        TestRunner.Eq(src.Provenance, dst.Provenance, label + " provenance");
        if (src.IsMissing)
        {
            TestRunner.True(dst.Value is null, label + " value null when source Missing");
        }
        else
        {
            TestRunner.True(dst.Value is not null && EqualityComparer<T>.Default.Equals(src.Value, dst.Value.Value), label + " value identical");
        }
        AssertEvidence(src.Evidence, dst.SourceColumn, dst.RawValue, dst.Note, label);
    }

    private static void AssertRefField(ImportField<string> src, InspectedField<string?> dst, string label)
    {
        TestRunner.Eq(src.Provenance, dst.Provenance, label + " provenance");
        TestRunner.Eq(src.IsMissing ? null : src.Value, dst.Value, label + " value");
        AssertEvidence(src.Evidence, dst.SourceColumn, dst.RawValue, dst.Note, label);
    }

    private static void AssertEvidence(FieldEvidence? evidence, string? column, string? raw, string? note, string label)
    {
        TestRunner.Eq(evidence?.SourceColumn, column, label + " source column");
        TestRunner.Eq(evidence?.RawValue, raw, label + " raw value");
        TestRunner.Eq(evidence?.Note, note, label + " note");
    }

    private static void AssertReadOnly<T>(IReadOnlyList<T> list, string label)
    {
        TestRunner.True(list is ICollection<T> c && c.IsReadOnly, label + " is read-only");
        TestRunner.True(list is not List<T>, label + " is not a mutable List");
    }

    private static void AddViewProbes(List<(string Label, object Collection)> probes, string label, WoodlandInspectionView view)
    {
        probes.Add(($"{label} view trees", view.ObservedYear0.Trees));
        probes.Add(($"{label} view boundary vertices", view.ObservedYear0.Boundary.VerticesLocalM));
        probes.Add(($"{label} view categories", view.SpeciesBreakdown.Categories));
        probes.Add(($"{label} view species", view.SpeciesBreakdown.Species));
        probes.Add(($"{label} view microhabitats", view.Microhabitats));
        probes.Add(($"{label} view checklist", view.BlockerChecklist));
        probes.Add(($"{label} view notes", view.Notes));
        probes.Add(($"{label} view observation dates", view.ProvenanceBoundary.ObservationDates));
        if (view.Attribution is not null)
        {
            probes.Add(($"{label} view attribution dates", view.Attribution.ObservationDates));
        }
        foreach (BlockerChecklistItem item in view.BlockerChecklist)
        {
            probes.Add(($"{label} {item.Code} tree ids", item.AffectedTreeIds));
            probes.Add(($"{label} {item.Code} species names", item.AffectedRawSpeciesNames));
        }
    }

    /// <summary>
    /// Tries to change a collection through the object its ICollection.SyncRoot returns (a ReadOnlyCollection
    /// or ReadOnlyDictionary hands back its mutable backing store there). Returns true when a change succeeded.
    /// </summary>
    private static bool TryMutateThroughSyncRoot(object collection)
    {
        if (collection is not ICollection c)
        {
            return false;
        }

        switch (c.SyncRoot)
        {
            case Array array:
                Array.Reverse(array);
                return true;
            case IList list when !list.IsReadOnly:
                if (list.Count > 0)
                {
                    object? first = list[0];
                    list.RemoveAt(0);
                    list.Add(first);
                }
                return true;
            case IDictionary dictionary when !dictionary.IsReadOnly:
                dictionary.Clear();
                return true;
            case IList list:
                try { list.Clear(); return true; } catch (NotSupportedException) { return false; }
            case IDictionary dictionary:
                try { dictionary.Clear(); return true; } catch (NotSupportedException) { return false; }
            default:
                return false;
        }
    }

    /// <summary>Distinct InventoryOnlyBlocker/Refusal codes in the closed importer sources (src/Core/WoodlandImport/*.cs).</summary>
    private static IReadOnlyList<string> ImporterIssueCodes()
    {
        string dir = Path.GetFullPath(Path.Combine(TestFixtures.QualifiedFixturesDir(), "..", "..", "..", "..", "..", "src", "Core", "WoodlandImport"));
        var codes = new SortedSet<string>(StringComparer.Ordinal);
        var literal = new Regex("\"([A-Z][A-Za-z0-9]+)\"");
        foreach (string file in Directory.GetFiles(dir, "*.cs", SearchOption.TopDirectoryOnly))
        {
            string[] lines = File.ReadAllLines(file);
            for (int i = 1; i < lines.Length; i++)
            {
                // Every issue is built as new ImportValidationIssue(code, IssueSeverity.X, ...): the code is on the line before.
                if (Regex.IsMatch(lines[i], "^\\s*IssueSeverity\\.(InventoryOnlyBlocker|Refusal),"))
                {
                    foreach (Match m in literal.Matches(lines[i - 1]))
                    {
                        codes.Add(m.Groups[1].Value);
                    }
                }
            }

            // Codes passed through a variable: the cap code argument and the living-species switch.
            string text = string.Join("\n", lines);
            foreach (Match m in Regex.Matches(text, "capIssueCode:\\s*\"([A-Za-z0-9]+)\""))
            {
                codes.Add(m.Groups[1].Value);
            }
            foreach (Match m in Regex.Matches(text, "=>\\s*\"([A-Za-z]+LivingSpecies)\""))
            {
                codes.Add(m.Groups[1].Value);
            }
        }
        return codes.ToList();
    }

    private static void AssertSortedOrdinal(IReadOnlyList<string> values, string label)
    {
        for (int i = 1; i < values.Count; i++)
        {
            TestRunner.True(StringComparer.Ordinal.Compare(values[i - 1], values[i]) <= 0, label);
        }
    }
}
