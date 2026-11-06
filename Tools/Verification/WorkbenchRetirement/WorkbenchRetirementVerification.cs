using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable Tier-2 gate for #287 (D-070 workbench retirement). Stage with run_workbench_retirement.py, which removes the
// source and its .meta afterwards. All disk writes go to the isolated persistentDataPath of the runner's XDG_CONFIG_HOME.
// Every assertion is PASS/FAIL; a failure stops the run and the log keeps the evidence. Nothing here edits production code,
// the scene, Reference Future data or the player's real save.
public static class WorkbenchRetirementVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "WorkbenchRetirementVerification.Begin"))
            new GameObject("Disposable workbench retirement verification").AddComponent<WorkbenchRetirementVerificationRunner>();
    }
}

public sealed class WorkbenchRetirementVerificationRunner : MonoBehaviour
{
    private const string WorkbenchId = "workbench-01";
    private ForestEcologyController e;
    private ScenarioOneManager m;
    private ForestSaveController saves;
    private ForestStartingStand stand;
    private ScenarioOneUiRoot ui;
    private int checks;

    private static string SavePath => Path.Combine(Application.persistentDataPath, "forest-save.json");
    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) { Debug.Log("WORKBENCH_FAIL " + what + " [" + detail + "]"); throw new InvalidOperationException(what + " [" + detail + "]"); }
        checks++;
        Debug.Log("WORKBENCH_PASS " + what + (detail.Length == 0 ? "" : " [" + detail + "]"));
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        Debug.Log(failure == null ? "WORKBENCH_RETIREMENT_VERIFY_PASS checks=" + checks : "WORKBENCH_RETIREMENT_VERIFY_FAIL checks=" + checks + " " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private static List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
    private static ForestBuildable[] Buildables() => FindObjectsByType<ForestBuildable>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    private static ForestBuildable Workbench() => Buildables().Single(b => b.BuildId == WorkbenchId);
    private string Hash() => ScenarioReferenceArchive.CurrentWorldHash(saves.CaptureData());
    private static string Ids(ForestSaveData d) => string.Join(",", d.buildables.Select(b => b.buildId).OrderBy(x => x, StringComparer.Ordinal));
    private static string TreeSig(IEnumerable<ForestTree> trees) =>
        string.Join("|", trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).Select(t =>
            t.TreeId + "@" + t.transform.position.x.ToString("R") + "," + t.transform.position.z.ToString("R") + ":" + t.Diameter.ToString("R")));

    private void ResetWorld(int model)
    {
        foreach (ForestTree tree in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(tree.gameObject);
        e.ApplyStandGeometry(model);
        e.ResetForDeterministicRun();
        stand.Generate();
        m.InitializeNewScenario();
        e.InvalidateCompetition();
    }

    // Visible presentation of the workbench: any enabled renderer on an active object, any enabled collider.
    private static string Presentation(ForestBuildable b) =>
        "activeRenderers=" + b.GetComponentsInChildren<Renderer>(false).Count(r => r.enabled && r.gameObject.activeInHierarchy)
        + " enabledColliders=" + b.GetComponentsInChildren<Collider>(true).Count(c => c.enabled);

    // The coverage adapter's Site masks, read through the real private production method (read-only).
    private int SiteMasks()
    {
        MethodInfo read = typeof(ScenarioOneManager).GetMethod("ReadCoverageWorld", BindingFlags.Instance | BindingFlags.NonPublic);
        Check(read != null, "ReadCoverageWorld is reachable for the Site-mask count");
        object[] args = { 40f, null };
        var mapped = (ScenarioOneCoverageWorld.Mapped)read.Invoke(m, args);
        Check(mapped != null, "coverage world reads", args[1] as string);
        return mapped.Masks.Count(x => x.Kind == ScenarioOneCoverage.MaskKind.Site);
    }

    private IEnumerator Run()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        m = FindFirstObjectByType<ScenarioOneManager>();
        saves = FindFirstObjectByType<ForestSaveController>();
        stand = FindFirstObjectByType<ForestStartingStand>();
        ui = FindFirstObjectByType<ScenarioOneUiRoot>();
        Check(e != null && m != null && saves != null && stand != null, "the Scenario One ForestTest scene is loaded");
        Check(Application.persistentDataPath.Contains("/config/unity3d/") && !File.Exists(SavePath), "persistentDataPath is the runner's isolated config with no pre-existing save", Application.persistentDataPath);
        saves.enabled = false;   // no autosave path may write during the gate

        // ---- 0. the scene itself ----
        ForestBuildable[] all = Buildables();
        Check(all.Length == 7 && all.Count(b => b.RetiresFromStartingWorld) == 1 && Workbench().RetiresFromStartingWorld, "scene has seven buildables and exactly one carries the retirement policy: workbench-01");
        string expectedNonRetired = string.Join(",", all.Where(b => b.BuildId != WorkbenchId).Select(b => b.BuildId).OrderBy(x => x, StringComparer.Ordinal));
        Check(expectedNonRetired.Split(',').Length == 6, "independent expected inventory (every scene buildable except the literal id workbench-01) is six", expectedNonRetired);
        Debug.Log("WORKBENCH_INFO nonretired=" + expectedNonRetired);

        // ---- A. fresh Enlarged80 new game ----
        m.InitializeNewScenario();
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(e.StandGeometryModelVersion == 1 && Living().Count == 1344, "fresh Enlarged80 new game has 1,344 trees");
        ForestBuildable wb = Workbench();
        Check(wb.IsRetiredFromStartingWorld && !wb.IsBuilt && !wb.ConstructionAvailable, "new game: workbench-01 is retired, unbuilt, unavailable");
        Check(Presentation(wb) == "activeRenderers=0 enabledColliders=0", "new game: no visible renderer and no enabled collider (no ghost, no prompt target)", Presentation(wb));
        ForestSaveData newGame = saves.CaptureData();
        Check(Ids(newGame) == expectedNonRetired && newGame.buildables.Count(b => b.buildId == WorkbenchId) == 0, "new game capture omits workbench-01 and lists exactly the six nonretired buildables", Ids(newGame));
        Check(SiteMasks() == 6, "new game: six Site masks from the production coverage adapter");
        string newStartHash = Hash();
        string newStartTrees = TreeSig(Living());
        Debug.Log("WORKBENCH_NEW_ENLARGED80_START_CURRENT_HASH " + newStartHash);

        // Baseline equivalence: the same world with the workbench un-retired is the pre-#284 behaviour.
        Workbench().SetRetiredFromStartingWorld(false);
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(!Workbench().IsRetiredFromStartingWorld, "baseline-equivalent run really has the workbench active");
        string baselineHash = Hash();
        string baselineTrees = TreeSig(Living());
        Check(Ids(saves.CaptureData()).Contains(WorkbenchId) && SiteMasks() == 7, "baseline-equivalent world: seven buildables saved, seven Site masks");
        Check(baselineTrees == newStartTrees, "identical 1,344 tree identities, positions and diameters with and without the retired workbench (no tree moved)");
        Check(baselineHash != newStartHash, "world hash differs only because one buildable entry is omitted");
        Debug.Log("WORKBENCH_BASELINE_EQUIVALENT_ENLARGED80_START_CURRENT_HASH " + baselineHash + " (historical label 04A78188A6F8E6F9 is rack-model-0 fixture; compare, do not assume)");
        // Same buildable-only delta, proven by restoring the entry: baseline capture minus the workbench entry hashes to the new-game hash.
        ForestSaveData stripped = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(saves.CaptureData()));
        stripped.buildables.RemoveAll(b => b.buildId == WorkbenchId);
        Check(ScenarioReferenceArchive.CurrentWorldHash(stripped) == newStartHash, "baseline capture minus the one workbench-01 entry equals the new-game hash exactly (delta is that entry only)");

        // ---- return to a fresh retired new game for the lifecycle cases ----
        Workbench().SetRetiredFromStartingWorld(true);
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(Workbench().IsRetiredFromStartingWorld && Hash() == newStartHash && TreeSig(Living()) == newStartTrees, "new game regenerates identically and retired again (deterministic)");

        // ---- B. old saves keep the workbench ----
        foreach (bool built in new[] { false, true })
        {
            ForestSaveData old = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(newGame));
            old.buildables.Add(new BuildableSaveData { buildId = WorkbenchId, built = built });
            Check(saves.LoadData(old, false), "old save naming workbench-01 (built=" + built + ") loads");
            yield return null;
            wb = Workbench();
            Check(!wb.IsRetiredFromStartingWorld && wb.IsBuilt == built, "old save: workbench identity preserved, not retired, built=" + built);
            ForestSaveData again = saves.CaptureData();
            Check(again.buildables.Count(b => b.buildId == WorkbenchId) == 1 && again.buildables.Count == 7, "old save: re-capture keeps exactly one workbench-01 and seven entries");
            Check(again.buildables.Single(b => b.buildId == WorkbenchId).built == built, "old save: historical built state round-trips");
            Check(SiteMasks() == 7, "old save: seven Site masks (historical)");
            Check(Workbench().GetComponentsInChildren<Renderer>(false).Any(r => r.enabled && r.gameObject.activeInHierarchy) == built, "old save: workbench is visible exactly when built");
        }
        // Old-save prerequisites: Saw Pit requires the workbench; its built state must still follow the saved list.
        ForestSaveData withDependent = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(newGame));
        withDependent.buildables.Add(new BuildableSaveData { buildId = WorkbenchId, built = true });
        Check(saves.LoadData(withDependent, false) && Buildables().Where(b => b.HasPrerequisite).All(b => b.IsPrerequisiteMet), "old save: every buildable with a workbench prerequisite sees it met");

        // ---- C. disk round trip of a fresh new-game save, via the real Save / CanLoad / LoadData path ----
        ResetWorld(StandGeometryModel.Enlarged80);
        saves.Save();
        Check(File.Exists(SavePath), "new-game Save() wrote the isolated save file", SavePath);
        string disk = File.ReadAllText(SavePath);
        Check(!disk.Contains(WorkbenchId), "the new-game save file does not contain workbench-01");
        ResetWorld(StandGeometryModel.Legacy40);   // scratch: a different world, workbench still retired by new-game policy
        Check(saves.CanLoad(out string problem), "CanLoad accepts the new-game save", problem);
        ForestSaveData fromDisk = JsonUtility.FromJson<ForestSaveData>(disk);
        Check(saves.LoadData(fromDisk, false), "the new-game save loads");
        yield return null;
        Check(Workbench().IsRetiredFromStartingWorld && Presentation(Workbench()) == "activeRenderers=0 enabledColliders=0", "reloaded new-game world: workbench stays retired and hidden");
        Check(SiteMasks() == 6 && Hash() == newStartHash && TreeSig(Living()) == newStartTrees, "reloaded new-game world: six Site masks, same trees, same world hash");
        saves.Save();
        Check(!File.ReadAllText(SavePath).Contains(WorkbenchId), "re-saving after reload still omits workbench-01 (stable round trip)");

        // ---- D. negative: saves with no buildables list at all ----
        foreach (bool nullList in new[] { false, true })
        {
            ForestSaveData none = JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(newGame));
            none.buildables = nullList ? null : new List<BuildableSaveData>();
            ForestBuildable b0 = Workbench(); b0.SetRetiredFromStartingWorld(false);   // start from the opposite state so the assertion proves reconciliation
            Check(saves.LoadData(none, false), "save with " + (nullList ? "null" : "empty") + " buildables loads");
            yield return null;
            Check(Workbench().IsRetiredFromStartingWorld, "save with " + (nullList ? "null" : "empty") + " buildables retires the workbench (documented 'absent means retired'; no old site invented)");
        }

        // ---- E. Reference Future v1 preview and return ----
        ResetWorld(StandGeometryModel.Enlarged80);
        string before = Hash(); string beforeTrees = TreeSig(Living());
        ScenarioReferenceArchive archive = ScenarioReferenceArchive.Load();
        foreach (int year in new[] { 0, 20, 50, 100 })
        {
            Check(m.TryBeginReferencePreview(year), "Reference preview opens at year " + year);
            yield return null; yield return null;
            Check(!Workbench().IsRetiredFromStartingWorld, "preview year " + year + ": the reference's workbench-01 is restored");
            ForestSaveData shown = saves.CaptureData();
            Check(shown.buildables.Count(b => b.buildId == WorkbenchId) == 1, "preview year " + year + ": reference world still names workbench-01 once");
            ScenarioReferenceMilestone ms = archive.AtYear(year);
            Check(ms != null && ms.verifiedFrozenWorld, "preview year " + year + ": archive milestone is verified frozen");
            m.EndReferencePreview();
            yield return null; yield return null;
            Check(Workbench().IsRetiredFromStartingWorld && Presentation(Workbench()) == "activeRenderers=0 enabledColliders=0", "after year " + year + " preview: workbench retired again with no leaked renderer or collider");
            Check(Hash() == before && TreeSig(Living()) == beforeTrees && SiteMasks() == 6 && Ids(saves.CaptureData()) == expectedNonRetired,
                "after year " + year + " preview: exact new-game world, six Site masks, no transient save difference");
        }

        // ---- F. Year 0 / Year 3 hashes, continuous vs through a save/load ----
        ResetWorld(StandGeometryModel.Enlarged80);
        string y0 = Hash();
        string yearZeroJson = JsonUtility.ToJson(saves.CaptureData());
        for (int y = 0; y < 3; y++) { Check(m.AdvanceYear(), "continuous year " + (y + 1) + " advances"); yield return null; try { ui?.AcknowledgeAnnualReview(); } catch (Exception) { } }
        string y3 = Hash();
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(m.AdvanceYear(), "pre-load year advances"); yield return null;
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(yearZeroJson), false), "reload of the year-0 new-game save"); yield return null;
        Check(Workbench().IsRetiredFromStartingWorld, "year-0 reload keeps the workbench retired");
        for (int y = 0; y < 3; y++) { Check(m.AdvanceYear(), "resumed year " + (y + 1) + " advances"); yield return null; try { ui?.AcknowledgeAnnualReview(); } catch (Exception) { } }
        Check(Hash() == y3 && Workbench().IsRetiredFromStartingWorld, "three resumed years equal three uninterrupted years; workbench still retired");
        Debug.Log("WORKBENCH_NEW_ENLARGED80_YEAR0_CURRENT_HASH " + y0);
        Debug.Log("WORKBENCH_NEW_ENLARGED80_YEAR3_CURRENT_HASH " + y3);
        Check(y0 == newStartHash, "year-0 hash equals the start hash measured earlier");

        // ---- G. Legacy40 fresh world accounting (frozen-anchor sensitivity, measured not assumed) ----
        ResetWorld(StandGeometryModel.Legacy40);
        string legacyNew = Hash(); string legacyTrees = TreeSig(Living());
        Check(Living().Count == 336 && SiteMasks() == 6, "fresh Legacy40 new game: 336 trees, workbench retired (policy is new-game scoped, not geometry scoped)");
        Workbench().SetRetiredFromStartingWorld(false);
        ResetWorld(StandGeometryModel.Legacy40);
        string legacyBase = Hash();
        Check(TreeSig(Living()) == legacyTrees, "Legacy40: same 336 trees with and without the retired workbench");
        Debug.Log("WORKBENCH_LEGACY40_FRESH_NEW_CURRENT_HASH " + legacyNew);
        Debug.Log("WORKBENCH_LEGACY40_FRESH_BASELINE_EQUIVALENT_CURRENT_HASH " + legacyBase);
        Workbench().SetRetiredFromStartingWorld(true);
        ResetWorld(StandGeometryModel.Enlarged80);
        Check(Hash() == newStartHash, "scratch world returned to the new-game Enlarged80 start");
        if (File.Exists(SavePath)) File.Delete(SavePath);
        yield return null;
    }
}
