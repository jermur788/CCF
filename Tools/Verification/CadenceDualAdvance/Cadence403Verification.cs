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
// Disk writes are restricted to the driver-isolated config; no scene saves. Remove staged sources and metas.
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

[DefaultExecutionOrder(-10000)]
public sealed class Cadence403Runner : MonoBehaviour
{
    ScenarioOneUiRoot ui;
    ScenarioOneManager manager;
    ForestSaveController saves;
    WorkPlanView workPlan;
    string output;
    int checks, shots, caseFailures;
    bool holdTicks;
    void Update() { if(holdTicks && ui!=null && ui.Skipping) Set(ui,"skipFrame",Time.frameCount); }

    readonly List<string> evidence = new List<string>();
    static object Field(object o, string n) => o.GetType().GetField(n, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(o);
    void Check(bool ok, string name, string detail = "")
    {
        checks++;
        if(output!=null && saves!=null && ui?.Learning!=null && ui.Manager!=null && ui.Ecology!=null) { int stage=ui.Learning.CurrentTopicIndex;string path=Path.Combine(output,"stage-"+stage+".json");if(!File.Exists(path))File.WriteAllText(path,JsonUtility.ToJson(saves.CaptureData())); }
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
        Set(ui,"manager",manager);Set(ui,"ecology",FindFirstObjectByType<ForestEcologyController>());manager.enabled=true;manager.SetModalOpen(false);ui.enabled=true;
        Check(saves.LoadData(state, false), "in-memory reload accepted");
        Time.timeScale=1;yield return Settle();
        var session=FindFirstObjectByType<StandaloneSessionMenu>();if(session!=null)session.Close();
        ui.CloseHelp(); ui.CloseAll();manager.SetModalOpen(false);
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
        Debug.Log(failure == null && caseFailures==0 ? $"CADENCE403_PASS checks={checks} shots={shots}" : "CADENCE403_FAIL cases="+caseFailures+" "+failure);
        EditorApplication.ExitPlaymode(); EditorApplication.Exit(failure == null && caseFailures==0 ? 0 : 1);
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
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_ALLLOCKS")=="1") {
            if(FindFirstObjectByType<StandaloneSessionMenu>()==null) {new GameObject("Disposable real session menu fixture").AddComponent<StandaloneSessionMenu>();yield return Settle();FindFirstObjectByType<StandaloneSessionMenu>().Close();}
            var lockGallery=new GameObject("Disposable gallery input lock fixture").AddComponent<RecentAssetReviewControls>();lockGallery.enabled=false;
        }

        File.WriteAllText(Path.Combine(output,"authentic.json"),JsonUtility.ToJson(authentic));
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_SUITE")=="1") {
            yield return Case("parity/quiet",Cadence408Parity.Run(ui,saves,authentic));
            yield return Restore(authentic);ui.ShowWorkPlan();ui.Learning.Refresh(true);
            Plan(Live().First(t=>!t.IsCropTree&&!Bay().Contains(t.transform.position.x,t.transform.position.z)));
            Check(ui.ApproveWorkPlan()&&ui.AdvanceFromWorkPlan(),"suite authentic post-teaching felling");yield return Settle();ui.AcknowledgeAnnualReview();ui.ShowWorkPlan();ui.Learning.Refresh(true);
            var postFell=saves.CaptureData();Debug.Log("CADENCE408_FIXTURE post-felling year="+manager.CurrentEcologicalYear+" target="+ui.SkipTarget(out int? anchor)+" fellingAnchor="+anchor);
            yield return Case("parity/post-felling",Cadence408Parity.Run(ui,saves,postFell));
            yield return Matrix(authentic,original);
            yield return Extra(authentic,original);
            yield return Case("entry/rack-plan-problem",RackProblemCase(authentic,false));
            yield return Case("entry/rack-approval-problem",RackProblemCase(authentic,true));
            yield return Case("compat/old-no-cadence",OldCase(original));
            yield return Case("compat/reviewed-two-cycle",ReviewedCycles(authentic));
            yield return Case("stop/lesson-real-replanting",LessonCase(authentic));
            yield break;
        }
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_GUARDS")=="1") {
            yield return Case("entry/rack-plan-problem",RackProblemCase(authentic,false));
            yield return Case("entry/rack-approval-problem",RackProblemCase(authentic,true));
            yield return Case("compat/old-no-cadence",OldCase(original));
            yield return Case("compat/reviewed-two-cycle",ReviewedCycles(authentic));
            yield return Case("stop/lesson-real-replanting",LessonCase(authentic));
            yield return Case("locks/native-F5-F9-F10",NativeLocks(authentic));
            yield break;
        }
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_REPAIR")=="1") {
            yield return Case("stop/lesson-unmark-diagnostic",UnmarkDiagnostic(authentic));
            yield return Case("stop/modal-diagnostic",ModalDiagnostic(authentic));
            yield return Case("stop/lesson-real-replanting",LessonCase(authentic));
            yield return Case("fault/manager-reference-loss",LossCase(authentic,true));
            yield return Case("fault/ecology-reference-loss",LossCase(authentic,false));
            yield return Case("F8/one-year-clears",OneYearClear(authentic));
            yield return Case("locks/native-F5-F9-F10",NativeLocks(authentic));
            yield return Case("timing/stress5",TimingCase(authentic,5));
            yield return Case("timing/normal4",TimingCase(authentic,4));
            yield return Case("boundary/24-to25",BoundaryCase(authentic,24));
            yield return Case("boundary/99-to100",BoundaryCase(authentic,99));
            yield break;
        }
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_EXTRA")=="1") { yield return Extra(authentic,original); yield break; }
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_UI")=="1") { yield return UiMatrix(authentic,original); yield break; }
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_MATRIX")=="1") { yield return Matrix(authentic,original); yield break; }
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
        } finally { Set(ui,"manager",manager);Set(ui,"ecology",FindFirstObjectByType<ForestEcologyController>());manager.SetModalOpen(false);Check(saves.LoadData(original,false),"restore original world"); }
    }
    static void Set(object o,string n,object v) => o.GetType().GetField(n,BindingFlags.NonPublic|BindingFlags.Instance).SetValue(o,v);
    IEnumerator Case(string name,IEnumerator routine) {
        Exception failure=null;var frames=new Stack<IEnumerator>();frames.Push(routine);
        while(frames.Count>0) {
            bool more=false; object next=null;
            try {more=frames.Peek().MoveNext();if(more)next=frames.Peek().Current;}catch(Exception e){failure=e;break;}
            if(!more){frames.Pop();continue;}if(next is IEnumerator c)frames.Push(c);else yield return next;
        }
        if(failure!=null)while(frames.Count>0) {try {(frames.Pop() as IDisposable)?.Dispose();}catch(Exception cleanup){Debug.Log("CADENCE408_CLEANUP "+cleanup);}}
        holdTicks=false;
        if(ui.Skipping) {Set(ui,"cancelSkip",true);while(ui.Skipping)yield return null;}
        if(failure!=null)caseFailures++;
        Debug.Log("CADENCE408_CASE "+name+" "+(failure==null?"PASS":"FAIL "+failure));
    }
    IEnumerator RejectCase(ForestSaveData state,string name,Action action,string reason,Action undo=null) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        try { action();Debug.Log("CADENCE408_REASON "+name+" exact="+ui.SkipProblem());
            string before=JsonUtility.ToJson(saves.CaptureData());CadenceParity.Reject(ui,reason);
            Check(before==JsonUtility.ToJson(saves.CaptureData()),"entry blocker full saved-world nonmutation",name); }
        finally {undo?.Invoke();}
    }
    IEnumerator StopCase(ForestSaveData state,string name,Action action,string reason,int k) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        int start=manager.CurrentEcologicalYear;var inputs=InputStates();
        yield return CadenceParity.RunStop(ui,action,reason,k);
        CadenceParity.RejectWrongStopReason(ui,start,manager.CurrentEcologicalYear,reason);
        LogInputs(name,inputs);DestinationAck();
    }
    IEnumerator Matrix(ForestSaveData state,ForestSaveData original) {
        yield return Case("entry/early-original",RejectCase(original,"early-original",()=>{},"Finish the current lesson"));
        yield return Case("entry/closed-plan",RejectCase(state,"closed-plan",()=>manager.OpenWorkPlan(false),"Open the Work Plan first."));
        yield return Case("entry/paused",RejectCase(state,"paused",()=>Time.timeScale=0,"Finish the open question",()=>Time.timeScale=1));
        yield return Case("entry/modal",RejectCase(state,"modal",()=>manager.SetModalOpen(true),"Finish the open question",()=>manager.SetModalOpen(false)));
        yield return Case("entry/authoring",RejectCase(state,"authoring",()=>manager.SetReferenceAuthoring(true),"Return to your forest",()=>manager.SetReferenceAuthoring(false)));
        yield return Case("entry/controller",RejectCase(state,"controller",()=>manager.enabled=false,"Forest controller unavailable.",()=>manager.enabled=true));
        foreach(var status in new[]{ScenarioWorkStatus.Pending,ScenarioWorkStatus.Approved}) foreach(bool invalid in new[]{false,true}) foreach(int origin in new[]{0,1}) {
            var st=status;var bad=invalid;var prescription=origin;
            string name="entry/order/"+st+"/invalid="+bad+"/prescription="+origin;
            yield return Case(name,RejectCase(state,name,()=>{
                var data=manager.CaptureSaveData();data.workOrders.Add(new ScenarioOneWorkOrder{workOrderId=data.nextWorkOrderId++,status=st,type=ScenarioWorkType.FellTree,prescriptionId=prescription,validationMessage=bad?"invalid disposable fixture":""});manager.RestoreSaveData(data);ui.Learning.Refresh(true);
            },"Pending and Approved"));
        }
        yield return Case("stop/cancel0",StopCase(state,"cancel0",()=>Set(ui,"cancelSkip",true),"Cancelled between years.",0));
        yield return Case("stop/cancel1",StopCase(state,"cancel1",()=>Set(ui,"cancelSkip",true),"Cancelled between years.",1));
        yield return Case("stop/authoring",StopCase(state,"authoring",()=>manager.SetReferenceAuthoring(true),"Return to your forest first.",1));manager.SetReferenceAuthoring(false);
        yield return Case("stop/controller-disabled",StopCase(state,"controller",()=>manager.enabled=false,"Forest controller unavailable.",1));manager.enabled=true;
        yield return Case("stop/new-pending",StopCase(state,"pending",()=>Plan(Live().First(t=>!t.IsCropTree&&!Bay().Contains(t.transform.position.x,t.transform.position.z))),"Resolve or remove all Pending and Approved work first.",1));
        foreach(var outcome in new[]{ScenarioOneOutcome.Completed,ScenarioOneOutcome.Failed}) {
            var result=outcome;
            yield return Case("stop/outcome/"+result,StopCase(state,"outcome",()=>typeof(ScenarioOneManager).GetMethod("SetOutcome",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(manager,new object[]{result,manager.CurrentEcologicalYear,"Disposable qualification injection"}),"Scenario outcome changed.",1));
        }
        yield return Case("stop/modal-between-ticks",StopCase(state,"modal",()=>ui.Help.Show(MenuHelpView.Menu.WorkPlan),"Finish the open question or editing action first.",1));
        yield return Case("stop/Failed-cash-zero-injected",CashOutcomeCase(state));
        yield return Case("fault/OnDisable",DisableCase(state));
        yield return Case("fault/exception",ExceptionCase(state));
        yield return Case("compatibility/disk",DiskCase(state));
        var archive=ScenarioReferenceArchive.Load();
        foreach(int year in new[]{0,20,50,100}) {
            var m=archive.AtYear(year);Debug.Log("CADENCE408_REFERENCE year="+year+" current="+ScenarioReferenceArchive.CurrentWorldHash(m.world)+" world="+ScenarioReferenceArchive.WorldHash(m.world));
        }
    }
    IEnumerator DisableCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        var prior=FindObjectsByType<Behaviour>(FindObjectsSortMode.None).Where(b=>b is ForestSaveController||b is StandaloneSessionMenu||b is ForestPlayer||b is ForestTreeMarkingManager||b is RackTool||b is ForestBuildable||b is ForestSawPit||b is ForestWoodStorage||b is RecentAssetReviewControls).ToDictionary(b=>b,b=>b.enabled);
        Check(ui.BeginSkip(ui.SkipTarget(out _)),"disable begin");Check(prior.Keys.All(b=>!b.enabled),"disable all input types locked");ui.enabled=false;
        Check(!ui.Skipping&&prior.All(p=>p.Key.enabled==p.Value),"OnDisable restores input states");LogInputs("OnDisable",prior);ui.enabled=true;
    }
    IEnumerator ExceptionCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        var exceptionInputs=InputStates();Check(ui.BeginSkip(ui.SkipTarget(out _)),"exception begin");Set(ui,"skipProgress",null);
        while(ui.Skipping)yield return null;
        var label=(Label)Field(Field(ui,"review"),"skipSummary");
        Check(label.text.StartsWith("No years advanced. Stopped after an error: "),"exception exact summary",label.text);Check(exceptionInputs.All(p=>p.Key.enabled==p.Value),"exception restores all nine input types");LogInputs("exception",exceptionInputs);CadenceParity.RejectWrongStopReason(ui,manager.CurrentEcologicalYear,manager.CurrentEcologicalYear,label.text.Substring("No years advanced. ".Length));DestinationAck();
    }
    IEnumerator DiskCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);Check(ui.BeginSkip(ui.SkipTarget(out _)),"disk skip begins");while(ui.Skipping)yield return null;
        Check(Application.persistentDataPath.StartsWith("/media/jer/Files/CCF/Verification/CadenceV1-FixedQual-408-"),"isolated disk save custody",Application.persistentDataPath);var before=saves.CaptureData();saves.Save();saves.Load();yield return Settle();
        var reloaded=saves.CaptureData();ScenarioReferenceArchive.Canonicalize(before);ScenarioReferenceArchive.Canonicalize(reloaded);
        Check(JsonUtility.ToJson(before)==JsonUtility.ToJson(reloaded),"disk roundtrip complete canonical JSON");
        Check(ScenarioReferenceArchive.CurrentWorldHash(before)==ScenarioReferenceArchive.CurrentWorldHash(reloaded),"disk roundtrip world hash");ui.ShowReview();yield return Settle();
        var label=(Label)Field(Field(ui,"review"),"skipSummary");Check(label.text==""&&label.resolvedStyle.display==DisplayStyle.None,"disk reload clears hidden session summary");
    }

    readonly RackAuthoringNativeInput native=new RackAuthoringNativeInput();
    IEnumerator NativeButton(Button button) {
        string text=button?.text;yield return Settle();button=ButtonText(text)??button;
        Check(button!=null && button.enabledInHierarchy,"native input target exists and enabled",button?.text);
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_NATIVE")=="1") {
            var center=button.worldBound.center;var root=ui.RootElement.worldBound;
            var view=EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));
            var rectProperty=view.GetType().GetProperty("targetInParent",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
            Rect rect=rectProperty!=null?(Rect)rectProperty.GetValue(view):new Rect(0,21,view.position.width,view.position.height-21);
            Check(!float.IsNaN(center.x)&&!float.IsNaN(center.y),"native target has completed layout",text);
            int x=Mathf.RoundToInt(view.position.x+rect.x+center.x/root.width*rect.width),y=Mathf.RoundToInt(view.position.y+rect.y+center.y/root.height*rect.height);
            Debug.Log("CADENCE408_NATIVE_RECT window="+view.position+" rect="+rect);
            Debug.Log("CADENCE408_NATIVE_CLICK "+button.text+" panel="+center+" root="+root+" desktop="+x+","+y);
            yield return native.Send(new RackAuthoringNativeInput.Op{op="absolute",x=x,y=y},RackAuthoringNativeInput.Op.Click());
            yield break;
        }
        Debug.Log("CADENCE408_UI_INPUT registered Clickable callback; OS focus/Return attempt did not activate in runtime4");
        typeof(Clickable).GetMethod("Invoke",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(button.clickable,new object[]{null}); yield return null;
    }
    IEnumerator Shot(string name) {
        yield return Settle();yield return new WaitForEndOfFrame();
        var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(output,name+".png"),image.EncodeToPNG());
        Debug.Log("CADENCE408_SCREENSHOT "+name+" size="+image.width+"x"+image.height);Destroy(image);shots++;
    }
    Button ButtonText(string text) => ui.RootElement.Query<Button>().ToList().FirstOrDefault(b=>b.text==text);
    IEnumerator UiMatrix(ForestSaveData state,ForestSaveData original) {
        foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080)})
            yield return Case("UI/"+size.x+"x"+size.y,UiSize(state,original,size));
    }
    IEnumerator UiSize(ForestSaveData state,ForestSaveData original,Vector2Int size) {
            Cadence408GameSize.SetGameSize(size.x,size.y);yield return Settle();
            string tag=size.x+"x"+size.y;
            yield return Restore(original);ui.ShowWorkPlan();yield return Shot(tag+"-disabled-work-plan");
            yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);yield return Shot(tag+"-enabled-work-plan");
            yield return NativeButton(ui.RootElement.Query<Button>().ToList().First(b=>b.text.StartsWith("Skip to Year")));
            yield return Shot(tag+"-confirmation");
            var confirm=ButtonText("Confirm skip");
            Plan(Live().First(t=>!t.IsCropTree&&!Bay().Contains(t.transform.position.x,t.transform.position.z)));
            typeof(Clickable).GetMethod("Invoke",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(confirm.clickable,new object[]{null});
            Check(!ui.Skipping && ButtonText("Confirm skip")==null && ui.RootElement.Query<Label>().ToList().Any(l=>l.text.Contains("Pending and Approved")),"F7 refused confirmation redraws current pending reason");
            yield return Shot(tag+"-F7-refused-current-reason");
            yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
            yield return NativeButton(ui.RootElement.Query<Button>().ToList().First(b=>b.text.StartsWith("Skip to Year")));
            holdTicks=true;yield return NativeButton(ButtonText("Confirm skip"));
            Check(ui.Skipping,"native Confirm begins held between-tick skip");
            holdTicks=false;yield return null;holdTicks=true;
            Check(ui.Skipping,"progress remains active after first ordinary tick");
            if(ui.Skipping) {
                yield return new WaitForEndOfFrame();var image=ScreenCapture.CaptureScreenshotAsTexture();File.WriteAllBytes(Path.Combine(output,tag+"-progress.png"),image.EncodeToPNG());Destroy(image);
            }
            holdTicks=false;while(ui.Skipping)yield return null;
            var summary=(Label)Field(Field(ui,"review"),"skipSummary");Check(!string.IsNullOrEmpty(summary.text),"immediate session summary");
            yield return Shot(tag+"-destination-summary");
            var review=Field(ui,"review");yield return NativeButton((Button)Field(review,"earlierYear"));yield return Shot(tag+"-earlier-year");
            yield return NativeButton((Button)Field(review,"laterYear"));yield return Shot(tag+"-latest-year");
            yield return Restore(original);ui.ShowReview();yield return Shot(tag+"-loaded-world-hidden-summary");
            Check(summary.text==""&&summary.resolvedStyle.display==DisplayStyle.None,"F8 empty label no layout space");
            yield return OneYearClear(state);yield return Shot(tag+"-one-year-cleared-summary");
            yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);int cancelStart=manager.CurrentEcologicalYear;var prior=InputStates();
            yield return NativeButton(ui.RootElement.Query<Button>().ToList().First(b=>b.text.StartsWith("Skip to Year")));
            holdTicks=true;yield return NativeButton(ButtonText("Confirm skip"));Check(ui.Skipping,"native cancellation fixture begins");
            yield return NativeButton(ButtonText("Cancel after the current year"));holdTicks=false;while(ui.Skipping)yield return null;
            CadenceParity.RejectWrongStopReason(ui,cancelStart,cancelStart,"Cancelled between years.");LogInputs("native-cancel",prior);DestinationAck();
            yield return Shot(tag+"-native-cancel-zero-years");
    }

    IEnumerator Extra(ForestSaveData state,ForestSaveData original) {
        if(FindFirstObjectByType<StandaloneSessionMenu>()==null) {new GameObject("Disposable real session menu fixture").AddComponent<StandaloneSessionMenu>();yield return Settle();FindFirstObjectByType<StandaloneSessionMenu>().Close();}
        var gallery=new GameObject("Disposable gallery input lock fixture").AddComponent<RecentAssetReviewControls>();gallery.enabled=false;

        for(int stage=0;stage<7;stage++) {
            string path=Path.Combine(output,"stage-"+stage+".json");
            if(File.Exists(path)) yield return Case("entry/stage-"+stage,RejectCase(JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(path)),"stage-"+stage,()=>Debug.Log("CADENCE408_STAGE restored="+ui.Learning.CurrentTopicIndex),"Finish the current lesson"));
            else Debug.Log("CADENCE408_NOT_RUN entry/stage-"+stage+" no authentic stage snapshot observed during all real teaching handlers");
        }
        yield return Case("MFM/historical-approved",MfmCase(state));
        yield return Case("entry/unread",RejectCase(state,"unread",()=>{Check(ui.AdvanceFromWorkPlan(),"genuine unread year");ui.ShowWorkPlan();},"Read and acknowledge"));
        yield return Case("entry/preview-injected",RejectCase(state,"preview",()=>Set(manager,"referencePreviewActive",true),"Return to your forest",()=>Set(manager,"referencePreviewActive",false)));
        var map=Field(ui,"map");var editor=map.GetType().GetProperty("RackEditor").GetValue(map);
        yield return Case("entry/reset-pending",RejectCase(state,"reset",()=>Set(editor,"<ResetPending>k__BackingField",true),"Finish the open question",()=>Set(editor,"<ResetPending>k__BackingField",false)));
        yield return Case("entry/reopen-pending",RejectCase(state,"reopen",()=>Set(map,"reopenPending",true),"Finish the open question",()=>Set(map,"reopenPending",false)));
        yield return Case("entry/consent",RejectCase(state,"consent",()=>Set(editor,"consentRetry",new Func<RackPlanOperationResult>(()=>default)),"Finish the open question",()=>Set(editor,"consentRetry",null)));
        var gesture=editor.GetType().GetField("gesture",BindingFlags.NonPublic|BindingFlags.Instance);
        yield return Case("entry/drag",RejectCase(state,"drag",()=>{gesture.SetValue(editor,Enum.Parse(gesture.FieldType,"Create"));Set(editor,"dragged",true);},"Finish the open question",()=>{gesture.SetValue(editor,Enum.Parse(gesture.FieldType,"None"));Set(editor,"dragged",false);}));
        var ghost=editor.GetType().GetField("ghost",BindingFlags.NonPublic|BindingFlags.Instance);
        yield return Case("entry/ghost",RejectCase(state,"ghost",()=>ghost.SetValue(editor,Activator.CreateInstance(Nullable.GetUnderlyingType(ghost.FieldType))),"Finish the open question",()=>ghost.SetValue(editor,null)));
        yield return Case("stop/preview-injected",StopCase(state,"preview",()=>Set(manager,"referencePreviewActive",true),"Return to your forest first.",1));Set(manager,"referencePreviewActive",false);
        yield return Case("stop/new-approved",StopCase(state,"approved",()=>{Plan(Live().First(t=>!t.IsCropTree&&!Bay().Contains(t.transform.position.x,t.transform.position.z)));Check(manager.ApprovePendingWork(),"new work approved by real handler");},"Resolve or remove all Pending and Approved work first.",1));
        yield return Case("entry/help",RejectCase(state,"help",()=>ui.Help.Show(MenuHelpView.Menu.WorkPlan),"Finish the open question",()=>ui.CloseHelp()));
        var session=FindFirstObjectByType<StandaloneSessionMenu>();
        yield return Case("entry/session",RejectCase(state,"session",()=>session.Open(),"Finish the open question",()=>session.Close()));
        var rack=(RackTool)Field(ui,"rackTool");
        yield return Case("entry/rack-active",RejectCase(state,"rack-active",()=>Set(rack,"active",true),"Finish the open question",()=>rack.Exit()));
        yield return Case("entry/rack-intro",RejectCase(state,"rack-intro",()=>Set(rack,"introPending",true),"Finish the open question",()=>Set(rack,"introPending",false)));
        yield return Case("entry/advance-warning",RejectCase(state,"advance-warning",()=>Set(workPlan??(WorkPlanView)Field(ui,"workPlan"),"confirmingAdvance",true),"Finish the open question",()=>Set(Field(ui,"workPlan"),"confirmingAdvance",false)));
        foreach(var outcome in new[]{ScenarioOneOutcome.Failed}) {
            yield return Case("entry/ecology-unavailable",RejectCase(state,"ecology",()=>Set(ui,"ecology",null),"Forest controller unavailable.",()=>Set(ui,"ecology",FindFirstObjectByType<ForestEcologyController>())));
            yield return Case("entry/Failed",RejectCase(state,"Failed",()=>typeof(ScenarioOneManager).GetMethod("SetOutcome",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(manager,new object[]{outcome,manager.CurrentEcologicalYear,"Disposable injection"}),"Year advance is unavailable."));
        }
        yield return Case("stop/help-real-modal",StopCase(state,"help",()=>ui.Help.Show(MenuHelpView.Menu.WorkPlan),"Finish the open question or editing action first.",1));ui.CloseHelp();
        yield return Case("fault/false-tick2",FalseCase(state));
        yield return Case("locks/pre-disabled",PreDisabled(state));
        yield return Case("F9/crop-mortality",MortalityCase(state));
        yield return Case("compat/Legacy40",LegacyCase());
        yield return Case("stop/storm",StormCase(state));
        yield return Case("stop/lesson-real-replanting",LessonCase(state));
        yield return Case("fault/manager-reference-loss",LossCase(state,true));
        yield return Case("fault/ecology-reference-loss",LossCase(state,false));
        yield return Case("F8/one-year-clears",OneYearClear(state));
        if(Environment.GetEnvironmentVariable("CCF_CADENCE_NATIVE")=="1")yield return Case("locks/native-F5-F9-F10",NativeLocks(state));
        yield return Case("timing/stress5",TimingCase(state,5));
        yield return Case("timing/normal4",TimingCase(state,4));
        yield return Case("boundary/24-to25",BoundaryCase(state,24));
        yield return Case("boundary/99-to100",BoundaryCase(state,99));
    }
    Dictionary<Behaviour,bool> InputStates() => FindObjectsByType<Behaviour>(FindObjectsSortMode.None).Where(b=>b is ForestSaveController||b is StandaloneSessionMenu||b is ForestPlayer||b is ForestTreeMarkingManager||b is RackTool||b is ForestBuildable||b is ForestSawPit||b is ForestWoodStorage||b is RecentAssetReviewControls).ToDictionary(b=>b,b=>b.enabled);
    IEnumerator FalseCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);var inputs=InputStates();int start=manager.CurrentEcologicalYear;
        Check(ui.BeginSkip(ui.SkipTarget(out _)),"false tick begin");while(ui.Skipping&&manager.CurrentEcologicalYear==start)yield return null;
        var prior=manager.CaptureSaveData().annualReports.Select(r=>JsonUtility.ToJson(r)).ToArray();Set(manager,"cashCents",-1L);
        while(ui.Skipping)yield return null;
        var label=(Label)Field(Field(ui,"review"),"skipSummary");Debug.Log("CADENCE408_FALSE exact="+label.text);
        Check(manager.CurrentEcologicalYear==start+1 && label.text.Contains("Year advance failed: "),"failed second tick keeps first year");
        Check(prior.SequenceEqual(manager.CaptureSaveData().annualReports.Select(r=>JsonUtility.ToJson(r)))&&inputs.All(p=>p.Key.enabled==p.Value),"false tick records and inputs preserved");LogInputs("false-tick2",inputs);CadenceParity.RejectWrongStopReason(ui,start,start+1,"Year advance failed: "+manager.Feedback);DestinationAck();
    }
    IEnumerator PreDisabled(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);bool old=ui.Player.enabled;ui.Player.enabled=false;var inputs=InputStates();
        try {Check(ui.BeginSkip(ui.SkipTarget(out _)),"pre-disabled begin");foreach(var group in inputs.Keys.GroupBy(b=>b.GetType().Name))Debug.Log("CADENCE408_LOCK type="+group.Key+" instances="+group.Count()+" disabled="+group.All(b=>!b.enabled));Set(ui,"cancelSkip",true);while(ui.Skipping)yield return null;Check(inputs.All(p=>p.Key.enabled==p.Value),"all prior states restored including disabled player");}finally{ui.Player.enabled=old;}
    }
    IEnumerator MortalityCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);string id=Live().First(t=>t.IsCropTree).TreeId;
        Check(Live().Single(t=>t.TreeId==id).ApplyMortality("qualification identical forced mortality",manager.CurrentEcologicalYear+1),"direct real mortality API");Check(manager.AdvanceYear(),"direct mortality annual tick");var direct=saves.CaptureData();
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);Check(ui.BeginSkip(ui.SkipTarget(out _)),"mortality begin");Set(ui,"skipTarget",manager.CurrentEcologicalYear+1);Check(!ui.Marking.enabled,"marking disabled at death");
        Check(Live().Single(t=>t.TreeId==id).ApplyMortality("qualification identical forced mortality",manager.CurrentEcologicalYear+1),"skip real mortality API");while(ui.Skipping)yield return null;
        var actual=saves.CaptureData();Debug.Log("CADENCE408_MORTALITY direct="+ScenarioReferenceArchive.CurrentWorldHash(direct)+" skip="+ScenarioReferenceArchive.CurrentWorldHash(actual));
        ScenarioReferenceArchive.Canonicalize(direct);ScenarioReferenceArchive.Canonicalize(actual);Check(JsonUtility.ToJson(direct)==JsonUtility.ToJson(actual),"F9 full canonical JSON direct parity");
        Check(ScenarioReferenceArchive.CurrentWorldHash(direct)==ScenarioReferenceArchive.CurrentWorldHash(actual),"F9 current world equality");yield return new WaitForSecondsRealtime(.7f);
        Check(!ui.Marking.GetCropTreeIds().Contains(id),"dead Crop Tree absent after stale sweep");Debug.Log("CADENCE408_MORTALITY counts crop="+ui.Marking.LivingCropTreeCount+" fell="+ui.Marking.MarkedCount);
    }
    IEnumerator LegacyCase() {
        var legacy=ScenarioReferenceArchive.Load().AtYear(20).world;Check(legacy.standGeometryModel==0,"genuine frozen Legacy40 fixture");yield return Restore(legacy);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        Debug.Log("CADENCE408_LEGACY stage="+ui.Learning.CurrentTopicIndex+" reason="+ui.SkipProblem()+" version="+legacy.version);
        if(ui.Learning.CurrentTopicIndex!=7)CadenceParity.Reject(ui,"Finish the current lesson");else Check(ui.BeginSkip(ui.SkipTarget(out _)),"genuine legacy stage7 eligible");
    }
    IEnumerator StormCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);ui.Ecology.StormModelVersion=StormModel.WindthrowV1;ui.Ecology.ForceStormNextYear(1f,90f);
        int start=manager.CurrentEcologicalYear;var inputs=InputStates();Check(ui.BeginSkip(ui.SkipTarget(out _)),"forced real storm begins");while(ui.Skipping)yield return null;
        var label=(Label)Field(Field(ui,"review"),"skipSummary");Debug.Log("CADENCE408_STORM victims="+ui.Ecology.LastStormPerformance.Victims+" events="+manager.StormEvents.Count+" reason="+label.text);
        Check(ui.Ecology.LastStormPerformance.Victims>0&&manager.StormEvents.Count>0&&label.text.Contains("Storm recorded."),"real forced storm event victims stop");LogInputs("storm",inputs);CadenceParity.RejectWrongStopReason(ui,start,start+1,"Storm recorded.");DestinationAck();
    }
    IEnumerator BoundaryCase(ForestSaveData state,int boundary) {
        string reuse=Environment.GetEnvironmentVariable("CCF_CADENCE_BOUNDARY_SOURCE");
        if(!string.IsNullOrEmpty(reuse)) {
            string path=Path.Combine(reuse,"authentic-boundary-"+boundary+".json");
            Check(File.Exists(path),"authentic exported boundary exists",path);
            state=JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(path));Check(state.ecologicalYear==boundary,"exported true boundary year");
        }
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        for(int year=manager.CurrentEcologicalYear;year<=boundary;year++) {
            if(year==boundary) {
                ui.ShowReview();ui.AcknowledgeAnnualReview();ui.ShowWorkPlan();ui.Learning.Refresh(true);Debug.Log("CADENCE408_BOUNDARY start="+year+" stage="+ui.Learning.CurrentTopicIndex+" reason="+ui.SkipProblem());
                var boundaryState=saves.CaptureData();
                Check(ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(manager.ManagementEvents,manager.WorkOrders),"boundary genuine reviewed two-cycle predicate");
                Check(manager.AdvanceYear(),"boundary direct control advance");var direct=saves.CaptureData();
                yield return Restore(boundaryState);ui.ShowWorkPlan();ui.Learning.Refresh(true);
                File.WriteAllText(Path.Combine(output,"authentic-boundary-"+boundary+".json"),JsonUtility.ToJson(boundaryState));int objectiveCount=manager.ObjectiveAchievements.Count;var boundaryInputs=InputStates();var priorOutcome=manager.Outcome;
                if(ui.BeginSkip(ui.SkipTarget(out _))) {
                    while(ui.Skipping)yield return null;
                    string reason=manager.Outcome!=priorOutcome?"Scenario outcome changed.":manager.ObjectiveAchievements.Count>objectiveCount?"An objective was achieved.":"Review boundary reached.";
                    Debug.Log("CADENCE408_BOUNDARY destination="+manager.CurrentEcologicalYear+" summary="+((Label)Field(Field(ui,"review"),"skipSummary")).text);
                    var actual=saves.CaptureData();ScenarioReferenceArchive.Canonicalize(direct);ScenarioReferenceArchive.Canonicalize(actual);
                    Check(JsonUtility.ToJson(direct)==JsonUtility.ToJson(actual),"boundary full JSON direct parity");
                    Debug.Log("CADENCE408_BOUNDARY_PARITY year="+manager.CurrentEcologicalYear+" direct="+ScenarioReferenceArchive.CurrentWorldHash(direct)+" skip="+ScenarioReferenceArchive.CurrentWorldHash(actual)+" outcome="+manager.Outcome);
                    Check(manager.CurrentEcologicalYear==boundary+1&&boundaryInputs.All(p=>p.Key.enabled==p.Value),"boundary exact tick and nine restore");
                    CadenceParity.RejectWrongStopReason(ui,boundary,boundary+1,reason);DestinationAck();
                    if(boundary==24) {
                        yield return Restore(boundaryState);ui.ShowWorkPlan();ui.Learning.Refresh(true);Check(ui.BeginSkip(ui.SkipTarget(out _)),"isolated boundary reason begin");Set(ui,"skipObjectives",int.MaxValue);Set(ui,"skipOutcome",manager.Outcome==ScenarioOneOutcome.Active?ScenarioOneOutcome.Completed:manager.Outcome);
                        while(ui.Skipping)yield return null;CadenceParity.RejectWrongStopReason(ui,24,25,"Review boundary reached.");DestinationAck();
                        Debug.Log("CADENCE408_BOUNDARY_ISOLATED PASS test-only stop-priority snapshots suppress objective and outcome stops; authoritative state/history unchanged");
                        yield return Restore(boundaryState);ui.ShowWorkPlan();ui.Learning.Refresh(true);Check(ui.BeginSkip(ui.SkipTarget(out _)),"objective priority isolation begins");Set(ui,"skipOutcome",ScenarioOneOutcome.Completed);
                        while(ui.Skipping)yield return null;CadenceParity.RejectWrongStopReason(ui,24,25,"An objective was achieved.");DestinationAck();
                        Debug.Log("CADENCE408_OBJECTIVE_ISOLATED PASS genuine achieved objective; only test-side skipOutcome snapshot suppresses higher-priority completion stop");
                    }
                }
                else Check(false,"authentic boundary eligibility",ui.SkipProblem());
            } else {Check(manager.AdvanceYear(),"authentic progression year "+(year+1));ui.ShowReview();ui.AcknowledgeAnnualReview();}
            yield return null;
        }
        if(boundary==99) {ui.ShowWorkPlan();ui.Learning.Refresh(true);CadenceParity.Reject(ui,"Year advance is unavailable.");}
    }

    IEnumerator MfmCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        var coverage=manager.ReadCoverage(out string problem);Check(coverage!=null&&coverage.Valid,"MFM genuine coverage",problem);
        var stems=Live().Select(t=>new ScenarioOneCoverage.Stem(t.TreeId,t.transform.position.x,t.transform.position.z,t.Diameter,t.IsLiving,t.Species!=null?t.Species.SpeciesId:"",t.IsCropTree,t.IsMarkedForFell)).ToList();
        var suggestions=ScenarioOneCropTreeCandidates.Suggest(coverage,stems,stems.Where(t=>t.Crop).Select(t=>t.Id).ToList(),null,null,ScenarioOnePrescriptionCalibration.V1);
        Check(suggestions.Valid,"MFM suggestions valid",suggestions.Problem);
        foreach(var candidate in suggestions.Candidates)ui.Marking.Mark(Live().Single(t=>t.TreeId==candidate.TreeId),TreeMarkType.CropTree,false);
        var units=coverage.Areas.Where(a=>!a.IsSource&&a.State==ScenarioOneCoverage.State.Covered).Take(1).Select(a=>a.Id).ToList();
        var created=manager.CreatePrescription(units);Check(created.Succeeded,"MFM genuine prescription created",created.Message);
        var approved=manager.ApprovePrescription(created.PrescriptionId);Check(approved.Succeeded,"MFM genuine prescription approved",approved.Message);
        ui.ShowWorkPlan();ui.Learning.Refresh(true);CadenceParity.Reject(ui,"Pending and Approved");Debug.Log("CADENCE408_MFM genuine Pending rejected");Check(ui.ApproveWorkPlan(),"MFM work plan approved");CadenceParity.Reject(ui,"Pending and Approved");Debug.Log("CADENCE408_MFM genuine Approved rejected");Check(ui.AdvanceFromWorkPlan(),"MFM approved work executed");yield return Settle();ui.AcknowledgeAnnualReview();ui.CloseAll();ui.CloseHelp();ui.ShowWorkPlan();ui.Learning.Refresh(true);
        int selectedRack=((StandMapView)Field(ui,"map")).RackEditor.SelectedRackId;int confirmed=((RackTool)Field(ui,"rackTool")).ConfirmedCount;var before=saves.CaptureData();string prescriptions=JsonUtility.ToJson(before.scenarioOne);var orders=before.scenarioOne.workOrders.Select(o=>JsonUtility.ToJson(o)).ToArray();var marks=ui.Marking.GetMarkedIds().OrderBy(x=>x).ToArray();
        var inputs=InputStates();Check(ui.BeginSkip(ui.SkipTarget(out _)),"historical MFM skip begins");while(ui.Skipping)yield return null;var after=saves.CaptureData();LogInputs("historical MFM",inputs);
        Check(before.scenarioOne.prescriptions.Select(o=>JsonUtility.ToJson(o)).SequenceEqual(after.scenarioOne.prescriptions.Select(o=>JsonUtility.ToJson(o))),"historical prescriptions unchanged");Check(selectedRack==((StandMapView)Field(ui,"map")).RackEditor.SelectedRackId&&confirmed==((RackTool)Field(ui,"rackTool")).ConfirmedCount,"MFM rack selection and 3D confirmation unchanged");
        Check(orders.SequenceEqual(after.scenarioOne.workOrders.Select(o=>JsonUtility.ToJson(o)))&&!manager.WorkOrders.Any(o=>o.IsOpen)&&marks.SequenceEqual(ui.Marking.GetMarkedIds().OrderBy(x=>x)),"no automatic order or Fell mark");
    }

    IEnumerator NativeLocks(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        Check(Application.persistentDataPath.StartsWith("/media/jer/Files/CCF/Verification/CadenceV1-FixedQual-408-"),"native hotkey save isolation");
        string path=Path.Combine(Application.persistentDataPath,"forest-save.json");string before=File.Exists(path)?Convert.ToBase64String(File.ReadAllBytes(path)):"ABSENT";
        saves.Save();before=Convert.ToBase64String(File.ReadAllBytes(path));var writeTime=File.GetLastWriteTimeUtc(path);int worldVersion=manager.TeachingWorldVersion;string world=JsonUtility.ToJson(saves.CaptureData());
        var inputs=InputStates();Check(ui.BeginSkip(ui.SkipTarget(out _)),"native hotkey skip begins");Set(ui,"skipTarget",manager.CurrentEcologicalYear+5);
        Check(inputs.Keys.All(b=>!b.enabled),"all nine disabled before OS save/load/menu keys");
        holdTicks=true;yield return native.Send(RackAuthoringNativeInput.Op.Key("F5"),RackAuthoringNativeInput.Op.Key("F9"),RackAuthoringNativeInput.Op.Key("F10"));
        Check(ui.Skipping&&!StandaloneSessionMenu.IsOpen,"OS keys leave skip running and menu unavailable");
        Check(before==(File.Exists(path)?Convert.ToBase64String(File.ReadAllBytes(path)):"ABSENT"),"OS save/load keys leave slot bytes unchanged");
        Check(File.GetLastWriteTimeUtc(path)==writeTime&&manager.TeachingWorldVersion==worldVersion&&JsonUtility.ToJson(saves.CaptureData())==world,"OS save/load keys neither write nor reload the actual isolated world");holdTicks=false;Set(ui,"cancelSkip",true);while(ui.Skipping)yield return null;
        Check(inputs.All(p=>p.Key.enabled==p.Value),"native hotkey exit restores nine inputs");
    }
    IEnumerator LossCase(ForestSaveData state,bool loseManager) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);var inputs=InputStates();Check(ui.BeginSkip(ui.SkipTarget(out _)),"loss begin");
        try {Set(ui,loseManager?"manager":"ecology",null);while(ui.Skipping)yield return null;Check(inputs.All(p=>p.Key.enabled==p.Value),"loss restores all inputs");LogInputs("reference-loss manager="+loseManager,inputs);}
        finally {Set(ui,"manager",manager);Set(ui,"ecology",FindFirstObjectByType<ForestEcologyController>());manager.SetModalOpen(false);}
    }
    IEnumerator OneYearClear(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);Check(ui.BeginSkip(ui.SkipTarget(out _)),"one-year clearing begins");while(ui.Skipping)yield return null;
        ui.AcknowledgeAnnualReview();ui.ShowWorkPlan();Check(ui.AdvanceFromWorkPlan(),"genuine one-year after skip");yield return Settle();
        var label=(Label)Field(Field(ui,"review"),"skipSummary");Check(label.text==""&&label.resolvedStyle.display==DisplayStyle.None,"one-year clears summary and layout");
    }
    IEnumerator TimingCase(ForestSaveData state,int n) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        if(n==4) {Plan(Live().First(t=>!t.IsCropTree&&!Bay().Contains(t.transform.position.x,t.transform.position.z)));Check(ui.ApproveWorkPlan()&&ui.AdvanceFromWorkPlan(),"timing genuine felling");yield return Settle();ui.AcknowledgeAnnualReview();ui.CloseAll();ui.CloseHelp();ui.ShowWorkPlan();ui.Learning.Refresh(true);}
        int start=manager.CurrentEcologicalYear;int target=ui.SkipTarget(out _);double time=Time.realtimeSinceStartupAsDouble;
        Check(ui.BeginSkip(target),"timing begins");if(n==5)Set(ui,"skipTarget",start+5);
        while(ui.Skipping)yield return null;
        Debug.Log("CADENCE408_TIMING n="+n+" actual="+(manager.CurrentEcologicalYear-start)+" seconds="+(Time.realtimeSinceStartupAsDouble-time).ToString("F6")+" stressOverride="+(n==5));Check(manager.CurrentEcologicalYear==start+n,"timing full range");
    }

    IEnumerator LessonCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);var inputs=InputStates();int start=manager.CurrentEcologicalYear;
        Check(ui.BeginSkip(ui.SkipTarget(out _)),"lesson fixture begins");
        Check(manager.TryPurchaseStock("beech-sapling",1)&&manager.TryDesignatePlanting("beech-sapling",22),"real replanting designated between ticks");
        Check(manager.ApprovePendingWork(),"real replanting approved for resolver injection");
        var order=manager.WorkOrders.Last(o=>o.IsOpen&&o.type==ScenarioWorkType.PlantJuvenile);
        typeof(ScenarioOneManager).GetMethod("ResolveOrder",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(manager,new object[]{order,new ScenarioAnnualReport{year=start}});
        Check(order.status==ScenarioWorkStatus.Completed,"real replanting resolver completed");
        var ignored=ui.RackPlanProblem;ui.Learning.Refresh(true);
        Debug.Log("CADENCE408_LESSON_INJECTION real purchased/designated/approved planting through production ResolveOrder at current year; no lesson-field/event/save-JSON writes; stage="+ui.Learning.CurrentTopicIndex);
        Check(ui.Learning.CurrentTopicIndex!=7,"injection genuinely changed derived lesson before next tick");
        while(ui.Skipping)yield return null;
        Check(manager.CurrentEcologicalYear==start&&inputs.All(p=>p.Key.enabled==p.Value),"lesson changed before any next tick and inputs restored");
        CadenceParity.RejectWrongStopReason(ui,start,start,"The lesson changed.");DestinationAck();
    }
    IEnumerator UnmarkDiagnostic(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);int start=manager.CurrentEcologicalYear;
        Check(ui.BeginSkip(ui.SkipTarget(out _)),"unmark diagnostic begin");
        while(ui.Skipping&&manager.CurrentEcologicalYear==start)yield return null;
        ui.Marking.Unmark(Live().First(t=>t.IsCropTree),false);ui.Learning.Refresh(true);
        Debug.Log("CADENCE408_UNMARK injectionYear="+manager.CurrentEcologicalYear+" stageAfterRefresh="+ui.Learning.CurrentTopicIndex);
        while(ui.Skipping)yield return null;
        Debug.Log("CADENCE408_UNMARK destination="+manager.CurrentEcologicalYear+" summary="+((Label)Field(Field(ui,"review"),"skipSummary")).text);
        Check(ui.Learning.CurrentTopicIndex==7,"unmark does not change completed historical lesson");
    }
    IEnumerator ModalDiagnostic(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);int start=manager.CurrentEcologicalYear;
        Check(ui.BeginSkip(ui.SkipTarget(out _)),"modal diagnostic begin");
        while(ui.Skipping&&manager.CurrentEcologicalYear==start)yield return null;
        ui.Help.Show(MenuHelpView.Menu.WorkPlan);
        Debug.Log("CADENCE408_MODAL injectionYear="+manager.CurrentEcologicalYear+" open="+ui.Help.IsOpen+" preStop="+typeof(ScenarioOneUiRoot).GetMethod("SkipStopReason",BindingFlags.Instance|BindingFlags.NonPublic).Invoke(ui,null));
        while(ui.Skipping)yield return null;
        Debug.Log("CADENCE408_MODAL destination="+manager.CurrentEcologicalYear+" summary="+((Label)Field(Field(ui,"review"),"skipSummary")).text);
        CadenceParity.RejectWrongStopReason(ui,start,manager.CurrentEcologicalYear,"Finish the open question or editing action first.");ui.CloseHelp();
    }

    void LogInputs(string context,Dictionary<Behaviour,bool> prior) {
        foreach(var group in prior.GroupBy(p=>p.Key.GetType().Name))Debug.Log("CADENCE408_RESTORE context="+context+" type="+group.Key+" count="+group.Count()+" restored="+group.All(p=>p.Key!=null&&p.Key.enabled==p.Value));
        Check(prior.Keys.Select(b=>b.GetType().Name).Distinct().Count()==9,"fixture includes all nine input types",context);
        Check(prior.All(p=>p.Key!=null&&p.Key.enabled==p.Value),"full nine-type restoration",context);
    }
    void DestinationAck() {
        int count=manager.ManagementEvents.Count(e=>e.eventType==ScenarioManagementEventType.AnnualReviewAcknowledged);bool seen=manager.AnnualReviewSeen;
        ui.AcknowledgeAnnualReview();ui.AcknowledgeAnnualReview();
        int delta=manager.ManagementEvents.Count(e=>e.eventType==ScenarioManagementEventType.AnnualReviewAcknowledged)-count;
        Debug.Log("CADENCE408_ACK year="+manager.CurrentEcologicalYear+" alreadyRead="+seen+" delta="+delta);
        Check(delta==(seen?0:1),"genuine destination acknowledgment exactly once (zero for an already-read zero-tick destination)");
    }
    IEnumerator ReviewedCycles(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);
        Check(ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(manager.ManagementEvents,manager.WorkOrders),"genuine two-cycle chronology");
        Check(ui.BeginSkip(ui.SkipTarget(out _)),"reviewed-cycle skip begins");while(ui.Skipping)yield return null;
        Check(ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(manager.ManagementEvents,manager.WorkOrders),"skip retains genuine two-cycle chronology");DestinationAck();
    }
    IEnumerator OldCase(ForestSaveData original) {
        Check(original.scenarioOne.managementEvents.All(e=>e.eventType!=ScenarioManagementEventType.AnnualReviewAcknowledged),"fresh authentic world has no cadence/review history");
        yield return Restore(original);ui.ShowWorkPlan();ui.Learning.Refresh(true);CadenceParity.Reject(ui,"Finish the current lesson");
        var old=ScenarioReferenceArchive.Load().AtYear(20).world;yield return Restore(old);ui.ShowWorkPlan();ui.Learning.Refresh(true);CadenceParity.Reject(ui,"Finish the current lesson");
        ui.CloseAll();Check(manager.AdvanceYear(),"genuine old save ordinary advance still works");
        Debug.Log("CADENCE408_OLD version="+old.version+" ordinaryDestination="+manager.CurrentEcologicalYear);
    }
    IEnumerator RackProblemCase(ForestSaveData state,bool approval) {
        string stagePath=Path.Combine(output,"stage-"+(approval?0:2)+".json");
        Check(File.Exists(stagePath),"authentic rack problem stage snapshot exists",stagePath);
        yield return Restore(JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(stagePath)));ui.ShowWorkPlan();ui.Learning.Refresh(true);
        if(approval) {
            var planner=new RackPlanner(manager,ui.Ecology);planner.Refresh();
            foreach(float line in planner.LayoutLines) {
                Check(RackGeometry.ClipLineToSquare(new RackPoint(0,line),0,planner.HalfExtentM,out RackPoint start,out RackPoint end),"approval fixture rack clips");
                Check(manager.AddRack(start,end).Succeeded,"approval fixture real rack draft");
            }
            Check(manager.ConfirmRackPlan().Succeeded,"approval fixture real rack plan confirmed");ui.ShowWorkPlan();ui.Learning.Refresh(true);
            string problem=ui.RackApprovalProblem;
            Debug.Log("CADENCE408_RACK_APPROVAL underlying="+problem+" skip="+ui.SkipProblem());
            Check(!string.IsNullOrEmpty(problem),"authentic rack-approval problem present");CadenceParity.Reject(ui,"Finish the current lesson");
        } else {
            for(int i=0;i<100 && string.IsNullOrEmpty(ui.RackPlanProblem);i++) {
                var candidate=Candidates().FirstOrDefault();if(candidate==null)break;Plan(candidate);
            }
            Debug.Log("CADENCE408_RACK_PLAN underlying="+ui.RackPlanProblem+" skip="+ui.SkipProblem());
            Check(!string.IsNullOrEmpty(ui.RackPlanProblem),"authentic hard rack-plan problem present");CadenceParity.Reject(ui,"Finish the current lesson");
        }
    }

    IEnumerator CashOutcomeCase(ForestSaveData state) {
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);int start=manager.CurrentEcologicalYear;
        Set(manager,"cashCents",0L);Check(manager.AdvanceYear(),"direct zero-cash tick succeeds");var direct=saves.CaptureData();Check(manager.Outcome==ScenarioOneOutcome.Failed,"ordinary evaluation records genuine Failed outcome");
        yield return Restore(state);ui.ShowWorkPlan();ui.Learning.Refresh(true);var inputs=InputStates();Check(ui.BeginSkip(ui.SkipTarget(out _)),"cash outcome skip begins");Set(manager,"cashCents",0L);
        while(ui.Skipping)yield return null;var actual=saves.CaptureData();ScenarioReferenceArchive.Canonicalize(direct);ScenarioReferenceArchive.Canonicalize(actual);
        Check(JsonUtility.ToJson(direct)==JsonUtility.ToJson(actual),"genuine Failed outcome full JSON direct parity");
        Debug.Log("CADENCE408_FAILED condition=injected cash zero; production annual evaluation event/outcome year="+manager.CurrentEcologicalYear+" direct="+ScenarioReferenceArchive.CurrentWorldHash(direct)+" skip="+ScenarioReferenceArchive.CurrentWorldHash(actual));
        CadenceParity.RejectWrongStopReason(ui,start,start+1,"Scenario outcome changed.");LogInputs("Failed-cash-zero",inputs);DestinationAck();
    }

}
#endif
