#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class WindDisplayAudit
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("WindDisplayAudit.Begin"))
            new GameObject("Disposable current wind audit").AddComponent<WindDisplayAuditRunner>();
    }
}

public sealed class WindDisplayAuditRunner : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        Exception failure = null;
        try { Run(); }
        catch (Exception error) { failure = error; Debug.LogError("WIND_DISPLAY_AUDIT_FAIL " + error); }
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    private static string Number(float value) => value.ToString("R", CultureInfo.InvariantCulture);

    private void Run()
    {
        var ecology = FindFirstObjectByType<ForestEcologyController>();
        var saves = FindFirstObjectByType<ForestSaveController>();
        ForestSaveData original = saves.CaptureData();
        if (original.version != 18 || original.rngModelVersion != 1 || original.regenerationModel != 2 || original.growthModel != 1)
            throw new Exception("Current-main save/model authority differs from the packet expectation");
        var initialIds = new HashSet<string>(original.trees.Select(tree => tree.treeId));
        string directory = Environment.GetEnvironmentVariable("CCF_STORM_OUTPUT");
        Directory.CreateDirectory(directory);
        using (var writer = new StreamWriter(Path.Combine(directory, "current_wind_display.csv")))
        {
            writer.WriteLine("scenario_year,authored_stand_age,tree_id,authored_tree,dbh_cm,height_m,hd,light,recent_opening,diagnostic_index,display_label");
            for (int year = 0; year <= 40; year++)
            {
                if (year > 0) ecology.AdvanceOneYear();
                if (year != 0 && year != 10 && year != 20 && year != 40) continue;
                var trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(tree => tree.IsLiving).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
                foreach (ForestTree tree in trees)
                {
                    var cell = ecology.Cells[ecology.GetCellIndex(tree.transform.position)];
                    writer.WriteLine(string.Join(",", year, 20 + year, tree.TreeId, initialIds.Contains(tree.TreeId), Number(tree.Diameter), Number(tree.Height), Number(tree.Height / (tree.Diameter / 100f)), Number(cell.Light), Number(cell.RecentOpening), Number(ecology.GetWindRisk(tree)), ecology.GetWindRiskLabel(tree)));
                }
                Debug.Log("WIND_DISPLAY_DISTRIBUTION year=" + year + " authoredAge=" + (20 + year) + " living=" + trees.Length + " labels=" + string.Join(";", trees.GroupBy(tree => ecology.GetWindRiskLabel(tree)).Select(group => group.Key + "=" + group.Count())));
            }
        }
        if (!saves.LoadData(original, false)) throw new Exception("Could not restore audit fixture");
        Debug.Log("WIND_DISPLAY_AUDIT_PASS currentMain18_rng1_regen2_growth1=true checkpoints=0,10,20,40");
    }
}
#endif
