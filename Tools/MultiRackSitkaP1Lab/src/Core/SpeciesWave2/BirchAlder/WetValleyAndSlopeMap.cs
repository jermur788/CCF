namespace SitkaP1.Core.SpeciesWave2.BirchAlder;

/// <summary>Closed rectangular stand bounds in flat local metres.</summary>
public readonly record struct WetValleyBounds(double MinX, double MinY, double MaxX, double MaxY);

/// <summary>
/// Two-zone "Wet Valley &amp; Slope" layout helper for the standard 0.64-hectare broadleaf stand.
///
/// The stand is 80 x 80 m split into five equal 16 x 80 m lanes. The low half of every lane is the wet valley bottom,
/// where Common alder thrives. The upper half is the fresh moist mineral slope, where Sitka spruce and Downy birch
/// compete. Splitting on the y midpoint (rather than the x midpoint) keeps each 16 x 80 m lane whole instead of cutting
/// a lane in half; that matters to the lane-based woodland generator and to the player's reading of the map.
///
/// This is a layout convention, not a surveyed moisture boundary. Existing woodlands are unaffected: nothing creates a
/// map unless a caller asks for one, and the default class everywhere else is <see cref="SiteMoistureClass.NeutralLegacy"/>.
/// </summary>
public sealed class WetValleyAndSlopeMap
{
    public const double StandardWidthM = 80.0;
    public const double StandardLengthM = 80.0;
    public const int StandardLaneCount = 5;
    public const double StandardLaneWidthM = StandardWidthM / StandardLaneCount;
    public const double StandardAreaHectares = StandardWidthM * StandardLengthM / 10000.0;

    public static WetValleyAndSlopeMap CreateStandard() =>
        new(new WetValleyBounds(0.0, 0.0, StandardWidthM, StandardLengthM), StandardLaneCount);

    public WetValleyAndSlopeMap(WetValleyBounds bounds, int laneCount)
    {
        if (!double.IsFinite(bounds.MinX) || !double.IsFinite(bounds.MinY) ||
            !double.IsFinite(bounds.MaxX) || !double.IsFinite(bounds.MaxY) ||
            bounds.MinX >= bounds.MaxX || bounds.MinY >= bounds.MaxY)
            throw new ArgumentOutOfRangeException(nameof(bounds), bounds, "Expected finite bounds with positive width and height.");

        ArgumentOutOfRangeException.ThrowIfLessThan(laneCount, 1);

        double spanX = bounds.MaxX - bounds.MinX;
        double spanY = bounds.MaxY - bounds.MinY;
        double midpointY = double.IsFinite(spanY)
            ? bounds.MinY + spanY / 2.0
            : bounds.MinY / 2.0 + bounds.MaxY / 2.0;
        if (midpointY <= bounds.MinY || midpointY >= bounds.MaxY)
            throw new ArgumentOutOfRangeException(nameof(bounds), bounds, "Y bounds must have an interior representable midpoint.");

        double laneWidth = spanX / laneCount;
        if (!double.IsFinite(laneWidth) || laneWidth <= 0.0)
            throw new ArgumentOutOfRangeException(nameof(laneCount), laneCount, "Lane count must leave a positive lane width.");

        Bounds = bounds;
        LaneCount = laneCount;
        LaneWidthM = laneWidth;
        MidpointY = midpointY;
    }

    public WetValleyBounds Bounds { get; }

    public int LaneCount { get; }

    public double LaneWidthM { get; }

    /// <summary>The valley/slope boundary. It belongs to the slope (<see cref="SiteMoistureClass.FreshMoistMineral"/>).</summary>
    public double MidpointY { get; }

    /// <summary>Moisture class at a point. Bounds are inclusive; the y midpoint is already slope.</summary>
    public SiteMoistureClass MoistureAt(double x, double y)
    {
        RequireInside(x, y);
        return y < MidpointY
            ? SiteMoistureClass.WetMovingAlluvial
            : SiteMoistureClass.FreshMoistMineral;
    }

    /// <summary>The species response at a point, read from the existing moisture table.</summary>
    public BirchAlderSite For(string speciesId, double x, double y) =>
        BirchAlderEngine.Site(speciesId, MoistureAt(x, y));

    /// <summary>
    /// Lane index 0..LaneCount-1 for an x coordinate, using closed lanes so the right-hand edge belongs to the last
    /// lane. Part 2 needs this to align mapped trees with lane numbers.
    /// </summary>
    public int LaneIndexOf(double x)
    {
        if (!(x >= Bounds.MinX && x <= Bounds.MaxX))
            throw new ArgumentOutOfRangeException(nameof(x), x, "X must be inside the stand.");

        double offset = x - Bounds.MinX;
        if (offset >= LaneWidthM * (LaneCount - 1))
            return LaneCount - 1;

        return (int)(offset / LaneWidthM);
    }

    /// <summary>Closed x bounds of one lane, from the stand's left edge to the stand's right edge.</summary>
    public WetValleyBounds LaneBounds(int laneIndex)
    {
        if (laneIndex < 0 || laneIndex >= LaneCount)
            throw new ArgumentOutOfRangeException(nameof(laneIndex), laneIndex, "Lane index outside the stand.");

        double minX = Bounds.MinX + LaneWidthM * laneIndex;
        double maxX = laneIndex == LaneCount - 1 ? Bounds.MaxX : Bounds.MinX + LaneWidthM * (laneIndex + 1);
        return new WetValleyBounds(minX, Bounds.MinY, maxX, Bounds.MaxY);
    }

    private void RequireInside(double x, double y)
    {
        if (!(x >= Bounds.MinX && x <= Bounds.MaxX) || !double.IsFinite(x))
            throw new ArgumentOutOfRangeException(nameof(x), x, "X must be a finite value inside the stand.");
        if (!(y >= Bounds.MinY && y <= Bounds.MaxY) || !double.IsFinite(y))
            throw new ArgumentOutOfRangeException(nameof(y), y, "Y must be a finite value inside the stand.");
    }
}
