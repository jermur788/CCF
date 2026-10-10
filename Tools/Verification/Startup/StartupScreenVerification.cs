#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

// Disposable gate for the standalone startup / title screen. Copy into Assets/ForestPrototype/, run via
// run_startup.py (or -executeMethod StartupScreenVerification.Begin), then remove the copy and its .meta.
//
// It builds a StandaloneSessionMenu in startup mode exactly as the Player bootstrap does (ShowStartupScreen),
// drives it through the same buttons a tester would click, and checks that every action reaches the existing
// save / new-session / quit paths. The player's save file is backed up and restored. No scene is saved.
//
// Interactive-only parts (virtual keyboard presses, Game View sizing, screenshots) are skipped in batch mode and
// reported as STARTUP_SKIPPED; they are never counted as passed.
//
// Environment:
//   CCF_STARTUP_SCREEN=1   run the Bootstrap check instead (the real Bootstrap must open the title in the Editor)
//   CCF_ACCEPTANCE_OUTPUT  directory for rendered captures (interactive run only)
public static class StartupScreenVerification
{
    private const string PendingExit = "CCF.StartupScreenVerification.ExitCode";

    [InitializeOnLoadMethod]
    private static void InstallEditorExit()
    {
        EditorApplication.update -= FinishEditorExit;
        EditorApplication.update += FinishEditorExit;
    }

    private static void FinishEditorExit()
    {
        int code = SessionState.GetInt(PendingExit, -1);
        if (code < 0 || EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        SessionState.EraseInt(PendingExit);
        EditorApplication.Exit(code);
    }

    // SessionState survives the domain reload when Play Mode ends.
    public static void Complete(int code)
    {
        SessionState.SetInt(PendingExit, code);
        EditorApplication.delayCall += EditorApplication.ExitPlaymode;
    }

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
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
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Startup review " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("StartupScreenVerification.Begin"))
            new GameObject("Disposable startup screen verification").AddComponent<StartupScreenVerificationRunner>();
    }
}

public sealed class StartupScreenVerificationRunner : MonoBehaviour
{
    private static readonly string[] TitleButtons = { "Start New Scenario", "Continue", "Controls / Help", "Quit" };

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private string savePath;
    private readonly Dictionary<string, byte[]> originalFiles = new Dictionary<string, byte[]>();
    private readonly List<string> logLines = new List<string>();
    private readonly List<string> faultyLines = new List<string>();
    private readonly List<string> skipped = new List<string>();
    private bool keysDelivered = true;

    private static void Check(bool ok, string why)
    {
        if (!ok) throw new InvalidOperationException(why);
        Debug.Log("STARTUP_CHECK " + why);
    }

    private void OnLog(string condition, string stackTrace, LogType type)
    {
        logLines.Add(condition);
        if (type == LogType.Exception && stackTrace != null && (stackTrace.Contains("StandaloneSessionMenu") || stackTrace.Contains("ForestSaveController")))
            faultyLines.Add(condition);
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 8; i++) yield return null;
        Application.logMessageReceived += OnLog;
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        manager = ui.Manager;
        saves = FindFirstObjectByType<ForestSaveController>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        player = ui.Player;
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        foreach (string path in new[] { savePath, savePath + ".bak", savePath + ".tmp" })
            originalFiles[path] = File.Exists(path) ? File.ReadAllBytes(path) : null;

        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        Exception failure = null;
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        Application.logMessageReceived -= OnLog;
        // Put the machine's own save files back exactly as found.
        foreach (KeyValuePair<string, byte[]> file in originalFiles)
        {
            if (file.Value != null) File.WriteAllBytes(file.Key, file.Value);
            else if (File.Exists(file.Key)) File.Delete(file.Key);
        }
        foreach (string line in skipped) Debug.Log("STARTUP_SKIPPED " + line);
        Debug.Log(failure == null ? "STARTUP_SCREEN_VERIFY_PASS" : "STARTUP_SCREEN_VERIFY_FAIL " + failure);
        StartupScreenVerification.Complete(failure == null ? 0 : 1);
    }

    // ---- helpers -------------------------------------------------------------------------------------------

    private static IEnumerator Frames(int count) { for (int i = 0; i < count; i++) yield return null; }

    private string Hash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    private void DeleteSaveFiles()
    {
        foreach (string path in originalFiles.Keys) if (File.Exists(path)) File.Delete(path);
    }

    private static byte[] Bytes(string path) => File.Exists(path) ? File.ReadAllBytes(path) : null;
    private static bool Same(byte[] a, byte[] b) => a != null && b != null && a.SequenceEqual(b);

    // What Bootstrap does in a Player: add the menu, ask it to open on the title.
    private static StandaloneSessionMenu NewStartupMenu()
    {
        var menu = new GameObject("Startup screen under test").AddComponent<StandaloneSessionMenu>();
        menu.ShowStartupScreen();
        return menu;
    }

    private static IEnumerator WaitOpen(StandaloneSessionMenu menu)
    {
        for (int frame = 0; frame < 240 && !StandaloneSessionMenu.IsOpen; frame++) yield return null;
        Check(StandaloneSessionMenu.IsOpen && menu.StartupScreenActive, "the startup screen opens on its own once the scene is ready");
        yield return Frames(2);
    }

    private static IEnumerator Dispose(StandaloneSessionMenu menu)
    {
        if (menu == null) yield break;
        if (StandaloneSessionMenu.IsOpen) menu.Close();
        Destroy(menu.gameObject);
        yield return Frames(2);
    }

    private static VisualElement Root(StandaloneSessionMenu menu) => menu.GetComponent<UIDocument>().rootVisualElement;
    private static List<string> Labels(StandaloneSessionMenu menu) => Root(menu).Query<Label>().ToList().Select(l => l.text).ToList();
    private static List<string> ButtonTexts(StandaloneSessionMenu menu) => Root(menu).Query<Button>().ToList().Select(b => b.text).ToList();
    private static Button FindButton(StandaloneSessionMenu menu, string text) => Root(menu).Query<Button>().ToList().FirstOrDefault(b => b.text == text);

    // Editor synthetic mouse/navigation events do not reach this runtime panel, so run the button's own registered
    // callback (the entry point the project's other gates use). Real mouse and keyboard navigation of the panel is
    // a separate human check.
    private static IEnumerator Click(StandaloneSessionMenu menu, string text)
    {
        Button button = FindButton(menu, text);
        Check(button != null, "button present: " + text);
        Check(button.enabledInHierarchy, "button enabled: " + text);
        MethodInfo invoke = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        Check(invoke != null, "Clickable callback entry point available");
        invoke.Invoke(button.clickable, new object[] { null });
        yield return Frames(2);
    }

    private static IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private void CheckForestHeldBack(string when)
    {
        Check(StandaloneSessionMenu.IsOpen && Time.timeScale == 0f, when + ": time is paused");
        Check(!player.enabled && !saves.enabled && !ui.enabled, when + ": player, save shortcuts and forest UI are disabled");
        Check(UnityEngine.Cursor.lockState != CursorLockMode.Locked, when + ": the mouse is free for the menu, not locked to the forest");
    }

    private void CheckForestHandedBack(string when, bool playerWasEnabled)
    {
        Check(!StandaloneSessionMenu.IsOpen && Time.timeScale == 1f, when + ": menu closed and time running");
        Check(player.enabled == playerWasEnabled && saves.enabled && ui.enabled, when + ": player, save shortcuts and forest UI handed back");
    }

    // ---- the run -------------------------------------------------------------------------------------------

    private IEnumerator Run()
    {
        ForestSaveData fresh = saves.CaptureData();
        string freshHash = ScenarioReferenceArchive.WorldHash(fresh);
        Check(manager.CurrentEcologicalYear == 0, "the scene starts at year 0");

        if (Environment.GetEnvironmentVariable("CCF_STARTUP_SCREEN") == "1")
        {
            yield return BootstrapRun(freshHash);
            yield break;
        }
        bool playerWasEnabled = player.enabled;
        yield return EditorDefaultPolicy();
        yield return NoSaveFlow(freshHash, playerWasEnabled);
        yield return SavedForestFlow(fresh, freshHash, playerWasEnabled);
        yield return ParityFlow(fresh, freshHash);
        yield return Rendered();
        Check(keysDelivered, "virtual key presses reached the game (the F10 control), so the key checks were meaningful");
        Check(faultyLines.Count == 0, "no exception was thrown from the menu or the save controller");
        Check(Hash() == freshHash, "the world ends exactly where it began");
    }

    // Editor Play Mode enters the stand directly unless CCF_STARTUP_SCREEN=1.
    private static IEnumerator EditorDefaultPolicy()
    {
        var menus = FindObjectsByType<StandaloneSessionMenu>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Check(menus.Length == 0 && !StandaloneSessionMenu.IsOpen,
            "Editor Play Mode enters the stand directly: Bootstrap makes no startup screen unless CCF_STARTUP_SCREEN=1");
        yield break;
    }

    // The real Bootstrap (Editor opt-in) must create the menu and open the title with no help from this gate.
    private IEnumerator BootstrapRun(string freshHash)
    {
        for (int frame = 0; frame < 300 && !StandaloneSessionMenu.IsOpen; frame++) yield return null;
        var menu = FindFirstObjectByType<StandaloneSessionMenu>();
        Check(menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive, "Bootstrap opened the startup screen on its own");
        yield return Frames(2);
        CheckForestHeldBack("bootstrapped title");
        Check(Hash() == freshHash, "the bootstrapped title leaves the world unchanged");
        yield return Click(menu, "Start New Scenario");
        if (StandaloneSessionMenu.IsOpen)
        {
            // A save already exists on this machine: Start New must have asked first.
            Check(File.Exists(savePath), "Start New only waits when a save exists");
            yield return Click(menu, "Start New Scenario");
        }
        CheckForestHandedBack("bootstrapped Start New Scenario", true);
        Check(Hash() == freshHash, "Start New Scenario enters the unchanged fresh world");
    }

    // Title with no save: content, isolation, Continue disabled, Controls / Help, Quit, Start New, then F10.
    private IEnumerator NoSaveFlow(string freshHash, bool playerWasEnabled)
    {
        DeleteSaveFiles();
        StandaloneSessionMenu menu = NewStartupMenu();
        yield return WaitOpen(menu);
        CheckForestHeldBack("title");
        Check(Hash() == freshHash, "opening the title leaves the world unchanged");

        List<string> labels = Labels(menu);
        Check(labels.Contains(StandaloneSessionMenu.StartupTitle) && labels.Contains(StandaloneSessionMenu.StartupScenarioName), "title and scenario name shown");
        Check(labels.Contains(StandaloneSessionMenu.StartupScenarioSubtitle), "subtitle shown");
        Check(manager.Definition.DisplayName.EndsWith(StandaloneSessionMenu.StartupScenarioSubtitle),
            "subtitle is the tail of the scenario definition's display name (" + manager.Definition.DisplayName + ")");
        Check(ButtonTexts(menu).SequenceEqual(TitleButtons), "exactly Start New Scenario, Continue, Controls / Help, Quit");
        Check(FindButton(menu, "Start New Scenario").enabledInHierarchy && FindButton(menu, "Controls / Help").enabledInHierarchy && FindButton(menu, "Quit").enabledInHierarchy,
            "Start New Scenario, Controls / Help and Quit are enabled");
        Check(!FindButton(menu, "Continue").enabledInHierarchy && !menu.ContinueAvailable, "Continue is disabled when there is no save");
        Check(menu.ContinueNote != null && menu.ContinueNote.StartsWith("No saved forest") && labels.Contains(menu.ContinueNote), "the disabled Continue states a concise reason");
        Check(labels.Any(t => t.StartsWith("Private test build") && t.Contains(menu.BuildIdentity.buildId)), "private-test build identity shown");
        Check(labels.Count == 5, "nothing else is written on the title (no ecology, no management advice): " + string.Join(" | ", labels));

        logLines.Clear();
        menu.ContinueSavedForest();
        yield return Frames(2);
        Check(StandaloneSessionMenu.IsOpen && menu.StartupScreenActive && Hash() == freshHash && !logLines.Any(l => l.Contains("CCF_STARTUP_CONTINUE")),
            "Continue with no save attempts no load and leaves the title open");
        Check(!File.Exists(savePath), "the title created no save file");

        if (Application.isBatchMode)
            skipped.Add("virtual key presses on the title (need an interactive Editor)");
        else
        {
            foreach (Key key in new[] { Key.F10, Key.W, Key.E, Key.Tab, Key.M, Key.O, Key.X, Key.C, Key.F1, Key.F5, Key.F9, Key.Escape, Key.Space, Key.Digit1 })
                yield return Press(key);
            Check(StandaloneSessionMenu.IsOpen && menu.StartupScreenActive, "F10 and gameplay keys do not leave the title");
            CheckForestHeldBack("after key presses");
            Check(manager.CurrentEcologicalYear == 0 && !manager.WorkPlanOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !ui.Help.IsOpen,
                "no key opened a forest panel or advanced time");
            Check(Hash() == freshHash && !File.Exists(savePath), "no key changed the world or wrote a save (F5)");
        }

        yield return Click(menu, "Controls / Help");
        labels = Labels(menu);
        foreach (string key in new[] { "W A S D", "Mouse", "E", "M", "Tab", "O", "F1", "Esc", "F5 / F9", "F10" })
            Check(labels.Contains(key), "controls list shows " + key);
        string all = string.Join(" | ", labels);
        Check(all.Contains("Stand Map") && all.Contains("Work Plan") && all.Contains("Learning objectives") && all.Contains("Session menu"),
            "controls list names Stand Map, Work Plan, Learning objectives and the session menu");
        Check(ButtonTexts(menu).SequenceEqual(new[] { "Back" }), "Controls / Help offers only Back");
        CheckForestHeldBack("controls");
        yield return Click(menu, "Back");
        Check(ButtonTexts(menu).SequenceEqual(TitleButtons) && menu.StartupScreenActive, "Back returns to the title");

        // Quit uses the existing quit path. Application.Quit is ignored inside the Editor.
        logLines.Clear();
        yield return Click(menu, "Quit");
        Check(logLines.Any(l => l.Contains("CCF_SESSION_NORMAL_QUIT")), "Quit invokes the existing quit path (CCF_SESSION_NORMAL_QUIT)");
        Check(StandaloneSessionMenu.IsOpen, "Quit did not enter the stand");

        // Start New Scenario with no save: no confirmation, straight into the unchanged fresh stand.
        logLines.Clear();
        yield return Click(menu, "Start New Scenario");
        CheckForestHandedBack("Start New Scenario", playerWasEnabled);
        Check(logLines.Any(l => l.Contains("CCF_STARTUP_NEW_SCENARIO")), "Start New Scenario is logged for Player.log evidence");
        Check(ecology.StandGeometryModelVersion == StandGeometryPolicy.NewGameModelForSession,
            "the fresh scenario uses the new-game stand geometry policy, read from the policy and not restated by the title");
        Check(manager.CurrentEcologicalYear == 0 && Hash() == freshHash, "Start New Scenario enters the unchanged year-0 world");
        if (Environment.GetEnvironmentVariable("CCF_STAND_GEOMETRY") == null)
        {
            Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80, "production default: Enlarged80");
            Check(ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData()) == "04A78188A6F8E6F9",
                "Enlarged80 start anchor 04A78188A6F8E6F9 unchanged (valid for this generator and configuration only)");
        }
        Check(!File.Exists(savePath), "Start New Scenario wrote no save");

        // F10 still opens the existing fuller session card in the stand. The key press doubles as the control for
        // the virtual-keyboard checks above.
        if (Application.isBatchMode)
        {
            skipped.Add("F10 key press in the stand (need an interactive Editor); the Open() route is checked instead");
            menu.Open();
        }
        else
        {
            yield return Press(Key.F10);
            keysDelivered = StandaloneSessionMenu.IsOpen;
            if (!keysDelivered) menu.Open();
        }
        yield return Frames(2);
        Check(StandaloneSessionMenu.IsOpen && !menu.StartupScreenActive, "the session card, not the title, opens once the player is in the stand");
        Check(ButtonTexts(menu).Any(t => t.StartsWith("Start a fresh")) && ButtonTexts(menu).Any(t => t.StartsWith("Quit")), "session card still offers a fresh test and Quit");
        menu.Close();
        yield return Frames(2);
        CheckForestHandedBack("closing the session card", playerWasEnabled);
        yield return Dispose(menu);
    }

    // Title with a real save: Continue enabled, Start New asks first, Continue loads through the existing path.
    private IEnumerator SavedForestFlow(ForestSaveData fresh, string freshHash, bool playerWasEnabled)
    {
        DeleteSaveFiles();
        Check(manager.AdvanceYear(), "advance to year 1 so the save differs from the fresh world");
        saves.Save();
        Check(File.Exists(savePath), "a real save was written by the existing Save()");
        string savedHash = Hash();
        byte[] savedBytes = File.ReadAllBytes(savePath);
        Check(savedHash != freshHash, "the saved forest differs from the fresh forest");
        Check(saves.LoadData(fresh, false) && Hash() == freshHash, "fresh world restored behind the title");
        yield return Frames(2);

        StandaloneSessionMenu menu = NewStartupMenu();
        yield return WaitOpen(menu);
        CheckForestHeldBack("title with a save");
        Check(saves.CanLoad(out string problem) && problem == null, "CanLoad accepts the real save");
        Check(menu.ContinueAvailable && FindButton(menu, "Continue").enabledInHierarchy, "Continue is enabled for a valid save");
        Check(Labels(menu).Count == 4, "no reason line while Continue is available: " + string.Join(" | ", Labels(menu)));
        Check(Hash() == freshHash && Same(Bytes(savePath), savedBytes), "reading the save for the title touched neither the world nor the file");

        // Start New with a save on disk asks first, says the save is kept, and changes nothing while asking.
        yield return Click(menu, "Start New Scenario");
        Check(StandaloneSessionMenu.IsOpen && menu.StartupScreenActive, "Start New Scenario with a save asks first");
        Check(Labels(menu).Contains("Start a new Scenario One?") && Labels(menu).Any(t => t.Contains("saved forest") && t.Contains("save over it")),
            "the confirmation says the saved forest stays until it is saved over");
        Check(ButtonTexts(menu).SequenceEqual(new[] { "Start New Scenario", "Back" }), "the confirmation offers Start New Scenario and Back");
        Check(Hash() == freshHash && Same(Bytes(savePath), savedBytes), "nothing changed while asking");
        yield return Click(menu, "Back");
        Check(ButtonTexts(menu).SequenceEqual(TitleButtons), "Back returns to the title");

        // Continue loads the saved forest through the existing validated path.
        logLines.Clear();
        yield return Click(menu, "Continue");
        CheckForestHandedBack("Continue", playerWasEnabled);
        Check(logLines.Any(l => l.Contains("CCF_STARTUP_CONTINUE")), "Continue is logged for Player.log evidence");
        Check(Hash() == savedHash && manager.CurrentEcologicalYear == 1, "Continue restored the saved year-1 forest");
        Check(ecology.StandGeometryModelVersion == fresh.standGeometryModel, "the loader applied the save's own stand geometry");
        Check(Same(Bytes(savePath), savedBytes), "loading did not modify the save file");
        yield return Dispose(menu);
        Check(saves.LoadData(fresh, false) && Hash() == freshHash, "fresh world restored");
        yield return Frames(2);

        // Confirmed Start New keeps the save and enters the unchanged fresh world.
        menu = NewStartupMenu();
        yield return WaitOpen(menu);
        yield return Click(menu, "Start New Scenario");
        yield return Click(menu, "Start New Scenario");
        CheckForestHandedBack("confirmed Start New Scenario", playerWasEnabled);
        Check(Hash() == freshHash && manager.CurrentEcologicalYear == 0, "confirmed Start New Scenario enters the unchanged fresh world");
        Check(Same(Bytes(savePath), savedBytes), "the saved forest was kept");
        yield return Dispose(menu);

        // An unreadable save leaves Continue disabled with the loader's own reason; the world is untouched.
        File.WriteAllText(savePath, "this is not a forest save");
        menu = NewStartupMenu();
        yield return WaitOpen(menu);
        Check(!menu.ContinueAvailable && !FindButton(menu, "Continue").enabledInHierarchy, "an unreadable save disables Continue");
        Check(menu.ContinueNote == "Save file unreadable." && Labels(menu).Contains(menu.ContinueNote), "the reason is the existing loader message");
        menu.ContinueSavedForest();
        yield return Frames(2);
        Check(StandaloneSessionMenu.IsOpen && menu.StartupScreenActive && Hash() == freshHash, "Continue refused for an unreadable save; the world is untouched");
        yield return Dispose(menu);
        DeleteSaveFiles();
    }

    // The title offers Continue exactly when the existing loader would accept the file.
    private IEnumerator ParityFlow(ForestSaveData fresh, string freshHash)
    {
        DeleteSaveFiles();
        Check(manager.AdvanceYear(), "advance to year 1 for the parity fixtures");
        saves.Save();
        byte[] valid = File.ReadAllBytes(savePath);
        Check(saves.LoadData(fresh, false) && Hash() == freshHash, "fresh world restored before the parity fixtures");

        ScenarioReferenceArchive reference = ScenarioReferenceArchive.Load();
        byte[] legacy = reference != null ? Encoding.UTF8.GetBytes(JsonUtility.ToJson(reference.AtYear(20).world)) : null;
        var cases = new List<(string name, byte[] bytes, bool? expected)>
        {
            ("valid save in the current stand geometry", valid, true),
            ("truncated save", valid.Take(valid.Length / 2).ToArray(), false),
            ("empty JSON object", Encoding.UTF8.GetBytes("{}"), false),
            ("not JSON", Encoding.UTF8.GetBytes("not json"), false),
        };
        if (legacy != null) cases.Add(("Legacy40 reference save (Year 20)", legacy, null));

        foreach (var c in cases)
        {
            File.WriteAllBytes(savePath, c.bytes);
            bool can = saves.CanLoad(out string note);
            string before = Hash();
            int geometryBefore = ecology.StandGeometryModelVersion;
            saves.Load();
            yield return Frames(2);
            bool changed = Hash() != before || ecology.StandGeometryModelVersion != geometryBefore;
            Debug.Log("STARTUP_PARITY " + c.name + " canLoad=" + can + " loaderChangedWorld=" + changed + " note=" + note);
            Check(can == changed, "CanLoad agrees with the real loader: " + c.name);
            if (c.expected.HasValue)
                Check(can == c.expected.Value, c.name + (c.expected.Value ? " is accepted" : " is refused"));
            if (can && c.name.StartsWith("Legacy40"))
                Check(ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40, "the loader, not the title, chose the save's own Legacy40 geometry");
            Check(saves.LoadData(fresh, false) && Hash() == freshHash, "fresh world restored after: " + c.name);
            yield return Frames(2);
        }
        if (legacy == null) skipped.Add("Legacy40 reference fixture (reference archive not available)");
        DeleteSaveFiles();
    }

    // Layout and captures at 1280 x 720 and 1920 x 1080 (interactive Editor with a display only).
    private IEnumerator Rendered()
    {
        if (Application.isBatchMode)
        {
            skipped.Add("rendered layout and captures at 1280x720 and 1920x1080 (need an interactive Editor with a display)");
            yield break;
        }
        string output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        Check(!string.IsNullOrEmpty(output), "CCF_ACCEPTANCE_OUTPUT names the capture directory");
        Directory.CreateDirectory(output);
        DeleteSaveFiles();
        saves.Save();
        byte[] validSave = File.ReadAllBytes(savePath);

        foreach (int width in new[] { 1280, 1920 })
        {
            int height = width == 1280 ? 720 : 1080;
            StartupScreenVerification.SetGameSize(width, height);
            yield return Frames(14);
            Debug.Log("STARTUP_RENDER_SIZE requested=" + width + "x" + height + " actual=" + Screen.width + "x" + Screen.height);

            DeleteSaveFiles();
            StandaloneSessionMenu menu = NewStartupMenu();
            yield return WaitOpen(menu);
            yield return Frames(6);
            CheckLayout(menu, "title-nosave-" + width);
            yield return Capture(output, "startup-title-nosave-" + width);
            yield return Dispose(menu);

            File.WriteAllBytes(savePath, validSave);
            menu = NewStartupMenu();
            yield return WaitOpen(menu);
            yield return Frames(6);
            CheckLayout(menu, "title-save-" + width);
            yield return Capture(output, "startup-title-save-" + width);
            yield return Click(menu, "Start New Scenario");
            yield return Frames(6);
            CheckLayout(menu, "confirm-" + width);
            yield return Capture(output, "startup-confirm-" + width);
            yield return Click(menu, "Back");
            yield return Click(menu, "Controls / Help");
            yield return Frames(6);
            CheckLayout(menu, "controls-" + width);
            yield return Capture(output, "startup-controls-" + width);
            yield return Dispose(menu);
            Debug.Log("STARTUP_RENDER_SIZE_PASS " + width + "x" + height);
        }
        DeleteSaveFiles();
    }

    private static string Describe(VisualElement element) => (element as TextElement)?.text ?? element.name ?? element.GetType().Name;

    private static void CheckLayout(StandaloneSessionMenu menu, string name)
    {
        VisualElement root = Root(menu);
        Rect bounds = root.worldBound;
        Check(bounds.width > 0f && bounds.height > 0f, name + ": the panel has a size");
        Check(root.resolvedStyle.backgroundColor.a >= 0.95f, name + ": the backdrop hides the forest underneath");
        var shown = new List<VisualElement>();
        shown.AddRange(root.Query<Label>().ToList().Where(l => l.resolvedStyle.display != DisplayStyle.None && !string.IsNullOrEmpty(l.text)));
        shown.AddRange(root.Query<Button>().ToList().Where(b => b.resolvedStyle.display != DisplayStyle.None));
        foreach (VisualElement element in shown)
        {
            Rect r = element.worldBound;
            Check(!float.IsNaN(r.x) && !float.IsNaN(r.y) && r.width > 0f && r.height > 0f, name + ": laid out: " + Describe(element));
            Check(r.xMin >= bounds.xMin + 8f && r.xMax <= bounds.xMax - 8f && r.yMin >= bounds.yMin + 8f && r.yMax <= bounds.yMax - 8f,
                name + ": fully on screen (no clipping): " + Describe(element));
        }
        for (int i = 0; i < shown.Count; i++)
            for (int j = i + 1; j < shown.Count; j++)
                Check(!shown[i].worldBound.Overlaps(shown[j].worldBound), name + ": no overlap between '" + Describe(shown[i]) + "' and '" + Describe(shown[j]) + "'");
        foreach (Button button in root.Query<Button>().ToList())
            Check(button.worldBound.height >= 40f, name + ": button tall enough to click: " + button.text);

        Label subtitle = root.Query<Label>().ToList().FirstOrDefault(l => l.text == StandaloneSessionMenu.StartupScenarioSubtitle);
        Label build = root.Query<Label>().ToList().FirstOrDefault(l => l.text.StartsWith("Private test build"));
        if (subtitle != null && build != null)
            Check(build.resolvedStyle.fontSize < subtitle.resolvedStyle.fontSize && build.resolvedStyle.fontSize >= 14f,
                name + ": build identity is readable but secondary to the subtitle");
    }

    private static IEnumerator Capture(string output, string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG());
        Destroy(image);
    }
}
#endif
