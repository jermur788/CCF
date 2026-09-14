using System;
using System.Collections.Generic;

[Serializable]
public sealed class ForestSaveData
{
    public int wood;
    public List<TreeSaveData> trees = new List<TreeSaveData>();
    public List<BuildableSaveData> buildables = new List<BuildableSaveData>();
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
