namespace SitkaP1.Core;

/// <summary>One area's committed state inside the root: its roster (with the shared ecological year) and its Crop marks.</summary>
internal sealed record MultiAreaAreaState(string AreaId, AnnualState State, IReadOnlyList<string> CropTreeIds);

/// <summary>An executed key, kept so an identical retry is answered with its receipt instead of another Fell.</summary>
internal sealed record MultiAreaExecutedWork(MultiAreaSourceToken Source, IReadOnlyList<MultiAreaWorkOrder> Orders,
    MultiAreaWorkEvent Event);

/// <summary>
/// The ONE immutable commit root of the whole experiment (MA-1 spec §3). Every area's roster and Crop marks, the shared
/// year, both clocks, the work revision, the plan-state revision, the pending orders, the approved plan, the event ledger,
/// the executed-key map and the annual history are published together by a single reference assignment, so no reader
/// can see area A changed while area B is not.
/// </summary>
internal sealed record MultiAreaRoot(
    int EcologicalYear,
    int AnnualRevision,
    int WorkRevision,
    int PlanStateRevision,
    IReadOnlyList<MultiAreaAreaState> Areas,
    IReadOnlyList<MultiAreaWorkEvent> Events,
    IReadOnlyDictionary<string, MultiAreaExecutedWork> Executed,
    IReadOnlyList<MultiAreaWorkOrder> Pending,
    MultiAreaApprovedPlan? Approved,
    IReadOnlyList<MultiAreaYearRecord> AnnualHistory);

/// <summary>The per-area annual step: the unchanged AnnualRules.Calculate and the same next-state construction as AnnualContinuation.</summary>
internal static class MultiAreaAnnual
{
    internal static (AnnualReport Report, AnnualState Next) Step(AnnualState state, int simulationSeed, float areaHa)
    {
        var report = AnnualRules.Calculate(state, simulationSeed, areaHa);
        var next = new AnnualState(report.YearAfter, Array.AsReadOnly(report.Trees.Select(row => row.After).ToArray()));
        return (report, next);
    }
}

/// <summary>
/// In-memory, no-durability MA-1 session over several isolated areas (owner-approved MA-1 spec §2–§4): one shared
/// ecological year in which each area runs the unchanged annual rules on its own trees, area and seed; one manual
/// cross-area Work Plan (approve fells nothing; a separate Execute commits every area once or refuses all); and the
/// owner's safeguards. Every read and command takes <c>sync</c> exactly once, so the fence it checks, the transition it
/// performs and the view it returns all come from one root. This is NOT two composed AnnualContinuation sessions.
/// </summary>
internal sealed class MultiAreaContinuation
{
    private sealed record AreaInfo(string AreaId, string Label, string StockingNote, int Seed, float AreaHa,
        string AreaM2Exact, IReadOnlyList<MultiAreaVertex> Polygon, PlayerMapExtent Extent);

    private readonly object sync = new();
    private readonly IReadOnlyList<AreaInfo> areas; // ordinal by AreaId, the same order as root.Areas
    private readonly MultiAreaExperimentInfo experiment;
    private readonly string areaIds;
    // One random identity per in-memory session. It is never part of a token, report or manifest.
    private readonly string sessionId =
        Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
    private MultiAreaRoot root;

    internal MultiAreaContinuation(MultiAreaExperiment definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        definition.Validate();
        string contentHash = definition.ContentHash();
        experiment = new MultiAreaExperimentInfo(definition.ExperimentId, MultiAreaIdentity.Format, MultiAreaIdentity.SchemaVersion,
            MultiAreaIdentity.Domain, MultiAreaIdentity.SourceClass, contentHash, definition.GeneratorId,
            definition.GeneratorVersion, definition.GeneratorSeed, MultiAreaIdentity.ReviewStatus, MultiAreaIdentity.Disclosure,
            MultiAreaIdentity.EdgeModel, MultiAreaIdentity.SiteNote);
        areas = Array.AsReadOnly(definition.Areas.Select(InfoOf).ToArray());
        areaIds = string.Join(",", definition.Areas.Select(area => area.AreaId));
        var states = definition.Areas.Select(area => new MultiAreaAreaState(area.AreaId,
            new AnnualState(0, MultiAreaExperiment.GenesisTrees(area)), Array.Empty<string>())).ToArray();
        root = new MultiAreaRoot(0, 0, 0, 0, Array.AsReadOnly(states), Array.Empty<MultiAreaWorkEvent>(),
            new Dictionary<string, MultiAreaExecutedWork>(StringComparer.Ordinal), Array.Empty<MultiAreaWorkOrder>(), null,
            Array.Empty<MultiAreaYearRecord>());
    }

    internal string SessionId => sessionId;

    /// <summary>The whole view, built from ONE root reference under ONE lock acquisition.</summary>
    internal MultiAreaSessionView ReadView()
    {
        lock (sync) return ViewOf(root);
    }

    // ------------------------------------------------------------------------------------------------ Crop marks

    internal MultiAreaCommandResult MarkCrop(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            int index = AreaIndex(areaId);
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "UnknownTreeId",
                "only a living exact tree ID of this area can be retained as Crop");
            if (root.Pending.Any(order => order.TreeId == treeId) ||
                (root.Approved?.Orders.Any(order => order.TreeId == treeId) ?? false))
                throw new P1Refusal("CropAlreadyPlannedForFell", "remove the pending Fell or cancel its approved plan first");
            var area = root.Areas[index];
            if (area.CropTreeIds.Contains(treeId, StringComparer.Ordinal))
                throw new P1Refusal("CropAlreadyMarked", "this tree is already retained as Crop");
            var crops = area.CropTreeIds.Append(treeId).Order(StringComparer.Ordinal).ToArray();
            root = root with
            {
                Areas = Replace(root.Areas, index, area with { CropTreeIds = Array.AsReadOnly(crops) }),
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    internal MultiAreaCommandResult UnmarkCrop(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            int index = AreaIndex(areaId);
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "UnknownTreeId",
                "only a living exact tree ID of this area can have a Crop mark removed");
            var area = root.Areas[index];
            if (!area.CropTreeIds.Contains(treeId, StringComparer.Ordinal))
                throw new P1Refusal("CropNotMarked", "this tree has no retained Crop mark");
            var crops = area.CropTreeIds.Where(id => id != treeId).ToArray();
            root = root with
            {
                Areas = Replace(root.Areas, index, area with { CropTreeIds = Array.AsReadOnly(crops) }),
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    // ------------------------------------------------------------------------------------------------ the Work Plan

    /// <summary>Add one manually chosen living tree of one area, with a recorded reason. This is not work and fells nothing.</summary>
    internal MultiAreaCommandResult AddPending(MultiAreaFence fence, string areaId, string treeId, string reason)
    {
        lock (sync)
        {
            RequireFence(fence);
            int index = AreaIndex(areaId);
            if (string.IsNullOrWhiteSpace(treeId))
                throw new P1Refusal("UnknownTreeId", "a tree ID is required");
            if (string.IsNullOrWhiteSpace(reason))
                throw new P1Refusal("WorkReasonRequired", "every manual selection needs a recorded reason");
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");
            if (root.Pending.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)))
                throw new P1Refusal("WorkDuplicateTreeId", "the tree ID is already in the pending Work Plan");
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "WorkTreeNotLive",
                "only a living exact tree ID of this area can be proposed for Fell");
            if (root.Areas[index].CropTreeIds.Contains(treeId, StringComparer.Ordinal))
                throw new P1Refusal("CropTreeRetained", "unmark this retained Crop Tree before proposing it for Fell");
            var pending = root.Pending.Append(new MultiAreaWorkOrder(areaId, treeId, WorkAction.Fell, reason)).ToArray();
            root = root with { Pending = Array.AsReadOnly(pending), PlanStateRevision = checked(root.PlanStateRevision + 1) };
            return WithView(null, null);
        }
    }

    /// <summary>Remove one pending order of one area before approval. Never touches biology.</summary>
    internal MultiAreaCommandResult RemovePending(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            AreaIndex(areaId);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "the plan is approved; cancel it before editing");
            var match = root.Pending.FirstOrDefault(order => order.TreeId == treeId);
            if (match is null)
                throw new P1Refusal("UnknownWorkOrder", "the tree ID is not in the pending Work Plan");
            if (match.AreaId != areaId)
                throw new P1Refusal("MultiAreaWrongArea", "the pending order for " + treeId + " belongs to " + match.AreaId + ", not " + areaId);
            var pending = root.Pending.Where(order => !ReferenceEquals(order, match)).ToArray();
            root = root with { Pending = Array.AsReadOnly(pending), PlanStateRevision = checked(root.PlanStateRevision + 1) };
            return WithView(null, null);
        }
    }

    /// <summary>Approve the whole cross-area pending plan under an engine-minted PlanId and key. Zero Fell.</summary>
    internal MultiAreaCommandResult Approve(MultiAreaFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is already awaiting execution");
            if (root.Pending.Count == 0)
                throw new P1Refusal("WorkEmptyPlan", "there is no pending work to approve");
            int planState = checked(root.PlanStateRevision + 1);
            string planId = FormattableString.Invariant($"{sessionId}:ma-plan:{planState}");
            var plan = new MultiAreaApprovedPlan(planId, MintedExecutionKey(planId), Token(root),
                Array.AsReadOnly(root.Pending.ToArray()));
            root = root with { Approved = plan, Pending = Array.Empty<MultiAreaWorkOrder>(), PlanStateRevision = planState };
            return WithView(null, null);
        }
    }

    /// <summary>Withdraw the approved plan. Zero Fell; its orders are not returned to pending.</summary>
    internal MultiAreaCommandResult CancelApproved(MultiAreaFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            if (root.Approved is null)
                throw new P1Refusal("WorkPlanNotApproved", "there is no approved plan to cancel");
            root = root with { Approved = null, PlanStateRevision = checked(root.PlanStateRevision + 1) };
            return WithView(null, null);
        }
    }

    /// <summary>
    /// Execute the engine's own approved plan across every area: all-or-nothing. The whole batch is revalidated before any
    /// change; every area's new roster, the per-area before/after and the one event are built; then one root assignment
    /// publishes them together with the work revision, the executed key and the cleared approval.
    /// <paramref name="afterAreaBuilt"/> (after each area's roster is built) and <paramref name="beforePublish"/> are
    /// test-only fault seams; the adapter never supplies them.
    /// </summary>
    internal MultiAreaCommandResult Execute(MultiAreaFence fence, string planId, string executionKey,
        Action<string>? afterAreaBuilt = null, Action? beforePublish = null)
    {
        lock (sync)
        {
            RequireSession(fence);
            if (string.IsNullOrWhiteSpace(executionKey))
                throw new P1Refusal("WorkExecutionKeyRequired", "an execution key is required");
            // Replay rule: answered before the staleness fence on purpose, so a genuine retry after an unconfirmed success
            // receives its receipt instead of a second Fell, but only for the same plan, original source and session.
            if (root.Executed.TryGetValue(executionKey, out var executed))
            {
                if (string.Equals(executed.Event.PlanId, planId, StringComparison.Ordinal) && executed.Source == fence.Source)
                    return WithView(new MultiAreaExecutionResult(executionKey, executed.Event.EcologicalYear,
                        executed.Event.WorkRevision, executed.Event, Replayed: true), null);
                throw new P1Refusal("WorkExecutionKeyConflict", "this execution key was already used for a different plan or source");
            }
            RequireFence(fence);
            var plan = root.Approved;
            if (plan is null || !string.Equals(plan.PlanId, planId, StringComparison.Ordinal))
                throw new P1Refusal("WorkPlanNotApproved", "only the currently approved plan can execute");
            if (!string.Equals(executionKey, plan.ExecutionKey, StringComparison.Ordinal))
                throw new P1Refusal("WorkExecutionKeyConflict",
                    "a first execution must use the execution key the engine minted for the approved plan");
            if (plan.Source != Token(root))
                throw new P1Refusal("WorkPlanStaleSource", "the source, year or a revision moved since approval");
            if (plan.Orders.Count == 0)
                throw new P1Refusal("WorkEmptyPlan", "the approved plan contains no orders");

            // 1. Revalidate the WHOLE batch against the current root before anything is built.
            var ordersByArea = new Dictionary<string, Dictionary<string, MultiAreaWorkOrder>>(StringComparer.Ordinal);
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var order in plan.Orders)
            {
                int index = AreaIndex(order.AreaId);
                if (order.Action != WorkAction.Fell)
                    throw new P1Refusal("WorkActionInvalid", "Fell is the only work action in this model");
                if (!seen.Add(order.TreeId))
                    throw new P1Refusal("WorkDuplicateTreeId", "the approved plan contains a duplicate tree ID");
                RequireLivingInArea(index, order.TreeId, unknownCode: "WorkTreeNotLive", deadCode: "WorkTreeNotLive",
                    "a selected ID is not a living exact-case tree of its area in the current source");
                if (root.Areas[index].CropTreeIds.Contains(order.TreeId, StringComparer.Ordinal))
                    throw new P1Refusal("CropTreeRetained", "a retained Crop Tree cannot be felled");
                if (string.IsNullOrWhiteSpace(order.Reason))
                    throw new P1Refusal("WorkReasonRequired", "every order needs its recorded reason");
                if (!ordersByArea.TryGetValue(order.AreaId, out var inArea))
                    ordersByArea[order.AreaId] = inArea = new Dictionary<string, MultiAreaWorkOrder>(StringComparer.Ordinal);
                inArea[order.TreeId] = order;
            }
            // 2. Owner safeguard: a batch that would leave ANY currently populated area with no living tree is refused whole.
            foreach (var (areaId, inArea) in ordersByArea)
            {
                var living = root.Areas[AreaIndex(areaId)].State.Trees.Where(tree => tree.IsLiving).ToArray();
                if (living.Length > 0 && living.All(tree => inArea.ContainsKey(tree.TreeId)))
                    throw new P1Refusal("WorkWouldLeaveNoLivingStems",
                        "the approved plan would leave " + areaId + " with no living stems; the entire plan is refused");
            }

            // 3. Build every area's roster and its real before/after; nothing reachable changes yet.
            int workRevision = checked(root.WorkRevision + 1);
            int year = root.EcologicalYear;
            var nextAreas = new MultiAreaAreaState[root.Areas.Count];
            var changes = new MultiAreaAreaChange[root.Areas.Count];
            for (int i = 0; i < root.Areas.Count; i++)
            {
                var area = root.Areas[i];
                ordersByArea.TryGetValue(area.AreaId, out var targets);
                var trees = new AnnualTree[area.State.Trees.Count];
                var fells = new List<MultiAreaFellRecord>();
                for (int t = 0; t < trees.Length; t++)
                {
                    var tree = area.State.Trees[t];
                    if (tree.IsLiving && targets is not null && targets.TryGetValue(tree.TreeId, out var order))
                    {
                        trees[t] = tree with { Death = new AnnualDeath(tree.TreeId, year, FellCause.Harvest, FellCause.Mechanism) };
                        fells.Add(new MultiAreaFellRecord(area.AreaId, tree.TreeId, FellCause.Harvest, FellCause.Mechanism,
                            order.Reason, year, workRevision, executionKey, plan.PlanId));
                    }
                    else
                    {
                        trees[t] = tree;
                    }
                }
                var before = WorkRules.Summarize(area.State.Trees, areas[i].AreaHa);
                var after = WorkRules.Summarize(trees, areas[i].AreaHa);
                changes[i] = new MultiAreaAreaChange(area.AreaId, before, after,
                    Array.AsReadOnly(fells.OrderBy(record => record.TreeId, StringComparer.Ordinal).ToArray()));
                nextAreas[i] = targets is null ? area : area with { State = new AnnualState(year, Array.AsReadOnly(trees)) };
                afterAreaBuilt?.Invoke(area.AreaId);
            }
            var committed = new MultiAreaWorkEvent(executionKey, plan.PlanId, year, workRevision, Array.AsReadOnly(changes));

            // Test-only fault seam: rosters, event and revisions are complete, but nothing has been published.
            beforePublish?.Invoke();

            // 4. ONE reference assignment publishes every area, the event, the work revision, the key and the cleared plan.
            var executedMap = new Dictionary<string, MultiAreaExecutedWork>(root.Executed, StringComparer.Ordinal)
            {
                [executionKey] = new MultiAreaExecutedWork(plan.Source, Array.AsReadOnly(plan.Orders.ToArray()), committed),
            };
            root = root with
            {
                Areas = Array.AsReadOnly(nextAreas),
                WorkRevision = workRevision,
                Events = Array.AsReadOnly(root.Events.Append(committed).ToArray()),
                Executed = executedMap,
                Approved = null,
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(new MultiAreaExecutionResult(executionKey, year, workRevision, committed, Replayed: false), null);
        }
    }

    // ------------------------------------------------------------------------------------------------ the shared year

    /// <summary>
    /// One genuine shared ecological year. Each populated area runs the unchanged AnnualRules.Calculate on its own trees,
    /// seed and area; an area with no living tree is carried forward unchanged. Unexecuted work needs explicit consent and
    /// is then cancelled for the whole experiment at zero Fell. Refused only when every area is empty.
    /// <paramref name="afterArea"/> and <paramref name="beforeCommit"/> are test-only fault seams.
    /// </summary>
    internal MultiAreaCommandResult Advance(MultiAreaFence fence, bool consentToCancelWork, Action<string>? afterArea = null,
        Action? beforeCommit = null)
    {
        lock (sync)
        {
            RequireFence(fence);
            bool hasWork = root.Pending.Count > 0 || root.Approved is not null;
            if (hasWork && !consentToCancelWork)
                throw new P1Refusal("WorkPendingConsentRequired",
                    "advancing the year with unexecuted work needs explicit consent to cancel all of it at zero Fell");
            if (root.Areas.All(area => !area.State.Trees.Any(tree => tree.IsLiving)))
                throw new P1Refusal("ContinuationNoLivingStems", "every area has no living stems; there is no year to advance");

            int year = root.EcologicalYear;
            int revision = checked(root.AnnualRevision + 1);
            var nextAreas = new MultiAreaAreaState[root.Areas.Count];
            var records = new MultiAreaAreaYear[root.Areas.Count];
            var reports = new MultiAreaAreaReport[root.Areas.Count];
            bool lostCrop = false;
            for (int i = 0; i < root.Areas.Count; i++)
            {
                var area = root.Areas[i];
                var info = areas[i];
                AnnualState next;
                if (area.State.Trees.Any(tree => tree.IsLiving))
                {
                    var (report, stepped) = MultiAreaAnnual.Step(area.State, info.Seed, info.AreaHa);
                    next = stepped;
                    records[i] = new MultiAreaAreaYear(area.AreaId, report.Before, report.After,
                        Array.AsReadOnly(report.Deaths.ToArray()), NoLivingStems: false);
                    reports[i] = new MultiAreaAreaReport(area.AreaId, report);
                }
                else
                {
                    // Naturally empty (owner D-5): the engine is not run for this area; its dead roster is carried forward.
                    next = new AnnualState(checked(year + 1), area.State.Trees);
                    var empty = WorkRules.Summarize(area.State.Trees, info.AreaHa);
                    records[i] = new MultiAreaAreaYear(area.AreaId, empty, empty, Array.Empty<AnnualDeath>(), NoLivingStems: true);
                    reports[i] = new MultiAreaAreaReport(area.AreaId, null);
                }
                // Crop marks are plan state: a mark on a tree that died this year is cleared in the same publish.
                var livingIds = next.Trees.Where(tree => tree.IsLiving).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
                var crops = area.CropTreeIds.Where(livingIds.Contains).ToArray();
                lostCrop |= crops.Length != area.CropTreeIds.Count;
                nextAreas[i] = new MultiAreaAreaState(area.AreaId, next, Array.AsReadOnly(crops));
                afterArea?.Invoke(area.AreaId);
            }
            var yearRecord = new MultiAreaYearRecord(year, checked(year + 1), revision, root.WorkRevision,
                Array.AsReadOnly(records), hasWork);

            // Test-only fault seam: every area is calculated, nothing is published.
            beforeCommit?.Invoke();

            root = root with
            {
                EcologicalYear = checked(year + 1),
                AnnualRevision = revision,
                Areas = Array.AsReadOnly(nextAreas),
                AnnualHistory = Array.AsReadOnly(root.AnnualHistory.Append(yearRecord).ToArray()),
                Pending = hasWork ? Array.Empty<MultiAreaWorkOrder>() : root.Pending,
                Approved = hasWork ? null : root.Approved,
                PlanStateRevision = checked(root.PlanStateRevision + (hasWork || lostCrop ? 1 : 0)),
            };
            return WithView(null, new MultiAreaAnnualResult(year, checked(year + 1), Array.AsReadOnly(reports)));
        }
    }

    // ------------------------------------------------------------------------------------------------ read-only inspection

    /// <summary>
    /// Read-only competition inspection of ANY living tree of one area, before or after any Crop designation. It runs the
    /// existing single-area diagnostic unchanged on this area's own trees (so neighbours across a boundary are excluded,
    /// exactly as in the annual rules) and never changes the root.
    /// </summary>
    internal MultiAreaCompetitionResult Competition(MultiAreaFence fence, string areaId, string treeId)
    {
        MultiAreaSessionView view;
        int index;
        lock (sync)
        {
            RequireFence(fence);
            index = AreaIndex(areaId);
            if (string.IsNullOrWhiteSpace(treeId))
                throw new P1Refusal("CompetitionTreeUnknown", "a tree ID is required");
            if (FindTree(index, treeId) is null && OwnerOf(treeId) is string owner)
                throw new P1Refusal("MultiAreaWrongArea", "tree " + treeId + " belongs to " + owner + ", not " + areaId);
            view = ViewOf(root);
        }
        // Pure arithmetic over the immutable view; it needs no lock and keeps no state.
        var report = PlayerCompetitionDiagnostics.Measure(MultiAreaProjection.Of(view, view.Areas[index]), treeId);
        return new MultiAreaCompetitionResult(view, areaId, report, null, null);
    }

    /// <summary>Read-only exact-ID hit-test on one area's CURRENT map; a stale fence is refused.</summary>
    internal MultiAreaHitResult HitTest(MultiAreaFence fence, string areaId, double u, double v)
    {
        MultiAreaSessionView view;
        int index;
        lock (sync)
        {
            RequireFence(fence);
            index = AreaIndex(areaId);
            view = ViewOf(root);
        }
        var scene = MultiAreaProjection.Scene(view, view.Areas[index]);
        return new MultiAreaHitResult(view, areaId, PlayerMapScene.HitTest(scene, u, v), null, null);
    }

    // ------------------------------------------------------------------------------------------------ helpers (caller holds sync)

    // Fence order: session, then the full source token, then the plan-state revision.
    private void RequireFence(MultiAreaFence fence)
    {
        RequireSession(fence);
        if (fence.Source != Token(root))
            throw new P1Refusal("PlayerSourceStale",
                "the source, year or a revision changed since this view was read; nothing changed, re-read the current view");
        if (fence.PlanStateRevision != root.PlanStateRevision)
            throw new P1Refusal("PlayerPlanStateStale",
                "the Work Plan changed since this view was read; nothing changed, re-read the current view");
    }

    private void RequireSession(MultiAreaFence fence)
    {
        if (fence is null || !string.Equals(fence.SessionId, sessionId, StringComparison.Ordinal))
            throw new P1Refusal("PlayerSessionMismatch", "this request does not belong to the current multi-area session");
    }

    private int AreaIndex(string areaId)
    {
        for (int i = 0; i < areas.Count; i++)
            if (string.Equals(areas[i].AreaId, areaId, StringComparison.Ordinal)) return i;
        throw new P1Refusal("MultiAreaUnknownArea", "no area of this experiment has that exact ID");
    }

    private AnnualTree? FindTree(int areaIndex, string treeId)
    {
        foreach (var tree in root.Areas[areaIndex].State.Trees)
            if (string.Equals(tree.TreeId, treeId, StringComparison.Ordinal)) return tree;
        return null;
    }

    private string? OwnerOf(string treeId)
    {
        for (int i = 0; i < root.Areas.Count; i++)
            if (FindTree(i, treeId) is not null) return root.Areas[i].AreaId;
        return null;
    }

    // A tree of another area is refused as MultiAreaWrongArea, before any unknown/dead refusal, so a wrong-area request
    // can never be mistaken for a typo.
    private void RequireLivingInArea(int areaIndex, string treeId, string unknownCode, string deadCode, string message)
    {
        var tree = string.IsNullOrEmpty(treeId) ? null : FindTree(areaIndex, treeId);
        if (tree is null)
        {
            if (!string.IsNullOrEmpty(treeId) && OwnerOf(treeId) is string owner)
                throw new P1Refusal("MultiAreaWrongArea",
                    "tree " + treeId + " belongs to " + owner + ", not " + root.Areas[areaIndex].AreaId);
            throw new P1Refusal(unknownCode, message);
        }
        if (!tree.IsLiving) throw new P1Refusal(deadCode, message);
    }

    private MultiAreaSourceToken Token(MultiAreaRoot current) => new(MultiAreaIdentity.Domain, experiment.ExperimentId,
        experiment.ManifestContentHash, areaIds, current.EcologicalYear, current.AnnualRevision, current.WorkRevision,
        DerivedManifest.RngModelVersion, DerivedManifest.GrowthModelVersion, DerivedManifest.P1AlgorithmVersion,
        MultiAreaIdentity.AreaModelVersion, MultiAreaIdentity.ActionEligibleDbhThresholdMm);

    // The one execution key the engine mints for a plan. The view shows it and a first Execute requires it.
    private static string MintedExecutionKey(string planId) => planId + ":execute";

    private MultiAreaCommandResult WithView(MultiAreaExecutionResult? execution, MultiAreaAnnualResult? annual) =>
        new(ViewOf(root), execution, annual, null, null);

    // Every exposed collection is a fresh read-only copy taken from the single root passed in.
    private MultiAreaSessionView ViewOf(MultiAreaRoot current)
    {
        var areaViews = new MultiAreaAreaView[current.Areas.Count];
        for (int i = 0; i < current.Areas.Count; i++)
        {
            var info = areas[i];
            var state = current.Areas[i];
            var trees = Array.AsReadOnly(state.State.Trees.ToArray());
            areaViews[i] = new MultiAreaAreaView(info.AreaId, info.Label, info.StockingNote, MultiAreaIdentity.Site, info.Seed,
                info.AreaHa, info.AreaM2Exact, Array.AsReadOnly(info.Polygon.ToArray()), info.Extent, trees,
                WorkRules.Summarize(trees, info.AreaHa), Array.AsReadOnly(state.CropTreeIds.ToArray()));
        }
        var approved = current.Approved is null ? null : current.Approved with
        {
            Orders = Array.AsReadOnly(current.Approved.Orders.ToArray()),
        };
        return new MultiAreaSessionView(sessionId, current.PlanStateRevision, Token(current), experiment,
            Array.AsReadOnly(areaViews), Array.AsReadOnly(current.Pending.ToArray()), approved,
            Array.AsReadOnly(current.Events.ToArray()), Array.AsReadOnly(current.AnnualHistory.ToArray()));
    }

    private static IReadOnlyList<MultiAreaAreaState> Replace(IReadOnlyList<MultiAreaAreaState> list, int index,
        MultiAreaAreaState value)
    {
        var copy = list.ToArray();
        copy[index] = value;
        return Array.AsReadOnly(copy);
    }

    private static AreaInfo InfoOf(MultiAreaDefinition area)
    {
        var polygon = Array.AsReadOnly(area.PolygonMm.Select(p => new MultiAreaVertex(p.XMm / 1000.0, p.YMm / 1000.0)).ToArray());
        var (minX, minY, maxX, maxY) = MultiAreaGeometry.Bounds(area.PolygonMm);
        // A square map around the polygon with a 1 m margin, in the single-area map convention (U east, V south).
        long side = Math.Max(maxX - minX, maxY - minY) + 2_000;
        var extent = new PlayerMapExtent((minX - 1_000) / 1000.0, (minY - 1_000) / 1000.0, side / 1000.0, area.AreaHa);
        return new AreaInfo(area.AreaId, area.Label, area.StockingNote, area.SimulationSeed, area.AreaHa, area.AreaM2Exact,
            polygon, extent);
    }
}
