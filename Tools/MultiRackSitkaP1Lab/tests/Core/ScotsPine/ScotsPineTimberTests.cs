using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;
using static SitkaP1.Core.ScotsPine.Tests.TestRunner;

namespace SitkaP1.Core.ScotsPine.Tests;

internal static class ScotsPineTimberTests
{
    private const string Suite = "ScotsPineTimber";

    // Sitka spruce values from src/Core/MultiArea/Economy (not compiled into this project, so copied here as literals).
    // The pine module must not return these: D062TimberPriceBook sitka-sawlog selected 9500 c/t, 890 kg/m3 fresh proxy.
    private const decimal SitkaSawlogSelectedEurPerTonne = 95m;
    private const decimal SitkaFreshVolumeProxyTPerM3 = 0.890m;

    private static ScotsPineTimberRequest Stem(double prunedM = 0.0, bool tight = false, bool mature = true, bool smallRoundwood = true) =>
        new(VolumeM3: 2.0m, IsMature: mature, PrunedHeightM: prunedM, HadTightEarlyStocking: tight, SuitableForSmallRoundwood: smallRoundwood);

    public static void Register()
    {
        TestRunner.Add(Suite, "TIMBER-01a Sawlog via pruning to the 5-6 m target at EUR 68/t and 1.02 t/m3", () =>
        {
            foreach (double pruned in new[] { 5.0, 5.5, 6.0, 7.0 })
            {
                ScotsPineTimberResult r = ScotsPineEngine.GradeTimber(Stem(prunedM: pruned));
                Eq(Sortment.Sawlog, r.Sortment, $"pruned {pruned}");
                Eq(68m, r.UnitPriceEurPerTonne, "sawlog price");
                Eq(new ValueBand(50, 85), r.PriceBandEurPerTonne, "sawlog band");
                Eq(2.0m, r.VolumeM3, "volume");
                Eq(2.04m, r.GreenTonnes, "2 m3 x 1.02");
                Eq(138.72m, r.GrossValueEur, "2.04 t x 68");
                True(r.Reason.Contains("pruning", StringComparison.Ordinal), "reason names pruning");
            }
        });

        TestRunner.Add(Suite, "TIMBER-01b Sawlog via tight early stocking without pruning", () =>
        {
            ScotsPineTimberResult r = ScotsPineEngine.GradeTimber(Stem(prunedM: 0.0, tight: true));
            Eq(Sortment.Sawlog, r.Sortment, "tight stocking");
            Eq(138.72m, r.GrossValueEur, "value");
            True(r.Reason.Contains("tight early stocking", StringComparison.Ordinal), "reason names stocking");
        });

        TestRunner.Add(Suite, "TIMBER-01c unpruned open-grown mature stems downgrade to Small Roundwood (EUR 45/t) or Pulp/Chip (EUR 38/t)", () =>
        {
            ScotsPineTimberResult small = ScotsPineEngine.GradeTimber(Stem(prunedM: 4.99));
            Eq(Sortment.SmallRoundwood, small.Sortment, "small roundwood");
            Eq(45m, small.UnitPriceEurPerTonne, "SRW price");
            Eq(new ValueBand(30, 60), small.PriceBandEurPerTonne, "SRW band");
            Eq(91.80m, small.GrossValueEur, "2.04 t x 45");
            True(small.Reason.Contains("missed the pruning target", StringComparison.Ordinal), "downgrade reason");

            ScotsPineTimberResult pulp = ScotsPineEngine.GradeTimber(Stem(smallRoundwood: false));
            Eq(Sortment.PulpChip, pulp.Sortment, "pulp/chip");
            Eq(38m, pulp.UnitPriceEurPerTonne, "pulp price");
            Eq(new ValueBand(25, 50), pulp.PriceBandEurPerTonne, "pulp band");
            Eq(77.52m, pulp.GrossValueEur, "2.04 t x 38");

            ScotsPineTimberResult immature = ScotsPineEngine.GradeTimber(Stem(prunedM: 6.0, tight: true, mature: false));
            Eq(Sortment.SmallRoundwood, immature.Sortment, "immature stem never sawlog");
            True(immature.Reason.Contains("maturity", StringComparison.Ordinal), "immature reason");

            True(small.Reason != pulp.Reason && small.Reason != immature.Reason, "reasons are distinct");
        });

        TestRunner.Add(Suite, "TIMBER-01d sapling material cost is EUR 0.45 per tree; invalid stems refused", () =>
        {
            Eq(0.45m, ScotsPineEngine.SaplingMaterialCostEurPerTree, "sapling cost");
            Eq(450.00m, 1000 * ScotsPineEngine.SaplingMaterialCostEurPerTree, "1,000 saplings");
            Eq(0m, ScotsPineEngine.GradeTimber(Stem() with { VolumeM3 = 0m }).GrossValueEur, "zero volume, zero value");
            Throws<ArgumentOutOfRangeException>(() => ScotsPineEngine.GradeTimber(Stem() with { VolumeM3 = -0.1m }), "negative volume");
            Throws<ArgumentOutOfRangeException>(() => ScotsPineEngine.GradeTimber(Stem(prunedM: -1)), "negative pruning");
            Throws<ArgumentOutOfRangeException>(() => ScotsPineEngine.GradeTimber(Stem(prunedM: double.PositiveInfinity)), "infinite pruning");
        });

        TestRunner.Add(Suite, "TIMBER-02 prices and density are Scots pine-specific (card-backed, not Sitka), tagged Calibration", () =>
        {
            Eq(1.02m, ScotsPineEngine.GreenDensityTPerM3, "pine green density");
            Eq((decimal)FutureSpeciesCatalog.ScotsPine.Timber.GreenDensityTPerM3, ScotsPineEngine.GreenDensityTPerM3, "density read from pine card");
            True(ScotsPineEngine.GreenDensityTPerM3 != SitkaFreshVolumeProxyTPerM3, "not the Sitka 890 kg/m3 proxy");
            True(ScotsPineEngine.SawlogPriceEurPerTonne != SitkaSawlogSelectedEurPerTonne, "not the Sitka sawlog price");

            Eq(68m, ScotsPineEngine.SawlogPriceEurPerTonne, "sawlog");
            Eq(45m, ScotsPineEngine.SmallRoundwoodPriceEurPerTonne, "small roundwood");
            Eq(38m, ScotsPineEngine.PulpChipPriceEurPerTonne, "pulp/chip");
            foreach (SortmentBand band in FutureSpeciesCatalog.ScotsPine.Timber.SortmentBands)
            {
                decimal price = band.Sortment switch
                {
                    Sortment.Sawlog => ScotsPineEngine.SawlogPriceEurPerTonne,
                    Sortment.SmallRoundwood => ScotsPineEngine.SmallRoundwoodPriceEurPerTonne,
                    Sortment.PulpChip => ScotsPineEngine.PulpChipPriceEurPerTonne,
                    _ => throw new InvalidOperationException("unexpected pine sortment " + band.Sortment),
                };
                True(band.EurPerTonne.Contains((double)price), $"{band.Sortment} price inside its pine band");
            }
            True(ScotsPineEngine.GradeTimber(Stem(prunedM: 5)).Evidence.Includes(EvidenceClass.Calibration), "calibration tag");
        });
    }
}
