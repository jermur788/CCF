using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

public static class OperationFuzzTests
{
    /// <summary>Test-only deterministic xorshift (fixed seed). It drives operation order only — it is NOT used to create fixtures.</summary>
    private sealed class Xor
    {
        private ulong _s;
        public Xor(ulong seed) { _s = seed; }
        public int Next(int n) { _s ^= _s << 13; _s ^= _s >> 7; _s ^= _s << 17; return (int)(_s % (ulong)n); }
    }

    [Test]
    public static void RandomOperationSequences_KeepEveryInvariant()
    {
        foreach (ulong seed in new ulong[] { 1, 7, 42, 20261114 })
        {
            var rng = new Xor(seed * 2654435761UL + 88172645463325252UL);
            var s = Fx.Demo();
            var original = s.State.Trees.ToDictionary(t => t.TreeId, t => (t.XMm, t.YMm, t.DbhMm, t.HeightMm, t.AgeAtInventoryYears));
            int felledSoFar = 0; long lastEvents = 0;
            for (int step = 0; step < 300; step++)
            {
                var free = s.State.Trees.Where(t => s.State.IsAdult(t) && t.Mark == MarkState.Unmarked).Select(t => t.TreeId).ToList();
                var marked = s.State.Trees.Where(t => t.Mark == MarkState.PlayerMarked).Select(t => t.TreeId).ToList();
                var plans = s.State.Plans.Select(p => p.PlanId).ToList();
                List<string> Pick(List<string> pool, int max) => pool.OrderBy(_ => rng.Next(1000)).Take(1 + rng.Next(max)).ToList();
                string RandPlan() => plans.Count == 0 ? "P0001" : plans[rng.Next(plans.Count)];
                int op = rng.Next(11);
                switch (op)
                {
                    case 0: case 1: if (free.Count > 0) s.MarkTrees(Pick(free, 4)); break;
                    case 2: if (marked.Count > 0) s.UnmarkTrees(Pick(marked, 3)); break;
                    case 3: if (marked.Count > 0) s.CreatePlan(Pick(marked, 4)); break;
                    case 4: s.ProposePlan(RandPlan()); break;
                    case 5: s.ApprovePlan(RandPlan()); break;
                    case 6: s.QueuePlan(RandPlan()); break;
                    case 7: s.ExecutePlan(RandPlan()); break;
                    case 8: if (rng.Next(3) == 0) s.AdvanceExperimentStep(); else s.CancelPlan(RandPlan()); break;
                    case 9: s.RejectPlan(RandPlan()); break;
                    default: // adversarial requests: dead/duplicate/garbage IDs must be refused without changing anything
                        string h = s.StateHash; long n = s.Events.Count;
                        var dead = s.State.Trees.First(t => t.Status != TreeStatus.Live).TreeId;
                        T.False(s.MarkTrees(new[] { dead }).Ok, "dead mark refused");
                        T.False(s.CreatePlan(new[] { "x", "x" }).Ok, "duplicate refused");
                        T.Eq(h, s.StateHash, "refusals leave the state hash alone"); T.Eq(n, (long)s.Events.Count, "and append nothing");
                        break;
                }
                if (step % 60 == 59) s = LabSession.FromFile(LabFile.Parse(LabFile.Serialize(s.ToFile())), Fx.Catalog); // periodic save/load

                SnapshotValidator.ValidateSnapshot(s.State);                                  // every structural invariant
                T.Eq(CanonicalJson.SnapshotHash(s.State), s.StateHash, $"seed {seed} step {step}: hash tracks state");
                T.True(s.Events.Count >= lastEvents, "event log never shrinks"); lastEvents = s.Events.Count;
                int felled = s.State.Trees.Count(t => t.Status == TreeStatus.Felled);
                T.True(felled >= felledSoFar, "felled trees never come back"); felledSoFar = felled;
                foreach (var t in s.State.Trees)
                {
                    var o = original[t.TreeId];
                    T.True((t.XMm, t.YMm, t.DbhMm, t.HeightMm, t.AgeAtInventoryYears) == o, $"{t.TreeId}: position/DBH/height/age never change (NO BIOLOGICAL GROWTH)");
                }
                // open plans always reference only PLAYER_MARKED live adults, each tree in at most one open plan
                var seen = new HashSet<string>();
                foreach (var p in s.State.Plans.Where(p => p.IsOpen))
                    foreach (var id in p.TreeIds) T.True(seen.Add(id) && s.State.FindTree(id)!.Mark == MarkState.PlayerMarked, "plan membership exclusive & marked");
            }
            var replay = s.VerifyReplay();
            T.True(replay.Ok, $"seed {seed}: final replay {replay.Error}");
            T.True(s.Events.Count > 40, $"seed {seed}: the run exercised a real history ({s.Events.Count} events)");
        }
    }

    [Test]
    public static void Core_StaysFastOn2000Trees()
    {
        var s = Fx.Session("sitka-flat-perf-2000-v1");
        var ids = s.State.Trees.Where(t => s.State.IsAdult(t)).Take(300).Select(t => t.TreeId).ToList();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 20; i++) { s.Preview(ids); Views.Build(s); }
        double perOp = sw.Elapsed.TotalMilliseconds / 20;
        Console.WriteLine($"      perf: preview(300)+full view of 2000 trees = {perOp:F2} ms/op");
        T.True(perOp < 500, "Core preview + view build is far below the 2 s budget");
        Fx.Ok(s.MarkTrees(ids)); Fx.Ok(s.CreatePlan(ids));
        T.True(s.VerifyReplay().Ok, "2000-tree replay");
    }
}
