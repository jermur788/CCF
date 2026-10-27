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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

// Disposable RENDERED qualification for the Scenario One playtest UX revision (Help, tree inspection, Work Plan, Annual Review,
// Stand Map). The BEFORE screenshots were taken with the first revision of this file against the pinned candidate 8942b5c3; the
// AFTER run adds the checks that need the revision's own members (inspected-tree cage, [V] detail). Every screen is captured at
// each size in CCF_UI_SIZES (default "1920x1080,1280x720,1024x768,1680x1050"), and the harness measures instead of eyeballing:
//   * PICK: every visible, unclipped Button must be the element UI Toolkit's own hit test returns at the Button's visual centre
//     (a button under an open modal's backdrop is "covered", which is correct, and is counted separately);
//   * OVERLAP: which visible HUD panels intersect each other (an inspection card must not sit on another panel);
//   * TEXT: no visible text uses the implementation words "cell" / "mean light" / "regenerating cells";
//   * MORE_DETAIL: the first-use screens' "More detail" button changes the screen and can be closed again.
// Limits: Editor-synthetic mouse events do not reach the runtime panel, so a click invokes the button's registered callback;
// real pointer alignment is checked separately against a Player with xdotool (see the handoff).
//
// Run (graphical Editor):  -executeMethod RackUxShots.Begin    CCF_UI_SIZES=...  CCF_DIAG_DIR=<folder>  CCF_RACKUX_TAG=before|after
public static class RackUxShots
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "RackUx " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

    public static Vector2Int[] Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        if (string.IsNullOrEmpty(text)) text = "1920x1080,1280x720,1024x768,1680x1050";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
    }

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(Sizes()[0].x, Sizes()[0].y);
        EditorApplication.isPlaying = true;
    }

    // A genuinely fresh player: no first-use screen seen, no lesson recorded. The Editor runs on an isolated config folder.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "RackUxShots.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "RackUxShots.Begin"))
            new GameObject("Rack UX shots").AddComponent<RackUxShotsRunner>();
    }
}

public sealed class RackUxShotsRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private ForestTreeMarkingManager marks;
    private string output, tag;
    private int shots, passed;
    private readonly List<string> failures = new List<string>();
    private readonly List<string> csv = new List<string>();

    private static string F(double v, string f = "0.##") => v.ToString(f, Inv);
    private void Hard(string name, bool ok, string detail)
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"RACKUX_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Measure(string name, string detail) => Debug.Log($"RACKUX_MEASURE {name} {detail}");

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"RACKUX_SHOTS_DONE shots={shots} hardChecks={passed}" : $"RACKUX_SHOTS_FAILED shots={shots} hardChecks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
        Debug.Log("RACKUX_SHOTS_DONE_MARKER");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    // ---------- helpers ----------
    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private IEnumerator Invoke(Button button)
    {
        if (button == null) throw new InvalidOperationException("button not found");
        MethodInfo invoke = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        invoke.Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }

    private static bool Visible(VisualElement e)
    {
        for (VisualElement p = e; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity <= 0.01f) return false;
        return e.worldBound.width > 0 && e.worldBound.height > 0;
    }

    private VisualElement UiRoot() => ui.RootElement;
    private Button BtnStarts(VisualElement root, params string[] texts) => root == null ? null
        : root.Query<Button>().ToList().FirstOrDefault(b => b.text != null && texts.Any(t => b.text.StartsWith(t, StringComparison.Ordinal)) && Visible(b));
    private static string Texts(VisualElement root)
        => root == null ? "" : string.Join("\n", root.Query<TextElement>().ToList().Where(Visible).Select(t => t.text).Where(s => !string.IsNullOrEmpty(s)));

    private ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce" && !t.TreeId.StartsWith("Q", StringComparison.Ordinal))
        .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    private IEnumerator AimAt(Func<ForestTree, bool> accept, Action<ForestTree> found)
    {
        foreach (ForestTree tree in Trees().Where(accept).Take(40))
        {
            player.transform.position = tree.transform.position - new Vector3(0, 0, 3);
            for (int i = 0; i < 8; i++) yield return null;
            if (marks.AimedTree == tree) { found(tree); yield break; }
        }
        found(null);
    }

    // True when the element's centre lies outside a scroll view that contains it (a card scrolled out of sight is not on screen).
    private static bool Clipped(VisualElement e)
    {
        Vector2 c = e.worldBound.center;
        for (VisualElement p = e.parent; p != null; p = p.parent)
            if (p is ScrollView sv && !sv.contentViewport.worldBound.Contains(c)) return true;
        return false;
    }

    // A button under an open modal's backdrop is covered, not misaligned.
    private static bool IsBackdrop(VisualElement hit)
    {
        for (VisualElement p = hit; p != null; p = p.parent)
            if (p.ClassListContains("modal-backdrop")) return true;
        return false;
    }

    private static readonly System.Text.RegularExpressions.Regex Banned = new System.Text.RegularExpressions.Regex(
        @"\bcells?\b|mean light|regenerating cells", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    // Measurements that do not depend on how a picture looks: hit-test alignment of every button, HUD-panel overlap and wording.
    private void Audit(string state, Vector2Int size)
    {
        VisualElement root = UiRoot();
        IPanel panel = root.panel;
        int buttons = 0, covered = 0, misses = 0; var offenders = new List<string>();
        foreach (Button b in root.Query<Button>().ToList())
        {
            if (!Visible(b) || Clipped(b)) continue;
            buttons++;
            Rect r = b.worldBound;
            VisualElement hit = panel.Pick(r.center);
            bool ok = hit == b || (hit != null && b.Contains(hit));
            if (ok) continue;
            if (hit != null && IsBackdrop(hit) && !b.Contains(hit)) { covered++; continue; }
            misses++;
            if (offenders.Count < 4) offenders.Add($"[{b.text}] centre={r.center} hit={(hit == null ? "none" : hit.name + "." + string.Join(".", hit.GetClasses()))}");
        }
        csv.Add($"PICK,{state},{size.x}x{size.y},{buttons},{misses},covered={covered} \"{string.Join(" ; ", offenders)}\"");
        Hard($"PICK_{state}_{size.x}x{size.y}", misses == 0, $"visibleButtons={buttons} covered={covered} misses={misses} {string.Join(" ; ", offenders)}");

        // Wording: the screens a player reads must not use the grid's implementation words.
        string text = Texts(root);
        var bad = Banned.Matches(text).Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value.ToLowerInvariant()).Distinct().ToList();
        if (size.x == RackUxShots.Sizes()[0].x) Hard($"TEXT_{state}", bad.Count == 0, bad.Count == 0 ? "" : "found: " + string.Join(", ", bad));

        string[] classes = { "hud-status", "hud-ground", "hud-forecast", "hud-waypoint", "estimate-panel", "competitor-panel", "menu-help-panel" };
        var rects = new List<(string name, Rect rect)>();
        var seen = new HashSet<VisualElement>();
        foreach (string c in classes)
            foreach (VisualElement e in root.Query<VisualElement>(className: c).ToList())
                if (Visible(e) && seen.Add(e)) rects.Add((c, e.worldBound));
        foreach (string id in new[] { "inspect-card", "inspect-detail" })
        {
            VisualElement e = root.Q<VisualElement>(id);
            if (e != null && Visible(e)) rects.Add((id, e.worldBound));
        }
        // Text panels (the action prompt, the transient message, the marking summary) are the Labels themselves; their containers span the screen.
        foreach (Label l in root.Query<Label>(className: "panel").ToList()) if (Visible(l) && seen.Add(l)) rects.Add(("label-panel:" + Short(l.text), l.worldBound));
        var overlaps = new List<string>();
        for (int i = 0; i < rects.Count; i++)
            for (int j = i + 1; j < rects.Count; j++)
                if (rects[i].rect.Overlaps(rects[j].rect))
                {
                    Rect a = rects[i].rect, b = rects[j].rect;
                    float w = Mathf.Min(a.xMax, b.xMax) - Mathf.Max(a.xMin, b.xMin), h = Mathf.Min(a.yMax, b.yMax) - Mathf.Max(a.yMin, b.yMin);
                    if (w > 2f && h > 2f) overlaps.Add($"{rects[i].name}×{rects[j].name} {F(w, "0")}x{F(h, "0")}");
                }
        csv.Add($"OVERLAP,{state},{size.x}x{size.y},{rects.Count},{overlaps.Count},\"{string.Join(" ; ", overlaps)}\"");
        Measure("OVERLAP", $"{state} {size.x}x{size.y} panels={rects.Count} overlaps={overlaps.Count} {string.Join(" ; ", overlaps)}");
        if (state.Contains("inspect") && !state.Contains("intro"))
            Hard($"NO_PANEL_OVERLAP_{state}_{size.x}x{size.y}", overlaps.Count == 0, string.Join(" ; ", overlaps));
    }

    private static string Short(string s) => string.IsNullOrEmpty(s) ? "" : (s.Length > 18 ? s.Substring(0, 18) : s).Replace(",", " ");

    private IEnumerator Shoot(string state, Action everyFrame = null)
    {
        foreach (Vector2Int size in RackUxShots.Sizes())
        {
            RackUxShots.SetGameSize(size.x, size.y);
            for (int i = 0; i < 8; i++) { everyFrame?.Invoke(); yield return null; }
            everyFrame?.Invoke();
            yield return new WaitForEndOfFrame();
            Audit(state, size);
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + state);
            File.WriteAllBytes(Path.Combine(output, state + "-" + size.x + "x" + size.y + ".png"), image.EncodeToPNG());
            Destroy(image);
            shots++;
        }
    }

    private IEnumerator Settle(int frames = 3) { for (int i = 0; i < frames; i++) yield return null; }

    // More detail: a first-use screen's button opens the long text and can be closed again (back to the short text).
    private IEnumerator MoreDetailRoundTrip(string menuName)
    {
        Button more = ui.Help.Root.Q<Button>("menu-help-more");
        Hard($"MORE_DETAIL_IS_OFFERED_{menuName}", more != null && Visible(more) && more.text == MenuHelpView.MoreDetailLabel, more == null ? "no button" : $"text=[{more.text}]");
        if (more == null || !Visible(more)) yield break;
        string brief = Texts(ui.Help.Root);
        yield return Invoke(more);
        string full = Texts(ui.Help.Root);
        Hard($"MORE_DETAIL_OPENS_THE_FULL_TEXT_{menuName}", ui.Help.IsOpen && full.Length > brief.Length + 100 && more.text == MenuHelpView.LessDetailLabel, $"chars {brief.Length}→{full.Length} button=[{more.text}]");
        yield return Invoke(more);
        Hard($"LESS_DETAIL_RETURNS_TO_THE_SHORT_TEXT_{menuName}", ui.Help.IsOpen && Texts(ui.Help.Root) == brief && more.text == MenuHelpView.MoreDetailLabel, $"button=[{more.text}]");
    }

    // The centre of every button a pointer test needs, in SCREEN pixels at each size: the same layout a built Player draws at that
    // window size, so a real pointer can be aimed at the real control (see the handoff for the xdotool run).
    private IEnumerator PointerProbePhase()
    {
        var rows = new List<string> { "size,screen,control,x,y" };
        foreach (Vector2Int size in RackUxShots.Sizes())
        {
            RackUxShots.SetGameSize(size.x, size.y);
            for (int i = 0; i < 6; i++) yield return null;
            ui.CloseAll(); ui.CloseHelp(); yield return Settle(6);
            rows.AddRange(Probe("hud", size, null));
            PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + MenuHelpView.Menu.WalkingHud);
            ui.Help.Introduce(MenuHelpView.Menu.WalkingHud); yield return Settle(6);
            rows.AddRange(Probe("help-brief", size, null));
            ui.CloseHelp(); yield return Settle(4);
            ui.ShowWorkPlan(); yield return Settle(6); if (ui.Help.IsOpen) ui.CloseHelp(); yield return Settle(4);
            rows.AddRange(Probe("workplan", size, null));
            ui.ShowMap(); yield return Settle(6); if (ui.Help.IsOpen) ui.CloseHelp(); yield return Settle(4);
            rows.AddRange(Probe("map", size, new[] { "A1", "H10", "P16" }));
            ui.CloseAll(); yield return Settle(4);
        }
        File.WriteAllLines(Path.Combine(output, "pointer-targets.csv"), rows);
    }

    private IEnumerable<string> Probe(string screen, Vector2Int size, string[] cells)
    {
        VisualElement root = UiRoot();
        float scale = Screen.width / root.worldBound.width;
        foreach (Button b in root.Query<Button>().ToList())
        {
            if (!Visible(b) || Clipped(b)) continue;
            string label = b.text;
            if (string.IsNullOrEmpty(label) && b.ClassListContains("map-cell"))
            {
                string cell = b.Q<Label>()?.text ?? "";
                if (cells == null || !cells.Contains(cell)) continue;
                label = "cell " + cell;
            }
            if (string.IsNullOrEmpty(label)) continue;
            Vector2 c = b.worldBound.center * scale;
            yield return $"{size.x}x{size.y},{screen},\"{label.Replace("\"", "")}\",{F(c.x, "0.0")},{F(c.y, "0.0")}";
        }
    }

    // ---------- storyline ----------
    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; ecology = ui.Ecology; player = ui.Player; marks = ui.Marking;
        if (Application.isBatchMode) throw new InvalidOperationException("RackUxShots needs a graphical Editor");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        tag = Environment.GetEnvironmentVariable("CCF_RACKUX_TAG") ?? "run";
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/RackUx/evidence" : diag, tag);
        Directory.CreateDirectory(output);
        Measure("START", $"tag={tag} year={ecology.EcologicalYear} geometry={ecology.StandGeometryModelVersion} screen={Screen.width}x{Screen.height}");

        yield return HelpPhase();
        yield return InspectionPhase();
        yield return PlanAndReviewPhase();
        yield return MapPhase();
        yield return PointerProbePhase();
        File.WriteAllLines(Path.Combine(output, "audit.csv"), new[] { "kind,state,size,count,problems,detail" }.Concat(csv));
    }

    private IEnumerator HelpPhase()
    {
        Hard("HELP_FRESH_PLAYER_SEES_INTRO", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, $"open={ui.Help.IsOpen}");
        yield return Shoot("01-help-intro");
        Button more = ui.Help.Root.Q<Button>("menu-help-more");
        string before = Texts(ui.Help.Root);
        yield return Invoke(more);
        string after = Texts(ui.Help.Root);
        Hard("MORE_DETAIL_CHANGES_THE_SCREEN", ui.Help.IsOpen && after != before && after.Length > before.Length && more.text == MenuHelpView.LessDetailLabel, $"chars {before.Length}→{after.Length} button=[{more.text}]");
        Hard("HELP_HAS_NO_FIRST_USE_SENTENCE", !after.Contains("First-use introductions") && !before.Contains("First-use introductions"), "");
        yield return Shoot("02-help-full");
        yield return Invoke(more);
        Hard("LESS_DETAIL_GOES_BACK", ui.Help.IsOpen && Texts(ui.Help.Root) == before && more.text == MenuHelpView.MoreDetailLabel, $"button=[{more.text}]");
        yield return Invoke(more);
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        Hard("HELP_CLOSES", !ui.Help.IsOpen, "");
        yield return Settle(20);
    }

    private IEnumerator InspectionPhase()
    {
        ForestTree tree = null;
        yield return AimAt(t => true, t => tree = t);
        Hard("INSPECT_CAN_AIM_AT_A_TREE", tree != null, tree != null ? tree.TreeId : "none");
        if (tree == null) yield break;
        yield return Press(Key.E);
        for (int i = 0; i < 6; i++) yield return null;
        Hard("INSPECT_E_INSPECTS", player.IsInspecting && player.InspectedTree == tree, $"inspecting={player.IsInspecting}");
        if (ui.Help.IsOpen)
        {
            yield return Shoot("03-inspect-intro");
            yield return MoreDetailRoundTrip("TreeInspection");
            Button b = ui.Help.Root.Q<Button>("menu-help-more");
            if (b != null && Visible(b)) { yield return Invoke(b); yield return Shoot("03b-inspect-intro-detail"); }
            yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        }
        yield return Settle(10);
        Hard("CAGE_FOLLOWS_THE_INSPECTED_TREE", ui.Indicator.Visible && ui.Indicator.Target == tree, $"visible={ui.Indicator.Visible} target={(ui.Indicator.Target == null ? "none" : ui.Indicator.Target.TreeId)} inspected={tree.TreeId}");
        string card = Texts(UiRoot().Q<VisualElement>("inspect-card"));
        string estimate = Texts(UiRoot().Q<VisualElement>("inspect-estimate"));
        Hard("PRIMARY_CARD_HAS_NO_STEM_VOLUME", !card.Contains("Stem volume"), card.Replace("\n", " / "));
        Hard("PRIMARY_CARD_HAS_THE_DECISION_FACTS", card.Contains("Age") && card.Contains("DBH") && card.Contains("Height") && card.Contains("Growing space") && card.Contains("Sitka spruce"), card.Replace("\n", " / "));
        Hard("IF_FELLED_NOW_HAS_STEM_VOLUME", estimate.Contains("If felled now") && estimate.Contains("Stem volume"), estimate.Replace("\n", " / "));
        Hard("NO_YOU_CHOOSE_THE_TREES_LINE", !Texts(UiRoot()).Contains("You choose the trees"), "");
        yield return Shoot("04-inspect-card");
        // [V] opens and closes the detail card (a key, because the cursor is locked while inspecting).
        yield return Press(Key.V);
        yield return Settle(4);
        string detail = Texts(UiRoot().Q<VisualElement>("inspect-detail"));
        Hard("V_SHOWS_MORE_DETAIL", ui.Inspection.ShowsDetail && detail.Contains("Stem volume") && detail.Contains("Light at ground level"), detail.Replace("\n", " / "));
        yield return Shoot("04b-inspect-more-detail");
        yield return Press(Key.V);
        yield return Settle(4);
        Hard("V_AGAIN_CLOSES_IT", !ui.Inspection.ShowsDetail && UiRoot().Q<VisualElement>("inspect-detail").resolvedStyle.display == DisplayStyle.None, "");
        yield return Press(Key.C);
        yield return Settle(10);
        yield return Shoot("05-inspect-crop-tree");
        yield return Press(Key.E);
        yield return Settle(6);
        Hard("CAGE_HIDES_WHEN_INSPECTION_ENDS", !ui.Indicator.Visible, "");
    }

    private IEnumerator PlanAndReviewPhase()
    {
        // Mark two trees to fell (a Crop Tree already exists from the inspection above).
        var chosen = new List<ForestTree>();
        for (int n = 0; n < 2; n++)
        {
            ForestTree t = null;
            yield return AimAt(x => !x.IsCropTree && !x.IsMarkedForFell && !chosen.Contains(x), x => t = x);
            if (t == null) break;
            yield return Press(Key.X);
            chosen.Add(t);
        }
        Hard("PLAN_MARKED_TREES", chosen.Count == 2 && chosen.All(t => t.IsMarkedForFell), $"marked={chosen.Count}");
        yield return Shoot("06-walking-with-marks");
        yield return Press(Key.Tab);
        if (ui.Help.IsOpen)
        {
            yield return Shoot("07-workplan-intro");
            yield return MoreDetailRoundTrip("WorkPlan");
            yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        }
        yield return Settle(8);
        yield return Shoot("08-workplan");
        Button approve = BtnStarts(UiRoot(), "Approve Work Plan", "Approve pending work");
        yield return Invoke(approve);
        yield return Shoot("09-workplan-approved");
        int year = ecology.EcologicalYear;
        yield return Invoke(BtnStarts(UiRoot(), "Advance one year"));
        Hard("PLAN_ADVANCES_THE_YEAR", ecology.EcologicalYear == year + 1, $"year={ecology.EcologicalYear}");
        yield return Settle(8);
        Hard("REVIEW_OPENS", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review, $"screen={ui.CurrentScreen}");
        yield return Shoot("10-annual-review");
        Button ack = UiRoot().Q<Button>("annual-review-acknowledge");
        if (ack != null) yield return Invoke(ack);
        ui.CloseAll();
        yield return Settle(6);
    }

    private IEnumerator MapPhase()
    {
        ui.ShowMap();
        yield return Settle(10);
        if (ui.Help.IsOpen)
        {
            yield return Shoot("11-map-intro");
            yield return MoreDetailRoundTrip("StandMap");
            yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        }
        yield return Settle(6);
        // The map is modal: its dimmed backdrop is what a click anywhere outside the window lands on, and the HUD is not drawn.
        VisualElement mapRoot = ui.Map.Root;
        VisualElement atCorner = mapRoot.panel.Pick(mapRoot.worldBound.position + new Vector2(6, 6));
        Hard("MAP_BACKDROP_TAKES_THE_POINTER", atCorner == mapRoot, atCorner == null ? "none" : atCorner.name + "." + string.Join(".", atCorner.GetClasses()));
        VisualElement hudStatus = UiRoot().Q<VisualElement>(className: "hud-status");
        Hard("MAP_HIDES_THE_HUD", hudStatus == null || !Visible(hudStatus), "");
        yield return Shoot("12-stand-map");
        // Every layer tab, then the marks layer where the Fell/Crop counts are shown.
        foreach (string tab in new[] { "Regeneration", "Browsing / protection", "Fell & crop marks" })
        {
            Button t = BtnStarts(UiRoot(), tab);
            Hard("MAP_TAB_EXISTS_" + tab, t != null, "");
            if (t != null) yield return Invoke(t);
        }
        yield return Shoot("13-stand-map-marks");
        ui.CloseAll();
        yield return Settle(6);
    }
}
#endif
