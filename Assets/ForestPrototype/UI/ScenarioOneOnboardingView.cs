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
        Art(new Rect(108, 90, 58, 74));
        Text(PageIndex == 2 ? "SCENARIO ONE" : "INTRODUCTION", 190, 92, 600, 25, 16, "onboarding-eyebrow");
        Text(ScenarioOneOnboardingCopy.Titles[PageIndex], 190, 117, right - 222, 62, 50, "title");
        Text(ScenarioOneOnboardingCopy.Leads[PageIndex], 112, 179, right - 145, 72, 24);
        Rule(85, PageIndex == 2 ? 253 : 258, right - 106);
        if (PageIndex == 0) Definition();
        else if (PageIndex == 1) Process();
        else Scenario();
        float footerY = PageIndex == 2 ? 830 : 818;
        Rule(79, footerY, right - 94);
        Art(new Rect(96, PageIndex == 2 ? 843 : 830, 56, 45));
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
            Art(new Rect(icons[i], 684, 80, 80));
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
            Art(new Rect(x + 15, 706, 64, 63));
            Text(ScenarioOneOnboardingCopy.ProcessStatements[i], x + 104, 694, w - 115, 96, i == 3 ? 16 : 18);
            if (i < 4) Text("›", x + w - 4, 480, 28, 55, 51, "onboarding-accent", true);
        }
    }

    private void Scenario()
    {
        Art(new Rect(83, 264, 580, 326));
        Art(new Rect(961, 264, 592, 326));
        // Only the cycle artwork is sampled; its explanatory words remain live labels.
        Art(new Rect(680, 347, 278, 110));
        Text("REPEATED\nSELECTIVE\nMANAGEMENT", 726, 466, 190, 76, 20, "onboarding-eyebrow", true);
        Text("Over many years", 715, 551, 211, 28, 21, null, true);
        Text("INITIAL STATE", 90, 589, 574, 26, 17, "onboarding-eyebrow", true);
        Text("Even-aged Sitka plantation", 90, 614, 574, 28, 21, null, true);
        Text("LONG-TERM GOAL", 965, 589, 586, 26, 17, "onboarding-eyebrow", true);
        Text("Sitka, Oak and Beech under continuous cover", 965, 614, 586, 28, 21, null, true);
        Rule(87, 643, 1465);
        Art(new Rect(97, 660, 84, 85));
        Art(new Rect(585, 659, 84, 85));
        Art(new Rect(1083, 660, 86, 85));
        Text("Initial state", 200, 661, 340, 35, 26, "title");
        Text("Target state", 703, 661, 338, 35, 26, "title");
        Text("Key idea", 1202, 661, 336, 35, 26, "title");
        Text("• " + ScenarioOneOnboardingCopy.InitialState.Replace("\n", "\n• "), 200, 700, 345, 104, 21);
        Text("• " + ScenarioOneOnboardingCopy.TargetState.Replace("\n", "\n• "), 703, 697, 347, 122, 20);
        Text(ScenarioOneOnboardingCopy.KeyIdea, 1202, 697, 337, 119, 21);
        VerticalRule(552, 659, 155); VerticalRule(1054, 659, 155);
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

    private void Art(Rect source)
    {
        var picture = new ArtworkRegion(artwork, source);
        Place(picture, source.x, source.y, source.width, source.height);
        canvas.Add(picture);
    }

    // Texture UVs preserve the exact approved illustrations without re-encoding,
    // duplicating crops or rasterising the interface's text/buttons into an image.
    private sealed class ArtworkRegion : VisualElement
    {
        private readonly Texture2D texture;
        private readonly Rect source;
        public ArtworkRegion(Texture2D texture, Rect source)
        {
            this.texture = texture; this.source = source;
            pickingMode = PickingMode.Ignore;
            generateVisualContent += Draw;
        }

        private void Draw(MeshGenerationContext context)
        {
            if (texture == null) return;
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
