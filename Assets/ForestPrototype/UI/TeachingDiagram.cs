using UnityEngine;
using UnityEngine.UIElements;

// Original schematic UI geometry, deliberately unscaled. No imported images or forestry prescriptions.
public sealed class TeachingDiagram : VisualElement
{
    private readonly int lesson;
    private static readonly Color Green = new Color(.39f, .66f, .42f), Gold = new Color(.96f, .85f, .48f);
    public TeachingDiagram(int lesson, string caption)
    {
        this.lesson = lesson;
        name = "teaching-diagram";
        style.height = 156; style.minHeight = 156; style.flexShrink = 0;
        style.maxWidth = 660; style.marginTop = 6; style.marginBottom = 6;
        pickingMode = PickingMode.Ignore;
        generateVisualContent += Draw;
        string[] labels;
        switch (lesson)
        {
            case -1: labels = new[] { "Even-aged plantation", "Retain + make space", "More varied woodland" }; break;
            case 1: labels = new[] { "Look at the stand", "Compare neighbours", "Favour a future tree" }; break;
            case 2: labels = new[] { "Crowded crowns", "Select a competitor", "Space, with cover" }; break;
            case 3: labels = new[] { "Selected felling", "Choose timber's fate", "Fallen wood / habitat" }; break;
            case 4: labels = new[] { "Work + time", "Observe + review", "Reassess + repeat" }; break;
            case 5: labels = new[] { "Check light + site", "Young tree", "Browsing / protection" }; break;
            case 6: labels = new[] { "Planting year", "Another year", "Survival or failure" }; break;
            default: labels = new[] { "Retained forest", "Young generation", "Continue adapting" }; break;
        }
        var row = new VisualElement(); row.style.flexDirection = FlexDirection.Row;
        row.style.position = Position.Absolute; row.style.left = 0; row.style.right = 0; row.style.top = 4;
        foreach (string text in labels)
        {
            var label = UiKit.Text(text, "muted"); label.style.width = Length.Percent(33.333f);
            label.style.unityTextAlign = TextAnchor.MiddleCenter; label.style.whiteSpace = WhiteSpace.Normal;
            row.Add(label);
        }
        Add(row);
        var note = UiKit.Text(caption, "faint"); note.style.position = Position.Absolute;
        note.style.left = 4; note.style.right = 4; note.style.bottom = 0;
        note.style.whiteSpace = WhiteSpace.Normal; note.style.unityTextAlign = TextAnchor.MiddleCenter; Add(note);
    }
    private void Draw(MeshGenerationContext context)
    {
        Painter2D p = context.painter2D;
        float unit = contentRect.width / 3f;
        void Line(float x, float y, float xx, float yy, Color color, float thick = 3)
        { p.strokeColor = color; p.lineWidth = thick; p.BeginPath(); p.MoveTo(new Vector2(x, y)); p.LineTo(new Vector2(xx, yy)); p.Stroke(); }
        void Tree(float x, float y, float size, Color color)
        {
            Line(x, y, x, y + size, new Color(.60f, .42f, .26f));
            p.fillColor = color; p.BeginPath(); p.MoveTo(new Vector2(x, y - size));
            p.LineTo(new Vector2(x - size * .55f, y + size * .45f)); p.LineTo(new Vector2(x + size * .55f, y + size * .45f)); p.ClosePath(); p.Fill();
        }
        for (int c = 0; c < 3; c++)
        {
            float x = unit * (c + .5f), y = 87;
            Line(unit * c + 12, 116, unit * (c + 1) - 12, 116, Green, 2);
            if (c < 2)
            { float a = unit * (c + 1); Line(a - 12, 77, a + 12, 77, Gold, 2); Line(a + 5, 70, a + 12, 77, Gold, 2); Line(a + 5, 84, a + 12, 77, Gold, 2); }
            if (lesson == 4)
            {
                p.strokeColor = Gold; p.lineWidth = 3; p.BeginPath(); p.Arc(new Vector2(x, 79), 25, 30, 320); p.Stroke();
                Line(x + 18, 58, x + 25, 56, Gold); Line(x + 25, 56, x + 25, 66, Gold);
                Tree(x, 91, 15, Green);
            }
            else if (lesson == 5 || lesson == 6)
            {
                Tree(x, y + 12, c == 0 && lesson == 5 ? 25 : 13 + c * 3, Green);
                if (lesson == 5 && c == 0) { Line(x + 26, 51, x + 18, 69, Gold); Line(x + 35, 59, x + 28, 77, Gold); }
                if (lesson == 5 && c == 2) { Line(x - 18, 76, x - 18, 116, Gold); Line(x + 18, 76, x + 18, 116, Gold); Line(x - 18, 88, x + 18, 88, Gold, 2); }
                if (lesson == 6 && c == 2) { Line(x + 32, 96, x + 32, 116, new Color(.63f, .46f, .30f)); Line(x + 26, 101, x + 37, 106, Gold, 2); }
            }
            else if (lesson == 3)
            {
                Tree(x - 25, y, 25, Green);
                if (c == 0) { Tree(x + 22, y, 24, Green); Line(x + 8, 65, x + 36, 98, Gold); }
                else { Line(x + 3, 110, x + 40, 110, new Color(.67f, .46f, .28f), 9); }
            }
            else
            {
                for (int t = -1; t <= 1; t++)
                {
                    if (lesson == 2 && c == 2 && t == 1) continue;
                    if (lesson == -1 && c == 1 && t == 0) continue;
                    float size = (lesson == -1 || lesson == 7) && c > 0 ? 15 + (t + 1) * 8 : 26;
                    Tree(x + t * 29, y, size, (lesson == 1 || lesson == 2) && c > 0 && t == 0 ? Gold : Green);
                }
                if (lesson == 2 && c == 1) { Line(x + 15, 58, x + 40, 97, Gold); Line(x + 40, 58, x + 15, 97, Gold); }
            }
        }
    }
}
