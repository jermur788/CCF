#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// CCF #359 verification-only probe, session 2: gate H (old-world fail-close) in a fresh Legacy40 game (CCF_STAND_GEOMETRY=0) and inside a
// frozen Reference Future v1 preview. NEW code; it edits no game file or earlier harness. In these worlds there must be no V2 headland
// surface, no headland on the Stand Map, no minimap guidance, no five-rack figure and no V2 Objective 1 wording. It writes no save and
// does not advance a year. Pre-V2 (model 0) Enlarged80 loads are covered by the session-1 probe.
public static class Ccf359OldWorld
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        Vector2Int first = Ccf359V2ClosureSizes.Get()[0];
        Ccf359V2ClosureSizes.SetGameSize(first.x, first.y);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "Ccf359OldWorld.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "Ccf359OldWorld.Begin"))
            new GameObject("CCF 359 old world").AddComponent<Ccf359OldWorldRunner>();
    }
}

// Separate small copy of the Game-view sizing helper so this file does not depend on the session-1 probe being staged.
public static class Ccf359V2ClosureSizes
{
    public static Vector2Int[] Get()
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "CCF359old " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
    }
}

public sealed class Ccf359OldWorldRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private string output;
    private int passed, shots;
    private readonly List<string> failures = new List<string>(), evidence = new List<string>();
    private Vector2Int size;

    private void Check(string name, bool ok, string detail = "")
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        string line = $"{(ok ? "PASS" : "FAIL")} [{size.x}x{size.y}] {name} {detail}";
        Debug.Log("C359OLD_" + line); evidence.Add(line);
    }
    private void Measure(string name, string detail) { Debug.Log($"C359OLD_MEASURE [{size.x}x{size.y}] {name} {detail}"); evidence.Add($"MEASURE {name} {detail}"); }

    private static IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }
    private static IEnumerator Wait(float seconds) { double until = Time.realtimeSinceStartupAsDouble + seconds; while (Time.realtimeSinceStartupAsDouble < until) yield return null; }
    private static bool Shown(VisualElement e)
    {
        for (VisualElement c = e; c != null; c = c.parent)
            if (c.resolvedStyle.display == DisplayStyle.None || c.resolvedStyle.visibility == Visibility.Hidden) return false;
        return true;
    }
    private static string Texts(VisualElement root) => string.Join("\n", root.Query<TextElement>().ToList().Where(t => Shown(t) && !string.IsNullOrEmpty(t.text)).Select(t => t.text));

    private IEnumerator Shot(string state)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + state);
        File.WriteAllBytes(Path.Combine(output, state + "-" + Screen.width + "x" + Screen.height + ".png"), image.EncodeToPNG());
        Destroy(image); shots++;
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        size = new Vector2Int(Screen.width, Screen.height);
        Exception failure = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Verify());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        if (output != null) File.WriteAllLines(Path.Combine(output, "ccf359-oldworld-evidence.txt"), evidence);
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"CCF359_OLDWORLD_PASS shots={shots} checks={passed}"
            : $"CCF359_OLDWORLD_FAIL shots={shots} checks={passed} exception={failure} failures=[{string.Join(" | ", failures)}]");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    private IEnumerator OldWorld(string label)
    {
        ui.CloseAll(); yield return Wait(1.2f); yield return Frames(6);
        var surfaces = Resources.FindObjectsOfTypeAll<MeshFilter>().Where(m => m != null && m.gameObject.scene.IsValid() && m.gameObject.name == "Scenario One V2 Headland").ToList();
        Check(label + "_V2_IS_NOT_ACTIVE", !manager.RackPlanningV2Active, "geometry=" + ui.Ecology.StandGeometryModelVersion);
        Check(label + "_NO_ACTIVE_V2_HEADLAND_SURFACE", surfaces.All(m => !m.gameObject.activeInHierarchy), "objects=" + surfaces.Count);
        StandMapModel model = ui.MapSource.Model;
        Check(label + "_NO_HEADLAND_AND_NO_GUIDANCE_IN_THE_MAP_MODEL", model.ExistingHeadlandSurfaces.Count == 0 && model.MiniGuidance.Count == 0 && !model.Racks.Any(r => r.Symbol == MapSymbol.RackSuggested),
            $"headland={model.ExistingHeadlandSurfaces.Count} guidance={model.MiniGuidance.Count}");
        if (ui.Minimap != null && ui.Minimap.Visible)
            Check(label + "_MINIMAP_DRAWS_NO_GUIDANCE_OR_HEADLAND", !ui.Minimap.Canvas.LastPlan.Any(i => i.SourceIndex == -1 && (i.Symbol == MapSymbol.RackSuggested || i.Symbol == MapSymbol.RackBand || i.Symbol == MapSymbol.Path || i.Symbol == MapSymbol.PathCasing)));
        else Measure(label + "_MINIMAP", "not visible in this world/state");
        yield return Shot(label.ToLowerInvariant() + "-hud");
        ui.ShowMap(); yield return Wait(1.2f); yield return Frames(6);
        int strokes = ui.Map.Canvas.LastPlan.Count(i => (i.Symbol == MapSymbol.Path || i.Symbol == MapSymbol.PathCasing) && i.SourceIndex == -1);
        Check(label + "_STAND_MAP_DRAWS_NO_HEADLAND", strokes == 0 && !ui.Map.Canvas.LastPlan.Any(i => i.Symbol == MapSymbol.RackSuggested), "strokes=" + strokes);
        Check(label + "_LEGEND_HAS_NO_V2_HEADLAND_ENTRY", !Texts(ui.Map.Root).Contains("Existing V2 headland"));
        yield return Shot(label.ToLowerInvariant() + "-stand-map");
        ui.CloseAll(); ui.ShowObjectives(); yield return Frames(20);
        TeachingDiagram diagram = ui.Learning.Root.Query<TeachingDiagram>().ToList().FirstOrDefault(Shown);
        var stage = diagram == null ? null : (TeachingDiagramPlan.Stage)typeof(TeachingDiagram).GetField("stage", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(diagram);
        string card = Texts(ui.Learning.Root);
        Measure(label + "_OBJECTIVES", $"objectivesOpen={ui.CurrentScreen} topic={ui.Learning.CurrentTopicIndex} diagram={(stage == null ? "none" : stage.Id)}");
        Check(label + "_NO_FIVE_RACK_FIGURE", stage == null || stage.Id != "access-five-rack", "id=" + stage?.Id);
        if (ui.Learning.CurrentTopicIndex == 0 && stage != null)
            Check(label + "_OBJECTIVE_1_KEEPS_THE_ORIGINAL_PICTURE", stage.Id == "access" && !stage.Items.Any(it => it.Kind == TeachingDiagramPlan.Kind.Corridor), "id=" + stage.Id);
        Check(label + "_NO_FIGURE_TITLE_OR_V2_CONCEPT_WORDING", !card.Contains(TeachingDiagramPlan.FigureTitle) && !card.Contains(LearningObjectivesView.RackV2Concept.Substring(0, 40)));
        yield return Shot(label.ToLowerInvariant() + "-objectives");
        ui.CloseAll(); yield return Frames(4);
    }

    private IEnumerator Verify()
    {
        if (Application.isBatchMode) throw new InvalidOperationException("this probe needs a graphical Editor");
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = FindFirstObjectByType<ScenarioOneManager>();
        if (ui == null || manager == null) throw new InvalidOperationException("not the Scenario One scene");
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        output = Path.Combine(string.IsNullOrEmpty(diag) ? Application.dataPath + "/../Build/Ccf359" : diag, "ccf359-oldworld");
        Directory.CreateDirectory(output);
        yield return Wait(0.5f);
        if (ui.Help.Onboarding.IsOpen) { for (int i = 0; i < 3 && ui.Help.Onboarding.IsOpen; i++) { ui.Help.Onboarding.Next(); yield return Frames(3); } }
        if (ui.Help.IsOpen) ui.CloseHelp();
        Check("WORLD_IS_A_FRESH_LEGACY40_GAME", ui.Ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40 && manager.CurrentEcologicalYear == 0,
            "geometry=" + ui.Ecology.StandGeometryModelVersion + " year=" + manager.CurrentEcologicalYear);
        int index = 0;
        foreach (Vector2Int s in Ccf359V2ClosureSizes.Get())
        {
            if (index++ > 0) { Ccf359V2ClosureSizes.SetGameSize(s.x, s.y); yield return Wait(1.5f); }
            size = new Vector2Int(Screen.width, Screen.height);
            yield return OldWorld("LEGACY40");
        }
        Ccf359V2ClosureSizes.SetGameSize(Ccf359V2ClosureSizes.Get()[0].x, Ccf359V2ClosureSizes.Get()[0].y); yield return Wait(1.5f);
        size = new Vector2Int(Screen.width, Screen.height);
        bool began = manager.TryBeginReferencePreview(50);
        Check("FROZEN_REFERENCE_FUTURE_V1_PREVIEW_OPENS", began, "feedback=" + (typeof(ScenarioOneManager).GetField("feedback", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(manager) ?? ""));
        if (!began) yield break;
        yield return Wait(1.0f);
        Check("PREVIEW_ACTIVE", manager.ReferencePreviewActive);
        yield return OldWorld("REFERENCE_V1_PREVIEW_YEAR50");
        manager.EndReferencePreview(); yield return Wait(1.0f);
        Check("PREVIEW_ENDED_BACK_IN_THE_LEGACY40_WORLD", !manager.ReferencePreviewActive && ui.Ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40 && manager.CurrentEcologicalYear == 0);
        yield return OldWorld("LEGACY40_AFTER_PREVIEW");
    }
}
#endif
