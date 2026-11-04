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
public static class TeachingCapUnityVerification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        RackUxShots.SetGameSize(1280, 720);
        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("TeachingCapUnityVerification.Begin"))
            new GameObject("Disposable T03 cap gate").AddComponent<TeachingCapUnityRunner>();
    }
}

public sealed class TeachingCapUnityRunner : MonoBehaviour
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
        Debug.Log(failure == null ? $"TEACHING_CAP_UNITY_PASS checks={checks} shots={shots}" : "TEACHING_CAP_UNITY_FAIL " + failure);
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
    Button Button(string text) => ((VisualElement)Field(workPlan,"footer")).Query<Button>().ToList().Single(b => b.text == text);
    Label Feedback() => (Label)Field(workPlan, "feedback");
    IEnumerator Render(string name, bool hard)
    {
        ui.ShowWorkPlan(); yield return Settle(); ui.CloseHelp(); yield return Settle();
        foreach (var size in new[] { new Vector2Int(1280,720), new Vector2Int(1920,1080) })
        {
            RackUxShots.SetGameSize(size.x, size.y);
            for (int i=0;i<120 && (Screen.width!=size.x || Screen.height!=size.y);i++) yield return null;
            yield return Settle(); workPlan.Refresh(true); yield return Settle();
            Check(Screen.width==size.x && Screen.height==size.y, name+" rendered resolution", Screen.width+"x"+Screen.height);
            Check(!ui.Help.IsOpen,"first-use help closed before capture");
            Button approve = Button("Approve Work Plan"), advance = Button("Advance one year");
            Check(approve.enabledInHierarchy == !hard && advance.enabledInHierarchy == !hard,
                name+" visible button states", $"approve={approve.enabledInHierarchy} advance={advance.enabledInHierarchy}");
            string message = Feedback().text;
            Check(message.Contains(hard ? "Teaching rack safeguard exceeded" : "Note (advisory)") && message.Contains("40%")
                && message.Contains("teaching footprint") && message.Contains(hard ? "Cancel/unmark" : "does not block"), name+" message", message);
            Rect viewport = ui.RootElement.worldBound;
            foreach (VisualElement element in new VisualElement[] { Feedback(), approve, advance })
            {
                Rect b=element.worldBound;
                Check(b.width>0 && b.height>0 && viewport.Contains(b.min) && viewport.Contains(b.max), name+" message/button fits", b.ToString());
            }
            Check(hard || ui.RootElement.panel.Pick(approve.worldBound.center) == approve ||
                approve.Contains(ui.RootElement.panel.Pick(approve.worldBound.center)), name+" Approve unobscured");
            File.WriteAllText(Path.Combine(output, name+"-"+size.x+"x"+size.y+".txt"), message+"\n"+
                $"Approve={approve.enabledInHierarchy} Advance={advance.enabledInHierarchy}\nshare={Bay().RemovedBasalShare} actionable={Bay().ActionableRemovals}");
            yield return new WaitForEndOfFrame();
            Texture2D image=ScreenCapture.CaptureScreenshotAsTexture();
            Check(image!=null && image.width==size.x && image.height==size.y, name+" capture dimensions");
            File.WriteAllBytes(Path.Combine(output, name+"-"+size.x+"x"+size.y+".png"), image.EncodeToPNG()); Destroy(image); shots++;
        }
        ui.CloseAll();
    }
    IEnumerator Run()
    {
        output=Environment.GetEnvironmentVariable("CCF_DIAG_DIR"); Directory.CreateDirectory(output);
        ui=FindFirstObjectByType<ScenarioOneUiRoot>(); manager=ui.Manager; saves=FindFirstObjectByType<ForestSaveController>();
        workPlan=(WorkPlanView)Field(ui,"workPlan"); ui.CloseAll(); ui.CloseHelp();
        string asset="Assets/ForestPrototype/UI/ScenarioOneTeachingCap.cs";
        Check(AssetDatabase.LoadAssetAtPath<MonoScript>(asset)!=null && AssetDatabase.AssetPathToGUID(asset)=="5b7d1c0e9a3f4e68b2c4d8a1f6e3079b", "hand-written meta imports with original GUID");
        Check(manager.RackPlanningV2Active && ui.Ecology.StandGeometryModelVersion==StandGeometryModel.Enlarged80, "production V2 Enlarged80 fixture");
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
        ForestSaveData accessed=saves.CaptureData();

        // The original hard safeguard, with actual cancellable work and no completed mixed sample.
        foreach(var t in Candidates()) ui.Marking.Mark(t,TreeMarkType.Fell,false);
        Check(!Bay().CanopyKept && Bay().ActionableRemovals>0,"lesson exceeds cap with actionable marks");
        int year=manager.CurrentEcologicalYear;
        Check(!ui.ApproveWorkPlan() && !ui.AdvanceFromWorkPlan() && manager.CurrentEcologicalYear==year,"hard entry points refuse without advancing");
        yield return Render("lesson-hard",true);
        yield return Restore(accessed);

        // Complete the bounded first release using real orders and execution, never fabricated completed records.
        PlanRelease();
        var order=manager.WorkOrders.First(o=>o.IsOpen && o.type==ScenarioWorkType.FellTree && o.rackId==0);
        Check(manager.SetPendingFellingOutcome(order.workOrderId,FellingMaterialOutcome.RetainAsFallenDeadwood),"first-release mixed outcome");
        Check(Bay().CanopyKept && Bay().ReleasePlanned,"bounded first release feasible");
        Check(ui.ApproveWorkPlan() && ui.AdvanceFromWorkPlan(),"bounded first release approves and advances"); yield return Settle();
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp(); yield return Settle();
        Check(ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(),Facts().ReleaseReviewed),"executed and reviewed sample complete");
        ForestSaveData completed=saves.CaptureData();
        foreach(var t in Candidates()) { Plan(t); if(Bay().RemovedBasalShare>.46f) break; }
        Check(Bay().RemovedBasalShare>.40f && Bay().ActionableRemovals>0,"cycle-2 thinning exceeds cumulative cap");
        Check(string.IsNullOrEmpty(ui.RackPlanProblem) && !string.IsNullOrEmpty(ui.RackPlanAdvisory),"cycle-2 cap advisory");
        string before=ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        yield return Render("cycle2-advisory",false);
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData())==before,"rendering read-only world hash");
        ForestSaveData laterPlan=saves.CaptureData(); yield return Restore(laterPlan);
        Check(string.IsNullOrEmpty(ui.RackPlanProblem) && !string.IsNullOrEmpty(ui.RackPlanAdvisory),"cycle-2 advisory survives reload");
        int history=manager.ManagementEvents.Count;
        Check(ui.ApproveWorkPlan(),"cycle-2 over-cap approves");
        Check(ui.AdvanceFromWorkPlan(),"cycle-2 over-cap advances"); yield return Settle();
        Check(manager.CurrentEcologicalYear==year+2 && manager.ManagementEvents.Count>history,"cycle-2 executed and appended history");
        ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp(); yield return Settle();
        Debug.Log("TEACHING_CAP_T04_RESIDUAL guide="+ui.Learning.CurrentTopicIndex+" share="+Bay().RemovedBasalShare);
        Check(Bay().ActionableRemovals==0 && string.IsNullOrEmpty(ui.RackPlanProblem),"executed cycle-2 excess cannot deadlock");

        // Sold-only and short mixed release: denominator drift with nothing actionable; then one mark and one plan.
        foreach(bool mixed in new[]{false,true})
        {
            yield return Restore(accessed);
            // Six removals around ONE Crop Tree deliberately leave the far Crop Tree unreleased.
            // Retaining three Crop Trees plus a living actionable sentinel still allows later denominator drift above 40%.
            var focus=Bay().Future.First();
            var shortTargets=Candidates().Where(t=>Vector2.Distance(new Vector2(focus.X,focus.Z),
                new Vector2(t.transform.position.x,t.transform.position.z))<8).OrderByDescending(t=>t.Diameter).Take(6).ToArray();
            Check(shortTargets.Length==6,"short-release fixture has six nearby targets");
            foreach(var target in shortTargets) Plan(target);
            if(mixed)
            {
                var shortOrder=manager.WorkOrders.Last(o=>o.IsOpen && o.rackId==0);
                Check(manager.SetPendingFellingOutcome(shortOrder.workOrderId,FellingMaterialOutcome.RetainAsFallenDeadwood),"short mixed deadwood outcome");
            }
            Check(ui.ApproveWorkPlan() && ui.AdvanceFromWorkPlan(),"short release executes"); yield return Settle();
            ui.AcknowledgeAnnualReview(); ui.CloseAll(); ui.CloseHelp(); yield return Settle();
            Check(!ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(),Facts().ReleaseReviewed),"short release not complete");
            float prior=Bay().RemovedBasalShare; int deaths=0;
            // Keep the largest remaining non-Crop Tree available for the one-more-mark/plan check.
            var survivors=Candidates().OrderBy(t=>t.Diameter).ToArray();
            foreach(var t in survivors.Take(survivors.Length-1))
            {
                if(Bay().RemovedBasalShare>.43f) break;
                t.ApplyMortality("self-thinning",manager.CurrentEcologicalYear); deaths++;
            }
            Check(Bay().RemovedBasalShare>.40f && Bay().RemovedBasalShare>prior && deaths>0 && Bay().ActionableRemovals==0,
                "forced mortality pushes short release over cap without actionable work", $"before={prior} after={Bay().RemovedBasalShare} deaths={deaths}");
            string label=mixed?"short-mixed-drift":"sold-only-drift";
            yield return Render(label+"-advisory",false);
            var drift=saves.CaptureData(); yield return Restore(drift);
            var extra=Candidates().First(); ui.Marking.Mark(extra,TreeMarkType.Fell,false);
            Check(!string.IsNullOrEmpty(ui.RackPlanProblem) && !ui.ApproveWorkPlan() && !ui.AdvanceFromWorkPlan(),"one more in-footprint mark restores hard cap");
            yield return Render(label+"-one-mark-hard",true);
            ui.Marking.Unmark(extra,false); Plan(extra);
            Check(!string.IsNullOrEmpty(ui.RackPlanProblem) && !ui.ApproveWorkPlan() && !ui.AdvanceFromWorkPlan(),"one more in-footprint plan restores hard cap");
            yield return Restore(drift);
            Check(!ui.ApproveWorkPlan() && manager.Feedback.Contains("no valid pending work"),"empty plan reaches normal approval no-op, not cap refusal");
            var outside=Live().First(t=>!t.IsCropTree && !Bay().Contains(t.transform.position.x,t.transform.position.z));
            Plan(outside);
            Check(Bay().ActionableRemovals==0 && string.IsNullOrEmpty(ui.RackPlanProblem),"outside-footprint ordinary work retains advisory");
            Check(ui.ApproveWorkPlan(),"drift-only footprint Approve works for ordinary outside work");
            Check(ui.AdvanceFromWorkPlan(),"drift-only Advance works"); yield return Settle();
        }
        yield return Restore(completed);
        Check(ScenarioOneTeachingCap.SampleHistoricallyComplete(Bay(),Facts().ReleaseReviewed),"completed sample reload preserved");
    }
}
#endif
