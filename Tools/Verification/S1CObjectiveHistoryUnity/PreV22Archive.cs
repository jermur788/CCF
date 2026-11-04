// Read-only baseline source from aca74acacc565f72ab802177cf69ca6ebb54d637. Type names prefixed only to coexist with the candidate.
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public static class PreV22Archive
{
public static void Canonicalize(PreV22ForestSaveData data)
    {
        if (data == null) return;
        data.trees?.Sort((a, b) => string.CompareOrdinal(a.treeId, b.treeId));
        data.cells?.Sort((a, b) => a.index.CompareTo(b.index));
        if (data.cells != null)
            foreach (PreV22ForestCellSaveData cell in data.cells)
                cell.cohorts?.Sort((a, b) => string.CompareOrdinal(a.speciesId, b.speciesId));
        data.buildables?.Sort((a, b) => string.CompareOrdinal(a.buildId, b.buildId));
        data.storages?.Sort((a, b) => string.CompareOrdinal(a.storageId, b.storageId));
        data.markedTreeIds?.Sort(StringComparer.Ordinal);
        data.cropTreeIds?.Sort(StringComparer.Ordinal);
        PreV22ScenarioOneSaveData scenario = data.scenarioOne;
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
        scenario.racks?.Sort((a, b) => a.rackId.CompareTo(b.rackId));
        if (scenario.racks != null)
            foreach (PreV22RackSaveData rack in scenario.racks)
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
    // Save v21 (RACK-V2-03B) added rack-plan fields to PreV22ScenarioOneSaveData and a rack link to every work
    // order. Every hash above must keep reproducing each world's PRE-V21 identity while the world's V2 rack
    // plan is still the empty default (the only worlds any accepted anchor covers), or the anchors would
    // move solely because the schema grew. A world that actually carries rack state hashes the full v21
    // JSON instead: pre-v21 code could not have produced such a world, so its identity is genuinely new
    // ("new V2 worlds may receive new V2-specific anchors"). The rule is deliberate and versioned here
    // rather than rebaselining accepted evidence.
    public static string WorldHash(PreV22ForestSaveData data)
        => GeometryIsLegacy(data) ? LegacyCompatibleWorldHash(data) : CurrentWorldHash(data);

    public static string CurrentWorldHash(PreV22ForestSaveData data)
    {
        Canonicalize(data);
        return Hash(HashLayout(JsonUtility.ToJson(data), data));
    }

    public static string LegacyCompatibleWorldHash(PreV22ForestSaveData data)
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
    // stream and the header is part of it. (Only PreV22ForestSaveData carries a "version" key.)
    private static string HashLayout(string json, PreV22ForestSaveData data)
        => RackV2StateIsDefault(data)
            ? WithoutRackV2Fields(json).Replace("\"version\":" + PreV22ForestSaveData.CurrentVersion + ",", "\"version\":20,")
            : json;

    // True when the world holds no persisted V2 rack state: no confirmation, the id allocator untouched,
    // no rack records and no rack-owned orders. This is exactly the state every pre-v21 save has, and also
    // a brand-new V2 world before its first rack, so both keep their pre-v21 identity until the plan holds
    // real content. rackPlanningModel is deliberately not part of the test: the workflow flag alone must
    // not move an anchor.
    private static bool RackV2StateIsDefault(PreV22ForestSaveData data)
    {
        PreV22ScenarioOneSaveData scenario = data?.scenarioOne;
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
    // (declared last and adjacent in PreV22ScenarioOneSaveData) and each work order's trailing rack link.
    private static string WithoutRackV2Fields(string json)
        => json.Replace(",\"rackPlanningModel\":0,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackPlanningModel\":1,\"rackPlanConfirmed\":false,\"nextRackId\":1,\"racks\":[]", "")
               .Replace(",\"rackId\":0", "");

    private static string WithoutGeometryField(string json) => json.Replace("\"standGeometryModel\":0,", "");

    private static bool GeometryIsLegacy(PreV22ForestSaveData data)
        => data != null && data.standGeometryModel == StandGeometryModel.Legacy40;

    // Compatibility check only: hashes a regeneration-model-0 world in the
    // exact v15 byte layout (version 15, no regenerationModel field), so
    // anchors recorded before save v16 can be compared after the schema bump.
    // Returns null for model-1 worlds, which have no v15 equivalent.
    public static string LegacyV15WorldHash(PreV22ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data)
            || data.regenerationModel != RegenerationModel.Legacy || data.growthModel != GrowthModel.Legacy)
            return null;
        PreV22ForestSaveData copy = JsonUtility.FromJson<PreV22ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 15;
        string json = WithoutGeometryField(JsonUtility.ToJson(copy)).Replace("\"regenerationModel\":0,", "").Replace("\"growthModel\":0,", "");
        return Hash(WithoutStormFields(WithoutCompetitionFields(WithoutRackV2Fields(json))));
    }

    // As above for growth-model-0 worlds in the exact v16 layout (version 16,
    // no growthModel field), so historical v16 anchors (models 0/1) stay
    // comparable after the v17 bump. Returns null for growth-model-1 worlds.
    public static string LegacyV16WorldHash(PreV22ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data)
            || data.growthModel != GrowthModel.Legacy || data.regenerationModel >= RegenerationModel.Competition)
            return null;
        PreV22ForestSaveData copy = JsonUtility.FromJson<PreV22ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 16;
        string json = WithoutGeometryField(JsonUtility.ToJson(copy)).Replace("\"growthModel\":0,", "");
        return Hash(WithoutStormFields(WithoutCompetitionFields(WithoutRackV2Fields(json))));
    }
    private static string WithoutCompetitionFields(string json)
        => System.Text.RegularExpressions.Regex.Replace(json,
            @",""(?:brambleCover|brackenCover|competitionUpdatedYear)"":-?[0-9]+(?:\.[0-9]+)?(?:[Ee][+-]?[0-9]+)?", "");

    // Exact v17 compatibility only; never substitutes for model-2 world hashes.
    public static string LegacyV17WorldHash(PreV22ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data) || data.regenerationModel >= RegenerationModel.Competition) return null;
        var copy = JsonUtility.FromJson<PreV22ForestSaveData>(JsonUtility.ToJson(data));
        Canonicalize(copy);
        copy.version = 17;
        return Hash(WithoutStormFields(WithoutCompetitionFields(WithoutRackV2Fields(WithoutGeometryField(JsonUtility.ToJson(copy))))));
    }

    private static bool StormsInert(PreV22ForestSaveData data)
        => data != null && data.stormModel == StormModel.None
            && (data.scenarioOne?.stormEvents == null || data.scenarioOne.stormEvents.Count == 0);

    private static string WithoutStormFields(string json)
        => json.Replace("\"stormModel\":0,", "").Replace(",\"stormEvents\":[]", "");

    // Compatibility only: retains the full accepted model-2 state and exact
    // v18 layout. Active storm worlds deliberately have no historical hash.
    public static string LegacyV18WorldHash(PreV22ForestSaveData data)
    {
        if (!GeometryIsLegacy(data) || !StormsInert(data) || !RackV2StateIsDefault(data)) return null;
        Canonicalize(data);
        string json = WithoutRackV2Fields(WithoutGeometryField(JsonUtility.ToJson(data)));
        // Do not JSON-round-trip floating values merely to change a header.
        // This comparator must use exactly the former WorldHash byte stream.
        int firstComma = json.IndexOf(',');
        json = "{\"version\":18" + json.Substring(firstComma);
        return Hash(WithoutStormFields(json));
    }

}
