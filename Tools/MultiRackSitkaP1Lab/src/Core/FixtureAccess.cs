using SitkaLab.Core;

namespace SitkaP1.Core;

/// <summary>Whether a pending (unreviewed) trust record may be used. Only the reviewer-evidence CLI passes <see cref="ReviewEvidenceOnly"/>.</summary>
public enum TrustUse { RequireAccepted, ReviewEvidenceOnly }

/// <summary>Result of a read-only original-P0 inspection. It is a report, never a P1 experiment state.</summary>
public sealed record OriginalInspection(
    P1Domain Domain,
    string FixtureId,
    string FileSha256,
    string P0SnapshotHash,
    string TrustTableVersion,
    string TrustLabel,
    long InventoryYear,
    int TreeCount,
    int LiveCount,
    int DeadCount,
    int LiveBelowActionThreshold,
    long AdultThresholdMm,
    int PolygonVertexCount,
    string PolygonAreaM2Exact,
    bool AnnualAdvanceAllowed,
    string Disclosure);

/// <summary>Result of loading the derived 30 m manifest through its compiled catalog record.</summary>
public sealed record DerivedLoad(DerivedManifest Manifest, DerivedCatalogRecord Record, string TrustLabel);

public static class FixtureAccess
{
    /// <summary>
    /// Read-only original-P0 inspection (spec §4.1 / §9.2 T-02), all-or-nothing, in the specified order:
    /// trust record → trust status → no-follow descriptor checks → exact bytes → SHA-256 → P0 strict parse →
    /// P0 snapshot hash → original-state rules. Never writes; never reads P0's MANIFEST.sha256; never activates state.
    /// </summary>
    public static OriginalInspection InspectOriginal(string p0FixtureDirectory, string fixtureId, TrustUse use,
        IReadOnlyList<OriginalTrustRecord>? table = null)
    {
        NoFollowRead.RequireSupportedPlatform();
        if (fixtureId.StartsWith(DerivedManifest.IdPrefix, StringComparison.Ordinal))
            throw new P1Refusal("UnknownOriginalFixture", "derived P1 fixture IDs cannot be imported as original P0 fixtures");
        var rec = (table ?? OriginalTrustTable.Records).FirstOrDefault(r => string.Equals(r.FixtureId, fixtureId, StringComparison.Ordinal))
            ?? throw new P1Refusal("UnknownOriginalFixture", "fixture ID is not in the P1 original trust table: " + fixtureId);
        if (rec.Status != ReviewStatus.Accepted && use != TrustUse.ReviewEvidenceOnly)
            throw new P1Refusal("OriginalTrustNotAccepted", "the trust record is pending independent review and Manager acceptance");
        if (rec.P0Format != LabConstants.Format || rec.P0SchemaVersion != LabConstants.SchemaVersion)
            throw new P1Refusal("UnsupportedVersion", "trust record names an unsupported P0 format/schema");

        byte[] bytes = NoFollowRead.ReadExact(p0FixtureDirectory, rec.FileName, rec.FileByteLength);
        string fileSha = DerivedManifest.Sha256Hex(bytes);
        if (fileSha != rec.FileSha256) throw new P1Refusal("FileHashMismatch", "file SHA-256 differs from the trusted value");

        LabFileContent f;
        try { f = LabFile.Parse(bytes); }
        catch (LabException e) { throw new P1Refusal("P0Parse:" + e.Code, "P0 strict reader refused the file: " + e.Message); }

        string snap = CanonicalJson.SnapshotHash(f.Snapshot);
        if (snap != rec.P0SnapshotHash || f.SnapshotHash != rec.P0SnapshotHash)
            throw new P1Refusal("SnapshotHashMismatch", "P0 snapshot hash differs from the trusted value");

        var s = f.Snapshot;
        if (f.FixtureId != rec.FixtureId) throw new P1Refusal("OriginalNotPristine", "embedded fixtureId differs from the trusted ID");
        if (s.ExperimentStep != 0 || s.ForestRevision != 0) throw new P1Refusal("OriginalNotPristine", "original must have experimentStep 0 and forestRevision 0");
        if (s.Plans.Count != 0 || f.Events.Count != 0) throw new P1Refusal("OriginalNotPristine", "original must have no plans and no event log");
        if (s.Trees.Any(t => t.Mark != MarkState.Unmarked)) throw new P1Refusal("OriginalNotPristine", "original trees must be unmarked");
        if (s.Trees.Any(t => t.Status == TreeStatus.Felled)) throw new P1Refusal("OriginalNotPristine", "felled trees are unsupported in an original import");
        if (s.Trees.Any(t => t.ZMm != 0) || s.SlopeDeg != 0) throw new P1Refusal("OriginalNotPristine", "nonzero elevation/slope is refused");

        int live = s.Trees.Count(t => t.Status == TreeStatus.Live);
        return new OriginalInspection(
            P1Domain.OriginalPolygonExploratoryV1, rec.FixtureId, fileSha, snap, OriginalTrustTable.Version,
            rec.Status == ReviewStatus.Accepted ? "TRUSTED" : "UNTRUSTED-PENDING-REVIEW (reviewer evidence only)",
            s.InventoryYear, s.Trees.Count, live, s.Trees.Count - live,
            s.Trees.Count(t => t.Status == TreeStatus.Live && t.DbhMm < s.AdultThresholdMm), s.AdultThresholdMm,
            s.Polygon.Count, ExactAreaM2(s.Polygon), AnnualAdvanceAllowed: false, P1Disclosure.Original);
    }

    /// <summary>Exact shoelace area as a decimal string in m² (integer mm² / 10^6, with an exact ".5" possible).</summary>
    public static string ExactAreaM2(IReadOnlyList<Vertex> poly)
    {
        Int128 twice = 0;
        for (int i = 0; i < poly.Count; i++)
        {
            var a = poly[i];
            var b = poly[(i + 1) % poly.Count];
            twice += (Int128)a.XMm * b.YMm - (Int128)b.XMm * a.YMm;
        }
        if (twice < 0) twice = -twice;
        // area mm² = twice/2 ; area m² = twice / 2_000_000 → print exactly with up to 7 decimals (denominator 2·10^6 divides 10^7).
        Int128 scaled = twice * 5; // = area m² × 10^7
        Int128 whole = scaled / 10_000_000;
        Int128 frac = scaled % 10_000_000;
        return frac == 0 ? whole.ToString() : whole + "." + frac.ToString().PadLeft(7, '0').TrimEnd('0');
    }

    /// <summary>
    /// #643 M-02: reads a derived fixture from <paramref name="directory"/> with the same descriptor-anchored, no-follow,
    /// size-capped read as the original import. The file name and exact length come from the compiled catalog record (never
    /// from the caller), and ID, domain and review status are checked BEFORE any file is opened. A symlinked ancestor or
    /// leaf, hard link, FIFO, directory or wrong-size file is refused without reading its contents; then the bytes go through
    /// <see cref="LoadDerived"/> (SHA-256, strict canonical parse, domain, content hash).
    /// </summary>
    public static DerivedLoad ReadDerived(string directory, string derivedFixtureId, TrustUse use,
        IReadOnlyList<DerivedCatalogRecord>? catalog = null)
    {
        var rec = DerivedRecordFor(derivedFixtureId, use, catalog);
        byte[] bytes = NoFollowRead.ReadExact(directory, rec.FileName, rec.FileByteLength);
        return LoadDerived(derivedFixtureId, bytes, use, catalog);
    }

    private static DerivedCatalogRecord DerivedRecordFor(string derivedFixtureId, TrustUse use, IReadOnlyList<DerivedCatalogRecord>? catalog)
    {
        if (!derivedFixtureId.StartsWith(DerivedManifest.IdPrefix, StringComparison.Ordinal))
            throw new P1Refusal("NotDerivedFixtureId", "only p1d- IDs are derived fixtures; original P0 IDs use the original import");
        var rec = (catalog ?? DerivedCatalog.Records).FirstOrDefault(r => string.Equals(r.DerivedFixtureId, derivedFixtureId, StringComparison.Ordinal))
            ?? throw new P1Refusal("UnknownDerivedFixture", "fixture ID is not in the P1 derived catalog: " + derivedFixtureId);
        if (rec.Domain != P1Domain.DerivedP1ThirtyMetreExperimentV1)
            throw new P1Refusal("DomainUnavailable", "only the 30 m derived domain exists in F0");
        if (rec.Status != ReviewStatus.Accepted && use != TrustUse.ReviewEvidenceOnly)
            throw new P1Refusal("ManifestNotAccepted", "the derived manifest is pending independent review and Manager acceptance");
        return rec;
    }

    /// <summary>
    /// Loads a derived 30 m fixture from bytes through its compiled catalog record. Original P0 IDs and files are refused.
    /// A pending record loads only for reviewer evidence; it never unlocks an annual step (see <see cref="AnnualGate"/>).
    /// </summary>
    public static DerivedLoad LoadDerived(string derivedFixtureId, byte[] bytes, TrustUse use,
        IReadOnlyList<DerivedCatalogRecord>? catalog = null)
    {
        var rec = DerivedRecordFor(derivedFixtureId, use, catalog);
        if (bytes.Length != rec.FileByteLength) throw new P1Refusal("FileSizeMismatch", "derived fixture size differs from the catalog");
        if (DerivedManifest.Sha256Hex(bytes) != rec.FileSha256) throw new P1Refusal("FileHashMismatch", "derived fixture SHA-256 differs from the catalog");
        var m = DerivedManifest.Parse(bytes);
        if (m.DerivedFixtureId != rec.DerivedFixtureId) throw new P1Refusal("FixtureIdMismatch", "embedded ID differs from the catalog ID");
        if (m.ContentHash != rec.ContentHash) throw new P1Refusal("ContentHashMismatch", "content hash differs from the catalog");
        return new DerivedLoad(m, rec, rec.Status == ReviewStatus.Accepted ? "TRUSTED" : "UNTRUSTED-PENDING-REVIEW (reviewer evidence only)");
    }
}

/// <summary>
/// The single gate in front of any ecological year. In this F0 stage there is NO annual engine: the best possible answer
/// is a refusal that names why. Order: domain first, then catalog trust, then engine availability.
/// </summary>
public static class AnnualGate
{
    public static P1Refusal Check(P1Domain domain, DerivedCatalogRecord? record) => domain switch
    {
        P1Domain.OriginalPolygonExploratoryV1 => new P1Refusal("OriginalAnnualRefused",
            "original P0 fixtures are read-only in F0; annual advance waits for the later P1-POLY polygon engine"),
        P1Domain.DerivedUnitySquareParityV1 => new P1Refusal("DomainUnavailable",
            "the legal Unity 40/80 m domain needs a later C-6 capture and is not available in F0"),
        _ when record is null => new P1Refusal("UnknownDerivedFixture", "no catalog record"),
        _ when record.Domain != domain => new P1Refusal("DomainMismatch", "catalog record domain differs from the requested domain"),
        _ when record.Status != ReviewStatus.Accepted => new P1Refusal("ManifestNotAccepted",
            "no ecological year runs until the manifest and original trust records pass independent review and Manager acceptance"),
        _ => new P1Refusal("AnnualEngineNotImplemented", "the annual source-rule adapter is not part of this F0 stage"),
    };
}
