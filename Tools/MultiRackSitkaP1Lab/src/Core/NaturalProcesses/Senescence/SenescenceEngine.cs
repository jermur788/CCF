namespace SitkaP1.Core.NaturalProcesses.Senescence;

/// <summary>
/// Pure, deterministic annual evaluator for natural old-age tree death (SPEC-multi-2d-senescence-late-growth-v1).
///
/// Version gate: <see cref="SenescenceModelVersion.Disabled"/> (default) emits zero deaths, draws zero RNG, and
/// skips all species/age inspection so frozen baselines and legacy saves remain byte-identical.
///
/// Determinism: <see cref="SenescenceModelVersion.OldAgeV1"/> makes exactly one stateless keyed roll per tree per year:
/// 64-bit FNV-1a over the versioned key string, then the same SplitMix64 mixing with year and seed that Model 1 uses,
/// returning a uniform 53-bit double in [0, 1). No <see cref="Random"/>, clock, or I/O is used, and input tree order
/// cannot affect outcomes.
///
/// Why 64-bit, not Model 1's 32-bit key hash: with a 32-bit hash two different tree ids can share a key hash (for example
/// <c>common-alder/MA-A-5B7Z</c> and <c>common-alder/MA-A-Q3QA</c> both hash to 0xF88F6C5C), and because year and seed
/// are mixed in afterwards such a pair would share every roll in every year. For a fixed year and seed the map from the
/// 64-bit key hash to the 64-bit mixed state is a bijection, so trees can only share a state if their full 64-bit key
/// hashes collide. Nothing is truncated to 32 bits before the final 53-bit uniform conversion.
///
/// Version discipline (D-050 rule 4): OldAgeV1 and the prefix <c>SENESCENCE-OLD-AGE-v1-</c> have never been accepted,
/// registered in RngSalts or persisted. This collision-hardened roll is part of v1's pre-registration definition. Any
/// change to the roll after Part 2 integration requires a new model version (OldAgeV2), never an edit to v1.
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
            double roll = RollKeyed(key, request.Year, request.Seed);
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
    /// Pure deterministic uniform roll in [0, 1) keyed by (speciesId, treeId, year, seed) under
    /// <see cref="SenescenceStreams.OldAgeTreePrefix"/>.
    /// </summary>
    public static double RollUniform(string speciesId, string treeId, int year, int seed)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(year, 1);
        string key = SenescenceStreams.KeyFor(speciesId, treeId);
        return RollKeyed(key, year, seed);
    }

    /// <summary>The 64-bit FNV-1a hash of every UTF-16 code unit of <paramref name="key"/>.</summary>
    internal static ulong KeyHash64(string key)
    {
        ulong hash = 14695981039346656037UL;
        foreach (char c in key)
            hash = unchecked((hash ^ c) * 1099511628211UL);
        return hash;
    }

    /// <summary>
    /// The keyed roll over a full key. Internal (not private) only so the standalone test executable, which compiles these
    /// sources directly, can test the hash and the mixing separately.
    /// </summary>
    internal static double RollKeyed(string key, int year, int seed)
    {
        // 64-bit key hash, then Model 1's SplitMix64 mixing with year and seed. Each step is a bijection on 64 bits for a
        // fixed year and seed, so distinct key hashes always give distinct mixed states. The top 53 bits give a uniform
        // double in [0, 1) with full IEEE-754 resolution against small residual hazards.
        ulong state = Mix(KeyHash64(key) ^ (ulong)(uint)year);
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
