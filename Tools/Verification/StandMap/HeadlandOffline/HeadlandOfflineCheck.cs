using System;
using System.Linq;
using UnityEngine;

public static class HeadlandOfflineCheck
{
    private static int passed, failed;
    private static void Check(bool condition, string name)
    {
        if (condition) passed++; else failed++;
        Console.WriteLine((condition ? "PASS " : "FAIL ") + name);
    }
    private static bool Near(float first, float second) => Math.Abs(first - second) < 0.0001f;
    private static bool Same(RackPoint first, RackPoint second) => first.X == second.X && first.Z == second.Z;
    private static MapHeadlandSurface Project()
    {
        var fixture = new HeadlandSourceFixture();
        return fixture.Source.Read(true, 0f).ExistingHeadlandSurfaces.Single();
    }

    public static int Main()
    {
        for (int bits = 0; bits < 8; bits++)
            Check(MapHeadlandSurface.ShouldShowExistingHeadland((bits & 1) != 0, (bits & 2) != 0, (bits & 4) != 0) == (bits == 7), "pure truth table " + bits);
        MapHeadlandSurface surface = Project();
        Check(Same(surface.Headland.Start, new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.SouthZ))
            && Same(surface.Headland.End, new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.NorthZ)), "source constants parity: western endpoints");
        Check(Same(surface.Feeder.Start, new RackPoint(ScenarioOneHeadlandGeometry.HeadlandX, ScenarioOneHeadlandGeometry.FeederZ))
            && Same(surface.Feeder.End, new RackPoint(ScenarioOneHeadlandGeometry.PathX, ScenarioOneHeadlandGeometry.FeederZ)), "source constants parity: feeder endpoints, no approach stub");
        Check(surface.Headland.WidthM == ScenarioOneHeadlandGeometry.Width && surface.Feeder.WidthM == ScenarioOneHeadlandGeometry.Width
            && surface.HeadlandJunction.RadiusM == ScenarioOneHeadlandGeometry.PadRadius && surface.PathJunction.RadiusM == ScenarioOneHeadlandGeometry.PadRadius
            && Same(surface.HeadlandJunction.Centre, surface.Feeder.Start) && Same(surface.PathJunction.Centre, surface.Feeder.End), "source constants parity: both widths and two pads");
        Console.WriteLine("GEOMETRY " + HeadlandFixture.Bytes(surface));

        StandMapModel model = HeadlandFixture.Create();
        string paths = HeadlandFixture.Bytes(model.Paths), racks = HeadlandFixture.Bytes(model.Racks);
        string legend = HeadlandFixture.Bytes(StandMapLegend.Entries(model, MapOverlay.None));
        model.Restamp();
        int staticVersion = model.StaticVersion, dynamicVersion = model.DynamicVersion, canopyVersion = model.CanopyVersion;
        var before = StandMapDrawPlan.BuildStatic(model, 520f, MapDetail.Full);
        string dynamic = HeadlandFixture.Bytes(StandMapDrawPlan.BuildDynamic(model, 520f, MapDetail.Full));
        model.ExistingHeadlandSurfaces.Add(surface);
        model.Restamp();
        Check(model.StaticVersion == staticVersion + 1 && model.DynamicVersion == dynamicVersion && model.CanopyVersion == canopyVersion, "visibility bumps only static version");
        int stableHash = model.ContentHash();
        model.Restamp();
        Check(model.ContentHash() == stableHash && model.StaticVersion == staticVersion + 1, "deterministic static hash and unchanged restamp");
        var after = StandMapDrawPlan.BuildStatic(model, 520f, MapDetail.Full);
        int first = after.FindIndex(item => item.Symbol == MapSymbol.PathCasing && item.SourceIndex == -1);
        var added = after.Skip(first).Take(8).ToArray();
        var without = after.Take(first).Concat(after.Skip(first + 8)).ToList();
        Console.WriteLine($"FULL_STATIC_COUNT BEFORE={before.Count} AFTER={after.Count} HEADLAND={added.Length}; two casing strokes, two pad outlines, two tan strokes, two pad fills");
        Check(after.Count == before.Count + 8 && HeadlandFixture.Bytes(before) == HeadlandFixture.Bytes(without), "exactly eight new full static items; all old items/order identical");
        Check(after[first - 1].Symbol == MapSymbol.RackBand && after[first + 8].Symbol == MapSymbol.PathCasing && after[first + 8].SourceIndex == 0, "layer immediately after rack bands and before original Paths");
        Check(added.Take(4).All(item => item.Symbol == MapSymbol.PathCasing) && added.Skip(4).All(item => item.Symbol == MapSymbol.Path), "both casings and both pad outlines before all tan fills");
        Check(added[0].Points.Count == 2 && added[1].Points.Count == 2 && added[4].Points.Count == 2 && added[5].Points.Count == 2
            && added[2].Closed && !added[2].Filled && added[3].Closed && !added[3].Filled
            && added[6].Filled && added[7].Filled && added[6].StrokePx == 0f && added[7].StrokePx == 0f, "two segments and two separate outline/fill pads, no late dark disc outline");
        StandMapTransform transform = model.TransformFor(520f);
        MapHeadlandSegment[] segments = { surface.Headland, surface.Feeder };
        for (int index = 0; index < segments.Length; index++)
        {
            MapPosition start = transform.WorldToPixels(segments[index].Start), end = transform.WorldToPixels(segments[index].End);
            Check(added[index].Points[0].Equals(start) && added[index].Points[1].Equals(end)
                && Near(added[index + 4].WidthPx, transform.MetresToPixels(segments[index].WidthM))
                && Near(added[index].WidthPx, added[index + 4].WidthPx + 2.6f), "exact projected endpoints/width/casing " + index);
        }
        for (int index = 0; index < 2; index++)
        {
            MapHeadlandPad pad = index == 0 ? surface.HeadlandJunction : surface.PathJunction;
            Check(Same(added[index + 2].World, pad.Centre) && Same(added[index + 6].World, pad.Centre)
                && Near(added[index + 6].Points[0].X - added[index + 6].Anchor.X, transform.MetresToPixels(pad.RadiusM)), "pad centre/radius " + index);
        }
        Check(dynamic == HeadlandFixture.Bytes(StandMapDrawPlan.BuildDynamic(model, 520f, MapDetail.Full)), "selected rack entrance/handles/dynamic plan unchanged");
        Check(paths == HeadlandFixture.Bytes(model.Paths) && racks == HeadlandFixture.Bytes(model.Racks) && model.Racks.Count == 5, "Dirt Path and five-rack positions/plan byte-identical");
        Check(StandMapLegend.Entries(model, MapOverlay.None).Count == StandMapLegend.Entries(HeadlandFixture.Create(), MapOverlay.None).Count + 1
            && StandMapLegend.Entries(model, MapOverlay.None).Any(entry => entry.Text == "Existing V2 headland / feeder — not a planned rack"), "presence-gated legend reuses Path, distinguishes rack plans");
        foreach (float side in new[] { 128f, 196f, 520f, 880f })
            foreach (MapOverlay overlay in Enum.GetValues<MapOverlay>())
                Check(HeadlandFixture.Bytes(StandMapDrawPlan.Build(model, side, MapDetail.Mini, overlay))
                    == HeadlandFixture.Bytes(StandMapDrawPlan.Build(HeadlandFixture.Create(), side, MapDetail.Mini, overlay)), "Mini byte-identical, side " + side + " overlay " + overlay);
        Check(HeadlandFixture.Bytes(StandMapDrawPlan.BuildStatic(model, 520f, MapDetail.Full)) == HeadlandFixture.Bytes(after), "repeat full projection byte-identical");
        var equivalent = HeadlandFixture.Create();
        equivalent.ExistingHeadlandSurfaces.Add(Project());
        Check(equivalent.ContentHash() == model.ContentHash(), "independently projected equivalent model hash identical");
        for (int field = 0; field < 10; field++)
        {
            float change = 0.001f;
            var changed = new MapHeadlandSurface(
                new RackPoint(surface.Headland.Start.X + (field == 0 ? change : 0), surface.Headland.Start.Z + (field == 1 ? change : 0)),
                new RackPoint(surface.Headland.End.X + (field == 2 ? change : 0), surface.Headland.End.Z + (field == 3 ? change : 0)),
                new RackPoint(surface.Feeder.Start.X + (field == 4 ? change : 0), surface.Feeder.Start.Z + (field == 5 ? change : 0)),
                new RackPoint(surface.Feeder.End.X + (field == 6 ? change : 0), surface.Feeder.End.Z + (field == 7 ? change : 0)),
                surface.Headland.WidthM + (field == 8 ? change : 0), surface.HeadlandJunction.RadiusM + (field == 9 ? change : 0));
            equivalent.ExistingHeadlandSurfaces[0] = changed;
            Check(equivalent.ContentHash() != stableHash, "sub-centimetre geometry invalidates static hash field " + field);
        }
        equivalent.ExistingHeadlandSurfaces[0] = surface;
        equivalent.ExistingHeadlandSurfaces.Add(surface);
        Check(equivalent.ContentHash() != stableHash, "collection count participates in static hash");
        model.ExistingHeadlandSurfaces.Clear();
        model.Restamp();
        Check(model.StaticVersion == staticVersion + 2 && HeadlandFixture.Bytes(StandMapLegend.Entries(model, MapOverlay.None)) == legend, "hide invalidates static layer and removes only new legend");

        Negative("Legacy40", fixture => fixture.Ecology.StandGeometryModelVersion = StandGeometryModel.Legacy40);
        Negative("non-V2 Enlarged80", fixture => fixture.Manager.RackPlanningV2Active = false);
        Negative("Reference preview", fixture => fixture.Manager.ReferencePreviewActive = true);
        Negative("no manager", fixture => fixture.Ui.Manager = null);
        Negative("disabled presentation", fixture => fixture.Presentation.isActiveAndEnabled = false);
        Negative("inactive surface", fixture => fixture.Surface.activeSelf = false);
        Negative("inactive parent hierarchy", fixture => fixture.Root.activeSelf = false);
        Negative("disabled renderer", fixture => fixture.Renderer.enabled = false);
        Negative("missing renderer", fixture => fixture.Renderer.Destroyed = true);
        Negative("duplicate named surface", fixture => fixture.AddSurface());
        Negative("ambiguous attached presentations", fixture => fixture.Root.Attach(new ScenarioOneHeadlandPresentation()));
        Negative("missing attached starting stand", fixture => fixture.Root.Components.OfType<ForestStartingStand>().Single().Destroyed = true);
        Negative("wrong manager/ecology attachment", fixture => fixture.Manager.gameObject = new GameObject("Other world"));
        var absent = new HeadlandSourceFixture(true, false);
        Check(absent.Source.Read(true, 0f).ExistingHeadlandSurfaces.Count == 0 && absent.Source.HeadlandProjectionReads == 0, "missing template/surface fails closed");
        absent.AddSurface();
        Check(absent.Source.Read(true, 0.25f).ExistingHeadlandSurfaces.Count == 1 && absent.Source.HeadlandProjectionReads == 1, "late physical surface appears at next cadence");
        var timed = new HeadlandSourceFixture();
        StandMapModel live = timed.Source.Read(true, 0f);
        live.Paths.AddRange(HeadlandFixture.Create().Paths);
        live.Racks.AddRange(HeadlandFixture.Create().Racks);
        string livePaths = HeadlandFixture.Bytes(live.Paths), liveRacks = HeadlandFixture.Bytes(live.Racks);
        for (int frame = 1; frame < 600; frame++) timed.Source.Read(true, frame / 60f);
        Console.WriteLine($"STEADY_REFRESH FRAMES=600 PROJECTIONS={timed.Source.HeadlandProjectionReads} ATTACHED_LOOKUPS={timed.Root.AttachedLookups}");
        Check(timed.Source.HeadlandProjectionReads == 1 && timed.Root.AttachedLookups == 1, "600 walking/pointer/view refreshes: one projection, one attached lookup");
        timed.Surface.activeSelf = false;
        Check(timed.Source.Read(true, 10f).ExistingHeadlandSurfaces.Count == 0 && timed.Source.HeadlandProjectionReads == 1, "surface loss hides without projection");
        timed.Surface.activeSelf = true;
        Check(timed.Source.Read(true, 10.1f).ExistingHeadlandSurfaces.Count == 0, "before 0.25 second deadline no visibility read");
        Check(timed.Source.Read(true, 10.25f).ExistingHeadlandSurfaces.Count == 1 && timed.Source.HeadlandProjectionReads == 2, "0.25 second deadline reacquires surface once");
        timed.Manager.RackPlanningV2Active = false;
        Check(timed.Source.Read(true, 10.5f).ExistingHeadlandSurfaces.Count == 0, "V2 to non-V2 on unchanged scenery key hides");
        timed.Manager.RackPlanningV2Active = true;
        Check(timed.Source.Read(true, 10.75f).ExistingHeadlandSurfaces.Count == 1 && timed.Source.HeadlandProjectionReads == 3, "non-V2 to V2 on unchanged scenery key projects once");
        Check(timed.Source.Read(false, 10.8f).ExistingHeadlandSurfaces.Count == 0, "no property clears immediately, independent of cadence");
        Check(timed.Source.Read(true, 10.81f).ExistingHeadlandSurfaces.Count == 1 && timed.Source.HeadlandProjectionReads == 4, "property return reacquires attached presentation");
        timed.Surface.Destroyed = true;
        timed.Surface.transform.Destroyed = true;
        Check(timed.Source.Read(true, 11.1f).ExistingHeadlandSurfaces.Count == 0, "destroyed child hides");
        timed.AddSurface();
        Check(timed.Source.Read(true, 11.35f).ExistingHeadlandSurfaces.Count == 1 && timed.Source.HeadlandProjectionReads == 5, "replacement child reprojects once");
        timed.Presentation.Destroyed = true;
        Check(timed.Source.Read(true, 11.6f).ExistingHeadlandSurfaces.Count == 0, "Unity-style destroyed presentation invalidates cached reference");
        timed.Presentation = timed.Root.Attach(new ScenarioOneHeadlandPresentation());
        Check(timed.Source.Read(true, 12.1f).ExistingHeadlandSurfaces.Count == 1, "destroyed presentation replacement acquired after backoff");
        timed.Root.scene = new Scene { handle = new SceneHandle(2) };
        int reads = timed.Source.HeadlandProjectionReads;
        Check(timed.Source.Read(true, 12.11f).ExistingHeadlandSurfaces.Count == 1 && timed.Source.HeadlandProjectionReads == reads + 1, "scene identity invalidates cache immediately");
        Check(livePaths == HeadlandFixture.Bytes(live.Paths) && liveRacks == HeadlandFixture.Bytes(live.Racks),
            "source transitions preserve existing Paths and five racks byte-for-byte");
        var missing = new HeadlandSourceFixture(false, false);
        for (int frame = 0; frame <= 1200; frame++) missing.Source.Read(true, frame / 20f);
        Check(missing.Root.AttachedLookups == 6 && missing.Source.HeadlandProjectionReads == 0, "unresolved component: six local retries total across 60 seconds");
        Check(missing.Source.LookupTimes.SequenceEqual(new[] { 0f, 0.5f, 1.5f, 3.5f, 7.5f, 15.5f }), "retry backoff deadlines, never 4 Hz discovery");
        missing.Presentation = missing.Root.Attach(new ScenarioOneHeadlandPresentation());
        missing.AddSurface();
        Check(missing.Source.Read(true, 61f).ExistingHeadlandSurfaces.Count == 0, "exhausted proof budget remains fail-closed");
        missing.Manager.RackPlanningV2Active = false;
        missing.Source.Read(true, 61.25f);
        missing.Manager.RackPlanningV2Active = true;
        Check(missing.Source.Read(true, 61.5f).ExistingHeadlandSurfaces.Count == 1, "eligibility transition resets bounded acquisition budget");
        var replacement = new HeadlandSourceFixture();
        missing.Ui.Manager = replacement.Manager;
        missing.Ui.Ecology = replacement.Ecology;
        Check(missing.Source.Read(true, 61.51f).ExistingHeadlandSurfaces.Count == 1, "source/ecology identity reacquires without stale surface");
        Console.WriteLine($"HEADLAND_OFFLINE_TALLY PASS={passed} FAIL={failed}; Unity/runtime/render NOT RUN");
        return failed == 0 ? 0 : 1;
    }

    private static void Negative(string name, Action<HeadlandSourceFixture> configure)
    {
        var fixture = new HeadlandSourceFixture();
        configure(fixture);
        Check(fixture.Source.Read(true, 0f).ExistingHeadlandSurfaces.Count == 0 && fixture.Source.HeadlandProjectionReads == 0, "fail closed: " + name);
    }
}
