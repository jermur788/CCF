using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Deterministic reference-run driver. Executes an authored management schedule
// through the real Scenario One work-order, approval, settlement and ecology
// pipeline. Never constructs forest state directly.
public static class ScenarioReferenceRunner
{
    public static ScenarioReferenceSchedule BuildSchedule()
    {
        TextAsset authored = Resources.Load<TextAsset>("ScenarioOneReferenceScheduleV1");
        if (authored != null)
        {
            ScenarioReferenceSchedule loaded = JsonUtility.FromJson<ScenarioReferenceSchedule>(authored.text);
            if (loaded != null && loaded.directives != null && loaded.directives.Count > 0)
                return loaded;
            throw new InvalidOperationException("Authored Scenario One reference schedule is unreadable.");
        }
        // This structured authoring draft generates the first verified asset.
        // Once frozen, the Resources JSON above is the reproducible v1 source.
        var schedule = new ScenarioReferenceSchedule();
        void Add(int year, ReferenceActionType action, ReferenceTargetingRule targeting,
            float baFraction, int count, float deadwoodFraction, string speciesId, string note)
        {
            schedule.directives.Add(new ScenarioReferenceDirective
            {
                year = year, action = action, targeting = targeting,
                baFraction = baFraction, count = count,
                deadwoodFraction = deadwoodFraction, speciesId = speciesId, note = note
            });
        }

        // Year 0: survey only, then first thinning executes in year 1.
        Add(0, ReferenceActionType.Survey, ReferenceTargetingRule.FutureTreeRelease, 0f, 0, 0f, "",
            "Survey stand; identify retained future Sitka and broadleaf establishment areas.");
        Add(0, ReferenceActionType.Thin, ReferenceTargetingRule.FutureTreeRelease, 0.15f, 0, 0.10f, "",
            "First selective thinning: ~15% BA around future trees; retain 10% as deadwood.");

        // Year 5: second thinning and first broadleaf planting.
        Add(4, ReferenceActionType.Thin, ReferenceTargetingRule.FutureTreeRelease, 0.13f, 0, 0.10f, "",
            "Second selective thinning: ~13% BA; several moderate openings.");
        Add(4, ReferenceActionType.Plant, ReferenceTargetingRule.BrightestCells, 0f, 12, 0f, "sessile-oak",
            "Plant Sessile Oak into brighter suitable openings.");
        Add(4, ReferenceActionType.Plant, ReferenceTargetingRule.PartialShadeCells, 0f, 10, 0f, "beech",
            "Plant Beech mainly in partial shade and gap edges.");

        // Year 10: third lighter thinning and targeted regeneration control.
        Add(9, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.11f, 0, 0.15f, "",
            "Third lighter thinning: ~11% BA around regeneration and future trees.");
        Add(9, ReferenceActionType.RemoveRegeneration, ReferenceTargetingRule.CompetingSitkaRegen, 0f, 5, 0f, "sitka-spruce",
            "Remove Sitka regeneration only where it crowds priority broadleaf patches.");

        // Years 10-15: replace failed broadleaf pockets only.
        Add(13, ReferenceActionType.ReplaceFailedPlanting, ReferenceTargetingRule.FailedBroadleafPockets, 0f, 5, 0f, "",
            "Replace failed broadleaf pockets where conditions remain suitable.");

        // Year 15: fourth selective thinning.
        Add(14, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.10f, 0, 0.20f, "",
            "Fourth selective thinning: ~10% BA; release established Oak/Beech; retain deadwood.");

        // Year 20: management transition point — no intervention.
        Add(19, ReferenceActionType.LowIntervention, ReferenceTargetingRule.FutureTreeRelease, 0f, 0, 0f, "",
            "Transition from even-aged crop to individual-tree/cohort management.");

        // Year 25: release and prune selected timber stems.
        Add(24, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.05f, 0, 0.15f, "",
            "Remove small number of immediate competitors around promising broadleaf and Sitka.");
        Add(24, ReferenceActionType.Prune, ReferenceTargetingRule.BestTimberStems, 0f, 10, 0f, "",
            "Prune selected worthwhile timber stems.");

        // Year 30: regeneration management or low-intervention year.
        Add(29, ReferenceActionType.RemoveRegeneration, ReferenceTargetingRule.CompetingSitkaRegen, 0f, 3, 0f, "sitka-spruce",
            "Selectively remove excessive Sitka regeneration threatening mixed composition.");

        // Year 35: light selective harvest where canopy reclosed.
        Add(34, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.09f, 0, 0.15f, "",
            "Light selective harvest: ~9% BA where canopy reclosed; retain large structural Sitka.");

        // Year 40: deadwood-focused intervention.
        Add(39, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.08f, 0, 0.50f, "",
            "Deadwood-focused intervention: retain larger proportion; do not maximize extraction.");

        // Years 45-50: pruning and very limited planting.
        Add(44, ReferenceActionType.Prune, ReferenceTargetingRule.BestTimberStems, 0f, 8, 0f, "",
            "Selective pruning / release of best younger timber stems.");
        Add(49, ReferenceActionType.ReplaceFailedPlanting, ReferenceTargetingRule.FailedBroadleafPockets, 0f, 3, 0f, "",
            "Very limited planting: only where regeneration and previous planting have failed.");

        // Year 55: generational turnover without canopy reset.
        Add(54, ReferenceActionType.Thin, ReferenceTargetingRule.OverDominantSitka, 0f, 8, 0.25f, "",
            "Remove limited mature/over-dominant Sitka where younger cohorts can occupy space.");
        Add(54, ReferenceActionType.RemoveRegeneration, ReferenceTargetingRule.CompetingSitkaRegen, 0f, 5, 0f, "sitka-spruce",
            "Keep selected broadleaf parent patches free of immediate Sitka regeneration competition.");

        // Years 60-70: adaptive/light management.
        Add(64, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.05f, 0, 0.30f, "",
            "Adaptive light management: intervene only where local closure justifies.");
        Add(64, ReferenceActionType.RemoveRegeneration, ReferenceTargetingRule.CompetingSitkaRegen, 0f, 6, 0f, "sitka-spruce",
            "Continue selective regeneration removal.");
        Add(69, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.04f, 0, 0.30f, "",
            "Respond to canopy closure around mixed-age regeneration patches.");

        // Year 75: limited older canopy removal.
        Add(74, ReferenceActionType.Thin, ReferenceTargetingRule.OverDominantSitka, 0.05f, 6, 0.35f, "",
            "Remove limited older canopy trees where younger cohorts are ready; retain legacy trees.");

        // Years 80-90: light-touch interventions.
        Add(84, ReferenceActionType.Prune, ReferenceTargetingRule.BestTimberStems, 0f, 5, 0f, "",
            "Selected pruning of remaining worthwhile stems.");
        Add(84, ReferenceActionType.RemoveRegeneration, ReferenceTargetingRule.CompetingSitkaRegen, 0f, 2, 0f, "sitka-spruce",
            "Light regeneration management.");
        Add(84, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.03f, 0, 0.30f, "",
            "Renew a few lighter cells without resetting the canopy.");
        Add(89, ReferenceActionType.Thin, ReferenceTargetingRule.BroadleafRelease, 0.03f, 0, 0.30f, "",
            "Light-touch release; no major canopy disturbance.");

        // Year 100: no final cosmetic treatment.
        Add(99, ReferenceActionType.LowIntervention, ReferenceTargetingRule.FutureTreeRelease, 0f, 0, 0f, "",
            "No final cosmetic treatment; record resulting forest as Reference Future v1.");
        return schedule;
    }

    public static ScenarioReferenceSurvey Survey(ScenarioOneManager manager,
        ForestEcologyController ecology, int stride)
    {
        var survey = new ScenarioReferenceSurvey { surveyedYear = ecology.EcologicalYear };
        List<ForestTree> trees = AllLivingTrees();
        trees.Sort((a, b) => string.CompareOrdinal(a.TreeId, b.TreeId));
        for (int i = 0; i < trees.Count; i += Mathf.Max(1, stride))
            survey.futureTreeIds.Add(trees[i].TreeId);

        // Candidate broadleaf cells: rank by current light; Oak prefers brighter,
        // Beech prefers partial shade. Recorded for planning; planting decisions
        // at later years re-check current conditions.
        List<(int index, float light)> ranked = new List<(int, float)>();
        for (int i = 0; i < ecology.CellCount; i++)
            ranked.Add((i, ecology.Cells[i].Light));
        ranked.Sort(CompareBright);
        survey.oakCandidateCells = ranked.Take(16).Select(entry => entry.index).ToList();
        survey.beechCandidateCells = ranked.Skip(8).Take(16).Select(entry => entry.index).ToList();
        return survey;
    }

    // Executes all directives whose creation year matches the current ecological
    // year, then the caller advances the year normally.
    public static int ExecuteYear(ScenarioOneManager manager, ForestEcologyController ecology,
        ForestTreeMarkingManager marking, ScenarioReferenceSchedule schedule,
        ScenarioReferenceSurvey survey, int currentYear)
    {
        int executed = 0;
        foreach (ScenarioReferenceDirective directive in schedule.directives)
        {
            if (directive.year != currentYear)
                continue;
            switch (directive.action)
            {
                case ReferenceActionType.Survey:
                    // Survey is setup; already performed before the run loop.
                    break;
                case ReferenceActionType.Thin:
                    executed += ExecuteThinning(manager, ecology, marking, directive, survey) ? 1 : 0;
                    break;
                case ReferenceActionType.Plant:
                    executed += ExecutePlanting(manager, ecology, directive, survey) ? 1 : 0;
                    break;
                case ReferenceActionType.RemoveRegeneration:
                    executed += ExecuteRegenerationRemoval(manager, ecology, directive, survey) ? 1 : 0;
                    break;
                case ReferenceActionType.Prune:
                    executed += ExecutePruning(manager, ecology, directive, survey) ? 1 : 0;
                    break;
                case ReferenceActionType.ReplaceFailedPlanting:
                    executed += ExecuteReplaceFailed(manager, ecology, directive, survey) ? 1 : 0;
                    break;
                case ReferenceActionType.LowIntervention:
                    break;
            }
        }
        return executed;
    }

    // --- Thinning -------------------------------------------------------
    private static bool ExecuteThinning(ScenarioOneManager manager, ForestEcologyController ecology,
        ForestTreeMarkingManager marking, ScenarioReferenceDirective directive, ScenarioReferenceSurvey survey)
    {
        List<ForestTree> all = AllLivingTrees();
        if (all.Count == 0)
            return false;
        List<ForestTree> targets;
        switch (directive.targeting)
        {
            case ReferenceTargetingRule.OverDominantSitka:
                targets = SelectOverDominant(all, directive.count);
                break;
            case ReferenceTargetingRule.BroadleafRelease:
                targets = SelectBroadleafRelease(all, survey.futureTreeIds, ecology, directive.baFraction);
                break;
            default:
                targets = SelectThinningTargets(all, survey.futureTreeIds, directive.baFraction);
                break;
        }
        if (targets.Count == 0)
            return false;

        int retained = Mathf.RoundToInt(targets.Count * Mathf.Clamp01(directive.deadwoodFraction));
        float basalAreaBefore = all.Sum(BasalArea);
        Debug.Log($"REFERENCE_THIN year={ecology.EcologicalYear + 1} targeting={directive.targeting} "
            + $"trees={targets.Count} baFraction={targets.Sum(BasalArea) / basalAreaBefore:0.000} "
            + $"target={directive.baFraction:0.000} deadwood={retained}");

        // First batch: retained as deadwood.
        if (retained > 0)
        {
            manager.PlanningFellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
            marking.ClearAll();
            for (int i = 0; i < retained && i < targets.Count; i++)
                marking.Mark(targets[i], TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
            marking.ClearAll();
        }

        // Second batch: sold and extracted.
        int remaining = targets.Count - retained;
        if (remaining > 0)
        {
            manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
            marking.ClearAll();
            for (int i = retained; i < targets.Count; i++)
                marking.Mark(targets[i], TreeMarkType.Fell, false);
            manager.AddMarkedTreesToWorkPlan();
            marking.ClearAll();
        }
        return true;
    }

    private static List<ForestTree> SelectThinningTargets(List<ForestTree> all,
        HashSet<string> futureTreeIds, float baFraction)
    {
        List<ForestTree> future = all.Where(t => futureTreeIds.Contains(t.TreeId)).ToList();
        List<ForestTree> candidates = all.Where(t => t.CanChop && !futureTreeIds.Contains(t.TreeId)).ToList();
        if (candidates.Count == 0)
            return new List<ForestTree>();

        // Concentrate thinning around retained future trees: closest non-future
        // trees are removed first, creating small release openings.
        var distance = new Dictionary<string, float>(StringComparer.Ordinal);
        foreach (ForestTree candidate in candidates)
        {
            float best = float.MaxValue;
            Vector3 cPos = candidate.transform.position;
            foreach (ForestTree f in future)
            {
                float d = Vector2.Distance(new Vector2(cPos.x, cPos.z),
                    new Vector2(f.transform.position.x, f.transform.position.z));
                if (d < best) best = d;
            }
            distance[candidate.TreeId] = best;
        }
        candidates.Sort((a, b) =>
        {
            int cmp = distance[a.TreeId].CompareTo(distance[b.TreeId]);
            return cmp != 0 ? cmp : string.CompareOrdinal(a.TreeId, b.TreeId);
        });

        float totalBA = all.Sum(BasalArea);
        float targetBA = totalBA * Mathf.Clamp01(baFraction);
        float accumulated = 0f;
        var selected = new List<ForestTree>();
        foreach (ForestTree tree in candidates)
        {
            if (accumulated >= targetBA)
                break;
            selected.Add(tree);
            accumulated += BasalArea(tree);
        }
        return selected;
    }

    private static List<ForestTree> SelectOverDominant(List<ForestTree> all, int count)
    {
        // Remove largest stems only where younger cohorts are nearby to occupy
        // the space; retain the rest as legacy structure.
        List<ForestTree> candidates = all.Where(t => t.CanChop
            && t.Species?.SpeciesId == "sitka-spruce").ToList();
        var eligible = new List<ForestTree>();
        foreach (ForestTree tree in candidates)
        {
            Vector3 pos = tree.transform.position;
            bool hasYounger = candidates.Any(other =>
            {
                if (other == tree || other.AgeYears >= tree.AgeYears - 10)
                    return false;
                Vector3 oPos = other.transform.position;
                return Vector2.Distance(new Vector2(pos.x, pos.z), new Vector2(oPos.x, oPos.z)) < 10f;
            });
            if (hasYounger)
                eligible.Add(tree);
        }
        return eligible.OrderByDescending(t => t.Diameter)
            .ThenBy(t => t.TreeId, StringComparer.Ordinal)
            .Take(Mathf.Max(0, count)).ToList();
    }

    private static List<ForestTree> SelectBroadleafRelease(List<ForestTree> all,
        HashSet<string> futureTreeIds, ForestEcologyController ecology, float baFraction)
    {
        var priorities = new List<Vector3>();
        foreach (ForestTree tree in all)
            if (tree.Species?.SpeciesId == "sessile-oak" || tree.Species?.SpeciesId == "beech")
                priorities.Add(tree.transform.position);
        for (int i = 0; i < ecology.CellCount; i++)
            if (ecology.Cells[i].Regeneration.Any(cohort => cohort != null && cohort.Density > 0f
                    && (cohort.SpeciesId == "sessile-oak" || cohort.SpeciesId == "beech")))
            {
                Vector2 center = ecology.Cells[i].Center;
                priorities.Add(new Vector3(center.x, 0f, center.y));
            }
        if (priorities.Count == 0)
            return SelectThinningTargets(all, futureTreeIds, baFraction);

        var candidates = new List<(ForestTree tree, float distance)>();
        foreach (ForestTree tree in all)
        {
            if (!tree.CanChop || tree.Species?.SpeciesId != "sitka-spruce"
                || futureTreeIds.Contains(tree.TreeId))
                continue;
            int index = ecology.GetCellIndex(tree.transform.position);
            if (index < 0 || ecology.Cells[index].Light >= 0.5f)
                continue;
            float nearest = float.MaxValue;
            foreach (Vector3 target in priorities)
            {
                float distance = Vector2.Distance(new Vector2(tree.transform.position.x, tree.transform.position.z),
                    new Vector2(target.x, target.z));
                if (distance < nearest) nearest = distance;
            }
            if (nearest <= 11f)
                candidates.Add((tree, nearest));
        }
        candidates.Sort((a, b) =>
        {
            int cmp = a.distance.CompareTo(b.distance);
            return cmp != 0 ? cmp : string.CompareOrdinal(a.tree.TreeId, b.tree.TreeId);
        });
        float targetBa = all.Sum(BasalArea) * Mathf.Clamp01(baFraction);
        float removedBa = 0f;
        var selected = new List<ForestTree>();
        foreach (var candidate in candidates)
        {
            if (removedBa >= targetBa) break;
            selected.Add(candidate.tree);
            removedBa += BasalArea(candidate.tree);
        }
        return selected;
    }

    // --- Planting -------------------------------------------------------
    private static bool ExecutePlanting(ScenarioOneManager manager, ForestEcologyController ecology,
        ScenarioReferenceDirective directive, ScenarioReferenceSurvey survey)
    {
        if (string.IsNullOrEmpty(directive.speciesId))
            return false;
        string itemId = SpeciesToItemId(manager, directive.speciesId);
        if (manager.Definition.FindShopEntry(itemId) == null)
            throw new InvalidOperationException("Reference nursery item missing for " + directive.speciesId);
        int available = manager.GetStockQuantity(itemId) - manager.GetReservedStockQuantity(itemId);
        if (available < directive.count && !manager.TryPurchaseStock(itemId, directive.count - available))
            throw new InvalidOperationException("Could not purchase " + itemId + " for reference planting.");
        // Re-rank by current light so planting follows actual canopy openings.
        List<(int index, float light)> ranked = new List<(int, float)>();
        for (int i = 0; i < ecology.CellCount; i++)
            ranked.Add((i, ecology.Cells[i].Light));
        ranked.Sort((a, b) => directive.targeting == ReferenceTargetingRule.BrightestCells
            ? CompareBright(a, b)
            : ComparePartialShade(a, b));

        // First pass: plant in cells with no regeneration (no capacity conflict).
        // Second pass: clear competing Sitka then plant in the same cell.
        int planted = 0;
        var deferred = new List<int>();
        foreach ((int index, float light) in ranked)
        {
            if (planted >= directive.count)
                break;
            ForestRegenerationCohort existing = ecology.Cells[index].FindCohort(directive.speciesId);
            if (existing != null && existing.Density > 0f)
                continue;
            bool suitable = directive.targeting == ReferenceTargetingRule.BrightestCells
                ? light >= 0.12f : light >= 0.08f;
            if (!suitable)
                continue;
            bool hasSitka = ecology.Cells[index].FindCohort("sitka-spruce") is { Density: > 0.1f };
            if (!hasSitka)
            {
                if (manager.TryDesignatePlanting(itemId, index))
                {
                    planted++;
                    RecordPlantingCell(survey, directive.speciesId, index);
                }
            }
            else
                deferred.Add(index);
        }

        // Second pass: clear Sitka in deferred cells, then plant.
        foreach (int index in deferred)
        {
            if (planted >= directive.count)
                break;
            ForestRegenerationCohort sitka = ecology.Cells[index].FindCohort("sitka-spruce");
            if (sitka != null && sitka.Density > 0f)
                manager.TryDesignateRegenerationRemoval("sitka-spruce", index);
            if (manager.TryDesignatePlanting(itemId, index))
            {
                planted++;
                RecordPlantingCell(survey, directive.speciesId, index);
            }
        }
        Debug.Log($"REFERENCE_PLANT year={ecology.EcologicalYear} species={directive.speciesId} "
            + $"designated={planted}/{directive.count} lightMin={ranked.Min(entry => entry.light):0.000} "
            + $"lightMax={ranked.Max(entry => entry.light):0.000}");
        return planted > 0;
    }

    // --- Regeneration removal -------------------------------------------
    private static bool ExecuteRegenerationRemoval(ScenarioOneManager manager,
        ForestEcologyController ecology, ScenarioReferenceDirective directive, ScenarioReferenceSurvey survey)
    {
        // Remove selected Sitka cohorts sharing or adjoining live broadleaf
        // cohorts and recruited trees. After promotion the cohort disappears,
        // but the established broadleaf still deserves its local light patch.
        var priorityCells = new HashSet<int>();
        for (int i = 0; i < ecology.CellCount; i++)
            if (ecology.Cells[i].Regeneration.Any(cohort => cohort != null && cohort.Density > 0f
                && (cohort.SpeciesId == "beech" || cohort.SpeciesId == "sessile-oak")))
                priorityCells.Add(i);
        foreach (ForestTree tree in AllLivingTrees())
            if (tree.Species?.SpeciesId == "beech" || tree.Species?.SpeciesId == "sessile-oak")
            {
                int index = ecology.GetCellIndex(tree.transform.position);
                if (index >= 0) priorityCells.Add(index);
            }
        int axis = ecology.CellsPerAxis;
        List<int> candidates = Enumerable.Range(0, ecology.CellCount)
            .Where(index => ecology.Cells[index].FindCohort(directive.speciesId) is { Density: > 0f })
            .Where(index => priorityCells.Any(priority =>
                Mathf.Abs(priority % axis - index % axis) <= 1
                && Mathf.Abs(priority / axis - index / axis) <= 1))
            .OrderBy(index => priorityCells.Contains(index) ? 0 : 1)
            .ThenByDescending(index => ecology.Cells[index].FindCohort(directive.speciesId).Density)
            .ThenBy(index => index)
            .ToList();
        int removed = 0;
        foreach (int index in candidates)
        {
            if (removed >= directive.count) break;
            if (manager.TryDesignateRegenerationRemoval(directive.speciesId, index))
                removed++;
        }
        return removed > 0;
    }

    // --- Pruning --------------------------------------------------------
    private static bool ExecutePruning(ScenarioOneManager manager, ForestEcologyController ecology,
        ScenarioReferenceDirective directive, ScenarioReferenceSurvey survey)
    {
        // Prune selected worthwhile timber stems: larger living trees with
        // available pruning lifts that are not scheduled for felling this year.
        var plannedFellingIds = new HashSet<string>(manager.WorkOrders
            .Where(order => order.type == ScenarioWorkType.FellTree && order.IsOpen)
            .Select(order => order.targetTreeId), StringComparer.Ordinal);
        List<ForestTree> candidates = AllLivingTrees()
            .Where(t => t.CanChop && manager.Definition.NextPruningTargetHeightM(t.PruningLifts) > 0f)
            .Where(t => !plannedFellingIds.Contains(t.TreeId))
            .OrderByDescending(t => t.Diameter)
            .ThenBy(t => t.TreeId, StringComparer.Ordinal)
            .ToList();
        int pruned = 0;
        foreach (ForestTree tree in candidates)
        {
            if (pruned >= directive.count)
                break;
            if (manager.TryDesignatePruning(tree.TreeId))
                pruned++;
        }
        return pruned > 0;
    }

    // --- Replace failed broadleaf ---------------------------------------
    private static bool ExecuteReplaceFailed(ScenarioOneManager manager,
        ForestEcologyController ecology, ScenarioReferenceDirective directive, ScenarioReferenceSurvey survey)
    {
        // Check previously planted broadleaf cells; replant where the cohort is
        // missing or density has collapsed. Deterministic and conditional.
        int replaced = 0;
        foreach (string speciesId in new[] { "beech", "sessile-oak" })
        {
            string itemId = SpeciesToItemId(manager, speciesId);
            foreach (int cellIndex in (speciesId == "beech"
                         ? survey.beechPlantedCells : survey.oakPlantedCells).ToList())
            {
                if (replaced >= directive.count)
                    break;
                ForestRegenerationCohort cohort = ecology.Cells[cellIndex].FindCohort(speciesId);
                bool failed = cohort == null || cohort.Density <= 0.05f;
                if (!failed)
                    continue;
                if (AllLivingTrees().Any(tree => tree.Species?.SpeciesId == speciesId
                    && ecology.GetCellIndex(tree.transform.position) == cellIndex))
                    continue; // successful promotion is not a failed planting
                if (ecology.Cells[cellIndex].Light < (speciesId == "sessile-oak" ? 0.12f : 0.08f))
                    continue;
                if (manager.GetStockQuantity(itemId) <= manager.GetReservedStockQuantity(itemId)
                    && !manager.TryPurchaseStock(itemId, 1))
                    continue;
                if (manager.TryDesignatePlanting(itemId, cellIndex))
                    replaced++;
            }
        }
        return replaced > 0;
    }

    // --- Helpers --------------------------------------------------------
    private static string SpeciesToItemId(ScenarioOneManager manager, string speciesId)
    {
        if (manager?.Definition?.ShopEntries == null)
            return speciesId;
        foreach (ScenarioShopEntry offer in manager.Definition.ShopEntries)
            if (offer != null && offer.speciesId == speciesId)
                return offer.itemId;
        return speciesId;
    }

    private static void RecordPlantingCell(ScenarioReferenceSurvey survey, string speciesId, int cellIndex)
    {
        List<int> planted = speciesId == "beech" ? survey.beechPlantedCells : survey.oakPlantedCells;
        if (!planted.Contains(cellIndex))
            planted.Add(cellIndex);
    }

    private static int CompareBright((int index, float light) first, (int index, float light) second)
    {
        int comparison = second.light.CompareTo(first.light);
        return comparison != 0 ? comparison : first.index.CompareTo(second.index);
    }

    private static int ComparePartialShade((int index, float light) first, (int index, float light) second)
    {
        int comparison = Mathf.Abs(first.light - 0.28f).CompareTo(Mathf.Abs(second.light - 0.28f));
        return comparison != 0 ? comparison : first.index.CompareTo(second.index);
    }

    private static List<ForestTree> AllLivingTrees()
    {
        return UnityEngine.Object.FindObjectsByType<ForestTree>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => t != null && !t.IsStump && !string.IsNullOrEmpty(t.TreeId))
            .ToList();
    }

    private static float BasalArea(ForestTree tree)
    {
        return Mathf.PI * Mathf.Pow(tree.Diameter / 200f, 2f);
    }
}
