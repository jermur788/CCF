// Offline check for the S1-C T-03 intervention drill-down: ScenarioManagementHistory.DescribeWork, compiled from the
// PRODUCTION sources on the plain .NET SDK (no Unity). Nothing here is used by the game.
//
// Proves the attribution rules the packet names: grouped commissioned-harvest money is one job settlement and never a
// per-order figure; harvestJobId is not read as a job identity; cancelled orders surface from their cancellation event
// alone; failed work is distinct and keeps its recorded reason; fields a record does not hold stay "not recorded" instead
// of being rebuilt; grouping and ordering are deterministic; and the selected-year UI does not join raw record lists.
//
// Usage: run_t03_offline_check.py   Prints S1C_T03_DRILLDOWN_* tokens.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CCF.Forestry.WorkEconomy;
using UnityEngine;

public static class S1CDrillDownOfflineCheck
{
    private static int failures, checks;

    public static int Main()
    {
        Fixture f = Fixture.Build();
        GroupedFellingYear(f);
        HarvestJobIdIsNotAnIdentity(f);
        PlantingYear(f);
        CancelledOrders(f);
        FailedOrders(f);
        NoWorkYear(f);
        MissingFieldsStayMissing(f);
        PassThroughFidelity(f);
        Determinism(f);
        PerOrderMoneyRule(f);
        UiSourceAudit();
        Console.WriteLine(failures == 0
            ? $"S1C_T03_DRILLDOWN_PASS checks={checks}"
            : $"S1C_T03_DRILLDOWN_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    private static YearHistoryView View(Fixture f, int year) =>
        ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, year);

    private static YearWorkDetail Work(Fixture f, int year) => ScenarioManagementHistory.DescribeWork(View(f, year));

    // ---- 1. grouped felling year: aggregate-first, targets on demand, exactly one settlement, no per-order money ----
    private static void GroupedFellingYear(Fixture f)
    {
        YearHistoryView view = View(f, 1);
        YearWorkDetail work = ScenarioManagementHistory.DescribeWork(view);
        WorkGroupDetail fell = work.DoneFor(ScenarioWorkType.FellTree);
        WorkGroupDetail salvage = work.DoneFor(ScenarioWorkType.SalvageDeadwood);
        Check(fell != null && fell.Items.Count == 3, "grouped year: three felling tasks in one group", fell == null ? "null" : fell.Items.Count.ToString());
        Check(fell.Items.Select(i => i.TargetTreeId).SequenceEqual(new[] { "P0101", "P0102", "P0103" }), "grouped year: recorded target tree ids in event order",
            string.Join(",", fell.Items.Select(i => i.TargetTreeId)));
        Check(salvage != null && salvage.Items.Count == 1, "grouped year: salvage is its own group");
        Check(fell.IsGroupedHarvest && salvage.IsGroupedHarvest, "grouped year: felling and salvage are both flagged as the grouped harvest job");
        Check(work.HarvestSettlement != null && ReferenceEquals(work.HarvestSettlement, view.HarvestSettlement),
            "grouped year: YearHistoryView.HarvestSettlement is consumed as the settlement, not recomputed");
        Check(work.HarvestSettlement.SettlementEventId == Fixture.SettlementEventId
            && work.HarvestSettlement.SettlementRevenueCents == Fixture.SettlementRevenue
            && work.HarvestSettlement.SettlementContractorCostCents == Fixture.SettlementCost,
            "grouped year: settlement carries the one recorded job figure (revenue, cost, event id)");
        Check(work.HarvestSettlement.ResolutionYearLabel == 1, "grouped year: job label is the resolution year");

        var harvestItems = fell.Items.Concat(salvage.Items).ToList();
        Check(harvestItems.All(i => i.IsGroupedHarvest && !i.OwnMoneyRecorded), "grouped year: no harvest item claims money of its own");
        Check(harvestItems.All(i => i.ContractorCostCents == 0 && i.StockCostCents == 0 && i.OwnerMinutes == 0),
            "grouped year: no per-order cost fields are populated on grouped harvest items");
        Check(harvestItems.Count == 4 && work.Done.Count(g => g.IsGroupedHarvest) == 2, "grouped year: exactly one settlement spans the four harvest tasks");

        // The records of the earlier harvest events may hold stale per-order figures. They must never reach an item.
        ScenarioManagementEvent first = f.Events.First(e => e.eventType == ScenarioManagementEventType.WorkResolved && e.workOrderId == Fixture.FellA);
        long savedCost = first.contractorCostCents, savedRevenue = first.timberRevenueCents;
        first.contractorCostCents = 999999; first.timberRevenueCents = 888888;
        YearWorkDetail poisoned = Work(f, 1);
        Check(poisoned.DoneFor(ScenarioWorkType.FellTree).Items.All(i => i.ContractorCostCents == 0) && Serialise(poisoned) == Serialise(work),
            "grouped year: a per-order money value written on a harvest event does not appear in the detail");
        first.contractorCostCents = savedCost; first.timberRevenueCents = savedRevenue;

        Check(fell.DistinctRecordedCells == 2, "grouped year: aggregate cell count counts only recorded cells", fell.DistinctRecordedCells.ToString());
        Check(fell.SpeciesCounts.Count == 1 && fell.SpeciesCounts[0].Key == "sitka-spruce" && fell.SpeciesCounts[0].Value == 3, "grouped year: aggregate species summary");
    }

    // ---- 2. harvestJobId is a year label, not a unique job identity ----
    private static void HarvestJobIdIsNotAnIdentity(Fixture f)
    {
        string baseline = Serialise(Work(f, 1));
        var saved = f.Orders.Select(o => o.harvestJobId).ToList();
        for (int i = 0; i < f.Orders.Count; i++) f.Orders[i].harvestJobId = 4000 + i;
        Check(Serialise(Work(f, 1)) == baseline, "harvestJobId: detail is identical whatever harvestJobId holds");
        for (int i = 0; i < f.Orders.Count; i++) f.Orders[i].harvestJobId = saved[i];
        string history = ReadSource("Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs");
        string describe = history.Substring(history.IndexOf("public static YearWorkDetail DescribeWork", StringComparison.Ordinal));
        describe = describe.Substring(0, describe.IndexOf("// Years that have been completed", StringComparison.Ordinal));
        Check(!describe.Contains("harvestJobId"), "harvestJobId: the drill-down code never reads it");
        Check(!typeof(WorkItemDetail).GetFields().Any(x => x.Name.IndexOf("JobId", StringComparison.OrdinalIgnoreCase) >= 0)
            && !typeof(YearWorkDetail).GetFields().Any(x => x.Name.IndexOf("JobId", StringComparison.OrdinalIgnoreCase) >= 0),
            "harvestJobId: no job-id member is exposed by the drill-down types");
    }

    // ---- 3. planting year: aggregate species/count, targets and method only as recorded ----
    private static void PlantingYear(Fixture f)
    {
        YearWorkDetail work = Work(f, 2);
        WorkGroupDetail plant = work.DoneFor(ScenarioWorkType.PlantJuvenile);
        Check(plant != null && plant.Items.Count == 4 && !plant.IsGroupedHarvest, "planting: four planting tasks, not a grouped harvest");
        Check(plant.SpeciesCounts.Select(p => p.Key + ":" + p.Value).SequenceEqual(new[] { "beech:1", "sessile-oak:3" }), "planting: species counts ordinal-sorted",
            string.Join(",", plant.SpeciesCounts.Select(p => p.Key + ":" + p.Value)));
        Check(plant.Items.Select(i => i.ExecutionMethod).SequenceEqual(new[] { WorkExecutionMethod.LandownerSimulated, WorkExecutionMethod.LandownerSimulated, WorkExecutionMethod.Contractor, WorkExecutionMethod.Contractor })
            && plant.Items.All(i => i.ExecutionMethodRecorded), "planting: execution method is the recorded one per task");
        Check(plant.Items[0].PositionRecorded && Math.Abs(plant.Items[0].PositionX - 12.5f) < 1e-4f && plant.Items[0].CellIndex == 7, "planting: recorded position and cell pass through");
        Check(!plant.Items[2].PositionRecorded && plant.Items[2].CellIndex == 9, "planting: a cell-only record has a cell and no position");
        Check(!plant.Items[3].PositionRecorded && plant.Items[3].CellIndex == -1, "planting: a task with neither is left unlocated");
        Check(plant.DistinctRecordedCells == 2, "planting: aggregate cell count ignores the unlocated task", plant.DistinctRecordedCells.ToString());
        Check(plant.Items[0].ShelterRecorded && plant.Items[0].ShelterInstalled && !plant.Items[1].ShelterRecorded,
            "planting: shelter is reported only while the order record still exists");
        Check(plant.Items.All(i => i.TargetTreeId.Length == 0), "planting: no target tree is invented");
        Check(work.HarvestSettlement == null && work.Done.Count == 1, "planting: a planting year has no harvest settlement");
    }

    // ---- 4. cancelled orders survive only through the cancellation event ----
    private static void CancelledOrders(Fixture f)
    {
        YearWorkDetail work = Work(f, 3);
        Check(work.Cancelled.Items.Count == 2, "cancelled: both cancellations surface", work.Cancelled.Items.Count.ToString());
        Check(work.Done.Count == 0 && work.Failed.Items.Count == 0, "cancelled: a cancellation is neither done nor failed work");
        WorkItemDetail fell = work.Cancelled.Items[0];
        Check(fell.Status == WorkItemStatus.Cancelled && fell.TaskType == ScenarioWorkType.FellTree && fell.TargetTreeId == "P0505" && fell.WorkOrderId == Fixture.CancelledFell,
            "cancelled: target and order id come from the cancellation event");
        Check(!fell.OrderStillExists && !f.Orders.Any(o => o.workOrderId == Fixture.CancelledFell), "cancelled: the order object is gone and the entry does not need it");
        Check(work.Cancelled.Items.All(i => !i.ExecutionMethodRecorded), "cancelled: a cancellation never claims an execution method");
        Check(work.Cancelled.Items.All(i => i.FailureReason.Length == 0 && i.Treatment == ScenarioEcologicalTreatment.None && !i.OwnMoneyRecorded && !i.IsGroupedHarvest),
            "cancelled: no outcome, treatment or money is attached");
        Check(work.Cancelled.Items.All(i => i.Year == 3), "cancelled: year is the year of the cancellation event");
        WorkItemDetail plant = work.Cancelled.Items[1];
        Check(plant.TaskType == ScenarioWorkType.PlantJuvenile && plant.SpeciesId == "beech" && plant.CellIndex == 3, "cancelled: a cancelled planting keeps the species and cell it recorded");
        Check(work.Cancelled.Key == "cancelled" && work.HarvestSettlement == null, "cancelled: keyed separately and never settles money");
    }

    // ---- 5. failed orders are distinct and keep their recorded reason ----
    private static void FailedOrders(Fixture f)
    {
        YearWorkDetail work = Work(f, 4);
        Check(work.Failed.Items.Count == 2, "failed: two failed tasks", work.Failed.Items.Count.ToString());
        Check(work.Failed.Items.All(i => i.Status == WorkItemStatus.Failed), "failed: statuses are Failed");
        Check(work.Failed.Items[0].FailureReason == "Cancelled: target missing, dead, already felled or otherwise invalid; no harvest charge."
            && work.Failed.Items[1].FailureReason == "Planting cell is outside the stand.", "failed: the recorded failure reasons are carried verbatim");
        WorkItemDetail failedHarvest = work.Failed.Items[0];
        Check(failedHarvest.TaskType == ScenarioWorkType.FellTree && !failedHarvest.IsGroupedHarvest && !failedHarvest.OwnMoneyRecorded && failedHarvest.BiologicalVolumeM3 == 0f,
            "failed: a failed harvest order is outside the grouped job, with no money or volume");
        Check(work.Failed.Items.All(i => i.ExecutionMethodRecorded && i.Treatment == ScenarioEcologicalTreatment.None), "failed: resolution fields keep method and no treatment");
        WorkGroupDetail fell = work.DoneFor(ScenarioWorkType.FellTree);
        Check(fell != null && fell.Items.Count == 2 && fell.Items.All(i => i.IsGroupedHarvest), "failed: the same year's successful felling is the grouped job");
        Check(work.HarvestSettlement != null && work.HarvestSettlement.GroupedOrderIds.Count == 2 && !work.HarvestSettlement.GroupedOrderIds.Contains(Fixture.FailedFell),
            "failed: the failed harvest order is not part of the settlement group");
        Check(work.Done.All(g => g.Items.All(i => i.FailureReason.Length == 0)), "failed: successful work carries no failure reason");
        Check(work.Failed.Key == "failed" && work.Done.All(g => g.Key.StartsWith("done-")), "failed: failed and done groups have distinct keys");
    }

    // ---- 6. a year without work stays coherent ----
    private static void NoWorkYear(Fixture f)
    {
        YearWorkDetail work = Work(f, 5);
        Check(work != null && work.Year == 5 && work.Done.Count == 0 && work.Failed.Items.Count == 0 && work.Cancelled.Items.Count == 0 && work.HarvestSettlement == null,
            "no-work year: nothing to drill into and no settlement");
        Check(DescribeNull() == null, "no-work year: a missing year yields no detail");
    }

    private static YearWorkDetail DescribeNull() => ScenarioManagementHistory.DescribeWork(null);

    // ---- 7. missing target/location fields never become reconstructed facts ----
    private static void MissingFieldsStayMissing(Fixture f)
    {
        YearWorkDetail work = Work(f, 6);
        WorkItemDetail bare = work.DoneFor(ScenarioWorkType.PruneTree).Items[0];
        Check(bare.TargetTreeId.Length == 0 && bare.CellIndex == -1 && !bare.PositionRecorded && bare.SpeciesId.Length == 0,
            "missing fields: a bare record stays blank");
        // The order record still exists and holds the tree id/cell/position. The detail reads the event, so it is not borrowed.
        ScenarioOneWorkOrder order = f.Orders.First(o => o.workOrderId == Fixture.BarePrune);
        Check(order.targetTreeId == "P0900" && order.cellIndex == 5 && bare.TargetTreeId.Length == 0 && bare.CellIndex == -1,
            "missing fields: values only the current order holds are not substituted for the event");
        Check(bare.OrderStillExists, "missing fields: the item reports that the order record still exists, without using it");
        WorkItemDetail bareFailed = work.Failed.Items[0];
        Check(bareFailed.FailureReason.Length == 0, "missing fields: a failure with no recorded reason has an empty reason (shown as not recorded)");
        Check(!bareFailed.PositionRecorded && bareFailed.CellIndex == -1, "missing fields: a failure with no location stays unlocated");
    }

    // ---- 8. every item is exactly its event ----
    private static void PassThroughFidelity(Fixture f)
    {
        bool allMatch = true;
        int items = 0;
        foreach (int year in ScenarioManagementHistory.CompletedYears(f.Events, f.Reports))
        {
            YearWorkDetail work = Work(f, year);
            var all = work.Done.SelectMany(g => g.Items).Concat(work.Failed.Items).Concat(work.Cancelled.Items).ToList();
            foreach (WorkItemDetail item in all)
            {
                items++;
                ScenarioManagementEvent e = f.Events.First(x => x.eventId == item.EventId);
                bool kind = item.Status == WorkItemStatus.Cancelled
                    ? e.eventType == ScenarioManagementEventType.OrderCancelled
                    : e.eventType == ScenarioManagementEventType.WorkResolved;
                bool same = kind && e.year == item.Year && e.workOrderId == item.WorkOrderId && e.taskType == item.TaskType
                    && (e.targetTreeId ?? "") == item.TargetTreeId && (e.speciesId ?? "") == item.SpeciesId
                    && (e.cellIndex >= 0 ? e.cellIndex : -1) == item.CellIndex
                    && e.worldPosition.x == item.PositionX && e.worldPosition.z == item.PositionZ;
                if (!same) { allMatch = false; Console.WriteLine("MISMATCH event " + e.eventId); }
            }
            int events = f.Events.Count(e => e.year == year && (e.eventType == ScenarioManagementEventType.WorkResolved || e.eventType == ScenarioManagementEventType.OrderCancelled));
            if (events != all.Count) { allMatch = false; Console.WriteLine($"COUNT year {year}: {events} events vs {all.Count} items"); }
        }
        Check(allMatch && items == 17, "fidelity: every resolved or cancelled event is exactly one item with the event's own facts", items.ToString());
    }

    // ---- 9. deterministic grouping and ordering ----
    private static void Determinism(Fixture f)
    {
        string first = SerialiseAll(f), second = SerialiseAll(f);
        Check(first == second, "determinism: repeated queries serialise identically");
        var reversed = new Fixture();
        reversed.Events.AddRange(f.Events.AsEnumerable().Reverse());
        reversed.Reports.AddRange(f.Reports.AsEnumerable().Reverse());
        reversed.Snapshots.AddRange(f.Snapshots.AsEnumerable().Reverse());
        reversed.Deadwood.AddRange(f.Deadwood.AsEnumerable().Reverse());
        reversed.Orders.AddRange(f.Orders.AsEnumerable().Reverse());
        reversed.Storms.AddRange(f.Storms);
        Check(SerialiseAll(reversed) == first, "determinism: input list order does not change items, groups or ordering");
        YearWorkDetail work = Work(f, 1);
        Check(work.Done.Select(g => g.TaskType).SequenceEqual(new[] { ScenarioWorkType.FellTree, ScenarioWorkType.PruneTree, ScenarioWorkType.SalvageDeadwood }), "determinism: succeeded groups follow task-type (enum) order");
        Check(work.Done.All(g => g.Items.Select(i => i.EventId).SequenceEqual(g.Items.Select(i => i.EventId).OrderBy(x => x))), "determinism: items follow event id");
        // The query is pure: it must not change the records it reads.
        string before = RecordHash(f);
        for (int i = 0; i < 3; i++) SerialiseAll(f);
        Check(RecordHash(f) == before, "determinism: querying twice leaves the records unchanged");
    }

    // ---- 10. only successful non-harvest work carries its own recorded money ----
    private static void PerOrderMoneyRule(Fixture f)
    {
        WorkItemDetail plant = Work(f, 2).DoneFor(ScenarioWorkType.PlantJuvenile).Items[0];
        Check(plant.OwnMoneyRecorded && plant.StockCostCents == 500 && plant.OwnerMinutes == 5 && plant.ContractorCostCents == 0,
            "money: a successful planting passes through its own recorded cost fields");
        WorkItemDetail pruned = Work(f, 1).DoneFor(ScenarioWorkType.PruneTree).Items[0];
        Check(pruned.OwnMoneyRecorded && pruned.ContractorCostCents == 1800 && !pruned.IsGroupedHarvest, "money: a successful pruning passes through its own recorded cost");
        var everyDoneHarvest = Enumerable.Range(1, 6).Select(y => Work(f, y)).SelectMany(w => w.Done).Where(g => g.TaskType == ScenarioWorkType.FellTree || g.TaskType == ScenarioWorkType.SalvageDeadwood)
            .SelectMany(g => g.Items).ToList();
        Check(everyDoneHarvest.Count > 0 && everyDoneHarvest.All(i => !i.OwnMoneyRecorded || !i.IsGroupedHarvest), "money: grouped harvest items never carry their own money");
        Check(Enumerable.Range(1, 6).Select(y => Work(f, y)).Where(w => w.HarvestSettlement != null).All(w => ReferenceEquals(w.HarvestSettlement, View(f, w.Year).HarvestSettlement) || w.HarvestSettlement.ResolutionYearLabel == w.Year),
            "money: each settling year exposes exactly its own single settlement");
    }

    // ---- 11. the selected-year UI boundary ----
    private static void UiSourceAudit()
    {
        string ui = ReadSource("Assets/ForestPrototype/UI/AnnualReviewView.cs");
        Check(CountOf(ui, ".ManagementEvents") == 1 && CountOf(ui, ".AnnualReports") == 3 && CountOf(ui, ".EcologicalSnapshots") == 2,
            "ui audit: raw-list mentions equal the T-02 baseline (change key and all-year trend strips only; none added by T-03)",
            $"events={CountOf(ui, ".ManagementEvents")} reports={CountOf(ui, ".AnnualReports")} snapshots={CountOf(ui, ".EcologicalSnapshots")}");
        var rawLines = ui.Split('\n').Where(l => Regex.IsMatch(l, @"\.(ManagementEvents|AnnualReports|EcologicalSnapshots)")).Select(l => l.Trim()).ToList();
        Check(rawLines.All(l => l.Contains("m.ManagementEvents.Count") || l.Contains("m.AnnualReports.Count") || l.Contains("m.AnnualReports.Skip") || l.Contains("m.EcologicalSnapshots.Skip")),
            "ui audit: each raw-list mention is a count or an all-year trend", string.Join(" || ", rawLines.Select(l => l.Length > 70 ? l.Substring(0, 70) : l)));
        Check(ui.Contains("ScenarioManagementHistory.DescribeWork(year)"), "ui audit: the Work column reads the drill-down from the selected YearHistoryView");
        Check(!ui.Contains("harvestJobId") && !ui.Contains(".workOrders") && !ui.Contains("WorkOrders"), "ui audit: the view does not read harvestJobId or raw work orders");
        Check(!Regex.IsMatch(ui, @"(ManagementEvents|AnnualReports|EcologicalSnapshots)\s*\.\s*(Where|First|Any|Select|Join|Single|Last)"),
            "ui audit: no join or filter over a raw record list");
        string workBody = ui.Substring(ui.IndexOf("private void BuildWork(", StringComparison.Ordinal));
        workBody = workBody.Substring(0, workBody.IndexOf("private static void BuildMoney(", StringComparison.Ordinal));
        Check(!workBody.Contains("contractorCostCents") && !workBody.Contains("timberRevenueCents"), "ui audit: the Work card reads no per-event money field");
        int settlementPresentations = CountOf(workBody, "AddHarvestSettlement(c, detail)");
        Check(settlementPresentations == 1 && CountOf(ui, "name = \"annual-harvest-settlement\"") == 1, "ui audit: exactly one grouped settlement presentation is built per Work card");
        Check(ui.Contains("(!showingEarlierYear)") || ui.Contains("if (!showingEarlierYear) ui.AcknowledgeAnnualReview()"), "ui audit: the older-year acknowledgement guard is still present");
        Check(ui.Contains("&& !historical ? DisplayStyle.Flex : DisplayStyle.None"), "ui audit: the acknowledgement footer predicate is unchanged");
        string drill = ui.Substring(ui.IndexOf("private void AddDetail(", StringComparison.Ordinal));
        drill = drill.Substring(0, drill.IndexOf("private static void BuildMoney(", StringComparison.Ordinal));
        Check(!Regex.IsMatch(drill, @"\b(AcknowledgeAnnualReview|MarkAnnualReviewSeen|RecordEvent)\b|\.(Approve|Cancel|Advance|Save)\w*\("), "ui audit: the drill-down code reaches no state-changing call");
    }

    private static int CountOf(string text, string needle)
    {
        int count = 0, at = 0;
        while ((at = text.IndexOf(needle, at, StringComparison.Ordinal)) >= 0) { count++; at += needle.Length; }
        return count;
    }

    private static string ReadSource(string relative)
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? Directory.GetCurrentDirectory();
        return File.ReadAllText(Path.Combine(root, relative));
    }

    private static string SerialiseAll(Fixture f)
    {
        var sb = new StringBuilder();
        foreach (int year in ScenarioManagementHistory.CompletedYears(f.Events, f.Reports))
            sb.Append(Serialise(Work(f, year)));
        return sb.ToString();
    }

    private static string Serialise(YearWorkDetail work)
    {
        var sb = new StringBuilder();
        sb.Append("Y").Append(work.Year);
        if (work.HarvestSettlement != null)
            sb.Append(" S[").Append(work.HarvestSettlement.SettlementEventId).Append('/').Append(work.HarvestSettlement.SettlementRevenueCents)
              .Append('/').Append(work.HarvestSettlement.SettlementContractorCostCents).Append('/').Append(work.HarvestSettlement.ResolutionYearLabel).Append(']');
        foreach (WorkGroupDetail g in work.Done.Concat(new[] { work.Failed, work.Cancelled }))
        {
            sb.Append("\n G ").Append(g.Key).Append(" grouped=").Append(g.IsGroupedHarvest).Append(" cells=").Append(g.DistinctRecordedCells)
              .Append(" species=").Append(string.Join(",", g.SpeciesCounts.Select(p => p.Key + p.Value)));
            foreach (WorkItemDetail i in g.Items)
                sb.Append("\n  ").Append(i.EventId).Append('|').Append(i.WorkOrderId).Append('|').Append(i.TaskType).Append('|').Append(i.Status)
                  .Append('|').Append(i.Year).Append('|').Append(i.TargetTreeId).Append('|').Append(i.SpeciesId).Append('|').Append(i.CellIndex)
                  .Append('|').Append(i.PositionRecorded).Append(':').Append(i.PositionX.ToString("R", CultureInfo.InvariantCulture)).Append(',')
                  .Append(i.PositionZ.ToString("R", CultureInfo.InvariantCulture)).Append('|').Append(i.ExecutionMethodRecorded).Append(':').Append(i.ExecutionMethod)
                  .Append('|').Append(i.FailureReason).Append('|').Append(i.Treatment).Append('|').Append(i.BiologicalVolumeM3.ToString("R", CultureInfo.InvariantCulture))
                  .Append('|').Append(i.OrderStillExists).Append('|').Append(i.ShelterRecorded).Append(i.ShelterInstalled).Append('|').Append(i.IsGroupedHarvest)
                  .Append('|').Append(i.OwnMoneyRecorded).Append(':').Append(i.ContractorCostCents).Append(':').Append(i.StockCostCents).Append(':').Append(i.OwnerMinutes);
        }
        return sb.Append('\n').ToString();
    }

    private static string RecordHash(Fixture f)
    {
        var sb = new StringBuilder();
        foreach (ScenarioManagementEvent e in f.Events)
            sb.Append(e.eventId).Append(e.year).Append(e.eventType).Append(e.outcome).Append(e.workOrderId).Append(e.targetTreeId).Append(e.cellIndex)
              .Append(e.contractorCostCents).Append(e.timberRevenueCents).Append(e.stockCostCents).Append(e.failureReason).Append(e.executionMethod).Append(';');
        foreach (ScenarioOneWorkOrder o in f.Orders) sb.Append(o.workOrderId).Append(o.status).Append(o.harvestJobId).Append(o.resolvedYear).Append(';');
        return sb.ToString();
    }

    // The fixture is built by hand from the shapes ScenarioOneManager records (RecordOrderEvent, the WorkResolved block of
    // AdvanceYear and SettleHarvestJob), including the awkward ones: a grouped harvest whose settlement sits on the last
    // harvest event, a cancelled order with no order record, a failed harvest order outside the job, and a bare record.
    private sealed class Fixture
    {
        public const int FellA = 71, FellB = 72, FellC = 73, SalvageA = 74, PruneA = 75;
        public const int CancelledFell = 70, CancelledPlant = 76, FailedFell = 85, FailedPlant = 86, BarePrune = 99;
        public const int SettlementEventId = 4;
        public const long SettlementRevenue = 180000, SettlementCost = 250000;

        public readonly List<ScenarioManagementEvent> Events = new List<ScenarioManagementEvent>();
        public readonly List<ScenarioAnnualReport> Reports = new List<ScenarioAnnualReport>();
        public readonly List<ScenarioEcologicalSnapshot> Snapshots = new List<ScenarioEcologicalSnapshot>();
        public readonly List<ScenarioDeadwoodRecord> Deadwood = new List<ScenarioDeadwoodRecord>();
        public readonly List<ScenarioOneWorkOrder> Orders = new List<ScenarioOneWorkOrder>();
        public readonly List<StormEventRecord> Storms = new List<StormEventRecord>();
        private int nextEvent = 1;

        public static Fixture Build()
        {
            var f = new Fixture();

            // Year 1: grouped commissioned harvest (three fellings + one windthrow salvage) plus one pruning. The settlement
            // lands on the LAST harvest event; the earlier harvest events keep the zero money they recorded before settlement.
            f.Order(FellA, ScenarioWorkType.FellTree, "P0101", 1, ScenarioWorkStatus.Completed, harvestJob: 1);
            f.Order(FellB, ScenarioWorkType.FellTree, "P0102", 1, ScenarioWorkStatus.Completed, harvestJob: 1);
            f.Order(FellC, ScenarioWorkType.FellTree, "P0103", 1, ScenarioWorkStatus.Completed, harvestJob: 1);
            f.Order(SalvageA, ScenarioWorkType.SalvageDeadwood, "P0104", 1, ScenarioWorkStatus.Completed, harvestJob: 1);
            f.Order(PruneA, ScenarioWorkType.PruneTree, "P0202", 1, ScenarioWorkStatus.Completed, harvestJob: -1);
            f.Resolve(1, FellA, ScenarioWorkType.FellTree, "P0101", species: "sitka-spruce", cell: 11, pos: new Vector3(30.5f, 4f, 20.25f), treatment: ScenarioEcologicalTreatment.TreeFelledAndExtracted, volume: 0.62f);
            f.Resolve(1, FellB, ScenarioWorkType.FellTree, "P0102", species: "sitka-spruce", cell: 11, pos: new Vector3(31.5f, 4f, 21.25f), treatment: ScenarioEcologicalTreatment.TreeFelledAndExtracted, volume: 0.55f);
            f.Resolve(1, FellC, ScenarioWorkType.FellTree, "P0103", species: "sitka-spruce", cell: 12, pos: new Vector3(33.5f, 4f, 22.25f), treatment: ScenarioEcologicalTreatment.TreeRetainedAsDeadwood, volume: 0.48f);
            ScenarioManagementEvent settled = f.Resolve(1, SalvageA, ScenarioWorkType.SalvageDeadwood, "P0104", species: "sitka-spruce", cell: 13, pos: new Vector3(35.5f, 0.5f, 24.25f), treatment: ScenarioEcologicalTreatment.WindthrowSalvaged, volume: 0.2f);
            f.Resolve(1, PruneA, ScenarioWorkType.PruneTree, "P0202", species: "sitka-spruce", cell: 20, pos: new Vector3(10.5f, 4f, 9.25f), treatment: ScenarioEcologicalTreatment.TreePruned, contractor: 1800);
            Check(settled.eventId == SettlementEventId, "fixture: the settlement event is the last harvest event");
            settled.contractorCostCents = SettlementCost; settled.timberRevenueCents = SettlementRevenue; settled.cashDeltaCents = SettlementRevenue - SettlementCost;
            f.Advance(1, new ScenarioAnnualReport { year = 1, completedTasks = 5, contractorCostCents = SettlementCost + 1800, timberRevenueCents = SettlementRevenue, harvestMinimumAdjustmentCents = 60000, closingCashCents = 1130000, ownerMinutes = 0 });

            // Year 2: planting — two by the owner (one sheltered, order gone for the second), two by contractor. The third has a
            // cell but no position; the fourth has neither.
            f.Order(60, ScenarioWorkType.PlantJuvenile, "", 2, ScenarioWorkStatus.Completed, species: "sessile-oak", shelter: true);
            f.Resolve(2, 60, ScenarioWorkType.PlantJuvenile, "", species: "sessile-oak", cell: 7, pos: new Vector3(12.5f, 3f, 40.75f), method: WorkExecutionMethod.LandownerSimulated, treatment: ScenarioEcologicalTreatment.JuvenilePlanted, stock: 500, ownerMinutes: 5);
            f.Resolve(2, 61, ScenarioWorkType.PlantJuvenile, "", species: "sessile-oak", cell: 7, pos: new Vector3(13.5f, 3f, 41.75f), method: WorkExecutionMethod.LandownerSimulated, treatment: ScenarioEcologicalTreatment.JuvenilePlanted, stock: 500, ownerMinutes: 5);
            f.Resolve(2, 62, ScenarioWorkType.PlantJuvenile, "", species: "sessile-oak", cell: 9, method: WorkExecutionMethod.Contractor, treatment: ScenarioEcologicalTreatment.JuvenilePlanted, contractor: 900);
            f.Resolve(2, 63, ScenarioWorkType.PlantJuvenile, "", species: "beech", method: WorkExecutionMethod.Contractor, treatment: ScenarioEcologicalTreatment.JuvenilePlanted, contractor: 900);
            f.Advance(2, new ScenarioAnnualReport { year = 2, completedTasks = 4, closingCashCents = 1120000 });

            // Year 3: two cancellations. Cancelling deletes the order object, so neither has an order record.
            f.Cancel(3, CancelledFell, ScenarioWorkType.FellTree, "P0505", cell: 4, pos: new Vector3(55.5f, 4f, 12.25f));
            f.Cancel(3, CancelledPlant, ScenarioWorkType.PlantJuvenile, "", species: "beech", cell: 3);
            f.Advance(3, new ScenarioAnnualReport { year = 3, closingCashCents = 1120000 });

            // Year 4: a successful grouped harvest (two fellings), a failed harvest order outside the job, and a failed planting.
            f.Order(FailedFell, ScenarioWorkType.FellTree, "GONE1", 4, ScenarioWorkStatus.Failed, harvestJob: 4);
            f.Order(87, ScenarioWorkType.FellTree, "P0301", 4, ScenarioWorkStatus.Completed, harvestJob: 4);
            f.Order(88, ScenarioWorkType.FellTree, "P0302", 4, ScenarioWorkStatus.Completed, harvestJob: 4);
            f.Resolve(4, FailedFell, ScenarioWorkType.FellTree, "GONE1", species: "sitka-spruce", cell: 2, pos: new Vector3(5.5f, 4f, 6.25f), failed: "Cancelled: target missing, dead, already felled or otherwise invalid; no harvest charge.");
            f.Resolve(4, 87, ScenarioWorkType.FellTree, "P0301", species: "sitka-spruce", cell: 15, pos: new Vector3(40.5f, 4f, 40.25f), treatment: ScenarioEcologicalTreatment.TreeFelledAndExtracted, volume: 0.4f);
            ScenarioManagementEvent settled4 = f.Resolve(4, 88, ScenarioWorkType.FellTree, "P0302", species: "sitka-spruce", cell: 15, pos: new Vector3(41.5f, 4f, 41.25f), treatment: ScenarioEcologicalTreatment.TreeFelledAndExtracted, volume: 0.45f);
            settled4.contractorCostCents = 250000; settled4.timberRevenueCents = 70000;
            f.Resolve(4, FailedPlant, ScenarioWorkType.PlantJuvenile, "", species: "beech", cell: -1, failed: "Planting cell is outside the stand.");
            f.Advance(4, new ScenarioAnnualReport { year = 4, failedTasks = 2, completedTasks = 2, contractorCostCents = 250000, timberRevenueCents = 70000, closingCashCents = 940000 });

            // Year 5: no work at all.
            f.Advance(5, new ScenarioAnnualReport { year = 5, closingCashCents = 940000 });

            // Year 6: bare records. The pruning has no tree id, cell, position or species on its event, while its order record
            // (still present) holds all of them; the failure has no recorded reason.
            f.Orders.Add(new ScenarioOneWorkOrder { workOrderId = BarePrune, type = ScenarioWorkType.PruneTree, status = ScenarioWorkStatus.Completed, targetTreeId = "P0900", cellIndex = 5, worldPosition = new Vector3(9f, 4f, 9f), resolvedYear = 6 });
            f.Resolve(6, BarePrune, ScenarioWorkType.PruneTree, "", treatment: ScenarioEcologicalTreatment.TreePruned);
            f.Resolve(6, 98, ScenarioWorkType.FellTree, "", failed: "");
            f.Advance(6, new ScenarioAnnualReport { year = 6, closingCashCents = 940000 });
            return f;
        }

        private void Order(int id, ScenarioWorkType type, string tree, int resolved, ScenarioWorkStatus status, int harvestJob = -1, string species = "", bool shelter = false)
        {
            Orders.Add(new ScenarioOneWorkOrder
            {
                workOrderId = id, type = type, status = status, targetTreeId = tree, speciesId = species, createdYear = resolved - 1, resolvedYear = resolved,
                harvestJobId = harvestJob, installShelter = shelter
            });
        }

        private ScenarioManagementEvent Resolve(int year, int orderId, ScenarioWorkType type, string tree, string species = "", int cell = -1, Vector3 pos = default(Vector3),
            WorkExecutionMethod method = WorkExecutionMethod.Contractor, ScenarioEcologicalTreatment treatment = ScenarioEcologicalTreatment.None, float volume = 0f,
            long contractor = 0, long stock = 0, int ownerMinutes = 0, string failed = null)
        {
            bool fail = failed != null;
            var e = new ScenarioManagementEvent
            {
                eventId = nextEvent++, year = year, eventType = ScenarioManagementEventType.WorkResolved,
                outcome = fail ? ScenarioManagementOutcome.Failed : ScenarioManagementOutcome.Succeeded,
                workOrderId = orderId, taskType = type, targetTreeId = tree, speciesId = species, cellIndex = cell, worldPosition = pos,
                executionMethod = method, ecologicalTreatment = fail ? ScenarioEcologicalTreatment.None : treatment,
                biologicalVolumeM3 = volume, contractorCostCents = contractor, stockCostCents = stock, ownerMinutes = ownerMinutes,
                failureReason = fail ? failed : ""
            };
            Events.Add(e);
            return e;
        }

        // OrderCancelled events are written by RecordOrderEvent: target, species, cell and position of the order, no method.
        private void Cancel(int year, int orderId, ScenarioWorkType type, string tree, string species = "", int cell = -1, Vector3 pos = default(Vector3))
        {
            Events.Add(new ScenarioManagementEvent
            {
                eventId = nextEvent++, year = year, eventType = ScenarioManagementEventType.OrderCancelled, outcome = ScenarioManagementOutcome.Cancelled,
                workOrderId = orderId, taskType = type, targetTreeId = tree, speciesId = species, cellIndex = cell, worldPosition = pos
            });
        }

        private void Advance(int year, ScenarioAnnualReport report)
        {
            Events.Add(new ScenarioManagementEvent { eventId = nextEvent++, year = year, eventType = ScenarioManagementEventType.YearAdvanced, outcome = ScenarioManagementOutcome.Succeeded });
            Reports.Add(report);
            Snapshots.Add(new ScenarioEcologicalSnapshot { year = year, livingTrees = 300 - year });
        }
    }
}
