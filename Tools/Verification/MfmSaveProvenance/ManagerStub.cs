// Lets the REAL ScenarioOneManager.Prescriptions.cs compile and run offline. Capture/Restore mirror the three production
// call sites (CaptureSaveData, RestoreSaveData, InitializeNewScenario); the runner's source audit proves those call sites exist.
using System.Collections.Generic;
using System.Linq;

public sealed partial class ScenarioOneManager
{
    public ScenarioOneSaveData Capture() => new ScenarioOneSaveData
    {
        nextPrescriptionId = nextPrescriptionId,
        prescriptions = ClonePrescriptions(prescriptions)
    };
    public void Restore(ScenarioOneSaveData data, int saveVersion)
    {
        ScenarioOneSaveMigration.NormalizePrescriptions(data, saveVersion);
        RestorePrescriptions(data, saveVersion);
    }
    public void Reset() => ClearPrescriptions();
}
