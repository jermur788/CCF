using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Plan = TeachingDiagramPlan;

// The teaching picture for the scenario introduction (lesson -1) and for each lesson (0..7). It only DRAWS what TeachingDiagramPlan says; what
// each picture contains, and in which order concepts appear, is decided there (and checked offline).
//
// The picture is original geometry (no imported image): crowns, strips, rings and marks drawn with Painter2D in design units and scaled to the width
// it is given. Its words are ordinary UI Labels laid over the empty top and bottom bands, so they follow the same fonts and sizes as the rest of
// the UI instead of being baked into a picture. The picture is passive: it takes no clicks, and it is a teaching illustration, not a map of the
// player's forest (the real maps are the Stand Map and the HUD minimap).
public sealed class TeachingDiagram : VisualElement
{
    // The widest the picture is drawn, in UI units. Wider than the old 660 so its words can be read at 1280 x 720, where one UI unit is 0.8 pixel.
    private const float MaxWidth = 900f;
    // Text never gets smaller than this however far the picture is scaled down (UI units; the type scale's small and body sizes).
    private const float MinLabelSize = 17f, MinLegendSize = 15f;

    private readonly Plan.Stage stage;
    private readonly VisualElement frame, art;
    private readonly List<KeyValuePair<Label, Plan.Text>> texts = new List<KeyValuePair<Label, Plan.Text>>();
    private readonly List<bool> isLegend = new List<bool>();
    private float shownWidth = -1f;
    // The transform of the item being drawn: its panel's offset and scale (the introduction scales the plantation down), then the width scale.
    private float offsetX, offsetY, panelScale = 1f, scale = 1f;

    // lesson: -1 for the introduction, otherwise the teaching sequence's index (0 = Create access racks).
    public TeachingDiagram(int lesson, string caption)
    {
        stage = Plan.For(lesson);
        name = "teaching-diagram";
        AddToClassList("teaching-diagram");
        pickingMode = PickingMode.Ignore;
        style.flexShrink = 0;

        frame = new VisualElement { name = "teaching-diagram-frame", pickingMode = PickingMode.Ignore };
        frame.AddToClassList("teaching-diagram-frame");
        frame.style.maxWidth = MaxWidth;
        // The first lesson's picture keeps the name the rack onboarding checks look for.
        art = new VisualElement { name = lesson == 0 ? "rack-lesson-diagram" : "teaching-diagram-art", pickingMode = PickingMode.Ignore };
        art.style.position = Position.Absolute;
        art.style.left = 0; art.style.top = 0; art.style.right = 0; art.style.bottom = 0;
        art.generateVisualContent += Draw;
        frame.Add(art);
        foreach (Plan.Text text in stage.Labels) AddText(text, false);
        foreach (Plan.Text text in stage.Legend) AddText(text, true);
        frame.RegisterCallback<GeometryChangedEvent>(OnFrameGeometry);
        Add(frame);

        if (!string.IsNullOrEmpty(caption)) Add(UiKit.Text(caption, "diagram-caption"));
    }

    private void AddText(Plan.Text text, bool legend)
    {
        Label label = UiKit.Text(text.Value, legend ? "diagram-legend" : "diagram-label");
        label.pickingMode = PickingMode.Ignore;
        label.style.color = Colour(text.Role, 1f);
        frame.Add(label);
        texts.Add(new KeyValuePair<Label, Plan.Text>(label, text));
        isLegend.Add(legend);
    }

    // The picture keeps its proportions: the height follows the width, and the words are placed and sized from the same scale.
    private void OnFrameGeometry(GeometryChangedEvent evt)
    {
        float width = evt.newRect.width;
        if (width < 1f || Mathf.Abs(width - shownWidth) < 0.5f) return;
        shownWidth = width;
        float s = width / stage.Width;
        frame.style.height = stage.Height * s;
        for (int i = 0; i < texts.Count; i++)
        {
            Label label = texts[i].Key;
            Plan.Text text = texts[i].Value;
            label.style.left = text.X * s;
            if (isLegend[i])
            {
                // The legend sits on the bottom edge of the picture: its box is as tall as its text (which has a floor), so anchoring from the top
                // let it hang past the frame when the picture is drawn smaller.
                label.style.top = StyleKeyword.Auto;
                label.style.bottom = (stage.Height - text.Y - text.Height) * s;
            }
            else label.style.top = text.Y * s;
            label.style.width = text.Width * s;
            label.style.fontSize = Mathf.Max(isLegend[i] ? MinLegendSize : MinLabelSize, text.FontSize * s);
        }
        art.MarkDirtyRepaint();
    }

    private static Color Colour(Plan.Role role, float alpha)
    {
        Plan.Blend(role, alpha, out byte r, out byte g, out byte b);
        return new Color32(r, g, b, 255);
    }

    private void Draw(MeshGenerationContext context)
    {
        Rect area = art.contentRect;
        if (area.width < 4f || area.height < 4f) return;
        scale = Mathf.Min(area.width / stage.Width, area.height / stage.Height);
        Painter2D p = context.painter2D;
        p.lineJoin = LineJoin.Round;
        p.lineCap = LineCap.Butt;
        offsetX = offsetY = 0f; panelScale = 1f;
        foreach (Plan.Item item in stage.Items) DrawItem(p, item);
        foreach (Plan.Panel panel in stage.Panels)
        {
            offsetX = panel.X; offsetY = panel.Y; panelScale = panel.Scale;
            foreach (Plan.Item item in panel.Items) DrawItem(p, item);
        }
    }

    // ---- design units -> pixels ----
    private float PX(float x) => (offsetX + x * panelScale) * scale;
    private float PY(float y) => (offsetY + y * panelScale) * scale;
    private float PR(float r) => r * panelScale * scale;
    private Vector2 Pt(float x, float y) => new Vector2(PX(x), PY(y));
    // A stroke is the design width scaled with its panel, but never thinner than the minimum.
    private float Stroke(float width) => Mathf.Max(Plan.StrokeMinimum, width * panelScale) * scale;

    private void DrawItem(Painter2D p, Plan.Item item)
    {
        float a = item.Alpha;
        switch (item.Kind)
        {
            case Plan.Kind.Crown:
                Ellipse(p, item.X, item.Y, item.Radius, item.Radius, Colour(item.Role, a), 0f);
                Ellipse(p, item.X, item.Y, item.Radius, item.Radius, Colour(Plan.Role.CanopyEdge, a), Plan.StrokeNormal);
                break;
            case Plan.Kind.Rack:
                float half = item.Width * 0.5f;
                Polygon(p, Colour(item.Role, a * Plan.RackFillAlpha), item.X - half, item.Y, item.X + half, item.Y, item.X + half, item.Y1, item.X - half, item.Y1);
                Line(p, Colour(item.Role, a), Plan.StrokeNormal, item.X - half, item.Y, item.X - half, item.Y1);
                Line(p, Colour(item.Role, a), Plan.StrokeNormal, item.X + half, item.Y, item.X + half, item.Y1);
                break;
            case Plan.Kind.Opening:
                Ellipse(p, item.X, item.Y, item.RadiusX, item.RadiusY, Colour(Plan.Role.Opening, a * Plan.OpeningFillAlpha), 0f);
                DashedEllipse(p, item.X, item.Y, item.RadiusX, item.RadiusY, Colour(item.Role, a));
                break;
            case Plan.Kind.Log:
                Line(p, Colour(item.Role, a), item.Width, item.X, item.Y, item.X1, item.Y1);
                // A short cut end tells timber apart from a management band.
                Line(p, Colour(Plan.Role.Accent, a), Plan.StrokeNormal, item.X1 - 3f, item.Y1 - 8f, item.X1 + 3f, item.Y1 + 8f);
                break;
            case Plan.Kind.Young:
                DrawYoung(p, item);
                break;
            case Plan.Kind.Shelter:
                Line(p, Colour(item.Role, a), Plan.StrokeNormal, item.X - item.Width * 0.5f, item.Y - item.Height * 0.5f, item.X + item.Width * 0.5f, item.Y - item.Height * 0.5f,
                    item.X + item.Width * 0.5f, item.Y + item.Height * 0.5f, item.X - item.Width * 0.5f, item.Y + item.Height * 0.5f, item.X - item.Width * 0.5f, item.Y - item.Height * 0.5f);
                break;
            case Plan.Kind.Clock:
            case Plan.Kind.Repeat:
                DrawCycle(p, item);
                break;
        }
        DrawMarks(p, item);
    }

    private void DrawYoung(Painter2D p, Plan.Item item)
    {
        float a = item.Alpha, r = item.Radius;
        switch (item.Shape)
        {
            case Plan.YoungShape.Oval:
                // A failed young tree keeps its outline and loses its fill.
                Ellipse(p, item.X, item.Y, r * 0.72f, r, Colour(item.Role, a), item.Failed ? Plan.StrokeNormal : 0f);
                break;
            case Plan.YoungShape.Diamond:
                Polygon(p, Colour(item.Role, item.Failed ? a * Plan.FailedDiamondAlpha : a), item.X, item.Y - r, item.X + r, item.Y, item.X, item.Y + r, item.X - r, item.Y);
                break;
            default:
                Ellipse(p, item.X, item.Y, r, r, Colour(item.Role, a), 0f);
                break;
        }
        if (item.Failed)
        {
            // The observed outcome is drawn at full strength even when the rest of the picture is quiet.
            Color warn = Colour(Plan.Role.Warn, 1f);
            Line(p, warn, Plan.StrokeMark, item.X - r, item.Y - r, item.X + r, item.Y + r);
            Line(p, warn, Plan.StrokeMark, item.X - r, item.Y + r, item.X + r, item.Y - r);
        }
    }

    // The management marks sit on top of the crown they belong to, in the strength of that crown.
    private void DrawMarks(Painter2D p, Plan.Item item)
    {
        if (item.Marks == Plan.Marks.None) return;
        float a = item.Alpha;
        if ((item.Marks & Plan.Marks.Band) != 0)
            Line(p, Colour(Plan.Role.Crop, a), Plan.StrokeMark * 1.7f, item.X - 15f, item.Y + 4f, item.X + 15f, item.Y + 4f);
        // The ring always has the original crown's size plus a margin, so a crown that has grown does not grow its ring.
        if ((item.Marks & Plan.Marks.Ring) != 0)
            Ellipse(p, item.X, item.Y, Plan.CrownRadius + 8f, Plan.CrownRadius + 8f, Colour(Plan.Role.Inspect, a), Plan.StrokeMark);
        if ((item.Marks & Plan.Marks.Slash) != 0)
            Line(p, Colour(Plan.Role.Fell, a), Plan.StrokeMark * 1.5f, item.X - 14f, item.Y + 14f, item.X + 14f, item.Y - 14f);
    }

    // A clock face (hands) or a repeat symbol (horizontal arrow): an open circle with an arrowhead at its start.
    private void DrawCycle(Painter2D p, Plan.Item item)
    {
        float a = item.Alpha, r = item.Radius, x = item.X, y = item.Y;
        Color colour = Colour(item.Role, a);
        var arc = new List<float>();
        for (int degrees = -65; degrees <= 240; degrees += 5)
        {
            float t = degrees * Mathf.Deg2Rad;
            arc.Add(x + r * Mathf.Cos(t));
            arc.Add(y + r * Mathf.Sin(t));
        }
        Line(p, colour, Plan.StrokeMark, arc.ToArray());
        float hx = x - r * 0.5f, hy = y - r * 0.866f;
        Polygon(p, colour, hx, hy, hx - 12f, hy + 2f, hx - 2f, hy + 13f);
        if (item.Kind == Plan.Kind.Clock)
            Line(p, colour, Plan.StrokeNormal, x, y - r * 0.45f, x, y, x + r * 0.38f, y + 8f);
        else
        {
            Line(p, colour, Plan.StrokeNormal, x - 18f, y, x + 18f, y);
            Polygon(p, colour, x + 18f, y, x + 7f, y - 8f, x + 7f, y + 8f);
        }
    }

    // ---- primitives ----
    private const int EllipseSteps = 40;

    private void Ellipse(Painter2D p, float cx, float cy, float rx, float ry, Color colour, float strokeWidth)
    {
        p.BeginPath();
        for (int i = 0; i < EllipseSteps; i++)
        {
            float t = i * Mathf.PI * 2f / EllipseSteps;
            Vector2 point = Pt(cx + rx * Mathf.Cos(t), cy + ry * Mathf.Sin(t));
            if (i == 0) p.MoveTo(point); else p.LineTo(point);
        }
        p.ClosePath();
        if (strokeWidth > 0f) { p.strokeColor = colour; p.lineWidth = Stroke(strokeWidth); p.Stroke(); }
        else { p.fillColor = colour; p.Fill(); }
    }

    // Short arcs, 18 degrees drawn and 12 skipped: a boundary of an area, not a line a tree could be selected by.
    private void DashedEllipse(Painter2D p, float cx, float cy, float rx, float ry, Color colour)
    {
        p.strokeColor = colour;
        p.lineWidth = Stroke(Plan.StrokeNormal);
        for (int start = 0; start < 360; start += 30)
        {
            p.BeginPath();
            for (int degrees = start; degrees <= start + 18; degrees += 3)
            {
                float t = degrees * Mathf.Deg2Rad;
                Vector2 point = Pt(cx + rx * Mathf.Cos(t), cy + ry * Mathf.Sin(t));
                if (degrees == start) p.MoveTo(point); else p.LineTo(point);
            }
            p.Stroke();
        }
    }

    private void Polygon(Painter2D p, Color colour, params float[] xy)
    {
        p.BeginPath();
        for (int i = 0; i + 1 < xy.Length; i += 2)
        {
            Vector2 point = Pt(xy[i], xy[i + 1]);
            if (i == 0) p.MoveTo(point); else p.LineTo(point);
        }
        p.ClosePath();
        p.fillColor = colour;
        p.Fill();
    }

    private void Line(Painter2D p, Color colour, float width, params float[] xy)
    {
        p.BeginPath();
        for (int i = 0; i + 1 < xy.Length; i += 2)
        {
            Vector2 point = Pt(xy[i], xy[i + 1]);
            if (i == 0) p.MoveTo(point); else p.LineTo(point);
        }
        p.strokeColor = colour;
        p.lineWidth = Stroke(width);
        p.Stroke();
    }
}
