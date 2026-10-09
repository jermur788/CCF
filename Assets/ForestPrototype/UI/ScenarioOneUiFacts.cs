using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Player-facing wording built from authoritative diagnoses. Browsing is shown
// qualitatively: the site pressure band plus the stem's protection state. No
// per-leader probability is displayed (not an accepted player-facing number).
public static class ScenarioOneUiFacts
{
    public static string BrowseState(RegenerationDiagnosis d, float backgroundPressure)
    {
        switch (d.Browse.Protection)
        {
            case BrowseProtectionState.EffectiveShelter:
                return d.ShelterStepsRemaining >= 0 ? $"Protected by shelter ({d.ShelterStepsRemaining} yr left)" : "Protected by shelter";
            case BrowseProtectionState.ExpiredShelter: return "Shelter expired — unprotected";
            case BrowseProtectionState.FailedShelter: return "Shelter failed — unprotected";
        }
        if (backgroundPressure <= 0f) return "No browsing pressure";
        if (d.Limit == RegenerationLimit.AboveBrowseReach || d.Browse.Reason == BrowseExposureReason.AboveBrowseReach)
            return "Above deer reach";
        if (d.Browse.Reason == BrowseExposureReason.NotPalatable) return "Rarely browsed (unpalatable)";
        return Capitalise(BrowsingConditions.PressureBand(backgroundPressure)) + " browse risk — unprotected";
    }

    // One causal sentence; it explains the binding constraint, never prescribes an action.
    public static string Why(RegenerationDiagnosis d, ForestEcologyController ecology = null, int cell = -1)
    {
        if (!d.HasJuvenile && ecology != null && cell >= 0 && cell < ecology.CellCount
            && !ecology.Cells[cell].SeedRainBySpecies.Values.Any(seed => seed > 0f))
            return "No seed is reaching this spot yet; an opening alone cannot establish natural regeneration.";
        if (!d.HasJuvenile)
            return d.Light < 0.10f ? "Too dark under the canopy for seedlings to establish." : "No regeneration has established here yet.";
        switch (d.Limit)
        {
            case RegenerationLimit.Promoting: return "Tall enough and in enough light to join the canopy layer.";
            case RegenerationLimit.LightLimited:
                return d.Light < d.PromotionMinimumLight && d.LightGrowthResponse >= d.PoorLightThreshold
                    ? "Growing, but light is below what it needs to join the canopy."
                    : "Light here is too low for strong growth (" + d.LightBand + ").";
            case RegenerationLimit.BrowsedExposed: return "Unprotected leaders are still within deer browsing height.";
            case RegenerationLimit.Protected: return "Protected from browsing; light decides its growth.";
            case RegenerationLimit.AboveBrowseReach: return "Above deer reach; light decides its growth.";
            default: return "Light is sufficient for continued growth.";
        }
    }

    public static string RegenerationSummary(ForestEcologyController ecology, int cell, IReadOnlyList<PlantedJuvenile> planted)
    {
        if (ecology == null || cell < 0) return "—";
        var parts = new List<string>();
        ForestEcologyCell c = ecology.Cells[cell];
        foreach (var group in (planted ?? new List<PlantedJuvenile>())
                     .Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId) && ecology.GetCellIndex(j.position) == cell)
                     .GroupBy(j => j.speciesId).OrderBy(g => g.Key))
            parts.Add($"planted {SpeciesName(group.Key)} ×{group.Count()}");
        foreach (ForestRegenerationCohort cohort in c.Regeneration.Where(r => r != null && r.Species != null && r.Density > 0f))
            parts.Add($"{cohort.Species.DisplayName} seedlings {cohort.Height.ToString("0.0", UiKit.Inv)} m");
        return parts.Count == 0 ? "none" : string.Join(" · ", parts);
    }

    public static string SpeciesName(string speciesId)
    {
        ForestTreeSpawner spawner = Object.FindFirstObjectByType<ForestTreeSpawner>();
        TreeSpeciesDefinition species = spawner != null ? spawner.ResolveSpecies(speciesId) : null;
        if (species != null) return species.DisplayName;
        switch (speciesId)
        {
            case "sitka-spruce": return "Sitka spruce";
            case "sessile-oak": return "sessile oak";
            case "beech": return "beech";
            default: return "Unknown species";
        }
    }

    // Display only: keep objective IDs, recorded labels and achievement values intact.
    public static string ObjectiveName(ScenarioObjectiveResult objective)
        => objective.displayName.Replace("sitka-spruce", SpeciesName("sitka-spruce"))
            .Replace("sessile-oak", SpeciesName("sessile-oak"));

    public static string ObjectiveLine(ScenarioObjectiveResult objective)
        => $"{(objective.achieved ? "✓ done" : "○ open")} · {ObjectiveName(objective)}: {objective.currentValue.ToString("0.##", UiKit.Inv)} / {objective.targetValue.ToString("0.##", UiKit.Inv)}";

    private static string Capitalise(string text) => string.IsNullOrEmpty(text) ? text : char.ToUpperInvariant(text[0]) + text.Substring(1);
}
