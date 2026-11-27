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
            // Copied from src/Core/RngSalts.cs at base c82e4e75 (not compiled in: this test exe is standalone).
            string[] existing = { "ADULT-MORT-", "PHASE2-MAST-", "PHASE2-PJ-", "STAGE5-MAST", "STAGE5-PROMO-" };
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

        TestRunner.Add(Suite, "JW-T64 test project is standalone: one Compile glob, zero ProjectReference/PackageReference", () =>
        {
            string? dir = FindDir("tests", "Core", "NaturalProcesses");
            TestRunner.True(dir is not null, "located tests");
            string csproj = File.ReadAllText(Path.Combine(dir!, "SitkaP1.NaturalProcesses.Tests.csproj"));
            TestRunner.True(!csproj.Contains("ProjectReference", StringComparison.Ordinal), "no ProjectReference");
            TestRunner.True(!csproj.Contains("PackageReference", StringComparison.Ordinal), "no PackageReference");
            TestRunner.True(!csproj.Contains("<Reference", StringComparison.Ordinal), "no binary reference");
            TestRunner.Eq(1, csproj.Split("<Compile ").Length - 1, "one Compile item");
            TestRunner.True(csproj.Contains("<Compile Include=\"../../../src/Core/NaturalProcesses/**/*.cs\" />", StringComparison.Ordinal), "glob");
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
