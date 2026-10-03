using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Fence geometry helper (Scenario 1 stretch; no fencing gameplay). Builds the
// ecological BrowseProtectedArea record a future fencing work order would
// create: an axis-aligned rectangle around a planting group with a margin,
// clipped to the property (stand) bounds. Geometry only; no cost or labour.
public static class BrowseProtectionGeometry
{
    // [S] Clearance between the outermost protected stem and the fence line.
    public const float DefaultMarginM = 2f;

    // Counter-clockwise rectangle (x, z) around the points plus margin, clipped
    // to the property. Empty if there are no points or the clip leaves no area.
    public static List<Vector2> RectangleAround(IEnumerable<Vector2> points, Rect property, float marginM = DefaultMarginM)
    {
        List<Vector2> list = points?.ToList() ?? new List<Vector2>();
        if (list.Count == 0)
            return new List<Vector2>();
        float margin = Mathf.Max(0f, marginM);
        float xMin = Mathf.Max(property.xMin, list.Min(p => p.x) - margin);
        float xMax = Mathf.Min(property.xMax, list.Max(p => p.x) + margin);
        float zMin = Mathf.Max(property.yMin, list.Min(p => p.y) - margin);
        float zMax = Mathf.Min(property.yMax, list.Max(p => p.y) + margin);
        if (xMax <= xMin || zMax <= zMin)
            return new List<Vector2>();
        return new List<Vector2> { new Vector2(xMin, zMin), new Vector2(xMax, zMin), new Vector2(xMax, zMax), new Vector2(xMin, zMax) };
    }

    // Fence length in metres along the closed polygon.
    public static float PerimeterM(IReadOnlyList<Vector2> polygon)
    {
        if (polygon == null || polygon.Count < 2)
            return 0f;
        float total = 0f;
        for (int i = 0; i < polygon.Count; i++)
            total += Vector2.Distance(polygon[i], polygon[(i + 1) % polygon.Count]);
        return total;
    }

    public static float AreaM2(IReadOnlyList<Vector2> polygon)
    {
        if (polygon == null || polygon.Count < 3)
            return 0f;
        float twice = 0f;
        for (int i = 0; i < polygon.Count; i++)
        {
            Vector2 a = polygon[i], b = polygon[(i + 1) % polygon.Count];
            twice += a.x * b.y - b.x * a.y;
        }
        return Mathf.Abs(twice) * 0.5f;
    }

    // The ecological record a fencing job would add (installedYear per the
    // shelter timing contract: the resolution year, report.year).
    public static BrowseProtectedArea AreaAround(string areaId, IEnumerable<Vector2> points, Rect property, int installedYear,
        float marginM = DefaultMarginM)
        => new BrowseProtectedArea
        {
            areaId = areaId ?? "",
            polygon = RectangleAround(points, property, marginM),
            installedYear = installedYear,
            breachedYear = -1
        };
}
