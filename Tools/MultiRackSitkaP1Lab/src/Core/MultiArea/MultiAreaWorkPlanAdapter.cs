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

    /// <summary>Generate the synthetic two-area experiment in code (no file is read) and start one in-memory session.</summary>
    internal static MultiAreaWorkPlanAdapter OpenSynthetic()
    {
        var experiment = MultiAreaGenerator.Generate();
        return new MultiAreaWorkPlanAdapter(new MultiAreaContinuation(experiment), experiment);
    }

    /// <summary>The canonical manifest bytes of this session's experiment (a fresh copy). SHA-256 of its body is the content hash.</summary>
    internal byte[] ManifestBytes() => (byte[])manifest.Clone();

    internal MultiAreaSessionView Read() => session.ReadView();

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
