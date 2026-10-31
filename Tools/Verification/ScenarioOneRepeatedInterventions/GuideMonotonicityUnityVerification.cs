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

// S1-D T-04 DISPOSABLE Unity gate (verification only; never part of production). Stage, run and remove with its generated .meta
// under the machine-wide Unity lock in a registered lane. It writes only to the isolated config's forest-save.json.
//
// Proves, on the real controller, live UI and real JsonUtility disk round trips, that once the one shared durable historical latch
// is true the teaching guide is a monotone floor at "recruitment" (index 5), "establishment" (6) or "transformation" (7) as later
// current facts dictate, and that without the latch the same worlds are still at "response" (index 4). The latch itself is
// edited into a captured fixture exactly as the accepted #143 gate edits it; the guide, facts and persistence are all production.
public static class GuideMonotonicityUnityVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "GuideMonotonicityUnityVerification.Begin"))
            new GameObject("Disposable T-04 guide verification").AddComponent<GuideMonotonicityUnityRunner>();
    }
}

public sealed class GuideMonotonicityUnityRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marks;
    private string fixtureText;
    private int checks;

    private void Check(bool ok, string what, string detail = "")
    {
        detail = detail ?? "";
        if (!ok) throw new InvalidOperationException(what + (detail.Length > 0 ? " [" + detail + "]" : ""));
        checks++;
        Debug.Log("T04_CHECK " + what + (detail.Length > 0 ? " [" + detail + "]" : ""));
    }

    private IEnumerator Start()
    {
        for (int i = 0; i < 8; i++) yield return null;
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception error) { failure = error; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        Debug.Log(failure == null ? "S1D_T04_GUIDE_UNITY_PASS checks=" + checks : "S1D_T04_GUIDE_UNITY_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }
    private IEnumerator Settled() { yield return new WaitForSecondsRealtime(0.35f); }

    // The facts the Learning panel itself uses (private; refreshed at most every quarter of a second).
    private ScenarioOneTeachingSequence.Facts LiveFacts()
    {
        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
        return (ScenarioOneTeachingSequence.Facts)method.Invoke(ui.Learning, null);
    }

    private static ForestTree[] LivingTrees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    private ForestSaveData Fixture(Action<ForestSaveData> mutate, bool latch)
    {
        ForestSaveData data = JsonUtility.FromJson<ForestSaveData>(fixtureText);
        mutate(data);
        data.scenarioOne.firstGuidedRackReviewComplete = latch;
        return data;
    }

    private IEnumerator Load(ForestSaveData data, string label)
    {
        Check(saves.LoadData(data, false), "fixture loads: " + label);
        yield return Frames(12);
        ui.CloseAll(); ui.CloseHelp();
        ui.RackLayout.Update(true); ui.Learning.ReloadProgress();
        for (int i = 0; i < 6; i++) ui.Learning.Refresh(true);
        yield return Settled();
    }

    private int Index() { ui.Learning.Refresh(true); return ui.Learning.CurrentTopicIndex; }
    private string Scenario() => JsonUtility.ToJson(manager.CaptureSaveData());

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; marks = ui.Marking; saves = FindFirstObjectByType<ForestSaveController>();
        Check(manager != null && marks != null && saves != null, "scene has live lesson dependencies");
        string path = Environment.GetEnvironmentVariable("CCF_MENU_STAGE7_SAVE");
        Check(!string.IsNullOrEmpty(path) && File.Exists(path), "accepted stage-7 fixture present", path ?? "<unset>");
        fixtureText = File.ReadAllText(path);
        // The accepted fixture is an old-schema (v20) save: the latch is only restored from v24+, so normalise it through the
        // production loader and capture once. The captured world carries the current version; only later facts differ per variant.
        ForestSaveData raw = JsonUtility.FromJson<ForestSaveData>(fixtureText);
        Check(saves.LoadData(raw, false), "accepted stage-7 fixture loads through production migration");
        yield return Frames(12);
        ForestSaveData normalised = saves.CaptureData();
        Check(normalised.version >= 24, "normalised fixture carries the current save version", "version=" + normalised.version);
        fixtureText = JsonUtility.ToJson(normalised);
        // Stable lesson identities are part of the contract; the guide must still expose exactly these eight.
        Check(string.Join(",", ScenarioOneTeachingSequence.Lessons.Select(l => l.Id)) ==
            "access,future-trees,release,habitat,response,recruitment,establishment,transformation", "eight lesson ids unchanged and ordered");

        // plantingYear in the fixture is 3 and the world is in year 4; the three variants differ only in later current facts.
        var variants = new (string name, int expected, Action<ForestSaveData> mutate)[]
        {
            ("lesson6-recruitment", 5, d =>
            {
                // "No planting has happened yet": beech/oak goals need a completed PlantJuvenile order or a succeeded planting event.
                d.scenarioOne.workOrders.RemoveAll(o => o.type == ScenarioWorkType.PlantJuvenile);
                d.scenarioOne.managementEvents.RemoveAll(e => e.taskType == ScenarioWorkType.PlantJuvenile);
            }),
            ("lesson7-establishment", 6, d => d.scenarioOne.managementEvents.RemoveAll(e =>
                e.eventType == ScenarioManagementEventType.AnnualReviewAcknowledged && e.year > 3)),
            ("lesson8-transformation", 7, d => { }),
        };

        foreach (var v in variants)
        {
            Debug.Log("T04_VARIANT " + v.name);
            // 1. Same world WITHOUT the latch: pre-amendment behaviour. The new session has no rack review, so it is at "response".
            yield return Load(Fixture(v.mutate, false), v.name + " unlatched");
            Check(!manager.FirstGuidedRackReviewComplete && !LiveFacts().RackReviewComplete && Index() == 4,
                v.name + ": latch false + incomplete session review stays at response", "index=" + Index());
            Check(LiveFacts().ReleaseReviewed && Index() == 4, v.name + ": ReleaseReviewed true without rack review does not skip response");
            Check(ScenarioOneTeachingSequence.CurrentUnlatched(LiveFacts()) == 4 && manager.FirstGuidedRackReviewComplete == false,
                v.name + ": unlatched world never self-latches through reads");

            // 2. Same world WITH the historical latch: the floor holds although the session review is empty.
            ForestSaveData latched = Fixture(v.mutate, true);
            yield return Load(latched, v.name + " latched");
            ScenarioOneTeachingSequence.Facts facts = LiveFacts();
            Check(manager.FirstGuidedRackReviewComplete && facts.FirstGuidedRackReviewComplete && !facts.RackReviewComplete,
                v.name + ": latch loaded; session review empty");
            Check(Index() == v.expected, v.name + ": latched guide index is the monotone floor/later fact", "index=" + Index() + " want " + v.expected);
            Check(ScenarioOneTeachingSequence.CurrentUnlatched(facts) == 4, v.name + ": without the latch this very world would regress to response");
            string reference = Scenario();

            // 3. Repeated view refreshes/reads never mutate persistence or the latch.
            for (int i = 0; i < 25; i++) { ui.Learning.Refresh(true); ui.Learning.RackPlanProblem(); ui.Learning.RackPlanAdvisory(); LiveFacts(); }
            Check(Scenario() == reference && manager.FirstGuidedRackReviewComplete && Index() == v.expected,
                v.name + ": 25 repeated refreshes leave persistence, latch and index unchanged");

            // 4. Real controller disk round trips (isolated config) at this lesson.
            for (int i = 0; i < 2; i++)
            {
                saves.Save();
                Check(File.Exists(Path.Combine(Application.persistentDataPath, "forest-save.json")), v.name + ": disk save written " + i);
                Check(saves.CanLoad(out string problem), v.name + ": disk save loadable " + i, problem);
                saves.Load(); yield return Frames(12); ui.CloseAll(); ui.CloseHelp(); yield return Settled();
                for (int r = 0; r < 6; r++) ui.Learning.Refresh(true);
                Check(manager.FirstGuidedRackReviewComplete && !LiveFacts().RackReviewComplete && Index() == v.expected,
                    v.name + ": disk reload keeps floor and index without session reconstruction " + i, "index=" + Index());
                Check(Scenario() == reference, v.name + ": reload leaves persisted scenario byte-identical " + i);
            }

            // 5. Crop Tree count drift and loss after historical completion.
            var crops = LivingTrees().Where(t => t.IsCropTree).ToArray();
            Check(crops.Length > 0, v.name + ": fixture has Crop Trees to drift", "n=" + crops.Length);
            marks.Unmark(crops[0], false); yield return Settled();
            Check(Index() == v.expected && manager.FirstGuidedRackReviewComplete, v.name + ": Crop Tree count decrease cannot regress guide");
            var extra = LivingTrees().First(t => !t.IsCropTree);
            marks.Mark(extra, TreeMarkType.CropTree, false); yield return Settled();
            Check(Index() == v.expected && manager.FirstGuidedRackReviewComplete, v.name + ": Crop Tree count increase cannot regress guide");
            int deaths = 0;
            foreach (var t in LivingTrees().Where(t => t.IsCropTree)) { if (t.ApplyMortality("self-thinning", manager.CurrentEcologicalYear)) deaths++; }
            yield return Frames(6); yield return Settled();
            Check(deaths > 0 && Index() == v.expected && manager.FirstGuidedRackReviewComplete,
                v.name + ": Crop Tree loss cannot regress guide (forced verification mortality)", "deaths=" + deaths);

            // 6. Executed-share drift on a captured copy, after completion (same technique as the accepted #143 gate).
            yield return Load(latched, v.name + " latched reload for share drift");
            ForestSaveData drift = saves.CaptureData();
            int changed = 0;
            foreach (var o in drift.scenarioOne.workOrders.Where(o => o.type == ScenarioWorkType.FellTree && o.status == ScenarioWorkStatus.Completed))
            { o.fellingOutcome = FellingMaterialOutcome.SellAndExtract; changed++; }
            Check(saves.LoadData(drift, false), v.name + ": executed-share drift fixture loads"); yield return Frames(12); yield return Settled();
            Check(changed > 0 && manager.FirstGuidedRackReviewComplete && Index() == v.expected,
                v.name + ": executed-share drift cannot regress guide", "orders=" + changed + " index=" + Index());

            // 7. Later in-footprint felling executed through the real manager after completion.
            yield return Load(latched, v.name + " latched reload for later felling");
            var victim = LivingTrees().First(t => !t.IsCropTree && t.CanChop);
            marks.Mark(victim, TreeMarkType.Fell, false); manager.AddMarkedTreesToWorkPlan();
            Check(manager.ApprovePendingWork() && manager.AdvanceYear(), v.name + ": later felling executes and year advances");
            yield return Frames(30); yield return Settled();
            Check(manager.FirstGuidedRackReviewComplete && Index() >= v.expected,
                v.name + ": after later felling and a further year the guide is not behind the floor", "index=" + Index());
            ForestSaveData after = saves.CaptureData();
            Check(after.scenarioOne.firstGuidedRackReviewComplete, v.name + ": latch still captured true after later work");
        }

        // A world without the latch field at all (pre-v24) stays conservative false and is never reconstructed from later facts.
        ForestSaveData old = Fixture(variants[2].mutate, false);
        old.version = 23;
        string oldJson = JsonUtility.ToJson(old).Replace("\"firstGuidedRackReviewComplete\":false,", "");
        Check(!oldJson.Contains("firstGuidedRackReviewComplete"), "legacy fixture truly lacks the latch field");
        yield return Load(JsonUtility.FromJson<ForestSaveData>(oldJson), "pre-v24 without latch field");
        Check(!manager.FirstGuidedRackReviewComplete && Index() == 4, "missing-latch save is conservative false and stays at response", "index=" + Index());
        for (int i = 0; i < 10; i++) { ui.Learning.Refresh(true); LiveFacts(); }
        Check(!manager.FirstGuidedRackReviewComplete, "later-lesson facts never reconstruct the latch");
    }
}
