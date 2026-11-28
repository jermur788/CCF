using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

/// <summary>Shared recipe fixtures and small geometry helpers. Every value here is a test choice, not a game rule.</summary>
internal static class Fixtures
{
    public static readonly IReadOnlyList<SpeciesMixEntry> StandardMix = Mix(
        (SpeciesMoistureTable.SitkaSpruce, 50.0),
        (SpeciesMoistureTable.SessileOak, 20.0),
        (SpeciesMoistureTable.EuropeanBeech, 10.0),
        (SpeciesMoistureTable.ScotsPine, 10.0),
        (SpeciesMoistureTable.DownyBirch, 5.0),
        (SpeciesMoistureTable.CommonAlder, 5.0));

    public static WoodlandShapeSpec StandardShape => WoodlandShapeSpec.Rectangle(80.0, 60.0);

    public static WoodlandShapeSpec SmallShape => WoodlandShapeSpec.Rectangle(40.0, 40.0);

    public static IReadOnlyList<SpeciesMixEntry> Mix(params (string Species, double Percent)[] entries)
    {
        var mix = new List<SpeciesMixEntry>(entries.Length);
        foreach ((string species, double percent) in entries)
            mix.Add(new SpeciesMixEntry(species, percent));
        return mix.AsReadOnly();
    }

    public static CustomWoodlandRecipe Recipe(
        WoodlandShapeSpec? boundary = null,
        int laneCount = 4,
        MoisturePatternSpec? pattern = null,
        IReadOnlyList<MoisturePaintOverride>? overrides = null,
        DeerBrowsePreset deer = DeerBrowsePreset.Standard,
        IReadOnlyList<SpeciesMixEntry>? mix = null,
        int startAge = 25,
        int stocking = 1500,
        int edgeSeedTrees = 0,
        ulong seed = 20261128UL) =>
        new(
            boundary ?? StandardShape,
            laneCount,
            pattern ?? MoisturePatternSpec.Uniform(SiteMoistureClass.FreshMoistMineral),
            overrides ?? Array.Empty<MoisturePaintOverride>(),
            deer,
            mix ?? StandardMix,
            startAge,
            stocking,
            edgeSeedTrees,
            seed);

    /// <summary>Boundary-inclusive point-in-ring test, used to prove a tree sits in the lane it was assigned.</summary>
    public static bool RingContains(IReadOnlyList<PlanPoint> ring, PlanPoint point)
    {
        bool inside = false;
        for (int i = 0, j = ring.Count - 1; i < ring.Count; j = i++)
        {
            PlanPoint a = ring[i];
            PlanPoint b = ring[j];

            if (WoodlandPolygon.DistanceToSegment(a, b, point) <= 1e-9)
                return true;

            if ((a.Z > point.Z) != (b.Z > point.Z))
            {
                double crossingX = a.X + (point.Z - a.Z) * (b.X - a.X) / (b.Z - a.Z);
                if (crossingX > point.X)
                    inside = !inside;
            }
        }

        return inside;
    }

    public static bool IsCounterClockwise(WoodlandPolygon polygon) =>
        WoodlandPolygon.SignedAreaTwice(polygon.Vertices) > 0.0;
}
