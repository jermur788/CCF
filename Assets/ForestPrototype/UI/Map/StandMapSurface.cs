using UnityEngine;
using UnityEngine.UIElements;

// A translucent field laid over the base map for an analytical layer (ground light, regeneration). The ecology holds one value per
// 5 m square, so the field is a small texture with one pixel per square, stretched over the property square with smooth filtering: the
// colour at the middle of a square is that square's value and the squares themselves are not drawn. It takes no pointer input.
public sealed class StandMapOverlayLayer : VisualElement
{
    private Texture2D texture;
    private int cells;

    public bool HasField => texture != null && resolvedStyle.display != DisplayStyle.None;
    // The one colour per square last shown (index z * cells + x, z = 0 the south row). Verification reads it to check each square is its own colour.
    public Color32[] LastColours { get; private set; }
    public int Cells => cells;

    public StandMapOverlayLayer()
    {
        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        style.left = style.top = style.right = style.bottom = 0;
        style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        style.display = DisplayStyle.None;
        RegisterCallback<DetachFromPanelEvent>(_ => Release());
    }

    // colours[z * cells + x], z = 0 the SOUTH row, exactly the ecology's cell index, so a texture pixel is one ecology square.
    public void Show(Color32[] colours, int cellsPerAxis)
    {
        if (colours == null || cellsPerAxis <= 0 || colours.Length < cellsPerAxis * cellsPerAxis) { Clear(); return; }
        if (texture == null || cells != cellsPerAxis)
        {
            if (texture != null) Object.Destroy(texture);
            cells = cellsPerAxis;
            texture = new Texture2D(cells, cells, TextureFormat.RGBA32, false) { name = "Stand map layer", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
            style.backgroundImage = new StyleBackground(Background.FromTexture2D(texture));
        }
        texture.SetPixels32(colours);
        texture.Apply(false);
        LastColours = (Color32[])colours.Clone();
        style.display = DisplayStyle.Flex;
    }

    public void Clear() => style.display = DisplayStyle.None;

    private void Release()
    {
        if (texture != null) Object.Destroy(texture);
        texture = null;
        style.backgroundImage = StyleKeyword.None;
    }
}

// The map as one element: the canopy picture, the analytical field and dimming (full map only), then the vector shapes on top. The full
// Stand Map and the HUD minimap are both one of these at different sizes and details; everything in it is drawn from the one shared model
// with the one shared transform. The owner sets its size (it must be square). It takes no pointer input.
public sealed class StandMapSurface : VisualElement
{
    private readonly VisualElement scrim;

    public StandMapCanopyLayer Canopy { get; }
    public StandMapOverlayLayer Field { get; }
    public StandMapCanvas Canvas { get; }
    public MapDetail Detail { get; }

    public StandMapSurface(MapDetail detail)
    {
        Detail = detail;
        pickingMode = PickingMode.Ignore;
        Canopy = new StandMapCanopyLayer(detail);
        Add(Canopy);
        if (detail == MapDetail.Full)
        {
            Field = new StandMapOverlayLayer();
            Add(Field);
            // Over the Marks layer the forest is dimmed so the marked trees stand out; this is the dimming.
            scrim = new VisualElement { pickingMode = PickingMode.Ignore };
            scrim.style.position = Position.Absolute;
            scrim.style.left = scrim.style.top = scrim.style.right = scrim.style.bottom = 0;
            scrim.style.backgroundColor = new Color(0.03f, 0.05f, 0.04f, 0.5f);
            scrim.style.display = DisplayStyle.None;
            Add(scrim);
        }
        Canvas = new StandMapCanvas(detail);
        Canvas.style.position = Position.Absolute;
        Canvas.style.left = Canvas.style.top = Canvas.style.right = Canvas.style.bottom = 0;
        Add(Canvas);
    }

    // Once a frame while on screen. The canopy and the vector shapes each redraw only if their own part of the model changed.
    public void Sync(StandMapModel model, MapOverlay overlay = MapOverlay.None)
    {
        Canopy.Sync(model);
        Canvas.Sync(model, overlay);
        if (scrim != null) scrim.style.display = overlay == MapOverlay.Marks ? DisplayStyle.Flex : DisplayStyle.None;
    }
}
