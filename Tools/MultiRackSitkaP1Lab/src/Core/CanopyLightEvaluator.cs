namespace SitkaP1.Core;

// Multi #900 (owner decision D-094): pure, read-only canopy cover, growing-space diagnostics and modelled forest-floor light
// over engine-authoritative trees and each area polygon. Three different quantities, reported separately:
//   Layer A  growing-space competition: Hegyi CI, living neighbours within 8 m, crown radius, stems/ha, basal area.
//   Layer B  geometric crown cover: the polygon-clipped UNION of living crown circles (never the sum of crown areas).
//   Layer C  Modelled Light Index (Unity k10a10 proxy — not calibrated PAR), version unity-k10a10-decoupled-v1.
// Only LIVING trees take part: felled stumps and biologically dead trees cast nothing. The tree neighbourhood is the
// connected woodland (every area); every summary is per area polygon and areas are never blended. Nothing here is saved.

/// <summary>A polygon vertex in engine metres (X east, Z north).</summary>
public sealed record CanopyVertex(double XM, double ZM);

/// <summary>One area to evaluate: its strictly convex counter-clockwise polygon, engine hectares and its own trees (any state).</summary>
public sealed record CanopyArea(string AreaId, IReadOnlyList<CanopyVertex> Polygon, float AreaHa, IReadOnlyList<AnnualTree> Trees);

/// <summary>Version identity, units and honesty text of every canopy/light read-out.</summary>
public sealed record LightModelMetadata(
    string Version,
    string Kind,
    bool CalibratedPar,
    string Label,
    double GridCellM,
    double ShadeBufferM,
    double ShadeReachPerCrownRadius,
    double VerticalFullHeightM,
    double CoverMicroCellM,
    string CiScope,
    string CrownRuleId,
    string Caveat,
    IReadOnlyList<string> Uncertainties);

/// <summary>Layer A, one living tree: connected-woodland Hegyi CI, living neighbours within 8 m (and how many are cross-border).</summary>
public sealed record TreeGrowingSpace(string TreeId, string AreaId, float Ci, int NeighboursWithin8M, int CrossBorderNeighbours,
    float CrownRadiusM);

/// <summary>Layer A, one area: living stems, stems/ha and basal area m²/ha (the engine's own stand arithmetic). No m²/tree.</summary>
public sealed record CanopyStandDiagnostics(int LivingStems, double StemsPerHa, double BasalAreaM2PerHa);

/// <summary>
/// Layer B, one area, on a 0.25 m micro-grid of cell centres strictly inside the polygon. Each included micro-cell is
/// weighted by its exact polygon-clipped area. Covered = clipped area of cells whose centre is inside at least one living
/// crown circle (the union). UncoveredM2 = polygon area − CoveredM2 and CoverFraction = CoveredM2 / polygon area.
/// SampledAreaM2 (the sum of clipped areas of included cells, ≤ the polygon area) is reported, not renormalised away.
/// Overlapped = clipped area inside two or more crowns. MeanOverlapCount is the area-weighted mean number of crowns over
/// the covered area. Openings are 4-connected uncovered regions; an opening's area is the sum of its clipped cell areas and
/// its width is the diameter of the largest circle inscribed in it on the micro-grid, bounded by crowns and the polygon edge.
/// </summary>
public sealed record CanopyCover(double MicroCellM, double SampledAreaM2, double CoveredM2, double UncoveredM2, double CoverFraction,
    double OverlappedM2, double MeanOverlapCount, int OpeningCount, double LargestOpeningAreaM2, double LargestOpeningWidthM);

/// <summary>Layer C, one light cell whose centre is inside the polygon: centre, polygon-clipped area and light index.</summary>
public sealed record CanopyLightCell(int Column, int Row, float CentreXM, float CentreZM, double ClippedAreaM2, float LightIndex,
    int CrossBorderTrees);

/// <summary>Layer C, one area: every included cell and the clipped-area-weighted summary. SampledAreaM2 ≤ the polygon area.</summary>
public sealed record CanopyLightField(double CellM, IReadOnlyList<CanopyLightCell> Cells, double SampledAreaM2, double MeanLightIndex,
    float MinLightIndex, float MaxLightIndex, int CellsWithCrossBorderInfluence);

public sealed record CanopyAreaReport(string AreaId, double PolygonAreaM2, CanopyStandDiagnostics Stand, CanopyCover Cover,
    CanopyLightField Light);

public sealed record CanopyLightReport(LightModelMetadata Metadata, IReadOnlyList<TreeGrowingSpace> Trees,
    IReadOnlyList<CanopyAreaReport> Areas);

public static class CanopyLightEvaluator
{
    public const string Version = "unity-k10a10-decoupled-v1";
    public const string Kind = "index";
    public const string Label = "Modelled Light Index (Unity k10a10 proxy — not calibrated PAR)";
    public const double DefaultLightCellM = 2.5;
    public const double UnityParityLightCellM = 5.0;
    public const double CoverMicroCellM = 0.25;
    /// <summary>Fixed physical shade buffer b. Independent of the grid; at a 5.0 m grid it equals Unity's cell/2.</summary>
    public const float ShadeBufferM = 2.5f;
    /// <summary>Unity CanopyShadeReachPerCrownRadius (k10a10 gameplay calibration): reach = r × 1 + b.</summary>
    public const float ShadeReachPerCrownRadius = 1f;
    /// <summary>vertical = clamp01(h / 8): gameplay calibration, not a measured height response.</summary>
    public const float VerticalFullHeightM = 8f;
    public const string CiScope = "connected-woodland Hegyi, 8 m radius, 0.5 m distance floor, self-excluded, ordinal TreeId order";

    public const string Caveat =
        "A relative 0–1 index from the Unity k10a10 gameplay kernel: reach = crown radius + 2.5 m, lateral = clamp01(1 − d/reach), " +
        "vertical = clamp01(height/8), light = product of (1 − influence) over living trees. It is NOT measured or calibrated PAR, " +
        "not percent transmittance and not the uncovered fraction. No LAI, extinction coefficient or crown opacity is modelled; " +
        "missing PAR/LAI evidence stays UNKNOWN.";

    public static readonly IReadOnlyList<string> Uncertainties = Array.AsReadOnly(new[]
    {
        "C2 crown bias: realised radii probably overstate mean projected radius by about 5–15 % (one Irish site, Frenchpark; maximum widths).",
        "Area A is a synthetic, highly stocked stand (about 74 m²/ha basal area); synthetic MA-1 stands are not an Irish empirical forecast.",
        "Open-grassland outer edge: no neighbours exist beyond the outer perimeter, so edge cells are brighter than a stand interior.",
        "Circular crown projection is a geometric simplification of real crown shape.",
        "Non-causal diagnostic: cover and the light index do not drive growth, mortality or regeneration in this engine.",
    });

    public static void RequireSupportedGrid(double lightCellM)
    {
        if (lightCellM != DefaultLightCellM && lightCellM != UnityParityLightCellM)
            throw new P1Refusal("CanopyGridUnsupported", "the light grid is 2.5 m (default) or 5.0 m (Unity parity) only");
    }

    public static LightModelMetadata Metadata(double lightCellM, CrownModelVersion crownModel) => new(Version, Kind, CalibratedPar: false,
        Label, lightCellM, ShadeBufferM, ShadeReachPerCrownRadius, VerticalFullHeightM, CoverMicroCellM, CiScope,
        CrownModel.RuleId(crownModel), Caveat, Uncertainties);

    /// <summary>
    /// The light kernel at one point over living trees given in ordinal TreeId order, in float32 with Unity's exact operation
    /// order: gap *= 1 − clamp01(clamp01(1 − d/reach) × clamp01(h/8)); light = 1 − clamp01(1 − gap). Finite, in [0, 1].
    /// </summary>
    public static float LightIndexAt(float xM, float zM, IReadOnlyList<AnnualTree> livingInTreeIdOrder) =>
        Kernel(xM, zM, livingInTreeIdOrder, owner: null, areaId: null).Light;

    public static CanopyLightReport Evaluate(IReadOnlyList<CanopyArea> areas, double lightCellM = DefaultLightCellM,
        CrownModelVersion crownModel = CrownModelVersion.LegacyFrozen)
    {
        RequireSupportedGrid(lightCellM);
        CrownModel.RequireKnown(crownModel);
        if (areas is null || areas.Count == 0) throw new P1Refusal("CanopyAreasRequired", "at least one area is required");

        var owner = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var area in areas)
        {
            if (string.IsNullOrEmpty(area.AreaId) || areas.Count(other => other.AreaId == area.AreaId) != 1)
                throw new P1Refusal("CanopyAreaIdInvalid", "area IDs must be present and unique");
            RequireConvexCounterClockwise(area.Polygon, area.AreaId);
            foreach (var tree in area.Trees)
            {
                if (!float.IsFinite(tree.XM) || !float.IsFinite(tree.ZM) || !float.IsFinite(tree.CrownRadiusM) ||
                    !float.IsFinite(tree.HeightM) || tree.CrownRadiusM < 0f || tree.HeightM < 0f)
                    throw new P1Refusal("CanopyTreeInvalid", "tree position, crown radius and height must be finite and non-negative: " + tree.TreeId);
                if (!owner.TryAdd(tree.TreeId, area.AreaId))
                    throw new P1Refusal("CanopyTreeIdNotUnique", "tree IDs must be unique across the woodland: " + tree.TreeId);
            }
        }

        // The connected woodland: every living tree of every area, ordinal TreeId order (deterministic products and sums).
        var woodland = CrownModel.Woodland(areas.SelectMany(area => area.Trees));
        float cutoff = SpeciesProfile.Sitka.HegyiCutoffM;

        var perTree = new List<TreeGrowingSpace>();
        foreach (var tree in woodland)
        {
            var (ci, neighbours) = CrownModel.WoodlandCompetition(tree, woodland);
            int crossBorder = 0;
            foreach (var other in woodland)
            {
                if (other.TreeId == tree.TreeId || owner[other.TreeId] == owner[tree.TreeId]) continue;
                // The same float distance and cutoff test as the Hegyi sum, so the two counts agree exactly.
                float dx = tree.XM - other.XM;
                float dz = tree.ZM - other.ZM;
                if ((float)Math.Sqrt(dx * dx + dz * dz) <= cutoff) crossBorder++;
            }
            perTree.Add(new TreeGrowingSpace(tree.TreeId, owner[tree.TreeId], ci, neighbours, crossBorder, tree.CrownRadiusM));
        }

        var reports = areas.Select(area =>
        {
            var stand = WorkRules.Summarize(area.Trees, area.AreaHa);
            return new CanopyAreaReport(area.AreaId, PolygonAreaM2(area.Polygon),
                new CanopyStandDiagnostics(stand.LivingCount, stand.StemsPerHa, stand.BasalAreaM2PerHa),
                Cover(area.Polygon, woodland), Light(area, woodland, owner, lightCellM));
        }).ToArray();

        return new CanopyLightReport(Metadata(lightCellM, crownModel), perTree.AsReadOnly(), Array.AsReadOnly(reports));
    }

    // ------------------------------------------------------------------------------------------------ Layer C: light

    private static (float Light, int CrossBorderTrees) Kernel(float x, float z, IReadOnlyList<AnnualTree> living,
        IReadOnlyDictionary<string, string>? owner, string? areaId)
    {
        float gap = 1f;
        int crossBorder = 0;
        foreach (var tree in living)
        {
            if (!tree.IsLiving) continue;
            float dx = x - tree.XM;
            float dz = z - tree.ZM;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            float reach = tree.CrownRadiusM * ShadeReachPerCrownRadius + ShadeBufferM;
            float lateral = Clamp01(1f - distance / reach);
            float vertical = Clamp01(tree.HeightM / VerticalFullHeightM);
            float influence = Clamp01(lateral * vertical);
            gap *= 1f - influence;
            if (influence > 0f && owner is not null && owner[tree.TreeId] != areaId) crossBorder++;
        }
        float canopy = Clamp01(1f - gap);
        return (1f - canopy, crossBorder);
    }

    private static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;

    // ------------------------------------------------------------------------------------------------ per-cell detail (#900 UI)
    // Internal read-only helpers for the /multi ground inspector. They never change Evaluate: each repeats an existing rule
    // exactly (the same float kernel, the same micro-grid and inside test, the same Sutherland–Hodgman clip).

    /// <summary>
    /// The living trees whose light influence at (x, z) is above zero, in the given (ordinal TreeId) order: exactly the
    /// factors of <see cref="LightIndexAt"/> that differ from 1. Evaluating the kernel over these trees alone therefore gives
    /// the same light index bit for bit.
    /// </summary>
    internal static IReadOnlyList<string> ContributorsAt(float x, float z, IReadOnlyList<AnnualTree> livingInTreeIdOrder)
    {
        var ids = new List<string>();
        foreach (var tree in livingInTreeIdOrder)
        {
            if (!tree.IsLiving) continue;
            float dx = x - tree.XM;
            float dz = z - tree.ZM;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            float reach = tree.CrownRadiusM * ShadeReachPerCrownRadius + ShadeBufferM;
            float lateral = Clamp01(1f - distance / reach);
            float vertical = Clamp01(tree.HeightM / VerticalFullHeightM);
            if (Clamp01(lateral * vertical) > 0f) ids.Add(tree.TreeId);
        }
        return ids.AsReadOnly();
    }

    /// <summary>The convex polygon clipped to one axis-aligned square (the light cell's drawn shape), in engine metres.</summary>
    internal static IReadOnlyList<CanopyVertex> ClippedSquare(IReadOnlyList<CanopyVertex> polygon, double x0, double z0, double x1, double z1)
    {
        var ring = polygon.Select(p => (p.XM, p.ZM)).ToList();
        ring = Clip(ring, p => p.Item1 - x0, (a, b) => Lerp(a, b, (x0 - a.Item1) / (b.Item1 - a.Item1)));
        ring = Clip(ring, p => x1 - p.Item1, (a, b) => Lerp(a, b, (x1 - a.Item1) / (b.Item1 - a.Item1)));
        ring = Clip(ring, p => p.Item2 - z0, (a, b) => Lerp(a, b, (z0 - a.Item2) / (b.Item2 - a.Item2)));
        ring = Clip(ring, p => z1 - p.Item2, (a, b) => Lerp(a, b, (z1 - a.Item2) / (b.Item2 - a.Item2)));
        return ring.Count < 3 ? Array.Empty<CanopyVertex>() : ring.Select(p => new CanopyVertex(p.Item1, p.Item2)).ToArray();
    }

    /// <summary>
    /// Layer B restricted to one light cell: the clipped area of the 0.25 m micro-cells inside the light cell whose centres
    /// are strictly inside the polygon (sampled), and of those whose centre is also within a living crown circle (covered).
    /// The micro-grid is the same world-anchored lattice, inside test, crown test and clipped weighting as Layer B.
    /// </summary>
    internal static (double CoveredM2, double SampledM2) CellCover(IReadOnlyList<CanopyVertex> polygon,
        IReadOnlyList<AnnualTree> living, int column, int row, double lightCellM)
    {
        const double s = CoverMicroCellM;
        int perCell = (int)Math.Round(lightCellM / s);
        int mc0 = column * perCell, mr0 = row * perCell;
        double x0 = column * lightCellM, z0 = row * lightCellM, x1 = (column + 1) * lightCellM, z1 = (row + 1) * lightCellM;
        // Only crowns whose bounding square touches this cell can cover one of its micro-cells.
        var near = living.Where(t => t.IsLiving && t.XM + t.CrownRadiusM >= x0 - s && t.XM - t.CrownRadiusM <= x1 + s &&
            t.ZM + t.CrownRadiusM >= z0 - s && t.ZM - t.CrownRadiusM <= z1 + s).ToArray();
        double covered = 0.0, sampled = 0.0;
        for (int r = mr0; r < mr0 + perCell; r++)
        for (int c = mc0; c < mc0 + perCell; c++)
        {
            double cx = (c + 0.5) * s, cz = (r + 0.5) * s;
            if (!StrictlyInside(polygon, cx, cz)) continue;
            double mx0 = c * s, mz0 = r * s;
            double a = SquareWhollyInside(polygon, mx0, mz0, mx0 + s, mz0 + s) ? s * s : ClippedSquareArea(polygon, mx0, mz0, mx0 + s, mz0 + s);
            sampled += a;
            foreach (var tree in near)
            {
                double dx = cx - tree.XM, dz = cz - tree.ZM, radius = tree.CrownRadiusM;
                if (dx * dx + dz * dz <= radius * radius) { covered += a; break; }
            }
        }
        return (covered, sampled);
    }

    private static CanopyLightField Light(CanopyArea area, AnnualTree[] woodland, IReadOnlyDictionary<string, string> owner, double cell)
    {
        var polygon = area.Polygon;
        var (minX, minZ, maxX, maxZ) = Bounds(polygon);
        // Cells are anchored at the world origin, so neighbouring areas share one cell lattice.
        int c0 = (int)Math.Floor(minX / cell), c1 = (int)Math.Ceiling(maxX / cell) - 1;
        int r0 = (int)Math.Floor(minZ / cell), r1 = (int)Math.Ceiling(maxZ / cell) - 1;
        var cells = new List<CanopyLightCell>();
        double weighted = 0.0, sampled = 0.0;
        float min = 1f, max = 0f;
        int crossCells = 0;
        for (int r = r0; r <= r1; r++)
        for (int c = c0; c <= c1; c++)
        {
            float cx = (float)((c + 0.5) * cell);
            float cz = (float)((r + 0.5) * cell);
            if (!StrictlyInside(polygon, cx, cz)) continue; // a cell whose centre is outside the polygon is excluded
            double clipped = ClippedSquareArea(polygon, c * cell, r * cell, (c + 1) * cell, (r + 1) * cell);
            var (light, crossBorder) = Kernel(cx, cz, woodland, owner, area.AreaId);
            cells.Add(new CanopyLightCell(c, r, cx, cz, clipped, light, crossBorder));
            weighted += light * clipped;
            sampled += clipped;
            min = MathF.Min(min, light);
            max = MathF.Max(max, light);
            if (crossBorder > 0) crossCells++;
        }
        if (cells.Count == 0)
            throw new P1Refusal("CanopyAreaTooSmall", "no light cell centre lies inside " + area.AreaId);
        return new CanopyLightField(cell, cells.AsReadOnly(), sampled, weighted / sampled, min, max, crossCells);
    }

    // ------------------------------------------------------------------------------------------------ Layer B: cover

    private static CanopyCover Cover(IReadOnlyList<CanopyVertex> polygon, AnnualTree[] woodland)
    {
        const double s = CoverMicroCellM;
        var (minX, minZ, maxX, maxZ) = Bounds(polygon);
        // One padding ring of outside cells keeps every opening bounded for the distance transform.
        int c0 = (int)Math.Floor(minX / s) - 1, c1 = (int)Math.Ceiling(maxX / s);
        int r0 = (int)Math.Floor(minZ / s) - 1, r1 = (int)Math.Ceiling(maxZ / s);
        int cols = c1 - c0 + 1, rows = r1 - r0 + 1;
        var inside = new bool[cols * rows];
        var count = new int[cols * rows];
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
            inside[r * cols + c] = StrictlyInside(polygon, (c0 + c + 0.5) * s, (r0 + r + 0.5) * s);

        // Rasterise each living crown circle (any area) onto this area's micro-grid: a centre within r is covered.
        foreach (var tree in woodland)
        {
            double tx = tree.XM, tz = tree.ZM, radius = tree.CrownRadiusM;
            int ca = Math.Max(0, (int)Math.Floor((tx - radius) / s) - 1 - c0);
            int cb = Math.Min(cols - 1, (int)Math.Floor((tx + radius) / s) + 1 - c0);
            int ra = Math.Max(0, (int)Math.Floor((tz - radius) / s) - 1 - r0);
            int rb = Math.Min(rows - 1, (int)Math.Floor((tz + radius) / s) + 1 - r0);
            for (int r = ra; r <= rb; r++)
            for (int c = ca; c <= cb; c++)
            {
                double dx = (c0 + c + 0.5) * s - tx;
                double dz = (r0 + r + 0.5) * s - tz;
                if (dx * dx + dz * dz <= radius * radius) count[r * cols + c]++;
            }
        }

        // Every included micro-cell is weighted by the exact area of the polygon clipped to it, so a boundary cell counts
        // only the part that lies inside the rack. Cells whose four corners are all inside (or on) the convex polygon are
        // whole; only boundary cells need the Sutherland–Hodgman clip shared with the light layer.
        var cellArea = new double[cols * rows];
        double sampledM2 = 0.0, coveredM2 = 0.0, overlappedM2 = 0.0, coveredCountArea = 0.0;
        var open = new bool[cols * rows];
        for (int i = 0; i < open.Length; i++)
        {
            if (!inside[i]) continue;
            double x0 = (c0 + i % cols) * s, z0 = (r0 + i / cols) * s;
            double a = SquareWhollyInside(polygon, x0, z0, x0 + s, z0 + s)
                ? s * s
                : ClippedSquareArea(polygon, x0, z0, x0 + s, z0 + s);
            cellArea[i] = a;
            sampledM2 += a;
            if (count[i] > 0)
            {
                coveredM2 += a;
                coveredCountArea += count[i] * a;
                if (count[i] >= 2) overlappedM2 += a;
            }
            else open[i] = true;
        }

        // Openings: 4-connected components of uncovered inside cells, labelled in row-major order (deterministic).
        var distance2 = SquaredDistanceToBlocked(open, cols, rows);
        var label = new int[cols * rows];
        var queue = new Queue<int>();
        int openings = 0;
        double largestAreaM2 = 0.0;
        double largestWidthM = 0.0;
        for (int start = 0; start < open.Length; start++)
        {
            if (!open[start] || label[start] != 0) continue;
            openings++;
            double size = 0.0;
            double maxD2 = 0.0;
            label[start] = openings;
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                int i = queue.Dequeue();
                size += cellArea[i];
                maxD2 = Math.Max(maxD2, distance2[i]);
                int r = i / cols, c = i % cols;
                foreach (int n in new[] { c > 0 ? i - 1 : -1, c < cols - 1 ? i + 1 : -1, r > 0 ? i - cols : -1, r < rows - 1 ? i + cols : -1 })
                    if (n >= 0 && open[n] && label[n] == 0) { label[n] = openings; queue.Enqueue(n); }
            }
            // Inscribed diameter: centre-to-nearest-blocked-centre distance d gives a clear width of (2d − 1) cells.
            double widthM = Math.Max(1.0, 2.0 * Math.Sqrt(maxD2) - 1.0) * s;
            if (size > largestAreaM2 || (size == largestAreaM2 && widthM > largestWidthM))
            {
                largestAreaM2 = size;
                largestWidthM = widthM;
            }
        }

        // Covered and uncovered partition the true polygon area (not the sampled area), and the fraction uses the polygon
        // area too. SampledAreaM2 stays visible beside it: the strip of boundary cells whose centres fall outside is not
        // sampled, and that loss is reported, not renormalised away (it is counted as uncovered).
        double polygonM2 = PolygonAreaM2(polygon);
        return new CanopyCover(s, sampledM2, coveredM2, polygonM2 - coveredM2, coveredM2 / polygonM2, overlappedM2,
            coveredM2 == 0.0 ? 0.0 : coveredCountArea / coveredM2, openings, largestAreaM2,
            openings == 0 ? 0.0 : largestWidthM);
    }

    /// <summary>True when all four corners of the square are inside or on the convex polygon (so the square is wholly inside).</summary>
    private static bool SquareWhollyInside(IReadOnlyList<CanopyVertex> polygon, double x0, double z0, double x1, double z1)
    {
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            if (Cross(a.XM, a.ZM, b.XM, b.ZM, x0, z0) < 0 || Cross(a.XM, a.ZM, b.XM, b.ZM, x1, z0) < 0 ||
                Cross(a.XM, a.ZM, b.XM, b.ZM, x1, z1) < 0 || Cross(a.XM, a.ZM, b.XM, b.ZM, x0, z1) < 0)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Exact squared Euclidean distance (in cells) from every cell to the nearest blocked (not open) cell centre,
    /// Felzenszwalb–Huttenlocher two-pass transform. The caller pads the grid with blocked cells.
    /// </summary>
    private static double[] SquaredDistanceToBlocked(bool[] open, int cols, int rows)
    {
        const double Far = 1e18;
        var d = new double[cols * rows];
        for (int i = 0; i < d.Length; i++) d[i] = open[i] ? Far : 0.0;
        int n = Math.Max(cols, rows);
        var f = new double[n];
        var outLine = new double[n];
        var v = new int[n];
        var z = new double[n + 1];
        for (int c = 0; c < cols; c++)
        {
            for (int r = 0; r < rows; r++) f[r] = d[r * cols + c];
            Transform1D(f, rows, outLine, v, z);
            for (int r = 0; r < rows; r++) d[r * cols + c] = outLine[r];
        }
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++) f[c] = d[r * cols + c];
            Transform1D(f, cols, outLine, v, z);
            for (int c = 0; c < cols; c++) d[r * cols + c] = outLine[c];
        }
        return d;
    }

    private static void Transform1D(double[] f, int n, double[] result, int[] v, double[] z)
    {
        int k = 0;
        v[0] = 0;
        z[0] = double.NegativeInfinity;
        z[1] = double.PositiveInfinity;
        for (int q = 1; q < n; q++)
        {
            double s = (f[q] + (double)q * q - (f[v[k]] + (double)v[k] * v[k])) / (2.0 * q - 2.0 * v[k]);
            while (s <= z[k])
            {
                k--;
                s = (f[q] + (double)q * q - (f[v[k]] + (double)v[k] * v[k])) / (2.0 * q - 2.0 * v[k]);
            }
            k++;
            v[k] = q;
            z[k] = s;
            z[k + 1] = double.PositiveInfinity;
        }
        k = 0;
        for (int q = 0; q < n; q++)
        {
            while (z[k + 1] < q) k++;
            double delta = q - v[k];
            result[q] = delta * delta + f[v[k]];
        }
    }

    // ------------------------------------------------------------------------------------------------ geometry (metres)

    private static void RequireConvexCounterClockwise(IReadOnlyList<CanopyVertex> polygon, string areaId)
    {
        if (polygon is null || polygon.Count < 3)
            throw new P1Refusal("PolygonInvalid", areaId + " polygon needs at least three vertices");
        foreach (var p in polygon)
            if (!double.IsFinite(p.XM) || !double.IsFinite(p.ZM))
                throw new P1Refusal("PolygonInvalid", areaId + " polygon has a non-finite vertex");
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            for (int k = 0; k < polygon.Count; k++)
            {
                if (k == i || k == (i + 1) % polygon.Count) continue;
                if (Cross(a.XM, a.ZM, b.XM, b.ZM, polygon[k].XM, polygon[k].ZM) <= 0)
                    throw new P1Refusal("PolygonInvalid", areaId + " polygon is not strictly convex counter-clockwise");
            }
        }
    }

    private static double Cross(double ax, double az, double bx, double bz, double px, double pz) =>
        (bx - ax) * (pz - az) - (bz - az) * (px - ax);

    /// <summary>Strictly inside a convex counter-clockwise polygon; a point exactly on an edge is outside (in no area).</summary>
    private static bool StrictlyInside(IReadOnlyList<CanopyVertex> polygon, double x, double z)
    {
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            if (Cross(a.XM, a.ZM, b.XM, b.ZM, x, z) <= 0) return false;
        }
        return true;
    }

    private static double PolygonAreaM2(IReadOnlyList<(double X, double Z)> ring)
    {
        double sum = 0.0;
        for (int i = 0; i < ring.Count; i++)
        {
            var a = ring[i];
            var b = ring[(i + 1) % ring.Count];
            sum += a.X * b.Z - b.X * a.Z;
        }
        return Math.Abs(sum) / 2.0;
    }

    private static double PolygonAreaM2(IReadOnlyList<CanopyVertex> polygon) =>
        PolygonAreaM2(polygon.Select(p => (p.XM, p.ZM)).ToArray());

    /// <summary>Area of (convex polygon ∩ axis-aligned square), by clipping the polygon against the square's four sides.</summary>
    private static double ClippedSquareArea(IReadOnlyList<CanopyVertex> polygon, double x0, double z0, double x1, double z1)
    {
        var ring = polygon.Select(p => (p.XM, p.ZM)).ToList();
        ring = Clip(ring, p => p.Item1 - x0, (a, b) => Lerp(a, b, (x0 - a.Item1) / (b.Item1 - a.Item1)));
        ring = Clip(ring, p => x1 - p.Item1, (a, b) => Lerp(a, b, (x1 - a.Item1) / (b.Item1 - a.Item1)));
        ring = Clip(ring, p => p.Item2 - z0, (a, b) => Lerp(a, b, (z0 - a.Item2) / (b.Item2 - a.Item2)));
        ring = Clip(ring, p => z1 - p.Item2, (a, b) => Lerp(a, b, (z1 - a.Item2) / (b.Item2 - a.Item2)));
        return ring.Count < 3 ? 0.0 : PolygonAreaM2(ring);
    }

    private static (double, double) Lerp((double, double) a, (double, double) b, double t) =>
        (a.Item1 + (b.Item1 - a.Item1) * t, a.Item2 + (b.Item2 - a.Item2) * t);

    // Sutherland–Hodgman against one half-plane: keep points with side(p) >= 0.
    private static List<(double, double)> Clip(List<(double, double)> ring, Func<(double, double), double> side,
        Func<(double, double), (double, double), (double, double)> intersect)
    {
        var result = new List<(double, double)>();
        for (int i = 0; i < ring.Count; i++)
        {
            var current = ring[i];
            var previous = ring[(i + ring.Count - 1) % ring.Count];
            bool currentIn = side(current) >= 0, previousIn = side(previous) >= 0;
            if (currentIn)
            {
                if (!previousIn) result.Add(intersect(previous, current));
                result.Add(current);
            }
            else if (previousIn) result.Add(intersect(previous, current));
        }
        return result;
    }

    private static (double MinX, double MinZ, double MaxX, double MaxZ) Bounds(IReadOnlyList<CanopyVertex> polygon) =>
        (polygon.Min(p => p.XM), polygon.Min(p => p.ZM), polygon.Max(p => p.XM), polygon.Max(p => p.ZM));
}
