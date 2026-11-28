namespace SitkaP1.Core.NaturalProcesses.Tests;

internal static class HygieneAndParityTests
{
    private const string Suite = "hygiene-parity";

    public static void Register()
    {
        TestRunner.Add(Suite, "JW-T60 keyed roll is bit-identical to frozen SimulationRandom.Roll model 1", () =>
        {
            (string Key, int Year, int Seed, int Bits)[] vectors =
            {
                ("STORM-OCCURS-v1", 1, 0, 1051323186), ("STORM-OCCURS-v1", 7, 12345, 1049917242), ("STORM-OCCURS-v1", 42, -99, 1039766992),
                ("STORM-SEVERITY-v1", 1, 0, 1036554648), ("STORM-SEVERITY-v1", 7, 12345, 1049880538), ("STORM-SEVERITY-v1", 42, -99, 1064514984),
                ("STORM-DIRECTION-v1", 1, 0, 1052229852), ("STORM-DIRECTION-v1", 7, 12345, 1025866496), ("STORM-DIRECTION-v1", 42, -99, 1060305130),
                ("WINDTHROW-v1-T0001", 1, 0, 1048712486), ("WINDTHROW-v1-T0001", 7, 12345, 1057203739), ("WINDTHROW-v1-T0001", 42, -99, 1056215194),
                ("JAY-ACORN-v1-OAK-7-0/distance", 1, 0, 1057653767), ("JAY-ACORN-v1-OAK-7-0/distance", 7, 12345, 1051964398),
                ("JAY-ACORN-v1-OAK-7-0/distance", 42, -99, 1059405114),
            };
            foreach (var v in vectors)
                TestRunner.Eq(v.Bits, BitConverter.SingleToInt32Bits(NaturalProcessRandom.RollModel1(v.Key, v.Year, v.Seed)), v.Key + "@" + v.Year);
        });

        TestRunner.Add(Suite, "JW-T61 vulnerability and BoundedRational failure chance are bit-identical to frozen StormModel 1", () =>
        {
            (StormTreeInput Tree, int VBits, int[] PBits)[] vectors =
            {
                (new("A", "L", "C", 22f, 28f, 22f, .3f, 0f), 1072054652, new[] { 1024343805, 1036485380, 1048213431 }),
                (new("B", "L", "C", 25f, 30f, 24f, .8f, 1.5f), 1088862659, new[] { 1040256488, 1050319944, 1058051891 }),
                (new("C", "L", "C", 6f, 10f, 20f, .1f, 0f), 1003402246, new[] { 956581371, 969301033, 982820948 }),
                (new("D", "L", "C", 30f, 45f, 28f, .5f, 2f), 1088207893, new[] { 1039681909, 1050009619, 1057868843 }),
            };
            foreach (var v in vectors)
            {
                float index = StormVulnerability.Index(v.Tree);
                TestRunner.Eq(v.VBits, BitConverter.SingleToInt32Bits(index), "V " + v.Tree.TreeId);
                for (int s = 0; s < 3; s++)
                    TestRunner.Eq(v.PBits[s], BitConverter.SingleToInt32Bits(StormVulnerability.FailureChance(index, StormVulnerability.Severities[s])), "p " + v.Tree.TreeId + " S" + s);
            }
            TestRunner.Eq(0.02f, StormVulnerability.AnnualProbability, "annual 0.02");
            TestRunner.Eq("0.02,0.06,0.18", string.Join(",", StormVulnerability.Severities.Select(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture))), "severities");
            TestRunner.Eq("1,1,1", string.Join(",", StormVulnerability.Weights.Select(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture))), "weights 1:1:1");
        });

        TestRunner.Add(Suite, "JW-T62 stream ids are fixed, versioned and prefix-free against the existing RngSalts families", () =>
        {
            TestRunner.Eq("STORM-OCCURS-v1", NaturalProcessStreams.StormOccurs, "occurs");
            TestRunner.Eq("STORM-SEVERITY-v1", NaturalProcessStreams.StormSeverity, "severity");
            TestRunner.Eq("STORM-DIRECTION-v1", NaturalProcessStreams.StormDirection, "direction");
            TestRunner.Eq("WINDTHROW-v1-", NaturalProcessStreams.WindthrowTreePrefix, "windthrow");
            // All nine RngSalts.Families entries, copied in registry order from src/Core/RngSalts.cs at base c82e4e75
            // (not compiled in: this test exe is standalone).
            string[] existing =
            {
                "ADULT-MORT-", "ADULT-MORT-GENERIC-", "ADULT-MORT-GENERIC-QUOTA-", "PHASE2-MAST-", "PHASE2-PJ-",
                "/vegetation", "/browse", "STAGE5-MAST", "STAGE5-PROMO-",
            };
            TestRunner.Eq(9, existing.Length, "all nine existing registry entries");
            foreach (string ours in NaturalProcessStreams.All)
            foreach (string theirs in existing)
                TestRunner.True(!ours.StartsWith(theirs, StringComparison.Ordinal) && !theirs.StartsWith(ours, StringComparison.Ordinal),
                    ours + " vs " + theirs);
            var all = NaturalProcessStreams.All.ToArray();
            for (int i = 0; i < all.Length; i++)
            for (int j = 0; j < all.Length; j++)
                if (i != j) TestRunner.True(!all[i].StartsWith(all[j], StringComparison.Ordinal), all[i] + " vs " + all[j]);
        });

        TestRunner.Add(Suite, "JW-T63 static hygiene: no file, network, clock, machine-state or culture-dependent API in src/Core/NaturalProcesses", () =>
        {
            string? dir = FindDir("src", "Core", "NaturalProcesses");
            TestRunner.True(dir is not null, "located source");
            string[] banned =
            {
                "System.IO", "File.", "Directory.", "FileStream", "StreamWriter", "StreamReader", "Path.",
                "System.Net", "HttpClient", "WebClient", "WebRequest", "Socket", "Dns.",
                "DateTime.Now", "DateTime.UtcNow", "DateTime.Today", "DateTimeOffset", "Stopwatch", "Environment.", "Guid.NewGuid",
                "Random.Shared", "new Random(", "System.Random", "CurrentCulture", "CurrentUICulture", "Process.", "Thread.Sleep",
                "UnityEngine", "Console.", "Assembly.Load",
            };
            string[] files = Directory.GetFiles(dir!, "*.cs", SearchOption.AllDirectories);
            TestRunner.True(files.Length >= 8, "at least 8 production files");
            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                foreach (string b in banned)
                    TestRunner.True(!content.Contains(b, StringComparison.Ordinal), Path.GetFileName(file) + " must not reference '" + b + "'");
            }
        });

        TestRunner.Add(Suite, "JW-T64 test project is standalone: one Compile glob with Senescence excluded, zero ProjectReference/PackageReference", () =>
        {
            string? dir = FindDir("tests", "Core", "NaturalProcesses");
            TestRunner.True(dir is not null, "located tests");
            string csproj = File.ReadAllText(Path.Combine(dir!, "SitkaP1.NaturalProcesses.Tests.csproj"));
            TestRunner.True(!csproj.Contains("ProjectReference", StringComparison.Ordinal), "no ProjectReference");
            TestRunner.True(!csproj.Contains("PackageReference", StringComparison.Ordinal), "no PackageReference");
            TestRunner.True(!csproj.Contains("<Reference", StringComparison.Ordinal), "no binary reference");
            TestRunner.Eq(2, csproj.Split("<Compile ").Length - 1, "one Compile include and one removal");
            TestRunner.True(csproj.Contains("<Compile Remove=\"../../../src/Core/NaturalProcesses/Senescence/**/*.cs\" />", StringComparison.Ordinal), "exclude only the standalone Senescence module");
            TestRunner.True(csproj.Contains("<Compile Include=\"../../../src/Core/NaturalProcesses/**/*.cs\" />", StringComparison.Ordinal), "glob");
        });
        RegisterCopyToRetention();
    }

    private static void RegisterCopyToRetention()
    {
        TestRunner.Add(Suite, "JW-T65 caller collections whose CopyTo retains the destination cannot change requests, with-copies, footprint or results (R3-01)", () =>
        {
            // Fixture self-check: on this SDK the hostile collection really captures LINQ's destination, so the attack is live.
            var probe = new Fixtures.RetainingCollection<int>(new[] { 1, 2, 3 });
            int[] linqCopy = probe.ToArray();
            probe.OverwriteCaptured(99);
            TestRunner.True(probe.CopyToCalls == 1 && linqCopy[0] == 99, "Enumerable.ToArray hands its destination to CopyTo (attack is real)");

            // Footprint: 80 x 80 m from four hostile points stays 6,400 m2 and keeps reporting the same outline.
            var corners = new Fixtures.RetainingCollection<LocalPoint>(new[]
                { new LocalPoint(0, 0), new LocalPoint(80, 0), new LocalPoint(80, 80), new LocalPoint(0, 80) });
            var footprint = new WoodlandFootprint(corners);
            corners.OverwriteCaptured(2, new LocalPoint(80, 40));
            TestRunner.Eq(6400.0, footprint.AreaSquareMetres, "footprint area stays 6,400 m2");
            TestRunner.Eq(new LocalPoint(80, 80), footprint.Vertices[2], "reported vertex 2 unchanged");
            TestRunner.True(footprint.Contains(new LocalPoint(70, 70)), "geometry still uses (80, 80)");
            TestRunner.Eq(0, corners.CopyToCalls, "footprint never calls the caller's CopyTo");

            // Jay request: construction, plain with-copy, init replacement, and the result built from the request.
            var oaks = new Fixtures.RetainingCollection<OnMapOakSeedCandidate>(Fixtures.OakCandidates(5, 4));
            var jayRequest = Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, oaks);
            var jayCopy = jayRequest with { };
            var oaksForInit = new Fixtures.RetainingCollection<OnMapOakSeedCandidate>(Fixtures.OakCandidates(5, 4));
            var jayReplaced = jayRequest with { OnMapOakCandidates = oaksForInit };
            var jayResult = JayAcornDispersal.Disperse(jayRequest);
            string jayIds = string.Join(",", jayRequest.OnMapOakCandidates.Select(c => c.CandidateId));
            string jayBaseline = Fixtures.Describe(jayResult);
            var bogusOak = new OnMapOakSeedCandidate("HOSTILE", "HOSTILE", "sessile-oak", new LocalPoint(1, 1));
            oaks.OverwriteCaptured(bogusOak);
            oaksForInit.OverwriteCaptured(bogusOak);
            TestRunner.Eq(0, oaks.CopyToCalls + oaksForInit.CopyToCalls, "jay request never calls the caller's CopyTo");
            TestRunner.Eq(jayIds, string.Join(",", jayRequest.OnMapOakCandidates.Select(c => c.CandidateId)), "jay request unchanged");
            TestRunner.Eq(jayIds, string.Join(",", jayCopy.OnMapOakCandidates.Select(c => c.CandidateId)), "jay with-copy unchanged");
            TestRunner.Eq(jayIds, string.Join(",", jayReplaced.OnMapOakCandidates.Select(c => c.CandidateId)), "jay init replacement unchanged");
            TestRunner.True(jayRequest == jayCopy && jayRequest == jayReplaced, "jay request and copies still equal");
            TestRunner.Eq(jayBaseline, Fixtures.Describe(jayResult), "jay result unchanged");
            TestRunner.Eq(jayBaseline, Fixtures.Describe(JayAcornDispersal.Disperse(jayRequest)), "re-running the request gives the same result");
            TestRunner.Eq(jayBaseline, Fixtures.Describe(JayAcornDispersal.Disperse(jayReplaced)), "init replacement gives the same result");

            // Storm request and result.
            var trees = new Fixtures.RetainingCollection<StormTreeInput>(Fixtures.StormTrees(60));
            var stormRequest = new StormRequest(StormModelVersion.WindthrowV1, 108, Fixtures.Seed, trees);
            var stormCopy = stormRequest with { };
            var treesForInit = new Fixtures.RetainingCollection<StormTreeInput>(Fixtures.StormTrees(60));
            var stormReplaced = stormRequest with { Trees = treesForInit };
            var stormResult = StormAdapter.Resolve(stormRequest);
            var bandTrees = new Fixtures.RetainingCollection<StormTreeInput>(Fixtures.StormTrees(5));
            var bands = StormAdapter.ExposureBands(bandTrees);
            string stormBaseline = Fixtures.Describe(stormResult);
            string bandsBaseline = string.Join(",", bands);
            TestRunner.True(stormResult.Victims.Count > 0, "fixture has victims");
            var bogusTree = new StormTreeInput("HOSTILE", "LANE-X", "CX", 40f, 10f, 40f, 1f, 2f);
            trees.OverwriteCaptured(bogusTree);
            treesForInit.OverwriteCaptured(bogusTree);
            bandTrees.OverwriteCaptured(bogusTree);
            TestRunner.Eq(0, trees.CopyToCalls + treesForInit.CopyToCalls + bandTrees.CopyToCalls, "storm inputs never call the caller's CopyTo");
            TestRunner.True(stormRequest.Trees.All(t => t.TreeId != "HOSTILE") && stormCopy.Trees.All(t => t.TreeId != "HOSTILE") &&
                stormReplaced.Trees.All(t => t.TreeId != "HOSTILE"), "storm request, with-copy and init replacement unchanged");
            TestRunner.True(stormRequest == stormCopy && stormRequest == stormReplaced, "storm request and copies still equal");
            TestRunner.Eq(stormBaseline, Fixtures.Describe(stormResult), "storm result unchanged");
            TestRunner.Eq(stormBaseline, Fixtures.Describe(StormAdapter.Resolve(stormReplaced)), "init replacement gives the same storm result");
            TestRunner.Eq(bandsBaseline, string.Join(",", bands), "exposure bands unchanged");

            // Salvage request, result and deadwood merge.
            var stock = new Fixtures.RetainingCollection<FallenDeadwoodRecord>(new[]
            {
                new FallenDeadwoodRecord("DW-A", "TA", StormAdapter.WindthrowCause, 108, DeadwoodForm.Fallen, 20f, 25f),
                new FallenDeadwoodRecord("DW-B", "TB", StormAdapter.WindthrowCause, 108, DeadwoodForm.Fallen, 22f, 27f),
            });
            var extracted = new Fixtures.RetainingCollection<string>(new[] { "DW-OLD" });
            var orders = new Fixtures.RetainingCollection<SalvageOrder>(new[] { new SalvageOrder("DW-A", SalvageChoice.SellAndExtract, "WP-1", 1.0) });
            var salvageRequest = new SalvageRequest(stock, extracted, orders, new SalvagePricing(42.50m, 2500m));
            var salvageCopy = salvageRequest with { };
            var salvageResult = SalvageAccounting.Apply(salvageRequest);
            var added = new Fixtures.RetainingCollection<FallenDeadwoodRecord>(new[]
                { new FallenDeadwoodRecord("DW-NEW", "TN", StormAdapter.WindthrowCause, 110, DeadwoodForm.Fallen, 20f, 25f) });
            var merged = SalvageAccounting.MergeWindthrowDeadwood(stock, extracted, added);
            string salvageBaseline = string.Join(",", salvageResult.RemainingDeadwood.Select(d => d.DeadwoodId)) + "/" +
                string.Join(",", salvageResult.ExtractedDeadwoodIds) + "/" + salvageResult.GrossSaleRevenue.ToString(System.Globalization.CultureInfo.InvariantCulture);
            string mergedBaseline = string.Join(",", merged.Deadwood.Select(d => d.DeadwoodId));
            var bogusDeadwood = new FallenDeadwoodRecord("DW-HOSTILE", "TX", "other", 1, DeadwoodForm.Fallen, 1f, 1f);
            stock.OverwriteCaptured(bogusDeadwood);
            added.OverwriteCaptured(bogusDeadwood);
            extracted.OverwriteCaptured("DW-A");
            orders.OverwriteCaptured(new SalvageOrder("DW-B", SalvageChoice.KeepForUse, "WP-9", 5.0));
            TestRunner.Eq(0, stock.CopyToCalls + extracted.CopyToCalls + orders.CopyToCalls + added.CopyToCalls, "salvage inputs never call the caller's CopyTo");
            TestRunner.True(salvageRequest == salvageCopy, "salvage request and with-copy still equal");
            TestRunner.Eq("DW-A,DW-B", string.Join(",", salvageRequest.Deadwood.Select(d => d.DeadwoodId)), "salvage request deadwood unchanged");
            TestRunner.Eq("DW-A,DW-B", string.Join(",", salvageCopy.Deadwood.Select(d => d.DeadwoodId)), "salvage with-copy deadwood unchanged");
            string salvageAgain = SalvageAccounting.Apply(salvageRequest) is var again
                ? string.Join(",", again.RemainingDeadwood.Select(d => d.DeadwoodId)) + "/" + string.Join(",", again.ExtractedDeadwoodIds) + "/" +
                  again.GrossSaleRevenue.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : "";
            TestRunner.Eq(salvageBaseline, salvageAgain, "re-running the salvage request gives the same result");
            TestRunner.Eq("DW-B/DW-A,DW-OLD/42.50", salvageBaseline, "salvage result is the expected one");
            TestRunner.Eq("DW-A,DW-B,DW-NEW", mergedBaseline, "merge result is the expected one");
            TestRunner.Eq(mergedBaseline, string.Join(",", merged.Deadwood.Select(d => d.DeadwoodId)), "merge result unchanged");
        });
    }

    private static string? FindDir(params string[] parts)
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d is not null)
        {
            string candidate = Path.Combine(new[] { d.FullName }.Concat(parts).ToArray());
            if (Directory.Exists(candidate)) return candidate;
            d = d.Parent;
        }
        return null;
    }
}
