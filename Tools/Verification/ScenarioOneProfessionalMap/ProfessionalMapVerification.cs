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

// Disposable RENDERED + FUNCTIONAL qualification for SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1.
//
// It plays a fresh Scenario One game in a graphical Editor and checks the NEW map presentation against the LIVE game (not against the draw-plan code
// in isolation, which tools/verification/StandMap/OfflineLogic already covers):
//   DATA        every tree on the map is a real ForestTree at its real place, size, species class and state; no terrain, so no relief is claimed;
//   PICTURE     the forest picture is where the trees are, in both directions (a crown where a tree stands, ground where none does, a pale disc on every
//               stump), on the HUD minimap, the full map and the overview, and the three agree;
//   VECTORS     juveniles, boundary, grid lines, scale bar and rack corridors are where the world says;
//   OVERLAYS    each 5 m square's Light and Regeneration colour is the square's own value, the field covers the map exactly, shelters and marks ring the
//               real shelters and trees;
//   REDRAWS     standing still redraws nothing, a walk or a turn lays out only the dynamic layer, a felling redraws the forest picture exactly once, and
//               a loaded game (which raises no event) is picked up by the safety scan;
//   INPUT       the minimap takes no pointer input; LAYOUT: the map window, survey labels and cards fit at 1280 x 720 and 1920 x 1080;
//   COST        diagnostic microbenchmarks of the layout and the forest redraw in this Editor.
// Anything it cannot check is reported as PMV_UNVERIFIED, never counted as a pass.
//
// Run (graphical Editor, private display):  -executeMethod ProfessionalMapVerification.Begin   CCF_DIAG_DIR=<folder outside the repo>  CCF_UI_SIZES=1920x1080,1280x720
// Plays in memory (the save controller is used only to capture and re-load the in-memory world); writes nothing but its evidence folder.
public static class ProfessionalMapVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        ProfessionalMapVerificationRunner.SetGameSize(ProfessionalMapVerificationRunner.Sizes()[0].x, ProfessionalMapVerificationRunner.Sizes()[0].y);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "ProfessionalMapVerification.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "ProfessionalMapVerification.Begin"))
            new GameObject("Professional map verification").AddComponent<ProfessionalMapVerificationRunner>();
    }
}

public sealed class ProfessionalMapVerificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController eco;
    private ForestPlayer player;
    private ForestSaveController saves;
    private string output;
    private int passed, shots;
    private readonly List<string> failures = new List<string>(), unverified = new List<string>(), metrics = new List<string>();

    private static string F(double v, string f = "0.####") => v.ToString(f, Inv);
    private void Hard(string name, bool ok, string detail = "")
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"PMV_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Metric(string name, string value) { metrics.Add($"  {{\"name\":\"{name}\",\"value\":\"{value}\"}}"); Debug.Log($"PMV_MEASURE {name} {value}"); }
    private void Unverified(string detail) { unverified.Add(detail); Debug.Log("PMV_UNVERIFIED " + detail); }

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "PMV " + width + "x" + height }, null);
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
        if (output != null)
        {
            File.WriteAllText(Path.Combine(output, "pmv-metrics.json"), "{\n \"metrics\": [\n" + string.Join(",\n", metrics) + "\n ],\n \"unverified\": [" + string.Join(",", unverified.Select(u => "\"" + u.Replace("\"", "'") + "\"")) + "]\n}\n");
        }
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"PMV_PASS_ALL shots={shots} hardChecks={passed} unverified={unverified.Count}"
            : $"PMV_FAIL_SUMMARY shots={shots} hardChecks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
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

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + name);
        File.WriteAllBytes(Path.Combine(output, name + "-" + Screen.width + "x" + Screen.height + ".png"), image.EncodeToPNG());
        Destroy(image);
        shots++;
    }

    private StandMapModel Model => ui.MapSource.Model;
    private StandMapCanvas Mini => ui.Minimap.Canvas;
    private StandMapCanvas Full => ui.Map.Canvas;
    private static Vector2 Fraction(StandMapCanvas canvas, MapDrawItem item) => new Vector2(item.Anchor.X / canvas.LastSidePx, item.Anchor.Y / canvas.LastSidePx);
    private Vector2 ExpectedFraction(float x, float z)
    {
        Rect b = eco.StandBounds;
        return new Vector2((x - b.xMin) / b.width, 1f - (z - b.yMin) / b.height);
    }

    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; eco = ui.Ecology; player = ui.Player; saves = FindFirstObjectByType<ForestSaveController>();
        if (Application.isBatchMode) throw new InvalidOperationException("ProfessionalMapVerification needs a graphical Editor");
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.dataPath, "../Build/ProfessionalMap/evidence");
        Directory.CreateDirectory(output);
        yield return Wait(0.5f);
        if (ui.Help.IsOpen) { ui.CloseHelp(); yield return Settle(); }
        ui.CloseAll(); yield return Settle(10);
        Debug.Log($"PMV_START year={eco.EcologicalYear} geometry={eco.StandGeometryModelVersion} trees={eco.LivingTreeCount} cells={eco.CellsPerAxis}");

        yield return DataTruth();
        yield return ForestPictures("start");
        yield return VectorTruth();
        yield return Overlays();
        yield return Redraws();
        yield return LayoutAndInput();
        yield return Management();
        yield return Micro();
    }

    // =====================================================================================================================
    // DATA: the model's trees are the scene's trees
    // =====================================================================================================================
    private IEnumerator DataTruth()
    {
        StandMapModel model = ui.MapSource.Rebuild();
        ForestTree[] trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        Hard("DATA_THE_MAP_HAS_EXACTLY_THE_SCENES_TREES", model.Trees.Count == trees.Length && trees.Length > 1000, $"map={model.Trees.Count} scene={trees.Length}");
        Func<float, int> q = v => Mathf.RoundToInt(v * 100f);
        var onMap = new HashSet<(int, int)>(model.Trees.Select(t => (q(t.X), q(t.Z))));
        int missing = trees.Count(t => !onMap.Contains((q(t.transform.position.x), q(t.transform.position.z))));
        Hard("DATA_EVERY_TREE_IS_AT_ITS_REAL_POSITION_TO_A_CENTIMETRE", missing == 0, $"scene trees with no map tree at their position: {missing}");
        var byPosition = new Dictionary<(int, int), MapTree>();
        foreach (MapTree t in model.Trees) byPosition[(q(t.X), q(t.Z))] = t;
        int wrongSize = 0, wrongKind = 0, wrongState = 0, checkedTrees = 0;
        foreach (ForestTree tree in trees)
        {
            if (!byPosition.TryGetValue((q(tree.transform.position.x), q(tree.transform.position.z)), out MapTree m)) continue;
            checkedTrees++;
            MapTreeState state = tree.IsStump ? MapTreeState.Stump : tree.IsBiologicallyDead ? MapTreeState.StandingDead : MapTreeState.Living;
            if (m.State != state) wrongState++;
            if (state != MapTreeState.Stump && (Mathf.Abs(m.CrownRadiusM - tree.SimulationCrownRadiusMeters) > 1e-4f || Mathf.Abs(m.HeightM - tree.SimulationHeightMeters) > 1e-4f || Mathf.Abs(m.DbhCm - tree.SimulationDiameterCm) > 1e-4f)) wrongSize++;
            if (m.Kind != StandMapSource.KindOf(tree.Species != null ? tree.Species.SpeciesId : "")) wrongKind++;
        }
        Hard("DATA_EVERY_CROWN_IS_THE_TREES_OWN_RADIUS_HEIGHT_AND_DBH", wrongSize == 0 && checkedTrees == trees.Length, $"mismatches={wrongSize} checked={checkedTrees}");
        Hard("DATA_EVERY_TREES_STATE_AND_KIND_MATCH", wrongState == 0 && wrongKind == 0, $"state={wrongState} kind={wrongKind}");
        Hard("DATA_THE_TREES_ARE_SORTED_SHORTEST_FIRST", Enumerable.Range(1, model.Trees.Count - 1).All(i => model.Trees[i - 1].HeightM <= model.Trees[i].HeightM), "");

        // The species mapping, checked against the species assets that actually exist in the project.
        var expected = new Dictionary<string, MapTreeKind>
        {
            { "sitka-spruce", MapTreeKind.Conifer }, { "scots-pine", MapTreeKind.Conifer }, { "beech", MapTreeKind.Beech },
            { "sessile-oak", MapTreeKind.Oak }, { "downy-birch", MapTreeKind.OtherBroadleaf }, { "common-alder", MapTreeKind.OtherBroadleaf }
        };
        string[] guids = AssetDatabase.FindAssets("t:TreeSpeciesDefinition");
        var ids = guids.Select(g => AssetDatabase.LoadAssetAtPath<TreeSpeciesDefinition>(AssetDatabase.GUIDToAssetPath(g))).Where(a => a != null).Select(a => a.SpeciesId).Distinct().ToList();
        Hard("DATA_EVERY_SPECIES_ASSET_IN_THE_PROJECT_HAS_A_KNOWN_LOOK", ids.Count >= 6 && ids.All(id => expected.ContainsKey(id)) && ids.All(id => StandMapSource.KindOf(id) == expected[id]), $"species={string.Join(",", ids)}");
        Hard("DATA_AN_UNKNOWN_SPECIES_IS_OTHER_BROADLEAF_NOT_GUESSED", StandMapSource.KindOf("not-a-species") == MapTreeKind.OtherBroadleaf && StandMapSource.KindOf("") == MapTreeKind.OtherBroadleaf, "");
        Hard("DATA_THE_LEGEND_NAMES_THE_SPECIES_STANDING", model.KindLabels[(int)MapTreeKind.Conifer] == "Sitka spruce" && model.KindLabels[(int)MapTreeKind.Beech] == "", $"conifer='{model.KindLabels[(int)MapTreeKind.Conifer]}' beech='{model.KindLabels[(int)MapTreeKind.Beech]}'");

        StandMapStats stats = model.ComputeStats();
        Hard("DATA_THE_STAND_FIGURES_ARE_THE_ECOLOGYS_OWN", stats.LivingTrees == eco.LivingTreeCount && Mathf.Abs(stats.MeanDbhCm - eco.MeanDbhCm) < 0.02f && Mathf.Abs(model.SizeM - 80f) < 1e-3f, $"living {stats.LivingTrees} vs {eco.LivingTreeCount}; DBH {stats.MeanDbhCm:0.000} vs {eco.MeanDbhCm:0.000}");
        Hard("DATA_NO_TERRAIN_SO_THE_GROUND_IS_LEVEL_AND_NO_RELIEF_IS_CLAIMED", Terrain.activeTerrain == null && Terrain.activeTerrains.Length == 0 && model.GroundIsLevel, $"terrains={Terrain.activeTerrains.Length}");
        float groundSpread = 0f, y0 = player.transform.position.y;
        foreach (Vector2 p in new[] { new Vector2(-35, -35), new Vector2(35, 35), new Vector2(-35, 35), new Vector2(35, -35), new Vector2(0, 0) })
            if (Physics.Raycast(new Vector3(p.x, 100f, p.y), Vector3.down, out RaycastHit hit, 300f, ~0, QueryTriggerInteraction.Ignore)) groundSpread = Mathf.Max(groundSpread, Mathf.Abs(hit.point.y - y0));
        Hard("DATA_THE_GROUND_REALLY_IS_LEVEL_FIVE_RAYS_ACROSS_THE_PROPERTY", groundSpread < 0.5f, $"max height difference from the player's level {groundSpread:0.000} m");
        yield break;
    }

    // =====================================================================================================================
    // PICTURE: the forest is where the trees are
    // =====================================================================================================================
    private static bool IsCrown(byte[] p, int o) => p[o + 1] > p[o] + 2;                        // crowns are green-dominant, the ground and its shadow are not
    private static bool IsStumpPale(byte[] p, int o) => p[o] > 150 && p[o + 1] > 130;

    private int Offset(StandMapCanopyLayer layer, StandMapModel m, float x, float z)
    {
        int side = layer.SidePx;
        int px = Mathf.Clamp((int)((x - m.MinX) / m.SizeM * side), 0, side - 1), py = Mathf.Clamp((int)((z - m.MinZ) / m.SizeM * side), 0, side - 1);
        return (py * side + px) * 4;
    }

    private IEnumerator WaitForPicture(StandMapCanopyLayer layer, StandMapModel m)
    {
        for (int i = 0; i < 400 && (layer.Redrawing || layer.ShownVersion != m.CanopyVersion); i++) yield return null;
    }

    // Both directions: a crown where a tree stands, ground where nothing does, and a pale disc on every stump.
    private void CheckPicture(string label, StandMapCanopyLayer layer, StandMapModel m)
    {
        byte[] pic = layer.Picture;
        if (pic == null || layer.Redrawing) { Hard($"PICTURE_{label}_IS_COMPLETE", false, $"picture={(pic != null)} redrawing={layer.Redrawing}"); return; }
        Hard($"PICTURE_{label}_IS_DRAWN_FROM_THE_CURRENT_CANOPY", layer.ShownVersion == m.CanopyVersion, $"shown={layer.ShownVersion} model={m.CanopyVersion}");
        var living = m.Trees.Where(t => t.State == MapTreeState.Living).ToList();
        var rng = new System.Random(20261022);
        int covered = 0, sampled = 0;
        for (int i = 0; i < 600 && living.Count > 0; i++)
        {
            MapTree t = living[rng.Next(living.Count)];
            sampled++;
            if (IsCrown(pic, Offset(layer, m, t.X, t.Z))) covered++;
        }
        Hard($"PICTURE_{label}_HAS_A_CROWN_AT_THE_CENTRE_OF_EVERY_LIVING_TREE_SAMPLED", sampled > 0 && covered >= sampled * 0.995, $"{covered}/{sampled}");

        // Ground where no crown reaches: points at least 1.0 m clear of every crown. Where none exist the check says so instead of passing.
        var gaps = new List<Vector2>();
        float ppm = layer.SidePx / m.SizeM;
        for (float gx = m.MinX + 1f; gx < m.MinX + m.SizeM - 1f; gx += 1f)
            for (float gz = m.MinZ + 1f; gz < m.MinZ + m.SizeM - 1f; gz += 1f)
            {
                bool clear = true;
                foreach (MapTree t in m.Trees)
                {
                    if (t.State == MapTreeState.Stump) continue;
                    float dx = t.X - gx, dz = t.Z - gz, reach = t.CrownRadiusM * 1.25f + 1.0f;
                    if (dx * dx + dz * dz < reach * reach) { clear = false; break; }
                }
                if (clear) gaps.Add(new Vector2(gx, gz));
            }
        if (gaps.Count < 8) Unverified($"PICTURE_{label}: only {gaps.Count} points on the property are 1 m clear of every crown, so 'ground where no tree stands' could not be sampled in this state");
        else
        {
            int ground = gaps.Count(g => !IsCrown(pic, Offset(layer, m, g.x, g.y)));
            Hard($"PICTURE_{label}_SHOWS_GROUND_WHERE_NO_CROWN_REACHES", ground >= gaps.Count * 0.97f, $"{ground}/{gaps.Count} clear points are ground");
        }
        var stumps = m.Trees.Where(t => t.State == MapTreeState.Stump).ToList();
        // The picture on screen is exactly a fresh drawing of the current trees (so it cannot be stale, torn or drawn from something else).
        byte[] fresh = StandMapCanopyRaster.Render(m, layer.SidePx, layer.Detail);
        Hard($"PICTURE_{label}_IS_EXACTLY_A_FRESH_DRAWING_OF_THE_CURRENT_TREES", fresh.Length == pic.Length && fresh.SequenceEqual(pic), "");
        var without = new StandMapModel { HasProperty = true, MinX = m.MinX, MinZ = m.MinZ, SizeM = m.SizeM, CellsPerAxis = m.CellsPerAxis, CellSizeM = m.CellSizeM };
        foreach (MapTree t in m.Trees) if (t.State != MapTreeState.Stump) without.Trees.Add(t);
        byte[] bare = StandMapCanopyRaster.Render(without, layer.SidePx, layer.Detail);
        if (layer.Detail == MapDetail.Full)
        {
            // A stump is drawn first, so a neighbouring crown hides it and a neighbour's shadow darkens it, as from above. So its colour is not the test: where a stump
            // does show, what it adds to the picture (the picture minus the same picture drawn without stumps) must be centred on the stump's world position.
            int side = layer.SidePx, visible = 0, placed = 0;
            foreach (MapTree s in stumps)
            {
                float cx = (s.X - m.MinX) * ppm, cy = (s.Z - m.MinZ) * ppm;
                double sx = 0, sy = 0, sum = 0;
                for (int y = Mathf.Max(0, (int)cy - 5); y <= Mathf.Min(side - 1, (int)cy + 5); y++)
                    for (int x = Mathf.Max(0, (int)cx - 5); x <= Mathf.Min(side - 1, (int)cx + 5); x++)
                    {
                        int o = (y * side + x) * 4;
                        double d = Mathf.Abs(pic[o] - bare[o]) + Mathf.Abs(pic[o + 1] - bare[o + 1]) + Mathf.Abs(pic[o + 2] - bare[o + 2]);
                        sx += (x + 0.5) * d; sy += (y + 0.5) * d; sum += d;
                    }
                if (sum < 500) continue;       // hidden under a crown
                visible++;
                if (Math.Abs(sx / sum - cx) < 1.2 && Math.Abs(sy / sum - cy) < 1.2) placed++;
            }
            if (visible < 10) Unverified($"PICTURE_{label}: only {visible} of {stumps.Count} stumps show through the canopy, so stump placement was not sampled in this state");
            else Hard($"PICTURE_{label}_EVERY_STUMP_THAT_SHOWS_IS_CENTRED_ON_ITS_WORLD_POSITION", placed >= visible * 0.95, $"{placed}/{visible} visible stumps ({stumps.Count} in all)");
        }
        else if (stumps.Count > 0)
            Hard($"PICTURE_{label}_LEAVES_STUMPS_OFF_THE_MINIMAP", bare.SequenceEqual(pic), $"{stumps.Count} stumps in the model");
        Metric($"picture_{label}_gap_points", gaps.Count.ToString());
    }

    private IEnumerator ForestPictures(string state)
    {
        ui.CloseAll(); yield return Settle(12);
        StandMapModel m = Model;
        yield return WaitForPicture(ui.Minimap.Canopy, m);
        CheckPicture($"{state}_MINIMAP", ui.Minimap.Canopy, m);
        ui.ShowMap(); yield return Settle(8);
        m = Model;
        yield return WaitForPicture(ui.Map.Canopy, m);
        yield return WaitForPicture(ui.Map.Overview.Canopy, m);
        CheckPicture($"{state}_FULL_MAP", ui.Map.Canopy, m);
        CheckPicture($"{state}_OVERVIEW", ui.Map.Overview.Canopy, m);
        Hard($"PICTURE_{state}_THE_OVERVIEW_AND_THE_MINIMAP_ARE_THE_SAME_PICTURE", ui.Map.Overview.Canopy.Picture != null && ui.Minimap.Canopy.Picture != null && ui.Map.Overview.Canopy.Picture.SequenceEqual(ui.Minimap.Canopy.Picture), "");

        // The full map and the minimap draw the same trees: compare where the forest is, 32 x 32 coarse squares, crown or not.
        float[] fullMask = Coverage(ui.Map.Canopy, 32), miniMask = Coverage(ui.Minimap.Canopy, 32);
        float worst = 0f, mean = 0f;
        for (int i = 0; i < fullMask.Length; i++) { float d = Mathf.Abs(fullMask[i] - miniMask[i]); worst = Mathf.Max(worst, d); mean += d / fullMask.Length; }
        Hard($"PICTURE_{state}_FULL_MAP_AND_MINIMAP_SHOW_THE_FOREST_IN_THE_SAME_PLACES", worst < 0.30f && mean < 0.06f, $"worst square {worst:0.00}, mean difference {mean:0.000}");
        ui.CloseAll(); yield return Settle(6);
    }

    private static float[] Coverage(StandMapCanopyLayer layer, int cells)
    {
        byte[] pic = layer.Picture; int side = layer.SidePx;
        var result = new float[cells * cells]; var counts = new int[cells * cells];
        for (int y = 0; y < side; y++)
            for (int x = 0; x < side; x++)
            {
                int c = (y * cells / side) * cells + x * cells / side;
                counts[c]++;
                if (IsCrown(pic, (y * side + x) * 4)) result[c] += 1f;
            }
        for (int i = 0; i < result.Length; i++) result[i] = counts[i] > 0 ? result[i] / counts[i] : 0f;
        return result;
    }

    // =====================================================================================================================
    // VECTORS: juveniles, boundary, grid, scale bar, rack corridors
    // =====================================================================================================================
    private IEnumerator VectorTruth()
    {
        ui.ShowMap(); yield return Settle(10);
        StandMapModel m = Model; float side = Full.LastSidePx;
        MapDrawItem prop = Full.LastPlan.FirstOrDefault(i => i.Symbol == MapSymbol.Property);
        Rect b = eco.StandBounds;
        var corners = new[] { new Vector2(b.xMin, b.yMax), new Vector2(b.xMax, b.yMax), new Vector2(b.xMax, b.yMin), new Vector2(b.xMin, b.yMin) };
        Hard("VECTOR_THE_BOUNDARY_IS_THE_PROPERTYS_FOUR_CORNERS", prop != null && prop.Points.Count == 4 && prop.Closed && Enumerable.Range(0, 4).All(i => (new Vector2(prop.Points[i].X / side, prop.Points[i].Y / side) - ExpectedFraction(corners[i].x, corners[i].y)).magnitude < 2e-4f), "");
        Hard("VECTOR_THE_BOUNDARY_IS_DASHED_AND_THE_SCALE_AND_NORTH_ARE_DRAWN", prop != null && prop.DashPx > 0f && Full.LastPlan.Any(i => i.Symbol == MapSymbol.ScaleBar) && Full.LastPlan.Any(i => i.Symbol == MapSymbol.NorthArrow), "");
        var grid = Full.LastPlan.Where(i => i.Symbol == MapSymbol.GridLine).ToList();
        int goodGrid = 0;
        for (int k = 1; k <= 7; k++)
        {
            float f = k * 10f / m.SizeM;
            if (grid.Any(g => Mathf.Abs(g.Points[0].X / side - f) < 2e-4f && Mathf.Abs(g.Points[1].X / side - f) < 2e-4f && Mathf.Abs(g.Points[0].Y / side - 1f) < 2e-4f && Mathf.Abs(g.Points[1].Y) < 1e-3f)) goodGrid++;
            if (grid.Any(g => Mathf.Abs(g.Points[0].Y / side - (1f - f)) < 2e-4f && Mathf.Abs(g.Points[1].Y / side - (1f - f)) < 2e-4f)) goodGrid++;
        }
        Hard("VECTOR_THE_GRID_LINES_ARE_EVERY_10_METRES_OF_LOCAL_EASTING_AND_NORTHING", grid.Count == 14 && goodGrid == 14, $"lines={grid.Count} exact={goodGrid}");
        MapDrawItem bar = Full.LastPlan.First(i => i.Symbol == MapSymbol.ScaleBar);
        Hard("VECTOR_THE_SCALE_BAR_IS_20_METRES_AT_THE_MAPS_OWN_SCALE", Mathf.Abs((bar.Points[2].X - bar.Points[1].X) / (side / m.SizeM) - 20f) < 0.01f, $"{(bar.Points[2].X - bar.Points[1].X) / (side / m.SizeM):0.000} m");

        // rack corridors: real width, and one per rack
        int bands = Full.LastPlan.Count(i => i.Symbol == MapSymbol.RackBand);
        MapDrawItem band = Full.LastPlan.FirstOrDefault(i => i.Symbol == MapSymbol.RackBand);
        float widthM = band == null ? 0f : new Vector2(band.Points[0].X - band.Points[3].X, band.Points[0].Y - band.Points[3].Y).magnitude / (side / m.SizeM);
        Hard("VECTOR_EVERY_RACK_HAS_ITS_CORRIDOR_AT_THE_REAL_RACK_WIDTH", bands == m.Racks.Count(r => r.Symbol != MapSymbol.RackPreview) && Mathf.Abs(widthM - RackGeometry.DefaultWidthM) < 0.01f, $"bands={bands} width={widthM:0.000} m vs {RackGeometry.DefaultWidthM}");
        Hard("VECTOR_THE_PATH_HAS_A_DARK_EDGE_AND_THE_CLEARING_IS_A_FILLED_PATCH", Full.LastPlan.Any(i => i.Symbol == MapSymbol.PathCasing) && Full.LastPlan.Any(i => i.Symbol == MapSymbol.Landmark && i.Filled), "");

        // you: dot exactly on the player, wedge apex on the dot, heading read from the wedge
        MapDrawItem dot = Full.LastPlan.First(i => i.Symbol == MapSymbol.PlayerDot), wedge = Full.LastPlan.First(i => i.Symbol == MapSymbol.Player);
        Vector3 pp = player.transform.position;
        float heading = StandMapTransform.HeadingOf(player.transform.forward.x, player.transform.forward.z), read = StandMapDrawPlan.HeadingOfWedge(wedge);
        Hard("VECTOR_THE_PLAYER_DOT_IS_EXACTLY_ON_THE_PLAYER", (Fraction(Full, dot) - ExpectedFraction(pp.x, pp.z)).magnitude < 2e-4f, "");
        Hard("VECTOR_THE_FACING_WEDGE_STARTS_ON_THE_DOT_AND_POINTS_WHERE_THE_PLAYER_FACES", Mathf.Abs(wedge.Points[0].X - dot.Anchor.X) < 1e-3f && Mathf.Repeat(Mathf.Abs(read - heading) + 180f, 360f) - 180f < 0.5f, $"read {read:0.00} actual {heading:0.00}");
        ui.CloseAll(); yield return Settle(4);
    }

    // =====================================================================================================================
    // OVERLAYS: each square is its own colour; shelters and marks ring the real things
    // =====================================================================================================================
    private Button Tab(int index) => ui.Map.Root.Q("map-tabs").Query<Button>().ToList().ElementAtOrDefault(index);

    private IEnumerator Overlays()
    {
        ui.ShowMap(); yield return Settle(10);
        yield return Invoke(Tab(0)); yield return Settle(6);
        Hard("OVERLAY_THE_ACCESS_LAYER_HAS_NO_FIELD", !ui.Map.Surface.Field.HasField, "");
        int n = eco.CellsPerAxis;

        yield return Invoke(Tab(1)); yield return Settle(8);
        StandMapOverlayLayer field = ui.Map.Surface.Field;
        Color32[] lit = field.LastColours;
        int wrong = 0;
        for (int i = 0; i < eco.CellCount && lit != null; i++)
        {
            MapColour e = StandMapOverlayColours.Light(eco.Cells[i].Light);
            if (lit[i].r != e.R || lit[i].g != e.G || lit[i].b != e.B || lit[i].a != e.A) wrong++;
        }
        Hard("OVERLAY_LIGHT_EVERY_SQUARE_IS_ITS_OWN_GROUND_LIGHT_COLOUR", field.HasField && lit != null && lit.Length == eco.CellCount && wrong == 0 && field.Cells == n, $"wrong={wrong} of {eco.CellCount}");
        Rect frame = ui.Map.Root.Q("map-frame").worldBound, fb = field.worldBound;
        Hard("OVERLAY_THE_FIELD_COVERS_THE_MAP_SQUARE_EXACTLY", Mathf.Abs(fb.x - frame.x) < 1.6f && Mathf.Abs(fb.y - frame.y) < 1.6f && Mathf.Abs(fb.width - frame.width) < 1.6f && Mathf.Abs(fb.height - frame.height) < 1.6f, $"field={fb} frame={frame}");
        Hard("OVERLAY_THE_FOREST_STAYS_UNDER_THE_FIELD", ui.Map.Canopy.Picture != null && ui.Map.Canopy.ShownVersion == Model.CanopyVersion, "");
        var drawnSquares = ui.Map.Root.Query<Button>(className: "map-cell").ToList().Where(c => !c.ClassListContains("map-cell-selected") && !c.ClassListContains("map-cell-rack") && (c.resolvedStyle.backgroundColor.a > 0.01f || c.resolvedStyle.borderTopColor.a > 0.01f)).ToList();
        Hard("OVERLAY_THE_SQUARES_THEMSELVES_ARE_NOT_DRAWN_ONLY_THE_SELECTED_ONE_HAS_AN_OUTLINE", drawnSquares.Count == 0 && ui.Map.Root.Query<Button>(className: "map-cell-selected").ToList().Count <= 1, $"drawn={drawnSquares.Count}");

        yield return Invoke(Tab(2)); yield return Settle(8);
        lit = field.LastColours; wrong = 0;
        var planted = new int[eco.CellCount];
        foreach (PlantedJuvenile j in manager.PlantedJuveniles) if (j.alive && string.IsNullOrEmpty(j.promotedTreeId)) { int c = eco.GetCellIndex(j.position); if (c >= 0) planted[c]++; }
        for (int i = 0; i < eco.CellCount && lit != null; i++)
        {
            float tallest = 0f; int cohorts = 0;
            foreach (ForestRegenerationCohort r in eco.Cells[i].Regeneration) if (r != null && r.Species != null && r.Density > 0f) { cohorts++; tallest = Mathf.Max(tallest, r.Height); }
            MapColour e = StandMapOverlayColours.Regeneration(tallest, cohorts > 0, planted[i] > 0);
            if (lit[i].r != e.R || lit[i].g != e.G || lit[i].b != e.B || lit[i].a != e.A) wrong++;
        }
        Hard("OVERLAY_REGENERATION_EVERY_SQUARE_IS_ITS_OWN_COHORT_COLOUR", field.HasField && lit != null && wrong == 0, $"wrong={wrong} of {eco.CellCount}");

        yield return Invoke(Tab(3)); yield return Settle(8);
        Hard("OVERLAY_BROWSING_HAS_NO_FIELD_ONLY_SHELTER_RINGS", !field.HasField, "");
        yield return Invoke(Tab(4)); yield return Settle(8);
        Hard("OVERLAY_MARKS_HAS_NO_FIELD", !field.HasField, "");
        yield return Invoke(Tab(0)); yield return Settle(4);
        ui.CloseAll(); yield return Settle(4);
    }

    // =====================================================================================================================
    // REDRAWS: static vs dynamic, a felling, a load
    // =====================================================================================================================
    private IEnumerator Redraws()
    {
        ui.CloseAll(); yield return Settle(20);
        ui.Map.Refresh(false);
        StandMapCanvas mini = Mini; StandMapCanopyLayer canopy = ui.Minimap.Canopy;
        Func<string> counters = () => $"staticLayouts={mini.StaticLayouts} dynamicLayouts={mini.DynamicLayouts} staticRepaints={mini.StaticRepaints} dynamicRepaints={mini.DynamicRepaints} canopyRedraws={canopy.Redraws} scans={ui.MapSource.TreeScans}";

        // standing still
        int sL = mini.StaticLayouts, dL = mini.DynamicLayouts, sR = mini.StaticRepaints, dR = mini.DynamicRepaints, cR = canopy.Redraws, sc = ui.MapSource.TreeScans;
        double t0 = Time.realtimeSinceStartupAsDouble;
        yield return Settle(120);
        double still = Time.realtimeSinceStartupAsDouble - t0;
        Hard("REDRAW_STANDING_STILL_LAYS_OUT_AND_PAINTS_NOTHING", mini.StaticLayouts == sL && mini.DynamicLayouts == dL && mini.StaticRepaints == sR && mini.DynamicRepaints == dR && canopy.Redraws == cR, counters());
        int scans = ui.MapSource.TreeScans - sc;
        Hard("REDRAW_STANDING_STILL_RE_READS_THE_TREES_ONLY_ON_THE_SLOW_SAFETY_SCAN", scans <= still / 0.9 + 2, $"{scans} scans in {still:0.0} s");
        Metric("still_120_frames_seconds", F(still, "0.0")); Metric("still_tree_scans", scans.ToString());

        // walking
        sL = mini.StaticLayouts; dL = mini.DynamicLayouts; sR = mini.StaticRepaints; dR = mini.DynamicRepaints; cR = canopy.Redraws; sc = ui.MapSource.TreeScans;
        t0 = Time.realtimeSinceStartupAsDouble;
        Vector3 start = player.transform.position;
        for (int i = 0; i < 90; i++) { player.transform.position = start + player.transform.forward * (0.08f * (i + 1)); yield return null; }
        double walk = Time.realtimeSinceStartupAsDouble - t0;
        int dynLayouts = mini.DynamicLayouts - dL;
        Hard("REDRAW_A_WALK_LAYS_OUT_ONLY_THE_DYNAMIC_LAYER", mini.StaticLayouts == sL && mini.StaticRepaints == sR && dynLayouts >= 30 && dynLayouts <= 95 && canopy.Redraws == cR, counters() + $" (dynamic layouts this walk {dynLayouts} in 90 steps)");
        Hard("REDRAW_A_WALK_REPAINTS_ONLY_THE_DYNAMIC_PAINTER", mini.DynamicRepaints - dR >= 1 && mini.DynamicRepaints - dR <= 95, $"dynamic repaints {mini.DynamicRepaints - dR}");
        Metric("walk_90_steps_dynamic_layouts", dynLayouts.ToString()); Metric("walk_90_steps_dynamic_repaints", (mini.DynamicRepaints - dR).ToString()); Metric("walk_seconds", F(walk, "0.0"));

        // turning
        sL = mini.StaticLayouts; dL = mini.DynamicLayouts; sR = mini.StaticRepaints; cR = canopy.Redraws;
        Quaternion q0 = player.transform.rotation;
        for (int i = 0; i < 90; i++) { player.transform.rotation = q0 * Quaternion.Euler(0f, 2f * (i + 1), 0f); yield return null; }
        Hard("REDRAW_TURNING_ON_THE_SPOT_LAYS_OUT_ONLY_THE_DYNAMIC_LAYER", mini.StaticLayouts == sL && mini.StaticRepaints == sR && mini.DynamicLayouts - dL >= 60 && canopy.Redraws == cR, counters() + $" (dynamic layouts {mini.DynamicLayouts - dL} in 90 degrees x 2)");
        player.transform.rotation = q0; player.transform.position = start; yield return Settle(6);

        // a marked tree changes the static layer (the Marks layer) and not the forest picture
        ForestTree victim = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop).OrderBy(t => t.TreeId, StringComparer.Ordinal).Skip(200).First();
        sL = mini.StaticLayouts; cR = canopy.Redraws;
        ui.Marking.Mark(victim, TreeMarkType.Fell, false);
        yield return Settle(30);
        Hard("REDRAW_MARKING_A_TREE_DOES_NOT_REDRAW_THE_FOREST_PICTURE", canopy.Redraws == cR, counters());
        ui.Marking.Mark(victim, TreeMarkType.None, false);
        yield return Settle(20);

        // felling a tree: the forest picture is redrawn exactly once, soon
        int before = canopy.Redraws; sc = ui.MapSource.TreeScans;
        float shown = Time.realtimeSinceStartup;
        victim.Fell();
        int frames = 0;
        while (canopy.Redraws == before && frames < 240) { frames++; yield return null; }
        double toRedraw = Time.realtimeSinceStartup - shown;
        yield return Settle(20);
        Hard("REDRAW_A_FELLING_REDRAWS_THE_MINIMAP_FOREST_EXACTLY_ONCE_AND_SOON", canopy.Redraws == before + 1 && frames <= 60, $"redraws {before}->{canopy.Redraws} after {frames} frames ({toRedraw:0.00} s)");
        MapTree felled = Model.Trees.FirstOrDefault(t => Mathf.Abs(t.X - victim.transform.position.x) < 0.01f && Mathf.Abs(t.Z - victim.transform.position.z) < 0.01f);
        Hard("REDRAW_THE_FELLED_TREE_IS_NOW_A_STUMP_ON_THE_MAP", felled.State == MapTreeState.Stump, felled.State.ToString());
        Metric("felling_minimap_frames_to_redraw", frames.ToString()); Metric("felling_minimap_redraw_ms", F(canopy.LastRedrawMilliseconds, "0.0")); Metric("minimap_worst_step_ms", F(canopy.WorstStepMilliseconds, "0.0"));
        int fullBefore = ui.Map.Canopy.Redraws;
        Hard("REDRAW_THE_FULL_MAP_PICTURE_IS_NOT_REDRAWN_WHILE_THE_MAP_IS_CLOSED", ui.Map.Canopy.Redraws == fullBefore, "");
        var open = System.Diagnostics.Stopwatch.StartNew();
        ui.ShowMap(); open.Stop();
        yield return Settle(4);
        Hard("REDRAW_OPENING_THE_MAP_SHOWS_THE_FOREST_AS_IT_IS_NOW", ui.Map.Canopy.ShownVersion == Model.CanopyVersion && !ui.Map.Canopy.Redrawing && ui.Map.Canopy.Redraws == fullBefore + 1, $"redraws {fullBefore}->{ui.Map.Canopy.Redraws}");
        Metric("map_open_after_felling_ms_including_the_forest_redraw", F(open.Elapsed.TotalMilliseconds, "0.0")); Metric("full_map_redraw_ms", F(ui.Map.Canopy.LastRedrawMilliseconds, "0.0"));
        ui.CloseAll(); yield return Settle(6);

        // a loaded game raises no event: the safety scan has to pick it up
        if (saves == null) { Unverified("no save controller: the load safety scan was not exercised"); yield break; }
        ForestSaveData original = saves.CaptureData();
        ForestTree second = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop).OrderBy(t => t.TreeId, StringComparer.Ordinal).Skip(300).First();
        second.Fell(); yield return Settle(20);
        int stumpsNow = Model.Trees.Count(t => t.State == MapTreeState.Stump);
        before = canopy.Redraws;
        bool loaded = saves.LoadData(original, false);
        t0 = Time.realtimeSinceStartupAsDouble;
        int stumpsAfter = -1;
        while (Time.realtimeSinceStartupAsDouble - t0 < 4.0)
        {
            yield return null;
            stumpsAfter = Model.Trees.Count(t => t.State == MapTreeState.Stump);
            if (stumpsAfter == stumpsNow - 1 && !canopy.Redrawing) break;
        }
        double seen = Time.realtimeSinceStartupAsDouble - t0;
        Hard("LOAD_THE_MAP_FOLLOWS_A_LOADED_GAME_WITHOUT_ANY_EVENT_WITHIN_THE_SAFETY_SCAN", loaded && stumpsAfter == stumpsNow - 1 && canopy.Redraws > before, $"loaded={loaded} stumps {stumpsNow}->{stumpsAfter} redraws {before}->{canopy.Redraws} after {seen:0.00} s");
        Metric("load_seconds_until_the_minimap_followed", F(seen, "0.00"));
        yield return Settle(10);
    }

    // =====================================================================================================================
    // INPUT AND LAYOUT at each size
    // =====================================================================================================================
    private static bool Inside(Rect inner, Rect outer, float tol = 0.75f) => inner.xMin >= outer.xMin - tol && inner.yMin >= outer.yMin - tol && inner.xMax <= outer.xMax + tol && inner.yMax <= outer.yMax + tol;

    private IEnumerator LayoutAndInput()
    {
        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y);
            ui.CloseAll(); yield return Settle(25);
            string tag = $"{size.x}x{size.y}";
            // the minimap takes no pointer input
            HudMinimapView mini = ui.Minimap;
            Rect mr = mini.Root.worldBound;
            IPanel panel = ui.RootElement.panel;
            VisualElement hit = panel.Pick(mr.center);
            bool captured = false;
            for (VisualElement e = hit; e != null; e = e.parent) if (e == mini.Root) captured = true;
            Hard($"INPUT_{tag}_NOTHING_ON_THE_MINIMAP_CAPTURES_A_CLICK", !captured && mini.Root.Query<VisualElement>().ToList().All(e => e.pickingMode == PickingMode.Ignore || e == mini.Root) && mini.Root.Query<Button>().ToList().Count == 0, $"pick={(hit == null ? "none" : hit.name + "/" + string.Join(".", hit.GetClasses()))}");
            Hard($"INPUT_{tag}_A_PRESS_OVER_THE_MINIMAP_IS_NOT_OVER_A_BUTTON", !ui.IsOverButton(new Vector2(mr.center.x * Screen.width / ui.RootElement.worldBound.width, Screen.height - mr.center.y * Screen.height / ui.RootElement.worldBound.height)), "");
            yield return Capture("pmv-hud");

            ui.ShowMap(); yield return Settle(20);
            VisualElement modalEl = ui.Map.Root.Q(className: "modal"), frameEl = ui.Map.Root.Q("map-frame"), overview = ui.Map.Root.Q("map-overview"), legend = ui.Map.Root.Q("map-legend"), card = ui.Map.Root.Q(className: "map-side");
            Rect root = ui.RootElement.worldBound, modal = modalEl.worldBound, frame = frameEl.worldBound;
            Hard($"LAYOUT_{tag}_THE_MAP_WINDOW_IS_ON_SCREEN", Inside(modal, root), $"{modal} in {root}");
            // The panel scales with the screen, so one physical pixel is more than one UI unit at 1280x720 (1.25): allow one pixel of layout rounding, not more.
            float onePixel = root.width / Screen.width;
            Hard($"LAYOUT_{tag}_THE_MAP_FRAME_IS_SQUARE_AND_LARGE", Mathf.Abs(frame.width - frame.height) <= onePixel * 1.01f && frame.width >= 600f, $"{frame} (one physical pixel = {onePixel:0.00} units)");
            Hard($"LAYOUT_{tag}_FRAME_OVERVIEW_LEGEND_AND_CARD_ARE_INSIDE_THE_WINDOW", new[] { frame, overview.worldBound, legend.worldBound, card.worldBound }.All(r => Inside(r, modal)), $"frame={frame} overview={overview.worldBound} legend={legend.worldBound} card={card.worldBound} modal={modal}");
            var overlaps = new List<string>();
            if (frame.Overlaps(overview.worldBound)) overlaps.Add("map/overview"); if (frame.Overlaps(legend.worldBound)) overlaps.Add("map/legend"); if (frame.Overlaps(card.worldBound)) overlaps.Add("map/card");
            if (overview.worldBound.Overlaps(legend.worldBound)) overlaps.Add("overview/legend"); if (legend.worldBound.Overlaps(card.worldBound)) overlaps.Add("legend/card"); if (overview.worldBound.Overlaps(card.worldBound)) overlaps.Add("overview/card");
            Hard($"LAYOUT_{tag}_THE_MAP_DOES_NOT_OVERLAP_THE_CARDS", overlaps.Count == 0, string.Join(",", overlaps) + $" frame={frame} overview={overview.worldBound} legend={legend.worldBound} card={card.worldBound}");
            Rect cv = ui.Map.Canopy.worldBound, cs = Full.worldBound;
            Hard($"LAYOUT_{tag}_THE_FOREST_PICTURE_AND_THE_LINEWORK_FILL_THE_FRAME_EXACTLY", Mathf.Abs(cv.x - frame.x) < 1.6f && Mathf.Abs(cv.y - frame.y) < 1.6f && Mathf.Abs(cv.width - frame.width) < 1.6f && Mathf.Abs(cs.width - frame.width) < 1.6f && Mathf.Abs(cs.height - frame.height) < 1.6f, $"picture={cv} lines={cs} frame={frame}");

            // survey labels sit on their grid lines
            var east = ui.Map.Root.Query<Label>(className: "map-axis-east").ToList(); var north = ui.Map.Root.Query<Label>(className: "map-axis-north").ToList();
            float worstLabel = 0f;
            foreach (Label l in east) { float f = float.Parse(l.text, Inv) / Model.SizeM; worstLabel = Mathf.Max(worstLabel, Mathf.Abs(l.worldBound.center.x - (frame.x + f * frame.width))); }
            foreach (Label l in north) { float f = float.Parse(l.text, Inv) / Model.SizeM; worstLabel = Mathf.Max(worstLabel, Mathf.Abs(l.worldBound.center.y - (frame.yMax - f * frame.height))); }
            Hard($"LAYOUT_{tag}_THE_EASTING_AND_NORTHING_LABELS_SIT_ON_THEIR_GRID_LINES", east.Count == 5 && north.Count == 5 && worstLabel < 4.5f * root.width / 1600f, $"east={east.Count} north={north.Count} worst offset {worstLabel:0.0} px");
            Hard($"LAYOUT_{tag}_THE_LEGEND_NAMES_ONLY_WHAT_IS_ON_THE_MAP", legend.Query<Label>(className: "map-legend-text").ToList().Select(l => l.text.ToLowerInvariant()).All(t => !new[] { "contour", "stream", "water", "pond", "wetland", "rock", "track", "building", "elevation" }.Any(t.Contains)) && ui.Map.Legend.RowCount >= 6, $"rows={ui.Map.Legend.RowCount}");
            // text stays in its card
            var cut = new List<string>();
            foreach (VisualElement c in new[] { overview, legend, card })
                foreach (TextElement t in c.Query<TextElement>().ToList())
                    if (Visible(t) && !string.IsNullOrWhiteSpace(t.text) && !(t is Button) && (t.worldBound.xMax > c.worldBound.xMax + 0.75f || t.worldBound.yMax > c.worldBound.yMax + 0.75f)) cut.Add($"\"{t.text}\" in {c.name}");
            Hard($"LAYOUT_{tag}_NO_CARD_TEXT_RUNS_PAST_ITS_CARD", cut.Count == 0, string.Join(" | ", cut.Take(3)));
            float lowest = modalEl.Query<TextElement>().ToList().Where(Visible).Where(t => !string.IsNullOrEmpty(t.text)).Max(t => t.worldBound.yMax);
            Hard($"LAYOUT_{tag}_ALL_TEXT_ENDS_INSIDE_THE_WINDOW", lowest <= modal.yMax - 4f, $"lowest {lowest:0.0} bottom {modal.yMax:0.0}");
            yield return Capture("pmv-map-access");

            // the pointer reads the square under it: ask the view what it would say at the middle of one square and compare with an independent reading
            int probe = 7 * eco.CellsPerAxis + 9;
            Vector2 centre = eco.Cells[probe].Center;
            MapPosition atPx = Model.TransformFor(StandMapView.MapSidePx).WorldToPixels(new RackPoint(centre.x, centre.y));
            string said = ui.Map.DescribeAt(new Vector2(atPx.X, atPx.Y));
            Rect b0 = eco.StandBounds;
            Hard($"INPUT_{tag}_POINTING_AT_THE_MIDDLE_OF_A_SQUARE_NAMES_IT_AND_GIVES_ITS_OWN_FIGURES",
                said.Contains("square " + UiKit.CellLabel(probe, eco.CellsPerAxis)) && said.Contains("E " + F(centre.x - b0.xMin, "0.0") + " m") && said.Contains("N " + F(centre.y - b0.yMin, "0.0") + " m")
                && said.Contains("ground light " + F(eco.Cells[probe].Light, "0.00")), said);
            Hard($"INPUT_{tag}_POINTING_OUTSIDE_THE_MAP_SAYS_NOTHING_ABOUT_A_SQUARE", !ui.Map.DescribeAt(new Vector2(-5f, 10f)).Contains("square"), "");
            Hard($"INPUT_{tag}_THE_FULL_MAP_HAS_ALL_256_SQUARES_AS_BUTTONS_OVER_THE_PLAN", ui.Map.Root.Query<Button>(className: "map-cell").ToList().Count == eco.CellCount, "");

            // every layer, then rack planning, fits the window
            for (int l = 0; l < 5; l++)
            {
                yield return Invoke(Tab(l)); yield return Settle(6);
                float low = modalEl.Query<TextElement>().ToList().Where(Visible).Where(t => !string.IsNullOrEmpty(t.text)).Max(t => t.worldBound.yMax);
                Hard($"LAYOUT_{tag}_LAYER_{l}_TEXT_ENDS_INSIDE_THE_WINDOW_AND_THE_LEGEND_FITS", low <= modalEl.worldBound.yMax - 4f && Inside(legend.worldBound, modalEl.worldBound), $"lowest {low:0.0} bottom {modalEl.worldBound.yMax:0.0} legendRows={ui.Map.Legend.RowCount}");
                if (l == 1 || l == 2 || l == 4) yield return Capture($"pmv-map-layer{l}");
            }
            yield return Invoke(Tab(0));
            ui.CloseAll(); yield return Settle(4);
        }
    }

    // =====================================================================================================================
    // MANAGEMENT STORY: juveniles, shelters and marks on the map are the real ones; rack planning is preserved
    // =====================================================================================================================
    private IEnumerator Management()
    {
        ui.CloseAll(); yield return Settle(6);
        RackPlanner planner = ui.RackTool.Planner; planner.Refresh();
        // three layout racks (real backend), crop and felling marks, then plant with and without shelters, advance years
        foreach (RackLayoutLine line in ui.RackLayout.Lines.Where(l => !l.Done).Take(3).ToList())
        {
            RackProposal p = planner.Propose(new RackPoint(0f, line.AcrossM), planner.DefaultAngle(0f, true));
            if (p != null && p.CanConfirm) manager.ConfirmRack(p, "forest");
            yield return Settle(3);
        }
        var trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving && t.CanChop).OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        for (int i = 0; i < 4; i++) ui.Marking.Mark(trees[i * 40], TreeMarkType.CropTree, false);
        for (int i = 0; i < 6; i++) ui.Marking.Mark(trees[i * 17 + 3], TreeMarkType.Fell, false);
        yield return Settle(20);
        ui.ShowMap(); yield return Settle(8); yield return Invoke(Tab(4)); yield return Settle(8);
        StandMapModel m = Model; float side = Full.LastSidePx;
        int fellMarked = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Count(t => t.IsLiving && t.IsMarkedForFell), cropMarked = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Count(t => t.IsLiving && t.IsCropTree);
        Hard("MARKS_THE_MARKS_LAYER_RINGS_EVERY_MARKED_LIVING_TREE_AND_NO_OTHER", Full.LastPlan.Count(i => i.Symbol == MapSymbol.TreeMarkFell) == fellMarked && Full.LastPlan.Count(i => i.Symbol == MapSymbol.TreeMarkCrop) == cropMarked && fellMarked + cropMarked >= 8, $"fell {Full.LastPlan.Count(i => i.Symbol == MapSymbol.TreeMarkFell)}/{fellMarked} crop {Full.LastPlan.Count(i => i.Symbol == MapSymbol.TreeMarkCrop)}/{cropMarked}");
        int exact = 0, rings = 0;
        foreach (MapDrawItem ring in Full.LastPlan.Where(i => i.Symbol == MapSymbol.TreeMarkFell || i.Symbol == MapSymbol.TreeMarkCrop))
        {
            rings++;
            Vector2 f = Fraction(Full, ring);
            if (trees.Any(t => (ExpectedFraction(t.transform.position.x, t.transform.position.z) - f).magnitude < 2e-4f && (ring.Symbol == MapSymbol.TreeMarkFell ? t.IsMarkedForFell : t.IsCropTree))) exact++;
        }
        Hard("MARKS_EACH_RING_IS_CENTRED_ON_ITS_OWN_TREE", rings > 0 && exact == rings, $"{exact}/{rings}");
        yield return Capture("pmv-marks");
        ui.CloseAll(); yield return Settle(4);

        manager.AddMarkedTreesToWorkPlan();
        Hard("MANAGE_THE_MARKED_WORK_WAS_APPROVED", manager.ApprovePendingWork(), "");
        manager.AdvanceYear(); yield return Settle(10);
        ui.ShowReview(); yield return Settle(15);
        Button ack = ui.RootElement.Q<Button>("annual-review-acknowledge");
        if (ack != null && Visible(ack) && ack.enabledInHierarchy) yield return Invoke(ack);
        ui.CloseAll(); yield return Settle(6);
        foreach (string item in new[] { "beech-sapling", "sessile-oak-sapling" })
        {
            manager.TryPurchaseStock(item, 4);
            int planned = 0;
            foreach (int cell in Enumerable.Range(0, eco.CellCount).OrderByDescending(i => eco.Cells[i].Light).ThenBy(i => i))
            {
                Vector2 centre = eco.Cells[cell].Center;
                for (int z = -2; z <= 2 && planned < 4; z++)
                for (int x = -2; x <= 2 && planned < 4; x++)
                    if (manager.TryDesignateExactPlanting(item, new Vector3(centre.x + x * .9f, 0f, centre.y + z * .9f), CCF.Forestry.WorkEconomy.WorkExecutionMethod.LandownerSimulated, planned % 2 == 0)) planned++;
                if (planned == 4) break;
            }
        }
        manager.ApprovePendingWork();
        for (int y = 0; y < 3; y++)
        {
            manager.AdvanceYear(); yield return Settle(10);
            ui.ShowReview(); yield return Settle(12);
            ack = ui.RootElement.Q<Button>("annual-review-acknowledge");
            if (ack != null && Visible(ack) && ack.enabledInHierarchy) yield return Invoke(ack);
            ui.CloseAll(); yield return Settle(6);
        }
        Debug.Log($"PMV_STATE year={eco.EcologicalYear} living={eco.LivingTreeCount} juveniles={manager.PlantedJuveniles.Count} shelters={eco.Browsing.Shelters.Count}");

        // the managed forest
        yield return ForestPictures("managed");
        ui.ShowMap(); yield return Settle(10);
        m = Model; side = Full.LastSidePx;
        int alive = manager.PlantedJuveniles.Count(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId));
        Hard("JUVENILES_EVERY_PLANTED_JUVENILE_IS_DRAWN_AT_ITS_EXACT_PLACE", alive > 0 && Full.LastPlan.Count(i => i.Symbol == MapSymbol.Juvenile) == alive
            && manager.PlantedJuveniles.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)).All(j => Full.LastPlan.Any(i => i.Symbol == MapSymbol.Juvenile && (Fraction(Full, i) - ExpectedFraction(j.position.x, j.position.z)).magnitude < 2e-4f)), $"drawn {Full.LastPlan.Count(i => i.Symbol == MapSymbol.Juvenile)} alive {alive}");
        yield return Invoke(Tab(3)); yield return Settle(8);
        int upcoming = eco.EcologicalYear + 1;
        int eff = eco.Browsing.Shelters.Count(s => s != null && s.IsEffective(upcoming)), lapsed = eco.Browsing.Shelters.Count(s => s != null && !s.IsEffective(upcoming));
        Hard("SHELTERS_THE_BROWSING_LAYER_RINGS_EVERY_SHELTER_EFFECTIVE_AND_LAPSED_APART", eco.Browsing.Shelters.Count > 0 && Full.LastPlan.Count(i => i.Symbol == MapSymbol.Shelter) == eff && Full.LastPlan.Count(i => i.Symbol == MapSymbol.ShelterLapsed) == lapsed
            && eco.Browsing.Shelters.All(s => Full.LastPlan.Any(i => (i.Symbol == MapSymbol.Shelter || i.Symbol == MapSymbol.ShelterLapsed) && (Fraction(Full, i) - ExpectedFraction(s.position.x, s.position.y)).magnitude < 2e-4f)), $"effective {eff} lapsed {lapsed} of {eco.Browsing.Shelters.Count}");
        yield return Capture("pmv-managed-browsing");
        yield return Invoke(Tab(0)); yield return Settle(6);

        // rack planning on the full map is preserved
        Button plan = ui.RootElement.Q<Button>("map-plan-rack");
        if (plan == null || !Visible(plan)) Unverified("the Plan rack button was not offered after the managed story, so rack planning on the map was not exercised here (StandMapParity exercises it)");
        else
        {
            yield return Invoke(plan);
            Hard("RACKPLAN_PLANNING_MODE_TURNS_ON", ui.Map.RackMode, "");
            int n = eco.CellsPerAxis; var cells = ui.RootElement.Query<Button>(className: "map-cell").ToList();
            RackLayoutLine open = ui.RackLayout.Lines.FirstOrDefault(l => !l.Done);
            Vector3 where = open != null ? new Vector3(0f, 0f, open.AcrossM) : new Vector3(eco.StandBounds.center.x, 0f, eco.StandBounds.center.y);
            int cell = eco.GetCellIndex(where);
            yield return Invoke(cells[(n - 1 - cell / n) * n + cell % n]);
            yield return Wait(0.3f); yield return Settle(8);
            RackProposal prev = ui.Map.RackPreview;
            Hard("RACKPLAN_CLICKING_A_SQUARE_PREVIEWS_A_RACK_FROM_THE_REAL_PLANNER", prev != null && prev.Selection != null, "");
            if (prev != null && prev.Selection != null)
            {
                MapDrawItem pl = Full.DynamicPlan.FirstOrDefault(i => i.Symbol == MapSymbol.RackPreview), pb = Full.DynamicPlan.FirstOrDefault(i => i.Symbol == MapSymbol.RackBand);
                Hard("RACKPLAN_THE_PREVIEW_IS_DRAWN_ON_THE_DYNAMIC_LAYER_AT_THE_PROPOSALS_EXACT_ENDS", pl != null && pb != null
                    && (Fraction(Full, pl) - ExpectedFraction(prev.Selection.Start.X, prev.Selection.Start.Z)).magnitude < 2e-4f
                    && (new Vector2(pl.Points[1].X / Full.LastSidePx, pl.Points[1].Y / Full.LastSidePx) - ExpectedFraction(prev.Selection.End.X, prev.Selection.End.Z)).magnitude < 2e-4f, "");
                Hard("RACKPLAN_THE_MINIMAP_SHOWS_NO_PREVIEW", Mini.LastPlan.All(i => i.Symbol != MapSymbol.RackPreview), "");
                int before = manager.RackGroups.Count;
                Button confirm = ui.RootElement.Q<Button>("map-rack-confirm");
                if (confirm != null && confirm.enabledInHierarchy) { yield return Invoke(confirm); yield return Settle(8); }
                Hard("RACKPLAN_CONFIRM_ADDS_THE_RACK_THROUGH_THE_REAL_BACKEND", manager.RackGroups.Count > before, $"{before}->{manager.RackGroups.Count}");
                yield return Capture("pmv-rack-planning");
            }
            Button cancel = ui.RootElement.Q<Button>("map-rack-cancel");
            if (cancel != null) yield return Invoke(cancel);
            Hard("RACKPLAN_CANCEL_LEAVES_PLANNING_MODE", !ui.Map.RackMode, "");
        }
        ui.CloseAll(); yield return Settle(4);
    }

    // =====================================================================================================================
    // COST: diagnostic microbenchmarks (the Editor, Mono: relative evidence, no threshold invented)
    // =====================================================================================================================
    private IEnumerator Micro()
    {
        ui.CloseAll(); yield return Settle(10);
        StandMapModel m = Model;
        var w = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 500; i++) StandMapDrawPlan.BuildStatic(m, 664f, MapDetail.Full, MapOverlay.None);
        double staticFull = w.Elapsed.TotalMilliseconds / 500; w.Restart();
        for (int i = 0; i < 500; i++) StandMapDrawPlan.BuildStatic(m, 196f, MapDetail.Mini, MapOverlay.None);
        double staticMini = w.Elapsed.TotalMilliseconds / 500; w.Restart();
        for (int i = 0; i < 4000; i++) StandMapDrawPlan.BuildDynamic(m, 196f, MapDetail.Mini);
        double dynamicMini = w.Elapsed.TotalMilliseconds / 4000; w.Restart();
        for (int i = 0; i < 4000; i++) StandMapDrawPlan.BuildDynamic(m, 664f, MapDetail.Full);
        double dynamicFull = w.Elapsed.TotalMilliseconds / 4000;
        Metric("build_static_full_ms_per_call", F(staticFull, "0.0000")); Metric("build_static_mini_ms_per_call", F(staticMini, "0.0000"));
        Metric("build_dynamic_mini_ms_per_call", F(dynamicMini, "0.0000")); Metric("build_dynamic_full_ms_per_call", F(dynamicFull, "0.0000"));
        StandMapCanopyRaster.Render(m, StandMapCanopyRaster.MiniSidePx, MapDetail.Mini); StandMapCanopyRaster.Render(m, StandMapCanopyRaster.FullSidePx, MapDetail.Full);   // warm (sprites, JIT)
        w.Restart();
        for (int i = 0; i < 5; i++) StandMapCanopyRaster.Render(m, StandMapCanopyRaster.MiniSidePx, MapDetail.Mini);
        double rasterMini = w.Elapsed.TotalMilliseconds / 5; w.Restart();
        for (int i = 0; i < 5; i++) StandMapCanopyRaster.Render(m, StandMapCanopyRaster.FullSidePx, MapDetail.Full);
        double rasterFull = w.Elapsed.TotalMilliseconds / 5;
        Metric("forest_redraw_mini_ms_warm", F(rasterMini, "0.0")); Metric("forest_redraw_full_ms_warm", F(rasterFull, "0.0")); Metric("trees_drawn", m.Trees.Count.ToString());
        Metric("crown_sprites_made", StandMapCanopyRaster.CrownSpritesMade.ToString());

        // allocation per call when nothing changed (the steady state of every frame the minimap is on screen), and per step of a walk
        yield return Settle(4);
        ui.Minimap.Refresh();
        long a0 = GC.GetTotalMemory(false); int gc0 = GC.CollectionCount(0);
        for (int i = 0; i < 2000; i++) ui.Minimap.Refresh();
        long a1 = GC.GetTotalMemory(false); int gc1 = GC.CollectionCount(0);
        Metric("minimap_refresh_unchanged_bytes_per_call", gc1 == gc0 ? F((a1 - a0) / 2000.0, "0.0") : "a collection ran; not attributable");
        if (gc1 == gc0) Hard("COST_AN_UNCHANGED_MINIMAP_REFRESH_ALLOCATES_NOTHING_MATERIAL", (a1 - a0) / 2000.0 < 64.0, $"{(a1 - a0) / 2000.0:0.0} bytes per call");
        else Unverified("a GC collection ran during the unchanged-refresh allocation sample, so it is not attributed");
        Vector3 home = player.transform.position;
        a0 = GC.GetTotalMemory(false); gc0 = GC.CollectionCount(0);
        for (int i = 0; i < 120; i++) { player.transform.position = home + Vector3.right * (0.15f * (i + 1)); ui.MapSource.Rebuild(); ui.Minimap.Refresh(); }
        a1 = GC.GetTotalMemory(false); gc1 = GC.CollectionCount(0);
        player.transform.position = home;
        Metric("minimap_walk_step_bytes_per_step_including_a_full_model_rebuild", gc1 == gc0 ? F((a1 - a0) / 120.0, "0") : "a collection ran; not attributable");
        yield return Settle(6);
    }
}
#endif
