using System;
using System.Collections.Generic;
using System.Linq;

// S1-D advisory management-cycle context (spec RD-4, FR-D1..D3, FR-F1).
//
// Everything here is DERIVED, on demand, from records the save already carries. Nothing is stored: there is no field, schema
// version, device preference, session latch or cache, and reading it writes nothing. It is the same kind of layer as
// ScenarioInterventionHistory (it consumes that class's intervention list and stale-order rule instead of re-parsing events).
//
// This is derived game context (layer L3), not forestry evidence and not an intervention schedule. It reports named facts, each
// with a reason, so the player can judge for themselves. It never says whether to intervene, never names an interval, and is
// never a gate: waiting and observing stays a valid choice, and an outstanding review acknowledgement never disables Approve
// Work Plan or Advance (nothing in the game reads this class to decide either).
//
// Chosen definitions (kept deliberately consistent with the intervention history the Annual Review already shows):
//  * "Resolved intervention" = one InterventionSummary from ScenarioInterventionHistory.Build / ScenarioOneManager.Interventions:
//    a work-plan year with at least one SUCCEEDED commissioned task. It is NOT the completion rule's plan parser
//    (ScenarioOneCompletionRules.Plans), which additionally requires a matching Completed order record and unique event IDs.
//    The two agree on normal saves and can differ on odd or malformed data; this class follows the history surface.
//  * "Reviewed" = an AnnualReviewAcknowledged event dated in or after the intervention's year whose event ID is greater than the
//    last WorkResolved event of that year (the same year-and-event-ID ordering the completion chronology uses). An
//    acknowledgement recorded before a later plan resolved does not cover that later plan.
//  * "Carried over" = an open order created in an earlier year than the current one.
//  * "Years since latest intervention" = CurrentYear minus the year that plan resolved. Context only.
public enum CycleReviewState
{
    NoIntervention,   // nothing has been resolved yet, so there is nothing to review
    Reviewed,         // an acknowledgement is recorded after the latest intervention resolved
    NotYetReviewed,   // none yet; advisory information only
    Unproven          // the event record cannot establish order (missing or duplicate event IDs); neither claim is made
}

// One named fact about the current cycle. Present says whether it applies; Reason is never empty, even when it does not apply.
public sealed class CycleComponent
{
    public string Id = "";
    public bool Present;
    public int Count;
    public string Reason = "";
    public readonly List<int> OrderIds = new List<int>();   // work orders behind an order-based component; ids only, no references
}

public sealed class ScenarioCycleState
{
    // Shown with every readout so no surface presents this as a schedule or a recommendation.
    public const string AdvisoryLabel = "Derived from your records as context for your own judgement. It is not a schedule, a recommendation or a requirement.";

    public int CurrentYear;
    public ScenarioOneOutcome Outcome;
    public int OutcomeYear = -1;
    public string OutcomeNote = "";

    public int ResolvedInterventionCount;
    public int LatestInterventionOrdinal;           // 0 when none
    public int LatestInterventionYear = -1;         // -1 when none
    public int YearsSinceLatestIntervention = -1;   // -1 when none; descriptive only
    public CycleReviewState LatestInterventionReview = CycleReviewState.NoIntervention;

    // Fixed order, for any surface that wants to list them.
    public readonly CycleComponent ApprovedAwaitingExecution = new CycleComponent { Id = "ApprovedAwaitingExecution" };
    public readonly CycleComponent PendingWork = new CycleComponent { Id = "PendingWork" };
    public readonly CycleComponent StaleOrInvalidOrders = new CycleComponent { Id = "StaleOrInvalidOrders" };
    public readonly CycleComponent LatestIntervention = new CycleComponent { Id = "LatestIntervention" };
    public readonly CycleComponent YearsSince = new CycleComponent { Id = "YearsSinceLatestIntervention" };

    public IReadOnlyList<CycleComponent> Components =>
        new[] { ApprovedAwaitingExecution, PendingWork, StaleOrInvalidOrders, LatestIntervention, YearsSince };

    // Builds the state from the persisted records. interventions must be ScenarioInterventionHistory.Build output over the same
    // records (ScenarioOneManager.Interventions); events and orders are the persisted lists. Mutates none of them.
    public static ScenarioCycleState Derive(IReadOnlyList<InterventionSummary> interventions,
        IReadOnlyList<ScenarioManagementEvent> events, IReadOnlyList<ScenarioOneWorkOrder> orders,
        int currentYear, ScenarioOneOutcome outcome, int outcomeYear, int reviewYear)
    {
        interventions = interventions ?? new List<InterventionSummary>();
        events = events ?? new List<ScenarioManagementEvent>();
        orders = orders ?? new List<ScenarioOneWorkOrder>();

        var state = new ScenarioCycleState { CurrentYear = currentYear, Outcome = outcome, OutcomeYear = outcomeYear };
        state.OutcomeNote = DescribeOutcome(outcome, outcomeYear, reviewYear);
        DeriveOrderComponents(state, orders, currentYear);
        DeriveInterventionComponents(state, interventions, events, currentYear);
        return state;
    }

    // Whether an acknowledgement is recorded for the plan resolved in interventionYear (definition above). Public so any surface
    // that needs "has this year's plan been reviewed" calls this one derivation instead of scanning events again.
    public static CycleReviewState ReviewStateForYear(IReadOnlyList<ScenarioManagementEvent> events, int interventionYear)
    {
        var recorded = (events ?? new List<ScenarioManagementEvent>()).Where(e => e != null).ToList();
        var resolution = recorded.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved && e.year == interventionYear).ToList();
        if (resolution.Count == 0) return CycleReviewState.NoIntervention;
        // Same guard as the completion chronology: ambiguous IDs cannot prove which came first.
        if (recorded.Any(e => e.eventId <= 0) || recorded.Select(e => e.eventId).Distinct().Count() != recorded.Count)
            return CycleReviewState.Unproven;
        int lastResolution = resolution.Max(e => e.eventId);
        return recorded.Any(e => e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged
            && e.year >= interventionYear && e.eventId > lastResolution)
            ? CycleReviewState.Reviewed
            : CycleReviewState.NotYetReviewed;
    }

    private static void DeriveOrderComponents(ScenarioCycleState state, IReadOnlyList<ScenarioOneWorkOrder> orders, int currentYear)
    {
        var open = orders.Where(o => o != null && o.IsOpen).OrderBy(o => o.workOrderId).ToList();
        var approved = open.Where(o => o.status == ScenarioWorkStatus.Approved).ToList();
        var pending = open.Where(o => o.status == ScenarioWorkStatus.Pending).ToList();

        Fill(state.ApprovedAwaitingExecution, approved);
        int approvedCarried = approved.Count(o => o.createdYear < currentYear);
        state.ApprovedAwaitingExecution.Reason = approved.Count == 0
            ? "No approved orders are waiting to be carried out."
            : $"{Plural(approved.Count, "approved order")} waiting for the next advance"
              + (approvedCarried > 0 ? $" ({approvedCarried} planned in an earlier year)." : ".");

        Fill(state.PendingWork, pending);
        int pendingCarried = pending.Count(o => o.createdYear < currentYear);
        state.PendingWork.Reason = pending.Count == 0
            ? "No planned orders are waiting for approval."
            : $"{Plural(pending.Count, "planned order")} not yet approved, so the next advance will not carry "
              + (pending.Count == 1 ? "it" : "them") + " out"
              + (pendingCarried > 0 ? $" ({pendingCarried} left over from an earlier year)." : ".");

        // One authority for what counts as stale or invalid: the history layer's own rule.
        List<StaleOrderNote> stale = ScenarioInterventionHistory.StaleOrders(orders, currentYear);
        Fill(state.StaleOrInvalidOrders, stale.Select(n => n.Order).ToList());
        int invalid = stale.Count(n => !string.IsNullOrEmpty(n.Order.validationMessage));
        state.StaleOrInvalidOrders.Reason = stale.Count == 0
            ? "No open order is invalid or old enough to need a fresh look."
            : $"{Plural(stale.Count, "open order")} may no longer suit the stand ({invalid} with a validation problem, "
              + $"{stale.Count - invalid} planned {ScenarioInterventionHistory.StaleAfterYears} or more years ago). They are listed, not removed or changed.";
    }

    private static void DeriveInterventionComponents(ScenarioCycleState state, IReadOnlyList<InterventionSummary> interventions,
        IReadOnlyList<ScenarioManagementEvent> events, int currentYear)
    {
        state.ResolvedInterventionCount = interventions.Count;
        InterventionSummary latest = interventions.Count > 0 ? interventions[interventions.Count - 1] : null;
        if (latest == null)
        {
            state.LatestIntervention.Reason = "No work plan has been carried out yet, so there is no intervention to review.";
            state.YearsSince.Reason = "No intervention yet, so there is nothing to count years from.";
            return;
        }

        state.LatestInterventionOrdinal = latest.Ordinal;
        state.LatestInterventionYear = latest.Year;
        state.YearsSinceLatestIntervention = Math.Max(0, currentYear - latest.Year);
        state.LatestInterventionReview = ReviewStateForYear(events, latest.Year);

        state.LatestIntervention.Present = true;
        state.LatestIntervention.Count = latest.Ordinal;
        string which = $"Intervention {latest.Ordinal} (carried out in year {latest.Year})";
        switch (state.LatestInterventionReview)
        {
            case CycleReviewState.Reviewed:
                state.LatestIntervention.Reason = $"{which} has an Annual Review acknowledgement recorded after it.";
                break;
            case CycleReviewState.NotYetReviewed:
                state.LatestIntervention.Reason = $"{which} has no Annual Review acknowledgement recorded after it yet. This is information only: it does not stop you approving a work plan or advancing.";
                break;
            default:
                state.LatestIntervention.Reason = $"{which}: the saved event record cannot show whether an acknowledgement came after it, so no claim is made. This does not stop you approving a work plan or advancing.";
                break;
        }

        state.YearsSince.Present = true;
        state.YearsSince.Count = state.YearsSinceLatestIntervention;
        state.YearsSince.Reason = $"{Plural(state.YearsSinceLatestIntervention, "year")} since intervention {latest.Ordinal}. "
            + "This only counts time; how long to wait is your judgement of the forest, and observing without acting is always allowed.";
    }

    private static void Fill(CycleComponent component, List<ScenarioOneWorkOrder> orders)
    {
        component.Present = orders.Count > 0;
        component.Count = orders.Count;
        component.OrderIds.Clear();
        component.OrderIds.AddRange(orders.Select(o => o.workOrderId));
    }

    private static string DescribeOutcome(ScenarioOneOutcome outcome, int outcomeYear, int reviewYear)
    {
        switch (outcome)
        {
            case ScenarioOneOutcome.Completed:
                return $"The scenario objectives were met in year {outcomeYear}. That is a record, not an end to management: cycle context stays available until the Century Review (year {reviewYear}).";
            case ScenarioOneOutcome.Failed:
                return "The scenario has ended as failed. The cycle context below describes the saved records only.";
            default:
                return "The scenario is active. Cycle context describes your saved records only.";
        }
    }

    private static string Plural(int count, string noun) => $"{count} {noun}{(count == 1 ? "" : "s")}";
}
