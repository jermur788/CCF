using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

// Scenario 1 annual-review ecology lines (Docs/Scenario1EcologyCompletion.md).
// Pure formatter over existing snapshots and authoritative ecology state; it
// changes nothing and adds no rule. The caller (ScenarioOneManager annual
// review) owns when and where the lines are shown.
public static class ScenarioEcologyReviewLines
{
    // A shelter "nears expiry" when it protects this many upcoming steps or fewer.
    public const int ShelterExpiryWarningSteps = 2;

    public static List<string> Lines(ScenarioEcologicalSnapshot previous, ScenarioEcologicalSnapshot current,
        ForestEcologyController ecology, IReadOnlyList<PlantedJuvenile> planted, int maxLines = 6)
    {
        var lines = new List<string>();
        if (current == null)
            return lines;

        // 1. Canopy and stand light.
        lines.Add(previous != null
            ? $"Canopy {F2(previous.meanCanopy)} → {F2(current.meanCanopy)} ({Signed(current.meanCanopy - previous.meanCanopy)}); "
              + $"mean light {F2(previous.meanLight)} → {F2(current.meanLight)}."
            : $"Canopy {F2(current.meanCanopy)}; mean light {F2(current.meanLight)}.");

        // 2. Managed openings: cells still carrying a recent opening.
        if (ecology != null && ecology.Cells != null)
        {
            ForestEcologyCell[] opened = ecology.Cells.Where(cell => cell.RecentOpening > 0.05f).ToArray();
            if (opened.Length > 0)
                lines.Add($"Opened cells: {opened.Length} with mean light {F2(opened.Average(cell => cell.Light))} "
                    + $"(stand {F2(current.meanLight)}).");
        }

        // 3. Regeneration by species, natural versus planted.
        var bySpecies = current.species.Where(s => s != null && (s.regenerationCells > 0 || s.plantedJuveniles > 0))
            .OrderBy(s => s.speciesId, System.StringComparer.Ordinal)
            .Select(s => $"{s.speciesId} {s.regenerationCells}"
                + (s.plantedRegenerationCells > 0 ? $" ({s.plantedRegenerationCells} planted)" : "")
                + (s.plantedJuveniles > 0 ? $" +{s.plantedJuveniles} planted juveniles" : ""))
            .ToList();
        string change = previous != null ? $" (was {previous.occupiedRegenerationCells})" : "";
        lines.Add(bySpecies.Count > 0
            ? $"Regenerating cells: {current.occupiedRegenerationCells}{change} — {string.Join(", ", bySpecies)}."
            : $"Regenerating cells: {current.occupiedRegenerationCells}{change}.");

        // 4. Planted individuals.
        List<PlantedJuvenile> individuals = planted?.Where(j => j != null && !j.legacyCohortManaged).ToList()
            ?? new List<PlantedJuvenile>();
        if (individuals.Count > 0)
        {
            int promoted = individuals.Count(j => !string.IsNullOrEmpty(j.promotedTreeId));
            int growing = individuals.Count(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId));
            int lost = individuals.Count(j => !j.alive);
            lines.Add($"Planted trees: {growing} growing, {promoted} joined the canopy layer, {lost} lost.");
        }

        if (ecology == null)
            return Trim(lines, maxLines);

        // 5. Browsing: band, plus last year's browsing where this session recorded it.
        float pressure = ecology.Browsing.BackgroundPressure;
        if (pressure > 0f)
        {
            string band = BrowsingConditions.PressureBand(pressure);
            int year = ecology.EcologicalYear;
            List<PlantedJuvenile> recorded = individuals.Where(j => j.lastBrowseAssessmentYear == year && year > 0).ToList();
            List<ForestRegenerationCohort> cohorts = ecology.Cells.SelectMany(cell => cell.Regeneration)
                .Where(c => c != null && c.Density > 0f && c.LastBrowseAssessmentYear == year && year > 0).ToList();
            string recent = "";
            if (recorded.Count > 0)
                recent += $"; {recorded.Count(j => j.lastYearBrowsed)} of {recorded.Count} planted juveniles browsed last year";
            if (cohorts.Count > 0)
                recent += $"; ~{cohorts.Average(c => c.LastBrowsedFraction):P0} of natural leaders at risk";
            lines.Add($"Browsing pressure {band}{recent}.");
        }

        // 6. Protection.
        int upcoming = ecology.EcologicalYear + 1;
        int sheltered = 0, fenced = 0, expiring = 0;
        foreach (PlantedJuvenile j in individuals.Where(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId)))
        {
            BrowseProtectionState state = ecology.Browsing.ProtectionAt(new Vector2(j.position.x, j.position.z), upcoming, out _);
            if (state == BrowseProtectionState.EffectiveShelter)
            {
                sheltered++;
                int left = RegenerationDiagnostics.ShelterStepsRemaining(ecology.Browsing, j.position, upcoming);
                if (left >= 0 && left <= ShelterExpiryWarningSteps)
                    expiring++;
            }
            else if (state == BrowseProtectionState.InsideIntactFence)
                fenced++;
        }
        if (sheltered + fenced > 0 || ecology.Browsing.HasProtection)
            lines.Add($"Protected juveniles: {sheltered} sheltered, {fenced} fenced"
                + (expiring > 0 ? $"; {expiring} shelter(s) expire within {ShelterExpiryWarningSteps} years." : "."));
        return Trim(lines, maxLines);
    }

    private static List<string> Trim(List<string> lines, int maxLines)
        => lines.Count > maxLines ? lines.GetRange(0, maxLines) : lines;

    private static string F2(float value) => value.ToString("0.00", CultureInfo.InvariantCulture);

    private static string Signed(float value) => (value >= 0f ? "+" : "") + value.ToString("0.00", CultureInfo.InvariantCulture);
}
