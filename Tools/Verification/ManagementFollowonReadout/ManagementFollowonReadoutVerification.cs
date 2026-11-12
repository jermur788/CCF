using System;
using System.Collections.Generic;
using System.Linq;

// Standalone focused source-verification fixture. Compile in a C# lane with the actual Scenario One types;
// no game scenes, work controller, saved worlds, fixture changes or Unity Editor operations are requested.
// SOURCE WRITTEN ONLY by browser worker: these assertions are NOT a recorded execution result.
public static class ManagementFollowonReadoutVerification
{
    private static int checks;
    private static void Check(bool condition, string label)
    {
        checks++;
        if (!condition) throw new InvalidOperationException("FAILED " + label);
    }

    private static YearHistoryView Year(int year, ScenarioEcologicalSnapshot snapshot = null)
        => new YearHistoryView { Year = year, YearAdvancedRecorded = true, Snapshot = snapshot };
    private static ScenarioEcologicalSnapshot Snapshot(int year, int count = 100)
        => new ScenarioEcologicalSnapshot { year = year, livingTrees = count, basalAreaM2PerHa = 12f,
            meanDbhCm = 18f, meanCanopy = 0.6f, meanLight = 0.4f, occupiedRegenerationCells = 5 };
    private static ScenarioSpeciesOutcome Species(string id, int natural, float density, int plantedCells, int planted)
        => new ScenarioSpeciesOutcome { speciesId = id, livingTrees = 50, basalAreaM2PerHa = 10f,
            regenerationCells = natural, regenerationDensity = density,
            plantedRegenerationCells = plantedCells, plantedJuveniles = planted };
    private static ScenarioOneWorkOrder Order(int id, int year, ScenarioWorkType type = ScenarioWorkType.FellTree,
        ScenarioWorkStatus status = ScenarioWorkStatus.Completed, int prescription = 0, string unit = "", int rack = 0)
        => new ScenarioOneWorkOrder { workOrderId = id, resolvedYear = year, type = type, status = status,
            prescriptionId = prescription, prescriptionCoverageUnitId = unit, rackId = rack,
            targetTreeId = "T" + id, speciesId = "sitka-spruce" };
    private static ScenarioManagementEvent Event(int id, int orderId, int year, ScenarioWorkType type = ScenarioWorkType.FellTree,
        ScenarioManagementOutcome outcome = ScenarioManagementOutcome.Succeeded, int prescription = 0)
        => new ScenarioManagementEvent { eventId = id, workOrderId = orderId, year = year,
            eventType = ScenarioManagementEventType.WorkResolved, taskType = type, outcome = outcome,
            prescriptionId = prescription, targetTreeId = "T" + orderId, speciesId = "sitka-spruce" };
    private static void Add(YearHistoryView year, ScenarioManagementEvent e, ScenarioOneWorkOrder o)
        {
        year.Events.Add(e);
        year.ResolvedWork.Add(new ResolvedWorkRecord { Event = e, Order = o, OrderStillExists = o != null });
    }
    private static ScenarioOneManagementFollowonReadout.Result Read(int selected, params YearHistoryView[] years)
        => ScenarioOneManagementFollowonReadout.Build(years, selected);

    private static void Adversarial(string selection)
    {
        if (selection == "all" || selection == "global")
        foreach (var kind in new[] { ScenarioManagementEventType.OrderCreated, ScenarioManagementEventType.YearAdvanced })
        {
            var year = Year(20);
            Add(year, Event(901, 91, 20, prescription: 8), Order(91, 20, prescription: 8, unit: "U1"));
            year.Events.Add(new ScenarioManagementEvent { eventId = 901, year = 20, eventType = kind });
            var result = Read(20, year);
            Check(result.RecordedWork[0].Link == ScenarioOneManagementFollowonReadout.OrderLink.AmbiguousIdentity,
                "global ID collision with " + kind);
            Check(result.ExecutedCoverage.Count == 0, "globally ambiguous work has no executed coverage");
        }
        if (selection == "all" || selection == "target")
        {
            var year = Year(21);
            var e = Event(902, 92, 21, prescription: 8);
            var o = Order(92, 21, prescription: 8, unit: "U1");
            o.targetTreeId = "OTHER";
            Add(year, e, o);
            Check(Read(21, year).RecordedWork[0].Link == ScenarioOneManagementFollowonReadout.OrderLink.InconsistentOrder,
                "retained order target mismatch");
            Check(Read(21, year).ExecutedCoverage.Count == 0, "target mismatch has no coverage");
            o.targetTreeId = e.targetTreeId; o.speciesId = "oak";
            Check(Read(21, year).ExecutedCoverage.Count == 0, "available species mismatch has no coverage");
            o.speciesId = ""; e.speciesId = ""; o.targetTreeId = ""; e.targetTreeId = "";
            var old = Read(21, year);
            Check(old.RecordedWork[0].Link != ScenarioOneManagementFollowonReadout.OrderLink.Verified
                && old.ExecutedCoverage.Count == 0 && old.RecordedWork[0].TargetTreeId == "",
                "absent historical tree identities stay unknown");
        }
        if (selection == "all" || selection == "harvest")
        foreach (var type in new[] { ScenarioWorkType.FellTree, ScenarioWorkType.SalvageDeadwood })
        {
            var year = Year(22);
            Add(year, Event(903, 93, 22, type), Order(93, 22, type));
            Check(!Read(22, year).RecordedWork[0].GroupedHarvestMoney, "no report or settlement " + type);
            year.Report = new ScenarioAnnualReport();
            Check(!Read(22, year).RecordedWork[0].GroupedHarvestMoney, "report without settlement " + type);
            year.Report = null; year.HarvestSettlement = new GroupedHarvestSettlement();
            Check(!Read(22, year).RecordedWork[0].GroupedHarvestMoney, "settlement without report " + type);
            year.Report = new ScenarioAnnualReport();
            Check(Read(22, year).RecordedWork[0].GroupedHarvestMoney, "recorded grouped annual settlement " + type);
        }
    }

    public static int Main(string[] args)
    {
        if (args.Length > 0)
        {
            Adversarial(args[0]);
            Console.WriteLine("Adversarial PASS " + checks);
            return 0;
        }
        // Exact-year latest: never backfill a missing selected or newest year from neighbouring snapshots.
        var a = Year(5, Snapshot(5)); var b = Year(9, Snapshot(9, 90));
        var found = Read(5, b, a);
        Check(found.Selected.Year == 5 && found.Latest.Year == 9, "exact records, ordered");
        Check(found.ComparisonState == ScenarioOneManagementFollowonReadout.ObservationState.Recorded, "later compared");
        Check(found.Comparison.YearsBetween == 4 && found.Comparison.LivingTreesChange == -10, "same units years");
        Check(Read(5, Year(5), b).ComparisonState ==
            ScenarioOneManagementFollowonReadout.ObservationState.NotRecorded, "missing selected snapshot");
        Check(Read(5, a, Year(9)).ComparisonState ==
            ScenarioOneManagementFollowonReadout.ObservationState.NotRecorded, "no latest fallback");
        Check(Read(5, Year(5, Snapshot(6)), b).SelectedState ==
            ScenarioOneManagementFollowonReadout.ObservationState.NotRecorded, "mismatched snapshot year");
        Check(Read(9, a, b).ComparisonState ==
            ScenarioOneManagementFollowonReadout.ObservationState.LatestNotLater, "no future observation");
        Check(Read(20, a, b).SelectedState ==
            ScenarioOneManagementFollowonReadout.ObservationState.NotRecorded, "uncompleted selected year");
        Check(Read(5).LatestCompletedYear == -1, "empty completed years");
        Check(Read(5, Year(5, Snapshot(5)), Year(5, Snapshot(5)), b).ComparisonState ==
            ScenarioOneManagementFollowonReadout.ObservationState.CannotCompare, "ambiguous duplicate year");

        var duplicateA = Year(6, Snapshot(6)); var duplicateB = Year(6, Snapshot(6));
        Add(duplicateA, Event(81, 81, 6, prescription: 3), Order(81, 6, prescription: 3, unit: "R1L"));
        Add(duplicateB, Event(82, 82, 6, prescription: 3), Order(82, 6, prescription: 3, unit: "R1H"));
        Check(Read(6, duplicateA, duplicateB).ExecutedCoverage.Count == 0,
            "duplicate completed-year work linkage fails closed");

        // Origin/species measures retain their separate meanings; absent species is missing, not zero.
        a.Snapshot.species.Add(Species("sitka-spruce", 2, 3f, 1, 4));
        a.Snapshot.species.Add(Species("oak", 2, 2f, 0, 1));
        b.Snapshot.species.Add(Species("sitka-spruce", 5, 6f, 3, 7));
        b.Snapshot.species.Add(Species("beech", 1, 1f, 0, 0));
        var change = Read(5, a, b).Comparison;
        var sitka = change.Species.Single(s => s.SpeciesId == "sitka-spruce");
        Check(sitka.NaturalRegenerationCellsChange == 3 && sitka.NaturalRegenerationDensityChange == 3f,
            "natural represented cells/density");
        Check(sitka.PlantedRegenerationCellsChange == 2 && sitka.ExactPlantedJuvenilesChange == 3,
            "distinct planted measures");
        Check(change.Species.Single(s => s.SpeciesId == "oak").State ==
            ScenarioOneManagementFollowonReadout.ObservationState.NotRecorded, "missing later oak not zero");
        Check(change.Species.Single(s => s.SpeciesId == "beech").State ==
            ScenarioOneManagementFollowonReadout.ObservationState.NotRecorded, "missing earlier beech not zero");
        b.Snapshot.species.Add(Species("beech", 2, 1f, 0, 0));
        Check(Read(5, a, b).ComparisonState ==
            ScenarioOneManagementFollowonReadout.ObservationState.CannotCompare, "duplicate species not silently averaged");
        b.Snapshot.species.RemoveAt(b.Snapshot.species.Count - 1);

        // Genuine successes, failed and pending orders, two units and different years.
        var first = Year(10, Snapshot(10)); var next = Year(11, Snapshot(11));
        first.Report = new ScenarioAnnualReport(); first.HarvestSettlement = new GroupedHarvestSettlement();
        Add(first, Event(101, 1, 10, prescription: 7), Order(1, 10, prescription: 7, unit: "R2L"));
        Add(first, Event(102, 2, 10, prescription: 7), Order(2, 10, prescription: 7, unit: "R2L"));
        Add(first, Event(103, 3, 10, prescription: 7), Order(3, 10, prescription: 7, unit: "R4H"));
        Add(next, Event(104, 4, 11, prescription: 7), Order(4, 11, prescription: 7, unit: "R2L"));
        Add(first, Event(105, 5, 10, outcome: ScenarioManagementOutcome.Failed, prescription: 7),
            Order(5, 10, status: ScenarioWorkStatus.Failed, prescription: 7, unit: "R4H"));
        Add(first, Event(106, 6, 10, prescription: 7),
            Order(6, 10, status: ScenarioWorkStatus.Pending, prescription: 7, unit: "R4H"));
        Add(first, Event(107, 7, 10, prescription: 7), null);
        var report = Read(10, next, first);
        Check(report.ExecutedCoverage.Count == 3, "separate year + prescription + unit");
        Check(report.ExecutedCoverage[0].WorkOrderIds.SequenceEqual(new[] { 1, 2 }), "stable ordered grouping");
        Check(report.ExecutedCoverage.All(g => !g.WorkOrderIds.Contains(5) &&
            !g.WorkOrderIds.Contains(6) && !g.WorkOrderIds.Contains(7)), "no failed pending orphan group");
        Check(report.RecordedWork.Single(w => w.WorkOrderId == 6).Link ==
            ScenarioOneManagementFollowonReadout.OrderLink.InconsistentOrder, "pending not executed");
        Check(report.RecordedWork.Single(w => w.WorkOrderId == 7).Link ==
            ScenarioOneManagementFollowonReadout.OrderLink.MissingOrder, "orphan unknown provenance");
        Check(report.PossibleTreatmentYears.Single(y => y.Year == 10).CandidateFellEvents == 3,
            "only linked non-rack successes candidate");
        Check(report.ExecutedCoverage[0].CoverageUnitId == "R2L", "unit string passed through, not parsed");
        Check(report.RecordedWork.Single(w => w.EventId == 101).GroupedHarvestMoney, "harvest money remains grouped");
        Check(report.RecordedWork.All(w => w.RackId == null || w.RackId.Value == 0),
            "no invented source rack");

        var access = Year(12, Snapshot(12));
        Add(access, Event(200, 20, 12), Order(20, 12, rack: 3));
        Check(Read(12, access).RecordedWork[0].Felling ==
            ScenarioOneManagementFollowonReadout.FellEvidence.RackOwnedAccess, "rack owned not stand treatment");
        var maintenance = Year(13, Snapshot(13));
        Add(maintenance, Event(201, 21, 13, ScenarioWorkType.PlantJuvenile),
            Order(21, 13, ScenarioWorkType.PlantJuvenile));
        Check(Read(13, maintenance).PossibleTreatmentYears.Count == 0, "planting not major treatment");
        var cancelled = Event(202, 22, 13);
        cancelled.eventType = ScenarioManagementEventType.OrderCancelled;
        cancelled.outcome = ScenarioManagementOutcome.Cancelled;
        maintenance.Cancellations.Add(new HistoryCancellation { Event = cancelled, WorkOrderId = 22, Year = 13 });
        Check(Read(13, maintenance).RecordedWork.Single(w => w.EventId == 202).Result ==
            ScenarioOneManagementFollowonReadout.Resolution.Cancelled, "cancel survives absent order");

        var old = Year(14, Snapshot(14));
        Add(old, Event(301, 31, 14), Order(31, 14));
        var prior = Read(14, old);
        Check(prior.RecordedWork.Count == 1 && prior.ExecutedCoverage.Count == 0,
            "old save without prescription still has work");
        var duplicated = Year(15, Snapshot(15));
        Add(duplicated, Event(401, 40, 15, prescription: 9), Order(40, 15, prescription: 9, unit: "R1L"));
        Add(duplicated, Event(401, 41, 15, prescription: 9), Order(41, 15, prescription: 9, unit: "R1H"));
        Check(Read(15, duplicated).ExecutedCoverage.Count == 0, "duplicate event id fail closed");
        Check(Read(15, duplicated).RecordedWork.All(w => w.Link ==
            ScenarioOneManagementFollowonReadout.OrderLink.AmbiguousIdentity), "duplicates retained and labelled");

        var duplicateOrders = Year(16, Snapshot(16));
        Add(duplicateOrders, Event(501, 50, 16, prescription: 8), Order(50, 16, prescription: 8, unit: "R1L"));
        Add(duplicateOrders, Event(502, 50, 16, prescription: 8), Order(50, 16, prescription: 8, unit: "R1L"));
        Check(Read(16, duplicateOrders).ExecutedCoverage.Count == 0, "duplicate resolved order fail closed");
        var mismatch = Year(17, Snapshot(17));
        Add(mismatch, Event(601, 60, 17, prescription: 8), Order(60, 16, prescription: 8, unit: "R3H"));
        Check(Read(17, mismatch).ExecutedCoverage.Count == 0, "mismatched resolved year fails");
        var empty = Year(18, Snapshot(18));
        Check(Read(18, empty).RecordedWork.Count == 0 && Read(18, empty).ExecutedCoverage.Count == 0, "no-work year");

        // Source data remains untouched; result contains numeric copies rather than mutable original snapshot references.
        int before = a.Snapshot.livingTrees;
        var repeated = Read(5, a, b);
        Check(Read(5, a, b).Comparison.LivingTreesChange == repeated.Comparison.LivingTreesChange,
            "deterministic repeated read");
        repeated.Selected.LivingTrees = -99;
        Check(a.Snapshot.livingTrees == before && a.Snapshot.species[0].regenerationCells == 2,
            "result modification cannot change saved observation");
        Check(first.ResolvedWork.Count == 6 && first.ResolvedWork[0].Order.prescriptionId == 7,
            "history inputs not changed");

        Adversarial("all");
        Console.WriteLine("ManagementFollowonReadout: PASS " + checks + " focused assertions (when actually run).");
        return 0;
    }
}
