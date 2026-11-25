namespace SitkaP1.Core;

using SitkaP1.Core.Phase1;

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Stage 5: add one fenced ClearUndergrowth order for one existing in-polygon ground cell of one area. Pending only: no
    /// cover, band, tree or clock changes until the approved plan is executed.
    /// </summary>
    internal MultiAreaCommandResult AddClearUndergrowth(MultiAreaFence fence, string areaId, int column, int row)
    {
        lock (sync)
        {
            RequireFence(fence);
            var stage5 = RequireStage5();
            AreaIndex(areaId);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");
            RequireClearCell(stage5, areaId, column, row);
            if (stage5.PendingClears.Any(order => order.AreaId == areaId && order.Column == column && order.Row == row))
                throw new P1Refusal("ClearDuplicateCell", "this ground cell is already in the pending Work Plan");
            var pending = stage5.PendingClears.Append(new MultiAreaClearUndergrowthOrder(areaId, column, row)).ToArray();
            root = root with
            {
                Stage5 = stage5 with { PendingClears = Array.AsReadOnly(pending) },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    /// <summary>Stage 5: remove one pending ClearUndergrowth order before approval. Never touches ecology.</summary>
    internal MultiAreaCommandResult RemoveClearUndergrowth(MultiAreaFence fence, string areaId, int column, int row)
    {
        lock (sync)
        {
            RequireFence(fence);
            var stage5 = RequireStage5();
            AreaIndex(areaId);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "the plan is approved; cancel it before editing");
            var match = stage5.PendingClears.FirstOrDefault(order => order.AreaId == areaId && order.Column == column && order.Row == row);
            if (match is null)
                throw new P1Refusal("UnknownWorkOrder", "that ground cell has no pending ClearUndergrowth order");
            var pending = stage5.PendingClears.Where(order => !ReferenceEquals(order, match)).ToArray();
            root = root with
            {
                Stage5 = stage5 with { PendingClears = Array.AsReadOnly(pending) },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }
}
