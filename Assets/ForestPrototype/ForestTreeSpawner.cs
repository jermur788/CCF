using UnityEngine;

// Simplest maintainable spawning path for naturally recruited trees: the same
// factory is used by the ecology controller and by save loading, so ecological
// logic never depends on how the placeholder tree is built.
public sealed class ForestTreeSpawner : MonoBehaviour
{
    [SerializeField] private TreeSpeciesDefinition defaultSpecies;
    [SerializeField] private Material barkMaterial;
    [SerializeField] private GameObject canopyPrefab;
    [SerializeField] private Transform forestParent;
    // Optional polished visuals handed to spawned trees. visualPrefab is the
    // default; visualPrefabAlternative gives the stand a second mature look,
    // picked deterministically from the persistent tree id so a given tree
    // always renders the same model across sessions and save/load.
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private GameObject visualPrefabAlternative;
    [SerializeField] private GameObject stumpPrefab;

    public TreeSpeciesDefinition DefaultSpecies => defaultSpecies;

    // Stable 50/50 split between the two visuals, keyed by the tree id
    // (FNV-1a, character order — identical on every machine and session).
    public static GameObject PickVisual(string treeId, GameObject first, GameObject second)
    {
        if (string.IsNullOrEmpty(treeId) || second == null)
            return first != null ? first : second;
        uint hash = 2166136261u;
        foreach (char c in treeId)
        {
            hash ^= c;
            hash *= 16777619u;
        }
        return (hash & 1u) == 0u ? first : second;
    }

    public ForestTree Spawn(string treeId, Vector3 groundPosition, int ageYears, float dbhCm, float heightMeters, float crownRadiusMeters)
    {
        if (defaultSpecies == null || barkMaterial == null || canopyPrefab == null)
        {
            Debug.LogError("ForestTreeSpawner is missing its species, bark material or canopy prefab.", this);
            return null;
        }

        var root = new GameObject("Tree " + treeId);
        root.transform.SetParent(forestParent != null ? forestParent : transform, false);
        root.transform.position = new Vector3(groundPosition.x, 0f, groundPosition.z);
        root.transform.rotation = Quaternion.identity;

        var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.name = "Trunk";
        trunk.transform.SetParent(root.transform, false);
        trunk.GetComponent<Renderer>().sharedMaterial = barkMaterial;

        var canopy = Instantiate(canopyPrefab, root.transform);
        canopy.name = "Canopy";

        var tree = root.AddComponent<ForestTree>();
        tree.InitializeForSpawn(treeId, trunk.transform, canopy.transform, defaultSpecies, ageYears, heightMeters, dbhCm, crownRadiusMeters);
        if (visualPrefab != null)
            tree.SetVisualPrefabs(PickVisual(treeId, visualPrefab, visualPrefabAlternative), stumpPrefab);
        return tree;
    }
}
