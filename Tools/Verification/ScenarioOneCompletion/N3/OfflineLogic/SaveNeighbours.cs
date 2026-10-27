// Offline save neighbours only. No ecology/visual world adapter runs in this test.
using System;
using System.Collections.Generic;
using UnityEngine;
public enum ScenarioOneOutcome { Active, Completed, Failed }
[Serializable] public sealed class ScenarioCenturyReview { public int year; }
[Serializable] public sealed class ScenarioUnderstoreyCell { public int cellIndex; public float brambleCover,brackenCover; public int lastUpdatedYear; }
[Serializable] public sealed class BrowseProtectedArea { public string areaId; public List<Vector2> polygon; public int installedYear; public int breachedYear=-1; }
[Serializable] public sealed class BrowseShelter { public string shelterId; public Vector2 position; public int installedYear; public int effectiveYears=8; public int failedYear=-1; }
[Serializable] public sealed class PlantingClearancePatch { public Vector3 center; public float radiusMeters=.564f; public int createdYear; public float brambleCover,brackenCover; public int competitionUpdatedYear; }
[Serializable] public sealed class StormEventRecord { public int year; public float severity,directionDegrees; public int cropTreesLost; }
public enum ForestTreeStage { Mature, Stump, Sapling, Young }
public enum TreeMarkType { None=0, Fell=1, CropTree=2 }   // mirrors ForestTree.cs
public enum RegenerationOrigin { Natural=0, Planted=1 }
public static class ForestEcologyCell { public const int MaxBandsPerSpeciesOrigin=4; }
public static class StormModel { public const int None=0,Latest=1; }
public static class StormCalibration { public static bool Finite(float v) => !float.IsNaN(v)&&!float.IsInfinity(v); }
