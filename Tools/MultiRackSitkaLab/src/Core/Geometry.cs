namespace SitkaLab.Core;

/// <summary>Exact integer-millimetre polygon geometry. All intermediate products use Int128.</summary>
public static class Geometry
{
    private static Int128 Cross(Vertex o, Vertex a, Vertex b) =>
        (Int128)(a.XMm - o.XMm) * (b.YMm - o.YMm) - (Int128)(a.YMm - o.YMm) * (b.XMm - o.XMm);

    private static int Sign(Int128 v) => v > 0 ? 1 : v < 0 ? -1 : 0;

    private static bool OnSegment(Vertex a, Vertex b, Vertex p) =>
        Cross(a, b, p) == 0 &&
        Math.Min(a.XMm, b.XMm) <= p.XMm && p.XMm <= Math.Max(a.XMm, b.XMm) &&
        Math.Min(a.YMm, b.YMm) <= p.YMm && p.YMm <= Math.Max(a.YMm, b.YMm);

    /// <summary>Closed-segment intersection test, including touching endpoints and collinear overlap.</summary>
    public static bool SegmentsIntersect(Vertex a1, Vertex a2, Vertex b1, Vertex b2)
    {
        int d1 = Sign(Cross(a1, a2, b1));
        int d2 = Sign(Cross(a1, a2, b2));
        int d3 = Sign(Cross(b1, b2, a1));
        int d4 = Sign(Cross(b1, b2, a2));
        if (d1 * d2 < 0 && d3 * d4 < 0) return true;
        if (d1 == 0 && OnSegment(a1, a2, b1)) return true;
        if (d2 == 0 && OnSegment(a1, a2, b2)) return true;
        if (d3 == 0 && OnSegment(b1, b2, a1)) return true;
        if (d4 == 0 && OnSegment(b1, b2, a2)) return true;
        return false;
    }

    /// <summary>Twice the signed shoelace area (mm²). Sign depends on winding.</summary>
    public static Int128 SignedArea2(IReadOnlyList<Vertex> poly)
    {
        Int128 sum = 0;
        for (int i = 0; i < poly.Count; i++)
        {
            var a = poly[i];
            var b = poly[(i + 1) % poly.Count];
            sum += (Int128)a.XMm * b.YMm - (Int128)b.XMm * a.YMm;
        }
        return sum;
    }

    /// <summary>Exact area in m² (twice-area mm² divided by 2,000,000 — a terminating decimal).</summary>
    public static decimal AreaM2(IReadOnlyList<Vertex> poly)
    {
        Int128 abs = SignedArea2(poly);
        if (abs < 0) abs = -abs;
        return (decimal)abs / 2_000_000m;
    }

    /// <summary>Returns an error message when the ring is not a valid P0 boundary; null when valid.</summary>
    public static string? ValidatePolygon(IReadOnlyList<Vertex> poly)
    {
        if (poly.Count < 3) return "polygon needs at least 3 vertices";
        if (poly.Count > LabConstants.MaxPolygonVertices) return $"polygon has more than {LabConstants.MaxPolygonVertices} vertices";
        foreach (var v in poly)
        {
            if (Math.Abs(v.XMm) > LabConstants.MaxMm || Math.Abs(v.YMm) > LabConstants.MaxMm)
                return "polygon coordinate outside the supported integer-millimetre range";
        }
        var seen = new HashSet<Vertex>();
        foreach (var v in poly)
            if (!seen.Add(v)) return "polygon repeats a vertex (closing vertex must not be repeated)";
        if (SignedArea2(poly) == 0) return "polygon has zero area (collinear vertices)";

        int n = poly.Count;
        for (int i = 0; i < n; i++)
        {
            // Adjacent edges i and i+1 share a vertex; reject a 180-degree reversal (overlap along the shared line).
            var p0 = poly[i];
            var p1 = poly[(i + 1) % n];
            var p2 = poly[(i + 2) % n];
            if (Cross(p0, p1, p2) == 0)
            {
                Int128 dot = (Int128)(p1.XMm - p0.XMm) * (p2.XMm - p1.XMm) + (Int128)(p1.YMm - p0.YMm) * (p2.YMm - p1.YMm);
                if (dot < 0) return "polygon edges fold back on themselves";
            }
        }
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                bool adjacent = j == i + 1 || (i == 0 && j == n - 1);
                if (adjacent) continue;
                if (SegmentsIntersect(poly[i], poly[(i + 1) % n], poly[j], poly[(j + 1) % n]))
                    return "polygon edges intersect or touch (self-intersecting / bow-tie ring)";
            }
        }
        return null;
    }

    /// <summary>Inclusive point-in-polygon test (a point exactly on an edge counts as inside). Assumes a valid simple ring.</summary>
    public static bool ContainsInclusive(IReadOnlyList<Vertex> poly, long xMm, long yMm)
    {
        var p = new Vertex(xMm, yMm);
        int n = poly.Count;
        for (int i = 0; i < n; i++)
            if (OnSegment(poly[i], poly[(i + 1) % n], p)) return true;

        bool inside = false;
        for (int i = 0; i < n; i++)
        {
            var a = poly[i];
            var b = poly[(i + 1) % n];
            if ((a.YMm > yMm) != (b.YMm > yMm))
            {
                Int128 lhs = (Int128)(xMm - a.XMm) * (b.YMm - a.YMm);
                Int128 rhs = (Int128)(b.XMm - a.XMm) * (yMm - a.YMm);
                bool left = (b.YMm - a.YMm) > 0 ? lhs < rhs : lhs > rhs;
                if (left) inside = !inside;
            }
        }
        return inside;
    }
}
