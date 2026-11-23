using System.Diagnostics;
using System.Globalization;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Multi #900 (D-094) canopy cover, growing space and Modelled Light Index tests: numerical oracles, exact 5.0 m Unity
/// parity, grid decoupling, living-only casting, per-rack area weighting, cross-border influence, and the matched control
/// versus a real 10-tree Fell in Area A (the gate; a 12-tree variant is supplementary) at Year 0 / +1 / +3 with crown radius
/// evidence and measured runtime; independent sloped-boundary and overlapping-crown oracles. Author tests, not independent review.
/// </summary>
internal static class CanopyLightTests
{
    private const string A = "MA-A";
    private const string B = "MA-B";
    private const CrownModelVersion C2 = CrownModelVersion.AreaScaledEquilibriumC2;
    private static readonly double[] Grids = { 2.5, 5.0 };

    internal static void Register()
    {
        // ---------------------------------------------------------------------------------------------- oracles
        T.Add("light O-01: empty stand = 1.000 everywhere; cover 0; one opening the size of the area", () =>
        {
            foreach (double g in Grids)
            {
                var area = Evaluate(g).Areas[0];
                T.True(area.Light.Cells.All(cell => cell.LightIndex == 1f) && area.Light.MeanLightIndex == 1.0, "light 1 at " + g);
                T.Eq(0.0, area.Cover.CoveredM2, "no cover");
                T.Eq(1600.0, area.Cover.UncoveredM2, "uncovered = the 40 m × 40 m polygon");
                T.Eq(1, area.Cover.OpeningCount, "one opening");
                T.Eq(area.Cover.SampledAreaM2, area.Cover.LargestOpeningAreaM2, "the opening is the whole sampled area");
            }
        });

        T.Add("light O-02: single mature tree overhead = 0.000; single 4 m young tree overhead = 0.500 (both grids)", () =>
        {
            foreach (double g in Grids)
            {
                float c = (float)(3.5 * g); // a cell centre on this grid
                T.Eq(0f, CellAt(Evaluate(g, Tree("t", c, c, 20f, 3f)), c, c).LightIndex, "mature overhead at " + g);
                T.Eq(0.5f, CellAt(Evaluate(g, Tree("t", c, c, 4f, 1.2f)), c, c).LightIndex, "4 m young overhead at " + g);
            }
        });

        T.Add("light O-03: drip-line edge r = 1.73 m gives 0.409 at both 5.0 m and 2.5 m (grid-decoupled reach)", () =>
        {
            var values = new List<float>();
            foreach (double g in Grids)
            {
                float c = (float)(3.5 * g);
                float light = CellAt(Evaluate(g, Tree("t", c + 1.73f, c, 20f, 1.73f)), c, c).LightIndex;
                T.True(Math.Abs(light - 0.409) < 5e-4, $"drip line {light} at {g}");
                values.Add(light);
            }
            T.True(Math.Abs(values[0] - values[1]) < 1e-6, "identical at 2.5 m and 5.0 m");
            Console.WriteLine($"  oracle drip-line r=1.73: 2.5 m {values[0]:F6}, 5.0 m {values[1]:F6}");
        });

        T.Add("light O-04: narrow two-tree gap ≈ 0.349 with 0 % crown-union overlap", () =>
        {
            foreach (double g in Grids)
            {
                float c = (float)(3.5 * g);
                var report = Evaluate(g, Tree("t1", c - 2.5f, c, 20f, 1.73f), Tree("t2", c + 2.5f, c, 20f, 1.73f));
                float light = CellAt(report, c, c).LightIndex;
                T.True(Math.Abs(light - 0.349) < 1e-3, $"gap light {light} at {g}");
                T.Eq(0.0, report.Areas[0].Cover.OverlappedM2, "no crown overlap");
                T.Eq(1.0, report.Areas[0].Cover.MeanOverlapCount, "every covered micro-cell has exactly one crown");
                Console.WriteLine($"  oracle two-tree gap at {g} m: light {light:F6}, overlapped {report.Areas[0].Cover.OverlappedM2} m2");
            }
        });

        T.Add("light O-05: dense lattice shows no grid-size artefact (decoupled), unlike the legacy cell/2 reach", () =>
        {
            var lattice = new List<AnnualTree>();
            for (int i = 0; i < 10; i++)
            for (int j = 0; j < 10; j++)
                lattice.Add(Tree($"L{i:D2}{j:D2}", 2f + 4f * i, 2f + 4f * j, 12f, 1.6f));
            var trees = lattice.ToArray();
            double fine = Evaluate(2.5, trees).Areas[0].Light.MeanLightIndex;
            double coarse = Evaluate(5.0, trees).Areas[0].Light.MeanLightIndex;
            double legacyFine = LegacyCoupledMean(trees, 2.5), legacyCoarse = LegacyCoupledMean(trees, 5.0);
            double ratio = Math.Max(fine, coarse) / Math.Min(fine, coarse);
            double legacyRatio = Math.Max(legacyFine, legacyCoarse) / Math.Min(legacyFine, legacyCoarse);
            Console.WriteLine($"  oracle lattice mean light: decoupled 2.5 m {fine:F4} / 5.0 m {coarse:F4} (ratio {ratio:F2}); legacy coupled 2.5 m {legacyFine:F4} / 5.0 m {legacyCoarse:F4} (ratio {legacyRatio:F2})");
            T.True(ratio < 1.5, "decoupled 2.5/5.0 mean differs only by sampling");
            T.True(legacyRatio > ratio, "the legacy coupled reach shows the larger grid artefact");
        });

        T.Add("light O-06: felled stumps and biologically dead trees cast nothing (light 1.000, cover 0)", () =>
        {
            foreach (double g in Grids)
            {
                float c = (float)(3.5 * g);
                var stump = Tree("s", c, c, 20f, 3f) with { Death = new AnnualDeath("s", 1, FellCause.Harvest, FellCause.Mechanism) };
                var dead = Tree("d", c + 1f, c, 20f, 3f) with { Death = new AnnualDeath("d", 1, "self-thinning", "hazard") };
                var report = Evaluate(g, stump, dead);
                T.True(report.Areas[0].Light.Cells.All(cell => cell.LightIndex == 1f), "no shade from stump or dead at " + g);
                T.Eq(0.0, report.Areas[0].Cover.CoveredM2, "no cover");
                T.Eq(0, report.Trees.Count, "no growing-space row for a non-living tree");
            }
        });

        T.Add("light O-07: sloped-boundary triangle: covered + uncovered = hand polygon area 30.5 m2; fraction over polygon, not sampled area", () =>
        {
            // Triangle (0,0) (9,1) (2,7): every edge cuts micro-cells diagonally. Hand shoelace area = |9·7 − 1·2| / 2 = 30.5 m2.
            const double HandArea = 30.5;
            var triangle = Array.AsReadOnly(new[] { new CanopyVertex(0, 0), new CanopyVertex(9, 1), new CanopyVertex(2, 7) });
            CanopyCover CoverOf(params AnnualTree[] trees) =>
                CanopyLightEvaluator.Evaluate(new[] { new CanopyArea("TRI", triangle, 0.00305f, trees) }).Areas[0].Cover;

            // A crown straddling the sloped (9,1)-(2,7) edge, and a crown that covers the whole triangle.
            foreach (var (name, trees) in new[] { ("edge crown", new[] { Tree("e", 5.5f, 4f, 20f, 2f) }), ("full crown", new[] { Tree("f", 3.5f, 3f, 20f, 20f) }) })
            {
                var cover = CoverOf(trees);
                T.True(Math.Abs(cover.CoveredM2 + cover.UncoveredM2 - HandArea) <= 1e-9 * HandArea, name + ": covered + uncovered = 30.5");
                T.True(Math.Abs(cover.CoverFraction - cover.CoveredM2 / HandArea) < 1e-15, name + ": fraction = covered / 30.5");
                T.True(cover.SampledAreaM2 < HandArea && Math.Abs(cover.SampledAreaM2 - HandArea) > 1e-3, name + ": sampled area visibly below the polygon");
                T.True(Math.Abs(cover.CoverFraction - cover.CoveredM2 / cover.SampledAreaM2) > 1e-4, name + ": not renormalised by the sampled area");
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "  oracle sloped triangle {0}: polygon {1} m2, sampled {2:F6} m2, covered {3:F6} m2, uncovered {4:F6} m2, fraction {5:F6} (covered/sampled would be {6:F6})",
                    name, HandArea, cover.SampledAreaM2, cover.CoveredM2, cover.UncoveredM2, cover.CoverFraction, cover.CoveredM2 / cover.SampledAreaM2));
            }
            var full = CoverOf(Tree("f", 3.5f, 3f, 20f, 20f));
            T.True(Math.Abs(full.CoveredM2 - full.SampledAreaM2) < 1e-12, "full crown covers every sampled micro-cell");
            T.True(full.CoverFraction < 1.0 && full.CoverFraction > 0.95, "edge loss stays visible: fraction below 1");
            // The clipped sum must be close to the true area: the unsampled strip is at most the perimeter × half a micro-cell.
            double perimeter = Math.Sqrt(82) + Math.Sqrt(85) + Math.Sqrt(53);
            T.True(HandArea - full.SampledAreaM2 <= perimeter * CanopyLightEvaluator.CoverMicroCellM / 2, "edge loss bounded by perimeter × s/2");
            // Unclipped counting (the old rule) would have reported whole 0.0625 m2 cells, i.e. area outside the rack.
            var empty = CoverOf();
            T.True(Math.Abs(empty.UncoveredM2 - HandArea) < 1e-12 && empty.CoveredM2 == 0.0, "empty: uncovered = polygon area");
            T.True(Math.Abs(empty.LargestOpeningAreaM2 - empty.SampledAreaM2) < 1e-9, "empty: one opening = the clipped sampled area");
        });

        T.Add("light O-08: duplicate crowns do not double cover; two crowns 3 m apart match the analytic circle union and lens", () =>
        {
            // r = 3 m on the 40 m square. Analytic: one disc 9π = 28.274334 m2; two discs at d = 3 m: lens
            // 2r²·acos(d/2r) − (d/2)·sqrt(4r² − d²) = 18·π/3 − 1.5·sqrt(27) = 11.055327 m2; union 18π − lens = 45.493341 m2.
            const double Disc = 28.274334, Lens = 11.055327, Union = 45.493341;
            var one = Evaluate(2.5, Tree("a", 20.1f, 20.1f, 20f, 3f)).Areas[0].Cover;
            var twin = Evaluate(2.5, Tree("a", 20.1f, 20.1f, 20f, 3f), Tree("b", 20.1f, 20.1f, 20f, 3f)).Areas[0].Cover;
            T.Eq(one.CoveredM2, twin.CoveredM2, "two identical crowns cover exactly what one covers");
            T.Eq(twin.CoveredM2, twin.OverlappedM2, "the whole duplicate area is overlapped");
            T.Eq(2.0, twin.MeanOverlapCount, "mean overlap count 2 under a duplicated crown");
            T.True(Math.Abs(one.CoveredM2 - Disc) / Disc < 0.03, $"one disc {one.CoveredM2} ≈ 9π");
            T.True(twin.CoveredM2 < 1.03 * Disc, "covered area is not doubled");
            var pair = Evaluate(2.5, Tree("a", 18.6f, 20.1f, 20f, 3f), Tree("b", 21.6f, 20.1f, 20f, 3f)).Areas[0].Cover;
            T.True(Math.Abs(pair.CoveredM2 - Union) / Union < 0.03, $"union {pair.CoveredM2} ≈ 45.493341");
            T.True(Math.Abs(pair.OverlappedM2 - Lens) / Lens < 0.05, $"overlap {pair.OverlappedM2} ≈ lens 11.055327");
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                "  oracle crowns r=3: one {0:F4} (9π {1}), duplicate {2:F4} overlapped {3:F4} mean {4}; pair d=3 union {5:F4} (analytic {6}), overlap {7:F4} (lens {8})",
                one.CoveredM2, Disc, twin.CoveredM2, twin.OverlappedM2, twin.MeanOverlapCount, pair.CoveredM2, Union, pair.OverlappedM2, Lens));
        });

        // ---------------------------------------------------------------------------------------------- MA-1 real trees
        T.Add("light P-01: 5.0 m mode equals the Unity RecomputeCanopy equation bit for bit on MA-1 (models 0 and 1)", () =>
        {
            foreach (var model in new[] { CrownModelVersion.LegacyFrozen, C2 })
            {
                var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), model);
                var woodland = Woodland(c.ReadView());
                int cells = 0;
                foreach (var area in c.ReadCanopyLight(5.0).Areas)
                foreach (var cell in area.Light.Cells)
                {
                    T.Eq(F32.Bits(UnityCell(cell.CentreXM, cell.CentreZM, woodland, 5f)), F32.Bits(cell.LightIndex), "Unity parity");
                    cells++;
                }
                // At 2.5 m the reach stays r + 2.5 (decoupled), so it is NOT Unity's r + cell/2 = r + 1.25.
                int differs = 0;
                foreach (var area in c.ReadCanopyLight(2.5).Areas)
                foreach (var cell in area.Light.Cells)
                {
                    T.Eq(F32.Bits(UnityCell(cell.CentreXM, cell.CentreZM, woodland, 5f)), F32.Bits(cell.LightIndex), "2.5 m uses b = 2.5");
                    if (UnityCell(cell.CentreXM, cell.CentreZM, woodland, 2.5f) != cell.LightIndex) differs++;
                }
                T.True(differs > 0, "the 2.5 m grid is decoupled from cell/2");
                Console.WriteLine($"  Unity 5.0 m parity {model}: {cells} cells bit-identical; 2.5 m cells differing from coupled reach: {differs}");
            }
        });

        T.Add("light P-02: values finite in [0,1], deterministic; per-rack summaries are clipped-area weighted and never blended", () =>
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), C2);
            foreach (double g in Grids)
            {
                var report = c.ReadCanopyLight(g);
                var again = CanopyLightEvaluator.Evaluate(Inputs(c.ReadView()), g, C2);
                T.Eq(Canon(report), Canon(again), "repeat evaluation identical at " + g);
                T.Eq("unity-k10a10-decoupled-v1", report.Metadata.Version, "version");
                T.Eq("index", report.Metadata.Kind, "kind");
                T.True(!report.Metadata.CalibratedPar && report.Metadata.Label.Contains("not calibrated PAR"), "not PAR");
                T.Eq(g, report.Metadata.GridCellM, "grid");
                T.Eq(2.5, report.Metadata.ShadeBufferM, "buffer");
                T.Eq(5, report.Metadata.Uncertainties.Count, "uncertainties");
                T.Eq(CrownModel.C2RuleId, report.Metadata.CrownRuleId, "crown rule");
                foreach (var area in report.Areas)
                {
                    T.True(area.Light.Cells.All(cell => float.IsFinite(cell.LightIndex) && cell.LightIndex is >= 0f and <= 1f), "bounded");
                    double sw = area.Light.Cells.Sum(cell => cell.ClippedAreaM2);
                    double slw = area.Light.Cells.Sum(cell => cell.ClippedAreaM2 * cell.LightIndex);
                    T.True(Math.Abs(sw - area.Light.SampledAreaM2) < 1e-9 && Math.Abs(slw / sw - area.Light.MeanLightIndex) < 1e-12, "area-weighted mean");
                    T.True(area.Light.SampledAreaM2 <= area.PolygonAreaM2 + 1e-9, "no renormalisation hides edge loss");
                    T.True(area.Light.Cells.Any(cell => cell.ClippedAreaM2 < g * g - 1e-9), "boundary cells are clipped");
                    T.True(area.Light.Cells.All(cell => cell.ClippedAreaM2 > 0 && cell.ClippedAreaM2 <= g * g + 1e-9), "clipped area in (0, cell]");
                    T.True(Math.Abs(area.Cover.CoveredM2 + area.Cover.UncoveredM2 - area.PolygonAreaM2) <= 1e-9 * area.PolygonAreaM2, "covered + uncovered = polygon area");
                    T.True(Math.Abs(area.Cover.CoverFraction - area.Cover.CoveredM2 / area.PolygonAreaM2) < 1e-15, "cover fraction uses the polygon area");
                    T.True(area.Cover.SampledAreaM2 <= area.PolygonAreaM2 + 1e-9 && area.Cover.CoveredM2 <= area.Cover.SampledAreaM2 + 1e-9, "cover sampled <= polygon");
                    Console.WriteLine($"  {area.AreaId} {g} m cover: polygon {area.PolygonAreaM2} m2, sampled {area.Cover.SampledAreaM2:F4} m2, covered {area.Cover.CoveredM2:F4} m2, uncovered {area.Cover.UncoveredM2:F4} m2, fraction {area.Cover.CoverFraction:F6}, overlapped {area.Cover.OverlappedM2:F4} m2, largest opening {area.Cover.LargestOpeningAreaM2:F4} m2");
                    Console.WriteLine($"  {area.AreaId} {g} m: polygon {area.PolygonAreaM2} m2, light cells {area.Light.Cells.Count}, sampled {area.Light.SampledAreaM2:F2} m2, mean {area.Light.MeanLightIndex:F4}, min {area.Light.MinLightIndex:F4}, max {area.Light.MaxLightIndex:F4}, cross-border cells {area.Light.CellsWithCrossBorderInfluence}");
                }
                var a = report.Areas.Single(x => x.AreaId == A);
                T.True(Math.Abs(a.PolygonAreaM2 - 915.0) < 1e-9, "A polygon 915 m2 (not blended with B)");
            }
            T.Refuses("CanopyGridUnsupported", () => c.ReadCanopyLight(1.0));
        });

        T.Add("light P-03: Layer A per tree: woodland CI, neighbours within 8 m, cross-border counts; stand density and BA per rack", () =>
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), C2);
            var v = c.ReadView();
            var report = c.ReadCanopyLight();
            var woodland = Woodland(v);
            T.Eq(woodland.Length, report.Trees.Count, "one row per living tree");
            foreach (var row in report.Trees)
            {
                var tree = woodland.Single(t => t.TreeId == row.TreeId);
                var (ci, n) = CrownModel.WoodlandCompetition(tree, woodland);
                T.True(F32.Bits(ci) == F32.Bits(row.Ci) && n == row.NeighboursWithin8M && row.CrownRadiusM == tree.CrownRadiusM, "row " + row.TreeId);
            }
            T.True(report.Trees.Any(row => row.AreaId == A && row.CrossBorderNeighbours > 0) &&
                   report.Trees.Any(row => row.AreaId == B && row.CrossBorderNeighbours > 0), "cross-border neighbours exist on both sides");
            foreach (var area in report.Areas)
            {
                var stand = CrownC2Tests.Area(v, area.AreaId).Stand;
                T.True(area.Stand.LivingStems == stand.LivingCount && area.Stand.StemsPerHa == stand.StemsPerHa &&
                       area.Stand.BasalAreaM2PerHa == stand.BasalAreaM2PerHa, "stand diagnostics equal the engine's own");
                Console.WriteLine($"  {area.AreaId} Layer A: {area.Stand.LivingStems} stems, {area.Stand.StemsPerHa:F1} stems/ha, BA {area.Stand.BasalAreaM2PerHa:F2} m2/ha; cover {100 * area.Cover.CoverFraction:F2} %, overlapped {area.Cover.OverlappedM2:F2} m2, mean overlap {area.Cover.MeanOverlapCount:F3}");
            }
        });

        // ---------------------------------------------------------------------------------------------- matched control
        T.Add("light MC-01 (GATE): matched untreated control vs a real 10-tree Fell in Area A at Year 0 / +1 / +3 with crown evidence and runtime", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var treated = MatchedControl(e, 10, "MC");

            // Runtime on the actual tree counts (fresh evaluations, not the cache).
            var inputs = Inputs(treated.ReadView());
            foreach (double g in Grids)
            {
                CanopyLightEvaluator.Evaluate(inputs, g, C2); // warm-up (JIT)
                var sw = Stopwatch.StartNew();
                const int runs = 5;
                for (int i = 0; i < runs; i++) CanopyLightEvaluator.Evaluate(inputs, g, C2);
                sw.Stop();
                Console.WriteLine($"  MC runtime {g} m grid: {sw.Elapsed.TotalMilliseconds / runs:F1} ms per full evaluation " +
                    $"({string.Join(" + ", inputs.Select(a => a.Trees.Count(t => t.IsLiving)))} living trees)");
            }
            var genesisInputs = Inputs(new MultiAreaContinuation(e, C2).ReadView());
            var sw0 = Stopwatch.StartNew();
            CanopyLightEvaluator.Evaluate(genesisInputs, 2.5, C2);
            sw0.Stop();
            Console.WriteLine($"  MC runtime genesis 2.5 m: {sw0.Elapsed.TotalMilliseconds:F1} ms ({string.Join(" + ", genesisInputs.Select(a => a.Trees.Count))} living trees)");
        });

        T.Add("light MC-02 (SUPPLEMENTARY, not the gate): the same matched control with a 12-tree Fell", () =>
        {
            MatchedControl(MultiAreaGenerator.Generate(), 12, "MC12");
        });
    }

    // ------------------------------------------------------------------------------------------------ matched control

    /// <summary>
    /// Runs an untreated control and a treated clone in which the <paramref name="n"/> Area A trees nearest (35, 9) m are
    /// felled through a real fenced Mark → Approve → Execute, prints stand/cover/light rows at Y0, Y+1 and Y+3, the crown
    /// radius distributions and the annual crown expansion of surviving trees, and returns the treated session at Y+3.
    /// </summary>
    private static MultiAreaContinuation MatchedControl(MultiAreaExperiment e, int n, string tag)
    {
        var control = new MultiAreaContinuation(e, C2);
        var treated = new MultiAreaContinuation(e, C2);
        var genesis = treated.ReadView();
        var fell = CrownC2Tests.ClusterInA(genesis, n);
        T.Eq(n, fell.Length, "the Fell has exactly " + n + " trees");
        Console.WriteLine($"  {tag} removed IDs ({fell.Length}): " + string.Join(", ", fell));

        // Fell neighbours: living trees of either area within 8 m of any felled stem at genesis (positions never move).
        var felledTrees = genesis.Areas.SelectMany(a => a.Trees).Where(t => fell.Contains(t.TreeId)).ToArray();
        var neighbourIds = genesis.Areas.SelectMany(a => a.Trees)
            .Where(t => t.IsLiving && !fell.Contains(t.TreeId) && felledTrees.Any(f => Dist(f, t) <= 8f))
            .Select(t => t.TreeId).ToHashSet(StringComparer.Ordinal);
        Console.WriteLine($"  {tag} Fell neighbours within 8 m: {neighbourIds.Count} " +
            $"(A {neighbourIds.Count(id => id.StartsWith("MA-A", StringComparison.Ordinal))}, B {neighbourIds.Count(id => id.StartsWith("MA-B", StringComparison.Ordinal))})");

        // Mark and Approve: zero ecological, canopy or light change.
        var v = treated.ReadView();
        foreach (var id in fell) v = treated.AddPending(CrownC2Tests.Fence(v), A, id, "matched-control Fell").View;
        v = treated.Approve(CrownC2Tests.Fence(v)).View;
        T.Eq(Canon(control.ReadCanopyLight()), Canon(treated.ReadCanopyLight()), "Approve alone changes nothing");
        Row(tag, "Y0 before", control, treated);

        var executed = treated.Execute(CrownC2Tests.Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
        var removed = executed.Areas.SelectMany(a => a.Trees).Where(t => !t.IsLiving).Select(t => t.TreeId).Order(StringComparer.Ordinal);
        T.True(removed.SequenceEqual(fell.Order(StringComparer.Ordinal)), "Execute removed exactly the approved IDs");
        var (cover0, light0, changedA0, _) = Row(tag, "Y0 after Fell", control, treated);
        T.True(cover0 < 0 && light0 > 0 && changedA0 > 0, "the Fell opens Area A: less cover, more light");
        RadiusRows(tag, "Y0 after Fell", control.ReadView(), treated.ReadView(), neighbourIds);

        var previous = (Control: control.ReadView(), Treated: treated.ReadView());
        for (int year = 1; year <= 3; year++)
        {
            var cNow = control.Advance(CrownC2Tests.Fence(control.ReadView()), false).View;
            var tNow = treated.Advance(CrownC2Tests.Fence(treated.ReadView()), false).View;
            ExpansionRows(tag, year, previous.Control, cNow, previous.Treated, tNow, neighbourIds);
            previous = (cNow, tNow);
            if (year is 1 or 3)
            {
                Row(tag, $"Y+{year}", control, treated);
                RadiusRows(tag, $"Y+{year}", cNow, tNow, neighbourIds);
            }
        }
        return treated;
    }

    private static float Dist(AnnualTree a, AnnualTree b)
    {
        float dx = a.XM - b.XM, dz = a.ZM - b.ZM;
        return (float)Math.Sqrt(dx * dx + dz * dz);
    }

    /// <summary>min / p25 / median / p75 / max / mean (nearest-rank percentiles on the sorted values).</summary>
    private static string Distribution(IEnumerable<double> values, string format)
    {
        var r = values.Order().ToArray();
        if (r.Length == 0) return "n=0";
        double P(double q) => r[(int)Math.Round(q * (r.Length - 1))];
        string F(double x) => x.ToString(format, CultureInfo.InvariantCulture);
        return $"n={r.Length}|min {F(r[0])}|p25 {F(P(.25))}|median {F(P(.5))}|p75 {F(P(.75))}|max {F(r[^1])}|mean {F(r.Average())}";
    }

    private static void RadiusRows(string tag, string stage, MultiAreaSessionView control, MultiAreaSessionView treated,
        IReadOnlySet<string> neighbours)
    {
        foreach (var (label, view) in new[] { ("control", control), ("treated", treated) })
        {
            var living = view.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
            Console.WriteLine($"  {tag}|radius|{stage}|{label}|A all living|" +
                Distribution(living.Where(t => t.TreeId.StartsWith("MA-A", StringComparison.Ordinal)).Select(t => (double)t.CrownRadiusM), "F3"));
            Console.WriteLine($"  {tag}|radius|{stage}|{label}|Fell neighbours|" +
                Distribution(living.Where(t => neighbours.Contains(t.TreeId)).Select(t => (double)t.CrownRadiusM), "F3"));
        }
    }

    /// <summary>Annual crown expansion r(year) − r(year − 1) of each tree living at both ends of the year, per run.</summary>
    private static void ExpansionRows(string tag, int year, MultiAreaSessionView controlBefore, MultiAreaSessionView controlAfter,
        MultiAreaSessionView treatedBefore, MultiAreaSessionView treatedAfter, IReadOnlySet<string> neighbours)
    {
        foreach (var (label, before, after) in new[] { ("control", controlBefore, controlAfter), ("treated", treatedBefore, treatedAfter) })
        {
            var start = before.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToDictionary(t => t.TreeId, StringComparer.Ordinal);
            var survivors = after.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving && start.ContainsKey(t.TreeId))
                .Select(t => (t.TreeId, Delta: (double)(t.CrownRadiusM - start[t.TreeId].CrownRadiusM))).ToArray();
            Console.WriteLine($"  {tag}|expansion|year {year}|{label}|A survivors|" +
                Distribution(survivors.Where(s => s.TreeId.StartsWith("MA-A", StringComparison.Ordinal)).Select(s => s.Delta), "+0.0000;-0.0000;0.0000"));
            Console.WriteLine($"  {tag}|expansion|year {year}|{label}|Fell-neighbour survivors|" +
                Distribution(survivors.Where(s => neighbours.Contains(s.TreeId)).Select(s => s.Delta), "+0.0000;-0.0000;0.0000"));
        }
    }

    private static (double CoverDeltaPp, double LightDelta, int ChangedA, int ChangedB) Row(string tag, string stage,
        MultiAreaContinuation control, MultiAreaContinuation treated)
    {
        var c = control.ReadCanopyLight();
        var t = treated.ReadCanopyLight();
        int changedA = 0, changedB = 0;
        double maxDelta = 0.0;
        foreach (var ta in t.Areas)
        {
            var ca = c.Areas.Single(x => x.AreaId == ta.AreaId);
            for (int i = 0; i < ta.Light.Cells.Count; i++)
            {
                double d = ta.Light.Cells[i].LightIndex - ca.Light.Cells[i].LightIndex;
                if (Math.Abs(d) > 1e-6) { if (ta.AreaId == A) changedA++; else changedB++; }
                maxDelta = Math.Max(maxDelta, Math.Abs(d));
            }
        }
        foreach (var (label, report) in new[] { ("control", c), ("treated", t) })
            foreach (var area in report.Areas)
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
                    "  {11}|{0}|{1}|{2}|stems {3}|BA {4:F2}|cover {5:F2} %|covered {12:F2} m2|uncovered {6:F2} m2|openings {7}|largest {8:F2} m2 / {9:F2} m wide|mean light {10:F4}",
                    stage, label, area.AreaId, area.Stand.LivingStems, area.Stand.BasalAreaM2PerHa, 100 * area.Cover.CoverFraction,
                    area.Cover.UncoveredM2, area.Cover.OpeningCount, area.Cover.LargestOpeningAreaM2, area.Cover.LargestOpeningWidthM,
                    area.Light.MeanLightIndex, tag, area.Cover.CoveredM2));
        var tA = t.Areas.Single(x => x.AreaId == A);
        var cA = c.Areas.Single(x => x.AreaId == A);
        double coverDelta = 100 * (tA.Cover.CoverFraction - cA.Cover.CoverFraction);
        double lightDelta = tA.Light.MeanLightIndex - cA.Light.MeanLightIndex;
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture,
            "  {7}|{0}|delta A|cover {1:+0.00;-0.00;0.00} pp|uncovered {2:+0.00;-0.00;0.00} m2|mean light {3:+0.0000;-0.0000;0.0000}|2.5 m cells changed A {4} B {5}|max |dlight| {6:F4}|delta B cover {8:+0.000;-0.000;0.000} pp",
            stage, coverDelta, tA.Cover.UncoveredM2 - cA.Cover.UncoveredM2, lightDelta, changedA, changedB, maxDelta, tag,
            100 * (t.Areas.Single(x => x.AreaId == B).Cover.CoverFraction - c.Areas.Single(x => x.AreaId == B).Cover.CoverFraction)));
        return (coverDelta, lightDelta, changedA, changedB);
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static readonly IReadOnlyList<CanopyVertex> Square = Array.AsReadOnly(new[]
    {
        new CanopyVertex(0, 0), new CanopyVertex(40, 0), new CanopyVertex(40, 40), new CanopyVertex(0, 40),
    });

    private static AnnualTree Tree(string id, float x, float z, float heightM, float radiusM) =>
        new(id, x, z, 30, 30f, heightM, radiusM);

    private static CanopyLightReport Evaluate(double grid, params AnnualTree[] trees) =>
        CanopyLightEvaluator.Evaluate(new[] { new CanopyArea("SQ", Square, 0.16f, trees) }, grid);

    private static CanopyLightCell CellAt(CanopyLightReport report, float x, float z) =>
        report.Areas[0].Light.Cells.Single(cell => cell.CentreXM == x && cell.CentreZM == z);

    private static AnnualTree[] Woodland(MultiAreaSessionView v) => CrownModel.Woodland(v.Areas.SelectMany(a => a.Trees));

    private static CanopyArea[] Inputs(MultiAreaSessionView v) => v.Areas.Select(a => new CanopyArea(a.AreaId,
        a.Polygon.Select(p => new CanopyVertex(p.XM, p.ZM)).ToArray(), a.AreaHa, a.Trees)).ToArray();

    /// <summary>Literal transcription of Unity ForestEcologyController.RecomputeCanopy (blob 0290a7be) for one cell.</summary>
    private static float UnityCell(float cx, float cz, AnnualTree[] trees, float cellSizeMeters)
    {
        const float CanopyShadeReachPerCrownRadius = 1f;
        float gap = 1f;
        foreach (var tree in trees)
        {
            float dx = cx - tree.XM, dz = cz - tree.ZM;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            float reach = tree.CrownRadiusM * CanopyShadeReachPerCrownRadius + cellSizeMeters * 0.5f;
            float lateral = Math.Clamp(1f - distance / reach, 0f, 1f);
            float vertical = Math.Clamp(tree.HeightM / 8f, 0f, 1f);
            float influence = Math.Clamp(lateral * vertical, 0f, 1f);
            gap *= 1f - influence;
        }
        float canopy = Math.Clamp(1f - gap, 0f, 1f);
        return 1f - canopy;
    }

    /// <summary>The superseded draft kernel (reach = r + cell/2), sampled at every cell centre of the 40 m square.</summary>
    private static double LegacyCoupledMean(AnnualTree[] trees, double cell)
    {
        double sum = 0;
        int n = 0;
        for (double x = cell / 2; x < 40; x += cell)
        for (double z = cell / 2; z < 40; z += cell)
        {
            sum += UnityCell((float)x, (float)z, trees, (float)cell);
            n++;
        }
        return sum / n;
    }

    /// <summary>A canonical text of every number in a read-out, for exact value comparison (records hold list references).</summary>
    private static string Canon(CanopyLightReport r)
    {
        var w = new System.Text.StringBuilder();
        w.Append(r.Metadata.Version).Append(r.Metadata.GridCellM).Append(r.Metadata.CrownRuleId).Append('\n');
        foreach (var t in r.Trees) w.Append(t).Append('\n');
        foreach (var a in r.Areas)
        {
            w.Append(a.AreaId).Append(a.PolygonAreaM2).Append(a.Stand).Append(a.Cover).Append('\n');
            w.Append(a.Light with { Cells = Array.Empty<CanopyLightCell>() }).Append('\n');
            foreach (var cell in a.Light.Cells) w.Append(cell).Append(F32.Hex(F32.Bits(cell.LightIndex))).Append('\n');
        }
        return w.ToString();
    }
}
