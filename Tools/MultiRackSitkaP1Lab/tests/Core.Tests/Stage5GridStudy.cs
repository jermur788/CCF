using System.Globalization;
using SitkaP1.Core;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// S5-07 grid consistency, split by owner Option A (2026-11-23; spec §4 and §9 Q4). The same MA-1 woodland runs on the 2.5 m
/// and the 5.0 m grid for 20 woodland-mast seeds × 30 years, in two scenarios: a year-0 managed thinning of every second
/// stem, and no work. Metrics: cumulative accepted establishment abundance and cumulative promotions, whole stand.
/// <list type="bullet">
/// <item><b>S5-07A, BLOCKING.</b> Both grids read ONE shared light field: the regeneration-free adult canopy (the post-adult-step
/// roster of a 1/1/1/0 run of the same scenario in that year) at every aligned 5 m block centre. The pooled and the mean
/// 2.5 m / 5.0 m ratio of both metrics must lie in [0.75, 1.33] in both scenarios. Light identity is proved: bit-equal light at
/// every coincident block in every year, and a year loop shown to reproduce MultiAreaContinuation.Advance bit for bit under
/// the model light. Also: zero out-of-polygon trees in every study run, and every 5.0 m run bit-identical to the first pass.</item>
/// <item><b>S5-07B, MANDATORY NON-BLOCKING DIAGNOSTIC.</b> The same comparison under each grid's own model light. It prints and
/// never asserts.</item>
/// </list>
/// Every number is printed before any assertion, so a failure is fully reported, never hidden.
/// </summary>
internal static class Stage5GridStudy
{
    internal const int Seeds = 20;
    internal const int Years = 30;
    internal const double ToleranceLow = 0.75;
    internal const double ToleranceHigh = 1.33;

    /// <summary>
    /// The first-pass <see cref="Stage5Support.Write"/> fingerprint of all forty 5.0 m model runs of this study (20 seeds × 2
    /// scenarios × 30 years), measured on the unmodified first-pass engine cb739cbd before any Option A edit. Option A must leave
    /// the 5.0 m grid bit-identical (owner purity criterion: a = 1.0, no edge-gate reduction).
    /// </summary>
    internal const string PinnedFiveMetrePurityHash = "c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8";

    private static readonly bool[] Scenarios = { true, false };
    private static string Name(bool thin) => thin ? "managed thinning" : "no work";

    /// <summary>One run's totals, whole stand and split by interior (all four 2.5 m centres in polygon) and edge blocks.</summary>
    private sealed record RunResult(double Accepted, double AcceptedInterior, double AcceptedEdge, int Promotions, int PromotionsInterior,
        int OutOfPolygon, string StateHash)
    {
        public int PromotionsEdge => Promotions - PromotionsInterior;
    }

    private static readonly Dictionary<(double Grid, int Seed, bool Thin), (RunResult Result, string? LegacyHash)> ModelRuns = new();
    private static HashSet<(int X, int Z)>? interiorBlocks;

    internal static void Register()
    {
        T.Add("S5-07A G-01 (BLOCKING): identical shared light field, 20 seeds × 30 years, managed thinning and no work: pooled AND mean 2.5 m / 5.0 m ratios of accepted abundance and promotions in [0.75, 1.33]; light proved identical every year; zero out-of-polygon trees; 5.0 m bit-identical to the first pass", () =>
        {
            PrintLattice();

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

            // (3) The shared-light study.
            var violations = new List<string>();
            int outOfPolygon = 0, coincidentChecks = 0;
            foreach (bool thin in Scenarios)
            {
                string scenario = Name(thin);
                var field = new SharedField(thin);
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
                    T.True(coincident >= Years * InteriorBlocks.Count, $"{scenario} seed {seed}: every interior block was compared every year");
                    coincidentChecks += coincident;
                    outOfPolygon += a25.OutOfPolygon + a50.OutOfPolygon;
                    r25.Add(a25);
                    r50.Add(a50);
                    PrintSeed("S5-07A", scenario, seed, a25, a50);
                }
                Gate(scenario, "accepted abundance", r25.Select(r => r.Accepted).ToList(), r50.Select(r => r.Accepted).ToList(), violations);
                Gate(scenario, "promotions", r25.Select(r => (double)r.Promotions).ToList(), r50.Select(r => (double)r.Promotions).ToList(), violations);
                PrintSplit("S5-07A", scenario, r25, r50);
            }
            Console.WriteLine($"  S5-07A light identity: {coincidentChecks} coincident block-year comparisons, all bit-equal");

            // (4) Zero out-of-polygon trees over every study run: the shared-light runs above and every model run (also used by S5-07B).
            foreach (bool thin in Scenarios)
                for (int seed = 1; seed <= Seeds; seed++)
                    outOfPolygon += ModelRun(2.5, seed, thin).Result.OutOfPolygon + ModelRun(5.0, seed, thin).Result.OutOfPolygon;

            T.Eq(PinnedFiveMetrePurityHash, purityHash, "5.0 m: Option A leaves every 5.0 m study run bit-identical to the first-pass engine (a = 1.0, no edge-gate reduction)");
            T.Eq(0, outOfPolygon, "zero out-of-polygon trees in every study run (exact)");
            if (violations.Count > 0)
                throw new Exception("S5-07A tolerance [0.75, 1.33] NOT met (" + violations.Count + " of 8 pooled/mean checks): " + string.Join("; ", violations));
        });

        T.Add("S5-07B G-02 (MANDATORY DIAGNOSTIC, NON-BLOCKING): each grid's own model light, same seeds, scenarios and horizon; real-light ratios, interior/edge sub-ratios and per-seed spread are disclosed; no threshold and no assertion", () =>
        {
            PrintLattice();
            foreach (bool thin in Scenarios)
            {
                string scenario = Name(thin);
                var r25 = new List<RunResult>();
                var r50 = new List<RunResult>();
                for (int seed = 1; seed <= Seeds; seed++)
                {
                    r25.Add(ModelRun(2.5, seed, thin).Result);
                    r50.Add(ModelRun(5.0, seed, thin).Result);
                    PrintSeed("S5-07B", scenario, seed, r25[^1], r50[^1]);
                }
                Report("S5-07B", scenario, "accepted abundance", r25.Select(r => r.Accepted).ToList(), r50.Select(r => r.Accepted).ToList());
                Report("S5-07B", scenario, "promotions", r25.Select(r => (double)r.Promotions).ToList(), r50.Select(r => (double)r.Promotions).ToList());
                PrintSplit("S5-07B", scenario, r25, r50);
            }
            Console.WriteLine("  S5-07B NOTE: light is the modelled index unity-k10a10-decoupled-v1, NOT calibrated PAR. Here each grid samples its own " +
                "canopy (including its own promoted adults) at its own cell centres, so the 2.5 m grid resolves gaps more finely; a higher 2.5 m " +
                "real-light recruitment reflects that finer gap-resolution sampling, not a scaling defect (the scaling policy itself is gated by " +
                "S5-07A). This diagnostic has no threshold and cannot fail the suite.");
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
        return (Summarise(h.Stage5, h.AreaIds, h.States, view), lightByYear);
    }

    // Totals of one finished run; areaIds[i] names the rack of states[i].
    private static RunResult Summarise(MultiAreaStage5State s5, IReadOnlyList<string> areaIds, IReadOnlyList<AnnualState> states,
        MultiAreaSessionView view)
    {
        double grid = s5.Options.GridCellM;
        var polygons = view.Areas.ToDictionary(a => a.AreaId, a => a.Polygon, StringComparer.Ordinal);
        var interiorCell = s5.Areas.ToDictionary(a => a.AreaId,
            a => a.Cells.Select(cell => InteriorBlocks.Contains(Stage5Promotion.BlockOf(cell.Column, cell.Row, grid))).ToArray(), StringComparer.Ordinal);
        double accepted = 0, acceptedInterior = 0, acceptedEdge = 0;
        int promotions = 0, promotionsInterior = 0, outside = 0;
        foreach (var year in s5.Accounts)
            foreach (var account in year.Areas)
            {
                accepted += account.EstablishmentAccepted;
                var interior = interiorCell[account.AreaId];
                for (int k = 0; k < interior.Length; k++)
                    if (interior[k]) acceptedInterior += account.CellEstablishmentAccepted[k];
                    else acceptedEdge += account.CellEstablishmentAccepted[k];
                foreach (var p in account.Promotions)
                {
                    promotions++;
                    if (InteriorBlocks.Contains((p.BlockX, p.BlockZ))) promotionsInterior++;
                    if (!Stage5Tests.InsidePolygon(polygons[p.AreaId], p.XM, p.ZM)) outside++;
                }
            }
        for (int i = 0; i < states.Count; i++)
            outside += states[i].Trees.Count(t => !Stage5Tests.InsidePolygon(polygons[areaIds[i]], t.XM, t.ZM));
        return new RunResult(accepted, acceptedInterior, acceptedEdge, promotions, promotionsInterior, outside, StateHash(states, s5));
    }

    private static string StateHash(IReadOnlyList<AnnualState> states, MultiAreaStage5State s5)
    {
        var w = new Model0FingerprintTests.Fingerprint();
        foreach (var state in states) w.State(state);
        WriteState(w, s5);
        return w.Hash();
    }

    /// <summary>Aligned 5 m blocks whose four 2.5 m cell centres are all in polygon; a 5.0 m cell is interior when its block is.</summary>
    private static HashSet<(int X, int Z)> InteriorBlocks => interiorBlocks ??=
        Session(null, new Stage5Options { GridCellM = 2.5 }).ReadStage5View().Blocks
            .Where(b => b.InPolygonCells == Stage5Promotion.CellsPerBlock(2.5)).Select(b => (b.BlockX, b.BlockZ)).ToHashSet();

    // ------------------------------------------------------------------------------------------------ the S5-07A control

    /// <summary>
    /// The S5-07A year loop. It repeats MultiAreaContinuation.Advance for a 1/1/1/1 session with no work orders by calling the
    /// same production functions in the same order: MultiAreaAnnual.StepStage4 for every rack with a living stem on the
    /// start-of-year woodland (an empty rack is carried), Stage5Annual.Step on the post-adult-step rosters, and the promoted
    /// adults joined in ordinal TreeId order. The only difference it can introduce is the light field handed to Stage 5, and
    /// S5-07A asserts it reproduces Advance bit for bit when that field is the model light (null).
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
    /// centre), so both grids see the same numbers at every coincident point. Excluded in both grids alike: the shading of
    /// Stage 5 promoted trees on the regeneration layer. Each run keeps its own adult step, seed rain and promoted adults.
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

    // Prints one metric (pooled, mean and per-seed spread) and returns (pooled, mean).
    private static (double Pooled, double Mean) Report(string part, string scenario, string metric, List<double> fine, List<double> coarse)
    {
        var ratios = fine.Zip(coarse, Ratio).ToList();
        double pooled = Ratio(fine.Sum(), coarse.Sum());
        var (min, median, max, mean) = Spread(ratios);
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {part} SUMMARY {scenario} | {metric}: pooled {pooled:F3} ({fine.Sum():F1} / {coarse.Sum():F1}), mean {mean:F3}; per seed min {min:F3} median {median:F3} max {max:F3}; {ratios.Count(r => !Inside(r))}/{ratios.Count} seeds outside [0.75, 1.33] (informational)"));
        return (pooled, mean);
    }

    private static void Gate(string scenario, string metric, List<double> fine, List<double> coarse, List<string> violations)
    {
        var (pooled, mean) = Report("S5-07A", scenario, metric, fine, coarse);
        if (!Inside(pooled)) violations.Add(string.Create(CultureInfo.InvariantCulture, $"{scenario} {metric} pooled {pooled:F3}"));
        if (!Inside(mean)) violations.Add(string.Create(CultureInfo.InvariantCulture, $"{scenario} {metric} mean {mean:F3}"));
    }

    private static void PrintSeed(string part, string scenario, int seed, RunResult fine, RunResult coarse) =>
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  {part} {scenario} seed {seed,2}: accepted 2.5/5.0 {Ratio(fine.Accepted, coarse.Accepted):F3}; promotions {fine.Promotions}/{coarse.Promotions} = {Ratio(fine.Promotions, coarse.Promotions):F3} (interior {fine.PromotionsInterior}/{coarse.PromotionsInterior}, edge {fine.PromotionsEdge}/{coarse.PromotionsEdge})"));

    private static void PrintSplit(string part, string scenario, List<RunResult> fine, List<RunResult> coarse)
    {
        string Line(string metric, Func<RunResult, double> of)
        {
            var ratios = fine.Zip(coarse, (f, c) => Ratio(of(f), of(c))).ToList();
            var (min, median, max, mean) = Spread(ratios);
            return string.Create(CultureInfo.InvariantCulture,
                $"{metric} pooled {Ratio(fine.Sum(of), coarse.Sum(of)):F3} ({fine.Sum(of):F1} / {coarse.Sum(of):F1}), mean {mean:F3}, seeds {min:F3}–{median:F3}–{max:F3}");
        }
        Console.WriteLine($"  {part} SPLIT {scenario} | interior blocks: " + Line("accepted", r => r.AcceptedInterior) + "; " + Line("promotions", r => r.PromotionsInterior));
        Console.WriteLine($"  {part} SPLIT {scenario} | edge blocks:     " + Line("accepted", r => r.AcceptedEdge) + "; " + Line("promotions", r => r.PromotionsEdge));
    }

    private static void PrintLattice()
    {
        var blocks25 = Session(null, new Stage5Options { GridCellM = 2.5 }).ReadStage5View().Blocks;
        var cells50 = Session(null, new Stage5Options { GridCellM = 5.0 }).ReadStage5View().Blocks;
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  S5-07 lattice: 2.5 m {blocks25.Sum(b => b.InPolygonCells)} cells in {blocks25.Count} blocks ({InteriorBlocks.Count} interior; edge blocks by in-polygon cells: " +
            $"3 → {blocks25.Count(b => b.InPolygonCells == 3)}, 2 → {blocks25.Count(b => b.InPolygonCells == 2)}, 1 → {blocks25.Count(b => b.InPolygonCells == 1)}); " +
            $"5.0 m {cells50.Count} cells ({cells50.Count(b => InteriorBlocks.Contains((b.BlockX, b.BlockZ)))} in interior blocks, " +
            $"{cells50.Count(b => !blocks25.Any(x => x.BlockX == b.BlockX && x.BlockZ == b.BlockZ))} in blocks with no 2.5 m cell)"));
    }
}
