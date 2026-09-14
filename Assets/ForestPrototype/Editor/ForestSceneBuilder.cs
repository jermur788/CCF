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
            var ground = Material("Forest Ground", new Color(0.22f, 0.29f, 0.12f));
            var bark = Material("Bark", new Color(0.24f, 0.13f, 0.065f));
            var leaves = Material("Leaves", new Color(0.10f, 0.29f, 0.12f));
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
                var canopy = Primitive("Canopy", PrimitiveType.Sphere, new Vector3(px, height, pz), new Vector3(3.3f, 3.6f, 3.3f), leaves, tree.transform);
                UnityEngine.Object.DestroyImmediate(canopy.GetComponent<Collider>());
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
        int players = 0, cameras = 0, listeners = 0, saveControllers = 0;
        var treeIds = new HashSet<string>();
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
                if (obj.GetComponent<CharacterController>() == null || new SerializedObject(player).FindProperty("view").objectReferenceValue == null)
                    throw new InvalidOperationException("Incomplete player.");
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
        }
        if (players != 1 || cameras != 1 || listeners != 1) throw new InvalidOperationException("Unexpected player/camera/listener count.");
        if (saveControllers != 1) throw new InvalidOperationException("Unexpected save controller count.");
        Debug.Log("FOREST_VALIDATED: no missing scripts, materials or player references; one player, camera and listener.");
    }
}
