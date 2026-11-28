namespace SitkaP1.Core.NaturalProcesses.Senescence;

/// <summary>
/// Pure, deterministic annual evaluator for natural old-age tree death (SPEC-multi-2d-senescence-late-growth-v1).
///
/// Version gate: <see cref="SenescenceModelVersion.Disabled"/> (default) emits zero deaths, draws zero RNG, and
/// skips all species/age inspection so frozen baselines and legacy saves remain byte-identical.
///
/// Determinism: <see cref="SenescenceModelVersion.OldAgeV1"/> makes exactly one stateless keyed roll per tree per year,
/// mirroring the Model 1 hashing approach of <c>NaturalProcessRandom.cs</c> (FNV-1a over the versioned key string,
/// followed by SplitMix64 mixing with year and seed, returning a uniform 53-bit double in [0, 1)). No
/// <see cref="Random"/>, clock, or I/O is used, and input tree order cannot affect outcomes.
/// </summary>
public static class SenescenceEngine
{
    /// <summary>Evaluates one simulation year of natural old-age mortality for the supplied trees.</summary>
    public static SenescenceResult Evaluate(SenescenceRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (!Enum.IsDefined(request.Model))
            throw new ArgumentOutOfRangeException(nameof(request), request.Model, "Unknown senescence model version.");
        ArgumentOutOfRangeException.ThrowIfLessThan(request.Year, 1);

        if (request.Model == SenescenceModelVersion.Disabled)
        {
            return new SenescenceResult(
                SenescenceModelVersion.Disabled,
                Array.Empty<SenescenceDeathRecord>(),
                RngDrawCount: 0,
                TreesEvaluated: 0,
                HazardEvaluations: Array.Empty<SenescenceHazardEvaluation>());
        }

        ArgumentNullException.ThrowIfNull(request.Trees);

        int count = request.Trees.Count;
        var validated = new SenescenceTreeInput[count];
        var seenIds = new HashSet<string>(StringComparer.Ordinal);

        for (int i = 0; i < count; i++)
        {
            SenescenceTreeInput? tree = request.Trees[i];
            if (tree is null)
                throw new ArgumentException($"Tree input at index {i} is null.", nameof(request));
            if (string.IsNullOrWhiteSpace(tree.TreeId))
                throw new ArgumentException($"Tree input at index {i} has a blank TreeId.", nameof(request));
            if (!seenIds.Add(tree.TreeId))
                throw new ArgumentException($"Duplicate TreeId '{tree.TreeId}'.", nameof(request));
            ArgumentOutOfRangeException.ThrowIfNegative(tree.AgeYears, nameof(request));
            _ = SenescenceHazard.GetProfile(tree.SpeciesId);
            validated[i] = tree;
        }

        // Canonical ordinal order by TreeId so caller collection order can never affect output order or fate.
        Array.Sort(validated, static (a, b) => StringComparer.Ordinal.Compare(a.TreeId, b.TreeId));

        var deaths = new List<SenescenceDeathRecord>();
        var evaluations = new SenescenceHazardEvaluation[count];
        int drawCount = 0;

        for (int i = 0; i < count; i++)
        {
            SenescenceTreeInput tree = validated[i];
            SenescenceSpeciesProfile profile = SenescenceHazard.GetProfile(tree.SpeciesId);
            double hazard = SenescenceHazard.AnnualHazard(tree.SpeciesId, tree.AgeYears);
            evaluations[i] = new SenescenceHazardEvaluation(
                tree.TreeId, tree.SpeciesId, tree.AgeYears, hazard, profile.Evidence);

            string key = SenescenceStreams.KeyFor(tree.SpeciesId, tree.TreeId);
            double roll = RollModel1(key, request.Year, request.Seed);
            drawCount++;

            if (hazard > 0.0 && roll < hazard)
                deaths.Add(new SenescenceDeathRecord(tree.TreeId, tree.SpeciesId, request.Year));
        }

        return new SenescenceResult(
            request.Model,
            deaths,
            drawCount,
            count,
            evaluations);
    }

    /// <summary>
    /// Pure deterministic Model 1 uniform roll in [0, 1) keyed by (speciesId, treeId, year, seed) under
    /// <see cref="SenescenceStreams.OldAgeTreePrefix"/>.
    /// </summary>
    public static double RollUniform(string speciesId, string treeId, int year, int seed)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(year, 1);
        string key = SenescenceStreams.KeyFor(speciesId, treeId);
        return RollModel1(key, year, seed);
    }

    /// <summary>
    /// The shared Model 1 hash over any key. Internal (not private) only so the standalone test executable, which compiles
    /// these sources directly, can prove the mixed state is bit-identical to the frozen Model 1 vectors.
    /// </summary>
    internal static double RollModel1(string key, int year, int seed)
    {
        // Mirrors NaturalProcessRandom.RollModel1: FNV-1a 32-bit over key chars, then SplitMix64 mixing with year and
        // seed. Model 1 keeps the top 24 bits of that state as a float; this roll keeps the top 53 bits, so it is a
        // uniform double in [0, 1) with full IEEE-754 resolution against small residual hazards.
        uint hash = 2166136261u;
        foreach (char c in key)
            hash = unchecked((hash ^ c) * 16777619u);

        ulong state = Mix(((ulong)hash << 32) ^ (ulong)(uint)year);
        state = Mix(state ^ unchecked((ulong)(uint)seed * 0x9E3779B97F4A7C15UL));
        return (state >> 11) * (1.0 / 9007199254740992.0);
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
