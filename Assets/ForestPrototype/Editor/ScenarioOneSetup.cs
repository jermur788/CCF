using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class ScenarioOneSetup
{
    public const string DefinitionPath = "Assets/ForestPrototype/ScenarioOne/ScenarioOne.asset";

    [MenuItem("Tools/Forest Prototype/Configure Scenario One")]
    public static void Configure()
    {
        ScenarioOneDefinition definition = AssetDatabase.LoadAssetAtPath<ScenarioOneDefinition>(DefinitionPath);
        if (definition == null)
        {
            definition = ScriptableObject.CreateInstance<ScenarioOneDefinition>();
            AssetDatabase.CreateAsset(definition, DefinitionPath);
        }
        ConfigureScene(ForestSceneBuilder.ScenePath, definition);
        ConfigureScene("Assets/Scenes/MixedSpeciesTest.unity", definition);
        AssetDatabase.SaveAssets();
        ForestSceneBuilder.Validate();
        Debug.Log("SCENARIO_ONE_CONFIGURED: definition and annual management shell wired.");
    }

    private static void ConfigureScene(string path, ScenarioOneDefinition definition)
    {
        Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        ForestEcologyController ecology = UnityEngine.Object.FindFirstObjectByType<ForestEcologyController>();
        if (ecology == null)
            throw new InvalidOperationException("No ecology controller in " + path);
        ScenarioOneManager manager = ecology.GetComponent<ScenarioOneManager>();
        if (manager == null)
            manager = ecology.gameObject.AddComponent<ScenarioOneManager>();
        manager.ConfigureDefinition(definition);
        EditorUtility.SetDirty(manager);
        EditorSceneManager.SaveScene(scene);
    }
}
