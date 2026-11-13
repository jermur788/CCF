#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

// Disposable #443 G5 qualification. Fresh isolated world, real OS keys/clicks; map/menu buttons use their Clickable.
// Reuses G2 sizing/input/aim helpers. Player placement beside trunks is an explicit fixture, not a claimed walked route.
// Adds rendered arrow pixels, correct panel name, out-of-order and edited-ID plans, annual execution and old-world controls.
// Source lives outside Assets and is staged/removed by run_gate.py. Never writes the real player's profile or saves.
public static class Objective1UxNative
{
    public const string Marker = "Objective1UxNative.Begin";

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Objective1UxRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Contains(Marker)) return;
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains(Marker))
            new GameObject("Disposable #438 rack entrance witness").AddComponent<Objective1UxRunner>();
    }
}

public sealed class Objective1UxRunner : MonoBehaviour
{
    private static readonly Vector2Int[] Sizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };
    private static readonly BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestTreeMarkingManager marks;
    private ForestSaveController saves;
    private ForestPlayer player;
    private string output, savePath, freshJson;
    private Vector3 clearingSpawn;
    private int sequence, checks;
    private readonly List<string> receipts = new List<string>();   // JSON objects, one per observation
    private readonly List<string> mismatches = new List<string>();
    private readonly List<string> feedbackFailures = new List<string>();

    // ---- reporting --------------------------------------------------------------------------------------------------
    private void Check(bool ok, string why)
    {
        if (!ok) throw new InvalidOperationException(why);
        checks++; Debug.Log("O1443_CHECK " + why);
    }
    private void Receipt(string kind, string json)
    {
        receipts.Add("{\"kind\":\"" + kind + "\"," + json + "}");
        Debug.Log("O1443_" + kind + " " + json);
    }
    private void Mismatch(string why) { mismatches.Add(why); throw new InvalidOperationException("ASSOCIATION STOP: " + why); }
    private static string Q(string s) => "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", " / ") + "\"";
    private static string F(float v) => v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);

    // ---- helpers (as RACK-V2-12) ------------------------------------------------------------------------------------
    private static IEnumerator Settle(int frames = 16) { for (int i = 0; i < frames; i++) yield return null; }
    private static IEnumerator Wait(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }
    private ForestTree[] AllTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private ForestTree Tree(string id) => AllTrees().Single(t => t.TreeId == id);
    private static bool Visible(VisualElement e)
    {
        if (e == null) return false;
        for (VisualElement p = e; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity <= 0.01f) return false;
        return e.worldBound.width > 0 && e.worldBound.height > 0;
    }
    private string Text() => string.Join("\n", ui.RootElement.Query<TextElement>().ToList().Where(Visible).Select(t => t.text).Where(s => !string.IsNullOrEmpty(s)));
    private Button Named(string name) => ui.RootElement.Q<Button>(name);
    private IEnumerator Invoke(Button button, string what)
    {
        Check(button != null, what + ": control present");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return Settle(3);
    }
    private IEnumerator CloseHelp()
    {
        for (int i = 0; i < 6 && ui.Help.IsOpen; i++)
        {
            Debug.Log("O1443_HELP_CLOSED " + ui.Help.CurrentMenu);
            Button close = ui.Help.Root.Q<Button>("menu-help-close");
            if (close != null && Visible(close)) yield return Invoke(close, "help close"); else ui.CloseHelp();
            yield return Settle(3);
        }
    }

    [Serializable] private sealed class Op { public string op, key; public int x, y, button; public float seconds; }
    [Serializable] private sealed class Request { public int seq; public Op[] ops; }
    private static Op Key(string key) => new Op { op = "key", key = key };
    private static Op Click() => new Op { op = "click", button = 1 };
    private IEnumerator Native(params Op[] ops) => NativeWithSettle(12, ops);
    private IEnumerator NativeWithSettle(int settleFrames, params Op[] ops)
    {
        string folder = Environment.GetEnvironmentVariable("CCF_DRIVER_DIR");
        if (string.IsNullOrEmpty(folder)) throw new InvalidOperationException("CCF_DRIVER_DIR (native OS input driver) is required");
        int seq = ++sequence;
        string path = Path.Combine(folder, $"request-{seq:D4}.json");
        File.WriteAllText(path + ".tmp", JsonUtility.ToJson(new Request { seq = seq, ops = ops }));
        File.Move(path + ".tmp", path);
        double until = Time.realtimeSinceStartupAsDouble + 60;
        while (!File.Exists(Path.Combine(folder, $"done-{seq:D4}")) && Time.realtimeSinceStartupAsDouble < until)
        {
            string error = Path.Combine(folder, "driver-error");
            if (File.Exists(error)) throw new InvalidOperationException("native driver: " + File.ReadAllText(error));
            yield return null;
        }
        if (!File.Exists(Path.Combine(folder, $"done-{seq:D4}"))) throw new TimeoutException("native OS input request " + seq);
        yield return Settle(settleFrames);
    }

    public static void SetGameSize(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, AnyInstance, null,
            new object[] { Enum.ToObject(kind, 1), width, height, "O1443 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", AnyInstance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", AnyInstance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.maximized = true; view.Show(); view.Focus(); view.Repaint();
    }

    // Captures at both sizes; at each size also records the minimap and HUD direction-box bounds in real screen pixels.
    private IEnumerator Shoot(string state)
    {
        foreach (Vector2Int size in Sizes)
        {
            SetGameSize(size.x, size.y);
            yield return Settle(12);
            yield return new WaitForEndOfFrame();
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            Check(image != null && image.width == size.x && image.height == size.y, "rendered capture " + state + " at " + size.x + "x" + size.y);
            int lit = 0, samples = 0;
            for (int y = 0; y < image.height; y += 25)
                for (int x = 0; x < image.width; x += 25) { samples++; if (image.GetPixel(x, y).maxColorComponent > 0.05f) lit++; }
            string file = state + "-" + size.x + "x" + size.y + ".png";
            if (state == "g08a-plan-racks-editing") CheckPlanningCardAccent(image);
            if (state == "g14-annual-review-before-acknowledgement") CheckReviewInstruction(true);
            if (state == "g15-annual-review-acknowledged") CheckReviewInstruction(false);
            if (state.Contains("before-first-spray")) CheckFirstArrowPixels(image);
            if (!manager.ReferencePreviewActive && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !manager.WorkPlanOpen && !ui.Help.IsOpen) { CheckMinimapFit(state); CheckGuidedField(state); }
            File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
            Destroy(image);
            Check(lit > samples / 20, "capture is not black " + file + " lit=" + lit + "/" + samples);
            Receipt("BOUNDS", "\"state\":" + Q(state) + ",\"size\":\"" + size.x + "x" + size.y + "\"," + Bounds());
        }
        SetGameSize(1280, 720);
        yield return Settle(10);
    }

    private void CheckPlanningCardAccent(Texture2D image)
    {
        VisualElement card = ui.Map.Root.Q("map-edit-card");
        Check(manager.RackEditingOpen && !manager.RackPlanConfirmed && Visible(card)
            && card.ClassListContains("map-planning-instructions"), "Plan Racks editing card is on screen before Confirm");
        Color actual = card.resolvedStyle.borderLeftColor;
        Color expected = new Color32(110, 163, 157, 255);
        Receipt("PLANNING_CARD_ACCENT", "\"size\":\"" + Screen.width + "x" + Screen.height + "\",\"colour\":["
            + F(actual.r) + "," + F(actual.g) + "," + F(actual.b) + "," + F(actual.a) + "],\"leftWidth\":" + F(card.resolvedStyle.borderLeftWidth));
        Check(Mathf.Abs(actual.r - expected.r) < .005f && Mathf.Abs(actual.g - expected.g) < .005f
            && Mathf.Abs(actual.b - expected.b) < .005f && actual.a > .99f,
            "Plan Racks editing card resolves the accent-coloured left edge");
        // UI Toolkit quantizes the 3-unit USS edge to device pixels (2 px at 0.8 scale, 3 px at 1.2).
        // Measure that edge in the capture as well: resolved colour alone cannot prove it rendered.
        float scale = Screen.width / ui.RootElement.worldBound.width;
        int expectedPixels = Mathf.Max(1, Mathf.FloorToInt(3f * scale + .01f));
        Check(card.resolvedStyle.borderLeftWidth * scale >= expectedPixels - .01f,
            "planning edge retains the 3-unit USS width after device-pixel quantization");
        Rect bounds = card.worldBound;
        int x0 = Mathf.Max(0, Mathf.FloorToInt(bounds.xMin * scale) - 1);
        int x1 = Mathf.Min(image.width, Mathf.CeilToInt(bounds.xMin * scale) + expectedPixels + 1);
        int y0 = Mathf.Max(0, Mathf.CeilToInt(bounds.yMin * scale) + 4);
        int y1 = Mathf.Min(image.height, Mathf.FloorToInt(bounds.yMax * scale) - 4);
        int accentPixels = 0;
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++)
            {
                Color pixel = image.GetPixel(x, image.height - 1 - y);
                if (Mathf.Abs(pixel.r - expected.r) < .025f && Mathf.Abs(pixel.g - expected.g) < .025f
                    && Mathf.Abs(pixel.b - expected.b) < .025f) accentPixels++;
            }
        Receipt("PLANNING_CARD_PIXELS", "\"size\":" + Q(Screen.width + "x" + Screen.height) + ",\"scale\":" + F(scale)
            + ",\"expectedWidthPx\":" + expectedPixels + ",\"resolvedWidthPx\":" + F(card.resolvedStyle.borderLeftWidth * scale)
            + ",\"accentPixels\":" + accentPixels + ",\"sampleHeightPx\":" + (y1 - y0));
        Check(y1 > y0 && accentPixels >= expectedPixels * (y1 - y0) * .8f,
            "planning edge visibly renders accent pixels along at least 80 percent of its expected area");
    }

    private void CheckReviewInstruction(bool needsAcknowledgement)
    {
        var reviewView = (AnnualReviewView)typeof(ScenarioOneUiRoot).GetField("review", AnyInstance).GetValue(ui);
        Label instruction = reviewView.Root.Q<Label>("next-action-reason");
        Check(instruction != null, "Annual Review has its own instruction label");
        Receipt("ANNUAL_REVIEW_INSTRUCTION", "\"needsAcknowledgement\":" + (needsAcknowledgement ? "true" : "false")
            + ",\"size\":\"" + Screen.width + "x" + Screen.height + "\",\"visible\":" + (Visible(instruction) ? "true" : "false") + ",\"instruction\":" + Q(instruction.text));
        Check(ui.NeedsAnnualReview == needsAcknowledgement, "Annual Review acknowledgement state matches capture");
        if (needsAcknowledgement)
            Check(Visible(instruction) && instruction.text == ScenarioOneRackBayText.ReviewOnScreenInstruction,
                "visible pending Annual Review instruction says Read and acknowledge the Annual Review");
        else
            Check(instruction.text.IndexOf("Read and acknowledge", StringComparison.OrdinalIgnoreCase) < 0,
                "acknowledged Annual Review instruction no longer says Read and acknowledge");
    }

    private string Rect(VisualElement e)
    {
        if (e == null || !Visible(e)) return "null";
        float scale = ui.RootElement.worldBound.width > 0 ? Screen.width / ui.RootElement.worldBound.width : 1f;
        Rect r = e.worldBound;
        return "{\"x\":" + F(r.x * scale) + ",\"y\":" + F(r.y * scale) + ",\"w\":" + F(r.width * scale) + ",\"h\":" + F(r.height * scale) + "}";
    }
    private string Bounds()
    {
        VisualElement mini = ui.Minimap != null ? ui.Minimap.Root : null;
        VisualElement surface = ui.Minimap != null ? ui.Minimap.Surface : null;
        return "\"screen\":\"" + Screen.width + "x" + Screen.height + "\",\"minimapPanel\":" + Rect(mini) + ",\"minimapSurface\":" + Rect(surface)
            + ",\"hudWaypoint\":" + Rect(ui.RootElement.Q("hud-waypoint-panel"));
    }

    // Everything the player could use to find the next entrance, plus what the source returns.
    private string Guidance()
    {
        bool target = marks.TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem);
        VisualElement panel = ui.RootElement.Q("hud-waypoint-panel");
        Label line = ui.RootElement.Q<Label>(className: "waypoint-line");
        GameObject cue = GameObject.Find("Next rack entrance cue");
        bool miniDestination = ui.MapSource != null && ui.MapSource.Model != null && ui.MapSource.Model.HasWaypoint;
        string hudText = panel != null && Visible(panel) ? string.Join(" | ", panel.Query<TextElement>().ToList().Where(Visible).Select(t => t.text).Where(s => !string.IsNullOrEmpty(s))) : "";
        return "\"sourceTarget\":" + (target ? "true" : "false") + ",\"targetRackId\":" + (target ? rack.Id : 0) + ",\"targetTreeId\":" + Q(target ? tree.TreeId : "")
            + ",\"targetEntranceZ\":" + (target ? F(rack.Entrance.Z) : "null") + ",\"problem\":" + Q(problem) + ",\"cueRackId\":" + marks.CueRackId
            + ",\"cue3dActive\":" + (cue != null && cue.activeInHierarchy ? "true" : "false")
            + ",\"cue3dPos\":" + Q(cue != null ? cue.transform.position.ToString("F2") : "")
            + ",\"hudPanelVisible\":" + (panel != null && Visible(panel) ? "true" : "false") + ",\"hudLine\":" + Q(line != null ? line.text : "")
            + ",\"hudText\":" + Q(hudText) + ",\"minimapHasDestination\":" + (miniDestination ? "true" : "false")
            + ",\"fieldStatus\":" + Q(marks.RackFieldStatus) + ",\"instruction\":" + Q(ui.Learning.CurrentInstruction)
            + ",\"marked\":" + manager.MarkedRackEntranceCount + ",\"helpOpen\":" + (ui.Help.IsOpen ? "true" : "false")
            + ",\"cursorLocked\":" + (Cursor.lockState == CursorLockMode.Locked ? "true" : "false")
            + ",\"player\":" + Q(player.transform.position.ToString("F2"));
    }

    private string RackRows()
    {
        var rows = new List<string>();
        foreach (RackPlanRack r in manager.ConfirmedRacks.OrderBy(r => r.Id))
        {
            Vector3 tp = Tree(r.EntranceTreeId).transform.position;
            List<ScenarioOneWorkOrder> orders = manager.WorkOrders.Where(o => o.rackId == r.Id).ToList();
            rows.Add("{\"visitIndex\":" + marks.RackVisitIndex(r.Id) + ",\"rackId\":" + r.Id + ",\"entrance\":[" + F(r.Entrance.X) + "," + F(r.Entrance.Z) + "],\"far\":[" + F(r.Far.X) + "," + F(r.Far.Z)
                + "],\"entranceTreeId\":" + Q(r.EntranceTreeId) + ",\"entranceTreePos\":[" + F(tp.x) + "," + F(tp.z) + "],\"entranceMarked\":"
                + (r.EntranceMarked ? "true" : "false") + ",\"removalTrees\":" + r.RemovalTreeIds.Count + ",\"orders\":" + orders.Count
                + ",\"ordersPending\":" + orders.Count(o => o.status == ScenarioWorkStatus.Pending)
                + ",\"ordersMatchRemovalSet\":" + (new HashSet<string>(orders.Select(o => o.targetTreeId)).SetEquals(r.RemovalTreeIds) && orders.Count == r.RemovalTreeIds.Count ? "true" : "false")
                + ",\"entranceTreeOrderRackId\":" + (manager.WorkOrders.Where(o => o.targetTreeId == r.EntranceTreeId).Select(o => o.rackId).DefaultIfEmpty(0).First())
                + ",\"locked\":" + (r.IsLocked ? "true" : "false") + "}");
        }
        return "[" + string.Join(",", rows) + "]";
    }

    private string GeometryState()
        => string.Join(";", manager.ConfirmedRacks.OrderBy(r => r.Id).Select(r => r.Id + ":" + r.EntranceTreeId + ":" + F(r.Entrance.X) + "," + F(r.Entrance.Z) + ">"
            + F(r.Far.X) + "," + F(r.Far.Z) + ":" + string.Join(",", r.RemovalTreeIds.OrderBy(i => i, StringComparer.Ordinal))))
           + "|orders:" + string.Join(";", manager.WorkOrders.OrderBy(o => o.workOrderId).Select(o => o.workOrderId + ":" + o.rackId + ":" + o.targetTreeId + ":" + o.status));

    // ---- lifecycle ------------------------------------------------------------------------------------------------------
    private IEnumerator Start()
    {
        yield return Settle(6);
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Path.GetTempPath(), "w438");
        Directory.CreateDirectory(output);
        var stack = new Stack<IEnumerator>(); stack.Push(Run()); Exception failure = null;
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) stack.Pop(); else if (current is IEnumerator next) stack.Push(next); else yield return current;
        }
        if (failure == null && feedbackFailures.Count > 0)
            failure = new InvalidOperationException("POST-SPRAY FEEDBACK: " + string.Join("; ", feedbackFailures));
        if (failure != null && ui != null)
        {
            SetGameSize(1280, 720);
            yield return Settle(6);
            yield return new WaitForEndOfFrame();
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            if (image != null) { File.WriteAllBytes(Path.Combine(output, "FAIL-state-1280x720.png"), image.EncodeToPNG()); Destroy(image); }
        }
        bool saveWritten = !string.IsNullOrEmpty(savePath) && File.Exists(savePath);
        File.WriteAllText(Path.Combine(output, "native-receipts.json"), "{\"mismatches\":[" + string.Join(",", mismatches.Select(Q))
            + "],\"feedbackFailures\":[" + string.Join(",", feedbackFailures.Select(Q)) + "],\"saveWritten\":"
            + (saveWritten ? "true" : "false") + ",\"failure\":" + Q(failure != null ? failure.ToString() : "") + ",\"receipts\":[\n" + string.Join(",\n", receipts) + "\n]}\n");
        string driver = Environment.GetEnvironmentVariable("CCF_DRIVER_DIR");
        if (!string.IsNullOrEmpty(driver)) File.WriteAllText(Path.Combine(driver, "finished"), failure == null ? "done" : "fail");
        if (failure != null) Debug.Log("OBJECTIVE1_UX_NATIVE_FAIL checks=" + checks + " exception=" + failure);
        else Debug.Log("OBJECTIVE1_UX_NATIVE_PASS checks=" + checks + " mismatches=" + mismatches.Count + " saveWritten=" + saveWritten);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(ui != null, "Scenario One ForestTest scene");
        manager = ui.Manager; marks = ui.Marking; player = ui.Player;
        saves = FindFirstObjectByType<ForestSaveController>();
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(config) + Path.DirectorySeparatorChar, StringComparison.Ordinal),
            "isolated save slot: " + Application.persistentDataPath);
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        Check(!File.Exists(savePath), "the isolated save slot starts empty");

        // 1. fresh game from the real startup screen
        StandaloneSessionMenu menu = null;
        for (int frame = 0; frame < 900; frame++)
        {
            menu = FindFirstObjectByType<StandaloneSessionMenu>();
            if (menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive) break;
            yield return null;
        }
        Check(menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive, "the real startup screen opened");
        Button start = menu.GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList().FirstOrDefault(b => b.text == "Start New Scenario");
        yield return Invoke(start, "Start New Scenario");
        yield return Settle(10);
        Check(ui.Help.Onboarding.IsOpen, "scenario onboarding is open");
        yield return Native(Key("Right"), Key("Right"));
        Check(ui.Help.Onboarding.PageIndex == 2, "third intro page reached with native arrows");
        yield return Shoot("g01-third-intro-page");
        yield return Invoke(ui.Help.Root.Q<Button>("onboarding-next"), "Begin from third intro page");
        yield return CloseHelp();
        yield return Wait(.4f);
        clearingSpawn = player.transform.position;
        freshJson = JsonUtility.ToJson(saves.CaptureData());
        Check(ui.ReduceMotion, "fresh profile Reduce Motion defaults ON");
        VisualElement objective = ui.RootElement.Q("active-objective-widget");
        Check(objective.resolvedStyle.borderTopWidth >= 2f, "initial HUD Objectives cue visible before first Objectives visit");
        yield return Shoot("g02-initial-objectives-cue-static");
        ui.SetReduceMotion(false); yield return Wait(.35f);
        yield return Shoot("g03-initial-objectives-cue-motion");
        ui.SetReduceMotion(true);
        yield return Native(Key("o"));
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Objectives, "native O opens Objectives");
        Check(Named("objective-primary-cta").ClassListContains("next-action"), "Objective 1 Stand Map CTA highlighted");
        Check(!ui.Learning.Root.Query<Label>(className: "next-action-badge").ToList().Any(Visible), "O1 CTA has no Next badge");
        yield return Shoot("g04-objectives");
        yield return DiagramAnchors();
        if (Environment.GetEnvironmentVariable("CCF_O1443_MODE") == "diagram-negative")
        {
            Receipt("DELTA_SCOPE", "\"mode\":\"diagram-negative\",\"coreRouteRepeated\":false");
            yield return NegativeControls();
            yield break;
        }
        yield return Invoke(Named("objective-primary-cta"), "Stand Map from Objectives");
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.StandMap, "first Stand Map help opens");
        yield return Shoot("g05-stand-map-help");
        yield return Invoke(ui.Help.Root.Q<Button>("menu-help-more"), "Stand Map More detail");
        Check(ui.Help.Root.Query<Label>(className: "keycap").ToList().Any(l => l.text == "M")
            && ui.Help.Root.Query<Label>(className: "keycap").ToList().Any(l => l.text == "Esc"), "Stand Map genuine keycaps present");
        yield return Shoot("g06-stand-map-help-detail");
        yield return CloseHelp();
        Check(Named("map-plan-rack").ClassListContains("next-action"), "first guided map highlights Plan Racks before any rack plan");
        yield return Shoot("g07-plan-racks-cue-static");
        ui.SetReduceMotion(false); yield return Wait(.4f); yield return Shoot("g08-plan-racks-cue-motion"); ui.SetReduceMotion(true);
        yield return Native(Key("m"));
        Check(manager.RackPlanningV2Active && manager.PlannedRackCount == 0 && manager.WorkOrders.Count == 0, "fresh Rack Planning V2 world, no racks, no work");
        Receipt("SPAWN", "\"player\":" + Q(player.transform.position.ToString("F2")) + ",\"headlandX\":" + F(ScenarioOneHeadlandGeometry.HeadlandX)
            + ",\"feederZ\":" + F(ScenarioOneHeadlandGeometry.FeederZ) + ",\"pathX\":" + F(ScenarioOneHeadlandGeometry.PathX)
            + ",\"southZ\":" + F(ScenarioOneHeadlandGeometry.SouthZ) + ",\"northZ\":" + F(ScenarioOneHeadlandGeometry.NorthZ));

        // 2. Stand Map -> Plan Racks -> suggested layout -> Confirm
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        yield return Settle(6);
        yield return Native(Key("m"));
        yield return CloseHelp();
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, "native M opens the Stand Map");
        yield return Invoke(Named("map-plan-rack"), "Plan Racks");
        yield return CloseHelp();
        yield return Shoot("g08a-plan-racks-editing");
        yield return Invoke(Named("map-edit-reset"), "Reset to suggested layout");
        if (ui.Map.RackEditor.ResetPending) yield return Invoke(Named("map-edit-reset-confirm"), "confirm reset");
        IReadOnlyList<SuggestedRackSegment> suggestion = manager.GetSuggestedRackLayout();
        Check(suggestion.Count == 5 && manager.PlannedRackCount == 5 && manager.RackLayoutMatchesSuggestion(suggestion), "five suggested racks planned");
        Receipt("SUGGESTION_ORDER", "\"segmentsInReturnedOrder\":[" + string.Join(",", suggestion.Select(s => "[" + F(s.Entrance.X) + "," + F(s.Entrance.Z) + "]")) + "]");
        yield return Invoke(Named("map-rack-plan-confirm"), "Confirm Rack Plan");
        yield return Settle(6);
        Check(manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "plan confirmed with five racks");
        Receipt("RACKS_AT_CONFIRM", "\"racks\":" + RackRows());
        string geometryAtConfirm = GeometryState();
        yield return Shoot("w01-map-confirmed");

        // Physical order: south-to-north by entrance Z, and walking distance along the headland from the feeder junction.
        List<RackPlanRack> southToNorth = manager.ConfirmedRacks.OrderBy(r => r.Entrance.Z).ToList();
        Receipt("PHYSICAL_ORDER", "\"southToNorthRackIds\":[" + string.Join(",", southToNorth.Select(r => r.Id)) + "],\"byStableId\":["
            + string.Join(",", manager.ConfirmedRacks.OrderBy(r => r.Id).Select(r => r.Id)) + "],\"distanceFromFeederJunctionM\":{"
            + string.Join(",", manager.ConfirmedRacks.OrderBy(r => r.Id).Select(r => "\"" + r.Id + "\":" + F(Mathf.Abs(r.Entrance.Z - ScenarioOneHeadlandGeometry.FeederZ)
                + Mathf.Abs(r.Entrance.X - ScenarioOneHeadlandGeometry.HeadlandX)))) + "},\"belowFeederRackIds\":["
            + string.Join(",", manager.ConfirmedRacks.Where(r => r.Entrance.Z < ScenarioOneHeadlandGeometry.FeederZ).Select(r => r.Id)) + "]");

        // 3. back to the forest: Tree Marking help, then Esc as the player did
        yield return Native(Key("m"));
        yield return Settle(10);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, "native M closes the map");
        Receipt("AFTER_MAP_CLOSE", "\"helpOpen\":" + (ui.Help.IsOpen ? "true" : "false") + ",\"helpMenu\":" + Q(ui.Help.IsOpen ? ui.Help.CurrentMenu.ToString() : "")
            + "," + Guidance());
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeMarks, "Tree Marking introduction opens before spraying");
        yield return ModalContainment("tree-marking");
        yield return Shoot("w02-tree-marking-intro");
        if (ui.Help.IsOpen) yield return Native(Key("Escape"));
        yield return Settle(8);
        Receipt("AFTER_ESC", "\"helpOpenAfterEsc\":" + (ui.Help.IsOpen ? "true" : "false"));
        yield return CloseHelp();
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        yield return Wait(0.6f);

        // 4. BEFORE the first spray
        Check(Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z), new Vector2(clearingSpawn.x, clearingSpawn.z)) < .2f,
            "still at genuine central-clearing spawn before first spray");
        Check(manager.MarkedRackEntranceCount == 0 && !ui.Help.IsOpen, "zero sprays and first marking help dismissed");
        Receipt("BEFORE_FIRST_SPRAY", Guidance());
        yield return Shoot("w03-before-first-spray");

        // 5. the player's sequence: spray the physically lowest entrance first
        yield return InvalidTargetProbe();
        RackPlanRack lowest = southToNorth[0];
        ForestTree lowestTree = Tree(lowest.EntranceTreeId);
        yield return Aim(lowestTree, false);
        yield return Native(Key("p"));
        Check(marks.RackSprayEquipped, "native P equips the spray marker");
        yield return Aim(lowestTree, true);
        Receipt("AIM_LOWEST", "\"rackId\":" + lowest.Id + ",\"treeId\":" + Q(lowestTree.TreeId) + ",\"prompt\":" + Q(marks.RackSprayPrompt()) + "," + Guidance());
        yield return Shoot("w04-aim-lowest-entrance");
        yield return SprayAndRecord(lowestTree, "FIRST_SPRAY_LOWEST");
        yield return Wait(0.4f);
        Receipt("AFTER_FIRST_SPRAY", Guidance());
        Check(manager.MarkedRackEntranceCount == 1 && CurrentOrdinal() == 2, "bottom first: count 1/5 and next Rack 2");
        yield return Shoot("w05-after-first-spray");
        RackPlanRack far = southToNorth[4];
        ForestTree farTree = Tree(far.EntranceTreeId);
        yield return Aim(farTree, true);
        Check(marks.RackSprayPrompt() == "Rack mark · [Left-click] Spray orange · [P] Stow" && UnnumberedRackLabel(marks.RackSprayPrompt()), "far tree prompt is nonempty and unnumbered before second physical spray");
        yield return Shoot("g09-top-rack5-before-second-spray");
        yield return SprayAndRecord(farTree, "SECOND_SPRAY_TOP");
        yield return Wait(.4f);
        Check(manager.MarkedRackEntranceCount == 2 && CurrentOrdinal() == 2, "top second: count 2/5 and next still Rack 2");
        Check(marks.RackSprayPrompt() == "Rack mark already made · Orange · [P] Stow" && UnnumberedRackLabel(marks.RackSprayPrompt()), "already marked far tree has nonempty unnumbered completed prompt");
        yield return Native(Click());
        Check(manager.MarkedRackEntranceCount == 2 && CurrentOrdinal() == 2, "repeat spray does not change derived count or next target");
        Receipt("REPEAT_SPRAY", "\"count\":" + manager.MarkedRackEntranceCount + "," + Guidance());
        Receipt("AFTER_TOP_SECOND_SPRAY", "\"sprayPrompt\":" + Q(marks.RackSprayPrompt()) + "," + Guidance());
        yield return Shoot("g10-top-second-count2-next-rack2");

        // 6. follow the cue for the rest
        for (int guard = 0; guard < 6 && manager.MarkedRackEntranceCount < 5; guard++)
        {
            Check(marks.TryGetRackFieldTarget(out RackPlanRack cued, out ForestTree target, out string problem), "a next target exists: " + problem);
            if (cued.EntranceMarked) Mismatch("cue points at an already-marked rack " + cued.Id);
            if (target.TreeId != cued.EntranceTreeId) Mismatch("cued tree " + target.TreeId + " is not rack " + cued.Id + " entrance " + cued.EntranceTreeId);
            Receipt("CUE_BEFORE_SPRAY", "\"step\":" + (manager.MarkedRackEntranceCount + 1) + "," + Guidance());
            yield return Aim(target, true);
            yield return SprayAndRecord(target, "SPRAY_FOLLOWING_CUE");
            if (manager.MarkedRackEntranceCount == 2) yield return Shoot("w06-after-second-spray");
        }
        yield return Wait(0.4f);
        Check(manager.MarkedRackEntranceCount == 5, "five entrances marked");
        Receipt("AT_5_OF_5", Guidance());
        yield return Native(Key("p"));
        yield return Wait(.4f);
        Check(ui.RootElement.Q("active-objective-instruction").Query<Label>(className: "keycap").ToList().Any(l => l.text == "Tab"), "5/5 HUD instruction renders Tab keycap");
        yield return Shoot("g11-five-of-five-tab-keycap");

        // 7. read-only association check: geometry and orders unchanged apart from marks, every order still Pending
        Receipt("RACKS_AT_5_OF_5", "\"racks\":" + RackRows());
        string geometryNow = GeometryState();
        if (geometryNow != geometryAtConfirm) Mismatch("rack geometry/removal sets/orders changed during marking");
        foreach (RackPlanRack r in manager.ConfirmedRacks)
        {
            if (!r.EntranceMarked) Mismatch("rack " + r.Id + " not marked at 5/5");
            List<ScenarioOneWorkOrder> orders = manager.WorkOrders.Where(o => o.rackId == r.Id).ToList();
            if (!new HashSet<string>(orders.Select(o => o.targetTreeId)).SetEquals(r.RemovalTreeIds)) Mismatch("rack " + r.Id + " orders differ from its removal set");
            if (orders.Any(o => o.status != ScenarioWorkStatus.Pending)) Mismatch("rack " + r.Id + " has non-Pending orders before approval");
            if (!orders.Any(o => o.targetTreeId == r.EntranceTreeId)) Mismatch("rack " + r.Id + " has no order for its own entrance tree");
        }
        if (manager.WorkOrders.Any(o => o.rackId <= 0)) Mismatch("loose non-rack work orders exist");
        Receipt("GEOMETRY_HASH", "\"atConfirm\":" + Q(Hash(geometryAtConfirm)) + ",\"at5of5\":" + Q(Hash(geometryNow)));

        Cursor.lockState = CursorLockMode.Locked;
        yield return Native(Key("Tab"));
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WorkPlan, "Work Plan first-use help opens");
        yield return ModalContainment("work-plan");
        yield return Shoot("g12-work-plan-first-help");
        yield return CloseHelp();
        Check(manager.WorkPlanOpen, "native Tab opens the Work Plan before approval");
        yield return Wait(0.4f);
        string plan = Text();
        Receipt("WORK_PLAN_TEXT", "\"rackLines\":[" + string.Join(",", plan.Split('\n').Where(l => l.IndexOf("rack", StringComparison.OrdinalIgnoreCase) >= 0).Take(40).Select(Q)) + "]");
        yield return Shoot("w07-work-plan-pending-not-approved");
        File.WriteAllText(Path.Combine(output, "work-plan-visible-text.txt"), plan);
        Check(manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Pending), "all work still Pending before approval");
        yield return AnnualExecution();
        yield return EditedIdPlan();
        yield return NegativeControls();
    }

    private int CurrentOrdinal()
        => marks.TryGetRackFieldTarget(out RackPlanRack r, out _, out _) ? marks.RackVisitIndex(r.Id) : 0;

    private void CheckFirstArrowPixels(Texture2D image)
    {
        VisualElement arrow = ui.RootElement.Q("hud-waypoint-arrow");
        Check(Visible(arrow), "actual arrow element displayed with nonzero rectangle (correct panel query)");
        Check(Visible(ui.RootElement.Q("hud-waypoint-panel")), "actual named HUD panel visible");
        Check(CurrentOrdinal() == 1 && ui.RootElement.Q<Label>("hud-waypoint-label").text.StartsWith("Next rack mark · ")
            && ui.RootElement.Q<Label>("hud-waypoint-label").text.EndsWith(" m"), "HUD label is unnumbered southern rack mark with distance");
        Check(ui.MapSource.Model.HasWaypoint && ui.MapSource.RackFieldTargetOrdinal == 1
            && Visible(ui.RootElement.Q("minimap-rack-target")) && ui.RootElement.Q<Label>("minimap-rack-label").text == "Rack mark", "matching unnumbered minimap destination visible before first spray");
        float scale = image.width / ui.RootElement.worldBound.width;
        Rect rect = arrow.worldBound; int yellow = 0;
        for (int y = Mathf.Max(0, Mathf.CeilToInt(rect.yMin * scale)); y < Mathf.Min(image.height, Mathf.FloorToInt(rect.yMax * scale)); y++)
            for (int x = Mathf.Max(0, Mathf.CeilToInt(rect.xMin * scale)); x < Mathf.Min(image.width, Mathf.FloorToInt(rect.xMax * scale)); x++)
            {
                Color c = image.GetPixel(x, image.height - 1 - y);
                if (c.r > .65f && c.g > .45f && c.b < .55f && c.r > c.g && c.g > c.b * 1.2f) yellow++;
            }
        Check(yellow >= 35, "rendered yellow arrow graphic pixels=" + yellow + " at " + image.width + "x" + image.height);
        Receipt("ARROW_PIXELS", "\"size\":" + Q(image.width + "x" + image.height) + ",\"yellowPixels\":" + yellow + ",\"arrowRect\":" + this.Rect(arrow));
    }

    private void CheckMinimapFit(string state)
    {
        VisualElement mini = ui.Minimap.Surface;
        Check(Visible(mini), "minimap visible " + state);
        Rect rect = mini.worldBound;
        foreach (VisualElement other in new[] { ui.RootElement.Q("hud-context-actions"), ui.RootElement.Q("hud-right-column"), ui.RootElement.Q(className: "reticle") })
            Check(!Visible(other) || !ui.Minimap.Root.worldBound.Overlaps(other.worldBound), "minimap does not overlap " + other?.name + " " + state);
        float scale = Screen.width / ui.RootElement.worldBound.width;
        float baseline = Screen.width == 1280 ? 157f * 156f : 235f * 236f;
        float ratio = rect.width * rect.height * scale * scale / baseline;
        Check(ratio >= 2.3f && ratio <= 2.6f, "measured minimap surface area ratio=" + F(ratio));
        Receipt("MINIMAP_AREA", "\"state\":" + Q(state) + ",\"size\":" + Q(Screen.width + "x" + Screen.height)
            + ",\"surface\":" + this.Rect(mini) + ",\"areaRatio\":" + F(ratio) + ",\"bottomBar\":" + this.Rect(ui.RootElement.Q("hud-context-actions"))
            + ",\"rightColumn\":" + this.Rect(ui.RootElement.Q("hud-right-column")) + ",\"reticle\":" + this.Rect(ui.RootElement.Q(className: "reticle")));
    }

    private IEnumerator CaptureCurrent(string state)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width == Screen.width && image.height == Screen.height, "rendered " + state);
        File.WriteAllBytes(Path.Combine(output, state + "-" + Screen.width + "x" + Screen.height + ".png"), image.EncodeToPNG());
        Destroy(image);
    }

    private IEnumerator DiagramAnchors()
    {
        foreach (Vector2Int size in Sizes)
        {
            SetGameSize(size.x, size.y); yield return Settle(16);
            ui.ShowObjectives(); yield return Settle(16);
            Rect anchor = ui.Learning.Root.Q("teaching-diagram").worldBound;
            for (int cycle = 1; cycle <= 3; cycle++)
            {
                Foldout detail = ui.Learning.Root.Q<Foldout>("objective-more-detail");
                Rect viewport = ui.Learning.Root.Q<ScrollView>().contentViewport.worldBound;
                Check(Visible(detail) && detail.Q<Toggle>().worldBound.yMax <= viewport.yMax, "More detail toggle visible without scrolling " + size + " cycle " + cycle);
                detail.value = true; yield return Settle(12);
                if (cycle == 1) yield return CaptureCurrent("g04c-objectives-detail-open");
                Rect opened = ui.Learning.Root.Q("teaching-diagram").worldBound;
                Check(Vector2.Distance(opened.position, anchor.position) <= 2f, "diagram anchor stable More detail open " + size + " cycle " + cycle);
                detail.value = false; yield return Settle(12);
                Rect closed = ui.Learning.Root.Q("teaching-diagram").worldBound;
                Check(Vector2.Distance(closed.position, anchor.position) <= 2f, "diagram anchor stable More detail close " + size + " cycle " + cycle);
                yield return Native(Key("Escape"), Key("o")); yield return Settle(12);
                Rect reopened = ui.Learning.Root.Q("teaching-diagram").worldBound;
                Check(Vector2.Distance(reopened.position, anchor.position) <= 2f, "diagram anchor stable Esc/reopen " + size + " cycle " + cycle);
                if (cycle == 3) yield return CaptureCurrent("g04d-objectives-final-anchor");
                Receipt("DIAGRAM_ANCHOR", "\"size\":" + Q(size.x + "x" + size.y) + ",\"cycle\":" + cycle + ",\"x\":" + F(reopened.x) + ",\"y\":" + F(reopened.y));
            }
        }
        yield return Shoot("g04b-objectives-after-detail-and-reopen");
    }

    private IEnumerator ModalContainment(string label)
    {
        Check(ui.Help.Root.resolvedStyle.backgroundColor.a >= .9f, label + " backdrop alpha=" + ui.Help.Root.resolvedStyle.backgroundColor.a);
        Check(ui.Help.Root.Q("menu-help-window").resolvedStyle.backgroundColor.a >= .99f, label + " foreground opaque");
        int count = manager.MarkedRackEntranceCount; bool plan = manager.WorkPlanOpen; var screen = ui.CurrentScreen;
        string signature = GeometryState();
        yield return Native(Key("m"), Key("o"), Key("Tab"), Key("p"), new Op {op="move",x=8,y=300}, Click());
        Check(ui.Help.IsOpen && manager.WorkPlanOpen == plan && ui.CurrentScreen == screen && manager.MarkedRackEntranceCount == count
            && GeometryState() == signature, label + " contains menu/marking keys and backdrop click");
        Receipt("MODAL_CONTAINMENT", "\"label\":" + Q(label) + ",\"alpha\":" + F(ui.Help.Root.resolvedStyle.backgroundColor.a) + ",\"contained\":true");
    }

    private IEnumerator AnnualExecution()
    {
        List<string> union = manager.ConfirmedRacks.SelectMany(r => r.RemovalTreeIds).OrderBy(i => i, StringComparer.Ordinal).ToList();
        Check(Named("workplan-approve").enabledInHierarchy, "Approve enabled after 5/5");
        yield return Invoke(Named("workplan-approve"), "Approve Work Plan"); yield return Wait(.4f);
        Check(manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Approved) == union.Count
            && manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Approved && union.Contains(o.targetTreeId)), "approval commits exactly rack orders");
        yield return Shoot("g13-work-plan-approved");
        ScenarioHarvestJob quote = manager.GetHarvestQuote(true);
        long cashBefore = manager.CashCents, expectedNet = quote.RevenueCents - quote.CostCents;
        int year = ui.Ecology.EcologicalYear;
        yield return Invoke(Named("workplan-advance"), "actual Advance one year"); yield return Wait(.8f);
        Check(ui.Ecology.EcologicalYear == year + 1 && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review, "actual year advance opens Annual Review");
        Check(manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Completed) && manager.ConfirmedRacks.All(r => r.IsLocked), "rack work completed and locked");
        HashSet<string> stumps = new HashSet<string>(AllTrees().Where(t => t.IsStump).Select(t => t.TreeId));
        Check(union.All(stumps.Contains) && manager.CashCents - cashBefore == expectedNet, "own rack trees felled and quoted cash change applied");
        Check(ui.NeedsAnnualReview && !manager.AnnualReviewSeen, "latest results need acknowledgement");
        Check(ui.AnnualReviewInstruction == "Read and acknowledge the Annual Review.", "review screen instruction does not ask to open Work Plan");
        Check(!Text().Contains("Read and acknowledge the Annual Review: open the Work Plan [Tab]."), "visible Annual Review never gives Work Plan opening direction");
        yield return Shoot("g14-annual-review-before-acknowledgement");
        int reviewYear = ui.Ecology.EcologicalYear;
        Check(!ui.AdvanceFromWorkPlan() && ui.Ecology.EcologicalYear == reviewYear, "review-first gate still refuses another advance");
        yield return Invoke(Named("annual-review-acknowledge"), "I've read the annual results");
        Check(manager.AnnualReviewSeen && !ui.NeedsAnnualReview, "latest Annual Review acknowledged");
        yield return Shoot("g15-annual-review-acknowledged");
        Receipt("ANNUAL_EXECUTION", "\"year\":" + ui.Ecology.EcologicalYear + ",\"rackTrees\":" + union.Count + ",\"cashChange\":" + (manager.CashCents-cashBefore) + ",\"acknowledged\":true");
    }

    private IEnumerator EditedIdPlan()
    {
        ui.CloseAll();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(freshJson), false), "reload fresh isolated fixture for second plan");
        yield return Settle(20);
        // Use ordinary API authoring in reverse spatial order. No IDs or saved records are rewritten by the harness.
        IReadOnlyList<SuggestedRackSegment> segments = manager.GetSuggestedRackLayout();
        foreach (SuggestedRackSegment segment in segments.Reverse())
            Check(manager.AddRack(segment.Entrance, segment.Far).Status == RackPlanOperationStatus.Applied, "author second plan north to south through AddRack");
        Check(manager.ConfirmRackPlan().Status == RackPlanOperationStatus.Applied, "confirm second five-rack plan");
        ui.MapSource.MarkRacksDirty(); yield return Settle(16); yield return CloseHelp();
        List<RackPlanRack> ordered = ForestTreeMarkingManager.RackVisitOrder(manager.ConfirmedRacks).ToList();
        Check(!ordered.Select(r=>r.Id).SequenceEqual(new[]{1,2,3,4,5}), "second plan stable IDs differ from physical visit order");
        Receipt("SECOND_PLAN_IDENTITIES", "\"racks\":" + RackRows());
        string geometry = GeometryState();
        if (!marks.RackSprayEquipped) { Cursor.lockState=CursorLockMode.Locked; yield return Native(Key("p")); }
        // Deliberately spray the far/top rack FIRST: progress is independent of the navigation suggestion.
        foreach (RackPlanRack rack in new[] { ordered[4] }.Concat(ordered.Take(4)))
        {
            ForestTree tree=Tree(rack.EntranceTreeId); yield return Aim(tree,true);
            int ordinal=marks.RackVisitIndex(rack.Id);
            Check(marks.RackSprayPrompt() == "Rack mark · [Left-click] Spray orange · [P] Stow" && UnnumberedRackLabel(marks.RackSprayPrompt()), "second plan unnumbered prompt aims at physical ordinal "+ordinal+" owning stable rack "+rack.Id);
            Receipt("SECOND_PLAN_ORDINAL", "\"visitIndex\":"+ordinal+",\"stableRackId\":"+rack.Id+",\"treeId\":"+Q(tree.TreeId)+",\"prompt\":"+Q(marks.RackSprayPrompt()));
            yield return Shoot("g16-edited-plan-ordinal-"+ordinal+"-stable-"+rack.Id);
            yield return SprayAndRecord(tree,"SECOND_PLAN_SPRAY");
            if (ordinal == 5)
            {
                Check(manager.MarkedRackEntranceCount == 1 && CurrentOrdinal() == 1, "reversed plan top first counts one and still suggests bottom");
                yield return Shoot("450-reversed-top-first-count1");
            }
            else if (ordinal == 1)
            {
                Check(manager.MarkedRackEntranceCount == 2 && CurrentOrdinal() == 2, "reversed plan distinct bottom second counts two and suggests next south-to-north");
                yield return Shoot("450-reversed-second-count2");
            }
            Check(GeometryState()==geometry,"second plan geometry/removal sets/Pending orders unchanged by spray "+ordinal);
            List<ScenarioOneWorkOrder> own = manager.WorkOrders.Where(o=>o.rackId==rack.Id).ToList();
            Check(new HashSet<string>(own.Select(o=>o.targetTreeId)).SetEquals(rack.RemovalTreeIds)
                && own.All(o=>o.status==ScenarioWorkStatus.Pending) && own.Any(o=>o.targetTreeId==tree.TreeId), "sprayed tree still maps to its own stable rack and own Pending removal orders");
        }
        Check(manager.MarkedRackEntranceCount==5,"second plan all five own entrances marked");
        yield return Shoot("450-reversed-complete");
        Receipt("SECOND_PLAN_FINAL", "\"racks\":"+RackRows());
    }

    private IEnumerator NegativeControls()
    {
        ui.CloseAll();
        ForestSaveData old = JsonUtility.FromJson<ForestSaveData>(freshJson);
        old.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        string input = JsonUtility.ToJson(old);
        Check(saves.LoadData(old,false), "old-world model-0 Enlarged80 fixture loads"); yield return Settle(20);
        Check(!manager.RackPlanningV2Active && !marks.TryGetRackFieldTarget(out _,out _,out _)
            && !ui.MapSource.Model.HasWaypoint && ui.MapSource.RackFieldTargetOrdinal==0, "old-world load has no rack field or minimap target");
        Check(!ui.Help.IsOpen || ui.Help.CurrentMenu != MenuHelpView.Menu.TreeMarks, "old-world never opens Tree Marks");
        Check(JsonUtility.ToJson(old)==input && !File.Exists(savePath), "old-world input and isolated disk slot not modified");
        yield return Shoot("g17-old-world-no-rack-target");
        Check(manager.TryBeginReferencePreview(50), "frozen Reference Future v1 preview opens"); yield return Wait(1f);
        Check(manager.ReferencePreviewActive && !marks.TryGetRackFieldTarget(out _,out _,out _)
            && ui.MapSource.RackFieldTargetOrdinal==0, "Reference Future has no rack target");
        yield return Shoot("g18-reference-future-untouched");
        manager.EndReferencePreview(); yield return Wait(.5f);
        Check(!manager.ReferencePreviewActive && !File.Exists(savePath), "Reference preview exits without saving");
        Receipt("NEGATIVE_CONTROLS", "\"oldWorldLoad\":true,\"oldWorldUnmodified\":true,\"referencePreview\":true,\"diskSaveWritten\":false");
    }

    private static string Hash(string s)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
            return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "").Substring(0, 16);
    }

    private IEnumerator SprayAndRecord(ForestTree tree, string label)
    {
        var before = manager.ConfirmedRacks.ToDictionary(r => r.Id, r => r.EntranceMarked);
        int countBefore = manager.MarkedRackEntranceCount;
        double requestedAt = Time.realtimeSinceStartupAsDouble;
        yield return NativeWithSettle(0, Click());
        yield return new WaitForEndOfFrame();
        var newly = manager.ConfirmedRacks.Where(r => r.EntranceMarked && !before[r.Id]).ToList();
        RackPlanRack owner = manager.ConfirmedRacks.FirstOrDefault(r => r.EntranceTreeId == tree.TreeId);
        bool painted = tree.transform.Find("Orange Rack Entrance Mark") != null;
        Receipt(label, "\"sprayedTreeId\":" + Q(tree.TreeId) + ",\"treeOwnerRackId\":" + (owner != null ? owner.Id : 0)
            + ",\"newlyMarkedRackIds\":[" + string.Join(",", newly.Select(r => r.Id)) + "],\"countBefore\":" + countBefore + ",\"countAfter\":" + manager.MarkedRackEntranceCount
            + ",\"orangePaintOnSprayedTree\":" + (painted ? "true" : "false") + ",\"message\":" + Q(marks.TransientMessage));
        if (newly.Count != 1) Mismatch(label + ": expected exactly one newly marked rack, got " + newly.Count);
        else if (owner == null || newly[0].Id != owner.Id || newly[0].EntranceTreeId != tree.TreeId)
            Mismatch(label + ": tree " + tree.TreeId + " marked rack " + newly[0].Id + " (owner " + (owner != null ? owner.Id : 0) + ")");
        if (!painted) Mismatch(label + ": no orange paint on " + tree.TreeId);
        int ordinal = marks.RackVisitIndex(owner.Id);
        string transient = marks.TransientMessage;
        Check(UnnumberedRackLabel(transient), "post-spray feedback is nonempty and has no rack identity number");
        VisualElement hudMessageRow = ui.RootElement.Q(className: "hud-message");
        Label hudMessage = hudMessageRow?.Q<Label>();
        foreach (Vector2Int size in Sizes)
        {
            if (Screen.width != size.x || Screen.height != size.y)
            {
                SetGameSize(size.x, size.y);
                yield return Settle(2);
                yield return new WaitForEndOfFrame();
            }
            string file = label.ToLowerInvariant() + "-post-spray-rack-" + ordinal + "-stable-" + owner.Id + "-" + size.x + "x" + size.y + ".png";
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            double elapsed = Time.realtimeSinceStartupAsDouble - requestedAt;
            Check(image != null && image.width == size.x && image.height == size.y, "post-spray rendered capture " + file);
            File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
            Destroy(image);
            Receipt("POST_SPRAY_CAPTURE", "\"file\":" + Q(file) + ",\"visitIndex\":" + ordinal + ",\"stableRackId\":" + owner.Id
                + ",\"treeId\":" + Q(tree.TreeId) + ",\"secondsSinceInputRequest\":" + elapsed.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture)
                + ",\"transientMessage\":" + Q(marks.TransientMessage) + ",\"hudMessage\":" + Q(hudMessage != null ? hudMessage.text : ""));
            Check(elapsed < 2.5, "post-spray capture within 2.5 seconds of input request for Rack " + ordinal + " at " + size);
            Check(!string.IsNullOrEmpty(marks.TransientMessage) && Visible(hudMessageRow) && hudMessage != null && hudMessage.text == transient,
                "post-spray capture shows the live HUD transient message for Rack " + ordinal + " at " + size);
        }
        SetGameSize(1280, 720);
        yield return Settle(2);
        var planView = (WorkPlanView)typeof(ScenarioOneUiRoot).GetField("workPlan", AnyInstance).GetValue(ui);
        planView.Refresh(true);
        var planFeedback = (Label)typeof(WorkPlanView).GetField("feedback", AnyInstance).GetValue(planView);
        Receipt("POST_SPRAY_FEEDBACK", "\"visitIndex\":" + ordinal + ",\"stableRackId\":" + owner.Id + ",\"treeId\":" + Q(tree.TreeId)
            + ",\"transientMessage\":" + Q(transient) + ",\"hudMessage\":" + Q(hudMessage.text) + ",\"managerFeedback\":" + Q(manager.Feedback)
            + ",\"workPlanFeedback\":" + Q(planFeedback.text) + ",\"workPlanLineVisible\":" + (Visible(planFeedback) ? "true" : "false"));
        Check(!string.IsNullOrEmpty(planFeedback.text) && !string.IsNullOrEmpty(manager.Feedback), "post-spray Work Plan feedback recorded");
        CheckFeedbackOrdinal(transient, ordinal, label + " transient");
        CheckFeedbackOrdinal(hudMessage.text, ordinal, label + " HUD row");
        CheckFeedbackOrdinal(planFeedback.text, ordinal, label + " Work Plan line");
        CheckFeedbackOrdinal(manager.Feedback, ordinal, label + " manager feedback");
    }

    private static bool UnnumberedRackLabel(string text)
        => !string.IsNullOrWhiteSpace(text) && !System.Text.RegularExpressions.Regex.IsMatch(text,
            @"\brack\s+[0-9]+\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    private void CheckGuidedField(string state)
    {
        if (!manager.RackPlanningV2Active || !manager.RackPlanConfirmed) return;
        int count = manager.MarkedRackEntranceCount;
        string expected = "Rack marks: " + count + " of 5" + (count == 5 ? " — complete" : "");
        Check(ui.RackLayout.V2CounterText == expected && ui.Learning.CurrentProgress == expected,
            "derived rack mark counter and objective progress agree: " + state);
        Check(Text().Contains(expected) && UnnumberedRackLabel(Text()), "visible field HUD has count and no numbered rack identity: " + state);
        var panels = new[] { ui.RootElement.Q("hud-context-actions"), ui.RootElement.Q("hud-waypoint-panel"),
            ui.RootElement.Q("active-objective-widget"), ui.Minimap.Root };
        for (int i = 0; i < panels.Length; i++)
            for (int j = i + 1; j < panels.Length; j++)
                Check(!Visible(panels[i]) || !Visible(panels[j]) || !panels[i].worldBound.Overlaps(panels[j].worldBound),
                    "prompt/arrow/counter/minimap nonoverlap " + i + "/" + j + " " + state);
        bool target = marks.TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem);
        if (target)
        {
            RackPlanRack next = ForestTreeMarkingManager.RackVisitOrder(manager.ConfirmedRacks).First(r => !r.EntranceMarked && !r.IsLocked);
            Check(rack.Id == next.Id && tree.TreeId == next.EntranceTreeId && marks.CueRackId == rack.Id,
                "next suggestion keeps physical south-to-north tree/rack association " + state);
            Check(ui.MapSource.Model.HasWaypoint && ui.MapSource.RackFieldTargetOrdinal == marks.RackVisitIndex(rack.Id)
                && Vector2.Distance(new Vector2(tree.transform.position.x, tree.transform.position.z),
                    new Vector2(ui.MapSource.Model.Waypoint.X, ui.MapSource.Model.Waypoint.Z)) < .01f,
                "minimap destination is the exact target tree position " + state);
            Label arrow = ui.RootElement.Q<Label>("hud-waypoint-label"), label = ui.RootElement.Q<Label>("minimap-rack-label");
            float distance = Vector2.Distance(new Vector2(player.transform.position.x, player.transform.position.z),
                new Vector2(tree.transform.position.x, tree.transform.position.z));
            Check(Visible(arrow) && arrow.text == $"Next rack mark · {distance:0} m" && UnnumberedRackLabel(arrow.text)
                && Visible(label) && label.text == "Rack mark" && UnnumberedRackLabel(label.text), "nonempty unnumbered target labels retain correct distance " + state);
            var transform = new StandMapTransform(ui.MapSource.Model.MinX, ui.MapSource.Model.MinZ, ui.MapSource.Model.SizeM, HudMinimapView.SidePx);
            MapPosition at = transform.WorldToPixels(ui.MapSource.Model.Waypoint);
            VisualElement ring = ui.RootElement.Q("minimap-rack-target");
            Check(Visible(ring) && Mathf.Abs(ring.style.left.value.value - (at.X - 12f)) < .01f
                && Mathf.Abs(ring.style.top.value.value - (at.Y - 12f)) < .01f, "visible ring retains exact projected placement " + state);
            if (marks.RackSprayEquipped) Check(UnnumberedRackLabel(marks.RackSprayPrompt()), "nonempty unnumbered spray prompt " + state);
        }
        else Check(count == 5 || !string.IsNullOrEmpty(problem), "missing target is complete or fails closed " + state);
        Receipt("RACK_MARKS_FIELD", "\"state\":" + Q(state) + ",\"size\":" + Q(Screen.width + "x" + Screen.height)
            + ",\"counter\":" + Q(expected) + "," + Guidance());
    }

    private IEnumerator InvalidTargetProbe()
    {
        RackPlanRack first = ForestTreeMarkingManager.RackVisitOrder(manager.ConfirmedRacks).First();
        RackSaveData record = manager.Racks.Single(r => r.rackId == first.Id);
        string original = record.entranceTreeId;
        string geometry = GeometryState();
        const string stale = "450-MISSING-TREE";
        try
        {
            record.entranceTreeId = stale;
            Check(!marks.TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out string problem)
                && rack.Id == first.Id && tree == null && !string.IsNullOrEmpty(problem), "invalid membership fails closed on first rack without choosing another tree");
            record.removalTreeIds.Add(stale);
            yield return Settle(16);
            Check(!marks.TryGetRackFieldTarget(out rack, out tree, out problem) && rack.Id == first.Id && tree == null
                && problem.Contains("no longer standing"), "stale recorded tree fails closed despite removal membership");
            Check(marks.CueRackId == 0 && ui.MapSource.RackFieldTargetOrdinal == 0
                && !Visible(ui.RootElement.Q("minimap-rack-target")) && !Visible(ui.RootElement.Q("hud-waypoint-panel")),
                "stale target hides arrow/ring/cue instead of redirecting to a different rack");
            Check(manager.MarkedRackEntranceCount == 0, "invalid target probe adds no marks");
            Receipt("INVALID_TARGET", "\"fixtureOnly\":true,\"noDiskWrite\":true," + Guidance());
        }
        finally
        {
            record.entranceTreeId = original;
            record.removalTreeIds.Remove(stale);
        }
        yield return Settle(16);
        Check(GeometryState() == geometry && CurrentOrdinal() == 1 && marks.CueRackId == first.Id,
            "invalid fixture fully restored with original first target and associations");
    }

    private static bool RackNumbersMatchOrdinal(string text, int ordinal)
    {
        foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(text ?? "", @"\brack\s+([0-9]+)\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            if (!int.TryParse(match.Groups[1].Value, out int number) || number != ordinal) return false;
        return true;
    }

    private void CheckFeedbackOrdinal(string text, int ordinal, string label)
    {
        bool matches = RackNumbersMatchOrdinal(text, ordinal);
        Receipt("FEEDBACK_ORDINAL_ASSERTION", "\"label\":" + Q(label) + ",\"visitIndex\":" + ordinal + ",\"text\":" + Q(text) + ",\"pass\":" + (matches ? "true" : "false"));
        if (matches) Check(true, label + " names no rack number other than visit ordinal " + ordinal);
        else
        {
            string problem = label + " for visit ordinal " + ordinal + ": " + text;
            feedbackFailures.Add(problem);
            Debug.LogWarning("O1443_FEEDBACK_ASSERTION_FAIL " + problem);
        }
    }

    private IEnumerator Aim(ForestTree tree, bool requireValid)
    {
        var controller = player.GetComponent<CharacterController>();
        foreach (Vector3 direction in new[] { Vector3.left, Vector3.back, Vector3.forward, new Vector3(-1, 0, -1), new Vector3(-1, 0, 1), Vector3.right })
            foreach (float distance in new[] { 2.2f, 1.8f, 2.8f })
            {
                if (controller != null) controller.enabled = false;
                Vector3 p = tree.transform.position + direction.normalized * distance;
                player.transform.position = new Vector3(p.x, tree.transform.position.y + 0.2f, p.z);
                if (controller != null) controller.enabled = true;
                player.LookToward(tree.transform.position + Vector3.up * 1.4f);
                Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
                Physics.SyncTransforms();
                yield return Settle(10);
                player.LookToward(tree.transform.position + Vector3.up * 1.4f);
                yield return Settle(3);
                if (!requireValid || marks.RackSprayPrompt().Contains("Spray orange")) yield break;
            }
        if (requireValid) throw new InvalidOperationException("no clear spray vantage for " + tree.TreeId + ": " + marks.RackSprayPrompt());
    }
}
#endif
