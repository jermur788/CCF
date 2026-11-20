using System.Runtime.CompilerServices;

// This diagnostic is private to the standalone lab's existing UI assembly, not an extension of the public player facade.
[assembly: InternalsVisibleTo("sitkap1-ui")]

namespace SitkaP1.Core;

/// <summary>One living neighbour inside the annual model's 8 m cutoff. Strength is a share band, not felling advice.</summary>
internal sealed record PlayerCompetitor(string TreeId, float DbhCm, float DistanceM, float Contribution,
    double SharePercent, string Strength, string PlannedRemoval);

/// <summary>
/// Read-only evidence from one immutable player view. Neighbours retain annual ordinal-ID summation order;
/// TopFive is a ranked projection of those same entries. A planned-for-Fell target has no retained-tree after estimate.
/// </summary>
internal sealed record PlayerCompetitionReport(string TargetTreeId, float CutoffM, float CurrentTotal,
    float? HypotheticalTotal, bool TargetPlannedForFell, IReadOnlyList<PlayerCompetitor> Neighbours,
    IReadOnlyList<PlayerCompetitor> TopFive, int RemainingCount, double RemainingSharePercent);

internal static class PlayerCompetitionDiagnostics
{
    /// <summary>
    /// Repeats only the frozen annual Hegyi arithmetic, including f32 distance and ordinal-ID accumulation.
    /// Does not run growth, mortality or RNG, and never owns or modifies session state.
    /// </summary>
    internal static PlayerCompetitionReport Measure(PlayerSessionView view, string treeId)
    {
        ArgumentNullException.ThrowIfNull(view);
        var target = view.Frame.Trees.FirstOrDefault(tree => tree.TreeId == treeId)
            ?? throw new P1Refusal("CompetitionTreeUnknown", "no tree has this exact ID");
        if (!target.IsLiving)
            throw new P1Refusal("CompetitionTargetDead", "competition inspection needs a living target");

        var pending = view.Pending.Where(order => order.Action == WorkAction.Fell)
            .Select(order => order.TreeId).ToHashSet(StringComparer.Ordinal);
        var approved = (view.Approved?.Orders ?? Array.Empty<WorkOrder>()).Where(order => order.Action == WorkAction.Fell)
            .Select(order => order.TreeId).ToHashSet(StringComparer.Ordinal);
        bool targetRemoved = pending.Contains(treeId) || approved.Contains(treeId);
        var terms = new List<(AnnualTree Tree, float Distance, float Contribution, string Removal)>();
        float current = 0f, after = 0f;
        foreach (var tree in view.Frame.Trees.Where(tree => tree.IsLiving).OrderBy(tree => tree.TreeId, StringComparer.Ordinal))
        {
            if (tree.TreeId == treeId) continue;
            float dx = target.XM - tree.XM;
            float dz = target.ZM - tree.ZM;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            if (distance > 8f) continue;
            float term = (tree.DbhCm / MathF.Max(1f, target.DbhCm)) / MathF.Max(0.5f, distance);
            string removal = approved.Contains(tree.TreeId) ? "Approved" : pending.Contains(tree.TreeId) ? "Pending" : "None";
            current += term;
            if (removal == "None") after += term;
            terms.Add((tree, distance, term, removal));
        }
        var neighbours = terms.Select(term =>
        {
            double share = current > 0f ? 100.0 * term.Contribution / current : 0.0;
            return new PlayerCompetitor(term.Tree.TreeId, term.Tree.DbhCm, term.Distance, term.Contribution,
                share, share < 5.0 ? "Low" : share < 10.0 ? "Moderate" : "Strong", term.Removal);
        }).ToArray();
        var ranked = neighbours.OrderByDescending(entry => entry.Contribution)
            .ThenBy(entry => entry.TreeId, StringComparer.Ordinal).ToArray();
        return new PlayerCompetitionReport(treeId, 8f, current, targetRemoved ? null : after, targetRemoved,
            Array.AsReadOnly(neighbours), Array.AsReadOnly(ranked.Take(5).ToArray()), Math.Max(0, ranked.Length - 5),
            ranked.Skip(5).Sum(entry => entry.SharePercent));
    }
}
