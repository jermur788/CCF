using System;
using System.Collections.Generic;

[Serializable]
public sealed class ForestSaveData
{
    public const int CurrentVersion = 4;

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
    public int stage;
    public float stageTimer;
    public int chopProgress;
    // Version 3 simulation state; absent in legacy saves.
    public bool hasSimulation;
    public int ageYears;
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
    public float regenDensity;
    public float regenHeight;
    public int regenEstablishYear;
    public float recentOpening;
}
