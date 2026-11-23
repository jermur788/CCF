namespace SitkaP1.Core;

// Stage 4 (D-095) session state and read-only views. These are NEW records: no existing view record (MultiAreaSessionView,
// MultiAreaAreaView, AnnualTree, MultiAreaWorkOrder, ...) changes shape, so the /multi page and PlayerMultiApi JSON are
// untouched and the UI stays on model 0. Session-only: nothing here is saved.

/// <summary>One fenced Prune order: one exact living tree of one area and its target crown-base height in metres.</summary>
internal sealed record MultiAreaPruneOrder(string AreaId, string TreeId, float TargetCrownBaseHeightM);

/// <summary>One executed pruning lift, recorded with its plan, key, year and the immediate crown change.</summary>
internal sealed record MultiAreaPruneRecord(string AreaId, string TreeId, float TargetCrownBaseHeightM, int EcologicalYear,
    int WorkRevision, string ExecutionKey, string PlanId, float CrownRadiusBeforeM, float CrownRadiusAfterM, int LiftsAfter);

/// <summary>
/// The Stage 4 part of the one commit root. It is published together with the rest of <see cref="MultiAreaRoot"/>, so a
/// reader never sees a tree's crown cut without its pruning history, or a death without its deadwood record.
/// </summary>
internal sealed record MultiAreaStage4State(
    IReadOnlyDictionary<string, TreePruningState> Pruning,
    IReadOnlyList<MultiAreaPruneOrder> PendingPrunes,
    IReadOnlyList<MultiAreaPruneOrder> ApprovedPrunes,
    IReadOnlyList<MultiAreaPruneRecord> PruneLog,
    IReadOnlyList<FallenDeadwoodRecord> Deadwood,
    int NextDeadwoodNumber)
{
    public static readonly MultiAreaStage4State Empty = new(
        new Dictionary<string, TreePruningState>(StringComparer.Ordinal), Array.Empty<MultiAreaPruneOrder>(),
        Array.Empty<MultiAreaPruneOrder>(), Array.Empty<MultiAreaPruneRecord>(), Array.Empty<FallenDeadwoodRecord>(), 1);

    public TreePruningState PruningOf(string treeId) =>
        Pruning.TryGetValue(treeId, out var state) ? state : TreePruningState.Unpruned;
}

/// <summary>Per-tree Stage 4 read-out: life state, death cause/year and pruning history.</summary>
internal sealed record MultiAreaStage4TreeView(string TreeId, LifeState LifeState, string? DeathCause, int? DeathYear,
    int PruningLifts, float CrownBaseHeightM, int LastPruningYear, float CrownRadiusM);

/// <summary>
/// Per-rack Stage 4 read-out: the rack's own relative density (its living stems over its own polygon area), the natural
/// deaths of the last committed year and of the whole session, and its immutable fallen-deadwood records.
/// </summary>
internal sealed record MultiAreaStage4AreaView(string AreaId, float AreaHa, double RelativeDensity, int AnnualNaturalDeaths,
    int CumulativeNaturalDeaths, IReadOnlyList<FallenDeadwoodRecord> Deadwood, IReadOnlyList<MultiAreaStage4TreeView> Trees);

/// <summary>The Stage 4 read-out of one committed root, with its model versions and evidence labels.</summary>
internal sealed record MultiAreaStage4View(string SessionId, int EcologicalYear, int PlanStateRevision, MultiAreaSourceToken Source,
    AdultModelVersion AdultModelVersion, MortalityModelVersion MortalityModelVersion, CrownModelVersion CrownModelVersion,
    IReadOnlyList<string> EvidenceLabels, IReadOnlyList<MultiAreaStage4AreaView> Areas,
    IReadOnlyList<MultiAreaPruneOrder> PendingPrunes, IReadOnlyList<MultiAreaPruneOrder> ApprovedPrunes,
    IReadOnlyList<MultiAreaPruneRecord> PruneLog);
