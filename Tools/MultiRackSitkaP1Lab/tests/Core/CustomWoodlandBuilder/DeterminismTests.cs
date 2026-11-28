using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

internal static class DeterminismTests
{
    /// <summary>
    /// Pinned canonical digest for the fixture below, under .NET SDK 8.0.133. Changing any tree position, moisture
    /// class, lane vertex, deer value or mix weight changes this value.
    /// </summary>
    public const string PinnedDigest = "86db0555aaf41405995b4213ecb3cbcadd9c0bab8fa36ac58bc37412dce8cbf5";

    public static void Register()
    {
        TestRunner.Add("Determinism", "DET-01 the canonical digest matches the pinned value", DigestPinned);
        TestRunner.Add("Determinism", "DET-02 canonical text is stable, complete and sensitive to overrides", CanonicalTextIsComplete);
    }

    /// <summary>The digest the console gate prints, computed over the whole fixture build.</summary>
    public static string CanonicalDigest()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(CanonicalRecipe());
        byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(build.CanonicalText));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static CustomWoodlandRecipe CanonicalRecipe() => Fixtures.Recipe(
        laneCount: 5,
        pattern: MoisturePatternSpec.WetHollowBogPocket(9.0),
        overrides: new[] { new MoisturePaintOverride(7, 7, SiteMoistureClass.WetMovingAlluvial) },
        deer: DeerBrowsePreset.Heavy,
        stocking: 1500,
        edgeSeedTrees: 6);

    private static void DigestPinned()
    {
        string digest = CanonicalDigest();
        TestRunner.Eq(PinnedDigest, digest, "canonical digest");
    }

    private static void CanonicalTextIsComplete()
    {
        CustomWoodlandBuild build = CustomWoodlandBuilder.Build(CanonicalRecipe());
        CustomWoodlandBuild again = CustomWoodlandBuilder.Build(CanonicalRecipe());

        TestRunner.Eq(build.CanonicalText, again.CanonicalText, "the canonical text is stable inside one process");

        int treeLines = 0;
        foreach (string line in build.CanonicalText.Split('\n'))
        {
            if (line.StartsWith("tree ", StringComparison.Ordinal))
                treeLines++;
        }

        TestRunner.Eq(build.Stand.Trees.Count, treeLines, "one canonical line per tree");
        TestRunner.True(
            build.CanonicalText.Contains("moisture pattern=WetHollowBogPocket", StringComparison.Ordinal),
            "the pattern is recorded");
        TestRunner.True(
            build.CanonicalText.Contains("preset=Heavy", StringComparison.Ordinal),
            "the deer preset is recorded");
        TestRunner.True(
            build.CanonicalText.Contains("caution=1", StringComparison.Ordinal),
            "at least one bog tree was recorded");
        TestRunner.True(
            !build.CanonicalText.Contains("NaN", StringComparison.Ordinal),
            "no canonical value is NaN");

        CustomWoodlandBuild repainted = CustomWoodlandBuilder.Build(Fixtures.Recipe(
            laneCount: 5,
            pattern: MoisturePatternSpec.WetHollowBogPocket(9.0),
            overrides: Array.Empty<MoisturePaintOverride>(),
            deer: DeerBrowsePreset.Heavy,
            stocking: 1500,
            edgeSeedTrees: 6));

        TestRunner.True(build.CanonicalText != repainted.CanonicalText, "removing a paint override changes the build");
        TestRunner.Eq(build.Stand.Trees.Count, repainted.Stand.Trees.Count, "the override count does not change the stand size");
    }
}
