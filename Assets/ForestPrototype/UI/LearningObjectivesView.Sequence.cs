using System.Linq;
using UnityEngine;

public sealed partial class LearningObjectivesView
{
    // World facts are derived; genuine inspection/decision evidence stays in this view's session and repeats after load.
    private readonly ScenarioOneRackReview rackReview = new ScenarioOneRackReview();
    private int teachingWorldVersion = -1;
    private int lastRackInspectionSerial = -1;
    private ScenarioOneTeachingSequence.Facts sequenceFacts;
    private float nextSequenceRefresh;
    private ScenarioOneTeachingSequence.Facts TeachingFacts()
    {
        if (teachingWorldVersion != ui.Manager.TeachingWorldVersion)
        {
            teachingWorldVersion = ui.Manager.TeachingWorldVersion;
            rackReview.Reset(ui.Player != null ? ui.Player.InspectionSerial : -1);
            sequenceFacts = null;
        }
        if (sequenceFacts != null && Time.unscaledTime < nextSequenceRefresh) return sequenceFacts;
        nextSequenceRefresh = Time.unscaledTime + .25f;
        var m = ui.Manager;
        var orders = m.WorkOrders;
        int firstFellingYear = orders.Where(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed)
            .Select(o => o.resolvedYear).DefaultIfEmpty(int.MaxValue).Min();
        var routes = ui.RackTool != null && ui.RackTool.Planner != null ? ui.RackTool.Planner.LayoutLines : null;
        // A Rack Planning V2 world opens its access through the racks' own work, not through planted trees that happen to lie on the
        // authored lines: the year is when that rack-owned felling was carried out, and access is open once every rack's work is resolved.
        bool rackV2 = m.RackPlanningV2Active && ui.RackLayout != null;
        if (rackV2)
            firstFellingYear = orders.Where(o => o.rackId > 0 && o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed)
                .Select(o => o.resolvedYear).DefaultIfEmpty(int.MaxValue).Max();
        else if (routes != null && routes.Count > 0)
            firstFellingYear = orders.Where(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed
                && o.targetTreeId.StartsWith("P", System.StringComparison.Ordinal)
                && routes.Any(z => Mathf.Abs(o.worldPosition.z - z) <= RackGeometry.DefaultWidthM * .5f))
                .Select(o => o.resolvedYear).DefaultIfEmpty(int.MaxValue).Max();
        var laterFelling = orders.Where(o => o.type == ScenarioWorkType.FellTree && o.createdYear >= firstFellingYear).ToList();
        // The real competition cutoff identifies a relevant neighbour, without prescribing a release percentage.
        // This is a teaching predicate only, never a work-validation or scenario-completion rule.
        var trees = Object.FindObjectsByType<ForestTree>(FindObjectsSortMode.None);
        var cropTrees = trees.Where(t => t.IsLiving && t.IsCropTree).ToList();
        // The original plantation P-ID contract is also used by RackPlanner to recover the authored rows.
        // Later recruitment in an access corridor needs maintenance, not a restart of the first lesson.
        var originalAccessTrees = routes != null ? trees.Where(t => t.TreeId.StartsWith("P", System.StringComparison.Ordinal)
            && routes.Any(z => Mathf.Abs(t.transform.position.z - z) <= RackGeometry.DefaultWidthM * .5f)).ToList() : null;
        bool originalAccessOpen = originalAccessTrees != null && originalAccessTrees.Count > 0
            && originalAccessTrees.All(t => !t.CanChop || t.IsCropTree);
        bool NearCrop(ScenarioOneWorkOrder o) => cropTrees.Any(t => t.TreeId != o.targetTreeId && Vector2.Distance(
            new Vector2(t.transform.position.x, t.transform.position.z),
            new Vector2(o.worldPosition.x, o.worldPosition.z)) <= ForestEcologyController.HegyiCutoffMeters);
        bool plannedRelease = laterFelling.Any(o => o.IsOpen && string.IsNullOrEmpty(o.validationMessage) && NearCrop(o));
        var release = laterFelling.Where(o => o.status == ScenarioWorkStatus.Completed && NearCrop(o)).ToList();
        int releaseYear = release.Select(o => o.resolvedYear).DefaultIfEmpty(-1).Max();
        int plantingYear = orders.Where(o => o.type == ScenarioWorkType.PlantJuvenile && o.status == ScenarioWorkStatus.Completed)
            .Select(o => o.resolvedYear).DefaultIfEmpty(-1).Max();
        bool Reviewed(int year) => year >= 0 && m.ManagementEvents.Any(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
            && e.year >= year && e.eventId > m.ManagementEvents.Where(r => r.eventType == ScenarioManagementEventType.WorkResolved && r.year == year)
                .Select(r => r.eventId).DefaultIfEmpty(0).Max());
        bool Goal(string id) => m.Objectives.Any(o => o.objectiveId == id && o.achieved);
        // Objectives 2-5 are one lesson on one rack area (ScenarioOneRackBay). A stand with no rack layout (a 40 m legacy stand) keeps the simple rules above.
        ScenarioOneRackBay.Analysis bay;
        if (m.RackPlanningV2Active)
        {
            // Rack Planning V2 (MFM-02): the area is the third CONFIRMED rack across the racks' common normal, never an authored line.
            // While the plan is unconfirmed (or invalid) there is no area: it suspends instead of rebinding to a line or another rack.
            // An active V2 world is always Enlarged80, so that is the stand the racks lie in.
            var sample = m.RackPlanConfirmed
                ? ScenarioOneRackSampleGeometry.Derive(m.ConfirmedRacks, StandGeometryModel.StandSizeMeters(StandGeometryModel.Enlarged80) * 0.5f) : null;
            bay = sample != null ? ScenarioOneRackBayReader.Read(trees, orders, sample) : null;
        }
        else
        {
            var layout = ui.RackTool != null ? ui.RackTool.Planner.LayoutStatus() : null;
            ScenarioOneRackBay.Frame? frame = layout != null && layout.Count > 0
                ? new ScenarioOneRackBay.Frame(layout[layout.Count / 2].Start, layout[layout.Count / 2].End,
                    ScenarioOneRackBay.HalfWidth(routes, routes.Count / 2)) : (ScenarioOneRackBay.Frame?)null;
            bay = routes != null && routes.Count > 0 ? ScenarioOneRackBayReader.Read(trees, orders, routes, frame) : null;
        }
        bool area = bay != null && bay.Applies;
        bool releaseReviewed = Reviewed(area ? bay.ReleaseYear : releaseYear);
        rackReview.Synchronize(bay, releaseReviewed, ui.Player != null ? ui.Player.InspectionSerial : -1);
        return sequenceFacts = new ScenarioOneTeachingSequence.Facts
        {
            AccessOpen = rackV2 ? ui.RackLayout.Harvested && m.CurrentEcologicalYear > 0
                : ui.RackLayout != null && ui.RackLayout.Total > 0
                    ? originalAccessOpen && m.CurrentEcologicalYear > 0 : firstFellingYear < int.MaxValue,
            AccessReviewed = Reviewed(firstFellingYear),
            CropTrees = area ? (bay.HasEnoughFuture ? bay.FutureCount : 0) : cropTrees.Count,
            ReleasePlanned = area ? bay.ReleasePlanned : plannedRelease, ReleaseExecuted = area ? bay.ReleaseExecuted : release.Count > 0,
            DeadwoodExecuted = area ? bay.MixedExecuted : Goal("fallen-deadwood") || m.ManagementEvents.Any(e => e.eventType == ScenarioManagementEventType.WorkResolved
                && e.outcome == ScenarioManagementOutcome.Succeeded && e.ecologicalTreatment == ScenarioEcologicalTreatment.TreeRetainedAsDeadwood),
            ReleaseReviewed = releaseReviewed, Bay = area ? bay : null,
            RackReviewComplete = area && rackReview.Complete(bay), RevisitedTrees = rackReview.InspectedCount,
            ReviewDecisions = rackReview.DecisionCount, StandPortionsReviewed = rackReview.StandPortionsReviewed,
            ReviewedTwoCycles = ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(m.ManagementEvents, orders),
            BeechEstablished = Goal("introduced-beech"), OakEstablished = Goal("introduced-sessile-oak"),
            PlantingYear = plantingYear, Year = m.CurrentEcologicalYear,
            EstablishmentReviewed = plantingYear >= 0 && m.CurrentEcologicalYear > plantingYear
                && m.ManagementEvents.Any(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
                    && e.year > plantingYear)
        };
    }

    // Called only while the real inspection panel is visible in the walked world, never from Annual Review or a map.
    public void ObserveRackInspection(ForestTree tree)
    {
        if (ui.Player == null || ui.Player.InspectionSerial == lastRackInspectionSerial) return;
        lastRackInspectionSerial = ui.Player.InspectionSerial;
        var f = TeachingFacts();
        if (tree == null || !tree.IsLiving || f.Bay == null || ScenarioOneTeachingSequence.Current(f) != 4) return;
        rackReview.Observe(f.Bay, tree.TreeId, tree.transform.position.x, tree.transform.position.z, ui.Player.InspectionSerial);
        sequenceFacts = null;
    }

    public bool DecideRackInspection(ForestTree tree, bool further)
    {
        sequenceFacts = null; // Include a competitor marked since the last quarter-second refresh.
        var f = TeachingFacts();
        if (tree == null || !tree.IsLiving || tree.IsMarkedForFell || ui.Manager.IsPlannedForFelling(tree.TreeId)
            || f.Bay == null || ScenarioOneTeachingSequence.Current(f) != 4) return false;
        bool changed = rackReview.Decide(f.Bay, tree.TreeId, tree.transform.position.x, tree.transform.position.z,
            further ? ScenarioOneRackReview.Decision.ReleaseFurther : ScenarioOneRackReview.Decision.RetainHere);
        sequenceFacts = null;
        return changed;
    }

    public string RackInspectionPrompt(ForestTree tree)
    {
        var f = TeachingFacts();
        int stage = ScenarioOneTeachingSequence.Current(f);
        if (tree == null || !tree.IsLiving || f.Bay == null || stage < 1 || stage > 4) return "";
        float x = tree.transform.position.x, z = tree.transform.position.z;
        if (!f.Bay.Contains(x, z)) return "Outside the teaching footprint: removals here earn no lesson credit.";
        int zone = f.Bay.Geometry.Zone(x, z);
        string location = "Teaching rack: " + (zone == 0 ? "near" : zone == 1 ? "middle" : "far") + " zone, "
            + (f.Bay.Geometry.Across(x, z) < 0 ? "right" : "left") + " side. ";
        if (stage != 4 || !f.ReleaseReviewed) return location;
        string status = $"Review: {f.ReviewDecisions}/3 tree decisions, {f.StandPortionsReviewed}/6 stand portions. ";
        return location + status + (f.Bay.Future.Any(t => t.Id == tree.TreeId)
            ? "Review this tree and surrounding retained stand. [J] Retain here  [K] Release further (mark relevant competitors first)."
            : "Review the surrounding retained stand here. [J] Keep this part of the stand. No cutting is required.");
    }

    // Interactive planning safeguard only; protected scenario-completion/economy rules remain unchanged.
    public string RackPlanProblem()
    {
        sequenceFacts = null;
        var f = TeachingFacts();
        return f.Bay != null && f.AccessOpen && !f.Bay.CanopyKept
            ? "Teaching rack safeguard exceeded (40% cumulative trunk area). Cancel/unmark in-footprint removals before approval or advance. Already executed excess cannot complete this lesson."
            : "";
    }

    // D-064 / OPEN-2: in a Rack Planning V2 world field marking happens before approval. While a rack that owns Pending removal work
    // is unconfirmed or its entrance tree carries no orange mark, the Work Plan does not approve. Read-only, like RackPlanProblem: the
    // manager's own approval API (simulation, reference and harness paths) is unchanged, and model-0 / Legacy40 worlds never see it.
    public string RackMarkingProblem()
    {
        ScenarioOneManager m = ui.Manager;
        if (m == null || !m.RackPlanningV2Active) return "";
        bool waiting = m.RackPlan.Any(r => r.Orders.Pending > 0
            && (r.Status != RackPlanStatus.Confirmed || !r.EntranceMarked || string.IsNullOrEmpty(r.EntranceTreeId)));
        if (!waiting) return "";
        return m.RackPlanConfirmed
            ? $"Mark the rack entrances in the forest before approving the rack work: {m.MarkedRackEntranceCount} / {ScenarioOneManager.RackPlanTargetCount} marked. [P] spray marker, aim within 3.5 m, left-click."
            : "Confirm the rack plan on the Stand Map [M], then mark the rack entrances in the forest before approving the rack work.";
    }

    // Expose the existing persisted acknowledgement at later teaching boundaries, even after N3 is satisfied.
    public bool NeedsReviewAcknowledgement
    {
        get
        {
            var f = TeachingFacts(); int index = ScenarioOneTeachingSequence.Current(f);
            if (index == 1) return !f.AccessReviewed;
            if (index == 4) return !f.ReleaseReviewed;
            return index == 6 && !ui.Manager.ManagementEvents.Any(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
                && e.year == ui.Manager.CurrentEcologicalYear);
        }
    }

    private string TeachingInstruction(ScenarioOneTeachingSequence.Facts f)
    {
        int index = ScenarioOneTeachingSequence.Current(f);
        string areaText = ScenarioOneRackBayText.Instruction(index, f);
        if (areaText != null) return areaText;
        if (index == 1 && !f.AccessReviewed)
            return "The racks are open. Read the Annual Review and choose I've read the annual results. Then walk back to inspect the changed stand before selecting future trees.";
        if (index == 3 && f.DeadwoodExecuted && !f.ReleaseExecuted)
            return "Fallen wood is already present. Review the selected release, costs and retained canopy in Work Plan [Tab]. Approve the work, then Advance one year to see the response.";
        if (index == 4 && !f.ReviewedTwoCycles && f.ReleaseReviewed)
            return "Results reviewed. Walk back and reassess the changed forest, then resolve another justified Work Plan in a later year. Your earlier plans had no review between them; this later cycle can demonstrate adaptive management.";
        if (index == 4 && !f.ReviewedTwoCycles)
            return "Read and acknowledge the Annual Review before the next intervention. Reassess the forest and resolve another justified Work Plan in a later year; the scenario needs a reviewed second management cycle.";
        if (index == 6 && f.Year <= f.PlantingYear)
            return "Read the planting-year Annual Review and inspect the planted sites. Then use Advance one year in Work Plan [Tab]. This extra year is an observation exercise, not a universal intervention interval.";
        if (index == 6)
            return "Read and acknowledge the Annual Review for this later year. Walk back to the planted sites and compare survival, browsing and vegetation with the planting-year results.";
        if (index == 7 && ui.Manager.Outcome == ScenarioOneOutcome.Completed)
            return "Scenario goals achieved. Review the completion summary and management history. You can continue observing the forest; transformation remains an ongoing process.";
        return ScenarioOneTeachingSequence.Lessons[index].Action;
    }

    // The same lesson as CurrentInstruction but short enough for the always-visible HUD panel (ActiveObjectiveHudWidget). It follows the same
    // states, so what the HUD asks for never contradicts the Objectives screen.
    public ActiveObjectiveHudWidget.Model HudModel()
    {
        var f = TeachingFacts();
        int index = ScenarioOneTeachingSequence.Current(f);
        var lesson = ScenarioOneTeachingSequence.Lessons[index];
        return new ActiveObjectiveHudWidget.Model
        {
            Number = index + 1,
            Total = ScenarioOneTeachingSequence.Lessons.Length,
            Title = lesson.Heading,
            Instruction = HudInstruction(f, index, lesson),
            Progress = TeachingProgress(f),
            Complete = index == ScenarioOneTeachingSequence.Lessons.Length - 1 && ui.Manager.Outcome == ScenarioOneOutcome.Completed
        };
    }

    private string HudInstruction(ScenarioOneTeachingSequence.Facts f, int index, ScenarioOneTeachingSequence.Lesson lesson)
    {
        string areaText = ScenarioOneRackBayText.Hud(index, f);
        if (areaText != null) return areaText;
        if (index == 0)
        {
            RackLayoutTracker layout = ui.RackLayout;
            // The V2 wording is the Objectives screen's, so the two cannot disagree.
            if (layout != null && layout.RackPlanV2) return layout.V2ObjectiveInstruction;
            if (layout != null && layout.Complete)
                return layout.WorkPending
                    ? "Open the Work Plan [Tab], approve the rack work, then advance one year."
                    : "Open the Work Plan [Tab] and advance one year.";
            // The V2 wording is the Objectives screen's, so the two cannot disagree and neither names a tool V2 does not have yet.
            if (layout != null && layout.HasProgress)
                return ui.Manager.RackPlanningV2Active ? layout.ProgressInstruction : "Plan the remaining racks: [B] in the forest, or [M] on the Stand Map.";
        }
        if (index == 1 && !f.AccessReviewed) return "Read the Annual Review, then walk back and look at the opened stand.";
        if (index == 3 && f.DeadwoodExecuted && !f.ReleaseExecuted) return "Review the release in the Work Plan [Tab], approve it and advance a year.";
        if (index == 4) return f.ReleaseReviewed ? "Reassess the forest, then plan another justified job in a later year."
            : "Read the Annual Review, reassess, then plan another justified job.";
        if (index == 6) return f.Year <= f.PlantingYear ? "Read the review, inspect the planted trees, then advance one year."
            : "Read the review and compare the planted trees with last year.";
        if (index == 7 && ui.Manager.Outcome == ScenarioOneOutcome.Completed) return "Scenario goals achieved. Keep observing: transformation is ongoing.";
        return lesson.HudAction;
    }

    private string TeachingProgress(ScenarioOneTeachingSequence.Facts f)
    {
        string areaText = ScenarioOneRackBayText.Progress(ScenarioOneTeachingSequence.Current(f), f);
        if (areaText != null) return areaText;
        switch (ScenarioOneTeachingSequence.Current(f))
        {
            case 0: return ui.RackLayout != null && ui.RackLayout.RackPlanV2 ? ui.RackLayout.V2CounterText
                : ui.RackLayout != null ? $"Access routes: {ui.RackLayout.Done} of {ui.RackLayout.Total} planned" : "Plan permanent access";
            case 1: return !f.AccessReviewed ? "Rack work done · read the annual results" : $"Future trees selected: {f.CropTrees} · choose at least one";
            case 2: return "Future tree selected · plan a nearby competitor for removal";
            case 3: return (f.ReleaseExecuted ? "Release done" : "Release planned") + (f.DeadwoodExecuted ? " · fallen wood present" : " · fallen wood still needed");
            case 4: return f.ReviewedTwoCycles ? "Second intervention resolved · review the response before acting again" : "Review before later work · a reviewed second cycle is still needed";
            case 5: return $"Beech: {(f.BeechEstablished ? "present" : "still needed")} · sessile oak: {(f.OakEstablished ? "present" : "still needed")}";
            case 6: return $"Planting year {f.PlantingYear} · current year {f.Year}";
            default: return $"Scenario goals met: {ui.Manager.Objectives.Count(o => o.achieved)} of {ui.Manager.Objectives.Count} · review the individual goals below";
        }
    }
}
