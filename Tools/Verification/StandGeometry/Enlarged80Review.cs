using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable 80B rendered review (D-056): the real enlarged new-game world through the real UI, at 1280x720 and
// 1920x1080, with captures into CCF_ACCEPTANCE_OUTPUT. Interactive (DISPLAY set). It walks the first cycle:
// inspect an interior and an edge tree -> mark spatially separated trees -> Stand Map -> waypoint -> Work Plan
// (residual stand / cash) -> approve -> advance -> Annual Review -> look at the altered forest, and measures the
// map layout. It never writes the player's save file and never saves the scene.
public static class Enlarged80Review
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    public static void SetGameSize(int width, int height)
    {
#if UNITY_EDITOR
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Review " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "Enlarged80Review.Begin"))
            new GameObject("Disposable enlarged 80 review").AddComponent<Enlarged80ReviewRunner>();
    }
}

public sealed class Enlarged80ReviewRunner : MonoBehaviour
{
    private ForestEcologyController e;
    private ScenarioOneManager m;
    private ScenarioOneUiRoot ui;
    private ForestPlayer player;
    private ForestTreeMarkingManager marking;
    private string output;
    private readonly JObject report = new JObject();

    private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);
    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        if (!string.IsNullOrEmpty(output)) { Directory.CreateDirectory(output); File.WriteAllText(Path.Combine(output, "review-report.json"), report.ToString()); }
        Debug.Log(failure == null ? "ENLARGED80_REVIEW_PASS" : "ENLARGED80_REVIEW_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        if (string.IsNullOrEmpty(output)) yield break;
        Directory.CreateDirectory(output);
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(output, name + ".jpg"), image.EncodeToJPG(88));
        Destroy(image);
    }

    private IEnumerator Settle(int frames = 12) { for (int i = 0; i < frames; i++) { ui.CloseHelp(); yield return null; } }

    private void Stand(Vector3 position, Vector3 lookAt)
    {
        player.transform.position = position;
        player.LookToward(lookAt);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
    }

    private IEnumerator Run()
    {
        e = FindFirstObjectByType<ForestEcologyController>(); m = FindFirstObjectByType<ScenarioOneManager>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); player = FindFirstObjectByType<ForestPlayer>(); marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(e.StandGeometryModelVersion == StandGeometryModel.Enlarged80 && e.CellCount == 256, "the review runs in the real Enlarged80 new-game world");
        report["world"] = new JObject { ["geometry"] = e.StandGeometryModelVersion, ["cells"] = e.CellCount, ["trees"] = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Count(t => t.IsLiving) };
        List<ForestTree> trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        Vector3 start = player.transform.position;
        yield return Settle(20);

        // ---- Views of the property: start, interior, and an edge ---------------------------------------------------
        Enlarged80Review.SetGameSize(1280, 720); yield return Settle(15);
        player.LookToward(start + Vector3.forward * 8f + Vector3.up * 1.3f); yield return Settle(); yield return Capture("view-start-north");
        Stand(new Vector3(-22.5f, start.y, -22.5f), new Vector3(-22.5f, 1.3f, -10f)); yield return Settle(); yield return Capture("view-interior-sw");
        Stand(new Vector3(33f, start.y, 4f), new Vector3(40f, 1.3f, 4f)); yield return Settle(); yield return Capture("view-edge-east-boundary");
        Stand(new Vector3(17f, start.y, 4f), new Vector3(23f, 1.3f, 4f)); yield return Settle(); yield return Capture("view-old-boundary-east");
        Stand(new Vector3(-38f, start.y, -38f), new Vector3(0f, 1.3f, 0f)); yield return Settle(); yield return Capture("view-from-sw-corner-diagonal");

        // ---- Stand Map at both resolutions -----------------------------------------------------------------------------
        foreach (Vector2Int size in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
        {
            Enlarged80Review.SetGameSize(size.x, size.y); yield return Settle(20);
            Stand(start, start + Vector3.forward * 8f);
            ui.ShowMap(); yield return Settle(20); ui.Map.Refresh(true); yield return Settle(10);
            Check(ui.Map.SetWaypointCell(e, 255), "waypoint at P16"); yield return Settle(10);
            yield return Capture("map-light-" + size.x + "-waypoint-P16");
            VisualElement modal = ui.Map.Root.Q(className: "modal");
            Rect grid = ui.Map.Root.Q(className: "map-grid").worldBound, side = ui.Map.Root.Q(className: "map-side").worldBound, root = ui.RootElement.worldBound;
            var cells = ui.Map.Root.Query<Button>(className: "map-cell").ToList();
            bool allInside = cells.All(c => root.Contains(c.worldBound.min) && root.Contains(c.worldBound.max));
            float smallest = cells.Min(c => Mathf.Min(c.worldBound.width, c.worldBound.height));
            var labelTexts = cells.SelectMany(c => c.Query<Label>().ToList()).Where(l => l.resolvedStyle.display == DisplayStyle.Flex).ToList();
            bool textFits = labelTexts.All(l => l.worldBound.width <= l.parent.worldBound.width + 1f && l.worldBound.height <= l.parent.worldBound.height + 1f);
            report["map" + size.x] = new JObject
            {
                ["cells"] = cells.Count, ["allCellsInsideScreen"] = allInside, ["smallestCellPx"] = F(smallest), ["gridRect"] = grid.ToString(), ["sidePanelRect"] = side.ToString(),
                ["gridAndSideOverlap"] = grid.Overlaps(side), ["modalRect"] = modal?.worldBound.ToString(), ["cellLabelTextOverflows"] = !textFits, ["root"] = root.ToString()
            };
            Check(cells.Count == 256 && allInside && !grid.Overlaps(side), "all 256 cells are on screen and the grid does not overlap the side panel at " + size.x);
            // marks layer after marking spatially separated trees
            if (size.x == 1280)
            {
                var ne = trees.Where(t => Mathf.Abs(t.transform.position.x - 24f) < 7f && Mathf.Abs(t.transform.position.z - 24f) < 7f).Take(20).ToList();
                var sw = trees.Where(t => Mathf.Abs(t.transform.position.x + 24f) < 7f && Mathf.Abs(t.transform.position.z + 24f) < 7f).Take(20).ToList();
                for (int i = 0; i < ne.Count; i += 2) marking.Mark(ne[i], TreeMarkType.Fell, false);
                for (int i = 0; i < sw.Count; i += 2) marking.Mark(sw[i], TreeMarkType.Fell, false);
                FieldInfo layerField = typeof(StandMapView).GetField("layer", BindingFlags.Instance | BindingFlags.NonPublic);
                layerField.SetValue(ui.Map, Enum.Parse(layerField.FieldType, "Marks")); ui.Map.Refresh(true); yield return Settle(10);
                yield return Capture("map-marks-1280-two-areas");
                layerField.SetValue(ui.Map, Enum.Parse(layerField.FieldType, "Light")); ui.Map.Refresh(true);
            }
            ui.CloseAll(); yield return Settle(5);
        }

        // ---- First cycle through the real UI ---------------------------------------------------------------------------
        Enlarged80Review.SetGameSize(1280, 720); yield return Settle(20);
        MethodInfo inspect = typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.NonPublic | BindingFlags.Instance);
        ForestTree interior = trees.OrderBy(t => Vector2.Distance(new Vector2(t.transform.position.x, t.transform.position.z), new Vector2(-24f, -24f))).First();
        ForestTree edge = trees.OrderBy(t => Vector2.Distance(new Vector2(t.transform.position.x, t.transform.position.z), new Vector2(38f, 4f))).First();
        foreach (var (name, tree) in new[] { ("interior", interior), ("edge", edge) })
        {
            Vector3 p = tree.transform.position;
            Stand(new Vector3(p.x - 2.2f, start.y, p.z), p + Vector3.up * 1.3f); yield return Settle(20);
            for (int i = 0; i < 8; i++) { UnityEngine.Cursor.lockState = CursorLockMode.Locked; inspect.Invoke(player, new object[] { tree }); yield return null; }
            yield return Settle(6); ui.CloseHelp(); yield return Settle(6);
            yield return Capture("inspect-" + name);
            report["inspect_" + name] = new JObject { ["tree"] = tree.TreeId, ["edgeDistanceM"] = F(Mathf.Min(Mathf.Min(p.x + 40f, 40f - p.x), Mathf.Min(p.z + 40f, 40f - p.z))), ["competitionIndex"] = F(e.GetCompetitionIndex(tree)) };
            typeof(ForestPlayer).GetField("isInspecting", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, false);
        }
        yield return null;
        // plan, approve, advance
        int imported = m.AddMarkedTreesToWorkPlan();
        Check(imported > 0, "marks became work orders");
        ui.ShowWorkPlan(); yield return Settle(20); yield return Capture("workplan-1280");
        long cashBefore = m.CashCents;
        Check(m.ApprovePendingWork(), "approved"); ui.CloseAll(); yield return Settle(5);
        double t0 = Time.realtimeSinceStartupAsDouble;
        Check(m.AdvanceYear(), "advanced"); report["advanceYearSeconds"] = F(Time.realtimeSinceStartupAsDouble - t0);
        yield return Settle(10);
        ui.ShowReview(); yield return Settle(20); yield return Capture("annual-review-1280");
        report["firstCycle"] = new JObject { ["marksImported"] = imported, ["cashBeforeCents"] = cashBefore, ["cashAfterCents"] = m.CashCents, ["livingTreesAfter"] = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Count(t => t.IsLiving) };
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); yield return Settle(10);
        Stand(new Vector3(-24f, start.y, -34f), new Vector3(-24f, 1.3f, -24f)); yield return Settle(20); yield return Capture("after-opening-sw");
        Stand(new Vector3(24f, start.y, 14f), new Vector3(24f, 1.3f, 24f)); yield return Settle(20); yield return Capture("after-opening-ne");
        ui.ShowMap(); yield return Settle(20); ui.Map.Refresh(true); yield return Settle(10); yield return Capture("map-after-intervention-1280");
        ui.CloseAll(); yield return null;
        // walking distances for the navigation estimate
        report["walkingEstimate"] = new JObject
        {
            ["startToNorthEastAreaMeters"] = F(Vector2.Distance(new Vector2(start.x, start.z), new Vector2(24f, 24f))),
            ["northEastToSouthWestAreaMeters"] = F(Vector2.Distance(new Vector2(24f, 24f), new Vector2(-24f, -24f))),
            ["secondsWalking4mps_NE_to_SW"] = F(Vector2.Distance(new Vector2(24f, 24f), new Vector2(-24f, -24f)) / 4f), ["secondsRunning7mps_NE_to_SW"] = F(Vector2.Distance(new Vector2(24f, 24f), new Vector2(-24f, -24f)) / 7f)
        };
    }
}
