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
    public string referenceId = "aspirational-design-targets";
    public string referenceScheduleHash = "";
    public List<ScenarioObjectiveResult> referenceComparisons = new List<ScenarioObjectiveResult>();
    public List<ScenarioReferenceManagementComparison> managementComparisons =
        new List<ScenarioReferenceManagementComparison>();
}

[Serializable]
public sealed class ScenarioReferenceManagementComparison
{
    public string comparisonId = "";
    public string displayName = "";
    public float playerValue;
    public float referenceValue;
}

// Read-only evaluation of management outcomes. When a verified reference is
// present, review comparisons use its actual saved world/history, not targets.
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
        Add(results, "managed-opening", "Carry out a commissioned thinning", felled ? 1 : 0, 1);
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
                int present = species != null ? species.livingTrees + species.regenerationCells
                    + species.plantedJuveniles : 0;
                Add(results, "introduced-" + speciesId, "Establish planted " + speciesId,
                    planted && present > 0 ? 1 : 0, 1);
            }
        return results;
    }

    public static ScenarioCenturyReview Review(ScenarioOneDefinition definition,
        ScenarioEcologicalSnapshot snapshot, ScenarioOneOutcome outcome, int completedYear,
        string originalSpeciesId, ScenarioReferenceArchive reference = null,
        IReadOnlyList<ScenarioManagementEvent> playerEvents = null,
        IReadOnlyList<ScenarioAnnualReport> playerReports = null,
        int playerLegacySitka = -1)
    {
        ForestSaveData referenceWorld = reference?.AtYear(100)?.world;
        ScenarioOneSaveData referenceScenario = referenceWorld?.scenarioOne;
        ScenarioEcologicalSnapshot referenceSnapshot = referenceScenario?.ecologicalSnapshots?
            .FirstOrDefault(item => item.year == 100);
        if (referenceSnapshot == null)
            reference = null;
        var review = new ScenarioCenturyReview
        {
            year = snapshot.year,
            outcome = outcome,
            completedYear = completedYear,
            referenceId = reference != null ? reference.referenceId : "aspirational-design-targets",
            referenceScheduleHash = reference != null ? reference.scheduleHash : ""
        };
        ScenarioSpeciesOutcome original = Species(snapshot, originalSpeciesId);
        ScenarioSpeciesOutcome referenceOriginal = reference != null ? Species(referenceSnapshot, originalSpeciesId) : null;
        Add(review.referenceComparisons, "original-trees", "Original-species trees",
            original != null ? original.livingTrees : 0,
            referenceOriginal != null ? referenceOriginal.livingTrees : definition.ReferenceOriginalTrees);
        if (definition.ShopEntries != null)
            foreach (string speciesId in definition.ShopEntries.Where(item => item != null
                && !string.IsNullOrEmpty(item.speciesId))
                     .Select(item => item.speciesId).Distinct(StringComparer.Ordinal))
            {
                ScenarioSpeciesOutcome species = Species(snapshot, speciesId);
                ScenarioSpeciesOutcome referenceSpecies = reference != null ? Species(referenceSnapshot, speciesId) : null;
                Add(review.referenceComparisons, "reference-" + speciesId, speciesId + " presence",
                    species != null ? species.livingTrees + species.regenerationCells + species.plantedJuveniles : 0,
                    referenceSpecies != null ? referenceSpecies.livingTrees + referenceSpecies.regenerationCells
                        : definition.ReferenceBroadleafPresence);
            }
        Add(review.referenceComparisons, "reference-regeneration", "Regenerating cells",
            snapshot.occupiedRegenerationCells,
            reference != null ? referenceSnapshot.occupiedRegenerationCells : definition.ReferenceRegenerationCells);
        Add(review.referenceComparisons, "reference-deadwood", "Fallen deadwood m³",
            snapshot.deadwoodVolumeM3,
            reference != null ? referenceSnapshot.deadwoodVolumeM3 : definition.ReferenceDeadwoodVolumeM3);
        Add(review.referenceComparisons, "reference-canopy", "Mean canopy",
            snapshot.meanCanopy,
            reference != null ? referenceSnapshot.meanCanopy : definition.ReferenceMeanCanopy);

        if (reference != null)
        {
            IReadOnlyList<ScenarioManagementEvent> referenceEvents = referenceScenario.managementEvents;
            AddManagement(review, "first-oak", "First successful Oak planting year",
                FirstPlantYear(playerEvents, "sessile-oak"), FirstPlantYear(referenceEvents, "sessile-oak"));
            AddManagement(review, "first-beech", "First successful Beech planting year",
                FirstPlantYear(playerEvents, "beech"), FirstPlantYear(referenceEvents, "beech"));
            AddManagement(review, "legacy-sitka", "Old plantation Sitka still standing",
                playerLegacySitka, referenceWorld.trees.Count(tree =>
                    tree.speciesId == originalSpeciesId && tree.treeId.StartsWith("P", StringComparison.Ordinal)
                    && tree.stage != (int)ForestTreeStage.Stump));
            AddManagement(review, "regen-control", "Sitka regeneration-control tasks",
                CountTreatments(playerEvents, ScenarioEcologicalTreatment.RegenerationRemoved),
                CountTreatments(referenceEvents, ScenarioEcologicalTreatment.RegenerationRemoved));
            AddManagement(review, "deadwood-retention", "Trees left as fallen deadwood",
                CountTreatments(playerEvents, ScenarioEcologicalTreatment.TreeRetainedAsDeadwood),
                CountTreatments(referenceEvents, ScenarioEcologicalTreatment.TreeRetainedAsDeadwood));
            AddManagement(review, "intervention-years", "Years with completed management work",
                playerReports != null ? playerReports.Count(report => report.completedTasks > 0) : 0,
                referenceScenario.annualReports.Count(report => report.completedTasks > 0));
        }
        return review;
    }

    private static float FirstPlantYear(IReadOnlyList<ScenarioManagementEvent> events, string speciesId)
    {
        if (events == null) return -1;
        var years = events.Where(entry => entry != null
            && entry.eventType == ScenarioManagementEventType.WorkResolved
            && entry.outcome == ScenarioManagementOutcome.Succeeded
            && entry.taskType == ScenarioWorkType.PlantJuvenile && entry.speciesId == speciesId)
            .Select(entry => entry.year).ToList();
        return years.Count > 0 ? years.Min() : -1;
    }

    private static int CountTreatments(IReadOnlyList<ScenarioManagementEvent> events,
        ScenarioEcologicalTreatment treatment)
    {
        return events != null ? events.Count(entry => entry != null
            && entry.eventType == ScenarioManagementEventType.WorkResolved
            && entry.outcome == ScenarioManagementOutcome.Succeeded
            && entry.ecologicalTreatment == treatment) : 0;
    }

    private static void AddManagement(ScenarioCenturyReview review, string id, string display,
        float playerValue, float referenceValue)
    {
        review.managementComparisons.Add(new ScenarioReferenceManagementComparison
        {
            comparisonId = id,
            displayName = display,
            playerValue = playerValue,
            referenceValue = referenceValue
        });
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
