namespace SitkaP1.Core.CustomWoodlandBuilder;

/// <summary>Why a lane band was rejected. Zero value is explicitly "not evaluated" so a default cannot read as usable.</summary>
public enum LaneRejectionReason
{
    NotEvaluated = 0,

    /// <summary>The lane is usable.</summary>
    None,

    TooFewVertices,
    BelowMinimumArea,
    BelowMinimumWidth,
}

/// <summary>One clipped work lane. <see cref="Boundary"/> is the exact polygon intersection with the lane band.</summary>
public sealed record WorkLane(
    int Index,
    IReadOnlyList<PlanPoint> Boundary,
    double MeanWidthM,
    double AreaM2,
    double MinZ,
    double MaxZ,
    LaneRejectionReason Rejection)
{
    public bool IsUsable => Rejection == LaneRejectionReason.None;
}

/// <summary>
/// The clipped lane layout for one boundary. A plan is valid only when every lane is usable; lanes are never dropped,
/// merged or widened to make a plan fit, because that would change the work layout behind the player's back.
/// </summary>
public sealed class LanePlan
{
    internal LanePlan(IReadOnlyList<WorkLane> lanes, double bandHeightM, double southZ)
    {
        Lanes = lanes;
        BandHeightM = bandHeightM;
        SouthZ = southZ;
        TotalAreaM2 = 0.0;
        foreach (WorkLane lane in lanes)
            TotalAreaM2 += lane.AreaM2;

        FailedLaneIndex = null;
        Rejection = LaneRejectionReason.None;
        IsValid = true;
        foreach (WorkLane lane in lanes)
        {
            if (lane.IsUsable)
                continue;

            IsValid = false;
            FailedLaneIndex = lane.Index;
            Rejection = lane.Rejection;
            break;
        }
    }

    /// <summary>Lanes in south-to-north order. Index 0 is the southernmost band.</summary>
    public IReadOnlyList<WorkLane> Lanes { get; }

    public int LaneCount => Lanes.Count;

    /// <summary>North-south depth of one band, in metres. All bands are equal.</summary>
    public double BandHeightM { get; }

    /// <summary>The southern edge of lane 0, in metres.</summary>
    public double SouthZ { get; }

    public double TotalAreaM2 { get; }

    public bool IsValid { get; }

    /// <summary>The first lane that failed, or null when the plan is valid.</summary>
    public int? FailedLaneIndex { get; }

    public LaneRejectionReason Rejection { get; }

    /// <summary>
    /// Lane index for a point inside the boundary. Bands are half-open from the south, so a point exactly on a band
    /// boundary belongs to the northern band; the last band includes the northern boundary.
    /// </summary>
    public int LaneIndexAt(PlanPoint point)
    {
        if (!point.IsFinite)
            throw new ArgumentOutOfRangeException(nameof(point), point, "the point must be finite.");

        int index = (int)Math.Floor((point.Z - SouthZ) / BandHeightM);
        return Math.Clamp(index, 0, LaneCount - 1);
    }
}

/// <summary>
/// Splits a boundary into equal parallel bands and clips each band against the polygon with Sutherland-Hodgman
/// half-plane clipping. Every lane vertex therefore lies inside or on the boundary by construction.
///
/// A lane that is degenerate (fewer than three distinct vertices), narrower than the declared minimum mean width, or
/// smaller than the declared minimum area refuses the whole plan. The two thresholds are calibration values for a
/// workable teaching lane, not an Irish machine-access specification.
/// </summary>
public static class LaneClipper
{
    public const int MinimumLaneCount = 2;
    public const int MaximumLaneCount = 8;

    /// <summary>Smallest usable mean lane width, in metres. Calibration.</summary>
    public const double MinimumLaneWidthM = 8.0;

    /// <summary>Smallest usable lane area, in square metres. Calibration.</summary>
    public const double MinimumLaneAreaM2 = 60.0;

    private const double Tolerance = 1e-9;

    public static LanePlan Clip(WoodlandPolygon polygon, int laneCount)
    {
        if (polygon is null)
            throw new ArgumentNullException(nameof(polygon));

        if (laneCount < MinimumLaneCount || laneCount > MaximumLaneCount)
            throw new ArgumentOutOfRangeException(
                nameof(laneCount), laneCount,
                $"a woodland needs {MinimumLaneCount} to {MaximumLaneCount} work lanes.");

        double bandHeight = polygon.LengthM / laneCount;
        var lanes = new List<WorkLane>(laneCount);

        for (int index = 0; index < laneCount; index++)
        {
            double southZ = polygon.MinZ + index * bandHeight;
            double northZ = index == laneCount - 1 ? polygon.MaxZ : polygon.MinZ + (index + 1) * bandHeight;

            List<PlanPoint> clipped = ClipToBand(polygon.Vertices, southZ, northZ);
            double area = Math.Abs(WoodlandPolygon.SignedAreaTwice(clipped)) / 2.0;
            double meanWidth = bandHeight > 0.0 ? area / bandHeight : 0.0;

            LaneRejectionReason rejection;
            if (clipped.Count < 3)
                rejection = LaneRejectionReason.TooFewVertices;
            else if (!double.IsFinite(area) || area < MinimumLaneAreaM2)
                rejection = LaneRejectionReason.BelowMinimumArea;
            else if (meanWidth < MinimumLaneWidthM)
                rejection = LaneRejectionReason.BelowMinimumWidth;
            else
                rejection = LaneRejectionReason.None;

            lanes.Add(new WorkLane(index, clipped.AsReadOnly(), meanWidth, area, southZ, northZ, rejection));
        }

        return new LanePlan(lanes.AsReadOnly(), bandHeight, polygon.MinZ);
    }

    private static List<PlanPoint> ClipToBand(IReadOnlyList<PlanPoint> subject, double southZ, double northZ)
    {
        var current = new List<PlanPoint>(subject);
        current = ClipHalfPlane(current, southZ, keepAbove: true);
        current = ClipHalfPlane(current, northZ, keepAbove: false);
        return Simplify(current);
    }

    private static List<PlanPoint> ClipHalfPlane(List<PlanPoint> subject, double zBound, bool keepAbove)
    {
        var output = new List<PlanPoint>(subject.Count + 4);
        if (subject.Count == 0)
            return output;

        for (int i = 0; i < subject.Count; i++)
        {
            PlanPoint current = subject[i];
            PlanPoint next = subject[(i + 1) % subject.Count];

            bool currentInside = keepAbove ? current.Z >= zBound : current.Z <= zBound;
            bool nextInside = keepAbove ? next.Z >= zBound : next.Z <= zBound;

            if (currentInside)
                output.Add(current);

            if (currentInside != nextInside)
            {
                double delta = next.Z - current.Z;
                if (Math.Abs(delta) <= Tolerance)
                    continue;

                double t = (zBound - current.Z) / delta;
                t = Math.Clamp(t, 0.0, 1.0);
                output.Add(new PlanPoint(current.X + t * (next.X - current.X), zBound));
            }
        }

        return output;
    }

    private static List<PlanPoint> Simplify(List<PlanPoint> points)
    {
        var simplified = new List<PlanPoint>(points.Count);
        foreach (PlanPoint point in points)
        {
            if (simplified.Count > 0 && SamePoint(simplified[^1], point))
                continue;
            simplified.Add(point);
        }

        while (simplified.Count > 1 && SamePoint(simplified[0], simplified[^1]))
            simplified.RemoveAt(simplified.Count - 1);

        return simplified;
    }

    private static bool SamePoint(PlanPoint a, PlanPoint b) =>
        Math.Abs(a.X - b.X) <= Tolerance && Math.Abs(a.Z - b.Z) <= Tolerance;
}
