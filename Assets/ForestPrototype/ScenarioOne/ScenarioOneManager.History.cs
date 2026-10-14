using System.Collections.Generic;
using System.Linq;

// S1-C / S1-D read access to the derived management history (ScenarioInterventionHistory). Everything here is computed
// on demand from persisted records; nothing is stored, so there is no new save field and no schema change.
public sealed partial class ScenarioOneManager
{
    // One entry per resolved work plan (a year in which at least one commissioned task succeeded), oldest first.
    // The ordinal is a presentation count; it is separate from the thinning tiers the economy uses.
    public IReadOnlyList<InterventionSummary> Interventions =>
        ScenarioInterventionHistory.Build(managementEvents, annualReports, ecologicalSnapshots, deadwoodRecords, workOrders);

    // Open orders that deserve a fresh look: they carry a validation problem, or were planned a couple of years ago.
    // They are surfaced, never expired or deleted.
    public IReadOnlyList<StaleOrderNote> StaleOrders => ScenarioInterventionHistory.StaleOrders(workOrders, CurrentYear);

    // What the finished scenario looked like at the year its objectives were met (not year 100). Null until then.
    public CompletionSummary CompletionReview =>
        outcome == ScenarioOneOutcome.Completed && definition != null
            ? ScenarioInterventionHistory.Completion(outcomeYear, definition.StartingCashCents, managementEvents, annualReports,
                ecologicalSnapshots, deadwoodRecords, workOrders)
            : null;

    // Presentation only: which "a reviewed second cycle is still missing" message applies now. It reads the same records as the
    // completion rule and never records, approves or acknowledges anything.
    public CompletionGuidanceState CompletionGuidance
    {
        get
        {
            if (outcome != ScenarioOneOutcome.Active || ecology == null) return CompletionGuidanceState.None;
            IReadOnlyList<ScenarioObjectiveResult> objectives = Objectives;
            bool allMet = objectives.Count > 0 && objectives.All(result => result.achieved);
            return ScenarioOneCompletionRules.GuidanceState(managementEvents, workOrders, allMet);
        }
    }

    // The repeatable "what next" line for every cycle after the first review. While a reviewed second cycle is still missing it
    // says so (after completion and stale orders, which keep their priority).
    public string NextStepHint => ScenarioInterventionHistory.NextStepHint(Interventions, CurrentYear, StaleOrders.Count,
        outcome == ScenarioOneOutcome.Completed, outcomeYear, ReviewYear,
        ScenarioOneCompletionRules.GuidanceText(CompletionGuidance));

    // Called by the UI when the player opens the Work Plan, so orders left from earlier years are checked against today's
    // forest (a target may have died, stock may have been used) and cached quotes are rebuilt. It only refreshes each
    // open order's validation message; nothing is removed, cancelled or approved.
    public void RevalidateOpenOrders()
    {
        ValidateOpenOrders();
        InvalidateEconomyQuotes();
    }
}
