using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public sealed class ForestSpeciesVisualSet
{
    public TreeSpeciesDefinition species;
    public GameObject seedlingVisualPrefab;
    public GameObject matureVisualPrefab;
    public GameObject poleVisualPrefab;
    public GameObject stumpPrefab;
}

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
    [SerializeField] private GameObject poleVisualPrefab;
    [SerializeField] private ForestSpeciesVisualSet[] speciesVisualSets;

    public TreeSpeciesDefinition DefaultSpecies => defaultSpecies;

    public IReadOnlyList<TreeSpeciesDefinition> KnownSpecies
    {
        get
        {
            var known = new List<TreeSpeciesDefinition>();
            if (defaultSpecies != null && !string.IsNullOrEmpty(defaultSpecies.SpeciesId))
                known.Add(defaultSpecies);
            if (speciesVisualSets != null)
            {
                foreach (ForestSpeciesVisualSet set in speciesVisualSets)
                {
                    TreeSpeciesDefinition candidate = set != null ? set.species : null;
                    if (candidate == null || string.IsNullOrEmpty(candidate.SpeciesId))
                        continue;
                    if (!known.Exists(s => string.Equals(s.SpeciesId, candidate.SpeciesId, StringComparison.Ordinal)))
                        known.Add(candidate);
                }
            }
            known.Sort((a, b) => string.CompareOrdinal(a.SpeciesId, b.SpeciesId));
            return known;
        }
    }

    public TreeSpeciesDefinition ResolveSpecies(string speciesId)
    {
        if (string.IsNullOrEmpty(speciesId))
            return defaultSpecies;
        if (defaultSpecies != null && defaultSpecies.SpeciesId == speciesId)
            return defaultSpecies;
        if (speciesVisualSets != null)
        {
            foreach (ForestSpeciesVisualSet set in speciesVisualSets)
                if (set != null && set.species != null && set.species.SpeciesId == speciesId)
                    return set.species;
        }
        return null;
    }

    public GameObject GetSeedlingVisualPrefab(TreeSpeciesDefinition requestedSpecies)
    {
        ForestSpeciesVisualSet set = FindVisualSet(requestedSpecies);
        return set != null ? set.seedlingVisualPrefab : null;
    }

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
        return Spawn(treeId, defaultSpecies, groundPosition, ageYears, dbhCm, heightMeters, crownRadiusMeters);
    }

    public ForestTree Spawn(string treeId, TreeSpeciesDefinition requestedSpecies, Vector3 groundPosition, int ageYears, float dbhCm, float heightMeters, float crownRadiusMeters)
    {
        TreeSpeciesDefinition species = requestedSpecies != null ? requestedSpecies : defaultSpecies;
        if (species == null || barkMaterial == null || canopyPrefab == null)
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
        tree.InitializeForSpawn(treeId, trunk.transform, canopy.transform, species, ageYears, heightMeters, dbhCm, crownRadiusMeters);
        ApplySpeciesVisuals(tree, species);
        return tree;
    }

    public void ApplySpeciesVisuals(ForestTree tree, TreeSpeciesDefinition requestedSpecies)
    {
        if (tree == null)
            return;
        TreeSpeciesDefinition species = requestedSpecies != null ? requestedSpecies : defaultSpecies;
        ForestSpeciesVisualSet speciesVisuals = FindVisualSet(species);
        if (speciesVisuals != null)
            tree.SetVisualStages(speciesVisuals.matureVisualPrefab, speciesVisuals.poleVisualPrefab, speciesVisuals.stumpPrefab != null ? speciesVisuals.stumpPrefab : stumpPrefab);
        else if (species == defaultSpecies && visualPrefab != null)
            tree.SetVisualStages(PickVisual(tree.TreeId, visualPrefab, visualPrefabAlternative), poleVisualPrefab, stumpPrefab);
    }

    private ForestSpeciesVisualSet FindVisualSet(TreeSpeciesDefinition species)
    {
        if (speciesVisualSets == null || species == null)
            return null;
        foreach (ForestSpeciesVisualSet set in speciesVisualSets)
            if (set != null && set.species == species)
                return set;
        return null;
    }
}
