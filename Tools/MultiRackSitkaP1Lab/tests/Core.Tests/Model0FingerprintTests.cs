using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Multi #899 Stage 1: an exact fingerprint of CrownModel 0 (LegacyFrozen) output on fixed runs. The hash was captured on
/// base 1e4e0221 BEFORE any engine change and is pinned here, so every later refactor must reproduce model 0 bit for bit.
/// The fingerprint writes every float/double as its IEEE-754 bit pattern, so it is independent of number formatting.
/// </summary>
internal static class Model0FingerprintTests
{
    // Captured on base 1e4e02215d373e224fe6a74074ae3f62983db242 before any src/Core change (see the #899/#900 handoff).
    internal const string PinnedSingleAreaHash = "7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece";
    internal const string PinnedMultiAreaHash = "a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d";

    internal static void Register(string manifestPath)
    {
        string fixtureDirectory = Path.GetDirectoryName(manifestPath)!;

        T.Add("model0 F-01: single-area p1d-30m fixture, ten genuine years, exact pinned fingerprint", () =>
        {
            string hash = SingleAreaFingerprint(fixtureDirectory);
            Console.WriteLine("  model0 single-area fingerprint " + hash);
            T.Eq(hash, SingleAreaFingerprint(fixtureDirectory), "repeat run is identical");
            if (PinnedSingleAreaHash.Length > 0) T.Eq(PinnedSingleAreaHash, hash, "pinned model 0 single-area fingerprint");
        });

        T.Add("model0 F-02: MA-1 two-area session, two multi-tree Fells and ten years, exact pinned fingerprint", () =>
        {
            string hash = MultiAreaFingerprint();
            Console.WriteLine("  model0 multi-area fingerprint " + hash);
            T.Eq(hash, MultiAreaFingerprint(), "repeat run is identical");
            if (PinnedMultiAreaHash.Length > 0) T.Eq(PinnedMultiAreaHash, hash, "pinned model 0 multi-area fingerprint");
        });
    }

    internal static string SingleAreaFingerprint(string fixtureDirectory)
    {
        var engine = AnnualEngine.Open(P1Domain.DerivedP1ThirtyMetreExperimentV1, "p1d-30m-sitka-adult-v1", fixtureDirectory);
        var continuation = engine.ContinueInMemory();
        var w = new Fingerprint();
        w.State(continuation.Snapshot.State);
        for (int year = 0; year < 10; year++)
        {
            var snapshot = continuation.Snapshot;
            if (!snapshot.State.Trees.Any(tree => tree.IsLiving)) break;
            w.Report(continuation.Advance(snapshot.State.EcologicalYear, snapshot.Revision));
        }
        w.State(continuation.Snapshot.State);
        return w.Hash();
    }

    internal static string MultiAreaFingerprint()
    {
        var c = new MultiAreaContinuation(MultiAreaGenerator.Generate());
        var w = new Fingerprint();
        foreach (var area in c.ReadView().Areas) w.Trees(area.Trees);
        for (int year = 0; year < 10; year++)
        {
            if (year is 0 or 3) FellFirst(c, w, inA: 12, inB: 3);
            var v = c.ReadView();
            var result = c.Advance(new MultiAreaFence(v.SessionId, v.Source, v.PlanStateRevision), false);
            foreach (var area in result.Annual!.Areas)
            {
                w.Text(area.AreaId);
                if (area.Report is { } report) w.Report(report); else w.Text("carried");
            }
        }
        foreach (var area in c.ReadView().Areas) w.Trees(area.Trees);
        return w.Hash();
    }

    // Fell the first n living trees (ordinal ID order) of each area through the real fenced Work Plan.
    private static void FellFirst(MultiAreaContinuation c, Fingerprint w, int inA, int inB)
    {
        var v = c.ReadView();
        foreach (var (areaId, count) in new[] { ("MA-A", inA), ("MA-B", inB) })
            foreach (var tree in v.Areas.Single(a => a.AreaId == areaId).Trees.Where(t => t.IsLiving).Take(count))
            {
                v = c.AddPending(new MultiAreaFence(v.SessionId, v.Source, v.PlanStateRevision), areaId, tree.TreeId,
                    "fingerprint fixed selection").View;
            }
        v = c.Approve(new MultiAreaFence(v.SessionId, v.Source, v.PlanStateRevision)).View;
        var executed = c.Execute(new MultiAreaFence(v.SessionId, v.Source, v.PlanStateRevision), v.Approved!.PlanId,
            v.Approved.ExecutionKey);
        foreach (var change in executed.Execution!.Event.Areas)
        {
            w.Text(change.AreaId);
            w.Stand(change.Before);
            w.Stand(change.After);
            foreach (var fell in change.Fells) w.Text(fell.TreeId + "|" + fell.Cause + "|" + fell.Mechanism);
        }
    }

    /// <summary>A canonical, line-based writer whose SHA-256 is the fingerprint.</summary>
    internal sealed class Fingerprint
    {
        private readonly StringBuilder text = new();

        internal void Text(string value) => text.Append(value).Append('\n');

        private static string F(float value) => F32.Hex(F32.Bits(value));
        private static string D(double value) => BitConverter.DoubleToUInt64Bits(value).ToString("x16", CultureInfo.InvariantCulture);
        private static string N(double? value) => value is double d ? D(d) : "null";
        private static string I(int value) => value.ToString(CultureInfo.InvariantCulture);

        internal void Tree(AnnualTree t) => Text(string.Join("|", t.TreeId, F(t.XM), F(t.ZM), I(t.AgeYears), F(t.DbhCm), F(t.HeightM),
            F(t.CrownRadiusM), t.Death is null ? "living" : t.Death.EcologicalYear + "/" + t.Death.Cause + "/" + t.Death.Mechanism));

        internal void Trees(IEnumerable<AnnualTree> trees)
        {
            foreach (var t in trees) Tree(t);
        }

        internal void State(AnnualState state)
        {
            Text("year " + I(state.EcologicalYear));
            Trees(state.Trees);
        }

        internal void Stand(AnnualStand s) =>
            Text(string.Join("|", I(s.LivingCount), D(s.StemsPerHa), D(s.BasalAreaM2PerHa), D(s.QuadraticMeanDbhCm), D(s.RelativeDensity)));

        internal void Report(AnnualReport r)
        {
            Text("report " + I(r.YearBefore) + "->" + I(r.YearAfter));
            Stand(r.Before);
            Stand(r.PostGrowth);
            Stand(r.After);
            Text(D(r.PostGrowthDensityPressure) + "|" + N(r.MeanVulnerability));
            foreach (var row in r.Trees)
            {
                Tree(row.Before);
                Tree(row.After);
                Text(string.Join("|", I(row.Neighbours), F(row.PreGrowthCi), N(row.Suppression), N(row.DeathProbability),
                    row.MortalityRoll is float roll ? F(roll) : "null"));
            }
            foreach (var d in r.Deaths) Text(d.TreeId + "|" + I(d.EcologicalYear) + "|" + d.Cause + "|" + d.Mechanism);
        }

        internal string Hash() => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString()))).ToLowerInvariant();
    }
}
