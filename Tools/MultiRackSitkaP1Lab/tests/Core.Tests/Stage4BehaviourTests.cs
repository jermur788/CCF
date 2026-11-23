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
            // The expectation is built from the INPUT trees only (independent Hegyi, growth, S, V, P, p and rolls); nothing
            // from the report feeds it. The report is then compared against it.
            var e = ExpectMortality(trees, trees, areaHa, seed, 1);
            var report = AnnualRules.CalculateStage4(new AnnualState(0, trees), seed, areaHa, CrownModel.Woodland(trees), _ => 0, Stage4Options.Default);
            T.True(e.PostGrowthRd > 0.6 && e.PostGrowthRd < 1.0, "mid-range RD (" + e.PostGrowthRd.ToString("0.0000", CultureInfo.InvariantCulture) + ")");
            T.True(e.Ids.All(id => e.Probability[id] > 0.0), "every tree has a positive hazard and a roll");
            AssertMortality(report, e, "M-02");
            for (int i = 0; i < e.Ids.Count; i++)
            for (int j = 0; j < e.Ids.Count; j++)
                if (e.Suppression[e.Ids[i]] > e.Suppression[e.Ids[j]])
                    T.True(e.Probability[e.Ids[i]] >= e.Probability[e.Ids[j]], "more suppressed => higher p");
            T.Eq(0, e.Boundary.Count, "no boundary kills below RD 1.0");
            T.True(report.Deaths.All(d => d.Mechanism == "hazard" && d.Cause == LifeStates.DensitySuppression && d.EcologicalYear == 1),
                "hazard deaths only, cause DensitySuppression, year 1");
            Console.WriteLine(FormattableString.Invariant(
                $"  S4 mid-range (independent oracle): RD {e.PostGrowthRd:0.0000}, P {e.Pressure:0.0000}, p range {e.Probability.Values.Min():0.000000}-{e.Probability.Values.Max():0.000000}, hazard {e.Hazard.Count}, report deaths {report.Deaths.Count}"));
        });

        T.Add("S4 M-03: RD above 1.0 triggers deterministic boundary kills (S desc, TreeId asc) until RD <= 1.0", () =>
        {
            var trees = Grid("MA-T", 10, 10, 2f, i => 28f + (i * 5 % 7));
            const float areaHa = 0.04f;
            const int seed = 777;
            // Hazard victims come from SimulationRandom.Roll on "ADULT-MORT-"+TreeId against the independent p; boundary victims
            // from those independent survivors in S-desc/TreeId-asc order until the oracle RD is <= 1.0.
            var e = ExpectMortality(trees, trees, areaHa, seed, 1);
            T.True(e.PostGrowthRd > 1.0, "oracle post-growth RD > 1.0 (" + e.PostGrowthRd + ")");
            T.True(e.Hazard.Count > 0 && e.Boundary.Count > 0, "both hazard and boundary victims expected");
            T.True(e.FinalRd <= 1.0, "oracle RD after the boundary <= 1.0");
            T.True(e.RdBeforeLastBoundaryKill > 1.0, "the last boundary victim was still needed");
            var report = AnnualRules.CalculateStage4(new AnnualState(0, trees), seed, areaHa, CrownModel.Woodland(trees), _ => 0, Stage4Options.Default);
            AssertMortality(report, e, "M-03");
            Console.WriteLine(FormattableString.Invariant(
                $"  S4 boundary (independent oracle): post-growth RD {e.PostGrowthRd:0.0000}, hazard {e.Hazard.Count}, boundary {e.Boundary.Count}, final RD {e.FinalRd:0.0000}; report identical"));
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

        T.Add("S4 D-02: an overstocked two-rack session with boundary deaths: one deadwood per hazard AND boundary victim, end to end", () =>
        {
            var c = new MultiAreaContinuation(BoundaryExperiment(), MultiAreaModelSelection.Stage4);
            var allVictims = new HashSet<string>(StringComparer.Ordinal);
            int boundaryTotal = 0, hazardTotal = 0;
            Console.WriteLine("  S4 boundary-death session (year area: post-growth RD | hazard | boundary | final RD | new deadwood)");
            for (int year = 1; year <= 3; year++)
            {
                var start = c.ReadView();
                var startDeadwood = c.ReadStage4View().Areas.SelectMany(a => a.Deadwood).ToArray();
                var woodland = CrownModel.Woodland(start.Areas.SelectMany(a => a.Trees));
                // Independent expectation for every rack, from the start-of-year committed trees only.
                var expected = start.Areas.Select(a => ExpectMortality(a.Trees, woodland, a.AreaHa, a.SimulationSeed, year)).ToArray();
                var result = Advance(c);
                var after = c.ReadView();
                var s4 = c.ReadStage4View();
                T.Eq(year, after.Source.EcologicalYear, "year advanced");

                // 1. The engine's mortality equals the independent oracle in every rack.
                for (int i = 0; i < start.Areas.Count; i++)
                    AssertMortality(result.Areas[i].Report!, expected[i], start.Areas[i].AreaId + " y" + year);

                // 2. Deadwood: earlier records untouched, then exactly one new record per victim, numbered on from the last.
                var allDeadwood = s4.Areas.SelectMany(a => a.Deadwood).OrderBy(d => d.DeadwoodId, StringComparer.Ordinal).ToArray();
                var previous = startDeadwood.OrderBy(d => d.DeadwoodId, StringComparer.Ordinal).ToArray();
                T.Eq(previous.Length, allDeadwood.Count(d => previous.Any(p => p.DeadwoodId == d.DeadwoodId)), "old IDs still present once");
                foreach (var old in previous)
                    T.Eq(old, allDeadwood.Single(d => d.DeadwoodId == old.DeadwoodId), "record " + old.DeadwoodId + " is immutable");
                var fresh = allDeadwood.Where(d => !previous.Any(p => p.DeadwoodId == d.DeadwoodId)).ToArray();
                int number = previous.Length + 1;
                int k = 0;
                for (int i = 0; i < start.Areas.Count; i++)
                {
                    var area = start.Areas[i];
                    var e = expected[i];
                    var victims = e.Hazard.Concat(e.Boundary).ToArray();
                    var areaFresh = fresh.Where(d => d.AreaId == area.AreaId).ToArray();
                    T.Eq(victims.Length, areaFresh.Length, area.AreaId + " y" + year + ": one deadwood record per victim");
                    foreach (string id in victims)
                    {
                        var record = fresh[k++];
                        var startTree = area.Trees.Single(t => t.TreeId == id);
                        float dbh = e.GrownDbh[id], height = e.GrownHeight[id];
                        T.Eq(FallenDeadwoodRecord.IdFor(number++), record.DeadwoodId, "sequential, never reused ID");
                        T.Eq((id, area.AreaId, "sitka-spruce", F32.Bits(startTree.XM), F32.Bits(startTree.ZM), year, LifeStates.DensitySuppression,
                                F32.Bits(dbh), F32.Bits(height)),
                            (record.TreeId, record.AreaId, record.SpeciesId, F32.Bits(record.XM), F32.Bits(record.ZM), record.DeathYear, record.Cause,
                                F32.Bits(record.DbhCm), F32.Bits(record.HeightM)), id + " deadwood fields from the oracle");
                        T.Eq(F32.Bits(dbh * dbh * 0.00007854f * (height * 0.5f)), F32.Bits(record.EstimatedVolumeM3), id + " float32 volume");
                        T.True(allVictims.Add(id), id + " dies only once in the session");

                        // Life state: biologically dead, never a stump, never felled, never a Crop Tree.
                        var view = s4.Areas[i].Trees.Single(t => t.TreeId == id);
                        T.Eq((LifeState.BiologicallyDead, LifeStates.DensitySuppression, (int?)year), (view.LifeState, view.DeathCause, view.DeathYear),
                            id + " BiologicallyDead, cause, year");
                        T.True(view.LifeState != LifeState.FelledStump, id + " not a FelledStump");
                        T.True(!after.Areas[i].CropTreeIds.Contains(id), id + " no Crop mark");
                    }
                    foreach (string id in e.Boundary.Take(1).Concat(e.Boundary.Skip(1).TakeLast(1)))
                    {
                        var r = areaFresh.Single(d => d.TreeId == id);
                        Console.WriteLine(FormattableString.Invariant(
                            $"    boundary deadwood {r.DeadwoodId} {r.TreeId} {r.AreaId} {r.SpeciesId} ({r.XM:0.000},{r.ZM:0.000}) y{r.DeathYear} {r.Cause} DBH {r.DbhCm:0.000} cm H {r.HeightM:0.000} m V {r.EstimatedVolumeM3:0.000000} m3 S {e.Suppression[id]:0.000000}"));
                    }
                    hazardTotal += e.Hazard.Count;
                    boundaryTotal += e.Boundary.Count;
                    string span = areaFresh.Length > 0 ? " (" + areaFresh[0].DeadwoodId + ".." + areaFresh[^1].DeadwoodId + ")" : "";
                    Console.WriteLine(FormattableString.Invariant(
                        $"    y{year} {area.AreaId}: {e.PostGrowthRd:0.0000} | {e.Hazard.Count,2} | {e.Boundary.Count,2} | {e.FinalRd:0.0000} | {areaFresh.Length,2}{span}"));
                }
                T.Eq(fresh.Length, k, "no deadwood record without a victim");
                T.Eq(allDeadwood.Length, allDeadwood.Select(d => d.DeadwoodId).Distinct().Count(), "deadwood IDs unique");
                T.Eq(allDeadwood.Length, allDeadwood.Select(d => d.TreeId).Distinct().Count(), "one record per tree, ever");

                // 3. No Fell receipt, no work event and therefore no harvest/timber record for any victim.
                T.Eq(0, after.Events.Count, "an annual step creates no work event / Fell receipt");
                T.True(after.Events.SelectMany(ev => ev.Areas).SelectMany(a => a.Fells).All(f => !allVictims.Contains(f.TreeId)), "no Fell of a victim");

                // 4. Victims are absent from next year's woodland CI, cover and light: the session's read-out equals one evaluated
                //    with the victims deleted outright, and differs from one where they are put back alive.
                var nextWoodland = CrownModel.Woodland(after.Areas.SelectMany(a => a.Trees));
                T.True(nextWoodland.All(t => !allVictims.Contains(t.TreeId)), "victims are not in the next woodland CI set");
                foreach (var t in nextWoodland)
                {
                    var withoutVictims = after.Areas.SelectMany(a => a.Trees).Where(x => !allVictims.Contains(x.TreeId));
                    T.Eq(F32.Bits(HegyiOracle(t, withoutVictims)), F32.Bits(CrownModel.WoodlandCompetition(t, nextWoodland).Ci),
                        t.TreeId + " next-year woodland CI ignores victims");
                }
                var session = c.ReadCanopyLight();
                var removed = EvaluateCanopy(after, t => allVictims.Contains(t.TreeId) ? null : t);
                AssertSameCanopy(removed, session, "y" + year + " canopy/light with victims deleted");
                T.True(session.Trees.All(g => !allVictims.Contains(g.TreeId)), "victims have no growing-space row");
                var revived = EvaluateCanopy(after, t => fresh.Any(d => d.TreeId == t.TreeId) ? t with { Death = null } : t);
                T.True(fresh.Length == 0 || revived.Areas.Zip(session.Areas).Any(p =>
                        p.First.Cover.CoveredM2 != p.Second.Cover.CoveredM2 || p.First.Light.MeanLightIndex != p.Second.Light.MeanLightIndex),
                    "the comparison is sensitive: reviving this year's victims changes cover/light");
            }
            T.True(boundaryTotal > 0, "boundary deaths reached the session (" + boundaryTotal + ")");
            T.True(hazardTotal > 0, "hazard deaths reached the session (" + hazardTotal + ")");
            var final = c.ReadStage4View().Areas.SelectMany(a => a.Deadwood).ToArray();
            T.Eq(hazardTotal + boundaryTotal, final.Length, "total deadwood = total victims");
            T.Eq(string.Join(",", Enumerable.Range(1, final.Length).Select(FallenDeadwoodRecord.IdFor)),
                string.Join(",", final.Select(d => d.DeadwoodId).OrderBy(x => x, StringComparer.Ordinal)), "IDs DW0001.. contiguous, none reused");
            Console.WriteLine("  S4 boundary-death session totals: hazard " + hazardTotal + ", boundary " + boundaryTotal + ", deadwood " + final.Length);
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

        T.Add("S4 P-04: consented Year+1 cancels an approved Prune at zero lifts; Execute of the cancelled plan is refused", () =>
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

    /// <summary>
    /// The independently derived outcome of one Stage 4 annual step for one rack (site 1.0, no phases, no lifts).
    /// Hazard is in TreeId order; Boundary is in kill order. Keys are TreeIds of the rack's living start-of-year trees.
    /// </summary>
    private sealed record MortalityExpectation(IReadOnlyList<string> Ids, IReadOnlyDictionary<string, float> Ci,
        IReadOnlyDictionary<string, float> GrownDbh, IReadOnlyDictionary<string, float> GrownHeight, double PostGrowthRd, double Pressure,
        double? MeanVulnerability, IReadOnlyDictionary<string, double> Suppression, IReadOnlyDictionary<string, double> Probability,
        IReadOnlyDictionary<string, float> Roll, IReadOnlyList<string> Hazard, IReadOnlyList<string> Boundary, double FinalRd,
        double RdBeforeLastBoundaryKill);

    /// <summary>
    /// Independent oracle of the per-rack Stage 4 step, written from the spec formulas and the Unity constants:
    ///   CI (rack) = 8 m Hegyi over the rack's own living trees; CI_woodland = the same over <paramref name="woodland"/>;
    ///   DBH' = clamp(DBH + 1.2·clamp01(1−DBH/120)·1/(1+CI_woodland/5), 1, 200); H' = clamp(H + (NextHeight(H, age) − H), 0.1, 60);
    ///   RD = (N/areaHa)·(Dq/25)^2.063/1868 on the grown stems; P = max(0, (RD−0.6)/(1−0.6));
    ///   S = 1 − 1/(1 + CI/5); V = clamp01(S)^4; p = min(0.5, 0.08·P·P·V/mean V); roll = Roll(1, "ADULT-MORT-"+id, year, seed);
    ///   hazard death iff roll &lt; p; then, over the hazard survivors in S-desc/TreeId-asc order, boundary kills while RD &gt; 1.0
    ///   and more than one stem remains. Float32/double expression order follows the written rule so equality can be exact.
    /// Only the frozen height law (SitkaGrowthModel.NextHeight) and the RNG are shared with the engine.
    /// </summary>
    private static MortalityExpectation ExpectMortality(IEnumerable<AnnualTree> rackTrees, IEnumerable<AnnualTree> woodland, float areaHa,
        int seed, int year)
    {
        var rack = rackTrees.Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        var wood = woodland.Where(t => t.IsLiving).ToArray();
        int n = rack.Length;
        var ids = rack.Select(t => t.TreeId).ToArray();
        var ci = new float[n];
        var dbh = new float[n];
        var height = new float[n];
        for (int i = 0; i < n; i++)
        {
            var t = rack[i];
            ci[i] = HegyiOracle(t, rack);
            float wci = HegyiOracle(t, wood);
            float response = 1f / (1f + wci / 5f);
            float potential = 1.2f * 1f * Math.Clamp(1f - t.DbhCm / 120f, 0f, 1f);
            dbh[i] = Math.Clamp(t.DbhCm + potential * response, 1f, 200f);
            float heightGrowth = (SitkaGrowthModel.NextHeight(t.HeightM, t.AgeYears) - t.HeightM) * 1f;
            height[i] = Math.Clamp(t.HeightM + heightGrowth, 0.1f, 60f);
        }
        double SumSq(IEnumerable<int> idx) { double s = 0.0; foreach (int i in idx) s += (double)dbh[i] * dbh[i]; return s; }
        static double Rd(int count, float area, double sumSq) =>
            count == 0 ? 0.0 : (double)(count / area) * Math.Pow(Math.Sqrt(sumSq / count) / 25.0, 2.063) / 1868.0;

        double postRd = Rd(n, areaHa, SumSq(Enumerable.Range(0, n)));
        double pressure = Math.Max(0.0, (postRd - 0.6) / (1.0 - 0.6));
        var s = new Dictionary<string, double>(StringComparer.Ordinal);
        var p = new Dictionary<string, double>(StringComparer.Ordinal);
        var roll = new Dictionary<string, float>(StringComparer.Ordinal);
        var hazard = new List<string>();
        var boundary = new List<string>();
        var survivors = Enumerable.Range(0, n).ToList();
        double? meanV = null;
        double rdBeforeLast = double.NaN;
        if (pressure > 0.0)
        {
            var sup = ci.Select(x => 1.0 - 1.0 / (1.0 + (double)(x / 5f))).ToArray();
            double sum = 0.0;
            foreach (double v in sup) sum += Math.Pow(Math.Max(0.0, Math.Min(1.0, v)), 4.0);
            meanV = sum / n;
            survivors.Clear();
            for (int i = 0; i < n; i++)
            {
                s[ids[i]] = sup[i];
                double v = Math.Pow(Math.Max(0.0, Math.Min(1.0, sup[i])), 4.0);
                double prob = meanV.Value <= 0.0 ? 0.0 : Math.Min(0.5, 0.08 * pressure * pressure * v / meanV.Value);
                p[ids[i]] = prob;
                if (prob > 0.0)
                {
                    float r = SimulationRandom.Roll(DerivedManifest.RngModelVersion, "ADULT-MORT-" + ids[i], year, seed);
                    roll[ids[i]] = r;
                    if (r < prob) { hazard.Add(ids[i]); continue; }
                }
                survivors.Add(i);
            }
            int count = survivors.Count;
            double sumSq = SumSq(survivors);
            if (count > 1 && Rd(count, areaHa, sumSq) > 1.0)
                foreach (int i in survivors.OrderByDescending(i => sup[i]).ThenBy(i => ids[i], StringComparer.Ordinal).ToArray())
                {
                    if (count <= 1 || Rd(count, areaHa, sumSq) <= 1.0) break;
                    rdBeforeLast = Rd(count, areaHa, sumSq);
                    count--;
                    sumSq -= (double)dbh[i] * dbh[i];
                    boundary.Add(ids[i]);
                }
        }
        var alive = Enumerable.Range(0, n).Where(i => !hazard.Contains(ids[i]) && !boundary.Contains(ids[i])).ToArray();
        return new MortalityExpectation(ids, Map(ci), Map(dbh), Map(height), postRd, pressure, meanV, s, p, roll, hazard, boundary,
            Rd(alive.Length, areaHa, SumSq(alive)), rdBeforeLast);

        Dictionary<string, float> Map(float[] values) =>
            Enumerable.Range(0, n).ToDictionary(i => ids[i], i => values[i], StringComparer.Ordinal);
    }

    /// <summary>Compares one engine report with an independent expectation, bit for bit where the values are deterministic.</summary>
    private static void AssertMortality(AnnualReport report, MortalityExpectation e, string label)
    {
        static ulong D(double v) => BitConverter.DoubleToUInt64Bits(v);
        var rows = report.Trees.Where(r => r.Before.IsLiving).ToArray();
        T.Eq(string.Join(",", e.Ids), string.Join(",", rows.Select(r => r.Before.TreeId)), label + ": same living roster");
        T.Eq(D(e.PostGrowthRd), D(report.PostGrowth.RelativeDensity), label + ": post-growth RD");
        T.Eq(D(e.Pressure), D(report.PostGrowthDensityPressure), label + ": P");
        T.Eq(e.MeanVulnerability.HasValue, report.MeanVulnerability.HasValue, label + ": mean V present iff P > 0");
        if (e.MeanVulnerability is double mv) T.Eq(D(mv), D(report.MeanVulnerability!.Value), label + ": mean V");
        foreach (var row in rows)
        {
            string id = row.Before.TreeId;
            T.Eq(F32.Bits(e.Ci[id]), F32.Bits(row.PreGrowthCi), label + " " + id + ": rack CI");
            T.Eq(F32.Bits(e.GrownDbh[id]), F32.Bits(row.After.DbhCm), label + " " + id + ": DBH");
            T.Eq(F32.Bits(e.GrownHeight[id]), F32.Bits(row.After.HeightM), label + " " + id + ": height");
            T.Eq(e.Suppression.TryGetValue(id, out double sv) ? D(sv) : (ulong?)null, row.Suppression is double rs ? (ulong?)D(rs) : null, label + " " + id + ": S");
            T.Eq(e.Probability.TryGetValue(id, out double pv) ? D(pv) : (ulong?)null, row.DeathProbability is double rp ? (ulong?)D(rp) : null, label + " " + id + ": p");
            T.Eq(e.Roll.TryGetValue(id, out float rv) ? F32.Bits(rv) : (uint?)null, row.MortalityRoll is float rr ? (uint?)F32.Bits(rr) : null, label + " " + id + ": roll");
            bool dies = e.Hazard.Contains(id) || e.Boundary.Contains(id);
            T.Eq(dies ? LifeState.BiologicallyDead : LifeState.Living, LifeStates.Of(row.After), label + " " + id + ": life state");
        }
        var expectedDeaths = e.Hazard.Select(id => id + "/hazard").Concat(e.Boundary.Select(id => id + "/boundary"));
        T.Eq(string.Join(",", expectedDeaths), string.Join(",", report.Deaths.Select(d => d.TreeId + "/" + d.Mechanism)),
            label + ": complete victim set, hazard (TreeId order) then boundary (kill order)");
        T.True(report.Deaths.All(d => d.Cause == LifeStates.DensitySuppression && d.EcologicalYear == report.YearAfter), label + ": cause and year");
        T.Eq(D(e.FinalRd), D(report.After.RelativeDensity), label + ": final RD");
    }

    /// <summary>
    /// Two touching 20 × 20 m racks. Area A is a deliberately extreme synthetic 10 × 10 grid at 2 m pitch of 40–46 cm stems: its
    /// post-growth RD is about 4, and because p is capped at 0.5 the hazard alone cannot bring it under 1.0, so boundary victims
    /// are guaranteed in year 1 whatever the rolls (an RD ≈ 2 stand can fall under 1.0 on hazard alone). Area B is a light 4 × 4 grid
    /// at 5 m whose west column is 3.5 m from Area A's east column, so cross-border woodland CI is in play.
    /// </summary>
    private static MultiAreaExperiment BoundaryExperiment()
    {
        static MillimetrePoint P(long x, long y) => new(x, y);
        var a = new List<MultiAreaTreeDefinition>();
        for (int i = 0; i < 100; i++)
            a.Add(new MultiAreaTreeDefinition("MA-A-" + (i + 1).ToString("D3", CultureInfo.InvariantCulture),
                1_000 + (i % 10) * 2_000, 1_000 + (i / 10) * 2_000, 40, 400 + (i * 5 % 7) * 10, 25_000));
        var b = new List<MultiAreaTreeDefinition>();
        for (int i = 0; i < 16; i++)
            b.Add(new MultiAreaTreeDefinition("MA-B-" + (i + 1).ToString("D3", CultureInfo.InvariantCulture),
                22_500 + (i % 4) * 5_000, 2_500 + (i / 4) * 5_000, 30, 200, 16_000));
        return new MultiAreaExperiment("mad-s4-boundary-test", "s4-test", 1, 1, new[]
        {
            new MultiAreaDefinition("MA-A", "Area A", "S4 overstocked boundary test",
                new[] { P(0, 0), P(20_000, 0), P(20_000, 20_000), P(0, 20_000) }, 601, a),
            new MultiAreaDefinition("MA-B", "Area B", "S4 light neighbour",
                new[] { P(20_000, 0), P(40_000, 0), P(40_000, 20_000), P(20_000, 20_000) }, 602, b),
        });
    }

    /// <summary>Canopy/light of the view's committed racks after <paramref name="edit"/> (null deletes the tree outright).</summary>
    private static CanopyLightReport EvaluateCanopy(MultiAreaSessionView v, Func<AnnualTree, AnnualTree?> edit) =>
        CanopyLightEvaluator.Evaluate(v.Areas.Select(a => new CanopyArea(a.AreaId,
                Array.AsReadOnly(a.Polygon.Select(p => new CanopyVertex(p.XM, p.ZM)).ToArray()), a.AreaHa,
                Array.AsReadOnly(a.Trees.Select(edit).Where(t => t is not null).Select(t => t!).ToArray()))).ToArray(),
            CanopyLightEvaluator.DefaultLightCellM, CrownModelVersion.AreaScaledEquilibriumC2);

    private static void AssertSameCanopy(CanopyLightReport expected, CanopyLightReport actual, string what)
    {
        T.Eq(expected.Trees.Count, actual.Trees.Count, what + ": growing-space rows");
        for (int i = 0; i < expected.Trees.Count; i++) T.Eq(expected.Trees[i], actual.Trees[i], what + ": growing space " + expected.Trees[i].TreeId);
        T.Eq(expected.Areas.Count, actual.Areas.Count, what + ": areas");
        for (int i = 0; i < expected.Areas.Count; i++)
        {
            var x = expected.Areas[i];
            var y = actual.Areas[i];
            T.Eq(x.Stand, y.Stand, what + " " + x.AreaId + ": stand");
            T.Eq(x.Cover, y.Cover, what + " " + x.AreaId + ": cover");
            T.Eq((x.Light.SampledAreaM2, x.Light.MeanLightIndex, x.Light.MinLightIndex, x.Light.MaxLightIndex, x.Light.CellsWithCrossBorderInfluence),
                (y.Light.SampledAreaM2, y.Light.MeanLightIndex, y.Light.MinLightIndex, y.Light.MaxLightIndex, y.Light.CellsWithCrossBorderInfluence),
                what + " " + x.AreaId + ": light summary");
            T.True(x.Light.Cells.SequenceEqual(y.Light.Cells), what + " " + x.AreaId + ": every light cell");
        }
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
