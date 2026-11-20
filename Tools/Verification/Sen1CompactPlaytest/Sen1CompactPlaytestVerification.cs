using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Disposable wrapper around the unchanged 71-check E2 gate. Copy both harnesses into
// Assets/ForestPrototype for the run and remove both scripts and metas afterwards.
public static class Sen1CompactPlaytestVerification
{
    public static void Begin()
    {
        try
        {
            // Use the actual player-test helper instead of bypassing the entry being qualified.
            Type entry = AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetType("Sen1CompactPlaytestEntry")).FirstOrDefault(t => t != null);
            Require(entry != null, "Editor entry imported");
            entry.GetMethod("Open").Invoke(null, null);
            Scene scene = SceneManager.GetActiveScene();
            Require(scene.path == "Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity", "entry opened exact Compact scene");
            Require(!scene.isDirty, "opening did not modify serialized scene");
            GameObject world = GameObject.Find("Compact30World");
            Require(world != null && world.transform.Find("Trees").childCount == 175, "175 authored tree children");
            Require(UnityEngine.Object.FindObjectsByType<Collider>(FindObjectsSortMode.None).Length == 6, "six scene colliders");
            foreach (GameObject root in scene.GetRootGameObjects())
                foreach (Transform item in root.GetComponentsInChildren<Transform>(true))
                {
                    Require(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(item.gameObject) == 0, "no missing script: " + item.name);
                    MeshFilter filter = item.GetComponent<MeshFilter>();
                    if (filter != null) Require(filter.sharedMesh != null, "mesh resolves: " + item.name);
                    Renderer renderer = item.GetComponent<Renderer>();
                    if (renderer != null)
                        foreach (Material material in renderer.sharedMaterials)
                            Require(material != null && material.shader != null && material.shader.isSupported,
                                "material/shader resolves: " + item.name);
                }
            Require(UnityEngine.Object.FindFirstObjectByType<ForestSaveController>() == null, "no scene save controller");
            Debug.Log("COMPACT30_PLAYTEST_ENTRY_PASS");
            // Its normal Play entry starts the unchanged E2 harness installed alongside this file.
            entry.GetMethod("Play").Invoke(null, null);
        }
        catch (Exception exception)
        {
            Debug.LogError("COMPACT30_PLAYTEST_ENTRY_FAIL " + exception);
            EditorApplication.Exit(1);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
}
