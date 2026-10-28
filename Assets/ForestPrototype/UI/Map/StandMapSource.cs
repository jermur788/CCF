using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Builds the ONE StandMapModel from the live game, for both the full Stand Map and the HUD minimap. It reads; it never writes to
// the forest, the Work Plan or a save, so map presentation cannot change the simulation.
//
// Where each thing on the map comes from (no copies are kept):
//   property, grid      the ecology controller's stand bounds and cells
//   trees               every ForestTree (position, species, crown radius, height, DBH, living / dead / stump, mark)
//   planted juveniles   ScenarioOneManager.PlantedJuveniles (alive, not yet promoted to a tree)
//   shelters            the ecology's browsing shelters (position, still effective next year or not)
//   racks               RackLayoutTracker.Lines (the planned layout, derived from the forest) and the racks confirmed this session
//                       (ScenarioOneManager.RackGroups) that are not on that layout
//   paths, clearing     the scene's "Dirt Path NN" planks and "Forest Clearing" (the same objects the starting stand keeps trees
//                       clear of), read once
//   ground              level unless the scene has a Terrain (it has none, so no relief is drawn)
//   you                 the player's position and facing
//   destination         the waypoint set on the Stand Map
// The rack proposal under the pointer while a rack is being drawn, moved or rotated: where it would be, and whether the editing
// rules accept it. Display only.
public readonly struct RackEditGhost
{
    public readonly RackPoint Start, End;
    public readonly float WidthM;
    public readonly bool Valid;

    public RackEditGhost(RackPoint start, RackPoint end, float widthM, bool valid)
    {
        Start = start;
        End = end;
        WidthM = widthM;
        Valid = valid;
    }
}

public sealed class StandMapSource
{
    // The racks, planted juveniles and shelters are re-read this often (and at once when a rack is confirmed): reading the racks walks
    // the Work Plan.
    private const float RackRefreshSeconds = 0.25f;
    // The trees are re-read at once when one is felled or dies, a year passes or a mark changes, and otherwise this often, as a safety
    // net for a change that raises no event (a saved game being loaded).
    private const float TreeScanSeconds = 1f;
    private const string PathPlankPrefix = "Dirt Path";

    private readonly ScenarioOneUiRoot ui;
    private readonly StandMapModel model = new StandMapModel();
    private int refreshedFrame = -1;
    private float nextRackRead, nextTreeScan;
    private bool racksDirty = true, sceneryRead, treesDirty = true;
    private long sceneryGeometry = -1;
    private int scannedYear = int.MinValue, scannedMarked = -1, scannedCrop = -1;

    // How many times the trees were read and how many of those changed the canopy. A verification reads these to show a walk reads nothing.
    public int TreeScans { get; private set; }
    public int CanopyChanges { get; private set; }

    // The rack the Stand Map is planning, if any. Set by the Stand Map; the minimap filters it out.
    public RackProposal Preview { get; set; }
    // Display mode only; suggestions are rebuilt from the foundation and never enter the rack plan.
    private bool showRackSuggestions;
    // The foundation's answer is taken once per planning session and per full rebuild, not at every pointer move of a drag: it
    // needs a scan of the plantation and depends only on the fixed plantation positions.
    private IReadOnlyList<SuggestedRackSegment> suggestions;
    public bool ShowRackSuggestions
    {
        get => showRackSuggestions;
        set
        {
            if (value && !showRackSuggestions) suggestions = null;
            showRackSuggestions = value;
        }
    }
    // Direct editing (RACK-V2-05): which plan rack is selected, and the proposal being dragged. Both are presentation only; the
    // plan changes only when an edit is applied to ScenarioOneManager, never from here.
    public int SelectedRackId { get; set; }
    public RackEditGhost? EditGhost { get; set; }

    public StandMapSource(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        ForestTree.Felled += OnTreeChanged;
        ForestTree.MortalityApplied += OnTreeChanged;
    }

    // The events are static: let go of them with the UI.
    public void Dispose()
    {
        ForestTree.Felled -= OnTreeChanged;
        ForestTree.MortalityApplied -= OnTreeChanged;
    }

    private void OnTreeChanged(ForestTree tree) { treesDirty = true; refreshedFrame = -1; }

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
        treesDirty = true;
        suggestions = null;
        Refresh();
        return model;
    }

    // A rack was confirmed, or the stand geometry changed: read the racks again at the next refresh.
    public void MarkRacksDirty() { racksDirty = true; treesDirty = true; suggestions = null; refreshedFrame = -1; }

    // Only the racks changed (a selection, a drag ghost): re-read them without scanning the forest again.
    public void MarkRackEditDirty() { racksDirty = true; refreshedFrame = -1; }

    private void Refresh()
    {
        refreshedFrame = Time.frameCount;
        ForestEcologyController eco = ui.Ecology;
        model.HasProperty = eco != null && eco.Cells != null && eco.CellCount > 0;
        if (!model.HasProperty)
        {
            model.Racks.Clear();
            model.Trees.Clear();
            model.Juveniles.Clear();
            model.Shelters.Clear();
            model.HasPlayer = model.HasWaypoint = false;
            model.StampTrees();
            model.Restamp();
            return;
        }

        Rect bounds = eco.StandBounds;
        model.MinX = bounds.xMin;
        model.MinZ = bounds.yMin;
        model.SizeM = bounds.width;
        model.CellsPerAxis = eco.CellsPerAxis;
        model.CellSizeM = eco.CellSizeMeters;

        // The track reaches the property edge only in a world made with the approach (StandApproachPath), so the path is read again when that changes.
        long sceneryKey = (long)eco.StandGeometryModelVersion * 1000000L + StandApproachPath.Version;
        if (!sceneryRead || sceneryGeometry != sceneryKey)
        {
            sceneryGeometry = sceneryKey;
            ReadScenery();
        }

        int marked = ui.Marking != null ? ui.Marking.LivingMarkedCount : -1, crop = ui.Marking != null ? ui.Marking.LivingCropTreeCount : -1;
        if (treesDirty || eco.EcologicalYear != scannedYear || marked != scannedMarked || crop != scannedCrop || Time.unscaledTime >= nextTreeScan)
        {
            treesDirty = false;
            scannedYear = eco.EcologicalYear; scannedMarked = marked; scannedCrop = crop;
            nextTreeScan = Time.unscaledTime + TreeScanSeconds;
            ReadTrees();
        }

        if (racksDirty || Time.unscaledTime >= nextRackRead)
        {
            racksDirty = false;
            nextRackRead = Time.unscaledTime + RackRefreshSeconds;
            ReadRacks();
            ReadSmallLayers(eco);
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
        model.Restamp();
    }

    private static readonly System.Comparison<MapTree> ByHeight = (a, b) =>
    {
        int c = a.HeightM.CompareTo(b.HeightM);
        if (c != 0) return c;
        c = a.X.CompareTo(b.X);
        return c != 0 ? c : a.Z.CompareTo(b.Z);
    };

    // Every tree on the property, shortest first so a taller crown is drawn over a shorter one. A tree's species decides only how it is drawn.
    private void ReadTrees()
    {
        TreeScans++;
        ForestTree[] found = Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        model.Trees.Clear();
        var names = new HashSet<string>[4];
        foreach (ForestTree tree in found)
        {
            if (tree == null) continue;
            Vector3 p = tree.transform.position;
            MapTreeState state = tree.IsStump ? MapTreeState.Stump : tree.IsBiologicallyDead ? MapTreeState.StandingDead : MapTreeState.Living;
            MapTreeKind kind = KindOf(tree.Species != null ? tree.Species.SpeciesId : "");
            if (state == MapTreeState.Living && tree.Species != null && !string.IsNullOrEmpty(tree.Species.DisplayName))
            {
                int k = (int)kind;
                if (names[k] == null) names[k] = new HashSet<string>();
                names[k].Add(tree.Species.DisplayName);
            }
            model.Trees.Add(new MapTree
            {
                X = p.x, Z = p.z, Kind = kind, State = state,
                CrownRadiusM = state == MapTreeState.Stump ? 0f : tree.SimulationCrownRadiusMeters,
                HeightM = state == MapTreeState.Stump ? 0f : tree.SimulationHeightMeters,
                DbhCm = state == MapTreeState.Stump ? 0f : tree.SimulationDiameterCm,
                Mark = tree.IsMarkedForFell ? MapTreeMark.Fell : tree.IsCropTree ? MapTreeMark.Crop : MapTreeMark.None,
                Seed = SeedOf(tree.TreeId)
            });
        }
        model.Trees.Sort(ByHeight);
        for (int k = 0; k < 4; k++)
            model.KindLabels[k] = names[k] == null ? "" : string.Join(", ", names[k].OrderBy(n => n, System.StringComparer.Ordinal));
        if (model.StampTrees()) CanopyChanges++;
    }

    // Conifer or broadleaf is a botanical fact about the species, applied by species id. A species the map has no look for is drawn as
    // "other broadleaf" rather than guessed to be a particular one.
    public static MapTreeKind KindOf(string speciesId)
    {
        switch (speciesId)
        {
            case "sitka-spruce":
            case "scots-pine": return MapTreeKind.Conifer;
            case "beech": return MapTreeKind.Beech;
            case "sessile-oak": return MapTreeKind.Oak;
            default: return MapTreeKind.OtherBroadleaf;
        }
    }

    // A stable number from the tree's id (FNV-1a), used only to vary how its crown is drawn.
    private static int SeedOf(string id)
    {
        unchecked
        {
            uint h = 2166136261u;
            if (id != null) foreach (char c in id) { h ^= c; h *= 16777619u; }
            return (int)h;
        }
    }

    // The racks as the planner and the Work Plan know them. The planned layout is derived from the forest, so it is right after a
    // load; a rack confirmed this session that is off that layout is session-only like the rack groups themselves.
    private void ReadRacks()
    {
        model.Racks.Clear();
        model.MiniGuidance.Clear();
        bool v2 = ui.Manager != null && ui.Manager.RackPlanningV2Active;
        if (v2)
        {
            // Both presentations read the same two authoritative sources and keep no rack list of their own: the foundation's
            // suggested routes (guidance) and the plan's rack objects (ScenarioOneManager.RackPlan). Nothing here writes to either.
            bool confirmed = ui.Manager.RackPlanConfirmed;
            if (!confirmed)
            {
                // The one plantation-tree snapshot and the foundation call live on the manager, so the map and the editing rules
                // read the same guidance. Dashed guidance shows on the minimap until the plan is confirmed; on the Stand Map it is
                // part of planning, so outside planning mode it goes to MiniGuidance and Racks holds no suggestion.
                if (suggestions == null) suggestions = ui.Manager.GetSuggestedRackLayout();
                foreach (SuggestedRackSegment segment in suggestions)
                    (ShowRackSuggestions ? model.Racks : model.MiniGuidance).Add(new MapRack { Start = segment.Entrance, End = segment.Far, WidthM = segment.WidthM, Symbol = MapSymbol.RackSuggested });
            }
            foreach (RackPlanRack rack in ui.Manager.RackPlan)
                model.Racks.Add(new MapRack
                {
                    Start = rack.Entrance,
                    End = rack.Far,
                    WidthM = rack.WidthM,
                    // A rack with executed removals is locked and shown as harvested; every other player rack is the solid amber one.
                    Symbol = rack.IsLocked ? MapSymbol.RackHarvested : MapSymbol.RackPlayer,
                    RackId = rack.Id,
                    Locked = rack.IsLocked,
                    // The minimap's solid lines are the confirmed plan, and executed work is always real; a draft rack is planning only.
                    FullOnly = !confirmed && !rack.IsLocked,
                    Selected = ShowRackSuggestions && rack.Id == SelectedRackId
                });
        }
        // The authored layout lines and the session rack groups belong to the model-0 rack tool. A V2 world's racks are the plan's own
        // objects, so those legacy shapes never enter it.
        RackLayoutTracker layout = ui.RackLayout;
        if (layout != null && !v2)
            foreach (RackLayoutLine line in layout.Lines)
                model.Racks.Add(new MapRack
                {
                    Start = line.Start,
                    End = line.End,
                    LayoutIndex = line.Index,
                    // Done = nothing left to plan; no tree left standing in the corridor = it has been harvested.
                    Symbol = !line.Done ? MapSymbol.RackToDo : line.TreesLeft > 0 ? MapSymbol.RackPlanned : MapSymbol.RackOpen
                });
        if (ui.Manager != null && !v2)
            foreach (RackGroup group in ui.Manager.RackGroups)
                if (group.LayoutIndex < 0 && (group.Start.X != group.End.X || group.Start.Z != group.End.Z))
                    model.Racks.Add(new MapRack { Start = group.Start, End = group.End, Symbol = MapSymbol.RackOther });
        RackProposal preview = Preview;
        if (preview != null && preview.Selection != null && preview.CanConfirm)
            model.Racks.Add(new MapRack { Start = preview.Selection.Start, End = preview.Selection.End, Symbol = MapSymbol.RackPreview, LayoutIndex = preview.LayoutIndex });
        if (EditGhost.HasValue && ShowRackSuggestions)
        {
            RackEditGhost ghost = EditGhost.Value;
            model.Racks.Add(new MapRack { Start = ghost.Start, End = ghost.End, WidthM = ghost.WidthM, Symbol = ghost.Valid ? MapSymbol.RackDraft : MapSymbol.RackRefused });
        }
    }

    // Planted juveniles (individuals, at their exact places) and browsing shelters. Both lists are short.
    private void ReadSmallLayers(ForestEcologyController eco)
    {
        model.Juveniles.Clear();
        IReadOnlyList<PlantedJuvenile> juveniles = ui.Manager != null ? ui.Manager.PlantedJuveniles : null;
        for (int i = 0; juveniles != null && i < juveniles.Count; i++)
        {
            PlantedJuvenile j = juveniles[i];
            if (j == null || !j.alive || !string.IsNullOrEmpty(j.promotedTreeId)) continue;
            model.Juveniles.Add(new MapJuvenile { X = j.position.x, Z = j.position.z, HeightM = j.heightMeters, Kind = KindOf(j.speciesId) });
        }
        model.Shelters.Clear();
        List<BrowseShelter> shelters = eco.Browsing != null ? eco.Browsing.Shelters : null;
        int upcoming = eco.EcologicalYear + 1;
        for (int i = 0; shelters != null && i < shelters.Count; i++)
            if (shelters[i] != null)
                model.Shelters.Add(new MapShelter { X = shelters[i].position.x, Z = shelters[i].position.y, Effective = shelters[i].IsEffective(upcoming) });
    }

    // The path and the clearing are scene objects that never move, so they are found once. The planks are ordered by name
    // ("Dirt Path 00", "Dirt Path 01", ...) along the path; a jump of more than two metres starts a new line.
    private void ReadScenery()
    {
        sceneryRead = true;
        model.Paths.Clear();
        model.Landmarks.Clear();
        model.GroundIsLevel = Terrain.activeTerrain == null;
        List<Transform> planks = Object.FindObjectsByType<Transform>(FindObjectsInactive.Exclude)
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
}
