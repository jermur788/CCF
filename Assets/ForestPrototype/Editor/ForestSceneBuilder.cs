using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public static class ForestSceneBuilder
{
    public const string ScenePath = "Assets/Scenes/ForestTest.unity";
    private const string MaterialFolder = "Assets/ForestPrototype/Materials";
    private const string SpeciesFolder = "Assets/ForestPrototype/Species";
    private const string PrefabsFolder = "Assets/ForestPrototype/Prefabs";
    private const string MeshesFolder = "Assets/ForestPrototype/Meshes";
    private const string ConiferCanopyPath = PrefabsFolder + "/PF_ConiferCanopy.prefab";

    // Explicit command only: importing these scripts never replaces an open scene.
    [MenuItem("Tools/Forest Prototype/Create Test Scene")]
    public static void Create()
    {
        if (File.Exists(ScenePath))
        {
            Debug.Log("ForestTest already exists; leaving it unchanged.");
            return;
        }
        if (Application.isBatchMode)
            EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity", OpenSceneMode.Single);
        var previous = SceneManager.GetActiveScene();
        if (string.IsNullOrEmpty(previous.path))
        {
            Debug.LogWarning("Save the current untitled scene before creating ForestTest.");
            return;
        }
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        try
        {
            Directory.CreateDirectory(MaterialFolder);
            AssetDatabase.Refresh();
            var sitkaSpruce = Species("sitka-spruce", "Sitka spruce", "Picea sitchensis", SpeciesFolder + "/SitkaSpruce.asset");
            var coniferCanopy = GetOrCreateConiferCanopyPrefab();
            var ground = Material("Forest Ground", new Color(0.22f, 0.29f, 0.12f));
            var bark = Material("Bark", new Color(0.24f, 0.13f, 0.065f));
            var rock = Material("Stone", new Color(0.35f, 0.38f, 0.31f));
            Primitive("Ground", PrimitiveType.Cube, new Vector3(0, -0.5f, 0), new Vector3(40, 1, 40), ground);
            // Solid visible borders keep the player on the small test area.
            Primitive("North Ridge", PrimitiveType.Cube, new Vector3(0, 1, 20), new Vector3(42, 2, 1), rock);
            Primitive("South Ridge", PrimitiveType.Cube, new Vector3(0, 1, -20), new Vector3(42, 2, 1), rock);
            Primitive("East Ridge", PrimitiveType.Cube, new Vector3(20, 1, 0), new Vector3(1, 2, 40), rock);
            Primitive("West Ridge", PrimitiveType.Cube, new Vector3(-20, 1, 0), new Vector3(1, 2, 40), rock);
            var forest = new GameObject("Forest");
            var random = new System.Random(73);
            int count = 0;
            for (int x = -16; x <= 16; x += 4)
            for (int z = -16; z <= 16; z += 4)
            {
                // Open central walking corridor and spawn clearing.
                if (Mathf.Abs(x) < 3 || (Mathf.Abs(x) <= 4 && z < -8)) continue;
                float px = x + (float)random.NextDouble() * 1.5f - 0.75f;
                float pz = z + (float)random.NextDouble() * 1.5f - 0.75f;
                float height = 4f + (float)random.NextDouble() * 2f;
                var tree = new GameObject("Tree " + (++count));
                tree.transform.SetParent(forest.transform);
                tree.transform.position = new Vector3(px, 0f, pz);
                var trunk = Primitive("Trunk", PrimitiveType.Cylinder, new Vector3(px, height / 2, pz), new Vector3(0.65f, height / 2, 0.65f), bark, tree.transform);
                var canopy = (GameObject)PrefabUtility.InstantiatePrefab(coniferCanopy, tree.transform);
                canopy.name = "Canopy";
                canopy.transform.localPosition = new Vector3(0f, height, 0f);
                canopy.transform.localScale = new Vector3(3.3f, 3.63f, 3.3f);
                var treeState = tree.AddComponent<ForestTree>();
                var serializedTree = new SerializedObject(treeState);
                serializedTree.FindProperty("treeId").stringValue = "T" + count.ToString("00");
                serializedTree.FindProperty("trunk").objectReferenceValue = trunk.transform;
                serializedTree.FindProperty("canopy").objectReferenceValue = canopy.transform;
                serializedTree.FindProperty("species").objectReferenceValue = sitkaSpruce;
                serializedTree.FindProperty("heightMeters").floatValue = height;
                serializedTree.FindProperty("diameterCm").floatValue = 65f;
                serializedTree.FindProperty("crownRadiusMeters").floatValue = 1.65f;
                serializedTree.ApplyModifiedPropertiesWithoutUndo();
            }
            var workbench = CreateWorkbench(new Vector3(-2.75f, 0f, 11f));
            CreateLogRack(workbench.GetComponent<ForestBuildable>(), new Vector3(-0.3f, 0f, 11.5f));
            CreateShelter(workbench.GetComponent<ForestBuildable>(), new Vector3(-5.6f, 0f, 12.2f));
            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.transform.rotation = Quaternion.Euler(48, -30, 0);
            sun.color = new Color(1f, 0.94f, 0.82f);
            sun.intensity = 1.6f;
            sun.shadows = LightShadows.Soft;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.52f, 0.65f, 0.76f);
            RenderSettings.ambientEquatorColor = new Color(0.34f, 0.40f, 0.30f);
            RenderSettings.ambientGroundColor = new Color(0.18f, 0.20f, 0.14f);
            var player = new GameObject("Player");
            player.transform.position = new Vector3(0, 0.08f, -13);
            var body = player.AddComponent<CharacterController>();
            body.height = 1.8f;
            body.radius = 0.3f;
            body.center = new Vector3(0, 0.9f, 0);
            body.stepOffset = 0.25f;
            body.slopeLimit = 45f;
            body.skinWidth = 0.03f;
            body.minMoveDistance = 0f;
            var camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.SetParent(player.transform, false);
            camera.transform.localPosition = new Vector3(0, 1.65f, 0);
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 120f;
            camera.fieldOfView = 75f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.52f, 0.66f, 0.77f);
            camera.gameObject.AddComponent<AudioListener>();
            var controller = player.AddComponent<ForestPlayer>();
            var serialized = new SerializedObject(controller);
            serialized.FindProperty("view").objectReferenceValue = camera.transform;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            var gameState = new GameObject("Game State");
            gameState.AddComponent<ForestSaveController>();
            gameState.AddComponent<ForestEcologyController>();
            var spawner = gameState.AddComponent<ForestTreeSpawner>();
            var spawnerSerialized = new SerializedObject(spawner);
            spawnerSerialized.FindProperty("defaultSpecies").objectReferenceValue = sitkaSpruce;
            spawnerSerialized.FindProperty("barkMaterial").objectReferenceValue = bark;
            spawnerSerialized.FindProperty("canopyPrefab").objectReferenceValue = coniferCanopy;
            spawnerSerialized.FindProperty("forestParent").objectReferenceValue = forest.transform;
            spawnerSerialized.ApplyModifiedPropertiesWithoutUndo();
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            ValidateScene(scene);
            Debug.Log("FOREST_CREATED: " + ScenePath + "; trees=" + count);
        }
        finally
        {
            if (previous.IsValid()) SceneManager.SetActiveScene(previous);
            EditorSceneManager.CloseScene(scene, true);
        }
    }

    private static Material Material(string name, Color color)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        var material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material != null) return material;
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) throw new InvalidOperationException("URP Lit shader missing.");
        material = new Material(shader) { name = name };
        material.SetColor("_BaseColor", color);
        material.SetFloat("_Smoothness", 0f);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static TreeSpeciesDefinition Species(string id, string displayName, string latinName, string path)
    {
        var species = AssetDatabase.LoadAssetAtPath<TreeSpeciesDefinition>(path);
        if (species != null) return species;
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        species = ScriptableObject.CreateInstance<TreeSpeciesDefinition>();
        species.name = displayName;
        var serialized = new SerializedObject(species);
        serialized.FindProperty("speciesId").stringValue = id;
        serialized.FindProperty("displayName").stringValue = displayName;
        serialized.FindProperty("latinName").stringValue = latinName;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.CreateAsset(species, path);
        return species;
    }

    // Placeholder conifer silhouette: three stacked cones sized so the canopy
    // transform's unit space matches the old sphere envelope.
    public static GameObject GetOrCreateConiferCanopyPrefab()
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(ConiferCanopyPath);
        if (prefab != null) return prefab;

        var cone = GetOrCreateConiferConeMesh();
        var leaves = Material("Leaves", new Color(0.10f, 0.29f, 0.12f));
        var root = new GameObject("PF_ConiferCanopy");
        AddCone(root.transform, cone, leaves, "Crown Lower", new Vector3(0f, -0.2f, 0f), new Vector3(1f, 0.6f, 1f));
        AddCone(root.transform, cone, leaves, "Crown Middle", new Vector3(0f, 0.02f, 0f), new Vector3(0.7f, 0.6f, 0.7f));
        AddCone(root.transform, cone, leaves, "Crown Upper", new Vector3(0f, 0.28f, 0f), new Vector3(0.4f, 0.44f, 0.4f));
        Directory.CreateDirectory(PrefabsFolder);
        prefab = PrefabUtility.SaveAsPrefabAsset(root, ConiferCanopyPath);
        UnityEngine.Object.DestroyImmediate(root);
        return prefab;
    }

    public static Mesh GetOrCreateConiferConeMesh()
    {
        var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(MeshesFolder + "/ConiferCone.asset");
        if (mesh != null) return mesh;

        const int segments = 14;
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        for (int i = 0; i < segments; i++)
        {
            float angle = 2f * Mathf.PI * i / segments;
            vertices.Add(new Vector3(Mathf.Cos(angle) * 0.5f, -0.5f, Mathf.Sin(angle) * 0.5f));
        }
        int apex = vertices.Count;
        vertices.Add(new Vector3(0f, 0.5f, 0f));
        int baseCenter = vertices.Count;
        vertices.Add(new Vector3(0f, -0.5f, 0f));
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            triangles.Add(i);
            triangles.Add(apex);
            triangles.Add(next);
            triangles.Add(baseCenter);
            triangles.Add(i);
            triangles.Add(next);
        }

        mesh = new Mesh { name = "ConiferCone" };
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        Directory.CreateDirectory(MeshesFolder);
        AssetDatabase.CreateAsset(mesh, MeshesFolder + "/ConiferCone.asset");
        return mesh;
    }

    private static void AddCone(Transform parent, Mesh mesh, Material material, string name, Vector3 localPosition, Vector3 localScale)
    {
        var cone = new GameObject(name);
        cone.transform.SetParent(parent, false);
        cone.transform.localPosition = localPosition;
        cone.transform.localScale = localScale;
        cone.AddComponent<MeshFilter>().sharedMesh = mesh;
        cone.AddComponent<MeshRenderer>().sharedMaterial = material;
    }

    public static GameObject CreateWorkbench(Vector3 position)
    {
        var bark = Material("Bark", new Color(0.24f, 0.13f, 0.065f));
        var leaves = Material("Leaves", new Color(0.10f, 0.29f, 0.12f));
        var stone = Material("Stone", new Color(0.35f, 0.38f, 0.31f));

        var workbench = new GameObject("Forestry Workbench");
        workbench.transform.position = position;
        var collider = workbench.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.7f, 0f);
        collider.size = new Vector3(2.4f, 1.4f, 1.2f);

        var unbuilt = new GameObject("Unbuilt Site");
        unbuilt.transform.SetParent(workbench.transform, false);
        Visual(unbuilt.transform, "Frame Post Front Left", new Vector3(-0.9f, 0.6f, -0.4f), new Vector3(0.14f, 1.2f, 0.14f), bark);
        Visual(unbuilt.transform, "Frame Post Front Right", new Vector3(0.9f, 0.6f, -0.4f), new Vector3(0.14f, 1.2f, 0.14f), bark);
        Visual(unbuilt.transform, "Frame Post Back Left", new Vector3(-0.9f, 0.6f, 0.4f), new Vector3(0.14f, 1.2f, 0.14f), bark);
        Visual(unbuilt.transform, "Frame Post Back Right", new Vector3(0.9f, 0.6f, 0.4f), new Vector3(0.14f, 1.2f, 0.14f), bark);
        Visual(unbuilt.transform, "Frame Rail Front", new Vector3(0f, 1.2f, -0.4f), new Vector3(2.04f, 0.12f, 0.14f), bark);
        Visual(unbuilt.transform, "Frame Rail Back", new Vector3(0f, 1.2f, 0.4f), new Vector3(2.04f, 0.12f, 0.14f), bark);
        Visual(unbuilt.transform, "Frame Rail Left", new Vector3(-0.9f, 1.2f, 0f), new Vector3(0.14f, 0.12f, 0.94f), bark);
        Visual(unbuilt.transform, "Frame Rail Right", new Vector3(0.9f, 1.2f, 0f), new Vector3(0.14f, 0.12f, 0.94f), bark);

        var built = new GameObject("Built Workbench");
        built.transform.SetParent(workbench.transform, false);
        Visual(built.transform, "Workbench Top", new Vector3(0f, 1.25f, 0f), new Vector3(2.4f, 0.2f, 1.2f), bark);
        Visual(built.transform, "Leg Front Left", new Vector3(-0.9f, 0.58f, -0.4f), new Vector3(0.18f, 1.15f, 0.18f), bark);
        Visual(built.transform, "Leg Front Right", new Vector3(0.9f, 0.58f, -0.4f), new Vector3(0.18f, 1.15f, 0.18f), bark);
        Visual(built.transform, "Leg Back Left", new Vector3(-0.9f, 0.58f, 0.4f), new Vector3(0.18f, 1.15f, 0.18f), bark);
        Visual(built.transform, "Leg Back Right", new Vector3(0.9f, 0.58f, 0.4f), new Vector3(0.18f, 1.15f, 0.18f), bark);
        Visual(built.transform, "Tool Rack", new Vector3(0f, 2.25f, 0f), new Vector3(2.8f, 0.16f, 1.6f), bark);
        Visual(built.transform, "Rack Support Left", new Vector3(-1.1f, 1.76f, 0f), new Vector3(0.1f, 0.82f, 0.1f), bark);
        Visual(built.transform, "Rack Support Right", new Vector3(1.1f, 1.76f, 0f), new Vector3(0.1f, 0.82f, 0.1f), bark);
        built.SetActive(false);

        var buildable = workbench.AddComponent<ForestBuildable>();
        var serialized = new SerializedObject(buildable);
        serialized.FindProperty("woodCost").intValue = 8;
        serialized.FindProperty("interactionDistance").floatValue = 4f;
        serialized.FindProperty("displayName").stringValue = "Forestry Workbench";
        serialized.FindProperty("buildId").stringValue = "workbench-01";
        serialized.FindProperty("unbuiltVisual").objectReferenceValue = unbuilt;
        serialized.FindProperty("builtVisual").objectReferenceValue = built;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        return workbench;
    }

    public static GameObject CreateLogRack(ForestBuildable workbench, Vector3 position)
    {
        var bark = Material("Bark", new Color(0.24f, 0.13f, 0.065f));
        var stone = Material("Stone", new Color(0.35f, 0.38f, 0.31f));

        var rack = new GameObject("Log Rack");
        rack.transform.position = position;
        var collider = rack.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 0.55f, 0f);
        collider.size = new Vector3(1.8f, 1.1f, 1.1f);

        var unbuilt = new GameObject("Unbuilt Site");
        unbuilt.transform.SetParent(rack.transform, false);
        Visual(unbuilt.transform, "Foundation Marker", new Vector3(0f, 0.05f, 0f), new Vector3(1.8f, 0.1f, 1.0f), stone);
        Visual(unbuilt.transform, "Corner Post A", new Vector3(-0.7f, 0.45f, -0.4f), new Vector3(0.1f, 0.9f, 0.1f), stone);
        Visual(unbuilt.transform, "Corner Post B", new Vector3(0.7f, 0.45f, -0.4f), new Vector3(0.1f, 0.9f, 0.1f), stone);

        var built = new GameObject("Built Visual");
        built.transform.SetParent(rack.transform, false);
        var frame = new GameObject("Rack Frame");
        frame.transform.SetParent(built.transform, false);
        Visual(frame.transform, "Frame Post Front Left", new Vector3(-0.75f, 0.5f, -0.4f), new Vector3(0.12f, 1.0f, 0.12f), bark);
        Visual(frame.transform, "Frame Post Front Right", new Vector3(0.75f, 0.5f, -0.4f), new Vector3(0.12f, 1.0f, 0.12f), bark);
        Visual(frame.transform, "Frame Post Back Left", new Vector3(-0.75f, 0.5f, 0.4f), new Vector3(0.12f, 1.0f, 0.12f), bark);
        Visual(frame.transform, "Frame Post Back Right", new Vector3(0.75f, 0.5f, 0.4f), new Vector3(0.12f, 1.0f, 0.12f), bark);
        Visual(frame.transform, "Top Rail Left", new Vector3(-0.75f, 0.98f, 0f), new Vector3(0.1f, 0.1f, 0.92f), bark);
        Visual(frame.transform, "Top Rail Right", new Vector3(0.75f, 0.98f, 0f), new Vector3(0.1f, 0.1f, 0.92f), bark);

        var fillLow = new GameObject("Log Fill Low");
        fillLow.transform.SetParent(built.transform, false);
        AddLog(fillLow.transform, "Log", new Vector3(0f, 0.24f, -0.18f), bark);
        AddLog(fillLow.transform, "Log", new Vector3(0f, 0.24f, 0.18f), bark);

        var fillMedium = new GameObject("Log Fill Medium");
        fillMedium.transform.SetParent(built.transform, false);
        AddLog(fillMedium.transform, "Log", new Vector3(0f, 0.46f, -0.18f), bark);
        AddLog(fillMedium.transform, "Log", new Vector3(0f, 0.46f, 0.18f), bark);

        var fillFull = new GameObject("Log Fill Full");
        fillFull.transform.SetParent(built.transform, false);
        AddLog(fillFull.transform, "Log", new Vector3(0f, 0.68f, -0.26f), bark);
        AddLog(fillFull.transform, "Log", new Vector3(0f, 0.68f, 0f), bark);
        AddLog(fillFull.transform, "Log", new Vector3(0f, 0.68f, 0.26f), bark);
        built.SetActive(false);

        var buildable = rack.AddComponent<ForestBuildable>();
        var serialized = new SerializedObject(buildable);
        serialized.FindProperty("woodCost").intValue = 6;
        serialized.FindProperty("interactionDistance").floatValue = 4f;
        serialized.FindProperty("displayName").stringValue = "Log Rack";
        serialized.FindProperty("buildId").stringValue = "log-rack-build-01";
        serialized.FindProperty("requiredBuildable").objectReferenceValue = workbench;
        serialized.FindProperty("unbuiltVisual").objectReferenceValue = unbuilt;
        serialized.FindProperty("builtVisual").objectReferenceValue = built;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        var storage = rack.AddComponent<ForestWoodStorage>();
        var storageSerialized = new SerializedObject(storage);
        storageSerialized.FindProperty("storageId").stringValue = "log-rack-01";
        storageSerialized.FindProperty("capacity").intValue = 100;
        storageSerialized.FindProperty("interactionDistance").floatValue = 4f;
        storageSerialized.FindProperty("displayName").stringValue = "Log Rack";
        storageSerialized.FindProperty("buildable").objectReferenceValue = buildable;
        storageSerialized.FindProperty("logFillLow").objectReferenceValue = fillLow;
        storageSerialized.FindProperty("logFillMedium").objectReferenceValue = fillMedium;
        storageSerialized.FindProperty("logFillFull").objectReferenceValue = fillFull;
        storageSerialized.ApplyModifiedPropertiesWithoutUndo();
        return rack;
    }

    public static GameObject CreateShelter(ForestBuildable workbench, Vector3 position)
    {
        var bark = Material("Bark", new Color(0.24f, 0.13f, 0.065f));
        var stone = Material("Stone", new Color(0.35f, 0.38f, 0.31f));

        var shelter = new GameObject("Basic Shelter");
        shelter.transform.position = position;
        var collider = shelter.AddComponent<BoxCollider>();
        collider.center = new Vector3(0f, 1.2f, 0f);
        collider.size = new Vector3(4.2f, 2.4f, 3.3f);

        var unbuilt = new GameObject("Unbuilt Site");
        unbuilt.transform.SetParent(shelter.transform, false);
        Visual(unbuilt.transform, "Footing Front", new Vector3(0f, 0.05f, -1.4f), new Vector3(4.0f, 0.1f, 0.14f), stone);
        Visual(unbuilt.transform, "Footing Back", new Vector3(0f, 0.05f, 1.4f), new Vector3(4.0f, 0.1f, 0.14f), stone);
        Visual(unbuilt.transform, "Footing Left", new Vector3(-1.9f, 0.05f, 0f), new Vector3(0.14f, 0.1f, 2.9f), stone);
        Visual(unbuilt.transform, "Footing Right", new Vector3(1.9f, 0.05f, 0f), new Vector3(0.14f, 0.1f, 2.9f), stone);
        Visual(unbuilt.transform, "Stake Front Left", new Vector3(-1.9f, 0.35f, -1.4f), new Vector3(0.12f, 0.7f, 0.12f), bark);
        Visual(unbuilt.transform, "Stake Front Right", new Vector3(1.9f, 0.35f, -1.4f), new Vector3(0.12f, 0.7f, 0.12f), bark);
        Visual(unbuilt.transform, "Stake Back Left", new Vector3(-1.9f, 0.35f, 1.4f), new Vector3(0.12f, 0.7f, 0.12f), bark);
        Visual(unbuilt.transform, "Stake Back Right", new Vector3(1.9f, 0.35f, 1.4f), new Vector3(0.12f, 0.7f, 0.12f), bark);

        var built = new GameObject("Built Shelter");
        built.transform.SetParent(shelter.transform, false);
        Visual(built.transform, "Back Post Left", new Vector3(-1.9f, 1.2f, 1.4f), new Vector3(0.16f, 2.4f, 0.16f), bark);
        Visual(built.transform, "Back Post Right", new Vector3(1.9f, 1.2f, 1.4f), new Vector3(0.16f, 2.4f, 0.16f), bark);
        Visual(built.transform, "Front Post Left", new Vector3(-1.9f, 0.9f, -1.4f), new Vector3(0.16f, 1.8f, 0.16f), bark);
        Visual(built.transform, "Front Post Right", new Vector3(1.9f, 0.9f, -1.4f), new Vector3(0.16f, 1.8f, 0.16f), bark);
        Visual(built.transform, "Back Beam", new Vector3(0f, 2.3f, 1.4f), new Vector3(3.96f, 0.14f, 0.14f), bark);
        Visual(built.transform, "Front Beam", new Vector3(0f, 1.72f, -1.4f), new Vector3(3.96f, 0.14f, 0.14f), bark);
        Visual(built.transform, "Back Wall Board Low", new Vector3(0f, 0.55f, 1.42f), new Vector3(3.9f, 0.6f, 0.08f), bark);
        Visual(built.transform, "Back Wall Board Middle", new Vector3(0f, 1.25f, 1.42f), new Vector3(3.9f, 0.6f, 0.08f), bark);
        Visual(built.transform, "Back Wall Board High", new Vector3(0f, 1.95f, 1.42f), new Vector3(3.9f, 0.6f, 0.08f), bark);
        var roof = Visual(built.transform, "Roof", new Vector3(0f, 2.12f, 0f), new Vector3(4.4f, 0.12f, 3.7f), bark);
        roof.transform.localRotation = Quaternion.Euler(-12f, 0f, 0f);
        built.SetActive(false);

        var buildable = shelter.AddComponent<ForestBuildable>();
        var serialized = new SerializedObject(buildable);
        serialized.FindProperty("woodCost").intValue = 20;
        serialized.FindProperty("interactionDistance").floatValue = 4f;
        serialized.FindProperty("displayName").stringValue = "Basic Shelter";
        serialized.FindProperty("buildId").stringValue = "shelter-01";
        serialized.FindProperty("requiredBuildable").objectReferenceValue = workbench;
        serialized.FindProperty("unbuiltVisual").objectReferenceValue = unbuilt;
        serialized.FindProperty("builtVisual").objectReferenceValue = built;
        serialized.ApplyModifiedPropertiesWithoutUndo();
        return shelter;
    }

    private static GameObject Visual(Transform parent, string name, Vector3 localPosition, Vector3 localScale, Material material)
    {
        var visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.name = name;
        visual.transform.SetParent(parent, false);
        visual.transform.localPosition = localPosition;
        visual.transform.localScale = localScale;
        visual.GetComponent<Renderer>().sharedMaterial = material;
        UnityEngine.Object.DestroyImmediate(visual.GetComponent<Collider>());
        return visual;
    }

    private static GameObject AddLog(Transform parent, string name, Vector3 localPosition, Material material)
    {
        var log = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        log.name = name;
        log.transform.SetParent(parent, false);
        log.transform.localPosition = localPosition;
        log.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        log.transform.localScale = new Vector3(0.22f, 0.7f, 0.22f);
        log.GetComponent<Renderer>().sharedMaterial = material;
        UnityEngine.Object.DestroyImmediate(log.GetComponent<Collider>());
        return log;
    }

    private static GameObject Primitive(string name, PrimitiveType type, Vector3 position, Vector3 scale, Material material, Transform parent = null)
    {
        var obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.position = position;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().sharedMaterial = material;
        return obj;
    }

    public static void Validate()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        ValidateScene(scene);
    }

    private static void ValidateScene(Scene scene)
    {
        int players = 0, cameras = 0, listeners = 0, saveControllers = 0, ecologyControllers = 0, spawners = 0;
        var treeIds = new HashSet<string>();
        var buildableIds = new HashSet<string>();
        var storageIds = new HashSet<string>();
        var validatedSpecies = new HashSet<string>();
        foreach (var root in scene.GetRootGameObjects())
        foreach (var transform in root.GetComponentsInChildren<Transform>(true))
        {
            var obj = transform.gameObject;
            if (GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(obj) != 0)
                throw new InvalidOperationException("Missing script on " + obj.name);
            foreach (var renderer in obj.GetComponents<Renderer>())
            foreach (var material in renderer.sharedMaterials)
                if (material == null || material.shader == null || material.shader.name == "Hidden/InternalErrorShader")
                    throw new InvalidOperationException("Missing material/shader on " + obj.name);
            var player = obj.GetComponent<ForestPlayer>();
            if (player != null)
            {
                players++;
                var playerSerialized = new SerializedObject(player);
                if (obj.GetComponent<CharacterController>() == null || playerSerialized.FindProperty("view").objectReferenceValue == null)
                    throw new InvalidOperationException("Incomplete player.");
                if (playerSerialized.FindProperty("maxCarriedWood").intValue <= 0)
                    throw new InvalidOperationException("Invalid player carrying capacity on " + obj.name);
            }
            var buildable = obj.GetComponent<ForestBuildable>();
            if (buildable != null)
            {
                if (string.IsNullOrEmpty(buildable.BuildId))
                    throw new InvalidOperationException("Missing buildable id on " + obj.name);
                if (!buildableIds.Add(buildable.BuildId))
                    throw new InvalidOperationException("Duplicate buildable id: " + buildable.BuildId);
            }
            var storage = obj.GetComponent<ForestWoodStorage>();
            if (storage != null)
            {
                if (string.IsNullOrEmpty(storage.StorageId))
                    throw new InvalidOperationException("Missing storage id on " + obj.name);
                if (!storageIds.Add(storage.StorageId))
                    throw new InvalidOperationException("Duplicate storage id: " + storage.StorageId);
                if (storage.Capacity <= 0)
                    throw new InvalidOperationException("Invalid storage capacity on " + obj.name);
                if (storage.Buildable == null)
                    throw new InvalidOperationException("Wood storage without a ForestBuildable: " + obj.name);
                if (!storage.Buildable.HasPrerequisite)
                    throw new InvalidOperationException("Wood storage buildable is missing its prerequisite: " + obj.name);
            }
            var ecology = obj.GetComponent<ForestEcologyController>();
            if (ecology != null)
            {
                var serializedEcology = new SerializedObject(ecology);
                if (serializedEcology.FindProperty("standSizeMeters").floatValue <= 0f ||
                    serializedEcology.FindProperty("cellSizeMeters").floatValue <= 0f)
                    throw new InvalidOperationException("Invalid ecology grid configuration on " + obj.name);
            }
            var spawner = obj.GetComponent<ForestTreeSpawner>();
            if (spawner != null)
            {
                spawners++;
                var serializedSpawner = new SerializedObject(spawner);
                if (serializedSpawner.FindProperty("defaultSpecies").objectReferenceValue == null ||
                    serializedSpawner.FindProperty("barkMaterial").objectReferenceValue == null ||
                    serializedSpawner.FindProperty("canopyPrefab").objectReferenceValue == null ||
                    serializedSpawner.FindProperty("forestParent").objectReferenceValue == null)
                    throw new InvalidOperationException("Incomplete tree spawner on " + obj.name);
            }
            var tree = obj.GetComponent<ForestTree>();
            if (tree != null)
            {
                var serializedTree = new SerializedObject(tree);
                var trunkTransform = serializedTree.FindProperty("trunk").objectReferenceValue as Transform;
                var canopyTransform = serializedTree.FindProperty("canopy").objectReferenceValue as Transform;
                string treeId = serializedTree.FindProperty("treeId").stringValue;
                var species = serializedTree.FindProperty("species").objectReferenceValue as TreeSpeciesDefinition;
                if (string.IsNullOrEmpty(treeId) || trunkTransform == null || canopyTransform == null)
                    throw new InvalidOperationException("Incomplete tree state on " + obj.name);
                if (!treeIds.Add(treeId))
                    throw new InvalidOperationException("Duplicate tree id: " + treeId);
                if (species == null || string.IsNullOrEmpty(species.SpeciesId))
                    throw new InvalidOperationException("Missing species on " + obj.name);
                if (serializedTree.FindProperty("ageYears").intValue < 0)
                    throw new InvalidOperationException("Invalid tree age on " + obj.name);
                if (validatedSpecies.Add(species.SpeciesId))
                    ValidateSpecies(species);
                if (Mathf.Abs(trunkTransform.localPosition.x) > 0.001f || Mathf.Abs(trunkTransform.localPosition.z) > 0.001f ||
                    Mathf.Abs(canopyTransform.localPosition.x) > 0.001f || Mathf.Abs(canopyTransform.localPosition.z) > 0.001f)
                    throw new InvalidOperationException("Tree children are not centered on the root: " + obj.name);
                if (serializedTree.FindProperty("heightMeters").floatValue <= 0f ||
                    serializedTree.FindProperty("diameterCm").floatValue <= 0f ||
                    serializedTree.FindProperty("crownRadiusMeters").floatValue <= 0f)
                    throw new InvalidOperationException("Tree simulation data is not positive: " + obj.name);
            }
            cameras += obj.GetComponents<Camera>().Length;
            listeners += obj.GetComponents<AudioListener>().Length;
            saveControllers += obj.GetComponents<ForestSaveController>().Length;
            ecologyControllers += obj.GetComponents<ForestEcologyController>().Length;
        }
        if (players != 1 || cameras != 1 || listeners != 1) throw new InvalidOperationException("Unexpected player/camera/listener count.");
        if (saveControllers != 1) throw new InvalidOperationException("Unexpected save controller count.");
        if (ecologyControllers != 1) throw new InvalidOperationException("Unexpected ecology controller count.");
        if (spawners != 1) throw new InvalidOperationException("Unexpected tree spawner count.");
        Debug.Log("FOREST_VALIDATED: no missing scripts, materials or player references; one player, camera and listener.");
    }

    private static void ValidateSpecies(TreeSpeciesDefinition species)
    {
        if (species.PotentialDbhGrowthCmPerYear <= 0f || species.MaxDbhCm <= 1f ||
            species.PotentialHeightGrowthMPerYear <= 0f || species.MaxHeightM <= 1f ||
            species.Ci50 <= 0f ||
            species.SeedDispersalScaleM <= 0f || species.SeedSaturationS50 <= 0f ||
            species.PromotionHeightM <= 0f)
            throw new InvalidOperationException("Sitka growth data is incomplete on species " + species.SpeciesId);
        var serialized = new SerializedObject(species);
        var light = serialized.FindProperty("lightResponseLight");
        var factor = serialized.FindProperty("lightResponseFactor");
        if (light == null || factor == null || light.arraySize < 2 || light.arraySize != factor.arraySize)
            throw new InvalidOperationException("Malformed species light curve on " + species.SpeciesId);
    }
}
