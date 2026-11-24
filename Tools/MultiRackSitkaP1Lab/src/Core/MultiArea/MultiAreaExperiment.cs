using System.Text.RegularExpressions;

namespace SitkaP1.Core;

/// <summary>
/// Fixed identity and honesty text of the MA-1 multi-area experiment family (owner-approved MA-1 spec §1–§2). It is a
/// separate family from the P1 30 m fixture: its own format, domain string and "mad-" ID prefix, which the P1 parsers
/// refuse by construction (the domain is not a <see cref="P1Domain"/> value and the ID has no "p1d-" prefix).
/// </summary>
internal static class MultiAreaIdentity
{
    internal const string Format = "ccf-sitka-multiarea-experiment";
    internal const int SchemaVersion = 1;
    internal const string Domain = "MultiAreaSitkaExperimentV1";
    internal const string IdPrefix = "mad-";
    internal const string SourceClass = "SyntheticAuthored";
    // The author never marks its own experiment as accepted (same rule as TrustTables.cs).
    internal const string ReviewStatus = "PendingIndependentReview";
    internal const string AreaModelVersion = "ma1-isolated-per-area-v1";
    // CrownModel 1 sessions only (#899, D-094): the crown neighbourhood is the connected woodland; everything else per area.
    internal const string AreaModelVersionConnectedCrownC2 = "ma1-connected-woodland-crown-c2-v1";
    internal const string Site = "UNKNOWN";
    internal const string Disclosure =
        "SYNTHETIC AUTHORED TWO-AREA SITKA EXPERIMENT. NOT A SURVEYED IRISH FOREST. AREAS NOT ASSESSED COMPARABLE. " +
        "SITE UNKNOWN. EDGE NEIGHBOURS EXCLUDED BY THIS INITIAL MODEL. NO TREATMENT TRANSFER BETWEEN AREAS. " +
        "NOT UNITY NUMERICAL OR GEOMETRY PARITY.";
    internal const string EdgeModel =
        "isolated per area: competition, growth and mortality use only the trees of the same area; " +
        "neighbours across an area boundary are excluded";
    internal const string EdgeModelConnectedCrownC2 =
        "CrownModel 1 (area-scaled-equilibrium-c2-v1): crown initialisation and annual crown response use Hegyi competition " +
        "over every living tree of the connected woodland (both areas, 8 m); the outer perimeter borders open grassland with " +
        "no synthetic neighbours; DBH growth, height, mortality, Work Plans and summaries remain per area";
    // Stage 4 sessions only (D-095): adult DBH and crown use the connected woodland; density mortality stays per rack.
    internal const string AreaModelVersionStage4 = "ma1-connected-woodland-stage4-adult-mortality-v1";
    internal const string EdgeModelStage4 =
        "Stage 4 (AdultModelVersion 1, MortalityModelVersion 1, CrownModelVersion 1): adult DBH growth and the C2 crown " +
        "response use Hegyi competition over every living tree of the connected woodland (both areas, 8 m); density " +
        "mortality (relative density and suppression) is evaluated per rack from that rack's own living stems and polygon " +
        "area, never pooled; the outer perimeter borders open grassland with no synthetic neighbours";
    internal const string SiteNoteStage4 =
        "site UNKNOWN; Stage 4 reads a per-position site-productivity seam that is exactly 1.0 everywhere (no soil map)";
    internal const string SiteNote =
        "site UNKNOWN; the unchanged engine uses its constant site value 1.0f for every area; no per-area site value is active";
    internal const string LocalFrame =
        "integer-mm XY experiment frame; +X east; +Y north; engine XM=(float)(xMm/1000.0) m, ZM=(float)(yMm/1000.0) m";
    internal const string AreaHaExpression = "(float)((double)twiceShoelaceMm2/2e10)";
    // Fell eligibility is not restricted by size (D-093 P1); the value is carried as informational metadata, as in P1.
    internal const long ActionEligibleDbhThresholdMm = ManifestGenerator.ActionThresholdMm;
}

/// <summary>One synthetic starting stem: exact integer source values. Every stem is living at genesis.</summary>
internal sealed record MultiAreaTreeDefinition(string TreeId, long XMm, long YMm, int AgeYears, long DbhMm, long HeightMm);

/// <summary>One management area: a neutral ID, a strictly convex counter-clockwise polygon, its own seed and its trees.</summary>
internal sealed record MultiAreaDefinition(string AreaId, string Label, string StockingNote,
    IReadOnlyList<MillimetrePoint> PolygonMm, int SimulationSeed, IReadOnlyList<MultiAreaTreeDefinition> Trees)
{
    internal long TwiceAreaMm2 => MultiAreaGeometry.TwiceSignedAreaMm2(PolygonMm);
    internal float AreaHa => MultiAreaGeometry.Hectares(TwiceAreaMm2);
    internal string AreaM2Exact => MultiAreaGeometry.ExactSquareMetres(TwiceAreaMm2);
}

/// <summary>
/// The whole versioned synthetic experiment. It is generated in code (no file is read) and published as canonical
/// manifest bytes with a SHA-256 content hash, so anyone can regenerate it and compare the hash.
/// </summary>
internal sealed record MultiAreaExperiment(string ExperimentId, string GeneratorId, int GeneratorVersion, long GeneratorSeed,
    IReadOnlyList<MultiAreaDefinition> Areas)
{
    /// <summary>Optional disclosure override for non-MA-1 synthetic demos; null keeps <see cref="MultiAreaIdentity.Disclosure"/> byte-identical.</summary>
    internal string? DisclosureOverride { get; init; }

    private static readonly Regex SafeId = new("^[A-Za-z0-9_-]{1,64}$", RegexOptions.CultureInvariant);
    private static readonly Regex AreaIdPattern = new("^MA-[A-Z0-9]{1,16}$", RegexOptions.CultureInvariant);

    /// <summary>
    /// The identity and isolation rules of #852 §3.4 as adapted for MA-1. Any defect throws a <see cref="P1Refusal"/>;
    /// nothing is coerced. The checks are ordered so each test mutation is refused for its own reason.
    /// </summary>
    internal void Validate()
    {
        Expect(ExperimentId is not null && ExperimentId.StartsWith(MultiAreaIdentity.IdPrefix, StringComparison.Ordinal) &&
            SafeId.IsMatch(ExperimentId), "ExperimentIdInvalid", "an experiment ID is a safe ID starting with mad-");
        Expect(Areas is { Count: >= 2 }, "AreaCountInvalid", "a multi-area experiment needs at least two areas");

        string? previousArea = null;
        var seeds = new HashSet<int>();
        var allIds = new HashSet<string>(StringComparer.Ordinal);
        var foldedIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var area in Areas!)
        {
            Expect(area.AreaId is not null && AreaIdPattern.IsMatch(area.AreaId), "AreaIdInvalid",
                "area IDs are neutral MA-<letters/digits> names");
            Expect(previousArea is null || string.CompareOrdinal(previousArea, area.AreaId) < 0, "AreaIdOrder",
                "area IDs must be unique and in ordinal order");
            previousArea = area.AreaId;
            MultiAreaGeometry.RequireStrictlyConvexCounterClockwise(area.PolygonMm, area.AreaId + " polygon");
            Expect(seeds.Add(area.SimulationSeed), "SeedNotUnique", "every area needs its own simulation seed");
            Expect(area.Trees is { Count: >= 1 }, "AreaEmpty", "every area starts with at least one living tree");

            string? previousTree = null;
            var positions = new HashSet<(long, long)>();
            foreach (var tree in area.Trees!)
            {
                Expect(tree.TreeId is not null && SafeId.IsMatch(tree.TreeId), "TreeIdInvalid", "tree IDs are safe IDs");
                Expect(!tree.TreeId!.StartsWith("P30-", StringComparison.OrdinalIgnoreCase), "TreeIdReservedFamily",
                    "P30- IDs belong to the original 30 m roster and are never reused");
                Expect(tree.TreeId.StartsWith(area.AreaId + "-", StringComparison.Ordinal), "TreeAreaMismatch",
                    "every tree ID starts with its own area ID: " + tree.TreeId);
                Expect(previousTree is null || string.CompareOrdinal(previousTree, tree.TreeId) < 0, "TreeIdOrder",
                    "tree IDs must be unique and in ordinal order within an area: " + tree.TreeId);
                previousTree = tree.TreeId;
                Expect(allIds.Add(tree.TreeId), "TreeIdNotUnique", "tree IDs are unique across the experiment: " + tree.TreeId);
                Expect(foldedIds.Add(tree.TreeId), "TreeIdCaseCollision", "two tree IDs differ only by letter case: " + tree.TreeId);
                Expect(MultiAreaGeometry.StrictlyInside(area.PolygonMm, new MillimetrePoint(tree.XMm, tree.YMm)),
                    "TreeOutsideArea", "a tree must lie strictly inside its own area polygon: " + tree.TreeId);
                Expect(positions.Add((tree.XMm, tree.YMm)), "DuplicatePosition", "two trees share one position: " + tree.TreeId);
                Expect(tree.AgeYears is >= 1 and <= (int)DerivedManifest.MaxAgeYears, "AgeInvalid", "age outside 1..10000: " + tree.TreeId);
                Expect(tree.DbhMm is >= DerivedManifest.MinDbhMm and <= DerivedManifest.MaxDbhMm, "DbhOutOfDomain",
                    "DBH outside [1, 200] cm: " + tree.TreeId);
                Expect(tree.HeightMm is >= DerivedManifest.MinHeightMm and <= DerivedManifest.MaxHeightMm, "HeightOutOfDomain",
                    "height outside [0.1, 60] m: " + tree.TreeId);
            }
        }

        for (int i = 0; i < Areas!.Count; i++)
        for (int j = i + 1; j < Areas.Count; j++)
            Expect(!MultiAreaGeometry.Overlap(Areas[i].PolygonMm, Areas[j].PolygonMm), "AreasOverlap",
                Areas[i].AreaId + " and " + Areas[j].AreaId + " overlap; areas may touch but never share ground");
    }

    /// <summary>The engine's starting roster for one area: exact integer values converted by the P1 conversion rules.</summary>
    internal static IReadOnlyList<AnnualTree> GenesisTrees(MultiAreaDefinition area) => Array.AsReadOnly(area.Trees
        .Select(tree => new AnnualTree(tree.TreeId, F32.FromBits(AxisBits(tree.XMm)), F32.FromBits(AxisBits(tree.YMm)),
            tree.AgeYears, F32.FromBits(DerivedManifest.DbhCmBitsFor(tree.DbhMm)),
            F32.FromBits(DerivedManifest.HeightMBitsFor(tree.HeightMm)), F32.FromBits(DerivedManifest.CrownBitsFor(tree.DbhMm))))
        .ToArray());

    /// <summary>Experiment-frame millimetres to engine metres: one binary64 division, then one float32 cast.</summary>
    internal static uint AxisBits(long mm) => F32.Bits((float)(mm / 1000.0));

    // ---------------------------------------------------------------- canonical manifest

    /// <summary>Canonical bytes of everything except <c>contentHash</c>; the content hash is SHA-256 of exactly these bytes.</summary>
    internal byte[] ManifestBodyBytes() => Write(includeHash: false).ToBytes();

    /// <summary>The complete canonical manifest (no trailing newline): the body with its content hash as the last member.</summary>
    internal byte[] ManifestFileBytes() => Write(includeHash: true).ToBytes();

    internal string ContentHash() => DerivedManifest.Sha256Hex(ManifestBodyBytes());

    private CanonicalWriter Write(bool includeHash)
    {
        var w = new CanonicalWriter().BeginObject()
            .Str("format", MultiAreaIdentity.Format)
            .Int("schemaVersion", MultiAreaIdentity.SchemaVersion)
            .Str("experimentId", ExperimentId)
            .Str("domain", MultiAreaIdentity.Domain)
            .Str("sourceClass", MultiAreaIdentity.SourceClass)
            .Str("reviewStatus", MultiAreaIdentity.ReviewStatus)
            .Str("disclosure", DisclosureOverride ?? MultiAreaIdentity.Disclosure);
        w.BeginObject("generator")
            .Str("id", GeneratorId)
            .Int("version", GeneratorVersion)
            .Int("seed", GeneratorSeed)
            .Str("note", "Designed in code by the MA-1 author. Not derived, sliced or rescaled from the P1 30 m fixture or any P0 fixture.")
            .EndObject();
        w.BeginObject("models")
            .Int("rngModelVersion", DerivedManifest.RngModelVersion)
            .Int("growthModelVersion", DerivedManifest.GrowthModelVersion)
            .Str("annualRules", DerivedManifest.P1AlgorithmVersion)
            .Str("areaModel", MultiAreaIdentity.AreaModelVersion)
            .Str("edgeModel", MultiAreaIdentity.EdgeModel)
            .F32("siteProductivityInEngine", DerivedManifest.SiteProductivityBits)
            .Str("siteNote", MultiAreaIdentity.SiteNote)
            .Str("conversions", "dbhCm=(float)(dbhMm/10.0); heightM=(float)(heightMm/1000.0); crown " + DerivedManifest.CrownRuleId)
            .EndObject();
        w.Str("localFrame", MultiAreaIdentity.LocalFrame)
            .Int("initialEcologicalYear", 0)
            .Int("actionEligibleDbhThresholdMm", MultiAreaIdentity.ActionEligibleDbhThresholdMm)
            .Str("areaHaExpression", MultiAreaIdentity.AreaHaExpression);
        w.BeginArray("areas");
        foreach (var area in Areas)
        {
            w.BeginObject()
                .Str("areaId", area.AreaId)
                .Str("label", area.Label)
                .Str("stocking", area.StockingNote)
                .Str("site", MultiAreaIdentity.Site);
            w.BeginArray("polygonMm");
            foreach (var p in area.PolygonMm) w.BeginObject().Int("xMm", p.XMm).Int("yMm", p.YMm).EndObject();
            w.EndArray()
                .Int("twiceShoelaceMm2", area.TwiceAreaMm2)
                .Str("areaM2Exact", area.AreaM2Exact)
                .F32("areaHa", F32.Bits(area.AreaHa))
                .Int("simulationSeed", area.SimulationSeed)
                .Int("treeCount", area.Trees.Count);
            w.BeginArray("trees");
            foreach (var t in area.Trees)
            {
                w.BeginObject()
                    .Str("treeId", t.TreeId)
                    .Int("xMm", t.XMm).Int("yMm", t.YMm)
                    .Int("ageYears", t.AgeYears)
                    .Int("dbhMm", t.DbhMm).Int("heightMm", t.HeightMm)
                    .F32("dbhCm", DerivedManifest.DbhCmBitsFor(t.DbhMm))
                    .F32("heightM", DerivedManifest.HeightMBitsFor(t.HeightMm))
                    .F32("crownRadiusM", DerivedManifest.CrownBitsFor(t.DbhMm))
                    .F32("xM", AxisBits(t.XMm)).F32("zM", AxisBits(t.YMm))
                    .EndObject();
            }
            w.EndArray().EndObject();
        }
        w.EndArray();
        if (includeHash) w.Str("contentHash", ContentHash());
        return w.EndObject();
    }

    private static void Expect(bool ok, string code, string message)
    {
        if (!ok) throw new P1Refusal(code, message);
    }
}
