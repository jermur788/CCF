#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CCF.Forestry.WorkEconomy;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// Disposable MATCHED screenshots of the Scenario One Stand Map and HUD minimap (SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1).
//
// The same script is run against the OLD map (the frozen Lane B presentation, base d330317) and the NEW one, so a before/after pair
// shows the SAME forest at the SAME moment. It plays one deterministic management story in memory (nothing is saved):
//   s1-start        the fresh Scenario One plantation, nothing planned
//   s2-racks-marks  three layout racks confirmed, one off-layout rack, crop and felling marks, a waypoint, the player moved
//   s3-year4        the racks harvested, a selective felling, planted juveniles, four ecological years of regeneration and growth
// and at each stage, at each requested size, captures the HUD with its minimap, the full map on every layer, and (where the rack
// planner is offered) the full map with a rack being planned.
//
// It finds the map's parts only through names and classes that both presentations keep ("map-tabs", "map-frame", "map-cell",
// "map-plan-rack", "map-rack-cancel", "hud-minimap"), and the layer tabs by position, so a renamed tab does not break it.
// The simulation is untouched by either map presentation, so the world hash of each stage must be IDENTICAL before and after:
// that is written to shots.json and is the proof the pairs show the same forest.
//
// Run (graphical Editor, private display):
//   -executeMethod MapRedesignShots.Begin    CCF_DIAG_DIR=<folder outside the repo>  CCF_UI_SIZES=1920x1080,1280x720
// Reads and plays the game in memory, writes nothing but its evidence folder. Remove it from Assets afterwards.
public static class MapRedesignShots
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        MapRedesignShotsRunner.SetGameSize(MapRedesignShotsRunner.Sizes()[0].x, MapRedesignShotsRunner.Sizes()[0].y);
        EditorApplication.isPlaying = true;
    }

    // First-use help windows are another harness's business; mark them seen so the HUD and the map are visible directly.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "MapRedesignShots.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MapRedesignShots.Begin"))
            new GameObject("Map redesign shots").AddComponent<MapRedesignShotsRunner>();
    }
}

public sealed class MapRedesignShotsRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController eco;
    private ForestPlayer player;
    private ForestSaveController saves;
    private RackPlanner planner;
    private string output;
    private int shots;
    private readonly List<string> manifest = new List<string>(), notes = new List<string>(), stateLines = new List<string>();

    private static string F(double v, string f = "0.####") => v.ToString(f, Inv);
    private void Note(string text) { notes.Add(text); Debug.Log("MAPSHOTS_NOTE " + text); }

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "MapShots " + width + "x" + height }, null);
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
        if (string.IsNullOrEmpty(text)) text = "1920x1080,1280x720";
        return text.Split(',').Select(s => s.Trim().Split('x')).Select(p => new Vector2Int(int.Parse(p[0]), int.Parse(p[1]))).ToArray();
    }

    // ---------- run loop ----------
    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "map-redesign-shots");
        Directory.CreateDirectory(output);
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Play());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        File.WriteAllText(Path.Combine(output, "shots.json"), "{\n \"shots\": [\n" + string.Join(",\n", manifest) + "\n ],\n \"states\": [\n" + string.Join(",\n", stateLines) + "\n ]\n}\n");
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        Debug.Log(failure == null ? $"MAPSHOTS_DONE shots={shots} notes={notes.Count}" : $"MAPSHOTS_FAIL shots={shots} exception={failure}");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
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

    private IEnumerator Invoke(Button button)
    {
        if (button == null) throw new InvalidOperationException("button not found");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }

    // The panel is laid out in UI units (1600 x 900 reference); a screenshot is in physical pixels.
    private float PixelsPerUnit => ui.RootElement.worldBound.width > 0f ? Screen.width / ui.RootElement.worldBound.width : 1f;
    private string RectPx(VisualElement e)
    {
        if (e == null || !Visible(e)) return "null";
        float s = PixelsPerUnit; Rect r = e.worldBound;
        return $"[{F(r.xMin * s, "0.0")},{F(r.yMin * s, "0.0")},{F(r.width * s, "0.0")},{F(r.height * s, "0.0")}]";
    }

    private IEnumerator Capture(string state, string shot, string layer)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + shot);
        string file = $"{state}-{shot}-{Screen.width}x{Screen.height}.png";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        Destroy(image);
        shots++;
        VisualElement frame = ui.Map != null ? ui.Map.Root.Q("map-frame") : null;
        manifest.Add($"  {{\"file\":\"{file}\",\"state\":\"{state}\",\"shot\":\"{shot}\",\"layer\":\"{layer}\",\"width\":{Screen.width},\"height\":{Screen.height},"
            + $"\"mapFramePx\":{RectPx(frame)},\"minimapPx\":{RectPx(ui.Minimap != null ? ui.Minimap.Root : null)}}}");
    }

    // ---------- the management story ----------
    private IEnumerator Play()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        eco = FindFirstObjectByType<ForestEcologyController>();
        player = FindFirstObjectByType<ForestPlayer>();
        saves = FindFirstObjectByType<ForestSaveController>();
        if (ui == null || manager == null || eco == null || player == null) throw new InvalidOperationException("the Scenario One scene did not start");
        planner = ui.RackTool.Planner;
        Debug.Log($"MAPSHOTS_START geometry={StandGeometryModel.Label(eco.StandGeometryModelVersion)} cells={eco.CellCount} trees={eco.LivingTreeCount}");

        yield return Settle(10);
        yield return ShootStage("s1-start", false);

        // S2: racks, off-layout rack, marks, waypoint, player placed in the forest.
        yield return BuildRacksAndMarks();
        yield return ShootStage("s2-racks-marks", true);

        // S3: harvest and four years of growth.
        yield return ManageYears();
        yield return ShootStage("s3-year4", true);
    }

    private IEnumerator BuildRacksAndMarks()
    {
        ui.CloseAll(); yield return Settle(6);
        planner.Refresh();
        int made = 0;
        foreach (RackLayoutLine line in ui.RackLayout.Lines.Where(l => !l.Done).Take(3).ToList())
        {
            RackProposal p = planner.Propose(new RackPoint(0f, line.AcrossM), planner.DefaultAngle(0f, true));
            if (p != null && p.CanConfirm && manager.ConfirmRack(p, "forest") > 0) made++;
            yield return Settle(3);
        }
        RackProposal other = planner.Propose(new RackPoint(eco.StandBounds.center.x + 6f, eco.StandBounds.center.y), 90f);
        if (other != null && other.CanConfirm && other.LayoutIndex < 0) manager.ConfirmRack(other, "forest");
        Note($"racks confirmed on the layout: {made}; off-layout rack: {(other != null && other.CanConfirm && other.LayoutIndex < 0)}");

        // Crop trees (the largest, well apart) each with the two biggest neighbours within 4.5 m marked to fell.
        List<ForestTree> trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop).OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        var crops = new List<ForestTree>();
        foreach (ForestTree t in trees)
        {
            if (crops.Count >= 6) break;
            Vector2 at = new Vector2(t.transform.position.x, t.transform.position.z);
            if (crops.All(c => Vector2.Distance(at, new Vector2(c.transform.position.x, c.transform.position.z)) > 11f)) crops.Add(t);
        }
        int fells = 0;
        foreach (ForestTree crop in crops)
        {
            ui.Marking.Mark(crop, TreeMarkType.CropTree, false);
            Vector2 c = new Vector2(crop.transform.position.x, crop.transform.position.z);
            foreach (ForestTree n in trees.Where(t => t != crop && !crops.Contains(t) && Vector2.Distance(c, new Vector2(t.transform.position.x, t.transform.position.z)) <= 4.5f).Take(2))
            { ui.Marking.Mark(n, TreeMarkType.Fell, false); fells++; }
        }
        Note($"crop trees {crops.Count}, felling marks {fells}");

        // A destination and a player position deep in the stand, facing north-east.
        Rect b = eco.StandBounds;
        Vector3 at3 = new Vector3(b.center.x - 9.3f, player.transform.position.y, b.center.y + 4.1f);
        player.transform.position = at3;
        player.LookToward(new Vector3(at3.x + 20f, at3.y, at3.z + 14f));
        ui.ShowMap(); yield return Settle(6);
        int cell = eco.GetCellIndex(new Vector3(b.center.x + 14f, 0f, b.center.y - 13f));
        ui.Map.SetWaypointCell(eco, cell);
        ui.CloseAll(); yield return Settle(10);
    }

    private IEnumerator ManageYears()
    {
        ui.CloseAll(); yield return Settle(4);
        Note($"marks to the Work Plan: {manager.AddMarkedTreesToWorkPlan()}; approve: {manager.ApprovePendingWork()}");
        yield return AdvanceAndAcknowledge("year 1 harvest");

        // The two protection decisions the sequential teaching shows, through the same public planting path.
        foreach (string item in new[] { "beech-sapling", "sessile-oak-sapling" })
        {
            bool bought = manager.TryPurchaseStock(item, 4);
            int planned = 0;
            foreach (int cell in Enumerable.Range(0, eco.CellCount).OrderByDescending(i => eco.Cells[i].Light).ThenBy(i => i))
            {
                Vector2 centre = eco.Cells[cell].Center;
                for (int z = -2; z <= 2 && planned < 4; z++)
                for (int x = -2; x <= 2 && planned < 4; x++)
                    if (manager.TryDesignateExactPlanting(item, new Vector3(centre.x + x * .9f, 0f, centre.y + z * .9f), WorkExecutionMethod.LandownerSimulated, planned % 2 == 0)) planned++;
                if (planned == 4) break;
            }
            Note($"planting {item}: bought={bought} designated={planned}");
        }
        Note("approve planting: " + manager.ApprovePendingWork());
        yield return AdvanceAndAcknowledge("year 2 planting");
        yield return AdvanceAndAcknowledge("year 3");
        yield return AdvanceAndAcknowledge("year 4");
        ui.CloseAll(); yield return Settle(10);
    }

    private IEnumerator AdvanceAndAcknowledge(string label)
    {
        ui.CloseAll(); yield return Settle(4);
        bool advanced = manager.AdvanceYear();
        Note($"advance ({label}): {advanced} -> year {eco.EcologicalYear}");
        yield return Settle(10);
        ui.ShowReview(); yield return Settle(20);
        Button ack = ui.RootElement.Q<Button>("annual-review-acknowledge");
        if (ack != null && Visible(ack) && ack.enabledInHierarchy) yield return Invoke(ack);
        ui.CloseAll(); yield return Settle(6);
    }

    // ---------- shots at one stage ----------
    private IEnumerator ShootStage(string state, bool planning)
    {
        ForestSaveData data = saves != null ? saves.CaptureData() : null;
        string hash = data != null ? ScenarioReferenceArchive.WorldHash(data) : "n/a";
        int living = eco.LivingTreeCount, marks = ui.Marking != null ? ui.Marking.LivingMarkedCount : -1;
        stateLines.Add($"  {{\"state\":\"{state}\",\"year\":{eco.EcologicalYear},\"livingTrees\":{living},\"marked\":{marks},\"racks\":{ui.MapSource.Model.Racks.Count},\"plantedJuveniles\":{manager.PlantedJuveniles.Count},\"worldHash\":\"{hash}\"}}");
        Debug.Log($"MAPSHOTS_STATE {state} year={eco.EcologicalYear} living={living} marked={marks} hash={hash}");
        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y);
            ui.CloseAll(); yield return Settle(20);
            yield return Capture(state, "hud-minimap", "walking");
            ui.ShowMap(); yield return Settle(20);
            string[] names = { "map-access", "map-light", "map-regeneration", "map-browsing", "map-marks" };
            VisualElement tabs = ui.Map.Root.Q("map-tabs");
            for (int l = 0; l < names.Length; l++)
            {
                Button tab = tabs != null ? tabs.Query<Button>().ToList().ElementAtOrDefault(l) : null;
                yield return Invoke(tab);
                yield return Settle(10);
                yield return Capture(state, names[l], names[l].Substring(4));
            }
            // Back to the first layer for the planning shot.
            yield return Invoke(tabs.Query<Button>().ToList()[0]);
            yield return Settle(6);
            if (planning) yield return PlanningShot(state);
            ui.CloseAll(); yield return Settle(6);
        }
    }

    private IEnumerator PlanningShot(string state)
    {
        Button plan = ui.RootElement.Q<Button>("map-plan-rack");
        if (plan == null || !Visible(plan)) { Note($"{state}: the 'Plan rack' button was not offered, so no planning shot"); yield break; }
        yield return Invoke(plan);
        int n = eco.CellsPerAxis;
        var cells = ui.RootElement.Query<Button>(className: "map-cell").ToList();
        RackLayoutLine open = ui.RackLayout.Lines.FirstOrDefault(l => !l.Done);
        Vector3 where = open != null ? new Vector3(0f, 0f, open.AcrossM) : new Vector3(eco.StandBounds.center.x, 0f, eco.StandBounds.center.y);
        int cell = eco.GetCellIndex(where);
        // The cells are laid out north row first, so a world cell index maps to this list index.
        yield return Invoke(cells[(n - 1 - cell / n) * n + cell % n]);
        yield return Wait(0.3f);
        yield return Settle(8);
        yield return Capture(state, "map-rack-planning", "rack-planning");
        Button cancel = ui.RootElement.Q<Button>("map-rack-cancel");
        if (cancel != null) yield return Invoke(cancel);
        yield return Settle(4);
    }
}
#endif
