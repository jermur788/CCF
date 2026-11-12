using System.Collections.Generic;
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
    private VisualElement animatedGuidance;
    public bool ReduceMotion { get; private set; } = true;
    public void SetReduceMotion(bool reduce) {
        ReduceMotion = reduce; PlayerPrefs.SetInt("CCF.UI.ReduceMotion", reduce ? 1 : 0); PlayerPrefs.Save();
    }
    private Button nextAction;
    private Button guidanceCandidate;
    private VisualElement guidanceScope;
    private Label nextBadge;
    private readonly Dictionary<Button, bool> guidancePrimaries = new Dictionary<Button, bool>();
    // Only T2-owned controls. Treatment review, waypoints and Legacy40 keep their original styling.
    private static readonly string[] GuidanceButtonNames = {
        "objective-primary-cta", "objective-work-plan", "objective-back-to-forest",
        "map-work-plan", "map-back-to-forest", "map-plan-rack", "map-rack-plan-confirm", "map-rack-plan-reopen",
        "map-consent-decline", "map-consent-accept", "map-edit-reset-cancel", "map-edit-reset-confirm",
        "map-reopen-decline", "map-reopen-accept", "workplan-approve", "workplan-advance",
        "workplan-read-results", "workplan-advance-return", "annual-review-acknowledge"
    };

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

    private bool skipping;
    private bool cancelSkip;
    private int skipTarget, skipStart, skipObjectives, skipStorms, skipLesson;
    private ScenarioOneOutcome skipOutcome;
    private int skipFrame;
    private readonly Dictionary<Behaviour, bool> skipInputs = new Dictionary<Behaviour, bool>();
    private readonly Dictionary<VisualElement, bool> skipControls = new Dictionary<VisualElement, bool>();
    private bool priorModal;
    private VisualElement skipOverlay;
    private Label skipProgress;
    public bool Skipping => skipping;

    // Intermediate unread reviews are expected; only entry requires a genuine acknowledgment.
    public string SkipProblem(bool betweenTicks = false)
    {
        if (!betweenTicks && skipping) return "A skip is already running.";
        if (manager == null || ecology == null || !manager.isActiveAndEnabled) return "Forest controller unavailable.";
        if (!manager.WorkPlanOpen) return "Open the Work Plan first.";
        if (learning.CurrentTopicIndex != 7) return "Finish the current lesson first. Advance 1 Year is still available where permitted.";
        foreach (var order in manager.WorkOrders)
            if (order.status == ScenarioWorkStatus.Pending || order.status == ScenarioWorkStatus.Approved)
                return "Resolve or remove all Pending and Approved work first.";
        if (!betweenTicks && NeedsAnnualReview) return "Read and acknowledge the latest Annual Review first.";
        if (manager.ReferencePreviewActive || manager.ReferenceAuthoringActive) return "Return to your forest first.";
        if ((!betweenTicks && manager.ModalOpen) || StandaloneSessionMenu.IsOpen || Time.timeScale <= 0f
            || help.IsOpen || workPlan.ConfirmingAdvance || rackTool.Active || rackTool.IntroPending
            || map.RackEditor.ConsentPending || map.RackEditor.ResetPending || map.ReopenPending
            || map.RackEditor.Dragging || map.RackEditor.Ghost.HasValue) return "Finish the open question or editing action first.";
        if (!string.IsNullOrEmpty(RackPlanProblem)) return RackPlanProblem;
        if (!string.IsNullOrEmpty(RackApprovalProblem)) return RackApprovalProblem;
        if (!manager.CanAdvanceYearNow || !manager.CanEditWorkPlan) return "Year advance is unavailable. " + manager.Feedback;
        return "";
    }

    public int SkipTarget(out int? lastFelling) => ScenarioManagementCadence.Compute(manager.CurrentEcologicalYear,
        manager.CenturyReviewYear, manager.WorkOrders, manager.ManagementEvents, out lastFelling);

    public bool BeginSkip(int confirmedTarget)
    {
        learning.Refresh(true);
        if (!string.IsNullOrEmpty(SkipProblem()) || confirmedTarget != SkipTarget(out _) || confirmedTarget <= manager.CurrentEcologicalYear) return false;
        skipStart = manager.CurrentEcologicalYear; skipTarget = confirmedTarget;
        skipObjectives = manager.ObjectiveAchievements.Count; skipStorms = manager.StormEvents.Count;
        skipLesson = learning.CurrentTopicIndex; skipOutcome = manager.Outcome;
        priorModal = manager.ModalOpen;
        skipping = true; cancelSkip = false; skipFrame = Time.frameCount;
        try
        {
            // Disable both hotkey owners and the session-menu surface. Preserve disabled components too.
            foreach (Behaviour input in FindObjectsByType<Behaviour>(FindObjectsSortMode.None))
                if (input is ForestSaveController || input is StandaloneSessionMenu || input is ForestPlayer
                    || input is ForestTreeMarkingManager || input is RackTool || input is ForestBuildable
                    || input is ForestSawPit || input is ForestWoodStorage || input is RecentAssetReviewControls)
                { skipInputs[input] = input.enabled; input.enabled = false; }
            manager.SetModalOpen(true);
            foreach (VisualElement child in RootElement.Children()) { skipControls[child] = child.enabledSelf; child.SetEnabled(false); }
            skipOverlay = UiKit.Box("layer", "modal-backdrop");
            var card = UiKit.Box("panel", "modal"); skipOverlay.Add(card);
            skipProgress = UiKit.Add(card, "Preparing year advance…", "body");
            card.Add(UiKit.Button("Cancel after the current year", () => cancelSkip = true));
            RootElement.Add(skipOverlay);
        }
        catch { RestoreSkipInputs(); throw; }
        return true;
    }

    private void RestoreSkipInputs()
    {
        foreach (var input in skipInputs) if (input.Key != null) input.Key.enabled = input.Value;
        skipInputs.Clear();
        if (manager != null) manager.SetModalOpen(priorModal);
        skipOverlay?.RemoveFromHierarchy(); skipOverlay = null;
        foreach (var control in skipControls) control.Key.SetEnabled(control.Value);
        skipControls.Clear();
        skipping = false;
    }

    private void FinishSkip(string reason)
    {
        int end = manager.CurrentEcologicalYear;
        RestoreSkipInputs();
        review.SetSkipSummary(end == skipStart ? "No years advanced. " + reason
            : $"Session only: advanced Years {skipStart + 1}–{end}. {reason} Browse earlier years below; only the latest review can be acknowledged.");
        ShowReview(); review.Refresh(true);
    }

    private string SkipStopReason()
    {
        if (manager == null || ecology == null) return "Forest controller unavailable.";
        return manager.Outcome != skipOutcome ? "Scenario outcome changed."
                : manager.ObjectiveAchievements.Count > skipObjectives ? "An objective was achieved."
                : (manager.CurrentEcologicalYear > skipStart && ecology.LastStormPerformance.Victims > 0) || manager.StormEvents.Count > skipStorms ? "Storm recorded."
                : learning.CurrentTopicIndex != skipLesson ? "The lesson changed."
                : (skipStart < 25 && manager.CurrentEcologicalYear >= 25) || manager.CurrentEcologicalYear >= manager.CenturyReviewYear ? "Review boundary reached."
                : manager.CurrentEcologicalYear >= skipTarget ? "Suggested reassessment year reached." : SkipProblem(true);
    }

    private void TickSkip()
    {
        if (Time.frameCount == skipFrame) return;
        skipFrame = Time.frameCount;
        try
        {
            string problem = SkipStopReason();
            if (cancelSkip || !string.IsNullOrEmpty(problem)) { FinishSkip(cancelSkip ? "Cancelled between years." : problem); return; }
            skipProgress.text = $"Advancing year {manager.CurrentEcologicalYear - skipStart + 1} of {skipTarget - skipStart}";
            if (!manager.AdvanceYear()) { FinishSkip("Year advance failed: " + manager.Feedback); return; }
            learning.Refresh(true);
            string stop = SkipStopReason();
            if (!string.IsNullOrEmpty(stop)) FinishSkip(stop);
        }
        catch (System.Exception exception) { FinishSkip("Stopped after an error: " + exception.Message); Debug.LogException(exception); }
    }

    private void OnDisable() { if (skipping) RestoreSkipInputs(); }

    private void Start()
    {
        ReduceMotion = PlayerPrefs.GetInt("CCF.UI.ReduceMotion", 1) != 0;
        nextBadge = new Label("Next:") { pickingMode = PickingMode.Ignore };
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
        competitors.OccludingPanels.Add(inspection.Root);
        competitors.OccludingPanels.Add(inspection.CompetitorPanel);
        // The objective widget and status panel sit in this column: a numbered tag drawn over their text is unreadable, so it waits behind them too.
        competitors.OccludingPanels.Add(hud.RightColumn);
        competitors.OccludingPanels.Add(hud.ObjectiveWidget.Root);
        competitors.OccludingPanels.Add(hud.ContextActions);
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
        if (skipping) { TickSkip(); return; }
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
            if (keys != null && keys.vKey.wasPressedThisFrame && !help.IsOpen)
                inspection.StepDetail(keys.leftShiftKey.isPressed || keys.rightShiftKey.isPressed ? -1 : 1);
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
            if (player != null && player.InspectedTree != null)
            {
                inspection.Refresh(player.InspectedTree);
                PlaceInspectionColumn();
            }
            hud.Refresh();
        }
        else
            competitors.Clear();
        hud.ShowStatus(true);
        hud.ShowMinimap(!inspecting);
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
            foreach (Label status in document.rootVisualElement.Query<Label>("menu-current-status").ToList())
                status.text = $"Current Year {manager.CurrentEcologicalYear} · Cash {UiKit.Money(manager.CashCents)}";
            if (!preview) learning.Observe();
            if (planOpen) workPlan.Refresh(false);
            if (screen == UiScreen.Review) review.Refresh(false);
            if (screen == UiScreen.Map) map.Refresh(false);
            if (screen == UiScreen.Objectives) learning.Refresh(false);
        }
    }

    // One right-hand card below the persistent Year/Cash strip. The bottom bar has its own reserved space.
    private void PlaceInspectionColumn()
    {
        float top = hud.RightColumn.layout.yMax + 10f;
        inspection.Root.style.top = top;
        inspection.Root.style.maxHeight = Mathf.Max(160f, document.rootVisualElement.layout.height - top - 108f);
        hud.SetPromptMaxWidth(700f);
    }

    private void LateUpdate()
    {
        // No forest/quote/validator lookups here. Apply cached presentation after layout each frame,
        // including the first frame after opening or dismissing Help.
        ApplyNextActionGuidance();
        AnimateGuidance();
        if (competitors == null || manager == null) return;
        bool walking = !manager.ReferencePreviewActive && !manager.WorkPlanOpen && screen == UiScreen.None;
        competitors.LateUpdate(walking && player != null ? player.InspectedTree : null);
        indicator?.LateUpdate();
    }

    // Derive only after a genuine card rebuild/view entry, never on an unchanged timer tick.
    public void RefreshNextActionGuidance()
    {
        if (learning == null || map == null || workPlan == null || manager == null) return;
        ClearNextAction();
        guidanceCandidate = null;
        guidanceScope = null;
        foreach (var entry in guidancePrimaries)
            entry.Key.EnableInClassList("btn-primary", entry.Value);
        guidancePrimaries.Clear();
        foreach (VisualElement view in new[] { learning.Root, map.Root, workPlan.Root, review.Root })
        {
            Label note = view.Q<Label>("next-action-reason");
            if (note != null) { note.text = ""; note.style.display = DisplayStyle.None; }
        }
        VisualElement scope = manager.WorkPlanOpen ? workPlan.Root
            : screen == UiScreen.Objectives ? learning.Root : screen == UiScreen.Map ? map.Root
            : screen == UiScreen.Review ? review.Root : null;
        if (scope == null) return;
        string target = null;
        string reason = learning.CurrentInstruction;
        bool mapQuestions = scope == map.Root && map.RackMode;
        bool consent = mapQuestions && map.RackEditor.ConsentPending;
        bool reset = mapQuestions && map.RackEditor.ResetPending;
        if (consent && reset)
            reason = "Two rack questions are open; keep or dismiss them before continuing.";
        else if (consent) target = "map-consent-decline";
        else if (reset) target = "map-edit-reset-cancel";
        else if (mapQuestions && map.ReopenPending) target = "map-reopen-decline";
        else if (scope == workPlan.Root && workPlan.ConfirmingAdvance) target = "workplan-advance-return";
        else if (manager.RackPlanningV2Active && (learning.TeachingRacks
            // The teaching topic advances when rack work resolves, before the first annual acknowledgement.
            || (NeedsAnnualReview && manager.AnnualReports.Count == 1 && rackLayout != null && rackLayout.Harvested)))
        {
            bool objectives = scope == learning.Root;
            bool onMap = scope == map.Root;
            if (NeedsAnnualReview)
                target = scope == review.Root ? "annual-review-acknowledge"
                    : objectives ? "objective-work-plan" : onMap ? "map-work-plan" : "workplan-read-results";
            else if (!manager.RackPlanConfirmed)
            {
                if (objectives) target = "objective-primary-cta";
                else if (onMap)
                {
                    if (!map.RackPlanningAvailable)
                        reason = !manager.CanEditWorkPlan ? manager.OutcomeReason + " Work Plan editing is unavailable."
                            : "Rack planning requires the rack tool and forest data to be available.";
                    else if (!map.RackMode) target = "map-plan-rack";
                    else if (map.RackEditor.Dragging || map.RackEditor.Ghost.HasValue)
                        reason = string.IsNullOrEmpty(map.RackEditor.Status) ? learning.CurrentInstruction : map.RackEditor.Status;
                    else if (manager.CanConfirmRackPlan(out string problem)) target = "map-rack-plan-confirm";
                    else reason = problem;
                }
            }
            else if (manager.MarkedRackEntranceCount < ScenarioOneManager.RackPlanTargetCount)
            {
                bool markable = false;
                string unavailableReason = "";
                foreach (RackPlanRack rack in manager.RackPlan)
                {
                    if (rack.EntranceMarked || rack.IsLocked) continue;
                    if (manager.TryGetRackEntranceTarget(rack.Id, out ForestTree tree, out string problem)
                        && !tree.IsCropTree && !tree.IsMarkedForFell) { markable = true; break; }
                    else if (!string.IsNullOrEmpty(problem)) unavailableReason = problem;
                }
                if (markable) target = objectives ? "objective-back-to-forest" : onMap ? "map-back-to-forest" : null;
                else reason = "No unmarked rack entrance is currently available to mark. " + unavailableReason;
            }
            else if (rackLayout != null)
            {
                string problem = RackPlanProblem;
                if (string.IsNullOrEmpty(problem) && rackLayout.WorkPending) problem = RackApprovalProblem;
                if (!string.IsNullOrEmpty(problem)) reason = problem;
                else if (rackLayout.WorkPending)
                {
                    target = objectives ? "objective-work-plan" : onMap ? "map-work-plan" : "workplan-approve";
                    reason = "Review every pending job and its cost: Approve Work Plan commits all eligible pending work, including jobs outside the racks.";
                    if (scope == workPlan.Root)
                    {
                        ScenarioHarvestJob quote = manager.GetHarvestQuote(false);
                        long cost = manager.GetWorkPlanSummary().ExternalCostCents;
                        if (!quote.Eligible || cost > manager.CashCents)
                        {
                            target = null;
                            reason = !quote.Eligible ? quote.Problem : $"Approval needs {UiKit.Money(cost)} including approved work; available cash is {UiKit.Money(manager.CashCents)}.";
                        }
                        foreach (ScenarioOneWorkOrder order in manager.WorkOrders)
                            if (order.rackId > 0 && order.status == ScenarioWorkStatus.Pending && !string.IsNullOrEmpty(order.validationMessage))
                            { target = null; reason = order.validationMessage; break; }
                    }
                }
                else
                {
                    // Completed or failed work alone is not evidence that there is work ready for annual execution.
                    bool approved = false, failed = false;
                    foreach (RackPlanRack rack in manager.RackPlan)
                    {
                        approved |= rack.Orders.Approved > 0;
                        failed |= rack.Orders.Failed > 0;
                    }
                    if (rackLayout.Complete && approved && !failed && manager.CanAdvanceYearNow && ecology != null)
                        target = objectives ? "objective-work-plan" : onMap ? "map-work-plan" : "workplan-advance";
                    else reason = "Review the rack work and its results in the Work Plan. " + manager.Feedback;
                }
            }
        }
        else if (manager.RackPlanningV2Active)
        {
            // The long instruction lives on the Objectives card (under More detail). Other windows get the one short line.
            reason = scope == learning.Root ? "" : learning.CurrentShortInstruction;
            if (learning.ReviewFirst)
                target = scope == review.Root ? "annual-review-acknowledge" : scope == learning.Root ? "objective-work-plan" : scope == map.Root ? "map-work-plan" : "workplan-read-results";
            else if (learning.ExtensionActive)
                target = scope == learning.Root ? "objective-primary-cta" : scope == map.Root ? (map.TreatmentReviewOpen ? "map-back-to-forest" : "map-treatment-review") : null;
            else if (scope == learning.Root) target = "objective-back-to-forest";
            else if (scope == map.Root) target = learning.TeachingBay != null && !map.HasWaypoint ? "map-teaching-waypoint" : "map-back-to-forest";
            // Work Plan approvals and year advances keep their deliberate static gates.
        }
        else return;

        // Restore original classes above before deciding whether this context belongs to T2.
        // Later objectives and Legacy40 return before this allowlist is demoted.
        foreach (string name in GuidanceButtonNames)
        {
            Button owned = scope.Q<Button>(name);
            if (owned == null) continue;
            guidancePrimaries[owned] = owned.ClassListContains("btn-primary");
            owned.RemoveFromClassList("btn-primary");
        }
        guidanceScope = scope;
        guidanceCandidate = target != null ? scope.Q<Button>(target) : null;
        Label explanation = scope.Q<Label>("next-action-reason");
        if (explanation != null)
        {
            explanation.text = reason;
            explanation.style.display = string.IsNullOrEmpty(reason) ? DisplayStyle.None : DisplayStyle.Flex;
        }
        ApplyNextActionGuidance();
    }

    private void ClearNextAction()
    {
        if (nextAction != null)
        {
            nextAction.RemoveFromClassList("btn-primary");
            nextAction.RemoveFromClassList("next-action");
            nextAction = null;
        }
        nextBadge.RemoveFromHierarchy();
    }

    // Cheap visibility/suspension checks only; expensive facts are cached by the rebuild above.
    private void ApplyNextActionGuidance()
    {
        bool suspended = Time.timeScale <= 0f || manager == null || manager.ReferencePreviewActive || StandaloneSessionMenu.IsOpen
            || help == null || help.IsOpen || helpClosedFrame == Time.frameCount;
        Button candidate = guidanceCandidate;
        VisualElement scope = guidanceScope;
        bool eligible = !suspended && candidate != null && candidate.enabledInHierarchy && GuidanceVisible(candidate, scope);
        if (nextAction == candidate && eligible) return;
        ClearNextAction();
        if (!eligible) return;
        nextAction = candidate;
        nextBadge.AddToClassList("next-action-badge");
        candidate.parent.Insert(candidate.parent.IndexOf(candidate), nextBadge);
        candidate.AddToClassList("btn-primary");
        candidate.AddToClassList("next-action");
    }

    private void AnimateGuidance()
    {
        VisualElement target = null;
        bool suspended = Time.timeScale <= 0f || manager == null || manager.ReferencePreviewActive || StandaloneSessionMenu.IsOpen || help == null || help.IsOpen
            || helpClosedFrame == Time.frameCount || map.RackEditor.ConsentPending || map.RackEditor.ResetPending || map.ReopenPending || workPlan.ConfirmingAdvance;
        if (!suspended && nextAction != null && ScenarioOneTeachingSequence.SafePulseTarget(nextAction.name)) target = nextAction;
        else if (!suspended && manager.RackPlanningV2Active && screen == UiScreen.None && !manager.WorkPlanOpen && learning.CurrentTopicIndex > 0
            && learning.CurrentTopicIndex < 8 && !NeedsAnnualReview && !learning.NeedsReviewAcknowledgement)
            target = hud.ObjectiveWidget.Root;
        if (animatedGuidance != null && animatedGuidance != target) ResetGuidanceBorder(animatedGuidance);
        animatedGuidance = target;
        if (target == null) return;
        // Constant visibility, gentle 1.2s border tint; no focus movement, opacity flashing or enabled-state changes.
        float wave = ReduceMotion ? 0f : .5f + .5f * Mathf.Sin(Time.unscaledTime * (2f * Mathf.PI / 1.2f));
        Color c = Color.Lerp(new Color(.72f,.69f,.43f), new Color(1f,.94f,.63f), wave);
        target.style.borderTopColor = target.style.borderBottomColor = target.style.borderLeftColor = target.style.borderRightColor = c;
        target.style.borderTopWidth = target.style.borderBottomWidth = target.style.borderLeftWidth = target.style.borderRightWidth = 2f;
    }
    private static void ResetGuidanceBorder(VisualElement e) {
        e.style.borderTopColor = e.style.borderBottomColor = e.style.borderLeftColor = e.style.borderRightColor = StyleKeyword.Null;
        e.style.borderTopWidth = e.style.borderBottomWidth = e.style.borderLeftWidth = e.style.borderRightWidth = StyleKeyword.Null;
    }

    private static bool GuidanceVisible(VisualElement element, VisualElement scope)
    {
        for (VisualElement current = element; current != null; current = current.parent)
        {
            if (current.style.display == DisplayStyle.None || current.resolvedStyle.display == DisplayStyle.None
                || current.resolvedStyle.visibility == Visibility.Hidden) return false;
            if (current == scope) return true;
        }
        return false;
    }

    // Inline questions do not get default/restored focus. This does not opt any button into navigation.
    public static void ClearPromptFocus(VisualElement scope)
    {
        VisualElement focused = scope.panel?.focusController?.focusedElement as VisualElement;
        if (focused != null && scope.Contains(focused)) focused.Blur();
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
        // Delete / Backspace remove the selected rack while the map is planning racks (RACK-V2-05).
        if (screen == UiScreen.Map)
            map.HandleKeys(keyboard);
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
        if (screen == UiScreen.Review) RefreshNextActionGuidance();
    }

    private MenuHelpView.Menu HelpContext()
    {
        if (rackTool != null && (rackTool.Active || rackTool.IntroPending)) return MenuHelpView.Menu.Rack;
        if (manager.WorkPlanOpen) return MenuHelpView.Menu.WorkPlan;
        if (screen == UiScreen.Map) return MenuHelpView.Menu.StandMap;
        if (screen == UiScreen.Review) return MenuHelpView.Menu.AnnualReview;
        if (player != null && player.InspectedTree != null) return MenuHelpView.Menu.TreeInspection;
        // Tree Marks unlocks once field marking is relevant (a confirmed V2 rack plan); the first walking frame after that shows it.
        bool treeMarksUnlocked = MenuHelpView.TreeMarksUnlocked(manager.RackPlanningV2Active, manager.RackPlanConfirmed);
        if (MenuHelpView.TreeMarksContext(treeMarksUnlocked, MenuHelpView.Introduced(MenuHelpView.Menu.TreeMarks), marking != null && marking.RackSprayEquipped))
            return MenuHelpView.Menu.TreeMarks;
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
    // S1-D T-03: the same footprint rule once the teaching sample is complete. A note only; ApproveWorkPlan/AdvanceFromWorkPlan ignore it.
    public string RackPlanAdvisory => learning.RackPlanAdvisory();
    // D-064: V2 rack work is approved only after its entrances are marked. Approval only; advancing a year is not gated by it.
    public string RackApprovalProblem => learning.RackMarkingProblem();

    public bool ApproveWorkPlan()
    {
        if (!string.IsNullOrEmpty(RackPlanProblem) || !string.IsNullOrEmpty(RackApprovalProblem)) return false;
        return manager.ApprovePendingWork();
    }

    public bool AdvanceFromWorkPlan()
    {
        review.SetSkipSummary("");
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

    // TryShowYear validates before any panel changes. Open the same review surface without the
    // normal SetScreen(Review) route resetting its deliberate selection to ShowLatest().
    public bool TryShowReviewYear(int year)
    {
        if (!review.TryShowYear(year)) return false;
        manager.OpenWorkPlan(false);
        screen = UiScreen.Review;
        CloseHelp();
        manager.SetAuxiliaryPanelOpen(true);
        return true;
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
