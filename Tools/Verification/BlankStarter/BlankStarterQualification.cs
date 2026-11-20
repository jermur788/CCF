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
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

// Copy temporarily to Assets/ForestPrototype; remove that copy and its .meta after the Editor exits.
// Starts only for the named -executeMethod. Blank mode must run inside a path-filtered strace;
// historical mode loads and round-trips a separately staged copy in an isolated config root.
public static class BlankStarterQualification
{
    public static void Begin()
    {
        string mode = Environment.GetEnvironmentVariable("CCF_BLANK_MODE");
        string scene = mode == "historical" ? "ForestTest" : "BlankStarter";
        EditorSceneManager.OpenScene($"Assets/Scenes/{scene}.unity");
        // Fixed Game view resolution, using the same Editor API pattern as VisualSystemFlowShots.
        const BindingFlags all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        object sizes = typeof(ScriptableSingleton<>).MakeGenericType(sizesType)
            .GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        object size = Activator.CreateInstance(assembly.GetType("UnityEditor.GameViewSize"), all, null,
            new object[] { Enum.ToObject(assembly.GetType("UnityEditor.GameViewSizeType"), 1), 1280, 720, "#857 verification" }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type gameViewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow gameView = EditorWindow.GetWindow(gameViewType);
        gameViewType.GetProperty("selectedSizeIndex", all).SetValue(gameView, index);
        gameView.Show();
        gameView.Focus();
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Contains("BlankStarterQualification.Begin")) return;
        var host = new GameObject("Disposable #857 qualification");
        Object.DontDestroyOnLoad(host);
        host.AddComponent<BlankStarterQualificationRunner>();
    }
}

public sealed class BlankStarterQualificationRunner : MonoBehaviour
{
    private const BindingFlags PrivateInstance = BindingFlags.Instance | BindingFlags.NonPublic;
    private const string Banner = "Development preview — unsaved";
    private readonly List<string> runtimeErrors = new List<string>();
    private string output;
    private int passed;

    private void OnEnable() => Application.logMessageReceived += OnLog;
    private void OnDisable() => Application.logMessageReceived -= OnLog;

    private void OnLog(string message, string stack, LogType kind)
    {
        if ((kind == LogType.Exception || kind == LogType.Error || kind == LogType.Assert)
            && !stack.Contains("UnityEditor.Search"))
            runtimeErrors.Add(message + "\n" + stack);
    }

    private void Check(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException(name);
        passed++;
        Debug.Log("CCF857 PASS " + name);
    }

    private IEnumerator Start()
    {
        output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        if (string.IsNullOrEmpty(output) || !Directory.Exists(output))
        {
            Debug.LogError("CCF857 missing evidence directory");
            EditorApplication.Exit(2);
            yield break;
        }
        bool historical = Environment.GetEnvironmentVariable("CCF_BLANK_MODE") == "historical";
        var stack = new Stack<IEnumerator>();
        stack.Push(historical ? Historical() : Blank());
        Exception failure = null;
        while (stack.Count != 0)
        {
            bool more;
            object yielded = null;
            try { more = stack.Peek().MoveNext(); if (more) yielded = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) stack.Pop();
            else if (yielded is IEnumerator nested) stack.Push(nested);
            else yield return yielded;
        }
        foreach (string error in runtimeErrors) Debug.LogWarning("CCF857 runtime error evidence: " + error);
        bool success = failure == null && runtimeErrors.Count == 0;
        Debug.Log($"CCF857 {(success ? "PASS" : "FAIL")} total={passed} mode={(historical ? "historical" : "blank")} failure={failure} runtimeErrors={runtimeErrors.Count}");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(success ? 0 : 1);
    }

    private static IEnumerator Frames(int count)
    {
        for (int i = 0; i < count; ++i) yield return null;
    }

    private IEnumerator Screenshot(string name)
    {
        yield return new WaitForEndOfFrame();
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width > 500 && image.height > 300, $"rendered frame {name} {Screen.width}×{Screen.height}");
        File.WriteAllBytes(Path.Combine(output, name + ".png"), image.EncodeToPNG());
        Destroy(image);
    }

    private static void Invoke(object instance, string method) =>
        instance.GetType().GetMethod(method, PrivateInstance).Invoke(instance, null);

    private static void SetField(object instance, string field, object value) =>
        instance.GetType().GetField(field, PrivateInstance).SetValue(instance, value);

    private static object GetField(object instance, string field) =>
        instance.GetType().GetField(field, PrivateInstance).GetValue(instance);

    private static bool Visible(VisualElement element)
    {
        if (element == null || element.panel == null) return false;
        for (VisualElement parent = element; parent != null; parent = parent.parent)
            if (parent.resolvedStyle.display == DisplayStyle.None || parent.resolvedStyle.visibility != Visibility.Visible)
                return false;
        return true;
    }

    private static bool Text(VisualElement root, string text) =>
        root.Query<TextElement>().ToList().Any(label => label.text == text && Visible(label));

    private static VisualElement MenuRoot(StandaloneSessionMenu menu) => menu.GetComponent<UIDocument>().rootVisualElement;

    // Uses the Unity Input System's native update/device path, not direct calls into game key handlers.
    private static IEnumerator KeyPress(Keyboard keyboard, params Key[] keys)
    {
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(keys));
        yield return null;
        yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        yield return null;
        yield return null;
    }

    private IEnumerator WaitForMenu()
    {
        for (int i = 0; i < 900 && !StandaloneSessionMenu.IsOpen; ++i) yield return null;
        Check(StandaloneSessionMenu.IsOpen, "real startup opened");
    }

    private IEnumerator Blank()
    {
        Check(Application.unityVersion == "6000.6.0f1", "Editor version");
        Check(Application.persistentDataPath == Environment.GetEnvironmentVariable("CCF_857_WITNESS_PATH"),
            "Unity resolves the configured witness directory: " + Application.persistentDataPath);
        Check(PlayerPrefs.HasKey(LearningObjectivesView.PreferencePrefix + "rack.create"),
            "existing rack lesson preference present as the isolation witness (read only)");
        yield return WaitForMenu();
        var menu = FindFirstObjectByType<StandaloneSessionMenu>();
        var ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        var saves = FindFirstObjectByType<ForestSaveController>();
        Scene scene = SceneManager.GetActiveScene();
        Check(scene.path == "Assets/Scenes/BlankStarter.unity" && !EditorBuildSettings.scenes.Any(entry => entry.path == scene.path),
            $"direct Editor entry into the unlisted blank scene (reported build index {scene.buildIndex})");
        Check(ui != null && ui.BlankPreview != null && saves != null, "scene binds the shared UI and the guarded save controller");
        Check(!menu.ContinueAvailable && menu.ContinueNote == BlankStarterPreview.SaveRefusal, "startup RefreshContinueState refuses visibly");
        menu.ContinueSavedForest();
        Check(menu.StartupScreenActive && StandaloneSessionMenu.IsOpen && !menu.ContinueAvailable, "direct startup Continue refuses");
        Check(!saves.CanLoad(out string reason) && reason == BlankStarterPreview.SaveRefusal, "direct CanLoad refuses");
        yield return Frames(3);
        yield return Screenshot("01-blank-startup");
        // Run the actual UI handler behind the safe 'Open preview' button, not only the public close path.
        Invoke(menu, "StartNewScenarioClicked");
        yield return Frames(8);
        Check(!StandaloneSessionMenu.IsOpen && SceneManager.GetActiveScene().name == "BlankStarter", "startup Open preview does not switch scene");
        CheckWorld(ui, 30, 30);
        Check(Text(ui.RootElement, Banner) && Text(ui.RootElement, "Tree inspection unavailable."), "preview indicator and unavailable inspection");
        foreach (string heading in new[] { "Work Plan [Tab]", "Stand Map [M]", "Annual Review", "Objectives [O]" })
            Check(Text(ui.RootElement, heading), "shared walking HUD navigation " + heading);
        Check(ui.Player.InspectedTree == null && ui.Player.CarriedWood == 0, "walking player has no tree or wood");
        Check((BlankStarterPreview)GetField(ui.Player, "blankStarterPreview") == ui.BlankPreview, "player cached preview before walking (legacy IMGUI off)");
        yield return Screenshot("02-blank-walking-hud");

        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        Mouse mouse = InputSystem.AddDevice<Mouse>();
        try
        {
            Check(Application.isFocused, "graphical Game view focused for walk/look");
            Vector3 pose = ui.Player.transform.position;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            yield return Frames(5);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
            yield return Frames(15);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState());
            yield return Frames(2);
            Vector3 moved = ui.Player.transform.position - pose;
            Check(new Vector2(moved.x, moved.z).magnitude > 0.05f, "W via Input System moves the real player: " + moved);
            Quaternion rotation = ui.Player.transform.rotation;
            InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(22, -11));
            yield return Frames(3);
            Check(Quaternion.Angle(rotation, ui.Player.transform.rotation) > 0.2f, "mouse delta changes real look direction");
            Check(ui.Player.GetComponent<CharacterController>().isGrounded, "character grounded after moving");

            yield return KeyPress(keyboard, Key.E, Key.F, Key.G, Key.T, Key.B, Key.X, Key.F1);
            Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && ui.Help == null && ui.Player.InspectedTree == null
                && ui.Player.CarriedWood == 0, "inspection/Fell/plant/time/rack/help input leaves preview empty");
            Check(!ui.ApproveWorkPlan() && !ui.AdvanceFromWorkPlan() && !ui.TryShowReviewYear(1), "direct plan/year/review actions refuse");
            yield return KeyPress(keyboard, Key.Tab);
            Check(Text(ui.RootElement, "Work Plan") && Text(ui.RootElement, "No eligible operations in this blank starter."), "exact Work Plan empty state");
            RejectVisibleActions(ui.RootElement, "approve", "year+", "purchase", "quote", "cash", "fell");
            yield return Screenshot("03-blank-work-plan");
            ui.ShowReview();
            yield return Frames(3);
            Check(Text(ui.RootElement, "Annual Review") && Text(ui.RootElement, "No year has been advanced; no annual review is available."), "exact Annual Review empty state");
            RejectVisibleActions(ui.RootElement, "earlier", "later", "read the annual", "completion");
            yield return Screenshot("04-blank-annual-review");
            yield return KeyPress(keyboard, Key.O);
            Check(Text(ui.RootElement, "Objectives") && Text(ui.RootElement, "No objectives configured (development preview)."), "exact Objectives empty state");
            RejectVisibleActions(ui.RootElement, "completed lessons", "rack", "current objective");
            yield return Screenshot("05-blank-objectives");
            yield return BindSlot(ui, "objective", "blank-preview-objective", "[SCENARIO OBJECTIVE — DEV ONLY]");
            yield return KeyPress(keyboard, Key.O);
            yield return BindSlot(ui, "introduction", "blank-preview-introduction", "[SCENARIO INTRODUCTION — DEV ONLY]");
            yield return BindSlot(ui, "guidance", "blank-preview-guidance", "[SCENARIO GUIDANCE — DEV ONLY]");

            yield return KeyPress(keyboard, Key.M);
            CheckMap(ui, 30, 30);
            yield return KeyPress(keyboard, Key.B, Key.Delete);
            Check(!ui.Map.RackMode && !ui.Map.RackPlanningAvailable && ui.Map.RackEditor == null && ui.RackTool == null
                && ui.PlannedMarkers == null && ui.RackLayout != null && ui.RackLayout.Lines.Count == 0
                && !ui.Minimap.TeachingNoteVisible, "no rack edits, tools, markers, guidance or layout rows");
            yield return Screenshot("06-blank-map30");
            yield return KeyPress(keyboard, Key.Escape);

            yield return KeyPress(keyboard, Key.F5);
            Check((string)GetField(saves, "message") == BlankStarterPreview.SaveRefusal, "F5 while walking refused");
            SetField(saves, "message", "");
            yield return KeyPress(keyboard, Key.F9);
            Check((string)GetField(saves, "message") == BlankStarterPreview.SaveRefusal, "F9 while walking refused");

            yield return KeyPress(keyboard, Key.F10);
            Check(StandaloneSessionMenu.IsOpen && !saves.enabled && Text(MenuRoot(menu), BlankStarterPreview.SaveRefusal), "F10 builds existing session menu with visible refusal");
            RejectVisibleActions(MenuRoot(menu), "save current", "load", "fresh", "tutorial", "restart");
            yield return Screenshot("07-blank-F10");
            yield return KeyPress(keyboard, Key.F5);
            yield return KeyPress(keyboard, Key.F9);
            menu.SaveAndClose();
            menu.Restart();
            Check(StandaloneSessionMenu.IsOpen && SceneManager.GetActiveScene().name == "BlankStarter", "menu SaveAndClose and Restart preserve open session");
            saves.Save();
            Check((string)GetField(saves, "message") == BlankStarterPreview.SaveRefusal, "direct public Save refuses");
            SetField(saves, "message", "");
            saves.Load();
            Check((string)GetField(saves, "message") == BlankStarterPreview.SaveRefusal, "direct public Load refuses");
            yield return KeyPress(keyboard, Key.F10);
            Check(!StandaloneSessionMenu.IsOpen && saves.enabled, "F10 resumes same in-memory preview");
            yield return KeyPress(keyboard, Key.F5);
            yield return KeyPress(keyboard, Key.F9);
            CheckWorld(ui, 30, 30);
        }
        finally
        {
            InputSystem.RemoveDevice(mouse);
            InputSystem.RemoveDevice(keyboard);
        }

        yield return InvalidFootprints(ui.BlankPreview);
        yield return EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/BlankStarter45x60.unity", new LoadSceneParameters(LoadSceneMode.Single));
        yield return Frames(8);
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(SceneManager.GetActiveScene().name == "BlankStarter45x60" && Text(ui.RootElement, "Blank Starter (TEST ONLY)"),
            "new test-only scene opens with shared runtime scripts");
        CheckWorld(ui, 45, 60);
        yield return Screenshot("08-copy-walking-hud");
        ui.ShowMap();
        yield return Frames(3);
        CheckMap(ui, 45, 60);
        yield return Screenshot("09-copy-map45x60");
        yield return InvalidFootprints(ui.BlankPreview);
    }

    private void CheckWorld(ScenarioOneUiRoot ui, float width, float depth)
    {
        Check(ui.BlankPreview.TryGetGroundBounds(out Bounds ground), "real ground collider exists");
        Bounds renderer = GameObject.Find("Ground").GetComponent<MeshRenderer>().bounds;
        float margin = Mathf.Min(
            width * 0.5f - Mathf.Abs(ui.Player.transform.position.x), depth * 0.5f - Mathf.Abs(ui.Player.transform.position.z));
        Check(Mathf.Abs(ground.size.x - width) <= 0.05f && Mathf.Abs(ground.size.z - depth) <= 0.05f
            && Mathf.Abs(renderer.size.x - width) <= 0.05f && Mathf.Abs(renderer.size.z - depth) <= 0.05f
            && Mathf.Abs(ground.max.y) < 0.001f && Mathf.Abs(renderer.max.y) < 0.001f,
            $"collider {ground.size} renderer {renderer.size} surface y={ground.max.y}");
        float radius = ui.Player.GetComponent<CharacterController>().radius;
        Check(margin >= radius, $"player position {ui.Player.transform.position}, margin {margin:F3} ≥ radius {radius:F3}");
        Check(SceneManager.GetActiveScene().GetRootGameObjects().Length == 4 && ui.Manager == null && ui.Ecology == null && ui.Marking == null
            && FindFirstObjectByType<ForestTree>() == null && FindFirstObjectByType<ForestTreeSpawner>() == null
            && FindFirstObjectByType<ForestBuildable>() == null && FindFirstObjectByType<ForestWoodStorage>() == null,
            "four expected scene roots, no manager/ecology/forest actors");
    }

    private void CheckMap(ScenarioOneUiRoot ui, float width, float depth)
    {
        Check(Text(ui.RootElement, "Stand Map") && Text(ui.RootElement, $"{width} × {depth} m · Level ground"), "truthful map headings and dimensions");
        StandMapModel model = ui.MapSource.Rebuild();
        Check(model.HasProperty && model.GroundIsLevel && model.HasPlayer && !model.HasWaypoint && model.CellsPerAxis == 0 && model.CellSizeM == 0
            && model.Racks.Count == 0 && model.Trees.Count == 0 && model.Juveniles.Count == 0 && model.Shelters.Count == 0
            && model.Paths.Count == 0 && model.Landmarks.Count == 0 && model.MiniGuidance.Count == 0,
            "all preview layers empty: racks trees juveniles shelters paths landmarks mini-guidance ecology grid and waypoint");
        var transform = model.TransformFor(StandMapView.MapSidePx);
        VisualElement boundary = ui.Map.Root.Q("blank-map-boundary");
        // Measure the actual boundary element's geometry in logical panel units. UI Toolkit
        // rounds the resolved layout to display pixels; check that separately at raster precision.
        float logicalWidth = boundary.style.width.value.value, logicalHeight = boundary.style.height.value.value;
        float projectedWidth = logicalWidth / transform.PixelsPerMetre;
        float projectedDepth = logicalHeight / transform.PixelsPerMetre;
        float xInset = (StandMapView.MapSidePx - logicalWidth) * 0.5f;
        float zInset = (StandMapView.MapSidePx - logicalHeight) * 0.5f;
        Check(Mathf.Abs(projectedWidth - width) <= 0.05f && Mathf.Abs(projectedDepth - depth) <= 0.05f
            && Mathf.Abs(xInset - boundary.style.left.value.value) <= 0.1f && Mathf.Abs(zInset - boundary.style.top.value.value) <= 0.1f,
            $"layout bounds {projectedWidth:F3}×{projectedDepth:F3} m, inset {xInset:F2},{zInset:F2}, {transform.PixelsPerMetre:F3} panel units/m");
        Vector2 a = RuntimePanelUtils.ScreenToPanel(boundary.panel, Vector2.zero);
        Vector2 b = RuntimePanelUtils.ScreenToPanel(boundary.panel, new Vector2(1f, 1f));
        float rounding = Mathf.Max(Mathf.Abs(b.x - a.x), Mathf.Abs(b.y - a.y)) + 0.01f;
        Check(Mathf.Abs(boundary.resolvedStyle.width - logicalWidth) <= rounding
            && Mathf.Abs(boundary.resolvedStyle.height - logicalHeight) <= rounding
            && Mathf.Abs(boundary.resolvedStyle.left - xInset) <= rounding
            && Mathf.Abs(boundary.resolvedStyle.top - zInset) <= rounding,
            $"rendered layout within one physical pixel: {boundary.resolvedStyle.width:F3}×{boundary.resolvedStyle.height:F3} panel units");
        RejectVisibleActions(ui.Map.Root, "rack", "light", "regeneration", "treatment", "clearing");
    }

    private void RejectVisibleActions(VisualElement root, params string[] forbidden)
    {
        foreach (Button button in root.Query<Button>().ToList().Where(Visible))
            Check(!forbidden.Any(text => button.text.IndexOf(text, StringComparison.OrdinalIgnoreCase) >= 0), "only safe button: " + button.text);
    }

    private IEnumerator BindSlot(ScenarioOneUiRoot ui, string field, string element, string marker)
    {
        string distinctive = "Exact authoring text for " + field + " #857";
        foreach (string value in new[] { "", distinctive, "" })
        {
            SetField(ui.BlankPreview, field, value);
            yield return Frames(3);
            Label label = ui.RootElement.Q<Label>(element);
            Check(Visible(label) && label.text == (value == "" ? marker : value) && Text(ui.RootElement, Banner),
                $"{field}: input='{value}' output='{label?.text}' alongside preview banner");
            if (value != "") yield return Screenshot("slot-" + field);
        }
    }

    private IEnumerator InvalidFootprints(BlankStarterPreview original)
    {
        // Real Awake and shared UI in an in-memory scene, without changing either serialized scene.
        Scene invalidScene = SceneManager.CreateScene("Disposable invalid " + original.Title);
        foreach (float bad in new[] { 0f, -1f, float.NaN, float.PositiveInfinity })
        {
            var host = new GameObject("Disposable invalid preview");
            host.SetActive(false);
            SceneManager.MoveGameObjectToScene(host, invalidScene);
            var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.transform.SetParent(host.transform, false);
            var player = Instantiate(original.Player.gameObject, host.transform);
            var preview = host.AddComponent<BlankStarterPreview>();
            var invalidUi = host.AddComponent<ScenarioOneUiRoot>();
            SetField(preview, "width", bad);
            SetField(preview, "depth", GetField(original, "depth"));
            SetField(preview, "ground", ground.GetComponent<BoxCollider>());
            SetField(preview, "player", player.GetComponent<ForestPlayer>());
            host.SetActive(true);
            yield return Frames(3);
            Check(preview.FootprintError?.StartsWith("Invalid footprint:") == true && !preview.TryGetGroundBounds(out _)
                && !ground.activeInHierarchy && !player.activeInHierarchy && preview.Player == null
                && Text(invalidUi.RootElement, preview.FootprintError) && Text(invalidUi.RootElement, Banner),
                $"{original.Title} invalid input {bad}: visible error copy, no active ground or player");
            Destroy(host);
            yield return null;
        }
        yield return SceneManager.UnloadSceneAsync(invalidScene);
    }

    private IEnumerator Historical()
    {
        Check(Application.persistentDataPath == Environment.GetEnvironmentVariable("CCF_857_HISTORICAL_PATH"),
            "historical save location matches isolated fixture");
        yield return WaitForMenu();
        var ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        var menu = FindFirstObjectByType<StandaloneSessionMenu>();
        var saves = FindFirstObjectByType<ForestSaveController>();
        Check(ui != null && ui.Manager != null && ui.BlankPreview == null && SceneManager.GetActiveScene().name == "ForestTest", "historical ForestTest entry");
        Check(ui.Ecology.StandGeometryModelVersion == StandGeometryModel.Legacy40 && ui.Ecology.CellCount == 64
            && FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Count(tree => tree.IsLiving) == 336,
            "legacy 40 m fresh-world geometry, 64 cells, 336 live trees");
        var frozen = ScenarioReferenceArchive.Load();
        Check(frozen != null && new[] { 0, 20, 50, 100 }.All(year => frozen.AtYear(year)?.verifiedFrozenWorld == true),
            "frozen Reference Future v1 four admitted milestones");
        Check(menu.ContinueAvailable && saves.CanLoad(out _), "historical fixture available through real Continue and CanLoad");
        menu.ContinueSavedForest();
        yield return Frames(10);
        Check(ui.Manager.CurrentEcologicalYear == 1 && ui.Manager.HistoryYears.Count == 1 && ui.Manager.WorkOrders.Count == 10,
            $"loaded authenticated v20 fixture: year {ui.Manager.CurrentEcologicalYear}, history {ui.Manager.HistoryYears.Count}, orders {ui.Manager.WorkOrders.Count}");
        Check(ui.RackTool != null && ui.RackLayout != null && ui.PlannedMarkers != null && ui.Ecology.StandBounds.width == 80,
            $"historical rack tools and Enlarged80 restored; groups={ui.Manager.RackGroups.Count}");
        ui.ShowMap();
        yield return Frames(5);
        StandMapModel model = ui.MapSource.Rebuild();
        Check(model.HasProperty && model.SizeM == 80 && model.DepthM == 80 && model.Trees.Count > 0 && model.Paths.Count > 0
            && model.Racks.Count + model.MiniGuidance.Count == 5,
            $"historical square map and populated layers: trees={model.Trees.Count} paths={model.Paths.Count}, racks={model.Racks.Count}");
        ui.CloseHelp();
        yield return Screenshot("10-historical-map");
        ui.ShowWorkPlan();
        yield return Frames(6);
        ui.CloseHelp();
        Check(ui.Manager.WorkPlanOpen && !Text(ui.RootElement, "No eligible operations in this blank starter."), "historical Work Plan remains active");
        yield return Screenshot("11-historical-work-plan");
        ui.ShowReview();
        yield return Frames(6);
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && !Text(ui.RootElement, "No year has been advanced; no annual review is available."),
            "historical Annual Review navigates to recorded year");
        yield return Screenshot("12-historical-review");

        ui.CloseAll();
        saves.Save();
        Check(saves.CanLoad(out _) && File.Exists(Path.Combine(Application.persistentDataPath, "forest-save.json.bak")),
            "historical current-version Save/CanLoad succeeds in the isolated config and keeps a backup");
        saves.Load();
        yield return Frames(3);
        Check(ui.Manager.CurrentEcologicalYear == 1 && ui.Manager.HistoryYears.Count == 1 && ui.Manager.WorkOrders.Count == 10,
            "isolated current-version round trip retains historical year, review and orders");

        int pathCount = model.Paths.Count;
        SetField(ui, "ecology", null);
        model = ui.MapSource.Rebuild();
        Check(pathCount > 0 && !model.HasProperty && !model.HasPlayer && !model.HasWaypoint
            && model.Paths.Count == 0 && model.Landmarks.Count == 0 && model.MiniGuidance.Count == 0
            && model.Trees.Count == 0 && model.Juveniles.Count == 0 && model.Shelters.Count == 0 && model.Racks.Count == 0
            && model.MinX == 0 && model.MinZ == 0 && model.SizeM == 0 && model.CellsPerAxis == 0 && model.CellSizeM == 0,
            "forced same-source populated→no-ecology transition clears all layers and geometry");
    }
}
#endif
