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
