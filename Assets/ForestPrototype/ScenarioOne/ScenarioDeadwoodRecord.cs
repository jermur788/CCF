using System;
using UnityEngine;

// A retained fallen stem left by Scenario One management. Deadwood is recorded
// in the management layer: it never feeds back into Forestry tree growth,
// competition or regeneration. Decay is deterministic and diagnostic, used for
// habitat history and later soundscape routing.
[Serializable]
public sealed class ScenarioDeadwoodRecord
{
    public string deadwoodId = "";
    public string treeId = "";
    public string speciesId = "";
    public Vector3 worldPosition;
    public int cellIndex = -1;
    public float originalVolumeM3;
    public float remainingVolumeM3;
    public float originalHeightMeters;
    public float originalDiameterCm;
    public int fallenYear;
    public int lastDecayYear;
    public string visualName = "";

    public int YearsSinceFall(int currentYear) => Mathf.Max(0, currentYear - fallenYear);

    // Deterministic 0..5 decay class from remaining volume. Fresh logs start at 0;
    // class 5 is a well-decayed legacy log. Diagnostic bands only.
    public int DecayClass => VolumeLossFraction <= 0.05f ? 0
        : VolumeLossFraction <= 0.20f ? 1
        : VolumeLossFraction <= 0.40f ? 2
        : VolumeLossFraction <= 0.60f ? 3
        : VolumeLossFraction <= 0.80f ? 4 : 5;

    public float VolumeLossFraction =>
        originalVolumeM3 <= 0.0001f ? 1f : 1f - Mathf.Clamp01(remainingVolumeM3 / originalVolumeM3);
}

public static class ScenarioDeadwood
{
    // [D] Provisional coarse-wood decay calibration. Roughly 3% volume loss per
    // year with a floor at 12% of the original stem, so logs persist for decades
    // of scenario play while recording a visible decay trajectory.
    public const float AnnualVolumeLossFraction = 0.03f;
    public const float MinimumVolumeFraction = 0.12f;

    // Habitat weighting [D] by decay class: early decay supports beetles and
    // cavity users, later decay supports fungi and saproxylic communities.
    public static readonly float[] HabitatValueByDecayClass = { 0.35f, 0.70f, 1.00f, 0.95f, 0.75f, 0.55f };

    public static float HabitatValue(ScenarioDeadwoodRecord record)
    {
        if (record == null || record.remainingVolumeM3 <= 0f)
            return 0f;
        int decayClass = Mathf.Clamp(record.DecayClass, 0, HabitatValueByDecayClass.Length - 1);
        return record.remainingVolumeM3 * HabitatValueByDecayClass[decayClass];
    }

    // Advances decay exactly once per ecological year. Returns the volume lost.
    public static float Decay(ScenarioDeadwoodRecord record, int year)
    {
        if (record == null || record.originalVolumeM3 <= 0f || year <= record.lastDecayYear)
            return 0f;
        float floor = record.originalVolumeM3 * MinimumVolumeFraction;
        float before = record.remainingVolumeM3;
        float target = Mathf.Max(floor, before * Mathf.Pow(1f - AnnualVolumeLossFraction, year - record.lastDecayYear));
        record.remainingVolumeM3 = target;
        record.lastDecayYear = year;
        return Mathf.Max(0f, before - target);
    }

    public static float TotalVolume(System.Collections.Generic.IEnumerable<ScenarioDeadwoodRecord> records)
    {
        float total = 0f;
        if (records == null)
            return 0f;
        foreach (ScenarioDeadwoodRecord record in records)
            if (record != null)
                total += record.remainingVolumeM3;
        return total;
    }

    public static float TotalHabitatValue(System.Collections.Generic.IEnumerable<ScenarioDeadwoodRecord> records)
    {
        float total = 0f;
        if (records == null)
            return 0f;
        foreach (ScenarioDeadwoodRecord record in records)
            total += HabitatValue(record);
        return total;
    }
}
