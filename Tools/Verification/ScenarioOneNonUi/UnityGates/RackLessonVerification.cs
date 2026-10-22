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

// Disposable live gate for the V2 lesson: actual Work Plan/Annual Review, keyboard inspection and decision hooks, and in-memory reload.
// InputSystem queued events are synthetic; the rendered/manual qualification still needs real OS input and both render sizes.
// Does not write save files. Stage/run/remove with its generated meta under the machine-wide Unity lock in a registered lane only.
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
    {
        var layout = ui.RackTool.Planner.LayoutStatus();
        var line = layout[layout.Count / 2];
        return ScenarioOneRackBayReader.Read(FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None), manager.WorkOrders,
            ui.RackTool.Planner.LayoutLines, new ScenarioOneRackBay.Frame(line.Start, line.End,
                ScenarioOneRackBay.HalfWidth(ui.RackTool.Planner.LayoutLines, layout.Count / 2)));
    }

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
        player.LookToward(tree.transform.position + Vector3.up * 1.4f);
        yield return Frames(8);
        if (marks.AimedTree != tree) throw new InvalidOperationException("could not aim at " + tree.TreeId);
        yield return Press(Key.E);
        yield return Frames(8);
        // A fresh profile opens the first-use Tree Inspection help here. It is modal and takes the keyboard, so close the tutorial
        // window after it has opened; the gate then exercises the same gameplay state a returning player sees. User-facing first-use
        // help is unchanged.
        ui.CloseHelp();
        yield return Frames(4);
        if (!player.IsInspecting || player.InspectedTree != tree) throw new InvalidOperationException("could not inspect " + tree.TreeId);
    }

    private static string F(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

    private IEnumerator ReviewStand()
    {
        ui.CloseAll(); ui.CloseHelp();
        foreach (string id in Bay().Future.Select(t => t.Id).ToArray())
        {
            yield return Inspect(Trees().First(t => t.TreeId == id));
            yield return Press(Key.J);
        }
        for (int part = 0; part < 6; part++)
        {
            var bay = Bay();
            ForestTree tree = Trees().First(t => !t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Portion(t.transform.position.x, t.transform.position.z) == part);
            yield return Inspect(tree); yield return Press(Key.J);
        }
        if (player.IsInspecting) yield return Press(Key.E);
        yield return Settled();
    }

    private IEnumerator Run()
    {
        ui = FindFirstObjectByType<ScenarioOneUiRoot>(); manager = ui.Manager; player = ui.Player; marks = ui.Marking; saves = FindFirstObjectByType<ForestSaveController>();
        Check(manager != null && player != null && marks != null && saves != null, "scene has live lesson dependencies");
        ui.CloseAll(); ui.CloseHelp(); yield return Frames(6);
        var planner = ui.RackTool.Planner;
        foreach (float across in planner.LayoutLines.ToArray())
        {
            planner.Refresh(); var proposal = planner.Propose(new RackPoint(0, across), 0f);
            if (proposal.CanConfirm) manager.ConfirmRack(proposal, "map");
        }
        Check(ui.ApproveWorkPlan(), "approve actual rack access work");
        Check(ui.AdvanceFromWorkPlan(), "execute actual rack access through UI action");
        yield return Frames(30); ui.RackLayout.Update(true); ui.CloseHelp();
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); yield return Settled();
        Check(LiveFacts().AccessOpen && LiveFacts().AccessReviewed, "live access executed and Annual Review acknowledged");
        var bay = Bay();
        var layoutLines = planner.LayoutLines.OrderBy(v => v).ToArray();
        Check(layoutLines.Length == 5, "the authored layout has five racks", "n=" + layoutLines.Length);
        float expectedHalfWidth = Mathf.Min(layoutLines[2] - layoutLines[1], layoutLines[3] - layoutLines[2]) * .5f;
        Check(bay.RackIndex == 2 && bay.RackCount == 5 && Mathf.Abs(bay.HalfWidthM - expectedHalfWidth) < .001f,
            "middle rack footprint is half the gap to its neighbouring layout racks", "half width " + F(bay.HalfWidthM));
        foreach (string id in new[] { "P0616", "P1112", "P1415" }) marks.Mark(Trees().First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        yield return Settled();
        Check(!Bay().HasEnoughFuture && ui.Learning.CurrentTopicIndex == 1, "clustered triple cannot complete Objective 2 live");
        foreach (var t in Trees().Where(t => t.IsCropTree)) marks.Unmark(t, false);
        foreach (string id in new[] { "P0714", "P1109", "P0706" }) marks.Mark(Trees().First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        yield return Settled(); bay = Bay();
        Check(bay.HasEnoughFuture && bay.NearSelected && bay.MiddleSelected && bay.FarSelected && bay.BothSides, "three distributed retained trees qualify");
        Check(ui.Learning.CurrentTopicIndex == 2, "Objective 3 follows distributed selections");
        ForestTree outside = Trees().First(t => !t.IsCropTree && !bay.Contains(t.transform.position.x,t.transform.position.z)
            && bay.Future.Any(f => Vector2.Distance(new Vector2(f.X,f.Z),new Vector2(t.transform.position.x,t.transform.position.z)) <= 8));
        marks.Mark(outside, TreeMarkType.Fell, false);
        Check(Bay().MarkedRemovals == 0 && Bay().MinWithMarks == 0, "nearby outside-footprint marks give no lesson credit");
        marks.Unmark(outside, false);
        for (int guard = 0; guard < 150; guard++)
        {
            bay = Bay(); if (bay.MinPlannedAndExecuted >= ScenarioOneRackBay.FirstReleaseShare) break;
            var f = bay.Future.OrderBy(t => t.ReleaseShare).First();
            ForestTree next = Trees().Where(t => !t.IsCropTree && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Contains(t.transform.position.x,t.transform.position.z)
                && Vector2.Distance(new Vector2(f.X,f.Z),new Vector2(t.transform.position.x,t.transform.position.z)) <= 8)
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.Diameter, f.DbhCm,
                    Vector2.Distance(new Vector2(f.X,f.Z),new Vector2(t.transform.position.x,t.transform.position.z)))).First();
            marks.Mark(next,TreeMarkType.Fell,false); manager.AddMarkedTreesToWorkPlan();
        }
        bay=Bay();
        Check(bay.ReleasePlanned && bay.CanopyKept, "initial in-footprint 30% plan is feasible live");
        foreach(var f in bay.Future)
        {
            yield return Inspect(Trees().First(t=>t.TreeId==f.Id));
            Check(Mathf.Abs(ui.Competitors.Report.PlannedShare-f.PlannedShare)<.005f,"panel and lesson initial share agree for "+f.Id);
        }
        if(player.IsInspecting)yield return Press(Key.E);
        var order=manager.WorkOrders.First(o=>o.IsOpen&&o.type==ScenarioWorkType.FellTree);
        Check(manager.SetPendingFellingOutcome(order.workOrderId,FellingMaterialOutcome.RetainAsFallenDeadwood),"set actual deadwood outcome");
        Check(Bay().MixedPlanned,"both actual outcomes counted inside footprint");
        // In-memory rollback of a deliberately excessive draft; approval must be denied by the actual interactive entry point.
        ForestSaveData safePlan=saves.CaptureData();
        foreach(var t in Trees().Where(t=>!t.IsCropTree&&bay.Contains(t.transform.position.x,t.transform.position.z)))marks.Mark(t,TreeMarkType.Fell,false);
        Check(!Bay().CanopyKept && !ui.ApproveWorkPlan() && !ui.AdvanceFromWorkPlan(),"over-cap interactive approval and execution refused");
        Check(saves.LoadData(safePlan,false),"restore valid in-memory plan"); yield return Frames(12);yield return Settled();
        Check(ui.ApproveWorkPlan(),"approve bounded intervention");
        Check(ui.AdvanceFromWorkPlan(),"execute bounded intervention through Work Plan UI action");
        yield return Frames(30);ui.CloseHelp();yield return Settled();
        Check(Bay().ReleaseExecuted&&Bay().MixedExecuted,"live executed release has sold and deadwood outcomes");
        Check(ui.Learning.CurrentTopicIndex==4&&!LiveFacts().RackReviewComplete,"Objective 5 requires real return review");
        ui.AcknowledgeAnnualReview();yield return Settled();
        Check(!LiveFacts().RackReviewComplete&&LiveFacts().RevisitedTrees==0,"generic acknowledgement gives no tree inspection credit");
        int removed=Bay().ExecutedRemovals;
        yield return ReviewStand();
        Check(LiveFacts().RackReviewComplete&&LiveFacts().RevisitedTrees==3&&LiveFacts().StandPortionsReviewed==6,"all three actual inspections and retained stand decisions complete return review");
        Check(Bay().ExecutedRemovals==removed&&ui.Learning.CurrentTopicIndex==5,"retain-here completes with zero further felling");
        ForestSaveData completed=saves.CaptureData();
        Check(saves.LoadData(completed,false),"reload after review with no schema additions");yield return Frames(12);yield return Settled();
        Check(!LiveFacts().RackReviewComplete&&LiveFacts().RevisitedTrees==0&&ui.Learning.CurrentTopicIndex==4,"reload requires fresh actual return inspection");
        yield return ReviewStand();
        Check(LiveFacts().RackReviewComplete&&Bay().ExecutedRemovals==removed,"fresh retain decisions after load restore lesson completion without cutting");
    }
}
