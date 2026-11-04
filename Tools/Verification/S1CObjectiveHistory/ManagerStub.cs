// Lets the REAL ScenarioOneManager.ObjectiveHistory.cs compile and run offline. Only the members that file reads are
// stubbed; ScenarioOneObjectives.Evaluate is replaced by a hook so each test chooses which objectives are achieved.
// ScenarioObjectiveResult is extracted from production by the runner (ObjectiveResultExtract.cs), not re-typed here.
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class ScenarioOneDefinition { }
public sealed class StubEcology { public int StandGeometryModelVersion; }

public static class ScenarioOneObjectives
{
    public static Func<ScenarioEcologicalSnapshot, List<ScenarioObjectiveResult>> Hook;
    public static List<ScenarioObjectiveResult> Evaluate(ScenarioOneDefinition definition, ScenarioEcologicalSnapshot snapshot,
        IReadOnlyList<ScenarioManagementEvent> events, string originalSpeciesId, IReadOnlyList<ScenarioOneWorkOrder> orders, int geometry)
        => Hook(snapshot);
}

public sealed partial class ScenarioOneManager
{
    private ScenarioOneDefinition definition = new ScenarioOneDefinition();
    private StubEcology ecology = new StubEcology();
    private List<ScenarioEcologicalSnapshot> ecologicalSnapshots = new List<ScenarioEcologicalSnapshot>();
    private List<ScenarioManagementEvent> managementEvents = new List<ScenarioManagementEvent>();
    private List<ScenarioOneWorkOrder> workOrders = new List<ScenarioOneWorkOrder>();
    private ScenarioOneOutcome outcome = ScenarioOneOutcome.Active;
    private int outcomeYear = -1;
    private string OriginalSpeciesId => "sitka";

    // Mirrors AdvanceYear + EvaluateProgress: the year's snapshot and YearAdvanced event are recorded, then (Active only)
    // the objectives are evaluated and recorded. The year-0 baseline (AddBaseline) is a snapshot that is never evaluated.
    public void AddEvent(int year, ScenarioManagementEventType type) { managementEvents.Add(new ScenarioManagementEvent { eventId = managementEvents.Count + 1, year = year, eventType = type }); }
    public void AddBaseline() { ecologicalSnapshots.Add(new ScenarioEcologicalSnapshot { year = 0 }); }
    public void EvaluateActive(int year)
    {
        ecologicalSnapshots.Add(new ScenarioEcologicalSnapshot { year = year });
        managementEvents.Add(new ScenarioManagementEvent { eventId = managementEvents.Count + 1, year = year, eventType = ScenarioManagementEventType.YearAdvanced });
        if (outcome != ScenarioOneOutcome.Active) return;
        List<ScenarioObjectiveResult> objectives = ScenarioOneObjectives.Evaluate(definition, ecologicalSnapshots.Last(), managementEvents,
            OriginalSpeciesId, workOrders, ecology.StandGeometryModelVersion);
        RecordObjectiveAchievements(objectives, ecologicalSnapshots.Last().year);
    }
    // Test hook: drop one year's snapshot, as a damaged save would.
    // Test hook: drop one year's YearAdvanced event, as a damaged save would.
    public void DropYearEvent(int year) { managementEvents.RemoveAll(item => item.year == year && item.eventType == ScenarioManagementEventType.YearAdvanced); }
    public void DropSnapshot(int year) { ecologicalSnapshots.RemoveAll(item => item.year == year); }

    // Mirrors CaptureSaveData / RestoreSaveData for the history field plus the real migration call. The yearly records
    // (snapshots, events) are copied from the saving session, as a real save carries them.
    public ScenarioOneSaveData Capture() => new ScenarioOneSaveData { objectiveAchievements = CloneObjectiveAchievements(objectiveAchievements) };
    public void CopyYearlyRecordsFrom(ScenarioOneManager other)
    {
        ecologicalSnapshots = other.ecologicalSnapshots.ToList();
        managementEvents = other.managementEvents.ToList();
        workOrders = other.workOrders.ToList();
        outcome = other.outcome;
        outcomeYear = other.outcomeYear;
    }
    public void Restore(ScenarioOneSaveData data, int saveVersion)
    {
        ScenarioOneSaveMigration.NormalizeObjectiveHistory(data, saveVersion);
        RestoreObjectiveHistory(data, saveVersion);
    }
    public void Reset() { ClearObjectiveHistory(); ecologicalSnapshots.Clear(); managementEvents.Clear(); outcome = ScenarioOneOutcome.Active; outcomeYear = -1; }
    public void SetOutcome(ScenarioOneOutcome value)
    {
        outcome = value;
        outcomeYear = value == ScenarioOneOutcome.Active ? -1 : ecologicalSnapshots.Last().year;
    }
}
