using System.Globalization;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Stage 4 behaviour oracles (spec multi-stage4-adult-sitka-parity-v1 §4): growth seams, RD/hazard/boundary, cross-rack
/// growth, overstocked vs thinned self-thinning with deadwood, pruning lifecycle and determinism. Expected values are
/// recomputed here from the written formulas, not read back from the engine's own helpers, wherever that is practical.
/// </summary>
internal static class Stage4BehaviourTests
{
    private static readonly SpeciesProfile Sitka = SpeciesProfile.Sitka;

    internal static void Register()
    {
        // ------------------------------------------------------------------------------------------- growth and seams

        T.Add("S4 G-01: neutral site 1.0 is bit-identical to no override; year-1 growth equals C2 growth on woodland CI", () =>
        {
            var neutral = new Stage4Options { SiteProductivity = (x, z) => 1f };
            T.Eq(RunFingerprint(Stage4Options.Default), RunFingerprint(neutral), "explicit 1.0 field == default, ten years with work");
            T.True(Stage4Options.Default.GrowthAgePhases is null, "production Sitka has no growth age phases");

            var c = Stage4();
            var start = c.ReadView();
            var woodland = CrownModel.Woodland(start.Areas.SelectMany(a => a.Trees));
            var annual = Advance(c);
            int checkedTrees = 0;
            foreach (var area in annual.Areas)
            foreach (var row in area.Report!.Trees.Where(r => r.Before.IsLiving))
            {
                float wci = AnnualRules.Competition(row.Before, woodland).Ci;
                var oracle = AnnualRules.GrowC2(row.Before, wci, wci); // frozen DBH/height law fed with the woodland CI
                T.Eq(F32.Bits(oracle.DbhCm), F32.Bits(row.After.DbhCm), row.Before.TreeId + " DBH");
                T.Eq(F32.Bits(oracle.HeightM), F32.Bits(row.After.HeightM), row.Before.TreeId + " height");
                T.Eq(F32.Bits(oracle.CrownRadiusM), F32.Bits(row.After.CrownRadiusM), row.Before.TreeId + " crown");
                T.Eq(row.Before.AgeYears + 1, row.After.AgeYears, row.Before.TreeId + " age + 1");
                checkedTrees++;
            }
            T.True(checkedTrees > 200, "every living MA-1 tree checked (" + checkedTrees + ")");
        });

        T.Add("S4 G-02: a synthetic non-1.0 site value scales DBH and height increments only", () =>
        {
            var baseline = Advance(Stage4());
            var boosted = Advance(Stage4(new Stage4Options { SiteProductivity = (x, z) => 1.25f }));
            int n = 0;
            for (int a = 0; a < 2; a++)
            {
                var b0 = baseline.Areas[a].Report!.Trees.Where(r => r.Before.IsLiving).ToArray();
                var b1 = boosted.Areas[a].Report!.Trees.Where(r => r.Before.IsLiving).ToArray();
                for (int i = 0; i < b0.Length; i++)
                {
                    T.Eq(b0[i].Before, b1[i].Before, "same start");
                    T.Eq(F32.Bits(b0[i].PreGrowthCi), F32.Bits(b1[i].PreGrowthCi), "CI is not scaled");
                    T.Eq(b0[i].After.AgeYears, b1[i].After.AgeYears, "age is not scaled");
                    T.Eq((b0[i].After.XM, b0[i].After.ZM), (b1[i].After.XM, b1[i].After.ZM), "position is not scaled");
                    Near(1.25, Inc(b1[i], t => t.DbhCm) / Inc(b0[i], t => t.DbhCm), 1e-4, "DBH increment x1.25");
                    Near(1.25, Inc(b1[i], t => t.HeightM) / Inc(b0[i], t => t.HeightM), 1e-4, "height increment x1.25");
                    n++;
                }
            }
            T.Refuses("SiteProductivityInvalid", () => Advance(Stage4(new Stage4Options { SiteProductivity = (x, z) => float.NaN })));
            T.Refuses("SiteProductivityInvalid", () => Advance(Stage4(new Stage4Options { SiteProductivity = (x, z) => 0f })));
            Console.WriteLine("  S4 site seam: " + n + " trees, DBH and height increments scaled by exactly the site value");
        });

        T.Add("S4 G-03: the growth-age-phase seam applies a synthetic table (Unity piecewise rule); Sitka production is inert", () =>
        {
            var table = new[] { new GrowthAgePhase(0f, 1f, 1f), new GrowthAgePhase(40f, 0.5f, 2f) };
            T.Eq((1f, 1f), GrowthAgePhases.FactorsAt(null, 50f), "no table: 1 and 1");
            T.Eq((1f, 1f), GrowthAgePhases.FactorsAt(table, 39f), "before the second start");
            T.Eq((0.5f, 2f), GrowthAgePhases.FactorsAt(table, 40f), "at the second start");
            var baseline = Advance(Stage4());
            var phased = Advance(Stage4(new Stage4Options { GrowthAgePhases = table }));
            int young = 0, old = 0;
            for (int a = 0; a < 2; a++)
            {
                var b0 = baseline.Areas[a].Report!.Trees.Where(r => r.Before.IsLiving).ToArray();
                var b1 = phased.Areas[a].Report!.Trees.Where(r => r.Before.IsLiving).ToArray();
                for (int i = 0; i < b0.Length; i++)
                {
                    if (b0[i].Before.AgeYears < 40)
                    {
                        T.Eq(F32.Bits(b0[i].After.DbhCm), F32.Bits(b1[i].After.DbhCm), "young DBH unchanged");
                        T.Eq(F32.Bits(b0[i].After.HeightM), F32.Bits(b1[i].After.HeightM), "young height unchanged");
                        young++;
                    }
                    else
                    {
                        Near(0.5, Inc(b1[i], t => t.DbhCm) / Inc(b0[i], t => t.DbhCm), 1e-4, "old DBH increment x0.5");
                        Near(2.0, Inc(b1[i], t => t.HeightM) / Inc(b0[i], t => t.HeightM), 1e-4, "old height increment x2");
                        old++;
                    }
                }
            }
            T.True(young > 0 && old > 0, "both phases exercised (" + young + " young, " + old + " old)");
            T.Refuses("GrowthAgePhasesInvalid", () => Stage4(new Stage4Options { GrowthAgePhases = new[] { new GrowthAgePhase(5f, 1f, 1f) } }));
            T.Refuses("GrowthAgePhasesInvalid", () => Stage4(new Stage4Options
            {
                GrowthAgePhases = new[] { new GrowthAgePhase(0f, 1f, 1f), new GrowthAgePhase(0f, 1f, 1f) },
            }));
            T.Refuses("GrowthAgePhasesInvalid", () => Stage4(new Stage4Options { GrowthAgePhases = new[] { new GrowthAgePhase(0f, -1f, 1f) } }));
        });

        // ------------------------------------------------------------------------------------------- RD / hazard / boundary

        T.Add("S4 M-01: RD below 0.6 gives zero pressure, no rolls and no deaths", () =>
        {
            var trees = Grid("MA-T", 4, 4, 5f, (i) => 20f);
            var report = AnnualRules.CalculateStage4(new AnnualState(0, trees), 101, 0.1f, CrownModel.Woodland(trees), _ => 0, Stage4Options.Default);
            Near(OracleRd(report.PostGrowth.LivingCount, 0.1f, report.Trees.Select(r => r.After)), report.PostGrowth.RelativeDensity, 0, "RD oracle");
            T.True(report.PostGrowth.RelativeDensity < 0.6, "RD < 0.6 (" + report.PostGrowth.RelativeDensity + ")");
            T.Eq(0.0, report.PostGrowthDensityPressure, "zero pressure");
            T.True(report.Trees.All(r => r.DeathProbability is null && r.MortalityRoll is null), "no hazard and no roll");
            T.Eq(0, report.Deaths.Count, "no deaths");
        });

        T.Add("S4 M-02: RD 0.6-1.0 hazard matches the Unity constants exactly and is suppression-biased; rolls are ADULT-MORT", () =>
        {
            var trees = Grid("MA-T", 10, 10, 2f, i => 15f + (i * 7 % 9) * 0.5f);
            const float areaHa = 0.04f;
            const int seed = 4242;
            var report = AnnualRules.CalculateStage4(new AnnualState(0, trees), seed, areaHa, CrownModel.Woodland(trees), _ => 0, Stage4Options.Default);
            double rd = OracleRd(report.PostGrowth.LivingCount, areaHa, report.Trees.Select(r => r.After));
            Near(rd, report.PostGrowth.RelativeDensity, 0, "RD = N/ha*(Dq/25)^2.063/1868");
            T.True(rd > 0.6 && rd < 1.0, "mid-range RD (" + rd.ToString("0.0000", CultureInfo.InvariantCulture) + ")");
            double p = Math.Max(0.0, (rd - 0.6) / 0.4);
            Near(p, report.PostGrowthDensityPressure, 0, "P = (RD-0.6)/0.4");
            var s = report.Trees.Select(r => 1.0 - 1.0 / (1.0 + (double)(r.PreGrowthCi / 5f))).ToArray();
            double meanV = s.Average(v => Math.Pow(v, 4));
            for (int i = 0; i < s.Length; i++)
            {
                var row = report.Trees[i];
                Near(Math.Min(0.5, 0.08 * p * p * Math.Pow(s[i], 4) / meanV), row.DeathProbability!.Value, 1e-15, row.Before.TreeId + " p");
                T.Eq(F32.Bits(SimulationRandom.Roll(DerivedManifest.RngModelVersion, "ADULT-MORT-" + row.Before.TreeId, 1, seed)),
                    F32.Bits(row.MortalityRoll!.Value), row.Before.TreeId + " roll");
                T.Eq(row.MortalityRoll < row.DeathProbability, !row.After.IsLiving, row.Before.TreeId + " death iff roll < p");
            }
            for (int i = 0; i < s.Length; i++)
            for (int j = 0; j < s.Length; j++)
                if (s[i] > s[j]) T.True(report.Trees[i].DeathProbability >= report.Trees[j].DeathProbability, "more suppressed => higher p");
            T.True(report.Deaths.All(d => d.Mechanism == "hazard" && d.Cause == LifeStates.DensitySuppression && d.EcologicalYear == 1),
                "hazard deaths only, cause DensitySuppression, year 1");
            Console.WriteLine(FormattableString.Invariant(
                $"  S4 mid-range: RD {rd:0.0000}, P {p:0.0000}, p range {report.Trees.Min(r => r.DeathProbability):0.000000}-{report.Trees.Max(r => r.DeathProbability):0.000000}, deaths {report.Deaths.Count}"));
        });

        T.Add("S4 M-03: RD above 1.0 triggers deterministic boundary kills (S desc, TreeId asc) until RD <= 1.0", () =>
        {
            var trees = Grid("MA-T", 10, 10, 2f, i => 28f + (i * 5 % 7));
            const float areaHa = 0.04f;
            var report = AnnualRules.CalculateStage4(new AnnualState(0, trees), 777, areaHa, CrownModel.Woodland(trees), _ => 0, Stage4Options.Default);
            T.True(report.PostGrowth.RelativeDensity > 1.0, "post-growth RD > 1.0 (" + report.PostGrowth.RelativeDensity + ")");
            var boundary = report.Deaths.Where(d => d.Mechanism == "boundary").Select(d => d.TreeId).ToArray();
            T.True(boundary.Length > 0, "boundary kills happened");
            // Oracle: the hazard survivors, ordered by suppression descending then TreeId ascending.
            var hazardDead = report.Deaths.Where(d => d.Mechanism == "hazard").Select(d => d.TreeId).ToHashSet();
            var survivors = report.Trees.Where(r => !hazardDead.Contains(r.Before.TreeId)).ToList();
            var order = survivors.OrderByDescending(r => 1.0 - 1.0 / (1.0 + (double)(r.PreGrowthCi / 5f)))
                .ThenBy(r => r.Before.TreeId, StringComparer.Ordinal).Select(r => r.Before.TreeId).ToArray();
            T.Eq(string.Join(",", order.Take(boundary.Length)), string.Join(",", boundary), "victims are the most suppressed, in order");
            var alive = survivors.Where(r => !boundary.Contains(r.Before.TreeId)).Select(r => r.After).ToArray();
            var beforeLast = survivors.Where(r => !boundary.Take(boundary.Length - 1).Contains(r.Before.TreeId)).Select(r => r.After with { Death = null }).ToArray();
            T.True(OracleRd(alive.Length, areaHa, alive) <= 1.0, "RD after the boundary <= 1.0");
            T.True(OracleRd(beforeLast.Length, areaHa, beforeLast) > 1.0, "the last victim was still needed");
            Near(OracleRd(alive.Length, areaHa, alive), report.After.RelativeDensity, 0, "reported final RD");
            Console.WriteLine(FormattableString.Invariant(
                $"  S4 boundary: post-growth RD {report.PostGrowth.RelativeDensity:0.0000}, hazard {hazardDead.Count}, boundary {boundary.Length}, final RD {report.After.RelativeDensity:0.0000}"));
        });

        T.Add("S4 M-04: boundary ties (equal suppression) are broken by TreeId ascending", () =>
        {
            // Five identical stems 10 m apart (CI 0, S 0, so mean V = 0 and no hazard) on a tiny nominal area: RD >> 1.
            var trees = Enumerable.Range(1, 5).Select(i => new AnnualTree("MA-T-" + i.ToString("D3", CultureInfo.InvariantCulture),
                i * 10f, 5f, 40, 30f, 20f, 3f)).ToArray();
            var report = AnnualRules.CalculateStage4(new AnnualState(0, trees), 9, 0.001f, CrownModel.Woodland(trees), _ => 0, Stage4Options.Default);
            T.True(report.Trees.All(r => r.DeathProbability == 0.0 && r.MortalityRoll is null), "zero hazard when every S is 0");
            T.Eq("MA-T-001,MA-T-002,MA-T-003,MA-T-004", string.Join(",", report.Deaths.Select(d => d.TreeId)), "TreeId order");
            T.True(report.Deaths.All(d => d.Mechanism == "boundary"), "boundary mechanism");
            T.Eq(1, report.After.LivingCount, "never below one survivor");
        });

        T.Add("S4 M-05: the biological-death transition is idempotent and distinct from felling", () =>
        {
            var tree = new AnnualTree("MA-T-001", 1f, 1f, 30, 20f, 15f, 2f);
            var death = new AnnualDeath("MA-T-001", 3, LifeStates.DensitySuppression, "hazard");
            var dead = LifeStates.ApplyBiologicalDeath(tree, death);
            T.Eq(LifeState.BiologicallyDead, LifeStates.Of(dead), "biologically dead");
            T.True(ReferenceEquals(dead, LifeStates.ApplyBiologicalDeath(dead, death with { EcologicalYear = 9 })), "second death is a no-op");
            var felled = tree with { Death = new AnnualDeath("MA-T-001", 3, FellCause.Harvest, FellCause.Mechanism) };
            T.Eq(LifeState.FelledStump, LifeStates.Of(felled), "a Fell is a stump");
            T.True(ReferenceEquals(felled, LifeStates.ApplyBiologicalDeath(felled, death)), "a stump cannot die biologically");
            T.Refuses("BiologicalDeathInvalid", () => LifeStates.ApplyBiologicalDeath(tree, death with { Cause = FellCause.Harvest }));
            T.Refuses("BiologicalDeathInvalid", () => LifeStates.ApplyBiologicalDeath(tree, death with { Cause = " " }));
            T.Refuses("BiologicalDeathInvalid", () => LifeStates.ApplyBiologicalDeath(tree, death with { TreeId = "MA-T-002" }));
            T.Eq(F32.Bits(30f * 30f * 0.00007854f * (20f * 0.5f)), F32.Bits(FallenDeadwoodRecord.StemVolumeM3(30f, 20f)), "volume formula");
            T.Eq("DW0001", FallenDeadwoodRecord.IdFor(1), "Unity deadwood numbering");
        });

        // ------------------------------------------------------------------------------------------- cross-rack growth

        T.Add("S4 X-01: felling a large Area A competitor within 8 m raises an Area B border tree's next DBH increment only", () =>
        {
            var e = CrossRackExperiment();
            var keep = OneYearDbh(new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4), fell: null);
            var felled = OneYearDbh(new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4), fell: "MA-A-002");
            T.True(felled.Inc["MA-B-001"] > keep.Inc["MA-B-001"], "border tree (4 m from the competitor) grows more after the Fell");
            T.True(felled.Ci["MA-B-001"] < keep.Ci["MA-B-001"], "border tree woodland CI falls");
            T.Eq(F32.Bits(keep.Inc["MA-B-003"]), F32.Bits(felled.Inc["MA-B-003"]), "tree 11 m from the competitor: identical increment");
            T.Eq(F32.Bits(keep.Ci["MA-B-003"]), F32.Bits(felled.Ci["MA-B-003"]), "tree 11 m away: identical CI");
            // Contrast: Stage 1-3 (adult model 0) grows DBH on the area's own CI, so the cross-border Fell changes nothing.
            var s3keep = OneYearDbh(new MultiAreaContinuation(e, MultiAreaModelSelection.Stage3CrownC2), fell: null);
            var s3felled = OneYearDbh(new MultiAreaContinuation(e, MultiAreaModelSelection.Stage3CrownC2), fell: "MA-A-002");
            T.Eq(F32.Bits(s3keep.Inc["MA-B-001"]), F32.Bits(s3felled.Inc["MA-B-001"]), "adult model 0: border increment unchanged");
            foreach (var id in new[] { "MA-B-001", "MA-B-003" })
                Console.WriteLine(FormattableString.Invariant(
                    $"  S4 cross-rack {id}: woodland CI {keep.Ci[id]:0.000000} -> {felled.Ci[id]:0.000000}; DBH increment {keep.Inc[id]:0.000000} -> {felled.Inc[id]:0.000000} cm (Stage 1-3: {s3keep.Inc[id]:0.000000} -> {s3felled.Inc[id]:0.000000})"));
        });

        // ------------------------------------------------------------------------------------------- self-thinning + deadwood

        T.Add("S4 D-01: overstocked vs thinned Area A over ten years: deaths, one deadwood per death, no stump, Area B RD alone", () =>
        {
            var over = Stage4();
            var thin = Stage4();
            var v = thin.ReadView();
            var areaA = v.Areas.Single(a => a.AreaId == "MA-A");
            foreach (var tree in areaA.Trees.Where(t => t.IsLiving).Where((t, i) => i % 2 == 1))
                v = thin.AddPending(Fence(v), "MA-A", tree.TreeId, "S4 thinning: every second stem").View;
            v = thin.Approve(Fence(v)).View;
            thin.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
            Console.WriteLine("  S4 self-thinning table (year: A RD/deaths over | A RD/deaths thinned | B RD/deaths over | B RD/deaths thinned)");
            foreach (var c in new[] { over, thin })
            {
                var canopyBefore = c.ReadCanopyLight();
                int recomputes = c.CanopyRecomputeCount;
                for (int year = 0; year < 10; year++)
                {
                    var start = c.ReadView();
                    var result = Advance(c);
                    CheckYear(c, start, result);
                }
                T.True(c.CanopyRecomputeCount == recomputes, "reads alone never recompute");
                var canopyAfter = c.ReadCanopyLight();
                T.Eq(recomputes + 1, c.CanopyRecomputeCount, "a committed annual step invalidates canopy/light");
                var dead = c.ReadStage4View().Areas.SelectMany(a => a.Trees).Where(t => t.LifeState == LifeState.BiologicallyDead)
                    .Select(t => t.TreeId).ToHashSet();
                T.True(canopyAfter.Trees.All(t => !dead.Contains(t.TreeId)), "dead stems are not in the crown/growing-space read-out");
                T.True(canopyAfter.Trees.Count < canopyBefore.Trees.Count || dead.Count == 0, "the read-out lost the dead stems");
            }
            var so = over.ReadStage4View();
            var st = thin.ReadStage4View();
            for (int y = 0; y < 10; y++)
            {
                var ho = over.ReadView().AnnualHistory[y];
                var ht = thin.ReadView().AnnualHistory[y];
                Console.WriteLine(FormattableString.Invariant(
                    $"    {y + 1,2}: A {ho.Areas[0].After.RelativeDensity:0.000}/{ho.Areas[0].NaturalDeaths.Count,2} | A {ht.Areas[0].After.RelativeDensity:0.000}/{ht.Areas[0].NaturalDeaths.Count,2} | B {ho.Areas[1].After.RelativeDensity:0.000}/{ho.Areas[1].NaturalDeaths.Count,2} | B {ht.Areas[1].After.RelativeDensity:0.000}/{ht.Areas[1].NaturalDeaths.Count,2}"));
            }
            int overA = so.Areas[0].CumulativeNaturalDeaths, thinA = st.Areas[0].CumulativeNaturalDeaths;
            T.True(overA > 0, "overstocked Area A self-thins (" + overA + " deaths)");
            T.True(thinA < overA, "thinned Area A loses fewer stems naturally (" + thinA + " < " + overA + ")");
            foreach (var record in so.Areas.SelectMany(a => a.Deadwood).Take(3))
                Console.WriteLine(FormattableString.Invariant(
                    $"  S4 deadwood {record.DeadwoodId} {record.TreeId} {record.AreaId} {record.SpeciesId} ({record.XM:0.000},{record.ZM:0.000}) y{record.DeathYear} {record.Cause} DBH {record.DbhCm:0.000} cm H {record.HeightM:0.000} m V {record.EstimatedVolumeM3:0.000000} m3"));
            Console.WriteLine("  S4 deadwood totals: overstocked " + so.Areas.Sum(a => a.Deadwood.Count) + ", thinned " + st.Areas.Sum(a => a.Deadwood.Count));
        });

        // ------------------------------------------------------------------------------------------- pruning

        T.Add("S4 P-01: Prune Pending and Approve change nothing; Execute cuts 0.92 at once and does not advance the year", () =>
        {
            var c = Stage4();
            var v = c.ReadView();
            var tree = PruneCandidate(v);
            c.ReadCanopyLight();
            int recomputes = c.CanopyRecomputeCount;
            var pending = c.AddPrune(Fence(v), "MA-B", tree.TreeId, 3f).View;
            SameTrees(v, pending, "Pending");
            T.Eq(1, c.ReadStage4View().PendingPrunes.Count, "one pending Prune");
            c.ReadCanopyLight();
            T.Eq(recomputes, c.CanopyRecomputeCount, "Pending does not invalidate canopy/light");
            var approved = c.Approve(Fence(pending)).View;
            SameTrees(v, approved, "Approve");
            T.Eq(1, c.ReadStage4View().ApprovedPrunes.Count, "Prune is in the approval");
            T.Eq(0, c.ReadStage4View().Areas.SelectMany(a => a.Trees).Sum(t => t.PruningLifts), "no lift recorded before Execute");
            c.ReadCanopyLight();
            T.Eq(recomputes, c.CanopyRecomputeCount, "Approve does not invalidate canopy/light");

            var executed = c.Execute(Fence(approved), approved.Approved!.PlanId, approved.Approved.ExecutionKey);
            var after = executed.View;
            var cut = Tree(after, tree.TreeId);
            T.Eq(F32.Bits(MathF.Max(0.1f, tree.CrownRadiusM * 0.92f)), F32.Bits(cut.CrownRadiusM), "r = max(0.1, r*0.92)");
            T.Eq(v.Source.EcologicalYear, after.Source.EcologicalYear, "the year does not advance");
            T.Eq(v.Source.AnnualRevision, after.Source.AnnualRevision, "no annual step");
            T.Eq(v.Source.WorkRevision + 1, after.Source.WorkRevision, "work revision + 1");
            T.Eq(tree with { CrownRadiusM = cut.CrownRadiusM }, cut, "only the crown radius changed");
            foreach (var other in v.Areas.SelectMany(a => a.Trees).Where(t => t.TreeId != tree.TreeId))
                T.Eq(other, Tree(after, other.TreeId), "other trees unchanged");
            var s4 = c.ReadStage4View().Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == tree.TreeId);
            T.Eq((1, 3f, 0), (s4.PruningLifts, s4.CrownBaseHeightM, s4.LastPruningYear), "lifts, crown base, year");
            T.Eq(1, c.ReadStage4View().PruneLog.Count, "one prune record");
            c.ReadCanopyLight();
            T.Eq(recomputes + 1, c.CanopyRecomputeCount, "Execute invalidates canopy/light");
            var replay = c.Execute(Fence(approved), approved.Approved.PlanId, approved.Approved.ExecutionKey);
            T.True(replay.Execution!.Replayed, "identical retry is a replay");
            T.Eq(F32.Bits(cut.CrownRadiusM), F32.Bits(Tree(replay.View, tree.TreeId).CrownRadiusM), "no second cut");
            Console.WriteLine(FormattableString.Invariant(
                $"  S4 prune {tree.TreeId}: H {tree.HeightM:0.000} m, r {tree.CrownRadiusM:0.000000} -> {cut.CrownRadiusM:0.000000} m, crown base 0 -> 3 m, year {after.Source.EcologicalYear}"));
        });

        T.Add("S4 P-02: the future crown target carries 0.92^lifts alongside CrownModel 1", () =>
        {
            var c = Stage4();
            var v = c.ReadView();
            var tree = PruneCandidate(v);
            v = Prune(c, v, "MA-B", tree.TreeId, 3f);
            var before = c.ReadView();
            var woodland = before.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
            var start = Tree(before, tree.TreeId);
            float ci = HegyiOracle(start, woodland);
            Advance(c);
            var grown = Tree(c.ReadView(), tree.TreeId);
            T.True(grown.IsLiving, "candidate survived the year");
            float rPot = MathF.Max(0.1f, 0.9415f + 0.07635f * grown.DbhCm);
            float target = MathF.Max(0.1f, rPot / MathF.Sqrt(1f + ci / 5f) * (float)Math.Pow(0.92f, 1));
            float expected = MathF.Max(0.1f, start.CrownRadiusM + (target - start.CrownRadiusM) * 0.15f);
            T.Eq(F32.Bits(expected), F32.Bits(grown.CrownRadiusM), "r' = r + 0.15 (r_target*0.92^1 - r)");
            float unprunedTarget = MathF.Max(0.1f, rPot / MathF.Sqrt(1f + ci / 5f));
            T.True(target < unprunedTarget, "the pruned target is below the unpruned target");
            T.Eq(1f, PruningRules.TargetFactor(0), "factor 1 with no lifts");
            T.Eq(F32.Bits((float)Math.Pow(0.92f, 3)), F32.Bits(PruningRules.TargetFactor(3)), "0.92^3");
        });

        T.Add("S4 P-03: Prune guards reject dead/felled trees, a 4th lift, a lift within 5 years and out-of-range targets", () =>
        {
            T.Refuses("Stage4ModelRequired", () =>
            {
                var m0 = new MultiAreaContinuation(MultiAreaGenerator.Generate());
                var v0 = m0.ReadView();
                m0.AddPrune(Fence(v0), "MA-B", PruneCandidate(v0).TreeId, 3f);
            });
            T.Refuses("Stage4ModelRequired", () =>
            {
                var m1 = new MultiAreaContinuation(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Stage3CrownC2);
                var v1 = m1.ReadView();
                m1.AddPrune(Fence(v1), "MA-B", PruneCandidate(v1).TreeId, 3f);
            });

            var c = Stage4();
            var v = c.ReadView();
            var tree = PruneCandidate(v);
            string id = tree.TreeId;
            float h = tree.HeightM;
            T.Refuses("PruneTargetNotAboveCrownBase", () => c.AddPrune(Fence(v), "MA-B", id, 0f));
            T.Refuses("PruneTargetNotAboveCrownBase", () => c.AddPrune(Fence(v), "MA-B", id, -1f));
            T.Refuses("PruneTargetOutOfRange", () => c.AddPrune(Fence(v), "MA-B", id, h * 0.6f));
            T.Refuses("PruneTargetOutOfRange", () => c.AddPrune(Fence(v), "MA-B", id, float.NaN));
            T.Refuses("PruneTargetOutOfRange", () => c.AddPrune(Fence(v), "MA-B", id, float.PositiveInfinity));
            T.Refuses("MultiAreaWrongArea", () => c.AddPrune(Fence(v), "MA-A", id, 3f));
            T.Refuses("UnknownTreeId", () => c.AddPrune(Fence(v), "MA-B", "MA-B-999", 3f));
            T.Eq((v.Source, v.PlanStateRevision), (c.ReadView().Source, c.ReadView().PlanStateRevision), "every refusal changed nothing");
            SameTrees(v, c.ReadView(), "refusals");
            T.Eq(0, c.ReadStage4View().PendingPrunes.Count, "no order queued by a refusal");

            // Fell/Prune exclusivity on one tree; a retained Crop Tree may be pruned.
            var withFell = c.AddPending(Fence(v), "MA-B", id, "S4 exclusivity").View;
            T.Refuses("WorkDuplicateTreeId", () => c.AddPrune(Fence(withFell), "MA-B", id, 3f));
            v = c.RemovePending(Fence(withFell), "MA-B", id).View;
            v = c.MarkCrop(Fence(v), "MA-B", id).View;
            var withPrune = c.AddPrune(Fence(v), "MA-B", id, 3f).View;
            T.Refuses("WorkDuplicateTreeId", () => c.AddPrune(Fence(withPrune), "MA-B", id, 4f));
            T.Refuses("WorkDuplicateTreeId", () => c.AddPending(Fence(withPrune), "MA-B", id, "S4 fell over a pending prune"));
            T.Refuses("WorkPendingConsentRequired", () => c.Advance(Fence(withPrune), false));
            v = c.RemovePrune(Fence(withPrune), "MA-B", id).View;
            T.Eq(0, c.ReadStage4View().PendingPrunes.Count, "removed");

            // Lift 1 in year 0, then the 5-year interval.
            v = Prune(c, v, "MA-B", id, 3f);
            T.Refuses("PruneIntervalTooShort", () => c.AddPrune(Fence(c.ReadView()), "MA-B", id, 4f));
            for (int y = 0; y < 4; y++) Advance(c);
            T.Refuses("PruneIntervalTooShort", () => c.AddPrune(Fence(c.ReadView()), "MA-B", id, 4f));
            Advance(c);
            v = c.ReadView();
            T.Eq(5, v.Source.EcologicalYear, "year 5");
            T.Refuses("PruneTargetNotAboveCrownBase", () => c.AddPrune(Fence(v), "MA-B", id, 3f));
            v = Prune(c, v, "MA-B", id, 4f);
            for (int y = 0; y < 5; y++) Advance(c);
            v = Prune(c, c.ReadView(), "MA-B", id, 5f);
            for (int y = 0; y < 5; y++) Advance(c);
            v = c.ReadView();
            T.True(Tree(v, id).IsLiving, "candidate alive at year 15");
            T.Refuses("PruneMaxLiftsReached", () => c.AddPrune(Fence(v), "MA-B", id, 6f));
            var history = c.ReadStage4View().Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == id);
            T.Eq((3, 5f, 10), (history.PruningLifts, history.CrownBaseHeightM, history.LastPruningYear), "three lifts recorded");
            T.Eq("0,5,10", string.Join(",", c.ReadStage4View().PruneLog.Where(r => r.TreeId == id).Select(r => r.EcologicalYear)), "lift years");

            // A naturally dead tree and a felled tree are refused.
            var naturallyDead = c.ReadStage4View().Areas.Single(a => a.AreaId == "MA-A").Trees.First(t => t.LifeState == LifeState.BiologicallyDead);
            T.Refuses("PruneTreeNotLiving", () => c.AddPrune(Fence(v), "MA-A", naturallyDead.TreeId, 1f));
            var victim = v.Areas.Single(a => a.AreaId == "MA-A").Trees.First(t => t.IsLiving);
            v = c.AddPending(Fence(v), "MA-A", victim.TreeId, "S4 fell for guard").View;
            v = c.Approve(Fence(v)).View;
            v = c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            T.Eq(LifeState.FelledStump, c.ReadStage4View().Areas[0].Trees.Single(t => t.TreeId == victim.TreeId).LifeState, "felled is a stump");
            T.Refuses("PruneTreeNotLiving", () => c.AddPrune(Fence(v), "MA-A", victim.TreeId, 1f));
        });

        T.Add("S4 P-04: an approved Prune is cancelled with consent at zero lifts; Execute revalidates and refuses all", () =>
        {
            var c = Stage4();
            var v = c.ReadView();
            var tree = PruneCandidate(v);
            v = c.AddPrune(Fence(v), "MA-B", tree.TreeId, 3f).View;
            v = c.Approve(Fence(v)).View;
            T.Refuses("WorkPendingConsentRequired", () => c.Advance(Fence(v), false));
            c.Advance(Fence(v), true);
            var s4 = c.ReadStage4View();
            T.Eq(0, s4.ApprovedPrunes.Count + s4.PendingPrunes.Count + s4.PruneLog.Count, "cancelled, nothing pruned");
            T.Eq(0, s4.Areas.SelectMany(a => a.Trees).Sum(t => t.PruningLifts), "zero lifts");
            T.Refuses("WorkPlanNotApproved", () => c.Execute(Fence(c.ReadView()), v.Approved!.PlanId, v.Approved.ExecutionKey));
        });

        // ------------------------------------------------------------------------------------------- views and determinism

        T.Add("S4 R-01: the Stage 4 read-out carries model versions, evidence labels and per-rack RD from the rack alone", () =>
        {
            var c = Stage4();
            for (int y = 0; y < 3; y++) Advance(c);
            var s4 = c.ReadStage4View();
            var v = c.ReadView();
            T.Eq((AdultModelVersion.ConnectedWoodland, MortalityModelVersion.RecordedDeadwood, CrownModelVersion.AreaScaledEquilibriumC2),
                (s4.AdultModelVersion, s4.MortalityModelVersion, s4.CrownModelVersion), "model versions");
            T.True(s4.EvidenceLabels.Any(l => l.Contains("UK/British")) && s4.EvidenceLabels.Any(l => l.Contains("Gameplay Calibration")) &&
                s4.EvidenceLabels.Any(l => l.Contains("Experimental Demonstration")), "evidence labels");
            for (int i = 0; i < 2; i++)
            {
                var living = v.Areas[i].Trees.Where(t => t.IsLiving).ToArray();
                Near(OracleRd(living.Length, v.Areas[i].AreaHa, living), s4.Areas[i].RelativeDensity, 0, v.Areas[i].AreaId + " RD from its own stems and area");
                T.Eq(v.AnnualHistory.Sum(h => h.Areas[i].NaturalDeaths.Count), s4.Areas[i].CumulativeNaturalDeaths, "cumulative deaths");
                T.Eq(v.AnnualHistory[^1].Areas[i].NaturalDeaths.Count, s4.Areas[i].AnnualNaturalDeaths, "last-year deaths");
            }
            var all = v.Areas.SelectMany(a => a.Trees.Where(t => t.IsLiving)).ToArray();
            double pooled = OracleRd(all.Length, v.Areas[0].AreaHa + v.Areas[1].AreaHa, all);
            T.True(Math.Abs(pooled - s4.Areas[1].RelativeDensity) > 1e-6, "Area B RD is not the pooled A+B value");
            var m0 = new MultiAreaContinuation(MultiAreaGenerator.Generate()).ReadStage4View();
            T.Eq((AdultModelVersion.Legacy, MortalityModelVersion.Legacy, CrownModelVersion.LegacyFrozen),
                (m0.AdultModelVersion, m0.MortalityModelVersion, m0.CrownModelVersion), "model 0 read-out reports 0/0/0");
            T.Eq(0, m0.Areas.Sum(a => a.Deadwood.Count), "no deadwood outside Stage 4");
        });

        T.Add("S4 R-02: identical seed and commands give bit-identical multi-year Stage 4 output (with Fells, Prunes and deaths)", () =>
        {
            string first = RunFingerprint(Stage4Options.Default);
            T.Eq(first, RunFingerprint(Stage4Options.Default), "two runs identical");
            T.True(first != Stage4Tests.PinnedStage3CrownModel1Hash && first != Model0FingerprintTests.PinnedMultiAreaHash, "Stage 4 really differs");
            Console.WriteLine("  S4 stage4 run fingerprint " + first);
        });
    }

    // ------------------------------------------------------------------------------------------- helpers

    internal static MultiAreaContinuation Stage4(Stage4Options? options = null) =>
        new(MultiAreaGenerator.Generate(), MultiAreaModelSelection.Stage4, options);

    private static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);

    private static MultiAreaAnnualResult Advance(MultiAreaContinuation c)
    {
        var v = c.ReadView();
        return c.Advance(Fence(v), false).Annual!;
    }

    private static AnnualTree Tree(MultiAreaSessionView v, string id) => v.Areas.SelectMany(a => a.Trees).Single(t => t.TreeId == id);

    private static void SameTrees(MultiAreaSessionView a, MultiAreaSessionView b, string what)
    {
        T.Eq(a.Source.EcologicalYear, b.Source.EcologicalYear, what + ": year");
        T.Eq(a.Source.WorkRevision, b.Source.WorkRevision, what + ": work revision");
        var bt = b.Areas.SelectMany(x => x.Trees).ToArray();
        var at = a.Areas.SelectMany(x => x.Trees).ToArray();
        for (int i = 0; i < at.Length; i++) T.Eq(at[i], bt[i], what + ": tree " + at[i].TreeId);
    }

    /// <summary>The open Area B stem with the smallest Hegyi CI (most likely to survive many years).</summary>
    private static AnnualTree PruneCandidate(MultiAreaSessionView v)
    {
        var all = v.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
        return v.Areas.Single(a => a.AreaId == "MA-B").Trees.Where(t => t.IsLiving)
            .OrderBy(t => HegyiOracle(t, all)).ThenBy(t => t.TreeId, StringComparer.Ordinal).First();
    }

    private static MultiAreaSessionView Prune(MultiAreaContinuation c, MultiAreaSessionView v, string area, string id, float target)
    {
        v = c.AddPrune(Fence(v), area, id, target).View;
        v = c.Approve(Fence(v)).View;
        return c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
    }

    /// <summary>Independent Hegyi: neighbours within 8 m (inclusive), DBH ratio over max(0.5 m, distance), float32.</summary>
    private static float HegyiOracle(AnnualTree target, IEnumerable<AnnualTree> trees)
    {
        float ci = 0f;
        foreach (var t in trees.Where(t => t.IsLiving && t.TreeId != target.TreeId).OrderBy(t => t.TreeId, StringComparer.Ordinal))
        {
            float dx = target.XM - t.XM, dz = target.ZM - t.ZM;
            float d = (float)Math.Sqrt(dx * dx + dz * dz);
            if (d > 8f) continue;
            ci += (t.DbhCm / MathF.Max(1f, target.DbhCm)) / MathF.Max(0.5f, d);
        }
        return ci;
    }

    /// <summary>Comeau 2010 via the Unity constants: N/ha * (Dq/25)^2.063 / 1868, with N/ha = count / (float) area.</summary>
    private static double OracleRd(int count, float areaHa, IEnumerable<AnnualTree> trees)
    {
        var living = trees.Where(t => t.IsLiving).ToArray();
        if (living.Length == 0) return 0.0;
        double sum = 0.0;
        foreach (var t in living) sum += (double)t.DbhCm * t.DbhCm;
        double dq = Math.Sqrt(sum / living.Length);
        return (living.Length / areaHa) * Math.Pow(dq / 25.0, 2.063) / 1868.0;
    }

    private static double Inc(AnnualTreeAudit row, Func<AnnualTree, float> field) => (double)field(row.After) - field(row.Before);

    private static void Near(double expected, double actual, double tolerance, string what)
    {
        if (!(Math.Abs(expected - actual) <= tolerance * Math.Max(1.0, Math.Abs(expected))))
            throw new Exception($"{what}: expected {expected:R}, got {actual:R}");
    }

    private static AnnualTree[] Grid(string prefix, int cols, int rows, float pitch, Func<int, float> dbh)
    {
        var trees = new List<AnnualTree>();
        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            int i = trees.Count;
            float d = dbh(i);
            trees.Add(new AnnualTree(prefix + "-" + (i + 1).ToString("D3", CultureInfo.InvariantCulture),
                pitch / 2 + c * pitch, pitch / 2 + r * pitch, 30, d, 18f, Sitka.PotentialCrownRadiusM(d)));
        }
        return trees.ToArray();
    }

    /// <summary>
    /// Two touching 20 × 20 m racks. MA-A-002 (DBH 60 cm) is 4 m west of the Area B border tree MA-B-001; MA-B-003 is 11 m
    /// from MA-A-002 but 7 m from MA-B-001, so it has competition that the Fell does not touch. Stocking is low (RD ≪ 0.6).
    /// </summary>
    private static MultiAreaExperiment CrossRackExperiment()
    {
        static MillimetrePoint P(long x, long y) => new(x, y);
        var a = new MultiAreaDefinition("MA-A", "Area A", "S4 cross-rack test", new[] { P(0, 0), P(20_000, 0), P(20_000, 20_000), P(0, 20_000) },
            501, new[]
            {
                new MultiAreaTreeDefinition("MA-A-001", 2_000, 2_000, 40, 300, 20_000),
                new MultiAreaTreeDefinition("MA-A-002", 18_000, 10_000, 40, 600, 30_000),
            });
        var b = new MultiAreaDefinition("MA-B", "Area B", "S4 cross-rack test", new[] { P(20_000, 0), P(40_000, 0), P(40_000, 20_000), P(20_000, 20_000) },
            502, new[]
            {
                new MultiAreaTreeDefinition("MA-B-001", 22_000, 10_000, 40, 200, 18_000),
                new MultiAreaTreeDefinition("MA-B-002", 38_000, 18_000, 40, 200, 18_000),
                new MultiAreaTreeDefinition("MA-B-003", 29_000, 10_000, 40, 200, 18_000),
            });
        return new MultiAreaExperiment("mad-s4-cross-rack-test", "s4-test", 1, 1, new[] { a, b });
    }

    private sealed record DbhYear(Dictionary<string, float> Inc, Dictionary<string, float> Ci);

    private static DbhYear OneYearDbh(MultiAreaContinuation c, string? fell)
    {
        var v = c.ReadView();
        if (fell is not null)
        {
            v = c.AddPending(Fence(v), "MA-A", fell, "S4 cross-rack competitor").View;
            v = c.Approve(Fence(v)).View;
            v = c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
        }
        var woodland = v.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
        var ci = woodland.ToDictionary(t => t.TreeId, t => HegyiOracle(t, woodland));
        var report = c.Advance(Fence(v), false).Annual!;
        T.True(report.Areas.All(a => a.Report!.Deaths.Count == 0), "no natural deaths in the low-density cross-rack stand");
        var inc = report.Areas.SelectMany(a => a.Report!.Trees).Where(r => r.Before.IsLiving)
            .ToDictionary(r => r.Before.TreeId, r => r.After.DbhCm - r.Before.DbhCm);
        return new DbhYear(inc, ci);
    }

    /// <summary>Per-year Stage 4 invariants: one deadwood per death with the dead stem's values; no stump; per-rack RD.</summary>
    private static void CheckYear(MultiAreaContinuation c, MultiAreaSessionView start, MultiAreaAnnualResult result)
    {
        var after = c.ReadView();
        var s4 = c.ReadStage4View();
        foreach (var areaResult in result.Areas)
        {
            var report = areaResult.Report;
            if (report is null) continue;
            int i = after.Areas.ToList().FindIndex(a => a.AreaId == areaResult.AreaId);
            var deadwood = s4.Areas[i].Deadwood.Where(d => d.DeathYear == report.YearAfter).ToArray();
            T.Eq(report.Deaths.Count, deadwood.Length, areaResult.AreaId + " one deadwood record per death");
            T.Eq(report.Deaths.Count, deadwood.Select(d => d.TreeId).Distinct().Count(), "distinct trees");
            foreach (var death in report.Deaths)
            {
                var tree = Tree(after, death.TreeId);
                var record = deadwood.Single(d => d.TreeId == death.TreeId);
                T.Eq(LifeState.BiologicallyDead, LifeStates.Of(tree), "biologically dead, not a stump");
                T.Eq(LifeStates.DensitySuppression, tree.Death!.Cause, "cause");
                T.Eq((areaResult.AreaId, "sitka-spruce", tree.XM, tree.ZM, report.YearAfter, LifeStates.DensitySuppression, tree.DbhCm, tree.HeightM),
                    (record.AreaId, record.SpeciesId, record.XM, record.ZM, record.DeathYear, record.Cause, record.DbhCm, record.HeightM),
                    "deadwood fields are the dead stem's values");
                T.Eq(F32.Bits(tree.DbhCm * tree.DbhCm * 0.00007854f * (tree.HeightM * 0.5f)), F32.Bits(record.EstimatedVolumeM3), "volume");
                T.True(!after.Events.SelectMany(e => e.Areas).SelectMany(a => a.Fells).Any(f => f.TreeId == death.TreeId), "never a Fell record");
                T.True(!after.Areas[i].CropTreeIds.Contains(death.TreeId), "no Crop mark on a dead stem");
            }
            var living = after.Areas[i].Trees.Where(t => t.IsLiving).ToArray();
            Near(OracleRd(living.Length, after.Areas[i].AreaHa, living), report.After.RelativeDensity, 0, areaResult.AreaId + " RD from the rack alone");
            // Next year's CI excludes this year's dead: they are not in the connected woodland any more.
            var dead = report.Deaths.Select(d => d.TreeId).ToHashSet();
            T.True(CrownModel.Woodland(after.Areas.SelectMany(a => a.Trees)).All(t => !dead.Contains(t.TreeId)), "dead leave the woodland CI set");
        }
        var ids = s4.Areas.SelectMany(a => a.Deadwood).Select(d => d.DeadwoodId).ToArray();
        T.Eq(ids.Length, ids.Distinct().Count(), "deadwood IDs unique");
        T.Eq(start.Events.Count, after.Events.Count, "a year adds no work event");
    }

    /// <summary>A fixed Stage 4 script (Fells in years 0 and 3, Prunes in years 0 and 5, ten years), fingerprinted with the Stage 4 view.</summary>
    private static string RunFingerprint(Stage4Options options)
    {
        var c = Stage4(options);
        var w = new Model0FingerprintTests.Fingerprint();
        string? pruned = null;
        for (int year = 0; year < 10; year++)
        {
            var v = c.ReadView();
            if (year is 0 or 3)
            {
                foreach (var (areaId, count) in new[] { ("MA-A", 12), ("MA-B", 3) })
                    foreach (var tree in v.Areas.Single(a => a.AreaId == areaId).Trees.Where(t => t.IsLiving).Take(count))
                        v = c.AddPending(Fence(v), areaId, tree.TreeId, "S4 fingerprint selection").View;
            }
            if (year is 0 or 5)
            {
                pruned ??= PruneCandidate(v).TreeId;
                v = c.AddPrune(Fence(v), "MA-B", pruned, year == 0 ? 3f : 4.5f).View;
            }
            if (v.Pending.Count > 0 || c.ReadStage4View().PendingPrunes.Count > 0)
            {
                v = c.Approve(Fence(v)).View;
                v = c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
            }
            var result = c.Advance(Fence(v), false);
            foreach (var area in result.Annual!.Areas)
            {
                w.Text(area.AreaId);
                if (area.Report is { } report) w.Report(report); else w.Text("carried");
            }
        }
        var s4 = c.ReadStage4View();
        foreach (var area in s4.Areas)
        {
            w.Text(area.AreaId + "|" + BitConverter.DoubleToUInt64Bits(area.RelativeDensity).ToString("x16") + "|" +
                area.AnnualNaturalDeaths + "|" + area.CumulativeNaturalDeaths);
            foreach (var d in area.Deadwood)
                w.Text(string.Join("|", d.DeadwoodId, d.TreeId, d.AreaId, d.SpeciesId, F32.Hex(F32.Bits(d.XM)), F32.Hex(F32.Bits(d.ZM)),
                    d.DeathYear, d.Cause, F32.Hex(F32.Bits(d.DbhCm)), F32.Hex(F32.Bits(d.HeightM)), F32.Hex(F32.Bits(d.EstimatedVolumeM3))));
            foreach (var t in area.Trees)
                w.Text(string.Join("|", t.TreeId, t.LifeState, t.DeathCause ?? "-", t.DeathYear?.ToString(CultureInfo.InvariantCulture) ?? "-",
                    t.PruningLifts, F32.Hex(F32.Bits(t.CrownBaseHeightM)), t.LastPruningYear, F32.Hex(F32.Bits(t.CrownRadiusM))));
        }
        foreach (var p in s4.PruneLog)
            w.Text(string.Join("|", p.AreaId, p.TreeId, F32.Hex(F32.Bits(p.TargetCrownBaseHeightM)), p.EcologicalYear,
                F32.Hex(F32.Bits(p.CrownRadiusBeforeM)), F32.Hex(F32.Bits(p.CrownRadiusAfterM)), p.LiftsAfter));
        return w.Hash();
    }
}
