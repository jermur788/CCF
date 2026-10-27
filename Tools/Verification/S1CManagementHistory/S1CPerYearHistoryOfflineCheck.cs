// Offline check for the S1-C per-year management history query layer (T-01). Compiles the PRODUCTION
// ScenarioManagementHistory.cs, ScenarioInterventionHistory.cs and the real persisted record classes (management
// events, annual reports, ecological snapshots, deadwood records, work orders, storm records) together with the real
// StormModel.cs and SimulationRandom.cs, and runs on the plain .NET SDK. Nothing here is used by the game.
//
// Proves the ten packet requirements: per-year coherence, event fidelity, cancellation-only history, resolvedYear
// association, annual-report money authority, grouped-harvest honesty, deterministic snapshot deltas, deterministic
// layer classification, query purity (no mutation, repeatable), and unchanged existing intervention-history behaviour.
//
// Usage: run_offline_check.py   Prints S1C_HISTORY_QUERY_* tokens.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using CCF.Forestry.WorkEconomy;

public static class S1CPerYearHistoryOfflineCheck
{
    private static int failures, checks;

    public static int Main()
    {
        Fixture f = Fixture.Build();
        PerYearCoherence(f);
        EventFidelity(f);
        CancellationHistory(f);
        ResolvedYearAssociation(f);
        MoneyAuthority(f);
        GroupedHarvestHonesty(f);
        DeltaDeterminism(f);
        LayerClassification(f);
        PurityAndRepeatability(f);
        ExistingBehaviourRegression(f);
        Console.WriteLine(failures == 0
            ? $"S1C_HISTORY_QUERY_PASS checks={checks}"
            : $"S1C_HISTORY_QUERY_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // ---- 1. one coherent view per completed year ----
    private static void PerYearCoherence(Fixture f)
    {
        List<YearHistoryView> views = ScenarioManagementHistory.BuildYearViews(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms);
        Check(views.Count == 5, "per-year view count", $"got {views.Count} want 5");
        Check(views.Select(v => v.Year).SequenceEqual(new[] { 1, 2, 3, 4, 5 }), "per-year oldest first",
            string.Join(",", views.Select(v => v.Year)));
        Check(views.All(v => v.HasCompletedRecord), "every view has a completed-year record");
        Check(views[0].Report != null && views[0].Report.year == 1, "year 1 carries its annual report");
        Check(views.All(v => v.YearAdvancedRecorded), "every completed year records YearAdvanced");
        Check(views[0].Storms.Count == 1 && views.Skip(1).All(v => v.Storms.Count == 0), "storm records only in their year");
    }

    // ---- 2. per-year event counts and types match the raw records ----
    private static void EventFidelity(Fixture f)
    {
        List<YearHistoryView> views = ScenarioManagementHistory.BuildYearViews(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms);
        bool counts = true, types = true;
        foreach (YearHistoryView view in views)
        {
            var raw = f.Events.Where(e => e.year == view.Year).ToList();
            if (view.Events.Count != raw.Count) counts = false;
            foreach (ScenarioManagementEventType type in Enum.GetValues(typeof(ScenarioManagementEventType)))
                if (view.Events.Count(e => e.eventType == type) != raw.Count(e => e.eventType == type)) types = false;
        }
        Check(counts, "per-year event counts match raw records");
        Check(types, "per-year event types match raw records");
        Check(views[0].OrderCreatedEvents.Count() == 2 && views[0].OrderApprovedEvents.Count() == 1
            && views[0].StockPurchaseEvents.Count() == 1, "creation/approval/stock events grouped for the UI");
    }

    // ---- 3. a cancelled order survives only in the event log ----
    private static void CancellationHistory(Fixture f)
    {
        YearHistoryView year1 = ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 1);
        Check(year1.Cancellations.Count == 1, "one cancellation in year 1");
        HistoryCancellation cancellation = year1.Cancellations[0];
        Check(cancellation.WorkOrderId == Fixture.CancelledOrderId && !cancellation.OrderStillExists,
            "cancelled order absent from orders but present as history");
        Check(cancellation.Event.eventType == ScenarioManagementEventType.OrderCancelled, "cancellation is the OrderCancelled event");
        Check(cancellation.TargetTreeId == "P0505" && cancellation.TaskType == ScenarioWorkType.FellTree,
            "cancellation keeps the recorded target and task type");
        Check(year1.ResolvedOrders.All(o => o.workOrderId != Fixture.CancelledOrderId), "cancelled order not in resolved orders");
    }

    // ---- 4. resolved retained orders associate with the correct resolvedYear ----
    private static void ResolvedYearAssociation(Fixture f)
    {
        List<YearHistoryView> views = ScenarioManagementHistory.BuildYearViews(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms);
        Check(views[1].ResolvedOrders.Any(o => o.workOrderId == Fixture.PlantedOrderId && o.resolvedYear == 2),
            "order planned in year 1 and resolved in year 2 belongs to year 2");
        Check(views[0].ResolvedOrders.All(o => o.workOrderId != Fixture.PlantedOrderId), "that order is not in year 1's resolved set");
        Check(views[0].OrderCreatedEvents.Any(e => e.workOrderId == Fixture.PlantedOrderId), "its creation event stays in year 1");
        ResolvedWorkRecord paired = views[1].ResolvedWork.Single(r => r.Event.workOrderId == Fixture.PlantedOrderId);
        Check(paired.OrderStillExists && paired.Order != null && paired.Event.targetTreeId == paired.Order.targetTreeId,
            "resolved work pairs event with its surviving order and passes IDs unchanged");
        Check(views[3].ResolvedWork.Any(r => !r.OrderStillExists && r.Event.workOrderId == Fixture.VanishedOrderId),
            "resolved event whose order record is gone is still shown");
    }

    // ---- 5. annual-report money is the per-year authority ----
    private static void MoneyAuthority(Fixture f)
    {
        YearHistoryView year1 = ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 1);
        Check(year1.Money.Layer == HistoryLayer.Derived, "money summary is labelled derived");
        Check(ReferenceEquals(year1.Money.Authority, year1.Report), "money summary points at the annual report");
        Check(year1.Money.ContractorCostCents == year1.Report.contractorCostCents
            && year1.Money.TimberRevenueCents == year1.Report.timberRevenueCents
            && year1.Money.HarvestMinimumAdjustmentCents == year1.Report.harvestMinimumAdjustmentCents
            && year1.Money.ClosingCashCents == year1.Report.closingCashCents
            && year1.Money.OwnerMinutes == year1.Report.ownerMinutes, "money values come from the report, not the events");
        Check(year1.Money.Sales.Count == year1.Report.timberSales.Count
            && year1.Money.Sales[0].revenueCents == year1.Report.timberSales[0].revenueCents, "timber sales copied from the report");
        Check(year1.Money.StockCostCents == 0, "year 1 stock cost is the event sum (0: stock was spent resolving year 2's planting)");
        YearHistoryView year2 = ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 2);
        Check(year2.Money.StockCostCents == 500 && year2.Money.Authority == year2.Report, "year 2 stock cost is its event sum");
    }

    // ---- 6. grouped-harvest settlement is not fabricated per order ----
    private static void GroupedHarvestHonesty(Fixture f)
    {
        YearHistoryView year1 = ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 1);
        GroupedHarvestSettlement settlement = year1.HarvestSettlement;
        Check(settlement != null && settlement.Layer == HistoryLayer.Derived, "grouped settlement exists and is labelled derived");
        Check(settlement.SettlementEventId == Fixture.SettlementEventId, "settlement sits on the last resolved harvest event",
            $"got {settlement.SettlementEventId} want {Fixture.SettlementEventId}");
        Check(settlement.SettlementRevenueCents == 180000 && settlement.SettlementContractorCostCents == 250000,
            "settlement carries the whole job's recorded money");
        Check(settlement.ResolutionYearLabel == 1, "job label is the resolution year");
        Check(settlement.GroupedOrderIds.Count == 2 && settlement.GroupedOrderIds.Contains(Fixture.FellOrderIdA)
            && settlement.GroupedOrderIds.Contains(Fixture.FellOrderIdB), "both felling orders are in the group");
        var felling = year1.ResolvedWork.Where(r => r.Event.taskType == ScenarioWorkType.FellTree).ToList();
        Check(felling.All(r => r.MoneyIsGroupedSettlement), "felling records flagged grouped-settlement money");
        Check(felling.First(r => r.Event.eventId != Fixture.SettlementEventId).Event.timberRevenueCents == 0
            && felling.First(r => r.Event.eventId != Fixture.SettlementEventId).Event.contractorCostCents == 0,
            "the earlier felling event keeps zero money: no per-order allocation is invented");
        Check(felling.First(r => r.Event.eventId == Fixture.SettlementEventId).Event.timberRevenueCents == 180000,
            "the settlement event carries the whole job, exactly as recorded");
        ResolvedWorkRecord planting = ScenarioManagementHistory
            .BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 2)
            .ResolvedWork.Single(r => r.Event.taskType == ScenarioWorkType.PlantJuvenile);
        Check(!planting.MoneyIsGroupedSettlement, "planting keeps per-order money meaning");
        Check(!settlement.GroupedOrderIds.Contains(Fixture.LegacyFellOrderId), "legacy year-4 felling is its own year group");
        YearHistoryView year4 = ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 4);
        Check(year4.HarvestSettlement != null && year4.HarvestSettlement.GroupedOrderIds.Contains(Fixture.LegacyFellOrderId),
            "legacy order with harvestJobId -1 still reads as its year's group");
    }

    // ---- 7. ecological before/after/delta references are deterministic ----
    private static void DeltaDeterminism(Fixture f)
    {
        YearHistoryView year2 = ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 2);
        Check(year2.Delta != null && year2.Delta.Layer == HistoryLayer.Derived, "delta present and labelled derived");
        Check(ReferenceEquals(year2.Delta.Before, f.Snapshots[1]) && ReferenceEquals(year2.Delta.After, f.Snapshots[2]),
            "delta references the recorded snapshots either side of the year");
        Check(year2.Delta.LivingTrees == f.Snapshots[2].livingTrees - f.Snapshots[1].livingTrees
            && Approx(year2.Delta.MeanCanopy, f.Snapshots[2].meanCanopy - f.Snapshots[1].meanCanopy)
            && year2.Delta.OccupiedRegenerationCells == f.Snapshots[2].occupiedRegenerationCells - f.Snapshots[1].occupiedRegenerationCells,
            "delta values are exact subtraction of the recorded snapshots");
        YearHistoryView again = ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 2);
        Check(DumpDelta(year2.Delta) == DumpDelta(again.Delta), "delta is deterministic across queries");
    }

    // ---- 8. layer/provenance classification is deterministic ----
    private static void LayerClassification(Fixture f)
    {
        List<YearHistoryView> views = ScenarioManagementHistory.BuildYearViews(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms);
        YearHistoryView year1 = views[0];
        Check(year1.Delta.Layer == HistoryLayer.Derived && year1.Money.Layer == HistoryLayer.Derived
            && year1.HarvestSettlement.Layer == HistoryLayer.Derived, "derived members classify as Derived");
        Check(ReferenceEquals(year1.Report, f.Reports[0])
            && year1.Events.All(e => f.Events.Contains(e))
            && ReferenceEquals(year1.Storms[0], f.Storms[0])
            && year1.DeadwoodFallen.All(d => f.Deadwood.Contains(d))
            && year1.ResolvedOrders.All(o => f.Orders.Contains(o)), "recorded members are pass-through record references");
        Check(year1.Report != null && year1.Delta != null, "a year can show recorded fact and derived comparison together");
        string first = DumpLayers(year1), second = DumpLayers(ScenarioManagementHistory.BuildYearView(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms, 1));
        Check(first == second, "classification is deterministic");
    }

    // ---- 9. two consecutive queries agree and change nothing ----
    private static void PurityAndRepeatability(Fixture f)
    {
        string before = DumpRecords(f);
        List<YearHistoryView> one = ScenarioManagementHistory.BuildYearViews(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms);
        List<YearHistoryView> two = ScenarioManagementHistory.BuildYearViews(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders, f.Storms);
        string after = DumpRecords(f);
        Check(before == after, "query construction mutates no persisted record");
        Check(DumpViews(one) == DumpViews(two), "two consecutive queries are equivalent");
    }

    // ---- 10. existing intervention-history behaviour is unchanged ----
    private static void ExistingBehaviourRegression(Fixture f)
    {
        List<InterventionSummary> interventions = ScenarioInterventionHistory.Build(f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders);
        Check(interventions.Count == 4, "Build: four resolved plans", $"got {interventions.Count}");
        Check(interventions.Select(i => i.Year).SequenceEqual(new[] { 1, 2, 4, 5 }), "Build: intervention years unchanged",
            string.Join(",", interventions.Select(i => i.Year)));
        Check(interventions[0].Ordinal == 1 && interventions[0].Felled == 2 && interventions[0].Pruned == 1
            && interventions[0].Planted == 0, "Build: year-1 counts unchanged");
        Check(interventions[0].RevenueCents == 180000 && interventions[0].CostCents == 250000 + 1800,
            "Build: money projection unchanged");
        Check(interventions[1].Planted == 1 && interventions[1].PlantedSheltered == 1 && interventions[1].CostCents == 500,
            "Build: year-2 planting projection unchanged");
        List<StaleOrderNote> stale = ScenarioInterventionHistory.StaleOrders(f.Orders, 5);
        Check(stale.Count == 1 && stale[0].Order.workOrderId == 900 && stale[0].AgeYears == 4, "StaleOrders unchanged");
        CompletionSummary completion = ScenarioInterventionHistory.Completion(5, Fixture.StartingCash, f.Events, f.Reports, f.Snapshots, f.Deadwood, f.Orders);
        Check(completion.Year == 5 && completion.TotalFelled == 3 && completion.TotalPruned == 3 && completion.TotalPlanted == 1,
            "Completion totals unchanged");
        Check(ScenarioInterventionHistory.NextStepHint(new List<InterventionSummary>(), 1, 0, false, -1, 100)
            .StartsWith("No intervention yet"), "NextStepHint: empty history unchanged");
        Check(ScenarioInterventionHistory.NextStepHint(interventions, 5, 0, false, -1, 100)
            .StartsWith("Intervention 4 was carried out in year 5"), "NextStepHint: recent intervention unchanged");
    }

    // ---- helpers ----

    private static bool Approx(float a, float b) => Math.Abs(a - b) < 0.0001f;

    private static string DumpDelta(SnapshotDelta d) => d == null ? "null"
        : $"{d.Before.year}->{d.After.year}:{d.LivingTrees}:{d.MeanCanopy:0.0000}:{d.MeanLight:0.0000}:{d.OccupiedRegenerationCells}:{d.DeadwoodVolumeM3:0.0000}:{d.DeadwoodCount}";

    private static string DumpLayers(YearHistoryView v)
        => $"{v.Delta.Layer}|{v.Money.Layer}|{v.HarvestSettlement?.Layer.ToString() ?? "none"}";

    private static string DumpViews(List<YearHistoryView> views)
    {
        var text = new StringBuilder();
        foreach (YearHistoryView v in views)
        {
            text.Append(v.Year).Append('|').Append(v.Events.Count).Append('|').Append(DumpDelta(v.Delta)).Append('|')
                .Append(v.Money.ContractorCostCents).Append(',').Append(v.Money.TimberRevenueCents).Append(',')
                .Append(v.Money.StockCostCents).Append('|').Append(v.OtherDeathsInYear).Append('|')
                .Append(v.HarvestSettlement == null ? "none"
                    : $"{v.HarvestSettlement.SettlementEventId}:{v.HarvestSettlement.SettlementRevenueCents}:{string.Join(",", v.HarvestSettlement.GroupedOrderIds.OrderBy(i => i))}")
                .Append('|').Append(string.Join(",", v.Cancellations.Select(c => c.WorkOrderId)))
                .Append('|').Append(string.Join(",", v.ResolvedOrders.Select(o => o.workOrderId)))
                .AppendLine();
        }
        return text.ToString();
    }

    // Canonical field dump of every persisted record the query reads, to prove the query changes none of them.
    private static string DumpRecords(Fixture f)
    {
        var text = new StringBuilder();
        foreach (ScenarioManagementEvent e in f.Events)
            text.Append("E;").Append(e.eventId).Append(';').Append(e.year).Append(';').Append(e.eventType).Append(';')
                .Append(e.outcome).Append(';').Append(e.workOrderId).Append(';').Append(e.taskType).Append(';')
                .Append(e.targetTreeId).Append(';').Append(e.timberRevenueCents).Append(';').Append(e.contractorCostCents)
                .Append(';').Append(e.cashDeltaCents).Append(';').Append(e.stockCostCents).Append(';').Append(e.failureReason).AppendLine();
        foreach (ScenarioAnnualReport r in f.Reports)
            text.Append("R;").Append(r.year).Append(';').Append(r.contractorCostCents).Append(';').Append(r.timberRevenueCents)
                .Append(';').Append(r.closingCashCents).Append(';').Append(r.harvestMinimumAdjustmentCents).Append(';').Append(r.ownerMinutes).AppendLine();
        foreach (ScenarioEcologicalSnapshot s in f.Snapshots)
            text.Append("S;").Append(s.year).Append(';').Append(s.livingTrees).Append(';').Append(s.meanCanopy.ToString("0.0000", CultureInfo.InvariantCulture))
                .Append(';').Append(s.occupiedRegenerationCells).Append(';').Append(s.deadwoodVolumeM3.ToString("0.0000", CultureInfo.InvariantCulture)).AppendLine();
        foreach (ScenarioDeadwoodRecord d in f.Deadwood)
            text.Append("D;").Append(d.deadwoodId).Append(';').Append(d.treeId).Append(';').Append(d.fallenYear).AppendLine();
        foreach (ScenarioOneWorkOrder o in f.Orders)
            text.Append("O;").Append(o.workOrderId).Append(';').Append(o.type).Append(';').Append(o.status).Append(';')
                .Append(o.targetTreeId).Append(';').Append(o.createdYear).Append(';').Append(o.resolvedYear).Append(';')
                .Append(o.harvestJobId).Append(';').Append(o.validationMessage).AppendLine();
        foreach (StormEventRecord s in f.Storms)
            text.Append("T;").Append(s.year).Append(';').Append(s.severity.ToString("0.0000", CultureInfo.InvariantCulture))
                .Append(';').Append(s.directionDegrees.ToString("0.0000", CultureInfo.InvariantCulture)).Append(';').Append(s.cropTreesLost).AppendLine();
        return text.ToString();
    }

    // A small world the way ScenarioOneManager records it: two plans with a grouped harvest settlement, an idle year
    // with a planting resolution, an all-failed year, a legacy-shaped single felling (harvestJobId -1), a final pruning
    // plan, a cancelled order, a resolved order whose record is gone, natural deaths and one storm. Event ids are
    // explicit so the settlement position is readable.
    private sealed class Fixture
    {
        public const long StartingCash = 1200000;
        public const int CancelledOrderId = 70, FellOrderIdA = 71, FellOrderIdB = 72, PlantedOrderId = 73,
            PrunedOrderId = 74, LegacyFellOrderId = 80, VanishedOrderId = 81, PruneYear5OrderId = 95,
            SettlementEventId = 7;

        public readonly List<ScenarioManagementEvent> Events = new List<ScenarioManagementEvent>();
        public readonly List<ScenarioAnnualReport> Reports = new List<ScenarioAnnualReport>();
        public readonly List<ScenarioEcologicalSnapshot> Snapshots = new List<ScenarioEcologicalSnapshot>();
        public readonly List<ScenarioDeadwoodRecord> Deadwood = new List<ScenarioDeadwoodRecord>();
        public readonly List<ScenarioOneWorkOrder> Orders = new List<ScenarioOneWorkOrder>();
        public readonly List<StormEventRecord> Storms = new List<StormEventRecord>();

        private int nextDeadwood = 1;

        public static Fixture Build()
        {
            var f = new Fixture();
            f.Snapshot(0, 336, 0.55f, 0.20f, 0, 0f);

            // Orders of the year-1 plan (approved before the advance into year 1). The cancelled order 70 deliberately
            // has NO order record: cancellation deletes the order object and only its events survive (ids 1-2 below).
            f.Order(FellOrderIdA, ScenarioWorkType.FellTree, "P0101", created: 0, resolved: 1, harvestJob: 1,
                ScenarioWorkStatus.Completed);
            f.Order(FellOrderIdB, ScenarioWorkType.FellTree, "P0102", created: 0, resolved: 1, harvestJob: 1,
                ScenarioWorkStatus.Completed);
            f.Order(PrunedOrderId, ScenarioWorkType.PruneTree, "P0202", created: 0, resolved: 1, harvestJob: -1,
                ScenarioWorkStatus.Completed);
            f.Order(PlantedOrderId, ScenarioWorkType.PlantJuvenile, "", created: 1, resolved: 2, harvestJob: -1,
                ScenarioWorkStatus.Completed, species: "sessile-oak", shelter: true);

            // Year 1 events (ids 1-9). The grouped harvest settlement lands on event 7, the LAST resolved harvest event
            // of the year (SettleHarvestJob overwrites that event's money fields with the whole job); the earlier
            // felling event keeps the zero money it recorded before settlement.
            f.Add(1, 1, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CancelledOrderId, ScenarioWorkType.FellTree, "P0505");
            f.Add(2, 1, ScenarioManagementEventType.OrderCancelled, ScenarioManagementOutcome.Cancelled, CancelledOrderId, ScenarioWorkType.FellTree, "P0505");
            f.Add(3, 1, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, PlantedOrderId, ScenarioWorkType.PlantJuvenile, "");
            f.Add(4, 1, ScenarioManagementEventType.OrderApproved, ScenarioManagementOutcome.None, PlantedOrderId, ScenarioWorkType.PlantJuvenile, "");
            f.Add(5, 1, ScenarioManagementEventType.StockPurchased, ScenarioManagementOutcome.None, 0, ScenarioWorkType.PlantJuvenile, "", stockCost: 500);
            ScenarioManagementEvent firstFell = f.Add(6, 1, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, FellOrderIdA, ScenarioWorkType.FellTree, "P0101");
            ScenarioManagementEvent settled = f.Add(7, 1, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, FellOrderIdB, ScenarioWorkType.FellTree, "P0102");
            f.Add(8, 1, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, PrunedOrderId, ScenarioWorkType.PruneTree, "P0202");
            f.Add(9, 1, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.Succeeded, 0, ScenarioWorkType.FellTree, "");
            firstFell.timberRevenueCents = 0; firstFell.contractorCostCents = 0; firstFell.cashDeltaCents = 0;
            settled.timberRevenueCents = 180000; settled.contractorCostCents = 250000;
            settled.cashDeltaCents = -70000; settled.closingCashCents = StartingCash - 70000;
            f.Reports.Add(new ScenarioAnnualReport
            {
                year = 1,
                completedTasks = 3,
                contractorCostCents = 250000 + 1800,
                timberRevenueCents = 180000,
                harvestMinimumAdjustmentCents = 60000,
                closingCashCents = StartingCash - 70000,
                ownerMinutes = 20
            });
            f.Reports[0].timberSales.Add(new ScenarioTimberSale { assortment = TimberAssortment.Sawlog, soldVolumeCm3 = 9000000, revenueCents = 150000 });
            f.Reports[0].timberSales.Add(new ScenarioTimberSale { assortment = TimberAssortment.Pulp, soldVolumeCm3 = 4000000, revenueCents = 30000 });
            f.Storms.Add(new StormEventRecord { year = 1, severity = 0.06f, directionDegrees = 90f, cropTreesLost = 2 });
            f.Deadwood.Add(f.Wood("P0101", 1));
            f.Deadwood.Add(f.Wood("P0102", 1));
            f.Snapshot(1, 332, 0.54f, 0.21f, 2, 0.6f);

            // Year 2: the planting planned in year 1 resolves here; two natural deaths recorded.
            f.Add(10, 2, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, PlantedOrderId, ScenarioWorkType.PlantJuvenile, "", stockCost: 500, species: "sessile-oak");
            f.Add(11, 2, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.Succeeded, 0, ScenarioWorkType.FellTree, "");
            f.Deadwood.Add(f.Wood("N2-0", 2));
            f.Deadwood.Add(f.Wood("N2-1", 2));
            f.Reports.Add(new ScenarioAnnualReport { year = 2, completedTasks = 1, closingCashCents = StartingCash - 70000 - 500, ownerMinutes = 5 });
            f.Snapshot(2, 330, 0.545f, 0.22f, 4, 1.2f);

            // Year 3: every task failed — not an intervention, but still a completed year with its report.
            f.Add(12, 3, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Failed, 90, ScenarioWorkType.FellTree, "GONE1", failure: "Target tree is missing or already felled.");
            f.Add(13, 3, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Failed, 91, ScenarioWorkType.PlantJuvenile, "", failure: "Planting cell is outside the stand.", species: "beech");
            f.Add(14, 3, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.Succeeded, 0, ScenarioWorkType.FellTree, "");
            f.Reports.Add(new ScenarioAnnualReport { year = 3, completedTasks = 0, failedTasks = 2, closingCashCents = StartingCash - 700500 });
            f.Snapshot(3, 328, 0.548f, 0.225f, 4, 1.5f);

            // Year 4: legacy-shaped single felling (harvestJobId -1, as migrated from pre-v15 saves) plus a resolved
            // event whose order record is gone.
            f.Order(LegacyFellOrderId, ScenarioWorkType.FellTree, "PO0303", created: 3, resolved: 4, harvestJob: -1,
                ScenarioWorkStatus.Completed);
            ScenarioManagementEvent legacySettle = f.Add(15, 4, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, LegacyFellOrderId, ScenarioWorkType.FellTree, "PO0303");
            legacySettle.timberRevenueCents = 70000; legacySettle.contractorCostCents = 250000;
            legacySettle.cashDeltaCents = -180000; legacySettle.closingCashCents = StartingCash - 700500 - 180000;
            f.Add(16, 4, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, VanishedOrderId, ScenarioWorkType.PruneTree, "P0404");
            f.Add(17, 4, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.Succeeded, 0, ScenarioWorkType.FellTree, "");
            f.Reports.Add(new ScenarioAnnualReport
            {
                year = 4,
                completedTasks = 2,
                contractorCostCents = 250000,
                timberRevenueCents = 70000,
                closingCashCents = StartingCash - 700500 - 180000
            });
            f.Deadwood.Add(f.Wood("PO0303", 4));
            f.Snapshot(4, 322, 0.53f, 0.24f, 5, 2.4f);

            // Year 5: one pruning plan.
            f.Order(PruneYear5OrderId, ScenarioWorkType.PruneTree, "P0808", created: 4, resolved: 5, harvestJob: -1,
                ScenarioWorkStatus.Completed);
            f.Add(18, 5, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, PruneYear5OrderId, ScenarioWorkType.PruneTree, "P0808");
            f.Add(19, 5, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.Succeeded, 0, ScenarioWorkType.FellTree, "");
            f.Reports.Add(new ScenarioAnnualReport { year = 5, completedTasks = 1, closingCashCents = StartingCash - 700500 - 180000 });
            f.Snapshot(5, 320, 0.532f, 0.245f, 5, 2.6f);

            // One order planned long ago and never resolved (stale).
            f.Orders.Add(new ScenarioOneWorkOrder
            {
                workOrderId = 900,
                type = ScenarioWorkType.FellTree,
                status = ScenarioWorkStatus.Pending,
                targetTreeId = "P0777",
                createdYear = 1
            });
            return f;
        }

        private ScenarioManagementEvent Add(int eventId, int year, ScenarioManagementEventType type,
            ScenarioManagementOutcome outcome, int orderId, ScenarioWorkType task, string tree,
            long stockCost = 0, string failure = "", string species = "")
        {
            var entry = new ScenarioManagementEvent
            {
                eventId = eventId,
                year = year,
                eventType = type,
                outcome = outcome,
                workOrderId = orderId,
                taskType = task,
                targetTreeId = tree,
                speciesId = species,
                stockCostCents = stockCost,
                failureReason = failure
            };
            Events.Add(entry);
            return entry;
        }

        private void Order(int id, ScenarioWorkType type, string tree, int created, int resolved, int harvestJob,
            ScenarioWorkStatus status, string species = "", bool shelter = false)
        {
            Orders.Add(new ScenarioOneWorkOrder
            {
                workOrderId = id,
                type = type,
                status = status,
                targetTreeId = tree,
                speciesId = species,
                createdYear = created,
                resolvedYear = resolved,
                harvestJobId = harvestJob,
                installShelter = shelter
            });
        }

        private ScenarioDeadwoodRecord Wood(string treeId, int year) => new ScenarioDeadwoodRecord
        {
            deadwoodId = "DW" + nextDeadwood++.ToString("0000"),
            treeId = treeId,
            speciesId = "sitka-spruce",
            fallenYear = year,
            originalVolumeM3 = 0.3f
        };

        private void Snapshot(int year, int living, float canopy, float light, int regen, float deadwood)
        {
            Snapshots.Add(new ScenarioEcologicalSnapshot
            {
                year = year,
                livingTrees = living,
                meanCanopy = canopy,
                meanLight = light,
                occupiedRegenerationCells = regen,
                deadwoodVolumeM3 = deadwood,
                deadwoodCount = year * 2
            });
        }
    }
}
