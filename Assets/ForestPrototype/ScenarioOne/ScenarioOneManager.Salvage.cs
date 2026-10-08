using System;
using System.Linq;
using CCF.Forestry.WorkEconomy;
using UnityEngine;

public sealed partial class ScenarioOneManager
{
    public static bool IsHarvestOrder(ScenarioOneWorkOrder order)
        => order != null && (order.type == ScenarioWorkType.FellTree || order.type == ScenarioWorkType.SalvageDeadwood);

    public ScenarioOneWorkOrder OpenSalvageOrder(string treeId)
        => workOrders.FirstOrDefault(order => order.IsOpen && order.type == ScenarioWorkType.SalvageDeadwood && order.targetTreeId == treeId);

    public bool ToggleSalvagePlan(string treeId)
    {
        var order = OpenSalvageOrder(treeId);
        if (order != null) return order.status == ScenarioWorkStatus.Pending ? RemovePendingOrder(order.workOrderId) : CancelApprovedOrder(order.workOrderId);
        return TryDesignateSalvage(treeId, FellingMaterialOutcome.SellAndExtract);
    }

    public bool CanSalvage(string treeId)
        => ecology != null && ecology.StormModelVersion == StormModel.WindthrowV1
            && windVictims.TryGetValue(treeId ?? "", out ForestTree tree) && tree != null
            && tree.IsBiologicallyDead && tree.MortalityCause == "windthrow"
            && deadwoodRecords.Any(record => record.treeId == treeId && record.remainingVolumeM3 > 0f && record.DecayClass <= 1);

    public bool TryDesignateSalvage(string treeId, FellingMaterialOutcome choice)
    {
        if (!CanManage() || !CanSalvage(treeId) || HasOpenTreeOrder(treeId, ScenarioWorkType.SalvageDeadwood)
            || (choice != FellingMaterialOutcome.SellAndExtract && choice != FellingMaterialOutcome.KeepForUse))
            return false;
        ScenarioDeadwoodRecord record = deadwoodRecords.Single(item => item.treeId == treeId);
        var order = new ScenarioOneWorkOrder
        {
            workOrderId = nextWorkOrderId++, type = ScenarioWorkType.SalvageDeadwood, status = ScenarioWorkStatus.Pending,
            targetTreeId = treeId, speciesId = record.speciesId, worldPosition = record.worldPosition, cellIndex = record.cellIndex,
            fellingOutcome = choice, executionMethod = WorkExecutionMethod.Contractor, createdYear = CurrentYear,
            expectedVolumeM3 = record.remainingVolumeM3, harvestJobId = CurrentYear + 1
        };
        workOrders.Add(order);
        RecordOrderEvent(order, ScenarioManagementEventType.OrderCreated, ScenarioManagementOutcome.None, CurrentYear);
        InvalidateEconomyQuotes();
        feedback = "Salvage designated: " + treeId + ". Approve the grouped work quote before advancing. Unselected stems remain deadwood.";
        return true;
    }

    private void ResolveSalvage(ScenarioOneWorkOrder order, ScenarioAnnualReport report)
    {
        if (!CanSalvage(order.targetTreeId))
        { Fail(order, report, "Fallen stem is missing, already salvaged or too decayed; no salvage charge."); return; }
        ScenarioDeadwoodRecord record = deadwoodRecords.Single(item => item.treeId == order.targetTreeId);
        float volume = record.remainingVolumeM3;
        // Preserve the one historical record/root plate. Zero remaining stem
        // volume derives a salvaged presentation and carries no habitat volume.
        record.remainingVolumeM3 = 0f;
        record.lastDecayYear = report.year;
        var visual = transform.Find("Fallen Log " + record.deadwoodId)?.GetComponent<ScenarioWindthrowVisual>();
        if (visual != null) visual.Refresh(record);
        order.expectedVolumeM3 = volume; order.status = ScenarioWorkStatus.Completed; order.resolvedYear = report.year;
        report.completedTasks++;
        if (order.fellingOutcome == FellingMaterialOutcome.KeepForUse) report.keptForUseVolumeM3 += volume;
        else report.harvestedVolumeM3 += volume;
        SpawnFellingResidueVisual(order);
    }
}
