using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 6 — stand map. Diagnostic and navigation only: it shows recorded
// cell state and lets the player set a waypoint. It never moves the player and
// never recommends where to fell or plant.
public sealed class StandMapView
{
    private enum Layer { Light, Regeneration, Browse, Marks }

    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    private readonly VisualElement grid, side, layerButtons;
    private readonly Label legend;
    private Layer layer = Layer.Light;
    private int selectedCell = -1;
    private int waypointCell = -1;
    private GameObject waypointMarker;
    private string shownKey = "";
    private bool journeyStartedAway;
    public bool HasWaypoint => waypointCell >= 0;

    public StandMapView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer", "modal-backdrop");
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

        layerButtons = UiKit.Box("row");
        modal.Add(layerButtons);

        VisualElement body = UiKit.Box("map-body");
        VisualElement left = UiKit.Box("map-left");
        grid = UiKit.Box("map-grid");
        left.Add(grid);
        legend = UiKit.Add(left, "", "muted");
        UiKit.Add(left, "North is up. Columns A→ run west to east; rows 1→ run south to north. White border: you. Outlined: selected.", "faint");
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
        string key = $"{eco.EcologicalYear}|{layer}|{selectedCell}|{playerCell}|{waypointCell}|"
            + (ui.Marking != null ? ui.Marking.LivingMarkedCount + "/" + ui.Marking.LivingCropTreeCount : "");
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
        AddLayerButton("Light", Layer.Light);
        AddLayerButton("Regeneration", Layer.Regeneration);
        AddLayerButton("Browsing / protection", Layer.Browse);
        AddLayerButton("Fell & crop marks", Layer.Marks);
    }

    private void AddLayerButton(string text, Layer value)
    {
        layerButtons.Add(UiKit.Button(text, () =>
        {
            layer = value;
            ui.Learning.Record(value == Layer.Light ? "map.light" : value == Layer.Regeneration ? "map.regeneration"
                : value == Layer.Browse ? "map.browse" : "map.marks");
            Refresh(true);
        }, true, layer == value ? "btn-selected" : "btn"));
    }

    private void BuildGrid(ForestEcologyController eco, int playerCell)
    {
        grid.Clear();
        int n = eco.CellsPerAxis;
        Dictionary<int, (int fell, int crop)> marks = CountMarks(eco);
        float pressure = eco.Browsing.BackgroundPressure;
        int upcoming = eco.EcologicalYear + 1;
        float size = Mathf.Clamp(640f / Mathf.Max(1, n), 30f, 80f);

        for (int z = n - 1; z >= 0; z--)
        {
            VisualElement row = UiKit.Box("map-row");
            for (int x = 0; x < n; x++)
            {
                int index = z * n + x;
                ForestEcologyCell c = eco.Cells[index];
                var cell = new Button(() => { selectedCell = index; ui.Learning.Record("map.select"); Refresh(true); }) { text = "", focusable = false };
                cell.AddToClassList("map-cell");
                cell.style.width = size;
                cell.style.height = size * 0.875f;
                if (index == selectedCell) cell.AddToClassList("map-cell-selected");
                if (index == playerCell) cell.AddToClassList("map-cell-player");

                string value;
                Color colour;
                switch (layer)
                {
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
                cell.style.backgroundColor = colour;
                if (index == waypointCell) value = "⚑ " + value;
                VisualElement labels = new VisualElement { pickingMode = PickingMode.Ignore };
                Label name = UiKit.Text(UiKit.CellLabel(index, n));
                Label number = UiKit.Text(value);
                name.pickingMode = number.pickingMode = PickingMode.Ignore;
                labels.Add(name);
                labels.Add(number);
                cell.Add(labels);
                row.Add(cell);
            }
            grid.Add(row);
        }

        switch (layer)
        {
            case Layer.Regeneration: legend.text = "Value: P = planted juveniles alive, then tallest seedling cohort height. Brighter green = taller/more."; break;
            case Layer.Browse: legend.text = $"Site browsing pressure: {BrowsingConditions.PressureBand(pressure)}. S = shelters effective next year; — = no shelter."; break;
            case Layer.Marks: legend.text = "■ = trees marked to fell, ◆ = crop trees (counts per cell)."; break;
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

    private void BuildSide(ForestEcologyController eco)
    {
        side.Clear();
        if (selectedCell < 0 || selectedCell >= eco.Cells.Length)
        {
            UiKit.Add(side, "Select a cell to see its diagnosis.", "body");
            return;
        }
        ForestEcologyCell c = eco.Cells[selectedCell];
        UiKit.Add(side, "Cell " + UiKit.CellLabel(selectedCell, eco.CellsPerAxis), "heading");
        UiKit.Line(side, "Ground light", $"{UiKit.F(c.Light, "0.00")} ({RegenerationDiagnostics.LightBand(c.Light)})");
        UiKit.Line(side, "Canopy cover", UiKit.F(c.Canopy, "0.00"));
        UiKit.Add(side, "Regeneration: " + ScenarioOneUiFacts.RegenerationSummary(eco, selectedCell, ui.Manager.PlantedJuveniles), "body");

        float pressure = eco.Browsing.BackgroundPressure;
        var diagnoses = RegenerationDiagnostics.DiagnoseCell(eco, selectedCell);
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        foreach (PlantedJuvenile j in ui.Manager.PlantedJuveniles.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId) && eco.GetCellIndex(j.position) == selectedCell).Take(4))
            diagnoses.Add(RegenerationDiagnostics.Diagnose(eco, j.position, ui.Manager.PlantedJuveniles, spawner));
        if (diagnoses.Count == 0)
        {
            RegenerationDiagnosis empty = default;
            empty.Light = c.Light;
            UiKit.Add(side, "Why: " + ScenarioOneUiFacts.Why(empty, eco, selectedCell), "muted");
        }
        foreach (RegenerationDiagnosis d in diagnoses.Take(6))
        {
            UiKit.Add(side, $"{d.SpeciesName}{(d.IsPlantedIndividual ? " " + d.JuvenileId : "")}, {UiKit.F(d.Height, "0.00")} m — {ScenarioOneUiFacts.BrowseState(d, pressure)}", "body");
            UiKit.Add(side, "Why: " + ScenarioOneUiFacts.Why(d), "muted");
        }

        int index = selectedCell;
        side.Add(UiKit.Button(waypointCell == index ? "Clear waypoint" : "Set waypoint", () =>
        {
            if (waypointCell == index) ClearWaypoint(); else SetWaypoint(eco, index);
            Refresh(true);
        }, true, "btn-primary"));
        UiKit.Add(side, "A waypoint marks the cell in the forest and in the status panel. Walk there yourself; the map does not move you.", "faint");
    }

    private void SetWaypoint(ForestEcologyController eco, int index)
    {
        waypointCell = index;
        journeyStartedAway = ui.Player != null && eco.GetCellIndex(ui.Player.transform.position) != index;
        ui.Learning.Record("map.waypoint");
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
