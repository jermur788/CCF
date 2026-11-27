using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using static SitkaP1.Core.BirchAlder.Tests.TestRunner;

namespace SitkaP1.Core.BirchAlder.Tests;

/// <summary>
/// Builds one canonical text over a fixed grid of engine outputs and hashes it. Doubles are written as raw IEEE-754 bits
/// so any change in the last bit changes the digest. Running the executable twice must print the same digest.
/// </summary>
internal static class DeterminismTests
{
    private const string Suite = "Determinism";

    /// <summary>Digest observed on the first verified run (SDK 8.0.133). Any output change must update it deliberately.</summary>
    internal const string PinnedDigest = "ca2e61bd3432329352cdd63a5c2c2b97bc7ba1824f284aee7728e627f1740f23";

    public static string CanonicalDigest() =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(CanonicalText()))).ToLowerInvariant();

    private static string Bits(double value) => BitConverter.DoubleToInt64Bits(value).ToString("X16", CultureInfo.InvariantCulture);

    private static string Bits(double? value) => value is { } v ? Bits(v) : "null";

    private static string CanonicalText()
    {
        var text = new StringBuilder();
        foreach (string species in BirchAlderEngine.SupportedSpecies)
        foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
        {
            BirchAlderSite site = BirchAlderEngine.Site(species, moisture);
            text.Append("M|").Append(species).Append('|').Append(moisture).Append('|')
                .Append(Bits(site.Response.EstablishmentMultiplier)).Append('|')
                .Append(Bits(site.Response.GrowthMultiplier)).Append('|')
                .Append(site.HabitatProtectionCaution).Append('\n');
        }

        WetValleyAndSlopeMap map = WetValleyAndSlopeMap.CreateStandard();
        for (int i = 0; i <= 16; i++)
        for (int j = 0; j <= 16; j++)
            text.Append("Z|").Append(i).Append('|').Append(j).Append('|')
                .Append(map.MoistureAt(i * 5.0, j * 5.0)).Append('|')
                .Append(map.LaneIndexOf(i * 5.0)).Append('\n');

        foreach (string species in BirchAlderEngine.SupportedSpecies)
        {
            GrowthTeachingDomain domain = BirchAlderEngine.GrowthDomain(species);
            for (int age = (int)domain.AgeYears.Low; age <= (int)domain.AgeYears.High; age += 5)
            for (int stems = 200; stems <= 4000; stems += 700)
            {
                BirchAlderGrowthResult g = BirchAlderEngine.EvaluateGrowth(new(
                    species, age, stems, 10 + age, 6.0 + age / 4.0, SiteMoistureClass.FreshMoistMineral));
                text.Append("G|").Append(species).Append('|').Append(age).Append('|').Append(stems).Append('|')
                    .Append(Bits(g.PotentialDbhIncrementCm)).Append('|').Append(Bits(g.SiteAdjustedDbhIncrementCm)).Append('\n');
            }

            foreach (ulong seed in new[] { 0UL, 7UL, 0xC0FFEE20261127UL })
            for (int year = 0; year < 40; year++)
            foreach (SiteMoistureClass moisture in new[] { SiteMoistureClass.WetMovingAlluvial, SiteMoistureClass.NeutralLegacy })
            {
                BirchAlderRegenerationResult r = BirchAlderEngine.EvaluateRegeneration(new(
                    species, seed, year, 15 + year % 4, 0.50 + (year % 6) * 0.05, (year % 5) * 0.1,
                    moisture, ExposedMineralGround: year % 2 == 0));
                text.Append("R|").Append(species).Append('|').Append(seed).Append('|').Append(year).Append('|')
                    .Append(moisture).Append('|')
                    .Append(r.Status).Append('|').Append(Bits(r.EstablishmentMultiplier)).Append('\n');
            }
        }

        for (int age = 45; age <= 65; age++)
        {
            text.Append("H|").Append(age).Append('|')
                .Append(Bits(BirchAlderEngine.AlderHeartRotRiskProbability(age))).Append('|');
            int downgraded = 0;
            for (ulong stem = 0; stem < 100; stem++)
                if (BirchAlderEngine.InspectAlderHeartRot(stem, age)) downgraded++;
            text.Append(downgraded).Append('\n');

            // Per-stem flags, so reassigning which identities downgrade changes the digest even when the total does not.
            text.Append("HS|").Append(age).Append('|');
            for (ulong stem = 0; stem < 12; stem++)
                text.Append(BirchAlderEngine.InspectAlderHeartRot(stem, age) ? '1' : '0');
            text.Append('\n');
        }

        foreach (string species in BirchAlderEngine.SupportedSpecies)
        foreach (bool mature in new[] { true, false })
        foreach (bool form in new[] { true, false })
        foreach (double dbh in new[] { 20.0, 30.0, 50.0, 55.0 })
        foreach (int age in new[] { 20, 40, 55 })
        {
            BirchAlderTimberResult t = BirchAlderEngine.GradeTimber(new(
                species, 1UL, 1.37m, mature, dbh, form, age));
            text.Append("T|").Append(species).Append('|').Append(mature).Append('|').Append(form).Append('|')
                .Append(Bits(dbh)).Append('|').Append(age).Append('|')
                .Append(t.Sortment).Append('|').Append(t.UnitPriceEurPerTonne.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(t.GreenTonnes.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(t.GrossValueEur.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(t.QualityStemUnpriced).Append('|').Append(t.HeartRotDowngrade).Append('|')
                .Append(t.BuyerNote ?? "none").Append('|').Append(t.Reason).Append('\n');
        }

        return text.ToString();
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "DET-01 canonical output text and SHA-256 are identical across repeated evaluations", () =>
        {
            string first = CanonicalText();
            string second = CanonicalText();
            Eq(first, second, "canonical text");
            Eq(CanonicalDigest(), CanonicalDigest(), "digest");
            True(first.Length > 1000, "canonical text covers the grid");
            Eq(PinnedDigest, CanonicalDigest(), "pinned digest");
        });
    }
}
