using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable gate for the adopted C8 growth/competition and k10a10 canopy/light
// calibration (Docs/EcologyCalibrationAdoption.md). Unlike the calibration
// harness on task/sitka-growth-competition-calibration and
// task/canopy-light-calibration, nothing here is a shadow copy: every stand is
// stepped by production AdvanceOneYear and light is read from production cells.
// Stands come from the real Scenario One generator (ForestStartingStand) with
// only its size fields changed on the play-mode instance; nothing is saved.
//
// The gate checks production against the calibration reports' figures (same
// generator, same treatments) within small tolerances, the 40 m edge-bias
// reduction, the treatment light ordering, and that no tree dies
// automatically. A measurement hash is logged for cross-process comparison.
// Copy into Assets/ForestPrototype, run
// EcologyCalibrationAdoptionVerification.Begin in Editor batchmode, read
// ECOCAL_* lines, then remove the copy and its .meta.
public static class EcologyCalibrationAdoptionVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif
    public static bool Requested =>
        Array.Exists(Environment.GetCommandLineArgs(), a => a.EndsWith("EcologyCalibrationAdoptionVerification.Begin", StringComparison.Ordinal));

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Requested)
            new GameObject("Ecology calibration adoption gate").AddComponent<EcologyCalibrationAdoptionGate>();
    }
}

public sealed class EcologyCalibrationAdoptionGate : MonoBehaviour
{
    private const int Years = 10;
    private const float InteriorEdge = 20f;

    private ForestEcologyController ecology;
    private ForestStartingStand generator;
    private readonly StringBuilder measured = new StringBuilder();

    private sealed class Run
    {
        public Dictionary<string, float> Dbh0 = new Dictionary<string, float>();
        public Dictionary<string, float> Dbh10 = new Dictionary<string, float>();
        public Dictionary<string, float> Growth1 = new Dictionary<string, float>();
        public Dictionary<string, float> Edge = new Dictionary<string, float>();
        public Dictionary<int, float> MeanLight = new Dictionary<int, float>();
        public HashSet<string> QTrees;
        public int Recruits, Dead;
    }

    private static void Check(bool ok, string message)
    {
        if (!ok) throw new InvalidOperationException(message);
    }

    private IEnumerator Start()
    {
        yield return null;
        string failure = null;
        IEnumerator run = Verify();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception e) { failure = e.ToString(); break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("ECOLOGY_CALIBRATION_ADOPTION_VERIFY_PASS");
        else Debug.LogError("ECOLOGY_CALIBRATION_ADOPTION_VERIFY_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Verify()
    {
        ecology = FindFirstObjectByType<ForestEcologyController>();
        generator = FindFirstObjectByType<ForestStartingStand>();
        TreeSpeciesDefinition sitka = ecology.ResolveSpecies();
        Check(ecology != null && generator != null && sitka != null, "scene setup");

        // 1. The adopted values are what production actually uses.
        Check(ForestEcologyController.HegyiCutoffMeters == 8f, "Hegyi cutoff is not 8 m");
        Check(ForestEcologyController.CanopyShadeReachPerCrownRadius == 1f, "canopy shade reach is not 1.0 x crown radius");
        Check(sitka.SpeciesId == "sitka-spruce" && sitka.Ci50 == 5f && sitka.PotentialDbhGrowthCmPerYear == 1.2f,
            $"Sitka Ci50/potential are {sitka.Ci50}/{sitka.PotentialDbhGrowthCmPerYear}");
        Emit($"ECOCAL_PARAMETERS hegyiCutoff={ForestEcologyController.HegyiCutoffMeters} shadeReach={ForestEcologyController.CanopyShadeReachPerCrownRadius}xCrown+halfCell sitkaCi50={sitka.Ci50} sitkaPotential={sitka.PotentialDbhGrowthCmPerYear} rngModel={ecology.RngModelVersion}");

        // 2. 80 m bounded stand, the primary calibration environment.
        var runs = new Dictionary<string, Run>();
        foreach (string treatment in new[] { "control", "releaseQ", "releaseM", "releaseH" })
        {
            IEnumerator r = Simulate(80f, treatment, runs);
            while (r.MoveNext()) yield return r.Current;
        }
        Run control = runs["control"];

        float interiorGrowth = control.Growth1.Where(p => control.Edge[p.Key] >= InteriorEdge).Average(p => p.Value);
        float wholeGrowth = control.Growth1.Values.Average();
        float q = Response(control, runs["releaseQ"], runs["releaseQ"].QTrees, false);
        float m = Response(control, runs["releaseM"], null, false);
        float h = Response(control, runs["releaseH"], null, false);
        float hInterior = Response(control, runs["releaseH"], null, true);
        Emit($"ECOCAL_GROWTH_80 unthinnedYear1 interior={interiorGrowth:F4} whole={wholeGrowth:F4} | 10-year increment response qTrees={q:F3} moderate={m:F3} heavy={h:F3} heavyInterior={hInterior:F3}");
        foreach (var pair in runs)
            Emit($"ECOCAL_LIGHT_80 treatment={pair.Key} meanLight y0={pair.Value.MeanLight[0]:F4} y1={pair.Value.MeanLight[1]:F4} y5={pair.Value.MeanLight[5]:F4} y10={pair.Value.MeanLight[10]:F4} recruits={pair.Value.Recruits} biologicallyDead={pair.Value.Dead}");

        // Calibration report figures (shadow model at 4b721ff / c648d0b, same
        // generator and treatments). Production runs the full annual step, so
        // small differences could only come from recruitment.
        Near(interiorGrowth, 0.3566f, 0.02f, "unthinned interior DBH growth");
        Near(wholeGrowth, 0.3877f, 0.02f, "unthinned whole-stand DBH growth");
        Near(q, 1.171f, 0.02f, "Q-tree 10-year increment response");
        Near(m, 1.558f, 0.02f, "moderate-release 10-year increment response");
        Near(h, 1.739f, 0.02f, "heavy-release 10-year increment response");
        Near(hInterior, 1.844f, 0.02f, "heavy-release interior 10-year increment response");
        NearAbs(runs["control"].MeanLight[0], 0.011f, "control light y0");
        NearAbs(runs["control"].MeanLight[10], 0.043f, "control light y10");
        NearAbs(runs["releaseQ"].MeanLight[10], 0.061f, "Q-tree light y10");
        NearAbs(runs["releaseM"].MeanLight[5], 0.148f, "moderate light y5");
        NearAbs(runs["releaseM"].MeanLight[10], 0.163f, "moderate light y10");
        NearAbs(runs["releaseH"].MeanLight[0], 0.210f, "heavy light y0");
        NearAbs(runs["releaseH"].MeanLight[5], 0.254f, "heavy light y5");
        NearAbs(runs["releaseH"].MeanLight[10], 0.257f, "heavy light y10");
        foreach (int y in new[] { 0, 5, 10 })
            Check(runs["control"].MeanLight[y] < 0.05f
                  && runs["control"].MeanLight[y] < runs["releaseQ"].MeanLight[y]
                  && runs["releaseQ"].MeanLight[y] < runs["releaseM"].MeanLight[y]
                  && runs["releaseM"].MeanLight[y] < runs["releaseH"].MeanLight[y],
                $"treatment light ordering broken at year {y}");
        Check(runs.Values.All(r => r.Dead == 0), "a tree died without an explicit mortality call");

        // 3. 40 m edge bias against the 80 m interior: about 1.41x before C8.
        var small = new Dictionary<string, Run>();
        IEnumerator s = Simulate(40f, "control", small);
        while (s.MoveNext()) yield return s.Current;
        float bias = small["control"].Growth1.Values.Average() / interiorGrowth;
        Emit($"ECOCAL_EDGE_BIAS u40WholeYear1={small["control"].Growth1.Values.Average():F4} u80Interior={interiorGrowth:F4} ratio={bias:F3}");
        Check(bias < 1.2f, "40 m edge bias did not fall under C8: " + bias);

        Debug.Log($"ECOCAL_MEASUREMENT_HASH {Fnv64(measured.ToString())}");
    }

    private IEnumerator Simulate(float size, string treatment, Dictionary<string, Run> runs)
    {
        BuildStand(size);
        yield return null;
        var run = new Run();
        List<ForestTree> living = Living();
        if (treatment == "releaseQ") run.QTrees = new HashSet<string>(LargestPerBlock(size, living, 10f).Select(t => t.TreeId));
        HashSet<string> felled = treatment == "control" ? new HashSet<string>()
            : treatment == "releaseQ" ? QHalo(living, run.QTrees)
            : KeepLargest(size, living, treatment == "releaseM" ? 10f / 3f : 4f);
        ecology.BeginChangeBatch();
        foreach (ForestTree t in living)
            if (felled.Contains(t.TreeId)) t.Fell();
        ecology.EndChangeBatch();

        List<ForestTree> retained = Living();
        foreach (ForestTree t in retained)
        {
            run.Dbh0[t.TreeId] = t.Diameter;
            run.Edge[t.TreeId] = size * 0.5f - Mathf.Max(Mathf.Abs(t.transform.position.x), Mathf.Abs(t.transform.position.z));
        }
        run.MeanLight[0] = ecology.Cells.Average(c => c.Light);
        for (int year = 1; year <= Years; year++)
        {
            ecology.AdvanceOneYear();
            if (year == 1)
                foreach (ForestTree t in retained) run.Growth1[t.TreeId] = ecology.GetAnnualDbhGrowth(t);
            if (year == 1 || year == 5 || year == 10) run.MeanLight[year] = ecology.Cells.Average(c => c.Light);
            yield return null;
        }
        foreach (ForestTree t in retained) run.Dbh10[t.TreeId] = t.Diameter;
        ForestTree[] all = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        run.Recruits = all.Count(t => !t.IsStump && !run.Dbh0.ContainsKey(t.TreeId));
        run.Dead = all.Count(t => t.IsBiologicallyDead);
        runs[treatment] = run;
    }

    // Mean 10-year DBH increment of the subject trees, released / unthinned.
    private static float Response(Run control, Run released, HashSet<string> subject, bool interiorOnly)
    {
        var ids = released.Dbh0.Keys.Where(id => (subject == null || subject.Contains(id))
            && (!interiorOnly || released.Edge[id] >= InteriorEdge)).ToList();
        float rel = ids.Average(id => released.Dbh10[id] - released.Dbh0[id]);
        float ctl = ids.Average(id => control.Dbh10[id] - control.Dbh0[id]);
        return rel / ctl;
    }

    private void Near(float actual, float expected, float relTolerance, string what)
    {
        Check(Mathf.Abs(actual / expected - 1f) <= relTolerance, $"{what}: {actual:F4} vs calibration {expected:F4}");
    }

    private void NearAbs(float actual, float expected, string what)
    {
        Check(Mathf.Abs(actual - expected) <= 0.005f, $"{what}: {actual:F4} vs calibration {expected:F4}");
    }

    private void Emit(string line)
    {
        measured.Append(line).Append('\n');
        Debug.Log(line);
    }

    // ---------- stand generation (same recipe as the calibration reports) ----------

    private void BuildStand(float size)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        Field(typeof(ForestEcologyController), "standSizeMeters").SetValue(ecology, size);
        ecology.ResetForDeterministicRun();
        float spacing = (float)Field(typeof(ForestStartingStand), "latticeSpacingMeters").GetValue(generator);
        Field(typeof(ForestStartingStand), "standWidthMeters").SetValue(generator, size);
        Field(typeof(ForestStartingStand), "standDepthMeters").SetValue(generator, size);
        Field(typeof(ForestStartingStand), "latticePerAxis").SetValue(generator, Mathf.FloorToInt((size - 2f) / spacing) + 1);
        Field(typeof(ForestStartingStand), "targetTreeCount").SetValue(generator, Mathf.RoundToInt(2100f * size * size / 10000f));
        Field(typeof(ForestStartingStand), "pathCorridorRadiusMeters").SetValue(generator, 0f);
        Field(typeof(ForestStartingStand), "clearingRadiusMeters").SetValue(generator, 0f);
        generator.Generate();
    }

    private static FieldInfo Field(Type type, string name) =>
        type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new MissingFieldException(type.Name, name);

    private static List<ForestTree> Living() =>
        FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => !t.IsStump && t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    private static List<ForestTree> LargestPerBlock(float size, List<ForestTree> living, float block)
    {
        float origin = -size * 0.5f;
        return living
            .GroupBy(t => (Mathf.FloorToInt((t.transform.position.x - origin) / block),
                           Mathf.FloorToInt((t.transform.position.z - origin) / block)))
            .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First())
            .ToList();
    }

    private static HashSet<string> QHalo(List<ForestTree> living, HashSet<string> qTrees)
    {
        var q = living.Where(t => qTrees.Contains(t.TreeId)).Select(t => new Vector2(t.transform.position.x, t.transform.position.z)).ToList();
        return new HashSet<string>(living.Where(t => !qTrees.Contains(t.TreeId)
            && q.Any(p => Vector2.Distance(p, new Vector2(t.transform.position.x, t.transform.position.z)) <= 2.5f)).Select(t => t.TreeId));
    }

    private static HashSet<string> KeepLargest(float size, List<ForestTree> living, float block)
    {
        var keep = new HashSet<string>(LargestPerBlock(size, living, block).Select(t => t.TreeId));
        return new HashSet<string>(living.Where(t => !keep.Contains(t.TreeId)).Select(t => t.TreeId));
    }

    private static string Fnv64(string text)
    {
        ulong hash = 14695981039346656037UL;
        foreach (char c in text) { hash ^= c; hash *= 1099511628211UL; }
        return hash.ToString("X16");
    }
}
