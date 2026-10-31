#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// Disposable RACK-V2-04 Unity/rendered gate. Copy into Assets/ForestPrototype, run Begin, remove it and its .meta.
//
// In a fresh model-1 Enlarged80 world it adds ONE authoritative player rack as a fixture, snapshots the
// full live save, then exercises the Stand Map's Plan Racks mode: the tool/mode control, the dashed
// suggested layer built from ScenarioOneSuggestedRackLayout.Get, the legend wording, tab navigation
// remaining navigation, and the visual distinction from the solid player rack. At 1280x720 and
// 1920x1080 it captures the map before planning, planning active, the suggested layer with legend, and
// suggestions beside the solid fixture. Every save field, rack id/geometry/order lifecycle and counter
// is compared before and after all display work: a fixture change is the only mutation.
//
// Run (graphical Editor, private display, normal machine-wide lock held):
//   -executeMethod RackV2MapPlanningVerification.Begin   CCF_DIAG_DIR=<folder outside the repo>   CCF_UI_SIZES=1280x720,1920x1080
// Writes only its evidence folder. No Save(), scene save, annual advance or archive write.
public static class RackV2MapPlanningVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Vector2Int[] sizes = RackV2MapPlanningRunner.Sizes();
        RackV2MapPlanningRunner.SetGameSize(sizes[0].x, sizes[0].y);
        EditorApplication.isPlaying = true;
    }

    // First-use help is another harness's business; mark it seen so the map and legend are unobscured in the evidence.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "RackV2MapPlanningVerification.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
            PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "RackV2MapPlanningVerification.Begin"))
            new GameObject("Disposable RACK-V2-04 map planning gate").AddComponent<RackV2MapPlanningRunner>();
    }
}

public sealed class RackV2MapPlanningRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private int checks;
    private int shots;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private string output;
    private string baselineSave, baselineWorld, baselineCounters, baselineRacks;
    private readonly List<string> manifest = new List<string>(), notes = new List<string>(), stateLines = new List<string>();

    private void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
        checks++;
        Debug.Log("RACKV2MAP_CHECK " + message);
    }

    // The original five navigation tabs are the historical contract. MFM-08's Treatment review is additive and
    // only exists while the accepted Rack Planning V2 UI is active; it must never replace or rename one of them.
    private static readonly string[] OriginalLayerTabs =
    {
        "Access & position", "Light", "Regeneration", "Browsing / protection", "Fell & crop marks"
    };

    private void CheckLayerTabsContract(IReadOnlyList<string> labels, bool rackPlanningV2Active, string stage)
    {
        int expected = rackPlanningV2Active ? OriginalLayerTabs.Length + 1 : OriginalLayerTabs.Length;
        Check(labels.Count == expected, stage + " navigation tab count is " + expected
            + " (Rack Planning V2 active: " + rackPlanningV2Active + ")");
        for (int i = 0; i < OriginalLayerTabs.Length; i++)
            Check(labels[i] == OriginalLayerTabs[i], stage + " navigation tab " + (i + 1) + " remains '" + OriginalLayerTabs[i] + "'");
        if (rackPlanningV2Active)
            Check(labels[OriginalLayerTabs.Length] == "Treatment review", stage + " Treatment review is the additive V2 layer");
        else
            Check(!labels.Contains("Treatment review"), stage + " historical/non-MFM state has no Treatment review tab");
    }

    private void Note(string text) { notes.Add(text); Debug.Log("RACKV2MAP_NOTE " + text); }

    private static string Hash(string text)
    {
        using (SHA256 hash = SHA256.Create())
            return string.Concat(hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2")));
    }

    private void State(string text)
    {
        stateLines.Add("  " + text);
        Debug.Log("RACKV2MAP_STATE " + text);
    }

    // ---------- Editor window helpers (same approach as the Stand Map parity harness) ----------
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "RackV2Map " + width + "x" + height }, null);
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
        if (string.IsNullOrEmpty(text)) text = "1280x720,1920x1080";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
    }

    private static IEnumerator Settle(int frames = 6) { for (int i = 0; i < frames; i++) yield return null; }

    private static IEnumerator Wait(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }

    private static bool Visible(VisualElement e)
    {
        for (VisualElement p = e; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity <= 0.01f) return false;
        return e.worldBound.width > 0 && e.worldBound.height > 0;
    }

    private static void Invoke(Button button)
    {
        if (button == null) throw new InvalidOperationException("button not found");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
    }

    private IEnumerator ClickPlan()
    {
        Button plan = ui.Map.Root.Q<Button>("map-plan-rack");
        if (plan == null || !Visible(plan)) throw new InvalidOperationException("Plan Racks control not visible");
        Invoke(plan);
        yield return null; yield return null; yield return Settle(8);
    }

    private float PixelsPerUnit => ui.RootElement.worldBound.width > 0f ? Screen.width / ui.RootElement.worldBound.width : 1f;
    private string RectPx(VisualElement e)
    {
        if (e == null || !Visible(e)) return "null";
        float s = PixelsPerUnit; Rect r = e.worldBound;
        return "[" + (r.xMin * s).ToString("0.0", Inv) + "," + (r.yMin * s).ToString("0.0", Inv) + ","
            + (r.width * s).ToString("0.0", Inv) + "," + (r.height * s).ToString("0.0", Inv) + "]";
    }

    private IEnumerator Capture(string state, string shot)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null) throw new InvalidOperationException("no rendered capture for " + shot);
        if (image.width != Screen.width || image.height != Screen.height)
            throw new InvalidOperationException("capture " + image.width + "x" + image.height + " does not match game view " + Screen.width + "x" + Screen.height);
        string file = state + "-" + shot + "-" + Screen.width + "x" + Screen.height + ".png";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        Destroy(image);
        shots++;
        VisualElement frame = ui.Map != null ? ui.Map.Root.Q("map-frame") : null;
        Button plan = ui.Map != null ? ui.Map.Root.Q<Button>("map-plan-rack") : null;
        int suggested = ui.MapSource.Model.Racks.Count(r => r.Symbol == MapSymbol.RackSuggested);
        int player = ui.MapSource.Model.Racks.Count(r => r.Symbol == MapSymbol.RackPlayer);
        manifest.Add("  {\"file\":\"" + file + "\",\"state\":\"" + state + "\",\"shot\":\"" + shot + "\",\"width\":" + Screen.width
            + ",\"height\":" + Screen.height + ",\"mapFramePx\":" + RectPx(frame) + ",\"planControlPx\":" + RectPx(plan)
            + ",\"planActive\":" + (plan != null && plan.ClassListContains("btn-selected")).ToString().ToLowerInvariant()
            + ",\"suggestedRacks\":" + suggested + ",\"playerRacks\":" + player + "}");
    }

    // ---------- fixtures and snapshots ----------
    private static List<RackTree> MapTrees()
        => UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t != null && !string.IsNullOrEmpty(t.TreeId) && t.TreeId.StartsWith("P", StringComparison.Ordinal))
            .Select(t => new RackTree(t.TreeId, t.transform.position.x, t.transform.position.z)).ToList();

    private IReadOnlyList<SuggestedRackSegment> Suggestions()
        => ScenarioOneSuggestedRackLayout.Get(manager.RackPlanningModelVersion, ecology.StandGeometryModelVersion, MapTrees());

    private string SaveJson() => JsonUtility.ToJson(saves.CaptureData());
    private string Counters() => manager.PlannedRackCount + "/" + manager.NextRackId + "/" + manager.MarkedRackEntranceCount
        + "/" + manager.RackPlanConfirmed + "/" + manager.WorkOrders.Count;
    private string RackRecords() => string.Join(";", manager.RackPlan.Select(r =>
        r.Id + ":" + r.Status + ":" + r.Entrance.X.ToString("R", Inv) + "," + r.Entrance.Z.ToString("R", Inv) + ":"
        + r.Far.X.ToString("R", Inv) + "," + r.Far.Z.ToString("R", Inv) + ":" + r.WidthM.ToString("R", Inv)
        + ":" + r.EntranceMarked + ":" + r.Orders));

    private void Snapshot()
    {
        baselineSave = SaveJson();
        baselineWorld = ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData());
        baselineCounters = Counters();
        baselineRacks = RackRecords();
        State("fixture save=" + Hash(baselineSave) + " world=" + baselineWorld + " counters=" + baselineCounters
            + " orders=" + manager.WorkOrders.Count);
    }

    private void CheckNonMutation(string stage)
    {
        string after = SaveJson();
        Check(baselineSave == after, stage + " full serialized save identical (rack ids/geometry/status, marks, orders, world)");
        Check(baselineWorld == ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData()), stage + " world hash unchanged");
        Check(baselineCounters == Counters(), stage + " planned/marked counters, next id and plan confirmation unchanged");
        Check(baselineRacks == RackRecords(), stage + " authoritative rack records and order lifecycles unchanged");
        Check(manager.PlannedRackCount == 1 && manager.NextRackId == 2, stage + " only the one fixture rack exists");
        Check(manager.RackPlan.Count == 1 && manager.ConfirmedRacks.Count == 0 && !manager.RackPlanConfirmed,
            stage + " suggestions are not player/confirmed racks");
        State("non-mutation " + stage + " after=" + Hash(after) + " counters=" + Counters());
    }

    private static bool Same(RackPoint a, RackPoint b)
        => Mathf.Abs(a.X - b.X) < 0.0001f && Mathf.Abs(a.Z - b.Z) < 0.0001f;

    private void CheckSuggestedLayer(string stage, bool expected)
    {
        StandMapModel model = ui.MapSource.Model;
        List<MapRack> shown = model.Racks.Where(r => r.Symbol == MapSymbol.RackSuggested).ToList();
        List<MapRack> player = model.Racks.Where(r => r.Symbol == MapSymbol.RackPlayer).ToList();
        Check(player.Count == 1 && Same(player[0].Start, manager.RackPlan[0].Entrance) && Same(player[0].End, manager.RackPlan[0].Far),
            stage + " authoritative rack shown as one solid player rack");
        if (!expected)
        {
            Check(shown.Count == 0, stage + " no suggestions outside planning mode");
            return;
        }
        IReadOnlyList<SuggestedRackSegment> api = Suggestions();
        Check(api.Count == 5, stage + " foundation returns five suggested routes");
        Check(shown.Count == api.Count, stage + " dashed layer consumes exactly the foundation result");
        for (int i = 0; i < api.Count; i++)
        {
            Check(Same(shown[i].Start, api[i].Entrance) && Same(shown[i].End, api[i].Far)
                && Mathf.Abs(shown[i].WidthM - api[i].WidthM) < 0.0001f,
                stage + " suggested route " + i + " matches ScenarioOneSuggestedRackLayout.Get");
        }
        Check(manager.PlannedRackCount == 1, stage + " suggestions not counted as player racks");
    }

    // ---------- run loop ----------
    private IEnumerator Start()
    {
        for (int i = 0; i < 6; i++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "rack-v2-map-planning");
        Directory.CreateDirectory(output);
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        File.WriteAllText(Path.Combine(output, "shots.json"),
            "{\n \"sizes\": \"" + string.Join(",", Sizes().Select(s => s.x + "x" + s.y)) + "\",\n \"shots\": [\n"
            + string.Join(",\n", manifest) + "\n ],\n \"states\": [\n" + string.Join(",\n", stateLines) + "\n ]\n}\n");
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        Debug.Log(failure == null ? "RACK_V2_MAP_PLANNING_UNITY_PASS checks=" + checks + " shots=" + shots
            : "RACK_V2_MAP_PLANNING_UNITY_FAIL checks=" + checks + " shots=" + shots + " exception=" + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Path.GetFullPath(Application.persistentDataPath)
            .StartsWith(Path.GetFullPath(config) + Path.DirectorySeparatorChar, StringComparison.Ordinal), "isolated persistent data path");
        Check(ui != null && manager != null && ecology != null && saves != null, "Scenario One scene components present");
        Check(manager.RackPlanningV2Active && ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80,
            "fresh model-1 Enlarged80 rack world");
        Check(manager.PlannedRackCount == 0 && manager.NextRackId == 1 && !manager.RackPlanConfirmed,
            "empty authoritative plan before the fixture");
        Debug.Log("RACKV2MAP_START geometry=" + StandGeometryModel.Label(ecology.StandGeometryModelVersion)
            + " trees=" + ecology.LivingTreeCount + " grid=" + ecology.CellsPerAxis);

        IReadOnlyList<SuggestedRackSegment> api = Suggestions();
        Check(api.Count == 5, "five suggested routes available before any map work");
        RackPlanOperationResult added = manager.AddRack(api[0].Entrance, api[0].Far);
        Check(added.Succeeded, "non-destructive authoritative fixture rack added: " + added.Message);
        Check(manager.PlannedRackCount == 1 && manager.NextRackId == 2, "fixture allocates exactly one rack id");
        Snapshot();

        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y);
            ui.CloseAll(); yield return Settle(20);
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, size.x + "x" + size.y + " map closed before the stage");
            ui.ShowMap(); yield return Settle(20);
            // A first-use map help window is expected on a fresh device; close it so the captured map and legend are the subject.
            if (ui.Help != null && ui.Help.IsOpen) { ui.CloseHelp(); yield return Settle(12); }
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, size.x + "x" + size.y + " Stand Map opened");
            Check(ui.Help == null || !ui.Help.IsOpen, size.x + "x" + size.y + " no help window obscures the map");
            Check(!ui.Map.RackMode, size.x + "x" + size.y + " planning mode is off on entry");
            Check(Screen.width == size.x && Screen.height == size.y, "game view is " + size.x + "x" + size.y);

            // The control is a tool/mode sibling of the navigation tabs, not a tab.
            VisualElement tabs = ui.Map.Root.Q("map-tabs");
            VisualElement tools = ui.Map.Root.Q("map-tools");
            Button plan = ui.Map.Root.Q<Button>("map-plan-rack");
            Check(tabs != null && tools != null && plan != null && Visible(plan), "toolbar and Plan Racks control visible");
            Check(tabs.parent == tools.parent, "map tools and map tabs are siblings in the map panel");
            Check(plan.parent == tools, "Plan Racks belongs to the tool row");
            Check(!tabs.Query<Button>().ToList().Contains(plan), "Plan Racks is not one of the navigation tabs");
            Check(plan.text == "Plan Racks" && !plan.ClassListContains("btn-selected"), "inactive label is 'Plan Racks'");
            List<Button> tabsNow = tabs.Query<Button>().ToList();
            CheckLayerTabsContract(tabsNow.Select(b => b.text).ToList(), manager.RackPlanningV2Active, size.x + "x" + size.y);
            // The MFM-08 candidate always runs with Rack Planning V2 active, so also assert the historical/non-MFM
            // contract through the same predicate: five tabs and no Treatment review when V2 is not active.
            CheckLayerTabsContract(OriginalLayerTabs, false, "historical/non-MFM contract");
            int selectedBefore = tabsNow.FindIndex(b => b.ClassListContains("btn-selected"));
            yield return Capture(size.x + "x" + size.y, "map-before-planning");
            CheckSuggestedLayer(size.x + "x" + size.y + " before", false);

            yield return ClickPlan();
            plan = ui.Map.Root.Q<Button>("map-plan-rack");
            Check(ui.Map.RackMode, size.x + "x" + size.y + " Plan Racks activated the mode");
            Check(plan.text == "Planning Racks — Active" && plan.ClassListContains("btn-selected"), "active state is unmistakable");
            Check(ui.Map.Root.Q("map-tabs").Query<Button>().ToList().FindIndex(b => b.ClassListContains("btn-selected")) == selectedBefore,
                "activating the mode does not select a navigation tab");
            CheckSuggestedLayer(size.x + "x" + size.y + " active", true);
            List<string> legend = ui.Map.Legend.Root.Query<Label>().ToList()
                .Where(l => l.ClassListContains("map-legend-text")).Select(l => l.text).ToList();
            Check(legend.Contains("Dashed = suggested"), "legend states 'Dashed = suggested'");
            Check(legend.Contains("Solid = your planned rack"), "legend states 'Solid = your planned rack'");
            Check(ui.Map.Legend.RowCount >= 2, "legend rows rendered");
            Check(Visible(ui.Map.Legend.Root), "legend card is visible in the captured map");
            yield return Capture(size.x + "x" + size.y, "planning-active");

            // Navigation tabs still navigate while the mode stays active.
            VisualElement tabsAfter = ui.Map.Root.Q("map-tabs");
            List<Button> layerButtons = tabsAfter.Query<Button>().ToList();
            Invoke(layerButtons[1]); yield return Settle(12);
            Check(ui.Map.RackMode, "choosing the Light layer keeps planning mode active");
            CheckSuggestedLayer(size.x + "x" + size.y + " after-navigation", true);
            VisualElement tabsBack = ui.Map.Root.Q("map-tabs");
            Invoke(tabsBack.Query<Button>().ToList()[selectedBefore]); yield return Settle(12);
            Check(ui.Map.RackMode, "returning to the Access layer keeps planning mode active");
            CheckSuggestedLayer(size.x + "x" + size.y + " restored", true);
            legend = ui.Map.Legend.Root.Query<Label>().ToList().Where(l => l.ClassListContains("map-legend-text")).Select(l => l.text).ToList();
            Check(legend.Contains("Dashed = suggested") && legend.Contains("Solid = your planned rack"), "legend wording persists");
            yield return Capture(size.x + "x" + size.y, "suggested-layer-legend");

            // Leaving the mode removes only the guidance layer; the player rack and every record stay.
            yield return ClickPlan();
            Check(!ui.Map.RackMode, "Plan Racks toggles the mode off");
            CheckSuggestedLayer(size.x + "x" + size.y + " toggled-off", false);
            yield return ClickPlan();
            Check(ui.Map.RackMode, "re-entering planning mode restores guidance");
            CheckSuggestedLayer(size.x + "x" + size.y + " re-entered", true);
            yield return Capture(size.x + "x" + size.y, "suggested-vs-player");
            yield return ClickPlan();
            Check(!ui.Map.RackMode, "planning mode left before the next size");
        }

        ui.CloseAll(); yield return Settle(10);
        CheckNonMutation("all rendered display work");

        int captures = manifest.Count;
        Check(captures == Sizes().Length * 4, "four captures at each requested resolution: " + captures);
    }
}
#endif
