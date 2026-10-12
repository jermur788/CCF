// Stand-ins for three types that ForestSaveData.cs mentions but the derived-history logic never reads. They exist only
// so the real record classes (ScenarioManagementEvent, ScenarioAnnualReport, snapshots, deadwood, work orders) compile
// without dragging in the scenario definition, reference archive and ecology cell graph. The enum values mirror the
// production enum (ScenarioOneObjectives.cs) exactly.
public enum ScenarioOneOutcome { Active, Completed, Failed }
public sealed class ScenarioCenturyReview { public int year; }
public sealed class ScenarioUnderstoreyCell { public int cellIndex; }
// More save-schema neighbours that ForestSaveData.cs lists but the history logic never reads (browse protection,
// planting clearance and storm event records are separate persisted layers).
[System.Serializable] public sealed class BrowseProtectedArea { }
[System.Serializable] public sealed class BrowseShelter { }
[System.Serializable] public sealed class PlantingClearancePatch { }
[System.Serializable] public sealed class StormEventRecord { }
