using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

// Staged only in a separate diagnostic Player, never the tester candidate.
public sealed class S1BStandalonePlayerSmoke : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
#if !UNITY_EDITOR
        if (Environment.GetCommandLineArgs().Contains("--ccf-smoke-save") || Environment.GetCommandLineArgs().Contains("--ccf-smoke-load"))
        {
            var host = new GameObject("S1-B diagnostic smoke"); DontDestroyOnLoad(host); host.AddComponent<S1BStandalonePlayerSmoke>();
        }
#endif
    }
    private string output;
    private IEnumerator Start()
    {
        var args = Environment.GetCommandLineArgs(); int i = Array.IndexOf(args, "--ccf-smoke-output");
        if (i < 0 || i + 1 >= args.Length) { Debug.LogError("S1B_SMOKE_FAIL missing output"); Application.Quit(1); yield break; }
        output = args[i + 1]; Directory.CreateDirectory(output);
        var run = Run(args.Contains("--ccf-smoke-save"));
        while (true)
        {
            bool more; object current = null;
            try { more = run.MoveNext(); if (more) current = run.Current; }
            catch (Exception e) { Debug.LogException(e); Debug.LogError("S1B_SMOKE_FAIL " + e.Message); File.WriteAllText(Path.Combine(output,"failure.txt"),e.ToString()); Application.Quit(1); yield break; }
            if (!more) yield break;
            yield return current;
        }
    }
    private static void Check(bool value, string message) { if (!value) throw new Exception(message); Debug.Log("S1B_SMOKE_CHECK " + message); }
    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame(); var image = ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(output,name+".png"),image.EncodeToPNG()); Destroy(image);
    }
    private IEnumerator Run(bool savePhase)
    {
        yield return new WaitForSecondsRealtime(3);
        var menu = FindFirstObjectByType<StandaloneSessionMenu>();
        Check(menu != null && StandaloneSessionMenu.IsOpen, "Initial session menu present");
        Check(menu.BuildIdentity.gitSha.Length == 40 && menu.BuildIdentity.buildId.Contains(menu.BuildIdentity.gitSha.Substring(0,7)), "Exact committed build identity");
        yield return Capture(savePhase ? "initial-menu-save" : "initial-menu-load");
        menu.Close(); yield return null; yield return null;
        var ui = FindFirstObjectByType<ScenarioOneUiRoot>(); var saves = FindFirstObjectByType<ForestSaveController>(); var manager = ui.Manager;
        ui.CloseHelp(); ui.CloseAll(); yield return null;
        Check(!StandaloneSessionMenu.IsOpen && FindFirstObjectByType<ForestPlayer>().enabled, "Forest control restored");
        if (savePhase)
        {
            Check(manager.CurrentEcologicalYear == 0, "Fresh Scenario One year zero");
            ui.ShowHelp(MenuHelpView.Menu.WalkingHud); Check(ui.Help.IsOpen,"S1-A Help opens"); yield return Capture("help"); ui.CloseHelp();
            ui.ShowObjectives(); yield return Capture("learning"); ui.CloseAll();
            ui.ShowMap(); yield return Capture("map"); ui.CloseHelp(); ui.CloseAll();
            var trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t=>t.IsLiving).OrderBy(t=>t.TreeId,StringComparer.Ordinal).ToArray();
            Check(trees.Length > 2,"Starting stand exists");
            ForestTreeMarkingManager.Instance.ToggleCropTree(trees[0]);
            ForestTreeMarkingManager.Instance.ToggleMark(trees[1]);
            ui.ShowWorkPlan(); ui.CloseHelp(); yield return Capture("work-plan");
            Check(manager.ApprovePendingWork(), "Chosen management work approved");
            Check(ui.AdvanceFromWorkPlan(), "First management cycle advanced"); yield return null; yield return null;
            Check(manager.CurrentEcologicalYear == 1 && manager.AnnualReports.Count == 1, "Annual report at year one");
            ui.CloseHelp(); yield return Capture("annual-review"); ui.AcknowledgeAnnualReview(); ui.CloseAll();
            yield return Capture("post-intervention-forest");
            Check(!trees[1].IsLiving,"Resolved felling changed chosen tree");
            saves.Save(); string path=Path.Combine(Application.persistentDataPath,"forest-save.json");
            Check(File.Exists(path),"Save file created");
            string hash=ScenarioReferenceArchive.WorldHash(saves.CaptureData()); File.WriteAllText(Path.Combine(output,"saved-world-hash.txt"),hash);
            File.WriteAllText(Path.Combine(output,"actual-save-path.txt"),path);
            File.WriteAllText(Path.Combine(output,"save-phase-pass.txt"),"Diagnostic Player API smoke; not human input/performance verification\n"+hash);
            Debug.Log("S1B_PLAYER_SAVE_PHASE_PASS hash="+hash);
        }
        else
        {
            saves.Load(); yield return null;
            string expected=File.ReadAllText(Path.Combine(output,"saved-world-hash.txt"));
            string actual=ScenarioReferenceArchive.WorldHash(saves.CaptureData());Check(actual==expected,"Same executable process-relaunch restores exact forest state");
            yield return Capture("loaded-forest");
            ui.ShowWorkPlan(); ui.CloseHelp();Check(ui.AdvanceFromWorkPlan(),"Continue ordinary advance after reload");ui.CloseHelp();ui.CloseAll();
            Check(manager.CurrentEcologicalYear==2,"Continued to year two");
            string path=Path.Combine(Application.persistentDataPath,"forest-save.json");var bytes=File.ReadAllBytes(path);
            menu.Open();Check(StandaloneSessionMenu.IsOpen && Time.timeScale==0,"Session menu pauses controls");menu.Restart();
            yield return new WaitForSecondsRealtime(3);
            Check(FindFirstObjectByType<ScenarioOneManager>().CurrentEcologicalYear==0,"New-test route resets forest");
            Check(File.ReadAllBytes(path).SequenceEqual(bytes),"New-test route retains save bytes");
            yield return Capture("fresh-test-after-restart");
            File.WriteAllText(Path.Combine(output,"load-reset-phase-pass.txt"),"Same diagnostic executable, process relaunch, load/continue/reset PASS; environment must be labelled separately.\n");
            Debug.Log("S1B_PLAYER_LOAD_RESET_PHASE_PASS");
        }
        menu.Quit();
    }
}
