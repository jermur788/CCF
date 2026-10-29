// Offline collaborators only. ForestTree's pruning code is extracted verbatim by run_pruning_check.py.
// No scene/manager execution, Unity serialization or real visuals are simulated here.
using System;
using System.Text.Json;
using CCF.Forestry.TimberYield;

namespace UnityEngine
{
    public class ScriptableObject { }
    [AttributeUsage(AttributeTargets.Field)] public sealed class SerializeField : Attribute { }
    [AttributeUsage(AttributeTargets.Field)] public sealed class MinAttribute : Attribute { public MinAttribute(float v) { } }
    [AttributeUsage(AttributeTargets.Field)] public sealed class RangeAttribute : Attribute { public RangeAttribute(float a, float b) { } }
    [AttributeUsage(AttributeTargets.Field)] public sealed class HeaderAttribute : Attribute { public HeaderAttribute(string s) { } }
    [AttributeUsage(AttributeTargets.Field)] public sealed class TooltipAttribute : Attribute { public TooltipAttribute(string s) { } }
    [AttributeUsage(AttributeTargets.Class)] public sealed class CreateAssetMenuAttribute : Attribute { public string fileName, menuName; }
    public struct Vector3 { public float x, y, z; }
    public static class Mathf
    {
        public static float Max(float a, float b) => Math.Max(a, b);
        public static int Max(int a, int b) => Math.Max(a, b);
        public static float Clamp01(float a) => Math.Clamp(a, 0, 1);
        public static int Clamp(int a, int b, int c) => Math.Clamp(a, b, c);
        public static float Pow(float a, float b) => (float)Math.Pow(a, b);
        public static int RoundToInt(float a) => (int)Math.Round(a);
    }
    public static class JsonUtility
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { IncludeFields = true, IgnoreReadOnlyProperties = true };
        public static string ToJson(object o) => JsonSerializer.Serialize(o, Options);
        public static T FromJson<T>(string s) => JsonSerializer.Deserialize<T>(s, Options);
    }
}

public sealed class TreeSpeciesDefinition { public string SpeciesId; }
public static class StandGeometryModel
{
    public const int Legacy40 = 0;
    public static bool IsKnown(int m) => m == 0 || m == 1;
    public static float StandSizeMeters(int m) => m == 1 ? 80f : 40f;
}
public static class ScenarioOneUiFacts { public static string SquareLabel(int cell) => "#" + cell; }
public static class WindthrowSalvage
{
    // Salvage is outside this bounded gate. Fail loudly if any fixture accidentally enters it.
    public static StemMeasurements Measurements(ScenarioDeadwoodRecord record, int year) => throw new NotSupportedException("Salvage not tested offline");
}
