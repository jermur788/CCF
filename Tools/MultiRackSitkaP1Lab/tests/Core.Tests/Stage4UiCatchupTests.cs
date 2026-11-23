using System.Reflection;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// /multi Stage 1–4 UI catch-up (spec multi-stage4-ui-catchup-v1): execute-time Prune revalidation with whole-plan atomic
/// refusal (Stage 4 handoff §9 item 12), the 1/1/1 /multi default with the 0/0/0 path intact, the engine-authoritative UI
/// read-out (Stage 4 states, scheduled pruning eligibility, canopy summary, deadwood map positions, 2.5 m ground cells) and
/// the scheduled Prune command. Expected values come from independent recomputation where practical.
/// </summary>
internal static class Stage4UiCatchupTests
{
    private const string A = "MA-A";
    private const string B = "MA-B";
    private static readonly FieldInfo RootField =
        typeof(MultiAreaContinuation).GetField("root", BindingFlags.NonPublic | BindingFlags.Instance)!;

    internal static void Register()
    {
        T.Add("UI U-01: a Prune guard that turns false between Approve and Execute refuses the WHOLE plan with zero mutation", () =>
        {
            // Through public commands the guard cannot turn false after Approve: every tree, pruning or year change moves the
            // source token, so Execute refuses WorkPlanStaleSource first (and Year+1 cancels approved work). The drift is
            // therefore injected straight into the private root, keeping the token and plan identical, to reach the
            // revalidation step itself. Three guards are driven false in turn: max lifts, the 5-year interval, non-living.
            var drifts = new (string Code, Func<MultiAreaRoot, string, MultiAreaRoot> Drift)[]
            {
                ("PruneMaxLiftsReached", (root, id) => WithPruning(root, id, new TreePruningState(3, 2f, -1))),
                ("PruneIntervalTooShort", (root, id) => WithPruning(root, id, new TreePruningState(1, 2f, root.EcologicalYear))),
                ("PruneTreeNotLiving", (root, id) => WithDeath(root, id)),
            };
            foreach (var (code, drift) in drifts)
            {
                var (c, player, approved, fell, prune1, prune2) = ApprovedMixedPlan();
                var drifted = drift(Root(c), prune2);
                RootField.SetValue(c, drifted);
                var beforeView = c.ReadView();
                var beforeS4 = c.ReadStage4View();

                var refused = player.Execute(Fence(approved), approved.Approved!.PlanId, approved.Approved.ExecutionKey);
                T.Eq(code, refused.RefusalCode, code + ": stable refusal code");
                T.True(ReferenceEquals(drifted, Root(c)), code + ": nothing was published (the root reference is unchanged)");
                var after = refused.View;
                T.Eq(beforeView.Source, after.Source, code + ": same token (year, annual and work revisions)");
                T.Eq(beforeView.PlanStateRevision, after.PlanStateRevision, code + ": same plan-state revision");
                T.Eq(0, after.Source.EcologicalYear, code + ": the year did not advance");
                T.Eq(0, after.Events.Count, code + ": no work event");
                T.True(after.Approved is not null && after.Approved.PlanId == approved.Approved.PlanId &&
                    after.Approved.ExecutionKey == approved.Approved.ExecutionKey, code + ": the plan is still approved and identifiable");
                T.Eq(1, after.Approved!.Orders.Count, code + ": the approved Fell order is still there");
                var bt = beforeView.Areas.SelectMany(a => a.Trees).ToArray();
                var at = after.Areas.SelectMany(a => a.Trees).ToArray();
                for (int i = 0; i < bt.Length; i++)
                {
                    T.Eq(F32.Bits(bt[i].CrownRadiusM), F32.Bits(at[i].CrownRadiusM), code + ": crown radius of " + bt[i].TreeId);
                    T.Eq(bt[i], at[i], code + ": tree " + bt[i].TreeId);
                }
                T.True(Tree(after, fell).IsLiving, code + ": the approved Fell did not happen");
                var s4 = c.ReadStage4View();
                T.Eq(0, s4.PruneLog.Count, code + ": no lift record");
                T.Eq(2, s4.ApprovedPrunes.Count, code + ": both Prune orders are still approved");
                T.Eq(Lifts(beforeS4, prune1), Lifts(s4, prune1), code + ": the VALID Prune was not applied either");
                T.Eq(code, player.Execute(Fence(approved), approved.Approved.PlanId, approved.Approved.ExecutionKey).RefusalCode,
                    code + ": a retry gives the same refusal");
                T.True(ReferenceEquals(drifted, Root(c)), code + ": the retry published nothing");
            }

            // Control: the same plan without drift executes, so the refusal above came from the revalidated guard alone.
            var (c2, p2, ok, fell2, first, second) = ApprovedMixedPlan();
            var start = c2.ReadView();
            var done = p2.Execute(Fence(ok), ok.Approved!.PlanId, ok.Approved.ExecutionKey);
            T.True(done.RefusalCode is null, "control: the undrifted plan executes");
            T.True(!Tree(done.View, fell2).IsLiving, "control: the Fell happened");
            foreach (var id in new[] { first, second })
                T.Eq(F32.Bits(MathF.Max(0.1f, Tree(start, id).CrownRadiusM * 0.92f)), F32.Bits(Tree(done.View, id).CrownRadiusM), "control: " + id + " cut");
            T.Eq(0, done.View.Source.EcologicalYear, "control: the year did not advance");
        });

        T.Add("UI U-02: /multi opens 1/1/1 by default; an explicit 0/0/0 session stays the frozen path and refuses Prune", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var stage4 = MultiAreaWorkPlanAdapter.OpenSynthetic().ReadSnapshot();
            var m = stage4.Readout.Models;
            T.Eq((1, 1, 1), (m.CrownModelVersion, m.AdultModelVersion, m.MortalityModelVersion), "default /multi models");
            T.Eq("unity-k10a10-decoupled-v1", m.LightModelVersion, "light model version");
            T.Eq((2.5, 2.5), (m.LightCellM, m.ShadeBufferM), "2.5 m cells, b = 2.5 m");
            T.True(!m.CalibratedPar && m.NotCalibratedCaveat.Contains("NOT measured or calibrated PAR") &&
                m.LightLabel == "Modelled Light Index (Unity k10a10 proxy — not calibrated PAR)", "not-calibrated-PAR caveat and label");
            T.True(m.SyntheticStandDisclaimer.Contains("Experimental Demonstration"), "synthetic stand disclaimer");
            var reference = new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4).ReadView();
            T.True(stage4.View.Areas.SelectMany(a => a.Trees).SequenceEqual(reference.Areas.SelectMany(a => a.Trees)),
                "the default /multi trees are the Stage 4 genesis (C2 crown radii)");
            T.Eq(MultiAreaIdentity.AreaModelVersionStage4, stage4.View.Source.AreaModelVersion, "Stage 4 area model in the token");

            var legacy = MultiAreaWorkPlanAdapter.OpenSynthetic(MultiAreaModelSelection.Legacy);
            var frozen = new MultiAreaContinuation(e).ReadView();
            T.Eq(MultiAreaModelSelection.Legacy, new MultiAreaContinuation(e).SelectedModels, "the Core default ctor stays 0/0/0");
            var lv = legacy.Read();
            T.True(lv.Areas.SelectMany(a => a.Trees).SequenceEqual(frozen.Areas.SelectMany(a => a.Trees)) &&
                lv.Source.AreaModelVersion == frozen.Source.AreaModelVersion, "an explicit 0/0/0 adapter is the frozen model 0 session");
            var snapshot = legacy.ReadSnapshot();
            T.Eq((0, 0, 0), (snapshot.Readout.Models.CrownModelVersion, snapshot.Readout.Models.AdultModelVersion,
                snapshot.Readout.Models.MortalityModelVersion), "model 0 read-out reports 0/0/0");
            T.True(snapshot.Readout.Areas.SelectMany(a => a.Trees).All(t => !t.PruneEligible && t.PruneRefusalCode == "Stage4ModelRequired"),
                "no tree is prune-eligible on model 0, with the engine's reason");
            var tree = FirstLiving(lv, B);
            var refused = legacy.AddPrune(Fence(lv), B, tree);
            T.True(refused.RefusalCode == "Stage4ModelRequired" && refused.View.PlanStateRevision == lv.PlanStateRevision,
                "model 0 Prune is refused with a fresh, unchanged view");
        });

        T.Add("UI U-03: the read-out is the engine's: life states, RD, cover, deadwood map positions and ground cells", () =>
        {
            var c = StageFour();
            var v = c.ReadView();
            var fells = v.Areas[0].Trees.Where(t => t.IsLiving).Take(12).Select(t => t.TreeId).ToArray();
            foreach (var id in fells) v = c.AddPending(Fence(v), A, id, "UI U-03 thinning").View;
            v = c.Approve(Fence(v)).View;
            v = c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            for (int y = 0; y < 3; y++) v = c.Advance(Fence(v), false).View;
            var snap = c.ReadUiSnapshot();
            v = snap.View;
            var r = snap.Readout;
            var s4 = c.ReadStage4View();
            var canopy = c.ReadCanopyLight();
            T.Eq((v.Source, v.PlanStateRevision), (r.Source, r.PlanStateRevision), "read-out names the view's root");
            int deaths = 0;
            for (int i = 0; i < 2; i++)
            {
                var area = v.Areas[i];
                var ra = r.Areas[i];
                T.Eq(area.AreaId, ra.AreaId, "area order");
                T.Eq(area.Trees.Count, ra.Trees.Count, area.AreaId + " one read-out row per tree");
                for (int t = 0; t < area.Trees.Count; t++)
                {
                    var tree = area.Trees[t];
                    var row = ra.Trees[t];
                    var expected = tree.Death is null ? LifeState.Living : tree.Death.Cause == "fell" ? LifeState.FelledStump : LifeState.BiologicallyDead;
                    T.Eq(tree.TreeId, row.TreeId, "tree order");
                    T.Eq(expected, row.LifeState, tree.TreeId + " life state");
                    if (expected == LifeState.BiologicallyDead)
                        T.Eq((tree.Death!.Cause, (int?)tree.Death.EcologicalYear), (row.DeathCause!, row.DeathYear), tree.TreeId + " death cause/year");
                    if (!tree.IsLiving) T.True(!row.PruneEligible && row.PruneRefusalCode == "PruneTreeNotLiving", tree.TreeId + " dead: not prunable");
                }
                T.Eq(s4.Areas[i].RelativeDensity, ra.Canopy.RelativeDensity, area.AreaId + " RD");
                T.Eq((s4.Areas[i].AnnualNaturalDeaths, s4.Areas[i].CumulativeNaturalDeaths), (ra.Canopy.AnnualNaturalDeaths, ra.Canopy.CumulativeNaturalDeaths), "deaths");
                var cover = canopy.Areas[i].Cover;
                T.Eq((cover.CoverFraction, cover.UncoveredM2, cover.OpeningCount, cover.LargestOpeningAreaM2),
                    (ra.Canopy.GeometricCoverFraction, ra.Canopy.GeometricUncoveredM2, ra.Canopy.OpeningCount, ra.Canopy.LargestOpeningAreaM2), "cover");

                // Deadwood: every record, at the same map position as that tree's own map item.
                var scene = MultiAreaProjection.Scene(v, area);
                T.Eq(s4.Areas[i].Deadwood.Count, ra.Canopy.FallenDeadwood.Count, area.AreaId + " deadwood count");
                foreach (var d in ra.Canopy.FallenDeadwood)
                {
                    var item = scene.Single(x => x.TreeId == d.TreeId);
                    T.True(Math.Abs(item.U - d.U) < 1e-9 && Math.Abs(item.V - d.V) < 1e-9, d.DeadwoodId + " at its tree's map position");
                    deaths++;
                }

                // Ground cells: one per light cell; contributors reproduce the light index bit for bit; shape and cover check.
                var living = CrownModel.Woodland(v.Areas.SelectMany(a => a.Trees));
                var cells = canopy.Areas[i].Light.Cells;
                T.Eq(cells.Count, ra.Ground.Count, area.AreaId + " one ground cell per light cell");
                double coveredSum = 0.0, worst = 0.0, worstEdge = 0.0;
                var polygon = area.Polygon.Select(p => (p.XM, p.ZM)).ToArray();
                for (int k = 0; k < cells.Count; k++)
                {
                    var g = ra.Ground[k];
                    T.Eq((cells[k].Column, cells[k].Row, F32.Bits(cells[k].LightIndex)), (g.Column, g.Row, F32.Bits(g.LightIndex)), "cell identity and light");
                    var subset = living.Where(t => g.ContributingTreeIds.Contains(t.TreeId)).ToArray();
                    T.Eq(F32.Bits(CanopyLightEvaluator.LightIndexAt(g.CentreXM, g.CentreZM, living)),
                        F32.Bits(CanopyLightEvaluator.LightIndexAt(g.CentreXM, g.CentreZM, subset)), "contributors alone give the light index");
                    T.Eq(F32.Bits(g.LightIndex), F32.Bits(CanopyLightEvaluator.LightIndexAt(g.CentreXM, g.CentreZM, subset)), "cell light from contributors");
                    T.True(g.LightIndex < 1f == g.ContributingTreeIds.Count > 0, "shaded iff contributors exist");
                    T.True(Math.Abs(Shoelace(g.Polygon.Select(p => (p.XM, p.ZM)).ToArray()) - g.ClippedAreaM2) < 1e-9, "drawn shape area = clipped area");
                    T.True(g.Polygon.All(p => Math.Abs(p.U - (p.XM - area.Extent.MinXM)) < 1e-12 &&
                        Math.Abs(p.V - (area.Extent.MinZM + area.Extent.SideM - p.ZM)) < 1e-12), "map metres of the shape");
                    T.True(g.GeometricCoveredFraction >= 0 && g.GeometricCoveredFraction <= 1, "fraction in [0, 1]");
                    double oracle = SampledCover(polygon, living, g.Column, g.Row, 2.5);
                    double gap = Math.Abs(oracle - g.GeometricCoveredFraction);
                    // Whole cells must agree closely. A boundary cell counts micro-cells whose centre falls outside the rack as
                    // uncovered (Layer B's rule: the unsampled strip is not renormalised away), so it may read slightly lower.
                    if (g.ClippedAreaM2 == 6.25) worst = Math.Max(worst, gap);
                    else { worstEdge = Math.Max(worstEdge, gap); T.True(g.GeometricCoveredFraction <= oracle + 0.04, "edge cell never reads clearly higher"); }
                    coveredSum += g.GeometricCoveredFraction * g.ClippedAreaM2;
                }
                // 0.04: the engine resolves crown edges on Layer B's 0.25 m micro-grid (one micro-cell is 1 % of a 6.25 m² cell);
                // the oracle samples at 0.05 m. Observed worst whole-cell gaps are printed below.
                T.True(worst <= 0.04, area.AreaId + " whole-cell cover within 0.04 of an independent 0.05 m point sampler (worst " + worst.ToString("0.0000") + ")");
                T.True(worstEdge <= 0.15, area.AreaId + " edge-cell cover within 0.15 (worst " + worstEdge.ToString("0.0000") + ")");
                T.True(coveredSum <= cover.CoveredM2 + 1e-6, area.AreaId + " cell cover sums to no more than Layer B covered area");
                Console.WriteLine(FormattableString.Invariant(
                    $"  UI {area.AreaId}: cover {ra.Canopy.GeometricCoverFraction:0.0000}, open {ra.Canopy.GeometricUncoveredM2:0.0} m2, RD {ra.Canopy.RelativeDensity:0.000}, deaths {ra.Canopy.AnnualNaturalDeaths}/{ra.Canopy.CumulativeNaturalDeaths}, cells {ra.Ground.Count}, worst whole/edge cell-cover gap {worst:0.0000}/{worstEdge:0.0000}, cell-sum {coveredSum:0.00} of {cover.CoveredM2:0.00} m2"));
            }
            T.True(deaths > 0, "the run produced deadwood to check (" + deaths + ")");

            // A read-out is only paired with its own root.
            var old = v;
            var moved = c.MarkCrop(Fence(v), B, FirstLiving(v, B)).View;
            T.True(c.ReadUiReadout(old.Source, old.PlanStateRevision) is null, "a stale (source, plan-state) pair gets no read-out");
            T.True(c.ReadUiReadout(moved.Source, moved.PlanStateRevision) is not null, "the current pair does");
        });

        T.Add("UI U-04: scheduled Prune uses the Unity Sitka lifts 2.5/5/6.5 m; eligibility and reasons come from the engine", () =>
        {
            T.True(PruningRules.SitkaLiftTargetHeightsM.SequenceEqual(new[] { 2.5f, 5f, 6.5f }), "Unity ScenarioOne schedule");
            T.True(float.IsNaN(PruningRules.NextLiftTargetM(3)) && float.IsNaN(PruningRules.NextLiftTargetM(-1)), "no fourth lift");
            var e = MultiAreaGenerator.Generate();
            var c = new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4);
            var player = new MultiAreaWorkPlanAdapter(c, e);
            var v = player.Read();
            string id = FirstLiving(v, B);
            var row = Row(player, id);
            T.True(row.PruneEligible && row.PruneRefusalCode is null && row.NextPruneTargetM == 2.5f && row.PruningLifts == 0 &&
                row.MaxPruningLifts == 3, "an open B tree is eligible for lift 1 at 2.5 m");
            float r0 = Tree(v, id).CrownRadiusM;

            var pending = player.AddPrune(Fence(v), B, id);
            T.True(pending.RefusalCode is null, "queued");
            T.Eq(2.5f, c.ReadStage4View().PendingPrunes.Single().TargetCrownBaseHeightM, "the engine chose the 2.5 m target");
            T.Eq("PendingPrune", Row(player, id).PrunePlanState, "Pending Prune state");
            T.Eq(F32.Bits(r0), F32.Bits(Tree(pending.View, id).CrownRadiusM), "Pending changes no crown");
            T.Eq("WorkDuplicateTreeId", player.AddPrune(Fence(pending.View), B, id).RefusalCode, "one order per tree");
            var approved = player.Approve(Fence(pending.View));
            T.Eq("ApprovedPrune", Row(player, id).PrunePlanState, "Approved Prune state");
            T.Eq(F32.Bits(r0), F32.Bits(Tree(approved.View, id).CrownRadiusM), "Approve changes no crown");
            var done = player.Execute(Fence(approved.View), approved.View.Approved!.PlanId, approved.View.Approved.ExecutionKey);
            T.Eq(F32.Bits(MathF.Max(0.1f, r0 * 0.92f)), F32.Bits(Tree(done.View, id).CrownRadiusM), "Execute cuts 0.92 at once");
            T.Eq(0, done.View.Source.EcologicalYear, "year unchanged");
            row = Row(player, id);
            T.True(!row.PruneEligible && row.PruneRefusalCode == "PruneIntervalTooShort" && row.NextPruneEligibleYear == 5 &&
                row.NextPruneTargetM == 5f && row.PruneRefusalReason == PruningRules.Message("PruneIntervalTooShort") && row.PrunePlanState is null,
                "after lift 1: refused by the 5-year guard, with its reason and next eligible year");
            var refused = player.AddPrune(Fence(done.View), B, id);
            T.True(refused.RefusalCode == "PruneIntervalTooShort" && refused.View.PlanStateRevision == done.View.PlanStateRevision,
                "an ineligible Prune is refused with a fresh, unchanged view");
            v = done.View;
            for (int lift = 2; lift <= 3; lift++)
            {
                for (int y = 0; y < 5; y++) v = player.Advance(Fence(v), false).View;
                T.True(Row(player, id).PruneEligible && Row(player, id).NextPruneTargetM == PruningRules.SitkaLiftTargetHeightsM[lift - 1],
                    "eligible again for lift " + lift);
                v = player.AddPrune(Fence(v), B, id).View;
                v = player.Approve(Fence(v)).View;
                v = player.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            }
            for (int y = 0; y < 5; y++) v = player.Advance(Fence(v), false).View;
            row = Row(player, id);
            T.True(Tree(v, id).IsLiving && row.PruningLifts == 3 && !row.PruneEligible && row.PruneRefusalCode == "PruneMaxLiftsReached" &&
                row.NextPruneTargetM is null, "after three lifts: max-lifts refusal and no next target");
            T.Eq("2.5,5,6.5", string.Join(",", c.ReadStage4View().PruneLog.Select(p => p.TargetCrownBaseHeightM.ToString(System.Globalization.CultureInfo.InvariantCulture))),
                "the three scheduled targets were used");
            var removed = player.RemovePrune(Fence(v), B, id);
            T.Eq("UnknownWorkOrder", removed.RefusalCode, "remove without a pending Prune is refused");
        });
    }

    // ------------------------------------------------------------------------------------------- helpers

    private static MultiAreaContinuation StageFour() => new(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Stage4);

    private static MultiAreaRoot Root(MultiAreaContinuation c) => (MultiAreaRoot)RootField.GetValue(c)!;

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);

    private static AnnualTree Tree(MultiAreaSessionView v, string id) => v.Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == id);

    private static string FirstLiving(MultiAreaSessionView v, string areaId) =>
        v.Areas.Single(a => a.AreaId == areaId).Trees.First(t => t.IsLiving).TreeId;

    private static int Lifts(MultiAreaStage4View s4, string id) => s4.Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == id).PruningLifts;

    private static MultiAreaUiTree Row(MultiAreaWorkPlanAdapter player, string id) =>
        player.ReadSnapshot().Readout.Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == id);

    /// <summary>A Stage 4 session with one approved plan: a Fell in A and two Prunes in B (every guard valid at Approve).</summary>
    private static (MultiAreaContinuation, MultiAreaWorkPlanAdapter, MultiAreaSessionView, string Fell, string Prune1, string Prune2) ApprovedMixedPlan()
    {
        var e = MultiAreaGenerator.Generate();
        var c = new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4);
        var player = new MultiAreaWorkPlanAdapter(c, e);
        var v = player.Read();
        string fell = FirstLiving(v, A);
        var bTrees = v.Areas.Single(a => a.AreaId == B).Trees.Where(t => t.IsLiving).Select(t => t.TreeId).Take(2).ToArray();
        v = player.AddPending(Fence(v), A, fell, "UI U-01 Fell").View;
        v = player.AddPrune(Fence(v), B, bTrees[0]).View;
        v = player.AddPrune(Fence(v), B, bTrees[1]).View;
        var approved = player.Approve(Fence(v));
        T.True(approved.RefusalCode is null && approved.View.Approved is not null, "mixed plan approved");
        return (c, player, approved.View, fell, bTrees[0], bTrees[1]);
    }

    private static MultiAreaRoot WithPruning(MultiAreaRoot root, string treeId, TreePruningState state)
    {
        var pruning = new Dictionary<string, TreePruningState>(root.Stage4!.Pruning, StringComparer.Ordinal) { [treeId] = state };
        return root with { Stage4 = root.Stage4 with { Pruning = pruning } };
    }

    private static MultiAreaRoot WithDeath(MultiAreaRoot root, string treeId)
    {
        var areas = root.Areas.Select(area => area with
        {
            State = new AnnualState(area.State.EcologicalYear, Array.AsReadOnly(area.State.Trees.Select(t => t.TreeId == treeId
                ? t with { Death = new AnnualDeath(t.TreeId, root.EcologicalYear, LifeStates.DensitySuppression, "hazard") } : t).ToArray())),
        }).ToArray();
        return root with { Areas = Array.AsReadOnly(areas) };
    }

    private static double Shoelace(IReadOnlyList<(double X, double Z)> ring)
    {
        double sum = 0.0;
        for (int i = 0; i < ring.Count; i++) sum += ring[i].X * ring[(i + 1) % ring.Count].Z - ring[(i + 1) % ring.Count].X * ring[i].Z;
        return Math.Abs(sum) / 2.0;
    }

    /// <summary>
    /// Independent cover estimate of one light cell: points on a 0.05 m lattice inside the cell and strictly inside the
    /// polygon (its own half-plane test), covered when within any living crown. Approximates the clipped-area-weighted
    /// fraction; it shares no code with the engine's micro-grid.
    /// </summary>
    private static double SampledCover((double X, double Z)[] polygon, IReadOnlyList<AnnualTree> living, int column, int row, double cell)
    {
        const double step = 0.05;
        int n = (int)Math.Round(cell / step), inside = 0, covered = 0;
        for (int j = 0; j < n; j++)
        for (int i = 0; i < n; i++)
        {
            double x = column * cell + (i + 0.5) * step, z = row * cell + (j + 0.5) * step;
            bool inPolygon = true;
            for (int k = 0; k < polygon.Length && inPolygon; k++)
            {
                var a = polygon[k];
                var b = polygon[(k + 1) % polygon.Length];
                inPolygon = (b.X - a.X) * (z - a.Z) - (b.Z - a.Z) * (x - a.X) > 0;
            }
            if (!inPolygon) continue;
            inside++;
            if (living.Any(t => (x - t.XM) * (x - t.XM) + (z - t.ZM) * (z - t.ZM) <= (double)t.CrownRadiusM * t.CrownRadiusM)) covered++;
        }
        return inside == 0 ? 0.0 : (double)covered / inside;
    }
}
