// Shared by two builds of the check: the pre-v23 code (ac876a6, save v22) and the v23 code. Both print the same
// fixture worlds through every world-identity hash, so the runner can prove the v23 bump moved none of them.
using System.Collections.Generic;
using System.Text;

public static class HashFixtures
{
    private static ForestSaveData World(int geometry, int regen, int growth, int storm, int rackModel, bool rackState, bool rackOrder)
    {
        var data = new ForestSaveData
        {
            ecologicalYear = 40, standGeometryModel = geometry, regenerationModel = regen, growthModel = growth, stormModel = storm,
            trees = new List<TreeSaveData> { new TreeSaveData { treeId = "P2" }, new TreeSaveData { treeId = "P1" } },
            scenarioOne = new ScenarioOneSaveData
            {
                initialized = true, cashCents = 12345, rackPlanningModel = rackModel,
                workOrders = new List<ScenarioOneWorkOrder>
                {
                    new ScenarioOneWorkOrder { workOrderId = 2, type = ScenarioWorkType.FellTree, targetTreeId = "P2" },
                    new ScenarioOneWorkOrder { workOrderId = 1, type = ScenarioWorkType.FellTree, targetTreeId = "P1" }
                },
                annualReports = new List<ScenarioAnnualReport> { new ScenarioAnnualReport { year = 2 }, new ScenarioAnnualReport { year = 1 } }
            }
        };
        if (rackOrder) data.scenarioOne.workOrders[0].rackId = 1;
        if (rackState)
        {
            data.scenarioOne.rackPlanConfirmed = true;
            data.scenarioOne.nextRackId = 2;
            data.scenarioOne.racks = new List<RackSaveData> { new RackSaveData { rackId = 1, entranceTreeId = "P1", removalTreeIds = new List<string> { "P2", "P1" } } };
        }
        return data;
    }

    // A persisted older save as the loader hands it over: parsed from JSON with its own version still the old one, so the hash path
    // is fed an input whose version differs from the build's CurrentVersion. Fields the old schema never wrote are removed from the
    // text first (a no-op in the pre-v23 build), so both builds parse the same bytes an old writer produced.
    private static ForestSaveData Parsed(ForestSaveData world, int version)
    {
        world.version = version;
        string json = UnityEngine.JsonUtility.ToJson(world)
            .Replace(",\"nextPrescriptionId\":1,\"prescriptions\":[]", "")
            .Replace(",\"prescriptionId\":0,\"prescriptionCoverageUnitId\":\"\",\"prescriptionCropTreeId\":\"\",\"prescriptionReason\":0", "")
            .Replace(",\"prescriptionId\":0", "");
        if (version < 22)
            json = System.Text.RegularExpressions.Regex.Replace(json, @",""objectiveAchievements"":\[[^\]]*\]", "");
        return UnityEngine.JsonUtility.FromJson<ForestSaveData>(json);
    }

    private static ForestSaveData WithHistory(ForestSaveData world)
    {
        world.scenarioOne.objectiveAchievements = new List<ScenarioObjectiveAchievement>
        { new ScenarioObjectiveAchievement { objectiveId = "minimum-year", achievedYear = 5, targetValueAtAchievement = 25f } };
        return world;
    }

    public static List<KeyValuePair<string, ForestSaveData>> Build() => new List<KeyValuePair<string, ForestSaveData>>
    {
        new KeyValuePair<string, ForestSaveData>("legacy40-default", World(0, 0, 0, 0, 0, false, false)),
        new KeyValuePair<string, ForestSaveData>("legacy40-regen2-growth1", World(0, 2, 1, 0, 0, false, false)),
        new KeyValuePair<string, ForestSaveData>("legacy40-regen1", World(0, 1, 0, 0, 0, false, false)),
        new KeyValuePair<string, ForestSaveData>("legacy40-storm1", World(0, 2, 1, 1, 0, false, false)),
        new KeyValuePair<string, ForestSaveData>("enlarged80-model0", World(1, 2, 1, 0, 0, false, false)),
        new KeyValuePair<string, ForestSaveData>("enlarged80-model1-empty-plan", World(1, 2, 1, 0, 1, false, false)),
        new KeyValuePair<string, ForestSaveData>("enlarged80-model1-rack-state", World(1, 2, 1, 0, 1, true, true)),
        new KeyValuePair<string, ForestSaveData>("legacy40-rack-order-only", World(0, 2, 1, 0, 0, false, true)),
        new KeyValuePair<string, ForestSaveData>("parsed-v21-legacy40-default", Parsed(World(0, 2, 1, 0, 0, false, false), 21)),
        new KeyValuePair<string, ForestSaveData>("parsed-v21-enlarged80-model1-empty-plan", Parsed(World(1, 2, 1, 0, 1, false, false), 21)),
        new KeyValuePair<string, ForestSaveData>("parsed-v21-enlarged80-rack-state", Parsed(World(1, 2, 1, 0, 1, true, true), 21)),
        new KeyValuePair<string, ForestSaveData>("parsed-v21-legacy40-rack-order-only", Parsed(World(0, 2, 1, 0, 0, false, true), 21)),
        new KeyValuePair<string, ForestSaveData>("parsed-v22-legacy40-default", Parsed(World(0, 2, 1, 0, 0, false, false), 22)),
        new KeyValuePair<string, ForestSaveData>("parsed-v22-legacy40-history", Parsed(WithHistory(World(0, 2, 1, 0, 0, false, false)), 22)),
        new KeyValuePair<string, ForestSaveData>("parsed-v22-enlarged80-empty-plan", Parsed(World(1, 2, 1, 0, 1, false, false), 22)),
        new KeyValuePair<string, ForestSaveData>("parsed-v22-enlarged80-rack-state-history", Parsed(WithHistory(World(1, 2, 1, 0, 1, true, true)), 22)),
        new KeyValuePair<string, ForestSaveData>("current-enlarged80-rack-state-history", WithHistory(World(1, 2, 1, 0, 1, true, true))),
        new KeyValuePair<string, ForestSaveData>("current-legacy40-history", WithHistory(World(0, 2, 1, 0, 0, false, false))),
    };

    public static string Dump()
    {
        var text = new StringBuilder();
        foreach (var fixture in Build())
        {
            ForestSaveData d = fixture.Value;
            text.Append(fixture.Key).Append("|WorldHash=").Append(ScenarioReferenceArchive.WorldHash(d))
                .Append("|Current=").Append(ScenarioReferenceArchive.CurrentWorldHash(d))
                .Append("|LegacyCompat=").Append(ScenarioReferenceArchive.LegacyCompatibleWorldHash(d) ?? "null")
                .Append("|V15=").Append(ScenarioReferenceArchive.LegacyV15WorldHash(d) ?? "null")
                .Append("|V16=").Append(ScenarioReferenceArchive.LegacyV16WorldHash(d) ?? "null")
                .Append("|V17=").Append(ScenarioReferenceArchive.LegacyV17WorldHash(d) ?? "null")
                .Append("|V18=").Append(ScenarioReferenceArchive.LegacyV18WorldHash(d) ?? "null").Append('\n');
        }
        return text.ToString();
    }
}
