using UnityEngine;
using UnityEngine.UIElements;

// Small line illustrations in the same V2 accent as other UI, without raster
// background seams. They convey the reference's existing concepts, not controls.
public sealed class OnboardingIcon : VisualElement
{
    private readonly string kind;
    private readonly bool circle;
    private Painter2D pen;
    private float scaleX, scaleY;

    public OnboardingIcon(string kind, bool circle)
    {
        this.kind = kind; this.circle = circle;
        pickingMode = PickingMode.Ignore;
        generateVisualContent += Draw;
    }

    private void Draw(MeshGenerationContext context)
    {
        pen = context.painter2D;
        scaleX = contentRect.width / 100; scaleY = contentRect.height / 100;
        pen.strokeColor = resolvedStyle.color;
        pen.fillColor = resolvedStyle.color;
        pen.lineWidth = kind == "cycle" && !circle ? 3.5f : 2.1f;
        if (circle) Ring(50, 50, 47);
        switch (kind)
        {
            case "brand": Tree(26, 10, 68, true); Tree(73, 10, 68, true); break;
            case "trees": Tree(30, 35, 40); Tree(51, 22, 49); Tree(72, 37, 39); break;
            case "retain": Tree(30, 18, 68); Leaf(61, 51, 1.1f); break;
            case "leaf":
                Path(true, 30, 64, 32, 46, 45, 33, 72, 25, 67, 53, 54, 66, 30, 64);
                Path(false, 24, 76, 64, 37); Path(false, 42, 59, 42, 44); Path(false, 49, 52, 63, 52); break;
            case "sprout":
                Leaf(50, 48, 0.75f); Path(true, 50, 63, 51, 46, 66, 37, 78, 37, 74, 52, 63, 62, 50, 63);
                Path(false, 50, 64, 50, 83); break;
            case "cycle":
            case "forest-cycle":
                float radius = circle ? 31 : 45;
                Arc(50, 50, radius, 185, 340); Arc(50, 50, radius, 5, 160);
                if (circle) { Path(false, 64, 30, 80, 38, 80, 21); Path(false, 36, 70, 20, 62, 20, 79); }
                else { Path(false, 80, 24, 93, 42, 94, 20); Path(false, 20, 76, 7, 58, 6, 80); }
                if (kind == "forest-cycle") { Tree(36, 44, 18); Tree(50, 37, 25); Tree(64, 45, 18); }
                break;
            case "book":
                Path(true, 8, 17, 27, 13, 49, 20, 49, 83, 27, 77, 8, 81);
                Path(true, 51, 20, 73, 13, 92, 17, 92, 81, 73, 77, 51, 83);
                Path(false, 3, 21, 3, 88, 27, 83, 50, 89, 73, 83, 97, 88, 97, 21); break;
            case "observe":
                Ring(27, 64, 20); Ring(73, 64, 20);
                Path(false, 9, 56, 19, 24, 36, 24, 43, 58);
                Path(false, 57, 58, 64, 24, 81, 24, 91, 56); Path(false, 43, 46, 57, 46); break;
            case "logs":
                Ring(28, 68, 17); Ring(58, 76, 17); Ring(27, 37, 14);
                Path(false, 17, 23, 63, 3, 79, 5, 84, 16, 83, 28, 43, 50);
                Path(false, 36, 54, 78, 34, 91, 35, 97, 48, 91, 61, 72, 72); break;
            case "chart":
                Path(true, 10, 66, 20, 66, 20, 90, 10, 90); Path(true, 40, 51, 50, 51, 50, 90, 40, 90);
                Path(true, 70, 32, 80, 32, 80, 90, 70, 90);
                Path(false, 9, 45, 32, 24, 49, 32, 79, 5, 79, 18); Path(false, 63, 6, 79, 5); break;
        }
    }

    private Vector2 Point(float x, float y) => new Vector2(x * scaleX, y * scaleY);
    private void Path(bool close, params float[] coordinates)
    {
        pen.BeginPath(); pen.MoveTo(Point(coordinates[0], coordinates[1]));
        for (int i = 2; i < coordinates.Length; i += 2) pen.LineTo(Point(coordinates[i], coordinates[i + 1]));
        if (close) pen.ClosePath();
        pen.Stroke();
    }
    private void Ring(float x, float y, float radius) => Arc(x, y, radius, 0, 360);
    private void Arc(float x, float y, float radius, float start, float end)
    {
        pen.BeginPath();
        for (int i = 0; i <= 48; i++)
        {
            float angle = Mathf.Lerp(start, end, i / 48f) * Mathf.Deg2Rad;
            Vector2 point = Point(x + Mathf.Cos(angle) * radius, y + Mathf.Sin(angle) * radius);
            if (i == 0) pen.MoveTo(point); else pen.LineTo(point);
        }
        pen.Stroke();
    }
    private void Tree(float x, float y, float height, bool filled = false)
    {
        float w = height * 0.28f;
        pen.BeginPath();
        pen.MoveTo(Point(x, y)); pen.LineTo(Point(x - w * 0.65f, y + height * 0.35f));
        pen.LineTo(Point(x - w * 0.3f, y + height * 0.35f)); pen.LineTo(Point(x - w, y + height * 0.7f));
        pen.LineTo(Point(x - w * 0.5f, y + height * 0.7f)); pen.LineTo(Point(x - w * 1.3f, y + height));
        pen.LineTo(Point(x + w * 1.3f, y + height)); pen.LineTo(Point(x + w * 0.5f, y + height * 0.7f));
        pen.LineTo(Point(x + w, y + height * 0.7f)); pen.LineTo(Point(x + w * 0.3f, y + height * 0.35f));
        pen.LineTo(Point(x + w * 0.65f, y + height * 0.35f)); pen.ClosePath();
        if (filled) pen.Fill(); else pen.Stroke();
        Path(false, x, y + height, x, y + height * 1.25f);
    }
    private void Leaf(float x, float y, float size)
    {
        Path(true, x, y + 20 * size, x - 21 * size, y + 8 * size, x - 24 * size, y - 14 * size,
            x - 4 * size, y - 10 * size, x + 2 * size, y + 4 * size, x, y + 20 * size);
    }
}
