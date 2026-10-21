using UnityEngine;
using UnityEngine.UIElements;

// The walking HUD's minimap: a reduced presentation of the SAME map as the Stand Map, not a second map. It owns no geometry, no
// coordinate system, no rack or path data and no symbols. Every frame it hands the shared model (StandMapSource) to a
// StandMapCanvas set to MapDetail.Mini, so a rack confirmed on the Stand Map (or in the forest) appears here from the same data.
//
// It shows the whole property, north up, so a place has the same relative position here as on the Stand Map. It carries no
// forestry values and no controls, and takes no pointer input.
public sealed class HudMinimapView
{
    // Side of the map in UI units (the UI scales with the screen; the reference screen is 1600 x 900).
    public const float SidePx = 196f;
    private const float FrameAllowancePx = 10f;

    private readonly ScenarioOneUiRoot ui;
    private readonly StandMapCanvas canvas;
    public VisualElement Root { get; }
    public StandMapCanvas Canvas => canvas;
    public bool Visible => Root.style.display != DisplayStyle.None;
    private bool suppressed;

    // Space the minimap takes at the bottom right, for panels that stack above it (and a gap).
    public float ReservedHeightPx => Visible ? SidePx + FrameAllowancePx + 12f : 0f;

    public HudMinimapView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("panel", "hud-minimap");
        Root.name = "hud-minimap";
        canvas = new StandMapCanvas(MapDetail.Mini, true) { name = "hud-minimap-canvas" };
        canvas.style.width = SidePx;
        canvas.style.height = SidePx;
        Root.Add(canvas);
        // North is up; the only text on the minimap.
        Label north = UiKit.Text("N", "hud-minimap-north");
        north.pickingMode = PickingMode.Ignore;
        Root.Add(north);
        Root.style.display = DisplayStyle.None;
    }

    // A panel that needs the whole right column asks the minimap to step aside (see WalkingHudView.ShowMinimap).
    public void SetSuppressed(bool value)
    {
        suppressed = value;
        if (value) Root.style.display = DisplayStyle.None;
    }

    public void Refresh()
    {
        StandMapModel model = ui.MapSource != null ? ui.MapSource.Model : null;
        bool show = model != null && model.HasProperty && !suppressed;
        Root.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        if (show) canvas.Sync(model);
    }
}
