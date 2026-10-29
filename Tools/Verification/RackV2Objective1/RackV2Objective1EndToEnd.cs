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
using Cursor = UnityEngine.Cursor;

// Disposable Tier-2 gate for RACK-V2-12: the whole Scenario One Objective 1 flow in ONE fresh game, in one Editor process.
//
//   real startup screen -> Start New Scenario -> premise -> walk (native W) -> Stand Map (native M) -> Plan Racks -> suggested layout
//   -> edit (select + native Delete, refused 4-rack confirm, Undo, move) -> Confirm Rack Plan -> back to the forest -> Tree Marks help
//   -> approval before marking is refused -> spray the five entrance trees (native P + left-click, with negative cases)
//   -> Objective 1 marking complete -> approve in the Work Plan (rack.approve) -> advance one year (execution) -> rack.result
//   -> lesson 2 on the confirmed teaching rack -> ordinary X/C marking -> a real disk save + load round trip.
//
// Native OS events come from native_input.py on the private Xephyr display (request files, as in RACK-V2-08/09). UI buttons are
// invoked through their own Clickable, as the guided/combined gates do. Fixtures, stated where used: the player is placed beside each
// entrance tree (a short native walk is asserted separately; no claim that the player walked to every entrance), and the one map
// "move" edit is committed through the editor's own pointer-release path because the real-pointer drag is qualified by RACK-V2-05/06.
//
// CCF_O1E2E_MODE=qualify (default) asserts every step. CCF_O1E2E_MODE=diagnostic measures the approval-before-marking case instead of
// asserting it, restores the in-memory state if approval went through, and carries on, so one baseline run can show every defect.
// Save safety: refuses to run unless Application.persistentDataPath is inside XDG_CONFIG_HOME (the runner's isolated config).
public static class RackV2Objective1EndToEnd
{
    public const string Marker = "RackV2Objective1EndToEnd.Begin";

    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        RackV2Objective1Runner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    // A first-time player: no lesson credit and no first-use help seen on this device (isolated config only).
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Contains(Marker)) return;
        foreach (string id in LearningObjectivesView.StepIds) PlayerPrefs.DeleteKey(LearningObjectivesView.PreferencePrefix + id);
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) PlayerPrefs.DeleteKey(MenuHelpView.PreferencePrefix + menu);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains(Marker))
            new GameObject("Disposable RACK-V2-12 Objective 1 end-to-end gate").AddComponent<RackV2Objective1Runner>();
    }
}

public sealed class RackV2Objective1Runner : MonoBehaviour
{
    private static readonly Vector2Int[] Sizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };
    private static readonly BindingFlags AnyInstance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestTreeMarkingManager marks;
    private ForestSaveController saves;
    private ForestPlayer player;
    private ForestEcologyController ecology;
    private string output, savePath, mode;
    private int checks, shots, defects, sequence;
    private readonly List<string> captures = new List<string>();
    private readonly List<string> helpSeen = new List<string>();

    // ---- reporting ------------------------------------------------------------------------------------------------
    private void Check(bool ok, string why)
    {
        if (!ok) throw new InvalidOperationException(why);
        checks++; Debug.Log("O1E2E_CHECK " + why);
    }
    private static void Note(string name, string detail) => Debug.Log("O1E2E_MEASURE " + name + " " + detail);
    private bool Diagnostic => mode == "diagnostic";
    // Qualification asserts; diagnostic records the defect and continues.
    private void Expect(bool ok, string why)
    {
        if (ok || !Diagnostic) { Check(ok, why); return; }
        defects++; Debug.Log("O1E2E_DEFECT " + why);
    }

    // ---- small helpers --------------------------------------------------------------------------------------------
    private static IEnumerator Settle(int frames = 16) { for (int i = 0; i < frames; i++) yield return null; }
    private static IEnumerator Wait(float seconds)
    {
        double until = Time.realtimeSinceStartupAsDouble + seconds;
        while (Time.realtimeSinceStartupAsDouble < until) yield return null;
    }
    private ForestTree[] AllTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
    private ForestTree[] Living() => AllTrees().Where(t => t.IsLiving).ToArray();
    private ForestTree Tree(string id) => AllTrees().Single(t => t.TreeId == id);
    private string RackState() => JsonUtility.ToJson(saves.CaptureData().scenarioOne);

    private static bool Visible(VisualElement e)
    {
        for (VisualElement p = e; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None || p.resolvedStyle.visibility == Visibility.Hidden || p.resolvedStyle.opacity <= 0.01f) return false;
        return e.worldBound.width > 0 && e.worldBound.height > 0;
    }
    private string Text() => string.Join("\n", ui.RootElement.Query<TextElement>().ToList().Where(Visible).Select(t => t.text).Where(s => !string.IsNullOrEmpty(s)));
    private Button ButtonStarts(string text) => ui.RootElement.Query<Button>().ToList()
        .FirstOrDefault(b => b.text != null && b.text.StartsWith(text, StringComparison.Ordinal) && Visible(b));
    private Button Named(string name) => ui.RootElement.Q<Button>(name);

    private IEnumerator Invoke(Button button, string what)
    {
        Check(button != null, what + ": control present");
        typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(button.clickable, new object[] { null });
        yield return Settle(3);
    }

    // First-use help screens open by themselves the first time a context is reached. Close them the way a player does and remember which.
    private IEnumerator CloseHelp()
    {
        for (int i = 0; i < 6 && ui.Help.IsOpen; i++)
        {
            helpSeen.Add(ui.Help.CurrentMenu.ToString());
            Button close = ui.Help.Root.Q<Button>("menu-help-close");
            if (close != null && Visible(close)) yield return Invoke(close, "help close"); else ui.CloseHelp();
            yield return Settle(3);
        }
    }

    // ---- native OS input (private display only) -------------------------------------------------------------------
    [Serializable] private sealed class Op { public string op, key; public int x, y, button; public float seconds; }
    [Serializable] private sealed class Request { public int seq; public Op[] ops; }
    private static Op Key(string key) => new Op { op = "key", key = key };
    private static Op KeyDown(string key) => new Op { op = "keydown", key = key };
    private static Op KeyUp(string key) => new Op { op = "keyup", key = key };
    private static Op Sleep(float seconds) => new Op { op = "sleep", seconds = seconds };
    private static Op Click() => new Op { op = "click", button = 1 };

    private IEnumerator Native(params Op[] ops)
    {
        string folder = Environment.GetEnvironmentVariable("CCF_DRIVER_DIR");
        if (string.IsNullOrEmpty(folder)) throw new InvalidOperationException("CCF_DRIVER_DIR (native OS input driver) is required");
        int seq = ++sequence;
        string path = Path.Combine(folder, $"request-{seq:D4}.json");
        File.WriteAllText(path + ".tmp", JsonUtility.ToJson(new Request { seq = seq, ops = ops }));
        File.Move(path + ".tmp", path);
        double until = Time.realtimeSinceStartupAsDouble + 60;
        while (!File.Exists(Path.Combine(folder, $"done-{seq:D4}")) && Time.realtimeSinceStartupAsDouble < until)
        {
            string error = Path.Combine(folder, "driver-error");
            if (File.Exists(error)) throw new InvalidOperationException("native driver: " + File.ReadAllText(error));
            yield return null;
        }
        if (!File.Exists(Path.Combine(folder, $"done-{seq:D4}"))) throw new TimeoutException("native OS input request " + seq);
        yield return Settle(12);
    }

    // ---- game view and captures -----------------------------------------------------------------------------------
    public static void SetGameSize(int width, int height)
    {
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, AnyInstance, null,
            new object[] { Enum.ToObject(kind, 1), width, height, "RACK-V2-12 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", AnyInstance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", AnyInstance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.maximized = true; view.Show(); view.Focus(); view.Repaint();
    }

    // Both supported resolutions; the view returns to 1280x720 so native input always meets the same Game view.
    private IEnumerator Shoot(string state)
    {
        foreach (Vector2Int size in Sizes)
        {
            SetGameSize(size.x, size.y);
            yield return Settle(10);
            yield return new WaitForEndOfFrame();
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            Check(image != null && image.width == size.x && image.height == size.y, "rendered capture " + state + " at " + size.x + "x" + size.y);
            bool visible = false;
            for (int y = 0; y < image.height && !visible; y += 25)
                for (int x = 0; x < image.width && !visible; x += 25)
                    visible = image.GetPixel(x, y).maxColorComponent > 0.05f;
            string file = state + "-" + size.x + "x" + size.y + ".png";
            File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
            Destroy(image);
            Check(visible, "capture has visible pixels " + file);
            captures.Add(file); shots++;
        }
        SetGameSize(1280, 720);
        yield return Settle(10);
    }

    // ---- lifecycle -----------------------------------------------------------------------------------------------
    private IEnumerator Start()
    {
        mode = (Environment.GetEnvironmentVariable("CCF_O1E2E_MODE") ?? "qualify").ToLowerInvariant();
        yield return Settle(6);
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Path.GetTempPath(), "rackv2-o1e2e");
        Directory.CreateDirectory(output);
        var stack = new Stack<IEnumerator>(); stack.Push(Run()); Exception failure = null;
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) stack.Pop(); else if (current is IEnumerator next) stack.Push(next); else yield return current;
        }
        if (failure != null && manager != null)
        {
            File.WriteAllText(Path.Combine(output, "failure-state.txt"), "planned=" + manager.PlannedRackCount + " marked=" + manager.MarkedRackEntranceCount
                + " confirmed=" + manager.RackPlanConfirmed + " screen=" + ui.CurrentScreen + " help=" + ui.Help.IsOpen + "/" + ui.Help.CurrentMenu
                + " message=" + (marks != null ? marks.TransientMessage : "") + " instruction=" + ui.Learning.CurrentInstruction
                + " player=" + player.transform.position + "\n" + failure);
            SetGameSize(1280, 720);
            yield return Settle(6);
            yield return new WaitForEndOfFrame();
            Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
            if (image != null) { File.WriteAllBytes(Path.Combine(output, "FAIL-state-1280x720.png"), image.EncodeToPNG()); Destroy(image); }
        }
        File.WriteAllLines(Path.Combine(output, "captures.txt"), captures);
        string driver = Environment.GetEnvironmentVariable("CCF_DRIVER_DIR");
        if (!string.IsNullOrEmpty(driver)) File.WriteAllText(Path.Combine(driver, "finished"), failure == null ? "pass" : "fail");
        Note("HELP_FIRST_USE_SEEN", string.Join(",", helpSeen));
        if (failure != null) Debug.Log("RACKV2_O1E2E_FAIL checks=" + checks + " exception=" + failure);
        else if (Diagnostic) Debug.Log("RACKV2_O1E2E_DIAGNOSTIC_DONE checks=" + checks + " defects=" + defects + " shots=" + shots);
        else Debug.Log("RACKV2_O1E2E_PASS checks=" + checks + " shots=" + shots);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(ui != null, "Scenario One ForestTest scene");
        manager = ui.Manager; marks = ui.Marking; player = ui.Player; ecology = ui.Ecology;
        saves = FindFirstObjectByType<ForestSaveController>();
        string config = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        Check(!string.IsNullOrEmpty(config) && Path.GetFullPath(Application.persistentDataPath).StartsWith(Path.GetFullPath(config) + Path.DirectorySeparatorChar, StringComparison.Ordinal),
            "isolated save slot: " + Application.persistentDataPath);
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        Check(!File.Exists(savePath), "the isolated save slot starts empty");
        Note("MODE", mode);

        yield return StartNewGame();
        yield return WalkAndOpenMap();
        yield return PlanEditConfirm();
        yield return ReturnToForest();
        yield return ApprovalBeforeMarking();
        yield return SprayEntrances();
        yield return ApproveExecuteResult();
        yield return SubsequentObjective();
        yield return DiskRoundTrip();
    }

    // ---- 1. a fresh game from the real startup screen ----------------------------------------------------------------
    private IEnumerator StartNewGame()
    {
        StandaloneSessionMenu menu = null;
        for (int frame = 0; frame < 900; frame++)
        {
            menu = FindFirstObjectByType<StandaloneSessionMenu>();
            if (menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive) break;
            yield return null;
        }
        Check(menu != null && StandaloneSessionMenu.IsOpen && menu.StartupScreenActive, "the real startup screen opened");
        yield return Settle(3);
        Check(!menu.ContinueAvailable, "a first-time player has nothing to continue");
        Button start = menu.GetComponent<UIDocument>().rootVisualElement.Query<Button>().ToList().FirstOrDefault(b => b.text == "Start New Scenario");
        yield return Invoke(start, "Start New Scenario");
        yield return Settle(10);
        Check(!StandaloneSessionMenu.IsOpen, "Start New Scenario closed the startup screen");
        Check(ui.Help.Onboarding.IsOpen, "a new scenario opens its premise");
        for (int page = 0; page < 12 && ui.Help.Onboarding.IsOpen; page++) { ui.Help.Onboarding.Next(); yield return Settle(3); }
        Check(!ui.Help.Onboarding.IsOpen, "the premise pages through to the forest");
        yield return CloseHelp();

        Check(ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80 && manager.RackPlanningV2Active
            && manager.RackPlanningModelVersion == RackPlanningModel.RackPlanningFieldMarkingV2, "a new game is the Enlarged80 Rack Planning V2 world");
        Check(manager.PlannedRackCount == 0 && !manager.RackPlanConfirmed && manager.WorkOrders.Count == 0 && Living().Length == 1344,
            "the fresh world has no racks, no work and 1344 living stems");
        Check(manager.RackFirstStageActive && ui.Learning.TeachingRacks && ui.Learning.CurrentTopicIndex == 0, "Objective 1 (access racks) is the current objective");
        string instruction = ui.Learning.CurrentInstruction;
        Note("OBJECTIVE_BEFORE_PLANNING", instruction.Replace("\n", " / "));
        Check(instruction.Contains("Stand Map [M]") && instruction.Contains("Plan Racks") && instruction.Contains("0 / 5"),
            "Objective 1 tells the player to plan on the Stand Map (spec §30 steps 2-3)");

        ui.ShowObjectives(); yield return Settle(20);
        yield return CloseHelp();
        foreach (Foldout foldout in ui.RootElement.Query<Foldout>().ToList()) foldout.value = true;
        yield return Settle(10);
        string objective = Text();
        Check(objective.Contains("Extraction racks allow machinery to reach trees without creating access everywhere across the stand")
            && objective.Contains("16–20 m") && objective.Contains("Plan Racks"), "Objective 1 details explain purpose, conditional spacing and map planning (§30 step 1, §31)");
        Check(!objective.Contains("18 m is the ideal") && !objective.Contains("always required"), "no universal 18 m or five-rack claim (§31)");
        yield return Shoot("c01-objective-1-before-planning");
        ui.CloseAll(); yield return Settle(6);
    }

    // ---- 2. walk, and open the Stand Map with the real M key -------------------------------------------------------
    private IEnumerator WalkAndOpenMap()
    {
        Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
        yield return Settle(6);
        string before = RackState();
        yield return Native(Key("p"));
        Check(!marks.RackSprayEquipped && RackState() == before, "the spray marker cannot be equipped before a confirmed plan (native P)");

        Vector3 start = player.transform.position;
        yield return Native(KeyDown("w"), Sleep(1.5f), KeyUp("w"));
        yield return CloseHelp();
        float walked = Vector2.Distance(new Vector2(start.x, start.z), new Vector2(player.transform.position.x, player.transform.position.z));
        Note("NATIVE_WALK", "metres=" + walked.ToString("0.00") + " from=" + start + " to=" + player.transform.position);
        Check(walked > 0.5f, "the player walks with the real W key (" + walked.ToString("0.00") + " m)");

        Cursor.lockState = CursorLockMode.Locked;
        yield return Native(Key("m"));
        yield return CloseHelp();
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, "the real M key opens the Stand Map");
        Check(ui.Map.RackPlanningAvailable && !ui.Map.RackMode && Named("map-plan-rack") != null && Visible(Named("map-plan-rack")),
            "the map offers Plan Racks as a tool, not yet active");
        yield return Shoot("c02-map-before-planning");
    }

    // ---- 3. plan, edit, confirm on the map ---------------------------------------------------------------------------
    private IEnumerator PlanEditConfirm()
    {
        StandMapRackEditor editor = ui.Map.RackEditor;
        yield return Invoke(Named("map-plan-rack"), "Plan Racks");
        yield return CloseHelp();
        Check(ui.Map.RackMode && editor.Active, "Plan Racks mode is active");
        yield return Invoke(Named("map-edit-reset"), "Reset to suggested layout");
        if (editor.ResetPending) yield return Invoke(Named("map-edit-reset-confirm"), "confirm reset");
        IReadOnlyList<SuggestedRackSegment> suggestion = manager.GetSuggestedRackLayout();
        Check(suggestion.Count == 5 && manager.PlannedRackCount == 5 && manager.RackLayoutMatchesSuggestion(suggestion), "the suggested layout becomes five player racks");
        Check(!manager.RackPlanConfirmed && manager.WorkOrders.Count == 0 && Living().Length == 1344, "drafting creates no work and cuts nothing");
        yield return Shoot("c03-plan-racks-suggested");

        // Delete one rack with the real Delete key: four racks cannot be confirmed.
        int[] ids = manager.RackPlan.Select(r => r.Id).ToArray();
        Check(editor.Select(ids[2]), "a rack can be selected on the map");
        yield return Settle(4);
        yield return Native(Key("Delete"));
        Check(manager.PlannedRackCount == 4 && manager.RackPlan.All(r => r.Id != ids[2]), "the real Delete key removes the selected rack");
        yield return Settle(6);
        Button confirm = Named("map-rack-plan-confirm");
        string fourRacks = RackState();
        Check(confirm != null && !confirm.enabledInHierarchy && ui.RootElement.Q<Label>("map-rack-plan-confirm-reason") != null,
            "Confirm Rack Plan is disabled with a reason for four racks");
        Check(!manager.ConfirmRackPlanFromMap().Succeeded && RackState() == fourRacks && !manager.RackPlanConfirmed, "a four-rack plan is refused and nothing changes");
        yield return Invoke(Named("map-edit-undo"), "Undo");
        Check(manager.PlannedRackCount == 5, "Undo restores the deleted rack");
        Note("UNDO_IDS", "before=[" + string.Join(",", ids) + "] after=[" + string.Join(",", manager.RackPlan.Select(r => r.Id)) + "]");

        // Move one rack a short way. The drag itself is qualified with the real pointer by RACK-V2-05/06; this commits exactly what
        // the editor's pointer release commits (StandMapRackEditor.Apply, Gesture.Move).
        RackPlanRack moved = manager.RackPlan.OrderBy(r => r.Id).First();
        Type gesture = typeof(StandMapRackEditor).GetNestedType("Gesture", BindingFlags.NonPublic);
        MethodInfo apply = typeof(StandMapRackEditor).GetMethod("Apply", AnyInstance);
        Check(gesture != null && apply != null, "the editor's pointer-release commit path exists");
        bool movedOk = false; float usedShift = 0f;
        foreach (float shift in new[] { 1.0f, -1.0f, 0.6f, -0.6f })
        {
            string beforeMove = RackState();
            editor.Select(moved.Id);
            apply.Invoke(editor, new object[] { Enum.Parse(gesture, "Move"), moved.Id,
                new RackPoint(moved.Entrance.X, moved.Entrance.Z + shift), new RackPoint(moved.Far.X, moved.Far.Z + shift) });
            yield return Settle(4);
            if (RackState() != beforeMove && manager.TryGetRack(moved.Id, out RackPlanRack now)
                && Mathf.Abs(now.Entrance.Z - (moved.Entrance.Z + shift)) < 0.05f && manager.CanConfirmRackPlan(out _))
            { movedOk = true; usedShift = shift; break; }
            if (RackState() != beforeMove) { editor.UndoLast(); yield return Settle(4); }
        }
        Check(movedOk, "a rack can be moved and the plan stays confirmable");
        Note("MOVE_EDIT", "rack=" + moved.Id + " shiftZ=" + usedShift + " status=" + editor.Status);
        Check(!manager.RackLayoutMatchesSuggestion(manager.GetSuggestedRackLayout()), "the edited plan differs from the suggestion (a player plan)");
        editor.Select(moved.Id); yield return Settle(6);
        string spacing = editor.SpacingText();
        Note("SPACING", spacing);
        Check(!string.IsNullOrEmpty(spacing) && Text().Contains(spacing.Split('\n')[0]), "the selected rack's spacing is shown on the map");
        yield return Shoot("c04-edited-plan-spacing");

        int livingBefore = Living().Length;
        confirm = Named("map-rack-plan-confirm");
        Check(confirm != null && confirm.enabledInHierarchy, "Confirm Rack Plan is enabled for five valid racks");
        yield return Invoke(confirm, "Confirm Rack Plan");
        yield return Settle(6);
        Check(manager.RackPlanConfirmed && manager.ConfirmedRacks.Count == 5, "the plan is confirmed with five racks");
        int union = manager.ConfirmedRacks.Sum(r => r.RemovalTreeIds.Count);
        Check(manager.WorkOrders.Count(o => o.rackId > 0) == union && manager.WorkOrders.All(o => o.rackId > 0 && o.status == ScenarioWorkStatus.Pending),
            "confirmation creates one Pending rack-owned order per frozen removal tree and nothing else (" + union + ")");
        Check(Living().Length == livingBefore && manager.MarkedRackEntranceCount == 0, "confirmation fells nothing and marks nothing");
        Check(manager.ConfirmedRacks.All(r => !string.IsNullOrEmpty(r.EntranceTreeId) && r.RemovalTreeIds.Contains(r.EntranceTreeId)),
            "every confirmed rack has an entrance tree inside its own removal set");
        Note("ENTRANCES", string.Join(",", manager.ConfirmedRacks.Select(r => r.Id + ":" + r.EntranceTreeId)));
        Note("LESSONS_AFTER_CONFIRM", "create=" + ui.Learning.IsDone("rack.create") + " layout=" + ui.Learning.IsDone("rack.layout")
            + " approve=" + ui.Learning.IsDone("rack.approve"));
        yield return Wait(0.4f);
        Check(ui.Learning.IsDone("rack.create"), "lesson rack.create is recorded");
        Expect(!ui.Learning.IsDone("rack.layout"), "the rack-layout step is not complete until the five entrances are marked (§30 step 7)");
        yield return Shoot("c05-confirmed-map");
    }

    // ---- 4. back to the forest: Tree Marks help, counters, cue ----------------------------------------------------------
    private IEnumerator ReturnToForest()
    {
        yield return Native(Key("m"));
        yield return Settle(10);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, "the real M key closes the map");
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeMarks, "Tree Marks help introduces itself when field marking becomes relevant");
        string help = Text();
        Check(help.Contains("Orange") && help.Contains("Red") && help.Contains("Blue"), "Tree Marks help lists orange, red and blue");
        yield return Shoot("c06-tree-marks-help");
        yield return CloseHelp();
        Check(!ui.Help.IsOpen, "Tree Marks help closes");
        string instruction = ui.Learning.CurrentInstruction;
        Note("OBJECTIVE_AFTER_CONFIRM", instruction.Replace("\n", " / "));
        Check(instruction.Contains("mark the first rack tree nearest the path") && instruction.Contains("[P]"), "Objective 1 now asks for field marking");
        yield return Settle(6);
        Check(marks.RackFieldStatus.StartsWith("Rack entrances marked 0 / 5", StringComparison.Ordinal), "the derived marked-entrance counter starts at 0 / 5");
        Check(marks.CueRackId == manager.ConfirmedRacks.OrderBy(r => r.Id).First().Id, "the entrance cue points at the first unmarked rack");
    }

    // ---- 5. D-064 / OPEN-2: field marking occurs before approval --------------------------------------------------------
    private IEnumerator ApprovalBeforeMarking()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yield return Native(Key("Tab"));
        yield return CloseHelp();
        Check(manager.WorkPlanOpen, "the real Tab key opens the Work Plan");
        yield return Wait(0.4f);
        ForestSaveData snapshot = saves.CaptureData();
        string before = JsonUtility.ToJson(snapshot);
        Button approve = ButtonStarts("Approve Work Plan");
        Check(approve != null, "the Work Plan shows Approve Work Plan");
        bool buttonEnabled = approve.enabledInHierarchy;
        bool approved = ui.ApproveWorkPlan();
        yield return Settle(4);
        int approvedOrders = manager.WorkOrders.Count(o => o.status == ScenarioWorkStatus.Approved);
        Note("APPROVE_BEFORE_MARKING", "buttonEnabled=" + buttonEnabled + " approved=" + approved + " approvedOrders=" + approvedOrders
            + " marked=" + manager.MarkedRackEntranceCount + " feedback=" + Text().Split('\n').FirstOrDefault(l => l.Contains("rack entrance")));
        Expect(!buttonEnabled, "Approve Work Plan is unavailable while rack entrances are unmarked (D-064: marking before approval)");
        Expect(!approved && approvedOrders == 0 && JsonUtility.ToJson(saves.CaptureData()) == before, "approval before marking is refused and changes nothing");
        if (!Diagnostic) Check(Text().Contains("Mark the rack entrances"), "the Work Plan says why: mark the rack entrances first");
        yield return Shoot("c07-workplan-before-marking");
        if (approved)
        {
            // Diagnostic only: put the world back exactly as it was so the rest of the flow is still the real sequence.
            Check(saves.LoadData(snapshot, false), "diagnostic: restore the pre-approval state");
            yield return Settle(10);
            Check(manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Pending), "diagnostic: rack work is Pending again");
        }
        yield return Native(Key("Tab"));
        yield return CloseHelp();
        Check(!manager.WorkPlanOpen, "the real Tab key closes the Work Plan");
    }

    // ---- 6. spray the five entrance trees with the real P key and left button -----------------------------------------
    private IEnumerator Aim(ForestTree tree, bool requireValid)
    {
        var controller = player.GetComponent<CharacterController>();
        foreach (Vector3 direction in new[] { Vector3.left, Vector3.back, Vector3.forward, new Vector3(-1, 0, -1), new Vector3(-1, 0, 1), Vector3.right })
            foreach (float distance in new[] { 2.2f, 1.8f, 2.8f })
            {
                if (controller != null) controller.enabled = false;
                Vector3 p = tree.transform.position + direction.normalized * distance;
                player.transform.position = new Vector3(p.x, tree.transform.position.y + 0.2f, p.z);
                if (controller != null) controller.enabled = true;
                player.LookToward(tree.transform.position + Vector3.up * 1.4f);
                Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false;
                Physics.SyncTransforms();
                yield return Settle(10);
                player.LookToward(tree.transform.position + Vector3.up * 1.4f);
                yield return Settle(3);
                if (!requireValid || marks.RackSprayPrompt().Contains("Spray orange")) yield break;
            }
        if (requireValid) throw new InvalidOperationException("no clear spray vantage for " + tree.TreeId + ": " + marks.RackSprayPrompt());
    }

    private IEnumerator SprayEntrances()
    {
        List<RackPlanRack> racks = manager.ConfirmedRacks.OrderBy(r => r.Id).ToList();
        for (int i = 0; i < racks.Count; i++)
        {
            RackPlanRack rack = manager.ConfirmedRacks.First(r => r.Id == racks[i].Id);
            Check(marks.TryGetRackFieldTarget(out RackPlanRack cued, out ForestTree target, out string problem) && cued.Id == rack.Id && target.TreeId == rack.EntranceTreeId,
                "rack " + rack.Id + " resolves its own entrance tree " + rack.EntranceTreeId + " " + problem);
            Check(marks.CueRackId == rack.Id, "the cue has moved to rack " + rack.Id);
            if (i == 0)
            {
                yield return Aim(target, false);
                yield return Native(Key("p"));
                Check(marks.RackSprayEquipped, "the real P key equips the spray marker after confirmation");
                yield return Aim(target, true);
                yield return Shoot("c08-first-entrance-target");

                // Negative: an unrelated tree and a later removal tree of this rack cannot be marked.
                ForestTree unrelated = Living().Where(t => !manager.Racks.Any(r => r.removalTreeIds.Contains(t.TreeId)))
                    .OrderBy(t => (t.transform.position - target.transform.position).sqrMagnitude).First();
                yield return Aim(unrelated, false);
                yield return Native(Click());
                Check(manager.MarkedRackEntranceCount == 0 && marks.TransientMessage.Contains("not a rack entrance"), "spraying an unrelated tree is refused");
                ForestTree later = rack.RemovalTreeIds.Where(id => id != rack.EntranceTreeId).Select(Tree).Where(t => t.IsLiving)
                    .OrderBy(t => (t.transform.position - target.transform.position).sqrMagnitude).FirstOrDefault();
                if (later != null)
                {
                    yield return Aim(later, false);
                    yield return Native(Click());
                    Check(manager.MarkedRackEntranceCount == 0, "spraying a later removal tree in the same rack is refused (" + later.TreeId + ")");
                }
            }
            yield return Aim(target, true);
            yield return Native(Click());
            Check(manager.MarkedRackEntranceCount == i + 1, "native spray marks rack " + rack.Id + " (" + (i + 1) + " / 5)");
            Check(manager.ConfirmedRacks.First(r => r.Id == rack.Id).EntranceMarked && target.transform.Find("Orange Rack Entrance Mark") != null,
                "rack " + rack.Id + " records the mark and its entrance tree carries the orange paint");
            if (i == 0)
            {
                yield return Native(Click());
                Check(manager.MarkedRackEntranceCount == 1, "spraying the same entrance again does not count twice");
            }
            Check(manager.WorkOrders.All(o => o.status == ScenarioWorkStatus.Pending), "marking approves and executes nothing");
            Check(i == racks.Count - 1 || !ui.Learning.IsDone("rack.approve"), "rack.approve is not recorded while marking is incomplete");
        }
        yield return Settle(8);
        Check(manager.MarkedRackEntranceCount == 5 && manager.PlannedRackCount == 5, "five entrances marked; planned and marked counters are separate (5 / 5, 5 / 5)");
        Check(marks.CueRackId == 0 && marks.RackFieldStatus.StartsWith("Rack entrances marked 5 / 5", StringComparison.Ordinal), "the cue is gone and the counter reads 5 / 5");
        yield return Wait(0.4f);
        string instruction = ui.Learning.CurrentInstruction;
        Note("OBJECTIVE_AFTER_MARKING", instruction.Replace("\n", " / "));
        Check(instruction.Contains("review and approve the rack removal work"), "Objective 1 hands over to approval once five entrances are marked");
        Check(ui.Learning.IsDone("rack.layout"), "the rack-layout step completes at five marked entrances (§30 step 7)");
        yield return Shoot("c09-five-entrances-marked");
        yield return Native(Key("p"));
        Check(!marks.RackSprayEquipped, "the real P key stows the spray marker");
    }

    // ---- 7. rack.approve -> execution -> rack.result ------------------------------------------------------------------
    private IEnumerator ApproveExecuteResult()
    {
        Cursor.lockState = CursorLockMode.Locked;
        yield return Native(Key("Tab"));
        yield return CloseHelp();
        Check(manager.WorkPlanOpen, "the real Tab key opens the Work Plan");
        yield return Wait(0.4f);
        List<string> union = manager.ConfirmedRacks.SelectMany(r => r.RemovalTreeIds).OrderBy(i => i, StringComparer.Ordinal).ToList();
        Button approve = ButtonStarts("Approve Work Plan");
        Check(approve != null && approve.enabledInHierarchy, "Approve Work Plan is available once the entrances are marked");
        yield return Invoke(approve, "Approve Work Plan");
        yield return Wait(0.4f);
        var approved = manager.WorkOrders.Where(o => o.status == ScenarioWorkStatus.Approved).ToList();
        Check(approved.Count == union.Count && approved.All(o => o.rackId > 0)
            && approved.Select(o => o.targetTreeId).OrderBy(i => i, StringComparer.Ordinal).SequenceEqual(union)
            && manager.WorkOrders.All(o => o.status != ScenarioWorkStatus.Pending), "approval approves exactly the frozen rack trees");
        Check(ui.Learning.IsDone("rack.approve"), "lesson rack.approve is recorded after approval");
        yield return Shoot("c10-workplan-approved");

        ScenarioHarvestJob quote = manager.GetHarvestQuote(true);
        long expectedNet = quote.RevenueCents - quote.CostCents, cashBefore = manager.CashCents;
        int livingBefore = Living().Length, year = ecology.EcologicalYear;
        yield return Invoke(ButtonStarts("Advance one year"), "Advance one year");
        yield return Settle(10);
        Check(ecology.EcologicalYear == year + 1, "one year advanced (" + year + " -> " + ecology.EcologicalYear + ")");
        Check(Living().Length == livingBefore - union.Count, "execution felled exactly the rack trees: " + livingBefore + " -> " + Living().Length);
        HashSet<string> stumps = new HashSet<string>(AllTrees().Where(t => t.IsStump).Select(t => t.TreeId));
        Check(union.All(stumps.Contains), "every frozen rack tree is a stump");
        Check(manager.CashCents - cashBefore == expectedNet, "cash change equals the quoted net (" + (manager.CashCents - cashBefore) + ")");
        Check(manager.WorkOrders.Where(o => o.rackId > 0).All(o => o.status == ScenarioWorkStatus.Completed)
            && manager.ConfirmedRacks.All(r => r.IsLocked), "all rack work is Completed and every rack is locked");
        Check(manager.PlannedRackCount == 5 && manager.MarkedRackEntranceCount == 5, "counters are unchanged by execution");
        yield return Wait(0.8f);
        yield return Shoot("c11-annual-review");
        Button acknowledge = ui.RootElement.Q<Button>("annual-review-acknowledge");
        if (acknowledge != null && Visible(acknowledge)) yield return Invoke(acknowledge, "acknowledge annual review");
        ui.CloseAll(); yield return Settle(6);
        yield return CloseHelp();
        yield return Wait(0.8f);
        Check(ui.Learning.IsDone("rack.result"), "lesson rack.result is recorded after execution");
        Check(manager.Interventions.Count == 1 && manager.AnnualReports.Count >= 1, "one intervention and its annual report are recorded");
    }

    // ---- 8. the next objective is reachable on the player's confirmed racks --------------------------------------------
    private IEnumerator SubsequentObjective()
    {
        Check(!manager.RackFirstStageActive && !ui.Learning.TeachingRacks && ui.Learning.CurrentTopicIndex == 1, "the rack stage ends; Objective 2 is current");
        Check(ui.Learning.Summary.Contains(ScenarioOneTeachingSequence.Lessons[1].Title), "the cue names lesson 2: " + ui.Learning.Summary.Replace("\n", " / "));
        var sample = ScenarioOneRackSampleGeometry.Derive(manager.ConfirmedRacks, StandGeometryModel.StandSizeMeters(StandGeometryModel.Enlarged80) * 0.5f);
        Check(sample != null && sample.Valid && sample.Racks.Count == 5 && sample.TeachingRack.Id == sample.Racks[ScenarioOneRackSampleGeometry.TeachingRackIndex].Id,
            "the teaching area is the central confirmed rack of the player's plan");
        var bay = ScenarioOneRackBayReader.Read(AllTrees(), manager.WorkOrders, sample);
        Check(bay != null && bay.Applies, "the Objectives 2-5 teaching area applies to the confirmed plan");
        Note("TEACHING_RACK", "id=" + sample.TeachingRack.Id + " ordered=[" + string.Join(",", sample.Racks.Select(r => r.Id)) + "]");
        Check(string.IsNullOrEmpty(ui.RackPlanProblem), "no rack safeguard blocks the next objective");

        ForestTree aimed = null;
        foreach (ForestTree t in Living().Where(t => !t.TreeId.StartsWith("Q", StringComparison.Ordinal) && bay.Contains(t.transform.position.x, t.transform.position.z)).Take(40))
        {
            player.transform.position = t.transform.position - new Vector3(0, 0, 3);
            player.LookToward(t.transform.position + Vector3.up * 1.5f);
            Cursor.lockState = CursorLockMode.Locked;
            yield return Settle(8);
            if (marks.AimedTree == t) { aimed = t; break; }
        }
        Check(aimed != null && player.InteractionPromptText().Contains("[X] Fell") && player.InteractionPromptText().Contains("[C] Crop Tree"),
            "lesson 2's tree marking is offered in the teaching area");
        yield return Native(Key("c"));
        yield return CloseHelp();
        Check(aimed.IsCropTree, "the real C key designates a Crop Tree in Objective 2");
        yield return Shoot("c12-objective-2-crop-tree");
        yield return Native(Key("c"));
        Check(!aimed.IsCropTree, "the real C key clears it again (no state left behind)");
    }

    // ---- 9. the completed Objective 1 state survives a real disk round trip ---------------------------------------------
    private IEnumerator DiskRoundTrip()
    {
        string signature = Signature();
        saves.Save();
        yield return Settle(4);
        Check(File.Exists(savePath), "Save() wrote the isolated save file");
        ForestSaveData onDisk = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(savePath));
        Check(onDisk.version == ForestSaveData.CurrentVersion && ForestSaveValidation.ValidateRackV2(onDisk) == null && onDisk.scenarioOne.racks.Count == 5,
            "the save is current schema v" + ForestSaveData.CurrentVersion + " and passes V2 rack validation");
        Check(saves.CanLoad(out string problem), "the save is loadable: " + problem);
        saves.Load();
        yield return Settle(12);
        yield return CloseHelp();
        Check(Signature() == signature, "rack ids, geometry, marks, order states and locks are exact after load");
        yield return Wait(0.8f);
        Check(ui.Learning.CurrentTopicIndex == 1 && manager.MarkedRackEntranceCount == 5, "after load Objective 2 is still current and 5 / 5 entrances are marked");
    }

    private string Signature()
    {
        var parts = new List<string>();
        foreach (RackPlanRack rack in manager.RackPlan)
            parts.Add(rack.Id + ":" + rack.Entrance.X.ToString("R") + "/" + rack.Entrance.Z.ToString("R") + ":" + rack.Far.X.ToString("R") + "/" + rack.Far.Z.ToString("R")
                + ":" + rack.Status + ":" + rack.EntranceTreeId + ":" + rack.EntranceMarked + ":" + string.Join(",", rack.RemovalTreeIds) + ":" + rack.Orders);
        foreach (ScenarioOneWorkOrder order in manager.WorkOrders.OrderBy(o => o.workOrderId))
            parts.Add(order.workOrderId + ":" + order.status + ":" + order.targetTreeId + ":" + order.rackId);
        parts.Add("planned=" + manager.PlannedRackCount + " marked=" + manager.MarkedRackEntranceCount + " confirmed=" + manager.RackPlanConfirmed
            + " next=" + manager.NextRackId + " model=" + manager.RackPlanningModelVersion);
        return string.Join("|", parts);
    }
}
#endif
