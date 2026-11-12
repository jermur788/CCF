using System;
using System.Collections.Generic;

// OFFLINE TEST ADAPTER ONLY: minimum signatures copied from inspected production source.
// Never ship this file under Assets; production compilation uses the actual saved/history classes.
// Passing this fixture verifies helper behaviour in isolation, NOT Unity/Assembly-CSharp binding.
public enum ScenarioWorkType { FellTree, PlantJuvenile, RemoveRegeneration, PruneTree, SalvageDeadwood }
public enum ScenarioWorkStatus { Pending, Approved, Completed, Failed }
public enum ScenarioManagementOutcome { None, Succeeded, Failed, Cancelled }
public enum ScenarioManagementEventType
{
    StockPurchased, OrderCreated, OrderApproved, OrderCancelled, WorkResolved, YearAdvanced,
    ScenarioCompleted, ScenarioFailed, CenturyReviewed, AnnualReviewAcknowledged
}
public sealed class ScenarioManagementEvent
{
    public int eventId, year, workOrderId, prescriptionId;
    public ScenarioWorkType taskType;
    public ScenarioManagementEventType eventType;
    public ScenarioManagementOutcome outcome;
    public string targetTreeId = "", speciesId = "", failureReason = "";
}
public sealed class ScenarioOneWorkOrder
{
    public int workOrderId, resolvedYear = -1, rackId, prescriptionId;
    public string prescriptionCoverageUnitId = "";
    public ScenarioWorkType type;
    public ScenarioWorkStatus status;
}
public sealed class ResolvedWorkRecord
{
    public ScenarioManagementEvent Event;
    public ScenarioOneWorkOrder Order;
    public bool OrderStillExists;
}
public sealed class HistoryCancellation
{
    public ScenarioManagementEvent Event;
    public int WorkOrderId, Year;
}
public sealed class ScenarioSpeciesOutcome
{
    public string speciesId = "";
    public int livingTrees, regenerationCells, plantedRegenerationCells, plantedJuveniles;
    public float basalAreaM2PerHa, regenerationDensity;
}
public sealed class ScenarioEcologicalSnapshot
{
    public int year, livingTrees, occupiedRegenerationCells, deadwoodCount;
    public float basalAreaM2PerHa, meanDbhCm, meanCanopy, meanLight, deadwoodVolumeM3;
    public List<ScenarioSpeciesOutcome> species = new List<ScenarioSpeciesOutcome>();
}
public sealed class YearHistoryView
{
    public int Year;
    public bool YearAdvancedRecorded;
    public object Report;
    public bool HasCompletedRecord => Report != null || YearAdvancedRecorded;
    public ScenarioEcologicalSnapshot Snapshot;
    public readonly List<ResolvedWorkRecord> ResolvedWork = new List<ResolvedWorkRecord>();
    public readonly List<HistoryCancellation> Cancellations = new List<HistoryCancellation>();
}
