using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SitkaP1.Core;

/// <summary>One synthetic starting stem in the 30 m derived fixture.</summary>
public sealed class DerivedTree
{
    public string TreeId = "";
    public long XMm;
    public long YMm;
    public string Status = "live";
    public long AgeYears;
    public long DbhMm;
    public long HeightMm;
    // Derived values. Each must equal the documented conversion of the integer fields bit for bit.
    public uint DbhCmBits;
    public uint HeightMBits;
    public uint CrownRadiusMBits;
    public uint UnityXBits;
    public uint UnityZBits;
}

/// <summary>
/// The complete deterministic starting-forest manifest for <see cref="P1Domain.DerivedP1ThirtyMetreExperimentV1"/>
/// (packet "F0 fixture manifest hard gate"; spec §0 B01, §5.1). Format <c>ccf-sitka-p1-derived-fixture</c> v1,
/// suffix <c>.sitkap1fx.json</c>. Header values are fixed by this version; any other value is refused, never coerced.
/// </summary>
public sealed class DerivedManifest
{
    public const string Format = "ccf-sitka-p1-derived-fixture";
    public const int SchemaVersion = 1;
    public const string FileSuffix = ".sitkap1fx.json";
    public const string IdPrefix = "p1d-";
    public const string CatalogVersion = "p1-f0-derived-catalog-v1";
    public const string GeneratorId = "p1-f0-square30-generator";
    public const int GeneratorVersion = 1;
    public const string GeometryModel = "P1DerivedSquare30m";
    public const long SideMm = 30_000;
    public const long CentreMm = 15_000;
    public const string LocalFrame = "integer-mm XY; origin (0,0) at the south-west stand corner; +X east; +Y north; trees in [0,30000) x [0,30000)";
    public const string XzTransformId = "p1-square30-corner-to-centred-xz-v1";
    public const string XzRule = "UnityX=(float)((xMm-15000)/1000.0) m; UnityZ=(float)((yMm-15000)/1000.0) m; UnityY=0; exact int64 subtraction, then binary64 division, then one f32 cast";
    public const string AreaM2Exact = "900";
    public const string AreaHaExpression = "(float)(900.0/10000.0)";
    public const uint AreaHaBits = 0x3db851ec;   // nearest binary32 to 0.09 (0.0900000035762786865234375)
    public const string P1AlgorithmVersion = "p1-f0-adult-sitka-30m-v1";
    public const int GrowthModelVersion = 2;     // new-game source target; closed all-Reference Sitka uses the model-1 adult density route (spec §3)
    public const int RngModelVersion = 1;        // SimulationRandom.MixedModel
    public const uint SiteProductivityBits = 0x3f800000; // 1.0f: current source value (ForestEcologyCell.SiteProductivity has no writer)
    public const string Stage = "MatureEquivalent";
    public const string Species = "Sitka";
    public const string DemandClass = "Reference";
    public const string CrownRuleId = "potential-crown-at-ci0-linear-f32-v1";
    public const string CrownRule = "r0=MathF.Max(0.1f, 0.9415f + 0.07635f*dbhCm) evaluated in .NET float32 with dbhCm=(float)(dbhMm/10.0); source linear crown potential at CI=0 (usePowerLawCrown: 0); synthetic initialisation, not a measured crown or a Unity-observed value";
    public const string DbhRule = "dbhCm=(float)(dbhMm/10.0)";
    public const string HeightRule = "heightM=(float)(heightMm/1000.0)";
    public static readonly string[] ExcludedSystems =
        { "browsing", "canopyLight", "juvenileLifecycle", "mixedSpecies", "pruning", "recruitment", "regeneration", "storm" };
    public const int MaxTrees = 10_000;           // P1 F0 placeholder overflow bound, unvalidated; not a biological claim
    public const long MinDbhMm = 10, MaxDbhMm = 2_000;          // source ForestTree.ApplyGrowth DBH clamp [1, 200] cm
    public const long MinHeightMm = 100, MaxHeightMm = 60_000;  // source height clamp [0.1, 60] m
    public const long MaxAgeYears = 10_000;       // integer headroom only; no biological age limit is claimed

    private static readonly Regex SafeId = new("^[A-Za-z0-9_-]{1,64}$", RegexOptions.CultureInvariant);

    public string DerivedFixtureId = "";
    public long GeneratorSeed;
    public long SimulationSeed;
    public long InitialEcologicalYear;
    public long ActionEligibleDbhThresholdMm;
    public List<DerivedTree> Trees = new();
    public string ContentHash = "";

    // ---------------------------------------------------------------- conversions (the only definitions used anywhere)

    public static uint DbhCmBitsFor(long dbhMm) => F32.Bits((float)(dbhMm / 10.0));
    public static uint HeightMBitsFor(long heightMm) => F32.Bits((float)(heightMm / 1000.0));
    public static uint UnityAxisBitsFor(long mm) => F32.Bits((float)((mm - CentreMm) / 1000.0));
    public static uint CrownBitsFor(long dbhMm)
    {
        float dbhCm = (float)(dbhMm / 10.0);
        float r = MathF.Max(CcfSourcePins.CrownRadiusFloorM, CcfSourcePins.CrownRadiusIntercept + CcfSourcePins.CrownRadiusPerCmDbh * dbhCm);
        return F32.Bits(r);
    }

    public static string Sha256Hex(byte[] bytes) => Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();

    // ---------------------------------------------------------------- canonical serialization

    /// <summary>Canonical bytes of everything except <c>contentHash</c>; the content hash is SHA-256 of exactly these bytes.</summary>
    public byte[] BodyBytes() => Write(includeHash: false).ToBytes();

    /// <summary>The complete canonical file bytes (no trailing newline).</summary>
    public byte[] FileBytes() => Write(includeHash: true).ToBytes();

    private CanonicalWriter Write(bool includeHash)
    {
        var w = new CanonicalWriter().BeginObject()
            .Str("format", Format)
            .Int("derivedSchemaVersion", SchemaVersion)
            .Str("sourceClass", "Synthetic")
            .Str("domain", P1DomainWire.Name(P1Domain.DerivedP1ThirtyMetreExperimentV1))
            .Str("derivedFixtureId", DerivedFixtureId)
            .Str("catalogVersion", CatalogVersion)
            .Str("disclosure", P1Disclosure.Derived30);

        w.BeginObject("derivation")
            .Str("kind", "designed")
            .Str("generatorId", GeneratorId)
            .Int("generatorVersion", GeneratorVersion)
            .Int("generatorSeed", GeneratorSeed)
            .StrOrNull("originalP0FixtureId", null)
            .Str("note", "Designed P1 fixture. Not derived from, rescaled from or trimmed from any original P0 fixture.")
            .EndObject();

        w.BeginObject("geometry")
            .Str("geometryModel", GeometryModel)
            .Int("sideMm", SideMm)
            .Str("localFrame", LocalFrame);
        w.BeginArray("polygonMm");
        foreach (var (x, y) in new[] { (0L, 0L), (SideMm, 0L), (SideMm, SideMm), (0L, SideMm) })
            w.BeginObject().Int("xMm", x).Int("yMm", y).EndObject();
        w.EndArray()
            .Int("elevationMm", 0)
            .Str("xzTransformId", XzTransformId)
            .Str("xzRule", XzRule)
            .EndObject();

        w.BeginObject("area")
            .Str("areaM2Exact", AreaM2Exact)
            .Str("areaHaExpression", AreaHaExpression)
            .F32("areaHa", AreaHaBits)
            .Str("areaPolicy", "declared 0.09 ha for N/ha and RD; not the Unity standSizeMeters^2 expression (no 30 m Unity stand exists)")
            .EndObject();

        w.BeginObject("ecology")
            .Int("initialEcologicalYear", InitialEcologicalYear)
            .Int("firstAdvanceEcologicalYear", InitialEcologicalYear + 1)
            .StrOrNull("sourceInventoryYear", null)
            .Int("simulationSeed", SimulationSeed)
            .Int("growthModelVersion", GrowthModelVersion)
            .Int("rngModelVersion", RngModelVersion)
            .Str("p1AlgorithmVersion", P1AlgorithmVersion)
            .F32("siteProductivity", SiteProductivityBits)
            .Str("stage", Stage)
            .Str("species", Species)
            .Str("demandClass", DemandClass)
            .Int("pruning", 0)
            .Int("actionEligibleDbhThresholdMm", ActionEligibleDbhThresholdMm)
            .Str("populationRule", "every living stem is a synthetic Mature-equivalent Sitka and enters CI, RD, growth and natural mortality, including stems below the action threshold")
            .StrArray("excludedSystems", ExcludedSystems)
            .EndObject();

        w.BeginObject("conversions")
            .Str("dbhRule", DbhRule)
            .Str("heightRule", HeightRule)
            .Str("crownRuleId", CrownRuleId)
            .Str("crownRule", CrownRule)
            .EndObject();

        w.BeginObject("sourceProvenance")
            .Str("ccfInspectedCommit", CcfSourcePins.InspectedCommit)
            .Str("classification", "source-cited constants and rules are CCF source facts; tree positions, sizes, ages, seeds and crowns are synthetic P1 choices");
        w.BeginArray("files");
        foreach (var p in CcfSourcePins.Files)
            w.BeginObject().Str("path", p.Path).Str("role", p.Role).Str("gitBlob", p.GitBlob).Str("sha256", p.Sha256).EndObject();
        w.EndArray().EndObject();

        w.Int("treeCount", Trees.Count);
        w.BeginArray("trees");
        foreach (var t in Trees)
        {
            w.BeginObject()
                .Str("treeId", t.TreeId)
                .Int("xMm", t.XMm).Int("yMm", t.YMm)
                .Str("status", t.Status)
                .Int("ageYears", t.AgeYears)
                .Int("dbhMm", t.DbhMm).Int("heightMm", t.HeightMm)
                .F32("dbhCm", t.DbhCmBits).F32("heightM", t.HeightMBits).F32("crownRadiusM", t.CrownRadiusMBits)
                .F32("unityX", t.UnityXBits).F32("unityZ", t.UnityZBits)
                .EndObject();
        }
        w.EndArray();
        if (includeHash) w.Str("contentHash", ContentHash);
        return w.EndObject();
    }

    // ---------------------------------------------------------------- strict parse

    /// <summary>
    /// Strict parse: size cap, strict JSON, exact keys, fixed header values, per-tree domain and derived-value checks,
    /// content hash, then byte-for-byte canonical comparison. Throws <see cref="P1Refusal"/> on any defect.
    /// Domain is checked before anything else in the body so a cross-domain file is refused as such.
    /// </summary>
    public static DerivedManifest Parse(byte[] bytes)
    {
        if (bytes.Length > 16 * 1024 * 1024) throw new P1Refusal("FileTooLarge", "derived fixture exceeds 16 MiB");
        var root = J.Keys(StrictJson.Parse(bytes), "manifest",
            "format", "derivedSchemaVersion", "sourceClass", "domain", "derivedFixtureId", "catalogVersion", "disclosure",
            "derivation", "geometry", "area", "ecology", "conversions", "sourceProvenance", "treeCount", "trees", "contentHash");

        Expect(J.Str(root["format"], "format") == Format, "WrongFormat", "format must be " + Format);
        Expect(J.Int(root["derivedSchemaVersion"], "derivedSchemaVersion") == SchemaVersion, "UnsupportedVersion", "derivedSchemaVersion must be 1");
        Expect(J.Str(root["sourceClass"], "sourceClass") == "Synthetic", "SchemaInvalid", "sourceClass must be Synthetic");
        var domain = P1DomainWire.Parse(J.Str(root["domain"], "domain"));
        Expect(domain == P1Domain.DerivedP1ThirtyMetreExperimentV1, "DomainMismatch",
            "a derived 30 m fixture must carry DerivedP1ThirtyMetreExperimentV1, found " + P1DomainWire.Name(domain));

        var m = new DerivedManifest { DerivedFixtureId = J.Str(root["derivedFixtureId"], "derivedFixtureId") };
        Expect(m.DerivedFixtureId.StartsWith(IdPrefix, StringComparison.Ordinal) && SafeId.IsMatch(m.DerivedFixtureId),
            "NotDerivedFixtureId", "derived fixture IDs must be safe IDs starting with " + IdPrefix);
        Expect(J.Str(root["catalogVersion"], "catalogVersion") == CatalogVersion, "UnsupportedVersion", "unknown catalogVersion");
        Expect(J.Str(root["disclosure"], "disclosure") == P1Disclosure.Derived30, "DisclosureMissing", "disclosure text must be exact");

        var der = J.Keys(root["derivation"], "derivation", "kind", "generatorId", "generatorVersion", "generatorSeed", "originalP0FixtureId", "note");
        Expect(J.Str(der["kind"], "derivation.kind") == "designed", "DerivationInvalid", "derivation.kind must be designed");
        Expect(J.StrOrNull(der["originalP0FixtureId"], "derivation.originalP0FixtureId") is null, "DerivationInvalid",
            "a designed 30 m fixture must not claim an original P0 source");
        Expect(J.Str(der["generatorId"], "derivation.generatorId") == GeneratorId, "UnsupportedVersion", "unknown generatorId");
        Expect(J.Int(der["generatorVersion"], "derivation.generatorVersion") == GeneratorVersion, "UnsupportedVersion", "unknown generatorVersion");
        m.GeneratorSeed = J.Int(der["generatorSeed"], "derivation.generatorSeed");
        Expect(m.GeneratorSeed is >= int.MinValue and <= int.MaxValue, "SeedInvalid", "generatorSeed must fit int32");

        var geo = J.Keys(root["geometry"], "geometry", "geometryModel", "sideMm", "localFrame", "polygonMm", "elevationMm", "xzTransformId", "xzRule");
        Expect(J.Str(geo["geometryModel"], "geometry.geometryModel") == GeometryModel, "GeometryInvalid", "geometryModel must be " + GeometryModel);
        Expect(J.Int(geo["sideMm"], "geometry.sideMm") == SideMm, "GeometryInvalid", "sideMm must be 30000");
        Expect(J.Str(geo["localFrame"], "geometry.localFrame") == LocalFrame, "GeometryInvalid", "localFrame text must be exact");
        Expect(J.Int(geo["elevationMm"], "geometry.elevationMm") == 0, "GeometryInvalid", "elevation must be 0");
        Expect(J.Str(geo["xzTransformId"], "geometry.xzTransformId") == XzTransformId, "GeometryInvalid", "unknown xzTransformId");
        Expect(J.Str(geo["xzRule"], "geometry.xzRule") == XzRule, "GeometryInvalid", "xzRule text must be exact");
        var poly = J.Arr(geo["polygonMm"], "geometry.polygonMm");
        var expected = new[] { (0L, 0L), (SideMm, 0L), (SideMm, SideMm), (0L, SideMm) };
        Expect(poly.Items.Count == 4, "GeometryInvalid", "polygon must be the 4-vertex 30 m square");
        for (int i = 0; i < 4; i++)
        {
            var v = J.Keys(poly.Items[i], "polygon vertex", "xMm", "yMm");
            Expect(J.Int(v["xMm"], "xMm") == expected[i].Item1 && J.Int(v["yMm"], "yMm") == expected[i].Item2,
                "GeometryInvalid", "polygon vertices/order/origin differ from the declared square");
        }

        var area = J.Keys(root["area"], "area", "areaM2Exact", "areaHaExpression", "areaHa", "areaPolicy");
        Expect(J.Str(area["areaM2Exact"], "area.areaM2Exact") == AreaM2Exact, "AreaInvalid", "area must be exactly 900 m2");
        Expect(J.Str(area["areaHaExpression"], "area.areaHaExpression") == AreaHaExpression, "AreaInvalid", "area expression must be exact");
        Expect(J.F32Bits(area["areaHa"], "area.areaHa") == AreaHaBits, "AreaInvalid", "areaHa bits must be 3db851ec");
        Expect(F32.Bits((float)(900.0 / 10000.0)) == AreaHaBits, "AreaInvalid", "runtime evaluation of the area expression differs");
        J.Str(area["areaPolicy"], "area.areaPolicy");

        var eco = J.Keys(root["ecology"], "ecology", "initialEcologicalYear", "firstAdvanceEcologicalYear", "sourceInventoryYear",
            "simulationSeed", "growthModelVersion", "rngModelVersion", "p1AlgorithmVersion", "siteProductivity", "stage", "species",
            "demandClass", "pruning", "actionEligibleDbhThresholdMm", "populationRule", "excludedSystems");
        m.InitialEcologicalYear = J.Int(eco["initialEcologicalYear"], "ecology.initialEcologicalYear");
        Expect(m.InitialEcologicalYear == 0, "EcologyInvalid", "initialEcologicalYear must be 0 in v1");
        Expect(J.Int(eco["firstAdvanceEcologicalYear"], "ecology.firstAdvanceEcologicalYear") == 1, "EcologyInvalid", "first advance must be year 1");
        Expect(J.StrOrNull(eco["sourceInventoryYear"], "ecology.sourceInventoryYear") is null, "EcologyInvalid",
            "a designed fixture has no inventory year; calendar years never enter the RNG");
        m.SimulationSeed = J.Int(eco["simulationSeed"], "ecology.simulationSeed");
        Expect(m.SimulationSeed is >= int.MinValue and <= int.MaxValue, "SeedInvalid", "simulationSeed must fit int32 (source int)");
        Expect(J.Int(eco["growthModelVersion"], "ecology.growthModelVersion") == GrowthModelVersion, "UnsupportedVersion", "growthModelVersion must be 2");
        Expect(J.Int(eco["rngModelVersion"], "ecology.rngModelVersion") == RngModelVersion, "UnsupportedVersion", "rngModelVersion must be 1");
        Expect(J.Str(eco["p1AlgorithmVersion"], "ecology.p1AlgorithmVersion") == P1AlgorithmVersion, "UnsupportedVersion", "unknown p1AlgorithmVersion");
        Expect(J.F32Bits(eco["siteProductivity"], "ecology.siteProductivity") == SiteProductivityBits, "EcologyInvalid", "siteProductivity must be 1.0f");
        Expect(J.Str(eco["stage"], "ecology.stage") == Stage, "EcologyInvalid", "stage must be MatureEquivalent");
        Expect(J.Str(eco["species"], "ecology.species") == Species, "EcologyInvalid", "species must be Sitka");
        Expect(J.Str(eco["demandClass"], "ecology.demandClass") == DemandClass, "EcologyInvalid", "demandClass must be Reference");
        Expect(J.Int(eco["pruning"], "ecology.pruning") == 0, "EcologyInvalid", "pruning must be 0");
        m.ActionEligibleDbhThresholdMm = J.Int(eco["actionEligibleDbhThresholdMm"], "ecology.actionEligibleDbhThresholdMm");
        Expect(m.ActionEligibleDbhThresholdMm is >= MinDbhMm and <= MaxDbhMm, "EcologyInvalid", "threshold outside the DBH domain");
        J.Str(eco["populationRule"], "ecology.populationRule");
        var excl = J.Arr(eco["excludedSystems"], "ecology.excludedSystems").Items.Select(n => J.Str(n, "excludedSystems item")).ToArray();
        Expect(excl.SequenceEqual(ExcludedSystems), "EcologyInvalid", "excludedSystems must be the exact v1 list");

        var conv = J.Keys(root["conversions"], "conversions", "dbhRule", "heightRule", "crownRuleId", "crownRule");
        Expect(J.Str(conv["crownRuleId"], "conversions.crownRuleId") == CrownRuleId, "UnsupportedVersion", "unknown crown initialisation rule");
        Expect(J.Str(conv["dbhRule"], "dbhRule") == DbhRule && J.Str(conv["heightRule"], "heightRule") == HeightRule
            && J.Str(conv["crownRule"], "crownRule") == CrownRule, "SchemaInvalid", "conversion rule text must be exact");

        var prov = J.Keys(root["sourceProvenance"], "sourceProvenance", "ccfInspectedCommit", "classification", "files");
        Expect(J.Str(prov["ccfInspectedCommit"], "ccfInspectedCommit") == CcfSourcePins.InspectedCommit, "SourceProvenanceMismatch", "source commit differs");
        J.Str(prov["classification"], "sourceProvenance.classification");
        var files = J.Arr(prov["files"], "sourceProvenance.files");
        Expect(files.Items.Count == CcfSourcePins.Files.Count, "SourceProvenanceMismatch", "source file list differs");
        for (int i = 0; i < files.Items.Count; i++)
        {
            var f = J.Keys(files.Items[i], "source file", "path", "role", "gitBlob", "sha256");
            var p = CcfSourcePins.Files[i];
            Expect(J.Str(f["path"], "path") == p.Path && J.Str(f["role"], "role") == p.Role && J.Str(f["gitBlob"], "gitBlob") == p.GitBlob
                && J.Str(f["sha256"], "sha256") == p.Sha256, "SourceProvenanceMismatch", "source file pin differs: " + p.Path);
        }

        long treeCount = J.Int(root["treeCount"], "treeCount");
        var trees = J.Arr(root["trees"], "trees");
        Expect(treeCount == trees.Items.Count, "TreeCountMismatch", "treeCount differs from the number of trees");
        Expect(treeCount is >= 1 and <= MaxTrees, "TreeCountMismatch", "tree count outside 1..10000");
        string? previous = null;
        foreach (var node in trees.Items)
        {
            var o = J.Keys(node, "tree", "treeId", "xMm", "yMm", "status", "ageYears", "dbhMm", "heightMm",
                "dbhCm", "heightM", "crownRadiusM", "unityX", "unityZ");
            var t = new DerivedTree
            {
                TreeId = J.Str(o["treeId"], "treeId"),
                XMm = J.Int(o["xMm"], "xMm"),
                YMm = J.Int(o["yMm"], "yMm"),
                Status = J.Str(o["status"], "status"),
                AgeYears = J.Int(o["ageYears"], "ageYears"),
                DbhMm = J.Int(o["dbhMm"], "dbhMm"),
                HeightMm = J.Int(o["heightMm"], "heightMm"),
                DbhCmBits = J.F32Bits(o["dbhCm"], "dbhCm"),
                HeightMBits = J.F32Bits(o["heightM"], "heightM"),
                CrownRadiusMBits = J.F32Bits(o["crownRadiusM"], "crownRadiusM"),
                UnityXBits = J.F32Bits(o["unityX"], "unityX"),
                UnityZBits = J.F32Bits(o["unityZ"], "unityZ"),
            };
            ValidateTree(t, previous);
            previous = t.TreeId;
            m.Trees.Add(t);
        }

        m.ContentHash = J.Str(root["contentHash"], "contentHash");
        Expect(m.ContentHash == Sha256Hex(m.BodyBytes()), "ContentHashMismatch", "contentHash is not the SHA-256 of the canonical body");
        Expect(m.FileBytes().AsSpan().SequenceEqual(bytes), "NotCanonical", "file bytes are not the canonical v1 serialization");
        return m;
    }

    public static void ValidateTree(DerivedTree t, string? previousId)
    {
        Expect(SafeId.IsMatch(t.TreeId), "TreeIdInvalid", "tree ID is not a safe ID: " + t.TreeId);
        Expect(previousId is null || string.CompareOrdinal(previousId, t.TreeId) < 0, "TreeIdOrder",
            "tree IDs must be unique and in ordinal order: " + t.TreeId);
        Expect(t.XMm is >= 0 and < SideMm && t.YMm is >= 0 and < SideMm, "TreeOutsideStand", "tree outside [0,30000) mm: " + t.TreeId);
        Expect(t.Status == "live", "StatusInvalid", "v1 derived fixtures contain living stems only: " + t.TreeId);
        Expect(t.AgeYears is >= 1 and <= MaxAgeYears, "AgeInvalid", "age outside 1..10000: " + t.TreeId);
        Expect(t.DbhMm is >= MinDbhMm and <= MaxDbhMm, "DbhOutOfDomain", "DBH outside [1,200] cm: " + t.TreeId);
        Expect(t.HeightMm is >= MinHeightMm and <= MaxHeightMm, "HeightOutOfDomain", "height outside [0.1,60] m: " + t.TreeId);
        Expect(t.DbhCmBits == DbhCmBitsFor(t.DbhMm) && t.HeightMBits == HeightMBitsFor(t.HeightMm)
            && t.CrownRadiusMBits == CrownBitsFor(t.DbhMm), "DerivedValueMismatch", "f32 DBH/height/crown differ from the declared conversion: " + t.TreeId);
        Expect(t.UnityXBits == UnityAxisBitsFor(t.XMm) && t.UnityZBits == UnityAxisBitsFor(t.YMm), "DerivedValueMismatch",
            "XZ coordinates differ from the declared transform: " + t.TreeId);
    }

    private static void Expect(bool ok, string code, string message)
    {
        if (!ok) throw new P1Refusal(code, message);
    }
}
