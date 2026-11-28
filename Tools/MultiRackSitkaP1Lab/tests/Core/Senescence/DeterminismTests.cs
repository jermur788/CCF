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
    /// Digest observed on the first verified run (SDK 8.0.133, linux-x64). Any output change must update it deliberately.
    /// The late-life continuation uses Math.Exp, so the pin is tied to the verified runtime and platform.
    /// </summary>
    internal const string PinnedDigest = "46da98026d8e591702ba09cf272bf3b0a76cd4f969ea5c0e1a4825197b52878c";

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

        TestRunner.Add(Suite, "DET-02 the keyed roll reproduces frozen SimulationRandom model 1 bit-for-bit and widens it to a 53-bit uniform double in [0, 1)", () =>
        {
            // Frozen model 1 vectors (key, year, seed, bits of the model 1 float), copied verbatim at base b2006767 from
            // tests/Core/NaturalProcesses/HygieneAndParityTests.cs (JW-T60), which pins them to SimulationRandom.Roll model 1.
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
            {
                double u = SenescenceEngine.RollModel1(v.Key, v.Year, v.Seed);
                True(u >= 0.0 && u < 1.0, v.Key + " in [0, 1)");
                // Model 1 keeps the top 24 bits of the same mixed state as a float; this roll keeps the top 53.
                float model1 = (float)(Math.Floor(u * 16777216.0) / 16777216.0);
                Eq(v.Bits, BitConverter.SingleToInt32Bits(model1), v.Key + "@" + v.Year + "/" + v.Seed);
            }

            BitEq(SenescenceEngine.RollModel1(SenescenceStreams.KeyFor(Alder, "T0001"), 3, Seed), SenescenceEngine.RollUniform(Alder, "T0001", 3, Seed),
                "RollUniform is model 1 over the versioned key");

            double sum = 0.0, min = 1.0, max = 0.0;
            for (int i = 0; i < 20_000; i++)
            {
                double u = SenescenceEngine.RollUniform(SixSpecies[i % 6], $"U{i:D5}", 1 + i % 40, Seed);
                sum += u;
                min = Math.Min(min, u);
                max = Math.Max(max, u);
            }
            True(min >= 0.0 && max < 1.0, "20,000 draws stay in [0, 1)");
            ApproxEq(0.5, sum / 20_000, 0.01, "mean of 20,000 draws");
        });
    }
}
