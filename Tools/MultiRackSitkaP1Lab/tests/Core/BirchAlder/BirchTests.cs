using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using static SitkaP1.Core.BirchAlder.Tests.TestRunner;

namespace SitkaP1.Core.BirchAlder.Tests;

internal static class BirchTests
{
    private const string Suite = "DownyBirch";
    private const string Species = BirchAlderSpecies.DownyBirch;
    private const ulong Seed = 0xC0FFEE20261127UL;

    private static BirchAlderGrowthRequest Growth(
        int age = 20,
        double stems = 200,
        double basalArea = 5,
        double topHeight = 12,
        SiteMoistureClass moisture = SiteMoistureClass.FreshMoistMineral) =>
        new(Species, age, stems, basalArea, topHeight, moisture);

    /// <summary>A gap that passes every birch gate except the seed year, which the caller picks.</summary>
    private static BirchAlderRegenerationRequest OpenGap(
        int year,
        SiteMoistureClass moisture = SiteMoistureClass.FreshMoistMineral) =>
        new(Species, Seed, year, OldestOnMapSeedTreeAgeYears: 20, CanopyOpenness: 0.80, UndergrowthCover: 0.10, moisture);

    private static int FirstYear(bool good)
    {
        for (int year = 0; year < 100; year++)
            if (BirchAlderEngine.IsGoodSeedYear(Species, Seed, year) == good)
                return year;
        throw new InvalidOperationException("no matching year in 0..99");
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "BIRCH-GROWTH-01a pioneer flush moderates with age; crowding strongly suppresses diameter", () =>
        {
            double young = BirchAlderEngine.EvaluateGrowth(Growth(age: 5)).PotentialDbhIncrementCm!.Value;
            double old = BirchAlderEngine.EvaluateGrowth(Growth(age: 70)).PotentialDbhIncrementCm!.Value;
            BitEq(BirchAlderEngine.BirchJuvenileIncrementCm, young, "uncrowded juvenile increment");
            BitEq(BirchAlderEngine.BirchAdultIncrementCm, old, "uncrowded adult increment");
            ApproxEq(BirchAlderEngine.BirchAdultIncrementCm * (1.0 - BirchAlderEngine.BirchMaximumCrowdingReduction),
                BirchAlderEngine.EvaluateGrowth(Growth(age: 70, stems: 5000, basalArea: 60)).PotentialDbhIncrementCm!.Value,
                1e-12, "fully crowded adult increment");

            double openYoung = BirchAlderEngine.EvaluateGrowth(Growth(age: 20, stems: 200, basalArea: 5)).PotentialDbhIncrementCm!.Value;
            double crowdedYoung = BirchAlderEngine.EvaluateGrowth(Growth(age: 20, stems: 5000, basalArea: 60)).PotentialDbhIncrementCm!.Value;
            True(crowdedYoung < openYoung, "crowding reduces the young increment");
            ApproxEq(openYoung * (1.0 - BirchAlderEngine.BirchMaximumCrowdingReduction), crowdedYoung, 1e-12,
                "full crowding applies the maximum reduction");

            double previous = double.MaxValue;
            for (int age = 5; age <= 70; age++)
            {
                double potential = BirchAlderEngine.EvaluateGrowth(Growth(age: age)).PotentialDbhIncrementCm!.Value;
                True(potential > 0.0 && potential < previous, $"increment falls monotonically at age {age}");
                previous = potential;
            }
        });

        TestRunner.Add(Suite, "BIRCH-GROWTH-01b the site growth modifier is applied once and NeutralLegacy leaves growth unchanged", () =>
        {
            BirchAlderGrowthResult fresh = BirchAlderEngine.EvaluateGrowth(Growth(moisture: SiteMoistureClass.FreshMoistMineral));
            BitEq(fresh.PotentialDbhIncrementCm!.Value, fresh.SiteAdjustedDbhIncrementCm!.Value, "fresh G is x1.0");

            BirchAlderGrowthResult bog = BirchAlderEngine.EvaluateGrowth(Growth(moisture: SiteMoistureClass.StagnantPoorBog));
            ApproxEq(bog.PotentialDbhIncrementCm!.Value * 0.35, bog.SiteAdjustedDbhIncrementCm!.Value, 1e-15, "bog L growth x0.35");
            True(bog.Site.HabitatProtectionCaution, "bog caution travels with the growth result");

            BirchAlderGrowthResult neutral = BirchAlderEngine.EvaluateGrowth(Growth(moisture: SiteMoistureClass.NeutralLegacy));
            BitEq(neutral.PotentialDbhIncrementCm!.Value, neutral.SiteAdjustedDbhIncrementCm!.Value, "neutral leaves growth unchanged");
            True(neutral.Evidence.Includes(EvidenceClass.Calibration), "tagged calibration");
        });

        TestRunner.Add(Suite, "BIRCH-GROWTH-02 growth is unavailable outside the provisional teaching domain", () =>
        {
            var outside = new[]
            {
                Growth(age: 4), Growth(age: 71),
                Growth(stems: 199), Growth(stems: 5001),
                Growth(basalArea: 4.9), Growth(basalArea: 60.1),
                Growth(topHeight: 3.99), Growth(topHeight: double.NaN),
                Growth(stems: double.NaN),
            };
            foreach (BirchAlderGrowthRequest request in outside)
            {
                BirchAlderGrowthResult result = BirchAlderEngine.EvaluateGrowth(request);
                True(!result.IsWithinTeachingDomain, "outside: " + request);
                True(result.SiteAdjustedDbhIncrementCm is null, "no adjusted value: " + request);
            }
            True(BirchAlderEngine.EvaluateGrowth(Growth(age: 5, stems: 200, basalArea: 5, topHeight: 4.0)).IsWithinTeachingDomain,
                "low corners are inclusive");
            True(BirchAlderEngine.EvaluateGrowth(Growth(age: 70, stems: 5000, basalArea: 60, topHeight: 4.0)).IsWithinTeachingDomain,
                "high corners are inclusive");
        });

        TestRunner.Add(Suite, "BIRCH-REGEN-01a blocked without a mature on-map seed tree; 12 years is enough", () =>
        {
            for (int year = 0; year < 40; year++)
            {
                Eq(BirchAlderRegenerationStatus.NoMatureLocalSeedTree,
                    BirchAlderEngine.EvaluateRegeneration(OpenGap(year) with { OldestOnMapSeedTreeAgeYears = null }).Status, $"no tree year {year}");
                Eq(BirchAlderRegenerationStatus.NoMatureLocalSeedTree,
                    BirchAlderEngine.EvaluateRegeneration(OpenGap(year) with { OldestOnMapSeedTreeAgeYears = 11, CanopyOpenness = 1.0, UndergrowthCover = 0.0 }).Status,
                    $"11-year tree year {year}");
            }
            Eq(12, BirchAlderEngine.BirchMinimumSeedTreeAgeYears, "birch seed tree threshold");
            Eq(12, BirchAlderEngine.MinimumSeedTreeAgeYears(Species), "threshold by id");
            BitEq(0.0, BirchAlderEngine.EvaluateRegeneration(OpenGap(FirstYear(true)) with { OldestOnMapSeedTreeAgeYears = null }).EstablishmentMultiplier,
                "blocked gap establishes nothing");
            Eq(BirchAlderRegenerationStatus.Eligible,
                BirchAlderEngine.EvaluateRegeneration(OpenGap(FirstYear(true)) with { OldestOnMapSeedTreeAgeYears = 12 }).Status, "12-year tree is enough");
        });

        TestRunner.Add(Suite, "BIRCH-REGEN-01b blocked in a poor seed year, a dark opening or high competing vegetation", () =>
        {
            int good = FirstYear(true);
            Eq(BirchAlderRegenerationStatus.PoorSeedYear, BirchAlderEngine.EvaluateRegeneration(OpenGap(FirstYear(false))).Status, "poor year");
            Eq(BirchAlderRegenerationStatus.CanopyTooDark, BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.64 }).Status, "dark opening");
            Eq(BirchAlderRegenerationStatus.CanopyTooDark, BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.0 }).Status, "closed canopy");
            Eq(BirchAlderRegenerationStatus.TooMuchUndergrowth, BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { UndergrowthCover = 0.36 }).Status, "high vegetation");
            Eq(BirchAlderRegenerationStatus.Eligible,
                BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.65, UndergrowthCover = 0.35 }).Status, "thresholds inclusive");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 1.1 }), "openness > 1");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { UndergrowthCover = double.NaN }), "NaN cover");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.EvaluateRegeneration(OpenGap(good) with { OldestOnMapSeedTreeAgeYears = -1 }), "negative age");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.IsGoodSeedYear(Species, Seed, -1), "negative year");
        });

        TestRunner.Add(Suite, "BIRCH-REGEN-01c eligible gaps return the site establishment modifier, including a cautioned bog", () =>
        {
            int good = FirstYear(true);
            BitEq(1.00, BirchAlderEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.FreshMoistMineral)).EstablishmentMultiplier, "fresh G");
            BitEq(1.00, BirchAlderEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.SeasonallyWetAerated)).EstablishmentMultiplier, "seasonally wet G");
            BitEq(1.00, BirchAlderEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.WetMovingAlluvial)).EstablishmentMultiplier, "wet alluvial establishment G");

            BirchAlderRegenerationResult bog = BirchAlderEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.StagnantPoorBog));
            Eq(BirchAlderRegenerationStatus.Eligible, bog.Status, "birch can colonise drained peat; it is not gated on mineral ground");
            BitEq(0.70, bog.EstablishmentMultiplier, "bog M establishment");
            True(bog.HabitatProtectionCaution, "but the bog caution is always reported");
        });

        TestRunner.Add(Suite, "BIRCH-REGEN-02a good seed years recur every 3-5 years: one per four-year block", () =>
        {
            ulong[] seeds = { 0UL, 1UL, Seed, ulong.MaxValue, 0x5EED5EED5EED5EEDUL };
            foreach (ulong seed in seeds)
            {
                var goodYears = Enumerable.Range(0, 400).Where(y => BirchAlderEngine.IsGoodSeedYear(Species, seed, y)).ToList();
                Eq(100, goodYears.Count, $"seed {seed:X}: one good year per block");
                for (int block = 0; block < 100; block++)
                    Eq(1, goodYears.Count(y => y / 4 == block), $"seed {seed:X} block {block}");
                for (int i = 1; i < goodYears.Count; i++)
                {
                    int gap = goodYears[i] - goodYears[i - 1];
                    True(gap is >= 3 and <= 5, $"seed {seed:X}: gap {gap} between {goodYears[i - 1]} and {goodYears[i]}");
                }
            }
        });

        TestRunner.Add(Suite, "BIRCH-REGEN-02b an open gap regenerates exactly in good seed years, and birch and alder schedules differ", () =>
        {
            int eligibleYears = 0;
            for (int year = 0; year < 40; year++)
            {
                BirchAlderRegenerationResult result = BirchAlderEngine.EvaluateRegeneration(OpenGap(year));
                Eq(BirchAlderEngine.IsGoodSeedYear(Species, Seed, year), result.CanRegenerate, $"year {year}");
                Eq(result.GoodSeedYear, result.CanRegenerate, $"year {year} flag");
                if (result.CanRegenerate) eligibleYears++;
            }
            Eq(10, eligibleYears, "ten good years in forty");

            bool anyDifference = false;
            for (ulong seed = 1; seed <= 16 && !anyDifference; seed++)
                anyDifference = Enumerable.Range(0, 40).Any(y => BirchAlderEngine.IsGoodSeedYear(Species, seed, y) != BirchAlderEngine.IsGoodSeedYear(Species, Seed, y));
            True(anyDifference, "woodland seed changes the schedule");

            bool speciesDiffer = Enumerable.Range(0, 40).Any(y =>
                BirchAlderEngine.IsGoodSeedYear(Species, Seed, y) !=
                BirchAlderEngine.IsGoodSeedYear(BirchAlderSpecies.CommonAlder, Seed, y));
            True(speciesDiffer, "birch and alder do not share one mast calendar");
        });

        TestRunner.Add(Suite, "BIRCH-REGEN-02c identical requests give identical results in any query order", () =>
        {
            var forward = Enumerable.Range(0, 60).Select(y => BirchAlderEngine.EvaluateRegeneration(OpenGap(y))).ToList();
            var backward = Enumerable.Range(0, 60).Reverse().Select(y => BirchAlderEngine.EvaluateRegeneration(OpenGap(y))).Reverse().ToList();
            for (int i = 0; i < forward.Count; i++)
                Eq(forward[i], backward[i], $"year {i}");
        });
    }
}
