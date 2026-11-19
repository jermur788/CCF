using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// The Compact30 E1 scene is an authored visual shell: it contains the 30 x 30 m ground, H1, the
// winding footpath, the 175 managed Sitka prefab instances and the player, but no scenario
// components. This bootstrap adds the existing management systems at runtime, in the same
// composition the historical scene builder uses, so the E1 scene file itself is not edited.
//
// It runs only for the exact Compact scene name. Every other scene, including ForestTest and the
// frozen Reference Future content, is untouched. All candidate content is validated before any
// management component exists; a failure refuses the world instead of silently running an empty
// 30 m shell with the five-rack UI and free player actions.

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
        if (Compact30Session.Refused || UnityEngine.Object.FindFirstObjectByType<Compact30Session>() != null) return;

        GameObject world = GameObject.Find("Compact30World");
        if (world == null)
        {
            Compact30Session.Refuse("Compact30World root is missing from the scene.");
            return;
        }

        TreeSpeciesDefinition sitka = Resources.Load<TreeSpeciesDefinition>("Compact30SitkaSpruce");
        TreeSpeciesDefinition oak = Resources.Load<TreeSpeciesDefinition>("Compact30SessileOak");
        TreeSpeciesDefinition beech = Resources.Load<TreeSpeciesDefinition>("Compact30Beech");
        ScenarioOneDefinition definition = Resources.Load<ScenarioOneDefinition>("Compact30Definition");
        if (sitka == null || oak == null || beech == null || definition == null)
        {
            Compact30Session.Refuse("Candidate species or definition fixtures are missing from Resources.");
            return;
        }
        if (!sitka.IsSimulationReady || !oak.IsSimulationReady || !beech.IsSimulationReady)
        {
            Compact30Session.Refuse("A candidate species is not simulation-ready.");
            return;
        }

        Compact30RosterFile roster = LoadRoster();
        Compact30SiteFile sites = LoadSites();
        if (!Compact30Rules.RosterIsValid(roster, out string rosterProblem))
        {
            Compact30Session.Refuse("Roster refused: " + rosterProblem);
            return;
        }
        if (sites == null || sites.sites == null || sites.sites.Count != 4)
        {
            Compact30Session.Refuse("Compact30 must define exactly four G1 sites.");
            return;
        }
        var stems = new List<Compact30Point>();
        foreach (Compact30TreeRow row in roster.rows)
            stems.Add(new Compact30Point(row.x_m, row.z_m));
        var occupied = new List<Compact30Point>();
        foreach (Compact30SiteRow site in sites.sites)
        {
            if (!Compact30Rules.ValidateSite(site, sites, stems, occupied, out string siteProblem))
            {
                Compact30Session.Refuse("G1 site " + (site != null ? site.id : "?") + " refused: " + siteProblem);
                return;
            }
            occupied.Add(new Compact30Point(site.x_m, site.z_m));
        }

        Transform treesRoot = world.transform.Find("Trees");
        if (!TryMapAuthoredTrees(roster, treesRoot, out Dictionary<string, Transform> authored, out string treeProblem))
        {
            Compact30Session.Refuse(treeProblem);
            return;
        }

        // Ecology first, on its own object: its Awake then sees Compact geometry, builds the 6 x 6
        // grid and correctly skips the Legacy40/Enlarged80 boundary helper.
        var ecologyObject = new GameObject("Compact30 Ecology");
        ecologyObject.SetActive(false);
        var ecology = ecologyObject.AddComponent<ForestEcologyController>();
        ecology.ApplyStandGeometry(StandGeometryModel.Compact30);
        ecology.SimulationSeed = 20260914;
        var spawner = ecologyObject.AddComponent<ForestTreeSpawner>();
        ecologyObject.AddComponent<ForestTreeMarkingManager>();
        ecologyObject.SetActive(true);

        // Scenario second: the manager's Awake follows the historical new-game policy (Enlarged80),
        // then the Compact binding immediately re-applies its own geometry and resets the state.
        var scenarioObject = new GameObject("Compact30 Scenario");
        scenarioObject.SetActive(false);
        var manager = scenarioObject.AddComponent<ScenarioOneManager>();
        manager.ConfigureDefinition(definition);
        scenarioObject.SetActive(true);

        ecology.ApplyStandGeometry(StandGeometryModel.Compact30);
        manager.InitializeNewScenario();
        manager.ApprovedWorkOrdersImmutable = true;
        manager.SuppressHistoricalCompletion = true;

        Material bark = CreateBarkMaterial();
        GameObject canopyTemplate = CreateCanopyTemplate(ecologyObject.transform);
        spawner.ConfigureForRuntime(sitka, bark, canopyTemplate, treesRoot, new[] { oak, beech });

        BindTrees(roster, authored, spawner, sitka);
        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();

        scenarioObject.AddComponent<Compact30Session>();
        Debug.Log("COMPACT30: " + roster.rows.Count + " living Sitka bound, 36-cell Compact30 world ready.", scenarioObject);
    }

    private static Compact30RosterFile LoadRoster()
    {
        TextAsset asset = Resources.Load<TextAsset>("Compact30Roster");
        return asset != null ? JsonUtility.FromJson<Compact30RosterFile>(asset.text) : null;
    }

    private static Compact30SiteFile LoadSites()
    {
        TextAsset asset = Resources.Load<TextAsset>("Compact30Sites");
        return asset != null ? JsonUtility.FromJson<Compact30SiteFile>(asset.text) : null;
    }

    private static bool TryMapAuthoredTrees(Compact30RosterFile roster, Transform treesRoot,
        out Dictionary<string, Transform> authored, out string problem)
    {
        authored = new Dictionary<string, Transform>(StringComparer.Ordinal);
        problem = "";
        if (treesRoot == null)
        {
            problem = "The Trees container is missing from Compact30World.";
            return false;
        }
        var wanted = new HashSet<string>(StringComparer.Ordinal);
        foreach (Compact30TreeRow row in roster.rows) wanted.Add(row.id);
        foreach (Transform child in treesRoot)
        {
            int underscore = child.name.IndexOf('_');
            string id = underscore > 0 ? child.name.Substring(0, underscore) : child.name;
            if (wanted.Contains(id)) authored[id] = child;
        }
        foreach (Compact30TreeRow row in roster.rows)
        {
            if (authored.ContainsKey(row.id)) continue;
            problem = "The authored scene is missing roster tree " + row.id + "; refusing the world rather than "
                + "inventing a replacement visual.";
            return false;
        }
        return true;
    }

    private static Material CreateBarkMaterial()
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        if (shader == null) return null;
        var material = new Material(shader) { name = "Compact30 placeholder bark" };
        material.color = new Color(0.28f, 0.22f, 0.16f);
        return material;
    }

    private static GameObject CreateCanopyTemplate(Transform parent)
    {
        GameObject canopy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        canopy.name = "Compact30 placeholder canopy";
        Collider collider = canopy.GetComponent<Collider>();
        if (collider != null) UnityEngine.Object.Destroy(collider);
        canopy.transform.SetParent(parent, false);
        canopy.transform.localScale = Vector3.one;
        canopy.SetActive(false);
        return canopy;
    }

    private static void BindTrees(Compact30RosterFile roster, Dictionary<string, Transform> authored,
        ForestTreeSpawner spawner, TreeSpeciesDefinition sitka)
    {
        foreach (Compact30TreeRow row in roster.rows)
        {
            Transform target = authored[row.id];
            target.position = new Vector3(row.x_m, 0f, row.z_m);
            if (target.GetComponent<ForestTree>() != null) continue;
            target.gameObject.SetActive(false);
            ForestTree tree = target.gameObject.AddComponent<ForestTree>();
            tree.InitializeAuthoredVisual(row.id, sitka, row.age_y, row.height_m, row.dbh_cm, row.crown_m);
            var capsule = target.gameObject.AddComponent<CapsuleCollider>();
            capsule.radius = 0.25f;
            capsule.height = Mathf.Max(1.2f, row.height_m);
            capsule.center = new Vector3(0f, capsule.height * 0.5f, 0f);
            target.gameObject.AddComponent<Compact30AuthoredTree>();
            target.gameObject.SetActive(true);
        }
    }
}
