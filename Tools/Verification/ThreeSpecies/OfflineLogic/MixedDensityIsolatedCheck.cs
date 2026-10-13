// Compile-time proof that the species-aware occupancy route cannot use Sitka density arithmetic.
//
// This is built with MixedDensityModel.cs ALONE: SitkaGrowthModel.cs is deliberately not in the build. If the
// occupancy model referenced SitkaGrowthModel (RelativeDensity, DensityPressure, Vulnerability,
// MaximumRelativeDensity, ...) this project would fail to compile. Success therefore means the occupancy route has
// no access to the British Sitka size-density line, hazard or maximum line.
using System;
using System.Collections.Generic;

public static class MixedDensityIsolatedCheck
{
    // string.GetHashCode is randomised per process in .NET, so use a fixed hash to keep the printed numbers reproducible.
    private static double KeyedRoll(string id)
    {
        uint hash = 2166136261u;
        foreach (char c in id) hash = unchecked((hash ^ c) * 16777619u);
        return (hash & 0xFFFFFF) / 16777216.0;
    }

    public static int Main(string[] args)
    {
        var stems = new List<DensityStem>();
        for (int i = 0; i < 600; i++)
            stems.Add(new DensityStem("O" + i.ToString("D4"), i % 2 == 0 ? GrowingSpaceDemandClass.High : GrowingSpaceDemandClass.Intermediate, 30f, 1.0 + i % 5));
        DensityMortalityPlan plan = MixedDensityModel.Plan(stems, 0.64f, KeyedRoll);
        bool ok = plan.Occupancy > 1.0 && plan.VictimIndices.Count > 0 && plan.OccupancyAfter <= 1.0 + 1e-12
            && MixedDensityModel.SelectRoute(true, true) == DensityRoute.SpeciesAwareOccupancy;
        Console.WriteLine(ok
            ? $"MIXED_DENSITY_ISOLATED_PASS compiledWithoutSitkaGrowthModel=true occupancy={plan.Occupancy:0.0000} victims={plan.VictimIndices.Count}"
            : "MIXED_DENSITY_ISOLATED_FAIL");
        return ok ? 0 : 1;
    }
}
