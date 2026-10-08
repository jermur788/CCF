using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable P2 gate: Crop Tree competitor reasoning. Copy into
// Assets/ForestPrototype, run CropTreeCompetitorVerification.Begin, then remove
// the copy and its .meta.
//
// BATCH-SAFE part (-batchmode -nographics): the breakdown against live
// ForestTree objects and ForestEcologyController.GetCompetitionIndex, release
// preview, suppressed-neighbour ranking, determinism, no state written, save
// round trip, dismiss/reopen of world markers, objectives and lessons unchanged.
// INTERACTIVE part (no -batchmode, DISPLAY set): inspection card and forecast
// rendered at 1280x720, 1600x900 and 1920x1080, fit checks and captures to
// CCF_ACCEPTANCE_OUTPUT. Never writes the player's save file.
public static class CropTreeCompetitorVerification
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif

    public static void SetGameSize(int width, int height)
    {
#if UNITY_EDITOR
        if (Application.isBatchMode) return;
        Assembly assembly = typeof(Editor).Assembly;
        Type sizesType = assembly.GetType("UnityEditor.GameViewSizes");
        Type singleton = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        object sizes = singleton.GetProperty("instance", BindingFlags.Public | BindingFlags.Static).GetValue(null);
        MethodInfo getGroup = sizesType.GetMethod("GetGroup");
        object group = getGroup.Invoke(sizes, new[] { Enum.ToObject(getGroup.GetParameters()[0].ParameterType, 0) });
        Type sizeType = assembly.GetType("UnityEditor.GameViewSize"), kind = assembly.GetType("UnityEditor.GameViewSizeType");
        object size = Activator.CreateInstance(sizeType, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
            null, new object[] { Enum.ToObject(kind, 1), width, height, "P2 " + width + "x" + height }, null);
        int index = (int)group.GetType().GetMethod("GetTotalCount").Invoke(group, null);
        group.GetType().GetMethod("AddCustomSize").Invoke(group, new[] { size });
        Type viewType = assembly.GetType("UnityEditor.GameView");
        EditorWindow view = EditorWindow.GetWindow(viewType);
        viewType.GetProperty("selectedSizeIndex", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).SetValue(view, index);
        view.Show(); view.Focus(); view.Repaint();
#endif
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "CropTreeCompetitorVerification.Begin"))
            new GameObject("Disposable P2 competitor verification").AddComponent<CropTreeCompetitorVerificationRunner>();
    }
}

public sealed class CropTreeCompetitorVerificationRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestTreeMarkingManager marking;
    private ForestPlayer player;
    private string output;

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }

    private IEnumerator Start()
    {
        for (int i = 0; i < 5; i++) yield return null;
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        Exception failure = null;
        IEnumerator checks = Verify();
        while (true)
        {
            bool more;
            object current = null;
            try { more = checks.MoveNext(); if (more) current = checks.Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) break;
            yield return current;
        }
        if (marking != null) marking.ClearAll();
        Debug.Log(failure == null ? "P2_COMPETITOR_VERIFY_PASS" : "P2_COMPETITOR_VERIFY_FAIL: " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private ForestTree Tree(string id) => FindObjectsByType<ForestTree>(FindObjectsSortMode.None).First(t => t.TreeId == id);

    private List<CompetitionTree> SceneInput()
        => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Where(t => t.IsLiving)
            .OrderBy(t => t.TreeId, StringComparer.Ordinal)
            .Select(t => new CompetitionTree(t.TreeId, new Vector2(t.transform.position.x, t.transform.position.z), t.Diameter, t.IsMarkedForFell, t.IsCropTree))
            .ToList();

    private string WorldState()
    {
        var trees = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).OrderBy(t => t.TreeId, StringComparer.Ordinal)
            .Select(t => $"{t.TreeId}:{t.MarkType}:{t.Diameter:R}:{t.Height:R}:{t.IsLiving}");
        return string.Join(";", trees) + "|" + JsonUtility.ToJson(manager.CaptureSaveData());
    }

    private IEnumerator Verify()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        player = FindFirstObjectByType<ForestPlayer>();
        ForestSaveController saves = FindFirstObjectByType<ForestSaveController>();
        ui = manager != null ? manager.GetComponent<ScenarioOneUiRoot>() : null;
        Check(manager != null && ecology != null && marking != null && player != null && saves != null && ui != null, "scenario systems missing");
        for (int i = 0; i < 10 && ui.Competitors == null; i++) yield return null;
        Check(ui.Competitors != null, "competitor assessment not created by the UI root");
        Check(ecology.EcologicalYear == 0, "expected a fresh Year-0 Scenario One stand");
        marking.ClearAll();
        yield return null;

        // 1. Contributions reconcile with the authoritative competition index for every tree.
        List<CompetitionTree> input = SceneInput();
        ecology.InvalidateCompetition(); // harness only: force a recompute from current state
        float worst = 0f;
        for (int i = 0; i < input.Count; i++)
        {
            CropTreeCompetitionReport r = CropTreeCompetition.Analyse(input, i);
            float authoritative = ecology.GetCompetitionIndex(Tree(input[i].Id));
            float parts = r.Ranked.Sum(e => e.Contribution);
            worst = Mathf.Max(worst, Mathf.Max(Mathf.Abs(r.CompetitionNow - authoritative), Mathf.Abs(parts - authoritative)));
        }
        Check(worst <= 1e-4f, "breakdown does not reconcile with GetCompetitionIndex, worst " + worst);
        Debug.Log($"P2_RECONCILE_PASS trees={input.Count} worstAbsDiff={worst:E2} cutoff={ForestEcologyController.HegyiCutoffMeters}");

        // Real-stand A/B/C: Crop Tree P0707, small suppressed neighbour P0710 (6 m away), large close neighbour P0706.
        ForestTree crop = Tree("P0707");
        string before = WorldState();
        var learningBefore = LearningIds();
        string objectivesBefore = string.Join(",", manager.Objectives.Select(o => o.objectiveId + o.achieved));
        marking.Mark(crop, TreeMarkType.CropTree, false);
        yield return null;
        ui.Competitors.Update(crop);
        CropTreeCompetitionReport report = ui.Competitors.Report;
        Check(report != null && ui.Competitors.ReportTree == crop, "no report for the inspected Crop Tree");
        int rankSmall = report.Ranked.FindIndex(e => e.Id == "P0710"), rankLarge = report.Ranked.FindIndex(e => e.Id == "P0706");
        Check(rankLarge == 0 && rankSmall > report.NeighbourCount / 2 && report.Ranked[rankSmall].Strength == CompetitorStrength.Low,
            $"suppressed/large ranking unexpected: P0706 rank {rankLarge + 1}, P0710 rank {rankSmall + 1} of {report.NeighbourCount}");
        Debug.Log($"P2_SUPPRESSED_CASE_PASS P0706 rank 1 {report.Ranked[0].Share:P1} {report.Ranked[0].Strength}; "
            + $"P0710 rank {rankSmall + 1}/{report.NeighbourCount} {report.Ranked[rankSmall].Share:P1} {report.Ranked[rankSmall].Strength}");

        // 2. A marked competitor lowers the preview by exactly its contribution; 3. an unrelated mark does not.
        float now = report.CompetitionNow, largest = report.Ranked[0].Contribution;
        marking.Mark(Tree("P0706"), TreeMarkType.Fell, false);
        ForestTree far = FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving)
            .OrderByDescending(t => Vector3.Distance(t.transform.position, crop.transform.position)).First();
        marking.Mark(far, TreeMarkType.Fell, false);
        yield return new WaitForSecondsRealtime(0.3f);
        ui.Competitors.Update(crop);
        report = ui.Competitors.Report;
        Check(Mathf.Abs(report.CompetitionAfterPlanned - (now - largest)) < 1e-4f && report.PlannedNeighbourCount == 1,
            $"release preview wrong: {report.CompetitionNow} -> {report.CompetitionAfterPlanned}, expected {now - largest}");
        Debug.Log($"P2_RELEASE_PREVIEW_PASS {report.CompetitionNow:F3} -> {report.CompetitionAfterPlanned:F3} ({report.ChangeFraction:P1}); far mark {far.TreeId} ignored");

        // Unmark restores the preview.
        marking.Unmark(Tree("P0706"), false);
        marking.Unmark(far, false);
        yield return new WaitForSecondsRealtime(0.3f);
        ui.Competitors.Update(crop);
        Check(!ui.Competitors.Report.HasPlannedRemovals && ui.Competitors.Report.CompetitionAfterPlanned == ui.Competitors.Report.CompetitionNow,
            "unmarking did not restore the preview");

        // Multi-Crop: a Fell mark near several Crop Trees counts as one stem.
        marking.Mark(Tree("P0606"), TreeMarkType.CropTree, false);
        marking.Mark(Tree("P0706"), TreeMarkType.Fell, false);
        yield return new WaitForSecondsRealtime(0.3f);
        ui.Competitors.Update(crop);
        CropTreeReleaseSummary summary = ui.Competitors.Summary;
        Check(summary.PlannedFells == 1 && summary.PlannedNearACropTree == 1 && summary.PlannedNearSeveralCropTrees == 1 && summary.CropTrees == 2,
            $"multi-crop summary wrong: fells {summary.PlannedFells} near {summary.PlannedNearACropTree} several {summary.PlannedNearSeveralCropTrees}");
        Debug.Log($"P2_MULTI_CROP_PASS cropTrees=2 fells=1 nearSeveral=1 mean {summary.MeanCompetitionNow:F3} -> {summary.MeanCompetitionAfter:F3}");
        marking.Unmark(Tree("P0606"), false);
        marking.Unmark(Tree("P0706"), false);
        yield return new WaitForSecondsRealtime(0.3f);

        // 6. Determinism within the process (the logged hash is compared across two processes).
        ui.Competitors.Update(crop);
        string a = Describe(ui.Competitors.Report);
        string b = Describe(CropTreeCompetition.Analyse(SceneInput(), SceneInput().FindIndex(t => t.Id == "P0707")));
        Check(a == b, "breakdown not deterministic");
        Debug.Log("P2_DETERMINISM_HASH " + StableHash(a));

        // 9. World markers appear only while assessing, and dismiss/reopen cleanly.
        ui.Competitors.Update(crop);
        Check(ActiveRings() == Mathf.Min(CropTreeCompetition.ListedCount, report.NeighbourCount), "rings missing while assessing");
        ui.Competitors.Clear();
        ui.Competitors.Update(null);
        Check(ActiveRings() == 0, "rings left after dismiss");
        ui.Competitors.Update(crop);
        Check(ActiveRings() == Mathf.Min(CropTreeCompetition.ListedCount, report.NeighbourCount), "rings missing after reopen");
        ui.Competitors.Update(Tree("P0710")); // not a Crop Tree: no markers
        Check(ActiveRings() == 0, "markers shown for a tree that is not a Crop Tree");
        ui.Competitors.Clear();

        // 7/10. Inspecting competition wrote nothing: unmark the Crop Tree and compare.
        marking.Unmark(crop, false);
        yield return new WaitForSecondsRealtime(0.3f);
        Check(WorldState() == before, "world or scenario state changed by competitor analysis");
        Check(LearningIds().SequenceEqual(learningBefore), "a learning step completed from the analysis");
        Check(string.Join(",", manager.Objectives.Select(o => o.objectiveId + o.achieved)) == objectivesBefore, "objectives changed");
        Debug.Log("P2_NO_STATE_PASS marks, trees, scenario save data, lessons and objectives unchanged");

        // 8. Save round trip needs no new state: the captured data loads and the breakdown is identical.
        ForestSaveData data = saves.CaptureData();
        Check(data.version == ForestSaveData.CurrentVersion, "save version changed");
        string json = JsonUtility.ToJson(data);
        Check(!json.Contains("ompetitor"), "save data contains competitor state");
        string reportBefore = Describe(CropTreeCompetition.Analyse(SceneInput(), SceneInput().FindIndex(t => t.Id == "P0707")));
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false), "save round trip failed to load");
        for (int i = 0; i < 5; i++) yield return null;
        string reportAfter = Describe(CropTreeCompetition.Analyse(SceneInput(), SceneInput().FindIndex(t => t.Id == "P0707")));
        Check(reportBefore == reportAfter, "breakdown differs after save/load");
        // The live assessment must follow the reloaded tree objects, not the destroyed ones.
        ForestTree reloaded = Tree("P0707");
        marking.Mark(reloaded, TreeMarkType.CropTree, false);
        yield return new WaitForSecondsRealtime(0.3f);
        ui.Competitors.Update(reloaded);
        Check(ui.Competitors.ReportTree == reloaded && ui.Competitors.Report != null
            && Describe(ui.Competitors.Report) == reportAfter && ActiveRings() > 0, "assessment did not rebuild after load");
        ui.Competitors.Clear();
        marking.Unmark(reloaded, false);
        yield return new WaitForSecondsRealtime(0.3f);
        Debug.Log($"P2_SAVE_PASS version=v{ForestSaveData.CurrentVersion} no competitor fields; breakdown identical after load");

        // Performance: inspection and mark-change recalculation on the Year-0 stand.
        var watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 50; i++) CropTreeCompetition.Analyse(SceneInput(), i % 336);
        double inspectMs = watch.Elapsed.TotalMilliseconds / 50.0;
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsSortMode.None).Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).Where((t, i) => i % 21 == 0))
            marking.Mark(t, TreeMarkType.CropTree, false);
        watch.Restart();
        for (int i = 0; i < 10; i++) CropTreeCompetition.Summarise(SceneInput());
        Debug.Log($"P2_PERF stems={SceneInput().Count} cropTrees={marking.LivingCropTreeCount} inspectMsIncludingSceneScan={inspectMs:F3} markChangeMs={watch.Elapsed.TotalMilliseconds / 10.0:F3}");
        marking.ClearAll();
        yield return null;

        // 10. Storm state (post-storm port): the assessment leaves wind exposure untouched, and a
        // windthrown neighbour (biologically dead) leaves the breakdown, which still reconciles.
        ForestTree assessed = Tree("P0707");
        marking.Mark(assessed, TreeMarkType.CropTree, false);
        yield return new WaitForSecondsRealtime(0.3f);
        string windBefore = ecology.GetStormExposureLabel(assessed) + "|" + ecology.GetStormExposureExplanation(assessed);
        ui.Competitors.Update(assessed);
        ui.Competitors.Clear();
        Check(ecology.GetStormExposureLabel(assessed) + "|" + ecology.GetStormExposureExplanation(assessed) == windBefore,
            "competitor assessment changed the storm exposure text");
        ForestTree victim = Tree("P0706");
        Check(victim.ApplyMortality("windthrow", ecology.EcologicalYear), "fixture windthrow death");
        yield return new WaitForSecondsRealtime(0.3f);
        ui.Competitors.Update(assessed);
        ecology.InvalidateCompetition();
        float afterDeath = ecology.GetCompetitionIndex(assessed);
        Check(ui.Competitors.Report != null && ui.Competitors.Report.Ranked.All(e => e.Id != "P0706")
            && Mathf.Abs(ui.Competitors.Report.CompetitionNow - afterDeath) <= 1e-4f,
            $"windthrown neighbour still counted or breakdown out of step: {ui.Competitors.Report?.CompetitionNow} vs {afterDeath}");
        ui.Competitors.Clear();
        Debug.Log($"P2_STORM_COEXIST_PASS stormModel={ecology.StormModelVersion} wind '{ecology.GetStormExposureLabel(assessed)}' unchanged; "
            + $"windthrown P0706 removed; competition {afterDeath:F3} reconciles");

        if (Application.isBatchMode)
        {
            Debug.Log("P2_RENDERED_SKIPPED batchmode (run interactively for layout and captures)");
            yield break;
        }
        yield return Rendered();
    }

    private IEnumerator Rendered()
    {
        ForestTree crop = Tree("P0707");
        marking.Mark(crop, TreeMarkType.CropTree, false);
        marking.Mark(Tree("P0607"), TreeMarkType.Fell, false);
        player.transform.position = crop.transform.position - new Vector3(0f, 0f, 2.2f);
        player.LookToward(crop.transform.position + Vector3.up * 1.3f);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked; UnityEngine.Cursor.visible = false;
        for (int i = 0; i < 20; i++) yield return null;
        MethodInfo inspect = typeof(ForestPlayer).GetMethod("InspectTree", BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (Vector2Int size in new[] { new Vector2Int(1280, 720), new Vector2Int(1600, 900), new Vector2Int(1920, 1080) })
        {
            CropTreeCompetitorVerification.SetGameSize(size.x, size.y);
            for (int i = 0; i < 8; i++)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                inspect.Invoke(player, new object[] { crop });
                yield return null;
            }
            Check(player.InspectedTree == crop, "inspection closed during rendered check at " + size.x);
            VisualElement card = ui.RootElement.Q(className: "inspect");
            Rect viewport = ui.RootElement.worldBound, bounds = card.worldBound;
            Check(card.resolvedStyle.display == DisplayStyle.Flex && viewport.Contains(bounds.min) && viewport.Contains(bounds.max),
                $"inspection card clipped at {size.x}: {bounds} in {viewport}");
            float contentBottom = card.Query<Label>().ToList().Where(l => l.resolvedStyle.display == DisplayStyle.Flex).Max(l => l.worldBound.yMax);
            Check(contentBottom <= bounds.yMax + 1f, $"card content overflows at {size.x}: {contentBottom} > {bounds.yMax}");
            VisualElement status = ui.RootElement.Q(className: "hud-status");
            Check(status == null || !bounds.Overlaps(status.worldBound), "card overlaps HUD status at " + size.x);
            int tagsVisible = ui.RootElement.Query<Label>(className: "competitor-tag").ToList().Count(l => l.resolvedStyle.display == DisplayStyle.Flex);
            Check(tagsVisible > 0, "no competitor tags visible at " + size.x);
            yield return Capture("p2-inspection-" + size.x);
            Debug.Log($"P2_RENDERED_PASS {size.x}x{size.y} card={bounds} tagsVisible={tagsVisible}");
        }
        CropTreeCompetitorVerification.SetGameSize(1600, 900);
    }

    private IEnumerator Capture(string name)
    {
        yield return new WaitForEndOfFrame();
        if (string.IsNullOrEmpty(output)) yield break;
        Directory.CreateDirectory(output);
        Texture2D image = ScreenCapture.CaptureScreenshotAsTexture();
        Check(image != null && image.width > 600, "missing capture " + name);
        File.WriteAllBytes(Path.Combine(output, name + ".jpg"), image.EncodeToJPG(85));
        Destroy(image);
    }

    private static int ActiveRings()
    {
        GameObject root = GameObject.Find("Crop Tree competitor rings (runtime)");
        return root == null ? 0 : root.GetComponentsInChildren<LineRenderer>(false).Length;
    }

    private static List<string> LearningIds()
        => LearningObjectivesView.StepIds.Where(id => PlayerPrefs.GetInt(LearningObjectivesView.PreferencePrefix + id, 0) == 1).ToList();

    private static string Describe(CropTreeCompetitionReport r)
        => string.Join(";", r.Ranked.Select(e => $"{e.Id}:{e.Contribution:R}")) + "|" + r.CompetitionNow.ToString("R");

    private static string StableHash(string text)
    {
        ulong hash = 1469598103934665603UL;
        foreach (char c in text) { hash ^= c; hash *= 1099511628211UL; }
        return hash.ToString("X16");
    }
}
