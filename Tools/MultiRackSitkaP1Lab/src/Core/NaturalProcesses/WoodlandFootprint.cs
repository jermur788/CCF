namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// The woodland outline in local metres. Used to size the jay caps (hectares) and to decide whether a carried acorn lands
/// inside the woodland. Points exactly on the outline count as inside.
/// </summary>
public sealed class WoodlandFootprint
{
    private readonly LocalPoint[] _vertices;
    private readonly IReadOnlyList<LocalPoint> _readOnlyVertices;

    /// <summary>Copies <paramref name="vertices"/>: changing the caller's collection afterwards never moves the outline.</summary>
    public WoodlandFootprint(IReadOnlyList<LocalPoint> vertices)
    {
        _vertices = (vertices ?? Array.Empty<LocalPoint>()).ToArray();
        _readOnlyVertices = Array.AsReadOnly(_vertices);
    }

    public static WoodlandFootprint Rectangle(double minX, double minY, double maxX, double maxY) =>
        new(new[] { new LocalPoint(minX, minY), new LocalPoint(maxX, minY), new LocalPoint(maxX, maxY), new LocalPoint(minX, maxY) });

    public IReadOnlyList<LocalPoint> Vertices => _readOnlyVertices;

    /// <summary>True when the outline has at least three finite vertices and a positive area.</summary>
    public bool IsValid =>
        _vertices.Length >= 3 &&
        _vertices.All(v => double.IsFinite(v.X) && double.IsFinite(v.Y)) &&
        AreaSquareMetres > 0.0;

    /// <summary>Shoelace area, always non-negative.</summary>
    public double AreaSquareMetres
    {
        get
        {
            double twice = 0.0;
            for (int i = 0; i < _vertices.Length; i++)
            {
                LocalPoint a = _vertices[i];
                LocalPoint b = _vertices[(i + 1) % _vertices.Length];
                twice += a.X * b.Y - b.X * a.Y;
            }
            return Math.Abs(twice) * 0.5;
        }
    }

    public double AreaHectares => AreaSquareMetres / 10_000.0;

    public double MinX => _vertices.Min(v => v.X);
    public double MaxX => _vertices.Max(v => v.X);
    public double MinY => _vertices.Min(v => v.Y);
    public double MaxY => _vertices.Max(v => v.Y);

    /// <summary>Boundary-inclusive even-odd ray cast.</summary>
    public bool Contains(LocalPoint p)
    {
        if (!double.IsFinite(p.X) || !double.IsFinite(p.Y)) return false;
        bool inside = false;
        for (int i = 0, j = _vertices.Length - 1; i < _vertices.Length; j = i++)
        {
            LocalPoint a = _vertices[i];
            LocalPoint b = _vertices[j];
            if (OnSegment(a, b, p)) return true;
            bool crosses = (a.Y > p.Y) != (b.Y > p.Y);
            if (crosses && p.X < (b.X - a.X) * (p.Y - a.Y) / (b.Y - a.Y) + a.X)
                inside = !inside;
        }
        return inside;
    }

    private static bool OnSegment(LocalPoint a, LocalPoint b, LocalPoint p)
    {
        const double tolerance = 1e-9;
        double cross = (b.X - a.X) * (p.Y - a.Y) - (b.Y - a.Y) * (p.X - a.X);
        if (Math.Abs(cross) > tolerance) return false;
        return p.X >= Math.Min(a.X, b.X) - tolerance && p.X <= Math.Max(a.X, b.X) + tolerance &&
               p.Y >= Math.Min(a.Y, b.Y) - tolerance && p.Y <= Math.Max(a.Y, b.Y) + tolerance;
    }
}
