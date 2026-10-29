using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Cov = ScenarioOneCoverage;
using World = ScenarioOneCoverageWorld;
using Cal = ScenarioOnePrescriptionCalibration;

// Plain .NET checks of MFM-08A, the pure mapping from authoritative world state to ScenarioOneCoverage masks/features
// (ScenarioOneCoverageWorld). The fixture is the MFM-03 frozen Enlarged80 Rack V2 stand used by the MFM-04/07 checks. The Source is
// built from the frozen infrastructure fixture (planks, clearing, buildables, player) exactly as ScenarioOneManager.ReadCoverageWorld
// builds it from the live scene, except that the player is deliberately NOT given to the mapper.
public static class CoverageWorldOfflineCheck
{
    private static int checks, failures;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) failures++;
    }

    private static readonly float[] RackZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private const float EntranceX = -35.4f, FarX = 40f, Half = 40f;
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private static string repoRoot;

    private sealed class Fixture
    {
        public ScenarioOneRackSampleGeometry.Result Geometry;
        public List<Cov.Stem> Stems;
        public List<ScenarioOneRackBay.Order> Orders;
        public World.Source Source;
        public double PlayerX, PlayerZ;
        public List<Cov.Mask> ReferenceMasks;
    }

    public static int Main(string[] args)
    {
        if (args.Length < 3) { Console.Error.WriteLine("usage: <repo-root> <fixture-stand.csv> <fixture-infrastructure.json>"); return 2; }
        repoRoot = args[0];
        Fixture fx = Load(args[1], args[2]);
        if (fx == null) { Console.WriteLine("MFM08A_COVERAGE_WORLD_FAIL fixture"); return 1; }

        InfrastructureMasks(fx);
        ExclusionsChangeCoverage(fx);
        FeatureKinds(fx);
        ProtectedAreaPolygon(fx);
        DeterminismAndNoMutation(fx);
        FailClosed(fx);
        StaticGuards();

        Console.WriteLine((failures == 0 ? "MFM08A_COVERAGE_WORLD_PASS" : "MFM08A_COVERAGE_WORLD_FAIL") + " checks=" + checks + " failed=" + failures);
        return failures == 0 ? 0 : 1;
    }

    // ----------------------------------------------------------------------------------------------------------------------- fixture

    private static Fixture Load(string csv, string infraPath)
    {
        var raw = new List<(string Id, float X, float Z, float Dbh)>();
        foreach (string line in File.ReadAllLines(csv).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] c = line.Split(',');
            raw.Add((c[0], float.Parse(c[1], Inv), float.Parse(c[2], Inv), float.Parse(c[3], Inv)));
        }
        var racks = new List<ScenarioOneRackSampleGeometry.RackInput>();
        for (int i = 0; i < RackZ.Length; i++)
            racks.Add(new ScenarioOneRackSampleGeometry.RackInput(i + 1, new RackPoint(EntranceX, RackZ[i]), new RackPoint(FarX, RackZ[i]), RackGeometry.DefaultWidthM));
        var geometry = ScenarioOneRackSampleGeometry.Derive(racks, Half);
        Check(geometry.Valid && geometry.TeachingRack.Id == 3, "fixture: geometry valid, teaching rack 3", geometry.Problem);
        if (!geometry.Valid) return null;
        var claimed = new HashSet<string>(StringComparer.Ordinal);
        foreach (var rack in racks)
        {
            var input = raw.Select(s => new RackTree(s.Id, s.X, s.Z, true, false, claimed.Contains(s.Id))).ToList();
            foreach (RackTree t in RackGeometry.Select(input, rack.Entrance, rack.Far, rack.WidthM).Trees) claimed.Add(t.Id);
        }
        var fx = new Fixture
        {
            Geometry = geometry,
            Stems = raw.Select(s => new Cov.Stem(s.Id, s.X, s.Z, s.Dbh, !claimed.Contains(s.Id), Cov.SitkaSpeciesId, SampleCrops.Contains(s.Id))).ToList(),
            Orders = claimed.OrderBy(id => id, StringComparer.Ordinal)
                .Select(id => new ScenarioOneRackBay.Order(id, false, true, ScenarioOneRackBay.Disposition.Sold, 0)).ToList()
        };

        string text = File.ReadAllText(Path.Combine(repoRoot, "Assets", "ForestPrototype", "ScenarioOne", "ScenarioOneHeadlandGeometry.cs"));
        double C(string name)
        {
            var m = System.Text.RegularExpressions.Regex.Match(text, "const float " + name + " = (-?[0-9.]+)f;");
            if (!m.Success) throw new InvalidOperationException("headland constant " + name);
            return double.Parse(m.Groups[1].Value, Inv);
        }
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(infraPath));
        System.Text.Json.JsonElement infra = doc.RootElement;
        double rPath = infra.GetProperty("pathCorridorRadiusM").GetDouble(), rClear = infra.GetProperty("clearingRadiusM").GetDouble();
        var src = new World.Source
        {
            PathCorridorRadiusM = rPath, ClearanceRadiusM = rClear, StandHalfExtentM = Half,
            Headland = new World.Headland
            {
                HeadlandX = C("HeadlandX"), SouthZ = C("SouthZ"), NorthZ = C("NorthZ"), FeederZ = C("FeederZ"), PathX = C("PathX"),
                Width = C("Width"), PadRadius = C("PadRadius"), StemGuard = C("StemGuard")
            }
        };
        foreach (var p in infra.GetProperty("scenePlanks").EnumerateArray())
            src.PathPlanks.Add(new World.Point("plank", (float)p.GetProperty("x").GetDouble(), (float)p.GetProperty("z").GetDouble()));
        foreach (var p in infra.GetProperty("approachExtension").EnumerateArray())
        {
            double z = p.GetProperty("z").GetDouble();
            if (z >= -Half - rPath && z <= Half + rPath) src.PathPlanks.Add(new World.Point("approach", (float)p.GetProperty("x").GetDouble(), (float)z));
        }
        var cl = infra.GetProperty("clearing");
        src.HasClearing = true;
        src.Clearing = new World.Point("Forest Clearing", (float)cl.GetProperty("x").GetDouble(), (float)cl.GetProperty("z").GetDouble());
        foreach (var b in infra.GetProperty("buildables").EnumerateArray())
            src.Sites.Add(new World.Point("buildable", (float)b.GetProperty("x").GetDouble(), (float)b.GetProperty("z").GetDouble()));
        var pl = infra.GetProperty("player");
        fx.PlayerX = pl.GetProperty("x").GetDouble();
        fx.PlayerZ = pl.GetProperty("z").GetDouble();
        fx.Source = src;
        Check(src.PathPlanks.Count > 20 && src.Sites.Count > 0, "fixture: planks and buildables present", src.PathPlanks.Count + "/" + src.Sites.Count);

        // The reference set: the accepted MFM-03/04 mask construction, with the Clearing radius the Manager chose (the clearance setting).
        var refs = new List<Cov.Mask>();
        foreach (World.Point p in src.PathPlanks) refs.Add(Cov.Mask.Circle(Cov.MaskKind.Track, p.X, p.Z, rPath));
        refs.Add(Cov.Mask.Circle(Cov.MaskKind.Clearing, src.Clearing.X, src.Clearing.Z, rClear));
        foreach (World.Point p in src.Sites) refs.Add(Cov.Mask.Circle(Cov.MaskKind.Site, p.X, p.Z, rClear));
        double strip = src.Headland.Width * 0.5 + src.Headland.StemGuard, pad = src.Headland.PadRadius + src.Headland.StemGuard;
        refs.Add(new Cov.Mask(Cov.MaskKind.Headland, src.Headland.HeadlandX, src.Headland.SouthZ, src.Headland.HeadlandX, src.Headland.NorthZ, strip));
        refs.Add(new Cov.Mask(Cov.MaskKind.Headland, src.Headland.HeadlandX, src.Headland.FeederZ, src.Headland.PathX, src.Headland.FeederZ, strip));
        refs.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, src.Headland.HeadlandX, src.Headland.FeederZ, pad));
        refs.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, src.Headland.PathX, src.Headland.FeederZ, pad));
        fx.ReferenceMasks = refs;
        return fx;
    }

    private static Cov.Result Classify(Fixture fx, IReadOnlyList<Cov.Mask> masks, IReadOnlyList<Cov.Feature> features = null, IReadOnlyCollection<string> manual = null)
        => Cov.Classify(new Cov.Input
        {
            Geometry = fx.Geometry, Stems = fx.Stems, Orders = fx.Orders, Masks = masks ?? new Cov.Mask[0],
            Features = features ?? new Cov.Feature[0], PlayerManualAreas = manual ?? new string[0]
        }, Cal.V1);

    private static World.Mapped Map(World.Source s)
    {
        World.Mapped m = World.Map(s, out string problem);
        Check(m != null, "map succeeds", problem);
        return m;
    }

    private static string Describe(Cov.Mask m) => m.Kind + ":" + m.AX.ToString("R", Inv) + "," + m.AZ.ToString("R", Inv) + "," + m.BX.ToString("R", Inv) + "," + m.BZ.ToString("R", Inv) + "," + m.RadiusM.ToString("R", Inv);

    // ------------------------------------------------------------------------------------------------------------------------ masks

    private static void InfrastructureMasks(Fixture fx)
    {
        World.Mapped mapped = Map(fx.Source);
        var kinds = mapped.Masks.GroupBy(m => m.Kind).ToDictionary(g => g.Key, g => g.Count());
        foreach (Cov.MaskKind kind in Enum.GetValues(typeof(Cov.MaskKind)))
            Check(kinds.ContainsKey(kind) && kinds[kind] > 0, "mask kind mapped: " + kind, kinds.ContainsKey(kind) ? kinds[kind].ToString() : "0");
        Check(kinds[Cov.MaskKind.Track] == fx.Source.PathPlanks.Count, "one Track circle per Dirt Path plank", kinds[Cov.MaskKind.Track].ToString());
        Check(kinds[Cov.MaskKind.Clearing] == 1, "one Clearing circle", kinds[Cov.MaskKind.Clearing].ToString());
        Check(kinds[Cov.MaskKind.Site] == fx.Source.Sites.Count, "one Site circle per buildable", kinds[Cov.MaskKind.Site].ToString());
        Check(kinds[Cov.MaskKind.Headland] == 4, "headland: strip, feeder strip, two pads", kinds[Cov.MaskKind.Headland].ToString());
        Check(mapped.Masks.Where(m => m.Kind == Cov.MaskKind.Track).All(m => m.RadiusM == fx.Source.PathCorridorRadiusM), "Track radius is the path corridor setting");
        Check(mapped.Masks.Where(m => m.Kind == Cov.MaskKind.Clearing || m.Kind == Cov.MaskKind.Site).All(m => m.RadiusM == fx.Source.ClearanceRadiusM),
            "Clearing and Site radius is the clearance setting");
        Check(!mapped.Masks.Any(m => m.Kind == Cov.MaskKind.Site && m.AX == fx.PlayerX && m.AZ == fx.PlayerZ && m.BX == m.AX && m.BZ == m.AZ)
              || fx.Source.Sites.Any(s => s.X == fx.PlayerX && s.Z == fx.PlayerZ),
            "no Site mask at the player's position");
        var mine = new SortedSet<string>(mapped.Masks.Select(Describe), StringComparer.Ordinal);
        var reference = new SortedSet<string>(fx.ReferenceMasks.Select(Describe), StringComparer.Ordinal);
        Check(mine.SetEquals(reference), "mapped masks equal the accepted reference construction", mine.Count + "/" + reference.Count);

        // The player's start / live position is not a mapper input: moving it cannot change the result (no player field exists at all).
        string json = string.Join("|", typeof(World.Source).GetFields().Select(f => f.Name));
        Check(!json.Contains("Player"), "Source has no player field", json);
    }

    private static void ExclusionsChangeCoverage(Fixture fx)
    {
        World.Mapped mapped = Map(fx.Source);
        Cov.Result empty = Classify(fx, null);
        Cov.Result full = Classify(fx, mapped.Masks);
        Check(empty.Valid && full.Valid, "coverage valid with and without masks", empty.Problem + full.Problem);
        Check(empty.Signature() != full.Signature(), "production masks change the coverage result vs the old empty arrays");
        Check(full.Source.NetAreaM2 < empty.Source.NetAreaM2 || full.Areas.Zip(empty.Areas, (a, b) => a.Metrics.NetAreaM2 < b.Metrics.NetAreaM2).Any(x => x),
            "masked net area is smaller somewhere");
        Check(full.Signature() == Classify(fx, fx.ReferenceMasks).Signature(), "mapped coverage signature equals the reference-mask coverage");

        // Removing or moving one authoritative source changes the adapter result and the coverage.
        World.Source minus = Clone(fx.Source);
        minus.PathPlanks.Clear();
        World.Mapped without = Map(minus);
        Check(without.Masks.Count == mapped.Masks.Count - fx.Source.PathPlanks.Count, "removing the planks removes exactly the Track masks");
        Check(Classify(fx, without.Masks).Signature() != full.Signature(), "removing the path changes coverage");
        World.Source moved = Clone(fx.Source);
        moved.Clearing = new World.Point("Forest Clearing", fx.Source.Clearing.X + 6f, fx.Source.Clearing.Z);
        World.Mapped movedMap = Map(moved);
        Check(!new SortedSet<string>(movedMap.Masks.Select(Describe)).SetEquals(new SortedSet<string>(mapped.Masks.Select(Describe))), "moving the clearing changes the masks");
        World.Source noClearing = Clone(fx.Source);
        noClearing.HasClearing = false;
        Check(Map(noClearing).Masks.Count == mapped.Masks.Count - 1, "no Forest Clearing means no Clearing mask (not an invented one)");
    }

    // --------------------------------------------------------------------------------------------------------------------- features

    private static (Cov.Area Area, Cov.Stem Stem) PickArea(Cov.Result result, Fixture fx, int skip)
    {
        var covered = result.Areas.Where(a => !a.IsSource && a.State == Cov.State.Covered && a.StemIds.Count > 0).ToList();
        Cov.Area area = covered[skip];
        Cov.Stem stem = fx.Stems.First(s => s.Id == area.StemIds[0]);
        return (area, stem);
    }

    private static void FeatureKinds(Fixture fx)
    {
        World.Mapped baseMap = Map(fx.Source);
        Cov.Result baseline = Classify(fx, baseMap.Masks);
        var a = PickArea(baseline, fx, 0);
        var b = PickArea(baseline, fx, 1);
        Check(a.Area.Id != b.Area.Id, "two distinct covered areas for the feature fixtures", a.Area.Id + "/" + b.Area.Id);

        void Kind(string name, Cov.FeatureKind kind, Action<World.Source, World.Point> put, Cov.Reason expected)
        {
            World.Source s = Clone(fx.Source);
            put(s, new World.Point(name + "-1", a.Stem.X, a.Stem.Z));
            World.Mapped m = Map(s);
            Check(m.Features.Count == 1 && m.Features[0].Kind == kind && m.Features[0].Id == name + "-1", name + ": one feature of the right kind and the authoritative id");
            Cov.Result r = Classify(fx, m.Masks, m.Features);
            Cov.Area got = r.Find(a.Area.Id);
            Check(got.State == Cov.State.Excluded && got.Reasons.Contains(expected), name + ": area becomes Excluded with the exact reason", got.State + "/" + string.Join(",", got.Reasons));
            Check(got.Evidence.Contains(name + "-1"), name + ": evidence carries the authoritative id");
            Check(r.Find(b.Area.Id).State == baseline.Find(b.Area.Id).State, name + ": other areas unchanged");
            // Moving the source to the other area moves the exclusion with it (production is not passing empty arrays).
            World.Source s2 = Clone(fx.Source);
            put(s2, new World.Point(name + "-1", b.Stem.X, b.Stem.Z));
            Cov.Result r2 = Classify(fx, Map(s2).Masks, Map(s2).Features);
            Check(r2.Find(a.Area.Id).State == baseline.Find(a.Area.Id).State && r2.Find(b.Area.Id).State == Cov.State.Excluded, name + ": moving the source moves the exclusion");
        }
        Kind("juv", Cov.FeatureKind.PlantedJuvenile, (s, p) => s.PlantedJuveniles.Add(p), Cov.Reason.PlantedJuvenile);
        Kind("shelter", Cov.FeatureKind.Shelter, (s, p) => s.Shelters.Add(p), Cov.Reason.Shelter);
        Kind("clearance", Cov.FeatureKind.ClearancePatch, (s, p) => s.ClearancePatches.Add(p), Cov.Reason.ClearancePatch);
        Kind("deadwood", Cov.FeatureKind.DeadwoodRecord, (s, p) => s.DeadwoodRecords.Add(p), Cov.Reason.DeadwoodRecord);
        Kind("order", Cov.FeatureKind.ActiveManagementWork, (s, p) => s.ActiveManagementWork.Add(p), Cov.Reason.ActiveManagementWork);

        // Natural regeneration flags but does not exclude.
        World.Source n = Clone(fx.Source);
        n.NaturalRegeneration.Add(new World.Point("regen-cell-7", a.Stem.X, a.Stem.Z));
        World.Mapped nm = Map(n);
        Cov.Area na = Classify(fx, nm.Masks, nm.Features).Find(a.Area.Id);
        Check(na.State == Cov.State.Covered && na.Flags.Contains(Cov.Flag.NaturalRegeneration) && na.Evidence.Contains("regen-cell-7"),
            "NaturalRegeneration: stays Covered, flagged, evidence id kept", na.State + "/" + string.Join(",", na.Flags));
    }

    private static void ProtectedAreaPolygon(Fixture fx)
    {
        World.Mapped baseMap = Map(fx.Source);
        Cov.Result baseline = Classify(fx, baseMap.Masks);
        var a = PickArea(baseline, fx, 0);
        var b = PickArea(baseline, fx, 1);
        World.Polygon Square(string id, double cx, double cz, double r)
        {
            var p = new World.Polygon { Id = id };
            p.Vertices.Add(new World.Point(id, (float)(cx - r), (float)(cz - r)));
            p.Vertices.Add(new World.Point(id, (float)(cx + r), (float)(cz - r)));
            p.Vertices.Add(new World.Point(id, (float)(cx + r), (float)(cz + r)));
            p.Vertices.Add(new World.Point(id, (float)(cx - r), (float)(cz + r)));
            return p;
        }
        World.Source s = Clone(fx.Source);
        s.ProtectedAreas.Add(Square("fence-1", a.Stem.X, a.Stem.Z, 3.0));
        World.Mapped m = Map(s);
        Check(m.Features.Count > 4 && m.Features.All(f => f.Kind == Cov.FeatureKind.ProtectedArea && f.Id == "fence-1"), "protected area: sampled points all carry the area id", m.Features.Count.ToString());
        Cov.Result r = Classify(fx, m.Masks, m.Features);
        Cov.Area got = r.Find(a.Area.Id);
        Check(got.State == Cov.State.Excluded && got.Reasons.Contains(Cov.Reason.ProtectedArea) && got.Evidence.Contains("fence-1"), "protected area excludes the area it covers", got.State.ToString());
        Check(r.Find(b.Area.Id).State == baseline.Find(b.Area.Id).State, "protected area leaves a distant area alone");
        // Sampling is clipped to the stand.
        World.Source far = Clone(fx.Source);
        far.ProtectedAreas.Add(Square("fence-out", 200, 200, 5));
        Check(Map(far).Features.Count == 0, "a protected area wholly outside the stand adds no feature");
        // A sliver thinner than the lattice step is still caught through its edges.
        World.Polygon sliver = new World.Polygon { Id = "sliver" };
        sliver.Vertices.Add(new World.Point("sliver", (float)a.Stem.X, (float)a.Stem.Z));
        sliver.Vertices.Add(new World.Point("sliver", (float)a.Stem.X + 5f, (float)a.Stem.Z));
        sliver.Vertices.Add(new World.Point("sliver", (float)a.Stem.X + 5f, (float)a.Stem.Z + 0.1f));
        World.Source sl = Clone(fx.Source);
        sl.ProtectedAreas.Add(sliver);
        World.Mapped sm = Map(sl);
        Check(Classify(fx, sm.Masks, sm.Features).Find(a.Area.Id).State == Cov.State.Excluded, "a thin protected polygon still excludes through its edge points");
    }

    // ----------------------------------------------------------------------------------------------- determinism, mutation, refusal

    private static void DeterminismAndNoMutation(Fixture fx)
    {
        World.Source s = Clone(fx.Source);
        var baseline = Classify(fx, Map(fx.Source).Masks);
        var a = PickArea(baseline, fx, 0);
        s.PlantedJuveniles.Add(new World.Point("j2", a.Stem.X, a.Stem.Z));
        s.PlantedJuveniles.Add(new World.Point("j1", a.Stem.X + 0.5f, a.Stem.Z));
        s.NaturalRegeneration.Add(new World.Point("regen-cell-1", a.Stem.X, a.Stem.Z));
        string before = Snapshot(s);
        World.Mapped m1 = Map(s), m2 = Map(s);
        Check(Snapshot(s) == before, "mapping does not mutate the source");
        Check(string.Join(";", m1.Masks.Select(Describe)) == string.Join(";", m2.Masks.Select(Describe))
              && string.Join(";", m1.Features.Select(f => f.Kind + f.Id + f.X + f.Z)) == string.Join(";", m2.Features.Select(f => f.Kind + f.Id + f.X + f.Z)), "mapping twice gives identical output");
        World.Source rev = Clone(s);
        rev.PlantedJuveniles.Reverse();
        rev.PathPlanks.Reverse();
        rev.Sites.Reverse();
        Check(Classify(fx, Map(rev).Masks, Map(rev).Features).Signature() == Classify(fx, m1.Masks, m1.Features).Signature(), "input order cannot change coverage");
        Check(Classify(fx, m1.Masks, m1.Features).Signature() == Classify(fx, m1.Masks, m1.Features).Signature(), "classifying twice is identical (no hidden state)");
    }

    private static void FailClosed(Fixture fx)
    {
        World.Source s = Clone(fx.Source);
        s.PathCorridorRadiusM = 0;
        Check(World.Map(s, out string p1) == null && p1.Length > 0, "zero path radius is refused, not guessed", p1);
        s = Clone(fx.Source);
        s.ClearanceRadiusM = 0;
        Check(World.Map(s, out string p2) == null && p2.Length > 0, "zero clearance radius is refused, not guessed", p2);
        s = Clone(fx.Source);
        s.Headland = null;
        Check(World.Map(s, out string p3) == null && p3.Length > 0, "missing headland is refused", p3);
        Check(World.Map(null, out string p4) == null && p4.Length > 0, "null source is refused", p4);
    }

    // ---------------------------------------------------------------------------------------------------------------- static guards

    private static void StaticGuards()
    {
        string so = Path.Combine(repoRoot, "Assets", "ForestPrototype", "ScenarioOne");
        string approval = File.ReadAllText(Path.Combine(so, "ScenarioOneManager.PrescriptionApproval.cs"));
        string world = File.ReadAllText(Path.Combine(so, "ScenarioOneCoverageWorld.cs"));
        Check(!approval.Contains("new ScenarioOneCoverage.Mask[0]") && !approval.Contains("new ScenarioOneCoverage.Feature[0]"), "production no longer passes empty mask/feature arrays");
        Check(approval.Contains("Masks = mapped.Masks") && approval.Contains("Features = mapped.Features"), "production passes the mapped masks and features");
        int start = approval.IndexOf("private ScenarioOneCoverageWorld.Mapped ReadCoverageWorld", StringComparison.Ordinal);
        int end = approval.IndexOf("private static int CompareAt", StringComparison.Ordinal);
        string reader = start >= 0 && end > start ? approval.Substring(start, end - start) : "";
        Check(reader.Length > 0, "ReadCoverageWorld located");
        Check(reader.Contains("PathCorridorRadiusMeters") && reader.Contains("ClearingRadiusMeters"), "radii are read through the authoritative accessors");
        Check(!reader.Contains("ForestPlayer"), "the reader never touches the player");
        Check(!approval.Contains("BindingFlags") && !approval.Contains("GetField("), "no reflection in the adapter");
        string[] writes = { ".Add(new ScenarioOneWorkOrder", "workOrders.Add", "MarkForFell", "managementEvents.Add", "plantedJuveniles.Add", "deadwoodRecords.Add", "clearancePatches.Add", "shelters.Add" };
        Check(writes.All(w => !reader.Contains(w)), "the reader writes no order, mark, event or record");
        Check(!world.Contains("using UnityEngine"), "the pure mapper has no UnityEngine dependency");
        string stand = File.ReadAllText(Path.Combine(repoRoot, "Assets", "ForestPrototype", "ForestStartingStand.cs"));
        Check(stand.Contains("public float PathCorridorRadiusMeters => pathCorridorRadiusMeters;") && stand.Contains("public float ClearingRadiusMeters => clearingRadiusMeters;"),
            "accessors are read-only expression-bodied views of the existing fields");
        Check(stand.Contains("[SerializeField, Min(0.5f)] private float pathCorridorRadiusMeters = 1.5f;") && stand.Contains("[SerializeField, Min(1f)] private float clearingRadiusMeters = 2.2f;"),
            "the clearance settings themselves are unchanged");
    }

    // ------------------------------------------------------------------------------------------------------------------------ helpers

    private static World.Source Clone(World.Source s)
    {
        var c = new World.Source
        {
            PathCorridorRadiusM = s.PathCorridorRadiusM, ClearanceRadiusM = s.ClearanceRadiusM, StandHalfExtentM = s.StandHalfExtentM,
            Headland = s.Headland, HasClearing = s.HasClearing, Clearing = s.Clearing
        };
        c.PathPlanks.AddRange(s.PathPlanks); c.Sites.AddRange(s.Sites); c.PlantedJuveniles.AddRange(s.PlantedJuveniles);
        c.Shelters.AddRange(s.Shelters); c.ClearancePatches.AddRange(s.ClearancePatches); c.ProtectedAreas.AddRange(s.ProtectedAreas);
        c.DeadwoodRecords.AddRange(s.DeadwoodRecords); c.ActiveManagementWork.AddRange(s.ActiveManagementWork); c.NaturalRegeneration.AddRange(s.NaturalRegeneration);
        return c;
    }

    private static string Snapshot(World.Source s)
        => s.PathPlanks.Count + "|" + s.Sites.Count + "|" + s.PlantedJuveniles.Count + "|" + string.Join(",", s.PlantedJuveniles.Select(p => p.Id)) + "|" + s.NaturalRegeneration.Count
           + "|" + s.PathCorridorRadiusM + "|" + s.ClearanceRadiusM;
}
