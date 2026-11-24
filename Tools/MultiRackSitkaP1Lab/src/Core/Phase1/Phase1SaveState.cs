namespace SitkaP1.Core.Phase1;

// The complete mutable state of one 2D multi-area session, as a plain serialization DTO. Every type mirrors one engine
// record field for field (same names, same order, same numeric types), so a later capture/restore (A1) is a pure field
// copy. The DTO is deliberately public and engine-free: it carries values only, never behaviour.
//
// Deliberately NOT here (spec §3): the presentational /multi view JSON, canopy/ground read-out caches, the transient
// session ID, any live source fence, and the approved plan's PlanId / execution key / approval token (authority-bearing;
// only its orders are kept). Source tokens appear ONLY as inert recorded provenance inside an executed-key receipt; they
// are never accepted as a live fence, and a load always creates a fresh session whose ID no old fence carries.
//
// Mirrors (engine, read-only at cf6fe7a5): MultiAreaRoot, MultiAreaAreaState, AnnualState/AnnualTree/AnnualDeath/
// AnnualStand, MultiAreaWorkOrder/ApprovedPlan/FellRecord/AreaChange/WorkEvent/ExecutedWork/YearRecord/AreaYear,
// MultiAreaSourceToken, MultiAreaStage4State (+TreePruningState, PruneOrder/Record, FallenDeadwoodRecord),
// MultiAreaStage5State (+Stage5Options, cells, bands, clears, accounts, promotions, block credit), MultiAreaPhase2State
// (+Phase2Options, bands, PlantedJuvenile, BrowseShelter, SpotPatch, planting orders, work log, accounts).

/// <summary>The whole saved state: deterministic options plus the complete mutable root.</summary>
public sealed record Phase1SaveState(
    Phase1Options Options,
    int EcologicalYear,
    int AnnualRevision,
    int WorkRevision,
    int PlanStateRevision,
    IReadOnlyList<Phase1AreaState> Areas,
    IReadOnlyList<Phase1WorkEvent> Events,
    IReadOnlyList<Phase1ExecutedWork> Executed,
    IReadOnlyList<Phase1WorkOrder> Pending,
    Phase1ApprovedPlan? Approved,
    IReadOnlyList<Phase1YearRecord> AnnualHistory,
    Phase1Stage4State? Stage4,
    Phase1Stage5State? Stage5,
    Phase1Phase2State? Phase2);

// ------------------------------------------------------------------------------------------------ options (inputs)

/// <summary>The session options that affect continuation. Each part is present exactly when its model is selected.</summary>
public sealed record Phase1Options(Phase1Stage4Options? Stage4, Phase1Stage5Options? Stage5, Phase1Phase2Options? Phase2);

/// <summary>Stage4Options: the site-productivity field (only the uniform field has a data form) and growth age phases.</summary>
public sealed record Phase1Stage4Options(string SiteProductivity, IReadOnlyList<Phase1GrowthAgePhase>? GrowthAgePhases);

public sealed record Phase1GrowthAgePhase(float StartAgeYears, float DbhIncrementFactor, float HeightIncrementFactor);

/// <summary>Stage5Options exactly: grid size, background browse pressure and the optional explicit mast seed.</summary>
public sealed record Phase1Stage5Options(double GridCellM, float BackgroundBrowsePressure, int? MastSeed);

/// <summary>Phase2Options exactly. Genesis species are kept as an ordinal-sorted list, never a dictionary walk.</summary>
public sealed record Phase1Phase2Options(int? OakMastSeed, int? BeechMastSeed, IReadOnlyList<Phase1TreeSpecies> GenesisSpecies);

/// <summary>One TreeId → species ID entry (Phase 2 genesis species and adult species identity).</summary>
public sealed record Phase1TreeSpecies(string TreeId, string SpeciesId);

// ------------------------------------------------------------------------------------------------ rosters and work

public sealed record Phase1AreaState(string AreaId, int EcologicalYear, IReadOnlyList<Phase1Tree> Trees,
    IReadOnlyList<string> CropTreeIds);

/// <summary>AnnualTree exactly, dead trees included (a death keeps the stem's last values).</summary>
public sealed record Phase1Tree(string TreeId, float XM, float ZM, int AgeYears, float DbhCm, float HeightM, float CrownRadiusM,
    Phase1Death? Death);

public sealed record Phase1Death(string TreeId, int EcologicalYear, string Cause, string Mechanism);

public sealed record Phase1Stand(int LivingCount, double StemsPerHa, double BasalAreaM2PerHa, double QuadraticMeanDbhCm,
    double RelativeDensity);

/// <summary>A recorded MultiAreaSourceToken (provenance only; never a live fence).</summary>
public sealed record Phase1SourceToken(string Domain, string ExperimentId, string ManifestContentHash, string AreaIds,
    int EcologicalYear, int AnnualRevision, int WorkRevision, int RngModelVersion, int GrowthModelVersion,
    string P1AlgorithmVersion, string AreaModelVersion, long ActionEligibleDbhThresholdMm);

/// <summary>A Fell order. <see cref="Action"/> is the public engine enum WorkAction (only Fell exists).</summary>
public sealed record Phase1WorkOrder(string AreaId, string TreeId, WorkAction Action, string Reason);

/// <summary>
/// The Fell orders of the plan that was approved when the save was made — orders only. The engine-minted PlanId, execution
/// key and approval source token are authority-bearing and are deliberately NOT saved (owner load policy A, reviewer
/// ruling B1): A1 rehydrates these orders, with the approved Prune/Clear/planting orders, as an unapproved pending draft
/// that the player must approve again under a fresh session.
/// </summary>
public sealed record Phase1ApprovedPlan(IReadOnlyList<Phase1WorkOrder> Orders);

public sealed record Phase1FellRecord(string AreaId, string TreeId, string Cause, string Mechanism, string Reason,
    int EcologicalYear, int WorkRevision, string ExecutionKey, string PlanId);

public sealed record Phase1AreaChange(string AreaId, Phase1Stand Before, Phase1Stand After, IReadOnlyList<Phase1FellRecord> Fells);

public sealed record Phase1WorkEvent(string ExecutionKey, string PlanId, int EcologicalYear, int WorkRevision,
    IReadOnlyList<Phase1AreaChange> Areas);

/// <summary>
/// One executed-key receipt (MultiAreaExecutedWork without its Event): the event it published is the ledger entry with
/// the same execution key, so the receipt and the ledger cannot disagree. Kept in ordinal key order.
/// </summary>
public sealed record Phase1ExecutedWork(string ExecutionKey, Phase1SourceToken RecordedSource, IReadOnlyList<Phase1WorkOrder> Orders);

public sealed record Phase1AreaYear(string AreaId, Phase1Stand Before, Phase1Stand After, IReadOnlyList<Phase1Death> NaturalDeaths,
    bool NoLivingStems);

public sealed record Phase1YearRecord(int YearBefore, int YearAfter, int AnnualRevisionAfter, int WorkRevision,
    IReadOnlyList<Phase1AreaYear> Areas, bool CancelledUnexecutedWork);

// ------------------------------------------------------------------------------------------------ Stage 4

public sealed record Phase1Stage4State(IReadOnlyList<Phase1TreePruning> Pruning, IReadOnlyList<Phase1PruneOrder> PendingPrunes,
    IReadOnlyList<Phase1PruneOrder> ApprovedPrunes, IReadOnlyList<Phase1PruneRecord> PruneLog,
    IReadOnlyList<Phase1Deadwood> Deadwood, int NextDeadwoodNumber);

/// <summary>One TreeId → TreePruningState entry, kept in ordinal TreeId order.</summary>
public sealed record Phase1TreePruning(string TreeId, int Lifts, float CrownBaseHeightM, int LastPruningYear);

public sealed record Phase1PruneOrder(string AreaId, string TreeId, float TargetCrownBaseHeightM);

public sealed record Phase1PruneRecord(string AreaId, string TreeId, float TargetCrownBaseHeightM, int EcologicalYear,
    int WorkRevision, string ExecutionKey, string PlanId, float CrownRadiusBeforeM, float CrownRadiusAfterM, int LiftsAfter);

public sealed record Phase1Deadwood(string DeadwoodId, string TreeId, string AreaId, string SpeciesId, float XM, float ZM,
    int DeathYear, string Cause, float DbhCm, float HeightM, float EstimatedVolumeM3);

// ------------------------------------------------------------------------------------------------ Stage 5

/// <summary>MultiAreaStage5State without its Options (they live in <see cref="Phase1Options"/>).</summary>
public sealed record Phase1Stage5State(int MastSeed, IReadOnlyList<Phase1Stage5AreaCells> Areas,
    IReadOnlyList<Phase1ClearOrder> PendingClears, IReadOnlyList<Phase1ClearOrder> ApprovedClears,
    IReadOnlyList<Phase1ClearRecord> ClearLog, IReadOnlyList<Phase1Stage5YearAccount> Accounts,
    IReadOnlyList<Phase1BlockCredit> Blocks);

public sealed record Phase1Stage5AreaCells(string AreaId, IReadOnlyList<Phase1Stage5Cell> Cells);

public sealed record Phase1Stage5Cell(int Column, int Row, float CentreXM, float CentreZM, double ClippedAreaM2, float Light,
    float Bramble, float Bracken, IReadOnlyList<Phase1Stage5Band> Bands);

public sealed record Phase1Stage5Band(float Density, float Height, int EstablishYear, int OriginYear, float LastBrowsedFraction,
    int LastBrowseAssessmentYear);

public sealed record Phase1ClearOrder(string AreaId, int Column, int Row);

public sealed record Phase1ClearRecord(string AreaId, int Column, int Row, int EcologicalYear, int WorkRevision,
    string ExecutionKey, string PlanId, float BrambleBefore, float BrackenBefore, int BandsKept, double AbundanceKept);

public sealed record Phase1PromotionRecord(string TreeId, string AreaId, int Column, int Row, int BlockX, int BlockZ,
    int EstablishYear, int AgeYears, float HeightM, float DbhCm, float CrownRadiusM, float WoodlandCi, float XM, float ZM,
    bool CentreFallback, float ExportedDensity, int CohortBandsExported, double CohortDensityExported);

public sealed record Phase1Stage5AreaAccount(string AreaId, int Cells, double SeedArrival, double StartingAbundance,
    double VegetationLoss, double LightLoss, double BrowseLoss, double EstablishmentRequested, double EstablishmentAccepted,
    double CapacityRejected, int BandsCreated, int BandMerges, int ThresholdCrossings, int QualifyingCells,
    int BlockCapDeferredCells, double PromotionExported, double EndingAbundance, double MeanRelativeOccupancy,
    double MeanBramble, double MeanBracken, double MeanLight, IReadOnlyList<Phase1PromotionRecord> Promotions,
    int CohortBandsExported, int BudgetWithheldBlocks, IReadOnlyList<float> CellEstablishmentAccepted);

public sealed record Phase1BlockCredit(int BlockX, int BlockZ, int InPolygonCells, int Credit);

public sealed record Phase1Stage5YearAccount(int YearBefore, int YearAfter, double MastRoll, string MastLabel, float MastMultiplier,
    IReadOnlyList<Phase1Stage5AreaAccount> Areas);

// ------------------------------------------------------------------------------------------------ Phase 2

/// <summary>MultiAreaPhase2State without its Options (they live in <see cref="Phase1Options"/>).</summary>
public sealed record Phase1Phase2State(int OakMastSeed, int BeechMastSeed, IReadOnlyList<Phase1TreeSpecies> TreeSpecies,
    IReadOnlyList<Phase1Phase2AreaBands> Areas, IReadOnlyList<Phase1PlantedJuvenile> Juveniles,
    IReadOnlyList<Phase1Shelter> Shelters, IReadOnlyList<Phase1SpotPatch> Patches,
    IReadOnlyList<Phase1PlantingOrder> PendingOrders, IReadOnlyList<Phase1PlantingOrder> ApprovedOrders,
    IReadOnlyList<Phase1Phase2WorkRecord> WorkLog, int NextOrderNumber, int NextJuvenileNumber, int NextPatchNumber,
    IReadOnlyList<Phase1Phase2YearAccount> Accounts);

public sealed record Phase1Phase2Band(string SpeciesId, string Origin, float Density, float Height, int EstablishYear,
    int OriginYear, float LastBrowsedFraction, int LastBrowseAssessmentYear);

public sealed record Phase1Phase2CellBands(int Column, int Row, IReadOnlyList<Phase1Phase2Band> Bands);

public sealed record Phase1Phase2AreaBands(string AreaId, IReadOnlyList<Phase1Phase2CellBands> Cells);

public sealed record Phase1PlantedJuvenile(string JuvenileId, string AreaId, string SpeciesId, float XM, float ZM, int PlantingYear,
    float AgeYears, float HeightM, bool Alive, string? PromotedTreeId, int DeathYear, string? DeathCause,
    bool LastYearBrowsed, float LastBrowseProbability, float LastVegetationLossProbability, int LastAssessmentYear);

public sealed record Phase1Shelter(string ShelterId, string JuvenileId, float XM, float ZM, int InstalledYear, int EffectiveYears,
    int FailedYear);

public sealed record Phase1SpotPatch(string PatchId, string AreaId, float XM, float ZM, float RadiusM, int CreatedYear,
    int UpdatedYear, float Bramble, float Bracken);

/// <summary>A polymorphic planting order. The codec writes an explicit type tag for each concrete kind.</summary>
public abstract record Phase1PlantingOrder(string OrderId, string AreaId, float XM, float ZM);

public sealed record Phase1PlantOrder(string OrderId, string AreaId, string SpeciesId, float XM, float ZM)
    : Phase1PlantingOrder(OrderId, AreaId, XM, ZM);

public sealed record Phase1ShelterOrder(string OrderId, string AreaId, float XM, float ZM)
    : Phase1PlantingOrder(OrderId, AreaId, XM, ZM);

public sealed record Phase1SpotClearOrder(string OrderId, string AreaId, float XM, float ZM)
    : Phase1PlantingOrder(OrderId, AreaId, XM, ZM);

public sealed record Phase1Phase2WorkRecord(string OrderId, string Kind, string AreaId, float XM, float ZM, string? SpeciesId,
    string? JuvenileId, string? ShelterId, string? PatchId, int EcologicalYear, int WorkRevision, string ExecutionKey, string PlanId);

public sealed record Phase1SpeciesAreaAccount(string AreaId, string SpeciesId, double SeedArrival, double StartingAbundance,
    double VegetationLoss, double LightLoss, double BrowseLoss, double EstablishmentRequested, double EstablishmentAccepted,
    double CapacityRejected, int BandsCreated, int BandMerges, int ThresholdCrossings, int QualifyingCells, int BlockCapDeferredCells,
    double PromotionExported, double EndingAbundance, int CohortBandsExported, int BudgetWithheldBlocks,
    IReadOnlyList<Phase1PromotionRecord> Promotions);

public sealed record Phase1PlantedAccount(string SpeciesId, int Starting, int VegetationDeaths, int LightDeaths, int BrowseDeaths,
    int Remaining, int Promoted, IReadOnlyList<string> PromotedTreeIds);

public sealed record Phase1MastRecord(string SpeciesId, int MastSeed, double Roll, string Label, float Multiplier);

public sealed record Phase1DensityRecord(string AreaId, bool Mixed, int Stems, int SitkaSubset, int SitkaHazardDeaths,
    int SitkaBoundaryDeaths, double Occupancy, double GenericResponse, double NonReferenceShare, int EligibleForGeneric,
    double ExpectedGeneric, int GenericQuota, double QuotaRemainder, double? QuotaRoll, int GenericDeaths, int BudgetPassedOver,
    bool SingleStemFallback, double StandBasalAreaM2, double GenericBasalAreaM2, IReadOnlyList<string> GenericVictims);

public sealed record Phase1Phase2YearAccount(int YearBefore, int YearAfter, IReadOnlyList<Phase1MastRecord> Mast,
    IReadOnlyList<Phase1SpeciesAreaAccount> Natural, IReadOnlyList<Phase1PlantedAccount> Planted,
    IReadOnlyList<Phase1DensityRecord> Density);
