using System.Globalization;
using SitkaP1.Core;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// S5-07 grid consistency. Defined by owner Option A and amended by the owner's final bounded-repair decision D1
/// (2026-11-23); see spec §4 and §9 Q4. The same MA-1 woodland runs on the 2.5 m and the 5.0 m grid for 20 woodland-mast
/// seeds × 30 years, in two scenarios: a year-0 managed thinning of every second stem, and no work. Metrics: cumulative
/// accepted establishment abundance and cumulative promotions.
/// <list type="bullet">
/// <item><b>S5-07A, BLOCKING.</b>
/// <list type="bullet">
/// <item>Population: the aligned 5 m blocks represented on BOTH lattices, i.e. whose 5 m centre is inside a polygon. The test
/// asserts that these are exactly the 5.0 m cells.</item>
/// <item>Units: per hectare of matched in-polygon area, from each lattice's own in-polygon cells in those blocks (6.25 m² per
/// 2.5 m cell, 25 m² per 5.0 m cell).</item>
/// <item>Light: both grids read ONE shared field, the regeneration-free adult canopy at every aligned 5 m block centre. That
/// canopy is the post-adult-step roster of a 1/1/1/0 run of the same scenario in that year.</item>
/// <item>Gate: the pooled and the mean 2.5 m / 5.0 m per-hectare ratios of both metrics lie in [0.75, 1.33] in both
/// scenarios.</item>
/// <item>Light identity is proved, not assumed: light is bit-equal at every matched block in every year, and the year loop is
/// shown to reproduce MultiAreaContinuation.Advance bit for bit under the model light.</item>
/// <item>Also: zero out-of-polygon trees in every study run, and every 5.0 m run bit-identical to the first pass.</item>
/// </list></item>
/// <item><b>S5-07B, MANDATORY NON-BLOCKING DIAGNOSTIC.</b> It prints and never asserts:
/// <list type="bullet">
/// <item>the same comparison under each grid's own model light;</item>
/// <item>the interior and edge splits under both light bases;</item>
/// <item>the blocks that exist on the 2.5 m lattice only.</item>
/// </list></item>
/// </list>
/// Every number is printed before any assertion, so a failure is fully reported, never hidden.
/// </summary>
internal static class Stage5GridStudy
{
    internal const int Seeds = 20;
    internal const int Years = 30;
    internal const double ToleranceLow = 0.75;
    internal const double ToleranceHigh = 1.33;
    private const double SquareMetresPerHectare = 10_000.0;

    /// <summary>
    /// The first-pass <see cref="Stage5Support.Write"/> fingerprint of all forty 5.0 m model runs of this study (20 seeds × 2
    /// scenarios × 30 years). It was measured on the unmodified first-pass engine cb739cbd, before any Option A edit. Option A
    /// must leave the 5.0 m grid bit-identical (owner purity criterion: a = 1.0, no edge-gate reduction).
    /// </summary>
    internal const string PinnedFiveMetrePurityHash = "c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8";

    // The kind of ground a block covers:
    // - interior: all four 2.5 m centres are in a polygon (always matched; asserted);
    // - matched edge: the 5 m centre is in a polygon, so the 5.0 m grid has a cell there;
    // - unmatched: the block exists on the 2.5 m lattice only.
    private const int Interior = 0, MatchedEdge = 1, Unmatched = 2;

    private static readonly bool[] Scenarios = { true, false };
    private static string Name(bool thin) => thin ? "managed thinning" : "no work";

    /// <summary>One run's totals: the accounts' accepted abundance, and per-cell acceptance and promotions by kind of ground.</summary>
    private sealed record RunResult(double Accepted, double[] AcceptedBy, int[] PromotionsBy, int OutOfPolygon, string StateHash)
    {
        public int Promotions => PromotionsBy.Sum();
        public double AcceptedMatched => AcceptedBy[Interior] + AcceptedBy[MatchedEdge];
        public int PromotionsMatched => PromotionsBy[Interior] + PromotionsBy[MatchedEdge];
        public double AcceptedEdge => AcceptedBy[MatchedEdge] + AcceptedBy[Unmatched];
        public int PromotionsEdge => PromotionsBy[MatchedEdge] + PromotionsBy[Unmatched];
    }

    private static readonly Dictionary<(double Grid, int Seed, bool Thin), (RunResult Result, string? LegacyHash)> ModelRuns = new();
    private static readonly Dictionary<(double Grid, int Seed, bool Thin), RunResult> SharedRuns = new();
    private static readonly Dictionary<bool, SharedField> Fields = new();
    private static LatticeFacts? latticeFacts;
    private static LatticeFacts Lattice => latticeFacts ??= new LatticeFacts();

    internal static void Register()
    {
        T.Add("S5-07A G-01 (BLOCKING): matched-area population (blocks represented on both lattices, asserted 1:1 with the 5.0 m cells); identical shared light field; 20 seeds × 30 years; managed thinning and no work. Pooled AND mean 2.5 m / 5.0 m ratios per hectare of matched in-polygon area, for accepted abundance and promotions, in [0.75, 1.33]; light proved identical every year; zero out-of-polygon trees; 5.0 m bit-identical to the first pass", () =>
        {
            var lattice = Lattice;
            lattice.Verify();

            // (1) The harness year loop is Advance: under the model light it reproduces a real session bit for bit, on both grids.
            foreach (var (grid, seed, thin) in new[] { (2.5, 1, true), (5.0, 1, false) })
            {
                var h = new Harness(StartSession(grid, seed, thin));
                for (int y = 0; y < Years; y++) h.Step(null);
                T.Eq(ModelRun(grid, seed, thin).Result.StateHash, StateHash(h.States, h.Stage5),
                    $"the S5-07A year loop reproduces MultiAreaContinuation.Advance bit for bit at {grid} m (rosters and whole Stage 5 state, 30 years)");
            }
            Console.WriteLine("  S5-07A harness: the year loop equals Advance under the model light (2.5 m managed thinning seed 1, 5.0 m no work seed 1; 30 years)");

            // (2) Owner purity criterion: every 5.0 m model run of the study is bit-identical to the first-pass engine.
            var purity = new Model0FingerprintTests.Fingerprint();
            foreach (bool thin in Scenarios)
                for (int seed = 1; seed <= Seeds; seed++)
                    purity.Text((thin ? "thin" : "none") + " " + seed + " " + ModelRun(5.0, seed, thin).LegacyHash);
            string purityHash = purity.Hash();
            Console.WriteLine("  S5-07A 5.0 m purity fingerprint " + purityHash + " (pinned, measured at cb739cbd: " + PinnedFiveMetrePurityHash + ")");

            // (3) The shared-light study over the matched-area population.
            var violations = new List<string>();
            int outOfPolygon = 0, coincidentChecks = 0;
            foreach (bool thin in Scenarios)
            {
                string scenario = Name(thin);
                var field = FieldFor(thin);
                T.True(field.YearZeroRoster.SequenceEqual(StartSession(2.5, 1, thin).ReadView().Areas.SelectMany(a => a.Trees)),
                    scenario + ": the 1/1/1/0 reference starts from the same year-0 roster as the Stage 5 runs");
                var r25 = new List<RunResult>();
                var r50 = new List<RunResult>();
                for (int seed = 1; seed <= Seeds; seed++)
                {
                    var (a25, light25) = SharedRun(2.5, seed, thin, field);
                    var (a50, light50) = SharedRun(5.0, seed, thin, field);
                    int coincident = 0, mismatched = 0;
                    for (int y = 1; y <= Years; y++)
                        foreach (var (block, bits) in light50[y])
                            if (light25[y].TryGetValue(block, out uint other))
                            {
                                coincident++;
                                if (other != bits) mismatched++;
                            }
                    T.Eq(0, mismatched, $"{scenario} seed {seed}: both grids read bit-equal light at every coincident block centre in every year");
                    T.Eq(Years * lattice.MatchedBlocks.Count, coincident,
                        $"{scenario} seed {seed}: every matched block was compared in every year (the whole gated population reads identical light)");
                    T.True(a50.PromotionsBy[Unmatched] == 0 && a50.AcceptedBy[Unmatched] == 0.0,
                        $"{scenario} seed {seed}: the 5.0 m run has no unmatched ground (all of it is in the gated population)");
                    coincidentChecks += coincident;
                    outOfPolygon += a25.OutOfPolygon + a50.OutOfPolygon;
                    r25.Add(a25);
                    r50.Add(a50);
                    PrintSeed("S5-07A", scenario, seed, a25, a50, lattice);
                }
                Report("S5-07A whole stand (information only, NOT gated since owner decision D1)", scenario, "accepted abundance",
                    r25.Select(r => r.Accepted).ToList(), r50.Select(r => r.Accepted).ToList());
                Report("S5-07A whole stand (information only, NOT gated since owner decision D1)", scenario, "promotions",
                    r25.Select(r => (double)r.Promotions).ToList(), r50.Select(r => (double)r.Promotions).ToList());
                AreaReport("S5-07A GATE", scenario, "accepted abundance", r25.Select(r => r.AcceptedMatched).ToList(),
                    r50.Select(r => r.AcceptedMatched).ToList(), lattice, violations);
                AreaReport("S5-07A GATE", scenario, "promotions", r25.Select(r => (double)r.PromotionsMatched).ToList(),
                    r50.Select(r => (double)r.PromotionsMatched).ToList(), lattice, violations);
                PrintSplit("S5-07A shared field", scenario, r25, r50, lattice);
            }
            Console.WriteLine($"  S5-07A light identity: {coincidentChecks} coincident block-year comparisons (every matched block, every year, every run), all bit-equal");

            // (4) Zero out-of-polygon trees over every study run: the shared-light runs above and every model run (also used by S5-07B).
            foreach (bool thin in Scenarios)
                for (int seed = 1; seed <= Seeds; seed++)
                    outOfPolygon += ModelRun(2.5, seed, thin).Result.OutOfPolygon + ModelRun(5.0, seed, thin).Result.OutOfPolygon;

            T.Eq(PinnedFiveMetrePurityHash, purityHash, "5.0 m: Option A leaves every 5.0 m study run bit-identical to the first-pass engine (a = 1.0, no edge-gate reduction)");
            T.Eq(0, outOfPolygon, "zero out-of-polygon trees in every study run (exact)");
            if (violations.Count > 0)
                throw new Exception("S5-07A tolerance [0.75, 1.33] NOT met on the matched-area population (" + violations.Count + " of 8 pooled/mean checks): " + string.Join("; ", violations));
        });

        T.Add("S5-07B G-02 (MANDATORY DIAGNOSTIC, NON-BLOCKING): each grid's own model light, same seeds, scenarios and horizon. Discloses real-light ratios (whole stand and matched area), interior/edge sub-ratios under both light bases, per-seed spread, and the blocks on the 2.5 m lattice only; no threshold and no assertion", () =>
        {
            var lattice = Lattice;
            lattice.Print();
            foreach (bool thin in Scenarios)
            {
                string scenario = Name(thin);
                var m25 = new List<RunResult>();
                var m50 = new List<RunResult>();
                for (int seed = 1; seed <= Seeds; seed++)
                {
                    m25.Add(ModelRun(2.5, seed, thin).Result);
                    m50.Add(ModelRun(5.0, seed, thin).Result);
                    PrintSeed("S5-07B model light", scenario, seed, m25[^1], m50[^1], lattice);
                }
                Report("S5-07B model light, whole stand", scenario, "accepted abundance", m25.Select(r => r.Accepted).ToList(), m50.Select(r => r.Accepted).ToList());
                Report("S5-07B model light, whole stand", scenario, "promotions", m25.Select(r => (double)r.Promotions).ToList(), m50.Select(r => (double)r.Promotions).ToList());
                AreaReport("S5-07B model light, matched area (not gated)", scenario, "accepted abundance", m25.Select(r => r.AcceptedMatched).ToList(),
                    m50.Select(r => r.AcceptedMatched).ToList(), lattice, null);
                AreaReport("S5-07B model light, matched area (not gated)", scenario, "promotions", m25.Select(r => (double)r.PromotionsMatched).ToList(),
                    m50.Select(r => (double)r.PromotionsMatched).ToList(), lattice, null);
                PrintSplit("S5-07B model light", scenario, m25, m50, lattice);

                var s25 = new List<RunResult>();
                var s50 = new List<RunResult>();
                for (int seed = 1; seed <= Seeds; seed++)
                {
                    s25.Add(SharedResult(2.5, seed, thin));
                    s50.Add(SharedResult(5.0, seed, thin));
                }
                PrintSplit("S5-07B shared field", scenario, s25, s50, lattice);
                PrintUnmatched("model light", scenario, m25, lattice);
                PrintUnmatched("shared field", scenario, s25, lattice);
            }
            Console.WriteLine("  S5-07B NOTE (boundary ground): the whole-stand 2.5 m figures exceed the 5.0 m figures partly because the finer " +
                $"lattice represents boundary ground the coarser lattice cannot. {lattice.UnmatchedBlocks.Count} aligned 5 m blocks hold in-polygon " +
                "2.5 m cells, but their 5 m centre lies outside every polygon, so the 5.0 m grid has no cell there. Their promotions and " +
                "abundance (UNMATCHED lines above) count in the 2.5 m whole stand with nothing on the 5.0 m side to compare against. That is " +
                "why S5-07A gates the matched-area population, per hectare (owner decision D1, 2026-11-23).");
            Console.WriteLine("  S5-07B NOTE (light): light is the modelled index unity-k10a10-decoupled-v1, NOT calibrated PAR. Under the model " +
                "light each grid samples its own canopy (including its own promoted adults) at its own cell centres, so the 2.5 m grid " +
                "resolves gaps more finely. A higher 2.5 m real-light recruitment reflects that finer gap-resolution sampling, not a scaling " +
                "defect; the scaling policy itself is gated by S5-07A. This diagnostic has no threshold and cannot fail the suite.");
        });
    }

    // ------------------------------------------------------------------------------------------------ runs

    private static MultiAreaContinuation StartSession(double grid, int seed, bool thin)
    {
        var c = Session(null, new Stage5Options { GridCellM = grid, MastSeed = seed });
        if (thin) ThinEverySecond(c);
        return c;
    }

    /// <summary>A real 1/1/1/1 session under its own model light (memoised: S5-07A and S5-07B share these runs).</summary>
    private static (RunResult Result, string? LegacyHash) ModelRun(double grid, int seed, bool thin)
    {
        if (ModelRuns.TryGetValue((grid, seed, thin), out var cached)) return cached;
        var c = StartSession(grid, seed, thin);
        for (int y = 0; y < Years; y++) Advance(c);
        var view = c.ReadView();
        string? legacy = null;
        if (grid == 5.0)
        {
            var w = new Model0FingerprintTests.Fingerprint();
            foreach (var area in view.Areas) w.Trees(area.Trees);
            Write(w, c.ReadStage5View());
            legacy = w.Hash();
        }
        var root = Root(c);
        var result = Summarise(root.Stage5!, root.Areas.Select(a => a.AreaId).ToArray(), root.Areas.Select(a => a.State).ToArray(), view);
        return ModelRuns[(grid, seed, thin)] = (result, legacy);
    }

    private static SharedField FieldFor(bool thin) => Fields.TryGetValue(thin, out var field) ? field : Fields[thin] = new SharedField(thin);

    /// <summary>A shared-field run's totals (memoised; S5-07B reuses the S5-07A runs, or makes them when run on its own).</summary>
    private static RunResult SharedResult(double grid, int seed, bool thin) =>
        SharedRuns.TryGetValue((grid, seed, thin), out var cached) ? cached : SharedRun(grid, seed, thin, FieldFor(thin)).Result;

    /// <summary>The same session under the S5-07A shared field, through the harness; also returns each year's light per block.</summary>
    private static (RunResult Result, Dictionary<(int X, int Z), uint>[] LightByYear) SharedRun(double grid, int seed, bool thin, SharedField field)
    {
        var c = StartSession(grid, seed, thin);
        var view = c.ReadView();
        var h = new Harness(c);
        var lightByYear = new Dictionary<(int X, int Z), uint>[Years + 1];
        for (int y = 1; y <= Years; y++)
        {
            h.Step(field.For);
            // The light each cell's rules used this year (Stage5Cell.Light): one value per block, read back from the state.
            var blocks = new Dictionary<(int X, int Z), uint>();
            foreach (var cell in h.Stage5.Areas.SelectMany(a => a.Cells))
            {
                var key = Stage5Promotion.BlockOf(cell.Column, cell.Row, grid);
                uint bits = F32.Bits(cell.Light);
                if (!blocks.TryAdd(key, bits) && blocks[key] != bits)
                    throw new Exception($"S5-07A: cells of block {key} read different light in year {y}");
            }
            lightByYear[y] = blocks;
        }
        var result = Summarise(h.Stage5, h.AreaIds, h.States, view);
        SharedRuns[(grid, seed, thin)] = result;
        return (result, lightByYear);
    }

    // Totals of one finished run; areaIds[i] names the rack of states[i].
    private static RunResult Summarise(MultiAreaStage5State s5, IReadOnlyList<string> areaIds, IReadOnlyList<AnnualState> states,
        MultiAreaSessionView view)
    {
        double grid = s5.Options.GridCellM;
        var lattice = Lattice;
        var polygons = view.Areas.ToDictionary(a => a.AreaId, a => a.Polygon, StringComparer.Ordinal);
        var groundOfCell = s5.Areas.ToDictionary(a => a.AreaId,
            a => a.Cells.Select(cell => lattice.GroundOf(Stage5Promotion.BlockOf(cell.Column, cell.Row, grid))).ToArray(), StringComparer.Ordinal);
        double accepted = 0;
        var acceptedBy = new double[3];
        var promotionsBy = new int[3];
        int outside = 0;
        foreach (var year in s5.Accounts)
            foreach (var account in year.Areas)
            {
                accepted += account.EstablishmentAccepted;
                var ground = groundOfCell[account.AreaId];
                for (int k = 0; k < ground.Length; k++) acceptedBy[ground[k]] += account.CellEstablishmentAccepted[k];
                foreach (var p in account.Promotions)
                {
                    promotionsBy[lattice.GroundOf((p.BlockX, p.BlockZ))]++;
                    if (!Stage5Tests.InsidePolygon(polygons[p.AreaId], p.XM, p.ZM)) outside++;
                }
            }
        for (int i = 0; i < states.Count; i++)
            outside += states[i].Trees.Count(t => !Stage5Tests.InsidePolygon(polygons[areaIds[i]], t.XM, t.ZM));
        return new RunResult(accepted, acceptedBy, promotionsBy, outside, StateHash(states, s5));
    }

    private static string StateHash(IReadOnlyList<AnnualState> states, MultiAreaStage5State s5)
    {
        var w = new Model0FingerprintTests.Fingerprint();
        foreach (var state in states) w.State(state);
        WriteState(w, s5);
        return w.Hash();
    }

    // ------------------------------------------------------------------------------------------------ the S5-07A population

    /// <summary>
    /// The S5-07 block populations of MA-1 and their in-polygon areas, read from the year-0 lattices of both grids. A block is
    /// <b>matched</b> when its 5 m centre is strictly inside an area polygon: that is where the 5.0 m grid, under the lattices'
    /// own centre-in-polygon rule, has its cell. <see cref="Verify"/> asserts that these blocks are exactly the 5.0 m cells.
    /// </summary>
    private sealed class LatticeFacts
    {
        internal readonly IReadOnlyList<Stage5BlockCredit> Fine, Coarse;
        internal readonly HashSet<(int X, int Z)> InteriorBlocks, MatchedBlocks, UnmatchedBlocks;
        private readonly Stage5Cell[] fineCells, coarseCells;

        internal LatticeFacts()
        {
            var fine = Session(null, new Stage5Options { GridCellM = 2.5 });
            var coarse = Session(null, new Stage5Options { GridCellM = 5.0 });
            var polygons = fine.ReadView().Areas.Select(a => a.Polygon).ToArray();
            Fine = fine.ReadStage5View().Blocks;
            Coarse = coarse.ReadStage5View().Blocks;
            fineCells = fine.ReadStage5View().Areas.SelectMany(a => a.Cells).ToArray();
            coarseCells = coarse.ReadStage5View().Areas.SelectMany(a => a.Cells).ToArray();
            InteriorBlocks = Fine.Where(b => b.InPolygonCells == Stage5Promotion.CellsPerBlock(2.5)).Select(Key).ToHashSet();
            MatchedBlocks = Fine.Where(b => polygons.Any(p => Stage5Tests.InsidePolygon(p, Centre(b.BlockX), Centre(b.BlockZ))))
                .Select(Key).ToHashSet();
            UnmatchedBlocks = Fine.Select(Key).Where(block => !MatchedBlocks.Contains(block)).ToHashSet();
        }

        private static (int X, int Z) Key(Stage5BlockCredit b) => (b.BlockX, b.BlockZ);

        /// <summary>The 5 m block centre on one axis, in the lattices' own float arithmetic ((index + 0.5) · 5).</summary>
        private static float Centre(int block) => (float)((block + 0.5) * Stage5Constants.PromotionBlockM);

        internal int GroundOf((int X, int Z) block) =>
            InteriorBlocks.Contains(block) ? Interior : MatchedBlocks.Contains(block) ? MatchedEdge : Unmatched;

        /// <summary>One lattice's in-polygon area (m²) in a set of blocks: its own in-polygon cells there × the cell area.</summary>
        internal double AreaM2(double grid, Func<(int X, int Z), bool> inSet) =>
            (grid == 2.5 ? Fine : Coarse).Where(b => inSet(Key(b))).Sum(b => b.InPolygonCells) * grid * grid;

        internal double FineMatchedM2 => AreaM2(2.5, MatchedBlocks.Contains);
        internal double CoarseMatchedM2 => AreaM2(5.0, MatchedBlocks.Contains);

        internal int FineCells(Func<(int X, int Z), bool> inSet) => Fine.Where(b => inSet(Key(b))).Sum(b => b.InPolygonCells);

        // The clipped polygon area of a lattice's cells in a set of blocks (information only; the gate uses whole cells).
        private double ClippedM2(double grid, Func<(int X, int Z), bool> inSet) =>
            (grid == 2.5 ? fineCells : coarseCells).Where(c => inSet(Stage5Promotion.BlockOf(c.Column, c.Row, grid))).Sum(c => c.ClippedAreaM2);

        private string ByCells(IEnumerable<(int X, int Z)> blocks)
        {
            var n = blocks.Select(block => Fine.Single(b => Key(b) == block).InPolygonCells).ToArray();
            return $"by in-polygon 2.5 m cells 4 → {n.Count(x => x == 4)}, 3 → {n.Count(x => x == 3)}, 2 → {n.Count(x => x == 2)}, 1 → {n.Count(x => x == 1)}";
        }

        internal void Print()
        {
            var matchedEdge = MatchedBlocks.Where(block => !InteriorBlocks.Contains(block)).ToArray();
            Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
                $"  S5-07 lattice: 2.5 m {fineCells.Length} cells in {Fine.Count} blocks; 5.0 m {coarseCells.Length} cells in {Coarse.Count} blocks."));
            Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
                $"  S5-07 MATCHED population (5 m centre inside a polygon = the 5.0 m cells): {MatchedBlocks.Count} blocks = {InteriorBlocks.Count} interior + " +
                $"{matchedEdge.Length} edge (edge {ByCells(matchedEdge)}). In-polygon area: 2.5 m {FineCells(MatchedBlocks.Contains)} cells × 6.25 m² = " +
                $"{FineMatchedM2:F2} m² ({FineMatchedM2 / SquareMetresPerHectare:F4} ha); 5.0 m {Coarse.Where(b => MatchedBlocks.Contains(Key(b))).Sum(b => b.InPolygonCells)} " +
                $"cells × 25 m² = {CoarseMatchedM2:F2} m² ({CoarseMatchedM2 / SquareMetresPerHectare:F4} ha). For information, the clipped polygon area of those " +
                $"cells: 2.5 m {ClippedM2(2.5, MatchedBlocks.Contains):F2} m², 5.0 m {ClippedM2(5.0, MatchedBlocks.Contains):F2} m²."));
            Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
                $"  S5-07 UNMATCHED blocks (2.5 m lattice only; their 5 m centre is outside every polygon): {UnmatchedBlocks.Count} blocks ({ByCells(UnmatchedBlocks)}), " +
                $"{FineCells(UnmatchedBlocks.Contains)} cells = {AreaM2(2.5, UnmatchedBlocks.Contains):F2} m² in-polygon 2.5 m area " +
                $"(clipped {ClippedM2(2.5, UnmatchedBlocks.Contains):F2} m²); the 5.0 m grid has no cell there."));
        }

        /// <summary>S5-07A population checks: the matched blocks are exactly the 5.0 m cells, one to one, and both areas are positive.</summary>
        internal void Verify()
        {
            Print();
            T.True(Coarse.All(b => b.InPolygonCells == 1), "1:1: every 5.0 m block holds exactly one 5.0 m cell");
            T.Eq(Coarse.Count, coarseCells.Length, "1:1: as many 5.0 m blocks as 5.0 m cells");
            T.True(coarseCells.All(c => c.CentreXM == Centre(Stage5Promotion.BlockOf(c.Column, c.Row, 5.0).X) &&
                c.CentreZM == Centre(Stage5Promotion.BlockOf(c.Column, c.Row, 5.0).Z)), "every 5.0 m cell is centred on its 5 m block centre");
            T.True(MatchedBlocks.SetEquals(Coarse.Select(Key)),
                "1:1: the 2.5 m blocks whose 5 m centre is inside a polygon are exactly the blocks of the 5.0 m cells (and each holds a 2.5 m cell)");
            T.Eq(101, MatchedBlocks.Count, "MA-1: 101 matched blocks, one per 5.0 m cell");
            T.True(InteriorBlocks.IsSubsetOf(MatchedBlocks), "every interior block is matched");
            T.True(UnmatchedBlocks.Count + MatchedBlocks.Count == Fine.Count, "every 2.5 m block is either matched or unmatched");
            T.True(FineMatchedM2 > 0 && CoarseMatchedM2 > 0, $"both matched in-polygon areas are positive (2.5 m {FineMatchedM2} m², 5.0 m {CoarseMatchedM2} m²)");
        }
    }

    // ------------------------------------------------------------------------------------------------ the S5-07A control

    /// <summary>
    /// The S5-07A year loop. It repeats MultiAreaContinuation.Advance for a 1/1/1/1 session with no work orders, calling the
    /// same production functions in the same order:
    /// <list type="number">
    /// <item>MultiAreaAnnual.StepStage4 for every rack with a living stem on the start-of-year woodland (an empty rack is carried);</item>
    /// <item>Stage5Annual.Step on the post-adult-step rosters;</item>
    /// <item>the promoted adults joined in ordinal TreeId order.</item>
    /// </list>
    /// The only difference it can introduce is the light field handed to Stage 5. S5-07A asserts that it reproduces Advance bit
    /// for bit when that field is the model light (null).
    /// </summary>
    private sealed class Harness
    {
        private readonly (string Id, int Seed, float AreaHa, IReadOnlyList<MultiAreaVertex> Polygon)[] areas;
        private readonly MultiAreaStage4State stage4;
        internal readonly string[] AreaIds;
        internal AnnualState[] States;
        internal MultiAreaStage5State Stage5;
        private int year;

        internal Harness(MultiAreaContinuation c)
        {
            var root = Root(c);
            T.True(!root.HasUnexecutedWork, "the S5-07A loop starts from a root with no pending work");
            var view = c.ReadView();
            areas = root.Areas.Select(a =>
            {
                var v = view.Areas.Single(x => x.AreaId == a.AreaId);
                return (Id: a.AreaId, Seed: v.SimulationSeed, AreaHa: v.AreaHa, Polygon: v.Polygon);
            }).ToArray();
            AreaIds = root.Areas.Select(a => a.AreaId).ToArray();
            stage4 = root.Stage4!;
            States = root.Areas.Select(a => a.State).ToArray();
            Stage5 = root.Stage5!;
            year = root.EcologicalYear;
        }

        internal void Step(Func<int, MultiAreaStage5State, IReadOnlyDictionary<string, IReadOnlyList<CanopyLightCell>>>? sharedLight)
        {
            var woodland = CrownModel.Woodland(States.SelectMany(state => state.Trees));
            var next = new AnnualState[States.Length];
            for (int i = 0; i < States.Length; i++)
                next[i] = States[i].Trees.Any(tree => tree.IsLiving)
                    ? MultiAreaAnnual.StepStage4(States[i], areas[i].Seed, areas[i].AreaHa, woodland, id => stage4.PruningOf(id).Lifts,
                        Stage4Options.Default).Next
                    : new AnnualState(year + 1, States[i].Trees);
            var inputs = areas.Select((a, i) => new Stage5AreaInput(a.Id, a.Seed, a.Polygon, a.AreaHa, next[i].Trees)).ToArray();
            var stepped = sharedLight is null
                ? Stage5Annual.Step(Stage5, inputs, year + 1)
                : Stage5Annual.Step(Stage5, inputs, year + 1, sharedLight(year + 1, Stage5));
            for (int i = 0; i < next.Length; i++)
            {
                var promoted = stepped.PromotedTrees[areas[i].Id];
                if (promoted.Count == 0) continue;
                next[i] = new AnnualState(year + 1,
                    Array.AsReadOnly(next[i].Trees.Concat(promoted).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray()));
            }
            States = next;
            Stage5 = stepped.Next;
            year++;
        }
    }

    /// <summary>
    /// The S5-07A shared light field of one scenario: the regeneration-free adult canopy. Each year it is the post-adult-step
    /// roster of a 1/1/1/0 run of the same woodland and scenario, read with the unchanged CanopyLightEvaluator kernel at the
    /// centre of each aligned 5 m block. Every cell of both grids reads its block's value (a 5.0 m cell's centre is its block
    /// centre), so both grids see the same numbers at every coincident point.
    /// <para>Excluded in both grids alike: the shading of Stage 5 promoted trees on the regeneration layer. Each run keeps its
    /// own adult step, seed rain and promoted adults.</para>
    /// </summary>
    private sealed class SharedField
    {
        private readonly AnnualTree[][] woodlandByYear = new AnnualTree[Years + 1][];
        private readonly Dictionary<(int Year, int X, int Z), float> light = new();
        internal readonly AnnualTree[] YearZeroRoster;

        internal SharedField(bool thin)
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Stage4);
            if (thin) ThinEverySecond(c);
            YearZeroRoster = c.ReadView().Areas.SelectMany(a => a.Trees).ToArray();
            for (int y = 1; y <= Years; y++)
            {
                Advance(c);
                woodlandByYear[y] = CrownModel.Woodland(c.ReadView().Areas.SelectMany(a => a.Trees));
            }
        }

        private float At(int year, int blockX, int blockZ)
        {
            if (light.TryGetValue((year, blockX, blockZ), out float value)) return value;
            float x = (float)((blockX + 0.5) * Stage5Constants.PromotionBlockM);
            float z = (float)((blockZ + 0.5) * Stage5Constants.PromotionBlockM);
            return light[(year, blockX, blockZ)] = CanopyLightEvaluator.LightIndexAt(x, z, woodlandByYear[year]);
        }

        internal IReadOnlyDictionary<string, IReadOnlyList<CanopyLightCell>> For(int year, MultiAreaStage5State state) =>
            state.Areas.ToDictionary(area => area.AreaId, area => (IReadOnlyList<CanopyLightCell>)Array.AsReadOnly(area.Cells.Select(cell =>
            {
                var (blockX, blockZ) = Stage5Promotion.BlockOf(cell.Column, cell.Row, state.Options.GridCellM);
                return new CanopyLightCell(cell.Column, cell.Row, cell.CentreXM, cell.CentreZM, cell.ClippedAreaM2, At(year, blockX, blockZ), 0);
            }).ToArray()), StringComparer.Ordinal);
    }

    // ------------------------------------------------------------------------------------------------ reporting

    private static double Ratio(double a, double b) => b == 0 ? (a == 0 ? 1.0 : double.PositiveInfinity) : a / b;

    private static (double Min, double Median, double Max, double Mean) Spread(List<double> ratios)
    {
        var sorted = ratios.OrderBy(r => r).ToArray();
        int n = sorted.Length;
        double median = n % 2 == 1 ? sorted[n / 2] : (sorted[n / 2 - 1] + sorted[n / 2]) / 2.0;
        return (sorted[0], median, sorted[^1], ratios.Average());
    }

    private static bool Inside(double r) => r >= ToleranceLow && r <= ToleranceHigh;

    // Prints one raw-total metric (pooled, mean and per-seed spread). Never gates.
    private static void Report(string part, string scenario, string metric, List<double> fine, List<double> coarse)
    {
        var ratios = fine.Zip(coarse, Ratio).ToList();
        double pooled = Ratio(fine.Sum(), coarse.Sum());
        var (min, median, max, mean) = Spread(ratios);
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {part} SUMMARY {scenario} | {metric}: pooled {pooled:F3} ({fine.Sum():F1} / {coarse.Sum():F1}), mean {mean:F3}; per seed min {min:F3} median {median:F3} max {max:F3}; {ratios.Count(r => !Inside(r))}/{ratios.Count} seeds outside [0.75, 1.33] (informational)"));
    }

    /// <summary>
    /// One metric over the matched-area population, per hectare of each lattice's own matched in-polygon area (spec §9 Q4, D1):
    /// x_2.5,s / A_2.5 against x_5.0,s / A_5.0. Pooled = (Σ_s x_2.5,s / A_2.5) / (Σ_s x_5.0,s / A_5.0); mean = the mean over
    /// seeds of the per-seed ratio. With <paramref name="violations"/> given, both must lie in [0.75, 1.33] (the S5-07A gate);
    /// without, the line is a diagnostic only.
    /// </summary>
    private static void AreaReport(string part, string scenario, string metric, List<double> fine, List<double> coarse, LatticeFacts lattice,
        List<string>? violations)
    {
        double fineHa = lattice.FineMatchedM2 / SquareMetresPerHectare;
        double coarseHa = lattice.CoarseMatchedM2 / SquareMetresPerHectare;
        var finePerHa = fine.Select(x => x / fineHa).ToList();
        var coarsePerHa = coarse.Select(x => x / coarseHa).ToList();
        var ratios = finePerHa.Zip(coarsePerHa, Ratio).ToList();
        double pooled = Ratio(finePerHa.Sum(), coarsePerHa.Sum());
        var (min, median, max, mean) = Spread(ratios);
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {part} {scenario} | {metric} per ha of matched in-polygon area: pooled {pooled:F3}, mean {mean:F3}; " +
            $"2.5 m Σ {fine.Sum():F1} over {lattice.FineMatchedM2:F2} m² (mean {finePerHa.Average():F1}/ha per run), " +
            $"5.0 m Σ {coarse.Sum():F1} over {lattice.CoarseMatchedM2:F2} m² (mean {coarsePerHa.Average():F1}/ha per run), {fine.Count} seeds; " +
            $"per seed min {min:F3} median {median:F3} max {max:F3}; {ratios.Count(r => !Inside(r))}/{ratios.Count} seeds outside [0.75, 1.33] (informational); " +
            $"raw-count ratio {Ratio(fine.Sum(), coarse.Sum()):F3} (not area-normalised, not gated)"));
        if (violations is null) return;
        if (!Inside(pooled)) violations.Add(string.Create(CultureInfo.InvariantCulture, $"{scenario} {metric} per ha pooled {pooled:F3}"));
        if (!Inside(mean)) violations.Add(string.Create(CultureInfo.InvariantCulture, $"{scenario} {metric} per ha mean {mean:F3}"));
    }

    private static void PrintSeed(string part, string scenario, int seed, RunResult fine, RunResult coarse, LatticeFacts lattice)
    {
        double fineHa = lattice.FineMatchedM2 / SquareMetresPerHectare, coarseHa = lattice.CoarseMatchedM2 / SquareMetresPerHectare;
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {part} {scenario} seed {seed,2}: matched per ha: accepted {fine.AcceptedMatched / fineHa:F1} vs {coarse.AcceptedMatched / coarseHa:F1} = " +
            $"{Ratio(fine.AcceptedMatched / fineHa, coarse.AcceptedMatched / coarseHa):F3}; promotions {fine.PromotionsMatched} ({fine.PromotionsMatched / fineHa:F1}/ha) vs " +
            $"{coarse.PromotionsMatched} ({coarse.PromotionsMatched / coarseHa:F1}/ha) = {Ratio(fine.PromotionsMatched / fineHa, coarse.PromotionsMatched / coarseHa):F3}. " +
            $"Whole stand: accepted {Ratio(fine.Accepted, coarse.Accepted):F3}, promotions {fine.Promotions}/{coarse.Promotions} = {Ratio(fine.Promotions, coarse.Promotions):F3} " +
            $"(interior {fine.PromotionsBy[Interior]}/{coarse.PromotionsBy[Interior]}, matched edge {fine.PromotionsBy[MatchedEdge]}/{coarse.PromotionsBy[MatchedEdge]}, " +
            $"2.5 m-only blocks {fine.PromotionsBy[Unmatched]})"));
    }

    private static void PrintSplit(string part, string scenario, List<RunResult> fine, List<RunResult> coarse, LatticeFacts l)
    {
        // perHa: ratio of per-hectare values = raw ratio × (5.0 m area / 2.5 m area) of that block set; otherwise raw totals.
        string Line(string metric, Func<RunResult, double> of, double fineM2, double coarseM2, bool perHa)
        {
            double scale = perHa ? coarseM2 / fineM2 : 1.0;
            var ratios = fine.Zip(coarse, (f, c) => Ratio(of(f), of(c)) * scale).ToList();
            var (min, median, max, mean) = Spread(ratios);
            return string.Create(CultureInfo.InvariantCulture,
                $"{metric} {(perHa ? "per ha" : "raw")} pooled {Ratio(fine.Sum(of), coarse.Sum(of)) * scale:F3} (Σ {fine.Sum(of):F1} / {coarse.Sum(of):F1}), mean {mean:F3}, seeds {min:F3}–{median:F3}–{max:F3}");
        }
        Func<(int X, int Z), bool> matchedEdge = block => l.GroundOf(block) == MatchedEdge;
        double intF = l.AreaM2(2.5, l.InteriorBlocks.Contains), intC = l.AreaM2(5.0, l.InteriorBlocks.Contains);
        double edgeF = l.AreaM2(2.5, matchedEdge), edgeC = l.AreaM2(5.0, matchedEdge);
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {part} SPLIT {scenario} | interior blocks ({l.InteriorBlocks.Count}; 2.5 m {intF:F2} m², 5.0 m {intC:F2} m²): ") +
            Line("accepted", r => r.AcceptedBy[Interior], intF, intC, true) + "; " + Line("promotions", r => r.PromotionsBy[Interior], intF, intC, true));
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {part} SPLIT {scenario} | matched edge blocks ({l.MatchedBlocks.Count - l.InteriorBlocks.Count}; 2.5 m {edgeF:F2} m², 5.0 m {edgeC:F2} m²): ") +
            Line("accepted", r => r.AcceptedBy[MatchedEdge], edgeF, edgeC, true) + "; " + Line("promotions", r => r.PromotionsBy[MatchedEdge], edgeF, edgeC, true));
        Console.WriteLine($"  {part} SPLIT {scenario} | all edge blocks, raw totals (2.5 m: matched and 2.5 m-only edge blocks; 5.0 m: matched edge cells): " +
            Line("accepted", r => r.AcceptedEdge, 1, 1, false) + "; " + Line("promotions", r => r.PromotionsEdge, 1, 1, false));
    }

    // S5-07B disclosure of the blocks that exist on the 2.5 m lattice only, under one light basis (no 5.0 m counterpart exists).
    private static void PrintUnmatched(string basis, string scenario, List<RunResult> fine, LatticeFacts l)
    {
        var promotions = fine.Select(r => (double)r.PromotionsBy[Unmatched]).ToList();
        var (min, median, max, _) = Spread(promotions);
        double area = l.AreaM2(2.5, l.UnmatchedBlocks.Contains);
        double promoted = promotions.Sum(), accepted = fine.Sum(r => r.AcceptedBy[Unmatched]);
        double wholePromoted = fine.Sum(r => r.Promotions), wholeAccepted = fine.Sum(r => r.AcceptedBy.Sum());
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  S5-07B UNMATCHED {basis} {scenario} | {l.UnmatchedBlocks.Count} blocks on the 2.5 m lattice only " +
            $"({l.FineCells(l.UnmatchedBlocks.Contains)} in-polygon 2.5 m cells, {area:F2} m²; no 5.0 m cell): promotions Σ {promoted:F0} over {fine.Count} seeds " +
            $"(per seed {min:F0}–{median:F1}–{max:F0}; {promoted / fine.Count / (area / SquareMetresPerHectare):F1}/ha per run), {Ratio(promoted, wholePromoted):P1} of the 2.5 m " +
            $"whole-stand promotions; accepted abundance Σ {accepted:F1}, {Ratio(accepted, wholeAccepted):P1} of the 2.5 m whole-stand per-cell acceptance"));
    }
}
