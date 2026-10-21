using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Builds the ONE StandMapModel from the live game, for both the full Stand Map and the HUD minimap. It reads; it never writes to
// the forest, the Work Plan or a save, so map presentation cannot change the simulation.
//
// Where each thing on the map comes from (no copies are kept):
//   property, grid      the ecology controller's stand bounds and cells
//   racks               RackLayoutTracker.Lines (the planned layout, derived from the forest) and the racks confirmed this session
//                       (ScenarioOneManager.RackGroups) that are not on that layout
//   paths, clearing     the scene's "Dirt Path NN" planks and "Forest Clearing" (the same objects the starting stand keeps trees
//                       clear of), read once
//   you                 the player's position and facing
//   destination         the waypoint set on the Stand Map
public sealed class StandMapSource
{
    // The racks are re-read this often (and at once when a rack is confirmed): reading them walks the Work Plan.
    private const float RackRefreshSeconds = 0.25f;
    private const string PathPlankPrefix = "Dirt Path";

    private readonly ScenarioOneUiRoot ui;
    private readonly StandMapModel model = new StandMapModel();
    private int refreshedFrame = -1;
    private int lastHash = int.MinValue;
    private float nextRackRead;
    private bool racksDirty = true, sceneryRead;

    // The rack the Stand Map is planning, if any. Set by the Stand Map; the minimap filters it out.
    public RackProposal Preview { get; set; }

    public StandMapSource(ScenarioOneUiRoot ui) { this.ui = ui; }

    // The model as of this frame. Cheap to call from several presentations in one frame.
    public StandMapModel Model
    {
        get
        {
            if (refreshedFrame != Time.frameCount) Refresh();
            return model;
        }
    }

    // The model re-read right now, whatever the frame (for a screen that has just changed something).
    public StandMapModel Rebuild()
    {
        racksDirty = true;
        Refresh();
        return model;
    }

    // A rack was confirmed, or the stand geometry changed: read the racks again at the next refresh.
    public void MarkRacksDirty() { racksDirty = true; refreshedFrame = -1; }

    private void Refresh()
    {
        refreshedFrame = Time.frameCount;
        ForestEcologyController eco = ui.Ecology;
        model.HasProperty = eco != null && eco.Cells != null && eco.CellCount > 0;
        if (!model.HasProperty)
        {
            model.Racks.Clear();
            model.HasPlayer = model.HasWaypoint = false;
            Stamp();
            return;
        }

        Rect bounds = eco.StandBounds;
        model.MinX = bounds.xMin;
        model.MinZ = bounds.yMin;
        model.SizeM = bounds.width;
        model.CellsPerAxis = eco.CellsPerAxis;
        model.CellSizeM = eco.CellSizeMeters;

        if (!sceneryRead) ReadScenery();
        if (racksDirty || Time.unscaledTime >= nextRackRead)
        {
            racksDirty = false;
            nextRackRead = Time.unscaledTime + RackRefreshSeconds;
            ReadRacks();
        }

        ForestPlayer player = ui.Player;
        model.HasPlayer = player != null;
        if (model.HasPlayer)
        {
            Vector3 position = player.transform.position, forward = player.transform.forward;
            model.Player = new RackPoint(position.x, position.z);
            // The player's body turns only about the vertical axis (looking up and down tilts the view, not the body), so forward is
            // already level; a straight-up look would leave no direction, in which case the last heading stays.
            if (forward.x * forward.x + forward.z * forward.z > 1e-4f)
                model.PlayerHeadingDegrees = StandMapTransform.HeadingOf(forward.x, forward.z);
        }

        Vector2 target = default;
        model.HasWaypoint = ui.Map != null && ui.Map.TryGetWaypointTarget(out target);
        if (model.HasWaypoint)
            model.Waypoint = new RackPoint(target.x, target.y);
        Stamp();
    }

    // The racks as the planner and the Work Plan know them. The planned layout is derived from the forest, so it is right after a
    // load; a rack confirmed this session that is off that layout is session-only like the rack groups themselves.
    private void ReadRacks()
    {
        model.Racks.Clear();
        RackLayoutTracker layout = ui.RackLayout;
        if (layout != null)
            foreach (RackLayoutLine line in layout.Lines)
                model.Racks.Add(new MapRack
                {
                    Start = line.Start,
                    End = line.End,
                    LayoutIndex = line.Index,
                    // Done = nothing left to plan; no tree left standing in the corridor = it has been harvested.
                    Symbol = !line.Done ? MapSymbol.RackToDo : line.TreesLeft > 0 ? MapSymbol.RackPlanned : MapSymbol.RackOpen
                });
        if (ui.Manager != null)
            foreach (RackGroup group in ui.Manager.RackGroups)
                if (group.LayoutIndex < 0 && (group.Start.X != group.End.X || group.Start.Z != group.End.Z))
                    model.Racks.Add(new MapRack { Start = group.Start, End = group.End, Symbol = MapSymbol.RackOther });
        RackProposal preview = Preview;
        if (preview != null && preview.Selection != null && preview.CanConfirm)
            model.Racks.Add(new MapRack { Start = preview.Selection.Start, End = preview.Selection.End, Symbol = MapSymbol.RackPreview, LayoutIndex = preview.LayoutIndex });
    }

    // The path and the clearing are scene objects that never move, so they are found once. The planks are ordered by name
    // ("Dirt Path 00", "Dirt Path 01", ...) along the path; a jump of more than two metres starts a new line.
    private void ReadScenery()
    {
        sceneryRead = true;
        model.Paths.Clear();
        model.Landmarks.Clear();
        List<Transform> planks = Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t.name.StartsWith(PathPlankPrefix, System.StringComparison.Ordinal))
            .OrderBy(t => t.name, System.StringComparer.Ordinal).ToList();
        MapPath current = null;
        foreach (Transform plank in planks)
        {
            var point = new RackPoint(plank.position.x, plank.position.z);
            if (current != null)
            {
                RackPoint last = current.Points[current.Points.Count - 1];
                float dx = point.X - last.X, dz = point.Z - last.Z;
                if (dx * dx + dz * dz > 4f) current = null;
            }
            if (current == null)
            {
                // A plank is as wide as its x scale; the whole path takes the first plank's width.
                current = new MapPath { WidthM = Mathf.Max(0.5f, plank.lossyScale.x) };
                model.Paths.Add(current);
            }
            current.Points.Add(point);
        }
        GameObject clearing = GameObject.Find("Forest Clearing");
        if (clearing != null)
            model.Landmarks.Add(new MapLandmark
            {
                At = new RackPoint(clearing.transform.position.x, clearing.transform.position.z),
                RadiusM = Mathf.Max(1f, clearing.transform.lossyScale.x * 0.5f),
                Name = "Clearing"
            });
    }

    private void Stamp()
    {
        int hash = model.ContentHash();
        if (hash == lastHash) return;
        lastHash = hash;
        model.Version++;
    }
}
