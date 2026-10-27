using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// Paints a StandMapDrawPlan. This one element is used by BOTH the full Stand Map and the HUD minimap: it decides nothing about
// what is drawn or where (StandMapDrawPlan, StandMapTransform and StandMapSymbols do, from the shared model), it only turns the
// shapes into pixels. A presentation chooses a MapDetail (the symbol filter) and a size, nothing else.
//
// The shapes are held in two layers, each painted by its own child element:
//   static   the grid, clearing, rack corridors, path, rack lines, juveniles, shelters, marks, boundary and scale: laid out once
//            per change (a rack confirmed, a tree marked), never because you took a step;
//   dynamic  you, the destination and the rack being planned: the only things that change while you walk.
// So walking repaints a handful of shapes, not the whole map. The canopy picture under them is another element (StandMapCanopyLayer).
//
// The element must be square: the map is the property square. It never takes pointer input; interaction belongs to the
// presentation around it (the full map's square buttons).
public sealed class StandMapCanvas : VisualElement
{
    // Cuts one layer's shapes into pixels. It owns nothing but the list it was last given.
    private sealed class LayerPainter : VisualElement
    {
        private List<MapDrawItem> plan = new List<MapDrawItem>();
        public int Repaints { get; private set; }

        public LayerPainter()
        {
            pickingMode = PickingMode.Ignore;
            style.position = Position.Absolute;
            style.left = style.top = style.right = style.bottom = 0;
            generateVisualContent += Paint;
        }

        public void Show(List<MapDrawItem> next)
        {
            plan = next;
            MarkDirtyRepaint();
        }

        private static Color ToColor(MapColour c) => new Color32(c.R, c.G, c.B, c.A);

        private void Paint(MeshGenerationContext context)
        {
            Repaints++;
            Painter2D painter = context.painter2D;
            painter.lineJoin = LineJoin.Round;
            var pieces = new List<MapPosition[]>();
            foreach (MapDrawItem item in plan)
            {
                if (item.Points.Count < 2) continue;
                if (item.Filled && item.Colour.A > 0)
                {
                    painter.BeginPath();
                    Trace(painter, item.Points, item.Closed);
                    painter.fillColor = ToColor(item.Colour);
                    painter.Fill();
                }
                float width = item.Filled ? item.StrokePx : item.WidthPx;
                if (width <= 0f) continue;
                painter.strokeColor = ToColor(item.Filled ? item.Stroke : item.Colour);
                painter.lineWidth = width;
                if (item.DashPx > 0f && item.GapPx > 0f)
                {
                    // A dash shorter than a few pixels is a dot: round caps make it one.
                    painter.lineCap = item.DashPx <= 3f ? LineCap.Round : LineCap.Butt;
                    var line = new List<MapPosition>(item.Points);
                    if (item.Closed) line.Add(item.Points[0]);
                    pieces.Clear();
                    StandMapGridPlan.Dashes(line, item.DashPx, item.GapPx, pieces);
                    painter.BeginPath();
                    foreach (MapPosition[] piece in pieces)
                    {
                        painter.MoveTo(new Vector2(piece[0].X, piece[0].Y));
                        for (int i = 1; i < piece.Length; i++) painter.LineTo(new Vector2(piece[i].X, piece[i].Y));
                    }
                    painter.Stroke();
                }
                else
                {
                    painter.lineCap = LineCap.Round;
                    painter.BeginPath();
                    Trace(painter, item.Points, item.Closed);
                    painter.Stroke();
                }
            }
        }

        private static void Trace(Painter2D painter, List<MapPosition> points, bool closed)
        {
            painter.MoveTo(new Vector2(points[0].X, points[0].Y));
            for (int i = 1; i < points.Count; i++) painter.LineTo(new Vector2(points[i].X, points[i].Y));
            if (closed) painter.ClosePath();
        }
    }

    private readonly MapDetail detail;
    private readonly LayerPainter staticPainter = new LayerPainter(), dynamicPainter = new LayerPainter();
    private StandMapModel model;
    private int paintedStatic = -1, paintedDynamic = -1, paintedVersion = -1;
    private float paintedSide = -1f;
    private MapOverlay overlay, paintedOverlay = (MapOverlay)(-1);
    private List<MapDrawItem> staticPlan = new List<MapDrawItem>(), dynamicPlan = new List<MapDrawItem>(), lastPlan = new List<MapDrawItem>();

    public MapDetail Detail => detail;
    // The shapes last laid out (static layer first), and the side they were laid out for. Verification reads these.
    public IReadOnlyList<MapDrawItem> LastPlan => lastPlan;
    public IReadOnlyList<MapDrawItem> StaticPlan => staticPlan;
    public IReadOnlyList<MapDrawItem> DynamicPlan => dynamicPlan;
    public float LastSidePx { get; private set; }
    public int PaintedVersion => paintedVersion;
    // How often each layer was laid out and painted. A walk must move the dynamic counts and leave the static ones alone.
    public int StaticLayouts { get; private set; }
    public int DynamicLayouts { get; private set; }
    public int StaticRepaints => staticPainter.Repaints;
    public int DynamicRepaints => dynamicPainter.Repaints;

    public StandMapCanvas(MapDetail detail)
    {
        this.detail = detail;
        pickingMode = PickingMode.Ignore;
        Add(staticPainter);
        Add(dynamicPainter);
        RegisterCallback<GeometryChangedEvent>(_ => { if (model != null) Sync(model, overlay); });
    }

    // Hand the canvas the current model and the analytical layer; it lays out a layer again only when that layer's content, the layer
    // choice or its own size has changed.
    public void Sync(StandMapModel next, MapOverlay layer = MapOverlay.None)
    {
        model = next;
        overlay = layer;
        float side = Side();
        if (model == null || float.IsNaN(side) || side < 2f) return;
        bool sizeChanged = !Mathf.Approximately(side, paintedSide);
        bool staticStale = sizeChanged || model.StaticVersion != paintedStatic || overlay != paintedOverlay;
        bool dynamicStale = sizeChanged || model.DynamicVersion != paintedDynamic;
        if (!staticStale && !dynamicStale) return;
        if (staticStale)
        {
            staticPlan = StandMapDrawPlan.BuildStatic(model, side, detail, overlay);
            staticPainter.Show(staticPlan);
            paintedStatic = model.StaticVersion;
            paintedOverlay = overlay;
            StaticLayouts++;
        }
        if (dynamicStale)
        {
            dynamicPlan = StandMapDrawPlan.BuildDynamic(model, side, detail);
            dynamicPainter.Show(dynamicPlan);
            paintedDynamic = model.DynamicVersion;
            DynamicLayouts++;
        }
        paintedSide = side;
        LastSidePx = side;
        paintedVersion = model.Version;
        lastPlan = new List<MapDrawItem>(staticPlan.Count + dynamicPlan.Count);
        lastPlan.AddRange(staticPlan);
        lastPlan.AddRange(dynamicPlan);
    }

    private float Side() => Mathf.Min(contentRect.width, contentRect.height);
}
