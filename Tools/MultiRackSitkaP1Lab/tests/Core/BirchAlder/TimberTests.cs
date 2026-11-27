using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using static SitkaP1.Core.BirchAlder.Tests.TestRunner;

namespace SitkaP1.Core.BirchAlder.Tests;

internal static class TimberTests
{
    private const string Suite = "Timber";

    // Sitka and Scots pine values copied as literals from the economy and the parked pine lane: this project cannot
    // compile those sources. Neither broadleaf may return them.
    private const decimal SitkaSawlogSelectedEurPerTonne = 95m;
    private const decimal PineSawlogPriceEurPerTonne = 68m;
    private const decimal SitkaFreshVolumeProxyTPerM3 = 0.890m;

    private static BirchAlderTimberRequest Stem(
        string species = BirchAlderSpecies.DownyBirch,
        decimal volume = 2.0m,
        bool mature = true,
        double dbh = 20,
        bool form = false,
        int age = 20,
        ulong seed = 1UL) =>
        new(species, seed, volume, mature, dbh, form, age);

    /// <summary>The first stem identity that a heart-rot inspection downgrades at the given age.</summary>
    private static ulong FirstRotStem(int age)
    {
        for (ulong stem = 0; stem < 10000; stem++)
            if (BirchAlderEngine.InspectAlderHeartRot(stem, age))
                return stem;
        throw new InvalidOperationException("no downgraded stem in 0..9999 at age " + age);
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "TIMBER-01a Downy birch bulk: EUR 42/t, 0.93 t/m3, band 25-60, value 78.12", () =>
        {
            BirchAlderTimberResult r = BirchAlderEngine.GradeTimber(Stem());
            Eq(Sortment.Bulk, r.Sortment, "bulk sortment");
            Eq(42m, r.UnitPriceEurPerTonne, "bulk price");
            Eq(new ValueBand(25, 60), r.PriceBandEurPerTonne, "bulk band");
            Eq(2.0m, r.VolumeM3, "volume");
            Eq(1.86m, r.GreenTonnes, "2 m3 x 0.93");
            Eq(78.12m, r.GrossValueEur, "1.86 t x 42");
            True(!r.QualityStemUnpriced && !r.HeartRotDowngrade, "plain bulk stem carries no flags");
            Eq(null, r.BuyerNote, "no buyer note");
            True(r.Evidence.Includes(EvidenceClass.Calibration), "calibration tag");
        });

        TestRunner.Add(Suite, "TIMBER-01b Common alder bulk: EUR 42/t, 0.842 t/m3, value 70.728", () =>
        {
            BirchAlderTimberResult r = BirchAlderEngine.GradeTimber(Stem(species: BirchAlderSpecies.CommonAlder, age: 30));
            Eq(Sortment.Bulk, r.Sortment, "bulk sortment");
            Eq(42m, r.UnitPriceEurPerTonne, "bulk price");
            Eq(new ValueBand(25, 60), r.PriceBandEurPerTonne, "bulk band");
            Eq(1.684m, r.GreenTonnes, "2 m3 x 0.842");
            Eq(70.728m, r.GrossValueEur, "1.684 t x 42");
            True(!r.QualityStemUnpriced, "plain bulk stem is not a quality stem");
        });

        TestRunner.Add(Suite, "TIMBER-02 prices and densities are species-specific and never Sitka or pine", () =>
        {
            Eq(42m, BirchAlderEngine.BulkPriceEurPerTonne, "bulk price");
            True(BirchAlderEngine.BulkPriceEurPerTonne != SitkaSawlogSelectedEurPerTonne, "not the Sitka sawlog price");
            True(BirchAlderEngine.BulkPriceEurPerTonne != PineSawlogPriceEurPerTonne, "not the Scots pine sawlog price");
            True(FutureSpeciesCatalog.DownyBirch.Timber.SortmentBands.Single().EurPerTonne.Contains(42.0), "birch price inside its card band");
            True(FutureSpeciesCatalog.CommonAlder.Timber.SortmentBands.Single().EurPerTonne.Contains(42.0), "alder price inside its card band");

            Eq(0.93m, BirchAlderEngine.GreenDensityTPerM3(BirchAlderSpecies.DownyBirch), "birch card density");
            Eq(0.842m, BirchAlderEngine.GreenDensityTPerM3(BirchAlderSpecies.CommonAlder), "alder card density");
            True(BirchAlderEngine.GreenDensityTPerM3(BirchAlderSpecies.DownyBirch) !=
                 BirchAlderEngine.GreenDensityTPerM3(BirchAlderSpecies.CommonAlder), "the two densities differ");
            True(BirchAlderEngine.GreenDensityTPerM3(BirchAlderSpecies.DownyBirch) != SitkaFreshVolumeProxyTPerM3, "birch is not the Sitka proxy");

            Eq(0.95m, BirchAlderEngine.SaplingMaterialCostEurPerTree(BirchAlderSpecies.DownyBirch), "birch sapling");
            Eq(0.95m, BirchAlderEngine.SaplingMaterialCostEurPerTree(BirchAlderSpecies.CommonAlder), "alder sapling");
            Eq(950.00m, BirchAlderEngine.SaplingMaterialCostEur(BirchAlderSpecies.CommonAlder, 1000), "1,000 alder saplings");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.SaplingMaterialCostEur(BirchAlderSpecies.DownyBirch, -1), "negative count");
        });

        TestRunner.Add(Suite, "TIMBER-03a a Downy birch quality stem is flagged, unpriced and sells at bulk only", () =>
        {
            BirchAlderTimberResult quality = BirchAlderEngine.GradeTimber(Stem(dbh: 30, form: true));
            Eq(Sortment.Bulk, quality.Sortment, "bulk sortment");
            True(quality.QualityStemUnpriced, "quality stem is unpriced");
            Eq(BirchAlderEngine.QualityStemBuyerNote, quality.BuyerNote, "buyer note");
            Eq("Quality stem — no local specialist buyer", quality.BuyerNote, "exact note text");
            Eq(42m, quality.UnitPriceEurPerTonne, "bulk price only");
            Eq(78.12m, quality.GrossValueEur, "bulk value only");
            True(quality.Reason.Contains("quality stem", StringComparison.OrdinalIgnoreCase), "reason names the quality stem");

            True(!BirchAlderEngine.GradeTimber(Stem(dbh: 29.9, form: true)).QualityStemUnpriced, "just under 30 cm is not a quality stem");
            True(!BirchAlderEngine.GradeTimber(Stem(dbh: 40, form: false)).QualityStemUnpriced, "straight and sound is required");
            True(!BirchAlderEngine.GradeTimber(Stem(dbh: 40, form: true, mature: false)).QualityStemUnpriced, "an immature stem is never a quality stem");
            True(BirchAlderEngine.GradeTimber(Stem(dbh: 45, form: true)).QualityStemUnpriced, "well above the trigger still qualifies");
        });

        TestRunner.Add(Suite, "TIMBER-03b a well-released Common alder clean bole flags as a quality stem toward 50 cm", () =>
        {
            BirchAlderTimberResult quality = BirchAlderEngine.GradeTimber(
                Stem(species: BirchAlderSpecies.CommonAlder, dbh: 50, form: true, age: 40));
            True(quality.QualityStemUnpriced, "alder quality stem");
            Eq(BirchAlderEngine.QualityStemBuyerNote, quality.BuyerNote, "alder buyer note");
            Eq(70.728m, quality.GrossValueEur, "alder bulk value only");

            True(!BirchAlderEngine.GradeTimber(Stem(species: BirchAlderSpecies.CommonAlder, dbh: 49.9, form: true, age: 40)).QualityStemUnpriced,
                "just under 50 cm is not a quality stem");
            True(!BirchAlderEngine.GradeTimber(Stem(species: BirchAlderSpecies.CommonAlder, dbh: 60, form: false, age: 40)).QualityStemUnpriced,
                "a poorly released bole is not a quality stem");
        });

        TestRunner.Add(Suite, "ROT-03 a heart-rot-downgraded alder stem sells as firewood/pulp with a plain reason", () =>
        {
            ulong stem = FirstRotStem(55);
            BirchAlderTimberResult r = BirchAlderEngine.GradeTimber(
                Stem(species: BirchAlderSpecies.CommonAlder, dbh: 55, form: true, age: 55, seed: stem));
            True(r.HeartRotDowngrade, "downgrade flag");
            Eq(Sortment.PulpChip, r.Sortment, "firewood/pulp sortment");
            True(!r.QualityStemUnpriced, "a rotten stem is never a quality stem");
            Eq(null, r.BuyerNote, "no buyer note on a downgrade");
            Eq(42m, r.UnitPriceEurPerTonne, "still the single bulk price");
            True(r.Reason.Contains("heart-rot", StringComparison.OrdinalIgnoreCase), "reason names heart rot");
            True(r.Reason.Contains("55", StringComparison.Ordinal), "reason names the age");

            BirchAlderTimberResult young = BirchAlderEngine.GradeTimber(
                Stem(species: BirchAlderSpecies.CommonAlder, dbh: 55, form: true, age: 49, seed: stem));
            True(!young.HeartRotDowngrade, "no downgrade before 50");
            True(young.QualityStemUnpriced, "the same stem is a quality stem at 49");
        });

        TestRunner.Add(Suite, "TIMBER-04 invalid stems are refused", () =>
        {
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.GradeTimber(Stem(volume: -0.1m)), "negative volume");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.GradeTimber(Stem(dbh: double.NaN)), "NaN dbh");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.GradeTimber(Stem(dbh: -1)), "negative dbh");
            Throws<ArgumentOutOfRangeException>(() => BirchAlderEngine.GradeTimber(Stem(age: -1)), "negative age");
            Throws<ArgumentException>(() => BirchAlderEngine.GradeTimber(Stem(species: SpeciesMoistureTable.SitkaSpruce)), "wrong species");
            Eq(0m, BirchAlderEngine.GradeTimber(Stem(volume: 0m)).GrossValueEur, "zero volume, zero value");
        });
    }
}
