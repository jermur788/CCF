using System.Collections.Generic;
using System.Linq;

// What the player should be told while a reviewed second cycle is still missing. Presentation only: it reads the same
// chronology as the completion rule and never decides, records or changes anything.
public enum CompletionGuidanceState
{
    None,
    // A qualifying Work Plan exists but no acknowledgement has been recorded after it.
    AcknowledgeReview,
    // The first cycle is reviewed, all eight objectives are met, and only one qualifying Work Plan exists.
    ObjectivesMetOneCycle,
    // The first cycle is reviewed and the second cycle is not yet resolved.
    AnotherIntervention
}

// Completion chronology is derived from existing v20 records, never from annualReviewSeen.
// One annual resolution is one Work Plan, matching ScenarioInterventionHistory's year convention.
public static class ScenarioOneCompletionRules
{
    private sealed class ResolvedPlan
    {
        public int Year;
        public int FirstEventId;
        public int LastEventId;
    }

    private static List<ResolvedPlan> Plans(IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioOneWorkOrder> orders)
    {
        if (events == null || orders == null) return new List<ResolvedPlan>();
        // Ambiguous IDs cannot prove within-year chronology. Normal RecordEvent/Restore maintain uniqueness.
        var recorded = events.Where(e => e != null).ToList();
        if (recorded.Any(e => e.eventId <= 0) || recorded.Select(e => e.eventId).Distinct().Count() != recorded.Count)
            return new List<ResolvedPlan>();
        var completedOrders = orders.Where(o => o != null && o.status == ScenarioWorkStatus.Completed)
            .GroupBy(o => o.workOrderId).Where(g => g.Key > 0 && g.Count() == 1)
            .ToDictionary(g => g.Key, g => g.First());
        return recorded.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved && e.year >= 0)
            .GroupBy(e => e.year).Where(g => g.Any(e =>
                e.outcome == ScenarioManagementOutcome.Succeeded
                && completedOrders.TryGetValue(e.workOrderId, out ScenarioOneWorkOrder order)
                && order.resolvedYear == e.year && order.type == e.taskType
                && System.Enum.IsDefined(typeof(ScenarioWorkType), order.type)))
            .Select(g => new ResolvedPlan { Year = g.Key, FirstEventId = g.Min(e => e.eventId),
                LastEventId = g.Max(e => e.eventId) }).OrderBy(p => p.Year).ToList();
    }

    public static bool HasReviewedTwoCycleSequence(IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioOneWorkOrder> orders)
    {
        List<ResolvedPlan> plans = Plans(events, orders);
        if (plans.Count < 2) return false;
        foreach (ResolvedPlan first in plans)
            foreach (ResolvedPlan second in plans)
                if (first.Year < second.Year && events.Any(e => e != null
                    && e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
                    && e.year >= first.Year && e.year <= second.Year
                    && e.eventId > first.LastEventId && e.eventId < second.FirstEventId))
                    return true;
        return false;
    }

    public static bool NeedsReviewAcknowledgement(IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioOneWorkOrder> orders)
    {
        if (HasReviewedTwoCycleSequence(events, orders)) return false;
        ResolvedPlan latest = Plans(events, orders).LastOrDefault();
        return latest != null && !events.Any(e => e != null
            && e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
            && e.year >= latest.Year && e.eventId > latest.LastEventId);
    }

    // Repeated clicks/load at the same boundary are idempotent; newly resolved work permits a fresh acknowledgement.
    public static bool ShouldRecordAcknowledgement(IReadOnlyList<ScenarioManagementEvent> events, int year)
    {
        int lastResolution = events.Where(e => e != null && e.eventType == ScenarioManagementEventType.WorkResolved
            && e.year <= year).Select(e => e.eventId).DefaultIfEmpty(0).Max();
        return !events.Any(e => e != null && e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
            && e.year == year && e.eventId > lastResolution);
    }

    // Which message applies. Precedence: acknowledge first (it blocks everything else), then "objectives met, one cycle", then
    // the plain "another intervention". Nothing is returned once the reviewed two-cycle sequence exists or before any
    // qualifying Work Plan, so the existing hints are untouched in those states.
    public static CompletionGuidanceState GuidanceState(IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioOneWorkOrder> orders, bool allObjectivesMet)
    {
        if (HasReviewedTwoCycleSequence(events, orders)) return CompletionGuidanceState.None;
        List<ResolvedPlan> plans = Plans(events, orders);
        if (plans.Count == 0) return CompletionGuidanceState.None;
        if (NeedsReviewAcknowledgement(events, orders)) return CompletionGuidanceState.AcknowledgeReview;
        return allObjectivesMet && plans.Count == 1
            ? CompletionGuidanceState.ObjectivesMetOneCycle : CompletionGuidanceState.AnotherIntervention;
    }

    // Wording. The second intervention is never named: felling, pruning, planting or anything else qualifies.
    public static string GuidanceText(CompletionGuidanceState state)
    {
        switch (state)
        {
            case CompletionGuidanceState.AcknowledgeReview:
                return "Open the Annual Review and choose \u201cI've read the annual results\u201d. Scenario One completes only when an intervention "
                    + "you have reviewed there is followed by another intervention.";
            case CompletionGuidanceState.ObjectivesMetOneCycle:
                return "All eight objectives are met, but Scenario One also needs a second management decision after you review the first. "
                    + "Read what your intervention did in the Annual Review, then carry out another intervention when you judge the forest needs it.";
            case CompletionGuidanceState.AnotherIntervention:
                return "You have reviewed your intervention. Return to the forest and carry out another intervention when you judge the forest needs it; "
                    + "Scenario One completes once the eight objectives are met and that second intervention is done.";
            default:
                return "";
        }
    }
}
