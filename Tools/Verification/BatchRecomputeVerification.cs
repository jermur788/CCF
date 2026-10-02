using System;
using System.Collections;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable gate: a batched felling run must leave the same cell light,
// canopy and seed rain as per-felling rebuilds, and rebuild only once.
// Copy into Assets/ForestPrototype, run BatchRecomputeVerification.Begin in
// Editor batchmode, then remove the copy and its .meta.
public static class BatchRecomputeVerification
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
        new GameObject("Batch Recompute Verification").AddComponent<BatchRecomputeVerificationRunner>();
    }
}

public sealed class BatchRecomputeVerificationRunner : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        string failure = null;
        try { Verify(); } catch (Exception e) { failure = e.ToString(); }
        if (failure == null) Debug.Log("BATCH_RECOMPUTE_VERIFY_PASS");
        else Debug.LogError("BATCH_RECOMPUTE_VERIFY_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private static string Snapshot(ForestEcologyController ecology)
    {
        var sb = new System.Text.StringBuilder();
        foreach (ForestEcologyCell c in ecology.Cells)
            sb.Append(c.Canopy.ToString("R")).Append(',').Append(c.Light.ToString("R")).Append(',')
              .Append(string.Join("|", c.Regeneration.Select(r => r.SpeciesId + ":" + r.SeedRain.ToString("R")))).Append(',').Append(c.RecentOpening.ToString("R")).Append(';');
        return sb.ToString();
    }

    private void Verify()
    {
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        if (ecology == null || saves == null) throw new InvalidOperationException("controllers missing");
        ecology.AdvanceOneYear();
        ForestSaveData start = saves.CaptureData();
        string[] ids = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(t => !t.IsStump).Select(t => t.TreeId).OrderBy(i => i, StringComparer.Ordinal).Take(6).ToArray();
        if (ids.Length < 6) throw new InvalidOperationException("too few trees");

        Fell(ids);
        string perFelling = Snapshot(ecology);

        if (!saves.LoadData(start, false)) throw new InvalidOperationException("restore rejected");
        ecology.BeginChangeBatch();
        Fell(ids);
        string during = Snapshot(ecology);
        ecology.EndChangeBatch();
        string batched = Snapshot(ecology);

        if (during == batched) throw new InvalidOperationException("batch did not defer the rebuild");
        if (perFelling != batched) throw new InvalidOperationException("batched result differs from per-felling rebuilds");

        // Nested batches flush only at the outermost end; an empty batch is a no-op.
        saves.LoadData(start, false);
        ecology.BeginChangeBatch(); ecology.BeginChangeBatch();
        Fell(ids);
        ecology.EndChangeBatch();
        if (Snapshot(ecology) == batched) throw new InvalidOperationException("inner EndChangeBatch flushed early");
        ecology.EndChangeBatch();
        if (Snapshot(ecology) != batched) throw new InvalidOperationException("nested batch result differs");
        ecology.EndChangeBatch(); // unbalanced end must be harmless

        // Shared Hegyi term equals the original expression.
        float a = 12.3f, t = 0.4f, d = 0.2f;
        if (ForestEcologyController.HegyiTerm(a, t, d) != (a / Mathf.Max(1f, t)) / Mathf.Max(0.5f, d))
            throw new InvalidOperationException("HegyiTerm differs from original expression");
    }

    private static void Fell(string[] ids)
    {
        foreach (string id in ids)
            FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                .First(t => t.TreeId == id).Fell();
    }
}
