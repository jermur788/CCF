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
    IReadOnlyList<MultiAreaYearRecord> AnnualHistory,
    MultiAreaStage4State? Stage4 = null) // Stage 4 sessions only (D-095); null on every model 0 / Stage 1–3 root
{
    /// <summary>True when any Fell or Prune order is pending or approved and not yet executed.</summary>
    internal bool HasUnexecutedWork => Pending.Count > 0 || Approved is not null || (Stage4?.PendingPrunes.Count ?? 0) > 0;
}

/// <summary>The per-area annual step: the unchanged AnnualRules.Calculate and the same next-state construction as AnnualContinuation.</summary>
internal static class MultiAreaAnnual
{
    internal static (AnnualReport Report, AnnualState Next) Step(AnnualState state, int simulationSeed, float areaHa) =>
        Step(state, simulationSeed, areaHa, CrownModelVersion.LegacyFrozen, woodland: null);

    /// <summary>
    /// The same step with an explicit crown model. For CrownModel 1 <paramref name="woodland"/> is the connected-woodland
    /// neighbourhood read from the start-of-year root (never from another area's freshly stepped state), so area order
    /// cannot change any result and no area mutates another.
    /// </summary>
    internal static (AnnualReport Report, AnnualState Next) Step(AnnualState state, int simulationSeed, float areaHa,
        CrownModelVersion crownModel, AnnualTree[]? woodland)
    {
        var report = AnnualRules.Calculate(state, simulationSeed, areaHa, crownModel, woodland);
        var next = new AnnualState(report.YearAfter, Array.AsReadOnly(report.Trees.Select(row => row.After).ToArray()));
        return (report, next);
    }

    /// <summary>The Stage 4 step of one rack: woodland-driven growth, this rack's own density mortality, same next-state rule.</summary>
    internal static (AnnualReport Report, AnnualState Next) StepStage4(AnnualState state, int simulationSeed, float areaHa,
        AnnualTree[] woodland, Func<string, int> pruningLiftsOf, Stage4Options options)
    {
        var report = AnnualRules.CalculateStage4(state, simulationSeed, areaHa, woodland, pruningLiftsOf, options);
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
    private readonly CrownModelVersion crownModel;
    private readonly MultiAreaModelSelection models;
    private readonly Stage4Options? stage4Options; // non-null only for the full Stage 4 selection
    private readonly string areaModelVersion;
    private readonly IReadOnlyList<AreaInfo> areas; // ordinal by AreaId, the same order as root.Areas
    private readonly MultiAreaExperimentInfo experiment;
    private readonly string areaIds;
    // One random identity per in-memory session. It is never part of a token, report or manifest.
    private readonly string sessionId =
        Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
    private MultiAreaRoot root;
    // Session-only canopy/light read-outs (#900), one per light grid, valid for one committed tree state. Tree state changes
    // only on Execute (WorkRevision) or an annual step (AnnualRevision: natural death and crown growth), so Pending, Approve,
    // Crop marks and reads never recompute or change them. Never saved and never part of a token.
    private readonly Dictionary<double, (int AnnualRevision, int WorkRevision, CanopyLightReport Report)> canopyCache = new();
    private int canopyRecomputeCount;
    // The /multi ground cells (per-cell shape, cover fraction and contributing trees) for the cached 2.5 m report. Same
    // validity rule as canopyCache; session-only.
    private (int AnnualRevision, int WorkRevision, IReadOnlyList<IReadOnlyList<MultiAreaGroundCell>> Ground)? groundCache;

    internal MultiAreaContinuation(MultiAreaExperiment definition) : this(definition, CrownModelVersion.LegacyFrozen)
    {
    }

    /// <summary>
    /// Start a session with an explicit crown model. Model 0 is exactly the frozen MA-1 path. Model 1 (C2) derives each
    /// living tree's year-0 radius as r_0 = r_target(CI_0) over the connected genesis woodland; the experiment manifest, its
    /// content hash and every other genesis value are unchanged (a derived session path, not a new fixture).
    /// </summary>
    internal MultiAreaContinuation(MultiAreaExperiment definition, CrownModelVersion crownModel)
        : this(definition, new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, crownModel))
    {
    }

    /// <summary>
    /// Start a session with explicit adult, mortality and crown models (Stage 4, D-095). Only the frozen selection, the
    /// Stage 1–3 crown selection and the full Stage 4 selection are accepted. <paramref name="stage4"/> carries the neutral
    /// site-productivity field and the species profile; it is ignored (and must be null) unless the selection is Stage 4.
    /// </summary>
    internal MultiAreaContinuation(MultiAreaExperiment definition, MultiAreaModelSelection models, Stage4Options? stage4 = null)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(models);
        models.RequireSupported();
        if (stage4 is not null && !models.IsStage4)
            throw new P1Refusal("Stage4ModelRequired", "Stage 4 options need the full Stage 4 model selection");
        var crownModel = models.Crown;
        definition.Validate();
        this.models = models;
        this.stage4Options = models.IsStage4 ? (stage4 ?? Stage4Options.Default).RequireValid() : null;
        this.crownModel = crownModel;
        bool connected = crownModel != CrownModelVersion.LegacyFrozen;
        areaModelVersion = models.IsStage4 ? MultiAreaIdentity.AreaModelVersionStage4
            : connected ? MultiAreaIdentity.AreaModelVersionConnectedCrownC2 : MultiAreaIdentity.AreaModelVersion;
        string contentHash = definition.ContentHash();
        experiment = new MultiAreaExperimentInfo(definition.ExperimentId, MultiAreaIdentity.Format, MultiAreaIdentity.SchemaVersion,
            MultiAreaIdentity.Domain, MultiAreaIdentity.SourceClass, contentHash, definition.GeneratorId,
            definition.GeneratorVersion, definition.GeneratorSeed, MultiAreaIdentity.ReviewStatus, MultiAreaIdentity.Disclosure,
            models.IsStage4 ? MultiAreaIdentity.EdgeModelStage4
                : connected ? MultiAreaIdentity.EdgeModelConnectedCrownC2 : MultiAreaIdentity.EdgeModel,
            models.IsStage4 ? MultiAreaIdentity.SiteNoteStage4 : MultiAreaIdentity.SiteNote);
        areas = Array.AsReadOnly(definition.Areas.Select(InfoOf).ToArray());
        areaIds = string.Join(",", definition.Areas.Select(area => area.AreaId));
        var genesis = definition.Areas.Select(MultiAreaExperiment.GenesisTrees).ToArray();
        if (connected)
        {
            var woodland = CrownModel.Woodland(genesis.SelectMany(trees => trees));
            genesis = genesis.Select(trees => CrownModel.Equilibrate(SpeciesProfile.Sitka, trees, woodland)).ToArray();
        }
        var states = definition.Areas.Select((area, i) => new MultiAreaAreaState(area.AreaId,
            new AnnualState(0, genesis[i]), Array.Empty<string>())).ToArray();
        root = new MultiAreaRoot(0, 0, 0, 0, Array.AsReadOnly(states), Array.Empty<MultiAreaWorkEvent>(),
            new Dictionary<string, MultiAreaExecutedWork>(StringComparer.Ordinal), Array.Empty<MultiAreaWorkOrder>(), null,
            Array.Empty<MultiAreaYearRecord>(), models.IsStage4 ? MultiAreaStage4State.Empty : null);
    }

    internal string SessionId => sessionId;

    internal CrownModelVersion SelectedCrownModel => crownModel;

    internal MultiAreaModelSelection SelectedModels => models;

    /// <summary>The whole view, built from ONE root reference under ONE lock acquisition.</summary>
    internal MultiAreaSessionView ReadView()
    {
        lock (sync) return ViewOf(root);
    }

    /// <summary>
    /// The Stage 4 read-out (life states, pruning, per-rack RD, natural deaths and deadwood) of ONE root under ONE lock.
    /// It is a separate read so the existing session view keeps its exact shape; it changes nothing. On a model 0 or
    /// Stage 1–3 session it still reports life states and RD, with no pruning or deadwood (those exist only in Stage 4).
    /// </summary>
    internal MultiAreaStage4View ReadStage4View()
    {
        lock (sync) return Stage4ViewOf(root);
    }

    /// <summary>
    /// The /multi page read: the session view and its full UI read-out (Stage 4 states, pruning eligibility, canopy summary,
    /// deadwood and the 2.5 m ground cells), both from ONE root under ONE lock. Changes nothing.
    /// </summary>
    internal MultiAreaUiSnapshot ReadUiSnapshot()
    {
        lock (sync)
        {
            var view = ViewOf(root);
            return new MultiAreaUiSnapshot(view, ReadoutOf(view));
        }
    }

    /// <summary>
    /// The UI read-out of the CURRENT root, but only when it is still the root named by <paramref name="source"/> and
    /// <paramref name="planStateRevision"/> (the root a command's view came from); null when anything moved since, so a
    /// read-out is never paired with a view of another root.
    /// </summary>
    internal MultiAreaUiReadout? ReadUiReadout(MultiAreaSourceToken source, int planStateRevision)
    {
        lock (sync)
        {
            if (source != Token(root) || planStateRevision != root.PlanStateRevision) return null;
            return ReadoutOf(ViewOf(root));
        }
    }

    // Caller holds sync. The canopy report and the ground cells are cached per committed tree state (AnnualRevision and
    // WorkRevision), like ReadCanopyLight: Pending, Approve, Crop marks and reads reuse them.
    private MultiAreaUiReadout ReadoutOf(MultiAreaSessionView view)
    {
        var canopy = ReadCanopyLight(CanopyLightEvaluator.DefaultLightCellM);
        if (groundCache is null || groundCache.Value.AnnualRevision != root.AnnualRevision ||
            groundCache.Value.WorkRevision != root.WorkRevision)
        {
            var woodland = CrownModel.Woodland(view.Areas.SelectMany(area => area.Trees));
            var ground = view.Areas.Select((area, i) => MultiAreaUiReadouts.GroundOf(area, canopy.Areas[i], woodland)).ToArray();
            groundCache = (root.AnnualRevision, root.WorkRevision, Array.AsReadOnly(ground));
        }
        return MultiAreaUiReadouts.Build(view, Stage4ViewOf(root), canopy, models, groundCache.Value.Ground);
    }

    // Caller holds sync. The body of ReadStage4View, unchanged, over the root passed in.
    private MultiAreaStage4View Stage4ViewOf(MultiAreaRoot current)
    {
        var stage4 = current.Stage4 ?? MultiAreaStage4State.Empty;
        var last = current.AnnualHistory.Count == 0 ? null : current.AnnualHistory[^1];
        var areaViews = new MultiAreaStage4AreaView[current.Areas.Count];
        for (int i = 0; i < areaViews.Length; i++)
        {
            var area = current.Areas[i];
            var trees = area.State.Trees.Select(tree =>
            {
                var pruning = stage4.PruningOf(tree.TreeId);
                return new MultiAreaStage4TreeView(tree.TreeId, LifeStates.Of(tree),
                    LifeStates.Of(tree) == LifeState.BiologicallyDead ? tree.Death!.Cause : null,
                    LifeStates.Of(tree) == LifeState.BiologicallyDead ? tree.Death!.EcologicalYear : null,
                    pruning.Lifts, pruning.CrownBaseHeightM, pruning.LastPruningYear, tree.CrownRadiusM);
            }).ToArray();
            int annual = last?.Areas[i].NaturalDeaths.Count ?? 0;
            int cumulative = current.AnnualHistory.Sum(year => year.Areas[i].NaturalDeaths.Count);
            areaViews[i] = new MultiAreaStage4AreaView(area.AreaId, areas[i].AreaHa,
                WorkRules.Summarize(area.State.Trees, areas[i].AreaHa).RelativeDensity, annual, cumulative,
                Array.AsReadOnly(stage4.Deadwood.Where(record => record.AreaId == area.AreaId).ToArray()),
                Array.AsReadOnly(trees));
        }
        return new MultiAreaStage4View(sessionId, current.EcologicalYear, current.PlanStateRevision, Token(current),
            models.Adult, models.Mortality, models.Crown, Stage4Evidence.All, Array.AsReadOnly(areaViews),
            Array.AsReadOnly(stage4.PendingPrunes.ToArray()), Array.AsReadOnly(stage4.ApprovedPrunes.ToArray()),
            Array.AsReadOnly(stage4.PruneLog.ToArray()));
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
            if (root.Pending.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)) ||
                (root.Stage4?.PendingPrunes.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)) ?? false))
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

    /// <summary>
    /// Stage 4: add one fenced Prune order (one exact living tree of one area, target crown-base height). Pending only: no
    /// tree, crown or clock changes. The verified guards are checked now for a clean refusal and again at Execute.
    /// A tree may carry at most one order (Fell or Prune) in a plan; a retained Crop Tree may be pruned.
    /// </summary>
    internal MultiAreaCommandResult AddPrune(MultiAreaFence fence, string areaId, string treeId, float targetCrownBaseHeightM)
    {
        lock (sync)
        {
            RequireFence(fence);
            var stage4 = RequireStage4();
            int index = AreaIndex(areaId);
            if (string.IsNullOrWhiteSpace(treeId))
                throw new P1Refusal("UnknownTreeId", "a tree ID is required");
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is awaiting execution; cancel it before editing");
            if (root.Pending.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)) ||
                stage4.PendingPrunes.Any(order => string.Equals(order.TreeId, treeId, StringComparison.Ordinal)))
                throw new P1Refusal("WorkDuplicateTreeId", "the tree ID is already in the pending Work Plan");
            RequireLivingInArea(index, treeId, unknownCode: "UnknownTreeId", deadCode: "PruneTreeNotLiving",
                "only a living exact tree ID of this area can be pruned");
            PruningRules.Require(FindTree(index, treeId)!, stage4.PruningOf(treeId), targetCrownBaseHeightM, root.EcologicalYear);
            var pending = stage4.PendingPrunes.Append(new MultiAreaPruneOrder(areaId, treeId, targetCrownBaseHeightM)).ToArray();
            root = root with
            {
                Stage4 = stage4 with { PendingPrunes = Array.AsReadOnly(pending) },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    /// <summary>
    /// Stage 4, /multi page: add one Prune order whose target crown-base height is the engine's next scheduled lift for that
    /// tree (<see cref="PruningRules.NextLiftTargetM"/>, the Unity Sitka schedule). The player never sends a target. Every
    /// fence, guard and refusal is exactly that of <see cref="AddPrune"/> (the lock is re-entrant, so it sees the same root).
    /// </summary>
    internal MultiAreaCommandResult AddScheduledPrune(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            int lifts = (root.Stage4 ?? MultiAreaStage4State.Empty).PruningOf(treeId ?? string.Empty).Lifts;
            return AddPrune(fence, areaId, treeId!, PruningRules.NextLiftTargetM(lifts));
        }
    }

    /// <summary>Stage 4: remove one pending Prune order before approval. Never touches biology.</summary>
    internal MultiAreaCommandResult RemovePrune(MultiAreaFence fence, string areaId, string treeId)
    {
        lock (sync)
        {
            RequireFence(fence);
            var stage4 = RequireStage4();
            AreaIndex(areaId);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "the plan is approved; cancel it before editing");
            var match = stage4.PendingPrunes.FirstOrDefault(order => order.TreeId == treeId);
            if (match is null)
                throw new P1Refusal("UnknownWorkOrder", "the tree ID has no pending Prune order");
            if (match.AreaId != areaId)
                throw new P1Refusal("MultiAreaWrongArea", "the pending Prune for " + treeId + " belongs to " + match.AreaId + ", not " + areaId);
            var pending = stage4.PendingPrunes.Where(order => !ReferenceEquals(order, match)).ToArray();
            root = root with
            {
                Stage4 = stage4 with { PendingPrunes = Array.AsReadOnly(pending) },
                PlanStateRevision = checked(root.PlanStateRevision + 1),
            };
            return WithView(null, null);
        }
    }

    /// <summary>
    /// Approve the whole cross-area pending plan under an engine-minted PlanId and key. Zero Fell, and (Stage 4) zero
    /// pruning: pending Prune orders move into the approval with the Fell orders and nothing ecological changes.
    /// </summary>
    internal MultiAreaCommandResult Approve(MultiAreaFence fence)
    {
        lock (sync)
        {
            RequireFence(fence);
            if (root.Approved is not null)
                throw new P1Refusal("WorkPlanAlreadyApproved", "an approved plan is already awaiting execution");
            var stage4 = root.Stage4;
            if (root.Pending.Count == 0 && (stage4?.PendingPrunes.Count ?? 0) == 0)
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
            if (plan.Orders.Count == 0 && approvedPrunes.Count == 0)
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
                PruningRules.Require(FindTree(index, order.TreeId)!, root.Stage4!.PruningOf(order.TreeId),
                    order.TargetCrownBaseHeightM, root.EcologicalYear);
                if (!prunesByArea.TryGetValue(order.AreaId, out var inArea))
                    prunesByArea[order.AreaId] = inArea = new Dictionary<string, MultiAreaPruneOrder>(StringComparer.Ordinal);
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
            bool hasWork = root.HasUnexecutedWork;
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
            // CrownModel 1 only: the start-of-year connected woodland, shared read-only by every area's step.
            AnnualTree[]? woodland = crownModel == CrownModelVersion.LegacyFrozen
                ? null
                : CrownModel.Woodland(root.Areas.SelectMany(area => area.State.Trees));
            // Stage 4 only: deadwood records are numbered in area order, then in the order the rules recorded the deaths.
            var stage4 = root.Stage4;
            var deadwood = new List<FallenDeadwoodRecord>();
            int nextDeadwood = stage4?.NextDeadwoodNumber ?? 1;
            for (int i = 0; i < root.Areas.Count; i++)
            {
                var area = root.Areas[i];
                var info = areas[i];
                AnnualState next;
                if (area.State.Trees.Any(tree => tree.IsLiving))
                {
                    var (report, stepped) = stage4 is not null
                        ? MultiAreaAnnual.StepStage4(area.State, info.Seed, info.AreaHa, woodland!,
                            treeId => stage4.PruningOf(treeId).Lifts, stage4Options!)
                        : crownModel == CrownModelVersion.LegacyFrozen
                            ? MultiAreaAnnual.Step(area.State, info.Seed, info.AreaHa)
                            : MultiAreaAnnual.Step(area.State, info.Seed, info.AreaHa, crownModel, woodland);
                    next = stepped;
                    if (stage4 is not null)
                    {
                        var byId = stepped.Trees.ToDictionary(tree => tree.TreeId, StringComparer.Ordinal);
                        foreach (var death in report.Deaths)
                        {
                            var dead = byId[death.TreeId];
                            deadwood.Add(new FallenDeadwoodRecord(FallenDeadwoodRecord.IdFor(nextDeadwood++), dead.TreeId,
                                area.AreaId, SpeciesProfile.Sitka.SpeciesId, dead.XM, dead.ZM, death.EcologicalYear, death.Cause,
                                dead.DbhCm, dead.HeightM, FallenDeadwoodRecord.StemVolumeM3(dead.DbhCm, dead.HeightM)));
                        }
                    }
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
                // Stage 4: consented cancellation clears Prune orders with the Fells; deadwood is appended, never rewritten.
                Stage4 = stage4 is null ? null : stage4 with
                {
                    PendingPrunes = hasWork ? Array.Empty<MultiAreaPruneOrder>() : stage4.PendingPrunes,
                    ApprovedPrunes = hasWork ? Array.Empty<MultiAreaPruneOrder>() : stage4.ApprovedPrunes,
                    Deadwood = Array.AsReadOnly(stage4.Deadwood.Concat(deadwood).ToArray()),
                    NextDeadwoodNumber = nextDeadwood,
                },
            };
            return WithView(null, new MultiAreaAnnualResult(year, checked(year + 1), Array.AsReadOnly(reports)));
        }
    }

    // ------------------------------------------------------------------------------------------------ canopy and light (#900)

    /// <summary>
    /// Read-only canopy cover, growing-space and modelled light read-out of the CURRENT committed trees, per area polygon,
    /// with the connected woodland as the neighbourhood. Computed at most once per committed tree state and light grid
    /// (load, Execute, natural death, annual crown growth); Pending, Approve, Crop marks and repeated reads return the same
    /// cached value. Session-only: nothing is saved and no root, token or revision changes.
    /// </summary>
    internal CanopyLightReport ReadCanopyLight(double lightCellM = CanopyLightEvaluator.DefaultLightCellM)
    {
        CanopyLightEvaluator.RequireSupportedGrid(lightCellM);
        lock (sync)
        {
            if (canopyCache.TryGetValue(lightCellM, out var cached) &&
                cached.AnnualRevision == root.AnnualRevision && cached.WorkRevision == root.WorkRevision)
                return cached.Report;
            var input = new CanopyArea[root.Areas.Count];
            for (int i = 0; i < input.Length; i++)
                input[i] = new CanopyArea(areas[i].AreaId,
                    Array.AsReadOnly(areas[i].Polygon.Select(v => new CanopyVertex(v.XM, v.ZM)).ToArray()), areas[i].AreaHa,
                    root.Areas[i].State.Trees);
            var report = CanopyLightEvaluator.Evaluate(input, lightCellM, crownModel);
            canopyCache[lightCellM] = (root.AnnualRevision, root.WorkRevision, report);
            canopyRecomputeCount++;
            return report;
        }
    }

    /// <summary>How many times a canopy/light read-out has been computed in this session (for recompute-trigger tests).</summary>
    internal int CanopyRecomputeCount
    {
        get { lock (sync) return canopyRecomputeCount; }
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

    private MultiAreaStage4State RequireStage4() => root.Stage4 ??
        throw new P1Refusal("Stage4ModelRequired",
            "pruning needs AdultModelVersion 1, MortalityModelVersion 1 and CrownModelVersion 1; this session is not Stage 4");

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
        areaModelVersion, MultiAreaIdentity.ActionEligibleDbhThresholdMm);

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
