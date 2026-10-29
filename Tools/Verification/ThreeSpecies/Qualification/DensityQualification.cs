using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable Unity qualification harness for the composite density v2 rework (a7a759d). It MEASURES and ASSERTS only what the
// qualification packet asks for beyond MixedDensityVerification. It changes no production code, tunes nothing and
// imposes no forestry threshold: HARD results are exactness, determinism, routing and mechanical-bias checks; everything
// else is printed as QUAL_MEASURE for a human to judge.
//
// Copy to Assets/ForestPrototype, run with
//   -batchmode -nographics -executeMethod DensityQualification.Begin
// (CCF_STAND_GEOMETRY=0 Legacy40, 1 Enlarged80; CCF_QUAL_PHASES=comma list; CCF_DIAG_DIR=output folder), then remove the
// copy and its .meta. Saves are captured/loaded in memory, plus one disk round trip into the isolated config folder.
//
// LONGRUN also writes component-ba-<geo>.csv: one row per scenario-year giving the basal area the Sitka component and the
// generic component each removed, the combined total, and whether each fired (verification-only; no production change). Read
// it with Tools/Verification/ThreeSpecies/Qualification/component_ba.py.
public static class DensityQualification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "DensityQualification.Begin"))
            new GameObject("Density qualification").AddComponent<DensityQualificationRunner>();
    }
}

// Pure verification policy, exercised by the non-Unity self-test. No density rules or thresholds live here.
public static class DensityQualificationCoverage
{
    public const string Phases = "RATIOS,ROLLS,ANCHORS,SITKA100,SITKACOMP,PARITY,CURVE,FAIRNESS,FAIRNESSHIGH,CONTINUITY,FINALBROADLEAF,LATEMIX,SAVELOAD,PERF,LONGRUN";
    public const string Selectors = "A,B,C,D,E,F,G,C1,G1";

    public static string[] SelectPhases(string filter)
    {
        string[] selected = (filter ?? Phases).Split(',').Select(token => token.Trim().ToUpperInvariant()).ToArray();
        if (selected.Any(token => !Phases.Split(',').Contains(token)))
            throw new InvalidOperationException("Unknown CCF_QUAL_PHASES token in [" + filter + "]. Valid phases: " + Phases);
        return selected;
    }

    public static string[] ScenarioTokens(string filter)
    {
        if (filter == null) return Selectors.Split(',');
        string[] tokens = filter.Split(',').Select(token => token.Trim()).ToArray();
        if (tokens.Any(token => !Selectors.Split(',').Contains(token)))
            throw new InvalidOperationException("CCF_QUAL_SCENARIOS filter selected nothing for an unknown token in [" + filter
                + "]. Use short selectors: " + Selectors);
        return tokens;
    }

    public static string[] SelectScenarios(string[] available, string filter)
    {
        string[] tokens = ScenarioTokens(filter);
        string[] selected = available.Where(name => tokens.Any(token => name.StartsWith(token + "_", StringComparison.Ordinal))).ToArray();
        if (selected.Length == 0)
            throw new InvalidOperationException("CCF_QUAL_SCENARIOS filter selected nothing.");
        return selected;
    }

    public static void RequireCoverage(int hardChecks, bool measurementOnly, int rows, int expectedRows)
    {
        if (measurementOnly)
        {
            if (expectedRows <= 0 || rows != expectedRows)
                throw new InvalidOperationException("Measurement filter selected nothing or produced incomplete rows: " + rows + "/" + expectedRows);
        }
        else if (hardChecks <= 0)
            throw new InvalidOperationException("No hard checks ran (hardChecks=0) for a check-bearing phase; the phase/filter may have selected nothing.");
    }
}

public sealed class DensityQualificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string Growth1Neutral = "7E57B9DAEF5BF4D0", Growth1Normal = "35E2BF1C2F55C2DE";

    private ForestEcologyController e;
    private ScenarioOneManager manager;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private ForestSaveData original;
    private TreeSpeciesDefinition sitka, oak, beech;
    private string geo, outDir;
    private readonly List<string> hardFailures = new List<string>();
    private int hardPassed, longRunRows, expectedLongRunRows, perfRecords, expectedPerfRecords;
    private bool measurementOnly;

    private static string F(double v, string f = "0.####") => v.ToString(f, Inv);
    private static long Bits(double v) => BitConverter.DoubleToInt64Bits(v);

    private void Hard(string name, bool ok, string detail)
    {
        if (ok) hardPassed++; else hardFailures.Add(name + " " + detail);
        Debug.Log($"QUAL_{(ok ? "PASS" : "FAIL")} {geo} {name} {detail}");
    }
    private void Measure(string name, string detail) => Debug.Log($"QUAL_MEASURE {geo} {name} {detail}");
    private void Csv(string file, string line) => File.AppendAllText(Path.Combine(outDir, file), line + "\n");

    private ForestSaveData Clone(ForestSaveData d) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));
    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    private string TreeStateHash()
    {
        ulong hash = 14695981039346656037UL;
        void Bytes(byte[] values) { foreach (byte value in values) { hash ^= value; hash *= 1099511628211UL; } }
        void Text(string value) { Bytes(Encoding.UTF8.GetBytes(value ?? "")); Bytes(new byte[1]); }
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).OrderBy(t => t.TreeId, StringComparer.Ordinal))
        {
            Text(t.TreeId); Text(t.IsLiving ? "L" : "D"); Text(t.MortalityCause);
            Bytes(BitConverter.GetBytes(t.Diameter)); Bytes(BitConverter.GetBytes(t.Height)); Bytes(BitConverter.GetBytes(t.AgeYears));
        }
        Text("YEAR:" + e.EcologicalYear);
        return hash.ToString("X16");
    }

    private static string Fnv(IEnumerable<string> lines)
    {
        ulong hash = 14695981039346656037UL;
        foreach (string line in lines)
        {
            foreach (byte b in Encoding.UTF8.GetBytes(line)) { hash ^= b; hash *= 1099511628211UL; }
            hash ^= 10; hash *= 1099511628211UL;
        }
        return hash.ToString("X16");
    }

    // ---------- fixtures (the same construction as MixedDensityVerification, so the two gates describe the same stands) ----------

    private float DbhOf(int k, float dbhMin, float spread) => dbhMin + ((k * 7919) % 100) / 100f * spread;
    private int StemCount(float stemsPerHectare) => Mathf.Max(1, Mathf.RoundToInt(stemsPerHectare * e.StandAreaHectares));

    // neutralIds gives every stem the same id prefix, so the per-tree roll (a hash of the id) and the sorted order the
    // competition index sums in cannot depend on species. Fairness tests need that; the other fixtures keep the gate's S/B/O prefixes.
    private void Stand(int growth, Func<int, TreeSpeciesDefinition> speciesOf, int count, float dbhMin, float spread, int? seed = null, bool neutralIds = false)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = growth;
        if (seed.HasValue) e.SimulationSeed = seed.Value;
        Rect bounds = e.StandBounds;
        int perAxis = Mathf.CeilToInt(Mathf.Sqrt(count));
        float spacing = (bounds.width - 2f) / perAxis;
        for (int k = 0; k < count; k++)
        {
            TreeSpeciesDefinition s = speciesOf(k);
            float d = DbhOf(k, dbhMin, spread);
            string prefix = neutralIds ? "T" : s == sitka ? "S" : s == oak ? "O" : "B";
            var position = new Vector3(bounds.xMin + 1f + (k / perAxis + 0.5f) * spacing, 0f, bounds.yMin + 1f + (k % perAxis + 0.5f) * spacing);
            spawner.Spawn(prefix + k.ToString("0000"), s, position, 40, d, 2f + 0.62f * d, s.PotentialCrownRadiusM(d));
        }
        e.InvalidateCompetition();
        e.RecomputeCanopy();
    }

    private double PlannedOccupancy(int n, Func<int, TreeSpeciesDefinition> speciesOf, float dbhMin, float spread)
    {
        var stems = Enumerable.Range(0, n).Select(k => new DensityStem("T" + k.ToString("0000"), speciesOf(k).GrowingSpaceDemand, DbhOf(k, dbhMin, spread), 0.0)).ToList();
        return MixedDensityModel.Occupancy(stems, e.StandAreaHectares);
    }

    private int CountForOccupancy(double target, Func<int, TreeSpeciesDefinition> speciesOf, float dbhMin, float spread)
    {
        for (int c = 10; c < 8000; c++)
            if (PlannedOccupancy(c, speciesOf, dbhMin, spread) >= target) return c;
        return 8000;
    }

    private AdultMortalityAccount DensityStep()
    {
        ForestTree first = Living().FirstOrDefault();
        if (first != null) e.GetCompetitionIndex(first);
        typeof(ForestEcologyController).GetMethod("ApplyAdultDensityMortality", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(e, null);
        return e.LastAdultMortality;
    }

    private List<DensityStem> StemsOfLiving()
    {
        ForestTree first = Living().FirstOrDefault();
        if (first != null) e.GetCompetitionIndex(first);
        return Living().Select(t => new DensityStem(t.TreeId, t.Species.GrowingSpaceDemand, t.Diameter, e.GetCompetitionIndex(t))).ToList();
    }

    // The v3 controller route, rebuilt from the same inputs and the same keyed rolls the controller uses: the Sitka component first
    // (original Sitka process on the Sitka stems alone), then the generic component among the stems it left standing.
    private DensityComponentPlan ComponentPlan(List<DensityStem> list)
    {
        Func<string, double> sitkaRoll = id => SimulationRandom.Roll(e.RngModelVersion, "ADULT-MORT-" + id, e.EcologicalYear, e.SimulationSeed);
        Func<string, double> genericRoll = id => SimulationRandom.Roll(e.RngModelVersion, "ADULT-MORT-GENERIC-" + id, e.EcologicalYear, e.SimulationSeed);
        SitkaComponentResult sitka = SitkaSubsetDensitySignal.Resolve(list, e.StandAreaHectares, sitkaRoll);
        return MixedDensityModel.PlanComponents(list, e.StandAreaHectares, sitka.HazardVictimIndices, sitka.BoundaryVictimIndices, genericRoll);
    }

    // Sitka-component deaths of a step: the whole step on the legacy route (every stem is Sitka), the component's own victims otherwise.
    private static int SitkaDeaths(AdultMortalityAccount a) => a.Route == DensityRoute.LegacySitkaExact ? a.Deaths : a.SitkaComponentDeaths;

    private double BasalAreaPerHectare(IEnumerable<ForestTree> trees)
    {
        double sum = 0.0;
        foreach (ForestTree t in trees) sum += Math.PI / 4.0 * (t.Diameter / 100.0) * (t.Diameter / 100.0);
        return sum / e.StandAreaHectares;
    }

    // Plain cross-section at breast height of one stem, in square metres (no class weight, no per-hectare).
    private static double StemBaM2(ForestTree t)
    {
        double dbhM = t.Diameter / 100.0;
        return Math.PI / 4.0 * dbhM * dbhM;
    }

    private double OccupancyOfTrees(IEnumerable<ForestTree> trees)
        => MixedDensityModel.Occupancy(trees.Select(t => new DensityStem(t.TreeId, t.Species.GrowingSpaceDemand, t.Diameter, 0.0)).ToList(), e.StandAreaHectares);

    // ---------- RATIOS: class contributions, and no mechanism other than plain addition ----------

    private IEnumerator Ratios()
    {
        var sb = new StringBuilder();
        bool exact = true;
        foreach (float dbh in new[] { 5f, 12f, 25f, 50f, 80f })
        {
            double r = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", sitka.GrowingSpaceDemand, dbh, 0.0));
            double b = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", beech.GrowingSpaceDemand, dbh, 0.0));
            double o = MixedDensityModel.StemOccupiedSpace(new DensityStem("a", oak.GrowingSpaceDemand, dbh, 0.0));
            exact &= r < b && b < o && Math.Abs(b / r - 1.5) < 1e-12 && Math.Abs(o / r - 2.0) < 1e-12;
            sb.Append($"dbh{dbh}:{F(r, "0.000000")}/{F(b, "0.000000")}/{F(o, "0.000000")} ");
        }
        Hard("CLASS_RATIOS_REFERENCE_BEECH_OAK_1.0_1.5_2.0", exact, sb.ToString().Trim());

        // The model surface contains no mixture mechanism, and no species type reaches the pressure/loss/victim formulas.
        var tokens = new HashSet<string>();
        foreach (MemberInfo m in typeof(MixedDensityModel).GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly))
            foreach (Match t in Regex.Matches(m.Name, "[A-Z]+(?![a-z])|[A-Z]?[a-z]+|[0-9]+")) tokens.Add(t.Value.ToLowerInvariant());
        string[] banned = { "dec", "dmc", "bonus", "productivity", "overyield", "overyielding", "mixture", "synergy", "facilitation" };
        string[] present = banned.Where(tokens.Contains).ToArray();
        Hard("NO_DEC_DMC_OR_MIXTURE_BONUS_MEMBER", present.Length == 0, $"members={tokens.Count} bannedTokensPresent=[{string.Join(",", present)}]");

        bool numbersOnly = true;
        var offenders = new List<string>();
        foreach (string name in new[] { "OvercrowdingPressure", "ExpectedAnnualLoss", "Suppression", "Vulnerability", "AnnualLossProbability" })
            foreach (MethodInfo m in typeof(MixedDensityModel).GetMethods(BindingFlags.Public | BindingFlags.Static).Where(x => x.Name == name))
                foreach (ParameterInfo p in m.GetParameters())
                    if (p.ParameterType != typeof(double) && p.ParameterType != typeof(OccupancyCalibration)) { numbersOnly = false; offenders.Add(name + ":" + p.ParameterType.Name); }
        bool stemHasNoSpecies = typeof(DensityStem).GetFields(BindingFlags.Public | BindingFlags.Instance).All(f => !f.Name.ToLowerInvariant().Contains("species") && f.FieldType != typeof(TreeSpeciesDefinition));
        Hard("PRESSURE_LOSS_VICTIM_FORMULAS_TAKE_NUMBERS_ONLY", numbersOnly && stemHasNoSpecies, $"stemHasNoSpecies={stemHasNoSpecies} offenders=[{string.Join(",", offenders)}]");
        yield return null;
    }

    // ---------- ANCHORS: the lifecycle fixture under model 1 and model 2 in THIS geometry ----------

    private IEnumerator Anchors()
    {
        MethodInfo hashMethod = typeof(ScenarioOneInteractionGate).GetMethod("LifecycleHash", BindingFlags.Static | BindingFlags.NonPublic);
        float scenarioPressure = manager.Definition.BackgroundBrowsePressure;
        var results = new Dictionary<string, string>();
        foreach (int model in new[] { 1, 2 })
            foreach (float pressure in new[] { 0f, scenarioPressure })
            {
                ForestStandScenarios.ApplyLifecycleFixture();
                e.RngModelVersion = 1; e.RegenerationModelVersion = 1; e.GrowthModelVersion = model;
                e.Browsing.BackgroundPressure = pressure; e.Browsing.ClearProtection();
                for (int year = 0; year < 80; year++) { e.AdvanceOneYear(); if (year % 10 == 9) yield return null; }
                results[$"m{model}-{(pressure == 0f ? "neutral" : "normal")}"] = (string)hashMethod.Invoke(null, new object[] { e });
            }
        e.Browsing.BackgroundPressure = scenarioPressure;
        Hard("LIFECYCLE_80Y_MODEL2_EQUALS_MODEL1_IN_THIS_GEOMETRY", results["m1-neutral"] == results["m2-neutral"] && results["m1-normal"] == results["m2-normal"],
            $"model1 neutral={results["m1-neutral"]} normal={results["m1-normal"]} | model2 neutral={results["m2-neutral"]} normal={results["m2-normal"]}");
        if (geo == "L40")
            Hard("LIFECYCLE_ANCHORS_7E57_35E2_LEGACY40", results["m1-neutral"] == Growth1Neutral && results["m1-normal"] == Growth1Normal
                && results["m2-neutral"] == Growth1Neutral && results["m2-normal"] == Growth1Normal, $"expect {Growth1Neutral}/{Growth1Normal}");
        else
            Measure("LIFECYCLE_ENLARGED80_VALUES_ARE_NOT_LEGACY40_CONSTANTS", $"geometry cells={e.Cells.Length} (hash covers every cell, so it cannot equal the Legacy40 constants) neutral={results["m1-neutral"]} normal={results["m1-normal"]}");
    }

    // ---------- SITKA100: pure Sitka, model 1 against model 2, every year for 100 years ----------

    private IEnumerator Sitka100()
    {
        var fixtures = new (string label, float perHa, float dbhMin, float spread)[] { ("DENSE", 1200f, 24f, 10f), ("MODERATE", 600f, 18f, 8f) };
        foreach (var fx in fixtures)
        {
            var rows = new Dictionary<int, List<string>> { [1] = new List<string>(), [2] = new List<string>() };
            var victims = new Dictionary<int, List<string>> { [1] = new List<string>(), [2] = new List<string>() };
            var hashes = new Dictionary<int, string>();
            var deathYears = new List<int>();
            int hazard = 0, boundary = 0, deaths = 0;
            bool legacyOnly = true;
            foreach (int growth in new[] { 1, 2 })
            {
                Stand(growth, _ => sitka, StemCount(fx.perHa), fx.dbhMin, fx.spread);
                yield return null;
                for (int y = 0; y < 100; y++)
                {
                    HashSet<string> before = Living().Select(t => t.TreeId).ToHashSet();
                    e.AdvanceOneYear();
                    AdultMortalityAccount a = e.LastAdultMortality;
                    rows[growth].Add($"{a.Route}|{Bits(a.RelativeDensity):X16}|{Bits(a.DensityPressure):X16}|{Bits(a.StemsPerHectare):X16}|{Bits(a.QuadraticMeanDbhCm):X16}|{a.HazardDeaths}|{a.BoundaryDeaths}|{a.Deaths}|{a.LivingBefore}");
                    victims[growth].Add(string.Join(",", before.Except(Living().Select(t => t.TreeId)).OrderBy(id => id, StringComparer.Ordinal)));
                    if (growth == 2)
                    {
                        legacyOnly &= a.Route == DensityRoute.LegacySitkaExact && a.Occupancy == 0.0;
                        hazard += a.HazardDeaths; boundary += a.BoundaryDeaths; deaths += a.Deaths;
                        if (a.Deaths > 0) deathYears.Add(y + 1);
                    }
                    if (y % 10 == 9) yield return null;
                }
                hashes[growth] = TreeStateHash();
            }
            bool rowsEqual = rows[1].SequenceEqual(rows[2]), victimsEqual = victims[1].SequenceEqual(victims[2]);
            Hard($"PURE_SITKA_{fx.label}_100Y_MODEL2_EXACTLY_EQUALS_MODEL1", rowsEqual && victimsEqual && hashes[1] == hashes[2] && legacyOnly,
                $"stems={StemCount(fx.perHa)} rowsEqual={rowsEqual} victimIdsEqual={victimsEqual} hash1={hashes[1]} hash2={hashes[2]} legacyRouteEveryYear={legacyOnly} rowHash={Fnv(rows[2])}");
            Measure($"PURE_SITKA_{fx.label}_MORTALITY", $"deathYears={deathYears.Count} first={(deathYears.Count > 0 ? deathYears[0] : -1)} totalDeaths={deaths} hazard={hazard} boundary={boundary} years=[{string.Join(",", deathYears.Take(60))}]");
        }
    }

    // ---------- PARITY: a stand below the onset grows identically under model 1 and model 2 (no mixture productivity effect) ----------

    // ---------- SITKACOMP: the Sitka component is the original controller block, checked live ----------
    // On a pure-Sitka stand under growth model 2 the controller runs the ORIGINAL block (legacy route). SitkaSubsetDensitySignal.Resolve, given the very same
    // stems, competition indices and keyed rolls, must name exactly the trees that block kills. This is the controller-level twin of the offline literal-port check.

    private IEnumerator SitkaComp()
    {
        var fixtures = new (float perHa, float dbhMin, float spread)[] { (1200f, 24f, 10f), (900f, 30f, 12f), (1500f, 16f, 8f), (700f, 40f, 15f) };
        int trials = 0, same = 0, deaths = 0, boundary = 0, legacyRoute = 0;
        foreach (var fx in fixtures)
            for (int sd = 0; sd < 6; sd++)
            {
                Stand(2, _ => sitka, StemCount(fx.perHa), fx.dbhMin, fx.spread, 15000 + sd);
                List<DensityStem> list = StemsOfLiving();
                Func<string, double> roll = id => SimulationRandom.Roll(e.RngModelVersion, "ADULT-MORT-" + id, e.EcologicalYear, e.SimulationSeed);
                SitkaComponentResult resolved = SitkaSubsetDensitySignal.Resolve(list, e.StandAreaHectares, roll);
                var predicted = new HashSet<string>(resolved.HazardVictimIndices.Concat(resolved.BoundaryVictimIndices).Select(i => list[i].TreeId), StringComparer.Ordinal);
                AdultMortalityAccount a = DensityStep();
                var actual = new HashSet<string>(FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(x => x.IsBiologicallyDead).Select(x => x.TreeId), StringComparer.Ordinal);
                trials++;
                if (predicted.SetEquals(actual)) same++;
                if (a.Route == DensityRoute.LegacySitkaExact) legacyRoute++;
                deaths += actual.Count; boundary += a.BoundaryDeaths;
                yield return null;
            }
        Hard("SITKACOMP_COMPONENT_VICTIMS_EQUAL_THE_ORIGINAL_CONTROLLER_BLOCK_ON_A_PURE_SITKA_STAND", same == trials && legacyRoute == trials && deaths > 0 && boundary > 0,
            $"stands={trials} identicalVictimSets={same} legacyRoute={legacyRoute} victims={deaths} boundaryDeaths={boundary}");
    }

    private IEnumerator Parity()
    {
        Func<int, TreeSpeciesDefinition> mix = k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak;
        var hash = new Dictionary<int, string>();
        var rowsByModel = new Dictionary<int, List<string>> { [1] = new List<string>(), [2] = new List<string>() };
        double maxOccupancy = 0.0; int deaths1 = 0, deaths2 = 0;
        foreach (int growth in new[] { 1, 2 })
        {
            Stand(growth, mix, StemCount(150f), 20f, 10f);
            yield return null;
            for (int y = 0; y < 30; y++)
            {
                e.AdvanceOneYear();
                AdultMortalityAccount a = e.LastAdultMortality;
                rowsByModel[growth].Add($"{a.Deaths}|{a.LivingBefore}");
                if (growth == 1) deaths1 += a.Deaths; else { deaths2 += a.Deaths; maxOccupancy = Math.Max(maxOccupancy, OccupancyOfTrees(Living())); }
                if (y % 10 == 9) yield return null;
            }
            hash[growth] = TreeStateHash();
        }
        Hard("SPARSE_MIXED_STAND_GROWS_IDENTICALLY_UNDER_MODEL1_AND_MODEL2", hash[1] == hash[2] && deaths1 == 0 && deaths2 == 0 && maxOccupancy < 1.0,
            $"hash1={hash[1]} hash2={hash[2]} densityDeaths1={deaths1} densityDeaths2={deaths2} maxOccupancy={F(maxOccupancy)} (growth, height and age are bit-identical, so the occupancy route adds no growth or productivity effect)");
    }

    // ---------- CURVE: annual loss against occupancy, onset included, on the real controller ----------

    private IEnumerator Curve()
    {
        var designs = new (string label, Func<int, TreeSpeciesDefinition> speciesOf)[]
        {
            ("PURE_OAK", _ => oak), ("PURE_BEECH", _ => beech), ("MIXED_THREE", k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak)
        };
        const int seeds = 12;
        foreach (var design in designs)
            foreach (double target in new[] { 0.9, 1.0, 1.05, 1.2, 1.5, 2.0, 3.0 })
            {
                // target 1.0 means "just below the onset": the first stem count that reaches 1.0 is slightly above it.
                int count = CountForOccupancy(target, design.speciesOf, 24f, 10f) - (target == 1.0 ? 1 : 0);
                double occ = 0, expectedSum = 0, varSum = 0, observed = 0, stems = 0, pressure = 0, planExpected = 0;
                for (int s = 0; s < seeds; s++)
                {
                    Stand(2, design.speciesOf, count, 24f, 10f, 5000 + s);
                    List<DensityStem> list = StemsOfLiving();
                    DensityComponentPlan plan = ComponentPlan(list);
                    planExpected += plan.GenericLossFraction;
                    // The generic component draws among the stems the Sitka component left standing: expected loss per eligible stem.
                    var sitkaVictims = new HashSet<int>(plan.SitkaVictimIndices);
                    var eligible = list.Where((x, i) => !sitkaVictims.Contains(i)).ToList();
                    double meanVuln = eligible.Count == 0 ? 0.0 : eligible.Select(x => MixedDensityModel.Vulnerability(MixedDensityModel.Suppression(x.CompetitionIndex))).Average();
                    foreach (DensityStem x in eligible)
                    {
                        double p = MixedDensityModel.AnnualLossProbability(plan.GenericLossFraction, MixedDensityModel.Suppression(x.CompetitionIndex), meanVuln);
                        expectedSum += p; varSum += p * (1.0 - p);
                    }
                    AdultMortalityAccount a = DensityStep();
                    occ = a.Occupancy; pressure = a.DensityPressure;
                    observed += a.GenericDeaths; stems += eligible.Count;
                    if (a.Deaths != plan.VictimIndices.Count) Hard($"CURVE_{design.label}_{F(target)}_CONTROLLER_MATCHES_PURE_PLAN", false, $"seed={s} controller={a.Deaths} plan={plan.VictimIndices.Count}");
                    if (a.Occupancy <= MixedDensityModel.OnsetOccupancy && a.Deaths > 0)
                        Hard($"CURVE_{design.label}_{F(target)}_NO_DEATHS_AT_OR_BELOW_ONSET", false, $"seed={s} occupancy={F(a.Occupancy)} deaths={a.Deaths}");
                }
                double z = varSum > 0 ? (observed - expectedSum) / Math.Sqrt(varSum) : 0.0;
                Measure($"CURVE_{design.label}", $"targetOccupancy={F(target)} stems={count} occupancy={F(occ)} pressure={F(pressure, "0.0000")} modelExpectedLossFraction={F(planExpected / seeds, "0.0000")} observedLossFraction={F(observed / Math.Max(1.0, stems), "0.0000")} expectedFromProbabilities={F(expectedSum / Math.Max(1.0, stems), "0.0000")} z={F(z, "0.00")} seeds={seeds}");
                Csv($"curve-{geo}.csv", $"{design.label},{target},{count},{F(occ, "0.0000")},{F(pressure, "0.0000")},{F(observed / Math.Max(1.0, stems), "0.0000")},{F(expectedSum / Math.Max(1.0, stems), "0.0000")},{F(z, "0.00")}");
                yield return null;
            }
    }

    // ---------- FAIRNESS: no species-driven victim selection ----------

    // Independent specification oracle: never reads the allocator's quota, budget counters or chosen set.
    // Species/demand affects pressure upstream, but NOT the conditional allocation below. This is a greedy
    // priority-maximal set up to quota, not a maximum-cardinality knapsack (which is not the production contract).
    private sealed class AllocationOracle
    {
        public readonly List<int> Priority = new List<int>();
        public readonly HashSet<int> Chosen = new HashSet<int>();
        public int Quota, PassedOver;
        public double ExpectedCount, RoundingVariance, Budget;
        public bool GuaranteedNonBinding, Fallback;
    }

    private static double OracleBa(DensityStem s) => Math.PI / 4.0 * (s.DbhCm / 100.0) * (s.DbhCm / 100.0);

    private static AllocationOracle AllocationExpectation(IReadOnlyList<DensityStem> stems, float area,
        ICollection<int> sitkaTaken, Func<string, double> roll)
    {
        var result = new AllocationOracle();
        var canonical = Enumerable.Range(0, stems.Count).OrderBy(i => stems[i].TreeId, StringComparer.Ordinal).ToArray();
        double occupied = 0, nonReference = 0, ba = 0;
        foreach (int i in canonical)
        {
            double weight = stems[i].Demand == GrowingSpaceDemandClass.Reference ? MixedDensityModel.ReferenceWeight :
                stems[i].Demand == GrowingSpaceDemandClass.Intermediate ? MixedDensityModel.IntermediateWeight : MixedDensityModel.HighWeight;
            double space = weight * OracleBa(stems[i]);
            ba += OracleBa(stems[i]); occupied += space;
            if (stems[i].Demand != GrowingSpaceDemandClass.Reference) nonReference += space;
            if (!sitkaTaken.Contains(i)) result.Priority.Add(i);
        }
        double excess = Math.Max(0, occupied / area / MixedDensityModel.OccupancyNormalization - MixedDensityModel.OnsetOccupancy);
        double loss = MixedDensityModel.GenericMaximumAnnualLoss *
            (1 - Math.Exp(-MixedDensityModel.GenericResponseStrength * Math.Pow(excess, MixedDensityModel.GenericResponseExponent)));
        result.ExpectedCount = loss * (occupied > 0 ? nonReference / occupied : 0) * result.Priority.Count;
        int floor = (int)Math.Floor(result.ExpectedCount + 1e-10);
        double remainder = Math.Max(0, result.ExpectedCount - floor);
        result.RoundingVariance = remainder * (1 - remainder);
        result.Quota = Math.Min(result.Priority.Count, floor + (remainder > 0 && roll("~generic-quota") < remainder ? 1 : 0));
        result.Budget = MixedDensityModel.GenericAnnualBasalAreaBudgetShare * ba;
        // Decide applicability BEFORE observing any roll or victim. Filtering on realised budget binding
        // conditions on the outcome and can itself bias a count test.
        result.GuaranteedNonBinding = result.Priority.Select(i => OracleBa(stems[i])).OrderByDescending(v => v)
            .Take(Math.Min(result.Priority.Count, (int)Math.Ceiling(result.ExpectedCount))).Sum() <= result.Budget + 1e-12;
        var weights = result.Priority.ToDictionary(i => i, i => Math.Pow(
            1 - 1 / (1 + Math.Max(0, stems[i].CompetitionIndex) / MixedDensityModel.NeutralHalfSaturationCi), MixedDensityModel.VulnerabilityExponent));
        bool anyWeight = weights.Values.Any(w => w > 0);
        var keys = result.Priority.ToDictionary(i => i, i => {
            double w = anyWeight ? weights[i] : 1;
            return w > 0 ? -Math.Log(Math.Max(1e-15, Math.Min(1 - 1e-15, roll(stems[i].TreeId)))) / w : double.PositiveInfinity;
        });
        result.Priority.Sort((a, b) => keys[a] != keys[b] ? keys[a].CompareTo(keys[b]) : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId));
        double remaining = result.Budget;
        foreach (int i in result.Priority)
        {
            if (result.Chosen.Count == result.Quota) break;
            double cost = OracleBa(stems[i]);
            if (cost > remaining + 1e-12) { result.PassedOver++; continue; }
            result.Chosen.Add(i); remaining -= cost;
        }
        if (result.Quota > 0 && result.Chosen.Count == 0)
        {
            result.Fallback = true;
            result.Chosen.Add(result.Priority[0]);
        }
        return result;
    }

    private static bool AllocationValid(IReadOnlyList<DensityStem> stems, AllocationOracle oracle, IEnumerable<int> victims)
    {
        int[] array = victims.ToArray();
        var chosen = new HashSet<int>(array);
        if (chosen.Count != array.Length || !chosen.SetEquals(oracle.Chosen)) return false;
        double spent = chosen.Sum(i => OracleBa(stems[i]));
        if (oracle.Fallback)
            return chosen.Count == 1 && chosen.Contains(oracle.Priority[0]) && oracle.Priority.All(i => OracleBa(stems[i]) > oracle.Budget + 1e-12);
        return spent <= oracle.Budget + 1e-12 && chosen.Count <= oracle.Quota &&
            (chosen.Count == oracle.Quota || oracle.Priority.Where(i => !chosen.Contains(i)).All(i => spent + OracleBa(stems[i]) > oracle.Budget + 1e-12));
    }

    private static string CountVerdict(int trials, double observed, double expected, double variance)
    {
        if (trials < 30 || variance <= 0) return "NOT_APPLICABLE";
        return Math.Abs((observed - expected) / Math.Sqrt(variance)) < 4 ? "PASS" : "FAIL";
    }

    private void FairnessOracleSelfTests()
    {
        var stems = Enumerable.Range(0, 16).Select(i => new DensityStem("T" + i.ToString("00"), GrowingSpaceDemandClass.High, i == 0 ? 80 : 10, 5)).ToList();
        Func<string, double> roll = id => id == "~generic-quota" ? 0.9 : id == "T00" ? 0.999 : 0.5;
        var oracle = AllocationExpectation(stems, .001f, new int[0], roll);
        var plan = MixedDensityModel.PlanComponents(stems, .001f, new int[0], new int[0], roll);
        Hard("FAIRNESS_SELFTEST_CORRECT_ALLOCATOR", AllocationValid(stems, oracle, plan.GenericVictimIndices) && oracle.PassedOver > 0 && oracle.Chosen.Count > 1, "oversized highest-priority stem skipped; smaller fitting stems selected");
        var wrong = new HashSet<int>(oracle.Chosen);
        int removed = oracle.Priority.First(wrong.Contains), replacement = oracle.Priority.Last(i => !wrong.Contains(i) && i != 0);
        wrong.Remove(removed); wrong.Add(replacement);
        Hard("FAIRNESS_SELFTEST_BROKEN_PRIORITY_REJECTED", !AllocationValid(stems, oracle, wrong), "same count and BA, wrong competitive priority");
        Hard("FAIRNESS_SELFTEST_IGNORED_BUDGET_REJECTED", !AllocationValid(stems, oracle, oracle.Priority.Take(oracle.Quota)), "unbudgeted first-quota allocator rejected");
        Hard("FAIRNESS_SELFTEST_ALWAYS_FIRST_REJECTED", !AllocationValid(stems, oracle, new[] { 0 }), "first oversized stem is forbidden when another fits");
        Hard("FAIRNESS_SELFTEST_EMPTY_ALLOCATOR_REJECTED", !AllocationValid(stems, oracle, new int[0]), "nonempty valid allocation required");
        var sparse = stems.Take(3).Select(s => new DensityStem(s.TreeId, s.Demand, 30, 5)).ToList();
        var sparseOracle = AllocationExpectation(sparse, .001f, new int[0], roll);
        var sparsePlan = MixedDensityModel.PlanComponents(sparse, .001f, new int[0], new int[0], roll);
        Hard("FAIRNESS_SELFTEST_NEVER_STALL_FALLBACK", sparseOracle.Fallback && AllocationValid(sparse, sparseOracle, sparsePlan.GenericVictimIndices), "only allowed budget overrun: no eligible stem fits alone");
        Hard("FAIRNESS_SELFTEST_ZERO_SAMPLES_NOT_GREEN", CountVerdict(0, 0, 0, 0) == "NOT_APPLICABLE" && CountVerdict(29, 0, 0, 1) == "NOT_APPLICABLE" && CountVerdict(150, 0, 0, 0) == "NOT_APPLICABLE", "zero/too few samples and zero variance cannot report statistical PASS");
        Hard("FAIRNESS_SELFTEST_COUNT_DETECTS_BIAS", CountVerdict(150, 200, 100, 10) == "FAIL" && CountVerdict(150, 100, 100, 10) == "PASS", "rounded-quota count test has live positive and negative controls");
    }

    private IEnumerator Fairness() { yield return Fairness(1.6, ""); }

    private IEnumerator Fairness(double targetOccupancy, string suffix)
    {
        var kinds = new[] { sitka, beech, oak };
        if (suffix == "") FairnessOracleSelfTests();
        var designs = new (string label, float dbhMin, float spread, double occupancy, int trials)[] {
            ("MATCHED_EQUAL_DBH", 30f, 0f, targetOccupancy, 150), ("UNEQUAL_DBH", 24f, 10f, targetOccupancy, 150),
            ("NON_BINDING_CONTROL", 30f, 0f, 1.05, 30) };
        foreach (var design in designs)
        {
            Func<int, TreeSpeciesDefinition> baseMap = k => kinds[k % 3];
            int trials = design.trials;
            int count = CountForOccupancy(design.occupancy, baseMap, design.dbhMin, design.spread);

            // Competition index must not depend on species: same layout, species rotated, index per position identical.
            Stand(2, k => kinds[k % 3], count, design.dbhMin, design.spread, null, true);
            var ciA = Living().ToDictionary(t => int.Parse(t.TreeId.Substring(1), Inv), t => e.GetCompetitionIndex(t));
            Stand(2, k => kinds[(k + 1) % 3], count, design.dbhMin, design.spread, null, true);
            var ciB = Living().ToDictionary(t => int.Parse(t.TreeId.Substring(1), Inv), t => e.GetCompetitionIndex(t));
            double maxDiff = ciA.Keys.Max(k => Math.Abs((double)ciA[k] - ciB[k]));
            Hard($"FAIRNESS{suffix}_{design.label}_COMPETITION_INDEX_IS_SPECIES_NEUTRAL", ciA.Count == ciB.Count && maxDiff == 0.0, $"stems={ciA.Count} maxAbsDifferenceAcrossRotatedSpecies={F(maxDiff, "0.0e+0")} (neutral ids, so any difference would come from species)");

            var obs = new int[3]; var expectedSpecies = new int[3];
            double freeObs = 0, freeExp = 0, freeVar = 0;
            int budgetBoundTrials = 0, nonBinding = 0, fallback = 0, allocationEvaluated = 0;
            bool faithful = true, allocated = true, neutral = true, positionExact = true;
            int totalVictims = 0;
            for (int t = 0; t < trials; t++)
            {
                int rotation = t % 3;
                Stand(2, k => kinds[(k + rotation) % 3], count, design.dbhMin, design.spread, 7000 + t, true);
                Dictionary<string, int> speciesIndex = Living().ToDictionary(x => x.TreeId, x => x.Species == sitka ? 0 : x.Species == beech ? 1 : 2, StringComparer.Ordinal);
                List<DensityStem> list = StemsOfLiving();
                DensityComponentPlan plan = ComponentPlan(list);
                Func<string, double> roll = id => SimulationRandom.Roll(e.RngModelVersion, "ADULT-MORT-GENERIC-" + id, e.EcologicalYear, e.SimulationSeed);
                var oracle = AllocationExpectation(list, e.StandAreaHectares, plan.SitkaVictimIndices, roll);
                allocated &= AllocationValid(list, oracle, plan.GenericVictimIndices) && plan.GenericQuota == oracle.Quota &&
                    plan.GenericBudgetPassedOver == oracle.PassedOver && plan.GenericBudgetLimited == oracle.Quota - oracle.Chosen.Count;
                allocationEvaluated++;
                // Relabel demand while preserving total class occupancy, size, competition, ids, rolls and Sitka exclusion.
                // Equal-DBH labels may be permuted freely: conditional allocation must not change at any position.
                var relabeled = list.ToList();
                var eligibleIndices = Enumerable.Range(0, list.Count).Where(i => !plan.SitkaVictimIndices.Contains(i)).ToArray();
                if (design.spread == 0 && eligibleIndices.Length > 1)
                {
                    for (int k = 0; k < eligibleIndices.Length; k++)
                    {
                        int i = eligibleIndices[k];
                        relabeled[i] = new DensityStem(list[i].TreeId, list[eligibleIndices[(k + 1) % eligibleIndices.Length]].Demand, list[i].DbhCm, list[i].CompetitionIndex);
                    }
                    var relabeledPlan = MixedDensityModel.PlanComponents(relabeled, e.StandAreaHectares, plan.SitkaVictimIndices, new int[0], roll);
                    neutral &= new HashSet<int>(relabeledPlan.GenericVictimIndices).SetEquals(plan.GenericVictimIndices);
                }
                // Input/list-position invariance: the same physical stems and id-keyed draws in reverse storage order.
                var reversed = list.AsEnumerable().Reverse().ToList();
                var reversedPlan = MixedDensityModel.PlanComponents(reversed, e.StandAreaHectares, plan.SitkaVictimIndices.Select(i => list.Count - 1 - i).ToArray(), new int[0], roll);
                neutral &= new HashSet<string>(reversedPlan.GenericVictimIndices.Select(i => reversed[i].TreeId)).SetEquals(plan.GenericVictimIndices.Select(i => list[i].TreeId));
                var predicted = new HashSet<string>(plan.VictimIndices.Select(i => list[i].TreeId), StringComparer.Ordinal);
                // v3: the Sitka component is Sitka-only by design. Species neutrality is asserted for the GENERIC component, i.e. among the
                // stems it chose from (everything the Sitka component left standing), by competitive position alone.
                var sitkaTaken = new HashSet<string>(plan.SitkaVictimIndices.Select(i => list[i].TreeId), StringComparer.Ordinal);
                DensityStep();
                var actual = new HashSet<string>(FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(x => x.IsBiologicallyDead).Select(x => x.TreeId), StringComparer.Ordinal);
                faithful &= predicted.SetEquals(actual);
                totalVictims += actual.Count - sitkaTaken.Count;
                bool budgetActed = oracle.PassedOver > 0 || oracle.Chosen.Count < oracle.Quota;
                if (budgetActed) budgetBoundTrials++;
                if (oracle.Fallback) fallback++;
                if (oracle.GuaranteedNonBinding)
                {
                    nonBinding++; freeObs += actual.Count - sitkaTaken.Count;
                    freeExp += oracle.ExpectedCount; freeVar += oracle.RoundingVariance;
                }
                foreach (int i in eligibleIndices)
                {
                    int s = speciesIndex[list[i].TreeId];
                    bool died = actual.Contains(list[i].TreeId), expected = oracle.Chosen.Contains(i);
                    if (died) obs[s]++;
                    if (expected) expectedSpecies[s]++;
                    positionExact &= died == expected;
                }
                if (t % 10 == 9) yield return null;
            }
            Hard($"FAIRNESS{suffix}_{design.label}_CONTROLLER_VICTIMS_EQUAL_THE_PURE_PLAN", faithful && totalVictims > 0, $"trials={trials} stems={count} genericVictims={totalVictims}");
            Hard($"FAIRNESS{suffix}_{design.label}_CONDITIONAL_ALLOCATION", allocated && positionExact && allocationEvaluated == trials && trials > 0,
                $"evaluated={allocationEvaluated} excluded=0 budgetBound={budgetBoundTrials} fallback={fallback} oracle=independent-rounded-quota/exponential-priority/greedy-BA-budget");
            Hard($"FAIRNESS{suffix}_{design.label}_NO_SPECIES_OR_POSITION_RESIDUAL", neutral && positionExact && obs.SequenceEqual(expectedSpecies),
                $"actualBySpecies={string.Join(",", obs)} conditionalExpectedBySpecies={string.Join(",", expectedSpecies)}; exact per-position residual=0 required; label and storage-order invariance");
            string verdict = CountVerdict(nonBinding, freeObs, freeExp, freeVar);
            string detail = $"evaluated={nonBinding} N/A={trials - nonBinding} observed={freeObs} expected={F(freeExp)} variance={F(freeVar)} oracle=rounded-quota (NOT independent per-stem Bernoulli)";
            if (verdict == "NOT_APPLICABLE")
                Debug.Log($"QUAL_NOT_APPLICABLE {geo} FAIRNESS{suffix}_{design.label}_COUNT {detail}; too few preclassified non-binding trials or zero variance; no z-score");
            else Hard($"FAIRNESS{suffix}_{design.label}_COUNT", verdict == "PASS", detail + $" z={F((freeObs - freeExp) / Math.Sqrt(freeVar), "0.00")}");
            if (design.label == "NON_BINDING_CONTROL")
                Hard($"FAIRNESS{suffix}_NON_BINDING_CONTROL_COVERAGE", nonBinding == trials && verdict == "PASS", $"required={trials} evaluated={nonBinding} statisticalVerdict={verdict}");
        }

        if (suffix != "") yield break;
        // The same selection inside a real annual step: victims should be the more competitively suppressed stems.
        double victimCi = 0, survivorCi = 0, victimN = 0, survivorN = 0, aucNum = 0, aucDen = 0;
        for (int t = 0; t < 12; t++)
        {
            Stand(2, k => kinds[k % 3], StemCount(1300f), 24f, 10f, 9000 + t);
            ForestTree[] before = Living();
            var ci = before.ToDictionary(x => x.TreeId, x => (double)e.GetCompetitionIndex(x), StringComparer.Ordinal);
            e.AdvanceOneYear();
            var dead = new HashSet<string>(before.Where(x => !x.IsLiving).Select(x => x.TreeId), StringComparer.Ordinal);
            var dv = dead.Select(id => ci[id]).ToList(); var sv = ci.Where(kv => !dead.Contains(kv.Key)).Select(kv => kv.Value).ToList();
            victimCi += dv.Sum(); survivorCi += sv.Sum(); victimN += dv.Count; survivorN += sv.Count;
            foreach (double a in dv) foreach (double b in sv) { aucDen++; aucNum += a > b ? 1.0 : a == b ? 0.5 : 0.0; }
            yield return null;
        }
        double auc = aucDen > 0 ? aucNum / aucDen : 0.0;
        Hard("ANNUAL_STEP_VICTIMS_ARE_THE_MORE_SUPPRESSED_STEMS", victimN > 0 && victimCi / victimN > survivorCi / survivorN && auc > 0.5,
            $"victims={victimN} meanCiVictims={F(victimCi / Math.Max(1, victimN))} meanCiSurvivors={F(survivorCi / Math.Max(1, survivorN))} auc={F(auc, "0.000")}");
    }


    // ---------- CONTINUITY / FINALBROADLEAF: the live controller across Sitka shares and at the last broadleaf ----------
    // Tolerances are the density worker's own predeclared ones (CompositeDensityV2Predeclared.md): expected loss at 99.9% Sitka within .005
    // of pure; adjacent shares within 2 x the composition change + .005; one broadleaf removed or dead within .005 and 5 deaths per 1000.
    // Realised deaths are random, so each tolerance is widened by three standard errors of the difference of the seed means.

    private static double Sd(IList<double> v)
    {
        if (v.Count < 2) return 0.0;
        double m = v.Average(); return Math.Sqrt(v.Sum(x => (x - m) * (x - m)) / (v.Count - 1));
    }

    private Func<int, TreeSpeciesDefinition> ShareOf(int count, double sitkaShare)
    {
        int broadleaf = (int)Math.Round(count * (1.0 - sitkaShare));
        return k => (int)((long)(k + 1) * broadleaf / count) > (int)((long)k * broadleaf / count) ? (k % 2 == 0 ? oak : beech) : sitka;
    }

    private IEnumerator Continuity()
    {
        const int count = 1000, seeds = 10;
        var regimes = new (string label, float dbhMin, float spread)[] { ("DBH16_24", 16f, 8f), ("DBH24_34", 24f, 10f), ("DBH32_44", 32f, 12f) };
        double[] shares = { 1.0, 0.999, 0.99, 0.95, 0.90 };
        foreach (var regime in regimes)
        {
            var fractions = new List<double>[shares.Length];
            var sitkaFractions = new List<double>[shares.Length];
            var routes = new string[shares.Length];
            for (int si = 0; si < shares.Length; si++)
            {
                fractions[si] = new List<double>(); sitkaFractions[si] = new List<double>();
                Func<int, TreeSpeciesDefinition> speciesOf = ShareOf(count, shares[si]);
                for (int sd = 0; sd < seeds; sd++)
                {
                    Stand(2, speciesOf, count, regime.dbhMin, regime.spread, 11000 + sd);
                    int broadleaf = Living().Count(t => t.Species != sitka);
                    int sitkaStems = Living().Count(t => t.Species == sitka);
                    AdultMortalityAccount a = DensityStep();
                    routes[si] = a.Route.ToString();
                    fractions[si].Add(a.LivingBefore > 0 ? (double)a.Deaths / a.LivingBefore : 0.0);
                    sitkaFractions[si].Add(sitkaStems > 0 ? (double)SitkaDeaths(a) / sitkaStems : 0.0);
                    if (sd == 0) Measure("CONTINUITY_STAND", $"{regime.label} sitkaShare={F(shares[si])} stems={a.LivingBefore} broadleaf={broadleaf} route={a.Route} occupancyOrRd={F(a.Route == DensityRoute.SpeciesAwareOccupancy ? a.Occupancy : a.RelativeDensity)} pressure={F(a.DensityPressure, "0.0000")}");
                }
                Measure("CONTINUITY", $"{regime.label} sitkaShare={F(shares[si])} route={routes[si]} meanDeathFraction={F(fractions[si].Average(), "0.0000")} sd={F(Sd(fractions[si]), "0.0000")} deathsPer1000={F(fractions[si].Average() * 1000.0, "0.0")} sitkaComponentLossPerSitkaStem={F(sitkaFractions[si].Average(), "0.0000")} sd={F(Sd(sitkaFractions[si]), "0.0000")}");
                Csv($"continuity-{geo}.csv", $"{regime.label},{shares[si]},{routes[si]},{F(fractions[si].Average(), "0.00000")},{F(Sd(fractions[si]), "0.00000")},{seeds},{F(sitkaFractions[si].Average(), "0.00000")},{F(Sd(sitkaFractions[si]), "0.00000")}");
                yield return null;
            }
            Hard($"CONTINUITY_{regime.label}_PURE_SITKA_USES_THE_LEGACY_ROUTE_AND_ANY_BROADLEAF_THE_COMPOSITE_ROUTE",
                routes[0] == "LegacySitkaExact" && routes.Skip(1).All(r => r == "SpeciesAwareOccupancy"), string.Join(",", routes));
            double se(int a, int b) => Math.Sqrt(Math.Pow(Sd(fractions[a]), 2) / seeds + Math.Pow(Sd(fractions[b]), 2) / seeds);
            double seSitka(int a, int b) => Math.Sqrt(Math.Pow(Sd(sitkaFractions[a]), 2) / seeds + Math.Pow(Sd(sitkaFractions[b]), 2) / seeds);
            double sitka999 = Math.Abs(sitkaFractions[1].Average() - sitkaFractions[0].Average());
            Hard($"CONTINUITY_{regime.label}_SITKA_COMPONENT_LOSS_PER_SITKA_STEM_AT_99_9_PERCENT_MATCHES_PURE", sitka999 <= 0.005 + 3.0 * seSitka(0, 1),
                $"difference={F(sitka999, "0.0000")} tolerance={F(0.005 + 3.0 * seSitka(0, 1), "0.0000")} (0.005 + 3 se; pure={F(sitkaFractions[0].Average(), "0.0000")} mixed={F(sitkaFractions[1].Average(), "0.0000")})");
            double d999 = Math.Abs(fractions[1].Average() - fractions[0].Average());
            Hard($"CONTINUITY_{regime.label}_99_9_PERCENT_SITKA_LOSS_MATCHES_PURE", d999 <= 0.005 + 3.0 * se(0, 1),
                $"difference={F(d999, "0.0000")} tolerance={F(0.005 + 3.0 * se(0, 1), "0.0000")} (0.005 + 3 se)");
            for (int i = 0; i + 1 < shares.Length; i++)
            {
                double d = Math.Abs(fractions[i + 1].Average() - fractions[i].Average());
                double tolerance = 2.0 * (shares[i] - shares[i + 1]) + 0.005 + 3.0 * se(i, i + 1);
                Hard($"CONTINUITY_{regime.label}_ADJACENT_{F(shares[i])}_TO_{F(shares[i + 1])}", d <= tolerance, $"difference={F(d, "0.0000")} tolerance={F(tolerance, "0.0000")} (2 x composition change + 0.005 + 3 se)");
            }
        }
    }

    private IEnumerator FinalBroadleaf()
    {
        const int count = 1000, seeds = 12;
        var regimes = new (string label, float dbhMin, float spread)[] { ("DBH16_24", 16f, 8f), ("DBH24_34", 24f, 10f), ("DBH32_44", 32f, 12f) };
        foreach (var regime in regimes)
        {
            var with = new List<double>(); var fell = new List<double>(); var natural = new List<double>(); var pure = new List<double>();
            var withDeaths = new List<double>(); var fellDeaths = new List<double>(); var naturalDeaths = new List<double>();
            var sitkaWith = new List<double>(); var sitkaFell = new List<double>(); var sitkaNatural = new List<double>();
            Func<int, TreeSpeciesDefinition> one = ShareOf(count, 0.999), none = _ => sitka;
            for (int sd = 0; sd < seeds; sd++)
            {
                int seed = 12000 + sd;
                Stand(2, none, count, regime.dbhMin, regime.spread, seed);
                AdultMortalityAccount p = DensityStep(); pure.Add((double)p.Deaths / Math.Max(1, p.LivingBefore));
                Stand(2, one, count, regime.dbhMin, regime.spread, seed);
                AdultMortalityAccount a = DensityStep(); with.Add((double)a.Deaths / Math.Max(1, a.LivingBefore)); withDeaths.Add(a.Deaths);
                sitkaWith.Add((double)SitkaDeaths(a) / Math.Max(1, a.LivingBefore - 1));
                // deliberate removal of the last broadleaf through the felling path, then the next annual density step
                Stand(2, one, count, regime.dbhMin, regime.spread, seed);
                ForestTree last = Living().First(t => t.Species != sitka);
                last.Fell(); e.InvalidateCompetition(); e.RecomputeCanopy();
                AdultMortalityAccount b = DensityStep(); fell.Add((double)b.Deaths / Math.Max(1, b.LivingBefore)); fellDeaths.Add(b.Deaths);
                sitkaFell.Add((double)SitkaDeaths(b) / Math.Max(1, b.LivingBefore));
                // natural death of the last broadleaf through the ecology's own mortality path
                Stand(2, one, count, regime.dbhMin, regime.spread, seed);
                last = Living().First(t => t.Species != sitka);
                last.ApplyMortality("self-thinning", e.EcologicalYear); e.InvalidateCompetition(); e.RecomputeCanopy();
                AdultMortalityAccount c = DensityStep(); natural.Add((double)c.Deaths / Math.Max(1, c.LivingBefore)); naturalDeaths.Add(c.Deaths);
                sitkaNatural.Add((double)SitkaDeaths(c) / Math.Max(1, c.LivingBefore));
                if (sd == 0) Measure("FINALBROADLEAF_ROUTES", $"{regime.label} withOneBroadleaf={a.Route} afterFelling={b.Route} afterNaturalDeath={c.Route} stems={a.LivingBefore}/{b.LivingBefore}/{c.LivingBefore}");
                if (sd % 3 == 2) yield return null;
            }
            double sePair(IList<double> x, IList<double> y) { var d = x.Select((v, i) => v - y[i]).ToList(); return Sd(d) / Math.Sqrt(d.Count); }
            double meanDiff(IList<double> x, IList<double> y) => x.Select((v, i) => v - y[i]).Average();
            Measure("FINALBROADLEAF", $"{regime.label} deathsPer1000: pureSitka={F(pure.Average() * 1000.0, "0.0")} oneBroadleafPresent={F(with.Average() * 1000.0, "0.0")} afterFelling={F(fell.Average() * 1000.0, "0.0")} afterNaturalDeath={F(natural.Average() * 1000.0, "0.0")} seeds={seeds}");
            Csv($"finalbroadleaf-{geo}.csv", $"{regime.label},{F(pure.Average(), "0.00000")},{F(with.Average(), "0.00000")},{F(fell.Average(), "0.00000")},{F(natural.Average(), "0.00000")},{seeds},{F(sitkaWith.Average(), "0.00000")},{F(sitkaFell.Average(), "0.00000")},{F(sitkaNatural.Average(), "0.00000")}");
            Measure("FINALBROADLEAF_SITKA_COMPONENT", $"{regime.label} Sitka-component loss per Sitka stem: oneBroadleafPresent={F(sitkaWith.Average(), "0.0000")} afterFelling={F(sitkaFell.Average(), "0.0000")} afterNaturalDeath={F(sitkaNatural.Average(), "0.0000")}");
            foreach (var pair in new[] { ("FELLING", sitkaFell), ("NATURAL_DEATH", sitkaNatural) })
            {
                double ds = Math.Abs(pair.Item2.Select((v, i) => v - sitkaWith[i]).Average()), tolS = 0.005 + 3.0 * sePair(pair.Item2, sitkaWith);
                Hard($"FINALBROADLEAF_{regime.label}_{pair.Item1}_SITKA_COMPONENT_LOSS_IS_UNCHANGED", ds <= tolS, $"sitkaLossDifference={F(ds, "0.0000")} tolerance={F(tolS, "0.0000")} (0.005 + 3 se)");
            }
            foreach (var pair in new[] { ("FELLING", fell, fellDeaths), ("NATURAL_DEATH", natural, naturalDeaths) })
            {
                double d = Math.Abs(meanDiff(pair.Item2, with)), tol = 0.005 + 3.0 * sePair(pair.Item2, with);
                double dd = Math.Abs(pair.Item3.Average() - withDeaths.Average());
                Hard($"FINALBROADLEAF_{regime.label}_{pair.Item1}_LOSS_MATCHES_THE_STAND_WITH_IT", d <= tol && dd <= 5.0 + 3.0 * 1000.0 * sePair(pair.Item2, with), $"lossDifference={F(d, "0.0000")} tolerance={F(tol, "0.0000")} deathDifferencePer1000={F(dd, "0.0")} (predeclared <= 0.005 and <= 5 per 1000, widened by 3 se)");
            }
            double dp = Math.Abs(meanDiff(with, pure)), tolp = 0.005 + 3.0 * sePair(with, pure);
            Hard($"FINALBROADLEAF_{regime.label}_ONE_BROADLEAF_VERSUS_PURE_SITKA", dp <= tolp, $"lossDifference={F(dp, "0.0000")} tolerance={F(tolp, "0.0000")}");
        }

        // The same event inside the full annual step (growth, competition, regeneration off): deaths per year around the last broadleaf's removal.
        var trajectories = new Dictionary<string, List<double>>();
        foreach (string mode in new[] { "never", "felled-year3", "died-year3" })
        {
            var deaths = new List<double>[6]; for (int i = 0; i < 6; i++) deaths[i] = new List<double>();
            for (int sd = 0; sd < 6; sd++)
            {
                Stand(2, ShareOf(count, 0.999), count, 24f, 10f, 13000 + sd);
                for (int year = 1; year <= 6; year++)
                {
                    if (year == 4 && mode != "never")
                    {
                        ForestTree last = Living().FirstOrDefault(t => t.Species != sitka);
                        if (last != null) { if (mode == "felled-year3") last.Fell(); else last.ApplyMortality("self-thinning", e.EcologicalYear); e.InvalidateCompetition(); e.RecomputeCanopy(); }
                    }
                    e.AdvanceOneYear();
                    deaths[year - 1].Add(e.LastAdultMortality.Deaths);
                }
                if (sd % 2 == 1) yield return null;
            }
            trajectories[mode] = deaths.Select(l => l.Average()).ToList();
            Measure("FINALBROADLEAF_TRAJECTORY", $"{mode} meanDensityDeathsPerYear(y1..y6)={string.Join(" ", deaths.Select(l => F(l.Average(), "0.0")))}");
        }
        double step = Math.Abs(trajectories["felled-year3"][3] - trajectories["never"][3]), stepN = Math.Abs(trajectories["died-year3"][3] - trajectories["never"][3]);
        Hard("FINALBROADLEAF_NO_NEXT_YEAR_MASS_MORTALITY_AFTER_FELLING_OR_NATURAL_DEATH", step <= 0.05 * trajectories["never"][3] + 5.0 && stepN <= 0.05 * trajectories["never"][3] + 5.0,
            $"year4 deaths: never={F(trajectories["never"][3], "0.0")} felled={F(trajectories["felled-year3"][3], "0.0")} died={F(trajectories["died-year3"][3], "0.0")} (within 5% + 5 stems)");
    }


    // ---------- LATEMIX: a large old Sitka stand at high relative density, with and without a few small broadleaves ----------
    // The long runs end in this state (Sitka BA 90-105 m2/ha, a handful of small Beech). The 1000-stem continuity fixtures are young and dense; this is
    // the other end of the parameter space. Same Sitka stems every time; only the number of small broadleaves changes (0 uses the legacy route).

    private void LateStand(int nSitka, float sitkaDbhMin, float sitkaSpread, int nBroad, int seed)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)) DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = 2;
        e.SimulationSeed = seed;
        Rect bounds = e.StandBounds;
        int total = nSitka + nBroad, perAxis = Mathf.CeilToInt(Mathf.Sqrt(total));
        float spacing = (bounds.width - 2f) / perAxis;
        int sitkaIndex = 0, broadIndex = 0;
        for (int k = 0; k < total; k++)
        {
            bool broad = (int)((long)(k + 1) * nBroad / total) > (int)((long)k * nBroad / total);
            TreeSpeciesDefinition s = broad ? (broadIndex % 2 == 0 ? beech : oak) : sitka;
            float d = broad ? 12f + ((broadIndex++ * 7919) % 100) / 100f * 8f : sitkaDbhMin + ((sitkaIndex++ * 7919) % 100) / 100f * sitkaSpread;
            string prefix = broad ? (s == oak ? "O" : "B") : "S";
            var position = new Vector3(bounds.xMin + 1f + (k / perAxis + 0.5f) * spacing, 0f, bounds.yMin + 1f + (k % perAxis + 0.5f) * spacing);
            spawner.Spawn(prefix + k.ToString("0000"), s, position, 40, d, 2f + 0.62f * d, s.PotentialCrownRadiusM(d));
        }
        e.InvalidateCompetition();
        e.RecomputeCanopy();
    }

    private IEnumerator LateMix()
    {
        var regimes = new (string label, int nSitka, float dbhMin, float spread)[] { ("OLD_SITKA_N100_DBH78_98", 100, 78f, 20f), ("MID_SITKA_N150_DBH55_70", 150, 55f, 15f) };
        int[] broadleaf = { 0, 1, 3, 12, 26 };
        const int seeds = 12;
        foreach (var regime in regimes)
        {
            var total = new List<double>[broadleaf.Length]; var sitkaLoss = new List<double>[broadleaf.Length]; var broadLoss = new List<double>[broadleaf.Length];
            var sitkaComponent = new List<double>[broadleaf.Length]; var totalDeaths = new List<double>[broadleaf.Length];
            // The stems-per-hectare of the Enlarged80 stand (0.64 ha) on any geometry: counts scale with the area (Legacy40 is 0.16 ha).
            double areaScale = e.StandAreaHectares / 0.64;
            int sitkaCount = Mathf.Max(8, Mathf.RoundToInt((float)(regime.nSitka * areaScale)));
            int[] broadCount = broadleaf.Select(b => b == 0 ? 0 : Mathf.Max(1, Mathf.RoundToInt((float)(b * areaScale)))).ToArray();
            for (int bi = 0; bi < broadleaf.Length; bi++)
            {
                total[bi] = new List<double>(); sitkaLoss[bi] = new List<double>(); broadLoss[bi] = new List<double>(); sitkaComponent[bi] = new List<double>(); totalDeaths[bi] = new List<double>();
                string route = "";
                for (int sd = 0; sd < seeds; sd++)
                {
                    LateStand(sitkaCount, regime.dbhMin, regime.spread, broadCount[bi], 14000 + sd);
                    AdultMortalityAccount a = DensityStep(); route = a.Route.ToString();
                    var dead = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(x => x.IsBiologicallyDead).ToList();
                    int sitkaDead = dead.Count(x => x.Species == sitka), broadDead = dead.Count - sitkaDead;
                    total[bi].Add((double)a.Deaths / Math.Max(1, a.LivingBefore));
                    totalDeaths[bi].Add(a.Deaths);
                    sitkaLoss[bi].Add((double)sitkaDead / sitkaCount);
                    sitkaComponent[bi].Add((double)SitkaDeaths(a) / sitkaCount);
                    if (broadCount[bi] > 0) broadLoss[bi].Add((double)broadDead / broadCount[bi]);
                }
                Measure("LATEMIX", $"{regime.label} sitkaStems={sitkaCount} broadleaves={broadCount[bi]} route={route} totalDeathFraction={F(total[bi].Average(), "0.000")} sitkaDeathFraction={F(sitkaLoss[bi].Average(), "0.000")} broadleafDeathFraction={(broadLoss[bi].Count > 0 ? F(broadLoss[bi].Average(), "0.000") : "-")} sd={F(Sd(total[bi]), "0.000")} seeds={seeds}");
                Csv($"latemix-{geo}.csv", $"{regime.label},{broadleaf[bi]},{route},{F(total[bi].Average(), "0.00000")},{F(sitkaLoss[bi].Average(), "0.00000")},{(broadLoss[bi].Count > 0 ? F(broadLoss[bi].Average(), "0.00000") : "")},{F(sitkaComponent[bi].Average(), "0.00000")},{F(totalDeaths[bi].Average(), "0.000")}");
                yield return null;
            }
            // v3 (CompositeDensityV3Predeclared.md C3, C4): the Sitka component must NOT change when small broadleaves are added (it is the original Sitka
            // process on the Sitka stems alone); the total gains at most the generic expectation, in stems. Both are HARD. The earlier (v2) fraction-based
            // total-loss continuity is kept as a measure: the fraction falls as broadleaves enlarge the denominator, the stems lost do not.
            double sePaired(IList<double> x, IList<double> y) { var d = x.Select((v, i) => v - y[i]).ToList(); return Sd(d) / Math.Sqrt(d.Count); }
            double seTotal(int a, int b) => Math.Sqrt(Math.Pow(Sd(total[a]), 2) / seeds + Math.Pow(Sd(total[b]), 2) / seeds);
            for (int bi = 1; bi < broadleaf.Length; bi++)
            {
                double dSitka = Math.Abs(sitkaComponent[bi].Average() - sitkaComponent[0].Average()), tolSitka = 0.005 + 3.0 * sePaired(sitkaComponent[bi], sitkaComponent[0]);
                Hard($"LATEMIX_{regime.label}_{broadleaf[bi]}_BROADLEAVES_LEAVE_THE_SITKA_COMPONENT_LOSS_AS_IN_THE_PURE_STAND", dSitka <= tolSitka,
                    $"sitkaComponentLossDifference={F(dSitka, "0.0000")} tolerance={F(tolSitka, "0.0000")} (0.005 + 3 se; pure={F(sitkaComponent[0].Average(), "0.000")} mixed={F(sitkaComponent[bi].Average(), "0.000")})");
                double added = totalDeaths[bi].Average() - totalDeaths[0].Average(), seAdded = sePaired(totalDeaths[bi], totalDeaths[0]);
                double upper = 0.5 + 0.005 * (sitkaCount + broadCount[bi]) + 3.0 * seAdded;
                Hard($"LATEMIX_{regime.label}_{broadleaf[bi]}_BROADLEAVES_ADD_AT_MOST_THE_GENERIC_EXPECTATION_IN_STEMS", added <= upper && added >= -3.0 * seAdded - 0.5,
                    $"addedDeathsPerYear={F(added, "0.000")} tolerance=[-{F(3.0 * seAdded + 0.5, "0.000")}, {F(upper, "0.000")}] (0.5 + 0.005 N + 3 se)");
                Measure("LATEMIX_TOTAL_FRACTION", $"{regime.label} broadleaves={broadleaf[bi]} totalDeathFraction {F(total[0].Average(), "0.000")} (pure) -> {F(total[bi].Average(), "0.000")} (mixed; diff {F(Math.Abs(total[bi].Average() - total[0].Average()), "0.0000")}, 0.005 + 3 se = {F(0.005 + 3.0 * seTotal(0, bi), "0.0000")})");
                Measure("LATEMIX_SITKA_SHIELDING", $"{regime.label} broadleaves={broadleaf[bi]} Sitka-component loss {F(sitkaComponent[0].Average(), "0.000")} (pure) -> {F(sitkaComponent[bi].Average(), "0.000")} (mixed); all Sitka stems dead {F(sitkaLoss[0].Average(), "0.000")} -> {F(sitkaLoss[bi].Average(), "0.000")} (adds the generic deaths that fall on Sitka)");
            }
        }
    }

    // ---------- ROLLS: the shared per-tree roll stream (SimulationRandom.Roll, unchanged by the branch) ----------

    private IEnumerator Rolls()
    {
        // The exact id pattern the density step uses, many seeds and years. If P(roll < p) differs from p, every density
        // result built on the stream is biased the same way for every species (it is the legacy Sitka route's stream too).
        int ids = 800, seeds = 400;
        foreach (double p in new[] { 0.01, 0.0375, 0.10, 0.5 })
        {
            long below = 0, total = 0;
            for (int s = 0; s < seeds; s++)
                for (int k = 0; k < ids; k++)
                {
                    float r = SimulationRandom.Roll(SimulationRandom.MixedModel, "ADULT-MORT-T" + k.ToString("0000"), s % 7, 20000 + s);
                    if (r < p) below++;
                    total++;
                }
            double expected = p * total, sd = Math.Sqrt(total * p * (1 - p));
            Measure("ROLLS_UNIFORMITY", $"p={F(p, "0.0000")} fractionBelow={F(below / (double)total, "0.00000")} z={F((below - expected) / sd, "0.00")} draws={total}");
            if (Math.Abs((below - expected) / sd) >= 4.0) Hard($"ROLLS_UNIFORM_AT_{F(p)}", false, $"fractionBelow={F(below / (double)total, "0.00000")}");
        }
        // Adjacent seeds should be independent for one id.
        double sumProd = 0, sumA = 0, sumB = 0; int pairs = 0;
        for (int k = 0; k < ids; k++)
            for (int s = 0; s < seeds - 1; s++)
            {
                double a = SimulationRandom.Roll(SimulationRandom.MixedModel, "ADULT-MORT-T" + k.ToString("0000"), 0, 20000 + s), b = SimulationRandom.Roll(SimulationRandom.MixedModel, "ADULT-MORT-T" + k.ToString("0000"), 0, 20001 + s);
                sumProd += a * b; sumA += a; sumB += b; pairs++;
            }
        double corr = (sumProd / pairs - (sumA / pairs) * (sumB / pairs)) / (1.0 / 12.0);
        Measure("ROLLS_ADJACENT_SEED_CORRELATION", $"corr={F(corr, "0.0000")} pairs={pairs} (independent draws give about 0 +- {F(1.0 / Math.Sqrt(pairs), "0.0000")})");
        Hard("ROLLS_ADJACENT_SEEDS_UNCORRELATED", Math.Abs(corr) < 4.0 / Math.Sqrt(pairs), $"corr={F(corr, "0.0000")}");
        yield return null;
    }

    // ---------- SAVELOAD ----------

    private static HashSet<string> JsonKeys(string json)
    {
        var set = new HashSet<string>();
        foreach (Match m in Regex.Matches(json, "\"([A-Za-z0-9_]+)\"\\s*:")) set.Add(m.Groups[1].Value);
        return set;
    }

    private IEnumerator SaveLoad()
    {
        Func<int, TreeSpeciesDefinition> mix = k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak;
        var keys = new Dictionary<int, HashSet<string>>();
        var json = new Dictionary<int, string>();
        foreach (int growth in new[] { 1, 2 })
        {
            Stand(growth, mix, StemCount(900f), 24f, 10f);
            yield return null;
            for (int y = 0; y < 3; y++) { e.AdvanceOneYear(); yield return null; }
            json[growth] = JsonUtility.ToJson(saves.CaptureData());
            keys[growth] = JsonKeys(json[growth]);
        }
        Hard("SAVE_V23_MODEL2_MIXED_ADDS_NO_PERSISTED_FIELD", ForestSaveData.CurrentVersion == 23 && keys[1].SetEquals(keys[2]) && json[2].Contains("\"version\":23") && json[2].Contains("\"growthModel\":2"),
            $"version={ForestSaveData.CurrentVersion} keys(model1)={keys[1].Count} keys(model2)={keys[2].Count} onlyIn2=[{string.Join(",", keys[2].Except(keys[1]))}] onlyIn1=[{string.Join(",", keys[1].Except(keys[2]))}]");

        // Unsupported futures are refused and refusing changes nothing.
        ForestSaveData cap = saves.CaptureData();
        string hashBefore = WorldHash();
        ForestSaveData three = Clone(cap); three.growthModel = 3;
        string problem3 = ForestSaveValidation.Validate(three, 0, e.CellCount);
        bool loaded3 = saves.LoadData(three, false);
        yield return null;
        ForestSaveData far = Clone(cap); far.growthModel = 99;
        string problem99 = ForestSaveValidation.Validate(far, 0, e.CellCount);
        bool loaded99 = saves.LoadData(far, false);
        yield return null;
        Hard("SAVE_UNSUPPORTED_GROWTH_MODELS_3_AND_99_REJECTED_WITHOUT_CHANGE", !loaded3 && problem3 != null && problem3.Contains("unknown growth model 3") && !loaded99 && problem99 != null && WorldHash() == hashBefore,
            $"model3 loaded={loaded3} problem=[{problem3}] | model99 loaded={loaded99} problem=[{problem99}] | worldUnchanged={WorldHash() == hashBefore}");
        // A newer FILE version is not refused: LoadData logs "newer than supported ... loading best-effort" (ForestSaveController, which this
        // branch does not touch). Observed here so the handoff states what actually happens; it is pre-existing behaviour, not a result of the branch.
        ForestSaveData v21 = Clone(cap); v21.version = ForestSaveData.CurrentVersion + 1;
        string problem21 = ForestSaveValidation.Validate(v21, 0, e.CellCount);
        bool loaded21 = saves.LoadData(v21, false);
        yield return null;
        Measure("SAVE_NEWER_FILE_VERSION_BEHAVIOUR", $"version{v21.version} validateProblem=[{problem21}] loaded={loaded21} (best-effort by existing design)");
        Check(saves.LoadData(Clone(cap), false), "restore after version probe"); yield return null;

        // A real disk round trip (isolated config folder), then the continuation matches an uninterrupted run.
        Check(saves.LoadData(Clone(cap), false), "reload capture"); yield return null;
        saves.Save();
        bool can = saves.CanLoad(out string canProblem);
        for (int y = 0; y < 6; y++) { e.AdvanceOneYear(); yield return null; }
        string uninterrupted = WorldHash();
        saves.Load();
        yield return null; yield return null;
        bool keptModel = e.GrowthModelVersion == GrowthModel.SpeciesAware;
        for (int y = 0; y < 6; y++) { e.AdvanceOneYear(); yield return null; }
        Hard("SAVE_DISK_ROUND_TRIP_MIXED_CONTINUES_IDENTICALLY", can && keptModel && WorldHash() == uninterrupted, $"canLoad={can} [{canProblem}] model2Kept={keptModel} uninterrupted={uninterrupted} resumed={WorldHash()}");

        // Through a heavily dying stand: snapshot at year 10, ten more years, restore, ten more years.
        Stand(2, mix, StemCount(1300f), 24f, 10f);
        yield return null;
        for (int y = 0; y < 10; y++) { e.AdvanceOneYear(); yield return null; }
        string snap = JsonUtility.ToJson(saves.CaptureData());
        var deathsA = new List<int>();
        for (int y = 0; y < 10; y++) { e.AdvanceOneYear(); deathsA.Add(e.LastAdultMortality.Deaths); yield return null; }
        string finalA = WorldHash();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(snap), false), "restore snapshot"); yield return null;
        var deathsB = new List<int>();
        for (int y = 0; y < 10; y++) { e.AdvanceOneYear(); deathsB.Add(e.LastAdultMortality.Deaths); yield return null; }
        Hard("SAVE_RESTORE_THROUGH_DYING_MIXED_STAND_REPRODUCES_MORTALITY", finalA == WorldHash() && deathsA.SequenceEqual(deathsB) && deathsA.Sum() > 0,
            $"deathsAfterRestore=[{string.Join(",", deathsB)}] uninterrupted=[{string.Join(",", deathsA)}] hashA={finalA} hashB={WorldHash()}");
    }

    private void Check(bool ok, string what) { if (!ok) Hard("PRECONDITION", false, what); }

    // ---------- PERF ----------

    private IEnumerator Perf()
    {
        var cases = new (string label, int growth, Func<int, TreeSpeciesDefinition> speciesOf, float perHa)[]
        {
            ("SITKA_MODEL1", 1, _ => sitka, 1200f), ("SITKA_MODEL2_LEGACY_ROUTE", 2, _ => sitka, 1200f),
            ("MIXED_BELOW_ONSET", 2, k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak, 250f),
            ("MIXED_DENSE", 2, k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak, 1300f),
            ("OAK_DENSE", 2, _ => oak, 1000f)
        };
        foreach (var c in cases)
        {
            Stand(c.growth, c.speciesOf, StemCount(c.perHa), 24f, 10f);
            yield return null;
            var ms = new List<double>(); var alloc = new List<double>();
            var sw = new System.Diagnostics.Stopwatch();
            int collectedYears = 0;
            for (int y = 0; y < 15; y++)
            {
                // GC.GetAllocatedBytesForCurrentThread reports nothing under Unity's Mono (first run: 0.00 everywhere), so the Mono
                // heap in use is read before and after; a year in which a collection ran is excluded because the delta is then meaningless.
                long before = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong(); int gcBefore = GC.CollectionCount(0);
                sw.Restart();
                e.AdvanceOneYear();
                sw.Stop();
                long after = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
                ms.Add(sw.Elapsed.TotalMilliseconds);
                if (GC.CollectionCount(0) == gcBefore) alloc.Add((after - before) / (1024.0 * 1024.0)); else collectedYears++;
                yield return null;
            }
            if (alloc.Count == 0) alloc.Add(double.NaN);
            var sorted = ms.OrderBy(v => v).ToList();
            perfRecords++;
            Measure($"PERF_ANNUAL_{c.label}", $"stems={StemCount(c.perHa)} years=15 meanMs={F(ms.Average(), "0.0")} medianMs={F(sorted[sorted.Count / 2], "0.0")} p95Ms={F(sorted[(int)(sorted.Count * 0.95)], "0.0")} maxMs={F(sorted.Last(), "0.0")} monoHeapGrowthMeanMB={F(alloc.Average(), "0.00")} max={F(alloc.Max(), "0.00")} yearsMeasured={alloc.Count(v => !double.IsNaN(v))}/15 yearsWithACollection={collectedYears}");
        }
        // The pure model on its own.
        Stand(2, k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak, StemCount(1300f), 24f, 10f);
        List<DensityStem> stems = StemsOfLiving();
        var timer = System.Diagnostics.Stopwatch.StartNew();
        const int reps = 200;
        for (int i = 0; i < reps; i++) ComponentPlan(stems);
        timer.Stop();
        perfRecords++;
        Measure("PERF_PURE_PLAN", $"stems={stems.Count} meanMsPerPlan={F(timer.Elapsed.TotalMilliseconds / reps, "0.000")}");
    }

    // ---------- LONGRUN ----------

    private sealed class RunResult
    {
        public readonly List<string> Rows = new List<string>();
        public readonly Dictionary<int, string> CheckpointHash = new Dictionary<int, string>();
        public readonly List<double> OccupancyUsed = new List<double>(), OccupancyAfter = new List<double>(), DeathFraction = new List<double>(), Ba = new List<double>();
        public readonly List<int> Living = new List<int>(), Deaths = new List<int>();
        // Self-thinning deaths by species (Sitka, Beech, Oak): observed, and expected from competitive position alone.
        public readonly double[] Obs = new double[3], Exp = new double[3], Var = new double[3];
        public string Summary = "";
        // Verification-only per-component basal-area rows (see component-ba-<geo>.csv) and how often each component fired.
        public readonly List<string> ComponentRows = new List<string>();
        public int BothFired, SitkaOnlyFired, GenericOnlyFired, NeitherFired, AttributionMismatch;
    }

    private static readonly int[] CheckpointYears = { 25, 50, 100, 150, 200, 300 };
    private int SpeciesIndex(ForestTree t) => t.Species == sitka ? 0 : t.Species == beech ? 1 : 2;
    private bool quietCheckpoints;

    // Schema of component-ba-<geo>.csv. One row per scenario-year. sitkaBaLossM2 and genericBaLossM2 are the basal area of the
    // stems each component killed (legacy route: the whole step is the Sitka component); combinedBaLossM2 is their sum and
    // deadBaM2 the basal area of every stem that actually died this year. combinedBaLossM2 and deadBaM2 should agree; the
    // offline reader (component_ba.py) proves it and rejects malformed or incomplete files. attributionExact is an
    // instrumentation-integrity flag (the rebuilt component plan matched the controller's own counts and the actual dead set),
    // never a forestry threshold.
    private const string ComponentBaHeader = "scenario,geo,year,route,livingBefore,baPreM2,densityDeaths,sitkaComponentDeaths,genericDeaths,sitkaBaLossM2,genericBaLossM2,combinedBaLossM2,sitkaBaFraction,genericBaFraction,combinedBaFraction,deadBaM2,deadBaFraction,sitkaFired,genericFired,bothFired,attributionExact";

    private IEnumerator RunScenario(string name, int growth, Func<int, TreeSpeciesDefinition> speciesOf, float perHa, float dbhMin, float spread, int years, RunResult result)
    {
        int count = StemCount(perHa);
        // CCF_QUAL_SEED selects another deterministic stream (competition, rolls and recruitment all key on the simulation seed); unset = the default stream.
        int? runSeed = int.TryParse(Environment.GetEnvironmentVariable("CCF_QUAL_SEED"), out int parsedSeed) ? parsedSeed : (int?)null;
        Stand(growth, speciesOf, count, dbhMin, spread, runSeed);
        yield return null;
        int initial = Living().Length;
        for (int year = 1; year <= years; year++)
        {
            // Competition as the annual step will compute it (UpdateCompetition is its first act on this same stand), so the
            // expected deaths below use the very indices the density step used. Read-only: the step recomputes it anyway.
            e.InvalidateCompetition();
            ForestTree[] pre = Living();
            var ciPre = new Dictionary<ForestTree, double>();
            foreach (ForestTree t in pre) ciPre[t] = e.GetCompetitionIndex(t);
            double meanVuln = pre.Length == 0 ? 0.0 : pre.Average(t => MixedDensityModel.Vulnerability(MixedDensityModel.Suppression(ciPre[t])));
            e.AdvanceOneYear();
            AdultMortalityAccount a = e.LastAdultMortality;
            // Expected deaths from competitive position alone are only defined for the GENERIC component, so the cumulative attribution counts the
            // years in which the Sitka component took nobody (every pure Oak/Beech year, and mixed years with no Sitka demand). In those years the
            // generic component chose among all stems, with expected fraction GenericLossFraction.
            if (a.Route == DensityRoute.SpeciesAwareOccupancy && a.SitkaComponentDeaths == 0)
            {
                double expectedLoss = a.GenericLossFraction;
                foreach (ForestTree t in pre)
                {
                    int s = SpeciesIndex(t);
                    double p = MixedDensityModel.AnnualLossProbability(expectedLoss, MixedDensityModel.Suppression(ciPre[t]), meanVuln);
                    result.Exp[s] += p; result.Var[s] += p * (1.0 - p);
                    if (t.IsBiologicallyDead && t.MortalityYear == e.EcologicalYear && t.MortalityCause == "self-thinning") result.Obs[s]++;
                }
            }
            ForestTree[] living = Living();
            double ba = BasalAreaPerHectare(living);
            double occAfter = OccupancyOfTrees(living);
            double dq = living.Length == 0 ? 0 : Math.Sqrt(living.Average(t => (double)t.Diameter * t.Diameter));
            double mean = living.Length == 0 ? 0 : living.Average(t => (double)t.Diameter);
            int nS = living.Count(t => t.Species == sitka), nB = living.Count(t => t.Species == beech), nO = living.Count(t => t.Species == oak);
            double baS = BasalAreaPerHectare(living.Where(t => t.Species == sitka)), baB = BasalAreaPerHectare(living.Where(t => t.Species == beech)), baO = BasalAreaPerHectare(living.Where(t => t.Species == oak));
            int other = living.Length - (a.LivingBefore - a.Deaths);
            // Who died this step, by the diameters the step used (growth precedes mortality in the annual order).
            var livingIds = new HashSet<string>(living.Select(t => t.TreeId), StringComparer.Ordinal);
            var diedNow = pre.Where(t => !livingIds.Contains(t.TreeId)).ToList();
            double baPre = pre.Sum(t => Math.PI / 4.0 * (t.Diameter / 100.0) * (t.Diameter / 100.0)), baDead = diedNow.Sum(t => Math.PI / 4.0 * (t.Diameter / 100.0) * (t.Diameter / 100.0));
            double deadBaFraction = baPre > 0 ? baDead / baPre : 0.0;
            // Verification-only per-component basal-area attribution. This changes no selection, threshold, RNG, equation or
            // result: the annual density step ran on the post-growth stand with step 1's competition index and this year's keyed
            // rolls, so the same component plan can be rebuilt from the pre-step stems, their diameters now (post-growth, the
            // diameters the step used) and that index, and the basal area of the victims attributed to the component that chose
            // them. The rebuild is checked against the controller's own counts and the stems that actually died; the outcome is
            // recorded (attributionExact), never used as a forestry threshold.
            double sitkaBaLoss = baDead, genericBaLoss = 0.0;   // legacy route: the whole step is the Sitka component, no generic
            int sitkaComponentDeaths = a.Route == DensityRoute.LegacySitkaExact ? a.Deaths : a.SitkaComponentDeaths;
            int genericComponentDeaths = a.Route == DensityRoute.LegacySitkaExact ? 0 : a.GenericDeaths;
            bool attributionExact = a.Deaths == diedNow.Count;
            if (a.Route == DensityRoute.SpeciesAwareOccupancy)
            {
                var stemsStep = new List<DensityStem>(pre.Length);
                foreach (ForestTree t in pre)
                {
                    TreeSpeciesDefinition ts = t.Species;
                    stemsStep.Add(new DensityStem(t.TreeId, ts != null ? ts.GrowingSpaceDemand : GrowingSpaceDemandClass.Reference, t.Diameter, ciPre[t]));
                }
                DensityComponentPlan plan = ComponentPlan(stemsStep);
                sitkaBaLoss = 0.0; genericBaLoss = 0.0;
                foreach (int i in plan.SitkaVictimIndices) sitkaBaLoss += StemBaM2(pre[i]);
                foreach (int i in plan.GenericVictimIndices) genericBaLoss += StemBaM2(pre[i]);
                sitkaComponentDeaths = plan.SitkaVictimIndices.Count;
                genericComponentDeaths = plan.GenericVictimIndices.Count;
                var planIds = new HashSet<string>(plan.VictimIndices.Select(i => stemsStep[i].TreeId), StringComparer.Ordinal);
                var deadIds = new HashSet<string>(diedNow.Select(t => t.TreeId), StringComparer.Ordinal);
                attributionExact = plan.VictimIndices.Count == a.Deaths && sitkaComponentDeaths == a.SitkaComponentDeaths
                    && genericComponentDeaths == a.GenericDeaths && planIds.SetEquals(deadIds);
            }
            if (!attributionExact) result.AttributionMismatch++;
            double combinedBaLoss = sitkaBaLoss + genericBaLoss;
            bool sitkaFired = sitkaComponentDeaths > 0, genericFired = genericComponentDeaths > 0;
            if (sitkaFired && genericFired) result.BothFired++;
            else if (sitkaFired) result.SitkaOnlyFired++;
            else if (genericFired) result.GenericOnlyFired++;
            else result.NeitherFired++;
            result.ComponentRows.Add($"{name},{geo},{year},{a.Route},{a.LivingBefore},{F(baPre, "0.00000")},{a.Deaths},{sitkaComponentDeaths},{genericComponentDeaths},{F(sitkaBaLoss, "0.00000")},{F(genericBaLoss, "0.00000")},{F(combinedBaLoss, "0.00000")},{F(baPre > 0 ? sitkaBaLoss / baPre : 0.0, "0.00000")},{F(baPre > 0 ? genericBaLoss / baPre : 0.0, "0.00000")},{F(baPre > 0 ? combinedBaLoss / baPre : 0.0, "0.00000")},{F(baDead, "0.00000")},{F(deadBaFraction, "0.00000")},{(sitkaFired ? 1 : 0)},{(genericFired ? 1 : 0)},{(sitkaFired && genericFired ? 1 : 0)},{(attributionExact ? 1 : 0)}");
            int bigBefore = pre.Count(t => t.Diameter >= 20f), bigDeaths = diedNow.Count(t => t.Diameter >= 20f);
            int diedS = diedNow.Count(t => t.Species == sitka), diedB = diedNow.Count(t => t.Species == beech), diedO = diedNow.Count(t => t.Species == oak);
            string row = $"{name},{geo},{year},{a.LivingBefore},{a.Deaths},{a.HazardDeaths},{a.BoundaryDeaths},{a.Route},{F(a.Occupancy, "0.0000")},{F(a.RelativeDensity, "0.0000")},{F(a.DensityPressure, "0.0000")},{living.Length},{other},{F(ba, "0.00")},{F(occAfter, "0.0000")},{F(dq, "0.00")},{F(mean, "0.00")},{nS},{nB},{nO},{F(baS, "0.00")},{F(baB, "0.00")},{F(baO, "0.00")},{SitkaDeaths(a)},{a.GenericDeaths},{F(deadBaFraction, "0.00000")},{bigBefore},{bigDeaths},{F(a.Route == DensityRoute.SpeciesAwareOccupancy ? a.SitkaSubsetRelativeDensity : a.RelativeDensity, "0.0000")},{diedS},{diedB},{diedO},{F(a.GenericLossFraction, "0.00000")},{F(a.NonReferenceShare, "0.0000")}";
            result.Rows.Add(row);
            result.Living.Add(living.Length); result.Deaths.Add(a.Deaths);
            result.OccupancyUsed.Add(a.Route == DensityRoute.SpeciesAwareOccupancy ? a.Occupancy : OccupancyOfTrees(living));
            result.OccupancyAfter.Add(occAfter); result.Ba.Add(ba);
            result.DeathFraction.Add(a.LivingBefore > 0 ? (double)a.Deaths / a.LivingBefore : 0.0);
            if (Array.IndexOf(CheckpointYears, year) >= 0) result.CheckpointHash[year] = TreeStateHash();
            if (Array.IndexOf(CheckpointYears, year) >= 0 && !quietCheckpoints)
            {
                string meanCi(int s) { var v = pre.Where(t => SpeciesIndex(t) == s).Select(t => ciPre[t]).ToList(); return v.Count == 0 ? "-" : F(v.Average(), "0.00"); }
                string cum(int s) => $"{result.Obs[s]}/{F(result.Exp[s], "0.0")}";
                Measure("LONGRUN_CHECKPOINT", $"scenario={name} year={year} living={living.Length}/{initial} baM2ha={F(ba, "0.0")} occupancy={F(occAfter, "0.000")} densityDeathsThisYear={a.Deaths} (hazard={a.HazardDeaths} boundary={a.BoundaryDeaths}) route={a.Route} species(S/B/O)={nS}/{nB}/{nO} baBySpecies(S/B/O)={F(baS, "0.0")}/{F(baB, "0.0")}/{F(baO, "0.0")} dq={F(dq, "0.0")}cm meanDbh={F(mean, "0.0")}cm meanCompetitionIndexPreStep(S/B/O)={meanCi(0)}/{meanCi(1)}/{meanCi(2)} cumulativeSelfThinningDeathsObserved/ExpectedFromCompetitionAlone(S/B/O)={cum(0)}|{cum(1)}|{cum(2)}");
            }
            if (year % 5 == 0) yield return null;
        }
    }

    private static double Slope(IList<double> y, int from, int to)
    {
        int n = to - from + 1; double sx = 0, sy = 0, sxx = 0, sxy = 0;
        for (int i = from; i <= to; i++) { double x = i; sx += x; sy += y[i]; sxx += x * x; sxy += x * y[i]; }
        double den = n * sxx - sx * sx;
        return Math.Abs(den) < 1e-12 ? 0.0 : (n * sxy - sx * sy) / den;
    }

    private IEnumerator LongRun()
    {
        Func<int, TreeSpeciesDefinition> three = k => k % 3 == 0 ? sitka : k % 3 == 1 ? beech : oak;
        var scenarios = new (string name, int growth, Func<int, TreeSpeciesDefinition> speciesOf, float perHa, float dbhMin, float spread)[]
        {
            ("A_OAK_DENSE", 2, _ => oak, 1000f, 24f, 10f),
            ("B_BEECH_DENSE", 2, _ => beech, 1300f, 24f, 10f),
            ("C_MIXED_DENSE", 2, three, 1300f, 24f, 10f),
            ("D_SITKA_DENSE_MODEL2", 2, _ => sitka, 1200f, 24f, 10f),
            ("E_OAK_YOUNG", 2, _ => oak, 2500f, 8f, 6f),
            ("F_BEECH_YOUNG", 2, _ => beech, 2500f, 8f, 6f),
            ("G_MIXED_YOUNG", 2, three, 2500f, 8f, 6f),
            // Same stems under growth model 1 (Sitka relative-density route for every species, species Ci50 in victim choice):
            // a continuity reference, not a correctness reference - model 1 treats Oak and Beech as Sitka.
            ("C1_MIXED_DENSE_MODEL1", 1, three, 1300f, 24f, 10f),
            ("G1_MIXED_YOUNG_MODEL1", 1, three, 2500f, 8f, 6f)
        };
        string filter = Environment.GetEnvironmentVariable("CCF_QUAL_SCENARIOS");
        string[] selected = DensityQualificationCoverage.SelectScenarios(scenarios.Select(sc => sc.name).ToArray(), filter);
        scenarios = scenarios.Where(sc => selected.Contains(sc.name)).ToArray();
        Measure("LONGRUN_SELECTION", $"filter=[{filter ?? "<all>"}] selected={scenarios.Length} scenarios={string.Join(",", selected)}");
        int horizon = int.TryParse(Environment.GetEnvironmentVariable("CCF_QUAL_YEARS"), out int h) && h >= 100 ? h : 100;
        bool replay = Environment.GetEnvironmentVariable("CCF_QUAL_REPLAY") != "0";
        expectedLongRunRows = scenarios.Length * horizon;
        string header = "scenario,geo,year,livingBefore,densityDeaths,hazard,boundary,route,occupancyUsed,sitkaRd,pressure,livingAfter,netOther,baM2ha,occupancyAfter,dq,meanDbh,nSitka,nBeech,nOak,baSitka,baBeech,baOak,sitkaComponentDeaths,genericDeaths,deadBaFraction,bigBefore,bigDeaths,sitkaRdBeforeStep,deathsSitka,deathsBeech,deathsOak,genericLossFraction,nonRefShare";
        Csv($"longrun-{geo}.csv", header);
        // Verification-only per-component basal-area evidence: one machine-readable row per scenario-year, written for exactly
        // the scenarios LONGRUN runs so Tier-3 can measure the Sitka and generic components' basal area annually in a mixed
        // stand. See component_ba.py for the offline schema/consistency reader. It is not a threshold and not a new gate.
        Csv($"component-ba-{geo}.csv", ComponentBaHeader);
        foreach (var sc in scenarios)
        {
            var r1 = new RunResult(); var r2 = new RunResult();
            quietCheckpoints = false;
            yield return RunScenario(sc.name, sc.growth, sc.speciesOf, sc.perHa, sc.dbhMin, sc.spread, horizon, r1);
            if (r1.Rows.Count != horizon || r1.Living.Count != horizon)
                throw new InvalidOperationException($"Incomplete LONGRUN measurements for {sc.name}: expectedYears={horizon} rows={r1.Rows.Count}");
            if (r1.ComponentRows.Count != horizon)
                throw new InvalidOperationException($"Incomplete component-ba measurements for {sc.name}: expectedYears={horizon} rows={r1.ComponentRows.Count}");
            longRunRows += r1.Rows.Count;
            foreach (string row in r1.Rows) Csv($"longrun-{geo}.csv", row);
            foreach (string row in r1.ComponentRows) Csv($"component-ba-{geo}.csv", row);
            Measure("COMPONENT_BA", $"scenario={sc.name} years={horizon} bothFiredYears={r1.BothFired} sitkaOnlyYears={r1.SitkaOnlyFired} genericOnlyYears={r1.GenericOnlyFired} neitherFiredYears={r1.NeitherFired} attributionMismatches={r1.AttributionMismatch} file=component-ba-{geo}.csv");
            if (replay)
            {
                quietCheckpoints = true;   // the replay is only compared, not reported twice
                yield return RunScenario(sc.name, sc.growth, sc.speciesOf, sc.perHa, sc.dbhMin, sc.spread, horizon, r2);
                quietCheckpoints = false;
                bool deterministic = r1.Rows.SequenceEqual(r2.Rows) && r1.CheckpointHash.All(kv => r2.CheckpointHash.TryGetValue(kv.Key, out string hh) && hh == kv.Value);
                Hard($"LONGRUN_{sc.name}_REPLAY_IS_EXACT", deterministic, $"rowHash1={Fnv(r1.Rows)} rowHash2={Fnv(r2.Rows)} hashY25/50/100={string.Join("/", r1.CheckpointHash.Where(kv => kv.Key <= 100).OrderBy(kv => kv.Key).Select(kv => kv.Value))}");
            }
            else Measure("LONGRUN_ROWHASH", $"scenario={sc.name} rowHash={Fnv(r1.Rows)} (replay skipped)");
            double tz(int s) => r1.Var[s] > 0 ? (r1.Obs[s] - r1.Exp[s]) / Math.Sqrt(r1.Var[s]) : 0.0;
            double tObs = r1.Obs.Sum(), tExp = r1.Exp.Sum(), tVar = r1.Var.Sum(), tRatio = tExp > 0 ? tObs / tExp : 1.0;
            string rel(int s) => F(r1.Var[s] > 0 ? (r1.Obs[s] - tRatio * r1.Exp[s]) / Math.Sqrt(r1.Var[s]) : 0.0, "0.0");
            if (tExp > 0)
                Measure("LONGRUN_DEATH_ATTRIBUTION", $"scenario={sc.name} observed(S/B/O)={r1.Obs[0]}/{r1.Obs[1]}/{r1.Obs[2]} expectedFromCompetitionAlone={F(r1.Exp[0], "0.0")}/{F(r1.Exp[1], "0.0")}/{F(r1.Exp[2], "0.0")} z={F(tz(0), "0.0")}/{F(tz(1), "0.0")}/{F(tz(2), "0.0")} pooledRatio={F(tRatio, "0.000")} zRelativeToPooled={rel(0)}/{rel(1)}/{rel(2)} (the stand's own deaths, species choice explained by competition position)");

            int n = r1.Living.Count;
            double initial = Math.Max(1, r1.Living[0] + r1.Deaths[0]);
            double maxFraction = r1.DeathFraction.Max();
            int heavyYears = r1.DeathFraction.Count(v => v >= 0.15);
            int deathYears = r1.Deaths.Count(d => d > 0);
            int signChanges = 0;
            for (int i = 51; i < Math.Min(n, 100); i++) if ((r1.Deaths[i] - r1.Deaths[i - 1]) * (r1.Deaths[i - 1] - r1.Deaths[i - 2]) < 0) signChanges++;
            double slope75 = Slope(r1.OccupancyAfter, n - 25, n - 1), slope25 = Slope(r1.OccupancyAfter, 25, 49);
            string occAt = string.Join(" ", new[] { 25, 50, 75, 100, 150, 200, 250, 300 }.Where(yr => yr <= n).Select(yr => $"Y{yr}={F(r1.OccupancyAfter[yr - 1], "0.00")}/BA{F(r1.Ba[yr - 1], "0")}/n{r1.Living[yr - 1]}"));
            int firstOnset = r1.OccupancyUsed.FindIndex(v => v > 1.0) + 1;
            int lastAbove = r1.OccupancyUsed.FindLastIndex(v => v > 1.0) + 1;
            Measure("LONGRUN_METRICS", $"scenario={sc.name} minLivingFraction={F(r1.Living.Min() / initial, "0.000")} finalLiving={r1.Living[n - 1]} maxAnnualDeathFraction={F(maxFraction, "0.000")} yearsWithDeathFractionOver15pc={heavyYears} yearsWithAnyDensityDeath={deathYears} deathSeriesSignChangesY50to100={signChanges} occupancy/baM2ha/living: Y1={F(r1.OccupancyAfter[0], "0.00")} {occAt} occupancySlopePerYearY25to50={F(slope25, "0.0000")} lastQuarterOfRun={F(slope75, "0.0000")} firstYearAboveOnset={firstOnset} lastYearAboveOnset={lastAbove} meanAnnualDeathFractionLastQuarter={F(r1.DeathFraction.Skip(n - 25).Average(), "0.0000")}");
            yield return null;
        }
    }

    // ---------- driver ----------

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        saves = FindFirstObjectByType<ForestSaveController>();
        sitka = spawner.ResolveSpecies("sitka-spruce");
        oak = spawner.ResolveSpecies("sessile-oak");
        beech = spawner.ResolveSpecies("beech");
        geo = e.StandGeometryModelVersion == StandGeometryModel.Legacy40 ? "L40" : "E80";
        string diag = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        outDir = string.IsNullOrEmpty(diag) ? Path.Combine(Application.dataPath, "..", "Build", "FinalDensityQualification", "evidence") : diag;
        Directory.CreateDirectory(outDir);
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Check(sitka != null && oak != null && beech != null, "species resolve");
        original = saves.CaptureData();
        string phases = Environment.GetEnvironmentVariable("CCF_QUAL_PHASES");
        var wanted = new HashSet<string>(DensityQualificationCoverage.SelectPhases(phases));
        DensityQualificationCoverage.ScenarioTokens(Environment.GetEnvironmentVariable("CCF_QUAL_SCENARIOS"));
        expectedPerfRecords = wanted.Contains("PERF") ? 6 : 0;
        measurementOnly = wanted.All(phase => phase == "PERF" || (phase == "LONGRUN" && Environment.GetEnvironmentVariable("CCF_QUAL_REPLAY") == "0"));
        Debug.Log($"QUAL_START geometry={geo} areaHa={F(e.StandAreaHectares, "0.00")} cells={e.CellCount} phases={string.Join(",", wanted)}");
        var started = System.Diagnostics.Stopwatch.StartNew();
        if (wanted.Contains("RATIOS")) { yield return Ratios(); Debug.Log($"QUAL_PHASE_DONE RATIOS t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("ROLLS")) { yield return Rolls(); Debug.Log($"QUAL_PHASE_DONE ROLLS t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("ANCHORS")) { yield return Anchors(); Debug.Log($"QUAL_PHASE_DONE ANCHORS t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("SITKA100")) { yield return Sitka100(); Debug.Log($"QUAL_PHASE_DONE SITKA100 t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("SITKACOMP")) { yield return SitkaComp(); Debug.Log($"QUAL_PHASE_DONE SITKACOMP t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("PARITY")) { yield return Parity(); Debug.Log($"QUAL_PHASE_DONE PARITY t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("CURVE")) { yield return Curve(); Debug.Log($"QUAL_PHASE_DONE CURVE t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("FAIRNESS")) { yield return Fairness(); Debug.Log($"QUAL_PHASE_DONE FAIRNESS t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("CONTINUITY")) { yield return Continuity(); Debug.Log($"QUAL_PHASE_DONE CONTINUITY t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("FAIRNESSHIGH")) { yield return Fairness(3.0, "_HIGH_LOSS"); Debug.Log($"QUAL_PHASE_DONE FAIRNESSHIGH t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("LATEMIX")) { yield return LateMix(); Debug.Log($"QUAL_PHASE_DONE LATEMIX t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("FINALBROADLEAF")) { yield return FinalBroadleaf(); Debug.Log($"QUAL_PHASE_DONE FINALBROADLEAF t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("SAVELOAD")) { yield return SaveLoad(); Debug.Log($"QUAL_PHASE_DONE SAVELOAD t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("PERF")) { yield return Perf(); Debug.Log($"QUAL_PHASE_DONE PERF t={started.Elapsed.TotalSeconds:0}s"); }
        if (wanted.Contains("LONGRUN")) { yield return LongRun(); Debug.Log($"QUAL_PHASE_DONE LONGRUN t={started.Elapsed.TotalSeconds:0}s"); }
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
        try { DensityQualificationCoverage.RequireCoverage(hardPassed, measurementOnly, longRunRows + perfRecords, expectedLongRunRows + expectedPerfRecords); }
        catch (Exception ex) { failure = failure ?? ex.Message; }
        bool ok = failure == null && hardFailures.Count == 0;
        Debug.Log(ok ? $"DENSITY_QUAL_PASS geometry={geo} hardChecks={hardPassed} measurementOnly={measurementOnly} longRunRows={longRunRows} expectedLongRunRows={expectedLongRunRows} perfRecords={perfRecords} expectedPerfRecords={expectedPerfRecords}" : $"DENSITY_QUAL_FAIL geometry={geo} exception={failure} hardFailures=[{string.Join(" | ", hardFailures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }
}
