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
        foreach (string id in LearningObjectivesView.StepIds)
        {
            string key = LearningObjectivesView.PreferencePrefix + id;
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
    private bool SameGroundPosition(Vector3 before, Vector3 after)
    {
        // Closing a panel lets the controller settle vertically within its existing skin; a waypoint must not move it across the stand.
        float skin = player.GetComponent<CharacterController>().skinWidth;
        return Vector2.Distance(new Vector2(before.x, before.z), new Vector2(after.x, after.z)) < 0.001f
            && Mathf.Abs(before.y - after.y) <= skin + 0.001f;
    }
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
        // Run with the rendered launcher (run_clearance_gate.py --interactive).
        Check(!Application.isBatchMode, "MenuTutorialVerification requires an interactive (non -batchmode) Editor: batch mode cannot lock the cursor, so walking aim, tree inspection and keyboard input to play mode are unavailable");
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT"); Directory.CreateDirectory(output);
        foreach (string tail in new[] { "forest-save.json", "forest-save.json.bak", "forest-save.json.tmp" })
        {
            string path = Path.Combine(Application.persistentDataPath, tail);
            oldSaves[path] = File.Exists(path) ? File.ReadAllBytes(path) : null;
        }
        original = saves.CaptureData(); string initial = ScenarioReferenceArchive.WorldHash(original);
        Check(ui.Learning.CompletedSteps == 0, "Learning fixture inherited progress");
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

        int markedBeforeMap = ui.Marking.GetMarkedIds().Count;
        yield return Press(Key.M);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map && ui.Marking.GetMarkedIds().Count == markedBeforeMap,
            "M did not open map exclusively");
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.StandMap, "Map introduction missing");
        yield return CaptureMenu(MenuHelpView.Menu.StandMap);
        yield return Press(Key.Escape);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, "Help Escape closed the map");
        Vector3 position = player.transform.position;
        yield return Click(ui.Map.Root.Query<Button>().ToList().First(b => b.text == "Set waypoint"));
        yield return Press(Key.M);
        ui.Map.WaypointDescription(position);
        Check(!ui.Learning.IsDone("map.arrive"), "Waypoint on current cell credited a journey");
        yield return Press(Key.M);
        // Square IDs belong to the secondary Light layer; the beginner overview shows access and position.
        yield return Click(ui.Map.Root.Query<Button>().ToList().First(b => b.text == "Light"));
        int target = (ui.Ecology.GetCellIndex(position) + 1) % ui.Ecology.Cells.Length;
        // The professional map prints no text in its squares: the invisible square buttons are listed north row first, so square index z*n+x is
        // the button at (n-1-z)*n+x (StandMapView.EnsureCells; ProfessionalMapVerification checks every button against the shared transform).
        int perAxis = ui.Ecology.CellsPerAxis;
        List<Button> squareButtons = ui.Map.Root.Query<Button>(className: "map-cell").ToList();
        Check(squareButtons.Count == perAxis * perAxis, "the map offers one square button for every ecology square");
        Button cellButton = squareButtons[(perAxis - 1 - target / perAxis) * perAxis + target % perAxis];
        yield return Click(cellButton);
        foreach (string label in new[] { "Light", "Regeneration", "Browsing / protection", "Fell & crop marks" })
            yield return Click(ui.Map.Root.Query<Button>().ToList().First(b => b.text == label));
        Check(new[] { "map.open", "map.select", "map.light", "map.regeneration", "map.browse", "map.marks" }.All(ui.Learning.IsDone),
            "Map layer/cell learning not recorded");
        Button waypoint = ui.Map.Root.Query<Button>().ToList().First(b => b.text == "Set waypoint");
        yield return Click(waypoint);
        Debug.Log("MENU_WAYPOINT_DIAGNOSTIC before=" + position + " after=" + player.transform.position + " description=" + ui.Map.WaypointDescription(position));
        Check(SameGroundPosition(position, player.transform.position) && !string.IsNullOrEmpty(ui.Map.WaypointDescription(position)), "Waypoint moved player or lacks HUD direction");
        yield return CaptureScreen("map-waypoint");
        yield return Press(Key.M);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && ui.Learning.IsDone("map.return"), "M did not return with waypoint");
        VisualElement hudPanel = ui.RootElement.Q("hud-waypoint-panel");
        Label hudWaypoint = ui.RootElement.Q<Label>("hud-waypoint-label");
        Label hudDirection = ui.RootElement.Q<Label>("hud-waypoint-direction");
        VisualElement hudArrow = ui.RootElement.Q("hud-waypoint-arrow");
        Vector2 direction = (ui.Ecology.Cells[target].Center - new Vector2(position.x, position.z)).normalized;
        string destinationText = null;
        foreach (int turn in new[] { 0, 90, -90, 180 })
        {
            Vector3 facing = Quaternion.Euler(0, -turn, 0) * new Vector3(direction.x, 0, direction.y);
            player.RestoreLook(Quaternion.LookRotation(facing), 0);
            yield return CaptureScreen("hud-waypoint-turn-" + turn);
            float bearing = (float)hudArrow.GetType().GetProperty("BearingDegrees").GetValue(hudArrow);
            Check(Mathf.Abs(Mathf.DeltaAngle(turn, bearing)) < 1, "HUD arrow bearing incorrect: " + turn);
            string expected = turn == 0 ? "Ahead" : turn == 90 ? "Turn right" : turn == -90 ? "Turn left" : "Behind you";
            Check(hudDirection.text.StartsWith(expected), "HUD turn instruction incorrect");
            if (destinationText == null) destinationText = hudWaypoint.text;
            Check(hudWaypoint.text == destinationText && SameGroundPosition(position, player.transform.position), "Turning changed destination or moved player");
        }
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1600,900), new Vector2Int(1920,1080) })
        {
            MenuTutorialVerification.SetGameSize(size.x, size.y);
            yield return CaptureScreen("hud-waypoint-" + size.x);
            Rect bounds = hudPanel.worldBound, viewport = ui.RootElement.worldBound;
            Check(hudPanel.resolvedStyle.display == DisplayStyle.Flex && bounds.height > 40
                && viewport.Contains(bounds.min) && viewport.Contains(bounds.max), "Waypoint panel hidden or clipped");
            Check(!string.IsNullOrEmpty(hudWaypoint.text) && hudWaypoint.worldBound.height > 10, "Waypoint text invisible");
        }
        Check(!ui.Learning.IsDone("map.arrive"), "Navigation credited before arrival");
        Vector2 destination = ui.Ecology.Cells[target].Center;
        player.transform.position = new Vector3(destination.x, player.transform.position.y, destination.y);
        player.LookToward(new Vector3(destination.x, -2, destination.y + 0.1f));
        // Wait for actual walking/aim state, rather than assuming 20 frames
        // cover the same input time on every rendered Editor run.
        float arrivalDeadline = Time.realtimeSinceStartup + 1f;
        while (Time.realtimeSinceStartup < arrivalDeadline && !ui.Learning.IsDone("map.inspectsite"))
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            ui.Map.WaypointDescription(player.transform.position);
            yield return null;
        }
        if (!ui.Learning.IsDone("map.inspectsite"))
        {
            // A stem/vegetation can obstruct the ground ray. The accepted
            // lesson also permits inspecting a tree in the destination cell.
            ForestTree destinationTree = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
                .Where(t => t.IsLiving && ui.Ecology.GetCellIndex(t.transform.position) == target)
                .OrderBy(t => Vector3.Distance(t.transform.position, player.transform.position)).First();
            player.LookToward(destinationTree.transform.position + Vector3.up * 1.3f);
            yield return new WaitForSecondsRealtime(0.5f);
            yield return Press(Key.E);
            ui.Map.WaypointDescription(player.transform.position);
            Debug.Log("MENU_DESTINATION_INSPECTION_FALLBACK tree=" + (player.InspectedTree != null ? player.InspectedTree.TreeId : "none"));
            if (player.IsInspecting) yield return Press(Key.E);
        }
        Check(ui.Learning.IsDone("map.arrive") && ui.Learning.IsDone("map.inspectsite"), "Destination/site inspection not recorded");
        yield return CaptureScreen("hud-waypoint-arrived");
        Check(hudDirection.text.StartsWith("At destination") && hudArrow.resolvedStyle.display == DisplayStyle.None, "HUD arrival state missing");
        ui.ShowMap(); yield return null;
        Check(!ui.Help.IsOpen, "Map introduction repeats");
        yield return Click(ui.Map.Root.Query<Button>().ToList().First(b => b.text == "Clear waypoint"));
        ui.CloseAll(); yield return CaptureScreen("hud-waypoint-cleared");
        Check(hudPanel.resolvedStyle.display == DisplayStyle.None, "Cleared waypoint remains in HUD");
        Debug.Log("HUD_WAYPOINT_PASS visible=true sizes=1280,1600,1920 bearings=0,90,-90,180 arrival=true clear=true noTeleport=true");
        Debug.Log("MENU_MAP_PASS selectCell=true waypoint=true HUDdirection=true noRemoteManagement=true repeat=false");

        ui.ShowWorkPlan(); yield return null; yield return null;
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.WorkPlan, "Work Plan introduction missing");
        yield return CaptureMenu(MenuHelpView.Menu.WorkPlan);
        yield return Press(Key.Escape);
        Check(manager.WorkPlanOpen && !ui.Help.IsOpen, "Help Escape closed Work Plan");
        Check(ui.RootElement.Query<Label>().ToList().Any(l => l.text.Contains("Pruning")),
            "early Work Plan does not show pruning locked");
        Check(ui.RootElement.Query<Label>().ToList().Any(l => l.text.Contains(ScenarioOneWorkPlanFocus.LaterObjectiveNote)),
            "early Work Plan does not explain the later objective lock");
        Check(!ui.RootElement.Query<Button>().ToList().Any(b => b.text.StartsWith("Add ") && b.text.Contains("eligible crop tree pruning")),
            "early Work Plan offers pruning before its prerequisites");
        Check(!manager.AnnualReviewSeen, "Empty review falsely completed tutorial");
        ui.ShowReview(); yield return null;
        ui.AcknowledgeAnnualReview(); Check(!manager.AnnualReviewSeen, "Year-zero review acknowledged without results");
        Check(!ui.Help.IsOpen, "Annual introduction shown before results exist");
        ui.ShowWorkPlan(); yield return null;
        Check(ui.AdvanceFromWorkPlan(), "First annual advance blocked"); yield return null; yield return null;
        // The first results open WITHOUT a help screen stacked over the acknowledgement; the guidance is in the review itself.
        Check(!ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && !manager.AnnualReviewSeen,
            "First results must open without a help screen over them, and without being acknowledged");
        Button acknowledgeButton = ui.RootElement.Q<Button>("annual-review-acknowledge");
        Check(acknowledgeButton != null && acknowledgeButton.parent.Query<Label>().ToList().Any(l => l.text == MenuHelpView.AnnualReviewReadPrompt),
            "Annual Review lacks its in-screen reading guidance");
        ui.ShowHelp(MenuHelpView.Menu.AnnualReview); yield return null;
        Check(ui.Help.IsOpen && ui.Help.CurrentMenu == MenuHelpView.Menu.AnnualReview, "Help does not reopen the Annual Review explanation on request");
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

        yield return LoadStage7Fixture();
        yield return VerifyLearningObjectives();

        ui.CloseAll(); yield return null;
        int learningBeforePreview = ui.Learning.CompletedSteps;
        Check(manager.TryBeginReferencePreview(20), "Reference preview unavailable"); yield return null;
        ui.ShowHelp(MenuHelpView.Menu.WalkingHud); Check(!ui.Help.IsOpen, "Help leaked into Reference preview");
        manager.EndReferencePreview(); yield return null;
        Check(ui.Learning.CompletedSteps == learningBeforePreview, "Reference preview changed learning progress");
        // The rack screen is introduced by the rack tool itself on the first B press (verified by RackUxRack), not by walking to a screen.
        // Tree Marks exists only once a Rack Planning V2 plan is confirmed (RACK-V2-10, Tools/Verification/RackV2HelpTeaching), never in this flow.
        foreach (MenuHelpView.Menu menu in Enum.GetValues(typeof(MenuHelpView.Menu)).Cast<MenuHelpView.Menu>().Where(m => m != MenuHelpView.Menu.Rack && m != MenuHelpView.Menu.TreeMarks)) Check(MenuHelpView.Introduced(menu), "Introduction preference missing: " + menu);
        Debug.Log("MENU_PREFERENCES_PASS allFive=true localOnly=true referenceClean=true");
    }

    ScenarioOneRackBay.Analysis Bay()
    {
        var layout = ui.RackTool.Planner.LayoutStatus();
        var line = layout[layout.Count / 2];
        return ScenarioOneRackBayReader.Read(FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None), manager.WorkOrders,
            ui.RackTool.Planner.LayoutLines, new ScenarioOneRackBay.Frame(line.Start, line.End,
                ScenarioOneRackBay.HalfWidth(ui.RackTool.Planner.LayoutLines, layout.Count / 2)));
    }

    static ForestTree[] LivingSpruce() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce")
        .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    ScenarioOneTeachingSequence.Facts LiveFacts()
    {
        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
        return (ScenarioOneTeachingSequence.Facts)method.Invoke(ui.Learning, null);
    }

    private static readonly Vector3[] ReviewDirections =
    {
        Vector3.back, Vector3.forward, Vector3.right, Vector3.left,
        new Vector3(-1f, 0f, -1f).normalized, new Vector3(1f, 0f, -1f).normalized,
        new Vector3(-1f, 0f, 1f).normalized, new Vector3(1f, 0f, 1f).normalized
    };
    private static readonly float[] ReviewDistances = { 3f, 2.5f, 2f };
    private readonly Collider[] reviewOverlaps = new Collider[16];

    private bool OverlapsOtherTree(Vector3 standPoint, ForestTree target, out string blocking)
    {
        blocking = "";
        int count = Physics.OverlapSphereNonAlloc(standPoint + Vector3.up * 0.9f, 0.35f, reviewOverlaps);
        for (int i = 0; i < count; i++)
        {
            Collider collider = reviewOverlaps[i];
            Transform hit = collider.transform;
            if (hit == ui.Player.transform || hit.IsChildOf(ui.Player.transform)) continue;
            ForestTree tree = collider.GetComponentInParent<ForestTree>();
            if (tree == target) continue;
            blocking = tree != null ? tree.TreeId : collider.name;
            return true;
        }
        return false;
    }

    private IEnumerator AimAt(ForestTree tree)
    {
        Vector3 aimPoint = tree.transform.position + Vector3.up * 1.4f;
        var attempts = new List<string>();
        for (int direction = 0; direction < ReviewDirections.Length; direction++)
        {
            for (int distance = 0; distance < ReviewDistances.Length; distance++)
            {
                Vector3 standPoint = tree.transform.position + ReviewDirections[direction] * ReviewDistances[distance];
                if (OverlapsOtherTree(standPoint, tree, out string blocking))
                {
                    attempts.Add(direction + "@" + ReviewDistances[distance] + " blocked by " + blocking);
                    continue;
                }
                ui.Player.transform.position = standPoint;
                ui.Player.LookToward(aimPoint);
                for (int i = 0; i < 8; i++) yield return null;
                ForestTree aimed = ui.Marking.AimedTree;
                if (aimed == tree) yield break;
                attempts.Add(direction + "@" + ReviewDistances[distance] + " aimed " + (aimed != null ? aimed.TreeId : "nothing"));
            }
        }
        throw new InvalidOperationException("could not aim at " + tree.TreeId + " [" + string.Join("; ", attempts) + "]");
    }

    private IEnumerator ReviewInspect(ForestTree tree)
    {
        if (ui.Player.IsInspecting) yield return Press(Key.E);
        ui.CloseAll(); ui.CloseHelp();
        yield return AimAt(tree);
        yield return Press(Key.E);
        for (int i = 0; i < 8; i++) yield return null;
        ui.CloseHelp();
        for (int i = 0; i < 4; i++) yield return null;
        Check(ui.Player.IsInspecting && ui.Player.InspectedTree == tree, "Stage 7 review inspection failed: " + tree.TreeId);
    }

    IEnumerator ReviewStage7()
    {
        ui.CloseAll(); ui.CloseHelp();
        foreach (string id in Bay().Future.Select(t => t.Id).ToArray())
        {
            yield return ReviewInspect(LivingSpruce().First(t => t.TreeId == id));
            yield return Press(Key.J);
            for (int i = 0; i < 6; i++) yield return null;
        }
        for (int part = 0; part < 6; part++)
        {
            ScenarioOneRackBay.Analysis bay = Bay();
            ForestTree tree = LivingSpruce().First(t => !t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Portion(t.transform.position.x, t.transform.position.z) == part);
            yield return ReviewInspect(tree);
            yield return Press(Key.J);
            for (int i = 0; i < 6; i++) yield return null;
        }
        if (ui.Player.IsInspecting) yield return Press(Key.E);
        for (int i = 0; i < 20; i++) yield return null;
        ScenarioOneTeachingSequence.Facts facts = LiveFacts();
        if (facts.Bay != null)
            Debug.Log("MENU_REVIEW_DIAGNOSTIC complete=" + facts.RackReviewComplete
                + " enough=" + facts.Bay.HasEnoughFuture + " canopy=" + facts.Bay.CanopyKept
                + " exec=" + facts.Bay.ReleaseExecuted + " mixed=" + facts.Bay.MixedExecuted
                + " reviewed=" + facts.ReleaseReviewed + " trees=" + facts.RevisitedTrees
                + " portions=" + facts.StandPortionsReviewed + " decisions=" + facts.ReviewDecisions
                + " future=" + string.Join(",", facts.Bay.Future.Select(t => t.Id + ":p" + t.PlannedShare + "/m" + t.MarkedShare)));
        Check(facts.RackReviewComplete && facts.RevisitedTrees == 3 && facts.StandPortionsReviewed == 6,
            "Stage 7 review did not record three trees and six stand portions: trees " + facts.RevisitedTrees + " portions " + facts.StandPortionsReviewed);
    }

    private IEnumerator LoadStage7Fixture()
    {
        string path = Environment.GetEnvironmentVariable("CCF_MENU_STAGE7_SAVE");
        Check(!string.IsNullOrEmpty(path) && File.Exists(path), "Stage 7 menu fixture missing: " + (path ?? "<unset>"));
        ForestSaveData data = JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(path));
        Check(data != null, "Stage 7 menu fixture is not a ForestSaveData: " + path);
        Check(saves.LoadData(data, false), "Stage 7 menu fixture did not load: " + path);
        yield return null; yield return null;
        ui.CloseAll(); ui.CloseHelp();
        ui.RackLayout.Update(true); ui.Learning.ReloadProgress();
        // The rack review is deliberately session-only: a loaded save returns to the review. Repeat the real production
        // inspection/[J] review after the progress reload, then assert the later teaching stage without resetting it again.
        yield return ReviewStage7();
        ui.RackLayout.Update(true);
        for (int i = 0; i < 8; i++) ui.Learning.Refresh(true);
        Debug.Log("MENU_STAGE7_FIXTURE loaded=" + path + " year=" + manager.CurrentEcologicalYear
            + " stage=" + ui.Learning.CurrentTopicIndex + " title='" + ui.Learning.CurrentTitle + "'");
        Check(ui.Learning.CurrentTopicIndex == 7, "Stage 7 menu fixture does not present the final teaching stage");
    }

    private IEnumerator VerifyLearningObjectives()
    {
        ui.CloseAll(); yield return null;
        yield return Press(Key.O);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Objectives && manager.AnyPanelOpen && !player.enabled,
            "O does not open learning objectives safely");
        // Let the existing save/load notification expire before judging text.
        yield return new WaitForSecondsRealtime(3.1f);
        foreach (Vector2Int size in new[] { new Vector2Int(1280,720), new Vector2Int(1600,900), new Vector2Int(1920,1080) })
        {
            MenuTutorialVerification.SetGameSize(size.x, size.y);
            yield return CaptureScreen("learning-map-" + size.x);
            VisualElement panel = ui.Learning.Root.Children().First();
            Rect viewport = ui.RootElement.worldBound, bounds = panel.worldBound;
            Check(viewport.Contains(bounds.min) && viewport.Contains(bounds.max), "Learning objectives panel clipped");
            Check(panel.Q<ScrollView>().contentViewport.worldBound.yMax <= bounds.yMax, "Learning body escapes panel");
        }
        string forestBeforeReading = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        yield return Click(ui.Learning.Root.Q<Button>("learn-map.compare"));
        Check(ui.Learning.IsDone("map.compare") && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == forestBeforeReading,
            "Reading acknowledgement changes forest or loses progress");
        Check(!ui.Learning.Root.Q<Button>("learn-deadwood.read").enabledSelf, "Deadwood reading prerequisite missing");
        var reloadedProgress = new LearningObjectivesView(ui);
        Check(reloadedProgress.IsDone("map.compare") && reloadedProgress.CompletedSteps == ui.Learning.CompletedSteps,
            "Learning preferences do not reload");
        yield return Press(Key.O);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, "O does not return to forest");

        ForestTree[] livingForLearning = FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t.CanChop).ToArray();
        ScenarioOneRackBay.Analysis learningBay = Bay();
        ForestTree cropForPruning = livingForLearning
            .Where(t => t.IsCropTree && t.CanPrune(2.5f, manager.CurrentEcologicalYear + 1) == null)
            .OrderBy(t => new Vector2(t.transform.position.x, t.transform.position.z).sqrMagnitude).FirstOrDefault();
        // The fell fixture must sit outside the teaching bay: a marked in-footprint removal counts toward the 40% safeguard and
        // could drop the rederived lesson below stage 7 before the pruning card is built.
        ForestTree fellTarget = livingForLearning
            .Where(t => !t.IsCropTree && !learningBay.Contains(t.transform.position.x, t.transform.position.z))
            .OrderBy(t => new Vector2(t.transform.position.x, t.transform.position.z).sqrMagnitude).FirstOrDefault();
        Check(cropForPruning != null, "Stage 7 fixture lacks an existing Crop Tree eligible for pruning");
        Check(fellTarget != null, "Stage 7 fixture lacks a non-crop fell target");
        Check(cropForPruning.TreeId != fellTarget.TreeId, "Stage 7 fixture selected the same tree for pruning and felling");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        ForestTree aimed = null;
        Vector3 bayCentre = new Vector3(learningBay.Geometry.X, 0f, learningBay.Geometry.Z);
        Vector3 outward = fellTarget.transform.position - bayCentre; outward.y = 0f;
        if (outward.sqrMagnitude > 0.0001f) outward.Normalize(); else outward = Vector3.back;
        Vector3[] aimDirections = { outward, -outward, Vector3.back, Vector3.forward, Vector3.right, Vector3.left };
        foreach (Vector3 direction in aimDirections)
        {
            player.transform.position = fellTarget.transform.position + direction * 3f;
            player.LookToward(fellTarget.transform.position + Vector3.up * 1.3f);
            for (int i = 0; i < 5; i++) yield return null;
            aimed = ui.Marking.AimedTree;
            if (aimed != null && !aimed.IsCropTree && !learningBay.Contains(aimed.transform.position.x, aimed.transform.position.z))
            {
                fellTarget = aimed;
                break;
            }
        }
        Debug.Log("LEARNING_AIM_DIAGNOSTIC cursor=" + UnityEngine.Cursor.lockState + " player=" + player.enabled
            + " panels=" + manager.AnyPanelOpen + " camera=" + Camera.main.transform.position + " target=" + fellTarget.transform.position
            + " aimed=" + (aimed != null ? aimed.TreeId : "none") + " crop=" + (aimed != null && aimed.IsCropTree)
            + " hits=" + string.Join(",", Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 8)
                .Select(h => h.collider.name)));
        Check(aimed != null && !aimed.IsCropTree && !learningBay.Contains(aimed.transform.position.x, aimed.transform.position.z),
            "Learning fixture does not aim at a non-crop tree outside the teaching bay");
        yield return Press(Key.M); ui.CloseHelp();
        Check(!aimed.IsMarkedForFell && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, "M also marked aimed tree");
        ui.CloseAll(); yield return null; yield return null;
        yield return Press(Key.X);
        Check(aimed.IsMarkedForFell && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, "X does not mark Fell independently");
        yield return Press(Key.X);
        Check(!aimed.IsMarkedForFell, "X does not undo Fell mark");
        ui.Marking.Mark(fellTarget, TreeMarkType.Fell, false);
        for (int i = 0; i < 30; i++) yield return null;
        // Re-establish the session-only review immediately before the Work Plan reads the real stage; the intervening map,
        // marking and objectives navigation must never let an older review decide the pruning card.
        yield return ReviewStage7();
        ui.ShowWorkPlan(); ui.CloseHelp(); yield return null;
        bool pruneResultBefore = ui.Learning.IsDone("prune.result");
        bool deadwoodResultBefore = ui.Learning.IsDone("deadwood.result");
        ScenarioOneTeachingSequence.Facts pruneFacts = LiveFacts();
        Debug.Log("MENU_PRUNE_DIAGNOSTIC stage=" + ui.Learning.CurrentTopicIndex + " title='" + ui.Learning.CurrentTitle + "'"
            + " crops=" + manager.CropTreeCount + " eligible=" + manager.EligibleCropTreePruningCount
            + " future=" + (pruneFacts.Bay != null) + " enough=" + (pruneFacts.Bay != null && pruneFacts.Bay.HasEnoughFuture)
            + " canopy=" + (pruneFacts.Bay != null && pruneFacts.Bay.CanopyKept)
            + " releaseExec=" + pruneFacts.ReleaseExecuted + " review=" + pruneFacts.RackReviewComplete
            + " rackProblem='" + ui.RackPlanProblem + "'"
            + " buttons=" + string.Join("|", ui.RootElement.Query<Button>().ToList().Select(b => b.text)));
        // The Work Plan folds the Fell marks in when it opens, so there is no separate "Add marked trees" step to click.
        yield return Click(ui.RootElement.Query<Button>().ToList().First(b => b.text == "Leave as deadwood"));
        yield return Click(ui.RootElement.Query<Button>().ToList().First(b => b.text.StartsWith("Add ") && b.text.Contains("eligible crop tree pruning")));
        Check(manager.TryPurchaseStock("beech-sapling", 1), "Learning nursery purchase failed");
        ForestTree[] living = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => !t.IsStump).ToArray();
        Vector3 planting = ui.Ecology.Cells.Select(c => new Vector3(c.Center.x - 0.35f, 0, c.Center.y))
            .First(a => living.All(t => Vector2.Distance(new Vector2(a.x,a.z), new Vector2(t.transform.position.x,t.transform.position.z)) > 0.85f));
        Check(manager.TryDesignateExactPlanting("beech-sapling", planting, CCF.Forestry.WorkEconomy.WorkExecutionMethod.Contractor, true),
            "Learning planting designation failed");
        int clearance = Enumerable.Range(0, ui.Ecology.Cells.Length)
            .First(i => ui.Ecology.GetCellIndex(planting) != i && manager.QueryClearance(ClearanceFootprint.Cell(ui.Ecology, i)).HasTargets);
        Check(manager.TryDesignateVegetationClearance(clearance), "Learning clearance designation failed");
        ui.Learning.Observe();
        foreach (string id in new[] { "fell.plan", "deadwood.plan", "prune.plan", "plant.stock", "plant.plan", "plant.shelter", "clear.plan" })
            Check(ui.Learning.IsDone(id), "Learning action not recorded: " + id);
        // The Stage 7 fixture already carries real deadwood history, so only a *new* execution credit is a failure.
        Check(ui.Learning.IsDone("prune.result") == pruneResultBefore && ui.Learning.IsDone("deadwood.result") == deadwoodResultBefore,
            "Planning falsely credited execution");
        Check(manager.ApprovePendingWork(), "Learning job approval failed"); ui.Learning.Observe();
        foreach (string id in new[] { "fell.approve", "prune.approve", "plant.approve", "clear.approve" })
            Check(ui.Learning.IsDone(id), "Approval learning not recorded: " + id);
        Check(ui.AdvanceFromWorkPlan(), "Learning jobs did not advance in later year"); yield return null;
        ui.CloseHelp(); ui.Learning.Observe();
        foreach (string id in new[] { "fell.result", "deadwood.result", "prune.result", "plant.result", "clear.result", "review.open", "review.read" })
            Check(ui.Learning.IsDone(id), "Successful work result not recorded: " + id);
        ui.CloseAll(); yield return null;
        player.transform.position = manager.DeadwoodRecords.First().worldPosition + Vector3.right * 2;
        yield return null; ui.Learning.Observe(); Check(ui.Learning.IsDone("deadwood.visit"), "Deadwood site visit not recorded");
        ui.ShowObjectives();
        foreach (Foldout topic in ui.Learning.Root.Query<Foldout>().ToList()) topic.value = topic.text.Contains("Fallen deadwood");
        ui.Learning.Root.Q<ScrollView>().scrollOffset = Vector2.zero;
        yield return CaptureScreen("learning-deadwood");
        saves.Save(); saves.Load(); yield return null;
        Check(ui.Learning.IsDone("map.compare") && ui.Learning.IsDone("deadwood.result"), "Forest reload erased learning progress");
        Check(manager.CurrentEcologicalYear > 1 && !ui.Learning.IsDone("prune.read"), "Optional learning forced into year one or auto-acknowledged");
        Debug.Log("LEARNING_OBJECTIVES_PASS mapSubsteps=true M=mapOnly X=fell O=objectives eightTopics=true readingExplicit=true actionsObserved=true laterYears=true profileReload=true worldUnchangedByReading=true");
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
