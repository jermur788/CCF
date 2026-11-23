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

    internal MultiAreaWorkPlanAdapter(MultiAreaContinuation session, MultiAreaExperiment experiment)
    {
        this.session = session;
        manifest = experiment.ManifestFileBytes();
    }

    /// <summary>
    /// Generate the synthetic two-area experiment in code (no file is read) and start one in-memory session. The /multi
    /// page's session is Stage 4 by default (owner decision 2026-11-23: CrownModelVersion 1, AdultModelVersion 1,
    /// MortalityModelVersion 1, canopy union and 2.5 m decoupled light). Any supported selection can still be chosen
    /// explicitly, including the frozen <see cref="MultiAreaModelSelection.Legacy"/>, which stays the Core default ctor.
    /// </summary>
    internal static MultiAreaWorkPlanAdapter OpenSynthetic(MultiAreaModelSelection? models = null)
    {
        var experiment = MultiAreaGenerator.Generate();
        return new MultiAreaWorkPlanAdapter(new MultiAreaContinuation(experiment, models ?? MultiAreaModelSelection.Stage4), experiment);
    }

    /// <summary>The canonical manifest bytes of this session's experiment (a fresh copy). SHA-256 of its body is the content hash.</summary>
    internal byte[] ManifestBytes() => (byte[])manifest.Clone();

    internal MultiAreaSessionView Read() => session.ReadView();

    /// <summary>The view and its UI read-out (Stage 4, canopy, deadwood, ground cells) from one root.</summary>
    internal MultiAreaUiSnapshot ReadSnapshot() => session.ReadUiSnapshot();

    /// <summary>The UI read-out of the root <paramref name="view"/> came from, or null if the session has moved on since.</summary>
    internal MultiAreaUiReadout? ReadoutFor(MultiAreaSessionView view) => session.ReadUiReadout(view.Source, view.PlanStateRevision);

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
