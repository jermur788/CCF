using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// One small picture in the legend: a line, corridor, ring, dot, diamond, wedge, crown or colour ramp, painted from the SAME symbol table
// the map is drawn from (colour, width, dash pattern), so the legend cannot drift from the map. A tree crown is the very crown the
// canopy picture draws, rendered once at swatch size.
public sealed class StandMapSwatch : VisualElement
{
    public const float WidthPx = 46f, HeightPx = 18f;
    private readonly LegendEntry entry;

    public StandMapSwatch(LegendEntry entry, Texture2D crown)
    {
        this.entry = entry;
        pickingMode = PickingMode.Ignore;
        style.width = WidthPx;
        style.height = HeightPx;
        style.flexShrink = 0;
        if (crown != null)
        {
            style.backgroundImage = new StyleBackground(Background.FromTexture2D(crown));
            style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        }
        generateVisualContent += Paint;
    }

    private static Color ToColor(MapColour c) => new Color32(c.R, c.G, c.B, c.A);

    private void Paint(MeshGenerationContext context)
    {
        Painter2D p = context.painter2D;
        float w = WidthPx, h = HeightPx, mid = h * 0.5f;
        MapSymbolStyle s = entry.Kind == LegendKind.Crown || entry.Kind == LegendKind.Stump || entry.Kind == LegendKind.Ramp ? null : StandMapSymbols.Get(entry.Symbol);
        p.lineJoin = LineJoin.Round;
        switch (entry.Kind)
        {
            case LegendKind.Band:
                if (s.HasFill) { p.fillColor = ToColor(s.Fill); p.BeginPath(); p.MoveTo(new Vector2(0, 2)); p.LineTo(new Vector2(w, 2)); p.LineTo(new Vector2(w, h - 2)); p.LineTo(new Vector2(0, h - 2)); p.ClosePath(); p.Fill(); }
                Stroke(p, s, mid, w, Mathf.Max(s.MinPx, 1.6f));
                break;
            case LegendKind.Line:
                if (entry.Symbol == MapSymbol.Path)
                {
                    MapSymbolStyle edge = StandMapSymbols.Get(MapSymbol.PathCasing);
                    p.strokeColor = ToColor(edge.Colour); p.lineWidth = 7.4f; p.lineCap = LineCap.Butt; p.BeginPath(); p.MoveTo(new Vector2(0, mid)); p.LineTo(new Vector2(w, mid)); p.Stroke();
                    p.strokeColor = ToColor(s.Colour); p.lineWidth = 4.8f; p.BeginPath(); p.MoveTo(new Vector2(0, mid)); p.LineTo(new Vector2(w, mid)); p.Stroke();
                }
                else Stroke(p, s, mid, w, Mathf.Max(s.MinPx, 1.6f));
                break;
            case LegendKind.Patch:
                p.fillColor = ToColor(s.Fill); p.BeginPath(); p.MoveTo(new Vector2(4, 3)); p.LineTo(new Vector2(w - 4, 3)); p.LineTo(new Vector2(w - 4, h - 3)); p.LineTo(new Vector2(4, h - 3)); p.ClosePath(); p.Fill();
                p.strokeColor = ToColor(s.Colour); p.lineWidth = 1.2f; p.Stroke();
                break;
            case LegendKind.Dot:
                Circle(p, w * 0.5f, mid, 3.6f); p.fillColor = ToColor(s.Colour); p.Fill(); p.strokeColor = new Color(0.07f, 0.11f, 0.055f, 0.86f); p.lineWidth = 1.1f; p.Stroke();
                break;
            case LegendKind.Ring:
                Circle(p, w * 0.5f, mid, 6f); p.strokeColor = ToColor(s.Colour); p.lineWidth = Mathf.Max(1.8f, s.MinPx); p.Stroke();
                break;
            case LegendKind.Diamond:
                p.BeginPath(); p.MoveTo(new Vector2(w * 0.5f, mid - 6)); p.LineTo(new Vector2(w * 0.5f + 6, mid)); p.LineTo(new Vector2(w * 0.5f, mid + 6)); p.LineTo(new Vector2(w * 0.5f - 6, mid)); p.ClosePath();
                p.fillColor = ToColor(s.Colour); p.Fill(); p.strokeColor = new Color(0.125f, 0.086f, 0.024f, 0.92f); p.lineWidth = 1.4f; p.Stroke();
                break;
            case LegendKind.Wedge:
            {
                MapSymbolStyle dot = StandMapSymbols.Get(MapSymbol.PlayerDot);
                float cx = 12f;
                p.BeginPath(); p.MoveTo(new Vector2(cx, mid)); p.LineTo(new Vector2(cx + 26f, mid - 8f)); p.LineTo(new Vector2(cx + 26f, mid + 8f)); p.ClosePath();
                p.fillColor = ToColor(s.Colour); p.Fill();
                Circle(p, cx, mid, 4.6f); p.fillColor = ToColor(dot.Colour); p.Fill(); p.strokeColor = new Color(1f, 1f, 1f, 0.95f); p.lineWidth = 1.5f; p.Stroke();
                break;
            }
            case LegendKind.Stump:
                Circle(p, w * 0.5f, mid, 3.2f); p.fillColor = ToColor(StandMapSymbols.StumpColour); p.Fill();
                break;
            case LegendKind.Ramp:
            {
                MapColour[] ramp = StandMapOverlayColours.Ramp(entry.Ramp, 8);
                // A checker-free dark backing, then the ramp, so a translucent colour reads as it will over the forest.
                p.fillColor = ToColor(StandMapSymbols.Ground); p.BeginPath(); p.MoveTo(new Vector2(0, 2)); p.LineTo(new Vector2(w, 2)); p.LineTo(new Vector2(w, h - 2)); p.LineTo(new Vector2(0, h - 2)); p.ClosePath(); p.Fill();
                for (int i = 0; i < ramp.Length; i++)
                {
                    float x0 = w * i / ramp.Length, x1 = w * (i + 1) / ramp.Length;
                    p.fillColor = ToColor(ramp[i]); p.BeginPath(); p.MoveTo(new Vector2(x0, 2)); p.LineTo(new Vector2(x1 + 0.4f, 2)); p.LineTo(new Vector2(x1 + 0.4f, h - 2)); p.LineTo(new Vector2(x0, h - 2)); p.ClosePath(); p.Fill();
                }
                break;
            }
        }
    }

    private static void Stroke(Painter2D p, MapSymbolStyle s, float y, float w, float width)
    {
        p.strokeColor = ToColor(s.Colour);
        p.lineWidth = width;
        if (s.DashPx > 0f && s.GapPx > 0f)
        {
            p.lineCap = s.DashPx <= 3f ? LineCap.Round : LineCap.Butt;
            var pieces = new List<MapPosition[]>();
            StandMapGridPlan.Dashes(new[] { new MapPosition(0, y), new MapPosition(w, y) }, s.DashPx, s.GapPx, pieces);
            p.BeginPath();
            foreach (MapPosition[] piece in pieces) { p.MoveTo(new Vector2(piece[0].X, piece[0].Y)); for (int i = 1; i < piece.Length; i++) p.LineTo(new Vector2(piece[i].X, piece[i].Y)); }
            p.Stroke();
        }
        else
        {
            p.lineCap = LineCap.Butt;
            p.BeginPath(); p.MoveTo(new Vector2(0, y)); p.LineTo(new Vector2(w, y)); p.Stroke();
        }
    }

    private static void Circle(Painter2D p, float cx, float cy, float r)
    {
        p.BeginPath();
        const int n = 20;
        for (int k = 0; k < n; k++)
        {
            float a = 2f * Mathf.PI * k / n;
            Vector2 v = new Vector2(cx + r * Mathf.Cos(a), cy + r * Mathf.Sin(a));
            if (k == 0) p.MoveTo(v); else p.LineTo(v);
        }
        p.ClosePath();
    }
}

// The legend card of the full map: a title and one row per category that is on the map right now, two to a row so it stays short.
public sealed class StandMapLegendView
{
    private const int CrownSwatchPx = 22;
    public VisualElement Root { get; }
    private readonly VisualElement body;
    private readonly Dictionary<long, Texture2D> crowns = new Dictionary<long, Texture2D>();
    private string shownKey = "";

    public int RowCount { get; private set; }

    public StandMapLegendView()
    {
        Root = UiKit.Box("card", "map-card", "map-legend");
        Root.name = "map-legend";
        UiKit.Add(Root, "Legend", "map-card-title");
        body = UiKit.Box("map-legend-body");
        Root.Add(body);
        Root.RegisterCallback<DetachFromPanelEvent>(_ => Release());
    }

    // key identifies the content; the rows are rebuilt only when it changes.
    public void Show(StandMapModel model, MapOverlay overlay, string key)
    {
        if (key == shownKey) return;
        shownKey = key;
        body.Clear();
        List<LegendEntry> entries = StandMapLegend.Entries(model, overlay);
        RowCount = entries.Count;
        foreach (LegendEntry entry in entries)
        {
            VisualElement row = UiKit.Box("map-legend-row");
            if (entry.Kind == LegendKind.Ramp) row.AddToClassList("map-legend-row-wide");
            row.Add(new StandMapSwatch(entry, CrownFor(entry)));
            Label text = UiKit.Text(entry.Text, "map-legend-text");
            row.Add(text);
            if (entry.Kind == LegendKind.Ramp) row.Add(UiKit.Text(entry.RampNote, "map-legend-note"));
            body.Add(row);
        }
    }

    private Texture2D CrownFor(LegendEntry entry)
    {
        if (entry.Kind != LegendKind.Crown) return null;
        long key = ((long)(int)entry.Crown << 8) | (long)(int)entry.CrownState;
        if (crowns.TryGetValue(key, out Texture2D found) && found != null) return found;
        byte[] rgba = StandMapCanopyRaster.RenderCrownSwatch(entry.Crown, entry.CrownState, CrownSwatchPx);
        var texture = new Texture2D(CrownSwatchPx, CrownSwatchPx, TextureFormat.RGBA32, false) { name = "Stand map legend crown", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp, hideFlags = HideFlags.HideAndDontSave };
        texture.LoadRawTextureData(rgba);
        texture.Apply(false);
        crowns[key] = texture;
        return texture;
    }

    private void Release()
    {
        foreach (Texture2D t in crowns.Values) if (t != null) Object.Destroy(t);
        crowns.Clear();
        shownKey = "";
    }
}
