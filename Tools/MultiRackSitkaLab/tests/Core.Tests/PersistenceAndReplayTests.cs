using SitkaLab.Testing;

namespace SitkaLab.Core.Tests;

public static class PersistenceAndReplayTests
{
    private static LabSession RoundTrip(LabSession s) => LabSession.FromFile(LabFile.Parse(LabFile.Serialize(s.ToFile())), Fx.Catalog);

    /// <summary>A rich history with nothing approved/pending outstanding: marks, plans of every terminal kind, an execution and steps.</summary>
    private static LabSession RichHistory()
    {
        var s = Fx.Demo();
        var rej = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.Proposed); Fx.Ok(s.RejectPlan(rej));
        var can = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.Approved); Fx.Ok(s.CancelPlan(can));
        var ex = Fx.Plan(s, Fx.Adults(s, 3), PlanStage.PendingWork); Fx.Ok(s.ExecutePlan(ex));
        Fx.Ok(s.AdvanceExperimentStep());
        var loose = Fx.Adults(s, 2); Fx.Ok(s.MarkTrees(loose)); Fx.Ok(s.UnmarkTrees(new[] { loose[0] }));
        Fx.Fails(s.ExecutePlan(ex), "PlanAlreadyExecuted", "audit event for the history");
        return s;
    }

    [Test]
    public static void EventChain_IsMonotonicHashLinkedAndReplayable()
    {
        var s = RichHistory();
        var ev = s.Events;
        T.True(ev.Count >= 15, "history is non-trivial: " + ev.Count);
        string expectedPrev = Fx.Catalog.TryGet("sitka-flat-demo-v1")!.SnapshotHash;
        for (int i = 0; i < ev.Count; i++)
        {
            T.Eq((long)(i + 1), ev[i].EventSeq, "monotonic gap-free eventSeq");
            T.Eq(expectedPrev, ev[i].PrevStateHash, $"event {i + 1}: prev == previous post");
            if (ev[i].Type == EventType.PlanExecutionRejected) T.Eq(ev[i].PrevStateHash, ev[i].PostStateHash, "audit-only event keeps the hash");
            else T.True(ev[i].PrevStateHash != ev[i].PostStateHash, $"event {i + 1} ({ev[i].Type}) changes the snapshot");
            expectedPrev = ev[i].PostStateHash;
        }
        T.Eq(expectedPrev, s.StateHash, "final post hash == current state hash");
        T.Eq(CanonicalJson.SnapshotHash(s.State), s.StateHash, "state hash == SHA-256 of canonical snapshot bytes");
        var replay = s.VerifyReplay();
        T.True(replay.Ok, "replay from the fixture: " + replay.Error);
        T.Eq(ev.Count, replay.Steps.Count, "every event re-derived");
        T.Eq(s.StateHash, replay.FinalHash, "replay reproduces the final hash");
        var types = ev.Select(e => e.Type).Distinct().ToHashSet();
        foreach (var t in new[] { EventType.TreeMarked, EventType.TreeUnmarked, EventType.PlanCreated, EventType.PlanProposed, EventType.PlanApproved, EventType.PlanQueued,
                     EventType.PlanRejected, EventType.PlanCancelled, EventType.TestWorkExecuted, EventType.PlanExecutionRejected, EventType.ExperimentStepAdvanced })
            T.True(types.Contains(t), "history covers " + t);
    }

    [Test]
    public static void CleanSaveLoad_ReproducesStateHashEventsAndBytes()
    {
        var s = RichHistory();
        byte[] saved = LabFile.Serialize(s.ToFile());
        var loaded = RoundTrip(s);
        T.Eq(s.StateHash, loaded.StateHash, "same state hash");
        T.Eq(s.Events.Count, loaded.Events.Count, "no activation events when nothing was approved/pending");
        T.True(LabFile.Serialize(loaded.ToFile()).AsSpan().SequenceEqual(saved), "re-saved bytes identical to the original save");
        T.Eq(CanonicalJson.Snapshot(s.State), CanonicalJson.Snapshot(loaded.State), "identical canonical snapshot");
        T.True(loaded.SessionEpoch != s.SessionEpoch, "load starts a new session epoch");
        T.True(loaded.VerifyReplay().Ok, "replay after load");
        var m = s.ActualMetrics(); T.Eq(m, loaded.ActualMetrics(), "identical Core metrics");
    }

    [Test]
    public static void Load_StalesApprovedAndPendingPlansByRecordedEvent_ChangingTheHash()
    {
        var s = Fx.Demo();
        var a = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.Approved);
        var b = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.PendingWork);
        var c = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.Proposed);
        string oldEpoch = s.SessionEpoch; string savedHash = s.StateHash; int savedEvents = s.Events.Count;
        byte[] file = LabFile.Serialize(s.ToFile());
        var loaded = LabSession.FromFile(LabFile.Parse(file), Fx.Catalog);
        T.Eq(PlanStage.Stale, Fx.PlanOf(loaded, a).Stage, "approved → STALE on load");
        T.Eq(PlanStage.Stale, Fx.PlanOf(loaded, b).Stage, "pending → STALE on load");
        T.Eq(PlanStage.Proposed, Fx.PlanOf(loaded, c).Stage, "proposed stays proposed");
        T.Eq(savedEvents + 2, loaded.Events.Count, "one PlanStale event per invalidated plan");
        T.True(loaded.Events.Skip(savedEvents).All(e => e.Type == EventType.PlanStale && e.Payload.Reason == "SessionLoaded"), "activation events are PlanStale/SessionLoaded");
        T.True(loaded.StateHash != savedHash, "the intentional stale transition is a different state with a different hash");
        T.True(loaded.SessionEpoch != oldEpoch, "new epoch");
        T.Eq(oldEpoch, Fx.PlanOf(loaded, a).ApprovalSessionEpoch, "historical approval epoch is kept from the event, not the live runtime");
        Fx.Fails(loaded.ExecutePlan(b), "PlanStale", "stale-on-load plan cannot execute");
        // and the stale state itself round-trips stably
        var again = RoundTrip(loaded);
        T.Eq(loaded.StateHash, again.StateHash, "stale state is stable across another save/load");
        T.True(again.VerifyReplay().Ok, "replay with historical approval epochs");
    }

    [Test]
    public static void FixtureReset_StartsFreshEpochAndDropsAllInMemoryPlans()
    {
        var old = Fx.Demo();
        var p = Fx.Plan(old, Fx.Adults(old, 2), PlanStage.PendingWork);
        var fresh = Fx.Demo(); // "reset": a new session from the checked-in fixture
        T.True(fresh.SessionEpoch != old.SessionEpoch, "reset gets a new opaque epoch");
        T.Eq(0, fresh.State.Plans.Count, "former approved/pending plans do not survive reset");
        Fx.Fails(fresh.ExecutePlan(p), "PlanNotFound", "old plan ID unknown after reset, even though forestRevision (0) is numerically identical");
        // Epoch guard even when the numeric revision matches: the same plan record under a different live epoch is rejected.
        var stolen = old.State.Clone();
        var injected = LabSession.ForTestingUnvalidated(stolen, LabSession.NewEpoch());
        Fx.Fails(injected.ExecutePlan(p), "EpochMismatch", "same revision, different epoch");
    }

    private static LabFileContent Copy(LabSession s) => LabFile.Parse(LabFile.Serialize(s.ToFile()));
    private static LabException LoadFails(LabFileContent f) => T.Throws<LabException>(() => LabSession.FromFile(LabFile.Parse(LabFile.Serialize(f)), Fx.Catalog), "load should fail");

    [Test]
    public static void Replay_DetectsTamperedHashesSequenceAndHistory()
    {
        var s = RichHistory();
        T.True(s.Events.Count > 8, "enough events to tamper with");

        var f = Copy(s); f.Events[3].PostStateHash = new string('a', 64);
        T.Eq("ReplayMismatch", LoadFails(f).Code, "tampered post hash");

        f = Copy(s); f.Events[4].PrevStateHash = new string('b', 64);
        T.Eq("ReplayMismatch", LoadFails(f).Code, "tampered prev hash");

        f = Copy(s); f.Events.RemoveAt(2);
        T.Eq("ReplayMismatch", LoadFails(f).Code, "deleted middle event (seq gap)");

        f = Copy(s); f.Events[5].EventSeq = 99;
        T.Eq("ReplayMismatch", LoadFails(f).Code, "bad sequence number");

        f = Copy(s); (f.Events[1], f.Events[2]) = (f.Events[2], f.Events[1]);
        T.Eq("ReplayMismatch", LoadFails(f).Code, "reordered events");

        f = Copy(s); f.Events.Add(new LabEvent { EventSeq = f.Events.Count + 1, Type = EventType.ExperimentStepAdvanced, Payload = new EventPayload { ExperimentStepAfter = 99, ForestRevisionAfter = 99 }, PrevStateHash = f.SnapshotHash, PostStateHash = f.SnapshotHash });
        T.Eq("ReplayMismatch", LoadFails(f).Code, "forged extra event");

        // Edit the saved snapshot (fell one more tree) and keep snapshotHash self-consistent: replay no longer reproduces it.
        f = Copy(s);
        var victim = f.Snapshot.Trees.First(t => t.Status == TreeStatus.Live && t.Mark == MarkState.Unmarked);
        victim.Status = TreeStatus.Felled;
        f.SnapshotHash = CanonicalJson.SnapshotHash(f.Snapshot);
        T.Eq("ReplayMismatch", LoadFails(f).Code, "snapshot not reproducible from fixture + event log");

        f = Copy(s); f.FixtureId = "oracle-quarter-ha";
        T.Eq("ReplayMismatch", LoadFails(f).Code, "events replayed against the wrong fixture");

        f = Copy(s); f.FixtureId = "no-such-fixture";
        T.Eq("UnknownFixture", LoadFails(f).Code, "fixtureId not in the checked-in catalog");

        f = Copy(s); f.SnapshotHash = new string('0', 64);
        T.Eq("HashMismatch", T.Throws<LabException>(() => LabFile.Parse(LabFile.Serialize(f)), "hash").Code, "malformed/mismatched snapshotHash");
    }

    [Test]
    public static void SaveFile_CarriesFormatMarkerSchemaAndSyntheticSourceClass()
    {
        var s = RichHistory();
        string text = System.Text.Encoding.UTF8.GetString(LabFile.Serialize(s.ToFile()));
        T.True(text.StartsWith("{\"format\":\"ccf-sitka-lab\",\"labSchemaVersion\":1,\"sourceClass\":\"Synthetic\",\"fixtureId\":\"sitka-flat-demo-v1\",\"snapshot\":{\"forestId\":"), "root order and header");
        T.True(text.Contains(",\"eventLog\":[{\"eventSeq\":1,\"eventType\":\"TreeMarked\",\"treeIds\":["), "event field order");
        T.True(text.EndsWith("\"}"), "snapshotHash is the last root property");
        T.False(text.Contains("dbhCm") || text.Contains("baM2") || text.Contains('.') && text.Contains("e+"), "no derived float metrics are persisted");
    }

    [Test]
    public static void ReplayUsesHistoricalApprovalEpoch_NotTheLiveRuntimeEpoch()
    {
        var s = Fx.Demo();
        var ex = Fx.Plan(s, Fx.Adults(s, 2), PlanStage.PendingWork);
        Fx.Ok(s.ExecutePlan(ex));
        var approval = s.Events.Single(e => e.Type == EventType.PlanApproved);
        T.Eq(s.SessionEpoch, approval.Payload.ApprovalSessionEpoch, "approval event records the epoch it happened under");
        var loaded = RoundTrip(s);
        T.True(loaded.SessionEpoch != approval.Payload.ApprovalSessionEpoch, "runtime epoch differs after load");
        T.True(loaded.VerifyReplay().Ok, "yet replay succeeds because it reads the epoch from the event payload");
    }
}
