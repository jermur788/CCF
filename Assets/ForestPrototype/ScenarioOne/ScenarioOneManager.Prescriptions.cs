using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// MFM-06 durable prescription state (save v23). Storage foundation only: this holds what a save restores and writes
// back unchanged. It creates no prescriptions and approves nothing; proposal and approval behaviour is MFM-07.
public sealed partial class ScenarioOneManager
{
    private List<PrescriptionSaveData> prescriptions = new List<PrescriptionSaveData>();
    private int nextPrescriptionId = 1;

    public IReadOnlyList<PrescriptionSaveData> Prescriptions => prescriptions;
    public int NextPrescriptionId => nextPrescriptionId;

    private void ClearPrescriptions()
    {
        prescriptions = new List<PrescriptionSaveData>();
        nextPrescriptionId = 1;
    }

    // A pre-v23 save was normalized to the empty defaults before this runs, and is read as "no prescription state"
    // regardless: nothing is synthesized from existing marks or work orders. The allocator never moves backwards
    // past an id already in use.
    private void RestorePrescriptions(ScenarioOneSaveData data, int saveVersion)
    {
        if (saveVersion < 23 || data == null)
        {
            ClearPrescriptions();
            return;
        }
        prescriptions = ClonePrescriptions(data.prescriptions);
        nextPrescriptionId = Mathf.Max(1, data.nextPrescriptionId);
        if (prescriptions.Count > 0)
            nextPrescriptionId = Mathf.Max(nextPrescriptionId, prescriptions.Max(item => item.prescriptionId) + 1);
    }

    private static List<PrescriptionSaveData> ClonePrescriptions(List<PrescriptionSaveData> source)
    {
        if (source == null) return new List<PrescriptionSaveData>();
        return source.Where(item => item != null)
            .Select(item => JsonUtility.FromJson<PrescriptionSaveData>(JsonUtility.ToJson(item))).ToList();
    }
}
