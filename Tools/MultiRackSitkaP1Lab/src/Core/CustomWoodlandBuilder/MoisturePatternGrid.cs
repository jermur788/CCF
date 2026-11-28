using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder;

/// <summary>One painted 2.5 m cell. Applied on top of the chosen base pattern, in declaration order.</summary>
public sealed record MoisturePaintOverride(int CellX, int CellZ, SiteMoistureClass Moisture);

/// <summary>The four deterministic natural site patterns a custom woodland can start from. Calibration shapes.</summary>
public enum MoisturePatternKind
{
    /// <summary>One supplied class everywhere.</summary>
    Uniform,

    /// <summary>A catena: dry ridge in the north, fresh mineral through the middle, wet toe in the south.</summary>
    RidgeAndSlope,

    /// <summary>A straight wet corridor along X with seasonally wet banks.</summary>
    StreamCorridor,

    /// <summary>A circular bog hollow with a seasonally wet ring.</summary>
    WetHollowBogPocket,
}

/// <summary>
/// One pattern request. Each factory fills only the fields its kind uses. A patterned grid must leave
/// <see cref="UniformClass"/> at <see cref="SiteMoistureClass.NeutralLegacy"/>, so a misconfigured uniform class cannot
/// be silently ignored.
/// </summary>
public sealed record MoisturePatternSpec
{
    private MoisturePatternSpec(MoisturePatternKind kind)
    {
        Kind = kind;
    }

    public MoisturePatternKind Kind { get; }

    public SiteMoistureClass UniformClass { get; private init; }

    public double CorridorWidthM { get; private init; }

    public double HollowRadiusM { get; private init; }

    public static MoisturePatternSpec Uniform(SiteMoistureClass moisture) =>
        new(MoisturePatternKind.Uniform) { UniformClass = moisture };

    public static MoisturePatternSpec RidgeAndSlope() => new(MoisturePatternKind.RidgeAndSlope);

    public static MoisturePatternSpec StreamCorridor(double corridorWidthM = 8.0) =>
        new(MoisturePatternKind.StreamCorridor) { CorridorWidthM = corridorWidthM };

    public static MoisturePatternSpec WetHollowBogPocket(double radiusM = 10.0) =>
        new(MoisturePatternKind.WetHollowBogPocket) { HollowRadiusM = radiusM };
}

/// <summary>
/// A 2.5 m ground grid over a custom woodland, aligned to the global 2.5 m lattice so custom and imported woodlands
/// agree on cell boundaries. Only cells whose centre lies inside (or on) the boundary are active; inactive cells carry
/// <see cref="SiteMoistureClass.NeutralLegacy"/> and are never read as a site class.
///
/// Bog is a habitat protection question, not a planting recommendation: any active bog cell raises
/// <see cref="HasHabitatProtectionCaution"/> and is counted. The caution is recomputed after paint overrides.
/// </summary>
public sealed class MoistureGrid
{
    /// <summary>The simulation's ground cell size, matching <c>Phase1Stage5Options.GridCellM</c>.</summary>
    public const double CellSizeM = 2.5;

    /// <summary>Extra seasonally wet band either side of the stream corridor, in metres. Calibration.</summary>
    public const double StreamBankMarginM = 6.0;

    /// <summary>Extra seasonally wet ring around the bog hollow, in metres. Calibration.</summary>
    public const double BogHollowRingM = 5.0;

    public const double MinimumCorridorWidthM = 2.0;
    public const double MaximumCorridorWidthM = 60.0;
    public const double MinimumHollowRadiusM = 2.0;
    public const double MaximumHollowRadiusM = 50.0;

    private const double Tolerance = 1e-9;

    private readonly SiteMoistureClass[] _classes;
    private readonly bool[] _active;

    internal MoistureGrid(
        WoodlandPolygon polygon,
        MoisturePatternSpec pattern,
        IReadOnlyList<MoisturePaintOverride> overrides,
        double originX,
        double originZ,
        int columns,
        int rows,
        SiteMoistureClass[] classes,
        bool[] active,
        int activeCellCount,
        int cautionCellCount)
    {
        Polygon = polygon;
        Pattern = pattern;
        Overrides = overrides;
        OriginX = originX;
        OriginZ = originZ;
        Columns = columns;
        Rows = rows;
        _classes = classes;
        _active = active;
        ActiveCellCount = activeCellCount;
        CautionCellCount = cautionCellCount;
    }

    public WoodlandPolygon Polygon { get; }

    public MoisturePatternSpec Pattern { get; }

    /// <summary>Paint overrides in the order they were applied; the last entry for a cell won.</summary>
    public IReadOnlyList<MoisturePaintOverride> Overrides { get; }

    public double OriginX { get; }

    public double OriginZ { get; }

    public int Columns { get; }

    public int Rows { get; }

    public int ActiveCellCount { get; }

    public int CautionCellCount { get; }

    public bool HasHabitatProtectionCaution => CautionCellCount > 0;

    public PlanPoint CellCentre(int cellX, int cellZ)
    {
        RequireIndex(cellX, cellZ);
        return new PlanPoint(
            OriginX + (cellX + 0.5) * CellSizeM,
            OriginZ + (cellZ + 0.5) * CellSizeM);
    }

    public bool IsActive(int cellX, int cellZ) =>
        cellX >= 0 && cellZ >= 0 && cellX < Columns && cellZ < Rows && _active[cellZ * Columns + cellX];

    /// <summary>The resolved class of an active cell. Throws for an inactive or out-of-range cell.</summary>
    public SiteMoistureClass ClassAt(int cellX, int cellZ)
    {
        RequireIndex(cellX, cellZ);
        if (!_active[cellZ * Columns + cellX])
            throw new InvalidOperationException(
                $"cell ({cellX}, {cellZ}) is outside the woodland boundary and carries no moisture class.");

        return _classes[cellZ * Columns + cellX];
    }

    /// <summary>True when the point falls in an active grid cell.</summary>
    public bool TryClassAtPoint(PlanPoint point, out SiteMoistureClass moisture)
    {
        moisture = SiteMoistureClass.NeutralLegacy;
        if (!TryCellIndexAt(point, out int cellX, out int cellZ) || !IsActive(cellX, cellZ))
            return false;

        moisture = _classes[cellZ * Columns + cellX];
        return true;
    }

    /// <summary>The cell index containing a point, or false when the point lies outside the grid entirely.</summary>
    public bool TryCellIndexAt(PlanPoint point, out int cellX, out int cellZ)
    {
        cellX = -1;
        cellZ = -1;
        if (!point.IsFinite)
            return false;

        double relativeX = (point.X - OriginX) / CellSizeM;
        double relativeZ = (point.Z - OriginZ) / CellSizeM;
        if (relativeX < 0.0 || relativeZ < 0.0 ||
            relativeX >= Columns || relativeZ >= Rows)
            return false;

        cellX = (int)Math.Floor(relativeX);
        cellZ = (int)Math.Floor(relativeZ);
        return cellX >= 0 && cellZ >= 0 && cellX < Columns && cellZ < Rows;
    }

    private void RequireIndex(int cellX, int cellZ)
    {
        if (cellX < 0 || cellZ < 0 || cellX >= Columns || cellZ >= Rows)
            throw new ArgumentOutOfRangeException(
                nameof(cellX), $"cell ({cellX}, {cellZ}) is outside the {Columns} x {Rows} grid.");
    }

    internal static MoistureGrid Build(
        WoodlandPolygon polygon,
        MoisturePatternSpec pattern,
        IReadOnlyList<MoisturePaintOverride> overrides)
    {
        if (polygon is null)
            throw new ArgumentNullException(nameof(polygon));
        if (pattern is null)
            throw new ArgumentNullException(nameof(pattern));
        if (overrides is null)
            throw new ArgumentNullException(nameof(overrides));

        ValidatePattern(pattern);

        double originX = Math.Floor(polygon.MinX / CellSizeM) * CellSizeM;
        double originZ = Math.Floor(polygon.MinZ / CellSizeM) * CellSizeM;
        int columns = Math.Max(1, (int)Math.Ceiling((polygon.MaxX - originX) / CellSizeM - Tolerance));
        int rows = Math.Max(1, (int)Math.Ceiling((polygon.MaxZ - originZ) / CellSizeM - Tolerance));

        var classes = new SiteMoistureClass[columns * rows];
        var active = new bool[columns * rows];
        int activeCount = 0;

        for (int cellZ = 0; cellZ < rows; cellZ++)
        {
            for (int cellX = 0; cellX < columns; cellX++)
            {
                var centre = new PlanPoint(
                    originX + (cellX + 0.5) * CellSizeM,
                    originZ + (cellZ + 0.5) * CellSizeM);

                int index = cellZ * columns + cellX;
                if (!polygon.ContainsPoint(centre))
                {
                    classes[index] = SiteMoistureClass.NeutralLegacy;
                    continue;
                }

                active[index] = true;
                activeCount++;
                classes[index] = BaseClass(pattern, polygon, centre);
            }
        }

        if (activeCount == 0)
            throw new ArgumentException(
                "the field is too small to contain a single 2.5 m ground cell.", nameof(polygon));

        foreach (MoisturePaintOverride painted in overrides)
        {
            if (!IsRealMoistureClass(painted.Moisture))
                throw new ArgumentException(
                    "paint overrides may only use a real moisture class, not NeutralLegacy.", nameof(overrides));

            if (painted.CellX < 0 || painted.CellZ < 0 || painted.CellX >= columns || painted.CellZ >= rows ||
                !active[painted.CellZ * columns + painted.CellX])
                throw new ArgumentException(
                    $"paint override ({painted.CellX}, {painted.CellZ}) does not address an active cell inside the field.",
                    nameof(overrides));

            classes[painted.CellZ * columns + painted.CellX] = painted.Moisture;
        }

        int cautionCount = 0;
        for (int index = 0; index < classes.Length; index++)
        {
            if (active[index] && SpeciesMoistureTable.HabitatProtectionCaution(classes[index]))
                cautionCount++;
        }

        return new MoistureGrid(
            polygon,
            pattern,
            Array.AsReadOnly(overrides.ToArray()),
            originX,
            originZ,
            columns,
            rows,
            classes,
            active,
            activeCount,
            cautionCount);
    }

    private static void ValidatePattern(MoisturePatternSpec pattern)
    {
        switch (pattern.Kind)
        {
            case MoisturePatternKind.Uniform:
                if (!IsRealMoistureClass(pattern.UniformClass))
                    throw new ArgumentException(
                        "a uniform grid needs one real moisture class; NeutralLegacy is reserved for existing woodlands.",
                        nameof(pattern));
                break;

            case MoisturePatternKind.RidgeAndSlope:
                RequireNoUniformClass(pattern);
                break;

            case MoisturePatternKind.StreamCorridor:
                RequireNoUniformClass(pattern);
                if (!double.IsFinite(pattern.CorridorWidthM) ||
                    pattern.CorridorWidthM < MinimumCorridorWidthM ||
                    pattern.CorridorWidthM > MaximumCorridorWidthM)
                    throw new ArgumentException(
                        $"the stream corridor width must be {MinimumCorridorWidthM:F0}-{MaximumCorridorWidthM:F0} m.",
                        nameof(pattern));
                break;

            case MoisturePatternKind.WetHollowBogPocket:
                RequireNoUniformClass(pattern);
                if (!double.IsFinite(pattern.HollowRadiusM) ||
                    pattern.HollowRadiusM < MinimumHollowRadiusM ||
                    pattern.HollowRadiusM > MaximumHollowRadiusM)
                    throw new ArgumentException(
                        $"the bog hollow radius must be {MinimumHollowRadiusM:F0}-{MaximumHollowRadiusM:F0} m.",
                        nameof(pattern));
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(pattern), pattern.Kind, "unknown moisture pattern.");
        }
    }

    private static void RequireNoUniformClass(MoisturePatternSpec pattern)
    {
        if (pattern.UniformClass != SiteMoistureClass.NeutralLegacy)
            throw new ArgumentException(
                "a patterned grid must leave the uniform class unset; a supplied class would be ignored otherwise.",
                nameof(pattern));
    }

    private static SiteMoistureClass BaseClass(MoisturePatternSpec pattern, WoodlandPolygon polygon, PlanPoint centre)
    {
        switch (pattern.Kind)
        {
            case MoisturePatternKind.Uniform:
                return pattern.UniformClass;

            case MoisturePatternKind.RidgeAndSlope:
            {
                double position = (centre.Z - polygon.MinZ) / polygon.LengthM;
                if (position < 0.25)
                    return SiteMoistureClass.SeasonallyWetAerated;
                return position < 0.75
                    ? SiteMoistureClass.FreshMoistMineral
                    : SiteMoistureClass.DryFreeDraining;
            }

            case MoisturePatternKind.StreamCorridor:
            {
                double midZ = polygon.MinZ + polygon.LengthM / 2.0;
                double distance = Math.Abs(centre.Z - midZ);
                double halfWidth = pattern.CorridorWidthM / 2.0;
                if (distance <= halfWidth)
                    return SiteMoistureClass.WetMovingAlluvial;
                return distance <= halfWidth + StreamBankMarginM
                    ? SiteMoistureClass.SeasonallyWetAerated
                    : SiteMoistureClass.FreshMoistMineral;
            }

            case MoisturePatternKind.WetHollowBogPocket:
            {
                PlanPoint centroid = polygon.Centroid;
                double dx = centre.X - centroid.X;
                double dz = centre.Z - centroid.Z;
                double distance = Math.Sqrt(dx * dx + dz * dz);
                if (distance <= pattern.HollowRadiusM)
                    return SiteMoistureClass.StagnantPoorBog;
                return distance <= pattern.HollowRadiusM + BogHollowRingM
                    ? SiteMoistureClass.SeasonallyWetAerated
                    : SiteMoistureClass.FreshMoistMineral;
            }

            default:
                throw new ArgumentOutOfRangeException(nameof(pattern), pattern.Kind, "unknown moisture pattern.");
        }
    }

    /// <summary>True for the five real moisture classes; false for <see cref="SiteMoistureClass.NeutralLegacy"/>.</summary>
    public static bool IsRealMoistureClass(SiteMoistureClass moisture) =>
        moisture is SiteMoistureClass.DryFreeDraining
            or SiteMoistureClass.FreshMoistMineral
            or SiteMoistureClass.SeasonallyWetAerated
            or SiteMoistureClass.WetMovingAlluvial
            or SiteMoistureClass.StagnantPoorBog;
}
