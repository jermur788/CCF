using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// Rack Planning & Field Marking V2 save validation (save v21, D-064). Complements the main validator:
// semantic checks on the parsed state, plus a JSON-presence pass for the fields JsonUtility would
// otherwise turn into silent zeros/defaults. Every check runs before the save may touch the live
// world. A malformed V2 plan is refused safely - never repaired, never partially applied.
public static partial class ForestSaveValidation
{
    // Semantic validation. Returns null when the save is safe to load, otherwise a short
    // player-readable reason. A save older than v21 is refused only if it claims V2 state it
    // cannot carry; otherwise it loads as the legacy world it is (model 0, empty rack plan).
    public static string ValidateRackV2(ForestSaveData data)
    {
        if (data == null)
            return null;
        ScenarioOneSaveData scenario = data.scenarioOne;
        if (scenario == null)
            return null;

        if (data.version < 21)
        {
            // Older saves are always model 0 with no plan. Reject one that claims V2 state.
            if (scenario.rackPlanningModel != RackPlanningModel.Legacy || scenario.rackPlanConfirmed
                || (scenario.racks != null && scenario.racks.Count > 0))
                return "a save older than version 21 cannot carry Rack Planning & Field Marking V2 state";
            if (HasRackOwnedOrder(scenario))
                return "a save older than version 21 cannot carry rack-owned work orders";
            return null;
        }

        if (!RackPlanningModel.IsKnown(scenario.rackPlanningModel))
            return $"unknown rack planning model {scenario.rackPlanningModel}";
        if (scenario.rackPlanningModel == RackPlanningModel.RackPlanningFieldMarkingV2
            && StandGeometryModel.ForSave(data) != StandGeometryModel.Enlarged80)
            return "rack planning V2 requires the Enlarged80 stand";

        if (scenario.nextRackId < 1)
            return "invalid next rack id";

        List<RackSaveData> racks = scenario.racks ?? new List<RackSaveData>();
        if (scenario.rackPlanningModel == RackPlanningModel.Legacy)
        {
            // A model-0 world carries no V2 plan: no records, no confirmation, no rack-owned
            // orders, and the id allocator still at its initial value.
            if (racks.Count > 0)
                return "a model-0 world cannot contain V2 rack records";
            if (scenario.rackPlanConfirmed)
                return "a model-0 world cannot hold a confirmed V2 rack plan";
            if (HasRackOwnedOrder(scenario))
                return "a model-0 world cannot contain rack-owned work orders";
            if (scenario.nextRackId != 1)
                return "a model-0 world cannot carry V2 rack id state";
            return null;
        }

        // Model 1: the persisted plan, draft or confirmed.
        if (racks.Count > 5)
            return "a V2 rack plan cannot hold more than five racks";
        if (scenario.rackPlanConfirmed && racks.Count != 5)
            return "a confirmed V2 rack plan needs exactly five racks";

        var treeIndex = new Dictionary<string, TreeSaveData>();
        foreach (TreeSaveData tree in data.trees)
            if (tree != null && !string.IsNullOrEmpty(tree.treeId))
                treeIndex[tree.treeId] = tree;

        var rackIds = new HashSet<int>();
        int highestRackId = 0;
        var treeOwner = new Dictionary<string, int>();
        foreach (RackSaveData rack in racks)
        {
            if (rack == null)
                return "a V2 rack record is missing";
            if (rack.rackId <= 0)
                return $"invalid rack id {rack.rackId}";
            if (!rackIds.Add(rack.rackId))
                return $"duplicate rack id {rack.rackId}";
            highestRackId = System.Math.Max(highestRackId, rack.rackId);

            if (!IsFinite(rack.entranceX) || !IsFinite(rack.entranceZ) || !IsFinite(rack.farX) || !IsFinite(rack.farZ))
                return $"rack {rack.rackId} has non-finite endpoints";
            float dx = rack.farX - rack.entranceX, dz = rack.farZ - rack.entranceZ;
            if (dx * dx + dz * dz < 0.0001f)
                return $"rack {rack.rackId} has degenerate geometry";

            List<string> removal = rack.removalTreeIds ?? new List<string>();
            if (scenario.rackPlanConfirmed && removal.Count == 0)
                return $"confirmed rack {rack.rackId} has no frozen removal set";
            var inRack = new HashSet<string>();
            foreach (string treeId in removal)
            {
                if (string.IsNullOrWhiteSpace(treeId))
                    return $"rack {rack.rackId} has an empty tree id";
                if (!treeIndex.TryGetValue(treeId, out TreeSaveData tree))
                    return $"rack {rack.rackId} references unknown tree {treeId}";
                if (!inRack.Add(treeId))
                    return $"rack {rack.rackId} lists tree {treeId} twice";
                if (treeOwner.TryGetValue(treeId, out int other))
                    return $"tree {treeId} is claimed by rack {other} and rack {rack.rackId}";
                treeOwner[treeId] = rack.rackId;
                // A V2 rack can never silently take a Crop Tree (blue + orange is forbidden).
                if (tree.markType == (int)TreeMarkType.CropTree)
                    return $"rack {rack.rackId} cannot remove Crop Tree {treeId}";
            }

            bool hasEntrance = !string.IsNullOrWhiteSpace(rack.entranceTreeId);
            if (hasEntrance && !inRack.Contains(rack.entranceTreeId))
                return $"rack {rack.rackId} entrance tree {rack.entranceTreeId} is not in its frozen removal set";
            if (rack.entranceMarked && !hasEntrance)
                return $"rack {rack.rackId} is marked without an entrance identity";
            if (rack.entranceMarked && hasEntrance)
            {
                TreeSaveData entrance = treeIndex[rack.entranceTreeId];
                // The orange rack mark shares exclusivity with the red Fell and blue Crop Tree marks.
                if (entrance.markType == (int)TreeMarkType.Fell)
                    return $"rack {rack.rackId} entrance tree {rack.entranceTreeId} already carries a Fell mark";
                if (entrance.markType == (int)TreeMarkType.CropTree)
                    return $"rack {rack.rackId} entrance tree {rack.entranceTreeId} already carries a Crop Tree mark";
            }
            if (scenario.rackPlanConfirmed && !hasEntrance)
                return $"confirmed rack {rack.rackId} has no entrance identity";
        }

        if (scenario.nextRackId <= highestRackId)
            return "invalid next rack id";

        // Work-order linkage: a positive rack link must name a real rack, and a rack can never
        // silently steal a tree an independent open felling order already owns.
        if (scenario.workOrders != null)
        {
            foreach (ScenarioOneWorkOrder order in scenario.workOrders)
            {
                if (order == null)
                    continue;
                if (order.rackId != 0)
                {
                    if (order.rackId < 0 || !rackIds.Contains(order.rackId))
                        return $"work order {order.workOrderId} links to missing rack {order.rackId}";
                }
                if (order.type == ScenarioWorkType.FellTree && order.IsOpen && !string.IsNullOrEmpty(order.targetTreeId)
                    && treeOwner.TryGetValue(order.targetTreeId, out int owner)
                    && order.rackId != owner)
                    return $"tree {order.targetTreeId} is already owned by open work order {order.workOrderId}";
            }
        }
        return null;
    }

    private static bool HasRackOwnedOrder(ScenarioOneSaveData scenario)
    {
        if (scenario.workOrders == null)
            return false;
        foreach (ScenarioOneWorkOrder order in scenario.workOrders)
            if (order != null && order.rackId > 0)
                return true;
        return false;
    }

    // JsonUtility turns missing fields into zeros and defaults. The disk-load path therefore checks
    // that a v21 save states its rack-plan fields explicitly, with the right types, before any parsed
    // state can reach the world. Presence is required only where the schema guarantees a field.
    public static string ValidateRackV2Json(string json, ForestSaveData data)
    {
        if (data == null || data.version < 21)
            return null;
        try
        {
            var root = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            if (!(root["scenarioOne"] is JObject scenario))
                return null; // no Scenario One state, so no rack-plan fields are required
            if (scenario["rackPlanningModel"]?.Type != JTokenType.Integer
                || scenario["rackPlanConfirmed"]?.Type != JTokenType.Boolean
                || scenario["nextRackId"]?.Type != JTokenType.Integer
                || !(scenario["racks"] is JArray))
                return "version 21 requires explicit rack-plan fields";
            foreach (JToken record in (JArray)scenario["racks"])
            {
                if (!(record is JObject rack)
                    || rack["rackId"]?.Type != JTokenType.Integer
                    || !Coordinate(rack["entranceX"]) || !Coordinate(rack["entranceZ"])
                    || !Coordinate(rack["farX"]) || !Coordinate(rack["farZ"])
                    || !(rack["removalTreeIds"] is JArray removal)
                    || rack["entranceTreeId"]?.Type != JTokenType.String
                    || rack["entranceMarked"]?.Type != JTokenType.Boolean)
                    return "a version 21 rack record is missing required fields";
                foreach (JToken treeId in removal)
                    if (treeId.Type != JTokenType.String)
                        return "a version 21 rack removal set holds a non-string tree id";
            }
            if (scenario["workOrders"] is JArray orders)
                foreach (JToken record in orders)
                    if (!(record is JObject order) || order["rackId"]?.Type != JTokenType.Integer)
                        return "a version 21 work order is missing its rack link";
        }
        catch (System.Exception)
        {
            return "invalid version 21 rack JSON";
        }
        return null;
    }

    private static bool Coordinate(JToken token)
        => token != null && (token.Type == JTokenType.Float || token.Type == JTokenType.Integer);
}
