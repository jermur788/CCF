using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// The Treatment review layer of the full Stand Map: area outlines and hatching, candidate and proposal glyphs, and a printed tag on
// each area. It is one element laid over the shared map surface (never inside it), so the HUD minimap, the shared model and the shared
// draw plan are untouched. It paints shapes decided by StandMapReviewPlan and takes no pointer input.
public sealed class StandMapReviewLayer : VisualElement
{
    private List<ReviewShape> shapes = new List<ReviewShape>();
    private readonly VisualElement tags;
    public int Repaints { get; private set; }
    public IReadOnlyList<ReviewShape> Shapes => shapes;

    public StandMapReviewLayer()
    {
        name = "map-review-layer";
        pickingMode = PickingMode.Ignore;
        style.position = Position.Absolute;
        style.left = style.top = style.right = style.bottom = 0;
        style.display = DisplayStyle.None;
        generateVisualContent += Paint;
        tags = new VisualElement { pickingMode = PickingMode.Ignore };
        tags.style.position = Position.Absolute;
        tags.style.left = tags.style.top = tags.style.right = tags.style.bottom = 0;
        Add(tags);
    }

    public void Hide() => style.display = DisplayStyle.None;

    public void Show(List<ReviewShape> next, IReadOnlyList<ReviewFootprint> footprints)
    {
        shapes = next ?? new List<ReviewShape>();
        style.display = DisplayStyle.Flex;
        tags.Clear();
        if (footprints != null)
            foreach (ReviewFootprint fp in footprints)
            {
                if (fp.Polygon.Count < 3 || string.IsNullOrEmpty(fp.Tag)) continue;
                ReviewPoint c = StandMapReviewPlan.Centroid(fp.Polygon);
                Label tag = new Label(fp.Tag) { pickingMode = PickingMode.Ignore };
                tag.AddToClassList("map-review-tag");
                tag.style.position = Position.Absolute;
                tag.style.left = c.X - 58f;
                tag.style.top = c.Y - 8f;
                tag.style.width = 116f;
                tag.style.unityTextAlign = TextAnchor.MiddleCenter;
                tag.style.fontSize = 10;
                tag.style.color = new Color(0.97f, 0.97f, 0.93f, 1f);
                tag.style.backgroundColor = new Color(0.05f, 0.07f, 0.06f, 0.78f);
                tag.style.paddingLeft = tag.style.paddingRight = 2;
                tags.Add(tag);
            }
        MarkDirtyRepaint();
    }

    private void Paint(MeshGenerationContext context)
    {
        Repaints++;
        Painter2D painter = context.painter2D;
        painter.lineJoin = LineJoin.Round;
        var pieces = new List<MapPosition[]>();
        foreach (ReviewShape shape in shapes)
        {
            if (shape.Points.Count < 2 || shape.WidthPx <= 0f) continue;
            painter.strokeColor = new Color32(shape.R, shape.G, shape.B, shape.A);
            painter.lineWidth = shape.WidthPx;
            painter.BeginPath();
            if (shape.DashPx > 0f && shape.GapPx > 0f)
            {
                painter.lineCap = shape.DashPx <= 3f ? LineCap.Round : LineCap.Butt;
                var line = new List<MapPosition>();
                foreach (ReviewPoint p in shape.Points) line.Add(new MapPosition(p.X, p.Y));
                if (shape.Closed) line.Add(line[0]);
                pieces.Clear();
                StandMapGridPlan.Dashes(line, shape.DashPx, shape.GapPx, pieces);
                foreach (MapPosition[] piece in pieces)
                {
                    painter.MoveTo(new Vector2(piece[0].X, piece[0].Y));
                    for (int i = 1; i < piece.Length; i++) painter.LineTo(new Vector2(piece[i].X, piece[i].Y));
                }
            }
            else
            {
                painter.lineCap = LineCap.Round;
                painter.MoveTo(new Vector2(shape.Points[0].X, shape.Points[0].Y));
                for (int i = 1; i < shape.Points.Count; i++) painter.LineTo(new Vector2(shape.Points[i].X, shape.Points[i].Y));
                if (shape.Closed) painter.ClosePath();
            }
            painter.Stroke();
        }
    }
}
