using System.Linq;
using UnityEngine;

public sealed partial class LearningObjectivesView
{
    // No tutorial forest, event, or save fields: rebuild from the existing world after each change/load.
    private ScenarioOneTeachingSequence.Facts sequenceFacts;
    private float nextSequenceRefresh;
    private ScenarioOneTeachingSequence.Facts TeachingFacts()
    {
        if (sequenceFacts != null && Time.unscaledTime < nextSequenceRefresh) return sequenceFacts;
        nextSequenceRefresh = Time.unscaledTime + .25f;
        var m = ui.Manager;
        var orders = m.WorkOrders;
        int firstFellingYear = orders.Where(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed)
            .Select(o => o.resolvedYear).DefaultIfEmpty(int.MaxValue).Min();
        var routes = ui.RackTool != null && ui.RackTool.Planner != null ? ui.RackTool.Planner.LayoutLines : null;
        if (routes != null && routes.Count > 0)
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
        return sequenceFacts = new ScenarioOneTeachingSequence.Facts
        {
            AccessOpen = ui.RackLayout != null && ui.RackLayout.Total > 0
                ? originalAccessOpen && m.CurrentEcologicalYear > 0 : firstFellingYear < int.MaxValue,
            AccessReviewed = Reviewed(firstFellingYear), CropTrees = cropTrees.Count,
            ReleasePlanned = plannedRelease, ReleaseExecuted = release.Count > 0,
            DeadwoodExecuted = Goal("fallen-deadwood") || m.ManagementEvents.Any(e => e.eventType == ScenarioManagementEventType.WorkResolved
                && e.outcome == ScenarioManagementOutcome.Succeeded && e.ecologicalTreatment == ScenarioEcologicalTreatment.TreeRetainedAsDeadwood), ReleaseReviewed = Reviewed(releaseYear),
            ReviewedTwoCycles = ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(m.ManagementEvents, orders),
            BeechEstablished = Goal("introduced-beech"), OakEstablished = Goal("introduced-sessile-oak"),
            PlantingYear = plantingYear, Year = m.CurrentEcologicalYear,
            EstablishmentReviewed = plantingYear >= 0 && m.CurrentEcologicalYear > plantingYear
                && m.ManagementEvents.Any(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
                    && e.year > plantingYear)
        };
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
        if (index == 0)
        {
            RackLayoutTracker layout = ui.RackLayout;
            if (layout != null && layout.Complete)
                return ui.Manager.WorkOrders.Any(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Pending)
                    ? "Open the Work Plan [Tab], approve the rack work, then advance one year."
                    : "Open the Work Plan [Tab] and advance one year.";
            if (layout != null && layout.HasProgress) return "Plan the remaining racks: [B] in the forest, or [M] on the Stand Map.";
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
        switch (ScenarioOneTeachingSequence.Current(f))
        {
            case 0: return ui.RackLayout != null ? $"Access routes: {ui.RackLayout.Done} of {ui.RackLayout.Total} planned" : "Plan permanent access";
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
