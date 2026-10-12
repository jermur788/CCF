// Offline logic check for MixedDensityModel (growth model 2, D-059/D-061).
// Compiles the PRODUCTION SitkaGrowthModel.cs and MixedDensityModel.cs unchanged
// (neither uses UnityEngine) together with this file, on the plain .NET SDK. No
// Unity Editor, no Library, nothing written inside the repository.
//
// This is NOT an in-Editor gate. It proves the pure arithmetic and decision
// rules. It cannot prove controller wiring, scene state or any Unity anchor hash;
// MixedDensityVerification.cs (Editor) is the gate for that.
//
// Usage: run_offline_check.py [--repo ROOT]   Prints THREE_SPECIES_OFFLINE_* tokens.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public static class MixedDensityOfflineCheck
{
    private static int failures;
    private static int checks;

    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static string F(double v, string f = "0.0000") => v.ToString(f, Inv);

    public static int Main(string[] args)
    {
        string repo = args.Length > 0 ? args[0] : ".";
        Weights();
        GrowthModelVersions();
        AssetClasses(repo);
        SitkaOnlyDensityIsBitExact();
        SitkaOnlyPlanMatchesOriginalRoute();
        MixedDensityBehaviour();
        MixedPlanBehaviour();
        Determinism();
        Console.WriteLine(failures == 0 ? $"THREE_SPECIES_OFFLINE_PASS checks={checks}" : $"THREE_SPECIES_OFFLINE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    private const string Sitka = "sitka-spruce", Oak = "sessile-oak", Beech = "beech";

    private static GrowingSpaceDemandClass ClassOf(string speciesId)
    {
        switch (speciesId)
        {
            case Oak: return GrowingSpaceDemandClass.High;
            case Beech: return GrowingSpaceDemandClass.Intermediate;
            default: return GrowingSpaceDemandClass.Reference;
        }
    }

    private static DensityStem Stem(string id, string species, float dbh, double suppression)
        => new DensityStem(id, species, ClassOf(species), dbh, suppression);

    // Mirrors how the controller derives suppression from the Hegyi response.
    private static double SuppressionFromCi(float ci, float ci50) => 1.0 - 1.0 / (1.0 + ci / ci50);

    // ---------- contracts ----------

    private static void Weights()
    {
        double r = MixedDensityModel.Weight(GrowingSpaceDemandClass.Reference);
        double i = MixedDensityModel.Weight(GrowingSpaceDemandClass.Intermediate);
        double h = MixedDensityModel.Weight(GrowingSpaceDemandClass.High);
        Check(r == 1.0, "WEIGHT_REFERENCE_EXACTLY_ONE", "reference=" + F(r));
        Check(r < i && i < h, "WEIGHT_ORDINAL_ORDER", $"{F(r)} < {F(i)} < {F(h)}");
        bool threw = false;
        try { MixedDensityModel.Weight((GrowingSpaceDemandClass)99); } catch (ArgumentOutOfRangeException) { threw = true; }
        Check(threw, "WEIGHT_UNKNOWN_CLASS_FAILS_LOUD");
        Check(Enum.GetValues(typeof(GrowingSpaceDemandClass)).Length == 3, "WEIGHT_THREE_CLASSES_ONLY");
        // A species can not straddle two classes.
        bool twoClasses = false;
        var tally = new MixedDensityModel.Tally();
        tally.Add(new DensityStem("a", "x", GrowingSpaceDemandClass.Reference, 20f, 0.1));
        try { tally.Add(new DensityStem("b", "x", GrowingSpaceDemandClass.High, 20f, 0.1)); } catch (InvalidOperationException) { twoClasses = true; }
        Check(twoClasses, "TALLY_SPECIES_IN_ONE_CLASS_ONLY");
    }

    private static void GrowthModelVersions()
    {
        Check(GrowthModel.Legacy == 0 && GrowthModel.SiteClassDensity == 1 && GrowthModel.SpeciesAware == 2 && GrowthModel.Latest == 2,
            "GROWTH_MODEL_VERSION_VALUES", $"legacy={GrowthModel.Legacy} siteClass={GrowthModel.SiteClassDensity} speciesAware={GrowthModel.SpeciesAware} latest={GrowthModel.Latest}");
        Check(GrowthModel.Normalize(-3) == 0 && GrowthModel.Normalize(0) == 0 && GrowthModel.Normalize(1) == 1
            && GrowthModel.Normalize(2) == 2 && GrowthModel.Normalize(7) == 2, "GROWTH_MODEL_NORMALIZE");
    }

    // The serialized species assets: Sitka has no field (default = Reference, so the
    // calibrated species needs no edit), Oak and Beech are explicitly classified.
    private static void AssetClasses(string repo)
    {
        string dir = Path.Combine(repo, "Assets", "ForestPrototype", "Species");
        if (!Directory.Exists(dir))
        {
            Check(false, "ASSET_DIRECTORY_FOUND", dir);
            return;
        }
        int? Read(string file)
        {
            foreach (string line in File.ReadAllLines(Path.Combine(dir, file)))
                if (line.StartsWith("  growingSpaceDemand:", StringComparison.Ordinal))
                    return int.Parse(line.Substring("  growingSpaceDemand:".Length).Trim(), Inv);
            return null;
        }
        int? sitka = Read("SitkaSpruce.asset"), oak = Read("SessileOak.asset"), beech = Read("BeechSpecies.asset");
        Check(sitka == null, "ASSET_SITKA_UNCHANGED_DEFAULT_REFERENCE", "field=" + (sitka?.ToString() ?? "absent"));
        Check(oak == (int)GrowingSpaceDemandClass.High, "ASSET_OAK_HIGH", "field=" + (oak?.ToString() ?? "absent"));
        Check(beech == (int)GrowingSpaceDemandClass.Intermediate, "ASSET_BEECH_INTERMEDIATE", "field=" + (beech?.ToString() ?? "absent"));
    }

    // ---------- the original Sitka route, restated as the oracle ----------
    // Verbatim logic of ForestEcologyController.ApplyAdultDensityMortality /
    // StandRelativeDensity at the locked base c1564b8 (the Sitka-only route that
    // growth model 2 leaves in place), over the same inputs.

    private sealed class OriginalResult
    {
        public double StemsPerHectare, Dq, Rd, Pressure;
        public int Hazard, Boundary;
        public List<int> Victims = new List<int>();
    }

    private static double OriginalStandRd(IReadOnlyList<DensityStem> stems, IEnumerable<int> indices, float area, out double sph, out double dq)
    {
        sph = 0.0; dq = 0.0;
        int count = 0; double squaredDbhSum = 0.0;
        foreach (int i in indices) { squaredDbhSum += (double)stems[i].DbhCm * stems[i].DbhCm; count++; }
        if (count == 0 || area <= 0f) return 0.0;
        sph = count / area;
        dq = Math.Sqrt(squaredDbhSum / count);
        return SitkaGrowthModel.RelativeDensity(sph, dq);
    }

    private static OriginalResult OriginalRoute(IReadOnlyList<DensityStem> stems, float area, Func<string, double> roll)
    {
        var result = new OriginalResult();
        if (stems.Count == 0 || area <= 0f) return result;
        result.Rd = OriginalStandRd(stems, Enumerable.Range(0, stems.Count), area, out result.StemsPerHectare, out result.Dq);
        result.Pressure = SitkaGrowthModel.DensityPressure(result.Rd);
        if (result.Pressure <= 0.0) return result;
        double vulnerabilitySum = 0.0;
        for (int i = 0; i < stems.Count; i++) vulnerabilitySum += SitkaGrowthModel.Vulnerability(stems[i].Suppression);
        double meanVulnerability = vulnerabilitySum / stems.Count;
        var survivors = new List<int>(stems.Count);
        for (int i = 0; i < stems.Count; i++)
        {
            double probability = SitkaGrowthModel.AnnualDeathProbability(result.Pressure, stems[i].Suppression, meanVulnerability);
            if (probability > 0.0 && roll(stems[i].TreeId) < probability) result.Victims.Add(i);
            else survivors.Add(i);
        }
        result.Hazard = result.Victims.Count;
        if (OriginalStandRd(stems, survivors, area, out _, out _) > SitkaGrowthModel.MaximumRelativeDensity)
        {
            var order = new List<int>(survivors);
            order.Sort((a, b) =>
            {
                int bySuppression = stems[b].Suppression.CompareTo(stems[a].Suppression);
                return bySuppression != 0 ? bySuppression : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
            });
            int count = survivors.Count;
            double squaredDbhSum = 0.0;
            foreach (int i in survivors) squaredDbhSum += (double)stems[i].DbhCm * stems[i].DbhCm;
            foreach (int i in order)
            {
                double rd = SitkaGrowthModel.RelativeDensity(count / area, Math.Sqrt(squaredDbhSum / count));
                if (rd <= SitkaGrowthModel.MaximumRelativeDensity || count <= 1) break;
                count--;
                squaredDbhSum -= (double)stems[i].DbhCm * stems[i].DbhCm;
                result.Victims.Add(i);
                result.Boundary++;
            }
        }
        return result;
    }

    // ---------- fixtures ----------

    // Stems sorted by tree id, as FindTrees() presents them to the controller.
    private static List<DensityStem> RandomStand(Random random, int count, double dbhMin, double dbhMax, Func<int, string> speciesOf, string prefix = "P")
    {
        var stems = new List<DensityStem>(count);
        for (int i = 0; i < count; i++)
        {
            string species = speciesOf(i);
            float dbh = (float)(dbhMin + random.NextDouble() * (dbhMax - dbhMin));
            float ci = (float)(random.NextDouble() * 8.0);
            stems.Add(Stem(prefix + i.ToString("D5", Inv), species, dbh, SuppressionFromCi(ci, species == Sitka ? 5f : 3f)));
        }
        return stems.OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
    }

    private static Func<string, double> KeyedRoll(int seed)
    {
        // Same shape as SimulationRandom.Roll: a pure function of (id, seed), no stream.
        return id =>
        {
            uint hash = 2166136261u;
            foreach (char c in id) hash = unchecked((hash ^ c) * 16777619u);
            hash = unchecked((hash ^ (uint)seed) * 16777619u);
            hash ^= hash >> 15; hash = unchecked(hash * 2246822519u); hash ^= hash >> 13;
            return (hash & 0xFFFFFFu) / 16777216.0;
        };
    }

    // ---------- Sitka-only exactness ----------

    private static void SitkaOnlyDensityIsBitExact()
    {
        var random = new Random(20261012);
        int trials = 0, exact = 0, rdChecked = 0;
        foreach (float area in new[] { 0.16f, 0.64f, 1.0f, 0.37f })
            for (int n = 1; n <= 1500; n += (n < 40 ? 1 : 37))
            {
                List<DensityStem> stems = RandomStand(random, n, 3.0, 95.0, _ => Sitka);
                double oracle = OriginalStandRd(stems, Enumerable.Range(0, stems.Count), area, out _, out _);
                double got = MixedDensityModel.RelativeDensity(stems, area);
                trials++;
                if (BitConverter.DoubleToInt64Bits(oracle) == BitConverter.DoubleToInt64Bits(got)) exact++;
                rdChecked++;
            }
        Check(exact == trials, "SITKA_ONLY_RELATIVE_DENSITY_BIT_EXACT", $"trials={trials} bitIdentical={exact}");
    }

    private static void SitkaOnlyPlanMatchesOriginalRoute()
    {
        var random = new Random(8675309);
        int trials = 0, same = 0, withDeaths = 0, withBoundary = 0, hazardOnly = 0;
        // Dbh ranges straddling the onset (RD 0.6) and the maximum line (RD 1).
        foreach ((int n, double lo, double hi) in new[] { (300, 8.0, 22.0), (600, 15.0, 30.0), (900, 18.0, 36.0), (1200, 22.0, 42.0), (1400, 30.0, 55.0), (200, 40.0, 80.0), (50, 10.0, 20.0) })
            foreach (float area in new[] { 0.64f, 0.16f })
                for (int rep = 0; rep < 8; rep++)
                {
                    int m = Math.Max(1, n - rep * 7);
                    List<DensityStem> stems = RandomStand(random, m, lo, hi, _ => Sitka);
                    Func<string, double> roll = KeyedRoll(rep * 31 + 7);
                    OriginalResult old = OriginalRoute(stems, area, roll);
                    DensityMortalityPlan plan = MixedDensityModel.Plan(stems, area, roll);
                    bool equal = old.Victims.SequenceEqual(plan.VictimIndices) && old.Hazard == plan.HazardDeaths && old.Boundary == plan.BoundaryDeaths
                        && BitConverter.DoubleToInt64Bits(old.Rd) == BitConverter.DoubleToInt64Bits(plan.RelativeDensity)
                        && BitConverter.DoubleToInt64Bits(old.Rd) == BitConverter.DoubleToInt64Bits(plan.UnweightedRelativeDensity)
                        && BitConverter.DoubleToInt64Bits(old.Pressure) == BitConverter.DoubleToInt64Bits(plan.DensityPressure);
                    trials++;
                    if (equal) same++;
                    if (old.Victims.Count > 0) withDeaths++;
                    if (old.Boundary > 0) withBoundary++;
                    if (old.Hazard > 0 && old.Boundary == 0) hazardOnly++;
                }
        Check(same == trials, "SITKA_ONLY_PLAN_IDENTICAL_TO_ORIGINAL_ROUTE",
            $"trials={trials} identical={same} trialsWithDeaths={withDeaths} withBoundaryDeaths={withBoundary} hazardOnly={hazardOnly}");
        Check(withDeaths > 20 && withBoundary > 3 && hazardOnly > 3, "SITKA_ONLY_FIXTURES_EXERCISE_BOTH_STAGES",
            $"withDeaths={withDeaths} withBoundary={withBoundary} hazardOnly={hazardOnly}");
    }

    // ---------- mixed behaviour ----------

    // Stems of one species with equal size laid out so the Sitka relationship gives a requested relative density.
    private static List<DensityStem> UniformStand(string species, int count, float dbh, double suppression, string prefix)
    {
        var stems = new List<DensityStem>(count);
        for (int i = 0; i < count; i++)
            stems.Add(Stem(prefix + i.ToString("D5", Inv), species, dbh, suppression));
        return stems;
    }

    private static void MixedDensityBehaviour()
    {
        const float area = 0.64f;
        // Same stems, same size: the weighted density is weight x the Sitka density.
        List<DensityStem> sitka = UniformStand(Sitka, 400, 28f, 0.2, "S");
        List<DensityStem> beech = UniformStand(Beech, 400, 28f, 0.2, "S");
        List<DensityStem> oak = UniformStand(Oak, 400, 28f, 0.2, "S");
        double rdS = MixedDensityModel.RelativeDensity(sitka, area);
        double rdB = MixedDensityModel.RelativeDensity(beech, area);
        double rdO = MixedDensityModel.RelativeDensity(oak, area);
        Check(Math.Abs(rdB / rdS - 1.5) < 1e-12 && Math.Abs(rdO / rdS - 2.0) < 1e-12 && rdS < rdB && rdB < rdO,
            "WEIGHTED_DENSITY_SCALES_WITH_CLASS", $"sitka={F(rdS)} beech={F(rdB)} oak={F(rdO)}");

        // Sitka-equivalent density below onset, so Sitka stands never self-thin; the same stems as Oak/Beech do.
        double onset = SitkaGrowthModel.MortalityOnsetRelativeDensity;
        Check(rdS < onset && rdB > onset && rdO > onset, "BROADLEAF_REACHES_ONSET_WHERE_SITKA_DOES_NOT",
            $"onset={F(onset, "0.00")} sitka={F(rdS)} beech={F(rdB)} oak={F(rdO)}");
        var roll = KeyedRoll(1);
        DensityMortalityPlan sitkaPlan = MixedDensityModel.Plan(sitka, area, roll);
        DensityMortalityPlan beechPlan = MixedDensityModel.Plan(beech, area, roll);
        DensityMortalityPlan oakPlan = MixedDensityModel.Plan(oak, area, roll);
        Check(sitkaPlan.VictimIndices.Count == 0 && sitkaPlan.DensityPressure == 0.0, "DENSE_PURE_SITKA_BELOW_ONSET_NO_PRESSURE");
        Check(beechPlan.DensityPressure > 0.0 && oakPlan.DensityPressure > beechPlan.DensityPressure, "DENSE_PURE_BEECH_AND_OAK_HAVE_PRESSURE_OAK_HIGHER",
            $"beechPressure={F(beechPlan.DensityPressure)} oakPressure={F(oakPlan.DensityPressure)}");

        // Pure broadleaf stands are not immune: a crowded one self-thins and is held on its maximum line.
        foreach ((string species, string label) in new[] { (Beech, "BEECH"), (Oak, "OAK") })
        {
            var random = new Random(label.GetHashCode() & 0xFFFF);
            List<DensityStem> stems = RandomStand(random, 900, 20.0, 40.0, _ => species);
            DensityMortalityPlan plan = MixedDensityModel.Plan(stems, area, KeyedRoll(5));
            var survivors = stems.Where((s, i) => !plan.VictimIndices.Contains(i)).ToList();
            double rdAfter = MixedDensityModel.RelativeDensity(survivors, area);
            Check(plan.VictimIndices.Count > 0 && rdAfter <= SitkaGrowthModel.MaximumRelativeDensity + 1e-12,
                $"CROWDED_PURE_{label}_SELF_THINS_TO_MAXIMUM_LINE",
                $"rdBefore={F(plan.RelativeDensity)} rdAfter={F(rdAfter)} victims={plan.VictimIndices.Count} hazard={plan.HazardDeaths} boundary={plan.BoundaryDeaths}");
        }

        // Mixed: replacing Sitka stems by Oak stems of the same size raises the weighted density, step by step.
        var sizes = Enumerable.Range(0, 300).ToList();
        double previous = -1.0; bool rising = true;
        foreach (int oaks in new[] { 0, 30, 60, 120, 200, 300 })
        {
            var stems = sizes.Select(i => Stem("M" + i.ToString("D4", Inv), i < oaks ? Oak : Sitka, 26f, 0.3)).OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
            double rd = MixedDensityModel.RelativeDensity(stems, area);
            if (rd <= previous) rising = false;
            previous = rd;
        }
        Check(rising, "REPLACING_SITKA_BY_OAK_RAISES_WEIGHTED_DENSITY");

        // Per-species Dq: a few small Oak stems barely move a Sitka stand's density (no pooled-Dq distortion).
        List<DensityStem> pure = UniformStand(Sitka, 450, 30f, 0.2, "S");
        double rdPure = MixedDensityModel.RelativeDensity(pure, area);
        var withSaplings = pure.Concat(UniformStand(Oak, 6, 6f, 0.2, "T")).OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
        double rdWith = MixedDensityModel.RelativeDensity(withSaplings, area);
        Check(rdWith >= rdPure && (rdWith - rdPure) / rdPure < 0.01, "FEW_SMALL_OAK_BARELY_MOVE_SITKA_DENSITY",
            $"pure={F(rdPure)} withSixSmallOak={F(rdWith)} change={F((rdWith - rdPure) / rdPure * 100, "0.000")}%");
        // The defect this model removes: the old pooled route counted every Oak or Beech stem as one Sitka stem.
        // With a substantial broadleaf component the demand-weighted density is higher than the pooled one.
        var half = UniformStand(Sitka, 225, 28f, 0.2, "S").Concat(UniformStand(Oak, 225, 28f, 0.2, "T")).OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
        double pooledSph = half.Count / area;
        double pooledDq = Math.Sqrt(half.Sum(s => (double)s.DbhCm * s.DbhCm) / half.Count);
        double pooledRd = SitkaGrowthModel.RelativeDensity(pooledSph, pooledDq);
        double weightedRd = MixedDensityModel.RelativeDensity(half, area);
        Check(weightedRd > pooledRd * 1.4, "OLD_POOLED_ROUTE_UNDERSTATES_SUBSTANTIAL_BROADLEAF_COMPONENT",
            $"pooledSitkaRoute={F(pooledRd)} weighted={F(weightedRd)} (half Sitka, half Oak, same size)");

        // Three species at once, tally add/remove consistent with recomputation.
        var rnd = new Random(77);
        List<DensityStem> three = RandomStand(rnd, 600, 12.0, 45.0, i => i % 3 == 0 ? Sitka : i % 3 == 1 ? Beech : Oak);
        var tally = new MixedDensityModel.Tally();
        foreach (DensityStem s in three) tally.Add(s);
        var kept = new List<DensityStem>(three);
        bool consistent = Math.Abs(tally.RelativeDensity(area) - MixedDensityModel.RelativeDensity(kept, area)) < 1e-12;
        for (int i = 0; i < 200 && consistent; i++)
        {
            DensityStem gone = kept[rnd.Next(kept.Count)];
            kept.Remove(gone); tally.Remove(gone);
            consistent &= Math.Abs(tally.RelativeDensity(area) - MixedDensityModel.RelativeDensity(kept, area)) < 1e-9;
        }
        Check(consistent && tally.Count == kept.Count, "TALLY_REMOVE_MATCHES_RECOMPUTATION", $"count={tally.Count}");
    }

    private static void MixedPlanBehaviour()
    {
        const float area = 0.64f;
        var random = new Random(424242);
        int trials = 0, boundaryOk = 0, orderOk = 0, selectivity = 0, selectivityTrials = 0;
        var perSpeciesDeaths = new Dictionary<string, int> { [Sitka] = 0, [Beech] = 0, [Oak] = 0 };
        var perSpeciesLiving = new Dictionary<string, int> { [Sitka] = 0, [Beech] = 0, [Oak] = 0 };
        for (int rep = 0; rep < 40; rep++)
        {
            int n = 500 + rep * 20;
            List<DensityStem> stems = RandomStand(random, n, 14.0, 38.0, i => i % 4 == 0 ? Oak : i % 4 == 1 ? Beech : Sitka);
            DensityMortalityPlan plan = MixedDensityModel.Plan(stems, area, KeyedRoll(rep));
            trials++;
            var dead = new HashSet<int>(plan.VictimIndices);
            if (dead.Count != plan.VictimIndices.Count) { Check(false, "PLAN_NO_DUPLICATE_VICTIMS"); return; }
            var survivors = stems.Where((s, i) => !dead.Contains(i)).ToList();
            if (survivors.Count <= 1 || MixedDensityModel.RelativeDensity(survivors, area) <= SitkaGrowthModel.MaximumRelativeDensity + 1e-12) boundaryOk++;
            // Boundary victims are the most suppressed survivors first (ties by id).
            var boundary = plan.VictimIndices.Skip(plan.HazardDeaths).ToList();
            bool ordered = true;
            for (int k = 1; k < boundary.Count; k++)
            {
                DensityStem a = stems[boundary[k - 1]], b = stems[boundary[k]];
                if (a.Suppression < b.Suppression || (a.Suppression == b.Suppression && string.CompareOrdinal(a.TreeId, b.TreeId) > 0)) ordered = false;
            }
            if (ordered) orderOk++;
            if (plan.VictimIndices.Count >= 5)
            {
                selectivityTrials++;
                double deadMean = plan.VictimIndices.Average(i => stems[i].Suppression);
                double liveMean = survivors.Average(s => s.Suppression);
                if (deadMean > liveMean) selectivity++;
            }
            for (int i = 0; i < stems.Count; i++)
            {
                perSpeciesLiving[stems[i].SpeciesId]++;
                if (dead.Contains(i)) perSpeciesDeaths[stems[i].SpeciesId]++;
            }
        }
        Check(boundaryOk == trials, "MIXED_PLAN_ENDS_ON_OR_BELOW_MAXIMUM_LINE", $"trials={trials} ok={boundaryOk}");
        Check(orderOk == trials, "MIXED_PLAN_BOUNDARY_VICTIMS_MOST_SUPPRESSED_FIRST", $"trials={trials} ok={orderOk}");
        Check(selectivityTrials > 10 && selectivity == selectivityTrials, "MIXED_PLAN_VICTIMS_MORE_SUPPRESSED_THAN_SURVIVORS", $"trials={selectivityTrials} ok={selectivity}");
        Check(perSpeciesDeaths.Values.All(v => v > 0), "MIXED_PLAN_EXERCISES_ALL_THREE_SPECIES",
            string.Join(" ", perSpeciesDeaths.OrderBy(p => p.Key, StringComparer.Ordinal).Select(p => $"{p.Key}={p.Value}/{perSpeciesLiving[p.Key]}")));

        // Threshold: just below the weighted onset there is no pressure and no roll; just above there is pressure.
        double onset = SitkaGrowthModel.MortalityOnsetRelativeDensity;
        int below = 0, above = 0;
        for (int n = 50; n < 3000; n++)
        {
            List<DensityStem> stems = UniformStand(Oak, n, 25f, 0.3, "O");
            double rd = MixedDensityModel.RelativeDensity(stems, area);
            if (rd <= onset) below = n; else { above = n; break; }
        }
        int calls = 0;
        Func<string, double> counting = id => { calls++; return 0.999999; };
        DensityMortalityPlan belowPlan = MixedDensityModel.Plan(UniformStand(Oak, below, 25f, 0.3, "O"), area, counting);
        int callsBelow = calls;
        DensityMortalityPlan abovePlan = MixedDensityModel.Plan(UniformStand(Oak, above, 25f, 0.3, "O"), area, counting);
        Check(belowPlan.DensityPressure == 0.0 && callsBelow == 0 && abovePlan.DensityPressure > 0.0 && above == below + 1,
            "ONSET_THRESHOLD_CROSSING", $"stemsBelow={below} stemsAbove={above} pressureAbove={F(abovePlan.DensityPressure, "0.000000")} rollsBelow={callsBelow}");
        Check(calls - callsBelow <= above, "PLAN_AT_MOST_ONE_ROLL_PER_STEM", $"rollsAbove={calls - callsBelow} stems={above}");

        // Empty and degenerate inputs.
        Check(MixedDensityModel.Plan(new List<DensityStem>(), area, KeyedRoll(0)).VictimIndices.Count == 0, "EMPTY_STAND_NO_VICTIMS");
        Check(MixedDensityModel.Plan(UniformStand(Oak, 5, 25f, 0.3, "O"), 0f, KeyedRoll(0)).VictimIndices.Count == 0, "ZERO_AREA_NO_VICTIMS");
        DensityMortalityPlan single = MixedDensityModel.Plan(UniformStand(Oak, 1, 90f, 0.9, "O"), 0.01f, KeyedRoll(0));
        Check(single.VictimIndices.Count <= 1 && MixedDensityModel.AnyNonReference(UniformStand(Oak, 1, 20f, 0.1, "O")) && !MixedDensityModel.AnyNonReference(UniformStand(Sitka, 3, 20f, 0.1, "S")),
            "SINGLE_STEM_AND_ANY_NON_REFERENCE");
    }

    // ---------- determinism ----------

    private static void Determinism()
    {
        const float area = 0.64f;
        var random = new Random(1234);
        List<DensityStem> stems = RandomStand(random, 800, 15.0, 40.0, i => i % 3 == 0 ? Oak : i % 3 == 1 ? Beech : Sitka);
        string Fingerprint(DensityMortalityPlan p) => $"{BitConverter.DoubleToInt64Bits(p.RelativeDensity):X16}|{p.HazardDeaths}|{p.BoundaryDeaths}|{string.Join(",", p.VictimIndices)}";
        string a = Fingerprint(MixedDensityModel.Plan(stems, area, KeyedRoll(9)));
        string b = Fingerprint(MixedDensityModel.Plan(stems, area, KeyedRoll(9)));
        Check(a == b, "REPLAY_IDENTICAL", a.Substring(0, 24));

        // Species may be interleaved in any way: only the order within each species matters, and the
        // controller always presents trees sorted by id. Re-listing the same sorted stems from a different
        // enumeration start must therefore not change the species-level density bit for bit.
        var bySpecies = stems.GroupBy(s => s.SpeciesId).OrderByDescending(g => g.Key, StringComparer.Ordinal).SelectMany(g => g).ToList();
        double d1 = MixedDensityModel.RelativeDensity(stems, area), d2 = MixedDensityModel.RelativeDensity(bySpecies, area);
        Check(BitConverter.DoubleToInt64Bits(d1) == BitConverter.DoubleToInt64Bits(d2), "SPECIES_INTERLEAVING_DOES_NOT_CHANGE_DENSITY", $"{F(d1, "0.000000000")} vs {F(d2, "0.000000000")}");

        // Year after year with fixed stems and a changing keyed roll: the keyed draw, not call order, decides victims.
        var distinct = new HashSet<string>();
        for (int year = 1; year <= 12; year++)
            distinct.Add(Fingerprint(MixedDensityModel.Plan(stems, area, KeyedRoll(year))));
        Check(distinct.Count > 6, "DIFFERENT_YEARS_GIVE_DIFFERENT_DRAWS", $"distinctOutcomes={distinct.Count}/12");
    }
}
