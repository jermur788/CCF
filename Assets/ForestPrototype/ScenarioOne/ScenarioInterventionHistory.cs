using System;
using System.Collections.Generic;
using System.Linq;

// Scenario One management history (S1-C): everything here is DERIVED, on demand, from records the save already
// carries (management events, annual reports, ecological snapshots, deadwood records and resolved work orders).
// Nothing in this file is stored. Because it is a pure function of persisted records, history is identical after a
// save and load by construction, cannot be duplicated, and needs no schema change.
//
// An "intervention" is a RESOLVED WORK PLAN: a year in which at least one commissioned task succeeded. It can mix
// felling, pruning, planting, protection and other work. The intervention ordinal here is a presentation count and
// is deliberately separate from the economy's FirstThinning / SecondThinning / LaterThinning tiers, which count
// felling years only and are not changed.
//
// Year convention (see ScenarioOneManager.AdvanceYear): a plan approved while the stand is at year Y-1 is executed by
// the advance into year Y. Its events and annual report carry year Y; the "before" snapshot is the one recorded at
// year Y-1, the "after" snapshot at year Y (work done, then one ecological step).
public sealed class InterventionSummary
{
    public int Ordinal;                 // 1-based among resolved plans
    public int Year;                    // year the plan was resolved (advance into this year)

    public int Felled;
    public int Salvaged;
    public int Pruned;
    public int Planted;
    public int PlantedSheltered;
    public int RegenerationRemoved;
    public int TasksFailed;
    public readonly List<KeyValuePair<string, int>> PlantedBySpecies = new List<KeyValuePair<string, int>>();
    public readonly List<string> FailureReasons = new List<string>();

    public long CostCents;              // contractor work plus shelter/material charged in that year
    public long RevenueCents;           // timber sold in that year
    public long NetCents => RevenueCents - CostCents;
    public long MinimumJobAdjustmentCents;
    public int OwnerMinutes;
    public long ClosingCashCents;
    public readonly List<ScenarioTimberSale> Sales = new List<ScenarioTimberSale>();

    public ScenarioEcologicalSnapshot Before;   // state when the plan was made (may be null)
    public ScenarioEcologicalSnapshot After;    // state after the work and one ecological step (may be null)
    public ScenarioEcologicalSnapshot Latest;   // most recent state, only when later than After (else null)

    public int OtherDeathsInYear;               // trees that died that year and were not felled by the plan
    public int OtherDeathsSince;                // the same, in every later year up to Latest

    public int TotalTasks => Felled + Salvaged + Pruned + Planted + RegenerationRemoved;
    public int YearsObserved => Latest != null ? Latest.year - Year : 0;
}

public sealed class StaleOrderNote
{
    public ScenarioOneWorkOrder Order;
    public int AgeYears;
    public string Reason = "";
}

// What a finished scenario looked like, from the same records (completion year, not necessarily year 100).
public sealed class CompletionSummary
{
    public int Year;
    public ScenarioEcologicalSnapshot Baseline;      // first recorded state
    public ScenarioEcologicalSnapshot AtCompletion;  // state in the completion year
    public readonly List<InterventionSummary> Interventions = new List<InterventionSummary>();
    public long StartingCashCents;
    public long ClosingCashCents;
    public long MinimumCashCents;
    public long TotalRevenueCents;
    public long TotalCostCents;
    public int TotalFelled, TotalPlanted, TotalPruned, TotalOtherDeaths;
    public long NetCents => TotalRevenueCents - TotalCostCents;
}

public static class ScenarioInterventionHistory
{
    // [C] An open order planned this many years ago, or more, is surfaced for a fresh look even if it still validates.
    public const int StaleAfterYears = 2;

    public static List<InterventionSummary> Build(
        IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioAnnualReport> reports,
        IReadOnlyList<ScenarioEcologicalSnapshot> snapshots,
        IReadOnlyList<ScenarioDeadwoodRecord> deadwood,
        IReadOnlyList<ScenarioOneWorkOrder> orders)
    {
        var result = new List<InterventionSummary>();
        var resolvedByYear = events.Where(e => e != null && e.eventType == ScenarioManagementEventType.WorkResolved)
            .GroupBy(e => e.year).OrderBy(g => g.Key).ToList();
        ScenarioEcologicalSnapshot latest = snapshots.Count > 0 ? snapshots[snapshots.Count - 1] : null;
        var orderById = new Dictionary<int, ScenarioOneWorkOrder>();
        foreach (ScenarioOneWorkOrder order in orders)
            if (order != null && !orderById.ContainsKey(order.workOrderId)) orderById.Add(order.workOrderId, order);
        Dictionary<int, HashSet<string>> felledByYear = FelledTargetsByYear(events);

        foreach (var yearGroup in resolvedByYear)
        {
            int year = yearGroup.Key;
            var done = yearGroup.Where(e => e.outcome == ScenarioManagementOutcome.Succeeded).OrderBy(e => e.eventId).ToList();
            if (done.Count == 0) continue;   // every task failed: not an intervention (the annual review still reports it)
            var summary = new InterventionSummary { Ordinal = result.Count + 1, Year = year };

            summary.Felled = done.Count(e => e.taskType == ScenarioWorkType.FellTree);
            summary.Salvaged = done.Count(e => e.taskType == ScenarioWorkType.SalvageDeadwood);
            summary.Pruned = done.Count(e => e.taskType == ScenarioWorkType.PruneTree);
            summary.RegenerationRemoved = done.Count(e => e.taskType == ScenarioWorkType.RemoveRegeneration);
            var planted = done.Where(e => e.taskType == ScenarioWorkType.PlantJuvenile).ToList();
            summary.Planted = planted.Count;
            summary.PlantedSheltered = planted.Count(e => orderById.TryGetValue(e.workOrderId, out ScenarioOneWorkOrder o) && o.installShelter);
            foreach (var species in planted.GroupBy(e => e.speciesId ?? "").OrderBy(g => g.Key, StringComparer.Ordinal))
                summary.PlantedBySpecies.Add(new KeyValuePair<string, int>(species.Key, species.Count()));

            var failed = yearGroup.Where(e => e.outcome != ScenarioManagementOutcome.Succeeded).OrderBy(e => e.eventId).ToList();
            summary.TasksFailed = failed.Count;
            foreach (string reason in failed.Select(e => e.failureReason).Where(r => !string.IsNullOrEmpty(r)).Distinct().Take(3))
                summary.FailureReasons.Add(reason);

            ScenarioAnnualReport report = reports.FirstOrDefault(r => r != null && r.year == year);
            if (report != null)
            {
                summary.CostCents = report.contractorCostCents + done.Sum(e => e.stockCostCents);
                summary.RevenueCents = report.timberRevenueCents;
                summary.MinimumJobAdjustmentCents = report.harvestMinimumAdjustmentCents;
                summary.OwnerMinutes = report.ownerMinutes;
                summary.ClosingCashCents = report.closingCashCents;
                if (report.timberSales != null)
                    foreach (ScenarioTimberSale sale in report.timberSales)
                        summary.Sales.Add(new ScenarioTimberSale { assortment = sale.assortment, soldVolumeCm3 = sale.soldVolumeCm3, revenueCents = sale.revenueCents });
            }

            summary.Before = snapshots.LastOrDefault(s => s != null && s.year < year);
            summary.After = snapshots.FirstOrDefault(s => s != null && s.year == year) ?? snapshots.FirstOrDefault(s => s != null && s.year > year);
            summary.Latest = latest != null && summary.After != null && latest.year > summary.After.year ? latest : null;

            summary.OtherDeathsInYear = OtherDeaths(deadwood, felledByYear, year, year);
            summary.OtherDeathsSince = latest != null && latest.year > year ? OtherDeaths(deadwood, felledByYear, year + 1, latest.year) : 0;
            result.Add(summary);
        }
        return result;
    }

    // Deadwood records stamped with a fall year in [fromYear, toYear] that were not trees the plan of that year felled
    // (felling that leaves a stem as deadwood stamps the resolution year too). Includes self-thinning, windthrow and any
    // other biological death; the persisted records do not say which.
    private static int OtherDeaths(IReadOnlyList<ScenarioDeadwoodRecord> deadwood, Dictionary<int, HashSet<string>> felledByYear, int fromYear, int toYear)
    {
        int count = 0;
        foreach (ScenarioDeadwoodRecord record in deadwood)
        {
            if (record == null || record.fallenYear < fromYear || record.fallenYear > toYear) continue;
            if (felledByYear.TryGetValue(record.fallenYear, out HashSet<string> felled) && felled.Contains(record.treeId ?? "")) continue;
            count++;
        }
        return count;
    }

    private static Dictionary<int, HashSet<string>> FelledTargetsByYear(IReadOnlyList<ScenarioManagementEvent> events)
    {
        var map = new Dictionary<int, HashSet<string>>();
        foreach (ScenarioManagementEvent e in events)
        {
            if (e == null || e.eventType != ScenarioManagementEventType.WorkResolved || e.outcome != ScenarioManagementOutcome.Succeeded
                || e.taskType != ScenarioWorkType.FellTree) continue;
            if (!map.TryGetValue(e.year, out HashSet<string> set)) map[e.year] = set = new HashSet<string>(StringComparer.Ordinal);
            set.Add(e.targetTreeId ?? "");
        }
        return map;
    }

    // Open orders that deserve a fresh look. Nothing is expired or deleted here; the caller only surfaces them.
    public static List<StaleOrderNote> StaleOrders(IReadOnlyList<ScenarioOneWorkOrder> orders, int currentYear)
    {
        var notes = new List<StaleOrderNote>();
        foreach (ScenarioOneWorkOrder order in orders.Where(o => o != null && o.IsOpen).OrderBy(o => o.workOrderId))
        {
            int age = Math.Max(0, currentYear - order.createdYear);
            if (!string.IsNullOrEmpty(order.validationMessage))
                notes.Add(new StaleOrderNote { Order = order, AgeYears = age, Reason = order.validationMessage });
            else if (age >= StaleAfterYears)
                notes.Add(new StaleOrderNote { Order = order, AgeYears = age,
                    Reason = $"Planned in year {order.createdYear}, {age} years ago. Check it still suits the stand before approving or advancing." });
        }
        return notes;
    }

    public static CompletionSummary Completion(
        int completionYear, long startingCashCents,
        IReadOnlyList<ScenarioManagementEvent> events, IReadOnlyList<ScenarioAnnualReport> reports,
        IReadOnlyList<ScenarioEcologicalSnapshot> snapshots, IReadOnlyList<ScenarioDeadwoodRecord> deadwood, IReadOnlyList<ScenarioOneWorkOrder> orders)
    {
        var summary = new CompletionSummary { Year = completionYear, StartingCashCents = startingCashCents };
        summary.Baseline = snapshots.FirstOrDefault(s => s != null);
        summary.AtCompletion = snapshots.FirstOrDefault(s => s != null && s.year == completionYear) ?? snapshots.LastOrDefault(s => s != null && s.year <= completionYear);
        foreach (InterventionSummary intervention in Build(events, reports, snapshots, deadwood, orders).Where(i => i.Year <= completionYear))
        {
            summary.Interventions.Add(intervention);
            summary.TotalFelled += intervention.Felled;
            summary.TotalPlanted += intervention.Planted;
            summary.TotalPruned += intervention.Pruned;
        }
        var upTo = reports.Where(r => r != null && r.year <= completionYear).OrderBy(r => r.year).ToList();
        summary.TotalRevenueCents = upTo.Sum(r => r.timberRevenueCents);
        summary.TotalCostCents = upTo.Sum(r => r.contractorCostCents)
            + events.Where(e => e != null && e.eventType == ScenarioManagementEventType.WorkResolved && e.outcome == ScenarioManagementOutcome.Succeeded && e.year <= completionYear)
                .Sum(e => e.stockCostCents);
        summary.ClosingCashCents = upTo.Count > 0 ? upTo[upTo.Count - 1].closingCashCents : startingCashCents;
        summary.MinimumCashCents = upTo.Count > 0 ? Math.Min(startingCashCents, upTo.Min(r => r.closingCashCents)) : startingCashCents;
        summary.TotalOtherDeaths = OtherDeaths(deadwood, FelledTargetsByYear(events), 1, completionYear);
        return summary;
    }

    // The one line that says what to do next, from the derived history alone. The tutorial steps before the first
    // review keep their own wording in ScenarioOneManager; this is the repeatable part for every later cycle.
    // completionGuidance is the optional "a reviewed second cycle is still missing" line (ScenarioOneCompletionGuidance.GuidanceText).
    // Completion and stale orders keep their priority; null or empty leaves every older result unchanged.
    public static string NextStepHint(IReadOnlyList<InterventionSummary> interventions, int currentYear, int staleOrderCount,
        bool completed, int completionYear, int centuryYear, string completionGuidance = null)
    {
        if (completed)
            return $"Objectives met in year {completionYear}. The completion summary is in the Annual Review. You can keep managing toward the Century Review (year {centuryYear}).";
        if (staleOrderCount > 0)
            return $"{staleOrderCount} planned order{(staleOrderCount == 1 ? "" : "s")} from earlier need{(staleOrderCount == 1 ? "s" : "")} attention in the Work Plan.";
        if (!string.IsNullOrEmpty(completionGuidance))
            return completionGuidance;
        if (interventions.Count == 0)
            return "No intervention yet. Inspect trees, mark them with X, import the marks into the Work Plan, approve and advance.";
        InterventionSummary last = interventions[interventions.Count - 1];
        int since = currentYear - last.Year;
        if (since <= 1)
            return $"Intervention {last.Ordinal} was carried out in year {last.Year}. Walk the forest to see how it is responding, then compare with your earlier decisions in the Annual Review history.";
        return $"{since} years since intervention {last.Ordinal}. Inspect the stand, read the history in the Annual Review, and plan intervention {last.Ordinal + 1} if the forest needs it.";
    }
}
