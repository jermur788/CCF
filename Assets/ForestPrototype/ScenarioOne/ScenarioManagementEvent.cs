using System;
using UnityEngine;

public enum ScenarioManagementEventType
{
    StockPurchased,
    OrderCreated,
    OrderApproved,
    OrderCancelled,
    WorkResolved,
    YearAdvanced,
    ScenarioCompleted,
    ScenarioFailed,
    CenturyReviewed,
    // Append-only v20 contract extension; all earlier numeric values remain unchanged.
    AnnualReviewAcknowledged = 9
}

public enum ScenarioManagementOutcome
{
    None,
    Succeeded,
    Failed,
    Cancelled
}

// Records what happened biologically, independently of a work order's intent.
public enum ScenarioEcologicalTreatment
{
    None,
    TreeFelledAndExtracted,
    JuvenilePlanted,
    RegenerationRemoved,
    TreePruned,
    TreeRetainedAsDeadwood,
    WindthrowSalvaged
}

[Serializable]
public sealed class ScenarioManagementEvent
{
    public int eventId;
    public int year;
    public ScenarioManagementEventType eventType;
    public ScenarioManagementOutcome outcome;
    public ScenarioEcologicalTreatment ecologicalTreatment;
    public int workOrderId;
    public ScenarioWorkType taskType;
    public string speciesId = "";
    public string targetTreeId = "";
    public int cellIndex = -1;
    public Vector3 worldPosition;
    public string stockItemId = "";
    public int quantity;
    public int stockUsed;
    public long stockCostCents;
    public long estimatedContractorCostCents;
    public long contractorCostCents;
    public long timberRevenueCents;
    public float biologicalVolumeM3;
    public float regenerationDensityRemoved;
    public long cashDeltaCents;
    public long closingCashCents;
    public string failureReason = "";
    public CCF.Forestry.WorkEconomy.WorkExecutionMethod executionMethod;
    public int ownerMinutes;
    // Version 23 (MFM-06): the prescription that generated the work this event records; 0 = not prescription-generated.
    // Declared last so the hash compatibility path can remove exactly this default text.
    public int prescriptionId;
}
