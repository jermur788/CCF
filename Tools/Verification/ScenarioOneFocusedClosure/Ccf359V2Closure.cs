#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Profiling;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

// CCF #359 verification-only probe, session 1 (V2 world, graphical Editor). NEW code; it edits no game file and no earlier harness.
// It covers only behaviour no passing gate on 045966d2 covered:
//   J  onboarding default focus and keys, then F1 Help open/close and focus, driven by queued New Input System keyboard events
//      (synthetic Editor input, NOT an OS-native or Windows test);
//   B  the V2 headland surface: exactly one object, active only in a V2 world, deactivated by a model-0 load or a Reference Preview,
//      reactivated (same object, no duplicate) when the V2 world returns;
//   C  the 3D surface mesh, the Stand Map model and the Stand Map's own headland strokes describe the same place (the #358 witness
//      checked those strokes by count only), the feeder meets the forest path, and there is no northern spur;
//   G  the live Objective 1 five-rack figure: five planned corridors at the live suggested routes, six bands, text inside its frame;
//   H  (pre-V2 load and Reference Preview part) the figure and the V2 wording are absent in those worlds;
//   D+ planning-mode and confirmed-rack Mini/Full parity (one-to-one, against the manager's own rack endpoints);
//   I  Work Plan locks at forced lesson indexes 0 / 5 / 7 (forced facts, NOT real progression to stage 5 / 7);
//   F  frame time and GC allocation, MEASURED only (no baseline exists, so no numeric pass limit is asserted).
// Each section records its own failure and the run continues; any failure makes the run FAIL.
public static class Ccf359V2Closure
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Vector2Int first = Ccf359V2ClosureRunner.Sizes()[0];
        Ccf359V2ClosureRunner.SetGameSize(first.x, first.y);
        EditorApplication.isPlaying = true;
    }

    // First-use menu help is another harness's subject; mark it seen. The scenario onboarding still opens (it ignores these preferences).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "Ccf359V2Closure.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "Ccf359V2Closure.Begin"))
            new GameObject("CCF 359 V2 closure").AddComponent<Ccf359V2ClosureRunner>();
    }
}

public sealed class Ccf359V2ClosureRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string SurfaceName = "Scenario One V2 Headland";
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private string output, freshJson;
    private int passed, shots;
    private readonly List<string> failures = new List<string>(), evidence = new List<string>();
    private string section = "";
    private Vector2Int size;

    private static string F(double v) => v.ToString("0.#####", Inv);
    private void Check(string name, bool ok, string detail = "")
    {
        string full = section + "." + name;
        if (ok) passed++; else failures.Add(full + " " + detail);
        Debug.Log($"C359V2_{(ok ? "PASS" : "FAIL")} [{size.x}x{size.y}] {full} {detail}");
        evidence.Add($"{(ok ? "PASS" : "FAIL")} [{size.x}x{size.y}] {full} {detail}");
    }
    private void Measure(string name, string detail)
    {
        Debug.Log($"C359V2_MEASURE [{size.x}x{size.y}] {section}.{name} {detail}");
        evidence.Add($"MEASURE [{size.x}x{size.y}] {section}.{name} {detail}");
    }

    public static Vector2Int[] Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        if (string.IsNullOrEmpty(text)) text = "1920x1080,1280x720";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
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
        object gameSize = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "CCF359 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

    // ---------------------------------------------------------------- driver
    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        size = new Vector2Int(Screen.width, Screen.height);
        Exception fatal = null;
        try { Setup(); } catch (Exception e) { fatal = e; }
        if (fatal == null)
        {
            var sections = new List<(string name, Func<IEnumerator> body)>
            {
                ("J_onboarding", OnboardingKeysAndFocus),
                ("fresh", CaptureFresh),
                ("J_help", HelpCloseAndFocus),
                ("BC_surface_map_3d", SurfaceMapAnd3D),
                ("G_five_rack_figure", FiveRackFigure),
                ("I_workplan_forced_locks", WorkPlanForcedLocks),
                ("F_performance", Performance),
                ("D_planning_confirmed_parity", PlanningAndConfirmedParity),
                ("BH_transitions", Transitions),
            };
            foreach (var s in sections)
            {
                section = s.name;
                Exception error = null;
                var stack = new Stack<IEnumerator>(); stack.Push(s.body());
                while (stack.Count > 0)
                {
                    bool more; object current = null;
                    try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
                    catch (Exception e) { error = e; break; }
                    if (!more) { stack.Pop(); continue; }
                    if (current is IEnumerator nested) stack.Push(nested); else yield return current;
                }
                if (error != null) Check("SECTION_COMPLETED_WITHOUT_EXCEPTION", false, error.GetType().Name + ": " + error.Message.Replace('\n', ' '));
                else Check("SECTION_COMPLETED_WITHOUT_EXCEPTION", true);
                if (s.name == "J_onboarding" && error != null) break; // nothing later is meaningful if the world never left onboarding
            }
        }
        if (output != null) File.WriteAllLines(Path.Combine(output, "ccf359-v2-evidence.txt"), evidence);
        bool ok = fatal == null && failures.Count == 0;
        Debug.Log(ok ? $"CCF359_V2_CLOSURE_PASS shots={shots} checks={passed}"
            : $"CCF359_V2_CLOSURE_FAIL shots={shots} checks={passed} fatal={fatal} failures=[{string.Join(" | ", failures)}]");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    private void Setup()
    {
        if (Application.isBatchMode) throw new InvalidOperationException("this probe needs a graphical Editor");
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        if (ui == null || manager == null || saves == null) throw new InvalidOperationException("not the Scenario One scene");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/Ccf359" : diag, "ccf359-v2");
        Directory.CreateDirectory(output);
    }

    // ---------------------------------------------------------------- helpers
    private static IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }
    private static IEnumerator Wait(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }

    private IEnumerator Shot(string state)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + state);
        File.WriteAllBytes(Path.Combine(output, state + "-" + Screen.width + "x" + Screen.height + ".png"), image.EncodeToPNG());
        Destroy(image); shots++;
    }

    private static Keyboard Kb() => Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
    private static IEnumerator Press(Key key)
    {
        Keyboard keyboard = Kb();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private Focusable Focused() => ui.RootElement?.panel?.focusController?.focusedElement;
    private static string Describe(Focusable f) => f == null ? "none" : (f is VisualElement v ? v.GetType().Name + "#" + v.name + (f is Button b ? "'" + b.text.Trim() + "'" : "") : f.GetType().Name);
    private static bool Shown(VisualElement e)
    {
        for (VisualElement c = e; c != null; c = c.parent)
            if (c.resolvedStyle.display == DisplayStyle.None || c.resolvedStyle.visibility == Visibility.Hidden) return false;
        return true;
    }

    private static List<GameObject> Surfaces()
        => Resources.FindObjectsOfTypeAll<MeshFilter>().Where(m => m != null && m.gameObject.scene.IsValid() && m.gameObject.name == SurfaceName).Select(m => m.gameObject).ToList();

    private static float FX(StandMapModel m, float x) => (x - m.MinX) / m.SizeM;
    private static float FY(StandMapModel m, float z) => 1f - (z - m.MinZ) / m.SizeM;
    private static float Dist(float ax, float ay, float bx, float by) => (float)Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));
    private static float PX(MapDrawItem i, int k, float side) => i.Points[k].X / side;
    private static float PY(MapDrawItem i, int k, float side) => i.Points[k].Y / side;

    private TeachingDiagramPlan.Stage DiagramStage(out TeachingDiagram diagram)
    {
        diagram = ui.Learning.Root.Query<TeachingDiagram>().ToList().FirstOrDefault(Shown);
        if (diagram == null) return null;
        return (TeachingDiagramPlan.Stage)typeof(TeachingDiagram).GetField("stage", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(diagram);
    }

    private static string Texts(VisualElement root) => string.Join("\n", root.Query<TextElement>().ToList().Where(t => Shown(t) && !string.IsNullOrEmpty(t.text)).Select(t => t.text));

    private IEnumerator MapRefreshed()
    {
        yield return Wait(1.2f); // the Stand Map source re-reads racks and the headland on a timer
        yield return Frames(6);
    }

    // ---------------------------------------------------------------- J: onboarding
    private IEnumerator OnboardingKeysAndFocus()
    {
        double until = Time.realtimeSinceStartupAsDouble + 10;
        while (!ui.Help.Onboarding.IsOpen && Time.realtimeSinceStartupAsDouble < until) yield return null;
        yield return Frames(10);
        ScenarioOneOnboardingView ob = ui.Help.Onboarding;
        Check("FRESH_GAME_OPENS_THE_ONBOARDING_AT_PAGE_1", ui.Help.IsOpen && ob.IsOpen && ob.PageIndex == 0 && manager.ModalOpen, $"open={ob.IsOpen} page={ob.PageIndex} modal={manager.ModalOpen}");
        Check("PAGE_1_TITLE_IS_THE_APPROVED_COPY", Texts(ob.Root).Contains(ScenarioOneOnboardingCopy.Titles[0]));
        Focusable f = Focused();
        Check("DEFAULT_FOCUS_IS_THE_PAGE_1_NEXT_BUTTON", f is Button b0 && b0.name == "onboarding-next" && Shown(b0), Describe(f));
        yield return Shot("j-onboarding-page1");
        yield return Press(Key.M); yield return Frames(4);
        Check("M_DOES_NOT_OPEN_THE_MAP_BEHIND_THE_ONBOARDING", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && ob.IsOpen, $"screen={ui.CurrentScreen}");
        yield return Press(Key.F1); yield return Frames(4);
        Check("F1_CANNOT_REPLACE_OR_SKIP_THE_ONBOARDING", ob.IsOpen && ob.PageIndex == 0, $"page={ob.PageIndex}");
        yield return Press(Key.Escape); yield return Frames(4);
        Check("ESC_ON_PAGE_1_DOES_NOT_SKIP", ob.IsOpen && ob.PageIndex == 0, $"page={ob.PageIndex}");
        yield return Press(Key.RightArrow); yield return Frames(6);
        Check("RIGHT_ARROW_ADVANCES_TO_PAGE_2", ob.IsOpen && ob.PageIndex == 1 && Texts(ob.Root).Contains(ScenarioOneOnboardingCopy.Titles[1]), $"page={ob.PageIndex}");
        f = Focused();
        Check("DEFAULT_FOCUS_IS_THE_PAGE_2_NEXT_BUTTON", f is Button b1 && b1.name == "onboarding-next" && Shown(b1), Describe(f));
        yield return Press(Key.Escape); yield return Frames(6);
        Check("ESC_GOES_BACK_TO_PAGE_1", ob.IsOpen && ob.PageIndex == 0, $"page={ob.PageIndex}");
        yield return Press(Key.RightArrow); yield return Frames(6);
        // Enter is handled by the focused primary button (the onboarding registers no root handler for it).
        yield return Press(Key.Enter); yield return Frames(8);
        Check("ENTER_ON_THE_FOCUSED_NEXT_BUTTON_ADVANCES_TO_PAGE_3", ob.IsOpen && ob.PageIndex == 2, $"page={ob.PageIndex}");
        if (ob.IsOpen && ob.PageIndex < 2) { ob.Next(); yield return Frames(6); Measure("FALLBACK", "Enter did not advance; the page was advanced by the API to continue the run"); }
        f = Focused();
        Check("DEFAULT_FOCUS_IS_THE_BEGIN_BUTTON_ON_PAGE_3", f is Button b2 && b2.name == "onboarding-next" && b2.text.Contains("Begin") && Shown(b2), Describe(f));
        Check("PAGE_3_TITLE_IS_THE_APPROVED_COPY", Texts(ob.Root).Contains(ScenarioOneOnboardingCopy.Titles[2]));
        yield return Shot("j-onboarding-page3");
        yield return Press(Key.Enter); yield return Frames(10);
        if (ob.IsOpen) { Measure("ENTER_ON_BEGIN", "Enter did not complete; Right arrow used"); yield return Press(Key.RightArrow); yield return Frames(10); }
        Check("BEGIN_CLOSES_THE_ONBOARDING_AND_RELEASES_THE_MODAL", !ob.IsOpen && !ui.Help.IsOpen && !manager.ModalOpen, $"open={ob.IsOpen} help={ui.Help.IsOpen} modal={manager.ModalOpen}");
        f = Focused();
        Check("NO_FOCUS_IS_LEFT_ON_A_HIDDEN_ONBOARDING_ELEMENT", f == null || (f is VisualElement fv && Shown(fv) && !ob.Root.Contains(fv)), Describe(f));
        // The forest takes keyboard input again: hold W and the player walks.
        Vector3 before = ui.Player.transform.position;
        InputSystem.QueueStateEvent(Kb(), new KeyboardState(Key.W));
        yield return Wait(0.8f);
        InputSystem.QueueStateEvent(Kb(), new KeyboardState()); yield return Frames(4);
        Vector3 after = ui.Player.transform.position;
        float moved = Vector3.Distance(new Vector3(before.x, 0, before.z), new Vector3(after.x, 0, after.z));
        Check("AFTER_BEGIN_W_MOVES_THE_PLAYER", moved > 0.2f, $"moved={F(moved)}m");
        ui.Player.transform.position = before; yield return Frames(4);
    }

    private IEnumerator CaptureFresh()
    {
        ui.CloseAll(); if (ui.Help.IsOpen) ui.CloseHelp(); yield return Frames(4);
        freshJson = JsonUtility.ToJson(saves.CaptureData());
        Check("FRESH_V2_WORLD_CAPTURED", manager.RackPlanningV2Active && !manager.RackPlanConfirmed && manager.PlannedRackCount == 0 && freshJson.Length > 1000,
            $"v2={manager.RackPlanningV2Active} racks={manager.PlannedRackCount}");
    }

    // ---------------------------------------------------------------- J: Help
    private IEnumerator HelpCloseAndFocus()
    {
        int sizeIndex = 0;
        foreach (Vector2Int s in Sizes())
        {
            if (sizeIndex++ > 0) { SetGameSize(s.x, s.y); yield return Wait(1.5f); }
            size = new Vector2Int(Screen.width, Screen.height);
            ui.CloseAll(); if (ui.Help.IsOpen) ui.CloseHelp(); yield return Frames(6);
            yield return Press(Key.F1); yield return Frames(10);
            VisualElement window = ui.Help.Root.Q("menu-help-window");
            Button close = ui.Help.Root.Q<Button>("menu-help-close");
            Rect screen = ui.RootElement.worldBound;
            Check("F1_OPENS_THE_HELP_WINDOW", ui.Help.IsOpen && window != null && Shown(window) && manager.ModalOpen, $"open={ui.Help.IsOpen}");
            Check("HELP_WINDOW_IS_ON_SCREEN", window != null && window.worldBound.width > 100 && window.worldBound.xMin >= screen.xMin - 1 && window.worldBound.xMax <= screen.xMax + 1
                && window.worldBound.yMin >= screen.yMin - 1 && window.worldBound.yMax <= screen.yMax + 1, window == null ? "none" : window.worldBound + " in " + screen);
            Check("HELP_CLOSE_BUTTON_IS_VISIBLE_INSIDE_THE_WINDOW_AND_SAYS_ESC", close != null && Shown(close) && close.text.Contains("[Esc]") && window != null && window.worldBound.Contains(close.worldBound.center), close == null ? "none" : "'" + close.text + "'");
            Focusable f = Focused();
            Check("HELP_TAKES_NO_KEYBOARD_FOCUS_TRAP", f == null || !(f is VisualElement hv && ui.Help.Root.Contains(hv)), Describe(f));
            yield return Shot("j-help-hud");
            yield return Press(Key.Escape); yield return Frames(6);
            Check("ESC_CLOSES_HELP_AND_NOTHING_ELSE", !ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !manager.ModalOpen && !ui.Player.IsInspecting,
                $"help={ui.Help.IsOpen} screen={ui.CurrentScreen} modal={manager.ModalOpen}");
            yield return Press(Key.M); yield return Frames(8);
            Check("M_OPENS_THE_STAND_MAP", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, $"screen={ui.CurrentScreen}");
            yield return Press(Key.F1); yield return Frames(8);
            Check("F1_OVER_THE_MAP_OPENS_HELP", ui.Help.IsOpen);
            yield return Press(Key.Escape); yield return Frames(6);
            Check("ONE_ESC_CLOSES_ONLY_THE_HELP_THE_MAP_STAYS", !ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, $"help={ui.Help.IsOpen} screen={ui.CurrentScreen}");
            yield return Press(Key.Escape); yield return Frames(6);
            Check("A_SECOND_ESC_CLOSES_THE_MAP", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, $"screen={ui.CurrentScreen}");
            yield return Press(Key.F1); yield return Frames(6);
            bool opened = ui.Help.IsOpen;
            yield return Press(Key.F1); yield return Frames(6);
            Check("F1_TOGGLES_HELP_CLOSED", opened && !ui.Help.IsOpen, $"opened={opened} open={ui.Help.IsOpen}");
            f = Focused();
            Check("NO_FOCUS_LEFT_IN_HIDDEN_HELP", f == null || !(f is VisualElement hv2 && ui.Help.Root.Contains(hv2)), Describe(f));
        }
        SetGameSize(Sizes()[0].x, Sizes()[0].y); yield return Wait(1.5f); size = new Vector2Int(Screen.width, Screen.height);
    }

    // ---------------------------------------------------------------- B + C
    // Every number the checks below compare comes from the LIVE mesh (world-space vertices), not from ScenarioOneHeadlandGeometry.
    private struct MeshFacts { public float SpineX, South, North, FeederZ, FeederEndX, Radius; public bool Ok; public string Detail; }

    private static MeshFacts ReadMesh(GameObject surface)
    {
        var m = new MeshFacts();
        Mesh mesh = surface.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] v = mesh.vertices.Select(p => surface.transform.TransformPoint(p)).ToArray();
        float minX = v.Min(p => p.x), maxX = v.Max(p => p.x);
        // Well east of the western edge (beyond any junction pad), every vertex belongs to an eastward arm: its z band and its east end.
        var east = v.Where(p => p.x > minX + 6f).ToList();
        m.Ok = east.Count > 0;
        if (!m.Ok) { m.Detail = "no geometry east of the western edge"; return m; }
        float eMinZ = east.Min(p => p.z), eMaxZ = east.Max(p => p.z);
        m.FeederZ = (eMinZ + eMaxZ) * 0.5f;
        float armHalf = (eMaxZ - eMinZ) * 0.5f;
        m.FeederEndX = east.Max(p => p.x);
        // The spine: western vertices clear of the feeder's z band (so the junction pad, which is wider than the strip, is excluded).
        var spine = v.Where(p => p.x < minX + 6f && Math.Abs(p.z - m.FeederZ) > armHalf + 0.5f).ToList();
        m.Ok = spine.Count > 0;
        if (!m.Ok) { m.Detail = "no spine vertices"; return m; }
        float sMin = spine.Min(p => p.x), sMax = spine.Max(p => p.x);
        m.SpineX = (sMin + sMax) * 0.5f;
        m.Radius = (sMax - sMin) * 0.5f;
        m.South = spine.Min(p => p.z) + m.Radius; m.North = spine.Max(p => p.z) - m.Radius; // end discs add one radius beyond the strip ends
        m.Detail = $"spineX={F(m.SpineX)} r={F(m.Radius)} south={F(m.South)} north={F(m.North)} eastZ=[{F(eMinZ)},{F(eMaxZ)}] feederZ={F(m.FeederZ)} eastEnd={F(m.FeederEndX)} meshMaxX={F(maxX)} verts={v.Length}";
        return m;
    }

    // The Stand Map's existing-headland strokes (Full detail, SourceIndex -1) must sit where the mesh says.
    private static string HeadlandStrokeMismatch(StandMapModel model, IReadOnlyList<MapDrawItem> full, float side, MeshFacts mf, float padRadius)
    {
        var problems = new List<string>();
        const float tol = 2e-3f; // fractions of the property (0.16 m at 80 m)
        var lines = full.Where(i => i.Symbol == MapSymbol.Path && i.SourceIndex == -1 && i.Points.Count == 2).ToList();
        var pads = full.Where(i => i.Symbol == MapSymbol.Path && i.SourceIndex == -1 && i.Points.Count == 20).ToList();
        bool Line(float ax, float az, float bx, float bz) => lines.Any(l =>
            (Dist(PX(l, 0, side), PY(l, 0, side), FX(model, ax), FY(model, az)) < tol && Dist(PX(l, 1, side), PY(l, 1, side), FX(model, bx), FY(model, bz)) < tol) ||
            (Dist(PX(l, 1, side), PY(l, 1, side), FX(model, ax), FY(model, az)) < tol && Dist(PX(l, 0, side), PY(l, 0, side), FX(model, bx), FY(model, bz)) < tol));
        bool Pad(float x, float z) => pads.Any(p => Dist(p.Anchor.X / side, p.Anchor.Y / side, FX(model, x), FY(model, z)) < tol
            && Math.Abs(Dist(PX(p, 0, side), PY(p, 0, side), p.Anchor.X / side, p.Anchor.Y / side) - padRadius / model.SizeM) < tol);
        if (lines.Count != 2) problems.Add("fillLines=" + lines.Count);
        if (pads.Count != 2) problems.Add("fillPads=" + pads.Count);
        if (!Line(mf.SpineX, mf.South, mf.SpineX, mf.North)) problems.Add("no spine stroke at the mesh spine");
        if (!Line(mf.SpineX, mf.FeederZ, mf.FeederEndX - padRadius, mf.FeederZ)) problems.Add("no feeder stroke at the mesh feeder");
        if (!Pad(mf.SpineX, mf.FeederZ)) problems.Add("no junction pad at the mesh junction");
        if (!Pad(mf.FeederEndX - padRadius, mf.FeederZ)) problems.Add("no path pad at the mesh feeder end");
        return string.Join("; ", problems);
    }

    private IEnumerator SurfaceMapAnd3D()
    {
        ui.CloseAll(); yield return MapRefreshed();
        List<GameObject> surfaces = Surfaces();
        GameObject surface = surfaces.FirstOrDefault();
        Check("EXACTLY_ONE_V2_HEADLAND_SURFACE_EXISTS_AND_IS_ACTIVE", surfaces.Count == 1 && surface.activeInHierarchy && surface.GetComponent<MeshRenderer>().enabled
            && surface.transform.parent != null && surface.transform.parent.GetComponent<ScenarioOneHeadlandPresentation>() != null, $"count={surfaces.Count}");
        if (surface == null) yield break;
        MeshFacts mf = ReadMesh(surface);
        Measure("MESH", mf.Detail);
        StandMapModel model = ui.MapSource.Model;
        Check("THE_SPINE_IS_THE_WESTERN_EDGE_AND_RUNS_NEARLY_THE_FULL_PROPERTY", mf.Ok && mf.SpineX < model.MinX + 8f && mf.South < model.MinZ + 4f && mf.North > model.MinZ + model.SizeM - 4f, mf.Detail);
        Check("THE_ONLY_EASTWARD_ARM_IS_ONE_SOUTHERN_FEEDER_NO_NORTHERN_SPUR",
            mf.Ok && mf.FeederZ < model.MinZ + model.SizeM * 0.5f && surface.GetComponent<MeshFilter>().sharedMesh.vertices
                .Select(p => surface.transform.TransformPoint(p)).Where(p => p.x > mf.SpineX + 3f).All(p => Math.Abs(p.z - mf.FeederZ) <= 2.3f), mf.Detail);
        // The feeder must actually meet the forest path the player walks (independent fact: the path comes from the scene's path pieces).
        float best = float.MaxValue, qx = mf.FeederEndX - ScenarioOneHeadlandGeometry.PadRadius, qz = mf.FeederZ;
        foreach (MapPath path in model.Paths)
            for (int k = 0; k + 1 < path.Points.Count; k++)
            {
                RackPoint a = path.Points[k], b = path.Points[k + 1];
                float dx = b.X - a.X, dz = b.Z - a.Z, len2 = dx * dx + dz * dz;
                float t = len2 > 0f ? Math.Max(0f, Math.Min(1f, ((qx - a.X) * dx + (qz - a.Z) * dz) / len2)) : 0f;
                best = Math.Min(best, Dist(qx, qz, a.X + t * dx, a.Z + t * dz));
            }
        Check("THE_FEEDER_END_MEETS_THE_FOREST_PATH", best <= ScenarioOneHeadlandGeometry.PadRadius, $"nearestPathPointM={F(best)}");
        Check("THE_STAND_MAP_MODEL_HOLDS_ONE_HEADLAND_AT_THE_MESH_PLACE", model.ExistingHeadlandSurfaces.Count == 1
            && Math.Abs(model.ExistingHeadlandSurfaces[0].Headland.Start.X - mf.SpineX) < 0.05f && Math.Abs(model.ExistingHeadlandSurfaces[0].Feeder.Start.Z - mf.FeederZ) < 0.05f
            && Math.Abs(model.ExistingHeadlandSurfaces[0].Headland.Start.Z - mf.South) < 0.05f && Math.Abs(model.ExistingHeadlandSurfaces[0].Headland.End.Z - mf.North) < 0.05f
            && Math.Abs(model.ExistingHeadlandSurfaces[0].Feeder.End.X - (mf.FeederEndX - ScenarioOneHeadlandGeometry.PadRadius)) < 0.05f, $"model={model.ExistingHeadlandSurfaces.Count}");

        int sizeIndex = 0;
        foreach (Vector2Int s in Sizes())
        {
            if (sizeIndex++ > 0) { SetGameSize(s.x, s.y); yield return Wait(1.5f); }
            size = new Vector2Int(Screen.width, Screen.height);
            ui.ShowMap(); yield return MapRefreshed();
            StandMapCanvas full = ui.Map.Canvas;
            var plan = full.LastPlan.ToList(); float side = full.LastSidePx;
            string mismatch = HeadlandStrokeMismatch(model, plan, side, mf, ScenarioOneHeadlandGeometry.PadRadius);
            Check("STAND_MAP_HEADLAND_STROKES_AND_PADS_ARE_AT_THE_MESH_PLACE", mismatch.Length == 0, mismatch);
            MeshFacts shifted = mf; shifted.North += 3f;
            Check("NEGATIVE_CONTROL_A_3M_NORTHERN_SHIFT_IS_DETECTED", HeadlandStrokeMismatch(model, plan, side, shifted, ScenarioOneHeadlandGeometry.PadRadius).Length > 0);
            MeshFacts spur = mf; spur.FeederZ = -spur.FeederZ;
            Check("NEGATIVE_CONTROL_A_NORTHERN_FEEDER_IS_DETECTED", HeadlandStrokeMismatch(model, plan, side, spur, ScenarioOneHeadlandGeometry.PadRadius).Length > 0);
            // No casing may be drawn over a fill (that is what puts a dark edge across a join): every headland casing precedes every headland fill.
            int lastCasing = plan.FindLastIndex(i => i.Symbol == MapSymbol.PathCasing && i.SourceIndex == -1), firstFill = plan.FindIndex(i => i.Symbol == MapSymbol.Path && i.SourceIndex == -1);
            Check("HEADLAND_CASINGS_ARE_ALL_DRAWN_BEFORE_ANY_HEADLAND_FILL", lastCasing >= 0 && firstFill > lastCasing, $"lastCasing={lastCasing} firstFill={firstFill}");
            yield return Shot("c-stand-map");
            ui.CloseAll(); yield return Frames(6);
            yield return TopDown3D("c-3d-top-down");
        }
        SetGameSize(Sizes()[0].x, Sizes()[0].y); yield return Wait(1.5f); size = new Vector2Int(Screen.width, Screen.height);
    }

    // A straight-down orthographic render of the whole property, north up, so the 3D surface can be laid beside the Stand Map.
    private IEnumerator TopDown3D(string state)
    {
        StandMapModel model = ui.MapSource.Model;
        var rig = new GameObject("CCF359 top-down camera");
        Camera cam = rig.AddComponent<Camera>();
        Camera main = Camera.main;
        if (main != null) cam.CopyFrom(main);
        cam.enabled = false; cam.orthographic = true; cam.orthographicSize = model.SizeM * 0.5f; cam.aspect = 1f;
        cam.nearClipPlane = 0.3f; cam.farClipPlane = 400f;
        cam.transform.position = new Vector3(model.MinX + model.SizeM * 0.5f, 120f, model.MinZ + model.SizeM * 0.5f);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        // The canopy hides the ground from above; render only the ground-level layers by culling tree renderers temporarily.
        var hidden = new List<Renderer>();
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsSortMode.None))
            foreach (Renderer r in t.GetComponentsInChildren<Renderer>()) if (r.enabled) { r.enabled = false; hidden.Add(r); }
        yield return null;
        int px = Math.Min(Screen.height, 1024);
        var rt = RenderTexture.GetTemporary(px, px, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        cam.targetTexture = rt; cam.Render();
        RenderTexture prev = RenderTexture.active; RenderTexture.active = rt;
        var tex = new Texture2D(px, px, TextureFormat.RGBA32, false, false);
        tex.ReadPixels(new Rect(0, 0, px, px), 0, 0); tex.Apply();
        RenderTexture.active = prev; cam.targetTexture = null; RenderTexture.ReleaseTemporary(rt);
        foreach (Renderer r in hidden) if (r != null) r.enabled = true;
        File.WriteAllBytes(Path.Combine(output, state + "-" + Screen.width + "x" + Screen.height + ".png"), tex.EncodeToPNG());
        shots++;
        Destroy(tex); Destroy(rig);
        Measure("TOP_DOWN", $"orthographic {px}px square = {F(model.SizeM)} m, trees hidden for the capture and re-enabled ({hidden.Count} renderers)");
    }

    // ---------------------------------------------------------------- G
    private IEnumerator FiveRackFigure()
    {
        var suggestions = manager.GetSuggestedRackLayout().OrderBy(sgm => sgm.Entrance.Z).ToList();
        int sizeIndex = 0;
        foreach (Vector2Int s in Sizes())
        {
            if (sizeIndex++ > 0) { SetGameSize(s.x, s.y); yield return Wait(1.5f); }
            size = new Vector2Int(Screen.width, Screen.height);
            ui.CloseAll(); ui.ShowObjectives(); yield return Frames(20);
            TeachingDiagramPlan.Stage stage = DiagramStage(out TeachingDiagram diagram);
            Check("OBJECTIVE_1_CARD_SHOWS_A_DIAGRAM", stage != null && ui.Learning.CurrentTopicIndex == 0, $"topic={ui.Learning.CurrentTopicIndex}");
            if (stage == null) continue;
            Check("IT_IS_THE_FIVE_RACK_FIGURE", stage.Id == "access-five-rack", "id=" + stage.Id);
            var racks = stage.Items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Corridor && i.Id.StartsWith("rack") && i.Id.Length == 5).OrderByDescending(i => i.Y).ToList();
            Check("FIVE_PLANNED_CORRIDORS", racks.Count == 5 && racks.All(r => r.State == TeachingDiagramPlan.CorridorState.Planned), "corridors=" + racks.Count);
            var bands = new HashSet<string>(stage.Items.Where(i => i.Id.StartsWith("band")).Select(i => i.Id.Substring(0, i.Id.IndexOf('-'))));
            Check("SIX_TREE_BANDS_BETWEEN_AND_BESIDE_THE_CORRIDORS", bands.Count == 6, "bands=" + string.Join(",", bands.OrderBy(x => x, StringComparer.Ordinal)));
            if (racks.Count == 5)
            {
                float[] gaps = Enumerable.Range(0, 4).Select(i => racks[i].Y - racks[i + 1].Y).ToArray();
                Check("CORRIDOR_SPACING_IS_NOT_UNIFORM", gaps.Max() - gaps.Min() > 2f, "gaps=" + string.Join(",", gaps.Select(g => F(g))));
                // Independent of the figure's own constants: the LIVE suggested routes of this world, through the figure's published scale.
                float Expected(float z) => TeachingDiagramPlan.FigureMapTop + (40f - z) * TeachingDiagramPlan.FigurePxPerM;
                bool match = suggestions.Count == 5 && Enumerable.Range(0, 5).All(i => Math.Abs(racks[i].Y - Expected(suggestions[i].Entrance.Z)) < 0.1f);
                Check("CORRIDORS_SIT_AT_THE_LIVE_SUGGESTED_ROUTES", match, "figureY=" + string.Join(",", racks.Select(r => F(r.Y))) + " liveZ=" + string.Join(",", suggestions.Select(x => F(x.Entrance.Z))));
            }
            VisualElement frame = diagram.Q("teaching-diagram-frame");
            Check("FIGURE_FRAME_IS_VISIBLE_AND_LARGE_ENOUGH", frame != null && Shown(frame) && frame.worldBound.width >= 560f, frame == null ? "none" : frame.worldBound.ToString());
            string text = Texts(diagram);
            Check("FIGURE_TITLE_SAYS_IT_IS_ILLUSTRATIVE", text.Contains(TeachingDiagramPlan.FigureTitle));
            var labels = frame == null ? new List<Label>() : frame.Query<Label>().ToList().Where(Shown).ToList();
            bool inside = labels.All(l => l.worldBound.xMin >= frame.worldBound.xMin - 1f && l.worldBound.xMax <= frame.worldBound.xMax + 1f && l.worldBound.yMax <= frame.worldBound.yMax + 1f);
            Check("FIGURE_WORDS_STAY_INSIDE_THE_FRAME", labels.Count > 0 && inside, "labels=" + labels.Count);
            int overlaps = 0;
            for (int i = 0; i < labels.Count; i++) for (int j = i + 1; j < labels.Count; j++)
            {
                Rect a = labels[i].worldBound, b = labels[j].worldBound;
                if (a.xMin < b.xMax - 1f && b.xMin < a.xMax - 1f && a.yMin < b.yMax - 1f && b.yMin < a.yMax - 1f) overlaps++;
            }
            Check("FIGURE_WORDS_DO_NOT_OVERLAP", overlaps == 0, "overlaps=" + overlaps);
            float smallest = diagram.Query<Label>().ToList().Where(Shown).Select(l => l.resolvedStyle.fontSize).DefaultIfEmpty(0f).Min();
            Check("FIGURE_WORDS_ARE_AT_LEAST_THE_FIGURE_FLOOR", smallest >= TeachingDiagramPlan.FigureMinTextSize - 0.01f, "smallestUiUnits=" + F(smallest));
            Check("CARD_USES_THE_V2_WORDING", Texts(ui.Learning.Root).Contains(LearningObjectivesView.RackV2Concept.Substring(0, 40)));
            yield return Shot("g-objective1-five-rack");
        }
        ui.CloseAll();
        SetGameSize(Sizes()[0].x, Sizes()[0].y); yield return Wait(1.5f); size = new Vector2Int(Screen.width, Screen.height);
    }

    // ---------------------------------------------------------------- I (forced)
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
    }

    private IEnumerator WorkPlanForcedLocks()
    {
        Check("FRESH_WORLD_HAS_NO_PLANTING_PRUNING_OR_STOCK", !manager.Inventory.Any(e => e.quantity > 0) && !manager.WorkOrders.Any(o => o.type == ScenarioWorkType.PruneTree));
        foreach (int stage in new[] { 0, 5, 7 })
        {
            ui.CloseAll(); ForceStage(stage); yield return Frames(4);
            ui.ShowWorkPlan(); yield return Frames(18);
            ForceStage(stage); yield return Frames(4); // ShowWorkPlan may refresh the view; re-inject and let it rebuild
            ui.CloseAll(); ui.ShowWorkPlan(); yield return Frames(18);
            var locked = ui.RootElement.Query(className: "card-locked").ToList().Where(Shown).ToList();
            int expected = stage == 0 ? 3 : stage == 5 ? 1 : 0;
            Check($"FORCED_LESSON_{stage}_INDEX_TAKES", ui.Learning.CurrentTopicIndex == stage, "topic=" + ui.Learning.CurrentTopicIndex);
            Check($"WORKPLAN_LOCKED_SECTIONS_AT_FORCED_LESSON_{stage}", locked.Count == expected,
                $"locked={locked.Count} expected={expected} [{string.Join(" | ", locked.Select(l => l.Q<Label>(className: "heading")?.text))}]");
            Check($"LOCKED_SECTIONS_ARE_DISABLED_AND_EXPLAIN_{stage}", locked.All(l => !l.enabledInHierarchy && Texts(l).Contains(ScenarioOneWorkPlanFocus.LaterObjectiveNote)));
            yield return Shot("i-workplan-forced-lesson-" + stage);
        }
        ui.CloseAll(); ForceStage(0); yield return Frames(8);
        Check("REAL_FACTS_RESTORED_TO_LESSON_0", ui.Learning.CurrentTopicIndex == 0, "topic=" + ui.Learning.CurrentTopicIndex);
    }

    // ---------------------------------------------------------------- F (measured only)
    private IEnumerator Performance()
    {
        foreach (var (state, open) in new (string, Action)[] {
            ("hud-still", () => ui.CloseAll()), ("stand-map-open", () => { ui.CloseAll(); ui.ShowMap(); }), ("objectives-open", () => { ui.CloseAll(); ui.ShowObjectives(); }) })
        {
            open(); yield return Wait(1.0f);
            using (var alloc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame", 400))
            using (var main = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Main Thread", 400))
            {
                int gc0 = GC.CollectionCount(0); long mono0 = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
                var ms = new List<float>(); var allocs = new List<long>(); var mains = new List<double>();
                for (int i = 0; i < 300; i++)
                {
                    yield return null;
                    ms.Add(Time.unscaledDeltaTime * 1000f);
                    if (alloc.Valid) allocs.Add(alloc.LastValue);
                    if (main.Valid) mains.Add(main.LastValue * 1e-6);
                }
                int gc1 = GC.CollectionCount(0); long mono1 = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
                ms.Sort();
                string a = allocs.Count == 0 ? "allocRecorder=invalid" : $"allocB/frame mean={F(allocs.Average())} max={allocs.Max()} firstHalfMean={F(allocs.Take(150).Average())} secondHalfMean={F(allocs.Skip(150).Average())}";
                string mt = mains.Count == 0 ? "mainRecorder=invalid" : $"mainThreadMs mean={F(mains.Average())}";
                Measure("FRAMES_" + state, $"frames=300 frameMs mean={F(ms.Average())} p50={F(ms[150])} p95={F(ms[284])} max={F(ms[299])} {mt} {a} gen0Collections={gc1 - gc0} monoUsedDeltaB={mono1 - mono0}");
                Check("MEASUREMENT_CAPTURED_" + state, ms.Count == 300 && alloc.Valid, $"allocValid={alloc.Valid} mainValid={main.Valid}");
            }
        }
        ui.CloseAll(); yield return Frames(4);
        Measure("CONTEXT", $"graphics={SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsDeviceType}) Editor play mode in a private Xephyr display: absolute times are NOT player performance; no baseline exists, so nothing is asserted");
    }

    // ---------------------------------------------------------------- D+ planning / confirmed parity
    private Button PlanButton() => ui.Map.Root.Query<Button>().ToList().FirstOrDefault(b => b.text == "Plan Racks" || b.text.StartsWith("Planning Racks"));
    private static IEnumerator Click(Button button)
    {
        if (button == null) throw new InvalidOperationException("button not found");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }

    // Confirmed racks must be drawn once on each canvas (band + line, same SourceIndex), at the same fractions and at the manager's own endpoints.
    private static string ConfirmedParity(StandMapModel model, IReadOnlyList<RackPlanRack> confirmed, IReadOnlyList<MapDrawItem> mini, float miniSide, IReadOnlyList<MapDrawItem> full, float fullSide)
    {
        var problems = new List<string>();
        const float pair = 1e-4f, indep = 2e-4f;
        var miniRackItems = mini.Where(i => i.Symbol == MapSymbol.RackBand || i.Symbol == MapSymbol.RackPlayer || i.Symbol == MapSymbol.RackSuggested || i.Symbol == MapSymbol.RackHarvested).ToList();
        if (miniRackItems.Any(i => i.SourceIndex < 0)) problems.Add("minimap still draws guidance or an unowned rack item: " + miniRackItems.Count(i => i.SourceIndex < 0));
        if (full.Any(i => i.Symbol == MapSymbol.RackSuggested)) problems.Add("Stand Map draws a suggestion after confirmation");
        if (confirmed.Count != 5) problems.Add("confirmed=" + confirmed.Count);
        var claimed = new HashSet<MapDrawItem>();
        foreach (RackPlanRack rack in confirmed)
        {
            int idx = -1;
            for (int k = 0; k < model.Racks.Count; k++) if (model.Racks[k].RackId == rack.Id) idx = k;
            if (idx < 0) { problems.Add("rack " + rack.Id + " missing from the model"); continue; }
            foreach (MapSymbol sym in new[] { MapSymbol.RackBand, MapSymbol.RackPlayer })
            {
                int pts = sym == MapSymbol.RackBand ? 4 : 2;
                var m = mini.Where(i => i.Symbol == sym && i.SourceIndex == idx && i.Points.Count == pts && !claimed.Contains(i)).ToList();
                var f = full.Where(i => i.Symbol == sym && i.SourceIndex == idx && i.Points.Count == pts && !claimed.Contains(i)).ToList();
                if (m.Count != 1 || f.Count != 1) { problems.Add($"rack {rack.Id} {sym}: mini={m.Count} full={f.Count}"); continue; }
                claimed.Add(m[0]); claimed.Add(f[0]);
                for (int k = 0; k < pts; k++)
                    if (Dist(PX(m[0], k, miniSide), PY(m[0], k, miniSide), PX(f[0], k, fullSide), PY(f[0], k, fullSide)) > pair) problems.Add($"rack {rack.Id} {sym} point {k} differs between canvases");
                foreach (var (item, side) in new[] { (m[0], miniSide), (f[0], fullSide) })
                {
                    float sx, sy, ex, ey;
                    if (sym == MapSymbol.RackBand) { sx = (PX(item, 0, side) + PX(item, 3, side)) * 0.5f; sy = (PY(item, 0, side) + PY(item, 3, side)) * 0.5f; ex = (PX(item, 1, side) + PX(item, 2, side)) * 0.5f; ey = (PY(item, 1, side) + PY(item, 2, side)) * 0.5f; }
                    else { sx = PX(item, 0, side); sy = PY(item, 0, side); ex = PX(item, 1, side); ey = PY(item, 1, side); }
                    if (Dist(sx, sy, FX(model, rack.Entrance.X), FY(model, rack.Entrance.Z)) > indep || Dist(ex, ey, FX(model, rack.Far.X), FY(model, rack.Far.Z)) > indep)
                        problems.Add($"rack {rack.Id} {sym} is not at the manager's endpoints");
                }
            }
        }
        return string.Join("; ", problems);
    }

    private IEnumerator PlanningAndConfirmedParity()
    {
        var suggestions = manager.GetSuggestedRackLayout().ToList();
        // 1. Outside planning: the minimap's dashed guidance lines, recorded for comparison with the planning view.
        ui.CloseAll(); yield return MapRefreshed();
        var mini = ui.Minimap.Canvas.LastPlan.ToList(); float miniSide = ui.Minimap.Canvas.LastSidePx;
        var guidance = mini.Where(i => i.Symbol == MapSymbol.RackSuggested && i.SourceIndex == -1).ToList();
        Check("MINIMAP_SHOWS_FIVE_DASHED_ROUTES_BEFORE_PLANNING", guidance.Count == 5 && suggestions.Count == 5, "guidance=" + guidance.Count);
        // 2. Planning mode: the Stand Map shows the same five routes, at the same fractions as the minimap guidance.
        ui.ShowMap(); yield return MapRefreshed();
        yield return Click(PlanButton()); yield return MapRefreshed();
        StandMapModel model = ui.MapSource.Model;
        var full = ui.Map.Canvas.LastPlan.ToList(); float fullSide = ui.Map.Canvas.LastSidePx;
        var planningLines = full.Where(i => i.Symbol == MapSymbol.RackSuggested && i.Points.Count == 2).ToList();
        Check("PLANNING_MODE_IS_ON_AND_THE_ROUTES_MOVED_TO_THE_STAND_MAP", ui.Map.RackMode && model.MiniGuidance.Count == 0 && planningLines.Count == 5, $"rackMode={ui.Map.RackMode} guidance={model.MiniGuidance.Count} fullSuggested={planningLines.Count}");
        bool same = guidance.Count == 5 && guidance.All(g => planningLines.Any(l =>
            Dist(PX(g, 0, miniSide), PY(g, 0, miniSide), PX(l, 0, fullSide), PY(l, 0, fullSide)) < 1e-4f && Dist(PX(g, 1, miniSide), PY(g, 1, miniSide), PX(l, 1, fullSide), PY(l, 1, fullSide)) < 1e-4f));
        Check("PLANNING_ROUTES_ARE_AT_THE_SAME_FRACTIONS_AS_THE_MINIMAP_GUIDANCE", same);
        bool atSuggestions = suggestions.All(sg => planningLines.Any(l => Dist(PX(l, 0, fullSide), PY(l, 0, fullSide), FX(model, sg.Entrance.X), FY(model, sg.Entrance.Z)) < 2e-4f
            && Dist(PX(l, 1, fullSide), PY(l, 1, fullSide), FX(model, sg.Far.X), FY(model, sg.Far.Z)) < 2e-4f));
        Check("PLANNING_ROUTES_ARE_AT_THE_MANAGER_SUGGESTED_ENDPOINTS", atSuggestions);
        yield return Shot("d-stand-map-planning");
        // 3. Five drafts from the suggestion (the authoritative editing API the map buttons call): Stand Map only, minimap unchanged.
        RackPlanOperationResult reset = manager.ResetRacksToSuggestion(manager.GetSuggestedRackLayout());
        yield return MapRefreshed();
        Check("FIVE_DRAFT_RACKS_CREATED", reset.Succeeded && manager.PlannedRackCount == 5 && !manager.RackPlanConfirmed, reset.Message);
        full = ui.Map.Canvas.LastPlan.ToList(); fullSide = ui.Map.Canvas.LastSidePx;
        Check("DRAFTS_ARE_ON_THE_STAND_MAP", full.Count(i => i.Symbol == MapSymbol.RackPlayer && i.SourceIndex >= 0) == 5, "fullPlayerRacks=" + full.Count(i => i.Symbol == MapSymbol.RackPlayer));
        yield return Click(PlanButton()); yield return Frames(4);
        ui.CloseAll(); yield return MapRefreshed();
        mini = ui.Minimap.Canvas.LastPlan.ToList();
        Check("DRAFTS_ARE_NOT_ON_THE_MINIMAP_WHICH_STILL_SHOWS_GUIDANCE", !mini.Any(i => i.Symbol == MapSymbol.RackPlayer) && mini.Count(i => i.Symbol == MapSymbol.RackSuggested && i.SourceIndex == -1) == 5,
            $"miniPlayer={mini.Count(i => i.Symbol == MapSymbol.RackPlayer)} miniGuidance={mini.Count(i => i.Symbol == MapSymbol.RackSuggested)}");
        // 4. Confirm, then the confirmed plan must be on both canvases one-to-one.
        RackPlanOperationResult confirm = manager.ConfirmRackPlanFromMap();
        yield return MapRefreshed();
        Check("THE_FIVE_RACK_PLAN_CONFIRMS", confirm.Status == RackPlanOperationStatus.Applied && manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, confirm.Message);
        int sizeIndex = 0;
        foreach (Vector2Int s in Sizes())
        {
            if (sizeIndex++ > 0) { SetGameSize(s.x, s.y); yield return Wait(1.5f); }
            size = new Vector2Int(Screen.width, Screen.height);
            ui.CloseAll(); yield return MapRefreshed();
            model = ui.MapSource.Model;
            Check("MINIMAP_PAINTED_FROM_THE_CURRENT_MODEL", ui.Minimap.Visible && ui.Minimap.Canvas.PaintedVersion == model.Version);
            mini = ui.Minimap.Canvas.LastPlan.ToList(); miniSide = ui.Minimap.Canvas.LastSidePx;
            yield return Shot("d-hud-minimap-confirmed");
            ui.ShowMap(); yield return MapRefreshed();
            full = ui.Map.Canvas.LastPlan.ToList(); fullSide = ui.Map.Canvas.LastSidePx;
            string problems = ConfirmedParity(model, manager.ConfirmedRacks, mini, miniSide, full, fullSide);
            Check("CONFIRMED_RACKS_ARE_ONE_TO_ONE_ON_BOTH_CANVASES_AT_THE_MANAGER_ENDPOINTS", problems.Length == 0, problems);
            // Negative control: the same check must reject a Stand Map whose first confirmed rack line is moved by 1% of the map.
            var broken = full.Select(i => i).ToList();
            int target = broken.FindIndex(i => i.Symbol == MapSymbol.RackPlayer && i.SourceIndex >= 0);
            if (target >= 0)
            {
                MapDrawItem src = broken[target];
                var clone = (MapDrawItem)Activator.CreateInstance(typeof(MapDrawItem));
                foreach (FieldInfo fi in typeof(MapDrawItem).GetFields(BindingFlags.Instance | BindingFlags.Public)) fi.SetValue(clone, fi.GetValue(src));
                clone.Points = src.Points.Select(p => new MapPosition(p.X + 0.01f * fullSide, p.Y)).ToList();
                broken[target] = clone;
            }
            Check("NEGATIVE_CONTROL_A_MOVED_CONFIRMED_RACK_IS_DETECTED", target >= 0 && ConfirmedParity(model, manager.ConfirmedRacks, mini, miniSide, broken, fullSide).Length > 0);
            Measure("COUNTS", $"mini={mini.Count} full={full.Count} miniRackItems={mini.Count(i => i.SourceIndex >= 0 && (i.Symbol == MapSymbol.RackBand || i.Symbol == MapSymbol.RackPlayer))}");
            yield return Shot("d-stand-map-confirmed");
        }
        ui.CloseAll();
        SetGameSize(Sizes()[0].x, Sizes()[0].y); yield return Wait(1.5f); size = new Vector2Int(Screen.width, Screen.height);
    }

    // ---------------------------------------------------------------- B transitions + H (pre-V2 load, Reference Preview)
    private IEnumerator WorldState(string label, bool expectV2)
    {
        ui.CloseAll(); yield return MapRefreshed();
        List<GameObject> surfaces = Surfaces();
        Check(label + "_V2_ACTIVE_IS_" + expectV2, manager.RackPlanningV2Active == expectV2, "v2=" + manager.RackPlanningV2Active + " geometry=" + ui.Ecology.StandGeometryModelVersion);
        Check(label + "_SURFACE_OBJECTS_AT_MOST_ONE_ACTIVE_ONLY_IF_V2", surfaces.Count <= 1 && surfaces.Count(g => g.activeInHierarchy) == (expectV2 ? 1 : 0), $"objects={surfaces.Count} active={surfaces.Count(g => g.activeInHierarchy)}");
        Check(label + "_STAND_MAP_MODEL_HEADLAND_ONLY_IF_V2", ui.MapSource.Model.ExistingHeadlandSurfaces.Count == (expectV2 ? 1 : 0), "model=" + ui.MapSource.Model.ExistingHeadlandSurfaces.Count);
        ui.ShowMap(); yield return MapRefreshed();
        int strokes = ui.Map.Canvas.LastPlan.Count(i => (i.Symbol == MapSymbol.Path || i.Symbol == MapSymbol.PathCasing) && i.SourceIndex == -1);
        Check(label + "_STAND_MAP_HEADLAND_STROKES_ONLY_IF_V2", expectV2 ? strokes == 8 : strokes == 0, "strokes=" + strokes);
        yield return Shot("bh-" + label.ToLowerInvariant() + "-stand-map");
        ui.CloseAll(); ui.ShowObjectives(); yield return Frames(20);
        TeachingDiagramPlan.Stage stage = DiagramStage(out _);
        string cardText = Texts(ui.Learning.Root);
        if (expectV2) Check(label + "_OBJECTIVE_1_SHOWS_THE_FIVE_RACK_FIGURE", stage != null && stage.Id == "access-five-rack", "id=" + stage?.Id + " topic=" + ui.Learning.CurrentTopicIndex);
        else
        {
            Check(label + "_NO_FIVE_RACK_FIGURE_IN_A_NON_V2_WORLD", stage == null || stage.Id != "access-five-rack", "id=" + stage?.Id + " topic=" + ui.Learning.CurrentTopicIndex);
            Check(label + "_NO_FIGURE_TITLE_AND_NO_V2_CONCEPT_WORDING", !cardText.Contains(TeachingDiagramPlan.FigureTitle) && !cardText.Contains(LearningObjectivesView.RackV2Concept.Substring(0, 40)));
        }
        yield return Shot("bh-" + label.ToLowerInvariant() + "-objectives");
        ui.CloseAll(); yield return Frames(4);
    }

    private IEnumerator Transitions()
    {
        if (freshJson == null) throw new InvalidOperationException("no fresh world captured");
        GameObject original = Surfaces().FirstOrDefault();
        // Start from the unconfirmed fresh V2 world (the parity section confirmed a plan in this session).
        Check("FRESH_V2_WORLD_RELOADS", saves.LoadData(JsonUtility.FromJson<ForestSaveData>(freshJson), false));
        yield return Frames(4);
        yield return WorldState("V2_FRESH", true);
        // A pre-V2 (model 0) Enlarged80 world loaded into the same session: the surface must go, the map and the figure must follow.
        ForestSaveData legacy = JsonUtility.FromJson<ForestSaveData>(freshJson);
        legacy.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        Check("MODEL_0_ENLARGED80_WORLD_LOADS", saves.LoadData(legacy, false));
        yield return Frames(4);
        yield return WorldState("PRE_V2_LOAD", false);
        // Back to the V2 world: the same single surface object comes back.
        Check("V2_WORLD_RELOADS", saves.LoadData(JsonUtility.FromJson<ForestSaveData>(freshJson), false));
        yield return Frames(4);
        yield return WorldState("V2_RELOADED", true);
        Check("THE_SAME_SURFACE_OBJECT_RETURNS_NO_DUPLICATE", Surfaces().Count == 1 && Surfaces()[0] == original);
        // Reference Future v1 preview from the V2 world (the frozen archive is Legacy40); then return.
        bool began = manager.TryBeginReferencePreview(50);
        Measure("REFERENCE_PREVIEW_FROM_V2", "began=" + began + " feedback=" + (typeof(ScenarioOneManager).GetField("feedback", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(manager) ?? ""));
        if (began)
        {
            yield return Wait(1.0f);
            Check("PREVIEW_IS_ACTIVE", manager.ReferencePreviewActive);
            yield return WorldState("REFERENCE_PREVIEW", false);
            manager.EndReferencePreview(); yield return Wait(1.0f);
            Check("PREVIEW_ENDED", !manager.ReferencePreviewActive);
            yield return WorldState("AFTER_PREVIEW", true);
            Check("STILL_THE_SAME_SURFACE_OBJECT_AFTER_PREVIEW", Surfaces().Count == 1 && Surfaces()[0] == original);
        }
    }
}
#endif
