namespace SitkaP1.Core;

/// <summary>
/// Stage 5 random streams (spec §6). Each stream is a System.Random seeded through the existing SimulationRandom model 1
/// (splitmix) with the stream key's FNV-1a hash as the salt, the same way Unity's SpeciesRandom salts a species stream.
/// The keys are new and isolated: nothing here calls SimulationRandom.Roll, so the adult mortality rolls
/// ("ADULT-MORT-" + TreeId, stateless per tree) can never be consumed or reordered by Stage 5.
/// </summary>
internal static class Stage5Random
{
    public const string MastKey = RngSalts.Stage5Mast;

    /// <summary>The promotion-placement stream of one cell in one year.</summary>
    public static string PromotionKey(string areaId, int column, int row, int year) => string.Create(
        System.Globalization.CultureInfo.InvariantCulture, $"{RngSalts.Stage5Promotion}{areaId}-{column}-{row}-{year}");

    public static System.Random Stream(string key, int seed, int year) =>
        SimulationRandom.Create(DerivedManifest.RngModelVersion, seed, year, unchecked((int)Fnv1a(key)));

    /// <summary>32-bit FNV-1a over the UTF-16 code units, as in SimulationRandom.Roll and Unity SpeciesRandom.</summary>
    internal static uint Fnv1a(string key)
    {
        uint hash = 2166136261u;
        foreach (char c in key) hash = unchecked((hash ^ c) * 16777619u);
        return hash;
    }
}

/// <summary>This year's woodland mast state: the roll, its label and the seed-potential multiplier.</summary>
internal sealed record Stage5MastState(double Roll, string Label, float Multiplier);

/// <summary>
/// One woodland-wide mast roll per year (owner Q1): r = NextDouble() on the STAGE5-MAST stream; r &lt; 0.15 good (×1.5),
/// r &lt; 0.15 + 0.35 poor (×0.4), else normal (×1.0). Thresholds are the Unity asset's float values (Unity UpdateMast).
/// </summary>
internal static class Stage5Mast
{
    public static Stage5MastState ForYear(int mastSeed, int year) =>
        FromRoll(Stage5Random.Stream(Stage5Random.MastKey, mastSeed, year).NextDouble());

    public static Stage5MastState FromRoll(double roll)
    {
        if (roll < Stage5Constants.MastGoodProbability) return new(roll, "good", Stage5Constants.MastGoodMultiplier);
        if (roll < Stage5Constants.MastGoodProbability + Stage5Constants.MastPoorProbability)
            return new(roll, "poor", Stage5Constants.MastPoorMultiplier);
        return new(roll, "normal", Stage5Constants.MastNormalMultiplier);
    }
}
