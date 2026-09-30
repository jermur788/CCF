using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Integrates already-delivered exports. Never runs the other agent's source
// builders, changes modelling files or assigns ecological meaning to defects.
public static class RecentDeliveriesAssetSetup
{
    public const string ReviewScene = "Assets/Scenes/ForestryAssetReview.unity";
    private const string Prefabs = "Assets/ForestPrototype/Prefabs/Forestry/SectionFive/";
    private const string CatalogPath = "Assets/ForestPrototype/ScenarioOne/Resources/RecentAssetVisualCatalog.asset";
    private static readonly string[] Grass = { "Woodland_Grass_Tuft_01", "Woodland_Grass_Tuft_02", "Woodland_Grass_Tuft_03" };
    private static readonly string[] Rush = { "Woodland_Rush_Clump_01", "Woodland_Rush_Clump_02" };
    private static readonly string[] Bent = { "SS_Defect_Bent_Young_02", "SS_Defect_Bent_Mature_02", "SS_Defect_Bent_PostMature_02" };
    private static readonly string[] Cavity = { "SS_Defect_Cavity_Young_02", "SS_Defect_Cavity_Mature_02", "SS_Defect_Cavity_PostMature_02" };

    [MenuItem("Tools/Forest Prototype/Integrate Recent Delivered Grass and Defect Assets")]
    public static void BuildAndWire()
    {
        SectionFiveAssetSetup.CopyPackage("Woodland_Grass_Rush", Grass.Concat(Rush).ToArray(), true);
        SectionFiveAssetSetup.CopyPackage("SS_Bent_Refined_Set", Bent, true);
        SectionFiveAssetSetup.CopyPackage("SS_Cavity_Refined_Set", Cavity, true);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        foreach (string name in Grass.Concat(Rush)) SectionFiveAssetSetup.Build("Woodland_Grass_Rush", name, true);
        foreach (string name in Bent) SectionFiveAssetSetup.Build("SS_Bent_Refined_Set", name, true);
        foreach (string name in Cavity) SectionFiveAssetSetup.Build("SS_Cavity_Refined_Set", name, true);
        RecentAssetVisualCatalog catalog = AssetDatabase.LoadAssetAtPath<RecentAssetVisualCatalog>(CatalogPath);
        if (catalog == null)
        {
            catalog = ScriptableObject.CreateInstance<RecentAssetVisualCatalog>();
            AssetDatabase.CreateAsset(catalog, CatalogPath);
        }
        catalog.grasses = Grass.Select(Load).ToArray(); catalog.rushes = Rush.Select(Load).ToArray();
        catalog.bentStages = Bent.Select(Load).ToArray(); catalog.cavityStages = Cavity.Select(Load).ToArray();
        EditorUtility.SetDirty(catalog);
        AssetDatabase.SaveAssets();
        WireMainGame();
        BuildReviewScene();
        ValidateAssets();
        ValidateReviewScene();
        Debug.Log("RECENT_DELIVERIES_BUILD_PASS models=33 prefabs=11 grassVariants=3 reviewScene=" + ReviewScene);
    }

    [MenuItem("Tools/Forest Prototype/Use Recent Assets in Main Game")]
    public static void WireMainGame()
    {
        RecentAssetVisualCatalog catalog = AssetDatabase.LoadAssetAtPath<RecentAssetVisualCatalog>(CatalogPath);
        if (catalog == null) throw new InvalidOperationException("Build the delivered prefabs before wiring the stand");
        if (catalog.rushDressingPositions == null || catalog.rushDressingPositions.Length == 0)
        {
            catalog.rushDressingPositions = new[] { new Vector3(-17f, 0f, -17f), new Vector3(17f, 0f, -17f) };
            EditorUtility.SetDirty(catalog);
        }
        AssetDatabase.SaveAssets();
        foreach (string path in new[] { "Assets/Scenes/ForestTest.unity", "Assets/Scenes/MixedSpeciesTest.unity" })
        {
            var scene = EditorSceneManager.OpenScene(path);
            ForestTreeSpawner spawner = UnityEngine.Object.FindFirstObjectByType<ForestTreeSpawner>();
            if (spawner == null) throw new InvalidOperationException("No spawner in " + path);
            WireSpawner(new SerializedObject(spawner));
            EditorSceneManager.SaveScene(scene);
        }
        Debug.Log("RECENT_DELIVERIES_MAIN_WIRED scenes=ForestTest,MixedSpeciesTest cosmeticTreeFamilies=Bent,Cavity rushAnchors="
            + catalog.rushDressingPositions.Length);
    }

    public static void WireSpawner(SerializedObject spawner)
    {
        if (AssetDatabase.LoadAssetAtPath<RecentAssetVisualCatalog>(CatalogPath) == null) return;
        spawner.FindProperty("useRecentStandVariants").boolValue = true;
        spawner.ApplyModifiedPropertiesWithoutUndo();
    }

    public static void ValidateAssets()
    {
        var catalog = AssetDatabase.LoadAssetAtPath<RecentAssetVisualCatalog>(CatalogPath);
        if (catalog == null || catalog.grasses.Length != 3 || catalog.rushes.Length != 2
            || catalog.bentStages.Length != 3 || catalog.cavityStages.Length != 3)
            throw new InvalidOperationException("Recent asset catalog incomplete");
        foreach (string name in Grass.Concat(Rush).Concat(Bent).Concat(Cavity))
        {
            GameObject prefab = Load(name);
            if (prefab == null || prefab.GetComponent<LODGroup>()?.GetLODs().Length != 3
                || prefab.GetComponentsInChildren<Collider>(true).Length != 0)
                throw new InvalidOperationException("Missing/colliding LOD prefab: " + name);
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                LOD[] lods = instance.GetComponent<LODGroup>().GetLODs();
                Bounds reference = new Bounds();
                long previousTriangles = long.MaxValue;
                for (int level = 0; level < 3; level++)
                {
                    if (lods[level].renderers.Length == 0) throw new InvalidOperationException("Empty LOD: " + name);
                    Bounds bounds = lods[level].renderers[0].bounds;
                    long triangles = 0;
                    foreach (Renderer renderer in lods[level].renderers)
                    {
                        bounds.Encapsulate(renderer.bounds);
                        foreach (Material material in renderer.sharedMaterials)
                        {
                            if (material == null || material.shader.name != "Universal Render Pipeline/Lit")
                                throw new InvalidOperationException("Missing URP material: " + name);
                            if (material.GetFloat("_Metallic") != 0f || material.GetFloat("_Smoothness") > 0.2f)
                                throw new InvalidOperationException("Non-matte material: " + name);
                            if (material.name == "SS_Benchmark_Needles"
                                && (material.GetFloat("_Cull") != 0f || !material.IsKeywordEnabled("_ALPHATEST_ON")
                                    || Mathf.Abs(material.GetFloat("_Cutoff") - 0.38f) > 0.001f))
                                throw new InvalidOperationException("Needle alpha setup incorrect: " + name);
                            if (material.name == "SS_Benchmark_Bark"
                                && (!material.IsKeywordEnabled("_NORMALMAP")
                                    || material.GetTexture("_BumpMap") == null || material.GetTexture("_MetallicGlossMap") != null))
                                throw new InvalidOperationException("Refined bark normal/gloss setup incorrect: " + name);
                        }
                        Mesh mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
                        for (int submesh = 0; submesh < mesh.subMeshCount; submesh++) triangles += mesh.GetIndexCount(submesh) / 3;
                    }
                    if (triangles <= 0 || triangles > previousTriangles) throw new InvalidOperationException("LOD geometry cost incorrect: " + name);
                    previousTriangles = triangles;
                    if (float.IsNaN(bounds.size.y) || bounds.size.y <= 0f || Mathf.Abs(bounds.min.y) > 0.05f)
                        throw new InvalidOperationException("Not grounded: " + name + " " + bounds);
                    if (level == 0) reference = bounds;
                    else if (Mathf.Abs(bounds.size.y - reference.size.y) > 0.06f)
                        throw new InvalidOperationException("LOD height jump: " + name);
                    if (name.StartsWith("SS_Defect_", StringComparison.Ordinal))
                    {
                        float authoredHeight = name.Contains("PostMature") ? 31f : name.Contains("Young") ? 11f : 26f;
                        if (Mathf.Abs(bounds.size.y - authoredHeight) > 0.06f)
                            throw new InvalidOperationException("Delivered metre scale lost: " + name);
                    }
                }
            }
            finally { UnityEngine.Object.DestroyImmediate(instance); }
        }
        Debug.Log("RECENT_DELIVERIES_ASSETS_PASS grounded=True lods=3 decreasingTriangles=True matteBark=True");
    }

    private static void BuildReviewScene()
    {
        // An existing scene may contain the user's review layout. Rebuilding
        // the prefabs is safe; overwriting that scene is not required.
        if (File.Exists(ReviewScene)) return;
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientMode = AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.52f, 0.65f, 0.76f);
        RenderSettings.ambientEquatorColor = new Color(0.34f, 0.40f, 0.30f);
        RenderSettings.ambientGroundColor = new Color(0.18f, 0.20f, 0.14f);
        Material groundMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/ForestPrototype/Materials/Forest Ground.mat");
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Review Ground";
        ground.transform.position = new Vector3(0f, -0.5f, 10f);
        ground.transform.localScale = new Vector3(130f, 1f, 130f);
        ground.GetComponent<Renderer>().sharedMaterial = groundMaterial;
        Light sun = new GameObject("Review Sun").AddComponent<Light>();
        sun.type = LightType.Directional; sun.intensity = 1.6f; sun.shadows = LightShadows.Soft;
        sun.transform.rotation = Quaternion.Euler(48f, -30f, 0f);
        for (int stage = 0; stage < 3; stage++)
        {
            Place(Bent[stage], new Vector3(-22f, 0f, stage * 26f));
            Place(Cavity[stage], new Vector3(12f, 0f, stage * 26f));
        }
        string[] plants = Grass.Concat(Rush).ToArray();
        for (int i = 0; i < plants.Length; i++) Place(plants[i], new Vector3(-18f + i * 8f, 0f, -14f));
        var player = new GameObject("Review Player");
        player.transform.position = new Vector3(-5f, 0.08f, -25f);
        var body = player.AddComponent<CharacterController>();
        body.height = 1.8f; body.radius = 0.3f; body.center = new Vector3(0f, 0.9f, 0f);
        var camera = new GameObject("Main Camera").AddComponent<Camera>();
        camera.tag = "MainCamera";
        camera.transform.SetParent(player.transform, false);
        camera.transform.localPosition = new Vector3(0f, 1.65f, 0f);
        camera.nearClipPlane = 0.05f; camera.farClipPlane = 180f; camera.fieldOfView = 75f;
        camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = RenderSettings.ambientSkyColor;
        camera.gameObject.AddComponent<AudioListener>();
        var controller = player.AddComponent<ForestPlayer>();
        var serialized = new SerializedObject(controller);
        serialized.FindProperty("view").objectReferenceValue = camera.transform;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        player.AddComponent<RecentAssetReviewControls>();
        EditorSceneManager.SaveScene(scene, ReviewScene);
    }

    private static void Place(string name, Vector3 position)
    {
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(Load(name));
        instance.transform.position = position;
        var sign = new GameObject("Label " + name).AddComponent<TextMesh>();
        sign.text = name + "\nDelivered visual reference";
        sign.fontSize = 48; sign.characterSize = name.StartsWith("SS_Defect_", StringComparison.Ordinal) ? 0.16f : 0.06f;
        sign.color = Color.white; sign.anchor = TextAnchor.MiddleCenter;
        sign.transform.position = position + new Vector3(0f, 0.9f, -2f);
    }

    public static void ValidateReviewScene()
    {
        var scene = EditorSceneManager.OpenScene(ReviewScene);
        GameObject[] roots = scene.GetRootGameObjects();
        if (roots.Sum(root => root.GetComponentsInChildren<LODGroup>(true).Length) != 11
            || roots.Sum(root => root.GetComponentsInChildren<ForestPlayer>(true).Length) != 1
            || roots.Sum(root => root.GetComponentsInChildren<Camera>(true).Length) != 1
            || roots.Sum(root => root.GetComponentsInChildren<AudioListener>(true).Length) != 1
            || roots.Sum(root => root.GetComponentsInChildren<RecentAssetReviewControls>(true).Length) != 1)
            throw new InvalidOperationException("Review scene display/player configuration is incorrect");
        if (roots.Any(root => root.GetComponentInChildren<ForestEcologyController>(true) != null
            || root.GetComponentInChildren<ForestSaveController>(true) != null || root.GetComponentInChildren<ForestTree>(true) != null))
            throw new InvalidOperationException("Art-review specimens entered authoritative forest state");
        Debug.Log("RECENT_DELIVERIES_REVIEW_PASS specimens=11 player=1 simulationRecords=0");
    }

    private static GameObject Load(string name) => AssetDatabase.LoadAssetAtPath<GameObject>(Prefabs + name + ".prefab");
}
