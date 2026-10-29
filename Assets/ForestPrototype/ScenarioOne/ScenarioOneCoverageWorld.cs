using System;
using System.Collections.Generic;

// MFM-08A: maps already-authoritative world state into the pure ScenarioOneCoverage.Input masks and features. Plain data, no UnityEngine,
// so the mapping is checked on plain .NET (Tools/Verification/MfmCoverageWorld). It reads and decides nothing: the caller
// (ScenarioOneManager.PrescriptionApproval.cs) gathers the Source from the live scene and manager; this class only turns it into masks.
//
// It invents no geometry. Every position, id and radius arrives in the Source from an existing authoritative owner:
//   Headland   ScenarioOneHeadlandGeometry constants (same strip/pad/stem-guard rule as its ContainsStemCentre)
//   Track      the scene's active "Dirt Path*" planks, each a circle of ForestStartingStand.PathCorridorRadiusMeters
//   Clearing   the "Forest Clearing" position, a circle of ForestStartingStand.ClearingRadiusMeters (not its visual scale)
//   Site       every ForestBuildable position, a circle of ForestStartingStand.ClearingRadiusMeters. The player's start / live position is
//              NOT a site: it is a starting-stand-generation-only exclusion and would otherwise move coverage with the player.
public static class ScenarioOneCoverageWorld
{
    // A protected area is a polygon but the coverage contract places a feature at one point. Each polygon is therefore presented as its
    // vertices plus points along its edges and a lattice inside it at this spacing, all carrying the area's own id. This is the one
    // sampling resolution of the adapter (a polygon thinner than this and lying between lattice points is still caught via its edges).
    public const double ProtectedAreaSampleStepM = 1.0;

    public readonly struct Point
    {
        public readonly string Id;
        public readonly float X, Z;
        public Point(string id, float x, float z) { Id = id ?? ""; X = x; Z = z; }
    }

    public sealed class Polygon
    {
        public string Id = "";
        public readonly List<Point> Vertices = new List<Point>();
    }

    public sealed class Headland
    {
        public double HeadlandX, SouthZ, NorthZ, FeederZ, PathX, Width, PadRadius, StemGuard;
    }

    public sealed class Source
    {
        public double PathCorridorRadiusM, ClearanceRadiusM;
        public double StandHalfExtentM;
        public Headland Headland;
        public readonly List<Point> PathPlanks = new List<Point>();
        public bool HasClearing;
        public Point Clearing;
        public readonly List<Point> Sites = new List<Point>();

        public readonly List<Point> PlantedJuveniles = new List<Point>();
        public readonly List<Point> Shelters = new List<Point>();
        public readonly List<Point> ClearancePatches = new List<Point>();
        public readonly List<Polygon> ProtectedAreas = new List<Polygon>();
        public readonly List<Point> DeadwoodRecords = new List<Point>();
        public readonly List<Point> ActiveManagementWork = new List<Point>();
        public readonly List<Point> NaturalRegeneration = new List<Point>();
    }

    public sealed class Mapped
    {
        public readonly List<ScenarioOneCoverage.Mask> Masks = new List<ScenarioOneCoverage.Mask>();
        public readonly List<ScenarioOneCoverage.Feature> Features = new List<ScenarioOneCoverage.Feature>();
    }

    // Null source or a missing/invalid radius is a refusal (null result + reason): coverage must never silently run without an exclusion.
    public static Mapped Map(Source source, out string problem)
    {
        problem = "";
        if (source == null) { problem = "No world source was supplied."; return null; }
        if (source.Headland == null) { problem = "The headland geometry is missing."; return null; }
        if (!(source.PathCorridorRadiusM > 0) || !(source.ClearanceRadiusM > 0))
        {
            problem = "The starting-stand clearance radii are not available.";
            return null;
        }
        var mapped = new Mapped();
        foreach (Point p in source.PathPlanks)
            mapped.Masks.Add(ScenarioOneCoverage.Mask.Circle(ScenarioOneCoverage.MaskKind.Track, p.X, p.Z, source.PathCorridorRadiusM));
        if (source.HasClearing)
            mapped.Masks.Add(ScenarioOneCoverage.Mask.Circle(ScenarioOneCoverage.MaskKind.Clearing, source.Clearing.X, source.Clearing.Z, source.ClearanceRadiusM));
        foreach (Point p in source.Sites)
            mapped.Masks.Add(ScenarioOneCoverage.Mask.Circle(ScenarioOneCoverage.MaskKind.Site, p.X, p.Z, source.ClearanceRadiusM));
        AddHeadland(mapped, source.Headland);

        Add(mapped, ScenarioOneCoverage.FeatureKind.PlantedJuvenile, source.PlantedJuveniles);
        Add(mapped, ScenarioOneCoverage.FeatureKind.Shelter, source.Shelters);
        Add(mapped, ScenarioOneCoverage.FeatureKind.ClearancePatch, source.ClearancePatches);
        foreach (Polygon polygon in source.ProtectedAreas)
            foreach (Point p in SamplePolygon(polygon, source.StandHalfExtentM))
                mapped.Features.Add(new ScenarioOneCoverage.Feature(ScenarioOneCoverage.FeatureKind.ProtectedArea, polygon.Id, p.X, p.Z));
        Add(mapped, ScenarioOneCoverage.FeatureKind.DeadwoodRecord, source.DeadwoodRecords);
        Add(mapped, ScenarioOneCoverage.FeatureKind.ActiveManagementWork, source.ActiveManagementWork);
        Add(mapped, ScenarioOneCoverage.FeatureKind.NaturalRegeneration, source.NaturalRegeneration);
        return mapped;
    }

    private static void Add(Mapped mapped, ScenarioOneCoverage.FeatureKind kind, List<Point> points)
    {
        foreach (Point p in points) mapped.Features.Add(new ScenarioOneCoverage.Feature(kind, p.Id, p.X, p.Z));
    }

    private static void AddHeadland(Mapped mapped, Headland h)
    {
        double strip = h.Width * 0.5 + h.StemGuard, pad = h.PadRadius + h.StemGuard;
        mapped.Masks.Add(new ScenarioOneCoverage.Mask(ScenarioOneCoverage.MaskKind.Headland, h.HeadlandX, h.SouthZ, h.HeadlandX, h.NorthZ, strip));
        mapped.Masks.Add(new ScenarioOneCoverage.Mask(ScenarioOneCoverage.MaskKind.Headland, h.HeadlandX, h.FeederZ, h.PathX, h.FeederZ, strip));
        mapped.Masks.Add(ScenarioOneCoverage.Mask.Circle(ScenarioOneCoverage.MaskKind.Headland, h.HeadlandX, h.FeederZ, pad));
        mapped.Masks.Add(ScenarioOneCoverage.Mask.Circle(ScenarioOneCoverage.MaskKind.Headland, h.PathX, h.FeederZ, pad));
    }

    // Edge points (starting at each vertex) and an interior lattice (world-grid aligned, so it does not depend on vertex order), kept inside the stand.
    public static List<Point> SamplePolygon(Polygon polygon, double halfExtent)
    {
        var points = new List<Point>();
        if (polygon == null || polygon.Vertices.Count < 3) return points;
        List<Point> v = polygon.Vertices;
        double minX = double.MaxValue, maxX = double.MinValue, minZ = double.MaxValue, maxZ = double.MinValue;
        for (int i = 0; i < v.Count; i++)
        {
            minX = Math.Min(minX, v[i].X); maxX = Math.Max(maxX, v[i].X);
            minZ = Math.Min(minZ, v[i].Z); maxZ = Math.Max(maxZ, v[i].Z);
            Point a = v[i], b = v[(i + 1) % v.Count];
            int steps = Math.Max(1, (int)Math.Ceiling(Math.Sqrt((double)(b.X - a.X) * (b.X - a.X) + (double)(b.Z - a.Z) * (b.Z - a.Z)) / ProtectedAreaSampleStepM));
            for (int s = 0; s < steps; s++)
            {
                double t = s / (double)steps;
                double x = a.X + (b.X - a.X) * t, z = a.Z + (b.Z - a.Z) * t;
                if (Math.Abs(x) <= halfExtent && Math.Abs(z) <= halfExtent) points.Add(new Point(polygon.Id, (float)x, (float)z));
            }
        }
        double x0 = Math.Max(Math.Ceiling(minX / ProtectedAreaSampleStepM), Math.Ceiling(-halfExtent / ProtectedAreaSampleStepM)) * ProtectedAreaSampleStepM;
        double z0 = Math.Max(Math.Ceiling(minZ / ProtectedAreaSampleStepM), Math.Ceiling(-halfExtent / ProtectedAreaSampleStepM)) * ProtectedAreaSampleStepM;
        double x1 = Math.Min(maxX, halfExtent), z1 = Math.Min(maxZ, halfExtent);
        for (double x = x0; x <= x1 + 1e-9; x += ProtectedAreaSampleStepM)
            for (double z = z0; z <= z1 + 1e-9; z += ProtectedAreaSampleStepM)
                if (Inside(v, x, z)) points.Add(new Point(polygon.Id, (float)x, (float)z));
        return points;
    }

    private static bool Inside(List<Point> v, double x, double z)
    {
        bool inside = false;
        for (int i = 0, j = v.Count - 1; i < v.Count; j = i++)
            if ((v[i].Z > z) != (v[j].Z > z) && x < (v[j].X - v[i].X) * (z - v[i].Z) / (v[j].Z - v[i].Z) + v[i].X)
                inside = !inside;
        return inside;
    }
}
