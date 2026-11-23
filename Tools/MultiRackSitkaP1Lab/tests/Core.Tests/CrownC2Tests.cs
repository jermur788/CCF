using System.Text.Json;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Multi #899 CrownModel 1 (C2, D-094) feature tests: model 0 parity through the new constructor, year-0 equilibrium,
/// determinism, radius bounds, the 15 % relaxation cap, no multi-metre jump after a Fell, the connected-woodland
/// neighbourhood across the shared A/B border, the open-grassland outer perimeter and the fenced Work Plan lifecycle.
/// Author tests, not independent qualification.
/// </summary>
internal static class CrownC2Tests
{
    private const string A = "MA-A";
    private const string B = "MA-B";
    private const CrownModelVersion C2 = CrownModelVersion.AreaScaledEquilibriumC2;
    private static readonly SpeciesProfile Sitka = SpeciesProfile.Sitka;

    internal static void Register()
    {
        T.Add("crown C2-01: an explicit LegacyFrozen session reproduces the pinned model 0 fingerprint; default is model 0", () =>
        {
            T.Eq(Model0FingerprintTests.PinnedMultiAreaHash,
                Model0FingerprintTests.MultiAreaFingerprint(CrownModelVersion.LegacyFrozen), "explicit model 0");
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate());
            T.Eq(CrownModelVersion.LegacyFrozen, c.SelectedCrownModel, "default crown model");
            T.Eq(MultiAreaIdentity.AreaModelVersion, c.ReadView().Source.AreaModelVersion, "model 0 token lineage unchanged");
            T.Refuses("CrownModelUnknown", () => new MultiAreaContinuation(MultiAreaGenerator.Generate(), (CrownModelVersion)7));
            T.Eq(CrownModel.C2RuleId, "area-scaled-equilibrium-c2-v1", "rule id");
        });

        T.Add("crown C2-00: independent hand-computed C2 radii (literal expected values, not derived from CrownModel)", () =>
        {
            // Hand: r_pot(30 cm) = 0.9415 + 0.07635·30 = 3.232 m.
            //   CI 0  -> 3.232 / sqrt(1)            = 3.232
            //   CI 5  -> 3.232 / sqrt(2)            = 2.2853691167949215
            //   CI 15 -> 3.232 / sqrt(4)            = 1.616
            //   one step from r = 2.0 at CI 5: 2.0 + (2.2853691167949215 − 2.0)·0.15 = 2.042805367519238
            // r_pot(45 cm) = 4.37725; CI 2.5 -> 4.37725 / sqrt(1.5) = 3.574009658865889
            var cases = new (string What, float Actual, double Expected)[]
            {
                ("DBH 30, CI 0", CrownModel.TargetRadiusM(Sitka, 30f, 0f), 3.232),
                ("DBH 30, CI 5", CrownModel.TargetRadiusM(Sitka, 30f, 5f), 2.2853691167949215),
                ("DBH 30, CI 15", CrownModel.TargetRadiusM(Sitka, 30f, 15f), 1.616),
                ("DBH 45, CI 2.5", CrownModel.TargetRadiusM(Sitka, 45f, 2.5f), 3.574009658865889),
                ("step r 2.0, DBH 30, CI 5", CrownModel.NextRadiusM(Sitka, 2.0f, 30f, 5f), 2.042805367519238),
            };
            foreach (var (what, actual, expected) in cases)
            {
                T.True(Math.Abs(actual - expected) <= 1e-6, $"{what}: {actual:R} vs hand {expected:R}");
                Console.WriteLine($"  C2 hand oracle {what}: engine {actual:R}, hand {expected:R}, |diff| {Math.Abs(actual - expected):E2}");
            }

            // End to end through the woodland Hegyi CI: target DBH 30 at (0,0); neighbours DBH 30 at 4 m and DBH 60 at 3 m.
            // Hand CI = (30/30)/4 + (60/30)/3 = 0.9166666666666666; r_0 = 3.232 / sqrt(1 + 0.18333…) = 2.971103650398022.
            var target = new AnnualTree("H-0", 0f, 0f, 30, 30f, 20f, 3f);
            var woodland = CrownModel.Woodland(new[] { target, new AnnualTree("H-1", 4f, 0f, 30, 30f, 20f, 3f), new AnnualTree("H-2", 0f, 3f, 30, 60f, 20f, 3f) });
            var (ci, neighbours) = CrownModel.WoodlandCompetition(target, woodland);
            T.True(Math.Abs(ci - 0.9166666666666666) <= 1e-6 && neighbours == 2, $"hand CI {ci:R}");
            float r0 = CrownModel.Equilibrate(Sitka, new[] { target }, woodland)[0].CrownRadiusM;
            T.True(Math.Abs(r0 - 2.971103650398022) <= 1e-6, $"hand r_0 {r0:R}");
            Console.WriteLine($"  C2 hand oracle Hegyi end-to-end: CI {ci:R} (hand 0.9166666666666666), r_0 {r0:R} (hand 2.971103650398022)");
        });

        T.Add("crown C2-02: year 0 starts at r_0 = r_target(CI_0) over the connected woodland; nothing else of genesis changes", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var legacy = new MultiAreaContinuation(e).ReadView();
            var c2 = new MultiAreaContinuation(e, C2).ReadView();
            T.Eq(legacy.Experiment.ManifestContentHash, c2.Experiment.ManifestContentHash, "experiment manifest/content hash unchanged");
            T.Eq(MultiAreaIdentity.AreaModelVersionConnectedCrownC2, c2.Source.AreaModelVersion, "model 1 lineage is distinct");
            var woodland = CrownModel.Woodland(legacy.Areas.SelectMany(area => area.Trees));
            int below = 0;
            for (int a = 0; a < legacy.Areas.Count; a++)
            for (int t = 0; t < legacy.Areas[a].Trees.Count; t++)
            {
                var old = legacy.Areas[a].Trees[t];
                var now = c2.Areas[a].Trees[t];
                T.Eq(old with { CrownRadiusM = 0f }, now with { CrownRadiusM = 0f }, "only the crown radius differs: " + old.TreeId);
                T.Eq(F32.Bits(Sitka.PotentialCrownRadiusM(old.DbhCm)), F32.Bits(old.CrownRadiusM), "model 0 starts open-grown");
                float ci0 = CrownModel.WoodlandCompetition(old, woodland).Ci;
                T.Eq(F32.Bits(CrownModel.TargetRadiusM(Sitka, old.DbhCm, ci0)), F32.Bits(now.CrownRadiusM), "r_0 = r_target(CI_0): " + old.TreeId);
                T.True(now.CrownRadiusM >= 0.1f && now.CrownRadiusM <= old.CrownRadiusM, "0.1 <= r_0 <= r_pot");
                if (ci0 > 0f && now.CrownRadiusM < old.CrownRadiusM) below++;
            }
            T.True(below > 200, "competing trees start below open-grown potential (" + below + ")");
            Console.WriteLine("  C2 year-0 radius A " + Dist(c2, A) + " | B " + Dist(c2, B));
            Console.WriteLine("  M0 year-0 radius A " + Dist(legacy, A) + " | B " + Dist(legacy, B));
        });

        T.Add("crown C2-03: model 1 is deterministic and area-order independent", () =>
        {
            string first = Model0FingerprintTests.MultiAreaFingerprint(C2);
            T.Eq(first, Model0FingerprintTests.MultiAreaFingerprint(C2), "two runs identical");
            T.True(first != Model0FingerprintTests.PinnedMultiAreaHash, "model 1 really differs from model 0");
            Console.WriteLine("  model1 multi-area fingerprint " + first);
            var v = new MultiAreaContinuation(MultiAreaGenerator.Generate(), C2).ReadView();
            var woodland = CrownModel.Woodland(v.Areas.SelectMany(area => area.Trees));
            var a1 = MultiAreaAnnual.Step(new AnnualState(0, Area(v, A).Trees), Area(v, A).SimulationSeed, Area(v, A).AreaHa, C2, woodland);
            var b1 = MultiAreaAnnual.Step(new AnnualState(0, Area(v, B).Trees), Area(v, B).SimulationSeed, Area(v, B).AreaHa, C2, woodland);
            var b2 = MultiAreaAnnual.Step(new AnnualState(0, Area(v, B).Trees), Area(v, B).SimulationSeed, Area(v, B).AreaHa, C2, woodland);
            var a2 = MultiAreaAnnual.Step(new AnnualState(0, Area(v, A).Trees), Area(v, A).SimulationSeed, Area(v, A).AreaHa, C2, woodland);
            T.True(Json(a1.Report).SequenceEqual(Json(a2.Report)) && Json(b1.Report).SequenceEqual(Json(b2.Report)), "A-then-B equals B-then-A");
            T.Refuses("CrownWoodlandRequired", () => AnnualRules.Calculate(new AnnualState(0, Area(v, A).Trees), 1, 0.1f, C2, null));
        });

        T.Add("crown C2-04: ten years: radii finite, within [0.1, r_pot], exactly one 15 % relaxation step towards r_target per year", () =>
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), C2);
            double maxStep = 0.0;
            for (int year = 0; year < 10; year++)
            {
                var before = c.ReadView();
                var woodland = CrownModel.Woodland(before.Areas.SelectMany(area => area.Trees));
                var after = c.Advance(Fence(before), false).View;
                foreach (var area in after.Areas)
                foreach (var tree in area.Trees.Where(t => t.Death is null || t.Death.EcologicalYear == year + 1))
                {
                    var old = Area(before, area.AreaId).Trees.Single(t => t.TreeId == tree.TreeId);
                    if (!old.IsLiving) continue;
                    float ci = CrownModel.WoodlandCompetition(old, woodland).Ci;
                    float target = CrownModel.TargetRadiusM(Sitka, tree.DbhCm, ci);
                    T.Eq(F32.Bits(CrownModel.NextRadiusM(Sitka, old.CrownRadiusM, tree.DbhCm, ci)), F32.Bits(tree.CrownRadiusM), "C2 step " + tree.TreeId);
                    float step = tree.CrownRadiusM - old.CrownRadiusM;
                    T.True(float.IsFinite(tree.CrownRadiusM) && tree.CrownRadiusM >= 0.1f, "finite and >= floor");
                    T.True(tree.CrownRadiusM <= Sitka.PotentialCrownRadiusM(tree.DbhCm) + 1e-6f, "never above open-grown potential");
                    T.True(MathF.Abs(step) <= 0.15f * MathF.Abs(target - old.CrownRadiusM) * 1.000001f + 1e-6f, "15 % cap");
                    maxStep = Math.Max(maxStep, Math.Abs(step));
                }
            }
            T.True(maxStep < 0.5, "no year moves any crown by half a metre (max " + maxStep.ToString("F4") + " m)");
            Console.WriteLine("  C2 largest single-year radius change over 10 untreated years: " + maxStep.ToString("F4") + " m");
        });

        T.Add("crown C2-05: a real 12-tree Fell causes no multi-metre jump; neighbours relax by one capped step", () =>
        {
            var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), C2);
            var fell = ClusterInA(c.ReadView(), 12);
            var v = ExecuteFell(c, fell);
            var woodland = CrownModel.Woodland(v.Areas.SelectMany(area => area.Trees));
            var next = c.Advance(Fence(v), false).View;
            double maxStep = 0.0;
            foreach (var area in next.Areas)
            foreach (var tree in area.Trees)
            {
                var old = Area(v, area.AreaId).Trees.Single(t => t.TreeId == tree.TreeId);
                if (!old.IsLiving) continue;
                maxStep = Math.Max(maxStep, Math.Abs(tree.CrownRadiusM - old.CrownRadiusM));
                float ci = CrownModel.WoodlandCompetition(old, woodland).Ci;
                T.Eq(F32.Bits(CrownModel.NextRadiusM(Sitka, old.CrownRadiusM, tree.DbhCm, ci)), F32.Bits(tree.CrownRadiusM), "capped step");
            }
            T.True(maxStep < 0.5, "max one-year change after the Fell " + maxStep.ToString("F4") + " m");
            Console.WriteLine("  C2 largest radius change in the year after a 12-tree Fell: " + maxStep.ToString("F4") + " m");
        });

        T.Add("crown C2-06: a stem near the shared A/B border sees its cross-border neighbours; an outer-perimeter stem sees none invented", () =>
        {
            var e = MultiAreaGenerator.Generate();
            var v = new MultiAreaContinuation(e, C2).ReadView();
            var legacy = new MultiAreaContinuation(e).ReadView();
            var woodland = CrownModel.Woodland(legacy.Areas.SelectMany(area => area.Trees));
            var all = legacy.Areas.SelectMany(area => area.Trees).ToArray();

            // The A tree with the most B trees within 8 m.
            var border = Area(legacy, A).Trees.OrderByDescending(t => Within8(t, Area(legacy, B).Trees)).ThenBy(t => t.TreeId, StringComparer.Ordinal).First();
            int crossCount = Within8(border, Area(legacy, B).Trees);
            T.True(crossCount > 0, "a stem near the shared border has B neighbours within 8 m");
            var areaOnly = AnnualRules.Competition(border, Area(legacy, A).Trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray());
            var connected = CrownModel.WoodlandCompetition(border, woodland);
            T.Eq(areaOnly.Neighbours + crossCount, connected.Neighbours, "woodland neighbours = own-area + cross-border");
            T.True(connected.Ci > areaOnly.Ci, "cross-border neighbours raise CI");
            var r0 = Area(v, A).Trees.Single(t => t.TreeId == border.TreeId).CrownRadiusM;
            T.Eq(F32.Bits(CrownModel.TargetRadiusM(Sitka, border.DbhCm, connected.Ci)), F32.Bits(r0), "r_0 uses the woodland CI");
            T.True(r0 < CrownModel.TargetRadiusM(Sitka, border.DbhCm, areaOnly.Ci), "and not the isolated-area CI");
            Console.WriteLine($"  border stem {border.TreeId}: own-area neighbours {areaOnly.Neighbours}, cross-border {crossCount}, CI {areaOnly.Ci:F3} -> {connected.Ci:F3}");

            // Outer perimeter: westernmost A stem and easternmost B stem border open grassland.
            foreach (var edge in new[] { Area(legacy, A).Trees.MinBy(t => t.XM)!, Area(legacy, B).Trees.MaxBy(t => t.XM)! })
            {
                var w = CrownModel.WoodlandCompetition(edge, woodland);
                int real = all.Count(t => t.TreeId != edge.TreeId && Dist(edge, t) <= 8f);
                T.Eq(real, w.Neighbours, "only real trees within 8 m count: " + edge.TreeId);
                string own = edge.TreeId[..4];
                T.Eq(0, all.Count(t => !t.TreeId.StartsWith(own, StringComparison.Ordinal) && Dist(edge, t) <= 8f), "no cross-border tree near the outer edge");
                var ownTrees = all.Where(t => t.TreeId.StartsWith(own, StringComparison.Ordinal)).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
                T.Eq(F32.Bits(AnnualRules.Competition(edge, ownTrees).Ci), F32.Bits(w.Ci), "outer stem: woodland CI equals own-area CI (no ghosts)");
            }
        });

        foreach (var model in new[] { CrownModelVersion.LegacyFrozen, C2 })
        {
            T.Add($"crown C2-07 ({model}): mark -> invalid command refused unchanged -> approve (no ecological change) -> execute (only approved, same year) -> advance", () =>
            {
                var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), model);
                var genesis = c.ReadView();
                var canopy0 = c.ReadCanopyLight();
                int recomputes = c.CanopyRecomputeCount;
                var fell = ClusterInA(genesis, 10);
                var v = genesis;
                foreach (var id in fell) v = c.AddPending(Fence(v), A, id, "lifecycle test").View;
                var marked = c.ReadView();
                T.True(TreesJson(marked).SequenceEqual(TreesJson(genesis)), "marking changes no tree");

                // Invalid commands: wrong-area tree, a stale fence. Each is refused and changes nothing.
                string bTree = Area(v, B).Trees[0].TreeId;
                T.Refuses("MultiAreaWrongArea", () => c.AddPending(Fence(marked), A, bTree, "wrong area"));
                T.Refuses("PlayerPlanStateStale", () => c.Approve(Fence(genesis)));
                T.True(Json(c.ReadView()).SequenceEqual(Json(marked)), "refusals left the whole view unchanged");

                var approved = c.Approve(Fence(marked)).View;
                T.True(TreesJson(approved).SequenceEqual(TreesJson(genesis)), "approve fells nothing");
                T.True(ReferenceEquals(canopy0, c.ReadCanopyLight()) && c.CanopyRecomputeCount == recomputes, "pending/approve recompute nothing");

                var executed = c.Execute(Fence(approved), approved.Approved!.PlanId, approved.Approved.ExecutionKey).View;
                T.Eq(0, executed.Source.EcologicalYear, "Execute does not advance the year");
                var died = executed.Areas.SelectMany(area => area.Trees).Where(t => !t.IsLiving).Select(t => t.TreeId).Order(StringComparer.Ordinal).ToArray();
                T.True(died.SequenceEqual(fell.Order(StringComparer.Ordinal)), "exactly the approved trees were felled");
                foreach (var area in executed.Areas)
                foreach (var tree in area.Trees.Where(t => t.IsLiving))
                    T.Eq(Area(genesis, area.AreaId).Trees.Single(t => t.TreeId == tree.TreeId), tree, "survivors untouched by Execute");
                var canopy1 = c.ReadCanopyLight();
                T.True(!ReferenceEquals(canopy0, canopy1) && c.CanopyRecomputeCount == recomputes + 1, "Execute triggers one recompute");

                var advanced = c.Advance(Fence(executed), false).View;
                T.Eq(1, advanced.Source.EcologicalYear, "the year advances only on Advance");
                T.True(!ReferenceEquals(canopy1, c.ReadCanopyLight()) && c.CanopyRecomputeCount == recomputes + 2, "annual growth triggers one recompute");
            });
        }
    }

    // ------------------------------------------------------------------------------------------------ helpers

    internal static MultiAreaFence Fence(MultiAreaSessionView v) => new(v.SessionId, v.Source, v.PlanStateRevision);
    internal static MultiAreaAreaView Area(MultiAreaSessionView v, string id) => v.Areas.Single(area => area.AreaId == id);
    private static byte[] Json(object value) => JsonSerializer.SerializeToUtf8Bytes(value);
    private static byte[] TreesJson(MultiAreaSessionView v) => Json(v.Areas.Select(area => area.Trees).ToArray());
    private static float Dist(AnnualTree a, AnnualTree b)
    {
        float dx = a.XM - b.XM, dz = a.ZM - b.ZM;
        return (float)Math.Sqrt(dx * dx + dz * dz);
    }
    private static int Within8(AnnualTree t, IEnumerable<AnnualTree> others) => others.Count(o => o.IsLiving && Dist(t, o) <= 8f);

    private static string Dist(MultiAreaSessionView v, string areaId)
    {
        var r = Area(v, areaId).Trees.Where(t => t.IsLiving).Select(t => (double)t.CrownRadiusM).Order().ToArray();
        double P(double q) => r[(int)Math.Round(q * (r.Length - 1))];
        return $"n={r.Length} min {r[0]:F3} p25 {P(.25):F3} median {P(.5):F3} p75 {P(.75):F3} max {r[^1]:F3} mean {r.Average():F3} m";
    }

    /// <summary>The n living Area A trees nearest (35, 9) m, a patch beside the shared A/B border (ties by ordinal ID).</summary>
    internal static string[] ClusterInA(MultiAreaSessionView v, int n) => Area(v, A).Trees.Where(t => t.IsLiving)
        .OrderBy(t => (t.XM - 35f) * (t.XM - 35f) + (t.ZM - 9f) * (t.ZM - 9f)).ThenBy(t => t.TreeId, StringComparer.Ordinal)
        .Take(n).Select(t => t.TreeId).ToArray();

    internal static MultiAreaSessionView ExecuteFell(MultiAreaContinuation c, IEnumerable<string> treeIds)
    {
        var v = c.ReadView();
        foreach (var id in treeIds) v = c.AddPending(Fence(v), A, id, "matched-control Fell").View;
        v = c.Approve(Fence(v)).View;
        return c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey).View;
    }
}
