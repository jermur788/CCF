using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder;

/// <summary>One species in a custom mix, as a percentage weight. The six entries must sum to 100.</summary>
public sealed record SpeciesMixEntry(string SpeciesId, double Percent);

/// <summary>One generated tree. Everything Part 2 needs is carried here, so it need not re-derive the stand.</summary>
public sealed record GeneratedTree(
    int Index,
    PlanPoint Position,
    string SpeciesId,
    int LaneIndex,
    SiteMoistureClass Moisture,
    bool IsEdgeSeedTree,
    int AgeYears,
    bool HabitatProtectionCaution);

/// <summary>The Year 0 stand a custom recipe produces. Immutable.</summary>
public sealed record GeneratedStand(
    IReadOnlyList<GeneratedTree> Trees,
    int PlantedStemCount,
    int EdgeSeedTreeCount,
    double StemsPerHectareAchieved,
    int TargetStemsPerHectare,
    int LivingTreeSafetyCap,
    bool NearSafetyCap)
{
    public int LivingTreeCount => Trees.Count;

    public bool HasHabitatProtectionCaution
    {
        get
        {
            foreach (GeneratedTree tree in Trees)
            {
                if (tree.HabitatProtectionCaution)
                    return true;
            }

            return false;
        }
    }
}

/// <summary>
/// Deterministic six-species stand generator. Placement is a jittered lattice inside the boundary and inside an active
/// 2.5 m cell, so every tree carries a defined moisture class. Species are assigned by exact largest-remainder counts
/// and a seeded shuffle. There is no <c>System.Random</c>, no clock and no ambient state.
///
/// Placing trees in active cells only is a deliberate simplification: a tree in an inactive edge cell would have no
/// site class, and inventing <see cref="SiteMoistureClass.NeutralLegacy"/> for it would misrepresent a new woodland.
/// </summary>
public static class CustomWoodlandStandGenerator
{
    /// <summary>Hard living-tree limit for this simulator. Calibration safety guardrail, not a carrying capacity.</summary>
    public const int LivingTreeSafetyCap = 3000;

    /// <summary>At or above this living-tree count the stand reports <see cref="GeneratedStand.NearSafetyCap"/>.</summary>
    public const int NearSafetyCapThreshold = 2500;

    public const int MinimumStartAgeYears = 10;
    public const int MaximumStartAgeYears = 60;
    public const int MinimumStockingStemsPerHectare = 500;
    public const int MaximumStockingStemsPerHectare = 3000;

    /// <summary>Maximum number of mature outer-edge seed trees a recipe may request.</summary>
    public const int MaximumEdgeSeedTreeCount = 200;

    /// <summary>Inward offset ring for edge seed trees, in metres. Calibration layout convention.</summary>
    public const double EdgeSeedTreeInsetM = 4.0;

    /// <summary>Minimum clearance from every boundary edge for an edge seed tree, in metres.</summary>
    public const double EdgeSeedTreeClearanceM = 0.5;

    /// <summary>Age of a mature edge seed tree: at least this, or the stand start age when that is older.</summary>
    public const int MinimumEdgeSeedTreeAgeYears = 60;

    private const double JitterFraction = 0.35;
    private const double MinimumLatticeSpacingM = 0.25;
    private const int MaximumSpacingShrinkAttempts = 40;
    private const double SpacingShrinkFactor = 0.94;

    private const ulong JitterSalt = 0x51A7C0DE2B4E9F31UL;
    private const ulong SpeciesShuffleSalt = 0x2C9B7E5DA1F3486BUL;
    private const ulong EdgeSpeciesShuffleSalt = 0x7D3E1A9C56B2048FUL;

    /// <summary>Planted target for a boundary and stocking, rounded half away from zero.</summary>
    public static int TargetPlantedStemCount(WoodlandPolygon polygon, int stockingStemsPerHectare)
    {
        if (polygon is null)
            throw new ArgumentNullException(nameof(polygon));

        double exact = stockingStemsPerHectare * polygon.AreaHectares;
        return (int)Math.Round(exact, MidpointRounding.AwayFromZero);
    }

    internal static GeneratedStand Generate(
        CustomWoodlandRecipe recipe,
        WoodlandPolygon polygon,
        LanePlan lanes,
        MoistureGrid grid)
    {
        int target = TargetPlantedStemCount(polygon, recipe.StockingStemsPerHectare);
        int living = target + recipe.EdgeSeedTreeCount;
        if (living > LivingTreeSafetyCap)
            throw new ArgumentException(
                $"a stand of {living} living trees ({target} planted + {recipe.EdgeSeedTreeCount} edge seed) " +
                $"exceeds the {LivingTreeSafetyCap} living-tree safety cap.",
                nameof(recipe));

        int[] counts = AllocateCounts(recipe.SpeciesMix, target);
        var speciesOrder = new List<string>(target);
        for (int i = 0; i < recipe.SpeciesMix.Count; i++)
        {
            for (int n = 0; n < counts[i]; n++)
                speciesOrder.Add(recipe.SpeciesMix[i].SpeciesId);
        }

        Shuffle(speciesOrder, recipe.Seed, SpeciesShuffleSalt);

        double spacing = Math.Sqrt(10000.0 / recipe.StockingStemsPerHectare);
        List<Candidate> candidates = BuildCandidates(polygon, grid, spacing, recipe.Seed);

        for (int attempt = 1; attempt < MaximumSpacingShrinkAttempts && candidates.Count < target; attempt++)
        {
            spacing *= SpacingShrinkFactor;
            if (spacing < MinimumLatticeSpacingM)
                break;
            candidates = BuildCandidates(polygon, grid, spacing, recipe.Seed);
        }

        if (candidates.Count < target)
            throw new ArgumentException(
                $"the field can seat only {candidates.Count} of the {target} requested stems at this stocking; " +
                "reduce the stocking or use a larger field.",
                nameof(recipe));

        var trees = new List<GeneratedTree>(target + recipe.EdgeSeedTreeCount);
        for (int i = 0; i < target; i++)
        {
            Candidate candidate = candidates[i * candidates.Count / target];
            trees.Add(new GeneratedTree(
                trees.Count,
                candidate.Point,
                speciesOrder[i],
                lanes.LaneIndexAt(candidate.Point),
                candidate.Moisture,
                IsEdgeSeedTree: false,
                recipe.StartAgeYears,
                SpeciesMoistureTable.HabitatProtectionCaution(candidate.Moisture)));
        }

        if (recipe.EdgeSeedTreeCount > 0)
            AddEdgeSeedTrees(recipe, polygon, lanes, grid, trees);

        if (trees.Count > LivingTreeSafetyCap)
            throw new ArgumentException(
                $"the generated stand has {trees.Count} living trees, above the {LivingTreeSafetyCap} safety cap.",
                nameof(recipe));

        double stemsPerHectare = target / polygon.AreaHectares;
        return new GeneratedStand(
            trees.AsReadOnly(),
            target,
            recipe.EdgeSeedTreeCount,
            stemsPerHectare,
            recipe.StockingStemsPerHectare,
            LivingTreeSafetyCap,
            trees.Count >= NearSafetyCapThreshold);
    }

    private static void AddEdgeSeedTrees(
        CustomWoodlandRecipe recipe,
        WoodlandPolygon polygon,
        LanePlan lanes,
        MoistureGrid grid,
        List<GeneratedTree> trees)
    {
        List<PlanPoint> ring = BuildEdgeSeedTreeRing(polygon, grid);
        if (ring.Count < recipe.EdgeSeedTreeCount)
            throw new ArgumentException(
                $"this boundary can seat only {ring.Count} mature edge seed trees at a " +
                $"{EdgeSeedTreeInsetM:F1} m inset, but {recipe.EdgeSeedTreeCount} were requested.",
                nameof(recipe));

        int[] counts = AllocateCounts(recipe.SpeciesMix, recipe.EdgeSeedTreeCount);
        var species = new List<string>(recipe.EdgeSeedTreeCount);
        for (int i = 0; i < recipe.SpeciesMix.Count; i++)
        {
            for (int n = 0; n < counts[i]; n++)
                species.Add(recipe.SpeciesMix[i].SpeciesId);
        }

        Shuffle(species, recipe.Seed, EdgeSpeciesShuffleSalt);
        int edgeAge = Math.Max(recipe.StartAgeYears, MinimumEdgeSeedTreeAgeYears);

        for (int i = 0; i < recipe.EdgeSeedTreeCount; i++)
        {
            PlanPoint point = ring[i * ring.Count / recipe.EdgeSeedTreeCount];
            grid.TryClassAtPoint(point, out SiteMoistureClass moisture);
            trees.Add(new GeneratedTree(
                trees.Count,
                point,
                species[i],
                lanes.LaneIndexAt(point),
                moisture,
                IsEdgeSeedTree: true,
                edgeAge,
                SpeciesMoistureTable.HabitatProtectionCaution(moisture)));
        }
    }

    /// <summary>How many mature edge seed trees this boundary can seat at the declared inset.</summary>
    public static int EdgeSeedTreeCapacity(WoodlandPolygon polygon, MoistureGrid grid)
    {
        if (polygon is null)
            throw new ArgumentNullException(nameof(polygon));
        if (grid is null)
            throw new ArgumentNullException(nameof(grid));

        return BuildEdgeSeedTreeRing(polygon, grid).Count;
    }

    private static List<PlanPoint> BuildEdgeSeedTreeRing(WoodlandPolygon polygon, MoistureGrid grid)
    {
        var ring = new List<PlanPoint>();
        IReadOnlyList<PlanPoint> vertices = polygon.Vertices;

        for (int i = 0; i < vertices.Count; i++)
        {
            PlanPoint a = vertices[i];
            PlanPoint b = vertices[(i + 1) % vertices.Count];
            double dx = b.X - a.X;
            double dz = b.Z - a.Z;
            double length = Math.Sqrt(dx * dx + dz * dz);
            if (length <= 0.0)
                continue;

            // The ring is counter-clockwise, so the left normal points into the field.
            double inwardX = -dz / length;
            double inwardZ = dx / length;
            int samples = Math.Max(1, (int)Math.Floor(length / 2.0));

            for (int sample = 1; sample <= samples; sample++)
            {
                double t = sample / (samples + 1.0);
                var point = new PlanPoint(
                    a.X + t * dx + inwardX * EdgeSeedTreeInsetM,
                    a.Z + t * dz + inwardZ * EdgeSeedTreeInsetM);

                if (!polygon.ContainsWithClearance(point, EdgeSeedTreeClearanceM))
                    continue;
                if (!grid.TryClassAtPoint(point, out _))
                    continue;
                if (IsNearExisting(ring, point, 1.0))
                    continue;

                ring.Add(point);
            }
        }

        return ring;
    }

    private static bool IsNearExisting(List<PlanPoint> points, PlanPoint candidate, double minimumDistanceM)
    {
        foreach (PlanPoint point in points)
        {
            double dx = point.X - candidate.X;
            double dz = point.Z - candidate.Z;
            if (dx * dx + dz * dz < minimumDistanceM * minimumDistanceM)
                return true;
        }

        return false;
    }

    private readonly record struct Candidate(PlanPoint Point, SiteMoistureClass Moisture);

    private static List<Candidate> BuildCandidates(
        WoodlandPolygon polygon,
        MoistureGrid grid,
        double spacing,
        ulong seed)
    {
        var candidates = new List<Candidate>();
        var random = new DeterministicRandom(seed, JitterSalt);

        double startX = polygon.MinX;
        double startZ = polygon.MinZ;
        int columns = (int)Math.Floor((polygon.WidthM + spacing) / spacing) + 1;
        int rows = (int)Math.Floor((polygon.LengthM + spacing) / spacing) + 1;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                double jitterX = (random.NextDouble() * 2.0 - 1.0) * JitterFraction * spacing;
                double jitterZ = (random.NextDouble() * 2.0 - 1.0) * JitterFraction * spacing;
                var point = new PlanPoint(
                    startX + (column + 0.5) * spacing + jitterX,
                    startZ + (row + 0.5) * spacing + jitterZ);

                if (!polygon.ContainsPoint(point))
                    continue;
                if (!grid.TryClassAtPoint(point, out SiteMoistureClass moisture))
                    continue;

                candidates.Add(new Candidate(point, moisture));
            }
        }

        return candidates;
    }

    /// <summary>
    /// Exact largest-remainder allocation over percentage weights. Ties go to the earlier entry, so the result is
    /// fully determined by the mix order. The counts always sum to <paramref name="total"/>.
    /// </summary>
    public static int[] AllocateCounts(IReadOnlyList<SpeciesMixEntry> mix, int total)
    {
        if (mix is null)
            throw new ArgumentNullException(nameof(mix));
        if (total < 0)
            throw new ArgumentOutOfRangeException(nameof(total), total, "the stem count must be non-negative.");

        var counts = new int[mix.Count];
        var remainders = new double[mix.Count];
        int assigned = 0;

        for (int i = 0; i < mix.Count; i++)
        {
            double exact = mix[i].Percent / 100.0 * total;
            double floored = Math.Floor(exact);
            counts[i] = (int)floored;
            remainders[i] = exact - floored;
            assigned += counts[i];
        }

        int remaining = total - assigned;
        if (remaining <= 0)
            return counts;

        var order = new int[mix.Count];
        for (int i = 0; i < order.Length; i++)
            order[i] = i;

        Array.Sort(order, (a, b) =>
        {
            int byRemainder = remainders[b].CompareTo(remainders[a]);
            return byRemainder != 0 ? byRemainder : a.CompareTo(b);
        });

        for (int i = 0; i < remaining; i++)
            counts[order[i % order.Length]]++;

        return counts;
    }

    private static void Shuffle(List<string> values, ulong seed, ulong salt)
    {
        var random = new DeterministicRandom(seed, salt);
        for (int i = values.Count - 1; i > 0; i--)
        {
            int j = random.NextInt(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }
    }

    /// <summary>SplitMix64 stream. Deterministic, seedable, and free of any platform-dependent randomness.</summary>
    private sealed class DeterministicRandom
    {
        private ulong _state;

        public DeterministicRandom(ulong seed, ulong salt)
        {
            _state = seed ^ salt;
        }

        public ulong NextULong()
        {
            _state += 0x9E3779B97F4A7C15UL;
            ulong z = _state;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }

        public double NextDouble() => (NextULong() >> 11) * (1.0 / 9007199254740992.0);

        public int NextInt(int exclusiveBound)
        {
            if (exclusiveBound <= 0)
                throw new ArgumentOutOfRangeException(nameof(exclusiveBound));

            return (int)(NextULong() % (ulong)exclusiveBound);
        }
    }
}
