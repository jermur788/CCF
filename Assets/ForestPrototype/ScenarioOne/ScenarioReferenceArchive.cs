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
    [NonSerialized] public bool verifiedFrozenWorld;
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
                string text = Encoding.UTF8.GetString(json.ToArray());
                ScenarioReferenceArchive archive = JsonUtility.FromJson<ScenarioReferenceArchive>(text);
                // v1 was serialized with the v12 save schema. Re-serializing its
                // world with v13 adds fields and yields a *different* hash. Check
                // the original embedded JSON instead, without rewriting the
                // frozen archive or changing the authoritative simulation.
                if (archive == null || archive.milestones == null)
                    return null;
                int offset = 0;
                foreach (ScenarioReferenceMilestone milestone in archive.milestones)
                {
                    int key = text.IndexOf("\"world\"", offset, StringComparison.Ordinal);
                    if (key < 0 || milestone == null)
                        return null;
                    int colon = text.IndexOf(':', key + 7);
                    if (colon < 0)
                        return null;
                    int opening = colon + 1;
                    while (opening < text.Length && char.IsWhiteSpace(text[opening])) opening++;
                    if (opening >= text.Length || text[opening] != '{')
                        return null;
                    var compact = new StringBuilder();
                    bool quoted = false, escaped = false;
                    int depth = 0;
                    int i = opening;
                    for (; i < text.Length; i++)
                    {
                        char c = text[i];
                        if (!quoted && char.IsWhiteSpace(c)) continue;
                        compact.Append(c);
                        if (c == '"' && !escaped) quoted = !quoted;
                        if (!quoted)
                        {
                            if (c == '{') depth++;
                            if (c == '}' && --depth == 0) { i++; break; }
                        }
                        if (quoted && c == '\\') escaped = !escaped;
                        else escaped = false;
                    }
                    if (depth != 0 || Hash(compact.ToString()) != milestone.worldHash)
                        return null;
                    milestone.verifiedFrozenWorld = true;
                    offset = i;
                }
                return archive;
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
            && referenceId == "reference-future-v1" && definitionVersion == "scenario-one-v12"
            && (definition.DefinitionVersion == "scenario-one-v12" || definition.DefinitionVersion == "scenario-one-v13")
            && saveVersion == 12 && saveVersion <= ForestSaveData.CurrentVersion
            && simulationSeed == ecology.SimulationSeed
            && AtYear(0)?.verifiedFrozenWorld == true && AtYear(100)?.verifiedFrozenWorld == true;
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
        data.cropTreeIds?.Sort(StringComparer.Ordinal);
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
        scenario.plantedJuveniles?.Sort((a, b) => string.CompareOrdinal(a.juvenileId, b.juvenileId));
        scenario.clearancePatches?.Sort((a, b) =>
        {
            int byYear = a.createdYear.CompareTo(b.createdYear);
            if (byYear != 0) return byYear;
            int byX = a.center.x.CompareTo(b.center.x);
            return byX != 0 ? byX : a.center.z.CompareTo(b.center.z);
        });
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

    // Compatibility check only: hashes a regeneration-model-0 world in the
    // exact v15 byte layout (version 15, no regenerationModel field), so
    // anchors recorded before save v16 can be compared after the schema bump.
    // Returns null for model-1 worlds, which have no v15 equivalent.
    public static string LegacyV15WorldHash(ForestSaveData data)
    {
        if (data == null || data.regenerationModel != RegenerationModel.Legacy || data.growthModel != GrowthModel.Legacy)
            return null;
        ForestSaveData copy = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 15;
        string json = JsonUtility.ToJson(copy).Replace("\"regenerationModel\":0,", "").Replace("\"growthModel\":0,", "");
        return Hash(json);
    }

    // As above for growth-model-0 worlds in the exact v16 layout (version 16,
    // no growthModel field), so v16 anchors (any regeneration model) stay
    // comparable after the v17 bump. Returns null for growth-model-1 worlds.
    public static string LegacyV16WorldHash(ForestSaveData data)
    {
        if (data == null || data.growthModel != GrowthModel.Legacy)
            return null;
        ForestSaveData copy = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 16;
        string json = JsonUtility.ToJson(copy).Replace("\"growthModel\":0,", "");
        return Hash(json);
    }
}
