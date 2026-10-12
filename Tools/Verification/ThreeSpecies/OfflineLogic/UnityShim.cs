// A deliberately tiny stand-in for the few UnityEngine members that the persisted Scenario One record classes use,
// so the REAL record classes can be compiled and RUN on stock .NET. (The Unity reference assemblies on NuGet type-check
// but cannot execute outside Unity; even Vector3 fails to load.) Nothing here is used by the game.
//   JsonUtility is implemented with System.Text.Json (public fields), which is enough for a faithful field-for-field copy.
using System;
using System.Text.Json;

namespace UnityEngine
{
    [Serializable] public struct Vector3 { public float x, y, z; public Vector3(float x, float y, float z) { this.x = x; this.y = y; this.z = z; } }
    [Serializable] public struct Vector2 { public float x, y; public Vector2(float x, float y) { this.x = x; this.y = y; } }

    public static class Mathf
    {
        public static float Max(float a, float b) => a > b ? a : b;
        public static int Max(int a, int b) => a > b ? a : b;
        public static float Min(float a, float b) => a < b ? a : b;
        public static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
        public static float Clamp(float v, float lo, float hi) => v < lo ? lo : v > hi ? hi : v;
        public static int Clamp(int v, int lo, int hi) => v < lo ? lo : v > hi ? hi : v;
        public static float Pow(float a, float b) => (float)Math.Pow(a, b);
    }

    [AttributeUsage(AttributeTargets.Field)] public sealed class SerializeField : Attribute { }

    public static class JsonUtility
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { IncludeFields = true };
        public static string ToJson(object value) => JsonSerializer.Serialize(value, value.GetType(), Options);
        public static T FromJson<T>(string json) => JsonSerializer.Deserialize<T>(json, Options);
    }
}
