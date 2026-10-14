using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable in-Editor gate: Oak and Beech adult growth under ACTIVE growth models (audit gap F).
//
// The existing Oak/Beech gates pin GrowthModel.Legacy. This gate runs the same species under growth
// models 0, 1 and 2 and records and asserts the CURRENT contract, so a later change to Oak or Beech
// growth (Phase 2/3 if evidence is supplied) is a visible, deliberate difference and not an accident:
//   - height follows the species law  dH = potential x site x (1 - H/Max)  with no competition term;
//   - DBH follows the Hegyi response, so crowded trees grow less than isolated ones;
//   - neither depends on the growth-model number while no density mortality acts;
//   - deterministic replay and save/load continuation.
// Oak and Beech currently share every adult growth parameter; that is asserted and reported, not hidden.
//
// STATUS: written and type-checked in a code-only cloud session. NOT YET RUN IN UNITY.
//
// Copy into Assets/ForestPrototype, run with
//   CCF_STAND_GEOMETRY=0 <UNITY> -batchmode -nographics -executeMethod BroadleafAdultGrowthVerification.Begin
// then remove the copy and its .meta. Saves are captured/loaded in memory only.
public static class BroadleafAdultGrowthVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "BroadleafAdultGrowthVerification.Begin"))
            new GameObject("Broadleaf adult growth verification").AddComponent<BroadleafAdultGrowthVerificationRunner>();
    }
}

public sealed class BroadleafAdultGrowthVerificationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ForestEcologyController e;
    private ScenarioOneManager manager;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private ForestSaveData original;
    private TreeSpeciesDefinition sitka, oak, beech;
    private readonly List<string> failures = new List<string>();
    private int passed;

    private static string F(double v, string f = "0.###") => v.ToString(f, Inv);
    private void Pass(string name, bool ok, string detail)
    {
        if (ok) passed++; else failures.Add(name + " " + detail);
        Debug.Log($"BROADLEAF_GROWTH_{(ok ? "PASS" : "FAIL")} {name} {detail}");
    }
    private void Check(bool ok, string what) { if (!ok) { failures.Add(what); Debug.LogError("BROADLEAF_GROWTH_CHECK_FAIL " + what); } }
    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    private string TreeStateHash()
    {
        ulong hash = 14695981039346656037UL;
        void Bytes(byte[] values) { foreach (byte value in values) { hash ^= value; hash *= 1099511628211UL; } }
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).OrderBy(t => t.TreeId, StringComparer.Ordinal))
        {
            Bytes(System.Text.Encoding.UTF8.GetBytes(t.TreeId ?? "")); Bytes(new byte[] { (byte)(t.IsLiving ? 1 : 0) });
            Bytes(BitConverter.GetBytes(t.Diameter)); Bytes(BitConverter.GetBytes(t.Height)); Bytes(BitConverter.GetBytes(t.AgeYears));
        }
        return hash.ToString("X16");
    }

    // Fixture control: the nine grid trees are the only stems the question is about. After about 40 years of age the parents seed the
    // sparse stand and natural recruits promote into it; they would add competition (the closed-form law assumes CI 0 at 12 m
    // spacing), change Living() and re-order the probe tree. Each annual step therefore removes every stem that is not a grid tree.
    // This leaves the adult-growth checks exactly as written; it only stops the fixture being polluted.
    private HashSet<string> fixtureIds = new HashSet<string>();
    private int recruitsRemoved;

    private void Step()
    {
        e.AdvanceOneYear();
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (!fixtureIds.Contains(t.TreeId)) { recruitsRemoved++; DestroyImmediate(t.gameObject); }
    }

    // 3 x 3 grid. Spacing above the 8 m Hegyi cutoff gives every tree a competition index of exactly 0.
    private void Grid(int growth, TreeSpeciesDefinition s, float spacing, float dbh = 5.25f, float height = 3.5f, int age = 12)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = growth;
        string prefix = s == oak ? "O" : s == beech ? "B" : "S";
        fixtureIds = new HashSet<string>(Enumerable.Range(0, 9).Select(k => prefix + k.ToString("00")));
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                spawner.Spawn(prefix + (i * 3 + j).ToString("00"), s, new Vector3((i - 1) * spacing, 0f, (j - 1) * spacing), age, dbh, height, s.PotentialCrownRadiusM(dbh));
        e.InvalidateCompetition();
        e.RecomputeCanopy();
    }

    // The documented law, restated independently of the controller.
    private static (float h, float d) Law(TreeSpeciesDefinition s, float h, float d, float site, float ci)
    {
        float dg = s.PotentialDbhGrowthCmPerYear * site * Mathf.Clamp01(1f - d / s.MaxDbhCm) * (1f / (1f + ci / s.Ci50));
        float hg = s.PotentialHeightGrowthMPerYear * site * Mathf.Clamp01(1f - h / s.MaxHeightM);
        return (h + hg, d + dg);
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

        // Oak and Beech share every adult growth parameter today (audit finding C/D). Reported, so that a
        // future divergence is a recorded decision.
        bool shared = oak.PotentialDbhGrowthCmPerYear == beech.PotentialDbhGrowthCmPerYear && oak.MaxDbhCm == beech.MaxDbhCm
            && oak.PotentialHeightGrowthMPerYear == beech.PotentialHeightGrowthMPerYear && oak.MaxHeightM == beech.MaxHeightM && oak.Ci50 == beech.Ci50;
        Pass("OAK_BEECH_ADULT_PARAMETERS_REPORT", true,
            $"sharedAdultParameters={shared} dbhPot={oak.PotentialDbhGrowthCmPerYear} heightPot={oak.PotentialHeightGrowthMPerYear} maxH={oak.MaxHeightM} maxD={oak.MaxDbhCm} ci50={oak.Ci50}");

        foreach (TreeSpeciesDefinition s in new[] { oak, beech })
        {
            string label = s == oak ? "OAK" : "BEECH";
            var heights = new Dictionary<int, float[]>();
            var dbhs = new Dictionary<int, float[]>();
            var hashes = new Dictionary<int, string>();
            foreach (int growth in new[] { 0, 1, 2 })
            {
                Grid(growth, s, 12f);
                yield return null;
                var h = new float[61]; var d = new float[61];
                ForestTree probe = Living()[4];
                h[0] = probe.Height; d[0] = probe.Diameter;
                for (int y = 1; y <= 60; y++)
                {
                    Step();
                    h[y] = probe.Height; d[y] = probe.Diameter;
                    if (y % 10 == 0) yield return null;
                }
                heights[growth] = h; dbhs[growth] = d; hashes[growth] = TreeStateHash();
                Check(Living().Length == 9, $"{label} growth {growth}: sparse stand lost a tree or gained a recruit the fixture did not remove (living={Living().Length})");
            }
            // Closed-form law with ci = 0 and site = 1 (the production site scalar).
            float lh = 3.5f, ld = 5.25f; bool lawOk = true;
            for (int y = 1; y <= 60; y++)
            {
                (lh, ld) = Law(s, lh, ld, 1f, 0f);
                if (Mathf.Abs(lh - heights[1][y]) > 1e-3f || Mathf.Abs(ld - dbhs[1][y]) > 1e-3f) lawOk = false;
            }
            Pass($"{label}_FOLLOWS_SPECIES_LAW_UNDER_GROWTH1", lawOk, $"H20={F(heights[1][20], "0.00")} H40={F(heights[1][40], "0.00")} H60={F(heights[1][60], "0.00")} D60={F(dbhs[1][60], "0.0")} (ci 0, site 1)");
            bool sameAcrossModels = Enumerable.Range(0, 61).All(y => heights[0][y] == heights[1][y] && heights[1][y] == heights[2][y] && dbhs[0][y] == dbhs[1][y] && dbhs[1][y] == dbhs[2][y]);
            Pass($"{label}_GROWTH_INDEPENDENT_OF_GROWTH_MODEL_WHEN_NO_DENSITY", sameAcrossModels, $"hash0={hashes[0]} hash1={hashes[1]} hash2={hashes[2]}");
            bool monotone = Enumerable.Range(1, 60).All(y => heights[2][y] > heights[2][y - 1] && dbhs[2][y] > dbhs[2][y - 1] && heights[2][y] <= s.MaxHeightM);
            Pass($"{label}_MONOTONE_AND_BOUNDED", monotone, $"maxHeight={s.MaxHeightM}");

            // Competition lowers DBH, not height.
            Grid(2, s, 12f); yield return null;
            for (int y = 0; y < 30; y++) Step();
            ForestTree isolated = Living()[4];
            float isolatedD = isolated.Diameter, isolatedH = isolated.Height;
            Grid(2, s, 2.5f); yield return null;
            for (int y = 0; y < 30; y++) Step();
            ForestTree crowdedCentre = Living().FirstOrDefault(t => t.TreeId.EndsWith("04"));
            Pass($"{label}_COMPETITION_REDUCES_DBH_NOT_HEIGHT", crowdedCentre != null && crowdedCentre.Diameter < isolatedD - 0.05f && Mathf.Abs(crowdedCentre.Height - isolatedH) < 1e-3f,
                crowdedCentre == null ? "centre tree died" : $"isolated D={F(isolatedD)} H={F(isolatedH)} crowded D={F(crowdedCentre.Diameter)} H={F(crowdedCentre.Height)}");

            // Deterministic replay and save/load continuation.
            string first = null;
            for (int run = 0; run < 2; run++)
            {
                Grid(2, s, 3.5f); yield return null;
                for (int y = 0; y < 25; y++) Step();
                string hash = TreeStateHash();
                if (run == 0) first = hash; else Pass($"{label}_REPLAY_IDENTICAL", hash == first, $"hash={hash}");
            }
            Grid(2, s, 3.5f); yield return null;
            for (int y = 0; y < 10; y++) Step();
            string mid = JsonUtility.ToJson(saves.CaptureData());
            for (int y = 0; y < 15; y++) Step();
            string uninterrupted = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
            Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(mid), false), label + " mid load");
            yield return null;
            for (int y = 0; y < 15; y++) Step();
            string resumed = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
            Pass($"{label}_SAVE_LOAD_CONTINUATION", uninterrupted == resumed, $"uninterrupted={uninterrupted} resumed={resumed}");
            Pass($"{label}_FIXTURE_RECRUITS_REMOVED_REPORT", true, $"recruitsRemovedSoFar={recruitsRemoved} (natural regeneration promoted into the sparse stand and was removed each year; the checks above are unchanged)");
        }

        // Sitka under growth model 2 still follows the Class III envelope (the Sitka-only height gate is unchanged).
        Grid(2, sitka, 12f, 20f, 12f, 25); yield return null;
        ForestTree s0 = Living()[4]; float expected = SitkaGrowthModel.NextHeight(s0.Height, s0.AgeYears);
        Step();
        Pass("SITKA_HEIGHT_STILL_CLASS_III_UNDER_GROWTH2", Mathf.Abs(Living()[4].Height - expected) < 1e-4f, $"expected={F(expected, "0.0000")} got={F(Living()[4].Height, "0.0000")}");
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
        Debug.Log(ok ? $"BROADLEAF_GROWTH_VERIFY_PASS checks={passed}" : $"BROADLEAF_GROWTH_VERIFY_FAIL {failure} checks=[{string.Join(" | ", failures)}]");
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
#endif
    }
}
