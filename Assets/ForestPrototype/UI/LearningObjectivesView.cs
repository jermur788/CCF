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
    // The "Next lesson" line. The HUD reads it every frame, so it is rebuilt only when a step is recorded or progress is reset.
    private string cueText;
    private int cueDoneCount = -1;
    public string Summary
    {
        get
        {
            if (cueText == null || cueDoneCount != done.Count)
            {
                cueText = CueText(NextCueStep(IsDone));
                cueDoneCount = done.Count;
            }
            return cueText;
        }
    }

    public LearningObjectivesView(ScenarioOneUiRoot ui)
    {
        this.ui = ui;
        foreach (string id in StepIds) if (PlayerPrefs.GetInt(PreferencePrefix + id, 0) == 1) done.Add(id);
        Root = UiKit.Box("layer", "modal-backdrop");
        VisualElement modal = UiKit.Box("panel", "modal");
        modal.style.backgroundColor = new Color(14f / 255f, 22f / 255f, 16f / 255f, 1f);
        Root.Add(modal);
        VisualElement header = UiKit.Box("modal-header");
        UiKit.Add(header, "Learning objectives", "title");
        VisualElement buttons = UiKit.Row(header);
        buttons.Add(UiKit.Button("Stand map [M]", () => ui.ShowMap()));
        buttons.Add(UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan()));
        buttons.Add(UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary"));
        modal.Add(header);
        progress = UiKit.Add(modal, "", "body");
        UiKit.Add(modal, "Start simple. Open a topic to read its steps. There is no year-one deadline. Optional management practice can wait until it suits your forest. Progress is remembered on this device across forests; these lessons are separate from scenario success objectives.", "muted");
        scroll = new ScrollView(ScrollViewMode.Vertical);
        scroll.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
        scroll.style.flexGrow = 1;
        modal.Add(scroll);
    }

    public void Record(string id)
    {
        if (done.Contains(id) || !StepIds.Contains(id)) return;
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
        if (!force && shownCount == done.Count && shownRegenerationModel == regenerationModel && shownMarket == (market ? 1 : 0) && shownObjectives == objectiveText) return;
        shownObjectives = objectiveText;
        shownCount = done.Count;
        shownRegenerationModel = regenerationModel;
        shownMarket = market ? 1 : 0;
        float position = scroll.scrollOffset.y;
        progress.text = $"{Topics.Count(t => t.Steps.All(s => IsDone(s.Id)))} of {Topics.Length} topics complete · {done.Count} of {StepIds.Count()} steps · " + Summary;
        scroll.Clear();
        var success = new Foldout { text = "Current forest success objectives", value = true, name = "scenario-success-objectives" };
        UiKit.Add(success, "These are the current scenario targets, separate from optional learning steps. They describe progress, not a prescription to fell, plant or clear.", "muted");
        foreach (ScenarioObjectiveResult objective in ui.Manager.Objectives)
            UiKit.Add(success, ScenarioOneUiFacts.ObjectiveLine(objective), "body");
        scroll.Add(success);
        Topic next = Topics.FirstOrDefault(t => t.Steps.Any(s => !IsDone(s.Id)));
        foreach (Topic topic in Topics)
        {
            var foldout = new Foldout { text = $"{topic.Title} · {topic.Steps.Count(s => IsDone(s.Id))}/{topic.Steps.Length}",
                value = expanded.TryGetValue(topic.Title, out bool open) ? open : topic == next };
            foldout.AddToClassList("learning-topic");
            foldout.RegisterValueChangedCallback(e => expanded[topic.Title] = e.newValue);
            string explanation = topic.Steps[0].Id == "clear.read"
                ? ClearanceExplanation(regenerationModel) : topic.Text;
            UiKit.Add(foldout, explanation, "body");
            foreach (Step step in topic.Steps)
            {
                VisualElement card = UiKit.Box("card");
                UiKit.Add(card, (IsDone(step.Id) ? "Done · " : "To do · ") + step.Title, "heading");
                UiKit.Add(card, StepText(step, market), "body");
                if (step.Reading && !IsDone(step.Id))
                {
                    bool ready = step.Requires == null || IsDone(step.Requires);
                    Button read = UiKit.Button("I've read and understood this", () => { Record(step.Id); Refresh(true); }, ready);
                    read.name = "learn-" + step.Id;
                    card.Add(read);
                    if (!ready) UiKit.Add(card, "First complete: " + Topics.SelectMany(t => t.Steps).First(s => s.Id == step.Requires).Title, "muted");
                }
                else if (!step.Reading && !IsDone(step.Id))
                    UiKit.Add(card, "Progress updates when you perform this step in the game.", "faint");
                foldout.Add(card);
            }
            scroll.Add(foldout);
        }
        scroll.schedule.Execute(() => scroll.scrollOffset = new Vector2(0, position));
    }

    public void Observe()
    {
        ScenarioOneManager m = ui.Manager;
        ForestPlayer p = ui.Player;
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
