using System.Linq;

namespace SitkaP1.Core;

/// <summary>
/// G2 typed intervention contract (Stage 5 §8, owner policies D-093).
///
/// These types are session-local and in-process only. Nothing here promises crash durability, save/load, a Host
/// authority or economy values (Stage 5 G3/G4 remain deferred). A Fell is a real, typed, source-authoritative state
/// change committed inside the same immutable session root as the roster and the event; it is never a preview.
/// </summary>
public enum WorkAction
{
    /// <summary>Harvest one exact living source tree. The only intervention implemented in this slice.</summary>
    Fell,
}

/// <summary>The single undivided Stage 5 management area. Carried explicitly so a later multi-area slice need not change ID semantics.</summary>
public static class ManagementScope
{
    public const string WholeUndividedThirtyMetres = "MS-P30-UNDIVIDED-30M-V1";
}

/// <summary>
/// The staleness token a Work Plan binds to. It names the exact source (fixture file bytes and canonical content),
/// both clocks (ecological year and annual revision), the separate work revision, the management scope and the
/// model lineage. Any mismatch refuses the whole transaction with zero mutation.
/// </summary>
public sealed record WorkSourceToken(
    string Domain,
    string FixtureId,
    string FixtureFileSha256,
    string FixtureContentHash,
    string ManagementScopeId,
    int EcologicalYear,
    int AnnualRevision,
    int WorkRevision,
    int RngModelVersion,
    int GrowthModelVersion,
    string P1AlgorithmVersion,
    long ActionEligibleDbhThresholdMm);

/// <summary>One reviewed order for one exact, case-sensitive, live source tree ID.</summary>
public sealed record WorkOrder(string TreeId, WorkAction Action, string Reason);

/// <summary>An immutable, explicitly approved whole batch. It is the only input <c>Execute</c> accepts.</summary>
public sealed record WorkPlan(string PlanId, WorkSourceToken Source, IReadOnlyList<WorkOrder> Orders);

/// <summary>The typed Fell cause. Deliberately distinct from the natural <c>AnnualDeath</c> causes ("self-thinning" + hazard/boundary).</summary>
public static class FellCause
{
    public const string Harvest = "fell";
    public const string Mechanism = "manual-work-plan";
}

/// <summary>One executed Fell, kept in the typed session event ledger (never in the natural-death cause string).</summary>
public sealed record FellRecord(
    string TreeId,
    string Cause,
    string Mechanism,
    string Reason,
    int EcologicalYear,
    int WorkRevision,
    string ExecutionKey,
    string PlanId);

/// <summary>One committed work event. A batch publishes exactly one of these, together with the roster and work revision.</summary>
public sealed record WorkEvent(
    string ExecutionKey,
    string PlanId,
    int EcologicalYear,
    int WorkRevision,
    AnnualStand Before,
    AnnualStand After,
    IReadOnlyList<FellRecord> Fells);

/// <summary>The recorded result of an execution key. <see cref="Replayed"/> is true for an identical same-process retry.</summary>
public sealed record WorkExecutionResult(
    string ExecutionKey,
    int EcologicalYear,
    int WorkRevision,
    WorkEvent Event,
    bool Replayed);

/// <summary>
/// The immutable committed-current frame a reader or planner may trust. It is built from the committed session root,
/// never from a preview, an exporter projection or a mutable continuation.
/// </summary>
public sealed record CommittedFrame(
    string ManagementScopeId,
    string Domain,
    string FixtureId,
    string FixtureFileSha256,
    string FixtureContentHash,
    int EcologicalYear,
    int AnnualRevision,
    int WorkRevision,
    IReadOnlyList<AnnualTree> Trees,
    AnnualStand Stand,
    IReadOnlyList<WorkEvent> Events);

/// <summary>
/// The immutable source/model identity carried into every token. Separated from the two clocks so a work revision does
/// not have to pretend to be an annual revision.
/// </summary>
public sealed record WorkSourceIdentity(
    string Domain,
    string FixtureId,
    string FixtureFileSha256,
    string FixtureContentHash,
    int RngModelVersion,
    int GrowthModelVersion,
    string P1AlgorithmVersion,
    long ActionEligibleDbhThresholdMm);

internal static class WorkRules
{
    /// <summary>
    /// Stand aggregates over living stems only. Kept byte-identical in arithmetic to the frozen <c>AnnualRules.Stand</c>
    /// (which this task is not allowed to change), so a work report and an annual report can never disagree silently.
    /// </summary>
    internal static AnnualStand Summarize(IEnumerable<AnnualTree> trees, float areaHa)
    {
        int count = 0;
        double squaredDbhSum = 0.0;
        foreach (var tree in trees)
        {
            if (!tree.IsLiving) continue;
            count++;
            squaredDbhSum += (double)tree.DbhCm * tree.DbhCm;
        }
        if (count == 0) return new AnnualStand(0, 0, 0, 0, 0);

        double stemsPerHa = count / areaHa;
        double dq = Math.Sqrt(squaredDbhSum / count);
        double basalAreaPerHa = squaredDbhSum * (Math.PI / 40000.0) / areaHa;
        return new AnnualStand(count, stemsPerHa, basalAreaPerHa, dq, SitkaGrowthModel.RelativeDensity(stemsPerHa, dq));
    }

    /// <summary>
    /// D-093 P2. A batch that would leave no living stem is refused as a whole. This is owner policy, not a legal rule.
    /// </summary>
    internal static void RequireNonEmptyStand(AnnualState state, IReadOnlyList<WorkOrder> orders)
    {
        var living = state.Trees.Where(tree => tree.IsLiving).Select(tree => tree.TreeId)
            .ToHashSet(StringComparer.Ordinal);
        var selected = new HashSet<string>(StringComparer.Ordinal);
        foreach (var order in orders) selected.Add(order.TreeId);
        if (living.Count > 0 && selected.IsSupersetOf(living))
            throw new P1Refusal("WorkWouldLeaveNoLivingStems",
                "the approved batch would leave no living stems; the entire batch is refused");
    }
}
