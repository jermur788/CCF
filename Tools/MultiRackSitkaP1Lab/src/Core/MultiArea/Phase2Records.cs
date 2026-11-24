namespace SitkaP1.Core;

// MULTI Phase 2 immutable state and read-only views. These are NEW records: no existing record changes shape (Stage 5 bands
// keep their shape and order, so the 1/1/1/1/0 fingerprint cannot move), and /multi, PlayerMultiApi JSON and every save
// schema are untouched. Session-only: nothing here is saved.

/// <summary>
/// One natural broadleaf abundance band (or the species' sub-threshold accumulator) in one ground cell, keyed by
/// (SpeciesId, Origin, EstablishYear). Phase 2 has natural bands only: planted juveniles are exact individuals.
/// </summary>
internal sealed record Phase2Band(string SpeciesId, string Origin, float Density, float Height, int EstablishYear, int OriginYear,
    float LastBrowsedFraction, int LastBrowseAssessmentYear)
{
    public const string Natural = "Natural";
}

/// <summary>The broadleaf bands of one ground cell (same lattice and order as the Stage 5 cells), species then year order.</summary>
internal sealed record Phase2CellBands(int Column, int Row, IReadOnlyList<Phase2Band> Bands)
{
    public double Abundance => Bands.Sum(band => (double)band.Density);
}

/// <summary>The broadleaf bands of every ground cell of one area, in the Stage 5 lattice order.</summary>
internal sealed record Phase2AreaBands(string AreaId, IReadOnlyList<Phase2CellBands> Cells);

/// <summary>
/// One exact planted juvenile (Unity PlantedJuvenile): nursery age 3 years and 0.6 m at planting, at an exact X/Z of one
/// area. Dead or promoted juveniles stay in the list with their outcome.
/// </summary>
internal sealed record PlantedJuvenile(string JuvenileId, string AreaId, string SpeciesId, float XM, float ZM, int PlantingYear,
    float AgeYears, float HeightM, bool Alive, string? PromotedTreeId, int DeathYear, string? DeathCause,
    bool LastYearBrowsed, float LastBrowseProbability, float LastVegetationLossProbability, int LastAssessmentYear)
{
    /// <summary>Living and not yet promoted: the only juveniles the annual step advances and a shelter can target.</summary>
    public bool IsActive => Alive && PromotedTreeId is null;
}

/// <summary>
/// One tree shelter, ported from Unity BrowseShelter: binary protection of the juvenile at its position while effective.
/// <see cref="FailedYear"/> is ported and never set (Unity never sets it; shelter failure is out of Phase 2 scope).
/// </summary>
internal sealed record BrowseShelter(string ShelterId, string JuvenileId, float XM, float ZM, int InstalledYear, int EffectiveYears,
    int FailedYear)
{
    /// <summary>Unity IsEffective: installed ≤ y &lt; installed + max(0, years), and not failed by y. Strict &lt; expiry.</summary>
    public bool IsEffective(int year) => year >= InstalledYear && year < InstalledYear + Math.Max(0, EffectiveYears) &&
        (FailedYear < 0 || year < FailedYear);
}

/// <summary>
/// One ~1 m² circular spot clearance (Unity PlantingClearancePatch): its own bramble/bracken covers, zero at creation, stepped
/// each later year toward the target of the cell containing its centre. <see cref="UpdatedYear"/> is the Unity
/// competitionUpdatedYear guard (set to the creation year, so the first annual step after Execute leaves the patch at 0).
/// </summary>
internal sealed record SpotPatch(string PatchId, string AreaId, float XM, float ZM, float RadiusM, int CreatedYear, int UpdatedYear,
    float Bramble, float Bracken)
{
    public bool Contains(float xM, float zM)
    {
        float dx = xM - XM, dz = zM - ZM;
        return dx * dx + dz * dz <= RadiusM * RadiusM;
    }
}

/// <summary>One fenced Phase 2 planting order. Orders resolve SpotClear → Plant → Shelter, then by order ID.</summary>
internal abstract record MultiAreaPlantingOrder(string OrderId, string AreaId, float XM, float ZM)
{
    internal abstract int ResolveRank { get; }
}

/// <summary>Plant one nursery juvenile of one broadleaf at an exact point (sessile-oak or beech).</summary>
internal sealed record MultiAreaPlantOrder(string OrderId, string AreaId, string SpeciesId, float XM, float ZM)
    : MultiAreaPlantingOrder(OrderId, AreaId, XM, ZM)
{
    internal override int ResolveRank => 1;
}

/// <summary>Shelter the living planted juvenile (or the same-batch Plant) within 0.25 m of the point.</summary>
internal sealed record MultiAreaShelterOrder(string OrderId, string AreaId, float XM, float ZM)
    : MultiAreaPlantingOrder(OrderId, AreaId, XM, ZM)
{
    internal override int ResolveRank => 2;
}

/// <summary>Clear bramble and bracken in a 1 m² circle centred on the point (selective: no natural band changes).</summary>
internal sealed record MultiAreaSpotClearOrder(string OrderId, string AreaId, float XM, float ZM)
    : MultiAreaPlantingOrder(OrderId, AreaId, XM, ZM)
{
    internal override int ResolveRank => 0;
}

/// <summary>One executed planting order with its plan, key, year and what it created. No cost, stock or minutes exist.</summary>
internal sealed record Phase2WorkRecord(string OrderId, string Kind, string AreaId, float XM, float ZM, string? SpeciesId,
    string? JuvenileId, string? ShelterId, string? PatchId, int EcologicalYear, int WorkRevision, string ExecutionKey, string PlanId);

/// <summary>One species' natural regeneration account in one area for one year (Oak and Beech; Sitka keeps its Stage 5 account).</summary>
internal sealed record Phase2SpeciesAreaAccount(string AreaId, string SpeciesId, double SeedArrival, double StartingAbundance,
    double VegetationLoss, double LightLoss, double BrowseLoss, double EstablishmentRequested, double EstablishmentAccepted,
    double CapacityRejected, int BandsCreated, int BandMerges, int ThresholdCrossings, int QualifyingCells, int BlockCapDeferredCells,
    double PromotionExported, double EndingAbundance, int CohortBandsExported, int BudgetWithheldBlocks,
    IReadOnlyList<Stage5PromotionRecord> Promotions);

/// <summary>The planted-juvenile account of one species in one year (Unity PlantedCompetitionSpeciesAccount).</summary>
internal sealed record Phase2PlantedAccount(string SpeciesId, int Starting, int VegetationDeaths, int LightDeaths, int BrowseDeaths,
    int Remaining, int Promoted, IReadOnlyList<string> PromotedTreeIds);

/// <summary>One broadleaf's mast state of one year.</summary>
internal sealed record Phase2MastRecord(string SpeciesId, int MastSeed, double Roll, string Label, float Multiplier);

/// <summary>
/// The density-mortality diagnostics of one rack in one year (spec §5): its route and, for a mixed rack, the generic
/// component's occupancy, response, share, quota and budget. Diagnostics only; they never feed back into the rules.
/// </summary>
internal sealed record Phase2DensityRecord(string AreaId, bool Mixed, int Stems, int SitkaSubset, int SitkaHazardDeaths,
    int SitkaBoundaryDeaths, double Occupancy, double GenericResponse, double NonReferenceShare, int EligibleForGeneric,
    double ExpectedGeneric, int GenericQuota, double QuotaRemainder, double? QuotaRoll, int GenericDeaths, int BudgetPassedOver,
    bool SingleStemFallback, double StandBasalAreaM2, double GenericBasalAreaM2, IReadOnlyList<string> GenericVictims);

/// <summary>One Phase 2 year: both broadleaf mast states, the natural and planted accounts and every rack's density record.</summary>
internal sealed record Phase2YearAccount(int YearBefore, int YearAfter, IReadOnlyList<Phase2MastRecord> Mast,
    IReadOnlyList<Phase2SpeciesAreaAccount> Natural, IReadOnlyList<Phase2PlantedAccount> Planted,
    IReadOnlyList<Phase2DensityRecord> Density);

/// <summary>
/// The Phase 2 part of the one commit root (optional, like Stage 4 and Stage 5 state; null whenever Species is 0). Published
/// with the rest of <see cref="MultiAreaRoot"/> by the same single reference assignment. <see cref="TreeSpecies"/> holds the
/// species of every broadleaf adult; a tree that is not in it is Sitka.
/// </summary>
internal sealed record MultiAreaPhase2State(
    Phase2Options Options,
    int OakMastSeed,
    int BeechMastSeed,
    IReadOnlyDictionary<string, string> TreeSpecies,
    IReadOnlyList<Phase2AreaBands> Areas,
    IReadOnlyList<PlantedJuvenile> Juveniles,
    IReadOnlyList<BrowseShelter> Shelters,
    IReadOnlyList<SpotPatch> Patches,
    IReadOnlyList<MultiAreaPlantingOrder> PendingOrders,
    IReadOnlyList<MultiAreaPlantingOrder> ApprovedOrders,
    IReadOnlyList<Phase2WorkRecord> WorkLog,
    int NextOrderNumber,
    int NextJuvenileNumber,
    int NextPatchNumber,
    IReadOnlyList<Phase2YearAccount> Accounts)
{
    /// <summary>The species of a tree: the broadleaf, or null for Sitka.</summary>
    public Phase2Species? BroadleafOf(string treeId) =>
        TreeSpecies.TryGetValue(treeId, out var id) ? Phase2Species.BroadleafOrNull(id) : null;

    public string SpeciesIdOf(string treeId) => TreeSpecies.TryGetValue(treeId, out var id) ? id : Phase2Species.SitkaId;

    /// <summary>True when any cell holds broadleaf abundance or any planted juvenile is still active.</summary>
    public bool HasJuveniles => Juveniles.Any(j => j.IsActive) ||
        Areas.Any(area => area.Cells.Any(cell => cell.Bands.Any(band => band.Density > 0f)));
}

/// <summary>The Phase 2 read-out of one committed root, with its options and evidence labels.</summary>
internal sealed record MultiAreaPhase2View(string SessionId, int EcologicalYear, int PlanStateRevision, MultiAreaSourceToken Source,
    MultiAreaModelSelection Models, Phase2Options Options, int OakMastSeed, int BeechMastSeed, IReadOnlyList<string> EvidenceLabels,
    IReadOnlyDictionary<string, string> TreeSpecies, IReadOnlyList<Phase2AreaBands> Areas, IReadOnlyList<PlantedJuvenile> Juveniles,
    IReadOnlyList<BrowseShelter> Shelters, IReadOnlyList<SpotPatch> Patches, IReadOnlyList<MultiAreaPlantingOrder> PendingOrders,
    IReadOnlyList<MultiAreaPlantingOrder> ApprovedOrders, IReadOnlyList<Phase2WorkRecord> WorkLog, IReadOnlyList<Phase2YearAccount> Accounts);
