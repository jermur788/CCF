using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

public static partial class ForestSaveValidation
{
    private static string ValidateStorms(ForestSaveData data)
    {
        if (data.stormModel < StormModel.None || data.stormModel > StormModel.Latest)
            return "unknown storm model";
        var events = data.scenarioOne?.stormEvents;
        if (data.stormModel == StormModel.None)
            return events != null && events.Count > 0 ? "storms-off save cannot contain storm events" : null;
        if (data.version < 19 || data.scenarioOne == null || events == null || data.ecologicalYear < 0)
            return "storm model 1 requires a version 19 Scenario One save";
        int previousYear = -1;
        var years = new HashSet<int>();
        foreach (StormEventRecord storm in events)
        {
            if (storm == null || storm.year <= previousYear || storm.year < 1 || storm.year > data.ecologicalYear
                || !StormCalibration.Finite(storm.severity) || storm.severity <= 0f || storm.severity > 1f
                || !StormCalibration.Finite(storm.directionDegrees) || storm.directionDegrees < 0f || storm.directionDegrees >= 360f
                || storm.cropTreesLost < 0)
                return "invalid or unordered storm event";
            int victims = data.trees.Count(tree => tree.biologicallyDead && tree.mortalityCause == "windthrow" && tree.mortalityYear == storm.year);
            if (storm.cropTreesLost > victims) return "storm crop losses exceed its victims";
            previousYear = storm.year;
            years.Add(storm.year);
        }
        var victimsById = data.trees.Where(tree => tree.biologicallyDead && tree.mortalityCause == "windthrow")
            .ToDictionary(tree => tree.treeId);
        if (data.scenarioOne.deadwoodRecords == null) return "storm deadwood history is missing";
        var recorded = new HashSet<string>();
        foreach (ScenarioDeadwoodRecord record in data.scenarioOne.deadwoodRecords)
        {
            if (record == null) return "a storm deadwood record is missing";
            if (!victimsById.TryGetValue(record.treeId ?? "", out TreeSaveData victim)) continue;
            if (!recorded.Add(record.treeId)) return "windthrow victim has duplicate deadwood history";
            if (string.IsNullOrEmpty(record.deadwoodId) || record.speciesId != victim.speciesId
                || record.fallenYear != victim.mortalityYear || record.lastDecayYear < record.fallenYear
                || record.lastDecayYear > data.ecologicalYear
                || !StormCalibration.Finite(record.originalVolumeM3) || record.originalVolumeM3 <= 0f
                || !StormCalibration.Finite(record.remainingVolumeM3) || record.remainingVolumeM3 < 0f
                || record.remainingVolumeM3 > record.originalVolumeM3
                || !StormCalibration.Finite(record.originalHeightMeters) || record.originalHeightMeters <= 0f
                || !StormCalibration.Finite(record.originalDiameterCm) || record.originalDiameterCm <= 0f
                || !StormCalibration.Finite(record.worldPosition.x) || !StormCalibration.Finite(record.worldPosition.y)
                || !StormCalibration.Finite(record.worldPosition.z))
                return "windthrow deadwood history is invalid";
        }
        foreach (TreeSaveData tree in victimsById.Values)
        {
            if (!years.Contains(tree.mortalityYear)) return "windthrow victim has no resolved storm event";
            if (!recorded.Contains(tree.treeId)) return "windthrow victim has no deadwood history";
        }
        return null;
    }

    public static string ValidateStormJson(string json, ForestSaveData data)
    {
        if (data == null || data.version < 19) return null;
        try
        {
            var root = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            if (root["stormModel"]?.Type != JTokenType.Integer || root["stormModel"].Value<long>() != data.stormModel)
                return "version 19 requires an explicit integer storm model";
            if (data.scenarioOne == null) return data.stormModel == 0 ? null : "storm model 1 requires Scenario One";
            if (!((root["scenarioOne"] as JObject)?["stormEvents"] is JArray events))
                return "version 19 requires an explicit storm event list";
            foreach (JToken token in events)
            {
                if (!(token is JObject storm) || !Year(storm["year"], data.ecologicalYear)
                    || !Number(storm["severity"]) || !FiniteBearing(storm["directionDegrees"])
                    || storm["cropTreesLost"]?.Type != JTokenType.Integer
                    || storm["cropTreesLost"].Value<long>() < 0 || storm["cropTreesLost"].Value<long>() > int.MaxValue)
                    return "storm event fields are missing or invalid";
            }
        }
        catch (System.Exception) { return "invalid storm JSON"; }
        return null;
    }

    private static bool FiniteBearing(JToken token)
    {
        if (token == null || (token.Type != JTokenType.Float && token.Type != JTokenType.Integer)) return false;
        double value = token.Value<double>();
        return !double.IsNaN(value) && !double.IsInfinity(value) && value >= 0 && value < 360;
    }
}
