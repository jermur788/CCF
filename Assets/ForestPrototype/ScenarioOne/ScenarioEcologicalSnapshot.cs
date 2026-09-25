using System;
using System.Collections.Generic;

[Serializable]
public sealed class ScenarioSpeciesOutcome
{
    public string speciesId = "";
    public int livingTrees;
    public float basalAreaM2PerHa;
    public int regenerationCells;
    public float regenerationDensity;
    public int plantedRegenerationCells;
}

// Ecological state observed after a Forestry annual step, not a replacement for
// Forestry's authoritative cells, individuals or biological update sequence.
[Serializable]
public sealed class ScenarioEcologicalSnapshot
{
    public int year;
    public int livingTrees;
    public float basalAreaM2PerHa;
    public float meanDbhCm;
    public float meanLight;
    public float meanCanopy;
    public int occupiedRegenerationCells;
    public float meanSharedRegenerationOccupancy;
    public float totalRecentOpening;
    public float meanMosses;
    public float meanFerns;
    public float meanGrasses;
    public float meanForbs;
    public float meanShrubs;
    public float meanFungi;
    public List<ScenarioSpeciesOutcome> species = new List<ScenarioSpeciesOutcome>();
}
