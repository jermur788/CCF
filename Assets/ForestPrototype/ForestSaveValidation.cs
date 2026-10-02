using System.Collections.Generic;

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
            }
        }

        return null;
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
