using System;
using System.Collections.Generic;
using System.Linq;
using CCF.Forestry.WorkEconomy;

// S1-C per-year management history (T-01): a read-only query layer over the same persisted records the derived
// intervention history already uses (management events, annual reports, ecological snapshots, deadwood records, work
// orders and storm records). Everything here is computed on demand. Nothing is stored, so there is no save field and no
// schema change, and reading history cannot mutate the world. The layer exists so a later Annual Review year browser
// can show every completed year without re-deriving raw record relationships in the UI.
//
// This extends the existing derived history architecture (ScenarioInterventionHistory) rather than replacing it: the
// intervention summary stays the higher-level projection of resolved plans, and this file exposes the underlying
// per-year records with provenance classification. Shared parsing helpers live on ScenarioInterventionHistory.
//
// Layer classification (approved S1-C spec separates recorded fact from derived comparison):
//   HistoryLayer.Recorded — a fact persisted at the time it happened (events, orders, annual report, snapshots, storm
//     records, deadwood records). Passed through unchanged, stable IDs included.
//   HistoryLayer.Derived  — computed from those records at query time (before/after deltas, money aggregation,
//     grouped-harvest settlement attribution, other-death counts). Labelled as derived, never stored, never presented
//     as recorded fact.
//
// Money honesty (approved S1-C FR-9): the annual report is the per-year money authority. A commissioned harvest is one
// grouped job that settles once; the whole settlement is recorded on the last resolved harvest event of that year
// (ScenarioOneManager.SettleHarvestJob overwrites that event's money fields). There is no authoritative per-order money
// inside the group, and harvestJobId is the resolution year, not a unique job identity. This layer surfaces the
// settlement as grouped metadata and never fabricates a per-order allocation.
//
// Year convention (ScenarioOneManager.AdvanceYear): work approved while the stand is at year Y-1 executes in the
// advance into year Y. Its events and annual report carry year Y, the "before" snapshot is the one recorded at year
// Y-1 and the "after" snapshot at year Y. A completed year is one that has an annual report and/or a YearAdvanced
// event.

public enum HistoryLayer
{
    Recorded = 1,
    Derived = 3
}

// A cancellation: the order object was deleted when it was cancelled, so the OrderCancelled event is the only surviving
// record of it (L1).
public sealed class HistoryCancellation
{
    public ScenarioManagementEvent Event;
    public int WorkOrderId;
    public string TargetTreeId = "";
    public ScenarioWorkType TaskType;
    public int CellIndex = -1;
    public int Year;
    public bool OrderStillExists;
}

// One resolved task: its WorkResolved event plus the order record when it still exists.
public sealed class ResolvedWorkRecord
{
    public ScenarioManagementEvent Event;
    public ScenarioOneWorkOrder Order;
    public bool OrderStillExists;

    // True when this event is a harvest task inside a year that settled one grouped harvest job. The event's own money
    // fields then carry either the grouped settlement (on the settlement event) or zeros (on the earlier events of the
    // group) — in both cases NOT authoritative per-order money. Planting/pruning/removal events keep their per-order
    // meaning and are false here.
    public bool MoneyIsGroupedSettlement;
}

// Deterministic before/after comparison of the two recorded ecological snapshots either side of the year (L3).
public sealed class SnapshotDelta
{
    public HistoryLayer Layer => HistoryLayer.Derived;
    public ScenarioEcologicalSnapshot Before;
    public ScenarioEcologicalSnapshot After;
    public int LivingTrees;
    public float MeanCanopy;
    public float MeanLight;
    public int OccupiedRegenerationCells;
    public float DeadwoodVolumeM3;
    public int DeadwoodCount;
}

// Per-year money (L3 aggregation over L1 records). The annual report is the authority; event sums are supporting
// detail only.
public sealed class YearMoneySummary
{
    public const string AuthorityNote =
        "Annual-report totals are the per-year money authority (S1-C FR-9). Event sums are supporting detail.";

    public HistoryLayer Layer => HistoryLayer.Derived;
    public ScenarioAnnualReport Authority;
    public long ContractorCostCents;
    public long TimberRevenueCents;
    public long HarvestMinimumAdjustmentCents;
    public long ClosingCashCents;
    public int OwnerMinutes;
    public long StockCostCents;
    public readonly List<ScenarioTimberSale> Sales = new List<ScenarioTimberSale>();
}

// The grouped settlement of the year's commissioned harvest, surfaced as metadata rather than per-order money (L3).
public sealed class GroupedHarvestSettlement
{
    public const string AttributionNote =
        "A commissioned harvest settles as one grouped job. The settlement is recorded on the last resolved harvest "
        + "event of the year; there is no authoritative per-order money inside the group, and the job label is the "
        + "resolution year, not a unique job identity.";

    public HistoryLayer Layer => HistoryLayer.Derived;

    // harvestJobId semantics: the resolution year. Deliberately named to stop it being read as a unique job id.
    public int ResolutionYearLabel;

    // The event SettleHarvestJob wrote the whole settlement onto (the last resolved harvest event of the year).
    public int SettlementEventId = -1;
    public long SettlementRevenueCents;
    public long SettlementContractorCostCents;

    // Harvest orders of that year's group that still exist as records. Derived from the events plus order harvestJobId
    // where available; for older saves without harvestJobId every resolved harvest order of the year belongs to the
    // single year group.
    public readonly List<int> GroupedOrderIds = new List<int>();
}

// S1-C T-03 drill-down. How one task of a completed year ended. Cancelled means the order was cancelled before it could be
// resolved: the order object is gone, so such an item is built from its OrderCancelled event alone.
public enum WorkItemStatus
{
    Succeeded = 1,
    Failed = 2,
    Cancelled = 3
}

// One recorded task (a resolved task or a cancellation) with exactly the target facts its event persisted. Nothing here is
// reconstructed from current state: an empty tree id, a negative cell index or PositionRecorded == false means "not
// recorded", and the reader must say so rather than fill the gap (S1-C FR-4).
public sealed class WorkItemDetail
{
    public HistoryLayer Layer => HistoryLayer.Recorded;

    public int EventId;
    public int WorkOrderId;
    public ScenarioWorkType TaskType;
    public WorkItemStatus Status;
    // The year stamped on the event: the resolution year for resolved work, the year it was cancelled in for a cancellation.
    public int Year;

    public string TargetTreeId = "";
    public string SpeciesId = "";
    public int CellIndex = -1;
    // The event keeps a world position as an unnamed default of (0, 0, 0) when none was written. That default is treated as
    // "not recorded"; a position genuinely recorded at the exact origin would read as not recorded, which errs honest.
    public bool PositionRecorded;
    public float PositionX, PositionY, PositionZ;

    // Resolution events carry the order's execution method. Cancellation events never do, so for a cancellation the method
    // is not recorded and ExecutionMethod must not be read.
    public bool ExecutionMethodRecorded;
    public WorkExecutionMethod ExecutionMethod;

    public string FailureReason = "";
    public ScenarioEcologicalTreatment Treatment;
    public float BiologicalVolumeM3;
    public bool OrderStillExists;

    // Whether a planting had a shelter installed. Only the order record holds this, so it is known only while that order
    // still exists; otherwise ShelterRecorded is false and ShelterInstalled must not be read.
    public bool ShelterRecorded;
    public bool ShelterInstalled;

    // True for a successful harvest task of a year that settled one grouped harvest job. Such an item has NO money of its
    // own: the whole job is on YearWorkDetail.HarvestSettlement (S1-C FR-9).
    public bool IsGroupedHarvest;

    // Per-task money exists only for successful non-harvest work (planting, pruning, regeneration removal), where the
    // resolution event records that task's own cost. It is never set for grouped harvest, failed or cancelled items.
    public bool OwnMoneyRecorded;
    public long ContractorCostCents;
    public long StockCostCents;
    public int OwnerMinutes;
}

// The items of one kind of work in a year: aggregate figures first, the items themselves for drill-down.
public sealed class WorkGroupDetail
{
    // Meaningful for succeeded groups. The failed and cancelled groups mix task types.
    public ScenarioWorkType TaskType;
    public WorkItemStatus Status;
    public bool IsGroupedHarvest;
    public readonly List<WorkItemDetail> Items = new List<WorkItemDetail>();

    // Distinct recorded map squares across the items; items with no recorded cell are not counted.
    public int DistinctRecordedCells;
    // Recorded species only, ordered by species id (ordinal) so the grouping is deterministic. Items without a species
    // are not counted here.
    public readonly List<KeyValuePair<string, int>> SpeciesCounts = new List<KeyValuePair<string, int>>();

    public string Key => Status == WorkItemStatus.Succeeded ? "done-" + TaskType : Status == WorkItemStatus.Failed ? "failed" : "cancelled";
}

// Everything a year's Work card can drill into. Built from one YearHistoryView only.
public sealed class YearWorkDetail
{
    public int Year;
    // Successful work, one group per task type, in task-type order.
    public readonly List<WorkGroupDetail> Done = new List<WorkGroupDetail>();
    public WorkGroupDetail Failed = new WorkGroupDetail { Status = WorkItemStatus.Failed };
    public WorkGroupDetail Cancelled = new WorkGroupDetail { Status = WorkItemStatus.Cancelled };
    // The single grouped harvest settlement of the year, shared with YearHistoryView. Null when no harvest task succeeded.
    public GroupedHarvestSettlement HarvestSettlement;

    public WorkGroupDetail DoneFor(ScenarioWorkType type) => Done.FirstOrDefault(g => g.TaskType == type);
}

// One completed year of management history. Recorded members are passed through unchanged; derived members are
// computed at query time and labelled through HistoryLayer.
public sealed class YearHistoryView
{
    public int Year;

    // --- L1 recorded facts ---

    // Per-year money authority. Null only if the report is missing (defensive: the year still shows its events).
    public ScenarioAnnualReport Report;
    public bool YearAdvancedRecorded;

    // Every management event stamped with this year, in eventId order: order creation/approval/cancellation/resolution,
    // stock purchases, year advance and any scenario outcome events.
    public readonly List<ScenarioManagementEvent> Events = new List<ScenarioManagementEvent>();

    // Resolved work paired with its order record (order included only where it still exists).
    public readonly List<ResolvedWorkRecord> ResolvedWork = new List<ResolvedWorkRecord>();

    // Orders that still exist and were resolved in this year (resolvedYear == Year), whatever their status.
    public readonly List<ScenarioOneWorkOrder> ResolvedOrders = new List<ScenarioOneWorkOrder>();

    // Cancellations of this year. The order object is gone; the event is the only surviving record.
    public readonly List<HistoryCancellation> Cancellations = new List<HistoryCancellation>();

    public readonly List<StormEventRecord> Storms = new List<StormEventRecord>();

    // Deadwood records stamped with this year's fall year: felling-retained stems and biological deaths alike.
    public readonly List<ScenarioDeadwoodRecord> DeadwoodFallen = new List<ScenarioDeadwoodRecord>();

    // Snapshot recorded at the end of this year, and the most recent one recorded before it. Either may be null.
    public ScenarioEcologicalSnapshot Snapshot;
    public ScenarioEcologicalSnapshot PreviousSnapshot;

    // --- L3 derived at query time ---

    public SnapshotDelta Delta;
    public YearMoneySummary Money = new YearMoneySummary();
    public GroupedHarvestSettlement HarvestSettlement;

    // Trees recorded as falling this year that this year's plan did not fell (self-thinning, windthrow or other
    // biological death; the records do not say which).
    public int OtherDeathsInYear;

    // --- convenience groupings over Events (mechanical filters of L1 facts) ---

    public IEnumerable<ScenarioManagementEvent> OrderCreatedEvents =>
        Events.Where(e => e.eventType == ScenarioManagementEventType.OrderCreated);
    public IEnumerable<ScenarioManagementEvent> OrderApprovedEvents =>
        Events.Where(e => e.eventType == ScenarioManagementEventType.OrderApproved);
    public IEnumerable<ScenarioManagementEvent> WorkResolvedEvents =>
        Events.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved);
    public IEnumerable<ScenarioManagementEvent> StockPurchaseEvents =>
        Events.Where(e => e.eventType == ScenarioManagementEventType.StockPurchased);
    public bool HasCompletedRecord => Report != null || YearAdvancedRecorded;
}

public static class ScenarioManagementHistory
{
    // One view per completed year, oldest first. Pure: the records are only read.
    public static List<YearHistoryView> BuildYearViews(
        IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioAnnualReport> reports,
        IReadOnlyList<ScenarioEcologicalSnapshot> snapshots,
        IReadOnlyList<ScenarioDeadwoodRecord> deadwood,
        IReadOnlyList<ScenarioOneWorkOrder> orders,
        IReadOnlyList<StormEventRecord> storms)
    {
        var views = new List<YearHistoryView>();
        foreach (int year in CompletedYears(events, reports))
            views.Add(BuildYearView(events, reports, snapshots, deadwood, orders, storms, year));
        return views;
    }

    // The view for one completed year, or null when that year was never completed.
    public static YearHistoryView BuildYearView(
        IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioAnnualReport> reports,
        IReadOnlyList<ScenarioEcologicalSnapshot> snapshots,
        IReadOnlyList<ScenarioDeadwoodRecord> deadwood,
        IReadOnlyList<ScenarioOneWorkOrder> orders,
        IReadOnlyList<StormEventRecord> storms,
        int year)
    {
        if (!CompletedYears(events, reports).Contains(year)) return null;
        events = events ?? Array.Empty<ScenarioManagementEvent>();
        reports = reports ?? Array.Empty<ScenarioAnnualReport>();
        snapshots = snapshots ?? Array.Empty<ScenarioEcologicalSnapshot>();
        deadwood = deadwood ?? Array.Empty<ScenarioDeadwoodRecord>();
        orders = orders ?? Array.Empty<ScenarioOneWorkOrder>();
        storms = storms ?? Array.Empty<StormEventRecord>();

        var view = new YearHistoryView { Year = year };
        view.Report = reports.FirstOrDefault(r => r != null && r.year == year);
        foreach (ScenarioManagementEvent entry in events.Where(e => e != null && e.year == year).OrderBy(e => e.eventId))
        {
            view.Events.Add(entry);
            if (entry.eventType == ScenarioManagementEventType.YearAdvanced) view.YearAdvancedRecorded = true;
        }

        var orderById = new Dictionary<int, ScenarioOneWorkOrder>();
        foreach (ScenarioOneWorkOrder order in orders)
            if (order != null && !orderById.ContainsKey(order.workOrderId)) orderById.Add(order.workOrderId, order);

        foreach (ScenarioManagementEvent entry in view.Events)
        {
            if (entry.eventType == ScenarioManagementEventType.WorkResolved)
            {
                orderById.TryGetValue(entry.workOrderId, out ScenarioOneWorkOrder order);
                view.ResolvedWork.Add(new ResolvedWorkRecord
                {
                    Event = entry,
                    Order = order,
                    OrderStillExists = order != null,
                    MoneyIsGroupedSettlement = false
                });
            }
            else if (entry.eventType == ScenarioManagementEventType.OrderCancelled)
            {
                orderById.TryGetValue(entry.workOrderId, out ScenarioOneWorkOrder order);
                view.Cancellations.Add(new HistoryCancellation
                {
                    Event = entry,
                    WorkOrderId = entry.workOrderId,
                    TargetTreeId = entry.targetTreeId ?? "",
                    TaskType = entry.taskType,
                    CellIndex = entry.cellIndex,
                    Year = year,
                    OrderStillExists = order != null
                });
            }
        }

        foreach (ScenarioOneWorkOrder order in orders)
            if (order != null && order.resolvedYear == year) view.ResolvedOrders.Add(order);
        foreach (StormEventRecord storm in storms)
            if (storm != null && storm.year == year) view.Storms.Add(storm);
        foreach (ScenarioDeadwoodRecord record in deadwood)
            if (record != null && record.fallenYear == year) view.DeadwoodFallen.Add(record);

        view.Snapshot = snapshots.FirstOrDefault(s => s != null && s.year == year);
        view.PreviousSnapshot = snapshots.LastOrDefault(s => s != null && s.year < year);
        view.Delta = BuildDelta(view.PreviousSnapshot, view.Snapshot);
        view.Money = BuildMoney(view.Report, view.Events);
        view.HarvestSettlement = BuildHarvestSettlement(view);
        foreach (ResolvedWorkRecord record in view.ResolvedWork)
            record.MoneyIsGroupedSettlement = view.HarvestSettlement != null && IsHarvestTask(record.Event.taskType);
        view.OtherDeathsInYear = ScenarioInterventionHistory.OtherDeaths(
            deadwood, ScenarioInterventionHistory.FelledTargetsByYear(events), year, year);
        return view;
    }

    // Drill-down detail for the selected year's Work card (S1-C T-03). Pure and read-only: it reads only the YearHistoryView,
    // which already holds the year's events and orders, so no caller re-joins raw record lists. Ordering is deterministic:
    // items by event id, succeeded groups by task type.
    //
    // Money honesty (FR-9): successful harvest tasks are marked IsGroupedHarvest and carry no money; the year's one grouped
    // settlement is HarvestSettlement. harvestJobId and per-order money inside the group are never read here.
    public static YearWorkDetail DescribeWork(YearHistoryView year)
    {
        if (year == null) return null;
        var detail = new YearWorkDetail { Year = year.Year, HarvestSettlement = year.HarvestSettlement };

        foreach (ResolvedWorkRecord record in year.ResolvedWork.OrderBy(r => r.Event.eventId))
        {
            WorkItemDetail item = FromResolution(record, year.HarvestSettlement != null);
            if (item.Status == WorkItemStatus.Succeeded)
            {
                WorkGroupDetail group = detail.DoneFor(item.TaskType);
                if (group == null)
                {
                    group = new WorkGroupDetail
                    {
                        TaskType = item.TaskType,
                        Status = WorkItemStatus.Succeeded,
                        IsGroupedHarvest = item.IsGroupedHarvest
                    };
                    detail.Done.Add(group);
                }
                group.Items.Add(item);
            }
            else detail.Failed.Items.Add(item);
        }
        detail.Done.Sort((a, b) => a.TaskType.CompareTo(b.TaskType));

        foreach (HistoryCancellation cancellation in year.Cancellations.OrderBy(c => c.Event.eventId))
            detail.Cancelled.Items.Add(FromCancellation(cancellation));

        foreach (WorkGroupDetail group in detail.Done) Summarise(group);
        Summarise(detail.Failed);
        Summarise(detail.Cancelled);
        return detail;
    }

    private static WorkItemDetail FromResolution(ResolvedWorkRecord record, bool yearHasHarvestSettlement)
    {
        ScenarioManagementEvent e = record.Event;
        bool succeeded = e.outcome == ScenarioManagementOutcome.Succeeded;
        bool harvest = IsHarvestTask(e.taskType);
        WorkItemDetail item = NewItem(e, succeeded ? WorkItemStatus.Succeeded : WorkItemStatus.Failed);
        item.ExecutionMethodRecorded = true;
        item.ExecutionMethod = e.executionMethod;
        item.FailureReason = succeeded ? "" : e.failureReason ?? "";
        item.Treatment = e.ecologicalTreatment;
        item.BiologicalVolumeM3 = succeeded ? e.biologicalVolumeM3 : 0f;
        item.OrderStillExists = record.OrderStillExists;
        if (record.OrderStillExists && e.taskType == ScenarioWorkType.PlantJuvenile)
        {
            item.ShelterRecorded = true;
            item.ShelterInstalled = record.Order.installShelter;
        }
        item.IsGroupedHarvest = succeeded && harvest && yearHasHarvestSettlement;
        if (succeeded && !harvest)
        {
            item.OwnMoneyRecorded = true;
            item.ContractorCostCents = e.contractorCostCents;
            item.StockCostCents = e.stockCostCents;
            item.OwnerMinutes = e.ownerMinutes;
        }
        return item;
    }

    // The order object was deleted on cancellation, so the event is the whole record. Its outcome fields are blank by
    // construction (nothing was resolved), and it never carried an execution method.
    private static WorkItemDetail FromCancellation(HistoryCancellation cancellation)
    {
        WorkItemDetail item = NewItem(cancellation.Event, WorkItemStatus.Cancelled);
        item.OrderStillExists = cancellation.OrderStillExists;
        return item;
    }

    private static WorkItemDetail NewItem(ScenarioManagementEvent e, WorkItemStatus status)
    {
        var item = new WorkItemDetail
        {
            EventId = e.eventId,
            WorkOrderId = e.workOrderId,
            TaskType = e.taskType,
            Status = status,
            Year = e.year,
            TargetTreeId = e.targetTreeId ?? "",
            SpeciesId = e.speciesId ?? "",
            CellIndex = e.cellIndex >= 0 ? e.cellIndex : -1
        };
        item.PositionX = e.worldPosition.x;
        item.PositionY = e.worldPosition.y;
        item.PositionZ = e.worldPosition.z;
        item.PositionRecorded = item.PositionX != 0f || item.PositionY != 0f || item.PositionZ != 0f;
        return item;
    }

    private static void Summarise(WorkGroupDetail group)
    {
        group.DistinctRecordedCells = group.Items.Where(i => i.CellIndex >= 0).Select(i => i.CellIndex).Distinct().Count();
        group.SpeciesCounts.Clear();
        foreach (var species in group.Items.Where(i => i.SpeciesId.Length > 0)
                     .GroupBy(i => i.SpeciesId).OrderBy(g => g.Key, StringComparer.Ordinal))
            group.SpeciesCounts.Add(new KeyValuePair<string, int>(species.Key, species.Count()));
    }

    // Years that have been completed: an annual report and/or a YearAdvanced event. Sorted, unique.
    public static List<int> CompletedYears(
        IReadOnlyList<ScenarioManagementEvent> events,
        IReadOnlyList<ScenarioAnnualReport> reports)
    {
        var years = new SortedSet<int>();
        if (reports != null)
            foreach (ScenarioAnnualReport report in reports)
                if (report != null) years.Add(report.year);
        if (events != null)
            foreach (ScenarioManagementEvent entry in events)
                if (entry != null && entry.eventType == ScenarioManagementEventType.YearAdvanced) years.Add(entry.year);
        return years.ToList();
    }

    // Harvest tasks are felling and salvage (same predicate as ScenarioOneManager.IsHarvestOrder).
    private static bool IsHarvestTask(ScenarioWorkType type)
        => type == ScenarioWorkType.FellTree || type == ScenarioWorkType.SalvageDeadwood;

    private static SnapshotDelta BuildDelta(ScenarioEcologicalSnapshot before, ScenarioEcologicalSnapshot after)
    {
        if (before == null || after == null) return null;
        return new SnapshotDelta
        {
            Before = before,
            After = after,
            LivingTrees = after.livingTrees - before.livingTrees,
            MeanCanopy = after.meanCanopy - before.meanCanopy,
            MeanLight = after.meanLight - before.meanLight,
            OccupiedRegenerationCells = after.occupiedRegenerationCells - before.occupiedRegenerationCells,
            DeadwoodVolumeM3 = after.deadwoodVolumeM3 - before.deadwoodVolumeM3,
            DeadwoodCount = after.deadwoodCount - before.deadwoodCount
        };
    }

    private static YearMoneySummary BuildMoney(ScenarioAnnualReport report, List<ScenarioManagementEvent> events)
    {
        var money = new YearMoneySummary { Authority = report };
        if (report != null)
        {
            money.ContractorCostCents = report.contractorCostCents;
            money.TimberRevenueCents = report.timberRevenueCents;
            money.HarvestMinimumAdjustmentCents = report.harvestMinimumAdjustmentCents;
            money.ClosingCashCents = report.closingCashCents;
            money.OwnerMinutes = report.ownerMinutes;
            if (report.timberSales != null)
                foreach (ScenarioTimberSale sale in report.timberSales)
                    if (sale != null)
                        money.Sales.Add(new ScenarioTimberSale
                        {
                            assortment = sale.assortment,
                            soldVolumeCm3 = sale.soldVolumeCm3,
                            revenueCents = sale.revenueCents
                        });
        }
        money.StockCostCents = events.Where(e => e.eventType == ScenarioManagementEventType.WorkResolved)
            .Sum(e => e.stockCostCents);
        return money;
    }

    // Reconstructs the grouped settlement metadata for the year's commissioned harvest (L3). The settlement is written
    // onto the last resolved harvest event of the year, so that event is identified from record order; the event's
    // money fields then describe the whole job. No per-order allocation is derived from it.
    private static GroupedHarvestSettlement BuildHarvestSettlement(YearHistoryView view)
    {
        List<ResolvedWorkRecord> harvest = view.ResolvedWork
            .Where(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded && IsHarvestTask(r.Event.taskType))
            .OrderBy(r => r.Event.eventId).ToList();
        if (harvest.Count == 0) return null;

        ResolvedWorkRecord settlement = harvest[harvest.Count - 1];
        var result = new GroupedHarvestSettlement
        {
            ResolutionYearLabel = view.Year,
            SettlementEventId = settlement.Event.eventId,
            SettlementRevenueCents = settlement.Event.timberRevenueCents,
            SettlementContractorCostCents = settlement.Event.contractorCostCents
        };
        foreach (ResolvedWorkRecord record in harvest)
        {
            bool grouped = !record.OrderStillExists || record.Order.harvestJobId == view.Year
                || record.Order.harvestJobId < 0;
            if (grouped && !result.GroupedOrderIds.Contains(record.Event.workOrderId))
                result.GroupedOrderIds.Add(record.Event.workOrderId);
        }
        return result;
    }
}
