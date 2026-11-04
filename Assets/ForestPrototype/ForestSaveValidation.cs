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
public static partial class ForestSaveValidation
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
        // The geometry decides which grid every cell-indexed record below is checked against.
        string geometryProblem = ValidateGeometry(data);
        if (geometryProblem != null)
            return geometryProblem;

        // Do not trust a caller's live grid size: historical saves always use Legacy40.
        ecologyCellCount = StandGeometryModel.CellCount(StandGeometryModel.ForSave(data));
        string cellProblem = ValidateScenarioCellIndices(data.scenarioOne, ecologyCellCount);
        if (cellProblem != null)
            return cellProblem;

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

        string stormProblem = ValidateStorms(data);
        if (stormProblem != null) return stormProblem;

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
                string indexProblem = ValidateRequiredCellIndex(cell.index, ecologyCellCount, "ecology cell");
                if (indexProblem != null) return indexProblem;
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
        string rackProblem = ValidateRackV2(data);
        if (rackProblem != null) return rackProblem;
        string historyProblem = ValidateObjectiveHistory(data);
        if (historyProblem != null) return historyProblem;
        // Last, so every older rejection keeps its existing reason and order.
        string eventProblem = ValidateManagementEventIds(data.scenarioOne);
        if (eventProblem != null) return eventProblem;
        // New in v23, so after every older check: existing rejections keep their reason and order.
        return ValidatePrescriptions(data);
    }

    // Repeated-intervention history orders events by eventId, so an ID that is not positive or is
    // shared by two events makes that order ambiguous. Reject the save instead of renumbering or
    // repairing it. Applies to every save version; a missing list has nothing to check.
    private static string ValidateManagementEventIds(ScenarioOneSaveData scenario)
    {
        if (scenario == null || scenario.managementEvents == null) return null;
        var firstRecordForId = new Dictionary<int, int>();
        for (int i = 0; i < scenario.managementEvents.Count; i++)
        {
            ScenarioManagementEvent entry = scenario.managementEvents[i];
            if (entry == null) continue; // already rejected by ValidateScenarioCellIndices
            if (entry.eventId <= 0)
                return $"management event record {i} has a non-positive ID ({entry.eventId})";
            if (firstRecordForId.TryGetValue(entry.eventId, out int first))
                return $"management event ID {entry.eventId} appears twice (records {first} and {i})";
            firstRecordForId.Add(entry.eventId, i);
        }
        return null;
    }

    private static string ValidateRequiredCellIndex(int index, int count, string label)
        => index >= 0 && index < count ? null : $"{label} {index} is outside the stand grid";

    private static string ValidateOptionalCellIndex(int index, int count, string label)
        => index == -1 ? null : ValidateRequiredCellIndex(index, count, label);

    private static string ValidateScenarioCellIndices(ScenarioOneSaveData scenario, int count)
    {
        if (scenario == null) return null;
        if (scenario.understoreyCells != null)
        {
            var indices = new HashSet<int>();
            foreach (var cell in scenario.understoreyCells)
            {
                if (cell == null) return "an understorey cell record is missing";
                string problem = ValidateRequiredCellIndex(cell.cellIndex, count, "understorey cell");
                if (problem != null) return problem;
                if (!indices.Add(cell.cellIndex)) return "an understorey cell appears twice";
            }
        }
        if (scenario.workOrders != null)
            foreach (var order in scenario.workOrders)
            {
                if (order == null) return "a work order record is missing";
                // Cell-based planting/removal need an assigned cell; tree-based orders
                // can carry the historical -1 sentinel and resolve by tree ID/position.
                bool required = order.type == ScenarioWorkType.PlantJuvenile
                    || order.type == ScenarioWorkType.RemoveRegeneration;
                string problem = required
                    ? ValidateRequiredCellIndex(order.cellIndex, count, "work order cell")
                    : ValidateOptionalCellIndex(order.cellIndex, count, "work order cell");
                if (problem != null) return problem;
            }
        if (scenario.plantedJuveniles != null)
            foreach (var juvenile in scenario.plantedJuveniles)
            {
                if (juvenile == null) return "a planted juvenile record is missing";
                string problem = ValidateOptionalCellIndex(juvenile.cellIndex, count, "planted juvenile cell");
                if (problem != null) return problem;
            }
        if (scenario.deadwoodRecords != null)
            foreach (var record in scenario.deadwoodRecords)
            {
                if (record == null) return "a deadwood record is missing";
                string problem = ValidateOptionalCellIndex(record.cellIndex, count, "deadwood cell");
                if (problem != null) return problem;
            }
        if (scenario.managementEvents != null)
            foreach (var entry in scenario.managementEvents)
            {
                if (entry == null) return "a management event record is missing";
                string problem = ValidateOptionalCellIndex(entry.cellIndex, count, "management event cell");
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

    // ---- PARITY-08 saved species identity ------------------------------------------------------------------------
    // Every record whose species decides what the loader builds or what a pending contractor job will do must name
    // a species the scene can simulate. Before this check the loader turned an unknown tree species into the default
    // Sitka and dropped unknown regeneration bands while it was already changing the world. An empty ID keeps its
    // historical meaning (the scene's default species), and only the versions in which the loader reads the field
    // are checked, so legacy saves are unaffected. History-only text (resolved orders, management events, deadwood
    // records, which are already tied to their victim tree) is not authoritative and is not checked.
    //   isLoadableSpecies: true when the scene can resolve a simulation-ready species with this non-empty ID.
    public static string ValidateSpecies(ForestSaveData data, System.Func<string, bool> isLoadableSpecies)
    {
        if (data == null || isLoadableSpecies == null)
            return null;
        bool Unknown(string id) => !string.IsNullOrEmpty(id) && !isLoadableSpecies(id);

        if (data.version >= 7 && data.trees != null)
            foreach (TreeSaveData tree in data.trees)
                if (tree != null && Unknown(tree.speciesId))
                    return $"tree {tree.treeId} has unknown species '{tree.speciesId}'";

        if (data.version >= 8 && data.cells != null)
            foreach (ForestCellSaveData cell in data.cells)
                if (cell?.cohorts != null)
                    foreach (ForestRegenerationCohortSaveData cohort in cell.cohorts)
                        if (cohort != null && cohort.density > 0f && Unknown(cohort.speciesId))
                            return $"ecology cell {cell.index} has regeneration of unknown species '{cohort.speciesId}'";

        ScenarioOneSaveData scenario = data.version >= 10 ? data.scenarioOne : null;
        if (scenario == null)
            return null;
        if (scenario.plantedJuveniles != null)
            foreach (PlantedJuvenileSaveData juvenile in scenario.plantedJuveniles)
                if (juvenile != null && Unknown(juvenile.speciesId))
                    return $"planted juvenile {juvenile.juvenileId} has unknown species '{juvenile.speciesId}'";
        if (scenario.workOrders != null)
            foreach (ScenarioOneWorkOrder order in scenario.workOrders)
                if (order != null && (order.status == ScenarioWorkStatus.Pending || order.status == ScenarioWorkStatus.Approved)
                    && Unknown(order.speciesId))
                    return $"work order {order.workOrderId} has unknown species '{order.speciesId}'";
        return null;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    // ---- S1-C T-05 objective history (save v22) ------------------------------------------------------------------
    // Same shape as the Rack V2 validator: a semantic check on the parsed state plus a JSON-presence pass for what
    // JsonUtility would silently default. Both run before the save can touch the live world; a malformed history is
    // refused, never repaired or partially applied.

    // Fixed objective ids from ScenarioOneObjectives.Evaluate; "introduced-<speciesId>" is built per shop species.
    private static readonly HashSet<string> FixedObjectiveIds = new HashSet<string>
    {
        "minimum-year", "retained-canopy", "continuous-canopy", "regeneration", "fallen-deadwood", "managed-opening"
    };
    private const string IntroducedObjectivePrefix = "introduced-";

    // Returns null when the history is safe to load, otherwise a short player-readable reason.
    public static string ValidateObjectiveHistory(ForestSaveData data)
    {
        ScenarioOneSaveData scenario = data?.scenarioOne;
        if (scenario == null)
            return null;
        List<ScenarioObjectiveAchievement> records = scenario.objectiveAchievements;
        if (records == null || records.Count == 0)
            return null; // missing or empty is the valid "not recorded" state at every version

        if (data.version < 22)
            return "a save older than version 22 cannot carry objective achievement history";

        var seen = new HashSet<string>();
        int previousYear = int.MinValue;
        foreach (ScenarioObjectiveAchievement record in records)
        {
            if (record == null)
                return "an objective achievement record is missing";
            if (!IsKnownObjectiveId(record.objectiveId))
                return $"unknown objective '{record.objectiveId}' in the achievement history";
            if (!seen.Add(record.objectiveId))
                return $"objective '{record.objectiveId}' appears twice in the achievement history";
            if (record.achievedYear < 0 || record.achievedYear > data.ecologicalYear)
                return $"objective '{record.objectiveId}' has an impossible achieved year {record.achievedYear}";
            // Records are appended as evaluation observes them, so years never go backwards.
            if (record.achievedYear < previousYear)
                return "the objective achievement history is out of order";
            previousYear = record.achievedYear;
            if (!IsFinite(record.targetValueAtAchievement) || record.targetValueAtAchievement < 0f)
                return $"objective '{record.objectiveId}' has an invalid recorded target";
        }
        return null;
    }

    private static bool IsKnownObjectiveId(string id)
        => !string.IsNullOrEmpty(id)
            && (FixedObjectiveIds.Contains(id)
                || (id.StartsWith(IntroducedObjectivePrefix, System.StringComparison.Ordinal)
                    && id.Length > IntroducedObjectivePrefix.Length && id.Trim() == id));

    // A v22 save must state the field explicitly, as an array of fully typed records, with no duplicate keys.
    public static string ValidateObjectiveHistoryJson(string json, ForestSaveData data)
    {
        if (data == null || data.version < 22)
            return null;
        try
        {
            var root = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            if (!(root["scenarioOne"] is JObject scenario))
                return null; // no Scenario One state, so no history field is required
            if (!(scenario["objectiveAchievements"] is JArray records))
                return "version 22 requires an explicit objective achievement history";
            foreach (JToken record in records)
                if (!(record is JObject item)
                    || item["objectiveId"]?.Type != JTokenType.String
                    || item["achievedYear"]?.Type != JTokenType.Integer
                    || !(item["targetValueAtAchievement"]?.Type == JTokenType.Float
                        || item["targetValueAtAchievement"]?.Type == JTokenType.Integer))
                    return "a version 22 objective achievement record is missing required fields";
        }
        catch (System.Exception)
        {
            return "invalid version 22 objective history JSON";
        }
        return null;
    }

    // ---- MFM-06 prescription state and provenance (save v23) ------------------------------------------------------------
    // Same contract as the rack and objective-history validators: semantic checks on the parsed state plus a JSON-presence pass
    // for fields JsonUtility would turn into silent defaults. Both run before the save may touch the live world; a malformed
    // prescription is refused, never repaired, renumbered or partially applied.
    private const int MaxPrescriptions = 1024;
    private const int MaxIdLength = 64;
    private const int MaxListEntries = 4096;

    // Returns null when the prescription state is safe to load, otherwise a short player-readable reason.
    public static string ValidatePrescriptions(ForestSaveData data)
    {
        ScenarioOneSaveData scenario = data?.scenarioOne;
        if (scenario == null)
            return null;

        if (data.version < 23)
        {
            // An older save is the "no prescription" world. Reject one that claims prescription state.
            bool claimsState = (scenario.prescriptions != null && scenario.prescriptions.Count > 0)
                || (scenario.workOrders != null && scenario.workOrders.Exists(o => o != null && HasPrescriptionLink(o)))
                || (scenario.managementEvents != null && scenario.managementEvents.Exists(e => e != null && e.prescriptionId != 0));
            return claimsState ? "a save older than version 23 cannot carry prescription state" : null;
        }

        List<PrescriptionSaveData> records = scenario.prescriptions ?? new List<PrescriptionSaveData>();
        if (scenario.nextPrescriptionId < 1)
            return $"invalid prescription id allocator {scenario.nextPrescriptionId}";
        if (records.Count > MaxPrescriptions)
            return "too many prescriptions";

        var byId = new Dictionary<int, PrescriptionSaveData>();
        foreach (PrescriptionSaveData record in records)
        {
            string problem = ValidatePrescriptionRecord(record, data, scenario);
            if (problem != null) return problem;
            if (!byId.TryAdd(record.prescriptionId, record))
                return $"prescription {record.prescriptionId} appears twice";
            if (record.prescriptionId >= scenario.nextPrescriptionId)
                return $"prescription {record.prescriptionId} is at or beyond the id allocator {scenario.nextPrescriptionId}";
        }

        if (scenario.workOrders != null)
            foreach (ScenarioOneWorkOrder order in scenario.workOrders)
            {
                if (order == null) continue; // already rejected by the scenario validation
                string problem = ValidateOrderProvenance(order, byId);
                if (problem != null) return problem;
            }
        if (scenario.managementEvents != null)
            foreach (ScenarioManagementEvent entry in scenario.managementEvents)
            {
                if (entry == null || entry.prescriptionId == 0) continue;
                if (!byId.ContainsKey(entry.prescriptionId))
                    return $"management event {entry.eventId} names unknown prescription {entry.prescriptionId}";
            }
        return null;
    }

    private static bool HasPrescriptionLink(ScenarioOneWorkOrder order)
        => order.prescriptionId != 0 || !string.IsNullOrEmpty(order.prescriptionCoverageUnitId)
            || !string.IsNullOrEmpty(order.prescriptionCropTreeId) || order.prescriptionReason != 0;

    private static string ValidatePrescriptionRecord(PrescriptionSaveData record, ForestSaveData data, ScenarioOneSaveData scenario)
    {
        if (record == null)
            return "a prescription record is missing";
        if (record.prescriptionId < 1)
            return $"invalid prescription id {record.prescriptionId}";
        string label = $"prescription {record.prescriptionId}";
        if (!IsVersionText(record.ruleVersion))
            return $"{label} has an invalid rule version";
        if (!IsVersionText(record.calibrationVersion))
            return $"{label} has an invalid calibration version";
        // The source sample is a persisted Rack V2 rack: an id the allocator has handed out.
        if (scenario.rackPlanningModel != RackPlanningModel.RackPlanningFieldMarkingV2)
            return $"{label} needs a Rack Planning V2 world";
        if (record.sourceRackId < 1 || record.sourceRackId >= scenario.nextRackId)
            return $"{label} names an impossible source rack {record.sourceRackId}";
        if (record.sourceYear < 0 || record.sourceYear > data.ecologicalYear)
            return $"{label} has an impossible source year {record.sourceYear}";
        if (!IsFinite(record.sourceAnchorX) || !IsFinite(record.sourceAnchorZ))
            return $"{label} has an invalid source anchor";
        string listProblem = ValidateIdList(record.sampleCropTreeIds, label + " sample Crop Trees")
            ?? ValidateIdList(record.sampleRemovalIds, label + " sample removals");
        if (listProblem != null) return listProblem;

        if (record.coverageDecisions == null)
            return $"{label} is missing its coverage decisions";
        if (record.coverageDecisions.Count > MaxListEntries)
            return $"{label} has too many coverage decisions";
        var units = new HashSet<string>();
        foreach (PrescriptionCoverageDecisionSaveData decision in record.coverageDecisions)
        {
            if (decision == null || !IsIdText(decision.unitId))
                return $"{label} has an invalid coverage unit";
            if (!PrescriptionCoverageChoice.IsKnown(decision.choice))
                return $"{label} has an unknown coverage choice {decision.choice}";
            if (!units.Add(decision.unitId))
                return $"{label} decides coverage unit '{decision.unitId}' twice";
        }

        if (!PrescriptionApprovalState.IsKnown(record.approvalState))
            return $"{label} has an unknown approval state {record.approvalState}";
        if (record.approvalState == PrescriptionApprovalState.Accepted && record.approvalYear != -1)
            return $"{label} is not approved but has an approval year";
        if (record.approvalState == PrescriptionApprovalState.Approved
            && (record.approvalYear < record.sourceYear || record.approvalYear > data.ecologicalYear))
            return $"{label} has an impossible approval year {record.approvalYear}";
        return null;
    }

    private static string ValidateOrderProvenance(ScenarioOneWorkOrder order, Dictionary<int, PrescriptionSaveData> byId)
    {
        if (order.prescriptionId < 0)
            return $"work order {order.workOrderId} has an invalid prescription link";
        if (order.prescriptionId == 0)
            // An ordinary order must carry no provenance at all: nothing is ever attributed after the fact.
            return HasPrescriptionLink(order) ? $"work order {order.workOrderId} has prescription provenance but no prescription" : null;
        if (!byId.TryGetValue(order.prescriptionId, out PrescriptionSaveData owner))
            return $"work order {order.workOrderId} names unknown prescription {order.prescriptionId}";
        if (owner.approvalState != PrescriptionApprovalState.Approved)
            return $"work order {order.workOrderId} comes from prescription {order.prescriptionId}, which is not approved";
        if (order.createdYear < owner.approvalYear)
            return $"work order {order.workOrderId} predates the approval of prescription {order.prescriptionId}";
        if (!IsIdText(order.prescriptionCoverageUnitId))
            return $"work order {order.workOrderId} has an invalid prescription coverage unit";
        if (!string.IsNullOrEmpty(order.prescriptionCropTreeId) && !IsIdText(order.prescriptionCropTreeId))
            return $"work order {order.workOrderId} has an invalid prescription Crop Tree";
        if (!PrescriptionReasonCode.IsKnown(order.prescriptionReason))
            return $"work order {order.workOrderId} has an unknown prescription reason {order.prescriptionReason}";
        return null;
    }

    private static string ValidateIdList(List<string> ids, string label)
    {
        if (ids == null)
            return $"{label} are missing";
        if (ids.Count > MaxListEntries)
            return $"{label} are too many";
        var seen = new HashSet<string>();
        foreach (string id in ids)
            if (!IsIdText(id) || !seen.Add(id))
                return $"{label} hold an invalid or repeated id";
        return null;
    }

    // Printable ASCII, trimmed, bounded: tree ids ("P0009"), coverage unit ids and similar.
    private static bool IsIdText(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length > MaxIdLength || text.Trim() != text)
            return false;
        foreach (char c in text)
            if (c < ' ' || c > '~')
                return false;
        return true;
    }

    // Rule and calibration versions: letters, digits, '.', '_' and '-' only ("MFM-V1-RULE-1").
    private static bool IsVersionText(string text)
    {
        if (string.IsNullOrEmpty(text) || text.Length > MaxIdLength)
            return false;
        foreach (char c in text)
            if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') || c == '.' || c == '_' || c == '-'))
                return false;
        return true;
    }

    // A v23 save must state the prescription fields explicitly with the right types and no duplicate keys.
    public static string ValidatePrescriptionsJson(string json, ForestSaveData data)
    {
        if (data == null || data.version < 23)
            return null;
        try
        {
            var root = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            if (!(root["scenarioOne"] is JObject scenario))
                return null; // no Scenario One state, so no prescription fields are required
            if (scenario["nextPrescriptionId"]?.Type != JTokenType.Integer || !(scenario["prescriptions"] is JArray records))
                return "version 23 requires explicit prescription fields";
            foreach (JToken token in records)
            {
                if (!(token is JObject item)
                    || item["prescriptionId"]?.Type != JTokenType.Integer
                    || item["ruleVersion"]?.Type != JTokenType.String
                    || item["calibrationVersion"]?.Type != JTokenType.String
                    || item["sourceRackId"]?.Type != JTokenType.Integer
                    || item["sourceYear"]?.Type != JTokenType.Integer
                    || !Coordinate(item["sourceAnchorX"]) || !Coordinate(item["sourceAnchorZ"])
                    || !IsStringArray(item["sampleCropTreeIds"]) || !IsStringArray(item["sampleRemovalIds"])
                    || !(item["coverageDecisions"] is JArray decisions)
                    || item["approvalState"]?.Type != JTokenType.Integer
                    || item["approvalYear"]?.Type != JTokenType.Integer)
                    return "a version 23 prescription record is missing required fields";
                foreach (JToken decisionToken in decisions)
                    if (!(decisionToken is JObject decision)
                        || decision["unitId"]?.Type != JTokenType.String
                        || decision["choice"]?.Type != JTokenType.Integer)
                        return "a version 23 coverage decision is missing required fields";
            }
            if (scenario["workOrders"] is JArray orders)
                foreach (JToken token in orders)
                    if (!(token is JObject order)
                        || order["prescriptionId"]?.Type != JTokenType.Integer
                        || order["prescriptionCoverageUnitId"]?.Type != JTokenType.String
                        || order["prescriptionCropTreeId"]?.Type != JTokenType.String
                        || order["prescriptionReason"]?.Type != JTokenType.Integer)
                        return "a version 23 work order is missing its prescription link";
            if (scenario["managementEvents"] is JArray events)
                foreach (JToken token in events)
                    if (!(token is JObject entry) || entry["prescriptionId"]?.Type != JTokenType.Integer)
                        return "a version 23 management event is missing its prescription link";
        }
        catch (System.Exception)
        {
            return "invalid version 23 prescription JSON";
        }
        return null;
    }

    private static bool IsStringArray(JToken token)
    {
        if (!(token is JArray array)) return false;
        foreach (JToken item in array)
            if (item.Type != JTokenType.String) return false;
        return true;
    }
}
