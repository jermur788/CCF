namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

internal static class BoundaryTests
{
    public static void Register()
    {
        TestRunner.Add("Boundary", "BOUND-01 rectangle geometry is exact and counter-clockwise", RectangleGeometry);
        TestRunner.Add("Boundary", "BOUND-02 clockwise input is normalised to counter-clockwise", ClockwiseNormalised);
        TestRunner.Add("Boundary", "BOUND-03 L-shape leaves the expected 5,800 m2 footprint", LShapeGeometry);
        TestRunner.Add("Boundary", "BOUND-04 tapered wedge area follows the mean width", TaperedWedgeGeometry);
        TestRunner.Add("Boundary", "BOUND-05 angled stream edge keeps its parallelogram area", AngledStreamEdgeGeometry);
        TestRunner.Add("Boundary", "BOUND-06 corner chamfers cut the corners and keep the ring simple", ChamferApplied);
        TestRunner.Add("Boundary", "BOUND-07 corner chamfers are bounded and validated", ChamferBounds);
        TestRunner.Add("Boundary", "BOUND-08 self-intersection and edge touching are refused", SelfIntersectionRefused);
        TestRunner.Add("Boundary", "BOUND-09 footprint, area and vertex-count limits are refused", SizeLimitsRefused);
        TestRunner.Add("Boundary", "BOUND-10 degenerate rings are refused", DegenerateRingRefused);
    }

    private static void RectangleGeometry()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(WoodlandShapeSpec.Rectangle(80.0, 60.0));

        TestRunner.Eq(4, polygon.Vertices.Count, "a rectangle has four vertices");
        TestRunner.ApproxEq(4800.0, polygon.AreaM2, 1e-9, "80 x 60 m area");
        TestRunner.ApproxEq(0.48, polygon.AreaHectares, 1e-12, "area in hectares");
        TestRunner.True(Fixtures.IsCounterClockwise(polygon), "the ring is counter-clockwise");
        TestRunner.ApproxEq(0.0, polygon.MinX, 1e-9, "min X");
        TestRunner.ApproxEq(80.0, polygon.MaxX, 1e-9, "max X");
        TestRunner.ApproxEq(60.0, polygon.LengthM, 1e-9, "north-south extent");
        TestRunner.ApproxEq(40.0, polygon.Centroid.X, 1e-9, "centroid X");
        TestRunner.ApproxEq(30.0, polygon.Centroid.Z, 1e-9, "centroid Z");

        TestRunner.ApproxEq(0.0, polygon.Vertices[0].X, 1e-9, "canonical start X");
        TestRunner.ApproxEq(0.0, polygon.Vertices[0].Z, 1e-9, "canonical start Z");

        TestRunner.True(polygon.ContainsPoint(new PlanPoint(40.0, 30.0)), "the centre is inside");
        TestRunner.True(polygon.ContainsPoint(new PlanPoint(0.0, 30.0)), "a boundary point counts as inside");
        TestRunner.True(!polygon.ContainsPoint(new PlanPoint(-0.5, 30.0)), "a point outside is outside");
        TestRunner.ApproxEq(0.0, polygon.DistanceToBoundary(new PlanPoint(0.0, 30.0)), 1e-9, "edge distance is zero on the edge");
        TestRunner.ApproxEq(10.0, polygon.DistanceToBoundary(new PlanPoint(10.0, 30.0)), 1e-9, "edge distance inside");
    }

    private static void ClockwiseNormalised()
    {
        var clockwise = new[]
        {
            new PlanPoint(0.0, 0.0),
            new PlanPoint(0.0, 60.0),
            new PlanPoint(80.0, 60.0),
            new PlanPoint(80.0, 0.0),
        };

        TestRunner.True(WoodlandPolygon.SignedAreaTwice(clockwise) < 0.0, "the fixture really is clockwise");

        WoodlandPolygon polygon = WoodlandPolygon.FromVertices(clockwise);
        TestRunner.True(Fixtures.IsCounterClockwise(polygon), "the built ring is counter-clockwise");
        TestRunner.ApproxEq(4800.0, polygon.AreaM2, 1e-9, "area survives normalisation");
        TestRunner.ApproxEq(0.0, polygon.Vertices[0].X, 1e-9, "canonical start X after reversal");
        TestRunner.ApproxEq(0.0, polygon.Vertices[0].Z, 1e-9, "canonical start Z after reversal");
    }

    private static void LShapeGeometry()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.LShape(100.0, 100.0, armWidthM: 40.0, armDepthM: 30.0));

        TestRunner.Eq(6, polygon.Vertices.Count, "an L-shape has six vertices");
        TestRunner.ApproxEq(5800.0, polygon.AreaM2, 1e-9, "bottom bar 100x30 plus column 40x70");
        TestRunner.True(Fixtures.IsCounterClockwise(polygon), "the ring is counter-clockwise");

        TestRunner.True(polygon.ContainsPoint(new PlanPoint(90.0, 10.0)), "the bottom bar is inside");
        TestRunner.True(polygon.ContainsPoint(new PlanPoint(10.0, 90.0)), "the column is inside");
        TestRunner.True(!polygon.ContainsPoint(new PlanPoint(90.0, 90.0)), "the missing bay is outside");
        TestRunner.ApproxEq(100.0, polygon.WidthM, 1e-9, "footprint width");
        TestRunner.ApproxEq(100.0, polygon.LengthM, 1e-9, "footprint depth");
    }

    private static void TaperedWedgeGeometry()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.TaperedWedge(southWidthM: 80.0, northWidthM: 40.0, depthM: 90.0));

        TestRunner.Eq(4, polygon.Vertices.Count, "a wedge has four vertices");
        TestRunner.ApproxEq(5400.0, polygon.AreaM2, 1e-9, "(80 + 40) / 2 x 90");
        TestRunner.ApproxEq(80.0, polygon.WidthM, 1e-9, "the south edge sets the footprint width");
        TestRunner.ApproxEq(90.0, polygon.LengthM, 1e-9, "depth");
        TestRunner.True(polygon.ContainsPoint(new PlanPoint(0.0, 5.0)), "the wide south end is inside");
        TestRunner.True(!polygon.ContainsPoint(new PlanPoint(38.0, 88.0)), "the narrow north end is outside near its shoulder");
    }

    private static void AngledStreamEdgeGeometry()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.AngledStreamEdge(widthM: 70.0, depthM: 50.0, streamSlantM: 20.0));

        TestRunner.Eq(4, polygon.Vertices.Count, "a parallelogram keeps four vertices");
        TestRunner.ApproxEq(3500.0, polygon.AreaM2, 1e-9, "70 x 50 m area");
        TestRunner.ApproxEq(90.0, polygon.WidthM, 1e-9, "the slant widens the footprint");

        bool hasSlantedNorthEdge = false;
        for (int i = 0; i < polygon.Vertices.Count; i++)
        {
            PlanPoint a = polygon.Vertices[i];
            PlanPoint b = polygon.Vertices[(i + 1) % polygon.Vertices.Count];
            if (Math.Abs(a.Z - 50.0) < 1e-9 && Math.Abs(b.Z - 50.0) < 1e-9)
                hasSlantedNorthEdge = true;
        }

        TestRunner.True(hasSlantedNorthEdge, "both north vertices sit on the far headland");
        TestRunner.True(polygon.ContainsPoint(new PlanPoint(50.0, 25.0)), "the middle of the field is inside");
    }

    private static void ChamferApplied()
    {
        WoodlandPolygon sharp = WoodlandPolygon.FromShape(WoodlandShapeSpec.Rectangle(80.0, 60.0));
        WoodlandPolygon chamfered = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.Rectangle(80.0, 60.0, CornerAdjustments.All(5.0)));

        TestRunner.Eq(4, sharp.Vertices.Count, "a sharp rectangle keeps four vertices");
        TestRunner.Eq(8, chamfered.Vertices.Count, "four chamfers add four vertices");
        TestRunner.ApproxEq(4750.0, chamfered.AreaM2, 1e-9, "four 5 m right triangles are removed");
        TestRunner.True(Fixtures.IsCounterClockwise(chamfered), "the chamfered ring stays counter-clockwise");
        TestRunner.True(chamfered.AreaM2 < sharp.AreaM2, "the chamfer removes material");

        foreach (PlanPoint vertex in chamfered.Vertices)
        {
            TestRunner.True(
                chamfered.ContainsPoint(vertex),
                "every chamfered vertex is on the new boundary, so containment must accept it");
        }

        TestRunner.True(
            !chamfered.ContainsPoint(new PlanPoint(0.2, 0.2)),
            "the original sharp corner is now outside the chamfered field");

        WoodlandPolygon zero = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.Rectangle(80.0, 60.0, CornerAdjustments.None));
        TestRunner.Eq(4, zero.Vertices.Count, "zero adjustments keep the sharp corners");
    }

    private static void ChamferBounds()
    {
        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(
                WoodlandShapeSpec.Rectangle(80.0, 60.0, CornerAdjustments.All(30.0))),
            "a chamfer of half the shorter adjacent edge would meet the next corner");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(
                WoodlandShapeSpec.Rectangle(80.0, 60.0, CornerAdjustments.All(-1.0))),
            "a negative chamfer is refused");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(
                WoodlandShapeSpec.Rectangle(80.0, 60.0, new CornerAdjustments(double.NaN, 0.0, 0.0, 0.0))),
            "a non-finite chamfer is refused");

        WoodlandPolygon nearLimit = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.Rectangle(80.0, 60.0, new CornerAdjustments(0.0, 0.0, 29.9, 0.0)));
        TestRunner.Eq(5, nearLimit.Vertices.Count, "a chamfer just inside the limit is accepted");
    }

    private static void SelfIntersectionRefused()
    {
        var bowTie = new[]
        {
            new PlanPoint(0.0, 0.0),
            new PlanPoint(80.0, 60.0),
            new PlanPoint(80.0, 0.0),
            new PlanPoint(0.0, 60.0),
        };
        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(bowTie), "a bow-tie ring is refused");

        var vertexOnEdge = new[]
        {
            new PlanPoint(0.0, 0.0),
            new PlanPoint(80.0, 0.0),
            new PlanPoint(80.0, 60.0),
            new PlanPoint(40.0, 0.0),
            new PlanPoint(0.0, 60.0),
        };
        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(vertexOnEdge),
            "a vertex touching a non-adjacent edge is refused");

        var spike = new[]
        {
            new PlanPoint(0.0, 0.0),
            new PlanPoint(80.0, 0.0),
            new PlanPoint(40.0, 0.0),
            new PlanPoint(80.0, 60.0),
            new PlanPoint(0.0, 60.0),
        };
        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(spike), "a doubled-back edge is refused");
    }

    private static void SizeLimitsRefused()
    {
        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(WoodlandShapeSpec.Rectangle(20.0, 60.0)),
            "a 20 m footprint is below the 30 m minimum");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(WoodlandShapeSpec.Rectangle(130.0, 60.0)),
            "a 130 m footprint is above the 120 m maximum");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(WoodlandShapeSpec.LShape(30.0, 30.0, armWidthM: 5.0, armDepthM: 5.0)),
            "a 275 m2 L-shape is below the 450 m2 minimum");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(WoodlandShapeSpec.LShape(80.0, 60.0, armWidthM: 80.0, armDepthM: 20.0)),
            "an L-shape arm covering the whole width is refused");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromShape(WoodlandShapeSpec.AngledStreamEdge(70.0, 50.0, streamSlantM: 71.0)),
            "a stream slant beyond the field width is refused");
    }

    private static void DegenerateRingRefused()
    {
        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(new[] { new PlanPoint(0.0, 0.0), new PlanPoint(60.0, 60.0) }),
            "two vertices are not a polygon");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(new[]
            {
                new PlanPoint(0.0, 0.0),
                new PlanPoint(80.0, 0.0),
                new PlanPoint(80.0, 60.0),
                new PlanPoint(0.0, 60.0),
                new PlanPoint(0.0, 0.0),
            }),
            "a repeated closing vertex is refused rather than silently stripped");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(new[]
            {
                new PlanPoint(0.0, 0.0),
                new PlanPoint(0.0, 0.0),
                new PlanPoint(80.0, 0.0),
                new PlanPoint(0.0, 60.0),
            }),
            "a zero-length edge is refused");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(new[]
            {
                new PlanPoint(0.0, 0.0),
                new PlanPoint(double.NaN, 0.0),
                new PlanPoint(0.0, 60.0),
            }),
            "a non-finite vertex is refused");

        TestRunner.Throws<ArgumentException>(
            () => WoodlandPolygon.FromVertices(SeventeenVertexSquare()),
            "more than sixteen vertices are refused");
    }

    private static PlanPoint[] SeventeenVertexSquare()
    {
        // Three extra collinear points per edge would already exceed the cap; this keeps the ring simple.
        var ring = new List<PlanPoint>();
        for (int i = 0; i < 17; i++)
        {
            double t = i / 17.0 * 4.0;
            if (t < 1.0)
                ring.Add(new PlanPoint(t * 60.0, 0.0));
            else if (t < 2.0)
                ring.Add(new PlanPoint(60.0, (t - 1.0) * 60.0));
            else if (t < 3.0)
                ring.Add(new PlanPoint(60.0 - (t - 2.0) * 60.0, 60.0));
            else
                ring.Add(new PlanPoint(0.0, 60.0 - (t - 3.0) * 60.0));
        }

        return ring.ToArray();
    }
}
