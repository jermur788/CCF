using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable save-hardening and growth-cache verification. Copy into
// Assets/ForestPrototype, run SaveHardeningVerification.Begin in Editor
// batchmode, then remove the temporary Assets copy and generated .meta.
// Restores the user's save and backup files afterwards.
public static class SaveHardeningVerification
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
        new GameObject("Save Hardening Verification").AddComponent<SaveHardeningVerificationRunner>();
    }
}

public sealed class SaveHardeningVerificationRunner : MonoBehaviour
{
    private string savePath;
    private byte[] saveBackup;
    private byte[] bakBackup;

    private IEnumerator Start()
    {
        yield return null;
        Exception failure = null;
        IEnumerator verify = Verify();
        while (true)
        {
            bool more;
            object current = null;
            try
            {
                more = verify.MoveNext();
                if (more) current = verify.Current;
            }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (failure == null) Debug.Log("SAVE_HARDENING_VERIFY_PASS");
        else Debug.LogError("SAVE_HARDENING_VERIFY_FAIL: " + failure);
        if (savePath != null)
        {
            Restore(savePath, saveBackup);
            Restore(savePath + ".bak", bakBackup);
            if (File.Exists(savePath + ".tmp")) File.Delete(savePath + ".tmp");
        }
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private static void Restore(string path, byte[] original)
    {
        if (original != null) File.WriteAllBytes(path, original);
        else if (File.Exists(path)) File.Delete(path);
    }

    private static ForestTree[] LivingTrees()
    {
        return FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Where(tree => !tree.IsStump).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
    }

    private IEnumerator Verify()
    {
        ForestEcologyController ecology = FindFirstObjectByType<ForestEcologyController>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        Check(ecology != null && saves != null, "ecology or save controller missing");
        savePath = Path.Combine(Application.persistentDataPath, "forest-save.json");
        if (File.Exists(savePath)) saveBackup = File.ReadAllBytes(savePath);
        if (File.Exists(savePath + ".bak")) bakBackup = File.ReadAllBytes(savePath + ".bak");

        // 1. Growth readout survives a lazy competition refresh after a felling.
        ecology.AdvanceOneYear();
        ForestTree[] living = LivingTrees();
        Check(living.Length > 2, "too few living trees for the growth check");
        ForestTree watched = living[0];
        float growth = ecology.GetAnnualDbhGrowth(watched);
        Check(growth > 0f, "no annual growth recorded after advancing a year");
        living[living.Length - 1].Fell();
        ecology.GetCompetitionIndex(watched); // what the inspection card does
        Check(ecology.GetAnnualDbhGrowth(watched) == growth,
            "inspecting a tree after a felling erased last year's growth");
        ecology.AdvanceOneYear();
        Check(ecology.GetAnnualDbhGrowth(watched) > 0f, "annual step did not record new growth");

        // 2. Saves are swapped in atomically and keep the previous save as .bak.
        saves.Save();
        string firstSave = File.ReadAllText(savePath);
        int savedYear = ecology.EcologicalYear;
        int savedTrees = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
        ecology.AdvanceOneYear();
        saves.Save();
        Check(File.Exists(savePath + ".bak") && File.ReadAllText(savePath + ".bak") == firstSave,
            "second save did not keep the first as a .bak backup");
        Check(!File.Exists(savePath + ".tmp"), "temporary save file was left behind");

        // 3. A loaded save clears per-tree growth from the previous timeline.
        File.WriteAllText(savePath, firstSave);
        saves.Load();
        yield return null;
        Check(ecology.EcologicalYear == savedYear, "valid save did not load");
        Check(LivingTrees().All(tree => ecology.GetAnnualDbhGrowth(tree) == 0f),
            "growth from the pre-load timeline survived the load");

        // 4. Malformed and empty saves are rejected without touching the forest.
        File.WriteAllText(savePath, "{ this is not json");
        saves.Load();
        yield return null;
        AssertUnchanged(ecology, savedYear, savedTrees, "malformed JSON");

        File.WriteAllText(savePath, "{}");
        saves.Load();
        yield return null;
        AssertUnchanged(ecology, savedYear, savedTrees, "empty '{}' save");

        Check(!saves.LoadData(null, false), "null save data was accepted");
        ForestSaveData duplicated = JsonUtility.FromJson<ForestSaveData>(firstSave);
        duplicated.trees.Add(JsonUtility.FromJson<TreeSaveData>(JsonUtility.ToJson(duplicated.trees[0])));
        Check(!saves.LoadData(duplicated, false), "save with a duplicate tree ID was accepted");
        yield return null;
        AssertUnchanged(ecology, savedYear, savedTrees, "duplicate tree ID");

        Debug.Log($"SAVE_HARDENING_DETAILS growthKept={growth:0.0000} trees={savedTrees} year={savedYear}");
    }

    private static void AssertUnchanged(ForestEcologyController ecology, int year, int trees, string label)
    {
        Check(ecology.EcologicalYear == year, label + " changed the ecological year");
        int now = FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length;
        Check(now == trees, $"{label} changed the tree count ({trees} -> {now})");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
