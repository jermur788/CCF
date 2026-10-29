using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// S1-C T-05 objective/progression history (save v22). The persisted record is objectiveAchievements: one entry per
// objective, appended the first time EvaluateProgress sees it achieved. It is history only: it never changes whether an
// objective counts as achieved, the scenario outcome, or outcomeYear.
public enum ObjectiveYearSource { NotRecorded, Recorded, Derived }

public sealed partial class ScenarioOneManager
{
    private List<ScenarioObjectiveAchievement> objectiveAchievements = new List<ScenarioObjectiveAchievement>();

    // Objectives that an earlier, already-evaluated year of this world achieved without leaving a record. That is what a
    // save from before v22 looks like. Recording them now would stamp a later year on something achieved earlier (or
    // re-achieved), so they are never recorded. Session state only, rebuilt on every load from the saved yearly records.
    private readonly HashSet<string> achievedBeforeRecording = new HashSet<string>();

    // Display-only (L3, labelled derived): the exact first year an unrecorded objective was achieved, recomputed from the
    // year-tagged snapshots, events and resolved orders. Never written to the save.
    private readonly Dictionary<string, int> derivedAchievementYears = new Dictionary<string, int>();

    public IReadOnlyList<ScenarioObjectiveAchievement> ObjectiveAchievements => objectiveAchievements;

    // The year this objective was first observed achieved, or -1 when no record exists ("not recorded").
    public int ObjectiveAchievedYear(string objectiveId)
    {
        ScenarioObjectiveAchievement record = objectiveAchievements.FirstOrDefault(item => item.objectiveId == objectiveId);
        return record != null ? record.achievedYear : -1;
    }

    // Recorded year if there is a record; otherwise the exactly derived year (older saves); otherwise -1 = not recorded.
    public int ObjectiveYear(string objectiveId, out ObjectiveYearSource source)
    {
        int recorded = ObjectiveAchievedYear(objectiveId);
        if (recorded >= 0) { source = ObjectiveYearSource.Recorded; return recorded; }
        if (derivedAchievementYears.TryGetValue(objectiveId, out int derived)) { source = ObjectiveYearSource.Derived; return derived; }
        source = ObjectiveYearSource.NotRecorded;
        return -1;
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
        derivedAchievementYears.Clear();
    }

    // Called at the end of a restore, once snapshots, events and orders are back. Replays the evaluations that already
    // happened: one per year that has a YearAdvanced event (the year-0 baseline snapshot was never evaluated, so a new game
    // keeps everything recordable). An objective achieved in such a year with no record predates recording: it is never
    // recorded later, and gets a derived year when the evidence is complete (every earlier evaluated year has its snapshot).
    private void RestoreObjectiveHistory(ScenarioOneSaveData data, int saveVersion)
    {
        achievedBeforeRecording.Clear();
        derivedAchievementYears.Clear();
        objectiveAchievements = saveVersion >= 22 ? CloneObjectiveAchievements(data.objectiveAchievements)
            : new List<ScenarioObjectiveAchievement>();
        if (definition == null)
            return;
        // Evaluation stops once the outcome is final, so later snapshots were never evaluated either.
        int lastEvaluatedYear = outcome == ScenarioOneOutcome.Active ? int.MaxValue : outcomeYear;
        bool evidenceGap = false;
        foreach (int year in managementEvents.Where(e => e != null && e.eventType == ScenarioManagementEventType.YearAdvanced
            && e.year <= lastEvaluatedYear).Select(e => e.year).Distinct().OrderBy(y => y))
        {
            ScenarioEcologicalSnapshot snapshot = ecologicalSnapshots.FirstOrDefault(item => item.year == year);
            if (snapshot == null) { evidenceGap = true; continue; }
            // The records as they stood at that year's evaluation.
            List<ScenarioManagementEvent> events = managementEvents.Where(e => e != null && e.year <= year).ToList();
            List<ScenarioOneWorkOrder> orders = workOrders.Where(o => o != null
                && (o.status != ScenarioWorkStatus.Completed || o.resolvedYear <= year)).ToList();
            foreach (ScenarioObjectiveResult result in ScenarioOneObjectives.Evaluate(definition, snapshot, events,
                OriginalSpeciesId, orders, ecology.StandGeometryModelVersion))
            {
                if (!result.achieved || achievedBeforeRecording.Contains(result.objectiveId)
                    || objectiveAchievements.Any(item => item.objectiveId == result.objectiveId))
                    continue;
                achievedBeforeRecording.Add(result.objectiveId);
                if (!evidenceGap) derivedAchievementYears[result.objectiveId] = year;
            }
        }
    }

    private static List<ScenarioObjectiveAchievement> CloneObjectiveAchievements(List<ScenarioObjectiveAchievement> source)
    {
        if (source == null) return new List<ScenarioObjectiveAchievement>();
        return source.Select(item => JsonUtility.FromJson<ScenarioObjectiveAchievement>(JsonUtility.ToJson(item))).ToList();
    }
}
