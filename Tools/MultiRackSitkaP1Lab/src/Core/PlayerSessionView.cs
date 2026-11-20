namespace SitkaP1.Core;

// Public, immutable DTOs of the one-area playable Stand Map slice (spec MULTI-STANDMAP-PLAYABLE-2D-V1 §6.1). They carry
// engine values only: nothing here is saved, and nothing here is a second source of ecological truth.

/// <summary>
/// The fence a player command echoes from the view it was read from: this session, the full source token (every field
/// is compared) and the plan-state revision. Any mismatch refuses the command before anything can change.
/// </summary>
public sealed record PlayerFence(string SessionId, WorkSourceToken Source, int PlanStateRevision);

/// <summary>
/// The engine's approved but unexecuted batch. <see cref="PlanId"/> and <see cref="ExecutionKey"/> are minted by the
/// engine; a player echoes them to Execute and never invents either.
/// </summary>
public sealed record PlayerApprovedPlan(string PlanId, string ExecutionKey, WorkSourceToken Source,
    IReadOnlyList<WorkOrder> Orders);

/// <summary>
/// One committed ecological year, recorded from the annual report itself: the actual stand before and after, that year's
/// natural deaths, and whether unexecuted work was cancelled at zero Fell with the player's consent.
/// </summary>
public sealed record AnnualYearRecord(int YearBefore, int YearAfter, int AnnualRevisionAfter, int WorkRevision,
    AnnualStand Before, AnnualStand After, IReadOnlyList<AnnualDeath> NaturalDeaths, bool CancelledUnexecutedWork);

/// <summary>The map square in source metres: X east from <see cref="MinXM"/>, Z north from <see cref="MinZM"/>, side <see cref="SideM"/>.</summary>
public sealed record PlayerMapExtent(double MinXM, double MinZM, double SideM, float AreaHa);

/// <summary>
/// One atomic read of the session. Every field comes from the same committed root under one lock acquisition, so the
/// frame, token, Work Plan and history can never disagree. Every collection is a read-only copy, and later commands never
/// change a view that was already returned.
/// </summary>
public sealed record PlayerSessionView(string SessionId, int PlanStateRevision, WorkSourceToken Source,
    CommittedFrame Frame, IReadOnlyList<WorkOrder> Pending, PlayerApprovedPlan? Approved,
    IReadOnlyList<AnnualYearRecord> AnnualHistory, PlayerMapExtent Extent);

/// <summary>
/// The outcome of one player command. When <see cref="RefusalCode"/> is set, nothing changed and <see cref="View"/> is a
/// freshly read current view. Otherwise <see cref="View"/> is the state the command produced.
/// </summary>
public sealed record PlayerCommandResult(PlayerSessionView View, WorkExecutionResult? Execution, AnnualReport? Annual,
    string? RefusalCode, string? RefusalMessage);

/// <summary>
/// The outcome of a read-only map hit-test: the exact-ID items whose drawn disc contains the point, topmost first. A stale
/// fence returns a refusal, no candidates and the fresh view, so the caller redraws and selects nothing.
/// </summary>
public sealed record PlayerHitTestResult(PlayerSessionView View, IReadOnlyList<PlayerMapItem> Candidates,
    string? RefusalCode, string? RefusalMessage);

/// <summary>
/// Fixed, truthful player-facing text (spec §7). A page shows these words or values from a view, and invents no label of
/// its own. In a template, {n} stands for a count and {y} for an ecological year.
/// </summary>
public static class PlayerLabels
{
    public const string Title = "Synthetic 30 m Sitka experiment";
    public const string Disclosure = P1Disclosure.Derived30;
    public const string ManagementArea =
        $"One undivided management area — {ManagementScope.WholeUndividedThirtyMetres} (not a rack)";
    public const string ComparisonExtension = "Comparison extension: DISABLED — Manual/NotCovered";
    public const string Money = "Costs, revenue, budget: UNAVAILABLE (not €0)";
    public const string SessionOnly = "Session only — nothing is saved";
    public const string NotCovered = "UNAVAILABLE / NOT COVERED";
    public const string ThresholdInformational = "170 mm is fixture metadata only; it does not restrict Fell (D-093)";
    public const string AllLiveBatchRefused = "A batch that would leave no living tree is refused as a whole (D-093)";

    public const string AddToWorkPlan = "Add to Work Plan — no tree is felled";
    public const string RemoveFromWorkPlan = "Remove from Work Plan";
    public const string ApproveWholePlan = "Approve whole Work Plan — 0 trees felled";
    public const string CancelApprovedPlan = "Cancel approved plan";
    public const string ExecuteApprovedPlan = "Execute approved plan";
    public const string ExecuteConfirmation = "Fells exactly {n} named trees in Year {y}, or refuses all";
    // The same confirmation for a plan of exactly one tree ({n} is then 1), so the wording is correct in the singular.
    public const string ExecuteConfirmationOneTree = "Fells exactly {n} named tree in Year {y}, or refuses all";
    public const string AlreadyExecuted = "Already executed — no additional Fell";
    public const string AdvanceYear = "Advance one ecological year";
    public const string ConsentKeepOrders = "Keep orders — stay in Year {y}";
    public const string ConsentCancelAndAdvance = "Cancel unexecuted work and advance one year";

    public const string NothingChanged = "Nothing changed";
    public const string OutcomeUnconfirmed = "Outcome unconfirmed — re-read current state";
    public const string SelectionAmbiguous = "Selection ambiguous — choose from the list";
    public const string CrownsOverlap = "{n} crowns overlap here";

    public const string StatusLiving = "Living";
    public const string StatusPendingFell = "Pending Fell (P)";
    public const string StatusApprovedFell = "Approved Fell (A)";
    public const string StatusFelled = "Felled Y{y}";
    public const string StatusNaturalDead = "Dead (natural) Y{y}";
}
