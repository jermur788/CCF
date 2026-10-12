// Offline check for the derived Scenario One management history (S1-C). Compiles the PRODUCTION
// ScenarioInterventionHistory.cs and the real persisted record classes (management events, annual reports, ecological
// snapshots, deadwood records, work orders) against the Unity reference assemblies, and runs on the plain .NET SDK.
// The Unity-side manager and UI that call it are exercised by ScenarioHistoryVerification.cs (type-checked only).
//
// Usage: run_offline_check.py --suite history   Prints THREE_SPECIES_HISTORY_* tokens.
using System;
using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using UnityEngine;

public static class ScenarioHistoryOfflineCheck
{
    private static int failures, checks;

    public static int Main()
    {
        Story story = Story.Build();
        Interventions(story);
        EmptyAndEdgeCases(story);
        Mortality(story);
        Money(story);
        Ecology(story);
        Derivation(story);
        StaleOrdersCheck(story);
        Completion(story);
        Hints(story);
        Wording(story);
        Console.WriteLine(failures == 0 ? $"THREE_SPECIES_HISTORY_PASS checks={checks}" : $"THREE_SPECIES_HISTORY_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // A twenty-five-year story shaped like the repeated-intervention harness (plans resolved in years 1, 6, 16, 25),
    // with idle years between, one year in which every task failed, a retained-as-deadwood felling, natural deaths and
    // a stale open order. Built the way ScenarioOneManager records things.
    private sealed class Story
    {
        public readonly List<ScenarioManagementEvent> Events = new List<ScenarioManagementEvent>();
        public readonly List<ScenarioAnnualReport> Reports = new List<ScenarioAnnualReport>();
        public readonly List<ScenarioEcologicalSnapshot> Snapshots = new List<ScenarioEcologicalSnapshot>();
        public readonly List<ScenarioDeadwoodRecord> Deadwood = new List<ScenarioDeadwoodRecord>();
        public readonly List<ScenarioOneWorkOrder> Orders = new List<ScenarioOneWorkOrder>();
        public const long StartingCash = 1200000;
        private int nextEvent = 1, nextOrder = 1, nextDeadwood = 1;

        public static Story Build()
        {
            var s = new Story();
            s.Snapshot(0, 336, 40f, 0.55f, 0.20f, 0, 0f, sitka: 336, oak: 0, beech: 0);
            long cash = StartingCash;
            for (int year = 1; year <= 25; year++)
            {
                var report = new ScenarioAnnualReport { year = year };
                if (year == 1)
                {   // intervention 1: thin 30 (sell 25, leave 5 as deadwood), prune 4, plant 6 (4 sheltered)
                    for (int i = 0; i < 30; i++) s.Resolve(year, ScenarioWorkType.FellTree, "P" + i.ToString("000"), "sitka-spruce", cost: 0);
                    for (int i = 0; i < 5; i++) s.Deadwood.Add(s.Wood("P" + i.ToString("000"), year));
                    for (int i = 0; i < 4; i++) s.Resolve(year, ScenarioWorkType.PruneTree, "P" + (100 + i).ToString("000"), "sitka-spruce");
                    for (int i = 0; i < 6; i++) s.Resolve(year, ScenarioWorkType.PlantJuvenile, "", i < 3 ? "sessile-oak" : "beech", shelter: i < 4, stockCost: 500);
                    report.contractorCostCents = 250000 + 4 * 1800 + 6 * 900; report.timberRevenueCents = 180000; report.harvestMinimumAdjustmentCents = 60000; report.ownerMinutes = 20;
                    report.timberSales.Add(new ScenarioTimberSale { assortment = TimberAssortment.Sawlog, soldVolumeCm3 = 9000000, revenueCents = 150000 });
                    report.timberSales.Add(new ScenarioTimberSale { assortment = TimberAssortment.Pulp, soldVolumeCm3 = 4000000, revenueCents = 30000 });
                }
                if (year == 3)
                {   // every task fails: not an intervention
                    s.Resolve(year, ScenarioWorkType.FellTree, "GONE1", "sitka-spruce", succeeded: false, failure: "Target tree is missing or already felled.");
                    s.Resolve(year, ScenarioWorkType.PlantJuvenile, "", "beech", succeeded: false, failure: "Planting cell is outside the stand.");
                }
                if (year == 6)
                {   // intervention 2: prune only (a plan need not fell)
                    for (int i = 0; i < 4; i++) s.Resolve(year, ScenarioWorkType.PruneTree, "P" + (100 + i).ToString("000"), "sitka-spruce");
                    report.contractorCostCents = 4 * 2200;
                }
                if (year == 16)
                {   // intervention 3: thin 12, clear vegetation in 3 cells, plant 2 oak, one task fails
                    for (int i = 0; i < 12; i++) s.Resolve(year, ScenarioWorkType.FellTree, "P" + (200 + i).ToString("000"), "sitka-spruce");
                    for (int i = 0; i < 3; i++) s.Resolve(year, ScenarioWorkType.RemoveRegeneration, "", "", cell: i);
                    for (int i = 0; i < 2; i++) s.Resolve(year, ScenarioWorkType.PlantJuvenile, "", "sessile-oak", stockCost: 500);
                    s.Resolve(year, ScenarioWorkType.PruneTree, "P999", "sitka-spruce", succeeded: false, failure: "Target crown base must exceed the current pruned height.");
                    report.contractorCostCents = 250000 + 3 * 1500 + 2 * 900; report.timberRevenueCents = 90000;
                    report.timberSales.Add(new ScenarioTimberSale { assortment = TimberAssortment.Sawlog, soldVolumeCm3 = 5000000, revenueCents = 90000 });
                }
                if (year == 25)
                {   // intervention 4
                    for (int i = 0; i < 8; i++) s.Resolve(year, ScenarioWorkType.FellTree, "P" + (300 + i).ToString("000"), "sitka-spruce");
                    report.contractorCostCents = 250000; report.timberRevenueCents = 70000;
                    report.timberSales.Add(new ScenarioTimberSale { assortment = TimberAssortment.Sawlog, soldVolumeCm3 = 3000000, revenueCents = 70000 });
                }
                // natural deaths (self-thinning) stamped with the year: years 2, 6, 7, 20
                int natural = year == 2 ? 3 : year == 6 ? 2 : year == 7 ? 4 : year == 20 ? 5 : 0;
                for (int i = 0; i < natural; i++) s.Deadwood.Add(s.Wood("N" + year + "-" + i, year));
                cash += report.timberRevenueCents - report.contractorCostCents - (year == 1 ? 6 * 500 : year == 16 ? 2 * 500 : 0);
                report.closingCashCents = cash;
                s.Reports.Add(report);
                s.Events.Add(new ScenarioManagementEvent { eventId = s.nextEvent++, year = year, eventType = ScenarioManagementEventType.YearAdvanced, outcome = ScenarioManagementOutcome.Succeeded });
                int oak = (year >= 1 ? 3 : 0) + (year >= 16 ? 2 : 0), beech = year >= 1 ? 3 : 0;
                s.Snapshot(year, 336 - year * 7 + oak + beech, 40f - year * 0.5f, 0.55f - year * 0.01f + (year >= 16 ? 0.05f : 0f), 0.20f + year * 0.005f,
                    year * 2, year * 0.3f, sitka: 336 - year * 7, oak: oak, beech: beech);
            }
            // an order planned long ago and never resolved (stale), one with a problem, one fresh
            s.Orders.Add(new ScenarioOneWorkOrder { workOrderId = 900, type = ScenarioWorkType.FellTree, status = ScenarioWorkStatus.Pending, targetTreeId = "P777", createdYear = 18 });
            s.Orders.Add(new ScenarioOneWorkOrder { workOrderId = 901, type = ScenarioWorkType.PlantJuvenile, status = ScenarioWorkStatus.Approved, createdYear = 24, validationMessage = "Planting stock or species is unavailable." });
            s.Orders.Add(new ScenarioOneWorkOrder { workOrderId = 902, type = ScenarioWorkType.PruneTree, status = ScenarioWorkStatus.Pending, createdYear = 25 });
            s.Orders.Add(new ScenarioOneWorkOrder { workOrderId = 903, type = ScenarioWorkType.FellTree, status = ScenarioWorkStatus.Completed, createdYear = 1, resolvedYear = 1 });
            return s;
        }

        private ScenarioDeadwoodRecord Wood(string treeId, int year) => new ScenarioDeadwoodRecord { deadwoodId = "DW" + nextDeadwood++.ToString("0000"), treeId = treeId, speciesId = "sitka-spruce", fallenYear = year, originalVolumeM3 = 0.3f };

        private void Resolve(int year, ScenarioWorkType type, string tree, string species, bool succeeded = true, string failure = "", bool shelter = false, long stockCost = 0, int cell = -1, long cost = 0)
        {
            int orderId = nextOrder++;
            Orders.Add(new ScenarioOneWorkOrder { workOrderId = orderId, type = type, status = succeeded ? ScenarioWorkStatus.Completed : ScenarioWorkStatus.Failed, targetTreeId = tree, speciesId = species,
                installShelter = shelter, createdYear = year - 1, resolvedYear = year, cellIndex = cell });
            Events.Add(new ScenarioManagementEvent { eventId = nextEvent++, year = year, eventType = ScenarioManagementEventType.WorkResolved,
                outcome = succeeded ? ScenarioManagementOutcome.Succeeded : ScenarioManagementOutcome.Failed, workOrderId = orderId, taskType = type,
                targetTreeId = tree, speciesId = species, stockCostCents = succeeded ? stockCost : 0, failureReason = failure });
        }

        private void Snapshot(int year, int living, float basalArea, float canopy, float light, int regenCells, float deadwoodM3, int sitka, int oak, int beech)
        {
            var snapshot = new ScenarioEcologicalSnapshot { year = year, livingTrees = living, basalAreaM2PerHa = basalArea, meanCanopy = canopy, meanLight = light,
                occupiedRegenerationCells = regenCells, deadwoodVolumeM3 = deadwoodM3, deadwoodCount = year };
            snapshot.species.Add(new ScenarioSpeciesOutcome { speciesId = "sitka-spruce", livingTrees = sitka });
            snapshot.species.Add(new ScenarioSpeciesOutcome { speciesId = "sessile-oak", livingTrees = oak });
            snapshot.species.Add(new ScenarioSpeciesOutcome { speciesId = "beech", livingTrees = beech });
            Snapshots.Add(snapshot);
        }
    }

    private static List<InterventionSummary> BuildAll(Story s) => ScenarioInterventionHistory.Build(s.Events, s.Reports, s.Snapshots, s.Deadwood, s.Orders);

    // ---------- interventions ----------

    private static void Interventions(Story s)
    {
        List<InterventionSummary> list = BuildAll(s);
        Check(list.Select(i => i.Year).SequenceEqual(new[] { 1, 6, 16, 25 }), "FOUR_INTERVENTIONS_IN_THE_RESOLVED_YEARS", string.Join(",", list.Select(i => i.Year)));
        Check(list.Select(i => i.Ordinal).SequenceEqual(new[] { 1, 2, 3, 4 }), "ORDINALS_ARE_ONE_TO_FOUR_WITH_NO_TWO_CYCLE_SPECIAL_CASE");
        Check(!list.Any(i => i.Year == 3), "A_YEAR_IN_WHICH_EVERY_TASK_FAILED_IS_NOT_AN_INTERVENTION");
        InterventionSummary a = list[0], b = list[1], c = list[2], d = list[3];
        Check(a.Felled == 30 && a.Pruned == 4 && a.Planted == 6 && a.PlantedSheltered == 4 && a.RegenerationRemoved == 0 && a.TasksFailed == 0, "FIRST_INTERVENTION_WORK_COUNTS",
            $"felled={a.Felled} pruned={a.Pruned} planted={a.Planted} sheltered={a.PlantedSheltered}");
        Check(a.PlantedBySpecies.Select(p => $"{p.Key}={p.Value}").SequenceEqual(new[] { "beech=3", "sessile-oak=3" }), "PLANTED_BY_SPECIES_IS_DETERMINISTICALLY_ORDERED", string.Join(",", a.PlantedBySpecies.Select(p => $"{p.Key}={p.Value}")));
        Check(b.Felled == 0 && b.Pruned == 4 && b.TotalTasks == 4, "A_PLAN_NEED_NOT_CONTAIN_FELLING", $"tasks={b.TotalTasks}");
        Check(c.Felled == 12 && c.RegenerationRemoved == 3 && c.Planted == 2 && c.TasksFailed == 1 && c.FailureReasons.Count == 1 && c.FailureReasons[0].Contains("crown base"), "MIXED_PLAN_COUNTS_AND_FAILURES",
            $"felled={c.Felled} cleared={c.RegenerationRemoved} planted={c.Planted} failed={c.TasksFailed}");
        Check(d.Felled == 8 && d.TotalTasks == 8, "FOURTH_CYCLE_IS_BUILT_THE_SAME_WAY");
        Check(list.All(i => i.Year == i.After.year), "AFTER_SNAPSHOT_IS_THE_PLANS_OWN_YEAR");
        Check(list.All(i => i.Before != null && i.Before.year == i.Year - 1), "BEFORE_SNAPSHOT_IS_THE_YEAR_BEFORE", string.Join(",", list.Select(i => i.Before.year)));
    }

    private static void EmptyAndEdgeCases(Story s)
    {
        var none = ScenarioInterventionHistory.Build(new List<ScenarioManagementEvent>(), new List<ScenarioAnnualReport>(), new List<ScenarioEcologicalSnapshot>(), new List<ScenarioDeadwoodRecord>(), new List<ScenarioOneWorkOrder>());
        Check(none.Count == 0, "NO_RECORDS_NO_INTERVENTIONS");
        // Events with no report or snapshot (a trimmed or partial save) still summarise the work without throwing.
        var partial = ScenarioInterventionHistory.Build(s.Events.Where(e => e.year == 6).ToList(), new List<ScenarioAnnualReport>(), new List<ScenarioEcologicalSnapshot>(), new List<ScenarioDeadwoodRecord>(), s.Orders);
        Check(partial.Count == 1 && partial[0].Pruned == 4 && partial[0].CostCents == 0 && partial[0].Before == null && partial[0].After == null && partial[0].Latest == null, "PARTIAL_RECORDS_DEGRADE_WITHOUT_ERROR");
        // Events listed out of order, or twice as a restored save might list them, must not change the ordinals' meaning.
        var shuffled = s.Events.OrderByDescending(e => e.eventId).ToList();
        var fromShuffled = ScenarioInterventionHistory.Build(shuffled, s.Reports, s.Snapshots, s.Deadwood, s.Orders);
        Check(fromShuffled.Select(i => (i.Ordinal, i.Year, i.Felled, i.Pruned, i.Planted)).SequenceEqual(BuildAll(s).Select(i => (i.Ordinal, i.Year, i.Felled, i.Pruned, i.Planted))), "EVENT_ORDER_DOES_NOT_CHANGE_THE_RESULT");
    }

    // ---------- mortality, money, ecology ----------

    private static void Mortality(Story s)
    {
        List<InterventionSummary> list = BuildAll(s);
        // Year 1: 5 felled-and-left-as-deadwood records exist but are the plan's own trees, so they are not 'other deaths'.
        Check(list[0].OtherDeathsInYear == 0, "FELLED_TREES_LEFT_AS_DEADWOOD_ARE_NOT_COUNTED_AS_OTHER_DEATHS", $"year1 other deaths={list[0].OtherDeathsInYear}");
        Check(list[1].OtherDeathsInYear == 2, "NATURAL_DEATHS_IN_THE_PLANS_YEAR", $"year6={list[1].OtherDeathsInYear}");
        Check(list[0].OtherDeathsSince == 3 + 2 + 4 + 5, "NATURAL_DEATHS_SINCE_THE_PLAN_SUM_LATER_YEARS", $"since year 1={list[0].OtherDeathsSince}");
        Check(list[1].OtherDeathsSince == 4 + 5, "NATURAL_DEATHS_SINCE_EXCLUDE_EARLIER_YEARS", $"since year 6={list[1].OtherDeathsSince}");
        Check(list[3].OtherDeathsSince == 0 && list[3].Latest == null, "NOTHING_SINCE_FOR_THE_LATEST_PLAN");
    }

    private static void Money(Story s)
    {
        List<InterventionSummary> list = BuildAll(s);
        InterventionSummary a = list[0];
        Check(a.RevenueCents == 180000 && a.CostCents == 250000 + 4 * 1800 + 6 * 900 + 6 * 500, "COST_INCLUDES_CONTRACTOR_WORK_AND_MATERIALS", $"cost={a.CostCents} revenue={a.RevenueCents}");
        Check(a.NetCents == a.RevenueCents - a.CostCents && a.MinimumJobAdjustmentCents == 60000 && a.OwnerMinutes == 20, "NET_AND_MINIMUM_JOB_ADJUSTMENT_ARE_REPORTED");
        Check(a.Sales.Count == 2 && a.Sales.Sum(x => x.revenueCents) == a.RevenueCents, "SALES_BY_PRODUCT_RECONCILE_WITH_REVENUE");
        Check(a.Sales[0] != s.Reports[0].timberSales[0], "SALES_ARE_COPIES_NOT_THE_PERSISTED_OBJECTS");
        Check(list[1].RevenueCents == 0 && list[1].CostCents == 4 * 2200 && list[1].NetCents < 0, "PRUNE_ONLY_PLAN_HAS_COST_AND_NO_REVENUE");
        Check(list.All(i => i.ClosingCashCents == s.Reports.First(r => r.year == i.Year).closingCashCents), "CLOSING_CASH_COMES_FROM_THE_ANNUAL_REPORT");
    }

    private static void Ecology(Story s)
    {
        List<InterventionSummary> list = BuildAll(s);
        InterventionSummary a = list[0];
        Check(a.Before.year == 0 && a.After.year == 1 && a.Latest.year == 25 && a.YearsObserved == 24, "BEFORE_AFTER_AND_LATEST_ARE_THE_RIGHT_YEARS", $"before={a.Before.year} after={a.After.year} latest={a.Latest.year}");
        Check(a.After.livingTrees < a.Before.livingTrees + 1 && a.Latest.occupiedRegenerationCells > a.After.occupiedRegenerationCells, "DELTAS_CAN_BE_READ_FROM_THE_SNAPSHOT_PAIR");
        Check(a.Latest.species.First(x => x.speciesId == "sessile-oak").livingTrees > a.After.species.First(x => x.speciesId == "sessile-oak").livingTrees, "SPECIES_COMPOSITION_CHANGE_IS_AVAILABLE");
        Check(list[2].Before.year == 15 && list[2].After.year == 16 && list[2].Latest.year == 25, "THIRD_PLAN_USES_ITS_OWN_SNAPSHOTS");
    }

    private static void Derivation(Story s)
    {
        string Fingerprint(List<InterventionSummary> l) => string.Join("|", l.Select(i => $"{i.Ordinal}:{i.Year}:{i.Felled}:{i.Pruned}:{i.Planted}:{i.PlantedSheltered}:{i.RegenerationRemoved}:{i.TasksFailed}:{i.CostCents}:{i.RevenueCents}:{i.OtherDeathsInYear}:{i.OtherDeathsSince}:{i.Before?.year}:{i.After?.year}:{i.Latest?.year}"));
        string first = Fingerprint(BuildAll(s)), second = Fingerprint(BuildAll(s));
        Check(first == second, "BUILD_IS_DETERMINISTIC_AND_REPEATABLE_NO_DUPLICATES", first.Length + " chars");
        // Rebuilding from a deep copy of every persisted record (what a save and load produces) gives the same history.
        var copyEvents = s.Events.Select(e => JsonUtility.FromJson<ScenarioManagementEvent>(JsonUtility.ToJson(e))).ToList();
        var copyReports = s.Reports.Select(r => JsonUtility.FromJson<ScenarioAnnualReport>(JsonUtility.ToJson(r))).ToList();
        var copySnapshots = s.Snapshots.Select(x => JsonUtility.FromJson<ScenarioEcologicalSnapshot>(JsonUtility.ToJson(x))).ToList();
        var copyDeadwood = s.Deadwood.Select(x => JsonUtility.FromJson<ScenarioDeadwoodRecord>(JsonUtility.ToJson(x))).ToList();
        var copyOrders = s.Orders.Select(x => JsonUtility.FromJson<ScenarioOneWorkOrder>(JsonUtility.ToJson(x))).ToList();
        Check(Fingerprint(ScenarioInterventionHistory.Build(copyEvents, copyReports, copySnapshots, copyDeadwood, copyOrders)) == first, "HISTORY_IS_IDENTICAL_AFTER_A_SERIALISE_AND_RESTORE_OF_THE_RECORDS");
        // Derivation must not write to the persisted records.
        string before = JsonUtility.ToJson(new Wrapper { reports = s.Reports, events = s.Events });
        BuildAll(s);
        Check(JsonUtility.ToJson(new Wrapper { reports = s.Reports, events = s.Events }) == before, "BUILD_DOES_NOT_MUTATE_THE_PERSISTED_RECORDS");
        Check(typeof(InterventionSummary).GetFields().All(f => !f.IsDefined(typeof(SerializeField), false)) && !typeof(InterventionSummary).IsDefined(typeof(SerializableAttribute), false),
            "SUMMARY_TYPES_ARE_NOT_SERIALISABLE_SO_NOTHING_CAN_BE_SAVED");
    }

    [Serializable] private sealed class Wrapper { public List<ScenarioAnnualReport> reports; public List<ScenarioManagementEvent> events; }

    // ---------- stale orders ----------

    private static void StaleOrdersCheck(Story s)
    {
        List<StaleOrderNote> notes = ScenarioInterventionHistory.StaleOrders(s.Orders, 25);
        Check(notes.Select(n => n.Order.workOrderId).SequenceEqual(new[] { 900, 901 }), "OLD_OPEN_ORDER_AND_PROBLEM_ORDER_ARE_SURFACED", string.Join(",", notes.Select(n => n.Order.workOrderId)));
        Check(notes[0].AgeYears == 7 && notes[0].Reason.Contains("year 18") && notes[0].Reason.Contains("7 years"), "STALE_REASON_SAYS_WHEN_IT_WAS_PLANNED", notes[0].Reason);
        Check(notes[1].Reason == "Planting stock or species is unavailable.", "PROBLEM_ORDER_REASON_IS_THE_VALIDATION_MESSAGE");
        Check(!notes.Any(n => n.Order.workOrderId == 902 || n.Order.workOrderId == 903), "FRESH_AND_COMPLETED_ORDERS_ARE_NOT_FLAGGED");
        int before = s.Orders.Count;
        ScenarioInterventionHistory.StaleOrders(s.Orders, 25);
        Check(s.Orders.Count == before && s.Orders.All(o => o.status != ScenarioWorkStatus.Failed || o.resolvedYear >= 0), "NOTHING_IS_EXPIRED_OR_DELETED");
        Check(ScenarioInterventionHistory.StaleOrders(s.Orders, 19).Select(n => n.Order.workOrderId).SequenceEqual(new[] { 901 }), "YOUNG_ORDERS_ARE_NOT_STALE_YET");
    }

    // ---------- completion ----------

    private static void Completion(Story s)
    {
        CompletionSummary summary = ScenarioInterventionHistory.Completion(16, Story.StartingCash, s.Events, s.Reports, s.Snapshots, s.Deadwood, s.Orders);
        Check(summary.Year == 16 && summary.Interventions.Select(i => i.Year).SequenceEqual(new[] { 1, 6, 16 }), "COMPLETION_AT_THE_ACTUAL_COMPLETION_YEAR_NOT_YEAR_100", string.Join(",", summary.Interventions.Select(i => i.Year)));
        Check(summary.Baseline.year == 0 && summary.AtCompletion.year == 16, "BASELINE_AND_COMPLETION_SNAPSHOTS");
        Check(summary.TotalFelled == 42 && summary.TotalPlanted == 8 && summary.TotalPruned == 8, "COMPLETION_TOTALS_COUNT_ONLY_UP_TO_THE_COMPLETION_YEAR", $"felled={summary.TotalFelled} planted={summary.TotalPlanted} pruned={summary.TotalPruned}");
        Check(summary.TotalRevenueCents == 270000 && summary.NetCents == summary.TotalRevenueCents - summary.TotalCostCents, "COMPLETION_FINANCIAL_OUTCOME", $"revenue={summary.TotalRevenueCents} cost={summary.TotalCostCents}");
        Check(summary.ClosingCashCents == s.Reports.First(r => r.year == 16).closingCashCents && summary.MinimumCashCents <= summary.ClosingCashCents && summary.MinimumCashCents == Math.Min(Story.StartingCash, s.Reports.Where(r => r.year <= 16).Min(r => r.closingCashCents)), "COMPLETION_CASH_POSITION_AND_LOW_POINT");
        Check(summary.TotalOtherDeaths == 3 + 2 + 4, "COMPLETION_COUNTS_NATURAL_DEATHS_UP_TO_THE_YEAR", $"{summary.TotalOtherDeaths}");
        CompletionSummary early = ScenarioInterventionHistory.Completion(2, Story.StartingCash, s.Events, s.Reports, s.Snapshots, s.Deadwood, s.Orders);
        Check(early.Interventions.Count == 1 && early.AtCompletion.year == 2, "COMPLETION_IS_NOT_TIED_TO_A_FIXED_YEAR");
        CompletionSummary empty = ScenarioInterventionHistory.Completion(0, Story.StartingCash, new List<ScenarioManagementEvent>(), new List<ScenarioAnnualReport>(), new List<ScenarioEcologicalSnapshot>(), new List<ScenarioDeadwoodRecord>(), new List<ScenarioOneWorkOrder>());
        Check(empty.ClosingCashCents == Story.StartingCash && empty.Interventions.Count == 0 && empty.Baseline == null, "COMPLETION_DEGRADES_WITHOUT_RECORDS");
    }

    // ---------- the repeatable hint ----------

    private static void Hints(Story s)
    {
        List<InterventionSummary> list = BuildAll(s);
        string H(int year, int stale, bool done = false, int doneYear = -1) => ScenarioInterventionHistory.NextStepHint(list.Where(i => i.Year <= year).ToList(), year, stale, done, doneYear, 100);
        Check(H(0, 0).StartsWith("No intervention yet"), "HINT_BEFORE_ANY_INTERVENTION");
        Check(H(1, 0).Contains("Intervention 1 was carried out in year 1"), "HINT_RIGHT_AFTER_THE_FIRST_INTERVENTION");
        Check(H(2, 0).Contains("Intervention 1 was carried out"), "HINT_STAYS_ON_THE_JUST_DONE_PLAN_FOR_ONE_YEAR");
        Check(H(5, 0).Contains("4 years since intervention 1") && H(5, 0).Contains("plan intervention 2"), "HINT_LATER_NAMES_THE_NEXT_ORDINAL", H(5, 0));
        Check(H(20, 0).Contains("4 years since intervention 3") && H(20, 0).Contains("plan intervention 4"), "HINT_IS_THE_SAME_RULE_IN_THE_THIRD_CYCLE", H(20, 0));
        Check(H(20, 2).StartsWith("2 planned orders") && H(20, 2).Contains("need attention"), "STALE_ORDERS_TAKE_PRIORITY_OVER_THE_HISTORY_HINT", H(20, 2));
        Check(H(7, 1).StartsWith("1 planned order from earlier needs attention"), "HINT_SINGULAR_GRAMMAR", H(7, 1));
        Check(H(16, 3, true, 16).Contains("Objectives met in year 16") && H(16, 3, true, 16).Contains("Century Review (year 100)"), "COMPLETED_HINT_POINTS_TO_THE_SUMMARY_AND_KEEPS_THE_CENTURY_REVIEW_SEPARATE", H(16, 3, true, 16));
        Check(Enumerable.Range(0, 30).All(y => !string.IsNullOrEmpty(H(y, y % 3))), "A_HINT_EXISTS_FOR_EVERY_STATE_NO_DEAD_END");
    }

    // ---------- wording ----------

    private static void Wording(Story s)
    {
        List<InterventionSummary> list = BuildAll(s);
        var context = new HistoryTextContext { SpeciesName = id => id == "sessile-oak" ? "sessile oak" : id == "beech" ? "beech" : "Sitka spruce" };
        InterventionSummary a = list[0], b = list[1], c = list[2];
        Check(ManagementHistoryText.Title(b) == "Intervention 2 · year 6", "TITLE_NAMES_THE_ORDINAL_AND_YEAR", ManagementHistoryText.Title(b));
        string work = ManagementHistoryText.Work(a, context);
        Check(work == "30 felled · 4 pruned · 6 planted (3 beech, 3 sessile oak; 4 sheltered)", "WORK_LINE_IS_READABLE_AND_COMPLETE", work);
        Check(ManagementHistoryText.Work(b, context) == "4 pruned", "PRUNE_ONLY_WORK_LINE", ManagementHistoryText.Work(b, context));
        string mixed = ManagementHistoryText.Work(c, context);
        Check(mixed.Contains("12 felled") && mixed.Contains("3 cells cleared") && mixed.Contains("1 task could not be done") && mixed.Contains("crown base"), "FAILED_TASKS_ARE_SAID_PLAINLY_WITH_THE_REASON", mixed);
        string money = ManagementHistoryText.Money(a, context);
        Check(money.Contains("Cost 2656.00") && money.Contains("timber income 1800.00") && money.Contains("net -856.00") && money.Contains("small-job minimum"), "MONEY_LINE_REPORTS_COST_INCOME_NET_AND_THE_MINIMUM", money);
        Check(!ManagementHistoryText.Money(b, context).Contains("small-job"), "NO_MINIMUM_NOTE_WHEN_THERE_WAS_NO_VISIT_MINIMUM");
        string during = ManagementHistoryText.DuringPlan(a);
        Check(during.StartsWith("That year: living trees 336 →") && during.Contains("canopy 0.55 →"), "BEFORE_AFTER_LINE", during);
        string since = ManagementHistoryText.Since(a, context);
        Check(since.StartsWith("By year 25:") && since.Contains("sessile oak 3 → 5") && since.Contains("14 tree deaths recorded since") && !since.Contains("caused"), "SINCE_LINE_REPORTS_RECORDED_CHANGE_WITHOUT_CLAIMING_CAUSE", since);
        Check(ManagementHistoryText.Since(list[3], context).StartsWith("No later years recorded yet"), "SINCE_LINE_FOR_THE_NEWEST_PLAN_SAYS_TO_WAIT_AND_WALK");
        Check(ManagementHistoryText.NotCausalNote.Contains("not proof"), "NON_CAUSAL_NOTE_PRESENT");
        string prompt = ManagementHistoryText.FirstInterventionPrompt(a);
        Check(prompt.Contains("first intervention (year 1)") && prompt.Contains("walk the forest") && !prompt.Contains("must") && !prompt.Contains("should fell"), "FIRST_PROMPT_INVITES_OBSERVATION_AND_PRESCRIBES_NOTHING", prompt);
        CompletionSummary done = ScenarioInterventionHistory.Completion(16, Story.StartingCash, s.Events, s.Reports, s.Snapshots, s.Deadwood, s.Orders);
        List<string> lines = ManagementHistoryText.Completion(done, context);
        Check(lines[0] == "The Scenario One objectives were met in year 16." && lines.Count(l => l.StartsWith("  Intervention")) == 3, "COMPLETION_LISTS_EACH_INTERVENTION", lines.Count + " lines");
        Check(lines.Any(l => l.StartsWith("Stand, start → year 16:")) && lines.Any(l => l.StartsWith("Living trees at completion:")) && lines.Any(l => l.StartsWith("Money: cash 12000.00 →")), "COMPLETION_SHOWS_STAND_COMPOSITION_AND_MONEY");
        Check(lines[lines.Count - 1].Contains("not a score") && lines[lines.Count - 1].Contains("Century Review (year 100) is separate"), "COMPLETION_IS_NOT_A_SCORE_AND_KEEPS_THE_CENTURY_REVIEW_SEPARATE");
        Check(lines.All(l => !string.IsNullOrWhiteSpace(l)), "NO_EMPTY_LINES");
    }
}
