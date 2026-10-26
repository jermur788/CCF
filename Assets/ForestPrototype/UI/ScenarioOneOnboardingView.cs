using System;
using UnityEngine;
using UnityEngine.UIElements;

// A reference-sized illustration layout scaled uniformly into the UI panel.
// Text and controls are live UI Toolkit elements. Only forestry pictures, icons
// and the blurred backdrop are sampled from the frozen reference artwork atlas.
public sealed class ScenarioOneOnboardingView
{
    private const float DesignWidth = 1672f, DesignHeight = 941f;
    private readonly VisualElement canvas;
    private readonly Action completed;
    private Texture2D artwork;
    public VisualElement Root { get; }
    public int PageIndex { get; private set; }
    public bool IsOpen { get; private set; }

    public ScenarioOneOnboardingView(Action completed)
    {
        this.completed = completed;
        Root = UiKit.Box("layer", "onboarding");
        Root.name = "scenario-onboarding";
        Root.style.display = DisplayStyle.None;
        UiKit.MakeModal(Root);
        canvas = UiKit.Box("onboarding-canvas");
        canvas.style.width = DesignWidth;
        canvas.style.height = DesignHeight;
        canvas.style.transformOrigin = new TransformOrigin(Length.Percent(0), Length.Percent(0), 0);
        Root.Add(canvas);
        Root.RegisterCallback<GeometryChangedEvent>(e => Fit(e.newRect.size));
    }

    public void Show()
    {
        IsOpen = true;
        PageIndex = 0;
        Root.style.display = DisplayStyle.Flex;
        Render();
        Fit(Root.layout.size);
    }

    public void Next()
    {
        if (!IsOpen) return;
        if (PageIndex == 2) { Hide(); completed(); }
        else { PageIndex++; Render(); }
    }

    public void Back()
    {
        if (!IsOpen || PageIndex == 0) return;
        PageIndex--;
        Render();
    }

    // Only the explicit load/session cleanup path may dismiss an unfinished sequence.
    public void Hide() { IsOpen = false; Root.style.display = DisplayStyle.None; }

    private void Fit(Vector2 area)
    {
        if (area.x <= 0 || area.y <= 0) return;
        float scale = Mathf.Min(area.x / DesignWidth, area.y / DesignHeight);
        canvas.style.scale = new Scale(new Vector3(scale, scale, 1));
        canvas.style.left = (area.x - DesignWidth * scale) / 2;
        canvas.style.top = (area.y - DesignHeight * scale) / 2;
    }

    private void Render()
    {
        canvas.Clear();
        artwork = Resources.Load<Texture2D>("Onboarding/page-" + (PageIndex + 1));
        if (artwork == null) Debug.LogError("CCF_ONBOARDING_ART_MISSING page=" + (PageIndex + 1));
        // Only the margins of this full atlas remain exposed behind the opaque live panel.
        Art(new Rect(0, 0, DesignWidth, DesignHeight));
        float right = PageIndex == 2 ? 1574 : 1607;
        float bottom = PageIndex == 2 ? 905 : 891;
        Box("onboarding-panel", 64, 64, right - 64, bottom - 64);
        Icon("brand", 108, 90, 58, 74, false);
        Text(PageIndex == 2 ? "SCENARIO ONE" : "INTRODUCTION", 190, 92, 600, 25, 16, "onboarding-eyebrow");
        Text(ScenarioOneOnboardingCopy.Titles[PageIndex], 190, 117, right - 222, 62, 50, "title");
        string lead = ScenarioOneOnboardingCopy.Leads[PageIndex];
        if (PageIndex == 0) lead = lead.Replace("managed. Instead", "managed.\nInstead");
        Text(lead, 112, 179, right - 145, 72, 24);
        Rule(85, PageIndex == 2 ? 253 : 258, right - 106);
        if (PageIndex == 0) Definition();
        else if (PageIndex == 1) Process();
        else Scenario();
        float footerY = PageIndex == 2 ? 830 : 818;
        Rule(79, footerY, right - 94);
        Icon("book", 96, PageIndex == 2 ? 843 : 830, 56, 45);
        VerticalRule(170, footerY + 13, 45);
        Text(ScenarioOneOnboardingCopy.Footers[PageIndex], 191, footerY + 13,
            PageIndex == 0 ? 1055 : 865, 48, PageIndex == 0 ? 22 : 19);
        if (PageIndex > 0)
            Button("←   Back", "onboarding-back", 1082, footerY + 12, 154, 46, Back, false);
        for (int i = 0; i < 3; i++)
        {
            var dot = Box(i == PageIndex ? "onboarding-dot onboarding-dot-current" : "onboarding-dot",
                (PageIndex == 2 ? 1272 : 1290) + i * 30, footerY + 30, 13, 13);
            dot.name = "onboarding-page-indicator-" + (i + 1);
        }
        Button(PageIndex == 2 ? "Begin    →" : "Next    →", "onboarding-next",
            right - 193, footerY + 12, 174, 46, Next, true);
    }

    private void Definition()
    {
        Art(new Rect(85, 272, 1501, 322));
        for (int i = 0; i < 3; i++)
        {
            Text(ScenarioOneOnboardingCopy.ProgressionHeadings[i], 90 + i * 505, 598, 490, 25, 16, "onboarding-eyebrow", true);
            Text(ScenarioOneOnboardingCopy.ProgressionCaptions[i], 90 + i * 505, 623, 490, 32, 18, null, true);
        }
        Rule(88, 663, 1490);
        int[] icons = { 106, 466, 843, 1244 };
        int[] labels = { 212, 571, 949, 1353 };
        int[] widths = { 218, 237, 260, 216 };
        for (int i = 0; i < 4; i++)
        {
            Icon(new[] { "leaf", "cycle", "trees", "sprout" }[i], icons[i], 684, 80, 80);
            Text(ScenarioOneOnboardingCopy.Principles[i], labels[i], 700, widths[i], 84, 25);
        }
        VerticalRule(435, 687, 110); VerticalRule(815, 687, 110); VerticalRule(1216, 687, 110);
    }

    private void Process()
    {
        float[] left = { 85, 399, 702, 1002, 1296 };
        float[] width = { 297, 287, 282, 277, 291 };
        for (int i = 0; i < 5; i++)
        {
            float x = left[i], w = width[i];
            Box("onboarding-card", x, 274, w, 520);
            var number = Text((i + 1).ToString(), x + 13, 293, 54, 54, 32, "onboarding-number", true);
            number.style.unityTextAlign = TextAnchor.MiddleCenter;
            Text(ScenarioOneOnboardingCopy.ProcessHeadings[i], x + 89, 300, w - 100, 62, 23);
            Art(new Rect(x + 1, 365, w - 2, 312));
            Icon(new[] { "observe", "retain", "logs", "sprout", "chart" }[i], x + 15, 706, 64, 63, false);
            Text(ScenarioOneOnboardingCopy.ProcessStatements[i], x + (i == 3 ? 90 : 104), 683, w - (i == 3 ? 99 : 115), 110, i == 3 ? 16 : 18);
            if (i < 4) Text("›", x + w - 4, 480, 28, 55, 51, "onboarding-accent", true);
        }
    }

    private void Scenario()
    {
        Art(new Rect(83, 264, 580, 326), new[] {
            new Vector2(0, 225), new Vector2(24, 130), new Vector2(65, 70), new Vector2(120, 30),
            new Vector2(180, 0), new Vector2(340, 0), new Vector2(425, 40), new Vector2(500, 90),
            new Vector2(550, 145), new Vector2(580, 225), new Vector2(580, 255), new Vector2(290, 326), new Vector2(0, 255)
        });
        Art(new Rect(961, 264, 592, 326), new[] {
            new Vector2(0, 225), new Vector2(24, 150), new Vector2(70, 70), new Vector2(135, 25),
            new Vector2(215, 0), new Vector2(460, 0), new Vector2(520, 70), new Vector2(565, 130),
            new Vector2(592, 225), new Vector2(592, 255), new Vector2(296, 326), new Vector2(0, 255)
        });
        // Only the cycle artwork is sampled; its explanatory words remain live labels.
        Icon("cycle", 765, 349, 108, 108, false);
        Text("→", 680, 391, 50, 60, 51, "onboarding-accent", true);
        Text("→", 910, 391, 50, 60, 51, "onboarding-accent", true);
        Text("REPEATED\nSELECTIVE\nMANAGEMENT", 706, 466, 230, 76, 20, "onboarding-eyebrow", true);
        Text("Over many years", 715, 551, 211, 28, 21, null, true);
        Text("INITIAL STATE", 90, 589, 574, 26, 17, "onboarding-eyebrow", true);
        Text("Even-aged Sitka plantation", 90, 614, 574, 28, 21, null, true);
        Text("LONG-TERM GOAL", 965, 589, 586, 26, 17, "onboarding-eyebrow", true);
        Text("Sitka, Oak and Beech under continuous cover", 965, 614, 586, 28, 21, null, true);
        Rule(87, 643, 1465);
        var row = Box("onboarding-info-row", 97, 659, 1442, 160);
        ScenarioColumn(row, "trees", "Initial state", "• " + ScenarioOneOnboardingCopy.InitialState.Replace("\n", "\n• "));
        ScenarioColumn(row, "sprout", "Target state", "• " + ScenarioOneOnboardingCopy.TargetState.Replace("\n", "\n• "));
        ScenarioColumn(row, "forest-cycle", "Key idea", ScenarioOneOnboardingCopy.KeyIdea);
    }

    private static void ScenarioColumn(VisualElement row, string iconKind, string heading, string body)
    {
        var column = UiKit.Box("onboarding-info-column");
        if (row.childCount > 0) column.AddToClassList("onboarding-info-column-divider");
        var header = UiKit.Box("onboarding-info-header");
        var icon = new OnboardingIcon(iconKind, true);
        icon.AddToClassList("onboarding-accent");
        icon.AddToClassList("onboarding-info-icon");
        header.Add(icon);
        header.Add(UiKit.Text(heading, "onboarding-copy", "title", "onboarding-info-title"));
        column.Add(header);
        column.Add(UiKit.Text(body, "onboarding-copy", "onboarding-info-body"));
        row.Add(column);
    }

    private VisualElement Box(string classes, float x, float y, float width, float height)
    {
        var box = UiKit.Box(classes.Split(' '));
        Place(box, x, y, width, height); canvas.Add(box); return box;
    }

    private void Rule(float x, float y, float width) => Box("onboarding-rule", x, y, width, 1);
    private void VerticalRule(float x, float y, float height) => Box("onboarding-rule", x, y, 1, height);

    private Label Text(string text, float x, float y, float width, float height, float size,
        string kind = null, bool centered = false)
    {
        Label label = kind == null ? UiKit.Text(text, "onboarding-copy") : UiKit.Text(text, "onboarding-copy", kind);
        label.style.fontSize = size;
        if (centered) label.style.unityTextAlign = TextAnchor.UpperCenter;
        Place(label, x, y, width, height); canvas.Add(label); return label;
    }

    private void Button(string text, string name, float x, float y, float width, float height, Action action, bool primary)
    {
        var button = UiKit.Button(text, action, true, primary ? "btn-primary" : "btn-ghost", "onboarding-action");
        button.name = name;
        button.focusable = true;
        // Enter is handled by this focused button, so the root must not also advance it.
        button.RegisterCallback<KeyDownEvent>(e => e.StopPropagation());
        Place(button, x, y, width, height); canvas.Add(button);
        if (primary) button.schedule.Execute(() => button.Focus());
    }

    private static void Place(VisualElement element, float x, float y, float width, float height)
    {
        element.style.position = Position.Absolute;
        element.style.left = x; element.style.top = y;
        element.style.width = width; element.style.height = height;
    }

    private void Icon(string kind, float x, float y, float width, float height, bool circle = true)
    {
        var icon = new OnboardingIcon(kind, circle);
        icon.AddToClassList("onboarding-accent");
        Place(icon, x, y, width, height); canvas.Add(icon);
    }

    private void Art(Rect source, Vector2[] contour = null)
    {
        var picture = new ArtworkRegion(artwork, source, contour);
        Place(picture, source.x, source.y, source.width, source.height);
        canvas.Add(picture);
    }

    // Texture UVs preserve the exact approved illustrations without re-encoding,
    // duplicating crops or rasterising the interface's text/buttons into an image.
    private sealed class ArtworkRegion : VisualElement
    {
        private readonly Texture2D texture;
        private readonly Rect source;
        private readonly Vector2[] contour;
        public ArtworkRegion(Texture2D texture, Rect source, Vector2[] contour)
        {
            this.texture = texture; this.source = source; this.contour = contour;
            pickingMode = PickingMode.Ignore;
            generateVisualContent += Draw;
        }

        private void WriteVertex(MeshWriteData mesh, Vector2 point, Rect uv, byte alpha = 255)
        {
            mesh.SetNextVertex(new Vertex {
                position = new Vector3(point.x / source.width * contentRect.width, point.y / source.height * contentRect.height, Vertex.nearZ),
                tint = new Color32(255, 255, 255, alpha),
                uv = new Vector2(uv.x + (source.x + point.x) / DesignWidth * uv.width,
                    uv.y + (1 - (source.y + point.y) / DesignHeight) * uv.height)
            });
        }

        private void Draw(MeshGenerationContext context)
        {
            if (texture == null) return;
            if (contour != null)
            {
                // A silhouette-shaped texture frame removes rectangular background seams
                // while keeping the exact frozen illustration pixels and species unchanged.
                MeshWriteData clipped = context.Allocate(contour.Length * 2 + 1, contour.Length * 9, texture);
                Rect atlas = clipped.uvRegion;
                Vector2 center = new Vector2(source.width / 2, source.height / 2);
                WriteVertex(clipped, center, atlas);
                foreach (Vector2 point in contour) WriteVertex(clipped, point, atlas);
                foreach (Vector2 point in contour)
                {
                    Vector2 outside = center + (point - center) * 1.13f;
                    outside.x = Mathf.Clamp(outside.x, 0, source.width);
                    outside.y = Mathf.Clamp(outside.y, 0, source.height);
                    WriteVertex(clipped, outside, atlas, 0);
                }
                for (int i = 0; i < contour.Length; i++)
                {
                    clipped.SetNextIndex(0); clipped.SetNextIndex((ushort)(i + 1));
                    clipped.SetNextIndex((ushort)((i + 1) % contour.Length + 1));
                    ushort a = (ushort)(i + 1), b = (ushort)((i + 1) % contour.Length + 1);
                    ushort c = (ushort)(a + contour.Length), d = (ushort)(b + contour.Length);
                    clipped.SetNextIndex(a); clipped.SetNextIndex(c); clipped.SetNextIndex(d);
                    clipped.SetNextIndex(d); clipped.SetNextIndex(b); clipped.SetNextIndex(a);
                }
                return;
            }
            MeshWriteData mesh = context.Allocate(4, 6, texture);
            Rect uv = mesh.uvRegion;
            float left = uv.x + source.x / DesignWidth * uv.width;
            float right = uv.x + source.xMax / DesignWidth * uv.width;
            float top = uv.y + (1 - source.y / DesignHeight) * uv.height;
            float bottom = uv.y + (1 - source.yMax / DesignHeight) * uv.height;
            Color32 white = Color.white;
            mesh.SetNextVertex(new Vertex { position = new Vector3(0, 0, Vertex.nearZ), tint = white, uv = new Vector2(left, top) });
            mesh.SetNextVertex(new Vertex { position = new Vector3(contentRect.width, 0, Vertex.nearZ), tint = white, uv = new Vector2(right, top) });
            mesh.SetNextVertex(new Vertex { position = new Vector3(contentRect.width, contentRect.height, Vertex.nearZ), tint = white, uv = new Vector2(right, bottom) });
            mesh.SetNextVertex(new Vertex { position = new Vector3(0, contentRect.height, Vertex.nearZ), tint = white, uv = new Vector2(left, bottom) });
            mesh.SetNextIndex(0); mesh.SetNextIndex(1); mesh.SetNextIndex(2);
            mesh.SetNextIndex(2); mesh.SetNextIndex(3); mesh.SetNextIndex(0);
        }
    }
}
