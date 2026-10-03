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
}
