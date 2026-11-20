using SitkaP1.Core;
using static SitkaP1.Core.Tests.PlayerSessionTests;

namespace SitkaP1.Core.Tests;

/// <summary>
/// MS-01…MS-06 (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §4.3–4.4, §5, plan P2): pure map geometry, paint order and the
/// exact-ID hit-test. Registered from <see cref="PlayerSessionTests"/>. Author feature tests, not qualification.
/// </summary>
internal static class PlayerMapSceneTests
{
    private const int Seed = 20261116;
    private static readonly float Area = F32.FromBits(DerivedManifest.AreaHaBits);

    internal static void Register(string manifestPath)
    {
        string fixtureDirectory = Path.GetDirectoryName(manifestPath)!;
        AnnualContinuation Open() => AnnualEngine.Open(P1Domain.DerivedP1ThirtyMetreExperimentV1,
            ManifestGenerator.FixtureId, fixtureDirectory).ContinueInMemory();

        T.Add("map MS-01: one item per frame tree with its exact ID, U = X + 15, V = 15 - Z and its true crown radius", () =>
        {
            var view = Open().ReadPlayerView();
            var scene = PlayerMapScene.Build(view);
            T.Eq(view.Frame.Trees.Count, scene.Count, "one item per frame tree");
            T.True(scene.Select(i => i.TreeId).Order(StringComparer.Ordinal)
                .SequenceEqual(view.Frame.Trees.Select(t => t.TreeId).Order(StringComparer.Ordinal)), "exactly the frame's IDs, each once");
            foreach (var tree in view.Frame.Trees)
            {
                var item = scene.Single(i => string.Equals(i.TreeId, tree.TreeId, StringComparison.Ordinal));
                T.Eq((double)tree.XM + 15.0, item.U, tree.TreeId + " U");
                T.Eq(15.0 - (double)tree.ZM, item.V, tree.TreeId + " V");
                T.Eq((double)tree.CrownRadiusM, item.RadiusM, tree.TreeId + " crown radius");
                T.Eq(tree.HeightM, item.HeightM, tree.TreeId + " height");
                T.Eq(tree.DbhCm, item.DbhCm, tree.TreeId + " DBH");
                T.Eq(PlayerMapStatus.Living, item.Status, tree.TreeId + " status");
                T.True(item.DeathYear is null && item.DeathCause is null, tree.TreeId + " has no death");
                T.True(item.U > 0 && item.U < 30 && item.V > 0 && item.V <= 30, tree.TreeId + " inside the 30 m square");
            }
            // North-up: the fixture's south row (P30-001…010) is drawn at the bottom, and its first column at the west.
            T.True(Item(view, "P30-001").V > Item(view, "P30-091").V, "the south row has the larger V");
            T.True(Item(view, "P30-001").U < Item(view, "P30-010").U, "east is to the right");
            ReadOnlyScene(scene, "Build");
        });

        T.Add("map MS-02: paint order is height-ascending then ordinal ID, stumps after every crown, deterministic", () =>
        {
            // Synthetic stand: equal heights force the ordinal tie-break; one natural death; two trees felled.
            var trees = new[]
            {
                Tree("B", -10, -10, 12f), Tree("A", -5, -5, 12f), Tree("C", 0, 0, 8f), Tree("D", 5, 5, 20f),
                Tree("E", 10, 10, 15f) with { Death = new AnnualDeath("E", 0, "self-thinning", "hazard") },
                Tree("F", -12, 12, 30f), Tree("G", 12, -12, 5f),
            };
            var view = Felled(new AnnualContinuation(new AnnualState(0, trees), Seed, Area), "G", "D");
            var scene = PlayerMapScene.Build(view);
            T.True(scene.Select(i => i.TreeId).SequenceEqual(new[] { "C", "A", "B", "E", "F", "D", "G" }),
                "paint order: " + string.Join(",", scene.Select(i => i.TreeId)));
            T.True(scene.Select(i => i.PaintOrder).SequenceEqual(Enumerable.Range(0, scene.Count)), "PaintOrder is the index");
            T.True(Bytes(scene).SequenceEqual(Bytes(PlayerMapScene.Build(view))), "deterministic across builds");

            // The real fixture after a Fell: crowns by height then ordinal ID, then stumps by ordinal ID.
            var fixture = Felled(Open(), "P30-077", "P30-012");
            var real = PlayerMapScene.Build(fixture);
            var crowns = real.Where(i => i.Status != PlayerMapStatus.Felled).ToArray();
            var stumps = real.Where(i => i.Status == PlayerMapStatus.Felled).ToArray();
            T.True(real.Take(crowns.Length).SequenceEqual(crowns), "every crown is painted before every stump");
            int ties = 0;
            for (int i = 1; i < crowns.Length; i++)
            {
                bool tie = crowns[i - 1].HeightM == crowns[i].HeightM;
                if (tie) ties++;
                T.True(crowns[i - 1].HeightM < crowns[i].HeightM
                    || (tie && string.CompareOrdinal(crowns[i - 1].TreeId, crowns[i].TreeId) < 0), "crown order at " + i);
            }
            T.True(ties > 0, "the fixture has equal heights, so the ordinal tie-break was exercised");
            T.True(stumps.Select(s => s.TreeId).SequenceEqual(new[] { "P30-012", "P30-077" }), "stumps by ordinal ID");
            T.True(real.Select(i => i.PaintOrder).SequenceEqual(Enumerable.Range(0, real.Count)), "fixture PaintOrder is the index");
            T.True(Bytes(real).SequenceEqual(Bytes(PlayerMapScene.Build(fixture))), "fixture scene is deterministic");
        });

        T.Add("map MS-03: hit-test on overlapping discs is topmost first, boundary inclusive and empty on bare ground", () =>
        {
            var trees = new[]
            {
                Tree("LOW", -5f, 0f, 10f, 2f),    // U 10,   V 15, r 2
                Tree("MID", -4f, 0f, 12f, 2f),    // U 11,   V 15, r 2
                Tree("TOP", -4.5f, 0f, 15f, 1f),  // U 10.5, V 15, r 1
                Tree("FAR", 10f, 10f, 11f, 1f),   // U 25,   V 5,  r 1
            };
            var scene = PlayerMapScene.Build(new AnnualContinuation(new AnnualState(0, trees), Seed, Area).ReadPlayerView());
            Ids(PlayerMapScene.HitTest(scene, 10.5, 15), "three overlapping discs", "TOP", "MID", "LOW");
            Ids(PlayerMapScene.HitTest(scene, 10.5, 16), "on TOP's north boundary", "TOP", "MID", "LOW");
            Ids(PlayerMapScene.HitTest(scene, 8, 15), "exactly on LOW's west boundary", "LOW");
            Ids(PlayerMapScene.HitTest(scene, 13, 15), "exactly on MID's east boundary", "MID");
            Ids(PlayerMapScene.HitTest(scene, Math.BitDecrement(8.0), 15), "just outside LOW");
            Ids(PlayerMapScene.HitTest(scene, 20, 20), "bare ground never picks the nearest tree");
            Ids(PlayerMapScene.HitTest(scene, 25, 5), "a lone disc", "FAR");
            Ids(PlayerMapScene.HitTest(scene, double.NaN, 15), "a non-finite point");
            Ids(PlayerMapScene.HitTest(scene, 10.5, double.PositiveInfinity), "an infinite point");
            ReadOnlyScene(PlayerMapScene.HitTest(scene, 10.5, 15), "HitTest");
        });

        T.Add("map MS-04: every living fixture tree is hit at its own centre, and candidates[0] is the topmost disc there", () =>
        {
            var scene = PlayerMapScene.Build(Open().ReadPlayerView());
            int overlapping = 0;
            foreach (var item in scene.Where(i => i.Status == PlayerMapStatus.Living))
            {
                var hits = PlayerMapScene.HitTest(scene, item.U, item.V);
                T.True(hits.Any(h => h.TreeId == item.TreeId), item.TreeId + " is a candidate at its own centre");
                // Independent brute force over the whole scene.
                var containing = scene.Where(s => (item.U - s.U) * (item.U - s.U) + (item.V - s.V) * (item.V - s.V)
                    <= s.RadiusM * s.RadiusM).Select(s => s.PaintOrder).OrderDescending().ToArray();
                T.True(hits.Select(h => h.PaintOrder).SequenceEqual(containing), item.TreeId + ": every containing disc, topmost first");
                T.Eq(containing[0], hits[0].PaintOrder, item.TreeId + ": candidates[0] has the highest paint order there");
                if (hits.Count > 1) overlapping++;
            }
            T.True(overlapping > 0, "the fixture has overlapping crowns, so the ordering was exercised");
            Console.WriteLine($"MS-04 tree centres covered by more than one crown: {overlapping}");
        });

        T.Add("map MS-05: status mapping for pending, approved, felled (stump) and natural dead", () =>
        {
            var trees = new[]
            {
                Tree("A", -6, 0, 10f), Tree("B", 0, 0, 12f), Tree("C", 6, 0, 14f),
                Tree("D", 0, 6, 9f) with { Death = new AnnualDeath("D", 0, "self-thinning", "boundary") },
                Tree("E", 0, -6, 11f),
            };
            var c = new AnnualContinuation(new AnnualState(0, trees), Seed, Area);
            var player = new PlayerWorkPlanAdapter(c);
            var v = player.Read();
            T.Eq(PlayerMapStatus.Living, Item(v, "A").Status, "A living");
            var dead = Item(v, "D");
            T.Eq(PlayerMapStatus.NaturalDead, dead.Status, "D natural dead");
            T.Eq<int?>(0, dead.DeathYear, "natural death year");
            T.Eq("self-thinning", dead.DeathCause, "natural death cause");
            T.Eq(2.0, dead.RadiusM, "a natural dead tree keeps its crown radius");

            v = Ok(player.AddPending(Fence(v), "A", "pending"), "add A");
            T.Eq(PlayerMapStatus.PendingFell, Item(v, "A").Status, "A pending");
            T.Eq(PlayerMapStatus.Living, Item(v, "B").Status, "B still living");
            v = Ok(player.Approve(Fence(v)), "approve A");
            T.Eq(PlayerMapStatus.ApprovedFell, Item(v, "A").Status, "A approved");
            var plan = v.Approved!;
            v = Ok(player.Execute(Fence(v), plan.PlanId, plan.ExecutionKey), "execute A");
            var stump = Item(v, "A");
            T.Eq(PlayerMapStatus.Felled, stump.Status, "A felled");
            T.Eq<int?>(0, stump.DeathYear, "Fell year");
            T.Eq(FellCause.Harvest, stump.DeathCause, "Fell cause");
            T.Eq(0.35, stump.RadiusM, "the stump presentation radius");
            var scene = PlayerMapScene.Build(v);
            T.Eq(scene.Count - 1, stump.PaintOrder, "the only stump is drawn last");
            Ids(PlayerMapScene.HitTest(scene, stump.U, stump.V), "the stump marker", "A");
            Ids(PlayerMapScene.HitTest(scene, stump.U + 0.5, stump.V), "inside the former crown, outside the stump: the gap is open");

            v = Ok(player.AddPending(Fence(v), "B", "pending B"), "add B");
            T.Eq(PlayerMapStatus.PendingFell, Item(v, "B").Status, "B pending");
            v = Ok(player.Approve(Fence(v)), "approve B");
            T.Eq(PlayerMapStatus.ApprovedFell, Item(v, "B").Status, "B approved");
            T.Eq(PlayerMapStatus.Felled, Item(v, "A").Status, "A stays felled");
            T.Eq(PlayerMapStatus.NaturalDead, Item(v, "D").Status, "D stays natural dead");
        });

        T.Add("map MS-06: an adapter hit-test with a stale fence is refused after Execute; a fresh fence returns exact IDs", () =>
        {
            var c = Open();
            var player = new PlayerWorkPlanAdapter(c);
            var v = Ok(player.AddPending(Fence(player.Read()), "P30-045", "hit-test probe"), "add");
            v = Ok(player.Approve(Fence(v)), "approve");
            var target = Item(v, "P30-045");
            var fresh = player.HitTest(Fence(v), target.U, target.V);
            T.True(fresh.RefusalCode is null, "a current fence is accepted");
            T.True(fresh.Candidates.Any(h => h.TreeId == "P30-045" && h.Status == PlayerMapStatus.ApprovedFell), "exact ID with its status");
            T.True(Bytes(fresh.View).SequenceEqual(Bytes(v)), "the hit-test returns the current view and changes nothing");

            var plan = v.Approved!;
            var executed = Ok(player.Execute(Fence(v), plan.PlanId, plan.ExecutionKey), "execute");
            var root = ReadRoot(c);
            var stale = player.HitTest(Fence(v), target.U, target.V);
            T.Eq("PlayerSourceStale", stale.RefusalCode, "the pre-Execute fence is refused");
            T.Eq(0, stale.Candidates.Count, "a stale hit-test selects nothing");
            T.True(Bytes(stale.View).SequenceEqual(Bytes(player.Read())), "the refusal carries the fresh view");
            T.Eq(executed.PlanStateRevision, stale.View.PlanStateRevision, "the fresh view is the post-Execute state");
            T.True(ReferenceEquals(root, ReadRoot(c)), "a hit-test never publishes");
            T.Eq("PlayerSessionMismatch", player.HitTest(Fence(executed) with { SessionId = "other" }, target.U, target.V).RefusalCode,
                "another session is refused");
            T.Eq("PlayerPlanStateStale", player.HitTest(Fence(executed) with { PlanStateRevision = executed.PlanStateRevision - 1 },
                target.U, target.V).RefusalCode, "a stale plan state is refused");

            var after = player.HitTest(Fence(executed), target.U, target.V);
            T.True(after.RefusalCode is null, "a fresh fence is accepted after Execute");
            T.Eq("P30-045", after.Candidates[0].TreeId, "the stump is topmost at its own centre");
            T.Eq(PlayerMapStatus.Felled, after.Candidates[0].Status, "and it is shown as felled");
        });
    }

    private static AnnualTree Tree(string id, float x, float z, float heightM, float crownRadiusM = 2f) =>
        new(id, x, z, 30, 20f, heightM, crownRadiusM);

    private static PlayerMapItem Item(PlayerSessionView view, string treeId) =>
        PlayerMapScene.Build(view).Single(i => string.Equals(i.TreeId, treeId, StringComparison.Ordinal));

    private static PlayerSessionView Felled(AnnualContinuation continuation, params string[] treeIds)
    {
        var player = new PlayerWorkPlanAdapter(continuation);
        var view = player.Read();
        foreach (var id in treeIds) view = Ok(player.AddPending(Fence(view), id, "map probe"), "add " + id);
        view = Ok(player.Approve(Fence(view)), "approve");
        var plan = view.Approved!;
        return Ok(player.Execute(Fence(view), plan.PlanId, plan.ExecutionKey), "execute");
    }

    private static void Ids(IReadOnlyList<PlayerMapItem> hits, string what, params string[] expected) =>
        T.True(hits.Select(h => h.TreeId).SequenceEqual(expected),
            $"{what}: expected [{string.Join(",", expected)}], got [{string.Join(",", hits.Select(h => h.TreeId))}]");

    private static void ReadOnlyScene(IReadOnlyList<PlayerMapItem> items, string what)
    {
        T.True(items is not PlayerMapItem[], what + " does not expose an array");
        T.True(items is IList<PlayerMapItem> { IsReadOnly: true }, what + " returns a read-only list");
    }
}
