using System;
using System.Collections.Generic;
using System.Linq;

// Session-only evidence of real inspections and decisions, not an ecological or saved progression system.
// The caller supplies ForestPlayer's inspection serial; keeping a panel open across work/review cannot create a revisit.
public sealed class ScenarioOneRackReview
{
    public enum Decision { RetainHere, ReleaseFurther }
    private readonly HashSet<string> inspected = new HashSet<string>(StringComparer.Ordinal);
    private readonly Dictionary<string, Decision> decisions = new Dictionary<string, Decision>(StringComparer.Ordinal);
    private readonly HashSet<int> portions = new HashSet<int>();
    private string context = "", currentTree = "";
    private int lastSerial = -1;
    private bool ready;
    public int InspectedCount => inspected.Count;
    public int DecisionCount => decisions.Count;
    public int StandPortionsReviewed => portions.Count;

    public void Reset(int inspectionSerial)
    {
        context = ""; currentTree = ""; ready = false; lastSerial = inspectionSerial;
        inspected.Clear(); decisions.Clear(); portions.Clear();
    }

    public void Synchronize(ScenarioOneRackBay.Analysis bay, bool resultsReviewed, int inspectionSerial)
    {
        bool eligible = bay != null && bay.Applies && bay.HasEnoughFuture && bay.ReleaseExecuted && bay.MixedExecuted && resultsReviewed;
        string key = eligible ? string.Join(",", bay.Future.Select(t => t.Id).OrderBy(id => id, StringComparer.Ordinal)) + "#" + bay.ReleaseKey : "";
        if (key != context || eligible != ready)
        {
            Reset(inspectionSerial); context = key; ready = eligible;
        }
    }

    public void Observe(ScenarioOneRackBay.Analysis bay, string treeId, float x, float z, int inspectionSerial)
    {
        if (inspectionSerial == lastSerial) return;
        lastSerial = inspectionSerial; currentTree = "";
        if (!ready || bay == null || !bay.Contains(x, z)) return;
        currentTree = treeId;
        if (bay.Future.Any(t => t.Id == treeId)) inspected.Add(treeId);
    }

    public bool Decide(ScenarioOneRackBay.Analysis bay, string treeId, float x, float z, Decision decision)
    {
        if (!ready || bay == null || !bay.CanopyKept || !bay.HasEnoughFuture || treeId != currentTree || !bay.Contains(x, z)) return false;
        var future = bay.Future.FirstOrDefault(t => t.Id == treeId);
        bool selected = bay.Future.Any(t => t.Id == treeId);
        // A further-release decision names real relevant marks/orders, not another numerical target.
        if (decision == Decision.ReleaseFurther && (!selected || future.MarkedShare + future.PlannedShare <= 0f)) return false;
        if (selected)
        {
            if (!inspected.Contains(treeId)) return false;
            decisions[treeId] = decision;
        }
        else if (decision != Decision.RetainHere) return false;
        portions.Add(bay.Portion(x, z));
        return true;
    }

    public bool Complete(ScenarioOneRackBay.Analysis bay)
    {
        if (!ready || bay == null || !bay.HasEnoughFuture || !bay.CanopyKept || !bay.ReleaseExecuted || !bay.MixedExecuted || portions.Count != 6) return false;
        foreach (var tree in bay.Future)
        {
            if (!inspected.Contains(tree.Id) || !decisions.TryGetValue(tree.Id, out Decision decision)) return false;
            if (decision == Decision.ReleaseFurther && tree.MarkedShare + tree.PlannedShare <= 0f) return false;
            // Retain-here must agree with the current plan around this tree; cancel conflicting marks/orders or choose release further.
            if (decision == Decision.RetainHere && tree.MarkedShare + tree.PlannedShare > 0f) return false;
        }
        return true;
    }
}
