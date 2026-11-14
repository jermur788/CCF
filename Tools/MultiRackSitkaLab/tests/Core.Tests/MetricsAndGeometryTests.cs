using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

public static class MetricsAndGeometryTests
{
    // ---- hand-computed oracle: 0.25 ha, live stems 20/30/40 cm -------------------------------------------------
    [Test]
    public static void Oracle_BasalAreaDensityDq_MatchHandCalculation()
    {
        var m = Fx.Oracle().ActualMetrics();
        T.Eq(2500m, m.AreaM2, "area m2 exact");
        T.Eq(0.25m, m.AreaHa, "area ha exact");
        T.Eq(3, m.EligibleLiveN, "eligible N");
        T.Eq(12.0, m.NPerHa, "N/ha");
        T.Near(0.227765, m.BaM2, 1e-6, "BA = pi*(0.01+0.0225+0.04)");
        T.Near(0.911062, m.BaM2PerHa, 1e-6, "BA/ha");
        T.Near(31.0913, m.DqCm!.Value, 1e-4, "Dq = sqrt((400+900+1600)/3) cm");
    }

    [Test]
    public static void Oracle_RemovingTheFortyCmStem_BaRemovalMatchesHand()
    {
        var s = Fx.Oracle();
        var p = s.Preview(new[] { "T003" });
        T.True(p.Valid, "preview valid");
        T.Near(0.125664, p.RemovedBaM2, 1e-6, "removal BA = pi*0.04");
        T.Near(0.04 / 0.0725, p.RemovedBaFractionOfLiveStand!.Value, 1e-12, "fraction of live stand BA");
        T.Eq(2, p.After.EligibleLiveN, "N after");
        T.Near(Math.Sqrt((400.0 + 900.0) / 2), p.After.DqCm!.Value, 1e-9, "Dq after");
    }

    [Test]
    public static void Preview_NeverMutatesTheForest()
    {
        var s = Fx.Oracle();
        string before = Fx.Snapshot(s);
        long events = s.Events.Count;
        s.Preview(new[] { "T001", "T002" });
        s.Preview(new[] { "T001", "T001" });
        T.Eq(before, Fx.Snapshot(s), "snapshot unchanged by previews");
        T.Eq(events, (long)s.Events.Count, "no events from previews");
        T.Eq(3, s.ActualMetrics().EligibleLiveN, "live count unchanged");
    }

    [Test]
    public static void Preview_AllSubsets_EqualSumOfStems()
    {
        var s = Fx.Oracle();
        string[] ids = { "T001", "T002", "T003" };
        double[] ba = { Math.PI * 0.01, Math.PI * 0.0225, Math.PI * 0.04 };
        for (int mask = 1; mask < 8; mask++)
        {
            var pick = Enumerable.Range(0, 3).Where(i => (mask & (1 << i)) != 0).ToList();
            var p = s.Preview(pick.Select(i => ids[i]).ToList());
            T.True(p.Valid, "subset valid");
            T.Near(pick.Sum(i => ba[i]), p.RemovedBaM2, 1e-12, $"subset {mask} removed BA");
            T.Near(0.227765467 - pick.Sum(i => ba[i]), p.After.BaM2, 1e-8, $"subset {mask} remaining BA");
        }
    }

    [Test]
    public static void Preview_RejectsDeadFelledBelowThresholdDuplicateAndMissing()
    {
        var snap = Fx.Square(50_000,
            Fx.Tree("A", 1000, 1000, 300), Fx.Tree("D", 2000, 2000, 300, TreeStatus.Dead),
            Fx.Tree("F", 3000, 3000, 300, TreeStatus.Felled), Fx.Tree("S", 4000, 4000, 90));
        var s = LabSession.ForTestingUnvalidated(snap, "aaaaaaaaaaaaaaaa");
        T.Eq("TreeNotLive", s.Preview(new[] { "D" }).InvalidReason, "dead");
        T.Eq("TreeNotLive", s.Preview(new[] { "F" }).InvalidReason, "felled");
        T.Eq("TreeBelowThreshold", s.Preview(new[] { "S" }).InvalidReason, "below threshold");
        T.Eq("TreeDuplicate", s.Preview(new[] { "A", "A" }).InvalidReason, "duplicate");
        T.Eq("TreeMissing", s.Preview(new[] { "Z" }).InvalidReason, "missing");
        T.Eq("EmptySelection", s.Preview(Array.Empty<string>()).InvalidReason, "empty");
        T.True(s.Preview(new[] { "A" }).Valid, "valid single");
    }

    [Test]
    public static void Metrics_ExcludeDeadFelledAndBelowThreshold_ButCountThemSeparately()
    {
        var snap = Fx.Square(50_000,
            Fx.Tree("A", 1000, 1000, 300), Fx.Tree("B", 2000, 2000, 100),   // exactly at threshold: counts
            Fx.Tree("C", 3000, 3000, 99), Fx.Tree("D", 4000, 4000, 500, TreeStatus.Dead), Fx.Tree("E", 5000, 5000, 500, TreeStatus.Felled));
        var m = Metrics.Compute(snap);
        T.Eq(2, m.EligibleLiveN, "A and B only (threshold inclusive)");
        T.Near(Math.PI * (0.15 * 0.15 + 0.05 * 0.05), m.BaM2, 1e-12, "BA of A+B");
        T.Eq(1, m.LiveBelowThreshold, "below-threshold live");
        T.Eq(1, m.Dead, "dead"); T.Eq(1, m.Felled, "felled"); T.Eq(3, m.LiveTotal, "live total");
        T.Near(Math.Sqrt((30.0 * 30 + 10.0 * 10) / 2), m.DqCm!.Value, 1e-12, "Dq");
    }

    [Test]
    public static void Dq_IsQuadraticMean_NotArithmeticMean()
    {
        var snap = Fx.Square(50_000, Fx.Tree("A", 1000, 1000, 100), Fx.Tree("B", 2000, 2000, 500));
        double dq = Metrics.Compute(snap).DqCm!.Value;
        T.Near(Math.Sqrt((100.0 + 2500.0) / 2), dq, 1e-12, "Dq");
        T.True(Math.Abs(dq - 30.0) > 5, "differs from the arithmetic mean 30 cm");
    }

    [Test]
    public static void ZeroEligibleAdults_GivesDefinedNAndNotApplicableDq()
    {
        var snap = Fx.Square(50_000, Fx.Tree("A", 1000, 1000, 50), Fx.Tree("B", 2000, 2000, 400, TreeStatus.Dead));
        var m = Metrics.Compute(snap);
        T.Eq(0, m.EligibleLiveN, "N"); T.Eq(0.0, m.NPerHa, "N/ha"); T.Eq(0.0, m.BaM2, "BA");
        T.True(m.DqCm is null, "Dq not applicable");
        T.True(Metrics.Compute(Fx.Square(50_000)).DqCm is null, "empty stand Dq not applicable");
        T.Eq(0, Metrics.PreviewRemoval(snap, new[] { "A" }).RemovedN, "nothing removable");
    }

    // ---- geometry -----------------------------------------------------------------------------------------------
    [Test]
    public static void Area_IsExactAndIndependentOfWinding()
    {
        var ccw = Fx.WithPolygon((0, 0), (100_000, 0), (0, 60_000));
        var cw = Fx.WithPolygon((0, 0), (0, 60_000), (100_000, 0));
        T.Eq(3000m, Geometry.AreaM2(ccw.Polygon), "triangle 100 x 60 / 2 m2");
        T.Eq(3000m, Geometry.AreaM2(cw.Polygon), "reversed winding same area");
        T.True(Geometry.ValidatePolygon(cw.Polygon) is null && Geometry.ValidatePolygon(ccw.Polygon) is null, "both windings valid");
        T.Eq(0.5m, Geometry.AreaM2(new List<Vertex> { new(0, 0), new(1000, 0), new(0, 1000) }), "half-integer mm2 area stays exact");
    }

    [Test]
    public static void Area_SkewedAndConcavePolygons_MatchIndependentShoelace()
    {
        // L-shape: 100x100 square minus a 50x50 corner => 7500 m2
        var l = Fx.WithPolygon((0, 0), (100_000, 0), (100_000, 50_000), (50_000, 50_000), (50_000, 100_000), (0, 100_000));
        T.True(Geometry.ValidatePolygon(l.Polygon) is null, "L-shape is a valid simple ring");
        T.Eq(7500m, Geometry.AreaM2(l.Polygon), "L-shape area");
        T.False(Geometry.ContainsInclusive(l.Polygon, 75_000, 75_000), "notch is outside");
        T.True(Geometry.ContainsInclusive(l.Polygon, 25_000, 75_000), "arm is inside");
        // demo fixture: independent decimal shoelace over the five vertices (hand-checkable: 5812.5 m2)
        var demo = Fx.Catalog.TryGet("sitka-flat-demo-v1")!.Snapshot;
        decimal sum = 0; var v = demo.Polygon;
        for (int i = 0; i < v.Count; i++) { var a = v[i]; var b = v[(i + 1) % v.Count]; sum += (decimal)a.XMm * b.YMm - (decimal)b.XMm * a.YMm; }
        T.Eq(Math.Abs(sum) / 2m / 1_000_000m, Geometry.AreaM2(v), "demo area equals independent shoelace");
    }

    [Test]
    public static void Boundary_StemOnEdgeAndVertexIsInside_OutsideIsNot()
    {
        var sq = Fx.WithPolygon((0, 0), (50_000, 0), (50_000, 50_000), (0, 50_000)).Polygon;
        T.True(Geometry.ContainsInclusive(sq, 0, 25_000), "on west edge");
        T.True(Geometry.ContainsInclusive(sq, 50_000, 50_000), "on a vertex");
        T.True(Geometry.ContainsInclusive(sq, 25_000, 0), "on south edge");
        T.True(Geometry.ContainsInclusive(sq, 1, 1), "just inside");
        T.False(Geometry.ContainsInclusive(sq, -1, 25_000), "1 mm outside west");
        T.False(Geometry.ContainsInclusive(sq, 50_001, 25_000), "1 mm outside east");
        var skew = Fx.WithPolygon((0, 0), (80_000, 0), (95_000, 60_000), (30_000, 75_000), (0, 50_000)).Polygon;
        T.True(Geometry.ContainsInclusive(skew, 0, 25_000), "demo west boundary");
        T.True(Geometry.ContainsInclusive(skew, 87_500, 30_000), "on the skewed east edge (80000,0)-(95000,60000)");
        T.False(Geometry.ContainsInclusive(skew, 87_501, 30_000), "1 mm beyond the skewed edge");
    }

    [Test]
    public static void InvalidRings_AreRejected()
    {
        void Bad(string why, params (long, long)[] pts) => T.True(Geometry.ValidatePolygon(Fx.WithPolygon(pts).Polygon) is not null, why + " must be rejected");
        Bad("bow-tie", (0, 0), (50_000, 50_000), (50_000, 0), (0, 50_000));
        Bad("fewer than 3 vertices", (0, 0), (1000, 1000));
        Bad("collinear (zero area)", (0, 0), (1000, 0), (2000, 0));
        Bad("repeated vertex", (0, 0), (1000, 0), (1000, 1000), (1000, 0), (0, 1000));
        Bad("closing vertex repeated", (0, 0), (1000, 0), (1000, 1000), (0, 0));
        Bad("spike folding back", (0, 0), (10_000, 0), (5_000, 0), (5_000, 5_000));
        Bad("figure-eight", (0, 0), (10_000, 0), (0, 10_000), (10_000, 10_000));
        Bad("self-touching (vertex on non-adjacent edge)", (0, 0), (10_000, 0), (10_000, 10_000), (5_000, 0), (5_000, 5_000), (0, 10_000));
        Bad("coordinate beyond range", (0, 0), (LabConstants.MaxMm + 1, 0), (0, 1000));
        T.True(Geometry.ValidatePolygon(Enumerable.Range(0, 300).Select(i => new Vertex(i * 1000, i * i)).ToList()) is not null, "too many vertices");
    }

    [Test]
    public static void ExtremeCoordinates_DoNotOverflow()
    {
        long m = LabConstants.MaxMm;
        var big = Fx.WithPolygon((-m, -m), (m, -m), (m, m), (-m, m));
        T.True(Geometry.ValidatePolygon(big.Polygon) is null, "maximal square valid");
        T.Eq(4_000_000_000_000m, Geometry.AreaM2(big.Polygon), "2,000 km x 2,000 km = 4e12 m2");
        T.True(Geometry.ContainsInclusive(big.Polygon, m, m), "corner stem");
    }
}
