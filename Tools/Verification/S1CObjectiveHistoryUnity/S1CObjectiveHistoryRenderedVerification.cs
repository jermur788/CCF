#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// Supplemental rendered evidence: the first harness scrolls only the OBJECTIVES heading into view.
// Scroll the whole objectives card and assert the actual history lines are in the viewport before capturing.
// Uses silent typed loading through the production controller so the transient load toast cannot cover the review.
public static class S1CObjectiveHistoryRenderedVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        S1CObjectiveHistoryUnityRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("S1CObjectiveHistoryRenderedVerification.Begin"))
            new GameObject("T05 supplemental rendered evidence").AddComponent<S1CObjectiveHistoryRenderedRunner>();
    }
}
public sealed class S1CObjectiveHistoryRenderedRunner : MonoBehaviour
{
    private readonly Dictionary<string, byte[]> originals = new Dictionary<string, byte[]>();
    private readonly List<string> checks = new List<string>();
    private string output;
    private int captures;
    private void Check(bool ok, string label)
    {
        checks.Add((ok ? "PASS " : "FAIL ") + label);
        Debug.Log((ok ? "T05_RENDER_CHECK_PASS " : "T05_RENDER_CHECK_FAIL ") + label);
        if (!ok) throw new InvalidOperationException(label);
    }
    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        Directory.CreateDirectory(Path.Combine(output, "shots"));
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
        foreach (var pair in originals)
            if (pair.Value == null) File.Delete(pair.Key); else File.WriteAllBytes(pair.Key, pair.Value);
        File.WriteAllText(Path.Combine(output, "render-result.txt"), string.Join("\n", checks) + "\nexception=" + failure + "\ncaptures=" + captures);
        Debug.Log(failure == null ? "T05_RENDER_PASS captures=" + captures : "T05_RENDER_FAIL " + failure);
        S1CObjectiveHistoryUnityVerification.Finish(failure == null);
    }
    private static IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }
    private IEnumerator Run()
    {
        StandaloneSessionMenu menu = null;
        for (int f = 0; f < 900 && (menu == null || !StandaloneSessionMenu.IsOpen); f++)
        { menu = FindFirstObjectByType<StandaloneSessionMenu>(); yield return null; }
        Check(menu != null && StandaloneSessionMenu.IsOpen, "startup ready");
        string path = Path.Combine(Application.persistentDataPath, "forest-save.json");
        Check(path.StartsWith(Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")), "isolated save path");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        foreach (string p in new[] { path, path + ".bak", path + ".tmp" }) originals[p] = File.Exists(p) ? File.ReadAllBytes(p) : null;
        menu.StartNewScenario();
        ScenarioOneUiRoot ui = null;
        for (int f = 0; f < 600 && (ui == null || ui.Help == null || !ui.Help.IsOpen); f++)
        { ui = FindFirstObjectByType<ScenarioOneUiRoot>(); yield return null; }
        Check(ui != null && ui.Help != null, "UI ready"); ui.CloseHelp(); yield return Frames(20);
        var saves = FindFirstObjectByType<ForestSaveController>();
        var player = FindFirstObjectByType<ForestPlayer>(); player.enabled = false;
        string fixtures = Environment.GetEnvironmentVariable("CCF_T05_FIXTURES");
        string earlier = Environment.GetEnvironmentVariable("CCF_T05_PRIOR_EVIDENCE");
        var states = new[] {
            (name: "recorded", file: Path.Combine(earlier, "recorded-v22-year3-save.json"), phrase: "(recorded)"),
            (name: "derived", file: Path.Combine(fixtures, "v20-y5.json"), phrase: "(derived from saved records)"),
            (name: "not-recorded", file: Path.Combine(earlier, "render-gap-v20-y5-no-year1-snapshot.json"), phrase: "Year reached: not recorded") };
        foreach (var size in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) })
        {
            S1CObjectiveHistoryUnityRunner.SetGameSize(size.x, size.y);
            for (int f = 0; f < 120 && (Screen.width != size.x || Screen.height != size.y); f++) yield return null;
            yield return Frames(24);
            Check(Screen.width == size.x && Screen.height == size.y, "render size " + size);
            foreach (var state in states)
            {
                ui.CloseAll(); yield return Frames(4);
                byte[] bytes = File.ReadAllBytes(state.file); File.WriteAllBytes(path, bytes);
                Check(saves.CanLoad(out string problem), state.name + " CanLoad " + problem);
                Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(System.Text.Encoding.UTF8.GetString(bytes)), false), state.name + " typed load");
                yield return Frames(12); ui.CloseAll(); ui.ShowReview(); yield return Frames(16);
                var modal = ui.RootElement.Q(className: "modal-review"); Check(modal != null, state.name + " review open");
                var scroll = modal.Q<ScrollView>(); Check(scroll != null, "review ScrollView");
                var history = modal.Query<Label>().ToList().Where(l => l.text != null && (l.text.StartsWith("First reached") || l.text.StartsWith("Year reached"))).ToList();
                Check(history.Count > 0 && history.Any(l => l.text.Contains(state.phrase)), state.name + " history text present");
                Check(state.name != "not-recorded" || history.All(l => l.text == state.phrase), "not-recorded does not imply a year");
                File.WriteAllText(Path.Combine(output, state.name + "-" + size.x + "x" + size.y + ".txt"), string.Join("\n", history.Select(l => l.text)));
                // Scroll the card, not just its heading. It fits at these sizes; each history line must be fully visible.
                var card = history[0].parent; scroll.ScrollTo(card); yield return Frames(12);
                scroll.verticalScroller.value = scroll.verticalScroller.highValue; yield return Frames(12);
                Rect viewport = scroll.contentViewport.worldBound;
                foreach (var label in history)
                {
                    Rect r = label.worldBound;
                    Check(r.xMin >= viewport.xMin - 1.5f && r.xMax <= viewport.xMax + 1.5f && r.yMin >= viewport.yMin - 1.5f && r.yMax <= viewport.yMax + 1.5f,
                        state.name + " " + size + " fully visible: " + label.text + " bounds=" + r + " viewport=" + viewport);
                    Vector2 measured = label.MeasureTextSize(label.text, label.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
                    Check(measured.y <= label.contentRect.height + 1.5f, state.name + " line not clipped: " + label.text);
                }
                yield return new WaitForEndOfFrame();
                Texture2D shot = ScreenCapture.CaptureScreenshotAsTexture();
                Check(shot != null && shot.width == size.x && shot.height == size.y, "capture dimensions");
                string name = "review-" + state.name + "-objectives-" + size.x + "x" + size.y + ".png";
                File.WriteAllBytes(Path.Combine(output, "shots", name), shot.EncodeToPNG()); Destroy(shot); captures++;
                Debug.Log("T05_RENDER_SHOT " + name);
            }
        }
    }
}
#endif
