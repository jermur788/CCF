namespace SitkaP1.Core;

/// <summary>How a tree is drawn and listed (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §4.4). The page gives each a non-colour cue.</summary>
public enum PlayerMapStatus
{
    Living,
    PendingFell,
    ApprovedFell,
    Felled,
    NaturalDead,
}

/// <summary>
/// One drawn tree in map metres, north-up with 1 unit = 1 m: <see cref="U"/> east of the west edge, <see cref="V"/> south
/// of the north edge. <see cref="TreeId"/> is the exact source ID. <see cref="PaintOrder"/> is the item's index in its
/// scene, and a higher value is drawn on top. Display geometry only, never an ecological value or an authority.
/// </summary>
public sealed record PlayerMapItem(string TreeId, int PaintOrder, double U, double V, double RadiusM, float HeightM,
    float DbhCm, PlayerMapStatus Status, int? DeathYear, string? DeathCause);

/// <summary>
/// Pure, colour-free map geometry for one <see cref="PlayerSessionView"/> (spec §4.3–4.4, §5): true positions and crown
/// radii, the exact paint order and an exact-ID hit-test. It reads only the view it is given and keeps no state.
/// </summary>
public static class PlayerMapScene
{
    // A felled tree is drawn as a small stump marker, so its canopy gap opens (presentation constant, spec §4.4).
    internal const double StumpRadiusM = 0.35;

    /// <summary>
    /// One item per frame tree, in paint order. Crowns (living, pending, approved, natural dead) come first, shortest to
    /// tallest and then by ordinal ID; stumps follow by ordinal ID. The last item is drawn on top.
    /// </summary>
    public static IReadOnlyList<PlayerMapItem> Build(PlayerSessionView view)
    {
        ArgumentNullException.ThrowIfNull(view);
        var pending = view.Pending.Select(order => order.TreeId).ToHashSet(StringComparer.Ordinal);
        var approved = (view.Approved?.Orders ?? Array.Empty<WorkOrder>()).Select(order => order.TreeId)
            .ToHashSet(StringComparer.Ordinal);
        var drawn = view.Frame.Trees.Select(tree => (Tree: tree, Status: StatusOf(tree, pending, approved))).ToArray();
        var crowns = drawn.Where(entry => entry.Status != PlayerMapStatus.Felled)
            .OrderBy(entry => entry.Tree.HeightM).ThenBy(entry => entry.Tree.TreeId, StringComparer.Ordinal);
        var stumps = drawn.Where(entry => entry.Status == PlayerMapStatus.Felled)
            .OrderBy(entry => entry.Tree.TreeId, StringComparer.Ordinal);

        // Spec §4.3: U = X − (west edge), V = (north edge) − Z, in f64 on the source f32 values.
        var extent = view.Extent;
        double northEdgeZ = extent.MinZM + extent.SideM;
        var items = new List<PlayerMapItem>(drawn.Length);
        foreach (var (tree, status) in crowns.Concat(stumps))
        {
            double radius = status == PlayerMapStatus.Felled ? StumpRadiusM : tree.CrownRadiusM;
            items.Add(new PlayerMapItem(tree.TreeId, items.Count, (double)tree.XM - extent.MinXM,
                northEdgeZ - (double)tree.ZM, radius, tree.HeightM, tree.DbhCm, status,
                tree.Death?.EcologicalYear, tree.Death?.Cause));
        }
        return items.AsReadOnly();
    }

    /// <summary>
    /// Every item whose drawn disc contains (u, v), boundary included, topmost first. A point on bare ground returns no
    /// item: the nearest tree is never chosen for a miss, and a non-finite point selects nothing.
    /// </summary>
    public static IReadOnlyList<PlayerMapItem> HitTest(IReadOnlyList<PlayerMapItem> scene, double u, double v)
    {
        ArgumentNullException.ThrowIfNull(scene);
        var hits = new List<PlayerMapItem>();
        if (double.IsFinite(u) && double.IsFinite(v))
        {
            foreach (var item in scene)
            {
                double du = u - item.U;
                double dv = v - item.V;
                if (du * du + dv * dv <= item.RadiusM * item.RadiusM)
                    hits.Add(item);
            }
        }
        return Array.AsReadOnly(hits.OrderByDescending(item => item.PaintOrder).ToArray());
    }

    private static PlayerMapStatus StatusOf(AnnualTree tree, HashSet<string> pending, HashSet<string> approved)
    {
        if (tree.Death is { } death)
            return death.Cause == FellCause.Harvest ? PlayerMapStatus.Felled : PlayerMapStatus.NaturalDead;
        if (approved.Contains(tree.TreeId))
            return PlayerMapStatus.ApprovedFell;
        return pending.Contains(tree.TreeId) ? PlayerMapStatus.PendingFell : PlayerMapStatus.Living;
    }
}
