namespace SitkaP1.Core.WoodlandImport;

/// <summary>Coordinate origin system from which a <see cref="WoodlandBoundary"/> was constructed.</summary>
public enum BoundaryCoordinateSource
{
    LocalMetres = 0,
    GeographicTangentPlane = 1,
}

/// <summary>
/// Surveyed woodland boundary polygon in local metres (+X east, +Y north).
/// Area is computed with the exact shoelace formula (no bounding-box approximation).
/// Self-intersecting, &lt;3-vertex, non-finite, or zero-area polygons are refused.
/// </summary>
public sealed record WoodlandBoundary
{
    /// <summary>WGS-84 mean spherical Earth radius in metres used for local tangent-plane projection.</summary>
    public const double EarthMeanRadiusM = 6_371_008.8;

    /// <summary>Tolerance in metres for boundary-inclusive edge containment and collinearity checks.</summary>
    public const double EdgeEpsilonM = 1e-9;

    /// <summary>Minimum polygon area in m² required to be considered non-degenerate.</summary>
    public const double MinAreaM2 = 1e-9;

    public required IReadOnlyList<LocalPoint2D> VerticesLocalM { get; init; }
    public required double AreaM2 { get; init; }
    public required double AreaHa { get; init; }
    public required BoundaryCoordinateSource CoordinateSource { get; init; }
    public GeoPoint2D? GeographicCentroid { get; init; }
    public required string ProjectionNote { get; init; }

    /// <summary>
    /// Validates and constructs a <see cref="WoodlandBoundary"/> from local metre vertices.
    /// If the closing vertex repeats the first vertex, the duplicate closing vertex is stripped.
    /// </summary>
    public static bool TryCreateLocal(
        IReadOnlyList<LocalPoint2D>? vertices,
        out WoodlandBoundary? boundary,
        out IReadOnlyList<ImportValidationIssue> issues,
        BoundaryCoordinateSource source = BoundaryCoordinateSource.LocalMetres,
        GeoPoint2D? geographicCentroid = null,
        string? projectionNote = null)
    {
        var list = new List<ImportValidationIssue>();
        boundary = null;

        if (vertices is null || vertices.Count == 0)
        {
            list.Add(new ImportValidationIssue(
                "BoundaryTooFewVertices",
                IssueSeverity.InventoryOnlyBlocker,
                "Surveyed boundary polygon must contain at least 3 vertices.",
                FieldName: "Boundary"));
            issues = list;
            return false;
        }

        // Check finiteness first before any geometric arithmetic.
        for (int i = 0; i < vertices.Count; i++)
        {
            if (!vertices[i].IsFinite)
            {
                list.Add(new ImportValidationIssue(
                    "BoundaryNonFiniteVertex",
                    IssueSeverity.InventoryOnlyBlocker,
                    $"Boundary vertex index {i} contains a non-finite coordinate.",
                    FieldName: "Boundary"));
                issues = list;
                return false;
            }
        }

        // Copy and strip an explicit closing vertex if last == first.
        var ring = new List<LocalPoint2D>(vertices.Count);
        for (int i = 0; i < vertices.Count; i++)
        {
            ring.Add(vertices[i]);
        }

        if (ring.Count >= 2 && PointsCoincide(ring[0], ring[^1]))
        {
            ring.RemoveAt(ring.Count - 1);
        }

        if (ring.Count < 3)
        {
            list.Add(new ImportValidationIssue(
                "BoundaryTooFewVertices",
                IssueSeverity.InventoryOnlyBlocker,
                $"Surveyed boundary polygon has only {ring.Count} distinct ring vertices; at least 3 are required.",
                FieldName: "Boundary"));
            issues = list;
            return false;
        }

        // Reject consecutive coincident vertices (zero-length edges).
        for (int i = 0; i < ring.Count; i++)
        {
            LocalPoint2D a = ring[i];
            LocalPoint2D b = ring[(i + 1) % ring.Count];
            if (PointsCoincide(a, b))
            {
                list.Add(new ImportValidationIssue(
                    "BoundaryDegenerateEdge",
                    IssueSeverity.InventoryOnlyBlocker,
                    $"Boundary edge {i} has zero length (duplicate consecutive vertices).",
                    FieldName: "Boundary"));
                issues = list;
                return false;
            }
        }

        // Reject collinear degenerate rings before self-intersection checks so a collinear line reports BoundaryZeroArea
        // while a symmetric figure-8 / bowtie reports BoundarySelfIntersecting.
        if (AreAllVerticesCollinear(ring))
        {
            list.Add(new ImportValidationIssue(
                "BoundaryZeroArea",
                IssueSeverity.InventoryOnlyBlocker,
                "Surveyed boundary polygon vertices are collinear (zero area).",
                FieldName: "Boundary"));
            issues = list;
            return false;
        }

        // Reject self-intersecting polygons (any non-adjacent edge pair intersecting or touching).
        if (HasSelfIntersection(ring))
        {
            list.Add(new ImportValidationIssue(
                "BoundarySelfIntersecting",
                IssueSeverity.InventoryOnlyBlocker,
                "Surveyed boundary polygon is self-intersecting.",
                FieldName: "Boundary"));
            issues = list;
            return false;
        }

        double areaM2 = ComputeShoelaceAreaM2(ring);
        if (!double.IsFinite(areaM2) || areaM2 <= MinAreaM2)
        {
            list.Add(new ImportValidationIssue(
                "BoundaryZeroArea",
                IssueSeverity.InventoryOnlyBlocker,
                "Surveyed boundary polygon has zero or degenerate area.",
                FieldName: "Boundary"));
            issues = list;
            return false;
        }

        boundary = new WoodlandBoundary
        {
            VerticesLocalM = ring.AsReadOnly(),
            AreaM2 = areaM2,
            AreaHa = areaM2 / 10_000.0,
            CoordinateSource = source,
            GeographicCentroid = geographicCentroid,
            ProjectionNote = projectionNote ?? "Local surveyed planar metres (no geographic projection applied).",
        };
        issues = list;
        return true;
    }

    /// <summary>
    /// Projects WGS-84 geographic vertices to local metres on a documented local tangent plane centred at
    /// the arithmetic centroid of the polygon ring (longitude scaled by cos(centroid latitude)), then validates
    /// the resulting planar polygon.
    /// </summary>
    public static bool TryCreateFromGeographic(
        IReadOnlyList<GeoPoint2D>? geoVertices,
        out WoodlandBoundary? boundary,
        out IReadOnlyList<ImportValidationIssue> issues)
    {
        var list = new List<ImportValidationIssue>();
        boundary = null;

        if (geoVertices is null || geoVertices.Count == 0)
        {
            list.Add(new ImportValidationIssue(
                "BoundaryTooFewVertices",
                IssueSeverity.InventoryOnlyBlocker,
                "Geographic boundary polygon must contain at least 3 vertices.",
                FieldName: "Boundary"));
            issues = list;
            return false;
        }

        for (int i = 0; i < geoVertices.Count; i++)
        {
            GeoPoint2D g = geoVertices[i];
            if (!g.IsFinite || g.LatitudeDeg < -89.999 || g.LatitudeDeg > 89.999 ||
                g.LongitudeDeg < -180.0 || g.LongitudeDeg > 180.0)
            {
                list.Add(new ImportValidationIssue(
                    "BoundaryNonFiniteVertex",
                    IssueSeverity.InventoryOnlyBlocker,
                    $"Geographic boundary vertex index {i} is non-finite or out of WGS-84 range.",
                    FieldName: "Boundary"));
                issues = list;
                return false;
            }
        }

        // Strip closing vertex before computing centroid so the closing vertex is not double-weighted.
        int ringCount = geoVertices.Count;
        if (ringCount >= 2 &&
            Math.Abs(geoVertices[0].LatitudeDeg - geoVertices[^1].LatitudeDeg) <= 1e-12 &&
            Math.Abs(geoVertices[0].LongitudeDeg - geoVertices[^1].LongitudeDeg) <= 1e-12)
        {
            ringCount--;
        }

        if (ringCount < 3)
        {
            list.Add(new ImportValidationIssue(
                "BoundaryTooFewVertices",
                IssueSeverity.InventoryOnlyBlocker,
                $"Geographic boundary polygon has only {ringCount} distinct ring vertices; at least 3 are required.",
                FieldName: "Boundary"));
            issues = list;
            return false;
        }

        double sumLat = 0.0;
        double sumLon = 0.0;
        for (int i = 0; i < ringCount; i++)
        {
            sumLat += geoVertices[i].LatitudeDeg;
            sumLon += geoVertices[i].LongitudeDeg;
        }

        var centroid = new GeoPoint2D(sumLat / ringCount, sumLon / ringCount);
        var projected = new LocalPoint2D[ringCount];
        for (int i = 0; i < ringCount; i++)
        {
            projected[i] = ProjectGeographicToLocalTangentPlane(geoVertices[i], centroid);
        }

        string note = string.Create(
            System.Globalization.CultureInfo.InvariantCulture,
            $"Local tangent-plane approximation at centroid ({centroid.LatitudeDeg:F6}, {centroid.LongitudeDeg:F6}) using R={EarthMeanRadiusM:F1} m and longitude scaled by cos(centroid_lat). Suitable for stand-scale woodland plots; not an ellipsoidal UTM projection.");

        return TryCreateLocal(
            projected,
            out boundary,
            out issues,
            source: BoundaryCoordinateSource.GeographicTangentPlane,
            geographicCentroid: centroid,
            projectionNote: note);
    }

    /// <summary>
    /// Projects a geographic WGS-84 point to local metres relative to <paramref name="centroid"/>
    /// on the documented local tangent plane (longitude scaled by cos(centroid latitude)).
    /// </summary>
    public static LocalPoint2D ProjectGeographicToLocalTangentPlane(GeoPoint2D point, GeoPoint2D centroid)
    {
        const double degToRad = Math.PI / 180.0;
        double metersPerDeg = EarthMeanRadiusM * degToRad;
        double cosLat0 = Math.Cos(centroid.LatitudeDeg * degToRad);
        double xM = (point.LongitudeDeg - centroid.LongitudeDeg) * cosLat0 * metersPerDeg;
        double yM = (point.LatitudeDeg - centroid.LatitudeDeg) * metersPerDeg;
        return new LocalPoint2D(xM, yM);
    }

    /// <summary>Computes exact planar polygon area in m² via the shoelace formula.</summary>
    public static double ComputeShoelaceAreaM2(IReadOnlyList<LocalPoint2D> ring)
    {
        if (ring.Count < 3) return 0.0;
        double twiceSigned = 0.0;
        int n = ring.Count;
        for (int i = 0; i < n; i++)
        {
            LocalPoint2D a = ring[i];
            LocalPoint2D b = ring[(i + 1) % n];
            twiceSigned += (a.XM * b.YM) - (b.XM * a.YM);
        }
        return Math.Abs(twiceSigned) * 0.5;
    }

    /// <summary>
    /// Point-in-polygon test using ray casting with a documented boundary-inclusive edge rule:
    /// points lying on any polygon edge or vertex (within <see cref="EdgeEpsilonM"/>) return <c>true</c>;
    /// strictly interior points return <c>true</c>; exterior or non-finite points return <c>false</c>.
    /// </summary>
    public bool Contains(LocalPoint2D point)
    {
        if (!point.IsFinite) return false;
        var ring = VerticesLocalM;
        int n = ring.Count;
        if (n < 3) return false;

        // 1. Documented edge rule: boundary-inclusive.
        for (int i = 0; i < n; i++)
        {
            if (IsPointOnSegment(point, ring[i], ring[(i + 1) % n]))
            {
                return true;
            }
        }

        // 2. Ray casting to +infinity along +X with half-open Y interval.
        bool inside = false;
        for (int i = 0, j = n - 1; i < n; j = i++)
        {
            double xi = ring[i].XM;
            double yi = ring[i].YM;
            double xj = ring[j].XM;
            double yj = ring[j].YM;

            bool straddles = (yi > point.YM) != (yj > point.YM);
            if (straddles)
            {
                double xIntersect = ((xj - xi) * (point.YM - yi) / (yj - yi)) + xi;
                if (point.XM < xIntersect)
                {
                    inside = !inside;
                }
            }
        }

        return inside;
    }

    /// <summary>Returns a translated copy of this boundary without modifying area, shape, or distances.</summary>
    public WoodlandBoundary Translate(double dxM, double dyM)
    {
        var moved = new LocalPoint2D[VerticesLocalM.Count];
        for (int i = 0; i < VerticesLocalM.Count; i++)
        {
            moved[i] = new LocalPoint2D(VerticesLocalM[i].XM + dxM, VerticesLocalM[i].YM + dyM);
        }

        return this with
        {
            VerticesLocalM = Array.AsReadOnly(moved),
        };
    }

    private static bool PointsCoincide(LocalPoint2D a, LocalPoint2D b) =>
        Math.Abs(a.XM - b.XM) <= EdgeEpsilonM && Math.Abs(a.YM - b.YM) <= EdgeEpsilonM;

    private static bool IsPointOnSegment(LocalPoint2D p, LocalPoint2D a, LocalPoint2D b)
    {
        double dx = b.XM - a.XM;
        double dy = b.YM - a.YM;
        double segLen = Math.Sqrt((dx * dx) + (dy * dy));
        if (segLen <= EdgeEpsilonM)
        {
            return PointsCoincide(p, a);
        }

        // Perpendicular distance from p to line ab must be <= EdgeEpsilonM.
        double cross = Math.Abs(((p.XM - a.XM) * dy) - ((p.YM - a.YM) * dx));
        if (cross > EdgeEpsilonM * segLen)
        {
            return false;
        }

        double minX = Math.Min(a.XM, b.XM) - EdgeEpsilonM;
        double maxX = Math.Max(a.XM, b.XM) + EdgeEpsilonM;
        double minY = Math.Min(a.YM, b.YM) - EdgeEpsilonM;
        double maxY = Math.Max(a.YM, b.YM) + EdgeEpsilonM;
        return p.XM >= minX && p.XM <= maxX && p.YM >= minY && p.YM <= maxY;
    }

    private static bool AreAllVerticesCollinear(IReadOnlyList<LocalPoint2D> ring)
    {
        if (ring.Count < 3) return true;
        LocalPoint2D a = ring[0];
        LocalPoint2D b = ring[1];
        double dx = b.XM - a.XM;
        double dy = b.YM - a.YM;
        double len = Math.Sqrt((dx * dx) + (dy * dy));
        if (len <= EdgeEpsilonM) return true;

        for (int i = 2; i < ring.Count; i++)
        {
            double cross = Math.Abs(Cross(a, b, ring[i]));
            if (cross > EdgeEpsilonM * len)
            {
                return false;
            }
        }
        return true;
    }

    private static bool HasSelfIntersection(IReadOnlyList<LocalPoint2D> ring)
    {
        int n = ring.Count;
        for (int i = 0; i < n; i++)
        {
            LocalPoint2D a1 = ring[i];
            LocalPoint2D a2 = ring[(i + 1) % n];

            for (int j = i + 1; j < n; j++)
            {
                // Skip adjacent edges (they share an endpoint by definition).
                if (j == i || j == (i + 1) % n || (j + 1) % n == i)
                {
                    continue;
                }

                LocalPoint2D b1 = ring[j];
                LocalPoint2D b2 = ring[(j + 1) % n];
                if (SegmentsIntersect(a1, a2, b1, b2))
                {
                    return true;
                }
            }
        }

        // Also check adjacent collinear fold-back (180-degree reversal overlapping prior edge).
        for (int i = 0; i < n; i++)
        {
            LocalPoint2D prev = ring[(i + n - 1) % n];
            LocalPoint2D curr = ring[i];
            LocalPoint2D next = ring[(i + 1) % n];
            double cross = Cross(prev, curr, next);
            if (Math.Abs(cross) <= EdgeEpsilonM)
            {
                double dot = ((prev.XM - curr.XM) * (next.XM - curr.XM)) +
                             ((prev.YM - curr.YM) * (next.YM - curr.YM));
                if (dot > 0.0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool SegmentsIntersect(LocalPoint2D a1, LocalPoint2D a2, LocalPoint2D b1, LocalPoint2D b2)
    {
        int o1 = Orientation(a1, a2, b1);
        int o2 = Orientation(a1, a2, b2);
        int o3 = Orientation(b1, b2, a1);
        int o4 = Orientation(b1, b2, a2);

        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        if (o1 == 0 && IsPointOnSegment(b1, a1, a2)) return true;
        if (o2 == 0 && IsPointOnSegment(b2, a1, a2)) return true;
        if (o3 == 0 && IsPointOnSegment(a1, b1, b2)) return true;
        if (o4 == 0 && IsPointOnSegment(a2, b1, b2)) return true;

        return false;
    }

    private static int Orientation(LocalPoint2D a, LocalPoint2D b, LocalPoint2D c)
    {
        double val = Cross(a, b, c);
        if (Math.Abs(val) <= EdgeEpsilonM) return 0;
        return val > 0.0 ? 1 : -1;
    }

    private static double Cross(LocalPoint2D a, LocalPoint2D b, LocalPoint2D c) =>
        ((b.XM - a.XM) * (c.YM - a.YM)) - ((b.YM - a.YM) * (c.XM - a.XM));
}
