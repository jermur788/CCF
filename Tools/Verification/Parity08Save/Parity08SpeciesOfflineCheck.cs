using System;
using System.Collections.Generic;

// PARITY-08: ForestSaveValidation.ValidateSpecies refuses unknown saved species on every authoritative path, keeps the
// historical meaning of empty IDs and pre-field versions, and ignores history-only records. Known species are the three
// simulation-ready Scenario One species. Run by run_offline_check.py.
public static class Parity08SpeciesOfflineCheck
{
    private static int checks;
    private static readonly Func<string, bool> Known = id => id == "sitka-spruce" || id == "beech" || id == "sessile-oak";

    private static ForestSaveData World(int version)
    {
        var data = new ForestSaveData { version = version };
        data.trees.Add(new TreeSaveData { treeId = "t1", speciesId = "sitka-spruce" });
        data.trees.Add(new TreeSaveData { treeId = "t2", speciesId = "beech" });
        data.trees.Add(new TreeSaveData { treeId = "t3", speciesId = "" });
        data.cells = new List<ForestCellSaveData> { new ForestCellSaveData { index = 0 } };
        data.cells[0].cohorts.Add(new ForestRegenerationCohortSaveData { speciesId = "sessile-oak", density = 2f });
        data.scenarioOne = new ScenarioOneSaveData();
        data.scenarioOne.plantedJuveniles.Add(new PlantedJuvenileSaveData { juvenileId = "j1", speciesId = "beech" });
        data.scenarioOne.workOrders.Add(new ScenarioOneWorkOrder { workOrderId = 1, speciesId = "sessile-oak", status = ScenarioWorkStatus.Pending });
        data.scenarioOne.workOrders.Add(new ScenarioOneWorkOrder { workOrderId = 2, speciesId = "", status = ScenarioWorkStatus.Approved });
        return data;
    }

    private static void Check(bool condition, string label)
    {
        checks++;
        if (!condition) throw new Exception("FAIL " + label);
        Console.WriteLine("PASS " + label);
    }

    private static void Refused(string label, int version, Action<ForestSaveData> corrupt)
    {
        ForestSaveData data = World(version);
        corrupt(data);
        string problem = ForestSaveValidation.ValidateSpecies(data, Known);
        Check(problem != null && problem.Contains("unknown species") && problem.Contains("not-a-species"), "refused: " + label + " -> " + problem);
    }

    private static void Accepted(string label, int version, Action<ForestSaveData> change)
    {
        ForestSaveData data = World(version);
        change(data);
        Check(ForestSaveValidation.ValidateSpecies(data, Known) == null, "accepted: " + label);
    }

    public static void Main()
    {
        int v = ForestSaveData.CurrentVersion;
        Check(v == 24, "schema is still v24 (no parity bump), CurrentVersion=" + v);
        Accepted("current-version three-species world", v, d => { });
        Accepted("v24 with the teaching latch true", v, d => d.scenarioOne.firstGuidedRackReviewComplete = true);

        Refused("tree species", v, d => d.trees[0].speciesId = "not-a-species");
        Refused("regeneration cohort species", v, d => d.cells[0].cohorts.Add(new ForestRegenerationCohortSaveData { speciesId = "not-a-species", density = 0.5f }));
        Refused("planted juvenile species", v, d => d.scenarioOne.plantedJuveniles[0].speciesId = "not-a-species");
        Refused("pending work order species", v, d => d.scenarioOne.workOrders[0].speciesId = "not-a-species");
        Refused("approved work order species", v, d => d.scenarioOne.workOrders[1].speciesId = "not-a-species");
        Refused("tree species not-a-species on a broadleaf record", v, d => d.trees[1].speciesId = "not-a-species");
        Refused("unknown tree in a v7 save (first version with tree species)", 7, d => d.trees[1].speciesId = "not-a-species");
        Refused("unknown cohort in a v8 save", 8, d => d.cells[0].cohorts[0].speciesId = "not-a-species");
        Refused("unknown juvenile in a v10 save", 10, d => d.scenarioOne.plantedJuveniles[0].speciesId = "not-a-species");

        // Historical meaning preserved: fields the loader does not read in that version are not checked.
        Accepted("v6 tree species field is ignored by the loader", 6, d => d.trees[0].speciesId = "not-a-species");
        Accepted("v7 cohort list is ignored before v8", 7, d => d.cells[0].cohorts[0].speciesId = "not-a-species");
        Accepted("v9 Scenario One records are ignored before v10", 9, d => d.scenarioOne.plantedJuveniles[0].speciesId = "not-a-species");
        Accepted("empty IDs keep the default-species meaning", v, d => { d.trees[0].speciesId = ""; d.scenarioOne.plantedJuveniles[0].speciesId = ""; });
        Accepted("a zero-density cohort is skipped by the loader and not checked", v, d => d.cells[0].cohorts.Add(new ForestRegenerationCohortSaveData { speciesId = "not-a-species", density = 0f }));
        Accepted("completed order species is history only", v, d => d.scenarioOne.workOrders.Add(new ScenarioOneWorkOrder { workOrderId = 3, speciesId = "not-a-species", status = ScenarioWorkStatus.Completed }));
        Accepted("failed order species is history only", v, d => d.scenarioOne.workOrders.Add(new ScenarioOneWorkOrder { workOrderId = 4, speciesId = "not-a-species", status = ScenarioWorkStatus.Failed }));
        Accepted("management event species is history only", v, d => d.scenarioOne.managementEvents.Add(new ScenarioManagementEvent { speciesId = "not-a-species" }));
        Accepted("species-less timber-sale records stay accepted", v, d => { var r = new ScenarioAnnualReport(); r.timberSales.Add(new ScenarioTimberSale()); d.scenarioOne.annualReports.Add(r); });
        Check(ForestSaveValidation.ValidateSpecies(World(v), null) == null && ForestSaveValidation.ValidateSpecies(null, Known) == null,
            "null inputs are a no-op (scene without a spawner keeps prior behaviour)");

        // The latch is stripped from world identity, so it cannot enter any parity/world hash.
        ForestSaveData off = World(v), on = World(v);
        on.scenarioOne.firstGuidedRackReviewComplete = true;
        Check(ScenarioReferenceArchive.WorldHash(off) == ScenarioReferenceArchive.WorldHash(on), "teaching latch does not move WorldHash");

        Console.WriteLine("PARITY08_SPECIES_OFFLINE_PASS checks=" + checks);
    }
}
