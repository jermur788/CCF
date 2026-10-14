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

// Disposable in-Editor gate for growth model 2 (species-aware mixed density, D-059/D-061).
//
// STATUS: written and type-checked in a code-only cloud session. NOT YET RUN IN UNITY.
// Run it before treating growth model 2 as verified.
//
// Copy into Assets/ForestPrototype, run with
//   -batchmode -nographics -executeMethod MixedDensityVerification.Begin
// (use CCF_STAND_GEOMETRY=0 for the historical Legacy40 path, as the regression runners do),
// then remove the copy and its .meta. Saves are captured/loaded in memory only.
//
// What it proves that the offline check (OfflineLogic/) cannot: the controller routes pure
// Sitka to the untouched Sitka route and anything else to the species-aware route, the
// species assets carry their classes, the save policy accepts model 2 and rejects 3, and
// the lifecycle anchors are unchanged when model 2 runs a pure-Sitka stand.
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
    // Enlarged80 lifecycle values under growth model 1 (and, identically, model 2). OBSERVED in the density-v2 Unity qualification
    // (U3); they are not Manager-accepted anchors and are pinned here only so the Enlarged80 run checks against Enlarged80 values.
    private const string Growth1NeutralE80 = "1FE6639780636394", Growth1NormalE80 = "702DE05F33384139";

    // Stand-geometry expectations are explicit: each geometry is checked against its own values and nothing falls back to another
    // geometry's anchors. An unknown geometry fails loudly instead of being compared with the wrong table.
    private static bool TryLifecycleExpectation(int geometry, out string neutral, out string normal, out string basis)
    {
        if (geometry == StandGeometryModel.Legacy40)
        {
            neutral = Growth1Neutral; normal = Growth1Normal; basis = "accepted Legacy40 anchors (SitkaGrowthModelVerification)";
            return true;
        }
        if (geometry == StandGeometryModel.Enlarged80)
        {
            neutral = Growth1NeutralE80; normal = Growth1NormalE80; basis = "observed Enlarged80 growth-model-1 values (U3), not accepted anchors";
            return true;
        }
        neutral = normal = basis = null;
        return false;
    }

    private ForestEcologyController e;
    private ScenarioOneManager manager;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private ForestSaveData original;
    private TreeSpeciesDefinition sitka, oak, beech;
    private readonly List<string> failures = new List<string>();
    private int passed;

    private static string F(double v, string f = "0.####") => v.ToString(f, Inv);
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

    // Demand-weighted density of the living trees, computed with the production model.
    private double WeightedRd(IEnumerable<ForestTree> trees)
    {
        var stems = trees.Select(t => new DensityStem(t.TreeId, t.Species.SpeciesId, t.Species.GrowingSpaceDemand, t.Diameter, 0.0)).ToList();
        return MixedDensityModel.RelativeDensity(stems, e.StandAreaHectares);
    }

    private double SitkaRd(IEnumerable<ForestTree> trees)
    {
        ForestTree[] a = trees.ToArray();
        if (a.Length == 0) return 0;
        double dq = Math.Sqrt(a.Average(t => (double)t.Diameter * t.Diameter));
        return SitkaGrowthModel.RelativeDensity(a.Length / e.StandAreaHectares, dq);
    }

    // A regular grid of stems at the requested stems/ha over the current stand bounds.
    private float DbhOf(int k, float dbhMin, float spread) => dbhMin + ((k * 7919) % 100) / 100f * spread;

    private int StemCount(float stemsPerHectare) => Mathf.Max(1, Mathf.RoundToInt(stemsPerHectare * e.StandAreaHectares));

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

    private double PlannedWeightedRd(int n, Func<int, TreeSpeciesDefinition> speciesOf, float dbhMin, float spread)
    {
        var stems = Enumerable.Range(0, n).Select(k => new DensityStem("T" + k.ToString("0000"), speciesOf(k).SpeciesId, speciesOf(k).GrowingSpaceDemand, DbhOf(k, dbhMin, spread), 0.0)).ToList();
        return MixedDensityModel.RelativeDensity(stems, e.StandAreaHectares);
    }

    // ---------- policy ----------

    private IEnumerator Policy()
    {
        int newGame = ScenarioOneManager.NewGameGrowthModel;
        Pass("POLICY_NEW_GAME_GROWTH", newGame == GrowthModel.SiteClassDensity || newGame == GrowthModel.SpeciesAware,
            $"newGameGrowth={newGame} (1 until the Manager adopts model 2 for new games; 2 afterwards)");
        Pass("POLICY_VERSION_CONSTANTS", GrowthModel.SpeciesAware == 2 && GrowthModel.Latest == 2 && GrowthModel.Normalize(7) == 2, $"latest={GrowthModel.Latest}");

        ForestSaveData two = Clone(original); two.growthModel = GrowthModel.SpeciesAware;
        Check(saves.LoadData(two, false), "growth 2 load"); yield return null;
        bool restoredTwo = e.GrowthModelVersion == 2;
        ForestSaveData three = Clone(original); three.growthModel = 3;
        string problem = ForestSaveValidation.Validate(three, 0, e.CellCount);
        // A v16 save is a Legacy40 format (saves up to v19 must state Legacy40). Under Legacy40 it loads and maps to growth model 0.
        // Under Enlarged80 the same labelled save carries the wrong geometry and must be REFUSED, leaving the session untouched.
        bool legacyGeometry = e.StandGeometryModelVersion == StandGeometryModel.Legacy40;
        ForestSaveData v16 = Clone(original); v16.version = 16; v16.growthModel = GrowthModel.SpeciesAware; v16.regenerationModel = RegenerationModel.AgeBands;
        int sessionGrowth = e.GrowthModelVersion;
        bool v16Accepted = saves.LoadData(v16, false); yield return null;
        bool v16Behaviour;
        string v16Detail;
        if (legacyGeometry)
        {
            Check(v16Accepted, "v16 load (Legacy40)");
            v16Behaviour = v16Accepted && e.GrowthModelVersion == 0;
            v16Detail = $"v16->0={v16Behaviour}";
        }
        else
        {
            Check(!v16Accepted, "v16-labelled save with Enlarged80 geometry is refused");
            v16Behaviour = !v16Accepted && e.GrowthModelVersion == sessionGrowth;
            v16Detail = $"v16(Enlarged80 geometry)refused={!v16Accepted} sessionUnchanged={e.GrowthModelVersion == sessionGrowth} (the v16->0 mapping is a Legacy40 property, checked in the Legacy40 run)";
        }
        Check(saves.LoadData(Clone(original), false), "original reload"); yield return null;
        Pass("POLICY_SAVE_SEMANTICS", restoredTwo && problem != null && problem.Contains("unknown growth model 3") && v16Behaviour,
            $"geometry={StandGeometryModel.Label(e.StandGeometryModelVersion)} growth2Restored={restoredTwo} growth3Rejected=[{problem}] {v16Detail}");

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
        yield return null;
    }

    // ---------- routing and exactness ----------

    private IEnumerator Routing()
    {
        // Pure Sitka under model 2 takes the Sitka route and behaves exactly as model 1, year by year.
        var yearly = new Dictionary<int, List<int>> { [1] = new List<int>(), [2] = new List<int>() };
        var finalHash = new Dictionary<int, string>();
        bool everSpeciesAware = false;
        foreach (int growth in new[] { 1, 2 })
        {
            Stand(growth, _ => sitka, StemCount(1200f), 24f, 10f);
            yield return null;
            for (int y = 0; y < 10; y++)
            {
                e.AdvanceOneYear();
                yearly[growth].Add(e.LastAdultMortality.Deaths);
                if (growth == 2 && e.LastAdultMortality.SpeciesAwareRoute) everSpeciesAware = true;
                yield return null;
            }
            finalHash[growth] = TreeStateHash();
        }
        int totalDeaths = yearly[1].Sum();
        Pass("PURE_SITKA_MODEL2_EQUALS_MODEL1", yearly[1].SequenceEqual(yearly[2]) && finalHash[1] == finalHash[2] && totalDeaths > 0 && !everSpeciesAware,
            $"deathsPerYear={string.Join(",", yearly[1])} hash1={finalHash[1]} hash2={finalHash[2]} speciesAwareRouteUsed={everSpeciesAware}");

        // One Oak among Sitka switches the stand to the species-aware route; model 1 never does.
        Func<int, TreeSpeciesDefinition> oneOak = k => k == 0 ? oak : sitka;
        Stand(2, oneOak, StemCount(900f), 24f, 10f);
        AdultMortalityAccount mixed = DensityStep();
        Stand(1, oneOak, StemCount(900f), 24f, 10f);
        AdultMortalityAccount model1 = DensityStep();
        Pass("ROUTE_SWITCHES_ON_NON_REFERENCE", mixed.SpeciesAwareRoute && !model1.SpeciesAwareRoute,
            $"model2SpeciesAware={mixed.SpeciesAwareRoute} model1SpeciesAware={model1.SpeciesAwareRoute} weightedRd={F(mixed.RelativeDensity)} unweightedRd={F(mixed.UnweightedRelativeDensity)}");
        Stand(2, _ => sitka, StemCount(900f), 24f, 10f);
        Pass("ROUTE_PURE_SITKA_USES_SITKA_ROUTE", !DensityStep().SpeciesAwareRoute, "pure Sitka stand under model 2");
    }

    // ---------- pure broadleaf stands and mixtures ----------

    private IEnumerator Stands()
    {
        // Same stems, same sizes, three species. Sitka stays below the onset; Beech and Oak do not.
        float onset = (float)SitkaGrowthModel.MortalityOnsetRelativeDensity;
        int n = StemCount(800f);
        var results = new Dictionary<string, AdultMortalityAccount>();
        foreach ((string label, TreeSpeciesDefinition s) in new[] { ("sitka", sitka), ("beech", beech), ("oak", oak) })
        {
            Stand(2, _ => s, n, 24f, 10f);
            results[label] = DensityStep();
            yield return null;
        }
        Pass("DENSE_PURE_SPECIES_DIFFER_BY_CLASS",
            results["sitka"].DensityPressure == 0.0 && results["beech"].DensityPressure > 0.0 && results["oak"].DensityPressure > results["beech"].DensityPressure
            && results["oak"].RelativeDensity > results["beech"].RelativeDensity && results["beech"].RelativeDensity > results["sitka"].RelativeDensity,
            $"sitkaRd={F(results["sitka"].RelativeDensity)} beechRd={F(results["beech"].RelativeDensity)} oakRd={F(results["oak"].RelativeDensity)} onset={F(onset)}");

        // Pure Beech and pure Oak really self-thin over time and are held on the maximum line; cause and year are recorded.
        foreach ((string label, TreeSpeciesDefinition s) in new[] { ("BEECH", beech), ("OAK", oak) })
        {
            Stand(2, _ => s, n, 24f, 10f);
            yield return null;
            int deadwoodBefore = manager.DeadwoodRecords.Count;
            var died = new Dictionary<string, (int year, string cause)>();
            double maxRdAfter = 0;
            for (int y = 0; y < 10; y++)
            {
                var before = Living().Select(t => t.TreeId).ToHashSet();
                e.AdvanceOneYear();
                foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    if (t.IsBiologicallyDead && before.Contains(t.TreeId) && !died.ContainsKey(t.TreeId))
                        died[t.TreeId] = ((int)typeof(ForestTree).GetField("mortalityYear", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(t), t.MortalityCause);
                maxRdAfter = Math.Max(maxRdAfter, WeightedRd(Living()));
                yield return null;
            }
            var records = manager.DeadwoodRecords.Skip(deadwoodBefore).ToList();
            Pass($"PURE_{label}_SELF_THINS", died.Count > 0 && died.Values.All(v => v.cause == "self-thinning" && v.year >= 1 && v.year <= 10),
                $"deaths={died.Count}/{n}");
            Pass($"PURE_{label}_HELD_ON_MAXIMUM_LINE", maxRdAfter <= SitkaGrowthModel.MaximumRelativeDensity + 1e-6, $"maxWeightedRdAfterMortality={F(maxRdAfter)}");
            Pass($"PURE_{label}_DEADWOOD_ONCE", records.Count == died.Count && records.Select(r => r.treeId).Distinct().Count() == records.Count,
                $"records={records.Count} deaths={died.Count}");
        }

        // Mixtures: Sitka+Oak, Sitka+Beech, Oak+Beech, all three. Weighted density exceeds the pooled Sitka-route density,
        // deaths are suppression-biased, and a repeated run is identical.
        var mixes = new Dictionary<string, Func<int, TreeSpeciesDefinition>>
        {
            ["SITKA_OAK"] = k => k % 2 == 0 ? sitka : oak,
            ["SITKA_BEECH"] = k => k % 2 == 0 ? sitka : beech,
            ["OAK_BEECH"] = k => k % 2 == 0 ? oak : beech,
            ["ALL_THREE"] = k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak
        };
        foreach (KeyValuePair<string, Func<int, TreeSpeciesDefinition>> mix in mixes)
        {
            string firstRun = null;
            double weighted = 0, pooled = 0;
            int deaths = 0;
            bool speciesAware = false;
            for (int run = 0; run < 2; run++)
            {
                Stand(2, mix.Value, n, 24f, 10f);
                yield return null;
                if (run == 0)
                {
                    AdultMortalityAccount account = DensityStep();
                    weighted = account.RelativeDensity; pooled = account.UnweightedRelativeDensity; speciesAware = account.SpeciesAwareRoute;
                    Stand(2, mix.Value, n, 24f, 10f);
                    yield return null;
                }
                for (int y = 0; y < 8; y++) { e.AdvanceOneYear(); if (run == 0) deaths += e.LastAdultMortality.Deaths; yield return null; }
                string hash = TreeStateHash();
                if (run == 0) firstRun = hash;
                else Pass($"MIX_{mix.Key}", speciesAware && weighted > pooled && deaths > 0 && hash == firstRun,
                    $"weightedRd={F(weighted)} pooledSitkaRouteRd={F(pooled)} deaths8y={deaths} replayIdentical={hash == firstRun}");
            }
        }
    }

    // ---------- onset threshold on the real controller ----------

    private IEnumerator Threshold()
    {
        // Find the stem count at which the weighted density first passes the onset, then check the controller either side.
        Func<int, TreeSpeciesDefinition> speciesOf = k => k % 2 == 0 ? sitka : oak;
        int below = 0, above = 0;
        for (int count = 20; count < 4000; count++)
        {
            if (PlannedWeightedRd(count, speciesOf, 24f, 10f) > SitkaGrowthModel.MortalityOnsetRelativeDensity) { above = count; below = count - 1; break; }
        }
        Check(above > 0, "threshold bracket found");
        Stand(2, speciesOf, below, 24f, 10f);
        AdultMortalityAccount underOnset = DensityStep();
        Stand(2, speciesOf, above, 24f, 10f);
        AdultMortalityAccount overOnset = DensityStep();
        Pass("ONSET_THRESHOLD_ON_CONTROLLER", underOnset.DensityPressure == 0.0 && underOnset.Deaths == 0 && overOnset.DensityPressure > 0.0 && overOnset.SpeciesAwareRoute,
            $"stemsBelow={below} pressure={F(underOnset.DensityPressure, "0.000000")} stemsAbove={above} pressure={F(overOnset.DensityPressure, "0.000000")}");
        yield return null;
    }

    // ---------- save / load through a dying mixed stand ----------

    private IEnumerator SaveLoad()
    {
        Stand(2, k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak, StemCount(800f), 24f, 10f);
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
        int geometry = e.StandGeometryModelVersion;
        string geometryLabel = StandGeometryModel.Label(geometry);
        bool known = TryLifecycleExpectation(geometry, out string expectNeutral, out string expectNormal, out string basis);
        // Measure growth model 1 and model 2 in THIS geometry, so equality does not depend on any pinned value.
        var byGrowth = new Dictionary<int, Dictionary<string, string>>();
        foreach (int growth in new[] { GrowthModel.SiteClassDensity, GrowthModel.SpeciesAware })
        {
            byGrowth[growth] = new Dictionary<string, string>();
            foreach (float pressure in new[] { 0f, scenarioPressure })
            {
                ForestStandScenarios.ApplyLifecycleFixture();
                e.RngModelVersion = 1; e.RegenerationModelVersion = 1; e.GrowthModelVersion = growth;
                e.Browsing.BackgroundPressure = pressure; e.Browsing.ClearProtection();
                for (int year = 0; year < 80; year++) { e.AdvanceOneYear(); if (year % 10 == 9) yield return null; }
                byGrowth[growth][pressure == 0f ? "neutral" : "normal"] = (string)hashMethod.Invoke(null, new object[] { e });
            }
        }
        e.Browsing.BackgroundPressure = scenarioPressure;
        Dictionary<string, string> m1 = byGrowth[GrowthModel.SiteClassDensity], m2 = byGrowth[GrowthModel.SpeciesAware];
        Pass("LIFECYCLE_ANCHORS_MODEL2_EQUALS_MODEL1_MEASURED", m1["neutral"] == m2["neutral"] && m1["normal"] == m2["normal"],
            $"geometry={geometryLabel} neutral model1={m1["neutral"]} model2={m2["neutral"]} normal model1={m1["normal"]} model2={m2["normal"]}");
        Pass("LIFECYCLE_ANCHORS_GROWTH2_EQUAL_GROWTH1", known && m2["neutral"] == expectNeutral && m2["normal"] == expectNormal,
            known ? $"geometry={geometryLabel} neutral={m2["neutral"]} (expect {expectNeutral}) normal={m2["normal"]} (expect {expectNormal}); basis: {basis}"
                  : $"no lifecycle expectation is defined for stand geometry {geometry}; refusing to compare it with another geometry's anchors");
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
        yield return Routing();
        yield return Stands();
        yield return Threshold();
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
