using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ScenarioOneOutcome
{
    Active,
    Completed,
    Failed
}

[Serializable]
public sealed class ScenarioObjectiveResult
{
    public string objectiveId = "";
    public string displayName = "";
    public float currentValue;
    public float targetValue;
    public bool achieved;
}

[Serializable]
public sealed class ScenarioCenturyReview
{
    public int year;
    public ScenarioOneOutcome outcome;
    public int completedYear = -1;
    public List<ScenarioObjectiveResult> referenceComparisons = new List<ScenarioObjectiveResult>();
}

// Read-only evaluation of management outcomes. Thresholds and the provisional
// Year-100 reference are configured by the scenario definition, not the ecology.
public static class ScenarioOneObjectives
{
    public static List<ScenarioObjectiveResult> Evaluate(ScenarioOneDefinition definition,
        ScenarioEcologicalSnapshot snapshot, IReadOnlyList<ScenarioManagementEvent> events,
        string originalSpeciesId, IReadOnlyList<ScenarioOneWorkOrder> orders = null)
    {
        var results = new List<ScenarioObjectiveResult>();
        if (definition == null || snapshot == null)
            return results;

        Add(results, "minimum-year", "Reach the management review year", snapshot.year,
            definition.MinimumCompletionYear);
        ScenarioSpeciesOutcome original = Species(snapshot, originalSpeciesId);
        Add(results, "retained-canopy", "Retain original canopy trees", original != null ? original.livingTrees : 0,
            definition.MinimumRetainedOriginalTrees);
        Add(results, "continuous-canopy", "Keep continuous canopy", snapshot.meanCanopy,
            definition.MinimumMeanCanopy);
        Add(results, "regeneration", "Maintain regenerating cells", snapshot.occupiedRegenerationCells,
            definition.MinimumRegenerationCells);
        Add(results, "fallen-deadwood", "Retain fallen deadwood", snapshot.deadwoodVolumeM3,
            definition.MinimumDeadwoodVolumeM3);

        bool felled = events != null && events.Any(entry => entry != null
            && entry.eventType == ScenarioManagementEventType.WorkResolved
            && entry.outcome == ScenarioManagementOutcome.Succeeded
            && entry.taskType == ScenarioWorkType.FellTree);
        felled |= orders != null && orders.Any(order => order != null
            && order.type == ScenarioWorkType.FellTree && order.status == ScenarioWorkStatus.Completed);
        Add(results, "managed-opening", "Carry out contractor felling", felled ? 1 : 0, 1);
        if (definition.ShopEntries != null)
            foreach (string speciesId in definition.ShopEntries.Where(item => item != null
                && !string.IsNullOrEmpty(item.speciesId))
                     .Select(item => item.speciesId).Distinct(StringComparer.Ordinal))
            {
                ScenarioSpeciesOutcome species = Species(snapshot, speciesId);
                bool planted = events != null && events.Any(entry => entry != null
                    && entry.eventType == ScenarioManagementEventType.WorkResolved
                    && entry.outcome == ScenarioManagementOutcome.Succeeded
                    && entry.taskType == ScenarioWorkType.PlantJuvenile
                    && entry.speciesId == speciesId);
                planted |= orders != null && orders.Any(order => order != null
                    && order.type == ScenarioWorkType.PlantJuvenile
                    && order.status == ScenarioWorkStatus.Completed && order.speciesId == speciesId);
                int present = species != null ? species.livingTrees + species.regenerationCells : 0;
                Add(results, "introduced-" + speciesId, "Establish planted " + speciesId,
                    planted && present > 0 ? 1 : 0, 1);
            }
        return results;
    }

    public static ScenarioCenturyReview Review(ScenarioOneDefinition definition,
        ScenarioEcologicalSnapshot snapshot, ScenarioOneOutcome outcome, int completedYear,
        string originalSpeciesId)
    {
        var review = new ScenarioCenturyReview
        {
            year = snapshot.year,
            outcome = outcome,
            completedYear = completedYear
        };
        ScenarioSpeciesOutcome original = Species(snapshot, originalSpeciesId);
        Add(review.referenceComparisons, "original-trees", "Original-species trees",
            original != null ? original.livingTrees : 0, definition.ReferenceOriginalTrees);
        if (definition.ShopEntries != null)
            foreach (string speciesId in definition.ShopEntries.Where(item => item != null
                && !string.IsNullOrEmpty(item.speciesId))
                     .Select(item => item.speciesId).Distinct(StringComparer.Ordinal))
            {
                ScenarioSpeciesOutcome species = Species(snapshot, speciesId);
                Add(review.referenceComparisons, "reference-" + speciesId, speciesId + " presence",
                    species != null ? species.livingTrees + species.regenerationCells : 0,
                    definition.ReferenceBroadleafPresence);
            }
        Add(review.referenceComparisons, "reference-regeneration", "Regenerating cells",
            snapshot.occupiedRegenerationCells, definition.ReferenceRegenerationCells);
        Add(review.referenceComparisons, "reference-deadwood", "Fallen deadwood m³",
            snapshot.deadwoodVolumeM3, definition.ReferenceDeadwoodVolumeM3);
        Add(review.referenceComparisons, "reference-canopy", "Mean canopy",
            snapshot.meanCanopy, definition.ReferenceMeanCanopy);
        return review;
    }

    private static ScenarioSpeciesOutcome Species(ScenarioEcologicalSnapshot snapshot, string speciesId)
    {
        return snapshot.species != null ? snapshot.species.FirstOrDefault(item =>
            item != null && string.Equals(item.speciesId, speciesId, StringComparison.Ordinal)) : null;
    }

    private static void Add(List<ScenarioObjectiveResult> results, string id, string label, float current, float target)
    {
        results.Add(new ScenarioObjectiveResult
        {
            objectiveId = id,
            displayName = label,
            currentValue = current,
            targetValue = target,
            achieved = current >= target
        });
    }
}
