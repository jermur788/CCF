namespace SitkaP1.Core.NaturalProcesses.Tests;

internal static class StormAdapterTests
{
    private const string Suite = "storm";

    private static StormRequest Model1(int year, IReadOnlyList<StormTreeInput> trees, int seed = Fixtures.Seed, int? last = null) =>
        new(StormModelVersion.WindthrowV1, year, seed, trees, last);

    public static void Register()
    {
        TestRunner.Add(Suite, "JW-T20 StormModel 0 is storm-free with zero draws over 1000 years", () =>
        {
            var log = new List<string>();
            NaturalProcessRandom.DrawLog = log;
            try
            {
                for (int year = 1; year <= 1000; year++)
                {
                    var r = StormAdapter.Resolve(new StormRequest(StormModelVersion.StormFree, year, Fixtures.Seed, Fixtures.StormTrees(50)));
                    TestRunner.True(r.StormsOff && r.Event is null && r.Victims.Count == 0 && r.RngDrawCount == 0, "storm-free");
                }
            }
            finally { NaturalProcessRandom.DrawLog = null; }
            TestRunner.Eq(0, log.Count, "observed draws");
        });

        TestRunner.Add(Suite, "JW-T21 StormModel 0 shifts no other stream (jay output identical either way)", () =>
        {
            var jayReq = Fixtures.JayRequest(Fixtures.JayAndNearby(OuterEdge.North), Fixtures.Mast, Fixtures.OakCandidates(5, 4), year: 108);
            string before = Fixtures.Describe(JayAcornDispersal.Disperse(jayReq));
            StormAdapter.Resolve(new StormRequest(StormModelVersion.StormFree, 108, Fixtures.Seed, Fixtures.StormTrees(50)));
            string afterModel0 = Fixtures.Describe(JayAcornDispersal.Disperse(jayReq));
            StormAdapter.Resolve(Model1(108, Fixtures.StormTrees(50)));
            string afterModel1 = Fixtures.Describe(JayAcornDispersal.Disperse(jayReq));
            TestRunner.Eq(before, afterModel0, "model 0");
            TestRunner.Eq(before, afterModel1, "stateless streams: even model 1 moves nothing else");
        });

        TestRunner.Add(Suite, "JW-T22 StormModel 1 event years and severities match frozen Unity parity (seed 12345, years 1-1000)", () =>
        {
            var found = new List<(int, float)>();
            for (int year = 1; year <= 1000; year++)
            {
                var r = StormAdapter.Resolve(Model1(year, Array.Empty<StormTreeInput>()));
                TestRunner.True(r.Refusal is null, "no refusal");
                if (r.Event is not null) found.Add((r.Event.Year, r.Event.Severity));
                TestRunner.Eq(r.Event is null ? 1 : 3, r.RngDrawCount, "occurs (+severity+direction) draws");
            }
            TestRunner.Eq(Fixtures.FrozenEvents12345.Length, found.Count, "event count");
            for (int i = 0; i < found.Count; i++)
            {
                TestRunner.Eq(Fixtures.FrozenEvents12345[i].Year, found[i].Item1, "year " + i);
                TestRunner.Eq(Fixtures.FrozenEvents12345[i].Severity, found[i].Item2, "severity " + i);
            }
        });

        TestRunner.Add(Suite, "JW-T23 one woodland-wide event per year: lane split and tree list never change the event or victims", () =>
        {
            var twoLanes = Fixtures.StormTrees(400);
            var oneLane = Fixtures.StormTrees(400, laneOverride: "LANE-ONLY");
            var a = StormAdapter.Resolve(Model1(108, twoLanes));
            var b = StormAdapter.Resolve(Model1(108, oneLane));
            var c = StormAdapter.Resolve(Model1(108, Array.Empty<StormTreeInput>()));
            TestRunner.True(a.Event is not null, "storm in 108");
            TestRunner.Eq(a.Event, b.Event, "event independent of lanes");
            TestRunner.Eq(a.Event, c.Event, "event independent of trees");
            TestRunner.Eq(string.Join(",", a.Victims.Select(v => v.TreeId)), string.Join(",", b.Victims.Select(v => v.TreeId)), "victims independent of lanes");
            // Observed draws: one event (occurs + severity + direction) and exactly one roll per tree in each lane.
            var laneOf = twoLanes.ToDictionary(t => "WINDTHROW-v1-" + t.TreeId, t => t.LaneId, StringComparer.Ordinal);
            foreach (var (trees, name) in new[] { (twoLanes, "two lanes"), (oneLane, "one lane") })
            {
                var log = new List<string>();
                NaturalProcessRandom.DrawLog = log;
                try { StormAdapter.Resolve(Model1(108, trees)); }
                finally { NaturalProcessRandom.DrawLog = null; }
                TestRunner.Eq(403, log.Count, name + " observed draws in a storm year");
                TestRunner.Eq("STORM-OCCURS-v1,STORM-SEVERITY-v1,STORM-DIRECTION-v1", string.Join(",", log.Take(3)), name + " single event draws");
                TestRunner.Eq(0, log.Skip(3).Count(k => k.StartsWith("STORM-", StringComparison.Ordinal)), name + " no per-lane event draws");
                TestRunner.Eq(200, log.Count(k => laneOf.TryGetValue(k, out var lane) && lane == "LANE-A"), name + " LANE-A tree rolls");
                TestRunner.Eq(200, log.Count(k => laneOf.TryGetValue(k, out var lane) && lane == "LANE-B"), name + " LANE-B tree rolls");
                TestRunner.Eq(400, log.Distinct(StringComparer.Ordinal).Count() - 3, name + " one roll per tree");
            }
            var calmLog = new List<string>();
            NaturalProcessRandom.DrawLog = calmLog;
            try { StormAdapter.Resolve(Model1(107, twoLanes)); }
            finally { NaturalProcessRandom.DrawLog = null; }
            TestRunner.Eq("STORM-OCCURS-v1", string.Join(",", calmLog), "calm year: one occurrence draw, no tree rolls in either lane");
            TestRunner.True(a.Victims.Select(v => v.LaneId).Distinct().Count() == 2, "victims reported in both lanes from the single event");
            // The result type carries exactly one nullable event: there is no per-lane event collection.
            TestRunner.True(typeof(StormResult).GetProperties().Count(p => p.PropertyType == typeof(StormEvent)) == 1, "single event slot");
        });

        TestRunner.Add(Suite, "JW-T24 a second resolution for the same (or earlier) year is refused with zero draws", () =>
        {
            var r = StormAdapter.Resolve(Model1(108, Fixtures.StormTrees(10), last: 108));
            TestRunner.Eq(NaturalProcessRefusalCode.StormYearAlreadyResolved, r.Refusal?.Code ?? NaturalProcessRefusalCode.None, "code");
            TestRunner.Eq(0, r.RngDrawCount, "draws");
            var earlier = StormAdapter.Resolve(Model1(108, Fixtures.StormTrees(10), last: 200));
            TestRunner.Eq(NaturalProcessRefusalCode.StormYearAlreadyResolved, earlier.Refusal?.Code ?? NaturalProcessRefusalCode.None, "earlier year code");
            TestRunner.Eq(0, earlier.RngDrawCount, "earlier year draws");
            TestRunner.True(earlier.Event is null && earlier.Victims.Count == 0 && earlier.Exposure.Count == 0, "earlier year: no partial result");
            TestRunner.True(StormAdapter.Resolve(Model1(109, Fixtures.StormTrees(10), last: 108)).Refusal is null, "next year allowed");
        });

        TestRunner.Add(Suite, "JW-T25 direction is u * 360 (blow-towards, D-050) and fall glyph equals direction", () =>
        {
            int events = 0;
            bool belowBand = false, aboveBand = false;
            for (int year = 1; year <= 1000; year++)
            {
                var e = StormAdapter.Resolve(Model1(year, Array.Empty<StormTreeInput>())).Event;
                if (e is null) continue;
                events++;
                double u = NaturalProcessRandom.RollModel1("STORM-DIRECTION-v1", year, Fixtures.Seed);
                TestRunner.Eq(u * 360.0, e.DirectionDegrees, "direction formula");
                TestRunner.True(e.DirectionDegrees >= 0.0 && e.DirectionDegrees < 360.0, "direction range");
                TestRunner.Eq(e.DirectionDegrees, e.FallGlyphOrientationDegrees, "glyph");
                belowBand |= e.DirectionDegrees < 180.0;
                aboveBand |= e.DirectionDegrees > 270.0;
                var again = StormAdapter.Resolve(Model1(year, Array.Empty<StormTreeInput>())).Event!;
                TestRunner.Eq(e.DirectionDegrees, again.DirectionDegrees, "deterministic");
            }
            TestRunner.True(events > 0, "events seen");
            TestRunner.True(belowBand && aboveBand, "not confined to the old 180-270 band");
            TestRunner.True(typeof(WindthrowVictim).GetProperties().All(p => !p.Name.Contains("Bearing", StringComparison.Ordinal))
                && typeof(FallenDeadwoodRecord).GetProperties().All(p => !p.Name.Contains("Bearing", StringComparison.Ordinal)), "no per-tree fall bearing persisted");
        });

        TestRunner.Add(Suite, "JW-T26 feedback is qualitative Stable/Watch/Exposed with no numeric hazard or p", () =>
        {
            var trees = new[]
            {
                new StormTreeInput("A", "L", "C", 22f, 28f, 22f, .3f, 0f),    // V 1.799 -> Watch
                new StormTreeInput("B", "L", "C", 25f, 30f, 24f, .8f, 1.5f),  // V 7.210 -> Exposed
                new StormTreeInput("C", "L", "C", 6f, 10f, 20f, .1f, 0f),     // V 0.006 -> Stable
            };
            var bands = StormAdapter.ExposureBands(trees);
            TestRunner.Eq("Watch", bands[0].Label, "A");
            TestRunner.Eq("Exposed", bands[1].Label, "B");
            TestRunner.Eq("Stable", bands[2].Label, "C");
            var r = StormAdapter.Resolve(Model1(5, trees));
            TestRunner.Eq(3, r.Exposure.Count, "model 1 feedback every year, calm or not");
            Type[] numeric = { typeof(float), typeof(double), typeof(decimal) };
            TestRunner.True(typeof(TreeExposureFeedback).GetProperties().All(p => !numeric.Contains(p.PropertyType)), "no numeric field in feedback");
            TestRunner.True(typeof(WindthrowVictim).GetProperties().All(p => !numeric.Contains(p.PropertyType)), "victim exposes no p");
            TestRunner.True(!NaturalProcessText.BandExplanation.Contains('%'), "no percentage in text");
        });

        TestRunner.Add(Suite, "JW-T27 victims: windthrow cause, one fallen deadwood record, one canopy opening, matches independent two-pass roll", () =>
        {
            var trees = Fixtures.StormTrees(400);
            var r = StormAdapter.Resolve(Model1(108, trees));
            TestRunner.True(r.Event is not null && r.Victims.Count > 0, "storm with victims");
            var expected = trees.OrderBy(t => t.TreeId, StringComparer.Ordinal)
                .Where(t => NaturalProcessRandom.RollModel1("WINDTHROW-v1-" + t.TreeId, 108, Fixtures.Seed)
                    < StormVulnerability.FailureChance(StormVulnerability.Index(t), r.Event!.Severity))
                .Select(t => t.TreeId).ToArray();
            TestRunner.Eq(string.Join(",", expected), string.Join(",", r.Victims.Select(v => v.TreeId)), "victim set");
            TestRunner.Eq(3 + trees.Count, r.RngDrawCount, "one roll per tree");
            var byId = trees.ToDictionary(t => t.TreeId);
            foreach (var v in r.Victims)
            {
                TestRunner.Eq("windthrow", v.MortalityCause, "cause");
                TestRunner.Eq("windthrow", v.Deadwood.MortalityCause, "deadwood cause");
                TestRunner.Eq(DeadwoodForm.Fallen, v.Deadwood.Form, "fallen only");
                TestRunner.Eq(v.TreeId, v.Deadwood.OriginTreeId, "origin");
                TestRunner.Eq(108, v.Deadwood.FallenYear, "year");
                TestRunner.Eq(byId[v.TreeId].CellId, v.Opening.CellId, "opening at the victim's cell");
                TestRunner.Eq(1f, v.Opening.RecentOpeningIncrement, "one opening");
            }
            TestRunner.Eq(r.Victims.Count, r.Victims.Select(v => v.Deadwood.DeadwoodId).Distinct().Count(), "one deadwood per victim");
            TestRunner.Eq(1, Enum.GetValues<DeadwoodForm>().Length, "no snag/snapped form exists");

            // Frozen victim vectors computed outside this code base: Assets/ForestPrototype/StormModel.cs +
            // SimulationRandom.cs at origin/main, ReducedProposal + BoundedRational, key "WINDTHROW-v1-" + id
            // (ForestEcologyController.Storms.cs), seed 12345, Fixtures.StormTrees(60).
            var sixty = Fixtures.StormTrees(60);
            TestRunner.Eq("T0017,T0018,T0022,T0034,T0037,T0038,T0039,T0041,T0042,T0044",
                string.Join(",", StormAdapter.Resolve(Model1(108, sixty)).Victims.Select(v => v.TreeId)), "frozen victims year 108");
            TestRunner.Eq("T0017,T0018,T0019,T0020,T0021,T0022,T0034,T0037,T0038,T0039,T0043,T0044,T0045,T0056,T0058",
                string.Join(",", StormAdapter.Resolve(Model1(229, sixty)).Victims.Select(v => v.TreeId)), "frozen victims year 229");
        });

        TestRunner.Add(Suite, "JW-T31 storm request/result/exposure collections are snapshots: input mutation and output writes change nothing", () =>
        {
            var trees = Fixtures.StormTrees(60).ToArray();
            var request = Model1(108, trees);
            string baseline = Fixtures.Describe(StormAdapter.Resolve(request));
            for (int i = 0; i < trees.Length; i++)
                trees[i] = new StormTreeInput("X" + i, "LANE-X", "CX", 40f, 10f, 40f, 1f, 2f);
            Fixtures.AssertReadOnly(request.Trees, "request.Trees");
            var result = StormAdapter.Resolve(request);
            TestRunner.Eq(baseline, Fixtures.Describe(result), "request unchanged by input mutation");
            TestRunner.True(result.Victims.Count > 0, "fixture has victims");
            Fixtures.AssertReadOnly(result.Victims, "Victims");
            Fixtures.AssertReadOnly(result.Exposure, "Exposure");
            var bands = StormAdapter.ExposureBands(Fixtures.StormTrees(5));
            Fixtures.AssertReadOnly(bands, "ExposureBands output");
            TestRunner.Eq(baseline, Fixtures.Describe(result), "result unchanged by write attempts");

            // R2-01: a plain `with` copy shares no writable path back to the original.
            var requestCopy = request with { };
            var resultCopy = result with { };
            Fixtures.AssertReadOnly(requestCopy.Trees, "with-copy Trees");
            Fixtures.AssertReadOnly(resultCopy.Victims, "with-copy Victims");
            Fixtures.AssertReadOnly(resultCopy.Exposure, "with-copy Exposure");
            TestRunner.Eq(baseline, Fixtures.Describe(StormAdapter.Resolve(request)), "original request unchanged after with-copy probes");
            TestRunner.Eq(baseline, Fixtures.Describe(result), "original result unchanged after with-copy probes");
            var bandsBefore = string.Join(",", bands);
            Fixtures.AssertReadOnly(bands, "ExposureBands output (again)");
            TestRunner.Eq(bandsBefore, string.Join(",", bands), "exposure bands unchanged");

            // R2-03: the delta reviewer's probe — two equal requests built from the same array are equal again.
            var shared = Fixtures.StormTrees(60).ToArray();
            var first = request with { Trees = shared };
            var second = request with { Trees = shared };
            TestRunner.True(first == second, "requests initialised from the same array are equal");
            TestRunner.True(Model1(108, Fixtures.StormTrees(60)) == Model1(108, Fixtures.StormTrees(60).ToArray()), "structurally equal requests are equal");
            TestRunner.Eq(Model1(108, Fixtures.StormTrees(60)).GetHashCode(), Model1(108, Fixtures.StormTrees(60)).GetHashCode(), "structurally equal request hashes");
            TestRunner.True(requestCopy == request && resultCopy == result, "with-copies equal their sources");
            TestRunner.True(Model1(108, Fixtures.StormTrees(60)) != Model1(108, Fixtures.StormTrees(59)), "request with a missing tree differs");
            TestRunner.True(Model1(108, Fixtures.StormTrees(60)) != Model1(109, Fixtures.StormTrees(60)), "request with a different year differs");
            TestRunner.True(StormAdapter.Resolve(Model1(108, shared)) == StormAdapter.Resolve(Model1(108, Fixtures.StormTrees(60))), "equal requests give equal results");
            TestRunner.True(StormAdapter.ExposureBands(Fixtures.StormTrees(5)).Equals(StormAdapter.ExposureBands(Fixtures.StormTrees(5))), "equal exposure bands are equal");
        });

        TestRunner.Add(Suite, "JW-T28 shuffled input ordering yields the same victim set (5 shuffles)", () =>
        {
            var trees = Fixtures.StormTrees(400);
            string baseline = Fixtures.Describe(StormAdapter.Resolve(Model1(229, trees)));
            for (int s = 1; s <= 5; s++)
                TestRunner.Eq(baseline, Fixtures.Describe(StormAdapter.Resolve(Model1(229, Fixtures.Shuffled(trees, s)))), "shuffle " + s);
        });

        TestRunner.Add(Suite, "JW-T29 determinism: repeated 1000-year runs give identical results", () =>
        {
            var trees = Fixtures.StormTrees(120);
            string Run() => string.Join("\n", Enumerable.Range(1, 1000).Select(y => Fixtures.Describe(StormAdapter.Resolve(Model1(y, trees)))));
            string first = Run();
            TestRunner.Eq(first, Run(), "repeat");
            TestRunner.True(first.Length > 0, "non-empty");
        });

        TestRunner.Add(Suite, "JW-T30 non-neutral site/edge and invalid trees are refused (no invented multipliers)", () =>
        {
            var site = new[] { new StormTreeInput("A", "L", "C", 20f, 25f, 20f, .5f, 0f, SiteHazard: 1.2f) };
            var edge = new[] { new StormTreeInput("A", "L", "C", 20f, 25f, 20f, .5f, 0f, EdgeFacing: .5f) };
            var dup = new[] { new StormTreeInput("A", "L", "C", 20f, 25f, 20f, .5f, 0f), new StormTreeInput("A", "L", "C", 20f, 25f, 20f, .5f, 0f) };
            var bad = new[] { new StormTreeInput("A", "L", "C", float.NaN, 25f, 20f, .5f, 0f) };
            TestRunner.Eq(NaturalProcessRefusalCode.StormSiteHazardNotNeutral, StormAdapter.Resolve(Model1(5, site)).Refusal?.Code ?? default, "site");
            TestRunner.Eq(NaturalProcessRefusalCode.StormExternalEdgeNotNeutral, StormAdapter.Resolve(Model1(5, edge)).Refusal?.Code ?? default, "edge");
            TestRunner.Eq(NaturalProcessRefusalCode.DuplicateTreeId, StormAdapter.Resolve(Model1(5, dup)).Refusal?.Code ?? default, "dup");
            TestRunner.Eq(NaturalProcessRefusalCode.InvalidTreeInput, StormAdapter.Resolve(Model1(5, bad)).Refusal?.Code ?? default, "bad");
            TestRunner.Eq(NaturalProcessRefusalCode.UnknownStormModel,
                StormAdapter.Resolve(new StormRequest((StormModelVersion)2, 5, 1, site)).Refusal?.Code ?? default, "model 2 not defined");
            TestRunner.Eq(NaturalProcessRefusalCode.InvalidYear, StormAdapter.Resolve(Model1(0, Array.Empty<StormTreeInput>())).Refusal?.Code ?? default, "year");
        });
    }
}
