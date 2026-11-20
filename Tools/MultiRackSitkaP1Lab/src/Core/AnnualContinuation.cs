using System.Linq;

namespace SitkaP1.Core;

internal sealed record ContinuationSnapshot(AnnualState State, int Revision);

/// <summary>An identical same-process execution key, kept so a retry can be answered without another Fell.</summary>
internal sealed record ExecutedWork(WorkSourceToken Source, IReadOnlyList<WorkOrder> Orders, WorkEvent Event);

/// <summary>
/// One immutable session commit root. Roster, annual revision, work revision, event ledger, executed-key map and any
/// unexecuted work are published by a single reference assignment, so a reader can never observe a Fell roster without
/// its event, or an event without its work revision. The player plan-state revision and the per-year annual history
/// (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §6.3) travel in the same assignment; neither is part of any snapshot, report or digest.
/// </summary>
internal sealed record WorkRoot(
    ContinuationSnapshot Snapshot,
    int WorkRevision,
    IReadOnlyList<WorkEvent> Events,
    IReadOnlyDictionary<string, ExecutedWork> Executed,
    WorkPlan? ApprovedUnexecuted,
    IReadOnlyList<WorkOrder> Pending,
    int PlanStateRevision,
    IReadOnlyList<AnnualYearRecord> AnnualHistory);

/// <summary>
/// In-memory, no-durability Stage 5 session: genuine annual biology plus the G2 manual Work Plan → approved batch →
/// one atomic typed Fell transaction → real following-year survivor ecology.
/// </summary>
internal sealed class AnnualContinuation
{
    private readonly object sync = new();
    private readonly int simulationSeed;
    private readonly float areaHa;
    private readonly WorkSourceIdentity identity;
    private WorkRoot root;

    internal AnnualContinuation(AnnualState initial, int simulationSeed, float areaHa, WorkSourceIdentity? identity = null)
    {
        this.simulationSeed = simulationSeed;
        this.areaHa = areaHa;
        this.identity = identity ?? new WorkSourceIdentity(
            P1DomainWire.Name(P1Domain.DerivedP1ThirtyMetreExperimentV1), "synthetic", "synthetic", "synthetic",
            DerivedManifest.RngModelVersion, DerivedManifest.GrowthModelVersion, DerivedManifest.P1AlgorithmVersion,
            ManifestGenerator.ActionThresholdMm);
        // Own the collection so an external mutable list cannot change a committed snapshot.
        var snapshot = new ContinuationSnapshot(initial with { Trees = Array.AsReadOnly(initial.Trees.ToArray()) }, 0);
        root = new WorkRoot(snapshot, 0, Array.Empty<WorkEvent>(),
            new Dictionary<string, ExecutedWork>(StringComparer.Ordinal), null, Array.Empty<WorkOrder>(),
            0, Array.Empty<AnnualYearRecord>());
    }

    internal ContinuationSnapshot Snapshot
    {
        get { lock (sync) return root.Snapshot; }
    }

    /// <summary>The immutable committed-current frame a planner or reader may bind to.</summary>
    internal CommittedFrame Frame
    {
        get
        {
            lock (sync)
                return new CommittedFrame(ManagementScope.WholeUndividedThirtyMetres, identity.Domain, identity.FixtureId,
                    identity.FixtureFileSha256, identity.FixtureContentHash,
                    root.Snapshot.State.EcologicalYear, root.Snapshot.Revision, root.WorkRevision,
                    Array.AsReadOnly(root.Snapshot.State.Trees.ToArray()),
                    WorkRules.Summarize(root.Snapshot.State.Trees, areaHa),
                    Array.AsReadOnly(root.Events.ToArray()));
        }
    }

    internal WorkSourceToken CurrentToken
    {
        get { lock (sync) return Token(root); }
    }

    internal bool HasUnexecutedWork
    {
        get { lock (sync) return root.Pending.Count > 0 || root.ApprovedUnexecuted is not null; }
    }

    /// <summary>Add one manually chosen living exact ID to the pending batch. This is not work and fells nothing.</summary>
    internal void AddPendingFell(string treeId, string reason)
    {
        lock (sync)
        {
            if (string.IsNullOrWhiteSpace(treeId))
                throw new P1Refusal("UnknownTreeId", "a tree ID is required");
            if (string.IsNullOrWhiteSpace(reason))
                throw new P1Refusal("WorkReasonRequired", "every manual selection needs a recorded reason");
            if (root.ApprovedUnexecuted is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved",
                    "an approved batch is awaiting execution; cancel it before editing");
            if (root.Pending.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)))
                throw new P1Refusal("WorkDuplicateTreeId", "the tree ID is already in the pending batch");
            var tree = FindLiving(treeId);
            if (tree is null) throw new P1Refusal("UnknownTreeId", "no live tree has that exact case-sensitive ID");

            var pending = root.Pending.Append(new WorkOrder(treeId, WorkAction.Fell, reason)).ToArray();
            root = root with { Pending = Array.AsReadOnly(pending), PlanStateRevision = checked(root.PlanStateRevision + 1) };
        }
    }

    /// <summary>Remove one pending order before approval. Never touches biology.</summary>
    internal void RemovePendingFell(string treeId)
    {
        lock (sync)
        {
            if (root.ApprovedUnexecuted is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "the batch is approved; cancel it before editing");
            var pending = root.Pending.Where(order => !string.Equals(order.TreeId, treeId, StringComparison.Ordinal)).ToArray();
            if (pending.Length == root.Pending.Count)
                throw new P1Refusal("UnknownWorkOrder", "the tree ID is not in the pending batch");
            root = root with { Pending = Array.AsReadOnly(pending), PlanStateRevision = checked(root.PlanStateRevision + 1) };
        }
    }

    /// <summary>
    /// The single commitment gate. Produces an immutable batch bound to the current source, year and both revisions.
    /// Approval alone never fells a tree; <see cref="Execute"/> is a separate explicit action.
    /// </summary>
    internal WorkPlan ApprovePlan(string planId)
    {
        lock (sync)
        {
            if (string.IsNullOrWhiteSpace(planId))
                throw new P1Refusal("WorkPlanIdRequired", "an approved batch needs a stable identity");
            if (root.ApprovedUnexecuted is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved batch is already awaiting execution");
            if (root.Pending.Count == 0)
                throw new P1Refusal("WorkEmptyPlan", "there is no pending work to approve");

            var plan = new WorkPlan(planId, Token(root), Array.AsReadOnly(root.Pending.ToArray()));
            root = root with
            {
                ApprovedUnexecuted = plan, Pending = Array.Empty<WorkOrder>(),
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return plan;
        }
    }

    /// <summary>Withdraw an approved but unexecuted batch. Zero Fell, zero history.</summary>
    internal void CancelApprovedPlan()
    {
        lock (sync)
        {
            if (root.ApprovedUnexecuted is null)
                throw new P1Refusal("WorkPlanNotApproved", "there is no approved batch to cancel");
            root = root with { ApprovedUnexecuted = null, PlanStateRevision = checked(root.PlanStateRevision + 1) };
        }
    }

    /// <summary>
    /// Execute one explicitly approved whole batch. All-or-nothing: every validation runs before the single root
    /// publish, and <paramref name="beforePublish"/> is the test-only seam after all calculations and before any
    /// roster, event, work-revision or idempotency change.
    /// </summary>
    internal WorkExecutionResult Execute(WorkPlan plan, string executionKey, Action? beforePublish = null)
    {
        lock (sync)
        {
            if (string.IsNullOrWhiteSpace(executionKey))
                throw new P1Refusal("WorkExecutionKeyRequired", "an execution key is required");
            if (plan is null)
                throw new P1Refusal("WorkPlanNotApproved", "only an explicitly approved batch can execute");

            if (root.Executed.TryGetValue(executionKey, out var executed))
            {
                if (executed.Source == plan.Source && executed.Orders.SequenceEqual(plan.Orders))
                    return new WorkExecutionResult(executionKey, executed.Event.EcologicalYear, executed.Event.WorkRevision,
                        executed.Event, Replayed: true);
                throw new P1Refusal("WorkExecutionKeyConflict",
                    "this execution key was already used for a different payload");
            }

            if (root.ApprovedUnexecuted is null || !ReferenceEquals(root.ApprovedUnexecuted, plan))
                throw new P1Refusal("WorkPlanNotApproved", "the batch must be approved immediately before execution");
            if (plan.Source != Token(root))
                throw new P1Refusal("WorkPlanStaleSource",
                    "the source, year or revision moved since approval; review and approve the batch again");
            if (plan.Orders.Count == 0)
                throw new P1Refusal("WorkEmptyPlan", "the approved batch contains no orders");

            var seen = new HashSet<string>(StringComparer.Ordinal);
            var targets = new HashSet<string>(StringComparer.Ordinal);
            foreach (var order in plan.Orders)
            {
                if (!seen.Add(order.TreeId))
                    throw new P1Refusal("WorkDuplicateTreeId", "the approved batch contains a duplicate tree ID");
                if (FindLiving(order.TreeId) is null)
                    throw new P1Refusal("WorkTreeNotLive", "a selected ID is not a live exact-case tree in the current source");
                targets.Add(order.TreeId);
            }

            var state = root.Snapshot.State;
            WorkRules.RequireNonEmptyStand(state, plan.Orders);

            int workRevision = checked(root.WorkRevision + 1);
            int year = state.EcologicalYear;
            var fell = new List<FellRecord>(plan.Orders.Count);
            var trees = new AnnualTree[state.Trees.Count];
            for (int i = 0; i < state.Trees.Count; i++)
            {
                var tree = state.Trees[i];
                if (tree.IsLiving && targets.Contains(tree.TreeId))
                {
                    var order = plan.Orders.First(o => string.Equals(o.TreeId, tree.TreeId, StringComparison.Ordinal));
                    var death = new AnnualDeath(tree.TreeId, year, FellCause.Harvest, FellCause.Mechanism);
                    trees[i] = tree with { Death = death };
                    fell.Add(new FellRecord(tree.TreeId, FellCause.Harvest, FellCause.Mechanism, order.Reason,
                        year, workRevision, executionKey, plan.PlanId));
                }
                else
                {
                    trees[i] = tree;
                }
            }

            var before = WorkRules.Summarize(state.Trees, areaHa);
            var after = WorkRules.Summarize(trees, areaHa);
            var committed = new WorkEvent(executionKey, plan.PlanId, year, workRevision, before, after,
                Array.AsReadOnly(fell.OrderBy(record => record.TreeId, StringComparer.Ordinal).ToArray()));

            // Test-only fault seam: roster, event and revision are complete, but nothing has been published.
            beforePublish?.Invoke();

            var snapshot = new ContinuationSnapshot(new AnnualState(year, Array.AsReadOnly(trees)), root.Snapshot.Revision);
            var executedMap = new Dictionary<string, ExecutedWork>(root.Executed, StringComparer.Ordinal)
            {
                [executionKey] = new ExecutedWork(plan.Source, Array.AsReadOnly(plan.Orders.ToArray()), committed),
            };
            // Q-2: a published execution clears the approval, so the plan-state revision moves by exactly one.
            root = new WorkRoot(snapshot, workRevision, Array.AsReadOnly(root.Events.Append(committed).ToArray()),
                executedMap, null, Array.Empty<WorkOrder>(), checked(root.PlanStateRevision + 1), root.AnnualHistory);

            return new WorkExecutionResult(executionKey, year, workRevision, committed, Replayed: false);
        }
    }

    internal AnnualReport Advance(int expectedYear, int expectedRevision, Action? beforeCommit = null)
    {
        lock (sync)
        {
            if (root.Pending.Count > 0 || root.ApprovedUnexecuted is not null)
                throw new P1Refusal("WorkPendingConsentRequired",
                    "pending or approved-but-unexecuted work must be cancelled with explicit consent before the year advances");
            if (root.WorkRevision != 0)
                throw new P1Refusal("ContinuationWorkRevisionRequired",
                    "after a Fell, advancing the year requires an explicit expected work revision; use the work-revision overload");
            return AdvanceLocked(expectedYear, expectedRevision, 0, beforeCommit);
        }
    }

    /// <summary>
    /// Genuine annual step. With unexecuted work, explicit consent stale-cancels every order at zero Fell and then runs
    /// real biology on actual survivors; without consent it refuses with zero mutation.
    /// </summary>
    internal AnnualReport Advance(int expectedYear, int expectedRevision, int expectedWorkRevision,
        bool consentToCancelWork, Action? beforeCommit = null)
    {
        lock (sync)
        {
            bool hasWork = root.Pending.Count > 0 || root.ApprovedUnexecuted is not null;
            if (hasWork && !consentToCancelWork)
                throw new P1Refusal("WorkPendingConsentRequired",
                    "advancing the year with unexecuted work needs explicit consent to cancel it at zero Fell");
            return AdvanceLocked(expectedYear, expectedRevision, expectedWorkRevision, beforeCommit, cancelWork: hasWork);
        }
    }

    private AnnualReport AdvanceLocked(int expectedYear, int expectedRevision, int expectedWorkRevision,
        Action? beforeCommit, bool cancelWork = false)
    {
        var previous = root.Snapshot;
        if (expectedYear != previous.State.EcologicalYear)
            throw new P1Refusal("ContinuationYearMismatch", "expected ecological year is stale");
        if (expectedRevision != previous.Revision)
            throw new P1Refusal("ContinuationRevisionMismatch", "expected revision is stale");
        if (expectedWorkRevision != root.WorkRevision)
            throw new P1Refusal("ContinuationWorkRevisionMismatch", "expected work revision is stale");
        if (!previous.State.Trees.Any(tree => tree.IsLiving))
            throw new P1Refusal("ContinuationNoLivingStems", "no living stems remain to advance");

        int revision = checked(previous.Revision + 1);
        var report = AnnualRules.Calculate(previous.State, simulationSeed, areaHa);
        var next = new ContinuationSnapshot(new AnnualState(report.YearAfter,
            Array.AsReadOnly(report.Trees.Select(row => row.After).ToArray())), revision);
        // Source-linked history of this committed year (spec §0.4 Q-6): the report's own before/after and natural deaths.
        var yearRecord = new AnnualYearRecord(report.YearBefore, report.YearAfter, revision, root.WorkRevision,
            report.Before, report.After, Array.AsReadOnly(report.Deaths.ToArray()), cancelWork);
        var history = Array.AsReadOnly(root.AnnualHistory.Append(yearRecord).ToArray());
        // Test-only fault seam: all calculations completed, neither fence nor state published yet.
        beforeCommit?.Invoke();
        // Only a consented cancellation changes the plan state; a no-work year leaves the plan-state revision as it was.
        root = cancelWork
            ? root with
            {
                Snapshot = next, ApprovedUnexecuted = null, Pending = Array.Empty<WorkOrder>(),
                PlanStateRevision = checked(root.PlanStateRevision + 1), AnnualHistory = history,
            }
            : root with { Snapshot = next, AnnualHistory = history };
        return report;
    }

    private WorkSourceToken Token(WorkRoot current) => new(identity.Domain, identity.FixtureId,
        identity.FixtureFileSha256, identity.FixtureContentHash, ManagementScope.WholeUndividedThirtyMetres,
        current.Snapshot.State.EcologicalYear, current.Snapshot.Revision, current.WorkRevision,
        identity.RngModelVersion, identity.GrowthModelVersion, identity.P1AlgorithmVersion,
        identity.ActionEligibleDbhThresholdMm);

    private AnnualTree? FindLiving(string treeId)
    {
        foreach (var tree in root.Snapshot.State.Trees)
            if (string.Equals(tree.TreeId, treeId, StringComparison.Ordinal) && tree.IsLiving)
                return tree;
        return null;
    }

    // ------------------------------------------------------------------------------------------------------------------
    // Player session facade (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §6). Each read and command below takes `sync` exactly
    // once, so the fence it checks, the transition it performs and the view it returns all come from one root. The fenced
    // commands call the legacy methods above (the lock is reentrant); apart from the fence they add only the §6.5 rules.
    // ------------------------------------------------------------------------------------------------------------------

    // One random identity per in-memory session. It is never written into a snapshot, report or digest.
    private readonly string sessionId =
        Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

    // Fixture rule p1-square30-corner-to-centred-xz-v1 places every tree in [-15, 15) m on both axes (spec §4.3).
    private const double MapMinM = -DerivedManifest.CentreMm / 1000.0;
    private const double MapSideM = DerivedManifest.SideMm / 1000.0;

    internal string SessionId => sessionId;

    /// <summary>The whole player view, built from ONE root reference under ONE lock acquisition (spec §6.2).</summary>
    internal PlayerSessionView ReadPlayerView()
    {
        lock (sync) return ViewOf(root);
    }

    internal PlayerCommandResult AddPending(PlayerFence fence, string treeId, string reason)
    {
        lock (sync)
        {
            RequireFence(fence);
            AddPendingFell(treeId, reason);
            return WithCurrentView(null, null);
        }
    }

    internal PlayerCommandResult RemovePending(PlayerFence fence, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            RemovePendingFell(treeId);
            return WithCurrentView(null, null);
        }
    }

    /// <summary>Approve the whole pending batch under an engine-minted PlanId; a caller cannot choose one. Zero Fell.</summary>
    internal PlayerCommandResult Approve(PlayerFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            // n is the plan-state revision this approval publishes, so the PlanId is unique within the session.
            ApprovePlan(FormattableString.Invariant($"{sessionId}:plan:{checked(root.PlanStateRevision + 1)}"));
            return WithCurrentView(null, null);
        }
    }

    /// <summary>Withdraw the approved batch. Its orders are not returned to pending; the player re-adds them.</summary>
    internal PlayerCommandResult CancelApproved(PlayerFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            CancelApprovedPlan();
            return WithCurrentView(null, null);
        }
    }

    /// <summary>
    /// Execute the engine's own approved plan (spec §6.5); the caller names it by PlanId and never passes a plan object.
    /// <paramref name="beforePublish"/> is the legacy test-only fault seam, which the public adapter never supplies.
    /// </summary>
    internal PlayerCommandResult Execute(PlayerFence fence, string planId, string executionKey, Action? beforePublish = null)
    {
        lock (sync)
        {
            RequireSession(fence);
            if (string.IsNullOrWhiteSpace(executionKey))
                throw new P1Refusal("WorkExecutionKeyRequired", "an execution key is required");
            // A-2 replay rule: answered before the staleness fence on purpose, so a genuine retry after an unconfirmed
            // success receives its receipt instead of a second Fell, but only for the same plan, original source and session.
            if (root.Executed.TryGetValue(executionKey, out var executed))
            {
                if (string.Equals(executed.Event.PlanId, planId, StringComparison.Ordinal) && executed.Source == fence.Source)
                    return WithCurrentView(new WorkExecutionResult(executionKey, executed.Event.EcologicalYear,
                        executed.Event.WorkRevision, executed.Event, Replayed: true), null);
                throw new P1Refusal("WorkExecutionKeyConflict",
                    "this execution key was already used for a different plan or source");
            }
            RequireFence(fence);
            var approved = root.ApprovedUnexecuted;
            if (approved is null || !string.Equals(approved.PlanId, planId, StringComparison.Ordinal))
                throw new P1Refusal("WorkPlanNotApproved", "only the currently approved plan can execute");
            return WithCurrentView(Execute(approved, executionKey, beforePublish), null);
        }
    }

    /// <summary>
    /// One genuine ecological year through the legacy explicit-work-revision overload, fed the fence token's clocks.
    /// The legacy three-argument advance is not reachable from here. <paramref name="beforeCommit"/> is test-only.
    /// </summary>
    internal PlayerCommandResult Advance(PlayerFence fence, bool consentToCancelWork, Action? beforeCommit = null)
    {
        lock (sync)
        {
            RequireFence(fence);
            var source = fence.Source;
            var report = Advance(source.EcologicalYear, source.AnnualRevision, source.WorkRevision,
                consentToCancelWork, beforeCommit);
            return WithCurrentView(null, report);
        }
    }

    /// <summary>Read-only exact-identity hit-test on the CURRENT view; a stale fence is refused (spec §5 S-3).</summary>
    internal PlayerHitTestResult HitTest(PlayerFence fence, double u, double v)
    {
        PlayerSessionView view;
        lock (sync)
        {
            RequireFence(fence);
            view = ViewOf(root);
        }
        // The scene is pure geometry over this immutable view, so it needs no lock.
        return new PlayerHitTestResult(view, PlayerMapScene.HitTest(PlayerMapScene.Build(view), u, v), null, null);
    }

    // Caller holds sync. Fence order (spec §6.4): session, then the full source token, then the plan-state revision.
    private void RequireFence(PlayerFence fence)
    {
        RequireSession(fence);
        if (fence.Source != Token(root))
            throw new P1Refusal("PlayerSourceStale",
                "the source, year or a revision changed since this view was read; nothing changed, re-read the current view");
        if (fence.PlanStateRevision != root.PlanStateRevision)
            throw new P1Refusal("PlayerPlanStateStale",
                "the Work Plan changed since this view was read; nothing changed, re-read the current view");
    }

    private void RequireSession(PlayerFence fence)
    {
        // A missing fence cannot name this session, so it is refused exactly like another session's fence.
        if (fence is null || !string.Equals(fence.SessionId, sessionId, StringComparison.Ordinal))
            throw new P1Refusal("PlayerSessionMismatch", "this request does not belong to the current player session");
    }

    // Caller holds sync.
    private PlayerCommandResult WithCurrentView(WorkExecutionResult? execution, AnnualReport? annual) =>
        new(ViewOf(root), execution, annual, null, null);

    // Caller holds sync and passes the single root it read. Every exposed collection is a fresh read-only copy.
    private PlayerSessionView ViewOf(WorkRoot current)
    {
        var plan = current.ApprovedUnexecuted;
        var approved = plan is null ? null : new PlayerApprovedPlan(plan.PlanId, plan.PlanId + ":execute", plan.Source,
            Array.AsReadOnly(plan.Orders.ToArray()));
        return new PlayerSessionView(sessionId, current.PlanStateRevision, Token(current), FrameOf(current),
            Array.AsReadOnly(current.Pending.ToArray()), approved, Array.AsReadOnly(current.AnnualHistory.ToArray()),
            new PlayerMapExtent(MapMinM, MapMinM, MapSideM, areaHa));
    }

    // The same construction as Frame, from an explicit root (Frame itself is left unchanged; PS-17 compares the two).
    private CommittedFrame FrameOf(WorkRoot current) =>
        new(ManagementScope.WholeUndividedThirtyMetres, identity.Domain, identity.FixtureId,
            identity.FixtureFileSha256, identity.FixtureContentHash,
            current.Snapshot.State.EcologicalYear, current.Snapshot.Revision, current.WorkRevision,
            Array.AsReadOnly(current.Snapshot.State.Trees.ToArray()),
            WorkRules.Summarize(current.Snapshot.State.Trees, areaHa),
            Array.AsReadOnly(current.Events.ToArray()));
}
