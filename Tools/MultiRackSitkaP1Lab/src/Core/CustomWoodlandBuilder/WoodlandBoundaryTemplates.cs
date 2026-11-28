namespace SitkaP1.Core.CustomWoodlandBuilder;

/// <summary>The four flat-local-metre field shapes a player can start from. All are calibration stylisations.</summary>
public enum WoodlandBoundaryTemplate
{
    /// <summary>A plain rectangular field.</summary>
    Rectangle,

    /// <summary>A rectangular field with one rectangular corner bay removed.</summary>
    LShape,

    /// <summary>A field that narrows from the southern headland to the northern headland.</summary>
    TaperedWedge,

    /// <summary>A field whose far boundary runs at an angle, as along a straightened stream.</summary>
    AngledStreamEdge,
}

/// <summary>
/// A simple chamfer at each of a template's four outer corners, in metres. A zero value keeps a sharp corner.
/// Each chamfer is validated against the two edges that meet at that corner, so adjacent chamfers can never meet or
/// overlap. The limits are calibration layout guardrails, not a survey standard.
/// </summary>
public readonly record struct CornerAdjustments(
    double SouthWest,
    double SouthEast,
    double NorthEast,
    double NorthWest)
{
    /// <summary>All four corners sharp.</summary>
    public static CornerAdjustments None => default;

    /// <summary>The same chamfer at all four corners.</summary>
    public static CornerAdjustments All(double metres) => new(metres, metres, metres, metres);

    public bool IsAny => SouthWest != 0.0 || SouthEast != 0.0 || NorthEast != 0.0 || NorthWest != 0.0;
}

/// <summary>
/// One boundary request. Each factory fills only the dimensions its template uses; the unused dimensions stay zero and
/// are never read. Use <see cref="WoodlandPolygon.FromShape"/> to validate and materialise the ring.
/// </summary>
public sealed record WoodlandShapeSpec
{
    private WoodlandShapeSpec(WoodlandBoundaryTemplate template)
    {
        Template = template;
    }

    public WoodlandBoundaryTemplate Template { get; }

    /// <summary>East-west extent of the base footprint, in metres.</summary>
    public double WidthM { get; private init; }

    /// <summary>North-south extent of the base footprint, in metres.</summary>
    public double DepthM { get; private init; }

    /// <summary>L-shape only: how far the southern bar reaches east, in metres.</summary>
    public double ArmWidthM { get; private init; }

    /// <summary>L-shape only: how deep the southern bar is, in metres.</summary>
    public double ArmDepthM { get; private init; }

    /// <summary>Tapered wedge only: width at the southern headland, in metres.</summary>
    public double SouthWidthM { get; private init; }

    /// <summary>Tapered wedge only: width at the northern headland, in metres.</summary>
    public double NorthWidthM { get; private init; }

    /// <summary>Angled stream edge only: north-edge eastward shift over the field depth, in metres.</summary>
    public double StreamSlantM { get; private init; }

    public CornerAdjustments Corners { get; private init; }

    public static WoodlandShapeSpec Rectangle(double widthM, double depthM, CornerAdjustments corners = default) =>
        new(WoodlandBoundaryTemplate.Rectangle)
        {
            WidthM = widthM,
            DepthM = depthM,
            Corners = corners,
        };

    public static WoodlandShapeSpec LShape(
        double widthM,
        double depthM,
        double armWidthM,
        double armDepthM,
        CornerAdjustments corners = default) =>
        new(WoodlandBoundaryTemplate.LShape)
        {
            WidthM = widthM,
            DepthM = depthM,
            ArmWidthM = armWidthM,
            ArmDepthM = armDepthM,
            Corners = corners,
        };

    public static WoodlandShapeSpec TaperedWedge(
        double southWidthM,
        double northWidthM,
        double depthM,
        CornerAdjustments corners = default) =>
        new(WoodlandBoundaryTemplate.TaperedWedge)
        {
            SouthWidthM = southWidthM,
            NorthWidthM = northWidthM,
            DepthM = depthM,
            Corners = corners,
        };

    public static WoodlandShapeSpec AngledStreamEdge(
        double widthM,
        double depthM,
        double streamSlantM,
        CornerAdjustments corners = default) =>
        new(WoodlandBoundaryTemplate.AngledStreamEdge)
        {
            WidthM = widthM,
            DepthM = depthM,
            StreamSlantM = streamSlantM,
            Corners = corners,
        };
}

/// <summary>
/// Builds the four base rings and applies the validated corner chamfers. The L-shape's two notch corners are not
/// adjustable: its four adjustments belong to the south-west, south-east, north-east and north-west outer corners,
/// which are the two southern corners of the bar and the two northern ends of the column.
/// </summary>
internal static class WoodlandBoundaryTemplates
{
    internal static WoodlandPolygon Build(WoodlandShapeSpec shape)
    {
        if (shape is null)
            throw new ArgumentNullException(nameof(shape));

        (PlanPoint[] ring, int[] chamferTargets) = CreateBaseRing(shape);
        PlanPoint[] chamfered = ApplyChamfers(ring, chamferTargets, shape.Corners);
        return WoodlandPolygon.FromVertices(chamfered);
    }

    private static (PlanPoint[] Ring, int[] ChamferTargets) CreateBaseRing(WoodlandShapeSpec shape)
    {
        switch (shape.Template)
        {
            case WoodlandBoundaryTemplate.Rectangle:
            {
                RequirePositive(shape.WidthM, "widthM");
                RequirePositive(shape.DepthM, "depthM");
                return (new[]
                {
                    new PlanPoint(0.0, 0.0),
                    new PlanPoint(shape.WidthM, 0.0),
                    new PlanPoint(shape.WidthM, shape.DepthM),
                    new PlanPoint(0.0, shape.DepthM),
                }, new[] { 0, 1, 2, 3 });
            }

            case WoodlandBoundaryTemplate.LShape:
            {
                RequirePositive(shape.WidthM, "widthM");
                RequirePositive(shape.DepthM, "depthM");
                RequirePositive(shape.ArmWidthM, "armWidthM");
                RequirePositive(shape.ArmDepthM, "armDepthM");
                if (!(shape.ArmWidthM < shape.WidthM))
                    throw new ArgumentException("an L-shape needs armWidthM smaller than widthM.", nameof(shape));
                if (!(shape.ArmDepthM < shape.DepthM))
                    throw new ArgumentException("an L-shape needs armDepthM smaller than depthM.", nameof(shape));

                return (new[]
                {
                    new PlanPoint(0.0, 0.0),
                    new PlanPoint(shape.WidthM, 0.0),
                    new PlanPoint(shape.WidthM, shape.ArmDepthM),
                    new PlanPoint(shape.ArmWidthM, shape.ArmDepthM),
                    new PlanPoint(shape.ArmWidthM, shape.DepthM),
                    new PlanPoint(0.0, shape.DepthM),
                }, new[] { 0, 1, 4, 5 });
            }

            case WoodlandBoundaryTemplate.TaperedWedge:
            {
                RequirePositive(shape.SouthWidthM, "southWidthM");
                RequirePositive(shape.NorthWidthM, "northWidthM");
                RequirePositive(shape.DepthM, "depthM");

                double halfSouth = shape.SouthWidthM / 2.0;
                double halfNorth = shape.NorthWidthM / 2.0;
                return (new[]
                {
                    new PlanPoint(-halfSouth, 0.0),
                    new PlanPoint(halfSouth, 0.0),
                    new PlanPoint(halfNorth, shape.DepthM),
                    new PlanPoint(-halfNorth, shape.DepthM),
                }, new[] { 0, 1, 2, 3 });
            }

            case WoodlandBoundaryTemplate.AngledStreamEdge:
            {
                RequirePositive(shape.WidthM, "widthM");
                RequirePositive(shape.DepthM, "depthM");
                if (!double.IsFinite(shape.StreamSlantM))
                    throw new ArgumentException("streamSlantM must be finite.", nameof(shape));
                if (Math.Abs(shape.StreamSlantM) > shape.WidthM)
                    throw new ArgumentException(
                        "the stream slant along the far edge must not exceed the field width.", nameof(shape));

                return (new[]
                {
                    new PlanPoint(0.0, 0.0),
                    new PlanPoint(shape.WidthM, 0.0),
                    new PlanPoint(shape.WidthM + shape.StreamSlantM, shape.DepthM),
                    new PlanPoint(shape.StreamSlantM, shape.DepthM),
                }, new[] { 0, 1, 2, 3 });
            }

            default:
                throw new ArgumentOutOfRangeException(
                    nameof(shape), shape.Template, "unknown woodland boundary template.");
        }
    }

    private static PlanPoint[] ApplyChamfers(PlanPoint[] ring, int[] chamferTargets, CornerAdjustments corners)
    {
        double[] requested = { corners.SouthWest, corners.SouthEast, corners.NorthEast, corners.NorthWest };
        for (int i = 0; i < requested.Length; i++)
        {
            if (!double.IsFinite(requested[i]) || requested[i] < 0.0)
                throw new ArgumentException("corner adjustments must be finite and non-negative.", nameof(corners));
        }

        if (!corners.IsAny)
            return ring;

        var cutByIndex = new Dictionary<int, double>(4);
        for (int i = 0; i < chamferTargets.Length; i++)
        {
            if (requested[i] > 0.0)
                cutByIndex[chamferTargets[i]] = requested[i];
        }

        var result = new List<PlanPoint>(ring.Length * 2);
        for (int i = 0; i < ring.Length; i++)
        {
            PlanPoint previous = ring[(i - 1 + ring.Length) % ring.Length];
            PlanPoint current = ring[i];
            PlanPoint next = ring[(i + 1) % ring.Length];

            if (!cutByIndex.TryGetValue(i, out double cut))
            {
                result.Add(current);
                continue;
            }

            double previousLength = Distance(previous, current);
            double nextLength = Distance(current, next);
            double halfShortestEdge = 0.5 * Math.Min(previousLength, nextLength);
            if (cut >= halfShortestEdge)
                throw new ArgumentException(
                    $"a {cut:F2} m chamfer at vertex {i} must be shorter than half of the {halfShortestEdge * 2.0:F2} m " +
                    "shorter adjacent edge, otherwise it would meet or cross the next corner.",
                    nameof(corners));

            // Entry point on the incoming edge, then exit point on the outgoing edge. The corner itself is removed.
            result.Add(new PlanPoint(
                current.X + (previous.X - current.X) * (cut / previousLength),
                current.Z + (previous.Z - current.Z) * (cut / previousLength)));
            result.Add(new PlanPoint(
                current.X + (next.X - current.X) * (cut / nextLength),
                current.Z + (next.Z - current.Z) * (cut / nextLength)));
        }

        return result.ToArray();
    }

    private static void RequirePositive(double value, string name)
    {
        if (!double.IsFinite(value) || value <= 0.0)
            throw new ArgumentException($"{name} must be finite and positive, got {value:R}.", name);
    }

    private static double Distance(PlanPoint a, PlanPoint b)
    {
        double dx = b.X - a.X;
        double dz = b.Z - a.Z;
        return Math.Sqrt(dx * dx + dz * dz);
    }
}
