namespace SitkaP1.Core;

using SitkaP1.Core.Phase1;

internal sealed partial class MultiAreaContinuation
{
    // ------------------------------------------------------------------------------------------------ Crop marks

    internal MultiAreaCommandResult MarkCrop(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            int index = AreaIndex(areaId);
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "UnknownTreeId",
                "only a living exact tree ID of this area can be retained as Crop");
            if (root.Pending.Any(order => order.TreeId == treeId) ||
                (root.Approved?.Orders.Any(order => order.TreeId == treeId) ?? false))
                throw new P1Refusal("CropAlreadyPlannedForFell", "remove the pending Fell or cancel its approved plan first");
            var area = root.Areas[index];
            if (area.CropTreeIds.Contains(treeId, StringComparer.Ordinal))
                throw new P1Refusal("CropAlreadyMarked", "this tree is already retained as Crop");
            var crops = area.CropTreeIds.Append(treeId).Order(StringComparer.Ordinal).ToArray();
            root = root with
            {
                Areas = Replace(root.Areas, index, area with { CropTreeIds = Array.AsReadOnly(crops) }),
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    internal MultiAreaCommandResult UnmarkCrop(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            int index = AreaIndex(areaId);
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "UnknownTreeId",
                "only a living exact tree ID of this area can have a Crop mark removed");
            var area = root.Areas[index];
            if (!area.CropTreeIds.Contains(treeId, StringComparer.Ordinal))
                throw new P1Refusal("CropNotMarked", "this tree has no retained Crop mark");
            var crops = area.CropTreeIds.Where(id => id != treeId).ToArray();
            root = root with
            {
                Areas = Replace(root.Areas, index, area with { CropTreeIds = Array.AsReadOnly(crops) }),
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }
}
