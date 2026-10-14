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

// Presentation-only projection. The qualification filter mirrors the protected N3 rules; it never decides completion.
public static class ScenarioOneCompletionGuidance
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

    // Which message applies. Precedence: acknowledge first (it blocks everything else), then "objectives met, one cycle", then
    // the plain "another intervention". Nothing is returned once the reviewed two-cycle sequence exists or before any
    // qualifying Work Plan, so the existing hints are untouched in those states.
    public static CompletionGuidanceState GuidanceState(IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioOneWorkOrder> orders, bool allObjectivesMet)
    {
        if (ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(events, orders)) return CompletionGuidanceState.None;
        List<ResolvedPlan> plans = Plans(events, orders);
        if (plans.Count == 0) return CompletionGuidanceState.None;
        if (ScenarioOneCompletionRules.NeedsReviewAcknowledgement(events, orders)) return CompletionGuidanceState.AcknowledgeReview;
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
                    + "you have reviewed there is followed by another intervention in a different simulation year.";
            case CompletionGuidanceState.ObjectivesMetOneCycle:
                return "All eight objectives are met. Another management cycle is required in a different simulation year. "
                    + "Return to the forest and carry out another intervention when you judge the forest needs it.";
            case CompletionGuidanceState.AnotherIntervention:
                return "You have reviewed your intervention. Return to the forest and carry out another intervention when you judge the forest needs it; "
                    + "Scenario One completes once the eight objectives are met and that second intervention is resolved in a different simulation year.";
            default:
                return "";
        }
    }
}
