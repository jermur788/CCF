using System.Text;
using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

public static class FileFormatTests
{
    // Golden values: a change to any fixture or to the canonical form must be a deliberate, reviewed edit.
    private static readonly Dictionary<string, (string Snapshot, string File)> Golden = new()
    {
        ["oracle-quarter-ha"] = ("810030c4187c3132cafb22d46ff87c0b8dbfa43bc382a72463c686f8c554152c", "b1691934fa4a41cfd63462f4021811e0871742f0e1f23ecea77fe8da40b5cf62"),
        ["sitka-flat-demo-v1"] = ("b407c725e655dc80ca5fec22f4ad84540136e89928577ad97b3dce31b234f5b1", "d0eed68979b2d214555e84a59e80ff6866ccd5e7d1774883e7898fcf8fb4f2ed"),
        ["sitka-flat-perf-2000-v1"] = ("e4cebf0206c23977abc4b08857d706bdb1c110aa853b08241997081ccfde8726", "d12db11126de2843ff21538f26feaab09242ccaf6abd04e3f25c8014054790df"),
    };

    private static string OracleText() => File.ReadAllText(Path.Combine(ToolPaths.FixtureDir, "oracle-quarter-ha.sitkalab.json"), new UTF8Encoding(false));
    private static byte[] Bytes(string s) => new UTF8Encoding(false).GetBytes(s);

    private static string ParseCode(string text) => T.Throws<LabException>(() => LabFile.Parse(Bytes(text)), "parse should fail").Code;

    [Test]
    public static void Fixtures_ArePinnedCanonicalAndMatchManifest()
    {
        var manifest = File.ReadAllLines(Path.Combine(ToolPaths.FixtureDir, "MANIFEST.sha256"))
            .Select(l => l.Split("  ")).ToDictionary(p => p[1], p => p[0]);
        T.Eq(3, Fx.Catalog.All.Count, "three checked-in fixtures");
        foreach (var fx in Fx.Catalog.All)
        {
            var (snapGold, fileGold) = Golden[fx.FixtureId];
            T.Eq(snapGold, fx.SnapshotHash, fx.FixtureId + " snapshot SHA-256 pinned");
            T.Eq(fileGold, fx.FileSha256, fx.FixtureId + " file SHA-256 pinned");
            T.Eq(manifest[fx.FileName], fx.FileSha256, fx.FixtureId + " matches MANIFEST.sha256");
            var bytes = File.ReadAllBytes(Path.Combine(ToolPaths.FixtureDir, fx.FileName));
            T.True(bytes.Length > 0 && bytes[0] == (byte)'{', "no BOM");
            T.False(bytes.Contains((byte)'\n') || bytes.Contains((byte)'\r') || bytes.Contains((byte)' '), "compact: no whitespace/newlines in canonical bytes");
            var reparsed = LabFile.Parse(bytes);
            T.True(LabFile.Serialize(reparsed).AsSpan().SequenceEqual(bytes), "parse → serialize is byte-identical");
            T.Eq(fx.SnapshotHash, CanonicalJson.SnapshotHash(reparsed.Snapshot), "snapshot hash re-derived");
        }
    }

    [Test]
    public static void Fixtures_AreSyntheticAndNeverClaimMeasuredData()
    {
        foreach (var fx in Fx.Catalog.All)
        {
            T.True(fx.Snapshot.Trees.All(t => t.Species == "Sitka" && t.ZMm == 0), "Sitka only, flat");
            T.Eq(0L, fx.Snapshot.SlopeDeg, "flat");
            T.Eq(0, fx.Snapshot.Plans.Count, "no plans in a fixture");
        }
        T.Eq(2000, Fx.Catalog.TryGet("sitka-flat-perf-2000-v1")!.Snapshot.Trees.Count, "perf fixture has exactly 2000 trees");
        var demo = Fx.Catalog.TryGet("sitka-flat-demo-v1")!.Snapshot;
        T.True(demo.Trees.Any(t => t.Status == TreeStatus.Dead), "demo has dead stems");
        T.True(demo.Trees.Any(t => t.Status == TreeStatus.Live && t.DbhMm < demo.AdultThresholdMm), "demo has below-threshold stems");
        T.True(demo.Trees.Any(t => t.XMm == 0 && t.YMm == 25_000), "demo has a boundary-on-edge stem");
    }

    [Test]
    public static void LoadingTheSameFixtureTwice_GivesIdenticalCanonicalBytesAndMetrics()
    {
        foreach (var fx in Fx.Catalog.All)
        {
            var a = LabSession.FromFixture(fx); var b = LabSession.FromFixture(fx);
            T.True(LabFile.Serialize(a.ToFile()).AsSpan().SequenceEqual(LabFile.Serialize(b.ToFile())), "identical bytes");
            T.True(LabFile.Serialize(a.ToFile()).AsSpan().SequenceEqual(File.ReadAllBytes(Path.Combine(ToolPaths.FixtureDir, fx.FileName))), "a fresh session serialises to the checked-in bytes");
            T.Eq(a.ActualMetrics(), b.ActualMetrics(), "identical metrics");
            T.True(a.SessionEpoch != b.SessionEpoch, "session epoch is fresh on every start");
            T.True(LabConstants.IsEpoch(a.SessionEpoch) && a.SessionEpoch.Length == 32, "epoch is an opaque 32-hex token");
        }
    }

    // ---- input contract negatives (each mutates one thing in the oracle fixture text) ---------------------------
    [Test]
    public static void Parse_RejectsNonIntegerOverflowAndNonPositiveMeasurements()
    {
        string t = OracleText();
        T.Eq("NonIntegerNumber", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":200.5")), "fractional dbh");
        T.Eq("NonIntegerNumber", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":2e2")), "exponent form");
        T.Eq("NonIntegerNumber", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":-0")), "negative zero");
        T.Eq("JsonInvalid", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":0200")), "leading zero is not valid JSON");
        T.Eq("NumberOverflow", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":9999999999999999999")), "overflows int64");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":0")), "zero dbh");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":-5")), "negative dbh");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":1000000001")), "dbh beyond numeric-safety bound");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"heightMm\":12000", "\"heightMm\":0")), "zero height");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"ageAtInventoryYears\":28", "\"ageAtInventoryYears\":-1")), "negative age");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"adultThresholdMm\":100", "\"adultThresholdMm\":0")), "zero threshold");
        T.Eq("UnsupportedJsonValue", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":true")), "boolean");
        T.Eq("JsonInvalid", ParseCode(t.Replace("\"dbhMm\":200", "\"dbhMm\":NaN")), "NaN literal is not JSON");
    }

    [Test]
    public static void Parse_RejectsBadIdsDuplicatesSpeciesAndOutOfPolygonStems()
    {
        string t = OracleText();
        T.Eq("DuplicateId", ParseCode(t.Replace("\"treeId\":\"T002\"", "\"treeId\":\"T001\"")), "duplicate tree IDs");
        T.Eq("NotSorted", ParseCode(t.Replace("\"treeId\":\"T001\"", "\"treeId\":\"T009\"")), "unsorted IDs");
        T.Eq("InvalidId", ParseCode(t.Replace("\"treeId\":\"T001\"", "\"treeId\":\"T 01\"")), "space in ID");
        T.Eq("InvalidId", ParseCode(t.Replace("\"treeId\":\"T001\"", "\"treeId\":\"T\\\"1\"")), "quote in ID");
        T.Eq("InvalidId", ParseCode(t.Replace("\"treeId\":\"T001\"", "\"treeId\":\"" + new string('A', 65) + "\"")), "65-char ID");
        T.Eq("InvalidId", ParseCode(t.Replace("\"treeId\":\"T001\"", "\"treeId\":\"\"")), "empty ID");
        T.Eq("InvalidId", ParseCode(t.Replace("\"forestId\":\"oracle-quarter-ha\"", "\"forestId\":\"a/b\"")), "path-like forest ID");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"species\":\"Sitka\"", "\"species\":\"Oak\"")), "non-Sitka species");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"zMm\":0", "\"zMm\":5")), "non-zero z on a flat site");
        T.Eq("InvalidValue", ParseCode(t.Replace("\"slopeDeg\":0", "\"slopeDeg\":5")), "sloped site");
        T.Eq("OutsidePolygon", ParseCode(t.Replace("\"xMm\":10000,\"yMm\":10000", "\"xMm\":50001,\"yMm\":10000")), "stem 1 mm outside rejects the whole fixture");
        T.Eq("InvalidMark", ParseCode(t.Replace("\"status\":\"live\",\"markState\":\"UNMARKED\"}", "\"status\":\"dead\",\"markState\":\"PLAYER_MARKED\"}")), "dead tree cannot be marked");
    }

    [Test]
    public static void Parse_RejectsBowTieAndOtherInvalidPolygons()
    {
        string t = OracleText();
        string square = "\"standAreaPolygon\":[{\"xMm\":0,\"yMm\":0},{\"xMm\":50000,\"yMm\":0},{\"xMm\":50000,\"yMm\":50000},{\"xMm\":0,\"yMm\":50000}]";
        T.True(t.Contains(square), "test premise: polygon text found");
        string bow = "\"standAreaPolygon\":[{\"xMm\":0,\"yMm\":0},{\"xMm\":50000,\"yMm\":50000},{\"xMm\":50000,\"yMm\":0},{\"xMm\":0,\"yMm\":50000}]";
        T.Eq("InvalidPolygon", ParseCode(t.Replace(square, bow)), "bow-tie");
        string collinear = "\"standAreaPolygon\":[{\"xMm\":0,\"yMm\":0},{\"xMm\":1000,\"yMm\":0},{\"xMm\":2000,\"yMm\":0}]";
        T.Eq("InvalidPolygon", ParseCode(t.Replace(square, collinear)), "zero-area ring");
        string reversed = "\"standAreaPolygon\":[{\"xMm\":0,\"yMm\":0},{\"xMm\":0,\"yMm\":50000},{\"xMm\":50000,\"yMm\":50000},{\"xMm\":50000,\"yMm\":0}]";
        T.True(LabFile.Parse(Bytes(Rehash(t.Replace(square, reversed)))) is not null, "reversed winding is accepted");
        string noPoly = "\"standAreaPolygon\":[]";
        T.Eq("InvalidPolygon", ParseCode(t.Replace(square, noPoly)), "missing polygon");
    }

    /// <summary>Recomputes the snapshotHash so that a deliberately valid mutation is not rejected only for its stale hash.</summary>
    private static string Rehash(string text)
    {
        var root = (JObjAccess.Snapshot(text));
        return text.Replace("\"snapshotHash\":\"" + JObjAccess.OldHash(text) + "\"", "\"snapshotHash\":\"" + root + "\"");
    }

    private static class JObjAccess
    {
        public static string OldHash(string text) { int i = text.LastIndexOf("\"snapshotHash\":\"", StringComparison.Ordinal) + 16; return text.Substring(i, 64); }
        public static string Snapshot(string text)
        {
            int a = text.IndexOf("\"snapshot\":", StringComparison.Ordinal) + 11;
            int b = text.IndexOf(",\"eventLog\":", StringComparison.Ordinal);
            return CanonicalJson.Sha256Hex(new UTF8Encoding(false).GetBytes(text.Substring(a, b - a)));
        }
    }

    [Test]
    public static void Parse_RejectsSchemaDriftDuplicateKeysAndNonCanonicalBytes()
    {
        string t = OracleText();
        T.Eq("DuplicateKey", ParseCode(t.Replace("\"fixtureId\":\"oracle-quarter-ha\",", "\"fixtureId\":\"oracle-quarter-ha\",\"fixtureId\":\"oracle-quarter-ha\",")), "duplicate top-level key");
        T.Eq("SchemaInvalid", ParseCode(t.Replace("\"fixtureId\":\"oracle-quarter-ha\",", "\"fixtureId\":\"oracle-quarter-ha\",\"extra\":1,")), "unknown top-level key");
        T.Eq("SchemaInvalid", ParseCode(t.Replace("\"zMm\":0,\"dbhMm\":200", "\"dbhMm\":200,\"zMm\":0")), "out-of-order tree keys");
        T.Eq("NotCanonical", ParseCode(t.Replace("{\"format\"", "{ \"format\"")), "extra whitespace");
        T.Eq("NotCanonical", ParseCode(t + "\n"), "trailing newline");
        T.Eq("NotCanonical", ParseCode(t.Replace(",\"eventLog\":[]", ",\"eventLog\": []")), "space after colon");
        string wrongHash = t.Replace(Golden["oracle-quarter-ha"].Snapshot, new string('0', 64));
        T.Eq("HashMismatch", ParseCode(wrongHash), "snapshotHash must equal SHA-256 of canonical snapshot bytes");
        T.Eq("SchemaInvalid", ParseCode(t.Replace(Golden["oracle-quarter-ha"].Snapshot, Golden["oracle-quarter-ha"].Snapshot.ToUpperInvariant())), "uppercase hex hash");
        var bom = new byte[] { 0xEF, 0xBB, 0xBF }.Concat(File.ReadAllBytes(Path.Combine(ToolPaths.FixtureDir, "oracle-quarter-ha.sitkalab.json"))).ToArray();
        T.True(T.Throws<LabException>(() => LabFile.Parse(bom), "BOM").Code is "WrongFormat" or "JsonInvalid", "BOM rejected");
        var crlf = t.Replace(",\"snapshot\":", ",\r\n\"snapshot\":");
        T.Eq("NotCanonical", ParseCode(crlf), "CRLF line ending");
    }

    // ---- header gate: wrong-format content must fail before any forest/event parsing ---------------------------
    [Test]
    public static void HeaderGate_RejectsCcfShapedAndWrongFormatContent_BeforeBodyParsing()
    {
        // A CCF ForestSaveData-shaped document (different first key).
        string ccf = "{\"saveVersion\":3,\"forestName\":\"Scenario One\",\"trees\":[{\"id\":1,\"species\":\"Oak\"}]}";
        T.Eq("WrongFormat", ParseCode(ccf), "CCF-shaped JSON");
        // Header is wrong AND the body is syntactically broken: a body parse would report JsonInvalid, the header gate reports WrongFormat.
        T.Eq("WrongFormat", ParseCode("{\"format\":\"ccf-sitka-lab\",\"labSchemaVersion\":2,\"sourceClass\":\"Synthetic\",GARBAGE{{{"), "wrong schema version; garbage body never parsed");
        T.Eq("WrongFormat", ParseCode("{\"format\":\"ccf-forest\",\"labSchemaVersion\":1,\"sourceClass\":\"Synthetic\",GARBAGE"), "wrong format marker; body never parsed");
        T.Eq("WrongFormat", ParseCode("{\"format\":\"ccf-sitka-lab\",\"labSchemaVersion\":1,\"sourceClass\":\"Measured\",GARBAGE"), "sourceClass not Synthetic; body never parsed");
        T.Eq("WrongFormat", ParseCode("{\"labSchemaVersion\":1,\"format\":\"ccf-sitka-lab\",\"sourceClass\":\"Synthetic\"}"), "header keys out of order");
        T.Eq("WrongFormat", ParseCode("[]"), "array root");
        T.Eq("WrongFormat", ParseCode(""), "empty file");
        T.Eq("WrongFormat", ParseCode("not json at all"), "garbage");
        // Correct header + broken body is a body error (proves the gate passed and the parser then ran).
        T.Eq("JsonInvalid", ParseCode("{\"format\":\"ccf-sitka-lab\",\"labSchemaVersion\":1,\"sourceClass\":\"Synthetic\",GARBAGE"), "valid header, broken body");
    }
}
