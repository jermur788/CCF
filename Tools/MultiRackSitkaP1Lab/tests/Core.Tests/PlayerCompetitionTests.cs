using System.Text.Json;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

internal static class PlayerCompetitionTests
{
    internal static void Register(string manifestPath)
    {
        PlayerWorkPlanAdapter Open() => PlayerWorkPlanAdapter.Open(Path.GetDirectoryName(manifestPath)!);
        T.Add("competition PC-01: every fixture CI and neighbour count exactly match the annual calculation", () =>
        {
            var view = Open().Read();
            var annual = AnnualRules.Calculate(new AnnualState(view.Frame.EcologicalYear, view.Frame.Trees),
                20261116, view.Extent.AreaHa);
            foreach (var row in annual.Trees.Where(row => row.Before.IsLiving))
            {
                var report = PlayerCompetitionDiagnostics.Measure(view, row.Before.TreeId);
                T.Eq(F32.Bits(row.PreGrowthCi), F32.Bits(report.CurrentTotal), row.Before.TreeId + " CI f32 bits");
                T.Eq(row.Neighbours, report.Neighbours.Count, "same cutoff");
                float sum = 0f;
                foreach (var neighbour in report.Neighbours) sum += neighbour.Contribution;
                T.Eq(F32.Bits(sum), F32.Bits(report.CurrentTotal), "all contributions sum in annual order");
                T.True(Math.Abs(report.Neighbours.Sum(entry => entry.SharePercent) - 100.0) < 0.0001, "share sum tolerance");
            }
        });
        T.Add("competition PC-02: ranking is strongest five, ordinal ties and combined remaining share", () =>
        {
            var view = WithTrees(Open().Read(), Enumerable.Range(1, 8).Select(i => Tree("N" + i, 1, 0))
                .Prepend(Tree("T", 0, 0)).Reverse().ToArray());
            var r = PlayerCompetitionDiagnostics.Measure(view, "T");
            T.Eq("N1,N2,N3,N4,N5", string.Join(',', r.TopFive.Select(n => n.TreeId)), "ordinal tie breaks");
            T.Eq(3, r.RemainingCount, "remainder count");
            T.Eq(37.5, r.RemainingSharePercent, "combined shares");
            T.True(r.Neighbours.All(n => n.Strength == "Strong"), "presentation bands");
        });
        T.Add("competition PC-03: empty neighbourhood, cutoff includes 8 m and excludes next f32", () =>
        {
            var view = WithTrees(Open().Read(), Tree("T", 0, 0));
            var empty = PlayerCompetitionDiagnostics.Measure(view, "T");
            T.Eq(0f, empty.CurrentTotal, "empty total"); T.Eq(0, empty.TopFive.Count, "empty ranks");
            T.Eq(0f, empty.HypotheticalTotal!.Value, "empty after");
            view = WithTrees(view, Tree("T", 0, 0), Tree("Edge", 8, 0), Tree("Beyond", MathF.BitIncrement(8f), 0));
            var r = PlayerCompetitionDiagnostics.Measure(view, "T");
            T.Eq("Edge", r.Neighbours.Single().TreeId, "exact inclusion");
            T.Eq(0.125f, r.CurrentTotal, "edge term");
        });
        T.Add("competition PC-04: overlap and small target DBH use annual floors; dead neighbour excluded", () =>
        {
            var view = WithTrees(Open().Read(), Tree("T", 0, 0) with { DbhCm = 0.5f }, Tree("Overlap", 0, 0),
                Tree("Dead", 0, 0) with { Death = new AnnualDeath("Dead", 0, "fell", "manual-work-plan") });
            var r = PlayerCompetitionDiagnostics.Measure(view, "T");
            T.Eq(1, r.Neighbours.Count, "only living neighbour");
            T.Eq(40f, r.CurrentTotal, "20 / max(1,0.5) / max(0.5,0)");
            T.True(double.IsFinite(r.TopFive[0].SharePercent), "no NaN");
        });
        T.Add("competition PC-05: pending, approved and cancelled previews use only genuine neighbour removals", () =>
        {
            var player = Open(); var original = player.Read(); var target = original.Frame.Trees[44].TreeId;
            var initial = PlayerCompetitionDiagnostics.Measure(original, target);
            string neighbour = initial.TopFive[0].TreeId;
            var pending = player.AddPending(Fence(original), neighbour, "competition inspection test").View;
            var p = PlayerCompetitionDiagnostics.Measure(pending, target);
            T.Eq("Pending", p.Neighbours.Single(n => n.TreeId == neighbour).PlannedRemoval, "pending truth");
            var approved = player.Approve(Fence(pending)).View;
            var a = PlayerCompetitionDiagnostics.Measure(approved, target);
            T.Eq(p.HypotheticalTotal, a.HypotheticalTotal, "approval fells zero");
            T.Eq(initial.CurrentTotal, a.CurrentTotal, "current unchanged");
            T.Eq("Approved", a.Neighbours.Single(n => n.TreeId == neighbour).PlannedRemoval, "approved truth");
            var cancelled = player.CancelApproved(Fence(approved)).View;
            T.Eq(initial.CurrentTotal, PlayerCompetitionDiagnostics.Measure(cancelled, target).HypotheticalTotal!.Value, "cancel");
        });
        T.Add("competition PC-06: preview exactly matches recomputation after Execute, target Fell has no after", () =>
        {
            var player = Open(); var v = player.Read(); string target = v.Frame.Trees[44].TreeId;
            string neighbour = PlayerCompetitionDiagnostics.Measure(v, target).TopFive[0].TreeId;
            v = player.AddPending(Fence(v), neighbour, "release intent").View;
            var preview = PlayerCompetitionDiagnostics.Measure(v, target);
            T.True(PlayerCompetitionDiagnostics.Measure(v, neighbour).HypotheticalTotal is null, "not retained");
            v = player.Approve(Fence(v)).View;
            v = player.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            T.Eq(preview.HypotheticalTotal!.Value, PlayerCompetitionDiagnostics.Measure(v, target).CurrentTotal, "same surviving roster");
            T.Refuses("CompetitionTargetDead", () => PlayerCompetitionDiagnostics.Measure(v, neighbour));
            T.Refuses("CompetitionTreeUnknown", () => PlayerCompetitionDiagnostics.Measure(v, "unknown"));
        });
        T.Add("competition PC-07: reads leave session/views immutable and all ten annual digests unchanged", () =>
        {
            var inspected = Open(); var control = Open(); var old = inspected.Read();
            string before = JsonSerializer.Serialize(old);
            for (int year = 0; year < 10; year++)
            {
                var v = inspected.Read();
                foreach (var tree in v.Frame.Trees.Where(t => t.IsLiving)) PlayerCompetitionDiagnostics.Measure(v, tree.TreeId);
                T.Eq(JsonSerializer.Serialize(v), JsonSerializer.Serialize(inspected.Read()), "no session mutation");
                var x = inspected.Advance(Fence(v), false);
                var y = control.Advance(Fence(control.Read()), false);
                T.Eq(JsonSerializer.Serialize(y.Annual), JsonSerializer.Serialize(x.Annual), "same annual/RNG outcome " + year);
            }
            T.Eq(before, JsonSerializer.Serialize(old), "old view unchanged");
            var report = PlayerCompetitionDiagnostics.Measure(old, old.Frame.Trees[0].TreeId);
            T.True(((IList<PlayerCompetitor>)report.Neighbours).IsReadOnly, "neighbours immutable");
            T.True(((IList<PlayerCompetitor>)report.TopFive).IsReadOnly, "top five immutable");
        });
        T.Add("competition PC-08: share strength bands are presentation-only and exactly source-derived", () =>
        {
            var v = WithTrees(Open().Read(), Tree("T", 0, 0), Tree("Strong", 1, 0) with { DbhCm = 88 },
                Tree("Moderate", 1, 0) with { DbhCm = 8 }, Tree("Low", 1, 0) with { DbhCm = 4 });
            var r = PlayerCompetitionDiagnostics.Measure(v, "T");
            T.Eq("Strong,Moderate,Low", string.Join(',', r.TopFive.Select(n => n.Strength)), "5 and 10 percent bands");
        });
    }

    private static AnnualTree Tree(string id, float x, float z) => new(id, x, z, 30, 20, 20, 2);
    private static PlayerFence Fence(PlayerSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);
    private static PlayerSessionView WithTrees(PlayerSessionView v, params AnnualTree[] trees) => v with
    { Frame = v.Frame with { Trees = Array.AsReadOnly(trees) } };
}
