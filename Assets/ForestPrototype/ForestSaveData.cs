using System;
using System.Collections.Generic;

[Serializable]
public sealed class ForestSaveData
{
    public const int CurrentVersion = 2;

    public int version = CurrentVersion;
    // Carried wood; the field name stays "wood" so version-1 saves keep loading.
    public int wood;
    public List<TreeSaveData> trees = new List<TreeSaveData>();
    public List<BuildableSaveData> buildables = new List<BuildableSaveData>();
    public List<WoodStorageSaveData> storages = new List<WoodStorageSaveData>();
}

[Serializable]
public sealed class WoodStorageSaveData
{
    public string storageId = "";
    public int storedWood;
}

[Serializable]
public sealed class TreeSaveData
{
    public string treeId = "";
    public int stage;
    public float stageTimer;
    public int chopProgress;
}

[Serializable]
public sealed class BuildableSaveData
{
    public string buildId = "";
    public bool built;
}
