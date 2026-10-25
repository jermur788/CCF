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

// Disposable, explicit executeMethod only. The launcher uses isolated XDG config.
// Tests button callbacks and UI hit targets, not OS-native pointer/key delivery.
public static class OnboardingVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        OnboardingVerificationRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("OnboardingVerification.Begin"))
            new GameObject("Disposable onboarding verification").AddComponent<OnboardingVerificationRunner>();
    }
}

public sealed class OnboardingVerificationRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private StandaloneSessionMenu menu;
    private ForestSaveController saves;
    private string output;
    private int passed, failed, shots;
    private readonly List<string> errors = new List<string>();
    private readonly List<string> checks = new List<string>();
    private readonly List<string> overflows = new List<string>();

    public static void SetGameSize(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        object sizes = typeof(ScriptableSingleton<>).MakeGenericType(sizesType).GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo groupMethod = sizesType.GetMethod("GetGroup");
        object group = groupMethod.Invoke(sizes, new[] { Enum.ToObject(groupMethod.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object gameSize = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null,
            new object[] { Enum.ToObject(kind, 1), width, height, "Onboarding " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) throw new InvalidOperationException("CCF_DIAG_DIR is required");
        Directory.CreateDirectory(output);
        Application.logMessageReceived += Log;
        Exception exception = null;
        var stack = new Stack<IEnumerator>(); stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception e) { exception = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        if (exception != null) Check(false, "EXCEPTION " + exception);
        Check(errors.Count == 0, "NO_EXCEPTIONS_OR_ERRORS " + string.Join(" | ", errors));
        Application.logMessageReceived -= Log;
        File.WriteAllLines(Path.Combine(output, "checks.txt"), checks);
        File.WriteAllLines(Path.Combine(output, "text-overflows.txt"), overflows);
        File.WriteAllText(Path.Combine(output, "result.json"), "{\"passed\":" + passed + ",\"failed\":" + failed + ",\"shots\":" + shots + "}");
        Debug.Log("ONBOARDING_" + (failed == 0 ? "PASS" : "FAIL") + " checks=" + passed + " failures=" + failed + " shots=" + shots);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failed == 0 ? 0 : 1);
    }

    private IEnumerator Run()
    {
        for (int i = 0; i < 900 && !StandaloneSessionMenu.IsOpen; i++) yield return null;
        menu = FindFirstObjectByType<StandaloneSessionMenu>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        saves = FindFirstObjectByType<ForestSaveController>();
        Check(menu != null && ui != null && saves != null && menu.StartupScreenActive && StandaloneSessionMenu.IsOpen, "NORMAL_STARTUP");
        string config = Path.Combine(Directory.GetParent(output).FullName, "config") + Path.DirectorySeparatorChar;
        if (!Path.GetFullPath(Application.persistentDataPath).StartsWith(config, StringComparison.Ordinal))
            throw new InvalidOperationException("Refusing Save outside isolated config: " + Application.persistentDataPath);
        Check(!ui.Help.Onboarding.IsOpen, "NO_ONBOARDING_BEHIND_STARTUP");
        string before = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        foreach (Vector2Int size in new[] { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) })
        {
            SetGameSize(size.x, size.y);
            yield return Frames(30);
            Check(Screen.width == size.x && Screen.height == size.y, "RESOLUTION " + size);
            if (!StandaloneSessionMenu.IsOpen) menu.ShowStartupScreen();
            yield return Frames(5);
            menu.StartNewScenario();
            yield return Frames(15);
            Page(0);
            Check(ui.Help.Root.Q("menu-help-window").resolvedStyle.display == DisplayStyle.None, "OLD_INTRO_AND_MORE_DETAIL_HIDDEN");
            ui.Help.Onboarding.Back(); yield return Frames(2); Page(0);
            yield return Capture(1, size);
            Click("onboarding-next"); yield return Frames(6); Page(1);
            Click("onboarding-back"); yield return Frames(6); Page(0);
            Click("onboarding-next"); yield return Frames(6); Page(1);
            yield return Capture(2, size);
            string allText = string.Join(" ", ui.Help.Onboarding.Root.Query<Label>().ToList().Select(l => l.text));
            Check(!allText.Contains("Scenario One") && !allText.Contains("rack") && !allText.Contains("["), "PAGE2_GENERIC_NO_SCENARIO_CONTROLS");
            Click("onboarding-next"); yield return Frames(6); Page(2);
            Click("onboarding-back"); yield return Frames(6); Page(1);
            Click("onboarding-next"); yield return Frames(6); Page(2);
            yield return Capture(3, size);
            Check(ui.Help.Onboarding.Root.Query<Label>().ToList().Any(l => l.text.Contains("both Oak and Beech")), "PAGE3_BOTH_SPECIES_REQUIRED");
            Click("onboarding-next"); yield return Frames(8);
            Check(!ui.Help.IsOpen && !ui.Help.Onboarding.IsOpen && !ui.Manager.ModalOpen && !ui.Manager.AnyPanelOpen && ui.Player.enabled,
                "BEGIN_RESTORES_FOREST_INPUT");
            Check(UnityEngine.Cursor.lockState == CursorLockMode.Locked && !UnityEngine.Cursor.visible, "FOREST_CURSOR_RESTORED");
        }
        string after = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(before == after, "PURE_UI_WORLD_HASH_UNCHANGED " + before + " " + after);

        // Exercise the real save/Continue path, exclusively in the isolated config.
        string savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) throw new InvalidOperationException("Expected a fresh isolated save directory");
        saves.Save();
        yield return Frames(5);
        Check(saves.CanLoad(out string problem), "SAVED_FOREST_VALID " + problem);
        MenuHelpView.ResetProgress();
        menu.ShowStartupScreen(); yield return Frames(5);
        Check(menu.ContinueAvailable, "CONTINUE_AVAILABLE");
        menu.ContinueSavedForest(); yield return Frames(20);
        Check(!StandaloneSessionMenu.IsOpen && !ui.Help.Onboarding.IsOpen && !ui.Help.IsOpen, "CONTINUE_NO_ONBOARDING_WITH_FRESH_PREFS");
        Check(ui.Player.enabled && !ui.Manager.AnyPanelOpen, "CONTINUE_INPUT_RESTORED");
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == after, "CONTINUE_WORLD_PRESERVED");
        menu.Open(); yield return Frames(4);
        Check(StandaloneSessionMenu.IsOpen && !ui.Player.enabled && !ui.enabled && Time.timeScale == 0, "SESSION_MENU_PAUSES");
        menu.Close(); yield return Frames(5);
        Check(!StandaloneSessionMenu.IsOpen && ui.enabled && ui.Player.enabled && Time.timeScale > 0, "SESSION_MENU_RESTORES");
        ui.ShowCurrentHelp(); yield return Frames(3);
        Check(ui.Help.IsOpen && !ui.Help.Onboarding.IsOpen, "F1_HELP_REMAINS_SEPARATE");
        ui.CloseHelp(); yield return Frames(4);
        Check(!ui.Help.IsOpen, "F1_HELP_CLOSES");
        File.WriteAllText(Path.Combine(output, "persistent-path.txt"), Application.persistentDataPath);
    }

    private void Page(int index)
    {
        Check(ui.Help.IsOpen && ui.Help.Onboarding.IsOpen && ui.Help.Onboarding.PageIndex == index, "PAGE_" + (index + 1));
        Check(ui.Manager.ModalOpen && !ui.Player.enabled && UnityEngine.Cursor.visible && UnityEngine.Cursor.lockState == CursorLockMode.None,
            "PAGE_" + (index + 1) + "_MODAL_INPUT");
        Check(ui.Help.Onboarding.Root.Query<Label>().ToList().Any(l => l.text == ScenarioOneOnboardingCopy.Titles[index]), "LIVE_TITLE_" + (index + 1));
    }

    private void Click(string name)
    {
        Button button = ui.Help.Onboarding.Root.Q<Button>(name);
        if (button == null) throw new InvalidOperationException("Button missing: " + name);
        VisualElement hit = button.panel.Pick(button.worldBound.center);
        while (hit != null && hit != button) hit = hit.parent;
        Check(hit == button, "BUTTON_HIT_TARGET_" + name);
        // Same explicit callback smoke used by the existing startup/menu gates.
        typeof(Clickable).GetMethod("Invoke", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(button.clickable, new object[] { null });
    }

    private IEnumerator Capture(int page, Vector2Int size)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width == size.x && image.height == size.y, "ACTUAL_CAPTURE_SIZE_" + page + "_" + size);
        File.WriteAllBytes(Path.Combine(output, "page-" + page + "-" + size.x + "x" + size.y + ".png"), image.EncodeToPNG());
        Destroy(image); shots++;
        foreach (Label label in ui.Help.Onboarding.Root.Query<Label>().ToList())
        {
            Vector2 measured = label.MeasureTextSize(label.text, label.contentRect.width, VisualElement.MeasureMode.Exactly, 0, VisualElement.MeasureMode.Undefined);
            bool fits = measured.y <= label.contentRect.height + 3;
            if (!fits) overflows.Add(size + " page=" + page + " text=" + label.text + " measured=" + measured + " box=" + label.contentRect);
            Check(fits, "TEXT_FITS page=" + page + " " + label.text.Replace("\n", " / "));
        }
    }

    private static IEnumerator Frames(int count) { for (int i = 0; i < count; i++) yield return null; }
    private void Check(bool ok, string description)
    {
        if (ok) passed++; else failed++;
        string line = "ONBOARDING_CHECK_" + (ok ? "PASS " : "FAIL ") + description;
        checks.Add(line); Debug.Log(line);
    }
    private void Log(string message, string stack, LogType type)
    {
        if (type == LogType.Exception || type == LogType.Error || type == LogType.Assert) errors.Add(message);
    }
}
#endif
