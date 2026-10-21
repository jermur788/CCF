using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Paints a StandMapDrawPlan. This one element is used by BOTH the full Stand Map and the HUD minimap: it decides nothing about
// what is drawn or where (StandMapDrawPlan, StandMapTransform and StandMapSymbols do, from the shared model), it only turns the
// shapes into pixels. A presentation chooses a MapDetail (the symbol filter) and a size, nothing else.
//
// The element must be square: the map is the property square. It never takes pointer input; interaction belongs to the
// presentation around it (the full map's cell buttons).
public sealed class StandMapCanvas : VisualElement
{
    private readonly MapDetail detail;
    // The minimap has no cell grid behind it, so it fills the property square itself.
    private readonly bool fillsProperty;
    private StandMapModel model;
    private int paintedVersion = -1;
    private float paintedSide = -1f;
    private List<MapDrawItem> lastPlan = new List<MapDrawItem>();

    public MapDetail Detail => detail;
    // The shapes last actually painted, and the side they were laid out for. Verification reads these.
    public IReadOnlyList<MapDrawItem> LastPlan => lastPlan;
    public float LastSidePx { get; private set; }
    public int PaintedVersion => paintedVersion;

    public StandMapCanvas(MapDetail detail, bool fillsProperty)
    {
        this.detail = detail;
        this.fillsProperty = fillsProperty;
        pickingMode = PickingMode.Ignore;
        generateVisualContent += Paint;
        RegisterCallback<GeometryChangedEvent>(_ => MarkDirtyRepaint());
    }

    // Hand the canvas the current model; it repaints only when the model's content or its own size has changed.
    public void Sync(StandMapModel next)
    {
        model = next;
        if (model != null && (model.Version != paintedVersion || !Mathf.Approximately(Side(), paintedSide)))
            MarkDirtyRepaint();
    }

    private float Side() => Mathf.Min(contentRect.width, contentRect.height);

    private static Color ToColor(MapColour c) => new Color32(c.R, c.G, c.B, c.A);

    private void Paint(MeshGenerationContext context)
    {
        float side = Side();
        if (model == null || float.IsNaN(side) || side < 2f) return;
        paintedVersion = model.Version;
        paintedSide = side;
        LastSidePx = side;
        lastPlan = StandMapDrawPlan.Build(model, side, detail);

        Painter2D painter = context.painter2D;
        painter.lineJoin = LineJoin.Round;
        painter.lineCap = LineCap.Round;
        foreach (MapDrawItem item in lastPlan)
        {
            if (item.Points.Count < 2) continue;
            painter.BeginPath();
            painter.MoveTo(new Vector2(item.Points[0].X, item.Points[0].Y));
            for (int i = 1; i < item.Points.Count; i++)
                painter.LineTo(new Vector2(item.Points[i].X, item.Points[i].Y));
            if (item.Closed) painter.ClosePath();

            if (item.Symbol == MapSymbol.Property && fillsProperty)
            {
                painter.fillColor = ToColor(StandMapSymbols.MiniBackground);
                painter.Fill();
            }
            else if (item.Filled)
            {
                painter.fillColor = ToColor(item.Colour);
                painter.Fill();
            }
            // A filled marker gets a thin dark edge so it reads over any ground colour; everything else is a plain stroke.
            painter.strokeColor = item.Filled ? new Color(0.05f, 0.08f, 0.05f, 0.9f) : ToColor(item.Colour);
            painter.lineWidth = item.Filled ? 1.5f : item.WidthPx;
            painter.Stroke();
        }
    }
}
