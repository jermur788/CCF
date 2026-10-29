using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Player-facing wording built from authoritative diagnoses. Browsing is shown
// qualitatively: the site pressure band plus the stem's protection state. No
// per-leader probability is displayed (not an accepted player-facing number).
public static class ScenarioOneUiFacts
{
    // PARITY-07 / FR-9 wording shared by inspection, Work Plan and teaching, so the model limits are stated once.
    public const string BroadleafQualityLimit = "Oak and Beech grade here depends on size and pruning history in this version; fork and defect shape is not modelled.";
    public const string PremiumOakNote = "Premium Oak is only a possible late outcome: it needs a large, pruned, favourably grown tree with little competition, and it is not promised or required.";
    public const string SharedSpeciesRulesNote = "Site suitability, canopy shading and wind exposure use the same simplified rules for all three species; they are not calibrated per species.";

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

    // Display only: keep objective IDs, recorded labels and achievement values intact. The recorded label of the two regeneration
    // objectives uses the implementation word "cell"; the player reads plain forestry wording chosen by the objective's id.
    // The objective counts the 5 m × 5 m map squares that hold any young trees at all (seedlings and planted saplings included);
    // it does not measure that the young trees are established or successful, so the wording says only that they are present.
    public const string RegenerationObjectiveName = "Areas with young trees";
    public const string RegenerationAreasName = "Areas with young regeneration";

    public static string ObjectiveName(ScenarioObjectiveResult objective)
    {
        if (objective.objectiveId == "regeneration") return RegenerationObjectiveName;
        if (objective.objectiveId == "reference-regeneration") return RegenerationAreasName;
        return objective.displayName.Replace("sitka-spruce", SpeciesName("sitka-spruce"))
            .Replace("sessile-oak", SpeciesName("sessile-oak"));
    }

    // Player-facing wording for text that is recorded with the word "cell" (validation and failure reasons are stored in the
    // save, so they are translated when shown instead of being rewritten). One map square is a 5 m × 5 m cell of the ecology grid.
    public static string PlainText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        text = System.Text.RegularExpressions.Regex.Replace(text, @"\bcells\b", "map squares");
        return System.Text.RegularExpressions.Regex.Replace(text, @"\bcell\b", "map square");
    }

    // The map label of a cell index (A1 … P16), or "#N" when no ecology is available to give the grid width.
    public static string SquareLabel(int cellIndex)
    {
        ForestEcologyController ecology = Object.FindFirstObjectByType<ForestEcologyController>();
        return ecology != null && ecology.CellsPerAxis > 0 ? UiKit.CellLabel(cellIndex, ecology.CellsPerAxis) : "#" + cellIndex;
    }

    public static string CenturyComparisonDescription(ScenarioCenturyReview review)
    {
        return review.referenceId == "aspirational-design-targets"
            ? "Compared with Scenario One aspirational design targets: not a forecast, optimum or prescription."
            : "Compared with the frozen Reference Future, not an optimal score or prescription.";
    }

    public static string CenturyComparisonValueLabel(ScenarioCenturyReview review) =>
        review.referenceId == "aspirational-design-targets" ? "target" : "reference";

    public static string ObjectiveLine(ScenarioObjectiveResult objective)
    {
        string status = objective.achieved ? "✓ done" : "○ open";
        string current = objective.currentValue.ToString("0.##", UiKit.Inv), target = objective.targetValue.ToString("0.##", UiKit.Inv);
        // The exact figure is the number of 5 m × 5 m map squares holding young trees; the line says what it counts.
        if (objective.objectiveId == "regeneration")
            return $"{status} · Young trees present in {current} {(current == "1" ? "area" : "areas")} (at least {target} needed)";
        return $"{status} · {ObjectiveName(objective)}: {current} / {target}";
    }

    private static string Capitalise(string text) => string.IsNullOrEmpty(text) ? text : char.ToUpperInvariant(text[0]) + text.Substring(1);
}
