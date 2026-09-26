using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public sealed class ScenarioReferenceMilestone
{
    public int year;
    public string worldHash = "";
    public ForestSaveData world;
}

// Frozen output of a verified run, not a fabricated Year-100 forest. Each
// milestone is a normal full-world Forestry/Scenario One save captured after
// real work resolution and annual ecology, including history and provenance.
[Serializable]
public sealed class ScenarioReferenceArchive
{
    public const string ResourceName = "ScenarioOneReferenceFutureV1";
    public string referenceId = "reference-future-v1";
    public string scenarioId = "scenario-one";
    public string definitionVersion = "";
    public int saveVersion;
    public int simulationSeed;
    public string startingStandHash = "";
    public string scheduleHash = "";
    public ScenarioReferenceSchedule schedule;
    public List<string> futureTreeIds = new List<string>();
    public List<int> oakPlantingCells = new List<int>();
    public List<int> beechPlantingCells = new List<int>();
    public List<ScenarioReferenceMilestone> milestones = new List<ScenarioReferenceMilestone>();

    public ScenarioReferenceMilestone AtYear(int year)
    {
        return milestones?.FirstOrDefault(milestone => milestone != null && milestone.year == year);
    }

    public static ScenarioReferenceArchive Load()
    {
        TextAsset asset = Resources.Load<TextAsset>(ResourceName);
        if (asset == null) return null;
        try
        {
            using (var compressed = new MemoryStream(asset.bytes))
            using (var decoder = new GZipStream(compressed, CompressionMode.Decompress))
            using (var json = new MemoryStream())
            {
                decoder.CopyTo(json);
                return JsonUtility.FromJson<ScenarioReferenceArchive>(Encoding.UTF8.GetString(json.ToArray()));
            }
        }
        catch (Exception error)
        {
            Debug.LogWarning("Reference Future archive could not be read: " + error.Message);
            return null;
        }
    }

    public bool Matches(ScenarioOneDefinition definition, ForestEcologyController ecology)
    {
        return definition != null && ecology != null && scenarioId == definition.ScenarioId
            && definitionVersion == definition.DefinitionVersion
            && saveVersion == ForestSaveData.CurrentVersion
            && simulationSeed == ecology.SimulationSeed
            && AtYear(0)?.world != null && AtYear(100)?.world != null;
    }

    public static void Canonicalize(ForestSaveData data)
    {
        if (data == null) return;
        data.trees?.Sort((a, b) => string.CompareOrdinal(a.treeId, b.treeId));
        data.cells?.Sort((a, b) => a.index.CompareTo(b.index));
        if (data.cells != null)
            foreach (ForestCellSaveData cell in data.cells)
                cell.cohorts?.Sort((a, b) => string.CompareOrdinal(a.speciesId, b.speciesId));
        data.buildables?.Sort((a, b) => string.CompareOrdinal(a.buildId, b.buildId));
        data.storages?.Sort((a, b) => string.CompareOrdinal(a.storageId, b.storageId));
        data.markedTreeIds?.Sort(StringComparer.Ordinal);
        ScenarioOneSaveData scenario = data.scenarioOne;
        if (scenario == null) return;
        scenario.workOrders?.Sort((a, b) => a.workOrderId.CompareTo(b.workOrderId));
        scenario.inventory?.Sort((a, b) => string.CompareOrdinal(a.itemId, b.itemId));
        scenario.annualReports?.Sort((a, b) => a.year.CompareTo(b.year));
        scenario.managementEvents?.Sort((a, b) => a.eventId.CompareTo(b.eventId));
        scenario.ecologicalSnapshots?.Sort((a, b) => a.year.CompareTo(b.year));
        if (scenario.ecologicalSnapshots != null)
            foreach (ScenarioEcologicalSnapshot snapshot in scenario.ecologicalSnapshots)
                snapshot.species?.Sort((a, b) => string.CompareOrdinal(a.speciesId, b.speciesId));
        scenario.understoreyCells?.Sort((a, b) => a.cellIndex.CompareTo(b.cellIndex));
        scenario.deadwoodRecords?.Sort((a, b) => string.CompareOrdinal(a.deadwoodId, b.deadwoodId));
    }

    // Stable UTF-8 FNV-1a fingerprint of serialized content under the same
    // Unity/Forestry configuration. Lists must be canonicalized first.
    public static string Hash(string text)
    {
        ulong hash = 14695981039346656037UL;
        foreach (byte value in Encoding.UTF8.GetBytes(text ?? ""))
        {
            hash ^= value;
            hash *= 1099511628211UL;
        }
        return hash.ToString("X16");
    }

    public static string WorldHash(ForestSaveData data)
    {
        Canonicalize(data);
        return Hash(JsonUtility.ToJson(data));
    }
}
