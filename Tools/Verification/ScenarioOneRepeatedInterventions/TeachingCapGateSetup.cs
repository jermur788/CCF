#if UNITY_EDITOR
using System;
using UnityEngine;

// Explicit old-curriculum fixture, not a production model override. Installed only by the T-03 runner.
// The unchanged RackLesson/MenuTutorial gates predate Rack V2. Fresh V2 has no racks/orders yet.
public static class TeachingCapGateSetup
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Prepare()
    {
        if (Environment.GetEnvironmentVariable("CCF_T03_LEGACY") != "1") return;
        var saves = UnityEngine.Object.FindFirstObjectByType<ForestSaveController>();
        var data = saves.CaptureData();
        data.scenarioOne.rackPlanningModel = RackPlanningModel.Legacy;
        data.scenarioOne.rackPlanConfirmed = false;
        data.scenarioOne.nextRackId = 1;
        data.scenarioOne.racks.Clear();
        if (!saves.LoadData(data, false)) throw new InvalidOperationException("T03 legacy fixture rejected");
        Debug.Log("T03_LEGACY_FIXTURE model=0 geometry=" + data.standGeometryModel);
    }
}
#endif
