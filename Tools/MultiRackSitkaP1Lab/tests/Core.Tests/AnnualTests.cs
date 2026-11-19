using System.Text.Json;
using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

internal static class AnnualTests
{
    private const P1Domain Domain = P1Domain.DerivedP1ThirtyMetreExperimentV1;
    private const int Seed = 20261116;
    private static readonly float AreaHa = F32.FromBits(DerivedManifest.AreaHaBits);

    internal static void Register(string manifestPath, string scratch)
    {
        ContinuationTests.Register(manifestPath);

        AnnualEngine Open() => AnnualEngine.Open(Domain, ManifestGenerator.FixtureId, Path.GetDirectoryName(manifestPath)!);

        T.Add("R-31 T-19: positive pressure uses living-only stand and hazard inputs", () =>
        {
            var dead = Stem("dead", 1, 0, 50) with { Death = new AnnualDeath("dead", 0, "probe", "hazard") };
            const float areaHa = 0.00109f;
            var report = Run(areaHa, Stem("target", 0, 0), Stem("live", 4, 0, 25), dead);
            T.True(report.Trees.Select(r => r.Before.TreeId).SequenceEqual(new[] { "dead", "live", "target" }),
                "merged report has ordinal IDs");
            T.Eq(2, report.Before.LivingCount, "before stand excludes archived stem");
            double squaredDbhSum = 20.0 * 20.0 + 25.0 * 25.0;
            double dq = Math.Sqrt(squaredDbhSum / 2);
            // Preserve the controller's int/float division before widening to double.
            double stemsPerHa = 2 / areaHa;
            Near(stemsPerHa, report.Before.StemsPerHa, 1e-12, "living-only before stems/ha");
            Near(dq, report.Before.QuadraticMeanDbhCm, 1e-12, "living-only before Dq");
            Near(squaredDbhSum * (Math.PI / 40000.0) / areaHa, report.Before.BasalAreaM2PerHa,
                1e-12, "living-only before BA/ha");
            Near(SitkaGrowthModel.RelativeDensity(stemsPerHa, dq), report.Before.RelativeDensity,
                1e-12, "living-only before RD");

            float targetCi = (25f / 20f) / 4f;
            float liveCi = (20f / 25f) / 4f;
            // CI/5 is evaluated as float, then widened for the double suppression arithmetic.
            double targetSuppression = 1.0 - 1.0 / (1.0 + targetCi / 5f);
            double liveSuppression = 1.0 - 1.0 / (1.0 + liveCi / 5f);
            double meanVulnerability = (Math.Pow(targetSuppression, 4) + Math.Pow(liveSuppression, 4)) / 2;
            Near(meanVulnerability, report.MeanVulnerability!.Value, 1e-15, "living-only mean S^4");
            foreach (var (id, ci, suppression) in new[]
                { ("target", targetCi, targetSuppression), ("live", liveCi, liveSuppression) })
            {
                var row = report.Trees.Single(r => r.Before.TreeId == id);
                T.Eq(ci, row.PreGrowthCi, "living-only CI for " + id);
                Near(suppression, row.Suppression!.Value, 1e-15, "living-only suppression for " + id);
                double probability = Math.Min(0.5, 0.08 * report.PostGrowthDensityPressure
                    * report.PostGrowthDensityPressure * Math.Pow(suppression, 4) / meanVulnerability);
                Near(probability, row.DeathProbability!.Value, 1e-14, "living-only hazard for " + id);
            }
            var target = report.Trees.Single(r => r.Before.TreeId == "target");
            T.True(report.PostGrowthDensityPressure > 0, "positive pressure");
            T.Eq(0.3125f, target.PreGrowthCi, "living CI");
            T.Eq(AnnualRules.Grow(target.Before, targetCi).DbhCm, target.After.DbhCm,
                "target DBH uses living-only competition in production Grow");
            T.Eq(0.9411764740943909, (double)(1f / (1f + target.PreGrowthCi / 5f)), "f32 response");
            var mutant = Run(0.00109f, Stem("target", 0, 0), Stem("live", 4, 0, 25), dead with { Death = null });
            float wrong = mutant.Trees.Single(r => r.Before.TreeId == "target").PreGrowthCi;
            T.Eq(2.8125f, wrong, "mutant CI");
            T.Eq(0.6399999856948853, (double)(1f / (1f + wrong / 5f)), "mutant response");
            T.True(wrong != target.PreGrowthCi, "fixture distinguishes dead-inclusion competition");
            var second = AnnualRules.Calculate(new AnnualState(1, report.Trees.Select(r => r.After).ToArray()), Seed, 0.00109f);
            var archived = second.Trees.Single(r => r.Before.TreeId == "dead");
            T.Eq(dead.Death, archived.After.Death, "original death year, cause and mechanism retained");
            T.Eq(dead, archived.After, "archival fields frozen across second calculation");
            T.True(archived.MortalityRoll is null && archived.DeathProbability is null, "no second hazard");
            T.True(second.Deaths.All(d => d.TreeId != "dead"), "cannot die twice");
        });

        T.Add("R-32 T-20: boundary excludes archives and preserves last survivor", () =>
        {
            var dead = Stem("archive", 1, 0, 200) with { Death = new AnnualDeath("archive", 0, "probe", "boundary") };
            var report = Run(0.003f, Stem("A", 0, 0, 70), Stem("B", 0.25f, 0, 30), dead);
            T.True(report.PostGrowth.RelativeDensity > 1, "above strict threshold");
            T.Eq(2, report.PostGrowth.LivingCount, "dead excluded");
            T.Eq(1, report.After.LivingCount, "last survivor");
            T.Eq("B", report.Deaths.Single().TreeId, "descending suppression");
            T.Eq(dead, report.Trees.Single(r => r.Before.TreeId == "archive").After, "archive retained");
            var mutant = Run(0.003f, Stem("A", 0, 0, 70), Stem("B", 0.25f, 0, 30), dead with { Death = null });
            T.True(mutant.PostGrowth.LivingCount != report.PostGrowth.LivingCount, "dead inclusion fails count assertion");
        });

        T.Add("year 1: all 100 trees grow once and the stand matches #723", () =>
        {
            var engine = Open();
            var report = engine.Advance();
            T.Eq(0, report.YearBefore, "initial year");
            T.Eq(1, report.YearAfter, "advanced year");
            T.Eq(100, report.Before.LivingCount, "initial population");
            T.Eq(100, report.PostGrowth.LivingCount, "post-growth population");
            T.Eq(100, report.After.LivingCount, "survivors");
            T.Eq(0, report.Deaths.Count, "no deaths");
            T.Eq(1622, report.Trees.Sum(row => row.Neighbours), "directed pairs");
            T.Eq(12, report.Trees.Count(row => row.Before.DbhCm < 17f), "below-threshold population");
            Near(0.609795828454288, report.Before.RelativeDensity, 1e-12, "initial RD");
            // #723 displays BA/ha at exact 0.09 ha. Convert that reference to the manifest's stored area.
            Near(55.872911409 * 0.09 / AreaHa, report.Before.BasalAreaM2PerHa, 1e-9, "initial BA/ha");
            Near(25.855791791844318, report.PostGrowth.QuadraticMeanDbhCm, 1e-12, "post-growth Dq");
            Near(0.6375836886879149, report.PostGrowth.RelativeDensity, 1e-12, "post-growth RD");
            Near(0.09395922171978738, report.PostGrowthDensityPressure, 1e-12, "hazard pressure");
            Near(0.035410183953663125, report.MeanVulnerability!.Value, 1e-12, "mean vulnerability");
            Near(0.07062668276950532, report.Trees.Sum(row => row.DeathProbability!.Value), 1e-12, "expected deaths");
            T.Eq(report.PostGrowth, report.After, "no density change after growth");
            var max = report.Trees.MaxBy(row => row.PreGrowthCi)!;
            T.Eq("P30-053", max.Before.TreeId, "maximum CI ID");
            T.Eq(8.432238578796387f, max.PreGrowthCi, "maximum CI");
            Near(0.0030975518881867846, max.DeathProbability!.Value, 1e-12, "maximum hazard");
            var first = report.Trees[0];
            Near(33.602428, first.After.DbhCm, 0.000004, "first DBH");
            Near(23.659950, first.After.HeightM, 0.000004, "first height");
            Near(3.359647, first.After.CrownRadiusM, 0.000001, "first crown");
            T.Eq(2780209f / 16777216f, first.MortalityRoll!.Value, "G10 mixed-model roll");
            T.Eq(14399697f / 16777216f,
                SimulationRandom.Roll(SimulationRandom.LegacyModel, "ADULT-MORT-P30-001", 1, Seed), "G11 legacy control");

            for (int i = 0; i < report.Trees.Count; i++)
            {
                var row = report.Trees[i];
                T.Eq(FormattableString.Invariant($"P30-{i + 1:000}"), row.Before.TreeId, "ordinal stem ID");
                T.Eq(row.Before.TreeId, row.After.TreeId, "same stem");
                T.Eq(30, row.Before.AgeYears, "initial age");
                T.Eq(31, row.After.AgeYears, "aged once");
                T.True(row.After.DbhCm > row.Before.DbhCm && row.After.HeightM > row.Before.HeightM, "grew");
                T.True(row.After.IsLiving && row.DeathProbability > 0 && row.MortalityRoll >= row.DeathProbability,
                    "evaluated hazard and survived");
                T.Eq(row.After, engine.State.Trees[i], "committed tree");
            }
        });

        T.Add("year 1: deterministic report, seed/year/DBH perturbations change their respective results", () =>
        {
            var engine = Open();
            var genesis = engine.State;
            var baseline = engine.Advance();
            T.Eq(JsonSerializer.Serialize(baseline), JsonSerializer.Serialize(Open().Advance()), "same clean genesis");
            var newSeed = AnnualRules.Calculate(genesis, Seed + 1, AreaHa);
            var newYear = AnnualRules.Calculate(genesis with { EcologicalYear = 1 }, Seed, AreaHa);
            var first = baseline.Trees[0];
            T.True(newSeed.Trees[0].MortalityRoll != first.MortalityRoll, "seed changes roll");
            T.True(newYear.Trees[0].MortalityRoll != first.MortalityRoll, "year changes roll");
            var trees = genesis.Trees.ToArray();
            trees[0] = trees[0] with { DbhCm = trees[0].DbhCm + 1f };
            var newDbh = AnnualRules.Calculate(genesis with { Trees = trees }, Seed, AreaHa);
            T.True(newDbh.PostGrowth.RelativeDensity != baseline.PostGrowth.RelativeDensity, "DBH changes RD");
            T.True(newDbh.Trees[0].PreGrowthCi != first.PreGrowthCi, "DBH changes CI");
            T.Eq(first.MortalityRoll, newDbh.Trees[0].MortalityRoll, "DBH does not enter roll key");
        });

        T.Add("annual: injected precommit failure leaves genesis intact, retry commits once", () =>
        {
            var engine = Open();
            var genesis = engine.State;
            var expected = JsonSerializer.Serialize(Open().Advance());
            T.Refuses("InjectedFailure", () => engine.Advance(() => throw new P1Refusal("InjectedFailure", "probe")));
            T.True(ReferenceEquals(genesis, engine.State), "identical year-zero snapshot and tree array");
            T.Eq(expected, JsonSerializer.Serialize(engine.Advance()), "retry is a fresh calculation from genesis");
            var committed = engine.State;
            T.Refuses("AnnualAlreadyAdvanced", () => engine.Advance());
            T.True(ReferenceEquals(committed, engine.State), "duplicate attempt preserved committed snapshot");
        });

        T.Add("annual: concurrent requests on one engine commit one year", () =>
        {
            var engine = Open();
            string AdvanceOnce()
            {
                try { return engine.Advance().YearAfter.ToString(); }
                catch (P1Refusal e) { return e.Code; }
            }
            var results = new string[2];
            Parallel.Invoke(() => results[0] = AdvanceOnce(), () => results[1] = AdvanceOnce());
            T.True(results.Order(StringComparer.Ordinal).SequenceEqual(new[] { "1", "AnnualAlreadyAdvanced" }), "exactly one commit");
            T.Eq(1, engine.State.EcologicalYear, "one year advanced");
        });

        T.Add("annual: trusted fixture admission and retained original-domain refusals", () =>
        {
            string dir = Fx.Dir(scratch, "annual-altered");
            string name = Path.GetFileName(manifestPath);
            byte[] bytes = File.ReadAllBytes(manifestPath);
            bytes[0] ^= 1;
            File.WriteAllBytes(Path.Combine(dir, name), bytes);
            T.Refuses("FileHashMismatch", () => AnnualEngine.Open(Domain, ManifestGenerator.FixtureId, dir));
            T.Refuses("UnknownDerivedFixture", () => AnnualEngine.Open(Domain, "oracle-quarter-ha", dir));
            T.Refuses("OriginalAnnualRefused", () => AnnualEngine.Open(P1Domain.OriginalPolygonExploratoryV1, "oracle-quarter-ha", dir));
            T.Refuses("DomainUnavailable", () => AnnualEngine.Open(P1Domain.DerivedUnitySquareParityV1, ManifestGenerator.FixtureId, dir));
            T.Refuses("UnknownDomain", () => AnnualEngine.Open((P1Domain)99, ManifestGenerator.FixtureId, dir));
        });

        T.Add("annual CI: exact 8 m included, just outside excluded, 0.5 m floor applied", () =>
        {
            var report = Run(AreaHa, Stem("target", 0, 0), Stem("edge", 8, 0),
                Stem("outside", 0, MathF.BitIncrement(8f)), Stem("near", 0.25f, 0));
            var target = report.Trees.Single(row => row.Before.TreeId == "target");
            T.Eq(2, target.Neighbours, "two qualifying neighbours");
            T.Eq(2.125f, target.PreGrowthCi, "1/8 + 1/0.5");
            T.Eq(0.0, report.PostGrowthDensityPressure, "below onset");
            T.True(report.Trees.All(row => row.Suppression is null && row.DeathProbability is null
                && row.MortalityRoll is null), "no hazard work below onset");
        });

        T.Add("annual growth: no-CI and CI=5, DBH scale and clamps, crown relaxation, Class III height", () =>
        {
            var stem = Stem("tree", 0, 0);
            var open = AnnualRules.Grow(stem, 0);
            var suppressed = AnnualRules.Grow(stem, 5);
            T.Eq(21f, open.DbhCm, "20 cm open-grown gain");
            T.Eq(20.5f, suppressed.DbhCm, "CI50 halves gain");
            Near(20.921672761019323, open.HeightM, 0.000002, "Class III age 31");
            T.Eq(open.HeightM, suppressed.HeightM, "height independent of CI");
            Near(2.0817275, open.CrownRadiusM, 0.000001, "open crown relaxes toward post-growth potential");
            Near(1.888000625, suppressed.CrownRadiusM, 0.000001, "suppressed crown relaxes");
            T.Eq(120f, AnnualRules.Grow(stem with { DbhCm = 120f }, 0).DbhCm, "potential saturates");
            var clamped = AnnualRules.Grow(stem with { DbhCm = 201f, HeightM = 60f, CrownRadiusM = 0.1f }, float.MaxValue);
            T.Eq(200f, clamped.DbhCm, "DBH clamp (out-of-domain scalar probe)");
            T.Eq(60f, clamped.HeightM, "height clamp");
            T.Eq(0.1f, clamped.CrownRadiusM, "crown floor");
            var small = AnnualRules.Grow(stem with { DbhCm = 0.25f, HeightM = 0.02f }, float.MaxValue);
            T.Eq(1f, small.DbhCm, "DBH floor (out-of-domain scalar probe)");
            T.Eq(0.1f, small.HeightM, "height floor (out-of-domain scalar probe)");
        });

        T.Add("annual mortality: hazard cap, chronological growth, survivors-only boundary and ID tie-break", () =>
        {
            var report = Run(AreaHa, Stem("P30-004", 0, 0.25f, 200), Stem("P30-003", 0.25f, 0.25f, 200),
                Stem("P30-002", 0.25f, 0, 200), Stem("P30-001", 0, 0, 200));
            T.True(report.PostGrowth.RelativeDensity > 1 && report.After.RelativeDensity < 1, "boundary removed excess stocking");
            T.True(report.Trees.All(row => row.DeathProbability == 0.5), "all hazards capped");
            T.Eq(4, report.PostGrowth.LivingCount, "mortality uses fully grown stand");
            T.Eq(2, report.After.LivingCount, "two survivors");
            T.Eq(2, report.Deaths.Count, "one hazard and one boundary death");
            T.Eq(new AnnualDeath("P30-001", 1, "self-thinning", "hazard"), report.Deaths[0], "ordinal hazard first");
            T.Eq(new AnnualDeath("P30-002", 1, "self-thinning", "boundary"), report.Deaths[1], "first remaining ID at equal suppression");
            foreach (var row in report.Trees)
            {
                T.Eq(31, row.After.AgeYears, "age increment before any death");
                T.True(row.After.HeightM > row.Before.HeightM, "victims also grew in height");
                T.Eq(AnnualRules.Grow(row.Before, row.PreGrowthCi), row.After with { Death = null }, "only one growth step");
            }
        });

        T.Add("annual mortality: all hazard victims leave an empty stand and no boundary victims", () =>
        {
            var report = Run(0.003f, Stem("P30-008", 0, 0, 200), Stem("P30-001", 0.25f, 0, 200));
            T.Eq(2, report.PostGrowth.LivingCount, "grew both trees");
            T.Eq(2, report.Deaths.Count, "two hazard deaths");
            T.True(report.Deaths.All(death => death.Mechanism == "hazard"), "boundary does not remove hazard victims");
            T.Eq(new AnnualStand(0, 0, 0, 0, 0), report.After, "no survivors");
        });

        T.Add("annual mortality: boundary honours suppression priority and retains the last survivor", () =>
        {
            var priority = Run(0.003f, Stem("A", 0, 0, 70), Stem("B", 0.25f, 0, 30));
            T.True(priority.PostGrowth.RelativeDensity > 1, "high density");
            T.Eq(0, priority.Deaths.Count(d => d.Mechanism == "hazard"), "chosen rolls survive");
            T.Eq(new AnnualDeath("B", 1, "self-thinning", "boundary"), priority.Deaths.Single(), "most suppressed first");
            T.True(priority.After.RelativeDensity > 1, "singleton retains RD above 1");
            T.Eq(1, priority.After.LivingCount, "last survivor preserved after a removal");
            var singleton = Run(0.001f, Stem("solo", 0, 0, 200));
            T.True(singleton.PostGrowth.RelativeDensity > 1, "generic tiny-area singleton over boundary");
            T.Eq(0.0, singleton.Trees[0].DeathProbability!.Value, "zero vulnerability prevents hazard");
            T.Eq<float?>(null, singleton.Trees[0].MortalityRoll, "zero p skips roll even under positive pressure");
            T.Eq(singleton.PostGrowth, singleton.After, "already-singleton stand stays living");
        });
    }

    private static AnnualTree Stem(string id, float x, float z, float dbh = 20f) =>
        new(id, x, z, 30, dbh, 20.4f, 2f);

    private static AnnualReport Run(float areaHa, params AnnualTree[] trees) =>
        AnnualRules.Calculate(new AnnualState(0, trees), Seed, areaHa);

    private static void Near(double expected, double actual, double tolerance, string what) =>
        T.True(Math.Abs(actual - expected) <= tolerance, $"{what}: expected {expected:R} ± {tolerance:R}, got {actual:R}");
}
