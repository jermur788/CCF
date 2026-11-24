using System.Collections;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core;

namespace SitkaP1.Phase1Save.Tests;

/// <summary>Minimal dependency-free test runner (same style as Core.Tests' T): named actions, expected refusal codes.</summary>
public static class T
{
    private static readonly List<(string Name, Action Body)> Tests = new();
    public static void Add(string name, Action body) => Tests.Add((name, body));

    public static P1Refusal Refuses(string code, Action a)
    {
        try { a(); }
        catch (P1Refusal e)
        {
            if (e.Code == code) return e;
            throw new Exception($"expected refusal {code}, got {e.Code}: {e.Message}");
        }
        throw new Exception($"expected refusal {code}, but the call succeeded");
    }

    /// <summary>
    /// Requires <c>SaveInconsistent</c> AND a message naming the intended rule, so a mutation test cannot pass because some
    /// other, unrelated rule happened to refuse the edited save.
    /// </summary>
    public static P1Refusal RefusesRule(string ruleFragment, Action a)
    {
        var e = Refuses("SaveInconsistent", a);
        if (!e.Message.Contains(ruleFragment, StringComparison.Ordinal))
            throw new Exception($"refused by the wrong rule: expected one naming '{ruleFragment}', got: {e.Message}");
        return e;
    }

    /// <summary>Requires a typed <see cref="P1Refusal"/> of any code (never an untyped exception, never success).</summary>
    public static P1Refusal RefusesAny(Action a)
    {
        try { a(); }
        catch (P1Refusal e) { return e; }
        throw new Exception("expected a typed refusal, but the call succeeded");
    }

    public static void True(bool ok, string what) { if (!ok) throw new Exception("assertion failed: " + what); }

    public static void Eq<TV>(TV expected, TV actual, string what)
    {
        if (!EqualityComparer<TV>.Default.Equals(expected, actual)) throw new Exception($"{what}: expected {expected}, got {actual}");
    }

    public static void Bytes(byte[] expected, byte[] actual, string what)
    {
        if (!expected.AsSpan().SequenceEqual(actual)) throw new Exception(what + ": byte arrays differ");
    }

    public static int RunAll()
    {
        int failed = 0;
        foreach (var (name, body) in Tests)
        {
            try { body(); Console.WriteLine("PASS " + name); }
            catch (Exception e) { failed++; Console.WriteLine("FAIL " + name + " — " + e.Message); }
        }
        Console.WriteLine($"{Tests.Count - failed}/{Tests.Count} passed, {failed} failed");
        return failed == 0 ? 0 : 1;
    }
}

/// <summary>
/// Structural deep equality by reflection over public properties, independent of the codec: lists element by element,
/// float and double by exact bit pattern (so -0 ≠ +0), runtime types must match. Reports the first differing path.
/// </summary>
public static class Deep
{
    public static void Equal(object? expected, object? actual, string path = "$")
    {
        if (expected is null || actual is null)
        {
            if (expected is not null || actual is not null) throw new Exception($"{path}: one side is null");
            return;
        }
        var type = expected.GetType();
        if (type != actual.GetType()) throw new Exception($"{path}: type {type.Name} vs {actual.GetType().Name}");
        switch (expected)
        {
            case float f:
                if (BitConverter.SingleToUInt32Bits(f) != BitConverter.SingleToUInt32Bits((float)actual))
                    throw new Exception($"{path}: float bits differ");
                return;
            case double d:
                if (BitConverter.DoubleToUInt64Bits(d) != BitConverter.DoubleToUInt64Bits((double)actual))
                    throw new Exception($"{path}: double bits differ");
                return;
            case string or bool or int or long or Enum:
                if (!expected.Equals(actual)) throw new Exception($"{path}: '{expected}' vs '{actual}'");
                return;
            case IEnumerable list:
            {
                var a = list.Cast<object?>().ToList();
                var b = ((IEnumerable)actual).Cast<object?>().ToList();
                if (a.Count != b.Count) throw new Exception($"{path}: count {a.Count} vs {b.Count}");
                for (int i = 0; i < a.Count; i++) Equal(a[i], b[i], $"{path}[{i}]");
                return;
            }
        }
        foreach (var p in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (p.GetIndexParameters().Length > 0) continue;
            Equal(p.GetValue(expected), p.GetValue(actual), path + "." + p.Name);
        }
    }
}

/// <summary>Byte-level mutation helpers over a canonical save, including resealing the checksum after an edit.</summary>
public static class Mut
{
    public static string Text(byte[] bytes) => Encoding.UTF8.GetString(bytes);

    /// <summary>Replaces the first occurrence of <paramref name="from"/>; the anchor must exist.</summary>
    public static string ReplaceFirst(string s, string from, string to)
    {
        int i = s.IndexOf(from, StringComparison.Ordinal);
        if (i < 0) throw new Exception("mutation anchor not found: " + from);
        return s[..i] + to + s[(i + from.Length)..];
    }

    /// <summary>Recomputes the checksum of an edited canonical save so the targeted check, not the checksum, is what refuses.</summary>
    public static byte[] Reseal(string json)
    {
        const string h = "\"header\":", p = ",\"payload\":", c = ",\"checksum\":{";
        int hi = json.IndexOf(h, StringComparison.Ordinal) + h.Length;
        int pi = json.IndexOf(p, StringComparison.Ordinal);
        int ci = json.LastIndexOf(c, StringComparison.Ordinal);
        string header = json[hi..pi];
        string payload = json[(pi + p.Length)..ci];
        string sum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(header + payload))).ToLowerInvariant();
        const string v = "\"value\":\"";
        int vi = json.IndexOf(v, ci, StringComparison.Ordinal) + v.Length;
        return Encoding.UTF8.GetBytes(json[..vi] + sum + json[(vi + 64)..]);
    }

    public static byte[] EditAndReseal(byte[] bytes, string from, string to) => Reseal(ReplaceFirst(Text(bytes), from, to));
}
