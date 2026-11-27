namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// Fixed, versioned stream identifiers. They are prefix-free against the existing RngSalts families; Part 2 registers them
/// in RngSalts. Changing any of them, or the formulas that use them, needs a new model version (D-050 rule 4).
/// </summary>
public static class NaturalProcessStreams
{
    public const string StormOccurs = "STORM-OCCURS-v1";
    public const string StormSeverity = "STORM-SEVERITY-v1";
    public const string StormDirection = "STORM-DIRECTION-v1";
    public const string WindthrowTreePrefix = "WINDTHROW-v1-";

    public const string JayDivertPrefix = "JAY-DIVERT-v1-";
    public const string JayAcornPrefix = "JAY-ACORN-v1-";
    public const string JayDistanceSuffix = "/distance";
    public const string JayBearingSuffix = "/bearing";
    public const string NearbyAlongSuffix = "/along";
    public const string NearbySourcePrefix = "NEARBY-OAK-WOODLAND-";

    public static IReadOnlyList<string> All { get; } = Array.AsReadOnly(new[]
    {
        StormOccurs, StormSeverity, StormDirection, WindthrowTreePrefix,
        JayDivertPrefix, JayAcornPrefix,
    });
}

/// <summary>
/// Stateless keyed roll, a verbatim port of SimulationRandom.Roll model 1 (the RNG model new games and the 2D lab use).
/// A roll hashes its key, so drawing one key can never move another stream, and the draw order does not matter.
/// Each engine call owns one counter so callers and tests can prove how many draws were made.
/// </summary>
internal sealed class NaturalProcessRandom
{
    public const int RngModel = 1;

    /// <summary>Test-only observation seam: when set on a thread, every draw appends its key. Null in use.</summary>
    [ThreadStatic] internal static List<string>? DrawLog;

    public int DrawCount { get; private set; }

    public float Roll(string key, int year, int seed)
    {
        DrawCount++;
        DrawLog?.Add(key);
        return RollModel1(key, year, seed);
    }

    internal static float RollModel1(string key, int year, int seed)
    {
        uint hash = 2166136261u;
        foreach (char c in key)
            hash = unchecked((hash ^ c) * 16777619u);
        ulong state = Mix(((ulong)hash << 32) ^ (ulong)(uint)year);
        state = Mix(state ^ unchecked((ulong)(uint)seed * 0x9E3779B97F4A7C15UL));
        return (state >> 40) / 16777216f;
    }

    private static ulong Mix(ulong z)
    {
        unchecked
        {
            z += 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
    }
}
