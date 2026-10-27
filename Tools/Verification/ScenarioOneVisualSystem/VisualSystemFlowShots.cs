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

// Disposable screenshots of the two standalone menu screens (SCENARIO-ONE-PROFESSIONAL-MAP-REDESIGN-V1, Phase 1 visual audit):
//   * the startup / title screen a standalone tester sees first, and its controls card;
//   * the F10 session card (the pause / settings-like menu) once the player is in the stand.
// Exercises normal startup with remembered help credits, then Start New, intro/detail, forest, session and Continue.
// Writes a disposable save only in the launcher's isolated configuration; never run against a user save.
//
// Run (graphical Editor, private display):  CCF_STARTUP_SCREEN=1 CCF_DIAG_DIR=<folder outside the repo> CCF_UI_SIZES=1920x1080,1280x720
//   -executeMethod VisualSystemFlowShots.Begin
// CCF_STARTUP_SCREEN=1 makes the real Bootstrap open the title screen in the Editor. Plays in memory, saves nothing.
public static class VisualSystemFlowShots
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        VisualSystemFlowShotsRunner.SetGameSize(VisualSystemFlowShotsRunner.Sizes()[0].x, VisualSystemFlowShotsRunner.Sizes()[0].y);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void RememberHelp()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "VisualSystemFlowShots.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.SetInt(MenuHelpView.PreferencePrefix + menu, 1);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "VisualSystemFlowShots.Begin"))
            new GameObject("Menu shots").AddComponent<VisualSystemFlowShotsRunner>();
    }
}

public sealed class VisualSystemFlowShotsRunner : MonoBehaviour
{
    private string output;
    private int shots;
    private readonly List<string> notes = new List<string>();

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "VisualSystemFlowShots " + width + "x" + height }, null);
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

    private static IEnumerator Settle(int frames) { for (int i = 0; i < frames; i++) yield return null; }

    private IEnumerator Capture(string shot)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null || image.width < 600) throw new InvalidOperationException("no rendered capture for " + shot);
        File.WriteAllBytes(Path.Combine(output, $"{shot}-{Screen.width}x{Screen.height}.png"), image.EncodeToPNG());
        Destroy(image);
        shots++;
    }

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "menu-shots");
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
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        Debug.Log(failure == null ? $"VISUAL_FLOW_PASS shots={shots}" : $"VISUAL_FLOW_FAIL shots={shots} exception={failure}");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Play()
    {
        StandaloneSessionMenu menu = null;
        for (int frame = 0; frame < 900 && (menu == null || !StandaloneSessionMenu.IsOpen); frame++)
        {
            if (menu == null) menu = FindFirstObjectByType<StandaloneSessionMenu>();
            yield return null;
        }
        if (menu == null || !StandaloneSessionMenu.IsOpen) throw new InvalidOperationException("the startup screen did not open (is CCF_STARTUP_SCREEN=1 set?)");
        MethodInfo startupCard = typeof(StandaloneSessionMenu).GetMethod("StartupCard", BindingFlags.Instance | BindingFlags.NonPublic);

        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y);
            yield return Settle(30);
            if (startupCard != null) startupCard.Invoke(menu, null);
            yield return Settle(10);
            yield return Capture("m1-startup-title");
            menu.ShowControlsHelp();
            yield return Settle(10);
            yield return Capture("m2-startup-controls");
        }
        ScenarioOneUiRoot initialUi = FindFirstObjectByType<ScenarioOneUiRoot>();
        if (initialUi.Help.IsOpen) throw new InvalidOperationException("new-game intro opened behind startup");
        if (startupCard != null) startupCard.Invoke(menu, null);
        yield return Settle(10);
        menu.StartNewScenario();
        yield return Settle(20);
        if (StandaloneSessionMenu.IsOpen || !initialUi.Help.IsOpen || initialUi.Help.CurrentMenu != MenuHelpView.Menu.WalkingHud)
            throw new InvalidOperationException("Start New did not open the introduction despite remembered help");
        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y); yield return Settle(30);
            yield return Capture("m4-new-scenario-introduction");
            typeof(MenuHelpView).GetMethod("ToggleDetail", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(initialUi.Help, null);
            yield return Settle(10); yield return Capture("m5-intro-more-detail");
            typeof(MenuHelpView).GetMethod("ToggleDetail", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(initialUi.Help, null);
            yield return Settle(10);
        }
        initialUi.CloseHelp();
        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y); yield return Settle(30); yield return Capture("m6-first-forest-hud");
        }
        if (initialUi.Help.IsOpen || StandaloneSessionMenu.IsOpen) throw new InvalidOperationException("intro did not enter the forest");
        yield return Settle(30);
        notes.Add("startup closed through StartNewScenario(); menu open=" + StandaloneSessionMenu.IsOpen);

        // Dismiss any first-use help window so the F10 card is photographed over the walking HUD's own state, then open the session card.
        ScenarioOneUiRoot ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        if (ui != null) ui.CloseHelp();
        yield return Settle(10);
        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y);
            yield return Settle(30);
            menu.Open();
            yield return Settle(15);
            notes.Add($"session menu open at {size.x}x{size.y}: {StandaloneSessionMenu.IsOpen}");
            yield return Capture("m3-session-menu-f10");
            menu.Close();
            yield return Settle(10);
        }
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        string originalHash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        saves.Save();
        menu.ShowStartupScreen(); yield return Settle(20);
        if (!menu.ContinueAvailable) throw new InvalidOperationException("valid saved forest unavailable to Continue");
        menu.ContinueSavedForest(); yield return Settle(30);
        if (StandaloneSessionMenu.IsOpen || ui.Help.IsOpen || ScenarioReferenceArchive.WorldHash(saves.CaptureData()) != originalHash)
            throw new InvalidOperationException("Continue replayed intro or changed saved year-zero forest");
        notes.Add("PASS normal startup -> remembered-help Start New -> intro -> detail -> forest; Continue year-zero save without intro; exact world hash " + originalHash);
        if (!ui.AdvanceFromWorkPlan()) throw new InvalidOperationException("annual advance unavailable");
        yield return Settle(30);
        ui.CloseHelp(); ui.ShowReview();
        foreach (Vector2Int size in Sizes())
        {
            SetGameSize(size.x, size.y); yield return Settle(30); yield return Capture("m7-annual-review");
        }
    }
}
#endif
