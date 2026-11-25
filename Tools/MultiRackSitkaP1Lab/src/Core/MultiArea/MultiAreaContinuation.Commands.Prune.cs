namespace SitkaP1.Core;

using SitkaP1.Core.Phase1;

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Stage 4: add one fenced Prune order (one exact living tree of one area, target crown-base height). Pending only: no
    /// tree, crown or clock changes. The verified guards are checked now for a clean refusal and again at Execute.
    /// A tree may carry at most one order (Fell or Prune) in a plan; a retained Crop Tree may be pruned.
    /// </summary>
    internal MultiAreaCommandResult AddPrune(MultiAreaFence fence, string areaId, string treeId, float targetCrownBaseHeightM)
    {
        lock (sync)
        {
            RequireFence(fence);
            var stage4 = RequireStage4();
            int index = AreaIndex(areaId);
            if (string.IsNullOrWhiteSpace(treeId))
                throw new P1Refusal("UnknownTreeId", "a tree ID is required");
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");
            if (root.Pending.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)) ||
                stage4.PendingPrunes.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)))
                throw new P1Refusal("WorkDuplicateTreeId", "the tree ID is already in the pending Work Plan");
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "PruneTreeNotLiving",
                "only a living exact tree ID of this area can be pruned");
            RequirePrunable(FindTree(index, treeId)!, stage4.PruningOf(treeId), targetCrownBaseHeightM);
            var pending = stage4.PendingPrunes.Append(new MultiAreaPruneOrder(areaId, treeId, targetCrownBaseHeightM)).ToArray();
            root = root with
            {
                Stage4 = stage4 with { PendingPrunes = Array.AsReadOnly(pending) },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    /// <summary>
    /// Stage 4, /multi page: add one Prune order whose target crown-base height is the engine's next scheduled lift for that
    /// tree (<see cref="PruningRules.NextLiftTargetM"/>, the Unity Sitka schedule). The player never sends a target. Every
    /// fence, guard and refusal is exactly that of <see cref="AddPrune"/> (the lock is re-entrant, so it sees the same root).
    /// </summary>
    internal MultiAreaCommandResult AddScheduledPrune(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            int lifts = (root.Stage4 ?? MultiAreaStage4State.Empty).PruningOf(treeId ?? string.Empty).Lifts;
            float target = root.Phase2 is { } phase2
                ? Phase2Pruning.NextLiftTargetM(lifts, phase2.BroadleafOf(treeId ?? string.Empty))
                : PruningRules.NextLiftTargetM(lifts);
            return AddPrune(fence, areaId, treeId!, target);
        }
    }

    /// <summary>Stage 4: remove one pending Prune order before approval. Never touches biology.</summary>
    internal MultiAreaCommandResult RemovePrune(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            var stage4 = RequireStage4();
            AreaIndex(areaId);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "the plan is approved; cancel it before editing");
            var match = stage4.PendingPrunes.FirstOrDefault(order => order.TreeId == treeId);
            if (match is null)
                throw new P1Refusal("UnknownWorkOrder", "the tree ID has no pending Prune order");
            if (match.AreaId != areaId)
                throw new P1Refusal("MultiAreaWrongArea", "the pending Prune for " + treeId + " belongs to " + match.AreaId + ", not " + areaId);
            var pending = stage4.PendingPrunes.Where(order => !ReferenceEquals(order, match)).ToArray();
            root = root with
            {
                Stage4 = stage4 with { PendingPrunes = Array.AsReadOnly(pending) },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }
}
