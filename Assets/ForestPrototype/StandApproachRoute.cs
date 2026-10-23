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

    // The clearance the starting stand keeps round every "Dirt Path" plank (ForestStartingStand.pathCorridorRadiusMeters, 1.5 m in the scene).
    public const float StandCorridorM = 1.5f;

    // The starting stand's own trees, living or not: "P" + 4 digits (the Legacy40 core) or "PO" + 4 digits (the Enlarged80 outer plantation).
    // Later recruitment, planting and fixtures use other ids.
    public static bool IsOriginalPlantationId(string id)
    {
        if (string.IsNullOrEmpty(id) || id[0] != 'P') return false;
        int start = id.Length == 6 && id[1] == 'O' ? 2 : 1;
        if (id.Length - start != 4) return false;
        for (int i = start; i < id.Length; i++) if (id[i] < '0' || id[i] > '9') return false;
        return true;
    }

    // Was this world planted WITH the approach? The starting stand clears every plantation tree off the track, so a world made with it has
    // no original plantation tree record (living, dead or stump: the game keeps them all) within the corridor of the extension. A world
    // made before the approach existed was planted straight across that line, so it has some. The record is read, not stored: no save
    // field is needed, and an old save keeps the world it was made in. Only the extension counts (planks north of the join were always
    // there), and only inside the property.
    public static bool WorldHasApproach(IReadOnlyList<Point> route, float edgeZ, float joinZ, IEnumerable<(string id, float x, float z)> trees)
    {
        if (trees == null) return true;
        foreach (var tree in trees)
        {
            if (!IsOriginalPlantationId(tree.id)) continue;
            for (int i = 0; i < route.Count; i++)
            {
                Point p = route[i];
                if (p.Z >= joinZ || p.Z < edgeZ) continue;
                float dx = tree.x - p.X, dz = tree.z - p.Z;
                if (dx * dx + dz * dz <= StandCorridorM * StandCorridorM) return false;
            }
        }
        return true;
    }

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
