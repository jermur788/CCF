using System;
using CCF.Forestry.TimberYield;
using UnityEngine;

public static class WindthrowSalvage
{
    // Candidate [C] grading abstraction: basal damage, increasing with delay.
    // Uses existing WindDamage flags, not a new tariff or material category.
    public static StemMeasurements Measurements(ScenarioDeadwoodRecord record, int resolutionYear)
    {
        var stem = MerchantableStemModel.FromMetres(record.treeId, record.speciesId, record.originalDiameterCm,
            record.originalHeightMeters, record.remainingVolumeM3, "[S] Existing remaining windthrow stem budget; [C] basal damage window");
        int delay = Math.Max(0, resolutionYear - record.fallenYear - 1);
        int damagedEnd = Mathf.Min(stem.TotalHeightMm, Mathf.RoundToInt(Mathf.Clamp(record.originalHeightMeters * .1f, 1f, 2f) * 1000) + delay * 250);
        stem.QualitySections = new[] { new StemQualitySection { StartHeightMm = 0, EndHeightMm = damagedEnd, Flags = StemQualityFlags.WindDamage } };
        return stem;
    }
}
