using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Optional learning progress belongs to the local player, not the forest model.
// Actions are observed; explanatory reading is explicitly acknowledged.
public sealed partial class LearningObjectivesView
{
    private readonly ScenarioOneUiRoot ui;
    private readonly HashSet<string> done = new HashSet<string>();
    // Current-session teaching actions are transient. Device-wide practice credits remain in the existing preference keys.
    // Only actions observed while their lesson is current can move the guide on; old credits cannot skip objective 2.
    private readonly HashSet<string> guidedActions = new HashSet<string>();
    private readonly ScrollView scroll;
    private readonly Label progress;
    private readonly Dictionary<string, bool> expanded = new Dictionary<string, bool>();
    private int shownCount = -1;
    private int shownRegenerationModel = -1;
    private int shownMarket = -1;
    private string shownObjectives = "";
    public VisualElement Root { get; }
    public bool IsDone(string id) => done.Contains(id);
    public int CompletedSteps => done.Count;
    private string shownGuidance = "";
    public int CurrentTopicIndex => GuidedTopicIndex(
        ui.Ecology != null && ScenarioOneRackLayout.AppliesTo(ui.Ecology.StandGeometryModelVersion),
        ui.RackLayout != null && ui.RackLayout.Harvested, ui.Manager.CurrentEcologicalYear, GuidedIsDone);
    private bool GuidedIsDone(string id) => guidedActions.Contains(id)
        || (IsDone(id) && Topics.SelectMany(topic => topic.Steps).Any(step => step.Id == id && step.Reading));
    public bool TeachingRacks => CurrentTopicIndex == 0;
    public string CurrentTitle => Topics[CurrentTopicIndex].Title;
    public string CurrentInstruction
    {
        get
        {
            if (!TeachingRacks)
            {
                Topic topic = Topics[CurrentTopicIndex];
                Step next = topic.Steps.FirstOrDefault(step => !step.Reading && !GuidedIsDone(step.Id));
                return next != null ? next.Text : topic.Text;
            }
            RackLayoutTracker layout = ui.RackLayout;
            if (layout != null && layout.Complete)
            {
                bool pending = ui.Manager.WorkOrders.Any(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending);
                return pending ? "The rack layout is planned. Open Work Plan [Tab], review the cost and approve the rack work. Then advance one year."
                    : "Your rack work is ready. Open Work Plan [Tab] and choose Advance one year. Objective 2 follows that year advance.";
            }
            return layout != null && layout.Done > 0
                ? $"{layout.Done} of {layout.Total} access racks planned. Complete the layout with B, or use Plan rack on the Stand Map [M]."
                : "Press B, then click the tree or ground point where your rack should start. Aim along the route and click again to add it to the Work Plan.";
        }
    }
    public string Summary => TeachingRacks
        ? (ui.RackLayout != null && ui.RackLayout.Complete ? "Current objective: approve rack work, then advance one year · [O] Details"
            : "Current objective: create access racks · [B] Choose a start · [O] Details")
        : "Current objective: " + CurrentTitle + " · [O] Details";

    public LearningObjectivesView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        ReloadProgress();
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal");
        modal.style.backgroundColor = new Color(14f / 255f, 22f / 255f, 16f / 255f, 1f);
        Root.Add(modal);
        VisualElement header = UiKit.Box("modal-header");
        UiKit.Add(header, "Scenario One · one objective at a time", "title");
        VisualElement buttons = UiKit.Row(header);
        buttons.Add(UiKit.Button("Stand map [M]", () => ui.ShowMap()));
        buttons.Add(UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan()));
        buttons.Add(UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary"));
        modal.Add(header);
        progress = UiKit.Add(modal, "", "body");

        scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        scroll.style.flexGrow = 1;
        modal.Add(scroll);
    }

    // Progress is saved per device, not per forest. These two let a facilitator start the next participant fresh without
    // touching the forest or its save: ResetProgress forgets the saved steps, ReloadProgress makes this view follow.
    public static void ResetProgress()
    {
        foreach (string id in StepIds) PlayerPrefs.DeleteKey(PreferencePrefix + id);
        PlayerPrefs.Save();
    }

    public void ReloadProgress()
    {
        done.Clear();
        guidedActions.Clear();
        foreach (string id in StepIds) if (PlayerPrefs.GetInt(PreferencePrefix + id, 0) == 1) done.Add(id);
        shownCount = -1;
    }

    public void Record(string id)
    {
        if (!StepIds.Contains(id)) return;
        if (Topics[CurrentTopicIndex].Steps.Any(step => step.Id == id)) guidedActions.Add(id);
        if (done.Contains(id)) return;
        done.Add(id);
        PlayerPrefs.SetInt(PreferencePrefix + id, 1);
        PlayerPrefs.Save();
    }

    public static string ClearanceExplanation(int regenerationModel)
    {
        if (regenerationModel == RegenerationModel.Competition)
            return "Dense bramble or bracken can reduce the survival of small young trees. Clearing can reduce that competition, but it also removes young trees already growing inside the treatment area, and the vegetation can return.";
        return "Clearing removes the vegetation and young trees shown in the preview. It does not change young-tree survival or growth in this saved forest. Vegetation can return.";
    }

    public void Refresh(bool force)
    {
        int regenerationModel = ui.Ecology != null ? ui.Ecology.RegenerationModelVersion : RegenerationModel.Legacy;
        bool market = ui.Manager.BroadleafMarketActive;
        string objectiveText = string.Join("\n", ui.Manager.Objectives.Select(ScenarioOneUiFacts.ObjectiveLine));
        string guidance = CurrentTitle + CurrentInstruction;
        if (!force && shownGuidance == guidance && shownCount == done.Count && shownRegenerationModel == regenerationModel && shownMarket == (market ? 1 : 0) && shownObjectives == objectiveText) return;
        shownGuidance = guidance;
        shownObjectives = objectiveText;
        shownCount = done.Count;
        shownRegenerationModel = regenerationModel;
        shownMarket = market ? 1 : 0;
        float position = scroll.scrollOffset.y;
        progress.text = "Learn the management process one step at a time. [O] returns to this objective.";
        scroll.Clear();
        VisualElement current = UiKit.Box("card");
        current.name = "current-objective";
        current.style.borderLeftWidth = 4;
        current.style.borderLeftColor = new Color(0.91f, 0.81f, 0.47f);
        UiKit.Add(current, "CURRENT OBJECTIVE", "subtitle");
        UiKit.Add(current, CurrentTitle, "title").name = "current-objective-title";
        UiKit.Add(current, CurrentInstruction, "body").name = "current-objective-instruction";
        Topic active = Topics[CurrentTopicIndex];
        if (TeachingRacks)
        {
            UiKit.Add(current, "Racks are planned access routes through the plantation. They let forestry workers reach trees and bring timber out. Their layout shapes how the woodland can be managed later.", "body");
            current.Add(new RackLessonDiagram());
            UiKit.Add(current, "The clear strip is the rack. Standing trees remain on either side. Start near the woodland edge and aim along a row to open it forward from your chosen point.", "muted");
        }
        else UiKit.Add(current, active.Text, "body");
        var details = new Foldout { text = "Learn why / lesson details", value = expanded.TryGetValue(active.Title, out bool detailsOpen) && detailsOpen };
        details.RegisterValueChangedCallback(e => expanded[active.Title] = e.newValue);
        BuildTopic(details, active, regenerationModel, market, true);
        current.Add(details);
        scroll.Add(current);

        var success = new Foldout { text = "All scenario goals", value = expanded.TryGetValue("goals", out bool goalsOpen) && goalsOpen, name = "scenario-success-objectives" };
        success.RegisterValueChangedCallback(e => expanded["goals"] = e.newValue);
        UiKit.Add(success, "These forest success targets are evaluated throughout the scenario. The current lesson above guides what to learn next.", "muted");
        foreach (ScenarioObjectiveResult objective in ui.Manager.Objectives)
            UiKit.Add(success, ScenarioOneUiFacts.ObjectiveLine(objective), "body");
        scroll.Add(success);
        var library = new Foldout { text = "All learning topics / optional practice", value = false };
        UiKit.Add(library, "Practice credits are remembered on this device across forests. They do not determine scenario success. Reset tutorial/help progress in F10 starts that optional record again.", "muted");
        foreach (Topic topic in Topics)
        {
            var foldout = new Foldout { text = topic.Title, value = false };
            BuildTopic(foldout, topic, regenerationModel, market);
            library.Add(foldout);
        }
        scroll.Add(library);
        scroll.schedule.Execute(() => scroll.scrollOffset = new Vector2(0, position));
    }

    private void BuildTopic(VisualElement parent, Topic topic, int regenerationModel, bool market, bool current = false)
    {
        UiKit.Add(parent, topic.Steps[0].Id == "clear.read" ? ClearanceExplanation(regenerationModel) : topic.Text, "body");
        foreach (Step step in topic.Steps)
        {
            bool complete = current ? GuidedIsDone(step.Id) : IsDone(step.Id);
            VisualElement card = UiKit.Box("card");
            UiKit.Add(card, (complete ? "Done · " : "To do · ") + step.Title, "heading");
            UiKit.Add(card, StepText(step, market), "body");
            if (step.Reading && !complete)
            {
                bool ready = step.Requires == null || (current ? GuidedIsDone(step.Requires) : IsDone(step.Requires));
                Button read = UiKit.Button("I've read and understood this", () => { Record(step.Id); Refresh(true); }, ready);
                read.name = "learn-" + step.Id;
                card.Add(read);
                if (!ready) UiKit.Add(card, "First complete: " + Topics.SelectMany(t => t.Steps).First(s => s.Id == step.Requires).Title, "muted");
            }
            parent.Add(card);
        }
    }

    // Original, unscaled plan view informed by the supplied book photographs (pp.39–41) and stand diagram (p.66).
    // No source artwork, dimensions, machinery prescription or new asset dependency is reproduced.
    private sealed class RackLessonDiagram : VisualElement
    {
        public RackLessonDiagram()
        {
            name = "rack-lesson-diagram";
            style.height = 165; style.minHeight = 165; style.flexShrink = 0;
            style.maxWidth = 620; style.marginTop = 8; style.marginBottom = 8;
            pickingMode = PickingMode.Ignore;
            generateVisualContent += context =>
            {
                Painter2D p = context.painter2D;
                float width = contentRect.width, centre = width * 0.5f;
                p.fillColor = new Color(0.42f, 0.29f, 0.50f);
                p.BeginPath(); p.MoveTo(new Vector2(centre - 21, 8)); p.LineTo(new Vector2(centre + 21, 8));
                p.LineTo(new Vector2(centre + 21, 143)); p.LineTo(new Vector2(centre - 21, 143)); p.ClosePath(); p.Fill();
                p.fillColor = new Color(0.40f, 0.66f, 0.39f);
                for (int row = 0; row < 3; row++)
                    for (int column = 0; column < 6; column++)
                    {
                        float x = centre + (column < 3 ? -1 : 1) * (53 + (column % 3) * 36);
                        float y = 20 + row * 42;
                        p.BeginPath(); p.MoveTo(new Vector2(x, y)); p.LineTo(new Vector2(x - 11, y + 23));
                        p.LineTo(new Vector2(x + 11, y + 23)); p.ClosePath(); p.Fill();
                    }
                p.strokeColor = new Color(0.96f, 0.86f, 0.53f); p.lineWidth = 3;
                p.BeginPath(); p.MoveTo(new Vector2(centre, 132)); p.LineTo(new Vector2(centre, 25));
                p.LineTo(new Vector2(centre - 9, 36)); p.MoveTo(new Vector2(centre, 25)); p.LineTo(new Vector2(centre + 9, 36)); p.Stroke();
                p.fillColor = p.strokeColor; p.BeginPath(); p.Arc(new Vector2(centre, 132), 6, 0, 360); p.Fill();
            };
            Label caption = UiKit.Text("Standing trees     ←     Rack / access route     →     Standing trees", "faint");
            caption.style.unityTextAlign = TextAnchor.MiddleCenter;
            caption.style.position = Position.Absolute; caption.style.left = 0; caption.style.right = 0; caption.style.bottom = 0;
            Add(caption);
        }
    }

    // The rack lessons follow the forest and the Work Plan, so they stay true after a save and load: a rack counts as planned when no
    // tree on its line is left to plan, as approved when nothing felling is still pending, and as seen once its trees are gone.
    private void ObserveRacks(ScenarioOneManager m)
    {
        if (m.RackGroups.Count > 0) Record("rack.create");
        RackLayoutTracker layout = ui.RackLayout;
        if (layout == null || !layout.Complete) return;
        Record("rack.create");
        Record("rack.layout");
        bool pending = m.WorkOrders.Any(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending);
        bool approved = m.WorkOrders.Any(o => o.type == ScenarioWorkType.FellTree
            && (o.status == ScenarioWorkStatus.Approved || o.status == ScenarioWorkStatus.Completed));
        if (!pending && approved) Record("rack.approve");
        if (layout.Harvested && m.AnnualReports.Count > 0 && m.Interventions.Count > 0) Record("rack.result");
    }

    public void Observe()
    {
        ScenarioOneManager m = ui.Manager;
        ForestPlayer p = ui.Player;
        ObserveRacks(m);
        bool walking = ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && !m.AnyPanelOpen;
        if (walking && p != null && p.InspectedTree != null && p.InspectedTree.IsLiving) Record("tree.inspect");
        if (walking && ui.Marking != null)
        {
            if (ui.Marking.LivingCropTreeCount > 0) Record("tree.crop");
            if (ui.Marking.LivingMarkedCount > 0) Record("fell.mark");
            if (ui.Marking.AimingAtGround && p != null && !p.IsInspecting) Record("plant.ground");
            if (p != null && m.DeadwoodRecords.Any(d =>
                Vector2.Distance(new Vector2(p.transform.position.x, p.transform.position.z), new Vector2(d.worldPosition.x, d.worldPosition.z)) < 6f))
                Record("deadwood.visit");
        }
        if (m.WorkPlanOpen && !ui.Help.IsOpen)
        {
            Record("plan.open");
            foreach (ScenarioOneWorkOrder order in m.WorkOrders)
            {
                string topic = order.type == ScenarioWorkType.FellTree ? "fell"
                    : order.type == ScenarioWorkType.PruneTree ? "prune"
                    : order.type == ScenarioWorkType.PlantJuvenile ? "plant" : "clear";
                Record(topic + ".plan");
                if (order.status == ScenarioWorkStatus.Approved || order.status == ScenarioWorkStatus.Completed) Record(topic + ".approve");
                if (order.type == ScenarioWorkType.FellTree && order.fellingOutcome == FellingMaterialOutcome.RetainAsFallenDeadwood)
                    Record("deadwood.plan");
                if (order.type == ScenarioWorkType.PlantJuvenile && order.installShelter) Record("plant.shelter");
            }
            if (m.ManagementEvents.Any(e => e.eventType == ScenarioManagementEventType.StockPurchased)) Record("plant.stock");
        }
        if (ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && !ui.Help.IsOpen && m.AnnualReports.Count > 0)
        {
            Record("review.open");
            if (m.AnnualReviewSeen) Record("review.read");
            foreach (ScenarioManagementEvent e in m.ManagementEvents.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved
                && e.outcome == ScenarioManagementOutcome.Succeeded))
            {
                string topic = e.taskType == ScenarioWorkType.FellTree ? "fell"
                    : e.taskType == ScenarioWorkType.PruneTree ? "prune"
                    : e.taskType == ScenarioWorkType.PlantJuvenile ? "plant" : "clear";
                Record(topic + ".result");
                if (e.ecologicalTreatment == ScenarioEcologicalTreatment.TreeRetainedAsDeadwood) Record("deadwood.result");
            }
        }
    }
}
