namespace SitkaP1.Core;

using SitkaP1.Core.Phase1;

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Phase 2: add one fenced Plant order (one nursery Oak or Beech at an exact point of one area). Pending only: no juvenile,
    /// cover, tree or clock changes until the approved plan is executed. No stock, cash or minutes exist or change.
    /// </summary>
    internal MultiAreaCommandResult AddPlant(MultiAreaFence fence, string areaId, string speciesId, float xM, float zM) =>
        AddPlanting(fence, number => new MultiAreaPlantOrder(Phase2Planting.OrderIdFor(number), areaId, speciesId, xM, zM));

    /// <summary>Phase 2: add one fenced Shelter order for the planted juvenile (or same-batch Plant) at a point. Pending only.</summary>
    internal MultiAreaCommandResult AddShelter(MultiAreaFence fence, string areaId, float xM, float zM) =>
        AddPlanting(fence, number => new MultiAreaShelterOrder(Phase2Planting.OrderIdFor(number), areaId, xM, zM));

    /// <summary>Phase 2: add one fenced 1 m² SpotClear order centred on a point. Pending only.</summary>
    internal MultiAreaCommandResult AddSpotClear(MultiAreaFence fence, string areaId, float xM, float zM) =>
        AddPlanting(fence, number => new MultiAreaSpotClearOrder(Phase2Planting.OrderIdFor(number), areaId, xM, zM));

    // The whole pending planting batch plus the new order is validated now (a clean refusal names the later order) and again,
    // atomically, at Execute.
    private MultiAreaCommandResult AddPlanting(MultiAreaFence fence, Func<int, MultiAreaPlantingOrder> create)
    {
        lock (sync)
        {
            RequireFence(fence);
            var phase2 = RequirePhase2();
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");
            var order = create(phase2.NextOrderNumber);
            AreaIndex(order.AreaId);
            var batch = phase2.PendingOrders.Append(order).ToArray();
            RequireValidPlantingBatch(phase2, batch);
            root = root with
            {
                Phase2 = phase2 with { PendingOrders = Array.AsReadOnly(batch), NextOrderNumber = checked(phase2.NextOrderNumber + 1) },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    /// <summary>Phase 2: remove one pending planting order before approval. Never touches ecology.</summary>
    internal MultiAreaCommandResult RemovePlanting(MultiAreaFence fence, string orderId)
    {
        lock (sync)
        {
            RequireFence(fence);
            var phase2 = RequirePhase2();
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "the plan is approved; cancel it before editing");
            var match = phase2.PendingOrders.FirstOrDefault(order => order.OrderId == orderId)
                ?? throw new P1Refusal("UnknownWorkOrder", "no pending planting order has that ID");
            var pending = phase2.PendingOrders.Where(order => !ReferenceEquals(order, match)).ToArray();
            root = root with
            {
                Phase2 = phase2 with { PendingOrders = Array.AsReadOnly(pending) },
                Economy = WithoutPendingExecutor(root.Economy, EconomyOrderKey.ForPlanting(match)),
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }
}
