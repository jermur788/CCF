#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

// Disposable rendered verification for SCENARIO-ONE-UX-PEDAGOGY-V1 (not a regression gate for other tasks). It renders the screens this task changed
// at 1280 x 720 and 1920 x 1080 in the real UI, saves a screenshot of each, and adds numeric audits so a defect is measured, not only looked at:
//   intro (+ More detail), contextual help windows, HUD after the intro, the Objectives card for every lesson stage (with its picture), the Work Plan
//   at three stages, and the selected-tree treatment in four poses with a pixel diff proving the indicator adds no white.
// Lesson stages are forced by injecting teaching facts into the Objectives view (its facts cache), so the screens do not depend on playing through;
// real progression is covered by the separate guided / sequential-objectives gate. Nothing here writes game state that is saved.
//
// Run (graphical Editor, private Xephyr):  -executeMethod UxPedagogyShots.Begin   CCF_ACCEPTANCE_OUTPUT=<folder>   [CCF_UI_SIZES=1280x720,1920x1080]
// Results: UXP_* lines in the log, <name>-<w>x<h>.png screenshots, text-sizes.csv.
public static class UxPedagogyShots
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    public static void SetGameSize(int width, int height)
    {
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "UxPedagogy " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "UxPedagogyShots.Begin")) return;
        // Every first-use screen counts as seen, so none opens by itself over a shot; the shots open the ones they need.
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "UxPedagogyShots.Begin"))
            new GameObject("UX pedagogy shots").AddComponent<UxPedagogyShotsRunner>();
    }
}

public sealed class UxPedagogyShotsRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ForestPlayer player;
    private ScenarioOneManager manager;
    private string outDir;
    private int passes, failures, shotCount;
    private Vector2Int size;
    private Texture2D lastCapture;
    private readonly List<string> textRows = new List<string> { "state,size,uiScale,labels,minPx,medianPx,under12px,smallest" };

    private IEnumerator Start()
    {
        outDir = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT") ?? Path.Combine(Application.dataPath, "..", "Logs");
        Directory.CreateDirectory(outDir);
        for (int i = 0; i < 40; i++) yield return null;
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        player = FindFirstObjectByType<ForestPlayer>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        if (ui == null || player == null || manager == null) { Debug.Log("UXP_UNVERIFIED missing ui/player/manager"); Finish(1); yield break; }
        for (int i = 0; i < 90 && !(Application.isFocused); i++) yield return null;
        var sizes = (Environment.GetEnvironmentVariable("CCF_UI_SIZES") ?? "1280x720,1920x1080").Split(',')
            .Select(s => s.Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
        Debug.Log($"UXP_ENV focused={Application.isFocused} sizes={string.Join(",", sizes)} stand={manager != null} trees={FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Length}");
        foreach (Vector2Int each in sizes)
        {
            size = each;
            UxPedagogyShots.SetGameSize(size.x, size.y);
            for (int i = 0; i < 40; i++) yield return null;
            Debug.Log($"UXP_SIZE {size.x}x{size.y} screen={Screen.width}x{Screen.height} uiWidth={ui.RootElement.layout.width:0} uiHeight={ui.RootElement.layout.height:0} scale={UiScale():0.000}");
            yield return Section("intro", Intro());
            yield return Section("help", HelpWindows());
            yield return Section("hud", Hud());
            yield return Section("objectives", Objectives());
            yield return Section("workplan", WorkPlan());
            yield return Section("selection", Selection());
        }
        File.WriteAllLines(Path.Combine(outDir, "text-sizes.csv"), textRows);
        Debug.Log($"UXP_DONE pass={passes} fail={failures} shots={shotCount}");
        Finish(failures == 0 ? 0 : 1);
    }

    // ---- plumbing ----
    private IEnumerator Section(string name, IEnumerator body)
    {
        while (true)
        {
            bool more;
            try { more = body.MoveNext(); }
            catch (Exception e) { Fail("SECTION_" + name + "_THREW", e.GetType().Name + ": " + e.Message + " @ " + (e.StackTrace ?? "").Split('\n').FirstOrDefault()); yield break; }
            if (!more) yield break;
            yield return body.Current;
        }
    }

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        Debug.Log((ok ? "UXP_CHECK_PASS " : "UXP_CHECK_FAIL ") + $"[{size.x}x{size.y}] {name} {detail}");
    }
    private void Fail(string name, string detail) => Check(false, name, detail);

    private static IEnumerator Pause(int frames = 10) { for (int i = 0; i < frames; i++) yield return null; }

    private IEnumerator Invoke(Button button)
    {
        if (button == null) { Fail("BUTTON_MISSING", ""); yield break; }
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return Pause(8);
    }

    private static bool Visible(VisualElement e)
    {
        for (VisualElement p = e; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity <= 0.01f) return false;
        return e.worldBound.width > 0 && e.worldBound.height > 0;
    }

    private static string Texts(VisualElement root) => string.Join("\n", root.Query<TextElement>().ToList().Where(Visible).Select(e => e.text));

    // Physical pixels per UI unit: the panel scales with screen height from a 1600 x 900 reference.
    private float UiScale() => ui.RootElement.layout.width > 0 ? Screen.width / ui.RootElement.layout.width : 1f;

    private IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();
        if (lastCapture != null) Destroy(lastCapture);
        lastCapture = ScreenCapture.CaptureScreenshotAsTexture();
    }

    // A capture of an Editor window that never got a real surface comes back uniform (white). Catch that instead of passing blank screenshots.
    private static bool LooksBlank(Texture2D image)
    {
        Color32[] pixels = image.GetPixels32();
        int step = Mathf.Max(1, pixels.Length / 4000), different = 0;
        Color32 first = pixels[0];
        for (int i = 0; i < pixels.Length; i += step)
            if (Mathf.Abs(pixels[i].r - first.r) + Mathf.Abs(pixels[i].g - first.g) + Mathf.Abs(pixels[i].b - first.b) > 24) different++;
        return different < 40;
    }

    private IEnumerator Shot(string name)
    {
        yield return Capture();
        if (lastCapture == null || lastCapture.width < 600) { Fail("SHOT_" + name, "no rendered capture"); yield break; }
        Check(!LooksBlank(lastCapture), "SHOT_IS_NOT_BLANK_" + name, "a uniform capture means the Game view had no real surface; do not read it as a screenshot");
        File.WriteAllBytes(Path.Combine(outDir, $"{name}-{size.x}x{size.y}.png"), lastCapture.EncodeToPNG());
        shotCount++;
        Debug.Log($"UXP_SHOT {name}-{size.x}x{size.y}.png {lastCapture.width}x{lastCapture.height}");
    }

    private void Finish(int code)
    {
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(code);
    }

    // Force the Objectives view (and so the HUD widget and the Work Plan focus) to a lesson stage by injecting the facts it reads. Stage 0 restores the real facts.
    private void ForceStage(int stage)
    {
        var learning = ui.Learning;
        BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        FieldInfo facts = typeof(LearningObjectivesView).GetField("sequenceFacts", flags), next = typeof(LearningObjectivesView).GetField("nextSequenceRefresh", flags);
        if (stage <= 0) { facts.SetValue(learning, null); next.SetValue(learning, 0f); learning.Refresh(false); return; }
        var f = new ScenarioOneTeachingSequence.Facts { AccessOpen = true, Year = 1 };
        if (stage >= 2) { f.AccessReviewed = true; f.CropTrees = 1; }
        if (stage >= 3) f.ReleasePlanned = true;
        if (stage >= 4) { f.ReleaseExecuted = true; f.DeadwoodExecuted = true; }
        if (stage >= 5) { f.ReleaseReviewed = true; f.ReviewedTwoCycles = true; }
        if (stage >= 6) { f.BeechEstablished = true; f.OakEstablished = true; f.PlantingYear = 3; f.Year = 3; }
        if (stage >= 7) { f.Year = 5; f.EstablishmentReviewed = true; }
        facts.SetValue(learning, f);
        next.SetValue(learning, float.MaxValue);
        learning.Refresh(false);
        Check(learning.CurrentTopicIndex == stage, $"forced stage {stage}", "got " + learning.CurrentTopicIndex);
    }

    // ---- audits ----
    // Every visible label's size in physical pixels. The smallest text on a screen is what a player complains about.
    private void AuditText(string state, VisualElement root, float hardMinUiUnits = 14.5f)
    {
        float scale = UiScale();
        var sizes = new List<(float px, string text, float ui)>();
        foreach (TextElement label in root.Query<TextElement>().ToList())
        {
            if (!Visible(label) || string.IsNullOrWhiteSpace(label.text) || label is Button) continue;
            float uiSize = label.resolvedStyle.fontSize;
            sizes.Add((uiSize * scale, label.text.Length > 40 ? label.text.Substring(0, 40) : label.text, uiSize));
        }
        if (sizes.Count == 0) { Fail("TEXT_" + state, "no visible text"); return; }
        var ordered = sizes.OrderBy(s => s.px).ToList();
        float median = ordered[ordered.Count / 2].px;
        int under12 = ordered.Count(s => s.px < 12f);
        textRows.Add($"{state},{size.x}x{size.y},{scale.ToString("0.000", Inv)},{ordered.Count},{ordered[0].px.ToString("0.0", Inv)},{median.ToString("0.0", Inv)},{under12},\"{ordered[0].text.Replace("\"", "'")}\"");
        Check(ordered[0].ui >= hardMinUiUnits, $"TEXT_NOT_TINY_{state}", $"smallest={ordered[0].ui:0.0} UI units = {ordered[0].px:0.0}px \"{ordered[0].text}\" median={median:0.0}px labels={ordered.Count}");
        // Bigger type must not squeeze rows together or cut a one-line cell off at its edge, on any screen this audit looks at.
        AuditNotSquashed("TEXT_ROWS_NOT_SQUASHED_" + state, root);
        AuditNotCutOff("TEXT_NOT_CUT_OFF_" + state, root);
    }

    // A row squeezed below its text height makes the lines print on top of each other (what a panel does when it is capped in height and its
    // content is taller). Compares every visible label's box with the height its own text needs; a squashed one is reported with its words.
    private void AuditNotSquashed(string name, VisualElement root)
    {
        if (root == null) { Fail(name, "missing element"); return; }
        var squashed = new List<string>();
        int labels = 0;
        foreach (TextElement label in root.Query<TextElement>().ToList())
        {
            if (!Visible(label) || string.IsNullOrWhiteSpace(label.text) || label is Button) continue;
            labels++;
            bool wraps = label.resolvedStyle.whiteSpace == WhiteSpace.Normal;
            Vector2 natural = label.MeasureTextSize(label.text, wraps ? label.worldBound.width : 0f, wraps ? VisualElement.MeasureMode.Exactly : VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
            if (natural.y > label.worldBound.height + 2f)
                squashed.Add($"\"{(label.text.Length > 28 ? label.text.Substring(0, 28) : label.text)}\" needs {natural.y:0} has {label.worldBound.height:0}");
        }
        Debug.Log($"UXP_SQUASH {name} {size.x}x{size.y} labels={labels} squashed={squashed.Count}");
        Check(squashed.Count == 0, name, squashed.Count == 0 ? $"labels={labels}" : string.Join(" | ", squashed.Take(4)));
    }

    // One-line cells must show all their text: a label whose text is wider than its box is cut off at the edge ("Moderat").
    private void AuditNotCutOff(string name, VisualElement root)
    {
        if (root == null) { Fail(name, "missing element"); return; }
        var cut = new List<string>();
        int labels = 0;
        foreach (TextElement label in root.Query<TextElement>().ToList())
        {
            if (!Visible(label) || string.IsNullOrWhiteSpace(label.text) || label is Button || label.resolvedStyle.whiteSpace != WhiteSpace.NoWrap) continue;
            labels++;
            Vector2 natural = label.MeasureTextSize(label.text, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
            if (natural.x > label.worldBound.width + 0.5f) cut.Add($"\"{label.text}\" text={natural.x:0} box={label.worldBound.width:0}");
        }
        Debug.Log($"UXP_CUTOFF {name} {size.x}x{size.y} oneLineLabels={labels} cut={cut.Count}");
        Check(cut.Count == 0, name, cut.Count == 0 ? $"oneLineLabels={labels}" : string.Join(" | ", cut.Take(4)));
    }

    private void AuditContained(string name, VisualElement child, VisualElement bound)
    {
        if (child == null || bound == null) { Fail(name, "missing element"); return; }
        Rect c = child.worldBound, b = bound.worldBound;
        bool inside = Visible(child) && c.xMin >= b.xMin - 1f && c.xMax <= b.xMax + 1f && c.yMin >= b.yMin - 1f && c.yMax <= b.yMax + 1f;
        Check(inside, name, $"child={c} bound={b}");
    }

    // The picture keeps its proportions, its words sit inside it, no two words overlap and no legend entry is wider than its box.
    private void AuditDiagram(string state, VisualElement diagram, bool intro)
    {
        if (diagram == null) { Fail("DIAGRAM_PRESENT_" + state, "missing"); return; }
        VisualElement frame = diagram.Q("teaching-diagram-frame");
        Check(frame != null && Visible(frame), "DIAGRAM_FRAME_VISIBLE_" + state);
        if (frame == null) return;
        float w = frame.worldBound.width, h = frame.worldBound.height;
        float want = intro ? TeachingDiagramPlan.IntroWidth / TeachingDiagramPlan.IntroHeight : TeachingDiagramPlan.LessonWidth / TeachingDiagramPlan.LessonHeight;
        Check(Mathf.Abs(w / h - want) < 0.03f, "DIAGRAM_KEEPS_PROPORTIONS_" + state, $"{w:0}x{h:0} ratio={w / h:0.000} want={want:0.000}");
        Check(w >= 560f, "DIAGRAM_IS_LARGE_ENOUGH_" + state, $"{w:0} x {h:0} UI units (was 660 max x 156)");
        var labels = frame.Query<Label>().ToList().Where(Visible).ToList();
        foreach (Label label in labels)
        {
            Rect r = label.worldBound;
            Check(r.xMin >= frame.worldBound.xMin - 1f && r.xMax <= frame.worldBound.xMax + 1f && r.yMax <= frame.worldBound.yMax + 1f, $"DIAGRAM_WORD_INSIDE_{state}", $"\"{label.text}\" {r} frame={frame.worldBound}");
            Vector2 natural = label.MeasureTextSize(label.text, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
            if (label.ClassListContains("diagram-legend"))
                Check(natural.x <= r.width + 0.5f, $"DIAGRAM_LEGEND_FITS_{state}", $"\"{label.text}\" text={natural.x:0} box={r.width:0}");
        }
        for (int i = 0; i < labels.Count; i++)
            for (int j = i + 1; j < labels.Count; j++)
            {
                Rect a = labels[i].worldBound, b = labels[j].worldBound;
                bool overlap = a.xMin < b.xMax - 1f && b.xMin < a.xMax - 1f && a.yMin < b.yMax - 1f && b.yMin < a.yMax - 1f;
                if (overlap) Fail($"DIAGRAM_WORDS_DO_NOT_OVERLAP_{state}", $"\"{labels[i].text}\" vs \"{labels[j].text}\"");
            }
    }

    private void AuditHelp(string state)
    {
        VisualElement window = ui.Help.Root.Q("menu-help-window");
        Button close = ui.Help.Root.Q<Button>("menu-help-close");
        Check(window != null && Visible(window), "HELP_WINDOW_VISIBLE_" + state);
        if (window == null) return;
        AuditContained("HELP_WINDOW_ON_SCREEN_" + state, window, ui.RootElement);
        Check(close != null && Visible(close) && close.worldBound.yMax <= window.worldBound.yMax + 1f, "HELP_CLOSE_BUTTON_REACHABLE_" + state);
        AuditText("help-" + state, window);
        Debug.Log($"UXP_HELPWINDOW {state} {size.x}x{size.y} window={window.worldBound.width:0}x{window.worldBound.height:0} UI units");
        // Nothing the player must read may be hidden below the fold: the body fits its viewport (no vertical scrolling needed).
        ScrollView body = ui.Help.Root.Q<ScrollView>();
        if (body != null && state == "intro")
        {
            float content = body.contentContainer.layout.height, viewport = body.contentViewport.layout.height;
            Check(content <= viewport + 1f, "INTRO_FITS_WITHOUT_SCROLLING_" + state, $"content={content:0} viewport={viewport:0} UI units");
        }
    }

    // ---- sections ----
    private IEnumerator Intro()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        ui.Help.ShowScenarioIntroduction();
        yield return Pause(14);
        Check(ui.Help.IsOpen && Texts(ui.Help.Root).Contains("Transforming a Sitka Plantation"), "INTRO_OPENS_WITH_ITS_TITLE");
        string text = Texts(ui.Help.Root);
        Check(!text.ToLowerInvariant().Contains("not a promised outcome") && !text.ToLowerInvariant().Contains("schematic"), "INTRO_HAS_NO_DISCLAIMER_LINE");
        foreach (string want in new[] { "What this forest is", "What you are trying to do", "Why it takes many decisions", "access racks" })
            Check(text.Contains(want), "INTRO_EXPLAINS_" + want.Replace(' ', '_'));
        AuditDiagram("intro", ui.Help.Root.Q("teaching-diagram"), true);
        AuditHelp("intro");
        ScrollView introScroll = ui.Help.Root.Q<ScrollView>();
        foreach (VisualElement part in ui.Help.Root.Query(className: "intro-block").ToList().Cast<VisualElement>().Concat(ui.Help.Root.Query(className: "intro-start").ToList()))
            Check(introScroll != null && part.worldBound.yMax <= introScroll.contentViewport.worldBound.yMax + 1f && part.worldBound.yMin >= introScroll.contentViewport.worldBound.yMin - 1f,
                "INTRO_PART_NOT_CLIPPED", $"{(part.ClassListContains("intro-block") ? "block" : "start")} y={part.worldBound.yMin:0}-{part.worldBound.yMax:0} viewport={introScroll?.contentViewport.worldBound.yMin:0}-{introScroll?.contentViewport.worldBound.yMax:0}");
        yield return Shot("01-intro");
        Button more = ui.Help.Root.Q<Button>("menu-help-more");
        string before = Texts(ui.Help.Root);
        yield return Invoke(more);
        Check(ui.Help.IsOpen && Texts(ui.Help.Root).Length > before.Length, "MORE_DETAIL_OPENS_THE_FULL_TEXT");
        yield return Pause(6);
        AuditHelp("intro-more-detail");
        yield return Shot("02-intro-more-detail");
        yield return Invoke(more);
        Check(Texts(ui.Help.Root) == before, "LESS_DETAIL_RETURNS_TO_THE_INTRO");
        ui.CloseHelp();
        yield return Pause(6);
    }

    private IEnumerator HelpWindows()
    {
        // Each contextual help window, first-use short text and then "More detail", as a player meets them.
        foreach (var item in new[] { (MenuHelpView.Menu.WorkPlan, "workplan"), (MenuHelpView.Menu.StandMap, "map") })
        {
            ui.CloseAll(); ui.CloseHelp();
            PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + item.Item1);
            if (item.Item1 == MenuHelpView.Menu.WorkPlan) ui.ShowWorkPlan(); else ui.ShowMap();
            yield return Pause(16);
            if (!ui.Help.IsOpen) { Fail("HELP_OPENS_" + item.Item2, "first-use help did not open"); continue; }
            AuditHelp(item.Item2 + "-brief");
            yield return Shot("03-help-" + item.Item2 + "-brief");
            yield return Invoke(ui.Help.Root.Q<Button>("menu-help-more"));
            yield return Pause(6);
            AuditHelp(item.Item2 + "-more-detail");
            yield return Shot("03-help-" + item.Item2 + "-more-detail");
            ui.CloseHelp(); ui.CloseAll();
            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + item.Item1, 1);
            yield return Pause(6);
        }
    }

    private IEnumerator Hud()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        yield return Pause(20);
        VisualElement widget = ui.RootElement.Q("active-objective-widget");
        Check(widget != null && Visible(widget), "HUD_OBJECTIVE_VISIBLE_AFTER_THE_INTRO");
        if (widget != null)
        {
            AuditContained("HUD_OBJECTIVE_ON_SCREEN", widget, ui.RootElement);
            Check(widget.Q<Label>("active-objective-now")?.text == "NOW", "HUD_OBJECTIVE_SHOWS_THE_NOW_STATE");
            string title = widget.Q<Label>("active-objective-title")?.text ?? "", instruction = widget.Q<Label>("active-objective-instruction")?.text ?? "";
            Check(title.Contains("access racks"), "HUD_OBJECTIVE_NAMES_THE_CURRENT_OBJECTIVE", title);
            Check(instruction.Length > 10, "HUD_OBJECTIVE_HAS_A_SHORT_INSTRUCTION", instruction);
            Check(widget.Q<Button>("active-objective-details") != null, "HUD_OBJECTIVE_OFFERS_THE_DETAIL_SHORTCUT");
            AuditText("hud-objective", widget);
            // It is independent of the minimap: it needs only its model, and nothing named minimap is part of it.
            Check(widget.Query<VisualElement>().ToList().All(e => !(e.name ?? "").Contains("minimap")), "HUD_OBJECTIVE_IS_INDEPENDENT_OF_THE_MINIMAP");
        }
        yield return Shot("04-hud-after-intro");
    }

    private IEnumerator Objectives()
    {
        for (int stage = 0; stage < 8; stage++)
        {
            // ShowObjectives refreshes the view with the REAL facts, so open it first and inject the stage afterwards.
            ui.CloseAll(); ui.CloseHelp();
            ui.ShowObjectives();
            ForceStage(stage);
            yield return Pause(16);
            // The view restores its previous scroll position when it rebuilds; start every stage at the top, as a player opening it does.
            ui.Learning.Root.Q<ScrollView>().scrollOffset = Vector2.zero;
            yield return Pause(4);
            string state = "objective-" + (stage + 1);
            VisualElement card = ui.Learning.Root.Q("current-objective");
            Check(card != null && Visible(card), "OBJECTIVE_CARD_VISIBLE_" + state);
            if (card == null) continue;
            var names = new List<string> { "current-objective-title", "current-objective-concept", "current-objective-instruction", "current-objective-progress", "teaching-diagram" };
            if (stage == 0) names.Add("current-objective-practice");
            foreach (string name in names) AuditContained($"OBJECTIVE_PART_INSIDE_CARD_{state}_{name}", card.Q(name), card);
            // The rules the guided onboarding gate holds the card to, restated here so this harness fails on them too: the whole card is on
            // screen (with its picture) and its parts stack in the order title, why, what to do, progress without overlapping.
            AuditContained($"OBJECTIVE_CARD_FITS_THE_SCREEN_{state}", card, ui.RootElement);
            AuditContained($"OBJECTIVE_CARD_NEEDS_NO_SCROLLING_{state}", card, ui.Learning.Root.Q<ScrollView>().contentViewport);
            string[] stack = { "current-objective-title", "current-objective-concept", "current-objective-instruction", "current-objective-progress" };
            for (int i = 1; i < stack.Length; i++)
            {
                VisualElement above = card.Q(stack[i - 1]), below = card.Q(stack[i]);
                Check(above != null && below != null && above.worldBound.yMax <= below.worldBound.yMin + 1f, $"OBJECTIVE_PARTS_DO_NOT_OVERLAP_{state}_{stack[i]}");
            }
            // A label that does not wrap must fit its box: a long progress heading in the narrower words column would otherwise run under the
            // picture beside it, where no containment check sees it (found by reading the screenshots).
            foreach (Label label in card.Query<Label>().ToList().Where(Visible))
            {
                if (label.resolvedStyle.whiteSpace != WhiteSpace.NoWrap) continue;
                Vector2 natural = label.MeasureTextSize(label.text, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
                Check(natural.x <= label.worldBound.width + 1f, $"OBJECTIVE_NOWRAP_TEXT_FITS_{state}", $"\"{label.text}\" text={natural.x:0} box={label.worldBound.width:0}");
            }
            if (stage == 0)
            {
                Check(Visible(ui.RootElement.Q("rack-lesson-diagram")), "OBJECTIVE_1_RACK_DIAGRAM_VISIBLE");
                string cardText = Texts(card);
                foreach (string want in new[] { "What to do", "Why it matters", "What good practice looks like" })
                    Check(cardText.Contains(want), "OBJECTIVE_1_EXPLAINS_" + want.Replace(' ', '_'));
            }
            AuditDiagram(state, card.Q("teaching-diagram"), false);
            AuditText(state, ui.Learning.Root);
            ScrollView scroll = ui.Learning.Root.Q<ScrollView>();
            scroll.scrollOffset = Vector2.zero;
            yield return Pause(4);
            yield return Shot($"05-objective-{stage + 1}-top");
            scroll.scrollOffset = new Vector2(0f, scroll.contentContainer.layout.height);
            yield return Pause(6);
            yield return Shot($"05-objective-{stage + 1}-picture");
        }
        ui.CloseAll(); ForceStage(0);
    }

    private IEnumerator WorkPlan()
    {
        Check(!manager.Inventory.Any(e => e.quantity > 0), "FRESH_GAME_OWNS_NO_NURSERY_STOCK (so Planting really starts locked)");
        foreach (int stage in new[] { 0, 5, 7 })
        {
            ui.CloseAll(); ui.CloseHelp(); ForceStage(stage);
            ui.ShowWorkPlan();
            yield return Pause(18);
            var locked = ui.RootElement.Query(className: "card-locked").ToList().Where(Visible).ToList();
            int expected = stage == 0 ? 3 : stage == 5 ? 1 : 0;
            Check(locked.Count == expected, $"WORKPLAN_LOCKED_SECTIONS_AT_STAGE_{stage}", $"locked={locked.Count} expected={expected} [{string.Join(" | ", locked.Select(l => l.Q<Label>(className: "heading")?.text))}]");
            Check(locked.All(l => !l.enabledInHierarchy), $"WORKPLAN_LOCKED_SECTIONS_ARE_DISABLED_{stage}");
            Check(locked.All(l => Texts(l).Contains(ScenarioOneWorkPlanFocus.LaterObjectiveNote)), $"WORKPLAN_LOCKED_SECTIONS_EXPLAIN_{stage}");
            string all = Texts(ui.RootElement);
            if (stage == 0) Check(all.Contains("Available in a later objective."), "WORKPLAN_EXPLAINS_WHY_PLANTING_IS_UNAVAILABLE");
            AuditText($"workplan-stage-{stage}", ui.RootElement.Q("work-plan") ?? ui.RootElement);
            yield return Shot($"06-workplan-stage-{stage}");
        }
        ui.CloseAll(); ForceStage(0);
    }

    // ---- selected tree ----
    private sealed class Pose { public string Name; public ForestTree Tree; public Vector3 Camera; public Vector3 LookAt; public ForestTree Mark1, Mark2; }

    private IEnumerator Selection()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        List<ForestTree> trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop).ToList();
        Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);
        Vector3 centre = trees.Aggregate(Vector3.zero, (s, t) => s + Flat(t.transform.position)) / trees.Count;
        int Neighbours(ForestTree t) => trees.Count(o => o != t && Vector3.Distance(Flat(o.transform.position), Flat(t.transform.position)) < 4f);
        // Dense interior: a well-surrounded tree near the middle, seen from a few metres back with its neighbours all around it.
        ForestTree dense = trees.Where(t => Neighbours(t) >= 6).OrderBy(t => Vector3.Distance(Flat(t.transform.position), centre)).First();
        // Edge: the tree furthest east, seen from the inside looking outward and slightly up, so the open ground and sky are behind it.
        ForestTree edge = trees.OrderByDescending(t => t.transform.position.x).First();
        // Overlap: another tree stands between the camera and the selected one.
        ForestTree overlapTarget = trees.Where(t => Neighbours(t) >= 4).OrderBy(t => Vector3.Distance(Flat(t.transform.position), centre + new Vector3(8f, 0f, 6f))).First();
        ForestTree blocker = trees.Where(t => t != overlapTarget && Vector3.Distance(Flat(t.transform.position), Flat(overlapTarget.transform.position)) is > 1.8f and < 3.2f)
            .OrderBy(t => Vector3.Distance(Flat(t.transform.position), Flat(overlapTarget.transform.position))).First();
        Vector3 along = Flat(overlapTarget.transform.position - blocker.transform.position).normalized;
        // Close, with a Crop Tree and a Fell mark on neighbours, to tell selection from management marks.
        ForestTree close = dense;
        ForestTree nearest1 = trees.Where(t => t != close).OrderBy(t => Vector3.Distance(Flat(t.transform.position), Flat(close.transform.position))).Skip(0).First();
        ForestTree nearest2 = trees.Where(t => t != close && t != nearest1).OrderBy(t => Vector3.Distance(Flat(t.transform.position), Flat(close.transform.position))).First();
        var poses = new List<Pose>
        {
            new Pose { Name = "dense-stand", Tree = dense, Camera = dense.transform.position + new Vector3(0f, 0f, -3.2f), LookAt = dense.transform.position + Vector3.up * 1.5f },
            new Pose { Name = "edge-bright", Tree = edge, Camera = edge.transform.position + new Vector3(-3.2f, 0f, 0f), LookAt = edge.transform.position + Vector3.up * 3.2f },
            new Pose { Name = "overlapping", Tree = overlapTarget, Camera = blocker.transform.position - along * 1.4f, LookAt = overlapTarget.transform.position + Vector3.up * 1.5f },
            new Pose { Name = "close-with-management-marks", Tree = close, Camera = close.transform.position + new Vector3(2.4f, 0f, -2.4f), LookAt = close.transform.position + Vector3.up * 1.3f, Mark1 = nearest1, Mark2 = nearest2 },
        };
        foreach (Pose pose in poses) yield return PoseShots(pose);
        // Restore: clear marks, close inspection.
        foreach (ForestTree t in new[] { nearest1, nearest2 }) ui.Marking.Mark(t, TreeMarkType.None, false);
        typeof(ForestPlayer).GetField("isInspecting", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, false);
    }

    private IEnumerator PoseShots(Pose pose)
    {
        MethodInfo inspect = typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo inspecting = typeof(ForestPlayer).GetField("isInspecting", BindingFlags.Instance | BindingFlags.NonPublic);
        float ground = Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(pose.Camera) + Terrain.activeTerrain.transform.position.y : pose.Camera.y;
        player.transform.position = new Vector3(pose.Camera.x, ground + 0.1f, pose.Camera.z);
        player.LookToward(pose.LookAt);
        if (pose.Mark1 != null) ui.Marking.Mark(pose.Mark1, TreeMarkType.CropTree, false);
        if (pose.Mark2 != null) ui.Marking.Mark(pose.Mark2, TreeMarkType.Fell, false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        inspecting.SetValue(player, false);
        yield return Pause(10);
        // Baseline: the same view with nothing selected.
        yield return Capture();
        Color32[] before = lastCapture.GetPixels32(); int width = lastCapture.width, height = lastCapture.height;
        // Selected: the real indicator and card, the way a player sees them.
        for (int i = 0; i < 10; i++)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            player.LookToward(pose.LookAt);
            inspect.Invoke(player, new object[] { pose.Tree });
            yield return null;
        }
        Check(player.InspectedTree == pose.Tree, "SELECTED_TREE_IS_INSPECTED_" + pose.Name, $"distance={Vector3.Distance(Camera.main.transform.position, pose.Tree.InteractionPoint):0.0} m");
        Check(ui.Indicator != null && ui.Indicator.Visible, "SELECTION_INDICATOR_SHOWN_" + pose.Name);
        yield return Shot("07-selected-" + pose.Name);
        Color32[] after = lastCapture.GetPixels32();
        // Where the tree is on screen, and the screen rectangles of every panel, which are excluded: only the world is compared.
        Camera camera = Camera.main;
        Vector3 at = camera.WorldToScreenPoint(pose.Tree.transform.position + Vector3.up * 1.3f);
        float scale = UiScale();
        var panels = ui.RootElement.Query<VisualElement>().ToList().Where(e => (e.ClassListContains("panel") || e.ClassListContains("inspect") || e.ClassListContains("modal")) && Visible(e))
            .Select(e => new Rect(e.worldBound.xMin * scale, e.worldBound.yMin * scale, e.worldBound.width * scale, e.worldBound.height * scale)).ToList();
        float radius = height * 0.22f;
        int newWhite = 0, newOchre = 0, regionPixels = 0;
        for (int y = Mathf.Max(0, (int)(at.y - radius)); y < Mathf.Min(height, (int)(at.y + radius)); y++)
            for (int x = Mathf.Max(0, (int)(at.x - radius)); x < Mathf.Min(width, (int)(at.x + radius)); x++)
            {
                if (((x - at.x) * (x - at.x) + (y - at.y) * (y - at.y)) > radius * radius) continue;
                if (panels.Any(p => p.Contains(new Vector2(x, height - 1 - y)))) continue;
                regionPixels++;
                Color32 b = before[y * width + x], a = after[y * width + x];
                if (Mathf.Min(a.r, Mathf.Min(a.g, a.b)) >= 235 && Mathf.Min(b.r, Mathf.Min(b.g, b.b)) < 235) newWhite++;
                bool Ochre(Color32 c) => c.r > 150 && c.g > 100 && c.g < 200 && c.b < 110 && c.r > c.b + 70;
                if (Ochre(a) && !Ochre(b)) newOchre++;
            }
        Debug.Log($"UXP_SELECTION_DIFF {pose.Name} {size.x}x{size.y} region={regionPixels}px newWhite={newWhite} newOchre={newOchre} treeAtScreen=({at.x:0},{at.y:0}) z={at.z:0.0}");
        Check(at.z > 0f && regionPixels > 2000, "SELECTION_REGION_IS_ON_SCREEN_" + pose.Name, $"pixels={regionPixels}");
        Check(newWhite <= 6, "SELECTION_ADDS_NO_WHITE_" + pose.Name, $"newWhitePixels={newWhite}");
        Check(newOchre >= 60, "SELECTION_IS_VISIBLE_" + pose.Name, $"newOchrePixels={newOchre}");
        if (pose.Mark1 != null)
        {
            // Competitor ring: a Crop Tree lists the neighbours that compete with it, and the selected competitor takes the same ochre, never white.
            // So the selected tree itself becomes a Crop Tree here (a tree with no mark lists no competitors, which would leave this check empty).
            ui.Marking.Mark(pose.Tree, TreeMarkType.CropTree, false);
            for (int i = 0; i < 12; i++)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                player.LookToward(pose.LookAt);
                inspect.Invoke(player, new object[] { pose.Tree });
                yield return null;
            }
            Check(ui.Competitors.ListedCount > 0, "CROP_TREE_LISTS_COMPETITORS", $"listed={ui.Competitors.ListedCount}");
            PropertyInfo selected = typeof(CompetitorAssessment).GetProperty("SelectedIndex");
            if (ui.Competitors.ListedCount > 0 && selected != null) selected.GetSetMethod(true).Invoke(ui.Competitors, new object[] { 0 });
            yield return Pause(6);
            // Numbered tags are drawn in the world layer: none may sit over a panel's text (the HUD column, the card or the competitor list).
            var shownTags = ui.RootElement.Query<Label>(className: "competitor-tag").ToList().Where(Visible).ToList();
            var coveredPanels = ui.RootElement.Query<VisualElement>().ToList().Where(e => (e.ClassListContains("panel") || e.ClassListContains("inspect")) && Visible(e)).ToList();
            int coveredTags = shownTags.Count(t => coveredPanels.Any(p => p.worldBound.Contains(t.worldBound.center)));
            Debug.Log($"UXP_COMPETITOR_TAGS {size.x}x{size.y} shown={shownTags.Count} overPanels={coveredTags}");
            Check(shownTags.Count > 0, "COMPETITOR_TAGS_ARE_SHOWN", "the check below is empty without tags");
            Check(coveredTags == 0, "COMPETITOR_TAGS_DO_NOT_COVER_PANELS", $"tagsOverPanels={coveredTags}");
            // The competitor list is longest with a Fell mark and other Crop Trees near: its rows must keep their height, and the HUD column above it too.
            AuditNotSquashed("COMPETITOR_LIST_ROWS_ARE_NOT_SQUASHED", ui.RootElement.Q("competitor-panel"));
            // ...and, because they keep their height, the whole list must still fit under the cap: nothing cut off at the bottom.
            VisualElement competitorPanel = ui.RootElement.Q("competitor-panel");
            float lastLabelBottom = competitorPanel.Query<TextElement>().ToList().Where(Visible).Max(l => l.worldBound.yMax);
            Check(lastLabelBottom <= competitorPanel.worldBound.yMax + 1f, "COMPETITOR_LIST_IS_NOT_CLIPPED", $"lastLabelBottom={lastLabelBottom:0} panelBottom={competitorPanel.worldBound.yMax:0}");
            AuditNotCutOff("COMPETITOR_LIST_TEXT_IS_NOT_CUT_OFF", ui.RootElement.Q("competitor-panel"));
            AuditNotSquashed("HUD_COLUMN_ROWS_ARE_NOT_SQUASHED", ui.RootElement.Q("hud-right-column"));
            AuditNotSquashed("INSPECTION_CARD_ROWS_ARE_NOT_SQUASHED", ui.RootElement.Q(className: "inspect"));
            yield return Shot("07-selected-competitor-ring");
            ui.Marking.Mark(pose.Tree, TreeMarkType.None, false);
            ui.Marking.Mark(pose.Mark1, TreeMarkType.None, false);
            ui.Marking.Mark(pose.Mark2, TreeMarkType.None, false);
        }
        inspecting.SetValue(player, false);
        yield return Pause(6);
    }
}
#endif
