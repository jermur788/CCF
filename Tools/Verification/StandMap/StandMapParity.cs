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

// Disposable RENDERED + FUNCTIONAL qualification for B5/B8: the HUD minimap and the full Stand Map are two presentations of ONE map.
//
// It plays a fresh Scenario One game in a graphical Editor and checks, against the REAL canvases and the REAL cell buttons (not
// against the draw-plan code in isolation):
//   * identical world locations give identical relative positions in the minimap canvas and the full-map canvas, and both equal an
//     expectation computed here, independently, from the property bounds;
//   * the full map's cell buttons are laid out where the shared transform says (all 256);
//   * player facing is drawn the same way on both;
//   * a rack confirmed through the real backend appears on the minimap, and then on the full map, from the same data;
//   * the destination marker is the same place on both;
//   * the minimap carries no controls and no values, and the rack-planning preview exists on the full map only;
//   * layout at several screen sizes (no overlap with the HUD, the map fits), with screenshots.
//
// Targets the Enlarged80 new-game world (16 x 16 squares, the authored five-rack layout, 67 felling orders). Under the Editor-only Legacy40
// override (CCF_STAND_GEOMETRY=0) the geometry, position, facing and cell-layout checks still pass (95 of them were run), but the checks
// that assume that authored layout (rack counts, minimum item/point counts, felling orders after a load) do not apply.
// Run (graphical Editor):  -executeMethod StandMapParity.Begin   CCF_DIAG_DIR=<folder>  CCF_UI_SIZES=1920x1080,1280x720
// Reads the game, plays it in memory, writes nothing but its evidence folder. Remove it from Assets afterwards.
public static class StandMapParity
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        StandMapParityRunner.SetGameSize(StandMapParityRunner.Sizes()[0].x, StandMapParityRunner.Sizes()[0].y);
        EditorApplication.isPlaying = true;
    }

    // First-use screens are another harness's business; mark them seen so this one sees the HUD and the map directly.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "StandMapParity.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "StandMapParity.Begin"))
            new GameObject("Stand map parity").AddComponent<StandMapParityRunner>();
    }
}

public sealed class StandMapParityRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController eco;
    private ForestPlayer player;
    private RackPlanner planner;
    private string output;
    private int shots, passed;
    private readonly List<string> failures = new List<string>(), evidence = new List<string>(), unverified = new List<string>();

    private static string F(double v, string f = "0.####") => v.ToString(f, Inv);
    private void Hard(string name, bool ok, string detail = "")
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"STANDMAP_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Note(string name, string detail) { evidence.Add(name + " " + detail); Debug.Log($"STANDMAP_MEASURE {name} {detail}"); }
    private void Unverified(string detail) { unverified.Add(detail); Debug.Log("STANDMAP_UNVERIFIED " + detail); }

    // ---------- Editor window helpers ----------
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "StandMap " + width + "x" + height }, null);
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
        if (output != null) File.WriteAllLines(Path.Combine(output, "standmap-evidence.txt"), evidence.Concat(unverified.Select(u => "UNVERIFIED " + u)));
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"STANDMAP_PARITY_PASS shots={shots} hardChecks={passed} unverified={unverified.Count}"
            : $"STANDMAP_PARITY_FAIL shots={shots} hardChecks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
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

    private IEnumerator Capture(string state)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + state);
        File.WriteAllBytes(Path.Combine(output, state + "-" + Screen.width + "x" + Screen.height + ".png"), image.EncodeToPNG());
        Destroy(image);
        shots++;
    }

    // ---------- reading what the canvases actually painted ----------
    private StandMapCanvas Mini => ui.Minimap.Canvas;
    private StandMapCanvas Full => ui.Map.Canvas;

    private static MapDrawItem First(StandMapCanvas canvas, MapSymbol symbol) => canvas.LastPlan.FirstOrDefault(i => i.Symbol == symbol);
    private static int Count(StandMapCanvas canvas, MapSymbol symbol) => canvas.LastPlan.Count(i => i.Symbol == symbol);

    // Where an item's anchor sits as a fraction of the side the canvas painted at.
    private static Vector2 Fraction(StandMapCanvas canvas, MapDrawItem item) => new Vector2(item.Anchor.X / canvas.LastSidePx, item.Anchor.Y / canvas.LastSidePx);

    // The expectation, computed here from the property bounds and NOT through StandMapTransform: x from the west edge, y from the north edge.
    private Vector2 ExpectedFraction(Vector3 world)
    {
        Rect b = eco.StandBounds;
        return new Vector2((world.x - b.xMin) / b.width, 1f - (world.z - b.yMin) / b.height);
    }

    private static float Distance(Vector2 a, Vector2 b) => (a - b).magnitude;

    private static float HeadingFromArrow(StandMapCanvas canvas, MapDrawItem player)
    {
        MapPosition tip = player.Points[0];
        return StandMapTransform.HeadingOf(tip.X - player.Anchor.X, -(tip.Y - player.Anchor.Y));
    }

    private static float AngleDifference(float a, float b)
    {
        float d = Mathf.Abs(Mathf.Repeat(a - b + 180f, 360f) - 180f);
        return d;
    }

    // ---------- storyline ----------
    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; eco = ui.Ecology; player = ui.Player; planner = ui.RackTool.Planner;
        if (Application.isBatchMode) throw new InvalidOperationException("StandMapParity needs a graphical Editor");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/StandMap/evidence" : diag, "standmap");
        Directory.CreateDirectory(output);
        yield return Wait(0.5f);
        if (ui.Help.IsOpen) { ui.CloseHelp(); yield return Settle(); }
        ui.CloseAll(); yield return Settle();
        Note("START", $"year={eco.EcologicalYear} geometry={eco.StandGeometryModelVersion} bounds={eco.StandBounds} cells={eco.CellsPerAxis} screen={Screen.width}x{Screen.height}");

        yield return Model();
        yield return MinimapIsReduced();
        yield return PositionAndFacing();
        yield return WholePlanParity();
        yield return CellLayout();
        yield return RackConfirmation();
        yield return SaveAndLoad();
        yield return Waypoint();
        yield return PlanningPreviewIsFullMapOnly();
        yield return Layouts();
    }

    // ---- the data is one model, with the content the brief names ----
    private IEnumerator Model()
    {
        StandMapModel model = ui.MapSource.Model;
        Rect b = eco.StandBounds;
        Hard("MODEL_HAS_THE_PROPERTY", model.HasProperty && Mathf.Approximately(model.SizeM, b.width) && Mathf.Approximately(model.MinX, b.xMin) && Mathf.Approximately(model.MinZ, b.yMin), "");
        Hard("MODEL_PROPERTY_IS_SQUARE_AND_THE_GRID_TILES_IT", Mathf.Approximately(b.width, b.height) && Mathf.Abs(eco.CellsPerAxis * eco.CellSizeMeters - b.width) < 0.01f, $"{b.width}x{b.height} {eco.CellsPerAxis}x{eco.CellSizeMeters}");
        Hard("MODEL_HAS_A_PATH", model.Paths.Count > 0 && model.Paths.Sum(p => p.Points.Count) >= 10, $"paths={model.Paths.Count} points={model.Paths.Sum(p => p.Points.Count)}");
        Hard("MODEL_HAS_THE_CLEARING", model.Landmarks.Count == 1, $"landmarks={model.Landmarks.Count}");
        Hard("MODEL_HAS_EVERY_LAYOUT_RACK", ui.RackLayout.Lines.Count > 0 && model.Racks.Count(r => r.LayoutIndex >= 0) == ui.RackLayout.Lines.Count, $"lines={ui.RackLayout.Lines.Count} racks={model.Racks.Count}");
        Hard("MODEL_HAS_THE_PLAYER_INSIDE_THE_PROPERTY", model.HasPlayer && model.Player.X >= b.xMin && model.Player.X <= b.xMax && model.Player.Z >= b.yMin && model.Player.Z <= b.yMax, $"{model.Player.X},{model.Player.Z}");
        Note("MODEL", $"paths={model.Paths.Count} pathPoints={model.Paths.Sum(p => p.Points.Count)} pathWidthM={F(model.Paths[0].WidthM)} racks={model.Racks.Count} clearing=({F(model.Landmarks[0].At.X)},{F(model.Landmarks[0].At.Z)}) r={F(model.Landmarks[0].RadiusM)}");
        yield break;
    }

    // ---- the minimap: present, painted, non-interactive, no values ----
    private IEnumerator MinimapIsReduced()
    {
        HudMinimapView mini = ui.Minimap;
        Hard("MINIMAP_EXISTS_AND_IS_SHOWN_ON_THE_WALKING_HUD", mini != null && Visible(mini.Root), "");
        Hard("MINIMAP_IS_PAINTED_FROM_THE_SHARED_MODEL", Mini.LastPlan.Count > 0 && Mini.PaintedVersion == ui.MapSource.Model.Version, $"items={Mini.LastPlan.Count} version={Mini.PaintedVersion}/{ui.MapSource.Model.Version}");
        Hard("MINIMAP_IS_SQUARE", Mathf.Abs(Mini.worldBound.width - Mini.worldBound.height) < 1f, Mini.worldBound.ToString());
        Hard("MINIMAP_TAKES_NO_POINTER_INPUT", mini.Root.Query<VisualElement>().ToList().Concat(new[] { mini.Root }).All(e => e.pickingMode == PickingMode.Ignore) && mini.Root.Query<Button>().ToList().Count == 0, "");
        var texts = mini.Root.Query<TextElement>().ToList().Where(t => !string.IsNullOrEmpty(t.text)).Select(t => t.text.Trim()).ToList();
        Hard("MINIMAP_TEXT_IS_ONLY_THE_NORTH_MARK", texts.Count == 1 && texts[0] == "N", string.Join("|", texts));
        Hard("MINIMAP_SHOWS_NO_RACK_PLANNING_PREVIEW", Count(Mini, MapSymbol.RackPreview) == 0, "");
        Hard("MINIMAP_DRAWS_ONLY_SHARED_MINI_SYMBOLS", Mini.LastPlan.All(i => StandMapSymbols.IsDrawn(i.Symbol, MapDetail.Mini)), "");
        Hard("MINIMAP_DRAWS_PATH_CLEARING_RACKS_AND_YOU", Count(Mini, MapSymbol.Path) > 0 && Count(Mini, MapSymbol.Landmark) == 1 && Count(Mini, MapSymbol.Player) == 1
            && Mini.LastPlan.Count(i => i.Symbol >= MapSymbol.RackToDo && i.Symbol <= MapSymbol.RackOther) == ui.RackLayout.Lines.Count, $"items={Mini.LastPlan.Count}");
        Hard("MINIMAP_DRAW_ITEMS_HAVE_NO_TEXT_FIELD", typeof(MapDrawItem).GetFields().All(f => f.FieldType != typeof(string)), "");
        yield break;
    }

    // ---- the same world location is the same place on both, and equals an independent expectation ----
    private IEnumerator PositionAndFacing()
    {
        Rect b = eco.StandBounds;
        float m = 0.1f * b.width;
        // Corners, edges, centre and awkward interior points of the property, each looking a different way.
        var stops = new List<(Vector3 at, Vector3 look)>
        {
            (new Vector3(b.center.x, 0f, b.center.y), new Vector3(b.center.x, 0f, b.yMax)),
            (new Vector3(b.xMin + m, 0f, b.yMin + m), new Vector3(b.center.x, 0f, b.center.y)),
            (new Vector3(b.xMax - m, 0f, b.yMin + m), new Vector3(b.xMin, 0f, b.yMin + m)),
            (new Vector3(b.xMax - m, 0f, b.yMax - m), new Vector3(b.xMax - m, 0f, b.yMin)),
            (new Vector3(b.xMin + m, 0f, b.yMax - m), new Vector3(b.xMax, 0f, b.yMax - m)),
            (new Vector3(b.center.x + 0.137f * b.width, 0f, b.center.y - 0.291f * b.width), new Vector3(b.xMin, 0f, b.yMax)),
            (new Vector3(b.center.x - 0.423f * b.width, 0f, b.center.y + 0.052f * b.width), new Vector3(b.xMax, 0f, b.yMin)),
        };
        float worstExpect = 0f, worstBetween = 0f, worstHeading = 0f;
        int i = 0;
        foreach ((Vector3 at, Vector3 look) in stops)
        {
            ui.CloseAll(); yield return Settle(3);
            Vector3 p = player.transform.position;
            player.transform.position = new Vector3(at.x, p.y, at.z);
            player.LookToward(new Vector3(look.x, p.y, look.z));
            yield return Settle(8);
            MapDrawItem mark = First(Mini, MapSymbol.Player);
            Hard($"STOP{i}_MINIMAP_SHOWS_YOU", mark != null && Mini.PaintedVersion == ui.MapSource.Model.Version, $"version={Mini.PaintedVersion}/{ui.MapSource.Model.Version}");
            Vector3 actual = player.transform.position, forward = player.transform.forward;
            Vector2 expected = ExpectedFraction(actual);
            Vector2 onMini = Fraction(Mini, mark);
            float headingTruth = StandMapTransform.HeadingOf(forward.x, forward.z), headingMini = HeadingFromArrow(Mini, mark);
            float miniSide = Mini.LastSidePx;

            ui.ShowMap(); yield return Settle(8);
            MapDrawItem markFull = First(Full, MapSymbol.Player);
            Hard($"STOP{i}_FULL_MAP_SHOWS_YOU", markFull != null && Full.PaintedVersion == ui.MapSource.Model.Version, $"version={Full.PaintedVersion}/{ui.MapSource.Model.Version}");
            Vector2 onFull = Fraction(Full, markFull);
            float headingFull = HeadingFromArrow(Full, markFull);
            float fullSide = Full.LastSidePx;

            float dMini = Distance(onMini, expected), dFull = Distance(onFull, expected), between = Distance(onMini, onFull);
            worstExpect = Mathf.Max(worstExpect, dMini, dFull); worstBetween = Mathf.Max(worstBetween, between);
            worstHeading = Mathf.Max(worstHeading, AngleDifference(headingMini, headingTruth), AngleDifference(headingFull, headingTruth));
            Hard($"STOP{i}_SAME_RELATIVE_POSITION_ON_BOTH", between < 1e-4f && dMini < 2e-4f && dFull < 2e-4f,
                $"world=({F(actual.x)},{F(actual.z)}) expected=({F(expected.x)},{F(expected.y)}) mini=({F(onMini.x)},{F(onMini.y)}) full=({F(onFull.x)},{F(onFull.y)}) sides={F(miniSide)}/{F(fullSide)}");
            Hard($"STOP{i}_FACING_IS_THE_SAME_ON_BOTH", AngleDifference(headingMini, headingTruth) < 1f && AngleDifference(headingFull, headingTruth) < 1f,
                $"truth={F(headingTruth)} mini={F(headingMini)} full={F(headingFull)}");
            i++;
        }
        Note("POSITION_PARITY", $"7 stops: worst distance from independent expectation {worstExpect:0.0e+0}, worst mini-vs-full fraction difference {worstBetween:0.0e+0}, worst heading error {F(worstHeading)} deg; sides mini={F(Mini.LastSidePx)} full={F(Full.LastSidePx)}");
        Hard("THE_TWO_PRESENTATIONS_ARE_DIFFERENT_SIZES", Mathf.Abs(Mini.LastSidePx - Full.LastSidePx) > 100f, $"{Mini.LastSidePx} vs {Full.LastSidePx}");
        ui.CloseAll(); yield return Settle(4);
    }

    // ---- every shared item (paths, racks, clearing, property edge) lands at the same fraction on both ----
    private IEnumerator WholePlanParity()
    {
        ui.CloseAll(); yield return Settle(6);
        var mini = Mini.LastPlan.ToList(); float miniSide = Mini.LastSidePx;
        ui.ShowMap(); yield return Settle(8);
        var full = Full.LastPlan.ToList(); float fullSide = Full.LastSidePx;
        Note("PLAN_VERSIONS", $"minimap painted model version {Mini.PaintedVersion}, full map {Full.PaintedVersion} (the player did not move between the two)");
        int matched = 0, missing = 0, points = 0, expectedPoints = 0;
        float worst = 0f;
        foreach (MapDrawItem item in mini)
        {
            // Every outline / line point of a symbol that is not a marker is compared (markers are sized from the side, not placed by it).
            if (item.Symbol != MapSymbol.Player && item.Symbol != MapSymbol.Waypoint && item.Symbol != MapSymbol.Landmark) expectedPoints += item.Points.Count;
            MapDrawItem other = full.FirstOrDefault(o => o.Symbol == item.Symbol && o.SourceIndex == item.SourceIndex && o.Points.Count == item.Points.Count);
            if (other == null) { missing++; continue; }
            matched++;
            for (int k = 0; k < item.Points.Count; k++)
            {
                // Markers are sized from the map's own side; the line / outline points are the world mapping.
                if (item.Symbol == MapSymbol.Player || item.Symbol == MapSymbol.Waypoint || item.Symbol == MapSymbol.Landmark) break;
                Vector2 a = new Vector2(item.Points[k].X / miniSide, item.Points[k].Y / miniSide), c = new Vector2(other.Points[k].X / fullSide, other.Points[k].Y / fullSide);
                worst = Mathf.Max(worst, Distance(a, c)); points++;
            }
            worst = Mathf.Max(worst, Distance(new Vector2(item.Anchor.X / miniSide, item.Anchor.Y / miniSide), new Vector2(other.Anchor.X / fullSide, other.Anchor.Y / fullSide)));
        }
        Hard("PLAN_EVERY_MINIMAP_ITEM_HAS_A_FULL_MAP_COUNTERPART", missing == 0 && matched == mini.Count && mini.Count >= 8, $"matched={matched} missing={missing} of {mini.Count}");
        Hard("PLAN_PATH_RACK_AND_BOUNDARY_POINTS_ARE_AT_THE_SAME_FRACTIONS", worst < 1e-4f && points == expectedPoints && points >= 60, $"points={points} of {expectedPoints} worst={worst:0.0e+0}");
        Hard("PLAN_FULL_MAP_HAS_NOTHING_THE_MINIMAP_HAS_MOVED", full.Count >= mini.Count, $"full={full.Count} mini={mini.Count}");
        Note("PLAN_COUNTS", $"minimap={mini.Count} ({Describe(mini)}) full={full.Count} ({Describe(full)})");
        ui.CloseAll(); yield return Settle(4);
    }

    private static string Describe(IEnumerable<MapDrawItem> plan) => string.Join(",", plan.GroupBy(i => i.Symbol).Select(g => g.Key + "x" + g.Count()));

    // ---- the real cell buttons sit where the shared transform puts them ----
    private IEnumerator CellLayout()
    {
        ui.ShowMap(); yield return Settle(8);
        VisualElement frame = ui.Map.Root.Q("map-frame"), grid = ui.Map.Grid;
        Rect g = grid.worldBound, c = Full.worldBound;
        Hard("CELLS_FRAME_GRID_AND_CANVAS_ARE_THE_SAME_SQUARE", Mathf.Abs(g.width - g.height) < 1f && Mathf.Abs(g.x - c.x) < 0.6f && Mathf.Abs(g.y - c.y) < 0.6f && Mathf.Abs(g.width - c.width) < 0.6f && Mathf.Abs(g.height - c.height) < 0.6f && Mathf.Abs(frame.worldBound.width - g.width) < 0.6f,
            $"grid={g} canvas={c} frame={frame.worldBound}");
        var cells = ui.Map.Root.Query<Button>(className: "map-cell").ToList();
        int n = eco.CellsPerAxis;
        Hard("CELLS_ALL_PRESENT_IN_THE_OLD_DOM_ORDER", cells.Count == n * n, $"{cells.Count}");
        float worst = 0f, worstSize = 0f;
        for (int k = 0; k < cells.Count; k++)
        {
            // The grid is built from the top row (north) down, west to east.
            int z = n - 1 - k / n, x = k % n, index = z * n + x;
            Vector2 centre = eco.Cells[index].Center;
            Vector2 expected = ExpectedFraction(new Vector3(centre.x, 0f, centre.y));
            Rect r = cells[k].worldBound;
            Vector2 actual = new Vector2((r.center.x - g.x) / g.width, (r.center.y - g.y) / g.height);
            worst = Mathf.Max(worst, Distance(actual, expected));
            worstSize = Mathf.Max(worstSize, Mathf.Abs(r.width / g.width - 1f / n), Mathf.Abs(r.height / g.height - 1f / n));
        }
        Hard("CELLS_EVERY_BUTTON_IS_CENTRED_ON_ITS_WORLD_POSITION", worst < 0.002f, $"worst={worst:0.0e+0}");
        Hard("CELLS_TILE_THE_MAP_AT_ONE_SQUARE_EACH", worstSize < 0.002f, $"worst={worstSize:0.0e+0}");
        // The player's cell button and the player arrow are the same place.
        int playerCell = eco.GetCellIndex(player.transform.position);
        Rect pr = cells[(n - 1 - playerCell / n) * n + playerCell % n].worldBound;
        MapDrawItem you = First(Full, MapSymbol.Player);
        Vector2 youPx = new Vector2(g.x + you.Anchor.X / Full.LastSidePx * g.width, g.y + you.Anchor.Y / Full.LastSidePx * g.height);
        Hard("CELLS_THE_ARROW_IS_INSIDE_THE_PLAYERS_CELL_BUTTON", pr.Contains(youPx), $"cell={pr} arrow={youPx}");
        Note("CELLS", $"cell button px={F(cells[0].worldBound.width, "0.0")} grid px={F(g.width, "0.0")} (screen {Screen.width}x{Screen.height})");
        ui.CloseAll(); yield return Settle(4);
    }

    // ---- a rack confirmed through the real backend appears on the minimap, then the full map, from the same data ----
    private IEnumerator RackConfirmation()
    {
        ui.CloseAll(); yield return Settle(6);
        planner.Refresh();
        RackLayoutLine line = ui.RackLayout.Lines.FirstOrDefault(l => !l.Done);
        Hard("RACK_A_LAYOUT_RACK_STILL_TO_PLAN_EXISTS", line != null, "");
        if (line == null) yield break;
        MapRack before = ui.MapSource.Model.Racks.First(r => r.LayoutIndex == line.Index);
        int toDoBefore = Count(Mini, MapSymbol.RackToDo), plannedBefore = Count(Mini, MapSymbol.RackPlanned);
        Hard("RACK_IT_SHOWS_AS_STILL_TO_PLAN_ON_THE_MINIMAP", before.Symbol == MapSymbol.RackToDo && MiniRackSymbol(line.Index) == MapSymbol.RackToDo, $"model={before.Symbol}");

        RackProposal proposal = planner.Propose(new RackPoint(0f, line.AcrossM), planner.DefaultAngle(0f, true));
        Hard("RACK_PROPOSAL_IS_CONFIRMABLE_ON_THAT_LAYOUT_RACK", proposal != null && proposal.CanConfirm && proposal.LayoutIndex == line.Index, proposal == null ? "none" : $"layout={proposal.LayoutIndex} canConfirm={proposal.CanConfirm}");
        if (proposal == null || !proposal.CanConfirm) yield break;
        int added = manager.ConfirmRack(proposal, "forest");
        Hard("RACK_THE_REAL_BACKEND_ADDED_IT", added > 0, $"trees={added}");
        int frames = 0;
        while (MiniRackSymbol(line.Index) == MapSymbol.RackToDo && frames < 120) { frames++; yield return null; }
        Hard("RACK_THE_MINIMAP_CHANGED_BY_ITSELF_WITHIN_A_FEW_FRAMES", MiniRackSymbol(line.Index) == MapSymbol.RackPlanned && frames <= 6, $"symbol={MiniRackSymbol(line.Index)} frames={frames}");
        Hard("RACK_MINIMAP_COUNTS_MOVED_ONE_TO_PLANNED", Count(Mini, MapSymbol.RackToDo) == toDoBefore - 1 && Count(Mini, MapSymbol.RackPlanned) == plannedBefore + 1, $"toDo {toDoBefore}->{Count(Mini, MapSymbol.RackToDo)} planned {plannedBefore}->{Count(Mini, MapSymbol.RackPlanned)}");
        ui.ShowMap(); yield return Settle(8);
        MapDrawItem onFull = Full.LastPlan.FirstOrDefault(i => i.Symbol >= MapSymbol.RackToDo && i.Symbol <= MapSymbol.RackOther && ui.MapSource.Model.Racks[i.SourceIndex].LayoutIndex == line.Index);
        Hard("RACK_THE_FULL_MAP_SHOWS_THE_SAME_STATE", onFull != null && onFull.Symbol == MapSymbol.RackPlanned, onFull == null ? "missing" : onFull.Symbol.ToString());
        ui.CloseAll(); yield return Settle(4);

        // A rack that is not on the planned layout: crossways to the rows.
        RackProposal other = planner.Propose(new RackPoint(eco.StandBounds.center.x + 6f, eco.StandBounds.center.y), 90f);
        if (other != null && other.CanConfirm && other.LayoutIndex < 0)
        {
            int othersBefore = Count(Mini, MapSymbol.RackOther);
            int more = manager.ConfirmRack(other, "forest");
            frames = 0;
            while (Count(Mini, MapSymbol.RackOther) == othersBefore && frames < 120) { frames++; yield return null; }
            Hard("RACK_AN_OFF_LAYOUT_RACK_APPEARS_ON_THE_MINIMAP", more > 0 && Count(Mini, MapSymbol.RackOther) == othersBefore + 1 && frames <= 6, $"trees={more} frames={frames}");
            MapDrawItem seg = First(Mini, MapSymbol.RackOther);
            MapRack rack = ui.MapSource.Model.Racks[seg.SourceIndex];
            Vector2 s = ExpectedFraction(new Vector3(rack.Start.X, 0f, rack.Start.Z));
            Hard("RACK_ITS_ENDS_ARE_AT_THE_EXPECTED_FRACTIONS", Distance(new Vector2(seg.Points[0].X, seg.Points[0].Y) / Mini.LastSidePx, s) < 2e-4f, $"expected=({F(s.x)},{F(s.y)})");
            ui.ShowMap(); yield return Settle(8);
            Hard("RACK_THE_FULL_MAP_SHOWS_THE_OFF_LAYOUT_RACK_TOO", Count(Full, MapSymbol.RackOther) == othersBefore + 1, $"{Count(Full, MapSymbol.RackOther)}");
            ui.CloseAll(); yield return Settle(4);
        }
        else Unverified("an off-layout (cross-row) rack could not be confirmed by the planner here, so its appearance on the maps was not exercised");
    }

    // ---- save and load: what the two maps keep ----
    // The planned layout is derived from the forest and the Work Plan, so it must come back. A rack confirmed OFF the layout is
    // remembered only by the session's rack groups (existing design, no save field), so it is expected NOT to come back; this stage
    // measures and reports exactly that, it does not change it.
    private IEnumerator SaveAndLoad()
    {
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        if (saves == null) { Unverified("no save controller found, so the maps after a load were not exercised"); yield break; }
        ui.CloseAll(); yield return Settle(6);
        int toDo = Count(Mini, MapSymbol.RackToDo), planned = Count(Mini, MapSymbol.RackPlanned), open = Count(Mini, MapSymbol.RackOpen), other = Count(Mini, MapSymbol.RackOther);
        Vector2 youBefore = Fraction(Mini, First(Mini, MapSymbol.Player));
        int fellOrdersBefore = manager.WorkOrders.Count(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);
        ForestSaveData data = saves.CaptureData();
        saves.LoadData(data, false);
        for (int i = 0; i < 4; i++) yield return null;
        yield return Wait(1.0f);
        yield return Settle(10);
        int toDoAfter = Count(Mini, MapSymbol.RackToDo), plannedAfter = Count(Mini, MapSymbol.RackPlanned), openAfter = Count(Mini, MapSymbol.RackOpen), otherAfter = Count(Mini, MapSymbol.RackOther);
        Hard("LOAD_LAYOUT_RACKS_ARE_STILL_ON_THE_MINIMAP_IN_THE_SAME_STATES", toDoAfter == toDo && plannedAfter == planned && openAfter == open, $"toDo {toDo}->{toDoAfter} planned {planned}->{plannedAfter} open {open}->{openAfter}");
        int fellOrdersAfter = manager.WorkOrders.Count(o => o.IsOpen && o.type == ScenarioWorkType.FellTree);
        Hard("LOAD_THE_RACK_TREES_ARE_STILL_PLANNED_FELLING_JOBS", fellOrdersBefore > 0 && fellOrdersAfter == fellOrdersBefore, $"open felling orders {fellOrdersBefore}->{fellOrdersAfter}");
        MapDrawItem you = First(Mini, MapSymbol.Player);
        Hard("LOAD_THE_PLAYER_IS_STILL_SHOWN_AT_THE_SAME_PLACE", you != null && Distance(Fraction(Mini, you), youBefore) < 2e-3f, you == null ? "missing" : $"{youBefore} -> {Fraction(Mini, you)}");
        ui.ShowMap(); yield return Settle(8);
        Hard("LOAD_THE_FULL_MAP_AGREES_WITH_THE_MINIMAP", Count(Full, MapSymbol.RackToDo) == toDoAfter && Count(Full, MapSymbol.RackPlanned) == plannedAfter && Count(Full, MapSymbol.RackOther) == otherAfter, $"full toDo={Count(Full, MapSymbol.RackToDo)} planned={Count(Full, MapSymbol.RackPlanned)} other={Count(Full, MapSymbol.RackOther)}");
        ui.CloseAll(); yield return Settle(4);
        Note("LOAD_OFF_LAYOUT_RACKS", $"offLayoutRacksBeforeSaveLoad={other} afterLoad={otherAfter}; rack groups after load={manager.RackGroups.Count} (RackGroups are session-only by existing design; see LOAD_THE_RACK_TREES_ARE_STILL_PLANNED_FELLING_JOBS for the trees; only the map's line for an off-layout rack is not restored)");
        if (other > 0 && otherAfter == 0) Note("KNOWN_LIMIT_FOR_MANAGER_TRIAGE", "a rack confirmed OFF the planned layout is drawn on both maps this session but is not drawn after a save/load (rack groups are session-only; no save schema change was made)");
    }

    private MapSymbol MiniRackSymbol(int layoutIndex)
    {
        MapDrawItem item = Mini.LastPlan.FirstOrDefault(i => i.Symbol >= MapSymbol.RackToDo && i.Symbol <= MapSymbol.RackOther && ui.MapSource.Model.Racks[i.SourceIndex].LayoutIndex == layoutIndex);
        return item == null ? MapSymbol.Property : item.Symbol;
    }

    // ---- the destination ----
    private IEnumerator Waypoint()
    {
        ui.ShowMap(); yield return Settle(6);
        Hard("WAYPOINT_NONE_YET_NONE_DRAWN", Count(Full, MapSymbol.Waypoint) == 0, "");
        int index = Mathf.Min(eco.CellCount - 1, eco.CellsPerAxis * 3 + 11);
        Hard("WAYPOINT_SET_ON_THE_STAND_MAP", ui.Map.SetWaypointCell(eco, index), $"cell={index}");
        yield return Settle(8);
        Vector2 centre = eco.Cells[index].Center;
        Vector2 expected = ExpectedFraction(new Vector3(centre.x, 0f, centre.y));
        MapDrawItem onFull = First(Full, MapSymbol.Waypoint);
        Hard("WAYPOINT_FULL_MAP_DRAWS_IT_AT_THE_CELL_CENTRE", onFull != null && Distance(Fraction(Full, onFull), expected) < 2e-4f, onFull == null ? "missing" : $"{Fraction(Full, onFull)} vs {expected}");
        ui.CloseAll(); yield return Settle(8);
        MapDrawItem onMini = First(Mini, MapSymbol.Waypoint);
        Hard("WAYPOINT_MINIMAP_DRAWS_THE_SAME_PLACE", onMini != null && Distance(Fraction(Mini, onMini), expected) < 2e-4f, onMini == null ? "missing" : $"{Fraction(Mini, onMini)} vs {expected}");
    }

    // ---- rack planning is a full-map thing ----
    private IEnumerator PlanningPreviewIsFullMapOnly()
    {
        ui.ShowMap(); yield return Settle(8);
        Button plan = ui.RootElement.Query<Button>().ToList().FirstOrDefault(b => b.text != null && b.text.StartsWith("Plan rack", StringComparison.Ordinal) && Visible(b));
        if (plan == null) { Unverified("the 'Plan rack' button was not offered here, so the planning preview on the full map was not exercised"); ui.CloseAll(); yield break; }
        yield return Invoke(plan);
        Hard("PREVIEW_PLAN_RACK_MODE_IS_ON", ui.Map.RackMode, "");
        int n = eco.CellsPerAxis;
        var cells = ui.RootElement.Query<Button>(className: "map-cell").ToList();
        RackLayoutLine open = ui.RackLayout.Lines.FirstOrDefault(l => !l.Done);
        int cell = eco.GetCellIndex(new Vector3(0f, 0f, open != null ? open.AcrossM : eco.StandBounds.center.y));
        yield return Invoke(cells[(n - 1 - cell / n) * n + cell % n]);
        yield return Wait(0.3f);
        yield return Settle(6);
        Hard("PREVIEW_THE_FULL_MAP_DRAWS_THE_RACK_BEING_PLANNED", ui.Map.RackPreview != null && Count(Full, MapSymbol.RackPreview) == 1, $"preview={(ui.Map.RackPreview != null)} items={Count(Full, MapSymbol.RackPreview)}");
        yield return Capture("stand-map-rack-planning");
        ui.CloseAll(); yield return Settle(8);
        Hard("PREVIEW_THE_MINIMAP_DOES_NOT", Count(Mini, MapSymbol.RackPreview) == 0 && ui.Minimap.Root.Query<Button>().ToList().Count == 0, "");
        ui.ShowMap(); yield return Settle(4);
        Button cancel = ui.RootElement.Q<Button>("map-rack-cancel");
        if (cancel != null) yield return Invoke(cancel);
        ui.CloseAll(); yield return Settle(4);
    }

    // ---- layout at several screen sizes, with screenshots ----
    private IEnumerator Layouts()
    {
        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y);
            ui.CloseAll(); yield return Settle(20);
            string tag = $"{size.x}x{size.y}";
            Rect root = ui.RootElement.worldBound, mini = ui.Minimap.Root.worldBound;
            Hard($"LAYOUT_{tag}_MINIMAP_IS_ON_SCREEN_BOTTOM_RIGHT", root.Contains(mini.min) && root.Contains(mini.max) && mini.center.x > root.center.x && mini.center.y > root.center.y, $"mini={mini} root={root}");
            var overlaps = new List<string>();
            foreach (VisualElement child in ui.RootElement.Query<VisualElement>().ToList())
            {
                if (child == ui.Minimap.Root || ui.Minimap.Root.Contains(child) || child.Contains(ui.Minimap.Root) || !Visible(child)) continue;
                // Only things that actually draw: a panel, or text.
                bool draws = child is TextElement t ? !string.IsNullOrEmpty(t.text) : child.ClassListContains("panel") || child.ClassListContains("card");
                if (draws && child.worldBound.Overlaps(mini)) overlaps.Add((string.IsNullOrEmpty(child.name) ? string.Join(".", child.GetClasses()) : child.name) + child.worldBound);
            }
            Hard($"LAYOUT_{tag}_MINIMAP_OVERLAPS_NOTHING_ELSE_ON_THE_WALKING_HUD", overlaps.Count == 0, string.Join(" ; ", overlaps));
            yield return Capture("hud-minimap");

            ui.ShowMap(); yield return Settle(20);
            Rect frame = ui.Map.Root.Q("map-frame").worldBound, side = ui.Map.Root.Q(className: "map-side").worldBound;
            root = ui.RootElement.worldBound;
            var cells = ui.Map.Root.Query<Button>(className: "map-cell").ToList();
            bool inside = cells.All(c => root.Contains(c.worldBound.min) && root.Contains(c.worldBound.max));
            Hard($"LAYOUT_{tag}_FULL_MAP_FITS_ON_SCREEN_AND_CLEARS_THE_SIDE_PANEL", inside && !frame.Overlaps(side) && cells.Count == eco.CellCount, $"frame={frame} side={side} root={root}");
            Note($"LAYOUT_{tag}", $"cell px={F(cells[0].worldBound.width, "0.0")} frame={frame}");
            // Every layer: the text of each square has to sit inside its own square, and nothing on the screen may run past the
            // bottom of the window (the legend lines under the map were once cut off by it).
            string[] layerButtons = { "Access & position", "Light", "Regeneration", "Browsing / protection", "Fell & crop marks" };
            string[] layerShots = { "stand-map", "stand-map-light", "stand-map-regeneration", "stand-map-browsing", "stand-map-marks" };
            for (int l = 0; l < layerButtons.Length; l++)
            {
                {
                    Button tab = ui.Map.Root.Q("map-tabs").Query<Button>().ToList().FirstOrDefault(b => b.text == layerButtons[l]);
                    yield return Invoke(tab);
                    yield return Settle(6);
                }
                string layerTag = $"LAYOUT_{tag}_{layerButtons[l].Replace(" ", "").Replace("/", "").Replace("&", "")}";
                cells = ui.Map.Root.Query<Button>(className: "map-cell").ToList();
                var spill = new List<string>();
                int labelCount = 0;
                float worstOver = 0f;
                foreach (Button cell in cells)
                    foreach (Label label in cell.Query<Label>().ToList())
                    {
                        if (string.IsNullOrEmpty(label.text)) continue;
                        labelCount++;
                        Rect lb = label.worldBound, cb = cell.worldBound;
                        float over = Mathf.Max(Mathf.Max(cb.xMin - lb.xMin, lb.xMax - cb.xMax), Mathf.Max(cb.yMin - lb.yMin, lb.yMax - cb.yMax));
                        worstOver = Mathf.Max(worstOver, over);
                        if (over > 0.75f) spill.Add($"'{label.text}' label={lb} cell={cb}");
                    }
                Button first = cells[cells.Count - 1];
                Note(layerTag + "_FIRST_CELL_GEOMETRY", $"cell={first.worldBound} " + string.Join(" ", first.Query<VisualElement>().ToList().Where(e => e != first).Select(e => $"{(e is Label lab ? "label'" + lab.text + "'" : "box")}={e.worldBound}")));
                // The text is what has to be readable: a label may touch the square's edge, but none may stick out of the square.
                Hard(layerTag + "_CELL_TEXT_STAYS_INSIDE_ITS_SQUARE", worstOver <= 0.75f, $"labels={labelCount} spill={spill.Count} worstOvershoot={worstOver:0.##} e.g. {string.Join(" ; ", spill.Take(2))}");
                Hard(layerTag + "_OVERLAY_STYLE_IS_QUIET_ONLY_OVER_NUMBERS", Full.Quiet == (l != 0) && Full.LastPlan.Where(i => i.Symbol == MapSymbol.Player || i.Symbol == MapSymbol.Waypoint).All(i => i.Filled == (l == 0)),
                    $"quiet={Full.Quiet} markersFilled={string.Join(",", Full.LastPlan.Where(i => i.Symbol == MapSymbol.Player || i.Symbol == MapSymbol.Waypoint).Select(i => i.Filled))}");
                Rect modal = ui.Map.Root.Q(className: "modal").worldBound;
                float lowest = ui.Map.Root.Query<Label>().ToList().Where(Visible).Where(t => !string.IsNullOrEmpty(t.text)).Max(t => t.worldBound.yMax);
                Hard(layerTag + "_TEXT_ENDS_INSIDE_THE_WINDOW", lowest <= modal.yMax - 4f, $"lowestText={lowest:0.#} windowBottom={modal.yMax:0.#}");
                yield return Capture(layerShots[l]);
            }
            ui.CloseAll(); yield return Settle(4);
        }
    }
}
#endif
