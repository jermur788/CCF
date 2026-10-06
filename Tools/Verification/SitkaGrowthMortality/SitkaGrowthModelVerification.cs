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

// Disposable verification for growth model 1 (Irish Class III Sitka height +
// adult density mortality) and its save/model policy. Copy into
// Assets/ForestPrototype, run with -executeMethod SitkaGrowthModelVerification.Begin,
// then remove the copy and its .meta. Saves are captured/loaded in memory only.
public static class SitkaGrowthModelVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "SitkaGrowthModelVerification.Begin"))
            new GameObject("Sitka growth model verification").AddComponent<SitkaGrowthModelVerificationRunner>();
    }
}

public sealed class SitkaGrowthModelVerificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private const string Neutral0 = "BFC55473C1506067", Normal0 = "3485B6630C9EA448";
    private const string RegenNeutral1 = "962846D2F517B293", RegenNormal1 = "FBB8F470D85FF815";

    private ForestEcologyController e;
    private ForestStartingStand stand;
    private ForestTreeSpawner spawner;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestSaveData original;
    private TreeSpeciesDefinition sitka;
    private readonly List<string> failures = new List<string>();
    private int passed;

    private static string F(double v, string f = "0.###") => v.ToString(f, Inv);
    private void Pass(string name, bool ok, string detail)
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"GROWTH_MODEL_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Check(bool ok, string what) { if (!ok) { failures.Add(what); Debug.LogError("GROWTH_MODEL_CHECK_FAIL " + what); } }
    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
    private ForestSaveData Clone(ForestSaveData d) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));

    private void Build(int growth)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = growth;
        e.Browsing.BackgroundPressure = manager.Definition.BackgroundBrowsePressure;
        e.Browsing.ClearProtection();
        stand.Generate();
        e.ResetForDeterministicRun();
        e.InvalidateCompetition();
    }

    private static double TopHeight(ForestTree[] trees)
    {
        int top = Mathf.Max(1, Mathf.RoundToInt(100 * 0.16f));
        return trees.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).Take(top).Average(t => (double)t.Height);
    }

    private double RelativeDensity(ForestTree[] trees)
    {
        if (trees.Length == 0) return 0;
        double dq = Math.Sqrt(trees.Average(t => (double)t.Diameter * t.Diameter));
        return SitkaGrowthModel.RelativeDensity(trees.Length / e.StandAreaHectares, dq);
    }

    private string WorldHash() => ScenarioReferenceArchive.WorldHash(saves.CaptureData());

    // ---------- policy ----------

    private IEnumerator Policy()
    {
        Pass("POLICY_NEW_GAME_GROWTH1", ScenarioOneManager.NewGameGrowthModel == GrowthModel.SiteClassDensity && original.growthModel == 1
            && original.version == 17 && original.regenerationModel == 1 && original.rngModelVersion == 1,
            $"growth={original.growthModel} regen={original.regenerationModel} rng={original.rngModelVersion} version={original.version}");
        string stripped = JsonUtility.ToJson(Clone(original)).Replace("\"growthModel\":1,", "");
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(stripped), false), "stripped load"); yield return null;
        bool missing = e.GrowthModelVersion == 0;
        ForestSaveData v16 = Clone(original); v16.version = 16; v16.growthModel = 1;
        Check(saves.LoadData(v16, false), "v16 load"); yield return null;
        bool v16Zero = e.GrowthModelVersion == 0 && e.RegenerationModelVersion == 1;
        ForestSaveData zero = Clone(original); zero.growthModel = 0;
        Check(saves.LoadData(zero, false), "explicit 0 load"); yield return null;
        bool explicitZero = e.GrowthModelVersion == 0;
        Check(saves.LoadData(Clone(original), false), "explicit 1 load"); yield return null;
        bool explicitOne = e.GrowthModelVersion == 1;
        ForestSaveData bad = Clone(original); bad.growthModel = 7;
        string badProblem = ForestSaveValidation.Validate(bad, 0, e.CellCount);
        Pass("POLICY_SAVE_SEMANTICS", missing && v16Zero && explicitZero && explicitOne && badProblem != null && !stripped.Contains("growthModel"),
            $"missing0={missing} v16->0={v16Zero} explicit0={explicitZero} explicit1={explicitOne} invalidRejected=[{badProblem}]");
        bool previewZero = true, restored = true;
        foreach (int year in new[] { 0, 20, 50, 100 })
        {
            Check(manager.TryBeginReferencePreview(year), "preview " + year);
            yield return null; yield return null;
            previewZero &= e.GrowthModelVersion == 0;
            manager.EndReferencePreview();
            yield return null; yield return null;
            restored &= e.GrowthModelVersion == 1;
        }
        Pass("POLICY_REFERENCE_V1_GROWTH0", previewZero && restored, $"previewGrowth0={previewZero} playerRestored={restored}");
    }

    // ---------- site height ----------

    private IEnumerator SiteHeight()
    {
        var anchors = new Dictionary<int, double> { [20] = 14.2, [30] = 20.4, [40] = 24.9, [60] = 30.1, [80] = 32.5, [100] = 33.6 };
        bool envelopeOk = anchors.All(a => Math.Abs(SitkaGrowthModel.SiteTopHeightM(a.Key) - a.Value) < 0.06);
        Pass("SITE_ENVELOPE_CLASS_III", envelopeOk && Math.Abs(SitkaGrowthModel.SiteTopHeightM(30) - 20.4) < 1e-9,
            string.Join(" ", anchors.Keys.Select(a => $"H{a}={F(SitkaGrowthModel.SiteTopHeightM(a), "0.00")}")) + $" b0={F(SitkaGrowthModel.ClassIIIb0, "0.000")}");
        // Individual height keeps relative position; no competition response.
        float h1 = SitkaGrowthModel.NextHeight(10f, 20), h2 = SitkaGrowthModel.NextHeight(14.2f, 20);
        Pass("INDIVIDUAL_RELATIVE_HEIGHT", Math.Abs(h1 / 10f - h2 / 14.2f) < 1e-5 && h1 < h2,
            $"suppressed 10.00->{F(h1, "0.000")} dominant 14.20->{F(h2, "0.000")} (same ratio)");

        // DBH is unchanged by the height model while no tree dies (RD below onset early on).
        Build(0);
        for (int y = 0; y < 5; y++) { e.AdvanceOneYear(); yield return null; }
        var dbh0 = Living().ToDictionary(t => t.TreeId, t => t.Diameter);
        var h0 = Living().ToDictionary(t => t.TreeId, t => t.Height);
        Build(1);
        int deaths = 0;
        for (int y = 0; y < 5; y++) { e.AdvanceOneYear(); deaths += e.LastAdultMortality.Year == e.EcologicalYear ? e.LastAdultMortality.Deaths : 0; yield return null; }
        var dbh1 = Living().ToDictionary(t => t.TreeId, t => t.Diameter);
        bool sameDbh = deaths == 0 && dbh0.Count == dbh1.Count && dbh0.All(p => dbh1.TryGetValue(p.Key, out float d) && d == p.Value);
        bool taller = Living().Where(t => t.TreeId.StartsWith("P")).All(t => t.Height > h0[t.TreeId]);
        Pass("DBH_UNCHANGED_BY_HEIGHT_MODEL", sameDbh && taller, $"years=5 deaths={deaths} identicalDbh={sameDbh} plantedTallerThanLegacy={taller}");

        // Stand top height against Class III, unthinned and heavily thinned; deterministic repeat.
        var tops = new Dictionary<string, Dictionary<int, double>>();
        foreach (string regime in new[] { "unthinned", "heavy", "repeat" })
        {
            Build(1);
            var row = new Dictionary<int, double> { [20] = TopHeight(Living()) };
            for (int y = 1; y <= 80; y++)
            {
                int age = 20 + y;
                if (regime == "heavy" && (age - 1 == 25 || age - 1 == 40 || age - 1 == 55 || age - 1 == 70))
                {
                    ForestTree[] living = Living().Where(t => t.CanChop).ToArray();
                    e.BeginChangeBatch();
                    foreach (ForestTree t in living.OrderByDescending(t => e.GetCompetitionIndex(t)).ThenBy(t => t.TreeId, StringComparer.Ordinal).Take(Mathf.RoundToInt(living.Length * 0.35f)))
                        t.Fell();
                    e.EndChangeBatch();
                }
                e.AdvanceOneYear();
                if (anchors.ContainsKey(age)) row[age] = TopHeight(Living());
                if (y % 10 == 0) { yield return Resources.UnloadUnusedAssets(); GC.Collect(); } else yield return null;
            }
            tops[regime] = row;
        }
        string Line(string r) => string.Join(" ", tops[r].OrderBy(p => p.Key).Select(p => $"H{p.Key}={F(p.Value, "0.00")}"));
        bool anchorFit = new[] { 30, 40, 60, 80, 100 }.All(a => Math.Abs(tops["unthinned"][a] - anchors[a]) <= 0.10 * anchors[a]);
        Pass("TOP_HEIGHT_CLASS_III", anchorFit, "unthinned " + Line("unthinned"));
        double maxThinEffect = new[] { 30, 40, 60, 80, 100 }.Max(a => Math.Abs(tops["heavy"][a] - tops["unthinned"][a]) / tops["unthinned"][a]);
        Pass("THINNING_WEAK_ON_TOP_HEIGHT", maxThinEffect <= 0.10, $"maxRelativeDifference={F(maxThinEffect, "0.000")} heavy {Line("heavy")}");
        bool repeat = tops["repeat"].All(p => p.Value == tops["unthinned"][p.Key]);
        Pass("HEIGHT_DETERMINISTIC", repeat, "repeat identical=" + repeat);
    }

    // ---------- mortality, deadwood, save/load ----------

    // Synthetic Sitka stand in the hazard zone (14 x 14 = 1,225 stems/ha at 24-34 cm, RD about 0.9).
    private void DenseStand(int perAxis, float dbh, float spread)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = GrowthModel.SiteClassDensity;
        float spacing = 38f / perAxis;
        int k = 0;
        for (int i = 0; i < perAxis; i++)
            for (int j = 0; j < perAxis; j++, k++)
            {
                float d = dbh + ((k * 7919) % 100) / 100f * spread;
                spawner.Spawn("S" + k.ToString("0000"), sitka, new Vector3(-19f + (i + 0.5f) * spacing, 0f, -19f + (j + 0.5f) * spacing), 40, d, 2f + 0.62f * d, sitka.PotentialCrownRadiusM(d));
            }
        e.InvalidateCompetition();
        e.RecomputeCanopy();
    }

    private IEnumerator Mortality()
    {
        // Below the onset: no density deaths (Scenario One starting stand, RD 0.44).
        Build(1);
        double rd0 = RelativeDensity(Living());
        e.AdvanceOneYear();
        Pass("NO_DEATHS_BELOW_ONSET", rd0 < SitkaGrowthModel.MortalityOnsetRelativeDensity && e.LastAdultMortality.Deaths == 0,
            $"RD={F(rd0, "0.000")} onset={F(SitkaGrowthModel.MortalityOnsetRelativeDensity)} deaths={e.LastAdultMortality.Deaths}");

        // Dense stand: suppressed trees die far more often than dominants; cause/year;
        // deadwood exactly once; boundary keeps RD at or below the maximum line.
        DenseStand(14, 24f, 10f);
        yield return null;
        var died = new Dictionary<string, (int year, string cause)>();
        var suppressionAtStart = Living().ToDictionary(t => t.TreeId, t => e.GetCompetitionIndex(t));
        int deadwoodBefore = manager.DeadwoodRecords.Count;
        double maxRdAfter = 0;
        for (int y = 0; y < 10; y++)
        {
            var before = Living().Select(t => t.TreeId).ToHashSet();
            e.AdvanceOneYear();
            foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                if (t.IsBiologicallyDead && before.Contains(t.TreeId) && !died.ContainsKey(t.TreeId))
                    died[t.TreeId] = ((int)typeof(ForestTree).GetField("mortalityYear", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(t), t.MortalityCause);
            maxRdAfter = Math.Max(maxRdAfter, RelativeDensity(Living()));
            yield return null;
        }
        var ranked = suppressionAtStart.OrderBy(p => p.Value).ThenBy(p => p.Key, StringComparer.Ordinal).Select(p => p.Key).ToList();
        int quarter = ranked.Count / 4;
        double lowRate = ranked.Take(quarter).Count(died.ContainsKey) / (double)quarter;
        double highRate = ranked.Skip(ranked.Count - quarter).Count(died.ContainsKey) / (double)quarter;
        Pass("SUPPRESSED_DIE_MORE_THAN_DOMINANT", died.Count > 0 && highRate > 3 * Math.Max(lowRate, 0.01),
            $"deaths={died.Count}/{ranked.Count} mostSuppressedQuarter={F(highRate, "0.000")} leastSuppressedQuarter={F(lowRate, "0.000")}");
        bool causeYear = died.Values.All(v => v.cause == "self-thinning" && v.year >= 1 && v.year <= 10);
        Pass("DEATH_CAUSE_AND_YEAR", causeYear, "allSelfThinningWithStepYear=" + causeYear);
        var records = manager.DeadwoodRecords.Skip(deadwoodBefore).ToList();
        bool once = records.Count == died.Count && records.Select(r => r.treeId).Distinct().Count() == records.Count
            && records.All(r => died.ContainsKey(r.treeId) && r.fallenYear == died[r.treeId].year && r.originalVolumeM3 > 0f);
        Pass("DEADWOOD_CREATED_ONCE", once, $"records={records.Count} deaths={died.Count}");
        ForestTree anyDead = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).First(t => t.IsBiologicallyDead);
        int recordsNow = manager.DeadwoodRecords.Count;
        bool second = anyDead.ApplyMortality("self-thinning", e.EcologicalYear);
        Pass("NO_DUPLICATE_DEATH", !second && manager.DeadwoodRecords.Count == recordsNow, $"secondApply={second} recordsUnchanged={manager.DeadwoodRecords.Count == recordsNow}");
        Pass("BOUNDARY_MAXIMUM_LINE", maxRdAfter <= SitkaGrowthModel.MaximumRelativeDensity + 1e-6, $"maxRDAfterMortality={F(maxRdAfter, "0.0000")}");

        // Thinning lowers the suppression burden: same dense stand, 30% most-crowded removed at start.
        DenseStand(14, 24f, 10f);
        int unthinnedDeaths = 0, thinnedDeaths = 0;
        for (int y = 0; y < 10; y++) { e.AdvanceOneYear(); unthinnedDeaths += e.LastAdultMortality.Year == e.EcologicalYear ? e.LastAdultMortality.Deaths : 0; yield return null; }
        DenseStand(14, 24f, 10f);
        ForestTree[] all = Living();
        e.BeginChangeBatch();
        foreach (ForestTree t in all.OrderByDescending(t => e.GetCompetitionIndex(t)).ThenBy(t => t.TreeId, StringComparer.Ordinal).Take(all.Length * 3 / 10)) t.Fell();
        e.EndChangeBatch();
        for (int y = 0; y < 10; y++) { e.AdvanceOneYear(); thinnedDeaths += e.LastAdultMortality.Year == e.EcologicalYear ? e.LastAdultMortality.Deaths : 0; yield return null; }
        Pass("THINNING_LOWERS_MORTALITY", thinnedDeaths < unthinnedDeaths, $"unthinned={unthinnedDeaths} thinned30pct={thinnedDeaths}");

        // Save/load determinism through a dying stand (deaths + deadwood restored, continuation identical).
        DenseStand(14, 24f, 10f);
        for (int y = 0; y < 3; y++) { e.AdvanceOneYear(); yield return null; }
        string mid = JsonUtility.ToJson(saves.CaptureData());
        for (int y = 0; y < 5; y++) { e.AdvanceOneYear(); yield return null; }
        string uninterrupted = WorldHash();
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(mid), false), "mid load");
        yield return null;
        for (int y = 0; y < 5; y++) { e.AdvanceOneYear(); yield return null; }
        string resumed = WorldHash();
        Pass("SAVE_LOAD_DETERMINISTIC", uninterrupted == resumed, $"uninterrupted={uninterrupted} resumed={resumed}");
    }

    // ---------- anchors ----------

    private IEnumerator Anchors()
    {
        MethodInfo hashMethod = typeof(ScenarioOneInteractionGate).GetMethod("LifecycleHash", BindingFlags.Static | BindingFlags.NonPublic);
        float scenarioPressure = manager.Definition.BackgroundBrowsePressure;
        var results = new Dictionary<string, string>();
        foreach ((string label, int rng, int regen, int growth) in new[] { ("legacy", 0, 0, 0), ("regen1", 1, 1, 0), ("growth1", 1, 1, 1), ("growth1_repeat", 1, 1, 1) })
            foreach (float pressure in new[] { 0f, scenarioPressure })
            {
                ForestStandScenarios.ApplyLifecycleFixture();
                e.RngModelVersion = rng; e.RegenerationModelVersion = regen; e.GrowthModelVersion = growth;
                e.Browsing.BackgroundPressure = pressure; e.Browsing.ClearProtection();
                for (int year = 0; year < 80; year++) { e.AdvanceOneYear(); if (year % 10 == 9) yield return null; }
                results[label + (pressure == 0f ? "_neutral" : "_normal")] = (string)hashMethod.Invoke(null, new object[] { e });
            }
        e.Browsing.BackgroundPressure = scenarioPressure;
        Pass("LEGACY_ANCHORS_GROWTH0", results["legacy_neutral"] == Neutral0 && results["legacy_normal"] == Normal0,
            $"neutral={results["legacy_neutral"]} normal={results["legacy_normal"]}");
        Pass("REGEN1_ANCHORS_GROWTH0", results["regen1_neutral"] == RegenNeutral1 && results["regen1_normal"] == RegenNormal1,
            $"neutral={results["regen1_neutral"]} normal={results["regen1_normal"]}");
        Pass("GROWTH1_DETERMINISTIC", results["growth1_neutral"] == results["growth1_repeat_neutral"] && results["growth1_normal"] == results["growth1_repeat_normal"],
            $"neutral={results["growth1_neutral"]} normal={results["growth1_normal"]}");
        Debug.Log($"GROWTH_MODEL_ANCHORS_GROWTH1 neutral={results["growth1_neutral"]} normal={results["growth1_normal"]}");
    }

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        stand = FindFirstObjectByType<ForestStartingStand>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        sitka = spawner.ResolveSpecies("sitka-spruce");
        original = saves.CaptureData();
        yield return Policy();
        yield return SiteHeight();
        yield return Mortality();
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
        Debug.Log(ok ? $"SITKA_GROWTH_MODEL_VERIFY_PASS checks={passed}" : $"SITKA_GROWTH_MODEL_VERIFY_FAIL {failure} checks=[{string.Join(" | ", failures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }
}
