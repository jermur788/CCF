// Plain .NET checks of MFM-04, the deterministic prescription core: ScenarioOneCoverage, ScenarioOnePrescription,
// ScenarioOneCropTreeCandidates and ScenarioOnePrescriptionCalibration. Decision rules only; no Unity, no save, no scene.
//
// Fixture: the MFM-03 frozen Enlarged80 Rack V2 starting stand (1,344 Sitka stems, year 0) with the five MFM-02 qualified racks and the
// MFM-03 infrastructure masks. Its identity with the live MFM-02 Unity record is re-checked here (75 stems / 1.4417531 m² / SHA
// c2f2fd5418794915). Expected per-area values are MFM-03's evidence results.json (SHA-256 aa73c75f...7c31), so this check proves the
// C# core implements the accepted calibration definitions rather than a re-derivation of them.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Cov = ScenarioOneCoverage;
using Pres = ScenarioOnePrescription;
using Cand = ScenarioOneCropTreeCandidates;
using Cal = ScenarioOnePrescriptionCalibration;

public static class PrescriptionCoreOfflineCheck
{
    private static int checks, failures;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) failures++;
    }
    private static string F(double v) => v.ToString("0.######", Inv);

    // ---- fixture constants (MFM-02 handoff / MFM-03 exporter) ----
    private static readonly float[] RackZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private const float EntranceX = -35.4f, FarX = 40f, Half = 40f;
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private const double LiveInitialReleaseBaShare = 0.17898431420326233;

    // MFM-03 results.json, anchor-aligned thirds: id (W/C/E = L/C/H here), stems, net m², dDensity, dDbh, dCi (percent).
    private static readonly (string Id, int Stems, double Net, double DDens, double DDbh, double DCi)[] Mfm03 =
    {
        ("R1L", 61, 240.062, 4.964, -0.260, -33.173), ("R1C", 53, 221.392, -1.111, -2.107, -16.642), ("R1H", 72, 329.099, -9.627, 0.861, -12.877),
        ("R2L", 92, 392.984, -3.296, -0.524, -8.743), ("R2C", 65, 306.166, -12.302, 2.187, -15.093), ("R2H", 98, 429.932, -5.841, -0.441, 4.675),
        ("R3L", 86, 351.742, 0.997, 0.259, -2.218), ("R3H", 96, 383.814, 3.320, -0.670, 6.652),
        ("R4L", 100, 386.616, 6.845, 0.726, 3.447), ("R4C", 74, 275.589, 10.918, 2.339, 13.693), ("R4H", 96, 422.718, -6.189, -0.151, 5.160),
        ("R5L", 77, 314.455, 1.150, 0.715, -6.644), ("R5C", 68, 290.089, -3.170, 3.299, -8.254), ("R5H", 79, 338.438, -3.577, 1.973, -12.043),
    };
    private static readonly (string Id, int Edge)[] Mfm03Edge =
        { ("R1L", 45), ("R1C", 26), ("R1H", 47), ("R2L", 23), ("R2C", 0), ("R2H", 29), ("R3L", 22), ("R3H", 27), ("R4L", 25), ("R4C", 0), ("R4H", 28), ("R5L", 43), ("R5C", 33), ("R5H", 47) };
    // MFM-03 16 m unit-centre-first candidates, in order.
    private static readonly string[] Mfm03Candidates16 =
        { "PO0321", "PO0333", "PO1107", "P0006", "PO1134", "PO2107", "PO1700", "PO2133", "PO1740", "PO2907", "P2010", "PO2933", "PO3709", "PO3501", "PO3919", "PO3736", "PO4028" };

    private sealed class Fixture
    {
        public ScenarioOneRackSampleGeometry.Result Geometry;
        public List<Cov.Stem> Stems;            // post-access: rack removals are stumps
        public List<ScenarioOneRackBay.Order> Orders;  // completed rack-access orders
        public List<Cov.Mask> Masks;
        public HashSet<string> RackRemovals;
    }

    public static int Main(string[] args)
    {
        if (args.Length < 3) { Console.Error.WriteLine("usage: <repo-root> <fixture-stand.csv> <fixture-infrastructure.json>"); return 2; }
        string root = args[0];
        SourceParity(root);
        StaticGuards(root);
        Fixture fx = Load(root, args[1], args[2]);
        if (fx == null) { Console.WriteLine("MFM04_PRESCRIPTION_CORE_FAIL fixture"); return 1; }

        Cov.Result coverage = Coverage(fx);
        Boundaries();
        CalibrationVariants(fx);
        Exclusions(fx, coverage);
        Cand.Result candidates = Candidates(fx, coverage);
        Pres.ReleasePlan release = Release(fx, coverage, candidates);
        string fitDigest = DerivationAndFit(fx, coverage);
        Determinism(fx, coverage, candidates, release);

        Console.WriteLine("MFM04_DIGEST coverage=" + Sha(coverage.Signature()) + " candidates=" + Sha(candidates.Signature())
            + " release=" + Sha(release.Signature()) + " fit=" + fitDigest + " calibration=" + Sha(Cal.V1.Signature()));
        Console.WriteLine((failures == 0 ? "MFM04_PRESCRIPTION_CORE_PASS" : "MFM04_PRESCRIPTION_CORE_FAIL") + " checks=" + checks + " failed=" + failures);
        return failures == 0 ? 0 : 1;
    }

    // ------------------------------------------------------------------------------------------------------- authority / static

    private static void SourceParity(string root)
    {
        string eco = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ForestEcologyController.cs"));
        Check(eco.Contains("public const float HegyiCutoffMeters = 8f;"), "parity: production Hegyi cutoff is 8 m");
        Check(eco.Contains("return (neighbourDbhCm / Mathf.Max(1f, targetDbhCm)) / Mathf.Max(0.5f, distanceMeters);"), "parity: production HegyiTerm matches the offline stand-in");
        Check(ForestEcologyController.HegyiCutoffMeters == 8f, "parity: offline stand-in cutoff is 8 m");
        string sitka = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "SitkaGrowthModel.cs"));
        Check(sitka.Contains("public const string SpeciesId = \"" + Cov.SitkaSpeciesId + "\";"), "parity: coverage Sitka id equals SitkaGrowthModel.SpeciesId");
        Check(ScenarioOneRackBay.FirstReleaseShare == 0.30f && ScenarioOneRackBay.MaxRemovedBasalShare == 0.40f
              && ScenarioOneRackBay.FutureTreeSpacingM == 8f && ScenarioOneRackBay.TeachingHalfWidthM == 9f && ScenarioOneRackBay.HalfLengthM == 12f,
            "protected: 30% release, 40% safeguard, 8 m teaching spacing, 9 m x 12 m sample unchanged");
        Check(Cal.V1.CandidateSpacingM != ScenarioOneRackBay.FutureTreeSpacingM && Cal.V1.CandidateSpacingM != ForestEcologyController.HegyiCutoffMeters,
            "candidate spacing is its own parameter (not the 8 m teaching spacing or Hegyi reach)");
    }

    // The core must stay pure, stateless and blind to hidden quality. Comments are stripped first so prose may name what is excluded.
    private static void StaticGuards(string root)
    {
        string dir = Path.Combine(root, "Assets", "ForestPrototype", "ScenarioOne");
        string[] files = { "ScenarioOneCoverage.cs", "ScenarioOnePrescription.cs", "ScenarioOneCropTreeCandidates.cs", "ScenarioOnePrescriptionCalibration.cs" };
        string[] banned = { "UnityEngine", "TreeMarkType", "ScenarioOneWorkOrder", "ScenarioOneManager", "ForestTree", "SaveData", "PlayerPrefs",
            "Random", "DateTime", "GetHashCode", "static readonly List", "static readonly Dictionary", "static readonly HashSet", "File." };
        foreach (string file in files)
        {
            string path = Path.Combine(dir, file);
            Check(File.Exists(path) && File.Exists(path + ".meta"), "static: " + file + " and its .meta exist");
            string code = StripComments(File.ReadAllText(path));
            foreach (string token in banned)
                Check(!code.Contains(token), "static: " + file + " does not use " + token);
        }
        string candidates = StripComments(File.ReadAllText(Path.Combine(dir, "ScenarioOneCropTreeCandidates.cs")));
        foreach (string token in new[] { "DbhCm", "Competition", "Height", "BasalArea", "Share" })
            Check(!candidates.Contains(token), "static: candidate ordering uses no " + token + " (spacing-only, no quality ranking)");
    }

    private static string StripComments(string text)
        => string.Join("\n", text.Split('\n').Select(line => { int i = line.IndexOf("//", StringComparison.Ordinal); return i >= 0 ? line.Substring(0, i) : line; }));

    // ----------------------------------------------------------------------------------------------------------------- fixture

    private static Fixture Load(string root, string csv, string infraPath)
    {
        var raw = new List<(string Id, float X, float Z, float Dbh)>();
        foreach (string line in File.ReadAllLines(csv).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] c = line.Split(',');
            raw.Add((c[0], float.Parse(c[1], Inv), float.Parse(c[2], Inv), float.Parse(c[3], Inv)));
        }
        Check(raw.Count == 1344, "fixture: 1344 starting stems", raw.Count.ToString());
        var racks = new List<ScenarioOneRackSampleGeometry.RackInput>();
        for (int i = 0; i < RackZ.Length; i++)
            racks.Add(new ScenarioOneRackSampleGeometry.RackInput(i + 1, new RackPoint(EntranceX, RackZ[i]), new RackPoint(FarX, RackZ[i]), RackGeometry.DefaultWidthM));
        var geometry = ScenarioOneRackSampleGeometry.Derive(racks, Half);
        Check(geometry.Valid && geometry.TeachingRack.Id == 3, "fixture: MFM-02 geometry valid, teaching rack 3", geometry.Problem);
        if (!geometry.Valid) return null;

        // Footprint identity against the live MFM-02 Unity record (pre-access world).
        var fp = raw.Where(s => geometry.Teaching.Contains(s.X, s.Z)).OrderBy(s => s.Id, StringComparer.Ordinal).ToList();
        double fpBa = fp.Sum(s => Math.PI * Math.Pow(s.Dbh / 200.0, 2.0));
        Check(fp.Count == 75 && Math.Abs(fpBa - 1.441753070295678) <= 1e-6 && Sha(string.Join(",", fp.Select(s => s.Id))) == "c2f2fd5418794915",
            "fixture identity: live MFM-02 footprint 75 stems / 1.4417531 m2 / SHA c2f2fd5418794915", fp.Count + " " + F(fpBa));

        // Rack access in rack-id order, as the manager does.
        var claimed = new HashSet<string>(StringComparer.Ordinal);
        foreach (var rack in racks)
        {
            var input = raw.Select(s => new RackTree(s.Id, s.X, s.Z, true, false, claimed.Contains(s.Id))).ToList();
            foreach (RackTree t in RackGeometry.Select(input, rack.Entrance, rack.Far, rack.WidthM).Trees) claimed.Add(t.Id);
        }
        Check(claimed.Count == 149, "fixture: 149 rack-access removals (MFM-03)", claimed.Count.ToString());
        var stems = raw.Select(s => new Cov.Stem(s.Id, s.X, s.Z, s.Dbh, !claimed.Contains(s.Id), Cov.SitkaSpeciesId, SampleCrops.Contains(s.Id))).ToList();
        var orders = claimed.OrderBy(id => id, StringComparer.Ordinal)
            .Select(id => new ScenarioOneRackBay.Order(id, false, true, ScenarioOneRackBay.Disposition.Sold, 0)).ToList();
        return new Fixture { Geometry = geometry, Stems = stems, Orders = orders, Masks = Masks(root, infraPath), RackRemovals = claimed };
    }

    // The MFM-03 infrastructure masks (analyse_coverage.py Masks), built from production headland constants and the frozen scene anchors.
    private static List<Cov.Mask> Masks(string root, string infraPath)
    {
        string text = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ScenarioOne", "ScenarioOneHeadlandGeometry.cs"));
        double C(string name)
        {
            var m = System.Text.RegularExpressions.Regex.Match(text, "const float " + name + " = (-?[0-9.]+)f;");
            if (!m.Success) throw new InvalidOperationException("headland constant " + name);
            return double.Parse(m.Groups[1].Value, Inv);
        }
        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(infraPath));
        JsonElement infra = doc.RootElement;
        double rPath = infra.GetProperty("pathCorridorRadiusM").GetDouble(), rSite = infra.GetProperty("clearingRadiusM").GetDouble();
        var masks = new List<Cov.Mask>();
        foreach (JsonElement p in infra.GetProperty("scenePlanks").EnumerateArray())
            masks.Add(Cov.Mask.Circle(Cov.MaskKind.Track, p.GetProperty("x").GetDouble(), p.GetProperty("z").GetDouble(), rPath));
        foreach (JsonElement p in infra.GetProperty("approachExtension").EnumerateArray())
        {
            double z = p.GetProperty("z").GetDouble();
            if (z >= -Half - rPath && z <= Half + rPath) masks.Add(Cov.Mask.Circle(Cov.MaskKind.Track, p.GetProperty("x").GetDouble(), z, rPath));
        }
        JsonElement cl = infra.GetProperty("clearing");
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Clearing, cl.GetProperty("x").GetDouble(), cl.GetProperty("z").GetDouble(), cl.GetProperty("visualRadiusM").GetDouble()));
        JsonElement pl = infra.GetProperty("player");
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Site, pl.GetProperty("x").GetDouble(), pl.GetProperty("z").GetDouble(), rSite));
        foreach (JsonElement b in infra.GetProperty("buildables").EnumerateArray())
            masks.Add(Cov.Mask.Circle(Cov.MaskKind.Site, b.GetProperty("x").GetDouble(), b.GetProperty("z").GetDouble(), rSite));
        double strip = C("Width") * 0.5 + C("StemGuard"), pad = C("PadRadius") + C("StemGuard");
        double hx = C("HeadlandX"), fz = C("FeederZ"), px = C("PathX");
        masks.Add(new Cov.Mask(Cov.MaskKind.Headland, hx, C("SouthZ"), hx, C("NorthZ"), strip));
        masks.Add(new Cov.Mask(Cov.MaskKind.Headland, hx, fz, px, fz, strip));
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, hx, fz, pad));
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, px, fz, pad));
        return masks;
    }

    private static Cov.Input Input(Fixture fx, IReadOnlyList<Cov.Stem> stems = null, IReadOnlyList<ScenarioOneRackBay.Order> orders = null,
        IReadOnlyList<Cov.Feature> features = null, IReadOnlyCollection<string> manual = null, ScenarioOneRackSampleGeometry.Result geometry = null)
        => new Cov.Input { Geometry = geometry ?? fx.Geometry, Stems = stems ?? fx.Stems, Orders = orders ?? fx.Orders, Masks = fx.Masks, Features = features, PlayerManualAreas = manual };

    // ---------------------------------------------------------------------------------------------- coverage = MFM-03 reproduction

    private static Cov.Result Coverage(Fixture fx)
    {
        Cov.Result r = Cov.Classify(Input(fx), Cal.V1);
        Check(r.Valid, "coverage: valid on the fixture", r.Problem);
        if (!r.Valid) return r;
        Check(r.Source.Stems == 75 && Math.Abs(r.Source.NetAreaM2 - 309.80953943924106) <= 0.5, "coverage: source 75 stems, net area 309.81 m2 (MFM-03)",
            r.Source.Stems + " " + F(r.Source.NetAreaM2));
        Check(Math.Abs(r.Source.MeanDbhCm - 15.404493166605631) <= 1e-5 && Math.Abs(r.Source.MeanCompetition - 8.048336448570092) <= 1e-5,
            "coverage: source mean DBH 15.4045 cm and mean CI 8.04834 equal MFM-03", F(r.Source.MeanDbhCm) + " " + F(r.Source.MeanCompetition));
        Check(r.Areas.Count == 15 && r.Remainders.Count == 2, "coverage: 15 anchor-aligned thirds and two explicit remainders");
        double worstArea = 0, worstDens = 0, worstMean = 0;
        foreach (var e in Mfm03)
        {
            Cov.Area a = r.Find(e.Id);
            if (a == null || a.Comparison == null) { Check(false, "coverage: " + e.Id + " compared"); continue; }
            worstArea = Math.Max(worstArea, Math.Abs(a.Metrics.NetAreaM2 - e.Net));
            worstDens = Math.Max(worstDens, Math.Abs(100 * a.Comparison.DeltaDensity - e.DDens));
            worstMean = Math.Max(worstMean, Math.Max(Math.Abs(100 * a.Comparison.DeltaMeanDbh - e.DDbh), Math.Abs(100 * a.Comparison.DeltaMeanCompetition - e.DCi)));
            Console.WriteLine("DBG " + e.Id + " net=" + F(a.Metrics.NetAreaM2) + " exp=" + F(e.Net) + " dd=" + F(100 * a.Comparison.DeltaDensity - e.DDens));
            Check(a.Metrics.Stems == e.Stems, "coverage: " + e.Id + " holds MFM-03's " + e.Stems + " target stems", a.Metrics.Stems.ToString());
        }
        Check(worstArea <= 0.5, "coverage: every net area within 0.5 m2 of MFM-03's exact-interval integration", F(worstArea));
        Check(worstDens <= 0.2, "coverage: every density difference within 0.2 pp of MFM-03", F(worstDens));
        Check(worstMean <= 0.002, "coverage: every mean DBH / mean CI difference equals MFM-03 (same stems, same CompetitionShare)", F(worstMean));
        foreach (var e in Mfm03Edge)
            Check(r.Find(e.Id).Metrics.StemsNearStandEdge == e.Edge, "coverage: " + e.Id + " has " + e.Edge + " stems within 8 m of the stand edge (MFM-03)");

        Cov.Area source = r.Find("R3C");
        Check(source.IsSource && source.State == Cov.State.Manual && source.Reasons.SequenceEqual(new[] { Cov.Reason.SourceSample }) && source.Metrics.Stems == 0,
            "coverage: R3C is the sample: Manual / SourceSample, never a target");
        Cov.Area r1l = r.Find("R1L");
        Check(r1l.State == Cov.State.NotCovered && r1l.Reasons.SequenceEqual(new[] { Cov.Reason.MeanCompetitionDiffers }) && r1l.Fit == Cov.Fit.ClearFail
              && r1l.Comparison.LimitingMetric == Cov.Metric.MeanCompetition, "coverage: R1L (MFM-03 R1W) not covered, clear fail on mean competition only");
        Check(Math.Abs(r1l.Comparison.Margin - 1.6586623514579157) <= 1e-4, "coverage: R1L margin 1.659 (MFM-03)", F(r1l.Comparison.Margin));
        foreach (string id in new[] { "R1C", "R2C", "R4C" })
            Check(r.Find(id).State == Cov.State.Covered && r.Find(id).Fit == Cov.Fit.BoundaryFit, "coverage: " + id + " is a covered boundary fit (MFM-03 section 8)");
        foreach (string id in new[] { "R3L", "R2H", "R2L" })
            Check(r.Find(id).State == Cov.State.Covered && r.Find(id).Fit == Cov.Fit.ClearFit, "coverage: " + id + " is a clear fit (MFM-03 section 8)");
        Check(r.CoveredCount == 13 && r.NonSourceCount == 14 && Math.Abs(r.CoveredFraction - 13.0 / 14.0) < 1e-12,
            "coverage: 13 of 14 non-source areas covered (MFM-03)", r.CoveredCount + "/" + r.NonSourceCount);
        Check(!r.SecondSampleRequired, "coverage: default calibration requires no second sample (open user question kept as a parameter)");
        Check(r.Remainders.All(m => m.IsEmpty && m.AreaM2 == 0 && m.Stems == 0 && m.State == Cov.State.Manual && m.Reason == Cov.Reason.RemainderLeftForManualManagement),
            "coverage: both fixture remainders explicit, empty and left for manual management");
        Check(r.SampleRemovalIds.Count == 0, "coverage: untreated sample has nothing put back");
        int allStems = r.Areas.Sum(a => a.Metrics.Stems) + r.Source.Stems;
        int living = fx.Stems.Count(s => s.Living);
        Check(allStems <= living && r.Areas.SelectMany(a => a.StemIds).Concat(r.SourceStemIds).Distinct().Count() == allStems,
            "coverage: every stem in at most one area or the source", allStems + "/" + living);
        return r;
    }

    // ----------------------------------------------------------------------------------------------------- tolerance boundaries

    private static Cov.Metrics M(double density, double dbh, double ci) => new Cov.Metrics { Stems = 40, NetAreaM2 = 1, DensityPerHa = density, MeanDbhCm = dbh, MeanCompetition = ci };

    private static void Boundaries()
    {
        Cov.Metrics src = M(1000, 20, 8);
        Cov.Comparison Cmp(Cov.Metrics t) => Cov.Compare(src, t, Cal.V1);
        var c = Cmp(M(1200, 20, 8));
        Check(c.Fits && Math.Abs(c.Margin - 1.0) < 1e-12 && c.Fit == Cov.Fit.BoundaryFit && c.LimitingMetric == Cov.Metric.Density, "boundary: density exactly +20% fits (inclusive), margin 1, boundary fit");
        c = Cmp(M(1200.001, 20, 8));
        Check(!c.Fits && c.Failures.SequenceEqual(new[] { Cov.Reason.DensityDiffers }) && c.Fit == Cov.Fit.BoundaryFail, "boundary: density +20.0001% fails (DensityDiffers, boundary fail)");
        Check(Cmp(M(800, 20, 8)).Fits && !Cmp(M(799.999, 20, 8)).Fits, "boundary: density -20% fits, -20.0001% fails");
        Check(Cmp(M(1000, 23, 8)).Fits && Cmp(M(1000, 17, 8)).Fits, "boundary: mean DBH exactly +/-15% fits");
        c = Cmp(M(1000, 23.001, 8));
        Check(!c.Fits && c.Failures.SequenceEqual(new[] { Cov.Reason.MeanDbhDiffers }) && c.LimitingMetric == Cov.Metric.MeanDbh, "boundary: mean DBH +15.005% fails (MeanDbhDiffers)");
        Check(Cmp(M(1000, 20, 9.6)).Fits && Cmp(M(1000, 20, 6.4)).Fits, "boundary: mean CI exactly +/-20% fits");
        c = Cmp(M(1000, 20, 6.39));
        Check(!c.Fits && c.Failures.SequenceEqual(new[] { Cov.Reason.MeanCompetitionDiffers }) && c.LimitingMetric == Cov.Metric.MeanCompetition, "boundary: mean CI -20.125% fails (MeanCompetitionDiffers)");
        c = Cmp(M(1300, 26, 11));
        Check(c.Failures.SequenceEqual(new[] { Cov.Reason.DensityDiffers, Cov.Reason.MeanDbhDiffers, Cov.Reason.MeanCompetitionDiffers }) && c.Fit == Cov.Fit.ClearFail,
            "boundary: all three metrics out -> all three reasons, clear fail");
        Check(Cmp(M(1129, 20, 8)).Fit == Cov.Fit.ClearFit && Cmp(M(1130, 20, 8)).Fit == Cov.Fit.BoundaryFit, "boundary: margin 0.645 clear fit, 0.65 boundary fit (presentation band)");
        Check(Cmp(M(1300, 20, 8)).Fit == Cov.Fit.BoundaryFail && Cmp(M(1301, 20, 8)).Fit == Cov.Fit.ClearFail, "boundary: margin 1.5 boundary fail, 1.505 clear fail");
        Check(Cmp(M(1000, 20, 8)).Margin == 0 && Cmp(M(1000, 20, 8)).Fit == Cov.Fit.ClearFit, "boundary: identical metrics margin 0, clear fit");
    }

    // ------------------------------------------------------------------------------------- calibration variants (MFM-03 sweeps)

    private static Cal Variant(double density, double dbh, double ci, int minStems, Cal.SecondSamplePolicy policy = Cal.SecondSamplePolicy.NotRequired, double limit = 1.0)
        => new Cal("test", 12.0, density, dbh, ci, minStems, 0.65, 1.5, 16.0, 0.1, policy, limit);

    private static void CalibrationVariants(Fixture fx)
    {
        int Covered(Cal cal) => Cov.Classify(Input(fx), cal).CoveredCount;
        Check(Covered(Variant(0.20, 0.15, 0.15, 30)) == 11, "sweep: CI 15% -> 11 covered (R1C, R2C drop; MFM-03)");
        Check(Covered(Variant(0.20, 0.15, 0.10, 30)) == 8, "sweep: CI 10% -> 8 covered (MFM-03)");
        Check(Covered(Variant(0.10, 0.15, 0.20, 30)) == 11, "sweep: density 10% -> 11 covered (R2C, R4C drop; MFM-03)");
        Check(Covered(Variant(0.20, 0.05, 0.20, 30)) == 13, "sweep: DBH 5% -> 13 covered (DBH not discriminating at year 0; MFM-03)");
        Cov.Result n60 = Cov.Classify(Input(fx), Variant(0.20, 0.15, 0.20, 60));
        Check(n60.CoveredCount == 12 && n60.Find("R1C").State == Cov.State.NotCovered && n60.Find("R1C").Reasons.SequenceEqual(new[] { Cov.Reason.TooFewStems })
              && n60.Find("R1C").Comparison == null, "minimum stems: n>=60 -> R1C (53 stems) not compared, TooFewStems; 12 covered (MFM-03)");
        Check(Cov.Classify(Input(fx), Variant(0.20, 0.15, 0.20, 53)).Find("R1C").State == Cov.State.Covered
              && Cov.Classify(Input(fx), Variant(0.20, 0.15, 0.20, 54)).Find("R1C").State == Cov.State.NotCovered, "minimum stems: exactly 53 compared, 54 not (inclusive)");
        Cov.Result tooSmall = Cov.Classify(Input(fx), Variant(0.20, 0.15, 0.20, 76));
        Check(!tooSmall.Valid && tooSmall.Problem.Contains("fewer than"), "minimum stems: a 75-stem sample under a 76 minimum is refused, not compared", tooSmall.Problem);

        Cal ask = Cal.V1.WithSecondSample(Cal.SecondSamplePolicy.RequiredAboveCoveredFraction, 0.9);
        Check(Cov.Classify(Input(fx), ask).SecondSampleRequired, "second sample: policy 'required above 90%' flags 13/14 (92.9%)");
        Check(!Cov.Classify(Input(fx), Cal.V1.WithSecondSample(Cal.SecondSamplePolicy.RequiredAboveCoveredFraction, 0.95)).SecondSampleRequired,
            "second sample: policy 'required above 95%' does not flag 13/14");
        Check(ask.Signature() != Cal.V1.Signature() && ask.Version != Cal.V1.Version, "second sample: a different policy is a different calibration version/signature");
        Check(Cal.V1.Signature().Contains("rule=" + Cal.RuleVersion) && Cal.V1.Signature().Contains("tailGuard=none") && Cal.V1.Version == "MFM03-CAL-1",
            "calibration: signature records rule version, calibration version and the absent tail guard");

        // Missing / malformed inputs.
        Check(!Cov.Classify(Input(fx, geometry: new ScenarioOneRackSampleGeometry.Result()), Cal.V1).Valid, "missing: invalid geometry -> invalid result, no fallback");
        Check(!Cov.Classify(Input(fx, stems: new List<Cov.Stem>()), Cal.V1).Valid, "missing: no stems -> invalid result");
        var dup = new List<Cov.Stem>(fx.Stems) { fx.Stems[0] };
        Check(!Cov.Classify(Input(fx, stems: dup), Cal.V1).Valid, "missing: duplicate tree id -> invalid result");
        Check(!Cov.Classify(Input(fx), Variant(0, 0.15, 0.20, 30)).Valid, "missing: zero tolerance calibration refused");
        // An area emptied of trees is not compared and divides by nothing.
        Cov.Result baseline = Cov.Classify(Input(fx), Cal.V1);
        var r5h = new HashSet<string>(baseline.Find("R5H").StemIds, StringComparer.Ordinal);
        var emptied = fx.Stems.Where(s => !r5h.Contains(s.Id)).ToList();
        Cov.Area empty = Cov.Classify(Input(fx, stems: emptied), Cal.V1).Find("R5H");
        Check(empty.Metrics.Stems == 0 && empty.State == Cov.State.NotCovered && empty.Reasons.SequenceEqual(new[] { Cov.Reason.TooFewStems }) && empty.Comparison == null
              && !double.IsNaN(empty.Metrics.MeanCompetition) && !double.IsNaN(empty.Metrics.DensityPerHa), "missing: an area with no stems is NotCovered/TooFewStems with finite metrics");
    }

    // ------------------------------------------------------------------------------------------------------- exclusions / flags

    private static Cov.Stem With(Cov.Stem s, bool? living = null, string species = null, bool? crop = null, bool? fellMark = null)
        => new Cov.Stem(s.Id, s.X, s.Z, s.DbhCm, living ?? s.Living, species ?? s.SpeciesId, crop ?? s.Crop, fellMark ?? s.FellMark);

    private static string FirstStem(Cov.Result r, string area, int skip = 0) => r.Find(area).StemIds[skip];

    private static void Exclusions(Fixture fx, Cov.Result baseline)
    {
        Cov.Area Centre(string id) => baseline.Find(id);
        (float X, float Z) At(string area)
        {
            var s = fx.Stems.First(t => t.Id == Centre(area).StemIds[0]);
            return (s.X, s.Z);
        }
        var features = new List<Cov.Feature>();
        void Feature(Cov.FeatureKind kind, string area, string id) { var p = At(area); features.Add(new Cov.Feature(kind, id, p.X + 0.01f, p.Z)); }
        Feature(Cov.FeatureKind.PlantedJuvenile, "R2H", "J1");
        Feature(Cov.FeatureKind.Shelter, "R2C", "S1");
        Feature(Cov.FeatureKind.ClearancePatch, "R3L", "C1");
        Feature(Cov.FeatureKind.ProtectedArea, "R3H", "PA1");
        Feature(Cov.FeatureKind.DeadwoodRecord, "R4C", "D1");
        Feature(Cov.FeatureKind.ActiveManagementWork, "R1C", "W1");
        Feature(Cov.FeatureKind.NaturalRegeneration, "R2L", "N1");
        // A feature inside the sample changes nothing about the sample or any area.
        features.Add(new Cov.Feature(Cov.FeatureKind.PlantedJuvenile, "J-sample", 0.5f, -1.5f));

        string fell = FirstStem(baseline, "R4H"), open = FirstStem(baseline, "R5C"), done = FirstStem(baseline, "R5L"), oak = FirstStem(baseline, "R1H"),
            crop = FirstStem(baseline, "R4L");
        var stems = fx.Stems.Select(s => s.Id == fell ? With(s, fellMark: true) : s.Id == done ? With(s, living: false) : s.Id == oak ? With(s, species: "oak")
            : s.Id == crop ? With(s, crop: true) : s).ToList();
        var orders = new List<ScenarioOneRackBay.Order>(fx.Orders)
        {
            new ScenarioOneRackBay.Order(open, true, false, ScenarioOneRackBay.Disposition.Sold, -1),
            new ScenarioOneRackBay.Order(done, false, true, ScenarioOneRackBay.Disposition.Deadwood, 1)
        };
        Cov.Result r = Cov.Classify(Input(fx, stems, orders, features), Cal.V1);
        Check(r.Valid, "exclusions: valid", r.Problem);
        void Excluded(string area, Cov.Reason reason, string evidence)
        {
            Cov.Area a = r.Find(area);
            Check(a.State == Cov.State.Excluded && a.Reasons.SequenceEqual(new[] { reason }) && a.Evidence.Contains(evidence) && a.Comparison == null,
                "exclusions: " + area + " excluded by " + reason + " with evidence " + evidence, a.State + " " + string.Join(",", a.Reasons));
        }
        Excluded("R2H", Cov.Reason.PlantedJuvenile, "J1");
        Excluded("R2C", Cov.Reason.Shelter, "S1");
        Excluded("R3L", Cov.Reason.ClearancePatch, "C1");
        Excluded("R3H", Cov.Reason.ProtectedArea, "PA1");
        Excluded("R4C", Cov.Reason.DeadwoodRecord, "D1");
        Excluded("R1C", Cov.Reason.ActiveManagementWork, "W1");
        Excluded("R4H", Cov.Reason.PlayerFellMark, fell);
        Excluded("R5C", Cov.Reason.PlayerOpenWork, open);
        Excluded("R5L", Cov.Reason.PlayerCompletedWork, done);
        Excluded("R1H", Cov.Reason.NonSitkaAdult, oak);
        Cov.Area regen = r.Find("R2L");
        Check(regen.State == Cov.State.Covered && regen.Flags.SequenceEqual(new[] { Cov.Flag.NaturalRegeneration }) && regen.Evidence.Contains("N1"),
            "flags: natural regeneration is recorded and flagged, not excluded (R2L stays covered)");
        Cov.Area flagged = r.Find("R4L");
        Check(flagged.State == Cov.State.Covered && flagged.Flags.SequenceEqual(new[] { Cov.Flag.PlayerCropTrees }) && flagged.Evidence.Contains(crop),
            "flags: a player Crop Tree outside the sample is flagged and does not exclude its area (manual designation stays available)");
        Check(r.Find("R3C").State == Cov.State.Manual && r.Source.Stems == 75 && Math.Abs(r.Source.MeanCompetition - baseline.Source.MeanCompetition) < 1e-12,
            "exclusions: a feature inside the sample leaves the sample unchanged");
        Check(r.CoveredCount == 13 - 10 && r.Find("R1L").State == Cov.State.NotCovered, "exclusions: 10 exclusions leave 3 covered; R1L still not covered", r.CoveredCount.ToString());

        // Player manual choices always win, including over an exclusion; remainders record the choice too.
        Cov.Result m = Cov.Classify(Input(fx, stems, orders, features, new[] { "R2L", "R2H", "REM-Right" }), Cal.V1);
        Check(m.Find("R2L").State == Cov.State.Manual && m.Find("R2L").Reasons.SequenceEqual(new[] { Cov.Reason.PlayerChoseManual }), "manual: player choice makes R2L Manual/PlayerChoseManual");
        Check(m.Find("R2H").State == Cov.State.Manual && m.Find("R2H").Reasons.SequenceEqual(new[] { Cov.Reason.PlayerChoseManual }), "manual: player choice wins over an exclusion");
        Check(m.Remainders[0].PlayerChoseManual && !m.Remainders[1].PlayerChoseManual && m.Remainders.All(x => x.State == Cov.State.Manual), "manual: remainder records the player's choice");
        Check(Cov.Classify(Input(fx, manual: new[] { "R3C" }), Cal.V1).Find("R3C").Reasons.SequenceEqual(new[] { Cov.Reason.SourceSample }), "manual: the sample stays the source whatever the player chose");

        // A non-Sitka adult in the sample stops the comparison: V1 is Sitka-only.
        var sampleOak = fx.Stems.Select(s => s.Id == "P0605" ? With(s, species: "oak") : s).ToList();
        Cov.Result so = Cov.Classify(Input(fx, sampleOak), Cal.V1);
        Check(!so.Valid && so.Problem.Contains("non-Sitka"), "species: a non-Sitka adult in the sample invalidates coverage", so.Problem);

        // Non-empty outer remainder (MFM-03 probe: racks 4 m north) is explicit, measured and left for manual management.
        float[] north = { -28.1650f, -12.0750f, 4f, 20.05f, 36.26f };
        var probe = ScenarioOneRackSampleGeometry.Derive(north.Select((z, i) => new ScenarioOneRackSampleGeometry.RackInput(i + 1,
            new RackPoint(EntranceX, z), new RackPoint(FarX, z), RackGeometry.DefaultWidthM)).ToList(), Half);
        var probeStems = fx.Stems.Select(s => With(s, living: true)).ToList();
        Cov.Result pr = Cov.Classify(Input(fx, probeStems, new List<ScenarioOneRackBay.Order>(), geometry: probe), Cal.V1);
        Cov.Remainder right = pr.Remainders[0];
        Check(pr.Valid && !right.IsEmpty && right.State == Cov.State.Manual && right.Reason == Cov.Reason.RemainderLeftForManualManagement
              && Math.Abs(right.AreaM2 - probe.RightRemainder.AreaM2) <= 1.0 && right.Stems > 0,
            "remainder: a non-empty remainder is measured (grid area within 1 m2 of the polygon area) and left for manual management, never compared",
            F(right.AreaM2) + " vs " + F(probe.RightRemainder.AreaM2) + ", " + right.Stems + " stems");
        Check(pr.Areas.Sum(a => a.Metrics.NetAreaM2) + pr.Source.NetAreaM2 + right.AreaM2 + pr.Remainders[1].AreaM2 < 6400, "remainder: areas never double-count ground");
    }

    // ---------------------------------------------------------------------------------------------------------- candidates

    private static Cand.Result Candidates(Fixture fx, Cov.Result coverage)
    {
        Cand.Result r = Cand.Suggest(coverage, fx.Stems, SampleCrops, null, null, Cal.V1);
        Check(r.Valid, "candidates: valid", r.Problem);
        var ids = r.Candidates.Select(c => c.TreeId).ToArray();
        Check(ids.SequenceEqual(Mfm03Candidates16), "candidates: 16 m unit-centre-first gives MFM-03's 17 candidates in order", string.Join(",", ids));
        Check(r.Candidates.Select(c => c.Order).SequenceEqual(Enumerable.Range(1, r.Candidates.Count)), "candidates: queue order is 1..n");
        Check(ids.Distinct().Count() == ids.Length, "candidates: unique");
        var living = fx.Stems.Where(s => s.Living).ToDictionary(s => s.Id);
        var crops = SampleCrops.Select(id => living[id]).ToList();
        double minPair = double.MaxValue, minCrop = double.MaxValue;
        for (int i = 0; i < r.Candidates.Count; i++)
        {
            var a = r.Candidates[i];
            for (int j = i + 1; j < r.Candidates.Count; j++) minPair = Math.Min(minPair, Dist(a.X, a.Z, r.Candidates[j].X, r.Candidates[j].Z));
            foreach (var c in crops) minCrop = Math.Min(minCrop, Dist(a.X, a.Z, c.X, c.Z));
        }
        Check(minPair >= 16 && minCrop >= 16, "candidates: every candidate >= 16 m from every other candidate and every sample Crop Tree", F(minPair) + " " + F(minCrop));
        Check(r.Candidates.All(c => coverage.Find(c.AreaId).State == Cov.State.Covered && coverage.Find(c.AreaId).StemIds.Contains(c.TreeId)),
            "candidates: only target stems of covered areas (none in R1L or the sample)");
        Check(r.Candidates.All(c => c.Decision == Cand.Decision.Open && c.Reason == Cand.CandidateReason.SpacingOnlyInspectToJudge),
            "candidates: all open, reason spacing-only (inspect to judge); none becomes a Crop Tree");
        Check(r.AreasWithoutCandidate.Count == 0 && r.StaleDecisions.Count == 0, "candidates: every covered area gets one; no stale decisions");

        foreach (var (spacing, count, none) in new[] { (8.0, 70, ""), (10.0, 41, ""), (12.0, 33, ""), (14.0, 25, ""), (18.0, 13, "R2C"), (20.0, 11, "R2C,R5C") })
        {
            Cand.Result s = Cand.Suggest(coverage, fx.Stems, SampleCrops, null, null, Cal.V1.WithCandidateSpacing(spacing));
            Check(s.Candidates.Count == count && string.Join(",", s.AreasWithoutCandidate) == none,
                "candidates: " + spacing + " m gives " + count + " candidates, areas without: [" + none + "] (MFM-03)", s.Candidates.Count + " [" + string.Join(",", s.AreasWithoutCandidate) + "]");
        }

        // Batch semantics: decisions label the set; nothing is replaced. A confirmed candidate becomes a Crop Tree and keeps its place.
        var confirmedStems = fx.Stems.Select(s => s.Id == "PO0321" ? With(s, crop: true) : s).ToList();
        Cand.Result decided = Cand.Suggest(coverage, confirmedStems, SampleCrops, new[] { "PO0321" }, new[] { "PO1107", "PX-gone" }, Cal.V1);
        Check(decided.Candidates.Select(c => c.TreeId).SequenceEqual(Mfm03Candidates16), "batch: confirming PO0321 and rejecting PO1107 leaves the same set in the same order");
        Check(decided.Candidates.First(c => c.TreeId == "PO0321").Decision == Cand.Decision.Confirmed && decided.Candidates.First(c => c.TreeId == "PO1107").Decision == Cand.Decision.Rejected
              && decided.Candidates.Count(c => c.Decision == Cand.Decision.Open) == 15, "batch: decisions recorded per candidate, 15 still open");
        Check(decided.StaleDecisions.SequenceEqual(new[] { "PX-gone" }), "batch: a decision on a tree no longer suggested is reported stale, not dropped");
        Check(!Cand.Suggest(coverage, fx.Stems, SampleCrops, new[] { "PO0321" }, new[] { "PO0321" }, Cal.V1).Valid, "batch: confirmed-and-rejected is refused");

        // Manual override: a player Crop Tree near a candidate blocks it; the player's choice wins and is never itself a candidate.
        var po1107 = living["PO1107"];
        var near = fx.Stems.Where(s => s.Living && coverage.Find("R2L").StemIds.Contains(s.Id) && s.Id != "PO1107")
            .OrderBy(s => Dist(s.X, s.Z, po1107.X, po1107.Z)).ThenBy(s => s.Id, StringComparer.Ordinal).First();
        var manualStems = fx.Stems.Select(s => s.Id == near.Id ? With(s, crop: true) : s).ToList();
        Cand.Result manual = Cand.Suggest(coverage, manualStems, SampleCrops, null, null, Cal.V1);
        Check(!manual.Candidates.Any(c => c.TreeId == "PO1107" || c.TreeId == near.Id)
              && manual.Candidates.All(c => Dist(c.X, c.Z, near.X, near.Z) >= 16), "manual override: a player Crop Tree blocks nearby suggestions and is not suggested", near.Id);

        // No hidden quality: shuffling every diameter leaves the candidates unchanged (positions, species and player state only).
        var dbhs = fx.Stems.Select(s => s.DbhCm).ToList();
        var shuffledDbh = fx.Stems.Select((s, i) => new Cov.Stem(s.Id, s.X, s.Z, dbhs[(i * 7 + 3) % dbhs.Count], s.Living, s.SpeciesId, s.Crop, s.FellMark)).ToList();
        Check(Cand.Suggest(coverage, shuffledDbh, SampleCrops, null, null, Cal.V1).Signature() == r.Signature(),
            "no quality ranking: permuting every DBH leaves the candidate queue identical");
        return r;
    }

    private static double Dist(double ax, double az, double bx, double bz) => Math.Sqrt((ax - bx) * (ax - bx) + (az - bz) * (az - bz));

    // ------------------------------------------------------------------------------------------------- release + safeguard

    private static Pres.ReleasePlan Release(Fixture fx, Cov.Result coverage, Cand.Result candidates)
    {
        var confirmed = candidates.Candidates.Select(c => c.TreeId).ToList();
        var confirmedStems = fx.Stems.Select(s => confirmed.Contains(s.Id) ? With(s, crop: true) : s).ToList();
        var rule = Pres.ReleaseRule.Create(0.30, LiveInitialReleaseBaShare);
        Pres.ReleasePlan plan = Pres.ProposeRelease(rule, coverage, confirmedStems, confirmed, SampleCrops);
        Check(plan.Valid, "release: valid", plan.Problem);
        double heaviest = plan.Safeguard.Areas.Max(a => a.Share);
        Check(plan.CropTrees.Count == 17 && plan.CropTrees.All(c => c.Status == Pres.CropStatus.Released && c.PlannedShare >= 0.30 - 1e-6),
            "release: all 17 confirmed candidates reach the 30% target at the 17.9% sample ceiling (MFM-03 indicator 17/17)");
        Check(plan.Proposals.Count == 84 && Math.Abs(heaviest - 0.1494483325642523) <= 1e-6,
            "release: 84 proposed removals, heaviest area 14.94% of basal area (MFM-03 indicator)", plan.Proposals.Count + " " + F(heaviest));
        Check(plan.Safeguard.AllWithin && plan.Safeguard.Areas.All(a => a.Share <= LiveInitialReleaseBaShare + 1e-9), "safeguard: every covered area within the ceiling");
        var protectedIds = new HashSet<string>(confirmed.Concat(SampleCrops));
        Check(plan.Proposals.Select(p => p.TreeId).Distinct().Count() == plan.Proposals.Count
              && plan.Proposals.Select(p => p.TreeId).SequenceEqual(plan.Proposals.Select(p => p.TreeId).OrderBy(x => x, StringComparer.Ordinal)),
            "release: proposals unique and sorted by tree id");
        Check(plan.Proposals.All(p => !protectedIds.Contains(p.TreeId) && confirmed.Contains(p.ForCropTreeId)
              && coverage.Find(p.AreaId).State == Cov.State.Covered && coverage.Find(p.AreaId).StemIds.Contains(p.TreeId)
              && !coverage.SourceStemIds.Contains(p.TreeId) && p.Reason == Pres.ProposalReason.CompetitorHighShareAroundConfirmedCropTree),
            "release: never a Crop Tree or candidate, only around confirmed Crop Trees, only covered-area target stems, never in the sample");

        // Five confirmed, twelve still open: only the confirmed are served, and open candidates are kept standing.
        var firstFive = confirmed.Take(5).ToList();
        var openCandidates = confirmed.Skip(5).ToList();
        var partStems = fx.Stems.Select(s => firstFive.Contains(s.Id) ? With(s, crop: true) : s).ToList();
        var partial = Pres.ProposeRelease(rule, coverage, partStems, firstFive, SampleCrops, openCandidates);
        Check(partial.Valid && partial.CropTrees.Count == 5 && partial.Proposals.Count > 0 && partial.Proposals.All(p => firstFive.Contains(p.ForCropTreeId))
              && !partial.Proposals.Any(p => openCandidates.Contains(p.TreeId)),
            "release: only confirmed Crop Trees are served; open candidates are never proposed for removal");

        // Tight ceiling: the safeguard binds and says so; nothing goes over.
        var tight = Pres.ProposeRelease(Pres.ReleaseRule.Create(0.30, 0.02), coverage, confirmedStems, confirmed, SampleCrops);
        Check(tight.Valid && tight.CropTrees.Any(c => c.Status == Pres.CropStatus.LimitedByCeiling) && tight.Safeguard.AllWithin
              && tight.Safeguard.Areas.All(a => a.Share <= 0.02 + 1e-9), "safeguard: a 2% ceiling limits release (LimitedByCeiling) and no area exceeds it");
        // The protected bounds always apply.
        var clamp = Pres.ReleaseRule.Create(0.10, 0.90);
        Check(clamp.TargetShare == ScenarioOneRackBay.FirstReleaseShare && clamp.BasalCeilingShare == ScenarioOneRackBay.MaxRemovedBasalShare,
            "safeguard: rule target never below 30%, ceiling never above 40%");
        var wide = Pres.ProposeRelease(clamp, coverage, confirmedStems, confirmed, SampleCrops);
        Check(wide.Safeguard.Areas.All(a => a.Share <= 0.40 + 1e-9 && a.Ceiling == 0.40f), "safeguard: a requested 90% ceiling is held at 40%");
        // Revalidation of an arbitrary set.
        var overSet = coverage.Find("R2L").StemIds.Concat(new[] { coverage.SourceStemIds[0] }).ToList();
        var report = Pres.EvaluateSafeguard(coverage, fx.Stems, overSet, LiveInitialReleaseBaShare);
        Check(!report.AllWithin && !report.Areas.First(a => a.AreaId == "R2L").Within && report.OutsideCoveredAreas.SequenceEqual(new[] { coverage.SourceStemIds[0] }),
            "safeguard: revalidation flags an over-ceiling area and a removal outside covered areas");

        // Ineligible Crop Trees are reported and never served.
        string notCovered = coverage.Find("R1L").StemIds[0], stump = fx.RackRemovals.OrderBy(x => x, StringComparer.Ordinal).First();
        var odd = Pres.ProposeRelease(rule, coverage, fx.Stems, new[] { notCovered, "P-missing", "P0810", stump }, SampleCrops);
        Pres.CropStatus S(string id) => odd.CropTrees.First(c => c.Id == id).Status;
        Check(S(notCovered) == Pres.CropStatus.NotInCoveredArea && S("P-missing") == Pres.CropStatus.NotFound && S("P0810") == Pres.CropStatus.SampleCropTree
              && S(stump) == Pres.CropStatus.NotLiving && odd.Proposals.Count == 0, "release: not-covered, missing, sample and felled Crop Trees are reported and get no proposals");
        var oakStems = fx.Stems.Select(s => s.Id == confirmed[0] ? With(s, species: "beech") : s).ToList();
        Check(Pres.ProposeRelease(rule, coverage, oakStems, new[] { confirmed[0] }, SampleCrops).CropTrees[0].Status == Pres.CropStatus.NotSitka,
            "species: a broadleaf Crop Tree gets no Sitka-calibrated release");
        // Coverage must describe the current state.
        var gone = fx.Stems.Select(s => s.Id == coverage.Find("R2L").StemIds[0] ? With(s, living: false) : s).ToList();
        var stale = Pres.ProposeRelease(rule, coverage, gone, confirmed, SampleCrops);
        Check(!stale.Valid && stale.Problem.Contains("stale"), "release: stale coverage (a target tree has gone) is refused", stale.Problem);
        return plan;
    }

    // --------------------------------------------------------------------------------------------------- derivation + fit

    private static string DerivationAndFit(Fixture fx, Cov.Result untreated)
    {
        var geometry = fx.Geometry;
        Pres.Derivation none = Pres.Derive(geometry, fx.Stems.Select(s => With(s, crop: false)).ToList(), fx.Orders, Cal.V1);
        Check(none.Status == Pres.Status.SampleNotQualified && none.Rule.TargetShare == 0, "derive: no sample work -> SampleNotQualified, no rule", none.Problem);
        Check(Pres.Derive(new ScenarioOneRackSampleGeometry.Result(), fx.Stems, fx.Orders, Cal.V1).Status == Pres.Status.InvalidGeometry, "derive: invalid geometry refused");

        // The player releases each Crop Tree exactly as the rule would (built by the rule itself at the 40% safeguard), then executes it.
        Pres.FitResult plan = Pres.CheckFit(Pres.ReleaseRule.Create(0.30, 0.40), SampleCrops, new string[0], geometry, fx.Stems, fx.Masks);
        var picks = plan.RuleOnly.Select(t => t.Id).ToList();
        Check(plan.Valid && picks.Count > 0 && plan.Agreements.Count == 0 && plan.PlayerOnly.Count == 0, "fit: rule replay on the untreated sample proposes " + picks.Count + " removals");
        Pres.Derivation agree = Derive(fx, picks, out var agreeStems);
        Check(agree.Ready, "derive: the executed sample passes the lesson and derives", agree.Problem);
        Check(agree.SampleCropTreeIds.SequenceEqual(SampleCrops) && agree.SampleRemovalIds.SequenceEqual(picks.OrderBy(x => x, StringComparer.Ordinal)),
            "derive: accepted Crop Trees and executed removals are the player's");
        Check(agree.Rule.TargetShare >= 0.30 && Math.Abs(agree.Rule.TargetShare - Math.Max(0.30, agree.PlayerMinimumReleaseShare)) < 1e-12
              && Math.Abs(agree.Rule.BasalCeilingShare - Math.Min(0.40, agree.SampleRemovedBasalShare)) < 1e-12 && agree.Rule.BasalCeilingShare <= 0.40,
            "derive: target = max(30%, least executed release); ceiling = min(40%, sample removed basal share)", agree.Rule.Signature());
        Check(agree.SoldRemovals == picks.Count && agree.DeadwoodRemovals == 0, "derive: disposition recorded (all sold), not applied to proposals");
        Pres.FitResult same = Pres.CheckFit(agree, geometry, agreeStems, fx.Masks);
        Check(same.Valid && same.PlayerOnly.Count == 0 && same.RuleOnly.Count == 0 && same.Agreements.Count == picks.Count && same.CropTrees.All(c => c.RuleReachedTarget),
            "fit: a player who treated exactly like the rule gets full agreement", same.Agreements.Count + "/" + picks.Count);
        Check(Math.Abs(same.PlayerBasalShare - same.RuleBasalShare) < 1e-12 && same.CropTrees.All(c => Math.Abs(c.PlayerShare - c.RuleShare) < 1e-12),
            "fit: identical Share and basal-area consequences reported");

        // Coverage of the treated stand: the sample's removals are put back, so the comparison equals the untreated one exactly.
        var agreeOrders = OrdersFor(fx, picks);
        Cov.Result treated = Cov.Classify(Input(fx, agreeStems, agreeOrders), Cal.V1);
        Check(treated.SampleRemovalIds.SequenceEqual(picks.OrderBy(x => x, StringComparer.Ordinal)), "coverage: the treated sample's removals are put back for comparison");
        Check(WithoutPutBack(treated.Signature()) == WithoutPutBack(untreated.Signature()), "coverage: comparison state is post-access, pre-treatment (identical to the untreated stand)");

        // A disagreeing player: drops one rule removal, adds a weak competitor and a tree that serves no Crop Tree.
        var crop0 = fx.Stems.First(s => s.Id == SampleCrops[0]);
        var footprint = fx.Stems.Where(s => s.Living && geometry.Teaching.Contains(s.X, s.Z) && !SampleCrops.Contains(s.Id) && !picks.Contains(s.Id)).ToList();
        var crops = SampleCrops.Select(id => fx.Stems.First(s => s.Id == id)).ToList();
        string farAway = footprint.Where(s => crops.All(c => Dist(s.X, s.Z, c.X, c.Z) > 8.5)).Select(s => s.Id).OrderBy(x => x, StringComparer.Ordinal).First();
        string weak = footprint.Where(s => Dist(s.X, s.Z, crop0.X, crop0.Z) <= 7.5 && Dist(s.X, s.Z, crop0.X, crop0.Z) >= 5
                && crops.Skip(1).All(c => Dist(s.X, s.Z, c.X, c.Z) > 8.5))
            .OrderBy(s => s.DbhCm).ThenBy(s => s.Id, StringComparer.Ordinal).Select(s => s.Id).First();
        Pres.Derivation differ = null; List<Cov.Stem> differStems = null; string dropped = null;
        foreach (string q in plan.RuleOnly.Where(t => t.ServesCropTreeId == SampleCrops[0]).Select(t => t.Id))
        {
            var playerSet = picks.Where(p => p != q).Concat(new[] { weak, farAway }).ToList();
            var d = Derive(fx, playerSet, out var st);
            if (d.Ready) { differ = d; differStems = st; dropped = q; break; }
        }
        Check(differ != null, "fit: a disagreeing player's sample still passes the lesson", dropped ?? "none");
        if (differ != null)
        {
            Pres.FitResult fit = Pres.CheckFit(differ, geometry, differStems, fx.Masks);
            var playerIds = new HashSet<string>(differ.SampleRemovalIds);
            var ruleIds = new HashSet<string>(fit.Agreements.Concat(fit.RuleOnly).Select(t => t.Id));
            Check(fit.Valid && fit.Agreements.Concat(fit.PlayerOnly).Select(t => t.Id).OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(playerIds.OrderBy(x => x, StringComparer.Ordinal))
                  && fit.Agreements.All(t => t.PlayerRemoved && t.RuleRemoved) && fit.PlayerOnly.All(t => t.PlayerRemoved && !t.RuleRemoved)
                  && fit.RuleOnly.All(t => !t.PlayerRemoved && t.RuleRemoved), "fit: agreements + player-only = the player's removals; agreements + rule-only = the rule's");
            Check(fit.PlayerOnly.Any(t => t.Id == weak && t.ServesCropTreeId == SampleCrops[0]) , "fit: the weak extra competitor is a player-only decision attributed to its Crop Tree", weak);
            Check(fit.PlayerOnly.Any(t => t.Id == farAway && t.ServesCropTreeId == "" && t.ShareOfCropTree == 0), "fit: a removal serving no Crop Tree is listed as such", farAway);
            Check(fit.RuleOnly.Any(t => t.Id == dropped), "fit: the strong competitor the player kept is a rule-only decision", dropped);
            Check(fit.CropTrees.Count == 3 && fit.CropTrees.All(c => c.PlayerShare > 0 && c.RuleShare > 0) && fit.PlayerBasalShare > 0 && fit.RuleBasalShare > 0
                  && fit.RuleBasalShare <= differ.Rule.BasalCeilingShare + 1e-6, "fit: per-Crop-Tree Share and basal-area consequences reported; rule within its ceiling");
            Check(Pres.CheckFit(differ, geometry, differStems, fx.Masks).Signature() == fit.Signature(), "determinism: repeated fit identical");
        }

        // Open or unexecuted sample work, and non-Sitka sample trees, stop the derivation.
        string extra = footprint.Select(s => s.Id).First(id => id != weak && id != farAway);
        var marked = agreeStems.Select(s => s.Id == extra ? With(s, fellMark: true) : s).ToList();
        Check(Pres.Derive(geometry, marked, agreeOrders, Cal.V1).Status == Pres.Status.SampleHasOpenWork, "derive: a loose Fell mark in the sample -> SampleHasOpenWork");
        var openOrders = new List<ScenarioOneRackBay.Order>(agreeOrders) { new ScenarioOneRackBay.Order(extra, true, false, ScenarioOneRackBay.Disposition.Sold, -1) };
        Check(Pres.Derive(geometry, agreeStems, openOrders, Cal.V1).Status == Pres.Status.SampleHasOpenWork, "derive: planned felling in the sample -> SampleHasOpenWork");
        var sampleOak = agreeStems.Select(s => s.Id == extra ? With(s, species: "oak") : s).ToList();
        Check(Pres.Derive(geometry, sampleOak, agreeOrders, Cal.V1).Status == Pres.Status.SampleHasNonSitka, "derive: a non-Sitka sample tree -> SampleHasNonSitka");
        // Over-removal fails the lesson's own 40% safeguard and cannot be generalised.
        var heavy = picks.Concat(footprint.Where(s => s.Id != weak && s.Id != farAway).Select(s => s.Id).Take(60)).ToList();
        Pres.Derivation over = Derive(fx, heavy, out _);
        Check(over.Status == Pres.Status.SampleNotQualified && over.SampleRemovedBasalShare > 0.40, "derive: a sample over the 40% safeguard is not qualified", F(over.SampleRemovedBasalShare));

        // Shuffled input order changes nothing.
        var shuffled = Shuffle(agreeStems);
        var shuffledOrders = Enumerable.Reverse(agreeOrders).ToList();
        Check(Pres.Derive(geometry, shuffled, shuffledOrders, Cal.V1).Signature() == agree.Signature()
              && Pres.CheckFit(Pres.Derive(geometry, shuffled, shuffledOrders, Cal.V1), geometry, shuffled, fx.Masks).Signature() == same.Signature(),
            "determinism: derivation and fit identical for shuffled trees and orders");
        return Sha(agree.Signature() + "|" + same.Signature());
    }

    private static List<ScenarioOneRackBay.Order> OrdersFor(Fixture fx, IEnumerable<string> removals)
        => fx.Orders.Concat(removals.Select(id => new ScenarioOneRackBay.Order(id, false, true, ScenarioOneRackBay.Disposition.Sold, 1))).ToList();

    private static Pres.Derivation Derive(Fixture fx, List<string> removals, out List<Cov.Stem> stems)
    {
        var set = new HashSet<string>(removals);
        stems = fx.Stems.Select(s => set.Contains(s.Id) ? With(s, living: false) : s).ToList();
        return Pres.Derive(fx.Geometry, stems, OrdersFor(fx, removals), Cal.V1);
    }

    private static string WithoutPutBack(string signature)
    {
        int start = signature.IndexOf(" put-back=", StringComparison.Ordinal), end = signature.IndexOf(" | ", start, StringComparison.Ordinal);
        return signature.Substring(0, start) + signature.Substring(end);
    }

    // ---------------------------------------------------------------------------------------------------------- determinism

    private static List<T> Shuffle<T>(List<T> list)
    {
        // Deterministic interleave: reverse, then take every third element first.
        var reversed = Enumerable.Reverse(list).ToList();
        return reversed.Where((_, i) => i % 3 == 0).Concat(reversed.Where((_, i) => i % 3 != 0)).ToList();
    }

    private static void Determinism(Fixture fx, Cov.Result coverage, Cand.Result candidates, Pres.ReleasePlan release)
    {
        string covSig = coverage.Signature();
        Check(Cov.Classify(Input(fx), Cal.V1).Signature() == covSig, "determinism: repeated coverage identical");
        var shuffled = Shuffle(fx.Stems);
        Check(Cov.Classify(Input(fx, shuffled, Enumerable.Reverse(fx.Orders).ToList()), Cal.V1).Signature() == covSig, "determinism: coverage identical for shuffled trees and orders");
        // Racks listed in another order, and every rack drawn far-to-entrance, describe the same plan.
        var reversedRacks = RackZ.Select((z, i) => new ScenarioOneRackSampleGeometry.RackInput(i + 1, new RackPoint(FarX, z), new RackPoint(EntranceX, z), RackGeometry.DefaultWidthM))
            .Reverse().ToList();
        var flipped = ScenarioOneRackSampleGeometry.Derive(reversedRacks, Half);
        Check(Cov.Classify(Input(fx, geometry: flipped), Cal.V1).Signature() == covSig, "determinism: coverage identical for reordered, reversed-direction racks");
        Check(Cand.Suggest(coverage, shuffled, SampleCrops.Reverse().ToList(), null, null, Cal.V1).Signature() == candidates.Signature(), "determinism: candidates identical for shuffled input");
        var confirmed = candidates.Candidates.Select(c => c.TreeId).ToList();
        var confirmedStems = Shuffle(fx.Stems.Select(s => confirmed.Contains(s.Id) ? With(s, crop: true) : s).ToList());
        Check(Pres.ProposeRelease(Pres.ReleaseRule.Create(0.30, LiveInitialReleaseBaShare), coverage, confirmedStems, Enumerable.Reverse(confirmed).ToList(), SampleCrops).Signature()
              == release.Signature(), "determinism: release proposals identical for shuffled trees and confirmation order");
        Check(Cal.V1.Signature() == Cal.V1.Signature() && covSig.StartsWith(Cal.V1.Signature(), StringComparison.Ordinal),
            "determinism: every coverage signature starts with the exact calibration signature");
    }

    private static string Sha(string text)
    {
        using (var sha = SHA256.Create())
            return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2"))).Substring(0, 16);
    }
}
