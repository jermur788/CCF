using System;
using System.Collections.Generic;
using UnityEngine;

// Authored reference-management schedule for Scenario One Reference Future v1.
// The schedule is structured data, not ad-hoc editor manipulation: the same
// scenario version, simulation seed and schedule reproduce the same run.

public enum ReferenceActionType
{
    Survey,
    Thin,
    Plant,
    RemoveRegeneration,
    Prune,
    ReplaceFailedPlanting,
    LowIntervention
}

public enum ReferenceTargetingRule
{
    FutureTreeRelease,
    BrightestCells,
    PartialShadeCells,
    CompetingSitkaRegen,
    BestTimberStems,
    OverDominantSitka,
    FailedBroadleafPockets,
    BroadleafRelease
}

[Serializable]
public sealed class ScenarioReferenceDirective
{
    // Ecological year at which work orders are created; resolution occurs in
    // the transition to year+1 through the normal Scenario One annual step.
    public int year;
    public ReferenceActionType action;
    public ReferenceTargetingRule targeting;
    [Range(0f, 1f)] public float baFraction;
    public int count;
    [Range(0f, 1f)] public float deadwoodFraction;
    public string speciesId = "";
    public string note = "";
}

[Serializable]
public sealed class ScenarioReferenceSchedule
{
    public string scheduleId = "reference-future-v1";
    public string scenarioId = "scenario-one";
    public int simulationSeed = 20260914;
    public int futureTreeStride = 4;
    public List<ScenarioReferenceDirective> directives = new List<ScenarioReferenceDirective>();
}

// Deterministic survey result computed once from the starting stand. Future
// trees are retained structure/seed trees; candidate cells are planned
// broadleaf establishment areas.
public sealed class ScenarioReferenceSurvey
{
    public HashSet<string> futureTreeIds = new HashSet<string>(StringComparer.Ordinal);
    public List<int> oakCandidateCells = new List<int>();
    public List<int> beechCandidateCells = new List<int>();
    public List<int> oakPlantedCells = new List<int>();
    public List<int> beechPlantedCells = new List<int>();
    public int surveyedYear;
}
