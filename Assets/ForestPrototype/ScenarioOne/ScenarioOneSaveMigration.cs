using System.Collections.Generic;
using CCF.Forestry.WorkEconomy;

// v15 migration changes only newly introduced scenario fields. Biology and pressure are untouched.
public static class ScenarioOneSaveMigration
{
    public static void NormalizeLegacy(ScenarioOneSaveData data, int version)
    {
        if (data == null || version >= 15) return;
        data.shelters = new List<BrowseShelter>();
        data.protectedAreas = new List<BrowseProtectedArea>();
        data.ownerMinutesUsedThisYear = 0;
        if (data.workOrders != null)
            foreach (var order in data.workOrders)
                if (order != null) { order.executionMethod = WorkExecutionMethod.Contractor; order.installShelter = false; order.harvestJobId = -1; }
        if (data.annualReports != null)
            foreach (var report in data.annualReports)
                if (report != null) { report.harvestMinimumAdjustmentCents = 0; report.ownerMinutes = 0; report.timberSales = new List<ScenarioTimberSale>(); }
        if (data.managementEvents != null)
            foreach (var entry in data.managementEvents)
                if (entry != null) { entry.executionMethod = WorkExecutionMethod.Contractor; entry.ownerMinutes = 0; }
    }

    // v21 normalization for every save that predates the Rack Planning & Field Marking V2 fields.
    // JsonUtility turns missing fields into zero/null (so a v20 save would look like nextRackId 0),
    // and the accepted policy for pre-V2 worlds is explicit: model 0, no plan, and every existing
    // order effectively rackId 0. Nothing is synthesized: only a v21 save that states rack data
    // brings it back (ScenarioOneManager.RestoreSaveData reads it only when the save is v21).
    public static void NormalizeRackV2(ScenarioOneSaveData data, int version)
    {
        if (data == null || version >= 21) return;
        data.rackPlanningModel = RackPlanningModel.Legacy;
        data.rackPlanConfirmed = false;
        data.nextRackId = 1;
        data.racks = new List<RackSaveData>();
        if (data.workOrders != null)
            foreach (var order in data.workOrders)
                if (order != null) order.rackId = 0;
    }

    // v22 normalization: a save older than the objective history field has no record of when anything
    // was achieved. It gets the empty list, which reads as "not recorded"; nothing is derived or
    // back-filled here (ScenarioOneManager.RestoreSaveData reads the list only from a v22 save).
    public static void NormalizeObjectiveHistory(ScenarioOneSaveData data, int version)
    {
        if (data == null || version >= 22) return;
        data.objectiveAchievements = new List<ScenarioObjectiveAchievement>();
    }
}
