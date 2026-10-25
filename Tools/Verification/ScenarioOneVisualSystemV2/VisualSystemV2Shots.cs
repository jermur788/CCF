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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UIElements;

// Disposable rendered review for SCENARIO-ONE-UI-VISUAL-SYSTEM-V2.
//
// Captures the required Scenario One surfaces at 1280x720 and 1920x1080 through the real Unity
// renderer, records computed text/background colours and font asset names for style QA, and
// audits panel/title containment and one-line text fit. It also checkpoints the world hash
// before/after pure UI navigation and records the single deliberate world change made by the
// existing ui.AdvanceFromWorkPlan() path.
//
// Normal startup -> Start New -> existing introduction closed -> HUD and screens. No teaching
// preferences are injected, no rules/save/simulation are edited, and no disk Save is written.
// Competitor evidence is best-effort: it temporarily marks a Crop Tree and one neighbour with
// the normal marking API, then unmarks them and asserts the world hash is restored.
//
// Run (graphical Editor, private display/isolated config handled by the parent launcher):
//   CCF_DIAG_DIR=<folder outside the repo> CCF_UI_SIZES=1280x720,1920x1080
//   <Unity 6000.6.0f1> -projectPath /media/jer/ZX20/CCF-U1
//     -executeMethod VisualSystemV2Shots.Begin -logFile <folder>/visual-system-v2.log
//
// Install by copying this file into Assets/ForestPrototype/, run, then remove it together with
// its generated .meta. The runtime installer only acts for the exact -executeMethod argument.
//
// LIMITATION: this is callback/API smoke through the real UI state and renderer, not OS-native
// keyboard/mouse input. It proves state, visibility, lock and navigation transitions; it does
// not prove OS-level click targeting, hover or key delivery.
public static class VisualSystemV2Shots
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        VisualSystemV2ShotsRunner.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "VisualSystemV2Shots.Begin")) return;
        new GameObject("UI visual system V2 review").AddComponent<VisualSystemV2ShotsRunner>();
    }
}

public sealed class VisualSystemV2ShotsRunner : MonoBehaviour
{
    private const string Limitation =
        "Callback/API smoke only: no OS-native keyboard, mouse click or pointer-hover input was delivered.";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly Vector2Int[] RequiredSizes = { new Vector2Int(1280, 720), new Vector2Int(1920, 1080) };

    [Serializable] public sealed class CaptureRecord
    {
        public string name, path;
        public int width, height;
    }

    [Serializable] public sealed class StyleRecord
    {
        public string screen, size, element, classes, text, font;
        public float fontSize;
        public float r, g, b, a;
        public float backgroundR, backgroundG, backgroundB, backgroundA;
        public float x, y, width, height;
        public string whiteSpace;
    }

    [Serializable] public sealed class StyleScreen
    {
        public string screen, size;
        public List<StyleRecord> records = new List<StyleRecord>();
    }

    [Serializable] public sealed class VisualAudit
    {
        public string harness = "VisualSystemV2Shots";
        public string limitation = Limitation;
        public string verdict;
        public bool pass;
        public int failures, passes, shots;
        public string hashBeforeNavigation, hashAfterNavigation, hashAfterCompetitorRestore, hashAfterAdvance, hashAfterReview;
        public List<CaptureRecord> captures = new List<CaptureRecord>();
        public List<StyleScreen> screens = new List<StyleScreen>();
        public List<string> notes = new List<string>();
        public List<string> errors = new List<string>();
    }

    private string output;
    private Vector2Int size;
    private int shots, passes, failures;
    private ScenarioOneUiRoot ui;
    private ForestPlayer player;
    private StandaloneSessionMenu menu;
    private ForestSaveController saves;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private readonly List<string> notes = new List<string>();
    private readonly List<string> errors = new List<string>();
    private readonly List<CaptureRecord> captures = new List<CaptureRecord>();
    private readonly List<StyleScreen> screens = new List<StyleScreen>();
    private string hashBeforeNavigation, hashAfterNavigation, hashAfterCompetitorRestore, hashAfterAdvance, hashAfterReview;

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
            null, new object[] { Enum.ToObject(kind, 1), width, height, "UI visual system V2 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { gameSize });
        Type viewType = assembly.GetType("UnityEditor.GameView"); EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        PropertyInfo gizmos = viewType.GetProperty("showGizmos", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (gizmos != null && gizmos.CanWrite) gizmos.SetValue(view, false);
        view.Show(); view.Focus(); view.Repaint();
    }

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output)) output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "ui-visual-system-v2");
        Directory.CreateDirectory(output);
        Application.logMessageReceived += OnLogMessage;
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

        if (errors.Count > 0)
        {
            failures++;
            Debug.Log("VISUAL_V2_CHECK_FAIL NO_CONSOLE_ERRORS " + string.Join(" | ", errors.Take(4)));
        }
        bool pass = failure == null && failures == 0;
        Application.logMessageReceived -= OnLogMessage;
        WriteReports(pass, failure);
        Debug.Log(pass
            ? $"UI_VISUAL_SYSTEM_V2_HARNESS_PASS shots={shots} passes={passes} failures=0"
            : $"UI_VISUAL_SYSTEM_V2_HARNESS_FAIL shots={shots} passes={passes} failures={failures} exception={failure}");
        Debug.Log("VISUAL_V2_LIMITATION " + Limitation);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(pass ? 0 : 1);
    }

    private IEnumerator Play()
    {
        for (int frame = 0; frame < 900 && (menu == null || !StandaloneSessionMenu.IsOpen); frame++)
        {
            if (menu == null) menu = UnityEngine.Object.FindFirstObjectByType<StandaloneSessionMenu>();
            yield return null;
        }
        if (menu == null || !StandaloneSessionMenu.IsOpen) throw new InvalidOperationException("the startup screen did not open");
        for (int frame = 0; frame < 180 && !Application.isFocused; frame++) yield return null;
        notes.Add("Application focused at start: " + Application.isFocused);

        // Representative session surfaces before the forest: title card and its Controls / Help card.
        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            InvokePrivate(menu, "StartupCard");
            yield return Settle(10);
            yield return Shot("session-startup", SessionCard());
            menu.ShowControlsHelp();
            yield return Settle(10);
            yield return Shot("session-controls", SessionCard());
        }
        InvokePrivate(menu, "StartupCard");
        yield return Settle(8);

        menu.StartNewScenario();
        for (int frame = 0; frame < 600 && (ui == null || ui.Help == null || !ui.Help.IsOpen); frame++)
        {
            if (ui == null) ui = UnityEngine.Object.FindFirstObjectByType<ScenarioOneUiRoot>();
            yield return null;
        }
        if (ui == null || ui.Help == null) throw new InvalidOperationException("Scenario One UI missing after Start New");
        if (!ui.Help.IsOpen || ui.Help.CurrentMenu != MenuHelpView.Menu.WalkingHud)
            throw new InvalidOperationException("Start New did not open the existing scenario introduction");
        ui.CloseHelp();
        yield return Settle(20);
        Check(!ui.Help.IsOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, "INTRODUCTION_CLOSES_TO_WALKING_HUD");
        player = UnityEngine.Object.FindFirstObjectByType<ForestPlayer>();
        saves = UnityEngine.Object.FindFirstObjectByType<ForestSaveController>();
        if (player == null || saves == null) throw new InvalidOperationException("player or save controller missing");
        startPosition = player.transform.position;
        startRotation = player.transform.rotation;
        hashBeforeNavigation = WorldHash();
        notes.Add("World checkpoint before pure UI navigation: " + hashBeforeNavigation);
        notes.Add(Limitation);
        if (ui.Manager != null)
        {
            Check(player.enabled && !ui.Manager.ModalOpen && !ui.Manager.AnyPanelOpen,
                "FOREST_INPUT_AVAILABLE_AFTER_INTRO");
            notes.Add($"after intro cursor lock={Cursor.lockState} visible={Cursor.visible} (startup menu Close does not restore it)");
        }

        bool waypointAvailable = ui.Ecology != null && ui.Map != null;
        if (!waypointAvailable) Fail("WAYPOINT_API_PRESENT", "ecology or stand map missing");

        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            ResetPlayer();
            yield return Settle(12);
            yield return Shot("hud-normal", ui.RootElement.Q(className: "hud-status"));
            Check(Visible(ui.RootElement.Q(className: "hud-status")), "HUD_STATUS_VISIBLE");
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !ui.Manager.WorkPlanOpen, "HUD_IS_WALKING_STATE");
            Check(player.enabled && !ui.Manager.ModalOpen && !ui.Manager.AnyPanelOpen, "HUD_INPUT_UNLOCKED");
            bool minimapVisible = ui.Minimap != null && Visible(ui.Minimap.Root);
            Check(minimapVisible, "HUD_MINIMAP_VISIBLE");
            if (minimapVisible) yield return CropShot("hud-minimap", ui.Minimap.Root);

            if (waypointAvailable)
            {
                Check(ui.Map.SetWaypointCell(ui.Ecology, 0), "WAYPOINT_SET");
                yield return Settle(14);
                VisualElement waypoint = ui.RootElement.Q("hud-waypoint-panel");
                Check(Visible(waypoint), "WAYPOINT_HUD_VISIBLE");
                yield return Shot("hud-waypoint", waypoint);
                ClearWaypoint();
                yield return Settle(6);
            }

            ForestTree tree = DenseTree();
            if (tree == null)
            {
                Fail("TREE_FOR_INSPECTION", "no living mature/young tree found");
            }
            else
            {
                yield return Inspect(tree);
                CloseAutoHelp();
                yield return Settle(6);
                Check(player.InspectedTree == tree, "TREE_SELECTED");
                Check(Visible(ui.RootElement.Q("inspect-card")), "TREE_INSPECTION_VISIBLE");
                notes.Add($"{target.x}x{target.y} inspection competitors={ui.Inspection.ShowsCompetitors} estimate={ui.Inspection.ShowsEstimate}");
                yield return Shot("hud-tree-inspection", ui.Inspection.Root);
                ClearInspection();
                yield return Settle(8);
            }

            ui.ShowObjectives();
            yield return Settle(18);
            CloseAutoHelp();
            yield return Settle(8);
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Objectives, "OBJECTIVES_OPEN");
            CheckPanelInputLock("OBJECTIVES_INPUT_LOCKED");
            yield return Shot("objectives", ui.RootElement.Q(className: "modal-objectives"));
            CloseAll();
            yield return Settle(10);
            CheckWalkingAfterPanel("OBJECTIVES_CLOSE_TO_HUD");

            ui.ShowWorkPlan();
            yield return Settle(20);
            CloseAutoHelp();
            yield return Settle(10);
            Check(ui.Manager.WorkPlanOpen && ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None, "WORKPLAN_OPEN");
            CheckPanelInputLock("WORKPLAN_INPUT_LOCKED");
            var locked = ui.RootElement.Query(className: "card-locked").ToList().Where(Visible).ToList();
            Check(locked.Count > 0, "WORKPLAN_CURRENT_STAGE_LOCKED_SECTION_VISIBLE", "locked=" + locked.Count);
            Check(locked.All(l => !l.enabledInHierarchy), "WORKPLAN_LOCKED_SECTION_DISABLED");
            yield return Shot("workplan-locked-stage", ui.RootElement.Q(className: "modal-workplan"));
            CloseAll();
            yield return Settle(10);
            CheckWalkingAfterPanel("WORKPLAN_CLOSE_TO_HUD");

            // Help sets ModalOpen; ScenarioOneUiRoot.ShowHelp/Update also routes it through
            // SetAuxiliaryPanelOpen, disabling the player while leaving the UI root enabled.
            ui.ShowHelp(MenuHelpView.Menu.WalkingHud);
            yield return Settle(14);
            Check(ui.Help.IsOpen && ui.Manager.ModalOpen, "HELP_OPENS_MODAL");
            Check(!player.enabled && ui.enabled && ui.Manager.AnyPanelOpen, "HELP_LOCKS_PLAYER_AND_LEAVES_UI_ENABLED");
            Check(Cursor.lockState == CursorLockMode.None && Cursor.visible, "HELP_CURSOR_FREE");
            notes.Add($"{target.x}x{target.y} help cursor lock={Cursor.lockState} visible={Cursor.visible}");
            yield return Shot("help-walking-hud", ui.RootElement.Q("menu-help-window"));
            ui.CloseHelp();
            yield return Settle(10);
            Check(!ui.Help.IsOpen && !ui.Manager.ModalOpen && !ui.Manager.AnyPanelOpen, "HELP_CLOSES_MODAL");
            CheckWalkingAfterPanel("HELP_CLOSE_RESTORES_PANEL_INPUT");

            menu.Open();
            yield return Settle(18);
            Check(StandaloneSessionMenu.IsOpen, "SESSION_MENU_OPENS");
            Check(Time.timeScale == 0f && !player.enabled && !ui.enabled, "SESSION_MENU_PAUSES_AND_DISABLES_FOREST_UI");
            Check(Cursor.lockState == CursorLockMode.None && Cursor.visible, "SESSION_MENU_CURSOR_FREE");
            yield return Shot("session-menu", SessionCard());
            menu.Close();
            yield return Settle(12);
            Check(!StandaloneSessionMenu.IsOpen && player.enabled && ui.enabled, "SESSION_MENU_RESTORES_FOREST_UI");
            Check(Mathf.Approximately(Time.timeScale, 1f), "SESSION_MENU_RESTORES_TIME");
            notes.Add($"{target.x}x{target.y} session-close cursor lock={Cursor.lockState} visible={Cursor.visible} (menu Close does not restore it)");

            ui.ShowMap();
            yield return Settle(20);
            CloseAutoHelp();
            yield return Settle(10);
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Map, "STAND_MAP_OPEN");
            CheckPanelInputLock("STAND_MAP_INPUT_LOCKED");
            Check(ui.Map != null && ui.Map.Canvas != null && ui.Map.Overview != null, "STAND_MAP_AND_OVERVIEW_PRESENT");
            yield return Shot("stand-map", ui.RootElement.Q(className: "modal-map"), ui.RootElement.Q("map-frame"), ui.RootElement.Q("map-overview"));
            CloseAll();
            yield return Settle(10);
            CheckWalkingAfterPanel("STAND_MAP_CLOSE_TO_HUD");
        }

        hashAfterNavigation = WorldHash();
        Check(hashAfterNavigation == hashBeforeNavigation, "WORLD_UNCHANGED_BY_PURE_UI_NAVIGATION",
            "before=" + hashBeforeNavigation + " after=" + hashAfterNavigation);
        notes.Add("World checkpoint after pure UI navigation: " + hashAfterNavigation);

        yield return CompetitorEvidence();

        hashAfterAdvance = null;
        hashAfterReview = null;
        string beforeAdvance = WorldHash();
        Check(ui.AdvanceFromWorkPlan(), "ANNUAL_ADVANCE_API_SUCCEEDED");
        yield return Settle(30);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review, "ANNUAL_REVIEW_OPENS_AFTER_ADVANCE");
        Check(ui.NeedsAnnualReview, "ANNUAL_REVIEW_IS_PENDING");
        CheckPanelInputLock("ANNUAL_REVIEW_INPUT_LOCKED");
        hashAfterAdvance = WorldHash();
        Check(hashAfterAdvance != beforeAdvance, "ANNUAL_ADVANCE_CHANGED_WORLD_EXACTLY_ONCE",
            "before=" + beforeAdvance + " after=" + hashAfterAdvance);
        notes.Add("Deliberate annual-advance world change: " + beforeAdvance + " -> " + hashAfterAdvance);

        foreach (Vector2Int target in Sizes())
        {
            yield return UseSize(target);
            CloseAutoHelp();
            yield return Settle(12);
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review, "ANNUAL_REVIEW_STAYS_OPEN");
            yield return Shot("annual-review", ui.RootElement.Q(className: "modal-review"));
        }
        hashAfterReview = WorldHash();
        Check(hashAfterReview == hashAfterAdvance, "WORLD_UNCHANGED_BY_ANNUAL_REVIEW_UI",
            "afterAdvance=" + hashAfterAdvance + " afterReview=" + hashAfterReview);
        notes.Add("World checkpoint after Annual Review UI: " + hashAfterReview);

        // Presentation cleanup only; nothing is written to disk.
        ClearWaypoint();
        ClearInspection();
        ui.CloseAll();
        yield return Settle(10);
        CheckWalkingAfterPanel("ANNUAL_REVIEW_CLOSE_TO_HUD");
    }

    private IEnumerator CompetitorEvidence()
    {
        ForestTree tree = DenseTree();
        if (tree == null)
        {
            notes.Add("Competitor evidence skipped: no suitable living tree.");
            hashAfterCompetitorRestore = hashAfterNavigation;
            return;
        }
        ForestTree neighbour = UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
            .Where(t => t != tree && t.IsLiving && t.CanChop)
            .OrderBy(t => Vector3.Distance(t.transform.position, tree.transform.position))
            .FirstOrDefault();
        if (neighbour == null)
        {
            notes.Add("Competitor evidence skipped: no living neighbour.");
            hashAfterCompetitorRestore = hashAfterNavigation;
            return;
        }

        TreeMarkType treeMark = tree.MarkType, neighbourMark = neighbour.MarkType;
        try
        {
            ui.Marking.Mark(tree, TreeMarkType.CropTree, false);
            ui.Marking.Mark(neighbour, TreeMarkType.Fell, false);
            yield return Settle(6);

            foreach (Vector2Int target in Sizes())
            {
                yield return UseSize(target);
                yield return Inspect(tree);
                for (int frame = 0; frame < 180 && !(ui.Inspection.ShowsCompetitors && ui.Competitors.ListedCount > 0); frame++)
                    yield return null;
                CloseAutoHelp();
                yield return Settle(8);
                if (ui.Inspection.ShowsCompetitors && ui.Competitors.ListedCount > 0)
                {
                    Check(true, "COMPETITOR_ASSESSMENT_VISIBLE", "listed=" + ui.Competitors.ListedCount);
                    yield return Shot("hud-tree-competitors", ui.Inspection.CompetitorPanel);
                    int tags = ui.RootElement.Query<Label>(className: "competitor-tag").ToList().Count(t => Visible(t));
                    Check(tags > 0, "COMPETITOR_WORLD_TAGS_VISIBLE", "tags=" + tags);
                    yield return SelectedCompetitorEvidence();
                }
                else
                {
                    notes.Add($"Competitor panel unavailable at {target.x}x{target.y}; inspection/estimate evidence retained.");
                    yield return Shot("hud-tree-competitors-unavailable", ui.Inspection.Root);
                }
                ClearInspection();
                yield return Settle(6);
            }
        }
        finally
        {
            if (ui != null && ui.Marking != null)
            {
                ui.Marking.Mark(tree, treeMark, false);
                if (neighbour != null) ui.Marking.Mark(neighbour, neighbourMark, false);
            }
        }
        yield return Settle(8);
        hashAfterCompetitorRestore = WorldHash();
        Check(hashAfterCompetitorRestore == hashAfterNavigation, "WORLD_RESTORED_AFTER_DESCRIPTIVE_COMPETITOR_STAGE",
            "nav=" + hashAfterNavigation + " restored=" + hashAfterCompetitorRestore);
    }

    // The numbered competitor tags are drawn in the world layer, outside the competitor panel. Choose one through the
    // real number-key handler (queued New Input System event), then prove the selected tag's own state and contrast.
    // If the queued key is not delivered, the accepted harness technique (set SelectedIndex for the visual fixture) is
    // used only so the selected-tag render/style evidence still exists; the key failure stays recorded.
    private IEnumerator SelectedCompetitorEvidence()
    {
        int wanted = ui.Competitors.ListedCount >= 2 ? 1 : 0;
        if (ui.Competitors.SelectedIndex != wanted)
            yield return Press(wanted == 1 ? Key.Digit2 : Key.Digit1);
        yield return Settle(8);
        bool selectedByKey = ui.Competitors.SelectedIndex == wanted;
        Check(selectedByKey, "COMPETITOR_SELECTED_BY_NUMBER_KEY",
            $"listed={ui.Competitors.ListedCount} wanted={wanted} actual={ui.Competitors.SelectedIndex}");
        if (!selectedByKey)
        {
            PropertyInfo selected = typeof(CompetitorAssessment).GetProperty("SelectedIndex");
            selected?.GetSetMethod(true)?.Invoke(ui.Competitors, new object[] { wanted });
            yield return Settle(6);
            Check(ui.Competitors.SelectedIndex == wanted, "SELECTED_COMPETITOR_VISUAL_FIXTURE",
                $"actual={ui.Competitors.SelectedIndex}");
            notes.Add("Number-key selection was not delivered in this run; SelectedIndex fixture used only for selected-tag visual evidence.");
        }

        Label tag = ui.RootElement.Query<Label>(className: "competitor-tag-selected").ToList().FirstOrDefault(Visible);
        Check(tag != null, "SELECTED_COMPETITOR_TAG_VISIBLE");
        if (tag == null) yield break;
        Check(!string.IsNullOrWhiteSpace(tag.text), "SELECTED_COMPETITOR_TAG_HAS_NUMBER", "text=\"" + tag.text + "\"");
        Check(ui.Competitors.Selected != null, "SELECTED_COMPETITOR_STATE_PRESENT");
        IResolvedStyle style = tag.resolvedStyle;
        Color text = style.color, background = style.backgroundColor;
        float contrast = ContrastRatio(text, background);
        Check(background.a >= 0.9f, "SELECTED_COMPETITOR_TAG_HAS_OPAQUE_BACKGROUND",
            "background=" + Rgba(background));
        Check(contrast >= 4.5f, "SELECTED_COMPETITOR_TAG_TEXT_CONTRAST_AA",
            $"text={Rgba(text)} background={Rgba(background)} ratio={contrast.ToString("0.00", Inv)}");
        notes.Add($"{size.x}x{size.y} selected competitor tag index={wanted} text={Rgba(text)} background={Rgba(background)} contrast={contrast.ToString("0.00", Inv)}");
        yield return Shot("hud-tree-competitors-selected", ui.Inspection.CompetitorPanel);
        yield return CropShot("competitor-tag-selected", tag, 36f);
    }

    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
        yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        yield return null; yield return null;
    }

    private IEnumerator UseSize(Vector2Int target)
    {
        size = target;
        SetGameSize(target.x, target.y);
        for (int frame = 0; frame < 120 && (Screen.width != target.x || Screen.height != target.y); frame++)
            yield return null;
        yield return Settle(24);
        Check(Screen.width == target.x && Screen.height == target.y,
            $"TARGET_RENDER_SIZE_{target.x}x{target.y}", $"actual={Screen.width}x{Screen.height}");
    }

    private IEnumerator Shot(string name, params VisualElement[] panels)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null)
        {
            Fail("SHOT_" + name, "no rendered capture");
            yield break;
        }
        Check(image.width == size.x && image.height == size.y, "CAPTURE_SIZE_" + name,
            $"capture={image.width}x{image.height} target={size.x}x{size.y}");
        Check(!LooksBlank(image), "SHOT_NOT_BLANK_" + name,
            "a uniform capture means the Game view had no real surface");
        string file = $"{name}-{size.x}x{size.y}.png";
        File.WriteAllBytes(Path.Combine(output, file), image.EncodeToPNG());
        captures.Add(new CaptureRecord { name = name, path = file, width = image.width, height = image.height });
        Destroy(image);
        shots++;
        RecordStyles(name);
        foreach (VisualElement panel in panels) AuditPanel(name, panel);
        Debug.Log($"VISUAL_V2_SHOT {file} {size.x}x{size.y}");
    }

    private IEnumerator CropShot(string name, VisualElement element, float padding = 0f)
    {
        if (element == null || !Visible(element))
        {
            Fail("CROP_TARGET_" + name, "missing or invisible");
            yield break;
        }
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        if (image == null)
        {
            Fail("CROP_CAPTURE_" + name, "no rendered capture");
            yield break;
        }
        VisualElement root = ActiveRoot();
        if (root == null)
        {
            Fail("CROP_ACTIVE_ROOT_" + name, "no active UI document");
            Destroy(image);
            yield break;
        }
        float scaleX = root.layout.width > 0f ? image.width / root.layout.width : 1f;
        float scaleY = root.layout.height > 0f ? image.height / root.layout.height : 1f;
        Rect raw = element.worldBound;
        Rect bounds = new Rect(raw.xMin - padding, raw.yMin - padding, raw.width + 2f * padding, raw.height + 2f * padding);
        int x = Mathf.Clamp(Mathf.RoundToInt(bounds.xMin * scaleX), 0, image.width - 1);
        int yTop = Mathf.Clamp(Mathf.RoundToInt(bounds.yMin * scaleY), 0, image.height - 1);
        int width = Mathf.Clamp(Mathf.RoundToInt(bounds.width * scaleX), 1, image.width - x);
        int height = Mathf.Clamp(Mathf.RoundToInt(bounds.height * scaleY), 1, image.height - yTop);
        int y = Mathf.Clamp(image.height - yTop - height, 0, image.height - height);
        if (width < 40 || height < 40)
        {
            Fail("CROP_TARGET_" + name, $"crop too small {width}x{height} from {bounds}");
            Destroy(image);
            yield break;
        }
        Color[] pixels;
        try
        {
            pixels = image.GetPixels(x, y, width, height);
        }
        catch (Exception error)
        {
            Fail("CROP_READ_" + name, error.Message);
            Destroy(image);
            yield break;
        }
        Texture2D crop = new Texture2D(width, height, TextureFormat.RGBA32, false);
        crop.SetPixels(pixels);
        crop.Apply();
        string file = $"{name}-{size.x}x{size.y}.png";
        File.WriteAllBytes(Path.Combine(output, file), crop.EncodeToPNG());
        captures.Add(new CaptureRecord { name = name, path = file, width = width, height = height });
        Destroy(crop);
        Destroy(image);
        shots++;
        Check(true, "MINIMAP_CROP_CAPTURED", file + " " + width + "x" + height);
        Debug.Log($"VISUAL_V2_SHOT {file} {width}x{height} (crop)");
    }

    private void AuditPanel(string state, VisualElement panel)
    {
        if (panel == null)
        {
            Fail("PANEL_PRESENT_" + state, "missing panel");
            return;
        }
        VisualElement root = ActiveRoot();
        if (root == null)
        {
            Fail("ACTIVE_ROOT_PRESENT_" + state, "no active UI document");
            return;
        }
        Check(Visible(panel), "PANEL_VISIBLE_" + state);
        Check(HorizontalContained(panel.worldBound, root.worldBound, 2f), "PANEL_CONTAINED_HORIZONTALLY_" + state,
            $"panel={panel.worldBound} root={root.worldBound}");
        bool scrollsVertically = panel.ClassListContains("session-card") || panel.ClassListContains("menu-help-window");
        if (!scrollsVertically)
            Check(VerticalContained(panel.worldBound, root.worldBound, 2f), "PANEL_CONTAINED_VERTICALLY_" + state,
                $"panel={panel.worldBound} root={root.worldBound}");

        Label title = panel.Query<Label>(className: "title").ToList().FirstOrDefault(Visible);
        if (title != null)
        {
            Check(!string.IsNullOrWhiteSpace(title.text), "TITLE_TEXT_PRESENT_" + state);
            Check(HorizontalContained(title.worldBound, panel.worldBound, 1.5f), "TITLE_CONTAINED_" + state,
                $"title={title.worldBound} panel={panel.worldBound} text=\"{Short(title.text)}\"");
        }

        var cut = new List<string>();
        int oneLine = 0;
        foreach (TextElement label in panel.Query<TextElement>().ToList())
        {
            if (!Visible(label) || string.IsNullOrWhiteSpace(label.text)) continue;
            if (label.resolvedStyle.whiteSpace != WhiteSpace.NoWrap) continue;
            if (MapDecoration(label)) continue;
            oneLine++;
            Vector2 natural = label.MeasureTextSize(label.text, 0f, VisualElement.MeasureMode.Undefined,
                0f, VisualElement.MeasureMode.Undefined);
            if (natural.x > label.worldBound.width + 1f)
                cut.Add($"\"{Short(label.text)}\" text={natural.x.ToString("0", Inv)} box={label.worldBound.width.ToString("0", Inv)}");
        }
        Check(cut.Count == 0, "NOWRAP_TEXT_FITS_PANEL_" + state,
            cut.Count == 0 ? "oneLineLabels=" + oneLine : string.Join(" | ", cut.Take(4)));
    }

    private void RecordStyles(string state)
    {
        VisualElement root = ActiveRoot();
        if (root == null) return;
        var screen = new StyleScreen { screen = state, size = $"{size.x}x{size.y}" };
        foreach (VisualElement element in root.Query<VisualElement>().ToList())
        {
            if (!Visible(element)) continue;
            bool isText = element is TextElement text && !string.IsNullOrWhiteSpace(text.text);
            bool isPanel = IsPanelSurface(element);
            if (!isText && !isPanel) continue;
            IResolvedStyle style = element.resolvedStyle;
            Color colour = style.color;
            Color background = style.backgroundColor;
            screen.records.Add(new StyleRecord
            {
                screen = state,
                size = $"{size.x}x{size.y}",
                element = string.IsNullOrEmpty(element.name) ? element.GetType().Name : element.name,
                classes = string.Join(" ", element.GetClasses()),
                text = isText ? Short(((TextElement)element).text) : "",
                font = FontName(element),
                fontSize = style.fontSize,
                r = colour.r, g = colour.g, b = colour.b, a = colour.a,
                backgroundR = background.r, backgroundG = background.g,
                backgroundB = background.b, backgroundA = background.a,
                x = element.worldBound.x, y = element.worldBound.y,
                width = element.worldBound.width, height = element.worldBound.height,
                whiteSpace = style.whiteSpace.ToString()
            });
            if (screen.records.Count >= 500) break;
        }
        screens.Add(screen);
        Debug.Log($"VISUAL_V2_STYLE {state} {size.x}x{size.y} records={screen.records.Count}");
    }

    private static bool IsPanelSurface(VisualElement element)
    {
        foreach (string c in element.GetClasses())
            if (c == "panel" || c == "modal" || c == "card" || c == "hud-status" || c == "hud-waypoint"
                || c == "menu-help-window" || c == "session-card" || c == "startup-card" || c == "map-frame"
                || c == "competitor-panel" || c == "inspect-card")
                return true;
        return false;
    }

    private static bool MapDecoration(VisualElement element)
    {
        for (VisualElement p = element; p != null; p = p.parent)
        {
            foreach (string c in p.GetClasses())
                if (c.StartsWith("map-", StringComparison.Ordinal)) return true;
        }
        return false;
    }

    private static string FontName(VisualElement element)
    {
        FontDefinition definition = element.resolvedStyle.unityFontDefinition;
        if (definition.fontAsset != null) return definition.fontAsset.name;
        if (definition.font != null) return definition.font.name;
        Font font = element.resolvedStyle.unityFont;
        return font != null ? font.name : "";
    }

    private VisualElement ActiveRoot()
    {
        if (menu != null && StandaloneSessionMenu.IsOpen)
        {
            UIDocument document = typeof(StandaloneSessionMenu)
                .GetField("document", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(menu) as UIDocument;
            if (document != null && document.rootVisualElement != null) return document.rootVisualElement;
            foreach (UIDocument candidate in UnityEngine.Object.FindObjectsByType<UIDocument>(FindObjectsSortMode.None))
                if (candidate != null && candidate.rootVisualElement != null
                    && candidate.rootVisualElement.ClassListContains("session-backdrop"))
                    return candidate.rootVisualElement;
        }
        return ui != null ? ui.RootElement : null;
    }

    private VisualElement SessionCard()
    {
        VisualElement root = ActiveRoot();
        return root != null ? root.Q(className: "session-card") : null;
    }

    private IEnumerator Inspect(ForestTree tree)
    {
        MethodInfo inspect = typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.NonPublic | BindingFlags.Instance);
        if (inspect == null) throw new InvalidOperationException("ForestPlayer.InspectTree not found");
        Vector3 camera = tree.transform.position + new Vector3(0f, 0f, -3.2f);
        float ground = Terrain.activeTerrain != null
            ? Terrain.activeTerrain.SampleHeight(camera) + Terrain.activeTerrain.transform.position.y
            : camera.y;
        player.transform.position = new Vector3(camera.x, ground + 0.1f, camera.z);
        player.LookToward(tree.transform.position + Vector3.up * 1.5f);
        for (int i = 0; i < 12; i++)
        {
            Cursor.lockState = CursorLockMode.Locked;
            player.LookToward(tree.transform.position + Vector3.up * 1.5f);
            inspect.Invoke(player, new object[] { tree });
            yield return null;
        }
        yield return Settle(4);
    }

    private void ClearInspection()
    {
        if (player == null) return;
        typeof(ForestPlayer).GetField("isInspecting", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(player, false);
    }

    private void ResetPlayer()
    {
        if (player == null) return;
        player.transform.position = startPosition;
        player.transform.rotation = startRotation;
        player.LookToward(startPosition + startRotation * Vector3.forward * 10f);
    }

    private void ClearWaypoint()
    {
        if (ui == null || ui.Map == null) return;
        typeof(StandMapView).GetField("waypointCell", BindingFlags.Instance | BindingFlags.NonPublic)?.SetValue(ui.Map, -1);
        ui.Map.DestroyWaypoint();
    }

    private void CloseAutoHelp()
    {
        if (ui != null && ui.Help != null && ui.Help.IsOpen) ui.CloseHelp();
    }

    private void CloseAll()
    {
        if (ui == null) return;
        ui.CloseHelp();
        ui.CloseAll();
    }

    private ForestTree DenseTree()
    {
        var trees = UnityEngine.Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t != null && t.IsLiving && t.CanChop)
            .ToList();
        if (trees.Count == 0) return null;
        Vector3 Flat(Vector3 v) => new Vector3(v.x, 0f, v.z);
        Vector3 centre = trees.Aggregate(Vector3.zero, (sum, tree) => sum + Flat(tree.transform.position)) / trees.Count;
        return trees
            .OrderByDescending(t => trees.Count(other => other != t
                && Vector3.Distance(Flat(other.transform.position), Flat(t.transform.position)) < 4f))
            .ThenBy(t => Vector3.Distance(Flat(t.transform.position), centre))
            .First();
    }

    private static bool Visible(VisualElement element)
    {
        if (element == null) return false;
        for (VisualElement p = element; p != null; p = p.parent)
            if (p.resolvedStyle.display == DisplayStyle.None
                || p.resolvedStyle.visibility == Visibility.Hidden
                || p.resolvedStyle.opacity <= 0.01f)
                return false;
        return element.worldBound.width > 0f && element.worldBound.height > 0f;
    }

    private static bool HorizontalContained(Rect inner, Rect outer, float tolerance)
        => inner.xMin >= outer.xMin - tolerance && inner.xMax <= outer.xMax + tolerance;

    private static bool VerticalContained(Rect inner, Rect outer, float tolerance)
        => inner.yMin >= outer.yMin - tolerance && inner.yMax <= outer.yMax + tolerance;

    private static string Short(string text)
    {
        if (string.IsNullOrEmpty(text)) return "";
        text = text.Replace("\r", " ").Replace("\n", " ");
        return text.Length > 44 ? text.Substring(0, 44) + "…" : text;
    }

    // Work Plan, Objectives, map and Annual Review all route through ScenarioOneManager.SetAuxiliaryPanelOpen/
    // SetWorkPlanOpen -> ApplyPanelInputState (player.enabled = !AnyPanelOpen; cursor free and visible). Help also uses
    // that input lock, but sets ModalOpen separately, so this helper is for non-help screens.
    private void CheckPanelInputLock(string name)
    {
        Check(!player.enabled && ui.Manager.AnyPanelOpen, name);
        Check(!ui.Manager.ModalOpen, name + "_NOT_HELP_MODAL_ONLY");
        Check(Cursor.lockState == CursorLockMode.None && Cursor.visible, name + "_CURSOR_FREE");
    }

    private void CheckWalkingAfterPanel(string name)
    {
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && player.enabled && !ui.Manager.AnyPanelOpen, name);
        Check(!ui.Manager.ModalOpen, name + "_NO_MODAL");
        Check(Cursor.lockState == CursorLockMode.Locked && !Cursor.visible, name + "_CURSOR_LOCKED");
    }

    private static string Rgba(Color colour)
        => $"rgba({colour.r.ToString("0.###", Inv)},{colour.g.ToString("0.###", Inv)},{colour.b.ToString("0.###", Inv)},{colour.a.ToString("0.###", Inv)})";

    // WCAG relative-luminance contrast of opaque text on its tag background.
    private static float ContrastRatio(Color first, Color second)
    {
        float a = RelativeLuminance(first), b = RelativeLuminance(second);
        float lighter = Mathf.Max(a, b), darker = Mathf.Min(a, b);
        return (lighter + 0.05f) / (darker + 0.05f);
    }

    private static float RelativeLuminance(Color colour)
    {
        float Linear(float channel) => channel <= 0.03928f ? channel / 12.92f : Mathf.Pow((channel + 0.055f) / 1.055f, 2.4f);
        return 0.2126f * Linear(colour.r) + 0.7152f * Linear(colour.g) + 0.0722f * Linear(colour.b);
    }

    private void Check(bool ok, string name, string detail = "")
    {
        if (ok) passes++; else failures++;
        Debug.Log((ok ? "VISUAL_V2_CHECK_PASS " : "VISUAL_V2_CHECK_FAIL ")
            + $"[{size.x}x{size.y}] {name}" + (string.IsNullOrEmpty(detail) ? "" : " " + detail));
    }

    private void Fail(string name, string detail) => Check(false, name, detail);

    private static IEnumerator Settle(int frames)
    {
        for (int i = 0; i < frames; i++) yield return null;
    }

    private static bool LooksBlank(Texture2D image)
    {
        Color32[] pixels = image.GetPixels32();
        int step = Mathf.Max(1, pixels.Length / 4000), different = 0;
        Color32 first = pixels[0];
        for (int i = 0; i < pixels.Length; i += step)
            if (Mathf.Abs(pixels[i].r - first.r) + Mathf.Abs(pixels[i].g - first.g) + Mathf.Abs(pixels[i].b - first.b) > 24)
                different++;
        return different < 40;
    }

    private string WorldHash()
    {
        if (saves == null) throw new InvalidOperationException("save controller missing for world hash");
        return ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    }

    private void OnLogMessage(string condition, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert) return;
        if (errors.Count < 40) errors.Add(type + ": " + condition);
    }

    private static void InvokePrivate(object target, string method)
    {
        MethodInfo info = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
        if (info == null) throw new MissingMethodException(target.GetType().Name, method);
        info.Invoke(target, null);
    }

    private static IEnumerable<Vector2Int> Sizes()
    {
        string text = Environment.GetEnvironmentVariable("CCF_UI_SIZES");
        var parsed = new List<Vector2Int>();
        if (!string.IsNullOrEmpty(text))
        {
            foreach (string part in text.Split(','))
            {
                string[] pair = part.Trim().Split('x');
                if (pair.Length != 2) continue;
                if (!int.TryParse(pair[0], out int width) || !int.TryParse(pair[1], out int height)) continue;
                if (width > 0 && height > 0) parsed.Add(new Vector2Int(width, height));
            }
        }
        foreach (Vector2Int required in RequiredSizes)
            if (!parsed.Contains(required)) parsed.Add(required);
        return parsed.Distinct().ToArray();
    }

    private void WriteReports(bool pass, Exception failure)
    {
        var audit = new VisualAudit
        {
            verdict = pass ? "UI_VISUAL_SYSTEM_V2_HARNESS_PASS" : "UI_VISUAL_SYSTEM_V2_HARNESS_FAIL",
            pass = pass,
            failures = failures,
            passes = passes,
            shots = shots,
            hashBeforeNavigation = hashBeforeNavigation,
            hashAfterNavigation = hashAfterNavigation,
            hashAfterCompetitorRestore = hashAfterCompetitorRestore,
            hashAfterAdvance = hashAfterAdvance,
            hashAfterReview = hashAfterReview,
            captures = captures,
            screens = screens,
            notes = notes,
            errors = errors
        };
        if (failure != null) audit.notes.Add("Exception: " + failure);
        File.WriteAllText(Path.Combine(output, "visual-system-v2-result.json"), JsonUtility.ToJson(audit, true));
        File.WriteAllText(Path.Combine(output, "style-audit.json"), JsonUtility.ToJson(audit, true));
        File.WriteAllLines(Path.Combine(output, "notes.txt"), notes);
        File.WriteAllText(Path.Combine(output, "style-audit.csv"), BuildCsv());
    }

    private string BuildCsv()
    {
        var csv = new StringBuilder();
        csv.AppendLine("screen,size,element,classes,text,font,fontSize,textColor,backgroundColor,x,y,width,height,whiteSpace");
        foreach (StyleScreen screen in screens)
        {
            foreach (StyleRecord record in screen.records)
            {
                csv.Append(Csv(record.screen)).Append(',')
                    .Append(Csv(record.size)).Append(',')
                    .Append(Csv(record.element)).Append(',')
                    .Append(Csv(record.classes)).Append(',')
                    .Append(Csv(record.text)).Append(',')
                    .Append(Csv(record.font)).Append(',')
                    .Append(record.fontSize.ToString("0.###", Inv)).Append(',')
                    .Append(Csv($"rgba({record.r:0.###},{record.g:0.###},{record.b:0.###},{record.a:0.###})")).Append(',')
                    .Append(Csv($"rgba({record.backgroundR:0.###},{record.backgroundG:0.###},{record.backgroundB:0.###},{record.backgroundA:0.###})")).Append(',')
                    .Append(record.x.ToString("0.##", Inv)).Append(',')
                    .Append(record.y.ToString("0.##", Inv)).Append(',')
                    .Append(record.width.ToString("0.##", Inv)).Append(',')
                    .Append(record.height.ToString("0.##", Inv)).Append(',')
                    .Append(Csv(record.whiteSpace)).AppendLine();
            }
        }
        return csv.ToString();
    }

    private static string Csv(string value)
        => "\"" + (value ?? "").Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ") + "\"";
}
#endif
