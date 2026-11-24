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

/// <summary>
/// The outcome of one Load. <see cref="DraftOrderCount"/> is how many formerly approved, unexecuted orders came back as an
/// unapproved visible Pending draft (0 when the save held no approval). With <see cref="RefusalCode"/> set nothing changed
/// and <see cref="View"/> is the unchanged live session; otherwise the view carries the restored forest under a NEW
/// session id.
/// </summary>
internal sealed record MultiAreaLoadResult(MultiAreaSessionView View, int DraftOrderCount, string? Checksum,
    string? RefusalCode, string? RefusalMessage);

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
    public const string MultiSessionOnly = "Saved games: named slots on this computer only — no cloud or remote backup";
    // Short forms for the header chips; each chip carries its full statement above as a tooltip, and the Details pane
    // lists the full statements.
    public const string MultiChipSynthetic = "Synthetic — not surveyed";
    public const string MultiChipNotComparable = "Areas not comparable";
    public const string MultiChipSiteUnknown = "Site UNKNOWN";
    public const string MultiChipEdgeExcluded = "Edge neighbours excluded";
    public const string MultiChipNoTransfer = "No treatment transfer";
    public const string MultiChipSessionOnly = "Saved games: this computer only";
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

    // Stage 1–4 UI catch-up (spec multi-stage4-ui-catchup-v1): the page shows these for a 1/1/1 session, where the model 0
    // "edge neighbours excluded" statements above would no longer be true. The manifest disclosure itself is unchanged.
    public const string MultiDisclosureStage4 =
        "SYNTHETIC AUTHORED TWO-AREA SITKA EXPERIMENT. NOT A SURVEYED IRISH FOREST. AREAS NOT ASSESSED COMPARABLE. SITE UNKNOWN. " +
        "CONNECTED WOODLAND: GROWTH AND CROWNS USE BOTH AREAS; DENSITY MORTALITY, WORK PLANS AND SUMMARIES STAY PER AREA. " +
        "NO TREATMENT TRANSFER BETWEEN AREAS. NOT UNITY NUMERICAL OR GEOMETRY PARITY.";
    public const string MultiConnectedWoodland =
        "Connected woodland — adult growth and crowns use Hegyi competition over both areas' living trees (8 m); density mortality, Work Plans and summaries stay per area";
    public const string MultiChipConnectedWoodland = "Connected woodland";
    public const string MultiCompetitionAreaOnly =
        "This read-only diagnostic ranks this area's own trees only; Stage 4 growth and crowns use the connected woodland";
    public const string MultiLightGuideLabel = "Modelled Light Guide (Unity k10a10 proxy — not calibrated PAR)";
    public const string MultiAddToPrunePlan = "Add to Prune plan";
    public const string MultiPrunePendingNote = "Queues a Prune only: no crown is cut until the approved plan is executed";
    public const string MultiExecutePruneConfirmation =
        "Prunes exactly {n} named trees in Year {y} (one lift each; the crown is cut at once; the year does not change), or refuses all";
    public const string MultiDeadwoodNote = "Fallen deadwood: one log per natural death, at the stem position; not to scale, no fall direction, no decay or value";
    public const string MultiNotModelledStage4 =
        "Seed, regeneration, understorey, site and money: NOT MODELLED. Saving keeps this forest in named slots on this computer. Light is a modelled index (Unity k10a10 proxy — not calibrated PAR).";

    // Pass 1 Map Integration (spec multi-map-integration-pass1-v1): truthful Phase 2 / mixed-demo labels.
    public const string MultiLightCaveatShort = "Modelled light · not calibrated PAR";
    public const string MultiDeadwoodSchematic = "direction schematic, not simulated";
    public const string MultiClearUndergrowthRule = "Whole 2.5 m cell · spares seedlings";
    public const string MultiNaturalSeedlingsNote = "Cell-level presence only — Density is abstract occupancy, not a literal stem count";
    public const string MultiNotModelledPhase2 =
        "Site/soil map, storms, decay, habitat score and economy/cash/prices: NOT MODELLED. Saving keeps this forest in named slots on this computer. Light is a modelled index (Modelled light · not calibrated PAR).";

    // Save/Load (Phase 1, spec multi-2d-phase1-save-time-history-v1). Server-managed named slots in one folder on this
    // machine: no path entry, no upload, no download, no cloud or remote copy, and no claim of crash-proof durability
    // beyond what the Linux host actually did.
    public const string MultiSavesOpen = "Saved games";
    public const string MultiSavesTitle = "Saved games — two-area forest";
    public const string MultiSavesIntro =
        "Named slots in this app's own folder on this computer. Nothing is uploaded, downloaded or copied to a cloud or remote service.";
    public const string MultiSavesNameLabel = "Slot name";
    public const string MultiSavesNameHint =
        "Lowercase letters, digits and '-', 1–48 characters, starting with a letter or digit. The slot name is also its label.";
    public const string MultiSavesSave = "Save current forest";
    public const string MultiSavesOverwrite = "Replace the existing slot with this forest";
    public const string MultiSavesLoad = "Load";
    public const string MultiSavesRecover = "Restore backup";
    public const string MultiSavesEmpty = "No saved slots in this folder yet.";
    public const string MultiSavesLoadConfirm =
        "Load '{slot}' and replace the forest on screen now? Unsaved changes to the current forest are lost, and any previously approved but unexecuted work comes back as a pending draft you must approve again.";
    public const string MultiSavesDurable = "Saved and durably committed to disk.";
    public const string MultiSavesDurabilityUnconfirmed =
        "The slot was replaced, but the folder flush was not confirmed: a power loss could still roll this slot back to the previous file. Do not treat it as durably saved.";
    public const string MultiSavesPendingDraft =
        "{n} previously approved order(s) came back as an unapproved pending draft; approve them again to execute.";
}
