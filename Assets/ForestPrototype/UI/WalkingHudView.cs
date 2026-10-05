using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Screen 1 — walking HUD. Compact status (top right), contextual ground report
// and marking summary (bottom left), one action prompt under the reticle.
public sealed class WalkingHudView
{
    private readonly ScenarioOneUiRoot ui;
    public VisualElement Root { get; }

    private readonly Label statusTitle, cash, objectives, browse, access, waypoint;
    private readonly VisualElement groundPanel;
    private readonly Label groundTitle, groundLight, groundBrowse, groundRegen, groundWhy;
    private readonly Label markSummary, treatment;
    private readonly VisualElement promptBox;
    private readonly Label prompt;
    private readonly VisualElement messageBox;
    private readonly Label message;
    private readonly VisualElement hotbarBox;
    private readonly Label hotbar;
    private readonly VisualElement reticle;

    public WalkingHudView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("layer");
        Root.pickingMode = PickingMode.Ignore;

        VisualElement status = UiKit.Box("panel", "hud-status");
        statusTitle = UiKit.Add(status, "", "heading");
        VisualElement cashRow = UiKit.Row(status, "spread");
        cashRow.Add(UiKit.Text("Cash", "body"));
        cash = UiKit.Add(cashRow, "", "money");
        objectives = UiKit.Add(status, "", "body");
        browse = UiKit.Add(status, "", "body");
        waypoint = UiKit.Add(status, "", "body", "waypoint-line");
        access = UiKit.Add(status, "", "muted");
        Button helpButton = UiKit.Button("Help [F1]", () => ui.ShowCurrentHelp());
        status.Add(helpButton);
        Root.Add(status);

        VisualElement bottom = UiKit.Box("hud-bottom");
        groundPanel = UiKit.Box("panel", "hud-ground");
        groundTitle = UiKit.Add(groundPanel, "", "heading");
        groundLight = UiKit.Add(groundPanel, "", "body");
        groundBrowse = UiKit.Add(groundPanel, "", "body");
        groundRegen = UiKit.Add(groundPanel, "", "body");
        groundWhy = UiKit.Add(groundPanel, "", "muted");
        bottom.Add(groundPanel);
        treatment = UiKit.Add(bottom, "", "muted");
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

        foreach (VisualElement e in Root.Query<VisualElement>().ToList()) e.pickingMode = PickingMode.Ignore;
        helpButton.pickingMode = PickingMode.Position;
    }

    public void Refresh()
    {
        ScenarioOneManager m = ui.Manager;
        ForestEcologyController eco = ui.Ecology;
        ForestPlayer player = ui.Player;
        ForestTreeMarkingManager marks = ui.Marking;

        string phase = m.Outcome == ScenarioOneOutcome.Completed ? "objectives met"
            : m.Outcome == ScenarioOneOutcome.Failed ? "failed" : "planning";
        statusTitle.text = $"Scenario One · Year {m.CurrentEcologicalYear} · {phase}";
        cash.text = UiKit.Money(m.CashCents);
        var objectiveList = m.Objectives;
        objectives.text = $"Objectives {objectiveList.Count(o => o.achieved)} of {objectiveList.Count}";
        int effective = 0, expired = 0;
        if (eco != null)
            foreach (BrowseShelter shelter in eco.Browsing.Shelters)
            {
                if (shelter == null) continue;
                if (shelter.IsEffective(eco.EcologicalYear + 1)) effective++; else expired++;
            }
        string band = eco != null ? BrowsingConditions.PressureBand(eco.Browsing.BackgroundPressure) : "none";
        browse.text = $"Browsing: {band} · shelters {effective} effective" + (expired > 0 ? $", {expired} expired/failed" : "");
        int open = m.WorkOrders.Count(o => o.IsOpen);
        access.text = $"[N] Stand map   ·   [Tab] Work Plan ({open} task{(open == 1 ? "" : "s")})";
        waypoint.text = ui.Map.WaypointDescription(player != null ? player.transform.position : Vector3.zero);
        waypoint.style.display = string.IsNullOrEmpty(waypoint.text) ? DisplayStyle.None : DisplayStyle.Flex;

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
            groundTitle.text = $"Ground · cell {UiKit.CellLabel(cell, eco.CellsPerAxis)}";
            groundLight.text = $"Light {UiKit.F(light, "0.00")} ({RegenerationDiagnostics.LightBand(light)})";
            groundBrowse.text = d.HasJuvenile
                ? (d.IsPlantedIndividual ? $"{d.SpeciesName} {d.JuvenileId}, {UiKit.F(d.Height, "0.00")} m: " : $"{d.SpeciesName}, {UiKit.F(d.Height, "0.00")} m: ")
                    + ScenarioOneUiFacts.BrowseState(d, eco.Browsing.BackgroundPressure)
                : $"Browsing: {band}";
            groundRegen.text = "Regeneration: " + ScenarioOneUiFacts.RegenerationSummary(eco, cell, m.PlantedJuveniles);
            groundWhy.text = "Why: " + ScenarioOneUiFacts.Why(d);
        }

        // One prompt: the player's action line, refined by the mark prompt.
        string action = player != null && locked ? player.InteractionPromptText() : "";
        if (marks != null && marks.AimedTree != null && player != null && player.AimedTree != null && player.AimedTree.CanChop)
            action = "[E] Inspect   ·   " + marks.MarkPromptText() + "   ·   [Tab] Plan";
        prompt.text = action;
        prompt.style.display = string.IsNullOrEmpty(action) ? DisplayStyle.None : DisplayStyle.Flex;

        string note = marks != null && !string.IsNullOrEmpty(marks.TransientMessage) ? marks.TransientMessage
            : player != null ? player.TransientMessage : "";
        message.text = note;
        messageBox.style.display = string.IsNullOrEmpty(note) ? DisplayStyle.None : DisplayStyle.Flex;

        string bar = player != null ? player.PlantingHotbarText() : "";
        hotbar.text = bar;
        hotbarBox.style.display = string.IsNullOrEmpty(bar) ? DisplayStyle.None : DisplayStyle.Flex;
        promptBox.style.display = string.IsNullOrEmpty(action) && string.IsNullOrEmpty(bar) ? DisplayStyle.None : DisplayStyle.Flex;

        if (marks != null)
        {
            markSummary.text = $"Marked: FELL {marks.LivingMarkedCount} (red)  ·  CROP TREE {marks.LivingCropTreeCount} (blue)  ·  "
                + $"fell volume {UiKit.F(marks.MarkedVolumeM3, "0.0")} m³      [M] Fell  [C] Crop  [G] Plant";
            treatment.text = marks.TreatmentOutcome ?? "";
            treatment.style.display = string.IsNullOrEmpty(treatment.text) ? DisplayStyle.None : DisplayStyle.Flex;
        }
    }
}
