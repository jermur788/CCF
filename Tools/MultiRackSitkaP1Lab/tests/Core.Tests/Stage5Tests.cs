using SitkaP1.Core;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Multi Stage 5 qualification (spec multi-stage5-sitka-regeneration-v1, requirements S5-01 … S5-10). Each test names the
/// criterion it observes. S5-07 (grid consistency) is the long-running study in <see cref="Stage5GridStudy"/>.
/// </summary>
internal static class Stage5Tests
{
    /// <summary>The 1/1/1/0 full-run fingerprint (S4 R-02 recipe), measured on base e7c1a57d before any Stage 5 edit.</summary>
    internal const string PinnedStage4FullRunHash = "8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb";

    /// <summary>
    /// The 1/1/1/1 full-run fingerprint (S5-10 D-01 recipe), printed by the approved Stage 5 head 5b757d9b and pinned by MULTI
    /// Phase 2 P2-01 (declared test change, closes reviewer nit R8).
    /// </summary>
    internal const string PinnedStage5FullRunHash = "c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35";

    private const MultiAreaRegenerationModelVersion Off = MultiAreaRegenerationModelVersion.Off;
    private const MultiAreaRegenerationModelVersion Regen1 = MultiAreaRegenerationModelVersion.SitkaCohortCompetitionBrowse;

    internal static void Register()
    {
        FrozenAndContracts();
        GapAndInterior();
        Undergrowth();
        ClearUndergrowth();
        Browsing();
        SeedRain();
        Promotion();
        AdultEmptyRack();
        DeterminismAndAtomicity();
        Stage5GridStudy.Register();
    }

    // ============================================================================================ S5-01 frozen paths
    private static void FrozenAndContracts()
    {
        T.Add("S5-01 F-01: Regeneration 0 is byte-identical: explicit 0/0/0/0, 0/0/1/0 and 1/1/1/0 reproduce their pinned fingerprints", () =>
        {
            T.Eq(Model0FingerprintTests.PinnedMultiAreaHash, Stage4Tests.Fingerprint(new MultiAreaModelSelection(AdultModelVersion.Legacy,
                MortalityModelVersion.Legacy, CrownModelVersion.LegacyFrozen, Off)), "0/0/0/0");
            T.Eq(Stage4Tests.PinnedStage3CrownModel1Hash, Stage4Tests.Fingerprint(new MultiAreaModelSelection(AdultModelVersion.Legacy,
                MortalityModelVersion.Legacy, CrownModelVersion.AreaScaledEquilibriumC2, Off)), "0/0/1/0");
            string full = Stage4BehaviourTests.RunFingerprint(Stage4Options.Default, new MultiAreaModelSelection(AdultModelVersion.ConnectedWoodland,
                MortalityModelVersion.RecordedDeadwood, CrownModelVersion.AreaScaledEquilibriumC2, Off));
            Console.WriteLine("  S5 1/1/1/0 full-run fingerprint " + full);
            T.Eq(PinnedStage4FullRunHash, full, "explicit 1/1/1/0 full run");
            T.Eq(PinnedStage4FullRunHash, Stage4BehaviourTests.RunFingerprint(Stage4Options.Default), "default Stage 4 recipe");
        });

        T.Add("S5-01 F-02: tokens keep every earlier lineage and append the regeneration version only when it is not 0 (Q3); Stage 5 state is null on earlier roots", () =>
        {
            var e = MultiAreaGenerator.Generate();
            T.Eq(MultiAreaIdentity.AreaModelVersion, new MultiAreaContinuation(e).ReadView().Source.AreaModelVersion, "0/0/0");
            T.Eq(MultiAreaIdentity.AreaModelVersionConnectedCrownC2,
                new MultiAreaContinuation(e, MultiAreaModelSelection.Stage3CrownC2).ReadView().Source.AreaModelVersion, "0/0/1");
            T.Eq(MultiAreaIdentity.AreaModelVersionStage4, new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4).ReadView().Source.AreaModelVersion, "1/1/1");
            var s5 = Session(e);
            T.Eq(MultiAreaIdentity.AreaModelVersionStage4 + "+regeneration-v1", s5.ReadView().Source.AreaModelVersion, "1/1/1/1");
            T.Eq(new MultiAreaContinuation(e).ReadView().Experiment.ManifestContentHash, s5.ReadView().Experiment.ManifestContentHash, "manifest hash unchanged");
            foreach (var selection in new[] { MultiAreaModelSelection.Legacy, MultiAreaModelSelection.Stage3CrownC2, MultiAreaModelSelection.Stage4 })
                T.True(Root(new MultiAreaContinuation(e, selection)).Stage5 is null, "no Stage 5 state on " + selection);
            T.True(Root(s5).Stage5 is not null && Root(s5).Stage4 is not null, "1/1/1/1 carries Stage 4 and Stage 5 state");
            var s4Trees = new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4).ReadView().Areas.SelectMany(a => a.Trees).ToArray();
            var s5Trees = s5.ReadView().Areas.SelectMany(a => a.Trees).ToArray();
            T.True(s4Trees.SequenceEqual(s5Trees), "year-0 adult rosters identical at 1/1/1/0 and 1/1/1/1");
        });

        T.Add("S5-01 F-03: unsupported selections are typed refusals (all seven incomplete Regeneration-1 selections); Stage 5 options, reads and ClearUndergrowth need 1/1/1/1", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var cw = AdultModelVersion.ConnectedWoodland;
            var rd = MortalityModelVersion.RecordedDeadwood;
            var c2 = CrownModelVersion.AreaScaledEquilibriumC2;
            foreach (var bad in new[]
            {
                new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, CrownModelVersion.LegacyFrozen, Regen1),
                new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, c2, Regen1),
                new MultiAreaModelSelection(AdultModelVersion.Legacy, rd, CrownModelVersion.LegacyFrozen, Regen1),
                new MultiAreaModelSelection(AdultModelVersion.Legacy, rd, c2, Regen1),
                new MultiAreaModelSelection(cw, MortalityModelVersion.Legacy, CrownModelVersion.LegacyFrozen, Regen1),
                new MultiAreaModelSelection(cw, MortalityModelVersion.Legacy, c2, Regen1),
                new MultiAreaModelSelection(cw, rd, CrownModelVersion.LegacyFrozen, Regen1),
            })
                T.Refuses("Stage5ModelIncomplete", () => new MultiAreaContinuation(e, bad));
            T.Refuses("RegenerationModelUnknown", () => new MultiAreaContinuation(e, new MultiAreaModelSelection(cw, rd, c2, (MultiAreaRegenerationModelVersion)7)));
            T.Refuses("Stage4ModelIncomplete", () => new MultiAreaContinuation(e, new MultiAreaModelSelection(cw, MortalityModelVersion.Legacy, c2, Off)));
            T.Refuses("Stage5ModelRequired", () => new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4, null, Stage5Options.Default));
            T.Refuses("CanopyGridUnsupported", () => Session(e, new Stage5Options { GridCellM = 1.0 }));
            T.Refuses("Stage5BrowsePressureInvalid", () => Session(e, new Stage5Options { BackgroundBrowsePressure = 1.5f }));
            var s4 = new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4);
            T.Refuses("Stage5ModelRequired", () => s4.ReadStage5View());
            T.Refuses("Stage5ModelRequired", () => s4.AddClearUndergrowth(Fence(s4.ReadView()), "MA-A", 5, 5));
            T.True(MultiAreaModelSelection.Stage5.IsStage4 && MultiAreaModelSelection.Stage5.IsStage5, "1/1/1/1 is Stage 4 plus Stage 5");
            T.True(MultiAreaModelSelection.Stage4.IsStage4 && !MultiAreaModelSelection.Stage4.IsStage5, "1/1/1/0 is Stage 4 only");
        });
    }

    // ============================================================================================ S5-02 dark interior vs managed gap
    private static void GapAndInterior()
    {
        T.Add("S5-02 G-01: same seed sources: a managed gap raises the light index; the dense unthinned interior never reaches promotion height, and its cohorts stay materially shorter and sparser than the gap's across the horizon (comparative, owner decision D2); the poor-light census is printed, never asserted", () =>
        {
            // Area A interior: the gap is every living tree within 7.5 m of (20, 15); the test cells are those whose centre is
            // within 5 m of that point, read in both sessions. Light is the modelled k10a10 index, not calibrated PAR.
            const float gx = 20f, gz = 15f;
            const int horizon = 15;
            // Margins fixed in the spec (second amendment record, D2) before this pass measured anything; they come from spec
            // constants, not from observed data:
            //   one maximal year of growth: 0.35 × the LightResponse peak 0.90 = 0.315 m (§3.2), so a lead of at least this much
            //   cannot be a one-year establishment offset;
            //   0.8, the §3.2 poor-light survival factor (the spec's own one-year suppression penalty);
            //   year 12, the first year a year-1 cohort can reach 3.5 m (0.15 + 11 × 0.315 = 3.615 m); year 15, the horizon end.
            const double maxAnnualGrowthM = 0.35 * 0.90;
            const double poorLightSurvival = 0.8;
            int[] materialYears = { 12, horizon };
            var dense = Session();
            var gap = Session();
            int felled = FellGap(gap, "MA-A", gx, gz, 7.5f);
            T.True(felled >= 10, "the gap removes a real group of stems (" + felled + ")");
            Stage5Cell[] Cells(MultiAreaContinuation c) => c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-A").Cells
                .Where(cell => (cell.CentreXM - gx) * (cell.CentreXM - gx) + (cell.CentreZM - gz) * (cell.CentreZM - gz) <= 25f).ToArray();
            var testCells = Cells(dense).Select(c => (c.Column, c.Row)).ToHashSet();
            T.True(testCells.Count > 0 && testCells.SetEquals(Cells(gap).Select(c => (c.Column, c.Row))), "the same test cells in both sessions");
            static double Tallest(Stage5Cell[] cells) => cells.SelectMany(c => c.Bands).Select(b => (double)b.Height).DefaultIfEmpty(0).Max();
            Stage5PromotionRecord[] Promotions(MultiAreaContinuation c) => c.ReadStage5View().Accounts.SelectMany(y => y.Areas)
                .SelectMany(a => a.Promotions).Where(p => p.AreaId == "MA-A" && testCells.Contains((p.Column, p.Row))).ToArray();

            var rows = new List<(int Year, double DenseTallest, double GapReached, double DenseAbundance, double GapAbundance, int Poor, double DenseLight)>();
            double gapLight1 = 0, denseLight1 = 0, gapTallest12 = 0;
            bool denseQualified = false;
            for (int year = 1; year <= horizon; year++)
            {
                Advance(dense);
                Advance(gap);
                var d = Cells(dense);
                var g = Cells(gap);
                denseQualified |= d.Any(c => c.Bands.Any(b => b.Height >= Stage5Constants.PromotionHeightM));
                if (year == 1) { gapLight1 = g.Average(c => (double)c.Light); denseLight1 = d.Average(c => (double)c.Light); }
                if (year == 12) gapTallest12 = Tallest(g);
                // A gap cohort that has promoted has left the bands; it counts at its promotion height (the height it reached).
                double gapReached = Math.Max(Tallest(g), Promotions(gap).Select(p => (double)p.HeightM).DefaultIfEmpty(0).Max());
                // Each cell's Light is the light its rules used in that year's step (the poor-light penalty reads exactly it).
                rows.Add((year, Tallest(d), gapReached, d.Sum(c => c.Abundance), g.Sum(c => c.Abundance),
                    d.Count(c => Stage5Juvenile.LightSurvival(c.Light) < 1.0), d.Average(c => (double)c.Light)));
            }
            int densePromotions = Promotions(dense).Length;
            Console.WriteLine($"  S5-02 cells {testCells.Count}, felled {felled}: light y1 dense {denseLight1:F3} gap {gapLight1:F3}; gap promotions from the test cells {Promotions(gap).Length}, dense {densePromotions}");
            Console.WriteLine("  S5-02 per year (dense tallest / gap tallest reached, m; dense / gap standing abundance; dense mean light): " +
                string.Join("; ", rows.Select(r => $"y{r.Year} {r.DenseTallest:F2}/{r.GapReached:F2} m, {r.DenseAbundance:F3}/{r.GapAbundance:F3}, {r.DenseLight:F3}")));
            Console.WriteLine("  S5-02 DIAGNOSTIC (printed, never asserted): poor-light census, dense test cells with LR < 0.15 (survival × 0.8) per year: " +
                string.Join(", ", rows.Select(r => $"y{r.Year} {r.Poor}/{testCells.Count}")) +
                $". Year 12: {rows[11].Poor}/{testCells.Count} dense test cells in the poor-light band (the interior's light rises as Area A self-thins).");
            foreach (int y in materialYears)
            {
                var r = rows[y - 1];
                Console.WriteLine($"  S5-02 year {y}: gap lead {r.GapReached - r.DenseTallest:F3} m (margin ≥ {maxAnnualGrowthM:F3} m); " +
                    $"dense / gap abundance {r.DenseAbundance / r.GapAbundance:F3} (margin ≤ {poorLightSurvival:F1})");
            }

            T.True(gapLight1 > denseLight1 + 0.2, "the gap raises the modelled light index");
            T.True(!denseQualified && densePromotions == 0, "the dense interior never reaches promotion height (3.5 m) nor promotes in 15 years");
            foreach (var r in rows)
            {
                T.True(r.DenseTallest <= r.GapReached, $"year {r.Year}: the dense interior is never taller than the gap ({r.DenseTallest:F3} vs {r.GapReached:F3} m)");
                T.True(r.DenseAbundance <= r.GapAbundance, $"year {r.Year}: the dense interior is never more abundant than the gap ({r.DenseAbundance:F4} vs {r.GapAbundance:F4})");
            }
            foreach (int y in materialYears)
            {
                var r = rows[y - 1];
                T.True(r.GapReached - r.DenseTallest >= maxAnnualGrowthM,
                    $"year {y}: materially shorter: the gap leads by at least one maximal year of growth, 0.315 m ({r.GapReached - r.DenseTallest:F3} m)");
                T.True(r.DenseAbundance <= poorLightSurvival * r.GapAbundance,
                    $"year {y}: materially sparser: dense abundance at most 0.8 × the gap's ({r.DenseAbundance:F4} vs {r.GapAbundance:F4})");
            }
            T.True(gapTallest12 >= 1.5, "gap cohorts grow past the bramble/bracken escape height (1.5 m) toward promotion by year 12");
        });
    }

    // ============================================================================================ S5-03 bramble and bracken
    private static void Undergrowth()
    {
        T.Add("S5-03 U-01: bramble and bracken start at 0.25·T and track light with gain 0.30 / loss 0.45, exactly", () =>
        {
            static float Clamp01(float v) => v < 0f ? 0f : v > 1f ? 1f : v;
            // The spec expression in Unity's float order, written here independently: T = 0.8 · 1 · (0.1 + (1 − 0.1) · light).
            static float Target(float light) => 0.8f * Clamp01(1f * 1f) * (0.1f + (1f - 0.1f) * Clamp01(light));
            var c = Session();
            var g = c.ReadStage5View();
            int cells = 0;
            foreach (var cell in g.Areas.SelectMany(a => a.Cells))
            {
                T.Eq(F32.Bits(Target(cell.Light) * 0.25f), F32.Bits(cell.Bramble), "genesis bramble = 0.25·T");
                T.Eq(F32.Bits(cell.Bramble), F32.Bits(cell.Bracken), "genesis bracken = bramble");
                T.True(Math.Abs(TargetOracle(cell.Light) * 0.25 - cell.Bramble) < 1e-6, "double oracle agrees");
                cells++;
            }
            Advance(c);
            var y1 = c.ReadStage5View();
            int gains = 0, losses = 0;
            for (int a = 0; a < g.Areas.Count; a++)
            for (int i = 0; i < g.Areas[a].Cells.Count; i++)
            {
                var before = g.Areas[a].Cells[i];
                var after = y1.Areas[a].Cells[i];
                float t = Target(after.Light);
                float rate = t > before.Bramble ? 0.30f : 0.45f;
                if (t > before.Bramble) gains++; else losses++;
                T.Eq(F32.Bits(Clamp01(before.Bramble + (t - before.Bramble) * Clamp01(rate))), F32.Bits(after.Bramble), "bramble step");
                T.Eq(F32.Bits(Clamp01(before.Bracken + (t - before.Bracken) * Clamp01(rate))), F32.Bits(after.Bracken), "bracken step");
            }
            // The loss branch on a treeless pure step: cover 0.9 above the full-light target 0.8 moves 45 % of the way down.
            var s = Stage5Annual.Genesis(Stage5Options.Default, 1, EmptySquares());
            s = WithCell(s, "MA-A", 0, cell => cell with { Bramble = 0.9f, Bracken = 0.1f });
            var stepped = Stage5Annual.Step(s, EmptySquares(), 1).Next.Areas[0].Cells[0];
            T.Eq(F32.Bits(Clamp01(0.9f + (Target(1f) - 0.9f) * 0.45f)), F32.Bits(stepped.Bramble), "loss 0.45 above target");
            T.Eq(F32.Bits(Clamp01(0.1f + (Target(1f) - 0.1f) * 0.30f)), F32.Bits(stepped.Bracken), "gain 0.30 below target");
            Console.WriteLine($"  S5-03 genesis cells {cells}; year-1 steps: {gains} gain, {losses} loss");
        });

        T.Add("S5-03 U-02: p_veg = 0.35·max(bramble, bracken)·heightVuln (full ≤ 0.3 m, zero ≥ 1.5 m) and it enters band survival", () =>
        {
            T.Eq(F32.Bits(0.35f * 0.6f * 1f), F32.Bits(Stage5Undergrowth.VegetationLossProbability(0.6f, 0.2f, 0.2f)), "full at 0.2 m, uses the max");
            T.Eq(F32.Bits(0.35f * 0.6f), F32.Bits(Stage5Undergrowth.VegetationLossProbability(0.2f, 0.6f, 0.3f)), "full at exactly 0.3 m");
            T.Eq(0f, Stage5Undergrowth.VegetationLossProbability(0.9f, 0.9f, 1.5f), "zero at 1.5 m");
            T.Eq(0f, Stage5Undergrowth.VegetationLossProbability(0.9f, 0.9f, 3f), "zero above 1.5 m");
            T.True(Math.Abs(0.35 * 0.8 * 0.5 - Stage5Undergrowth.VegetationLossProbability(0.8f, 0.1f, 0.9f)) < 1e-6, "half-way at 0.9 m");
            // In a band: identical cells except cover; a short band pays (1 − p_veg), a band ≥ 1.5 m does not.
            var s = Stage5Annual.Genesis(Stage5Options.Default, 1, EmptySquares());
            var bare = WithCell(s, "MA-A", 0, cell => cell with { Bramble = 0f, Bracken = 0f, Bands = new[] { Band(0.1f, 0.2f, 1), Band(0.1f, 2f, 2) } });
            var dense = WithCell(s, "MA-A", 0, cell => cell with { Bramble = 0.8f, Bracken = 0.8f, Bands = new[] { Band(0.1f, 0.2f, 1), Band(0.1f, 2f, 2) } });
            var b = Stage5Annual.Step(bare, EmptySquares(), 5).Next.Areas[0].Cells[0];
            var d = Stage5Annual.Step(dense, EmptySquares(), 5).Next.Areas[0].Cells[0];
            // Oracle from the spec: light 1 → LR 0.8 (≥ 0.15, light survival 1); p = 0.2 · 0.15 · V(0.2 m) = 0.03.
            double p = 0.2 * 0.15;
            double coverBare = 0 + (0.8 - 0) * 0.30, coverDense = 0.8;
            double expectBare = 0.1 * (1 - 0.04 * p) * (1 - 0.35 * coverBare);
            double expectDense = 0.1 * (1 - 0.04 * p) * (1 - 0.35 * coverDense);
            T.True(Math.Abs(expectBare - b.Bands[0].Density) < 1e-6 && Math.Abs(expectDense - d.Bands[0].Density) < 1e-6, "short band survival oracle");
            T.True(d.Bands[0].Density < b.Bands[0].Density, "more cover, more loss");
            T.Eq(F32.Bits(b.Bands[1].Density), F32.Bits(d.Bands[1].Density), "a 2 m band takes no bramble/bracken loss");
            T.True(Math.Abs(0.1 - d.Bands[1].Density) < 1e-7, "and no browse loss above 0.8 m");
        });
    }

    // ============================================================================================ S5-04 ClearUndergrowth
    private static void ClearUndergrowth()
    {
        T.Add("S5-04 C-01: Pending and Approve change nothing; Execute zeroes bramble and bracken, keeps every band, never advances the year; replay is idempotent", () =>
        {
            var c = Session();
            ThinEverySecond(c);
            for (int y = 0; y < 6; y++) Advance(c);
            var target = c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-B").Cells.First(cell => cell.Bands.Count > 0 && cell.Bramble > 0f);
            var other = c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-A").Cells.First(cell => cell.Bramble > 0f);
            var s0 = c.ReadStage5View();
            var v0 = c.ReadView();
            var v1 = c.AddClearUndergrowth(Fence(v0), "MA-B", target.Column, target.Row).View;
            var s1 = c.ReadStage5View();
            T.True(ReferenceEquals(s0.Areas, s1.Areas), "Pending: no cell changed");
            T.Eq(v0.Source, v1.Source, "Pending: same token, same year");
            T.Eq(v0.PlanStateRevision + 1, v1.PlanStateRevision, "Pending moves the plan-state revision only");
            T.Eq(1, s1.PendingClears.Count, "one pending clear");
            T.Refuses("ClearDuplicateCell", () => c.AddClearUndergrowth(Fence(v1), "MA-B", target.Column, target.Row));
            var v2 = c.Approve(Fence(v1)).View;
            var s2 = c.ReadStage5View();
            T.True(ReferenceEquals(s0.Areas, s2.Areas) && s2.PendingClears.Count == 0 && s2.ApprovedClears.Count == 1, "Approve: nothing ecological changed");
            T.Eq(v0.Source.EcologicalYear, v2.Source.EcologicalYear, "Approve: same year");
            var executed = c.Execute(Fence(v2), v2.Approved!.PlanId, v2.Approved.ExecutionKey);
            var s3 = c.ReadStage5View();
            var cleared = s3.CellOf("MA-B", target.Column, target.Row)!;
            T.Eq(0f, cleared.Bramble, "bramble cleared");
            T.Eq(0f, cleared.Bracken, "bracken cleared");
            T.True(ReferenceEquals(target.Bands, cleared.Bands), "every Sitka band kept (selective clearing)");
            T.Eq(F32.Bits(other.Bramble), F32.Bits(s3.CellOf("MA-A", other.Column, other.Row)!.Bramble), "other cells untouched");
            T.Eq(v0.Source.EcologicalYear, executed.View.Source.EcologicalYear, "Execute never advances the year");
            T.Eq(v0.Source.AnnualRevision, executed.View.Source.AnnualRevision, "no annual revision");
            T.Eq(v0.Source.WorkRevision + 1, executed.View.Source.WorkRevision, "one work revision");
            T.True(v0.Areas.SelectMany(a => a.Trees).SequenceEqual(executed.View.Areas.SelectMany(a => a.Trees)), "no tree changed");
            var record = s3.ClearLog.Single();
            T.Eq((target.Bramble, target.Bracken, target.Bands.Count), (record.BrambleBefore, record.BrackenBefore, record.BandsKept), "clear record");
            T.Eq(v2.Approved.ExecutionKey, record.ExecutionKey, "engine-minted key recorded");
            var rootAfter = Root(c);
            var replay = c.Execute(Fence(v2), v2.Approved.PlanId, v2.Approved.ExecutionKey);
            T.True(replay.Execution!.Replayed && ReferenceEquals(rootAfter, Root(c)), "identical retry: receipt only, nothing published");
            // Temporary: the next year regrows cover from 0 toward T at gain 0.30.
            Advance(c);
            var regrown = c.ReadStage5View().CellOf("MA-B", target.Column, target.Row)!;
            float t = Stage5Undergrowth.Target(regrown.Light);
            T.Eq(F32.Bits(0f + (t - 0f) * 0.30f), F32.Bits(regrown.Bramble), "regrowth after one year");
        });

        T.Add("S5-04 C-02: stale fences, invalid cells and a drifted batch refuse the WHOLE plan with zero mutation; consented Year+1 cancels clears", () =>
        {
            var c = Session();
            var v = c.ReadView();
            var aCell = c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-A").Cells[10];
            var bCell = c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-B").Cells[10];
            T.Refuses("MultiAreaUnknownArea", () => c.AddClearUndergrowth(Fence(v), "MA-Z", aCell.Column, aCell.Row));
            T.Refuses("ClearCellNotInArea", () => c.AddClearUndergrowth(Fence(v), "MA-A", 0, 0));
            T.Refuses("ClearCellNotInArea", () => c.AddClearUndergrowth(Fence(v), "MA-A", 9999, 9999));
            T.Refuses("MultiAreaWrongArea", () => c.AddClearUndergrowth(Fence(v), "MA-A", bCell.Column, bCell.Row));
            T.Refuses("PlayerPlanStateStale", () => c.AddClearUndergrowth(new MultiAreaFence(v.SessionId, v.Source, v.PlanStateRevision + 1), "MA-A", aCell.Column, aCell.Row));
            T.Eq(v.PlanStateRevision, c.ReadView().PlanStateRevision, "refusals changed nothing");

            // A valid mixed plan (one Fell, two clears) whose clear list drifts in the private root, keeping token and plan.
            var fell = v.Areas.Single(a => a.AreaId == "MA-B").Trees.First(t => t.IsLiving).TreeId;
            v = c.AddPending(Fence(v), "MA-B", fell, "S5-04 mixed plan").View;
            v = c.AddClearUndergrowth(Fence(v), "MA-A", aCell.Column, aCell.Row).View;
            v = c.AddClearUndergrowth(Fence(v), "MA-B", bCell.Column, bCell.Row).View;
            v = c.Approve(Fence(v)).View;
            foreach (var (code, drift) in new (string, MultiAreaClearUndergrowthOrder)[]
            {
                ("ClearCellNotInArea", new MultiAreaClearUndergrowthOrder("MA-A", 9999, 9999)),
                ("MultiAreaWrongArea", new MultiAreaClearUndergrowthOrder("MA-A", bCell.Column, bCell.Row)),
                ("ClearDuplicateCell", new MultiAreaClearUndergrowthOrder("MA-A", aCell.Column, aCell.Row)),
                ("MultiAreaUnknownArea", new MultiAreaClearUndergrowthOrder("MA-Z", 1, 1)),
            })
            {
                var original = Root(c);
                var drifted = original with
                {
                    Stage5 = original.Stage5! with { ApprovedClears = Array.AsReadOnly(original.Stage5.ApprovedClears.Append(drift).ToArray()) },
                };
                RootField.SetValue(c, drifted);
                var refused = MultiAreaWorkPlanCall(() => c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey));
                T.Eq(code, refused, code + ": whole batch refused");
                T.True(ReferenceEquals(drifted, Root(c)), code + ": nothing published");
                T.True(c.ReadView().Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == fell).IsLiving, code + ": the Fell did not happen");
                T.True(c.ReadStage5View().CellOf("MA-A", aCell.Column, aCell.Row)!.Bramble > 0f, code + ": the valid clear did not happen");
                RootField.SetValue(c, original);
            }
            // Stale fence after a real change: the approved plan cannot execute against another source.
            var stale = Fence(v);
            var advanced = c.Advance(Fence(c.ReadView()), consentToCancelWork: true).View;
            T.Eq(0, c.ReadStage5View().ApprovedClears.Count + c.ReadStage5View().PendingClears.Count, "consented Year+1 cancels every clear order");
            T.Refuses("PlayerSourceStale", () => c.Execute(stale, v.Approved!.PlanId, v.Approved.ExecutionKey));
            T.Eq(0, c.ReadStage5View().ClearLog.Count, "no clear ever executed");
            T.True(advanced.Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == fell).IsLiving, "cancelled Fell never happened");
            // Unexecuted clears alone need consent to advance.
            var w = c.AddClearUndergrowth(Fence(c.ReadView()), "MA-A", aCell.Column, aCell.Row).View;
            T.Refuses("WorkPendingConsentRequired", () => c.Advance(Fence(w), false));
        });
    }

    private static string? MultiAreaWorkPlanCall(Action action)
    {
        try { action(); return null; }
        catch (P1Refusal e) { return e.Code; }
    }

    // ============================================================================================ S5-05 browsing
    private static void Browsing()
    {
        T.Add("S5-05 B-01: pressure 0 is bit-identical to light-only; 0.2 gives p ≤ 0.03, a 0.90·p height slowdown and a 0.04·p survival dip; ≥ 0.8 m takes no browse loss", () =>
        {
            T.Eq(F32.Bits(0.2f * 0.15f * 1f), F32.Bits(Stage5Juvenile.BrowseProbability(0.2f, 0.4f)), "p at full vulnerability");
            // The spec's 0.03 is the real product 0.2 × 0.15; Unity's float expression 0.2f × 0.15f rounds to 0.030000001.
            T.True(Stage5Juvenile.BrowseProbability(0.2f, 0.1f) <= 0.03 + 1e-7, "p ≤ 0.03 at pressure 0.2 (float32 rounding only)");
            T.True(Math.Abs(0.03 * 0.5 - Stage5Juvenile.BrowseProbability(0.2f, 0.65f)) < 1e-7, "V tapers linearly 0.5 → 0.8 m");
            T.Eq(0f, Stage5Juvenile.BrowseProbability(0.2f, 0.8f), "no browse at the escape height");
            T.Eq(0f, Stage5Juvenile.BrowseProbability(1f, 2f), "no browse above it, any pressure");
            T.True(Stage5Juvenile.BrowseProbability(1f, 0.1f) <= 0.95f, "cap 0.95 holds");
            for (float light = 0f; light <= 1.0001f; light += 0.05f)
            {
                float h = 0.3f;
                T.Eq(F32.Bits(h + 0.35f * Stage5Juvenile.LightResponse(light) * 1f), F32.Bits(Stage5Juvenile.GrowHeight(h, light, 0f)), "p = 0 light-only height");
                T.Eq(BitConverter.DoubleToUInt64Bits(Stage5Juvenile.LightSurvival(light)),
                    BitConverter.DoubleToUInt64Bits(Stage5Juvenile.LightBrowseSurvival(light, 0f)), "p = 0 light-only survival");
                T.True(Math.Abs(LightResponseOracle(light) - Stage5Juvenile.LightResponse(light)) < 1e-6, "LR oracle at " + light);
            }
            // Pure step, two identical cells except the pressure.
            var opts0 = new Stage5Options { BackgroundBrowsePressure = 0f };
            var opts2 = new Stage5Options { BackgroundBrowsePressure = 0.2f };
            Stage5Band[] Bands() => new[] { Band(0.1f, 0.3f, 1), Band(0.1f, 0.65f, 2), Band(0.1f, 0.9f, 3) };
            var s0 = WithCell(Stage5Annual.Genesis(opts0, 1, EmptySquares()), "MA-A", 0, cell => cell with { Bands = Bands() });
            var s2 = WithCell(Stage5Annual.Genesis(opts2, 1, EmptySquares()), "MA-A", 0, cell => cell with { Bands = Bands() });
            var a0 = Stage5Annual.Step(s0, EmptySquares(), 7);
            var a2 = Stage5Annual.Step(s2, EmptySquares(), 7);
            var c0 = a0.Next.Areas[0].Cells[0];
            var c2 = a2.Next.Areas[0].Cells[0];
            T.True(c0.Bands.All(b => b.LastBrowsedFraction == 0f) && a0.Account.Areas.Sum(a => a.BrowseLoss) == 0, "pressure 0: no browse at all");
            float lr = Stage5Juvenile.LightResponse(1f);
            T.Eq(F32.Bits(0.3f + 0.35f * lr * 1f), F32.Bits(c0.Bands[0].Height), "pressure 0: light-only height, bit for bit");
            double p = 0.2 * 0.15;
            T.True(Math.Abs(0.3 + 0.35 * 0.8 * (1 - 0.9 * p) - c2.Bands[0].Height) < 1e-6, "0.2: increment × (1 − 0.90·p)");
            T.True(Math.Abs(c0.Bands[0].Density * (1 - 0.04 * p) - c2.Bands[0].Density) < 1e-7, "0.2: survival × (1 − 0.04·p)");
            T.True(c2.Bands[1].Height < c0.Bands[1].Height && c2.Bands[1].Height > c2.Bands[0].Height, "partial vulnerability at 0.65 m");
            T.Eq(F32.Bits(c0.Bands[2].Height), F32.Bits(c2.Bands[2].Height), "0.9 m band: same height under browsing");
            T.Eq(F32.Bits(c0.Bands[2].Density), F32.Bits(c2.Bands[2].Density), "0.9 m band: same survival under browsing");
            T.Eq(0f, c2.Bands[2].LastBrowsedFraction, "0.9 m band browse fraction 0");
            // Whole sessions: pressure 0 never records a browse loss; the default 0.2 does.
            var none = Session(null, opts0);
            var some = Session();
            for (int y = 0; y < 5; y++) { Advance(none); Advance(some); }
            T.Eq(0.0, none.ReadStage5View().Accounts.Sum(y => y.Areas.Sum(a => a.BrowseLoss)), "session at pressure 0");
            T.True(some.ReadStage5View().Accounts.Sum(y => y.Areas.Sum(a => a.BrowseLoss)) > 0, "session at pressure 0.2");
        });
    }

    // ============================================================================================ S5-06 cross-rack seed rain
    private static void SeedRain()
    {
        T.Add("S5-06 S-01: mature Sitka in A seed B and vice versa; zero seed from outer grassland; one shared mast roll per year", () =>
        {
            var mature = new[] { (5_000, 5_000, 40, 300, 20_000), (5_000, 15_000, 40, 320, 21_000), (10_000, 10_000, 45, 350, 22_000) };
            var young = new[] { (30_000, 10_000, 5, 40, 2_000) };
            var mirror = mature.Select(t => (40_000 - t.Item1, t.Item2, t.Item3, t.Item4, t.Item5)).ToArray();
            var youngA = young.Select(t => (40_000 - t.Item1, t.Item2, t.Item3, t.Item4, t.Item5)).ToArray();
            foreach (var (name, e, _, sink) in new[]
            {
                ("A seeds B", TwoSquares("mad-s5-seed-ab", mature, young), "MA-A", "MA-B"),
                ("B seeds A", TwoSquares("mad-s5-seed-ba", youngA, mirror), "MA-B", "MA-A"),
            })
            {
                var c = Session(e, new Stage5Options { MastSeed = 11 });
                Advance(c);
                var v = c.ReadView();
                var s5 = c.ReadStage5View();
                var year = s5.Accounts.Single();
                var sinkAccount = year.Areas.Single(a => a.AreaId == sink);
                // Oracle: Σ_i M(age)·clamp01(r/4)·mast·exp(−d/20) over the living post-step trees of BOTH racks, d ≤ 150 m.
                var trees = v.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
                double expected = 0;
                foreach (var cell in s5.Areas.Single(a => a.AreaId == sink).Cells)
                    foreach (var t in trees)
                    {
                        double maturity = t.AgeYears <= 20 ? 0 : t.AgeYears >= 30 ? 1 : (t.AgeYears - 20) / 10.0;
                        double d = Math.Sqrt(Math.Pow(cell.CentreXM - t.XM, 2) + Math.Pow(cell.CentreZM - t.ZM, 2));
                        if (d <= 150) expected += maturity * Math.Clamp(t.CrownRadiusM / 4.0, 0, 1) * year.MastMultiplier * Math.Exp(-d / 20.0);
                    }
                T.True(sinkAccount.SeedArrival > 0 && Math.Abs(expected - sinkAccount.SeedArrival) <= 1e-5 * expected,
                    name + $": the sink rack receives the cross-rack seed rain (engine {sinkAccount.SeedArrival:R}, oracle {expected:R})");
                T.True(s5.Areas.Single(a => a.AreaId == sink).Cells.Any(cell => cell.Bands.Count > 0), name + ": seed establishes in the sink rack");
            }
            // Only young stems (M = 0): nothing seeds; the open grassland beyond the perimeter adds nothing.
            var none = Session(TwoSquares("mad-s5-seed-none", new[] { (5_000, 5_000, 5, 40, 2_000) }, young), new Stage5Options { MastSeed = 11 });
            for (int y = 0; y < 3; y++) Advance(none);
            var n5 = none.ReadStage5View();
            T.Eq(0.0, n5.Accounts.Sum(y => y.Areas.Sum(a => a.SeedArrival)), "zero seed without a mature tree");
            T.True(n5.Areas.All(a => a.Cells.All(cell => cell.Bands.Count == 0)), "and no regeneration");
            // One shared mast roll per year on the isolated stream.
            var m = Session(null, new Stage5Options { MastSeed = 4242 });
            for (int y = 0; y < 12; y++) Advance(m);
            foreach (var year in m.ReadStage5View().Accounts)
            {
                var expected = Stage5Mast.ForYear(4242, year.YearAfter);
                T.Eq((expected.Roll, expected.Label, expected.Multiplier), (year.MastRoll, year.MastLabel, year.MastMultiplier), "mast " + year.YearAfter);
                T.Eq(year.MastRoll < 0.15f ? "good" : year.MastRoll < 0.5 ? "poor" : "normal", year.MastLabel, "Q1 thresholds");
            }
            T.Eq(12, m.ReadStage5View().Accounts.Select(y => y.YearAfter).Distinct().Count(), "one roll per year");
        });

        T.Add("S5-06 S-02: a Stage 5 year is identical under reversed area processing order", () =>
        {
            var c = Session();
            ThinEverySecond(c);
            for (int y = 0; y < 14; y++) Advance(c);
            var v = c.ReadView();
            var state = StateOf(c.ReadStage5View());
            var forward = Stage5Annual.Step(state, Inputs(v), v.Source.EcologicalYear + 1);
            var reversed = Stage5Annual.Step(state, Inputs(v).Reverse().ToArray(), v.Source.EcologicalYear + 1);
            T.Eq(Hash(c, forward), Hash(c, reversed), "same Stage 5 result");
            T.True(forward.Account.Areas.Sum(a => a.EstablishmentAccepted) > 0, "the compared year has real establishment");
            foreach (var area in v.Areas)
                T.True(forward.PromotedTrees[area.AreaId].SequenceEqual(reversed.PromotedTrees[area.AreaId]), "same promotions in " + area.AreaId);
        });
    }

    private static string Hash(MultiAreaContinuation c, Stage5AnnualResult r)
    {
        var w = new Model0FingerprintTests.Fingerprint();
        var s = c.ReadStage5View();
        Write(w, s with { Areas = r.Next.Areas, Accounts = r.Next.Accounts, EcologicalYear = r.Account.YearAfter });
        foreach (var (areaId, trees) in r.PromotedTrees.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            w.Text("promoted " + areaId);
            w.Trees(trees);
        }
        return w.Hash();
    }

    // ============================================================================================ S5-08 promotion
    private static void Promotion()
    {
        T.Add("S5-08 P-01: the oldest qualifying band (≥ 3.5 m) becomes one adult with Age, Height, DBH = clamp(1.5·H, 2, 20) and the CrownModel 1 radius r_0, not r_pot", () =>
        {
            var scale = Stage5CellScale.For(2.5);
            var bands = new List<Stage5BandWork>
            {
                Stage5BandWork.From(Band(0.1f, 3.0f, 2)), Stage5BandWork.From(Band(0.1f, 3.6f, 3)),
                Stage5BandWork.From(Band(0.1f, 3.7f, 5)), Stage5BandWork.From(Band(0.001f, 4.0f, 1)),
            };
            T.Eq(3, Stage5Promotion.OldestQualifyingBand(bands, 0.02f, scale)!.EstablishYear, "oldest represented band ≥ 3.5 m; no light gate");
            T.Eq((0, 0), Stage5Promotion.BlockOf(1, 1, 2.5), "2.5 m cells 0–1 are block 0");
            T.Eq((1, 3), Stage5Promotion.BlockOf(2, 7, 2.5), "2.5 m block");
            T.Eq((4, 9), Stage5Promotion.BlockOf(4, 9, 5.0), "a 5 m cell is its own block");
            var neighbour = new AnnualTree("MA-A-001", 11f, 10f, 40, 30f, 20f, 3f);
            var (tree, ci) = Stage5Promotion.NewAdult("R20-MA-A-4-4", 10f, 10f, 20, 3, 3.9f, new[] { neighbour });
            float dbh = Math.Clamp(3.9f * 1.5f, 2f, 20f);
            double ciOracle = (30.0 / Math.Max(1.0, dbh)) / Math.Max(0.5, 1.0);
            double rPot = Math.Max(0.1, 0.9415 + 0.07635 * dbh);
            T.Eq((17, F32.Bits(3.9f), F32.Bits(dbh)), (tree.AgeYears, F32.Bits(tree.HeightM), F32.Bits(tree.DbhCm)), "age, height, DBH");
            T.True(Math.Abs(ciOracle - ci) < 1e-5, "woodland Hegyi CI at the new stem");
            T.True(Math.Abs(Math.Max(0.1, rPot / Math.Sqrt(1 + ciOracle / 5.0)) - tree.CrownRadiusM) < 1e-5, "r_0 = r_pot / sqrt(1 + CI/5)");
            T.True(tree.CrownRadiusM < rPot - 0.05, "r_0 is not the open-grown r_pot");
            T.Eq(1, Stage5Promotion.NewAdult("x", 1, 1, 5, 5, 3.6f, Array.Empty<AnnualTree>()).Tree.AgeYears, "Age = max(1, year − establishYear)");
            T.Eq(F32.Bits(20f), F32.Bits(Stage5Promotion.NewAdult("x", 1, 1, 30, 5, 30f, Array.Empty<AnnualTree>()).Tree.DbhCm), "DBH capped at 20 cm");
            T.Eq("R12-MA-B-7-3", Stage5Promotion.TreeIdFor(12, "MA-B", 7, 3), "TreeId format");
        });

        T.Add("S5-08 P-02: one promotion per aligned 5 m block per year (oldest, then tallest); the block's other band of that cohort leaves with it (Option A1); a younger cohort promotes the next year; positions deterministic and strictly in-polygon", () =>
        {
            var s = Stage5Annual.Genesis(Stage5Options.Default, 1, EmptySquares());
            int Index(string area, int col, int row) => s.Areas.Single(a => a.AreaId == area).Cells.ToList().FindIndex(c => c.Column == col && c.Row == row);
            s = WithCell(s, "MA-A", Index("MA-A", 0, 0), c => c with { Bands = new[] { Band(0.1f, 3.6f, 3) } });
            s = WithCell(s, "MA-A", Index("MA-A", 1, 1), c => c with { Bands = new[] { Band(0.1f, 3.7f, 3) } });
            s = WithCell(s, "MA-A", Index("MA-A", 1, 0), c => c with { Bands = new[] { Band(0.1f, 3.9f, 4) } });
            s = WithCell(s, "MA-B", Index("MA-B", 4, 0), c => c with { Bands = new[] { Band(0.1f, 3.6f, 6) } });
            var y20 = Stage5Annual.Step(s, EmptySquares(), 20);
            T.Eq("R20-MA-A-1-1", y20.PromotedTrees["MA-A"].Single().TreeId, "block (0,0): oldest year, then tallest wins");
            T.Eq("R20-MA-B-4-0", y20.PromotedTrees["MA-B"].Single().TreeId, "another block promotes in the same year");
            var a20 = y20.Account.Areas.Single(a => a.AreaId == "MA-A");
            var record = a20.Promotions.Single();
            // Cell (0,0) holds the same year-3 cohort as the winner: it leaves with it into the one tree (it is not deferred).
            T.Eq((1, 1), (record.CohortBandsExported, a20.CohortBandsExported), "one sibling band of the winner's cohort exported");
            T.Eq((double)0.1f, record.CohortDensityExported, "its abundance is booked on the promotion record");
            T.Eq((double)0.1f + (double)0.1f, a20.PromotionExported, "winner and sibling are both promotion export of MA-A, each once");
            T.Eq(1, a20.BlockCapDeferredCells, "only the younger year-4 cohort waits");
            T.Eq(0, y20.Account.Areas.Sum(a => a.BudgetWithheldBlocks), "full blocks: the block credit never binds");
            T.True(y20.Next.Areas[0].Cells[Index("MA-A", 0, 0)].Bands.Count == 0, "the sibling cohort band is gone");
            T.True(y20.Next.Areas[0].Cells[Index("MA-A", 1, 0)].Bands.Single().EstablishYear == 4, "the other cohort stays");
            var y21 = Stage5Annual.Step(y20.Next, EmptySquares(), 21);
            T.Eq("R21-MA-A-1-0", y21.PromotedTrees["MA-A"].Single().TreeId, "next year: the younger cohort (a successive cohort is not a leak)");
            var y22 = Stage5Annual.Step(y21.Next, EmptySquares(), 22);
            T.Eq(0, y22.PromotedTrees["MA-A"].Count, "the exported cohort never promotes a second tree");
            var promoted = y20.PromotedTrees["MA-A"].Single();
            T.Eq((record.XM, record.ZM, record.AgeYears), (promoted.XM, promoted.ZM, promoted.AgeYears), "record matches the tree");
            T.True(promoted.XM >= 2.5f && promoted.XM <= 5f && promoted.ZM >= 2.5f && promoted.ZM <= 5f, "inside its source cell");
            T.True(InsidePolygon(EmptySquares()[0].Polygon, promoted.XM, promoted.ZM), "strictly inside the area polygon");
            T.True(y20.Next.Areas[0].Cells[Index("MA-A", 1, 1)].Bands.Count == 0, "the promoted band is removed");
            var again = Stage5Annual.Step(s, EmptySquares(), 20);
            T.True(again.PromotedTrees["MA-A"].SequenceEqual(y20.PromotedTrees["MA-A"]), "placement is deterministic");
        });

        T.Add("S5-08 P-03: promoted adults are unique, strictly in-polygon, and enter adult competition, the canopy union and light at the next refresh", () =>
        {
            var c = Session();
            ThinEverySecond(c);
            Stage5PromotionRecord? first = null;
            for (int y = 0; y < 25 && first is null; y++)
            {
                Advance(c);
                first = c.ReadStage5View().Accounts[^1].Areas.SelectMany(a => a.Promotions).FirstOrDefault();
            }
            T.True(first is not null, "a promotion occurred within 25 years");
            var v = c.ReadView();
            var tree = v.Areas.Single(a => a.AreaId == first!.AreaId).Trees.Single(t => t.TreeId == first!.TreeId);
            T.Eq((first!.XM, first.ZM, first.HeightM, first.DbhCm, first.CrownRadiusM), (tree.XM, tree.ZM, tree.HeightM, tree.DbhCm, tree.CrownRadiusM), "roster tree = record");
            T.True(InsidePolygon(v.Areas.Single(a => a.AreaId == first.AreaId).Polygon, tree.XM, tree.ZM), "strictly inside its area");
            var ids = v.Areas.SelectMany(a => a.Trees).Select(t => t.TreeId).ToArray();
            T.Eq(ids.Length, ids.Distinct(StringComparer.Ordinal).Count(), "all tree IDs unique");
            T.True(c.ReadCanopyLight().Trees.Any(t => t.TreeId == tree.TreeId), "in the canopy/light woodland at the refresh");
            var next = Advance(c).Areas.Single(a => a.AreaId == first.AreaId).Report!;
            T.True(next.Trees.Any(row => row.Before.TreeId == tree.TreeId && row.Before.IsLiving), "in the next adult step (competition, growth, mortality)");
        });

        T.Add("S5-08 P-04: block-winner tie-break (F6): equal EstablishYear and height → lowest Column, then lowest Row; the tied loser's band leaves with the winner", () =>
        {
            // In the engine: block (0,1) = columns 0–1, rows 2–3. Cells (1,2) and (0,3) tie on year and height; (1,2) comes first
            // in lattice order, yet (0,3) must win because Column precedes Row in the key.
            var s = Stage5Annual.Genesis(Stage5Options.Default, 1, EmptySquares());
            int Index(int col, int row) => s.Areas[0].Cells.ToList().FindIndex(c => c.Column == col && c.Row == row);
            int i12 = Index(1, 2), i03 = Index(0, 3);
            T.True(i12 < i03, "the loser is first in lattice order");
            s = WithCell(s, "MA-A", i12, c => c with { Bands = new[] { Band(0.1f, 3.6f, 3) } });
            s = WithCell(s, "MA-A", i03, c => c with { Bands = new[] { Band(0.1f, 3.6f, 3) } });
            var y20 = Stage5Annual.Step(s, EmptySquares(), 20);
            var record = y20.Account.Areas[0].Promotions.Single();
            T.Eq(("R20-MA-A-0-3", 1), (record.TreeId, record.CohortBandsExported), "Column 0 wins over Column 1 although its Row is larger; the tie partner is exported");
            T.Eq(0, Stage5Annual.Step(y20.Next, EmptySquares(), 21).PromotedTrees["MA-A"].Count, "no second tree from the tied cohort");
            // The key itself, with the input deliberately in reverse order: Column first, then Row.
            static Stage5PromotionCandidate C(int col, int row) => new(0, "MA-A", 0, col, row, 1, 1, 3, 3.6f);
            T.Eq((2, 2), Pick(Stage5Promotion.Winners(new[] { C(2, 3), C(2, 2) }).Single()), "same Column: lowest Row");
            T.Eq((2, 3), Pick(Stage5Promotion.Winners(new[] { C(3, 2), C(2, 3) }).Single()), "lowest Column before lowest Row");
            T.Eq((3, 3), Pick(Stage5Promotion.Winners(new[] { C(2, 2), C(3, 3) with { EstablishYear = 2 } }).Single()), "an older cohort beats any position");
            static (int, int) Pick(Stage5PromotionCandidate c) => (c.Column, c.Row);
        });

        T.Add("S5-08 P-05: Stage5Cell.Light is the light the year's rules used (F3): it equals the canopy read-out in every year without a promotion, and in a promotion year the light of the committed roster without that year's new adults", () =>
        {
            var c = Session();
            ThinEverySecond(c);
            int promotionYears = 0, quietYears = 0, differingInPromotionYears = 0;
            for (int y = 1; y <= 30; y++)
            {
                Advance(c);
                var v = c.ReadView();
                var s5 = c.ReadStage5View();
                var newIds = s5.Accounts[^1].Areas.SelectMany(a => a.Promotions).Select(p => p.TreeId).ToHashSet(StringComparer.Ordinal);
                var without = v.Areas.Select(a => new CanopyArea(a.AreaId, Array.AsReadOnly(a.Polygon.Select(p => new CanopyVertex(p.XM, p.ZM)).ToArray()),
                    a.AreaHa, a.Trees.Where(t => !newIds.Contains(t.TreeId)).ToArray())).ToArray();
                var rulesLight = CanopyLightEvaluator.LightFields(without, 2.5);
                var canopy = c.ReadCanopyLight(2.5);
                int mismatch = 0, differing = 0;
                for (int i = 0; i < without.Length; i++)
                {
                    var cells = s5.Areas.Single(a => a.AreaId == without[i].AreaId).Cells;
                    var shown = canopy.Areas.Single(a => a.AreaId == without[i].AreaId).Light.Cells;
                    T.Eq(cells.Count, rulesLight[i].Cells.Count, "same lattice");
                    for (int k = 0; k < cells.Count; k++)
                    {
                        if (F32.Bits(cells[k].Light) != F32.Bits(rulesLight[i].Cells[k].LightIndex)) mismatch++;
                        if (F32.Bits(cells[k].Light) != F32.Bits(shown[k].LightIndex)) differing++;
                    }
                }
                T.Eq(0, mismatch, "year " + y + ": Light is the light of the committed roster without this year's new adults");
                if (newIds.Count == 0)
                {
                    quietYears++;
                    T.Eq(0, differing, "year " + y + " (no promotion): Light equals the canopy read-out");
                }
                else
                {
                    promotionYears++;
                    differingInPromotionYears += differing;
                }
            }
            Console.WriteLine($"  S5-08 P-05 years without promotion {quietYears}, with promotion {promotionYears}; cells differing from the canopy read-out in promotion years (new adults not yet shading) {differingInPromotionYears}");
            T.True(quietYears > 0 && promotionYears > 0, "both kinds of year were checked");
        });

        T.Add("S5-08 P-06: Option A2 block credit: K += in-polygon cells, promote when K ≥ C and pay C, then K = min(K, C); full blocks every year, 2-cell blocks every 2nd year, 1-cell blocks every 4th; 5.0 m never binds", () =>
        {
            // One treeless 7.5 × 7.5 m area (light 1, no seed): 2.5 m cells in columns/rows 0–2. Blocks: (0,0) 4 cells,
            // (0,1) and (1,0) 2 cells, (1,1) 1 cell. C = 4 cells per full block.
            var square = new[] { new Stage5AreaInput("MA-A", 801, new[] { new MultiAreaVertex(0, 0), new MultiAreaVertex(7.5, 0),
                new MultiAreaVertex(7.5, 7.5), new MultiAreaVertex(0, 7.5) }, 0.005625f, Array.Empty<AnnualTree>()) };
            T.Eq(4, Stage5Promotion.CellsPerBlock(2.5), "C at 2.5 m");
            T.Eq(1, Stage5Promotion.CellsPerBlock(5.0), "C at 5.0 m");
            var s = Stage5Annual.Genesis(Stage5Options.Default, 1, square);
            T.Eq("0,0,4,0;0,1,2,0;1,0,2,0;1,1,1,0", string.Join(";", s.Blocks.Select(b => $"{b.BlockX},{b.BlockZ},{b.InPolygonCells},{b.Credit}")),
                "genesis: every block with its in-polygon cells and no credit");
            int Index(int col, int row) => s.Areas[0].Cells.ToList().FindIndex(c => c.Column == col && c.Row == row);
            Stage5Band[] Cohorts(params int[] years) => years.Select(y => Band(0.05f, 3.6f, y)).ToArray();
            foreach (var (col, row) in new[] { (0, 0), (2, 0), (2, 2) })
                s = WithCell(s, "MA-A", Index(col, row), cell => cell with { Bands = Cohorts(3, 5, 7) });
            var years = new Dictionary<(int, int), List<int>> { [(0, 0)] = new(), [(1, 0)] = new(), [(1, 1)] = new() };
            int withheld = 0;
            for (int year = 17; year <= 40; year++)
            {
                var r = Stage5Annual.Step(s, square, year);
                foreach (var p in r.Account.Areas[0].Promotions) years[(p.BlockX, p.BlockZ)].Add(year);
                withheld += r.Account.Areas[0].BudgetWithheldBlocks;
                s = r.Next;
            }
            T.Eq("17,18,19", string.Join(",", years[(0, 0)]), "full block (budget 1): one cohort every year");
            T.Eq("18,20,22", string.Join(",", years[(1, 0)]), "2-cell block (budget 0.5): every second year");
            T.Eq("20,24,28", string.Join(",", years[(1, 1)]), "1-cell block (budget 0.25): every fourth year");
            T.Eq(3 + 9, withheld, "years a waiting cohort was withheld by the credit (2-cell: 3, 1-cell: 9)");
            T.Eq("4,4,4,4", string.Join(",", s.Blocks.Select(b => b.Credit)), "idle credit is capped at one promotion, not banked");
            // After a long idle spell the first cohort promotes at once, the next only when a full block has accrued again.
            s = WithCell(s, "MA-A", Index(2, 2), cell => cell with { Bands = Cohorts(30, 31) });
            var later = new List<int>();
            for (int year = 41; year <= 45; year++)
            {
                var r = Stage5Annual.Step(s, square, year);
                if (r.PromotedTrees["MA-A"].Count > 0) later.Add(year);
                s = r.Next;
            }
            // K: 4 + 1 = 5 → promote → 1; then 2, 3 (withheld); 4 → promote → 0; then 1. At most 1 + ⌊1·5/4⌋ = 2 in these 5 years.
            T.Eq("41,44", string.Join(",", later), "1-cell block after idle: promotes at once, then waits until a full block has accrued");

            // 5.0 m: every block is one cell with budget 1, so three cohorts promote in three consecutive years.
            var five = Stage5Annual.Genesis(new Stage5Options { GridCellM = 5.0 }, 1, EmptySquares());
            T.True(five.Blocks.All(b => b.InPolygonCells == 1), "5.0 m: one cell per block");
            five = WithCell(five, "MA-A", 0, cell => cell with { Bands = Cohorts(3, 5, 7).Select(b => b with { Density = 0.2f }).ToArray() });
            var fiveYears = new List<int>();
            for (int year = 17; year <= 21; year++)
            {
                var r = Stage5Annual.Step(five, EmptySquares(), year);
                T.Eq(0, r.Account.Areas.Sum(a => a.BudgetWithheldBlocks + a.CohortBandsExported), "5.0 m: no budget refusal and nothing to export");
                if (r.PromotedTrees["MA-A"].Count > 0) fiveYears.Add(year);
                five = r.Next;
            }
            T.Eq("17,18,19", string.Join(",", fiveYears), "5.0 m: a = 1 and no edge-gate reduction");
        });

        T.Add("S5-08 P-07: no double counting (Option A1): in every area-year starting − losses + accepted − promotion export = ending within the float-merge rounding bound, and every exported band is booked once", () =>
        {
            foreach (double grid in new[] { 2.5, 5.0 })
            {
                var c = Session(null, new Stage5Options { GridCellM = grid, MastSeed = 3 });
                ThinEverySecond(c);
                for (int y = 0; y < 30; y++) Advance(c);
                var areaYears = c.ReadStage5View().Accounts.SelectMany(y => y.Areas).ToArray();
                // Losses are exact float differences of each band (Sterbenz), so the only residual is the float rounding of band
                // merges: at most about 0.5 ulp(1.5) ≈ 6e-8 per merge, a few merges per cell-year, ≤ 383 cells → < 1e-4. One
                // represented band counted twice or lost would be at least the representation threshold, 0.01·a ≥ 2.5e-3.
                double worst = areaYears.Max(a => Math.Abs(a.StartingAbundance - (a.LightLoss + a.BrowseLoss + a.VegetationLoss) +
                    a.EstablishmentAccepted - a.PromotionExported - a.EndingAbundance));
                var records = areaYears.SelectMany(a => a.Promotions).ToArray();
                double booked = areaYears.Sum(a => a.PromotionExported);
                double exported = records.Sum(p => (double)p.ExportedDensity + p.CohortDensityExported);
                int cohortBands = areaYears.Sum(a => a.CohortBandsExported);
                Console.WriteLine($"  S5-08 P-07 {grid:F1} m: {records.Length} promotions, {cohortBands} cohort bands exported with them, worst area-year residual {worst:E2}");
                T.True(worst <= 1e-4, $"{grid} m: abundance identity holds in every area-year (worst {worst:E2})");
                T.True(Math.Abs(booked - exported) <= 1e-9 * Math.Max(1.0, booked), $"{grid} m: accounts book exactly the exported abundance of the records");
                T.Eq(records.Sum(p => p.CohortBandsExported), cohortBands, $"{grid} m: every cohort band booked once, in its own area");
                if (grid == 2.5) T.True(cohortBands > 0, "2.5 m: cohort export really happens in a 30-year run");
                else T.Eq(0, cohortBands, "5.0 m: one cell per block, so nothing is exported with a winner");
            }
        });

        T.Add("S5-08 P-08: an aligned 5 m block that straddles two racks (F6): its credit counts the in-polygon cells of both racks; the winner may lie in either rack and the cohort's band in the other rack leaves with it, booked to that rack; one tree per cohort; the abundance identity holds in each rack", () =>
        {
            // Two touching treeless 7.5 × 7.5 m racks: light is 1 everywhere and there is no seed. The shared edge x = 7.5 m
            // runs through the aligned 5 m blocks of block column 1:
            //   block (1,0) = MA-A cells (2,0), (2,1) and MA-B cells (3,0), (3,1): a full block, n = 4, budget 1 per year;
            //   block (1,1) = MA-A cell (2,2) and MA-B cell (3,2): n = 2, budget 1 every second year.
            // Counted per rack instead, they would be 2-cell and 1-cell blocks and promote less often than asserted below.
            static MultiAreaVertex V(double x, double z) => new(x, z);
            var racks = new[]
            {
                new Stage5AreaInput("MA-A", 801, new[] { V(0, 0), V(7.5, 0), V(7.5, 7.5), V(0, 7.5) }, 0.005625f, Array.Empty<AnnualTree>()),
                new Stage5AreaInput("MA-B", 802, new[] { V(7.5, 0), V(15, 0), V(15, 7.5), V(7.5, 7.5) }, 0.005625f, Array.Empty<AnnualTree>()),
            };
            var s = Stage5Annual.Genesis(Stage5Options.Default, 1, racks);
            T.Eq("MA-A columns 0-2, MA-B columns 3-5",
                $"MA-A columns {s.Areas[0].Cells.Min(c => c.Column)}-{s.Areas[0].Cells.Max(c => c.Column)}, MA-B columns {s.Areas[1].Cells.Min(c => c.Column)}-{s.Areas[1].Cells.Max(c => c.Column)}",
                "the shared edge splits block column 1 (2.5 m columns 2 and 3) between the racks");
            T.Eq("0,0,4;0,1,2;1,0,4;1,1,2;2,0,4;2,1,2", string.Join(";", s.Blocks.Select(b => $"{b.BlockX},{b.BlockZ},{b.InPolygonCells}")),
                "a straddling block counts the in-polygon cells of both racks: (1,0) 2 + 2 = 4, (1,1) 1 + 1 = 2");
            T.True(Stage5Annual.Genesis(new Stage5Options { GridCellM = 5.0 }, 1, racks).Blocks.All(b => b.BlockX != 1),
                "5.0 m: block column 1 has no cell (its 5 m centre lies on the shared edge, in neither rack): a 2.5 m-only block, as in S5-07");
            int Index(string area, int col, int row) => s.Areas.Single(a => a.AreaId == area).Cells.ToList().FindIndex(c => c.Column == col && c.Row == row);
            // Block (1,0): cohort 3 wins in MA-A (taller), cohort 5 wins in MA-B (taller), cohort 7 only in MA-B.
            s = WithCell(s, "MA-A", Index("MA-A", 2, 0), c => c with { Bands = new[] { Band(0.1f, 3.7f, 3), Band(0.1f, 3.6f, 5) } });
            s = WithCell(s, "MA-B", Index("MA-B", 3, 1), c => c with { Bands = new[] { Band(0.1f, 3.6f, 3), Band(0.1f, 3.8f, 5) } });
            s = WithCell(s, "MA-B", Index("MA-B", 3, 0), c => c with { Bands = new[] { Band(0.1f, 3.6f, 7) } });
            // Block (1,1): cohort 3 wins in MA-B (taller), cohort 5 only in MA-A, cohort 7 only in MA-B.
            s = WithCell(s, "MA-A", Index("MA-A", 2, 2), c => c with { Bands = new[] { Band(0.1f, 3.6f, 3), Band(0.1f, 3.6f, 5) } });
            s = WithCell(s, "MA-B", Index("MA-B", 3, 2), c => c with { Bands = new[] { Band(0.1f, 3.7f, 3), Band(0.1f, 3.6f, 7) } });

            var log = new List<string>();
            var exportedByYear = new Dictionary<int, string>();
            int withheldA = 0, withheldB = 0, cohortBands = 0;
            double worst = 0, booked = 0, recorded = 0;
            for (int year = 17; year <= 24; year++)
            {
                var r = Stage5Annual.Step(s, racks, year);
                foreach (var a in r.Account.Areas)
                {
                    foreach (var p in a.Promotions) log.Add($"{year}:{p.TreeId}+{p.CohortBandsExported}");
                    var polygon = racks.Single(x => x.AreaId == a.AreaId).Polygon;
                    T.True(r.PromotedTrees[a.AreaId].All(t => InsidePolygon(polygon, t.XM, t.ZM)), $"year {year}: every {a.AreaId} tree is strictly inside its own rack");
                    worst = Math.Max(worst, Math.Abs(a.StartingAbundance - (a.LightLoss + a.BrowseLoss + a.VegetationLoss) + a.EstablishmentAccepted -
                        a.PromotionExported - a.EndingAbundance));
                    booked += a.PromotionExported;
                    recorded += a.Promotions.Sum(p => (double)p.ExportedDensity + p.CohortDensityExported);
                    cohortBands += a.CohortBandsExported;
                }
                var byArea = r.Account.Areas.ToDictionary(a => a.AreaId, StringComparer.Ordinal);
                withheldA += byArea["MA-A"].BudgetWithheldBlocks;
                withheldB += byArea["MA-B"].BudgetWithheldBlocks;
                exportedByYear[year] = $"A {byArea["MA-A"].CohortBandsExported}/{byArea["MA-A"].PromotionExported:R}, B {byArea["MA-B"].CohortBandsExported}/{byArea["MA-B"].PromotionExported:R}";
                if (year == 17)
                {
                    var record = byArea["MA-A"].Promotions.Single();
                    T.Eq((double)0.1f, record.CohortDensityExported, "year 17: the MA-A winner's record carries the cohort-3 band exported from MA-B");
                    T.Eq((1, (double)0.1f), (byArea["MA-B"].CohortBandsExported, byArea["MA-B"].PromotionExported),
                        "year 17: the exported MA-B band is booked to MA-B, the rack it left");
                    T.Eq((0, (double)0.1f), (byArea["MA-A"].CohortBandsExported, byArea["MA-A"].PromotionExported), "year 17: MA-A books only its own winner");
                    var left = r.Next.Areas.Single(a => a.AreaId == "MA-B").Cells[Index("MA-B", 3, 1)].Bands;
                    T.Eq("5", string.Join(",", left.Select(b => b.EstablishYear)), "year 17: MA-B cell (3,1) lost its cohort-3 band and keeps cohort 5");
                    T.Eq(1, byArea["MA-B"].BudgetWithheldBlocks, "year 17: block (1,1) holds 2 of 4 credit cells, so its MA-B winner waits (booked to MA-B)");
                }
                if (year == 18)
                    T.Eq((2, 0), (byArea["MA-A"].CohortBandsExported, byArea["MA-B"].CohortBandsExported),
                        "year 18: both MA-B winners take their cohort's MA-A band (cohort 5 from (2,0), cohort 3 from (2,2)), booked to MA-A");
                s = r.Next;
            }
            Console.WriteLine("  S5-08 P-08 promotions (year:tree+cohort bands exported): " + string.Join(", ", log));
            Console.WriteLine("  S5-08 P-08 exports per year (cohort bands / promotion export): " + string.Join("; ", exportedByYear.Select(p => p.Key + " " + p.Value)));
            // Full straddling block: one cohort every year (budget 1), the winner alternating between the racks.
            // 2-cell straddling block: one cohort every second year (budget 0.5), counted over both racks.
            T.Eq("17:R17-MA-A-2-0+1, 18:R18-MA-B-3-1+1, 18:R18-MA-B-3-2+1, 19:R19-MA-B-3-0+0, 20:R20-MA-A-2-2+0, 22:R22-MA-B-3-2+0",
                string.Join(", ", log), "straddling blocks: promotion schedule, winners in either rack, cohort export across the edge");
            T.Eq((1, 2), (withheldA, withheldB), "block (1,1) credit withheld in years 17, 19 and 21, each booked to the rack of that year's would-be winner");
            T.Eq(3, cohortBands, "three bands left with a winner in the other rack");
            T.True(s.Areas.All(a => a.Cells.All(c => c.Bands.Count == 0)), "nine bands became six trees, one per cohort per block, with nothing left over");
            // (1,0): 4 accrues each idle year and is capped at 4. (1,1): 0 after year 22, then 2 in year 23 and 4 in year 24.
            T.Eq("1,0,4,4;1,1,2,4", string.Join(";", s.Blocks.Where(b => b.BlockX == 1).Select(b => $"{b.BlockX},{b.BlockZ},{b.InPolygonCells},{b.Credit}")),
                "after the idle years 23–24 each straddling block holds one full block of credit (4), never more");
            T.True(worst <= 1e-6, $"the abundance identity holds in each rack in every year (worst {worst:E2})");
            T.True(Math.Abs(booked - recorded) <= 1e-9, "the racks book exactly the abundance the promotion records carry");
        });
    }

    internal static bool InsidePolygon(IReadOnlyList<MultiAreaVertex> polygon, double x, double z)
    {
        for (int i = 0; i < polygon.Count; i++)
        {
            var a = polygon[i];
            var b = polygon[(i + 1) % polygon.Count];
            if ((b.XM - a.XM) * (z - a.ZM) - (b.ZM - a.ZM) * (x - a.XM) <= 0) return false;
        }
        return true;
    }

    // ============================================================================================ S5-09 adult-empty rack
    private static void AdultEmptyRack()
    {
        T.Add("S5-09 E-01: a rack with 0 living adults advances, receives neighbour seed, updates undergrowth, grows and promotes cohorts", () =>
        {
            var mature = new[] { (3_000, 4_000, 40, 300, 20_000), (3_000, 16_000, 40, 320, 21_000), (8_000, 10_000, 45, 350, 22_000) };
            var c = Session(TwoSquares("mad-s5-empty-rack", mature, new[] { (30_000, 10_000, 5, 40, 2_000) }), new Stage5Options { MastSeed = 5 });
            // B's only stem dies (injected into the private root: a Work Plan may never empty a populated rack, D-093).
            var root = Root(c);
            RootField.SetValue(c, root with
            {
                Areas = Array.AsReadOnly(root.Areas.Select(a => a.AreaId != "MA-B" ? a : a with
                {
                    State = a.State with { Trees = Array.AsReadOnly(a.State.Trees.Select(t => t with { Death = new AnnualDeath(t.TreeId, 0, "test-injected", "test") }).ToArray()) },
                }).ToArray()),
            });
            var genesisB = c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-B").Cells;
            int promotedYear = -1;
            for (int y = 1; y <= 18 && promotedYear < 0; y++)
            {
                var result = Advance(c);
                var v = c.ReadView();
                T.Eq(y, v.Source.EcologicalYear, "shared clock");
                T.Eq(y, Root(c).Areas.Single(a => a.AreaId == "MA-B").State.EcologicalYear, "B advances its own ecological year");
                var year = c.ReadStage5View().Accounts[^1].Areas.Single(a => a.AreaId == "MA-B");
                if (y == 1)
                {
                    T.True(result.Areas.Single(a => a.AreaId == "MA-B").Report is null, "B's adult step is skipped (no living adult)");
                    T.True(year.SeedArrival > 0 && year.EstablishmentAccepted > 0, "B receives seed from A and establishes");
                    var cellsB = c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-B").Cells;
                    T.True(cellsB.Zip(genesisB).Any(p => p.First.Bramble != p.Second.Bramble), "B's undergrowth updates");
                }
                if (year.Promotions.Count > 0) promotedYear = y;
            }
            T.True(promotedYear > 0, "B promotes a cohort although it started the run with no living adult");
            var afterPromotion = Advance(c);
            T.True(afterPromotion.Areas.Single(a => a.AreaId == "MA-B").Report is not null, "the promoted adult runs the adult step next year");
            Console.WriteLine("  S5-09 first promotion in the adult-empty rack in year " + promotedYear);
        });

        T.Add("S5-09 E-02: with no living adult in ANY rack, Year+1 still advances while juvenile abundance remains (no seed; bands grow and promote); with no abundance either it is refused (F6)", () =>
        {
            var mature = new[] { (3_000, 4_000, 40, 300, 20_000), (3_000, 16_000, 40, 320, 21_000), (8_000, 10_000, 45, 350, 22_000) };
            var young = new[] { (30_000, 10_000, 5, 40, 2_000) };
            // Every stem of every rack dies (injected into the private root: a Work Plan may never empty a populated rack, D-093).
            static void KillEveryStem(MultiAreaContinuation c)
            {
                var root = Root(c);
                RootField.SetValue(c, root with
                {
                    Areas = Array.AsReadOnly(root.Areas.Select(a => a with
                    {
                        State = a.State with
                        {
                            Trees = Array.AsReadOnly(a.State.Trees.Select(t => t.IsLiving
                                ? t with { Death = new AnnualDeath(t.TreeId, root.EcologicalYear, "test-injected", "test") } : t).ToArray()),
                        },
                    }).ToArray()),
                });
            }

            var c = Session(TwoSquares("mad-s5-all-empty", mature, young), new Stage5Options { MastSeed = 5 });
            for (int y = 0; y < 3; y++) Advance(c);
            T.True(c.ReadStage5View().Areas.All(a => a.Cells.Any(cell => cell.Bands.Count > 0)), "both racks hold regeneration before the adults die");
            KillEveryStem(c);
            T.True(c.ReadView().Areas.All(a => !a.Trees.Any(t => t.IsLiving)) && Root(c).Stage5!.HasAbundance, "no living adult anywhere, abundance remains");
            int promotedYear = -1;
            for (int y = 4; y <= 25 && promotedYear < 0; y++)
            {
                var result = Advance(c);
                T.Eq(y, c.ReadView().Source.EcologicalYear, "the shared clock advances");
                T.True(result.Areas.All(a => a.Report is null), "no adult step runs in any rack");
                var year = c.ReadStage5View().Accounts[^1];
                T.Eq(0.0, year.Areas.Sum(a => a.SeedArrival), "no living mature tree, so no seed");
                T.True(year.Areas.Sum(a => a.StartingAbundance) > 0, "the bands are still stepped");
                if (year.Areas.Sum(a => a.Promotions.Count) > 0) promotedYear = y;
            }
            T.True(promotedYear > 0, "a cohort promotes in the adult-empty woodland");
            T.True(Advance(c).Areas.Any(a => a.Report is not null), "the promoted adult runs the adult step next year");
            Console.WriteLine("  S5-09 E-02 first promotion in the all-empty woodland in year " + promotedYear);

            var none = Session(TwoSquares("mad-s5-all-empty-none", mature, young), new Stage5Options { MastSeed = 5 });
            KillEveryStem(none);
            T.True(!Root(none).Stage5!.HasAbundance, "year 0: no juvenile abundance yet");
            T.Refuses("ContinuationNoLivingStems", () => Advance(none));
            T.Eq(0, none.ReadView().Source.EcologicalYear, "the refused year changed nothing");
        });
    }

    // ============================================================================================ S5-10 determinism and atomicity
    private static void DeterminismAndAtomicity()
    {
        T.Add("S5-10 D-01: identical multi-year 1/1/1/1 runs give identical hashes; Stage 5 never changes the ADULT-MORT rolls before any promotion", () =>
        {
            string first = FullRun();
            T.Eq(first, FullRun(), "two 30-year runs identical");
            Console.WriteLine("  S5 1/1/1/1 full-run fingerprint " + first);
            T.Eq(PinnedStage5FullRunHash, first, "pinned 1/1/1/1 full run (Phase 2 P2-01, R8)");
            // Before any promotion (10 years cannot grow 0.15 m to 3.5 m), the 1/1/1/0 recipe on 1/1/1/1 reproduces the
            // 1/1/1/0 hash exactly: every adult roll, growth, death and deadwood record is unchanged by Stage 5.
            T.Eq(PinnedStage4FullRunHash, Stage4BehaviourTests.RunFingerprint(Stage4Options.Default, MultiAreaModelSelection.Stage5), "adult output unchanged");
            var check = Session();
            for (int y = 0; y < 10; y++) Advance(check);
            T.Eq(0, check.ReadStage5View().Accounts.Sum(y => y.Areas.Sum(a => a.Promotions.Count)), "no promotion in the first ten years");
        });

        T.Add("S5-10 D-02: a forced pre-commit failure (Year+1 or Execute) leaves the previous MultiAreaRoot intact; the retried year equals an uninterrupted one", () =>
        {
            var c = Session();
            var twin = Session();
            ThinEverySecond(c);
            ThinEverySecond(twin);
            for (int y = 0; y < 14; y++) { Advance(c); Advance(twin); }
            var cell = c.ReadStage5View().Areas[0].Cells[20];
            var v = c.AddClearUndergrowth(Fence(c.ReadView()), "MA-A", cell.Column, cell.Row).View;
            v = c.Approve(Fence(v)).View;
            var before = Root(c);
            Throws<InvalidOperationException>(() => c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey,
                beforePublish: () => throw new InvalidOperationException("forced")));
            T.True(ReferenceEquals(before, Root(c)), "Execute: nothing published");
            Throws<InvalidOperationException>(() => c.Advance(Fence(v), true, beforeCommit: () => throw new InvalidOperationException("forced")));
            T.True(ReferenceEquals(before, Root(c)), "Year+1: nothing published (no tree, band, cover, promotion or order changed)");
            c.CancelApproved(Fence(c.ReadView()));
            Advance(c);
            Advance(twin);
            var w1 = new Model0FingerprintTests.Fingerprint();
            var w2 = new Model0FingerprintTests.Fingerprint();
            Write(w1, c.ReadStage5View());
            Write(w2, twin.ReadStage5View());
            foreach (var a in c.ReadView().Areas) w1.Trees(a.Trees);
            foreach (var a in twin.ReadView().Areas) w2.Trees(a.Trees);
            T.Eq(w2.Hash(), w1.Hash(), "the retried year equals the uninterrupted twin");
        });
    }

    /// <summary>The 1/1/1/1 fingerprint: thinning in year 0, a clear in year 5, 30 years; adult reports and the Stage 5 read-out.</summary>
    internal static string FullRun() => FullRun(Session());

    /// <summary>The same recipe on an already constructed session (Phase 2 P2-01 runs it on an explicit five-field selection).</summary>
    internal static string FullRun(MultiAreaContinuation c)
    {
        var w = new Model0FingerprintTests.Fingerprint();
        ThinEverySecond(c);
        for (int year = 0; year < 30; year++)
        {
            if (year == 5)
            {
                var cell = c.ReadStage5View().Areas[1].Cells[30];
                var v = c.AddClearUndergrowth(Fence(c.ReadView()), "MA-B", cell.Column, cell.Row).View;
                v = c.Approve(Fence(v)).View;
                c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            }
            foreach (var area in Advance(c).Areas)
            {
                w.Text(area.AreaId);
                if (area.Report is { } report) w.Report(report); else w.Text("carried");
            }
        }
        foreach (var area in c.ReadView().Areas) w.Trees(area.Trees);
        var s5 = c.ReadStage5View();
        Write(w, s5);
        WriteAmendment(w, s5.Blocks, s5.Accounts);
        return w.Hash();
    }
}
