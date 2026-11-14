namespace SitkaLab.Core;

/// <summary>Stand arithmetic for eligible living adults (live and DBH >= threshold). Computed only here — the browser never recomputes it.</summary>
public sealed record StandMetrics(
    decimal AreaM2,
    decimal AreaHa,
    int EligibleLiveN,
    double NPerHa,
    double BaM2,
    double BaM2PerHa,
    double? DqCm,
    int LiveTotal,
    int LiveBelowThreshold,
    int Dead,
    int Felled);

public sealed record RemovalPreview(
    bool Valid,
    IReadOnlyList<string> InvalidIds,
    string? InvalidReason,
    int RemovedN,
    double RemovedBaM2,
    double? RemovedBaFractionOfLiveStand,
    StandMetrics Before,
    StandMetrics After);

public static class Metrics
{
    /// <summary>BA of one stem in m²: pi * (dbhCm / 200)^2.</summary>
    public static double BasalAreaM2(long dbhMm)
    {
        double dbhCm = dbhMm / 10.0;
        double r = dbhCm / 200.0;
        return Math.PI * r * r;
    }

    public static StandMetrics Compute(ForestSnapshot s, ISet<string>? hypotheticallyRemoved = null)
    {
        decimal areaM2 = Geometry.AreaM2(s.Polygon);
        decimal areaHa = areaM2 / 10_000m;
        int n = 0, liveTotal = 0, below = 0, dead = 0, felled = 0;
        double ba = 0, sumDbhCm2 = 0;
        foreach (var t in s.Trees) // ordinal ID order => fixed summation order => deterministic doubles
        {
            switch (t.Status)
            {
                case TreeStatus.Dead: dead++; continue;
                case TreeStatus.Felled: felled++; continue;
            }
            if (hypotheticallyRemoved is not null && hypotheticallyRemoved.Contains(t.TreeId)) continue;
            liveTotal++;
            if (t.DbhMm < s.AdultThresholdMm) { below++; continue; }
            n++;
            ba += BasalAreaM2(t.DbhMm);
            double cm = t.DbhMm / 10.0;
            sumDbhCm2 += cm * cm;
        }
        double haD = (double)areaHa;
        return new StandMetrics(
            areaM2, areaHa, n,
            n / haD, ba, ba / haD,
            n == 0 ? null : Math.Sqrt(sumDbhCm2 / n),
            liveTotal, below, dead, felled);
    }

    /// <summary>Removal preview that never mutates the forest. Any ineligible or duplicate ID makes the preview invalid.</summary>
    public static RemovalPreview PreviewRemoval(ForestSnapshot s, IReadOnlyList<string> ids)
    {
        var before = Compute(s);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var bad = new List<string>();
        string? reason = null;
        foreach (var id in ids)
        {
            if (!seen.Add(id)) { bad.Add(id); reason ??= "TreeDuplicate"; continue; }
            var t = s.FindTree(id);
            if (t is null) { bad.Add(id); reason ??= "TreeMissing"; }
            else if (t.Status != TreeStatus.Live) { bad.Add(id); reason ??= "TreeNotLive"; }
            else if (t.DbhMm < s.AdultThresholdMm) { bad.Add(id); reason ??= "TreeBelowThreshold"; }
        }
        if (ids.Count == 0) reason ??= "EmptySelection";
        if (bad.Count > 0 || ids.Count == 0)
            return new RemovalPreview(false, bad, reason, 0, 0, null, before, before);

        var after = Compute(s, seen);
        // Removed BA is summed directly over the removed stems (ordinal order) rather than by subtraction.
        double direct = 0;
        foreach (var t in s.Trees)
            if (seen.Contains(t.TreeId)) direct += BasalAreaM2(t.DbhMm);
        double? fraction = before.BaM2 > 0 ? direct / before.BaM2 : null;
        return new RemovalPreview(true, bad, null, seen.Count, direct, fraction, before, after);
    }
}
