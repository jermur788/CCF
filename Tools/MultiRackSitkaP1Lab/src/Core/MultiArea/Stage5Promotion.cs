namespace SitkaP1.Core;

/// <summary>One cell whose oldest qualifying band could promote this year.</summary>
internal sealed record Stage5PromotionCandidate(int AreaOrder, string AreaId, int CellIndex, int Column, int Row, int BlockX,
    int BlockZ, int EstablishYear, float Height);

/// <summary>
/// Promotion of juvenile bands into CrownModel 1 adults (spec §3.3, §4 and owner Q2/Q5, amended by owner Option A).
/// <list type="bullet">
/// <item>The oldest represented band with height ≥ 3.5 m qualifies (Unity OldestQualifyingBand). No light gate (Q2).</item>
/// <item>At most one promotion per aligned 5 m block per year, blocks = floor(x/5), floor(z/5) in the world frame (Q5,
/// SIMULATION ABSTRACTION). Winner: oldest EstablishYear, then tallest, then lowest (Column, Row).</item>
/// <item>Option A1: the block's other represented bands of the winner's cohort (same EstablishYear) leave with it into the
/// same tree; bands of other cohorts stay. Option A2: the block must hold a full block of credit (see Stage5Annual).</item>
/// <item>The band becomes ONE adult: Age = max(1, year − establishYear), Height = band height, DBH = clamp(1.5·H, 2, 20) cm,
/// r_0 = CrownModel 1 equilibrium radius max(0.1, r_pot(DBH) / sqrt(1 + CI_woodland/5)), not the open-grown r_pot.</item>
/// <item>Its position is jittered inside the source cell from the cell's STAGE5-PROMO stream and is always strictly inside
/// the area polygon (fallback: the cell centre, which is inside by cell membership).</item>
/// </list>
/// </summary>
internal static class Stage5Promotion
{
    /// <summary>The oldest represented band that can promote, or null. A sub-threshold accumulator never promotes.</summary>
    public static Stage5BandWork? OldestQualifyingBand(List<Stage5BandWork> bands, float light, Stage5CellScale scale)
    {
        Stage5BandWork? oldest = null;
        foreach (var band in bands)
        {
            if (band.Density < scale.Threshold || !CanPromote(band.Height, light)) continue;
            if (oldest is null || band.EstablishYear < oldest.EstablishYear) oldest = band;
        }
        return oldest;
    }

    /// <summary>
    /// The represented band of one cohort (one establishment year) in a cell, or null. There is at most one: the band
    /// invariants (Unity NormalizeBands) keep establishment years unique within a cell. The sub-threshold accumulator is not a
    /// represented band and is never returned.
    /// </summary>
    public static Stage5BandWork? CohortBand(List<Stage5BandWork> bands, int establishYear, Stage5CellScale scale)
    {
        Stage5BandWork? found = null;
        foreach (var band in bands)
        {
            if (band.Density < scale.Threshold || band.EstablishYear != establishYear) continue;
            if (found is not null)
                throw new InvalidOperationException("Stage 5 bands of one cell must have unique establishment years.");
            found = band;
        }
        return found;
    }

    /// <summary>
    /// C, the number of grid cells in a full aligned 5 m block: 4 at 2.5 m and 1 at 5.0 m. It is 1/a, so a block with n
    /// in-polygon cells has the budget n × a = n / C promotions per year. Fixed by the grid geometry, not a tuned constant.
    /// </summary>
    public static int CellsPerBlock(double cellM)
    {
        CanopyLightEvaluator.RequireSupportedGrid(cellM);
        double perSide = Stage5Constants.PromotionBlockM / cellM;
        return (int)Math.Round(perSide * perSide);
    }

    /// <summary>Unity CanPromote: height ≥ 3.5 m and light ≥ promotionMinimumLight (0 for Sitka, Q2).</summary>
    public static bool CanPromote(float heightM, float light) =>
        heightM >= Stage5Constants.PromotionHeightM && light >= Stage5Constants.PromotionMinimumLight;

    /// <summary>The aligned 5 m block of a lattice cell, from its centre (exact for the 2.5 m and 5.0 m lattices).</summary>
    public static (int X, int Z) BlockOf(int column, int row, double cellM) =>
        ((int)Math.Floor((column + 0.5) * cellM / Stage5Constants.PromotionBlockM),
         (int)Math.Floor((row + 0.5) * cellM / Stage5Constants.PromotionBlockM));

    /// <summary>
    /// The one winner of every block, in a fixed order (block, then winner) that does not depend on area or cell order.
    /// Winner key: oldest EstablishYear, then tallest, then lowest Column, then lowest Row (owner Q5).
    /// </summary>
    public static IReadOnlyList<Stage5PromotionCandidate> Winners(IEnumerable<Stage5PromotionCandidate> candidates) =>
        candidates
            .GroupBy(candidate => (candidate.BlockX, candidate.BlockZ))
            .OrderBy(group => group.Key.BlockX).ThenBy(group => group.Key.BlockZ)
            .Select(group => group
                .OrderBy(c => c.EstablishYear)
                .ThenByDescending(c => c.Height)
                .ThenBy(c => c.Column)
                .ThenBy(c => c.Row)
                .First())
            .ToArray();

    /// <summary>
    /// A deterministic position inside the source cell: centre + (NextDouble() − 0.5) · cell on each axis (Unity
    /// PromoteSpeciesCohorts), drawn from the cell's own stream and retried up to a fixed number of times until it is strictly
    /// inside the polygon; otherwise the cell centre.
    /// </summary>
    public static (float XM, float ZM, bool CentreFallback) Place(IReadOnlyList<MultiAreaVertex> polygon, float centreXM,
        float centreZM, double cellM, string areaId, int column, int row, int year, int seed)
    {
        var stream = Stage5Random.Stream(Stage5Random.PromotionKey(areaId, column, row, year), seed, year);
        float cell = (float)cellM;
        for (int attempt = 0; attempt < Stage5Constants.PromotionPlacementAttempts; attempt++)
        {
            float offsetX = (float)(stream.NextDouble() - 0.5) * cell;
            float offsetZ = (float)(stream.NextDouble() - 0.5) * cell;
            float x = centreXM + offsetX;
            float z = centreZM + offsetZ;
            if (StrictlyInside(polygon, x, z)) return (x, z, false);
        }
        return (centreXM, centreZM, true);
    }

    /// <summary>
    /// The promoted adult. CI_woodland is the connected-woodland 8 m Hegyi index at the new stem over the living woodland
    /// after this year's adult step (same-year promotions are not neighbours of each other, so the order cannot matter).
    /// </summary>
    public static (AnnualTree Tree, float WoodlandCi) NewAdult(string treeId, float xM, float zM, int yearAfter, int establishYear,
        float heightM, AnnualTree[] woodland)
    {
        int age = Math.Max(1, yearAfter - establishYear);
        float dbh = Math.Clamp(heightM * Stage5Constants.PromotionDbhCmPerMetreHeight, Stage5Constants.PromotionDbhMinCm,
            Stage5Constants.PromotionDbhMaxCm);
        var probe = new AnnualTree(treeId, xM, zM, age, dbh, heightM, 0f);
        float ci = CrownModel.WoodlandCompetition(probe, woodland).Ci;
        return (probe with { CrownRadiusM = CrownModel.TargetRadiusM(SpeciesProfile.Sitka, dbh, ci) }, ci);
    }

    /// <summary>"R{year}-{areaId}-{col}-{row}" (spec §3.3).</summary>
    public static string TreeIdFor(int year, string areaId, int column, int row) =>
        string.Create(System.Globalization.CultureInfo.InvariantCulture, $"R{year}-{areaId}-{column}-{row}");

    /// <summary>Strictly inside a strictly convex counter-clockwise polygon in metres; a point on an edge is outside.</summary>
    public static bool StrictlyInside(IReadOnlyList<MultiAreaVertex> polygon, double x, double z)
    {
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            if ((b.XM - a.XM) * (z - a.ZM) - (b.ZM - a.ZM) * (x - a.XM) <= 0) return false;
        }
        return true;
    }
}
