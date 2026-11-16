using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using SitkaLab.Core;
using SitkaP1.Core;
using SitkaP1.Core.Tests;

// P1-F0 targeted tests. Read-only toward P0 and Scenario One: mutated copies are made only in a fresh temporary directory
// outside the repository (TMPDIR), which this program removes afterwards. No test runs an ecological year.

string p1Root = Fx.FindP1Root();
string repoRoot = Path.GetFullPath(Path.Combine(p1Root, "..", ".."));
string p0Fixtures = Path.Combine(repoRoot, "Tools", "MultiRackSitkaLab", "fixtures", "synthetic");
string manifestPath = Path.Combine(p1Root, "fixtures", "derived", "p1d-30m-sitka-adult-v1.sitkap1fx.json");
byte[] manifestBytes = File.ReadAllBytes(manifestPath);
string scratch = Directory.CreateTempSubdirectory("ccf-p1-tests.").FullName;
if (Path.GetFullPath(scratch).StartsWith(repoRoot + "/", StringComparison.Ordinal))
    throw new Exception("TMPDIR must be outside the repository");

// Mutate the committed manifest text and re-seal its content hash so the targeted check (not the hash) is what refuses.
byte[] Mut(string from, string to, bool reseal = true)
{
    string s = Encoding.UTF8.GetString(manifestBytes);
    if (!s.Contains(from, StringComparison.Ordinal)) throw new Exception("mutation anchor not found: " + from);
    s = Fx.ReplaceFirst(s, from, to);
    return reseal ? Fx.Reseal(s) : Encoding.UTF8.GetBytes(s);
}

// ------------------------------------------------------------------ domain tags
T.Add("domain: three exact names round-trip", () =>
{
    foreach (var d in Enum.GetValues<P1Domain>()) T.Eq(d, P1DomainWire.Parse(P1DomainWire.Name(d)), "round trip");
});
T.Add("domain: wrong case, blank and legacy names refused", () =>
{
    foreach (var s in new[] { "derivedP1ThirtyMetreExperimentV1", "DerivedP1ThirtyMetreExperimentV2", "", "Original", "OriginalPolygonExploratoryV1 " })
        T.Refuses("UnknownDomain", () => P1DomainWire.Parse(s));
});

// ------------------------------------------------------------------ f32 bits
T.Add("f32: bit strings are exact lowercase 8-hex and finite", () =>
{
    T.Eq(0x3db851ecu, F32.ParseHex("3db851ec"), "area bits");
    T.Eq(0x80000000u, F32.ParseHex("80000000"), "negative zero keeps its bits");
    T.Refuses("FloatBitsInvalid", () => F32.ParseHex("3DB851EC"));
    T.Refuses("FloatBitsInvalid", () => F32.ParseHex("3db851e"));
    T.Refuses("FloatBitsInvalid", () => F32.ParseHex("000000003db851ec"));
    T.Refuses("FloatNotFinite", () => F32.ParseHex("7fc00000"));
    T.Refuses("FloatNotFinite", () => F32.ParseHex("7f800000"));
    T.Refuses("FloatNotFinite", () => F32.ParseHex("ff800000"));
});
T.Add("f32: 900/10000 evaluates to 3db851ec in .NET float32", () =>
{
    T.Eq(0x3db851ecu, F32.Bits((float)(900.0 / 10000.0)), "binary64 then cast");
    T.Eq(0x3db851ecu, F32.Bits(900f / 10000f), "binary32 division");
});

// ------------------------------------------------------------------ strict JSON
T.Add("json: duplicate key, comment, trailing comma, float number, BOM, deep nesting refused", () =>
{
    T.Refuses("DuplicateKey", () => StrictJson.Parse("{\"a\":1,\"a\":2}"u8));
    T.Refuses("JsonInvalid", () => StrictJson.Parse("{\"a\":1/*c*/}"u8));
    T.Refuses("JsonInvalid", () => StrictJson.Parse("{\"a\":1,}"u8));
    T.Refuses("NumberNotInteger", () => StrictJson.Parse("{\"a\":1.0}"u8));
    T.Refuses("NumberNotInteger", () => StrictJson.Parse("{\"a\":1e3}"u8));
    T.Refuses("NumberOutOfRange", () => StrictJson.Parse("{\"a\":9007199254740992}"u8));
    T.Refuses("NotCanonical", () => StrictJson.Parse(new byte[] { 0xEF, 0xBB, 0xBF, (byte)'{', (byte)'}' }));
    T.Refuses("JsonInvalid", () => StrictJson.Parse(Encoding.UTF8.GetBytes(new string('[', 17) + new string(']', 17))));
    T.Refuses("StringNotAllowed", () => StrictJson.Parse("{\"a\":\"\\u00e9\"}"u8));
});

// ------------------------------------------------------------------ manifest determinism and catalog
T.Add("manifest: generator is deterministic and equals the committed file", () =>
{
    var a = ManifestGenerator.Generate().FileBytes();
    var b = ManifestGenerator.Generate().FileBytes();
    T.True(a.AsSpan().SequenceEqual(b), "two generations identical");
    T.True(a.AsSpan().SequenceEqual(manifestBytes), "committed manifest equals generator output");
});
T.Add("manifest: strict parse round-trips and matches the compiled catalog record", () =>
{
    var m = DerivedManifest.Parse(manifestBytes);
    var rec = DerivedCatalog.Find(m.DerivedFixtureId)!;
    T.Eq(rec.FileByteLength, (long)manifestBytes.Length, "length");
    T.Eq(rec.FileSha256, DerivedManifest.Sha256Hex(manifestBytes), "file sha");
    T.Eq(rec.ContentHash, m.ContentHash, "content hash");
    T.Eq(100, m.Trees.Count, "tree count");
    T.Eq(12, m.Trees.Count(t => t.DbhMm < m.ActionEligibleDbhThresholdMm), "below action threshold (still biological)");
    T.True(m.Trees.All(t => t.Status == "live" && t.AgeYears == 30), "all living, age 30");
});
T.Add("manifest: biological population is all living stems, action set is a separate label", () =>
{
    var m = DerivedManifest.Parse(manifestBytes);
    int living = m.Trees.Count(t => t.Status == "live");
    int actionable = m.Trees.Count(t => t.Status == "live" && t.DbhMm >= m.ActionEligibleDbhThresholdMm);
    T.Eq(100, living, "living");
    T.Eq(88, actionable, "actionable");
});
T.Add("manifest: trees are spatially distinct and inside the stand", () =>
{
    var m = DerivedManifest.Parse(manifestBytes);
    T.Eq(100, m.Trees.Select(t => (t.XMm, t.YMm)).Distinct().Count(), "distinct positions");
    T.True(m.Trees.All(t => t.XMm is >= 900 and <= 29100 && t.YMm is >= 900 and <= 29100), "inside [900,29100]");
});

// ------------------------------------------------------------------ manifest negatives (each must be refused for its own reason)
T.Add("manifest: cross-domain tags refused", () =>
{
    T.Refuses("DomainMismatch", () => DerivedManifest.Parse(Mut("\"domain\":\"DerivedP1ThirtyMetreExperimentV1\"", "\"domain\":\"OriginalPolygonExploratoryV1\"")));
    T.Refuses("DomainMismatch", () => DerivedManifest.Parse(Mut("\"domain\":\"DerivedP1ThirtyMetreExperimentV1\"", "\"domain\":\"DerivedUnitySquareParityV1\"")));
    T.Refuses("UnknownDomain", () => DerivedManifest.Parse(Mut("\"domain\":\"DerivedP1ThirtyMetreExperimentV1\"", "\"domain\":\"derivedP1ThirtyMetreExperimentV1\"")));
});
T.Add("manifest: altered seed / area / origin / axis / transform / area expression refused", () =>
{
    T.Refuses("ContentHashMismatch", () => DerivedManifest.Parse(Mut("\"simulationSeed\":20261116", "\"simulationSeed\":20261117", reseal: false)));
    T.Refuses("AreaInvalid", () => DerivedManifest.Parse(Mut("\"b\":\"3db851ec\"", "\"b\":\"3db851ed\"")));
    T.Refuses("AreaInvalid", () => DerivedManifest.Parse(Mut("\"areaM2Exact\":\"900\"", "\"areaM2Exact\":\"1600\"")));
    T.Refuses("GeometryInvalid", () => DerivedManifest.Parse(Mut("{\"xMm\":0,\"yMm\":0}", "{\"xMm\":1,\"yMm\":0}")));
    T.Refuses("GeometryInvalid", () => DerivedManifest.Parse(Mut("\"sideMm\":30000", "\"sideMm\":40000")));
    T.Refuses("GeometryInvalid", () => DerivedManifest.Parse(Mut("+X east; +Y north", "+X north; +Y east")));
    T.Refuses("GeometryInvalid", () => DerivedManifest.Parse(Mut("p1-square30-corner-to-centred-xz-v1", "corner-square-to-centred-xz-v1")));
    T.Refuses("GeometryInvalid", () => DerivedManifest.Parse(Mut("\"geometryModel\":\"P1DerivedSquare30m\"", "\"geometryModel\":\"Legacy40\"")));
});
T.Add("manifest: versions, seeds, years and disclosure are strict", () =>
{
    T.Refuses("UnsupportedVersion", () => DerivedManifest.Parse(Mut("\"rngModelVersion\":1", "\"rngModelVersion\":0")));
    T.Refuses("UnsupportedVersion", () => DerivedManifest.Parse(Mut("\"growthModelVersion\":2", "\"growthModelVersion\":0")));
    T.Refuses("UnsupportedVersion", () => DerivedManifest.Parse(Mut("\"derivedSchemaVersion\":1", "\"derivedSchemaVersion\":2")));
    T.Refuses("SeedInvalid", () => DerivedManifest.Parse(Mut("\"simulationSeed\":20261116", "\"simulationSeed\":4294967296")));
    T.Refuses("EcologyInvalid", () => DerivedManifest.Parse(Mut("\"sourceInventoryYear\":null", "\"sourceInventoryYear\":\"2026\"")));
    T.Refuses("SchemaInvalid", () => DerivedManifest.Parse(Mut("\"sourceInventoryYear\":null", "\"sourceInventoryYear\":2026")));
    T.Refuses("EcologyInvalid", () => DerivedManifest.Parse(Mut("\"initialEcologicalYear\":0", "\"initialEcologicalYear\":2026")));
    T.Refuses("DisclosureMissing", () => DerivedManifest.Parse(Mut("NOT AN IRISH FIELD FORECAST. NOT UNITY", "NOT UNITY")));
    T.Refuses("DerivationInvalid", () => DerivedManifest.Parse(Mut("\"originalP0FixtureId\":null", "\"originalP0FixtureId\":\"oracle-quarter-ha\"")));
    T.Refuses("NotDerivedFixtureId", () => DerivedManifest.Parse(Mut("\"derivedFixtureId\":\"p1d-30m-sitka-adult-v1\"", "\"derivedFixtureId\":\"oracle-quarter-ha\"")));
    T.Refuses("SourceProvenanceMismatch", () => DerivedManifest.Parse(Mut("e4c73402d46bb3b75a9cba2e4622857a913fac92", "e4c73402d46bb3b75a9cba2e4622857a913fac93")));
});
T.Add("manifest: tree ID / status / position / size / derived-bit defects refused", () =>
{
    T.Refuses("TreeIdOrder", () => DerivedManifest.Parse(Mut("\"treeId\":\"P30-002\"", "\"treeId\":\"P30-001\"")));
    T.Refuses("TreeIdOrder", () => DerivedManifest.Parse(Mut("\"treeId\":\"P30-001\"", "\"treeId\":\"p30-001\"")));
    T.Refuses("TreeIdInvalid", () => DerivedManifest.Parse(Mut("\"treeId\":\"P30-001\"", "\"treeId\":\"P30 001\"")));
    T.Refuses("StatusInvalid", () => DerivedManifest.Parse(Mut("\"status\":\"live\"", "\"status\":\"dead\"")));
    T.Refuses("TreeOutsideStand", () => DerivedManifest.Parse(Mut("\"xMm\":1069", "\"xMm\":30000")));
    T.Refuses("TreeOutsideStand", () => DerivedManifest.Parse(Mut("\"xMm\":1069", "\"xMm\":-1")));
    T.Refuses("DbhOutOfDomain", () => DerivedManifest.Parse(Mut("\"dbhMm\":329", "\"dbhMm\":2001")));
    T.Refuses("HeightOutOfDomain", () => DerivedManifest.Parse(Mut("\"heightMm\":23070", "\"heightMm\":60001")));
    T.Refuses("AgeInvalid", () => DerivedManifest.Parse(Mut("\"ageYears\":30", "\"ageYears\":0")));
    T.Refuses("DerivedValueMismatch", () => DerivedManifest.Parse(Mut("\"dbhMm\":329", "\"dbhMm\":328")));
    T.Refuses("DerivedValueMismatch", () => DerivedManifest.Parse(Mut("\"xMm\":1069", "\"xMm\":1070")));
    T.Refuses("DerivedValueMismatch", () => DerivedManifest.Parse(Mut("\"b\":\"405d04c1\"", "\"b\":\"405d04c2\"")));
    T.Refuses("FloatNotFinite", () => DerivedManifest.Parse(Mut("\"b\":\"405d04c1\"", "\"b\":\"7fc00000\"")));
    T.Refuses("FloatKindInvalid", () => DerivedManifest.Parse(Mut("{\"k\":\"f32\",\"b\":\"405d04c1\"}", "{\"k\":\"f64\",\"b\":\"405d04c1\"}")));
    T.Refuses("TreeCountMismatch", () => DerivedManifest.Parse(Mut("\"treeCount\":100", "\"treeCount\":99")));
});
T.Add("manifest: unknown/missing keys, whitespace and hash defects refused", () =>
{
    T.Refuses("UnknownKey", () => DerivedManifest.Parse(Mut("\"pruning\":0", "\"pruning\":0,\"extra\":1")));
    T.Refuses("MissingKey", () => DerivedManifest.Parse(Mut("\"pruning\":0,", "")));
    T.Refuses("DuplicateKey", () => DerivedManifest.Parse(Mut("\"pruning\":0", "\"pruning\":0,\"pruning\":0")));
    // Whitespace leaves the canonical body (and so the original content hash) unchanged; the canonical byte check refuses it.
    T.Refuses("NotCanonical", () => DerivedManifest.Parse(Mut("\"pruning\":0", "\"pruning\": 0", reseal: false)));
    T.Refuses("NotCanonical", () => DerivedManifest.Parse(Mut("\"stage\":\"MatureEquivalent\",\"species\":\"Sitka\"", "\"species\":\"Sitka\",\"stage\":\"MatureEquivalent\"", reseal: false)));
    T.Refuses("NotCanonical", () => DerivedManifest.Parse(manifestBytes.Concat(new byte[] { (byte)'\n' }).ToArray()));
    T.Refuses("ContentHashMismatch", () => DerivedManifest.Parse(Fx.WithHash(manifestBytes, new string('0', 64))));
});

// ------------------------------------------------------------------ derived catalog loading
T.Add("derived: pending catalog record refuses normal load; reviewer evidence load is labelled untrusted", () =>
{
    T.Refuses("ManifestNotAccepted", () => FixtureAccess.LoadDerived("p1d-30m-sitka-adult-v1", manifestBytes, TrustUse.RequireAccepted));
    var load = FixtureAccess.LoadDerived("p1d-30m-sitka-adult-v1", manifestBytes, TrustUse.ReviewEvidenceOnly);
    T.True(load.TrustLabel.StartsWith("UNTRUSTED-PENDING-REVIEW", StringComparison.Ordinal), "untrusted label");
});
T.Add("derived: altered bytes, wrong IDs and original IDs refused", () =>
{
    T.Refuses("FileHashMismatch", () => FixtureAccess.LoadDerived("p1d-30m-sitka-adult-v1", Mut("\"generatorSeed\":6390001", "\"generatorSeed\":6390002"), TrustUse.ReviewEvidenceOnly));
    T.Refuses("NotDerivedFixtureId", () => FixtureAccess.LoadDerived("oracle-quarter-ha", manifestBytes, TrustUse.ReviewEvidenceOnly));
    T.Refuses("UnknownDerivedFixture", () => FixtureAccess.LoadDerived("p1d-unknown", manifestBytes, TrustUse.ReviewEvidenceOnly));
    T.Refuses("UnknownDerivedFixture", () => FixtureAccess.LoadDerived("p1d-30M-sitka-adult-v1", manifestBytes, TrustUse.ReviewEvidenceOnly));
});

// ------------------------------------------------------------------ original trust table and read-only inspection
T.Add("original: trust records match pinned P0 blobs and P0's own snapshot hash (not MANIFEST.sha256)", () =>
{
    foreach (var r in OriginalTrustTable.Records)
    {
        var blob = Fx.Git(repoRoot, $"cat-file blob {OriginalTrustTable.P0QualifiedCommit}:Tools/MultiRackSitkaLab/fixtures/synthetic/{r.FileName}");
        T.Eq(r.FileByteLength, (long)blob.Length, r.FixtureId + " length");
        T.Eq(r.FileSha256, DerivedManifest.Sha256Hex(blob), r.FixtureId + " sha");
        T.Eq(r.P0SnapshotHash, CanonicalJson.SnapshotHash(LabFile.Parse(blob).Snapshot), r.FixtureId + " recomputed snapshot hash");
        T.Eq(ReviewStatus.PendingIndependentReview, r.Status, r.FixtureId + " status");
    }
    T.Eq(OriginalTrustTable.P0QualifiedTree, Encoding.ASCII.GetString(Fx.Git(repoRoot, "rev-parse HEAD:Tools/MultiRackSitkaLab")).Trim(), "P0 tree at HEAD");
});
T.Add("original: pending trust refuses real inspection before opening any file", () =>
{
    T.Refuses("OriginalTrustNotAccepted", () => FixtureAccess.InspectOriginal(p0Fixtures, "oracle-quarter-ha", TrustUse.RequireAccepted));
    T.Refuses("OriginalTrustNotAccepted", () => FixtureAccess.InspectOriginal("/nonexistent-dir", "oracle-quarter-ha", TrustUse.RequireAccepted));
});
T.Add("original: all three fixtures inspect read-only under reviewer evidence, annual never allowed", () =>
{
    var q = FixtureAccess.InspectOriginal(p0Fixtures, "oracle-quarter-ha", TrustUse.ReviewEvidenceOnly);
    T.Eq(3, q.LiveCount, "oracle live"); T.Eq("2500", q.PolygonAreaM2Exact, "oracle area 50 m square");
    var demo = FixtureAccess.InspectOriginal(p0Fixtures, "sitka-flat-demo-v1", TrustUse.ReviewEvidenceOnly);
    T.Eq(153, demo.LiveCount, "demo live (spec m-8)"); T.Eq(9, demo.DeadCount, "demo dead"); T.Eq(15, demo.LiveBelowActionThreshold, "demo below threshold");
    var perf = FixtureAccess.InspectOriginal(p0Fixtures, "sitka-flat-perf-2000-v1", TrustUse.ReviewEvidenceOnly);
    T.Eq(1893, perf.LiveCount, "perf live"); T.Eq(107, perf.DeadCount, "perf dead"); T.Eq(182, perf.LiveBelowActionThreshold, "perf below threshold");
    foreach (var r in new[] { q, demo, perf })
    {
        T.True(!r.AnnualAdvanceAllowed, "annual not allowed");
        T.Eq(P1Domain.OriginalPolygonExploratoryV1, r.Domain, "domain");
        T.True(r.TrustLabel.StartsWith("UNTRUSTED-PENDING-REVIEW", StringComparison.Ordinal), "untrusted label");
    }
});
T.Add("original: derived and unknown IDs refused by the original import", () =>
{
    T.Refuses("UnknownOriginalFixture", () => FixtureAccess.InspectOriginal(p0Fixtures, "p1d-30m-sitka-adult-v1", TrustUse.ReviewEvidenceOnly));
    T.Refuses("UnknownOriginalFixture", () => FixtureAccess.InspectOriginal(p0Fixtures, "ORACLE-QUARTER-HA", TrustUse.ReviewEvidenceOnly));
    T.Refuses("UnknownOriginalFixture", () => FixtureAccess.InspectOriginal(p0Fixtures, "sitka-flat-demo", TrustUse.ReviewEvidenceOnly));
});
T.Add("original: altered byte, whitespace change and consistent fixture+MANIFEST forgery refused", () =>
{
    var rec = OriginalTrustTable.Find("oracle-quarter-ha")!;
    var orig = File.ReadAllBytes(Path.Combine(p0Fixtures, rec.FileName));
    // same length, one digit changed
    var altered = (byte[])orig.Clone();
    int idx = Encoding.ASCII.GetString(altered).IndexOf("\"dbhMm\":200", StringComparison.Ordinal) + 9;
    altered[idx] = (byte)'3';
    var d1 = Fx.Dir(scratch, "altered"); File.WriteAllBytes(Path.Combine(d1, rec.FileName), altered);
    T.Refuses("FileHashMismatch", () => FixtureAccess.InspectOriginal(d1, rec.FixtureId, TrustUse.ReviewEvidenceOnly));
    // whitespace added (snapshot hash unchanged) → length differs → refused before hashing
    var d2 = Fx.Dir(scratch, "ws"); File.WriteAllBytes(Path.Combine(d2, rec.FileName), orig.Concat(new byte[] { (byte)'\n' }).ToArray());
    T.Refuses("FileSizeMismatch", () => FixtureAccess.InspectOriginal(d2, rec.FixtureId, TrustUse.ReviewEvidenceOnly));
    // a fully consistent forgery (re-serialized P0 file with matching embedded hash and a matching MANIFEST.sha256)
    var f = LabFile.Parse(orig);
    f.Snapshot.Trees[0].DbhMm = 201;
    f.SnapshotHash = CanonicalJson.SnapshotHash(f.Snapshot);
    var forged = LabFile.Serialize(f);
    var d3 = Fx.Dir(scratch, "forged");
    File.WriteAllBytes(Path.Combine(d3, rec.FileName), forged);
    File.WriteAllText(Path.Combine(d3, "MANIFEST.sha256"), DerivedManifest.Sha256Hex(forged) + "  " + rec.FileName + "\n");
    T.Eq(orig.Length, forged.Length, "forgery keeps the length");
    T.Refuses("FileHashMismatch", () => FixtureAccess.InspectOriginal(d3, rec.FixtureId, TrustUse.ReviewEvidenceOnly));
});
T.Add("original: symlink file, symlinked directory, hard link, FIFO and directory refused", () =>
{
    var rec = OriginalTrustTable.Find("oracle-quarter-ha")!;
    var orig = Path.Combine(p0Fixtures, rec.FileName);
    var good = Fx.Dir(scratch, "good"); File.Copy(orig, Path.Combine(good, rec.FileName));
    FixtureAccess.InspectOriginal(good, rec.FixtureId, TrustUse.ReviewEvidenceOnly); // control: a plain copy is accepted

    var sl = Fx.Dir(scratch, "symlink"); File.CreateSymbolicLink(Path.Combine(sl, rec.FileName), Path.Combine(good, rec.FileName));
    T.Refuses("SymlinkRefused", () => FixtureAccess.InspectOriginal(sl, rec.FixtureId, TrustUse.ReviewEvidenceOnly));

    var sld = Path.Combine(scratch, "dirlink"); Directory.CreateSymbolicLink(sld, good);
    T.Refuses("DirectorySymlinkRefused", () => FixtureAccess.InspectOriginal(sld, rec.FixtureId, TrustUse.ReviewEvidenceOnly));

    var hl = Fx.Dir(scratch, "hardlink"); File.Copy(orig, Path.Combine(hl, rec.FileName));
    T.Eq(0, Fx.Link(Path.Combine(hl, rec.FileName), Path.Combine(hl, "second-name")), "link()");
    T.Refuses("HardLinked", () => FixtureAccess.InspectOriginal(hl, rec.FixtureId, TrustUse.ReviewEvidenceOnly));

    var ff = Fx.Dir(scratch, "fifo");
    T.Eq(0, Fx.MkFifo(Path.Combine(ff, rec.FileName), 0x180), "mkfifo()");
    T.Refuses("NotRegularFile", () => FixtureAccess.InspectOriginal(ff, rec.FixtureId, TrustUse.ReviewEvidenceOnly));

    var dd = Fx.Dir(scratch, "dir"); Directory.CreateDirectory(Path.Combine(dd, rec.FileName));
    T.Refuses("NotRegularFile", () => FixtureAccess.InspectOriginal(dd, rec.FixtureId, TrustUse.ReviewEvidenceOnly));

    var missing = Fx.Dir(scratch, "missing");
    T.Refuses("FileMissing", () => FixtureAccess.InspectOriginal(missing, rec.FixtureId, TrustUse.ReviewEvidenceOnly));
});
T.Add("original: canonical, internally valid but non-pristine P0 snapshots refused (step, mark, plan, felled)", () =>
{
    var orig = File.ReadAllBytes(Path.Combine(p0Fixtures, "oracle-quarter-ha.sitkalab.json"));
    void Check(string label, Action<LabFileContent> change)
    {
        var f = LabFile.Parse(orig);
        change(f);
        f.SnapshotHash = CanonicalJson.SnapshotHash(f.Snapshot);
        var bytes = LabFile.Serialize(f);
        LabFile.Parse(bytes); // still a valid P0 file
        var dir = Fx.Dir(scratch, "np-" + label);
        File.WriteAllBytes(Path.Combine(dir, "oracle-quarter-ha.sitkalab.json"), bytes);
        var testTable = new[] { new OriginalTrustRecord("oracle-quarter-ha", "oracle-quarter-ha.sitkalab.json", bytes.Length,
            DerivedManifest.Sha256Hex(bytes), f.SnapshotHash, "ccf-sitka-lab", 1, OriginalTrustTable.P0QualifiedCommit,
            ReviewStatus.Accepted, "test-only", "test-only") };
        T.Refuses("OriginalNotPristine", () => FixtureAccess.InspectOriginal(dir, "oracle-quarter-ha", TrustUse.RequireAccepted, testTable));
    }
    Check("step", f => f.Snapshot.ExperimentStep = 1);
    Check("mark", f => f.Snapshot.Trees[0].Mark = MarkState.PlayerMarked);
    Check("felled", f => f.Snapshot.Trees[0].Status = TreeStatus.Felled);
});

// ------------------------------------------------------------------ annual gate: no ecological year can run in this stage
T.Add("annual: original refused, Unity-square unavailable, pending 30 m manifest refused, mismatched record refused", () =>
{
    var rec = DerivedCatalog.Find("p1d-30m-sitka-adult-v1");
    T.Eq("OriginalAnnualRefused", AnnualGate.Check(P1Domain.OriginalPolygonExploratoryV1, rec).Code, "original");
    T.Eq("DomainUnavailable", AnnualGate.Check(P1Domain.DerivedUnitySquareParityV1, rec).Code, "unity square");
    T.Eq("ManifestNotAccepted", AnnualGate.Check(P1Domain.DerivedP1ThirtyMetreExperimentV1, rec).Code, "pending 30 m");
    T.Eq("UnknownDerivedFixture", AnnualGate.Check(P1Domain.DerivedP1ThirtyMetreExperimentV1, null).Code, "no record");
    var accepted = rec! with { Status = ReviewStatus.Accepted, ReviewRef = "x", ManagerAcceptanceRef = "y" };
    T.Eq("AnnualEngineNotImplemented", AnnualGate.Check(P1Domain.DerivedP1ThirtyMetreExperimentV1, accepted).Code, "even accepted: no engine in this stage");
    var wrongDomain = accepted with { Domain = P1Domain.OriginalPolygonExploratoryV1 };
    T.Eq("DomainMismatch", AnnualGate.Check(P1Domain.DerivedP1ThirtyMetreExperimentV1, wrongDomain).Code, "record domain");
});

// ------------------------------------------------------------------ reused Scenario One source (function level only)
T.Add("source: linked CCF files are byte-identical to the pinned blobs", () =>
{
    foreach (var p in CcfSourcePins.Files)
    {
        T.Eq(p.GitBlob, Encoding.ASCII.GetString(Fx.Git(repoRoot, "rev-parse HEAD:" + p.Path)).Trim(), p.Path + " blob at HEAD");
        T.Eq(p.Sha256, DerivedManifest.Sha256Hex(File.ReadAllBytes(Path.Combine(repoRoot, p.Path))), p.Path + " working-tree sha256");
    }
});
T.Add("source: SimulationRandom.Roll is deterministic, in [0,1), and ID-case sensitive (function probe, not a year)", () =>
{
    float a = SimulationRandom.Roll(1, "ADULT-MORT-P30-001", 1, 20261116);
    T.Eq(F32.Bits(a), F32.Bits(SimulationRandom.Roll(1, "ADULT-MORT-P30-001", 1, 20261116)), "repeatable");
    T.True(a >= 0f && a < 1f, "range");
    T.True(F32.Bits(a) != F32.Bits(SimulationRandom.Roll(1, "ADULT-MORT-p30-001", 1, 20261116)), "case changes the roll");
    T.True(F32.Bits(a) != F32.Bits(SimulationRandom.Roll(0, "ADULT-MORT-P30-001", 1, 20261116)), "model 0 differs from model 1");
});
T.Add("source: Class III height envelope anchors at 20.4 m at age 30 (function probe)", () =>
{
    T.True(Math.Abs(SitkaGrowthModel.SiteTopHeightM(30) - 20.4) < 1e-12, "E(30)=20.4");
});

int result = T.RunAll();
Directory.Delete(scratch, recursive: true);
return result;

static class Fx
{
    public static string FindP1Root()
    {
        var d = new DirectoryInfo(AppContext.BaseDirectory);
        while (d != null && !File.Exists(Path.Combine(d.FullName, "MultiRackSitkaP1Lab.sln"))) d = d.Parent;
        return d?.FullName ?? throw new Exception("P1 root not found");
    }

    public static string ReplaceFirst(string s, string from, string to)
    {
        int i = s.IndexOf(from, StringComparison.Ordinal);
        return s[..i] + to + s[(i + from.Length)..];
    }

    /// <summary>Recompute contentHash over the mutated body so only the targeted rule can refuse.</summary>
    public static byte[] Reseal(string s)
    {
        const string marker = ",\"contentHash\":\"";
        int i = s.LastIndexOf(marker, StringComparison.Ordinal);
        string body = s[..i] + "}";
        string hash = DerivedManifest.Sha256Hex(Encoding.UTF8.GetBytes(body));
        return Encoding.UTF8.GetBytes(s[..i] + marker + hash + "\"}");
    }

    public static byte[] WithHash(byte[] bytes, string hash)
    {
        string s = Encoding.UTF8.GetString(bytes);
        const string marker = ",\"contentHash\":\"";
        int i = s.LastIndexOf(marker, StringComparison.Ordinal);
        return Encoding.UTF8.GetBytes(s[..i] + marker + hash + "\"}");
    }

    public static string Dir(string root, string name) => Directory.CreateDirectory(Path.Combine(root, name)).FullName;

    public static byte[] Git(string repo, string args)
    {
        var psi = new ProcessStartInfo("git", $"-C \"{repo}\" {args}") { RedirectStandardOutput = true, UseShellExecute = false };
        using var p = Process.Start(psi)!;
        using var ms = new MemoryStream();
        p.StandardOutput.BaseStream.CopyTo(ms);
        p.WaitForExit();
        if (p.ExitCode != 0) throw new Exception("git " + args + " failed");
        return ms.ToArray();
    }

    [DllImport("libc", EntryPoint = "link", SetLastError = true)]
    public static extern int Link([MarshalAs(UnmanagedType.LPUTF8Str)] string oldPath, [MarshalAs(UnmanagedType.LPUTF8Str)] string newPath);
    [DllImport("libc", EntryPoint = "mkfifo", SetLastError = true)]
    public static extern int MkFifo([MarshalAs(UnmanagedType.LPUTF8Str)] string path, uint mode);
}
