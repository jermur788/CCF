using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

// Optional learning progress belongs to the local player, not the forest model.
// Actions are observed; explanatory reading is explicitly acknowledged.
public sealed class LearningObjectivesView
{
    public const string PreferencePrefix = "CCF.Learning.v1.";
    public sealed class Step
    {
        public readonly string Id, Title, Text, Requires;
        public readonly bool Reading;
        public Step(string id, string title, string text, bool reading = false, string requires = null)
        { Id = id; Title = title; Text = text; Reading = reading; Requires = requires; }
    }
    public sealed class Topic
    {
        public readonly string Title, Text;
        public readonly Step[] Steps;
        public Topic(string title, string text, params Step[] steps)
        { Title = title; Text = text; Steps = steps; }
    }
    public static readonly Topic[] Topics = {
        new Topic("1. Stand Map", "Start with opening the map and reading one cell. Practise navigation next; explore the other layers when you need them. The map finds places to investigate. It cannot make forestry decisions or carry out work remotely.",
            new Step("map.open", "Open the map", "Press M, or choose Stand map on the HUD. North is up. The white cell border shows your position; letter/number labels identify locations."),
            new Step("map.select", "Select a cell and read its information", "Click a cell. The side panel shows its light, canopy and young growth. Read that diagnosis before choosing a destination."),
            new Step("map.light", "Read the Light layer", "Choose Light. Values run from 0 (dark) to 1 (open), measured at ground level. Brighter cells have more ground light. This does not measure a particular tree's crown light."),
            new Step("map.waypoint", "Set a waypoint", "Choose a cell away from where you are standing, then Set waypoint. This adds a destination marker; it does not move you."),
            new Step("map.return", "Return to the forest", "Press M or Esc, or use Back to forest. The HUD now gives the destination's compass direction and distance."),
            new Step("map.arrive", "Walk to the destination", "Follow the HUD direction and watch the distance decrease until it says you are here. Walk from outside the destination cell; setting a waypoint on your own cell is not this practice journey."),
            new Step("map.inspectsite", "Inspect the site on foot", "At the destination, look at the ground report or inspect a nearby tree with E. Compare what you see with the cell diagnosis before choosing any management work."),
            new Step("map.regeneration", "Explore Regeneration", "Choose Regeneration. P counts living planted juveniles; the height is the tallest natural seedling cohort. Read the side panel for species and conditions. More light or an opening can create an opportunity for natural regeneration, but it needs seed and suitable conditions; success is not guaranteed. An empty cell is information, not an instruction to plant."),
            new Step("map.browse", "Explore Browsing / protection", "Choose Browsing / protection. The map shows site browsing pressure and shelters expected to be effective next year. S counts shelters; a dash means none. Visit young growth and compare its condition with the ground report."),
            new Step("map.marks", "Explore Fell & crop marks", "Choose Fell & crop marks. Squares count Fell proposals; diamonds count Crop Trees. Red means proposed removal, blue means retain/favour. This is a view of decisions made in the forest, not a remote marking tool."),
            new Step("map.compare", "Understand the limits of a map", "A cell summarises an area. Switch layers to compare light, young growth, protection and your marks, then walk there. A colour or a low number alone does not tell you which tree to fell.", true, "map.inspectsite")),
        new Topic("2. Tree inspection and Crop Trees", "Begin with trees worth retaining and favouring, then inspect their competitors. Being smaller or suppressed is not by itself a reason to remove a tree. There is no single correct tree picked by this lesson.",
            new Step("tree.inspect", "Inspect a living tree", "Look at a tree and press E. Read its identity, age, height, DBH and competition. E closes inspection; F1 revisits the explanation."),
            new Step("tree.read", "Understand the measurements", "DBH is trunk diameter at breast height. Competition describes neighbours restricting growth. The light field is ground light in the tree's cell. Use the fields together with the tree and its neighbours, not as a command to remove it.", true, "tree.inspect"),
            new Step("tree.crop", "Designate a Crop Tree", "Aim at a living tree you want to retain and press C. A blue mark means retain/favour, not fell. Press C again to undo it. Choose a tree yourself after inspection; this designation is also used by crop-tree pruning.")),
        new Topic("3. Felling", "Continuous-cover forestry needs repeated selective management. More felling is not automatically better: consider what remains, then return later to inspect and reassess. Learn the proposal → plan → approval → results sequence. Take time to compare trees; no cutting is required in year one.",
            new Step("fell.mark", "Propose a tree for felling", "Aim at a living tree and press X. The red mark is only a proposal. X again removes it. Crop and Fell marks are alternatives on the same tree; check the label before proceeding."),
            new Step("fell.plan", "Import the Fell mark into the Work Plan", "Press Tab. Opening the Work Plan automatically imports red Fell marks as jobs; Add marked trees can also refresh that import. Inspect each job's timber estimate, contractor cost and material outcome. Removing a pending job lets you reconsider."),
            new Step("fell.cost", "Read the harvest costs", "Compare total labour, timber income, net value and remaining cash. A small visit can be dominated by the contractor's minimum charge. Estimates are re-quoted when work resolves; no broadleaf timber market is configured in this scenario.", true, "fell.plan"),
            new Step("fell.approve", "Approve a felling job when ready", "Approve pending work commits the planned jobs. Approval does not immediately cut trees. Check every pending job first: that button can approve more than your felling proposal."),
            new Step("fell.result", "Review a successful felling result", "Advance a year when your plan is ready. Open Annual Review and read WORK DONE, MONEY and FOREST. Successful felling removes the standing tree; failed work needs its failure reason checked. Walk back to inspect the change.")),
        new Topic("4. Pruning", "Pruning removes lower branches from retained Crop Trees for future timber quality and a clearer stem. Scenario One records the treatment but applies no timber-price premium for pruning. It costs contractor time and money. A tree may need to grow or recover before it is eligible.",
            new Step("prune.read", "Understand eligibility", "The Work Plan reports how many Crop Trees are eligible. This game's configured lift heights are 2.5 m, 5 m and 6.5 m, with at most three lifts, at least five years between lifts and a crown-base limit below 60% of tree height. These are scenario rules, not a recommendation to prune every tree.", true, "tree.crop"),
            new Step("prune.plan", "Plan an eligible pruning lift", "In Work Plan, find Pruning and Add eligible crop tree pruning tasks. This can add several eligible trees; review each target height and cost, and remove jobs you do not want. If none are eligible, return in a later year."),
            new Step("prune.approve", "Approve the selected pruning work", "Check contractor cost and available cash, then approve pending work. A designated Crop Tree stays standing; the lift is executed during the annual work cycle."),
            new Step("prune.result", "Read the pruning result", "After a successful lift, read Annual Review. Return to that tree and inspect its Pruning field for the lift count and clear-stem height. Do not expect another immediate lift.")),
        new Topic("5. Fallen deadwood", "Deadwood is a material outcome of felling. Keeping a fallen stem on site differs from selling it or keeping timber for construction.",
            new Step("deadwood.read", "Understand the material choices", "Sell extracts timber for configured sale income. Keep for use retains usable construction timber. Leave as deadwood retains the fallen stem on site for the scenario's habitat record; it earns no timber sale income. Contractor work can still cost money.", true, "fell.plan"),
            new Step("deadwood.plan", "Choose Leave as deadwood", "On a pending felling job, choose Leave as deadwood. Compare the new quote and cash balance before approving. This changes that job's outcome; it does not create a log until work succeeds."),
            new Step("deadwood.result", "Review retained deadwood", "Resolve the approved job in a later annual cycle and read FOREST in Annual Review. It reports newly retained volume and fallen logs on site. The scenario records decay over time; deadwood is not stored construction timber."),
            new Step("deadwood.visit", "Visit a retained fallen stem", "Walk back to within a few metres of a retained log and look at the site. Compare the standing-tree removal and the fallen stem with the recorded outcome. This lesson can wait until you choose a suitable habitat treatment.")),
        new Topic("6. Planting and protection", "Use ground observations to choose a site. Planting is not automatically an improvement; natural regeneration is not required for every useful management outcome. Planning a spot, paying for nursery stock and approving work are separate steps.",
            new Step("plant.ground", "Read the ground report", "Look at the ground to see light, regeneration and browsing conditions. Use the map to locate another area if useful. Check existing growth before deciding whether planting is appropriate."),
            new Step("plant.stock", "Buy nursery stock", "In Work Plan, open Nursery, choose a quantity and buy the species you intend to plant. Read the total purchase cost and cash warning before buying. Buying stock does not plant it. Contractor planting and shelter materials cost extra; using your own time for unsheltered planting has no external labour charge. Stock is consumed when successful planting work uses it."),
            new Step("plant.plan", "Mark a planting spot", "Back in the forest, press G for planting mode, select owned stock and click the intended ground spot. This creates a planned marker, not an immediately established tree. Esc leaves planting mode."),
            new Step("plant.execution", "Read executor and shelter choices", "Work Plan lets planting use contractor labour or your limited annual time. Shelters add material cost and use the same executor as planting. Check browsing and your budget; changed approved work needs approval again. Review each order before advancing.", true, "plant.plan"),
            new Step("plant.shelter", "Plan a shelter when appropriate", "Set Shelters ON in Work Plan and inspect the planting order's with shelter label. Do this only when you choose protection for the site; this optional practice can wait. The browsing layer later shows whether shelters remain effective."),
            new Step("plant.approve", "Approve planting work", "Check stock availability, labour/time, shelters and available cash. Approve the jobs you intend to carry out. Approval reserves a plan; establishment is recorded after the annual cycle succeeds."),
            new Step("plant.result", "Read planting results", "Read Annual Review after successful planting, then revisit the marked site. Later years show growth and browsing response. A successful job does not guarantee that a juvenile survives indefinitely.")),
        new Topic("7. Vegetation clearance", "Clearance removes competing vegetation inside the previewed cell footprint. It is a planned contractor operation; standing trees remain.",
            new Step("clear.read", "Read the footprint before planning", "Aim at ground with competing vegetation and use U to plan area clearance. Read the preview for the actual targets and area. Existing young growth inside the footprint may be removed, so inspect it before approving.", true, "plant.ground"),
            new Step("clear.plan", "Plan a clearance area", "Use the forest preview to designate a cell, then find Vegetation clearance in Work Plan. Check the cell label, targeted vegetation, contractor cost and any validation problem. Do not clear a cell just because the map is dark."),
            new Step("clear.approve", "Approve clearance when ready", "Approve the chosen work after checking what will be removed. Pending jobs can be removed if you change your mind. The plan is resolved at the annual cycle, not at designation."),
            new Step("clear.result", "Read clearance results", "Read the successful work result. Then revisit the footprint on foot to compare removed competing growth with the planned area. Standing trees are not part of this operation, and the site can develop new vegetation later.")),
        new Topic("8. Work Plan and Annual Review", "The forest is where you decide what should happen. The Work Plan reviews execution and cost; Annual Review records what actually happened.",
            new Step("plan.open", "Open Work Plan", "Press Tab. Compare your open jobs, cash, expected timber income and available owner time. Opening the plan neither approves jobs nor advances time."),
            new Step("plan.read", "Understand approval and time", "Approve pending work commits the chosen jobs. Advance one year executes approved jobs and advances the forest. Check all pending jobs and costs first. Tutorials have no first-year deadline; take observations and decisions at your own pace.", true, "plan.open"),
            new Step("review.open", "Open actual annual results", "After an annual cycle, read WORK DONE for successes/failures, MONEY for costs/income and FOREST for changes. An empty year-zero review is not a completed cycle."),
            new Step("review.read", "Read and acknowledge the results", "Use I've read the annual results after reading the three sections. The first acknowledgement unlocks further year advances. Keep using the results to choose what to inspect next; you need not finish every learning topic to continue."))
    };

    private readonly ScenarioOneUiRoot ui;
    private readonly HashSet<string> done = new HashSet<string>();
    private readonly ScrollView scroll;
    private readonly Label progress;
    private readonly Dictionary<string, bool> expanded = new Dictionary<string, bool>();
    private int shownCount = -1;
    private int shownRegenerationModel = -1;
    private string shownObjectives = "";
    public VisualElement Root { get; }
    public static IEnumerable<string> StepIds => Topics.SelectMany(t => t.Steps).Select(s => s.Id);
    public bool IsDone(string id) => done.Contains(id);
    public int CompletedSteps => done.Count;
    public string Summary
    {
        get
        {
            Topic next = Topics.FirstOrDefault(t => t.Steps.Any(s => !IsDone(s.Id)));
            if (next == null) return "Learning complete · revisit with O";
            Step step = next.Steps.First(s => !IsDone(s.Id));
            return "Next lesson: " + step.Title + " · [O] Objectives";
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
        string objectiveText = string.Join("\n", ui.Manager.Objectives.Select(ScenarioOneUiFacts.ObjectiveLine));
        if (!force && shownCount == done.Count && shownRegenerationModel == regenerationModel && shownObjectives == objectiveText) return;
        shownObjectives = objectiveText;
        shownCount = done.Count;
        shownRegenerationModel = regenerationModel;
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
                UiKit.Add(card, step.Text, "body");
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
