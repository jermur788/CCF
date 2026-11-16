namespace SitkaP1.Core;

/// <summary>
/// Deterministic generator for the first P1 derived fixture <c>p1d-30m-sitka-adult-v1</c> (generator v1).
///
/// Every value below is a SYNTHETIC design choice, labelled as such in the manifest; none is measured, Irish, or taken from P0:
///   • 100 living stems on a 10 × 10 grid with 3 m pitch (first row/column at 1.5 m), each displaced by an integer jitter in
///     [−600, +600] mm on each axis, so all stems stay inside [900, 29100] mm and no two coincide.
///     100 stems / 0.09 ha ≈ 1,111 stems/ha.
///   • Even-aged: every stem is 30 years old (the source Class III height curve is anchored at 20.4 m at age 30).
///   • DBH (mm) = 240 + trunc((S − 2000) × 90 / 1000), where S is the sum of four integer uniforms in [0, 1000]
///     (Irwin–Hall shape; mean 240 mm, SD ≈ 52 mm), clamped to [100, 400] mm.
///   • Height (mm) = 20400 + (DBH − 240) × 30, i.e. 16.2–25.2 m: a linear synthetic size–height relation, not a fitted curve.
///   • Action-eligibility threshold 170 mm (a separate label only; every living stem is in the biological population).
///   • Ordinal IDs <c>P30-001</c> … <c>P30-100</c> in row-major order (south row first, west to east).
///
/// Randomness: SplitMix64 over (generatorSeed, tree index, salt), integer arithmetic only, so the bytes do not depend on
/// floating-point evaluation. This generator RNG is NOT the source mortality RNG (SimulationRandom), and generatorSeed is a
/// different number from the ecological simulationSeed.
/// </summary>
public static class ManifestGenerator
{
    public const string FixtureId = "p1d-30m-sitka-adult-v1";
    public const long GeneratorSeed = 6_390_001;
    public const long SimulationSeed = 20_261_116;
    public const int GridCount = 10;
    public const long PitchMm = 3_000;
    public const long FirstOffsetMm = 1_500;
    public const long JitterMm = 600;
    public const long AgeYears = 30;
    public const long ActionThresholdMm = 170;

    public static DerivedManifest Generate()
    {
        var m = new DerivedManifest
        {
            DerivedFixtureId = FixtureId,
            GeneratorSeed = GeneratorSeed,
            SimulationSeed = SimulationSeed,
            InitialEcologicalYear = 0,
            ActionEligibleDbhThresholdMm = ActionThresholdMm,
        };
        for (int row = 0; row < GridCount; row++)
        for (int col = 0; col < GridCount; col++)
        {
            int index = row * GridCount + col;
            long x = FirstOffsetMm + col * PitchMm + Jitter(index, 1);
            long y = FirstOffsetMm + row * PitchMm + Jitter(index, 2);
            long sum = 0;
            for (int k = 0; k < 4; k++) sum += (long)(Next(index, 10 + k) % 1001UL);
            long dbh = Math.Clamp(240 + (sum - 2000) * 90 / 1000, 100, 400);
            long height = 20_400 + (dbh - 240) * 30;
            m.Trees.Add(new DerivedTree
            {
                TreeId = "P30-" + (index + 1).ToString("D3", System.Globalization.CultureInfo.InvariantCulture),
                XMm = x,
                YMm = y,
                Status = "live",
                AgeYears = AgeYears,
                DbhMm = dbh,
                HeightMm = height,
                DbhCmBits = DerivedManifest.DbhCmBitsFor(dbh),
                HeightMBits = DerivedManifest.HeightMBitsFor(height),
                CrownRadiusMBits = DerivedManifest.CrownBitsFor(dbh),
                UnityXBits = DerivedManifest.UnityAxisBitsFor(x),
                UnityZBits = DerivedManifest.UnityAxisBitsFor(y),
            });
        }
        m.ContentHash = DerivedManifest.Sha256Hex(m.BodyBytes());
        return m;
    }

    private static long Jitter(int index, int salt) => (long)(Next(index, salt) % (ulong)(2 * JitterMm + 1)) - JitterMm;

    private static ulong Next(int index, int salt)
    {
        ulong z = unchecked((ulong)GeneratorSeed * 0x9E3779B97F4A7C15UL);
        z = SplitMix(z ^ (ulong)(uint)index);
        return SplitMix(z ^ ((ulong)(uint)salt << 32));
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
