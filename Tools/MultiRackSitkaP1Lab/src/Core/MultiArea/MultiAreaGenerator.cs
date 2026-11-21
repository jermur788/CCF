namespace SitkaP1.Core;

/// <summary>
/// Deterministic generator of the MA-1 synthetic two-area experiment <c>mad-sitka-ma1-two-area-v1</c> (generator v1).
///
/// Every value below is a SYNTHETIC teaching design choice, not a measurement, an Irish survey value or a P0/P1 fixture
/// value. The two areas are deliberately different so the player can see different competition and growth, but they are
/// NOT claimed comparable, and no site, soil, exposure or history is known (site UNKNOWN):
///   • MA-A "Area A": a convex pentagon of 915 m², one even-aged cohort (24 years), close spacing (2.24 m jittered grid),
///     smaller stems (DBH mean 222 mm). Dense, so the unchanged density rules start natural thinning early.
///   • MA-B "Area B": a convex quadrilateral of 1,500 m² that shares one boundary segment with MA-A, two cohorts (70 % aged
///     47 years with DBH mean 345 mm, 30 % aged 29 years with DBH mean 225 mm), wide spacing (4.3 m jittered grid).
///   • Positions: a jittered square grid over the polygon's bounding box, keeping only points at least the edge margin
///     inside the polygon, so trees never sit on a boundary and two trees never coincide (jitter is under half a pitch).
///   • DBH (mm) = mean + trunc((S − 2000) × spread / 1000), with S the sum of four integer uniforms in [0, 1000]
///     (Irwin–Hall shape), clamped per cohort. Height (mm) = cohort height at the mean + (DBH − mean) × slope: a linear
///     synthetic size–height relation near the source Class III envelope for that age, not a fitted curve.
///   • IDs are the area ID, a hyphen and a three-digit number in row-major order (south row first, west to east).
/// Randomness: SplitMix64 over (generator seed, area index, grid cell, salt), integer arithmetic only, so the bytes do not
/// depend on floating-point evaluation. It is not the engine's mortality RNG, and the generator seed differs from both
/// area simulation seeds.
/// </summary>
internal static class MultiAreaGenerator
{
    internal const string ExperimentId = "mad-sitka-ma1-two-area-v1";
    internal const string GeneratorId = "ma1-two-area-synthetic-generator";
    internal const int GeneratorVersion = 1;
    internal const long GeneratorSeed = 8_850_001;

    private sealed record Cohort(int Percent, int AgeYears, long MeanDbhMm, long SpreadPer1000, long MinDbhMm, long MaxDbhMm,
        long HeightAtMeanMm, long HeightPerDbhMm);

    private sealed record AreaPlan(string AreaId, string Label, string Stocking, MillimetrePoint[] Polygon, int Seed,
        long PitchMm, long JitterMm, long EdgeMarginMm, Cohort[] Cohorts);

    private static readonly AreaPlan[] Plans =
    {
        new("MA-A", "Area A",
            "synthetic dense even-aged stand: one cohort aged 24, 2.24 m jittered grid, DBH mean 222 mm",
            new MillimetrePoint[] { new(2_000, 2_000), new(36_000, 2_000), new(40_000, 18_000), new(24_000, 32_000), new(3_000, 26_000) },
            Seed: 885_101, PitchMm: 2_240, JitterMm: 450, EdgeMarginMm: 700,
            new[] { new Cohort(100, 24, 222, 75, 90, 360, 17_000, 25) }),
        new("MA-B", "Area B",
            "synthetic open two-cohort stand: 70 percent aged 47 and 30 percent aged 29, 4.3 m jittered grid",
            new MillimetrePoint[] { new(36_000, 2_000), new(90_000, 0), new(86_000, 44_000), new(40_000, 18_000) },
            Seed: 885_202, PitchMm: 4_300, JitterMm: 900, EdgeMarginMm: 900,
            new[]
            {
                new Cohort(70, 47, 345, 80, 220, 480, 27_200, 20),
                new Cohort(30, 29, 225, 60, 120, 330, 19_900, 25),
            }),
    };

    /// <summary>The one MA-1 experiment, validated. Two calls return equal values and identical manifest bytes.</summary>
    internal static MultiAreaExperiment Generate()
    {
        var areas = Plans.Select((plan, index) => GenerateArea(plan, index)).ToArray();
        var experiment = new MultiAreaExperiment(ExperimentId, GeneratorId, GeneratorVersion, GeneratorSeed, Array.AsReadOnly(areas));
        experiment.Validate();
        return experiment;
    }

    private static MultiAreaDefinition GenerateArea(AreaPlan plan, int areaIndex)
    {
        var (minX, minY, maxX, maxY) = MultiAreaGeometry.Bounds(plan.Polygon);
        var trees = new List<MultiAreaTreeDefinition>();
        int rows = checked((int)((maxY - minY) / plan.PitchMm) + 1);
        int cols = checked((int)((maxX - minX) / plan.PitchMm) + 1);
        for (int row = 0; row < rows; row++)
        for (int col = 0; col < cols; col++)
        {
            int cell = row * 1_000 + col;
            long x = minX + plan.PitchMm / 2 + col * plan.PitchMm + Jitter(areaIndex, cell, 1, plan.JitterMm);
            long y = minY + plan.PitchMm / 2 + row * plan.PitchMm + Jitter(areaIndex, cell, 2, plan.JitterMm);
            if (!MultiAreaGeometry.InsideWithMargin(plan.Polygon, new MillimetrePoint(x, y), plan.EdgeMarginMm)) continue;

            var cohort = PickCohort(plan.Cohorts, (int)(Next(areaIndex, cell, 3) % 100UL));
            long sum = 0;
            for (int k = 0; k < 4; k++) sum += (long)(Next(areaIndex, cell, 10 + k) % 1001UL);
            long dbh = Math.Clamp(cohort.MeanDbhMm + (sum - 2_000) * cohort.SpreadPer1000 / 1_000, cohort.MinDbhMm, cohort.MaxDbhMm);
            long height = cohort.HeightAtMeanMm + (dbh - cohort.MeanDbhMm) * cohort.HeightPerDbhMm;
            string id = plan.AreaId + "-" + (trees.Count + 1).ToString("D3", System.Globalization.CultureInfo.InvariantCulture);
            trees.Add(new MultiAreaTreeDefinition(id, x, y, cohort.AgeYears, dbh, height));
        }
        return new MultiAreaDefinition(plan.AreaId, plan.Label, plan.Stocking, Array.AsReadOnly(plan.Polygon.ToArray()),
            plan.Seed, trees.AsReadOnly());
    }

    private static Cohort PickCohort(Cohort[] cohorts, int percentile)
    {
        int cumulative = 0;
        foreach (var cohort in cohorts)
        {
            cumulative += cohort.Percent;
            if (percentile < cumulative) return cohort;
        }
        return cohorts[^1];
    }

    private static long Jitter(int areaIndex, int cell, int salt, long jitterMm) =>
        (long)(Next(areaIndex, cell, salt) % (ulong)(2 * jitterMm + 1)) - jitterMm;

    private static ulong Next(int areaIndex, int cell, int salt)
    {
        ulong z = unchecked((ulong)GeneratorSeed * 0x9E3779B97F4A7C15UL);
        z = SplitMix(z ^ (ulong)(uint)areaIndex);
        z = SplitMix(z ^ ((ulong)(uint)cell << 16));
        return SplitMix(z ^ ((ulong)(uint)salt << 40));
    }

    private static ulong SplitMix(ulong z)
    {
        unchecked
        {
            z += 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
    }
}
