#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable gate: real natural/planted render paths; no growth, RNG or saves.
public static class JuvenileDisplayHeightVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("JuvenileDisplayHeightVerification.Begin"))
            new GameObject("Juvenile display height gate").AddComponent<JuvenileDisplayHeightGate>();
    }
}
public sealed class JuvenileDisplayHeightGate : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        Exception failure = null;
        try { Verify(); }
        catch (Exception ex) { failure = ex; Debug.LogError("JUVENILE_DISPLAY_HEIGHT_VERIFY_FAIL " + ex); }
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }
    private static float Height(string name)
    {
        var visual = FindObjectsByType<Transform>(FindObjectsSortMode.None).Single(t => t.name == name);
        var renderers = visual.GetComponentsInChildren<Renderer>(true);
        return renderers.Max(r => r.bounds.max.y) - renderers.Min(r => r.bounds.min.y);
    }
    private static void Verify()
    {
        const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        var e = FindFirstObjectByType<ForestEcologyController>();
        var m = FindFirstObjectByType<ScenarioOneManager>();
        var s = FindFirstObjectByType<ForestTreeSpawner>();
        var visuals = FindFirstObjectByType<ScenarioHabitatVisuals>();
        var planted = (List<PlantedJuvenile>)typeof(ScenarioOneManager).GetField("plantedJuveniles", flags).GetValue(m);
        int cell = e.CellCount / 2;
        Vector2 centre = e.Cells[cell].Center;
        string output = Environment.GetEnvironmentVariable("CCF_DIAG_DIR");
        using (var w = new StreamWriter(Path.Combine(output,"juvenile_display_heights.csv")))
        {
            w.WriteLine("species,authoritative_height_m,refresh,natural_world_height_m,planted_world_height_m");
            foreach (string id in new[] { "sitka-spruce", "sessile-oak", "beech" })
            foreach (float target in new[] { .15f, .4f, .6f, 1.2f, 2f })
            {
                e.Cells[cell].ClearRegeneration();
                var species = s.ResolveSpecies(id);
                var band = new ForestRegenerationCohort(species);
                band.Restore(.3f,target,0);
                e.Cells[cell].InsertBand(band);
                planted.Clear();
                planted.Add(new PlantedJuvenile { juvenileId="height-fixture",speciesId=id,position=new Vector3(centre.x+1,0,centre.y),heightMeters=target,ageYears=1 });
                visuals.Rebuild(e,m.UnderstoreyCells,m.DeadwoodRecords,m.PlantedJuveniles);
                for (int repeat=0; repeat<3; repeat++)
                {
                    typeof(ForestEcologyController).GetMethod("SyncSeedlingVisuals",flags).Invoke(e,null);
                    float natural = Height(species.DisplayName+" seedling cell "+cell);
                    float exact = id == "sitka-spruce" ? natural : Height("Planted juvenile height-fixture");
                    if (Mathf.Abs(natural-target)>.002f || Mathf.Abs(exact-target)>.002f || Mathf.Abs(natural-exact)>.002f)
                        throw new Exception($"Height mismatch {id}: target={target}, natural={natural}, planted={exact}");
                    if (band.Height!=target || planted[0].heightMeters!=target || band.Density!=.3f)
                        throw new Exception("Display refresh mutated authoritative state");
                    w.WriteLine(string.Join(",", id,target.ToString("R",CultureInfo.InvariantCulture),repeat,natural.ToString("R",CultureInfo.InvariantCulture),(id == "sitka-spruce" ? "" : exact.ToString("R",CultureInfo.InvariantCulture))));
                }
            }
        }
        Debug.Log("JUVENILE_DISPLAY_HEIGHT_VERIFY_PASS naturalSpecies=3 plantedSpecies=2 heights=5 refreshes=3 cases=45 pairedCases=30 toleranceMetres=0.002 authoritativeStateUnchanged=true");
    }
}
#endif
