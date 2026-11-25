namespace SitkaP1.Core;

using SitkaP1.Core.Phase1;

internal sealed partial class MultiAreaContinuation
{
    // ------------------------------------------------------------------------------------------------ the Work Plan

    /// <summary>Add one manually chosen living tree of one area, with a recorded reason. This is not work and fells nothing.</summary>
    internal MultiAreaCommandResult AddPending(MultiAreaFence fence, string areaId, string treeId, string reason)
    {
        lock (sync)
        {
            RequireFence(fence);
            int index = AreaIndex(areaId);
            if (string.IsNullOrWhiteSpace(treeId))
                throw new P1Refusal("UnknownTreeId", "a tree ID is required");
            if (string.IsNullOrWhiteSpace(reason))
                throw new P1Refusal("WorkReasonRequired", "every manual selection needs a recorded reason");
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");
            if (root.Pending.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)) ||
                (root.Stage4?.PendingPrunes.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)) ?? false))
                throw new P1Refusal("WorkDuplicateTreeId", "the tree ID is already in the pending Work Plan");
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "WorkTreeNotLive",
                "only a living exact tree ID of this area can be proposed for Fell");
            if (root.Areas[index].CropTreeIds.Contains(treeId, StringComparer.Ordinal))
                throw new P1Refusal("CropTreeRetained", "unmark this retained Crop Tree before proposing it for Fell");
            var pending = root.Pending.Append(new MultiAreaWorkOrder(areaId, treeId, WorkAction.Fell, reason)).ToArray();
            root = root with { Pending = Array.AsReadOnly(pending), PlanStateRevision = checked(root.PlanStateRevision + 1) };
            return WithView(null, null);
        }
    }

    /// <summary>Remove one pending order of one area before approval. Never touches biology.</summary>
    internal MultiAreaCommandResult RemovePending(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            AreaIndex(areaId);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "the plan is approved; cancel it before editing");
            var match = root.Pending.FirstOrDefault(order => order.TreeId == treeId);
            if (match is null)
                throw new P1Refusal("UnknownWorkOrder", "the tree ID is not in the pending Work Plan");
            if (match.AreaId != areaId)
                throw new P1Refusal("MultiAreaWrongArea", "the pending order for " + treeId + " belongs to " + match.AreaId + ", not " + areaId);
            var pending = root.Pending.Where(order => !ReferenceEquals(order, match)).ToArray();
            root = root with { Pending = Array.AsReadOnly(pending), PlanStateRevision = checked(root.PlanStateRevision + 1) };
            return WithView(null, null);
        }
    }
}
