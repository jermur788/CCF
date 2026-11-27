using System.Globalization;

namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// The one transport routine shared by on-map and Nearby Oak Woodland acorns. Two keyed draws per acorn, keyed by
/// (year, source, candidateIndex): distance r = 5 + 195·u² metres (5–200 m, short hops more common) and a uniform bearing
/// in [0, 2π) measured anticlockwise from east. An acorn landing outside the woodland is exported seed: it is never
/// redrawn, clamped or retried. Shape and range are gameplay calibration, not a measured jay caching kernel.
/// </summary>
internal static class JayAcornTransport
{
    public const double MinDistanceM = 5.0;
    public const double DistanceSpanM = 195.0;
    public const double MaxDistanceM = MinDistanceM + DistanceSpanM;

    public static string StreamKey(string sourceKey, int candidateIndex) =>
        NaturalProcessStreams.JayAcornPrefix + sourceKey + "-" + candidateIndex.ToString(CultureInfo.InvariantCulture);

    public static JayAcornCandidate Carry(
        NaturalProcessRandom rng,
        int year,
        int seed,
        string sourceKey,
        int candidateIndex,
        string candidateId,
        JayCandidateOrigin origin,
        string? sourceTreeId,
        OuterEdge? sourceEdge,
        LocalPoint source,
        WoodlandFootprint woodland)
    {
        string key = StreamKey(sourceKey, candidateIndex);
        double u = rng.Roll(key + NaturalProcessStreams.JayDistanceSuffix, year, seed);
        double v = rng.Roll(key + NaturalProcessStreams.JayBearingSuffix, year, seed);
        double distance = MinDistanceM + DistanceSpanM * u * u;
        double bearing = 2.0 * Math.PI * v;
        var destination = new LocalPoint(source.X + distance * Math.Cos(bearing), source.Y + distance * Math.Sin(bearing));
        JayCandidateFate fate = woodland.Contains(destination)
            ? JayCandidateFate.DeliveredInsideWoodland
            : JayCandidateFate.ExportedOutsideWoodland;
        return new JayAcornCandidate(candidateId, origin, sourceTreeId, sourceEdge, source, distance, bearing, destination, fate);
    }
}
