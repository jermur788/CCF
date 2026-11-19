using UnityEngine;
using UnityEngine.SceneManagement;

// The Compact30 E1 scene is an authored visual shell: it contains the 30 x 30 m ground, H1, the
// winding footpath, the 175 managed Sitka prefab instances and the player, but no scenario
// components. This bootstrap adds the existing management systems at runtime, in the same
// composition the historical scene builder uses, so the E1 scene file itself is not edited.
//
// It runs only for the exact Compact scene name. Every other scene, including ForestTest and the
// frozen Reference Future content, is untouched.

public sealed class Compact30AuthoredTree : MonoBehaviour { }

public static class Compact30Bootstrap
{
    public const string SceneName = "ScenarioOneCompact30";
    private static bool subscribed;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void AfterFirstScene()
    {
        if (!subscribed)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            subscribed = true;
        }
        TryBind(SceneManager.GetActiveScene());
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode) => TryBind(scene);

    private static void TryBind(Scene scene)
    {
        if (!scene.IsValid() || scene.name != SceneName) return;
        if (Object.FindFirstObjectByType<Compact30Session>() != null) return;

        GameObject world = GameObject.Find("Compact30World");
        if (world == null)
        {
            Debug.LogError("COMPACT30: Compact30World root is missing; no management loop was started.");
            return;
        }

        TreeSpeciesDefinition sitka = Resources.Load<TreeSpeciesDefinition>("Compact30SitkaSpruce");
        TreeSpeciesDefinition oak = Resources.Load<TreeSpeciesDefinition>("Compact30SessileOak");
        TreeSpeciesDefinition beech = Resources.Load<TreeSpeciesDefinition>("Compact30Beech");
        ScenarioOneDefinition definition = Resources.Load<ScenarioOneDefinition>("Compact30Definition");
        if (sitka == null || oak == null || beech == null || definition == null)
        {
            Debug.LogError("COMPACT30: candidate species or definition fixtures are missing from Resources; "
                + "no management loop was started.");
            return;
        }

        var state = new GameObject("Compact30 Game State");
        state.SetActive(false);

        // Apply the geometry before Awake: the ecology controller then builds its 6 x 6 grid and
        // correctly skips the Legacy40/Enlarged80 world-boundary helper.
        var ecology = state.AddComponent<ForestEcologyController>();
        ecology.ApplyStandGeometry(StandGeometryModel.Compact30);
        ecology.SimulationSeed = 20260914;

        var spawner = state.AddComponent<ForestTreeSpawner>();
        state.AddComponent<ForestTreeMarkingManager>();
        var manager = state.AddComponent<ScenarioOneManager>();
        manager.ConfigureDefinition(definition);

        state.SetActive(true);

        // ScenarioOneManager.Awake follows the historical new-game policy (Enlarged80). Compact30
        // binds its own geometry immediately afterwards and resets scenario state, so no old-world
        // state survives the transient new-game defaults.
        ecology.ApplyStandGeometry(StandGeometryModel.Compact30);
        manager.InitializeNewScenario();
        manager.ApprovedWorkOrdersImmutable = true;
        manager.SuppressHistoricalCompletion = true;

        Material bark = CreateBarkMaterial();
        GameObject canopyTemplate = CreateCanopyTemplate(state.transform);
        Transform treesRoot = world.transform.Find("Trees");
        spawner.ConfigureForRuntime(sitka, bark, canopyTemplate, treesRoot,
            new[] { oak, beech });

        Compact30RosterFile roster = LoadRoster();
        if (!Compact30Rules.RosterIsValid(roster, out string reason))
        {
            Debug.LogError("COMPACT30: roster refused: " + reason);
            return;
        }
        BindTrees(roster, treesRoot, spawner, sitka);
        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();

        state.AddComponent<Compact30Session>();
        Debug.Log("COMPACT30: " + roster.rows.Count + " living Sitka bound, 36-cell Compact30 world ready.", state);
    }

    private static Compact30RosterFile LoadRoster()
    {
        TextAsset asset = Resources.Load<TextAsset>("Compact30Roster");
        return asset != null ? JsonUtility.FromJson<Compact30RosterFile>(asset.text) : null;
    }

    private static Material CreateBarkMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var material = new Material(shader) { name = "Compact30 placeholder bark" };
        material.color = new Color(0.28f, 0.22f, 0.16f);
        return material;
    }

    private static GameObject CreateCanopyTemplate(Transform parent)
    {
        GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        canopy.name = "Compact30 placeholder canopy";
        Collider collider = canopy.GetComponent<Collider>();
        if (collider != null) Object.Destroy(collider);
        canopy.transform.SetParent(parent, false);
        canopy.transform.localScale = Vector3.one;
        canopy.SetActive(false);
        return canopy;
    }

    private static void BindTrees(Compact30RosterFile roster, Transform treesRoot, ForestTreeSpawner spawner,
        TreeSpeciesDefinition sitka)
    {
        var byName = new System.Collections.Generic.Dictionary<string, Transform>();
        if (treesRoot != null)
            foreach (Transform child in treesRoot)
                byName[child.name] = child;

        foreach (Compact30TreeRow row in roster.rows)
        {
            Transform authored = null;
            foreach (var pair in byName)
                if (pair.Key == row.id || pair.Key.StartsWith(row.id + "_", System.StringComparison.Ordinal))
                {
                    authored = pair.Value;
                    break;
                }

            Vector3 position = new Vector3(row.x_m, 0f, row.z_m);
            if (authored == null)
            {
                // The E1 visual shell is missing a roster stem: still create the authoritative living
                // tree so the world always has the 175-row identity, and say so.
                spawner.Spawn(row.id, sitka, position, row.age_y, row.dbh_cm, row.height_m, row.crown_m);
                Debug.LogWarning("COMPACT30: no authored visual for " + row.id + "; spawned a placeholder tree.");
                continue;
            }

            authored.position = position;
            if (authored.GetComponent<ForestTree>() != null) continue;
            authored.gameObject.SetActive(false);
            ForestTree tree = authored.gameObject.AddComponent<ForestTree>();
            tree.InitializeAuthoredVisual(row.id, sitka, row.age_y, row.height_m, row.dbh_cm, row.crown_m);
            var capsule = authored.gameObject.AddComponent<CapsuleCollider>();
            capsule.radius = 0.25f;
            capsule.height = Mathf.Max(1.2f, row.height_m);
            capsule.center = new Vector3(0f, capsule.height * 0.5f, 0f);
            authored.gameObject.AddComponent<Compact30AuthoredTree>();
            authored.gameObject.SetActive(true);
        }
    }
}
