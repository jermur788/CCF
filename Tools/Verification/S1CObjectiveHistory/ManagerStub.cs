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
    public static Func<List<ScenarioObjectiveResult>> Hook;
    public static List<ScenarioObjectiveResult> Evaluate(ScenarioOneDefinition definition, ScenarioEcologicalSnapshot snapshot,
        IReadOnlyList<ScenarioManagementEvent> events, string originalSpeciesId, IReadOnlyList<ScenarioOneWorkOrder> orders, int geometry)
        => Hook();
}

public sealed partial class ScenarioOneManager
{
    private ScenarioOneDefinition definition = new ScenarioOneDefinition();
    private StubEcology ecology = new StubEcology();
    private List<ScenarioEcologicalSnapshot> ecologicalSnapshots = new List<ScenarioEcologicalSnapshot>();
    private List<ScenarioManagementEvent> managementEvents = new List<ScenarioManagementEvent>();
    private List<ScenarioOneWorkOrder> workOrders = new List<ScenarioOneWorkOrder>();
    private ScenarioOneOutcome outcome = ScenarioOneOutcome.Active;
    private string OriginalSpeciesId => "sitka";

    // Mirrors the Active branch of the production EvaluateProgress up to the record call (the runner audits that wiring).
    public void EvaluateActive(int year)
    {
        ecologicalSnapshots.Add(new ScenarioEcologicalSnapshot { year = year });
        if (outcome != ScenarioOneOutcome.Active) return;
        List<ScenarioObjectiveResult> objectives = ScenarioOneObjectives.Evaluate(definition, ecologicalSnapshots.Last(), managementEvents,
            OriginalSpeciesId, workOrders, ecology.StandGeometryModelVersion);
        RecordObjectiveAchievements(objectives, ecologicalSnapshots.Last().year);
    }

    // Mirrors CaptureSaveData / RestoreSaveData for the history field plus the real migration call.
    public ScenarioOneSaveData Capture() => new ScenarioOneSaveData { objectiveAchievements = CloneObjectiveAchievements(objectiveAchievements) };
    public void Restore(ScenarioOneSaveData data, int saveVersion, int snapshotYear)
    {
        ScenarioOneSaveMigration.NormalizeObjectiveHistory(data, saveVersion);
        ecologicalSnapshots = new List<ScenarioEcologicalSnapshot> { new ScenarioEcologicalSnapshot { year = snapshotYear } };
        RestoreObjectiveHistory(data, saveVersion);
    }
    public void Reset() { ClearObjectiveHistory(); ecologicalSnapshots.Clear(); outcome = ScenarioOneOutcome.Active; }
    public void SetOutcome(ScenarioOneOutcome value) { outcome = value; }
}
