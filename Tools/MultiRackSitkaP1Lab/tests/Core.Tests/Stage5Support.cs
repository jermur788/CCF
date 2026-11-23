using System.Globalization;
using System.Reflection;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>Shared helpers for the Stage 5 qualification tests: sessions, fences, fixtures, oracles and the fingerprint.</summary>
internal static class Stage5Support
{
    internal static readonly FieldInfo RootField =
        typeof(MultiAreaContinuation).GetField("root", BindingFlags.NonPublic | BindingFlags.Instance)!;

    internal static MultiAreaRoot Root(MultiAreaContinuation c) => (MultiAreaRoot)RootField.GetValue(c)!;

    internal static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);

    internal static MultiAreaContinuation Session(MultiAreaExperiment? experiment = null, Stage5Options? options = null) =>
        new(experiment ?? MultiAreaGenerator.Generate(), MultiAreaModelSelection.Stage5, null, options);

    internal static MultiAreaAnnualResult Advance(MultiAreaContinuation c, bool consent = false)
    {
        var v = c.ReadView();
        return c.Advance(Fence(v), consent).Annual!;
    }

    /// <summary>Fell every second living tree (ordinal order) of every area through the real fenced Work Plan.</summary>
    internal static void ThinEverySecond(MultiAreaContinuation c)
    {
        var v = c.ReadView();
        foreach (var area in v.Areas)
            foreach (var tree in area.Trees.Where(t => t.IsLiving).Where((t, i) => i % 2 == 0))
                v = c.AddPending(Fence(v), area.AreaId, tree.TreeId, "S5 managed thinning").View;
        v = c.Approve(Fence(v)).View;
        c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
    }

    /// <summary>Fell every living tree of one area within <paramref name="radiusM"/> of a point (a managed gap).</summary>
    internal static int FellGap(MultiAreaContinuation c, string areaId, float x, float z, float radiusM)
    {
        var v = c.ReadView();
        var targets = v.Areas.Single(a => a.AreaId == areaId).Trees
            .Where(t => t.IsLiving && (t.XM - x) * (t.XM - x) + (t.ZM - z) * (t.ZM - z) <= radiusM * radiusM).ToArray();
        foreach (var tree in targets) v = c.AddPending(Fence(v), areaId, tree.TreeId, "S5 managed gap").View;
        v = c.Approve(Fence(v)).View;
        c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
        return targets.Length;
    }

    internal static Stage5AreaInput[] Inputs(MultiAreaSessionView v) => v.Areas.Select(a => new Stage5AreaInput(a.AreaId, a.SimulationSeed,
        a.Polygon, a.AreaHa, a.Trees)).ToArray();

    internal static MultiAreaStage5State StateOf(MultiAreaStage5View s5) => new(s5.Options, s5.MastSeed, s5.Areas, s5.PendingClears,
        s5.ApprovedClears, s5.ClearLog, s5.Accounts);

    /// <summary>
    /// Two touching 20 × 20 m square racks (A west, B east). Each needs at least one stem; trees are (id suffix, x mm, y mm,
    /// age, DBH mm, height mm).
    /// </summary>
    internal static MultiAreaExperiment TwoSquares(string id, (int X, int Y, int Age, int DbhMm, int HeightMm)[] a,
        (int X, int Y, int Age, int DbhMm, int HeightMm)[] b)
    {
        static MillimetrePoint P(long x, long y) => new(x, y);
        static MultiAreaTreeDefinition[] Trees(string areaId, (int X, int Y, int Age, int DbhMm, int HeightMm)[] list) =>
            list.Select((t, i) => new MultiAreaTreeDefinition(areaId + "-" + (i + 1).ToString("D3", CultureInfo.InvariantCulture),
                t.X, t.Y, t.Age, t.DbhMm, t.HeightMm)).ToArray();
        return new MultiAreaExperiment(id, "s5-test", 1, 7, new[]
        {
            new MultiAreaDefinition("MA-A", "Area A", "S5 test", new[] { P(0, 0), P(20_000, 0), P(20_000, 20_000), P(0, 20_000) }, 701, Trees("MA-A", a)),
            new MultiAreaDefinition("MA-B", "Area B", "S5 test", new[] { P(20_000, 0), P(40_000, 0), P(40_000, 20_000), P(20_000, 20_000) }, 702, Trees("MA-B", b)),
        });
    }

    /// <summary>Two empty (treeless) 10 × 10 m racks as pure-step inputs: no tree, so every cell's light is exactly 1.</summary>
    internal static Stage5AreaInput[] EmptySquares() => new[]
    {
        new Stage5AreaInput("MA-A", 801, new[] { new MultiAreaVertex(0, 0), new MultiAreaVertex(10, 0), new MultiAreaVertex(10, 10), new MultiAreaVertex(0, 10) },
            0.01f, Array.Empty<AnnualTree>()),
        new Stage5AreaInput("MA-B", 802, new[] { new MultiAreaVertex(10, 0), new MultiAreaVertex(20, 0), new MultiAreaVertex(20, 10), new MultiAreaVertex(10, 10) },
            0.01f, Array.Empty<AnnualTree>()),
    };

    /// <summary>Replace one cell of one area in a state.</summary>
    internal static MultiAreaStage5State WithCell(MultiAreaStage5State s, string areaId, int index, Func<Stage5Cell, Stage5Cell> edit) =>
        s with
        {
            Areas = Array.AsReadOnly(s.Areas.Select(a => a.AreaId != areaId ? a : a with
            {
                Cells = Array.AsReadOnly(a.Cells.Select((cell, i) => i == index ? edit(cell) : cell).ToArray()),
            }).ToArray()),
        };

    internal static Stage5Band Band(float density, float height, int year) => new(density, height, year, year, 0f, -1);

    /// <summary>The cell at (column, row) of one area in a Stage 5 read-out, or null.</summary>
    internal static Stage5Cell? CellOf(this MultiAreaStage5View s5, string areaId, int column, int row) =>
        s5.Areas.Single(a => a.AreaId == areaId).Cells.FirstOrDefault(c => c.Column == column && c.Row == row);

    /// <summary>Passes only when <paramref name="action"/> throws <typeparamref name="TException"/>.</summary>
    internal static void Throws<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new Exception("expected " + typeof(TException).Name + ", but the call succeeded");
    }

    // ---------------------------------------------------------------- independent oracles (written from the spec text)

    /// <summary>LR(light): piecewise-linear through the six spec anchors, in double.</summary>
    internal static double LightResponseOracle(double light)
    {
        double[] x = { 0, 0.10, 0.20, 0.25, 0.55, 1.00 };
        double[] y = { 0, 0.10, 0.30, 0.50, 0.90, 0.80 };
        light = Math.Clamp(light, 0, 1);
        for (int i = 1; i < x.Length; i++)
            if (light <= x[i]) return y[i - 1] + (y[i] - y[i - 1]) * (light - x[i - 1]) / (x[i] - x[i - 1]);
        return y[^1];
    }

    internal static double TargetOracle(double light) => 0.8 * (0.1 + 0.9 * Math.Clamp(light, 0, 1));

    // ---------------------------------------------------------------- fingerprint of a Stage 5 read-out

    internal static void Write(Model0FingerprintTests.Fingerprint w, MultiAreaStage5View s5)
    {
        static string F(float v) => F32.Hex(F32.Bits(v));
        static string D(double v) => BitConverter.DoubleToUInt64Bits(v).ToString("x16", CultureInfo.InvariantCulture);
        w.Text("stage5 year " + s5.EcologicalYear + " grid " + D(s5.Options.GridCellM) + " pressure " + F(s5.Options.BackgroundBrowsePressure) +
            " mastSeed " + s5.MastSeed + " token " + s5.Source.AreaModelVersion);
        foreach (var area in s5.Areas)
        {
            w.Text(area.AreaId + " cells " + area.Cells.Count);
            foreach (var cell in area.Cells)
            {
                w.Text(string.Join("|", cell.Column, cell.Row, F(cell.CentreXM), F(cell.CentreZM), D(cell.ClippedAreaM2), F(cell.Light),
                    F(cell.Bramble), F(cell.Bracken), cell.Bands.Count));
                foreach (var b in cell.Bands)
                    w.Text(string.Join("|", F(b.Density), F(b.Height), b.EstablishYear, b.OriginYear, F(b.LastBrowsedFraction), b.LastBrowseAssessmentYear));
            }
        }
        foreach (var o in s5.PendingClears.Concat(s5.ApprovedClears)) w.Text("order " + o.AreaId + "|" + o.Column + "|" + o.Row);
        foreach (var r in s5.ClearLog)
            w.Text(string.Join("|", "clear", r.AreaId, r.Column, r.Row, r.EcologicalYear, r.WorkRevision, F(r.BrambleBefore), F(r.BrackenBefore),
                r.BandsKept, D(r.AbundanceKept)));
        foreach (var y in s5.Accounts)
        {
            w.Text(string.Join("|", "year", y.YearBefore, y.YearAfter, D(y.MastRoll), y.MastLabel, F(y.MastMultiplier)));
            foreach (var a in y.Areas)
            {
                w.Text(string.Join("|", a.AreaId, a.Cells, D(a.SeedArrival), D(a.StartingAbundance), D(a.VegetationLoss), D(a.LightLoss),
                    D(a.BrowseLoss), D(a.EstablishmentRequested), D(a.EstablishmentAccepted), D(a.CapacityRejected), a.BandsCreated,
                    a.BandMerges, a.ThresholdCrossings, a.QualifyingCells, a.BlockCapDeferredCells, D(a.PromotionExported),
                    D(a.EndingAbundance), D(a.MeanRelativeOccupancy), D(a.MeanBramble), D(a.MeanBracken), D(a.MeanLight)));
                foreach (var p in a.Promotions)
                    w.Text(string.Join("|", p.TreeId, p.AreaId, p.Column, p.Row, p.BlockX, p.BlockZ, p.EstablishYear, p.AgeYears, F(p.HeightM),
                        F(p.DbhCm), F(p.CrownRadiusM), F(p.WoodlandCi), F(p.XM), F(p.ZM), p.CentreFallback, F(p.ExportedDensity)));
            }
        }
    }
}
