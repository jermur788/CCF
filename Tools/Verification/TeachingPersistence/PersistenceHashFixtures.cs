using System;
using System.Collections.Generic;
using UnityEngine;
// Shared verbatim with v23 baseline and v24 candidate. Reflection only sets the new field in the candidate.
public static class PersistenceHashFixtures
{
    public static string Hashes(ForestSaveData d) => ScenarioReferenceArchive.WorldHash(d) + "|" + ScenarioReferenceArchive.CurrentWorldHash(d)
        + "|" + ScenarioReferenceArchive.LegacyCompatibleWorldHash(d) + "|" + ScenarioReferenceArchive.LegacyV15WorldHash(d)
        + "|" + ScenarioReferenceArchive.LegacyV16WorldHash(d) + "|" + ScenarioReferenceArchive.LegacyV17WorldHash(d) + "|" + ScenarioReferenceArchive.LegacyV18WorldHash(d);
    public static int Main()
    {
        foreach (int geometry in new[] { 0, 1 }) foreach (bool rack in new[] { false, true }) foreach (bool rx in new[] { false, true })
        foreach (int version in new[] { 18, 21, 22, 23, 24 }) foreach (bool latch in new[] { false, true })
        {
            var d = new ForestSaveData { version = Math.Min(version, ForestSaveData.CurrentVersion), standGeometryModel = geometry, scenarioOne = new ScenarioOneSaveData() };
            if (rack) { d.scenarioOne.rackPlanConfirmed = true; d.scenarioOne.nextRackId = 2; d.scenarioOne.racks.Add(new RackSaveData { rackId = 1 }); }
            if (rx) { d.scenarioOne.nextPrescriptionId = 2; d.scenarioOne.prescriptions.Add(new PrescriptionSaveData { prescriptionId = 1, sourceRackId = 1, ruleVersion = "MFM-V1-RULE-1", calibrationVersion = "MFM-V1-CAL-1", sourceYear = 2, sampleCropTreeIds = new List<string>{"P1"}, sampleRemovalIds = new List<string>{"P2"}, approvalState = PrescriptionApprovalState.Approved, approvalYear = 3 }); d.scenarioOne.workOrders.Add(new ScenarioOneWorkOrder { workOrderId = 1, prescriptionId = 1, prescriptionCoverageUnitId = "R1C", prescriptionCropTreeId = "P1", prescriptionReason = 1 }); d.scenarioOne.managementEvents.Add(new ScenarioManagementEvent { eventId = 1, prescriptionId = 1 }); }
            typeof(ScenarioOneSaveData).GetField("firstGuidedRackReviewComplete")?.SetValue(d.scenarioOne, latch);
            Console.WriteLine(geometry + "/" + rack + "/" + rx + "/" + version + "/" + latch + " " + Hashes(d));
        }
        return 0;
    }
}
