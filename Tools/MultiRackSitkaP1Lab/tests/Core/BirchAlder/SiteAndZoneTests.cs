using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using static SitkaP1.Core.BirchAlder.Tests.TestRunner;

namespace SitkaP1.Core.BirchAlder.Tests;

internal static class SiteAndZoneTests
{
    private const string Suite = "SiteAndZone";

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
        TestRunner.Add(Suite, "SITE-01a NeutralLegacy is bit-exact (1.0, 1.0) for both species", () =>
        {
            Eq(2, BirchAlderEngine.SupportedSpecies.Count, "two species");
            foreach (string species in BirchAlderEngine.SupportedSpecies)
            {
                BirchAlderSite site = BirchAlderEngine.Site(species, SiteMoistureClass.NeutralLegacy);
                BitEq(1.0, site.Response.EstablishmentMultiplier, species + " neutral establishment");
                BitEq(1.0, site.Response.GrowthMultiplier, species + " neutral growth");
                True(!site.HabitatProtectionCaution, species + " neutral has no bog caution");
            }
        });

        TestRunner.Add(Suite, "SITE-01b engine returns the existing table cell unchanged for both species x six classes", () =>
        {
            int cells = 0;
            foreach (string species in BirchAlderEngine.SupportedSpecies)
            foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
            {
                BirchAlderSite site = BirchAlderEngine.Site(species, moisture);
                Eq(SpeciesMoistureTable.Cell(species, moisture), site.Cell, $"{species}/{moisture} cell");
                Eq(moisture, site.Moisture, $"{species}/{moisture} class");
                cells++;
            }
            Eq(12, cells, "2 species x 6 classes");
        });

        TestRunner.Add(Suite, "SITE-01c establishment and growth stay separate (birch bog M/L, alder bog L/X)", () =>
        {
            BirchAlderSite birch = BirchAlderEngine.Site(BirchAlderSpecies.DownyBirch, SiteMoistureClass.StagnantPoorBog);
            BitEq(0.70, birch.Response.EstablishmentMultiplier, "birch bog establishment M");
            BitEq(0.35, birch.Response.GrowthMultiplier, "birch bog growth L");

            BirchAlderSite alder = BirchAlderEngine.Site(BirchAlderSpecies.CommonAlder, SiteMoistureClass.StagnantPoorBog);
            BitEq(0.35, alder.Response.EstablishmentMultiplier, "alder bog establishment L");
            BitEq(0.10, alder.Response.GrowthMultiplier, "alder bog growth X");
        });

        TestRunner.Add(Suite, "SITE-01d bog carries the habitat caution for both species; other classes do not", () =>
        {
            foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
            {
                bool expected = moisture == SiteMoistureClass.StagnantPoorBog;
                foreach (string species in BirchAlderEngine.SupportedSpecies)
                    Eq(expected, BirchAlderEngine.Site(species, moisture).HabitatProtectionCaution, $"{species}/{moisture}");
            }
        });

        TestRunner.Add(Suite, "SITE-01e cards are reused, not copied, and their locked traits are unchanged", () =>
        {
            True(ReferenceEquals(FutureSpeciesCatalog.DownyBirch, BirchAlderEngine.BirchProfile), "birch card is the catalog instance");
            True(ReferenceEquals(FutureSpeciesCatalog.CommonAlder, BirchAlderEngine.AlderProfile), "alder card is the catalog instance");
            True(ReferenceEquals(BirchAlderEngine.BirchProfile, BirchAlderEngine.Profile(BirchAlderSpecies.DownyBirch)), "birch looks up by id");

            SpeciesProfileCard birch = BirchAlderEngine.BirchProfile;
            Eq(SpeciesMoistureTable.DownyBirch, birch.SpeciesId, "birch id");
            Eq(ShadeTolerance.Light, birch.Crown.ShadeTolerance, "birch shade casting");
            Eq(Level.High, birch.Silviculture.LightDemand, "birch light demand");
            Eq(Level.MediumHigh, birch.Silviculture.BrowseSensitivity, "birch browse");
            Eq(PruningRelevance.Moderate, birch.Silviculture.PruningRelevance, "birch pruning");
            True(!birch.Silviculture.SelfPruning, "birch does not self-prune");
            Eq(ExternalColonisation.AuthoredOnly, birch.Colonisation.ExternalColonisation, "birch no off-map arrival");
            True(!birch.Colonisation.WaterAssistedDispersalEligible, "birch is wind-dispersed only");
            Eq(new ValueBand(30, 35), birch.Timber.QualityStemInspectionTriggerDbhCm, "birch quality trigger");
            True(birch.Timber.PremiumBuyerRequired, "birch needs a premium buyer");

            SpeciesProfileCard alder = BirchAlderEngine.AlderProfile;
            Eq(SpeciesMoistureTable.CommonAlder, alder.SpeciesId, "alder id");
            Eq(ShadeTolerance.Moderate, alder.Crown.ShadeTolerance, "alder shade casting");
            Eq(Level.High, alder.Silviculture.LightDemand, "alder light demand");
            Eq(Level.LowMedium, alder.Silviculture.BrowseSensitivity, "alder browse");
            True(alder.Silviculture.SelfPruning, "alder self-prunes");
            True(alder.Silviculture.ThinningMoreImportantThanPruning, "alder thinning matters more than pruning");
            True(alder.Colonisation.WaterAssistedDispersalEligible, "alder water-assisted local dispersal");
            Eq(new ValueBand(50, 60), alder.Timber.HeartRotInspectionRiskOnsetYears, "alder heart-rot onset band");
            True(alder.Timber.PremiumBuyerRequired, "alder needs a premium buyer");
        });

        TestRunner.Add(Suite, "SITE-01f an unsupported species id is refused everywhere", () =>
        {
            foreach (string other in new[] { SpeciesMoistureTable.SitkaSpruce, SpeciesMoistureTable.ScotsPine, "_oak", "" })
            {
                Throws<ArgumentException>(() => BirchAlderEngine.Site(other, SiteMoistureClass.FreshMoistMineral), "Site " + other);
                Throws<ArgumentException>(() => BirchAlderEngine.Profile(other), "Profile " + other);
                Throws<ArgumentException>(() => BirchAlderEngine.GrowthDomain(other), "GrowthDomain " + other);
                Throws<ArgumentException>(() => BirchAlderEngine.SaplingMaterialCostEurPerTree(other), "Sapling " + other);
            }
            Throws<ArgumentException>(() => BirchAlderEngine.Site(null!, SiteMoistureClass.FreshMoistMineral), "null species");
        });

        TestRunner.Add(Suite, "ZONE-01a the standard preset is 80 x 80 m, five 16 x 80 m lanes, 0.64 ha", () =>
        {
            WetValleyAndSlopeMap map = WetValleyAndSlopeMap.CreateStandard();
            ApproxEq(80.0, map.Bounds.MaxX - map.Bounds.MinX, 1e-12, "width");
            ApproxEq(80.0, map.Bounds.MaxY - map.Bounds.MinY, 1e-12, "length");
            Eq(5, map.LaneCount, "lane count");
            BitEq(16.0, map.LaneWidthM, "lane width");
            BitEq(40.0, map.MidpointY, "y midpoint");
            ApproxEq(0.64, WetValleyAndSlopeMap.StandardAreaHectares, 1e-12, "0.64 ha");

            for (int lane = 0; lane < 5; lane++)
            {
                WetValleyBounds bounds = map.LaneBounds(lane);
                ApproxEq(16.0, bounds.MaxX - bounds.MinX, 1e-12, $"lane {lane} width");
                ApproxEq(80.0, bounds.MaxY - bounds.MinY, 1e-12, $"lane {lane} length");
                Eq(lane == 0 ? 0.0 : lane * 16.0, bounds.MinX, $"lane {lane} left edge");
            }
            Eq(80.0, map.LaneBounds(4).MaxX, "last lane reaches the right edge");
        });

        TestRunner.Add(Suite, "ZONE-01b y midpoint splits valley from slope; midpoint is slope; bounds inclusive", () =>
        {
            WetValleyAndSlopeMap map = WetValleyAndSlopeMap.CreateStandard();
            Eq(SiteMoistureClass.WetMovingAlluvial, map.MoistureAt(0, 0), "min corner is valley");
            Eq(SiteMoistureClass.WetMovingAlluvial, map.MoistureAt(80, 0), "wet along the whole bottom edge");
            Eq(SiteMoistureClass.WetMovingAlluvial, map.MoistureAt(37, Math.BitDecrement(40.0)), "just below midpoint is valley");
            Eq(SiteMoistureClass.FreshMoistMineral, map.MoistureAt(37, 40), "midpoint is slope");
            Eq(SiteMoistureClass.FreshMoistMineral, map.MoistureAt(80, 80), "max corner is slope");
            Throws<ArgumentOutOfRangeException>(() => map.MoistureAt(-0.0001, 10), "x below stand");
            Throws<ArgumentOutOfRangeException>(() => map.MoistureAt(80.0001, 10), "x above stand");
            Throws<ArgumentOutOfRangeException>(() => map.MoistureAt(10, -0.0001), "y below stand");
            Throws<ArgumentOutOfRangeException>(() => map.MoistureAt(10, 80.0001), "y above stand");
            Throws<ArgumentOutOfRangeException>(() => map.MoistureAt(double.NaN, 10), "NaN x");
            Throws<ArgumentOutOfRangeException>(() => map.MoistureAt(10, double.PositiveInfinity), "infinite y");
        });

        TestRunner.Add(Suite, "ZONE-01c the two zones cover the whole stand exactly, with no bog anywhere", () =>
        {
            WetValleyAndSlopeMap map = WetValleyAndSlopeMap.CreateStandard();
            int valley = 0, slope = 0;
            const int steps = 80; // 81 x 81 inclusive grid, one sample per metre plus both edges.
            for (int i = 0; i <= steps; i++)
            for (int j = 0; j <= steps; j++)
            {
                double x = map.Bounds.MinX + i;
                double y = map.Bounds.MinY + j;
                SiteMoistureClass moisture = map.MoistureAt(x, y);
                if (moisture == SiteMoistureClass.WetMovingAlluvial) valley++;
                else if (moisture == SiteMoistureClass.FreshMoistMineral) slope++;
                else throw new InvalidOperationException("unexpected class " + moisture);
                True(!map.For(BirchAlderSpecies.CommonAlder, x, y).HabitatProtectionCaution, "no bog in the helper");
            }
            Eq(81 * 81, valley + slope, "full coverage");
            Eq(81 * 40, valley, "rows 0..39 are valley");
            Eq(81 * 41, slope, "rows 40..80 are slope (midpoint row included)");
        });

        TestRunner.Add(Suite, "ZONE-01d lane indexing and lane bounds stay consistent at every x", () =>
        {
            WetValleyAndSlopeMap map = WetValleyAndSlopeMap.CreateStandard();
            for (int lane = 0; lane < map.LaneCount; lane++)
            {
                WetValleyBounds bounds = map.LaneBounds(lane);
                Eq(lane, map.LaneIndexOf(bounds.MinX), $"lane {lane} left edge");
                Eq(lane, map.LaneIndexOf(bounds.MinX + 7.999), $"lane {lane} interior");
            }
            Eq(0, map.LaneIndexOf(15.999), "just inside lane 0");
            Eq(1, map.LaneIndexOf(16.0), "lane boundary belongs to the next lane");
            Eq(4, map.LaneIndexOf(80.0), "right edge is the last lane");
            Throws<ArgumentOutOfRangeException>(() => map.LaneIndexOf(80.5), "outside x");
            Throws<ArgumentOutOfRangeException>(() => map.LaneBounds(-1), "negative lane");
            Throws<ArgumentOutOfRangeException>(() => map.LaneBounds(5), "lane beyond the stand");
        });

        TestRunner.Add(Suite, "ZONE-02a Common alder leads growth on the wet valley; Downy birch competes on the fresh slope", () =>
        {
            WetValleyAndSlopeMap map = WetValleyAndSlopeMap.CreateStandard();
            BirchAlderSite alderValley = map.For(BirchAlderSpecies.CommonAlder, 40, 10);
            BirchAlderSite birchValley = map.For(BirchAlderSpecies.DownyBirch, 40, 10);
            BitEq(1.0, alderValley.Response.GrowthMultiplier, "alder valley growth G/G");
            BitEq(0.70, birchValley.Response.GrowthMultiplier, "birch valley growth M on the wet half");

            foreach (string other in SpeciesMoistureTable.SpeciesIds.Where(id => id != SpeciesMoistureTable.CommonAlder))
            {
                SiteResponse response = SpeciesMoistureTable.Response(other, SiteMoistureClass.WetMovingAlluvial);
                True(alderValley.Response.GrowthMultiplier > response.GrowthMultiplier,
                    "alder valley growth leads " + other);
            }

            BirchAlderSite birchSlope = map.For(BirchAlderSpecies.DownyBirch, 40, 70);
            BirchAlderSite alderSlope = map.For(BirchAlderSpecies.CommonAlder, 40, 70);
            BitEq(1.0, birchSlope.Response.GrowthMultiplier, "birch slope growth G");
            BitEq(1.0, alderSlope.Response.GrowthMultiplier, "alder slope growth G too");
            Eq(SiteMoistureClass.FreshMoistMineral, birchSlope.Moisture, "slope class");
        });

        TestRunner.Add(Suite, "ZONE-02b alder slows sharply on stagnant bog and reports the caution", () =>
        {
            BirchAlderSite bog = BirchAlderEngine.Site(BirchAlderSpecies.CommonAlder, SiteMoistureClass.StagnantPoorBog);
            BitEq(0.10, bog.Response.GrowthMultiplier, "alder bog growth X");
            True(bog.HabitatProtectionCaution, "bog caution");
            foreach (string other in SpeciesMoistureTable.SpeciesIds)
            {
                SiteResponse response = SpeciesMoistureTable.Response(other, SiteMoistureClass.StagnantPoorBog);
                True(bog.Response.GrowthMultiplier <= response.GrowthMultiplier, "alder bog growth is the floor against " + other);
            }
            True(bog.Response.GrowthMultiplier <= 0.10, "bog growth is at the X tier, far below the valley");
            Eq(4, ActiveClasses.Count(c => !BirchAlderEngine.Site(BirchAlderSpecies.CommonAlder, c).HabitatProtectionCaution),
                "four non-bog classes have no caution");
        });
    }
}
