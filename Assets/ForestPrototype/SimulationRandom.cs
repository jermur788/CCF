// Seeds for the annual simulation's random draws.
//
// Model 0 is the original scheme (seed * 397 ^ year, FNV-style hashes). It is
// kept bit-for-bit so legacy saves, Reference Future v1 and the regression
// anchors replay unchanged. Consecutive years seeded this way produce visibly
// correlated first draws from System.Random, which biases mast and survival rolls.
//
// Model 1 passes every seed through a splitmix64 finaliser so neighbouring
// seeds, years and ids give independent draws. It is opt-in and stored in the
// save (ForestSaveData.rngModelVersion); a save without the field is model 0.
public static class SimulationRandom
{
    public const int LegacyModel = 0;
    public const int MixedModel = 1;
    public const int LatestModel = MixedModel;

    public static int NormalizeModel(int model)
    {
        return model < LegacyModel ? LegacyModel : model > LatestModel ? LatestModel : model;
    }

    // Seed for a System.Random. salt carries species or phase identity (0 if none).
    public static int Seed(int model, int simulationSeed, int year, int salt)
    {
        if (model == LegacyModel)
            return unchecked((simulationSeed * 397) ^ year ^ salt);
        ulong state = unchecked((ulong)(uint)simulationSeed * 0x9E3779B97F4A7C15UL);
        state = Mix(state ^ (ulong)(uint)year);
        state = Mix(state ^ (ulong)(uint)salt);
        return unchecked((int)(state >> 32));
    }

    public static System.Random Create(int model, int simulationSeed, int year, int salt)
    {
        return new System.Random(Seed(model, simulationSeed, year, salt));
    }

    // Uniform [0,1) from a string id, year and seed. Model 0 is the original hash.
    public static float Roll(int model, string id, int year, int simulationSeed)
    {
        uint hash = 2166136261u;
        foreach (char c in id ?? "")
            hash = unchecked((hash ^ c) * 16777619u);
        if (model == LegacyModel)
        {
            hash = unchecked((hash ^ (uint)year) * 16777619u);
            hash = unchecked((hash ^ (uint)simulationSeed) * 16777619u);
            return (hash & 0xFFFFFFu) / 16777216f;
        }
        ulong state = Mix(((ulong)hash << 32) ^ (ulong)(uint)year);
        state = Mix(state ^ ((ulong)(uint)simulationSeed * 0x9E3779B97F4A7C15UL));
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
