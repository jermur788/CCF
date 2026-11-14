using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

public static class StateMachineTests
{
    private static List<(string Id, TreeStatus St)> Statuses(LabSession s) => s.State.Trees.Select(t => (t.TreeId, t.Status)).ToList();

    [Test]
    public static void MarkProposeApproveQueue_ChangeNothingBiological()
    {
        var s = Fx.Demo();
        var before = Statuses(s); var m0 = s.ActualMetrics();
        var ids = Fx.Adults(s, 5);
        void Same(string step)
        {
            T.Seq(before, Statuses(s), step + ": tree statuses");
            T.Eq(m0, s.ActualMetrics(), step + ": actual stand metrics");
            T.Eq(0L, s.State.ForestRevision, step + ": forest revision");
            T.False(s.Events.Any(e => e.Type == EventType.TestWorkExecuted), step + ": no successful work event");
        }
        Fx.Ok(s.MarkTrees(ids)); Same("mark");
        var planId = s.CreatePlan(ids).PlanId!; Same("create");
        Fx.Ok(s.ProposePlan(planId)); Same("propose");
        Fx.Ok(s.ApprovePlan(planId)); Same("approve");
        Fx.Ok(s.QueuePlan(planId)); Same("queue");
        T.Eq(5, s.Events.Count, "one event per snapshot-changing action");
        T.Eq(PlanStage.PendingWork, Fx.PlanOf(s, planId).Stage, "pending");
        T.True(s.State.Trees.Where(t => ids.Contains(t.TreeId)).All(t => t.Mark == MarkState.PlayerMarked && t.Status == TreeStatus.Live), "marked but still live");
    }

    [Test]
    public static void Execute_FellsAllAndOnlyTheListedTrees_Once()
    {
        var s = Fx.Demo();
        var ids = Fx.Adults(s, 6);
        var others = s.State.Trees.Where(t => !ids.Contains(t.TreeId)).Select(t => (t.TreeId, t.Status, t.Mark)).ToList();
        var m0 = s.ActualMetrics();
        double expectedRemoved = ids.Sum(id => Metrics.BasalAreaM2(s.State.FindTree(id)!.DbhMm));
        var preview = s.Preview(ids);
        string planId = Fx.Plan(s, ids, PlanStage.PendingWork);
        Fx.Ok(s.ExecutePlan(planId));
        T.True(ids.All(id => s.State.FindTree(id)!.Status == TreeStatus.Felled && s.State.FindTree(id)!.Mark == MarkState.Unmarked), "all listed felled and unmarked");
        T.Seq(others, s.State.Trees.Where(t => !ids.Contains(t.TreeId)).Select(t => (t.TreeId, t.Status, t.Mark)), "no other tree changed");
        var m1 = s.ActualMetrics();
        T.Eq(m0.EligibleLiveN - 6, m1.EligibleLiveN, "N reduced by exactly 6");
        T.Near(expectedRemoved, m0.BaM2 - m1.BaM2, 1e-9, "BA reduced by the removed stems");
        T.Near(preview.After.BaM2, m1.BaM2, 1e-12, "executed result equals Core preview");
        T.Eq(1L, s.State.ForestRevision, "revision incremented once");
        T.Eq(PlanStage.Executed, Fx.PlanOf(s, planId).Stage, "plan executed");
        T.Eq(1, s.Events.Count(e => e.Type == EventType.TestWorkExecuted), "exactly one TestWorkExecuted");
        T.Seq(ids, s.Events.Single(e => e.Type == EventType.TestWorkExecuted).TreeIds, "event lists the exact IDs");
    }

    [Test]
    public static void RepeatedExecution_IsRejectedAuditOnly_AndFellsNothingMore()
    {
        var s = Fx.Demo();
        string planId = Fx.Plan(s, Fx.Adults(s, 3), PlanStage.PendingWork);
        Fx.Ok(s.ExecutePlan(planId));
        string hash = s.StateHash; var m = s.ActualMetrics(); int events = s.Events.Count;
        Fx.Fails(s.ExecutePlan(planId), "PlanAlreadyExecuted", "second execution");
        T.Eq(events + 1, s.Events.Count, "exactly one audit event");
        var audit = s.Events[^1];
        T.Eq(EventType.PlanExecutionRejected, audit.Type, "audit event type");
        T.Eq(audit.PrevStateHash, audit.PostStateHash, "audit-only: equal prev/post hash");
        T.Eq(hash, s.StateHash, "forest state hash unchanged");
        T.Eq(m, s.ActualMetrics(), "metrics unchanged");
        T.Eq(1, s.Events.Count(e => e.Type == EventType.TestWorkExecuted), "still one successful work event");
    }

    [Test]
    public static void MarkStateIsSeparateFromBiologicalStatusAndPlanState()
    {
        var s = Fx.Demo();
        var dead = s.State.Trees.First(t => t.Status == TreeStatus.Dead).TreeId;
        var small = s.State.Trees.First(t => t.Status == TreeStatus.Live && t.DbhMm < s.State.AdultThresholdMm).TreeId;
        var adult = Fx.Adults(s, 1)[0];
        Fx.Fails(s.MarkTrees(new[] { dead }), "Rejected", "dead tree"); 
        Fx.Fails(s.MarkTrees(new[] { small }), "Rejected", "below-threshold tree is displayed but not actionable");
        Fx.Fails(s.MarkTrees(new[] { "NoSuchTree" }), "Rejected", "unknown tree");
        Fx.Fails(s.MarkTrees(new[] { adult, adult }), "TreeDuplicate", "duplicate in request");
        Fx.Fails(s.MarkTrees(Array.Empty<string>()), "EmptySelection", "empty");
        Fx.Fails(s.MarkTrees(new[] { "bad id" }), "InvalidId", "unsafe ID");
        T.Eq(0, s.Events.Count, "failed marks append no events");
        Fx.Ok(s.MarkTrees(new[] { adult }));
        T.Eq(MarkState.PlayerMarked, s.State.FindTree(adult)!.Mark, "marked");
        T.Eq(TreeStatus.Live, s.State.FindTree(adult)!.Status, "status independent of mark");
        Fx.Fails(s.MarkTrees(new[] { adult }), "Rejected", "already marked");
        Fx.Ok(s.UnmarkTrees(new[] { adult }));
        T.Eq(MarkState.Unmarked, s.State.FindTree(adult)!.Mark, "unmarked");
        Fx.Fails(s.UnmarkTrees(new[] { adult }), "Rejected", "not marked");
        // a mark request with one bad ID changes nothing (all-or-nothing)
        var two = Fx.Adults(s, 2);
        Fx.Fails(s.MarkTrees(new[] { two[0], dead, two[1] }), "Rejected", "mixed request");
        T.True(s.State.Trees.All(t => t.Mark == MarkState.Unmarked), "no partial marking");
    }

    [Test]
    public static void OneOpenPlanPerTree_AndPlansAreNeverEditedInPlace()
    {
        var s = Fx.Demo();
        var ids = Fx.Adults(s, 3);
        string a = Fx.Plan(s, ids, PlanStage.Draft);
        Fx.Fails(s.CreatePlan(ids), "Rejected", "same trees in a second plan");
        Fx.Fails(s.UnmarkTrees(new[] { ids[0] }), "Rejected", "cannot unmark a tree reserved by an open plan");
        T.Seq(ids, Fx.PlanOf(s, a).TreeIds, "plan list is exactly the original sorted IDs");
        Fx.Ok(s.CancelPlan(a));
        T.True(ids.All(id => s.State.FindTree(id)!.Mark == MarkState.Unmarked), "cancel releases reserved trees to UNMARKED");
        Fx.Fails(s.CreatePlan(ids), "Rejected", "released trees must be re-marked before a new plan");
        Fx.Ok(s.MarkTrees(ids));
        var b = s.CreatePlan(ids);
        Fx.Ok(b);
        T.True(b.PlanId != a, "a revised plan gets a NEW plan ID");
        T.Eq(PlanStage.Cancelled, Fx.PlanOf(s, a).Stage, "old plan stays CANCELLED");
        Fx.Fails(s.ExecutePlan(a), "PlanCancelledOrRejected", "cancelled plan cannot execute");
    }

    [Test]
    public static void IllegalTransitions_AreRejectedAndAppendNoStateChangingEvent()
    {
        var s = Fx.Demo();
        var ids = Fx.Adults(s, 2);
        string draft = Fx.Plan(s, ids, PlanStage.Draft);
        Fx.Fails(s.ApprovePlan(draft), "Rejected", "approve a DRAFT");
        Fx.Fails(s.QueuePlan(draft), "Rejected", "queue a DRAFT");
        Fx.Fails(s.RejectPlan(draft), "Rejected", "reject a DRAFT (only PROPOSED can be rejected)");
        Fx.Ok(s.ProposePlan(draft));
        Fx.Fails(s.ProposePlan(draft), "Rejected", "propose twice");
        Fx.Fails(s.QueuePlan(draft), "Rejected", "queue a PROPOSED plan");
        Fx.Ok(s.ApprovePlan(draft));
        Fx.Fails(s.ProposePlan(draft), "Rejected", "propose an APPROVED plan");
        Fx.Fails(s.RejectPlan(draft), "Rejected", "reject an APPROVED plan");
        Fx.Fails(s.ApprovePlan(draft), "Rejected", "approve twice");
        Fx.Fails(s.ExecutePlan(draft), "PlanNotPending", "execute before queueing");
        Fx.Ok(s.QueuePlan(draft));
        Fx.Fails(s.QueuePlan(draft), "Rejected", "queue a PENDING_WORK plan again (no idempotent re-queue)");
        Fx.Ok(s.CancelPlan(draft));
        Fx.Fails(s.CancelPlan(draft), "Rejected", "cancel a terminal plan");
        // A cancelled plan keeps its approval tokens and the revision/epoch are unchanged: only the state guard stops it re-entering the pipeline.
        Fx.Fails(s.QueuePlan(draft), "Rejected", "queue a CANCELLED plan");
        Fx.Fails(s.ApprovePlan(draft), "Rejected", "approve a CANCELLED plan");
        Fx.Fails(s.ProposePlan(draft), "Rejected", "propose a CANCELLED plan");
        string done = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.PendingWork); Fx.Ok(s.ExecutePlan(done));
        Fx.Fails(s.QueuePlan(done), "Rejected", "queue an EXECUTED plan");
        Fx.Fails(s.CancelPlan(done), "Rejected", "cancel an EXECUTED plan");
        Fx.Fails(s.ApprovePlan("P9999"), "PlanNotFound", "unknown plan");
        Fx.Fails(s.ExecutePlan("P9999"), "PlanNotFound", "unknown plan execution");
    }

    [Test]
    public static void RejectedProposal_ReleasesTreesAndChangesNothing()
    {
        var s = Fx.Demo(); var m0 = s.ActualMetrics();
        var ids = Fx.Adults(s, 4);
        string p = Fx.Plan(s, ids, PlanStage.Proposed);
        Fx.Ok(s.RejectPlan(p));
        T.Eq(PlanStage.Rejected, Fx.PlanOf(s, p).Stage, "rejected");
        T.True(ids.All(id => s.State.FindTree(id)!.Mark == MarkState.Unmarked), "released");
        T.Eq(m0, s.ActualMetrics(), "no forest change");
        Fx.Fails(s.ApprovePlan(p), "Rejected", "rejected plan is terminal");
    }

    [Test]
    public static void CalendarStep_StalesApprovedAndPendingPlans_AndNeverGrowsAnything()
    {
        var s = Fx.Demo();
        var a = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.Approved);
        var b = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.PendingWork);
        var c = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.Proposed);
        var growthBefore = s.State.Trees.Select(t => (t.TreeId, t.AgeAtInventoryYears, t.DbhMm, t.HeightMm, t.Status, t.XMm, t.YMm)).ToList();
        var m0 = s.ActualMetrics();
        Fx.Ok(s.AdvanceExperimentStep());
        T.Eq(1L, s.State.ExperimentStep, "step"); T.Eq(1L, s.State.ForestRevision, "revision incremented");
        T.Eq(PlanStage.Stale, Fx.PlanOf(s, a).Stage, "approved → STALE"); T.Eq(PlanStage.Stale, Fx.PlanOf(s, b).Stage, "pending → STALE");
        T.Eq(PlanStage.Proposed, Fx.PlanOf(s, c).Stage, "proposed plan is not approved, so it is unaffected");
        T.Seq(growthBefore, s.State.Trees.Select(t => (t.TreeId, t.AgeAtInventoryYears, t.DbhMm, t.HeightMm, t.Status, t.XMm, t.YMm)), "age, DBH, height, status and position unchanged");
        T.Eq(m0, s.ActualMetrics(), "stand metrics unchanged (NO BIOLOGICAL GROWTH)");
        T.True(s.Events.Where(e => e.Type == EventType.PlanStale).All(e => e.Payload.Reason == "ExperimentStepAdvanced"), "stale reason recorded");
        Fx.Fails(s.ExecutePlan(b), "PlanStale", "stale plan cannot execute");
        Fx.Fails(s.ApprovePlan(a), "Rejected", "stale plan cannot be re-approved in place");
        Fx.Fails(s.QueuePlan(a), "Rejected", "stale plan cannot be queued");
        T.Eq(0, s.State.Trees.Count(t => t.Status == TreeStatus.Felled), "nothing was felled");
        Fx.Ok(s.ApprovePlan(c)); // a still-PROPOSED plan may be approved later and captures the NEW revision
        T.Eq(1L, Fx.PlanOf(s, c).BaseForestRevision, "re-captured revision");
    }

    [Test]
    public static void ExecutingOnePlan_StalesOtherApprovedPlans()
    {
        var s = Fx.Demo();
        var ex = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.PendingWork);
        var other = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.Approved);
        Fx.Ok(s.ExecutePlan(ex));
        T.Eq(PlanStage.Stale, Fx.PlanOf(s, other).Stage, "other approved plan is stale after any execution");
        T.Eq("ForestRevisionChanged", s.Events[^1].Payload.Reason, "reason");
        T.True(Fx.PlanOf(s, other).TreeIds.All(id => s.State.FindTree(id)!.Mark == MarkState.Unmarked), "stale plan's trees released");
        Fx.Fails(s.ExecutePlan(other), "PlanStale", "cannot execute");
    }

    // ---- all-or-nothing, proven against deliberately corrupted in-memory state -----------------------------------
    private const string Epoch = "0123456789abcdef0123456789abcdef";

    private static (LabSession S, string Id, List<string> Ids) CorruptSession(Action<ForestSnapshot, List<string>> corrupt, long planRevision = 0, string epoch = Epoch)
    {
        var live = Fx.Demo(); // only used to pick real adult IDs
        var ids = Fx.Adults(live, 4);
        var snap = live.State.Clone();
        foreach (var id in ids) snap.FindTree(id)!.Mark = MarkState.PlayerMarked;
        var plan = new PlanRecord { PlanId = "P0001", Stage = PlanStage.PendingWork, TreeIds = new List<string>(ids), BaseForestRevision = planRevision, ApprovalSessionEpoch = epoch };
        snap.Plans.Add(plan);
        corrupt(snap, plan.TreeIds);
        return (LabSession.ForTestingUnvalidated(snap, Epoch), "P0001", ids);
    }

    private static void AssertAtomicRejection(string name, string code, (LabSession S, string Id, List<string> Ids) c)
    {
        var s = c.S;
        string hash = s.StateHash; var statuses = Statuses(s); var m = s.ActualMetrics();
        var r = s.ExecutePlan(c.Id);
        Fx.Fails(r, code, name);
        T.Seq(statuses, Statuses(s), name + ": ZERO trees felled (no partial execution)");
        T.Eq(m, s.ActualMetrics(), name + ": metrics unchanged");
        T.Eq(1, s.Events.Count, name + ": exactly one audit event");
        var e = s.Events[0];
        T.Eq(EventType.PlanExecutionRejected, e.Type, name + ": audit event type");
        T.Eq(code, e.Payload.Reason, name + ": reason recorded");
        T.Eq(hash, e.PrevStateHash, name + ": prev hash"); T.Eq(hash, e.PostStateHash, name + ": audit-only (post == prev)"); T.Eq(hash, s.StateHash, name + ": state hash unchanged");
        T.Eq(PlanStage.PendingWork, s.State.FindPlan(c.Id)!.Stage, name + ": plan left as it was");
        T.False(s.Events.Any(x => x.Type == EventType.TestWorkExecuted), name + ": no successful work recorded");
    }

    [Test]
    public static void Atomic_OneDeadTree_RejectsTheWholePlan()
    {
        var c = CorruptSession((snap, ids) => snap.FindTree(ids[2])!.Status = TreeStatus.Dead);
        AssertAtomicRejection("dead tree", "TreeNotLive", c);
        T.Seq(new[] { c.Ids[2] }, c.S.Events[0].TreeIds, "offending ID listed in the audit event");
    }

    [Test]
    public static void Atomic_OneAlreadyFelledTree_RejectsTheWholePlan() =>
        AssertAtomicRejection("felled tree", "TreeNotLive", CorruptSession((snap, ids) => snap.FindTree(ids[0])!.Status = TreeStatus.Felled));

    [Test]
    public static void Atomic_OneBelowThresholdTree_RejectsTheWholePlan() =>
        AssertAtomicRejection("below-threshold tree", "TreeBelowThreshold", CorruptSession((snap, ids) => snap.FindTree(ids[1])!.DbhMm = snap.AdultThresholdMm - 1));

    [Test]
    public static void Atomic_OneMissingTree_RejectsTheWholePlan() =>
        AssertAtomicRejection("missing tree", "TreeMissing", CorruptSession((snap, ids) => { ids[3] = "ZZ_not_there"; }));

    [Test]
    public static void Atomic_DuplicateIdInPlan_RejectsTheWholePlan() =>
        AssertAtomicRejection("duplicate tree ID", "TreeDuplicate", CorruptSession((snap, ids) => { ids[1] = ids[0]; }));

    [Test]
    public static void Atomic_TreeNotReservedToThePlan_RejectsTheWholePlan() =>
        AssertAtomicRejection("unreserved tree", "TreeNotReserved", CorruptSession((snap, ids) => snap.FindTree(ids[0])!.Mark = MarkState.Unmarked));

    [Test]
    public static void Atomic_ForestRevisionMismatch_RejectsTheWholePlan() =>
        AssertAtomicRejection("revision mismatch", "RevisionMismatch", CorruptSession((snap, ids) => snap.ForestRevision = 7));

    [Test]
    public static void Atomic_SessionEpochMismatch_RejectsTheWholePlan() =>
        AssertAtomicRejection("epoch mismatch", "EpochMismatch", CorruptSession((snap, ids) => { }, 0, "ffffffffffffffffffffffffffffffff"));

    [Test]
    public static void Atomic_ValidCorruptionFreePlan_ExecutesAllFour()
    {
        var c = CorruptSession((snap, ids) => { });
        Fx.Ok(c.S.ExecutePlan(c.Id));
        T.True(c.Ids.All(id => c.S.State.FindTree(id)!.Status == TreeStatus.Felled), "control: the same construction executes when nothing is wrong");
    }
}
