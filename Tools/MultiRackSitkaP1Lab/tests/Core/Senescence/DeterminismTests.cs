using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core.NaturalProcesses.Senescence;
using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using static SitkaP1.Core.Senescence.Tests.Fixtures;
using static SitkaP1.Core.Senescence.Tests.TestRunner;

namespace SitkaP1.Core.Senescence.Tests;

/// <summary>
/// AC-5: one canonical text over a fixed grid of engine outputs, hashed with SHA-256. Doubles are written as raw IEEE-754
/// bits so any change in the last bit changes the digest. Two separate processes must print the same digest.
/// </summary>
internal static class DeterminismTests
{
    private const string Suite = "Determinism";

    /// <summary>
    /// Digest observed on the first verified run after repair cycle 1 (64-bit key hash; SDK 8.0.133, linux-x64). The
    /// pre-repair digest 46da9802...b52878c is retired. Any output change must update it deliberately.
    /// The late-life continuation uses Math.Exp, so the pin is tied to the verified runtime and platform.
    /// </summary>
    internal const string PinnedDigest = "313e0f5a795bd7774752affb673927cc875bc6d89b9b988ddaadf3efa4e4bbb6";

    public static string CanonicalDigest() =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(CanonicalText()))).ToLowerInvariant();

    private static string Bits(double value) => BitConverter.DoubleToInt64Bits(value).ToString("X16", CultureInfo.InvariantCulture);

    private static string Bits(double? value) => value is { } v ? Bits(v) : "null";

    private static string CanonicalText()
    {
        var text = new StringBuilder();
        AppendHazardCurves(text);
        AppendGrowthGrid(text);
        AppendRolls(text);
        AppendPopulation(text);
        return text.ToString();
    }

    private static void AppendHazardCurves(StringBuilder text)
    {
        foreach (SenescenceSpeciesProfile p in SenescenceHazard.Profiles)
        {
            text.Append("P|").Append(p.SpeciesId).Append('|').Append(p.OnsetAgeYears).Append('|').Append(p.RampYears).Append('|')
                .Append(Bits(p.MaximumAnnualHazard)).Append('|').Append(p.Evidence).Append('\n');
            for (int age = 0; age <= 300; age++)
                text.Append("H|").Append(p.SpeciesId).Append('|').Append(age).Append('|')
                    .Append(Bits(SenescenceHazard.AnnualHazard(p.SpeciesId, age))).Append('\n');
            foreach (var (start, end) in new[] { (0, 300), (60, 70), (70, 150), (p.OnsetAgeYears, p.OnsetAgeYears + p.RampYears) })
                text.Append("S|").Append(p.SpeciesId).Append('|').Append(start).Append('|').Append(end).Append('|')
                    .Append(Bits(SenescenceHazard.SurvivalProbability(p.SpeciesId, start, end))).Append('\n');
        }
    }

    private static void AppendGrowthGrid(StringBuilder text)
    {
        foreach (string species in LateLifeGrowth.SupportedSpecies)
        {
            int cutoff = LateLifeGrowth.CutoffAgeYears(species);
            text.Append("GC|").Append(species).Append('|').Append(cutoff).Append('|')
                .Append(Bits(LateLifeGrowth.NominalIncrementAtOrBeyondCutoff(species, cutoff))).Append('|')
                .Append(Bits(LateLifeGrowth.WindowSlopeCmPerYear(species))).Append('|')
                .Append(Bits(LateLifeGrowth.ExponentialDecayRatePerYear(species))).Append('\n');
            for (int age = cutoff; age <= 300; age++)
                text.Append("GN|").Append(species).Append('|').Append(age).Append('|')
                    .Append(Bits(LateLifeGrowth.NominalIncrementAtOrBeyondCutoff(species, age))).Append('\n');

            (ValueBand Stems, ValueBand Basal, double MinTop) domain = species == Pine
                ? (FutureSpeciesCatalog.ScotsPine.Readiness.StemsPerHectare!.Value, FutureSpeciesCatalog.ScotsPine.Readiness.BasalAreaM2PerHa!.Value,
                   FutureSpeciesCatalog.ScotsPine.Readiness.MinimumTopHeightM!.Value)
                : (BirchAlderEngine.GrowthDomain(species).StemsPerHectare, BirchAlderEngine.GrowthDomain(species).BasalAreaM2PerHa,
                   BirchAlderEngine.GrowthDomain(species).MinimumTopHeightM);
            double Mid(ValueBand band) => band.Low + (band.High - band.Low) / 2;
            (double Stems, double Basal, string Label)[] crowding =
            {
                (domain.Stems.Low, domain.Basal.Low, "zero"),
                (Mid(domain.Stems), domain.Basal.Low, "stocking"),
                (domain.Stems.Low, Mid(domain.Basal), "basal"),
                (domain.Stems.High, domain.Basal.High, "full"),
                (domain.Stems.High + 1, domain.Basal.Low, "outside"),
            };
            int[] ages = { 4, 5, 10, 33, 42, 44, cutoff - 1, cutoff, cutoff + 1, cutoff + 2, cutoff + 5, cutoff + 10, cutoff + 20,
                           cutoff + 40, cutoff + 80, 150, 200, 250, 300 };
            foreach (int age in ages)
            foreach (var c in crowding)
            foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
            {
                LateLifeGrowthResult r = LateLifeGrowth.EvaluateGrowth(
                    new LateLifeGrowthRequest(species, age, c.Stems, c.Basal, domain.MinTop + 3, moisture));
                text.Append("G|").Append(species).Append('|').Append(age).Append('|').Append(c.Label).Append('|').Append(moisture).Append('|')
                    .Append(Bits(r.PotentialDbhIncrementCm)).Append('|').Append(Bits(r.SiteAdjustedDbhIncrementCm)).Append('|')
                    .Append(Bits(r.SiteResponse.EstablishmentMultiplier)).Append('|').Append(Bits(r.SiteResponse.GrowthMultiplier)).Append('|')
                    .Append(r.IsWithinCalibrationWindow).Append('|').Append(r.IsAvailable).Append('\n');
            }
        }
    }

    private static void AppendRolls(StringBuilder text)
    {
        foreach (string species in SixSpecies)
        foreach (string tree in new[] { "T0001", "T0002", "LANE-3/T0001", "X" })
        foreach (int year in new[] { 1, 2, 100 })
        foreach (int seed in new[] { 0, Seed })
            text.Append("R|").Append(SenescenceStreams.KeyFor(species, tree)).Append('|').Append(year).Append('|').Append(seed).Append('|')
                .Append(Bits(SenescenceEngine.RollUniform(species, tree, year, seed))).Append('\n');
    }

    /// <summary>
    /// A fixed 360-tree, six-species population run for 40 years: each year is evaluated, the dead are removed and the
    /// survivors age by one year. Ages start ten years before each species' onset so the ramp and ceiling are both covered.
    /// </summary>
    private static void AppendPopulation(StringBuilder text)
    {
        var alive = new List<SenescenceTreeInput>();
        int n = 0;
        foreach (SenescenceSpeciesProfile p in SenescenceHazard.Profiles)
            for (int i = 0; i < 60; i++)
                alive.Add(new SenescenceTreeInput($"P{n++:D4}", p.SpeciesId, p.OnsetAgeYears - 10 + i * 2));

        for (int year = 1; year <= 40; year++)
        {
            SenescenceResult r = Run(year, alive);
            double hazardSum = 0.0;
            foreach (SenescenceHazardEvaluation e in r.HazardEvaluations)
                hazardSum += e.AnnualHazard;
            text.Append("Y|").Append(year).Append('|').Append(r.Model).Append('|').Append(r.RngDrawCount).Append('|')
                .Append(r.TreesEvaluated).Append('|').Append(r.Deaths.Count).Append('|').Append(Bits(hazardSum)).Append('\n');
            foreach (SenescenceDeathRecord d in r.Deaths)
                text.Append("D|").Append(d.Year).Append('|').Append(d.TreeId).Append('|').Append(d.SpeciesId).Append('|')
                    .Append(d.Cause).Append('|').Append(d.PlayerCauseText).Append('|').Append(d.BiologicalDeath).Append('|')
                    .Append(d.TimberRevenueEur.ToString(CultureInfo.InvariantCulture)).Append('|')
                    .Append(d.ClearsCropTreeMarks).Append('|').Append(d.CountsAsFell).Append('\n');

            var dead = r.Deaths.Select(d => d.TreeId).ToHashSet(StringComparer.Ordinal);
            alive = alive.Where(t => !dead.Contains(t.TreeId)).Select(t => t with { AgeYears = t.AgeYears + 1 }).ToList();
        }

        SenescenceResult off = SenescenceEngine.Evaluate(new SenescenceRequest(41, Seed, SenescenceModelVersion.Disabled, alive));
        text.Append("X|").Append(alive.Count).Append('|').Append(off.Model).Append('|').Append(off.RngDrawCount).Append('|')
            .Append(off.TreesEvaluated).Append('|').Append(off.Deaths.Count).Append('\n');
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "DET-01 canonical output text and SHA-256 are identical across repeated evaluations and match the pinned digest", () =>
        {
            string first = CanonicalText();
            string second = CanonicalText();
            Eq(first, second, "canonical text");
            Eq(CanonicalDigest(), CanonicalDigest(), "digest");
            True(first.Length > 100_000, "canonical text covers hazards, growth, rolls and the multi-year population");
            Eq(PinnedDigest, CanonicalDigest(), "pinned digest");
        });

        TestRunner.Add(Suite, "DET-02 collision-hardened keyed roll: 64-bit key hash, the reviewer's 32-bit collision pair diverges, 20,000 ids give 20,000 distinct rolls", () =>
        {
            // Named regression (review of c1201e8f): under the old 32-bit FNV-1a key hash both keys hashed to 0xF88F6C5C, so the
            // two trees shared every roll in every year and seed.
            const string first = "MA-A-5B7Z", second = "MA-A-Q3QA";
            string keyA = SenescenceStreams.KeyFor(Alder, first), keyB = SenescenceStreams.KeyFor(Alder, second);
            Eq(0xF88F6C5Cu, Fnv1a32(keyA), "fixture: the pair really collides under 32-bit FNV-1a");
            Eq(0xF88F6C5Cu, Fnv1a32(keyB), "fixture: the pair really collides under 32-bit FNV-1a (second id)");
            True(SenescenceEngine.KeyHash64(keyA) != SenescenceEngine.KeyHash64(keyB), "64-bit key hashes differ");
            foreach (int seed in new[] { 0, Seed })
                for (int year = 1; year <= 5; year++)
                    True(SenescenceEngine.RollUniform(Alder, first, year, seed) != SenescenceEngine.RollUniform(Alder, second, year, seed),
                        $"the pair rolls differently in year {year}, seed {seed}");

            // The hash is plain 64-bit FNV-1a (offset 14695981039346656037, prime 1099511628211) over the whole key.
            Eq(14695981039346656037UL, SenescenceEngine.KeyHash64(""), "empty key is the offset basis");
            Eq(0xAF63DC4C8601EC8CUL, SenescenceEngine.KeyHash64("a"), "published FNV-1a 64 test vector for \"a\"");
            Eq(0x85944171F73967E8UL, SenescenceEngine.KeyHash64("foobar"), "published FNV-1a 64 test vector for \"foobar\"");
            Eq(0x7AD32CD93C9A71BCUL, SenescenceEngine.KeyHash64(keyA), "independently computed 64-bit hash of the first key");
            Eq(0xA5AFC5D9BBE8D8FCUL, SenescenceEngine.KeyHash64(keyB), "independently computed 64-bit hash of the second key");

            // Stable for the same inputs; any change of species, tree, year or seed changes the draw.
            double baseline = SenescenceEngine.RollUniform(Alder, first, 3, Seed);
            BitEq(baseline, SenescenceEngine.RollUniform(Alder, first, 3, Seed), "same inputs, same roll");
            BitEq(SenescenceEngine.RollKeyed(keyA, 3, Seed), baseline, "RollUniform is the keyed roll over the versioned key");
            True(baseline != SenescenceEngine.RollUniform(Birch, first, 3, Seed), "species changes the draw");
            True(baseline != SenescenceEngine.RollUniform(Alder, first + "X", 3, Seed), "tree id changes the draw");
            True(baseline != SenescenceEngine.RollUniform(Alder, first, 4, Seed), "year changes the draw");
            True(baseline != SenescenceEngine.RollUniform(Alder, first, 3, Seed + 1), "seed changes the draw");

            // Empirical collision check: 20,000 synthetic ids at one year and seed give 20,000 distinct rolls, all in [0, 1).
            var rolls = new HashSet<long>();
            double sum = 0.0;
            for (int i = 0; i < 20_000; i++)
            {
                double u = SenescenceEngine.RollUniform(Alder, $"MA-A-{i:X5}", 7, Seed);
                True(u >= 0.0 && u < 1.0, "roll in [0, 1)");
                rolls.Add(BitConverter.DoubleToInt64Bits(u));
                sum += u;
            }
            Eq(20_000, rolls.Count, "20,000 ids, 20,000 distinct rolls");
            ApproxEq(0.5, sum / 20_000, 0.01, "mean of 20,000 rolls");

            // The death rate stays sane: 20,000 age-200 alder (hazard 0.030) lose between 2 % and 4.3 % in one year.
            double rate = Run(7, Cohort(Alder, 200, 20_000, "MA-A-")).Deaths.Count / 20_000.0;
            True(rate >= 0.02 && rate <= 0.043, $"age-200 alder death rate {rate:R} in [0.02, 0.043]");
        });
    }

    /// <summary>The old 32-bit FNV-1a key hash, kept only to prove the regression fixture still collides under it.</summary>
    private static uint Fnv1a32(string key)
    {
        uint hash = 2166136261u;
        foreach (char c in key)
            hash = unchecked((hash ^ c) * 16777619u);
        return hash;
    }
}
