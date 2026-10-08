using System.Collections.Generic;
using Newtonsoft.Json.Linq;

// Checks a parsed save before it is allowed to touch the live world.
//
// JsonUtility happily turns "{}" or a truncated-but-well-formed file into a
// ForestSaveData full of defaults (empty tree list, current version). Loading
// that would destroy every tree in the scene, so loading must refuse it
// instead. These checks only reject data that cannot be a real save; they do
// not migrate or repair anything, and every legacy version still passes.
//
// Kept free of scene lookups so it can be tested without Play mode.
public static class ForestSaveValidation
{
    // Highest ForestTreeStage value (Mature, Stump, Sapling, Young).
    private const int MaxTreeStage = 3;

    // Returns null when the save is safe to load, otherwise a short
    // player-readable reason.
    //   liveTreeCount: trees currently in the scene (any state).
    //   ecologyCellCount: cells in the live ecology grid, or 0 if unknown.
    public static string Validate(ForestSaveData data, int liveTreeCount, int ecologyCellCount)
    {
        if (data == null)
            return "the file is empty or not a forest save";
        if (data.trees == null)
            return "the save has no tree list";

        // Felled trees stay as stumps and recruits are never removed, so a real
        // save of a populated forest always lists trees.
        if (data.trees.Count == 0 && liveTreeCount > 0)
            return $"the save lists no trees but the forest has {liveTreeCount}";

        var treeIds = new HashSet<string>();
        for (int i = 0; i < data.trees.Count; i++)
        {
            TreeSaveData tree = data.trees[i];
            if (tree == null)
                return $"tree record {i} is missing";
            if (string.IsNullOrEmpty(tree.treeId))
                return $"tree record {i} has no ID";
            if (!treeIds.Add(tree.treeId))
                return $"tree ID {tree.treeId} appears twice";
            if (tree.stage < 0 || tree.stage > MaxTreeStage)
                return $"tree {tree.treeId} has an unknown stage ({tree.stage})";
            if (!IsFinite(tree.stageTimer))
                return $"tree {tree.treeId} has an invalid stage timer";
            if (data.version >= 14 && tree.biologicallyDead)
            {
                if (tree.stage == (int)ForestTreeStage.Stump)
                    return $"tree {tree.treeId} cannot be both harvested and biologically dead";
                if (string.IsNullOrWhiteSpace(tree.mortalityCause) || tree.mortalityYear < 0)
                    return $"tree {tree.treeId} has an invalid mortality cause/year";
            }
            if (data.version >= 3 && tree.hasSimulation)
            {
                if (!IsFinite(tree.heightMeters) || !IsFinite(tree.diameterCm) || !IsFinite(tree.crownRadiusMeters)
                    || !IsFinite(tree.position.x) || !IsFinite(tree.position.y) || !IsFinite(tree.position.z))
                    return $"tree {tree.treeId} has invalid size or position values";
                if (tree.ageYears < 0)
                    return $"tree {tree.treeId} has a negative age";
            }
        }

        if (data.regenerationModel < RegenerationModel.Legacy || data.regenerationModel > RegenerationModel.Latest)
            return $"unknown regeneration model {data.regenerationModel}";
        if (data.growthModel < GrowthModel.Legacy || data.growthModel > GrowthModel.Latest)
            return $"unknown growth model {data.growthModel}";
        if (data.regenerationModel == RegenerationModel.Competition)
        {
            string competition = ValidateCompetition(data, ecologyCellCount);
            if (competition != null) return competition;
        }
        bool ageBands = data.version >= 16 && data.regenerationModel >= RegenerationModel.AgeBands;

        if (data.cells != null)
        {
            var cellIndices = new HashSet<int>();
            foreach (ForestCellSaveData cell in data.cells)
            {
                if (cell == null)
                    return "an ecology cell record is missing";
                if (cell.index < 0 || (ecologyCellCount > 0 && cell.index >= ecologyCellCount))
                    return $"ecology cell {cell.index} is outside the stand grid";
                if (!cellIndices.Add(cell.index))
                    return $"ecology cell {cell.index} appears twice";
                if (!IsFinite(cell.recentOpening) || !IsFinite(cell.establishmentSuitability)
                    || !IsFinite(cell.regenDensity) || !IsFinite(cell.regenHeight))
                    return $"ecology cell {cell.index} has invalid values";
                if (cell.cohorts == null)
                    continue;
                foreach (ForestRegenerationCohortSaveData cohort in cell.cohorts)
                {
                    if (cohort != null && (!IsFinite(cohort.density) || !IsFinite(cohort.height)))
                        return $"ecology cell {cell.index} has an invalid regeneration cohort";
                }
                if (ageBands)
                {
                    // Repeated species records are valid bands when their keys differ.
                    var bandKeys = new HashSet<string>();
                    var bandCounts = new Dictionary<string, int>();
                    foreach (ForestRegenerationCohortSaveData cohort in cell.cohorts)
                    {
                        if (cohort == null || cohort.density <= 0f)
                            continue;
                        if (cohort.origin != (int)RegenerationOrigin.Natural && cohort.origin != (int)RegenerationOrigin.Planted)
                            return $"ecology cell {cell.index} has a regeneration band with an unknown origin";
                        string group = cohort.speciesId + "|" + cohort.origin;
                        if (!bandKeys.Add(group + "|" + cohort.establishYear))
                            return $"ecology cell {cell.index} repeats regeneration band {cohort.speciesId} year {cohort.establishYear}";
                        // Four represented bands plus at most one sub-threshold accumulator.
                        string slot = group + (cohort.density < RegenerationModel.RepresentationThreshold ? "|sub" : "|band");
                        int limit = cohort.density < RegenerationModel.RepresentationThreshold ? 1 : ForestEcologyCell.MaxBandsPerSpeciesOrigin;
                        bandCounts.TryGetValue(slot, out int count);
                        if (count + 1 > limit)
                            return $"ecology cell {cell.index} exceeds the band limit for {cohort.speciesId}";
                        bandCounts[slot] = count + 1;
                    }
                }
            }
        }

        if (data.version >= 15 && data.scenarioOne != null)
        {
            string problem = ValidateScenarioV15(data.scenarioOne, data.ecologicalYear);
            if (problem != null) return problem;
        }
        return null;
    }

    private static string ValidateCompetition(ForestSaveData data, int count)
    {
        if (data.version < 18 || data.ecologicalYear < 0 || data.scenarioOne == null)
            return "model 2 requires a version 18 Scenario One save";
        var cells = data.scenarioOne.understoreyCells;
        if (cells == null || cells.Count != (count > 0 ? count : data.cells?.Count ?? 0))
            return "model 2 competitor grid is missing or incomplete";
        var indices = new HashSet<int>();
        int expectedIndex = 0;
        foreach (var cell in cells)
            if (cell == null || cell.cellIndex != expectedIndex++ || !indices.Add(cell.cellIndex)
                || !Cover(cell.brambleCover) || !Cover(cell.brackenCover)
                || cell.lastUpdatedYear < 0 || cell.lastUpdatedYear > data.ecologicalYear)
                return "invalid model 2 cell competitor state";
        var patches = data.scenarioOne.clearancePatches;
        if (patches == null) return "model 2 clearance-patch state is missing";
        var keys = new HashSet<(int, float, float, float)>();
        foreach (var patch in patches)
            if (patch == null || !IsFinite(patch.center.x) || !IsFinite(patch.center.y) || !IsFinite(patch.center.z)
                || !IsFinite(patch.radiusMeters) || patch.radiusMeters <= 0f
                || !Cover(patch.brambleCover) || !Cover(patch.brackenCover)
                || patch.createdYear < 0 || patch.createdYear > data.ecologicalYear
                || patch.competitionUpdatedYear < patch.createdYear || patch.competitionUpdatedYear > data.ecologicalYear
                || !keys.Add((patch.createdYear, patch.center.x, patch.center.z, patch.radiusMeters)))
                return "invalid or duplicate model 2 local competitor state";
        return null;
    }

    private static bool Cover(float value) => IsFinite(value) && value >= 0f && value <= 1f;

    // JsonUtility defaults missing numeric fields to zero. The disk-load path
    // therefore checks field presence/types before parsed state touches the world.
    public static string ValidateCompetitionJson(string json, ForestSaveData data)
    {
        if (data == null || data.regenerationModel != RegenerationModel.Competition) return null;
        try
        {
            var root = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            var scenario = root["scenarioOne"] as JObject;
            if (!(scenario?["understoreyCells"] is JArray cells) || !(scenario["clearancePatches"] is JArray patches))
                return "model 2 requires explicit competitor lists";
            foreach (var record in cells)
                if (!(record is JObject cell) || !Number(cell["brambleCover"]) || !Number(cell["brackenCover"])
                    || !Year(cell["lastUpdatedYear"], data.ecologicalYear))
                    return "model 2 cell competitor fields are missing or invalid";
            foreach (var record in patches)
                if (!(record is JObject patch) || !Number(patch["brambleCover"]) || !Number(patch["brackenCover"])
                    || !Year(patch["competitionUpdatedYear"], data.ecologicalYear))
                    return "model 2 local competitor fields are missing or invalid";
        }
        catch (System.Exception) { return "invalid model 2 competitor JSON"; }
        return null;
    }
    private static bool Number(JToken token)
    {
        if (token == null || (token.Type != JTokenType.Float && token.Type != JTokenType.Integer)) return false;
        double value = token.Value<double>();
        return !double.IsNaN(value) && !double.IsInfinity(value) && value >= 0d && value <= 1d;
    }
    private static bool Year(JToken token, int year)
        => token?.Type == JTokenType.Integer && token.Value<long>() >= 0 && token.Value<long>() <= year;

    private static string ValidateScenarioV15(ScenarioOneSaveData scenario, int year)
    {
        if (scenario.ownerMinutesUsedThisYear < 0) return "negative owner time";
        if (scenario.workOrders != null)
            foreach (var order in scenario.workOrders)
                if (order == null || !System.Enum.IsDefined(typeof(CCF.Forestry.WorkEconomy.WorkExecutionMethod), order.executionMethod)
                    || order.harvestJobId < -1 || order.estimatedCostCents < 0 || order.estimatedMinutes < 0)
                    return "invalid v15 work order";
        var ids = new HashSet<string>();
        if (scenario.shelters != null)
            foreach (var shelter in scenario.shelters)
            {
                if (shelter == null || string.IsNullOrWhiteSpace(shelter.shelterId) || !ids.Add(shelter.shelterId)
                    || !IsFinite(shelter.position.x) || !IsFinite(shelter.position.y) || shelter.installedYear < 0
                    || (long)shelter.installedYear > (long)year + 1 || shelter.effectiveYears <= 0
                    || (long)shelter.installedYear + shelter.effectiveYears > int.MaxValue
                    || shelter.failedYear < -1 || (shelter.failedYear >= 0 && shelter.failedYear < shelter.installedYear))
                    return "invalid or duplicate shelter record";
            }
        ids.Clear();
        if (scenario.protectedAreas != null)
            foreach (var area in scenario.protectedAreas)
            {
                if (area == null || string.IsNullOrWhiteSpace(area.areaId) || !ids.Add(area.areaId) || area.polygon == null
                    || area.polygon.Count < 3 || area.installedYear < 0 || (long)area.installedYear > (long)year + 1
                    || area.breachedYear < -1 || (area.breachedYear >= 0 && area.breachedYear < area.installedYear))
                    return "invalid or duplicate protected area";
                foreach (var point in area.polygon) if (!IsFinite(point.x) || !IsFinite(point.y)) return "invalid protection polygon coordinate";
            }
        if (scenario.annualReports != null)
            foreach (var report in scenario.annualReports)
            {
                if (report == null || report.ownerMinutes < 0 || report.harvestMinimumAdjustmentCents < 0) return "invalid v15 economy report";
                var products = new HashSet<CCF.Forestry.WorkEconomy.TimberAssortment>();
                if (report.timberSales != null) foreach (var sale in report.timberSales)
                    if (sale == null || !System.Enum.IsDefined(typeof(CCF.Forestry.WorkEconomy.TimberAssortment), sale.assortment)
                        || !products.Add(sale.assortment) || sale.soldVolumeCm3 < 0 || sale.revenueCents < 0) return "invalid timber sale report";
            }
        if (scenario.managementEvents != null)
            foreach (var entry in scenario.managementEvents)
                if (entry == null || entry.ownerMinutes < 0 || !System.Enum.IsDefined(typeof(CCF.Forestry.WorkEconomy.WorkExecutionMethod), entry.executionMethod))
                    return "invalid v15 management event";
        return null;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
