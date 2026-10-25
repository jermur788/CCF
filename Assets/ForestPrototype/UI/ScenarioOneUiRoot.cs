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
    private InspectedTreeIndicator indicator;
    private RackTool rackTool;
    private RackLayoutTracker rackLayout;
    private PlannedFellMarkers plannedMarkers;
    private StandMapSource mapSource;
    public CompetitorAssessment Competitors => competitors;
    // The one map model both the Stand Map and the HUD minimap are drawn from.
    public StandMapSource MapSource => mapSource;
    public HudMinimapView Minimap => hud != null ? hud.Minimap : null;
    public InspectedTreeIndicator Indicator => indicator;
    public TreeInspectionView Inspection => inspection;
    public RackTool RackTool => rackTool;
    public RackLayoutTracker RackLayout => rackLayout;
    public PlannedFellMarkers PlannedMarkers => plannedMarkers;
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

    // True when the screen position (origin at the bottom left, as the mouse reports it) is over a button of this UI. A press there
    // belongs to the button: the forest must not take the mouse back or inspect the tree behind it, or the button never sees the release.
    public bool IsOverButton(Vector2 screenPosition)
    {
        VisualElement root = RootElement;
        IPanel panel = root != null ? root.panel : null;
        if (panel == null) return false;
        VisualElement hit = panel.Pick(RuntimePanelUtils.ScreenToPanel(panel, new Vector2(screenPosition.x, Screen.height - screenPosition.y)));
        for (; hit != null; hit = hit.parent)
            if (hit is Button) return true;
        return false;
    }

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

        // Extraction racks: the in-world tool lives on the player, the markers and layout tracker are presentation helpers.
        rackLayout = new RackLayoutTracker(this);
        mapSource = new StandMapSource(this);
        if (manager != null)
        {
            rackTool = (player != null ? player.gameObject : gameObject).AddComponent<RackTool>();
            rackTool.Initialise(this, manager, ecology);
            plannedMarkers = gameObject.AddComponent<PlannedFellMarkers>();
            manager.RackConfirmed += OnRackConfirmed;
        }
        learning = new LearningObjectivesView(this);
        hud = new WalkingHudView(this);
        inspection = new TreeInspectionView(this);
        workPlan = new WorkPlanView(this);
        review = new AnnualReviewView(this);
        map = new StandMapView(this);
        root.Add(hud.Root);
        competitors = new CompetitorAssessment(this, root);
        indicator = new InspectedTreeIndicator();
        root.Add(inspection.Root);
        root.Add(inspection.CompetitorPanel);
        root.Add(inspection.EstimatePanel);
        competitors.OccludingPanels.Add(inspection.Root);
        competitors.OccludingPanels.Add(inspection.CompetitorPanel);
        // The objective widget and status panel sit in this column: a numbered tag drawn over their text is unreadable, so it waits behind them too.
        competitors.OccludingPanels.Add(hud.RightColumn);
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
        if (FindFirstObjectByType<StandaloneSessionMenu>() == null && manager != null && manager.CurrentEcologicalYear == 0 && manager.WorkOrders.Count == 0 && manager.AnnualReports.Count == 0)
            help.ShowScenarioIntroduction();
    }

    // A rack was added to the Work Plan, from the forest or from the map: the first lesson step is done either way.
    private void OnRackConfirmed(RackGroup group, string source)
    {
        rackLayout?.Update(true);
        // The maps are drawn from the same rack data: read it again now rather than at the next timer tick.
        mapSource?.MarkRacksDirty();
        learning?.Record("rack.create");
        if (source == "map") learning?.Record("rack.map");
    }

    // The map holds cell indices of one grid; follow a geometry change instead of reading them on the new grid.
    private void OnStandGeometryApplied(int model)
    {
        if (map != null) map.OnStandGeometryApplied(ecology, model);
        mapSource?.MarkRacksDirty();
    }

    private void OnDestroy()
    {
        if (ecology != null)
            ecology.StandGeometryApplied -= OnStandGeometryApplied;
        if (manager != null)
            manager.RackConfirmed -= OnRackConfirmed;
        if (map != null) map.DestroyWaypoint();
        mapSource?.Dispose();
        competitors?.Destroy();
        indicator?.Destroy();
        if (panelSettings != null) Destroy(panelSettings);
    }

    private void Update()
    {
        if (manager == null || document == null)
            return;
        UiKit.SetCompact(document.rootVisualElement);
        bool preview = manager.ReferencePreviewActive;
        if (preview) CloseHelp();
        else HandleKeys();

        bool planOpen = manager.WorkPlanOpen && !preview;
        if (planOpen && screen != UiScreen.None)
            SetScreen(UiScreen.None);
        if (planOpen && !workPlanWasOpen)
        {
            // Re-entry: check every open order against the forest as it is now before showing it.
            manager.RevalidateOpenOrders();
            workPlan.CancelAdvanceConfirmation();
            workPlan.Refresh(true);
        }
        workPlanWasOpen = planOpen;

        bool walking = !preview && !planOpen && screen == UiScreen.None;
        hud.Root.style.display = walking ? DisplayStyle.Flex : DisplayStyle.None;
        bool inspecting = walking && player != null && player.InspectedTree != null;
        inspection.Root.style.display = inspecting ? DisplayStyle.Flex : DisplayStyle.None;
        if (inspecting)
        {
            // [V] shows or hides the detail card; the cursor is locked while inspecting, so it is a key, not a button.
            Keyboard keys = Keyboard.current;
            if (keys != null && keys.vKey.wasPressedThisFrame && !help.IsOpen) inspection.ToggleDetail();
            indicator.Update(player.InspectedTree);
            if (!help.IsOpen)
            {
                learning.ObserveRackInspection(player.InspectedTree);
                if (keys != null && keys.jKey.wasPressedThisFrame) learning.DecideRackInspection(player.InspectedTree, false);
                if (keys != null && keys.kKey.wasPressedThisFrame) learning.DecideRackInspection(player.InspectedTree, true);
            }
        }
        else
        {
            indicator.Hide();
            hud.SetPromptMaxWidth(900f);
        }
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
            {
                inspection.Refresh(player.InspectedTree);
                PlaceInspectionColumn();
            }
        }
        else
            competitors.Clear();
        hud.ShowStatus(!(inspecting && inspection.ShowsCompetitors));
        hud.ShowMinimap(!(inspecting && inspection.ShowsCompetitors));
        PlaceSidePanel(inspection.CompetitorPanel, inspecting && inspection.ShowsCompetitors);
        PlaceSidePanel(inspection.EstimatePanel, inspecting && inspection.ShowsEstimate);
        if (!preview)
        {
            MenuHelpView.Menu context = HelpContext();
            if (help.IsOpen && help.CurrentMenu != context) CloseHelp();
            // The Annual Review never opens a help screen over itself: its first-results guidance is inside the review,
            // beside the acknowledgement button. Help [F1] still opens the full explanation on request.
            // The rack screen is opened by the rack tool itself (first B press), never by context.
            if (screen != UiScreen.Objectives && context != MenuHelpView.Menu.AnnualReview && context != MenuHelpView.Menu.Rack)
                help.Introduce(context);
            manager.SetAuxiliaryPanelOpen(screen != UiScreen.None || help.IsOpen || helpClosedFrame == Time.frameCount);
        }

        if (!preview && rackLayout != null && ecology != null && ScenarioOneRackLayout.AppliesTo(ecology.StandGeometryModelVersion))
            rackLayout.Update();
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

    // The inspection card and its detail sit top-left. Their height is capped just above the highest panel of the bottom-left
    // stack (ground report, forecast, marking summary), so the card can never cover those panels whatever the screen size.
    private void PlaceInspectionColumn()
    {
        float limit = document.rootVisualElement.layout.height - 12f;
        VisualElement bottom = hud.Root.Q(className: "hud-bottom");
        if (bottom != null)
            foreach (VisualElement child in bottom.Children())
                if (child.resolvedStyle.display != DisplayStyle.None && child.layout.height > 0f)
                    limit = Mathf.Min(limit, child.worldBound.yMin - 10f);
        inspection.Root.style.maxHeight = Mathf.Max(160f, limit - 14f);
        float between = document.rootVisualElement.layout.width - 2f * (TreeInspectionView.ColumnWidth + 28f);
        hud.SetPromptMaxWidth(Mathf.Clamp(between, 280f, 900f));
    }

    // The competitor and estimate panels sit on the right, just under the HUD status panel and objective widget,
    // and never extend past the bottom of the screen.
    private void PlaceSidePanel(VisualElement panel, bool show)
    {
        panel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        if (!show) return;
        // Under the top-right column (the objective widget and the status panel), whatever height they have.
        VisualElement column = hud.RightColumn;
        float top = column != null && column.layout.height > 0f ? column.layout.yMax + 10f : 250f;
        // The minimap sits at the bottom right of this same column, so the panel stops above it. (The competitor list is the one
        // panel too tall for that: it steps the minimap aside instead, so ReservedHeightPx is 0 while it is open.)
        float available = document.rootVisualElement.layout.height - top - 14f - (Minimap != null ? Minimap.ReservedHeightPx : 0f);
        panel.style.top = top;
        if (available > 0f) panel.style.maxHeight = available;
    }

    private void LateUpdate()
    {
        if (competitors == null || manager == null) return;
        bool walking = !manager.ReferencePreviewActive && !manager.WorkPlanOpen && screen == UiScreen.None;
        competitors.LateUpdate(walking && player != null ? player.InspectedTree : null);
        indicator?.LateUpdate();
    }

    private void HandleKeys()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;
        if (help.Onboarding.IsOpen)
        {
            // Startup navigation only. Esc goes back; F1 cannot replace/skip onboarding.
            if (keyboard.escapeKey.wasPressedThisFrame || keyboard.leftArrowKey.wasPressedThisFrame) help.Onboarding.Back();
            else if (keyboard.rightArrowKey.wasPressedThisFrame) help.Onboarding.Next();
            return;
        }
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
        // The help and first-use window is modal: until it is closed (Continue, Esc or F1) no other screen opens behind it or
        // replaces it. ScenarioOneManager applies the same rule to Tab.
        if (help.IsOpen)
            return;
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
            review.ShowLatest();
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
        if (rackTool != null && (rackTool.Active || rackTool.IntroPending)) return MenuHelpView.Menu.Rack;
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

    // Test hygiene: the next participant starts with the first-use screens and lessons again. It clears the local help and
    // lesson preferences only; the forest, its save and the learning-gate on the first review are not touched.
    public void ResetTeachingProgress()
    {
        // Closing marks the open screen as seen, so close it first and clear afterwards.
        if (help != null && help.IsOpen) help.Close();
        MenuHelpView.ResetProgress();
        LearningObjectivesView.ResetProgress();
        if (learning != null) learning.ReloadProgress();
    }

    public void ShowScenarioIntroduction() => help.ShowScenarioIntroduction();

    public void AcknowledgeAnnualReview()
    {
        if (screen != UiScreen.Review || manager.AnnualReports.Count == 0) return;
        manager.MarkAnnualReviewSeen();
        review.Refresh(true);
        learning.Refresh(true);
    }

    // A UI learning gate only: simulation/reference harness APIs remain unchanged.
    public string RackPlanProblem => learning.RackPlanProblem();

    public bool ApproveWorkPlan()
    {
        if (!string.IsNullOrEmpty(RackPlanProblem)) return false;
        return manager.ApprovePendingWork();
    }

    public bool AdvanceFromWorkPlan()
    {
        if (!string.IsNullOrEmpty(RackPlanProblem)) return false;
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
