#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Profiling;
using Debug = UnityEngine.Debug;

// Disposable rendered verification for SCENARIO-ONE-PLAYER-TEST-REMEDIATION-V1-COMBINED-INTEGRATION: the two approved lanes MEET in the walking HUD
// (Lane A: active-objective widget + status panel + competitor/estimate panels in the top-right column; Lane B: the minimap at the bottom right), and
// the cursor/focus rules cross both (popups, the Stand Map, rack planning). Neither lane's own harness measures the combination, so this one does,
// in the real UI at 1280 x 720 and 1920 x 1080, against the real elements:
//   HUD   : objective widget, status panel and minimap visible, on screen and mutually clear at every one of the eight objective stages;
//   PANELS: the felling-estimate panel (every ordinary E-inspection) stays above the minimap and unclipped; the competitor list (a Crop Tree with
//           several competitors) is unclipped, the minimap steps aside for it and returns when the inspection ends (the R3 fix);
//   MAP   : the Stand Map window with the shared type scale: every layer's text ends inside the window, nothing in the side card runs past the card or
//           is cut off (found on the integrated tree by the Lane B parity gate: the legend ran past the window, the card heading past its card);
//   FOCUS : cursor lock, visibility, player input and HUD visibility across inspect -> More detail -> Objectives -> Stand Map -> rack planning ->
//           Work Plan -> help -> back to the forest, and a rapid open/close burst; the forest always gets its mouse back;
//   PERF  : a bounded DIAGNOSTIC frame-time / GC sample with the map closed (minimap painting while the player moves), standing still, and the
//           full Stand Map open. Editor frame times in a software-rendered nested display are not a player's frame times; there is no threshold.
// Plays the new game in memory and writes nothing but its evidence folder (no save is written).
// Run (graphical Editor, private Xephyr): -executeMethod CombinedHudComposition.Begin   CCF_ACCEPTANCE_OUTPUT=<folder>   [CCF_UI_SIZES=1280x720,1920x1080]   [CCF_COMBINED_SECTIONS=hud-stages,estimate,competitors,focus,map-window,perf]
// Results: CHC_* lines in the log, <name>-<w>x<h>.png screenshots, combined-hud-measure.csv, combined-hud-perf.csv.
public static class CombinedHudComposition
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        CombinedHudCompositionRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "CombinedHudComposition.Begin")) return;
        // Every first-use screen counts as seen, so none opens by itself over a measurement; the sections open the ones they need.
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "CombinedHudComposition.Begin"))
            new GameObject("Combined HUD composition").AddComponent<CombinedHudCompositionRunner>();
    }
}

public sealed class CombinedHudCompositionRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ForestPlayer player;
    private ScenarioOneManager manager;
    private string outDir;
    private int passes, failures, shotCount;
    private Vector2Int size;
    private Texture2D lastCapture;
    // The sizes run one after the other in the SAME game: the first rack, once made, stays made.
    private bool firstRackMade;
    private readonly List<string> measureRows = new List<string> { "state,size,measure,value" };
    private readonly List<string> perfRows = new List<string> { "size,mode,frames,meanMs,p50Ms,p95Ms,maxMs,gc0Collections,gcAllocBytesPerFrame" };

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Combined " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
    }

    private IEnumerator Start()
    {
        outDir = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT") ?? Path.Combine(Application.dataPath, "..", "Logs");
        Directory.CreateDirectory(outDir);
        for (int i = 0; i < 40; i++) yield return null;
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        player = FindFirstObjectByType<ForestPlayer>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        if (ui == null || player == null || manager == null) { Debug.Log("CHC_UNVERIFIED missing ui/player/manager"); Finish(1); yield break; }
        string rackModel = Environment.GetEnvironmentVariable("CCF_RACK_VERIFY_MODEL");
        Check(rackModel == "legacy" || rackModel == "v2", "EXPLICIT_RACK_MODEL_SELECTED");
        if (rackModel == "legacy")
        {
            ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
            ForestSaveData legacy = saves.CaptureData();
            legacy.version = 20;
            legacy.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
            legacy.scenarioOne.rackPlanConfirmed = false;
            legacy.scenarioOne.nextRackId = 1;
            legacy.scenarioOne.racks = new List<RackSaveData>();
            Check(saves.LoadData(legacy, false), "EXPLICIT_MODEL_0_COMPATIBILITY_FIXTURE_LOADS");
        }
        Check(rackModel == "legacy" ? !manager.RackPlanningV2Active && manager.RackPlanningModelVersion == RackPlanningModel.Legacy
            : manager.RackPlanningV2Active, "SELECTED_RACK_MODEL_IS_ACTIVE");
        // Traverse the current introduction controls before composition measurements.
        while (ui.Help.Onboarding.IsOpen) yield return Invoke(ui.RootElement.Q<Button>("onboarding-next"));
        for (int i = 0; i < 90 && !Application.isFocused; i++) yield return null;
        var sizes = (Environment.GetEnvironmentVariable("CCF_UI_SIZES") ?? "1280x720,1920x1080").Split(',')
            .Select(s => s.Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
        Debug.Log($"CHC_ENV focused={Application.isFocused} sizes={string.Join(",", sizes)} trees={FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Length} year={manager.CurrentEcologicalYear}");
        foreach (Vector2Int each in sizes)
        {
            size = each;
            SetGameSize(size.x, size.y);
            for (int i = 0; i < 40; i++) yield return null;
            Debug.Log($"CHC_SIZE {size.x}x{size.y} screen={Screen.width}x{Screen.height} uiWidth={ui.RootElement.layout.width:0} uiHeight={ui.RootElement.layout.height:0}");
            yield return Section("hud-stages", HudAtEveryStage());
            yield return Section("estimate", EstimateState());
            yield return Section("competitors", CompetitorState());
            yield return Section("focus", FocusTransitions());
            yield return Section("map-window", MapWindow());
            yield return Section("perf", PerformanceSample());
        }
        File.WriteAllLines(Path.Combine(outDir, "combined-hud-measure.csv"), measureRows);
        File.WriteAllLines(Path.Combine(outDir, "combined-hud-perf.csv"), perfRows);
        Debug.Log($"CHC_DONE pass={passes} fail={failures} shots={shotCount}");
        Finish(failures == 0 ? 0 : 1);
    }

    // ---- plumbing ----
    private IEnumerator Section(string name, IEnumerator body)
    {
        // CCF_COMBINED_SECTIONS=map-window,focus runs only those sections (a quick iteration aid while fixing one thing; a gate run leaves it unset and runs them all).
        string only = Environment.GetEnvironmentVariable("CCF_COMBINED_SECTIONS");
        if (!string.IsNullOrEmpty(only) && !only.Split(',').Contains(name)) yield break;
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
        Debug.Log((ok ? "CHC_CHECK_PASS " : "CHC_CHECK_FAIL ") + $"[{size.x}x{size.y}] {name} {detail}");
    }
    private void Fail(string name, string detail) => Check(false, name, detail);
    private void Measure(string state, string measure, float value)
    {
        measureRows.Add($"{state},{size.x}x{size.y},{measure},{value.ToString("0.####", Inv)}");
        Debug.Log($"CHC_MEASURE [{size.x}x{size.y}] {state} {measure}={value.ToString("0.####", Inv)}");
    }

    private static IEnumerator Pause(int frames = 10) { for (int i = 0; i < frames; i++) yield return null; }
    private static IEnumerator Wait(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }

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

    private IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();
        if (lastCapture != null) Destroy(lastCapture);
        lastCapture = ScreenCapture.CaptureScreenshotAsTexture();
    }

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
        Check(!LooksBlank(lastCapture), "SHOT_IS_NOT_BLANK_" + name, "a uniform capture means the Game view had no real surface");
        File.WriteAllBytes(Path.Combine(outDir, $"{name}-{size.x}x{size.y}.png"), lastCapture.EncodeToPNG());
        shotCount++;
        Debug.Log($"CHC_SHOT {name}-{size.x}x{size.y}.png {lastCapture.width}x{lastCapture.height}");
    }

    private void Finish(int code)
    {
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(code);
    }

    // The same stage injection the Lane A harness uses: the Objectives view reads teaching facts, so a stage can be shown without playing to it.
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

    private static string NameOf(VisualElement e) => string.IsNullOrEmpty(e.name) ? string.Join(".", e.GetClasses()) : e.name;

    // The HUD pieces that draw something and could collide: panels and text boxes, not the full-width layout containers they sit in.
    private List<(string name, VisualElement element)> HudPieces()
    {
        var pieces = new List<(string, VisualElement)>();
        void Add(string name, VisualElement e) { if (e != null && Visible(e)) pieces.Add((name, e)); }
        Add("objective-widget", ui.RootElement.Q("active-objective-widget"));
        Add("status-panel", ui.RootElement.Q(className: "hud-status"));
        Add("minimap", ui.Minimap.Root);
        Add("waypoint-panel", ui.RootElement.Q("hud-waypoint-panel"));
        VisualElement bottom = ui.RootElement.Q(className: "hud-bottom");
        if (bottom != null) foreach (VisualElement child in bottom.Children()) Add("bottom-" + NameOf(child), child);
        foreach (Label l in ui.RootElement.Query<Label>(className: "hud-prompt-box").ToList()) Add("prompt", l);
        VisualElement message = ui.RootElement.Q(className: "hud-message");
        if (message != null) foreach (Label l in message.Query<Label>().ToList()) Add("message", l);
        return pieces;
    }

    private void AuditHudClear(string state)
    {
        Rect root = ui.RootElement.worldBound;
        var pieces = HudPieces();
        foreach (var piece in pieces)
        {
            Rect r = piece.element.worldBound;
            Check(r.xMin >= root.xMin - 1f && r.xMax <= root.xMax + 1f && r.yMin >= root.yMin - 1f && r.yMax <= root.yMax + 1f, $"HUD_{state}_{piece.name}_IS_ON_SCREEN", r.ToString());
        }
        var overlaps = new List<string>();
        for (int i = 0; i < pieces.Count; i++)
            for (int j = i + 1; j < pieces.Count; j++)
                if (pieces[i].element.worldBound.Overlaps(pieces[j].element.worldBound))
                    overlaps.Add($"{pieces[i].name}{pieces[i].element.worldBound} x {pieces[j].name}{pieces[j].element.worldBound}");
        Check(overlaps.Count == 0, $"HUD_{state}_NO_PIECE_OVERLAPS_ANOTHER", $"pieces={pieces.Count} overlaps={string.Join(" ; ", overlaps)}");
    }

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
        Check(squashed.Count == 0, name, squashed.Count == 0 ? $"labels={labels}" : string.Join(" | ", squashed.Take(4)));
    }

    private float LowestLabel(VisualElement panel) => panel.Query<TextElement>().ToList().Where(Visible).Max(l => l.worldBound.yMax);

    // ---- 1: the walking HUD at every objective stage ----
    private IEnumerator HudAtEveryStage()
    {
        for (int stage = 0; stage < 8; stage++)
        {
            ui.CloseAll(); ui.CloseHelp();
            ForceStage(stage);
            // The HUD feed ticks four times a second of wall time.
            yield return Wait(0.6f);
            yield return Pause(6);
            string state = "stage" + (stage + 1);
            VisualElement widget = ui.RootElement.Q("active-objective-widget"), status = ui.RootElement.Q(className: "hud-status"), mini = ui.Minimap.Root, column = ui.RootElement.Q("hud-right-column");
            Check(widget != null && Visible(widget), $"HUD_{state}_OBJECTIVE_IS_VISIBLE");
            Check(status != null && Visible(status), $"HUD_{state}_STATUS_IS_VISIBLE");
            Check(Visible(mini), $"HUD_{state}_MINIMAP_IS_VISIBLE");
            if (widget == null || status == null || column == null) continue;
            Check(widget.Q<Label>("active-objective-now")?.text == "NOW" && !string.IsNullOrEmpty(widget.Q<Label>("active-objective-title")?.text), $"HUD_{state}_OBJECTIVE_SHOWS_NOW_AND_A_TITLE", widget.Q<Label>("active-objective-title")?.text);
            AuditHudClear(state);
            float gap = mini.worldBound.yMin - column.worldBound.yMax;
            Measure(state, "column_to_minimap_gap_ui_units", gap);
            Measure(state, "objective_widget_height_ui_units", widget.worldBound.height);
            Check(gap >= 8f, $"HUD_{state}_COLUMN_CLEARS_THE_MINIMAP", $"column bottom {column.worldBound.yMax:0} minimap top {mini.worldBound.yMin:0} gap {gap:0}");
            AuditNotSquashed($"HUD_{state}_COLUMN_ROWS_ARE_NOT_SQUASHED", column);
            Check(mini.Query<VisualElement>().ToList().Concat(new[] { mini }).All(e => e.pickingMode == PickingMode.Ignore), $"HUD_{state}_MINIMAP_CAPTURES_NO_INPUT");
            if (stage == 0 || stage == 3 || stage == 7) yield return Shot($"c01-hud-objective-and-minimap-{state}");
        }
        ForceStage(0);
        yield return Pause(10);
    }

    // ---- 2: an ordinary E-inspection shows the felling estimate in the right column; the minimap must stay clear of it ----
    private sealed class Spot { public ForestTree Tree; public Vector3 Camera; public Vector3 LookAt; public ForestTree Competitor1, Competitor2; }

    // Trees owned by a confirmed Rack V2 plan (removal set or marked entrance) refuse ordinary Crop/Fell marks by accepted design
    // (ForestTreeMarkingManager.RackEntrances). The focus section of an earlier size confirms that plan in this same session, so the
    // later size's competitor fixture must pick trees the plan does not own. Empty in legacy mode and at the first size.
    private HashSet<string> RackOwnedTreeIds()
    {
        var owned = new HashSet<string>();
        foreach (RackSaveData rack in manager.Racks)
        {
            if (rack == null) continue;
            if (rack.removalTreeIds != null) foreach (string id in rack.removalTreeIds) owned.Add(id);
            if (!string.IsNullOrEmpty(rack.entranceTreeId)) owned.Add(rack.entranceTreeId);
        }
        return owned;
    }

    private List<ForestTree> Choppable()
    {
        HashSet<string> owned = RackOwnedTreeIds();
        return FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop && !owned.Contains(t.TreeId)).ToList();
    }

    private Spot DenseSpot()
    {
        List<ForestTree> trees = Choppable();
        Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);
        Vector3 centre = trees.Aggregate(Vector3.zero, (s, t) => s + Flat(t.transform.position)) / trees.Count;
        int Neighbours(ForestTree t) => trees.Count(o => o != t && Vector3.Distance(Flat(o.transform.position), Flat(t.transform.position)) < 4f);
        ForestTree dense = trees.Where(t => Neighbours(t) >= 6).OrderBy(t => Vector3.Distance(Flat(t.transform.position), centre)).First();
        ForestTree n1 = trees.Where(t => t != dense).OrderBy(t => Vector3.Distance(Flat(t.transform.position), Flat(dense.transform.position))).First();
        ForestTree n2 = trees.Where(t => t != dense && t != n1).OrderBy(t => Vector3.Distance(Flat(t.transform.position), Flat(dense.transform.position))).First();
        return new Spot { Tree = dense, Camera = dense.transform.position + new Vector3(0f, 0f, -3.2f), LookAt = dense.transform.position + Vector3.up * 1.5f, Competitor1 = n1, Competitor2 = n2 };
    }

    private IEnumerator Inspect(Spot spot)
    {
        MethodInfo inspect = typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.NonPublic | BindingFlags.Instance);
        float ground = Terrain.activeTerrain != null ? Terrain.activeTerrain.SampleHeight(spot.Camera) + Terrain.activeTerrain.transform.position.y : spot.Camera.y;
        player.transform.position = new Vector3(spot.Camera.x, ground + 0.1f, spot.Camera.z);
        player.LookToward(spot.LookAt);
        for (int i = 0; i < 12; i++)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            player.LookToward(spot.LookAt);
            inspect.Invoke(player, new object[] { spot.Tree });
            yield return null;
        }
    }

    private void StopInspecting() => typeof(ForestPlayer).GetField("isInspecting", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, false);

    private IEnumerator EstimateState()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        Spot spot = DenseSpot();
        yield return Inspect(spot);
        yield return Wait(0.4f);
        Check(player.InspectedTree == spot.Tree, "ESTIMATE_TREE_IS_INSPECTED");
        VisualElement estimate = ui.RootElement.Q("inspect-estimate"), mini = ui.Minimap.Root, column = ui.RootElement.Q("hud-right-column"), card = ui.RootElement.Q(className: "inspect");
        Check(estimate != null && Visible(estimate), "ESTIMATE_PANEL_IS_SHOWN_FOR_AN_ORDINARY_TREE");
        Check(Visible(mini), "ESTIMATE_MINIMAP_STAYS_VISIBLE", "the minimap only steps aside for the competitor list");
        Check(Visible(ui.RootElement.Q("active-objective-widget")), "ESTIMATE_OBJECTIVE_STAYS_VISIBLE");
        if (estimate != null && Visible(estimate))
        {
            Rect e = estimate.worldBound, m = mini.worldBound, root = ui.RootElement.worldBound;
            Check(!e.Overlaps(m), "ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP", $"estimate={e} minimap={m}");
            Measure("estimate", "panel_bottom_to_minimap_top_gap_ui_units", m.yMin - e.yMax);
            Check(e.yMin >= column.worldBound.yMax - 1f, "ESTIMATE_PANEL_STARTS_UNDER_THE_HUD_COLUMN", $"estimateTop={e.yMin:0} columnBottom={column.worldBound.yMax:0}");
            Check(e.yMax <= root.yMax - 4f, "ESTIMATE_PANEL_IS_ON_SCREEN", e.ToString());
            Check(LowestLabel(estimate) <= e.yMax + 1f, "ESTIMATE_PANEL_IS_NOT_CLIPPED", $"lastLabelBottom={LowestLabel(estimate):0} panelBottom={e.yMax:0}");
            AuditNotSquashed("ESTIMATE_PANEL_ROWS_ARE_NOT_SQUASHED", estimate);
        }
        if (card != null) Check(!card.worldBound.Overlaps(column.worldBound), "ESTIMATE_INSPECTION_CARD_CLEARS_THE_RIGHT_COLUMN", $"card={card.worldBound}");
        AuditHudClear("estimate");
        yield return Shot("c02-inspect-estimate-with-minimap");
        StopInspecting();
        yield return Pause(10);
    }

    // ---- 3: the competitor list needs the right column's full height: the minimap steps aside for it and comes back (the R3 fix) ----
    private IEnumerator CompetitorState()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        Spot spot = DenseSpot();
        ui.Marking.Mark(spot.Competitor1, TreeMarkType.CropTree, false);
        ui.Marking.Mark(spot.Competitor2, TreeMarkType.Fell, false);
        ui.Marking.Mark(spot.Tree, TreeMarkType.CropTree, false);
        yield return Inspect(spot);
        yield return Wait(0.4f);
        Check(ui.Competitors.ListedCount >= 3, "COMPETITOR_LIST_HAS_SEVERAL_ROWS", $"listed={ui.Competitors.ListedCount}");
        VisualElement panel = ui.RootElement.Q("competitor-panel"), mini = ui.Minimap.Root, widget = ui.RootElement.Q("active-objective-widget"), column = ui.RootElement.Q("hud-right-column");
        Check(panel != null && Visible(panel), "COMPETITOR_PANEL_IS_SHOWN");
        Check(!Visible(mini), "COMPETITOR_MINIMAP_STEPS_ASIDE", "otherwise its 218 reserved units clip the list");
        Check(widget != null && Visible(widget), "COMPETITOR_OBJECTIVE_STAYS_VISIBLE");
        if (panel != null && Visible(panel))
        {
            Rect p = panel.worldBound, root = ui.RootElement.worldBound;
            Check(LowestLabel(panel) <= p.yMax + 1f, "COMPETITOR_LIST_IS_NOT_CLIPPED", $"lastLabelBottom={LowestLabel(panel):0} panelBottom={p.yMax:0}");
            Check(p.yMax <= root.yMax - 4f, "COMPETITOR_PANEL_IS_ON_SCREEN", p.ToString());
            Check(p.yMin >= column.worldBound.yMax - 1f, "COMPETITOR_PANEL_STARTS_UNDER_THE_OBJECTIVE", $"panelTop={p.yMin:0} columnBottom={column.worldBound.yMax:0}");
            Measure("competitors", "list_spare_height_ui_units", p.yMax - LowestLabel(panel));
            AuditNotSquashed("COMPETITOR_LIST_ROWS_ARE_NOT_SQUASHED", panel);
            AuditNotSquashed("COMPETITOR_HUD_COLUMN_ROWS_ARE_NOT_SQUASHED", column);
        }
        var tags = ui.RootElement.Query<Label>(className: "competitor-tag").ToList().Where(Visible).ToList();
        Check(tags.Count > 0, "COMPETITOR_TAGS_ARE_SHOWN");
        Check(tags.All(t => !t.worldBound.Overlaps(mini.worldBound) || !Visible(mini)), "COMPETITOR_TAGS_NEVER_SIT_OVER_THE_MINIMAP");
        yield return Shot("c03-competitor-list-minimap-aside");
        // The inspection ends: the minimap is back, the right column is the objective widget and the status panel again.
        StopInspecting();
        ui.Marking.Mark(spot.Tree, TreeMarkType.None, false);
        ui.Marking.Mark(spot.Competitor1, TreeMarkType.None, false);
        ui.Marking.Mark(spot.Competitor2, TreeMarkType.None, false);
        yield return Wait(0.4f);
        yield return Pause(8);
        Check(Visible(mini), "COMPETITOR_MINIMAP_RETURNS_WHEN_THE_INSPECTION_ENDS");
        Check(Visible(ui.RootElement.Q(className: "hud-status")), "COMPETITOR_STATUS_PANEL_RETURNS_TOO");
        AuditHudClear("after-competitors");
    }


    // ---- 5: the Stand Map window under the shared (Lane A) type scale ----
    private IEnumerator MapWindow()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        ui.ShowMap();
        yield return Settled();
        VisualElement modal = ui.Map.Root.Q(className: "modal"), card = ui.Map.Root.Q(className: "map-side"), root = ui.RootElement;
        Check(modal != null && card != null, "MAPWIN_WINDOW_AND_SIDE_CARD_EXIST");
        if (modal == null || card == null) yield break;
        Check(modal.worldBound.yMin >= 0f && modal.worldBound.yMax <= root.worldBound.yMax, "MAPWIN_WINDOW_IS_ON_SCREEN", modal.worldBound.ToString());
        string[] layers = { "Access & position", "Light", "Regeneration", "Browsing / protection", "Fell & crop marks" };
        for (int pass = 0; pass < 2; pass++)
        {
            // pass 1 repeats the audit with the side card's "Site details" foldout opened (its title is the longest line on the card, and its lines are the card's widest).
            for (int l = 0; l < layers.Length; l++)
            {
                Button tab = ui.Map.Root.Q("map-tabs").Query<Button>().ToList().FirstOrDefault(b => b.text == layers[l]);
                yield return Invoke(tab);
                yield return Pause(6);
                // Every layer press rebuilds the side card with a closed foldout, so the foldout is opened after the press (only the Access layer has one).
                Foldout details = ui.Map.Root.Q<Foldout>("map-site-details");
                if (l == 0) Check(details != null, "MAPWIN_ACCESS_LAYER_OFFERS_THE_SITE_DETAILS_FOLDOUT", "the foldout audit below would otherwise be skipped silently");
                if (pass == 1 && details != null) { details.value = true; yield return Pause(8); Check(details.value && details.contentContainer.childCount > 0, "MAPWIN_FOLDOUT_IS_OPEN_WITH_ITS_LINES", $"children={details.contentContainer.childCount}"); }
                string tag = $"MAPWIN_{(pass == 1 ? "FOLDOUT_" : "")}{layers[l].Replace(" ", "").Replace("/", "").Replace("&", "")}";
                var texts = modal.Query<TextElement>().ToList().Where(t => Visible(t) && !string.IsNullOrWhiteSpace(t.text)).ToList();
                float lowest = texts.Max(t => t.worldBound.yMax);
                Check(lowest <= modal.worldBound.yMax - 4f, tag + "_TEXT_ENDS_INSIDE_THE_WINDOW", $"lowestText={lowest:0.0} windowBottom={modal.worldBound.yMax:0.0}");
                var past = card.Query<TextElement>().ToList().Where(t => Visible(t) && !string.IsNullOrWhiteSpace(t.text) && !(t is Button) && t.worldBound.xMax > card.worldBound.xMax + 0.5f)
                    .Select(t => $"\"{(t.text.Length > 30 ? t.text.Substring(0, 30) : t.text)}\" right={t.worldBound.xMax:0.0} card={card.worldBound.xMax:0.0}").ToList();
                Check(past.Count == 0, tag + "_SIDE_CARD_TEXT_STAYS_INSIDE_THE_CARD", string.Join(" | ", past.Take(3)));
                var cut = new List<string>();
                foreach (TextElement t in modal.Query<TextElement>().ToList())
                {
                    if (!Visible(t) || string.IsNullOrWhiteSpace(t.text) || t is Button || t.resolvedStyle.whiteSpace != WhiteSpace.NoWrap) continue;
                    // The cell labels are sized by their squares and checked by the parity gate; this audit is for the words around the map.
                    if (t.ClassListContains("map-cell") || t.GetFirstAncestorOfType<Button>() != null) continue;
                    Vector2 natural = t.MeasureTextSize(t.text, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
                    if (natural.x > t.worldBound.width + 0.5f) cut.Add($"\"{t.text}\" text={natural.x:0} box={t.worldBound.width:0}");
                }
                Check(cut.Count == 0, tag + "_ONE_LINE_TEXT_IS_NOT_CUT_OFF", string.Join(" | ", cut.Take(3)));
                if (l == 0) yield return Shot(pass == 0 ? "c08-stand-map-window-access" : "c08-stand-map-window-site-details-open");
            }
        }
        ui.CloseAll();
        yield return Settled();
    }

    // ---- 4: cursor / focus across every transition ----
    private string FocusState() => $"lock={UnityEngine.Cursor.lockState} cursorVisible={UnityEngine.Cursor.visible} playerEnabled={player.enabled} panelOpen={manager.AnyPanelOpen} screen={ui.CurrentScreen} helpOpen={ui.Help.IsOpen} inspecting={player.IsInspecting} hudShown={Visible(ui.RootElement.Q(className: "hud-status"))}";

    private bool Walking() => UnityEngine.Cursor.lockState == CursorLockMode.Locked && !UnityEngine.Cursor.visible && player.enabled && !manager.AnyPanelOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !ui.Help.IsOpen;
    private bool PanelState() => UnityEngine.Cursor.lockState == CursorLockMode.None && UnityEngine.Cursor.visible && !player.enabled && manager.AnyPanelOpen;

    private Button FindPlanRack() => ui.RootElement.Query<Button>().ToList().FirstOrDefault(b => b.name == "map-plan-rack" && Visible(b));

    private IEnumerator Settled(int frames = 14)
    {
        // The forest takes its mouse back through OnEnable of the player a frame after the panel closes.
        for (int i = 0; i < frames; i++) { yield return null; }
        yield return Wait(0.2f);
    }

    private IEnumerator FocusTransitions()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        Spot spot = DenseSpot();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        yield return Settled();
        Check(Walking(), "FOCUS_00_START_IN_THE_FOREST", FocusState());

        yield return Inspect(spot);
        Check(player.InspectedTree == spot.Tree && UnityEngine.Cursor.lockState == CursorLockMode.Locked, "FOCUS_01_TREE_INSPECTION_KEEPS_THE_MOUSE_LOCKED", FocusState());
        ui.Inspection.ToggleDetail();
        yield return Pause(10);
        Check(ui.Inspection.ShowsDetail && Visible(ui.RootElement.Q(className: "inspect-detail")) && UnityEngine.Cursor.lockState == CursorLockMode.Locked, "FOCUS_02_MORE_DETAIL_OPENS_WITHOUT_FREEING_THE_MOUSE", FocusState());
        yield return Shot("c04-inspect-more-detail-with-hud");
        ui.Inspection.ToggleDetail();
        yield return Pause(6);

        ui.ShowObjectives();
        yield return Settled();
        Check(PanelState() && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Objectives, "FOCUS_03_OBJECTIVES_FREE_THE_MOUSE", FocusState());
        Check(!Visible(ui.RootElement.Q(className: "hud-status")) && !Visible(ui.Minimap.Root) && !Visible(ui.RootElement.Q("active-objective-widget")), "FOCUS_03_HUD_IS_NOT_DRAWN_UNDER_THE_OBJECTIVES_WINDOW", FocusState());
        ui.CloseAll();
        yield return Settled();
        Check(Walking(), "FOCUS_04_CLOSING_OBJECTIVES_GIVES_THE_MOUSE_BACK", FocusState());
        Check(Visible(ui.Minimap.Root) && Visible(ui.RootElement.Q("active-objective-widget")), "FOCUS_04_MINIMAP_AND_OBJECTIVE_ARE_BACK", FocusState());

        ui.ShowMap();
        yield return Settled();
        Check(PanelState() && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, "FOCUS_05_STAND_MAP_FREES_THE_MOUSE", FocusState());
        Check(!Visible(ui.Minimap.Root), "FOCUS_05_MINIMAP_IS_NOT_DRAWN_OVER_THE_FULL_MAP", FocusState());
        if (manager.RackPlanningV2Active)
        {
            Check(ui.Map.RackPlanningAvailable && FindPlanRack() != null, "FOCUS_05_V2_PLAN_RACKS_IS_IMMEDIATELY_AVAILABLE");
            Check(manager.RackGroups.Count == 0 && !RackTool.AnyActive && !ui.RackTool.Active, "FOCUS_05_V2_NEEDS_NO_FIRST_PERSON_RACK");
            if (!firstRackMade) Check(manager.PlannedRackCount == 0 && manager.WorkOrders.Count == 0,
                "FOCUS_05_V2_REPLACEMENT_IS_AVAILABLE_BEFORE_ANY_RACK_OR_WORK");
            ui.CloseAll(); yield return Settled();
            Check(Walking(), "FOCUS_05_V2_CLOSING_FRESH_MAP_RESTORES_FOREST", FocusState());
        }
        else
        {
            // Existing rack-first teaching design (identical on the base, Lane A and Lane B): the map offers "Plan rack" only once the first rack has been
            // made in the forest. A new game has not made one yet; the second screen size runs in the same game, where it has.
            bool gated = manager.RackFirstStageActive && !ui.Learning.IsDone("rack.create");
            if (!firstRackMade)
            {
                Check(gated, "FOCUS_05_A_NEW_GAME_IS_IN_THE_RACK_FIRST_STAGE", $"rackFirst={manager.RackFirstStageActive} rackCreateDone={ui.Learning.IsDone("rack.create")}");
                Check(FindPlanRack() == null, "FOCUS_05_PLAN_RACK_WAITS_FOR_THE_FIRST_FOREST_RACK (existing design, unchanged)");
            }
            ui.CloseAll();
            yield return Settled();
            if (!firstRackMade)
            {
                // The first rack, made through the real backend (the real B-key click flow with OS input is the guided gate's job).
                RackPlanner planner = ui.RackTool.Planner;
                planner.Refresh();
                RackLayoutLine first = ui.RackLayout.Lines.FirstOrDefault(l => !l.Done);
                Check(first != null, "FOCUS_06_THE_PLANNED_LAYOUT_HAS_A_RACK_TO_MAKE");
                if (first != null)
                {
                    RackProposal proposal = planner.Propose(new RackPoint(0f, first.AcrossM), planner.DefaultAngle(0f, true));
                    Check(proposal != null && proposal.CanConfirm && proposal.LayoutIndex == first.Index, "FOCUS_06_THE_FIRST_RACK_CAN_BE_CONFIRMED", proposal == null ? "none" : $"layout={proposal.LayoutIndex} can={proposal.CanConfirm}");
                    if (proposal != null && proposal.CanConfirm) { manager.ConfirmRack(proposal, "forest"); firstRackMade = true; }
                }
                yield return Wait(0.5f);
                Check(ui.Learning.IsDone("rack.create"), "FOCUS_06_THE_FIRST_RACK_IS_RECORDED_BY_THE_TEACHING");
            }
        }
        Check(Visible(ui.Minimap.Root), "FOCUS_06_MINIMAP_SHOWS_THE_NEW_RACK_AFTER_CONFIRMATION", FocusState());
        yield return Shot("c04b-hud-minimap-after-first-rack");
        ui.ShowMap();
        yield return Settled();
        Button plan = FindPlanRack();
        Check(plan != null, "FOCUS_06_PLAN_RACK_IS_OFFERED_ON_THE_MAP_AFTER_THE_FIRST_RACK");
        if (plan != null)
        {
            // Rack mode stays as the player left it when the map closes (only its own button changes it; same on the base). Start from OFF.
            if (ui.Map.RackMode) { yield return Invoke(plan); plan = FindPlanRack(); }
            Check(!ui.Map.RackMode, "FOCUS_06_RACK_MODE_STARTS_OFF");
            yield return Invoke(plan);
            Check(ui.Map.RackMode && PanelState(), "FOCUS_06_RACK_PLANNING_ON_THE_MAP_KEEPS_THE_POINTER_FREE", FocusState());
            if (manager.RackPlanningV2Active)
            {
                if (!firstRackMade)
                {
                    yield return Invoke(ui.RootElement.Q<Button>("map-edit-reset"));
                    Check(manager.PlannedRackCount == 5, "FOCUS_06_V2_AUTHORITATIVE_MAP_LAYOUT_CREATED");
                    yield return Invoke(ui.RootElement.Q<Button>("map-rack-plan-confirm"));
                    Check(manager.RackPlanConfirmed, "FOCUS_06_V2_AUTHORITATIVE_PLAN_CONFIRMED");
                    firstRackMade = manager.RackPlanConfirmed;
                }
                ui.Map.Refresh(true); ui.MapSource.MarkRacksDirty();
                StandMapModel model = ui.MapSource.Rebuild();
                Check(manager.RackPlanConfirmed && manager.PlannedRackCount == 5 && manager.WorkOrders.Any(o => o.rackId > 0),
                    "FOCUS_06_V2_CONFIRMED_RACKS_OWN_REAL_WORK");
                Check(model.Racks.Count(r => r.Symbol == MapSymbol.RackPlayer) == manager.PlannedRackCount
                    && model.MiniGuidance.Count == 0 && !ui.Minimap.TeachingNoteVisible,
                    "FOCUS_06_V2_CONFIRMED_SOLID_MAP_MINIMAP_CONTRACT");
            }
            else
            {
                int n = ui.Ecology.CellsPerAxis;
                var cells = ui.RootElement.Query<Button>(className: "map-cell").ToList();
                RackLayoutLine open = ui.RackLayout.Lines.FirstOrDefault(l => !l.Done);
                int cell = ui.Ecology.GetCellIndex(new Vector3(0f, 0f, open != null ? open.AcrossM : ui.Ecology.StandBounds.center.y));
                yield return Invoke(cells[(n - 1 - cell / n) * n + cell % n]);
                yield return Wait(0.3f);
                Check(ui.Map.RackPreview != null, "FOCUS_06_A_CLICKED_SQUARE_PREVIEWS_A_RACK", FocusState());
            }
            yield return Shot("c05-stand-map-rack-planning");
        }
        ui.CloseAll();
        yield return Settled();
        Check(Walking(), "FOCUS_07_CLOSING_THE_MAP_GIVES_THE_MOUSE_BACK", FocusState());
        Measure("focus", "rack_mode_still_on_after_the_map_closed (1 = yes, same as the base)", ui.Map.RackMode ? 1f : 0f);
        if (ui.Map.RackMode)
        {
            // Leave the game as found: switch it off again so nothing else starts in rack mode.
            ui.ShowMap(); yield return Settled();
            Button off = FindPlanRack();
            if (off != null) yield return Invoke(off);
            ui.CloseAll(); yield return Settled();
        }
        Check(Visible(ui.Minimap.Root), "FOCUS_07_MINIMAP_IS_BACK", FocusState());

        ui.ShowWorkPlan();
        yield return Settled();
        Check(PanelState(), "FOCUS_08_WORK_PLAN_FREES_THE_MOUSE", FocusState());
        yield return Shot("c06-work-plan-objective-1-disabled-sections");
        ui.CloseAll();
        yield return Settled();
        Check(Walking(), "FOCUS_09_CLOSING_THE_WORK_PLAN_GIVES_THE_MOUSE_BACK", FocusState());

        ui.ShowCurrentHelp();
        yield return Settled();
        Check(ui.Help.IsOpen && UnityEngine.Cursor.visible && UnityEngine.Cursor.lockState == CursorLockMode.None, "FOCUS_10_HELP_OPENS_WITH_A_FREE_VISIBLE_POINTER", FocusState());
        yield return Shot("c07-help-over-the-hud");
        ui.CloseHelp();
        yield return Settled();
        Check(Walking(), "FOCUS_11_CLOSING_HELP_GIVES_THE_MOUSE_BACK", FocusState());

        // Rapid burst: the window the player might alternate through in a second.
        for (int i = 0; i < 10; i++)
        {
            if (i % 3 == 0) ui.ShowMap(); else if (i % 3 == 1) ui.ShowObjectives(); else ui.ShowWorkPlan();
            yield return Pause(2);
            ui.CloseAll();
            yield return Pause(2);
        }
        yield return Settled();
        Check(Walking(), "FOCUS_12_A_RAPID_OPEN_CLOSE_BURST_LEAVES_THE_MOUSE_IN_THE_FOREST", FocusState());
        Vector3 before = player.transform.forward;
        player.LookToward(player.transform.position + Quaternion.Euler(0f, 40f, 0f) * before * 5f);
        yield return Pause(4);
        Check(Vector3.Angle(before, player.transform.forward) > 20f, "FOCUS_13_THE_PLAYER_CAN_STILL_TURN", $"turned {Vector3.Angle(before, player.transform.forward):0} deg");
        StopInspecting();
        yield return Pause(6);
    }

    // ---- 5: a bounded diagnostic sample. Editor + software rendering: relative comparison only, no threshold ----
    private IEnumerator PerformanceSample()
    {
        ui.CloseAll(); ui.CloseHelp(); ForceStage(0);
        StopInspecting();
        yield return Pause(20);
        Rect b = ui.Ecology.StandBounds;
        Vector3 start = new Vector3(b.center.x - 6f, player.transform.position.y, b.center.y - 6f);
        Action<int> walk = f =>
        {
            // A slow walk and turn: the minimap repaints whenever the player's cell-rounded position or heading changes.
            float t = f / 240f;
            player.transform.position = start + new Vector3(Mathf.Sin(t * 6.28f) * 5f, 0f, Mathf.Cos(t * 6.28f) * 5f);
            player.LookToward(player.transform.position + Quaternion.Euler(0f, t * 720f, 0f) * Vector3.forward * 4f);
        };
        int paintedStart = ui.Minimap.Canvas.PaintedVersion, versionStart = ui.MapSource.Model.Version;
        // A/B: the same walk with the minimap taking no part (the shared map source detached for the sample, restored straight after),
        // interleaved ON, OFF, ON, OFF, ON, OFF so one noisy window (this machine is shared) cannot decide the comparison.
        FieldInfo sourceField = typeof(ScenarioOneUiRoot).GetField("mapSource", BindingFlags.Instance | BindingFlags.NonPublic);
        object source = sourceField.GetValue(ui);
        for (int round = 1; round <= 3; round++)
        {
            yield return Sample("walking-minimap-ON-r" + round, 240, walk);
            if (round == 1)
            {
                Measure("perf", "model_versions_while_walking_240_frames", ui.MapSource.Model.Version - versionStart);
                Measure("perf", "minimap_painted_version_advance_240_frames", ui.Minimap.Canvas.PaintedVersion - paintedStart);
            }
            sourceField.SetValue(ui, null);
            yield return Pause(10);
            if (round == 1) Check(!Visible(ui.Minimap.Root), "PERF_AB_THE_MINIMAP_IS_OFF_FOR_THE_COMPARISON");
            yield return Sample("walking-minimap-OFF-r" + round, 240, walk);
            sourceField.SetValue(ui, source);
            yield return Pause(10);
            if (round == 1) Check(Visible(ui.Minimap.Root), "PERF_AB_THE_MINIMAP_COMES_BACK");
        }
        // The CPU cost of one draw-plan build for the minimap (the part of a repaint this code owns), measured directly. (Build(..) no longer takes the old "quiet" flag; the call is otherwise the same.)
        StandMapModel model = ui.MapSource.Model;
        float side = ui.Minimap.Canvas.LastSidePx > 1f ? ui.Minimap.Canvas.LastSidePx : 196f;
        var watch = System.Diagnostics.Stopwatch.StartNew();
        const int builds = 2000;
        for (int i = 0; i < builds; i++) StandMapDrawPlan.Build(model, side, MapDetail.Mini);
        watch.Stop();
        Measure("perf", "minimap_draw_plan_build_ms_per_call", (float)watch.Elapsed.TotalMilliseconds / builds);
        Measure("perf", "minimap_draw_plan_items", StandMapDrawPlan.Build(model, side, MapDetail.Mini).Count);
        yield return Sample("standing-still-minimap-ON", 240, f => { });
        ui.ShowMap();
        yield return Pause(20);
        yield return Sample("full-stand-map-open-HUD-hidden", 240, f => { });
        ui.CloseAll();
        yield return Pause(20);
    }

    private IEnumerator Sample(string mode, int frames, Action<int> step)
    {
        var times = new List<float>(frames);
        long gc0 = GC.CollectionCount(0);
        using (ProfilerRecorder alloc = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame"))
        {
            long allocated = 0;
            for (int f = 0; f < frames; f++)
            {
                step(f);
                yield return null;
                times.Add(Time.unscaledDeltaTime * 1000f);
                if (alloc.Valid) allocated += alloc.LastValue;
            }
            times.Sort();
            float mean = times.Average(), p50 = times[times.Count / 2], p95 = times[(int)(times.Count * 0.95f)], max = times[times.Count - 1];
            int collections = (int)(GC.CollectionCount(0) - gc0);
            float perFrame = alloc.Valid ? allocated / (float)frames : -1f;
            perfRows.Add($"{size.x}x{size.y},{mode},{frames},{mean.ToString("0.0", Inv)},{p50.ToString("0.0", Inv)},{p95.ToString("0.0", Inv)},{max.ToString("0.0", Inv)},{collections},{perFrame.ToString("0", Inv)}");
            Debug.Log($"CHC_PERF [{size.x}x{size.y}] {mode} frames={frames} mean={mean:0.0}ms p50={p50:0.0} p95={p95:0.0} max={max:0.0} gc0={collections} gcAllocPerFrame={(perFrame < 0 ? "n/a" : perFrame.ToString("0"))}B recorderValid={alloc.Valid}");
        }
        Check(times.Count == frames, "PERF_SAMPLE_" + mode + "_COMPLETED");
    }
}
#endif
