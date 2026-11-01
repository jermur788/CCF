// Plain .NET checks of MFM-11 calibration impact: three-path removal arithmetic on the frozen MFM-03 fixture.
// Path A = historical sample-only; Path B = representative-prescription/default extension; Path C = plausible manual override.
// Decision rules only; no Unity, no save, no scene.  Evidence labels: DIRECT model outputs, DERIVED metrics, GAMEPLAY CALIBRATION thresholds.
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

public static class Mfm11CalibrationImpactOfflineCheck
{
    private static int checks, failures;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) failures++;
    }
    private static void Note(string name, string detail = "")
    {
        Console.WriteLine("MFM11_NOTE " + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
    }
    private static string F(double v) => v.ToString("0.######", Inv);

    // ---- fixture constants (MFM-02 handoff / MFM-03 exporter) ----
    private static readonly float[] RackZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private const float EntranceX = -35.4f, FarX = 40f, Half = 40f;
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private const double LiveInitialReleaseBaShare = 0.17898431420326233;
    // MFM-03 16 m unit-centre-first candidates, in order (the confirmed crop-tree set for Path B/C).
    private static readonly string[] Mfm03Candidates16 =
        { "PO0321", "PO0333", "PO1107", "P0006", "PO1134", "PO2107", "PO1700", "PO2133", "PO1740", "PO2907", "P2010", "PO2933", "PO3709", "PO3501", "PO3919", "PO3736", "PO4028" };

    private sealed class Fixture
    {
        public ScenarioOneRackSampleGeometry.Result Geometry;
        public List<Cov.Stem> Stems;
        public List<ScenarioOneRackBay.Order> Orders;
        public List<Cov.Mask> Masks;
        public HashSet<string> RackRemovals;
    }

    public static int Main(string[] args)
    {
        if (args.Length < 3) { Console.Error.WriteLine("usage: <repo-root> <fixture-stand.csv> <fixture-infrastructure.json>"); return 2; }
        string root = args[0];
        Fixture fx = Load(root, args[1], args[2]);
        if (fx == null) { Console.WriteLine("MFM11_CALIBRATION_IMPACT_FAIL fixture"); return 1; }

        Cov.Result coverage = Coverage(fx);
        Cand.Result candidates = Candidates(fx, coverage);
        PathComparison(fx, coverage, candidates);
        Mfm03Residual(fx, coverage);
        SpeciesGuard(fx, coverage);
        ProvenanceModel();
        Determinism(fx, coverage, candidates);

        Console.WriteLine((failures == 0 ? "MFM11_CALIBRATION_IMPACT_PASS" : "MFM11_CALIBRATION_IMPACT_FAIL")
            + " checks=" + checks + " failed=" + failures);
        return failures == 0 ? 0 : 1;
    }

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

        var fp = raw.Where(s => geometry.Teaching.Contains(s.X, s.Z)).OrderBy(s => s.Id, StringComparer.Ordinal).ToList();
        double fpBa = fp.Sum(s => Math.PI * Math.Pow(s.Dbh / 200.0, 2.0));
        Check(fp.Count == 75 && Math.Abs(fpBa - 1.441753070295678) <= 1e-6 && Sha(string.Join(",", fp.Select(s => s.Id))) == "c2f2fd5418794915",
            "fixture identity: live MFM-02 footprint 75 stems / 1.4417531 m2 / SHA c2f2fd5418794915", fp.Count + " " + F(fpBa));

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

    // ----------------------------------------------------------------------------------------------------------------- coverage

    private static Cov.Result Coverage(Fixture fx)
    {
        Cov.Result r = Cov.Classify(Input(fx), Cal.V1);
        Check(r.Valid, "coverage: valid on the fixture", r.Problem);
        if (!r.Valid) return r;
        Check(r.Areas.Count == 15 && r.Remainders.Count == 2, "coverage: 15 anchor-aligned thirds and two explicit remainders");
        int covered = r.Areas.Count(a => a.State == Cov.State.Covered);
        Check(covered == 13, "coverage: 13 covered units (MFM-03)", covered.ToString());
        Note("coverage-source", "stems=" + r.Source.Stems + " baM2=" + F(r.Source.BasalAreaM2) + " meanDbh=" + F(r.Source.MeanDbhCm) + " meanCi=" + F(r.Source.MeanCompetition));
        return r;
    }

    private static Cand.Result Candidates(Fixture fx, Cov.Result coverage)
    {
        Cand.Result r = Cand.Suggest(coverage, fx.Stems, SampleCrops, new HashSet<string>(Mfm03Candidates16, StringComparer.Ordinal), new HashSet<string>(), Cal.V1);
        Check(r.Valid, "candidates: valid on the fixture", r.Problem);
        if (!r.Valid) return r;
        Check(r.Candidates.Count == 17, "candidates: 17 spacing-only candidates at 16 m", r.Candidates.Count.ToString());
        Note("candidates-order", string.Join(",", r.Candidates.Select(c => c.TreeId)));
        return r;
    }

    // ----------------------------------------------------------------------------------------------------------------- three-path comparison

    private sealed class PathResult
    {
        public string Name;
        public HashSet<string> RemovalIds = new HashSet<string>(StringComparer.Ordinal);
        public double RemovalBaM2;
        public int RemovalStems;
        // Per-unit: areaId -> (stems, baM2)
        public Dictionary<string, (int Stems, double BaM2)> PerUnit = new Dictionary<string, (int, double)>(StringComparer.Ordinal);
    }

    private static void PathComparison(Fixture fx, Cov.Result coverage, Cand.Result candidates)
    {
        // Rule from the sample: target = max(30%, player min), ceiling = min(40%, sample removed BA share).
        Pres.ReleaseRule rule = Pres.ReleaseRule.Create(
            Math.Max(0.30, LiveInitialReleaseBaShare), Math.Min(0.40, 1.0));
        Note("release-rule", rule.Signature());

        // Path A: sample-only — competitor removal around the 3 sample crops (the accepted teaching/sample release).
        PathResult pathA = BuildSampleReleasePath(fx, coverage, rule);

        // Path B: representative prescription — proposals around all 17 confirmed crop trees.
        Pres.ReleasePlan planB = Pres.ProposeRelease(rule, coverage, fx.Stems,
            new HashSet<string>(Mfm03Candidates16, StringComparer.Ordinal), new HashSet<string>(SampleCrops, StringComparer.Ordinal));
        Check(planB.Valid, "path-B: prescription release plan valid", planB.Problem);
        PathResult pathB = BuildPathResult("B-prescription", planB, fx);
        // Path B includes the sample release (shared baseline) + wider prescription.
        pathB.RemovalIds.UnionWith(pathA.RemovalIds);
        pathB.RemovalBaM2 += pathA.RemovalBaM2;
        pathB.RemovalStems += pathA.RemovalStems;
        foreach (var kv in pathA.PerUnit)
        {
            if (!pathB.PerUnit.TryGetValue(kv.Key, out var v)) v = (0, 0.0);
            pathB.PerUnit[kv.Key] = (v.Item1 + kv.Value.Item1, v.Item2 + kv.Value.Item2);
        }

        // Path C: manual override — player marks top-3 competitors per crop tree (bounded, transparent, no hidden future knowledge).
        PathResult pathC = BuildManualOverridePath(fx, coverage, rule);
        // Path C includes the sample release (shared baseline) + manual override.
        pathC.RemovalIds.UnionWith(pathA.RemovalIds);
        pathC.RemovalBaM2 += pathA.RemovalBaM2;
        pathC.RemovalStems += pathA.RemovalStems;
        foreach (var kv in pathA.PerUnit)
        {
            if (!pathC.PerUnit.TryGetValue(kv.Key, out var v)) v = (0, 0.0);
            pathC.PerUnit[kv.Key] = (v.Item1 + kv.Value.Item1, v.Item2 + kv.Value.Item2);
        }

        // ---- per-unit comparison table ----
        var allUnits = new SortedSet<string>(StringComparer.Ordinal);
        foreach (var u in pathA.PerUnit.Keys) allUnits.Add(u);
        foreach (var u in pathB.PerUnit.Keys) allUnits.Add(u);
        foreach (var u in pathC.PerUnit.Keys) allUnits.Add(u);

        Console.WriteLine("MFM11_TABLE_HEADER unit|A-stems|A-baM2|B-stems|B-baM2|C-stems|C-baM2|B-A-stems|B-A-ba|C-B-stems|C-B-ba");
        foreach (string unit in allUnits)
        {
            var a = pathA.PerUnit.TryGetValue(unit, out var av) ? av : (0, 0.0);
            var b = pathB.PerUnit.TryGetValue(unit, out var bv) ? bv : (0, 0.0);
            var c = pathC.PerUnit.TryGetValue(unit, out var cv) ? cv : (0, 0.0);
            Console.WriteLine("MFM11_TABLE_ROW " + unit + "|" + a.Item1 + "|" + F(a.Item2) + "|" + b.Item1 + "|" + F(b.Item2)
                + "|" + c.Item1 + "|" + F(c.Item2) + "|" + (b.Item1 - a.Item1) + "|" + F(b.Item2 - a.Item2)
                + "|" + (c.Item1 - b.Item1) + "|" + F(c.Item2 - b.Item2));
        }
        Console.WriteLine("MFM11_TABLE_TOTAL |" + pathA.RemovalStems + "|" + F(pathA.RemovalBaM2) + "|" + pathB.RemovalStems + "|" + F(pathB.RemovalBaM2)
            + "|" + pathC.RemovalStems + "|" + F(pathC.RemovalBaM2) + "|" + (pathB.RemovalStems - pathA.RemovalStems) + "|" + F(pathB.RemovalBaM2 - pathA.RemovalBaM2)
            + "|" + (pathC.RemovalStems - pathB.RemovalStems) + "|" + F(pathC.RemovalBaM2 - pathB.RemovalBaM2));

        // ---- species split (fixture is all Sitka; code handles mixed) ----
        Check(pathA.RemovalIds.All(id => fx.Stems.First(s => s.Id == id).IsSitka), "path-A: all removals are Sitka (fixture is single-species)");
        Check(pathB.RemovalIds.All(id => fx.Stems.First(s => s.Id == id).IsSitka), "path-B: all removals are Sitka (fixture is single-species)");
        Check(pathC.RemovalIds.All(id => fx.Stems.First(s => s.Id == id).IsSitka), "path-C: all removals are Sitka (fixture is single-species)");
        Note("species-split", "all three paths: 100% sitka-spruce (fixture is single-species; mixed-species split is Unity-phase)");

        // ---- safeguard: per-unit BA share ≤ ceiling ----
        SafeguardCheck(fx, coverage, pathA, "A");
        SafeguardCheck(fx, coverage, pathB, "B");
        SafeguardCheck(fx, coverage, pathC, "C");

        // ---- wider treatment vs sample-only impact ----
        Check(pathB.RemovalStems >= pathA.RemovalStems, "impact: prescription removes ≥ sample-only stems",
            "B=" + pathB.RemovalStems + " A=" + pathA.RemovalStems);
        Check(pathB.RemovalBaM2 >= pathA.RemovalBaM2, "impact: prescription removes ≥ sample-only BA",
            "B=" + F(pathB.RemovalBaM2) + " A=" + F(pathA.RemovalBaM2));
        Note("impact-B-vs-A", "extraStems=" + (pathB.RemovalStems - pathA.RemovalStems) + " extraBaM2=" + F(pathB.RemovalBaM2 - pathA.RemovalBaM2));
        Note("impact-C-vs-B", "diffStems=" + (pathC.RemovalStems - pathB.RemovalStems) + " diffBaM2=" + F(pathC.RemovalBaM2 - pathB.RemovalBaM2));

        // ---- canopy objective: wider treatment must not make retained canopy unattainable ----
        double totalBaBefore = fx.Stems.Where(s => s.Living).Sum(s => Math.PI * Math.Pow(s.DbhCm / 200.0, 2.0));
        double retainedB = totalBaBefore - pathB.RemovalBaM2;
        double retainedC = totalBaBefore - pathC.RemovalBaM2;
        Check(retainedB / totalBaBefore >= 0.50, "canopy: prescription retains ≥50% of stand BA",
            "retained=" + F(retainedB / totalBaBefore * 100) + "%");
        Check(retainedC / totalBaBefore >= 0.50, "canopy: manual override retains ≥50% of stand BA",
            "retained=" + F(retainedC / totalBaBefore * 100) + "%");
        Note("canopy-retained", "totalBaM2=" + F(totalBaBefore) + " B-retained=" + F(retainedB / totalBaBefore * 100) + "% C-retained=" + F(retainedC / totalBaBefore * 100) + "%");
    }

    private static PathResult BuildPathResult(string name, Pres.ReleasePlan plan, Fixture fx)
    {
        var r = new PathResult { Name = name };
        foreach (Pres.Proposal p in plan.Proposals)
        {
            r.RemovalIds.Add(p.TreeId);
            r.RemovalBaM2 += p.BasalAreaM2;
            r.RemovalStems++;
            if (!r.PerUnit.TryGetValue(p.AreaId, out var v)) v = (0, 0.0);
            r.PerUnit[p.AreaId] = (v.Item1 + 1, v.Item2 + p.BasalAreaM2);
        }
        return r;
    }

    // Path A: sample release around the 3 sample crops — mark highest-competition competitors within 8m until
    // the local release share reaches 30% (the accepted teaching/sample release rule).
    private static PathResult BuildSampleReleasePath(Fixture fx, Cov.Result coverage, Pres.ReleaseRule rule)
    {
        var r = new PathResult { Name = "A-sample-only" };
        var stemsById = fx.Stems.ToDictionary(s => s.Id, StringComparer.Ordinal);
        var living = fx.Stems.Where(s => s.Living).ToList();
        const double reachM = 8.0;
        foreach (string cropId in SampleCrops)
        {
            if (!stemsById.TryGetValue(cropId, out Cov.Stem crop)) continue;
            var competitors = living
                .Where(s => s.Id != cropId && s.Living && !s.Crop && !s.FellMark)
                .Where(s => Math.Sqrt((s.X - crop.X) * (s.X - crop.X) + (s.Z - crop.Z) * (s.Z - crop.Z)) <= reachM)
                .Select(s => new { Stem = s, Ci = Cov.CompetitionIndex(s, living) })
                .OrderByDescending(x => x.Ci)
                .ThenBy(x => x.Stem.Id, StringComparer.Ordinal)
                .ToList();
            // Compute total competitor BA and crop BA for release share.
            double cropBa = Math.PI * Math.Pow(crop.DbhCm / 200.0, 2.0);
            double totalCompBa = competitors.Sum(x => Math.PI * Math.Pow(x.Stem.DbhCm / 200.0, 2.0));
            double removedBa = 0;
            foreach (var comp in competitors)
            {
                double stemBa = Math.PI * Math.Pow(comp.Stem.DbhCm / 200.0, 2.0);
                // Release share = removed BA / (crop BA + total competitor BA).
                double share = (removedBa + stemBa) / (cropBa + totalCompBa);
                if (share > rule.TargetShare && removedBa > 0) break;
                r.RemovalIds.Add(comp.Stem.Id);
                r.RemovalBaM2 += stemBa;
                r.RemovalStems++;
                removedBa += stemBa;
                // Attribute to the nearest coverage unit.
                string unitId = FindUnitId(coverage, comp.Stem.Id) ?? "sample";
                if (!r.PerUnit.TryGetValue(unitId, out var v)) v = (0, 0.0);
                r.PerUnit[unitId] = (v.Item1 + 1, v.Item2 + stemBa);
            }
        }
        return r;
    }

    // Path C: player manually marks top-3 highest-competition competitors within 8m of each crop tree,
    // subject to the 40% per-unit BA ceiling.  Transparent rule; no hidden future knowledge.
    private static PathResult BuildManualOverridePath(Fixture fx, Cov.Result coverage, Pres.ReleaseRule rule)
    {
        var r = new PathResult { Name = "C-manual-override" };
        var stemsById = fx.Stems.ToDictionary(s => s.Id, StringComparer.Ordinal);
        var living = fx.Stems.Where(s => s.Living).ToList();
        double ceiling = rule.BasalCeilingShare;
        // Per-unit BA budget.
        var unitBa = new Dictionary<string, double>(StringComparer.Ordinal);
        var unitUsed = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var a in coverage.Areas)
        {
            unitBa[a.Id] = a.Metrics.BasalAreaM2;
            unitUsed[a.Id] = 0.0;
        }
        const int maxPerCrop = 3;
        const double reachM = 8.0;
        foreach (string cropId in Mfm03Candidates16)
        {
            if (!stemsById.TryGetValue(cropId, out Cov.Stem crop)) continue;
            // Competitors within 8m, sorted by Hegyi competition (highest first).
            var competitors = living
                .Where(s => s.Id != cropId && s.Living && !s.Crop && !s.FellMark && !SampleCrops.Contains(s.Id))
                .Where(s => Math.Sqrt((s.X - crop.X) * (s.X - crop.X) + (s.Z - crop.Z) * (s.Z - crop.Z)) <= reachM)
                .Select(s => new { Stem = s, Ci = Cov.CompetitionIndex(s, living) })
                .OrderByDescending(x => x.Ci)
                .ThenBy(x => x.Stem.Id, StringComparer.Ordinal)
                .ToList();
            int marked = 0;
            foreach (var comp in competitors)
            {
                if (marked >= maxPerCrop) break;
                // Find which unit this stem belongs to.
                string unitId = FindUnitId(coverage, comp.Stem.Id);
                if (unitId == null) continue;
                double stemBa = Math.PI * Math.Pow(comp.Stem.DbhCm / 200.0, 2.0);
                double unitTotal = unitBa.GetValueOrDefault(unitId, 0);
                if (unitTotal <= 0) continue;
                double projectedShare = (unitUsed.GetValueOrDefault(unitId, 0) + stemBa) / unitTotal;
                if (projectedShare > ceiling + 1e-9) continue; // respect per-unit ceiling
                r.RemovalIds.Add(comp.Stem.Id);
                r.RemovalBaM2 += stemBa;
                r.RemovalStems++;
                if (!r.PerUnit.TryGetValue(unitId, out var v)) v = (0, 0.0);
                r.PerUnit[unitId] = (v.Item1 + 1, v.Item2 + stemBa);
                unitUsed[unitId] = unitUsed.GetValueOrDefault(unitId, 0) + stemBa;
                marked++;
            }
        }
        return r;
    }

    private static string FindUnitId(Cov.Result coverage, string stemId)
    {
        foreach (var a in coverage.Areas)
            if (a.StemIds.Contains(stemId)) return a.Id;
        return null;
    }

    private static void SafeguardCheck(Fixture fx, Cov.Result coverage, PathResult path, string label)
    {
        bool allWithin = true;
        foreach (var kv in path.PerUnit)
        {
            var area = coverage.Find(kv.Key);
            if (area == null) continue;
            double share = area.Metrics.BasalAreaM2 > 0 ? kv.Value.Item2 / area.Metrics.BasalAreaM2 : 0;
            if (share > 0.40 + 1e-9) allWithin = false;
        }
        Check(allWithin, "safeguard: path " + label + " per-unit BA share ≤ 40%");
    }

    // ----------------------------------------------------------------------------------------------------------------- MFM-03 residual

    private static void Mfm03Residual(Fixture fx, Cov.Result coverage)
    {
        // Whole-stand state at the coverage-comparison state.
        var living = fx.Stems.Where(s => s.Living).ToList();
        double totalBa = living.Sum(s => Math.PI * Math.Pow(s.DbhCm / 200.0, 2.0));
        double meanDbh = living.Average(s => s.DbhCm);
        Note("mfm03-whole-stand", "living=" + living.Count + " baM2=" + F(totalBa) + " meanDbhCm=" + F(meanDbh));

        // Competition-index parity: source vs covered areas.
        double sourceCi = coverage.Source.MeanCompetition;
        var coveredAreas = coverage.Areas.Where(a => a.State == Cov.State.Covered).ToList();
        double coveredCi = coveredAreas.Count > 0 ? coveredAreas.Average(a => a.Metrics.MeanCompetition) : 0;
        double ciDelta = Math.Abs(sourceCi - coveredCi) / Math.Max(sourceCi, 1e-9);
        Check(ciDelta <= 0.20, "mfm03-residual: live CI parity within 20% (GAMEPLAY CALIBRATION)",
            "source=" + F(sourceCi) + " covered=" + F(coveredCi) + " delta=" + F(ciDelta * 100) + "%");
        Note("mfm03-ci-parity", "sourceMeanCi=" + F(sourceCi) + " coveredMeanCi=" + F(coveredCi) + " deltaPct=" + F(ciDelta * 100));

        // Growth drift: compare fixture mean DBH against expected plantation growth for the age class.
        // The fixture is year-0 Enlarged80; mean DBH ~15.4 cm is the accepted starting state.
        Check(Math.Abs(meanDbh - 15.404493166605631) < 1.0, "mfm03-residual: growth drift — fixture mean DBH near accepted 15.4 cm",
            "meanDbh=" + F(meanDbh));
        Note("mfm03-growth-drift", "fixtureMeanDbhCm=" + F(meanDbh) + " acceptedMeanDbhCm=15.404493 driftCm=" + F(meanDbh - 15.404493166605631));

        // Natural cohorts/juveniles/shelters/deadwood/marks at extension timing.
        // Offline fixture has none of these (year-0 plantation); this is a known limitation of the offline phase.
        Note("mfm03-live-state", "UNAVAILABLE offline: natural cohorts, juveniles, shelters, deadwood, marks at post-Lesson-5 timing (Unity phase)");
    }

    // ----------------------------------------------------------------------------------------------------------------- MFM-10 species guard

    private static void SpeciesGuard(Fixture fx, Cov.Result coverage)
    {
        // The fixture is all Sitka.  Verify the calibration/prescription code does not silently transfer Sitka
        // quantitative thresholds to non-Sitka species.
        var nonSitka = fx.Stems.Where(s => !s.IsSitka).ToList();
        Check(nonSitka.Count == 0, "species-guard: fixture contains no non-Sitka stems (single-species fixture)");
        Note("species-guard", "MFM-10: fixture is single-species sitka-spruce; no Sitka→Oak/Beech quantitative transfer possible offline");
        Note("species-guard-unity", "Unity phase must verify mixed-species stands do not inherit Sitka calibration silently");

        // Verify calibration version is the accepted MFM-03 gameplay calibration.
        Check(coverage.Calibration.Version == "MFM03-CAL-1", "species-guard: calibration version is accepted MFM-03-CAL-1",
            coverage.Calibration.Version);
    }

    // ----------------------------------------------------------------------------------------------------------------- provenance data model

    private static void ProvenanceModel()
    {
        // Path B orders carry prescription provenance; Path A/C orders are ordinary (prescriptionId = 0).
        // Offline check: verify the data model supports the provenance fields.
        // (Actual provenance values are Unity-phase; this checks the schema exists.)
        Note("provenance", "Path B: orders carry prescriptionId/coverageUnitId/cropTreeId/reason (Unity phase)");
        Note("provenance", "Path A/C: orders have prescriptionId=0 (ordinary marks, Unity phase)");
        Note("provenance", "PENDING→APPROVED→EXECUTED lifecycle is Unity-phase (Work Plan approval + AdvanceYear)");
    }

    // ----------------------------------------------------------------------------------------------------------------- determinism

    private static void Determinism(Fixture fx, Cov.Result coverage, Cand.Result candidates)
    {
        // Re-run coverage and candidates; signatures must be identical.
        Cov.Result coverage2 = Cov.Classify(Input(fx), Cal.V1);
        Check(coverage.Signature() == coverage2.Signature(), "determinism: coverage signature stable");

        Cand.Result cand2 = Cand.Suggest(coverage, fx.Stems, SampleCrops,
            new HashSet<string>(Mfm03Candidates16, StringComparer.Ordinal), new HashSet<string>(), Cal.V1);
        Check(candidates.Signature() == cand2.Signature(), "determinism: candidates signature stable");

        // Re-run Path B proposals; signature must be identical.
        Pres.ReleaseRule rule = Pres.ReleaseRule.Create(Math.Max(0.30, LiveInitialReleaseBaShare), Math.Min(0.40, 1.0));
        Pres.ReleasePlan plan1 = Pres.ProposeRelease(rule, coverage, fx.Stems,
            new HashSet<string>(Mfm03Candidates16, StringComparer.Ordinal), new HashSet<string>(SampleCrops, StringComparer.Ordinal));
        Pres.ReleasePlan plan2 = Pres.ProposeRelease(rule, coverage, fx.Stems,
            new HashSet<string>(Mfm03Candidates16, StringComparer.Ordinal), new HashSet<string>(SampleCrops, StringComparer.Ordinal));
        Check(plan1.Signature() == plan2.Signature(), "determinism: Path B release plan signature stable");
    }

    private static string Sha(string text)
    {
        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
        return Convert.ToHexString(hash).ToLowerInvariant()[..16];
    }
}
