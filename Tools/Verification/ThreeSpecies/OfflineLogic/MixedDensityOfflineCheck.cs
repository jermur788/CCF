// Offline logic check for the growth model 2 density routes (D-059, D-061).
// Compiles the PRODUCTION SitkaGrowthModel.cs and MixedDensityModel.cs unchanged
// (neither uses UnityEngine) together with this file, on the plain .NET SDK. No
// Unity Editor, no Library, nothing written inside the repository.
//
// What it proves:
//   - ROUTING: pure Sitka (and every stand under growth models 0 and 1) takes the original Sitka route;
//     any Oak/Beech under model 2 takes the occupancy route. Pure functions, exhaustive and random.
//   - LEGACY ROUTE UNCHANGED: SHA-256 fingerprints of the original Sitka code, taken from the accepted
//     base commit c1564b8, match the working tree. This is the offline stand-in for "bit-identical":
//     the Sitka arithmetic and the controller statements around it are the same bytes as the base.
//   - NO SITKA ARITHMETIC IN THE OCCUPANCY ROUTE: source guards here, and the separate density-isolated
//     suite compiles MixedDensityModel.cs WITHOUT SitkaGrowthModel.cs, which is a compile-time proof.
//   - BEHAVIOUR of the occupancy route on pure Oak, pure Beech and mixed fixtures.
//
// What it cannot prove: controller wiring at runtime, scene state or any Unity anchor hash. The Editor
// gate MixedDensityVerification.cs does that and has NOT been run in this environment.
//
// Usage: run_offline_check.py [--suite density]   (passes the repository root as the first argument)
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class MixedDensityOfflineCheck
{
    private static int failures;
    private static int checks;

    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static string F(double v, string f = "0.0000") => v.ToString(f, Inv);

    private const GrowingSpaceDemandClass Ref = GrowingSpaceDemandClass.Reference;
    private const GrowingSpaceDemandClass Mid = GrowingSpaceDemandClass.Intermediate;
    private const GrowingSpaceDemandClass Hi = GrowingSpaceDemandClass.High;
    private const float Area = 0.64f;   // Enlarged80 property, ha

    // Fingerprints of the ORIGINAL Sitka code at the accepted base commit c1564b8 (growth model 1).
    private const string SitkaModelClassSha256 = "1a81640dcd11cac892a51bde6e28115b84465e08fd20c5d4b31859337964d032";
    private const string LegacyDecisionBlockSha256 = "ee7dc266441de752d3c9e322faee493b2484d3e7d0b23206c167b238dae45fa8";
    private const string StandRelativeDensitySha256 = "80ef453c02a4298dbde5d41a7508016e0008bd0a55d07855c20b5276122e6448";

    public static int Main(string[] args)
    {
        string repo = args.Length > 0 ? args[0] : ".";
        Weights();
        GrowthModelVersions();
        AssetClasses(repo);
        Routing();
        LegacyRouteIsUnchanged(repo);
        OccupancyAbstraction();
        DemandOrder();
        PureBroadleafDense();
        MixedStands();
        Thresholds();
        VictimSelection();
        Continuity();
        Determinism();
        Degenerate();
        Console.WriteLine(failures == 0 ? $"THREE_SPECIES_OFFLINE_PASS checks={checks}" : $"THREE_SPECIES_OFFLINE_FAIL failures={failures} checks={checks}");
        return failures == 0 ? 0 : 1;
    }

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name} {detail}");
        if (!ok) failures++;
    }

    // ---------- fixtures ----------

    private static DensityStem Stem(string id, GrowingSpaceDemandClass demand, double dbh, double ci)
        => new DensityStem(id, demand, (float)dbh, ci);

    // Equal-sized, equally competed stems of one class.
    private static List<DensityStem> Uniform(GrowingSpaceDemandClass demand, int count, float dbh, double ci, string prefix)
    {
        var stems = new List<DensityStem>(count);
        for (int i = 0; i < count; i++)
            stems.Add(Stem(prefix + i.ToString("D5", Inv), demand, dbh, ci));
        return stems;
    }

    // Random sizes and competition, sorted by tree id as FindTrees() presents them to the controller.
    private static List<DensityStem> RandomStand(Random random, int count, double dbhMin, double dbhMax, Func<int, GrowingSpaceDemandClass> classOf, string prefix = "P")
    {
        var stems = new List<DensityStem>(count);
        for (int i = 0; i < count; i++)
            stems.Add(Stem(prefix + i.ToString("D5", Inv), classOf(i), dbhMin + random.NextDouble() * (dbhMax - dbhMin), random.NextDouble() * 8.0));
        return stems.OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
    }

    private static GrowingSpaceDemandClass ThreeWay(int i) => i % 3 == 0 ? Ref : i % 3 == 1 ? Mid : Hi;

    // A per-tree uniform draw that is a pure function of (id, seed): no stream, no call order.
    private static Func<string, double> KeyedRoll(int seed)
    {
        return id =>
        {
            unchecked
            {
                ulong h = 14695981039346656037UL;
                foreach (char c in id) { h ^= c; h *= 1099511628211UL; }
                h ^= (ulong)(uint)seed * 0x9E3779B97F4A7C15UL;
                h ^= h >> 30; h *= 0xBF58476D1CE4E5B9UL; h ^= h >> 27; h *= 0x94D049BB133111EBUL; h ^= h >> 31;
                return (h >> 11) / 9007199254740992.0;
            }
        };
    }

    private static double Occ(IReadOnlyList<DensityStem> stems) => MixedDensityModel.Occupancy(stems, Area);

    private static string Fingerprint(DensityMortalityPlan p)
        => $"{BitConverter.DoubleToInt64Bits(p.Occupancy):X16}|{p.HazardDeaths}|{p.BoundaryDeaths}|{string.Join(",", p.VictimIndices)}";

    // ---------- contracts ----------

    private static void Weights()
    {
        double r = MixedDensityModel.Weight(Ref), i = MixedDensityModel.Weight(Mid), h = MixedDensityModel.Weight(Hi);
        Check(r == 1.0 && i == 1.5 && h == 2.0, "WEIGHTS_ARE_THE_PROVISIONAL_CLASSES", $"{F(r)} {F(i)} {F(h)}");
        Check(r < i && i < h, "WEIGHT_ORDINAL_ORDER");
        bool threw = false;
        try { MixedDensityModel.Weight((GrowingSpaceDemandClass)99); } catch (ArgumentOutOfRangeException) { threw = true; }
        Check(threw, "WEIGHT_UNKNOWN_CLASS_FAILS_LOUD");
        Check(Enum.GetValues(typeof(GrowingSpaceDemandClass)).Length == 3, "WEIGHT_THREE_CLASSES_ONLY");
        Check((int)Ref == 0 && (int)Mid == 1 && (int)Hi == 2, "CLASS_VALUES_MATCH_SERIALIZED_ASSETS");
    }

    private static void GrowthModelVersions()
    {
        Check(GrowthModel.Legacy == 0 && GrowthModel.SiteClassDensity == 1 && GrowthModel.SpeciesAware == 2 && GrowthModel.Latest == 2,
            "GROWTH_MODEL_VERSION_VALUES", $"legacy={GrowthModel.Legacy} siteClass={GrowthModel.SiteClassDensity} speciesAware={GrowthModel.SpeciesAware} latest={GrowthModel.Latest}");
        Check(GrowthModel.Normalize(-3) == 0 && GrowthModel.Normalize(0) == 0 && GrowthModel.Normalize(1) == 1
            && GrowthModel.Normalize(2) == 2 && GrowthModel.Normalize(7) == 2, "GROWTH_MODEL_NORMALIZE");
    }

    // The serialized species assets: Sitka has no field (default = Reference), Oak and Beech are explicitly
    // classified, and no species other than Sitka is Reference (a Reference species would get the Sitka route).
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
        Check(oak == (int)Hi, "ASSET_OAK_HIGH", "field=" + (oak?.ToString() ?? "absent"));
        Check(beech == (int)Mid, "ASSET_BEECH_INTERMEDIATE", "field=" + (beech?.ToString() ?? "absent"));
        string[] referenceSpecies = Directory.GetFiles(dir, "*.asset")
            .Where(f => (Read(Path.GetFileName(f)) ?? 0) == (int)Ref).Select(Path.GetFileName).OrderBy(n => n, StringComparer.Ordinal).ToArray();
        Check(referenceSpecies.Length == 1 && referenceSpecies[0] == "SitkaSpruce.asset", "ONLY_SITKA_IS_REFERENCE_CLASS", string.Join(",", referenceSpecies));
    }

    // ---------- TEST A (offline part): routing ----------

    private static void Routing()
    {
        // Every combination of 1-6 Reference-only stems under model 2 takes the legacy route.
        var all = new List<DensityStem>();
        bool pureSitkaLegacy = true;
        for (int n = 1; n <= 6; n++)
        {
            all.Add(Stem("S" + n, Ref, 20 + n, 1.0));
            pureSitkaLegacy &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(all)) == DensityRoute.LegacySitkaExact;
        }
        var thousand = Uniform(Ref, 1000, 30f, 2.0, "S");
        pureSitkaLegacy &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(thousand)) == DensityRoute.LegacySitkaExact;
        Check(pureSitkaLegacy, "ROUTE_PURE_SITKA_IS_LEGACY_SITKA_EXACT");

        // Any stand containing a non-Reference stem under model 2 takes the occupancy route, wherever the stem sits.
        var random = new Random(61);
        int trials = 0, aware = 0;
        for (int t = 0; t < 400; t++)
        {
            int n = 1 + random.Next(60);
            var stems = Uniform(Ref, n, 25f, 1.0, "S").ToList();
            int at = random.Next(n);
            stems[at] = Stem(stems[at].TreeId, random.Next(2) == 0 ? Mid : Hi, 25, 1.0);
            trials++;
            if (MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(stems)) == DensityRoute.SpeciesAwareOccupancy) aware++;
        }
        Check(aware == trials, "ROUTE_ANY_OAK_OR_BEECH_IS_SPECIES_AWARE_OCCUPANCY", $"trials={trials} speciesAware={aware}");

        // Models 0 and 1 never leave the Sitka route, whatever the stand holds.
        Check(MixedDensityModel.SelectRoute(false, true) == DensityRoute.LegacySitkaExact
            && MixedDensityModel.SelectRoute(false, false) == DensityRoute.LegacySitkaExact, "ROUTE_MODELS_BELOW_2_ALWAYS_LEGACY");

        // One Oak flips a thousand-Sitka stand, and removing it flips it back.
        var mixed = Uniform(Ref, 1000, 30f, 2.0, "S").Concat(new[] { Stem("T", Hi, 30, 2.0) }).ToList();
        bool flips = MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(mixed)) == DensityRoute.SpeciesAwareOccupancy
            && MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(mixed.Take(1000).ToList())) == DensityRoute.LegacySitkaExact;
        Check(flips, "ROUTE_ONE_OAK_FLIPS_AND_REMOVAL_FLIPS_BACK");
    }

    // ---------- TEST A (offline part): the legacy route is the same bytes as the accepted base ----------

    private static string ReadSource(string repo, params string[] parts)
        => File.ReadAllText(Path.Combine(new[] { repo }.Concat(parts).ToArray())).Replace("\r\n", "\n");

    private static string Sha256(string text)
    {
        using (SHA256 sha = SHA256.Create())
            return string.Concat(sha.ComputeHash(Encoding.UTF8.GetBytes(text)).Select(b => b.ToString("x2", Inv)));
    }

    private static string Between(string text, string start, string end)
    {
        int i = text.IndexOf(start, StringComparison.Ordinal);
        if (i < 0) return null;
        int j = text.IndexOf(end, i, StringComparison.Ordinal);
        return j < 0 ? null : text.Substring(i, j - i);
    }

    private static string StripComments(string text)
    {
        var lines = text.Split('\n').Select(line => { int c = line.IndexOf("//", StringComparison.Ordinal); return c >= 0 ? line.Substring(0, c) : line; });
        return string.Join("\n", lines);
    }

    private static void LegacyRouteIsUnchanged(string repo)
    {
        string sitka, controller, model;
        try
        {
            sitka = ReadSource(repo, "Assets", "ForestPrototype", "SitkaGrowthModel.cs");
            controller = ReadSource(repo, "Assets", "ForestPrototype", "ForestEcologyController.cs");
            model = ReadSource(repo, "Assets", "ForestPrototype", "MixedDensityModel.cs");
        }
        catch (IOException ex)
        {
            Check(false, "SOURCES_FOUND", ex.Message);
            return;
        }

        int classAt = sitka.IndexOf("public static class SitkaGrowthModel", StringComparison.Ordinal);
        string sitkaClass = classAt >= 0 ? sitka.Substring(classAt) : "";
        Check(Sha256(sitkaClass) == SitkaModelClassSha256, "LEGACY_SITKA_ARITHMETIC_UNCHANGED_FROM_BASE_C1564B8", Sha256(sitkaClass).Substring(0, 16));

        string block = Between(controller, "account.RelativeDensity = StandRelativeDensity(living", "        KillDensityVictims(victims, account);");
        Check(block != null && Sha256(block) == LegacyDecisionBlockSha256, "LEGACY_CONTROLLER_DECISION_BLOCK_UNCHANGED_FROM_BASE_C1564B8",
            block == null ? "block not found" : Sha256(block).Substring(0, 16));
        string helper = Between(controller, "private double StandRelativeDensity(", "    // Dynamic crown radius");
        Check(helper != null && Sha256(helper) == StandRelativeDensitySha256, "LEGACY_STAND_RELATIVE_DENSITY_UNCHANGED_FROM_BASE_C1564B8",
            helper == null ? "helper not found" : Sha256(helper).Substring(0, 16));

        // Wiring, in source order: route chosen by the pure function, occupancy route returns before the legacy
        // statements, legacy route labelled, then the unchanged legacy block.
        int select = controller.IndexOf("MixedDensityModel.SelectRoute(UsesSpeciesAwareGrowth, anyNonReference)", StringComparison.Ordinal);
        int aware = controller.IndexOf("ApplySpeciesAwareDensityMortality(living, account);", StringComparison.Ordinal);
        int label = controller.IndexOf("account.Route = DensityRoute.LegacySitkaExact;", StringComparison.Ordinal);
        int legacy = controller.IndexOf("account.RelativeDensity = StandRelativeDensity(living", StringComparison.Ordinal);
        Check(select > 0 && select < aware && aware < label && label < legacy, "LEGACY_ROUTE_WIRING_ORDER", $"select={select} aware={aware} label={label} legacy={legacy}");
        Check(controller.Contains("private bool UsesSpeciesAwareGrowth => growthModelVersion >= GrowthModel.SpeciesAware;")
            && controller.Contains("bool anyNonReference = UsesSpeciesAwareGrowth && HasNonReferenceDemand(living);"),
            "SPECIES_AWARE_ROUTE_IS_GATED_ON_GROWTH_MODEL_2");

        // The occupancy route's controller method and model must not touch any Sitka density arithmetic.
        string method = Between(controller, "private void ApplySpeciesAwareDensityMortality(", "    private double StandRelativeDensity(");
        string methodCode = method == null ? null : StripComments(method);
        Check(methodCode != null && !methodCode.Contains("SitkaGrowthModel") && !methodCode.Contains("StandRelativeDensity") && !methodCode.Contains("RelativeDensity")
            && methodCode.Contains("MixedDensityModel.Plan("), "OCCUPANCY_ROUTE_CONTROLLER_METHOD_HAS_NO_SITKA_ARITHMETIC");
        string modelCode = StripComments(model);
        Check(!modelCode.Contains("SitkaGrowthModel") && !modelCode.Contains("RelativeDensity") && !modelCode.Contains("MaximumSdi") && !modelCode.Contains("2.063"),
            "MIXED_DENSITY_MODEL_SOURCE_HAS_NO_SITKA_REFERENCE");
    }

    // ---------- the occupancy abstraction ----------

    private static void OccupancyAbstraction()
    {
        // Plain geometry: basal area, exponent exactly 2 (not a fitted 2.063).
        bool geometry = true;
        foreach (double dbh in new[] { 4.0, 10.0, 25.0, 47.5, 90.0 })
        {
            double expected = Math.PI / 4.0 * (dbh / 100.0) * (dbh / 100.0);
            double got = MixedDensityModel.StemOccupancyM2(Stem("a", Ref, dbh, 0));
            double doubled = MixedDensityModel.StemOccupancyM2(Stem("a", Ref, dbh * 2.0, 0));
            geometry &= Math.Abs(got - expected) < 1e-15 && Math.Abs(doubled / got - 4.0) < 1e-6;
        }
        Check(geometry, "OCCUPANCY_IS_BASAL_AREA_GEOMETRY_EXPONENT_TWO");

        // No mixture bonus: occupancy is a plain sum over stems, so any split of a stand adds back up.
        var random = new Random(2026);
        double worst = 0.0;
        for (int t = 0; t < 200; t++)
        {
            List<DensityStem> stems = RandomStand(random, 100 + random.Next(500), 5.0, 70.0, ThreeWay);
            double whole = Occ(stems);
            double parts = Occ(stems.Where(s => s.Demand == Ref).ToList()) + Occ(stems.Where(s => s.Demand == Mid).ToList()) + Occ(stems.Where(s => s.Demand == Hi).ToList());
            worst = Math.Max(worst, Math.Abs(whole - parts) / Math.Max(whole, 1e-12));
        }
        Check(worst < 1e-12, "OCCUPANCY_IS_ADDITIVE_NO_MIXTURE_BONUS_NO_DEC_NO_DMC", $"worstRelativeDifference={worst:E2}");

        var one = Uniform(Hi, 300, 31f, 1.0, "O");
        double a = MixedDensityModel.Occupancy(one, 0.64f), b = MixedDensityModel.Occupancy(one, 1.28f);
        Check(Math.Abs(a / b - 2.0) < 1e-12, "OCCUPANCY_SCALES_INVERSELY_WITH_AREA", $"{F(a)} vs {F(b)}");
        var shuffled = one.OrderByDescending(s => s.TreeId, StringComparer.Ordinal).ToList();
        Check(BitConverter.DoubleToInt64Bits(MixedDensityModel.Occupancy(one, Area)) == BitConverter.DoubleToInt64Bits(MixedDensityModel.Occupancy(shuffled, Area)),
            "OCCUPANCY_INDEPENDENT_OF_LISTING_ORDER_BIT_FOR_BIT");

        // The abstraction is not the old identity "weighted density = weight x Sitka relative density".
        bool notSitkaMultiple = true;
        foreach (float dbh in new[] { 28f, 60f })
        {
            double sitkaRd = SitkaGrowthModel.RelativeDensity(400 / Area, dbh);
            double oakOccupancy = Occ(Uniform(Hi, 400, dbh, 1.0, "O"));
            notSitkaMultiple &= Math.Abs(oakOccupancy / sitkaRd - 2.0) > 1e-3;
        }
        Check(notSitkaMultiple, "OAK_OCCUPANCY_IS_NOT_WEIGHT_TIMES_SITKA_RELATIVE_DENSITY");
    }

    // ---------- TEST E: demand order ----------

    private static void DemandOrder()
    {
        bool ordered = true, ratios = true;
        foreach (double dbh in new[] { 3.0, 8.0, 15.0, 28.0, 45.0, 70.0, 110.0 })
        {
            double r = MixedDensityModel.StemOccupancyM2(Stem("a", Ref, dbh, 1.0));
            double m = MixedDensityModel.StemOccupancyM2(Stem("a", Mid, dbh, 1.0));
            double h = MixedDensityModel.StemOccupancyM2(Stem("a", Hi, dbh, 1.0));
            ordered &= r < m && m < h;
            ratios &= Math.Abs(m / r - 1.5) < 1e-12 && Math.Abs(h / r - 2.0) < 1e-12;
        }
        Check(ordered, "DEMAND_ORDER_HIGHER_CLASS_GREATER_CONTRIBUTION_AT_EQUAL_SIZE");
        Check(ratios, "DEMAND_ORDER_CONTRIBUTIONS_FOLLOW_THE_PROVISIONAL_WEIGHTS");

        // Same stems, same size and competition, three classes: occupancy and pressure follow the order.
        double oS = Occ(Uniform(Ref, 400, 28f, 1.0, "S")), oB = Occ(Uniform(Mid, 400, 28f, 1.0, "B")), oO = Occ(Uniform(Hi, 400, 28f, 1.0, "O"));
        double pS = MixedDensityModel.OvercrowdingPressure(oS), pB = MixedDensityModel.OvercrowdingPressure(oB), pO = MixedDensityModel.OvercrowdingPressure(oO);
        Check(oS < oB && oB < oO && pS == 0.0 && pB > 0.0 && pO > pB, "DEMAND_ORDER_RAISES_OCCUPANCY_AND_PRESSURE_FOR_SAME_STEMS",
            $"occupancy sitka={F(oS)} beech={F(oB)} oak={F(oO)} pressure sitka={F(pS)} beech={F(pB)} oak={F(pO)} onset={F(MixedDensityModel.OvercrowdingOnset, "0.00")}");
    }

    // ---------- TESTS B and C: pure Oak and pure Beech ----------

    private static void PureBroadleafDense()
    {
        foreach ((GrowingSpaceDemandClass cls, string label, int stems, int zoneStems) in new[] { (Hi, "OAK", 500, 400), (Mid, "BEECH", 600, 500) })
        {
            // Intentionally overcrowded: occupancy beyond capacity. The hazard thins it.
            List<DensityStem> dense = RandomStand(new Random(label.Length * 17), stems, 24.0, 36.0, _ => cls);
            DensityMortalityPlan plan = MixedDensityModel.Plan(dense, Area, KeyedRoll(5));
            var survivors = dense.Where((s, i) => !plan.VictimIndices.Contains(i)).ToList();
            double after = Occ(survivors);
            Check(plan.Occupancy > 1.0 && plan.OvercrowdingPressure > 1.0, $"PURE_{label}_DENSE_OCCUPANCY_EXCEEDS_CAPACITY",
                $"occupancy={F(plan.Occupancy)} pressure={F(plan.OvercrowdingPressure)} stems={stems}");
            Check(plan.VictimIndices.Count > 0 && plan.HazardDeaths > 0, $"PURE_{label}_CAN_DIE_FROM_DENSITY_PRESSURE",
                $"victims={plan.VictimIndices.Count} hazard={plan.HazardDeaths} boundary={plan.BoundaryDeaths}");
            Check(after <= 1.0 + 1e-12 && survivors.Count > 1 && Math.Abs(after - plan.OccupancyAfter) < 1e-9, $"PURE_{label}_ENDS_AT_OR_BELOW_CAPACITY",
                $"occupancyAfter={F(after)} reported={F(plan.OccupancyAfter)} survivors={survivors.Count}/{stems}");

            // The boundary also holds a pure broadleaf stand on capacity when the hazard draw spares every stem, and
            // it stops within one stem of the capacity rather than thinning far below it.
            DensityMortalityPlan spared = MixedDensityModel.Plan(dense, Area, id => 0.9999999);
            var keptAfterSpared = dense.Where((s, i) => !spared.VictimIndices.Contains(i)).ToList();
            double heaviest = dense.Max(s => MixedDensityModel.StemOccupancyM2(s)) / Area / MixedDensityModel.OccupancyCapacityM2PerHa;
            double afterSpared = Occ(keptAfterSpared);
            Check(spared.HazardDeaths == 0 && spared.BoundaryDeaths > 0 && afterSpared <= 1.0 + 1e-12 && afterSpared > 1.0 - heaviest, $"PURE_{label}_BOUNDARY_HOLDS_CAPACITY_WHEN_HAZARD_SPARES",
                $"boundaryDeaths={spared.BoundaryDeaths} occupancyAfter={F(afterSpared)} oneStemMax={F(heaviest, "0.0000")}");

            // Hazard zone only: inside the pressure range but under capacity, a pure stand still loses stems.
            List<DensityStem> zone = Uniform(cls, zoneStems, 28f, 3.0, "Z").Select((s, i) => Stem(s.TreeId, cls, 24.0 + (i % 9), 1.0 + (i % 7))).ToList();
            double zoneOccupancy = Occ(zone);
            int zoneDeaths = 0, zoneBoundary = 0;
            for (int seed = 1; seed <= 40; seed++)
            {
                DensityMortalityPlan z = MixedDensityModel.Plan(zone, Area, KeyedRoll(seed));
                zoneDeaths += z.VictimIndices.Count;
                zoneBoundary += z.BoundaryDeaths;
            }
            Check(zoneOccupancy > MixedDensityModel.OvercrowdingOnset && zoneOccupancy < 1.0 && zoneDeaths > 0 && zoneBoundary == 0, $"PURE_{label}_HAZARD_ZONE_LOSES_STEMS_WITHOUT_BOUNDARY",
                $"occupancy={F(zoneOccupancy)} deathsOver40Years={zoneDeaths} boundaryDeaths={zoneBoundary}");
        }

        // Reference only, same stems as the dense Oak fixture: below the threshold, so no density mortality.
        List<DensityStem> sitkaSame = RandomStand(new Random(3 * 17), 500, 24.0, 36.0, _ => Ref);
        DensityMortalityPlan sitkaPlan = MixedDensityModel.Plan(sitkaSame, Area, KeyedRoll(5));
        Check(sitkaPlan.Occupancy < 1.0 && sitkaPlan.OccupancyAfter == sitkaPlan.Occupancy, "SAME_STEMS_AS_REFERENCE_CLASS_ARE_LESS_CROWDED_THAN_OAK",
            $"referenceOccupancy={F(sitkaPlan.Occupancy)} victims={sitkaPlan.VictimIndices.Count}");
    }

    // ---------- TEST D: mixed Sitka / Oak / Beech ----------

    private static void MixedStands()
    {
        // Replace stems by higher-demand classes at the same size: occupancy rises step by step. Reverse: it falls.
        const int n = 300;
        double previous = -1.0; bool rising = true;
        foreach (int oaks in new[] { 0, 30, 60, 120, 200, 300 })
        {
            var stems = Enumerable.Range(0, n).Select(i => Stem("M" + i.ToString("D4", Inv), i < oaks ? Hi : Ref, 26, 1.0)).OrderBy(s => s.TreeId, StringComparer.Ordinal).ToList();
            double o = Occ(stems);
            if (o <= previous) rising = false;
            previous = o;
        }
        Check(rising, "MIXED_REPLACING_SITKA_BY_OAK_RAISES_OCCUPANCY_STEP_BY_STEP");

        var baseStand = RandomStand(new Random(8), 450, 12.0, 40.0, ThreeWay);
        int oakIndex = baseStand.FindIndex(s => s.Demand == Hi);
        DensityStem oak = baseStand[oakIndex];
        double with = Occ(baseStand);
        double without = Occ(baseStand.Where((s, i) => i != oakIndex).ToList());
        double asBeech = Occ(baseStand.Select((s, i) => i == oakIndex ? Stem(s.TreeId, Mid, s.DbhCm, s.CompetitionIndex) : s).ToList());
        double asSitka = Occ(baseStand.Select((s, i) => i == oakIndex ? Stem(s.TreeId, Ref, s.DbhCm, s.CompetitionIndex) : s).ToList());
        double expectedDrop = MixedDensityModel.StemOccupancyM2(oak) / Area / MixedDensityModel.OccupancyCapacityM2PerHa;
        Check(without < asSitka && asSitka < asBeech && asBeech < with, "MIXED_REMOVING_OR_DOWNGRADING_A_HIGH_DEMAND_STEM_LOWERS_OCCUPANCY_IN_ORDER",
            $"removed={F(without, "0.00000")} asSitka={F(asSitka, "0.00000")} asBeech={F(asBeech, "0.00000")} oak={F(with, "0.00000")}");
        Check(Math.Abs((with - without) - expectedDrop) < 1e-12, "MIXED_REMOVING_A_STEM_LOWERS_OCCUPANCY_BY_EXACTLY_ITS_CONTRIBUTION", $"drop={expectedDrop:E3}");

        // Deterministic replay of a dying three-species stand, including its victim list.
        List<DensityStem> dying = RandomStand(new Random(1234), 800, 18.0, 40.0, ThreeWay);
        string a = Fingerprint(MixedDensityModel.Plan(dying, Area, KeyedRoll(9)));
        string b = Fingerprint(MixedDensityModel.Plan(dying, Area, KeyedRoll(9)));
        DensityMortalityPlan plan = MixedDensityModel.Plan(dying, Area, KeyedRoll(9));
        var perClass = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        foreach (int i in plan.VictimIndices) perClass[dying[i].Demand]++;
        Check(a == b && plan.VictimIndices.Count > 0, "MIXED_THREE_SPECIES_REPLAY_IDENTICAL", a.Substring(0, 24));
        Check(perClass.Values.All(v => v > 0), "MIXED_STAND_LOSES_STEMS_OF_ALL_THREE_CLASSES (no class is immune)",
            $"reference={perClass[Ref]} intermediate={perClass[Mid]} high={perClass[Hi]} occupancy={F(plan.Occupancy)}");
    }

    // ---------- TESTS F and G: below and above the generic limits ----------

    private static void Thresholds()
    {
        // F: mixed stand under the onset. A roll that would kill any stem with a non-zero probability kills none.
        int calls = 0;
        Func<string, double> alwaysKills = id => { calls++; return 0.0; };
        List<DensityStem> below = RandomStand(new Random(41), 350, 18.0, 30.0, ThreeWay);
        DensityMortalityPlan low = MixedDensityModel.Plan(below, Area, alwaysKills);
        Check(low.Occupancy > 0.2 && low.Occupancy <= MixedDensityModel.OvercrowdingOnset && low.OvercrowdingPressure == 0.0 && low.VictimIndices.Count == 0 && calls == 0,
            "BELOW_THRESHOLD_MIXED_STAND_NO_DENSITY_MORTALITY", $"occupancy={F(low.Occupancy)} onset={F(MixedDensityModel.OvercrowdingOnset, "0.00")} victims={low.VictimIndices.Count} rolls={calls}");

        // Exact crossing: one stem fewer is under the onset (no pressure, no roll), one more is over it.
        int under = 0, over = 0;
        for (int stems = 20; stems < 4000; stems++)
        {
            double o = Occ(Uniform(Hi, stems, 25f, 1.0, "O"));
            if (o <= MixedDensityModel.OvercrowdingOnset) under = stems; else { over = stems; break; }
        }
        calls = 0;
        DensityMortalityPlan atUnder = MixedDensityModel.Plan(Uniform(Hi, under, 25f, 1.0, "O"), Area, alwaysKills);
        int callsUnder = calls;
        DensityMortalityPlan atOver = MixedDensityModel.Plan(Uniform(Hi, over, 25f, 1.0, "O"), Area, alwaysKills);
        Check(over == under + 1 && atUnder.OvercrowdingPressure == 0.0 && callsUnder == 0 && atOver.OvercrowdingPressure > 0.0 && atOver.VictimIndices.Count > 0,
            "THRESHOLD_CROSSING_IS_EXACT", $"stemsUnder={under} stemsOver={over} pressureOver={F(atOver.OvercrowdingPressure, "0.000000")}");
        Check(calls - callsUnder <= over, "AT_MOST_ONE_ROLL_PER_STEM", $"rolls={calls - callsUnder} stems={over}");

        // G: above the onset but below capacity the hazard decides; with a draw that never kills there are no deaths,
        // with one that always kills every stem with a non-zero probability dies.
        List<DensityStem> mid = RandomStand(new Random(42), 520, 20.0, 34.0, ThreeWay);
        DensityMortalityPlan zone = MixedDensityModel.Plan(mid, Area, KeyedRoll(2));
        DensityMortalityPlan zoneNever = MixedDensityModel.Plan(mid, Area, id => 0.9999999);
        DensityMortalityPlan zoneAlways = MixedDensityModel.Plan(mid, Area, id => 0.0);
        Check(zone.Occupancy > MixedDensityModel.OvercrowdingOnset && zone.Occupancy < 1.0 && zone.OvercrowdingPressure > 0.0 && zone.VictimIndices.Count > 0
            && zoneNever.VictimIndices.Count == 0 && zoneAlways.VictimIndices.Count > 0 && zoneAlways.BoundaryDeaths == 0,
            "ABOVE_THRESHOLD_MIXED_STAND_DENSITY_MORTALITY_OCCURS", $"occupancy={F(zone.Occupancy)} pressure={F(zone.OvercrowdingPressure)} deaths={zone.VictimIndices.Count} neverKillsDraw={zoneNever.VictimIndices.Count} alwaysKillsDraw={zoneAlways.VictimIndices.Count}");

        // Above capacity the boundary acts even when no hazard draw kills anything, and stops on the capacity.
        List<DensityStem> crowded = RandomStand(new Random(43), 900, 20.0, 40.0, ThreeWay);
        DensityMortalityPlan boundary = MixedDensityModel.Plan(crowded, Area, id => 0.9999999);
        var survivors = crowded.Where((s, i) => !boundary.VictimIndices.Contains(i)).ToList();
        Check(boundary.Occupancy > 1.0 && boundary.HazardDeaths == 0 && boundary.BoundaryDeaths > 0 && Occ(survivors) <= 1.0 + 1e-12 && survivors.Count > 1,
            "ABOVE_CAPACITY_MIXED_STAND_BOUNDARY_ACTS_AND_STOPS_ON_CAPACITY", $"occupancy={F(boundary.Occupancy)} boundaryDeaths={boundary.BoundaryDeaths} after={F(Occ(survivors))}");
    }

    // ---------- victim selection: competitive position only ----------

    private static void VictimSelection()
    {
        // Boundary victims go most suppressed (highest Hegyi index) first, ties by ordinal id; never by class.
        var random = new Random(424242);
        int trials = 0, orderOk = 0, selectivity = 0, selectivityTrials = 0, noDuplicates = 0;
        var perClassDeaths = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        for (int rep = 0; rep < 40; rep++)
        {
            List<DensityStem> stems = RandomStand(random, 600 + rep * 20, 16.0, 38.0, ThreeWay);
            DensityMortalityPlan plan = MixedDensityModel.Plan(stems, Area, KeyedRoll(rep));
            trials++;
            if (plan.VictimIndices.Distinct().Count() == plan.VictimIndices.Count) noDuplicates++;
            var boundary = plan.VictimIndices.Skip(plan.HazardDeaths).ToList();
            bool ordered = true;
            for (int k = 1; k < boundary.Count; k++)
            {
                DensityStem a = stems[boundary[k - 1]], b = stems[boundary[k]];
                if (a.CompetitionIndex < b.CompetitionIndex || (a.CompetitionIndex == b.CompetitionIndex && string.CompareOrdinal(a.TreeId, b.TreeId) > 0)) ordered = false;
            }
            if (ordered) orderOk++;
            var dead = new HashSet<int>(plan.VictimIndices);
            if (plan.VictimIndices.Count >= 5)
            {
                selectivityTrials++;
                if (plan.VictimIndices.Average(i => stems[i].CompetitionIndex) > stems.Where((s, i) => !dead.Contains(i)).Average(s => s.CompetitionIndex)) selectivity++;
            }
            foreach (int i in plan.VictimIndices) perClassDeaths[stems[i].Demand]++;
        }
        Check(noDuplicates == trials, "PLAN_NO_DUPLICATE_VICTIMS", $"trials={trials}");
        Check(orderOk == trials, "BOUNDARY_VICTIMS_MOST_SUPPRESSED_FIRST_BY_COMPETITION_ONLY", $"trials={trials} ok={orderOk}");
        Check(selectivityTrials > 10 && selectivity == selectivityTrials, "VICTIMS_MORE_SUPPRESSED_THAN_SURVIVORS", $"trials={selectivityTrials} ok={selectivity}");
        Check(perClassDeaths.Values.All(v => v > 0), "ALL_THREE_CLASSES_LOSE_STEMS_IN_AGGREGATE", $"reference={perClassDeaths[Ref]} intermediate={perClassDeaths[Mid]} high={perClassDeaths[Hi]}");

        // The suppression scale is one scale for every class.
        Check(Math.Abs(MixedDensityModel.Suppression(MixedDensityModel.NeutralHalfSaturationCi) - 0.5) < 1e-12 && MixedDensityModel.Suppression(0) == 0.0
            && MixedDensityModel.Suppression(1e9) <= 1.0 && MixedDensityModel.Suppression(2.0) < MixedDensityModel.Suppression(6.0), "SUPPRESSION_ONE_COMMON_SCALE");

        // No systematic species preference: 150 slots of three stems (one per class) with identical size and
        // competition and rotating ids. Pressure sits in the hazard zone, so only the hazard draw decides.
        double scale = 1.0;
        List<DensityStem> Triples(double s, double ciFactorForNonReference)
        {
            var stems = new List<DensityStem>();
            for (int slot = 0; slot < 150; slot++)
            {
                double dbh = (20.0 + slot % 25) * s, ci = 1.0 + slot % 6;
                for (int j = 0; j < 3; j++)
                {
                    GrowingSpaceDemandClass cls = ThreeWay(slot + j);
                    stems.Add(Stem("N" + (slot * 3 + j).ToString("D5", Inv), cls, dbh, cls == Ref ? ci : ci * ciFactorForNonReference));
                }
            }
            return stems.OrderBy(x => x.TreeId, StringComparer.Ordinal).ToList();
        }
        double lo = 0.2, hi = 3.0;
        for (int i = 0; i < 40; i++) { scale = (lo + hi) / 2.0; if (Occ(Triples(scale, 1.0)) > 0.92) hi = scale; else lo = scale; }
        List<DensityStem> neutral = Triples(scale, 1.0);
        var counts = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        int boundaryDeaths = 0;
        for (int seed = 1; seed <= 400; seed++)
        {
            DensityMortalityPlan p = MixedDensityModel.Plan(neutral, Area, KeyedRoll(seed));
            boundaryDeaths += p.BoundaryDeaths;
            foreach (int i in p.VictimIndices) counts[neutral[i].Demand]++;
        }
        int total = counts.Values.Sum();
        double sigma = Math.Sqrt(total * (1.0 / 3.0) * (2.0 / 3.0));
        double spread = counts.Values.Max(v => Math.Abs(v - total / 3.0)) / sigma;
        Check(total > 3000 && boundaryDeaths == 0 && spread < 5.0, "HAZARD_HAS_NO_SPECIES_PREFERENCE_AT_EQUAL_COMPETITION_AND_SIZE",
            $"occupancy={F(Occ(neutral))} deaths={total} reference={counts[Ref]} intermediate={counts[Mid]} high={counts[Hi]} maxDeviation={F(spread, "0.00")}sigma");

        // Negative control: the same fixture with the OLD per-species suppression (a broadleaf stem of Ci50 3 reads
        // as Ci 5/3 higher on the Sitka Ci50 5 scale) is clearly biased against Oak and Beech, so the test above
        // has the power to detect a species preference. This is what was removed.
        List<DensityStem> biased = Triples(scale, 5.0 / 3.0);
        var biasedCounts = new Dictionary<GrowingSpaceDemandClass, int> { [Ref] = 0, [Mid] = 0, [Hi] = 0 };
        for (int seed = 1; seed <= 400; seed++)
            foreach (int i in MixedDensityModel.Plan(biased, Area, KeyedRoll(seed)).VictimIndices) biasedCounts[biased[i].Demand]++;
        int biasedTotal = biasedCounts.Values.Sum();
        double biasedSpread = biasedCounts.Values.Max(v => Math.Abs(v - biasedTotal / 3.0)) / Math.Sqrt(biasedTotal * (1.0 / 3.0) * (2.0 / 3.0));
        Check(biasedSpread > 5.0 && biasedCounts[Ref] < biasedCounts[Mid] && biasedCounts[Ref] < biasedCounts[Hi], "NEGATIVE_CONTROL_PER_SPECIES_CI50_WOULD_BE_DETECTED",
            $"reference={biasedCounts[Ref]} intermediate={biasedCounts[Mid]} high={biasedCounts[Hi]} maxDeviation={F(biasedSpread, "0.0")}sigma");
    }

    // ---------- continuity with the accepted Sitka line (report plus loose guard) ----------

    // The capacity is a [C] number chosen so that a stand of Reference-weight stems lands near the accepted Sitka
    // line, so the first Oak in a Sitka stand does not make the Sitka part immune or doomed. This reads
    // SitkaGrowthModel only to REPORT that, here in the check. The production occupancy code never does.
    private static void Continuity()
    {
        double lowest = double.MaxValue, highest = double.MinValue;
        foreach (float dbh in new[] { 8f, 10f, 15f, 20f, 25f, 30f, 40f, 50f, 60f, 80f })
            foreach (int stems in new[] { 100, 300, 600 })
            {
                double ratio = Occ(Uniform(Ref, stems, dbh, 1.0, "S")) / SitkaGrowthModel.RelativeDensity(stems / Area, dbh);
                lowest = Math.Min(lowest, ratio);
                highest = Math.Max(highest, ratio);
            }
        Check(lowest > 0.93 && highest < 1.12, "CONTINUITY_REFERENCE_WEIGHT_STAND_NEAR_ACCEPTED_SITKA_LINE",
            $"occupancy/SitkaRD over Dq 8-80 cm: {F(lowest)} to {F(highest)} (guard 0.93 to 1.12)");
    }

    // ---------- determinism ----------

    private static void Determinism()
    {
        var random = new Random(1234);
        List<DensityStem> stems = RandomStand(random, 800, 15.0, 40.0, ThreeWay);
        string a = Fingerprint(MixedDensityModel.Plan(stems, Area, KeyedRoll(9)));
        string b = Fingerprint(MixedDensityModel.Plan(stems, Area, KeyedRoll(9)));
        Check(a == b, "REPLAY_IDENTICAL", a.Substring(0, 24));

        // The model puts stems in ordinal id order itself, so listing order cannot change who dies or in what order.
        var rng = new Random(7);
        List<DensityStem> shuffled = stems.OrderBy(_ => rng.Next()).ToList();
        DensityMortalityPlan p1 = MixedDensityModel.Plan(stems, Area, KeyedRoll(9));
        DensityMortalityPlan p2 = MixedDensityModel.Plan(shuffled, Area, KeyedRoll(9));
        bool sameVictims = p1.VictimIndices.Select(i => stems[i].TreeId).SequenceEqual(p2.VictimIndices.Select(i => shuffled[i].TreeId));
        Check(sameVictims && p1.VictimIndices.Count > 0 && BitConverter.DoubleToInt64Bits(p1.Occupancy) == BitConverter.DoubleToInt64Bits(p2.Occupancy)
            && p1.HazardDeaths == p2.HazardDeaths && p1.BoundaryDeaths == p2.BoundaryDeaths, "LISTING_ORDER_DOES_NOT_CHANGE_VICTIMS_OR_OCCUPANCY",
            $"victims={p1.VictimIndices.Count}");

        var distinct = new HashSet<string>();
        for (int year = 1; year <= 12; year++)
            distinct.Add(Fingerprint(MixedDensityModel.Plan(stems, Area, KeyedRoll(year))));
        Check(distinct.Count > 6, "DIFFERENT_YEARS_GIVE_DIFFERENT_DRAWS", $"distinctOutcomes={distinct.Count}/12");
    }

    // ---------- degenerate inputs ----------

    private static void Degenerate()
    {
        Check(MixedDensityModel.Plan(new List<DensityStem>(), Area, KeyedRoll(0)).VictimIndices.Count == 0, "EMPTY_STAND_NO_VICTIMS");
        Check(MixedDensityModel.Plan(Uniform(Hi, 5, 25f, 1.0, "O"), 0f, KeyedRoll(0)).VictimIndices.Count == 0, "ZERO_AREA_NO_VICTIMS");
        DensityMortalityPlan single = MixedDensityModel.Plan(Uniform(Hi, 1, 90f, 1.0, "O"), 0.01f, KeyedRoll(0));
        Check(single.OccupancyAfter > 1.0 && single.VictimIndices.Count == 0, "LAST_STEM_IS_NEVER_REMOVED_BY_THE_BOUNDARY", $"occupancy={F(single.Occupancy)}");
        // No competition at all: nobody is vulnerable, so the hazard picks nobody; the boundary still holds the capacity.
        DensityMortalityPlan free = MixedDensityModel.Plan(Uniform(Hi, 800, 30f, 0.0, "F"), Area, id => 0.0);
        Check(free.OvercrowdingPressure > 0.0 && free.HazardDeaths == 0 && free.BoundaryDeaths > 0 && free.OccupancyAfter <= 1.0 + 1e-12, "NO_COMPETITION_NO_HAZARD_BOUNDARY_STILL_HOLDS",
            $"occupancy={F(free.Occupancy)} boundary={free.BoundaryDeaths} after={F(free.OccupancyAfter)}");
        Check(MixedDensityModel.AnyNonReference(Uniform(Hi, 1, 20f, 1.0, "O")) && !MixedDensityModel.AnyNonReference(Uniform(Ref, 3, 20f, 1.0, "S")), "ANY_NON_REFERENCE");
        Check(MixedDensityModel.StemOccupancyM2(Stem("z", Hi, 0, 1.0)) == 0.0 && MixedDensityModel.ExpectedAnnualLoss(0.0) == 0.0 && MixedDensityModel.OvercrowdingPressure(0.0) == 0.0,
            "ZERO_SIZE_AND_ZERO_PRESSURE_ARE_INERT");
    }
}
