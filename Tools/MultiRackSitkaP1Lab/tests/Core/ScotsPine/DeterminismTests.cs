using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;
using static SitkaP1.Core.ScotsPine.Tests.TestRunner;

namespace SitkaP1.Core.ScotsPine.Tests;

/// <summary>
/// Builds one canonical text over a fixed grid of engine outputs and hashes it. Doubles are written as raw IEEE-754 bits
/// so any change in the last bit changes the digest. Running the executable twice must print the same digest.
/// </summary>
internal static class DeterminismTests
{
    private const string Suite = "Determinism";

    /// <summary>Digest observed on the first verified run (SDK 8.0.133). Any output change must update it deliberately.</summary>
    internal const string PinnedDigest = "96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748";

    public static string CanonicalDigest() =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(CanonicalText()))).ToLowerInvariant();

    private static string Bits(double value) => BitConverter.DoubleToInt64Bits(value).ToString("X16", CultureInfo.InvariantCulture);

    private static string Bits(double? value) => value is { } v ? Bits(v) : "null";

    private static string CanonicalText()
    {
        var text = new StringBuilder();
        foreach (string species in SpeciesMoistureTable.SpeciesIds)
        foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
        {
            SiteMoistureResult r = SiteMoistureEngine.ForSpecies(species, moisture);
            text.Append("M|").Append(species).Append('|').Append(moisture).Append('|')
                .Append(Bits(r.Cell.Response.EstablishmentMultiplier)).Append('|')
                .Append(Bits(r.Cell.Response.GrowthMultiplier)).Append('|')
                .Append(r.Site.HabitatProtectionCaution).Append('\n');
        }

        DryRidgeAndMoistSlopeMap map = SiteMoistureEngine.CreateDryRidgeAndMoistSlope(new StandBounds(0, 0, 90, 45));
        for (int i = 0; i <= 18; i++)
            text.Append("Z|").Append(i).Append('|').Append(map.At(i * 5.0, 22.5).Moisture).Append('\n');

        for (int age = 10; age <= 60; age += 5)
        for (int stems = 100; stems <= 5000; stems += 700)
        {
            ScotsPineGrowthResult g = ScotsPineEngine.EvaluateGrowth(new(age, stems, 10 + age, 5.0 + age / 3.0, SiteMoistureClass.DryFreeDraining));
            text.Append("G|").Append(age).Append('|').Append(stems).Append('|')
                .Append(Bits(g.PotentialDbhIncrementCm)).Append('|').Append(Bits(g.SiteAdjustedDbhIncrementCm)).Append('\n');
        }

        foreach (ulong seed in new[] { 0UL, 7UL, 0xC0FFEE20261127UL })
        for (int year = 0; year < 50; year++)
        {
            ScotsPineRegenerationResult r = ScotsPineEngine.EvaluateRegeneration(
                new(seed, year, 20 + year % 3, 0.55 + (year % 5) * 0.05, (year % 4) * 0.1, SiteMoistureClass.FreshMoistMineral));
            text.Append("R|").Append(seed).Append('|').Append(year).Append('|').Append(r.Status).Append('|')
                .Append(Bits(r.EstablishmentMultiplier)).Append('\n');
        }

        foreach (bool mature in new[] { true, false })
        foreach (double pruned in new[] { 0.0, 4.9, 5.0, 6.0 })
        foreach (bool tight in new[] { true, false })
        foreach (bool srw in new[] { true, false })
        {
            ScotsPineTimberResult t = ScotsPineEngine.GradeTimber(new(1.37m, mature, pruned, tight, srw));
            text.Append("T|").Append(mature).Append('|').Append(Bits(pruned)).Append('|').Append(tight).Append('|').Append(srw).Append('|')
                .Append(t.Sortment).Append('|').Append(t.UnitPriceEurPerTonne.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(t.GreenTonnes.ToString(CultureInfo.InvariantCulture)).Append('|')
                .Append(t.GrossValueEur.ToString(CultureInfo.InvariantCulture)).Append('|').Append(t.Reason).Append('\n');
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
