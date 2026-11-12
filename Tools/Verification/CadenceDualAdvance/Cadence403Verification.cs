#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

// Disposable T-03 gate: real Rack V2 access, marks/orders, UI approval/advance, mortality and in-memory reload.
// Mortality is a deliberately forced fixture using ApplyMortality, not a claim about storm frequency or ecology calibration.
// No save writes or scene saves. Stage with RackUxShots for its existing Game-view size helper, then remove all staged metas.
public static class Cadence403Verification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");

        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("Cadence403Verification.Begin"))
            new GameObject("Disposable T03 cap gate").AddComponent<Cadence403Runner>();
    }
}

public sealed class Cadence403Runner : MonoBehaviour
{
    ScenarioOneUiRoot ui;
    ScenarioOneManager manager;
    ForestSaveController saves;
    WorkPlanView workPlan;
    string output;
    int checks, shots;
    readonly List<string> evidence = new List<string>();
    static object Field(object o, string n) => o.GetType().GetField(n, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o);
    void Check(bool ok, string name, string detail = "")
    {
        checks++;
        string line = (ok ? "PASS " : "FAIL ") + name + " " + detail;
        evidence.Add(line); Debug.Log("TEACHING_CAP_CHECK " + line);
        if (!ok) throw new InvalidOperationException(line);
    }
    ForestTree[] Trees() => FindObjectsByType<ForestTree>(FindObjectsSortMode.None);
    ForestTree[] Live() => Trees().Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
    ScenarioOneTeachingSequence.Facts Facts() => (ScenarioOneTeachingSequence.Facts)typeof(LearningObjectivesView)
        .GetMethod("TeachingFacts", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(ui.Learning, null);
    ScenarioOneRackBay.Analysis Bay() { var ignored = ui.RackPlanProblem; return Facts().Bay; }
    IEnumerator Settle() { yield return new WaitForSecondsRealtime(.4f); for (int i = 0; i < 6; i++) yield return null; }
    IEnumerator Restore(ForestSaveData state)
    {
        Check(saves.LoadData(state, false), "in-memory reload accepted");
        yield return Settle(); ui.CloseHelp(); ui.CloseAll();
    }
    IEnumerator Start()
    {
        for (int i = 0; i < 12; i++) yield return null;
        Exception failure = null; var stack = new Stack<IEnumerator>(); stack.Push(Run());
        while (stack.Count > 0)
        {
            bool more; object next = null;
            try { more = stack.Peek().MoveNext(); if (more) next = stack.Peek().Current; }
            catch (Exception e) { failure = e; break; }
            if (!more) { stack.Pop(); continue; }
            if (next is IEnumerator child) stack.Push(child); else yield return next;
        }
        if (output != null) File.WriteAllLines(Path.Combine(output, "checks.txt"), evidence);
        Debug.Log(failure == null ? $"CADENCE403_PASS checks={checks} shots={shots}" : "CADENCE403_FAIL " + failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null ? 0 : 1);
    }
    void Plan(ForestTree tree)
    {
        ui.Marking.Mark(tree, TreeMarkType.Fell, false);
        manager.AddMarkedTreesToWorkPlan();
        Check(manager.IsPlannedForFelling(tree.TreeId), "ordinary felling plan created", tree.TreeId);
    }
    ForestTree[] Candidates()
    {
        var bay=Bay();
        return Live().Where(t => !t.IsCropTree && !t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId)
            && bay.Contains(t.transform.position.x,t.transform.position.z)).ToArray();
    }
    void PlanRelease()
    {
        for (int guard = 0; guard < 100 && Bay().MinPlannedAndExecuted < ScenarioOneRackBay.FirstReleaseShare; guard++)
        {
            var f = Bay().Future.OrderBy(t => t.ReleaseShare).First();
            var next = Candidates().Where(t => Vector2.Distance(new Vector2(f.X, f.Z),
                new Vector2(t.transform.position.x, t.transform.position.z)) <= ForestEcologyController.HegyiCutoffMeters)
                .OrderByDescending(t => ForestEcologyController.HegyiTerm(t.Diameter, f.DbhCm,
                    Vector2.Distance(new Vector2(f.X, f.Z), new Vector2(t.transform.position.x, t.transform.position.z)))).First();
            Plan(next);
        }
    }
    void ReviewTree(ForestTree tree) {
        ui.CloseAll(); ui.CloseHelp();
        typeof(ForestPlayer).GetMethod("InspectTree",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(ui.Player,new object[]{tree});
        ui.Learning.ObserveRackInspection(tree);
        Check(ui.Learning.DecideRackInspection(tree,false),"real retain-here inspection decision",tree.TreeId);
    }
    IEnumerator Run()
    {
        output=Environment.GetEnvironmentVariable("CCF_DIAG_DIR"); Directory.CreateDirectory(output);
        ui=FindFirstObjectByType<ScenarioOneUiRoot>(); manager=ui.Manager; saves=FindFirstObjectByType<ForestSaveController>();
        ui.CloseAll(); ui.CloseHelp();
        var original=saves.CaptureData();
        try {
        var planner=new RackPlanner(manager,ui.Ecology); planner.Refresh();
        foreach(float line in planner.LayoutLines)
        {
            Check(RackGeometry.ClipLineToSquare(new RackPoint(0,line),0,planner.HalfExtentM,out RackPoint start,out RackPoint end), "rack clips");
            Check(manager.AddRack(start,end).Succeeded,"V2 rack drafted");
        }
        Check(manager.ConfirmRackPlan().Succeeded,"V2 five-rack plan confirmed");
        Check(!ui.ApproveWorkPlan() && !string.IsNullOrEmpty(ui.RackApprovalProblem),"V2 field-marking approval gate preserved");
        foreach(var rack in manager.RackPlan) Check(manager.MarkRackEntrance(rack.Id).Succeeded,"V2 entrance marked");
        Check(ui.ApproveWorkPlan(),"V2 access approves after marking");
        Check(ui.AdvanceFromWorkPlan(),"V2 access executes through UI"); yield return Settle();
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp(); ui.RackLayout.Update(true); yield return Settle();
        Check(Facts().AccessOpen && Facts().AccessReviewed,"real V2 access open and reviewed");
        foreach(string id in new[]{"P0714","P1109","P0706"})
            ui.Marking.Mark(Live().Single(t=>t.TreeId==id),TreeMarkType.CropTree,false);
        Check(Bay().HasEnoughFuture,"three distributed Crop Trees accepted");
        PlanRelease();
        var order=manager.WorkOrders.First(o=>o.IsOpen && o.type==ScenarioWorkType.FellTree && o.rackId==0);
        Check(manager.SetPendingFellingOutcome(order.workOrderId,FellingMaterialOutcome.RetainAsFallenDeadwood),"retain real deadwood");
        Check(ui.ApproveWorkPlan() && ui.AdvanceFromWorkPlan(),"execute real release"); yield return Settle();
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp(); yield return Settle();
        Check(ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(),Facts().ReleaseReviewed),"authentic sample completed");
        foreach (string id in Bay().Future.Select(t=>t.Id).ToArray())
            ReviewTree(Live().Single(t=>t.TreeId==id));
        for(int part=0;part<6;part++) {
            var bay=Bay();
            ReviewTree(Live().First(t=>!t.IsMarkedForFell && !manager.IsPlannedForFelling(t.TreeId)
                && bay.Portion(t.transform.position.x,t.transform.position.z)==part));
        }
        ui.Learning.Refresh(true);
        Check(Facts().RackReviewComplete && saves.CaptureData().scenarioOne.firstGuidedRackReviewComplete,"authentic field review earns latch");
        Check(manager.TryPurchaseStock("beech-sapling",1) && manager.TryPurchaseStock("sessile-oak-sapling",1),"buy genuine planting stock");
        Check(manager.TryDesignatePlanting("beech-sapling",20) && manager.TryDesignatePlanting("sessile-oak-sapling",21),"designate real planting");
        Check(ui.ApproveWorkPlan() && ui.AdvanceFromWorkPlan(),"execute planting"); yield return Settle();
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp();
        Check(ui.AdvanceFromWorkPlan(),"real establishment year"); yield return Settle();
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp(); ui.ShowWorkPlan(); ui.Learning.Refresh(true);
        Debug.Log("CADENCE403_STAGE actual="+ui.Learning.CurrentTopicIndex+" problem="+ui.SkipProblem());
        Check(ui.Learning.CurrentTopicIndex==7,"genuine stage seven fixture");
        var authentic=saves.CaptureData();
        File.WriteAllText(Path.Combine(output,"authentic.json"),JsonUtility.ToJson(authentic));
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_F8") != "1") {
            yield return Cadence408Parity.Run(ui,saves,authentic);
            yield return Restore(authentic);
            var fell=Live().First(t=>!t.IsCropTree && !Bay().Contains(t.transform.position.x,t.transform.position.z));
            Plan(fell); Check(ui.ApproveWorkPlan() && ui.AdvanceFromWorkPlan(),"authentic post-teaching felling executed");
            yield return Settle(); ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp(); ui.ShowWorkPlan(); ui.Learning.Refresh(true);
            Check(ui.Learning.CurrentTopicIndex==7,"post-felling genuine stage seven");
            Debug.Log("CADENCE408_FIXTURE post-felling year="+manager.CurrentEcologicalYear+" target="+ui.SkipTarget(out int? anchor)+" fellingAnchor="+anchor);
            yield return Cadence408Parity.Run(ui,saves,saves.CaptureData());
            yield break;
        }
        for(int attempt=1;attempt<=2;attempt++) {
            yield return Restore(authentic); ui.ShowWorkPlan(); ui.Learning.Refresh(true);
            Check(ui.BeginSkip(ui.SkipTarget(out _)),"F8 genuine skip begins");
            while(ui.Skipping) yield return null;
            var summary=(Label)Field(Field(ui,"review"),"skipSummary");
            string prior=summary.text; int destination=manager.CurrentEcologicalYear;
            Check(!string.IsNullOrEmpty(prior),"F8 source summary exists",prior);
            yield return Restore(original); ui.ShowReview(); yield return Settle();
            Debug.Log("CADENCE404_F8 attempt="+attempt+" sourceYear="+destination+" loadedYear="+manager.CurrentEcologicalYear+" summary="+summary.text);
            yield return new WaitForEndOfFrame();
            var image=ScreenCapture.CaptureScreenshotAsTexture();
            File.WriteAllBytes(Path.Combine(output,"F8-attempt"+attempt+".png"),image.EncodeToPNG()); Destroy(image);
            Check(summary.text=="" && summary.resolvedStyle.display==DisplayStyle.None,"F8 different-world summary cleared and hidden");
        }
        Debug.Log("CADENCE408_F8_PASS twice");
        } finally { Check(saves.LoadData(original,false),"restore original world"); }
    }
}
#endif
