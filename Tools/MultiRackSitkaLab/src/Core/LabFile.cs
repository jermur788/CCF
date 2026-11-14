using System.Text;
using System.Text.Json;

namespace SitkaLab.Core;

internal abstract class JNode { }
internal sealed class JObj : JNode { public readonly List<KeyValuePair<string, JNode>> Props = new(); }
internal sealed class JArr : JNode { public readonly List<JNode> Items = new(); }
internal sealed class JStr : JNode { public string V = ""; }
internal sealed class JInt : JNode { public long V; }
internal sealed class JNull : JNode { }

/// <summary>Strict JSON reader: no duplicate keys, integers only, no comments/trailing commas, no BOM.</summary>
internal static class StrictJson
{
    public static JNode Parse(byte[] bytes)
    {
        if (bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF)
            throw new LabException("JsonInvalid", "UTF-8 BOM is not allowed");
        try
        {
            var reader = new Utf8JsonReader(bytes, new JsonReaderOptions { CommentHandling = JsonCommentHandling.Disallow, AllowTrailingCommas = false, MaxDepth = 16 });
            if (!reader.Read()) throw new LabException("JsonInvalid", "empty document");
            var node = ReadValue(ref reader);
            if (reader.Read()) throw new LabException("JsonInvalid", "trailing content after the root value");
            return node;
        }
        catch (JsonException ex)
        {
            throw new LabException("JsonInvalid", "malformed JSON: " + ex.Message);
        }
    }

    private static JNode ReadValue(ref Utf8JsonReader r)
    {
        switch (r.TokenType)
        {
            case JsonTokenType.StartObject:
            {
                var o = new JObj();
                var seen = new HashSet<string>(StringComparer.Ordinal);
                while (true)
                {
                    if (!r.Read()) throw new LabException("JsonInvalid", "unterminated object");
                    if (r.TokenType == JsonTokenType.EndObject) return o;
                    if (r.TokenType != JsonTokenType.PropertyName) throw new LabException("JsonInvalid", "expected property name");
                    string key = r.GetString()!;
                    if (!seen.Add(key)) throw new LabException("DuplicateKey", $"duplicate key '{key}'");
                    if (!r.Read()) throw new LabException("JsonInvalid", "unterminated object");
                    o.Props.Add(new KeyValuePair<string, JNode>(key, ReadValue(ref r)));
                }
            }
            case JsonTokenType.StartArray:
            {
                var a = new JArr();
                while (true)
                {
                    if (!r.Read()) throw new LabException("JsonInvalid", "unterminated array");
                    if (r.TokenType == JsonTokenType.EndArray) return a;
                    a.Items.Add(ReadValue(ref r));
                }
            }
            case JsonTokenType.String:
                return new JStr { V = r.GetString()! };
            case JsonTokenType.Number:
            {
                var span = r.ValueSpan;
                // Integers only: digits with optional leading '-', no '.', exponent, '+', or leading zeros; no "-0".
                int i = 0;
                if (span.Length > 0 && span[0] == (byte)'-') i = 1;
                if (span.Length - i < 1 || span.Length - i > 19) throw new LabException("NonIntegerNumber", "number is not a bounded integer");
                for (int k = i; k < span.Length; k++)
                    if (span[k] < (byte)'0' || span[k] > (byte)'9') throw new LabException("NonIntegerNumber", "fractional or exponent number is not allowed");
                if (span[i] == (byte)'0' && span.Length - i > 1) throw new LabException("NonIntegerNumber", "leading zeros are not allowed");
                if (i == 1 && span[1] == (byte)'0') throw new LabException("NonIntegerNumber", "negative zero is not allowed");
                if (!r.TryGetInt64(out long v)) throw new LabException("NumberOverflow", "integer overflows a 64-bit value");
                return new JInt { V = v };
            }
            case JsonTokenType.Null:
                return new JNull();
            default:
                throw new LabException("UnsupportedJsonValue", "booleans and other value kinds are not part of the lab schema");
        }
    }
}

/// <summary>Reads and writes lab files (checked-in fixtures and lab saves).</summary>
public static class LabFile
{
    /// <summary>
    /// Header gate: only the first three root properties are read. Anything that is not a v1 synthetic lab file
    /// (including CCF game-save shapes) is rejected here, before any forest or event content is parsed.
    /// </summary>
    public static void CheckHeader(byte[] bytes)
    {
        try
        {
            var r = new Utf8JsonReader(bytes, new JsonReaderOptions { CommentHandling = JsonCommentHandling.Disallow, MaxDepth = 4 });
            if (!r.Read() || r.TokenType != JsonTokenType.StartObject) throw Wrong("root is not a JSON object");
            ExpectString(ref r, "format", LabConstants.Format);
            if (!r.Read() || r.TokenType != JsonTokenType.PropertyName || r.GetString() != "labSchemaVersion") throw Wrong("second property must be labSchemaVersion");
            if (!r.Read() || r.TokenType != JsonTokenType.Number || !r.TryGetInt32(out int ver) || ver != LabConstants.SchemaVersion) throw Wrong("labSchemaVersion must be 1");
            ExpectString(ref r, "sourceClass", LabConstants.SourceClass);
        }
        catch (JsonException)
        {
            throw Wrong("header could not be read");
        }
    }

    private static LabException Wrong(string why) => new("WrongFormat", "Not a ccf-sitka-lab v1 Synthetic file: " + why);

    private static void ExpectString(ref Utf8JsonReader r, string key, string value)
    {
        if (!r.Read() || r.TokenType != JsonTokenType.PropertyName || r.GetString() != key) throw Wrong($"property '{key}' expected at this position");
        if (!r.Read() || r.TokenType != JsonTokenType.String || r.GetString() != value) throw Wrong($"{key} must be \"{value}\"");
    }

    public static byte[] Serialize(LabFileContent f) => CanonicalJson.File(f);

    /// <summary>
    /// Full strict parse of a lab file. Steps: size limit, header gate, strict JSON, typed schema, structural validation,
    /// then byte-for-byte canonical comparison. Throws LabException on any defect.
    /// </summary>
    public static LabFileContent Parse(byte[] bytes)
    {
        if (bytes.Length > LabConstants.MaxFileBytes) throw new LabException("FileTooLarge", "lab file exceeds the size limit");
        CheckHeader(bytes);
        var root = (StrictJson.Parse(bytes) as JObj) ?? throw new LabException("SchemaInvalid", "root must be an object");
        var f = new Reader(root).ToFile();
        SnapshotValidator.ValidateSnapshot(f.Snapshot);
        // Canonical byte equality also enforces compact form, key order, LF-only/no trailing whitespace and no BOM.
        var canon = Serialize(f);
        if (!canon.AsSpan().SequenceEqual(bytes))
            throw new LabException("NotCanonical", "file bytes are not the canonical v1 serialization of their content");
        if (CanonicalJson.SnapshotHash(f.Snapshot) != f.SnapshotHash)
            throw new LabException("HashMismatch", "snapshotHash does not match the SHA-256 of the canonical snapshot bytes");
        return f;
    }

    private sealed class Reader
    {
        private readonly JObj _root;
        public Reader(JObj root) { _root = root; }

        public LabFileContent ToFile()
        {
            var o = Keys(_root, "file", "format", "labSchemaVersion", "sourceClass", "fixtureId", "snapshot", "eventLog", "snapshotHash");
            // header values were already verified by CheckHeader
            var f = new LabFileContent
            {
                FixtureId = Str(o["fixtureId"], "fixtureId"),
                SnapshotHash = Str(o["snapshotHash"], "snapshotHash"),
            };
            if (!LabConstants.IsSafeId(f.FixtureId)) throw new LabException("SchemaInvalid", "fixtureId is not a safe ID");
            if (!LabConstants.IsSha256Hex(f.SnapshotHash)) throw new LabException("SchemaInvalid", "snapshotHash is not lowercase SHA-256 hex");
            f.Snapshot = Snapshot(Obj(o["snapshot"], "snapshot"));
            var evs = Arr(o["eventLog"], "eventLog");
            if (evs.Items.Count > LabConstants.MaxEvents) throw new LabException("SchemaInvalid", "too many events");
            foreach (var e in evs.Items) f.Events.Add(Event(Obj(e, "event")));
            return f;
        }

        private static Dictionary<string, JNode> Keys(JObj obj, string what, params string[] expected)
        {
            if (obj.Props.Count != expected.Length)
                throw new LabException("SchemaInvalid", $"{what}: expected exactly {expected.Length} properties ({string.Join(",", expected)})");
            var d = new Dictionary<string, JNode>(StringComparer.Ordinal);
            for (int i = 0; i < expected.Length; i++)
            {
                if (obj.Props[i].Key != expected[i])
                    throw new LabException("SchemaInvalid", $"{what}: property {i + 1} must be '{expected[i]}' but was '{obj.Props[i].Key}' (unknown/out-of-order keys are rejected)");
                d[expected[i]] = obj.Props[i].Value;
            }
            return d;
        }

        private static JObj Obj(JNode n, string what) => n as JObj ?? throw new LabException("SchemaInvalid", $"{what} must be an object");
        private static JArr Arr(JNode n, string what) => n as JArr ?? throw new LabException("SchemaInvalid", $"{what} must be an array");
        private static string Str(JNode n, string what) => (n as JStr)?.V ?? throw new LabException("SchemaInvalid", $"{what} must be a string");
        private static long Int(JNode n, string what) => (n as JInt)?.V ?? throw new LabException("SchemaInvalid", $"{what} must be an integer");

        private static List<string> IdList(JNode n, string what)
        {
            var list = new List<string>();
            foreach (var item in Arr(n, what).Items) list.Add(Str(item, what + "[]"));
            return list;
        }

        private static ForestSnapshot Snapshot(JObj o)
        {
            var d = Keys(o, "snapshot", "forestId", "inventoryYear", "experimentStep", "forestRevision", "adultThresholdMm", "standAreaPolygon", "siteProfile", "trees", "plans");
            var s = new ForestSnapshot
            {
                ForestId = Str(d["forestId"], "forestId"),
                InventoryYear = Int(d["inventoryYear"], "inventoryYear"),
                ExperimentStep = Int(d["experimentStep"], "experimentStep"),
                ForestRevision = Int(d["forestRevision"], "forestRevision"),
                AdultThresholdMm = Int(d["adultThresholdMm"], "adultThresholdMm"),
            };
            var poly = Arr(d["standAreaPolygon"], "standAreaPolygon");
            if (poly.Items.Count > LabConstants.MaxPolygonVertices) throw new LabException("SchemaInvalid", "polygon has too many vertices");
            foreach (var v in poly.Items)
            {
                var vd = Keys(Obj(v, "vertex"), "vertex", "xMm", "yMm");
                s.Polygon.Add(new Vertex(Int(vd["xMm"], "xMm"), Int(vd["yMm"], "yMm")));
            }
            var site = Keys(Obj(d["siteProfile"], "siteProfile"), "siteProfile", "slopeDeg");
            s.SlopeDeg = Int(site["slopeDeg"], "slopeDeg");
            var trees = Arr(d["trees"], "trees");
            if (trees.Items.Count > LabConstants.MaxTrees) throw new LabException("SchemaInvalid", "too many trees");
            foreach (var tn in trees.Items)
            {
                var td = Keys(Obj(tn, "tree"), "tree", "treeId", "species", "xMm", "yMm", "zMm", "dbhMm", "heightMm", "ageAtInventoryYears", "status", "markState");
                var t = new TreeState
                {
                    TreeId = Str(td["treeId"], "treeId"),
                    Species = Str(td["species"], "species"),
                    XMm = Int(td["xMm"], "xMm"),
                    YMm = Int(td["yMm"], "yMm"),
                    ZMm = Int(td["zMm"], "zMm"),
                    DbhMm = Int(td["dbhMm"], "dbhMm"),
                    HeightMm = Int(td["heightMm"], "heightMm"),
                    AgeAtInventoryYears = Int(td["ageAtInventoryYears"], "ageAtInventoryYears"),
                };
                if (!Wire.TryTreeStatus(Str(td["status"], "status"), out var st)) throw new LabException("SchemaInvalid", $"tree {t.TreeId}: unknown status");
                if (!Wire.TryMark(Str(td["markState"], "markState"), out var mk)) throw new LabException("SchemaInvalid", $"tree {t.TreeId}: unknown markState");
                t.Status = st; t.Mark = mk;
                s.Trees.Add(t);
            }
            var plans = Arr(d["plans"], "plans");
            if (plans.Items.Count > LabConstants.MaxPlans) throw new LabException("SchemaInvalid", "too many plans");
            foreach (var pn in plans.Items)
            {
                var pd = Keys(Obj(pn, "plan"), "plan", "planId", "planState", "treeIds", "baseForestRevision", "approvalSessionEpoch");
                var p = new PlanRecord { PlanId = Str(pd["planId"], "planId"), TreeIds = IdList(pd["treeIds"], "treeIds") };
                if (!Wire.TryPlanStage(Str(pd["planState"], "planState"), out var ps)) throw new LabException("SchemaInvalid", $"plan {p.PlanId}: unknown planState");
                p.Stage = ps;
                p.BaseForestRevision = pd["baseForestRevision"] is JNull ? null : Int(pd["baseForestRevision"], "baseForestRevision");
                p.ApprovalSessionEpoch = pd["approvalSessionEpoch"] is JNull ? null : Str(pd["approvalSessionEpoch"], "approvalSessionEpoch");
                s.Plans.Add(p);
            }
            return s;
        }

        private static LabEvent Event(JObj o)
        {
            var d = Keys(o, "event", "eventSeq", "eventType", "treeIds", "payload", "prevStateHash", "postStateHash");
            var e = new LabEvent
            {
                EventSeq = Int(d["eventSeq"], "eventSeq"),
                TreeIds = IdList(d["treeIds"], "event.treeIds"),
                PrevStateHash = Str(d["prevStateHash"], "prevStateHash"),
                PostStateHash = Str(d["postStateHash"], "postStateHash"),
            };
            if (!Wire.TryEventType(Str(d["eventType"], "eventType"), out var type)) throw new LabException("SchemaInvalid", "unknown eventType");
            e.Type = type;
            if (!LabConstants.IsSha256Hex(e.PrevStateHash) || !LabConstants.IsSha256Hex(e.PostStateHash))
                throw new LabException("SchemaInvalid", $"event {e.EventSeq}: state hashes must be lowercase SHA-256 hex");
            var payload = Obj(d["payload"], "payload");
            string[] keys = type switch
            {
                EventType.TreeMarked or EventType.TreeUnmarked => Array.Empty<string>(),
                EventType.PlanCreated or EventType.PlanProposed or EventType.PlanQueued or EventType.PlanRejected or EventType.PlanCancelled => new[] { "planId" },
                EventType.PlanApproved => new[] { "planId", "baseForestRevision", "approvalSessionEpoch" },
                EventType.PlanStale or EventType.PlanExecutionRejected => new[] { "planId", "reason" },
                EventType.TestWorkExecuted => new[] { "planId", "forestRevisionAfter" },
                _ => new[] { "experimentStepAfter", "forestRevisionAfter" },
            };
            var pd = Keys(payload, $"event {e.EventSeq} payload", keys);
            var p = new EventPayload();
            if (pd.TryGetValue("planId", out var pid)) p.PlanId = Str(pid, "planId");
            if (pd.TryGetValue("baseForestRevision", out var br)) p.BaseForestRevision = Int(br, "baseForestRevision");
            if (pd.TryGetValue("approvalSessionEpoch", out var ep)) p.ApprovalSessionEpoch = Str(ep, "approvalSessionEpoch");
            if (pd.TryGetValue("reason", out var rs)) p.Reason = Str(rs, "reason");
            if (pd.TryGetValue("forestRevisionAfter", out var fr)) p.ForestRevisionAfter = Int(fr, "forestRevisionAfter");
            if (pd.TryGetValue("experimentStepAfter", out var xs)) p.ExperimentStepAfter = Int(xs, "experimentStepAfter");
            e.Payload = p;
            return e;
        }
    }

    internal static readonly UTF8Encoding Utf8NoBom = new(false);
}
