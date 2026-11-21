namespace SitkaP1.Core;

/// <summary>
/// Presents ONE area of a multi-area view in the shape of a single-area <see cref="PlayerSessionView"/>, so the existing,
/// frozen single-area helpers can be reused unchanged instead of being copied: <see cref="PlayerMapScene"/> (map items,
/// paint order, status cues and the exact-ID hit-test) and <see cref="PlayerCompetitionDiagnostics"/> (the read-only
/// Hegyi diagnostic). The projection holds only that area's trees, Crop marks and orders, so another area's trees can
/// never appear as neighbours. It is an internal, transient value: it is never returned to a caller as a session view, and
/// its token fields only name the experiment, the area and the view's clocks.
/// </summary>
internal static class MultiAreaProjection
{
    internal static PlayerSessionView Of(MultiAreaSessionView view, MultiAreaAreaView area)
    {
        ArgumentNullException.ThrowIfNull(view);
        ArgumentNullException.ThrowIfNull(area);
        var token = view.Source;
        var source = new WorkSourceToken(token.Domain, token.ExperimentId, token.ManifestContentHash, token.ManifestContentHash,
            area.AreaId, token.EcologicalYear, token.AnnualRevision, token.WorkRevision, token.RngModelVersion,
            token.GrowthModelVersion, token.P1AlgorithmVersion, token.ActionEligibleDbhThresholdMm);
        var frame = new CommittedFrame(area.AreaId, token.Domain, token.ExperimentId, token.ManifestContentHash,
            token.ManifestContentHash, token.EcologicalYear, token.AnnualRevision, token.WorkRevision, area.Trees, area.Stand,
            Array.Empty<WorkEvent>());
        var pending = view.Pending.Where(order => order.AreaId == area.AreaId)
            .Select(order => new WorkOrder(order.TreeId, order.Action, order.Reason)).ToArray();
        var approved = view.Approved is null ? null : new PlayerApprovedPlan(view.Approved.PlanId, view.Approved.ExecutionKey,
            source, Array.AsReadOnly(view.Approved.Orders.Where(order => order.AreaId == area.AreaId)
                .Select(order => new WorkOrder(order.TreeId, order.Action, order.Reason)).ToArray()));
        return new PlayerSessionView(view.SessionId, view.PlanStateRevision, source, frame, Array.AsReadOnly(pending), approved,
            Array.Empty<AnnualYearRecord>(), area.Extent)
        { CropTreeIds = area.CropTreeIds };
    }

    /// <summary>The area's map items, built by the unchanged single-area map scene.</summary>
    internal static IReadOnlyList<PlayerMapItem> Scene(MultiAreaSessionView view, MultiAreaAreaView area) =>
        PlayerMapScene.Build(Of(view, area));
}
