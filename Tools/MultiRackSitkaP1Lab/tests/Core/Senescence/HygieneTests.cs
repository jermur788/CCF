using static SitkaP1.Core.Senescence.Tests.TestRunner;

namespace SitkaP1.Core.Senescence.Tests;

/// <summary>
/// FR-6: the module sources stay pure. Because the existing NaturalProcesses hygiene test (JW-T63) text-scans
/// src/Core/NaturalProcesses/** recursively, these files must also pass its banned list once the integrator resolves
/// that project's compile glob (see the spec's integration prerequisites).
/// </summary>
internal static class HygieneTests
{
    private const string Suite = "Hygiene";

    public static void Register()
    {
        TestRunner.Add(Suite, "HYG-01 the four module sources use no file, network, clock, machine-state, culture or ambient RNG API (JW-T63 + G6 lists)", () =>
        {
            string? dir = Fixtures.FindDir("src", "Core", "NaturalProcesses", "Senescence");
            True(dir is not null, "located the module source");

            // JW-T63's banned list, copied verbatim at base b2006767, plus the extra G6 determinism tokens.
            string[] banned =
            {
                "System.IO", "File.", "Directory.", "FileStream", "StreamWriter", "StreamReader", "Path.",
                "System.Net", "HttpClient", "WebClient", "WebRequest", "Socket", "Dns.",
                "DateTime.Now", "DateTime.UtcNow", "DateTime.Today", "DateTimeOffset", "Stopwatch", "Environment.", "Guid.NewGuid",
                "Random.Shared", "new Random(", "System.Random", "CurrentCulture", "CurrentUICulture", "Process.", "Thread.Sleep",
                "UnityEngine", "Console.", "Assembly.Load",
                "Parallel.", "AsParallel", "RandomNumberGenerator",
            };
            string[] files = Directory.GetFiles(dir!, "*.cs", SearchOption.AllDirectories).OrderBy(f => f, StringComparer.Ordinal).ToArray();
            Eq("LateLifeGrowth.cs,SenescenceContracts.cs,SenescenceEngine.cs,SenescenceHazard.cs",
                string.Join(",", files.Select(Path.GetFileName)), "the four Part 1 module files");

            string[] allowedUsings =
            {
                "using SitkaP1.Core.SpeciesWave2;", "using SitkaP1.Core.SpeciesWave2.BirchAlder;", "using SitkaP1.Core.SpeciesWave2.ScotsPine;",
            };
            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                string content = File.ReadAllText(file);
                foreach (string b in banned)
                    True(!content.Contains(b, StringComparison.Ordinal), name + " must not reference '" + b + "'");
                True(content.Contains("namespace SitkaP1.Core.NaturalProcesses.Senescence;", StringComparison.Ordinal), name + " namespace");
                foreach (string line in content.Split('\n').Select(l => l.Trim()).Where(l => l.StartsWith("using ", StringComparison.Ordinal)))
                    True(allowedUsings.Contains(line), name + " depends only on Species Wave 2 and the BCL: " + line);
            }
        });
    }
}
