using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 6 — stand map. A forestry plan of the property: the real trees, the racks, the path, the clearing, you and your destination, on
// a local survey grid. The analytical layers (Light, Regeneration, Browsing, Marks) are translucent overlays on that same plan, so the player
// never loses orientation, and the figure behind a colour is read by pointing at it or from the selected-square card.
//
// Diagnostic and navigation only: it shows recorded state and lets the player set a waypoint and plan a rack through the existing rack
// tool. It never moves the player and never recommends where to fell or plant.
//
// The 5 m ecology squares remain the unit of selection, waypoint and rack placement, and the data behind the Light and Regeneration
// overlays; they are invisible buttons laid over the plan by the shared map transform, not a drawn grid.
public sealed class StandMapView
{
    private enum Layer { Access, Light, Regeneration, Browse, Marks }

    // Side of the map square in UI units. The square buttons, and everything the surface draws under them, are placed on this square by
    // the shared map transform (StandMapTransform); the minimap uses the same transform at its own, smaller side.
    public const float MapSidePx = 688f;
    private const float OverviewSidePx = 144f;
    private const string DefaultCaption = "Local grid · metres east and north of the south-west corner · north is up";

    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    private readonly VisualElement grid, side, layerButtons, toolButtons, frame, eastLabels, northLabels, overviewFacts;
    private readonly StandMapSurface surface, overviewSurface;
    private readonly StandMapLegendView legendView;
    private readonly Label caption, northMark, scaleMark;
    public StandMapCanvas Canvas => surface.Canvas;
    public StandMapSurface Surface => surface;
    public StandMapCanopyLayer Canopy => surface.Canopy;
    public StandMapLegendView Legend => legendView;
    // The overview minimap in the right-hand stack: the same map, filtered as the HUD minimap is.
    public StandMapSurface Overview => overviewSurface;
    // The cell grid, exposed so verification can compare its real layout with the shared transform.
    public VisualElement Grid => grid;
    private Layer layer = Layer.Access;
    private Button[] cells;
    private int selectedCell = -1;
    private int waypointCell = -1;
    private GameObject waypointMarker;
    private string shownKey = "";
    private float labelledSize = -1f;
    private int factsVersion = -1;
    private bool journeyStartedAway;
    public bool HasWaypoint => waypointCell >= 0;
    public string CaptionText => caption.text;

    // Plan rack: the Stand Map's way into the same rack tool the forest uses (RackPlanner / ScenarioOneManager.ConfirmRack). Pick a
    // square on the row you want, check the preview, add it. Presentation state only: nothing changes until the rack is added.
    private bool rackMode, rackHasPoint;
    private float rackAngle;
    private RackPoint rackPoint;
    private RackProposal rackProposal;
    private readonly HashSet<int> rackCells = new HashSet<int>();
    public bool RackMode => rackMode;
    public RackProposal RackPreview => rackProposal;
    // V2 planning is available immediately; the legacy tool retains its original teaching gate.
    public bool RackPlanningAvailable => ui.Manager != null && ui.RackTool != null && ui.Ecology != null && ui.Manager.CanEditWorkPlan
        && (ui.Manager.RackPlanningV2Active || !ui.Manager.RackFirstStageActive || ui.Learning.IsDone("rack.create"));

    public StandMapView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        // The UI is built after the ecology already has its geometry (that event was raised before this existed).
        if (ui.Ecology != null) heldGeometry = ui.Ecology.StandGeometryModelVersion;
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal", "modal-map");
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
        toolButtons = UiKit.Box("row");
        toolButtons.name = "map-tools";
        toolButtons.style.marginBottom = 6;
        modal.Add(toolButtons);

        VisualElement body = UiKit.Box("map-body");
        VisualElement left = UiKit.Box("map-left");
        VisualElement wrap = UiKit.Box("map-wrap");
        northLabels = UiKit.Box("map-north-labels");
        northLabels.style.height = MapSidePx;
        wrap.Add(northLabels);
        VisualElement column = UiKit.Box("map-column");
        // One square frame holds the surface (the forest picture, the analytical field and the shapes, all drawn from the shared model and
        // the shared transform) and, over it, the invisible square buttons that make the map interactive.
        frame = UiKit.Box("map-frame");
        frame.name = "map-frame";
        frame.style.width = MapSidePx;
        frame.style.height = MapSidePx;
        frame.style.flexShrink = 0;
        surface = new StandMapSurface(MapDetail.Full) { name = "map-surface" };
        surface.style.position = Position.Absolute;
        surface.style.left = surface.style.top = surface.style.right = surface.style.bottom = 0;
        frame.Add(surface);
        // The two words printed on the map itself, beside the north arrow and the scale bar that the plan draws.
        northMark = UiKit.Text("N", "map-north-label");
        northMark.pickingMode = PickingMode.Ignore;
        scaleMark = UiKit.Text("", "map-scale-label");
        scaleMark.pickingMode = PickingMode.Ignore;
        frame.Add(northMark);
        frame.Add(scaleMark);
        grid = UiKit.Box("map-grid");
        frame.Add(grid);
        frame.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        frame.RegisterCallback<PointerLeaveEvent>(_ => caption.text = DefaultCaption);
        column.Add(frame);
        eastLabels = UiKit.Box("map-east-labels");
        eastLabels.style.width = MapSidePx;
        column.Add(eastLabels);
        caption = UiKit.Add(column, DefaultCaption, "map-caption");
        caption.name = "map-caption";
        wrap.Add(column);
        left.Add(wrap);
        body.Add(left);

        // Two columns to the right of the map: the overview and the legend, then the card for the square or the rack. The card has the
        // window's full height, as the Stand Map's side card always has.
        VisualElement columnA = UiKit.Box("map-col-a");
        VisualElement overview = UiKit.Box("card", "map-card", "map-overview");
        overview.name = "map-overview";
        UiKit.Add(overview, "Overview", "map-card-title");
        VisualElement overviewRow = UiKit.Box("map-overview-row");
        VisualElement miniFrame = UiKit.Box("map-mini-frame");
        miniFrame.style.width = OverviewSidePx;
        miniFrame.style.height = OverviewSidePx;
        overviewSurface = new StandMapSurface(MapDetail.Mini) { name = "map-overview-surface" };
        overviewSurface.style.position = Position.Absolute;
        overviewSurface.style.left = overviewSurface.style.top = overviewSurface.style.right = overviewSurface.style.bottom = 0;
        miniFrame.Add(overviewSurface);
        overviewRow.Add(miniFrame);
        overviewFacts = UiKit.Box("map-facts");
        overviewRow.Add(overviewFacts);
        overview.Add(overviewRow);
        columnA.Add(overview);
        legendView = new StandMapLegendView();
        columnA.Add(legendView.Root);
        body.Add(columnA);
        side = UiKit.Box("card", "map-side");
        VisualElement columnB = UiKit.Box("map-col-b");
        columnB.Add(side);
        body.Add(columnB);
        modal.Add(body);
    }

    private MapOverlay Overlay => layer == Layer.Access ? MapOverlay.None : layer == Layer.Light ? MapOverlay.Light : layer == Layer.Regeneration ? MapOverlay.Regeneration
        : layer == Layer.Browse ? MapOverlay.Browsing : MapOverlay.Marks;

    public void Refresh(bool force)
    {
        ForestEcologyController eco = ui.Ecology;
        if (eco == null || eco.Cells == null)
            return;
        int playerCell = ui.Player != null ? eco.GetCellIndex(ui.Player.transform.position) : -1;
        if (selectedCell < 0) selectedCell = playerCell;
        // The rack being planned here is drawn on this map only (the minimap filters it out). Then the surface is given the shared
        // model, which is re-read now so it is as current as the squares below.
        ui.MapSource.Preview = rackMode && !ui.Manager.RackPlanningV2Active ? rackProposal : null;
        ui.MapSource.ShowRackSuggestions = rackMode && ui.Manager.RackPlanningV2Active;
        StandMapModel model = force ? ui.MapSource.Rebuild() : ui.MapSource.Model;
        // Opening the map must show the forest as it is now: finish any redraw that is part-way.
        if (force)
        {
            surface.Canopy.FinishNow(model);
            overviewSurface.Canopy.FinishNow(model);
        }
        surface.Sync(model, Overlay);
        overviewSurface.Sync(model, MapOverlay.None);
        string key = $"{eco.EcologicalYear}|{layer}|{selectedCell}|{playerCell}|{waypointCell}|{model.StaticVersion}|{model.CanopyVersion}"
            + $"|{rackMode}|{rackAngle}|{(rackProposal != null ? rackProposal.TreeCount : -1)}|{(ui.RackLayout != null ? ui.RackLayout.Version : -1)}";
        if (!force && key == shownKey)
            return;
        shownKey = key;

        BuildLayerButtons();
        EnsureCells(eco, model);
        UpdateCells(playerCell);
        UpdateField(eco);
        UpdateSurveyLabels(model);
        UpdateOverview(model, eco);
        legendView.Show(model, Overlay, key, rackMode && ui.Manager.RackPlanningV2Active);
        BuildSide(eco, model);
    }

    private void BuildLayerButtons()
    {
        layerButtons.Clear();
        AddLayerButton("Access & position", Layer.Access);
        AddLayerButton("Light", Layer.Light);
        AddLayerButton("Regeneration", Layer.Regeneration);
        AddLayerButton("Browsing / protection", Layer.Browse);
        AddLayerButton("Fell & crop marks", Layer.Marks);
        toolButtons.Clear();
        if (RackPlanningAvailable)
        {
            string label = ui.Manager.RackPlanningV2Active
                ? (rackMode ? "Planning Racks — Active" : "Plan Racks")
                : (rackMode ? "Plan rack: ON" : "Plan rack");
            Button plan = UiKit.Button(label, ToggleRackMode, true,
                rackMode ? new[] { "btn-selected" } : new[] { "btn-primary" });
            plan.name = "map-plan-rack";
            toolButtons.Add(plan);
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

    // The 5 m squares: one invisible button each, made once per geometry and placed by the shared map transform, so a square sits exactly
    // where the world point at its centre is drawn. They are listed north row first (the order they appear on screen).
    private void EnsureCells(ForestEcologyController eco, StandMapModel model)
    {
        int n = eco.CellsPerAxis;
        if (cells != null && cells.Length == n * n) return;
        grid.Clear();
        cells = new Button[n * n];
        StandMapTransform transform = model.TransformFor(MapSidePx);
        float size = transform.CellSidePx(n);
        for (int z = n - 1; z >= 0; z--)
            for (int x = 0; x < n; x++)
            {
                int index = z * n + x;
                var cell = new Button(() => { selectedCell = index; ui.Learning.Record("map.select"); if (rackMode && !ui.Manager.RackPlanningV2Active) PlaceRack(ui.Ecology, index); Refresh(true); }) { text = "", focusable = false };
                cell.AddToClassList("map-cell");
                MapPosition corner = transform.CellTopLeft(index, n);
                cell.style.position = Position.Absolute;
                cell.style.left = corner.X;
                cell.style.top = corner.Y;
                cell.style.width = size;
                cell.style.height = size;
                cells[index] = cell;
                grid.Add(cell);
            }
    }

    // Which square is selected, which one you stand in, and (when planning a rack) which ones the rack crosses.
    private void UpdateCells(int playerCell)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].EnableInClassList("map-cell-selected", i == selectedCell);
            cells[i].EnableInClassList("map-cell-player", i == playerCell);
            cells[i].EnableInClassList("map-cell-rack", rackMode && rackCells.Contains(i));
        }
    }

    // The Light and Regeneration overlays: one colour per square, laid over the plan as a smooth translucent field.
    private Color32[] fieldColours;
    private void UpdateField(ForestEcologyController eco)
    {
        if (layer != Layer.Light && layer != Layer.Regeneration) { surface.Field.Clear(); return; }
        int count = eco.CellCount;
        if (fieldColours == null || fieldColours.Length != count) fieldColours = new Color32[count];
        int[] planted = null;
        if (layer == Layer.Regeneration)
        {
            planted = new int[count];
            foreach (PlantedJuvenile j in ui.Manager.PlantedJuveniles)
            {
                if (!j.alive || !string.IsNullOrEmpty(j.promotedTreeId)) continue;
                int c = eco.GetCellIndex(j.position);
                if (c >= 0) planted[c]++;
            }
        }
        for (int i = 0; i < count; i++)
        {
            ForestEcologyCell c = eco.Cells[i];
            MapColour colour;
            if (layer == Layer.Light) colour = StandMapOverlayColours.Light(c.Light);
            else
            {
                float tallest = 0f;
                int cohorts = 0;
                foreach (ForestRegenerationCohort r in c.Regeneration)
                    if (r != null && r.Species != null && r.Density > 0f) { cohorts++; tallest = Mathf.Max(tallest, r.Height); }
                colour = StandMapOverlayColours.Regeneration(tallest, cohorts > 0, planted[i] > 0);
            }
            fieldColours[i] = new Color32(colour.R, colour.G, colour.B, colour.A);
        }
        surface.Field.Show(fieldColours, eco.CellsPerAxis);
    }

    // The survey furniture's words: easting and northing numbers along the edges, "N" by the arrow and the length beside the scale bar.
    // They depend only on the property, so they are rebuilt only when its size changes.
    private void UpdateSurveyLabels(StandMapModel model)
    {
        if (Mathf.Approximately(labelledSize, model.SizeM)) return;
        labelledSize = model.SizeM;
        eastLabels.Clear();
        northLabels.Clear();
        foreach (StandMapGridPlan.GridLabel label in StandMapGridPlan.Labels(model))
        {
            Label text = UiKit.Text(label.Text, "map-axis-label");
            text.pickingMode = PickingMode.Ignore;
            if (label.Easting)
            {
                text.AddToClassList("map-axis-east");
                text.style.left = label.Fraction * MapSidePx - 20f;
                eastLabels.Add(text);
            }
            else
            {
                text.AddToClassList("map-axis-north");
                text.style.top = (1f - label.Fraction) * MapSidePx - 9f;
                northLabels.Add(text);
            }
        }
        StandMapGridPlan.Furniture f = StandMapGridPlan.Layout(model.SizeM, MapSidePx, MapDetail.Full);
        northMark.style.left = f.ArrowX - 6f;
        northMark.style.top = f.ArrowY + f.ArrowSize * 0.7f + 1f;
        scaleMark.text = StandMapGridPlan.ScaleBarText(f.BarMetres);
        scaleMark.style.left = f.BarX1 + 8f;
        scaleMark.style.top = f.BarY - 13f;
    }

    // The overview card's figures, from the trees on the map. Rebuilt only when the trees change.
    private void UpdateOverview(StandMapModel model, ForestEcologyController eco)
    {
        if (factsVersion == model.CanopyVersion && overviewFacts.childCount > 0) return;
        factsVersion = model.CanopyVersion;
        overviewFacts.Clear();
        StandMapStats stats = model.ComputeStats();
        void Fact(string text)
        {
            overviewFacts.Add(UiKit.Text(text, "map-fact"));
        }
        Fact($"{model.SizeM.ToString("0", UiKit.Inv)} × {model.SizeM.ToString("0", UiKit.Inv)} m · {model.AreaHectares.ToString("0.00", UiKit.Inv)} ha");
        Fact(stats.LivingTrees.ToString("N0", UiKit.Inv) + " living trees");
        Fact(stats.StemsPerHectare.ToString("N0", UiKit.Inv) + " stems per ha");
        Fact("Mean DBH " + stats.MeanDbhCm.ToString("0.0", UiKit.Inv) + " cm");
        Fact("Basal area " + stats.BasalAreaM2PerHectare.ToString("0.0", UiKit.Inv) + " m²/ha");
        Fact(model.GroundIsLevel ? "Level ground · no relief data" : "Terrain present, not drawn");
    }

    // What the pointer is over: its local coordinates, the 5 m square and that square's own figures. This is where the numbers behind
    // the colours are read. framePx is a position inside the map frame in UI units, from its top-left corner.
    public string DescribeAt(Vector2 framePx)
    {
        ForestEcologyController eco = ui.Ecology;
        if (eco == null || eco.Cells == null) return DefaultCaption;
        if (framePx.x < 0f || framePx.y < 0f || framePx.x > MapSidePx || framePx.y > MapSidePx) return DefaultCaption;
        StandMapModel model = ui.MapSource.Model;
        RackPoint world = model.TransformFor(MapSidePx).PixelsToWorld(new MapPosition(framePx.x, framePx.y));
        int index = eco.GetCellIndex(new Vector3(world.X, 0f, world.Z));
        string text = $"E {UiKit.F(model.LocalEasting(world.X), "0.0")} m · N {UiKit.F(model.LocalNorthing(world.Z), "0.0")} m";
        if (index < 0) return text;
        ForestEcologyCell c = eco.Cells[index];
        text += $" · square {UiKit.CellLabel(index, eco.CellsPerAxis)} · ground light {UiKit.F(c.Light, "0.00")} · canopy {UiKit.F(c.Canopy, "0.00")}";
        if (layer == Layer.Regeneration)
        {
            float tallest = 0f;
            foreach (ForestRegenerationCohort r in c.Regeneration)
                if (r != null && r.Species != null && r.Density > 0f) tallest = Mathf.Max(tallest, r.Height);
            text += tallest > 0f ? $" · young trees up to {UiKit.F(tallest, "0.0")} m" : " · no young trees";
        }
        return text;
    }

    private void OnPointerMove(PointerMoveEvent e) => caption.text = DescribeAt(e.localPosition);

    // Marked trees per square, counted from the trees on the shared map (so the scene is not searched again).
    private static Dictionary<int, (int fell, int crop)> CountMarks(ForestEcologyController eco, StandMapModel model)
    {
        var counts = new Dictionary<int, (int fell, int crop)>();
        foreach (MapTree tree in model.Trees)
        {
            if (tree.State != MapTreeState.Living || tree.Mark == MapTreeMark.None) continue;
            int i = eco.GetCellIndex(new Vector3(tree.X, 0f, tree.Z));
            if (i < 0) continue;
            counts.TryGetValue(i, out var c);
            counts[i] = tree.Mark == MapTreeMark.Fell ? (c.fell + 1, c.crop) : (c.fell, c.crop + 1);
        }
        return counts;
    }

    private void ToggleRackMode()
    {
        rackMode = !rackMode;
        rackHasPoint = false;
        rackProposal = null;
        rackCells.Clear();
        if (rackMode && !ui.Manager.RackPlanningV2Active)
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
        // A square the planned layout crosses puts the rack on that layout row (rows are about 2 m apart and a square is 5 m, so the
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
            UiKit.Add(card, $"Planned layout: {layout.Done} of {layout.Total} racks in the Work Plan. A dashed line on the map is a rack still to do.", "muted");
        RackProposal p = rackProposal;
        if (p == null)
            UiKit.Add(card, "Click the map on the planted row you want to open.", "body");
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

    // What the selected layer shows, in one or two sentences, with the colour named in words.
    private string LayerNote(ForestEcologyController eco, StandMapModel model)
    {
        switch (layer)
        {
            case Layer.Light: return "Ground light reaching each 5 m square, 0 (dark) to 1 (open). Blue is shade, warm is open ground. Point at the map for a square's figure.";
            case Layer.Regeneration: return "Young trees by 5 m square, shaded by the tallest cohort: pale for a few seedlings, strong green from 3 m. Green dots are planted juveniles.";
            case Layer.Browse:
            {
                float pressure = eco.Browsing.BackgroundPressure;
                return $"Site browsing pressure: {BrowsingConditions.PressureBand(pressure)}. A ring is a shelter: blue while it protects next year, grey once it has expired.";
            }
            case Layer.Marks:
            {
                int fell = 0, crop = 0;
                foreach (MapTree t in model.Trees) { if (t.Mark == MapTreeMark.Fell) fell++; else if (t.Mark == MapTreeMark.Crop) crop++; }
                return $"Rings mark the trees you have marked, and the forest is dimmed so they stand out: red to fell ({fell}), blue Crop Tree ({crop}).";
            }
            default: return "Your position, the planned racks and the forest path. Select a place, then walk there.";
        }
    }

    private void BuildSide(ForestEcologyController eco, StandMapModel model)
    {
        side.Clear();
        // Planning a rack: the rack card is the whole side panel (the square diagnosis is back when the mode is off).
        if (rackMode)
        {
            if (ui.Manager.RackPlanningV2Active)
            {
                UiKit.Add(side, "Planning Racks — Active", "heading");
                UiKit.Add(side, "Dashed lines are suggested routes. Solid lines are your planned racks.", "body");
            }
            else BuildRackCard(eco);
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
            UiKit.Add(side, "Access and position", "heading");
            UiKit.Add(side, LayerNote(eco, model), "map-note");
            UiKit.Add(side, ui.Learning.Summary, "body");
            if (ui.RackLayout != null && ui.RackLayout.Total > 0)
                UiKit.Add(side, $"Access racks: {ui.RackLayout.Done} of {ui.RackLayout.Total} planned or open.", "muted");
            UiKit.Add(side, "Selected destination: " + UiKit.CellLabel(selectedCell, eco.CellsPerAxis), "body");
            var details = new Foldout { text = "Site details · light and young trees", value = false, name = "map-site-details" };
            side.Add(details);
            diagnosis = details;
        }
        else
            UiKit.Add(side, LayerNote(eco, model), "map-note");
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
        if (layer == Layer.Marks)
        {
            Dictionary<int, (int fell, int crop)> marks = CountMarks(eco, model);
            marks.TryGetValue(selectedCell, out var m);
            UiKit.Line(diagnosis, "Marked to fell here", m.fell.ToString(UiKit.Inv));
            UiKit.Line(diagnosis, "Crop Trees here", m.crop.ToString(UiKit.Inv));
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
        // The squares and the survey labels belong to the old geometry.
        cells = null;
        labelledSize = -1f;
        grid.Clear();
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
