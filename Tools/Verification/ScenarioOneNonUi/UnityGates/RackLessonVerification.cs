using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

// Disposable live gate for the rack-area lesson (Objectives 2-5; ScenarioOneRackBay + ScenarioOneRackBayReader + the teaching facts in
// LearningObjectivesView.Sequence.cs): SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1, item 4. The decision rules are checked on plain
// .NET; this checks that the live game feeds them the right data. It uses the real marking manager, Work Plan and save controller:
//   * the area is the middle rack's strip in the 80 m stand, and future trees are counted only inside it;
//   * Crop Trees elsewhere or too close do not progress the lesson;
//   * the share of each future tree's competition that is in the Work Plan equals the number the competitor panel shows for that tree;
//   * sold and deadwood choices are counted tree by tree;
//   * the lesson state is identical after a save and load, and on a second read (it stores nothing of its own).
// Not exercised live: carrying the work out and the Annual Review (needs the full rack flow and cash); the executed branch is covered
// by the plain .NET check, and the protected completion / economy gates cover the Work Plan and review themselves.
// Run with run_unity_harness.py in a graphical Editor with focus under the Unity lock; it stages and removes the copy and its .meta. It
// never writes the player's save file or the scene.
//
// Prints LESSON_CHECK <what>, then LESSON_PASS checks=N or LESSON_FAIL <reason>.
public static class RackLessonVerification
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
        if (Environment.GetCommandLineArgs().Any(a => a == "RackLessonVerification.Begin"))
            new GameObject("Disposable rack lesson verification").AddComponent<RackLessonVerificationRunner>();
    }
}

public sealed class RackLessonVerificationRunner : MonoBehaviour
{
    private ScenarioOneUiRoot ui;
    private ScenarioOneManager manager;
    private ForestPlayer player;
    private ForestTreeMarkingManager marks;
    private ForestSaveController saves;
    private int checks;

    private void Check(bool ok, string what, string detail = "")
    {
        if (!ok) throw new InvalidOperationException(what + (detail.Length > 0 ? " [" + detail + "]" : ""));
        checks++;
        Debug.Log("LESSON_CHECK " + what + (detail.Length > 0 ? " [" + detail + "]" : ""));
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
        Debug.Log(failure == null ? "LESSON_PASS checks=" + checks : "LESSON_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }

    private IEnumerator Press(Key key)
    {
        Keyboard keyboard = Keyboard.current ?? InputSystem.AddDevice<Keyboard>();
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key)); yield return null; yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null; yield return null;
    }

    private IEnumerator Frames(int n) { for (int i = 0; i < n; i++) yield return null; }

    private static ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && t.Species != null && t.Species.SpeciesId == "sitka-spruce")
        .OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    private ScenarioOneRackBay.Analysis Bay()
        => ScenarioOneRackBayReader.Read(FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None), manager.WorkOrders, ui.RackTool.Planner.LayoutLines);

    // The facts the Learning panel itself uses (private; refreshed at most every quarter of a second).
    private ScenarioOneTeachingSequence.Facts LiveFacts()
    {
        MethodInfo method = typeof(LearningObjectivesView).GetMethod("TeachingFacts", BindingFlags.Instance | BindingFlags.NonPublic);
        return (ScenarioOneTeachingSequence.Facts)method.Invoke(ui.Learning, null);
    }

    private IEnumerator Settled() { yield return new WaitForSecondsRealtime(0.35f); }

    private IEnumerator Inspect(ForestTree tree)
    {
        if (player.IsInspecting) yield return Press(Key.E);
        player.transform.position = tree.transform.position - new Vector3(0, 0, 3);
        yield return Frames(8);
        if (marks.AimedTree != tree) throw new InvalidOperationException("could not aim at " + tree.TreeId);
        yield return Press(Key.E);
        yield return Frames(8);
        if (!player.IsInspecting || player.InspectedTree != tree) throw new InvalidOperationException("could not inspect " + tree.TreeId);
    }

    private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; player = ui.Player; marks = ui.Marking; saves = FindFirstObjectByType<ForestSaveController>();
        Check(ui != null && manager != null && marks != null && saves != null, "the scene has its UI, manager, marking manager and save controller");
        ui.CloseAll(); ui.CloseHelp();
        yield return Frames(4);

        // ---- the area ----
        IReadOnlyList<float> lines = ui.RackTool.Planner.LayoutLines;
        Check(lines.Count == 5, "the 80 m stand has its five-rack layout", "lines " + lines.Count);
        ScenarioOneRackBay.Analysis bay = Bay();
        Check(bay.Applies && bay.RackCount == 5 && bay.RackIndex == 2, "the lesson area belongs to the middle rack of five");
        Check(Mathf.Abs(bay.CentreZ - lines.OrderBy(z => z).ElementAt(2)) < 0.001f && Mathf.Abs(bay.HalfWidthM - 7.6f) < 0.01f, "the area is the strip beside it, 7.6 m either way", "z " + F(bay.CentreZ));
        Check(bay.FutureCount == 0 && !bay.ReleasePlanned && !bay.ReleaseExecuted, "a fresh game has no future trees and no release");
        yield return Settled();
        var facts = LiveFacts();
        Check(facts.Bay != null && facts.Bay.Applies && facts.CropTrees == 0, "the Learning panel reads the same area and counts no future trees");

        // ---- choosing future trees: only trees that count progress the lesson ----
        var chosen = new List<ForestTree>();
        foreach (ForestTree tree in Trees())
        {
            if (chosen.Count == ScenarioOneRackBay.FutureTreesWanted) break;
            float across = Mathf.Abs(tree.transform.position.z - bay.CentreZ);
            if (Mathf.Abs(tree.transform.position.x) > 11f || across > 6.5f || across < 2f) continue;
            marks.Mark(tree, TreeMarkType.CropTree, false);
            if (Bay().FutureCount == chosen.Count + 1) chosen.Add(tree);
            else marks.Unmark(tree, false);                                  // too close to another, overtopped, or outside: it does not count
        }
        Check(chosen.Count == ScenarioOneRackBay.FutureTreesWanted, "three future trees can be chosen in the area", string.Join(",", chosen.Select(t => t.TreeId)));
        bay = Bay();
        Check(bay.HasEnoughFuture && bay.FutureCount == 3, "the area now has its three future trees");
        yield return Settled();
        facts = LiveFacts();
        Check(facts.CropTrees == 3, "the Learning panel counts them as the lesson's future trees", "count " + facts.CropTrees);

        ForestTree outsider = Trees().First(t => Mathf.Abs(t.transform.position.z - bay.CentreZ) > 14f && !t.IsCropTree && Mathf.Abs(t.transform.position.x) < 10f);
        marks.Mark(outsider, TreeMarkType.CropTree, false);
        ForestTree adjacent = Trees().First(t => !t.IsCropTree && Vector3.Distance(t.transform.position, chosen[0].transform.position) > 2f
            && Vector3.Distance(t.transform.position, chosen[0].transform.position) < 6f
            && Mathf.Abs(t.transform.position.z - bay.CentreZ) > 2f && Mathf.Abs(t.transform.position.z - bay.CentreZ) < 6.5f);
        marks.Mark(adjacent, TreeMarkType.CropTree, false);
        bay = Bay();
        Check(bay.FutureCount == 3 && bay.CropElsewhere >= 1 && (bay.TooClose.Contains(adjacent.TreeId) || bay.Overtopped.Contains(adjacent.TreeId)),
            "a Crop Tree outside the area, and one too close to a future tree, do not count", "elsewhere " + bay.CropElsewhere + ", too close " + bay.TooClose.Count);
        marks.Unmark(outsider, false); marks.Unmark(adjacent, false);
        yield return Settled();
        Check(LiveFacts().CropTrees == 3, "and the panel still counts three");

        // ---- releasing: the share in the Work Plan is the number the competitor panel shows ----
        Check(bay.MinWithMarks < ScenarioOneRackBay.FirstReleaseShare, "no release is planned yet");
        var toFell = new List<ForestTree>();
        foreach (var future in bay.Future)
        {
            ForestTree tree = Trees().First(t => t.TreeId == future.Id);
            // The strongest competitors first, from the competitor panel's own ranking.
            yield return Inspect(tree);
            var ranked = ui.Competitors.Report.Ranked;
            float share = 0f; int taken = 0;
            while (taken < 5 && share < 0.34f)
            {
                CompetitorEntry entry = ranked[taken++];
                if (entry.CropTree) continue;
                ForestTree competitor = Trees().First(t => t.TreeId == entry.Id);
                if (!competitor.IsMarkedForFell) { marks.Mark(competitor, TreeMarkType.Fell, false); toFell.Add(competitor); }
                share += entry.Share;
            }
            yield return Press(Key.E);
        }
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        int added = manager.AddMarkedTreesToWorkPlan();
        Check(added == toFell.Count && added > 0, "the marked trees go into the Work Plan as felling orders", "orders " + added);
        bay = Bay();
        Check(bay.PlannedRemovals == bay.SoldPlanned + bay.DeadwoodPlanned && bay.PlannedRemovals >= added, "the area counts the removals made for its future trees, each with its fate", "planned " + bay.PlannedRemovals);

        foreach (var future in bay.Future)
        {
            ForestTree tree = Trees().First(t => t.TreeId == future.Id);
            yield return Inspect(tree);
            Check(Mathf.Abs(ui.Competitors.Report.PlannedShare - future.PlannedShare) < 0.005f,
                "the lesson's planned share equals the competitor panel's for " + future.Id, "lesson " + F(future.PlannedShare) + ", panel " + F(ui.Competitors.Report.PlannedShare));
            yield return Press(Key.E);
        }

        // ---- sold and deadwood are counted tree by tree ----
        int soldBefore = bay.SoldPlanned;
        Check(bay.DeadwoodPlanned == 0 && !bay.MixedPlanned, "all sold: the choice has not been compared yet");
        ForestTree extra = Trees().First(t => !t.IsCropTree && !t.IsMarkedForFell && !manager.WorkOrders.Any(o => o.targetTreeId == t.TreeId)
            && Vector3.Distance(t.transform.position, chosen[0].transform.position) < ForestEcologyController.HegyiCutoffMeters);
        manager.PlanningFellingOutcome = FellingMaterialOutcome.RetainAsFallenDeadwood;
        marks.Mark(extra, TreeMarkType.Fell, false);
        Check(manager.AddMarkedTreesToWorkPlan() == 1, "one more tree is planned, to be left as deadwood", extra.TreeId);
        bay = Bay();
        Check(bay.DeadwoodPlanned == 1 && bay.SoldPlanned == soldBefore && bay.MixedPlanned, "one sold and one left as deadwood now count as a considered choice", $"sold {bay.SoldPlanned}, deadwood {bay.DeadwoodPlanned}");

        // ---- the lesson stores nothing: same answer again, and after a save and load ----
        string Signature(ScenarioOneRackBay.Analysis a) => string.Join("|", a.Future.Select(f => f.Id + ":" + F(f.PlannedShare) + ":" + F(f.ExecutedShare)))
            + $"#{a.PlannedRemovals}/{a.SoldPlanned}/{a.DeadwoodPlanned}/{F(a.RemovedBasalShare)}/{a.ReleasePlanned}/{a.MixedPlanned}";
        string before = Signature(Bay());
        Check(Signature(Bay()) == before, "reading the lesson twice gives the same answer");
        ForestSaveData data = saves.CaptureData();
        saves.LoadData(data, false);
        yield return Frames(12);
        yield return Settled();
        Check(Signature(Bay()) == before, "after a save and load the lesson is exactly where it was", before);
        var loaded = LiveFacts();
        Check(loaded.Bay != null && loaded.CropTrees == 3, "and the Learning panel agrees");
        Check(manager.WorkOrders.Count(o => o.type == ScenarioWorkType.FellTree && o.IsOpen) >= bay.PlannedRemovals, "the Work Plan still holds the felling orders after the load");
    }
}
