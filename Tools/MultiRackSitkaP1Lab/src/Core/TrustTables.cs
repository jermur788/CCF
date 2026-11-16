using System.Text;

namespace SitkaP1.Core;

/// <summary>
/// Review state of a compiled trust record. Only <see cref="Accepted"/> records may be used for real inspection or, for
/// derived fixtures, to unlock a (later) annual step. Moving a record to Accepted is a reviewed source change that must cite
/// the independent review and the Manager acceptance; an author never sets it.
/// </summary>
public enum ReviewStatus { PendingIndependentReview, Accepted }

/// <summary>Original-P0 fixture trust record (spec §9.2 T-02). Compiled into P1; never read from the P0 data directory.</summary>
public sealed record OriginalTrustRecord(
    string FixtureId,
    string FileName,
    long FileByteLength,
    string FileSha256,
    string P0SnapshotHash,
    string P0Format,
    int P0SchemaVersion,
    string P0SourceRevision,
    ReviewStatus Status,
    string? ReviewRef,
    string? ManagerAcceptanceRef);

/// <summary>Derived-fixture catalog record (spec §9.2 T-03, adapted to the 30 m domain).</summary>
public sealed record DerivedCatalogRecord(
    string DerivedFixtureId,
    P1Domain Domain,
    string FileName,
    long FileByteLength,
    string FileSha256,
    string ContentHash,
    string GeometryModel,
    uint AreaHaBits,
    string Purpose,
    string Derivation,
    ReviewStatus Status,
    string? ReviewRef,
    string? ManagerAcceptanceRef);

public static class OriginalTrustTable
{
    public const string Version = "p1-f0-original-trust-v1";
    public const string P0QualifiedCommit = "a7f41983c2f2f233b8fe999d1a5b10e72bdd2866";
    public const string P0QualifiedTree = "f212c8f64fb28b83aa870c7b5b7c72e03051e46a";

    // Values prepared by the #639 author from the blobs at P0QualifiedCommit (git cat-file), NOT copied from P0's
    // MANIFEST.sha256. Snapshot hashes are recomputed with P0's own CanonicalJson.SnapshotHash by the test suite.
    // ALL RECORDS ARE PENDING INDEPENDENT REVIEW AND MANAGER ACCEPTANCE: real inspection refuses them until then.
    public static readonly IReadOnlyList<OriginalTrustRecord> Records = new[]
    {
        new OriginalTrustRecord("oracle-quarter-ha", "oracle-quarter-ha.sitkalab.json", 969,
            "b1691934fa4a41cfd63462f4021811e0871742f0e1f23ecea77fe8da40b5cf62",
            "810030c4187c3132cafb22d46ff87c0b8dbfa43bc382a72463c686f8c554152c",
            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.PendingIndependentReview, null, null),
        new OriginalTrustRecord("sitka-flat-demo-v1", "sitka-flat-demo-v1.sitkalab.json", 26675,
            "d0eed68979b2d214555e84a59e80ff6866ccd5e7d1774883e7898fcf8fb4f2ed",
            "b407c725e655dc80ca5fec22f4ad84540136e89928577ad97b3dce31b234f5b1",
            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.PendingIndependentReview, null, null),
        new OriginalTrustRecord("sitka-flat-perf-2000-v1", "sitka-flat-perf-2000-v1.sitkalab.json", 324929,
            "d12db11126de2843ff21538f26feaab09242ccaf6abd04e3f25c8014054790df",
            "e4cebf0206c23977abc4b08857d706bdb1c110aa853b08241997081ccfde8726",
            "ccf-sitka-lab", 1, P0QualifiedCommit, ReviewStatus.PendingIndependentReview, null, null),
    };

    public static OriginalTrustRecord? Find(string fixtureId) =>
        Records.FirstOrDefault(r => string.Equals(r.FixtureId, fixtureId, StringComparison.Ordinal));

    /// <summary>SHA-256 of the canonical text of every record; recorded in experiment headers once accepted.</summary>
    public static string Digest() => DerivedManifest.Sha256Hex(Encoding.UTF8.GetBytes(Canonical(Records)));

    public static string Canonical(IEnumerable<OriginalTrustRecord> records)
    {
        var sb = new StringBuilder(Version).Append('\n');
        foreach (var r in records)
            sb.Append(string.Join('|', r.FixtureId, r.FileName, r.FileByteLength, r.FileSha256, r.P0SnapshotHash, r.P0Format,
                r.P0SchemaVersion, r.P0SourceRevision, r.Status, r.ReviewRef ?? "-", r.ManagerAcceptanceRef ?? "-")).Append('\n');
        return sb.ToString();
    }
}

public static class DerivedCatalog
{
    // The 30 m manifest's file hash and content hash are filled from the generator output and checked by the test suite.
    // PENDING INDEPENDENT REVIEW AND MANAGER ACCEPTANCE: no ecological year may run until this record is Accepted.
    public static readonly IReadOnlyList<DerivedCatalogRecord> Records = new[]
    {
        new DerivedCatalogRecord("p1d-30m-sitka-adult-v1", P1Domain.DerivedP1ThirtyMetreExperimentV1,
            "p1d-30m-sitka-adult-v1.sitkap1fx.json", 32567,
            "bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4",
            "b7dee8a34b4e66b4cc443b533604b5962f5fe4c1d2f28832270a0c2b02ba6627",
            DerivedManifest.GeometryModel, DerivedManifest.AreaHaBits, "experiment30m", "designed",
            ReviewStatus.PendingIndependentReview, null, null),
    };

    public static DerivedCatalogRecord? Find(string id) =>
        Records.FirstOrDefault(r => string.Equals(r.DerivedFixtureId, id, StringComparison.Ordinal));

    public static string Digest()
    {
        var sb = new StringBuilder(DerivedManifest.CatalogVersion).Append('\n');
        foreach (var r in Records)
            sb.Append(string.Join('|', r.DerivedFixtureId, P1DomainWire.Name(r.Domain), r.FileName, r.FileByteLength, r.FileSha256,
                r.ContentHash, r.GeometryModel, F32.Hex(r.AreaHaBits), r.Purpose, r.Derivation, r.Status, r.ReviewRef ?? "-",
                r.ManagerAcceptanceRef ?? "-")).Append('\n');
        return DerivedManifest.Sha256Hex(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
