using System;
using System.Collections.Generic;

// Where the forest track runs on its way in from the property edge (the Enlarged80 stand only; see StandApproachPath).
//
// The authored track (planks "Dirt Path 00".."51" in the scene) starts at its south end in the middle of the plantation. For the 80 m
// property that is about 25 m from the south edge, so the track seemed to begin out of nothing. This continues it south from that end,
// bending gently from the direction the authored planks leave in to due south, then straight to the edge and a few metres beyond it.
//
// Plain numbers, no UnityEngine, so the route can be checked on plain .NET (Tools/Verification/ScenarioOneNonUi/OfflineLogic).
// Positions are metres: x east, z north. A bearing is the angle from +z towards +x, as a plank's Y rotation is.
public static class StandApproachRoute
{
    // Distance between plank centres. The planks are 0.66 m long, so neighbours overlap and the track has no gaps on the bends.
    public const float StepM = 0.5f;
    // The track eases from the authored direction round to due south over this length, starting straight and ending straight, so the
    // join with the authored planks has no kink. At its tightest the bend is about 10 m radius, like the authored track's own curves.
    public const float BendLengthM = 7f;
    // The route ends this far before the far wall of the opening beyond the edge, so the track does not run into the rock.
    public const float EndZ = -48.5f;
    // The opening in the south boundary ridge, wide enough for the 2.6 m track with room either side.
    public const float GateWidthM = 6f;
    // How far the opening reaches past the property edge (the cutting the track runs into), and the ridge thickness.
    public const float ForecourtLengthM = 10f;
    public const float RidgeThicknessM = 1f;

    public readonly struct Point
    {
        public readonly float X, Z, BearingRadians;
        public Point(float x, float z, float bearing) { X = x; Z = z; BearingRadians = bearing; }
    }

    // The track continued from the authored south end (x, z) whose plank is turned plankYawRadians (the direction the authored track
    // runs, towards the clearing), going away from the clearing until it passes endZ. The first point is the authored end itself.
    public static List<Point> Build(float joinX, float joinZ, float plankYawRadians)
    {
        var points = new List<Point>();
        // Away from the clearing: half a turn from the way the plank runs. Due south is then a short bend away.
        float start = plankYawRadians + (float)Math.PI;
        float south = (float)Math.PI;
        float turn = south - start;
        float x = joinX, z = joinZ, travelled = 0f;
        points.Add(new Point(x, z, start));
        int guard = 0;
        while (z > EndZ && guard++ < 1000)
        {
            travelled += StepM;
            float t = Math.Min(1f, travelled / BendLengthM);
            float bearing = start + turn * (t * t * (3f - 2f * t));
            x += (float)Math.Sin(bearing) * StepM;
            z += (float)Math.Cos(bearing) * StepM;
            points.Add(new Point(x, z, bearing));
        }
        return points;
    }

    // The planks inside the property (the stand map draws these and the starting stand keeps its trees clear of them) are the ones at
    // or north of the edge; the rest lie in the opening beyond it.
    public static bool InsideProperty(Point p, float edgeZ) => p.Z >= edgeZ;

    // x where the route crosses the south property edge: the centre of the opening.
    public static float GateCentreX(IReadOnlyList<Point> route, float edgeZ)
    {
        for (int i = 1; i < route.Count; i++)
            if (route[i].Z <= edgeZ)
            {
                Point a = route[i - 1], b = route[i];
                float t = (a.Z - edgeZ) / Math.Max(1e-6f, a.Z - b.Z);
                return a.X + (b.X - a.X) * t;
            }
        return route.Count > 0 ? route[route.Count - 1].X : 0f;
    }
}
