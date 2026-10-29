using System.Collections.Generic;
using System.Linq;

// The lesson list (topics, steps and their order). Kept free of UI Toolkit and PlayerPrefs so the order and wording can be
// checked offline on plain .NET. Step ids are the keys of the saved learning progress: never rename or reuse one. This is
// one half of LearningObjectivesView; the screen itself is in LearningObjectivesView.cs.
public sealed partial class LearningObjectivesView
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
        new Topic("1. Create access racks", "Scenario One begins by opening machine routes through the plantation, before any selective thinning. Racks are permanent routes used to remove timber from the stand. Establishing them now gives machinery access for this and future thinnings. Nothing is cut until you approve the work and advance a year.",
            new Step("rack.read", "What an extraction rack is", "A rack is a straight corridor of trees felled across the plantation so machinery can drive along it. It is permanent: it stays open for this and later thinnings. This plantation follows an authored rack plan of five evenly spaced racks across the planted rows (seven rows left standing between them). The status line and the Stand Map show how many are done.", true),
            new Step("rack.create", "Create your first rack", "Press B in the forest. Click the intended starting tree or ground point to fix the origin. Aim to pivot the violet corridor forward; the mouse wheel adjusts its direction. Click again to add the previewed trees to the Work Plan. Right-click chooses another start; Esc or B cancels."),
            new Step("rack.layout", "Complete the rack layout", "Open the rest of the authored rack plan: five racks in all. Use B in the forest, or open the Stand Map [M] and choose Plan rack, which shows the rows still to do. The status line counts the racks already in the Work Plan."),
            new Step("rack.approve", "Approve the rack work", "Open the Work Plan [Tab]. Each rack is one group with its trees, timber and income. Check the cost, then choose Approve Work Plan. Approval does not cut anything yet."),
            new Step("rack.result", "Advance a year and see the racks", "Advance one year and read the Annual Review. Then walk a rack: it is now an open corridor of stumps across the plantation. Inspecting trees, Crop Trees and selective felling come next.")),
        new Topic("2. Tree inspection and Crop Trees", "Begin with trees worth retaining and favouring, then inspect their competitors. Being smaller or suppressed is not by itself a reason to remove a tree. There is no single correct tree picked by this lesson.",
            new Step("tree.inspect", "Inspect a living tree", "Look at a tree and press E. Read its identity, age, height, DBH and competition. E closes inspection; F1 revisits the explanation."),
            new Step("tree.read", "Understand the measurements", "DBH is trunk diameter at breast height. Competition describes neighbours restricting growth. The light figure is the light on the ground at the tree's base. Use the fields together with the tree and its neighbours, not as a command to remove it.", true, "tree.inspect"),
            new Step("tree.crop", "Designate a Crop Tree", "Aim at a living tree you want to retain and press C. A blue mark means retain/favour, not fell. Press C again to undo it. Choose a tree yourself after inspection; this designation is also used by crop-tree pruning.")),
        new Topic("3. Felling", "Continuous-cover forestry needs repeated selective management. More felling is not automatically better: consider what remains, then return later to inspect and reassess. Learn the proposal → plan → approval → results sequence. Take time to compare trees; no cutting is required in year one.",
            new Step("fell.mark", "Propose a tree for felling", "Aim at a living tree and press X. The red mark is only a proposal. X again removes it. Crop and Fell marks are alternatives on the same tree; check the label before proceeding."),
            new Step("fell.plan", "Import the Fell mark into the Work Plan", "Press Tab. Your red Fell marks join the Work Plan as jobs when you open it. Inspect each job's timber estimate, contractor cost and material outcome. Removing a pending job lets you reconsider."),
            new Step("fell.cost", "Read the harvest costs", "Compare total labour, timber income, net value and remaining cash. A small visit can be dominated by the contractor's minimum charge. Estimates are re-quoted when work resolves.", true, "fell.plan"),
            new Step("fell.approve", "Approve a felling job when ready", "Approve Work Plan commits the planned jobs. Approval does not immediately cut trees. Check every pending job first: that button can approve more than your felling proposal."),
            new Step("fell.result", "Review a successful felling result", "Advance a year when your plan is ready. Open Annual Review and read WORK DONE, MONEY and FOREST. Successful felling removes the standing tree; failed work needs its failure reason checked. Walk back to inspect the change.")),
        new Topic("4. Work Plan and Annual Review", "The forest is where you decide what should happen. The Work Plan reviews execution and cost; Annual Review records what actually happened.",
            new Step("plan.open", "Open Work Plan", "Press Tab. Compare your open jobs, cash, expected timber income and available owner time. Opening the plan neither approves jobs nor advances time."),
            new Step("plan.read", "Understand approval and time", "Approve Work Plan commits the chosen jobs. Advance one year executes approved jobs and advances the forest. Check all pending jobs and costs first. Tutorials have no first-year deadline; take observations and decisions at your own pace.", true, "plan.open"),
            new Step("review.open", "Open actual annual results", "After an annual cycle, read WORK DONE for successes/failures, MONEY for costs/income and FOREST for changes. An empty year-zero review is not a completed cycle."),
            new Step("review.read", "Read and acknowledge the results", "Use I've read the annual results after reading the three sections. The first acknowledgement unlocks further year advances. Keep using the results to choose what to inspect next; you need not finish every learning topic to continue.")),
        new Topic("5. Stand Map", "Start with opening the map and reading one square. Practise navigation next; explore the other layers when you need them. The map finds places to investigate. It cannot make forestry decisions or carry out work remotely.",
            new Step("map.open", "Open the map", "Press M, or choose Stand map on the HUD. North is up. The white square border shows your position; letter/number labels identify locations."),
            new Step("map.select", "Select a square and read its information", "Click a square. The side panel shows its light, canopy and young growth. Read that diagnosis before choosing a destination."),
            new Step("map.light", "Read the Light layer", "Choose Light. Values run from 0 (dark) to 1 (open), measured at ground level. Brighter squares have more ground light. This does not measure a particular tree's crown light."),
            new Step("map.waypoint", "Set a waypoint", "Choose a square away from where you are standing, then Set waypoint. This adds a destination marker; it does not move you."),
            new Step("map.return", "Return to the forest", "Press M or Esc, or use Back to forest. The HUD now gives the destination's compass direction and distance."),
            new Step("map.arrive", "Walk to the destination", "Follow the HUD direction and watch the distance decrease until it says you are here. Walk from outside the destination square; setting a waypoint on your own square is not this practice journey."),
            new Step("map.inspectsite", "Inspect the site on foot", "At the destination, look at the ground report or inspect a nearby tree with E. Compare what you see with the square diagnosis before choosing any management work."),
            new Step("map.regeneration", "Explore Regeneration", "Choose Regeneration. P counts living planted juveniles; the height is the tallest natural seedling cohort. Read the side panel for species and conditions. More light or an opening can create an opportunity for natural regeneration, but it needs seed and suitable conditions; success is not guaranteed. An empty square is information, not an instruction to plant."),
            new Step("map.browse", "Explore Browsing / protection", "Choose Browsing / protection. The map shows site browsing pressure and shelters expected to be effective next year. S counts shelters; a dash means none. Visit young growth and compare its condition with the ground report."),
            new Step("map.marks", "Explore Fell & crop marks", "Choose Fell & crop marks. Squares count Fell proposals; diamonds count Crop Trees. Red means proposed removal, blue means retain/favour. This is a view of decisions made in the forest, not a remote marking tool."),
            new Step("rack.map", "Plan a rack on the Stand Map", "Open the Stand Map [M], choose Plan rack, pick a square on the row you want and add the rack. The map uses the same rack tool as the forest. This practice is optional."),
            new Step("map.compare", "Understand the limits of a map", "A square summarises an area. Switch layers to compare light, young growth, protection and your marks, then walk there. A colour or a low number alone does not tell you which tree to fell.", true, "map.inspectsite")),
        new Topic("6. Pruning", "Pruning removes lower branches from retained Crop Trees for future timber quality and a clearer stem. Pruning sets no price by itself: where Oak timber is marketed, the recorded pruned section can make a large log eligible for a higher grade, while Sitka and Beech have no such premium here, and fork or defect shape is not modelled. It costs contractor time and money. A tree may need to grow or recover before it is eligible.",
            new Step("prune.read", "Understand eligibility", "The Work Plan reports how many Crop Trees are eligible. Sitka uses lifts to 2.5 m, 5 m and 6.5 m (at most three lifts, at least five years apart). When Oak and Beech timber markets are active, Oak and Beech use two lifts, to 3.5 m then 6.0 m, at least three years apart; otherwise they keep the three-lift schedule above. All are limited to a crown base below 60% of tree height. These are scenario rules, not a recommendation to prune every tree.", true, "tree.crop"),
            new Step("prune.plan", "Plan an eligible pruning lift", "In Work Plan, find Pruning and Add eligible crop tree pruning tasks. This can add several eligible trees; review each target height and cost, and remove jobs you do not want. If none are eligible, return in a later year."),
            new Step("prune.approve", "Approve the selected pruning work", "Check contractor cost and available cash, then approve pending work. A designated Crop Tree stays standing; the lift is executed during the annual work cycle."),
            new Step("prune.result", "Read the pruning result", "After a successful lift, read Annual Review. Return to that tree and inspect its Pruning field for the lift count and clear-stem height. Do not expect another immediate lift.")),
        new Topic("7. Fallen deadwood", "Deadwood is a material outcome of felling. Keeping a fallen stem on site differs from selling it or keeping timber for construction.",
            new Step("deadwood.read", "Understand the material choices", "Sell extracts timber for configured sale income. Keep for use retains usable construction timber. Leave as deadwood retains the fallen stem on site for the scenario's habitat record; it earns no timber sale income. Contractor work can still cost money.", true, "fell.plan"),
            new Step("deadwood.plan", "Choose Leave as deadwood", "On a pending felling job, choose Leave as deadwood. Compare the new quote and cash balance before approving. This changes that job's outcome; it does not create a log until work succeeds."),
            new Step("deadwood.result", "Review retained deadwood", "Resolve the approved job in a later annual cycle and read FOREST in Annual Review. It reports newly retained volume and fallen logs on site. The scenario records decay over time; deadwood is not stored construction timber."),
            new Step("deadwood.visit", "Visit a retained fallen stem", "Walk back to within a few metres of a retained log and look at the site. Compare the standing-tree removal and the fallen stem with the recorded outcome. This lesson can wait until you choose a suitable habitat treatment.")),
        new Topic("8. Planting and protection", "Use ground observations to choose a site. Planting is not automatically an improvement; natural regeneration is not required for every useful management outcome. Planning a spot, paying for nursery stock and approving work are separate steps.",
            new Step("plant.ground", "Read the ground report", "Look at the ground to see light, regeneration and browsing conditions. Use the map to locate another area if useful. Check existing growth before deciding whether planting is appropriate."),
            new Step("plant.stock", "Buy nursery stock", "In Work Plan, open Nursery, choose a quantity and buy the species you intend to plant. Read the total purchase cost and cash warning before buying. Buying stock does not plant it. Contractor planting and shelter materials cost extra; using your own time for unsheltered planting has no external labour charge. Stock is consumed when successful planting work uses it."),
            new Step("plant.plan", "Mark a planting spot", "Back in the forest, press G for planting mode, select owned stock and click the intended ground spot. This creates a planned marker, not an immediately established tree. Esc leaves planting mode."),
            new Step("plant.execution", "Read executor and shelter choices", "Work Plan lets planting use contractor labour or your limited annual time. Shelters add material cost and use the same executor as planting. Check browsing and your budget; changed approved work needs approval again. Review each order before advancing.", true, "plant.plan"),
            new Step("plant.shelter", "Plan a shelter when appropriate", "Set Shelters ON in Work Plan and inspect the planting order's with shelter label. Do this only when you choose protection for the site; this optional practice can wait. The browsing layer later shows whether shelters remain effective."),
            new Step("plant.approve", "Approve planting work", "Check stock availability, labour/time, shelters and available cash. Approve the jobs you intend to carry out. Approval reserves a plan; establishment is recorded after the annual cycle succeeds."),
            new Step("plant.result", "Read planting results", "Read Annual Review after successful planting, then revisit the marked site. Later years show growth and browsing response. A successful job does not guarantee that a juvenile survives indefinitely.")),
        new Topic("9. Vegetation clearance", "Clearance removes competing vegetation inside the previewed square footprint. It is a planned contractor operation; standing trees remain.",
            new Step("clear.read", "Read the footprint before planning", "Aim at ground with competing vegetation and use U to plan area clearance. Read the preview for the actual targets and area. Existing young growth inside the footprint may be removed, so inspect it before approving.", true, "plant.ground"),
            new Step("clear.plan", "Plan a clearance area", "Use the forest preview to designate a square, then find Vegetation clearance in Work Plan. Check the square label, targeted vegetation, contractor cost and any validation problem. Do not clear a square just because the map is dark."),
            new Step("clear.approve", "Approve clearance when ready", "Approve the chosen work after checking what will be removed. Pending jobs can be removed if you change your mind. The plan is resolved at the annual cycle, not at designation."),
            new Step("clear.result", "Read clearance results", "Read the successful work result. Then revisit the footprint on foot to compare removed competing growth with the planned area. Standing trees are not part of this operation, and the site can develop new vegetation later."))
    };

    // Objective 1 in a Rack Planning V2 world (D-064). Spec §31 gives the meaning of the first three sentences and §7 the five-rack
    // sentence. 16-20 m is the book's early-transformation example and this stand's calibrated band; five racks is calibration for this
    // stand. Neither is taught as a general rule.
    public const string RackV2Concept = "Extraction racks allow machinery to reach trees without creating access everywhere across the stand. "
        + "In early CCF transformation, rack spacing may commonly be around 16–20 m, but appropriate spacing depends on the stand, machinery and site. "
        + "Scenario One uses that range as a guide for this plantation. "
        + "This stand needs five extraction racks to create a workable first access layout. "
        + "Plan the racks on the map, then mark each rack entrance in the forest.";
    public const string RackV2Practice = "Racks stay open for this and later thinnings, so plan the whole layout on the map before you confirm it. "
        + "The Stand Map shows each rack's spacing against this stand's recommended range. Leave the rows between racks standing: that standing forest is what you are keeping. "
        + "Confirming adds pending removal work only; nothing is cut until you approve the Work Plan and advance a year.";

    // The Objective 1 sentence in a V2 world: the derived counter, then the next step of the spec §30 flow. Plan and confirm on the
    // map, mark the five entrances in the forest, then the existing rack.approve step (D-064 OPEN-2: marking comes before approval).
    public static string RackV2Instruction(string counter, bool confirmed, int marked, int target, bool workPending)
    {
        if (!confirmed) return counter + ". Plan Racks on the Stand Map [M], then choose Confirm Rack Plan.";
        if (marked < target)
            return counter + ". Return to the forest and mark the first rack tree nearest the path at each rack entrance: [P] spray marker, aim within 3.5 m, left-click.";
        return counter + (workPending ? ". Next, review and approve the rack removal work in the Work Plan [Tab], then advance one year."
            : ". Open the Work Plan [Tab] and advance one year.");
    }

    // The rack lessons' wording in a V2 world, where the map is the only rack author and the entrances are marked before approval.
    // Null keeps the step's own text. The step ids and their saved progress are unchanged.
    public static string RackV2StepText(string stepId)
    {
        switch (stepId)
        {
            case "rack.read": return "A rack is a straight corridor of removed trees that machinery drives along, so the rest of the stand can be reached without driving through it. It stays open for this and later thinnings. "
                + "Spacing is conditional: around 16–20 m between racks may be common in early transformation, around 30 m or more may be viable in developed CCF, and there is no universal rule. "
                + "Scenario One recommends 16–20 m and five racks for this plantation only.";
            case "rack.create": return "Open the Stand Map [M] and choose Plan Racks. Create and edit your first rack on the map; no trees are cut by planning.";
            case "rack.layout": return "Plan five racks on the Stand Map [M] and choose Confirm Rack Plan. Then return to the forest and mark each rack entrance tree in orange: [P] spray marker, aim within 3.5 m, left-click. Tree Marks help explains the paint.";
            case "rack.approve": return "Once the rack entrances are marked, open the Work Plan [Tab]. Check the rack removal work and its cost, then choose Approve Work Plan. Approval does not cut anything yet.";
            case "rack.map": return "Open the Stand Map [M] and choose Plan Racks to create or edit rack geometry. The map is the rack-planning surface; it does not approve or execute work.";
            default: return null;
        }
    }

    // Presentation only. A planned layout is still objective 1 until its work is actually carried out in an annual cycle.
    // This reads the existing stand/year state, so device-wide practice credits cannot skip the opening rack lesson.
    public static int GuidedTopicIndex(bool hasLayout, bool racksHarvested, int ecologicalYear, System.Func<string, bool> isDone)
    {
        if (hasLayout && (!racksHarvested || ecologicalYear <= 0)) return 0;
        for (int i = hasLayout ? 1 : 0; i < Topics.Length; i++)
            if (Topics[i].Steps.Any(step => !isDone(step.Id))) return i;
        return Topics.Length - 1;
    }

    public static IEnumerable<string> StepIds => Topics.SelectMany(t => t.Steps).Select(s => s.Id);

    // The step the HUD names as "Next lesson". Topics are in teaching order (look at trees, mark, plan, approve, read the
    // review, then the map and the optional practices), so the first step left in that order is the cue. Action steps
    // come before reading steps: the cue should point at something to do in the forest, while the explanatory readings
    // wait in the Objectives screen until nothing else is left. Null when every step is done.
    public static Step NextCueStep(System.Func<string, bool> isDone)
    {
        foreach (Topic topic in Topics)
            foreach (Step step in topic.Steps)
                if (!step.Reading && !isDone(step.Id)) return step;
        foreach (Topic topic in Topics)
            foreach (Step step in topic.Steps)
                if (step.Reading && !isDone(step.Id)) return step;
        return null;
    }

    // Whether oak and beech earn timber income depends on the saved forest's growth model (ScenarioOneManager.BroadleafMarketActive),
    // so a lesson must not say either way on its own. The Harvest card in the Work Plan reports the same thing per job.
    public static string MarketSentence(bool broadleafMarketActive) => broadleafMarketActive
        ? "In this forest oak and beech stems have timber prices as well as Sitka spruce, so check each job's roadside value."
        : "In this forest only Sitka spruce earns timber income: felled oak or beech is charged for but earns no sale income.";

    // The text shown for a step. Most steps are fixed; the harvest-cost reading adds the market sentence for the active forest.
    public static string StepText(Step step, bool broadleafMarketActive)
        => step.Id == "fell.cost" ? step.Text + " " + MarketSentence(broadleafMarketActive) : step.Text;

    public static string CueText(Step step) => step == null
        ? "Learning complete · revisit with O"
        : "Next lesson: " + step.Title + " · [O] Objectives";
}
