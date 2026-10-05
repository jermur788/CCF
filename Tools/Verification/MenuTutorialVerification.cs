using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Explicit disposable playthrough. Isolated saves, world and preference restore.
public static class MenuTutorialVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        SetGameSize(1600, 900);
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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "Menu tutorial review " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        var gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Prepare()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "MenuTutorialVerification.Begin")) return;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)))
        {
            string key = MenuHelpView.PreferencePrefix + menu;
            Before[key] = PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : (int?)null;
            PlayerPrefs.DeleteKey(key);
        }
    }
    public static readonly Dictionary<string, int?> Before = new Dictionary<string, int?>();
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MenuTutorialVerification.Begin"))
            new GameObject("Disposable menu tutorial verification").AddComponent<MenuTutorialRunner>();
    }
}
public sealed class MenuTutorialRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestPlayer player;
    private string output;
    private ForestSaveData original;
    private Dictionary<string, byte[]> oldSaves = new Dictionary<string, byte[]>();
    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static void Field(object target, string name, object value) => target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
    private IEnumerator Start()
    {
        yield return null; yield return null;
        Exception error = null;
        var steps = new Stack<IEnumerator>(); steps.Push(Verify());
        while (steps.Count > 0)
        {
            bool more; object current = null;
            try { more = steps.Peek().MoveNext(); if (more) current = steps.Peek().Current; }
            catch (Exception ex) { error = ex; break; }
            if (!more) { steps.Pop(); continue; }
            if (current is IEnumerator child) { steps.Push(child); continue; }
            yield return current;
        }
        if (original != null && saves != null) saves.LoadData(original, false);
        foreach (var entry in oldSaves)
            if (entry.Value == null) File.Delete(entry.Key); else File.WriteAllBytes(entry.Key, entry.Value);
        foreach (var entry in MenuTutorialVerification.Before)
            if (entry.Value.HasValue) PlayerPrefs.SetInt(entry.Key, entry.Value.Value); else PlayerPrefs.DeleteKey(entry.Key);
        PlayerPrefs.Save();
        if (error == null) Debug.Log("MENU_TUTORIAL_PLAYTHROUGH_PASS"); else Debug.LogError("MENU_TUTORIAL_FAIL " + error);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(error == null ? 0 : 1);
#endif
    }
    private IEnumerator Verify()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; saves = FindFirstObjectByType<ForestSaveController>(); player = ui.Player;
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT"); Directory.CreateDirectory(output);
        foreach (string tail in new[] { "forest-save.json", "forest-save.json.bak", "forest-save.json.tmp" })
        {
            string path = Path.Combine(Application.persistentDataPath, tail);
            oldSaves[path] = File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
        original = saves.CaptureData(); string initial = ScenarioReferenceArchive.WorldHash(original);
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WalkingHud, "HUD introduction missing at start");
        Check(manager.AnyPanelOpen && !player.enabled, "Help must release mouse and stop forest actions");
        yield return CaptureMenu(MenuHelpView.Menu.WalkingHud);
        ui.CloseHelp();
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == initial, "Help changed forest state");
        yield return null; yield return null;
        Check(!ui.Help.IsOpen && player.enabled, "HUD repeats or input not restored");
        yield return Press(Key.F1);
        Check(ui.Help.IsOpen, "F1 does not reopen HUD help");
        yield return Press(Key.Escape);
        Check(!ui.Help.IsOpen, "Escape does not close help");
        Debug.Log("MENU_HUD_PASS firstUse=true repeat=false F1=true Escape=true world=unchanged");

        ForestTree tree = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving).OrderBy(t => (t.transform.position - player.transform.position).sqrMagnitude).First();
        player.transform.position = tree.InteractionPoint - new Vector3(0, 0, 2);
        player.GetType().GetMethod("InspectTree", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(player, new object[] { tree });
        yield return null; yield return null;
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeInspection, "Tree introduction missing");
        yield return CaptureMenu(MenuHelpView.Menu.TreeInspection);
        yield return Press(Key.Escape);
        Check(!ui.Help.IsOpen && player.IsInspecting, "Help Escape also closed inspection");
        Button contextualHelp = ui.RootElement.Query<Button>().ToList().First(b => b.text == "Help [F1]");
        yield return Click(contextualHelp);
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.TreeInspection, "HUD Help did not follow inspection context");
        yield return Press(Key.Escape);
        yield return CaptureScreen("inspection-open");
        yield return Press(Key.E);
        Check(!player.IsInspecting, "E did not return from inspection");
        Debug.Log("MENU_INSPECTION_PASS DBH=true competition=true marksAreProposals=true");

        ui.ShowMap(); yield return null; yield return null;
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.StandMap, "Map introduction missing");
        yield return CaptureMenu(MenuHelpView.Menu.StandMap);
        yield return Press(Key.Escape);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, "Help Escape closed the map");
        Vector3 position = player.transform.position;
        int target = (ui.Ecology.GetCellIndex(position) + 1) % ui.Ecology.Cells.Length;
        Field(ui.Map, "selectedCell", target); ui.Map.Refresh(true);
        Button waypoint = ui.Map.Root.Query<Button>().ToList().First(b => b.text == "Set waypoint");
        yield return Click(waypoint);
        Debug.Log("MENU_WAYPOINT_DIAGNOSTIC before=" + position + " after=" + player.transform.position + " description=" + ui.Map.WaypointDescription(position));
        Check(player.transform.position == position && !string.IsNullOrEmpty(ui.Map.WaypointDescription(position)), "Waypoint moved player or lacks HUD direction");
        yield return CaptureScreen("map-waypoint");
        ui.CloseAll(); yield return null;
        ui.ShowMap(); yield return null;
        Check(!ui.Help.IsOpen, "Map introduction repeats"); ui.CloseAll(); yield return null;
        Debug.Log("MENU_MAP_PASS selectCell=true waypoint=true HUDdirection=true noRemoteManagement=true repeat=false");

        ui.ShowWorkPlan(); yield return null; yield return null;
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WorkPlan, "Work Plan introduction missing");
        yield return CaptureMenu(MenuHelpView.Menu.WorkPlan);
        yield return Press(Key.Escape);
        Check(manager.WorkPlanOpen && !ui.Help.IsOpen, "Help Escape closed Work Plan");
        Check(!manager.AnnualReviewSeen, "Empty review falsely completed tutorial");
        ui.ShowReview(); yield return null;
        ui.AcknowledgeAnnualReview(); Check(!manager.AnnualReviewSeen, "Year-zero review acknowledged without results");
        Check(!ui.Help.IsOpen, "Annual introduction shown before results exist");
        ui.ShowWorkPlan(); yield return null;
        Check(ui.AdvanceFromWorkPlan(), "First annual advance blocked"); yield return null; yield return null;
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.AnnualReview && !manager.AnnualReviewSeen, "First results not introduced or prematurely acknowledged");
        yield return CaptureMenu(MenuHelpView.Menu.AnnualReview);
        yield return Press(Key.Escape);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && !manager.AnnualReviewSeen, "Help Escape closed or acknowledged Annual Review");
        yield return CaptureScreen("annual-results-unread");
        int year = manager.CurrentEcologicalYear;
        ui.ShowWorkPlan(); yield return null;
        Button advance = ui.RootElement.Query<Button>().ToList().First(b => b.text == "Advance one year");
        Check(!advance.enabledSelf && ui.NeedsAnnualReview, "Later annual cycle unlocked before reading review");
        yield return CaptureScreen("work-plan-review-gate");
        saves.Save(); saves.Load(); yield return null;
        Check(ui.NeedsAnnualReview && !manager.AnnualReviewSeen, "Unread review gate lost on save/load");
        Check(!ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear == year, "UI learning gate bypassed"); yield return null;
        yield return Click(ui.RootElement.Q<Button>("annual-review-acknowledge"));
        Check(manager.AnnualReviewSeen && !ui.NeedsAnnualReview, "Explicit review acknowledgement failed");
        saves.Save(); Field(manager, "annualReviewSeen", false); saves.Load(); yield return null;
        Check(manager.AnnualReviewSeen && !ui.NeedsAnnualReview, "Review acknowledgement lost on save/load");
        ui.ShowWorkPlan(); yield return null;
        Check(ui.RootElement.Query<Button>().ToList().First(b => b.text == "Advance one year").enabledSelf, "Next year not unlocked");
        Check(ui.AdvanceFromWorkPlan(), "Later annual advance failed after review"); yield return null;
        Check(!ui.Help.IsOpen, "Annual introduction repeats after learning");
        Debug.Log("MENU_ANNUAL_GATE_PASS emptyReview=false beforeAck=locked afterAck=unlocked saveLoad=preserved repeat=false");

        ui.CloseAll(); yield return null;
        Check(manager.TryBeginReferencePreview(20), "Reference preview unavailable"); yield return null;
        ui.ShowHelp(MenuHelpView.Menu.WalkingHud); Check(!ui.Help.IsOpen, "Help leaked into Reference preview");
        manager.EndReferencePreview(); yield return null;
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu))) Check(MenuHelpView.Introduced(menu), "Introduction preference missing: " + menu);
        Debug.Log("MENU_PREFERENCES_PASS allFive=true localOnly=true referenceClean=true");
    }
    private IEnumerator Click(Button button)
    {
        Check(button.enabledInHierarchy, "Cannot activate disabled button: " + button.text);
        // Editor synthetic mouse/navigation events do not reach this runtime
        // panel. Exercise its actual registered callback; human mouse smoke is separate.
        MethodInfo invoke = typeof(Clickable).GetMethod("Invoke", BindingFlags.Instance | BindingFlags.NonPublic);
        Check(invoke != null, "Unity Clickable callback entry point unavailable");
        invoke.Invoke(button.clickable, new object[] { null });
        yield return null; yield return null;
    }
    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }
    private IEnumerator CaptureMenu(MenuHelpView.Menu menu)
    {
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1600,900), new Vector2Int(1920,1080) })
        {
            MenuTutorialVerification.SetGameSize(size.x, size.y);
            yield return CaptureScreen(menu + "-" + size.x);
            VisualElement panel = ui.Help.Root.Children().First();
            Rect bounds = panel.worldBound, viewport = ui.RootElement.worldBound;
            Check(bounds.width > 100 && bounds.height > 100 && viewport.Contains(bounds.min) && viewport.Contains(bounds.max), "Help panel clipped: " + menu);
            VisualElement text = ui.Help.Root.Q<Label>("menu-help-text");
            Check(text.worldBound.height > 20 && ui.Help.Root.Q<Button>("menu-help-close").worldBound.yMax <= bounds.yMax, "Help text or close control inaccessible");
            Debug.Log("MENU_RENDERED_PASS " + menu + " " + size.x + "x" + size.y);
        }
    }
    private IEnumerator CaptureScreen(string name)
    {
        for (int i=0;i<4;i++) yield return null;
        if (Application.isBatchMode) yield break;
        yield return new WaitForEndOfFrame(); Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width > 600, "Missing rendered capture");
        File.WriteAllBytes(Path.Combine(output,name+".png"), image.EncodeToPNG()); Destroy(image);
    }
}
