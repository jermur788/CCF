namespace SitkaP1.Core;

/// <summary>
/// The public player facade over the one in-memory annual continuation (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §6.4–6.7).
/// It holds no tree, counter, RNG, clock or plan state of its own: every value it returns is read from the engine under the
/// engine's single lock, and every command is fenced by session, full source token and plan-state revision before anything
/// can change. It exposes no test seam, no legacy unfenced advance and no way to pass a caller-built Work Plan.
/// </summary>
public sealed class PlayerWorkPlanAdapter
{
    private readonly AnnualContinuation continuation;

    internal PlayerWorkPlanAdapter(AnnualContinuation continuation) => this.continuation = continuation;

    /// <summary>Open the trusted, catalog-checked 30 m fixture and start one in-memory session. Nothing is ever saved.</summary>
    public static PlayerWorkPlanAdapter Open(string fixtureDirectory) =>
        new(AnnualEngine.Open(P1Domain.DerivedP1ThirtyMetreExperimentV1, ManifestGenerator.FixtureId, fixtureDirectory)
            .ContinueInMemory());

    /// <summary>One atomic read of the current session.</summary>
    public PlayerSessionView Read() => continuation.ReadPlayerView();

    /// <summary>Add one exact living tree ID with a written reason to the pending batch. Fells nothing.</summary>
    public PlayerCommandResult AddPending(PlayerFence fence, string treeId, string reason) =>
        Refusable(() => continuation.AddPending(fence, treeId, reason));

    public PlayerCommandResult RemovePending(PlayerFence fence, string treeId) =>
        Refusable(() => continuation.RemovePending(fence, treeId));

    /// <summary>Approve the whole pending batch under an engine-minted PlanId. Fells nothing.</summary>
    public PlayerCommandResult Approve(PlayerFence fence) => Refusable(() => continuation.Approve(fence));

    public PlayerCommandResult CancelApproved(PlayerFence fence) => Refusable(() => continuation.CancelApproved(fence));

    /// <summary>Execute the approved plan named by its minted PlanId and ExecutionKey: one atomic typed Fell, or nothing.</summary>
    public PlayerCommandResult Execute(PlayerFence fence, string planId, string executionKey) =>
        Refusable(() => continuation.Execute(fence, planId, executionKey));

    /// <summary>Advance one genuine ecological year; unexecuted work needs explicit consent and is then cancelled at zero Fell.</summary>
    public PlayerCommandResult Advance(PlayerFence fence, bool consentToCancelWork) =>
        Refusable(() => continuation.Advance(fence, consentToCancelWork));

    /// <summary>Exact-ID map hit-test on the current view, in map metres (U east of the west edge, V south of the north edge).</summary>
    public PlayerHitTestResult HitTest(PlayerFence fence, double u, double v)
    {
        try { return continuation.HitTest(fence, u, v); }
        catch (P1Refusal refusal)
        {
            return new PlayerHitTestResult(continuation.ReadPlayerView(), Array.AsReadOnly(Array.Empty<PlayerMapItem>()),
                refusal.Code, refusal.Message);
        }
    }

    // A refusal changed nothing, so it is returned with a freshly read current view for the caller to redraw (spec §6.6).
    // Any other exception propagates: the outcome is then unconfirmed and the caller must re-read (fail closed).
    private PlayerCommandResult Refusable(Func<PlayerCommandResult> command)
    {
        try { return command(); }
        catch (P1Refusal refusal)
        {
            return new PlayerCommandResult(continuation.ReadPlayerView(), null, null, refusal.Code, refusal.Message);
        }
    }
}
