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

    // Reference Future v1 is Legacy40 permanently (D-056): its saved cell indices only mean something on
    // the 40 x 40 m, 8 x 8 grid. It is never resized, regenerated, migrated or reinterpreted.
    public const int RequiredStandGeometryModel = StandGeometryModel.Legacy40;

    // True only when the archive is verified for this scenario AND the live world is in the archive's
    // geometry. A wrong live geometry must never read as a match merely because scenario, seed and
    // version agree.
    public bool Matches(ScenarioOneDefinition definition, ForestEcologyController ecology)
        => IdentityMatches(definition, ecology) && ecology.StandGeometryModelVersion == RequiredStandGeometryModel;

    // Everything except the live geometry. Reference preview uses this one: preview itself puts the world
    // into the archive's geometry (LoadData applies it from the archived save, before any cell-indexed
    // record is restored) and puts the player's own geometry and exact world back on exit.
    public bool IdentityMatches(ScenarioOneDefinition definition, ForestEcologyController ecology)
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
        scenario.stormEvents?.Sort((a, b) => a.year.CompareTo(b.year));
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
        scenario.objectiveAchievements?.Sort((a, b) =>
        {
            int byYear = a.achievedYear.CompareTo(b.achievedYear);
            return byYear != 0 ? byYear : string.CompareOrdinal(a.objectiveId, b.objectiveId);
        });
        scenario.racks?.Sort((a, b) => a.rackId.CompareTo(b.rackId));
        scenario.prescriptions?.Sort((a, b) => a.prescriptionId.CompareTo(b.prescriptionId));
        if (scenario.prescriptions != null)
            foreach (PrescriptionSaveData prescription in scenario.prescriptions)
            {
                prescription.sampleCropTreeIds?.Sort(StringComparer.Ordinal);
                prescription.sampleRemovalIds?.Sort(StringComparer.Ordinal);
                prescription.coverageDecisions?.Sort((a, b) => string.CompareOrdinal(a.unitId, b.unitId));
            }
        if (scenario.racks != null)
            foreach (RackSaveData rack in scenario.racks)
                rack.removalTreeIds?.Sort(StringComparer.Ordinal);
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

    // Two explicit world identities (D-056). They are different on purpose.
    //
    // CurrentWorldHash is the geometry-aware current identity. It NEVER strips or normalises the geometry
    // field, so an Enlarged80 world can never hash like an otherwise equivalent Legacy40 world. New
    // Enlarged80 anchors are labelled and use this.
    //
    // LegacyCompatibleWorldHash reproduces the historical pre-v20 layout (no geometry field, a pre-v20
    // header read as v19) for Legacy40 worlds ONLY, so every historical anchor stays valid. It returns
    // null for any other geometry: a non-Legacy40 world has no historical layout.
    //
    // WorldHash keeps the behaviour the existing gates were written against: a Legacy40 world is the legacy
    // compatible identity, any other geometry is the geometry-aware current identity.
    //
    // Save v21 (RACK-V2-03B) added rack-plan fields to ScenarioOneSaveData and a rack link to every work
    // order. Every hash above must keep reproducing each world's PRE-V21 identity while the world's V2 rack
    // plan is still the empty default (the only worlds any accepted anchor covers), or the anchors would
    // move solely because the schema grew. A world that actually carries rack state hashes the full v21
    // JSON instead: pre-v21 code could not have produced such a world, so its identity is genuinely new
    // ("new V2 worlds may receive new V2-specific anchors"). The rule is deliberate and versioned here
    // rather than rebaselining accepted evidence.
    //
    // Save v22 (S1-C T-05) added objectiveAchievements. It is a write-once observation record derived from the same
    // simulation, so it is NEVER part of a world identity: every hash below drops it, whether or not it is empty. That is
    // what keeps a pre-v22 anchor exact even when a v22 run has since recorded achievements. The one other effect of the
    // bump, the version header, is relabelled to the version the pre-v22 layout carried (20 or 21, see HashLayout).
    public static string WorldHash(ForestSaveData data)
        => GeometryIsLegacy(data) ? LegacyCompatibleWorldHash(data) : CurrentWorldHash(data);

    public static string CurrentWorldHash(ForestSaveData data)
    {
        Canonicalize(data);
        return Hash(HashLayout(JsonUtility.ToJson(data), data));
    }

    public static string LegacyCompatibleWorldHash(ForestSaveData data)
    {
        if (!GeometryIsLegacy(data))
            return null;
        Canonicalize(data);
        string json = HashLayout(WithoutGeometryField(JsonUtility.ToJson(data)), data);
        // A save written since v20 is re-labelled as the pre-v20 header it must reproduce (the v19
        // layout). The rewrite used to key on exactly v20; every later schema version needs it too.
        return Hash(data.version >= 20 ? "{\"version\":19" + json.Substring(json.IndexOf(',')) : json);
    }

    // The hashed layout: the pre-v21 one while the V2 rack plan is still the empty default, the full
    // current JSON as soon as the world carries rack state. The pre-v21 layout is exactly what save v20
    // wrote: no rack bytes AND the v20 version header, because the accepted anchors hash that byte
    // stream and the header is part of it. (Only ForestSaveData carries a "version" key.)
    // A world that carries rack state keeps the v21 header it always hashed with.
    private static string HashLayout(string json, ForestSaveData data)
    {
        // A world that carries prescription state (save v23) has no historical layout: it hashes as its full current JSON.
        if (!PrescriptionStateIsDefault(data))
            return json;
        json = WithoutLaterFields(json);
        // Keyed on the version this input actually carries (a persisted v21 stays v21), not on CurrentVersion.
        // Versions below the relabel target (an older input) were never rewritten and still are not.
        string header = "\"version\":" + data.version + ",";
        if (RackV2StateIsDefault(data))
            return WithoutRackV2Fields(json).Replace(header, data.version >= 21 ? "\"version\":20," : header);
        return json.Replace(header, data.version >= 22 ? "\"version\":21," : header);
    }

    // Removes the v22 history field whatever it holds. The pattern reads the array as quoted strings or non-bracket
    // characters, so an objective id can never end it early.
    private static string WithoutObjectiveAchievements(string json)
        => System.Text.RegularExpressions.Regex.Replace(json,
            @",""objectiveAchievements"":\[(?:[^\]""]|""(?:[^""\\]|\\.)*"")*\]", "");

    // Removes everything save v22 and v23 add while they hold only defaults: the v22 history field, then (for a world that
    // carries no prescription state, see PrescriptionStateIsDefault) the exact default text of the v23 fields. The order
    // run is removed before the single event field so the shorter text cannot clip it.
    private static string WithoutLaterFields(string json)
        => WithoutObjectiveAchievements(json
            .Replace(",\"nextPrescriptionId\":1,\"prescriptions\":[]", "")
            .Replace(",\"prescriptionId\":0,\"prescriptionCoverageUnitId\":\"\",\"prescriptionCropTreeId\":\"\",\"prescriptionReason\":0", "")
            .Replace(",\"prescriptionId\":0", ""));

    // True when the world holds no prescription state (save v23): allocator untouched, no prescription records, and no
    // work order or event linked to one. This is exactly what every pre-v23 save and every world that never made a
    // prescription looks like, so both keep their pre-v23 identity.
    private static bool PrescriptionStateIsDefault(ForestSaveData data)
    {
        ScenarioOneSaveData scenario = data?.scenarioOne;
        if (scenario == null)
            return true;
        if (scenario.nextPrescriptionId != 1 || (scenario.prescriptions != null && scenario.prescriptions.Count > 0))
            return false;
        if (scenario.workOrders != null)
            foreach (ScenarioOneWorkOrder order in scenario.workOrders)
                if (order != null && (order.prescriptionId != 0 || !string.IsNullOrEmpty(order.prescriptionCoverageUnitId)
                    || !string.IsNullOrEmpty(order.prescriptionCropTreeId) || order.prescriptionReason != 0))
                    return false;
        if (scenario.managementEvents != null)
            foreach (ScenarioManagementEvent entry in scenario.managementEvents)
                if (entry != null && entry.prescriptionId != 0)
                    return false;
        return true;
    }

    // True when the world holds no persisted V2 rack state: no confirmation, the id allocator untouched,
    // no rack records and no rack-owned orders. This is exactly the state every pre-v21 save has, and also
    // a brand-new V2 world before its first rack, so both keep their pre-v21 identity until the plan holds
    // real content. rackPlanningModel is deliberately not part of the test: the workflow flag alone must
    // not move an anchor.
    private static bool RackV2StateIsDefault(ForestSaveData data)
    {
        ScenarioOneSaveData scenario = data?.scenarioOne;
        if (scenario == null)
            return true;
        if (!RackPlanningModel.IsKnown(scenario.rackPlanningModel))
            return false;
        if (scenario.rackPlanConfirmed || scenario.nextRackId != 1)
            return false;
        if (scenario.racks != null && scenario.racks.Count > 0)
            return false;
        if (scenario.workOrders != null)
            foreach (ScenarioOneWorkOrder order in scenario.workOrders)
                if (order != null && order.rackId != 0)
                    return false;
        return true;
    }

    // Removes exactly the bytes save v21 adds while the plan is empty: the four rack-plan fields
    // (declared last and adjacent in ScenarioOneSaveData) and each work order's trailing rack link.
    private static string WithoutRackV2Fields(string json)
        => json.Replace(",\"rackPlanningModel\":0,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackPlanningModel\":1,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackId\":0", "");

    private static string WithoutGeometryField(string json) => json.Replace("\"standGeometryModel\":0,", "");

    private static bool GeometryIsLegacy(ForestSaveData data)
        => data != null && data.standGeometryModel == StandGeometryModel.Legacy40;

    // Compatibility check only: hashes a regeneration-model-0 world in the
    // exact v15 byte layout (version 15, no regenerationModel field), so
    // anchors recorded before save v16 can be compared after the schema bump.
    // Returns null for model-1 worlds, which have no v15 equivalent.
    public static string LegacyV15WorldHash(ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data) || !PrescriptionStateIsDefault(data)
            || data.regenerationModel != RegenerationModel.Legacy || data.growthModel != GrowthModel.Legacy)
            return null;
        ForestSaveData copy = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 15;
        string json = WithoutGeometryField(JsonUtility.ToJson(copy)).Replace("\"regenerationModel\":0,", "").Replace("\"growthModel\":0,", "");
        return Hash(WithoutStormFields(WithoutCompetitionFields(WithoutRackV2Fields(WithoutLaterFields(json)))));
    }

    // As above for growth-model-0 worlds in the exact v16 layout (version 16,
    // no growthModel field), so historical v16 anchors (models 0/1) stay
    // comparable after the v17 bump. Returns null for growth-model-1 worlds.
    public static string LegacyV16WorldHash(ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data) || !PrescriptionStateIsDefault(data)
            || data.growthModel != GrowthModel.Legacy || data.regenerationModel >= RegenerationModel.Competition)
            return null;
        ForestSaveData copy = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 16;
        string json = WithoutGeometryField(JsonUtility.ToJson(copy)).Replace("\"growthModel\":0,", "");
        return Hash(WithoutStormFields(WithoutCompetitionFields(WithoutRackV2Fields(WithoutLaterFields(json)))));
    }
    private static string WithoutCompetitionFields(string json)
        => System.Text.RegularExpressions.Regex.Replace(json,
            @",""(?:brambleCover|brackenCover|competitionUpdatedYear)"":-?[0-9]+(?:\.[0-9]+)?(?:[Ee][+-]?[0-9]+)?", "");

    // Exact v17 compatibility only; never substitutes for model-2 world hashes.
    public static string LegacyV17WorldHash(ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data) || !PrescriptionStateIsDefault(data) || data.regenerationModel >= RegenerationModel.Competition) return null;
        var copy = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 17;
        return Hash(WithoutStormFields(WithoutCompetitionFields(WithoutRackV2Fields(WithoutLaterFields(WithoutGeometryField(JsonUtility.ToJson(copy)))))));
    }

    private static bool StormsInert(ForestSaveData data)
        => data != null && data.stormModel == StormModel.None
            && (data.scenarioOne?.stormEvents == null || data.scenarioOne.stormEvents.Count == 0);

    private static string WithoutStormFields(string json)
        => json.Replace("\"stormModel\":0,", "").Replace(",\"stormEvents\":[]", "");

    // Compatibility only: retains the full accepted model-2 state and exact
    // v18 layout. Active storm worlds deliberately have no historical hash.
    public static string LegacyV18WorldHash(ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data) || !PrescriptionStateIsDefault(data)) return null;
        Canonicalize(data);
        string json = WithoutRackV2Fields(WithoutLaterFields(WithoutGeometryField(JsonUtility.ToJson(data))));
        // Do not JSON-round-trip floating values merely to change a header.
        // This comparator must use exactly the former WorldHash byte stream.
        int firstComma = json.IndexOf(',');
        json = "{\"version\":18" + json.Substring(firstComma);
        return Hash(WithoutStormFields(json));
    }

}
