// Stand-ins for types that ForestSaveData.cs mentions but the per-year history query never reads. They exist only so
// the real record classes (ScenarioManagementEvent, ScenarioAnnualReport, snapshots, deadwood, work orders) and the real
// StormModel.cs compile without dragging in the scenario definition, reference archive and ecology cell graph. The enum
// values mirror the production enum (ScenarioOneObjectives.cs) exactly. StormEventRecord is NOT stubbed here: the real
// StormModel.cs is compiled instead.
public enum ScenarioOneOutcome { Active, Completed, Failed }
public sealed class ScenarioCenturyReview { public int year; }
public sealed class ScenarioUnderstoreyCell { public int cellIndex; }
[System.Serializable] public sealed class BrowseProtectedArea { }
[System.Serializable] public sealed class BrowseShelter { }
[System.Serializable] public sealed class PlantingClearancePatch { }
