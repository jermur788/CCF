using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Scenario One player interface (UI Toolkit): walking HUD, tree inspection,
// Work Plan, annual review and stand map. Added by ScenarioOneManager at play
// time. Presentation only: every value is read from the authoritative systems
// and every action calls an existing ScenarioOneManager / marking API.
//
// Keys: Tab Work Plan (owned by ScenarioOneManager), M stand map, Esc closes
// the open map or review. X marks trees for felling.
public sealed class ScenarioOneUiRoot : MonoBehaviour
{
    public enum UiScreen { None, Map, Review, Objectives }

    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestPlayer player;
    private ForestTreeMarkingManager marking;
    private UIDocument document;
    private PanelSettings panelSettings;

    private WalkingHudView hud;
    private TreeInspectionView inspection;
    private WorkPlanView workPlan;
    private AnnualReviewView review;
    private StandMapView map;
    private LearningObjectivesView learning;
    private CompetitorAssessment competitors;
    public CompetitorAssessment Competitors => competitors;
    public LearningObjectivesView Learning => learning;
    private Label previewBanner;
    private MenuHelpView help;
    public MenuHelpView Help => help;
    public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;

    private UiScreen screen = UiScreen.None;
    private bool workPlanWasOpen;
    private float nextModalRefresh;
    private int helpClosedFrame = -1;

    public UiScreen CurrentScreen => screen;
    public ScenarioOneManager Manager => manager;
    public ForestEcologyController Ecology => ecology;
    public ForestPlayer Player => player;
    public ForestTreeMarkingManager Marking => marking;
    public VisualElement RootElement => document != null ? document.rootVisualElement : null;
    public StandMapView Map => map;

    private void Start()
    {
        manager = GetComponent<ScenarioOneManager>() ?? FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        if (ecology != null)
            ecology.StandGeometryApplied += OnStandGeometryApplied;
        player = FindFirstObjectByType<ForestPlayer>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();

        panelSettings = ScriptableObject.CreateInstance<PanelSettings>();
        panelSettings.name = "Scenario One UI (runtime)";
        panelSettings.themeStyleSheet = Resources.Load<ThemeStyleSheet>("ScenarioOneUiTheme");
        panelSettings.scaleMode = PanelScaleMode.ScaleWithScreenSize;
        panelSettings.referenceResolution = new Vector2Int(1600, 900);
        panelSettings.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
        panelSettings.match = 0.5f;
        panelSettings.sortingOrder = 10;

        var host = new GameObject("Scenario One UI");
        host.transform.SetParent(transform, false);
        document = host.AddComponent<UIDocument>();
        document.panelSettings = panelSettings;

        VisualElement root = document.rootVisualElement;
        root.AddToClassList("root");
        root.pickingMode = PickingMode.Ignore;

        learning = new LearningObjectivesView(this);
        hud = new WalkingHudView(this);
        inspection = new TreeInspectionView(this);
        workPlan = new WorkPlanView(this);
        review = new AnnualReviewView(this);
        map = new StandMapView(this);
        root.Add(hud.Root);
        competitors = new CompetitorAssessment(this, root);
        root.Add(inspection.Root);
        root.Add(inspection.CompetitorPanel);
        competitors.OccludingPanels.Add(inspection.Root);
        competitors.OccludingPanels.Add(inspection.CompetitorPanel);
        root.Add(workPlan.Root);
        root.Add(review.Root);
        root.Add(map.Root);
        root.Add(learning.Root);

        previewBanner = UiKit.Text("", "panel", "body");
        previewBanner.style.position = Position.Absolute;
        previewBanner.style.top = 14;
        previewBanner.style.alignSelf = Align.Center;
        root.Add(previewBanner);
        help = new MenuHelpView(this);
        root.Add(help.Root);
    }

    // The map holds cell indices of one grid; follow a geometry change instead of reading them on the new grid.
    private void OnStandGeometryApplied(int model)
    {
        if (map != null) map.OnStandGeometryApplied(ecology, model);
    }

    private void OnDestroy()
    {
        if (ecology != null)
            ecology.StandGeometryApplied -= OnStandGeometryApplied;
        if (map != null) map.DestroyWaypoint();
        competitors?.Destroy();
        if (panelSettings != null) Destroy(panelSettings);
    }

    private void Update()
    {
        if (manager == null || document == null)
            return;
        bool preview = manager.ReferencePreviewActive;
        if (preview) CloseHelp();
        else HandleKeys();

        bool planOpen = manager.WorkPlanOpen && !preview;
        if (planOpen && screen != UiScreen.None)
            SetScreen(UiScreen.None);
        if (planOpen && !workPlanWasOpen)
            workPlan.Refresh(true);
        workPlanWasOpen = planOpen;

        bool walking = !preview && !planOpen && screen == UiScreen.None;
        hud.Root.style.display = walking ? DisplayStyle.Flex : DisplayStyle.None;
        bool inspecting = walking && player != null && player.InspectedTree != null;
        inspection.Root.style.display = inspecting ? DisplayStyle.Flex : DisplayStyle.None;
        workPlan.Root.style.display = planOpen ? DisplayStyle.Flex : DisplayStyle.None;
        review.Root.style.display = !preview && screen == UiScreen.Review ? DisplayStyle.Flex : DisplayStyle.None;
        map.Root.style.display = !preview && screen == UiScreen.Map ? DisplayStyle.Flex : DisplayStyle.None;
        learning.Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None;
        previewBanner.style.display = preview ? DisplayStyle.Flex : DisplayStyle.None;
        if (preview)
            previewBanner.text = $"REFERENCE FUTURE v1 — YEAR {manager.ReferencePreviewYear}  ·  [Tab] Return to your forest";

        if (walking)
        {
            competitors.Update(player != null ? player.InspectedTree : null);
            hud.Refresh();
            if (player != null && player.InspectedTree != null)
                inspection.Refresh(player.InspectedTree);
        }
        else
            competitors.Clear();
        PlaceCompetitorPanel(inspecting && inspection.ShowsCompetitors);
        if (!preview)
        {
            MenuHelpView.Menu context = HelpContext();
            if (help.IsOpen && help.CurrentMenu != context) CloseHelp();
            if (screen != UiScreen.Objectives && (context != MenuHelpView.Menu.AnnualReview || manager.AnnualReports.Count > 0))
                help.Introduce(context);
            manager.SetAuxiliaryPanelOpen(screen != UiScreen.None || help.IsOpen || helpClosedFrame == Time.frameCount);
        }

        if (Time.unscaledTime >= nextModalRefresh)
        {
            nextModalRefresh = Time.unscaledTime + 0.25f;
            if (!preview) learning.Observe();
            if (planOpen) workPlan.Refresh(false);
            if (screen == UiScreen.Review) review.Refresh(false);
            if (screen == UiScreen.Map) map.Refresh(false);
            if (screen == UiScreen.Objectives) learning.Refresh(false);
        }
    }

    // The competitor panel sits on the right, just under the HUD status panel,
    // and never extends past the bottom of the screen.
    private void PlaceCompetitorPanel(bool show)
    {
        VisualElement panel = inspection.CompetitorPanel;
        panel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        if (!show) return;
        VisualElement status = hud.Root.Q(className: "hud-status");
        float top = status != null && status.layout.height > 0f ? status.layout.yMax + 10f : 250f;
        float available = document.rootVisualElement.layout.height - top - 14f;
        panel.style.top = top;
        if (available > 0f) panel.style.maxHeight = available;
    }

    private void LateUpdate()
    {
        if (competitors == null || manager == null) return;
        bool walking = !manager.ReferencePreviewActive && !manager.WorkPlanOpen && screen == UiScreen.None;
        competitors.LateUpdate(walking && player != null ? player.InspectedTree : null);
    }

    private void HandleKeys()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        if (help.IsOpen && keyboard.escapeKey.wasPressedThisFrame)
        {
            CloseHelp();
            return;
        }
        if (keyboard.f1Key.wasPressedThisFrame)
        {
            if (help.IsOpen) CloseHelp(); else ShowHelp(HelpContext());
            return;
        }
        if (keyboard.oKey.wasPressedThisFrame)
        {
            if (manager.WorkPlanOpen) manager.OpenWorkPlan(false);
            SetScreen(screen == UiScreen.Objectives ? UiScreen.None : UiScreen.Objectives);
        }
        else if (keyboard.mKey.wasPressedThisFrame)
        {
            if (manager.WorkPlanOpen) manager.OpenWorkPlan(false);
            SetScreen(screen == UiScreen.Map ? UiScreen.None : UiScreen.Map);
        }
        else if (screen != UiScreen.None && keyboard.escapeKey.wasPressedThisFrame)
            SetScreen(UiScreen.None);
        else if (screen != UiScreen.None && keyboard.tabKey.wasPressedThisFrame)
            // ScenarioOneManager opens the Work Plan on Tab in the same frame.
            SetScreen(UiScreen.None);
    }

    public bool ShowStormWaypoint(int cell)
    {
        if (manager.ReferencePreviewActive || !map.SetWaypointCell(ecology, cell)) return false;
        SetScreen(UiScreen.Map);
        return true;
    }

    public void SetScreen(UiScreen next)
    {
        if (screen == next)
            return;
        if (screen == UiScreen.Map && next == UiScreen.None && map.HasWaypoint)
            learning.Record("map.return");
        screen = next;
        CloseHelp();
        manager.SetAuxiliaryPanelOpen(screen != UiScreen.None);
        if (screen == UiScreen.Review)
        {
            review.Refresh(true);
        }
        else if (screen == UiScreen.Map)
        {
            learning.Record("map.open");
            map.Refresh(true);
        }
        else if (screen == UiScreen.Objectives)
            learning.Refresh(true);
    }

    private MenuHelpView.Menu HelpContext()
    {
        if (manager.WorkPlanOpen) return MenuHelpView.Menu.WorkPlan;
        if (screen == UiScreen.Map) return MenuHelpView.Menu.StandMap;
        if (screen == UiScreen.Review) return MenuHelpView.Menu.AnnualReview;
        if (player != null && player.InspectedTree != null) return MenuHelpView.Menu.TreeInspection;
        return MenuHelpView.Menu.WalkingHud;
    }

    public void ShowCurrentHelp() => ShowHelp(HelpContext());

    public void ShowHelp(MenuHelpView.Menu menu)
    {
        if (manager.ReferencePreviewActive) return;
        help.Show(menu);
        manager.SetAuxiliaryPanelOpen(true);
    }

    public void CloseHelp()
    {
        if (help == null || !help.IsOpen) return;
        help.Close();
        // Do not let this Escape/click also act on the forest in the same frame.
        helpClosedFrame = Time.frameCount;
        manager.SetAuxiliaryPanelOpen(true);
    }

    public void AcknowledgeAnnualReview()
    {
        if (screen != UiScreen.Review || manager.AnnualReports.Count == 0) return;
        manager.MarkAnnualReviewSeen();
        review.Refresh(true);
    }

    // A UI learning gate only: simulation/reference harness APIs remain unchanged.
    public bool AdvanceFromWorkPlan()
    {
        if (NeedsAnnualReview) { ShowReview(); return false; }
        if (!manager.AdvanceYear()) return false;
        ShowReview();
        return true;
    }

    public void ShowWorkPlan()
    {
        CloseHelp();
        SetScreen(UiScreen.None);
        manager.OpenWorkPlan(true);
        workPlan.Refresh(true);
    }

    public void ShowReview()
    {
        manager.OpenWorkPlan(false);
        SetScreen(UiScreen.Review);
    }

    public void ShowMap()
    {
        manager.OpenWorkPlan(false);
        SetScreen(UiScreen.Map);
    }

    public void ShowObjectives()
    {
        manager.OpenWorkPlan(false);
        SetScreen(UiScreen.Objectives);
    }

    public void CloseAll()
    {
        CloseHelp();
        manager.OpenWorkPlan(false);
        SetScreen(UiScreen.None);
    }
}
