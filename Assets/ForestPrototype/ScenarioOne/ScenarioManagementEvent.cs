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
    CenturyReviewed
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
    TreeRetainedAsDeadwood
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
}
