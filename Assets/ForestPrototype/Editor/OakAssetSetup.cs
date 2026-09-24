using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class OakAssetSetup
{
    private const string Root = "Assets/ForestPrototype/Art/Trees/Sessile_Oak_Three_Stages";
    private const string Models = Root + "/Models";
    private const string Materials = Root + "/Materials";
    private const string TexturePath = Root + "/Textures/Oak_Bark_Albedo.png";
    private const string Prefabs = "Assets/ForestPrototype/Prefabs";
    private const string SpeciesPath = "Assets/ForestPrototype/Species/SessileOak.asset";

    public const string SaplingPrefabPath = Prefabs + "/SessileOakSapling01.prefab";
    public const string YoungPrefabPath = Prefabs + "/SessileOakYoung01.prefab";
    public const string MaturePrefabPath = Prefabs + "/SessileOakMature01.prefab";

    [MenuItem("Tools/Forest Prototype/Build Sessile Oak Visuals")]
    public static void BuildAndWire()
    {
        Directory.CreateDirectory(Materials);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
        ConfigureModels();
        Texture2D barkTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(TexturePath);
        if (barkTexture == null)
            throw new InvalidOperationException("Missing Oak bark texture at " + TexturePath);

        Material bark = Material("Oak_Bark", new Color(0.32f, 0.275f, 0.205f), barkTexture);
        Material[] leaves =
        {
            Material("Oak_Leaf_01", new Color(0.085f, 0.19f, 0.028f), null),
            Material("Oak_Leaf_02", new Color(0.14f, 0.27f, 0.045f), null),
            Material("Oak_Leaf_03", new Color(0.19f, 0.32f, 0.055f), null),
            Material("Oak_Leaf_04", new Color(0.105f, 0.22f, 0.038f), null)
        };

        BuildStage("Sessile_Oak_Sapling_01", "SessileOakSapling01", 2f, SaplingPrefabPath, bark, leaves);
        BuildStage("Sessile_Oak_Young_01", "SessileOakYoung01", 8f, YoungPrefabPath, bark, leaves);
        BuildStage("Sessile_Oak_Mature_01", "SessileOakMature01", 20f, MaturePrefabPath, bark, leaves);
        AssetDatabase.SaveAssets();

        WireScene(ForestSceneBuilder.ScenePath);
        WireScene("Assets/Scenes/MixedSpeciesTest.unity");
        ValidateOakAssets();
        ForestSceneBuilder.Validate();
        Debug.Log("OAK_VISUALS_BUILT: sapling, young and mature LOD prefabs created and wired.");
    }

    private static void ConfigureModels()
    {
        foreach (string path in ModelPaths())
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null)
                throw new InvalidOperationException("Oak FBX did not import: " + path);
            importer.importAnimation = false;
            importer.addCollider = false;
            importer.isReadable = false;
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.SaveAndReimport();
        }
    }

    private static IEnumerable<string> ModelPaths()
    {
        string[] stages = { "Sessile_Oak_Sapling_01", "Sessile_Oak_Young_01", "Sessile_Oak_Mature_01" };
        foreach (string stage in stages)
            for (int lod = 0; lod < 3; lod++)
                yield return Models + "/" + stage + "_LOD" + lod + ".fbx";
    }

    private static Material Material(string name, Color color, Texture2D texture)
    {
        string path = Materials + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                throw new InvalidOperationException("URP Lit shader is unavailable.");
            material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
        }
        material.SetColor("_BaseColor", color);
        material.SetColor("_Color", color);
        material.SetFloat("_Smoothness", 0.05f);
        material.SetFloat("_Metallic", 0f);
        if (texture != null)
        {
            material.SetTexture("_BaseMap", texture);
            material.SetTexture("_MainTex", texture);
        }
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void BuildStage(string modelPrefix, string prefabName, float authoredHeight,
        string prefabPath, Material bark, Material[] leaves)
    {
        var root = new GameObject(prefabName);
        var lods = new LOD[3];
        float[] transitions = { 0.55f, 0.20f, 0.04f };
        try
        {
            for (int lod = 0; lod < 3; lod++)
            {
                string modelPath = Models + "/" + modelPrefix + "_LOD" + lod + ".fbx";
                GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (source == null)
                    throw new InvalidOperationException("Missing Oak model " + modelPath);
                GameObject instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
                if (instance == null)
                    throw new InvalidOperationException("Could not instantiate Oak model " + modelPath);
                instance.name = "LOD" + lod;
                instance.transform.SetParent(root.transform, false);
                instance.transform.localPosition = Vector3.zero;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = Vector3.one / authoredHeight;
                Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
                if (renderers.Length != 2)
                    throw new InvalidOperationException($"{modelPath} expected two renderers, found {renderers.Length}");
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.name.IndexOf("Bark", StringComparison.OrdinalIgnoreCase) >= 0)
                        renderer.sharedMaterials = new[] { bark };
                    else if (renderer.name.IndexOf("Foliage", StringComparison.OrdinalIgnoreCase) >= 0)
                        renderer.sharedMaterials = leaves;
                    else
                        throw new InvalidOperationException("Unrecognized Oak mesh " + renderer.name);
                }
                lods[lod] = new LOD(transitions[lod], renderers) { fadeTransitionWidth = 0.1f };
            }
            var group = root.AddComponent<LODGroup>();
            group.fadeMode = LODFadeMode.CrossFade;
            group.animateCrossFading = false;
            group.SetLODs(lods);
            group.RecalculateBounds();
            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static void WireScene(string scenePath)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        ForestTreeSpawner spawner = null;
        foreach (GameObject root in scene.GetRootGameObjects())
        {
            spawner = root.GetComponentInChildren<ForestTreeSpawner>(true);
            if (spawner != null)
                break;
        }
        if (spawner == null)
            throw new InvalidOperationException("No tree spawner in " + scenePath);
        TreeSpeciesDefinition oak = AssetDatabase.LoadAssetAtPath<TreeSpeciesDefinition>(SpeciesPath);
        if (oak == null)
            throw new InvalidOperationException("Missing Oak species asset.");
        var serialized = new SerializedObject(spawner);
        SerializedProperty sets = serialized.FindProperty("speciesVisualSets");
        int oakIndex = -1;
        for (int i = 0; i < sets.arraySize; i++)
        {
            var candidate = sets.GetArrayElementAtIndex(i).FindPropertyRelative("species").objectReferenceValue as TreeSpeciesDefinition;
            if (candidate != null && candidate.SpeciesId == oak.SpeciesId)
            {
                oakIndex = i;
                break;
            }
        }
        if (oakIndex < 0)
        {
            oakIndex = sets.arraySize;
            sets.arraySize++;
        }
        SerializedProperty element = sets.GetArrayElementAtIndex(oakIndex);
        element.FindPropertyRelative("species").objectReferenceValue = oak;
        element.FindPropertyRelative("seedlingVisualPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>(SaplingPrefabPath);
        element.FindPropertyRelative("poleVisualPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>(YoungPrefabPath);
        element.FindPropertyRelative("matureVisualPrefab").objectReferenceValue =
            AssetDatabase.LoadAssetAtPath<GameObject>(MaturePrefabPath);
        serialized.ApplyModifiedPropertiesWithoutUndo();
        EditorSceneManager.SaveScene(scene);
    }

    private static void ValidateOakAssets()
    {
        foreach (string path in new[] { SaplingPrefabPath, YoungPrefabPath, MaturePrefabPath })
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
                throw new InvalidOperationException("Missing Oak prefab " + path);
            LODGroup group = prefab.GetComponent<LODGroup>();
            if (group == null || group.GetLODs().Length != 3)
                throw new InvalidOperationException("Oak prefab lacks three LOD levels: " + path);
            foreach (LOD lod in group.GetLODs())
            {
                if (lod.renderers.Length != 2)
                    throw new InvalidOperationException("Oak LOD does not contain bark and foliage: " + path);
                foreach (Renderer renderer in lod.renderers)
                    foreach (Material material in renderer.sharedMaterials)
                        if (material == null || material.shader == null)
                            throw new InvalidOperationException("Oak LOD has a missing material: " + path);
            }
            GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
                throw new InvalidOperationException("Could not instantiate Oak prefab for validation: " + path);
            try
            {
                Renderer[] firstLod = instance.GetComponent<LODGroup>().GetLODs()[0].renderers;
                Bounds bounds = firstLod[0].bounds;
                for (int i = 1; i < firstLod.Length; i++)
                    bounds.Encapsulate(firstLod[i].bounds);
                if (bounds.size.y < 0.95f || bounds.size.y > 1.05f || Mathf.Abs(bounds.min.y) > 0.03f)
                    throw new InvalidOperationException($"Oak prefab is not normalized and grounded: {path}, bounds={bounds}");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }
    }
}
