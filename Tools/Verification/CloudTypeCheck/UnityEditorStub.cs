// Minimal UnityEditor surface for type-checking only (never shipped, never run).
using System;
using UnityEngine;
namespace UnityEditor
{
    public static class EditorApplication
    {
        public static bool isPlaying { get; set; }
        public static bool isUpdating => false;
        public static bool isCompiling => false;
        public static bool isPlayingOrWillChangePlaymode => false;
        public static event Action update;
        public static CallbackFunction delayCall;
        public delegate void CallbackFunction();
        public static void ExitPlaymode() { }
        public static void Exit(int code) { }
    }
    public static class EditorPrefs
    {
        public static bool GetBool(string key, bool defaultValue = false) => defaultValue;
        public static void SetBool(string key, bool value) { }
        public static string GetString(string key, string defaultValue = "") => defaultValue;
        public static void SetString(string key, string value) { }
        public static void DeleteKey(string key) { }
    }
    public static class AssetDatabase
    {
        public static T LoadAssetAtPath<T>(string path) where T : UnityEngine.Object => null;
        public static UnityEngine.Object LoadMainAssetAtPath(string path) => null;
        public static string GetAssetPath(UnityEngine.Object o) => "";
        public static string[] GetAllAssetPaths() => null;
        public static string[] GetDependencies(string path, bool recursive) => null;
        public static string[] GetDependencies(string[] pathNames, bool recursive) => new string[0];
        public static Type GetMainAssetTypeAtPath(string path) => null;
        public static string[] FindAssets(string filter) => new string[0];
        public static string GUIDToAssetPath(string guid) => "";
        public static void Refresh() { }
        public static void CreateAsset(UnityEngine.Object o, string path) { }
    }
    public static class SessionState
    {
        public static int GetInt(string key, int defaultValue = 0) => defaultValue;
        public static void SetInt(string key, int value) { }
        public static void EraseInt(string key) { }
        public static string GetString(string key, string defaultValue = "") => defaultValue;
        public static void SetString(string key, string value) { }
    }
    public static class EditorUtility { public static void UnloadUnusedAssetsImmediate() { } }
    public class Editor { }
    public class ScriptableSingleton<T> where T : class { public static T instance => null; }
    // Scene-inspection members used by the disposable Editor harnesses. Compile-only.
    public class SerializedObject
    {
        public SerializedObject(UnityEngine.Object target) { }
        public SerializedProperty FindProperty(string path) => null;
    }
    public class SerializedProperty { public float floatValue; }
    public class MonoScript : UnityEngine.Object { }
    public static class UnityStats { public static int triangles => 0; }
    public class EditorWindow : ScriptableObject
    {
        public static EditorWindow focusedWindow => null;
        public bool maximized { get; set; }
        public static T GetWindow<T>() where T : EditorWindow => null;
        public static T GetWindow<T>(Type t) where T : EditorWindow => null;
        public static T GetWindow<T>(bool utility, string title) where T : EditorWindow => null;
        public void Show() { }
        public void Focus() { }
        public void Repaint() { }
        public static EditorWindow GetWindow(Type t) => null;
    }
    [AttributeUsage(AttributeTargets.Method)] public sealed class InitializeOnLoadMethodAttribute : Attribute { }
}
namespace UnityEditor.SceneManagement
{
    public static class EditorSceneManager
    {
        public static UnityEngine.SceneManagement.Scene OpenScene(string path) => default;
        public static UnityEngine.SceneManagement.Scene OpenScene(string path, OpenSceneMode mode) => default;
    }
    public enum OpenSceneMode { Single, Additive }
}
