// Plain .NET checks of MFM-02: the rack-served teaching sample and coverage geometry (ScenarioOneRackSampleGeometry) and the V2 entry of
// the rack-area lesson rules (ScenarioOneRackBay.AnalyseTeachingRack). Decision rules only; the live Unity gate checks the live feed.
// The V2 fixture is the qualified Rack V2 geometry (RACK-V2-COMBINED evidence rv2c-state.json): five racks from the headland (x -35.4)
// to the east edge (x 40) at the persisted z values below. The tree fixture is the same live-captured Legacy40 core the rack-bay check
// uses (core-stand-live-2m.csv); it covers the teaching footprint but not the outer ring, so its V2 counts are an offline prediction.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Geo = ScenarioOneRackSampleGeometry;

public static class RackSampleGeometryOfflineCheck
{
    private static int checks, failures;
    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) failures++;
    }
    private static string F(double v) => v.ToString("0.#####", CultureInfo.InvariantCulture);
    private static bool Near(double a, double b, double tolerance = 1e-3) => Math.Abs(a - b) <= tolerance;

    // The five persisted rack z values of the qualified headland fixture, rack ids 1..5 (south to north).
    private static readonly double[] QualifiedZ = { -35.2711754, -16.4217644, 0.2382353, 15.7935295, 35.2323532 };
    private const double HeadlandX = -35.4, EastX = 40.0;
    // The historical authored layout lines (the same capture RackBayOfflineCheck uses).
    private static readonly float[] HistoricalLines = { -31.4092f, -16.0152f, 0.0147f, 16.0224f, 31.3419f };

    private static RackPoint P(double x, double z) => new RackPoint((float)x, (float)z);
    private static Geo.RackInput Rack(int id, double ex, double ez, double fx, double fz, float width = 2.4f) => new Geo.RackInput(id, P(ex, ez), P(fx, fz), width);
    private static List<Geo.RackInput> Horizontal(int[] ids, double[] z, double x0 = HeadlandX, double x1 = EastX, float width = 2.4f)
    {
        var list = new List<Geo.RackInput>();
        for (int i = 0; i < z.Length; i++) list.Add(Rack(ids[i], x0, z[i], x1, z[i], width));
        return list;
    }
    private static readonly int[] Ids12345 = { 1, 2, 3, 4, 5 };
    private static List<Geo.RackInput> Qualified() => Horizontal(Ids12345, QualifiedZ);
    private static Geo.Result D(IReadOnlyList<Geo.RackInput> racks) => Geo.Derive(racks, 40f);

    private static Geo.RackInput Rotated(Geo.RackInput r, double degrees, bool swapEnds)
    {
        double a = degrees * Math.PI / 180.0, c = Math.Cos(a), s = Math.Sin(a);
        RackPoint Turn(RackPoint p) => P(p.X * c - p.Z * s, p.X * s + p.Z * c);
        return swapEnds ? new Geo.RackInput(r.Id, Turn(r.Far), Turn(r.Entrance), r.WidthM) : new Geo.RackInput(r.Id, Turn(r.Entrance), Turn(r.Far), r.WidthM);
    }

    private static IEnumerable<int[]> Permutations(int n)
    {
        var items = Enumerable.Range(0, n).ToArray();
        IEnumerable<int[]> Recurse(int k)
        {
            if (k == n) { yield return (int[])items.Clone(); yield break; }
            for (int i = k; i < n; i++)
            {
                (items[k], items[i]) = (items[i], items[k]);
                foreach (var p in Recurse(k + 1)) yield return p;
                (items[k], items[i]) = (items[i], items[k]);
            }
        }
        return Recurse(0);
    }

    // ---- the historical-geometry tree fixture, as RackBayOfflineCheck builds it ----
    private sealed class Stand
    {
        public List<ScenarioOneRackBay.Tree> Trees = new List<ScenarioOneRackBay.Tree>();
        public List<ScenarioOneRackBay.Order> Orders = new List<ScenarioOneRackBay.Order>();
        public Func<ScenarioOneRackBay.Analysis> Analyse;
        public ScenarioOneRackBay.Tree Get(string id) => Trees.First(t => t.Id == id);
        public void Crop(string id) { int i = Trees.FindIndex(t => t.Id == id); var t = Trees[i]; Trees[i] = new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, t.DbhCm, t.Living, true); }
        public void Plan(string id) { Orders.RemoveAll(o => o.TargetId == id); Orders.Add(new ScenarioOneRackBay.Order(id, true, false, ScenarioOneRackBay.Disposition.Sold, -1)); }
        public void Execute(int year)
        {
            foreach (var o in Orders.Where(o => o.Open).ToList())
            {
                Orders.Remove(o); Orders.Add(new ScenarioOneRackBay.Order(o.TargetId, false, true, o.Disposition, year));
                int i = Trees.FindIndex(t => t.Id == o.TargetId); var t = Trees[i]; Trees[i] = new ScenarioOneRackBay.Tree(t.Id, t.X, t.Z, t.DbhCm, false, false);
            }
        }
    }
    private static Stand LoadCore(string root, IEnumerable<double> rackZ)
    {
        var stand = new Stand();
        var zs = rackZ.ToArray();
        foreach (string line in File.ReadAllLines(Path.Combine(root, "Tools/Verification/ScenarioOneNonUi/OfflineLogic/core-stand-live-2m.csv")).Skip(1))
        {
            var c = line.Split(',');
            float x = float.Parse(c[1], CultureInfo.InvariantCulture), z = float.Parse(c[2], CultureInfo.InvariantCulture), d = float.Parse(c[3], CultureInfo.InvariantCulture);
            bool rack = zs.Any(l => Math.Abs(z - l) <= 1.2);
            stand.Trees.Add(new ScenarioOneRackBay.Tree(c[0], x, z, d, !rack, false));
            if (rack) stand.Orders.Add(new ScenarioOneRackBay.Order(c[0], false, true, ScenarioOneRackBay.Disposition.Sold, 0));
        }
        return stand;
    }
    private static float Dist(ScenarioOneRackBay.Tree a, ScenarioOneRackBay.Tree b) => (float)Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Z - b.Z) * (a.Z - b.Z));
    // The initial release the way a player would plan it: keep adding the strongest competitor of the weakest-released Crop Tree.
    private static void PlanInitial(Stand s)
    {
        for (int guard = 0; guard < 200; guard++)
        {
            var a = s.Analyse(); var weakest = a.Future.OrderBy(t => t.ReleaseShare).First();
            if (weakest.ReleaseShare >= ScenarioOneRackBay.FirstReleaseShare) return;
            var target = s.Get(weakest.Id);
            var next = s.Trees.Where(t => t.Living && !t.Crop && a.Contains(t.X, t.Z) && Dist(t, target) <= 8 && !s.Orders.Any(o => o.TargetId == t.Id && o.Open))
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.DbhCm, target.DbhCm, Dist(t, target))).First();
            s.Plan(next.Id);
        }
        throw new Exception("Initial release unreachable");
    }
    private static bool Same(ScenarioOneRackBay.Analysis a, ScenarioOneRackBay.Analysis b)
    {
        if (a.Applies != b.Applies || a.RackCount != b.RackCount || a.RackIndex != b.RackIndex || a.HalfWidthM != b.HalfWidthM
            || a.AreaCropCount != b.AreaCropCount || a.FutureCount != b.FutureCount || a.HasEnoughFuture != b.HasEnoughFuture
            || a.NearSelected != b.NearSelected || a.MiddleSelected != b.MiddleSelected || a.FarSelected != b.FarSelected || a.BothSides != b.BothSides
            || a.TooClose.Count != b.TooClose.Count || a.CropElsewhere != b.CropElsewhere || a.RemovedBasalShare != b.RemovedBasalShare
            || a.ExecutedRemovals != b.ExecutedRemovals || a.PlannedRemovals != b.PlannedRemovals || a.MarkedRemovals != b.MarkedRemovals
            || a.SoldExecuted != b.SoldExecuted || a.DeadwoodExecuted != b.DeadwoodExecuted || a.SoldPlanned != b.SoldPlanned || a.DeadwoodPlanned != b.DeadwoodPlanned
            || a.ReleaseYear != b.ReleaseYear || a.ReleasePlanned != b.ReleasePlanned || a.ReleaseExecuted != b.ReleaseExecuted
            || a.MixedPlanned != b.MixedPlanned || a.MixedExecuted != b.MixedExecuted || a.ReleaseKey != b.ReleaseKey
            || a.MinPlannedAndExecuted != b.MinPlannedAndExecuted || a.MinExecuted != b.MinExecuted || a.MinWithMarks != b.MinWithMarks)
            return false;
        for (int i = 0; i < a.Future.Count; i++)
        {
            var x = a.Future[i]; var y = b.Future[i];
            if (x.Id != y.Id || x.PlannedShare != y.PlannedShare || x.ExecutedShare != y.ExecutedShare || x.MarkedShare != y.MarkedShare) return false;
        }
        return true;
    }
    private static string Source(string root, string relative) => File.ReadAllText(Path.Combine(root, relative));

    public static int Run(string root)
    {
        // ---------------- A. the qualified five-rack fixture ----------------
        var qualified = Qualified();
        var q = D(qualified);
        Check(q.Valid, "qualified five-rack fixture is valid", q.Problem);
        Check(q.Racks.Select(r => r.Id).SequenceEqual(Ids12345) && q.Racks.Select((r, i) => r.Index == i).All(x => x), "fixture orders right-most to left-most: ids 1..5");
        Check(Enumerable.Range(0, 5).All(i => Near(q.Racks[i].OffsetM, QualifiedZ[i], 1e-4)), "offsets along the common normal equal the rack z values for an east-west axis");
        Check(q.AxisAngleDegrees == 0f && q.NormalZ == 1f && Math.Abs(q.NormalX) < 1e-6f && q.MaxAxisDeviationDegrees == 0f, "common axis is east, normal north, racks exactly parallel");
        Check(q.TeachingRack.Id == 3 && q.TeachingRack.Index == Geo.TeachingRackIndex && Geo.TeachingRackIndex == 2 && Geo.TeachingRackCount == 5, "teaching rack is the third of five (rack 3)");
        Check(Near(q.Anchor.X, 0, 1e-4) && Near(q.Anchor.Z, 0.2382353, 1e-4) && Near(q.AnchorAlongM, 35.4, 1e-4) && !q.AnchorClamped && Near(q.UnclampedAlongM, q.AnchorAlongM, 1e-4),
            "anchor is the closest point to stand centre, unclamped", "anchor " + q.Anchor + " along " + F(q.AnchorAlongM));
        double midpointX = (HeadlandX + EastX) * 0.5;
        Check(Math.Abs(q.Anchor.X - midpointX) > 2.0, "anchor is not the segment midpoint", "midpoint x " + F(midpointX) + " anchor x " + F(q.Anchor.X));
        float teachLength = q.TeachingRack.LengthM;
        Check(q.AnchorAlongM - ScenarioOneRackBay.HalfLengthM >= 0f && q.AnchorAlongM + ScenarioOneRackBay.HalfLengthM <= teachLength, "the 12 m each way teaching length lies on the confirmed segment");
        Check(q.Teaching.HalfWidth == 9f && ScenarioOneRackBay.TeachingHalfWidthM == 9f && ScenarioOneRackBay.HalfLengthM == 12f && ScenarioOneRackBay.DefaultHalfWidthM == 7.6f,
            "fixed 9.0 m teaching half-width, 12.0 m half-length; model-0 default half-width 7.6 m retained");

        // ---------------- B. deterministic ordering (any input order, any id assignment) ----------------
        string reference = q.Signature();
        int permutationCount = 0, signatureDiffers = 0;
        foreach (int[] order in Permutations(5))
        {
            permutationCount++;
            if (D(order.Select(i => qualified[i]).ToList()).Signature() != reference) signatureDiffers++;
        }
        Check(permutationCount == 120 && signatureDiffers == 0, "all 120 input orders give a bit-identical derivation", "differing=" + signatureDiffers);
        int wrongTeaching = 0, idPermutations = 0;
        foreach (int[] assign in Permutations(5))
        {
            idPermutations++;
            var racks = Horizontal(assign.Select(i => i + 1).ToArray(), QualifiedZ);
            var r = D(racks);
            if (!r.Valid || !Near(r.TeachingRack.Entrance.Z, QualifiedZ[2], 1e-4) || !Near(r.Anchor.Z, QualifiedZ[2], 1e-4)
                || !r.Racks.Select(x => x.Entrance.Z).SequenceEqual(r.Racks.Select(x => x.Entrance.Z).OrderBy(z => z))) wrongTeaching++;
        }
        Check(idPermutations == 120 && wrongTeaching == 0, "whatever ids the five racks were given, the teaching rack is the central one by position", "wrong=" + wrongTeaching);
        var permutedIds = D(Horizontal(new[] { 4, 5, 1, 3, 2 }, QualifiedZ));
        Check(permutedIds.TeachingRack.Id == 1 && permutedIds.Racks.Select(r => r.Id).SequenceEqual(new[] { 4, 5, 1, 3, 2 }), "ids 4,5,1,3,2 by position: the third by position is rack 1, not the third by id (rack 3)");

        // Tie-break: two collinear racks share one offset; the lower id is ordered first, so the third rack follows the id.
        List<Geo.RackInput> TieFixture(int idWest, int idEast) => new List<Geo.RackInput>
        {
            Rack(1, -35.4, -30, 40, -30), Rack(2, -35.4, -15, 40, -15), Rack(idWest, -35.4, 0, -5, 0), Rack(idEast, 5, 0, 40, 0), Rack(5, -35.4, 15, 40, 15)
        };
        var tieA = D(TieFixture(9, 4)); var tieB = D(TieFixture(4, 9));
        Check(tieA.Valid && tieB.Valid && tieA.TeachingRack.Id == 4 && tieB.TeachingRack.Id == 4, "tied offsets: the lower rack id is ordered first (third rack = id 4 either way)");
        Check(tieA.TeachingRack.Entrance.X == 5f && tieB.TeachingRack.Entrance.X == -35.4f, "the tie-break follows the id, not the position along the rack");
        var tieShuffled = TieFixture(9, 4); tieShuffled.Reverse();
        Check(D(tieShuffled).Signature() == tieA.Signature(), "tied racks derive identically from a shuffled list");
        var nearTie = new List<Geo.RackInput> { Rack(1, -35.4, -30, 40, -30), Rack(2, -35.4, -15, 40, -15), Rack(8, -35.4, 0.0004, -5, 0.0004), Rack(3, 5, 0, 40, 0), Rack(5, -35.4, 15, 40, 15) };
        Check(D(nearTie).TeachingRack.Id == 3, "offsets within a millimetre are a tie (resolved by id)");

        // Orientation of each rack's entrance/far does not matter; whole-fixture rotation moves nothing but the coordinates.
        int rotationBad = 0;
        foreach (double angle in new[] { 0.0, 35.0, 90.0, 145.0, 180.0, 215.0, 270.0, 330.0 })
            for (int mask = 0; mask < 32; mask += 7)
            {
                var spun = qualified.Select((r, i) => Rotated(r, angle, ((mask >> (i % 5)) & 1) == 1)).ToList();
                var r2 = D(spun);
                if (!r2.Valid || r2.TeachingRack.Id != 3 || !r2.Racks.Select(x => x.Id).SequenceEqual(Ids12345)
                    && !r2.Racks.Select(x => x.Id).SequenceEqual(Ids12345.Reverse())) rotationBad++;
            }
        Check(rotationBad == 0, "rotating the fixture or swapping rack ends keeps the teaching rack (rack 3) and a monotone order", "bad=" + rotationBad);
        var turned = D(qualified.Select(r => Rotated(r, 35.0, false)).ToList());
        Check(Near(turned.AxisAngleDegrees, 35.0, 1e-3) && turned.TeachingRack.Id == 3, "35 degree fixture: common axis 35 degrees", "axis " + F(turned.AxisAngleDegrees));
        var turnedAnchor = Rotated(new Geo.RackInput(0, q.Anchor, q.Anchor, 1f), 35.0, false);
        Check(Near(turned.Anchor.X, turnedAnchor.Entrance.X, 1e-3) && Near(turned.Anchor.Z, turnedAnchor.Entrance.Z, 1e-3), "the anchor turns with the fixture",
            "anchor " + turned.Anchor + " expected " + turnedAnchor.Entrance);

        // ---------------- C. invalid or indefensible plans fail visibly ----------------
        Check(!D(qualified.Take(4).ToList()).Valid && !D(qualified.Concat(new[] { Rack(6, HeadlandX, 25, EastX, 25) }).ToList()).Valid, "fewer or more than five racks are invalid");
        Check(!D(null).Valid && D(qualified.Take(4).ToList()).Problem.Contains("exactly 5"), "a missing or short plan says why");
        var dup = Qualified(); dup[4] = Rack(1, HeadlandX, 35.2323532, EastX, 35.2323532);
        Check(!D(dup).Valid && D(dup).Problem.Contains("more than once"), "a duplicate rack id is invalid");
        var zeroId = Qualified(); zeroId[0] = Rack(0, HeadlandX, -35.2711754, EastX, -35.2711754);
        Check(!D(zeroId).Valid, "a non-positive rack id is invalid");
        var nan = Qualified(); nan[1] = new Geo.RackInput(2, new RackPoint(float.NaN, -16f), P(EastX, -16), 2.4f);
        Check(!D(nan).Valid, "a non-finite endpoint is invalid");
        var empty = Qualified(); empty[3] = Rack(4, 10, 15.7935295, 10, 15.7935295);
        Check(!D(empty).Valid, "a rack with no length is invalid");
        var thin = Qualified(); thin[3] = Rack(4, HeadlandX, 15.7935295, EastX, 15.7935295, 0f);
        Check(!D(thin).Valid, "a rack with no corridor width is invalid");
        var crossing = Qualified(); crossing[1] = Rack(2, HeadlandX, -16.4217644, EastX, 10);
        var crossed = D(crossing);
        Check(!crossed.Valid && crossed.Problem.Contains("no defensible order"), "a rack that crosses its neighbours has no defensible order", crossed.Problem);
        var pentagon = new List<Geo.RackInput>();
        for (int i = 0; i < 5; i++) { double a = 36.0 * i * Math.PI / 180.0; pentagon.Add(Rack(i + 1, -30 * Math.Cos(a) + i, -30 * Math.Sin(a), 30 * Math.Cos(a) + i, 30 * Math.Sin(a))); }
        Check(!D(pentagon).Valid && D(pentagon).Problem.Contains("do not share a direction"), "racks that do not share a direction have no common normal");
        var skewed = Qualified(); skewed[4] = Rack(5, HeadlandX, 35.2323532, EastX, 36.5);
        Check(D(skewed).Valid && D(skewed).MaxAxisDeviationDegrees > 0f, "a slightly skewed but separated rack is still ordered, with its deviation reported", "max deviation " + F(D(skewed).MaxAxisDeviationDegrees));
        var shortTeaching = Qualified(); shortTeaching[2] = Rack(3, -10, 0.2382353, 10, 0.2382353);
        Check(!D(shortTeaching).Valid && D(shortTeaching).Problem.Contains("shorter"), "a teaching rack shorter than 24 m cannot hold the teaching length");
        var exact24 = Qualified(); exact24[2] = Rack(3, -12, 0.2382353, 12, 0.2382353);
        var e24 = D(exact24);
        Check(e24.Valid && Near(e24.AnchorAlongM, 12, 1e-4) && !e24.AnchorClamped, "a 24 m teaching rack is exactly long enough; the anchor is its midpoint");

        // ---------------- D. anchor: closest point, clamped so +-12 m stays on the segment ----------------
        var east = Qualified(); east[2] = Rack(3, 10, 0.5, 40, 0.5);
        var ea = D(east);
        Check(ea.Valid && ea.AnchorClamped && Near(ea.UnclampedAlongM, -10, 1e-4) && Near(ea.AnchorAlongM, 12, 1e-4) && Near(ea.Anchor.X, 22, 1e-4) && Near(ea.Anchor.Z, 0.5, 1e-4),
            "closest point lies beyond the entrance end: clamped to 12 m along (x 22)", "along " + F(ea.AnchorAlongM) + " unclamped " + F(ea.UnclampedAlongM));
        var west = Qualified(); west[2] = Rack(3, -40, 0.5, -10, 0.5);
        var we = D(west);
        Check(we.Valid && we.AnchorClamped && Near(we.UnclampedAlongM, 40, 1e-4) && Near(we.AnchorAlongM, 18, 1e-4) && Near(we.Anchor.X, -22, 1e-4),
            "closest point lies beyond the far end: clamped to length minus 12 m (x -22)", "along " + F(we.AnchorAlongM));
        var edge = Qualified(); edge[2] = Rack(3, -12, 0.5, 40, 0.5);
        var ed = D(edge);
        Check(ed.Valid && !ed.AnchorClamped && Near(ed.AnchorAlongM, 12, 1e-4) && Near(ed.Anchor.X, 0, 1e-4), "closest point exactly 12 m from the entrance needs no clamping");
        var reversed = Qualified(); reversed[2] = Rack(3, EastX, 0.2382353, HeadlandX, 0.2382353);
        var rv = D(reversed);
        Check(rv.Valid && Near(rv.Anchor.X, 0, 1e-4) && Near(rv.AnchorAlongM, 40.0, 1e-4) && rv.Teaching.AlongX == -1f, "entrance at the east end: along runs west, anchor still the closest point");
        int anchorBad = 0;
        foreach (double angle in new[] { 20.0, 45.0, 100.0, 160.0 })
        {
            double a = angle * Math.PI / 180.0, ux = Math.Cos(a), uz = Math.Sin(a), nx = -uz, nz = ux;
            var diagonal = new List<Geo.RackInput>();
            double[] offsets = { -34, -17, 6.5, 22, 38 };
            for (int i = 0; i < 5; i++)
            {
                double fx = nx * offsets[i], fz = nz * offsets[i] * 1.0;
                diagonal.Add(Rack(i + 1, fx - 30 * ux, fz - 30 * uz, fx + 30 * ux, fz + 30 * uz));
            }
            var dr = D(diagonal);
            double footX = nx * 6.5, footZ = nz * 6.5;
            if (!dr.Valid || dr.TeachingRack.Id != 3 || dr.AnchorClamped || !Near(dr.Anchor.X, footX, 2e-3) || !Near(dr.Anchor.Z, footZ, 2e-3)
                || !Near(dr.Anchor.X * ux + dr.Anchor.Z * uz, 0, 2e-3)) anchorBad++;
        }
        Check(anchorBad == 0, "angled racks: anchor is the perpendicular foot from the stand centre (dot with the rack direction is 0)", "bad=" + anchorBad);

        // ---------------- E. teaching footprint: fixed width, actual corridors ----------------
        var f = q.Teaching;
        float z0 = q.Anchor.Z;
        Check(f.Contains(0, z0 + 9f) && !f.Contains(0, z0 + 9.05f) && f.Contains(0, z0 - 9f) && !f.Contains(0, z0 - 9.05f), "footprint is exactly 9.0 m each side of the rack centreline");
        Check(f.Contains(12f, z0 + 3f) && !f.Contains(12.05f, z0 + 3f) && f.Contains(-12f, z0 + 3f) && !f.Contains(-12.05f, z0 + 3f), "footprint runs 12.0 m each way from the anchor");
        Check(!f.Contains(0, z0) && !f.Contains(0, z0 + 1.2f) && f.Contains(0, z0 + 1.21f) && f.InCorridor(0, z0), "the teaching rack's own corridor (2.4 m) is excluded");
        var wide = Horizontal(Ids12345, QualifiedZ, HeadlandX, EastX, 4f);
        var fw = D(wide).Teaching;
        Check(!fw.Contains(0, z0 + 1.5f) && fw.Contains(0, z0 + 2.1f) && f.Contains(0, z0 + 1.5f), "corridor exclusion uses the actual confirmed rack width (4 m rack: 2 m each side)");
        int independentBad = 0; int baseline = -1;
        foreach (double gap in new[] { 16.0, 18.0, 20.0, 30.0 })
        {
            var spaced = D(Horizontal(Ids12345, new[] { -2 * gap, -gap, 0.0, gap, 2 * gap }, HeadlandX, EastX));
            // Lattice of 0.5 m over the whole stand: how many ground points fall inside the footprint.
            int inside = 0;
            for (double x = -40; x <= 40; x += 0.5) for (double zz = -40; zz <= 40; zz += 0.5) if (spaced.Teaching.Contains((float)x, (float)zz)) inside++;
            if (baseline < 0) baseline = inside;
            if (spaced.Teaching.HalfWidth != 9f || inside != baseline) independentBad++;
        }
        Check(independentBad == 0 && baseline > 0, "9.0 m footprint is identical whatever the neighbouring rack spacing (16, 18, 20, 30 m)", "lattice points " + baseline);
        var tight = D(Horizontal(Ids12345, new[] { -12.0, -6.0, 0.0, 6.0, 12.0 }, HeadlandX, EastX)).Teaching;
        Check(tight.HalfWidth == 9f && !tight.Contains(0, 6f) && !tight.Contains(0, 5f) && !tight.Contains(0, 7f) && tight.Contains(0, 4.7f) && tight.Contains(0, 7.3f),
            "a neighbouring rack 6 m away has its real corridor (z 4.8..7.2) cut out of the footprint; the footprint does not shrink to 6 m");
        var shortNeighbour = new List<Geo.RackInput>
        {
            Rack(1, HeadlandX, -30, EastX, -30), Rack(2, HeadlandX, -15, EastX, -15), Rack(3, HeadlandX, 0, EastX, 0), Rack(4, HeadlandX, 6, -2, 6), Rack(5, HeadlandX, 30, EastX, 30)
        };
        var sn = D(shortNeighbour);
        Check(sn.Valid && sn.TeachingRack.Id == 3 && !sn.Teaching.Contains(-5f, 6f) && sn.Teaching.Contains(5f, 6f), "a neighbouring rack's corridor ends where the rack ends (actual segment, not an infinite line)");

        // Zones and sides are the old definitions, relative to the anchor and the entrance-to-far direction.
        var legacyFrame = new ScenarioOneRackBay.Frame(P(q.Anchor.X - 20, q.Anchor.Z), P(q.Anchor.X + 20, q.Anchor.Z), 9f);
        int zoneBad = 0;
        for (double x = -14; x <= 14; x += 0.7) for (double zz = -10; zz <= 10; zz += 0.7)
        {
            float px = (float)x, pz = (float)(z0 + zz);
            if (f.Zone(px, pz) != legacyFrame.Zone(px, pz) || f.Across(px, pz) != legacyFrame.Across(px, pz) || f.Along(px, pz) != legacyFrame.Along(px, pz)) zoneBad++;
        }
        Check(zoneBad == 0, "near/middle/far zones and left/right sides match the existing frame definitions at the anchor", "bad=" + zoneBad);
        Check(f.Zone((float)(q.Anchor.X - 4.5), z0) == 0 && f.Zone(q.Anchor.X, z0) == 1 && f.Zone((float)(q.Anchor.X + 4.5), z0) == 2 && f.Across(0, z0 + 3f) > 0 && f.Across(0, z0 - 3f) < 0,
            "near is the entrance end (west here), far the other end; positive across is the left of entrance-to-far");

        // ---------------- F. coverage strips: half-distance, explicit remainders, distinct from the teaching footprint ----------------
        bool halfway = true;
        for (int k = 0; k < 4; k++) halfway &= Near(q.Units[k].UpperOffsetM, (QualifiedZ[k] + QualifiedZ[k + 1]) * 0.5, 1e-4) && q.Units[k].UpperOffsetM == q.Units[k + 1].LowerOffsetM;
        Check(halfway && q.Units.Select(u => u.RackId).SequenceEqual(Ids12345), "interior unit boundaries lie halfway between neighbouring rack centrelines and units abut");
        Check(Near(q.Units[0].LowerOffsetM, QualifiedZ[0] - (QualifiedZ[1] - QualifiedZ[0]) * 0.5, 1e-4) && Near(q.Units[4].UpperOffsetM, QualifiedZ[4] + (QualifiedZ[4] - QualifiedZ[3]) * 0.5, 1e-4)
            && q.Units[0].LowerIsOuterEdge && q.Units[4].UpperIsOuterEdge && !q.Units[0].UpperIsOuterEdge && !q.Units[4].LowerIsOuterEdge && q.Units.Skip(1).Take(3).All(u => !u.LowerIsOuterEdge && !u.UpperIsOuterEdge),
            "the two outermost edges mirror the adjacent half-gap and are flagged as outer edges");
        double teachLow = z0 - 9.0, teachHigh = z0 + 9.0;
        Check(Math.Abs(q.Units[2].LowerOffsetM - teachLow) > 0.5 && Math.Abs(q.Units[2].UpperOffsetM - teachHigh) > 0.5 && Math.Abs(q.Units[2].WidthM * 0.5 - 9.0) > 0.5,
            "coverage boundaries are not the teaching footprint's 9.0 m edges", "unit " + F(q.Units[2].LowerOffsetM) + ".." + F(q.Units[2].UpperOffsetM) + " vs footprint " + F(teachLow) + ".." + F(teachHigh));
        Check(q.RightRemainder != null && q.LeftRemainder != null && q.RightRemainder.Side == Geo.Side.Right && q.LeftRemainder.Side == Geo.Side.Left, "both outer remainders are always reported");
        Check(q.RightRemainder.IsEmpty && q.LeftRemainder.IsEmpty && q.RightRemainder.StripEdgeOffsetM < -40f && q.LeftRemainder.StripEdgeOffsetM > 40f
            && q.RightRemainder.StandLimitOffsetM == -40f && q.LeftRemainder.StandLimitOffsetM == 40f,
            "qualified fixture: the mirrored outer strips already pass the stand edge, so both remainders are explicit but empty",
            "edges " + F(q.RightRemainder.StripEdgeOffsetM) + " / " + F(q.LeftRemainder.StripEdgeOffsetM));
        double total = q.Units.Sum(u => (double)u.AreaM2) + q.RightRemainder.AreaM2 + q.LeftRemainder.AreaM2;
        Check(Near(total, 6400.0, 0.05), "units plus remainders partition the 80 x 80 m stand (qualified)", F(total) + " m2");
        bool areas = true;
        foreach (var u in q.Units) areas &= Near(u.AreaM2, (Math.Min(u.UpperOffsetM, 40.0) - Math.Max(u.LowerOffsetM, -40.0)) * 80.0, 0.05);
        Check(areas && q.Units.All(u => Near(u.CorridorAreaM2, 2.4 * 75.4, 0.05) && u.Polygon.Count >= 3), "strip areas are their width cut to the stand times 80 m; each corridor is 2.4 x 75.4 m",
            string.Join(", ", q.Units.Select(u => F(u.AreaM2))));

        // Neighbours moving changes the strips but never the teaching footprint.
        var moved = D(Horizontal(Ids12345, new[] { QualifiedZ[0], QualifiedZ[1] - 3, QualifiedZ[2], QualifiedZ[3] + 3, QualifiedZ[4] }));
        int footprintDiffers = 0;
        for (double x = -14; x <= 14; x += 0.5) for (double zz = -12; zz <= 12; zz += 0.5)
            if (moved.Teaching.Contains((float)x, (float)(z0 + zz)) != q.Teaching.Contains((float)x, (float)(z0 + zz))) footprintDiffers++;
        Check(footprintDiffers == 0 && moved.Anchor.Z == q.Anchor.Z && moved.Teaching.HalfWidth == 9f && Math.Abs(moved.Units[2].LowerOffsetM - q.Units[2].LowerOffsetM) > 1.0 && Math.Abs(moved.Units[2].UpperOffsetM - q.Units[2].UpperOffsetM) > 1.0,
            "moving the neighbours 3 m moves the coverage boundaries but not one point of the teaching footprint");

        // A plan whose strips leave real remainders.
        var inner = D(Horizontal(Ids12345, new[] { -25.0, -12.0, 0.0, 12.0, 25.0 }));
        Check(inner.Valid && !inner.RightRemainder.IsEmpty && !inner.LeftRemainder.IsEmpty && Near(inner.RightRemainder.AreaM2, 8.5 * 80, 0.05) && Near(inner.LeftRemainder.AreaM2, 8.5 * 80, 0.05)
            && Near(inner.RightRemainder.StripEdgeOffsetM, -31.5, 1e-4) && Near(inner.LeftRemainder.StripEdgeOffsetM, 31.5, 1e-4),
            "racks at -25,-12,0,12,25: outer strips end at +-31.5, leaving two explicit 680 m2 remainders",
            F(inner.RightRemainder.AreaM2) + " / " + F(inner.LeftRemainder.AreaM2));
        Check(Near(inner.Units.Sum(u => (double)u.AreaM2) + inner.RightRemainder.AreaM2 + inner.LeftRemainder.AreaM2, 6400.0, 0.05), "units plus remainders partition the stand (inner racks)");
        bool partitionRotated = true;
        foreach (double angle in new[] { 20.0, 35.0, 90.0, 123.0 })
        {
            var rr = D(Horizontal(Ids12345, new[] { -25.0, -12.0, 0.0, 12.0, 25.0 }).Select(r => Rotated(r, angle, false)).ToList());
            double sum = rr.Units.Sum(u => (double)u.AreaM2) + rr.RightRemainder.AreaM2 + rr.LeftRemainder.AreaM2;
            partitionRotated &= rr.Valid && Near(sum, 6400.0, 0.1) && rr.RightRemainder.AreaM2 >= 0 && rr.LeftRemainder.AreaM2 >= 0 && rr.Units.All(u => u.Polygon.Count >= 3);
        }
        Check(partitionRotated, "units plus remainders still partition 6400 m2 when the racks run at 20, 35, 90 and 123 degrees");

        // Classification: every ground point has exactly one answer.
        Check(q.Classify(0f, (float)QualifiedZ[2]).Kind == Geo.PlacementKind.RackCorridor && q.Classify(0f, (float)QualifiedZ[2]).RackId == 3
            && q.Classify(0f, -30f).Kind == Geo.PlacementKind.CoverageUnit && q.Classify(0f, -30f).RackId == 1 && q.Classify(0f, -10f).RackId == 2
            && q.Classify(0f, 5f).RackId == 3 && q.Classify(0f, 10f).RackId == 4 && q.Classify(0f, 30f).RackId == 5 && q.Classify(0f, 40.01f).Kind == Geo.PlacementKind.OutsideStand,
            "classification: corridor of rack 3, strips of racks 1..5, outside the stand");
        Check(inner.Classify(0f, -35f).Kind == Geo.PlacementKind.OuterRemainder && inner.Classify(0f, -35f).Side == Geo.Side.Right
            && inner.Classify(0f, 35f).Kind == Geo.PlacementKind.OuterRemainder && inner.Classify(0f, 35f).Side == Geo.Side.Left && inner.Classify(0f, -31f).Kind == Geo.PlacementKind.CoverageUnit,
            "classification: outside the outermost strips is an explicit remainder on its own side");
        var counts = new Dictionary<Geo.PlacementKind, int>(); int unitLattice = 0, remainderLattice = 0;
        for (double x = -39.75; x <= 39.75; x += 0.5) for (double zz = -39.75; zz <= 39.75; zz += 0.5)
        {
            var place = inner.Classify((float)x, (float)zz);
            counts.TryGetValue(place.Kind, out int n); counts[place.Kind] = n + 1;
            if (place.Kind == Geo.PlacementKind.OuterRemainder) remainderLattice++;
            if (place.Kind == Geo.PlacementKind.CoverageUnit) unitLattice++;
        }
        Check(counts.Sum(c => c.Value) == 160 * 160 && !counts.ContainsKey(Geo.PlacementKind.OutsideStand) && Math.Abs(remainderLattice * 0.25 - 2 * 8.5 * 80) < 8.0,
            "0.5 m lattice over the stand: remainder points match the 1,360 m2 remainder area", "remainder " + remainderLattice * 0.25 + " m2");

        // ---------------- G. two separate 8 m constants; model-0 rules untouched ----------------
        string bay = Source(root, "Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBay.cs");
        Check(bay.Contains("public const float FutureTreeSpacingM = 8f;") && !bay.Contains("FutureTreeSpacingM = ForestEcologyController"), "teaching Crop Tree spacing is a literal 8 m, not an alias of the Hegyi neighbourhood");
        Check(ScenarioOneRackBay.FutureTreeSpacingM == 8f && ForestEcologyController.HegyiCutoffMeters == 8f && ScenarioOneRackBay.MaxRemovedBasalShare == 0.40f && ScenarioOneRackBay.FirstReleaseShare == 0.30f && ScenarioOneRackBay.FutureTreesWanted == 3,
            "both currently 8 m; 3 Crop Trees, 30% release and 40% safeguard retained");
        Check(bay.Contains("Distance(tree, f) <= ForestEcologyController.HegyiCutoffMeters") && bay.Contains("Distance(crop, other) < FutureTreeSpacingM"),
            "competitor reach still uses the Hegyi cutoff and Crop Tree spacing still uses the teaching spacing");
        string sequence = Source(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs");
        int v2Start = sequence.IndexOf("if (m.RackPlanningV2Active)", StringComparison.Ordinal), v2Else = sequence.IndexOf("else", v2Start, StringComparison.Ordinal);
        string v2Block = v2Start >= 0 && v2Else > v2Start ? sequence.Substring(v2Start, v2Else - v2Start) : "";
        string legacyBlock = v2Else > 0 ? sequence.Substring(v2Else, sequence.IndexOf("bool area =", v2Else, StringComparison.Ordinal) - v2Else) : "";
        Check(v2Block.Contains("m.RackPlanConfirmed") && v2Block.Contains("m.ConfirmedRacks") && !v2Block.Contains("routes") && !v2Block.Contains("layout") && !v2Block.Contains("LayoutStatus") && !v2Block.Contains("LayoutLines"),
            "V2 branch reads only the confirmed rack records, gated on confirmation; no authored line is reachable from it");
        Check(legacyBlock.Contains("layout[layout.Count / 2].Start, layout[layout.Count / 2].End") && legacyBlock.Contains("ScenarioOneRackBay.HalfWidth(routes, routes.Count / 2)")
            && legacyBlock.Contains("ScenarioOneRackBayReader.Read(trees, orders, routes, frame)"), "model-0 / Legacy40 branch keeps its original authored-layout statements");
        string reader = Source(root, "Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBayReader.cs");
        Check(reader.Contains("if (sample == null || !sample.Valid) return null;"), "an absent or invalid V2 sample yields no teaching area, never a fallback");

        // ---------------- H. nothing persisted, decided or proposed ----------------
        string helper = Source(root, "Assets/ForestPrototype/ScenarioOne/ScenarioOneRackSampleGeometry.cs");
        string[] forbidden = { "using UnityEngine", "UnityEngine.", "Serializable", "SerializeField", "ForestSaveData", "PlayerPrefs", "File.", "Prescription", "Proposal", "TreeMarkType", "WorkOrder", "CompetitionShare" };
        var found = forbidden.Where(token => helper.Contains(token)).ToList();
        Check(found.Count == 0, "geometry helper has no Unity, save, prescription, proposal, mark, order or Share state", string.Join(",", found));
        Check(!typeof(Geo.Result).IsSerializable && !typeof(Geo.CoverageUnit).IsSerializable && !typeof(Geo.OuterRemainder).IsSerializable, "geometry result types are not serializable");

        // ---------------- I. the V2 entry reproduces the historical analysis when fed the historical geometry ----------------
        var lines = HistoricalLines;
        float historicalHalfWidth = Math.Min(lines[2] - lines[1], lines[3] - lines[2]) * .5f;
        var corridors = lines.Select(l => new ScenarioOneRackBay.Corridor(P(-40, l), P(40, l), 1.2f)).ToArray();
        var historicalFrame = ScenarioOneRackBay.Frame.Anchored(P(0, lines[2]), 1f, 0f, historicalHalfWidth, corridors);
        Func<Stand, ScenarioOneRackBay.Analysis> legacy = s => ScenarioOneRackBay.Analyse(lines, s.Trees, s.Orders);
        Func<Stand, ScenarioOneRackBay.Analysis> viaV2 = s => ScenarioOneRackBay.AnalyseTeachingRack(s.Trees, s.Orders, historicalFrame, 3, 5, 2);
        Stand Hist() { var s = LoadCore(root, lines.Select(l => (double)l)); return s; }
        var h1 = Hist(); foreach (string id in new[] { "P0714", "P1109", "P0706" }) h1.Crop(id);
        h1.Analyse = () => legacy(h1);
        var v1 = Hist(); foreach (string id in new[] { "P0714", "P1109", "P0706" }) v1.Crop(id);
        v1.Analyse = () => viaV2(v1);
        bool sameSelect = Same(legacy(h1), viaV2(h1));
        PlanInitial(h1); PlanInitial(v1);
        bool samePlanned = Same(legacy(h1), viaV2(h1)) && Same(legacy(v1), viaV2(v1)) && h1.Orders.Count(o => o.Open) == v1.Orders.Count(o => o.Open);
        h1.Execute(2); v1.Execute(2);
        bool sameExecuted = Same(legacy(h1), viaV2(h1));
        int historicalFootprint = Hist().Trees.Count(t => t.Living && viaV2(Hist()).Contains(t.X, t.Z));
        Check(sameSelect && samePlanned && sameExecuted, "AnalyseTeachingRack fed the historical frame and corridors reproduces the legacy Analyse (selection, planned release, executed release)");
        var historicalAnalysis = viaV2(Hist());
        double historicalBa = Hist().Trees.Where(t => t.Living && historicalAnalysis.Contains(t.X, t.Z)).Sum(t => Math.PI * Math.Pow(t.DbhCm / 200.0, 2.0));
        Check(historicalAnalysis.Geometry.Contains(0, lines[2] + 3f) && historicalFootprint == 69 && Near(historicalBa, 1.29490, 1e-5) && historicalAnalysis.TeachingRackId == 3
            && historicalAnalysis.RackIndex == 2 && historicalAnalysis.RackCount == 5 && ScenarioOneRackBayText.AreaName(historicalAnalysis) == "the middle rack",
            "historical fixture through the V2 entry: 69 trees, 1.29490 m2; V2 areas are named 'the middle rack'", historicalFootprint + " trees " + historicalBa.ToString("R", CultureInfo.InvariantCulture));

        // ---------------- J. the new V2 footprint on the core fixture (offline prediction; Unity measures the live world) ----------------
        var v2 = q; // qualified geometry
        var v2Frame = v2.Teaching;
        Func<Stand, ScenarioOneRackBay.Analysis> viaQualified = s => ScenarioOneRackBay.AnalyseTeachingRack(s.Trees, s.Orders, v2Frame, v2.TeachingRack.Id, v2.Racks.Count, v2.TeachingRack.Index);
        var fresh = LoadCore(root, QualifiedZ); fresh.Analyse = () => viaQualified(fresh);
        var footprintTrees = fresh.Trees.Where(t => t.Living && v2Frame.Contains(t.X, t.Z)).ToList();
        double footprintBa = footprintTrees.Sum(t => Math.PI * Math.Pow(t.DbhCm / 200.0, 2.0));
        Console.WriteLine("V2_FIXTURE_OFFLINE footprint=" + footprintTrees.Count + " trees, BA=" + footprintBa.ToString("R", CultureInfo.InvariantCulture)
            + " m2 (historical 69 trees, 1.29490 m2); anchor " + q.Anchor + " half-width 9 m; rack 3 corridor excluded");
        Check(footprintTrees.Count > 69 && footprintTrees.All(t => !v2Frame.InCorridor(t.X, t.Z)), "9.0 m footprint holds more living trees than the 8.004 m historical one and none from a rack corridor", footprintTrees.Count + " trees");
        // One Crop Tree per zone, both sides, chosen the way a player would: well apart in the footprint.
        (double along, double across)[] targets = { (-8, 4.5), (0, -4.5), (8, 4.5) };
        foreach (var target in targets)
        {
            var pick = footprintTrees.OrderBy(t => Math.Pow(v2Frame.Along(t.X, t.Z) - target.along, 2) + Math.Pow(v2Frame.Across(t.X, t.Z) - target.across, 2)).First();
            fresh.Crop(pick.Id);
        }
        var selected = viaQualified(fresh);
        Check(selected.HasEnoughFuture && selected.NearSelected && selected.MiddleSelected && selected.FarSelected && selected.BothSides && selected.TooClose.Count == 0,
            "three Crop Trees (near/middle/far, both sides) at 8 m spacing qualify in the V2 footprint", string.Join(",", selected.Future.Select(t => t.Id)));
        bool spacingOk = true;
        for (int i = 0; i < selected.Future.Count; i++) for (int j = i + 1; j < selected.Future.Count; j++)
            spacingOk &= Math.Sqrt(Math.Pow(selected.Future[i].X - selected.Future[j].X, 2) + Math.Pow(selected.Future[i].Z - selected.Future[j].Z, 2)) >= ScenarioOneRackBay.FutureTreeSpacingM;
        Check(spacingOk, "the three picks are at least 8 m apart");
        PlanInitial(fresh);
        var released = viaQualified(fresh);
        Console.WriteLine("V2_FIXTURE_OFFLINE_CALIBRATION initial removals=" + released.PlannedRemovals + " BA share=" + released.RemovedBasalShare.ToString("R", CultureInfo.InvariantCulture)
            + " min=" + released.MinPlannedAndExecuted.ToString("R", CultureInfo.InvariantCulture));
        Check(released.ReleasePlanned && released.PlannedRemovals >= 10 && released.CanopyKept && released.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare,
            "30% initial release and 40% safeguard remain feasible in the V2 footprint", "removals " + released.PlannedRemovals + " BA " + F(released.RemovedBasalShare));
        fresh.Execute(2);
        var executed = viaQualified(fresh);
        Check(executed.ReleaseExecuted && executed.CanopyKept && executed.ExecutedRemovals == released.PlannedRemovals, "executed release is credited inside the V2 footprint only");
        // Felling that is not release: rack-corridor stumps never count as release, whichever rack they belong to.
        var access = LoadCore(root, QualifiedZ);
        Check(access.Orders.Count > 0 && viaQualified(access).ExecutedRemovals == 0 && viaQualified(access).RemovedBasalShare == 0f, "completed rack-corridor felling (the access work) earns no release credit and does not enter the safeguard");

        Console.WriteLine((failures == 0 ? "NONUI_RACKSAMPLE_PASS checks=" : "NONUI_RACKSAMPLE_FAIL failures=" + failures + " checks=") + checks);
        return failures == 0 ? 0 : 1;
    }
}
