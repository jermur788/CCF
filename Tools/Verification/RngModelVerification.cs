#if UNITY_EDITOR
using System;
using UnityEngine;

// Disposable RNG-model gate. Copy into Assets/ForestPrototype, run
// RngModelVerification.Begin in Editor batchmode (no play mode needed), then
// remove the copy and its .meta.
public static class RngModelVerification
{
    public static void Begin()
    {
        try
        {
            // Model 0 must equal the original expressions exactly.
            for (int seed = -5; seed < 50; seed += 7)
                for (int year = 0; year < 200; year++)
                {
                    Check(SimulationRandom.Seed(0, seed, year, 0) == (unchecked(seed * 397) ^ year), "legacy year seed");
                    Check(SimulationRandom.Seed(0, seed, year, 12345) == unchecked((seed * 397) ^ year ^ 12345), "legacy species seed");
                }
            Check(LegacyRoll("J0042", 17, 20260914) == SimulationRandom.Roll(0, "J0042", 17, 20260914), "legacy roll");

            // Old saves have no field; JsonUtility must leave it at 0.
            Check(JsonUtility.FromJson<ForestSaveData>("{\"version\":13}").rngModelVersion == 0, "absent field is not legacy");

            float legacy = Lag1(model => Year(model, 20260914));
            float mixed = Lag1(model => Year(model, 20260914), 1);
            float rollLegacy = MedianRollLag1(0), rollMixed = MedianRollLag1(1);
            Debug.Log($"RNG_DETAILS mastLag1 legacy={legacy:0.000} mixed={mixed:0.000} rollLag1 legacy={rollLegacy:0.000} mixed={rollMixed:0.000}");
            Check(Mathf.Abs(mixed) < 0.1f, "mixed mast draws still correlated");
            Check(Mathf.Abs(rollMixed) < 0.1f, "mixed survival rolls still correlated");
            float mean = 0; for (int i = 0; i < 20000; i++) mean += SimulationRandom.Roll(1, "J" + i, 5, 1);
            mean /= 20000; Check(mean > 0.48f && mean < 0.52f, "mixed roll not uniform: " + mean);
            Debug.Log("RNG_MODEL_VERIFY_PASS");
            UnityEditor.EditorApplication.Exit(0);
        }
        catch (Exception e) { Debug.LogError("RNG_MODEL_VERIFY_FAIL: " + e.Message); UnityEditor.EditorApplication.Exit(1); }
    }

    private static double Year(int model, int seed) { return 0; }
    private static float LegacyRoll(string id, int year, int seed)
    {
        uint hash = 2166136261u;
        foreach (char c in id) hash = (hash ^ c) * 16777619u;
        hash = (hash ^ (uint)year) * 16777619u;
        hash = (hash ^ (uint)seed) * 16777619u;
        return (hash & 0xFFFFFFu) / 16777216f;
    }

    private static float Lag1(Func<int, double> unused, int model = 0)
    {
        var xs = new double[400];
        for (int y = 0; y < xs.Length; y++) xs[y] = SimulationRandom.Create(model, 20260914, y + 1, 0).NextDouble();
        return Corr(xs);
    }

    private static float MedianRollLag1(int model)
    {
        var rs = new float[201];
        for (int i = 0; i < rs.Length; i++)
        {
            var xs = new double[200];
            for (int y = 0; y < xs.Length; y++) xs[y] = SimulationRandom.Roll(model, "J" + i.ToString("D4"), y + 1, 20260914);
            rs[i] = Corr(xs);
        }
        Array.Sort(rs); return rs[rs.Length / 2];
    }

    private static float Corr(double[] x)
    {
        int n = x.Length - 1; double mx = 0, my = 0;
        for (int i = 0; i < n; i++) { mx += x[i]; my += x[i + 1]; }
        mx /= n; my /= n; double sxy = 0, sxx = 0, syy = 0;
        for (int i = 0; i < n; i++) { sxy += (x[i] - mx) * (x[i + 1] - my); sxx += (x[i] - mx) * (x[i] - mx); syy += (x[i + 1] - my) * (x[i + 1] - my); }
        return (float)(sxy / Math.Sqrt(sxx * syy));
    }

    private static void Check(bool c, string m) { if (!c) throw new InvalidOperationException(m); }
}
#endif
