namespace SitkaP1.Core;

// Immutable MA-1 multi-area DTOs (owner-approved MA-1 spec §3–§4). They are internal to this lab: the public #849 player
// facade, its types and its labels are unchanged. Every list in a returned value is an owned read-only copy, and no
// returned value changes afterwards. Nothing here is saved.

/// <summary>
/// The staleness token of the whole experiment: its exact source (manifest content hash and area set), both clocks, the
/// separate work revision and the model lineage. All fields are scalars, so record equality compares every one of them
/// (the area set is one ordinal, comma-joined string for that reason). Any change refuses a fenced command whole.
/// </summary>
internal sealed record MultiAreaSourceToken(
    string Domain,
    string ExperimentId,
    string ManifestContentHash,
    string AreaIds,
    int EcologicalYear,
    int AnnualRevision,
    int WorkRevision,
    int RngModelVersion,
    int GrowthModelVersion,
    string P1AlgorithmVersion,
    string AreaModelVersion,
    long ActionEligibleDbhThresholdMm);

/// <summary>The fence a command echoes from the view it was read from: session, full token and plan-state revision.</summary>
internal sealed record MultiAreaFence(string SessionId, MultiAreaSourceToken Source, int PlanStateRevision);

/// <summary>One manually chosen order for one exact living tree of one named area, with the player's recorded reason.</summary>
internal sealed record MultiAreaWorkOrder(string AreaId, string TreeId, WorkAction Action, string Reason);

/// <summary>The engine's approved, unexecuted cross-area plan. PlanId and ExecutionKey are minted by the engine.</summary>
internal sealed record MultiAreaApprovedPlan(string PlanId, string ExecutionKey, MultiAreaSourceToken Source,
    IReadOnlyList<MultiAreaWorkOrder> Orders);

/// <summary>One executed Fell, recorded in the experiment's typed event ledger with its area.</summary>
internal sealed record MultiAreaFellRecord(string AreaId, string TreeId, string Cause, string Mechanism, string Reason,
    int EcologicalYear, int WorkRevision, string ExecutionKey, string PlanId);

/// <summary>One area's part of an executed plan: the real stand before and after, and its Fells (possibly none).</summary>
internal sealed record MultiAreaAreaChange(string AreaId, AnnualStand Before, AnnualStand After,
    IReadOnlyList<MultiAreaFellRecord> Fells);

/// <summary>The one event a whole cross-area plan publishes: one receipt with every area's before and after.</summary>
internal sealed record MultiAreaWorkEvent(string ExecutionKey, string PlanId, int EcologicalYear, int WorkRevision,
    IReadOnlyList<MultiAreaAreaChange> Areas);

/// <summary>The recorded result of an execution key. <see cref="Replayed"/> is true for an identical retry.</summary>
internal sealed record MultiAreaExecutionResult(string ExecutionKey, int EcologicalYear, int WorkRevision,
    MultiAreaWorkEvent Event, bool Replayed);

/// <summary>
/// One area's committed ecological year. <see cref="NoLivingStems"/> is true when the area had no living tree, so the
/// engine was not run for it and its (dead) roster was carried forward unchanged while other areas advanced.
/// </summary>
internal sealed record MultiAreaAreaYear(string AreaId, AnnualStand Before, AnnualStand After,
    IReadOnlyList<AnnualDeath> NaturalDeaths, bool NoLivingStems);

/// <summary>One shared ecological year of the whole experiment, with every area's own result.</summary>
internal sealed record MultiAreaYearRecord(int YearBefore, int YearAfter, int AnnualRevisionAfter, int WorkRevision,
    IReadOnlyList<MultiAreaAreaYear> Areas, bool CancelledUnexecutedWork);

/// <summary>One area's annual report; null when the area had no living tree and was carried forward.</summary>
internal sealed record MultiAreaAreaReport(string AreaId, AnnualReport? Report);

/// <summary>The per-area reports of one committed year, in area order.</summary>
internal sealed record MultiAreaAnnualResult(int YearBefore, int YearAfter, IReadOnlyList<MultiAreaAreaReport> Areas);

/// <summary>A polygon vertex in experiment metres (X east, Z north), for drawing only.</summary>
internal sealed record MultiAreaVertex(double XM, double ZM);

/// <summary>
/// One area in a view: its fixed identity and geometry, and its committed current roster and Crop marks. The map extent
/// is a square around the polygon in the same units and orientation as the single-area map (U east, V south).
/// </summary>
internal sealed record MultiAreaAreaView(string AreaId, string Label, string StockingNote, string Site, int SimulationSeed,
    float AreaHa, string AreaM2Exact, IReadOnlyList<MultiAreaVertex> Polygon, PlayerMapExtent Extent,
    IReadOnlyList<AnnualTree> Trees, AnnualStand Stand, IReadOnlyList<string> CropTreeIds);

/// <summary>The fixed provenance of the experiment, shown on the page.</summary>
internal sealed record MultiAreaExperimentInfo(string ExperimentId, string Format, int SchemaVersion, string Domain,
    string SourceClass, string ManifestContentHash, string GeneratorId, int GeneratorVersion, long GeneratorSeed,
    string ReviewStatus, string Disclosure, string EdgeModel, string SiteNote);

/// <summary>
/// One atomic read of the multi-area session: every field comes from one committed root under one lock acquisition.
/// </summary>
internal sealed record MultiAreaSessionView(string SessionId, int PlanStateRevision, MultiAreaSourceToken Source,
    MultiAreaExperimentInfo Experiment, IReadOnlyList<MultiAreaAreaView> Areas, IReadOnlyList<MultiAreaWorkOrder> Pending,
    MultiAreaApprovedPlan? Approved, IReadOnlyList<MultiAreaWorkEvent> Events, IReadOnlyList<MultiAreaYearRecord> AnnualHistory);

/// <summary>
/// The outcome of one command. With <see cref="RefusalCode"/> set, nothing changed and <see cref="View"/> is a fresh
/// current view; otherwise <see cref="View"/> is the state the command produced.
/// </summary>
internal sealed record MultiAreaCommandResult(MultiAreaSessionView View, MultiAreaExecutionResult? Execution,
    MultiAreaAnnualResult? Annual, string? RefusalCode, string? RefusalMessage);

/// <summary>A read-only competition inspection of one living tree of one area. It never changes the session.</summary>
internal sealed record MultiAreaCompetitionResult(MultiAreaSessionView View, string AreaId,
    PlayerCompetitionReport? Competition, string? RefusalCode, string? RefusalMessage);

/// <summary>A read-only exact-ID map hit-test in one area's map metres, topmost first.</summary>
internal sealed record MultiAreaHitResult(MultiAreaSessionView View, string AreaId, IReadOnlyList<PlayerMapItem> Candidates,
    string? RefusalCode, string? RefusalMessage);

/// <summary>
/// Fixed, truthful text for the two-area page. The page shows these words, or values from a view, and invents no label of
/// its own. In a template {n} is a count, {a} a number of areas and {y} an ecological year.
/// </summary>
internal static class MultiAreaLabels
{
    public const string MultiTitle = "Two-area synthetic Sitka experiment (MA-1)";
    public const string MultiDisclosure = MultiAreaIdentity.Disclosure;
    public const string MultiSyntheticStatement = "Synthetic authored experiment — not a surveyed Irish forest";
    public const string MultiNotComparable = "Areas not assessed comparable";
    public const string MultiSiteUnknown = "Site: UNKNOWN — no site, soil, exposure or history is modelled";
    public const string MultiEdgeExcluded = "Edge neighbours excluded — each area's competition uses only its own trees";
    public const string MultiNoTransfer = "No treatment transfer — nothing done in one area is applied to the other";
    public const string MultiSessionOnly = "Session only — nothing is saved";
    public const string MultiReviewStatus = "Experiment review status: pending independent review (author-generated)";
    public const string MultiOnePlan = "One Work Plan covers both areas";
    public const string MultiApprove = "Approve whole two-area Work Plan — 0 trees felled";
    public const string MultiCancelApproved = "Cancel approved plan";
    public const string MultiExecute = "Execute approved two-area plan";
    public const string MultiExecuteConfirmation = "Fells exactly {n} named trees across {a} areas in Year {y}, or refuses all";
    public const string MultiExecuteConfirmationOneTree = "Fells exactly {n} named tree across {a} area in Year {y}, or refuses all";
    public const string MultiAlreadyExecuted = "Already executed — no additional Fell";
    public const string MultiAddToPlan = "Add Fell to Work Plan — no tree is felled";
    public const string MultiRemoveFromPlan = "Remove from Work Plan";
    public const string MultiAdvanceYear = "Advance one ecological year (both areas)";
    public const string MultiConsentKeep = "Keep orders — stay in Year {y}";
    public const string MultiConsentCancel = "Cancel ALL unexecuted work in both areas and advance one year";
    public const string MultiLastLivingRefused =
        "A batch that would leave any populated area with no living tree is refused as a whole";
    public const string MultiEmptyAreaRule =
        "An area emptied by natural mortality stays empty while other areas advance; Year+1 is refused only when every area is empty";
    public const string MultiCompetitionReadOnly =
        "Read-only Hegyi competition diagnostic — inspecting changes nothing; it is not advice to fell or a growth forecast";
    public const string MultiCropSeparate = "Crop designation is a separate, explicit action; competition never marks or fells a tree";
    public const string MultiMarkCrop = "Retain as Crop Tree";
    public const string MultiUnmarkCrop = "Remove Crop designation";
    public const string MultiCropFellRefused = "Remove the Crop designation before proposing this tree for Fell";
    public const string MultiThresholdInformational = "170 mm is metadata only; it does not restrict Fell (D-093)";
    public const string MultiNothingChanged = "Nothing changed";
    public const string MultiOutcomeUnconfirmed = "Outcome unconfirmed — re-read current state";
    public const string MultiNoNaturalDeaths = "no natural deaths";
    public const string MultiAreaNoLivingStems = "no living stems — carried forward unchanged";
}
