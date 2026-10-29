// MFM-03 coverage calibration: MEASUREMENT-ONLY exporter. Not production code and never copied into Assets/.
//
// It compiles beside the unmodified production geometry/competition sources (ScenarioOneRackSampleGeometry, ScenarioOneRackBay,
// RackGeometry, CompetitionShare) and the existing offline ForestEcologyController stand-in, reads the frozen fixture stand and
// writes one JSON file of per-tree facts for analyse_coverage.py. It derives nothing that production does not already derive:
// coverage units/remainders come from ScenarioOneRackSampleGeometry.Derive, the footprint from its Teaching frame, rack removal
// sets from RackGeometry.Select, and every competition index from CompetitionShare.Measure.
//
// State measured: the fresh Enlarged80 Rack V2 starting stand (year 0) with the MFM-02 qualified five-rack plan, before access
// ("pre") and with every rack's removal set taken out ("post", same year, no growth). Growth, regeneration and post-Lesson-5
// management state are NOT simulated here; see the handoff's Unity-only residual.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

public static class CoverageExport
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    // MFM-02 qualified fixture (Docs/Handoffs/MFM-Implementation-02/HANDOFF.md, live Unity rack records on 8f9be4f).
    private static readonly float[] FixtureZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private const float EntranceX = -35.4f, FarX = 40f, StandHalfExtent = 40f;
    private const string LiveFootprintSha = "c2f2fd5418794915";

    // Hypothetical layouts used ONLY to show remainder behaviour when a remainder is not empty. They are not Rack V2-qualified plans.
    private static readonly (string Name, float[] Z)[] RemainderProbes =
    {
        ("probe-uniform16", new[] { -32f, -16f, 0f, 16f, 32f }),
        ("probe-shift-north-4m", new[] { -28.1650f, -12.0750f, 4f, 20.05f, 36.26f }),
        ("probe-shift-south-4m", new[] { -36.79f, -20.175f, -4f, 12f, 28.57f }),
        ("probe-compressed-16m-centred", new[] { -30f, -14f, 2f, 18f, 34f }),
    };

    private sealed class Stem { public string Id; public float X, Z, Dbh, Height; }

    public static int Main(string[] args)
    {
        if (args.Length < 3) { Console.Error.WriteLine("usage: CoverageExport <repo-root> <fixture-stand.csv> <out.json>"); return 2; }
        string root = args[0];
        var stems = ReadStand(args[1]);
        var problems = new List<string>();
        SourceParity(root, problems);

        var racks = new List<ScenarioOneRackSampleGeometry.RackInput>();
        for (int i = 0; i < FixtureZ.Length; i++)
            racks.Add(new ScenarioOneRackSampleGeometry.RackInput(i + 1, new RackPoint(EntranceX, FixtureZ[i]), new RackPoint(FarX, FixtureZ[i]), RackGeometry.DefaultWidthM));
        var geo = ScenarioOneRackSampleGeometry.Derive(racks, StandHalfExtent);
        if (!geo.Valid) { Console.Error.WriteLine("fixture geometry invalid: " + geo.Problem); return 1; }

        // Rack removal sets in rack-id order, as the manager does: a tree already claimed by an earlier rack is not re-selected.
        var claimed = new HashSet<string>(StringComparer.Ordinal);
        var removalByRack = new Dictionary<int, List<string>>();
        foreach (var rack in racks)
        {
            var input = stems.Select(s => new RackTree(s.Id, s.X, s.Z, true, false, claimed.Contains(s.Id))).ToList();
            RackSelection sel = RackGeometry.Select(input, rack.Entrance, rack.Far, rack.WidthM);
            removalByRack[rack.Id] = sel.Trees.Select(t => t.Id).ToList();
            foreach (var t in sel.Trees) claimed.Add(t.Id);
        }

        var pre = stems;
        var post = stems.Where(s => !claimed.Contains(s.Id)).ToList();
        var ciPre = Competition(pre, null);
        var neighbours = new Dictionary<string, List<(string Id, double Term)>>();
        var ciPost = Competition(post, neighbours);

        // Footprint identity against the live MFM-02 measurement (pre-access world).
        var footprintIds = pre.Where(s => geo.Teaching.Contains(s.X, s.Z)).Select(s => s.Id).OrderBy(v => v, StringComparer.Ordinal).ToList();
        double footprintBa = pre.Where(s => geo.Teaching.Contains(s.X, s.Z)).Sum(s => Math.PI * Math.Pow(s.Dbh / 200.0, 2.0));
        string footprintSha = Sha16(string.Join(",", footprintIds));
        if (footprintIds.Count != 75) problems.Add("footprint count " + footprintIds.Count + " != live 75");
        if (footprintSha != LiveFootprintSha) problems.Add("footprint SHA " + footprintSha + " != live " + LiveFootprintSha);
        if (Math.Abs(footprintBa - 1.441753070295678) > 1e-6) problems.Add("footprint BA " + footprintBa.ToString("R", Inv) + " != live 1.441753070295678");

        var sb = new StringBuilder();
        sb.Append("{\n");
        sb.Append("\"signature\":").Append(Q(geo.Signature())).Append(",\n");
        sb.Append("\"problems\":[").Append(string.Join(",", problems.Select(Q))).Append("],\n");
        sb.Append("\"footprint\":{\"count\":").Append(footprintIds.Count).Append(",\"ba\":").Append(N(footprintBa)).Append(",\"sha\":").Append(Q(footprintSha))
          .Append(",\"anchorX\":").Append(N(geo.Anchor.X)).Append(",\"anchorZ\":").Append(N(geo.Anchor.Z))
          .Append(",\"halfWidth\":").Append(N(geo.Teaching.HalfWidth)).Append(",\"halfLength\":").Append(N(ScenarioOneRackBay.HalfLengthM)).Append("},\n");
        sb.Append("\"racks\":[").Append(string.Join(",", geo.Racks.Select(r => "{\"id\":" + r.Id + ",\"index\":" + r.Index + ",\"offset\":" + N(r.OffsetM)
            + ",\"entranceX\":" + N(r.Entrance.X) + ",\"entranceZ\":" + N(r.Entrance.Z) + ",\"farX\":" + N(r.Far.X) + ",\"farZ\":" + N(r.Far.Z) + ",\"width\":" + N(r.WidthM)
            + ",\"removals\":[" + string.Join(",", removalByRack[r.Id].Select(Q)) + "]}"))).Append("],\n");
        sb.Append("\"units\":[").Append(string.Join(",", geo.Units.Select(UnitJson))).Append("],\n");
        sb.Append("\"remainders\":[").Append(RemainderJson(geo.RightRemainder)).Append(",").Append(RemainderJson(geo.LeftRemainder)).Append("],\n");
        sb.Append("\"normal\":[").Append(N(geo.NormalX)).Append(",").Append(N(geo.NormalZ)).Append("],\n");

        sb.Append("\"trees\":[\n");
        var postIds = new HashSet<string>(post.Select(s => s.Id), StringComparer.Ordinal);
        bool first = true;
        foreach (var s in pre)
        {
            var place = geo.Classify(s.X, s.Z);
            bool alive = postIds.Contains(s.Id);
            if (!first) sb.Append(",\n");
            first = false;
            sb.Append("{\"id\":").Append(Q(s.Id)).Append(",\"x\":").Append(N(s.X)).Append(",\"z\":").Append(N(s.Z)).Append(",\"dbh\":").Append(N(s.Dbh)).Append(",\"h\":").Append(N(s.Height))
              .Append(",\"place\":").Append(Q(place.Kind.ToString())).Append(",\"unit\":").Append(place.UnitIndex).Append(",\"side\":").Append(Q(place.Side.ToString()))
              .Append(",\"footprint\":").Append(geo.Teaching.Contains(s.X, s.Z) ? "true" : "false")
              .Append(",\"along\":").Append(N(geo.Teaching.Along(s.X, s.Z))).Append(",\"across\":").Append(N(geo.Teaching.Across(s.X, s.Z)))
              .Append(",\"rackRemoval\":").Append(alive ? "false" : "true")
              .Append(",\"ciPre\":").Append(N(ciPre[s.Id]))
              .Append(",\"ciPost\":").Append(alive ? N(ciPost[s.Id]) : "null");
            if (alive)
                sb.Append(",\"nb\":[").Append(string.Join(",", neighbours[s.Id].Select(n => "[" + Q(n.Id) + "," + N(n.Term) + "]"))).Append("]");
            sb.Append("}");
        }
        sb.Append("\n],\n");

        // Remainder probes: geometry only (stem positions from the fixture stand; their own rack removals are ignored).
        sb.Append("\"remainderProbes\":[").Append(string.Join(",", RemainderProbes.Select(p =>
        {
            var inputs = p.Z.Select((z, i) => new ScenarioOneRackSampleGeometry.RackInput(i + 1, new RackPoint(EntranceX, z), new RackPoint(FarX, z), RackGeometry.DefaultWidthM)).ToList();
            var g = ScenarioOneRackSampleGeometry.Derive(inputs, StandHalfExtent);
            if (!g.Valid) return "{\"name\":" + Q(p.Name) + ",\"valid\":false,\"problem\":" + Q(g.Problem) + "}";
            int right = 0, left = 0;
            foreach (var s in pre)
            {
                var place = g.Classify(s.X, s.Z);
                if (place.Kind == ScenarioOneRackSampleGeometry.PlacementKind.OuterRemainder) { if (place.Side == ScenarioOneRackSampleGeometry.Side.Right) right++; else left++; }
            }
            return "{\"name\":" + Q(p.Name) + ",\"valid\":true,\"z\":[" + string.Join(",", p.Z.Select(z => N(z))) + "],\"units\":[" + string.Join(",", g.Units.Select(UnitJson))
                + "],\"remainders\":[" + RemainderJson(g.RightRemainder) + "," + RemainderJson(g.LeftRemainder) + "],\"rightStems\":" + right + ",\"leftStems\":" + left + "}";
        }))).Append("]\n");
        sb.Append("}\n");
        File.WriteAllText(args[2], sb.ToString());

        Console.WriteLine("MFM03_EXPORT footprint=" + footprintIds.Count + " ba=" + N(footprintBa) + " sha=" + footprintSha + " rackRemovals=" + claimed.Count + " postLiving=" + post.Count);
        Console.WriteLine("MFM03_EXPORT signature " + geo.Signature());
        foreach (var p in problems) Console.WriteLine("MFM03_EXPORT_FAIL " + p);
        if (problems.Count == 0) Console.WriteLine("MFM03_EXPORT_PASS fixture identity and source parity");
        return problems.Count == 0 ? 0 : 1;
    }

    // Each living stem's current competition index: CompetitionShare.Measure over every other living stem (its CurrentTotal is the
    // same sum the ecology's UpdateCompetition computes, via the same Hegyi term and 8 m cutoff). Neighbour terms are kept for the
    // release indicator in the analysis.
    private static Dictionary<string, double> Competition(List<Stem> living, Dictionary<string, List<(string, double)>> neighbours)
    {
        var ci = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var target in living)
        {
            var samples = new List<CompetitionSample>();
            foreach (var other in living)
            {
                if (ReferenceEquals(other, target)) continue;
                float dx = other.X - target.X, dz = other.Z - target.Z;
                if (dx * dx + dz * dz > 81f) continue; // cheap pre-filter; Measure applies the exact 8 m cutoff
                samples.Add(CompetitionShare.SampleFrom(other.Id, other.X, other.Z, target.X, target.Z, other.Dbh, true, false, false));
            }
            CompetitionShareSet set = CompetitionShare.Measure(target.Dbh, samples);
            ci[target.Id] = set.CurrentTotal;
            if (neighbours != null) neighbours[target.Id] = set.Neighbours.Select(n => (n.Id, n.Contribution)).ToList();
        }
        return ci;
    }

    // The offline ForestEcologyController stand-in must still equal production's Hegyi term and cutoff.
    private static void SourceParity(string root, List<string> problems)
    {
        string source = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ForestEcologyController.cs"));
        if (!source.Contains("public const float HegyiCutoffMeters = 8f;")) problems.Add("production HegyiCutoffMeters is no longer 8f");
        if (!source.Contains("return (neighbourDbhCm / Mathf.Max(1f, targetDbhCm)) / Mathf.Max(0.5f, distanceMeters);")) problems.Add("production HegyiTerm expression changed");
        if (!source.Contains("ci += (diameters[j] / Mathf.Max(1f, targetDiameter)) / Mathf.Max(0.5f, distance);")) problems.Add("production annual-step Hegyi inline copy changed");
        if (ForestEcologyController.HegyiCutoffMeters != 8f) problems.Add("offline stand-in cutoff differs");
    }

    private static List<Stem> ReadStand(string path)
    {
        var list = new List<Stem>();
        foreach (string line in File.ReadAllLines(path).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] c = line.Split(',');
            list.Add(new Stem { Id = c[0], X = float.Parse(c[1], Inv), Z = float.Parse(c[2], Inv), Dbh = float.Parse(c[3], Inv), Height = float.Parse(c[4], Inv) });
        }
        return list;
    }

    private static string UnitJson(ScenarioOneRackSampleGeometry.CoverageUnit u) =>
        "{\"index\":" + u.Index + ",\"rackId\":" + u.RackId + ",\"rackOffset\":" + N(u.RackOffsetM) + ",\"lower\":" + N(u.LowerOffsetM) + ",\"upper\":" + N(u.UpperOffsetM)
        + ",\"lowerOuter\":" + (u.LowerIsOuterEdge ? "true" : "false") + ",\"upperOuter\":" + (u.UpperIsOuterEdge ? "true" : "false")
        + ",\"area\":" + N(u.AreaM2) + ",\"corridorArea\":" + N(u.CorridorAreaM2) + "}";

    private static string RemainderJson(ScenarioOneRackSampleGeometry.OuterRemainder r) =>
        "{\"side\":" + Q(r.Side.ToString()) + ",\"stripEdge\":" + N(r.StripEdgeOffsetM) + ",\"standLimit\":" + N(r.StandLimitOffsetM) + ",\"area\":" + N(r.AreaM2)
        + ",\"empty\":" + (r.IsEmpty ? "true" : "false") + "}";

    private static string Sha16(string text)
    {
        using (var sha = System.Security.Cryptography.SHA256.Create())
            return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2"))).Substring(0, 16);
    }

    private static string N(double v) => v.ToString("R", Inv);
    private static string Q(string s) => "\"" + (s ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"") + "\"";
}
