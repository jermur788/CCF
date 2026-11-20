using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Explicit entry for the personal Editor test. Historical startup/build scenes stay intact.
public static class Sen1CompactPlaytestEntry
{
    public const string ScenePath = "Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity";

    [MenuItem("CCF/Player Test/Open Compact30")]
    public static void Open()
    {
        OpenScene();
    }

    [MenuItem("CCF/Player Test/Play Compact30")]
    public static void Play()
    {
        if (OpenScene()) EditorApplication.isPlaying = true;
    }

    [MenuItem("CCF/Player Test/Open Compact30", true)]
    [MenuItem("CCF/Player Test/Play Compact30", true)]
    private static bool CanOpen() => !EditorApplication.isPlayingOrWillChangePlaymode;

    private static bool OpenScene()
    {
        if (!CanOpen()) return false;
        if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
        {
            Debug.LogError("Compact30 player-test scene is missing: " + ScenePath);
            return false;
        }
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
        EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        return true;
    }
}
