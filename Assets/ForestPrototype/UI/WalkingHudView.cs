using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 1 — walking HUD. Compact status (top right), contextual ground report
// and marking summary (bottom left), one action prompt under the reticle.
public sealed class WalkingHudView
{
    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }
    // Top-right column: the active-objective widget above the status panel. Panels that hang under the status panel start below it.
    public VisualElement RightColumn { get; }
    private readonly VisualElement statusPanel;
    public ActiveObjectiveHudWidget ObjectiveWidget { get; }
    private float nextObjectiveRefresh;
    // The key hint for planting appears when the Work Plan offers planting (the G key itself always works).
    private bool plantingOffered;

    private readonly Label statusTitle, cash, browse, access, waypoint;
    private readonly VisualElement waypointPanel;
    private readonly Label waypointDirection;
    private readonly WaypointArrow waypointArrow;
    private readonly VisualElement groundPanel;
    private readonly Label groundTitle, groundLight, groundBrowse, groundRegen, groundWhy;
    private readonly Label markSummary, treatment, cropForecast;
    private readonly VisualElement forecastPanel;
    private readonly VisualElement promptBox;
    private readonly Label prompt;
    private readonly VisualElement messageBox;
    private readonly Label message;
    private readonly VisualElement hotbarBox;
    private readonly Label hotbar;
    private readonly VisualElement reticle;
    // The extraction-rack preview card (top-left, where the inspection card would be: the rack tool and inspection never overlap).
    private readonly VisualElement rackPanel;
    private readonly Label rackHeading, rackTrees, rackTimber, rackMoney, rackNote, rackKeys;
    // Bottom right: the Stand Map's map, reduced (same model, transform and symbols; see HudMinimapView).
    private readonly HudMinimapView minimap;
    public HudMinimapView Minimap => minimap;

    public WalkingHudView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer");
        Root.pickingMode = PickingMode.Ignore;

        RightColumn = UiKit.Box("hud-right");
        RightColumn.name = "hud-right-column";
        ObjectiveWidget = new ActiveObjectiveHudWidget(() => ui.ShowObjectives());
        RightColumn.Add(ObjectiveWidget.Root);
        VisualElement status = statusPanel = UiKit.Box("panel", "hud-status");
        statusTitle = UiKit.Add(status, "", "heading");
        VisualElement cashRow = UiKit.Row(status, "spread");
        cashRow.Add(UiKit.Text("Cash", "body"));
        cash = UiKit.Add(cashRow, "", "money");
        browse = UiKit.Add(status, "", "body");
        access = UiKit.Add(status, "", "muted");
        VisualElement menuButtons = UiKit.Row(status, "row-wrap");
        Button mapButton = UiKit.Button("Map [M]", () => ui.ShowMap());
        Button objectivesButton = UiKit.Button("Objectives [O]", () => ui.ShowObjectives());
        Button helpButton = UiKit.Button("Help [F1]", () => ui.ShowCurrentHelp());
        menuButtons.Add(mapButton);
        menuButtons.Add(objectivesButton);
        menuButtons.Add(helpButton);
        RightColumn.Add(status);
        Root.Add(RightColumn);

        waypointPanel = UiKit.Box("panel", "hud-waypoint", "row");
        waypointPanel.name = "hud-waypoint-panel";
        waypointArrow = new WaypointArrow { name = "hud-waypoint-arrow" };
        waypointPanel.Add(waypointArrow);
        VisualElement destination = new VisualElement();
        destination.style.flexGrow = 1;
        waypoint = UiKit.Add(destination, "", "body", "waypoint-line");
        waypoint.name = "hud-waypoint-label";
        waypointDirection = UiKit.Add(destination, "", "muted");
        waypointDirection.name = "hud-waypoint-direction";
        waypointPanel.Add(destination);
        Root.Add(waypointPanel);

        VisualElement bottom = UiKit.Box("hud-bottom");
        groundPanel = UiKit.Box("panel", "hud-ground");
        groundTitle = UiKit.Add(groundPanel, "", "heading");
        groundLight = UiKit.Add(groundPanel, "", "body");
        groundBrowse = UiKit.Add(groundPanel, "", "body");
        groundRegen = UiKit.Add(groundPanel, "", "body");
        groundWhy = UiKit.Add(groundPanel, "", "muted");
        bottom.Add(groundPanel);
        // The thinning forecast sits on its own panel so it stays readable over the forest.
        forecastPanel = UiKit.Box("panel", "hud-forecast");
        forecastPanel.style.alignSelf = Align.FlexStart;
        forecastPanel.style.marginBottom = 6;
        cropForecast = UiKit.Add(forecastPanel, "", "body");
        cropForecast.name = "hud-crop-forecast";
        treatment = UiKit.Add(forecastPanel, "", "body");
        bottom.Add(forecastPanel);
        markSummary = UiKit.Add(bottom, "", "panel", "body");
        markSummary.style.alignSelf = Align.FlexStart;
        Root.Add(bottom);

        promptBox = UiKit.Box("hud-prompt");
        prompt = UiKit.Text("", "panel", "hud-prompt-box");
        promptBox.Add(prompt);
        Root.Add(promptBox);

        messageBox = UiKit.Box("hud-message");
        message = UiKit.Text("", "panel");
        messageBox.Add(message);
        Root.Add(messageBox);

        hotbarBox = UiKit.Box("hud-hotbar");
        hotbar = UiKit.Text("", "panel", "hud-prompt-box");
        hotbarBox.Add(hotbar);
        // Keep planting controls beneath the footprint prompt, above the
        // bottom-left ground report at every supported screen scale.
        promptBox.Add(hotbarBox);

        reticle = UiKit.Box("reticle");
        Root.Add(reticle);

        rackPanel = UiKit.Box("panel", "rack-panel");
        rackPanel.name = "hud-rack-panel";
        rackPanel.style.position = Position.Absolute;
        rackPanel.style.left = 14;
        rackPanel.style.top = 14;
        rackPanel.style.width = TreeInspectionView.ColumnWidth;
        rackPanel.style.display = DisplayStyle.None;
        rackHeading = UiKit.Add(rackPanel, "Extraction rack", "rack-heading");
        rackTrees = UiKit.Add(rackPanel, "", "rack-line");
        rackTimber = UiKit.Add(rackPanel, "", "rack-line");
        rackMoney = UiKit.Add(rackPanel, "", "rack-line");
        rackNote = UiKit.Add(rackPanel, "", "rack-keys");
        rackKeys = UiKit.Add(rackPanel, "", "rack-keys");
        Root.Add(rackPanel);

        minimap = new HudMinimapView(ui);
        Root.Add(minimap.Root);

        foreach (VisualElement e in Root.Query<VisualElement>().ToList()) e.pickingMode = PickingMode.Ignore;
        helpButton.pickingMode = PickingMode.Position;
        mapButton.pickingMode = PickingMode.Position;
        objectivesButton.pickingMode = PickingMode.Position;
    }

    // The action prompt under the reticle is centred and can be wide; while a tree is inspected the side columns are open, so the
    // UI root narrows it to the space between them (it wraps instead of running under a column).
    public void SetPromptMaxWidth(float width) => prompt.style.maxWidth = width;

    private int hudEventCount = -1, hudInterventions;

    // The status panel (year, cash, browsing, key hints) steps aside while the competitor list is open: that list is the tallest thing in the right
    // column and needs the room, the objective widget above stays, and the bottom bar repeats the key hints. Presentation only.
    public void ShowStatus(bool show) => statusPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;

    // The minimap steps aside for the same reason, while the competitor list needs the right column's full height (the map is for
    // orientation while walking, not while studying one tree). It comes back as soon as the list closes. Presentation only.
    public void ShowMinimap(bool show) => minimap.SetSuppressed(!show);

    public void Refresh()
    {
        ScenarioOneManager m = ui.Manager;
        ForestEcologyController eco = ui.Ecology;
        ForestPlayer player = ui.Player;
        ForestTreeMarkingManager marks = ui.Marking;

        minimap.Refresh();
        string phase = m.Outcome == ScenarioOneOutcome.Completed ? "objectives met"
            : m.Outcome == ScenarioOneOutcome.Failed ? "failed" : "planning";
        // The intervention count is derived from the recorded events; recompute it only when they change.
        if (m.ManagementEvents.Count != hudEventCount)
        {
            hudEventCount = m.ManagementEvents.Count;
            hudInterventions = m.Interventions.Count;
        }
        statusTitle.text = $"Scenario One · Year {m.CurrentEcologicalYear} · {phase}"
            + (hudInterventions > 0 ? $" · {hudInterventions} intervention{(hudInterventions == 1 ? "" : "s")}" : "");
        cash.text = UiKit.Money(m.CashCents);
        // The objective only changes when the teaching facts do (they are read about four times a second), so the panel is not rebuilt every frame.
        if (Time.unscaledTime >= nextObjectiveRefresh)
        {
            nextObjectiveRefresh = Time.unscaledTime + 0.25f;
            ObjectiveWidget.Apply(ui.Learning.HudModel());
            plantingOffered = WorkPlanView.PlantingOffered(ui);
        }
        int effective = 0, expired = 0;
        if (eco != null)
            foreach (BrowseShelter shelter in eco.Browsing.Shelters)
            {
                if (shelter == null) continue;
                if (shelter.IsEffective(eco.EcologicalYear + 1)) effective++; else expired++;
            }
        string band = eco != null ? BrowsingConditions.PressureBand(eco.Browsing.BackgroundPressure) : "none";
        browse.text = $"Deer browsing: {band} · shelters {effective} effective" + (expired > 0 ? $", {expired} expired/failed" : "");
        int open = m.WorkOrders.Count(o => o.IsOpen);
        access.text = $"[M] Stand map   ·   [Tab] Work Plan ({open} task{(open == 1 ? "" : "s")})";
        waypoint.text = ui.Map.WaypointDescription(player != null ? player.transform.position : Vector3.zero);
        Vector2 target = default;
        bool hasWaypoint = player != null && ui.Map.TryGetWaypointTarget(out target);
        waypointPanel.style.display = hasWaypoint ? DisplayStyle.Flex : DisplayStyle.None;
        if (hasWaypoint)
        {
            // Screen arrows point up when the destination is ahead, right when
            // it is to the player's right. Pitch does not change ground bearing.
            Vector2 delta = target - new Vector2(player.transform.position.x, player.transform.position.z);
            Vector2 forward = new Vector2(player.transform.forward.x, player.transform.forward.z);
            float bearing = -Vector2.SignedAngle(forward, delta);
            bool arrived = eco != null && delta.magnitude < eco.CellSizeMeters * 0.5f;
            waypointArrow.style.display = arrived ? DisplayStyle.None : DisplayStyle.Flex;
            waypointArrow.SetBearing(bearing);
            waypointDirection.text = arrived ? "At destination · inspect the site"
                : (Mathf.Abs(bearing) <= 22.5f ? "Ahead" : Mathf.Abs(bearing) >= 157.5f ? "Behind you"
                    : bearing < 0f ? "Turn left" : "Turn right") + " · [M] Change destination";
        }
        // Transient messages get their own row below the navigation panel.
        messageBox.style.top = hasWaypoint ? 100f : 14f;

        bool locked = UnityEngine.Cursor.lockState == CursorLockMode.Locked;
        reticle.style.display = locked ? DisplayStyle.Flex : DisplayStyle.None;

        // Ground report while aiming at the ground (not a tree).
        bool ground = marks != null && marks.AimingAtGround && eco != null && eco.GetCellIndex(marks.AimedGroundPoint) >= 0;
        groundPanel.style.display = ground ? DisplayStyle.Flex : DisplayStyle.None;
        if (ground)
        {
            Vector3 point = marks.AimedGroundPoint;
            int cell = eco.GetCellIndex(point);
            RegenerationDiagnosis d = RegenerationDiagnostics.Diagnose(eco, point, m.PlantedJuveniles, Object.FindFirstObjectByType<ForestTreeSpawner>());
            float light = eco.Cells[cell].Light;
            groundTitle.text = $"Ground · square {UiKit.CellLabel(cell, eco.CellsPerAxis)}";
            groundLight.text = $"Light on the ground {UiKit.F(light, "0.00")} ({RegenerationDiagnostics.LightBand(light)})";
            groundBrowse.text = d.HasJuvenile
                ? (d.IsPlantedIndividual ? $"{d.SpeciesName} {d.JuvenileId}, {UiKit.F(d.Height, "0.00")} m: " : $"{d.SpeciesName}, {UiKit.F(d.Height, "0.00")} m: ")
                    + ScenarioOneUiFacts.BrowseState(d, eco.Browsing.BackgroundPressure)
                : $"Browsing: {band}";
            groundRegen.text = "Regeneration: " + ScenarioOneUiFacts.RegenerationSummary(eco, cell, m.PlantedJuveniles);
            groundWhy.text = "Why: " + ScenarioOneUiFacts.Why(d, eco, cell);
        }

        // One prompt: the player's action line, refined by the mark prompt.
        string action = player != null && locked ? player.InteractionPromptText() : "";
        // During the opening rack stage the prompt points at racks; selective marking is introduced once the racks are open.
        bool rackStage = ui.Learning.TeachingRacks;
        RackTool rackTool = ui.RackTool;
        bool racking = rackTool != null && rackTool.Active;
        if (racking)
            action = "";
        else if (marks != null && marks.AimedTree != null && player != null && player.AimedTree != null && player.AimedTree.CanChop)
            action = rackStage ? "[E] Inspect   ·   [B] Create rack   ·   [Tab] Plan"
                : "[E] Inspect   ·   " + marks.MarkPromptText() + "   ·   [Tab] Plan";
        prompt.text = action;
        prompt.style.display = string.IsNullOrEmpty(action) ? DisplayStyle.None : DisplayStyle.Flex;

        string note = rackTool != null && !string.IsNullOrEmpty(rackTool.Notice) ? rackTool.Notice
            : marks != null && !string.IsNullOrEmpty(marks.TransientMessage) ? marks.TransientMessage
            : player != null ? player.TransientMessage : "";
        message.text = note;
        messageBox.style.display = string.IsNullOrEmpty(note) ? DisplayStyle.None : DisplayStyle.Flex;

        string bar = player != null ? player.PlantingHotbarText() : "";
        hotbar.text = bar;
        hotbarBox.style.display = string.IsNullOrEmpty(bar) ? DisplayStyle.None : DisplayStyle.Flex;
        promptBox.style.display = string.IsNullOrEmpty(action) && string.IsNullOrEmpty(bar) ? DisplayStyle.None : DisplayStyle.Flex;

        rackPanel.style.display = racking ? DisplayStyle.Flex : DisplayStyle.None;
        if (racking) FillRackPanel(rackTool);
        if (marks != null)
        {
            markSummary.text = rackStage ? RackStageSummary(marks)
                : $"Marked: FELL {marks.LivingMarkedCount} (red)  ·  CROP TREE {marks.LivingCropTreeCount} (blue)  ·  "
                + $"fell volume {UiKit.F(marks.MarkedVolumeM3, "0.0")} m³      [X] Fell  [C] Crop" + (plantingOffered ? "  [G] Plant" : "");
            treatment.text = marks.TreatmentOutcome ?? "";
            treatment.style.display = string.IsNullOrEmpty(treatment.text) ? DisplayStyle.None : DisplayStyle.Flex;
            cropForecast.text = CropForecast(ui.Competitors != null ? ui.Competitors.Summary : null);
            cropForecast.style.display = string.IsNullOrEmpty(cropForecast.text) ? DisplayStyle.None : DisplayStyle.Flex;
            forecastPanel.style.display = treatment.style.display == DisplayStyle.Flex || cropForecast.style.display == DisplayStyle.Flex
                ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }

    // The opening stage's one-line status: how far through the planned rack layout, and how to continue.
    private string RackStageSummary(ForestTreeMarkingManager marks)
    {
        RackLayoutTracker layout = ui.RackLayout;
        string text = layout != null && layout.Total > 0
            ? $"Extraction racks: {layout.Done} of {layout.Total} in the Work Plan  ·  [B] Create rack  ·  [M] Plan on the Stand Map  ·  [Tab] Work Plan"
            : "[B] Create an extraction rack  ·  [Tab] Work Plan";
        if (marks.LivingMarkedCount > 0 || marks.LivingCropTreeCount > 0)
            text += $"  ·  marks: FELL {marks.LivingMarkedCount}, CROP {marks.LivingCropTreeCount}";
        return text;
    }

    // The rack preview card: what the corridor would take and what it would be worth, in plain figures.
    private void FillRackPanel(RackTool tool)
    {
        RackProposal p = tool.Proposal;
        bool onLayout = p != null && p.LayoutIndex >= 0;
        rackHeading.text = (tool.HasOrigin ? "Rack start fixed · aim forward" : "Choose a rack start") + (onLayout ? $" · planned rack {p.LayoutIndex + 1} of {p.LayoutCount}"
            : p != null && p.LayoutCount > 0 ? " · not on the planned layout" : "");
        rackTrees.style.display = DisplayStyle.Flex;
        rackTimber.style.display = rackMoney.style.display = rackNote.style.display = DisplayStyle.None;
        if (p == null || p.Selection == null)
        {
            rackTrees.text = "Choose your start: aim at a tree or the ground, then click. The start ring shows the point you are choosing.";
        }
        else if (!p.CanConfirm)
        {
            rackTrees.text = string.IsNullOrEmpty(p.Problem) ? "Aim at a planted row." : p.Problem;
        }
        else
        {
            rackTrees.text = $"Can add to Work Plan · {p.TreeCount} tree{(p.TreeCount == 1 ? "" : "s")}";
            rackTimber.text = $"Estimated timber: {UiKit.F(p.SoldVolumeM3, "0.0")} m³ sold ({UiKit.F(p.StemVolumeM3, "0.0")} m³ stem volume)";
            rackMoney.text = $"Estimated net return: {UiKit.SignedMoney(p.NetCents)}  (timber {UiKit.Money(p.RevenueCents)}, harvesting {UiKit.Money(p.CostCents)}; this rack on its own)";
            rackTimber.style.display = rackMoney.style.display = DisplayStyle.Flex;
        }
        string notes = "";
        if (ui.Learning.TeachingRacks && ui.RackLayout != null)
            notes = p != null ? ui.RackLayout.PreviewFeedback(p)
                : ui.RackLayout.HasProgress ? ui.Learning.CurrentInstruction : "";
        if (p != null && p.Selection != null)
        {
            if (p.Selection.SkippedCropTrees > 0) notes += "  " + $"{p.Selection.SkippedCropTrees} Crop Tree{(p.Selection.SkippedCropTrees == 1 ? " is" : "s are")} kept.  ";
            if (p.Selection.AlreadyPlanned > 0 && p.CanConfirm) notes += "  " + $"{p.Selection.AlreadyPlanned} already in the Work Plan.";
        }
        rackNote.text = notes;
        rackNote.style.display = string.IsNullOrEmpty(notes) ? DisplayStyle.None : DisplayStyle.Flex;
        rackKeys.text = tool.HasOrigin ? "[Aim / wheel] Pivot from start   [Click / Enter] Add rack   [Right-click] New start   [Esc / B] Cancel"
            : "[Click / Enter] Fix this start   [Esc / B] Cancel";
    }

    // Release estimate across all Crop Trees for the current Fell marks.
    // Each Fell-marked stem is counted once, however many Crop Trees it is near.
    private static string CropForecast(CropTreeReleaseSummary summary)
    {
        if (summary == null || summary.PlannedFells == 0)
            return "";
        if (summary.CropTrees == 0)
            return "Crop Trees: none chosen yet. Keep trees with [C] to see how your Fell marks affect them.";
        int change = Mathf.RoundToInt(summary.ChangeFraction * 100f);
        string signed = (change > 0 ? "+" : change < 0 ? "−" : "") + Mathf.Abs(change).ToString(UiKit.Inv) + " %";
        return $"Crop Trees {summary.CropTrees}: competition around them {UiKit.F(summary.MeanCompetitionNow, "0.00")} → "
            + $"{UiKit.F(summary.MeanCompetitionAfter, "0.00")} ({signed}) · {summary.CropTreesReleased} lose at least a tenth · "
            + $"Fell marks within 8 m of a Crop Tree: {summary.PlannedNearACropTree} of {summary.PlannedFells}"
            + (summary.PlannedNearSeveralCropTrees > 0 ? $" ({summary.PlannedNearSeveralCropTrees} near more than one)" : "");
    }

    private sealed class WaypointArrow : VisualElement
    {
        private static readonly Vector2[] Outline = {
            new Vector2(18, 3), new Vector2(31, 18), new Vector2(23, 18),
            new Vector2(23, 32), new Vector2(13, 32), new Vector2(13, 18), new Vector2(5, 18)
        };
        public float BearingDegrees { get; private set; }
        public WaypointArrow()
        {
            style.width = 36; style.height = 36; style.flexShrink = 0; style.marginRight = 12;
            generateVisualContent += context =>
            {
                Painter2D painter = context.painter2D;
                painter.fillColor = UiKit.AccentColour;
                Quaternion rotation = Quaternion.Euler(0, 0, BearingDegrees);
                Vector2 centre = new Vector2(18, 18);
                painter.BeginPath();
                for (int i = 0; i < Outline.Length; i++)
                {
                    Vector2 point = (Vector2)(rotation * (Vector3)(Outline[i] - centre)) + centre;
                    if (i == 0) painter.MoveTo(point); else painter.LineTo(point);
                }
                painter.ClosePath(); painter.Fill();
            };
        }
        public void SetBearing(float degrees)
        {
            if (Mathf.Approximately(degrees, BearingDegrees)) return;
            BearingDegrees = degrees;
            MarkDirtyRepaint();
        }
    }
}
