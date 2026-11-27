using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;
using static SitkaP1.Core.ScotsPine.Tests.TestRunner;

namespace SitkaP1.Core.ScotsPine.Tests;

internal static class SiteMoistureTests
{
    private const string Suite = "SiteMoisture";

    private static readonly SiteMoistureClass[] ActiveClasses =
    {
        SiteMoistureClass.DryFreeDraining,
        SiteMoistureClass.FreshMoistMineral,
        SiteMoistureClass.SeasonallyWetAerated,
        SiteMoistureClass.WetMovingAlluvial,
        SiteMoistureClass.StagnantPoorBog,
    };

    public static void Register()
    {
        TestRunner.Add(Suite, "MOISTURE-01a NeutralLegacy is bit-exact (1.0, 1.0) for all six species", () =>
        {
            Eq(6, SpeciesMoistureTable.SpeciesIds.Count, "six species");
            foreach (string species in SpeciesMoistureTable.SpeciesIds)
            {
                SiteResponse response = SiteMoistureEngine.ForSpecies(species, SiteMoistureClass.NeutralLegacy).Cell.Response;
                BitEq(1.0, response.EstablishmentMultiplier, species + " neutral establishment");
                BitEq(1.0, response.GrowthMultiplier, species + " neutral growth");
            }
        });

        TestRunner.Add(Suite, "MOISTURE-01b engine returns the existing table cell unchanged for every species x class", () =>
        {
            int cells = 0;
            foreach (string species in SpeciesMoistureTable.SpeciesIds)
            foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
            {
                SiteMoistureResult result = SiteMoistureEngine.ForSpecies(species, moisture);
                Eq(SpeciesMoistureTable.Cell(species, moisture), result.Cell, $"{species}/{moisture} cell");
                Eq(moisture, result.Site.Moisture, $"{species}/{moisture} selection");
                cells++;
            }
            Eq(36, cells, "6 species x 6 classes");
        });

        TestRunner.Add(Suite, "MOISTURE-01c establishment and growth modifiers stay separate (Scots pine WetMovingAlluvial M/L)", () =>
        {
            SiteResponse response = SiteMoistureEngine.ForSpecies(SpeciesMoistureTable.ScotsPine, SiteMoistureClass.WetMovingAlluvial).Cell.Response;
            BitEq(0.70, response.EstablishmentMultiplier, "establishment M");
            BitEq(0.35, response.GrowthMultiplier, "growth L");
        });

        TestRunner.Add(Suite, "MOISTURE-01d unknown species is refused", () =>
            Throws<ArgumentException>(() => SiteMoistureEngine.ForSpecies("larch", SiteMoistureClass.DryFreeDraining), "unknown id"));

        TestRunner.Add(Suite, "MOISTURE-02a Scots pine is the sole leader on DryFreeDraining (both modifiers)", () =>
        {
            SiteResponse pine = SiteMoistureEngine.ForSpecies(SpeciesMoistureTable.ScotsPine, SiteMoistureClass.DryFreeDraining).Cell.Response;
            BitEq(1.0, pine.EstablishmentMultiplier, "pine dry establishment");
            BitEq(1.0, pine.GrowthMultiplier, "pine dry growth");
            foreach (string species in SpeciesMoistureTable.SpeciesIds.Where(id => id != SpeciesMoistureTable.ScotsPine))
            {
                SiteResponse other = SiteMoistureEngine.ForSpecies(species, SiteMoistureClass.DryFreeDraining).Cell.Response;
                True(other.EstablishmentMultiplier < pine.EstablishmentMultiplier, species + " dry establishment below pine");
                True(other.GrowthMultiplier < pine.GrowthMultiplier, species + " dry growth below pine");
            }
        });

        TestRunner.Add(Suite, "MOISTURE-02b Sitka is a joint leader on FreshMoistMineral (all six G/G)", () =>
        {
            SiteResponse sitka = SiteMoistureEngine.ForSpecies(SpeciesMoistureTable.SitkaSpruce, SiteMoistureClass.FreshMoistMineral).Cell.Response;
            foreach (string species in SpeciesMoistureTable.SpeciesIds)
            {
                MoistureCell cell = SiteMoistureEngine.ForSpecies(species, SiteMoistureClass.FreshMoistMineral).Cell;
                Eq(SuitabilityTier.Good, cell.EstablishmentTier, species + " fresh establishment tier");
                Eq(SuitabilityTier.Good, cell.GrowthTier, species + " fresh growth tier");
                True(sitka.EstablishmentMultiplier >= cell.Response.EstablishmentMultiplier, "Sitka not beaten on establishment by " + species);
                True(sitka.GrowthMultiplier >= cell.Response.GrowthMultiplier, "Sitka not beaten on growth by " + species);
            }

            // Contrast across the two zones: pine leads Sitka on the dry half; on the moist half Sitka is at the maximum.
            SiteResponse sitkaDry = SiteMoistureEngine.ForSpecies(SpeciesMoistureTable.SitkaSpruce, SiteMoistureClass.DryFreeDraining).Cell.Response;
            True(sitkaDry.GrowthMultiplier < sitka.GrowthMultiplier, "Sitka grows better on fresh moist than on dry");
            BitEq(1.0, sitka.GrowthMultiplier, "Sitka fresh growth G");
        });

        TestRunner.Add(Suite, "MOISTURE-03a weighted selection sums areas per class and picks the largest", () =>
        {
            var areas = new[]
            {
                new MoistureArea(SiteMoistureClass.DryFreeDraining, 400m),
                new MoistureArea(SiteMoistureClass.FreshMoistMineral, 300m),
                new MoistureArea(SiteMoistureClass.FreshMoistMineral, 200m),
                new MoistureArea(SiteMoistureClass.StagnantPoorBog, 450m),
            };
            Eq(SiteMoistureClass.FreshMoistMineral, SiteMoistureEngine.SelectDominant(areas).Moisture, "500 fresh beats 450 bog and 400 dry");
            Eq(SiteMoistureClass.FreshMoistMineral, SiteMoistureEngine.SelectDominant(areas.Reverse()).Moisture, "input order does not matter");
        });

        TestRunner.Add(Suite, "MOISTURE-03b ties use declaration order; empty or all-zero input is NeutralLegacy; bad input refused", () =>
        {
            Eq(SiteMoistureClass.DryFreeDraining, SiteMoistureEngine.SelectDominant(new[]
            {
                new MoistureArea(SiteMoistureClass.FreshMoistMineral, 50m),
                new MoistureArea(SiteMoistureClass.DryFreeDraining, 50m),
            }).Moisture, "equal totals -> earlier declared class");
            Eq(SiteMoistureClass.NeutralLegacy, SiteMoistureEngine.SelectDominant(Array.Empty<MoistureArea>()).Moisture, "empty");
            Eq(SiteMoistureClass.NeutralLegacy, SiteMoistureEngine.SelectDominant(new[]
            {
                new MoistureArea(SiteMoistureClass.WetMovingAlluvial, 0m),
            }).Moisture, "all-zero");
            Throws<ArgumentOutOfRangeException>(() => SiteMoistureEngine.SelectDominant(new[]
            {
                new MoistureArea(SiteMoistureClass.DryFreeDraining, -1m),
            }), "negative area");
            Throws<ArgumentOutOfRangeException>(() => SiteMoistureEngine.SelectDominant(new[]
            {
                new MoistureArea((SiteMoistureClass)99, 1m),
            }), "undefined class");
            Throws<ArgumentNullException>(() => SiteMoistureEngine.SelectDominant(null!), "null");
        });

        TestRunner.Add(Suite, "MOISTURE-03c Dry Ridge & Moist Slope splits on the x midpoint; midpoint belongs to FreshMoistMineral", () =>
        {
            DryRidgeAndMoistSlopeMap map = SiteMoistureEngine.CreateDryRidgeAndMoistSlope(new StandBounds(0, 0, 100, 60));
            BitEq(50.0, map.MidpointX, "midpoint");
            Eq(SiteMoistureClass.DryFreeDraining, map.At(0, 0).Moisture, "min corner");
            Eq(SiteMoistureClass.DryFreeDraining, map.At(Math.BitDecrement(50.0), 60).Moisture, "just below midpoint");
            Eq(SiteMoistureClass.FreshMoistMineral, map.At(50, 30).Moisture, "midpoint tie");
            Eq(SiteMoistureClass.FreshMoistMineral, map.At(100, 60).Moisture, "max corner");
            Throws<ArgumentOutOfRangeException>(() => map.At(100.0001, 10), "outside x");
            Throws<ArgumentOutOfRangeException>(() => map.At(10, -0.0001), "outside y");
            Throws<ArgumentOutOfRangeException>(() => map.At(double.NaN, 10), "NaN x");
            Throws<ArgumentOutOfRangeException>(() => SiteMoistureEngine.CreateDryRidgeAndMoistSlope(new StandBounds(5, 0, 5, 10)), "zero width");
        });

        TestRunner.Add(Suite, "MOISTURE-03d two-zone map covers every sampled point of the stand with exactly two halves, deterministically", () =>
        {
            var bounds = new StandBounds(-20, 10, 80, 70);
            DryRidgeAndMoistSlopeMap first = SiteMoistureEngine.CreateDryRidgeAndMoistSlope(bounds);
            DryRidgeAndMoistSlopeMap second = SiteMoistureEngine.CreateDryRidgeAndMoistSlope(bounds);
            int dry = 0, moist = 0;
            const int steps = 100; // 101 x 101 inclusive grid, cell centres on both edges and on the midpoint column.
            for (int i = 0; i <= steps; i++)
            for (int j = 0; j <= steps; j++)
            {
                double x = bounds.MinX + (bounds.MaxX - bounds.MinX) * i / steps;
                double y = bounds.MinY + (bounds.MaxY - bounds.MinY) * j / steps;
                SiteMoistureClass a = first.At(x, y).Moisture;
                Eq(a, second.At(x, y).Moisture, "same bounds, same answer");
                if (a == SiteMoistureClass.DryFreeDraining) dry++;
                else if (a == SiteMoistureClass.FreshMoistMineral) moist++;
                else throw new InvalidOperationException("unexpected class " + a);
                True(!first.At(x, y).HabitatProtectionCaution, "no bog in the two-zone map");
            }
            Eq(101 * 101, dry + moist, "full coverage");
            Eq(50 * 101, dry, "dry columns 0..49");
            Eq(51 * 101, moist, "moist columns 50..100 (midpoint column is moist)");
        });

        TestRunner.Add(Suite, "MOISTURE-04 HabitatProtectionCaution is raised for StagnantPoorBog only", () =>
        {
            foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
            {
                bool expected = moisture == SiteMoistureClass.StagnantPoorBog;
                Eq(expected, new MoistureSelection(moisture).HabitatProtectionCaution, moisture + " selection");
                foreach (string species in SpeciesMoistureTable.SpeciesIds)
                    Eq(expected, SiteMoistureEngine.ForSpecies(species, moisture).Site.HabitatProtectionCaution, $"{species}/{moisture}");
            }
            Eq(true, SiteMoistureEngine.SelectDominant(new[]
            {
                new MoistureArea(SiteMoistureClass.StagnantPoorBog, 10m),
                new MoistureArea(SiteMoistureClass.DryFreeDraining, 9m),
            }).HabitatProtectionCaution, "bog-dominant woodland carries caution");
            Eq(4, ActiveClasses.Count(c => !new MoistureSelection(c).HabitatProtectionCaution), "four non-bog active classes have no caution");
        });
    }
}
