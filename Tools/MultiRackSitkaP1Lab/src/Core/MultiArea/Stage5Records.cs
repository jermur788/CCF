namespace SitkaP1.Core;

// Multi Stage 5 immutable state and read-only views. These are NEW records: no existing view record changes shape, so
// /multi, PlayerMultiApi JSON and every Stage 1–4 path are untouched. Session-only: nothing here is saved.

/// <summary>
/// One abundance band of natural Sitka regeneration in one ground cell, or the cell's sub-threshold accumulator (a band
/// whose density is below the scaled representation threshold). Density is an abstract abundance, not a stem count
/// (Unity ForestEcologyController). The two browse fields are diagnostics of the latest annual step only.
/// </summary>
internal sealed record Stage5Band(float Density, float Height, int EstablishYear, int OriginYear, float LastBrowsedFraction,
    int LastBrowseAssessmentYear);

/// <summary>
/// One 2.5 m (or 5.0 m) ground cell whose centre lies strictly inside its area polygon: its lattice position (shared with
/// the canopy/light evaluator), the light index used in the latest step, bramble and bracken cover (0–1) and its bands in
/// establishment-year order.
/// </summary>
internal sealed record Stage5Cell(int Column, int Row, float CentreXM, float CentreZM, double ClippedAreaM2, float Light,
    float Bramble, float Bracken, IReadOnlyList<Stage5Band> Bands)
{
    /// <summary>Total juvenile abundance of the cell (all bands and the accumulator).</summary>
    public double Abundance => Bands.Sum(band => (double)band.Density);
}

/// <summary>Every Stage 5 ground cell of one area, in the evaluator's row-major lattice order.</summary>
internal sealed record Stage5AreaCells(string AreaId, IReadOnlyList<Stage5Cell> Cells);

/// <summary>One fenced ClearUndergrowth order: one existing in-polygon ground cell of one area.</summary>
internal sealed record MultiAreaClearUndergrowthOrder(string AreaId, int Column, int Row);

/// <summary>
/// One executed ClearUndergrowth: the cell, its plan, key and year, the cover it removed and the regeneration it kept
/// (selective clearing never touches a Sitka band).
/// </summary>
internal sealed record MultiAreaClearUndergrowthRecord(string AreaId, int Column, int Row, int EcologicalYear, int WorkRevision,
    string ExecutionKey, string PlanId, float BrambleBefore, float BrackenBefore, int BandsKept, double AbundanceKept);

/// <summary>One band promoted into one CrownModel 1 adult (the band's abundance is exported to exactly one tree).</summary>
internal sealed record Stage5PromotionRecord(string TreeId, string AreaId, int Column, int Row, int BlockX, int BlockZ,
    int EstablishYear, int AgeYears, float HeightM, float DbhCm, float CrownRadiusM, float WoodlandCi, float XM, float ZM,
    bool CentreFallback, float ExportedDensity);

/// <summary>
/// One area's read-only annual regeneration account (Unity RegenerationAnnualAccount, plus vegetation loss and the block
/// cap). Abundances are in the cell's own units, so they sum to an area total that is comparable between grids.
/// </summary>
internal sealed record Stage5AreaAccount(string AreaId, int Cells, double SeedArrival, double StartingAbundance,
    double VegetationLoss, double LightLoss, double BrowseLoss, double EstablishmentRequested, double EstablishmentAccepted,
    double CapacityRejected, int BandsCreated, int BandMerges, int ThresholdCrossings, int QualifyingCells,
    int BlockCapDeferredCells, double PromotionExported, double EndingAbundance, double MeanRelativeOccupancy,
    double MeanBramble, double MeanBracken, double MeanLight, IReadOnlyList<Stage5PromotionRecord> Promotions);

/// <summary>One shared Stage 5 year: the one woodland mast roll and every area's account, in area order.</summary>
internal sealed record Stage5YearAccount(int YearBefore, int YearAfter, double MastRoll, string MastLabel, float MastMultiplier,
    IReadOnlyList<Stage5AreaAccount> Areas);

/// <summary>
/// The Stage 5 part of the one commit root (optional, like <see cref="MultiAreaStage4State"/>; null on every Stage 1–4 root).
/// It is published with the rest of <see cref="MultiAreaRoot"/> by the same single reference assignment.
/// </summary>
internal sealed record MultiAreaStage5State(
    Stage5Options Options,
    int MastSeed,
    IReadOnlyList<Stage5AreaCells> Areas,
    IReadOnlyList<MultiAreaClearUndergrowthOrder> PendingClears,
    IReadOnlyList<MultiAreaClearUndergrowthOrder> ApprovedClears,
    IReadOnlyList<MultiAreaClearUndergrowthRecord> ClearLog,
    IReadOnlyList<Stage5YearAccount> Accounts)
{
    /// <summary>True when any cell of any area still holds juvenile abundance.</summary>
    public bool HasAbundance => Areas.Any(area => area.Cells.Any(cell => cell.Bands.Any(band => band.Density > 0f)));

    /// <summary>The cell at (column, row) of one area, or null.</summary>
    public Stage5Cell? CellOf(string areaId, int column, int row)
    {
        foreach (var area in Areas)
        {
            if (!string.Equals(area.AreaId, areaId, StringComparison.Ordinal)) continue;
            foreach (var cell in area.Cells)
                if (cell.Column == column && cell.Row == row) return cell;
        }
        return null;
    }
}

/// <summary>The Stage 5 read-out of one committed root, with its model selection, options and evidence labels.</summary>
internal sealed record MultiAreaStage5View(string SessionId, int EcologicalYear, int PlanStateRevision, MultiAreaSourceToken Source,
    MultiAreaModelSelection Models, Stage5Options Options, int MastSeed, IReadOnlyList<string> EvidenceLabels,
    IReadOnlyList<Stage5AreaCells> Areas, IReadOnlyList<MultiAreaClearUndergrowthOrder> PendingClears,
    IReadOnlyList<MultiAreaClearUndergrowthOrder> ApprovedClears, IReadOnlyList<MultiAreaClearUndergrowthRecord> ClearLog,
    IReadOnlyList<Stage5YearAccount> Accounts);
