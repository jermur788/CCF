using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// How far the player is through the planned Scenario One rack layout. Read-only and derived: a layout rack is done when no tree
// on its line is left to plan (each is in the Work Plan, or has been felled), so the answer comes from the forest and the plan and
// survives saving and loading without a stored flag. Refreshed a couple of times a second, never per frame.
//
// A Rack Planning V2 world (D-064) is not measured against those authored lines. There the racks are persisted objects and the
// felling they own is tagged with their rack id, so progress comes straight from ScenarioOneManager.Racks and RackOrderState. That
// is read fresh on every call, never cached, so it cannot outlive a load or a new game. Lines stays the authored layout because the
// Stand Map still draws it. Model-0 worlds (Legacy40, and Enlarged80 before V2) never take the V2 branches.
public sealed class RackLayoutTracker
{
    private const float RefreshSeconds = 0.5f;
    private readonly ScenarioOneUiRoot ui;
    private float nextRefresh;
    private int stamp = -1;
    private string planToken = "";
    public List<RackLayoutLine> Lines { get; private set; } = new List<RackLayoutLine>();

    public RackLayoutTracker(ScenarioOneUiRoot ui) { this.ui = ui; }

    private bool RackPlanV2 => ui.Manager != null && ui.Manager.RackPlanningV2Active;

    // Where the V2 rack objects stand: racks that exist, racks whose work is in the Work Plan or done, racks whose work has all been
    // resolved (nothing open and something carried out), and the rack-owned orders still open.
    private struct RackPlanProgress { public int Racks, Planned, Resolved, OpenOrders; }

    private RackPlanProgress MeasureRackPlan()
    {
        var progress = new RackPlanProgress();
        ScenarioOneManager manager = ui.Manager;
        foreach (RackSaveData rack in manager.Racks)
        {
            if (rack == null) continue;
            RackOrderLifecycle work = manager.RackOrderState(rack.rackId);
            progress.Racks++;
            progress.OpenOrders += work.Open;
            // A confirmed rack always owns at least one order, so any pending, approved or completed order means it is planned.
            if (work.Open + work.Completed > 0) progress.Planned++;
            if (work.Completed > 0 && work.Open == 0) progress.Resolved++;
        }
        return progress;
    }

    public int Total => RackPlanV2 ? Mathf.Max(ScenarioOneManager.RackPlanTargetCount, MeasureRackPlan().Racks) : Lines.Count;
    public int Done => RackPlanV2 ? MeasureRackPlan().Planned : Lines.Count(line => line.Done);
    // True when the layout exists and every rack of it is planned or harvested.
    public bool Complete => Total > 0 && Done == Total;
    // True when every rack of the layout has been harvested: no fellable tree is left in any corridor (V2: every rack's work is resolved).
    public bool Harvested => RackPlanV2 ? Complete && MeasureRackPlan().Resolved == Total : Total > 0 && Lines.All(line => line.TreesLeft == 0);
    public int Version => stamp;
    public int PlannedTrees => RackPlanV2 ? MeasureRackPlan().OpenOrders : Lines.Sum(line => line.Planned);
    private bool HasFellingWork => ui.Manager.WorkOrders.Any(order => order.IsOpen && order.type == ScenarioWorkType.FellTree);
    public bool HasProgress => RackPlanV2 ? Done > 0 || ui.Manager.Interventions.Count > 0
        : Total > 0 && (Done > 0 || PlannedTrees > 0 || HasFellingWork || ui.Manager.Interventions.Count > 0);

    // Felling that still waits for approval, and felling that is approved or already carried out. In a V2 world only the work the racks
    // own counts, so ordinary felling elsewhere in the Work Plan can neither hold back nor satisfy the rack lessons.
    public bool WorkPending
    {
        get
        {
            bool v2 = RackPlanV2;
            return ui.Manager.WorkOrders.Any(order => order != null && (v2 ? order.rackId > 0 : order.type == ScenarioWorkType.FellTree)
                && order.status == ScenarioWorkStatus.Pending);
        }
    }

    public bool WorkApproved
    {
        get
        {
            bool v2 = RackPlanV2;
            return ui.Manager.WorkOrders.Any(order => order != null && (v2 ? order.rackId > 0 : order.type == ScenarioWorkType.FellTree)
                && (order.status == ScenarioWorkStatus.Approved || order.status == ScenarioWorkStatus.Completed));
        }
    }

    // Explain actual required-route coverage, not merely whether a legal felling proposal can be added.
    // The tree IDs come from the existing layout selector; no qualification or completion rule is duplicated here.
    // This describes the authored routes the old rack tool and Stand Map still use, so it counts those lines in every world.
    public string PreviewFeedback(RackProposal proposal)
    {
        int total = Lines.Count, done = Lines.Count(line => line.Done);
        if (total == 0 || proposal == null || !proposal.CanConfirm) return "";
        var selected = new HashSet<string>(proposal.TreeIds);
        int covered = 0, finished = 0;
        foreach (RackLayoutLine line in Lines)
        {
            int added = line.RemainingTreeIds.Count(selected.Contains);
            covered += added;
            if (!line.Done && added == line.Remaining) finished++;
        }
        if (covered == 0)
            return "This rack adds no trees on the required access routes, so it will not advance Objective 1. Use Plan rack on Stand Map [M] to choose a marked route.";
        if (finished > 0)
            return $"This will fully plan {finished} required access route{(finished == 1 ? "" : "s")}. {total - done - finished} of {total} routes will still need work. Use Plan rack on Stand Map [M] for the rest.";
        int remaining = Lines.Sum(line => line.Remaining) - covered;
        return $"Adds {covered} trees on required access routes, but no whole route yet. {remaining} trees on those routes still need planning. Use Plan rack on Stand Map [M] to finish them.";
    }

    public string ProgressInstruction
    {
        get
        {
            // The V2 wording names no tool or screen: the player-facing V2 authoring and marking lanes are separate tasks.
            if (RackPlanV2) return $"{Done} of {Total} racks have their work in the Work Plan; {Total - Done} still need work.";
            string next = $"{Done} of {Total} required access routes fully planned; {Total - Done} still need work. Use Plan rack on Stand Map [M] to finish the marked routes.";
            if (PlannedTrees > 0) return $"{PlannedTrees} trees on the required routes are in your Work Plan. " + next;
            if (HasFellingWork && Done == 0)
                return "Your Work Plan has felling work, but none is on the required access routes. " + next;
            return next;
        }
    }

    public void Update(bool force = false)
    {
        if (!force && Time.unscaledTime < nextRefresh)
            return;
        nextRefresh = Time.unscaledTime + RefreshSeconds;
        RackTool tool = ui.RackTool;
        if (tool == null || tool.Planner == null)
            return;
        tool.Planner.Refresh();
        List<RackLayoutLine> next = tool.Planner.LayoutStatus();
        // Presentation code keys its rebuilds on this number, so it only changes when a rack's progress does.
        string signature = string.Join(";", next.Select(l => l.Index + ":" + l.Remaining + ":" + l.Planned + ":" + l.TreesLeft));
        string previous = string.Join(";", Lines.Select(l => l.Index + ":" + l.Remaining + ":" + l.Planned + ":" + l.TreesLeft));
        Lines = next;
        // A V2 world's progress is not in Lines, so it has to move the version on its own.
        string plan = RackPlanV2 ? Total + ":" + Done + ":" + PlannedTrees + ":" + Harvested : "";
        if (signature != previous || plan != planToken || stamp < 0)
            stamp++;
        planToken = plan;
    }
}
