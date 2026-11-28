namespace SitkaP1.Core.CustomWoodlandBuilder;

/// <summary>A point on the flat local ground plane, in metres. X is east; Z is north.</summary>
public readonly record struct PlanPoint(double X, double Z)
{
    public bool IsFinite => double.IsFinite(X) && double.IsFinite(Z);

    public override string ToString() => string.Create(
        System.Globalization.CultureInfo.InvariantCulture, $"({X:R}, {Z:R})");
}

/// <summary>
/// A validated simple polygon in flat local metres. Every consumer of a custom woodland boundary (the lane clipper,
/// the moisture grid and the stand generator) reads containment and distance from this one type, so they cannot
/// disagree about where the field ends.
///
/// Validation is deliberately strict and fails closed:
/// - 3 to 16 vertices, all finite, no repeated closing vertex, no zero-length edge and no spike;
/// - no touching or crossing between non-adjacent edges (a bowed field is refused, not repaired);
/// - a bounding footprint of 30-120 m on both axes and at least 450 m2 of area;
/// - the ring is normalised to counter-clockwise with a canonical start vertex.
///
/// These limits are gameplay calibration sizing guardrails, not a survey standard.
/// </summary>
public sealed class WoodlandPolygon
{
    /// <summary>Smallest permitted bounding-footprint extent on either axis, in metres. Calibration.</summary>
    public const double MinimumFootprintM = 30.0;

    /// <summary>Largest permitted bounding-footprint extent on either axis, in metres. Calibration.</summary>
    public const double MaximumFootprintM = 120.0;

    /// <summary>Smallest permitted field area, in square metres. Calibration.</summary>
    public const double MinimumAreaM2 = 450.0;

    /// <summary>Largest ring this engine accepts. An L-shape with all four chamfers has at most 10 vertices.</summary>
    public const int MaximumVertexCount = 16;

    private const double Tolerance = 1e-9;

    private readonly PlanPoint[] _vertices;

    private WoodlandPolygon(PlanPoint[] vertices, double areaM2, PlanPoint centroid, double minX, double minZ,
        double maxX, double maxZ)
    {
        _vertices = vertices;
        AreaM2 = areaM2;
        Centroid = centroid;
        MinX = minX;
        MinZ = minZ;
        MaxX = maxX;
        MaxZ = maxZ;
    }

    /// <summary>Vertices in counter-clockwise order, without a repeated closing vertex.</summary>
    public IReadOnlyList<PlanPoint> Vertices => _vertices;

    public double AreaM2 { get; }

    /// <summary>Area centroid of the ring, not the average of the vertices.</summary>
    public PlanPoint Centroid { get; }

    public double MinX { get; }

    public double MinZ { get; }

    public double MaxX { get; }

    public double MaxZ { get; }

    public double WidthM => MaxX - MinX;

    public double LengthM => MaxZ - MinZ;

    public double AreaHectares => AreaM2 / 10000.0;

    /// <summary>Builds a validated polygon from a caller-supplied ring. Clockwise rings are reversed.</summary>
    public static WoodlandPolygon FromVertices(IReadOnlyList<PlanPoint> vertices)
    {
        if (vertices is null)
            throw new ArgumentNullException(nameof(vertices));

        if (vertices.Count < 3 || vertices.Count > MaximumVertexCount)
            throw new ArgumentException(
                $"a woodland boundary needs 3 to {MaximumVertexCount} vertices, got {vertices.Count}.",
                nameof(vertices));

        var ring = new PlanPoint[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            if (!vertices[i].IsFinite)
                throw new ArgumentException($"vertex {i} is not finite: {vertices[i]}.", nameof(vertices));
            ring[i] = vertices[i];
        }

        if (PointsEqual(ring[0], ring[^1]))
            throw new ArgumentException(
                "the ring must not repeat its first vertex as the last vertex.", nameof(vertices));

        for (int i = 0; i < ring.Length; i++)
        {
            if (PointsEqual(ring[i], ring[(i + 1) % ring.Length]))
                throw new ArgumentException(
                    $"vertices {i} and {(i + 1) % ring.Length} are the same point, so an edge has zero length.",
                    nameof(vertices));
        }

        for (int i = 0; i < ring.Length; i++)
        {
            PlanPoint previous = ring[(i - 1 + ring.Length) % ring.Length];
            PlanPoint current = ring[i];
            PlanPoint next = ring[(i + 1) % ring.Length];
            if (IsSpike(previous, current, next))
                throw new ArgumentException(
                    $"vertex {i} reverses back along its own edge (a spike), which is not a simple polygon.",
                    nameof(vertices));
        }

        if (HasTouchingEdgePair(ring))
            throw new ArgumentException(
                "the ring is not a simple polygon: two non-adjacent edges touch or cross.", nameof(vertices));

        double signedAreaTwice = SignedAreaTwice(ring);
        double area = Math.Abs(signedAreaTwice) / 2.0;
        if (!double.IsFinite(area) || area < MinimumAreaM2)
            throw new ArgumentException(
                $"the field area {area:F1} m2 is below the {MinimumAreaM2:F0} m2 minimum.", nameof(vertices));

        if (signedAreaTwice < 0.0)
        {
            Array.Reverse(ring);
        }

        ring = RotateToCanonicalStart(ring);

        double minX = double.PositiveInfinity, minZ = double.PositiveInfinity;
        double maxX = double.NegativeInfinity, maxZ = double.NegativeInfinity;
        foreach (PlanPoint point in ring)
        {
            minX = Math.Min(minX, point.X);
            minZ = Math.Min(minZ, point.Z);
            maxX = Math.Max(maxX, point.X);
            maxZ = Math.Max(maxZ, point.Z);
        }

        double width = maxX - minX;
        double length = maxZ - minZ;
        if (width < MinimumFootprintM - Tolerance || length < MinimumFootprintM - Tolerance ||
            width > MaximumFootprintM + Tolerance || length > MaximumFootprintM + Tolerance)
            throw new ArgumentException(
                $"the bounding footprint {width:F1} x {length:F1} m is outside the permitted " +
                $"{MinimumFootprintM:F0}-{MaximumFootprintM:F0} m range on both axes.", nameof(vertices));

        var polygon = new WoodlandPolygon(ring, area, ComputeCentroid(ring, area), minX, minZ, maxX, maxZ);
        return polygon;
    }

    /// <summary>Builds a validated polygon from one of the four Irish field-shape templates.</summary>
    public static WoodlandPolygon FromShape(WoodlandShapeSpec shape) =>
        WoodlandBoundaryTemplates.Build(shape);

    /// <summary>True when the point is inside the ring or exactly on its boundary.</summary>
    public bool ContainsPoint(PlanPoint point)
    {
        if (!point.IsFinite)
            return false;

        bool inside = false;
        for (int i = 0, j = _vertices.Length - 1; i < _vertices.Length; j = i++)
        {
            PlanPoint a = _vertices[i];
            PlanPoint b = _vertices[j];

            if (PointOnSegment(a, b, point))
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

    /// <summary>Distance in metres from the point to the nearest boundary edge. Zero when outside or on the edge.</summary>
    public double DistanceToBoundary(PlanPoint point)
    {
        double best = double.PositiveInfinity;
        for (int i = 0; i < _vertices.Length; i++)
        {
            PlanPoint a = _vertices[i];
            PlanPoint b = _vertices[(i + 1) % _vertices.Length];
            best = Math.Min(best, DistanceToSegment(a, b, point));
        }

        return best;
    }

    /// <summary>True when the point is inside and at least <paramref name="clearanceM"/> from every edge.</summary>
    public bool ContainsWithClearance(PlanPoint point, double clearanceM) =>
        ContainsPoint(point) && DistanceToBoundary(point) >= clearanceM;

    /// <summary>Signed area twice the polygon area. Positive for counter-clockwise rings.</summary>
    public static double SignedAreaTwice(IReadOnlyList<PlanPoint> ring)
    {
        double sum = 0.0;
        for (int i = 0; i < ring.Count; i++)
        {
            PlanPoint current = ring[i];
            PlanPoint next = ring[(i + 1) % ring.Count];
            sum += current.X * next.Z - next.X * current.Z;
        }

        return sum;
    }

    /// <summary>Distance from a point to a finite segment.</summary>
    public static double DistanceToSegment(PlanPoint a, PlanPoint b, PlanPoint point)
    {
        double dx = b.X - a.X;
        double dz = b.Z - a.Z;
        double lengthSquared = dx * dx + dz * dz;
        if (lengthSquared <= 0.0)
            return Math.Sqrt((point.X - a.X) * (point.X - a.X) + (point.Z - a.Z) * (point.Z - a.Z));

        double t = ((point.X - a.X) * dx + (point.Z - a.Z) * dz) / lengthSquared;
        t = Math.Clamp(t, 0.0, 1.0);
        double projectedX = a.X + t * dx;
        double projectedZ = a.Z + t * dz;
        double offsetX = point.X - projectedX;
        double offsetZ = point.Z - projectedZ;
        return Math.Sqrt(offsetX * offsetX + offsetZ * offsetZ);
    }

    private static bool PointsEqual(PlanPoint a, PlanPoint b) =>
        Math.Abs(a.X - b.X) <= Tolerance && Math.Abs(a.Z - b.Z) <= Tolerance;

    private static bool PointOnSegment(PlanPoint a, PlanPoint b, PlanPoint point) =>
        DistanceToSegment(a, b, point) <= Tolerance;

    private static bool IsSpike(PlanPoint previous, PlanPoint current, PlanPoint next)
    {
        double inX = current.X - previous.X;
        double inZ = current.Z - previous.Z;
        double outX = next.X - current.X;
        double outZ = next.Z - current.Z;

        double cross = inX * outZ - inZ * outX;
        double inLength = Math.Sqrt(inX * inX + inZ * inZ);
        double outLength = Math.Sqrt(outX * outX + outZ * outZ);
        double scale = Math.Max(1.0, inLength * outLength);

        // Collinear and pointing back the way it came: the ring doubles back over itself.
        return Math.Abs(cross) <= Tolerance * scale && inX * outX + inZ * outZ < 0.0;
    }

    private static bool HasTouchingEdgePair(PlanPoint[] ring)
    {
        int count = ring.Length;
        for (int i = 0; i < count; i++)
        {
            PlanPoint a = ring[i];
            PlanPoint b = ring[(i + 1) % count];

            for (int j = i + 1; j < count; j++)
            {
                bool adjacent = j == i + 1 || (i == 0 && j == count - 1);
                if (adjacent)
                    continue;

                PlanPoint c = ring[j];
                PlanPoint d = ring[(j + 1) % count];
                if (SegmentsTouch(a, b, c, d))
                    return true;
            }
        }

        return false;
    }

    private static bool SegmentsTouch(PlanPoint a, PlanPoint b, PlanPoint c, PlanPoint d)
    {
        double rx = b.X - a.X;
        double rz = b.Z - a.Z;
        double sx = d.X - c.X;
        double sz = d.Z - c.Z;
        double qpx = c.X - a.X;
        double qpz = c.Z - a.Z;

        double denominator = rx * sz - rz * sx;
        double scale = Math.Max(1.0, Math.Sqrt(rx * rx + rz * rz) * Math.Sqrt(sx * sx + sz * sz));

        if (Math.Abs(denominator) > Tolerance * scale)
        {
            double t = (qpx * sz - qpz * sx) / denominator;
            double u = (qpx * rz - qpz * rx) / denominator;
            return t >= -Tolerance && t <= 1.0 + Tolerance && u >= -Tolerance && u <= 1.0 + Tolerance;
        }

        // Parallel: only a collinear overlap counts as touching.
        if (Math.Abs(qpx * rz - qpz * rx) > Tolerance * Math.Max(1.0, Math.Sqrt(rx * rx + rz * rz)))
            return false;

        double lengthSquared = rx * rx + rz * rz;
        if (lengthSquared <= 0.0)
            return false;

        double startOnFirst = (qpx * rx + qpz * rz) / lengthSquared;
        double endOnFirst = ((d.X - a.X) * rx + (d.Z - a.Z) * rz) / lengthSquared;
        double low = Math.Min(startOnFirst, endOnFirst);
        double high = Math.Max(startOnFirst, endOnFirst);
        return high >= -Tolerance && low <= 1.0 + Tolerance;
    }

    private static PlanPoint[] RotateToCanonicalStart(PlanPoint[] ring)
    {
        int start = 0;
        for (int i = 1; i < ring.Length; i++)
        {
            if (ring[i].X < ring[start].X - Tolerance ||
                (Math.Abs(ring[i].X - ring[start].X) <= Tolerance && ring[i].Z < ring[start].Z - Tolerance))
                start = i;
        }

        if (start == 0)
            return ring;

        var rotated = new PlanPoint[ring.Length];
        for (int i = 0; i < ring.Length; i++)
            rotated[i] = ring[(start + i) % ring.Length];
        return rotated;
    }

    private static PlanPoint ComputeCentroid(PlanPoint[] ring, double area)
    {
        double accumulatedX = 0.0;
        double accumulatedZ = 0.0;
        for (int i = 0; i < ring.Length; i++)
        {
            PlanPoint current = ring[i];
            PlanPoint next = ring[(i + 1) % ring.Length];
            double cross = current.X * next.Z - next.X * current.Z;
            accumulatedX += (current.X + next.X) * cross;
            accumulatedZ += (current.Z + next.Z) * cross;
        }

        double factor = 1.0 / (6.0 * area);
        return new PlanPoint(accumulatedX * factor, accumulatedZ * factor);
    }
}
