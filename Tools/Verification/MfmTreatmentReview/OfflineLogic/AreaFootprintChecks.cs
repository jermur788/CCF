// MFM-08 Geometry Repair 01: per-area footprints for the Treatment review map and Inspect in forest, axis-aligned and rotated.
// Pure: runs StandMapAreaFootprints on the real ScenarioOneRackSampleGeometry for an east-west plan and rotated plans, against independent
// recomputation of the accepted coverage rule (along = x*AxisX + z*AxisZ - anchorAlong, cut at +/- SegmentHalfLengthM).
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cov = ScenarioOneCoverage;
using Geo = ScenarioOneRackSampleGeometry;

public static class AreaFootprintChecks
{
    private static void Check(bool ok, string name) => TreatmentReviewOfflineCheck.Check(ok, name);
    private static readonly float[] RackZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private const float EntranceX = -35.4f, FarX = 40f, Half = 40f, Eps = 1e-2f;

    private static List<Geo.RackInput> Racks(double degrees, float shiftX = 0f, float shiftZ = 0f)
    {
        double a = degrees * Math.PI / 180.0, c = Math.Cos(a), s = Math.Sin(a);
        RackPoint Turn(float x, float z) => new RackPoint((float)(x * c - z * s) + shiftX, (float)(x * s + z * c) + shiftZ);
        var list = new List<Geo.RackInput>();
        for (int i = 0; i < RackZ.Length; i++) list.Add(new Geo.RackInput(i + 1, Turn(EntranceX, RackZ[i]), Turn(FarX, RackZ[i]), RackGeometry.DefaultWidthM));
        return list;
    }

    // A coverage result shaped exactly like production's: every unit x Low/Centre/High, the teaching unit's Centre is the source, both remainders.
    private static Cov.Result Coverage(Geo.Result g)
    {
        var r = new Cov.Result { Valid = true, Calibration = ScenarioOnePrescriptionCalibration.V1 };
        foreach (Geo.CoverageUnit u in g.Units)
            foreach (Cov.Segment seg in new[] { Cov.Segment.Low, Cov.Segment.Centre, Cov.Segment.High })
                r.Areas.Add(new Cov.Area
                {
                    Id = Cov.AreaId(u.RackId, seg), UnitIndex = u.Index, RackId = u.RackId, Segment = seg,
                    State = Cov.State.Covered, IsSource = u.Index == g.TeachingRack.Index && seg == Cov.Segment.Centre
                });
        r.Remainders.Add(new Cov.Remainder { Id = "remainder-right", Side = Geo.Side.Right });
        r.Remainders.Add(new Cov.Remainder { Id = "remainder-left", Side = Geo.Side.Left });
        return r;
    }

    private static double Along(Geo.Result g, RackPoint p) => p.X * (double)g.AxisX + p.Z * (double)g.AxisZ;
    private static double Area(IReadOnlyList<RackPoint> p) => Math.Abs(StandMapAreaFootprints.SignedArea(p));
    private static string Key(IReadOnlyList<RackPoint> p) => string.Join(";", p.Select(q => q.X.ToString("0.00") + "," + q.Z.ToString("0.00")).OrderBy(x => x, StringComparer.Ordinal));

    // What d55ec86a did: every area of a unit takes the whole-unit polygon, chosen by UnitIndex + RackId only.
    private static Dictionary<string, IReadOnlyList<RackPoint>> OldDerivation(Cov.Result cov, Geo.Result g)
    {
        var map = new Dictionary<string, IReadOnlyList<RackPoint>>();
        foreach (Cov.Area a in cov.Areas)
            foreach (Geo.CoverageUnit u in g.Units)
                if (u.Index == a.UnitIndex && u.RackId == a.RackId) map[a.Id] = u.Polygon;
        return map;
    }

    private static bool SegmentsDistinct(Cov.Result cov, Geo.Result g, Func<string, IReadOnlyList<RackPoint>> polygonOf, out string detail)
    {
        detail = "";
        foreach (Geo.CoverageUnit u in g.Units)
        {
            var keys = new List<string>();
            foreach (Cov.Area a in cov.Areas.Where(x => x.UnitIndex == u.Index && x.RackId == u.RackId))
            {
                IReadOnlyList<RackPoint> p = polygonOf(a.Id);
                if (p == null) { detail = "no polygon for " + a.Id; return false; }
                keys.Add(Key(p));
            }
            if (keys.Distinct().Count() != keys.Count) { detail = "rack " + u.RackId + " shares one polygon"; return false; }
        }
        return true;
    }

    private static void Plan(string label, IReadOnlyList<Geo.RackInput> racks)
    {
        Geo.Result g = Geo.Derive(racks, Half);
        Check(g.Valid, label + ": rack/sample geometry valid " + g.Problem);
        if (!g.Valid) return;
        Cov.Result cov = Coverage(g);
        StandMapAreaFootprints fp = StandMapAreaFootprints.Derive(cov, g);
        Check(fp.Valid, label + ": footprints derived");
        double anchorAlong = Along(g, g.Anchor);
        double s = cov.Calibration.SegmentHalfLengthM;

        // 1. Every area has its own footprint; Low / Centre / High of one unit are never one shared polygon.
        Check(cov.Areas.All(a => fp.PolygonOf(a.Id) != null && fp.PolygonOf(a.Id).Count >= 3), label + ": every area has a polygon (" + cov.Areas.Count + " areas)");
        Check(SegmentsDistinct(cov, g, fp.PolygonOf, out string d1), label + ": Low / Centre / High of every rack unit are distinct footprints " + d1);
        // Negative control: the d55ec86a derivation fails the very same predicate.
        var old = OldDerivation(cov, g);
        Check(!SegmentsDistinct(cov, g, id => old[id], out string d0), label + ": CONTROL - the d55ec86a whole-unit derivation fails the distinctness check (" + d0 + ")");

        // 2. Segment polygons respect the authoritative along-axis boundaries and tile the unit.
        foreach (Geo.CoverageUnit u in g.Units)
        {
            bool bounds = true, tiles = true, coverageRule = true;
            double sum = 0;
            foreach (Cov.Segment seg in new[] { Cov.Segment.Low, Cov.Segment.Centre, Cov.Segment.High })
            {
                Cov.Area a = cov.Areas.First(x => x.UnitIndex == u.Index && x.RackId == u.RackId && x.Segment == seg);
                double lo = seg == Cov.Segment.Low ? double.NegativeInfinity : seg == Cov.Segment.Centre ? anchorAlong - s : anchorAlong + s;
                double hi = seg == Cov.Segment.Low ? anchorAlong - s : seg == Cov.Segment.Centre ? anchorAlong + s : double.PositiveInfinity;
                List<RackPoint> independent = StandMapAreaFootprints.ClipAlong(u.Polygon, g.AxisX, g.AxisZ, lo, hi);
                sum += Area(independent);
                if (a.IsSource) continue; // the source is the teaching footprint, checked below
                IReadOnlyList<RackPoint> p = fp.PolygonOf(a.Id);
                if (p == null) continue;
                foreach (RackPoint q in p) if (Along(g, q) < lo - Eps || Along(g, q) > hi + Eps) bounds = false;
                if (Math.Abs(Area(p) - Area(independent)) > 0.05) tiles = false;
                // the segment the coverage rule would give the footprint's own centre
                fp.TryCentre(a.Id, out float cx, out float cz);
                double along = Along(g, new RackPoint(cx, cz)) - anchorAlong;
                int rule = along < -s ? 0 : along < s ? 1 : 2;
                if (rule != (int)seg) coverageRule = false;
            }
            if (Math.Abs(sum - Area(u.Polygon)) > 0.05) tiles = false;
            Check(bounds, label + ": rack " + u.RackId + " segment vertices stay inside their along-axis boundaries (anchor " + anchorAlong.ToString("0.00") + " +/- " + s.ToString("0.0") + ")");
            Check(tiles, label + ": rack " + u.RackId + " segments tile the unit strip exactly (" + Area(u.Polygon).ToString("0.0") + " m2)");
            Check(coverageRule, label + ": rack " + u.RackId + " each footprint centre falls in the segment coverage assigns it");
        }

        // 3. The source is the teaching footprint, not the whole unit strip.
        Cov.Area source = cov.Areas.Single(a => a.IsSource);
        IReadOnlyList<RackPoint> sp = fp.PolygonOf(source.Id);
        Geo.CoverageUnit su = g.Units.First(u => u.Index == source.UnitIndex && u.RackId == source.RackId);
        float l = ScenarioOneRackBay.HalfLengthM, w = g.Teaching.HalfWidth;
        Check(sp != null && sp.Count == 4 && Math.Abs(Area(sp) - 4.0 * l * w) < 0.05, label + ": the source polygon is the fixed teaching rectangle (" + (4.0 * l * w).ToString("0") + " m2)");
        Check(sp != null && sp.All(q => Math.Abs(g.Teaching.Along(q.X, q.Z)) <= l + Eps && Math.Abs(g.Teaching.Across(q.X, q.Z)) <= w + Eps)
              && sp.Any(q => Math.Abs(Math.Abs(g.Teaching.Along(q.X, q.Z)) - l) < Eps) && sp.Any(q => Math.Abs(Math.Abs(g.Teaching.Across(q.X, q.Z)) - w) < Eps),
            label + ": the source corners lie on the teaching frame's own along / across limits");
        Check(sp != null && Area(sp) < Area(su.Polygon) * 0.5, label + ": the source is much smaller than its whole unit strip (" + (sp != null ? Area(sp).ToString("0") : "-") + " of " + Area(su.Polygon).ToString("0") + " m2)");
        fp.TryCentre(source.Id, out float sx, out float sz);
        Check(Math.Abs(sx - g.Teaching.X) < Eps && Math.Abs(sz - g.Teaching.Z) < Eps, label + ": the source waypoint is the teaching anchor centre");

        // 4. Waypoints come from the same polygons and are spatially distinct and ordered along the rack axis.
        bool ordered = true, apart = true, same = true;
        foreach (Geo.CoverageUnit u in g.Units)
        {
            var c = new List<(double along, float x, float z)>();
            foreach (Cov.Segment seg in new[] { Cov.Segment.Low, Cov.Segment.Centre, Cov.Segment.High })
            {
                Cov.Area a = cov.Areas.First(x => x.UnitIndex == u.Index && x.RackId == u.RackId && x.Segment == seg);
                Check(fp.TryCentre(a.Id, out float cx, out float cz), label + ": centre available for " + a.Id);
                StandMapAreaFootprints.Centroid(fp.PolygonOf(a.Id), out float px, out float pz);
                if (Math.Abs(px - cx) > 1e-3f || Math.Abs(pz - cz) > 1e-3f) same = false;
                c.Add((Along(g, new RackPoint(cx, cz)), cx, cz));
            }
            if (!(c[0].along < c[1].along && c[1].along < c[2].along)) ordered = false;
            if (c[1].along - c[0].along < 5.0 || c[2].along - c[1].along < 5.0) apart = false;
        }
        Check(same, label + ": TryCentre is the centroid of the drawn polygon (one derivation)");
        Check(ordered, label + ": Low < Centre < High waypoints along the rack axis in every unit");
        Check(apart, label + ": Low / Centre / High waypoints are at least 5 m apart in every unit");

        // 5. Tags: printed at each footprint's own centre, no two stack (116 x 17.5 px at 688 px / 80 m).
        double pxPerM = 688.0 / 80.0;
        var rects = new List<(string id, double x, double y)>();
        foreach (Cov.Area a in cov.Areas) { fp.TryCentre(a.Id, out float cx, out float cz); rects.Add((a.Id, cx * pxPerM, cz * pxPerM)); }
        int stacked = 0, overlapping = 0;
        for (int i = 0; i < rects.Count; i++)
            for (int j = i + 1; j < rects.Count; j++)
            {
                if (Math.Abs(rects[i].x - rects[j].x) < 1 && Math.Abs(rects[i].y - rects[j].y) < 1) stacked++;
                if (Math.Abs(rects[i].x - rects[j].x) < 116 && Math.Abs(rects[i].y - rects[j].y) < 17.5) overlapping++;
            }
        Check(stacked == 0, label + ": no two area tags share a position (was 15 areas on 5 positions)");
        Check(overlapping == 0, label + ": no two area tag boxes overlap (" + overlapping + " overlaps)");

        // 6. Remainders keep their authoritative polygons.
        foreach (var r in new[] { ("remainder-right", g.RightRemainder), ("remainder-left", g.LeftRemainder) })
        {
            IReadOnlyList<RackPoint> p = fp.PolygonOf(r.Item1);
            Check(r.Item2.IsEmpty ? p == null : p != null && Key(p) == Key(r.Item2.Polygon), label + ": " + r.Item1 + " keeps its authoritative polygon");
        }
    }

    public static void Run(string root)
    {
        // Axis-aligned east-west plan (the live Rack V2 fixture) and rotated / translated plans.
        Plan("east-west", Racks(0));
        Plan("rotated 35 deg", Racks(35));
        Plan("rotated -20 deg", Racks(-20));
        Plan("rotated 90 deg", Racks(90));

        // Boundary and invalid input.
        StandMapAreaFootprints none = StandMapAreaFootprints.Derive(null, null);
        Check(!none.Valid && none.PolygonOf("R1L") == null && !none.TryCentre("R1L", out _, out _), "no coverage / geometry: no footprint, no waypoint, nothing guessed");

        // Source text: drawing, tags and the waypoint share the one derivation; no second geometry path in the review card.
        string card = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/StandMapTreatmentReview.cs"));
        Check(card.Contains("StandMapAreaFootprints.Derive(") && card.Split(new[] { "StandMapAreaFootprints.Derive(" }, StringSplitOptions.None).Length - 1 == 1, "the review derives footprints in exactly one place");
        Check(card.Contains("footprints.PolygonOf(row.Id)") && card.Contains("footprints.TryCentre(areaId"), "the map footprints and Inspect in forest both read that one derivation");
        Check(!card.Contains(".Polygon") || !card.Contains("unit.Polygon"), "the review card no longer picks whole-unit polygons by unit and rack");
        Check(!card.Contains("geometry.Units") && !card.Contains("OuterRemainder"), "no separate unit / remainder geometry logic remains in the review card");
        string helper = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/Map/StandMapAreaFootprints.cs"));
        Check(!helper.Contains("using UnityEngine") && !helper.Contains("Classify(") && !helper.Contains("ReadCoverage"), "the footprint helper is pure and adds no coverage logic");
        Check(helper.Contains("area.Segment") && !helper.Contains("area.Id.EndsWith") && !helper.Contains("Substring"), "segments come from Area.Segment, never parsed from the R1L / R1C / R1H id");
    }
}
