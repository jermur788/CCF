using System;
using System.Collections.Generic;

// One tree offered to a Crop Tree's competition breakdown. Living neighbours
// form the current competition; non-living entries are recorded removals that
// only the lesson's executed-release account uses.
public readonly struct CompetitionSample
{
    public readonly string Id;
    public readonly float X, Z;
    public readonly float DbhCm;
    public readonly bool Living;
    public readonly bool PlannedFell;
    public readonly bool InFootprint;

    public CompetitionSample(string id, float x, float z, float dbhCm, bool living, bool plannedFell, bool inFootprint)
    {
        Id = id ?? "";
        X = x; Z = z;
        DbhCm = dbhCm;
        Living = living;
        PlannedFell = plannedFell;
        InFootprint = inFootprint;
    }
}

public readonly struct CompetitionContribution
{
    public readonly string Id;
    public readonly float DbhCm;
    public readonly float DistanceM;
    public readonly double Contribution;
    public readonly float Share;
    public readonly bool Living;
    public readonly bool PlannedFell;
    public readonly bool InFootprint;

    public CompetitionContribution(string id, float dbhCm, float distanceM, double contribution, float share,
        bool living, bool plannedFell, bool inFootprint)
    {
        Id = id;
        DbhCm = dbhCm;
        DistanceM = distanceM;
        Contribution = contribution;
        Share = share;
        Living = living;
        PlannedFell = plannedFell;
        InFootprint = inFootprint;
    }
}

// Current competition is the sum over living neighbours inside the existing
// Hegyi reach. HistoricalTotal adds recorded non-access removals for the
// lesson's release account, so a stump cannot make an inside removal look
// larger merely by disappearing from the current denominator.
public sealed class CompetitionShareSet
{
    public float TargetDbhCm;
    public double CurrentTotal;
    public double HistoricalTotal;
    public readonly List<CompetitionContribution> Neighbours = new List<CompetitionContribution>();
}

// The single current-Share calculation: the production Hegyi term and cutoff,
// the living-neighbour denominator, and each neighbour's fraction of it.
public static class CompetitionShare
{
    public static CompetitionShareSet Measure(float targetDbhCm, IEnumerable<CompetitionSample> samples)
    {
        var result = new CompetitionShareSet { TargetDbhCm = targetDbhCm };
        if (samples == null) return result;
        float cutoff = ForestEcologyController.HegyiCutoffMeters;
        foreach (CompetitionSample sample in samples)
        {
            float dx = sample.X, dz = sample.Z;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            if (distance > cutoff) continue;
            double term = ForestEcologyController.HegyiTerm(sample.DbhCm, targetDbhCm, distance);
            if (sample.Living) result.CurrentTotal += term;
            else result.HistoricalTotal += term;
            result.Neighbours.Add(new CompetitionContribution(sample.Id, sample.DbhCm, distance, term, 0f,
                sample.Living, sample.PlannedFell, sample.InFootprint));
        }
        result.HistoricalTotal += result.CurrentTotal;
        if (result.CurrentTotal <= 0) return result;
        for (int i = 0; i < result.Neighbours.Count; i++)
        {
            CompetitionContribution entry = result.Neighbours[i];
            if (!entry.Living) continue;
            result.Neighbours[i] = new CompetitionContribution(entry.Id, entry.DbhCm, entry.DistanceM, entry.Contribution,
                (float)(entry.Contribution / result.CurrentTotal), entry.Living, entry.PlannedFell, entry.InFootprint);
        }
        return result;
    }

    // Compute sample positions relative to the Crop Tree. Keeping this in one
    // place makes both callers use the same rotation/translation-free distance.
    public static CompetitionSample SampleFrom(string id, float x, float z, float targetX, float targetZ,
        float dbhCm, bool living, bool plannedFell, bool inFootprint)
        => new CompetitionSample(id, x - targetX, z - targetZ, dbhCm, living, plannedFell, inFootprint);
}
