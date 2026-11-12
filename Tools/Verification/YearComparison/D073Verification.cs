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
using System.IO.Compression;
using System.Text;

// Disposable T-03 gate: real Rack V2 access, marks/orders, UI approval/advance, mortality and in-memory reload.
// Mortality is a deliberately forced fixture using ApplyMortality, not a claim about storm frequency or ecology calibration.
// Disk writes are restricted to the driver-isolated config; no scene saves. Remove staged sources and metas.
public static class D073Verification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");

        EditorApplication.isPlaying = true;
    }
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Contains("D073Verification.Begin"))
            new GameObject("Disposable T03 cap gate").AddComponent<D073Runner>();
    }
}

[DefaultExecutionOrder(-10000)]
public sealed class D073Runner : MonoBehaviour
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
        evidence.Add(line); Debug.Log("D073_CHECK " + line);
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
        Debug.Log(failure == null && caseFailures==0 ? $"D073_PASS checks={checks} shots={shots}" : "D073_FAIL cases="+caseFailures+" "+failure);
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
        Check(Application.persistentDataPath.StartsWith("/media/jer/Files/CCF/Verification/D073-YearComparison-417-"),"isolated host save/Prefs custody",Application.persistentDataPath);
        var original=saves.CaptureData();
        string resume=Environment.GetEnvironmentVariable("CCF_D073_RESUME_HISTORY");
        if(!string.IsNullOrEmpty(resume)) {
            try {
                var resumed=JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(resume));
                Check(resumed.ecologicalYear==6&&resumed.scenarioOne.firstGuidedRackReviewComplete,"verified authentic runtime1 Year6 history fixture");
                foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080)})
                    yield return Case("AC9/native-"+size.x+"x"+size.y,()=>NativeSize(resumed,size));
                foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080)}) {
                    Cadence408GameSize.SetGameSize(size.x,size.y);yield return Settle();
                    yield return Case("AC4/rendered-negative-"+size.x+"x"+size.y,()=>NegativeViews(resumed));
                }
            } finally {Check(saves.LoadData(original,false),"restore original disposable world after resumed UI checks");}
            yield break;
        }
        yield return Case("AC1/no-completed-year",Empty);
        yield return Case("provenance/real-types",Provenance);
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
        File.WriteAllText(Path.Combine(output,"authentic-stage7.json"),JsonUtility.ToJson(authentic));
        yield return Case("AC8/authentic-one-year-vs-long",()=>HistoryParity(authentic));
        var history=saves.CaptureData();
        yield return Case("AC7/baseline-versus-card",()=>ReadOnlyAB(history));
        yield return Case("AC12/actual-Work-column",()=>WorkParity(history));
        yield return Case("AC10/current-disk",()=>CurrentDisk(history));
        yield return Case("AC10/genuine-v9-Legacy40",()=>OldSave(history));
        yield return Case("AC10/frozen-reference",()=>Reference(history));
        foreach(var size in new[]{new Vector2Int(1280,720),new Vector2Int(1920,1080)})
            yield return Case("AC9/native-"+size.x+"x"+size.y,()=>NativeSize(history,size));
        yield return Case("AC4/rendered-negative-controls",()=>NegativeViews(history));
        } finally { Check(saves.LoadData(original,false),"restore original disposable world"); }
    }
    void Set(object o,string n,object v)=>o.GetType().GetField(n,BindingFlags.NonPublic|BindingFlags.Instance).SetValue(o,v);
    ForestSaveData Clone(ForestSaveData d)=>JsonUtility.FromJson<ForestSaveData>(JsonUtility.ToJson(d));
    string Canon(ForestSaveData d) { d=Clone(d);ScenarioReferenceArchive.Canonicalize(d);return JsonUtility.ToJson(d); }
    string Hash(ForestSaveData d)=>ScenarioReferenceArchive.CurrentWorldHash(Clone(d));
    AnnualReviewView Review= null;
    AnnualReviewView R => Review??(Review=(AnnualReviewView)Field(ui,"review"));
    ScenarioOneYearComparisonText.Card Card(int year)=>ScenarioOneYearComparisonText.Describe(ScenarioOneManagementFollowonReadout.Build(manager.HistoryYears,year),year,ScenarioOneUiFacts.SpeciesName);
    string Cards()=>string.Join("\n----\n",manager.HistoryYears.Select(y=>(string)typeof(YearComparisonVerification).GetMethod("Dump",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,new object[]{Card(y.Year)})));
    IEnumerator Case(string name,Func<IEnumerator> action) {
        int start=checks;Exception failure=null;var stack=new Stack<IEnumerator>();stack.Push(action());
        while(stack.Count>0) {
            bool more;object next=null;
            try {more=stack.Peek().MoveNext();if(more)next=stack.Peek().Current;}
            catch(Exception e){failure=e;break;}
            if(!more){stack.Pop();continue;}if(next is IEnumerator child)stack.Push(child);else yield return next;
        }
        if(failure!=null)caseFailures++;
        string line="D073_CASE "+name+" "+(failure==null?"PASS":"FAIL")+" checks="+(checks-start)+" "+failure;
        evidence.Add(line);Debug.Log(line);File.WriteAllLines(Path.Combine(output,"checks.txt"),evidence);
        if(failure!=null&&(name.StartsWith("AC7/")||name.StartsWith("AC9/")||name.StartsWith("AC10/")))throw failure;
    }
    IEnumerator Empty() {
        ui.ShowReview();yield return Settle();R.Refresh(true);
        Check(manager.HistoryYears.Count==0,"fresh world has no completed year");
        Check(R.Root.Q("annual-year-comparison")==null,"AC1 actual empty review has no card");
        yield return Shot("empty-no-completed-year");ui.CloseAll();
    }
    IEnumerator Provenance() {
        Check(YearComparisonVerification.Main(new[]{"all"})==0,"132 mapper controls on actual Unity production types");
        var years=(YearHistoryView[])typeof(YearComparisonVerification).GetMethod("CleanHistory",BindingFlags.NonPublic|BindingFlags.Static).Invoke(null,null);
        var e=years[0].ResolvedWork[1].Event;e.workOrderId=years[0].ResolvedWork[0].Event.workOrderId;
        var c=ScenarioOneYearComparisonText.Describe(ScenarioOneManagementFollowonReadout.Build(years,3),3,ScenarioOneUiFacts.SpeciesName);
        Check(c.Rows.Single(r=>r.Key=="work-ambiguous").Text.StartsWith("2 work record(s)"),"duplicate order IDs excluded in Unity real-types control");
        yield break;
    }
    IEnumerator HistoryParity(ForestSaveData authentic) {
        yield return Restore(Clone(authentic));ui.ShowWorkPlan();ui.Learning.Refresh(true);
        int start=manager.CurrentEcologicalYear,target=ui.SkipTarget(out _);
        Check(string.IsNullOrEmpty(ui.SkipProblem())&&target>start,"authentic eligible skip",ui.SkipProblem()+" target="+target);
        string baseline=Canon(saves.CaptureData());
        for(int year=start+1;year<=target;year++) {
            ui.ShowWorkPlan();Check(ui.AdvanceFromWorkPlan(),"actual one-year UI advance",year.ToString());yield return Settle();
            if(year<target)ui.AcknowledgeAnnualReview();
        }
        var manual=saves.CaptureData();string manualCards=Cards();
        File.WriteAllText(Path.Combine(output,"manual-final.json"),JsonUtility.ToJson(manual));File.WriteAllText(Path.Combine(output,"manual-cards.json"),manualCards);
        yield return Restore(Clone(authentic));ui.ShowWorkPlan();ui.Learning.Refresh(true);
        Check(Canon(saves.CaptureData())==baseline,"identical true pre-advance world");
        Check(ui.BeginSkip(target),"actual long advance begins");while(ui.Skipping)yield return null;yield return Settle();
        var skipped=saves.CaptureData();string skipCards=Cards();
        File.WriteAllText(Path.Combine(output,"skip-final.json"),JsonUtility.ToJson(skipped));File.WriteAllText(Path.Combine(output,"skip-cards.json"),skipCards);
        Check(manager.CurrentEcologicalYear==target,"long advance exact target",target.ToString());
        Check(manualCards==skipCards,"AC8 every dated card identical after actual one-year versus long advance",start+"→"+target);
        Check(manual.scenarioOne.annualReports.Select(r=>JsonUtility.ToJson(r)).SequenceEqual(skipped.scenarioOne.annualReports.Select(r=>JsonUtility.ToJson(r))),"same annual report context");
        Debug.Log("D073_PARITY manualHash="+Hash(manual)+" skipHash="+Hash(skipped)+" cardsExact=True; intermediate genuine acknowledgement events differ as expected");
        foreach(var y in manager.HistoryYears)Debug.Log("D073_CONTEXT year="+y.Year+" events="+y.Events.Count+" snapshot="+y.Snapshot?.year);
    }
    void ClickCallback(Button b) {Check(b!=null,"baseline/programmatic action exists");typeof(Clickable).GetMethod("Invoke",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(b.clickable,new object[]{null});}
    IEnumerator ReadOnlyAB(ForestSaveData history) {
        yield return Restore(Clone(history));ui.ShowReview();yield return Settle();R.Refresh(true);
        var before=saves.CaptureData();string raw=Canon(before),hash=Hash(before);int events=before.scenarioOne.managementEvents.Count;
        var firstCard=R.Root.Q("annual-year-comparison");for(int i=0;i<30;i++)yield return null;
        Check(ReferenceEquals(firstCard,R.Root.Q("annual-year-comparison")),"unchanged per-frame refresh keeps same card instance");
        R.TryShowYear(manager.HistoryYears[0].Year);yield return Settle();
        Check(((VisualElement)Field(R,"acknowledgement")).resolvedStyle.display==DisplayStyle.None,"earlier-year footer hidden");
        ClickCallback(R.Root.Q<Button>("annual-review-acknowledge"));
        Check(events==saves.CaptureData().scenarioOne.managementEvents.Count,"earlier hidden ACK callback guarded");
        var fold=R.Root.Q<Foldout>("annual-year-comparison-species");if(fold!=null){fold.value=true;fold.value=false;}
        R.Root.Q<Toggle>("annual-show-data-sources").value=true;
        ui.CloseAll();ui.ShowReview();yield return Settle();
        Check(Canon(saves.CaptureData())==raw&&Hash(saves.CaptureData())==hash,"AC7 browsing/toggle/open/close exact saved world and hash nonmutation");
        ClickCallback(R.Root.Q<Button>("annual-review-acknowledge"));yield return Settle();var with=saves.CaptureData();
        yield return Restore(Clone(history));ui.ShowReview();yield return Settle();
        var baseline=new D073BaselineAnnualReviewView(ui);baseline.ShowLatest();baseline.Refresh(true);baseline.TryShowYear(manager.HistoryYears[0].Year);
        ClickCallback(baseline.Root.Q<Button>("annual-review-acknowledge"));baseline.ShowLatest();baseline.Refresh(true);
        ui.CloseAll();ui.ShowReview();baseline.ShowLatest();baseline.Refresh(true);ClickCallback(baseline.Root.Q<Button>("annual-review-acknowledge"));yield return Settle();
        var without=saves.CaptureData();
        Check(Canon(with)==Canon(without)&&Hash(with)==Hash(without),"AC7 card vs exact pre-D073 view actions including ACK same full state/hash");
        Check(with.scenarioOne.managementEvents.Count==events+1&&with.scenarioOne.managementEvents.Last().eventType==ScenarioManagementEventType.AnnualReviewAcknowledged,"ACK only existing one acknowledgement event");
        File.WriteAllText(Path.Combine(output,"AC7-with-card.json"),JsonUtility.ToJson(with));File.WriteAllText(Path.Combine(output,"AC7-without-card.json"),JsonUtility.ToJson(without));
        yield return Restore(Clone(history));
    }
    IEnumerator WorkParity(ForestSaveData history) {
        yield return Restore(Clone(history));
        foreach(var y in manager.HistoryYears.Where(y=>y.Year<manager.HistoryYears.Last().Year)) {
            var read=ScenarioOneManagementFollowonReadout.Build(manager.HistoryYears,y.Year);var c=Card(y.Year);
            foreach(var type in new[]{ScenarioWorkType.FellTree,ScenarioWorkType.PlantJuvenile,ScenarioWorkType.PruneTree,ScenarioWorkType.SalvageDeadwood}) {
                int raw=y.ResolvedWork.Count(w=>w.Event.taskType==type&&w.Event.outcome==ScenarioManagementOutcome.Succeeded);
                int valid=read.RecordedWork.Count(w=>w.Year==y.Year&&w.TaskType==type&&w.Result==ScenarioOneManagementFollowonReadout.Resolution.Succeeded&&w.Link!=ScenarioOneManagementFollowonReadout.OrderLink.AmbiguousIdentity);
                Check(raw==valid,"AC12 authentic shared event counts equal",y.Year+" "+type+"="+raw);
            }
            Debug.Log("D073_WORK year="+y.Year+" reportRemoval="+(y.Report?.regenerationRemovalTasks??0)+" eventRemoval="+y.ResolvedWork.Count(w=>w.Event.taskType==ScenarioWorkType.RemoveRegeneration&&w.Event.outcome==ScenarioManagementOutcome.Succeeded));
        }
    }
    IEnumerator CurrentDisk(ForestSaveData history) {
        yield return Restore(Clone(history));var before=saves.CaptureData();saves.Save();saves.Load();yield return Settle();
        Check(Canon(before)==Canon(saves.CaptureData()),"current disk save/load exact canonical state");
        Check(Hash(before)==Hash(saves.CaptureData()),"current disk save/load world hash");
    }
    IEnumerator OldSave(ForestSaveData history) {
        var old=JsonUtility.FromJson<ForestSaveData>(File.ReadAllText(Environment.GetEnvironmentVariable("CCF_D073_OLD_SAVE")));
        Check(old.version==9&&old.ecologicalYear==39,"genuine unmodified archived host v9/year39");
        yield return Restore(old);Check(saves.CaptureData().standGeometryModel==0,"genuine old save loads Legacy40");
        Check(manager.CurrentEcologicalYear==39,"old save year retained");
        var loaded=saves.CaptureData();var ids=old.trees.Select(t=>t.treeId).OrderBy(x=>x).ToArray();
        Check(ids.SequenceEqual(loaded.trees.Select(t=>t.treeId).OrderBy(x=>x)),"genuine old save all tree identities retained");
        Check(loaded.standGeometryModel==0,"no old save geometry migration");
        ui.ShowReview();yield return Settle();var frozen=Canon(saves.CaptureData());string cards=Cards();
        foreach(var y in manager.HistoryYears)R.TryShowYear(y.Year);
        ui.CloseAll();Check(frozen==Canon(saves.CaptureData()),"genuine old history browsing leaves world unchanged");
        File.WriteAllText(Path.Combine(output,"genuine-v9-loaded.json"),JsonUtility.ToJson(loaded));File.WriteAllText(Path.Combine(output,"genuine-v9-cards.txt"),cards);
        Check(manager.AdvanceYear(),"genuine Legacy40 ordinary annual advance still works");
        yield return Restore(Clone(history));
    }
    IEnumerator Reference(ForestSaveData history) {
        ReferenceArchiveIntegrity.Verify();
        var archive=ScenarioReferenceArchive.Load();Check(archive.AtYear(100).worldHash=="7AD177B3CC2F73C7","frozen original Y100 pinned");
        yield return Restore(Clone(history));string before=Canon(saves.CaptureData());
        foreach(int year in new[]{0,20,50,100}) {
            Check(manager.TryBeginReferencePreview(year),"frozen reference actual preview opens",year.ToString());yield return Settle();
            Check(manager.ReferencePreviewActive&&manager.CurrentEcologicalYear==year,"reference preview state/year",year.ToString());
            var state=saves.CaptureData();Check(state.standGeometryModel==0,"reference remains Legacy40");
            string preview=Canon(state);ui.ShowReview();yield return Settle();foreach(var y in manager.HistoryYears)R.TryShowYear(y.Year);
            Check(Canon(saves.CaptureData())==preview,"reference review browse exact nonmutation",year.ToString());
            manager.EndReferencePreview();yield return Settle();Check(Canon(saves.CaptureData())==before,"reference return exact player world",year.ToString());
        }
    }
    readonly RackAuthoringNativeInput native=new RackAuthoringNativeInput();
    IEnumerator NativeClick(VisualElement element) {
        yield return Settle();Check(element!=null&&element.enabledInHierarchy,"native target present/enabled",element?.name);
        var center=element.worldBound.center;var root=ui.RootElement.worldBound;
        var view=EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));
        var prop=view.GetType().GetProperty("targetInParent",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
        Rect rect=prop!=null?(Rect)prop.GetValue(view):new Rect(0,21,view.position.width,view.position.height-21);
        int x=Mathf.RoundToInt(view.position.x+rect.x+center.x/root.width*rect.width),y=Mathf.RoundToInt(view.position.y+rect.y+center.y/root.height*rect.height);
        Debug.Log("D073_NATIVE target="+element.name+" text="+(element as TextElement)?.text+" panel="+element.worldBound+" window="+view.position+" targetRect="+rect+" desktop="+x+","+y);
        yield return native.Send(new RackAuthoringNativeInput.Op{op="absolute",x=x,y=y},RackAuthoringNativeInput.Op.Click());yield return Settle();
    }
    Button B(string text)=>R.Root.Query<Button>().ToList().FirstOrDefault(b=>b.text==text);
    IEnumerator Shot(string name) {
        yield return Settle();yield return new WaitForEndOfFrame();var img=ScreenCapture.CaptureScreenshotAsTexture();
        File.WriteAllBytes(Path.Combine(output,name+".png"),img.EncodeToPNG());Debug.Log("D073_SCREENSHOT "+name+" "+img.width+"x"+img.height);Destroy(img);shots++;
        File.WriteAllText(Path.Combine(output,name+"-labels.txt"),string.Join("\n",R.Root.Query<Label>().ToList().Select(l=>l.name+" | "+l.text+" | "+l.worldBound+" | "+l.resolvedStyle.display)));
    }
    void Fits(VisualElement element,string name) {
        var root=ui.RootElement.worldBound;var r=element.worldBound;
        Check(r.width>0&&r.xMin>=root.xMin-1&&r.xMax<=root.xMax+1&&r.yMin>=root.yMin-1&&r.yMax<=root.yMax+1,name+" inside game",r.ToString());
    }
    IEnumerator NativeSize(ForestSaveData history,Vector2Int size) {
        yield return Restore(Clone(history));Cadence408GameSize.SetGameSize(size.x,size.y);yield return Settle();ui.ShowReview();yield return Settle();R.Refresh(true);yield return Settle();
        R.Root.Q<Toggle>("annual-show-data-sources").value=false;yield return Settle();
        string tag="native-"+size.x+"x"+size.y;int latest=manager.HistoryYears.Last().Year;
        Check(Screen.width==size.x&&Screen.height==size.y,"exact requested rendered pixel resolution",Screen.width+"x"+Screen.height+"; logical panel="+ui.RootElement.worldBound);
        var sc=(ScrollView)Field(R,"scroll");sc.ScrollTo(R.Root.Q("annual-year-comparison"));yield return Settle();
        Check(Card(latest).Rows.Count==1&&R.Root.Q<Label>("annual-year-comparison-latest-year")!=null,"actual latest single hint");
        Fits(B("◀ Earlier year"),"year navigation");Fits(R.Root.Q<Button>("annual-review-acknowledge"),"latest ACK footer");yield return Shot(tag+"-latest-footer");
        string before=Canon(saves.CaptureData());int eventCount=saves.CaptureData().scenarioOne.managementEvents.Count;
        yield return NativeClick(B("◀ Earlier year"));Check((int)Field(R,"viewYear")==latest-1,"OS earlier selects previous year");
        while((int)Field(R,"viewYear")>3)yield return NativeClick(B("◀ Earlier year"));
        Check((int)Field(R,"viewYear")==3,"OS year selection reaches genuine planting-work Year3");
        Check(((VisualElement)Field(R,"acknowledgement")).resolvedStyle.display==DisplayStyle.None,"OS earlier cannot ACK latest");
        var species=R.Root.Q<Foldout>("annual-year-comparison-species");Check(species!=null&&!species.value,"species initially closed");sc.ScrollTo(species);yield return Settle();
        yield return NativeClick(species.Q<Toggle>());Check(species.value,"OS species foldout opens");
        yield return NativeClick(R.Root.Q<Toggle>("annual-show-data-sources"));Check(!R.Root.ClassListContains("hide-data-sources"),"OS sources shows chips");
        species=R.Root.Q<Foldout>("annual-year-comparison-species");Check(species.value,"species expansion survives source rebuild");
        var card=R.Root.Q("annual-year-comparison");sc.ScrollTo(card);yield return Shot(tag+"-earlier-sources");
        Check(card.parent==sc.contentContainer,"card exclusively inside existing scroll");
        var caveat=R.Root.Q<Label>("annual-year-comparison-caveat");sc.ScrollTo(caveat);yield return Settle();Fits(caveat,"caveat reachable");yield return Shot(tag+"-expanded-species-caveat");
        float scrollBefore=sc.scrollOffset.y;yield return NativeClick(caveat);yield return native.Send(RackAuthoringNativeInput.Op.Click(4),RackAuthoringNativeInput.Op.Click(4),RackAuthoringNativeInput.Op.Click(4));yield return Settle();
        Check(sc.scrollOffset.y<scrollBefore,"actual pointer wheel scroll changes position",scrollBefore+"→"+sc.scrollOffset.y);
        yield return NativeClick(R.Root.Q<Toggle>("annual-show-data-sources"));Check(R.Root.ClassListContains("hide-data-sources"),"OS sources hides chips");
        sc.ScrollTo(R.Root.Q("annual-year-comparison"));yield return Shot(tag+"-earlier-no-sources");
        while((int)Field(R,"viewYear")<latest)yield return NativeClick(B("Later year ▶"));Check((int)Field(R,"viewYear")==latest,"OS later returns latest");
        Check(Canon(saves.CaptureData())==before&&saves.CaptureData().scenarioOne.managementEvents.Count==eventCount,"all native view actions exact world/event nonmutation");
        yield return native.Send(RackAuthoringNativeInput.Op.Key("Escape"));yield return Settle();Check(Field(ui,"screen").ToString()=="None","OS Escape back to forest");
        ui.ShowReview();yield return Settle();Check((int)Field(R,"viewYear")==-1,"reopening follows latest");
        yield return NativeClick(R.Root.Q<Button>("annual-review-acknowledge"));Check(manager.AnnualReviewSeen,"OS actual latest ACK works");
        Check(saves.CaptureData().scenarioOne.managementEvents.Count==eventCount+1,"OS ACK records only existing acknowledgement event");yield return Shot(tag+"-acknowledged");
        yield return NativeClick(R.Root.Q<Button>("annual-review-walk"));Check(Field(ui,"screen").ToString()=="None","OS Back to forest button closes review");
    }
    IEnumerator NegativeViews(ForestSaveData history) {
        foreach(string mode in new[]{"missing-selected","missing-latest","ambiguous","species-one-sided"}) {
            var copy=Clone(history);int a=copy.scenarioOne.annualReports.First().year,b=copy.scenarioOne.annualReports.Last().year;
            var snaps=copy.scenarioOne.ecologicalSnapshots;
            if(mode=="missing-selected")snaps.RemoveAll(s=>s.year==a);
            if(mode=="missing-latest")snaps.RemoveAll(s=>s.year==b);
            if(mode=="ambiguous")snaps.First(s=>s.year==a).species.Add(JsonUtility.FromJson<ScenarioSpeciesOutcome>(JsonUtility.ToJson(snaps.First(s=>s.year==a).species.First())));
            if(mode=="species-one-sided")snaps.First(s=>s.year==a).species.RemoveAll(s=>s.speciesId=="beech");
            yield return Restore(copy);ui.ShowReview();R.TryShowYear(a);yield return Settle();R.Root.Q<Toggle>("annual-show-data-sources").value=true;yield return Settle();
            var card=R.Root.Q("annual-year-comparison");Check(card!=null,"negative actual view card exists",mode);
            if(mode=="species-one-sided") {var fold=card.Q<Foldout>();fold.value=true;Check(card.Query<Label>().ToList().Any(l=>l.text.Contains("not recorded")&&l.text.Contains("Year "+a)),"one-sided species absence is not zero");}
            else Check(card.Q<Label>("annual-year-comparison-"+(mode=="ambiguous"?"cannot-compare":mode))!=null,"negative exact gap row",mode);
            ((ScrollView)Field(R,"scroll")).ScrollTo(card);yield return Shot("negative-"+Screen.width+"x"+Screen.height+"-"+mode);
        }
        yield return Restore(Clone(history));
    }
}
#endif
