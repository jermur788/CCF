using System;
using System.Collections.Generic;

// Adapter to EXISTING Sitka numerics: only Reference stems enter this calculation.
// The signal includes the original hazard and boundary, preventing a last-broadleaf
// route cliff. Mixed allocation happens elsewhere and never reads a species Ci50.
public static class SitkaSubsetDensitySignal
{
    public static int RequiredDeaths(IReadOnlyList<DensityStem> stems, float areaHectares,
        Func<string,double> roll, double referenceCi50 = 5.0)
    {
        if (areaHectares <= 0f) return 0;
        var subset = new List<DensityStem>();
        foreach (var stem in stems) if (stem.Demand == GrowingSpaceDemandClass.Reference) subset.Add(stem);
        subset.Sort((a,b) => string.CompareOrdinal(a.TreeId,b.TreeId));
        if (subset.Count == 0) return 0;
        double squares = 0.0, vulnerabilitySum = 0.0;
        var suppression = new Dictionary<string,double>();
        foreach (var stem in subset)
        {
            squares += (double)stem.DbhCm * stem.DbhCm;
            double s = 1.0 - 1.0 / (1.0 + stem.CompetitionIndex / referenceCi50);
            suppression[stem.TreeId] = s; vulnerabilitySum += SitkaGrowthModel.Vulnerability(s);
        }
        double rd = SitkaGrowthModel.RelativeDensity(subset.Count / areaHectares, Math.Sqrt(squares / subset.Count));
        double pressure = SitkaGrowthModel.DensityPressure(rd);
        if (pressure <= 0.0) return 0;
        var survivors = new List<DensityStem>(); int deaths = 0;
        foreach (var stem in subset)
        {
            double probability = SitkaGrowthModel.AnnualDeathProbability(pressure, suppression[stem.TreeId], vulnerabilitySum / subset.Count);
            if (probability > 0.0 && roll(stem.TreeId) < probability) deaths++; else survivors.Add(stem);
        }
        squares = 0.0;
        foreach (var stem in survivors) squares += (double)stem.DbhCm * stem.DbhCm;
        survivors.Sort((a,b) => { int c = suppression[b.TreeId].CompareTo(suppression[a.TreeId]);return c != 0 ? c : string.CompareOrdinal(a.TreeId,b.TreeId); });
        int count = survivors.Count;
        foreach (var stem in survivors)
        {
            if (count <= 1 || SitkaGrowthModel.RelativeDensity(count / areaHectares, Math.Sqrt(squares / count)) <= SitkaGrowthModel.MaximumRelativeDensity) break;
            count--; squares -= (double)stem.DbhCm * stem.DbhCm; deaths++;
        }
        return deaths;
    }
}
