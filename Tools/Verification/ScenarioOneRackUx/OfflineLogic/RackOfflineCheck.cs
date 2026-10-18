using System;
using System.Collections.Generic;
using System.Linq;

// Offline check of the rack geometry on a synthetic Scenario-One-like plantation (41 x 41 lattice, 1.9 m, +-0.45 m jitter).
public static class RackOfflineCheck
{
    static int passed, failed;
    static void Check(bool ok, string name, string detail = "") { if (ok) passed++; else failed++; Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " " + detail : "")); }

    public static int Main()
    {
        var rng = new Random(12345);
        var trees = new List<RackTree>(); int n = 41; float sp = 1.9f;
        for (int r = 0; r < n; r++)
        for (int c = 0; c < n; c++)
        {
            if (rng.NextDouble() < 0.2) continue;   // omissions
            float z = (r - 20) * sp + (float)(rng.NextDouble() - 0.5) * 0.9f;
            float x = (c - 20) * sp + (float)(rng.NextDouble() - 0.5) * 0.9f;
            trees.Add(new RackTree("P" + r.ToString("D2") + c.ToString("D2"), x, z));
        }
        var pts = trees.Select(t => new RackPoint(t.X, t.Z)).ToList();
        float angle = RackRows.DominantAngle(pts);
        Check(angle == 0f || angle == 90f, "dominant angle of a square planting is one of its two row axes", angle.ToString());
        var rows0 = RackRows.Find(pts, 0f);
        Check(rows0.Count == 41, "41 rows found across the z coordinate", rows0.Count.ToString());
        var rows90 = RackRows.Find(pts, 90f);
        Check(rows90.Count == 41, "41 rows found along the other axis", rows90.Count.ToString());
        Check(rows0.All(r => r.Count > 20), "every row has trees");
        var layout = RackLayout.RowIndices(41, 7);
        Check(layout.SequenceEqual(new[] { 4, 12, 20, 28, 36 }), "even layout of 41 rows with 7 between: " + string.Join(",", layout));
        Check(RackLayout.RowIndices(41, 8).SequenceEqual(new[] { 6, 15, 24, 33 }), "layout with 8 between: " + string.Join(",", RackLayout.RowIndices(41, 8)));
        Check(RackLayout.RowIndices(5, 7).Count == 0, "too few rows: no layout");

        // A rack through a row tree: boundary to boundary, takes that row only.
        RackTree pick = trees.First(t => t.Id.StartsWith("P20"));
        int row20 = RackRows.Nearest(rows0, pick.Z);
        float centre = rows0[row20].Centre;
        Check(RackGeometry.ClipLineToSquare(new RackPoint(0, centre), 0f, 40f, out RackPoint a, out RackPoint b), "line clips to the stand");
        Check(Math.Abs(a.X + 40f) < 1e-3 && Math.Abs(b.X - 40f) < 1e-3 && Math.Abs(a.Z - centre) < 1e-3, "rack ends exactly on the stand boundary", a + " " + b);
        RackSelection sel = RackGeometry.Select(trees, a, b, RackGeometry.DefaultWidthM);
        int rowCount = trees.Count(t => Math.Abs(t.Z - centre) < 1.0f);
        Check(sel.Trees.Count == rowCount, "one row selected: " + sel.Trees.Count + " vs row " + rowCount);
        Check(sel.Trees.All(t => Math.Abs(t.Z - centre) <= 1.2f), "all selected trees lie within the half width");
        Check(sel.Trees.Zip(sel.Trees.Skip(1), (p, q) => p.X <= q.X).All(x => x), "ordered along the line");
        // Determinism under reordering.
        var shuffled = trees.OrderBy(t => t.Id.GetHashCode() ^ 0x5bd1e995).ToList();
        RackSelection sel2 = RackGeometry.Select(shuffled, a, b, RackGeometry.DefaultWidthM);
        Check(sel.Trees.Select(t => t.Id).SequenceEqual(sel2.Trees.Select(t => t.Id)), "selection and order do not depend on input order");
        // Rotation changes the selection.
        RackGeometry.ClipLineToSquare(new RackPoint(0, centre), 15f, 40f, out RackPoint c1, out RackPoint c2);
        RackSelection rot = RackGeometry.Select(trees, c1, c2, RackGeometry.DefaultWidthM);
        Check(rot.Trees.Count != sel.Trees.Count && Math.Abs(Math.Abs(c1.X) - 40f) < 1e-3 || Math.Abs(Math.Abs(c1.Z) - 40f) < 1e-3, "15 degree rack ends on the boundary", c1 + " " + c2);
        Check(rot.Trees.All(t => t.X >= -40.01f && t.X <= 40.01f), "rotated rack selects only trees inside the stand");
        // Crop trees and planned trees are left out and counted.
        var withFlags = trees.Select(t => t.Id == sel.Trees[0].Id ? new RackTree(t.Id, t.X, t.Z, true, true, false)
            : t.Id == sel.Trees[1].Id ? new RackTree(t.Id, t.X, t.Z, true, false, true) : t).ToList();
        RackSelection flagged = RackGeometry.Select(withFlags, a, b, RackGeometry.DefaultWidthM);
        Check(flagged.SkippedCropTrees == 1 && flagged.AlreadyPlanned == 1 && flagged.Trees.Count == sel.Trees.Count - 2, "crop tree and planned tree skipped and counted");
        // Bounded start/end: only the middle third.
        RackSelection part = RackGeometry.Select(trees, new RackPoint(-10, centre), new RackPoint(10, centre), RackGeometry.DefaultWidthM);
        Check(part.Trees.Count > 5 && part.Trees.All(t => t.X >= -10.01f && t.X <= 10.01f), "bounded rack takes only trees between start and end", part.Trees.Count.ToString());
        // Polygon stays inside the stand.
        foreach (float ang in new[] { 0f, 15f, 45f, 90f, 135f })
        {
            RackGeometry.ClipLineToSquare(new RackPoint(3, 2), ang, 40f, out RackPoint s, out RackPoint e);
            var poly = RackGeometry.CorridorPolygon(s, e, 2.4f, 40f);
            Check(poly.Count >= 3 && poly.All(p => Math.Abs(p.X) <= 40.001f && Math.Abs(p.Z) <= 40.001f), "corridor polygon is cut to the stand at " + ang + " degrees", poly.Count + " vertices");
        }
        Check(!RackGeometry.ClipLineToSquare(new RackPoint(100, 100), 0f, 40f, out _, out _), "a line outside the stand is rejected");
        Console.WriteLine(failed == 0 ? "RACK_OFFLINE_PASS checks=" + passed : "RACK_OFFLINE_FAIL failed=" + failed);
        return failed == 0 ? 0 : 1;
    }
}
