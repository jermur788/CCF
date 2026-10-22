using System;
using System.Collections.Generic;
using UnityEngine;

// How a neighbour's share of a tree's competition is described on screen.
// These bands are a presentation choice (share of this tree's own total),
// not a forestry recommendation and not part of the ecology model.
public enum CompetitorStrength { Low, Moderate, Strong }

// One living tree as seen by the competition breakdown. Plain data, so the
// breakdown can be tested without a scene.
public readonly struct CompetitionTree
{
    public readonly string Id;
    public readonly Vector2 Position;
    public readonly float DbhCm;
    // True when the tree has a valid open FellTree Work Plan order for the
    // competition breakdown. PlannedFell remains the existing Fell-mark input
    // used by the release summary, so that presentation keeps its old meaning.
    public readonly bool InWorkPlan;
    public readonly bool PlannedFell;
    public readonly bool CropTree;

    public CompetitionTree(string id, Vector2 position, float dbhCm, bool plannedFell, bool cropTree)
        : this(id, position, dbhCm, plannedFell, cropTree, plannedFell)
    {
    }

    public CompetitionTree(string id, Vector2 position, float dbhCm, bool plannedFell, bool cropTree, bool inWorkPlan)
    {
        Id = id ?? "";
        Position = position;
        DbhCm = dbhCm;
        PlannedFell = plannedFell;
        CropTree = cropTree;
        InWorkPlan = inWorkPlan;
    }
}

public sealed class CompetitorEntry
{
    public string Id;
    public int TreeIndex;
    public float DbhCm;
    public float DistanceM;
    public float Contribution;
    public float Share;
    public CompetitorStrength Strength;
    public bool PlannedFell;
    public bool CropTree;
    // Other Crop Trees (not the inspected one) that also have this tree inside their competition radius.
    public int OtherCropTreesNearby;
}

public sealed class CropTreeCompetitionReport
{
    public string TargetId = "";
    public float TargetDbhCm;
    // Sum of every neighbour's contribution: the same quantity as the tree's competition index.
    public float CompetitionNow;
    // The same sum with Fell-marked neighbours left out. A diagnostic estimate, not a growth result.
    public float CompetitionAfterPlanned;
    public int PlannedNeighbourCount;
    public float PlannedShare;
    // Every neighbour inside the radius, strongest first (ties by tree id).
    public readonly List<CompetitorEntry> Ranked = new List<CompetitorEntry>();

    public int NeighbourCount => Ranked.Count;
    public bool HasPlannedRemovals => PlannedNeighbourCount > 0;
    public float ChangeFraction => CompetitionNow > 0f ? (CompetitionAfterPlanned - CompetitionNow) / CompetitionNow : 0f;

    public float ShareOfFirst(int count)
    {
        float share = 0f;
        for (int i = 0; i < Ranked.Count && i < count; i++) share += Ranked[i].Share;
        return share;
    }
}

// Release estimate over every Crop Tree for the current Fell marks.
public sealed class CropTreeReleaseSummary
{
    public int CropTrees;
    public float MeanCompetitionNow;
    public float MeanCompetitionAfter;
    // Crop Trees whose competition would fall by at least ReleasedFraction.
    public int CropTreesReleased;
    // Each Fell-marked stem is counted once, however many Crop Trees it is near.
    public int PlannedFells;
    public int PlannedNearACropTree;
    public int PlannedNearSeveralCropTrees;

    public float ChangeFraction => MeanCompetitionNow > 0f ? (MeanCompetitionAfter - MeanCompetitionNow) / MeanCompetitionNow : 0f;
}

// Read-only breakdown of the existing Hegyi competition index into each
// neighbour's contribution. It repeats the pair set used by
// ForestEcologyController.UpdateCompetition (living trees, horizontal
// distance, the same cutoff) and uses the shared CompetitionShare calculation,
// so the parts add up to the tree's current competition. It adds no ecological
// rule and changes nothing.
public static class CropTreeCompetition
{
    public const int ListedCount = 5;
    public const float StrongShare = 0.10f;
    public const float ModerateShare = 0.05f;
    // "Released" in the summary: competition falls by at least a tenth. Presentation only.
    public const float ReleasedFraction = 0.10f;

    public static CompetitorStrength StrengthOf(float share)
        => share >= StrongShare ? CompetitorStrength.Strong
            : share >= ModerateShare ? CompetitorStrength.Moderate : CompetitorStrength.Low;

    // trees: living trees in ordinal tree-id order (the order the annual step sums in).
    public static CropTreeCompetitionReport Analyse(IReadOnlyList<CompetitionTree> trees, int targetIndex)
    {
        var report = new CropTreeCompetitionReport();
        if (trees == null || targetIndex < 0 || targetIndex >= trees.Count)
            return report;
        CompetitionTree target = trees[targetIndex];
        report.TargetId = target.Id;
        report.TargetDbhCm = target.DbhCm;
        var indices = new Dictionary<string, int>(StringComparer.Ordinal);
        var samples = new List<CompetitionSample>(trees.Count - 1);
        for (int j = 0; j < trees.Count; j++)
        {
            if (j == targetIndex)
                continue;
            CompetitionTree neighbour = trees[j];
            indices[neighbour.Id] = j;
            samples.Add(CompetitionShare.SampleFrom(neighbour.Id, neighbour.Position.x, neighbour.Position.y,
                target.Position.x, target.Position.y, neighbour.DbhCm, true, neighbour.InWorkPlan, true));
        }

        CompetitionShareSet competition = CompetitionShare.Measure(target.DbhCm, samples);
        report.CompetitionNow = (float)competition.CurrentTotal;
        double planned = 0;
        foreach (CompetitionContribution contribution in competition.Neighbours)
        {
            if (contribution.PlannedFell) planned += contribution.Contribution;
            report.Ranked.Add(new CompetitorEntry
            {
                Id = contribution.Id,
                TreeIndex = indices[contribution.Id],
                DbhCm = contribution.DbhCm,
                DistanceM = contribution.DistanceM,
                Contribution = (float)contribution.Contribution,
                Share = contribution.Share,
                Strength = StrengthOf(contribution.Share),
                PlannedFell = contribution.PlannedFell,
                CropTree = trees[indices[contribution.Id]].CropTree
            });
            if (contribution.PlannedFell) report.PlannedNeighbourCount++;
        }
        report.CompetitionAfterPlanned = report.CompetitionNow - (float)planned;
        report.PlannedShare = report.CompetitionNow > 0f ? (float)(planned / competition.CurrentTotal) : 0f;
        report.Ranked.Sort((a, b) =>
        {
            int byContribution = b.Contribution.CompareTo(a.Contribution);
            return byContribution != 0 ? byContribution : string.CompareOrdinal(a.Id, b.Id);
        });
        return report;
    }

    // Fills OtherCropTreesNearby for the first `count` entries only (the ones shown).
    public static void CountNearbyCropTrees(IReadOnlyList<CompetitionTree> trees, CropTreeCompetitionReport report, int count)
    {
        if (trees == null || report == null) return;
        float cutoff = ForestEcologyController.HegyiCutoffMeters;
        for (int i = 0; i < report.Ranked.Count && i < count; i++)
        {
            CompetitorEntry entry = report.Ranked[i];
            Vector2 position = trees[entry.TreeIndex].Position;
            int nearby = 0;
            for (int c = 0; c < trees.Count; c++)
            {
                if (!trees[c].CropTree || c == entry.TreeIndex || trees[c].Id == report.TargetId)
                    continue;
                if (Vector2.Distance(position, trees[c].Position) <= cutoff)
                    nearby++;
            }
            entry.OtherCropTreesNearby = nearby;
        }
    }

    public static CropTreeReleaseSummary Summarise(IReadOnlyList<CompetitionTree> trees)
    {
        var summary = new CropTreeReleaseSummary();
        if (trees == null) return summary;
        float cutoff = ForestEcologyController.HegyiCutoffMeters;
        var cropIndices = new List<int>();
        for (int i = 0; i < trees.Count; i++)
        {
            if (trees[i].CropTree) cropIndices.Add(i);
            if (trees[i].PlannedFell) summary.PlannedFells++;
        }

        float nowSum = 0f, afterSum = 0f;
        foreach (int index in cropIndices)
        {
            CropTreeCompetitionReport report = Analyse(trees, index);
            nowSum += report.CompetitionNow;
            afterSum += report.CompetitionAfterPlanned;
            if (report.CompetitionNow > 0f && -report.ChangeFraction >= ReleasedFraction)
                summary.CropTreesReleased++;
        }
        summary.CropTrees = cropIndices.Count;
        if (cropIndices.Count > 0)
        {
            summary.MeanCompetitionNow = nowSum / cropIndices.Count;
            summary.MeanCompetitionAfter = afterSum / cropIndices.Count;
        }

        for (int i = 0; i < trees.Count; i++)
        {
            if (!trees[i].PlannedFell) continue;
            int near = 0;
            foreach (int crop in cropIndices)
                if (Vector2.Distance(trees[i].Position, trees[crop].Position) <= cutoff)
                    near++;
            if (near >= 1) summary.PlannedNearACropTree++;
            if (near >= 2) summary.PlannedNearSeveralCropTrees++;
        }
        return summary;
    }

    // Describes the distribution, never what to do about it.
    public static string DistributionSentence(CropTreeCompetitionReport report)
    {
        if (report == null || report.NeighbourCount == 0)
            return "No living trees within " + ForestEcologyController.HegyiCutoffMeters.ToString("0", System.Globalization.CultureInfo.InvariantCulture)
                + " m: this tree has no competition from neighbours.";
        float first = report.Ranked[0].Share;
        if (first >= 0.20f)
            return $"One neighbour contributes a large share; the rest comes from {report.NeighbourCount - 1} other trees.";
        if (report.ShareOfFirst(ListedCount) < 0.40f)
            return "Competition is spread across many trees; no single neighbour dominates.";
        return "A few neighbours contribute most of the competition.";
    }

    public static string StrengthLabel(CompetitorStrength strength)
        => strength == CompetitorStrength.Strong ? "■■■ Strong"
            : strength == CompetitorStrength.Moderate ? "■■ Moderate" : "■ Low";

    // Share of potential diameter growth withheld, using the existing response
    // 1 / (1 + CI / Ci50) (ForestEcologyController.GetCurrentSuppression).
    public static float GrowthWithheld(float competition, float ci50)
        => ci50 > 0f ? Mathf.Clamp01(1f - 1f / (1f + competition / ci50)) : 0f;
}
