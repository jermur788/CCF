namespace SitkaP1.Core;

using SitkaP1.Core.Phase1;

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Approve the whole cross-area pending plan under an engine-minted PlanId and key. Zero Fell, and (Stage 4) zero
    /// pruning and (Stage 5) zero clearing: pending Prune and ClearUndergrowth orders move into the approval with the Fell
    /// orders and nothing ecological changes.
    /// </summary>
    internal MultiAreaCommandResult Approve(MultiAreaFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is already awaiting execution");
            var stage4 = root.Stage4;
            var stage5 = root.Stage5;
            var phase2 = root.Phase2;
            if (root.Pending.Count == 0 && (stage4?.PendingPrunes.Count ?? 0) == 0 && (stage5?.PendingClears.Count ?? 0) == 0 &&
                (phase2?.PendingOrders.Count ?? 0) == 0)
                throw new P1Refusal("WorkEmptyPlan", "there is no pending work to approve");
            int planState = checked(root.PlanStateRevision + 1);
            string planId = FormattableString.Invariant($"{sessionId}:ma-plan:{planState}");
            var plan = new MultiAreaApprovedPlan(planId, MintedExecutionKey(planId), Token(root),
                Array.AsReadOnly(root.Pending.ToArray()));
            root = root with
            {
                Approved = plan,
                Pending = Array.Empty<MultiAreaWorkOrder>(),
                PlanStateRevision = planState,
                Stage4 = stage4 is null ? null : stage4 with
                {
                    ApprovedPrunes = Array.AsReadOnly(stage4.PendingPrunes.ToArray()),
                    PendingPrunes = Array.Empty<MultiAreaPruneOrder>(),
                },
                Stage5 = stage5 is null ? null : stage5 with
                {
                    ApprovedClears = Array.AsReadOnly(stage5.PendingClears.ToArray()),
                    PendingClears = Array.Empty<MultiAreaClearUndergrowthOrder>(),
                },
                Phase2 = phase2 is null ? null : phase2 with
                {
                    ApprovedOrders = Array.AsReadOnly(phase2.PendingOrders.ToArray()),
                    PendingOrders = Array.Empty<MultiAreaPlantingOrder>(),
                },
            };
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
            root = root with
            {
                Approved = null,
                PlanStateRevision = checked(root.PlanStateRevision + 1),
                Stage4 = root.Stage4 is null ? null : root.Stage4 with { ApprovedPrunes = Array.Empty<MultiAreaPruneOrder>() },
                Stage5 = root.Stage5 is null ? null : root.Stage5 with { ApprovedClears = Array.Empty<MultiAreaClearUndergrowthOrder>() },
                Phase2 = root.Phase2 is null ? null : root.Phase2 with { ApprovedOrders = Array.Empty<MultiAreaPlantingOrder>() },
            };
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
            var approvedPrunes = root.Stage4?.ApprovedPrunes ?? Array.Empty<MultiAreaPruneOrder>();
            var approvedClears = root.Stage5?.ApprovedClears ?? Array.Empty<MultiAreaClearUndergrowthOrder>();
            var approvedPlanting = root.Phase2?.ApprovedOrders ?? Array.Empty<MultiAreaPlantingOrder>();
            if (plan.Orders.Count == 0 && approvedPrunes.Count == 0 && approvedClears.Count == 0 && approvedPlanting.Count == 0)
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
            // Stage 4: every approved Prune is revalidated against the current tree, its pruning history and the current year.
            var prunesByArea = new Dictionary<string, Dictionary<string, MultiAreaPruneOrder>>(StringComparer.Ordinal);
            foreach (var order in approvedPrunes)
            {
                int index = AreaIndex(order.AreaId);
                if (!seen.Add(order.TreeId))
                    throw new P1Refusal("WorkDuplicateTreeId", "the approved plan contains a duplicate tree ID");
                RequireLivingInArea(index, order.TreeId, unknownCode: "PruneTreeNotLiving", deadCode: "PruneTreeNotLiving",
                    "a pruned ID is not a living exact-case tree of its area in the current source");
                RequirePrunable(FindTree(index, order.TreeId)!, root.Stage4!.PruningOf(order.TreeId), order.TargetCrownBaseHeightM);
                if (!prunesByArea.TryGetValue(order.AreaId, out var inArea))
                    prunesByArea[order.AreaId] = inArea = new Dictionary<string, MultiAreaPruneOrder>(StringComparer.Ordinal);
                inArea[order.TreeId] = order;
            }
            // Stage 5: every approved clear must name an existing in-polygon cell of its own area, once.
            var clearCells = new HashSet<(string, int, int)>();
            foreach (var order in approvedClears)
            {
                AreaIndex(order.AreaId);
                RequireClearCell(root.Stage5!, order.AreaId, order.Column, order.Row);
                if (!clearCells.Add((order.AreaId, order.Column, order.Row)))
                    throw new P1Refusal("ClearDuplicateCell", "the approved plan clears one ground cell twice");
            }
            // Phase 2: the whole planting batch is revalidated atomically against the current root.
            if (approvedPlanting.Count > 0)
                RequireValidPlantingBatch(root.Phase2!, approvedPlanting);
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
            var pruning = root.Stage4 is null ? null
                : new Dictionary<string, TreePruningState>(root.Stage4.Pruning, StringComparer.Ordinal);
            var pruneRecords = new List<MultiAreaPruneRecord>();
            for (int i = 0; i < root.Areas.Count; i++)
            {
                var area = root.Areas[i];
                ordersByArea.TryGetValue(area.AreaId, out var targets);
                prunesByArea.TryGetValue(area.AreaId, out var prunes);
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
                    else if (tree.IsLiving && prunes is not null && prunes.TryGetValue(tree.TreeId, out var prune))
                    {
                        // One lift (Unity TryPrune): immediate r = max(0.1, r * 0.92); lifts + 1; crown base = target; year.
                        // No clock moves; the crown change publishes with WorkRevision, which invalidates canopy/light.
                        var history = pruning![tree.TreeId] = pruning.TryGetValue(tree.TreeId, out var old)
                            ? old with { Lifts = old.Lifts + 1, CrownBaseHeightM = prune.TargetCrownBaseHeightM, LastPruningYear = year }
                            : new TreePruningState(1, prune.TargetCrownBaseHeightM, year);
                        trees[t] = tree with { CrownRadiusM = PruningRules.CutRadiusM(tree.CrownRadiusM) };
                        pruneRecords.Add(new MultiAreaPruneRecord(area.AreaId, tree.TreeId, prune.TargetCrownBaseHeightM, year,
                            workRevision, executionKey, plan.PlanId, tree.CrownRadiusM, trees[t].CrownRadiusM, history.Lifts));
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
                nextAreas[i] = targets is null && prunes is null ? area : area with { State = new AnnualState(year, Array.AsReadOnly(trees)) };
                afterAreaBuilt?.Invoke(area.AreaId);
            }
            var committed = new MultiAreaWorkEvent(executionKey, plan.PlanId, year, workRevision, Array.AsReadOnly(changes));
            var nextStage4 = root.Stage4 is null ? null : root.Stage4 with
            {
                Pruning = pruning!,
                ApprovedPrunes = Array.Empty<MultiAreaPruneOrder>(),
                PruneLog = Array.AsReadOnly(root.Stage4.PruneLog
                    .Concat(pruneRecords.OrderBy(r => r.AreaId, StringComparer.Ordinal).ThenBy(r => r.TreeId, StringComparer.Ordinal))
                    .ToArray()),
            };
            // Stage 5: selective clearing (bramble and bracken to 0, every Sitka band kept); the year does not move.
            var nextStage5 = root.Stage5 is null || approvedClears.Count == 0 ? root.Stage5
                : Stage5Clearing.Apply(root.Stage5, approvedClears, year, workRevision, executionKey, plan.PlanId).Next with
                {
                    ApprovedClears = Array.Empty<MultiAreaClearUndergrowthOrder>(),
                };
            // Phase 2: SpotClear → Plant → Shelter; zero calendar advance, zero economy, zero stock.
            var nextPhase2 = root.Phase2 is null || approvedPlanting.Count == 0 ? root.Phase2
                : Phase2Planting.Apply(root.Phase2, approvedPlanting, year, workRevision, executionKey, plan.PlanId);

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
                Stage4 = nextStage4,
                Stage5 = nextStage5,
                Phase2 = nextPhase2,
            };
            return WithView(new MultiAreaExecutionResult(executionKey, year, workRevision, committed, Replayed: false), null);
        }
    }
}
