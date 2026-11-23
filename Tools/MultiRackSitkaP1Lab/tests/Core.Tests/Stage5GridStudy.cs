using System.Globalization;
using SitkaP1.Core;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// S5-07 grid consistency (spec §4, owner Q4). The same MA-1 woodland runs on the 2.5 m and the 5.0 m grid for 20 woodland
/// mast seeds × 30 years, in two scenarios (a year-0 managed thinning of every second stem, and no work). For each seed the
/// 2.5 m / 5.0 m ratio of (a) cumulative accepted establishment abundance and (b) cumulative promotions must lie in
/// [0.75, 1.33]. The shared-light control re-runs 2.5 m with every cell reading the light at its aligned 5 m block centre,
/// which is exactly the 5.0 m light field, so only the scaling policy differs. Zero out-of-polygon trees is exact.
/// Every ratio is printed before any assertion, so a failure is fully reported, never hidden.
/// </summary>
internal static class Stage5GridStudy
{
    internal const int Seeds = 20;
    internal const int Years = 30;
    internal const double ToleranceLow = 0.75;
    internal const double ToleranceHigh = 1.33;

    private sealed record RunResult(double Accepted, int Promotions, int EdgeBlockPromotions, int OutOfPolygon, double Ending, double MeanOccupancy);

    internal static void Register()
    {
        T.Add("S5-07 G-01: 2.5 m vs 5.0 m accepted abundance and multi-year promotions within [0.75, 1.33] over 20 mast seeds, with a shared-light control; zero out-of-polygon trees", () =>
        {
            var genesis25 = Session(null, new Stage5Options { GridCellM = 2.5 }).ReadStage5View();
            var cellsPerBlock = genesis25.Areas.SelectMany(a => a.Cells)
                .GroupBy(c => Stage5Promotion.BlockOf(c.Column, c.Row, 2.5)).ToDictionary(g => g.Key, g => g.Count());
            var edgeBlocks = cellsPerBlock.Where(p => p.Value < 4).Select(p => p.Key).ToHashSet();
            int cells25 = genesis25.Areas.Sum(a => a.Cells.Count);
            int cells50 = Session(null, new Stage5Options { GridCellM = 5.0 }).ReadStage5View().Areas.Sum(a => a.Cells.Count);
            Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
                $"  S5-07 lattice: 2.5 m {cells25} cells in {cellsPerBlock.Count} blocks ({edgeBlocks.Count} edge blocks with < 4 in-polygon cells); 5.0 m {cells50} cells"));

            // On the 5.0 m grid the shared-light control IS the model light (block centre = cell centre): proven, then reused.
            T.Eq(Run(5.0, 1, Stage5LightSampling.CellCentre, thin: true, edgeBlocks), Run(5.0, 1, Stage5LightSampling.SharedFiveMetreBlockCentre, thin: true, edgeBlocks),
                "5.0 m control equals the 5.0 m model");

            var violations = new List<string>();
            int outOfPolygon = 0;
            foreach (bool thin in new[] { true, false })
            {
                string scenario = thin ? "managed thinning" : "no work";
                var accModel = new List<double>(); var accShared = new List<double>();
                var proModel = new List<double>(); var proShared = new List<double>();
                double sumAcc50 = 0, sumAcc25 = 0, sumAcc25s = 0;
                int sumPro50 = 0, sumPro25 = 0, sumPro25s = 0, edgePro25 = 0;
                for (int seed = 1; seed <= Seeds; seed++)
                {
                    var r50 = Run(5.0, seed, Stage5LightSampling.CellCentre, thin, edgeBlocks);
                    var r25 = Run(2.5, seed, Stage5LightSampling.CellCentre, thin, edgeBlocks);
                    var r25s = Run(2.5, seed, Stage5LightSampling.SharedFiveMetreBlockCentre, thin, edgeBlocks);
                    outOfPolygon += r50.OutOfPolygon + r25.OutOfPolygon + r25s.OutOfPolygon;
                    accModel.Add(r25.Accepted / r50.Accepted);
                    accShared.Add(r25s.Accepted / r50.Accepted);
                    proModel.Add(Ratio(r25.Promotions, r50.Promotions));
                    proShared.Add(Ratio(r25s.Promotions, r50.Promotions));
                    sumAcc50 += r50.Accepted; sumAcc25 += r25.Accepted; sumAcc25s += r25s.Accepted;
                    sumPro50 += r50.Promotions; sumPro25 += r25.Promotions; sumPro25s += r25s.Promotions; edgePro25 += r25.EdgeBlockPromotions;
                    Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
                        $"  S5-07 {scenario} seed {seed,2}: accepted 2.5/5.0 {accModel[^1]:F3} (shared {accShared[^1]:F3}); promotions {r25.Promotions}/{r50.Promotions} = {proModel[^1]:F3} (shared {r25s.Promotions}/{r50.Promotions} = {proShared[^1]:F3}); ending abundance {r25.Ending / r50.Ending:F3}; occupancy {r25.MeanOccupancy / r50.MeanOccupancy:F3}"));
                }
                Summarise(scenario, "accepted abundance, model light", accModel, sumAcc25 / sumAcc50, violations);
                Summarise(scenario, "accepted abundance, shared-light control", accShared, sumAcc25s / sumAcc50, violations);
                Summarise(scenario, "promotions, model light", proModel, Ratio(sumPro25, sumPro50), violations);
                Summarise(scenario, "promotions, shared-light control", proShared, Ratio(sumPro25s, sumPro50), violations);
                Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
                    $"  S5-07 {scenario}: 2.5 m promotions in edge blocks {edgePro25} of {sumPro25} ({(sumPro25 == 0 ? 0 : 100.0 * edgePro25 / sumPro25):F1} %); totals 2.5 m {sumPro25}, shared {sumPro25s}, 5.0 m {sumPro50}"));
            }
            T.Eq(0, outOfPolygon, "zero out-of-polygon trees (exact)");
            if (violations.Count > 0)
                throw new Exception("S5-07 tolerance [0.75, 1.33] NOT met (" + violations.Count + " of 8 metric groups): " + string.Join("; ", violations));
        });
    }

    private static double Ratio(int a, int b) => b == 0 ? (a == 0 ? 1.0 : double.PositiveInfinity) : (double)a / b;

    private static void Summarise(string scenario, string metric, List<double> ratios, double pooled, List<string> violations)
    {
        int outside = ratios.Count(r => r < ToleranceLow || r > ToleranceHigh);
        Console.WriteLine(string.Create(CultureInfo.InvariantCulture,
            $"  S5-07 SUMMARY {scenario} | {metric}: min {ratios.Min():F3} max {ratios.Max():F3} mean {ratios.Average():F3} pooled {pooled:F3}; {outside}/{ratios.Count} seeds outside [0.75, 1.33]"));
        if (outside > 0)
            violations.Add(string.Create(CultureInfo.InvariantCulture,
                $"{scenario} {metric}: {outside}/{ratios.Count} seeds outside, range {ratios.Min():F3}–{ratios.Max():F3}, pooled {pooled:F3}"));
    }

    private static RunResult Run(double grid, int seed, Stage5LightSampling sampling, bool thin, HashSet<(int X, int Z)> edgeBlocks)
    {
        var c = Session(null, new Stage5Options { GridCellM = grid, MastSeed = seed, LightSampling = sampling });
        if (thin) ThinEverySecond(c);
        for (int y = 0; y < Years; y++) Advance(c);
        var v = c.ReadView();
        var s5 = c.ReadStage5View();
        var promotions = s5.Accounts.SelectMany(year => year.Areas).SelectMany(a => a.Promotions).ToArray();
        int outside = promotions.Count(p => !Stage5Tests.InsidePolygon(v.Areas.Single(a => a.AreaId == p.AreaId).Polygon, p.XM, p.ZM)) +
            v.Areas.Sum(a => a.Trees.Count(t => !Stage5Tests.InsidePolygon(a.Polygon, t.XM, t.ZM)));
        int edge = grid == 2.5 ? promotions.Count(p => edgeBlocks.Contains((p.BlockX, p.BlockZ))) : 0;
        var last = s5.Accounts[^1].Areas;
        return new RunResult(s5.Accounts.Sum(year => year.Areas.Sum(a => a.EstablishmentAccepted)), promotions.Length, edge, outside,
            last.Sum(a => a.EndingAbundance), last.Sum(a => a.MeanRelativeOccupancy * a.Cells) / last.Sum(a => a.Cells));
    }
}
