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
    public int CurrentTopicIndex => ScenarioOneTeachingSequence.Current(TeachingFacts());
    public bool TeachingRacks => CurrentTopicIndex == 0;
    public string CurrentTitle => ScenarioOneTeachingSequence.Lessons[CurrentTopicIndex].Title;
    public string CurrentInstruction
    {
        get
        {
            if (!TeachingRacks)
            {
                return TeachingInstruction(TeachingFacts());
            }
            RackLayoutTracker layout = ui.RackLayout;
            if (layout != null && layout.Complete)
            {
                bool pending = ui.Manager.WorkOrders.Any(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending);
                return pending ? "The rack layout is planned. Open Work Plan [Tab], review the cost and approve the rack work. Then advance one year."
                    : "Your rack work is ready. Open Work Plan [Tab] and choose Advance one year. Objective 2 follows that year advance.";
            }
            return layout != null && layout.HasProgress
                ? layout.ProgressInstruction
                : "Press B, click the tree or ground point where a rack should start, aim along the rows and click again to add it to the Work Plan. Then finish the marked routes on the Stand Map [M].";
        }
    }
    public string Summary => TeachingRacks
        ? (ui.RackLayout != null && ui.RackLayout.Complete ? "Current objective: approve rack work, then advance one year · [O] Details"
            : ui.RackLayout != null && ui.RackLayout.HasProgress
                ? $"Access routes: {ui.RackLayout.Done} of {ui.RackLayout.Total} fully planned · [M] Plan rack to finish · [O] Details"
                : "Current objective: create access racks · [B] Choose a start · [O] Details")
        : "Current objective: " + CurrentTitle + " · [O] Details";

    public LearningObjectivesView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        ReloadProgress();
        Root = UiKit.Box("layer", "modal-backdrop");
        UiKit.MakeModal(Root);
        VisualElement modal = UiKit.Box("panel", "modal", "modal-objectives");
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
        rackReview.Reset(ui.Player != null ? ui.Player.InspectionSerial : -1);
        sequenceFacts = null;
        done.Clear();
        foreach (string id in StepIds) if (PlayerPrefs.GetInt(PreferencePrefix + id, 0) == 1) done.Add(id);
        shownCount = -1;
    }

    public void Record(string id)
    {
        if (!StepIds.Contains(id)) return;
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
        if (force) sequenceFacts = null;
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
        VisualElement current = UiKit.Box("card", "objective-current");
        current.name = "current-objective";
        UiKit.Add(current, "CURRENT OBJECTIVE", "subtitle");
        UiKit.Add(current, CurrentTitle, "title").name = "current-objective-title";
        var lesson = ScenarioOneTeachingSequence.Lessons[CurrentTopicIndex];
        // The words on the left and the picture on the right, so the whole objective is on screen at once; a window too narrow for both
        // wraps the picture below the words.
        VisualElement body = UiKit.Box("objective-body");
        VisualElement words = UiKit.Box("objective-words");
        // Why it matters, what to do and, where the lesson teaches it, what good practice looks like: the reasoning, not only the controls.
        AddObjectivePart(words, "Why it matters", lesson.Concept, "current-objective-concept");
        AddObjectivePart(words, "What to do", CurrentInstruction, "current-objective-instruction");
        if (!string.IsNullOrEmpty(lesson.Practice)) AddObjectivePart(words, "What good practice looks like", lesson.Practice, "current-objective-practice");
        UiKit.Add(words, TeachingProgress(TeachingFacts()), "heading").name = "current-objective-progress";
        body.Add(words);
        // The picture grows with the lessons: this lesson's picture shows only what the lessons so far have taught.
        body.Add(new TeachingDiagram(CurrentTopicIndex, lesson.Caption));
        current.Add(body);
        scroll.Add(current);
        var completed = new Foldout { text = "Completed lessons", value = false, name = "completed-lessons" };
        for (int i = 0; i < CurrentTopicIndex; i++) UiKit.Add(completed, "✓ " + ScenarioOneTeachingSequence.Lessons[i].Title, "body");
        if (CurrentTopicIndex == 0) UiKit.Add(completed, "Your first lesson is in progress.", "muted");
        scroll.Add(completed);
        var later = new Foldout { text = "Later in this scenario", value = false, name = "future-lessons" };
        UiKit.Add(later, "Observe the consequences, favour future trees, secure young trees and repeat the management cycle. Lessons follow your forest and saved work history.", "muted");
        scroll.Add(later);

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

    private void BuildTopic(VisualElement parent, Topic topic, int regenerationModel, bool market)
    {
        UiKit.Add(parent, topic.Steps[0].Id == "clear.read" ? ClearanceExplanation(regenerationModel) : topic.Text, "body");
        foreach (Step step in topic.Steps)
        {
            bool complete = IsDone(step.Id);
            VisualElement card = UiKit.Box("card");
            UiKit.Add(card, (complete ? "Done · " : "To do · ") + step.Title, "heading");
            UiKit.Add(card, StepText(step, market), "body");
            if (step.Reading && !complete)
            {
                bool ready = step.Requires == null || IsDone(step.Requires);
                Button read = UiKit.Button("I've read and understood this", () => { Record(step.Id); Refresh(true); }, ready);
                read.name = "learn-" + step.Id;
                card.Add(read);
                if (!ready) UiKit.Add(card, "First complete: " + Topics.SelectMany(t => t.Steps).First(s => s.Id == step.Requires).Title, "muted");
            }
            parent.Add(card);
        }
    }

    private static void AddObjectivePart(VisualElement card, string heading, string text, string name)
    {
        UiKit.Add(card, heading, "objective-part-heading");
        UiKit.Add(card, text, "body", "objective-part-text").name = name;
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
