using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable diagnostic (not a pass/fail gate): how much does the isolated
// 40 m stand understate competition and light for trees near its edge?
// A 100 m stand of the same density is generated; the central 40 m window is
// measured with its surroundings present, then again after every tree outside
// the window is removed (the isolated-patch situation). Same trees, same
// cells, same year; only the surrounding trees differ.
// Copy into Assets/ForestPrototype, run EdgeBiasDiagnostic.Begin in Editor
// batchmode, read EDGE_BIAS_* lines, then remove the copy and its .meta.
public static class EdgeBiasDiagnostic
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
        new GameObject("Edge Bias Diagnostic").AddComponent<EdgeBiasDiagnosticRunner>();
    }
}

public sealed class EdgeBiasDiagnosticRunner : MonoBehaviour
{
    private const float BigStand = 100f, Window = 40f, Spacing = 1.9f;

    private struct Sample { public float edgeDistance, ci; }

    private IEnumerator Start()
    {
        yield return null;
        string failure = null;
        IEnumerator run = Run();
        while (true)
        {
            bool more; object cur = null;
            try { more = run.MoveNext(); if (more) cur = run.Current; }
            catch (Exception e) { failure = e.ToString(); break; }
            if (!more) break;
            yield return cur;
        }
        if (failure != null) Debug.LogError("EDGE_BIAS_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private static uint Fnv(string s)
    {
        uint h = 2166136261u;
        foreach (char c in s) { h ^= c; h *= 16777619u; }
        return h;
    }
    private static float Unit(string s) { return (Fnv(s) & 0xFFFFFF) / 16777216f; }

    private IEnumerator Run()
    {
        var ecology = FindFirstObjectByType<ForestEcologyController>();
        var spawner = FindFirstObjectByType<ForestTreeSpawner>();
        if (ecology == null || spawner == null || spawner.DefaultSpecies == null) throw new InvalidOperationException("scene setup");
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Destroy(t.gameObject);
        yield return null;
        typeof(ForestEcologyController).GetField("standSizeMeters", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(ecology, BigStand);
        ecology.RebuildGrid();

        // Same recipe at any size: jittered lattice, 5% omitted, DBH classes by hash.
        int perAxis = Mathf.FloorToInt((BigStand - 2f) / Spacing) + 1;
        float origin = -(perAxis - 1) * Spacing * 0.5f;
        int spawned = 0;
        for (int r = 0; r < perAxis; r++)
        for (int c = 0; c < perAxis; c++)
        {
            string id = $"E{r:D3}{c:D3}";
            if (Unit("omit-" + id) < 0.05f) continue;
            float x = origin + c * Spacing + (Unit("jx-" + id) - 0.5f) * Spacing * 0.6f;
            float z = origin + r * Spacing + (Unit("jz-" + id) - 0.5f) * Spacing * 0.6f;
            float k = Unit("class-" + id), u = Unit("dbh-" + id);
            float dbh = k < 0.15f ? Mathf.Lerp(19f, 24f, u) : k < 0.70f ? Mathf.Lerp(15f, 19f, u) : Mathf.Lerp(8f, 14f, u);
            float height = 6f + dbh * 0.75f;
            var species = spawner.DefaultSpecies;
            spawner.Spawn(id, species, new Vector3(x, 0f, z), 20, dbh, height, species.PotentialCrownRadiusM(dbh));
            spawned++;
        }
        yield return null;
        Debug.Log($"EDGE_BIAS_SETUP stems={spawned} density/ha={spawned / (BigStand * BigStand / 10000f):F0}");

        // Measure the window with surroundings, then isolated, from identical state.
        List<Sample> full = Collect(ecology);
        float[] fullLight = WindowCells(ecology, c => c.Light);
        float[] fullRain = WindowCells(ecology, c => c.Regeneration.Sum(rc => rc.SeedRain));

        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (Mathf.Abs(t.transform.position.x) > Window * 0.5f || Mathf.Abs(t.transform.position.z) > Window * 0.5f)
                Destroy(t.gameObject);
        yield return null;
        List<Sample> iso = Collect(ecology);
        float[] isoLight = WindowCells(ecology, c => c.Light);
        float[] isoRain = WindowCells(ecology, c => c.Regeneration.Sum(rc => rc.SeedRain));

        Report(full, iso, ecology, fullLight, isoLight, fullRain, isoRain);
    }

    private List<Sample> Collect(ForestEcologyController ecology)
    {
        ecology.InvalidateCompetition();
        ecology.RecomputeCanopy();
        ecology.RecomputeSeedRain();
        var trees = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => !t.IsStump && Mathf.Abs(t.transform.position.x) <= Window * 0.5f
                && Mathf.Abs(t.transform.position.z) <= Window * 0.5f)
            .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
        var ci = trees.ToDictionary(t => t.TreeId, t => ecology.GetCompetitionIndex(t));
        return new List<Sample>(trees.Select(t => new Sample
        {
            edgeDistance = Window * 0.5f - Mathf.Max(Mathf.Abs(t.transform.position.x), Mathf.Abs(t.transform.position.z)),
            ci = ci[t.TreeId]
        }));
    }

    // Window cells in a fixed order, so the with/without passes line up.
    private float[] WindowCells(ForestEcologyController ecology, Func<ForestEcologyCell, float> read)
    {
        return WindowCellList(ecology).Select(read).ToArray();
    }

    private static List<ForestEcologyCell> WindowCellList(ForestEcologyController ecology)
    {
        return ecology.Cells.Where(c => Mathf.Abs(c.Center.x) <= Window * 0.5f && Mathf.Abs(c.Center.y) <= Window * 0.5f).ToList();
    }

    private static float CellEdge(ForestEcologyCell c)
    {
        return Window * 0.5f - Mathf.Max(Mathf.Abs(c.Center.x), Mathf.Abs(c.Center.y));
    }

    private static float Mean(IEnumerable<float> v) { var a = v.ToArray(); return a.Length == 0 ? 0f : a.Average(); }

    private void Report(List<Sample> full, List<Sample> iso, ForestEcologyController ecology,
        float[] fullLight, float[] isoLight, float[] fullRain, float[] isoRain)
    {
        if (full.Count != iso.Count) throw new InvalidOperationException("window tree sets differ");
        float[][] bands = { new[] { 0f, 5f }, new[] { 5f, 10f }, new[] { 10f, 15f }, new[] { 15f, 20.01f } };
        foreach (float[] b in bands)
        {
            var idx = Enumerable.Range(0, full.Count).Where(i => full[i].edgeDistance >= b[0] && full[i].edgeDistance < b[1]).ToList();
            float f = Mean(idx.Select(i => full[i].ci)), s = Mean(idx.Select(i => iso[i].ci));
            Debug.Log($"EDGE_BIAS_CI band={b[0]:0}-{Mathf.Min(b[1], 20f):0}m trees={idx.Count} withSurroundings={f:0.000} isolated={s:0.000} isolatedAsPctOfFull={(f > 0 ? 100f * s / f : 0f):0.0}");
        }
        List<ForestEcologyCell> cellList = WindowCellList(ecology);
        foreach (float[] b in bands)
        {
            var ci = Enumerable.Range(0, cellList.Count).Where(i => CellEdge(cellList[i]) >= b[0] && CellEdge(cellList[i]) < b[1]).ToList();
            Debug.Log($"EDGE_BIAS_CELLS band={b[0]:0}-{Mathf.Min(b[1], 20f):0}m cells={ci.Count} light full={Mean(ci.Select(i => fullLight[i])):0.000} isolated={Mean(ci.Select(i => isoLight[i])):0.000} seedRain full={Mean(ci.Select(i => fullRain[i])):0.000} isolated={Mean(ci.Select(i => isoRain[i])):0.000}");
        }
        Debug.Log($"EDGE_BIAS_LIGHT meanWindowCellLight withSurroundings={Mean(fullLight):0.000} isolated={Mean(isoLight):0.000}");
        Debug.Log($"EDGE_BIAS_SEEDRAIN meanWindowCellSeedRain withSurroundings={Mean(fullRain):0.000} isolated={Mean(isoRain):0.000}");
        Debug.Log("EDGE_BIAS_DONE");
    }
}
