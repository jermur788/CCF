using System;
using System.Collections.Generic;
using UnityEngine;

public enum ScenarioExecutionMode
{
    ManagementOnly
}

[Serializable]
public sealed class ScenarioShopEntry
{
    public string itemId = "";
    public string speciesId = "";
    public string displayName = "";
    [Min(0)] public int unitPriceCents;
    [Min(0)] public int plantingMinutes;
}

[CreateAssetMenu(fileName = "ScenarioOne", menuName = "Forest Prototype/Scenario One Definition")]
public sealed class ScenarioOneDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string scenarioId = "scenario-one";
    [SerializeField] private string displayName = "Scenario One — Sitka Plantation to Continuous-Cover Forest";
    [SerializeField] private ScenarioExecutionMode executionMode = ScenarioExecutionMode.ManagementOnly;

    [Header("Economy — provisional gameplay calibration [D]")]
    [SerializeField, Min(0)] private long startingCashCents = 1200000;
    [SerializeField, Min(0)] private int contractorHourlyRateCents = 4500;
    [SerializeField, Min(0)] private int fellingBaseMinutes = 12;
    [SerializeField, Min(0f)] private float fellingMinutesPerCubicMetre = 10f;
    [SerializeField, Min(0)] private int sitkaTimberValueCentsPerCubicMetre = 7200;
    [SerializeField, Min(0)] private int broadleafTimberValueCentsPerCubicMetre = 6500;

    [Header("Planting stock — provisional gameplay calibration [D]")]
    [SerializeField] private List<ScenarioShopEntry> shopEntries = new List<ScenarioShopEntry>
    {
        new ScenarioShopEntry { itemId = "beech-sapling", speciesId = "beech", displayName = "European beech sapling", unitPriceCents = 450, plantingMinutes = 10 },
        new ScenarioShopEntry { itemId = "sessile-oak-sapling", speciesId = "sessile-oak", displayName = "Sessile oak sapling", unitPriceCents = 550, plantingMinutes = 10 }
    };

    [Header("Regeneration control — provisional gameplay calibration [D]")]
    [SerializeField, Min(0)] private int removalBaseMinutes = 6;
    [SerializeField, Min(0f)] private float removalMinutesPerCohortDensity = 3f;

    [Header("Understorey — provisional functional-group calibration [D]")]
    [SerializeField, Range(0f, 1f)] private float understoreyColonisationRate = 0.3f;
    [SerializeField, Range(0f, 1f)] private float understoreyLossRate = 0.45f;

    [Header("Deadwood and felling outcome — provisional gameplay calibration [D]")]
    [SerializeField] private FellingMaterialOutcome defaultFellingOutcome = FellingMaterialOutcome.SellAndExtract;
    [SerializeField, Min(0f)] private float deadwoodHabitatWeight = 1f;
    [SerializeField, Min(0f)] private float understoreyHabitatWeight = 1f;
    [SerializeField, Min(0f)] private float canopyDiversityHabitatWeight = 1f;

    [Header("Pruning — provisional gameplay calibration [D]")]
    [Tooltip("Target clear-stem heights (metres) for successive pruning lifts, following common Sitka clear-stem practice.")]
    [SerializeField] private float[] pruningLiftTargetHeightsM = { 2.5f, 5f, 6.5f };
    [SerializeField, Min(0)] private int pruningBaseMinutes = 8;
    [SerializeField, Min(0f)] private float pruningMinutesPerMetre = 2f;

    [Header("Progression — provisional calibration [D]")]
    [SerializeField, Min(1)] private int minimumCompletionYear = 25;

    public string ScenarioId => scenarioId;
    public string DisplayName => displayName;
    public ScenarioExecutionMode ExecutionMode => executionMode;
    public long StartingCashCents => startingCashCents;
    public int ContractorHourlyRateCents => contractorHourlyRateCents;
    public int FellingBaseMinutes => fellingBaseMinutes;
    public float FellingMinutesPerCubicMetre => fellingMinutesPerCubicMetre;
    public int MinimumCompletionYear => minimumCompletionYear;
    public IReadOnlyList<ScenarioShopEntry> ShopEntries => shopEntries;
    public int RemovalBaseMinutes => removalBaseMinutes;
    public float RemovalMinutesPerCohortDensity => removalMinutesPerCohortDensity;
    public float UnderstoreyColonisationRate => understoreyColonisationRate;
    public float UnderstoreyLossRate => understoreyLossRate;
    public FellingMaterialOutcome DefaultFellingOutcome => defaultFellingOutcome;
    public float DeadwoodHabitatWeight => deadwoodHabitatWeight;
    public float UnderstoreyHabitatWeight => understoreyHabitatWeight;
    public float CanopyDiversityHabitatWeight => canopyDiversityHabitatWeight;
    public IReadOnlyList<float> PruningLiftTargetHeightsM => pruningLiftTargetHeightsM;
    public int PruningBaseMinutes => pruningBaseMinutes;
    public float PruningMinutesPerMetre => pruningMinutesPerMetre;

    // Target crown-base height for the next lift on a tree, or -1 if no lift is
    // currently valid (tree already at the configured maximum).
    public float NextPruningTargetHeightM(int currentLifts)
    {
        if (pruningLiftTargetHeightsM == null || currentLifts < 0 || currentLifts >= pruningLiftTargetHeightsM.Length)
            return -1f;
        return pruningLiftTargetHeightsM[currentLifts];
    }

    public ScenarioShopEntry FindShopEntry(string itemId)
    {
        return shopEntries != null ? shopEntries.Find(entry => entry != null && entry.itemId == itemId) : null;
    }

    public int TimberValueCentsPerCubicMetre(string speciesId)
    {
        return speciesId == "sitka-spruce"
            ? sitkaTimberValueCentsPerCubicMetre
            : broadleafTimberValueCentsPerCubicMetre;
    }
}
