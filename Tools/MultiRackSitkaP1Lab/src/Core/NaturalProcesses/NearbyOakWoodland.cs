using System.Globalization;

namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// Nearby Oak Woodland: an authored, virtual oak source line just outside ONE outer edge of the woodland's extent. It is a
/// world option, not a simulated off-map stand: it has no trees, no maturity and no IDs. In an existing oak mast year it
/// offers up to 6 candidate acorns per hectare (nearest integer), carried by the same jay transport. Acorns that do not
/// land inside the woodland are exported (lost) and never retried.
/// </summary>
internal static class NearbyOakWoodland
{
    /// <summary>Gameplay calibration: external candidates per woodland hectare per mast year.</summary>
    public const double CandidatesPerHectare = 6.0;

    /// <summary>Authored offset of the virtual source line outside the edge (metres). Presentation geometry, not ecology.</summary>
    public const double SourceLineOffsetM = 1.0;

    public static string SourceKey(OuterEdge edge) =>
        NaturalProcessStreams.NearbySourcePrefix + edge.ToString();

    public static IReadOnlyList<JayAcornCandidate> Offer(
        NaturalProcessRandom rng, int year, int seed, OuterEdge edge, int count, WoodlandFootprint woodland)
    {
        string sourceKey = SourceKey(edge);
        var carried = new List<JayAcornCandidate>(count);
        for (int index = 0; index < count; index++)
        {
            double along = rng.Roll(JayAcornTransport.StreamKey(sourceKey, index) + NaturalProcessStreams.NearbyAlongSuffix, year, seed);
            LocalPoint source = PointOnSourceLine(edge, along, woodland);
            string candidateId = sourceKey + "-" + index.ToString(CultureInfo.InvariantCulture);
            carried.Add(JayAcornTransport.Carry(rng, year, seed, sourceKey, index, candidateId,
                JayCandidateOrigin.NearbyOakWoodland, sourceTreeId: null, sourceEdge: edge, source, woodland));
        }
        return carried;
    }

    /// <summary>A point on the source line for a fraction 0..1 along the chosen edge.</summary>
    public static LocalPoint PointOnSourceLine(OuterEdge edge, double fraction, WoodlandFootprint woodland)
    {
        double x = woodland.MinX + fraction * (woodland.MaxX - woodland.MinX);
        double y = woodland.MinY + fraction * (woodland.MaxY - woodland.MinY);
        return edge switch
        {
            OuterEdge.North => new LocalPoint(x, woodland.MaxY + SourceLineOffsetM),
            OuterEdge.South => new LocalPoint(x, woodland.MinY - SourceLineOffsetM),
            OuterEdge.East => new LocalPoint(woodland.MaxX + SourceLineOffsetM, y),
            OuterEdge.West => new LocalPoint(woodland.MinX - SourceLineOffsetM, y),
            _ => throw new ArgumentOutOfRangeException(nameof(edge)),
        };
    }
}
