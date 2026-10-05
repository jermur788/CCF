using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

// Scenario One player interface (UI Toolkit): walking HUD, tree inspection,
// Work Plan, annual review and stand map. Added by ScenarioOneManager at play
// time. Presentation only: every value is read from the authoritative systems
// and every action calls an existing ScenarioOneManager / marking API.
//
// Keys: Tab Work Plan (owned by ScenarioOneManager), N stand map, Esc closes
// the open map or review. M remains Fell marking.
public sealed class ScenarioOneUiRoot : MonoBehaviour
{
    public enum UiScreen { None, Map, Review }

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

        hud = new WalkingHudView(this);
        inspection = new TreeInspectionView(this);
        workPlan = new WorkPlanView(this);
        review = new AnnualReviewView(this);
        map = new StandMapView(this);
        root.Add(hud.Root);
        root.Add(inspection.Root);
        root.Add(workPlan.Root);
        root.Add(review.Root);
        root.Add(map.Root);

        previewBanner = UiKit.Text("", "panel", "body");
        previewBanner.style.position = Position.Absolute;
        previewBanner.style.top = 14;
        previewBanner.style.alignSelf = Align.Center;
        root.Add(previewBanner);
        help = new MenuHelpView(this);
        root.Add(help.Root);
    }

    private void OnDestroy()
    {
        if (map != null) map.DestroyWaypoint();
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
        inspection.Root.style.display = walking && player != null && player.InspectedTree != null ? DisplayStyle.Flex : DisplayStyle.None;
        workPlan.Root.style.display = planOpen ? DisplayStyle.Flex : DisplayStyle.None;
        review.Root.style.display = !preview && screen == UiScreen.Review ? DisplayStyle.Flex : DisplayStyle.None;
        map.Root.style.display = !preview && screen == UiScreen.Map ? DisplayStyle.Flex : DisplayStyle.None;
        previewBanner.style.display = preview ? DisplayStyle.Flex : DisplayStyle.None;
        if (preview)
            previewBanner.text = $"REFERENCE FUTURE v1 — YEAR {manager.ReferencePreviewYear}  ·  [Tab] Return to your forest";

        if (walking)
        {
            hud.Refresh();
            if (player != null && player.InspectedTree != null)
                inspection.Refresh(player.InspectedTree);
        }
        if (!preview)
        {
            MenuHelpView.Menu context = HelpContext();
            if (help.IsOpen && help.CurrentMenu != context) CloseHelp();
            if (context != MenuHelpView.Menu.AnnualReview || manager.AnnualReports.Count > 0)
                help.Introduce(context);
            manager.SetAuxiliaryPanelOpen(screen != UiScreen.None || help.IsOpen || helpClosedFrame == Time.frameCount);
        }

        if (Time.unscaledTime >= nextModalRefresh)
        {
            nextModalRefresh = Time.unscaledTime + 0.25f;
            if (planOpen) workPlan.Refresh(false);
            if (screen == UiScreen.Review) review.Refresh(false);
            if (screen == UiScreen.Map) map.Refresh(false);
        }
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
        if (keyboard.nKey.wasPressedThisFrame)
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

    public void SetScreen(UiScreen next)
    {
        if (screen == next)
            return;
        screen = next;
        CloseHelp();
        manager.SetAuxiliaryPanelOpen(screen != UiScreen.None);
        if (screen == UiScreen.Review)
        {
            review.Refresh(true);
        }
        else if (screen == UiScreen.Map)
            map.Refresh(true);
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

    public void CloseAll()
    {
        CloseHelp();
        manager.OpenWorkPlan(false);
        SetScreen(UiScreen.None);
    }
}
