// Offline check for the forest track's approach route (StandApproachRoute.cs): the track continues the authored track's south end smoothly,
// runs to the south property edge and a little beyond, is cut into the planks the starting stand and the stand map read, and clears only
// the one core tree the replica of the generator says it must. Pure maths on plain .NET; the Unity gate StandApproachVerification.cs
// checks the planks, the ridge opening, the map and the stand in the live game.
//
// The core trees are the 336 Legacy40 trees reproduced from the generator (core-stand-replica.csv). The authored track end is plank
// "Dirt Path 00" in ForestTest.unity: position (-0.928509, -14.75), turned 2 * atan2(0.23488314, 0.9720236) about Y; plank 01 is at (-0.67, -14.25).
//
// Prints NONUI_APPROACH_PASS / NONUI_APPROACH_FAIL.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public static class StandApproachOfflineCheck
{
    private const float JoinX = -0.928509f, JoinZ = -14.75f;
    private static readonly float PlankYaw = 2f * (float)Math.Atan2(0.23488314, 0.9720236);
    private const float EdgeZ = -40f;
    private const float Corridor = 1.5f;     // ForestStartingStand.pathCorridorRadiusMeters

    private static int failures, checks;

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    private static float Dist(float ax, float az, float bx, float bz) => (float)Math.Sqrt((ax - bx) * (ax - bx) + (az - bz) * (az - bz));
    private static string F(float v) => v.ToString("0.00", CultureInfo.InvariantCulture);

    public static int Run(string root)
    {
        List<StandApproachRoute.Point> route = StandApproachRoute.Build(JoinX, JoinZ, PlankYaw);
        Shape(route);
        Seams(route);
        CoreTrees(root, route);
        Console.WriteLine(failures == 0 ? $"NONUI_APPROACH_PASS checks={checks}" : $"NONUI_APPROACH_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Shape(List<StandApproachRoute.Point> route)
    {
        Check(route.Count > 40, "the route has planks to place", "n=" + route.Count);
        Check(route[0].X == JoinX && route[0].Z == JoinZ, "it starts exactly at the authored track's south end");

        // Continues the authored direction: from plank 01 (-0.67, -14.25) to plank 00 the authored track runs south-south-west.
        float authored = (float)Math.Atan2(JoinX - -0.67f, JoinZ - -14.25f);
        float first = (float)Math.Atan2(route[1].X - route[0].X, route[1].Z - route[0].Z);
        Check(Math.Abs(authored - first) < 2.5f * (float)Math.PI / 180f, "the first step goes the way the authored track already runs (no kink at the join)",
            "authored " + F(authored * 57.29578f) + " deg, route " + F(first * 57.29578f) + " deg");

        bool southward = true, evenSteps = true;
        float maxTurn = 0f;
        for (int i = 1; i < route.Count; i++)
        {
            if (!(route[i].Z < route[i - 1].Z)) southward = false;
            float step = Dist(route[i].X, route[i].Z, route[i - 1].X, route[i - 1].Z);
            if (Math.Abs(step - StandApproachRoute.StepM) > 0.01f) evenSteps = false;
            maxTurn = Math.Max(maxTurn, Math.Abs(route[i].BearingRadians - route[i - 1].BearingRadians));
        }
        Check(southward, "every plank is further south than the one before");
        Check(evenSteps, "planks are " + StandApproachRoute.StepM + " m apart all along, so the 0.66 m planks leave no gaps");
        // A bend of radius R turns by StepM / R per step.
        float radius = maxTurn > 0f ? StandApproachRoute.StepM / maxTurn : float.MaxValue;
        Check(radius >= 9f, "the bend is never tighter than about 9 m radius (a track, not a corner)", "radius " + F(radius) + " m");
        float firstTurn = Math.Abs(route[2].BearingRadians - route[1].BearingRadians);
        Check(firstTurn < 0.02f, "the bend starts straight, so the join with the authored planks has no kink", "first turn " + F(firstTurn * 57.29578f) + " deg");
        Check(Math.Abs(route[route.Count - 1].BearingRadians - (float)Math.PI) < 1e-4f, "it finishes running due south");
        Check(route[route.Count - 1].Z <= StandApproachRoute.EndZ && route[route.Count - 1].Z > StandApproachRoute.EndZ - StandApproachRoute.StepM - 0.01f,
            "it ends at the far end of the cutting", "z " + F(route[route.Count - 1].Z));

        float gate = StandApproachRoute.GateCentreX(route, EdgeZ);
        var atEdge = route.Where(p => p.Z <= EdgeZ).First();
        Check(Math.Abs(atEdge.X - gate) < 0.3f, "the opening is centred where the track crosses the edge", "x " + F(gate));
        Check(gate - StandApproachRoute.GateWidthM * 0.5f > -41f && gate + StandApproachRoute.GateWidthM * 0.5f < 41f, "the opening lies within the south ridge");
        Check(StandApproachRoute.GateWidthM >= 2.6f + 2f, "the opening is wider than the 2.6 m track with room either side");
    }

    // The planks inside the property take the "Dirt Path X" names the starting stand and map read; the rest are scenery beyond the edge.
    private static void Seams(List<StandApproachRoute.Point> route)
    {
        var inside = route.Where(p => StandApproachRoute.InsideProperty(p, EdgeZ)).ToList();
        var outside = route.Where(p => !StandApproachRoute.InsideProperty(p, EdgeZ)).ToList();
        Check(inside.Count + outside.Count == route.Count, "every plank is either inside the property or in the cutting");
        Check(inside.Count > 0 && inside[inside.Count - 1].Z >= EdgeZ && outside.Count > 0 && outside[0].Z < EdgeZ, "the inside planks end at the edge and the cutting planks begin beyond it");
        Check(outside.Count >= 10, "the cutting holds some track beyond the ridge", "n=" + outside.Count);

        // One line on the stand map: the map starts a new line when two planks are more than 2 m apart, and names sort in route order.
        bool oneLine = true;
        for (int i = 1; i < inside.Count; i++)
            if (Dist(inside[i].X, inside[i].Z, inside[i - 1].X, inside[i - 1].Z) > 2f) oneLine = false;
        Check(oneLine, "the inside planks join into one line for the stand map");

        var names = inside.Select((p, i) => "Dirt Path X" + i.ToString("D3", CultureInfo.InvariantCulture)).ToList();
        var sorted = names.OrderBy(n => n, StringComparer.Ordinal).ToList();
        Check(names.SequenceEqual(sorted), "the plank names sort in route order (ordinal, as the map sorts)");
        Check(names.All(n => string.CompareOrdinal(n, "Dirt Path 51") > 0 && n.StartsWith("Dirt Path", StringComparison.Ordinal)),
            "they sort after the authored 'Dirt Path 00'..'51' and carry the prefix the generator and map read");
        Check(inside.Count < 1000, "three digits are enough for the plank numbers");
    }

    // What the generator would do to the 336 Legacy40 core trees if it cleared 1.5 m round every inside plank.
    private static void CoreTrees(string root, List<StandApproachRoute.Point> route)
    {
        string path = Path.Combine(root, "Tools", "Verification", "ScenarioOneNonUi", "OfflineLogic", "core-stand-replica.csv");
        var trees = new List<(string id, float x, float z)>();
        foreach (string line in File.ReadAllLines(path).Skip(1))
        {
            string[] c = line.Split(',');
            trees.Add((c[0], float.Parse(c[1], CultureInfo.InvariantCulture), float.Parse(c[2], CultureInfo.InvariantCulture)));
        }
        Check(trees.Count == 336, "the core replica has the 336 Legacy40 trees", "n=" + trees.Count);

        var planks = route.Where(p => StandApproachRoute.InsideProperty(p, EdgeZ)).ToList();
        var cleared = trees.Where(t => planks.Any(p => p.Z < JoinZ && Dist(t.x, t.z, p.X, p.Z) <= Corridor)).Select(t => t.id).OrderBy(i => i, StringComparer.Ordinal).ToList();
        Check(cleared.Count == 1 && cleared[0] == "P0009", "the extension clears exactly one core tree, P0009 (the generator then restores one more, P1200, to keep 336)",
            "cleared " + string.Join(",", cleared));
        float nearest = trees.Where(t => !cleared.Contains(t.id)).Min(t => planks.Where(p => p.Z < JoinZ).Min(p => Dist(t.x, t.z, p.X, p.Z)));
        Check(nearest > Corridor + 0.1f, "every other core tree stands clear of the new track by more than the corridor", "nearest " + F(nearest) + " m");
    }
}
