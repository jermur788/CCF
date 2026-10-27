using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Extraction racks are planned as ordinary felling orders. A confirmed rack adds one pending FellTree order per tree to the
// draft Work Plan, exactly as an individual Fell mark does; approval, execution, cost, timber value and the harvest are the
// existing ones. After the harvest the rack exists only as what it physically is: a line of felled trees (stumps) and an open
// corridor. Nothing here is saved: the grouping below is presentation state for the current session, so the save schema and every
// saved order are unchanged (after a load, rack trees are still in the plan as individual felling jobs, none hidden).
public sealed class RackGroup
{
    // Session counter, 1-based; the player sees "Extraction rack N".
    public int Id;
    public float AngleDegrees, AcrossM, WidthM;
    // The segment of ground the rack covers (world x/z), so the maps can draw it. Session-only like the rest of the group.
    public RackPoint Start, End;
    // 0-based layout rack this one lies on, or -1.
    public int LayoutIndex = -1;
    // Work order ids still in the Work Plan.
    public readonly List<int> OrderIds = new List<int>();
    public int TreeCount => OrderIds.Count;
}

public sealed partial class ScenarioOneManager
{
    private readonly List<RackGroup> rackGroups = new List<RackGroup>();
    private int nextRackGroupId = 1;
    private int rackStageEvents = -1;
    private bool rackStageCached;

    // Raised when a rack has been added to the Work Plan; the second argument says where it was planned ("forest" or "map").
    public event Action<RackGroup, string> RackConfirmed;

    // Racks confirmed in this session that still have trees in the Work Plan. A rack whose orders were all removed or resolved is dropped.
    public IReadOnlyList<RackGroup> RackGroups
    {
        get { PruneRackGroups(); return rackGroups; }
    }

    public RackGroup RackGroupOfOrder(int workOrderId)
    {
        foreach (RackGroup group in RackGroups)
            if (group.OrderIds.Contains(workOrderId))
                return group;
        return null;
    }

    // True while the tree has an open (pending or approved) felling order.
    public bool IsPlannedForFelling(string treeId) => HasOpenTreeOrder(treeId, ScenarioWorkType.FellTree);

    // The opening teaching stage: a fresh Enlarged80 forest in which no commissioned work has been carried out yet. Scenario One
    // presents extraction racks first and introduces selective marking afterwards. Derived from the recorded history (no stored
    // flag, no schema change), and presentation only: individual marking keeps working in every state.
    public bool RackFirstStageActive
    {
        get
        {
            if (definition == null || ecology == null || outcome != ScenarioOneOutcome.Active
                || !ScenarioOneRackLayout.AppliesTo(ecology.StandGeometryModelVersion))
                return false;
            if (rackStageEvents != managementEvents.Count)
            {
                rackStageEvents = managementEvents.Count;
                rackStageCached = Interventions.Count == 0;
            }
            return rackStageCached;
        }
    }

    // The harvest quote for these trees as if they were the only job (no order is created and no order id is used). The rack
    // preview and the Stand Map call this; the Work Plan quotes the real combined job.
    public ScenarioHarvestJob QuoteTrees(IEnumerable<string> treeIds)
    {
        Dictionary<string, ForestTree> living = LivingTreesById();
        var orders = new List<ScenarioOneWorkOrder>();
        int temporaryId = 0;
        foreach (string id in treeIds)
        {
            if (!living.TryGetValue(id, out ForestTree tree) || tree == null || !tree.CanChop)
                continue;
            orders.Add(new ScenarioOneWorkOrder
            {
                workOrderId = --temporaryId,
                type = ScenarioWorkType.FellTree,
                status = ScenarioWorkStatus.Pending,
                targetTreeId = tree.TreeId,
                speciesId = tree.Species != null ? tree.Species.SpeciesId : "",
                worldPosition = tree.transform.position,
                fellingOutcome = planningFellingOutcome,
                executionMethod = CCF.Forestry.WorkEconomy.WorkExecutionMethod.Contractor,
                expectedVolumeM3 = tree.BiologicalStemVolumeM3
            });
        }
        int interventions = managementEvents.Where(entry => entry.eventType == ScenarioManagementEventType.WorkResolved
            && entry.outcome == ScenarioManagementOutcome.Succeeded && entry.taskType == ScenarioWorkType.FellTree)
            .Select(entry => entry.year).Distinct().Count();
        return ScenarioOneEconomyAdapter.QuoteHarvest(orders, living, definition, CurrentYear + 1, interventions, cashCents, null, 10000, BroadleafMarketActive);
    }

    // Adds the proposed rack to the draft Work Plan: one pending felling order for each tree the proposal selected and that can
    // still be felled and is not already planned. Returns the number of trees added; 0 changes nothing.
    public int ConfirmRack(RackProposal proposal, string source = "forest")
    {
        if (proposal == null || proposal.Selection == null || proposal.Selection.Trees.Count == 0)
        {
            feedback = "There is no rack to add.";
            return 0;
        }
        if (!CanManage()) return 0;
        Dictionary<string, ForestTree> living = LivingTreesById();
        ForestTreeMarkingManager marking = UnityEngine.Object.FindFirstObjectByType<ForestTreeMarkingManager>();
        var group = new RackGroup { Id = nextRackGroupId, AngleDegrees = proposal.AngleDegrees, WidthM = proposal.WidthM, LayoutIndex = proposal.LayoutIndex };
        foreach (RackTree entry in proposal.Selection.Trees)
        {
            if (!living.TryGetValue(entry.Id, out ForestTree tree) || tree == null || !tree.CanChop || tree.IsCropTree
                || HasOpenTreeOrder(entry.Id, ScenarioWorkType.FellTree))
                continue;
            ScenarioOneWorkOrder order = CreateFellingOrder(tree);
            workOrders.Add(order);
            RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
            group.OrderIds.Add(order.workOrderId);
            // The order carries the proposal now; a loose Fell mark on the same tree would only be a duplicate.
            if (marking != null && tree.IsMarkedForFell)
                marking.Unmark(tree, false);
        }
        if (group.OrderIds.Count == 0)
        {
            feedback = "Every tree on that line is already planned or cannot be felled.";
            return 0;
        }
        group.AcrossM = RackGeometry.Across(proposal.Selection.Start, proposal.AngleDegrees);
        group.Start = proposal.Selection.Start;
        group.End = proposal.Selection.End;
        rackGroups.Add(group);
        nextRackGroupId++;
        GetHarvestQuote(false);
        feedback = $"Added an extraction rack to the Work Plan: {group.OrderIds.Count} tree{(group.OrderIds.Count == 1 ? "" : "s")}. "
            + "Review it in the Work Plan [Tab], then approve the plan.";
        RackConfirmed?.Invoke(group, source);
        return group.OrderIds.Count;
    }

    // Takes a whole rack out of the Work Plan again (pending orders are removed, approved ones cancelled).
    public int RemoveRackGroup(int groupId)
    {
        RackGroup group = rackGroups.FirstOrDefault(g => g.Id == groupId);
        if (group == null) return 0;
        int removed = 0;
        foreach (int orderId in group.OrderIds.ToList())
        {
            ScenarioOneWorkOrder order = workOrders.FirstOrDefault(o => o.workOrderId == orderId);
            if (order == null) continue;
            if (order.status == ScenarioWorkStatus.Pending ? RemovePendingOrder(orderId) : order.status == ScenarioWorkStatus.Approved && CancelApprovedOrder(orderId))
                removed++;
        }
        rackGroups.Remove(group);
        feedback = $"Took the extraction rack out of the Work Plan ({removed} tree{(removed == 1 ? "" : "s")}).";
        return removed;
    }

    private void PruneRackGroups()
    {
        for (int i = rackGroups.Count - 1; i >= 0; i--)
        {
            rackGroups[i].OrderIds.RemoveAll(id => !workOrders.Any(o => o.workOrderId == id && o.IsOpen));
            if (rackGroups[i].OrderIds.Count == 0)
                rackGroups.RemoveAt(i);
        }
    }

    // A new game or a load starts with no racks grouped; their trees (if still planned) are ordinary felling orders.
    private void ClearRackGroups()
    {
        rackGroups.Clear();
        nextRackGroupId = 1;
        rackStageEvents = -1;
    }
}
