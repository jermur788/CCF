using System;
using System.Collections.Generic;
using System.Linq;

// D-073/D-074/D-076: browser-safe, SOURCE-ONLY readout of already recorded S1-C years.
// NO save, event, goal, MFM or scene writes. This is not an objective grader or an ecological attribution model.
// Read from ScenarioOneManager.HistoryYears (already-built completed year views). Never rebuild from current trees.
public static class ScenarioOneManagementFollowonReadout
{
    public enum ObservationState { Recorded, NotRecorded, CannotCompare, LatestNotLater }
    public enum OrderLink { Verified, MissingOrder, InconsistentOrder, AmbiguousIdentity }
    public enum Resolution { Succeeded, Failed, Cancelled, Unresolved }
    public enum FellEvidence { NotFelling, RackOwnedAccess, PossibleStandTreatment, NeedsClassification }

    // A copy of numeric values, never a mutable reference to the supplied persisted snapshot.
    // Cell counts are MODELLED occupied cells, not individuals, hectares, surviving seedlings or established trees.
    public sealed class SpeciesObservation
    {
        public string SpeciesId = "";
        public int LivingTrees;
        public float BasalAreaM2PerHa;
        public int NaturalRegenerationCells;
        public float NaturalRegenerationDensity;
        public int PlantedRegenerationCells;
        public int ExactPlantedJuveniles;
    }

    public sealed class StandObservation
    {
        public int Year;
        public int LivingTrees;
        public float BasalAreaM2PerHa, MeanDbhCm, MeanCanopy, MeanLight;
        public int OccupiedRegenerationCells;
        public int DeadwoodCount;
        public float DeadwoodVolumeM3;
        public readonly List<SpeciesObservation> Species = new List<SpeciesObservation>();
        public bool SpeciesIdsUnique = true;
        public const string Provenance = "Recorded annual stand/species model snapshot; not historical per-rack ecology.";
    }

    // "Change" is an arithmetic difference of two snapshots, NOT evidence that work caused it.
    public sealed class SpeciesComparison
    {
        public string SpeciesId = "";
        public ObservationState State;
        public int NaturalRegenerationCellsChange, PlantedRegenerationCellsChange, ExactPlantedJuvenilesChange;
        public float NaturalRegenerationDensityChange;
        public int LivingTreesChange;
        public float BasalAreaM2PerHaChange;
    }

    public sealed class StandComparison
    {
        public int FromYear, ToYear, YearsBetween;
        public int LivingTreesChange, OccupiedRegenerationCellsChange, DeadwoodCountChange;
        public float BasalAreaM2PerHaChange, MeanDbhCmChange, MeanCanopyChange, MeanLightChange, DeadwoodVolumeM3Change;
        public readonly List<SpeciesComparison> Species = new List<SpeciesComparison>();
        public const string Provenance = "Derived same-unit aggregate difference; timing alone proves no management causality.";
    }

    // One factual WorkResolved / OrderCancelled event. A missing or contradictory retained order is explicit.
    // No money is distributed to harvest trees: the existing annual report remains financial authority.
    public sealed class RecordedWork
    {
        public int Year, EventId, WorkOrderId;
        public ScenarioWorkType TaskType;
        public Resolution Result;
        public string TargetTreeId = "", SpeciesId = "", CoverageUnitId = "";
        public int PrescriptionId;
        public int? RackId; // null means order linkage cannot support a rack claim; 0 is explicitly 'not rack-owned'.
        public OrderLink Link;
        public FellEvidence Felling;
        public bool GroupedHarvestMoney; // no per-order allocation exposed
        public string FailureReason = "";
    }

    // A grouping of EXECUTED success only: exact resolution year + prescription + stored coverage unit.
    // It is NOT a historic per-rack canopy/young-growth observation; no source rack is inferred from unit-id text.
    public sealed class ExecutedCoverageGroup
    {
        public int ResolutionYear, PrescriptionId;
        public string CoverageUnitId = "";
        public readonly List<int> EventIds = new List<int>();
        public readonly List<int> WorkOrderIds = new List<int>();
    }

    // Advisory count of possibly meaningful stand felling. Even one or many candidate trees do NOT satisfy a new
    // major-intervention, interval, teaching, quality or completion rule.
    public sealed class PossibleTreatmentYear
    {
        public int Year;
        public int CandidateFellEvents, RackOwnedFellEvents, UnclassifiedFellEvents;
        public const string Status = "CANDIDATE ONLY — major stand treatment is NOT determined.";
    }

    public sealed class Result
    {
        public int SelectedYear, LatestCompletedYear = -1;
        public ObservationState SelectedState = ObservationState.NotRecorded;
        public ObservationState LatestState = ObservationState.NotRecorded;
        public ObservationState ComparisonState = ObservationState.CannotCompare;
        public StandObservation Selected, Latest;
        public StandComparison Comparison;
        public readonly List<RecordedWork> RecordedWork = new List<RecordedWork>();
        public readonly List<ExecutedCoverageGroup> ExecutedCoverage = new List<ExecutedCoverageGroup>();
        public readonly List<PossibleTreatmentYear> PossibleTreatmentYears = new List<PossibleTreatmentYear>();
        public const string Scope = "Read-only recorded history. Proposals, intentions, current rack metrics and per-rack ecological history are not inferred.";
    }

    // The latest year means the exact latest COMPLETED year. Do not silently use an earlier snapshot if it is
    // absent; ScenarioInterventionHistory.After's next-year fallback is deliberately NOT used here.
    public static Result Build(IReadOnlyList<YearHistoryView> years, int selectedYear)
    {
        var answer = new Result { SelectedYear = selectedYear };
        List<YearHistoryView> completed = (years ?? Array.Empty<YearHistoryView>())
            .Where(y => y != null && y.HasCompletedRecord).OrderBy(y => y.Year).ToList();

        if (completed.Count > 0)
        {
            answer.LatestCompletedYear = completed[completed.Count - 1].Year;
            List<YearHistoryView> latest = completed.Where(y => y.Year == answer.LatestCompletedYear).ToList();
            if (latest.Count == 1 && ExactSnapshot(latest[0]))
            {
                answer.Latest = CopyObservation(latest[0].Snapshot);
                answer.LatestState = ObservationState.Recorded;
            }
            List<YearHistoryView> selected = completed.Where(y => y.Year == selectedYear).ToList();
            if (selected.Count == 1 && ExactSnapshot(selected[0]))
            {
                answer.Selected = CopyObservation(selected[0].Snapshot);
                answer.SelectedState = ObservationState.Recorded;
            }
            if (selected.Count > 1 || latest.Count > 1)
                answer.ComparisonState = ObservationState.CannotCompare;
            else if (answer.LatestCompletedYear <= selectedYear)
                answer.ComparisonState = ObservationState.LatestNotLater;
            else if (answer.Selected == null || answer.Latest == null)
                answer.ComparisonState = ObservationState.NotRecorded;
            else if (!answer.Selected.SpeciesIdsUnique || !answer.Latest.SpeciesIdsUnique)
                answer.ComparisonState = ObservationState.CannotCompare;
            else
            {
                answer.Comparison = Compare(answer.Selected, answer.Latest);
                answer.ComparisonState = ObservationState.Recorded;
            }
        }

        // Event identity is global, order resolution identity is global. Never quietly de-duplicate a suspect
        // event/order into apparently authoritative executed prescription coverage.
        var resolved = completed.SelectMany(y => y.ResolvedWork.Where(w => w != null && w.Event != null)
            .Select(w => new WorkRow(y.Year, w.Event, w.Order, false))).ToList();
        var cancelled = completed.SelectMany(y => y.Cancellations.Where(c => c != null && c.Event != null)
            .Select(c => new WorkRow(y.Year, c.Event, null, true))).ToList();
        var all = resolved.Concat(cancelled).OrderBy(w => w.Year).ThenBy(w => w.Event.eventId)
            .ThenBy(w => w.Event.workOrderId).ThenBy(w => (int)w.Event.taskType)
            .ThenBy(w => w.Cancelled).ToList();
        var duplicateEvents = new HashSet<int>(all.GroupBy(w => w.Event.eventId)
            .Where(g => g.Key <= 0 || g.Count() > 1).Select(g => g.Key));
        var duplicateOrders = new HashSet<int>(resolved.GroupBy(w => w.Event.workOrderId)
            .Where(g => g.Key <= 0 || g.Count() > 1).Select(g => g.Key));

        foreach (WorkRow row in all)
        {
            ScenarioManagementEvent e = row.Event;
            bool validKind = row.Cancelled ? e.eventType == ScenarioManagementEventType.OrderCancelled
                : e.eventType == ScenarioManagementEventType.WorkResolved;
            Resolution status = row.Cancelled ? Resolution.Cancelled
                : e.outcome == ScenarioManagementOutcome.Succeeded ? Resolution.Succeeded
                : e.outcome == ScenarioManagementOutcome.Failed ? Resolution.Failed : Resolution.Unresolved;
            bool ambiguous = duplicateEvents.Contains(e.eventId) || (!row.Cancelled && duplicateOrders.Contains(e.workOrderId));
            OrderLink link = ambiguous ? OrderLink.AmbiguousIdentity
                : row.Cancelled || row.Order == null ? OrderLink.MissingOrder
                : ValidOrder(row) ? OrderLink.Verified : OrderLink.InconsistentOrder;
            if (!validKind) { status = Resolution.Unresolved; link = OrderLink.InconsistentOrder; }
            var item = new RecordedWork
            {
                Year = row.Year, EventId = e.eventId, WorkOrderId = e.workOrderId, TaskType = e.taskType,
                Result = status, Link = link, TargetTreeId = e.targetTreeId ?? "",
                SpeciesId = e.speciesId ?? "", FailureReason = e.failureReason ?? "",
                PrescriptionId = link == OrderLink.Verified ? row.Order.prescriptionId : e.prescriptionId,
                CoverageUnitId = link == OrderLink.Verified ? row.Order.prescriptionCoverageUnitId ?? "" : "",
                RackId = link == OrderLink.Verified ? (int?)row.Order.rackId : null,
                GroupedHarvestMoney = status == Resolution.Succeeded
                    && (e.taskType == ScenarioWorkType.FellTree || e.taskType == ScenarioWorkType.SalvageDeadwood)
            };
            item.Felling = e.taskType != ScenarioWorkType.FellTree ? FellEvidence.NotFelling
                : status != Resolution.Succeeded || link != OrderLink.Verified || !item.RackId.HasValue || item.RackId.Value < 0
                    ? FellEvidence.NeedsClassification
                    : item.RackId.Value > 0 ? FellEvidence.RackOwnedAccess : FellEvidence.PossibleStandTreatment;
            answer.RecordedWork.Add(item);
        }

        foreach (var group in answer.RecordedWork.Where(w => w.Result == Resolution.Succeeded
                && w.Link == OrderLink.Verified && w.PrescriptionId > 0
                && !string.IsNullOrWhiteSpace(w.CoverageUnitId))
            .GroupBy(w => new { w.Year, w.PrescriptionId, w.CoverageUnitId })
            .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.PrescriptionId)
            .ThenBy(g => g.Key.CoverageUnitId, StringComparer.Ordinal))
        {
            var result = new ExecutedCoverageGroup
            {
                ResolutionYear = group.Key.Year, PrescriptionId = group.Key.PrescriptionId,
                CoverageUnitId = group.Key.CoverageUnitId
            };
            foreach (RecordedWork item in group.OrderBy(w => w.EventId).ThenBy(w => w.WorkOrderId))
            {
                result.EventIds.Add(item.EventId);
                result.WorkOrderIds.Add(item.WorkOrderId);
            }
            answer.ExecutedCoverage.Add(result);
        }

        foreach (var group in answer.RecordedWork.Where(w => w.TaskType == ScenarioWorkType.FellTree
                && w.Result == Resolution.Succeeded).GroupBy(w => w.Year).OrderBy(g => g.Key))
        {
            answer.PossibleTreatmentYears.Add(new PossibleTreatmentYear
            {
                Year = group.Key,
                CandidateFellEvents = group.Count(w => w.Felling == FellEvidence.PossibleStandTreatment),
                RackOwnedFellEvents = group.Count(w => w.Felling == FellEvidence.RackOwnedAccess),
                UnclassifiedFellEvents = group.Count(w => w.Felling == FellEvidence.NeedsClassification)
            });
        }
        return answer;
    }

    private sealed class WorkRow
    {
        public readonly int Year;
        public readonly ScenarioManagementEvent Event;
        public readonly ScenarioOneWorkOrder Order;
        public readonly bool Cancelled;
        public WorkRow(int year, ScenarioManagementEvent e, ScenarioOneWorkOrder order, bool cancelled)
        { Year = year; Event = e; Order = order; Cancelled = cancelled; }
    }

    private static bool ExactSnapshot(YearHistoryView year)
        => year.Snapshot != null && year.Snapshot.year == year.Year;

    private static bool ValidOrder(WorkRow w)
    {
        ScenarioManagementEvent e = w.Event;
        ScenarioOneWorkOrder o = w.Order;
        return !w.Cancelled && o != null && e.eventId > 0 && e.workOrderId > 0
            && e.year == w.Year && e.eventType == ScenarioManagementEventType.WorkResolved
            && o.workOrderId == e.workOrderId && o.type == e.taskType && o.resolvedYear == e.year
            && o.prescriptionId == e.prescriptionId
            && ((e.outcome == ScenarioManagementOutcome.Succeeded && o.status == ScenarioWorkStatus.Completed)
                || (e.outcome == ScenarioManagementOutcome.Failed && o.status == ScenarioWorkStatus.Failed));
    }

    private static StandObservation CopyObservation(ScenarioEcologicalSnapshot s)
    {
        var result = new StandObservation
        {
            Year = s.year, LivingTrees = s.livingTrees, BasalAreaM2PerHa = s.basalAreaM2PerHa,
            MeanDbhCm = s.meanDbhCm, MeanCanopy = s.meanCanopy, MeanLight = s.meanLight,
            OccupiedRegenerationCells = s.occupiedRegenerationCells, DeadwoodCount = s.deadwoodCount,
            DeadwoodVolumeM3 = s.deadwoodVolumeM3
        };
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (ScenarioSpeciesOutcome species in (s.species ?? new List<ScenarioSpeciesOutcome>())
            .Where(x => x != null).OrderBy(x => x.speciesId ?? "", StringComparer.Ordinal))
        {
            string id = species.speciesId ?? "";
            if (id.Length == 0 || !seen.Add(id)) result.SpeciesIdsUnique = false;
            result.Species.Add(new SpeciesObservation
            {
                SpeciesId = id, LivingTrees = species.livingTrees, BasalAreaM2PerHa = species.basalAreaM2PerHa,
                NaturalRegenerationCells = species.regenerationCells,
                NaturalRegenerationDensity = species.regenerationDensity,
                PlantedRegenerationCells = species.plantedRegenerationCells,
                ExactPlantedJuveniles = species.plantedJuveniles
            });
        }
        return result;
    }

    private static StandComparison Compare(StandObservation a, StandObservation b)
    {
        var result = new StandComparison
        {
            FromYear = a.Year, ToYear = b.Year, YearsBetween = b.Year - a.Year,
            LivingTreesChange = b.LivingTrees - a.LivingTrees,
            BasalAreaM2PerHaChange = b.BasalAreaM2PerHa - a.BasalAreaM2PerHa,
            MeanDbhCmChange = b.MeanDbhCm - a.MeanDbhCm,
            MeanCanopyChange = b.MeanCanopy - a.MeanCanopy,
            MeanLightChange = b.MeanLight - a.MeanLight,
            OccupiedRegenerationCellsChange = b.OccupiedRegenerationCells - a.OccupiedRegenerationCells,
            DeadwoodCountChange = b.DeadwoodCount - a.DeadwoodCount,
            DeadwoodVolumeM3Change = b.DeadwoodVolumeM3 - a.DeadwoodVolumeM3
        };
        var first = a.Species.ToDictionary(s => s.SpeciesId, StringComparer.Ordinal);
        var second = b.Species.ToDictionary(s => s.SpeciesId, StringComparer.Ordinal);
        foreach (string id in first.Keys.Union(second.Keys).OrderBy(x => x, StringComparer.Ordinal))
        {
            if (!first.ContainsKey(id) || !second.ContainsKey(id))
            {
                result.Species.Add(new SpeciesComparison { SpeciesId = id, State = ObservationState.NotRecorded });
                continue;
            }
            SpeciesObservation x = first[id], y = second[id];
            result.Species.Add(new SpeciesComparison
            {
                SpeciesId = id, State = ObservationState.Recorded,
                LivingTreesChange = y.LivingTrees - x.LivingTrees,
                BasalAreaM2PerHaChange = y.BasalAreaM2PerHa - x.BasalAreaM2PerHa,
                NaturalRegenerationCellsChange = y.NaturalRegenerationCells - x.NaturalRegenerationCells,
                NaturalRegenerationDensityChange = y.NaturalRegenerationDensity - x.NaturalRegenerationDensity,
                PlantedRegenerationCellsChange = y.PlantedRegenerationCells - x.PlantedRegenerationCells,
                ExactPlantedJuvenilesChange = y.ExactPlantedJuveniles - x.ExactPlantedJuveniles
            });
        }
        return result;
    }
}
