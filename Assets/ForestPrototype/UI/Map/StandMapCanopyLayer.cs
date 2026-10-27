using UnityEngine;
using UnityEngine.UIElements;

// Shows the forest picture (StandMapCanopyRaster) of the shared model as the ground of a map. The full Stand Map and the HUD minimap each
// have one, at their own pixel size; both draw the SAME trees from the SAME model with the SAME world-to-map transform (the picture
// covers exactly the property square), so a tree is in the same place on both.
//
// It redraws only when the canopy has changed (a tree felled, died, grown, or planted out: StandMapModel.CanopyVersion), never because
// the player walked. A redraw is spread over a few frames (a few dozen trees per frame), so a felling never stalls a frame; the old
// picture stays on screen until the new one is complete. Opening the full map while a redraw is part-way finishes it at once.
public sealed class StandMapCanopyLayer : VisualElement
{
    private readonly MapDetail detail;
    private readonly int sidePx;
    private readonly int treesPerStep;
    private Texture2D texture;
    private byte[] picture;
    private StandMapCanopyRaster.Job job;
    private int jobVersion = -1, shownVersion = -1;

    public MapDetail Detail => detail;
    // The pixel side and the picture last shown (RGBA, rows from the south). Verification samples these to check the trees are where the model says.
    public int SidePx => sidePx;
    // (A redraw under way draws into the same buffer, so read it only when Redrawing is false.)
    public byte[] Picture => shownVersion >= 0 ? picture : null;
    // The canopy version the picture on screen was drawn from (-1 = nothing yet), and whether a newer redraw is under way.
    public int ShownVersion => shownVersion;
    public bool Redrawing => job != null;
    // Counters a verification reads to prove a walk redraws nothing and a felling redraws once.
    public int Redraws { get; private set; }
    public int Steps { get; private set; }
    public double LastRedrawMilliseconds { get; private set; }
    public double WorstStepMilliseconds { get; private set; }
    private double redrawStarted;

    public StandMapCanopyLayer(MapDetail detail)
    {
        this.detail = detail;
        sidePx = detail == MapDetail.Full ? StandMapCanopyRaster.FullSidePx : StandMapCanopyRaster.MiniSidePx;
        treesPerStep = detail == MapDetail.Full ? 48 : 4000;
        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        style.left = style.top = style.right = style.bottom = 0;
        style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        style.backgroundColor = new Color(StandMapSymbols.Ground.R / 255f, StandMapSymbols.Ground.G / 255f, StandMapSymbols.Ground.B / 255f, 1f);
        RegisterCallback<DetachFromPanelEvent>(_ => Release());
    }

    // Call once a frame while the map is on screen. Starts a redraw when the canopy changed, and advances one under way.
    public void Sync(StandMapModel model)
    {
        if (model == null || !model.HasProperty) return;
        if (model.CanopyVersion != jobVersion)
        {
            if (picture == null) picture = new byte[sidePx * sidePx * 4];
            // A newer canopy replaces a redraw that is still running. The picture on screen is a GPU copy, so drawing into this buffer
            // does not disturb it: it is replaced only when a redraw completes.
            job = new StandMapCanopyRaster.Job(model, sidePx, detail, picture);
            jobVersion = model.CanopyVersion;
            redrawStarted = Time.realtimeSinceStartupAsDouble;
        }
        if (job == null) return;
        double stepStart = Time.realtimeSinceStartupAsDouble;
        bool done = job.Step(treesPerStep);
        Steps++;
        WorstStepMilliseconds = System.Math.Max(WorstStepMilliseconds, (Time.realtimeSinceStartupAsDouble - stepStart) * 1000.0);
        if (done) Present();
    }

    // Finish any redraw now (the map has just been opened and must not show an out-of-date forest).
    public void FinishNow(StandMapModel model)
    {
        Sync(model);
        if (job != null) { job.Finish(); Present(); }
    }

    private void Present()
    {
        if (texture == null)
        {
            texture = new Texture2D(sidePx, sidePx, TextureFormat.RGBA32, false) { name = "Stand map canopy (" + detail + ")", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
            style.backgroundImage = new StyleBackground(Background.FromTexture2D(texture));
        }
        texture.LoadRawTextureData(picture);
        texture.Apply(false);
        shownVersion = jobVersion;
        job = null;
        Redraws++;
        LastRedrawMilliseconds = (Time.realtimeSinceStartupAsDouble - redrawStarted) * 1000.0;
    }

    private void Release()
    {
        if (texture != null) Object.Destroy(texture);
        texture = null;
        style.backgroundImage = StyleKeyword.None;
        job = null;
        jobVersion = shownVersion = -1;
    }
}
