#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
using CCF.Forestry.WorkEconomy;

// Disposable RENDERED + INPUT qualification for the Scenario One UX gap patch (84e881a). It plays a fresh-player session through the real
// UI (virtual keys, the UI's own registered button callbacks) and records, for each packet section, what actually happened.
//
// HARD results (UXQ_PASS / UXQ_FAIL) are the packet's explicit expectations. MEASURE lines are evidence for a human decision (font sizes,
// timings). Screenshots are written for every state at every window size and are meant to be LOOKED AT; nothing judges layout by itself.
//
// Limits stated up front: Editor-synthetic mouse events do not reach the runtime UI panel (the shipped gates say the same), so button
// presses invoke the button's registered callback; a physical mouse, native Windows and the Player build are NOT exercised here.
//
// Run (graphical Editor):  -executeMethod UxQualification.Begin     CCF_UI_SIZES="1280x720,1920x1080,1024x768"  CCF_DIAG_DIR=<folder>
public static class UxQualification
{
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "UX qualification " + width + "x" + height }, null);
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
        if (string.IsNullOrEmpty(text)) text = "1280x720,1920x1080,1024x768";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
    }

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(Sizes()[0].x, Sizes()[0].y);
        EditorApplication.isPlaying = true;
    }

    // A genuinely fresh player: no first-use screen has been seen and no lesson step is recorded. The Editor runs on an isolated
    // config folder, so nothing here touches a real player's preferences.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "UxQualification.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "UxQualification.Begin"))
            new GameObject("UX qualification").AddComponent<UxQualificationRunner>();
    }
}

// Explicit phase requests must all be known; a typo must not silently omit the requested coverage.
public static class UxQualificationPhases
{
    public const string Known = "ROLE,FIRST_CYCLE,GUARD_VARIANTS,RESET_FLOW,CONTROLS_AND_TERMINOLOGY,MODEL_AWARE_COPY,INSPECTION_CASES,DEADWOOD_LINE,INPUT_SMOKE,F10_INPUT,PRESERVATION,PERF_SANITY,PERF_CPU_ONLY,STARTUP_CONTROLS";

    public static string[] Select(string filter)
    {
        string[] phases = (filter ?? Known).Split(',').Select(token => token.Trim().ToUpperInvariant()).ToArray();
        if (phases.Any(token => !Known.Split(',').Contains(token)))
            throw new InvalidOperationException("Unknown CCF_UXQ_PHASES token in [" + filter + "]. Valid phases: " + Known);
        return phases;
    }
}

// F10 evidence is session-wide: a demonstrated transition removes the no-delivery exemption.
// Kept free of Unity APIs so the offline regression executes this exact classifier.
public sealed class F10DeliveryEvidence
{
    private bool everDelivered;

    public string Observe(bool beforeOpen, bool afterOpen, bool expectedOpen)
    {
        bool delivered = beforeOpen != afterOpen;
        everDelivered |= delivered;
        if (delivered && afterOpen == expectedOpen) return "PASS";
        return everDelivered ? "FAIL" : "UNVERIFIED";
    }
}

public sealed class UxQualificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private ForestTreeSpawner spawner;
    private ForestPlayer player;
    private StandaloneSessionMenu session;
    private readonly F10DeliveryEvidence f10Delivery = new F10DeliveryEvidence();
    private ForestSaveData original;
    private string output;
    private int shots, passed;
    private readonly List<string> failures = new List<string>();
    private readonly List<string> unverified = new List<string>();
    private readonly List<string> fontRows = new List<string>();

    private static string F(double v, string f = "0.##") => v.ToString(f, Inv);
    private void Hard(string name, bool ok, string detail)
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"UXQ_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Measure(string name, string detail) => Debug.Log($"UXQ_MEASURE {name} {detail}");
    private void Unverified(string detail)
    {
        unverified.Add(detail);
        Debug.Log("UXQ_UNVERIFIED " + detail);
    }
    private void RecordF10Toggle(string name, bool beforeOpen, bool expectedOpen)
    {
        bool afterOpen = StandaloneSessionMenu.IsOpen;
        string result = f10Delivery.Observe(beforeOpen, afterOpen, expectedOpen);
        string detail = $"beforeOpen={beforeOpen} afterOpen={afterOpen} expectedOpen={expectedOpen}; synthetic Editor keys only; native/Player input not exercised";
        if (result == "UNVERIFIED") Unverified(name + " " + detail);
        else Hard(name, result == "PASS", detail);
    }
    private ForestSaveData Clone(ForestSaveData d) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));
    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    // ---------- driver ----------

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
        try { if (original != null) { if (session != null && StandaloneSessionMenu.IsOpen) session.Close(); manager.EndReferencePreview(); saves.LoadData(original, false); } }
        catch (Exception error) { failure = failure ?? error; }
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"UX_QUAL_DONE shots={shots} hardChecks={passed} unverifiedChecks={unverified.Count}" : $"UX_QUAL_FAILED shots={shots} hardChecks={passed} unverifiedChecks={unverified.Count} exception={failure} failures=[{string.Join(" | ", failures)}]");
        Debug.Log("UX_QUAL_DONE_MARKER");   // the run completed; failures above are results, not a crash
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    // ---------- input and UI helpers ----------

    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private IEnumerator LeftClick()
    {
        Mouse mouse = Mouse.current ?? InputSystem.AddDevice<Mouse>();
        InputSystem.QueueStateEvent(mouse, new MouseState().WithButton(UnityEngine.InputSystem.LowLevel.MouseButton.Left)); yield return null; yield return null;
        InputSystem.QueueStateEvent(mouse, new MouseState()); yield return null; yield return null;
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
    private VisualElement SessionRoot() => session.GetComponent<UIDocument>().rootVisualElement;
    private Button Btn(VisualElement root, string text) => root == null ? null : root.Query<Button>().ToList().FirstOrDefault(b => b.text == text && Visible(b));
    private Button BtnStarts(VisualElement root, string text) => root == null ? null : root.Query<Button>().ToList().FirstOrDefault(b => b.text != null && b.text.StartsWith(text, StringComparison.Ordinal) && Visible(b));
    private static string Texts(VisualElement root, bool onlyVisible = true)
        => root == null ? "" : string.Join("\n", root.Query<TextElement>().ToList().Where(t => !onlyVisible || Visible(t)).Select(t => t.text).Where(s => !string.IsNullOrEmpty(s)));
    private static int Words(string s) => Regex.Matches(s ?? "", @"\S+").Count;
    private static int Count(string haystack, string needle) { int n = 0, i = 0; while ((i = haystack.IndexOf(needle, i, StringComparison.Ordinal)) >= 0) { n++; i += needle.Length; } return n; }
    private string Cue() => ui.Learning.Summary;

    private ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce" && !t.TreeId.StartsWith("Q", StringComparison.Ordinal))
        .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    // Stand beside a tree so that the walking aim picks it (the shipped gate positions the player 3 m south of the tree).
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

    // Screenshot at every requested size, plus a measurement of how large the text really is on that screen.
    // everyFrame keeps a transient state alive (an inspected tree); check runs at the end of the frame, once per size, before the capture.
    private IEnumerator Shoot(string state, Action everyFrame = null, Action<Vector2Int> check = null)
    {
        foreach (Vector2Int size in UxQualification.Sizes())
        {
            UxQualification.SetGameSize(size.x, size.y);
            for (int i = 0; i < 6; i++) { everyFrame?.Invoke(); yield return null; }
            everyFrame?.Invoke();
            yield return new WaitForEndOfFrame();
            check?.Invoke(size);
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + state);
            File.WriteAllBytes(Path.Combine(output, state + "-" + size.x + "x" + size.y + ".png"), image.EncodeToPNG());
            Destroy(image);
            shots++;
            MeasureText(state, size);
        }
    }

    // Physical text size: a text element's font size in panel units times the panel scale (screen width / panel width).
    private void MeasureText(string state, Vector2Int size)
    {
        VisualElement root = session != null && (StandaloneSessionMenu.IsOpen || session.StartupScreenActive) ? SessionRoot() : UiRoot();
        float panelWidth = root.worldBound.width;
        if (panelWidth <= 0) return;
        float scale = Screen.width / panelWidth;
        var sizes = new List<float>(); int clipped = 0; var offenders = new List<string>();
        foreach (TextElement t in root.Query<TextElement>().ToList())
        {
            if (!Visible(t) || string.IsNullOrWhiteSpace(t.text)) continue;
            // The numbered Crop Tree tags are anchored to trees in the world and are off screen whenever the tree is behind the camera.
            if (t.ClassListContains("competitor-tag")) continue;
            sizes.Add(t.resolvedStyle.fontSize * scale);
            Rect b = t.worldBound; Rect rb = root.worldBound;
            bool inScroll = false;
            for (VisualElement p = t.parent; p != null; p = p.parent) if (p is ScrollView) { inScroll = true; break; }
            if (!inScroll && (b.xMin < rb.xMin - 2 || b.yMin < rb.yMin - 2 || b.xMax > rb.xMax + 2 || b.yMax > rb.yMax + 2))
            {
                clipped++;
                if (offenders.Count < 4) offenders.Add($"[{Short(t.text)}] .{string.Join(".", t.GetClasses())} {b}");
            }
        }
        if (sizes.Count == 0) return;
        sizes.Sort();
        string row = $"{state},{size.x}x{size.y},scale={F(scale, "0.000")},texts={sizes.Count},minPx={F(sizes[0], "0.0")},medianPx={F(sizes[sizes.Count / 2], "0.0")},under10px={sizes.Count(s => s < 10f)},under12px={sizes.Count(s => s < 12f)},outsideViewportNotScrolled={clipped}";
        fontRows.Add(row);
        Measure("TEXT_SIZE", row);
        if (size.x >= 1280) Hard($"NO_TEXT_OUTSIDE_THE_VIEWPORT_{state}_{size.x}x{size.y}", clipped == 0, $"outsideViewportNotScrolled={clipped} {string.Join(" ; ", offenders)}");
    }

    // Runs one phase (flattening its nested coroutines) and turns an exception into a recorded failure, then puts the UI back to a neutral
    // state so the next phase starts from walking mode.
    private IEnumerator Safe(string name, IEnumerator root)
    {
        var stack = new Stack<IEnumerator>(); stack.Push(root);
        while (stack.Count > 0)
        {
            object current = null; bool more = false; string error = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception ex)
            {
                string frame = (ex.StackTrace ?? "").Split('\n').FirstOrDefault(l => l.Contains("UxQualification.cs")) ?? "";
                error = ex.GetType().Name + ": " + ex.Message + " | " + frame.Trim();
            }
            if (error != null) { Hard("PHASE_" + name + "_RAN_TO_THE_END", false, error); break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        if (session != null && StandaloneSessionMenu.IsOpen) session.Close();
        if (player != null && player.IsInspecting) { yield return Press(Key.Escape); }
        ui.CloseAll(); ui.CloseHelp();
        for (int i = 0; i < 3; i++) yield return null;
    }

    // ---------- the storyline ----------

    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; ecology = ui.Ecology; player = ui.Player; marks = ui.Marking;
        saves = FindFirstObjectByType<ForestSaveController>(); spawner = FindFirstObjectByType<ForestTreeSpawner>(); session = FindFirstObjectByType<StandaloneSessionMenu>();
        if (Application.isBatchMode) throw new InvalidOperationException("UxQualification needs a graphical Editor");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/UxQualification/evidence" : diag, "ui");
        Directory.CreateDirectory(output);
        original = saves.CaptureData();
        Measure("START", $"geometry={ecology.StandGeometryModelVersion} growthModel={ecology.GrowthModelVersion} year={ecology.EcologicalYear} screen={Screen.width}x{Screen.height} sizes={string.Join(",", UxQualification.Sizes())}");

        string only = Environment.GetEnvironmentVariable("CCF_UXQ_PHASES");
        string[] selectedPhases = UxQualificationPhases.Select(only);
        bool Want(string n) => selectedPhases.Contains(n);
        if (Want("ROLE")) yield return Safe("ROLE", Role());
        if (Want("FIRST_CYCLE")) yield return Safe("FIRST_CYCLE", FirstCycle());
        if (Want("GUARD_VARIANTS")) yield return Safe("GUARD_VARIANTS", GuardVariants());
        if (Want("RESET_FLOW")) yield return Safe("RESET_FLOW", ResetFlow());
        if (Want("CONTROLS_AND_TERMINOLOGY")) yield return Safe("CONTROLS_AND_TERMINOLOGY", ControlsAndTerminology());
        if (Want("MODEL_AWARE_COPY")) yield return Safe("MODEL_AWARE_COPY", ModelAwareCopy());
        if (Want("INSPECTION_CASES")) yield return Safe("INSPECTION_CASES", InspectionCases());
        if (Want("DEADWOOD_LINE")) yield return Safe("DEADWOOD_LINE", DeadwoodLine());
        if (Want("INPUT_SMOKE")) yield return Safe("INPUT_SMOKE", InputSmoke());
        if (Want("F10_INPUT") && !Want("INPUT_SMOKE")) yield return Safe("F10_INPUT", SessionMenuInput());
        if (Want("PRESERVATION")) yield return Safe("PRESERVATION", Preservation());
        if (Want("PERF_SANITY")) yield return Safe("PERF_SANITY", PerfSanity());
        if (Want("PERF_CPU_ONLY")) yield return Safe("PERF_CPU_ONLY", PerfCpuOnly());
        if (Want("STARTUP_CONTROLS")) yield return Safe("STARTUP_CONTROLS", StartupControls());
        File.WriteAllLines(Path.Combine(output, "text-sizes.csv"), new[] { "state,size,scale,texts,minPx,medianPx,under10px,under12px,outside" }.Concat(fontRows));
    }

    // ---- section 4/10: the first screen a new player sees ----
    private IEnumerator Role()
    {
        Hard("ROLE_FRESH_PLAYER_SEES_THE_WALKING_HUD_INTRODUCTION", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu}");
        string text = Texts(ui.Help.Root);
        int titles = Count(text, "Your role · Scenario One");
        Hard("ROLE_TITLE_APPEARS_ONCE", titles == 1, $"occurrences={titles}");
        string body = ui.Help.Root.Q<Label>("menu-help-text")?.text ?? "";
        Measure("ROLE_BODY_WORDS", $"words={Words(body)} (the patch states about 101)");
        Hard("ROLE_PANEL_HAS_MORE_DETAIL_BUTTON", Btn(ui.Help.Root, "More detail") != null || ui.Help.Root.Q<Button>("menu-help-more") != null, "button 'More detail' / menu-help-more");
        yield return Shoot("01-role-first-use");
        Button more = ui.Help.Root.Q<Button>("menu-help-more");
        yield return Invoke(more);
        string full = Texts(ui.Help.Root);
        Hard("ROLE_MORE_DETAIL_OPENS_THE_FULL_HELP", ui.Help.IsOpen && Words(full) > 300 && full.Contains("Keys:"), $"words={Words(full)} hasKeysLine={full.Contains("Keys:")}");
        yield return Shoot("02-full-help-more-detail");
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        Hard("ROLE_CONTINUE_CLOSES_AND_DOES_NOT_REOPEN", !ui.Help.IsOpen, $"open={ui.Help.IsOpen}");
        for (int i = 0; i < 60; i++) yield return null;
        Hard("ROLE_PANEL_DOES_NOT_REOPEN_ON_ITS_OWN", !ui.Help.IsOpen && player.enabled, $"open={ui.Help.IsOpen} playerEnabled={player.enabled}");
        yield return Press(Key.F1);
        Hard("ROLE_F1_REOPENS_THE_FULL_HELP", ui.Help.IsOpen && Texts(ui.Help.Root).Contains("Keys:"), $"open={ui.Help.IsOpen}");
        yield return Press(Key.Escape);
        Hard("ROLE_ESC_CLOSES_HELP", !ui.Help.IsOpen, "");
    }

    // ---- section 5/6/7: the first real cycle, in the order the cue teaches it ----
    private readonly List<string> cues = new List<string>();
    private void LogCue(string after) { string c = Cue(); cues.Add(c); Measure("CUE", $"after=[{after}] cue=[{c}]"); }

    // Lesson observations run every 0.25 s; keep the current UI state across at least one observation tick.
    private static IEnumerator WaitRealtime(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }

    private bool InspectionTargetReady(ForestTree tree)
        => tree != null && tree.IsLiving && player.isActiveAndEnabled && !player.IsInspecting
            && player.AimedTree == tree && UnityEngine.Cursor.lockState == CursorLockMode.Locked
            && !ui.Help.IsOpen && !manager.AnyPanelOpen && !StandaloneSessionMenu.IsOpen
            && Application.isFocused && EditorWindow.focusedWindow?.GetType().Name == "GameView";

    private string InspectionState(ForestTree tree)
    {
        string Id(ForestTree t) => t == null ? "none" : t.TreeId;
        Camera camera = Camera.main;
        string rays = camera == null ? "no-camera" : string.Join(";", Physics.RaycastAll(camera.transform.position, camera.transform.forward, 8f)
            .OrderBy(h => h.distance).Take(12).Select(h => $"{h.collider.name}:{Id(h.collider.GetComponentInParent<ForestTree>())}@{F(h.distance, "0.000")}"));
        return $"frame={Time.frameCount} realtime={F(Time.realtimeSinceStartupAsDouble, "0.000")} intended={Id(tree)} "
            + $"candidate={Id(player.AimedTree)} marking={Id(marks.AimedTree)} inspected={Id(player.InspectedTree)} "
            + $"player={player.transform.position.ToString("F4")} camera={(camera == null ? "none" : camera.transform.position.ToString("F4"))} "
            + $"forward={(camera == null ? "none" : camera.transform.forward.ToString("F4"))} "
            + $"targetPointDistance={(camera != null && tree != null ? F(Vector3.Distance(camera.transform.position, tree.InteractionPoint), "0.000") : "none")} "
            + $"enabled={player.isActiveAndEnabled} inspecting={player.IsInspecting} cursor={UnityEngine.Cursor.lockState} "
            + $"focused={Application.isFocused}/{EditorWindow.focusedWindow?.GetType().Name} help={ui.Help.IsOpen}/{ui.Help.CurrentMenu} "
            + $"screen={ui.CurrentScreen} panel={manager.AnyPanelOpen} session={StandaloneSessionMenu.IsOpen} "
            + $"keyboard={Keyboard.current?.deviceId} prompt=[{player.InteractionPromptText()}] rays=[{rays}]";
    }

    private IEnumerator EnterInspection(ForestTree tree)
    {
        // Planning reaches 8 m; inspection has its own shorter raycast. Require the player's actual candidate.
        double started = Time.realtimeSinceStartupAsDouble, deadline = started + 2.0;
        int stableFrames = 0;
        while (stableFrames < 3 && Time.realtimeSinceStartupAsDouble < deadline)
        {
            yield return null;
            stableFrames = InspectionTargetReady(tree) && Keyboard.current != null && !Keyboard.current.eKey.isPressed
                ? stableFrames + 1 : 0;
        }
        bool acquired = stableFrames == 3;
        Hard("FIRST_CYCLE_INSPECTION_TARGET_ACQUIRED", acquired,
            $"stableFrames={stableFrames} waitedSeconds={F(Time.realtimeSinceStartupAsDouble - started, "0.000")} " + InspectionState(tree));
        if (!acquired) throw new InvalidOperationException("inspection target acquisition timed out; E was not emitted");

        bool delivered = false, readyAtDelivery = false;
        int deliveryEdges = 0;
        void ObserveE()
        {
            if (Keyboard.current == null) return;
            Measure("INSPECTION_INPUT_UPDATE", $"update={InputState.currentUpdateType} eDown={Keyboard.current.eKey.isPressed} "
                + $"eEdge={Keyboard.current.eKey.wasPressedThisFrame} escEdge={Keyboard.current.escapeKey.wasPressedThisFrame} " + InspectionState(tree));
            if (InputState.currentUpdateType != InputUpdateType.Dynamic || !Keyboard.current.eKey.wasPressedThisFrame) return;
            delivered = true;
            deliveryEdges++;
            readyAtDelivery |= InspectionTargetReady(tree);
            Measure("INSPECTION_E_DYNAMIC_EDGE", $"ready={InspectionTargetReady(tree)} " + InspectionState(tree));
        }
        InputSystem.onAfterUpdate += ObserveE;
        try
        {
            Measure("INSPECTION_E_REQUEST", InspectionState(tree));
            yield return Press(Key.E); // One unchanged synthetic press; never retry a delivered interaction.
            deadline = Time.realtimeSinceStartupAsDouble + 2.0;
            while (!(player.IsInspecting && player.InspectedTree == tree) && Time.realtimeSinceStartupAsDouble < deadline)
                yield return null;
            Hard("FIRST_CYCLE_E_DELIVERED_TO_READY_TARGET", delivered && readyAtDelivery,
                $"dynamicEdge={delivered} readyAtDelivery={readyAtDelivery} " + InspectionState(tree));
            bool entered = player.IsInspecting && player.InspectedTree == tree;
            Measure("INSPECTION_E_RESULT", $"entered={entered} dynamicEdge={delivered} readyAtDelivery={readyAtDelivery} edges={deliveryEdges} " + InspectionState(tree));
            if (!entered || !delivered || !readyAtDelivery)
                throw new InvalidOperationException(!delivered ? "INPUT_NOT_DELIVERED: no Dynamic E edge; see interaction diagnostics"
                    : !readyAtDelivery ? "NO_VALID_TARGET_AT_E_DELIVERY: see interaction diagnostics"
                    : "VALID_TARGET_AND_E_DELIVERED_WITHOUT_INSPECTION: see interaction diagnostics");
        }
        finally { InputSystem.onAfterUpdate -= ObserveE; }
    }

    private IEnumerator FirstCycle()
    {
        // ROLE normally closes the first-use role panel; the cycle cannot aim at a tree while it is open, so it also runs alone.
        if (ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud) { ui.CloseHelp(); yield return null; yield return null; }
        yield return WaitRealtime(0.35f);
        LogCue("fresh");
        // Rack-first revision: the opening cue is the rack lesson; inspection is cued once the racks are open.
        Hard("CUE_STARTS_WITH_CREATE_YOUR_FIRST_RACK", cues[0].Contains("Create your first rack"), cues[0]);
        Hard("LEARNING_STEP_IDS_ARE_48_THE_42_SAVED_ONES_PLUS_SIX_RACK_STEPS", LearningObjectivesView.StepIds.Count() == 48, $"count={LearningObjectivesView.StepIds.Count()} sortedIdHash={Fnv(string.Join("|", LearningObjectivesView.StepIds.OrderBy(s => s, StringComparer.Ordinal)))}");

        ForestTree crop = null;
        yield return AimAt(t => true, t => crop = t);
        Hard("FIRST_CYCLE_CAN_AIM_AT_A_TREE", crop != null, crop != null ? crop.TreeId : "no tree could be aimed at");
        yield return EnterInspection(crop);
        Hard("E_INSPECTS_THE_AIMED_TREE", player.IsInspecting && player.InspectedTree == crop, InspectionState(crop));
        bool introOpened = ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeInspection;
        Hard("TREE_INSPECTION_FIRST_USE_SCREEN_OPENS_ONCE", introOpened, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu}");
        Measure("TREE_INSPECTION_FIRST_USE_WORDS", $"words={Words(ui.Help.Root.Q<Label>("menu-help-text")?.text)} (the patch states 65)");
        yield return Shoot("03-tree-inspection-first-use");
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        yield return Shoot("04-tree-inspection-card");
        yield return WaitRealtime(0.35f);
        LogCue("inspected a tree");
        yield return Press(Key.C);
        Hard("C_MARKS_A_CROP_TREE", marks.GetCropTreeIds().Contains(crop.TreeId), $"cropTrees={marks.GetCropTreeIds().Count}");
        yield return WaitRealtime(0.35f);
        LogCue("pressed C");
        yield return Press(Key.E);                // leave inspection
        ForestTree fell = null;
        yield return AimAt(t => t != crop, t => fell = t);
        yield return Press(Key.X);
        Hard("X_MARKS_THE_AIMED_TREE_FOR_FELLING", fell != null && fell.IsMarkedForFell, fell != null ? fell.TreeId : "no second tree could be aimed at");
        yield return WaitRealtime(0.35f);
        LogCue("pressed X");

        yield return Press(Key.Tab);
        Hard("TAB_OPENS_THE_WORK_PLAN", manager.AnyPanelOpen && !player.enabled, $"panel={manager.AnyPanelOpen} playerEnabled={player.enabled}");
        bool planIntro = ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WorkPlan;
        Hard("WORK_PLAN_FIRST_USE_SCREEN_OPENS", planIntro, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu}");
        Measure("WORK_PLAN_FIRST_USE_WORDS", $"words={Words(ui.Help.Root.Q<Label>("menu-help-text")?.text)} (the patch states 50)");
        yield return Shoot("05-work-plan-first-use");
        if (ui.Help.IsOpen) yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        yield return Shoot("06-work-plan-pending");
        yield return WaitRealtime(0.35f);
        LogCue("opened the Work Plan");

        // --- the wasted-year guard, pending only (section 6a/6b) ---
        int year0 = ecology.EcologicalYear;
        Button advance = BtnStarts(UiRoot(), "Advance one year");
        Hard("GUARD_ADVANCE_BUTTON_IS_AVAILABLE", advance != null && advance.enabledInHierarchy, advance == null ? "missing" : "enabled=" + advance.enabledInHierarchy);
        yield return Invoke(advance);
        VisualElement card = UiRoot().Q<VisualElement>("advance-warning");
        Hard("GUARD_PENDING_ONLY_DOES_NOT_ADVANCE_AND_SHOWS_THE_CARD", ecology.EcologicalYear == year0 && card != null && Visible(card), $"year={ecology.EcologicalYear} (was {year0}) cardVisible={(card != null && Visible(card))}");
        string cardText = card != null ? Texts(card) : "";
        Hard("GUARD_CARD_SAYS_PLANNED_WORK_IS_NOT_APPROVED_AND_OFFERS_THREE_CHOICES", cardText.Contains("not been approved") && Btn(card, AdvanceGuard.ReturnLabel) != null && Btn(card, AdvanceGuard.ApproveLabel) != null && Btn(card, AdvanceGuard.AdvanceAnywayLabel) != null, cardText.Replace("\n", " / "));
        yield return Shoot("07-pending-work-warning-card");
        yield return Invoke(Btn(card, AdvanceGuard.ReturnLabel));
        Hard("GUARD_BACK_TO_THE_PLAN_HIDES_THE_CARD_KEEPS_THE_ORDER_AND_THE_YEAR", !Visible(card) && ecology.EcologicalYear == year0 && manager.WorkOrders.Any(o => o.status == ScenarioWorkStatus.Pending), $"cardVisible={Visible(card)} year={ecology.EcologicalYear} pending={manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Pending)}");
        yield return Invoke(BtnStarts(UiRoot(), "Advance one year"));
        Hard("GUARD_CARD_RETURNS_ON_A_SECOND_ADVANCE", Visible(card), "");
        yield return Invoke(Btn(card, AdvanceGuard.ApproveLabel));
        Hard("GUARD_APPROVE_PENDING_WORK_NEEDS_THE_PLAYERS_CLICK_AND_THEN_APPROVES", !Visible(card) && manager.WorkOrders.Any(o => o.status == ScenarioWorkStatus.Approved) && ecology.EcologicalYear == year0, $"approved={manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Approved)} year={ecology.EcologicalYear}");
        yield return WaitRealtime(0.35f);
        LogCue("approved the plan");

        // --- the real advance with approved work: the first Annual Review (section 7) ---
        yield return Invoke(BtnStarts(UiRoot(), "Advance one year"));
        Hard("ADVANCE_WITH_APPROVED_WORK_NEEDS_NO_QUESTION_AND_PASSES_THE_YEAR", ecology.EcologicalYear == year0 + 1, $"year={ecology.EcologicalYear}");
        for (int i = 0; i < 6; i++) yield return null;
        Hard("FIRST_ANNUAL_REVIEW_OPENS_WITHOUT_A_HELP_MODAL", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && !ui.Help.IsOpen && !manager.AnnualReviewSeen, $"screen={ui.CurrentScreen} helpOpen={ui.Help.IsOpen} seen={manager.AnnualReviewSeen}");
        Button ack = UiRoot().Q<Button>("annual-review-acknowledge");
        string reviewText = Texts(UiRoot());
        Hard("ANNUAL_REVIEW_HAS_ITS_READING_GUIDANCE_INLINE_BESIDE_THE_ACKNOWLEDGEMENT", ack != null && Visible(ack) && ack.parent.Query<Label>().ToList().Any(l => l.text == MenuHelpView.AnnualReviewReadPrompt), ack == null ? "no acknowledge button" : "prompt present=" + ack.parent.Query<Label>().ToList().Any(l => l.text == MenuHelpView.AnnualReviewReadPrompt));
        Hard("ANNUAL_REVIEW_SHOWS_WORK_DONE_MONEY_FOREST_AND_THE_FIRST_INTERVENTION_PROMPT", reviewText.Contains("WORK DONE") && reviewText.Contains("MONEY") && reviewText.Contains("FOREST") && reviewText.Contains("first intervention"), "");
        yield return Shoot("08-annual-review-first");
        yield return Press(Key.F1);
        Hard("REVIEW_F1_OPENS_HELP_FOR_THE_REVIEW", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.AnnualReview, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu}");
        yield return Shoot("09-annual-review-help-f1");
        yield return Press(Key.Escape);
        Hard("REVIEW_ESC_CLOSES_HELP_WITHOUT_CLOSING_THE_REVIEW", !ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && !manager.AnnualReviewSeen, $"helpOpen={ui.Help.IsOpen} screen={ui.CurrentScreen} seen={manager.AnnualReviewSeen}");
        yield return WaitRealtime(0.35f);
        LogCue("first review open");
        yield return Invoke(ack);
        Hard("ACKNOWLEDGEMENT_IS_THE_ONE_HARD_GATE_AND_CLEARS_IT", manager.AnnualReviewSeen && !ui.NeedsAnnualReview, $"seen={manager.AnnualReviewSeen}");
        yield return WaitRealtime(0.35f);
        LogCue("acknowledged the review");
        ui.CloseAll(); yield return null; yield return null;
        yield return Shoot("10-hud-after-first-cycle");

        // The cue order: each new cue must come LATER in the packet's order than the one before; the map comes last.
        // Rack-first revision: this storyline marks trees by hand (selective marking still works in every state), so the lesson cue
        // never leaves the rack lesson: selective marking is not what the opening teaches, and the map is never first.
        bool mapEarly = cues.Take(cues.Count - 1).Any(c => c.Contains("Open the map"));
        Hard("CUE_STAYS_ON_THE_RACK_LESSON_UNTIL_A_RACK_IS_PLANNED", cues.All(c => c.Contains("Create your first rack")), $"cues={string.Join(" || ", cues)}");
        Hard("CUE_IS_NOT_MAP_FIRST", !mapEarly, $"cues={string.Join(" || ", cues)}");
    }

    private static string Fnv(string s)
    {
        ulong h = 14695981039346656037UL;
        foreach (byte b in System.Text.Encoding.UTF8.GetBytes(s)) { h ^= b; h *= 1099511628211UL; }
        return h.ToString("X16");
    }

    // ---- section 6: the remaining guard cases ----
    private IEnumerator GuardVariants()
    {
        // (f) nothing planned: no question
        manager.AdvanceYear();
        yield return null;
        ui.CloseAll(); manager.MarkAnnualReviewSeen();
        ui.ShowWorkPlan(); yield return null; yield return null;
        int y = ecology.EcologicalYear;
        Hard("GUARD_NO_PENDING_NO_CARD_AND_THE_YEAR_PASSES", manager.WorkOrders.All(o => !o.IsOpen), $"open orders={manager.WorkOrders.Count(o => o.IsOpen)}");
        yield return Invoke(BtnStarts(UiRoot(), "Advance one year"));
        VisualElement card = UiRoot().Q<VisualElement>("advance-warning");
        Hard("GUARD_NO_PENDING_ADVANCE_IS_IMMEDIATE", ecology.EcologicalYear == y + 1 && (card == null || !Visible(card)), $"year={ecology.EcologicalYear} (was {y})");
        ui.CloseAll(); manager.MarkAnnualReviewSeen(); yield return null;

        // (c) pending only -> Advance anyway: the year passes, the order stays pending
        ForestTree t1 = null; yield return AimAt(tr => !marks.GetCropTreeIds().Contains(tr.TreeId), tr => t1 = tr);
        yield return Press(Key.X);
        yield return Press(Key.Tab);
        if (ui.Help.IsOpen) yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        y = ecology.EcologicalYear;
        yield return Invoke(BtnStarts(UiRoot(), "Advance one year"));
        card = UiRoot().Q<VisualElement>("advance-warning");
        Hard("GUARD_CARD_AGAIN_FOR_NEW_PENDING_WORK", card != null && Visible(card) && ecology.EcologicalYear == y, $"cardVisible={(card != null && Visible(card))}");
        // (d) reopening the Work Plan after the card was shown: the old question must be gone
        ui.CloseAll(); yield return null; ui.ShowWorkPlan(); yield return null; yield return null;
        card = UiRoot().Q<VisualElement>("advance-warning");
        Hard("GUARD_REOPENING_THE_WORK_PLAN_DROPS_THE_OLD_QUESTION", card == null || !Visible(card), $"cardVisible={(card != null && Visible(card))}");
        yield return Invoke(BtnStarts(UiRoot(), "Advance one year"));
        card = UiRoot().Q<VisualElement>("advance-warning");
        yield return Invoke(Btn(card, AdvanceGuard.AdvanceAnywayLabel));
        Hard("GUARD_ADVANCE_ANYWAY_PASSES_THE_YEAR_AND_KEEPS_THE_ORDER_PENDING", ecology.EcologicalYear == y + 1 && manager.WorkOrders.Any(o => o.status == ScenarioWorkStatus.Pending), $"year={ecology.EcologicalYear} (was {y}) pending={manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Pending)}");
        Measure("GUARD_AFTER_ANYWAY", $"feedback=[{manager.Feedback}] screen={ui.CurrentScreen}");
        ui.CloseAll(); manager.MarkAnnualReviewSeen(); yield return null;

        // (e) some approved + some pending: no question, the approved work is done, the rest waits
        ForestTree t2 = null; yield return AimAt(tr => !marks.GetCropTreeIds().Contains(tr.TreeId) && !tr.IsMarkedForFell, tr => t2 = tr);
        ui.ShowWorkPlan(); yield return null; yield return null;
        manager.ApprovePendingWork();                  // approves the older pending order only
        yield return null;
        ui.CloseAll();
        ForestTree t3 = null; yield return AimAt(tr => !marks.GetCropTreeIds().Contains(tr.TreeId) && !tr.IsMarkedForFell && manager.WorkOrders.All(o => o.targetTreeId != tr.TreeId), tr => t3 = tr);
        yield return Press(Key.X);                     // a new mark on another tree, still pending
        ui.ShowWorkPlan(); yield return null; yield return null;
        Hard("GUARD_MIXED_STATE_HAS_APPROVED_AND_PENDING_ORDERS", manager.WorkOrders.Any(o => o.status == ScenarioWorkStatus.Approved) && manager.WorkOrders.Any(o => o.status == ScenarioWorkStatus.Pending && string.IsNullOrEmpty(o.validationMessage)),
            $"approved={manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Approved)} pending={manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Pending)}");
        y = ecology.EcologicalYear;
        yield return Invoke(BtnStarts(UiRoot(), "Advance one year"));
        card = UiRoot().Q<VisualElement>("advance-warning");
        Hard("GUARD_MIXED_STATE_ASKS_NOTHING_AND_ADVANCES", ecology.EcologicalYear == y + 1 && (card == null || !Visible(card)), $"year={ecology.EcologicalYear} (was {y})");
        Measure("GUARD_MIXED_STATE_NOTE", $"pending orders still waiting after the year: {manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Pending)} (by the patch's own design they wait silently when some work is approved)");
        ui.CloseAll(); manager.MarkAnnualReviewSeen(); yield return null;
    }

    // The Editor scene has no StandaloneSessionMenu until something creates one; creating it opens it, so close it again, as the shipped
    // gates do. After that F10 toggles it through its own Update.
    private IEnumerator EnsureSession()
    {
        if (session == null) session = FindFirstObjectByType<StandaloneSessionMenu>();
        if (session != null) yield break;
        session = new GameObject("Session controls under test").AddComponent<StandaloneSessionMenu>();
        for (int i = 0; i < 4; i++) yield return null;
        if (StandaloneSessionMenu.IsOpen) session.Close();
        for (int i = 0; i < 3; i++) yield return null;
    }

    // ---- section 4: the reset flow ----
    private IEnumerator ResetFlow()
    {
        yield return EnsureSession();
        session.Close(); ui.CloseAll(); ui.CloseHelp(); yield return null;
        saves.Save();                                           // a real save in the isolated config folder
        string savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        string saveBefore = File.Exists(savePath) ? Fnv(File.ReadAllText(savePath)) : "none";
        int helpSeen = Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Count(MenuHelpView.Introduced);
        int lessons = ui.Learning.CompletedSteps;
        string worldBefore = WorldHash();
        int yearBefore = ecology.EcologicalYear, treesBefore = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length;
        string objectivesBefore = string.Join(";", manager.Objectives.Select(o => ScenarioOneUiFacts.ObjectiveLine(o)));
        bool reviewSeenBefore = manager.AnnualReviewSeen;
        Hard("RESET_PRECONDITION_SOMETHING_TO_RESET", helpSeen >= 3 && lessons >= 4 && reviewSeenBefore && saveBefore != "none", $"helpScreensSeen={helpSeen} lessonSteps={lessons} reviewAcknowledged={reviewSeenBefore} saveFile={saveBefore}");

        bool beforeF10 = StandaloneSessionMenu.IsOpen;
        yield return Press(Key.F10);
        RecordF10Toggle("F10_OPENS_THE_SESSION_MENU", beforeF10, true);
        if (!StandaloneSessionMenu.IsOpen) session.Open();
        Hard("SESSION_MENU_OPEN_FOR_RESET", StandaloneSessionMenu.IsOpen, "mechanical state, not native input evidence");
        Measure("SESSION_MENU_TEXT", Texts(SessionRoot()).Replace("\n", " / "));
        yield return Shoot("11-session-menu");
        yield return Invoke(BtnStarts(SessionRoot(), "Reset tutorial/help progress"));
        string prompt = Texts(SessionRoot());
        Hard("RESET_ASKS_FOR_CONFIRMATION_AND_SAYS_THE_FOREST_IS_NOT_CHANGED", prompt.Contains("Reset tutorial and help progress") && prompt.Contains("not changed") && Btn(SessionRoot(), "Confirm") != null && Btn(SessionRoot(), "Cancel") != null, prompt.Replace("\n", " / "));
        yield return Shoot("12-reset-confirmation");
        yield return Invoke(Btn(SessionRoot(), "Cancel"));
        int helpAfterCancel = Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Count(MenuHelpView.Introduced);
        Hard("RESET_CANCEL_CHANGES_NOTHING", helpAfterCancel == helpSeen && ui.Learning.CompletedSteps == lessons && WorldHash() == worldBefore && !Texts(SessionRoot()).Contains("progress reset on this device") && StandaloneSessionMenu.IsOpen,
            $"helpSeen {helpSeen}->{helpAfterCancel} lessons {lessons}->{ui.Learning.CompletedSteps} worldUnchanged={WorldHash() == worldBefore} menuStillOpen={StandaloneSessionMenu.IsOpen}");

        yield return Invoke(BtnStarts(SessionRoot(), "Reset tutorial/help progress"));
        yield return Invoke(Btn(SessionRoot(), "Confirm"));
        int helpAfter = Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Count(MenuHelpView.Introduced);
        string saveAfter = File.Exists(savePath) ? Fnv(File.ReadAllText(savePath)) : "none";
        Hard("RESET_CONFIRM_CLEARS_HELP_AND_LESSON_PROGRESS", helpAfter == 0 && ui.Learning.CompletedSteps == 0 && Cue().Contains("Create your first rack"), $"helpSeen {helpSeen}->{helpAfter} lessons {lessons}->{ui.Learning.CompletedSteps} cue=[{Cue()}]");
        string objectivesAfter = string.Join(";", manager.Objectives.Select(o => ScenarioOneUiFacts.ObjectiveLine(o)));
        Hard("RESET_CONFIRM_KEEPS_FOREST_SAVE_OBJECTIVES_AND_REVIEW_ACKNOWLEDGEMENT", WorldHash() == worldBefore && saveAfter == saveBefore && objectivesAfter == objectivesBefore && manager.AnnualReviewSeen == reviewSeenBefore,
            $"worldUnchanged={WorldHash() == worldBefore} saveFileUnchanged={saveAfter == saveBefore} objectivesUnchanged={objectivesAfter == objectivesBefore} reviewAcknowledged={manager.AnnualReviewSeen}");
        Hard("RESET_IS_NOT_A_NEW_GAME", ecology.EcologicalYear == yearBefore && FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length == treesBefore, $"year={ecology.EcologicalYear} (was {yearBefore})");
        Hard("RESET_SHOWS_A_NOTE_THAT_THE_FOREST_AND_SAVE_WERE_NOT_CHANGED", Texts(SessionRoot()).Contains("Tutorial and help progress reset on this device") && Texts(SessionRoot()).Contains("not changed"), Texts(SessionRoot()).Replace("\n", " / "));
        yield return Shoot("13-session-menu-after-reset");

        beforeF10 = StandaloneSessionMenu.IsOpen;
        yield return Press(Key.F10);
        RecordF10Toggle("MENU_CLOSES_WITH_F10", beforeF10, false);
        if (StandaloneSessionMenu.IsOpen) session.Close();
        Hard("SESSION_MENU_CLOSED_AFTER_RESET", !StandaloneSessionMenu.IsOpen, "mechanical state, not native input evidence");
        for (int i = 0; i < 20; i++) yield return null;
        Hard("AFTER_RESET_THE_ROLE_PANEL_OPENS_ONCE", ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud && Count(Texts(ui.Help.Root), "Your role · Scenario One") == 1, $"open={ui.Help.IsOpen} menu={ui.Help.CurrentMenu} titles={Count(Texts(ui.Help.Root), "Your role · Scenario One")}");
        yield return Shoot("14-role-panel-after-reset");
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-more"));
        Hard("AFTER_RESET_MORE_DETAIL_OPENS_THE_FULL_HELP", Texts(ui.Help.Root).Contains("Keys:"), "");
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-close"));
        for (int i = 0; i < 60; i++) yield return null;
        Hard("AFTER_RESET_THE_PANEL_DOES_NOT_REOPEN", !ui.Help.IsOpen, "");
        yield return Press(Key.F1);
        Hard("AFTER_RESET_F1_REOPENS_THE_FULL_HELP", ui.Help.IsOpen, "");
        yield return Press(Key.Escape);
    }

    // ---- section 8 ----
    private IEnumerator ControlsAndTerminology()
    {
        yield return Press(Key.F1);
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-more"));
        string full = Texts(ui.Help.Root, false);
        Hard("FULL_HELP_CONTAINS_THE_SHARED_KEYS_LINE", full.Contains(ControlsReference.KeysLine), "");
        foreach (string token in new[] { "left-click", "mark for felling", "Crop Tree", "1–5", "planting", "clearance", "Work Plan", "map" })
            Hard("FULL_HELP_MENTIONS_" + token.Replace(' ', '_').ToUpperInvariant(), full.IndexOf(token, StringComparison.OrdinalIgnoreCase) >= 0, "");
        yield return Shoot("15-full-help-with-keys");
        yield return Press(Key.Escape);

        // terminology on the screens that carry it
        ForestTree t = null; yield return AimAt(tr => true, x => t = x);
        yield return Press(Key.E);
        if (ui.Help.IsOpen) ui.CloseHelp();
        string card = Texts(UiRoot());
        Hard("INSPECTION_SAYS_COMPETITION_INDEX_NOT_JUST_CI", card.IndexOf("competition index", StringComparison.OrdinalIgnoreCase) >= 0 || card.IndexOf("competition", StringComparison.OrdinalIgnoreCase) >= 0, "");
        Measure("INSPECTION_CI_TOKEN", $"standalone 'CI' tokens on the card: {Regex.Matches(card, @"\bCI\b").Count}");
        yield return Press(Key.E);
        ui.ShowWorkPlan(); yield return null; yield return null;
        string plan = Texts(UiRoot(), false);
        Hard("WORK_PLAN_EXPLAINS_BASAL_AREA_AND_THE_RESIDUAL_STAND", plan.IndexOf("basal area", StringComparison.OrdinalIgnoreCase) >= 0 && (plan.IndexOf("residual", StringComparison.OrdinalIgnoreCase) >= 0 || plan.Contains("WHAT YOU ARE LEAVING")), $"hasBasalArea={plan.IndexOf("basal area", StringComparison.OrdinalIgnoreCase) >= 0} hasResidual={plan.IndexOf("residual", StringComparison.OrdinalIgnoreCase) >= 0}");
        yield return Shoot("16-work-plan-leaving-card");
        ui.CloseAll(); ui.CloseHelp();
        for (int i = 0; i < 12; i++) yield return null;
        string hud = Texts(UiRoot(), false);
        Hard("HUD_SAYS_DEER_BROWSING", hud.Contains("Deer browsing"), Regex.Match(hud, @"[^\n]*rowsing[^\n]*").Value);
    }

    // ---- section 9 ----
    private IEnumerator ModelAwareCopy()
    {
        TreeSpeciesDefinition oak = spawner.ResolveSpecies("sessile-oak");
        Vector3 spot = Trees()[0].transform.position + new Vector3(5, 0, 0);
        ForestTree o = spawner.Spawn("QOAK1", oak, spot, 80, 50f, 24f, oak.PotentialCrownRadiusM(50f));
        ecology.InvalidateCompetition(); ecology.RecomputeCanopy(); yield return null;
        int model = ecology.GrowthModelVersion;
        foreach (int m in new[] { 1, 2 })
        {
            ecology.GrowthModelVersion = m;
            ui.Learning.Refresh(true);
            string lessons = Texts(ui.Learning.Root, false);
            bool onlySitka = lessons.Contains("only Sitka spruce earns timber income"), prices = lessons.Contains("oak and beech stems have timber prices");
            Hard($"MODEL{m}_LESSON_COPY_MATCHES_THE_MARKET", m == 1 ? onlySitka && !prices : prices && !onlySitka, $"onlySitkaSentence={onlySitka} broadleafPricesSentence={prices}");
            marks.Mark(o, TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
            ui.ShowWorkPlan(); yield return null; yield return null; yield return null;
            string plan = Texts(UiRoot(), false);
            bool staleNoMarket = plan.IndexOf("no broadleaf", StringComparison.OrdinalIgnoreCase) >= 0 || plan.IndexOf("no sale income", StringComparison.OrdinalIgnoreCase) >= 0 || plan.IndexOf("earn no sale", StringComparison.OrdinalIgnoreCase) >= 0 || plan.IndexOf("no timber market", StringComparison.OrdinalIgnoreCase) >= 0;
            Measure($"MODEL{m}_WORK_PLAN_HARVEST_TEXT", $"mentionsNoBroadleafIncome={staleNoMarket} excerpt=[{ExcerptAround(plan, new[] { "Oak", "oak", "broadleaf", "Beech" })}]");
            Hard($"MODEL{m}_WORK_PLAN_DOES_NOT_CONTRADICT_THE_ACTIVE_MARKET", m == 1 ? true : !staleNoMarket, $"staleNoMarketClaim={staleNoMarket}");
            if (m == 2) yield return Shoot("17-work-plan-oak-model2");
            if (m == 1) yield return Shoot("17b-work-plan-oak-model1");
            marks.Mark(o, TreeMarkType.Fell, false);   // unmark again
            manager.AddMarkedTreesToWorkPlan();
            foreach (var order in manager.WorkOrders.Where(w => w.IsOpen && w.targetTreeId == "QOAK1").ToList()) manager.RemovePendingOrder(order.workOrderId);
            ui.CloseAll(); yield return null;
        }
        ecology.GrowthModelVersion = model;
        if (o != null) DestroyImmediate(o.gameObject);
        ecology.InvalidateCompetition(); yield return null;
    }

    private static string ExcerptAround(string text, string[] words)
    {
        foreach (string w in words) { int i = text.IndexOf(w, StringComparison.Ordinal); if (i >= 0) return text.Substring(Math.Max(0, i - 60), Math.Min(220, text.Length - Math.Max(0, i - 60))).Replace("\n", " / "); }
        return "";
    }

    // ---- section 12 ----
    private IEnumerator SessionMenuInput()
    {
        // Subsets can omit RESET_FLOW, which normally creates this Editor-excluded component.
        yield return EnsureSession();
        if (session.StartupScreenActive) throw new InvalidOperationException("Leave the startup screen before testing stand F10");
        session.Close(); ui.CloseAll(); ui.CloseHelp(); yield return null;
        bool beforeF10 = StandaloneSessionMenu.IsOpen;
        yield return Press(Key.F10);
        RecordF10Toggle("INPUT_SESSIONMENU_OPENS", beforeF10, true);
        beforeF10 = StandaloneSessionMenu.IsOpen;
        yield return Press(Key.F10);
        RecordF10Toggle("INPUT_SESSIONMENU_CLOSES", beforeF10, false);
        // These are mechanical checks, not evidence that either synthetic key reached Update().
        session.Close();
        session.Open(); yield return null;
        Hard("SESSION_MENU_DIRECT_OPEN_ISOLATES_FOREST", StandaloneSessionMenu.IsOpen && !session.StartupScreenActive && !player.enabled && !saves.enabled && !ui.enabled && Time.timeScale == 0f, "direct Open(), not a keyboard pass");
        session.Close(); yield return null;
        Hard("SESSION_MENU_DIRECT_CLOSE_RESTORES_FOREST", !StandaloneSessionMenu.IsOpen && player.enabled && saves.enabled && ui.enabled && Time.timeScale > 0f, "direct Close(), not a keyboard pass");
    }

    private IEnumerator InputSmoke()
    {
        ui.CloseAll(); ui.CloseHelp(); yield return null;
        ForestTree t = null; yield return AimAt(tr => !marks.GetCropTreeIds().Contains(tr.TreeId), x => t = x);
        Measure("INPUT_CURSOR", $"walking cursorLock={UnityEngine.Cursor.lockState} visible={UnityEngine.Cursor.visible} playerEnabled={player.enabled}");
        yield return Press(Key.E); bool eOn = player.IsInspecting; yield return Press(Key.E); bool eOff = !player.IsInspecting;
        Hard("INPUT_E_INSPECTS_AND_RETURNS", eOn && eOff, $"on={eOn} off={eOff}");
        yield return Press(Key.X); bool xOn = t.IsMarkedForFell; yield return Press(Key.X); bool xOff = !t.IsMarkedForFell;
        Hard("INPUT_X_MARKS_AND_UNMARKS", xOn && xOff, $"on={xOn} off={xOff}");
        yield return Press(Key.C); bool cOn = marks.GetCropTreeIds().Contains(t.TreeId); yield return Press(Key.C); bool cOff = !marks.GetCropTreeIds().Contains(t.TreeId);
        Hard("INPUT_C_MARKS_AND_UNMARKS_A_CROP_TREE", cOn && cOff, $"on={cOn} off={cOff}");

        // Esc frees the mouse; the first left click then retakes it WITHOUT inspecting (documented), the next one inspects
        yield return Press(Key.Escape);
        Measure("INPUT_ESC_WALKING", $"after Esc: cursorLock={UnityEngine.Cursor.lockState} visible={UnityEngine.Cursor.visible} playerEnabled={player.enabled}");
        yield return LeftClick();
        bool firstClickInspected = player.IsInspecting;
        Measure("INPUT_FIRST_CLICK_AFTER_ESC", $"inspecting={firstClickInspected} cursorLock={UnityEngine.Cursor.lockState} (documented: the first click only retakes the mouse)");
        yield return LeftClick();
        Hard("INPUT_LEFT_CLICK_INSPECTS_THE_AIMED_TREE", player.IsInspecting, $"afterFirstClick={firstClickInspected} afterSecondClick={player.IsInspecting}");
        if (player.IsInspecting) yield return Press(Key.E);

        // crop tree competitors 1-5
        yield return Press(Key.C);
        yield return Press(Key.E);
        for (int i = 0; i < 8; i++) yield return null;
        Measure("INPUT_COMPETITOR_PANEL", $"reportTree={(ui.Competitors.ReportTree != null ? ui.Competitors.ReportTree.TreeId : "none")} listed={ui.Competitors.ListedCount} selected={ui.Competitors.SelectedIndex}");
        int before = ui.Competitors.SelectedIndex;
        yield return Press(Key.Digit2);
        Hard("INPUT_DIGIT_2_SELECTS_THE_SECOND_COMPETITOR", ui.Competitors.ListedCount < 2 || ui.Competitors.SelectedIndex == 1, $"listed={ui.Competitors.ListedCount} selected {before}->{ui.Competitors.SelectedIndex}");
        yield return Press(Key.E); yield return Press(Key.C);   // leave inspection, remove the crop mark
        // G planting mode, U clearance
        yield return Press(Key.G);
        Hard("INPUT_G_WITHOUT_STOCK_STAYS_OUT_OF_PLANTING_MODE", !player.IsPlantingMode, $"planting={player.IsPlantingMode} (the game says: purchase stock first)");
        manager.TryPurchaseStock("sessile-oak-sapling", 1);
        yield return Press(Key.G); bool g = player.IsPlantingMode;
        yield return Press(Key.Escape); bool gOff = !player.IsPlantingMode;
        Hard("INPUT_G_ENTERS_PLANTING_MODE_WITH_STOCK_AND_ESC_LEAVES_IT", g && gOff, $"on={g} offAfterEsc={gOff}");
        yield return Press(Key.U);
        Measure("INPUT_U_CLEARANCE", $"clearancePreviewVisible={player.Clearance != null && player.Clearance.Visible} label=[{(player.Clearance != null ? player.Clearance.Label : "")}]");

        // every modal: opening and closing, and what the mouse does afterwards
        var rows = new List<string>();
        foreach (var modal in new (string name, Key open, Key close)[] { ("WorkPlan", Key.Tab, Key.Tab), ("Map", Key.M, Key.M), ("Objectives", Key.O, Key.O), ("Help", Key.F1, Key.Escape), ("WorkPlanEsc", Key.Tab, Key.Escape) })
        {
            yield return Press(modal.open);
            bool opened = manager.AnyPanelOpen || ui.Help.IsOpen || StandaloneSessionMenu.IsOpen;
            Vector3 position = player.transform.position;
            // The first-use window is modal and owns the keyboard: the close key does nothing while it is open, so a screen that opens
            // with one (the Stand Map here) has it dismissed first. F1 is the window itself and closes with Esc as before.
            if (modal.open != Key.F1 && ui.Help.IsOpen) yield return Press(Key.Escape);
            yield return Press(modal.close);
            for (int i = 0; i < 4; i++) yield return null;
            bool closed = !manager.AnyPanelOpen && !ui.Help.IsOpen && !StandaloneSessionMenu.IsOpen;
            rows.Add($"{modal.name}: opened={opened} closed={closed} playerEnabled={player.enabled} cursorLock={UnityEngine.Cursor.lockState} cursorVisible={UnityEngine.Cursor.visible} playerMoved={(player.transform.position - position).magnitude > 0.01f}");
            Hard($"INPUT_{modal.name.ToUpperInvariant()}_OPENS_AND_CLOSES", opened && closed && player.enabled, rows.Last());
        }
        foreach (string r in rows) Measure("INPUT_MODAL_CLOSURE", r);
        yield return SessionMenuInput();
        Measure("INPUT_MOUSE_LIMIT", "Work Plan and every button were pressed through their registered callbacks; a physical mouse and native input were not exercised.");
    }

    // ---- section 13 ----
    private IEnumerator Preservation()
    {
        ui.CloseAll(); yield return null;
        // reversible marks and the competitor panel are exercised above; here: the cards and the history/completion presentation
        ui.ShowWorkPlan(); yield return null; yield return null;
        string plan = Texts(UiRoot(), false);
        Hard("PRESERVED_WORK_PLAN_HAS_MONEY_AND_WHAT_YOU_ARE_LEAVING", plan.Contains("MONEY") && plan.Contains("WHAT YOU ARE LEAVING"), $"money={plan.Contains("MONEY")} leaving={plan.Contains("WHAT YOU ARE LEAVING")}");
        ui.CloseAll();
        yield return Press(Key.M);
        Hard("PRESERVED_STAND_MAP_OPENS_AND_CLOSES", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, $"screen={ui.CurrentScreen}");
        yield return Shoot("18-stand-map");
        yield return Press(Key.M);
        Hard("PRESERVED_STAND_MAP_RETURNS", ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, $"screen={ui.CurrentScreen}");

        // history and the stale-order card: leave an order pending across two years
        ForestTree t = null; yield return AimAt(tr => !tr.IsMarkedForFell && !marks.GetCropTreeIds().Contains(tr.TreeId), x => t = x);
        yield return Press(Key.X);
        ui.ShowWorkPlan(); yield return null; yield return null;
        manager.AdvanceYear(); manager.AdvanceYear(); manager.MarkAnnualReviewSeen();
        ui.CloseAll(); ui.ShowWorkPlan(); yield return null; yield return null; yield return null;
        Hard("PRESERVED_STALE_ORDER_CARD_IS_SURFACED", Texts(UiRoot(), false).Contains("Orders needing attention") && manager.StaleOrders.Count >= 1, $"stale={manager.StaleOrders.Count}");
        yield return Shoot("19-work-plan-stale-order");
        ui.CloseAll(); yield return null; yield return null;
        Hard("PRESERVED_HUD_SHOWS_THE_INTERVENTION_COUNT", Texts(UiRoot()).Contains("intervention"), Regex.Match(Texts(UiRoot()), @"[^\n]*intervention[^\n]*").Value);
        yield return Shoot("20-hud-with-intervention-count", null, size =>
        {
            var problems = new List<string>(); CheckHudTitle(problems);
            Hard($"HUD_TITLE_SHOWS_THE_WHOLE_INTERVENTION_COUNT_{size.x}x{size.y}", problems.Count == 0, string.Join(" ; ", problems));
        });
        ui.ShowReview(); yield return null; yield return null;
        string review = Texts(UiRoot(), false);
        Hard("PRESERVED_ANNUAL_REVIEW_HAS_MANAGEMENT_HISTORY_AND_YEAR_STEPPING", review.IndexOf("management history", StringComparison.OrdinalIgnoreCase) >= 0 && review.Contains("Earlier year") && review.Contains("Later year"), Regex.Match(review, @"[^\n]*(MANAGEMENT|Management) (HISTORY|history)[^\n]*").Value);
        yield return Shoot("21-annual-review-history");
        ui.CloseAll();
        typeof(ScenarioOneManager).GetMethod("SetOutcome", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(manager, new object[] { ScenarioOneOutcome.Completed, manager.CurrentEcologicalYear, "forced for the rendered review" });
        ui.ShowReview(); yield return null; yield return null; yield return null;
        string done = Texts(UiRoot(), false);
        Hard("PRESERVED_COMPLETION_SUMMARY_APPEARS_AT_THE_COMPLETION_YEAR_AND_IS_NOT_A_SCORE", (done.Contains("COMPLETE") || done.Contains("Objectives met")) && done.Contains("not a score"), "");
        yield return Shoot("22-completion-summary");
        ui.CloseAll(); yield return null;
    }

    // ---- section 14 ----
    private IEnumerator PerfSanity()
    {
        ui.CloseAll(); ui.CloseHelp(); yield return null;
        // lesson cue / HUD per frame
        // The .NET per-thread allocation counter reports nothing under Unity's Mono, so the Mono heap in use is read around each frame and
        // frames in which a collection ran are left out of the allocation figure.
        var frames = new List<float>(); var allocs = new List<long>(); int collected = 0;
        for (int i = 0; i < 300; i++)
        {
            long a0 = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong(); int c0 = GC.CollectionCount(0);
            yield return null;
            frames.Add(Time.unscaledDeltaTime * 1000f);
            if (GC.CollectionCount(0) == c0) allocs.Add(UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() - a0); else collected++;
        }
        if (allocs.Count == 0) allocs.Add(0);
        var sorted = frames.OrderBy(v => v).ToList();
        Measure("PERF_HUD_FRAMES", $"frames=300 meanMs={F(frames.Average())} p95Ms={F(sorted[(int)(sorted.Count * 0.95)])} maxMs={F(sorted.Last())} monoHeapGrowthMeanBytesPerFrame={F(allocs.Average(), "0")} maxInAFrame={allocs.Max()} framesMeasured={allocs.Count}/300 framesWithACollection={collected}");
        // Work Plan open/close cost
        var sw = new System.Diagnostics.Stopwatch(); var openMs = new List<double>();
        for (int i = 0; i < 20; i++)
        {
            sw.Restart(); ui.ShowWorkPlan(); yield return null; sw.Stop(); openMs.Add(sw.Elapsed.TotalMilliseconds);
            ui.CloseAll(); yield return null;
        }
        Measure("PERF_WORK_PLAN_OPEN", $"opens=20 meanMsIncludingOneFrame={F(openMs.Average())} maxMs={F(openMs.Max())}");
        // the guard card: show/hide repeatedly
        ForestTree t = null; yield return AimAt(tr => !tr.IsMarkedForFell && !marks.GetCropTreeIds().Contains(tr.TreeId), x => t = x);
        yield return Press(Key.X);
        ui.ShowWorkPlan(); yield return null; yield return null;
        long g0 = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong(); int gcg = GC.CollectionCount(0); sw.Restart();
        for (int i = 0; i < 20; i++)
        {
            Button adv = BtnStarts(UiRoot(), "Advance one year");
            if (adv != null && adv.enabledInHierarchy) { MethodInfo inv = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic); inv.Invoke(adv.clickable, new object[] { null }); }
            yield return null;
            Button back = Btn(UiRoot(), AdvanceGuard.ReturnLabel);
            if (back != null) { MethodInfo inv = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic); inv.Invoke(back.clickable, new object[] { null }); }
            yield return null;
        }
        sw.Stop();
        Measure("PERF_GUARD_CARD_20_CYCLES", $"totalMs={F(sw.Elapsed.TotalMilliseconds)} monoHeapGrowthBytes={UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() - g0} collections={GC.CollectionCount(0) - gcg} year={ecology.EcologicalYear} (the year must not have moved)");
        ui.CloseAll(); yield return null;
        // reset / help interactions
        sw.Restart(); long r0 = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong(); int gcr = GC.CollectionCount(0);
        for (int i = 0; i < 10; i++) { ui.ResetTeachingProgress(); ui.ShowHelp(MenuHelpView.Menu.WalkingHud); yield return null; ui.CloseHelp(); yield return null; }
        sw.Stop();
        Measure("PERF_HELP_AND_RESET_10_CYCLES", $"totalMs={F(sw.Elapsed.TotalMilliseconds)} monoHeapGrowthBytes={UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() - r0} collections={GC.CollectionCount(0) - gcr}");
    }

    // ---- section 14, CPU only: the rendering in this run is software on a nested display, so frame times are not game measurements.
    // These calls are synchronous and involve no frame, so they measure the code the packet asks about.
    private IEnumerator PerfCpuOnly()
    {
        ui.CloseAll(); ui.CloseHelp(); yield return null;
        var sw = new System.Diagnostics.Stopwatch();
        // the lesson cue: the HUD reads this every frame
        string last = null; const int calls = 200000;
        long h0 = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong(); int c0 = GC.CollectionCount(0);
        sw.Restart();
        for (int i = 0; i < calls; i++) last = ui.Learning.Summary;
        sw.Stop();
        long grew = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() - h0;
        Measure("PERF_CUE_GETTER", $"calls={calls} totalMs={F(sw.Elapsed.TotalMilliseconds)} nsPerCall={F(sw.Elapsed.TotalMilliseconds * 1e6 / calls, "0")} monoHeapGrowthBytes={grew} collections={GC.CollectionCount(0) - c0} lastValue=[{last}]");
        Hard("PERF_CUE_GETTER_DOES_NOT_ALLOCATE_PER_CALL", grew < 64 * 1024 && GC.CollectionCount(0) == c0, $"{calls} calls grew the Mono heap by {grew} bytes");
        // the Work Plan: build and refresh, no frames
        var open = new List<double>();
        for (int i = 0; i < 20; i++) { sw.Restart(); ui.ShowWorkPlan(); sw.Stop(); open.Add(sw.Elapsed.TotalMilliseconds); ui.CloseAll(); yield return null; }
        Measure("PERF_WORK_PLAN_SHOW_CPU", $"opens=20 meanMs={F(open.Average())} maxMs={F(open.Max())} firstMs={F(open[0])}");
        // the guard card: the click handler and its refresh, no frames
        ForestTree tr = null; yield return AimAt(x => !x.IsMarkedForFell && !marks.GetCropTreeIds().Contains(x.TreeId), x => tr = x);
        yield return Press(Key.X);
        ui.ShowWorkPlan(); yield return null; yield return null;
        MethodInfo inv = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        var clicks = new List<double>(); int y0 = ecology.EcologicalYear;
        long g0 = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
        for (int i = 0; i < 20; i++)
        {
            Button adv = BtnStarts(UiRoot(), "Advance one year"); Button back = null;
            sw.Restart(); if (adv != null && adv.enabledInHierarchy) inv.Invoke(adv.clickable, new object[] { null }); sw.Stop(); clicks.Add(sw.Elapsed.TotalMilliseconds);
            back = Btn(UiRoot(), AdvanceGuard.ReturnLabel);
            sw.Restart(); if (back != null) inv.Invoke(back.clickable, new object[] { null }); sw.Stop(); clicks.Add(sw.Elapsed.TotalMilliseconds);
            yield return null;
        }
        Measure("PERF_GUARD_CARD_CPU", $"clicks=40 meanMs={F(clicks.Average())} maxMs={F(clicks.Max())} monoHeapGrowthBytes={UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong() - g0} year={ecology.EcologicalYear} (was {y0})");
        Hard("PERF_GUARD_CARD_REPEATED_USE_NEVER_ADVANCED_THE_YEAR", ecology.EcologicalYear == y0, $"year={ecology.EcologicalYear}");
        ui.CloseAll(); yield return null;
        // help and reset
        var helpMs = new List<double>();
        for (int i = 0; i < 10; i++) { sw.Restart(); ui.ResetTeachingProgress(); ui.ShowHelp(MenuHelpView.Menu.WalkingHud); ui.CloseHelp(); sw.Stop(); helpMs.Add(sw.Elapsed.TotalMilliseconds); yield return null; }
        Measure("PERF_HELP_RESET_CPU", $"cycles=10 meanMs={F(helpMs.Average())} maxMs={F(helpMs.Max())}");
    }

    // ---- U1.2: the three rendered defects ----

    private static bool Inside(Rect inner, Rect outer, float tolerance = 1f)
        => inner.xMin >= outer.xMin - tolerance && inner.yMin >= outer.yMin - tolerance && inner.xMax <= outer.xMax + tolerance && inner.yMax <= outer.yMax + tolerance;

    private static bool Overlap(Rect a, Rect b, float inset = 2f)
        => a.xMin + inset < b.xMax && b.xMin + inset < a.xMax && a.yMin + inset < b.yMax && b.yMin + inset < a.yMax;

    private static string Short(string s) => string.IsNullOrEmpty(s) ? "" : (s.Length > 44 ? s.Substring(0, 44) + "..." : s).Replace("\n", " ");

    private static void SetField(object target, string name, object value)
        => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).SetValue(target, value);

    // The title row of the HUD status panel: year, phase and the intervention count. It must show all of its text, wrapping if it has to.
    private void CheckHudTitle(List<string> problems)
    {
        VisualElement status = UiRoot().Q(className: "hud-status");
        Label title = status == null ? null : status.Query<Label>(className: "heading").ToList().FirstOrDefault();
        if (title == null || !Visible(title)) { problems.Add("the HUD title is not visible"); return; }
        if (ui.Manager.Interventions.Count > 0 && !title.text.Contains("intervention")) problems.Add($"the title has no intervention count: [{title.text}]");
        float oneLine = title.MeasureTextSize(title.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined).x;
        float available = title.contentRect.width;
        if (oneLine > available + 1f)
        {
            Vector2 wrapped = title.MeasureTextSize(title.text, available, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            bool wraps = title.resolvedStyle.whiteSpace == WhiteSpace.Normal;
            if (!wraps) problems.Add($"the title needs {F(oneLine)} but has {F(available)} and does not wrap: [{title.text}]");
            else if (title.contentRect.height + 1f < wrapped.y) problems.Add($"the wrapped title needs {F(wrapped.y)} high but has {F(title.contentRect.height)}");
        }
        if (!Inside(title.worldBound, status.worldBound)) problems.Add($"the title {title.worldBound} leaves its panel {status.worldBound}");
        Measure("HUD_TITLE", $"text=[{title.text}] oneLineWidth={F(oneLine)} available={F(available)} whiteSpace={title.resolvedStyle.whiteSpace} height={F(title.contentRect.height)}");
    }

    // One inspected tree: the card, the estimate or competitor panel beside it, and the HUD panels around them.
    private void CheckInspectionLayout(string state, List<string> problems)
    {
        VisualElement root = UiRoot();
        Rect screen = root.worldBound;
        VisualElement card = root.Q(className: "inspect");
        if (card == null || !Visible(card)) { problems.Add("the inspection card is not visible"); return; }
        var panels = new List<(string name, VisualElement element)> { ("inspection card", card) };
        VisualElement estimate = root.Q("inspect-estimate"), competitors = root.Q("competitor-panel");
        if (estimate != null && Visible(estimate)) panels.Add(("estimate panel", estimate));
        if (competitors != null && Visible(competitors)) panels.Add(("competitor panel", competitors));
        foreach (var (name, element) in panels)
        {
            Rect b = element.worldBound;
            if (!Inside(b, screen)) problems.Add($"{name} {b} leaves the screen {screen}");
            foreach (TextElement t in element.Query<TextElement>().ToList())
            {
                if (!Visible(t) || string.IsNullOrWhiteSpace(t.text)) continue;
                if (!Inside(t.worldBound, b)) { problems.Add($"{name}: text [{Short(t.text)}] {t.worldBound} is outside its panel {b}"); break; }
            }
            Measure("INSPECTION_PANEL", $"{state} {name} height={F(b.height)} ({F(100 * b.height / screen.height, "0")}% of the screen) bottom={F(b.yMax)} of {F(screen.yMax)}");
        }
        var hud = new List<(string name, VisualElement element)>();
        VisualElement status = root.Q(className: "hud-status");
        if (status != null && Visible(status)) hud.Add(("HUD status panel", status));
        VisualElement bottom = root.Q(className: "hud-bottom");
        if (bottom != null) foreach (VisualElement c in bottom.Children()) if (Visible(c) && c.worldBound.height > 0) hud.Add(("HUD bottom-left " + (c.ClassListContains("hud-forecast") ? "forecast" : c.ClassListContains("hud-ground") ? "ground report" : "mark summary"), c));
        foreach (var p in panels) foreach (var h in hud)
            if (Overlap(p.element.worldBound, h.element.worldBound)) problems.Add($"{p.name} {p.element.worldBound} overlaps the {h.name} {h.element.worldBound}");
        for (int i = 0; i < panels.Count; i++) for (int j = i + 1; j < panels.Count; j++)
            if (Overlap(panels[i].element.worldBound, panels[j].element.worldBound)) problems.Add($"{panels[i].name} overlaps {panels[j].name}");
        CheckHudTitle(problems);
    }

    private void KeepInspecting(ForestTree tree)
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(player, new object[] { tree });
        ui.CloseHelp();
    }

    private IEnumerator ShootInspection(string state, ForestTree tree, string expectation)
    {
        yield return Shoot(state, () => KeepInspecting(tree), size =>
        {
            var problems = new List<string>(); CheckInspectionLayout(state, problems);
            bool wantsEstimate = expectation == "estimate", wantsCompetitors = expectation == "competitors";
            VisualElement estimate = UiRoot().Q("inspect-estimate"), competitors = UiRoot().Q("competitor-panel");
            if (wantsEstimate && (estimate == null || !Visible(estimate))) problems.Add("the felling estimate is not shown");
            if (wantsCompetitors && (competitors == null || !Visible(competitors))) problems.Add("the competitor panel is not shown");
            if (wantsEstimate && !Texts(UiRoot()).Contains("If felled now")) problems.Add("no 'If felled now' estimate text");
            Hard($"INSPECTION_{state.ToUpperInvariant().Replace('-', '_')}_FITS_{size.x}x{size.y}", problems.Count == 0, string.Join(" ; ", problems));
        });
        SetField(player, "inspectedTree", null); SetField(player, "isInspecting", false);
        yield return null; yield return null;
    }

    private IEnumerator InspectionCases()
    {
        ui.CloseAll(); ui.CloseHelp(); yield return null;
        int model = ecology.GrowthModelVersion;
        ecology.GrowthModelVersion = 2;                     // the broadleaf market is on, so Oak and Beech show their product lists
        TreeSpeciesDefinition oak = spawner.ResolveSpecies("sessile-oak"), beech = spawner.ResolveSpecies("beech");
        ForestTree[] sitka = Trees();
        Vector3 at = sitka[0].transform.position;
        ForestTree prunedOak = spawner.Spawn("QOAK1", oak, at + new Vector3(6, 0, 0), 80, 52f, 24f, oak.PotentialCrownRadiusM(52f));
        ForestTree plainOak = spawner.Spawn("QOAK2", oak, at + new Vector3(-6, 0, 0), 80, 52f, 24f, oak.PotentialCrownRadiusM(52f));
        ForestTree beechTree = spawner.Spawn("QBEECH1", beech, at + new Vector3(0, 0, 7), 80, 48f, 24f, beech.PotentialCrownRadiusM(48f));
        SetField(prunedOak, "pruningLifts", 3); SetField(prunedOak, "crownBaseHeightM", 8f);   // a rendered-fixture state: the pruned-Oak product text is the longest
        ecology.InvalidateCompetition(); ecology.RecomputeCanopy(); yield return null; yield return null;

        // The bottom-left HUD stack at its tallest: a Crop Tree and several Fell marks give the mark summary and the forecast panel.
        int fellBefore = marks.LivingMarkedCount, cropBefore = marks.LivingCropTreeCount;   // earlier phases may leave marks
        ForestTree cropTree = sitka[1];
        ForestTree[] fells = sitka.Skip(2).Take(4).ToArray();
        marks.Mark(cropTree, TreeMarkType.CropTree, false);
        foreach (ForestTree f in fells) marks.Mark(f, TreeMarkType.Fell, false);
        yield return null; yield return null;
        Measure("INSPECTION_FIXTURE", $"growthModel={ecology.GrowthModelVersion} broadleafMarket={manager.BroadleafMarketActive} interventions={manager.Interventions.Count} fellMarks={fells.Length} prunedOakLifts={prunedOak.PruningLifts}");

        yield return ShootInspection("25-inspect-sitka-with-marks", sitka[sitka.Length - 1], "estimate");
        yield return ShootInspection("26-inspect-oak-pruned", prunedOak, "estimate");
        yield return ShootInspection("27-inspect-oak-unpruned", plainOak, "estimate");
        yield return ShootInspection("28-inspect-beech", beechTree, "estimate");
        marks.Mark(plainOak, TreeMarkType.CropTree, false); yield return null; yield return null;
        for (int i = 0; i < 6; i++) { KeepInspecting(plainOak); yield return null; }
        yield return ShootInspection("29-inspect-oak-crop-tree-competitors", plainOak, "competitors");
        marks.Unmark(plainOak, false);
        ecology.GrowthModelVersion = 1; ecology.InvalidateCompetition(); yield return null;
        yield return ShootInspection("30-inspect-oak-model1", prunedOak, "estimate");

        foreach (ForestTree f in fells) marks.Unmark(f, false);
        marks.Unmark(cropTree, false);
        foreach (ForestTree t in new[] { prunedOak, plainOak, beechTree }) if (t != null) DestroyImmediate(t.gameObject);
        ecology.GrowthModelVersion = model; ecology.InvalidateCompetition(); ecology.RecomputeCanopy(); yield return null;
        Hard("INSPECTION_FIXTURE_LEFT_THE_MARKS_AS_IT_FOUND_THEM", marks.LivingMarkedCount == fellBefore && marks.LivingCropTreeCount == cropBefore,
            $"fell={marks.LivingMarkedCount} (was {fellBefore}) crop={marks.LivingCropTreeCount} (was {cropBefore})");
    }

    // The "Deadwood volume" row of WHAT YOU ARE LEAVING: a long value must not squeeze its name into a column a syllable wide.
    private IEnumerator DeadwoodLine()
    {
        ui.CloseAll(); ui.CloseHelp(); yield return null;
        int fellBefore = marks.LivingMarkedCount;
        ForestTree t = null; yield return AimAt(tr => !tr.IsMarkedForFell && !marks.GetCropTreeIds().Contains(tr.TreeId) && manager.WorkOrders.All(o => o.targetTreeId != tr.TreeId), x => t = x);
        Hard("DEADWOOD_FIXTURE_TREE", t != null, t != null ? t.TreeId : "none");
        yield return Press(Key.X);
        manager.AddMarkedTreesToWorkPlan();
        ScenarioOneWorkOrder order = manager.WorkOrders.FirstOrDefault(o => o.IsOpen && o.targetTreeId == t.TreeId);
        Hard("DEADWOOD_FELL_ORDER_EXISTS", order != null && manager.SetPendingFellingOutcome(order.workOrderId, FellingMaterialOutcome.RetainAsFallenDeadwood), "retain as deadwood");
        ui.ShowWorkPlan(); for (int i = 0; i < 4; i++) yield return null; ui.CloseHelp(); yield return null;
        Label NameLabel() => UiRoot().Query<Label>().ToList().FirstOrDefault(l => l.text == "Deadwood volume");
        Hard("DEADWOOD_VOLUME_ROW_IS_SHOWN", NameLabel() != null, "planned deadwood > 0");
        yield return Shoot("31-work-plan-deadwood-volume", () =>
        {
            Label name = NameLabel(); ScrollView sv = UiRoot().Query<ScrollView>().ToList().FirstOrDefault(Visible);
            if (name != null && sv != null) sv.ScrollTo(name);
        }, size =>
        {
            Label name = NameLabel(); var problems = new List<string>();
            if (name == null) problems.Add("row missing");
            else
            {
                VisualElement row = name.parent, card = UiRoot().Q("workplan-leaving");
                Label value = row.Query<Label>().ToList().FirstOrDefault(l => l != name);
                float oneLine = name.MeasureTextSize(name.text, 0, VisualElement.MeasureMode.Undefined, 0, VisualElement.MeasureMode.Undefined).x;
                if (oneLine > name.contentRect.width + 1f) problems.Add($"the name needs {F(oneLine)} but has {F(name.contentRect.width)} (wraps)");
                if (value == null) problems.Add("no value label");
                else
                {
                    Vector2 wrapped = value.MeasureTextSize(value.text, value.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
                    if (value.contentRect.height + 1f < wrapped.y) problems.Add($"the value is clipped: needs {F(wrapped.y)} high, has {F(value.contentRect.height)}");
                    if (card != null && !Inside(value.worldBound, card.worldBound)) problems.Add($"the value {value.worldBound} leaves its card {card.worldBound}");
                }
                if (card != null && !Inside(name.worldBound, card.worldBound)) problems.Add($"the name {name.worldBound} leaves its card {card.worldBound}");
                Measure("DEADWOOD_ROW", $"{size.x}x{size.y} nameWidth={F(name.contentRect.width)} nameHeight={F(name.contentRect.height)} valueWidth={(value != null ? F(value.contentRect.width) : "-")} valueHeight={(value != null ? F(value.contentRect.height) : "-")}");
            }
            Hard($"DEADWOOD_VOLUME_NAME_DOES_NOT_WRAP_{size.x}x{size.y}", problems.Count == 0, string.Join(" ; ", problems));
        });
        if (order != null) manager.RemovePendingOrder(order.workOrderId);
        ui.CloseAll(); yield return null;
        if (t != null) marks.Unmark(t, false);
        Hard("DEADWOOD_FIXTURE_LEFT_THE_MARKS_AS_IT_FOUND_THEM", marks.LivingMarkedCount == fellBefore, $"fell={marks.LivingMarkedCount} (was {fellBefore})");
    }

    // ---- section 8/10: the startup Controls card (last: the startup screen is left only through Start/Continue) ----
    private IEnumerator StartupControls()
    {
        yield return EnsureSession();
        ui.CloseAll(); ui.CloseHelp(); yield return null;
        session.ShowStartupScreen();
        for (int i = 0; i < 6; i++) yield return null;
        yield return Shoot("23-startup-title");
        yield return Invoke(BtnStarts(SessionRoot(), "Controls / Help"));
        VisualElement root = SessionRoot();
        string controls = Texts(root, false);
        var buttons = root.Query<Button>().ToList().Where(Visible).Select(b => b.text).ToList();
        int missing = ControlsReference.Rows.Count(r => !controls.Contains(r[0]));
        Hard("STARTUP_CONTROLS_LISTS_EVERY_CONTROL_ROW", missing == 0, $"rows={ControlsReference.Rows.Length} missing={missing}");
        Hard("STARTUP_CONTROLS_CARD_OFFERS_ONLY_BACK", buttons.Count == 1 && buttons[0] == "Back", $"buttons=[{string.Join(" | ", buttons)}]");
        yield return Shoot("24-startup-controls-card");
        Button back = Btn(root, "Back");
        Rect bb = back != null ? back.worldBound : default; Rect rb = root.worldBound;
        Hard("STARTUP_CONTROLS_BACK_BUTTON_IS_ON_SCREEN", back != null && bb.xMin >= rb.xMin && bb.xMax <= rb.xMax && bb.yMin >= rb.yMin && bb.yMax <= rb.yMax, $"back={bb} viewport={rb}");
        // The card may stay scrollable. It has to scroll to the last row and keep Back on screen while it does.
        ScrollView list = root.Query<ScrollView>().ToList().FirstOrDefault(Visible);
        Hard("STARTUP_CONTROLS_LIST_SCROLLS", list != null && list.contentContainer.layout.height > list.contentViewport.layout.height + 1f,
            list == null ? "no scroll view" : $"content={F(list.contentContainer.layout.height)} viewport={F(list.contentViewport.layout.height)}");
        if (list != null)
        {
            list.scrollOffset = new Vector2(0, list.verticalScroller.highValue);
            for (int i = 0; i < 4; i++) yield return null;
            string lastKey = ControlsReference.Rows.Last()[0];
            Label lastRow = root.Query<Label>().ToList().FirstOrDefault(l => l.text == lastKey && Visible(l));
            Rect port = list.contentViewport.worldBound;
            Hard("STARTUP_CONTROLS_SCROLLS_TO_THE_LAST_ROW_WITH_BACK_STILL_VISIBLE", lastRow != null && Inside(lastRow.worldBound, port) && Btn(root, "Back") != null && Inside(Btn(root, "Back").worldBound, rb),
                $"lastRow=[{lastKey}] row={(lastRow != null ? lastRow.worldBound.ToString() : "not visible")} viewport={port}");
            yield return Shoot("24b-startup-controls-scrolled-to-end");
            list.scrollOffset = Vector2.zero;
            for (int i = 0; i < 3; i++) yield return null;
        }
        yield return Invoke(back);
        Hard("STARTUP_BACK_RETURNS_TO_THE_TITLE", session.StartupScreenActive, "");
    }
}
#endif
