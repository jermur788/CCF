using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// How far the player is through the planned Scenario One rack layout. Read-only and derived: a layout rack is done when no tree
// on its line is left to plan (each is in the Work Plan, or has been felled), so the answer comes from the forest and the plan and
// survives saving and loading without a stored flag. Refreshed a couple of times a second, never per frame.
public sealed class RackLayoutTracker
{
    private const float RefreshSeconds = 0.5f;
    private readonly ScenarioOneUiRoot ui;
    private float nextRefresh;
    private int stamp = -1;
    public List<RackLayoutLine> Lines { get; private set; } = new List<RackLayoutLine>();

    public RackLayoutTracker(ScenarioOneUiRoot ui) { this.ui = ui; }

    public int Total => Lines.Count;
    public int Done => Lines.Count(line => line.Done);
    // True when the layout exists and every rack of it is planned or harvested.
    public bool Complete => Total > 0 && Done == Total;
    // True when every rack of the layout has been harvested: no fellable tree is left in any corridor.
    public bool Harvested => Total > 0 && Lines.All(line => line.TreesLeft == 0);
    public int Version => stamp;
    public int PlannedTrees => Lines.Sum(line => line.Planned);
    private bool HasFellingWork => ui.Manager.WorkOrders.Any(order => order.IsOpen && order.type == ScenarioWorkType.FellTree);
    public bool HasProgress => Done > 0 || PlannedTrees > 0 || HasFellingWork || ui.Manager.Interventions.Count > 0;

    // Explain actual required-route coverage, not merely whether a legal felling proposal can be added.
    // The tree IDs come from the existing layout selector; no qualification or completion rule is duplicated here.
    public string PreviewFeedback(RackProposal proposal)
    {
        if (Total == 0 || proposal == null || !proposal.CanConfirm) return "";
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
            return $"This will fully plan {finished} required access route{(finished == 1 ? "" : "s")}. {Total - Done - finished} of {Total} routes will still need work. Use Plan rack on Stand Map [M] for the rest.";
        int remaining = Lines.Sum(line => line.Remaining) - covered;
        return $"Adds {covered} trees on required access routes, but no whole route yet. {remaining} trees on those routes still need planning. Use Plan rack on Stand Map [M] to finish them.";
    }

    public string ProgressInstruction
    {
        get
        {
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
        if (signature != previous || stamp < 0)
            stamp++;
    }
}
