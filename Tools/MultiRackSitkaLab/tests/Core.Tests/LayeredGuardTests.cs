using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

/// <summary>
/// MULTI-004 F-4. Several guards protect the same rule from more than one layer, so removing one used to change nothing a test could see
/// (the M17 pattern). These tests exercise each layer ON ITS OWN, using histories that a hostile or buggy writer could really produce:
/// every forged event below carries a correct prevStateHash and a postStateHash recomputed from the forged result, so the hash chain
/// is intact and only the state-machine guards inside replay can refuse it. No layer is removed or weakened to make a test pass.
/// </summary>
public static class LayeredGuardTests
{
    private const string ZeroEpoch = "00000000000000000000000000000000";

    /// <summary>Appends one forged, hash-consistent event whose result is produced by <paramref name="forgedResult"/>, and returns the file content.</summary>
    private static LabFileContent Forge(LabSession s, EventType type, Action<ForestSnapshot> forgedResult, EventPayload payload, IReadOnlyList<string> ids)
    {
        var f = s.ToFile();
        var post = f.Snapshot.Clone(); forgedResult(post);
        string postHash = CanonicalJson.SnapshotHash(post);
        f.Events.Add(new LabEvent { EventSeq = f.Events.Count + 1, Type = type, TreeIds = ids.ToList(), Payload = payload, PrevStateHash = s.StateHash, PostStateHash = postHash });
        f.Snapshot = post; f.SnapshotHash = postHash;
        return f;
    }

    /// <summary>The forged file must be structurally valid (it parses) and then be refused by the replay guards with the given message fragment.</summary>
    private static void ReplayRefuses(string what, LabFileContent forged, string fragment)
    {
        LabFileContent parsed;
        try { parsed = LabFile.Parse(LabFile.Serialize(forged)); }
        catch (LabException ex) { throw new TestFailure($"{what}: the forged file should be well-formed so that only the replay guard decides, but parsing failed: {ex.Code}: {ex.Message}"); }
        var ex2 = T.Throws<LabException>(() => LabSession.FromFile(parsed, Fx.Catalog), what);
        T.Eq("ReplayMismatch", ex2.Code, what + ": refused by replay");
        T.True(ex2.Message.Contains(fragment, StringComparison.Ordinal), $"{what}: refused by the intended guard (expected '{fragment}' in '{ex2.Message}')");
    }

    private static void SetStaleResult(ForestSnapshot post, string planId)
    {
        var p = post.FindPlan(planId)!;
        foreach (var id in p.TreeIds) post.FindTree(id)!.Mark = MarkState.Unmarked;     // what Release() would do if the guard were missing
        p.Stage = PlanStage.Stale; p.BaseForestRevision ??= 0; p.ApprovalSessionEpoch ??= ZeroEpoch;
    }

    [Test]
    public static void ForgedHistory_RequeueOfAnAlreadyPendingPlan_IsRefusedByReplay()
    {
        var s = Fx.Demo(); var ids = Fx.Adults(s, 3); string pid = Fx.Plan(s, ids, PlanStage.PendingWork);
        ReplayRefuses("re-queue of PENDING_WORK (the M17 shape)", Forge(s, EventType.PlanQueued, _ => { }, new EventPayload { PlanId = pid }, ids), "only an APPROVED plan can be queued");
    }

    [Test]
    public static void ForgedHistory_StaleOnAPlanThatWasNeverApprovedOrAlreadyFinished_IsRefusedByReplay()
    {
        // X01: the PlanStale stage guard. The live API never produces these (it stales only APPROVED/PENDING_WORK plans), so replay is the only layer that can say no.
        foreach (var stage in new[] { PlanStage.Draft, PlanStage.Proposed })
        {
            var s = Fx.Demo(); var ids = Fx.Adults(s, 2); string pid = Fx.Plan(s, ids, stage);
            ReplayRefuses($"PlanStale forged on a {stage} plan", Forge(s, EventType.PlanStale, p => SetStaleResult(p, pid), new EventPayload { PlanId = pid, Reason = "SessionLoaded" }, ids), "only APPROVED or PENDING_WORK plans can become STALE");
        }
        {
            var s = Fx.Demo(); var ids = Fx.Adults(s, 2); string pid = Fx.Plan(s, ids, PlanStage.PendingWork); Fx.Ok(s.ExecutePlan(pid));
            ReplayRefuses("PlanStale forged on an EXECUTED plan", Forge(s, EventType.PlanStale, p => SetStaleResult(p, pid), new EventPayload { PlanId = pid, Reason = "SessionLoaded" }, ids), "only APPROVED or PENDING_WORK plans can become STALE");
        }
        {
            var s = Fx.Demo(); var ids = Fx.Adults(s, 2); string pid = Fx.Plan(s, ids, PlanStage.Proposed); Fx.Ok(s.CancelPlan(pid));
            ReplayRefuses("PlanStale forged on a CANCELLED plan", Forge(s, EventType.PlanStale, p => SetStaleResult(p, pid), new EventPayload { PlanId = pid, Reason = "SessionLoaded" }, ids), "only APPROVED or PENDING_WORK plans can become STALE");
        }
        {
            // control: the same forging technique on a legitimately APPROVED plan IS accepted, so the refusals above come from the stage guard and not from a malformed file
            var s = Fx.Demo(); var ids = Fx.Adults(s, 2); string pid = Fx.Plan(s, ids, PlanStage.Approved);
            var ok = LabFile.Parse(LabFile.Serialize(Forge(s, EventType.PlanStale, p => SetStaleResult(p, pid), new EventPayload { PlanId = pid, Reason = "SessionLoaded" }, ids)));
            T.Eq(PlanStage.Stale, LabSession.FromFile(ok, Fx.Catalog).State.FindPlan(pid)!.Stage, "control: a legal stale event replays");
        }
    }

    [Test]
    public static void ForgedHistory_ExecutionOfAPlanThatWasNeverQueued_IsRefusedByReplay()
    {
        foreach (var stage in new[] { PlanStage.Draft, PlanStage.Proposed, PlanStage.Approved })
        {
            var s = Fx.Demo(); var ids = Fx.Adults(s, 3); string pid = Fx.Plan(s, ids, stage);
            ReplayRefuses($"TestWorkExecuted forged on a {stage} plan", Forge(s, EventType.TestWorkExecuted, post =>
            {
                foreach (var id in ids) { var t = post.FindTree(id)!; t.Status = TreeStatus.Felled; t.Mark = MarkState.Unmarked; }
                var p = post.FindPlan(pid)!; p.Stage = PlanStage.Executed; p.BaseForestRevision ??= 0; p.ApprovalSessionEpoch ??= ZeroEpoch;
                post.ForestRevision = s.State.ForestRevision + 1;
            }, new EventPayload { PlanId = pid, ForestRevisionAfter = s.State.ForestRevision + 1 }, ids), "must be queued");
        }
    }

    [Test]
    public static void ForgedHistory_ApprovalOfAPlanThatWasNeverProposed_IsRefusedByReplay()
    {
        var s = Fx.Demo(); var ids = Fx.Adults(s, 2); string pid = Fx.Plan(s, ids, PlanStage.Draft);
        const string epoch = "0123456789abcdef0123456789abcdef";
        ReplayRefuses("PlanApproved forged on a DRAFT", Forge(s, EventType.PlanApproved, post =>
        {
            var p = post.FindPlan(pid)!; p.Stage = PlanStage.Approved; p.BaseForestRevision = post.ForestRevision; p.ApprovalSessionEpoch = epoch;
        }, new EventPayload { PlanId = pid, BaseForestRevision = s.State.ForestRevision, ApprovalSessionEpoch = epoch }, ids), "only a PROPOSED plan can be approved");
    }

    [Test]
    public static void ForgedHistory_SecondExecutionAndDeadTreeMarkAndUnmarkOfAReservedTree_AreRefusedByReplay()
    {
        {
            var s = Fx.Demo(); var ids = Fx.Adults(s, 2); string pid = Fx.Plan(s, ids, PlanStage.PendingWork); Fx.Ok(s.ExecutePlan(pid));
            ReplayRefuses("second TestWorkExecuted", Forge(s, EventType.TestWorkExecuted, post => post.ForestRevision++, new EventPayload { PlanId = pid, ForestRevisionAfter = s.State.ForestRevision + 1 }, ids), "already been executed");
        }
        {
            var s = Fx.Demo(); string dead = s.State.Trees.First(t => t.Status == TreeStatus.Dead).TreeId;
            // The forged result leaves the snapshot valid (a dead tree can never be PLAYER_MARKED in a valid snapshot, which is a separate, earlier layer), so only replay's own guard can refuse the event.
            ReplayRefuses("TreeMarked forged for a dead tree", Forge(s, EventType.TreeMarked, _ => { }, new EventPayload(), new[] { dead }), "cannot be marked");
        }
        {
            var s = Fx.Demo(); var ids = Fx.Adults(s, 2); Fx.Plan(s, ids, PlanStage.Proposed);
            ReplayRefuses("TreeUnmarked forged for a tree held by an open plan", Forge(s, EventType.TreeUnmarked, _ => { }, new EventPayload(), new[] { ids[0] }), "belongs to an open plan");
        }
    }

    [Test]
    public static void SnapshotValidator_RefusesATreeInTwoOpenPlans_OnItsOwn()
    {
        // X16: the validator's double-reservation check. Replay would also stop such a history, but a saved file's own snapshot block is validated at parse time,
        // before any replay, so the validator has to refuse it by itself.
        var s = Fx.Demo(); var ids = Fx.Adults(s, 2); Fx.Plan(s, ids, PlanStage.Proposed);
        var snap = s.State.Clone();
        snap.Plans.Add(new PlanRecord { PlanId = "P0002", Stage = PlanStage.Draft, TreeIds = new List<string> { ids[0] } });
        snap.Plans.Sort((a, b) => string.CompareOrdinal(a.PlanId, b.PlanId));
        var ex = T.Throws<LabException>(() => SnapshotValidator.ValidateSnapshot(snap), "validator, called directly");
        T.Eq("InvalidPlan", ex.Code, "code");
        T.True(ex.Message.Contains("belongs to two open plans"), "refused by the double-reservation check: " + ex.Message);

        var f = s.ToFile(); f.Snapshot = snap; f.SnapshotHash = CanonicalJson.SnapshotHash(snap);
        var viaParse = T.Throws<LabException>(() => LabFile.Parse(LabFile.Serialize(f)), "the same snapshot inside a file");
        T.Eq("InvalidPlan", viaParse.Code, "file parse refuses it before any replay");
        T.True(viaParse.Message.Contains("belongs to two open plans"), "same guard: " + viaParse.Message);

        // control: a plan on different trees is fine
        var fine = s.State.Clone(); fine.Plans.Add(new PlanRecord { PlanId = "P0002", Stage = PlanStage.Draft, TreeIds = new List<string> { Fx.Adults(s, 1, 2)[0] } });
        fine.FindTree(fine.Plans[1].TreeIds[0])!.Mark = MarkState.PlayerMarked;
        SnapshotValidator.ValidateSnapshot(fine);
    }
}
