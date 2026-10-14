using System;
using System.Collections.Generic;

public static partial class MixedDensityModel
{
    // [C] smooth occupancy response, not a broadleaf RD or maximum density.
    public const double CompositeResponseStrength = 0.20;
    public const double CompositeMaximumGenericLoss = 0.25;

    public static double GenericCompositeLoss(double occupancy)
    {
        double excess = Math.Max(0.0, occupancy - 1.0);
        return CompositeMaximumGenericLoss * (1.0 - Math.Exp(-CompositeResponseStrength * excess * excess));
    }

    // Reference deaths are a signal from the unchanged Sitka subset calculation, never a
    // broadleaf size-density equation. Generic pressure vanishes continuously with its
    // non-Reference occupied-space share. One combined quota, one neutral allocator.
    public static DensityMortalityPlan PlanComposite(IReadOnlyList<DensityStem> stems,
        float areaHectares, int referenceDeaths, Func<string, double> roll)
    {
        if (referenceDeaths < 0 || referenceDeaths > stems.Count)
            throw new ArgumentOutOfRangeException(nameof(referenceDeaths));
        var plan = new DensityMortalityPlan();
        if (stems.Count == 0 || areaHectares <= 0f) return plan;
        int[] order = CanonicalOrder(stems);
        double occupied = 0.0, nonReference = 0.0, squares = 0.0;
        foreach (int i in order)
        {
            double space = StemOccupiedSpace(stems[i]); occupied += space;
            if (stems[i].Demand != GrowingSpaceDemandClass.Reference) nonReference += space;
            squares += (double)stems[i].DbhCm * stems[i].DbhCm;
        }
        plan.StemsPerHectare = stems.Count / areaHectares;
        plan.QuadraticMeanDbhCm = Math.Sqrt(squares / stems.Count);
        plan.Occupancy = OccupancyOf(occupied, areaHectares, OccupancyCalibration.Default);
        double generic = GenericCompositeLoss(plan.Occupancy) * (occupied > 0.0 ? nonReference / occupied : 0.0);
        double reference = (double)referenceDeaths / stems.Count;
        plan.ExpectedAnnualLoss = 1.0 - (1.0 - reference) * (1.0 - generic);
        plan.OvercrowdingPressure = plan.ExpectedAnnualLoss;
        // Floor plus one keyed Bernoulli handles fractional expected counts without a
        // permanent ceiling bias. The reference signal is already an integer minimum.
        double count = plan.ExpectedAnnualLoss * stems.Count;
        int quota = (int)Math.Floor(count + 1e-10);
        double remainder = Math.Max(0.0, count - quota);
        if (remainder > 0.0 && roll("COMPOSITE-QUOTA") < remainder) quota++;
        quota = Math.Min(stems.Count, Math.Max(referenceDeaths, quota));
        if (quota == 0) return plan;
        double totalVulnerability = 0.0;
        foreach (int i in order) totalVulnerability += Vulnerability(Suppression(stems[i].CompetitionIndex));
        var priority = new Dictionary<int, double>();
        foreach (int i in order)
        {
            double weight = totalVulnerability > 0.0 ? Vulnerability(Suppression(stems[i].CompetitionIndex)) : 1.0;
            double u = Math.Max(1e-15, Math.Min(1.0 - 1e-15, roll(stems[i].TreeId)));
            priority[i] = weight > 0.0 ? -Math.Log(u) / weight : double.PositiveInfinity;
        }
        Array.Sort(order, (a,b) => { int c = priority[a].CompareTo(priority[b]); return c != 0 ? c : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId); });
        for (int i = 0; i < quota; i++) plan.VictimIndices.Add(order[i]);
        plan.VictimIndices.Sort((a,b) => string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId));
        plan.HazardDeaths = plan.VictimIndices.Count;
        return plan;
    }
}
