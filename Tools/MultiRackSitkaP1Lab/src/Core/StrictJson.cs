using System.Text;
using System.Text.Json;

namespace SitkaP1.Core;

// A deliberately small JSON model for P1 files. Strictness rules (spec §9.2 T-07):
//   - UTF-8, no BOM, no comments, no trailing commas, depth <= 16;
//   - duplicate keys refused; numbers must be base-10 integers within ±(2^53 − 1) (authoritative floats are bit strings);
//   - strings limited to printable ASCII without '"' or '\' so the canonical form never needs escapes.
// Exact key sets and key ORDER are enforced by re-serializing the typed model and comparing bytes (see DerivedManifest).

public abstract class JNode { }
public sealed class JObj : JNode
{
    public readonly List<KeyValuePair<string, JNode>> Props = new();
}
public sealed class JArr : JNode { public readonly List<JNode> Items = new(); }
public sealed class JStr : JNode { public string Value = ""; }
public sealed class JInt : JNode { public long Value; }
public sealed class JBool : JNode { public bool Value; }
public sealed class JNull : JNode { }

public static class StrictJson
{
    public const int MaxDepth = 16;
    public const long MaxSafeInteger = 9_007_199_254_740_991;

    public static JNode Parse(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            throw new P1Refusal("NotCanonical", "a byte-order mark is not allowed");
        var reader = new Utf8JsonReader(bytes, new JsonReaderOptions
        {
            CommentHandling = JsonCommentHandling.Disallow,
            AllowTrailingCommas = false,
            MaxDepth = MaxDepth,
        });
        try
        {
            if (!reader.Read()) throw new P1Refusal("JsonInvalid", "empty document");
            var root = ReadValue(ref reader);
            if (reader.Read()) throw new P1Refusal("JsonInvalid", "content after the root value");
            return root;
        }
        catch (JsonException e)
        {
            throw new P1Refusal("JsonInvalid", "malformed JSON: " + e.Message);
        }
    }

    private static JNode ReadValue(ref Utf8JsonReader r)
    {
        switch (r.TokenType)
        {
            case JsonTokenType.StartObject:
            {
                var obj = new JObj();
                var seen = new HashSet<string>(StringComparer.Ordinal);
                while (true)
                {
                    if (!r.Read()) throw new P1Refusal("JsonInvalid", "truncated object");
                    if (r.TokenType == JsonTokenType.EndObject) return obj;
                    string key = r.GetString()!;
                    CheckString(key);
                    if (!seen.Add(key)) throw new P1Refusal("DuplicateKey", $"duplicate key '{key}'");
                    if (!r.Read()) throw new P1Refusal("JsonInvalid", "truncated object");
                    obj.Props.Add(new(key, ReadValue(ref r)));
                }
            }
            case JsonTokenType.StartArray:
            {
                var arr = new JArr();
                while (true)
                {
                    if (!r.Read()) throw new P1Refusal("JsonInvalid", "truncated array");
                    if (r.TokenType == JsonTokenType.EndArray) return arr;
                    arr.Items.Add(ReadValue(ref r));
                }
            }
            case JsonTokenType.String:
            {
                string s = r.GetString()!;
                CheckString(s);
                return new JStr { Value = s };
            }
            case JsonTokenType.Number:
            {
                foreach (byte b in r.ValueSpan)
                    if (b is (byte)'.' or (byte)'e' or (byte)'E')
                        throw new P1Refusal("NumberNotInteger", "JSON numbers must be integers; floats are stored as bit strings");
                if (!r.TryGetInt64(out long v) || v > MaxSafeInteger || v < -MaxSafeInteger)
                    throw new P1Refusal("NumberOutOfRange", "integer outside ±(2^53 − 1)");
                return new JInt { Value = v };
            }
            case JsonTokenType.True: return new JBool { Value = true };
            case JsonTokenType.False: return new JBool { Value = false };
            case JsonTokenType.Null: return new JNull();
            default: throw new P1Refusal("JsonInvalid", "unexpected token " + r.TokenType);
        }
    }

    public static void CheckString(string s)
    {
        foreach (char c in s)
            if (c < 0x20 || c > 0x7E || c == '"' || c == '\\')
                throw new P1Refusal("StringNotAllowed", "strings must be printable ASCII without quote or backslash");
    }
}

/// <summary>Typed, exact-shape access to a parsed node. Every accessor refuses on a wrong type.</summary>
public static class J
{
    /// <summary>Returns the properties of <paramref name="node"/> by name, requiring exactly <paramref name="keys"/> (any order here; order is enforced by the canonical byte check).</summary>
    public static Dictionary<string, JNode> Keys(JNode node, string what, params string[] keys)
    {
        if (node is not JObj o) throw new P1Refusal("SchemaInvalid", $"{what} must be an object");
        var map = new Dictionary<string, JNode>(StringComparer.Ordinal);
        foreach (var kv in o.Props) map[kv.Key] = kv.Value;
        foreach (var k in keys)
            if (!map.ContainsKey(k)) throw new P1Refusal("MissingKey", $"{what}.{k} is missing");
        foreach (var k in map.Keys)
            if (Array.IndexOf(keys, k) < 0) throw new P1Refusal("UnknownKey", $"{what}.{k} is not a known key");
        return map;
    }

    public static string Str(JNode n, string what) =>
        n is JStr s ? s.Value : throw new P1Refusal("SchemaInvalid", $"{what} must be a string");

    public static string? StrOrNull(JNode n, string what) =>
        n is JNull ? null : Str(n, what);

    public static long Int(JNode n, string what) =>
        n is JInt i ? i.Value : throw new P1Refusal("SchemaInvalid", $"{what} must be an integer");

    public static long? IntOrNull(JNode n, string what) =>
        n is JNull ? null : Int(n, what);

    public static JArr Arr(JNode n, string what) =>
        n as JArr ?? throw new P1Refusal("SchemaInvalid", $"{what} must be an array");

    public static uint F32Bits(JNode n, string what)
    {
        var o = Keys(n, what, "k", "b");
        if (Str(o["k"], what + ".k") != "f32") throw new P1Refusal("FloatKindInvalid", $"{what}.k must be \"f32\"");
        return F32.ParseHex(Str(o["b"], what + ".b"));
    }
}

/// <summary>Canonical compact writer: fixed key order chosen by the caller, no whitespace, no escapes.</summary>
public sealed class CanonicalWriter
{
    private readonly StringBuilder _sb = new();
    private readonly Stack<bool> _first = new();

    public CanonicalWriter BeginObject() { Comma(); _sb.Append('{'); _first.Push(true); return this; }
    public CanonicalWriter EndObject() { _first.Pop(); _sb.Append('}'); return this; }
    public CanonicalWriter BeginArray(string key) { Key(key); _sb.Append('['); _first.Push(true); return this; }
    public CanonicalWriter EndArray() { _first.Pop(); _sb.Append(']'); return this; }
    public CanonicalWriter BeginObject(string key) { Key(key); _sb.Append('{'); _first.Push(true); return this; }

    public CanonicalWriter Str(string key, string value) { Key(key); Quote(value); return this; }
    public CanonicalWriter StrOrNull(string key, string? value) { Key(key); if (value is null) _sb.Append("null"); else Quote(value); return this; }
    public CanonicalWriter Int(string key, long value) { Key(key); _sb.Append(value.ToString(System.Globalization.CultureInfo.InvariantCulture)); return this; }
    public CanonicalWriter IntOrNull(string key, long? value) { Key(key); _sb.Append(value is null ? "null" : value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)); return this; }
    public CanonicalWriter F32(string key, uint bits) { BeginObject(key).Str("k", "f32").Str("b", Core.F32.Hex(bits)).EndObject(); return this; }
    public CanonicalWriter StrArray(string key, IEnumerable<string> values)
    {
        BeginArray(key);
        foreach (var v in values) { Comma(); Quote(v); }
        return EndArray();
    }

    // Commas are written only before an object key or before an array element (BeginObject() without a key, StrArray items).
    // A value written after a key never writes a comma itself.
    private void Key(string key)
    {
        Comma();
        Quote(key);
        _sb.Append(':');
    }

    private void Comma()
    {
        if (_first.Count == 0) return;
        if (_first.Peek()) { _first.Pop(); _first.Push(false); }
        else _sb.Append(',');
    }

    private void Quote(string s)
    {
        StrictJson.CheckString(s);
        _sb.Append('"').Append(s).Append('"');
    }

    public byte[] ToBytes() => Encoding.UTF8.GetBytes(_sb.ToString());
    public override string ToString() => _sb.ToString();
}
