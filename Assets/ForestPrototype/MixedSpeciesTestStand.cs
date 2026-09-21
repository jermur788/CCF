using UnityEngine;

// Test-scene-only authoring component. ForestStartingStand creates the normal
// 336-tree Sitka crop first; this adds a small explicit Beech group without
// changing the canonical ForestTest starting scenario or enabling Beech regen.
public sealed class MixedSpeciesTestStand : MonoBehaviour
{
    [SerializeField] private TreeSpeciesDefinition beechSpecies;
    [SerializeField, Min(1)] private int beechCount = 4;

    private void Start()
    {
        ForestTreeSpawner spawner = FindFirstObjectByType<ForestTreeSpawner>();
        if (spawner == null || beechSpecies == null)
        {
            Debug.LogError("MixedSpeciesTestStand is missing its spawner or Beech species.", this);
            return;
        }

        for (int i = 0; i < beechCount; i++)
        {
            string id = "B-Mixed-" + i.ToString("00");
            if (FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Length > 0 &&
                FindTree(id) != null)
                continue;
            Vector3 position = BeechPosition(i);
            float dbh = 16f + (i % 2) * 2f;
            float height = 10f;
            spawner.Spawn(id, beechSpecies, position, 35, dbh, height, beechSpecies.PotentialCrownRadiusM(dbh));
        }
    }

    private static ForestTree FindTree(string id)
    {
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (tree != null && tree.TreeId == id)
                return tree;
        return null;
    }

    private static Vector3 BeechPosition(int index)
    {
        Vector3[] positions =
        {
            new Vector3(-10f, 0f, -10f),
            new Vector3(-2f, 0f, -10f),
            new Vector3(6f, 0f, -10f),
            new Vector3(10f, 0f, -6f)
        };
        return positions[Mathf.Clamp(index, 0, positions.Length - 1)];
    }
}
