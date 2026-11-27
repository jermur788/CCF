using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using static SitkaP1.Core.BirchAlder.Tests.TestRunner;

namespace SitkaP1.Core.BirchAlder.Tests;

internal static class AlderTests
{
    private const string Suite = "CommonAlder";
    private const string Species = BirchAlderSpecies.CommonAlder;
    private const ulong Seed = 0xC0FFEE20261127UL;

    private static BirchAlderGrowthRequest Growth(
        int age = 20,
        double stems = 200,
        double basalArea = 5,
        double topHeight = 12,
        SiteMoistureClass moisture = SiteMoistureClass.WetMovingAlluvial) =>
        new(Species, age, stems, basalArea, topHeight, moisture);

    /// <summary>A gap that passes every alder gate except the seed year, which the caller picks.</summary>
    private static BirchAlderRegenerationRequest OpenGap(
        int year,
        SiteMoistureClass moisture = SiteMoistureClass.WetMovingAlluvial) =>
        new(Species, Seed, year, OldestOnMapSeedTreeAgeYears: 20, CanopyOpenness: 0.80, UndergrowthCover: 0.10,
            moisture, ExposedMineralGround: true);

    private static int FirstYear(bool good)
    {
        for (int year = 0; year < 100; year++)
            if (BirchAlderEngine.IsGoodSeedYear(Species, Seed, year) == good)
                return year;
        throw new InvalidOperationException("no matching year in 0..99");
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "ALDER-GROWTH-01a very fast juvenile growth then a pronounced adult slowdown", () =>
        {
            double young = BirchAlderEngine.EvaluateGrowth(Growth(age: 5)).PotentialDbhIncrementCm!.Value;
            double old = BirchAlderEngine.EvaluateGrowth(Growth(age: 60)).PotentialDbhIncrementCm!.Value;
            BitEq(BirchAlderEngine.AlderJuvenileIncrementCm, young, "uncrowded juvenile increment");
            ApproxEq(BirchAlderEngine.AlderAdultIncrementCm, old, 1e-12, "uncrowded adult increment");
            True(young > BirchAlderEngine.BirchJuvenileIncrementCm, "alder starts faster than birch");
            True(old / young <= 0.25, "the adult increment is at most a quarter of the juvenile one");

            double previous = double.MaxValue;
            for (int age = 5; age <= 60; age++)
            {
                double potential = BirchAlderEngine.EvaluateGrowth(Growth(age: age)).PotentialDbhIncrementCm!.Value;
                True(potential > 0.0 && potential < previous, $"increment falls monotonically at age {age}");
                previous = potential;
            }
        });

        TestRunner.Add(Suite, "ALDER-GROWTH-01b crowding reduces growth, the site modifier is applied once, wet ground is x1.0", () =>
        {
            double open = BirchAlderEngine.EvaluateGrowth(Growth(age: 20, stems: 200, basalArea: 5)).PotentialDbhIncrementCm!.Value;
            double crowded = BirchAlderEngine.EvaluateGrowth(Growth(age: 20, stems: 4000, basalArea: 55)).PotentialDbhIncrementCm!.Value;
            ApproxEq(open * (1.0 - BirchAlderEngine.AlderMaximumCrowdingReduction), crowded, 1e-12, "full crowding");

            BirchAlderGrowthResult wet = BirchAlderEngine.EvaluateGrowth(Growth(moisture: SiteMoistureClass.WetMovingAlluvial));
            BitEq(wet.PotentialDbhIncrementCm!.Value, wet.SiteAdjustedDbhIncrementCm!.Value, "wet alluvial G is x1.0");

            BirchAlderGrowthResult bog = BirchAlderEngine.EvaluateGrowth(Growth(moisture: SiteMoistureClass.StagnantPoorBog));
            ApproxEq(bog.PotentialDbhIncrementCm!.Value * 0.10, bog.SiteAdjustedDbhIncrementCm!.Value, 1e-15, "bog X growth x0.10");
            True(bog.Site.HabitatProtectionCaution, "bog caution travels with the result");

            BirchAlderGrowthResult neutral = BirchAlderEngine.EvaluateGrowth(Growth(moisture: SiteMoistureClass.NeutralLegacy));
            BitEq(neutral.PotentialDbhIncrementCm!.Value, neutral.SiteAdjustedDbhIncrementCm!.Value, "neutral leaves growth unchanged");
        });

        TestRunner.Add(Suite, "ALDER-GROWTH-02 growth is unavailable outside the provisional teaching domain", () =>
        {
            var outside = new[]
            {
                Growth(age: 4), Growth(age: 61),
                Growth(stems: 199), Growth(stems: 4001),
                Growth(basalArea: 4.9), Growth(basalArea: 55.1),
                Growth(topHeight: 4.99), Growth(topHeight: double.NegativeInfinity),
                Growth(basalArea: double.NaN),
            };
            foreach (BirchAlderGrowthRequest request in outside)
            {
                BirchAlderGrowthResult result = BirchAlderEngine.EvaluateGrowth(request);
                True(!result.IsWithinTeachingDomain, "outside: " + request);
                True(result.SiteAdjustedDbhIncrementCm is null, "no adjusted value: " + request);
            }
            True(BirchAlderEngine.EvaluateGrowth(Growth(age: 5, stems: 200, basalArea: 5, topHeight: 5.0)).IsWithinTeachingDomain,
                "low corners are inclusive");
            True(BirchAlderEngine.EvaluateGrowth(Growth(age: 60, stems: 4000, basalArea: 55, topHeight: 5.0)).IsWithinTeachingDomain,
                "high corners are inclusive");
        });

        TestRunner.Add(Suite, "ALDER-REGEN-01a needs a mature on-map seed tree; 10 years is enough", () =>
        {
            Eq(10, BirchAlderEngine.AlderMinimumSeedTreeAgeYears, "alder seed tree threshold");
            Eq(10, BirchAlderEngine.MinimumSeedTreeAgeYears(Species), "threshold by id");
            for (int year = 0; year < 40; year++)
            {
                Eq(BirchAlderRegenerationStatus.NoMatureLocalSeedTree,
                    BirchAlderEngine.EvaluateRegeneration(OpenGap(year) with { OldestOnMapSeedTreeAgeYears = null }).Status, $"no tree year {year}");
                Eq(BirchAlderRegenerationStatus.NoMatureLocalSeedTree,
                    BirchAlderEngine.EvaluateRegeneration(OpenGap(year) with { OldestOnMapSeedTreeAgeYears = 9 }).Status, $"nine-year tree year {year}");
            }
            Eq(BirchAlderRegenerationStatus.Eligible,
                BirchAlderEngine.EvaluateRegeneration(OpenGap(FirstYear(true)) with { OldestOnMapSeedTreeAgeYears = 10 }).Status, "ten-year tree is enough");
        });

        TestRunner.Add(Suite, "ALDER-REGEN-01b needs light, non-stagnant moisture and exposed mineral ground", () =>
        {
            int good = FirstYear(true);

            Eq(BirchAlderRegenerationStatus.PoorSeedYear, BirchAlderEngine.EvaluateRegeneration(OpenGap(FirstYear(false))).Status, "poor year");
            Eq(BirchAlderRegenerationStatus.CanopyTooDark, BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.54 }).Status, "dark opening");

            Eq(BirchAlderRegenerationStatus.MoistureTooLow,
                BirchAlderEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.DryFreeDraining)).Status, "dry ground");
            BirchAlderRegenerationResult bog = BirchAlderEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.StagnantPoorBog));
            Eq(BirchAlderRegenerationStatus.BogNotAForestCrop, bog.Status, "stagnant bog is refused");
            True(bog.HabitatProtectionCaution, "and still reports the caution");
            BitEq(0.0, bog.EstablishmentMultiplier, "blocked bog establishes nothing");

            Eq(BirchAlderRegenerationStatus.NoExposedMineralGround,
                BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { ExposedMineralGround = false }).Status, "vegetated ground");
            Eq(BirchAlderRegenerationStatus.TooMuchUndergrowth,
                BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { UndergrowthCover = 0.41 }).Status, "high competition");

            Eq(BirchAlderRegenerationStatus.Eligible,
                BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.55, UndergrowthCover = 0.40 }).Status, "thresholds inclusive");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { UndergrowthCover = -0.1 }), "negative cover");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.IsGoodSeedYear(Species, Seed, -3), "negative year");
        });

        TestRunner.Add(Suite, "ALDER-REGEN-01c eligible gaps on all three wet classes return the site establishment modifier", () =>
        {
            int good = FirstYear(true);
            foreach (SiteMoistureClass moisture in new[]
                     {
                         SiteMoistureClass.WetMovingAlluvial,
                         SiteMoistureClass.FreshMoistMineral,
                         SiteMoistureClass.SeasonallyWetAerated,
                     })
            {
                BirchAlderRegenerationResult result = BirchAlderEngine.EvaluateRegeneration(OpenGap(good, moisture));
                Eq(BirchAlderRegenerationStatus.Eligible, result.Status, moisture + " status");
                BitEq(1.00, result.EstablishmentMultiplier, moisture + " establishment G");
                True(!result.HabitatProtectionCaution, moisture + " has no caution");
            }
        });

        TestRunner.Add(Suite, "ALDER-REGEN-02 good seed years recur every 3-5 years and differ from birch", () =>
        {
            ulong[] seeds = { 0UL, 1UL, Seed, ulong.MaxValue };
            foreach (ulong seed in seeds)
            {
                var goodYears = Enumerable.Range(0, 400).Where(y => BirchAlderEngine.IsGoodSeedYear(Species, seed, y)).ToList();
                Eq(100, goodYears.Count, $"seed {seed:X}: one good year per block");
                for (int i = 1; i < goodYears.Count; i++)
                {
                    int gap = goodYears[i] - goodYears[i - 1];
                    True(gap is >= 3 and <= 5, $"seed {seed:X}: gap {gap}");
                }
            }
            int alderYears = Enumerable.Range(0, 40).Count(y => BirchAlderEngine.IsGoodSeedYear(Species, Seed, y));
            Eq(10, alderYears, "ten good years in forty");
        });

        TestRunner.Add(Suite, "ROT-01 heart-rot risk is zero before 50, rises linearly to the maximum at 60, then stays flat", () =>
        {
            for (int age = 0; age < 50; age++)
                BitEq(0.0, BirchAlderEngine.AlderHeartRotRiskProbability(age), $"no risk at {age}");
            BitEq(BirchAlderEngine.AlderHeartRotMaximumProbability, BirchAlderEngine.AlderHeartRotRiskProbability(60), "maximum at 60");
            BitEq(BirchAlderEngine.AlderHeartRotMaximumProbability, BirchAlderEngine.AlderHeartRotRiskProbability(120), "flat after 60");
            ApproxEq(BirchAlderEngine.AlderHeartRotMaximumProbability / 2, BirchAlderEngine.AlderHeartRotRiskProbability(55), 1e-12, "halfway at 55");

            double previous = -1.0;
            for (int age = 50; age <= 60; age++)
            {
                double risk = BirchAlderEngine.AlderHeartRotRiskProbability(age);
                True(risk >= previous, $"risk does not fall at {age}");
                previous = risk;
            }
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.AlderHeartRotRiskProbability(-1), "negative age");
        });

        TestRunner.Add(Suite, "ROT-02 the per-stem roll never fires before 50, matches the risk rate and is order-independent", () =>
        {
            for (ulong stem = 0; stem < 1000; stem++)
                True(!BirchAlderEngine.InspectAlderHeartRot(stem, 49), "no downgrade at 49 for stem " + stem);

            const int sample = 10000;
            int at55 = 0, at65 = 0;
            for (ulong stem = 0; stem < sample; stem++)
            {
                if (BirchAlderEngine.InspectAlderHeartRot(stem, 55)) at55++;
                if (BirchAlderEngine.InspectAlderHeartRot(stem, 65)) at65++;
            }
            ApproxEq(0.175, at55 / (double)sample, 0.03, "about 17.5% downgrade at 55");
            ApproxEq(BirchAlderEngine.AlderHeartRotMaximumProbability, at65 / (double)sample, 0.04, "about 35% at 65");

            var forward = Enumerable.Range(0, 200).Select(i => BirchAlderEngine.InspectAlderHeartRot((ulong)i, 56)).ToList();
            var backward = Enumerable.Range(0, 200).Reverse().Select(i => BirchAlderEngine.InspectAlderHeartRot((ulong)i, 56)).Reverse().ToList();
            for (int i = 0; i < forward.Count; i++)
                Eq(forward[i], backward[i], "stem " + i);
            True(forward.Contains(true) && forward.Contains(false), "the roll produces both outcomes at 56");
        });
    }
}
