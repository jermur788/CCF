using System;
using System.Collections.Generic;

[Serializable]
public sealed class ForestSaveData
{
    public const int CurrentVersion = 20;

    public int version = CurrentVersion;
    // Carried wood; the field name stays "wood" so version-1 saves keep loading.
    public int wood;
    public int ecologicalYear;
    public int simulationSeed = 20260914;
    // RNG scheme (SimulationRandom). Deliberately defaults to 0, the legacy
    // scheme, so saves written before this field existed replay unchanged.
    public int rngModelVersion;
    // Version 16: regeneration representation (RegenerationModel). Defaults to
    // 0, legacy single-cohort regeneration, so older saves and Reference Future
    // v1 replay unchanged. Under model 1 cells may hold several cohort records
    // of one species: each is an age band keyed by species + origin + year.
    public int regenerationModel;
    // Version 17: adult growth model (GrowthModel). Defaults to 0, legacy
    // growth with no adult mortality, so older saves and Reference Future v1
    // replay unchanged. 1 = Class III Sitka height + adult density mortality.
    public int growthModel;
    // Version 19: independent storm mechanics. Missing legacy fields mean off.
    public int stormModel;
    // Version 20: the authoritative stand geometry (StandGeometryModel): 0 = Legacy40 (40 x 40 m, 64
    // cells), 1 = Enlarged80 (80 x 80 m, 256 cells). Explicit on every v20 save. A missing field and
    // every save before v20 mean Legacy40, so older saves and Reference Future v1 replay unchanged.
    // Every cell-indexed record below is only meaningful against this geometry's grid.
    public int standGeometryModel;
    public List<string> markedTreeIds = new List<string>();
    public List<string> cropTreeIds = new List<string>();
    public List<TreeSaveData> trees = new List<TreeSaveData>();
    public List<BuildableSaveData> buildables = new List<BuildableSaveData>();
    public List<WoodStorageSaveData> storages = new List<WoodStorageSaveData>();
    public List<ForestCellSaveData> cells = new List<ForestCellSaveData>();
    // Version 10: Scenario One management/economy state. Null in legacy saves.
    // Version 11: structured management events within scenarioOne.
    // Version 12: objective outcome, tutorial review and century comparison.
    // Version 13: persistent two-type tree marks, exact-position individual
    // plantings, treatment patches and retained construction timber.
    // Version 14: explicit biological tree mortality, separate from harvesting.
    // Version 15: scenario execution choices, protection and bounded economy reports.
    public ScenarioOneSaveData scenarioOne;
}

[Serializable]
public sealed class ScenarioOneSaveData
{
    public string scenarioId = "scenario-one";
    public bool initialized;
    public long cashCents;
    public int nextWorkOrderId = 1;
    public int nextManagementEventId = 1;
    public List<ScenarioOneWorkOrder> workOrders = new List<ScenarioOneWorkOrder>();
    public List<ScenarioInventoryEntry> inventory = new List<ScenarioInventoryEntry>();
    public List<ScenarioAnnualReport> annualReports = new List<ScenarioAnnualReport>();
    public List<ScenarioManagementEvent> managementEvents = new List<ScenarioManagementEvent>();
    public List<ScenarioEcologicalSnapshot> ecologicalSnapshots = new List<ScenarioEcologicalSnapshot>();
    public List<ScenarioUnderstoreyCell> understoreyCells = new List<ScenarioUnderstoreyCell>();
    public List<ScenarioDeadwoodRecord> deadwoodRecords = new List<ScenarioDeadwoodRecord>();
    public int nextDeadwoodId = 1;
    public ScenarioOneOutcome outcome;
    public int outcomeYear = -1;
    public string outcomeReason = "";
    public bool annualReviewSeen;
    public ScenarioCenturyReview centuryReview;
    public float retainedTimberM3;
    public List<PlantedJuvenileSaveData> plantedJuveniles = new List<PlantedJuvenileSaveData>();
    public List<PlantingClearancePatch> clearancePatches = new List<PlantingClearancePatch>();
    // Distinguishes the early v13 draft (which duplicated cohorts with inert
    // individual records) from authoritative individual planting v13.
    public int interactionSchemaVersion;
    public List<BrowseShelter> shelters = new List<BrowseShelter>();
    public List<BrowseProtectedArea> protectedAreas = new List<BrowseProtectedArea>();
    public int ownerMinutesUsedThisYear;
    public List<StormEventRecord> stormEvents = new List<StormEventRecord>();
}

[Serializable]
public sealed class ScenarioInventoryEntry
{
    public string itemId = "";
    public int quantity;
}

[Serializable]
public sealed class ScenarioAnnualReport
{
    public int year;
    public int completedTasks;
    public int failedTasks;
    public long contractorCostCents;
    public long timberRevenueCents;
    public float harvestedVolumeM3;
    public int regenerationRemovalTasks;
    public float removedRegenerationDensity;
    public int deadwoodCreated;
    public float deadwoodCreatedM3;
    public float deadwoodDecayedM3;
    public float keptForUseVolumeM3;
    public long closingCashCents;
    public long harvestMinimumAdjustmentCents;
    public int ownerMinutes;
    public List<ScenarioTimberSale> timberSales = new List<ScenarioTimberSale>();
}

[Serializable]
public sealed class ScenarioTimberSale
{
    public CCF.Forestry.WorkEconomy.TimberAssortment assortment;
    public long soldVolumeCm3;
    public long revenueCents;
}

[Serializable]
public sealed class TreeSaveData
{
    public string treeId = "";
    // Version 7: explicit individual species identity. Legacy saves omit it
    // and are loaded as the scene's default Sitka species.
    public string speciesId = "";
    public int stage;
    public float stageTimer;
    public int chopProgress;
    // Version 3 simulation state; absent in legacy saves.
    public bool hasSimulation;
    public int ageYears;
    // Version 6: recorded history; older saves start recording at zero.
    public float equivalentSuppressedYears;
    public float heightMeters;
    public float diameterCm;
    public float crownRadiusMeters;
    // Version 11: pruning history. Older saves load as unpruned.
    public int pruningLifts;
    public float crownBaseHeightM;
    public int lastPruningYear = -1;
    public int markType;
    // Older schemas omit these fields and restore no biological mortality.
    public bool biologicallyDead;
    public string mortalityCause = "";
    public int mortalityYear = -1;
    public UnityEngine.Vector3 position;
}

[Serializable]
public sealed class PlantedJuvenileSaveData
{
    public string juvenileId = "";
    public string speciesId = "";
    public UnityEngine.Vector3 position;
    public int cellIndex = -1;
    public int plantingYear;
    public float ageYears;
    public float heightMeters;
    public bool alive = true;
    public string stockItemId = "";
    public string promotedTreeId = "";
    public bool legacyCohortManaged;
}

[Serializable]
public sealed class BuildableSaveData
{
    public string buildId = "";
    public bool built;
}

[Serializable]
public sealed class WoodStorageSaveData
{
    public string storageId = "";
    public int storedWood;
}

[Serializable]
public sealed class ForestCellSaveData
{
    public int index;
    // Versions 1-7: one implicit default-Sitka cohort. Retained so JsonUtility
    // can deserialize legacy saves; version 8 writes cohorts instead.
    public float regenDensity;
    public float regenHeight;
    public int regenEstablishYear = -1;
    // Version 8+: explicit species-keyed regeneration state. Seed rain remains derived.
    public List<ForestRegenerationCohortSaveData> cohorts = new List<ForestRegenerationCohortSaveData>();
    public float recentOpening;
    // Smoothed disturbance response; a short history that cannot be rebuilt
    // from the other fields, so it is saved. Older saves carry -1 and the
    // loader reconstructs it deterministically from the opening.
    public float establishmentSuitability = -1f;
}

[Serializable]
public sealed class ForestRegenerationCohortSaveData
{
    public string speciesId = "";
    public float density;
    public float height;
    public int establishYear = -1;
    // Version 9: diagnostic provenance. Legacy cohorts default to Natural.
    public int origin;
    public int originYear = -1;
}
