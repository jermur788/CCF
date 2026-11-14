using System.Security.Cryptography;

namespace SitkaLab.Core;

public sealed record ReplayStep(long EventSeq, string EventType, bool Ok, string PrevHash, string PostHash, string? Error);

public sealed record ReplayResult(bool Ok, string FinalHash, IReadOnlyList<ReplayStep> Steps, string? Error)
{
    internal ForestSnapshot? Final { get; init; }
}

public static class Replayer
{
    /// <summary>Event-by-event replay from the original fixture snapshot, re-deriving and checking every SHA-256.</summary>
    public static ReplayResult Replay(ForestSnapshot start, IReadOnlyList<LabEvent> events)
    {
        var s = start.Clone();
        string hash = CanonicalJson.SnapshotHash(s);
        string? guardEpoch = null;
        var steps = new List<ReplayStep>();
        for (int i = 0; i < events.Count; i++)
        {
            var e = events[i];
            string? err = null;
            string post = hash;
            if (e.EventSeq != i + 1) err = $"eventSeq must be {i + 1} (monotonic, gap-free) but was {e.EventSeq}";
            else if (e.PrevStateHash != hash) err = "prevStateHash does not equal the previous post-state hash";
            else
            {
                if (e.Type == EventType.PlanApproved) guardEpoch = e.Payload.ApprovalSessionEpoch; // historical epoch comes from the event, never from the live runtime
                err = Engine.Apply(s, e, guardEpoch, true);
                if (err is null)
                {
                    post = CanonicalJson.SnapshotHash(s);
                    if (post != e.PostStateHash) err = "postStateHash does not match the re-derived canonical snapshot hash";
                }
            }
            steps.Add(new ReplayStep(e.EventSeq, e.Type.ToString(), err is null, e.PrevStateHash, e.PostStateHash, err));
            if (err is not null) return new ReplayResult(false, hash, steps, $"event {e.EventSeq}: {err}");
            hash = post;
        }
        return new ReplayResult(true, hash, steps, null) { Final = s };
    }
}

/// <summary>
/// One running laboratory session: the single forest authority plus its ordered, hash-chained event stream.
/// Not thread-safe; the host serialises access.
/// </summary>
public sealed class LabSession
{
    private ForestSnapshot _state;
    private readonly List<LabEvent> _events;
    private string _hash;
    private readonly string _baseFixtureId;
    private readonly ForestSnapshot _baseSnapshot;

    public string SessionEpoch { get; private set; }
    public string FixtureId => _baseFixtureId;
    public string StateHash => _hash;
    public IReadOnlyList<LabEvent> Events => _events;
    public ForestSnapshot State => _state;

    private LabSession(string fixtureId, ForestSnapshot baseSnapshot, ForestSnapshot state, List<LabEvent> events, string hash, string epoch)
    {
        _baseFixtureId = fixtureId; _baseSnapshot = baseSnapshot; _state = state; _events = events; _hash = hash; SessionEpoch = epoch;
    }

    /// <summary>Opaque runtime string, fresh on every start/reset/load (not a counter).</summary>
    internal static string NewEpoch() => Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

    public static LabSession FromFixture(LabFixture fx) =>
        new(fx.FixtureId, fx.Snapshot.Clone(), fx.Snapshot.Clone(), new List<LabEvent>(), fx.SnapshotHash, NewEpoch());

    /// <summary>Validates, replays and activates a saved lab file. Every approved/pending plan becomes STALE by a recorded event.</summary>
    public static LabSession FromFile(LabFileContent file, FixtureCatalog catalog)
    {
        var fx = catalog.TryGet(file.FixtureId) ?? throw new LabException("UnknownFixture", $"fixtureId '{file.FixtureId}' is not a checked-in synthetic fixture; the event history cannot be replayed");
        var replay = Replayer.Replay(fx.Snapshot, file.Events);
        if (!replay.Ok) throw new LabException("ReplayMismatch", replay.Error ?? "replay failed");
        if (replay.FinalHash != file.SnapshotHash) throw new LabException("ReplayMismatch", "replaying the event log from the fixture does not reproduce the saved snapshot hash");
        SnapshotValidator.ValidateSnapshot(replay.Final!);
        var session = new LabSession(fx.FixtureId, fx.Snapshot.Clone(), replay.Final!.Clone(), new List<LabEvent>(file.Events), replay.FinalHash, NewEpoch());
        var tx = new Tx(session);
        var err = session.StaleOpenApprovals(tx, "SessionLoaded");
        if (err is not null) throw new LabException("ActivationFailed", err);
        tx.Commit();
        return session;
    }

    /// <summary>Test seam: builds a session around a deliberately corrupt in-memory state to exercise execution-time re-validation.</summary>
    internal static LabSession ForTestingUnvalidated(ForestSnapshot state, string epoch)
    {
        var s = state.Clone();
        return new LabSession("injected", s.Clone(), s, new List<LabEvent>(), CanonicalJson.SnapshotHash(s), epoch);
    }

    public LabFileContent ToFile() => new()
    {
        FixtureId = _baseFixtureId,
        Snapshot = _state.Clone(),
        Events = new List<LabEvent>(_events),
        SnapshotHash = _hash,
    };

    public ReplayResult VerifyReplay() => Replayer.Replay(_baseSnapshot, _events);

    // ---- transaction helper: all appended events apply to a working clone and are committed together ----
    private sealed class Tx
    {
        private readonly LabSession _s;
        private readonly ForestSnapshot _work;
        private readonly List<LabEvent> _added = new();
        private string _hash;
        public ForestSnapshot Work => _work;
        public Tx(LabSession s) { _s = s; _work = s._state.Clone(); _hash = s._hash; }

        public string? Append(EventType type, IReadOnlyList<string> ids, EventPayload payload)
        {
            if (_s._events.Count + _added.Count >= LabConstants.MaxEvents) return "event log limit reached";
            var e = new LabEvent
            {
                EventSeq = _s._events.Count + _added.Count + 1,
                Type = type,
                TreeIds = new List<string>(ids),
                Payload = payload,
                PrevStateHash = _hash,
            };
            var err = Engine.Apply(_work, e, _s.SessionEpoch, false);
            if (err is not null) return err;
            e.PostStateHash = _hash = CanonicalJson.SnapshotHash(_work);
            _added.Add(e);
            return null;
        }

        public void Commit()
        {
            _s._state = _work;
            _s._events.AddRange(_added);
            _s._hash = _hash;
        }
    }

    private string? StaleOpenApprovals(Tx tx, string reason)
    {
        foreach (var id in tx.Work.Plans.Where(p => p.Stage is PlanStage.Approved or PlanStage.PendingWork).Select(p => p.PlanId).ToList())
        {
            var plan = tx.Work.FindPlan(id)!;
            var err = tx.Append(EventType.PlanStale, plan.TreeIds.ToList(), new EventPayload { PlanId = id, Reason = reason });
            if (err is not null) return err;
        }
        return null;
    }

    private OpResult Run(Func<Tx, string?> body, string? planId = null)
    {
        var tx = new Tx(this);
        var err = body(tx);
        if (err is not null) return OpResult.Fail("Rejected", err, null, planId);
        tx.Commit();
        return OpResult.Success(planId);
    }

    private static OpResult? NormaliseIds(IReadOnlyList<string> ids, out List<string> sorted)
    {
        sorted = new List<string>();
        if (ids.Count == 0) return OpResult.Fail("EmptySelection", "no tree IDs were supplied");
        if (ids.Count > LabConstants.MaxTrees) return OpResult.Fail("TooManyIds", "too many tree IDs");
        var set = new HashSet<string>(StringComparer.Ordinal);
        var dups = new List<string>();
        foreach (var id in ids)
        {
            if (!LabConstants.IsSafeId(id)) return OpResult.Fail("InvalidId", "a tree ID has an invalid form");
            if (!set.Add(id)) dups.Add(id);
        }
        if (dups.Count > 0) return OpResult.Fail("TreeDuplicate", "the tree ID list contains duplicates", dups.Distinct().ToList());
        sorted = set.ToList();
        sorted.Sort(StringComparer.Ordinal);
        return null;
    }

    public OpResult MarkTrees(IReadOnlyList<string> ids) => TreeMarkOp(ids, EventType.TreeMarked);
    public OpResult UnmarkTrees(IReadOnlyList<string> ids) => TreeMarkOp(ids, EventType.TreeUnmarked);

    private OpResult TreeMarkOp(IReadOnlyList<string> ids, EventType type)
    {
        var bad = NormaliseIds(ids, out var sorted);
        if (bad is not null) return bad;
        return Run(tx => tx.Append(type, sorted, new EventPayload()));
    }

    private string NextPlanId()
    {
        for (int n = _state.Plans.Count + 1; ; n++)
        {
            string id = $"P{n:D4}";
            if (_state.FindPlan(id) is null) return id;
        }
    }

    public OpResult CreatePlan(IReadOnlyList<string> ids)
    {
        var bad = NormaliseIds(ids, out var sorted);
        if (bad is not null) return bad;
        string planId = NextPlanId();
        return Run(tx => tx.Append(EventType.PlanCreated, sorted, new EventPayload { PlanId = planId }), planId);
    }

    private OpResult PlanOp(string planId, EventType type, Func<PlanRecord, EventPayload> payload)
    {
        var plan = _state.FindPlan(planId);
        if (plan is null) return OpResult.Fail("PlanNotFound", $"plan '{planId}' does not exist");
        return Run(tx => tx.Append(type, plan.TreeIds.ToList(), payload(plan)), planId);
    }

    public OpResult ProposePlan(string planId) => PlanOp(planId, EventType.PlanProposed, p => new EventPayload { PlanId = p.PlanId });
    public OpResult RejectPlan(string planId) => PlanOp(planId, EventType.PlanRejected, p => new EventPayload { PlanId = p.PlanId });
    public OpResult CancelPlan(string planId) => PlanOp(planId, EventType.PlanCancelled, p => new EventPayload { PlanId = p.PlanId });
    public OpResult QueuePlan(string planId) => PlanOp(planId, EventType.PlanQueued, p => new EventPayload { PlanId = p.PlanId });

    public OpResult ApprovePlan(string planId) => PlanOp(planId, EventType.PlanApproved, p => new EventPayload
    {
        PlanId = p.PlanId,
        BaseForestRevision = _state.ForestRevision,
        ApprovalSessionEpoch = SessionEpoch,
    });

    /// <summary>Execute Test Work: strictly all-or-nothing. A failed check appends one audit-only event and fells nothing.</summary>
    public OpResult ExecutePlan(string planId)
    {
        var plan = _state.FindPlan(planId);
        if (plan is null) return OpResult.Fail("PlanNotFound", $"plan '{planId}' does not exist");
        var bad = Engine.CheckExecute(_state, plan, SessionEpoch);
        if (bad is not null)
        {
            var audit = Run(tx => tx.Append(EventType.PlanExecutionRejected, bad.OffendingIds, new EventPayload { PlanId = planId, Reason = bad.Reason }), planId);
            if (!audit.Ok) return OpResult.Fail("Rejected", "execution rejected and the audit event could not be recorded: " + audit.Message, null, planId);
            return OpResult.Fail(bad.Reason, bad.Message + " — no tree was felled (audit-only event recorded).", bad.OffendingIds, planId);
        }
        return Run(tx =>
        {
            var err = tx.Append(EventType.TestWorkExecuted, plan.TreeIds.ToList(), new EventPayload { PlanId = planId, ForestRevisionAfter = _state.ForestRevision + 1 });
            return err ?? StaleOpenApprovals(tx, "ForestRevisionChanged");
        }, planId);
    }

    /// <summary>"Advance experiment step (calendar only)": no age, DBH, height, mortality or recruitment change.</summary>
    public OpResult AdvanceExperimentStep() => Run(tx =>
    {
        var err = tx.Append(EventType.ExperimentStepAdvanced, Array.Empty<string>(),
            new EventPayload { ExperimentStepAfter = _state.ExperimentStep + 1, ForestRevisionAfter = _state.ForestRevision + 1 });
        return err ?? StaleOpenApprovals(tx, "ExperimentStepAdvanced");
    });

    public RemovalPreview Preview(IReadOnlyList<string> ids) => Metrics.PreviewRemoval(_state, ids);
    public StandMetrics ActualMetrics() => Metrics.Compute(_state);
}
