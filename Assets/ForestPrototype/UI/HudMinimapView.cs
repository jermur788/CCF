using UnityEngine;
using UnityEngine.UIElements;

// The walking HUD's minimap: a reduced presentation of the SAME map as the Stand Map, not a second map. It owns no geometry, no
// coordinate system, no rack or path data and no symbols. Every frame it hands the shared model (StandMapSource) to a StandMapSurface set
// to MapDetail.Mini, so a rack confirmed on the Stand Map (or in the forest), a tree felled or a step you take appears here from the same
// data. The surface redraws only the part that changed: walking repaints you and the destination, a felling redraws the forest picture.
//
// It shows the whole property, north up, so a place has the same relative position here as on the Stand Map: the forest mass, the path,
// the racks, you and which way you face. It carries no grid, no figures and no controls (the rack being planned is not drawn on it),
// and takes no pointer input.
public sealed class HudMinimapView
{
    // Side of the map in UI units (the UI scales with the screen; the reference screen is 1600 x 900).
    public const float SidePx = 196f;
    private const float FrameAllowancePx = 10f;

    private readonly ScenarioOneUiRoot ui;
    private readonly StandMapSurface surface;
    public VisualElement Root { get; }
    public StandMapCanvas Canvas => surface.Canvas;
    public StandMapSurface Surface => surface;
    public StandMapCanopyLayer Canopy => surface.Canopy;
    public bool Visible => Root.style.display != DisplayStyle.None;
    private bool suppressed;

    // Space the minimap takes at the bottom right, for panels that stack above it (and a gap).
    public float ReservedHeightPx => Visible ? SidePx + FrameAllowancePx + 12f : 0f;

    public HudMinimapView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        Root = UiKit.Box("panel", "hud-minimap");
        Root.name = "hud-minimap";
        surface = new StandMapSurface(MapDetail.Mini) { name = "hud-minimap-canvas" };
        surface.style.width = SidePx;
        surface.style.height = SidePx;
        Root.Add(surface);
        // "N" beside the north arrow the plan draws (the one word on the minimap).
        Label north = UiKit.Text("N", "hud-minimap-north");
        north.pickingMode = PickingMode.Ignore;
        StandMapGridPlan.Furniture f = StandMapGridPlan.Layout(1f, SidePx, MapDetail.Mini);
        // The surface sits inside the panel's 4-unit padding.
        north.style.left = 4f + f.ArrowX - 6f;
        north.style.top = 4f + f.ArrowY + f.ArrowSize * 0.7f;
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
        if (show) surface.Sync(model);
    }
}
