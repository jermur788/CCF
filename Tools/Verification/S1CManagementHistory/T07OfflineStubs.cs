// Stand-ins for gameplay types that ForestSaveValidation.cs references but this offline S1-C T-07 gate does not
// exercise. Everything here mirrors the production field names and numeric/enum contract; the real save-record
// classes, the real StormModel.cs and the real per-year history reader are compiled unchanged alongside them.
// StormEventRecord, StormModel and StormCalibration are deliberately NOT stubbed here: they come from the real
// production StormModel.cs. Nothing here is used by the game.
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ScenarioOneOutcome { Active, Completed, Failed }
[Serializable] public sealed class ScenarioCenturyReview { public int year; }
[Serializable] public sealed class ScenarioUnderstoreyCell { public int cellIndex; public float brambleCover, brackenCover; public int lastUpdatedYear; }
[Serializable] public sealed class BrowseProtectedArea { public string areaId; public List<Vector2> polygon; public int installedYear; public int breachedYear = -1; }
[Serializable] public sealed class BrowseShelter { public string shelterId; public Vector2 position; public int installedYear; public int effectiveYears = 8; public int failedYear = -1; }
[Serializable] public sealed class PlantingClearancePatch { public Vector3 center; public float radiusMeters = .564f; public int createdYear; public float brambleCover, brackenCover; public int competitionUpdatedYear; }
public enum ForestTreeStage { Mature, Stump, Sapling, Young }
public enum TreeMarkType { None = 0, Fell = 1, CropTree = 2 }
public enum RegenerationOrigin { Natural = 0, Planted = 1 }
public static class ForestEcologyCell { public const int MaxBandsPerSpeciesOrigin = 4; }
