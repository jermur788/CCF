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
    public readonly struct Point
    {
        public readonly string Id;
        public readonly float X, Z;
        public Point(string id, float x, float z) { Id = id ?? ""; X = x; Z = z; }
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
        // Ids of authoritative protected-area polygons. Scenario One has no live producer; any entry makes coverage refuse (see Map).
        public readonly List<string> ProtectedAreaIds = new List<string>();
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
        // A protected area is a polygon and the coverage contract places a feature at one point. Nothing may sample it or silently omit it,
        // so an authoritative polygon makes the whole read fail closed until a faithful polygon rule is accepted.
        if (source.ProtectedAreaIds.Count > 0)
        {
            problem = "Protected area " + source.ProtectedAreaIds[0] + " exists but coverage has no accepted rule for a protected-area polygon.";
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
}
