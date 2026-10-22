using System.Collections.Generic;
using UnityEngine;

// Reads the live trees and Work Plan into the rack-area analysis (ScenarioOneRackBay) for the Objectives 2-5 lesson. Read-only: it writes no
// tree, mark, order or save, so the lesson can never change the forest it is teaching about.
public static class ScenarioOneRackBayReader
{
    public static ScenarioOneRackBay.Analysis Read(IEnumerable<ForestTree> forestTrees, IReadOnlyList<ScenarioOneWorkOrder> workOrders,
        IReadOnlyList<float> rackLines)
    {
        var trees = new List<ScenarioOneRackBay.Tree>();
        foreach (ForestTree tree in forestTrees)
        {
            if (tree == null || string.IsNullOrEmpty(tree.TreeId)) continue;
            Vector3 p = tree.transform.position;
            // A stump keeps the diameter it was felled at; a tree that died on its own is inactive and not listed at all.
            if (tree.IsLiving) trees.Add(new ScenarioOneRackBay.Tree(tree.TreeId, p.x, p.z, tree.Diameter, true, tree.IsCropTree, tree.IsMarkedForFell));
            else if (tree.IsStump) trees.Add(new ScenarioOneRackBay.Tree(tree.TreeId, p.x, p.z, tree.Diameter, false, false));
        }
        var orders = new List<ScenarioOneRackBay.Order>();
        if (workOrders != null)
            foreach (ScenarioOneWorkOrder order in workOrders)
            {
                if (order == null || order.type != ScenarioWorkType.FellTree) continue;
                bool open = order.IsOpen && string.IsNullOrEmpty(order.validationMessage);
                bool done = order.status == ScenarioWorkStatus.Completed;
                if (!open && !done) continue;
                orders.Add(new ScenarioOneRackBay.Order(order.targetTreeId, open, done, DispositionOf(order.fellingOutcome), order.resolvedYear));
            }
        return ScenarioOneRackBay.Analyse(rackLines, trees, orders);
    }

    private static ScenarioOneRackBay.Disposition DispositionOf(FellingMaterialOutcome outcome)
        => outcome == FellingMaterialOutcome.SellAndExtract ? ScenarioOneRackBay.Disposition.Sold
            : outcome == FellingMaterialOutcome.RetainAsFallenDeadwood ? ScenarioOneRackBay.Disposition.Deadwood
            : ScenarioOneRackBay.Disposition.Other;
}
