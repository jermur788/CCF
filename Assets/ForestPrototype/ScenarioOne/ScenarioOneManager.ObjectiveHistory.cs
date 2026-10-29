using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// S1-C T-05 objective/progression history (save v22). The persisted record is objectiveAchievements: one entry per
// objective, appended the first time EvaluateProgress sees it achieved. It is history only: it never changes whether an
// objective counts as achieved, the scenario outcome, or outcomeYear.
public sealed partial class ScenarioOneManager
{
    private List<ScenarioObjectiveAchievement> objectiveAchievements = new List<ScenarioObjectiveAchievement>();

    // Objectives that were already achieved when this world was loaded but have no record. That is what a save from before
    // v22 looks like. Recording them now would stamp a later year on something achieved earlier, so they stay "not recorded".
    // Session state only (rebuilt on every load, deliberately not saved).
    private readonly HashSet<string> achievedBeforeRecording = new HashSet<string>();

    public IReadOnlyList<ScenarioObjectiveAchievement> ObjectiveAchievements => objectiveAchievements;

    // The year this objective was first observed achieved, or -1 when no record exists ("not recorded").
    public int ObjectiveAchievedYear(string objectiveId)
    {
        ScenarioObjectiveAchievement record = objectiveAchievements.FirstOrDefault(item => item.objectiveId == objectiveId);
        return record != null ? record.achievedYear : -1;
    }

    private void RecordObjectiveAchievements(IReadOnlyList<ScenarioObjectiveResult> objectives, int year)
    {
        foreach (ScenarioObjectiveResult result in objectives)
        {
            if (result == null || !result.achieved || achievedBeforeRecording.Contains(result.objectiveId)
                || objectiveAchievements.Any(item => item.objectiveId == result.objectiveId))
                continue;
            objectiveAchievements.Add(new ScenarioObjectiveAchievement
            {
                objectiveId = result.objectiveId,
                achievedYear = year,
                targetValueAtAchievement = result.targetValue
            });
        }
    }

    private void ClearObjectiveHistory()
    {
        objectiveAchievements.Clear();
        achievedBeforeRecording.Clear();
    }

    // Called at the end of a restore, once snapshots, events and orders are back. Evaluation only runs when the year
    // advances, so a save always matches its last evaluation: any achieved objective without a record is from before v22.
    private void RestoreObjectiveHistory(ScenarioOneSaveData data, int saveVersion)
    {
        achievedBeforeRecording.Clear();
        objectiveAchievements = saveVersion >= 22 ? CloneObjectiveAchievements(data.objectiveAchievements)
            : new List<ScenarioObjectiveAchievement>();
        if (outcome != ScenarioOneOutcome.Active || ecologicalSnapshots.Count == 0 || definition == null)
            return;
        foreach (ScenarioObjectiveResult result in ScenarioOneObjectives.Evaluate(definition,
            ecologicalSnapshots[ecologicalSnapshots.Count - 1], managementEvents, OriginalSpeciesId, workOrders,
            ecology.StandGeometryModelVersion))
            if (result.achieved && objectiveAchievements.All(item => item.objectiveId != result.objectiveId))
                achievedBeforeRecording.Add(result.objectiveId);
    }

    private static List<ScenarioObjectiveAchievement> CloneObjectiveAchievements(List<ScenarioObjectiveAchievement> source)
    {
        if (source == null) return new List<ScenarioObjectiveAchievement>();
        return source.Select(item => JsonUtility.FromJson<ScenarioObjectiveAchievement>(JsonUtility.ToJson(item))).ToList();
    }
}
