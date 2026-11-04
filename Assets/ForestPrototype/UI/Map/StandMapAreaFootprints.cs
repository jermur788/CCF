using System;
using System.Collections.Generic;

// The world footprint of every Treatment review area, derived once from the same authoritative inputs coverage itself uses. Plain data,
// no UnityEngine, so it is checked offline for axis-aligned and rotated rack plans.
//
// Coverage (ScenarioOneCoverage) splits each rack-served unit along the common rack axis at the sample anchor +/- SegmentHalfLengthM into
// Low / Centre / High, using along = x*AxisX + z*AxisZ - anchorAlong. This class cuts the unit's own polygon with exactly those lines, so a
// segment footprint is the part of the unit strip that coverage assigns to that segment. The source sample is the fixed teaching footprint
// (Result.Teaching), never the whole strip. Remainders keep their authoritative remainder polygons.
//
// The map layer, its printed tags and Inspect in forest all read one Result, so they cannot disagree about where an area is.
public sealed class StandMapAreaFootprints
{
    private readonly Dictionary<string, List<RackPoint>> polygons = new Dictionary<string, List<RackPoint>>(StringComparer.Ordinal);
    public bool Valid { get; private set; }
    public string Problem { get; private set; } = "";

    public IReadOnlyList<RackPoint> PolygonOf(string areaId)
        => !string.IsNullOrEmpty(areaId) && polygons.TryGetValue(areaId, out List<RackPoint> p) ? p : null;

    // Area-weighted centre of the footprint (vertex mean if it is degenerate). Used for the waypoint and the printed tag.
    public bool TryCentre(string areaId, out float x, out float z)
    {
        x = z = 0f;
        IReadOnlyList<RackPoint> p = PolygonOf(areaId);
        if (p == null || p.Count < 3) return false;
        Centroid(p, out x, out z);
        return true;
    }

    public static StandMapAreaFootprints Derive(ScenarioOneCoverage.Result coverage, ScenarioOneRackSampleGeometry.Result geometry)
    {
        var result = new StandMapAreaFootprints();
        if (coverage == null || !coverage.Valid || coverage.Calibration == null) { result.Problem = "coverage is not valid"; return result; }
        if (geometry == null || !geometry.Valid) { result.Problem = "rack/sample geometry is not valid"; return result; }
        double axisX = geometry.AxisX, axisZ = geometry.AxisZ;
        double anchorAlong = geometry.Anchor.X * axisX + geometry.Anchor.Z * axisZ;
        double half = coverage.Calibration.SegmentHalfLengthM;

        foreach (ScenarioOneCoverage.Area area in coverage.Areas)
        {
            List<RackPoint> polygon = null;
            if (area.IsSource) polygon = TeachingPolygon(geometry.Teaching);
            else
            {
                ScenarioOneRackSampleGeometry.CoverageUnit unit = null;
                foreach (ScenarioOneRackSampleGeometry.CoverageUnit u in geometry.Units)
                    if (u.Index == area.UnitIndex && u.RackId == area.RackId) unit = u;
                if (unit != null && unit.Polygon != null && unit.Polygon.Count >= 3)
                {
                    double low = double.NegativeInfinity, high = double.PositiveInfinity;
                    if (area.Segment == ScenarioOneCoverage.Segment.Low) high = anchorAlong - half;
                    else if (area.Segment == ScenarioOneCoverage.Segment.Centre) { low = anchorAlong - half; high = anchorAlong + half; }
                    else low = anchorAlong + half;
                    polygon = ClipAlong(unit.Polygon, axisX, axisZ, low, high);
                }
            }
            if (polygon != null && polygon.Count >= 3 && Math.Abs(SignedArea(polygon)) > 1e-3) result.polygons[area.Id] = polygon;
        }
        foreach (ScenarioOneCoverage.Remainder r in coverage.Remainders)
        {
            ScenarioOneRackSampleGeometry.OuterRemainder shape = r.Side == ScenarioOneRackSampleGeometry.Side.Right ? geometry.RightRemainder : geometry.LeftRemainder;
            if (shape != null && !shape.IsEmpty && shape.Polygon != null && shape.Polygon.Count >= 3) result.polygons[r.Id] = new List<RackPoint>(shape.Polygon);
        }
        result.Valid = true;
        return result;
    }

    // The fixed teaching footprint: +/- HalfLengthM along the rack, +/- HalfWidth across it, about the anchor.
    public static List<RackPoint> TeachingPolygon(ScenarioOneRackBay.Frame frame)
    {
        float l = ScenarioOneRackBay.HalfLengthM, w = frame.HalfWidth;
        float nx = -frame.AlongZ, nz = frame.AlongX;
        return new List<RackPoint>
        {
            new RackPoint(frame.X - frame.AlongX * l - nx * w, frame.Z - frame.AlongZ * l - nz * w),
            new RackPoint(frame.X + frame.AlongX * l - nx * w, frame.Z + frame.AlongZ * l - nz * w),
            new RackPoint(frame.X + frame.AlongX * l + nx * w, frame.Z + frame.AlongZ * l + nz * w),
            new RackPoint(frame.X - frame.AlongX * l + nx * w, frame.Z - frame.AlongZ * l + nz * w)
        };
    }

    // Keep the part of a convex polygon whose along coordinate lies in [low, high] (Sutherland-Hodgman, one half-plane at a time).
    public static List<RackPoint> ClipAlong(IReadOnlyList<RackPoint> polygon, double axisX, double axisZ, double low, double high)
    {
        var current = new List<RackPoint>(polygon);
        if (!double.IsNegativeInfinity(low)) current = ClipHalfPlane(current, axisX, axisZ, low, true);
        if (!double.IsPositiveInfinity(high)) current = ClipHalfPlane(current, axisX, axisZ, high, false);
        return current;
    }

    private static List<RackPoint> ClipHalfPlane(List<RackPoint> input, double ax, double az, double limit, bool keepAbove)
    {
        var output = new List<RackPoint>();
        if (input.Count == 0) return output;
        Func<RackPoint, double> signed = p => (keepAbove ? 1.0 : -1.0) * (p.X * ax + p.Z * az - limit);
        for (int i = 0; i < input.Count; i++)
        {
            RackPoint a = input[i], b = input[(i + 1) % input.Count];
            double da = signed(a), db = signed(b);
            if (da >= 0) output.Add(a);
            if ((da >= 0) != (db >= 0))
            {
                double t = da / (da - db);
                output.Add(new RackPoint((float)(a.X + (b.X - a.X) * t), (float)(a.Z + (b.Z - a.Z) * t)));
            }
        }
        return output;
    }

    public static double SignedArea(IReadOnlyList<RackPoint> p)
    {
        double s = 0;
        for (int i = 0; i < p.Count; i++) { RackPoint a = p[i], b = p[(i + 1) % p.Count]; s += (double)a.X * b.Z - (double)b.X * a.Z; }
        return s * 0.5;
    }

    public static void Centroid(IReadOnlyList<RackPoint> p, out float x, out float z)
    {
        double area = SignedArea(p), cx = 0, cz = 0;
        if (Math.Abs(area) < 1e-6)
        {
            foreach (RackPoint q in p) { cx += q.X; cz += q.Z; }
            x = (float)(cx / p.Count); z = (float)(cz / p.Count);
            return;
        }
        for (int i = 0; i < p.Count; i++)
        {
            RackPoint a = p[i], b = p[(i + 1) % p.Count];
            double cross = (double)a.X * b.Z - (double)b.X * a.Z;
            cx += (a.X + b.X) * cross; cz += (a.Z + b.Z) * cross;
        }
        x = (float)(cx / (6.0 * area)); z = (float)(cz / (6.0 * area));
    }
}
