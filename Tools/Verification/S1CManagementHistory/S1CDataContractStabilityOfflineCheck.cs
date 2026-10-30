// Offline S1-C T-07 data-contract stability check. Compiles the PRODUCTION per-year history reader
// (ScenarioManagementHistory.cs), the real persisted record classes, the real ScenarioOneSaveMigration, the real
// ForestSaveValidation (so the accepted S1-D T-06 event-ID rule remains the only integrity authority) and the real
// StormModel, then runs on the plain .NET SDK. Nothing here is used by the game and nothing is written inside the
// repository.
//
// It proves the S1-C identity/data contract against two genuine saves and one deliberately constructed semantic case:
//   - genuine pre-S1-C v20-y5 save (source and digest recorded in the handoff);
//   - genuine recorded v22 S1-C save from the T-05 Unity evidence;
//   - a constructed cancellation/retention case that neither genuine save happens to contain.
//
// Proved: positive unique management-event IDs; event and work-order IDs pass through by reference and are never
// renumbered/re-keyed; work-order IDs survive a reparse (the offline reload analogue); resolution/cancellation linkage
// uses the surviving authoritative IDs; a cancelled order with no record is represented only by its event; repeated
// querying mutates no record, collection or next-ID counter; the reader owns no static cache/generator/store; a grouped
// harvest keeps harvestJobId as the resolution-year label and never invents per-order money; broader consumed
// identities (tree ids, cells, positions, species, objective records, snapshots, deadwood, storms) pass through; and the
// old save's newer provenance stays absent/not recorded rather than being synthesized.
//
// The production event-ID validator is tested separately with positive and duplicate/non-positive cases. The runner's
// --mutations mode additionally compiles deliberately broken copies of the reader and validator and requires this check
// to go red for each one.
//
// Usage: run_t07_gate.py --old-save ... --current-save ... --out ...
// Prints S1C_T07_OFFLINE_PASS or S1C_T07_OFFLINE_FAIL.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UnityEngine;

public sealed class T07SaveRoot
{
    public int version;
    public int ecologicalYear;
    public ScenarioOneSaveData scenarioOne;
}

public static class S1CDataContractStabilityOfflineCheck
{
    private static int checks, failures;
    private static string root;

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { IncludeFields = true };

    private sealed class SaveCase
    {
        public string Name;
        public string Path;
        public int Version;
        public int Year;
        public ScenarioOneSaveData Data;
        public bool RawHasObjectives;
        public bool RawHasPrescriptions;
        public bool RawHasEventPrescription;
        public bool RawHasOrderPrescription;
        public bool RawHasOrderRack;
        public int ExpectedEvents;
        public int ExpectedOrders;
        public int ExpectedNextEvent;
        public int ExpectedNextOrder;
        public int[] ExpectedYears;
        public int ExpectedAchievements;
    }

    public static int Main()
    {
        root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        string oldPath = Environment.GetEnvironmentVariable("CCF_T07_OLD_SAVE");
        string currentPath = Environment.GetEnvironmentVariable("CCF_T07_CURRENT_SAVE");
        if (string.IsNullOrEmpty(root)) { Fail("CCF_REPO_ROOT is not set"); return 1; }
        if (string.IsNullOrEmpty(oldPath)) { Fail("CCF_T07_OLD_SAVE is not set"); return 1; }
        if (string.IsNullOrEmpty(currentPath)) { Fail("CCF_T07_CURRENT_SAVE is not set"); return 1; }

        try
        {
            SaveCase oldSave = LoadCase(oldPath, "old-v20-y5", 20, 5, 17, 3, 18, 4, new[] { 1, 2, 3, 4, 5 }, 0);
            SaveCase currentSave = LoadCase(currentPath, "current-v22-y3", 22, 3, 10, 2, 11, 3, new[] { 1, 2, 3 }, 5);
            if (oldSave == null || currentSave == null) return Finish();

            CheckCaseBasics(oldSave);
            CheckCaseBasics(currentSave);
            OldSaveProvenance(oldSave);
            CurrentSaveProvenance(currentSave);
            IdentityContract(oldSave);
            IdentityContract(currentSave);
            HarvestSemantics(oldSave, new[] { (1, 3, 1, new[] { 1 }), (2, -1, -1, Array.Empty<int>()), (3, -1, -1, Array.Empty<int>()), (4, -1, -1, Array.Empty<int>()), (5, 15, 5, new[] { 2 }) });
            HarvestSemantics(currentSave, new[] { (1, -1, -1, Array.Empty<int>()), (2, 4, 2, new[] { 1 }), (3, -1, -1, Array.Empty<int>()) });
            SyntheticCancellationCase();
            ReloadStability(oldSave);
            ReloadStability(currentSave);
            Purity(oldSave);
            Purity(currentSave);
            ReaderHasNoSecondIdentityStore();
            DuplicateEventIdControl();

            Console.WriteLine($"S1C_T07_OLD_EVENT_IDS={string.Join(",", oldSave.Data.managementEvents.Select(e => e.eventId))}");
            Console.WriteLine($"S1C_T07_OLD_ORDER_IDS={string.Join(",", oldSave.Data.workOrders.Select(o => o.workOrderId))}");
            Console.WriteLine($"S1C_T07_CURRENT_EVENT_IDS={string.Join(",", currentSave.Data.managementEvents.Select(e => e.eventId))}");
            Console.WriteLine($"S1C_T07_CURRENT_ORDER_IDS={string.Join(",", currentSave.Data.workOrders.Select(o => o.workOrderId))}");
        }
        catch (Exception error)
        {
            Fail("unhandled exception while reading the contract: " + error.GetType().Name + ": " + error.Message);
        }
        return Finish();
    }

    private static int Finish()
    {
        Console.WriteLine(failures == 0
            ? $"S1C_T07_OFFLINE_PASS checks={checks}"
            : $"S1C_T07_OFFLINE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool condition, string label, string detail = "")
    {
        checks++;
        if (condition)
        {
            Console.WriteLine("PASS " + label + (detail.Length > 0 ? " " + detail : ""));
            return;
        }
        failures++;
        Console.WriteLine("FAIL " + label + (detail.Length > 0 ? " " + detail : ""));
    }

    private static void Fail(string label)
    {
        checks++;
        failures++;
        Console.WriteLine("FAIL " + label);
    }

    // ---- loading and production migration ----

    private static SaveCase LoadCase(string path, string name, int version, int year, int events, int orders,
        int nextEvent, int nextOrder, int[] years, int achievements)
    {
        if (!File.Exists(path)) { Fail(name + " fixture missing: " + path); return null; }
        string raw = File.ReadAllText(path);
        T07SaveRoot parsed;
        try
        {
            parsed = JsonSerializer.Deserialize<T07SaveRoot>(raw, JsonOptions);
        }
        catch (Exception error)
        {
            Fail(name + " fixture does not parse: " + error.Message);
            return null;
        }
        if (parsed == null || parsed.scenarioOne == null) { Fail(name + " fixture has no scenarioOne record"); return null; }

        var result = new SaveCase
        {
            Name = name,
            Path = path,
            Version = parsed.version,
            Year = parsed.ecologicalYear,
            Data = parsed.scenarioOne,
            ExpectedEvents = events,
            ExpectedOrders = orders,
            ExpectedNextEvent = nextEvent,
            ExpectedNextOrder = nextOrder,
            ExpectedYears = years,
            ExpectedAchievements = achievements
        };
        result.Data.managementEvents = result.Data.managementEvents ?? new List<ScenarioManagementEvent>();
        result.Data.workOrders = result.Data.workOrders ?? new List<ScenarioOneWorkOrder>();
        result.Data.annualReports = result.Data.annualReports ?? new List<ScenarioAnnualReport>();
        result.Data.ecologicalSnapshots = result.Data.ecologicalSnapshots ?? new List<ScenarioEcologicalSnapshot>();
        result.Data.deadwoodRecords = result.Data.deadwoodRecords ?? new List<ScenarioDeadwoodRecord>();
        result.Data.stormEvents = result.Data.stormEvents ?? new List<StormEventRecord>();
        result.Data.objectiveAchievements = result.Data.objectiveAchievements ?? new List<ScenarioObjectiveAchievement>();

        using (JsonDocument document = JsonDocument.Parse(raw))
        {
            JsonElement scenario = document.RootElement.GetProperty("scenarioOne");
            result.RawHasObjectives = scenario.TryGetProperty("objectiveAchievements", out _);
            result.RawHasPrescriptions = scenario.TryGetProperty("prescriptions", out _);
            result.RawHasEventPrescription = scenario.TryGetProperty("managementEvents", out JsonElement rawEvents)
                && rawEvents.ValueKind == JsonValueKind.Array
                && rawEvents.EnumerateArray().Any(e => e.TryGetProperty("prescriptionId", out _));
            result.RawHasOrderPrescription = scenario.TryGetProperty("workOrders", out JsonElement rawOrders)
                && rawOrders.ValueKind == JsonValueKind.Array
                && rawOrders.EnumerateArray().Any(o => o.TryGetProperty("prescriptionId", out _));
            result.RawHasOrderRack = scenario.TryGetProperty("workOrders", out JsonElement rawRackOrders)
                && rawRackOrders.ValueKind == JsonValueKind.Array
                && rawRackOrders.EnumerateArray().Any(o => o.TryGetProperty("rackId", out _));
        }

        ApplyProductionMigrations(result);
        return result;
    }

    // The supported load path runs these normalizers before the manager restores records. They may fill newer
    // default fields, but must never change an existing event ID, work-order ID or next-ID counter.
    private static void ApplyProductionMigrations(SaveCase saveCase)
    {
        ScenarioOneSaveData data = saveCase.Data;
        int[] eventIds = data.managementEvents.Where(e => e != null).Select(e => e.eventId).ToArray();
        int[] orderIds = data.workOrders.Where(o => o != null).Select(o => o.workOrderId).ToArray();
        int nextEvent = data.nextManagementEventId, nextOrder = data.nextWorkOrderId, nextDeadwood = data.nextDeadwoodId;

        ScenarioOneSaveMigration.NormalizeLegacy(data, saveCase.Version);
        ScenarioOneSaveMigration.NormalizeRackV2(data, saveCase.Version);
        ScenarioOneSaveMigration.NormalizeObjectiveHistory(data, saveCase.Version);
        ScenarioOneSaveMigration.NormalizePrescriptions(data, saveCase.Version);

        Check(data.managementEvents.Where(e => e != null).Select(e => e.eventId).SequenceEqual(eventIds),
            saveCase.Name + " production migration preserves event IDs");
        Check(data.workOrders.Where(o => o != null).Select(o => o.workOrderId).SequenceEqual(orderIds),
            saveCase.Name + " production migration preserves work-order IDs");
        Check(data.nextManagementEventId == nextEvent && data.nextWorkOrderId == nextOrder && data.nextDeadwoodId == nextDeadwood,
            saveCase.Name + " production migration preserves next-ID counters");
    }

    // ---- basic genuine-save identity facts ----

    private static void CheckCaseBasics(SaveCase saveCase)
    {
        Check(saveCase.Version == (saveCase.Name.StartsWith("old") ? 20 : 22), saveCase.Name + " save version", "version=" + saveCase.Version);
        Check(saveCase.Year == (saveCase.Name.StartsWith("old") ? 5 : 3), saveCase.Name + " ecological year", "year=" + saveCase.Year);
        Check(saveCase.Data.managementEvents.Count == saveCase.ExpectedEvents, saveCase.Name + " event count",
            "got " + saveCase.Data.managementEvents.Count + " want " + saveCase.ExpectedEvents);
        Check(saveCase.Data.workOrders.Count == saveCase.ExpectedOrders, saveCase.Name + " order count",
            "got " + saveCase.Data.workOrders.Count + " want " + saveCase.ExpectedOrders);
        Check(saveCase.Data.objectiveAchievements.Count == saveCase.ExpectedAchievements, saveCase.Name + " achievement count",
            "got " + saveCase.Data.objectiveAchievements.Count + " want " + saveCase.ExpectedAchievements);
        Check(saveCase.Data.nextManagementEventId == saveCase.ExpectedNextEvent, saveCase.Name + " next event counter",
            "got " + saveCase.Data.nextManagementEventId + " want " + saveCase.ExpectedNextEvent);
        Check(saveCase.Data.nextWorkOrderId == saveCase.ExpectedNextOrder, saveCase.Name + " next order counter",
            "got " + saveCase.Data.nextWorkOrderId + " want " + saveCase.ExpectedNextOrder);

        int[] eventIds = saveCase.Data.managementEvents.Select(e => e.eventId).ToArray();
        int[] orderIds = saveCase.Data.workOrders.Select(o => o.workOrderId).ToArray();
        Check(eventIds.All(id => id > 0), saveCase.Name + " every management-event ID is positive", string.Join(",", eventIds));
        Check(eventIds.Distinct().Count() == eventIds.Length, saveCase.Name + " management-event IDs are unique",
            "events=" + eventIds.Length + " distinct=" + eventIds.Distinct().Count());
        Check(orderIds.All(id => id > 0), saveCase.Name + " every work-order ID is positive", string.Join(",", orderIds));
        Check(orderIds.Distinct().Count() == orderIds.Length, saveCase.Name + " work-order IDs are unique",
            "orders=" + orderIds.Length + " distinct=" + orderIds.Distinct().Count());
        Check(saveCase.Data.nextManagementEventId > eventIds.DefaultIfEmpty(0).Max(), saveCase.Name + " next event counter is past every used event ID");
        Check(saveCase.Data.nextWorkOrderId > orderIds.DefaultIfEmpty(0).Max(), saveCase.Name + " next order counter is past every used order ID");

        int[] rawCompletedEvents = saveCase.Data.managementEvents.Where(e => saveCase.ExpectedYears.Contains(e.year)).Select(e => e.eventId).ToArray();
        List<YearHistoryView> views = BuildViews(saveCase.Data);
        Check(views.Select(v => v.Year).SequenceEqual(saveCase.ExpectedYears), saveCase.Name + " completed-year views",
            string.Join(",", views.Select(v => v.Year)));
        int[] viewEvents = views.SelectMany(v => v.Events).Select(e => e.eventId).ToArray();
        Check(viewEvents.SequenceEqual(rawCompletedEvents), saveCase.Name + " all completed-year event IDs appear exactly once in event order",
            "view=" + string.Join(",", viewEvents) + " raw=" + string.Join(",", rawCompletedEvents));
        Check(views.Sum(v => v.Events.Count) == rawCompletedEvents.Length, saveCase.Name + " no event is duplicated or dropped by the query");
    }

    private static void OldSaveProvenance(SaveCase saveCase)
    {
        Check(!saveCase.RawHasObjectives, "old save has no objectiveAchievements field in its source JSON");
        Check(saveCase.Data.objectiveAchievements.Count == 0, "old save objective history stays empty (not recorded)");
        Check(!saveCase.RawHasPrescriptions, "old save has no prescription records in its source JSON");
        Check(saveCase.Data.prescriptions.Count == 0, "old save prescription state stays empty after migration");
        Check(saveCase.Data.nextPrescriptionId == 1, "old save next-prescription counter stays the explicit default");
        Check(saveCase.Data.rackPlanningModel == 0 && saveCase.Data.racks.Count == 0, "old save rack V2 state stays legacy/empty after migration");
        Check(!saveCase.RawHasEventPrescription, "old save management events carry no prescriptionId field");
        Check(!saveCase.RawHasOrderPrescription, "old save work orders carry no prescriptionId field");
        Check(!saveCase.RawHasOrderRack, "old save work orders carry no rackId field");
        Check(saveCase.Data.managementEvents.All(e => e.prescriptionId == 0), "old save event prescription references parse as not recorded");
        Check(saveCase.Data.workOrders.All(o => o.prescriptionId == 0 && o.prescriptionCoverageUnitId.Length == 0
            && o.prescriptionCropTreeId.Length == 0 && o.prescriptionReason == 0 && o.rackId == 0),
            "old save order prescription/rack provenance parses as not recorded");
        Check(BuildViews(saveCase.Data).All(v => v.ObjectivesAchieved.Count == 0), "old save history synthesizes no objective achievement");
        int sharedLabelCount = saveCase.Data.workOrders.Count(o => o.harvestJobId == 5);
        YearHistoryView year5 = BuildViews(saveCase.Data).Single(v => v.Year == 5);
        Check(sharedLabelCount == 2, "old save reuses harvestJobId 5 on two orders: it is a year label, not a unique job ID",
            "orders with label 5=" + sharedLabelCount);
        Check(year5.HarvestSettlement != null && !year5.HarvestSettlement.GroupedOrderIds.Contains(3),
            "old save pending order 3 shares the year label but is not fabricated into the settled group");
        Console.WriteLine("OLD_SAVE_SOURCE=" + saveCase.Path);
    }

    private static void CurrentSaveProvenance(SaveCase saveCase)
    {
        Check(saveCase.RawHasObjectives, "current save carries its recorded objectiveAchievements field");
        Check(saveCase.Data.objectiveAchievements.Count == 5, "current save keeps all five objective achievements");
        Check(!saveCase.RawHasPrescriptions, "current v22 save has no prescription records in its source JSON");
        Check(saveCase.Data.prescriptions.Count == 0, "current save prescription state stays empty after migration");
        Check(saveCase.Data.nextPrescriptionId == 1, "current save next-prescription counter stays the explicit default");
        Check(!saveCase.RawHasEventPrescription && !saveCase.RawHasOrderPrescription, "current v22 save carries no prescription references");
        Check(saveCase.Data.managementEvents.All(e => e.prescriptionId == 0)
            && saveCase.Data.workOrders.All(o => o.prescriptionId == 0), "current save prescription references parse as not recorded");
        Console.WriteLine("CURRENT_SAVE_SOURCE=" + saveCase.Path);
    }

    // ---- pass-through and linkage ----

    private static void IdentityContract(SaveCase saveCase)
    {
        List<YearHistoryView> views = BuildViews(saveCase.Data);
        var eventById = saveCase.Data.managementEvents.ToDictionary(e => e.eventId);
        var orderById = saveCase.Data.workOrders.ToDictionary(o => o.workOrderId);

        foreach (YearHistoryView view in views)
        {
            Check(view.Events.All(e => saveCase.Data.managementEvents.Contains(e)), saveCase.Name + " year " + view.Year + " events are pass-through record references");
            Check(view.ResolvedOrders.All(o => saveCase.Data.workOrders.Contains(o)), saveCase.Name + " year " + view.Year + " resolved orders are pass-through references");
            Check(view.ResolvedWork.All(r => saveCase.Data.managementEvents.Contains(r.Event)
                && (r.Order == null || saveCase.Data.workOrders.Contains(r.Order))), saveCase.Name + " year " + view.Year + " resolved work references only input records");
            Check(view.Cancellations.All(c => saveCase.Data.managementEvents.Contains(c.Event)), saveCase.Name + " year " + view.Year + " cancellations reference only input events");
            Check(view.ObjectivesAchieved.All(a => saveCase.Data.objectiveAchievements.Contains(a)), saveCase.Name + " year " + view.Year + " achievements are pass-through references");
            Check(view.DeadwoodFallen.All(d => saveCase.Data.deadwoodRecords.Contains(d)), saveCase.Name + " year " + view.Year + " deadwood records are pass-through references");
            Check(view.Storms.All(s => saveCase.Data.stormEvents.Contains(s)), saveCase.Name + " year " + view.Year + " storm records are pass-through references");

            foreach (ResolvedWorkRecord record in view.ResolvedWork)
            {
                Check(record.Event.workOrderId > 0, saveCase.Name + " resolved event keeps a positive existing work-order ID",
                    "event=" + record.Event.eventId + " order=" + record.Event.workOrderId);
                if (orderById.TryGetValue(record.Event.workOrderId, out ScenarioOneWorkOrder expectedOrder))
                    Check(record.OrderStillExists && ReferenceEquals(record.Order, expectedOrder),
                        saveCase.Name + " resolved event links to the existing order by reference");
                else
                    Check(!record.OrderStillExists && record.Order == null,
                        saveCase.Name + " resolved event with no order record stays unlinked");
                if (record.Order != null)
                {
                    Check(record.OrderStillExists, saveCase.Name + " existing resolved order is marked present");
                    Check(record.Order.workOrderId == record.Event.workOrderId, saveCase.Name + " resolved event links to the existing order ID",
                        "event=" + record.Event.workOrderId + " order=" + record.Order.workOrderId);
                    if (record.Event.cellIndex >= 0)
                        Check(record.Event.cellIndex == record.Order.cellIndex, saveCase.Name + " resolved event cell is not re-keyed",
                            "event=" + record.Event.cellIndex + " order=" + record.Order.cellIndex);
                    if (!string.IsNullOrEmpty(record.Event.targetTreeId))
                        Check(record.Event.targetTreeId == record.Order.targetTreeId, saveCase.Name + " resolved event tree ID is not re-keyed",
                            "event=" + record.Event.targetTreeId + " order=" + record.Order.targetTreeId);
                }
                else
                {
                    Check(!record.OrderStillExists, saveCase.Name + " missing resolved order is marked absent");
                    Check(!orderById.ContainsKey(record.Event.workOrderId), saveCase.Name + " missing order is not fabricated under its event ID");
                }
            }

            foreach (HistoryCancellation cancellation in view.Cancellations)
            {
                Check(cancellation.WorkOrderId == cancellation.Event.workOrderId, saveCase.Name + " cancellation keeps the authoritative event work-order ID",
                    "event=" + cancellation.Event.workOrderId + " cancellation=" + cancellation.WorkOrderId);
                Check(cancellation.TargetTreeId == (cancellation.Event.targetTreeId ?? ""), saveCase.Name + " cancellation keeps the recorded tree ID");
                Check(cancellation.TaskType == cancellation.Event.taskType, saveCase.Name + " cancellation keeps the recorded task type");
                Check(cancellation.CellIndex == cancellation.Event.cellIndex, saveCase.Name + " cancellation keeps the recorded cell index");
                Check(cancellation.OrderStillExists == orderById.ContainsKey(cancellation.WorkOrderId), saveCase.Name + " cancellation existence matches the input records");
            }

            YearWorkDetail detail = ScenarioManagementHistory.DescribeWork(view);
            foreach (WorkItemDetail item in Items(detail))
            {
                Check(eventById.TryGetValue(item.EventId, out ScenarioManagementEvent source), saveCase.Name + " drill-down item points at an existing event ID",
                    "event=" + item.EventId);
                if (!eventById.TryGetValue(item.EventId, out source)) continue;
                Check(item.WorkOrderId == source.workOrderId, saveCase.Name + " drill-down item keeps the event work-order ID",
                    "event=" + item.EventId + " item=" + item.WorkOrderId + " source=" + source.workOrderId);
                Check(item.TaskType == source.taskType && item.Year == source.year, saveCase.Name + " drill-down item keeps task type and year");
                Check(item.TargetTreeId == (source.targetTreeId ?? ""), saveCase.Name + " drill-down item keeps the recorded tree ID");
                Check(item.SpeciesId == (source.speciesId ?? ""), saveCase.Name + " drill-down item keeps the recorded species ID");
                int expectedCell = source.cellIndex >= 0 ? source.cellIndex : -1;
                Check(item.CellIndex == expectedCell, saveCase.Name + " drill-down item keeps the recorded cell index (including the -1 sentinel)");
                Check(item.PositionX == source.worldPosition.x && item.PositionY == source.worldPosition.y && item.PositionZ == source.worldPosition.z,
                    saveCase.Name + " drill-down item keeps the recorded world position");
                bool expectedPosition = source.worldPosition.x != 0f || source.worldPosition.y != 0f || source.worldPosition.z != 0f;
                Check(item.PositionRecorded == expectedPosition, saveCase.Name + " drill-down position presence is honest");
                if (item.Status == WorkItemStatus.Cancelled)
                {
                    Check(!item.ExecutionMethodRecorded, saveCase.Name + " cancellation has no invented execution method");
                    Check(!item.OrderStillExists, saveCase.Name + " cancellation has no fabricated surviving order");
                    Check(!item.OwnMoneyRecorded, saveCase.Name + " cancellation invents no per-task money");
                }
                if (item.Status == WorkItemStatus.Failed)
                {
                    Check(item.ExecutionMethodRecorded, saveCase.Name + " failed resolution keeps its recorded execution method");
                    Check(!item.OwnMoneyRecorded, saveCase.Name + " failed resolution invents no per-task money");
                    Check(!item.IsGroupedHarvest, saveCase.Name + " failed harvest is not presented as a settled group");
                }
                if (item.Status == WorkItemStatus.Succeeded && item.IsGroupedHarvest)
                {
                    Check(!item.OwnMoneyRecorded, saveCase.Name + " grouped harvest invents no per-order money");
                    Check(item.ContractorCostCents == 0 && item.StockCostCents == 0 && item.OwnerMinutes == 0,
                        saveCase.Name + " grouped harvest item carries zero own money");
                }
                if (item.Status == WorkItemStatus.Succeeded && !item.IsGroupedHarvest)
                {
                    Check(item.OwnMoneyRecorded, saveCase.Name + " successful non-harvest keeps its own recorded money");
                    Check(item.ContractorCostCents == source.contractorCostCents
                        && item.StockCostCents == source.stockCostCents && item.OwnerMinutes == source.ownerMinutes,
                        saveCase.Name + " successful non-harvest money equals the recorded event");
                }
            }
        }

        foreach (ScenarioObjectiveAchievement achievement in saveCase.Data.objectiveAchievements)
        {
            Check(saveCase.Data.objectiveAchievements.Contains(achievement), saveCase.Name + " achievement record is authoritative input");
            Check(!string.IsNullOrEmpty(achievement.objectiveId), saveCase.Name + " achievement keeps its objective ID");
        }
    }

    // ---- grouped harvest ----

    private static void HarvestSemantics(SaveCase saveCase, (int Year, int SettlementEvent, int Label, int[] Orders)[] expected)
    {
        List<YearHistoryView> views = BuildViews(saveCase.Data);
        Check(views.Count == expected.Length, saveCase.Name + " harvest expectation covers every completed year");
        for (int i = 0; i < views.Count && i < expected.Length; i++)
        {
            YearHistoryView view = views[i];
            var want = expected[i];
            Check(view.Year == want.Year, saveCase.Name + " harvest expectation year");
            List<ResolvedWorkRecord> harvests = view.ResolvedWork
                .Where(r => r.Event.outcome == ScenarioManagementOutcome.Succeeded
                    && (r.Event.taskType == ScenarioWorkType.FellTree || r.Event.taskType == ScenarioWorkType.SalvageDeadwood))
                .OrderBy(r => r.Event.eventId).ToList();
            if (want.SettlementEvent < 0)
            {
                Check(view.HarvestSettlement == null && harvests.Count == 0, saveCase.Name + " year " + view.Year + " has no harvest settlement");
                continue;
            }
            Check(view.HarvestSettlement != null, saveCase.Name + " year " + view.Year + " has a grouped harvest settlement");
            if (view.HarvestSettlement == null) continue;
            GroupedHarvestSettlement settlement = view.HarvestSettlement;
            Check(settlement.ResolutionYearLabel == view.Year, saveCase.Name + " year " + view.Year + " harvest label is the resolution year",
                "label=" + settlement.ResolutionYearLabel);
            Check(settlement.SettlementEventId == want.SettlementEvent, saveCase.Name + " year " + view.Year + " settlement sits on the last resolved harvest event",
                "got " + settlement.SettlementEventId + " want " + want.SettlementEvent);
            Check(harvests.Count > 0 && harvests[harvests.Count - 1].Event.eventId == settlement.SettlementEventId,
                saveCase.Name + " year " + view.Year + " settlement event is the final harvest event");
            ScenarioManagementEvent settlementEvent = harvests.First(r => r.Event.eventId == settlement.SettlementEventId).Event;
            Check(settlement.SettlementRevenueCents == settlementEvent.timberRevenueCents
                && settlement.SettlementContractorCostCents == settlementEvent.contractorCostCents,
                saveCase.Name + " year " + view.Year + " settlement money is the recorded event money");
            Check(settlement.GroupedOrderIds.Distinct().Count() == settlement.GroupedOrderIds.Count,
                saveCase.Name + " year " + view.Year + " grouped order IDs are unique");
            int[] got = settlement.GroupedOrderIds.OrderBy(id => id).ToArray();
            int[] orderedWant = want.Orders.OrderBy(id => id).ToArray();
            Check(got.SequenceEqual(orderedWant), saveCase.Name + " year " + view.Year + " grouped order IDs use existing authoritative order IDs",
                "got " + string.Join(",", got) + " want " + string.Join(",", orderedWant));
            Check(settlement.GroupedOrderIds.All(id => harvests.Any(h => h.Event.workOrderId == id)),
                saveCase.Name + " year " + view.Year + " every grouped order has a resolved harvest event");
            foreach (ResolvedWorkRecord harvest in harvests)
            {
                Check(harvest.MoneyIsGroupedSettlement, saveCase.Name + " year " + view.Year + " harvest record is labelled grouped-settlement money");
                if (harvest.OrderStillExists)
                    Check(harvest.Order.harvestJobId == view.Year || harvest.Order.harvestJobId < 0,
                        saveCase.Name + " year " + view.Year + " resolved harvest order keeps a year label (or legacy -1), not a unique job ID",
                        "harvestJobId=" + harvest.Order.harvestJobId);
            }
            YearWorkDetail detail = ScenarioManagementHistory.DescribeWork(view);
            WorkGroupDetail done = detail.Done.Where(g => g.TaskType == ScenarioWorkType.FellTree || g.TaskType == ScenarioWorkType.SalvageDeadwood)
                .FirstOrDefault(g => g.Items.Count > 0);
            if (done != null)
                Check(done.Items.All(item => item.IsGroupedHarvest && !item.OwnMoneyRecorded),
                    saveCase.Name + " year " + view.Year + " drill-down harvest items carry no per-order money");
        }
    }

    // The genuine saves contain no cancellation, so this deliberately constructed case supplies the cancellation and
    // vanished-order evidence. It is semantic evidence only, never presented as an old save.
    private static void SyntheticCancellationCase()
    {
        var data = new ScenarioOneSaveData
        {
            initialized = true,
            scenarioId = "scenario-one",
            nextWorkOrderId = 904,
            nextManagementEventId = 7,
            nextDeadwoodId = 2,
            cashCents = 1000
        };
        data.managementEvents.AddRange(new[]
        {
            Event(1, 1, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, 900, ScenarioWorkType.FellTree, "P0900", 7),
            Event(2, 1, ScenarioManagementEventType.OrderCancelled, ScenarioManagementOutcome.Cancelled, 900, ScenarioWorkType.FellTree, "P0900", 7),
            Event(3, 1, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, 901, ScenarioWorkType.FellTree, "P0901", 8),
            Event(4, 1, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, 902, ScenarioWorkType.FellTree, "P0902", 9),
            Event(5, 1, ScenarioManagementEventType.WorkResolved, ScenarioManagementOutcome.Succeeded, 903, ScenarioWorkType.PlantJuvenile, "", 10),
            Event(6, 1, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.Succeeded, 0, ScenarioWorkType.FellTree, "", -1)
        });
        data.managementEvents[4].speciesId = "sessile-oak";
        data.managementEvents[4].stockCostCents = 250;
        data.managementEvents[3].timberRevenueCents = 500;
        data.managementEvents[3].contractorCostCents = 700;
        data.workOrders.AddRange(new[]
        {
            Order(902, ScenarioWorkType.FellTree, ScenarioWorkStatus.Completed, "P0902", 9, 1, 1),
            Order(903, ScenarioWorkType.PlantJuvenile, ScenarioWorkStatus.Completed, "", 10, 1, -1)
        });
        data.workOrders[1].speciesId = "sessile-oak";
        data.annualReports.Add(new ScenarioAnnualReport
        {
            year = 1, completedTasks = 3, contractorCostCents = 700, timberRevenueCents = 500, closingCashCents = 800
        });
        data.ecologicalSnapshots.Add(new ScenarioEcologicalSnapshot { year = 0, livingTrees = 10, meanCanopy = .5f, meanLight = .2f });
        data.ecologicalSnapshots.Add(new ScenarioEcologicalSnapshot { year = 1, livingTrees = 9, meanCanopy = .49f, meanLight = .21f });
        data.deadwoodRecords.Add(new ScenarioDeadwoodRecord
        {
            deadwoodId = "DW-900", treeId = "P0901", speciesId = "sitka-spruce", fallenYear = 1,
            originalVolumeM3 = 1f, remainingVolumeM3 = 1f, originalHeightMeters = 20f, originalDiameterCm = 30f
        });
        data.stormEvents.Add(new StormEventRecord { year = 1, severity = .05f, directionDegrees = 90f, cropTreesLost = 1 });
        data.objectiveAchievements.Add(new ScenarioObjectiveAchievement
        {
            objectiveId = "managed-opening", achievedYear = 1, targetValueAtAchievement = 1f
        });

        var saveCase = new SaveCase
        {
            Name = "constructed-cancellation", Path = "(constructed in check)", Version = 24, Year = 1, Data = data,
            ExpectedYears = new[] { 1 }, ExpectedEvents = 6, ExpectedOrders = 2, ExpectedNextEvent = 7,
            ExpectedNextOrder = 904, ExpectedAchievements = 1
        };
        ScenarioOneSaveMigration.NormalizeRackV2(data, saveCase.Version);
        ScenarioOneSaveMigration.NormalizeObjectiveHistory(data, saveCase.Version);
        ScenarioOneSaveMigration.NormalizePrescriptions(data, saveCase.Version);
        string before = Fingerprint(data);

        List<YearHistoryView> views = BuildViews(data);
        Check(views.Count == 1 && views[0].Year == 1, "constructed case has one completed year");
        YearHistoryView view = views[0];
        Check(view.Cancellations.Count == 1, "constructed case records exactly one cancellation");
        HistoryCancellation cancellation = view.Cancellations[0];
        Check(cancellation.WorkOrderId == 900, "cancelled order ID is the surviving event ID", "got " + cancellation.WorkOrderId);
        Check(cancellation.Event.eventId == 2, "cancellation event ID is unchanged", "got " + cancellation.Event.eventId);
        Check(!cancellation.OrderStillExists && data.workOrders.All(o => o.workOrderId != 900),
            "cancelled order has no fabricated replacement record");
        Check(cancellation.TargetTreeId == "P0900" && cancellation.TaskType == ScenarioWorkType.FellTree && cancellation.CellIndex == 7,
            "cancellation keeps tree, task and cell identities");

        YearWorkDetail detail = ScenarioManagementHistory.DescribeWork(view);
        WorkItemDetail cancelledItem = detail.Cancelled.Items.Single();
        Check(cancelledItem.EventId == 2 && cancelledItem.WorkOrderId == 900 && cancelledItem.Status == WorkItemStatus.Cancelled,
            "cancellation drill-down uses the surviving event and order ID");
        Check(cancelledItem.TargetTreeId == "P0900" && cancelledItem.CellIndex == 7 && !cancelledItem.ExecutionMethodRecorded,
            "cancellation drill-down keeps only recorded facts");
        Check(!cancelledItem.OrderStillExists && !cancelledItem.OwnMoneyRecorded, "cancellation drill-down invents no order or money");

        ResolvedWorkRecord vanished = view.ResolvedWork.Single(r => r.Event.eventId == 3);
        Check(!vanished.OrderStillExists && vanished.Order == null && vanished.Event.workOrderId == 901,
            "resolved event with a vanished order keeps its authoritative work-order ID");
        ResolvedWorkRecord existing = view.ResolvedWork.Single(r => r.Event.eventId == 4);
        Check(existing.OrderStillExists && ReferenceEquals(existing.Order, data.workOrders[0])
            && existing.Order.workOrderId == 902, "resolved event links to the existing order by reference");

        GroupedHarvestSettlement settlement = view.HarvestSettlement;
        Check(settlement != null && settlement.SettlementEventId == 4 && settlement.ResolutionYearLabel == 1,
            "constructed grouped settlement uses the last harvest event and the year label");
        Check(settlement != null && settlement.GroupedOrderIds.OrderBy(id => id).SequenceEqual(new[] { 901, 902 }),
            "constructed group includes the vanished and existing resolved harvest orders");
        Check(view.ResolvedWork.Where(r => r.Event.eventId == 3).All(r => r.Event.timberRevenueCents == 0),
            "earlier grouped harvest event keeps no invented per-order settlement");

        Check(view.ObjectivesAchieved.Count == 1 && ReferenceEquals(view.ObjectivesAchieved[0], data.objectiveAchievements[0]),
            "objective achievement passes through by reference");
        Check(view.DeadwoodFallen.Count == 1 && ReferenceEquals(view.DeadwoodFallen[0], data.deadwoodRecords[0]),
            "deadwood record passes through by reference");
        Check(view.Storms.Count == 1 && ReferenceEquals(view.Storms[0], data.stormEvents[0]),
            "storm record passes through by reference");
        Check(ReferenceEquals(view.Snapshot, data.ecologicalSnapshots[1]) && ReferenceEquals(view.PreviousSnapshot, data.ecologicalSnapshots[0]),
            "snapshot references pass through unchanged");

        List<YearHistoryView> again = BuildViews(data);
        ScenarioManagementHistory.DescribeWork(again[0]);
        string after = Fingerprint(data);
        Check(before == after, "constructed case query does not mutate any record or counter", ShortHash(before));
        Check(ViewsFingerprint(views) == ViewsFingerprint(again), "constructed case repeated query is deterministic");
    }

    private static ScenarioManagementEvent Event(int id, int year, ScenarioManagementEventType type,
        ScenarioManagementOutcome outcome, int orderId, ScenarioWorkType task, string tree, int cell)
        => new ScenarioManagementEvent
        {
            eventId = id, year = year, eventType = type, outcome = outcome, workOrderId = orderId,
            taskType = task, targetTreeId = tree, cellIndex = cell
        };

    private static ScenarioOneWorkOrder Order(int id, ScenarioWorkType type, ScenarioWorkStatus status,
        string tree, int cell, int resolvedYear, int harvestJob)
        => new ScenarioOneWorkOrder
        {
            workOrderId = id, type = type, status = status, targetTreeId = tree, cellIndex = cell,
            resolvedYear = resolvedYear, harvestJobId = harvestJob
        };

    // ---- reload and read purity ----

    private static void ReloadStability(SaveCase saveCase)
    {
        SaveCase reloaded = LoadCase(saveCase.Path, saveCase.Name + "-reload", saveCase.Version, saveCase.Year,
            saveCase.ExpectedEvents, saveCase.ExpectedOrders, saveCase.ExpectedNextEvent, saveCase.ExpectedNextOrder,
            saveCase.ExpectedYears, saveCase.ExpectedAchievements);
        if (reloaded == null) return;
        Check(!ReferenceEquals(saveCase.Data.managementEvents[0], reloaded.Data.managementEvents[0]),
            saveCase.Name + " reload is a genuine second parse, not the same object graph");
        Check(IdentityFingerprint(saveCase.Data) == IdentityFingerprint(reloaded.Data),
            saveCase.Name + " event, order and counter identities survive reload", ShortHash(IdentityFingerprint(reloaded.Data)));
        Check(ViewsFingerprint(BuildViews(saveCase.Data)) == ViewsFingerprint(BuildViews(reloaded.Data)),
            saveCase.Name + " per-year history output survives reload");
        Check(ResolvedOrderIds(saveCase.Data).SequenceEqual(ResolvedOrderIds(reloaded.Data)),
            saveCase.Name + " resolved work-order IDs are stable across query/reload");
    }

    private static void Purity(SaveCase saveCase)
    {
        // Measure purity on a freshly parsed copy: the earlier contract checks have already queried this case, so a
        // reader that mutates on every call would otherwise be invisible by the time the before-fingerprint is taken.
        SaveCase fresh = LoadCase(saveCase.Path, saveCase.Name + "-purity", saveCase.Version, saveCase.Year,
            saveCase.ExpectedEvents, saveCase.ExpectedOrders, saveCase.ExpectedNextEvent, saveCase.ExpectedNextOrder,
            saveCase.ExpectedYears, saveCase.ExpectedAchievements);
        if (fresh == null) return;
        string before = Fingerprint(fresh.Data);
        int eventCount = fresh.Data.managementEvents.Count, orderCount = fresh.Data.workOrders.Count;
        int nextEvent = fresh.Data.nextManagementEventId, nextOrder = fresh.Data.nextWorkOrderId;
        List<YearHistoryView> first = BuildViews(fresh.Data);
        foreach (YearHistoryView view in first) ScenarioManagementHistory.DescribeWork(view);
        List<YearHistoryView> second = BuildViews(fresh.Data);
        foreach (YearHistoryView view in second) ScenarioManagementHistory.DescribeWork(view);
        string after = Fingerprint(fresh.Data);
        Check(before == after, saveCase.Name + " repeated history queries mutate no record, collection or counter", ShortHash(before));
        Check(fresh.Data.managementEvents.Count == eventCount && fresh.Data.workOrders.Count == orderCount,
            saveCase.Name + " query preserves collection sizes");
        Check(fresh.Data.nextManagementEventId == nextEvent && fresh.Data.nextWorkOrderId == nextOrder,
            saveCase.Name + " query preserves next-ID counters");
        Check(!ReferenceEquals(first, second) && !ReferenceEquals(first[0], second[0]) && !ReferenceEquals(first[0].Events, second[0].Events),
            saveCase.Name + " each query builds a fresh view and event list");
        Check(first[0].Events.SequenceEqual(second[0].Events), saveCase.Name + " fresh queries still return the same pass-through event references");
        Check(ViewsFingerprint(first) == ViewsFingerprint(second), saveCase.Name + " repeated query output is identical");
    }

    // ---- no second identity authority ----

    private static void ReaderHasNoSecondIdentityStore()
    {
        CheckNoStaticFields(typeof(ScenarioManagementHistory), "ScenarioManagementHistory");
        CheckNoStaticFields(typeof(ScenarioInterventionHistory), "ScenarioInterventionHistory");

        string historyPath = Path.Combine(root, "Assets/ForestPrototype/ScenarioOne/ScenarioManagementHistory.cs");
        string interventionPath = Path.Combine(root, "Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs");
        string validationPath = Path.Combine(root, "Assets/ForestPrototype/ForestSaveValidation.cs");
        Check(File.Exists(historyPath) && File.Exists(interventionPath) && File.Exists(validationPath), "identity-authority source files exist");
        if (!File.Exists(historyPath) || !File.Exists(interventionPath) || !File.Exists(validationPath)) return;

        string history = File.ReadAllText(historyPath);
        string intervention = File.ReadAllText(interventionPath);
        string validation = File.ReadAllText(validationPath);
        foreach (var pair in new[] { ("ScenarioManagementHistory", history), ("ScenarioInterventionHistory", intervention) })
        {
            string label = pair.Item1, source = pair.Item2;
            Check(!source.Contains("nextManagementEventId") && !source.Contains("nextWorkOrderId"),
                label + " does not read or advance ID counters");
            Check(!source.Contains("Guid.NewGuid") && !source.Contains("System.Guid")
                && !source.Contains("new Random") && !source.Contains("UnityEngine.Random"),
                label + " contains no second ID generator");
            Check(!source.Contains("appears twice") && !source.Contains("eventId <= 0"),
                label + " contains no duplicate event-ID validator");
        }
        Check(validation.Contains("ValidateManagementEventIds")
            && validation.Contains("management event ID")
            && validation.Contains("eventId <= 0")
            && validation.Contains("firstRecordForId"),
            "ForestSaveValidation remains the management-event ID integrity authority");
    }

    private static void CheckNoStaticFields(Type type, string label)
    {
        FieldInfo[] fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        FieldInfo[] mutable = fields.Where(f => !f.IsLiteral).ToArray();
        Check(mutable.Length == 0, label + " owns no static field, cache, store or generator",
            mutable.Length == 0 ? "" : string.Join(",", mutable.Select(f => f.Name)));
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        Check(properties.Length == 0, label + " owns no static property cache or store",
            properties.Length == 0 ? "" : string.Join(",", properties.Select(p => p.Name)));
    }

    // ---- duplicate-ID negative control through the production validator ----

    private static void DuplicateEventIdControl()
    {
        Check(ForestSaveValidation.Validate(ValidSave(Ids(1, 2, 3)), 1, 64) == null, "production validator accepts distinct positive event IDs");
        string duplicate = ForestSaveValidation.Validate(ValidSave(Ids(1, 2, 2)), 1, 64);
        Check(duplicate == "management event ID 2 appears twice (records 1 and 2)",
            "production validator rejects a duplicate management-event ID", duplicate ?? "null");
        string nonPositive = ForestSaveValidation.Validate(ValidSave(Ids(1, 0)), 1, 64);
        Check(nonPositive != null && nonPositive.Contains("non-positive ID"),
            "production validator rejects a non-positive management-event ID", nonPositive ?? "null");

        ForestSaveData data = ValidSave(Ids(1, 2, 2));
        string before = JsonSerializer.Serialize(data, JsonOptions);
        ForestSaveValidation.Validate(data, 1, 64);
        Check(JsonSerializer.Serialize(data, JsonOptions) == before,
            "production validator never renumbers, repairs or reorders a rejected event list");
    }

    private static List<ScenarioManagementEvent> Ids(params int[] ids)
        => ids.Select(id => Event(id, 1, ScenarioManagementEventType.YearAdvanced, ScenarioManagementOutcome.Succeeded, 0, ScenarioWorkType.FellTree, "", -1)).ToList();

    private static ForestSaveData ValidSave(List<ScenarioManagementEvent> events)
        => new ForestSaveData
        {
            version = ForestSaveData.CurrentVersion,
            ecologicalYear = 1,
            growthModel = 1,
            regenerationModel = 1,
            stormModel = 0,
            standGeometryModel = 0,
            trees = new List<TreeSaveData> { new TreeSaveData { treeId = "P1", stage = 0 } },
            scenarioOne = new ScenarioOneSaveData
            {
                initialized = true,
                managementEvents = events,
                nextManagementEventId = (events == null ? 0 : events.Where(e => e != null).Select(e => e.eventId).DefaultIfEmpty(0).Max()) + 1
            }
        };

    // ---- helpers ----

    private static List<YearHistoryView> BuildViews(ScenarioOneSaveData data)
        => ScenarioManagementHistory.BuildYearViews(data.managementEvents, data.annualReports, data.ecologicalSnapshots,
            data.deadwoodRecords, data.workOrders, data.stormEvents, data.objectiveAchievements);

    private static IEnumerable<WorkItemDetail> Items(YearWorkDetail detail)
        => detail.Done.SelectMany(g => g.Items).Concat(detail.Failed.Items).Concat(detail.Cancelled.Items);

    private static string Fingerprint(ScenarioOneSaveData data)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data, JsonOptions)))).ToLowerInvariant();

    private static string ShortHash(string value) => value.Length <= 16 ? value : value.Substring(0, 16);

    private static string IdentityFingerprint(ScenarioOneSaveData data)
    {
        var text = new StringBuilder();
        foreach (ScenarioManagementEvent entry in data.managementEvents)
            text.Append("E").Append(entry.eventId).Append(':').Append(entry.year).Append(':').Append(entry.workOrderId).Append(';');
        foreach (ScenarioOneWorkOrder order in data.workOrders)
            text.Append("O").Append(order.workOrderId).Append(':').Append(order.resolvedYear).Append(':').Append(order.harvestJobId).Append(';');
        text.Append("C").Append(data.nextManagementEventId).Append(':').Append(data.nextWorkOrderId).Append(':').Append(data.nextDeadwoodId);
        return text.ToString();
    }

    private static int[] ResolvedOrderIds(ScenarioOneSaveData data)
        => BuildViews(data).SelectMany(v => v.ResolvedOrders).Select(o => o.workOrderId).ToArray();

    private static string ViewsFingerprint(List<YearHistoryView> views)
    {
        var text = new StringBuilder();
        foreach (YearHistoryView view in views)
        {
            text.Append("Y").Append(view.Year).Append('|');
            text.Append("E").Append(string.Join(",", view.Events.Select(e => e.eventId))).Append('|');
            text.Append("R").Append(string.Join(",", view.ResolvedWork.Select(r => r.Event.eventId + ":" + r.Event.workOrderId + ":" + r.OrderStillExists))).Append('|');
            text.Append("O").Append(string.Join(",", view.ResolvedOrders.Select(o => o.workOrderId))).Append('|');
            text.Append("C").Append(string.Join(",", view.Cancellations.Select(c => c.Event.eventId + ":" + c.WorkOrderId + ":" + c.OrderStillExists))).Append('|');
            text.Append("A").Append(string.Join(",", view.ObjectivesAchieved.Select(a => a.objectiveId + "@" + a.achievedYear))).Append('|');
            text.Append("S").Append(view.HarvestSettlement == null ? "none"
                : view.HarvestSettlement.ResolutionYearLabel + ":" + view.HarvestSettlement.SettlementEventId + ":" + string.Join(",", view.HarvestSettlement.GroupedOrderIds)).Append('|');
            YearWorkDetail detail = ScenarioManagementHistory.DescribeWork(view);
            text.Append("D").Append(string.Join(",", Items(detail).Select(i => i.EventId + ":" + i.WorkOrderId + ":" + i.Status + ":" + i.CellIndex + ":" + i.TargetTreeId))).AppendLine();
        }
        return text.ToString();
    }
}
