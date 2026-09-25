using System;
using UnityEngine;

public enum ScenarioWorkType
{
    FellTree,
    PlantJuvenile,
    RemoveRegeneration,
    PruneTree
}

public enum ScenarioWorkStatus
{
    Pending,
    Approved,
    Completed,
    Failed
}

public enum FellingMaterialOutcome
{
    SellAndExtract,
    RetainAsFallenDeadwood
}

[Serializable]
public sealed class ScenarioOneWorkOrder
{
    public int workOrderId;
    public ScenarioWorkType type;
    public ScenarioWorkStatus status;
    public string targetTreeId = "";
    public string speciesId = "";
    public string stockItemId = "";
    public int requiredStockQuantity;
    public Vector3 worldPosition;
    public int cellIndex = -1;
    public FellingMaterialOutcome fellingOutcome;
    public int estimatedMinutes;
    public long estimatedCostCents;
    public long expectedRevenueCents;
    public float expectedVolumeM3;
    public float expectedRegenerationDensity;
    public string validationMessage = "";
    public int createdYear;
    public int resolvedYear = -1;

    public bool IsOpen => status == ScenarioWorkStatus.Pending || status == ScenarioWorkStatus.Approved;

    public string ShortLabel
    {
        get
        {
            switch (type)
            {
                case ScenarioWorkType.FellTree: return "Fell " + targetTreeId;
                case ScenarioWorkType.PlantJuvenile: return "Plant " + speciesId + " in cell " + cellIndex;
                case ScenarioWorkType.RemoveRegeneration: return "Remove " + speciesId + " regeneration in cell " + cellIndex;
                case ScenarioWorkType.PruneTree: return "Prune " + targetTreeId;
                default: return type.ToString();
            }
        }
    }
}
