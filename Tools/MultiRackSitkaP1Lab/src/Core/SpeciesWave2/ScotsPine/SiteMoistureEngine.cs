namespace SitkaP1.Core.SpeciesWave2.ScotsPine;

public readonly record struct MoistureArea(SiteMoistureClass Moisture, decimal AreaM2);

public readonly record struct MoistureSelection(SiteMoistureClass Moisture)
{
    public bool HabitatProtectionCaution => SpeciesMoistureTable.HabitatProtectionCaution(Moisture);
}

public readonly record struct SiteMoistureResult(MoistureSelection Site, MoistureCell Cell);

public readonly record struct StandBounds(double MinX, double MinY, double MaxX, double MaxY);

public static class SiteMoistureEngine
{
    public static SiteMoistureResult ForSpecies(string speciesId, SiteMoistureClass moisture) =>
        new(new MoistureSelection(moisture), SpeciesMoistureTable.Cell(speciesId, moisture));

    /// <summary>Selects the largest total area. Ties use the table's class declaration order.</summary>
    public static MoistureSelection SelectDominant(IEnumerable<MoistureArea> areas)
    {
        ArgumentNullException.ThrowIfNull(areas);
        var totals = new Dictionary<SiteMoistureClass, decimal>();
        foreach (MoistureArea area in areas)
        {
            if (!Enum.IsDefined(area.Moisture) || area.AreaM2 < 0)
                throw new ArgumentOutOfRangeException(nameof(areas), area, "Expected a known class and non-negative area.");

            totals[area.Moisture] = totals.GetValueOrDefault(area.Moisture) + area.AreaM2;
        }

        var dominant = SiteMoistureClass.NeutralLegacy;
        decimal greatestArea = 0m;
        foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
        {
            decimal area = totals.GetValueOrDefault(moisture);
            if (area > greatestArea)
            {
                dominant = moisture;
                greatestArea = area;
            }
        }

        return new MoistureSelection(dominant);
    }

    public static DryRidgeAndMoistSlopeMap CreateDryRidgeAndMoistSlope(StandBounds bounds) => new(bounds);
}

public sealed class DryRidgeAndMoistSlopeMap
{
    internal DryRidgeAndMoistSlopeMap(StandBounds bounds)
    {
        if (!double.IsFinite(bounds.MinX) || !double.IsFinite(bounds.MaxX) || bounds.MinX >= bounds.MaxX ||
            !double.IsFinite(bounds.MinY) || !double.IsFinite(bounds.MaxY) || bounds.MinY >= bounds.MaxY)
            throw new ArgumentOutOfRangeException(nameof(bounds), bounds, "Expected finite bounds with positive width and height.");

        double spanX = bounds.MaxX - bounds.MinX;
        MidpointX = double.IsFinite(spanX)
            ? bounds.MinX + spanX / 2.0
            : bounds.MinX / 2.0 + bounds.MaxX / 2.0;
        if (MidpointX <= bounds.MinX || MidpointX >= bounds.MaxX)
            throw new ArgumentOutOfRangeException(nameof(bounds), bounds, "X bounds must have an interior representable midpoint.");

        Bounds = bounds;
    }

    public StandBounds Bounds { get; }
    public double MidpointX { get; }

    /// <summary>Samples closed rectangular bounds. The x midpoint belongs to FreshMoistMineral.</summary>
    public MoistureSelection At(double x, double y)
    {
        if (!(x >= Bounds.MinX && x <= Bounds.MaxX))
            throw new ArgumentOutOfRangeException(nameof(x), x, "X must be inside the stand.");
        if (!(y >= Bounds.MinY && y <= Bounds.MaxY))
            throw new ArgumentOutOfRangeException(nameof(y), y, "Y must be inside the stand.");

        return new MoistureSelection(x < MidpointX
            ? SiteMoistureClass.DryFreeDraining
            : SiteMoistureClass.FreshMoistMineral);
    }
}
