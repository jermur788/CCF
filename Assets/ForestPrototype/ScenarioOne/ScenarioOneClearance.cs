using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed partial class ScenarioOneManager
{
    // Read-only snapshot shared by the walking preview and world-effect resolver.
    public ClearanceTargets QueryClearance(ClearanceFootprint footprint, int treatmentYear = -1)
    {
        var result = new ClearanceTargets { Footprint = footprint };
        if (ecology?.Cells == null) return result;
        int year = treatmentYear < 0 ? ecology.EcologicalYear + 1 : treatmentYear;
        var prior = clearancePatches.Where(p => p.createdYear == year).ToList();
        // An identical same-year spot is already treated. Return no targets
        // directly, rather than relying on a numerically integrated ratio
        // rounding to exactly one. X/Z are the authoritative circle coordinates.
        if (footprint.IsCircle && prior.Exists(p => p.center.x == footprint.Center.x
            && p.center.z == footprint.Center.z && p.radiusMeters == footprint.Radius))
        { result.AlreadyTreated = true; return result; }
        float half = ecology.CellSizeMeters * .5f, area = ecology.CellSizeMeters * ecology.CellSizeMeters;
        var next = new List<PlantingClearancePatch>(prior);
        if (footprint.IsCircle) next.Add(new PlantingClearancePatch { center = footprint.Center, radiusMeters = footprint.Radius });
        for (int i = 0; i < ecology.CellCount; i++)
        {
            ForestEcologyCell cell = ecology.Cells[i];
            if (!footprint.IsCircle && i != footprint.CellIndex) continue;
            if (footprint.IsCircle && (Mathf.Abs(cell.Center.x - footprint.Center.x) > half + footprint.Radius
                || Mathf.Abs(cell.Center.y - footprint.Center.z) > half + footprint.Radius)) continue;
            float remaining = footprint.IsCircle
                ? Mathf.Clamp01((area - ClearanceUnionArea(cell.Center, half, next))
                    / Mathf.Max(.0001f, area - ClearanceUnionArea(cell.Center, half, prior))) : 0f;
            if (remaining >= 1f) continue;
            foreach (ForestRegenerationCohort cohort in cell.Regeneration)
            {
                if (cohort == null || cohort.Density <= 0f) continue;
                result.Cohorts.Add(new ClearanceCohortTarget { CellIndex = i, Cohort = cohort,
                    RemainingFraction = remaining, DisplayPosition = ecology.RegenerationDisplayPosition(i, cohort.SpeciesId) });
                result.Density += cohort.Density * (1f - remaining);
            }
            // A circular spot is persisted as existing clearance history rather
            // than cutting back the entire containing cell's habitat cover.
            if (!footprint.IsCircle && i < understoreyCells.Count)
            {
                ScenarioUnderstoreyCell u = understoreyCells[i];
                if (u != null && u.ferns + u.grasses + u.forbs + u.shrubs > 0f
                    || (u != null && ecology.RegenerationModelVersion == RegenerationModel.Competition && CompetitionCellExposure(i) > 0f)) result.Understorey.Add(u);
            }
        }
        foreach (PlantedJuvenile j in plantedJuveniles)
            if (j != null && j.alive && !j.legacyCohortManaged && string.IsNullOrEmpty(j.promotedTreeId)
                && footprint.Contains(j.position) && (!footprint.IsCircle || !prior.Any(p =>
                    new Vector2(j.position.x - p.center.x, j.position.z - p.center.z).sqrMagnitude <= p.radiusMeters * p.radiusMeters))) result.Juveniles.Add(j);
        foreach (HabitatVegetationSite site in ScenarioHabitatVisuals.VegetationSites(ecology, understoreyCells,
                     habitatVisuals != null ? habitatVisuals.OldWoodlandSourceConfidence : 0f))
            if (site.IsCompeting && footprint.Contains(site.Position) && !IsVegetationDisplayCleared(site.Position))
                result.GroundPlants.Add(site.Position);
        return result;
    }

    private void ApplyClearance(ClearanceTargets targets, int year)
    {
        foreach (ClearanceCohortTarget target in targets.Cohorts)
        {
            target.Cohort.Density *= target.RemainingFraction;
            if (target.Cohort.Density <= .001f) ecology.Cells[target.CellIndex].RemoveCohortIfEmpty(target.Cohort);
        }
        // Regeneration model 1: partially cleared bands rejoin the band
        // invariants (sub-threshold remainders form the accumulator). No-op in model 0.
        foreach (int cellIndex in targets.Cohorts.Select(t => t.CellIndex).Distinct())
            ecology.NormalizeRegenerationBands(cellIndex);
        foreach (PlantedJuvenile juvenile in targets.Juveniles) juvenile.alive = false;
        foreach (ScenarioUnderstoreyCell u in targets.Understorey)
        {
            u.ferns = u.grasses = u.forbs = u.shrubs = 0f;
            u.lastUpdatedYear = year; // recolonisation resumes at the next annual step
        }
        if (ecology.RegenerationModelVersion == RegenerationModel.Competition && !targets.Footprint.IsCircle)
            ResetCellCompetition(targets.Footprint.CellIndex, year);
        InvalidateCompetition();
        ecology.RefreshRegenerationDisplays();
    }

    // Saved spot treatments and completed area orders suppress presentation in
    // their treatment year. Subsequent annual ecology can recolonise normally.
    public bool IsVegetationDisplayCleared(Vector3 position)
    {
        if (ecology == null) return false;
        int year = ecology.EcologicalYear;
        foreach (PlantingClearancePatch p in clearancePatches)
            if (p.createdYear == year && new Vector2(position.x - p.center.x, position.z - p.center.z).sqrMagnitude
                <= p.radiusMeters * p.radiusMeters) return true;
        int cell = ecology.GetCellIndex(position);
        return workOrders.Any(o => o.type == ScenarioWorkType.RemoveRegeneration && string.IsNullOrEmpty(o.speciesId)
            && o.status == ScenarioWorkStatus.Completed && o.resolvedYear == year && o.cellIndex == cell);
    }
}
