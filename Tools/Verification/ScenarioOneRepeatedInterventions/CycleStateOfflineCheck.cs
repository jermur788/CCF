// Offline check for the S1-D T-02 advisory cycle state. Compiles the PRODUCTION ScenarioCycleState.cs,
// ScenarioInterventionHistory.cs, ScenarioOneCompletionRules.cs and the real persisted record classes, and runs on the plain
// .NET SDK. Nothing here is used by the game, and nothing is written inside the repository.
//
// Proves the twelve packet requirements: no-intervention explanation; one..four interventions use one derivation; idle years
// add nothing; approved vs pending; carried-over work; stale/invalid orders surfaced not changed; acknowledgement derived by
// year/event record and never a gate; descriptive years-since; non-mutating rebuild identical on a deep copy; Completed outcome
// keeps cycle context; no new save field and no UnityEngine dependency.
//
// Usage: run_offline_check.py   Prints CYCLE_STATE_OFFLINE_* tokens. It does not prove Unity behaviour or any UI.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

public static class CycleStateOfflineCheck
{
    private static int failures, checks;
    private static readonly List<ScenarioCycleState> AllStates = new List<ScenarioCycleState>();

    public static int Main()
    {
        NoIntervention();
        Symmetry();
        IdleYears();
        ApprovedVersusPending();
        CarriedOver();
        StaleAndInvalid();
        Acknowledgement();
        OutstandingAcknowledgementIsAdvisory();
        YearsSinceIsDescriptive();
        RebuildFromCopy();
        CompletedOutcome();
        InterventionDefinition();
        NoSaveFieldNoUnity();
        EveryReasonIsExplanatoryAndNonPrescriptive();
        Console.WriteLine(failures == 0
            ? $"CYCLE_STATE_OFFLINE_PASS checks={checks} NOT_UNITY_VERIFIED"
            : $"CYCLE_STATE_OFFLINE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // ---- fixture: persisted records as the manager would hold them ----
    private sealed class World
    {
        public readonly List<ScenarioManagementEvent> Events = new List<ScenarioManagementEvent>();
        public readonly List<ScenarioOneWorkOrder> Orders = new List<ScenarioOneWorkOrder>();
        public readonly List<ScenarioAnnualReport> Reports = new List<ScenarioAnnualReport>();
        public readonly List<ScenarioEcologicalSnapshot> Snapshots = new List<ScenarioEcologicalSnapshot>();
        public readonly List<ScenarioDeadwoodRecord> Deadwood = new List<ScenarioDeadwoodRecord>();
        private int nextEvent = 1, nextOrder = 1;

        // One resolved task in the advance into `year`; returns the event id.
        public int Resolve(int year, bool succeeded = true, bool withCompletedOrder = true)
        {
            int orderId = nextOrder++;
            if (withCompletedOrder)
                Orders.Add(new ScenarioOneWorkOrder { workOrderId = orderId, type = ScenarioWorkType.FellTree,
                    status = succeeded ? ScenarioWorkStatus.Completed : ScenarioWorkStatus.Failed, createdYear = year - 1, resolvedYear = year,
                    targetTreeId = "P" + orderId });
            return Add(year, ScenarioManagementEventType.WorkResolved,
                succeeded ? ScenarioManagementOutcome.Succeeded : ScenarioManagementOutcome.Failed, orderId, ScenarioWorkType.FellTree);
        }

        public int Ack(int year) => Add(year, ScenarioManagementEventType.AnnualReviewAcknowledged, ScenarioManagementOutcome.None, 0, ScenarioWorkType.FellTree);
        public int Advance(int year) => Add(year, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.None, 0, ScenarioWorkType.FellTree);

        public ScenarioOneWorkOrder Open(ScenarioWorkStatus status, int createdYear, string validation = "")
        {
            var order = new ScenarioOneWorkOrder { workOrderId = nextOrder++, type = ScenarioWorkType.FellTree, status = status,
                createdYear = createdYear, validationMessage = validation, targetTreeId = "Q" + nextOrder };
            Orders.Add(order);
            return order;
        }

        private int Add(int year, ScenarioManagementEventType type, ScenarioManagementOutcome outcome, int orderId, ScenarioWorkType task)
        {
            Events.Add(new ScenarioManagementEvent { eventId = nextEvent, year = year, eventType = type, outcome = outcome,
                workOrderId = orderId, taskType = task, targetTreeId = orderId > 0 ? "P" + orderId : "" });
            return nextEvent++;
        }
    }

    private static readonly int[] CycleYears = { 3, 7, 12, 20 };   // idle years between cycles

    // `cycles` resolved interventions with idle years between; every earlier one reviewed; the latest reviewed only if asked.
    private static World Cycles(int cycles, bool reviewLatest)
    {
        var w = new World();
        for (int i = 0; i < cycles; i++)
        {
            w.Advance(CycleYears[i] - 1);
            w.Resolve(CycleYears[i]);
            w.Advance(CycleYears[i]);
            if (i < cycles - 1 || reviewLatest) w.Ack(CycleYears[i]);
        }
        return w;
    }

    private static ScenarioCycleState Derive(World w, int year, ScenarioOneOutcome outcome = ScenarioOneOutcome.Active, int outcomeYear = -1, int reviewYear = 100)
    {
        List<InterventionSummary> interventions = ScenarioInterventionHistory.Build(w.Events, w.Reports, w.Snapshots, w.Deadwood, w.Orders);
        ScenarioCycleState state = ScenarioCycleState.Derive(interventions, w.Events, w.Orders, year, outcome, outcomeYear, reviewYear);
        AllStates.Add(state);
        return state;
    }

    // ---- 1. nothing resolved yet ----
    private static void NoIntervention()
    {
        ScenarioCycleState state = Derive(new World(), 1);
        Check(state.ResolvedInterventionCount == 0 && state.LatestInterventionYear == -1 && state.LatestInterventionOrdinal == 0, "no intervention: no latest");
        Check(state.LatestInterventionReview == CycleReviewState.NoIntervention, "no intervention: review state says so");
        Check(state.YearsSinceLatestIntervention == -1 && !state.YearsSince.Present, "no intervention: no years-since figure");
        Check(state.Components.All(c => !c.Present && c.Count == 0 && !string.IsNullOrWhiteSpace(c.Reason)),
            "no intervention: every component absent but explained");
        Check(!string.IsNullOrWhiteSpace(state.OutcomeNote), "no intervention: outcome note is not empty");
    }

    // ---- 2. one derivation for the 1st..4th intervention ----
    private static string Signature(ScenarioCycleState s) =>
        string.Join("|", s.Components.Select(c => c.Id + ":" + c.Present + ":" + (c.Id == "LatestIntervention" ? 0 : c.Count) + ":" + Regex.Replace(c.Reason, @"\d+", "#")))
        + "|" + s.LatestInterventionReview + "|" + s.YearsSinceLatestIntervention + "|" + Regex.Replace(s.OutcomeNote, @"\d+", "#");

    private static void Symmetry()
    {
        foreach (bool reviewed in new[] { true, false })
        {
            var signatures = new List<string>();
            for (int k = 1; k <= 4; k++)
            {
                World w = Cycles(k, reviewed);
                ScenarioCycleState state = Derive(w, CycleYears[k - 1] + 2);
                signatures.Add(Signature(state));
                Check(state.ResolvedInterventionCount == k && state.LatestInterventionOrdinal == k && state.LatestInterventionYear == CycleYears[k - 1],
                    $"cycle {k} ({(reviewed ? "reviewed" : "unreviewed")}): count/ordinal/year follow the history", $"{state.ResolvedInterventionCount}/{state.LatestInterventionOrdinal}/{state.LatestInterventionYear}");
                Check(state.LatestInterventionReview == (reviewed ? CycleReviewState.Reviewed : CycleReviewState.NotYetReviewed),
                    $"cycle {k} ({(reviewed ? "reviewed" : "unreviewed")}): review state", state.LatestInterventionReview.ToString());
                Check(state.YearsSinceLatestIntervention == 2, $"cycle {k}: years since is the plain difference");
            }
            Check(signatures.Distinct().Count() == 1, $"fourth cycle uses the same derivation as the first ({(reviewed ? "reviewed" : "unreviewed")})",
                signatures.Distinct().Count() == 1 ? "" : string.Join(" /// ", signatures));
        }
    }

    // ---- 3. waiting never creates an intervention ----
    private static void IdleYears()
    {
        World w = Cycles(2, true);
        ScenarioCycleState at9 = Derive(w, 9), at40 = Derive(w, 40);
        Check(at9.ResolvedInterventionCount == 2 && at40.ResolvedInterventionCount == 2, "idle years add no intervention");
        Check(at40.YearsSinceLatestIntervention - at9.YearsSinceLatestIntervention == 31, "idle years only lengthen the years-since count");
        World failedOnly = Cycles(1, true);
        failedOnly.Advance(7);
        failedOnly.Resolve(8, succeeded: false);
        failedOnly.Advance(8);
        ScenarioCycleState afterFailure = Derive(failedOnly, 9);
        Check(afterFailure.ResolvedInterventionCount == 1 && afterFailure.LatestInterventionYear == 3, "a year of only failed tasks is not an intervention (as in the history)");
        Check(ScenarioInterventionHistory.Build(failedOnly.Events, failedOnly.Reports, failedOnly.Snapshots, failedOnly.Deadwood, failedOnly.Orders).Count == 1,
            "…and the history agrees");
    }

    // ---- 4. approved work is not pending work ----
    private static void ApprovedVersusPending()
    {
        var w = new World();
        ScenarioOneWorkOrder a1 = w.Open(ScenarioWorkStatus.Approved, 4), a2 = w.Open(ScenarioWorkStatus.Approved, 4), p1 = w.Open(ScenarioWorkStatus.Pending, 4);
        w.Open(ScenarioWorkStatus.Completed, 2);
        w.Open(ScenarioWorkStatus.Failed, 2);
        ScenarioCycleState state = Derive(w, 4);
        Check(state.ApprovedAwaitingExecution.Present && state.ApprovedAwaitingExecution.Count == 2
            && state.ApprovedAwaitingExecution.OrderIds.SequenceEqual(new[] { a1.workOrderId, a2.workOrderId }), "approved orders are their own component");
        Check(state.PendingWork.Present && state.PendingWork.Count == 1 && state.PendingWork.OrderIds.SequenceEqual(new[] { p1.workOrderId }),
            "pending (unapproved) orders are a separate component");
        Check(!state.ApprovedAwaitingExecution.OrderIds.Intersect(state.PendingWork.OrderIds).Any(), "no order is in both");
        Check(state.ApprovedAwaitingExecution.Reason != state.PendingWork.Reason, "the two reasons say different things");
        Check(!state.ApprovedAwaitingExecution.Reason.Contains("earlier year") && !state.PendingWork.Reason.Contains("earlier year"),
            "orders made this year are not called carried over");
        Check(state.ApprovedAwaitingExecution.Count + state.PendingWork.Count == 3, "completed and failed orders are neither approved-waiting nor pending");
    }

    // ---- 5. carried-over work stays visible as pending context ----
    private static void CarriedOver()
    {
        var w = new World();
        ScenarioOneWorkOrder old = w.Open(ScenarioWorkStatus.Pending, 2);
        w.Open(ScenarioWorkStatus.Pending, 5);
        ScenarioCycleState state = Derive(w, 5);
        Check(state.PendingWork.Count == 2 && state.PendingWork.OrderIds.Contains(old.workOrderId), "an order left from year 2 is still pending work in year 5");
        Check(state.PendingWork.Reason.Contains("1 left over from an earlier year"), "…and the reason says one is left over", state.PendingWork.Reason);
        World approved = new World();
        approved.Open(ScenarioWorkStatus.Approved, 1);
        Check(Derive(approved, 5).ApprovedAwaitingExecution.Reason.Contains("1 planned in an earlier year"), "carried-over approved work is labelled too");
    }

    // ---- 6. stale or invalid open orders are surfaced, never expired or deleted ----
    private static void StaleAndInvalid()
    {
        var w = new World();
        ScenarioOneWorkOrder invalid = w.Open(ScenarioWorkStatus.Pending, 6, "Target tree has died.");
        ScenarioOneWorkOrder old = w.Open(ScenarioWorkStatus.Approved, 3);
        w.Open(ScenarioWorkStatus.Pending, 6);                        // fresh and valid: not stale
        w.Open(ScenarioWorkStatus.Completed, 1, "old note on a finished order");   // closed orders never count
        string before = Dump(new object[] { w.Orders, w.Events });
        ScenarioCycleState state = Derive(w, 6);
        List<StaleOrderNote> authority = ScenarioInterventionHistory.StaleOrders(w.Orders, 6);
        Check(state.StaleOrInvalidOrders.Present && state.StaleOrInvalidOrders.Count == authority.Count && authority.Count == 2,
            "stale/invalid count equals the history layer's own rule");
        Check(state.StaleOrInvalidOrders.OrderIds.SequenceEqual(authority.Select(n => n.Order.workOrderId)), "same orders, same order");
        Check(state.StaleOrInvalidOrders.OrderIds.Contains(invalid.workOrderId) && state.StaleOrInvalidOrders.OrderIds.Contains(old.workOrderId),
            "the invalid order and the old order are both surfaced");
        Check(state.StaleOrInvalidOrders.Reason.Contains("1 with a validation problem") && state.StaleOrInvalidOrders.Reason.Contains("not removed or changed"),
            "the reason distinguishes invalid from old and promises no change", state.StaleOrInvalidOrders.Reason);
        Check(Dump(new object[] { w.Orders, w.Events }) == before && w.Orders.Count == 4 && w.Orders.Count(o => o.IsOpen) == 3,
            "no order was expired, cancelled, deleted or revalidated");
        Check(state.ApprovedAwaitingExecution.OrderIds.Contains(old.workOrderId) && state.PendingWork.OrderIds.Contains(invalid.workOrderId),
            "a stale order is still counted as approved/pending work too");
    }

    // ---- 7. acknowledgement derived from year + event records ----
    private static void Acknowledgement()
    {
        CycleReviewState Review(World w) => Derive(w, 30).LatestInterventionReview;

        var w1 = new World(); w1.Resolve(3); w1.Ack(3);
        Check(Review(w1) == CycleReviewState.Reviewed, "ack in the intervention year after the resolution: reviewed");
        var w2 = new World(); w2.Resolve(3);
        Check(Review(w2) == CycleReviewState.NotYetReviewed, "no ack: not yet reviewed");
        var w3 = new World(); w3.Ack(3); w3.Resolve(3);
        Check(Review(w3) == CycleReviewState.NotYetReviewed, "an ack dated that year but recorded BEFORE the resolution does not count");
        var w4 = new World(); w4.Resolve(3); w4.Advance(3); w4.Ack(5);
        Check(Review(w4) == CycleReviewState.Reviewed, "an ack in a later year after the resolution counts");
        var w5 = new World(); w5.Ack(1); w5.Resolve(3);
        Check(Review(w5) == CycleReviewState.NotYetReviewed, "an ack from an earlier year does not cover a later intervention");
        var w6 = new World(); w6.Resolve(3); w6.Ack(2);
        Check(Review(w6) == CycleReviewState.NotYetReviewed, "an ack dated before the intervention year does not count even with a later event id");
        var w7 = new World(); w7.Resolve(3); w7.Ack(3); w7.Events.Add(null);
        Check(Review(w7) == CycleReviewState.Reviewed, "a null event entry is tolerated");
        var w8 = new World(); w8.Resolve(3); w8.Ack(3); w8.Events[1].eventId = w8.Events[0].eventId;
        Check(Review(w8) == CycleReviewState.Unproven, "duplicate event ids: order unprovable, no claim either way");
        var w9 = new World(); w9.Resolve(3); w9.Ack(3); w9.Events[0].eventId = 0;
        Check(Review(w9) == CycleReviewState.Unproven, "event id 0: order unprovable, no claim either way");
        Check(ScenarioCycleState.ReviewStateForYear(w1.Events, 3) == CycleReviewState.Reviewed && ScenarioCycleState.ReviewStateForYear(w1.Events, 9) == CycleReviewState.NoIntervention,
            "the per-year helper answers directly and says NoIntervention for a year with no resolution");
        Check(ScenarioCycleState.ReviewStateForYear(null, 3) == CycleReviewState.NoIntervention, "null events are tolerated");
    }

    // ---- 8. outstanding acknowledgement is advisory and gates nothing ----
    private static void OutstandingAcknowledgementIsAdvisory()
    {
        World w = Cycles(2, false);
        w.Open(ScenarioWorkStatus.Pending, 8);
        w.Open(ScenarioWorkStatus.Approved, 8);
        ScenarioCycleState state = Derive(w, 8);
        Check(state.LatestInterventionReview == CycleReviewState.NotYetReviewed && state.LatestInterventionOrdinal == 2, "after cycle two the latest is unreviewed");
        Check(state.ApprovedAwaitingExecution.Present && state.PendingWork.Present, "…while approved and pending work remain reported (nothing is switched off)");
        Check(state.LatestIntervention.Reason.Contains("does not stop you approving a work plan or advancing"), "…and the reason says it blocks neither Approve nor Advance",
            state.LatestIntervention.Reason);

        const BindingFlags Public = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        var members = new[] { typeof(ScenarioCycleState), typeof(CycleComponent) }.SelectMany(t => t.GetMembers(Public)).ToList();
        var banned = new Regex("Should|Ready|Can[A-Z]|Block|Gate|Allow|Disable|Enable|Recommend|Interval|Overdue|Due[A-Z]?|Required|Must|Intervene|Action", RegexOptions.None);
        Check(!members.Any(m => banned.IsMatch(m.Name)), "no public member is shaped like a gate, schedule or recommendation",
            string.Join(",", members.Where(m => banned.IsMatch(m.Name)).Select(m => m.Name)));
        var boolMembers = new[] { typeof(ScenarioCycleState), typeof(CycleComponent) }.SelectMany(t => t.GetFields(Public).Select(f => f.FieldType == typeof(bool) ? t.Name + "." + f.Name : null)
            .Concat(t.GetProperties(Public).Select(p => p.PropertyType == typeof(bool) ? t.Name + "." + p.Name : null))
            .Concat(t.GetMethods(Public).Select(mi => mi.ReturnType == typeof(bool) ? t.Name + "." + mi.Name : null))).Where(n => n != null).OrderBy(n => n).ToList();
        Check(boolMembers.SequenceEqual(new[] { "CycleComponent.Present" }), "the only boolean on the result says whether a fact applies; there is no single yes/no verdict",
            string.Join(",", boolMembers));

        // Nothing in the game reads the cycle state to decide Approve/Advance: only its own file and the read accessor mention it.
        string root = RepoRoot();
        var mentions = Directory.EnumerateFiles(Path.Combine(root, "Assets"), "*.cs", SearchOption.AllDirectories)
            .Where(f => File.ReadAllText(f).Contains("CycleState")).Select(f => Path.GetFileName(f)).OrderBy(n => n).ToList();
        Check(mentions.SequenceEqual(new[] { "ScenarioCycleState.cs", "ScenarioOneManager.History.cs" }), "only the adapter and its read accessor reference the cycle state (no UI or gate consumes it)",
            string.Join(",", mentions));
    }

    // ---- 9. years since is context only ----
    private static void YearsSinceIsDescriptive()
    {
        World w = Cycles(2, true);
        var masked = new HashSet<string>();
        bool flagsStable = true;
        foreach (int year in new[] { 12, 13, 14, 17, 40, 99 })
        {
            ScenarioCycleState s = Derive(w, year);
            masked.Add(Regex.Replace(s.YearsSince.Reason, @"\d+ years?", "N years"));
            flagsStable &= s.YearsSince.Present && s.YearsSince.Count == year - 7 && s.LatestInterventionReview == CycleReviewState.Reviewed
                && !s.ApprovedAwaitingExecution.Present && !s.PendingWork.Present && !s.StaleOrInvalidOrders.Present;
        }
        Check(flagsStable, "no years-since threshold changes any other component (no fixed interval)");
        Check(masked.Count == 1, "the years-since wording is identical at 5 and 92 years: no escalation");
        string reason = Derive(w, 14).YearsSince.Reason;
        Check(reason.Contains("only counts time") && reason.Contains("observing without acting is always allowed"), "the reason frames it as context and keeps waiting valid", reason);
        Check(Derive(w, 7).YearsSinceLatestIntervention == 0 && Derive(w, 5).YearsSinceLatestIntervention == 0, "never negative if the clock is earlier than the record");
    }

    // ---- 10. deterministic, non-mutating, identical on a field-for-field copy ----
    private static void RebuildFromCopy()
    {
        World w = Cycles(3, false);
        w.Open(ScenarioWorkStatus.Pending, 9); w.Open(ScenarioWorkStatus.Approved, 5, "No longer standing."); w.Open(ScenarioWorkStatus.Pending, 2);
        w.Reports.Add(new ScenarioAnnualReport { year = 7, contractorCostCents = 100, timberRevenueCents = 900, closingCashCents = 5000 });
        w.Snapshots.Add(new ScenarioEcologicalSnapshot { year = 6 }); w.Snapshots.Add(new ScenarioEcologicalSnapshot { year = 12 });
        string inputsBefore = Dump(new object[] { w.Events, w.Orders, w.Reports, w.Snapshots, w.Deadwood });
        string first = Dump(Derive(w, 15)), second = Dump(Derive(w, 15));
        Check(first == second, "deriving twice from the same records gives identical state");
        Check(Dump(new object[] { w.Events, w.Orders, w.Reports, w.Snapshots, w.Deadwood }) == inputsBefore, "deriving mutated none of the persisted inputs");

        var copy = new World();
        copy.Events.AddRange((List<ScenarioManagementEvent>)DeepCopy(w.Events));
        copy.Orders.AddRange((List<ScenarioOneWorkOrder>)DeepCopy(w.Orders));
        copy.Reports.AddRange((List<ScenarioAnnualReport>)DeepCopy(w.Reports));
        copy.Snapshots.AddRange((List<ScenarioEcologicalSnapshot>)DeepCopy(w.Snapshots));
        Check(Dump(new object[] { copy.Events, copy.Orders, copy.Reports, copy.Snapshots, copy.Deadwood }) == inputsBefore && !ReferenceEquals(copy.Orders[0], w.Orders[0]),
            "the copy is field-for-field equal and shares no objects");
        Check(Dump(Derive(copy, 15)) == first, "state rebuilt from the copy is equivalent (save/load shape)");
    }

    // ---- 11. Completed does not end cycle context ----
    private static void CompletedOutcome()
    {
        World w = Cycles(3, true);
        w.Open(ScenarioWorkStatus.Pending, 20);
        ScenarioCycleState active = Derive(w, 24, ScenarioOneOutcome.Active, -1, 100);
        ScenarioCycleState done = Derive(w, 24, ScenarioOneOutcome.Completed, 12, 100);
        Check(done.ResolvedInterventionCount == 3 && done.YearsSinceLatestIntervention == 12 && done.PendingWork.Present, "Completed still derives the cycle components");
        Check(Signature(done).Split('|').Take(5).SequenceEqual(Signature(active).Split('|').Take(5)), "components are identical to the Active state's");
        Check(done.OutcomeNote.Contains("year 12") && done.OutcomeNote.Contains("not an end to management") && done.OutcomeNote.Contains("year 100"),
            "the outcome note records the completion year and says management continues", done.OutcomeNote);
        ScenarioCycleState failed = Derive(w, 24, ScenarioOneOutcome.Failed, -1, 100);
        Check(!string.IsNullOrWhiteSpace(failed.OutcomeNote) && failed.ResolvedInterventionCount == 3, "a failed scenario still describes the records");
    }

    // ---- 12a. the chosen definitions, and how they relate to the completion rule ----
    private static void InterventionDefinition()
    {
        World w = Cycles(3, false);
        List<InterventionSummary> history = ScenarioInterventionHistory.Build(w.Events, w.Reports, w.Snapshots, w.Deadwood, w.Orders);
        ScenarioCycleState state = Derive(w, 22);
        Check(state.ResolvedInterventionCount == history.Count && state.LatestInterventionOrdinal == history.Last().Ordinal && state.LatestInterventionYear == history.Last().Year,
            "latest intervention is exactly the history surface's last InterventionSummary");

        // Normal save: the cycle state's review state and the completion rule's pending-acknowledgement test agree.
        World one = Cycles(1, false), oneReviewed = Cycles(1, true);
        Check(ScenarioOneCompletionRules.NeedsReviewAcknowledgement(one.Events, one.Orders) && Derive(one, 5).LatestInterventionReview == CycleReviewState.NotYetReviewed
            && !ScenarioOneCompletionRules.NeedsReviewAcknowledgement(oneReviewed.Events, oneReviewed.Orders) && Derive(oneReviewed, 5).LatestInterventionReview == CycleReviewState.Reviewed,
            "normal save: agrees with the completion chronology's own acknowledgement test");

        // After a reviewed two-cycle sequence the completion rule stops asking; the advisory state still reports the latest as unreviewed.
        World two = Cycles(2, false);
        Check(ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(two.Events, two.Orders) && Derive(two, 9).LatestInterventionReview == CycleReviewState.NotYetReviewed,
            "advisory review state is independent of the completion rule being satisfied");

        // Odd data: a succeeded task whose order record is gone. The history counts it, so this class does too (documented difference).
        var odd = new World(); odd.Resolve(3, true, withCompletedOrder: false); odd.Ack(3);
        Check(ScenarioInterventionHistory.Build(odd.Events, odd.Reports, odd.Snapshots, odd.Deadwood, odd.Orders).Count == 1 && Derive(odd, 5).ResolvedInterventionCount == 1
            && !ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(odd.Events, odd.Orders),
            "odd data: follows the history's definition, not the completion rule's plan parser");
    }

    // ---- 12b. no new save field, no Unity dependency, no hidden state ----
    private static void NoSaveFieldNoUnity()
    {
        string root = RepoRoot();
        string source = File.ReadAllText(Environment.GetEnvironmentVariable("CCF_CYCLE_SOURCE") ?? Path.Combine(root, "Assets", "ForestPrototype", "ScenarioOne", "ScenarioCycleState.cs"));
        var usings = Regex.Matches(source, @"^using\s+([\w\.]+);", RegexOptions.Multiline).Cast<Match>().Select(m => m.Groups[1].Value).ToList();
        Check(usings.All(u => u == "System" || u.StartsWith("System.")) && !source.Contains("UnityEngine"), "the implementation has no UnityEngine reference", string.Join(",", usings));
        Check(!Regex.IsMatch(source, @"PlayerPrefs|JsonUtility|\[Serializable\]|\[SerializeField\]|ScriptableObject|MonoBehaviour|DateTime|Random"), "no persistence, serialization, clock or randomness API");
        var staticState = source.Split('\n').Where(l => Regex.IsMatch(l, @"\bstatic\b") && l.TrimEnd().EndsWith(";") && !l.Contains("(") && !Regex.IsMatch(l, @"\b(const|readonly)\b")).ToList();
        Check(staticState.Count == 0, "no mutable static field (no cache, latch or hidden state)", string.Join(" ; ", staticState));
        Check(typeof(ScenarioCycleState).GetCustomAttributes(typeof(SerializableAttribute), false).Length == 0
            && typeof(CycleComponent).GetCustomAttributes(typeof(SerializableAttribute), false).Length == 0, "the result types are not [Serializable]");
        string save = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ForestSaveData.cs"));
        Check(!save.Contains("CycleState") && !save.Contains("CycleComponent") && !save.Contains("CycleReview"), "ForestSaveData carries no cycle-state field");
        string access = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ScenarioOne", "ScenarioOneManager.History.cs"));
        Check(Regex.IsMatch(access, @"public ScenarioCycleState CycleState =>\s+ScenarioCycleState\.Derive\(Interventions, managementEvents, workOrders, CurrentYear, outcome, outcomeYear, ReviewYear\);"),
            "the manager exposes a read-only expression-bodied accessor over the existing records and Interventions");
        Check(!Regex.IsMatch(access, @"CycleState\s*\{"), "the accessor has no setter or backing field");
    }

    // ---- every reason, in every state this suite produced, explains and does not prescribe ----
    private static void EveryReasonIsExplanatoryAndNonPrescriptive()
    {
        var banned = new Regex(@"\b(should|must|recommend\w*|overdue|ready|time to|you need|need to|intervene|required|requirement|ideal|optimal|every \d|at least \d|due)\b", RegexOptions.IgnoreCase);
        var texts = AllStates.SelectMany(s => s.Components.Select(c => c.Reason).Concat(new[] { s.OutcomeNote })).ToList();
        Check(AllStates.Count > 30 && texts.All(t => !string.IsNullOrWhiteSpace(t) && t.Length >= 20), "every component and outcome note in every produced state has a real explanation", $"{AllStates.Count} states");
        var offenders = texts.Where(t => banned.IsMatch(t)).Distinct().ToList();
        Check(offenders.Count == 0, "no reason instructs, schedules or recommends", string.Join(" ; ", offenders));
        Check(ScenarioCycleState.AdvisoryLabel.Contains("not a schedule") && ScenarioCycleState.AdvisoryLabel.Contains("for your own judgement"), "the advisory label says what this is not");
    }

    // ---- helpers ----
    private static string RepoRoot() => Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? throw new InvalidOperationException("CCF_REPO_ROOT not set");

    // Stable text of every public field, recursively, so two object graphs can be compared and a mutation shows up.
    private static string Dump(object value)
    {
        var sb = new StringBuilder();
        DumpInto(sb, value);
        return sb.ToString();
    }

    private static void DumpInto(StringBuilder sb, object value)
    {
        if (value == null) { sb.Append("null;"); return; }
        Type type = value.GetType();
        if (type.IsPrimitive || type.IsEnum || value is string || value is decimal)
        { sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture)).Append(';'); return; }
        if (value is IEnumerable sequence)
        {
            sb.Append('[');
            foreach (object item in sequence) DumpInto(sb, item);
            sb.Append("];");
            return;
        }
        sb.Append(type.Name).Append('{');
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance).OrderBy(f => f.Name, StringComparer.Ordinal))
        { sb.Append(field.Name).Append('='); DumpInto(sb, field.GetValue(value)); }
        sb.Append("};");
    }

    private static object DeepCopy(object value)
    {
        if (value == null) return null;
        Type type = value.GetType();
        if (type.IsPrimitive || type.IsEnum || value is string || value is decimal) return value;
        if (value is Array array)
        {
            Array arrayCopy = Array.CreateInstance(type.GetElementType(), array.Length);
            for (int i = 0; i < array.Length; i++) arrayCopy.SetValue(DeepCopy(array.GetValue(i)), i);
            return arrayCopy;
        }
        if (value is IList list && type.IsGenericType)
        {
            var copy = (IList)Activator.CreateInstance(type);
            foreach (object item in list) copy.Add(DeepCopy(item));
            return copy;
        }
        object clone = Activator.CreateInstance(type);
        foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
            field.SetValue(clone, DeepCopy(field.GetValue(value)));
        return clone;
    }
}
