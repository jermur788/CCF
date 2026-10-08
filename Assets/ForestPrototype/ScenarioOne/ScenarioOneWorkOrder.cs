using System;
using UnityEngine;

public enum ScenarioWorkType
{
    FellTree,
    PlantJuvenile,
    RemoveRegeneration,
    PruneTree,
    SalvageDeadwood
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
    RetainAsFallenDeadwood,
    KeepForUse
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
    public float targetCrownBaseHeightM;
    public bool requiresCropTree;
    // v12 cell-based orders remain on the historical cohort pathway.
    public bool exactPosition;
    public string validationMessage = "";
    public int createdYear;
    public int resolvedYear = -1;
    public CCF.Forestry.WorkEconomy.WorkExecutionMethod executionMethod;
    public bool installShelter;
    public int harvestJobId = -1;

    public bool IsOpen => status == ScenarioWorkStatus.Pending || status == ScenarioWorkStatus.Approved;

    public string ShortLabel
    {
        get
        {
            switch (type)
            {
                case ScenarioWorkType.FellTree: return "Fell " + targetTreeId;
                case ScenarioWorkType.SalvageDeadwood: return "Salvage " + targetTreeId;
                case ScenarioWorkType.PlantJuvenile: return "Plant " + speciesId + " at ("
                    + worldPosition.x.ToString("0.0") + ", " + worldPosition.z.ToString("0.0") + ")";
                case ScenarioWorkType.RemoveRegeneration: return string.IsNullOrEmpty(speciesId)
                    ? "Clear competing vegetation in cell " + cellIndex
                    : "Remove " + speciesId + " regeneration in cell " + cellIndex;
                case ScenarioWorkType.PruneTree: return "Prune " + targetTreeId + " to "
                    + (targetCrownBaseHeightM > 0f ? targetCrownBaseHeightM : expectedRegenerationDensity).ToString("0.0") + " m";
                default: return type.ToString();
            }
        }
    }
}
