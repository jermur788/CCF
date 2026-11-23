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

        T.Add("S5-01 F-03: unsupported selections are typed refusals; Stage 5 options, reads and ClearUndergrowth need 1/1/1/1", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var cw = AdultModelVersion.ConnectedWoodland;
            var rd = MortalityModelVersion.RecordedDeadwood;
            var c2 = CrownModelVersion.AreaScaledEquilibriumC2;
            foreach (var bad in new[]
            {
                new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, CrownModelVersion.LegacyFrozen, Regen1),
                new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, c2, Regen1),
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
        T.Add("S5-02 G-01: same seed sources: the dense unthinned interior stays suppressed; a managed gap raises the light index, establishes cohorts and grows them toward promotion", () =>
        {
            // Area A interior: the gap is every living tree within 7.5 m of (20, 15); the test cells are those whose centre is
            // within 5 m of that point, read in both sessions. Light is the modelled k10a10 index, not calibrated PAR.
            const float gx = 20f, gz = 15f;
            var dense = Session();
            var gap = Session();
            int felled = FellGap(gap, "MA-A", gx, gz, 7.5f);
            T.True(felled >= 10, "the gap removes a real group of stems (" + felled + ")");
            Stage5Cell[] Cells(MultiAreaContinuation c) => c.ReadStage5View().Areas.Single(a => a.AreaId == "MA-A").Cells
                .Where(cell => (cell.CentreXM - gx) * (cell.CentreXM - gx) + (cell.CentreZM - gz) * (cell.CentreZM - gz) <= 25f).ToArray();
            double denseTallest = 0, denseTallest12 = 0, gapTallest12 = 0, denseAbundance12 = 0, gapAbundance12 = 0, denseLight = 0, gapLight1 = 0, denseLight1 = 0;
            bool denseQualified = false;
            for (int year = 1; year <= 15; year++)
            {
                Advance(dense);
                Advance(gap);
                var d = Cells(dense);
                var g = Cells(gap);
                denseTallest = Math.Max(denseTallest, d.SelectMany(c => c.Bands).Select(b => (double)b.Height).DefaultIfEmpty(0).Max());
                denseQualified |= d.Any(c => c.Bands.Any(b => b.Height >= 3.5f));
                denseLight += d.Average(c => (double)c.Light) / 15.0;
                if (year == 1) { gapLight1 = g.Average(c => (double)c.Light); denseLight1 = d.Average(c => (double)c.Light); }
                if (year == 12)
                {
                    gapTallest12 = g.SelectMany(c => c.Bands).Select(b => (double)b.Height).DefaultIfEmpty(0).Max();
                    denseTallest12 = d.SelectMany(c => c.Bands).Select(b => (double)b.Height).DefaultIfEmpty(0).Max();
                    denseAbundance12 = d.Sum(c => c.Abundance);
                    gapAbundance12 = g.Sum(c => c.Abundance);
                }
            }
            var denseCellKeys = Cells(dense).Select(c => (c.Column, c.Row)).ToHashSet();
            int densePromotions = dense.ReadStage5View().Accounts.SelectMany(y => y.Areas).SelectMany(a => a.Promotions)
                .Count(p => p.AreaId == "MA-A" && denseCellKeys.Contains((p.Column, p.Row)));
            Console.WriteLine($"  S5-02 cells {denseCellKeys.Count}: light y1 dense {denseLight1:F3} gap {gapLight1:F3}; dense mean light 15 y {denseLight:F3} " +
                $"(LR {LightResponseOracle(denseLight):F3}); y12 abundance dense {denseAbundance12:F4} gap {gapAbundance12:F4}; tallest y12 dense {denseTallest12:F2} m gap {gapTallest12:F2} m; dense max over 15 y {denseTallest:F2} m");
            T.True(gapLight1 > denseLight1 + 0.2, "the gap raises the modelled light index");
            T.True(LightResponseOracle(denseLight) < 0.15, "the dense interior sits in the poor-light band (LR < 0.15, survival penalty)");
            // "Suppressed" is read as the spec states it: the interior never reaches promotion height or promotes, while the gap
            // (same seed sources) is taller and more abundant at the same age. A fixed interior height cap is not a spec value:
            // as Area A self-thins its interior light rises, so interior bands do grow slowly (reported above).
            T.True(!denseQualified && densePromotions == 0, "the dense interior never reaches promotion height nor promotes in 15 years");
            T.True(gapTallest12 > denseTallest12, "gap cohorts are taller than the dense interior's at the same time");
            T.True(gapAbundance12 > denseAbundance12, "the gap establishes more regeneration than the dense interior");
            T.True(gapTallest12 >= 1.5, "gap cohorts grow past the bramble/bracken escape height toward promotion");
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

        T.Add("S5-08 P-02: one promotion per aligned 5 m block per year (oldest, then tallest, then Column/Row); deferred cells promote later; positions deterministic and strictly in-polygon", () =>
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
            T.Eq(2, y20.Account.Areas.Single(a => a.AreaId == "MA-A").BlockCapDeferredCells, "two qualifying cells deferred by the cap");
            var y21 = Stage5Annual.Step(y20.Next, EmptySquares(), 21);
            T.Eq("R21-MA-A-0-0", y21.PromotedTrees["MA-A"].Single().TreeId, "next year: the oldest deferred band");
            var y22 = Stage5Annual.Step(y21.Next, EmptySquares(), 22);
            T.Eq("R22-MA-A-1-0", y22.PromotedTrees["MA-A"].Single().TreeId, "then the younger one");
            var record = y20.Account.Areas.Single(a => a.AreaId == "MA-A").Promotions.Single();
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
    }

    // ============================================================================================ S5-10 determinism and atomicity
    private static void DeterminismAndAtomicity()
    {
        T.Add("S5-10 D-01: identical multi-year 1/1/1/1 runs give identical hashes; Stage 5 never changes the ADULT-MORT rolls before any promotion", () =>
        {
            string first = FullRun();
            T.Eq(first, FullRun(), "two 30-year runs identical");
            Console.WriteLine("  S5 1/1/1/1 full-run fingerprint " + first);
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
    private static string FullRun()
    {
        var c = Session();
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
        Write(w, c.ReadStage5View());
        return w.Hash();
    }
}
