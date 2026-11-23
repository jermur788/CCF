namespace SitkaP1.Core;

/// <summary>
/// The owner-approved area-consistent scaling of the extensive band quantities (spec §4, SIMULATION ABSTRACTION):
/// a = (cell / 5 m)², so a 5.0 m run uses Unity's own per-cell values and a 2.5 m run (a = 0.25) keeps abundance per hectare
/// and relative occupancy equal. Growth, survival, seed rain, light and undergrowth are intensive and are not scaled.
/// </summary>
internal readonly record struct Stage5CellScale(float AreaFactor)
{
    public static Stage5CellScale For(double cellM)
    {
        CanopyLightEvaluator.RequireSupportedGrid(cellM);
        double ratio = cellM / Stage5Constants.UnityCellM;
        return new Stage5CellScale((float)(ratio * ratio));
    }

    /// <summary>Cell capacity: RegenDensityMax · a (1.5 at 5 m, 0.375 at 2.5 m).</summary>
    public float Capacity => Stage5Constants.RegenDensityMax * AreaFactor;

    /// <summary>Representation threshold: 0.01 · a. A band below it is the cell's sub-threshold accumulator.</summary>
    public float Threshold => Stage5Constants.RepresentationThreshold * AreaFactor;
}

/// <summary>A mutable band used only while one annual step is being built; it is frozen into <see cref="Stage5Band"/>.</summary>
internal sealed class Stage5BandWork
{
    public float Density;
    public float Height;
    public int EstablishYear;
    public int OriginYear;
    public float LastBrowsedFraction;
    public int LastBrowseAssessmentYear = -1;

    public static Stage5BandWork From(Stage5Band band) => new()
    {
        Density = band.Density, Height = band.Height, EstablishYear = band.EstablishYear, OriginYear = band.OriginYear,
        LastBrowsedFraction = band.LastBrowsedFraction, LastBrowseAssessmentYear = band.LastBrowseAssessmentYear,
    };

    public Stage5Band Freeze() => new(Density, Height, EstablishYear, OriginYear, LastBrowsedFraction, LastBrowseAssessmentYear);
}

/// <summary>Band counters of one area's annual account (diagnostics only; they never feed back into the rules).</summary>
internal sealed class Stage5BandCounters
{
    public double EstablishmentRequested, EstablishmentAccepted, CapacityRejected;
    public int BandsCreated, BandMerges, ThresholdCrossings;
}

/// <summary>
/// Natural Sitka establishment into abundance bands and the band invariants of one cell, ported verbatim from Unity
/// ForestEcologyController (regeneration model 1: EstablishNewBands, AdmitRecruitment, NormalizeBands, MergeBands) and
/// ForestEcologyCell (AdmitDensity, SharedOccupancy, InsertBand) @ 5e79e986. Stage 5 has one species (Sitka) and one origin
/// (Natural), so a cell's band list is simply ordered by establishment year. The only change is the §4 scale of capacity,
/// request and representation threshold.
/// </summary>
internal static class Stage5Bands
{
    /// <summary>
    /// The requested abundance of this year's establishment in one cell, already scaled by a, or 0 when nothing establishes:
    /// seedFactor = 1 − exp(−S/10); establishment = seedFactor · LR(light) · suitability (1.0); skip when ≤ 0.01;
    /// requested = 0.5 · establishment · a.
    /// </summary>
    public static float RequestedEstablishment(float seedRain, float light, Stage5CellScale scale)
    {
        if (seedRain <= 0f) return 0f;
        float seedFactor = 1f - (float)Math.Exp(-seedRain / Stage5Constants.SeedSaturationS50);
        float lightResponse = Stage5Juvenile.LightResponse(light);
        float establishment = seedFactor * lightResponse * Stage5Constants.EstablishmentSuitability;
        if (establishment <= Stage5Constants.MinimumEstablishment) return 0f;
        float requested = establishment * Stage5Constants.RegenDensityPerEstablishment;
        return requested * scale.AreaFactor;
    }

    /// <summary>Shared occupancy: Σ density / max(0.01, capacity) over bands with positive density.</summary>
    public static float SharedOccupancy(List<Stage5BandWork> bands, Stage5CellScale scale)
    {
        float occupancy = 0f;
        foreach (var band in bands)
            if (band.Density > 0f) occupancy += band.Density / MathF.Max(Stage5Constants.CapacityFloor, scale.Capacity);
        return occupancy;
    }

    /// <summary>
    /// Admits recruitment of this year's natural band into the cell and returns the accepted abundance. Capacity admits
    /// only free shared occupancy and never shrinks existing stock; a fully rejected request creates nothing. A recruit at or
    /// above the threshold is its own band (a same-year band combines); a smaller one joins the sub-threshold accumulator.
    /// </summary>
    public static float AdmitRecruitment(List<Stage5BandWork> bands, int establishYear, float height, float requested,
        Stage5CellScale scale, Stage5BandCounters counters)
    {
        if (requested <= 0f) return 0f;
        var recruit = new Stage5BandWork { Density = 0f, Height = MathF.Max(0f, height), EstablishYear = establishYear, OriginYear = establishYear };
        float accepted = AdmitDensity(bands, recruit, requested, scale);
        counters.EstablishmentRequested += requested;
        counters.EstablishmentAccepted += accepted;
        counters.CapacityRejected += requested - accepted;
        if (accepted <= 0f) return 0f;

        var sameYear = bands.Find(band => band.EstablishYear == establishYear);
        var accumulator = bands.Find(band => band.Density < scale.Threshold);
        if (sameYear is not null)
            CombineBands(bands, sameYear, recruit, scale, counters);
        else if (accepted < scale.Threshold && accumulator is not null)
            CombineBands(bands, accumulator, recruit, scale, counters);
        else
        {
            InsertBand(bands, recruit);
            counters.BandsCreated++;
        }
        Normalize(bands, scale, counters);
        return accepted;
    }

    /// <summary>Unity AdmitDensity: accepted = min(requested, max(0, 1 − occupancy) · max(0.01, capacity)).</summary>
    private static float AdmitDensity(List<Stage5BandWork> bands, Stage5BandWork target, float requested, Stage5CellScale scale)
    {
        if (requested <= 0f) return 0f;
        float free = MathF.Max(0f, 1f - SharedOccupancy(bands, scale));
        float accepted = MathF.Min(requested, free * MathF.Max(Stage5Constants.CapacityFloor, scale.Capacity));
        if (accepted <= 0f) return 0f;
        target.Density += accepted;
        return accepted;
    }

    /// <summary>
    /// The band invariants, applied until stable (Unity NormalizeBands): a zero band is removed; at most one band below the
    /// threshold (several merge, oldest pair first); establishment years are unique (a same-year pair combines); at most four
    /// represented bands (on overflow the closest-age pair merges, ties to the oldest pair). Merges conserve abundance.
    /// </summary>
    public static void Normalize(List<Stage5BandWork> bands, Stage5CellScale scale, Stage5BandCounters counters)
    {
        float threshold = scale.Threshold;
        for (int guard = 0; guard < 64; guard++)
        {
            var empty = bands.Find(band => band.Density <= 0f);
            if (empty is not null)
            {
                bands.Remove(empty);
                continue;
            }
            var small = bands.FindAll(band => band.Density < threshold);
            if (small.Count > 1)
            {
                CombineBands(bands, small[0], small[1], scale, counters);
                continue;
            }
            int collision = -1;
            for (int k = 0; k < bands.Count - 1 && collision < 0; k++)
                if (bands[k].EstablishYear == bands[k + 1].EstablishYear) collision = k;
            if (collision >= 0)
            {
                CombineBands(bands, bands[collision], bands[collision + 1], scale, counters);
                continue;
            }
            var represented = bands.FindAll(band => band.Density >= threshold);
            if (represented.Count > Stage5Constants.MaxRepresentedBands)
            {
                int pair = 0;
                int smallestGap = int.MaxValue;
                for (int k = 0; k < represented.Count - 1; k++)
                {
                    int gap = represented[k + 1].EstablishYear - represented[k].EstablishYear;
                    if (gap < smallestGap)
                    {
                        smallestGap = gap;
                        pair = k;
                    }
                }
                CombineBands(bands, represented[pair], represented[pair + 1], scale, counters);
                counters.BandMerges++;
                continue;
            }
            return;
        }
        throw new InvalidOperationException("Stage 5 regeneration bands did not settle.");
    }

    // A sub-threshold record that ends up at or above the threshold is a threshold crossing (reclassification, not a tree).
    private static void CombineBands(List<Stage5BandWork> bands, Stage5BandWork a, Stage5BandWork b, Stage5CellScale scale,
        Stage5BandCounters counters)
    {
        bool hadSubThreshold = a.Density < scale.Threshold || b.Density < scale.Threshold;
        var merged = MergeBands(bands, a, b);
        if (hadSubThreshold && merged.Density >= scale.Threshold) counters.ThresholdCrossings++;
    }

    /// <summary>
    /// Unity MergeBands: abundance is conserved; height, establishment year and origin year are abundance-weighted (years
    /// rounded half up), so a new recruit never simply inherits an older band's state.
    /// </summary>
    internal static Stage5BandWork MergeBands(List<Stage5BandWork> bands, Stage5BandWork a, Stage5BandWork b)
    {
        float total = a.Density + b.Density;
        double weightA = a.Density, weightB = b.Density, sum = weightA + weightB;
        var merged = new Stage5BandWork
        {
            Density = MathF.Max(0f, total),
            Height = MathF.Max(0f, (float)((a.Height * weightA + b.Height * weightB) / sum)),
            EstablishYear = (int)Math.Floor((a.EstablishYear * weightA + b.EstablishYear * weightB) / sum + 0.5),
            OriginYear = (int)Math.Floor((a.OriginYear * weightA + b.OriginYear * weightB) / sum + 0.5),
            LastBrowsedFraction = (float)((a.LastBrowsedFraction * weightA + b.LastBrowsedFraction * weightB) / sum),
            LastBrowseAssessmentYear = Math.Max(a.LastBrowseAssessmentYear, b.LastBrowseAssessmentYear),
        };
        bands.Remove(a);
        bands.Remove(b);
        InsertBand(bands, merged);
        return merged;
    }

    /// <summary>Unity InsertBand: before the first band with a later establishment year (equal years keep insertion order).</summary>
    internal static void InsertBand(List<Stage5BandWork> bands, Stage5BandWork band)
    {
        int index = bands.FindIndex(other => other.EstablishYear > band.EstablishYear);
        if (index < 0) bands.Add(band);
        else bands.Insert(index, band);
    }
}
