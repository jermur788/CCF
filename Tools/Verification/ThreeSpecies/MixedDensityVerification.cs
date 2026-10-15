using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable in-Editor gate for growth model 2 density (D-059, D-061), reworked after the first
// implementation was rejected for running Oak and Beech through the Sitka density equation.
//
// STATUS: written and type-checked in a code-only cloud session. NOT YET RUN IN UNITY.
// Run it before treating growth model 2 as verified.
//
// Copy into Assets/ForestPrototype, run with
//   -batchmode -nographics -executeMethod MixedDensityVerification.Begin
// (use CCF_STAND_GEOMETRY=0 for the historical Legacy40 path, as the regression runners do),
// then remove the copy and its .meta. Saves are captured/loaded in memory only.
//
// What it proves that the offline check (OfflineLogic/) cannot, on the real controller:
//   - ROUTE DIAGNOSTICS: every annual step records which route handled it (AdultMortalityAccount.Route).
//     Pure Sitka under model 2 reports LegacySitkaExact; any Oak/Beech reports SpeciesAwareOccupancy.
//   - PURE SITKA EXACT: model 2 equals model 1 year by year (route, relative density, pressure, hazard and
//     boundary counts bit for bit, victim ids, tree-state hash) and reproduces the lifecycle anchors.
//   - Pure Oak and pure Beech overcrowd, die from density pressure (a bounded annual loss, no hard ceiling) and
//     their occupancy falls; the species-aware step never computes a Sitka relative density.
//   - Mixed stands: dimensionless occupancy follows the demand classes and equals the pure model's prediction
//     from the real trees; replay is exact; below the onset nothing dies.
public static class MixedDensityVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "MixedDensityVerification.Begin"))
            new GameObject("Mixed density verification").AddComponent<MixedDensityVerificationRunner>();
    }
}

public sealed class MixedDensityVerificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    // Growth-model-1 pure-Sitka lifecycle anchors (SitkaGrowthModelVerification). Model 2 must reproduce them.
    private const string Growth1Neutral = "7E57B9DAEF5BF4D0", Growth1Normal = "35E2BF1C2F55C2DE";

    private ForestEcologyController e;
    private ScenarioOneManager manager;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private ForestSaveData original;
    private TreeSpeciesDefinition sitka, oak, beech;
    private readonly List<string> failures = new List<string>();
    private int passed;

    private static string F(double v, string f = "0.####") => v.ToString(f, Inv);
    private static long Bits(double v) => BitConverter.DoubleToInt64Bits(v);
    private void Pass(string name, bool ok, string detail)
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"MIXED_DENSITY_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Check(bool ok, string what) { if (!ok) { failures.Add(what); Debug.LogError("MIXED_DENSITY_CHECK_FAIL " + what); } }
    private ForestSaveData Clone(ForestSaveData d) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));
    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    // Hash of the simulation-relevant tree state, independent of the growth-model number
    // (the save JSON includes it, so WorldHash cannot be compared across models).
    private string TreeStateHash()
    {
        ulong hash = 14695981039346656037UL;
        void Bytes(byte[] values) { foreach (byte value in values) { hash ^= value; hash *= 1099511628211UL; } }
        void Text(string value) { Bytes(System.Text.Encoding.UTF8.GetBytes(value ?? "")); Bytes(new byte[1]); }
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).OrderBy(t => t.TreeId, StringComparer.Ordinal))
        {
            Text(t.TreeId); Text(t.IsLiving ? "L" : "D"); Text(t.MortalityCause);
            Bytes(BitConverter.GetBytes(t.Diameter)); Bytes(BitConverter.GetBytes(t.Height)); Bytes(BitConverter.GetBytes(t.AgeYears));
        }
        Text("YEAR:" + e.EcologicalYear);
        return hash.ToString("X16");
    }

    private AdultMortalityAccount DensityStep()
    {
        // Run only the density step on the stand as built: no growth between the density the test
        // measures and the density the model sees. Competition is computed lazily first.
        ForestTree first = Living().FirstOrDefault();
        if (first != null) e.GetCompetitionIndex(first);
        typeof(ForestEcologyController).GetMethod("ApplyAdultDensityMortality", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(e, null);
        return e.LastAdultMortality;
    }

    private static string Describe(AdultMortalityAccount a)
        => $"route={a.Route} occupancy={F(a.Occupancy)} sitkaRd={F(a.RelativeDensity)} pressure={F(a.DensityPressure)} hazard={a.HazardDeaths} boundary={a.BoundaryDeaths} deaths={a.Deaths}";

    // The real living trees as the occupancy model sees them: class from the species asset, DBH, Hegyi index.
    private List<DensityStem> StemsOfLiving()
    {
        ForestTree first = Living().FirstOrDefault();
        if (first != null) e.GetCompetitionIndex(first);
        return Living().Select(t => new DensityStem(t.TreeId, t.Species.GrowingSpaceDemand, t.Diameter, e.GetCompetitionIndex(t))).ToList();
    }

    private double OccupancyOfLiving() => MixedDensityModel.Occupancy(StemsOfLiving(), e.StandAreaHectares);

    // The legacy relative density of the living trees, restated exactly as the controller's Sitka route computes it.
    private double LegacyRdOfLiving()
    {
        ForestTree[] trees = Living();
        if (trees.Length == 0) return 0.0;
        double squaredDbhSum = 0.0;
        foreach (ForestTree t in trees) squaredDbhSum += (double)t.Diameter * t.Diameter;
        return SitkaGrowthModel.RelativeDensity(trees.Length / e.StandAreaHectares, Math.Sqrt(squaredDbhSum / trees.Length));
    }

    private float DbhOf(int k, float dbhMin, float spread) => dbhMin + ((k * 7919) % 100) / 100f * spread;

    private int StemCount(float stemsPerHectare) => Mathf.Max(1, Mathf.RoundToInt(stemsPerHectare * e.StandAreaHectares));

    // A regular grid of stems over the current stand bounds.
    private void Stand(int growth, Func<int, TreeSpeciesDefinition> speciesOf, int count, float dbhMin, float spread)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = growth;
        Rect bounds = e.StandBounds;
        int perAxis = Mathf.CeilToInt(Mathf.Sqrt(count));
        float spacing = (bounds.width - 2f) / perAxis;
        for (int k = 0; k < count; k++)
        {
            TreeSpeciesDefinition s = speciesOf(k);
            float d = DbhOf(k, dbhMin, spread);
            string prefix = s == sitka ? "S" : s == oak ? "O" : "B";
            var position = new Vector3(bounds.xMin + 1f + (k / perAxis + 0.5f) * spacing, 0f, bounds.yMin + 1f + (k % perAxis + 0.5f) * spacing);
            spawner.Spawn(prefix + k.ToString("0000"), s, position, 40, d, 2f + 0.62f * d, s.PotentialCrownRadiusM(d));
        }
        e.InvalidateCompetition();
        e.RecomputeCanopy();
    }

    // Occupancy the model predicts for a fixture BEFORE building it (the same ids, classes and sizes as Stand makes).
    private double PlannedOccupancy(int n, Func<int, TreeSpeciesDefinition> speciesOf, float dbhMin, float spread)
    {
        var stems = Enumerable.Range(0, n).Select(k => new DensityStem("T" + k.ToString("0000"), speciesOf(k).GrowingSpaceDemand, DbhOf(k, dbhMin, spread), 0.0)).ToList();
        return MixedDensityModel.Occupancy(stems, e.StandAreaHectares);
    }

    // ---------- policy ----------

    private IEnumerator Policy()
    {
        int newGame = ScenarioOneManager.NewGameGrowthModel;
        Pass("POLICY_NEW_GAME_GROWTH_UNCHANGED", newGame == GrowthModel.SiteClassDensity,
            $"newGameGrowth={newGame} (growth model 2 is NOT adopted; new games stay on model 1)");
        Pass("POLICY_VERSION_CONSTANTS", GrowthModel.SpeciesAware == 2 && GrowthModel.Latest == 2 && GrowthModel.Normalize(7) == 2, $"latest={GrowthModel.Latest}");
        Pass("POLICY_SAVE_VERSION_UNCHANGED", ForestSaveData.CurrentVersion == 20, $"saveVersion={ForestSaveData.CurrentVersion}");

        ForestSaveData two = Clone(original); two.growthModel = GrowthModel.SpeciesAware;
        Check(saves.LoadData(two, false), "growth 2 load"); yield return null;
        bool restoredTwo = e.GrowthModelVersion == 2;
        ForestSaveData three = Clone(original); three.growthModel = 3;
        string problem = ForestSaveValidation.Validate(three, 0, e.CellCount);
        ForestSaveData v16 = Clone(original); v16.version = 16; v16.growthModel = GrowthModel.SpeciesAware; v16.regenerationModel = RegenerationModel.AgeBands;
        // A save older than v20 cannot carry a stand geometry model (ForestSaveValidation.ValidateGeometry), so a v16-stamped copy of an
        // Enlarged80 world is not a possible save and is correctly refused. The v16 -> growth 0 reading can only be exercised from Legacy40;
        // under Enlarged80 the gate asserts the refusal instead.
        bool legacyGeometry = original.standGeometryModel == StandGeometryModel.Legacy40;
        bool v16Loaded = saves.LoadData(v16, false); yield return null;
        if (legacyGeometry) Check(v16Loaded, "v16 load");
        bool v16Zero = legacyGeometry ? e.GrowthModelVersion == 0 : !v16Loaded;
        Check(saves.LoadData(Clone(original), false), "original reload"); yield return null;
        Pass("POLICY_SAVE_SEMANTICS", restoredTwo && problem != null && problem.Contains("unknown growth model 3") && v16Zero,
            legacyGeometry ? $"growth2Restored={restoredTwo} growth3Rejected=[{problem}] v16->0={v16Zero}"
                           : $"growth2Restored={restoredTwo} growth3Rejected=[{problem}] v16 stamped on an Enlarged80 world refused={v16Zero} (pre-v20 saves are Legacy40 only; v16->0 is asserted by the Legacy40 run)");

        bool previewZero = true, restored = true;
        int session = e.GrowthModelVersion;
        foreach (int year in new[] { 0, 50 })
        {
            Check(manager.TryBeginReferencePreview(year), "preview " + year);
            yield return null; yield return null;
            previewZero &= e.GrowthModelVersion == 0;
            manager.EndReferencePreview();
            yield return null; yield return null;
            restored &= e.GrowthModelVersion == session;
        }
        Pass("POLICY_REFERENCE_V1_GROWTH0", previewZero && restored, $"previewGrowth0={previewZero} sessionModelRestored={restored}");
    }

    // ---------- species data ----------

    private IEnumerator SpeciesData()
    {
        Pass("SPECIES_CLASSES", sitka.GrowingSpaceDemand == GrowingSpaceDemandClass.Reference
            && beech.GrowingSpaceDemand == GrowingSpaceDemandClass.Intermediate && oak.GrowingSpaceDemand == GrowingSpaceDemandClass.High,
            $"sitka={sitka.GrowingSpaceDemand} beech={beech.GrowingSpaceDemand} oak={oak.GrowingSpaceDemand}");

        // TEST E: at equal size a higher demand class takes more occupancy (pure function; no biological magnitude is asserted).
        bool ordered = true;
        foreach (float dbh in new[] { 6f, 15f, 30f, 55f, 90f })
        {
            double r = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", sitka.GrowingSpaceDemand, dbh, 1.0));
            double m = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", beech.GrowingSpaceDemand, dbh, 1.0));
            double h = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", oak.GrowingSpaceDemand, dbh, 1.0));
            ordered &= r < m && m < h;
        }
        Pass("DEMAND_ORDER_ON_REAL_SPECIES_CLASSES", ordered, "Sitka < Beech < Oak at equal DBH");
        yield return null;
    }

    // ---------- TEST A: routing, call path and pure Sitka exactness ----------

    private IEnumerator PureSitkaExact()
    {
        // Same stand, same seeds, same years, growth model 1 then growth model 2.
        var years = new Dictionary<int, List<string>> { [1] = new List<string>(), [2] = new List<string>() };
        var victims = new Dictionary<int, List<string>> { [1] = new List<string>(), [2] = new List<string>() };
        var finalHash = new Dictionary<int, string>();
        int hazardYears = 0, boundaryYears = 0, totalDeaths = 0;
        bool model2AlwaysLegacy = true, model2NeverOccupancy = true;
        foreach (int growth in new[] { 1, 2 })
        {
            Stand(growth, _ => sitka, StemCount(1200f), 24f, 10f);
            yield return null;
            for (int y = 0; y < 10; y++)
            {
                HashSet<string> before = Living().Select(t => t.TreeId).ToHashSet();
                e.AdvanceOneYear();
                AdultMortalityAccount a = e.LastAdultMortality;
                years[growth].Add($"{a.Route}|{Bits(a.RelativeDensity):X16}|{Bits(a.DensityPressure):X16}|{Bits(a.StemsPerHectare):X16}|{Bits(a.QuadraticMeanDbhCm):X16}|{a.HazardDeaths}|{a.BoundaryDeaths}|{a.Deaths}|{a.LivingBefore}");
                victims[growth].Add(string.Join(",", before.Except(Living().Select(t => t.TreeId)).OrderBy(id => id, StringComparer.Ordinal)));
                if (growth == 2)
                {
                    model2AlwaysLegacy &= a.Route == DensityRoute.LegacySitkaExact;
                    model2NeverOccupancy &= a.Route != DensityRoute.SpeciesAwareOccupancy && a.Occupancy == 0.0;
                    if (a.HazardDeaths > 0) hazardYears++;
                    if (a.BoundaryDeaths > 0) boundaryYears++;
                    totalDeaths += a.Deaths;
                }
                yield return null;
            }
            finalHash[growth] = TreeStateHash();
        }
        bool identical = years[1].SequenceEqual(years[2]) && victims[1].SequenceEqual(victims[2]) && finalHash[1] == finalHash[2];
        Pass("PURE_SITKA_MODEL2_EXACTLY_EQUALS_MODEL1", identical && totalDeaths > 0 && hazardYears > 0,
            $"yearsIdentical={years[1].SequenceEqual(years[2])} victimIdsIdentical={victims[1].SequenceEqual(victims[2])} hash1={finalHash[1]} hash2={finalHash[2]} deaths={totalDeaths} hazardYears={hazardYears} boundaryYears={boundaryYears}");
        Pass("PURE_SITKA_MODEL2_CALLS_LEGACY_SITKA_ROUTE_ONLY", model2AlwaysLegacy && model2NeverOccupancy, $"alwaysLegacy={model2AlwaysLegacy} neverOccupancy={model2NeverOccupancy}");

        // Call path on the stand as built, both ways round.
        Stand(2, _ => sitka, StemCount(900f), 24f, 10f);
        double legacyRd = LegacyRdOfLiving();
        AdultMortalityAccount pure = DensityStep();
        Pass("CALL_PATH_PURE_SITKA_USES_SITKA_RELATIVE_DENSITY", pure.Route == DensityRoute.LegacySitkaExact && Math.Abs(pure.RelativeDensity - legacyRd) <= 1e-12 * Math.Max(1.0, legacyRd)
            && pure.Occupancy == 0.0 && legacyRd > 0.0, $"{Describe(pure)} independentSitkaRd={F(legacyRd)}");

        Func<int, TreeSpeciesDefinition> oneOak = k => k == 0 ? oak : sitka;
        Stand(2, oneOak, StemCount(900f), 24f, 10f);
        List<DensityStem> mixedStems = StemsOfLiving();
        double predicted = MixedDensityModel.Occupancy(mixedStems, e.StandAreaHectares);
        AdultMortalityAccount mixed = DensityStep();
        Stand(1, oneOak, StemCount(900f), 24f, 10f);
        AdultMortalityAccount model1 = DensityStep();
        Pass("CALL_PATH_ONE_OAK_SWITCHES_TO_OCCUPANCY_ROUTE", mixed.Route == DensityRoute.SpeciesAwareOccupancy && mixed.RelativeDensity == 0.0
            && Math.Abs(mixed.Occupancy - predicted) <= 1e-12 && model1.Route == DensityRoute.LegacySitkaExact && model1.RelativeDensity > 0.0 && model1.Occupancy == 0.0,
            $"model2 {Describe(mixed)} predictedOccupancy={F(predicted)} | model1 {Describe(model1)}");
    }

    // ---------- TESTS B and C: pure Oak and pure Beech ----------

    private IEnumerator PureBroadleaf()
    {
        // Same stems, same sizes, three species. The Sitka-class stand is below the onset; Beech and Oak are not.
        int n = StemCount(800f);
        var sizes = Enumerable.Range(0, n).Select(k => new DensityStem("T" + k.ToString("0000"), GrowingSpaceDemandClass.Reference, DbhOf(k, 24f, 10f), 0.0)).ToList();
        double referenceOccupancy = MixedDensityModel.Occupancy(sizes, e.StandAreaHectares);
        var accounts = new Dictionary<string, AdultMortalityAccount>();
        foreach ((string label, TreeSpeciesDefinition s) in new[] { ("beech", beech), ("oak", oak) })
        {
            Stand(2, _ => s, n, 24f, 10f);
            accounts[label] = DensityStep();
            yield return null;
        }
        Pass("DENSE_SAME_STEMS_OCCUPANCY_FOLLOWS_DEMAND_ORDER",
            referenceOccupancy < accounts["beech"].Occupancy && accounts["beech"].Occupancy < accounts["oak"].Occupancy
            && accounts["beech"].Route == DensityRoute.SpeciesAwareOccupancy && accounts["oak"].Route == DensityRoute.SpeciesAwareOccupancy
            && accounts["oak"].DensityPressure >= accounts["beech"].DensityPressure,
            $"asReference={F(referenceOccupancy)} beech={F(accounts["beech"].Occupancy)} oak={F(accounts["oak"].Occupancy)} onset={F(MixedDensityModel.OnsetOccupancy)}");

        // Intentionally overcrowded pure stands: occupancy well above the onset, deaths from density pressure, a bounded loss.
        foreach ((string label, TreeSpeciesDefinition s, float perHectare) in new[] { ("BEECH", beech, 1300f), ("OAK", oak, 1000f) })
        {
            int count = StemCount(perHectare);
            Stand(2, _ => s, count, 24f, 10f);
            yield return null;
            double before = OccupancyOfLiving();
            AdultMortalityAccount step = DensityStep();
            double after = OccupancyOfLiving();
            var dead = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(t => t.IsBiologicallyDead).ToList();
            Pass($"PURE_{label}_DENSE_OVERCROWDED_AND_DIES_FROM_DENSITY_PRESSURE",
                before > 1.0 && Math.Abs(step.Occupancy - before) <= 1e-12 && step.Route == DensityRoute.SpeciesAwareOccupancy && step.RelativeDensity == 0.0
                && step.DensityPressure > 0.0 && step.Deaths > 0 && dead.Count == step.Deaths && dead.All(t => t.MortalityCause == "self-thinning"),
                $"occupancyBefore={F(before)} {Describe(step)}");
            Pass($"PURE_{label}_OCCUPANCY_FALLS_AFTER_THE_STEP_AND_LOSS_IS_BOUNDED", after < before && Living().Length > 1 && step.DensityPressure < 1.0 && step.Deaths <= Math.Ceiling(MixedDensityModel.GenericMaximumAnnualLoss * count) && step.BoundaryDeaths == 0,
                $"occupancy {F(before)} -> {F(after)} living={Living().Length}/{count} pressure={F(step.DensityPressure)}");

            // Over the years: every death is a recorded self-thinning death in a valid year with deadwood created exactly once.
            Stand(2, _ => s, count, 24f, 10f);
            yield return null;
            int deadwoodBefore = manager.DeadwoodRecords.Count;
            var died = new Dictionary<string, (int year, string cause)>();
            bool onlyOccupancyRoute = true;
            for (int y = 0; y < 10; y++)
            {
                HashSet<string> alive = Living().Select(t => t.TreeId).ToHashSet();
                e.AdvanceOneYear();
                onlyOccupancyRoute &= e.LastAdultMortality.Route == DensityRoute.SpeciesAwareOccupancy && e.LastAdultMortality.RelativeDensity == 0.0;
                foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    if (t.IsBiologicallyDead && alive.Contains(t.TreeId) && !died.ContainsKey(t.TreeId))
                        died[t.TreeId] = (t.MortalityYear, t.MortalityCause);
                yield return null;
            }
            var records = manager.DeadwoodRecords.Skip(deadwoodBefore).ToList();
            Pass($"PURE_{label}_SELF_THINS_OVER_TEN_YEARS", died.Count > 0 && died.Values.All(v => v.cause == "self-thinning" && v.year >= 1 && v.year <= 10) && onlyOccupancyRoute,
                $"deaths={died.Count}/{count} onlyOccupancyRoute={onlyOccupancyRoute}");
            Pass($"PURE_{label}_DEADWOOD_ONCE", records.Count == died.Count && records.Select(r => r.treeId).Distinct().Count() == records.Count, $"records={records.Count} deaths={died.Count}");
        }
    }

    // ---------- TEST D: mixed Sitka / Oak / Beech ----------

    private IEnumerator Mixtures()
    {
        // Replacing a higher-demand stem by a lower class at the same place and size lowers occupancy in the ordinal direction.
        // Two Oaks, then Oak+Beech, then Oak+Sitka. The remaining Oak keeps the stand on the occupancy route throughout.
        int n = StemCount(900f);
        var variants = new (string label, Func<int, TreeSpeciesDefinition> speciesOf)[]
        {
            ("TWO_OAK", k => k < 2 ? oak : sitka),
            ("OAK_BEECH", k => k == 0 ? oak : k == 1 ? beech : sitka),
            ("OAK_SITKA", k => k == 0 ? oak : sitka)
        };
        var occupancy = new List<double>();
        foreach (var variant in variants)
        {
            Stand(2, variant.speciesOf, n, 24f, 10f);
            occupancy.Add(DensityStep().Occupancy);
            yield return null;
        }
        double perStep = MixedDensityModel.StemOccupiedSpace(new DensityStem("x", GrowingSpaceDemandClass.Reference, DbhOf(1, 24f, 10f), 0.0)) / e.StandAreaHectares / MixedDensityModel.OccupancyNormalization;
        Pass("MIXED_DOWNGRADING_A_HIGH_DEMAND_STEM_LOWERS_OCCUPANCY_IN_ORDER",
            occupancy[0] > occupancy[1] && occupancy[1] > occupancy[2]
            && Math.Abs((occupancy[0] - occupancy[1]) - 0.5 * perStep) <= 1e-9 && Math.Abs((occupancy[1] - occupancy[2]) - 0.5 * perStep) <= 1e-9,
            $"twoOak={F(occupancy[0], "0.000000")} oakBeech={F(occupancy[1], "0.000000")} oakSitka={F(occupancy[2], "0.000000")} stepPerHalfWeight={F(0.5 * perStep, "0.000000")}");

        // Four mixtures: occupancy equals the pure model's prediction from the real trees, deaths occur, replay is exact,
        // and the occupancy is a plain sum (no mixture bonus).
        var mixes = new Dictionary<string, Func<int, TreeSpeciesDefinition>>
        {
            ["SITKA_OAK"] = k => k % 2 == 0 ? sitka : oak,
            ["SITKA_BEECH"] = k => k % 2 == 0 ? sitka : beech,
            ["OAK_BEECH"] = k => k % 2 == 0 ? oak : beech,
            ["ALL_THREE"] = k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak
        };
        int count = StemCount(900f);
        foreach (KeyValuePair<string, Func<int, TreeSpeciesDefinition>> mix in mixes)
        {
            string firstRun = null;
            double observed = 0, predicted = 0, partsSum = 0;
            int deaths = 0;
            bool routed = true;
            for (int run = 0; run < 2; run++)
            {
                Stand(2, mix.Value, count, 24f, 10f);
                yield return null;
                if (run == 0)
                {
                    List<DensityStem> stems = StemsOfLiving();
                    predicted = MixedDensityModel.Occupancy(stems, e.StandAreaHectares);
                    partsSum = Enum.GetValues(typeof(GrowingSpaceDemandClass)).Cast<GrowingSpaceDemandClass>()
                        .Sum(c => MixedDensityModel.Occupancy(stems.Where(s => s.Demand == c).ToList(), e.StandAreaHectares));
                    AdultMortalityAccount account = DensityStep();
                    observed = account.Occupancy;
                    routed = account.Route == DensityRoute.SpeciesAwareOccupancy && account.RelativeDensity == 0.0;
                    Stand(2, mix.Value, count, 24f, 10f);
                    yield return null;
                }
                for (int y = 0; y < 8; y++)
                {
                    e.AdvanceOneYear();
                    if (run == 0) { deaths += e.LastAdultMortality.Deaths; routed &= e.LastAdultMortality.Route == DensityRoute.SpeciesAwareOccupancy; }
                    yield return null;
                }
                string hash = TreeStateHash();
                if (run == 0) firstRun = hash;
                else Pass($"MIX_{mix.Key}", routed && Math.Abs(observed - predicted) <= 1e-12 && Math.Abs(predicted - partsSum) <= 1e-9 && deaths > 0 && hash == firstRun,
                    $"occupancy={F(observed)} predicted={F(predicted)} sumOfClassParts={F(partsSum)} deaths8y={deaths} routed={routed} replayIdentical={hash == firstRun}");
            }
        }
    }

    // ---------- TESTS F and G: below and above the generic limits on the real controller ----------

    private IEnumerator Thresholds()
    {
        // F: below the onset nothing dies. G: just over it pressure starts. Find the bracket with the pure model, check the controller.
        Func<int, TreeSpeciesDefinition> speciesOf = k => k % 2 == 0 ? sitka : oak;
        int below = 0, above = 0;
        for (int count = 20; count < 6000; count++)
        {
            if (PlannedOccupancy(count, speciesOf, 24f, 10f) > MixedDensityModel.OnsetOccupancy) { above = count; below = count - 1; break; }
        }
        Check(above > 0, "threshold bracket found");
        Stand(2, speciesOf, below, 24f, 10f);
        AdultMortalityAccount underOnset = DensityStep();
        Stand(2, speciesOf, above, 24f, 10f);
        AdultMortalityAccount overOnset = DensityStep();
        Pass("BELOW_THRESHOLD_MIXED_STAND_NO_DENSITY_MORTALITY", underOnset.Route == DensityRoute.SpeciesAwareOccupancy && underOnset.DensityPressure == 0.0 && underOnset.Deaths == 0,
            $"stems={below} {Describe(underOnset)}");
        Pass("THRESHOLD_CROSSING_ON_CONTROLLER", overOnset.Route == DensityRoute.SpeciesAwareOccupancy && overOnset.DensityPressure > 0.0 && overOnset.Occupancy > underOnset.Occupancy,
            $"stemsBelow={below} stemsAbove={above} pressureAbove={F(overOnset.DensityPressure, "0.000000")}");

        // Well below the onset, across several years: no density mortality at all.
        Stand(2, speciesOf, StemCount(250f), 24f, 10f);
        yield return null;
        int quietDeaths = 0;
        bool quietRoute = true;
        for (int y = 0; y < 5; y++) { e.AdvanceOneYear(); quietDeaths += e.LastAdultMortality.Deaths; quietRoute &= e.LastAdultMortality.Route == DensityRoute.SpeciesAwareOccupancy; yield return null; }
        Pass("BELOW_THRESHOLD_NO_DENSITY_DEATHS_OVER_YEARS", quietDeaths == 0 && quietRoute, $"deaths5y={quietDeaths} occupancyRoute={quietRoute}");

        // G: a modest excess over the onset: pressure starts, and deaths occur over the years (a small bounded loss).
        float perHectare = 100f;
        int zoneCount = 0;
        for (int c = 50; c < 6000; c++)
            if (PlannedOccupancy(c, speciesOf, 24f, 10f) > 1.2) { zoneCount = c; perHectare = c / e.StandAreaHectares; break; }
        Stand(2, speciesOf, zoneCount, 24f, 10f);
        yield return null;
        AdultMortalityAccount zone = DensityStep();
        Pass("ABOVE_THRESHOLD_MIXED_STAND_HAS_PRESSURE", zone.Route == DensityRoute.SpeciesAwareOccupancy && zone.Occupancy > MixedDensityModel.OnsetOccupancy && zone.Occupancy < 1.5
            && zone.DensityPressure > 0.0 && zone.DensityPressure < 1.0 && zone.BoundaryDeaths == 0,
            $"stemsPerHa={F(perHectare, "0")} {Describe(zone)}");
        Stand(2, speciesOf, zoneCount, 24f, 10f);
        yield return null;
        int zoneDeaths = 0;
        for (int y = 0; y < 8; y++) { e.AdvanceOneYear(); zoneDeaths += e.LastAdultMortality.Deaths; yield return null; }
        Pass("ABOVE_THRESHOLD_MIXED_STAND_LOSES_STEMS_OVER_YEARS", zoneDeaths > 0, $"deaths8y={zoneDeaths}");

        // Heavily overcrowded: more pressure than the modest case, deaths occur, the annual loss stays bounded (no hard ceiling).
        Stand(2, speciesOf, StemCount(1300f), 24f, 10f);
        int crowdedStems = Living().Length;
        double crowdedBefore = OccupancyOfLiving();
        AdultMortalityAccount crowded = DensityStep();
        double crowdedAfter = OccupancyOfLiving();
        Pass("HEAVY_OVERCROWDING_THINS_MORE_BUT_REMAINS_BOUNDED", crowdedBefore > 1.5 && crowded.DensityPressure > zone.DensityPressure && crowded.Deaths > 0
            && crowded.Deaths <= Math.Ceiling(MixedDensityModel.GenericMaximumAnnualLoss * crowdedStems) && crowdedAfter < crowdedBefore,
            $"occupancyBefore={F(crowdedBefore)} occupancyAfter={F(crowdedAfter)} stems={crowdedStems} {Describe(crowded)}");
    }

    // ---------- save / load through a dying mixed stand ----------

    private IEnumerator SaveLoad()
    {
        Stand(2, k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak, StemCount(900f), 24f, 10f);
        yield return null;
        for (int y = 0; y < 3; y++) { e.AdvanceOneYear(); yield return null; }
        string mid = JsonUtility.ToJson(saves.CaptureData());
        for (int y = 0; y < 5; y++) { e.AdvanceOneYear(); yield return null; }
        string uninterrupted = WorldHash();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(mid), false), "mid load");
        yield return null;
        bool modelKept = e.GrowthModelVersion == GrowthModel.SpeciesAware;
        for (int y = 0; y < 5; y++) { e.AdvanceOneYear(); yield return null; }
        string resumed = WorldHash();
        Pass("SAVE_LOAD_DETERMINISTIC_MIXED", uninterrupted == resumed && modelKept, $"uninterrupted={uninterrupted} resumed={resumed} modelKept={modelKept}");
    }

    // ---------- anchors ----------

    private IEnumerator Anchors()
    {
        MethodInfo hashMethod = typeof(ScenarioOneInteractionGate).GetMethod("LifecycleHash", BindingFlags.Static | BindingFlags.NonPublic);
        float scenarioPressure = manager.Definition.BackgroundBrowsePressure;
        var results = new Dictionary<string, string>();
        foreach (int model in new[] { GrowthModel.SiteClassDensity, GrowthModel.SpeciesAware })
            foreach (float pressure in new[] { 0f, scenarioPressure })
            {
                ForestStandScenarios.ApplyLifecycleFixture();
                e.RngModelVersion = 1; e.RegenerationModelVersion = 1; e.GrowthModelVersion = model;
                e.Browsing.BackgroundPressure = pressure; e.Browsing.ClearProtection();
                for (int year = 0; year < 80; year++) { e.AdvanceOneYear(); if (year % 10 == 9) yield return null; }
                results[$"m{model}-{(pressure == 0f ? "neutral" : "normal")}"] = (string)hashMethod.Invoke(null, new object[] { e });
            }
        e.Browsing.BackgroundPressure = scenarioPressure;
        // The hash covers every ecology cell, so the recorded constants are Legacy40 values. In EVERY geometry growth model 2 must equal
        // growth model 1 on this pure-Sitka fixture; under Legacy40 both must also equal the recorded constants.
        bool sameAsModel1 = results["m2-neutral"] == results["m1-neutral"] && results["m2-normal"] == results["m1-normal"];
        bool legacy40 = e.StandGeometryModelVersion == StandGeometryModel.Legacy40;
        bool constants = results["m2-neutral"] == Growth1Neutral && results["m2-normal"] == Growth1Normal;
        Pass("LIFECYCLE_ANCHORS_GROWTH2_EQUAL_GROWTH1", sameAsModel1 && (!legacy40 || constants),
            $"geometry={(legacy40 ? "Legacy40" : "Enlarged80")} model1 neutral={results["m1-neutral"]} normal={results["m1-normal"]} | model2 neutral={results["m2-neutral"]} normal={results["m2-normal"]}"
            + (legacy40 ? $" | recorded Legacy40 constants {Growth1Neutral}/{Growth1Normal} matched={constants}" : " | recorded constants are Legacy40-only (asserted by the Legacy40 run)"));
    }

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        saves = FindFirstObjectByType<ForestSaveController>();
        sitka = spawner.ResolveSpecies("sitka-spruce");
        oak = spawner.ResolveSpecies("sessile-oak");
        beech = spawner.ResolveSpecies("beech");
        Check(sitka != null && oak != null && beech != null, "species resolve");
        original = saves.CaptureData();
        yield return SpeciesData();
        yield return Policy();
        yield return PureSitkaExact();
        yield return PureBroadleaf();
        yield return Mixtures();
        yield return Thresholds();
        yield return SaveLoad();
        yield return Anchors();
    }

    private IEnumerator Start()
    {
        yield return null; yield return null;
        string failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Execute());
        while (stack.Count > 0)
        {
            bool more = false; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception ex) { failure = ex.ToString(); }
            if (failure != null) break;
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        try { if (original != null) { manager.EndReferencePreview(); saves.LoadData(original, false); } }
        catch (Exception ex) { failure = failure ?? ex.ToString(); }
        bool ok = failure == null && failures.Count == 0;
        Debug.Log(ok ? $"MIXED_DENSITY_VERIFY_PASS checks={passed}" : $"MIXED_DENSITY_VERIFY_FAIL {failure} checks=[{string.Join(" | ", failures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }
}
