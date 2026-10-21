using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 6 — stand map. Diagnostic and navigation only: it shows recorded
// cell state and lets the player set a waypoint. It never moves the player and
// never recommends where to fell or plant.
public sealed class StandMapView
{
    private enum Layer { Access, Light, Regeneration, Browse, Marks }

    // Side of the map square in UI units. The cell buttons, and everything the canvas draws over them, are placed on this square by
    // the shared map transform (StandMapTransform); the minimap uses the same transform at its own, smaller side.
    public const float MapSidePx = 608f;

    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    private readonly VisualElement grid, side, layerButtons;
    private readonly StandMapCanvas canvas;
    private readonly Label legend, symbolLegend;
    public StandMapCanvas Canvas => canvas;
    // The cell grid, exposed so verification can compare its real layout with the shared transform.
    public VisualElement Grid => grid;
    private Layer layer = Layer.Access;
    private int selectedCell = -1;
    private int waypointCell = -1;
    private GameObject waypointMarker;
    private string shownKey = "";
    private bool journeyStartedAway;
    public bool HasWaypoint => waypointCell >= 0;

    // Plan rack: the Stand Map's way into the same rack tool the forest uses (RackPlanner / ScenarioOneManager.ConfirmRack). Pick a
    // square on the row you want, check the preview, add it. Presentation state only: nothing changes until the rack is added.
    private bool rackMode, rackHasPoint;
    private float rackAngle;
    private RackPoint rackPoint;
    private RackProposal rackProposal;
    private readonly HashSet<int> rackCells = new HashSet<int>();
    public bool RackMode => rackMode;
    public RackProposal RackPreview => rackProposal;
    // Offered once the first rack has been made in the forest, or at once when the opening rack stage is not in play.
    public bool RackPlanningAvailable => ui.Manager != null && ui.RackTool != null && ui.Ecology != null && ui.Manager.CanEditWorkPlan
        && (!ui.Manager.RackFirstStageActive || ui.Learning.IsDone("rack.create"));

    public StandMapView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        // The UI is built after the ecology already has its geometry (that event was raised before this existed).
        if (ui.Ecology != null) heldGeometry = ui.Ecology.StandGeometryModelVersion;
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal");
        Root.Add(modal);
        VisualElement header = UiKit.Box("modal-header");
        UiKit.Add(header, "Stand map", "title");
        VisualElement buttons = UiKit.Box("row");
        buttons.Add(UiKit.Button("Help [F1]", () => ui.ShowHelp(MenuHelpView.Menu.StandMap)));
        buttons.Add(UiKit.Button("Objectives [O]", () => ui.ShowObjectives()));
        buttons.Add(UiKit.Button("Work Plan", () => ui.ShowWorkPlan()));
        buttons.Add(UiKit.Button("Back to forest [M / Esc]", () => ui.CloseAll(), true, "btn-primary"));
        header.Add(buttons);
        modal.Add(header);

        layerButtons = UiKit.Box("map-tabs");
        layerButtons.name = "map-tabs";
        modal.Add(layerButtons);

        VisualElement body = UiKit.Box("map-body");
        VisualElement left = UiKit.Box("map-left");
        // One square frame holds the cell buttons (interaction and the analytical layers) and, over them, the shared map canvas
        // (property edge, paths, racks, you, destination) drawn by the same code and symbols as the HUD minimap.
        VisualElement frame = UiKit.Box("map-frame");
        frame.name = "map-frame";
        frame.style.width = MapSidePx;
        frame.style.height = MapSidePx;
        frame.style.flexShrink = 0;
        grid = UiKit.Box("map-grid");
        canvas = new StandMapCanvas(MapDetail.Full, false) { name = "map-canvas" };
        canvas.AddToClassList("map-canvas");
        frame.Add(grid);
        frame.Add(canvas);
        left.Add(frame);
        legend = UiKit.Add(left, "", "muted");
        UiKit.Add(left, "NORTH ↑ · white outline / YOU: your position · gold outline: selected destination.", "faint");
        symbolLegend = UiKit.Add(left, "On the map — " + StandMapSymbols.Legend(MapDetail.Full), "faint");
        body.Add(left);
        side = UiKit.Box("card", "map-side");
        body.Add(side);
        modal.Add(body);
    }

    public void Refresh(bool force)
    {
        ForestEcologyController eco = ui.Ecology;
        if (eco == null || eco.Cells == null)
            return;
        int playerCell = ui.Player != null ? eco.GetCellIndex(ui.Player.transform.position) : -1;
        if (selectedCell < 0) selectedCell = playerCell;
        // The rack being planned here is drawn on this map only (the minimap filters it out). Then the canvas is given the shared
        // model, which is re-read now so it is as current as the cells below.
        ui.MapSource.Preview = rackMode ? rackProposal : null;
        StandMapModel model = force ? ui.MapSource.Rebuild() : ui.MapSource.Model;
        // The analytical layers (and rack planning) print text in every square, so the shared lines and markers are drawn quietly over
        // them; the rack being planned keeps its full strength.
        canvas.Sync(model, layer != Layer.Access || rackMode);
        string key = $"{eco.EcologicalYear}|{layer}|{selectedCell}|{playerCell}|{waypointCell}|"
            + (ui.Marking != null ? ui.Marking.LivingMarkedCount + "/" + ui.Marking.LivingCropTreeCount : "")
            + $"|{rackMode}|{rackAngle}|{(rackProposal != null ? rackProposal.TreeCount : -1)}|{(ui.RackLayout != null ? ui.RackLayout.Version : -1)}";
        if (!force && key == shownKey)
            return;
        shownKey = key;

        BuildLayerButtons();
        BuildGrid(eco, playerCell);
        BuildSide(eco);
    }

    private void BuildLayerButtons()
    {
        layerButtons.Clear();
        AddLayerButton("Access & position", Layer.Access);
        AddLayerButton("Light", Layer.Light);
        AddLayerButton("Regeneration", Layer.Regeneration);
        AddLayerButton("Browsing / protection", Layer.Browse);
        AddLayerButton("Fell & crop marks", Layer.Marks);
        if (RackPlanningAvailable)
        {
            Button plan = UiKit.Button(rackMode ? "Plan rack: ON" : "Plan rack", ToggleRackMode, true,
                rackMode ? new[] { "btn-selected" } : new[] { "btn-primary" });
            plan.name = "map-plan-rack";
            plan.style.marginLeft = 12;
            layerButtons.Add(plan);
        }
    }

    private void AddLayerButton(string text, Layer value)
    {
        layerButtons.Add(UiKit.Button(text, () =>
        {
            layer = value;
            if (value != Layer.Access) ui.Learning.Record(value == Layer.Light ? "map.light" : value == Layer.Regeneration ? "map.regeneration"
                : value == Layer.Browse ? "map.browse" : "map.marks");
            Refresh(true);
        }, true, layer == value ? new[] { "btn-selected" } : new string[0]));
    }

    private void BuildGrid(ForestEcologyController eco, int playerCell)
    {
        grid.Clear();
        int n = eco.CellsPerAxis;
        Dictionary<int, (int fell, int crop)> marks = CountMarks(eco);
        float pressure = eco.Browsing.BackgroundPressure;
        int upcoming = eco.EcologicalYear + 1;
        // Every cell is placed by the shared map transform (the same one the minimap and the canvas drawn over this grid use), so a
        // square sits exactly where the world point at its centre is drawn.
        StandMapTransform transform = ui.MapSource.Model.TransformFor(MapSidePx);
        float size = transform.CellSidePx(n);

        for (int z = n - 1; z >= 0; z--)
        {
            for (int x = 0; x < n; x++)
            {
                int index = z * n + x;
                ForestEcologyCell c = eco.Cells[index];
                var cell = new Button(() => { selectedCell = index; ui.Learning.Record("map.select"); if (rackMode) PlaceRack(eco, index); Refresh(true); }) { text = "", focusable = false };
                cell.AddToClassList("map-cell");
                MapPosition corner = transform.CellTopLeft(index, n);
                cell.style.position = Position.Absolute;
                cell.style.left = corner.X;
                cell.style.top = corner.Y;
                cell.style.width = size;
                cell.style.height = size;
                if (index == selectedCell) cell.AddToClassList("map-cell-selected");
                if (index == playerCell) cell.AddToClassList("map-cell-player");
                bool onRack = rackMode && rackCells.Contains(index);
                if (onRack) cell.AddToClassList("map-cell-rack");

                string value;
                Color colour;
                switch (layer)
                {
                    case Layer.Access:
                    {
                        string rack = RackLayoutMark(c.Center, eco.CellSizeMeters);
                        value = index == playerCell ? "YOU" : rack.Trim();
                        colour = rack.Length > 0 ? new Color(0.40f, 0.27f, 0.49f) : new Color(0.18f, 0.29f, 0.18f);
                        break;
                    }
                    case Layer.Regeneration:
                    {
                        float tallest = 0f;
                        int cohorts = 0;
                        foreach (ForestRegenerationCohort r in c.Regeneration)
                            if (r != null && r.Species != null && r.Density > 0f) { cohorts++; tallest = Mathf.Max(tallest, r.Height); }
                        int planted = ui.Manager.PlantedJuveniles.Count(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId) && eco.GetCellIndex(j.position) == index);
                        value = cohorts + planted == 0 ? "none" : (planted > 0 ? $"P{planted} " : "") + (cohorts > 0 ? UiKit.F(tallest, "0.0") + "m" : "");
                        colour = Color.Lerp(new Color(0.25f, 0.2f, 0.15f), new Color(0.3f, 0.75f, 0.3f), Mathf.Clamp01(tallest / 3f + (planted > 0 ? 0.3f : 0f)));
                        break;
                    }
                    case Layer.Browse:
                    {
                        int sheltered = eco.Browsing.Shelters.Count(s => s != null && s.IsEffective(upcoming) && eco.GetCellIndex(new Vector3(s.position.x, 0f, s.position.y)) == index);
                        value = sheltered > 0 ? $"S{sheltered}" : "—";
                        colour = sheltered > 0 ? new Color(0.25f, 0.45f, 0.7f) : Color.Lerp(new Color(0.3f, 0.3f, 0.3f), new Color(0.7f, 0.45f, 0.2f), Mathf.Clamp01(pressure));
                        break;
                    }
                    case Layer.Marks:
                    {
                        marks.TryGetValue(index, out var m);
                        value = (m.fell > 0 ? $"■{m.fell} " : "") + (m.crop > 0 ? $"◆{m.crop}" : "");
                        if (value.Length == 0) value = "·";
                        colour = m.fell > 0 ? new Color(0.65f, 0.2f, 0.2f) : m.crop > 0 ? new Color(0.2f, 0.35f, 0.7f) : new Color(0.22f, 0.25f, 0.22f);
                        break;
                    }
                    default:
                        value = UiKit.F(c.Light, "0.00");
                        colour = Color.Lerp(new Color(0.08f, 0.15f, 0.08f), new Color(0.85f, 0.8f, 0.45f), Mathf.Clamp01(c.Light));
                        break;
                }
                // On the Access layer the shared map draws rack lines and you over the squares: keep this square's mark on the side of
                // the square those are not on, so a ◇, ✓ or YOU is not hidden under a line or the arrow.
                if (layer == Layer.Access && value.Length > 0)
                {
                    float drawnAt = DrawnThroughSquare(eco, c, index == playerCell);
                    cell.style.justifyContent = drawnAt >= 0f && drawnAt < 0.5f ? Justify.FlexEnd : Justify.FlexStart;
                }
                cell.style.backgroundColor = onRack ? Color.Lerp(colour, new Color(0.5f, 0.28f, 0.72f), 0.55f) : colour;
                string squareName = layer == Layer.Access && !rackMode ? ""
                    : UiKit.CellLabel(index, n) + (rackMode ? RackLayoutMark(c.Center, eco.CellSizeMeters) : "");
                VisualElement labels = new VisualElement { pickingMode = PickingMode.Ignore };
                Label name = UiKit.Text(squareName);
                Label number = UiKit.Text(value);
                name.pickingMode = number.pickingMode = PickingMode.Ignore;
                labels.Add(name);
                labels.Add(number);
                cell.Add(labels);
                grid.Add(cell);
            }
        }

        switch (layer)
        {
            case Layer.Access: legend.text = "YOU: your position · ◇: rack route still to plan · ✓: rack planned or open. Select a place, then walk there."; break;
            case Layer.Regeneration: legend.text = "Value: P = planted juveniles alive, then tallest seedling cohort height. Brighter green = taller/more."; break;
            case Layer.Browse: legend.text = $"Site browsing pressure: {BrowsingConditions.PressureBand(pressure)}. S = shelters effective next year; — = no shelter."; break;
            case Layer.Marks: legend.text = "■ = trees marked to fell, ◆ = crop trees (counts per square)."; break;
            default: legend.text = "Value: ground light 0 (dark) to 1 (open). Brighter = more light."; break;
        }
    }

    private static Dictionary<int, (int fell, int crop)> CountMarks(ForestEcologyController eco)
    {
        var counts = new Dictionary<int, (int fell, int crop)>();
        foreach (ForestTree tree in Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None))
        {
            if (tree == null || tree.IsStump || !(tree.IsMarkedForFell || tree.IsCropTree)) continue;
            int i = eco.GetCellIndex(tree.transform.position);
            if (i < 0) continue;
            counts.TryGetValue(i, out var c);
            counts[i] = tree.IsMarkedForFell ? (c.fell + 1, c.crop) : (c.fell, c.crop + 1);
        }
        return counts;
    }

    // Where, as a fraction down from the square's north edge, the map draws you or a planned rack line inside this square
    // (0 = north edge, 1 = south edge); -1 when neither passes through it.
    private float DrawnThroughSquare(ForestEcologyController eco, ForestEcologyCell cell, bool youAreHere)
    {
        float half = eco.CellSizeMeters * 0.5f;
        if (youAreHere && ui.Player != null)
            return Mathf.Clamp01((cell.Center.y + half - ui.Player.transform.position.z) / eco.CellSizeMeters);
        if (ui.RackLayout != null)
            foreach (RackLayoutLine line in ui.RackLayout.Lines)
                if (Mathf.Abs(cell.Center.y - line.AcrossM) <= half)
                    return Mathf.Clamp01((cell.Center.y + half - line.AcrossM) / eco.CellSizeMeters);
        return -1f;
    }

    // "◇" on a square the planned rack layout still has to cross, "✓" where the layout's rack is already in the Work Plan.
    private string RackLayoutMark(Vector2 centre, float size)
    {
        RackLayoutTracker layout = ui.RackLayout;
        if (layout == null) return "";
        foreach (RackLayoutLine line in layout.Lines)
            if (Mathf.Abs(centre.y - line.AcrossM) <= size * 0.5f)
                return line.Done ? StandMapSymbols.RackDoneMark : StandMapSymbols.RackToDoMark;
        return "";
    }

    private void ToggleRackMode()
    {
        rackMode = !rackMode;
        rackHasPoint = false;
        rackProposal = null;
        rackCells.Clear();
        if (rackMode)
        {
            ui.RackTool.Planner.Refresh();
            rackAngle = ui.RackTool.Planner.DefaultAngle(0f, true);
        }
        ui.RackLayout?.Update(true);
        Refresh(true);
    }

    // A pointed square becomes the rack's position; the planner puts the rack on the planted row nearest the square's centre.
    private void PlaceRack(ForestEcologyController eco, int index)
    {
        Vector2 centre = eco.Cells[index].Center;
        // A square the planned layout crosses puts the rack on that layout row (rows are about 1.9 m apart and a square is 5 m, so the
        // centre of the square is usually nearer a different row); anywhere else the planner takes the row nearest the centre.
        float across = centre.y;
        float turn = RackGeometry.NormaliseAngle(rackAngle);
        if (ui.RackLayout != null && (turn < 0.5f || turn > 179.5f))
            foreach (RackLayoutLine line in ui.RackLayout.Lines)
                if (Mathf.Abs(centre.y - line.AcrossM) <= eco.CellSizeMeters * 0.5f) { across = line.AcrossM; break; }
        rackPoint = new RackPoint(centre.x, across);
        rackHasPoint = true;
        RebuildRack(eco);
    }

    private void RebuildRack(ForestEcologyController eco)
    {
        rackCells.Clear();
        rackProposal = null;
        if (!rackHasPoint) return;
        RackPlanner planner = ui.RackTool.Planner;
        planner.Refresh();
        rackProposal = planner.Propose(rackPoint, rackAngle);
        // Remember the snapped line so a nudge moves from the row actually shown.
        RackPoint a = rackProposal.Selection.Start, b = rackProposal.Selection.End;
        rackPoint = new RackPoint((a.X + b.X) * 0.5f, (a.Z + b.Z) * 0.5f);
        float length = rackProposal.Selection.LengthM;
        for (float t = 0f; t <= length; t += eco.CellSizeMeters * 0.5f)
        {
            float f = length > 0f ? t / length : 0f;
            int cell = eco.GetCellIndex(new Vector3(Mathf.Lerp(a.X, b.X, f), 0f, Mathf.Lerp(a.Z, b.Z, f)));
            if (cell >= 0) rackCells.Add(cell);
        }
    }

    private void NudgeRack(ForestEcologyController eco, int direction)
    {
        if (!rackHasPoint) return;
        IReadOnlyList<RackRow> rows = ui.RackTool.Planner.Rows(rackAngle);
        float across = RackGeometry.Across(rackPoint, rackAngle);
        int i = RackRows.Nearest(rows, across);
        if (i < 0) return;
        int target = Mathf.Clamp(i + direction, 0, rows.Count - 1);
        RackPoint d = RackGeometry.Direction(rackAngle);
        float along = RackGeometry.Along(rackPoint, rackAngle), c = rows[target].Centre;
        rackPoint = new RackPoint(d.X * along - d.Z * c, d.Z * along + d.X * c);
        RebuildRack(eco);
        Refresh(true);
    }

    private void TurnRack(ForestEcologyController eco)
    {
        rackAngle = RackGeometry.NormaliseAngle(rackAngle + 90f);
        RebuildRack(eco);
        Refresh(true);
    }

    private void ConfirmRackFromMap(ForestEcologyController eco)
    {
        if (rackProposal == null) return;
        ui.Manager.ConfirmRack(rackProposal, "map");
        ui.RackLayout?.Update(true);
        RebuildRack(eco);
        Refresh(true);
    }

    // The rack card at the top of the side panel: figures from the same proposal the forest tool shows, and the few controls.
    private void BuildRackCard(ForestEcologyController eco)
    {
        VisualElement card = UiKit.Box("card", "rack-panel");
        card.name = "map-rack-card";
        RackLayoutTracker layout = ui.RackLayout;
        UiKit.Add(card, "Plan an extraction rack", "rack-heading");
        if (layout != null && layout.Total > 0)
            UiKit.Add(card, $"Planned layout: {layout.Done} of {layout.Total} racks in the Work Plan. ◇ marks squares on a rack still to do.", "muted");
        RackProposal p = rackProposal;
        if (p == null)
            UiKit.Add(card, "Click a square on the planted row you want to open.", "body");
        else
        {
            if (p.LayoutIndex >= 0) UiKit.Add(card, $"Planned rack {p.LayoutIndex + 1} of {p.LayoutCount}", "body");
            else if (p.LayoutCount > 0) UiKit.Add(card, "Not on the planned layout", "body");
            if (!p.CanConfirm)
                UiKit.Add(card, string.IsNullOrEmpty(p.Problem) ? "No tree can be felled on this line." : p.Problem, "body");
            else
            {
                UiKit.Add(card, $"{p.TreeCount} tree{(p.TreeCount == 1 ? "" : "s")}", "rack-line");
                UiKit.Add(card, $"Estimated timber: {UiKit.F(p.SoldVolumeM3, "0.0")} m³ sold ({UiKit.F(p.StemVolumeM3, "0.0")} m³ stem volume)", "body");
                UiKit.Add(card, $"Estimated net return: {UiKit.SignedMoney(p.NetCents)} (timber {UiKit.Money(p.RevenueCents)}, harvesting {UiKit.Money(p.CostCents)}; this rack on its own)", "body");
            }
            if (p.Selection.SkippedCropTrees > 0) UiKit.Add(card, $"{p.Selection.SkippedCropTrees} Crop Tree(s) on the line are kept.", "muted");
        }
        VisualElement row = UiKit.Row(card, "row-wrap");
        row.Add(UiKit.Button("◀ Row", () => NudgeRack(eco, -1), rackProposal != null));
        row.Add(UiKit.Button("Row ▶", () => NudgeRack(eco, 1), rackProposal != null));
        row.Add(UiKit.Button("Turn 90°", () => TurnRack(eco), rackProposal != null));
        Button add = UiKit.Button("Add rack to Work Plan", () => ConfirmRackFromMap(eco), p != null && p.CanConfirm, "btn-primary");
        add.name = "map-rack-confirm";
        card.Add(add);
        Button cancel = UiKit.Button("Cancel", ToggleRackMode, true, "btn-ghost");
        cancel.name = "map-rack-cancel";
        card.Add(cancel);
        UiKit.Add(card, "Nothing is cut until you approve the Work Plan.", "faint");
        side.Add(card);
    }

    private void BuildSide(ForestEcologyController eco)
    {
        side.Clear();
        // Planning a rack: the rack card is the whole side panel (the square diagnosis is back when the mode is off).
        if (rackMode)
        {
            BuildRackCard(eco);
            return;
        }
        if (selectedCell < 0 || selectedCell >= eco.Cells.Length)
        {
            UiKit.Add(side, "Select a square to see its diagnosis.", "body");
            return;
        }
        ForestEcologyCell c = eco.Cells[selectedCell];
        VisualElement diagnosis = side;
        if (layer == Layer.Access)
        {
            UiKit.Add(side, "Your woodland · north is up", "heading");
            UiKit.Add(side, "The grid shows your stand and its planned access routes. White marks where you are standing.", "body");
            UiKit.Add(side, ui.Learning.Summary, "body");
            if (ui.RackLayout != null && ui.RackLayout.Total > 0)
                UiKit.Add(side, $"Access racks: {ui.RackLayout.Done} of {ui.RackLayout.Total} planned or open.", "muted");
            UiKit.Add(side, "Selected destination: " + UiKit.CellLabel(selectedCell, eco.CellsPerAxis), "body");
            var details = new Foldout { text = "Site details · light and young trees", value = false, name = "map-site-details" };
            side.Add(details);
            diagnosis = details;
        }
        UiKit.Add(diagnosis, "Square " + UiKit.CellLabel(selectedCell, eco.CellsPerAxis), "heading");
        UiKit.Line(diagnosis, "Ground light", $"{UiKit.F(c.Light, "0.00")} ({RegenerationDiagnostics.LightBand(c.Light)})");
        UiKit.Line(diagnosis, "Canopy cover", UiKit.F(c.Canopy, "0.00"));
        UiKit.Add(diagnosis, "Regeneration: " + ScenarioOneUiFacts.RegenerationSummary(eco, selectedCell, ui.Manager.PlantedJuveniles), "body");

        float pressure = eco.Browsing.BackgroundPressure;
        var diagnoses = RegenerationDiagnostics.DiagnoseCell(eco, selectedCell);
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        foreach (PlantedJuvenile j in ui.Manager.PlantedJuveniles.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId) && eco.GetCellIndex(j.position) == selectedCell).Take(4))
            diagnoses.Add(RegenerationDiagnostics.Diagnose(eco, j.position, ui.Manager.PlantedJuveniles, spawner));
        if (diagnoses.Count == 0)
        {
            RegenerationDiagnosis empty = default;
            empty.Light = c.Light;
            UiKit.Add(diagnosis, "Why: " + ScenarioOneUiFacts.Why(empty, eco, selectedCell), "muted");
        }
        foreach (RegenerationDiagnosis d in diagnoses.Take(6))
        {
            UiKit.Add(diagnosis, $"{d.SpeciesName}{(d.IsPlantedIndividual ? " " + d.JuvenileId : "")}, {UiKit.F(d.Height, "0.00")} m — {ScenarioOneUiFacts.BrowseState(d, pressure)}", "body");
            UiKit.Add(diagnosis, "Why: " + ScenarioOneUiFacts.Why(d, eco, selectedCell), "muted");
        }

        int index = selectedCell;
        side.Add(UiKit.Button(waypointCell == index ? "Clear waypoint" : "Set waypoint", () =>
        {
            if (waypointCell == index) ClearWaypoint(); else SetWaypoint(eco, index);
            Refresh(true);
        }, true, "btn-primary"));
        UiKit.Add(side, "A waypoint marks the square in the forest and in the status panel. Walk there yourself; the map does not move you.", "faint");
    }

    public bool SetWaypointCell(ForestEcologyController eco, int index)
    {
        if (eco == null || eco.Cells == null || index < 0 || index >= eco.CellCount) return false;
        selectedCell = index;
        SetWaypoint(eco, index);
        Refresh(true);
        return true;
    }

    private void SetWaypoint(ForestEcologyController eco, int index)
    {
        waypointCell = index;
        journeyStartedAway = ui.Player != null && eco.GetCellIndex(ui.Player.transform.position) != index;
        ui.Learning.Record("map.waypoint");
        PlaceWaypointMarker(eco, index);
    }

    private void PlaceWaypointMarker(ForestEcologyController eco, int index)
    {
        Vector2 centre = eco.Cells[index].Center;
        if (waypointMarker == null)
        {
            // A tall thin post visible over regeneration; no collider so it never blocks play.
            waypointMarker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            waypointMarker.name = "Scenario One Waypoint";
            Object.Destroy(waypointMarker.GetComponent<Collider>());
            waypointMarker.transform.localScale = new Vector3(0.15f, 3f, 0.15f);
            Renderer renderer = waypointMarker.GetComponent<Renderer>();
            renderer.material.color = new Color(1f, 0.85f, 0.2f);
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
        float ground = 0f;
        if (Physics.Raycast(new Vector3(centre.x, 200f, centre.y), Vector3.down, out RaycastHit hit, 400f, ~0, QueryTriggerInteraction.Ignore))
            ground = hit.point.y;
        waypointMarker.transform.position = new Vector3(centre.x, ground + 3f, centre.y);
    }

    // The cell indices held here belong to one grid. When the stand geometry changes (a load, or Reference
    // preview entering and leaving Legacy40) the selection and waypoint of the old grid are set aside rather
    // than reinterpreted on the new grid, and the waypoint is put back if that geometry returns, so previewing
    // the Reference does not lose it. Presentation state only; it never enters a save.
    private int heldGeometry = StandGeometryModel.Legacy40;
    private int stashedGeometry = -1, stashedWaypointCell = -1;

    public void OnStandGeometryApplied(ForestEcologyController eco, int newModel)
    {
        if (newModel == heldGeometry)
            return;
        if (waypointCell >= 0)
        {
            stashedGeometry = heldGeometry;
            stashedWaypointCell = waypointCell;
        }
        selectedCell = -1;
        waypointCell = -1;
        DestroyWaypoint();
        shownKey = "";
        heldGeometry = newModel;
        if (stashedWaypointCell >= 0 && stashedGeometry == newModel && eco?.Cells != null && stashedWaypointCell < eco.Cells.Length)
        {
            waypointCell = stashedWaypointCell;
            journeyStartedAway = ui.Player != null && eco.GetCellIndex(ui.Player.transform.position) != waypointCell;
            PlaceWaypointMarker(eco, waypointCell);
            stashedGeometry = -1;
            stashedWaypointCell = -1;
        }
    }

    private void ClearWaypoint()
    {
        waypointCell = -1;
        DestroyWaypoint();
    }

    public void DestroyWaypoint()
    {
        if (waypointMarker != null)
            Object.Destroy(waypointMarker);
        waypointMarker = null;
    }

    public bool TryGetWaypointTarget(out Vector2 target)
    {
        ForestEcologyController eco = ui.Ecology;
        target = default;
        if (waypointCell < 0 || eco == null || eco.Cells == null || waypointCell >= eco.Cells.Length)
            return false;
        target = eco.Cells[waypointCell].Center;
        return true;
    }

    public string WaypointDescription(Vector3 playerPosition)
    {
        ForestEcologyController eco = ui.Ecology;
        if (!TryGetWaypointTarget(out Vector2 target)) return "";
        Vector2 delta = target - new Vector2(playerPosition.x, playerPosition.z);
        bool walking = ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !ui.Manager.AnyPanelOpen;
        if (walking && journeyStartedAway && delta.magnitude < eco.CellSizeMeters * 0.5f)
        {
            ui.Learning.Record("map.arrive");
            if ((ui.Marking != null && ui.Marking.AimingAtGround && eco.GetCellIndex(ui.Marking.AimedGroundPoint) == waypointCell)
                || (ui.Player != null && ui.Player.InspectedTree != null && eco.GetCellIndex(ui.Player.InspectedTree.transform.position) == waypointCell))
                ui.Learning.Record("map.inspectsite");
        }
        string cell = UiKit.CellLabel(waypointCell, eco.CellsPerAxis);
        if (delta.magnitude < eco.CellSizeMeters * 0.5f)
            return $"Waypoint {cell}: you are here";
        return $"Waypoint {cell}: {Mathf.RoundToInt(delta.magnitude)} m {Compass(delta)}";
    }

    private static string Compass(Vector2 delta)
    {
        string[] names = { "east", "north-east", "north", "north-west", "west", "south-west", "south", "south-east" };
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        int sector = Mathf.RoundToInt(angle / 45f);
        return names[((sector % 8) + 8) % 8];
    }
}
