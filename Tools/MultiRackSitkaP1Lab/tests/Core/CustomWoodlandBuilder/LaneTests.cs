using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

internal static class LaneTests
{
    public static void Register()
    {
        TestRunner.Add("Lanes", "LANE-01 five equal lanes tile an 80 x 60 field", EqualLanes);
        TestRunner.Add("Lanes", "LANE-02 clipped lanes stay inside a tapered wedge", ClippedInsideWedge);
        TestRunner.Add("Lanes", "LANE-03 the lane count is limited to 2-8", LaneCountBounds);
        TestRunner.Add("Lanes", "LANE-04 a narrow arm lane refuses the whole plan", NarrowArmRefused);
        TestRunner.Add("Lanes", "LANE-05 a lane below the minimum area is refused", SmallAreaRefused);
        TestRunner.Add("Lanes", "LANE-06 recipe validation reports the failing lane", ValidationReportsLane);
    }

    private static void EqualLanes()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(WoodlandShapeSpec.Rectangle(80.0, 60.0));
        LanePlan plan = LaneClipper.Clip(polygon, 5);

        TestRunner.True(plan.IsValid, "five 16 x 12 m bands are usable");
        TestRunner.Eq(5, plan.LaneCount, "lane count");
        TestRunner.ApproxEq(12.0, plan.BandHeightM, 1e-12, "band height");
        TestRunner.True(plan.FailedLaneIndex is null, "no lane failed");

        double previousSouth = double.NegativeInfinity;
        for (int index = 0; index < plan.Lanes.Count; index++)
        {
            WorkLane lane = plan.Lanes[index];
            TestRunner.Eq(index, lane.Index, "lane indices are ordered");
            TestRunner.Eq(4, lane.Boundary.Count, "a clipped rectangle band keeps four vertices");
            TestRunner.ApproxEq(80.0, lane.MeanWidthM, 1e-9, "lane mean width");
            TestRunner.ApproxEq(960.0, lane.AreaM2, 1e-9, "lane area");
            TestRunner.True(lane.MinZ > previousSouth, "lanes advance north");
            previousSouth = lane.MinZ;
        }

        TestRunner.ApproxEq(polygon.AreaM2, plan.TotalAreaM2, 1e-6, "the lanes tile the whole field");
        TestRunner.ApproxEq(0.0, plan.Lanes[0].MinZ, 1e-9, "lane 0 starts at the southern boundary");
        TestRunner.ApproxEq(60.0, plan.Lanes[4].MaxZ, 1e-9, "lane 4 ends at the northern boundary");
        TestRunner.Eq(2, plan.LaneIndexAt(new PlanPoint(40.0, 25.0)), "a point in the third band maps to lane 2");
        TestRunner.Eq(0, plan.LaneIndexAt(new PlanPoint(40.0, 0.0)), "the southern boundary belongs to lane 0");
        TestRunner.Eq(4, plan.LaneIndexAt(new PlanPoint(40.0, 60.0)), "the northern boundary belongs to the last lane");
    }

    private static void ClippedInsideWedge()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.TaperedWedge(100.0, 40.0, 90.0));
        LanePlan plan = LaneClipper.Clip(polygon, 4);

        TestRunner.True(plan.IsValid, "four bands of a 100 -> 40 m wedge are usable");
        TestRunner.ApproxEq(polygon.AreaM2, plan.TotalAreaM2, 1e-6, "clipped lanes keep the full area");

        foreach (WorkLane lane in plan.Lanes)
        {
            TestRunner.True(lane.Boundary.Count >= 3, "each lane has real geometry");
            foreach (PlanPoint vertex in lane.Boundary)
            {
                TestRunner.True(
                    polygon.ContainsPoint(vertex),
                    $"lane {lane.Index} vertex {vertex} is inside the field boundary");
            }

            foreach (PlanPoint vertex in lane.Boundary)
            {
                TestRunner.True(
                    vertex.Z >= lane.MinZ - 1e-9 && vertex.Z <= lane.MaxZ + 1e-9,
                    "every lane vertex stays inside its band");
            }
        }
    }

    private static void LaneCountBounds()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(WoodlandShapeSpec.Rectangle(80.0, 60.0));
        TestRunner.Throws<ArgumentOutOfRangeException>(
            () => LaneClipper.Clip(polygon, 1), "one lane is below the minimum");
        TestRunner.Throws<ArgumentOutOfRangeException>(
            () => LaneClipper.Clip(polygon, 9), "nine lanes are above the maximum");
    }

    private static void NarrowArmRefused()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.LShape(100.0, 100.0, armWidthM: 6.0, armDepthM: 20.0));
        LanePlan plan = LaneClipper.Clip(polygon, 4);

        TestRunner.True(!plan.IsValid, "a 6 m wide arm cannot carry a work lane");
        TestRunner.Eq(1, plan.FailedLaneIndex, "the first arm-only band fails");
        TestRunner.Eq(LaneRejectionReason.BelowMinimumWidth, plan.Rejection, "the reason is a narrow lane");
        TestRunner.ApproxEq(6.0, plan.Lanes[1].MeanWidthM, 1e-9, "the failing lane really is 6 m wide");
    }

    private static void SmallAreaRefused()
    {
        WoodlandPolygon polygon = WoodlandPolygon.FromShape(
            WoodlandShapeSpec.LShape(40.0, 40.0, armWidthM: 10.0, armDepthM: 10.0));
        LanePlan plan = LaneClipper.Clip(polygon, 8);

        TestRunner.True(!plan.IsValid, "a 50 m2 arm band is below the minimum lane area");
        TestRunner.Eq(2, plan.FailedLaneIndex, "the first column-only band fails");
        TestRunner.Eq(LaneRejectionReason.BelowMinimumArea, plan.Rejection, "the reason is too little area");
        TestRunner.True(plan.Lanes[2].AreaM2 < LaneClipper.MinimumLaneAreaM2, "the band area is below the threshold");
    }

    private static void ValidationReportsLane()
    {
        CustomWoodlandRecipe recipe = Fixtures.Recipe(
            boundary: WoodlandShapeSpec.LShape(100.0, 100.0, armWidthM: 6.0, armDepthM: 20.0),
            laneCount: 4,
            pattern: MoisturePatternSpec.Uniform(SiteMoistureClass.FreshMoistMineral));

        IReadOnlyList<string> errors = CustomWoodlandBuilder.Validate(recipe);
        TestRunner.True(errors.Count > 0, "validation reports the unusable lane");
        TestRunner.True(
            errors.Any(error => error.Contains("not a usable work lane", StringComparison.Ordinal)),
            "the message names the lane problem");

        TestRunner.Throws<ArgumentException>(
            () => CustomWoodlandBuilder.Build(recipe), "the build fails closed instead of dropping the lane");
    }
}
