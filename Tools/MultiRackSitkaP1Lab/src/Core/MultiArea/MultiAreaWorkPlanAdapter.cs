namespace SitkaP1.Core;

/// <summary>
/// The facade the UI process calls for the MA-1 two-area session. It holds no tree, clock, plan or RNG state of its own:
/// every value comes from the engine under the engine's single lock, and every command is fenced by session, full source
/// token and plan-state revision before anything can change. It exposes no fault seam and accepts no caller-built plan.
/// A refusal changed nothing, so it is returned with a freshly read current view; any other exception propagates, the
/// outcome is then unconfirmed and the caller must re-read (fail closed), as in <see cref="PlayerWorkPlanAdapter"/>.
/// </summary>
internal sealed class MultiAreaWorkPlanAdapter
{
    private readonly MultiAreaContinuation session;
    private readonly byte[] manifest;
    internal string ExperimentKey { get; }
    internal string ExperimentBadge { get; }

    internal MultiAreaWorkPlanAdapter(MultiAreaContinuation session, MultiAreaExperiment experiment,
        string experimentKey = MultiAreaDemoExperiments.Ma1Key, string? experimentBadge = null)
    {
        this.session = session;
        manifest = experiment.ManifestFileBytes();
        ExperimentKey = experimentKey;
        ExperimentBadge = experimentBadge ?? MultiAreaDemoExperiments.BadgeFor(experimentKey);
    }

    /// <summary>
    /// Generate the synthetic two-area experiment in code (no file is read) and start one in-memory session. The /multi
    /// page's session defaults to Phase 2 (<c>1/1/1/1/1</c>) on standard synthetic MA-1 (all Sitka). Any supported
    /// selection can still be chosen explicitly, including the frozen <see cref="MultiAreaModelSelection.Legacy"/>, which
    /// stays the Core default ctor.
    /// </summary>
    internal static MultiAreaWorkPlanAdapter OpenSynthetic(MultiAreaModelSelection? models = null) =>
        OpenExperiment(MultiAreaDemoExperiments.Ma1Key, models);

    /// <summary>
    /// Opens one supported synthetic <c>/multi</c> experiment (<c>ma-1</c> or <c>ma-1-mixed</c>) on Phase 2
    /// (<c>1/1/1/1/1</c>) by default. Refuses unknown experiment keys.
    /// </summary>
    internal static MultiAreaWorkPlanAdapter OpenExperiment(string experimentKey, MultiAreaModelSelection? models = null)
    {
        var selection = models ?? MultiAreaModelSelection.Phase2;
        if (string.Equals(experimentKey, MultiAreaDemoExperiments.Ma1Key, StringComparison.Ordinal))
        {
            var experiment = MultiAreaGenerator.Generate();
            return new MultiAreaWorkPlanAdapter(new MultiAreaContinuation(experiment, selection), experiment,
                MultiAreaDemoExperiments.Ma1Key, MultiAreaDemoExperiments.Ma1Badge);
        }
        if (string.Equals(experimentKey, MultiAreaDemoExperiments.Ma1MixedKey, StringComparison.Ordinal))
        {
            if (!selection.IsPhase2)
                throw new P1Refusal("Phase2ModelRequired", "ma-1-mixed requires the 1/1/1/1/1 Phase 2 model selection");
            var (experiment, options, badge) = MultiAreaDemoExperiments.CreateMixedDemo();
            return new MultiAreaWorkPlanAdapter(new MultiAreaContinuation(experiment, selection, null, null, options),
                experiment, MultiAreaDemoExperiments.Ma1MixedKey, badge);
        }
        throw new P1Refusal("MultiAreaUnknownExperiment",
            "supported --multi-experiment values are ma-1 and ma-1-mixed");
    }

    /// <summary>The canonical manifest bytes of this session's experiment (a fresh copy). SHA-256 of its body is the content hash.</summary>
    internal byte[] ManifestBytes() => (byte[])manifest.Clone();

    internal MultiAreaSessionView Read() => session.ReadView();

    /// <summary>The view and its UI read-out (Stage 4, canopy, deadwood, ground cells) from one root.</summary>
    internal MultiAreaUiSnapshot ReadSnapshot() => session.ReadUiSnapshot();

    /// <summary>Atomic composite snapshot (view, Stage 4 readout, compact ecology DTO, Stage 4/5/Phase 2 views) from one root under one lock.</summary>
    internal MultiAreaCompositeUiSnapshot ReadCompositeUiSnapshot() => session.ReadCompositeUiSnapshot();

    /// <summary>The UI read-out of the root <paramref name="view"/> came from, or null if the session has moved on since.</summary>
    internal MultiAreaUiReadout? ReadoutFor(MultiAreaSessionView view) => session.ReadUiReadout(view.Source, view.PlanStateRevision);

    /// <summary>Paired Stage 4 + Ecology readouts of the root <paramref name="view"/> came from, or null if moved (zero torn frames).</summary>
    internal MultiAreaCompositeUiReadout? CompositeFor(MultiAreaSessionView view) =>
        session.ReadCompositeUiFor(view.Source, view.PlanStateRevision);

    /// <summary>Read-only cell detail inspection (POST api/multi/cell); refusals return a fresh composite snapshot with zero mutation.</summary>
    internal MultiAreaCellDetailResult ReadCellDetail(MultiAreaFence fence, string areaId, int column, int row)
    {
        try { return session.ReadCellDetail(fence, areaId, column, row); }
        catch (P1Refusal refusal)
        {
            return new MultiAreaCellDetailResult(session.ReadCompositeUiSnapshot(), null, refusal.Code, refusal.Message);
        }
    }

    /// <summary>Queue one Prune (Pending only) at the engine's next scheduled lift height. Refusals return a fresh view.</summary>
    internal MultiAreaCommandResult AddPrune(MultiAreaFence fence, string areaId, string treeId) =>
        Refusable(() => session.AddScheduledPrune(fence, areaId, treeId));

    internal MultiAreaCommandResult RemovePrune(MultiAreaFence fence, string areaId, string treeId) =>
        Refusable(() => session.RemovePrune(fence, areaId, treeId));

    internal MultiAreaCommandResult MarkCrop(MultiAreaFence fence, string areaId, string treeId) =>
        Refusable(() => session.MarkCrop(fence, areaId, treeId));

    internal MultiAreaCommandResult UnmarkCrop(MultiAreaFence fence, string areaId, string treeId) =>
        Refusable(() => session.UnmarkCrop(fence, areaId, treeId));

    internal MultiAreaCommandResult AddPending(MultiAreaFence fence, string areaId, string treeId, string reason) =>
        Refusable(() => session.AddPending(fence, areaId, treeId, reason));

    internal MultiAreaCommandResult RemovePending(MultiAreaFence fence, string areaId, string treeId) =>
        Refusable(() => session.RemovePending(fence, areaId, treeId));

    internal MultiAreaCommandResult Approve(MultiAreaFence fence) => Refusable(() => session.Approve(fence));

    internal MultiAreaCommandResult CancelApproved(MultiAreaFence fence) => Refusable(() => session.CancelApproved(fence));

    internal MultiAreaCommandResult Execute(MultiAreaFence fence, string planId, string executionKey) =>
        Refusable(() => session.Execute(fence, planId, executionKey));

    internal MultiAreaCommandResult Advance(MultiAreaFence fence, bool consentToCancelWork) =>
        Refusable(() => session.Advance(fence, consentToCancelWork));

    internal MultiAreaCompetitionResult Competition(MultiAreaFence fence, string areaId, string treeId)
    {
        try { return session.Competition(fence, areaId, treeId); }
        catch (P1Refusal refusal) { return new MultiAreaCompetitionResult(session.ReadView(), areaId, null, refusal.Code, refusal.Message); }
    }

    internal MultiAreaHitResult HitTest(MultiAreaFence fence, string areaId, double u, double v)
    {
        try { return session.HitTest(fence, areaId, u, v); }
        catch (P1Refusal refusal)
        {
            return new MultiAreaHitResult(session.ReadView(), areaId, Array.AsReadOnly(Array.Empty<PlayerMapItem>()),
                refusal.Code, refusal.Message);
        }
    }

    private MultiAreaCommandResult Refusable(Func<MultiAreaCommandResult> command)
    {
        try { return command(); }
        catch (P1Refusal refusal) { return new MultiAreaCommandResult(session.ReadView(), null, null, refusal.Code, refusal.Message); }
    }
}
