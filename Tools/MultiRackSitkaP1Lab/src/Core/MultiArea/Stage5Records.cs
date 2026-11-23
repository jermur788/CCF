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
/// <para>
/// <see cref="Light"/> is the light index this cell's rules used in the latest step (spec §3 steps 5a–9): the light of the
/// woodland that step read, which is the post-adult-step roster of that year (at year 0, the genesis roster). Two things
/// are deliberately not in it: the adults promoted in that same step (they first shade the regeneration layer in the next
/// year's step), and any Fell executed since. So it equals the canopy read-out of the committed root in every year without
/// a promotion, and in a promotion year it equals the light of the committed roster without that year's new adults (both
/// asserted by test S5-08 P-05). Use the canopy read-out, not this value, to show the current canopy.
/// </para>
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

/// <summary>
/// One cohort promoted into one CrownModel 1 adult. <see cref="ExportedDensity"/> is the winning band's abundance (it sets
/// the tree). <see cref="CohortBandsExported"/> and <see cref="CohortDensityExported"/> are the block's other represented
/// bands of the same cohort that left with it (spec §4 Option A1); they may lie in another area of a straddling block.
/// </summary>
internal sealed record Stage5PromotionRecord(string TreeId, string AreaId, int Column, int Row, int BlockX, int BlockZ,
    int EstablishYear, int AgeYears, float HeightM, float DbhCm, float CrownRadiusM, float WoodlandCi, float XM, float ZM,
    bool CentreFallback, float ExportedDensity, int CohortBandsExported, double CohortDensityExported);

/// <summary>
/// One area's read-only annual regeneration account (Unity RegenerationAnnualAccount, plus vegetation loss and the block
/// rules). Abundances are in the cell's own units, so they sum to an area total that is comparable between grids.
/// <list type="bullet">
/// <item><see cref="PromotionExported"/>: all abundance this area's cells lost to promotion (winning bands and cohort-export
/// bands), so StartingAbundance − losses + EstablishmentAccepted − PromotionExported = EndingAbundance.</item>
/// <item><see cref="BlockCapDeferredCells"/>: qualifying cells whose qualifying band neither promoted nor left with the block's
/// promoted cohort this year (it waits: block cap, block credit or a younger cohort).</item>
/// <item><see cref="CohortBandsExported"/>: this area's bands that left with a cohort promoted in their block.</item>
/// <item><see cref="BudgetWithheldBlocks"/>: blocks whose would-be winner lies in this area but whose credit was short.</item>
/// <item><see cref="CellEstablishmentAccepted"/>: this year's accepted abundance per cell, in lattice order (diagnostics).</item>
/// </list>
/// </summary>
internal sealed record Stage5AreaAccount(string AreaId, int Cells, double SeedArrival, double StartingAbundance,
    double VegetationLoss, double LightLoss, double BrowseLoss, double EstablishmentRequested, double EstablishmentAccepted,
    double CapacityRejected, int BandsCreated, int BandMerges, int ThresholdCrossings, int QualifyingCells,
    int BlockCapDeferredCells, double PromotionExported, double EndingAbundance, double MeanRelativeOccupancy,
    double MeanBramble, double MeanBracken, double MeanLight, IReadOnlyList<Stage5PromotionRecord> Promotions,
    int CohortBandsExported, int BudgetWithheldBlocks, IReadOnlyList<float> CellEstablishmentAccepted);

/// <summary>
/// The promotion credit of one aligned 5 m block (spec §4 Option A2), in cell-years. Each year the block gains
/// <see cref="InPolygonCells"/>; a promotion spends one full block's worth of cells (4 at 2.5 m, 1 at 5.0 m); unspent credit
/// is capped at one promotion. So the block's long-run promotion budget is InPolygonCells × a per year, and a full block
/// (every 5.0 m cell is one) is never limited by it. Session state only: never saved.
/// </summary>
internal sealed record Stage5BlockCredit(int BlockX, int BlockZ, int InPolygonCells, int Credit);

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
    IReadOnlyList<Stage5YearAccount> Accounts,
    IReadOnlyList<Stage5BlockCredit> Blocks)
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
    IReadOnlyList<Stage5YearAccount> Accounts, IReadOnlyList<Stage5BlockCredit> Blocks);
