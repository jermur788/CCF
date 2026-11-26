using System.Globalization;

namespace SitkaP1.Core;

/// <summary>
/// Deterministic 0.64 ha calibration stand fixture (80 × 80 m, 5 extraction lanes MA-L1..MA-L5, exact 1,344 stems =
/// 2,100 stems/ha) for Phase 2B Economy & D-062 verification [GAMEPLAY CALIBRATION].
/// Uses integer-only SplitMix64 (independent of simulation RNG; no RngSalts key).
/// </summary>
internal static class EconomyCalibrationFixture
{
    internal const string ExperimentId = "mad-calib-064ha-5lane-1344-v1";
    internal const string GeneratorId = "ccf-2d-calib-064ha-generator";
    internal const int GeneratorVersion = 1;
    internal const long GeneratorSeed = 20_261_126L;

    internal const long StandSideMm = 80_000L;      // 80 m × 80 m = 6,400 m² = 0.64 ha
    internal const int LaneCount = 5;               // 5 lanes of 16 m × 80 m = 1,280 m² (0.128 ha) each
    internal const long LaneWidthMm = 16_000L;
    internal const int ExpectedTotalStems = 1_344;  // 269 × 4 + 268 = 1,344 stems (2,100 stems/ha)

    internal const string Disclosure =
        "SYNTHETIC 0.64 HA CALIBRATION STAND (80x80 M, 5 LANES, 1,344 STEMS, EUR 12,000 OPENING CASH). " +
        "GAMEPLAY CALIBRATION FIXTURE - NOT A SURVEYED IRISH FOREST OR FIELD FORECAST.";

    /// <summary>
    /// Builds the deterministic 80 × 80 m / 5-lane / 1,344-stem experiment. Two calls produce byte-identical manifests.
    /// </summary>
    internal static MultiAreaExperiment Create()
    {
        var areas = new MultiAreaDefinition[LaneCount];
        // Grid per 16 m × 80 m lane: 7 columns × 39 rows = 273 slots; lanes 0..3 take 269 stems, lane 4 takes 268 stems -> 1,344 total.
        const int cols = 7;
        const int rows = 39;
        const long marginMm = 800L;
        long spanX = LaneWidthMm - 2 * marginMm; // 14,400 mm across 6 intervals = 2,400 mm pitch
        long spanY = StandSideMm - 2 * marginMm; // 78,400 mm across 38 intervals ≈ 2,063 mm pitch

        for (int lane = 0; lane < LaneCount; lane++)
        {
            string areaId = "MA-L" + (lane + 1).ToString(CultureInfo.InvariantCulture);
            long x0 = lane * LaneWidthMm;
            long x1 = x0 + LaneWidthMm;
            var polygon = Array.AsReadOnly(new[]
            {
                new MillimetrePoint(x0, 0L),
                new MillimetrePoint(x1, 0L),
                new MillimetrePoint(x1, StandSideMm),
                new MillimetrePoint(x0, StandSideMm),
            });

            int targetCount = lane < LaneCount - 1 ? 269 : 268;
            var trees = new List<MultiAreaTreeDefinition>(targetCount);
            int idx = 0;
            for (int r = 0; r < rows && trees.Count < targetCount; r++)
            {
                for (int c = 0; c < cols && trees.Count < targetCount; c++)
                {
                    long baseX = x0 + marginMm + spanX * c / (cols - 1);
                    long baseY = marginMm + spanY * r / (rows - 1);
                    long jx = Jitter(lane, idx, 1, 250L);
                    long jy = Jitter(lane, idx, 2, 250L);
                    long x = Math.Clamp(baseX + jx, x0 + 400L, x1 - 400L);
                    long y = Math.Clamp(baseY + jy, 400L, StandSideMm - 400L);

                    // Mature-enough calibration stems (DBH 200..340 mm, height 17..26 m, age 28) for thinning & D-062 tests.
                    long u = (long)(Next(lane, idx, 3) % 141UL);
                    long dbhMm = 200L + u;
                    long heightMm = 17_000L + (dbhMm - 200L) * 60L;
                    string treeId = areaId + "-" + (trees.Count + 1).ToString("D3", CultureInfo.InvariantCulture);
                    trees.Add(new MultiAreaTreeDefinition(treeId, x, y, AgeYears: 28, DbhMm: dbhMm, HeightMm: heightMm));
                    idx++;
                }
            }

            areas[lane] = new MultiAreaDefinition(
                AreaId: areaId,
                Label: "Lane " + (lane + 1).ToString(CultureInfo.InvariantCulture),
                StockingNote: "0.64 ha calibration lane (16x80 m, 0.128 ha, ~2,100 stems/ha)",
                PolygonMm: polygon,
                SimulationSeed: 261_101 + lane * 101,
                Trees: Array.AsReadOnly(trees.ToArray()));
        }

        var experiment = new MultiAreaExperiment(ExperimentId, GeneratorId, GeneratorVersion, GeneratorSeed, Array.AsReadOnly(areas))
        {
            DisclosureOverride = Disclosure,
        };
        experiment.Validate();
        return experiment;
    }

    private static long Jitter(int lane, int index, int salt, long maxAbsMm) =>
        (long)(Next(lane, index, salt) % (ulong)(2 * maxAbsMm + 1)) - maxAbsMm;

    private static ulong Next(int lane, int index, int salt)
    {
        ulong z = unchecked((ulong)GeneratorSeed * 0x9E3779B97F4A7C15UL);
        z = SplitMix(z ^ (ulong)(uint)lane);
        z = SplitMix(z ^ ((ulong)(uint)index << 16));
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
