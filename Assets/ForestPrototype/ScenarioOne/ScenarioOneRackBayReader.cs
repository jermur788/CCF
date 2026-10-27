using System.Collections.Generic;
using UnityEngine;

// Reads the live trees and Work Plan into the rack-area analysis (ScenarioOneRackBay) for the Objectives 2-5 lesson. Read-only: it writes no
// tree, mark, order or save, so the lesson can never change the forest it is teaching about.
public static class ScenarioOneRackBayReader
{
    // Model 0 / Legacy40: the area is the middle authored layout line.
    public static ScenarioOneRackBay.Analysis Read(IEnumerable<ForestTree> forestTrees, IReadOnlyList<ScenarioOneWorkOrder> workOrders,
        IReadOnlyList<float> rackLines, ScenarioOneRackBay.Frame? frame = null)
    {
        Collect(forestTrees, workOrders, out var trees, out var orders);
        return ScenarioOneRackBay.Analyse(rackLines, trees, orders, frame);
    }

    // Rack Planning V2: the area is the third confirmed rack (ScenarioOneRackSampleGeometry). The caller passes a valid sample derived
    // from a CONFIRMED plan; with none there is no area (null), never an authored line or a different rack.
    public static ScenarioOneRackBay.Analysis Read(IEnumerable<ForestTree> forestTrees, IReadOnlyList<ScenarioOneWorkOrder> workOrders,
        ScenarioOneRackSampleGeometry.Result sample)
    {
        if (sample == null || !sample.Valid) return null;
        Collect(forestTrees, workOrders, out var trees, out var orders);
        return ScenarioOneRackBay.AnalyseTeachingRack(trees, orders, sample.Teaching, sample.TeachingRack.Id,
            sample.Racks.Count, sample.TeachingRack.Index);
    }

    private static void Collect(IEnumerable<ForestTree> forestTrees, IReadOnlyList<ScenarioOneWorkOrder> workOrders,
        out List<ScenarioOneRackBay.Tree> trees, out List<ScenarioOneRackBay.Order> orders)
    {
        trees = new List<ScenarioOneRackBay.Tree>();
        foreach (ForestTree tree in forestTrees)
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId)) continue;
            Vector3 p = tree.transform.position;
            // A stump keeps the diameter it was felled at; a tree that died on its own is inactive and not listed at all.
            if (tree.IsLiving) trees.Add(new ScenarioOneRackBay.Tree(tree.TreeId, p.x, p.z, tree.Diameter, true, tree.IsCropTree, tree.IsMarkedForFell));
            else if (tree.IsStump) trees.Add(new ScenarioOneRackBay.Tree(tree.TreeId, p.x, p.z, tree.Diameter, false, false));
        }
        orders = new List<ScenarioOneRackBay.Order>();
        if (workOrders != null)
            foreach (ScenarioOneWorkOrder order in workOrders)
            {
                if (order == null || order.type != ScenarioWorkType.FellTree) continue;
                bool open = order.IsValidOpenFelling;
                bool done = order.status == ScenarioWorkStatus.Completed;
                if (!open && !done) continue;
                orders.Add(new ScenarioOneRackBay.Order(order.targetTreeId, open, done, DispositionOf(order.fellingOutcome), order.resolvedYear));
            }
    }

    private static ScenarioOneRackBay.Disposition DispositionOf(FellingMaterialOutcome outcome)
        => outcome == FellingMaterialOutcome.SellAndExtract ? ScenarioOneRackBay.Disposition.Sold
            : outcome == FellingMaterialOutcome.RetainAsFallenDeadwood ? ScenarioOneRackBay.Disposition.Deadwood
            : ScenarioOneRackBay.Disposition.Other;
}
