using UnityEngine;
using UnityEngine.UIElements;

// The walking HUD's minimap: a reduced presentation of the SAME map as the Stand Map, not a second map. It owns no geometry, no
// coordinate system, no rack or path data and no symbols. Every frame it hands the shared model (StandMapSource) to a StandMapSurface set
// to MapDetail.Mini, so a rack confirmed on the Stand Map (or in the forest), a tree felled or a step you take appears here from the same
// data. The surface redraws only the part that changed: walking repaints you and the destination, a felling redraws the forest picture.
//
// In a Rack Planning V2 world it draws the same two authoritative sources the Stand Map reads: the foundation's suggested routes as
// dashed guidance until the plan is confirmed, then the confirmed rack objects as solid lines (executed racks pale). A short note
// says what the two line styles mean the first time they appear. All of it is read-only; the minimap writes nothing.
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

    // The dashed/solid teaching note (RACK-V2-07). Session memory only: the schema is not touched by this task, so a new session shows
    // it again. It appears while the suggestions are the thing on the map, for NoteSeconds, and never after the plan is confirmed.
    public const string TeachingNote = "Dashed lines show suggested rack positions. Solid lines show your confirmed plan.";
    public const float NoteSeconds = 30f;
    private const float NoteAllowancePx = 56f;
    private readonly Label note;
    private float noteShownAt = -1f;
    private bool noteDone;
    public bool TeachingNoteVisible => note.style.display != DisplayStyle.None;
    public string TeachingNoteText => note.text;

    // Space the minimap takes at the bottom right, for panels that stack above it (and a gap).
    public float ReservedHeightPx => Visible ? SidePx + FrameAllowancePx + 12f + (TeachingNoteVisible ? NoteAllowancePx : 0f) : 0f;

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
        // Under the map, inside the panel so it travels with it. It takes no pointer and holds no text until it is shown.
        note = UiKit.Text("", "hud-minimap-note");
        note.pickingMode = PickingMode.Ignore;
        note.style.display = DisplayStyle.None;
        note.style.width = SidePx;
        note.style.whiteSpace = WhiteSpace.Normal;
        note.style.fontSize = 13;
        note.style.marginTop = 4;
        Root.Add(note);
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
        UpdateTeachingNote(show);
    }

    // Shown only while a V2 plan is still open, so the dashed lines it explains are on the map; it ends after NoteSeconds or the moment
    // the plan is confirmed, whichever is first, and then stays away for the session.
    private void UpdateTeachingNote(bool minimapShown)
    {
        ScenarioOneManager manager = ui.Manager;
        bool wanted = minimapShown && !noteDone && manager != null && manager.RackPlanningV2Active;
        if (wanted && manager.RackPlanConfirmed) { noteDone = true; wanted = false; }
        if (wanted)
        {
            if (noteShownAt < 0f) noteShownAt = Time.unscaledTime;
            if (Time.unscaledTime - noteShownAt > NoteSeconds) { noteDone = true; wanted = false; }
        }
        note.text = wanted ? TeachingNote : "";
        note.style.display = wanted ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
