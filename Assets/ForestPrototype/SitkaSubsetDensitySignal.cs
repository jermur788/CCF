using System;
using System.Collections.Generic;

// The LEGACY SITKA COMPONENT of mixed-stand density mortality (growth model 2, D-059).
//
// It runs the original growth-model-1 Sitka process on the Sitka (Reference class) stems ALONE: British size-density
// relative density, density pressure, the suppression-weighted annual hazard, then the boundary that removes the most
// suppressed survivors until the Sitka subset is back on the maximum line. Every number comes from SitkaGrowthModel;
// nothing here is a new Sitka rule. Only Reference stems enter the relative density, so an Oak or Beech stem never
// contributes to a Sitka density.
//
// The victims it returns are Sitka stems. That is the point of this component: the deaths the Sitka density process
// demands are paid by the Sitka stems that process is about, exactly as if the broadleaves were not in the stand,
// so a broadleaf can neither shield a Sitka stem nor be drawn on to pay a Sitka debt. For an all-Sitka stand the
// victims are the ones the original controller block chooses (checked against a literal port in the offline suite).
//
// The per-tree roll is the same keyed domain the original block uses ("ADULT-MORT-" + tree id), so a Sitka stem draws
// the same number it would have drawn in a pure Sitka stand that year.
public sealed class SitkaComponentResult
{
    // Reference stems in the stand (the subset the process saw).
    public int SubsetCount;
    // British Sitka relative density and pressure of the SUBSET only (stems per hectare of the whole stand area).
    public double RelativeDensity;
    public double Pressure;
    // Indices into the stem list the caller passed. Hazard victims are the first stage, boundary victims the second.
    public readonly List<int> HazardVictimIndices = new List<int>();
    public readonly List<int> BoundaryVictimIndices = new List<int>();

    public int Deaths => HazardVictimIndices.Count + BoundaryVictimIndices.Count;
}

public static class SitkaSubsetDensitySignal
{
    // Ci50 of the Sitka asset. DensityStem carries a raw Hegyi index, not a species, so the Reference suppression
    // uses this one value (the offline suite checks it against SitkaSpruce.asset).
    public const double SitkaCi50 = 5.0;

    public static SitkaComponentResult Resolve(IReadOnlyList<DensityStem> stems, float areaHectares,
        Func<string, double> roll, double referenceCi50 = SitkaCi50)
    {
        var result = new SitkaComponentResult();
        if (areaHectares <= 0f) return result;

        // Reference stems in ascending ordinal tree-id order (the order every deterministic sum uses).
        var subset = new List<int>();
        for (int i = 0; i < stems.Count; i++)
            if (stems[i].Demand == GrowingSpaceDemandClass.Reference)
                subset.Add(i);
        subset.Sort((a, b) =>
        {
            int byId = string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
            return byId != 0 ? byId : a.CompareTo(b);
        });
        result.SubsetCount = subset.Count;
        if (subset.Count == 0) return result;

        double squares = 0.0, vulnerabilitySum = 0.0;
        var suppression = new Dictionary<int, double>(subset.Count);
        foreach (int i in subset)
        {
            DensityStem stem = stems[i];
            squares += (double)stem.DbhCm * stem.DbhCm;
            // float arithmetic on purpose: the original block divides the float competition index by the float Ci50.
            double s = 1.0 - 1.0 / (1.0 + (float)stem.CompetitionIndex / (float)referenceCi50);
            suppression[i] = s;
            vulnerabilitySum += SitkaGrowthModel.Vulnerability(s);
        }
        double meanVulnerability = vulnerabilitySum / subset.Count;
        result.RelativeDensity = SitkaGrowthModel.RelativeDensity(subset.Count / areaHectares, Math.Sqrt(squares / subset.Count));
        result.Pressure = SitkaGrowthModel.DensityPressure(result.RelativeDensity);
        if (result.Pressure <= 0.0) return result;

        var survivors = new List<int>(subset.Count);
        foreach (int i in subset)
        {
            double probability = SitkaGrowthModel.AnnualDeathProbability(result.Pressure, suppression[i], meanVulnerability);
            if (probability > 0.0 && roll(stems[i].TreeId) < probability) result.HazardVictimIndices.Add(i);
            else survivors.Add(i);
        }

        // Boundary: the subset may not stay above the maximum line. Most suppressed survivors first, ties by tree id.
        squares = 0.0;
        foreach (int i in survivors) squares += (double)stems[i].DbhCm * stems[i].DbhCm;
        int count = survivors.Count;
        if (SitkaGrowthModel.RelativeDensity(count / areaHectares, Math.Sqrt(squares / Math.Max(1, count))) <= SitkaGrowthModel.MaximumRelativeDensity)
            return result;
        survivors.Sort((a, b) =>
        {
            int bySuppression = suppression[b].CompareTo(suppression[a]);
            return bySuppression != 0 ? bySuppression : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
        });
        foreach (int i in survivors)
        {
            double rd = SitkaGrowthModel.RelativeDensity(count / areaHectares, Math.Sqrt(squares / count));
            if (rd <= SitkaGrowthModel.MaximumRelativeDensity || count <= 1) break;
            count--;
            squares -= (double)stems[i].DbhCm * stems[i].DbhCm;
            result.BoundaryVictimIndices.Add(i);
        }
        return result;
    }

    // The number of deaths the Sitka density process demands of the Sitka subset. Kept for callers and checks that only
    // need the count.
    public static int RequiredDeaths(IReadOnlyList<DensityStem> stems, float areaHectares,
        Func<string, double> roll, double referenceCi50 = SitkaCi50)
    {
        return Resolve(stems, areaHectares, roll, referenceCi50).Deaths;
    }
}
