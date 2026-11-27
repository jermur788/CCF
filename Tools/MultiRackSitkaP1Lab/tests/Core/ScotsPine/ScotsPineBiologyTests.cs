using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;
using static SitkaP1.Core.ScotsPine.Tests.TestRunner;

namespace SitkaP1.Core.ScotsPine.Tests;

internal static class ScotsPineBiologyTests
{
    private const string Suite = "ScotsPineBiology";
    private const ulong Seed = 0xC0FFEE20261127UL;

    /// <summary>Request that passes every regeneration gate except the seed year, which the caller picks.</summary>
    private static ScotsPineRegenerationRequest OpenGap(int year, SiteMoistureClass moisture = SiteMoistureClass.DryFreeDraining) =>
        new(Seed, year, OldestOnMapSeedTreeAgeYears: 25, CanopyOpenness: 0.80, UndergrowthCover: 0.10, moisture);

    private static int FirstYear(bool good)
    {
        for (int year = 0; year < 100; year++)
            if (ScotsPineEngine.IsGoodSeedYear(Seed, year) == good)
                return year;
        throw new InvalidOperationException("no matching year in 0..99");
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "PINE-01 engine reuses FutureSpeciesCatalog.ScotsPine and its locked traits", () =>
        {
            True(ReferenceEquals(FutureSpeciesCatalog.ScotsPine, ScotsPineEngine.Profile), "same card instance");
            SpeciesProfileCard card = ScotsPineEngine.Profile;
            Eq(SpeciesMoistureTable.ScotsPine, card.SpeciesId, "species id");
            True(card.Crown is ScotsPineCrownProfile, "Scots pine crown model");
            ApproxEq((0.6874 + 0.2358 * 20 - 0.0011 * 400) / 2, card.Crown.ComputePotentialCrownRadiusM(20), 1e-12, "R = CW/2 at D=20");
            BitEq(card.Crown.ComputePotentialCrownRadiusM(58.6), card.Crown.ComputePotentialCrownRadiusM(90), "clamp at 58.6 cm");
            BitEq(4.75, card.Crown.Ci50Default, "Ci50");
            Eq(ShadeTolerance.LightModerate, card.Crown.ShadeTolerance, "shade casting");
            Eq(Level.High, card.Silviculture.LightDemand, "light demand");
            Eq(Level.MediumHigh, card.Silviculture.BrowseSensitivity, "deer browse");
            Eq(new ValueBand(5.0, 6.0), card.Silviculture.PruningTargetHeightM, "pruning target");
            Eq(ExternalColonisation.AuthoredOnly, card.Colonisation.ExternalColonisation, "no off-map colonisation");
            True(card.Colonisation.LocalOnMapSeedDispersalAllowed, "local seed allowed");
        });

        TestRunner.Add(Suite, "GROWTH-01 growth inside the GROWFOR domain: shape, crowding and single moisture modifier", () =>
        {
            ScotsPineGrowthResult young = ScotsPineEngine.EvaluateGrowth(new(10, 100, 10, 5.0, SiteMoistureClass.DryFreeDraining));
            True(young.IsWithinCalibrationDomain, "domain low corner");
            BitEq(0.60, young.PotentialDbhIncrementCm!.Value, "young uncrowded potential");
            BitEq(0.60, young.SiteAdjustedDbhIncrementCm!.Value, "dry G growth x1.0");

            ScotsPineGrowthResult old = ScotsPineEngine.EvaluateGrowth(new(60, 5000, 10, 20.0, SiteMoistureClass.FreshMoistMineral));
            ApproxEq(0.10, old.PotentialDbhIncrementCm!.Value, 1e-12, "old fully crowded potential = 0.20 x 0.5");

            ScotsPineGrowthResult bog = ScotsPineEngine.EvaluateGrowth(new(35, 1200, 30, 12.0, SiteMoistureClass.StagnantPoorBog));
            ApproxEq(bog.PotentialDbhIncrementCm!.Value * 0.35, bog.SiteAdjustedDbhIncrementCm!.Value, 1e-15, "bog L growth x0.35");
            True(bog.Site.Site.HabitatProtectionCaution, "bog caution travels with growth result");

            ScotsPineGrowthResult neutral = ScotsPineEngine.EvaluateGrowth(new(35, 1200, 30, 12.0, SiteMoistureClass.NeutralLegacy));
            BitEq(neutral.PotentialDbhIncrementCm!.Value, neutral.SiteAdjustedDbhIncrementCm!.Value, "neutral leaves growth unchanged");
            True(neutral.Evidence.Includes(EvidenceClass.Calibration), "tagged calibration");
        });

        TestRunner.Add(Suite, "GROWTH-02 growth is unavailable outside the domain", () =>
        {
            var outside = new ScotsPineGrowthRequest[]
            {
                new(9, 1000, 30, 10, SiteMoistureClass.DryFreeDraining),
                new(61, 1000, 30, 10, SiteMoistureClass.DryFreeDraining),
                new(30, 99, 30, 10, SiteMoistureClass.DryFreeDraining),
                new(30, 5001, 30, 10, SiteMoistureClass.DryFreeDraining),
                new(30, 1000, 9.9, 10, SiteMoistureClass.DryFreeDraining),
                new(30, 1000, 100.1, 10, SiteMoistureClass.DryFreeDraining),
                new(30, 1000, 30, 4.99, SiteMoistureClass.DryFreeDraining),
                new(30, 1000, 30, double.NaN, SiteMoistureClass.DryFreeDraining),
                new(30, double.NaN, 30, 10, SiteMoistureClass.DryFreeDraining),
            };
            foreach (ScotsPineGrowthRequest request in outside)
            {
                ScotsPineGrowthResult result = ScotsPineEngine.EvaluateGrowth(request);
                True(!result.IsWithinCalibrationDomain, "outside: " + request);
                True(result.SiteAdjustedDbhIncrementCm is null, "no adjusted value: " + request);
            }
        });

        TestRunner.Add(Suite, "REGEN-01a blocked without a mature on-map seed tree, whatever the year or light", () =>
        {
            for (int year = 0; year < 40; year++)
            {
                Eq(ScotsPineRegenerationStatus.NoMatureLocalSeedTree,
                    ScotsPineEngine.EvaluateRegeneration(OpenGap(year) with { OldestOnMapSeedTreeAgeYears = null }).Status, $"no tree year {year}");
                Eq(ScotsPineRegenerationStatus.NoMatureLocalSeedTree,
                    ScotsPineEngine.EvaluateRegeneration(OpenGap(year) with { OldestOnMapSeedTreeAgeYears = 17, CanopyOpenness = 1.0, UndergrowthCover = 0.0 }).Status,
                    $"17-year tree year {year}");
            }
            Eq(18, ScotsPineEngine.MinimumSeedTreeAgeYears, "seed tree threshold");
            ScotsPineRegenerationResult blocked = ScotsPineEngine.EvaluateRegeneration(OpenGap(FirstYear(true)) with { OldestOnMapSeedTreeAgeYears = null });
            BitEq(0.0, blocked.EstablishmentMultiplier, "blocked gap establishes nothing");
            Eq(ScotsPineRegenerationStatus.Eligible,
                ScotsPineEngine.EvaluateRegeneration(OpenGap(FirstYear(true)) with { OldestOnMapSeedTreeAgeYears = 18 }).Status, "18-year tree is enough");
        });

        TestRunner.Add(Suite, "REGEN-01b blocked in a poor seed year, a dark canopy or high undergrowth", () =>
        {
            int good = FirstYear(true);
            Eq(ScotsPineRegenerationStatus.PoorSeedYear, ScotsPineEngine.EvaluateRegeneration(OpenGap(FirstYear(false))).Status, "poor year");
            Eq(ScotsPineRegenerationStatus.CanopyTooDark, ScotsPineEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.59 }).Status, "dark canopy");
            Eq(ScotsPineRegenerationStatus.CanopyTooDark, ScotsPineEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.0 }).Status, "closed canopy");
            Eq(ScotsPineRegenerationStatus.TooMuchUndergrowth, ScotsPineEngine.EvaluateRegeneration(OpenGap(good) with { UndergrowthCover = 0.31 }).Status, "high undergrowth");
            Eq(ScotsPineRegenerationStatus.Eligible,
                ScotsPineEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 0.60, UndergrowthCover = 0.30 }).Status, "thresholds inclusive");
            Throws<ArgumentOutOfRangeException>(() => ScotsPineEngine.EvaluateRegeneration(OpenGap(good) with { CanopyOpenness = 1.1 }), "openness > 1");
            Throws<ArgumentOutOfRangeException>(() => ScotsPineEngine.EvaluateRegeneration(OpenGap(good) with { UndergrowthCover = double.NaN }), "NaN cover");
            Throws<ArgumentOutOfRangeException>(() => ScotsPineEngine.EvaluateRegeneration(OpenGap(good) with { OldestOnMapSeedTreeAgeYears = -1 }), "negative age");
            Throws<ArgumentOutOfRangeException>(() => ScotsPineEngine.IsGoodSeedYear(Seed, -1), "negative year");
        });

        TestRunner.Add(Suite, "REGEN-01c eligible gaps return the site establishment modifier (dry 1.0, wet alluvial 0.70, bog 0.35)", () =>
        {
            int good = FirstYear(true);
            BitEq(1.00, ScotsPineEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.DryFreeDraining)).EstablishmentMultiplier, "dry");
            BitEq(0.70, ScotsPineEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.WetMovingAlluvial)).EstablishmentMultiplier, "wet alluvial M");
            ScotsPineRegenerationResult bog = ScotsPineEngine.EvaluateRegeneration(OpenGap(good, SiteMoistureClass.StagnantPoorBog));
            BitEq(0.35, bog.EstablishmentMultiplier, "bog L");
            True(bog.Site.Site.HabitatProtectionCaution, "bog caution");
        });

        TestRunner.Add(Suite, "REGEN-02a good seed years recur every 3-5 years: one per four-year block", () =>
        {
            ulong[] seeds = { 0UL, 1UL, Seed, ulong.MaxValue, 0x5EED5EED5EED5EEDUL };
            foreach (ulong seed in seeds)
            {
                var goodYears = Enumerable.Range(0, 400).Where(y => ScotsPineEngine.IsGoodSeedYear(seed, y)).ToList();
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

        TestRunner.Add(Suite, "REGEN-02b an otherwise-open gap regenerates exactly in good seed years; schedules vary by woodland", () =>
        {
            int eligibleYears = 0;
            for (int year = 0; year < 40; year++)
            {
                ScotsPineRegenerationResult result = ScotsPineEngine.EvaluateRegeneration(OpenGap(year));
                Eq(ScotsPineEngine.IsGoodSeedYear(Seed, year), result.CanRegenerate, $"year {year}");
                Eq(result.GoodSeedYear, result.CanRegenerate, $"year {year} flag");
                if (result.CanRegenerate) eligibleYears++;
            }
            Eq(10, eligibleYears, "ten good years in forty");

            bool anyDifference = false;
            for (ulong seed = 1; seed <= 16 && !anyDifference; seed++)
                anyDifference = Enumerable.Range(0, 40).Any(y => ScotsPineEngine.IsGoodSeedYear(seed, y) != ScotsPineEngine.IsGoodSeedYear(Seed, y));
            True(anyDifference, "woodland seed changes the schedule");
        });

        TestRunner.Add(Suite, "REGEN-02c identical requests give identical results in any query order", () =>
        {
            var forward = Enumerable.Range(0, 60).Select(y => ScotsPineEngine.EvaluateRegeneration(OpenGap(y))).ToList();
            var backward = Enumerable.Range(0, 60).Reverse().Select(y => ScotsPineEngine.EvaluateRegeneration(OpenGap(y))).Reverse().ToList();
            for (int i = 0; i < forward.Count; i++)
                Eq(forward[i], backward[i], $"year {i}");
        });
    }
}
