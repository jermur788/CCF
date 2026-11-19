using System.Text.Json;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

internal static class ContinuationTests
{
    private const int Seed = 20261116;
    private static readonly float Area = F32.FromBits(DerivedManifest.AreaHaBits);
    // SHA-256 of UTF-8 System.Text.Json { Snapshot, Report }, without a trailing newline.
    private static readonly string[] Digests = { "df2d44fdf9a879bd8191b89597f712ca68bacb62ff17b6033b918d9e9f5586f1", "23a1d889f551a840ae2ab7e689ab6b977dc1b7dabbf187fe7207aeae561c6abd", "953fc7ef660e66cc5a76e89a01d12aa2524d4e5d6d4d5bb04867d4700bb56afc", "6fc220c377555edc15892613c6468a3ca00884b310f85ad80d8102f7eef3f5ba", "cbd51e38889da9cf65376d38b03a0befe49f398a125802c7cb85d1079992a487", "01cfb6560386799d01dd3ef7c8a1b8d98b2cff6e4fc054e24509d6ca308b796e", "c6baab550107d0978ade9f3107ac715e9dcc0a28a3a01d27b70f53accb59c646", "61879c1e79aaf9c5b503d874a38837807da80d01bb6f645560a842beb03e7ab0", "faaea476fcdaec744e18fec2953bbe8125b56a8e3d52f826e30fdc4aa956169a", "10d31913531c627089a01e5c66aeea0c6fee4efad74e31d31f579e5264bcd7d5" };

    internal static void Register(string manifestPath)
    {
        AnnualContinuation Open() => AnnualEngine.Open(P1Domain.DerivedP1ThirtyMetreExperimentV1,
            ManifestGenerator.FixtureId, Path.GetDirectoryName(manifestPath)!).ContinueInMemory();

        T.Add("continuation: sequential fixture years and recorded deterministic digests", () =>
        {
            var first = Open();
            var repeat = Open();
            var initialIds = first.Snapshot.State.Trees.Select(t => t.TreeId).ToArray();
            var frozen = new Dictionary<string, AnnualTree>(StringComparer.Ordinal);
            for (int year = 1; year <= Digests.Length; year++)
            {
                var previous = first.Snapshot;
                var report = first.Advance(year - 1, year - 1);
                var again = repeat.Advance(year - 1, year - 1);
                T.Eq(year - 1, report.YearBefore, "exact previous year");
                T.Eq(year, report.YearAfter, "exact next year");
                T.Eq(year, first.Snapshot.State.EcologicalYear, "state year");
                T.Eq(year, first.Snapshot.Revision, "revision");
                T.True(initialIds.SequenceEqual(first.Snapshot.State.Trees.Select(t => t.TreeId)), "all ordinal identities carried");
                CheckChronology(report, frozen);
                var bytes = Bytes(first.Snapshot, report);
                T.True(bytes.SequenceEqual(Bytes(repeat.Snapshot, again)), "repeat bytes");
                var digest = DerivedManifest.Sha256Hex(bytes);
                Console.WriteLine($"YEAR {year} DIGEST {digest} DEATHS {report.Deaths.Count}");
                T.Eq(Digests[year - 1], digest, "recorded yearly digest");
                T.Eq(year - 1, previous.State.EcologicalYear, "previous snapshot immutable");
            }
        });

        T.Add("continuation: synthetic deaths freeze post-growth fields and chronology", () =>
        {
            var engine = Synthetic(0.003f, Stem("A", 70), Stem("B", 30));
            var frozen = new Dictionary<string, AnnualTree>(StringComparer.Ordinal);
            for (int year = 1; year <= 12; year++)
                CheckChronology(engine.Advance(year - 1, year - 1), frozen);
            T.True(frozen.ContainsKey("B"), "synthetic boundary death exercised");
            T.Eq(1, frozen["B"].Death!.EcologicalYear, "death year is not observation year");
            T.Eq(12, engine.Snapshot.State.EcologicalYear, "no ten-year cap");
        });

        T.Add("continuation: empty and archived-only refuse; singleton advances", () =>
        {
            foreach (var engine in new[] { Synthetic(Area), Synthetic(Area,
                Stem("dead") with { Death = new AnnualDeath("dead", 0, "probe", "hazard") }) })
            {
                var before = engine.Snapshot;
                var bytes = JsonSerializer.SerializeToUtf8Bytes(before);
                T.Refuses("ContinuationNoLivingStems", () => engine.Advance(0, 0));
                T.True(ReferenceEquals(before, engine.Snapshot), "same snapshot on terminal refusal");
                T.True(bytes.SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(engine.Snapshot)), "terminal bytes unchanged");
            }
            var singleton = Synthetic(0.001f, Stem("solo", 200));
            T.Eq(1, singleton.Advance(0, 0).After.LivingCount, "singleton legal");
            T.Eq(1, singleton.Snapshot.Revision, "singleton commits");
            var terminal = Synthetic(0.003f, Stem("P30-008", 200), Stem("P30-001", 200));
            T.Eq(0, terminal.Advance(0, 0).After.LivingCount, "genuine transition to zero");
            var last = terminal.Snapshot;
            T.Refuses("ContinuationNoLivingStems", () => terminal.Advance(1, 1));
            T.True(ReferenceEquals(last, terminal.Snapshot), "terminal transition retained");
        });

        T.Add("continuation: independent fences and concurrent duplicates", () =>
        {
            var engine = Open();
            engine.Advance(0, 0);
            var before = engine.Snapshot;
            T.Refuses("ContinuationYearMismatch", () => engine.Advance(0, 1));
            T.Refuses("ContinuationRevisionMismatch", () => engine.Advance(1, 0));
            T.Refuses("ContinuationYearMismatch", () => engine.Advance(0, 0));
            T.True(ReferenceEquals(before, engine.Snapshot), "fence refusals leave exact snapshot");
            using var start = new Barrier(2);
            Task<bool> Call() => Task.Run(() =>
            {
                start.SignalAndWait();
                try { engine.Advance(1, 1); return true; }
                catch (P1Refusal e) when (e.Code == "ContinuationYearMismatch") { return false; }
            });
            var a = Call(); var b = Call();
            Task.WaitAll(a, b);
            T.Eq(1, new[] { a.Result, b.Result }.Count(x => x), "one concurrent winner");
            T.Eq(2, engine.Snapshot.State.EcologicalYear, "one year committed");
            T.Eq(2, engine.Snapshot.Revision, "one revision committed");
        });

        T.Add("continuation: failed step leaves both fences and bytes intact, retry commits once", () =>
        {
            var engine = Open();
            var before = engine.Snapshot;
            var bytes = JsonSerializer.SerializeToUtf8Bytes(before);
            bool injected = false;
            try { engine.Advance(0, 0, () => throw new InvalidOperationException("injected")); }
            catch (InvalidOperationException e) when (e.Message == "injected") { injected = true; }
            T.True(injected, "fault seam reached");
            T.True(ReferenceEquals(before, engine.Snapshot), "failed step publishes nothing");
            T.True(bytes.SequenceEqual(JsonSerializer.SerializeToUtf8Bytes(engine.Snapshot)), "failed bytes unchanged");
            var report = engine.Advance(0, 0);
            var control = Open();
            var expected = control.Advance(0, 0);
            T.True(Bytes(control.Snapshot, expected).SequenceEqual(Bytes(engine.Snapshot, report)), "retry deterministic");
            T.Refuses("ContinuationYearMismatch", () => engine.Advance(0, 0));
        });
    }

    private static byte[] Bytes(ContinuationSnapshot snapshot, AnnualReport report) =>
        JsonSerializer.SerializeToUtf8Bytes(new { Snapshot = snapshot, Report = report });

    private static void CheckChronology(AnnualReport report, Dictionary<string, AnnualTree> frozen)
    {
        foreach (var row in report.Trees)
        {
            if (frozen.TryGetValue(row.Before.TreeId, out var dead))
            {
                T.Eq(dead, row.Before, "frozen before");
                T.Eq(dead, row.After, "all dead fields frozen");
                T.True(row.MortalityRoll is null && row.DeathProbability is null, "no repeated hazard");
                T.True(report.Deaths.All(d => d.TreeId != dead.TreeId), "no repeated death");
            }
            else
            {
                T.Eq(AnnualRules.Grow(row.Before, row.PreGrowthCi), row.After with { Death = null }, "one post-growth update");
                if (row.After.Death is { } death)
                {
                    T.Eq(report.YearAfter, death.EcologicalYear, "death chronology");
                    T.Eq(row.Before.TreeId, death.TreeId, "death identity");
                    T.True(report.Deaths.Contains(death), "new death recorded");
                    frozen.Add(death.TreeId, row.After);
                }
            }
        }
    }

    private static AnnualTree Stem(string id, float dbh = 20) => new(id, id == "B" ? 0.25f : 0, 0, 30, dbh, 20.4f, 2);
    private static AnnualContinuation Synthetic(float area, params AnnualTree[] trees) => new(new AnnualState(0, trees), Seed, area);
}
