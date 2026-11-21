namespace SitkaP1.Core;

/// <summary>One polygon vertex or tree position in integer millimetres of the experiment frame (+X east, +Y north).</summary>
internal readonly record struct MillimetrePoint(long XMm, long YMm);

/// <summary>
/// Exact integer geometry for the MA-1 multi-area experiment. Every test here uses whole millimetres and 64-bit integer
/// cross products, so a result never depends on floating-point rounding. Areas are required to be strictly convex
/// polygons listed counter-clockwise; that keeps the inside test and the overlap test simple and exact.
/// </summary>
internal static class MultiAreaGeometry
{
    /// <summary>10 km. Keeps every cross product (at most 2 × 10^14) far inside a 64-bit integer.</summary>
    internal const long MaxCoordinateMm = 10_000_000;

    /// <summary>
    /// Cross product of (b − a) and (p − a). Positive: p is left of the directed line a → b. Zero: p is on the line.
    /// </summary>
    internal static long Cross(MillimetrePoint a, MillimetrePoint b, MillimetrePoint p) =>
        checked((b.XMm - a.XMm) * (p.YMm - a.YMm) - (b.YMm - a.YMm) * (p.XMm - a.XMm));

    /// <summary>Twice the signed shoelace area in mm². Positive for a counter-clockwise polygon.</summary>
    internal static long TwiceSignedAreaMm2(IReadOnlyList<MillimetrePoint> polygon)
    {
        long sum = 0;
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            sum = checked(sum + a.XMm * b.YMm - b.XMm * a.YMm);
        }
        return sum;
    }

    /// <summary>
    /// Refuses (PolygonInvalid) anything but a strictly convex, counter-clockwise polygon of at least three distinct
    /// vertices inside [0, 10 km]. Every vertex must lie strictly left of every edge it is not part of; that one exact
    /// test excludes clockwise order, collinear vertices, repeated vertices and self-intersecting (star) shapes.
    /// </summary>
    internal static void RequireStrictlyConvexCounterClockwise(IReadOnlyList<MillimetrePoint> polygon, string what)
    {
        if (polygon is null || polygon.Count < 3)
            throw new P1Refusal("PolygonInvalid", what + " needs at least three vertices");
        foreach (var p in polygon)
            if (p.XMm < 0 || p.YMm < 0 || p.XMm > MaxCoordinateMm || p.YMm > MaxCoordinateMm)
                throw new P1Refusal("PolygonInvalid", what + " has a vertex outside [0, 10 km]");
        for (int i = 0; i < polygon.Count; i++)
        for (int j = i + 1; j < polygon.Count; j++)
            if (polygon[i] == polygon[j])
                throw new P1Refusal("PolygonInvalid", what + " repeats a vertex");
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            for (int k = 0; k < polygon.Count; k++)
            {
                if (k == i || k == (i + 1) % polygon.Count) continue;
                if (Cross(a, b, polygon[k]) <= 0)
                    throw new P1Refusal("PolygonInvalid",
                        what + " is not a strictly convex counter-clockwise polygon (vertex " + k + " against edge " + i + ")");
            }
        }
    }

    /// <summary>True only when p is strictly inside a strictly convex counter-clockwise polygon. A point on an edge is outside.</summary>
    internal static bool StrictlyInside(IReadOnlyList<MillimetrePoint> polygon, MillimetrePoint p)
    {
        for (int i = 0; i < polygon.Count; i++)
            if (Cross(polygon[i], polygon[(i + 1) % polygon.Count], p) <= 0) return false;
        return true;
    }

    /// <summary>
    /// True when p is inside the polygon and at least <paramref name="marginMm"/> from every edge line (exact, using
    /// 128-bit squares). Used only by the synthetic generator to keep trees away from the boundary.
    /// </summary>
    internal static bool InsideWithMargin(IReadOnlyList<MillimetrePoint> polygon, MillimetrePoint p, long marginMm)
    {
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            long cross = Cross(a, b, p);
            if (cross <= 0) return false;
            Int128 dx = b.XMm - a.XMm;
            Int128 dy = b.YMm - a.YMm;
            // distance = cross / |b − a| >= margin  ⇔  cross² >= margin² · |b − a|² (both sides non-negative).
            if ((Int128)cross * cross < (Int128)marginMm * marginMm * (dx * dx + dy * dy)) return false;
        }
        return true;
    }

    /// <summary>
    /// True when two strictly convex counter-clockwise polygons share a region of positive area. Touching along an edge
    /// or at a vertex is not an overlap. Separating-axis test with exact integers: the polygons do not overlap exactly
    /// when some edge of either one has every vertex of the other on or to the right of its line.
    /// </summary>
    internal static bool Overlap(IReadOnlyList<MillimetrePoint> first, IReadOnlyList<MillimetrePoint> second) =>
        !HasSeparatingEdge(first, second) && !HasSeparatingEdge(second, first);

    private static bool HasSeparatingEdge(IReadOnlyList<MillimetrePoint> owner, IReadOnlyList<MillimetrePoint> other)
    {
        for (int i = 0; i < owner.Count; i++)
        {
            var a = owner[i];
            var b = owner[(i + 1) % owner.Count];
            if (other.All(p => Cross(a, b, p) <= 0)) return true;
        }
        return false;
    }

    /// <summary>
    /// The exact area in square metres as a decimal string. <paramref name="twiceAreaMm2"/> / 2 is the area in mm², so
    /// the value in m² is twiceAreaMm2 / 2,000,000, which always has a finite decimal form (at most 7 decimals).
    /// </summary>
    internal static string ExactSquareMetres(long twiceAreaMm2)
    {
        if (twiceAreaMm2 <= 0) throw new P1Refusal("PolygonInvalid", "polygon area must be positive");
        long whole = twiceAreaMm2 / 2_000_000;
        long remainder = twiceAreaMm2 % 2_000_000;
        if (remainder == 0) return whole.ToString(System.Globalization.CultureInfo.InvariantCulture);
        string fraction = (remainder * 5).ToString("D7", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0');
        return whole.ToString(System.Globalization.CultureInfo.InvariantCulture) + "." + fraction;
    }

    /// <summary>
    /// Hectares as the engine denominator: one binary64 division of the exact integer, then one float32 cast, like the
    /// P1 fixture's declared area. This is the value passed to AnnualRules.Calculate and WorkRules.Summarize for the area.
    /// </summary>
    internal static float Hectares(long twiceAreaMm2) => (float)((double)twiceAreaMm2 / 2e10);

    /// <summary>The polygon's bounding box in millimetres.</summary>
    internal static (long MinX, long MinY, long MaxX, long MaxY) Bounds(IReadOnlyList<MillimetrePoint> polygon) =>
        (polygon.Min(p => p.XMm), polygon.Min(p => p.YMm), polygon.Max(p => p.XMm), polygon.Max(p => p.YMm));
}
