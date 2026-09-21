using System.Collections.Generic;
using UnityEngine;

// Explicit scenario configurations for the starting stand. The playable
// scenario is the canonical first-thinning plantation; the lifecycle fixture
// preserves the old 68-mature-tree stand purely as a deterministic test
// baseline for the multi-decade reproduction experiment. None of these
// numbers are universal Sitka constants - they are scenario settings.
public static class ForestStandScenarios
{
    // The pre-rebuild demonstration stand (68 mature trees), recorded from the
    // scene before the first-thinning rebuild. Format: id|x,z|age|dbh|height|crown.
    // Kept so the 80-year lifecycle experiment can always run against its
    // original baseline (mature reproductive parents) regardless of the
    // playable starting scenario.
    public const string LifecycleFixture =
        "T47|-15.98,8.03|45|66|5.52|1.67\n" +
        "T49|-5.44,-0.14|45|84|6.16|2.09\n" +
        "T56|16.85,-6.50|45|56|5.84|1.81\n" +
        "T59|-15.41,-11.23|45|59|5.70|1.57\n" +
        "T46|-6.70,-9.20|45|51|4.65|1.81\n" +
        "T24|14.42,12.48|45|51|5.85|1.55\n" +
        "T36|10.59,7.66|45|51|5.18|1.75\n" +
        "T48|-0.35,-6.79|45|76|5.19|1.93\n" +
        "T44|-13.22,15.02|45|66|6.40|1.94\n" +
        "T31|-16.32,-5.97|45|76|6.54|1.81\n" +
        "T03|11.87,3.43|45|51|6.42|1.85\n" +
        "T22|-9.08,-11.14|45|84|5.63|2.06\n" +
        "T34|-2.03,-9.71|45|65|6.61|1.88\n" +
        "T23|4.74,9.65|45|78|4.55|1.58\n" +
        "T27|-2.02,2.10|45|55|4.65|1.84\n" +
        "T13|10.72,15.99|45|82|6.09|1.60\n" +
        "T42|-0.81,-0.71|45|61|5.82|1.97\n" +
        "T26|6.64,-13.99|45|81|6.63|1.90\n" +
        "T21|-10.05,6.43|45|63|4.83|1.97\n" +
        "T15|8.06,-16.49|45|54|6.15|1.90\n" +
        "T35|13.53,16.55|45|82|5.47|1.74\n" +
        "T25|-8.52,15.60|45|69|5.30|1.94\n" +
        "T39|8.44,-8.85|45|69|5.33|1.83\n" +
        "T54|-5.37,-13.35|45|80|4.66|1.97\n" +
        "T45|-8.10,-16.40|45|68|6.61|1.60\n" +
        "T14|-6.50,4.72|45|60|4.51|1.60\n" +
        "T68|-8.62,-3.79|45|51|5.78|1.61\n" +
        "T28|-16.17,16.36|45|51|6.48|1.66\n" +
        "T10|13.27,-15.25|45|51|5.98|2.01\n" +
        "T29|-0.48,-3.81|45|53|6.66|1.72\n" +
        "T19|-8.70,2.12|45|70|6.93|1.50\n" +
        "T55|-11.31,12.33|45|64|5.85|1.92\n" +
        "T05|4.01,-14.95|45|70|6.89|1.91\n" +
        "T07|14.66,0.37|45|71|5.58|1.59\n" +
        "T37|-10.59,-6.63|45|74|6.20|1.69\n" +
        "T57|16.19,9.91|45|62|6.51|1.60\n" +
        "T43|-12.28,-3.58|45|79|4.59|1.72\n" +
        "T58|15.96,-16.20|45|68|6.24|1.73\n" +
        "T38|-13.72,5.46|45|54|5.10|1.64\n" +
        "T16|5.38,1.14|45|71|5.00|2.01\n" +
        "T64|8.69,-1.68|45|76|5.40|2.01\n" +
        "T60|4.82,14.33|45|74|5.69|1.51\n" +
        "T18|-1.32,-16.66|45|52|6.92|2.08\n" +
        "T65|13.16,-4.57|45|75|4.76|1.62\n" +
        "T62|12.05,-1.30|45|69|5.71|1.93\n" +
        "T63|4.37,-11.00|45|83|6.51|1.57\n" +
        "T06|-16.75,0.44|45|72|5.91|1.53\n" +
        "T02|6.67,16.72|45|57|5.39|1.51\n" +
        "T08|13.49,6.23|45|61|6.93|1.65\n" +
        "T17|-13.09,9.69|45|80|5.79|1.76\n" +
        "T50|14.87,-9.28|45|76|5.17|2.08\n" +
        "T32|3.05,16.63|45|78|6.00|1.56\n" +
        "T33|-15.28,12.21|45|70|6.88|1.85\n" +
        "T66|6.51,-5.08|45|65|5.60|1.62\n" +
        "T41|-10.16,9.72|45|80|5.26|1.62\n" +
        "T04|16.90,-2.95|45|82|5.38|2.02\n" +
        "T30|7.74,11.65|45|84|5.97|1.86\n" +
        "T67|-16.53,-3.04|45|82|6.36|1.89\n" +
        "T40|-13.06,-13.76|45|72|5.07|2.02\n" +
        "T61|6.02,5.76|45|84|6.67|1.71\n" +
        "T53|10.87,-13.56|45|56|5.56|1.93\n" +
        "T11|-3.88,-4.86|45|53|5.33|1.84\n" +
        "T52|9.42,-5.92|45|80|5.92|1.54\n" +
        "T09|12.02,-10.59|45|57|5.60|1.68\n" +
        "T20|-12.57,1.27|45|59|4.86|1.59\n" +
        "T51|-11.64,-9.82|45|62|4.91|2.03\n" +
        "T01|16.62,6.73|45|76|4.87|1.99\n" +
        "T12|-15.04,-16.23|45|50|5.05|1.88\n";

    public static void ApplyLifecycleFixture()
    {
        var spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
        var ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        if (spawner == null || ecology == null)
        {
            Debug.LogError("Lifecycle fixture needs a spawner and an ecology controller.");
            return;
        }
        ClearAllTrees();
        // Complete reset: the fixture must not inherit year, mast, cells,
        // regeneration, opening or cache state from whatever ran before it,
        // or a same-session re-apply diverges from a fresh run.
        ecology.ResetForDeterministicRun();
        foreach (string row in LifecycleFixture.Split('\n'))
        {
            if (string.IsNullOrWhiteSpace(row))
                continue;
            string[] parts = row.Split('|');
            if (parts.Length < 6)
                continue;
            string[] coords = parts[1].Split(',');
            Vector3 position = new Vector3(float.Parse(coords[0]), 0f, float.Parse(coords[1]));
            spawner.Spawn(parts[0], spawner.DefaultSpecies, position, int.Parse(parts[2]), float.Parse(parts[3]),
                float.Parse(parts[4]), float.Parse(parts[5]));
        }
        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();
        Debug.Log($"LIFECYCLE_FIXTURE applied: {ecology.LivingTreeCount} mature trees");
    }

    private static void ClearAllTrees()
    {
        // DestroyImmediate so a same-frame rebuild (fixture -> spawn -> run)
        // never simulates against doomed-but-still-alive trees.
        foreach (var tree in Object.FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Object.DestroyImmediate(tree.gameObject);
    }
}
