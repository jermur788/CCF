namespace SitkaP1.Core.NaturalProcesses.Tests;

internal static class JayDispersalTests
{
    private const string Suite = "jay";

    private static T WithDrawLog<T>(Func<T> body, out List<string> log)
    {
        log = new List<string>();
        NaturalProcessRandom.DrawLog = log;
        try { return body(); }
        finally { NaturalProcessRandom.DrawLog = null; }
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "JW-T01 jay OFF by default consumes zero RNG draws and returns empty results", () =>
        {
            TestRunner.True(!JayAcornDispersalSettings.Default.JayAcornDispersal, "Jay default OFF");
            TestRunner.True(!JayAcornDispersalSettings.Default.NearbyOakWoodland, "Nearby default OFF");
            var r = WithDrawLog(() => JayAcornDispersal.Disperse(Fixtures.JayRequest(JayAcornDispersalSettings.Default, Fixtures.Mast,
                Fixtures.OakCandidates(5, 4))), out var log);
            TestRunner.Eq(0, log.Count, "observed draws");
            TestRunner.Eq(0, r.RngDrawCount, "reported draws");
            TestRunner.True(!r.Ran && r.Refusal is null, "not run, not refused");
            TestRunner.Eq(0, r.DivertedOnMapCandidateIds.Count + r.Delivered.Count + r.Exported.Count, "empty");
        });

        TestRunner.Add(Suite, "JW-T02 Nearby ON with Jay OFF is refused with zero draws", () =>
        {
            var r = WithDrawLog(() => JayAcornDispersal.Disperse(Fixtures.JayRequest(new(false, true, OuterEdge.North), Fixtures.Mast,
                Fixtures.OakCandidates(2, 2))), out var log);
            TestRunner.Eq(NaturalProcessRefusalCode.NearbyOakWoodlandRequiresJayDispersal, r.Refusal?.Code ?? NaturalProcessRefusalCode.None, "code");
            TestRunner.Eq(0, log.Count, "draws");
            TestRunner.Eq(0, r.Delivered.Count + r.Exported.Count, "empty");
        });

        TestRunner.Add(Suite, "JW-T03 non-mast year produces zero jay input and zero draws (Jay and Nearby ON)", () =>
        {
            var r = WithDrawLog(() => JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayAndNearby(OuterEdge.West), Fixtures.NoMast,
                Fixtures.OakCandidates(5, 4))), out var log);
            TestRunner.Eq(0, log.Count, "draws");
            TestRunner.True(!r.Ran, "not run");
            TestRunner.Eq(0, r.DivertedOnMapCandidateIds.Count + r.Delivered.Count + r.Exported.Count, "no input");
        });

        TestRunner.Add(Suite, "JW-T04 pure Sitka with Nearby OFF produces zero jay input", () =>
        {
            var r = JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, new List<OnMapOakSeedCandidate>()));
            TestRunner.True(r.Ran && r.Refusal is null, "ran");
            TestRunner.Eq(0, r.OffMapCap, "off-map cap");
            TestRunner.Eq(0, r.DivertedOnMapCandidateIds.Count + r.Delivered.Count + r.Exported.Count, "no input");
            TestRunner.Eq(0, r.RngDrawCount, "draws");
        });

        foreach (OuterEdge edge in Enum.GetValues<OuterEdge>())
        {
            TestRunner.Add(Suite, $"JW-T05 pure Sitka with Nearby ON ({edge}) gives <= 4 edge-attributed candidates on 0.64 ha", () =>
            {
                var woodland = Fixtures.Woodland064();
                var r = JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayAndNearby(edge), Fixtures.Mast, new List<OnMapOakSeedCandidate>()));
                TestRunner.Eq(4, r.OffMapCap, "off-map cap");
                TestRunner.Eq(4, r.Delivered.Count + r.Exported.Count, "every offered candidate is accounted for");
                TestRunner.True(r.Delivered.Count <= 4, "delivered bounded");
                TestRunner.Eq(0, r.DivertedOnMapCandidateIds.Count, "nothing diverted");
                foreach (var c in r.Delivered.Concat(r.Exported))
                {
                    TestRunner.Eq(JayCandidateOrigin.NearbyOakWoodland, c.Origin, "origin");
                    TestRunner.True(c.SourceTreeId is null, "no fabricated source tree ID");
                    TestRunner.Eq<OuterEdge?>(edge, c.SourceEdge, "edge attribution");
                    TestRunner.True(!woodland.Contains(c.Source), "source line lies outside the woodland");
                    bool onLine = edge switch
                    {
                        OuterEdge.North => c.Source.Y == 81.0,
                        OuterEdge.South => c.Source.Y == -1.0,
                        OuterEdge.East => c.Source.X == 81.0,
                        _ => c.Source.X == -1.0,
                    };
                    TestRunner.True(onLine, "source on the authored line 1 m outside " + edge);
                }
                foreach (var c in r.Delivered) TestRunner.True(woodland.Contains(c.Destination), "delivered lands inside");
                foreach (var c in r.Exported) TestRunner.True(!woodland.Contains(c.Destination), "exported lands outside");
            });
        }

        TestRunner.Add(Suite, "JW-T06 on-map cap diverts (<= 6 on 0.64 ha), never double counts, keeps source tree ID", () =>
        {
            var supplied = Fixtures.OakCandidates(5, 4); // 20 candidates
            var r = JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, supplied));
            TestRunner.Eq(6, r.OnMapCap, "cap");
            TestRunner.Eq(6, r.DivertedOnMapCandidateIds.Count, "diverted == min(cap, supplied)");
            TestRunner.Eq(6, r.Delivered.Count + r.Exported.Count, "carried == diverted, nothing added on top");
            var byId = supplied.ToDictionary(c => c.CandidateId, StringComparer.Ordinal);
            TestRunner.Eq(6, r.DivertedOnMapCandidateIds.Distinct(StringComparer.Ordinal).Count(), "distinct");
            foreach (var c in r.Delivered.Concat(r.Exported))
            {
                TestRunner.True(byId.ContainsKey(c.CandidateId), "taken from the supplied list");
                TestRunner.Eq(byId[c.CandidateId].SourceTreeId, c.SourceTreeId, "source tree ID retained");
                TestRunner.Eq(JayCandidateOrigin.OnMapOakTree, c.Origin, "origin");
                TestRunner.True(r.DivertedOnMapCandidateIds.Contains(c.CandidateId), "carried candidate was diverted");
            }
            var ordinaryAfterDiversion = supplied.Where(c => !r.DivertedOnMapCandidateIds.Contains(c.CandidateId)).Count();
            TestRunner.Eq(supplied.Count, ordinaryAfterDiversion + r.Delivered.Count + r.Exported.Count, "zero double counting");
        });

        TestRunner.Add(Suite, "JW-T07 below cap: delivered+exported == supplied, no selection draws", () =>
        {
            var supplied = Fixtures.OakCandidates(1, 3);
            var r = JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, supplied));
            TestRunner.Eq(3, r.DivertedOnMapCandidateIds.Count, "all diverted");
            TestRunner.Eq(3, r.Delivered.Count + r.Exported.Count, "all carried");
            TestRunner.Eq(6, r.RngDrawCount, "exactly two transport draws per candidate");
        });

        TestRunner.Add(Suite, "JW-T08 out-of-bounds acorns are exported, never redrawn or clamped, stable across runs", () =>
        {
            var woodland = Fixtures.Woodland064();
            // Corner oaks guarantee many destinations outside the woodland.
            var supplied = new List<OnMapOakSeedCandidate>();
            for (int i = 0; i < 6; i++)
                supplied.Add(new OnMapOakSeedCandidate("CORNER-C" + i, "CORNER-OAK", "sessile-oak", new LocalPoint(1, 1)));
            int totalExported = 0;
            for (int year = 1; year <= 40; year++)
            {
                var req = Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, supplied, year);
                var first = JayAcornDispersal.Disperse(req);
                var second = JayAcornDispersal.Disperse(req);
                TestRunner.Eq(Fixtures.Describe(first), Fixtures.Describe(second), "stable across runs");
                TestRunner.Eq(12, first.RngDrawCount, "two draws per acorn: no retry draws");
                totalExported += first.Exported.Count;
                foreach (var c in first.Exported)
                {
                    TestRunner.True(!woodland.Contains(c.Destination), "exported is outside");
                    TestRunner.Eq(c.Source.X + c.DistanceM * Math.Cos(c.BearingRadians), c.Destination.X, "unclamped X");
                    TestRunner.Eq(c.Source.Y + c.DistanceM * Math.Sin(c.BearingRadians), c.Destination.Y, "unclamped Y");
                }
            }
            TestRunner.True(totalExported > 40, "corner sources export many acorns");
        });

        TestRunner.Add(Suite, "JW-T09 transport is r = 5 + 195u^2 (5-200 m) with uniform bearing from the keyed stream", () =>
        {
            var supplied = Fixtures.OakCandidates(1, 3);
            var r = JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, supplied, year: 11));
            foreach (var c in r.Delivered.Concat(r.Exported))
            {
                int index = int.Parse(c.CandidateId[^2..], System.Globalization.CultureInfo.InvariantCulture);
                string key = JayAcornTransport.StreamKey(c.SourceTreeId!, index);
                double u = NaturalProcessRandom.RollModel1(key + "/distance", 11, Fixtures.Seed);
                double v = NaturalProcessRandom.RollModel1(key + "/bearing", 11, Fixtures.Seed);
                TestRunner.Eq(5.0 + 195.0 * u * u, c.DistanceM, "distance formula");
                TestRunner.Eq(2.0 * Math.PI * v, c.BearingRadians, "bearing formula");
                TestRunner.True(c.DistanceM >= 5.0 && c.DistanceM <= 200.0, "range");
                TestRunner.True(c.BearingRadians >= 0 && c.BearingRadians < 2 * Math.PI, "bearing range");
            }
            TestRunner.True(JayAcornTransport.StreamKey("OAK-1", 2).StartsWith("JAY-ACORN-v1-", StringComparison.Ordinal), "dedicated stream");
        });

        TestRunner.Add(Suite, "JW-T10 shuffled on-map input gives the identical diversion and transport", () =>
        {
            var supplied = Fixtures.OakCandidates(6, 5);
            string baseline = Fixtures.Describe(JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayAndNearby(OuterEdge.South), Fixtures.Mast, supplied)));
            for (int s = 1; s <= 5; s++)
            {
                var shuffled = Fixtures.Shuffled(supplied, s);
                TestRunner.Eq(baseline, Fixtures.Describe(JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayAndNearby(OuterEdge.South), Fixtures.Mast, shuffled))), "order-independent");
            }
        });

        TestRunner.Add(Suite, "JW-T11 cap rounding is deterministic nearest integer", () =>
        {
            TestRunner.Eq(6, JayAcornDispersal.CapFor(10, 0.64), "10 x 0.64");
            TestRunner.Eq(4, JayAcornDispersal.CapFor(6, 0.64), "6 x 0.64");
            TestRunner.Eq(3, JayAcornDispersal.CapFor(10, 0.25), "midpoint away from zero");
            TestRunner.Eq(0, JayAcornDispersal.CapFor(10, 0.04), "0.4 rounds to 0");
            TestRunner.ApproxEq(0.64, Fixtures.Woodland064().AreaHectares, 1e-12, "fixture area");
        });

        TestRunner.Add(Suite, "JW-T12 delivered acorns get no microsite bias, no browse immunity, no instant seedlings", () =>
        {
            TestRunner.True(JayDispersalResult.RequiresOrdinaryEstablishmentEvaluation, "ordinary evaluation required");
            TestRunner.Eq(0.0, JayDispersalResult.MicrositeBonus, "no microsite bonus");
            TestRunner.True(!JayDispersalResult.BrowseImmune, "no browse immunity");
            TestRunner.True(!JayDispersalResult.CreatesInstantSeedlings, "no instant seedlings");
            // The destination ignores ground state: same rolls regardless of what the caller's ground looks like.
            TestRunner.True(typeof(JayDispersalRequest).GetProperties().All(p => !p.Name.Contains("Gap", StringComparison.Ordinal)
                && !p.Name.Contains("Bramble", StringComparison.Ordinal) && !p.Name.Contains("Moss", StringComparison.Ordinal)), "no microsite inputs");
        });

        TestRunner.Add(Suite, "JW-T13 invalid inputs are refused: non-oak, duplicate ID, bad boundary, bad year", () =>
        {
            var nonOak = new List<OnMapOakSeedCandidate> { new("X1", "BEECH-1", "beech", new LocalPoint(5, 5)) };
            TestRunner.Eq(NaturalProcessRefusalCode.NonOakSeedCandidate,
                JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, nonOak)).Refusal?.Code ?? NaturalProcessRefusalCode.None, "non-oak");
            var dup = Fixtures.OakCandidates(1, 1);
            dup.Add(dup[0]);
            TestRunner.Eq(NaturalProcessRefusalCode.DuplicateCandidateId,
                JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, dup)).Refusal?.Code ?? NaturalProcessRefusalCode.None, "duplicate");
            var badBoundary = new JayDispersalRequest(3, 1, Fixtures.JayOn, Fixtures.Mast,
                new WoodlandFootprint(new[] { new LocalPoint(0, 0), new LocalPoint(1, 1) }), Fixtures.OakCandidates(1, 1));
            TestRunner.Eq(NaturalProcessRefusalCode.InvalidWoodlandBoundary, JayAcornDispersal.Disperse(badBoundary).Refusal?.Code ?? NaturalProcessRefusalCode.None, "boundary");
            TestRunner.Eq(NaturalProcessRefusalCode.InvalidYear,
                JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayOn, Fixtures.Mast, Fixtures.OakCandidates(1, 1), year: 0)).Refusal?.Code ?? NaturalProcessRefusalCode.None, "year");
        });

        TestRunner.Add(Suite, "JW-T14 Nearby ON with oaks: both caps apply independently (6 diverted + 4 external)", () =>
        {
            var r = JayAcornDispersal.Disperse(Fixtures.JayRequest(Fixtures.JayAndNearby(OuterEdge.East), Fixtures.Mast, Fixtures.OakCandidates(5, 4)));
            var all = r.Delivered.Concat(r.Exported).ToList();
            TestRunner.Eq(6, all.Count(c => c.Origin == JayCandidateOrigin.OnMapOakTree), "on-map");
            TestRunner.Eq(4, all.Count(c => c.Origin == JayCandidateOrigin.NearbyOakWoodland), "external");
        });
    }
}
