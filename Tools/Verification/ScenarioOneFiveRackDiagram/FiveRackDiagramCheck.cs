// Offline check of the Objective 1 illustrative five-rack figure (TeachingDiagramPlan.ObjectiveOneFigure, D-072 FR-T, issue #296).
// Plain .NET, no Unity. Built and run by run_five_rack_check.py, which parses the authority constants and the scene planks and passes them in:
//
//   FiveRackDiagramCheck <inputs.txt>        structural/source-fidelity checks; prints FIVE_RACK_PASS or FIVE_RACK_FAIL
//   FiveRackDiagramCheck <inputs.txt> dump   canonical dump of the figure (determinism and review)
//
// This proves what the figure DRAWS and where its numbers come from. It does not prove that it renders, fits, reads well in grayscale or is
// understood: those need Unity captures at 1280x720 and 1920x1080 and independent reviewers.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public static class FiveRackDiagramCheck
{
    private static int checks, failures;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static Dictionary<string, string> input = new Dictionary<string, string>();

    public static int Main(string[] args)
    {
        foreach (string line in File.ReadAllLines(args[0]))
        {
            int eq = line.IndexOf('=');
            if (eq > 0) input[line.Substring(0, eq)] = line.Substring(eq + 1);
        }
        TeachingDiagramPlan.Site site = SiteFromInputs();
        if (args.Length > 1 && args[1] == "dump") { foreach (string l in Dump(TeachingDiagramPlan.ObjectiveOneFigure(site))) Console.WriteLine(l); return 0; }

        var stage = TeachingDiagramPlan.ObjectiveOneFigure(site);
        FixtureIdentity(site);
        Corridors(site, stage);
        Infrastructure(site, stage);
        ApproachMatchesScene(site);
        RetainedBands(site, stage);
        StateKeyAndSemantics(stage);
        WordsAndLayout(site, stage);
        Determinism(site, stage);
        Console.WriteLine(failures == 0 ? $"FIVE_RACK_PASS checks={checks}" : $"FIVE_RACK_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string what, string detail = "")
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + what + (detail.Length > 0 ? " :: " + detail : ""));
        if (!ok) failures++;
    }

    private static float F(string key) => float.Parse(input[key], Inv);

    // The Site exactly as TeachingDiagram.SiteFromAuthority builds it, from constants the runner parsed out of the authority sources.
    private static TeachingDiagramPlan.Site SiteFromInputs() => new TeachingDiagramPlan.Site
    {
        StandSizeM = F("StandSizeEnlarged80"), HeadlandX = F("HeadlandX"), HeadlandSouthZ = F("SouthZ"), HeadlandNorthZ = F("NorthZ"),
        FeederZ = F("FeederZ"), FeederEndX = F("PathX"), PathWidthM = F("Width"), PadRadiusM = F("PadRadius"), RackWidthM = F("DefaultWidthM"),
    };

    private static float WorldZ(TeachingDiagramPlan.Site s, float y) => s.StandSizeM * 0.5f - (y - TeachingDiagramPlan.FigureMapTop) / TeachingDiagramPlan.FigurePxPerM;
    private static float WorldX(TeachingDiagramPlan.Site s, float x) => (x - TeachingDiagramPlan.FigureMapLeft) / TeachingDiagramPlan.FigurePxPerM - s.StandSizeM * 0.5f;
    private static bool OnMap(TeachingDiagramPlan.Item i) => !i.Id.StartsWith("key-", StringComparison.Ordinal);

    private static void FixtureIdentity(TeachingDiagramPlan.Site site)
    {
        string[] sourceIds = input["SourceEntranceIds"].Split(',');
        Check(TeachingDiagramPlan.FixtureEntranceIds.SequenceEqual(sourceIds), "fixture entrance identities equal ScenarioOneSuggestedRackLayout.EntranceTreeIds, in order",
            string.Join(",", TeachingDiagramPlan.FixtureEntranceIds));
        Check(TeachingDiagramPlan.FixtureEntranceIds.Distinct().Count() == 5 && TeachingDiagramPlan.FixtureEntranceZ.Length == 5, "exactly five unique entrance identities and five Z values");
        Check(TeachingDiagramPlan.FixtureSnapshotSha256 == input["ExpectedSnapshotSha256"], "fixture snapshot hash is the recorded authentic Enlarged80 Rack V2 world");
        if (input.TryGetValue("SnapshotZ", out string snapshot) && snapshot != "NOT_RUN")
        {
            // The runner read each entrance tree's saved z (a double in JSON); Unity loads it into a float, as the layout does.
            float[] saved = snapshot.Split(',').Select(v => (float)double.Parse(v, Inv)).ToArray();
            for (int i = 0; i < 5; i++)
                Check(saved[i] == TeachingDiagramPlan.FixtureEntranceZ[i], "entrance " + TeachingDiagramPlan.FixtureEntranceIds[i] + " Z equals the snapshot tree's own Z (float)",
                    saved[i].ToString("R", Inv) + " vs " + TeachingDiagramPlan.FixtureEntranceZ[i].ToString("R", Inv));
        }
        else Console.WriteLine("NOT RUN snapshot re-read (archive absent); fixture compared with recorded values only");
        for (int i = 0; i < 5; i++)
            Check(TeachingDiagramPlan.FixtureEntranceZ[i] >= site.HeadlandSouthZ && TeachingDiagramPlan.FixtureEntranceZ[i] <= site.HeadlandNorthZ
                && (i == 0 || TeachingDiagramPlan.FixtureEntranceZ[i] > TeachingDiagramPlan.FixtureEntranceZ[i - 1]),
                "entrance " + (i + 1) + " lies on the headland, south to north (the layout's own validation)");
    }

    private static void Corridors(TeachingDiagramPlan.Site site, TeachingDiagramPlan.Stage stage)
    {
        var corridors = stage.Items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Corridor && OnMap(i)).ToList();
        Check(corridors.Count == 5, "exactly five rack corridors on the map (no sixth)", corridors.Count.ToString(Inv));
        Check(corridors.All(c => c.State == TeachingDiagramPlan.CorridorState.Planned), "every map corridor is planned (solid outline), none draft or harvested");
        Check(corridors.Select(c => c.SourceId).SequenceEqual(TeachingDiagramPlan.FixtureEntranceIds), "corridor identities are the five source entrances, south to north");
        float half = site.StandSizeM * 0.5f;
        for (int i = 0; i < corridors.Count; i++)
        {
            var c = corridors[i];
            float z = TeachingDiagramPlan.FixtureEntranceZ[i];
            // Layout algebra (ScenarioOneSuggestedRackLayout.Get): Entrance = (HeadlandX, z), Far = (StandSize / 2, z), Width = DefaultWidthM.
            Check(Math.Abs(WorldX(site, c.X) - site.HeadlandX) < 1e-3f && Math.Abs(WorldX(site, c.X1) - half) < 1e-3f,
                "rack " + (i + 1) + " runs from the headland entrance to the east edge", WorldX(site, c.X).ToString("0.###", Inv) + " -> " + WorldX(site, c.X1).ToString("0.###", Inv));
            Check(Math.Abs(WorldZ(site, c.Y) - z) < 1e-3f && Math.Abs(WorldZ(site, c.Y1) - z) < 1e-3f, "rack " + (i + 1) + " centreline at its entrance tree's Z (inverse transform)",
                WorldZ(site, c.Y).ToString("0.####", Inv));
            double degrees = Math.Atan2(c.Y1 - c.Y, c.X1 - c.X) * 180.0 / Math.PI;
            Check(Math.Abs(degrees) <= 1.0, "rack " + (i + 1) + " direction within 1 degree of due east (parallel)", degrees.ToString("0.###", Inv));
            Check(Math.Abs(c.Width - site.RackWidthM * TeachingDiagramPlan.FigurePxPerM) < 1e-3f, "rack " + (i + 1) + " width is RackGeometry.DefaultWidthM at the figure scale");
        }
        var gaps = new List<double>();
        for (int i = 1; i < corridors.Count; i++) gaps.Add(WorldZ(site, corridors[i].Y) - WorldZ(site, corridors[i - 1].Y));
        string shown = string.Join(", ", gaps.Select(g => g.ToString("0.000", Inv)));
        double[] expected = { 18.849411, 16.660000, 15.555294, 19.438824 };
        Check(gaps.Count == 4 && gaps.Zip(expected, (g, e) => Math.Abs(g - e) < 0.002).All(x => x), "centreline gaps are the real nonuniform source gaps", shown);
        Check(gaps.Count == 4 && gaps.Max() - gaps.Min() > 3.0, "spacing is not regularised", shown);
        var entrances = stage.Items.Where(i => i.Kind == TeachingDiagramPlan.Kind.Entrance && OnMap(i)).ToList();
        Check(entrances.Count == 5, "exactly five entrance squares on the map", entrances.Count.ToString(Inv));
        for (int i = 0; i < Math.Min(5, entrances.Count); i++)
            Check(entrances[i].SourceId == TeachingDiagramPlan.FixtureEntranceIds[i] && Math.Abs(WorldX(site, entrances[i].X) - site.HeadlandX) < 1e-3f
                && Math.Abs(WorldZ(site, entrances[i].Y) - TeachingDiagramPlan.FixtureEntranceZ[i]) < 1e-3f, "entrance " + (i + 1) + " square sits on the headland at its rack");
    }

    private static void Infrastructure(TeachingDiagramPlan.Site site, TeachingDiagramPlan.Stage stage)
    {
        var map = stage.Items.Where(OnMap).ToList();
        var tracks = map.Where(i => i.Kind == TeachingDiagramPlan.Kind.Track).ToList();
        var pads = map.Where(i => i.Kind == TeachingDiagramPlan.Kind.Pad).ToList();
        Check(tracks.Count == 6 && tracks.Count(t => t.Role == TeachingDiagramPlan.Role.TrackCasing) == 3, "three existing surfaces (headland, feeder, approach), each with casing and fill; nothing else",
            string.Join(",", tracks.Select(t => t.Id)));
        Check(pads.Count == 4, "exactly the two authored junction pads (casing + fill each)", pads.Count.ToString(Inv));
        var headland = tracks.FirstOrDefault(t => t.Id == "existing-headland");
        var feeder = tracks.FirstOrDefault(t => t.Id == "existing-feeder");
        var approach = tracks.FirstOrDefault(t => t.Id == "existing-approach");
        Check(headland != null && headland.Points.Length == 4 && Math.Abs(WorldX(site, headland.Points[0]) - site.HeadlandX) < 1e-3f
            && Math.Abs(WorldX(site, headland.Points[2]) - site.HeadlandX) < 1e-3f && Math.Abs(WorldZ(site, headland.Points[1]) - site.HeadlandSouthZ) < 1e-3f
            && Math.Abs(WorldZ(site, headland.Points[3]) - site.HeadlandNorthZ) < 1e-3f, "western headland is (HeadlandX, SouthZ) -> (HeadlandX, NorthZ) from ScenarioOneHeadlandGeometry");
        Check(feeder != null && feeder.Points.Length == 4 && Math.Abs(WorldX(site, feeder.Points[0]) - site.HeadlandX) < 1e-3f
            && Math.Abs(WorldX(site, feeder.Points[2]) - site.FeederEndX) < 1e-3f && Math.Abs(WorldZ(site, feeder.Points[1]) - site.FeederZ) < 1e-3f
            && Math.Abs(WorldZ(site, feeder.Points[3]) - site.FeederZ) < 1e-3f, "single southern feeder is (HeadlandX, FeederZ) -> (PathX, FeederZ) from ScenarioOneHeadlandGeometry");
        Check(tracks.All(t => Math.Abs(t.Width - site.PathWidthM * TeachingDiagramPlan.FigurePxPerM - (t.Role == TeachingDiagramPlan.Role.TrackCasing ? TeachingDiagramPlan.CasingExtra : 0f)) < 1e-3f),
            "track fills are the authored 2.6 m width at figure scale");
        Check(pads.Where(p => p.Role == TeachingDiagramPlan.Role.Track).All(p => Math.Abs(p.Radius - site.PadRadiusM * TeachingDiagramPlan.FigurePxPerM) < 1e-3f
            && Math.Abs(WorldZ(site, p.Y) - site.FeederZ) < 1e-3f) && pads.Select(p => Math.Round(WorldX(site, p.X), 3)).Distinct().OrderBy(x => x)
            .SequenceEqual(new[] { Math.Round((double)site.HeadlandX, 3), Math.Round((double)site.FeederEndX, 3) }.OrderBy(x => x)), "pads at both feeder ends with the authored radius");
        float[] z = TeachingDiagramPlan.FixtureEntranceZ;
        Check(site.FeederZ > z[0] && site.FeederZ < z[1], "the feeder joins the headland between the first and second entrances",
            z[0].ToString("0.##", Inv) + " < " + site.FeederZ.ToString("0.##", Inv) + " < " + z[1].ToString("0.##", Inv));
        Check(site.FeederZ - (-site.StandSizeM * 0.5f) > 10f, "the feeder is not drawn along the southern property edge");
        // Draw order: every existing-surface casing before every fill, all above the corridors, entrances on top.
        int lastCasing = stage.Items.FindLastIndex(i => OnMap(i) && i.Role == TeachingDiagramPlan.Role.TrackCasing);
        int firstFill = stage.Items.FindIndex(i => OnMap(i) && (i.Kind == TeachingDiagramPlan.Kind.Track || i.Kind == TeachingDiagramPlan.Kind.Pad) && i.Role == TeachingDiagramPlan.Role.Track);
        Check(lastCasing >= 0 && lastCasing < firstFill, "all existing-surface casings are drawn before any fill (no dark edge across a junction)");
        Check(stage.Items.FindLastIndex(i => OnMap(i) && i.Kind == TeachingDiagramPlan.Kind.Corridor) < stage.Items.FindIndex(i => OnMap(i) && i.Kind == TeachingDiagramPlan.Kind.Track)
            && stage.Items.FindIndex(i => OnMap(i) && i.Kind == TeachingDiagramPlan.Kind.Entrance) > stage.Items.FindLastIndex(i => OnMap(i) && i.Kind == TeachingDiagramPlan.Kind.Track),
            "draw order: corridors, then existing surfaces, then entrance squares");
        // No invented route: no other path north, no spine, no diagonal branch.
        Check(approach != null && Enumerable.Range(0, approach.Points.Length / 2).All(k => WorldZ(site, approach.Points[2 * k + 1]) <= TeachingDiagramPlan.FixtureClearingZ + 1e-3f),
            "the approach ends at the clearing; no northern spine");
        Check(map.Count(i => i.Kind == TeachingDiagramPlan.Kind.Corridor) == 5 && map.Where(i => i.Kind == TeachingDiagramPlan.Kind.Corridor).All(c => c.Y == c.Y1), "no diagonal or branch corridor");
    }

    private static void ApproachMatchesScene(TeachingDiagramPlan.Site site)
    {
        // Dense reference polyline: the scene's authored planks (north to south) then StandApproachRoute.Build from "Dirt Path 00".
        float[] planks = input["ScenePlanksXZ"].Split(',').Select(v => float.Parse(v, Inv)).ToArray();
        var dense = new List<(float x, float z)>();
        for (int i = planks.Length - 2; i >= 0; i -= 2) dense.Add((planks[i], planks[i + 1]));
        var route = StandApproachRoute.Build(planks[0], planks[1], F("Plank00YawRadians"));
        foreach (var p in route.Skip(1)) dense.Add((p.X, p.Z));
        float[] t = TeachingDiagramPlan.FixtureTrackXZ;
        double worst = 0;
        for (int k = 0; k + 1 < t.Length; k += 2)
        {
            double best = double.MaxValue;
            for (int j = 0; j + 1 < dense.Count; j++)
                best = Math.Min(best, TeachingDiagramPlan.SegmentDistance(t[k], t[k + 1], dense[j].x, dense[j].z, dense[j + 1].x, dense[j + 1].z));
            worst = Math.Max(worst, best);
        }
        Check(worst < 0.01, "every figure track point lies on the scene + StandApproachRoute track (within 1 cm)", worst.ToString("0.#####", Inv) + " m");
        Check(t[0] == dense[0].x && t[1] == dense[0].z, "the figure track starts at the north plank \"Dirt Path 51\"");
        Check(t[t.Length - 1] < -site.StandSizeM * 0.5f && t[t.Length - 1] > StandApproachRoute.EndZ, "the figure track leaves the property and stops before the route's own end (a stub)");
        double join = double.MaxValue;
        for (int j = 0; j + 1 < dense.Count; j++)
            join = Math.Min(join, TeachingDiagramPlan.SegmentDistance(site.FeederEndX, site.FeederZ, dense[j].x, dense[j].z, dense[j + 1].x, dense[j + 1].z));
        Check(join < 0.01, "the feeder's east end (PathX, FeederZ) lies on the real approach track", join.ToString("0.#####", Inv) + " m");
        Check(Math.Abs(StandApproachRoute.GateCentreX(route, -site.StandSizeM * 0.5f) - site.FeederEndX) < 1e-4f, "the approach crosses the south edge at PathX (the gate)");
        Check(Math.Abs(TeachingDiagramPlan.FixtureClearingX - F("ClearingX")) < 1e-4f && Math.Abs(TeachingDiagramPlan.FixtureClearingZ - F("ClearingZ")) < 1e-4f
            && Math.Abs(TeachingDiagramPlan.FixtureClearingRadiusM - F("ClearingScaleX") * 0.5f) < 1e-4f, "clearing position and radius equal the scene's \"Forest Clearing\"");
    }

    private static void RetainedBands(TeachingDiagramPlan.Site site, TeachingDiagramPlan.Stage stage)
    {
        float half = site.StandSizeM * 0.5f, rackHalf = site.RackWidthM * 0.5f, r = TeachingDiagramPlan.TreeRadiusM;
        float[] z = TeachingDiagramPlan.FixtureEntranceZ;
        var crowns = stage.Items.Where(i => OnMap(i) && i.Kind == TeachingDiagramPlan.Kind.Crown).ToList();
        Check(crowns.All(c => c.Role == TeachingDiagramPlan.Role.Canopy && c.Marks == TeachingDiagramPlan.Marks.None && !c.Quiet), "map trees are plain standing discs at full strength, unmarked");
        for (int band = 0; band <= 5; band++)
        {
            float lo = band == 0 ? -half : z[band - 1] + rackHalf, hi = band == 5 ? half : z[band] - rackHalf;
            int inside = crowns.Count(c => WorldZ(site, c.Y) - r >= lo - 1e-3f && WorldZ(site, c.Y) + r <= hi + 1e-3f);
            string name = band == 0 ? "outer south" : band == 5 ? "outer north" : "interior " + band;
            Check(inside >= 8, "retained band " + (band + 1) + " (" + name + ", " + lo.ToString("0.00", Inv) + ".." + hi.ToString("0.00", Inv) + " m) has standing trees wholly outside every corridor", inside.ToString(Inv));
        }
        for (int i = 0; i < 5; i++)
        {
            int standing = crowns.Count(c => Math.Abs(WorldZ(site, c.Y) - z[i]) < 1e-3f);
            Check(standing >= 10, "planned rack " + (i + 1) + " still has standing trees along it (planning does not clear)", standing.ToString(Inv));
        }
        Check(crowns.All(c => !TeachingDiagramPlan.OnExistingSurface(site, WorldX(site, c.X), WorldZ(site, c.Y))), "no tree disc on the existing feeder, pads, approach or clearing");
        Check(crowns.All(c => Math.Abs(WorldX(site, c.X)) + r <= half + 1e-3f && Math.Abs(WorldZ(site, c.Y)) + r <= half + 1e-3f
            && WorldX(site, c.X) - r >= site.HeadlandX + site.PathWidthM * 0.5f), "every tree disc is inside the property and east of the headland");
    }

    private static void StateKeyAndSemantics(TeachingDiagramPlan.Stage stage)
    {
        var all = stage.Items;
        var key = all.Where(i => !OnMap(i)).ToList();
        // Each state's shape/pattern signature, ignoring colour entirely.
        string Signature(TeachingDiagramPlan.Item i) => i.Kind + (i.Kind == TeachingDiagramPlan.Kind.Corridor ? "/" + i.State : "");
        var states = new Dictionary<string, string>
        {
            ["existing"] = Signature(key.Single(i => i.Id == "key-existing")),
            ["draft"] = Signature(key.Single(i => i.Id == "key-draft")),
            ["planned"] = Signature(key.Single(i => i.Id == "key-planned")),
            ["entrance"] = Signature(key.Single(i => i.Id == "key-entrance")),
            ["field-marked"] = Signature(key.Single(i => i.Id == "key-field-mark")),
            ["harvested"] = Signature(key.Single(i => i.Id == "key-harvested")),
            ["retained"] = Signature(key.Single(i => i.Id == "key-retained-tree")),
        };
        Check(states.Values.Distinct().Count() == states.Count, "every key state has its own shape/pattern (colour-independent)", string.Join(" ", states.Select(kv => kv.Key + "=" + kv.Value)));
        Check(key.Count(i => i.Kind == TeachingDiagramPlan.Kind.Stump) == 2 && key.Any(i => i.Id == "key-tree-1a") && key.Any(i => i.Id == "key-tree-2a"),
            "harvested swatch carries stumps; draft and planned swatches keep standing trees");
        var mapSignatures = new HashSet<string>(all.Where(OnMap).Select(Signature));
        Check(mapSignatures.Contains(states["existing"]) && mapSignatures.Contains(states["planned"]) && mapSignatures.Contains(states["entrance"]) && mapSignatures.Contains(states["retained"]),
            "every symbol on the map is explained in the key");
        Check(!mapSignatures.Contains(states["harvested"]) && !mapSignatures.Contains("Stump") && !mapSignatures.Contains(states["draft"]) && !mapSignatures.Contains(states["field-marked"]),
            "no harvested, stump, draft or field-marked symbol on the map: nothing is shown cut or built");
        Check(!all.Any(i => i.Role == TeachingDiagramPlan.Role.Fell || (i.Marks & TeachingDiagramPlan.Marks.Slash) != 0), "no Fell colour or removal slash anywhere: a planned rack is not a Fell mark");
        string[] k = TeachingDiagramPlan.FigureKey;
        Check(k.Length == 7 && k[0].StartsWith("Existing") && k[1].StartsWith("Draft") && k[2].StartsWith("Planned") && k[3].StartsWith("Entrance")
            && k[4].StartsWith("Field-marked") && k[5].StartsWith("Harvested") && k[6].Contains("standing tree"), "key rows name existing / draft / planned / entrance / field-marked / harvested / standing tree");
        Check(k[1].Contains("dashed") && k[2].Contains("solid") && k[2].Contains("trees stand") && k[4].Contains("not felled") && k[5].Contains("stumps"),
            "each key row names its pattern in words");
        string[] cleared = { "clear", "cut", "open", "felled" };
        Check(new[] { k[1], k[2], k[3] }.All(t => !cleared.Any(w => t.IndexOf(w, StringComparison.OrdinalIgnoreCase) >= 0)),
            "no draft/planned/entrance wording calls the route cleared, cut or open");
        string notes = string.Join(" ", stage.Notes);
        Check(notes.Contains("not a Fell mark") && notes.Contains("only Work Plan approval commits the work") && notes.Contains("pending work only"),
            "notes: confirming creates pending work only, field-marking is not a Fell mark, only Work Plan approval commits");
    }

    private static void WordsAndLayout(TeachingDiagramPlan.Site site, TeachingDiagramPlan.Stage stage)
    {
        Check(stage.Title == "Illustrative five-rack plan — not your saved Stand Map. Planning or confirming does not clear the routes.", "title/caption is exactly the approved wording");
        string notes = string.Join(" ", stage.Notes);
        Check(notes.Contains("16–20 m") && notes.Contains("guidance for this plantation, not a hard rule"), "16-20 m is labelled as plantation guidance, not a hard rule");
        Check(notes.Contains("(15.6–19.4 m)"), "the stated spread equals the fixture's own uneven gaps");
        Check(notes.Contains("Approximate scale") && notes.Contains("north up") && notes.Contains("not a tree count"), "approximate-scale caveat, orientation and no-census statement");
        Check(notes.Contains("between racks 1 and 2") && notes.Contains("existing western headland") && notes.Contains("existing southern feeder") && notes.Contains("existing approach track"),
            "notes name A/B/C existing infrastructure and where the feeder joins");
        Check(stage.Items.Count(i => i.Kind == TeachingDiagramPlan.Kind.NorthArrow) == 1 && stage.Labels.Any(t => t.Value == "N"), "north arrow with its N label");
        Check(new[] { "A", "B", "C", "1", "2", "3", "4", "5" }.All(v => stage.Labels.Count(t => t.Value == v) == 1), "map tags A, B, C and rack numbers 1-5 each appear once");
        Check(stage.MinTextSize >= 20f && stage.Labels.All(t => t.FontSize >= 22f), "figure words have a 20 UI-unit floor and 22-unit design size (>= 20 units at the 900-unit frame)");
        Check(stage.Width == TeachingDiagramPlan.LessonWidth && stage.Height == TeachingDiagramPlan.LessonHeight, "figure keeps the lesson picture proportions the Unity layout audit expects");
        var labels = stage.Labels;
        Check(labels.All(t => t.X >= 0 && t.X + t.Width <= stage.Width && t.Y >= 0 && t.Y + t.Height <= stage.Height), "every figure word box lies inside the picture");
        int overlaps = 0;
        for (int i = 0; i < labels.Length; i++)
            for (int j = i + 1; j < labels.Length; j++)
                if (labels[i].X < labels[j].X + labels[j].Width && labels[j].X < labels[i].X + labels[i].Width && labels[i].Y < labels[j].Y + labels[j].Height && labels[j].Y < labels[i].Y + labels[i].Height)
                { overlaps++; Console.WriteLine("  overlap " + labels[i].Value + " / " + labels[j].Value); }
        Check(overlaps == 0, "no two figure word boxes overlap (design units; rendered fit is a Unity check)");
        // Tags over the map sit clear of the corridors, so a letter never hides a rack edge.
        foreach (var t in labels.Where(t => t.Backed))
            Check(TeachingDiagramPlan.FixtureEntranceZ.All(z => Math.Abs(TeachingDiagramPlan.FigureY(site, z) - (t.Y + t.Height * 0.5f)) > t.Height * 0.5f + site.RackWidthM * TeachingDiagramPlan.FigurePxPerM * 0.5f),
                "map tag " + t.Value + " does not cover a rack corridor");
        var key = TeachingDiagramPlan.FigureKey;
        Check(key.All(t => t.Length <= 36), "key rows stay short (<= 36 characters, at most two lines in their boxes)", key.Max(t => t.Length).ToString(Inv));
    }

    private static void Determinism(TeachingDiagramPlan.Site site, TeachingDiagramPlan.Stage stage)
    {
        var again = TeachingDiagramPlan.ObjectiveOneFigure(site);
        Check(Dump(stage).SequenceEqual(Dump(again)), "the figure regenerates identically");
        Check(stage.Items.Select(i => i.Id).Distinct().Count() == stage.Items.Count, "every figure item id is unique", stage.Items.Count.ToString(Inv));
    }

    private static string N(float v) => Math.Round(v, 3).ToString("0.###", Inv);

    private static IEnumerable<string> Dump(TeachingDiagramPlan.Stage stage)
    {
        yield return "STAGE|" + stage.Id + "|" + N(stage.Width) + "|" + N(stage.Height) + "|" + N(stage.MinTextSize) + "|" + stage.Title;
        foreach (var i in stage.Items)
            yield return string.Join("|", i.Id, i.SourceId, i.Kind, i.Role, i.State, N(i.X), N(i.Y), N(i.X1), N(i.Y1), N(i.Radius), N(i.Width), i.Z.ToString(Inv),
                i.Points == null ? "-" : string.Join(";", i.Points.Select(N)));
        foreach (var t in stage.Labels) yield return string.Join("|", "LABEL", t.Value, N(t.X), N(t.Y), N(t.Width), N(t.Height), N(t.FontSize), t.Role, t.Backed);
        foreach (var n in stage.Notes) yield return "NOTE|" + n;
    }
}
