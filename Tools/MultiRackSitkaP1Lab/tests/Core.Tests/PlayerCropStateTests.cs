using System.Text.Json;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

internal static class PlayerCropStateTests
{
    internal static void Register(string manifestPath)
    {
        PlayerWorkPlanAdapter Open() => PlayerWorkPlanAdapter.Open(Path.GetDirectoryName(manifestPath)!);
        T.Add("crop CR-01: mark/unmark are fenced plan-only commits with immutable views and real map truth", () =>
        {
            var p = Open(); var v = p.Read(); string id = v.Frame.Trees[0].TreeId;
            string original = JsonSerializer.Serialize(v);
            var marked = Ok(p.MarkCrop(Fence(v), id));
            T.Eq(v.PlanStateRevision + 1, marked.PlanStateRevision, "mark bumps plan fence");
            T.Eq(v.Source, marked.Source, "source clocks unchanged");
            T.Eq(JsonSerializer.Serialize(v.Frame), JsonSerializer.Serialize(marked.Frame), "biology/work unchanged");
            T.Eq(id, marked.CropTreeIds.Single(), "server mark");
            T.True(PlayerMapScene.Build(marked).Single(t => t.TreeId == id).IsCrop, "map truth");
            T.True(((IList<string>)marked.CropTreeIds).IsReadOnly, "owned read-only view");
            var clear = Ok(p.UnmarkCrop(Fence(marked), id));
            T.Eq(marked.PlanStateRevision + 1, clear.PlanStateRevision, "unmark bump");
            T.Eq(0, clear.CropTreeIds.Count, "cleared");
            T.Eq(original, JsonSerializer.Serialize(v), "old view immutable");
            T.Eq(id, marked.CropTreeIds.Single(), "marked view remains immutable");
        });
        T.Add("crop CR-02: stale session/source/plan fences refuse mark and unmark with zero mutation", () =>
        {
            var p = Open(); var v = p.Read(); string id = v.Frame.Trees[0].TreeId;
            var marked = Ok(p.MarkCrop(Fence(v), id));
            foreach (var mark in new[] { true, false })
            {
                PlayerCommandResult Run(PlayerFence f) => mark ? p.MarkCrop(f, id) : p.UnmarkCrop(f, id);
                T.Eq("PlayerPlanStateStale", Run(Fence(v)).RefusalCode, "stale plan");
                T.Eq("PlayerSessionMismatch", Run(Fence(marked) with { SessionId = "foreign" }).RefusalCode, "foreign session");
                T.Eq("PlayerSourceStale", Run(Fence(marked) with { Source = marked.Source with { WorkRevision = 9 } }).RefusalCode, "stale source");
            }
            T.Eq(JsonSerializer.Serialize(marked), JsonSerializer.Serialize(p.Read()), "no refused mutation");
        });
        T.Add("crop CR-03: pending or approved Fell cannot become Crop and frozen work is unchanged", () =>
        {
            var p = Open(); var v = p.Read(); string id = v.Frame.Trees[0].TreeId;
            v = Ok(p.AddPending(Fence(v), id, "manual reason"));
            T.Eq("CropAlreadyPlannedForFell", p.MarkCrop(Fence(v), id).RefusalCode, "pending refusal");
            v = Ok(p.Approve(Fence(v))); string before = JsonSerializer.Serialize(v);
            T.Eq("CropAlreadyPlannedForFell", p.MarkCrop(Fence(v), id).RefusalCode, "approved refusal");
            T.Eq(before, JsonSerializer.Serialize(p.Read()), "frozen plan unchanged");
            v = Ok(p.CancelApproved(Fence(v))); v = Ok(p.MarkCrop(Fence(v), id));
            T.Eq(id, v.CropTreeIds.Single(), "cancel then mark allowed");
        });
        T.Add("crop CR-04: Crop refuses Fell; explicit unmark then Fell executes exactly once", () =>
        {
            var p = Open(); var v = p.Read(); string id = v.Frame.Trees[0].TreeId;
            v = Ok(p.MarkCrop(Fence(v), id));
            T.Eq("CropTreeRetained", p.AddPending(Fence(v), id, "reason").RefusalCode, "retention exclusive");
            v = Ok(p.UnmarkCrop(Fence(v), id)); v = Ok(p.AddPending(Fence(v), id, "explicit manual intent"));
            v = Ok(p.Approve(Fence(v))); var plan = v.Approved!; var fence = Fence(v);
            var executed = p.Execute(fence, plan.PlanId, plan.ExecutionKey);
            T.Eq(99, Ok(executed).Frame.Stand.LivingCount, "one Fell");
            T.True(p.Execute(fence, plan.PlanId, plan.ExecutionKey).Execution!.Replayed, "same receipt");
            T.Eq(1, p.Read().Frame.Events.Count, "one event");
            T.Eq("UnknownTreeId", p.MarkCrop(Fence(p.Read()), id).RefusalCode, "dead refusal");
        });
        T.Add("crop CR-05: unknown/dead/duplicate/unmarked IDs refuse without state change", () =>
        {
            var p = Open(); var v = p.Read(); string id = v.Frame.Trees[0].TreeId;
            T.Eq("UnknownTreeId", p.MarkCrop(Fence(v), "unknown").RefusalCode, "unknown mark");
            T.Eq("UnknownTreeId", p.UnmarkCrop(Fence(v), "unknown").RefusalCode, "unknown unmark");
            T.Eq("CropNotMarked", p.UnmarkCrop(Fence(v), id).RefusalCode, "unmarked");
            v = Ok(p.MarkCrop(Fence(v), id));
            T.Eq("CropAlreadyMarked", p.MarkCrop(Fence(v), id).RefusalCode, "duplicate");
            T.Eq(JsonSerializer.Serialize(v), JsonSerializer.Serialize(p.Read()), "no refused mutation");
        });
        T.Add("crop CR-06: Year+1 reconciles natural death in same publish; surviving marks persist", () =>
        {
            var trees = Array.AsReadOnly(Enumerable.Range(0, 12).Select(i => new AnnualTree("T" + i, i, 0, 30, 150, 20, 2)).ToArray());
            var c = new AnnualContinuation(new AnnualState(0, trees), 20261116, .09f);
            var p = new PlayerWorkPlanAdapter(c); var expected = AnnualRules.Calculate(new AnnualState(0, trees), 20261116, .09f);
            T.True(expected.Deaths.Count > 0, "test roster actually has natural deaths");
            var v = p.Read(); foreach (var t in trees) v = Ok(p.MarkCrop(Fence(v), t.TreeId));
            var marked = v; var next = Ok(p.Advance(Fence(v), false));
            T.Eq(marked.PlanStateRevision + 1, next.PlanStateRevision, "death reconciliation bump");
            foreach (var tree in next.Frame.Trees)
                T.Eq(tree.IsLiving, next.CropTreeIds.Contains(tree.TreeId), "only survivors retained");
            T.Eq(12, marked.CropTreeIds.Count, "old view unchanged");
            T.True(PlayerMapScene.Build(next).All(t => !t.IsCrop || next.Frame.Trees.Single(x => x.TreeId == t.TreeId).IsLiving), "no dead crop cue");
        });
        T.Add("crop CR-07: Crop state changes no annual/RNG outcome or map picking geometry for ten years", () =>
        {
            var a = Open(); var b = Open(); var v = a.Read();
            var plainScene = PlayerMapScene.Build(v); foreach (var t in v.Frame.Trees.Take(5)) v = Ok(a.MarkCrop(Fence(v), t.TreeId));
            var markedScene = PlayerMapScene.Build(v);
            foreach (var item in plainScene)
                T.Eq(string.Join(',', PlayerMapScene.HitTest(plainScene, item.U, item.V).Select(x => x.TreeId)),
                    string.Join(',', PlayerMapScene.HitTest(markedScene, item.U, item.V).Select(x => x.TreeId)), "same geometry/paint order");
            for (int year = 0; year < 10; year++)
            {
                var x = a.Advance(Fence(a.Read()), false); var y = b.Advance(Fence(b.Read()), false);
                T.Eq(JsonSerializer.Serialize(y.Annual), JsonSerializer.Serialize(x.Annual), "annual/RNG unchanged year " + year);
            }
        });
        T.Add("crop CR-08: unrelated crop changes never mutate an approved plan or lose marks on Execute", () =>
        {
            var p = Open(); var v = p.Read(); var ids = v.Frame.Trees.Take(2).Select(t => t.TreeId).ToArray();
            v = Ok(p.AddPending(Fence(v), ids[1], "other tree")); v = Ok(p.Approve(Fence(v)));
            string approved = JsonSerializer.Serialize(v.Approved); v = Ok(p.MarkCrop(Fence(v), ids[0]));
            T.Eq(approved, JsonSerializer.Serialize(v.Approved), "frozen plan identical");
            v = Ok(p.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey));
            T.Eq(ids[0], v.CropTreeIds.Single(), "crop survives other tree's Fell");
        });
    }
    private static PlayerFence Fence(PlayerSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);
    private static PlayerSessionView Ok(PlayerCommandResult r)
    { T.True(r.RefusalCode is null, r.RefusalCode ?? "committed"); return r.View; }
}
