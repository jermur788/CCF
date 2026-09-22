using System;
using System.Collections.Generic;

[Serializable]
public sealed class ForestSaveData
{
    public const int CurrentVersion = 9;

    public int version = CurrentVersion;
    // Carried wood; the field name stays "wood" so version-1 saves keep loading.
    public int wood;
    public int ecologicalYear;
    public int simulationSeed = 20260914;
    public List<string> markedTreeIds = new List<string>();
    public List<TreeSaveData> trees = new List<TreeSaveData>();
    public List<BuildableSaveData> buildables = new List<BuildableSaveData>();
    public List<WoodStorageSaveData> storages = new List<WoodStorageSaveData>();
    public List<ForestCellSaveData> cells = new List<ForestCellSaveData>();
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
    public UnityEngine.Vector3 position;
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
