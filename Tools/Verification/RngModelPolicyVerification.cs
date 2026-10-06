#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Disposable gate for the Scenario One RNG policy (Docs/Scenario1RngModelCorrection.md):
// new games use RNG model 1; saves keep their recorded model; saves without the
// field and Reference Future v1 stay on legacy model 0. In-memory only: never
// writes the save slot. Copy into Assets/ForestPrototype, run
// RngModelPolicyVerification.Begin, then remove the copy and its .meta.
public static class RngModelPolicyVerification
{
    private const string Requested = "RngModelPolicyVerification.Requested";

    public static void Begin()
    {
        EditorPrefs.SetBool(Requested, true);
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!EditorPrefs.GetBool(Requested, false)) return;
        EditorPrefs.SetBool(Requested, false);
        new GameObject("RNG model policy gate").AddComponent<RngModelPolicyGate>();
    }
}

public sealed class RngModelPolicyGate : MonoBehaviour
{
    private const string Neutral0 = "BFC55473C1506067", Normal0 = "3485B6630C9EA448";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestSaveData original;

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }

    private IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null;
        IEnumerator run = Verify();
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        try { if (original != null) { manager.EndReferencePreview(); saves.LoadData(original, false); } }
        catch (Exception error) { failure = failure ?? error; }
        Debug.Log(failure == null ? "RNG_MODEL_POLICY_VERIFY_PASS" : "RNG_MODEL_POLICY_VERIFY_FAIL " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private IEnumerator Verify()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        Check(manager != null && ecology != null && saves != null, "scene systems missing");

        // TEST 1 — a fresh Scenario One session is a new game on model 1.
        original = saves.CaptureData();
        Check(ecology.RngModelVersion == SimulationRandom.MixedModel && original.rngModelVersion == 1
            && ScenarioOneManager.NewGameRngModel == 1, "new game is not RNG model 1: " + ecology.RngModelVersion);
        Debug.Log("RNG_POLICY_NEW_GAME_PASS model=" + ecology.RngModelVersion);

        // TEST 4 + 2 — lifecycle anchors: model 0 reproduces the accepted
        // history; model 1 is deterministic across repeated runs.
        string[] m0 = new string[2], m1a = new string[2], m1b = new string[2];
        IEnumerator l;
        l = Lifecycle(0, m0); while (l.MoveNext()) yield return l.Current;
        l = Lifecycle(1, m1a); while (l.MoveNext()) yield return l.Current;
        l = Lifecycle(1, m1b); while (l.MoveNext()) yield return l.Current;
        Check(m0[0] == Neutral0 && m0[1] == Normal0, $"model-0 compatibility anchors changed: {m0[0]} {m0[1]}");
        Check(m1a[0] == m1b[0] && m1a[1] == m1b[1], "model-1 lifecycle is not deterministic");
        Debug.Log($"RNG_POLICY_MODEL0_COMPATIBILITY_PASS neutral={m0[0]} normal={m0[1]}");
        Debug.Log($"RNG_POLICY_MODEL1_ANCHORS neutral={m1a[0]} normal={m1a[1]} repeat=identical");
        Check(saves.LoadData(Clone(original), false), "restore fresh world"); yield return null;

        // TEST 3 — year independence of the per-individual roll domain.
        foreach (int model in new[] { 0, 1 })
        {
            double r = LagOneCorrelation(model, 2000, 30);
            Debug.Log($"RNG_POLICY_LAG1 model={model} ids=2000 years=30 r={r.ToString("F4", Inv)}");
            if (model == 1) Check(Math.Abs(r) < 0.03, "model-1 year-to-year rolls still correlated: " + r);
        }

        // B5 — planted-juvenile survival under the production survival rule.
        TreeSpeciesDefinition sitka = Species("sitka-spruce"), oak = Species("sessile-oak");
        foreach ((TreeSpeciesDefinition s, float light, int years) in new[] { (sitka, 0.005f, 18), (oak, 0.15f, 18), (oak, 0.05f, 6) })
            foreach (int model in new[] { 0, 1 })
            {
                double p = JuvenileEcologyRules.SurvivalResponse(s, light);
                int n = 4000, alive = 0;
                for (int i = 0; i < n; i++)
                {
                    string id = "PJ" + (10000 + i).ToString(Inv);
                    bool ok = true;
                    for (int y = 1; y <= years && ok; y++)
                        ok = JuvenileEcologyRules.Survives(s, light, SimulationRandom.Roll(model, id, y, ecology.SimulationSeed));
                    if (ok) alive++;
                }
                double expected = Math.Pow(p, years), observed = alive / (double)n;
                double se = Math.Sqrt(Math.Max(expected * (1 - expected), 1e-9) / n);
                Debug.Log($"RNG_POLICY_PLANTED species={s.SpeciesId} light={light.ToString("F3", Inv)} annualP={p.ToString("F4", Inv)} years={years} n={n} model={model} expected={expected.ToString("F4", Inv)} observed={observed.ToString("F4", Inv)} zScore={((observed - expected) / se).ToString("F1", Inv)}");
                if (model == 1) Check(Math.Abs(observed - expected) <= 4 * se + 0.002, $"model-1 planted survival departs from the annual probability: {observed} vs {expected}");
            }

        // TESTS 5–7 — saves keep their model; an absent field loads as model 0.
        foreach (int model in new[] { 1, 0 })
        {
            ForestSaveData data = Clone(original); data.rngModelVersion = model;
            ecology.RngModelVersion = 1 - model;
            Check(saves.LoadData(data, false) && ecology.RngModelVersion == model, "save did not retain model " + model);
            yield return null;
            Check(saves.CaptureData().rngModelVersion == model, "recapture changed model " + model);
            Debug.Log($"RNG_POLICY_SAVE_RETAINS_PASS model={model}");
        }
        string json = JsonUtility.ToJson(original);
        string stripped = json.Replace("\"rngModelVersion\":1,", "");
        Check(stripped != json && !stripped.Contains("rngModelVersion"), "could not construct a field-less save");
        ecology.RngModelVersion = 1;
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(stripped), false) && ecology.RngModelVersion == 0,
            "save without rngModelVersion did not load as legacy model 0");
        Check(JsonUtility.FromJson<ForestSaveData>("{\"version\":13}").rngModelVersion == 0, "absent field default");
        Debug.Log("RNG_POLICY_LEGACY_ABSENT_FIELD_PASS model=0");
        yield return null;

        // TEST 8 — Reference Future v1 previews on model 0; returning restores the player's model.
        Check(saves.LoadData(Clone(original), false) && ecology.RngModelVersion == 1, "restore model-1 world");
        yield return null;
        foreach (int year in new[] { 20, 50, 100 })
        {
            Check(manager.TryBeginReferencePreview(year), "reference preview " + year);
            yield return null;
            Check(ecology.RngModelVersion == SimulationRandom.LegacyModel, $"Reference Future v1 year {year} is not model 0");
            manager.EndReferencePreview();
            yield return null;
            Check(ecology.RngModelVersion == 1, "player model not restored after preview " + year);
        }
        Debug.Log("RNG_POLICY_REFERENCE_V1_MODEL0_PASS years=20,50,100 playerModelRestored=1");

        // B9 — uninterrupted run equals save/load continuation, for both models,
        // through the real Scenario One plan path (planting with shelters).
        foreach (int model in new[] { 0, 1 })
        {
            IEnumerator c = Continuation(model);
            while (c.MoveNext()) yield return c.Current;
        }
    }

    private IEnumerator Lifecycle(int model, string[] hashes)
    {
        MethodInfo hashMethod = typeof(ScenarioOneInteractionGate).GetMethod("LifecycleHash", BindingFlags.Static | BindingFlags.NonPublic);
        Check(hashMethod != null, "lifecycle hash entry point missing");
        float scenarioPressure = manager.Definition.BackgroundBrowsePressure;
        int k = 0;
        foreach (float pressure in new[] { 0f, scenarioPressure })
        {
            ForestStandScenarios.ApplyLifecycleFixture();
            ecology.RngModelVersion = model;
            // RNG policy anchors are defined under legacy regeneration (model 0).
            ecology.RegenerationModelVersion = RegenerationModel.Legacy;
            ecology.GrowthModelVersion = GrowthModel.Legacy;
            ecology.Browsing.BackgroundPressure = pressure;
            ecology.Browsing.ClearProtection();
            for (int year = 0; year < 80; year++)
            {
                ecology.AdvanceOneYear();
                if (year % 10 == 9) yield return null;
            }
            hashes[k++] = (string)hashMethod.Invoke(null, new object[] { ecology });
        }
        ecology.Browsing.BackgroundPressure = scenarioPressure;
    }

    private IEnumerator Continuation(int model)
    {
        ForestSaveData start = Clone(original); start.rngModelVersion = model; start.regenerationModel = RegenerationModel.Legacy; start.growthModel = GrowthModel.Legacy;
        Check(saves.LoadData(start, false), "continuation start"); yield return null;
        Check(manager.TryPurchaseStock("sessile-oak-sapling", 12), "stock");
        int planted = 0;
        foreach (int cell in Enumerable.Range(0, ecology.CellCount).OrderByDescending(i => ecology.Cells[i].Light).ThenBy(i => i).Take(6))
        {
            Vector2 c = ecology.Cells[cell].Center;
            for (int k = 0; k < 9 && planted < 12; k++)
            {
                var p = new Vector3(c.x + (k % 3 - 1) * 0.9f, 0f, c.y + (k / 3 - 1) * 0.9f);
                bool shelter = planted % 2 == 0;
                if (manager.TryDesignateExactPlanting("sessile-oak-sapling", p,
                        shelter ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor, shelter))
                    planted++;
            }
        }
        Check(planted == 12 && manager.ApprovePendingWork(), "plant plan");
        for (int y = 0; y < 8; y++) { Check(manager.AdvanceYear(), "advance " + y); yield return null; }
        string mid = JsonUtility.ToJson(saves.CaptureData());
        for (int y = 0; y < 12; y++) { Check(manager.AdvanceYear(), "advance b" + y); if (y % 4 == 3) yield return null; }
        ForestSaveData endA = saves.CaptureData();
        string a = ScenarioReferenceArchive.WorldHash(endA);
        int aliveA = manager.PlantedJuveniles.Count(j => j.alive), promotedA = manager.PlantedJuveniles.Count(j => !string.IsNullOrEmpty(j.promotedTreeId));
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(mid), false) && ecology.RngModelVersion == model, "mid load");
        yield return null;
        for (int y = 0; y < 12; y++) { Check(manager.AdvanceYear(), "advance c" + y); if (y % 4 == 3) yield return null; }
        string b = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Check(a == b, $"model {model}: save/load continuation differs {a} vs {b}");
        Debug.Log($"RNG_POLICY_CONTINUATION_PASS model={model} year={ecology.EcologicalYear} hash={a} plantedAliveOrPromoted={aliveA}/{12} promoted={promotedA} cash={manager.CashCents}");
    }

    private static double LagOneCorrelation(int model, int ids, int years)
    {
        var xs = new List<double>(); var ys = new List<double>();
        for (int i = 0; i < ids; i++)
        {
            string id = "PJ" + (20000 + i).ToString(Inv);
            for (int y = 1; y < years; y++)
            {
                xs.Add(SimulationRandom.Roll(model, id, y, 20260914));
                ys.Add(SimulationRandom.Roll(model, id, y + 1, 20260914));
            }
        }
        double mx = xs.Average(), my = ys.Average(), cov = 0, vx = 0, vy = 0;
        for (int i = 0; i < xs.Count; i++) { cov += (xs[i] - mx) * (ys[i] - my); vx += (xs[i] - mx) * (xs[i] - mx); vy += (ys[i] - my) * (ys[i] - my); }
        return cov / Math.Sqrt(vx * vy);
    }

    private static TreeSpeciesDefinition Species(string id)
    {
        TreeSpeciesDefinition s = FindFirstObjectByType<ForestTreeSpawner>().ResolveSpecies(id);
        Check(s != null, "species " + id);
        return s;
    }

    private static ForestSaveData Clone(ForestSaveData data) => JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(data));
}
#endif
